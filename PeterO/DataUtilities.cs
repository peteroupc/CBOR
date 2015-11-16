/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.IO;
using System.Text;

namespace PeterO {
    /// <summary>Contains methods useful for reading and writing strings.
    /// It is designed to have no dependencies other than the basic runtime
    /// class library.</summary>
  public static class DataUtilities {
    private const int StreamedStringBufferLength = 4096;

    /// <summary>Generates a text string from a UTF-8 byte array.</summary>
    /// <param name='bytes'>A byte array containing text encoded in
    /// UTF-8.</param>
    /// <param name='replace'>If true, replaces invalid encoding with the
    /// replacement character (U + FFFD). If false, stops processing when
    /// invalid UTF-8 is seen.</param>
    /// <returns>A string represented by the UTF-8 byte array.</returns>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='bytes'/> is null.</exception>
    /// <exception cref="ArgumentException">The string is not valid UTF-8
    /// and <paramref name='replace'/> is false.</exception>
    public static string GetUtf8String(byte[] bytes, bool replace) {
      if (bytes == null) {
        throw new ArgumentNullException("bytes");
      }
      var b = new StringBuilder();
      if (ReadUtf8FromBytes(bytes, 0, bytes.Length, b, replace) != 0) {
        throw new ArgumentException("Invalid UTF-8");
      }
      return b.ToString();
    }

