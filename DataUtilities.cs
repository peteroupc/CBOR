/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

using System;
using System.IO;
using System.Text;

namespace PeterO {
  /// <summary>
  /// Contains methods useful for reading and writing
  /// strings.  It is designed to have no dependencies
  /// other than the basic runtime class library.
  /// </summary>
  public static class DataUtilities {

    private static int StreamedStringBufferLength = 4096;

    /// <summary>
    /// Generates a text string from a UTF-8 byte array.
    /// </summary>
    /// <param name="bytes">A byte array containing text
    /// encoded in UTF-8.</param>
    /// <param name="replace">If true, replaces invalid encoding
    /// with the replacement character (U+FFFD).  If false,
    /// stops processing when invalid UTF-8 is seen.</param>
    /// <returns>A string represented by the UTF-8 byte array.</returns>
    /// <exception cref="System.ArgumentNullException">"bytes" is null.</exception>
    /// <exception cref="System.ArgumentException">The string
    /// is not valid UTF-8 and "replace" is false</exception>
    public static string GetUtf8String(byte[] bytes, bool replace) {
      StringBuilder b = new StringBuilder();
      if (ReadUtf8FromBytes(bytes, 0, bytes.Length, b, replace) != 0)
        throw new ArgumentException("Invalid UTF-8");
      return b.ToString();
    }

    /// <summary>
    /// Generates a text string from a portion of a UTF-8 byte array.
    /// </summary>
    /// <param name="bytes">A byte array containing text
    /// encoded in UTF-8.</param>
    /// <param name="offset">Offset into the byte array to start reading</param>
    /// <param name="byteLength">Length, in bytes, of the UTF-8 string</param>
    /// <param name="replace">If true, replaces invalid encoding
    /// with the replacement character (U+FFFD).  If false,
    /// stops processing when invalid UTF-8 is seen.</param>
    /// <returns>A string represented by the UTF-8 byte array.</returns>
    /// <exception cref="System.ArgumentNullException">"bytes" is null.</exception>
    /// <exception cref="System.ArgumentException">The portion of the byte array
    /// is not valid UTF-8 and "replace" is false</exception>
    public static string GetUtf8String(byte[] bytes, int offset, int byteLength, bool replace) {
      StringBuilder b = new StringBuilder();
      if (ReadUtf8FromBytes(bytes, offset, byteLength, b, replace) != 0)
        throw new ArgumentException("Invalid UTF-8");
      return b.ToString();
    }

    /// <summary>
    /// Encodes a string in UTF-8 as a byte array.
    /// </summary>
    /// <param name="str">A text string.</param>
    /// <param name="replace">If true, replaces unpaired surrogate
    /// code points with the replacement character (U+FFFD).  If false,
    /// stops processing when an unpaired surrogate code point is seen.</param>
    /// <returns>The string encoded in UTF-8.</returns>
    /// <exception cref="System.ArgumentNullException">"str" is null.</exception>
    /// <exception cref="System.ArgumentException">The string contains
    /// an unpaired surrogate code point
    /// and "replace" is false, or an internal error occurred.</exception>
    public static byte[] GetUtf8Bytes(string str, bool replace) {
      try {
        using (MemoryStream ms = new MemoryStream()) {
          if (WriteUtf8(str, ms, replace) != 0)
            throw new ArgumentException("Unpaired surrogate code point");
          return ms.ToArray();
        }
      } catch (IOException ex) {
        throw new ArgumentException("I/O error occurred", ex);
      }
    }

    /// <summary>
    /// Calculates the number of bytes needed to encode a string
    /// in UTF-8.
    /// </summary>
    /// <param name="s">A Unicode string.</param>
    /// <param name="replace">If true, treats unpaired
    /// surrogate code points as replacement characters (U+FFFD) instead,
    /// meaning each one takes 3 UTF-8 bytes.  If false, stops
    /// processing when an unpaired surrogate code point is reached.</param>
    /// <returns>The number of bytes needed to encode the given string
    /// in UTF-8, or -1 if the string contains an unpaired surrogate
    /// code point and "replace" is false.</returns>
    /// <exception cref="System.ArgumentNullException">"s" is null.</exception>
    public static long GetUtf8Length(String s, bool replace) {
      if (s == null) throw new ArgumentNullException("s");
      long size = 0;
      for (int i = 0; i < s.Length; i++) {
        int c = s[i];
        if (c <= 0x7F) {
          size++;
        } else if (c <= 0x7FF) {
          size += 2;
        } else if (c <= 0xD7FF || c >= 0xE000) {
          size += 3;
        } else if (c <= 0xDBFF) { // UTF-16 leading surrogate
          i++;
          if (i >= s.Length || s[i] < 0xDC00 || s[i] > 0xDFFF) {
            if (replace) {
              size += 3;
              i--;
            } else return -1;
          } else {
            size += 4;
          }
        } else {
          if (replace) size += 3;
          else return -1;
        }
      }
      return size;
    }


