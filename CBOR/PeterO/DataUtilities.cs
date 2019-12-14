/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.IO;
using System.Text;

namespace PeterO {
  /// <summary>Contains methods useful for reading and writing text
  /// strings. It is designed to have no dependencies other than the
  /// basic runtime class library.
  /// <para>Many of these methods work with text encoded in UTF-8, an
  /// encoding form of the Unicode Standard which uses one byte to encode
  /// the most basic characters and two to four bytes to encode other
  /// characters. For example, the <c>GetUtf8</c> method converts a text
  /// string to an array of bytes in UTF-8.</para>
  /// <para>In C# and Java, text strings are represented as sequences of
  /// 16-bit values called <c>char</c> s. These sequences are well-formed
  /// under UTF-16, a 16-bit encoding form of Unicode, except if they
  /// contain unpaired surrogate code points. (A surrogate code point is
  /// used to encode supplementary characters, those with code points
  /// U+10000 or higher, in UTF-16. A surrogate pair is a high surrogate,
  /// U+D800 to U+DBFF, followed by a low surrogate, U+DC00 to U+DFFF. An
  /// unpaired surrogate code point is a surrogate not appearing in a
  /// surrogate pair.) Many of the methods in this class allow setting
  /// the behavior to follow when unpaired surrogate code points are
  /// found in text strings, such as throwing an error or treating the
  /// unpaired surrogate as a replacement character
  /// (U+FFFD).</para></summary>
  public static class DataUtilities {
    private const int StreamedStringBufferLength = 4096;

    /// <summary>Generates a text string from a UTF-8 byte array.</summary>
    /// <param name='bytes'>A byte array containing text encoded in
    /// UTF-8.</param>
    /// <param name='replace'>If true, replaces invalid encoding with the
    /// replacement character (U+FFFD). If false, stops processing when
    /// invalid UTF-8 is seen.</param>
    /// <returns>A string represented by the UTF-8 byte array.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    /// <exception cref='ArgumentException'>The string is not valid UTF-8
    /// and <paramref name='replace'/> is false.</exception>
    public static string GetUtf8String(byte[] bytes, bool replace) {
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      var b = new StringBuilder();
      if (ReadUtf8FromBytes(bytes, 0, bytes.Length, b, replace) != 0) {
        throw new ArgumentException("Invalid UTF-8");
      }
      return b.ToString();
    }

    /// <summary>Finds the number of Unicode code points in the given text
    /// string. Unpaired surrogate code points increase this number by 1.
    /// This is not necessarily the length of the string in "char"
    /// s.</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <returns>The number of Unicode code points in the given
    /// string.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public static int CodePointLength(string str) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      var i = 0;
      var count = 0;
      while (i < str.Length) {
        int c = CodePointAt(str, i);
        ++count;
        i += (c >= 0x10000) ? 2 : 1;
      }
      return count;
    }

    /// <summary>Generates a text string from a portion of a UTF-8 byte
    /// array.</summary>
    /// <param name='bytes'>A byte array containing text encoded in
    /// UTF-8.</param>
    /// <param name='offset'>Offset into the byte array to start
    /// reading.</param>
    /// <param name='bytesCount'>Length, in bytes, of the UTF-8 text
    /// string.</param>
    /// <param name='replace'>If true, replaces invalid encoding with the
    /// replacement character (U+FFFD). If false, stops processing when
    /// invalid UTF-8 is seen.</param>
    /// <returns>A string represented by the UTF-8 byte array.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    /// <exception cref='ArgumentException'>The portion of the byte array
    /// is not valid UTF-8 and <paramref name='replace'/> is
    /// false.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='offset'/> is less than 0, <paramref name='bytesCount'/> is
    /// less than 0, or offset plus bytesCount is greater than the length
    /// of "data" .</exception>
    public static string GetUtf8String(
      byte[] bytes,
      int offset,
      int bytesCount,
      bool replace) {
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      if (offset < 0) {
        throw new ArgumentException("offset(" + offset + ") is less than " +
          "0");
      }
      if (offset > bytes.Length) {
        throw new ArgumentException("offset(" + offset + ") is more than " +
          bytes.Length);
      }
      if (bytesCount < 0) {
        throw new ArgumentException("bytesCount(" + bytesCount +
          ") is less than 0");
      }
      if (bytesCount > bytes.Length) {
        throw new ArgumentException("bytesCount(" + bytesCount +
          ") is more than " + bytes.Length);
      }
      if (bytes.Length - offset < bytesCount) {
        throw new ArgumentException("bytes's length minus " + offset + "(" +
          (bytes.Length - offset) + ") is less than " + bytesCount);
      }
      var b = new StringBuilder();
      if (ReadUtf8FromBytes(bytes, offset, bytesCount, b, replace) != 0) {
        throw new ArgumentException("Invalid UTF-8");
      }
      return b.ToString();
    }