    /// <summary>Generates a text string from a portion of a UTF-8 byte
    /// array.</summary>
    /// <param name='bytes'>A byte array containing text encoded in
    /// UTF-8.</param>
    /// <param name='offset'>Offset into the byte array to start
    /// reading.</param>
    /// <param name='bytesCount'>Length, in bytes, of the UTF-8
    /// string.</param>
    /// <param name='replace'>If true, replaces invalid encoding with the
    /// replacement character (U + FFFD). If false, stops processing when
    /// invalid UTF-8 is seen.</param>
    /// <returns>A string represented by the UTF-8 byte array.</returns>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='bytes'/> is null.</exception>
    /// <exception cref="ArgumentException">The portion of the byte array
    /// is not valid UTF-8 and <paramref name='replace'/> is
    /// false.</exception>
    /// <exception cref="ArgumentException">The parameter <paramref
    /// name='offset'/> is less than 0, <paramref name='bytesCount'/> is
    /// less than 0, or offset plus bytesCount is greater than the length
    /// of "data" .</exception>
    public static string GetUtf8String(
byte[] bytes,
int offset,
int bytesCount,
bool replace) {
      if (bytes == null) {
        throw new ArgumentNullException("bytes");
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + offset + ") is less than " +
                    "0");
      }
      if (offset > bytes.Length) {
        throw new ArgumentException("offset (" + offset + ") is more than " +
                    bytes.Length);
      }
      if (bytesCount < 0) {
        throw new ArgumentException("bytesCount (" + bytesCount +
                    ") is less than 0");
      }
      if (bytesCount > bytes.Length) {
        throw new ArgumentException("bytesCount (" + bytesCount +
                    ") is more than " + bytes.Length);
      }
      if (bytes.Length - offset < bytesCount) {
        throw new ArgumentException("bytes's length minus " + offset + " (" +
                (bytes.Length - offset) + ") is less than " + bytesCount);
      }
      var b = new StringBuilder();
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
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref="ArgumentException">The string contains an unpaired
    /// surrogate code point and <paramref name='replace'/> is false, or an
    /// internal error occurred.</exception>
    public static byte[] GetUtf8Bytes(string str, bool replace) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      try {
        using (var ms = new MemoryStream()) {
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
    /// <param name='str'>A String object.</param>
    /// <param name='replace'>If true, treats unpaired surrogate code
    /// points as having 3 UTF-8 bytes (the UTF-8 length of the replacement
    /// character U + FFFD).</param>
    /// <returns>The number of bytes needed to encode the given string in
    /// UTF-8, or -1 if the string contains an unpaired surrogate code
    /// point and <paramref name='replace'/> is false.</returns>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='str'/> is null.</exception>
    public static long GetUtf8Length(String str, bool replace) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      long size = 0;
      for (var i = 0; i < str.Length; ++i) {
        int c = str[i];
        if (c <= 0x7f) {
          ++size;
        } else if (c <= 0x7ff) {
          size += 2;
        } else if (c <= 0xd7ff || c >= 0xe000) {
          size += 3;
        } else if (c <= 0xdbff) {  // UTF-16 leading surrogate
          ++i;
          if (i >= str.Length || str[i] < 0xdc00 || str[i] > 0xdfff) {
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

    /// <summary>Gets the Unicode code point just before the given index of
    /// the string.</summary>
    /// <param name='str'>A string.</param>
    /// <param name='index'>Index of the current position into the
    /// string.</param>
    /// <returns>The Unicode code point at the previous position. Returns
    /// -1 if <paramref name='index'/> is 0 or less, or is greater than the
    /// string's length. Returns the replacement character (U + FFFD) if
    /// the previous character is an unpaired surrogate code
    /// point.</returns>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='str'/> is null.</exception>
    public static int CodePointBefore(string str, int index) {
      return CodePointBefore(str, index, 0);
    }

    /// <summary>Gets the Unicode code point just before the given index of
    /// the string.</summary>
    /// <param name='str'>A string.</param>
    /// <param name='index'>Index of the current position into the
    /// string.</param>
    /// <param name='surrogateBehavior'>Specifies what kind of value to
    /// return if the previous character is an unpaired surrogate code
    /// point: if 0, return the replacement character (U + FFFD); if 1,
    /// return the value of the surrogate code point; if neither 0 nor 1,
    /// return -1.</param>
    /// <returns>The Unicode code point at the previous position. Returns
    /// -1 if <paramref name='index'/> is 0 or less, or is greater than the
    /// string's length. Returns a value as specified under <paramref
    /// name='surrogateBehavior'/> if the previous character is an unpaired
    /// surrogate code point.</returns>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='str'/> is null.</exception>
    public static int CodePointBefore(
string str,
int index,
int surrogateBehavior) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (index <= 0) {
        return -1;
      }
      if (index > str.Length) {
        return -1;
      }
      int c = str[index - 1];
      if ((c & 0xfc00) == 0xdc00 && index - 2 >= 0 &&
          str[index - 2] >= 0xd800 && str[index - 2] <= 0xdbff) {
        // Get the Unicode code point for the surrogate pair
        return 0x10000 + ((str[index - 2] - 0xd800) << 10) + (c - 0xdc00);
      }
      if ((c & 0xf800) == 0xd800) {
        // unpaired surrogate
        return (surrogateBehavior == 0) ? 0xfffd : ((surrogateBehavior == 1) ?
                    c : (-1));
      }
      return c;
    }

    /// <summary>Gets the Unicode code point at the given index of the
    /// string.</summary>
    /// <param name='str'>A string.</param>
    /// <param name='index'>Index of the current position into the
    /// string.</param>
    /// <returns>The Unicode code point at the given position. Returns -1
    /// if <paramref name='index'/> is less than 0, or is the string's
    /// length or greater. Returns the replacement character (U + FFFD) if
    /// the current character is an unpaired surrogate code
    /// point.</returns>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='str'/> is null.</exception>
    public static int CodePointAt(string str, int index) {
      return CodePointAt(str, index, 0);
    }

    /// <summary>Gets the Unicode code point at the given index of the
    /// string.</summary>
    /// <param name='str'>A string.</param>
    /// <param name='index'>Index of the current position into the
    /// string.</param>
    /// <param name='surrogateBehavior'>Specifies what kind of value to
    /// return if the previous character is an unpaired surrogate code
    /// point: if 0, return the replacement character (U + FFFD); if 1,
    /// return the value of the surrogate code point; if neither 0 nor 1,
    /// return -1.</param>
    /// <returns>The Unicode code point at the current position. Returns -1
    /// if <paramref name='index'/> is less than 0, or is the string's
    /// length or greater. Returns a value as specified under <paramref
    /// name='surrogateBehavior'/> if the previous character is an unpaired
    /// surrogate code point.</returns>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='str'/> is null.</exception>
    public static int CodePointAt(
string str,
int index,
int surrogateBehavior) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (index >= str.Length) {
        return -1;
      }
      if (index < 0) {
        return -1;
      }
      int c = str[index];
      if ((c & 0xfc00) == 0xd800 && index + 1 < str.Length &&
          str[index + 1] >= 0xdc00 && str[index + 1] <= 0xdfff) {
        // Get the Unicode code point for the surrogate pair
        c = 0x10000 + ((c - 0xd800) << 10) + (str[index + 1] - 0xdc00);
        ++index;
      } else if ((c & 0xf800) == 0xd800) {
        // unpaired surrogate
        return (surrogateBehavior == 0) ? 0xfffd : ((surrogateBehavior == 1) ?
                    c : (-1));
      }
      return c;
    }

    /// <summary>Returns a string with upper-case ASCII letters (A to Z)
    /// converted to lower-case. Other characters remain
    /// unchanged.</summary>
    /// <param name='str'>A string.</param>
    /// <returns>The converted string, or null if <paramref name='str'/> is
    /// null.</returns>
    public static string ToLowerCaseAscii(string str) {
      if (str == null) {
        return null;
      }
      int len = str.Length;
      var c = (char)0;
      bool hasUpperCase = false;
      for (var i = 0; i < len; ++i) {
        c = str[i];
        if (c >= 'A' && c <= 'Z') {
          hasUpperCase = true;
          break;
        }
      }
      if (!hasUpperCase) {
        return str;
      }
      var builder = new StringBuilder();
      for (var i = 0; i < len; ++i) {
        c = str[i];
        if (c >= 'A' && c <= 'Z') {
          builder.Append((char)(c + 0x20));
        } else {
          builder.Append(c);
        }
      }
      return builder.ToString();
    }

    /// <summary>Compares two strings in Unicode code point order. Unpaired
    /// surrogates are treated as individual code points.</summary>
    /// <param name='strA'>The first string. Can be null.</param>
    /// <param name='strB'>The second string. Can be null.</param>
    /// <returns>A value indicating which string is " less" or " greater" .
    /// 0: Both strings are equal or null. Less than 0: a is null and b
    /// isn't; or the first code point that's different is less in A than
    /// in B; or b starts with a and is longer than a. Greater than 0: b is
    /// null and a isn't; or the first code point that's different is
    /// greater in A than in B; or a starts with b and is longer than
    /// b.</returns>
    public static int CodePointCompare(String strA, String strB) {
      if (strA == null) {
        return (strB == null) ? 0 : -1;
      }
      if (strB == null) {
        return 1;
      }
      int len = Math.Min(strA.Length, strB.Length);
      for (var i = 0; i < len; ++i) {
        int ca = strA[i];
        int cb = strB[i];
        if (ca == cb) {
          // normal code units and illegal surrogates
          // are treated as single code points
          if ((ca & 0xf800) != 0xd800) {
            continue;
          }
          bool incindex = false;
          if (i + 1 < strA.Length && strA[i + 1] >= 0xdc00 && strA[i + 1] <=
              0xdfff) {
            ca = 0x10000 + ((ca - 0xd800) << 10) + (strA[i + 1] - 0xdc00);
            incindex = true;
          }
          if (i + 1 < strB.Length && strB[i + 1] >= 0xdc00 && strB[i + 1] <=
              0xdfff) {
            cb = 0x10000 + ((cb - 0xd800) << 10) + (strB[i + 1] - 0xdc00);
            incindex = true;
          }
          if (ca != cb) {
            return ca - cb;
          }
          if (incindex) {
            ++i;
          }
        } else {
          if ((ca & 0xf800) != 0xd800 && (cb & 0xf800) != 0xd800) {
            return ca - cb;
          }
          if ((ca & 0xfc00) == 0xd800 && i + 1 < strA.Length &&
              strA[i + 1] >= 0xdc00 && strA[i + 1] <= 0xdfff) {
            ca = 0x10000 + ((ca - 0xd800) << 10) + (strA[i + 1] - 0xdc00);
          }
          if ((cb & 0xfc00) == 0xd800 && i + 1 < strB.Length &&
              strB[i + 1] >= 0xdc00 && strB[i + 1] <= 0xdfff) {
            cb = 0x10000 + ((cb - 0xd800) << 10) + (strB[i + 1] - 0xdc00);
          }
          return ca - cb;
        }
      }
      return (strA.Length == strB.Length) ? 0 : ((strA.Length < strB.Length) ?
                    -1 : 1);
    }

    /// <summary>Writes a portion of a string in UTF-8 encoding to a data
    /// stream.</summary>
    /// <param name='str'>A string to write.</param>
    /// <param name='offset'>The zero-based index where the string portion
    /// to write begins.</param>
    /// <param name='length'>The length of the string portion to
    /// write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <param name='replace'>If true, replaces unpaired surrogate code
    /// points with the replacement character (U + FFFD). If false, stops
    /// processing when an unpaired surrogate code point is seen.</param>
    /// <returns>0 if the entire string portion was written; or -1 if the
    /// string portion contains an unpaired surrogate code point and
    /// <paramref name='replace'/> is false.</returns>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='str'/> is null or <paramref name='stream'/> is
    /// null.</exception>
    /// <exception cref="ArgumentException">The parameter <paramref
    /// name='offset'/> is less than 0, <paramref name='length'/> is less
    /// than 0, or <paramref name='offset'/> plus <paramref name='length'/>
    /// is greater than the string's length.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static int WriteUtf8(
String str,
int offset,
int length,
Stream stream,
bool replace) {
      return WriteUtf8(str, offset, length, stream, replace, false);
    }

    /// <summary>Writes a portion of a string in UTF-8 encoding to a data
    /// stream.</summary>
    /// <param name='str'>A string to write.</param>
    /// <param name='offset'>The zero-based index where the string portion
    /// to write begins.</param>
    /// <param name='length'>The length of the string portion to
    /// write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <param name='replace'>If true, replaces unpaired surrogate code
    /// points with the replacement character (U + FFFD). If false, stops
    /// processing when an unpaired surrogate code point is seen.</param>
    /// <param name='lenientLineBreaks'>If true, replaces carriage return
    /// (CR) not followed by line feed (LF) and LF not preceded by CR with
    /// CR-LF pairs.</param>
    /// <returns>0 if the entire string portion was written; or -1 if the
    /// string portion contains an unpaired surrogate code point and
    /// <paramref name='replace'/> is false.</returns>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='str'/> is null or <paramref name='stream'/> is
    /// null.</exception>
    /// <exception cref="ArgumentException">The parameter <paramref
    /// name='offset'/> is less than 0, <paramref name='length'/> is less
    /// than 0, or <paramref name='offset'/> plus <paramref name='length'/>
    /// is greater than the string's length.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static int WriteUtf8(
String str,
int offset,
int length,
Stream stream,
bool replace,
bool lenientLineBreaks) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + offset + ") is less than " +
                    "0");
      }
      if (offset > str.Length) {
        throw new ArgumentException("offset (" + offset + ") is more than " +
                    str.Length);
      }
      if (length < 0) {
        throw new ArgumentException("length (" + length + ") is less than " +
                    "0");
      }
      if (length > str.Length) {
        throw new ArgumentException("length (" + length + ") is more than " +
                    str.Length);
      }
      if (str.Length - offset < length) {
        throw new ArgumentException("str.Length minus offset (" +
                (str.Length - offset) + ") is less than " + length);
      }
      byte[] bytes;
      int retval = 0;
      bytes = new byte[StreamedStringBufferLength];
      int byteIndex = 0;
      int endIndex = offset + length;
      for (int index = offset; index < endIndex; ++index) {
        int c = str[index];
        if (c <= 0x7f) {
          if (lenientLineBreaks) {
            if (c == 0x0d && (index + 1 >= endIndex || str[index + 1] !=
                    0x0a)) {
              // bare CR, convert to CRLF
              if (byteIndex + 2 > StreamedStringBufferLength) {
                // Write bytes retrieved so far
                stream.Write(bytes, 0, byteIndex);
                byteIndex = 0;
              }
              bytes[byteIndex++] = 0x0d;
              bytes[byteIndex++] = 0x0a;
              continue;
            } else if (c == 0x0d) {
              // CR-LF pair
              if (byteIndex + 2 > StreamedStringBufferLength) {
                // Write bytes retrieved so far
                stream.Write(bytes, 0, byteIndex);
                byteIndex = 0;
              }
              bytes[byteIndex++] = 0x0d;
              bytes[byteIndex++] = 0x0a;
              ++index;
              continue;
            }
            if (c == 0x0a) {
              // bare LF, convert to CRLF
              if (byteIndex + 2 > StreamedStringBufferLength) {
                // Write bytes retrieved so far
                stream.Write(bytes, 0, byteIndex);
                byteIndex = 0;
              }
              bytes[byteIndex++] = 0x0d;
              bytes[byteIndex++] = 0x0a;
              continue;
            }
          }
          if (byteIndex >= StreamedStringBufferLength) {
            // Write bytes retrieved so far
            stream.Write(bytes, 0, byteIndex);
            byteIndex = 0;
          }
          bytes[byteIndex++] = (byte)c;
        } else if (c <= 0x7ff) {
          if (byteIndex + 2 > StreamedStringBufferLength) {
            // Write bytes retrieved so far
            stream.Write(bytes, 0, byteIndex);
            byteIndex = 0;
          }
          bytes[byteIndex++] = (byte)(0xc0 | ((c >> 6) & 0x1f));
          bytes[byteIndex++] = (byte)(0x80 | (c & 0x3f));
        } else {
          if ((c & 0xfc00) == 0xd800 && index + 1 < endIndex &&
              str[index + 1] >= 0xdc00 && str[index + 1] <= 0xdfff) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c - 0xd800) << 10) + (str[index + 1] - 0xdc00);
            ++index;
          } else if ((c & 0xf800) == 0xd800) {
            // unpaired surrogate
            if (!replace) {
              retval = -1;
              break;  // write bytes read so far
            }
            c = 0xfffd;
          }
          if (c <= 0xffff) {
            if (byteIndex + 3 > StreamedStringBufferLength) {
              // Write bytes retrieved so far
              stream.Write(bytes, 0, byteIndex);
              byteIndex = 0;
            }
            bytes[byteIndex++] = (byte)(0xe0 | ((c >> 12) & 0x0f));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 6) & 0x3f));
            bytes[byteIndex++] = (byte)(0x80 | (c & 0x3f));
          } else {
            if (byteIndex + 4 > StreamedStringBufferLength) {
              // Write bytes retrieved so far
              stream.Write(bytes, 0, byteIndex);
              byteIndex = 0;
            }
            bytes[byteIndex++] = (byte)(0xf0 | ((c >> 18) & 0x07));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 12) & 0x3f));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 6) & 0x3f));
            bytes[byteIndex++] = (byte)(0x80 | (c & 0x3f));
          }
        }
      }
      stream.Write(bytes, 0, byteIndex);
      return retval;
    }

    /// <summary>Writes a string in UTF-8 encoding to a data
    /// stream.</summary>
    /// <param name='str'>A string to write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <param name='replace'>If true, replaces unpaired surrogate code
    /// points with the replacement character (U + FFFD). If false, stops
    /// processing when an unpaired surrogate code point is seen.</param>
    /// <returns>0 if the entire string was written; or -1 if the string
    /// contains an unpaired surrogate code point and <paramref
    /// name='replace'/> is false.</returns>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='str'/> is null or <paramref name='stream'/> is
    /// null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static int WriteUtf8(String str, Stream stream, bool replace) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      return WriteUtf8(str, 0, str.Length, stream, replace);
    }

    /// <summary>Reads a string in UTF-8 encoding from a byte
    /// array.</summary>
    /// <param name='data'>A byte array containing a UTF-8 string.</param>
    /// <param name='offset'>Offset into the byte array to start
    /// reading.</param>
    /// <param name='bytesCount'>Length, in bytes, of the UTF-8
    /// string.</param>
    /// <param name='builder'>A string builder object where the resulting
    /// string will be stored.</param>
    /// <param name='replace'>If true, replaces invalid encoding with the
    /// replacement character (U + FFFD). If false, stops processing when
    /// invalid UTF-8 is seen.</param>
    /// <returns>0 if the entire string was read without errors, or -1 if
    /// the string is not valid UTF-8 and <paramref name='replace'/> is
    /// false.</returns>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='data'/> is null or <paramref name='builder'/> is
    /// null.</exception>
    /// <exception cref="ArgumentException">The parameter <paramref
    /// name='offset'/> is less than 0, <paramref name='bytesCount'/> is
    /// less than 0, or offset plus bytesCount is greater than the length
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
        throw new ArgumentException("offset (" + offset + ") is less than " +
                    "0");
      }
      if (offset > data.Length) {
        throw new ArgumentException("offset (" + offset + ") is more than " +
                    data.Length);
      }
      if (bytesCount < 0) {
        throw new ArgumentException("bytesCount (" + bytesCount +
                    ") is less than 0");
      }
      if (bytesCount > data.Length) {
        throw new ArgumentException("bytesCount (" + bytesCount +
                    ") is more than " + data.Length);
      }
      if (data.Length - offset < bytesCount) {
        throw new ArgumentException("data.Length minus offset (" +
                (data.Length - offset) + ") is less than " + bytesCount);
      }
      if (builder == null) {
        throw new ArgumentNullException("builder");
      }
      int cp = 0;
      int bytesSeen = 0;
      int bytesNeeded = 0;
      int lower = 0x80;
      int upper = 0xbf;
      int pointer = offset;
      int endpointer = offset + bytesCount;
      while (pointer < endpointer) {
        int b = data[pointer] & (int)0xff;
        ++pointer;
        if (bytesNeeded == 0) {
          if ((b & 0x7f) == b) {
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
              builder.Append((char)0xfffd);
            } else {
              return -1;
            }
          }
          continue;
        }
        if (b < lower || b > upper) {
          cp = bytesNeeded = bytesSeen = 0;
          lower = 0x80;
          upper = 0xbf;
          if (replace) {
            --pointer;
            builder.Append((char)0xfffd);
            continue;
          }
          return -1;
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
          if (ret <= 0xffff) {
            builder.Append((char)ret);
          } else {
            int ch = ret - 0x10000;
            int lead = (ch / 0x400) + 0xd800;
            int trail = (ch & 0x3ff) + 0xdc00;
            builder.Append((char)lead);
            builder.Append((char)trail);
          }
        }
      }
      if (bytesNeeded != 0) {
        if (replace) {
          builder.Append((char)0xfffd);
        } else {
          return -1;
        }
      }
      return 0;
    }

    /// <summary>Reads a string in UTF-8 encoding from a data stream in
    /// full and returns that string. Replaces invalid encoding with the
    /// replacement character (U + FFFD).</summary>
    /// <param name='stream'>A readable data stream.</param>
    /// <returns>The string read.</returns>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='stream'/> is null.</exception>
    public static string ReadUtf8ToString(Stream stream) {
      return ReadUtf8ToString(stream, -1, true);
    }

    /// <summary>Reads a string in UTF-8 encoding from a data stream and
    /// returns that string.</summary>
    /// <param name='stream'>A readable data stream.</param>
    /// <param name='bytesCount'>The length, in bytes, of the string. If
    /// this is less than 0, this function will read until the end of the
    /// stream.</param>
    /// <param name='replace'>If true, replaces invalid encoding with the
    /// replacement character (U + FFFD). If false, throws an error if an
    /// unpaired surrogate code point is seen.</param>
    /// <returns>The string read.</returns>
    /// <exception cref='System.IO.IOException'>An I/O error occurred; or,
    /// the string is not valid UTF-8 and <paramref name='replace'/> is
    /// false.</exception>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='stream'/> is null.</exception>
    public static string ReadUtf8ToString(
Stream stream,
int bytesCount,
bool replace) {
      var builder = new StringBuilder();
      int retval = DataUtilities.ReadUtf8(stream, bytesCount, builder, replace);
      if (retval == -1) {
        throw new IOException(
       "Unpaired surrogate code point found.",
       new DecoderFallbackException());
      }
      return builder.ToString();
    }

    /// <summary>Reads a string in UTF-8 encoding from a data
    /// stream.</summary>
    /// <param name='stream'>A readable data stream.</param>
    /// <param name='bytesCount'>The length, in bytes, of the string. If
    /// this is less than 0, this function will read until the end of the
    /// stream.</param>
    /// <param name='builder'>A string builder object where the resulting
    /// string will be stored.</param>
    /// <param name='replace'>If true, replaces invalid encoding with the
    /// replacement character (U + FFFD). If false, stops processing when
    /// an unpaired surrogate code point is seen.</param>
    /// <returns>0 if the entire string was read without errors, -1 if the
    /// string is not valid UTF-8 and <paramref name='replace'/> is false,
    /// or -2 if the end of the stream was reached before the last
    /// character was read completely (which is only the case if <paramref
    /// name='bytesCount'/> is 0 or greater).</returns>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='stream'/> is null or <paramref name='builder'/> is
    /// null.</exception>
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
      int upper = 0xbf;
      int pointer = 0;
      while (pointer < bytesCount || bytesCount < 0) {
        int b = stream.ReadByte();
        if (b < 0) {
          if (bytesNeeded != 0) {
            bytesNeeded = 0;
            if (replace) {
              builder.Append((char)0xfffd);
              if (bytesCount >= 0) {
                return -2;
              }
              break;  // end of stream
            }
            return -1;
          }
          if (bytesCount >= 0) {
            return -2;
          }
          break;  // end of stream
        }
        if (bytesCount > 0) {
          ++pointer;
        }
        if (bytesNeeded == 0) {
          if ((b & 0x7f) == b) {
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
              builder.Append((char)0xfffd);
            } else {
              return -1;
            }
          }
          continue;
        }
        if (b < lower || b > upper) {
          cp = bytesNeeded = bytesSeen = 0;
          lower = 0x80;
          upper = 0xbf;
          if (replace) {
            builder.Append((char)0xfffd);
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
              builder.Append((char)0xfffd);
            }
            continue;
          }
          return -1;
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
          if (ret <= 0xffff) {
            builder.Append((char)ret);
          } else {
            int ch = ret - 0x10000;
            int lead = (ch / 0x400) + 0xd800;
            int trail = (ch & 0x3ff) + 0xdc00;
            builder.Append((char)lead);
            builder.Append((char)trail);
          }
        }
      }
      if (bytesNeeded != 0) {
        if (replace) {
          builder.Append((char)0xfffd);
        } else {
          return -1;
        }
      }
      return 0;
    }
  }
}
