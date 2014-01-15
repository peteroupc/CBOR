package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

import java.io.*;

    /**
     * Contains methods useful for reading and writing strings. It is designed
     * to have no dependencies other than the basic runtime class library.
     */
  public final class DataUtilities {
private DataUtilities(){}
    private static int StreamedStringBufferLength = 4096;
    /**
     * Generates a text string from a UTF-8 byte array.
     * @param bytes A byte array containing text encoded in UTF-8.
     * @param replace If true, replaces invalid encoding with the replacement
     * character (U + FFFD). If false, stops processing when invalid UTF-8
     * is seen.
     * @return A string represented by the UTF-8 byte array.
     * @throws java.lang.NullPointerException "bytes" is null.
     * @throws java.lang.IllegalArgumentException The string is not valid UTF-8
     * and "replace" is false
     */
    public static String GetUtf8String(byte[] bytes, boolean replace) {
      if (bytes == null)throw new NullPointerException("bytes");
      StringBuilder b = new StringBuilder();
      if (ReadUtf8FromBytes(bytes, 0, bytes.length, b, replace) != 0) {
 throw new IllegalArgumentException("Invalid UTF-8");
}
      return b.toString();
    }
    /**
     * Generates a text string from a portion of a UTF-8 byte array.
     * @param bytes A byte array containing text encoded in UTF-8.
     * @param offset Offset into the byte array to start reading
     * @param bytesCount Length, in bytes, of the UTF-8 string
     * @param replace If true, replaces invalid encoding with the replacement
     * character (U + FFFD). If false, stops processing when invalid UTF-8
     * is seen.
     * @return A string represented by the UTF-8 byte array.
     * @throws java.lang.NullPointerException "bytes" is null.
     * @throws java.lang.IllegalArgumentException The portion of the byte array
     * is not valid UTF-8 and "replace" is false
     */
    public static String GetUtf8String(byte[] bytes, int offset, int bytesCount, boolean replace) {
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
     * @throws java.lang.NullPointerException "str" is null.
     * @throws java.lang.IllegalArgumentException The string contains an unpaired
     * surrogate code point and "replace" is false, or an internal error
     * occurred.
     */
    public static byte[] GetUtf8Bytes(String str, boolean replace) {
      try {
        java.io.ByteArrayOutputStream ms=null;
try {
ms=new ByteArrayOutputStream();

          if (WriteUtf8(str, ms, replace) != 0) {
 throw new IllegalArgumentException("Unpaired surrogate code point");
}
          return ms.toByteArray();
}
finally {
try { if(ms!=null)ms.close(); } catch(IOException ex){}
}
      } catch (IOException ex) {
        throw new IllegalArgumentException("I/O error occurred", ex);
      }
    }
    /**
     * Calculates the number of bytes needed to encode a string in UTF-8.
     * @param replace If true, treats unpaired surrogate code points as
     * replacement characters (U + FFFD) instead, meaning each one takes
     * 3 UTF-8 bytes. If false, stops processing when an unpaired surrogate
     * code point is reached.
     * @param str A string object.
     * @return The number of bytes needed to encode the given string in UTF-8,
     * or -1 if the string contains an unpaired surrogate code point and &quot;
     * replace&quot; is false.
     * @throws java.lang.NullPointerException "s" is null.
     */
    public static long GetUtf8Length(String str, boolean replace) {
      if (str == null) { throw new NullPointerException("str"); }
      long size = 0;
      for (int i = 0; i < str.length(); ++i) {
        int c = str.charAt(i);
        if (c <= 0x7F) {
          size++;
        } else if (c <= 0x7FF) {
          size += 2;
        } else if (c <= 0xD7FF || c >= 0xE000) {
          size += 3;
        } else if (c <= 0xDBFF) {  // UTF-16 leading surrogate
          i++;
          if (i >= str.length() || str.charAt(i) < 0xDC00 || str.charAt(i) > 0xDFFF) {
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
    /**
     * Compares two strings in Unicode code point order. Unpairedsurrogates
     * are treated as individual code points.
     * @param strA The first string.
     * @param strB The second string.
     * @return A value indicating which string is &quot; less&quot; or &quot;
     * greater&quot; . 0: Both strings are equal or null. Less than 0: a is
     * null and b isn&apos; t; or the first code point that&apos; s different
     * is less in A than in B; or b starts with a and is longer than a. Greater
     * than 0: b is null and a isn&apos; t; or the first code point that&apos;
     * s different is greater in A than in B; or a starts with b and is longer
     * than b.
     */
    public static int CodePointCompare(String strA, String strB) {
      if (strA == null) { return (strB == null) ? 0 : -1; }
      if (strB == null) { return 1; }
      int len = Math.min(strA.length(), strB.length());
      for (int i = 0; i < len; ++i) {
        int ca = strA.charAt(i);
        int cb = strB.charAt(i);
        if (ca == cb) {
          // normal code units and illegal surrogates
          // are treated as single code points
          if ((ca & 0xF800) != 0xD800) {
            continue;
          }
          boolean incindex = false;
          if (i + 1 < strA.length() && strA.charAt(i + 1) >= 0xDC00 && strA.charAt(i + 1) <= 0xDFFF) {
            ca = 0x10000 + (ca - 0xD800) * 0x400 + (strA.charAt(i + 1) - 0xDC00);
            incindex = true;
          }
          if (i + 1 < strB.length() && strB.charAt(i + 1) >= 0xDC00 && strB.charAt(i + 1) <= 0xDFFF) {
            cb = 0x10000 + (cb - 0xD800) * 0x400 + (strB.charAt(i + 1) - 0xDC00);
            incindex = true;
          }
          if (ca != cb) { return ca - cb; }
          if (incindex) {
            i++;
          }
        } else {
          if ((ca & 0xF800) != 0xD800 && (cb & 0xF800) != 0xD800)
            return ca - cb;
          if (ca >= 0xd800 && ca <= 0xdbff && i + 1 < strA.length() &&
              strA.charAt(i + 1) >= 0xDC00 && strA.charAt(i + 1) <= 0xDFFF) {
            ca = 0x10000 + (ca - 0xD800) * 0x400 + (strA.charAt(i + 1) - 0xDC00);
          }
          if (cb >= 0xd800 && cb <= 0xdbff && i + 1 < strB.length() &&
              strB.charAt(i + 1) >= 0xDC00 && strB.charAt(i + 1) <= 0xDFFF) {
            cb = 0x10000 + (cb - 0xD800) * 0x400 + (strB.charAt(i + 1) - 0xDC00);
          }
          return ca - cb;
        }
      }
      if (strA.length() == strB.length()) { return 0; }
      return (strA.length() < strB.length()) ? -1 : 1;
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
     * portion contains an unpaired surrogate code point and &quot; replace&quot;
     * is false.
     * @throws java.lang.NullPointerException "str" is null or "stream"
     * is null.
     * @throws java.lang.IllegalArgumentException "offset" is less than 0, "length"
     * is less than 0, or "offset" plus "length" is greater than the string's
     * length.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static int WriteUtf8(String str, int offset, int length, OutputStream stream, boolean replace) throws IOException {
      if (stream == null) { throw new NullPointerException("stream"); }
      if (str == null) { throw new NullPointerException("str"); }
      if (offset < 0) { throw new IllegalArgumentException("offset" + " not greater or equal to " + "0" + " ("+offset+")"); }
      if (offset > str.length()) { throw new IllegalArgumentException("offset" + " not less or equal to "+str.length()+" ("+offset+")"); }
      if (length < 0) { throw new IllegalArgumentException("length" + " not greater or equal to " + "0" + " ("+length+")"); }
      if (length > str.length()) { throw new IllegalArgumentException("length" + " not less or equal to "+str.length()+" ("+length+")"); }
      if ((str.length() - offset) < length) throw new IllegalArgumentException("str's length minus " + offset + " not greater or equal to "+length+" ("+str.length() - offset+")");
      byte[] bytes;
      int retval = 0;
      bytes = new byte[StreamedStringBufferLength];
      int byteIndex = 0;
      int endIndex = offset + length;
      for (int index = offset; index < endIndex; ++index) {
        int c = str.charAt(index);
        if (c <= 0x7F) {
          if (byteIndex >= StreamedStringBufferLength) {
            // Write bytes retrieved so far
            stream.write(bytes,0,byteIndex);
            byteIndex = 0;
          }
          bytes[byteIndex++] = (byte)c;
        } else if (c <= 0x7FF) {
          if (byteIndex + 2 > StreamedStringBufferLength) {
            // Write bytes retrieved so far
            stream.write(bytes,0,byteIndex);
            byteIndex = 0;
          }
          bytes[byteIndex++] = (byte)(0xC0 | ((c >> 6) & 0x1F));
          bytes[byteIndex++] = (byte)(0x80 | (c & 0x3F));
        } else {
          if (c >= 0xD800 && c <= 0xDBFF && index + 1 < endIndex &&
              str.charAt(index + 1) >= 0xDC00 && str.charAt(index + 1) <= 0xDFFF) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + (c - 0xD800) * 0x400 + (str.charAt(index + 1) - 0xDC00);
            index++;
          } else if (c >= 0xD800 && c <= 0xDFFF) {
            // unpaired surrogate
            if (!replace) {
              retval = -1;
              break;  // write bytes read so far
            }
            c = 0xFFFD;
          }
          if (c <= 0xFFFF) {
            if (byteIndex + 3 > StreamedStringBufferLength) {
              // Write bytes retrieved so far
              stream.write(bytes,0,byteIndex);
              byteIndex = 0;
            }
            bytes[byteIndex++] = (byte)(0xE0 | ((c >> 12) & 0x0F));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 6) & 0x3F));
            bytes[byteIndex++] = (byte)(0x80 | (c & 0x3F));
          } else {
            if (byteIndex + 4 > StreamedStringBufferLength) {
              // Write bytes retrieved so far
              stream.write(bytes,0,byteIndex);
              byteIndex = 0;
            }
            bytes[byteIndex++] = (byte)(0xF0 | ((c >> 18) & 0x07));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 12) & 0x3F));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 6) & 0x3F));
            bytes[byteIndex++] = (byte)(0x80 | (c & 0x3F));
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
     * an unpaired surrogate code point and &quot; replace&quot; is false.
     * @throws java.lang.NullPointerException "str" is null or "stream"
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static int WriteUtf8(String str, OutputStream stream, boolean replace) throws IOException {
      if (str == null) { throw new NullPointerException("str"); }
      return WriteUtf8(str, 0, str.length(), stream, replace);
    }
    /**
     * Reads a string in UTF-8 encoding from a byte array.
     * @param data A byte array containing a UTF-8 string
     * @param offset Offset into the byte array to start reading
     * @param bytesCount Length, in bytes, of the UTF-8 string
     * @param builder A string builder object where the resulting string
     * will be stored.
     * @param replace If true, replaces invalid encoding with the replacement
     * character (U + FFFD). If false, stops processing when invalid UTF-8
     * is seen.
     * @return 0 if the entire string was read without errors, or -1 if the
     * string is not valid UTF-8 and &quot; replace&quot; is false.
     * @throws java.lang.NullPointerException "data" is null or "builder"
     * is null.
     * @throws java.lang.IllegalArgumentException "offset" is less than 0, "bytesCount"
     * is less than 0, or offset plus bytesCount is greater than the length
     * of "data".
     */
    public static int ReadUtf8FromBytes(
byte[] data, int offset, int bytesCount,
                                        StringBuilder builder,
                                        boolean replace) {
      if (data == null) { throw new NullPointerException("data"); }
      if (offset < 0) { throw new IllegalArgumentException("offset" + " not greater or equal to " + "0" + " ("+offset+")"); }
      if (offset > data.length) { throw new IllegalArgumentException("offset" + " not less or equal to "+data.length+" ("+offset+")"); }
      if (bytesCount < 0) { throw new IllegalArgumentException("bytesCount" + " not greater or equal to " + "0" + " ("+bytesCount+")"); }
      if (bytesCount > data.length) { throw new IllegalArgumentException("bytesCount" + " not less or equal to "+data.length+" ("+bytesCount+")"); }
      if ((data.length - offset) < bytesCount) { throw new IllegalArgumentException("data's length minus " + offset + " not greater or equal to "+bytesCount+" ("+data.length - offset+")"); }
      if (builder == null) { throw new NullPointerException("builder"); }
      int cp = 0;
      int bytesSeen = 0;
      int bytesNeeded = 0;
      int lower = 0x80;
      int upper = 0xBF;
      int pointer = offset;
      int endpointer = offset + bytesCount;
      while (pointer < endpointer) {
        int b = data[pointer] & (int)0xFF;
        pointer++;
        if (bytesNeeded == 0) {
          if ((b & 0x7F) == b) {
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
            if (replace)
              builder.append((char)0xFFFD);
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
            builder.append((char)0xFFFD);
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
            builder.append((char)ret);
          } else {
            int ch = ret - 0x10000;
            int lead = ch / 0x400 + 0xd800;
            int trail = (ch & 0x3FF) + 0xdc00;
            builder.append((char)lead);
            builder.append((char)trail);
          }
        }
      }
      if (bytesNeeded != 0) {
        if (replace)
          builder.append((char)0xFFFD);
        else
          return -1;
      }
      return 0;
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
     * is not valid UTF-8 and &quot; replace&quot; is false (even if the end
     * of the stream is reached), or -2 if the end of the stream was reached
     * before the entire string was read.
     * @throws java.io.IOException An I/O error occurred.
     * @throws java.lang.NullPointerException "stream" is null or "builder"
     * is null.
     */
    public static int ReadUtf8(
InputStream stream, int bytesCount, StringBuilder builder,
                               boolean replace) throws IOException {
      if (stream == null) { throw new NullPointerException("stream"); }
      if (builder == null) { throw new NullPointerException("builder"); }
      int cp = 0;
      int bytesSeen = 0;
      int bytesNeeded = 0;
      int lower = 0x80;
      int upper = 0xBF;
      int pointer = 0;
      while (pointer < bytesCount || bytesCount < 0) {
        int b = stream.read();
        if (b < 0) {
          if (bytesNeeded != 0) {
            bytesNeeded = 0;
            if (replace) {
              builder.append((char)0xFFFD);
              if (bytesCount >= 0)
                return -2;
              break;  // end of stream
            }
            return -1;
          } else {
            if (bytesCount >= 0)
              return -2;
            break;  // end of stream
          }
        }
        if (bytesCount > 0) {
          pointer++;
        }
        if (bytesNeeded == 0) {
          if ((b & 0x7F) == b) {
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
            if (replace)
              builder.append((char)0xFFFD);
            else
              return -1;
          }
          continue;
        } else if (b < lower || b > upper) {
          cp = bytesNeeded = bytesSeen = 0;
          lower = 0x80;
          upper = 0xbf;
          if (replace) {
            builder.append((char)0xFFFD);
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
              builder.append((char)0xFFFD);
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
            builder.append((char)ret);
          } else {
            int ch = ret - 0x10000;
            int lead = ch / 0x400 + 0xd800;
            int trail = (ch & 0x3FF) + 0xdc00;
            builder.append((char)lead);
            builder.append((char)trail);
          }
        }
      }
      if (bytesNeeded != 0) {
        if (replace)
          builder.append((char)0xFFFD);
        else
          return -1;
      }
      return 0;
    }
  }

