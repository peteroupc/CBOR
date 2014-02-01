/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
using System.IO;
using System.Text;

namespace PeterO {
    /// <summary>Contains methods useful for reading and writing strings.
    /// It is designed to have no dependencies other than the basic runtime
    /// class library.</summary>
  public static class DataUtilities {
    private static int valueStreamedStringBufferLength = 4096;

    /// <summary>Generates a text string from a UTF-8 byte array.</summary>
    /// <param name='bytes'>A byte array containing text encoded in UTF-8.</param>
    /// <param name='replace'>If true, replaces invalid encoding with
    /// the replacement character (U + FFFD). If false, stops processing
    /// when invalid UTF-8 is seen.</param>
    /// <returns>A string represented by the UTF-8 byte array.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='bytes'/> is null.</exception>
    /// <exception cref='System.ArgumentException'>The string is not
    /// valid UTF-8 and <paramref name='replace'/> is false.</exception>
    public static string GetUtf8String(byte[] bytes, bool replace) {
      if (bytes == null) {
        throw new ArgumentNullException("bytes");
      }
      StringBuilder b = new StringBuilder();
      if (ReadUtf8FromBytes(bytes, 0, bytes.Length, b, replace) != 0) {
        throw new ArgumentException("Invalid UTF-8");
      }
      return b.ToString();
    }

    /// <summary>Generates a text string from a portion of a UTF-8 byte array.</summary>
    /// <param name='bytes'>A byte array containing text encoded in UTF-8.</param>
    /// <param name='offset'>Offset into the byte array to start reading.</param>
    /// <param name='bytesCount'>Length, in bytes, of the UTF-8 string.</param>
    /// <param name='replace'>If true, replaces invalid encoding with
    /// the replacement character (U + FFFD). If false, stops processing
    /// when invalid UTF-8 is seen.</param>
    /// <returns>A string represented by the UTF-8 byte array.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='bytes'/> is null.</exception>
    /// <exception cref='System.ArgumentException'>The portion of the
    /// byte array is not valid UTF-8 and <paramref name='replace'/> is false.</exception>
    public static string GetUtf8String(byte[] bytes, int offset, int bytesCount, bool replace) {
      StringBuilder b = new StringBuilder();
      if (ReadUtf8FromBytes(bytes, offset, bytesCount, b, replace) != 0) {
        throw new ArgumentException("Invalid UTF-8");
      }
      return b.ToString();
    }

    /// <summary>Encodes a string in UTF-8 as a byte array.</summary>
    /// <param name='str'>A text string.</param>
    /// <param name='replace'>If true, replaces unpaired surrogate code
    /// points with the replacement character (U + FFFD). If false, stops
    /// processing when an unpaired surrogate code point is seen.</param>
    /// <returns>The string encoded in UTF-8.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='str'/> is null.</exception>
    /// <exception cref='System.ArgumentException'>The string contains
    /// an unpaired surrogate code point and <paramref name='replace'/>
    /// is false, or an internal error occurred.</exception>
    public static byte[] GetUtf8Bytes(string str, bool replace) {
      try {
        using (MemoryStream ms = new MemoryStream()) {
          if (WriteUtf8(str, ms, replace) != 0) {
            throw new ArgumentException("Unpaired surrogate code point");
          }
          return ms.ToArray();
        }
      } catch (IOException ex) {
        throw new ArgumentException("I/O error occurred", ex);
      }
    }

    /// <summary>Calculates the number of bytes needed to encode a string
    /// in UTF-8.</summary>
    /// <returns>The number of bytes needed to encode the given string in
    /// UTF-8, or -1 if the string contains an unpaired surrogate code point
    /// and <paramref name='replace'/> is false.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='str'/> is null.</exception>
    /// <param name='str'>A String object.</param>
    /// <param name='replace'>A Boolean object.</param>
    public static long GetUtf8Length(String str, bool replace) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      long size = 0;
      for (int i = 0; i < str.Length; ++i) {
        int c = str[i];
        if (c <= 0x7F) {
          ++size;
        } else if (c <= 0x7FF) {
          size += 2;
        } else if (c <= 0xD7FF || c >= 0xE000) {
          size += 3;
        } else if (c <= 0xDBFF) {  // UTF-16 leading surrogate
          ++i;
          if (i >= str.Length || str[i] < 0xDC00 || str[i] > 0xDFFF) {
            if (replace) {
              size += 3;
              --i;
            } else {
              return -1;
            }
          } else {
            size += 4;
          }
        } else {
          if (replace) {
            size += 3;
          } else {
            return -1;
          }
        }
      }
      return size;
    }