    ///<summary>Compares two strings in Unicode code point order. Unpaired
    ///surrogates are treated as individual code points.</summary>
    /// <param name="a">The first string.</param>
    /// <param name="b">The second string.</param>
    /// <returns>A value indicating which string is "less" or "greater".
    /// 0: Both strings are equal or null.
    /// Less than 0: a is null and b isn't; or the first code point that's
    /// different is less in A than in B; or b starts with a and is longer than a.
    /// Greater than 0: b is null and a isn't; or the first code point that's
    /// different is greater in A than in B; or a starts with b and is longer
    /// than b.</returns>
    public static int CodePointCompare(String strA, String strB) {
      if (strA == null) return (strB == null) ? 0 : -1;
      if (strB == null) return 1;
      int len = Math.Min(strA.Length, strB.Length);
      for (int i = 0; i < len; i++) {
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
            ca = 0x10000 + (ca - 0xD800) * 0x400 + (strA[i + 1] - 0xDC00);
            incindex = true;
          }
          if (i + 1 < strB.Length && strB[i + 1] >= 0xDC00 && strB[i + 1] <= 0xDFFF) {
            cb = 0x10000 + (cb - 0xD800) * 0x400 + (strB[i + 1] - 0xDC00);
            incindex = true;
          }
          if (ca != cb) return ca - cb;
          if (incindex) {
            i++;
          }
        } else {
          if ((ca & 0xF800) != 0xD800 && (cb & 0xF800) != 0xD800)
            return ca - cb;
          if (ca >= 0xd800 && ca <= 0xdbff && i + 1 < strA.Length &&
              strA[i + 1] >= 0xDC00 && strA[i + 1] <= 0xDFFF) {
            ca = 0x10000 + (ca - 0xD800) * 0x400 + (strA[i + 1] - 0xDC00);
          }
          if (cb >= 0xd800 && cb <= 0xdbff && i + 1 < strB.Length &&
              strB[i + 1] >= 0xDC00 && strB[i + 1] <= 0xDFFF) {
            cb = 0x10000 + (cb - 0xD800) * 0x400 + (strB[i + 1] - 0xDC00);
          }
          return ca - cb;
        }
      }
      if (strA.Length == strB.Length) return 0;
      return (strA.Length < strB.Length) ? -1 : 1;
    }

    /// <summary>
    /// Writes a portion of a string in UTF-8 encoding to a data stream.
    /// </summary>
    /// <param name="str">A string to write.</param>
    /// <param name="offset">The zero-based index where the string
    /// portion to write begins.</param>
    /// <param name="length">The length of the string portion to write.</param>
    /// <param name="stream">A writable data stream.</param>
    /// <param name="replace">If true, replaces unpaired surrogate
    /// code points with the replacement character (U+FFFD).  If false,
    /// stops processing when an unpaired surrogate code point is seen.</param>
    /// <returns>0 if the entire string portion was written; or -1 if the
    /// string portion contains an unpaired surrogate code point and "replace"
    /// is false.</returns>
    /// <exception cref="System.ArgumentNullException">"str" is null or "stream"
    /// is null.</exception>
    /// <exception cref="System.ArgumentException">"offset" is less than 0,
    /// "length" is less than 0, or "offset" plus "length" is greater than
    /// the string's length.</exception>
    /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
    public static int WriteUtf8(String str, int offset, int length, Stream stream, bool replace) {
      if ((stream) == null) throw new ArgumentNullException("stream");
      if((str)==null)throw new ArgumentNullException("str");
      if((offset)<0)throw new ArgumentOutOfRangeException("offset"+" not greater or equal to "+Convert.ToString((long)(0))+" ("+Convert.ToString((long)(offset))+")");
      if((offset)>str.Length)throw new ArgumentOutOfRangeException("offset"+" not less or equal to "+Convert.ToString((long)(str.Length))+" ("+Convert.ToString((long)(offset))+")");
      if((length)<0)throw new ArgumentOutOfRangeException("length"+" not greater or equal to "+Convert.ToString((long)(0))+" ("+Convert.ToString((long)(length))+")");
      if((length)>str.Length)throw new ArgumentOutOfRangeException("length"+" not less or equal to "+Convert.ToString((long)(str.Length))+" ("+Convert.ToString((long)(length))+")");
      if(((str.Length-offset))<length)throw new ArgumentOutOfRangeException("str's length minus "+offset+" not greater or equal to "+Convert.ToString((long)(length))+" ("+Convert.ToString((long)((str.Length-offset)))+")");
      byte[] bytes;
      int retval = 0;
      bytes = new byte[StreamedStringBufferLength];
      int byteIndex = 0;
      int endIndex=offset+length;
      for (int index = offset; index < endIndex; index++) {
        int c = str[index];
        if (c <= 0x7F) {
          if (byteIndex >= StreamedStringBufferLength) {
            // Write bytes retrieved so far
            stream.Write(bytes, 0, byteIndex);
            byteIndex = 0;
          }
          bytes[byteIndex++] = (byte)c;
        } else if (c <= 0x7FF) {
          if (byteIndex + 2 > StreamedStringBufferLength) {
            // Write bytes retrieved so far
            stream.Write(bytes, 0, byteIndex);
            byteIndex = 0;
          }
          bytes[byteIndex++] = ((byte)(0xC0 | ((c >> 6) & 0x1F)));
          bytes[byteIndex++] = ((byte)(0x80 | (c & 0x3F)));
        } else {
          if (c >= 0xD800 && c <= 0xDBFF && index + 1 < endIndex &&
              str[index + 1] >= 0xDC00 && str[index + 1] <= 0xDFFF) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + (c - 0xD800) * 0x400 + (str[index + 1] - 0xDC00);
            index++;
          } else if (c >= 0xD800 && c <= 0xDFFF) {
            // unpaired surrogate
            if (!replace) {
              retval = -1;
              break; // write bytes read so far
            }
            c = 0xFFFD;
          }
          if (c <= 0xFFFF) {
            if (byteIndex + 3 > StreamedStringBufferLength) {
              // Write bytes retrieved so far
              stream.Write(bytes, 0, byteIndex);
              byteIndex = 0;
            }
            bytes[byteIndex++] = ((byte)(0xE0 | ((c >> 12) & 0x0F)));
            bytes[byteIndex++] = ((byte)(0x80 | ((c >> 6) & 0x3F)));
            bytes[byteIndex++] = ((byte)(0x80 | (c & 0x3F)));
          } else {
            if (byteIndex + 4 > StreamedStringBufferLength) {
              // Write bytes retrieved so far
              stream.Write(bytes, 0, byteIndex);
              byteIndex = 0;
            }
            bytes[byteIndex++] = ((byte)(0xF0 | ((c >> 18) & 0x07)));
            bytes[byteIndex++] = ((byte)(0x80 | ((c >> 12) & 0x3F)));
            bytes[byteIndex++] = ((byte)(0x80 | ((c >> 6) & 0x3F)));
            bytes[byteIndex++] = ((byte)(0x80 | (c & 0x3F)));
          }
        }
      }
      stream.Write(bytes, 0, byteIndex);
      return retval;
    }


    /// <summary>
    /// Writes a string in UTF-8 encoding to a data stream.
    /// </summary>
    /// <param name="str">A string to write.</param>
    /// <param name="stream">A writable data stream.</param>
    /// <param name="replace">If true, replaces unpaired surrogate
    /// code points with the replacement character (U+FFFD).  If false,
    /// stops processing when an unpaired surrogate code point is seen.</param>
    /// <returns>0 if the entire string was written; or -1 if the
    /// string contains an unpaired surrogate code point and "replace"
    /// is false.</returns>
    /// <exception cref="System.ArgumentNullException">"str" is null or "stream"
    /// is null.</exception>
    /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
    public static int WriteUtf8(String str, Stream stream, bool replace) {
      if((str)==null)throw new ArgumentNullException("str");
      return WriteUtf8(str,0,str.Length,stream,replace);
    }

    /// <summary>
    /// Reads a string in UTF-8 encoding from a byte array.
    /// </summary>
    /// <param name="data">A byte array containing a UTF-8 string</param>
    /// <param name="offset">Offset into the byte array to start reading</param>
    /// <param name="byteLength">Length, in bytes, of the UTF-8 string</param>
    /// <param name="builder">A string builder object where the resulting
    /// string will be stored.</param>
    /// <param name="replace">If true, replaces invalid encoding
    /// with the replacement character (U+FFFD).  If false,
    /// stops processing when invalid UTF-8 is seen.</param>
    /// <returns>0 if the entire string was read without errors, or -1 if the string
    /// is not valid UTF-8 and "replace" is false.</returns>
    /// <exception cref="System.ArgumentNullException">"data" is null or "builder" is null.</exception>
    /// <exception cref="System.ArgumentException">"offset" is less than 0, "byteLength"
    /// is less than 0, or offset plus byteLength is greater than the length of "data".</exception>
    public static int ReadUtf8FromBytes(byte[] data, int offset, int byteLength,
                                        StringBuilder builder,
                                        bool replace) {
      if((data)==null)throw new ArgumentNullException("data");
      if((offset)<0)throw new ArgumentOutOfRangeException("offset"+" not greater or equal to "+Convert.ToString((long)(0))+" ("+Convert.ToString((long)(offset))+")");
      if((offset)>data.Length)throw new ArgumentOutOfRangeException("offset"+" not less or equal to "+Convert.ToString((long)(data.Length))+" ("+Convert.ToString((long)(offset))+")");
      if((byteLength)<0)throw new ArgumentOutOfRangeException("byteLength"+" not greater or equal to "+Convert.ToString((long)(0))+" ("+Convert.ToString((long)(byteLength))+")");
      if((byteLength)>data.Length)throw new ArgumentOutOfRangeException("byteLength"+" not less or equal to "+Convert.ToString((long)(data.Length))+" ("+Convert.ToString((long)(byteLength))+")");
      if(((data.Length-offset))<byteLength)throw new ArgumentOutOfRangeException("data's length minus "+offset+" not greater or equal to "+Convert.ToString((long)(byteLength))+" ("+Convert.ToString((long)((data.Length-offset)))+")");
      if ((builder) == null) throw new ArgumentNullException("builder");
      int cp = 0;
      int bytesSeen = 0;
      int bytesNeeded = 0;
      int lower = 0x80;
      int upper = 0xBF;
      int pointer = offset;
      int endpointer = offset + byteLength;
      while (pointer < endpointer) {
        int b = (data[pointer] & (int)0xFF);
        pointer++;
        if (bytesNeeded == 0) {
          if ((b&0x7F)==b) {
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
            if (replace)
              builder.Append((char)0xFFFD);
            else
              return -1;
          }
          continue;
        } else if (b < lower || b > upper) {
          cp = bytesNeeded = bytesSeen = 0;
          lower = 0x80;
          upper = 0xbf;
          if (replace) {
            pointer--;
            builder.Append((char)0xFFFD);
            continue;
          } else {
            return -1;
          }
        } else {
          lower = 0x80;
          upper = 0xbf;
          bytesSeen++;
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
            int lead = ch / 0x400 + 0xd800;
            int trail = (ch & 0x3FF) + 0xdc00;
            builder.Append((char)lead);
            builder.Append((char)trail);
          }
        }
      }
      if (bytesNeeded != 0) {
        if (replace)
          builder.Append((char)0xFFFD);
        else
          return -1;
      }
      return 0;
    }


    /// <summary>
    /// Reads a string in UTF-8 encoding from a data stream.
    /// </summary>
    /// <param name="stream">A readable data stream.</param>
    /// <param name="byteLength">The length, in bytes, of the string.
    /// If this is less than 0, this function will read until the end
    /// of the stream.</param>
    /// <param name="builder">A string builder object where the resulting
    /// string will be stored.</param>
    /// <param name="replace">If true, replaces invalid encoding
    /// with the replacement character (U+FFFD).  If false,
    /// stops processing when an unpaired surrogate code point is seen.</param>
    /// <returns>0 if the entire string was read without errors,
    /// -1 if the string is not valid UTF-8 and "replace" is false (even if
    /// the end of the stream is reached), or -2 if the end
    /// of the stream was reached before the entire string was
    /// read.
    /// </returns>
    /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
    /// <exception cref="System.ArgumentNullException">"stream" is null
    /// or "builder" is null.</exception>
    public static int ReadUtf8(Stream stream, int byteLength, StringBuilder builder,
                               bool replace) {
      if ((stream) == null) throw new ArgumentNullException("stream");
      if ((builder) == null) throw new ArgumentNullException("builder");
      int cp = 0;
      int bytesSeen = 0;
      int bytesNeeded = 0;
      int lower = 0x80;
      int upper = 0xBF;
      int pointer = 0;
      while (pointer < byteLength || byteLength < 0) {
        int b = stream.ReadByte();
        if (b < 0) {
          if (bytesNeeded != 0) {
            bytesNeeded = 0;
            if (replace) {
              builder.Append((char)0xFFFD);
              if (byteLength >= 0)
                return -2;
              break; // end of stream
            }
            return -1;
          } else {
            if (byteLength >= 0)
              return -2;
            break; // end of stream
          }
        }
        if (byteLength > 0) {
          pointer++;
        }
        if (bytesNeeded == 0) {
          if ((b&0x7F)==b) {
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
            if (replace)
              builder.Append((char)0xFFFD);
            else
              return -1;
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
          bytesSeen++;
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
            int lead = ch / 0x400 + 0xd800;
            int trail = (ch & 0x3FF) + 0xdc00;
            builder.Append((char)lead);
            builder.Append((char)trail);
          }
        }
      }
      if (bytesNeeded != 0) {
        if (replace)
          builder.Append((char)0xFFFD);
        else
          return -1;
      }
      return 0;
    }
  }
}