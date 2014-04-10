/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/24/2014
 * Time: 10:26 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PeterO.Mail
{
  internal sealed class BoundaryCheckerTransform : ITransform {
    private StreamWithUnget input;
    private byte[] buffer;
    private int bufferIndex;
    private int bufferCount;
    private bool started;
    private bool readingHeaders;
    private bool hasNewBodyPart;
    private List<string> boundaries;

    /// <summary>Not documented yet.</summary>
    /// <param name='size'>A 32-bit signed integer.</param>
    private void ResizeBuffer(int size) {
      if (this.buffer == null) {
        this.buffer = new byte[size + 10];
      } else if (size > this.buffer.Length) {
        byte[] newbuffer = new byte[size + 10];
        Array.Copy(this.buffer, newbuffer, this.buffer.Length);
        this.buffer = newbuffer;
      }
      this.bufferCount = size;
      this.bufferIndex = 0;
    }

    public BoundaryCheckerTransform(ITransform stream) {
      this.input = new StreamWithUnget(stream);
      this.boundaries = new List<string>();
      this.started = true;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='boundary'>A string object.</param>
    public void PushBoundary(string boundary) {
      this.boundaries.Add(boundary);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int ReadByte() {
      if (this.bufferIndex < this.bufferCount) {
        int ret = this.buffer[this.bufferIndex];
        ++this.bufferIndex;
        if (this.bufferIndex == this.bufferCount) {
          this.bufferCount = 0;
          this.bufferIndex = 0;
        }
        ret &= 0xff;
        return ret;
      }
      if (this.hasNewBodyPart) {
        return -1;
      }
      if (this.readingHeaders) {
        return this.input.ReadByte();
      }
      int c = this.input.ReadByte();
      if (c < 0) {
        this.started = false;
        return c;
      }
      if (c == '-' && this.started) {
        // Check for a boundary
        this.started = false;
        c = this.input.ReadByte();
        if (c == '-') {
          // Possible boundary candidate
          return this.CheckBoundaries(false);
        } else {
          this.input.Unget();
          return '-';
        }
      } else {
        this.started = false;
      }
      if (c == 0x0d) {
        c = this.input.ReadByte();
        if (c == 0x0a) {
          // Line break was read
          c = this.input.ReadByte();
          if (c == -1) {
            this.ResizeBuffer(1);
            this.buffer[0] = 0x0a;
            return 0x0d;
          } else if (c == 0x0d) {
            // Unget the CR, in case the next line is a boundary line
            this.input.Unget();
            this.ResizeBuffer(1);
            this.buffer[0] = 0x0a;
            return 0x0d;
          } else if (c != '-') {
            this.ResizeBuffer(2);
            this.buffer[0] = 0x0a;
            this.buffer[1] = (byte)c;
            return 0x0d;
          }
          c = this.input.ReadByte();
          if (c == -1) {
            this.ResizeBuffer(2);
            this.buffer[0] = 0x0a;
            this.buffer[1] = (byte)'-';
            return 0x0d;
          } else if (c == 0x0d) {
            // Unget the CR, in case the next line is a boundary line
            this.input.Unget();
            this.ResizeBuffer(2);
            this.buffer[0] = 0x0a;
            this.buffer[1] = (byte)'-';
            return 0x0d;
          } else if (c != '-') {
            this.ResizeBuffer(3);
            this.buffer[0] = 0x0a;
            this.buffer[1] = (byte)'-';
            this.buffer[2] = (byte)c;
            return 0x0d;
          }
          // Possible boundary candidate
          return this.CheckBoundaries(true);
        } else {
          this.input.Unget();
          return 0x0d;
        }
      } else {
        return c;
      }
    }

    private int CheckBoundaries(bool includeCrLf) {
      // Reached here when the "--" of a possible
      // boundary delimiter is read. We need to
      // check boundaries here in order to find out
      // whether to emit the CRLF before the "--".
      #if DEBUG
      if (this.bufferCount != 0) {
        throw new ArgumentException("this.bufferCount (" + Convert.ToString(
          (long)this.bufferCount,
          System.Globalization.CultureInfo.InvariantCulture) + ") is not equal to " + "0");
      }
      #endif

      bool done = false;
      while (!done) {
        done = true;
        int bufferStart = 0;
        if (includeCrLf) {
          this.ResizeBuffer(3);
          bufferStart = 3;
          // store LF, '-', and '-' in the buffer in case
          // the boundary check fails, in which case
          // this method will return CR
          this.buffer[0] = 0x0a;
          this.buffer[1] = (byte)'-';
          this.buffer[2] = (byte)'-';
        } else {
          bufferStart = 1;
          this.ResizeBuffer(1);
          this.buffer[0] = (byte)'-';
        }
        // Check up to 72 bytes (the maximum size
        // of a boundary plus 2 bytes for the closing
        // hyphens)
        int c;
        int bytesRead = 0;
        for (int i = 0; i < 72; ++i) {
          c = this.input.ReadByte();
          if (c < 0 || c >= 0x80 || c == 0x0d) {
            this.input.Unget();
            break;
          }
          ++bytesRead;
          // Console.Write("" + ((char)c));
          this.ResizeBuffer(bytesRead + bufferStart);
          this.buffer[bytesRead + bufferStart - 1] = (byte)c;
        }
        // Console.WriteLine("--" + (bytesRead));
        // NOTE: All boundary strings are assumed to
        // have only ASCII characters (with values
        // less than 128). Check boundaries from
        // top to bottom in the stack.
        string matchingBoundary = null;
        int matchingIndex = -1;
        for (int i = this.boundaries.Count - 1; i >= 0; --i) {
          string boundary = this.boundaries[i];
          // Console.WriteLine("Check boundary " + (boundary));
          if (!String.IsNullOrEmpty(boundary) && boundary.Length <= bytesRead) {
            bool match = true;
            for (int j = 0; j < boundary.Length; ++j) {
              if ((boundary[j] & 0xff) != (int)(this.buffer[j + bufferStart] & 0xff)) {
                match = false;
              }
            }
            if (match) {
              matchingBoundary = boundary;
              matchingIndex = i;
              break;
            }
          }
        }
        if (matchingBoundary != null) {
          bool closingDelim = false;
          // Pop the stack until the matching body part
          // is on top
          while (this.boundaries.Count > matchingIndex + 1) {
            this.boundaries.RemoveAt(matchingIndex + 1);
          }
          // Boundary line found
          if (matchingBoundary.Length + 1 < bytesRead) {
            if (this.buffer[matchingBoundary.Length + bufferStart] == '-' &&
                this.buffer[matchingBoundary.Length + 1 + bufferStart] == '-') {
              closingDelim = true;
            }
          }
          // Clear the buffer, the boundary line
          // isn't part of any body data
          this.bufferCount = 0;
          this.bufferIndex = 0;
          if (closingDelim) {
            // Pop this entry, it's the top of the stack
            this.boundaries.RemoveAt(this.boundaries.Count - 1);
            if (this.boundaries.Count == 0) {
              // There's nothing else significant
              // after this boundary,
              // so return now
              return -1;
            }
            // Read to end of line. Since this is the last body
            // part, the rest of the data before the next boundary
            // is insignificant
            while (true) {
              c = this.input.ReadByte();
              if (c == -1) {
                // The body higher up didn't end yet
                throw new InvalidDataException("Premature end of message");
              } else if (c == 0x0d) {
                c = this.input.ReadByte();
                if (c == -1) {
                  // The body higher up didn't end yet
                  throw new InvalidDataException("Premature end of message");
                } else if (c == 0x0a) {
                  // Start of new body part
                  c = this.input.ReadByte();
                  if (c == -1) {
                    throw new InvalidDataException("Premature end of message");
                  } else if (c == 0x0d) {
                    // Unget the CR, in case the next line is a boundary line
                    this.input.Unget();
                  } else if (c != '-') {
                    // Not a boundary delimiter
                    continue;
                  }
                  c = this.input.ReadByte();
                  if (c == -1) {
                    throw new InvalidDataException("Premature end of message");
                  } else if (c == 0x0d) {
                    // Unget the CR, in case the next line is a boundary line
                    this.input.Unget();
                  } else if (c != '-') {
                    // Not a boundary delimiter
                    continue;
                  }
                  // Found the next boundary delimiter
                  done = false;
                  break;
                } else {
                  this.input.Unget();
                }
              }
            }
            if (!done) {
              // Recheck the next line for a boundary delimiter
              continue;
            }
          } else {
            // Read to end of line (including CRLF; the
            // next line will start the headers of the
            // next body part).
            while (true) {
              c = this.input.ReadByte();
              if (c == -1) {
                throw new InvalidDataException("Premature end of message");
              } else if (c == 0x0d) {
                c = this.input.ReadByte();
                if (c == -1) {
                  throw new InvalidDataException("Premature end of message");
                } else if (c == 0x0a) {
                  // Start of new body part
                  this.hasNewBodyPart = true;
                  return -1;
                } else {
                  this.input.Unget();
                }
              }
            }
          }
        }
        // Not a boundary, return CR (the
        // ReadByte method will then return LF,
        // the hyphens, and the other bytes
        // already read)
        return includeCrLf ? 0x0d : '-';
      }
      // Not a boundary, return CR (the
      // ReadByte method will then return LF,
      // the hyphens, and the other bytes
      // already read)
      return includeCrLf ? 0x0d : '-';
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int BoundaryCount() {
      return this.boundaries.Count;
    }

    /// <summary>Not documented yet.</summary>
    public void StartBodyPartHeaders() {
      #if DEBUG
      if (!this.hasNewBodyPart) {
        throw new ArgumentException("doesn't satisfy this.hasNewBodyPart");
      }
      if (this.readingHeaders) {
        throw new ArgumentException("doesn't satisfy !this.hasNewBodyPart");
      }
      if (!this.bufferCount.Equals(0)) {
        throw new ArgumentException("this.bufferCount (" + Convert.ToString((long)this.bufferCount, System.Globalization.CultureInfo.InvariantCulture) + ") is not equal to " + "0");
      }
      #endif

      this.readingHeaders = true;
      this.hasNewBodyPart = false;
    }

    /// <summary>Not documented yet.</summary>
    public void EndBodyPartHeaders() {
      #if DEBUG
      if (!this.readingHeaders) {
        throw new ArgumentException("doesn't satisfy this.readingHeaders");
      }
      if (!this.bufferCount.Equals(0)) {
        throw new ArgumentException("this.bufferCount (" + Convert.ToString((long)this.bufferCount, System.Globalization.CultureInfo.InvariantCulture) + ") is not equal to " + "0");
      }
      #endif

      this.readingHeaders = false;
      this.hasNewBodyPart = false;
    }

    /// <summary>Gets a value indicating whether a new body part was detected.</summary>
    /// <value>Whether a new body part was detected.</value>
    public bool HasNewBodyPart {
      get {
        return this.hasNewBodyPart;
      }
    }
  }
}