    /// <summary>Compares two strings in Unicode code point order. Unpaired
    /// surrogates are treated as individual code points.</summary>
    /// <returns>A value indicating which string is " less" or " greater"
    /// . 0: Both strings are equal or null. Less than 0: a is null and b isn't;
    /// or the first code point that's different is less in A than in B; or b starts
    /// with a and is longer than a. Greater than 0: b is null and a isn't; or the
    /// first code point that' s different is greater in A than in B; or a starts
    /// with b and is longer than b.</returns>
    /// <param name='strA'>The first string.</param>
    /// <param name='strB'>The second string.</param>
    public static int CodePointCompare(String strA, String strB) {
      if (strA == null) {
        return (strB == null) ? 0 : -1;
      }
      if (strB == null) {
        return 1;
      }
      int len = Math.Min(strA.Length, strB.Length);
      for (int i = 0; i < len; ++i) {
        int ca = strA[i];
        int cb = strB[i];
        if (ca == cb) {
          // normal code units and illegal surrogates
          // are treated as single code points
          if ((ca & 0xF800) != 0xD800) {
            continue;
          }
          bool incindex = false;
          if (i + 1 < strA.Length && strA[i + 1] >= 0xDC00 && strA[i + 1] <= 0xDFFF) {
            ca = 0x10000 + ((ca - 0xD800) * 0x400) + (strA[i + 1] - 0xDC00);
            incindex = true;
          }
          if (i + 1 < strB.Length && strB[i + 1] >= 0xDC00 && strB[i + 1] <= 0xDFFF) {
            cb = 0x10000 + ((cb - 0xD800) * 0x400) + (strB[i + 1] - 0xDC00);
            incindex = true;
          }
          if (ca != cb) {
            return ca - cb;
          }
          if (incindex) {
            ++i;
          }
        } else {
          if ((ca & 0xF800) != 0xD800 && (cb & 0xF800) != 0xD800) {
            return ca - cb;
          }
          if (ca >= 0xd800 && ca <= 0xdbff && i + 1 < strA.Length &&
              strA[i + 1] >= 0xDC00 && strA[i + 1] <= 0xDFFF) {
            ca = 0x10000 + ((ca - 0xD800) * 0x400) + (strA[i + 1] - 0xDC00);
          }
          if (cb >= 0xd800 && cb <= 0xdbff && i + 1 < strB.Length &&
              strB[i + 1] >= 0xDC00 && strB[i + 1] <= 0xDFFF) {
            cb = 0x10000 + ((cb - 0xD800) * 0x400) + (strB[i + 1] - 0xDC00);
          }
          return ca - cb;
        }
      }
      if (strA.Length == strB.Length) {
        return 0;
      }
      return (strA.Length < strB.Length) ? -1 : 1;
    }