    /// <summary>
    /// <para>Encodes a string in UTF-8 as a byte array. This method does
    /// not insert a byte-order mark (U+FEFF) at the beginning of the
    /// encoded byte array.</para>
    /// <para>REMARK: It is not recommended to use
    /// <c>Encoding.UTF8.GetBytes</c> in.NET, or the <c>getBytes()</c>
    /// method in Java to do this. For instance, <c>getBytes()</c> encodes
    /// text strings in a default (so not fixed) character encoding, which
    /// can be undesirable.</para></summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='replace'>If true, replaces unpaired surrogate code
    /// points with the replacement character (U+FFFD). If false, stops
    /// processing when an unpaired surrogate code point is seen.</param>
    /// <returns>The string encoded in UTF-8.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='ArgumentException'>The string contains an unpaired
    /// surrogate code point and <paramref name='replace'/> is false, or an
    /// internal error occurred.</exception>
    public static byte[] GetUtf8Bytes(string str, bool replace) {
      return GetUtf8Bytes(str, replace, false);
    }

    /// <summary>
    /// <para>Encodes a string in UTF-8 as a byte array. This method does
    /// not insert a byte-order mark (U+FEFF) at the beginning of the
    /// encoded byte array.</para>
    /// <para>REMARK: It is not recommended to use
    /// <c>Encoding.UTF8.GetBytes</c> in.NET, or the <c>getBytes()</c>
    /// method in Java to do this. For instance, <c>getBytes()</c> encodes
    /// text strings in a default (so not fixed) character encoding, which
    /// can be undesirable.</para></summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='replace'>If true, replaces unpaired surrogate code
    /// points with the replacement character (U+FFFD). If false, stops
    /// processing when an unpaired surrogate code point is seen.</param>
    /// <param name='lenientLineBreaks'>If true, replaces carriage return
    /// (CR) not followed by line feed (LF) and LF not preceded by CR with
    /// CR-LF pairs.</param>
    /// <returns>The string encoded in UTF-8.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='ArgumentException'>The string contains an unpaired
    /// surrogate code point and <paramref name='replace'/> is false, or an
    /// internal error occurred.</exception>
    public static byte[] GetUtf8Bytes(
      string str,
      bool replace,
      bool lenientLineBreaks) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (!lenientLineBreaks && str.Length == 1) {
        int c = str[0];
        if ((c & 0xf800) == 0xd800) {
          if (replace) {
            c = 0xfffd;
          } else {
            throw new ArgumentException("Unpaired surrogate code point");
          }
        }
        if (c <= 0x80) {
          return new byte[] { (byte)c };
        } else if (c <= 0x7ff) {
          return new byte[] {
            (byte)(0xc0 | ((c >> 6) & 0x1f)),
            (byte)(0x80 | (c & 0x3f)),
          };
        } else {
          return new byte[] {
            (byte)(0xe0 | ((c >> 12) & 0x0f)),
            (byte)(0x80 | ((c >> 6) & 0x3f)),
            (byte)(0x80 | (c & 0x3f)),
          };
        }
      } else if (str.Length == 2) {
        int c = str[0];
        int c2 = str[1];
        if ((c & 0xfc00) == 0xd800 && (c2 & 0xfc00) == 0xdc00) {
          c = 0x10000 + ((c & 0x3ff) << 10) + (c2 & 0x3ff);
          return new byte[] {
            (byte)(0xf0 | ((c >> 18) & 0x07)),
            (byte)(0x80 | ((c >> 12) & 0x3f)),
            (byte)(0x80 | ((c >> 6) & 0x3f)),
            (byte)(0x80 | (c & 0x3f)),
          };
        } else if (!lenientLineBreaks && c <= 0x80 && c2 <= 0x80) {
          return new byte[] { (byte)c, (byte)c2 };
        }
      }
      try {
        using (var ms = new MemoryStream()) {
          if (WriteUtf8(str, 0, str.Length, ms, replace, lenientLineBreaks) !=
            0) {
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
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='replace'>If true, treats unpaired surrogate code
    /// points as having 3 UTF-8 bytes (the UTF-8 length of the replacement
    /// character U+FFFD).</param>
    /// <returns>The number of bytes needed to encode the given string in
    /// UTF-8, or -1 if the string contains an unpaired surrogate code
    /// point and <paramref name='replace'/> is false.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public static long GetUtf8Length(string str, bool replace) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
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
        } else if (c <= 0xdbff) { // UTF-16 leading surrogate
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
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='index'>Index of the current position into the
    /// string.</param>
    /// <returns>The Unicode code point at the previous position. Returns
    /// -1 if <paramref name='index'/> is 0 or less, or is greater than the
    /// string's length. Returns the replacement character (U+FFFD) if the
    /// code point at the previous position is an unpaired surrogate code
    /// point. If the return value is 65536 (0x10000) or greater, the code
    /// point takes up two UTF-16 code units.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public static int CodePointBefore(string str, int index) {
      return CodePointBefore(str, index, 0);
    }

    /// <summary>Gets the Unicode code point just before the given index of
    /// the string.</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='index'>Index of the current position into the
    /// string.</param>
    /// <param name='surrogateBehavior'>Specifies what kind of value to
    /// return if the previous code point is an unpaired surrogate code
    /// point: if 0, return the replacement character (U+FFFD); if 1,
    /// return the value of the surrogate code point; if neither 0 nor 1,
    /// return -1.</param>
    /// <returns>The Unicode code point at the previous position. Returns
    /// -1 if <paramref name='index'/> is 0 or less, or is greater than the
    /// string's length. Returns a value as specified under <paramref
    /// name='surrogateBehavior'/> if the code point at the previous
    /// position is an unpaired surrogate code point. If the return value
    /// is 65536 (0x10000) or greater, the code point takes up two UTF-16
    /// code units.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public static int CodePointBefore(
      string str,
      int index,
      int surrogateBehavior) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (index <= 0) {
        return -1;
      }
      if (index > str.Length) {
        return -1;
      }
      int c = str[index - 1];
      if ((c & 0xfc00) == 0xdc00 && index - 2 >= 0 &&
        (str[index - 2] & 0xfc00) == 0xd800) {
        // Get the Unicode code point for the surrogate pair
        return 0x10000 + ((str[index - 2] & 0x3ff) << 10) + (c & 0x3ff);
      }
      // unpaired surrogate
      if ((c & 0xf800) == 0xd800) {
        return (surrogateBehavior == 0) ? 0xfffd : ((surrogateBehavior == 1) ?
            c : -1);
      }
      return c;
    }

