package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.io.*;

    /**
     * Contains methods useful for reading and writing strings. It is designed
     * to have no dependencies other than the basic runtime class library.
     */
  public final class DataUtilities {
private DataUtilities() {
}
    private static final int StreamedStringBufferLength = 4096;

    /**
     * Generates a text string from a UTF-8 byte array.
     * @param bytes A byte array containing text encoded in UTF-8.
     * @param replace If true, replaces invalid encoding with the replacement
     * character (U + FFFD). If false, stops processing when invalid UTF-8
     * is seen.
     * @return A string represented by the UTF-8 byte array.
     * @throws java.lang.NullPointerException The parameter {@code bytes}
     * is null.
     * @throws java.lang.IllegalArgumentException The string is not valid UTF-8
     * and {@code replace} is false.
     */
    public static String GetUtf8String(byte[] bytes, boolean replace) {
      if (bytes == null) {
        throw new NullPointerException("bytes");
      }
      StringBuilder b = new StringBuilder();
      if (ReadUtf8FromBytes(bytes, 0, bytes.length, b, replace) != 0) {
        throw new IllegalArgumentException("Invalid UTF-8");
      }
      return b.toString();
    }

    /**
     * Generates a text string from a portion of a UTF-8 byte array.
     * @param bytes A byte array containing text encoded in UTF-8.
     * @param offset Offset into the byte array to start reading.
     * @param bytesCount Length, in bytes, of the UTF-8 string.
     * @param replace If true, replaces invalid encoding with the replacement
     * character (U + FFFD). If false, stops processing when invalid UTF-8
     * is seen.
     * @return A string represented by the UTF-8 byte array.
     * @throws java.lang.NullPointerException The parameter {@code bytes}
     * is null.
     * @throws java.lang.IllegalArgumentException The portion of the byte array
     * is not valid UTF-8 and {@code replace} is false.
     * @throws java.lang.IllegalArgumentException The parameter {@code offset}
     * is less than 0, {@code bytesCount} is less than 0, or offset plus bytesCount
     * is greater than the length of "data" .
     */
    public static String GetUtf8String(byte[] bytes, int offset, int bytesCount, boolean replace) {
      if (bytes == null) {
        throw new NullPointerException("bytes");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is less than " + "0");
      }
      if (offset > bytes.length) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is more than " + Long.toString((long)bytes.length));
      }
      if (bytesCount < 0) {
        throw new IllegalArgumentException("bytesCount (" + Long.toString((long)bytesCount) + ") is less than " + "0");
      }
      if (bytesCount > bytes.length) {
        throw new IllegalArgumentException("bytesCount (" + Long.toString((long)bytesCount) + ") is more than " + Long.toString((long)bytes.length));
      }
      if (bytes.length - offset < bytesCount) {
        throw new IllegalArgumentException("bytes's length minus " + offset + " (" + Long.toString((long)(bytes.length - offset)) + ") is less than " + Long.toString((long)bytesCount));
      }
      StringBuilder b = new StringBuilder();
      if (ReadUtf8FromBytes(bytes, offset, bytesCount, b, replace) != 0) {
        throw new IllegalArgumentException("Invalid UTF-8");
      }
      return b.toString();
    }

    /**
     * Encodes a string in UTF-8 as a byte array.
     * @param str A text string.
     * @param replace If true, replaces unpaired surrogate code points
     * with the replacement character (U + FFFD). If false, stops processing
     * when an unpaired surrogate code point is seen.
     * @return The string encoded in UTF-8.
     * @throws java.lang.NullPointerException The parameter {@code str}
     * is null.
     * @throws java.lang.IllegalArgumentException The string contains an unpaired
     * surrogate code point and {@code replace} is false, or an internal
     * error occurred.
     * @throws java.lang.IllegalArgumentException The parameter "offset" is less
     * than 0, "bytesCount" is less than 0, or offset plus bytesCount is greater
     * than the length of "data" .
     */
    public static byte[] GetUtf8Bytes(String str, boolean replace) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      try {
        java.io.ByteArrayOutputStream ms=null;
try {
ms=new java.io.ByteArrayOutputStream();

          if (WriteUtf8(str, ms, replace) != 0) {
            throw new IllegalArgumentException("Unpaired surrogate code point");
          }
          return ms.toByteArray();
}
finally {
try { if(ms!=null)ms.close(); } catch (java.io.IOException ex){}
}
      } catch (IOException ex) {
        throw new IllegalArgumentException("I/O error occurred", ex);
      }
    }

    /**
     * Calculates the number of bytes needed to encode a string in UTF-8.
     * @param str A string object.
     * @param replace If true, treats unpaired surrogate code points as
     * having 3 UTF-8 bytes (the UTF-8 length of the replacement character
     * U + FFFD).
     * @return The number of bytes needed to encode the given string in UTF-8,
     * or -1 if the string contains an unpaired surrogate code point and {@code
     * replace} is false.
     * @throws java.lang.NullPointerException The parameter {@code str}
     * is null.
     */
    public static long GetUtf8Length(String str, boolean replace) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      long size = 0;
      for (int i = 0; i < str.length(); ++i) {
        int c = str.charAt(i);
        if (c <= 0x7f) {
          ++size;
        } else if (c <= 0x7ff) {
          size += 2;
        } else if (c <= 0xd7ff || c >= 0xe000) {
          size += 3;
        } else if (c <= 0xdbff) {  // UTF-16 leading surrogate
          ++i;
          if (i >= str.length() || str.charAt(i) < 0xdc00 || str.charAt(i) > 0xdfff) {
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

    /**
     * Gets the Unicode code point just before the given index of the string.
     * @param str A string.
     * @param index Index of the current position into the string.
     * @return The Unicode code point at the previous position. Returns
     * -1 if {@code index} is 0 or less, or is greater than the string's length.
     * Returns the replacement character (U + FFFD) if the previous character
     * is an unpaired surrogate code point.
     * @throws java.lang.NullPointerException The parameter {@code str}
     * is null.
     */
    public static int CodePointBefore(String str, int index) {
      return CodePointBefore(str, index, 0);
    }

    /**
     * Gets the Unicode code point just before the given index of the string.
     * @param str A string.
     * @param index Index of the current position into the string.
     * @param surrogateBehavior Specifies what kind of value to return
     * if the previous character is an unpaired surrogate code point: if
     * 0, return the replacement character (U + FFFD); if 1, return the value
     * of the surrogate code point; if neither 0 nor 1, return -1.
     * @return The Unicode code point at the previous position. Returns
     * -1 if {@code index} is 0 or less, or is greater than the string's length.
     * Returns a value as specified under {@code surrogateBehavior} if
     * the previous character is an unpaired surrogate code point.
     * @throws java.lang.NullPointerException The parameter {@code str}
     * is null.
     */
    public static int CodePointBefore(String str, int index, int surrogateBehavior) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index <= 0) {
        return -1;
      }
      if (index > str.length()) {
        return -1;
      }
      int c = str.charAt(index - 1);
      if ((c & 0xfc00) == 0xdc00 && index - 2 >= 0 &&
             str.charAt(index - 2) >= 0xd800 && str.charAt(index - 2) <= 0xdbff) {
        // Get the Unicode code point for the surrogate pair
        return 0x10000 + ((str.charAt(index - 2) - 0xd800) << 10) + (c - 0xdc00);
      }
      if ((c & 0xf800) == 0xd800) {
        // unpaired surrogate
        return (surrogateBehavior == 0) ? 0xfffd : ((surrogateBehavior == 1) ? c : (-1));
      }
      return c;
    }

    /**
     * Gets the Unicode code point at the given index of the string.
     * @param str A string.
     * @param index Index of the current position into the string.
     * @return The Unicode code point at the given position. Returns -1 if
     * {@code index} is less than 0, or is the string's length or greater.
     * Returns the replacement character (U + FFFD) if the current character
     * is an unpaired surrogate code point.
     * @throws java.lang.NullPointerException The parameter {@code str}
     * is null.
     */
    public static int CodePointAt(String str, int index) {
      return CodePointAt(str, index, 0);
    }

    /**
     * Gets the Unicode code point at the given index of the string.
     * @param str A string.
     * @param index Index of the current position into the string.
     * @param surrogateBehavior Specifies what kind of value to return
     * if the previous character is an unpaired surrogate code point: if
     * 0, return the replacement character (U + FFFD); if 1, return the value
     * of the surrogate code point; if neither 0 nor 1, return -1.
     * @return The Unicode code point at the current position. Returns -1
     * if {@code index} is less than 0, or is the string's length or greater.
     * Returns a value as specified under {@code surrogateBehavior} if
     * the previous character is an unpaired surrogate code point.
     * @throws java.lang.NullPointerException The parameter {@code str}
     * is null.
     */
    public static int CodePointAt(String str, int index, int surrogateBehavior) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index >= str.length()) {
        return -1;
      }
      if (index < 0) {
        return -1;
      }
      int c = str.charAt(index);
      if ((c & 0xfc00) == 0xd800 && index + 1 < str.length() &&
          str.charAt(index + 1) >= 0xdc00 && str.charAt(index + 1) <= 0xdfff) {
        // Get the Unicode code point for the surrogate pair
        c = 0x10000 + ((c - 0xd800) << 10) + (str.charAt(index + 1) - 0xdc00);
        ++index;
      } else if ((c & 0xf800) == 0xd800) {
        // unpaired surrogate
        return (surrogateBehavior == 0) ? 0xfffd : ((surrogateBehavior == 1) ? c : (-1));
      }
      return c;
    }

    /**
     * Returns a string with upper-case ASCII letters (A to Z) converted
     * to lower-case. Other characters remain unchanged.
     * @param str A string.
     * @return The converted string, or null if {@code str} is null.
     */
    public static String ToLowerCaseAscii(String str) {
      if (str == null) {
        return null;
      }
      int len = str.length();
      char c = (char)0;
      boolean hasUpperCase = false;
      for (int i = 0; i < len; ++i) {
        c = str.charAt(i);
        if (c >= 'A' && c <= 'Z') {
          hasUpperCase = true;
          break;
        }
      }
      if (!hasUpperCase) {
        return str;
      }
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < len; ++i) {
        c = str.charAt(i);
        if (c >= 'A' && c <= 'Z') {
          builder.append((char)(c + 0x20));
        } else {
          builder.append(c);
        }
      }
      return builder.toString();
    }

    /**
     * Compares two strings in Unicode code point order. Unpaired surrogates
     * are treated as individual code points.
     * @param strA The first string. Can be null.
     * @param strB The second string. Can be null.
     * @return A value indicating which string is " less" or " greater" . 0:
     * Both strings are equal or null. Less than 0: a is null and b isn't; or
     * the first code point that's different is less in A than in B; or b starts
     * with a and is longer than a. Greater than 0: b is null and a isn't; or the
     * first code point that's different is greater in A than in B; or a starts
     * with b and is longer than b.
     */
    public static int CodePointCompare(String strA, String strB) {
      if (strA == null) {
        return (strB == null) ? 0 : -1;
      }
      if (strB == null) {
        return 1;
      }
      int len = Math.min(strA.length(), strB.length());
      for (int i = 0; i < len; ++i) {
        int ca = strA.charAt(i);
        int cb = strB.charAt(i);
        if (ca == cb) {
          // normal code units and illegal surrogates
          // are treated as single code points
          if ((ca & 0xf800) != 0xd800) {
            continue;
          }
          boolean incindex = false;
          if (i + 1 < strA.length() && strA.charAt(i + 1) >= 0xdc00 && strA.charAt(i + 1) <= 0xdfff) {
            ca = 0x10000 + ((ca - 0xd800) << 10) + (strA.charAt(i + 1) - 0xdc00);
            incindex = true;
          }
          if (i + 1 < strB.length() && strB.charAt(i + 1) >= 0xdc00 && strB.charAt(i + 1) <= 0xdfff) {
            cb = 0x10000 + ((cb - 0xd800) << 10) + (strB.charAt(i + 1) - 0xdc00);
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
          if ((ca & 0xfc00) == 0xd800 && i + 1 < strA.length() &&
              strA.charAt(i + 1) >= 0xdc00 && strA.charAt(i + 1) <= 0xdfff) {
            ca = 0x10000 + ((ca - 0xd800) << 10) + (strA.charAt(i + 1) - 0xdc00);
          }
          if ((cb & 0xfc00) == 0xd800 && i + 1 < strB.length() &&
              strB.charAt(i + 1) >= 0xdc00 && strB.charAt(i + 1) <= 0xdfff) {
            cb = 0x10000 + ((cb - 0xd800) << 10) + (strB.charAt(i + 1) - 0xdc00);
          }
          return ca - cb;
        }
      }
      return (strA.length() == strB.length()) ? 0 : ((strA.length() < strB.length()) ? -1 : 1);
    }

    /**
     * Writes a portion of a string in UTF-8 encoding to a data stream.
     * @param str A string to write.
     * @param offset The zero-based index where the string portion to write
     * begins.
     * @param length The length of the string portion to write.
     * @param stream A writable data stream.
     * @param replace If true, replaces unpaired surrogate code points
     * with the replacement character (U + FFFD). If false, stops processing
     * when an unpaired surrogate code point is seen.
     * @return 0 if the entire string portion was written; or -1 if the string
     * portion contains an unpaired surrogate code point and {@code replace}
     * is false.
     * @throws java.lang.NullPointerException The parameter {@code str}
     * is null or {@code stream} is null.
     * @throws java.lang.IllegalArgumentException The parameter {@code offset}
     * is less than 0, {@code length} is less than 0, or {@code offset} plus
     * {@code length} is greater than the string's length.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static int WriteUtf8(String str, int offset, int length, OutputStream stream, boolean replace) throws IOException {
      return WriteUtf8(str, offset, length, stream, replace, false);
    }

    /**
     * Writes a portion of a string in UTF-8 encoding to a data stream.
     * @param str A string to write.
     * @param offset The zero-based index where the string portion to write
     * begins.
     * @param length The length of the string portion to write.
     * @param stream A writable data stream.
     * @param replace If true, replaces unpaired surrogate code points
     * with the replacement character (U + FFFD). If false, stops processing
     * when an unpaired surrogate code point is seen.
     * @param lenientLineBreaks If true, replaces carriage return (CR)
     * not followed by line feed (LF) and LF not preceded by CR with CR-LF pairs.
     * @return 0 if the entire string portion was written; or -1 if the string
     * portion contains an unpaired surrogate code point and {@code replace}
     * is false.
     * @throws java.lang.NullPointerException The parameter {@code str}
     * is null or {@code stream} is null.
     * @throws java.lang.IllegalArgumentException The parameter {@code offset}
     * is less than 0, {@code length} is less than 0, or {@code offset} plus
     * {@code length} is greater than the string's length.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static int WriteUtf8(String str, int offset, int length, OutputStream stream, boolean replace, boolean lenientLineBreaks) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is less than " + "0");
      }
      if (offset > str.length()) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is more than " + Long.toString((long)str.length()));
      }
      if (length < 0) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is less than " + "0");
      }
      if (length > str.length()) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is more than " + Long.toString((long)str.length()));
      }
      if (str.length() - offset < length) {
        throw new IllegalArgumentException("str.length() minus offset (" + Long.toString((long)str.length() - offset) + ") is less than " + Long.toString((long)length));
      }
      byte[] bytes;
      int retval = 0;
      bytes = new byte[StreamedStringBufferLength];
      int byteIndex = 0;
      int endIndex = offset + length;
      for (int index = offset; index < endIndex; ++index) {
        int c = str.charAt(index);
        if (c <= 0x7f) {
          if (lenientLineBreaks) {
            if (c == 0x0d && (index + 1 >= endIndex || str.charAt(index + 1) != 0x0a)) {
              // bare CR, convert to CRLF
              if (byteIndex + 2 > StreamedStringBufferLength) {
                // Write bytes retrieved so far
                stream.write(bytes,0,byteIndex);
                byteIndex = 0;
              }
              bytes[byteIndex++] = 0x0d;
              bytes[byteIndex++] = 0x0a;
              continue;
            }
            if (c == 0x0a) {
              // bare LF, convert to CRLF
              if (byteIndex + 2 > StreamedStringBufferLength) {
                // Write bytes retrieved so far
                stream.write(bytes,0,byteIndex);
                byteIndex = 0;
              }
              bytes[byteIndex++] = 0x0d;
              bytes[byteIndex++] = 0x0a;
              continue;
            }
          }
          if (byteIndex >= StreamedStringBufferLength) {
            // Write bytes retrieved so far
            stream.write(bytes,0,byteIndex);
            byteIndex = 0;
          }
          bytes[byteIndex++] = (byte)c;
        } else if (c <= 0x7ff) {
          if (byteIndex + 2 > StreamedStringBufferLength) {
            // Write bytes retrieved so far
            stream.write(bytes,0,byteIndex);
            byteIndex = 0;
          }
          bytes[byteIndex++] = (byte)(0xc0 | ((c >> 6) & 0x1f));
          bytes[byteIndex++] = (byte)(0x80 | (c & 0x3f));
        } else {
          if ((c & 0xfc00) == 0xd800 && index + 1 < endIndex &&
              str.charAt(index + 1) >= 0xdc00 && str.charAt(index + 1) <= 0xdfff) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c - 0xd800) << 10) + (str.charAt(index + 1) - 0xdc00);
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
              stream.write(bytes,0,byteIndex);
              byteIndex = 0;
            }
            bytes[byteIndex++] = (byte)(0xe0 | ((c >> 12) & 0x0f));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 6) & 0x3f));
            bytes[byteIndex++] = (byte)(0x80 | (c & 0x3f));
          } else {
            if (byteIndex + 4 > StreamedStringBufferLength) {
              // Write bytes retrieved so far
              stream.write(bytes,0,byteIndex);
              byteIndex = 0;
            }
            bytes[byteIndex++] = (byte)(0xf0 | ((c >> 18) & 0x07));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 12) & 0x3f));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 6) & 0x3f));
            bytes[byteIndex++] = (byte)(0x80 | (c & 0x3f));
          }
        }
      }
      stream.write(bytes,0,byteIndex);
      return retval;
    }

    /**
     * Writes a string in UTF-8 encoding to a data stream.
     * @param str A string to write.
     * @param stream A writable data stream.
     * @param replace If true, replaces unpaired surrogate code points
     * with the replacement character (U + FFFD). If false, stops processing
     * when an unpaired surrogate code point is seen.
     * @return 0 if the entire string was written; or -1 if the string contains
     * an unpaired surrogate code point and {@code replace} is false.
     * @throws java.lang.NullPointerException The parameter {@code str}
     * is null or {@code stream} is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static int WriteUtf8(String str, OutputStream stream, boolean replace) throws IOException {
      if (str == null) {
        throw new NullPointerException("str");
      }
      return WriteUtf8(str, 0, str.length(), stream, replace);
    }

    /**
     * Reads a string in UTF-8 encoding from a byte array.
     * @param data A byte array containing a UTF-8 string.
     * @param offset Offset into the byte array to start reading.
     * @param bytesCount Length, in bytes, of the UTF-8 string.
     * @param builder A string builder object where the resulting string
     * will be stored.
     * @param replace If true, replaces invalid encoding with the replacement
     * character (U + FFFD). If false, stops processing when invalid UTF-8
     * is seen.
     * @return 0 if the entire string was read without errors, or -1 if the
     * string is not valid UTF-8 and {@code replace} is false.
     * @throws java.lang.NullPointerException The parameter {@code data}
     * is null or {@code builder} is null.
     * @throws java.lang.IllegalArgumentException The parameter {@code offset}
     * is less than 0, {@code bytesCount} is less than 0, or offset plus bytesCount
     * is greater than the length of {@code data} .
     */
    public static int ReadUtf8FromBytes(
      byte[] data,
      int offset,
      int bytesCount,
      StringBuilder builder,
      boolean replace) {
      if (data == null) {
        throw new NullPointerException("data");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is less than " + "0");
      }
      if (offset > data.length) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is more than " + Long.toString((long)data.length));
      }
      if (bytesCount < 0) {
        throw new IllegalArgumentException("bytesCount (" + Long.toString((long)bytesCount) + ") is less than " + "0");
      }
      if (bytesCount > data.length) {
        throw new IllegalArgumentException("bytesCount (" + Long.toString((long)bytesCount) + ") is more than " + Long.toString((long)data.length));
      }
      if (data.length - offset < bytesCount) {
        throw new IllegalArgumentException("data.length minus offset (" + Long.toString((long)data.length - offset) + ") is less than " + Long.toString((long)bytesCount));
      }
      if (builder == null) {
        throw new NullPointerException("builder");
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
            builder.append((char)b);
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
              builder.append((char)0xfffd);
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
            builder.append((char)0xfffd);
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
            builder.append((char)ret);
          } else {
            int ch = ret - 0x10000;
            int lead = (ch / 0x400) + 0xd800;
            int trail = (ch & 0x3ff) + 0xdc00;
            builder.append((char)lead);
            builder.append((char)trail);
          }
        }
      }
      if (bytesNeeded != 0) {
        if (replace) {
          builder.append((char)0xfffd);
        } else {
          return -1;
        }
      }
      return 0;
    }

    /**
     * Reads a string in UTF-8 encoding from a data stream in full and returns
     * that string. Replaces invalid encoding with the replacement character
     * (U + FFFD).
     * @param stream A readable data stream.
     * @return The string read.
     * @throws java.io.IOException An I/O error occurred.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     */
    public static String ReadUtf8ToString(
      InputStream stream) throws IOException {
      return ReadUtf8ToString(stream, -1, true);
    }

    /**
     * Reads a string in UTF-8 encoding from a data stream and returns that
     * string.
     * @param stream A readable data stream.
     * @param bytesCount The length, in bytes, of the string. If this is less
     * than 0, this function will read until the end of the stream.
     * @param replace If true, replaces invalid encoding with the replacement
     * character (U + FFFD). If false, throws an error if an unpaired surrogate
     * code point is seen.
     * @return The string read.
     * @throws java.io.IOException An I/O error occurred; or, the string
     * is not valid UTF-8 and {@code replace} is false.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     */
    public static String ReadUtf8ToString(
      InputStream stream,
      int bytesCount,
      boolean replace) throws IOException {
      StringBuilder builder = new StringBuilder();
      int retval = DataUtilities.ReadUtf8(stream, bytesCount, builder, replace);
      if (retval == -1) {
        throw new IOException("Unpaired surrogate code point found.", new java.nio.charset.MalformedInputException(1));
      }
      return builder.toString();
    }

    /**
     * Reads a string in UTF-8 encoding from a data stream.
     * @param stream A readable data stream.
     * @param bytesCount The length, in bytes, of the string. If this is less
     * than 0, this function will read until the end of the stream.
     * @param builder A string builder object where the resulting string
     * will be stored.
     * @param replace If true, replaces invalid encoding with the replacement
     * character (U + FFFD). If false, stops processing when an unpaired
     * surrogate code point is seen.
     * @return 0 if the entire string was read without errors, -1 if the string
     * is not valid UTF-8 and {@code replace} is false, or -2 if the end of the
     * stream was reached before the last character was read completely
     * (which is only the case if {@code bytesCount} is 0 or greater).
     * @throws java.io.IOException An I/O error occurred.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null or {@code builder} is null.
     */
    public static int ReadUtf8(
      InputStream stream,
      int bytesCount,
      StringBuilder builder,
      boolean replace) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      if (builder == null) {
        throw new NullPointerException("builder");
      }
      int cp = 0;
      int bytesSeen = 0;
      int bytesNeeded = 0;
      int lower = 0x80;
      int upper = 0xbf;
      int pointer = 0;
      while (pointer < bytesCount || bytesCount < 0) {
        int b = stream.read();
        if (b < 0) {
          if (bytesNeeded != 0) {
            bytesNeeded = 0;
            if (replace) {
              builder.append((char)0xfffd);
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
            builder.append((char)b);
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
              builder.append((char)0xfffd);
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
            builder.append((char)0xfffd);
            // "Read" the last byte again
            if (b < 0x80) {
              builder.append((char)b);
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
              builder.append((char)0xfffd);
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
            builder.append((char)ret);
          } else {
            int ch = ret - 0x10000;
            int lead = (ch / 0x400) + 0xd800;
            int trail = (ch & 0x3ff) + 0xdc00;
            builder.append((char)lead);
            builder.append((char)trail);
          }
        }
      }
      if (bytesNeeded != 0) {
        if (replace) {
          builder.append((char)0xfffd);
        } else {
          return -1;
        }
      }
      return 0;
    }
  }