    /// <summary>Writes a portion of a string in UTF-8 encoding to a data stream.</summary>
    /// <param name='str'>A string to write.</param>
    /// <param name='offset'>The zero-based index where the string portion
    /// to write begins.</param>
    /// <param name='length'>The length of the string portion to write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <param name='replace'>If true, replaces unpaired surrogate code
    /// points with the replacement character (U + FFFD). If false, stops
    /// processing when an unpaired surrogate code point is seen.</param>
    /// <returns>0 if the entire string portion was written; or -1 if the string
    /// portion contains an unpaired surrogate code point and <paramref
    /// name='replace'/> is false.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='str'/> is null or <paramref name='stream'/> is
    /// null.</exception>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='offset'/> is less than 0, <paramref name='length'/> is less
    /// than 0, or <paramref name='offset'/> plus <paramref name='length'/>
    /// is greater than the string's length.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    public static int WriteUtf8(String str, int offset, int length, Stream stream, bool replace) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (offset < 0) {
        throw new ArgumentException("offset not greater or equal to " + "0" + " (" + Convert.ToString(offset, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      if (offset > str.Length) {
        throw new ArgumentException("offset not less or equal to " + Convert.ToString(str.Length, System.Globalization.CultureInfo.InvariantCulture) + " (" + Convert.ToString(offset, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      if (length < 0) {
        throw new ArgumentException("length not greater or equal to " + "0" + " (" + Convert.ToString(length, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      if (length > str.Length) {
        throw new ArgumentException("length not less or equal to " + Convert.ToString(str.Length, System.Globalization.CultureInfo.InvariantCulture) + " (" + Convert.ToString(length, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      if ((str.Length - offset) < length) {
        throw new ArgumentException("str's length minus " + offset + " not greater or equal to " + Convert.ToString(length, System.Globalization.CultureInfo.InvariantCulture) + " (" +
                                    Convert.ToString(str.Length - offset, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      byte[] bytes;
      int retval = 0;
      bytes = new byte[valueStreamedStringBufferLength];
      int byteIndex = 0;
      int endIndex = offset + length;
      for (int index = offset; index < endIndex; ++index) {
        int c = str[index];
        if (c <= 0x7F) {
          if (byteIndex >= valueStreamedStringBufferLength) {
            // Write bytes retrieved so far
            stream.Write(bytes, 0, byteIndex);
            byteIndex = 0;
          }
          bytes[byteIndex++] = (byte)c;
        } else if (c <= 0x7FF) {
          if (byteIndex + 2 > valueStreamedStringBufferLength) {
            // Write bytes retrieved so far
            stream.Write(bytes, 0, byteIndex);
            byteIndex = 0;
          }
          bytes[byteIndex++] = (byte)(0xC0 | ((c >> 6) & 0x1F));
          bytes[byteIndex++] = (byte)(0x80 | (c & 0x3F));
        } else {
          if (c >= 0xD800 && c <= 0xDBFF && index + 1 < endIndex &&
              str[index + 1] >= 0xDC00 && str[index + 1] <= 0xDFFF) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c - 0xD800) * 0x400) + (str[index + 1] - 0xDC00);
            ++index;
          } else if (c >= 0xD800 && c <= 0xDFFF) {
            // unpaired surrogate
            if (!replace) {
              retval = -1;
              break;  // write bytes read so far
            }
            c = 0xFFFD;
          }
          if (c <= 0xFFFF) {
            if (byteIndex + 3 > valueStreamedStringBufferLength) {
              // Write bytes retrieved so far
              stream.Write(bytes, 0, byteIndex);
              byteIndex = 0;
            }
            bytes[byteIndex++] = (byte)(0xE0 | ((c >> 12) & 0x0F));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 6) & 0x3F));
            bytes[byteIndex++] = (byte)(0x80 | (c & 0x3F));
          } else {
            if (byteIndex + 4 > valueStreamedStringBufferLength) {
              // Write bytes retrieved so far
              stream.Write(bytes, 0, byteIndex);
              byteIndex = 0;
            }
            bytes[byteIndex++] = (byte)(0xF0 | ((c >> 18) & 0x07));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 12) & 0x3F));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 6) & 0x3F));
            bytes[byteIndex++] = (byte)(0x80 | (c & 0x3F));
          }
        }
      }
      stream.Write(bytes, 0, byteIndex);
      return retval;
    }

    /// <summary>Writes a string in UTF-8 encoding to a data stream.</summary>
    /// <param name='str'>A string to write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <param name='replace'>If true, replaces unpaired surrogate code
    /// points with the replacement character (U + FFFD). If false, stops
    /// processing when an unpaired surrogate code point is seen.</param>
    /// <returns>0 if the entire string was written; or -1 if the string contains
    /// an unpaired surrogate code point and <paramref name='replace'/>
    /// is false.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='str'/> is null or <paramref name='stream'/> is
    /// null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    public static int WriteUtf8(String str, Stream stream, bool replace) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      return WriteUtf8(str, 0, str.Length, stream, replace);
    }