    /// <summary>Gets the Unicode code point at the given index of the
    /// string.</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='index'>Index of the current position into the
    /// string.</param>
    /// <returns>The Unicode code point at the given position. Returns -1
    /// if <paramref name='index'/> is 0 or less, or is greater than the
    /// string's length. Returns the replacement character (U+FFFD) if the
    /// code point at that position is an unpaired surrogate code point. If
    /// the return value is 65536 (0x10000) or greater, the code point
    /// takes up two UTF-16 code units.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public static int CodePointAt(string str, int index) {
      return CodePointAt(str, index, 0);
    }

    /// <summary>Gets the Unicode code point at the given index of the
    /// string.</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='index'>Index of the current position into the
    /// string.</param>
    /// <param name='surrogateBehavior'>Specifies what kind of value to
    /// return if the code point at the given index is an unpaired
    /// surrogate code point: if 0, return the replacement character (U +
    /// FFFD); if 1, return the value of the surrogate code point; if
    /// neither 0 nor 1, return -1.</param>
    /// <returns>The Unicode code point at the given position. Returns -1
    /// if <paramref name='index'/> is 0 or less, or is greater than the
    /// string's length. Returns a value as specified under <paramref
    /// name='surrogateBehavior'/> if the code point at that position is an
    /// unpaired surrogate code point. If the return value is 65536
    /// (0x10000) or greater, the code point takes up two UTF-16 code
    /// units.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <example>
    /// <para>The following example shows how to iterate a text string code
    /// point by code point, terminating the loop when an unpaired
    /// surrogate is found.</para>
    /// <code>for (var i = 0;i&lt;str.Length; ++i) { int codePoint =
    /// DataUtilities.CodePointAt(str, i, 2); if (codePoint &lt; 0) { break; /*
    /// Unpaired surrogate */ } Console.WriteLine("codePoint:"+codePoint); if
    /// (codePoint &gt;= 0x10000) { i++; /* Supplementary code point */ } }</code>
    ///  .
    /// </example>
    public static int CodePointAt(
      string str,
      int index,
      int surrogateBehavior) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (index >= str.Length) {
        return -1;
      }
      if (index < 0) {
        return -1;
      }
      int c = str[index];
      if ((c & 0xfc00) == 0xd800 && index + 1 < str.Length &&
        (str[index + 1] & 0xfc00) == 0xdc00) {
        // Get the Unicode code point for the surrogate pair
        c = 0x10000 + ((c & 0x3ff) << 10) + (str[index + 1] & 0x3ff);
      } else if ((c & 0xf800) == 0xd800) {
        // unpaired surrogate
        return (surrogateBehavior == 0) ? 0xfffd : ((surrogateBehavior == 1) ?
            c : (-1));
      }
      return c;
    }

    /// <summary>Returns a string with the basic upper-case letters A to Z
    /// (U+0041 to U+005A) converted to the corresponding basic lower-case
    /// letters. Other characters remain unchanged.</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <returns>The converted string, or null if <paramref name='str'/> is
    /// null.</returns>
    public static string ToLowerCaseAscii(string str) {
      if (str == null) {
        return null;
      }
      var len = str.Length;
      var c = (char)0;
      var hasUpperCase = false;
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

    /// <summary>Returns a string with the basic lower-case letters A to Z
    /// (U+0061 to U+007A) converted to the corresponding basic upper-case
    /// letters. Other characters remain unchanged.</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <returns>The converted string, or null if <paramref name='str'/> is
    /// null.</returns>
    public static string ToUpperCaseAscii(string str) {
      if (str == null) {
        return null;
      }
      var len = str.Length;
      var c = (char)0;
      var hasLowerCase = false;
      for (var i = 0; i < len; ++i) {
        c = str[i];
        if (c >= 'a' && c <= 'z') {
          hasLowerCase = true;
          break;
        }
      }
      if (!hasLowerCase) {
        return str;
      }
      var builder = new StringBuilder();
      for (var i = 0; i < len; ++i) {
        c = str[i];
        if (c >= 'a' && c <= 'z') {
          builder.Append((char)(c - 0x20));
        } else {
          builder.Append(c);
        }
      }
      return builder.ToString();
    }

    /// <summary>Compares two strings in Unicode code point order. Unpaired
    /// surrogate code points are treated as individual code
    /// points.</summary>
    /// <param name='strA'>The first string. Can be null.</param>
    /// <param name='strB'>The second string. Can be null.</param>
    /// <returns>A value indicating which string is " less" or " greater" .
    /// 0: Both strings are equal or null. Less than 0: a is null and b
    /// isn't; or the first code point that's different is less in A than
    /// in B; or b starts with a and is longer than a. Greater than 0: b is
    /// null and a isn't; or the first code point that's different is
    /// greater in A than in B; or a starts with b and is longer than
    /// b.</returns>
    public static int CodePointCompare(string strA, string strB) {
      if (strA == null) {
        return (strB == null) ? 0 : -1;
      }
      if (strB == null) {
        return 1;
      }
      int len, ca, cb;
      len = Math.Min(strA.Length, strB.Length);
      for (var i = 0; i < len; ++i) {
        ca = strA[i];
        cb = strB[i];
        if (ca == cb) {
          // normal code units and illegal surrogates
          // are treated as single code points
          if ((ca & 0xf800) != 0xd800) {
            continue;
          }
          var incindex = false;
          if (i + 1 < strA.Length && (strA[i + 1] & 0xfc00) == 0xdc00) {
            ca = 0x10000 + ((ca & 0x3ff) << 10) + (strA[i + 1] & 0x3ff);
            incindex = true;
          }
          if (i + 1 < strB.Length && (strB[i + 1] & 0xfc00) == 0xdc00) {
            cb = 0x10000 + ((cb & 0x3ff) << 10) + (strB[i + 1] & 0x3ff);
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
            (strA[i + 1] & 0xfc00) == 0xdc00) {
            ca = 0x10000 + ((ca & 0x3ff) << 10) + (strA[i + 1] & 0x3ff);
          }
          if ((cb & 0xfc00) == 0xd800 && i + 1 < strB.Length &&
            (strB[i + 1] & 0xfc00) == 0xdc00) {
            cb = 0x10000 + ((cb & 0x3ff) << 10) + (strB[i + 1] & 0x3ff);
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
    /// <param name='offset'>The Index starting at 0 where the string
    /// portion to write begins.</param>
    /// <param name='length'>The length of the string portion to
    /// write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <param name='replace'>If true, replaces unpaired surrogate code
    /// points with the replacement character (U+FFFD). If false, stops
    /// processing when an unpaired surrogate code point is seen.</param>
    /// <returns>0 if the entire string portion was written; or -1 if the
    /// string portion contains an unpaired surrogate code point and
    /// <paramref name='replace'/> is false.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null or <paramref name='stream'/> is
    /// null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='length'/> is less than 0 or
    /// greater than <paramref name='str'/> 's length, or <paramref
    /// name='str'/> 's length minus <paramref name='offset'/> is less than
    /// <paramref name='length'/>.</exception>
    public static int WriteUtf8(
      string str,
      int offset,
      int length,
      Stream stream,
      bool replace) {
      return WriteUtf8(str, offset, length, stream, replace, false);
    }

    /// <summary>Writes a portion of a string in UTF-8 encoding to a data
    /// stream.</summary>
    /// <param name='str'>A string to write.</param>
    /// <param name='offset'>The Index starting at 0 where the string
    /// portion to write begins.</param>
    /// <param name='length'>The length of the string portion to
    /// write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <param name='replace'>If true, replaces unpaired surrogate code
    /// points with the replacement character (U+FFFD). If false, stops
    /// processing when an unpaired surrogate code point is seen.</param>
    /// <param name='lenientLineBreaks'>If true, replaces carriage return
    /// (CR) not followed by line feed (LF) and LF not preceded by CR with
    /// CR-LF pairs.</param>
    /// <returns>0 if the entire string portion was written; or -1 if the
    /// string portion contains an unpaired surrogate code point and
    /// <paramref name='replace'/> is false.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null or <paramref name='stream'/> is
    /// null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='offset'/> is less than 0, <paramref name='length'/> is less
    /// than 0, or <paramref name='offset'/> plus <paramref name='length'/>
    /// is greater than the string's length.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static int WriteUtf8(
      string str,
      int offset,
      int length,
      Stream stream,
      bool replace,
      bool lenientLineBreaks) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (offset < 0) {
        throw new ArgumentException("offset(" + offset + ") is less than " +
          "0");
      }
      if (offset > str.Length) {
        throw new ArgumentException("offset(" + offset + ") is more than " +
          str.Length);
      }
      if (length < 0) {
        throw new ArgumentException("length(" + length + ") is less than " +
          "0");
      }
      if (length > str.Length) {
        throw new ArgumentException("length(" + length + ") is more than " +
          str.Length);
      }
      if (str.Length - offset < length) {
        throw new ArgumentException("str.Length minus offset(" +
          (str.Length - offset) + ") is less than " + length);
      }
      if (length == 0) {
        return 0;
      }
      int endIndex, c;
      byte[] bytes;
      var retval = 0;
      // Take string portion's length into account when allocating
      // stream buffer, in case it's much smaller than the usual stream
      // string buffer length and to improve performance on small strings
      int bufferLength = Math.Min(StreamedStringBufferLength, length);
      if (bufferLength < StreamedStringBufferLength) {
        bufferLength = Math.Min(
          StreamedStringBufferLength,
          bufferLength * 3);
      }
      bytes = new byte[bufferLength];
      var byteIndex = 0;
      endIndex = offset + length;
      for (int index = offset; index < endIndex; ++index) {
        c = str[index];
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
            (str[index + 1] & 0xfc00) == 0xdc00) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c & 0x3ff) << 10) + (str[index + 1] & 0x3ff);
            ++index;
          } else if ((c & 0xf800) == 0xd800) {
            // unpaired surrogate
            if (!replace) {
              retval = -1;
              break; // write bytes read so far
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
    /// points with the replacement character (U+FFFD). If false, stops
    /// processing when an unpaired surrogate code point is seen.</param>
    /// <returns>0 if the entire string was written; or -1 if the string
    /// contains an unpaired surrogate code point and <paramref
    /// name='replace'/> is false.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null or <paramref name='stream'/> is
    /// null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static int WriteUtf8(string str, Stream stream, bool replace) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      return WriteUtf8(str, 0, str.Length, stream, replace);
    }

    /// <summary>Reads a string in UTF-8 encoding from a byte
    /// array.</summary>
    /// <param name='data'>A byte array containing a UTF-8 text
    /// string.</param>
    /// <param name='offset'>Offset into the byte array to start
    /// reading.</param>
    /// <param name='bytesCount'>Length, in bytes, of the UTF-8 text
    /// string.</param>
    /// <param name='builder'>A string builder object where the resulting
    /// string will be stored.</param>
    /// <param name='replace'>If true, replaces invalid encoding with the
    /// replacement character (U+FFFD). If false, stops processing when
    /// invalid UTF-8 is seen.</param>
    /// <returns>0 if the entire string was read without errors, or -1 if
    /// the string is not valid UTF-8 and <paramref name='replace'/> is
    /// false.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='data'/> is null or <paramref name='builder'/> is
    /// null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='offset'/> is less than 0, <paramref name='bytesCount'/> is
    /// less than 0, or offset plus bytesCount is greater than the length
    /// of <paramref name='data'/>.</exception>
    public static int ReadUtf8FromBytes(
      byte[] data,
      int offset,
      int bytesCount,
      StringBuilder builder,
      bool replace) {
      if (data == null) {
        throw new ArgumentNullException(nameof(data));
      }
      if (offset < 0) {
        throw new ArgumentException("offset(" + offset + ") is less than " +
          "0");
      }
      if (offset > data.Length) {
        throw new ArgumentException("offset(" + offset + ") is more than " +
          data.Length);
      }
      if (bytesCount < 0) {
        throw new ArgumentException("bytesCount(" + bytesCount +
          ") is less than 0");
      }
      if (bytesCount > data.Length) {
        throw new ArgumentException("bytesCount(" + bytesCount +
          ") is more than " + data.Length);
      }
      if (data.Length - offset < bytesCount) {
        throw new ArgumentException("data.Length minus offset(" +
          (data.Length - offset) + ") is less than " + bytesCount);
      }
      if (builder == null) {
        throw new ArgumentNullException(nameof(builder));
      }
      var cp = 0;
      var bytesSeen = 0;
      var bytesNeeded = 0;
      var lower = 0x80;
      var upper = 0xbf;
      int pointer, endpointer, b;
      pointer = offset;
      endpointer = offset + bytesCount;
      while (pointer < endpointer) {
        b = data[pointer] & (int)0xff;
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
          int ret, ch, lead, trail;
          ret = cp;
          cp = 0;
          bytesSeen = 0;
          bytesNeeded = 0;
          if (ret <= 0xffff) {
            builder.Append((char)ret);
          } else {
            ch = ret - 0x10000;
            lead = (ch >> 10) + 0xd800;
            trail = (ch & 0x3ff) + 0xdc00;
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
    /// replacement character (U+FFFD).</summary>
    /// <param name='stream'>A readable data stream.</param>
    /// <returns>The string read.</returns>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
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
    /// replacement character (U+FFFD). If false, throws an error if an
    /// unpaired surrogate code point is seen.</param>
    /// <returns>The string read.</returns>
    /// <exception cref='System.IO.IOException'>An I/O error occurred; or,
    /// the string is not valid UTF-8 and <paramref name='replace'/> is
    /// false.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    public static string ReadUtf8ToString(
      Stream stream,
      int bytesCount,
      bool replace) {
      var builder = new StringBuilder();
      if (DataUtilities.ReadUtf8(stream, bytesCount, builder, replace) == -1) {
        throw new IOException(
          "Unpaired surrogate code point found.",
          new ArgumentException("Unpaired surrogate code point found."));
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
    /// replacement character (U+FFFD). If false, stops processing when an
    /// unpaired surrogate code point is seen.</param>
    /// <returns>0 if the entire string was read without errors, -1 if the
    /// string is not valid UTF-8 and <paramref name='replace'/> is false,
    /// or -2 if the end of the stream was reached before the last
    /// character was read completely (which is only the case if <paramref
    /// name='bytesCount'/> is 0 or greater).</returns>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null or <paramref name='builder'/> is
    /// null.</exception>
    public static int ReadUtf8(
      Stream stream,
      int bytesCount,
      StringBuilder builder,
      bool replace) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (builder == null) {
        throw new ArgumentNullException(nameof(builder));
      }
      int b;
      var cp = 0;
      var bytesSeen = 0;
      var bytesNeeded = 0;
      var lower = 0x80;
      var upper = 0xbf;
      var pointer = 0;
      while (pointer < bytesCount || bytesCount < 0) {
        b = stream.ReadByte();
        if (b < 0) {
          if (bytesNeeded != 0) {
            bytesNeeded = 0;
            if (replace) {
              builder.Append((char)0xfffd);
              if (bytesCount >= 0) {
                return -2;
              }
              break; // end of stream
            }
            return -1;
          }
          if (bytesCount >= 0) {
            return -2;
          }
          break; // end of stream
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
          int ret, ch, lead, trail;
          ret = cp;
          cp = 0;
          bytesSeen = 0;
          bytesNeeded = 0;
          if (ret <= 0xffff) {
            builder.Append((char)ret);
          } else {
            ch = ret - 0x10000;
            lead = (ch >> 10) + 0xd800;
            trail = (ch & 0x3ff) + 0xdc00;
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