    /// <summary>Reads a string in UTF-8 encoding from a byte array.</summary>
    /// <param name='data'>A byte array containing a UTF-8 string.</param>
    /// <param name='offset'>Offset into the byte array to start reading.</param>
    /// <param name='bytesCount'>Length, in bytes, of the UTF-8 string.</param>
    /// <param name='builder'>A string builder object where the resulting
    /// string will be stored.</param>
    /// <param name='replace'>If true, replaces invalid encoding with
    /// the replacement character (U + FFFD). If false, stops processing
    /// when invalid UTF-8 is seen.</param>
    /// <returns>0 if the entire string was read without errors, or -1 if the
    /// string is not valid UTF-8 and <paramref name='replace'/> is false.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='data'/> is null or <paramref name='builder'/>
    /// is null.</exception>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='offset'/> is less than 0, <paramref name='bytesCount'/>
    /// is less than 0, or offset plus bytesCount is greater than the length
    /// of <paramref name='data'/> .</exception>
    public static int ReadUtf8FromBytes(
      byte[] data,
      int offset,
      int bytesCount,
      StringBuilder builder,
      bool replace) {
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      if (offset < 0) {
        throw new ArgumentException("offset not greater or equal to " + "0" + " (" + Convert.ToString(offset, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      if (offset > data.Length) {
        throw new ArgumentException("offset not less or equal to " + Convert.ToString(data.Length, System.Globalization.CultureInfo.InvariantCulture) + " (" + Convert.ToString(offset, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      if (bytesCount < 0) {
        throw new ArgumentException("bytesCount not greater or equal to " + "0" + " (" + Convert.ToString(bytesCount, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      if (bytesCount > data.Length) {
        throw new ArgumentException("bytesCount not less or equal to " + Convert.ToString(data.Length, System.Globalization.CultureInfo.InvariantCulture) + " (" + Convert.ToString(bytesCount, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      if ((data.Length - offset) < bytesCount) {
        throw new ArgumentException("data's length minus " + offset + " not greater or equal to " + Convert.ToString(bytesCount, System.Globalization.CultureInfo.InvariantCulture) + " (" + Convert.ToString(data.Length - offset, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      if (builder == null) {
        throw new ArgumentNullException("builder");
      }
      int cp = 0;
      int bytesSeen = 0;
      int bytesNeeded = 0;
      int lower = 0x80;
      int upper = 0xBF;
      int pointer = offset;
      int endpointer = offset + bytesCount;
      while (pointer < endpointer) {
        int b = data[pointer] & (int)0xFF;
        ++pointer;
        if (bytesNeeded == 0) {
          if ((b & 0x7F) == b) {
            builder.Append((char)b);
          } else if (b >= 0xc2 && b <= 0xdf) {
            bytesNeeded = 1;
            cp = (b - 0xc0) << 6;
          } else if (b >= 0xe0 && b <= 0xef) {
            lower = (b == 0xe0) ? 0xa0 : 0x80;
            upper = (b == 0xed) ? 0x9f : 0xbf;
            bytesNeeded = 2;
            cp = (b - 0xe0) << 12;
          } else if (b >= 0xf0 && b <= 0xf4) {
            lower = (b == 0xf0) ? 0x90 : 0x80;
            upper = (b == 0xf4) ? 0x8f : 0xbf;
            bytesNeeded = 3;
            cp = (b - 0xf0) << 18;
          } else {
            if (replace) {
              builder.Append((char)0xFFFD);
            } else {
              return -1;
            }
          }
          continue;
        } else if (b < lower || b > upper) {
          cp = bytesNeeded = bytesSeen = 0;
          lower = 0x80;
          upper = 0xbf;
          if (replace) {
            --pointer;
            builder.Append((char)0xFFFD);
            continue;
          } else {
            return -1;
          }
        } else {
          lower = 0x80;
          upper = 0xbf;
          ++bytesSeen;
          cp += (b - 0x80) << (6 * (bytesNeeded - bytesSeen));
          if (bytesSeen != bytesNeeded) {
            continue;
          }
          int ret = cp;
          cp = 0;
          bytesSeen = 0;
          bytesNeeded = 0;
          if (ret <= 0xFFFF) {
            builder.Append((char)ret);
          } else {
            int ch = ret - 0x10000;
            int lead = (ch / 0x400) + 0xd800;
            int trail = (ch & 0x3FF) + 0xdc00;
            builder.Append((char)lead);
            builder.Append((char)trail);
          }
        }
      }
      if (bytesNeeded != 0) {
        if (replace) {
          builder.Append((char)0xFFFD);
        } else {
          return -1;
        }
      }
      return 0;
    }

    /// <summary>Reads a string in UTF-8 encoding from a data stream.</summary>
    /// <param name='stream'>A readable data stream.</param>
    /// <param name='bytesCount'>The length, in bytes, of the string. If
    /// this is less than 0, this function will read until the end of the stream.</param>
    /// <param name='builder'>A string builder object where the resulting
    /// string will be stored.</param>
    /// <param name='replace'>If true, replaces invalid encoding with
    /// the replacement character (U + FFFD). If false, stops processing
    /// when an unpaired surrogate code point is seen.</param>
    /// <returns>0 if the entire string was read without errors, -1 if the
    /// string is not valid UTF-8 and <paramref name='replace'/> is false,
    /// or -2 if the end of the stream was reached before the last character
    /// was read completely.</returns>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null or <paramref name='builder'/>
    /// is null.</exception>
    public static int ReadUtf8(
      Stream stream,
      int bytesCount,
      StringBuilder builder,
      bool replace) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      if (builder == null) {
        throw new ArgumentNullException("builder");
      }
      int cp = 0;
      int bytesSeen = 0;
      int bytesNeeded = 0;
      int lower = 0x80;
      int upper = 0xBF;
      int pointer = 0;
      while (pointer < bytesCount || bytesCount < 0) {
        int b = stream.ReadByte();
        if (b < 0) {
          if (bytesNeeded != 0) {
            bytesNeeded = 0;
            if (replace) {
              builder.Append((char)0xFFFD);
              if (bytesCount >= 0) {
                return -2;
              }
              break;  // end of stream
            }
            return -1;
          } else {
            if (bytesCount >= 0) {
              return -2;
            }
            break;  // end of stream
          }
        }
        if (bytesCount > 0) {
          ++pointer;
        }
        if (bytesNeeded == 0) {
          if ((b & 0x7F) == b) {
            builder.Append((char)b);
          } else if (b >= 0xc2 && b <= 0xdf) {
            bytesNeeded = 1;
            cp = (b - 0xc0) << 6;
          } else if (b >= 0xe0 && b <= 0xef) {
            lower = (b == 0xe0) ? 0xa0 : 0x80;
            upper = (b == 0xed) ? 0x9f : 0xbf;
            bytesNeeded = 2;
            cp = (b - 0xe0) << 12;
          } else if (b >= 0xf0 && b <= 0xf4) {
            lower = (b == 0xf0) ? 0x90 : 0x80;
            upper = (b == 0xf4) ? 0x8f : 0xbf;
            bytesNeeded = 3;
            cp = (b - 0xf0) << 18;
          } else {
            if (replace) {
              builder.Append((char)0xFFFD);
            } else {
              return -1;
            }
          }
          continue;
        } else if (b < lower || b > upper) {
          cp = bytesNeeded = bytesSeen = 0;
          lower = 0x80;
          upper = 0xbf;
          if (replace) {
            builder.Append((char)0xFFFD);
            // "Read" the last byte again
            if (b < 0x80) {
              builder.Append((char)b);
            } else if (b >= 0xc2 && b <= 0xdf) {
              bytesNeeded = 1;
              cp = (b - 0xc0) << 6;
            } else if (b >= 0xe0 && b <= 0xef) {
              lower = (b == 0xe0) ? 0xa0 : 0x80;
              upper = (b == 0xed) ? 0x9f : 0xbf;
              bytesNeeded = 2;
              cp = (b - 0xe0) << 12;
            } else if (b >= 0xf0 && b <= 0xf4) {
              lower = (b == 0xf0) ? 0x90 : 0x80;
              upper = (b == 0xf4) ? 0x8f : 0xbf;
              bytesNeeded = 3;
              cp = (b - 0xf0) << 18;
            } else {
              builder.Append((char)0xFFFD);
            }
            continue;
          } else {
            return -1;
          }
        } else {
          lower = 0x80;
          upper = 0xbf;
          ++bytesSeen;
          cp += (b - 0x80) << (6 * (bytesNeeded - bytesSeen));
          if (bytesSeen != bytesNeeded) {
            continue;
          }
          int ret = cp;
          cp = 0;
          bytesSeen = 0;
          bytesNeeded = 0;
          if (ret <= 0xFFFF) {
            builder.Append((char)ret);
          } else {
            int ch = ret - 0x10000;
            int lead = (ch / 0x400) + 0xd800;
            int trail = (ch & 0x3FF) + 0xdc00;
            builder.Append((char)lead);
            builder.Append((char)trail);
          }
        }
      }
      if (bytesNeeded != 0) {
        if (replace) {
          builder.Append((char)0xFFFD);
        } else {
          return -1;
        }
      }
      return 0;
    }
  }
}
