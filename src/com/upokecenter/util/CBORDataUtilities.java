package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */



import java.io.*;
import java.math.*;



  /**
   * Contains methods useful for reading and writing strings and parsing
   * numbers.
   */
  public final class CBORDataUtilities {
private CBORDataUtilities(){}
    private static BigInteger LowestMajorType1 = BigInteger.ZERO .subtract(BigInteger.ONE.shiftLeft(64));
    private static BigInteger UInt64MaxValue = (BigInteger.ONE.shiftLeft(64)).subtract(BigInteger.ONE);

    private static int StreamedStringBufferLength = 4096;

    /**
     * Generates a text string from a UTF-8 byte array.
     * @param bytes A byte array containing text encoded in UTF-8.
     * @param replace If true, replaces invalid encoding with the replacement
     * character (U+FFFD). If false, stops processing when invalid UTF-8
     * is seen.
     * @return A string represented by the UTF-8 byte array.
     * @throws java.lang.NullPointerException "bytes" is null.
     * @throws java.lang.IllegalArgumentException The string is not valid UTF-8
     * and "replace" is false
     */
    public static String GetUtf8String(byte[] bytes, boolean replace) {
      StringBuilder b = new StringBuilder();
      if (ReadUtf8FromBytes(bytes, 0, bytes.length, b, replace) != 0)
        throw new IllegalArgumentException("Invalid UTF-8");
      return b.toString();
    }

    /**
     * Generates a text string from a portion of a UTF-8 byte array.
     * @param bytes A byte array containing text encoded in UTF-8.
     * @param offset Offset into the byte array to start reading
     * @param byteLength Length, in bytes, of the UTF-8 string
     * @param replace If true, replaces invalid encoding with the replacement
     * character (U+FFFD). If false, stops processing when invalid UTF-8
     * is seen.
     * @return A string represented by the UTF-8 byte array.
     * @throws java.lang.NullPointerException "bytes" is null.
     * @throws java.lang.IllegalArgumentException The portion of the byte array
     * is not valid UTF-8 and "replace" is false
     */
    public static String GetUtf8String(byte[] bytes, int offset, int byteLength, boolean replace) {
      StringBuilder b = new StringBuilder();
      if (ReadUtf8FromBytes(bytes, offset, byteLength, b, replace) != 0)
        throw new IllegalArgumentException("Invalid UTF-8");
      return b.toString();
    }

    /**
     * Encodes a string in UTF-8 as a byte array.
     * @param str A text string.
     * @param replace If true, replaces unpaired surrogate code points
     * with the replacement character (U+FFFD). If false, stops processing
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

          if (WriteUtf8(str, ms, replace) != 0)
            throw new IllegalArgumentException("Unpaired surrogate code point");
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
     * @param s A Unicode string.
     * @param replace If true, treats unpaired surrogate code points as
     * replacement characters (U+FFFD) instead, meaning each one takes
     * 3 UTF-8 bytes. If false, stops processing when an unpaired surrogate
     * code point is reached.
     * @return The number of bytes needed to encode the given string in UTF-8,
     * or -1 if the string contains an unpaired surrogate code point and "replace"
     * is false.
     * @throws java.lang.NullPointerException "s" is null.
     */
    public static long GetUtf8Length(String s, boolean replace) {
      if (s == null) throw new NullPointerException();
      long size = 0;
      for (int i = 0; i < s.length(); i++) {
        int c = s.charAt(i);
        if (c <= 0x7F) {
          size++;
        } else if (c <= 0x7FF) {
          size += 2;
        } else if (c <= 0xD7FF || c >= 0xE000) {
          size += 3;
        } else if (c <= 0xDBFF) { // UTF-16 leading surrogate
          i++;
          if (i >= s.length() || s.charAt(i) < 0xDC00 || s.charAt(i) > 0xDFFF) {
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
     * Compares two strings in Unicode code point order. Unpaired surrogates
     * are treated as individual code points.
     * @param a The first string.
     * @param b The second string.
     * @return A value indicating which string is "less" or "greater". 0:
     * Both strings are equal or null. Less than 0: a is null and b isn't; or
     * the first code point that's different is less in A than in B; or b starts
     * with a and is longer than a. Greater than 0: b is null and a isn't; or the
     * first code point that's different is greater in A than in B; or a starts
     * with b and is longer than b.
     */
    public static int CodePointCompare(String strA, String strB) {
      if (strA == null) return (strB == null) ? 0 : -1;
      if (strB == null) return 1;
      int len = Math.min(strA.length(), strB.length());
      for (int i = 0; i < len; i++) {
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
          if (ca != cb) return ca - cb;
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
      if (strA.length() == strB.length()) return 0;
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
     * with the replacement character (U+FFFD). If false, stops processing
     * when an unpaired surrogate code point is seen.
     * @return 0 if the entire string portion was written; or -1 if the string
     * portion contains an unpaired surrogate code point and "replace"
     * is false.
     * @throws java.lang.NullPointerException "str" is null or "stream"
     * is null.
     * @throws java.lang.IllegalArgumentException "offset" is less than 0, "length"
     * is less than 0, or "offset" plus "length" is greater than the string's
     * length.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static int WriteUtf8(String str, int offset, int length, OutputStream stream, boolean replace) throws IOException {
      if ((stream) == null) throw new NullPointerException("stream");
      if((str)==null)throw new NullPointerException("str");
if((offset)<0)throw new IllegalArgumentException("offset"+" not greater or equal to "+Long.toString((long)(0))+" ("+Long.toString((long)(offset))+")");
if((offset)>str.length())throw new IllegalArgumentException("offset"+" not less or equal to "+Long.toString((long)(str.length()))+" ("+Long.toString((long)(offset))+")");
if((length)<0)throw new IllegalArgumentException("length"+" not greater or equal to "+Long.toString((long)(0))+" ("+Long.toString((long)(length))+")");
if((length)>str.length())throw new IllegalArgumentException("length"+" not less or equal to "+Long.toString((long)(str.length()))+" ("+Long.toString((long)(length))+")");
if(((str.length()-offset))<length)throw new IllegalArgumentException("str's length minus "+offset+" not greater or equal to "+Long.toString((long)(length))+" ("+Long.toString((long)((str.length()-offset)))+")");
      byte[] bytes;
      int retval = 0;
      bytes = new byte[StreamedStringBufferLength];
      int byteIndex = 0;
      int endIndex=offset+length;
      for (int index = offset; index < endIndex; index++) {
        int c = str.charAt(index);
        if (c <= 0x7F) {
          if (byteIndex + 1 > StreamedStringBufferLength) {
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
          bytes[byteIndex++] = ((byte)(0xC0 | ((c >> 6) & 0x1F)));
          bytes[byteIndex++] = ((byte)(0x80 | (c & 0x3F)));
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
              break; // write bytes read so far
            }
            c = 0xFFFD;
          }
          if (c <= 0xFFFF) {
            if (byteIndex + 3 > StreamedStringBufferLength) {
              // Write bytes retrieved so far
              stream.write(bytes,0,byteIndex);
              byteIndex = 0;
            }
            bytes[byteIndex++] = ((byte)(0xE0 | ((c >> 12) & 0x0F)));
            bytes[byteIndex++] = ((byte)(0x80 | ((c >> 6) & 0x3F)));
            bytes[byteIndex++] = ((byte)(0x80 | (c & 0x3F)));
          } else {
            if (byteIndex + 4 > StreamedStringBufferLength) {
              // Write bytes retrieved so far
              stream.write(bytes,0,byteIndex);
              byteIndex = 0;
            }
            bytes[byteIndex++] = ((byte)(0xF0 | ((c >> 18) & 0x07)));
            bytes[byteIndex++] = ((byte)(0x80 | ((c >> 12) & 0x3F)));
            bytes[byteIndex++] = ((byte)(0x80 | ((c >> 6) & 0x3F)));
            bytes[byteIndex++] = ((byte)(0x80 | (c & 0x3F)));
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
     * with the replacement character (U+FFFD). If false, stops processing
     * when an unpaired surrogate code point is seen.
     * @return 0 if the entire string was written; or -1 if the string contains
     * an unpaired surrogate code point and "replace" is false.
     * @throws java.lang.NullPointerException "str" is null or "stream"
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static int WriteUtf8(String str, OutputStream stream, boolean replace) throws IOException {
      if((str)==null)throw new NullPointerException("str");
      return WriteUtf8(str,0,str.length(),stream,replace);
    }

    /**
     * Reads a string in UTF-8 encoding from a byte array.
     * @param data A byte array containing a UTF-8 string
     * @param offset Offset into the byte array to start reading
     * @param byteLength Length, in bytes, of the UTF-8 string
     * @param builder A string builder object where the resulting string
     * will be stored.
     * @param replace If true, replaces invalid encoding with the replacement
     * character (U+FFFD). If false, stops processing when invalid UTF-8
     * is seen.
     * @return 0 if the entire string was read without errors, or -1 if the
     * string is not valid UTF-8 and "replace" is false.
     * @throws java.lang.NullPointerException "data" is null or "builder"
     * is null.
     * @throws java.lang.IllegalArgumentException "offset" is less than 0, "byteLength"
     * is less than 0, or offset plus byteLength is greater than the length
     * of "data".
     */
    public static int ReadUtf8FromBytes(byte[] data, int offset, int byteLength,
                                        StringBuilder builder,
                                        boolean replace) {
      if((data)==null)throw new NullPointerException("data");
if((offset)<0)throw new IllegalArgumentException("offset"+" not greater or equal to "+Long.toString((long)(0))+" ("+Long.toString((long)(offset))+")");
if((offset)>data.length)throw new IllegalArgumentException("offset"+" not less or equal to "+Long.toString((long)(data.length))+" ("+Long.toString((long)(offset))+")");
if((byteLength)<0)throw new IllegalArgumentException("byteLength"+" not greater or equal to "+Long.toString((long)(0))+" ("+Long.toString((long)(byteLength))+")");
if((byteLength)>data.length)throw new IllegalArgumentException("byteLength"+" not less or equal to "+Long.toString((long)(data.length))+" ("+Long.toString((long)(byteLength))+")");
if(((data.length-offset))<byteLength)throw new IllegalArgumentException("data's length minus "+offset+" not greater or equal to "+Long.toString((long)(byteLength))+" ("+Long.toString((long)((data.length-offset)))+")");
      if ((builder) == null) throw new NullPointerException("builder");
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
            if (replace)
              builder.append((char)0xFFFD);
            else
              return -1;
          }
          continue;
        }
        if (b < lower || b > upper) {
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
        }
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
     * @param byteLength The length, in bytes, of the string. If this is less
     * than 0, this function will read until the end of the stream.
     * @param builder A string builder object where the resulting string
     * will be stored.
     * @param replace If true, replaces invalid encoding with the replacement
     * character (U+FFFD). If false, stops processing when an unpaired
     * surrogate code point is seen.
     * @return 0 if the entire string was read without errors, -1 if the string
     * is not valid UTF-8 and "replace" is false (even if the end of the stream
     * is reached), or -2 if the end of the stream was reached before the entire
     * string was read.
     * @throws java.io.IOException An I/O error occurred.
     * @throws java.lang.NullPointerException "stream" is null or "builder"
     * is null.
     */
    public static int ReadUtf8(InputStream stream, int byteLength, StringBuilder builder,
                               boolean replace) throws IOException {
      if ((stream) == null) throw new NullPointerException("stream");
      if ((builder) == null) throw new NullPointerException("builder");
      int cp = 0;
      int bytesSeen = 0;
      int bytesNeeded = 0;
      int lower = 0x80;
      int upper = 0xBF;
      int pointer = 0;
      while (pointer < byteLength || byteLength < 0) {
        int b = stream.read();
        if (b < 0) {
          if (bytesNeeded != 0) {
            bytesNeeded = 0;
            if (replace) {
              builder.append((char)0xFFFD);
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
            if (replace)
              builder.append((char)0xFFFD);
            else
              return -1;
          }
          continue;
        }
        if (b < lower || b > upper) {
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
        }
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
      if (bytesNeeded != 0) {
        if (replace)
          builder.append((char)0xFFFD);
        else
          return -1;
      }
      return 0;
    }

    /**
     * Parses a number whose format follows the JSON specification. See
     * #ParseJSONNumber(str, integersOnly, parseOnly) for more information.
     * @param str A string to parse.
     * @return A CBOR object that represents the parsed number, or null if
     * the exponent is less than -(2^64) or greater than 2^64-1 or if the entire
     * string does not represent a valid number.
     */
    public static CBORObject ParseJSONNumber(String str) {
      return ParseJSONNumber(str, false, false);
    }

    /**
     * Parses a number whose format follows the JSON specification (RFC
     * 4627). Roughly speaking, a valid number consists of an optional minus
     * sign, one or more digits (starting with 1 to 9 unless the only digit
     * is 0), an optional decimal point with one or more digits, and an optional
     * letter E or e with one or more digits (the exponent).
     * @param str A string to parse.
     * @param integersOnly If true, no decimal points or exponents are allowed
     * in the string.
     * @param positiveOnly If true, only positive numbers are allowed (the
     * leading minus is disallowed).
     * @return A CBOR object that represents the parsed number, or null if
     * the exponent is less than -(2^64) or greater than 2^64-1 or if the entire
     * string does not represent a valid number.
     */
    public static CBORObject ParseJSONNumber(String str,
                                             boolean integersOnly,
                                             boolean positiveOnly) {
      if (((str)==null || (str).length()==0))
        return null;
      char c = str.charAt(0);
      boolean negative = false;
      int index = 0;
      if (index >= str.length())
        return null;
      c = str.charAt(index);
      if (c == '-' && !positiveOnly) {
        negative = true;
        index++;
      }
      int numberStart = index;
      if (index >= str.length())
        return null;
      c = str.charAt(index);
      index++;
      int numberEnd = index;
      int fracStart = -1;
      int fracEnd = -1;
      boolean negExp = false;
      int expStart = -1;
      int expEnd = -1;
      int smallNumber = 0; // for small numbers (9 digits or less)
      int smallNumberCount = 0;
      if (c >= '1' && c <= '9') {
        smallNumber = (int)(c - '0');
        smallNumberCount++;
        while (index < str.length()) {
          c = str.charAt(index);
          if (c >= '0' && c <= '9') {
            index++;
            numberEnd = index;
            if (smallNumberCount < 9) {
              smallNumber *= 10;
              smallNumber += (int)(c - '0');
              smallNumberCount++;
            }
          } else {
            break;
          }
        }
      } else if (c != '0') {
        return null;
      }
      if (!integersOnly) {
        if (index < str.length() && str.charAt(index) == '.') {
          // Fraction
          index++;
          fracStart = index;
          if (index >= str.length())
            return null;
          c = str.charAt(index);
          index++;
          fracEnd = index;
          if (c >= '0' && c <= '9') {
            while (index < str.length()) {
              c = str.charAt(index);
              if (c >= '0' && c <= '9') {
                index++;
                fracEnd = index;
              } else {
                break;
              }
            }
          } else {
            // Not a fraction
            return null;
          }
        }
        if (index < str.length() && (str.charAt(index) == 'e' || str.charAt(index) == 'E')) {
          // Exponent
          index++;
          if (index >= str.length())
            return null;
          c = str.charAt(index);
          if (c == '-') {
            negExp = true;
            index++;
          }
          if (c == '+') index++;
          expStart = index;
          if (index >= str.length())
            return null;
          c = str.charAt(index);
          index++;
          expEnd = index;
          if (c >= '0' && c <= '9') {
            while (index < str.length()) {
              c = str.charAt(index);
              if (c >= '0' && c <= '9') {
                index++;
                expEnd = index;
              } else {
                break;
              }
            }
          } else {
            // Not an exponent
            return null;
          }
        }
      }
      if (index != str.length()) {
        // End of the String wasn't reached, so isn't a number
        return null;
      }
      if (fracStart < 0 && expStart < 0 && (numberEnd - numberStart) <= 9) {
        // Common case: small integer
        int value = smallNumber;
        if (negative) value = -value;
        return CBORObject.FromObject(value);
      } if (fracStart < 0 && expStart < 0 && (numberEnd - numberStart) <= 18) {
        // Common case: long-sized integer
        String strsub = (numberStart == 0 && numberEnd == str.length()) ? str :
          str.substring(numberStart,(numberStart)+(numberEnd - numberStart));
        long value = Long.parseLong(strsub);
        if (negative) value = -value;
        return CBORObject.FromObject(value);
      } else if (fracStart >= 0 && expStart < 0 &&
                (numberEnd - numberStart) + (fracEnd - fracStart) <= 9) {
        // Small whole part and small fractional part
        int int32fracpart = (fracStart < 0) ? 0 : Integer.parseInt(
          str.substring(fracStart,(fracStart)+(fracEnd - fracStart)));
        // Intval consists of the whole and fractional part
        String intvalString = str.substring(numberStart,(numberStart)+(numberEnd - numberStart)) +
          (int32fracpart == 0 ? "" : str.substring(fracStart,(fracStart)+(fracEnd - fracStart)));
        int int32val = Integer.parseInt(
          intvalString);
        if (negative) int32val = -int32val;
        int int32exp = 0;
        if (int32fracpart != 0) {
          // If there is a nonzero fractional part,
          // decrease the exponent by that part's length
          int32exp -= (int)(fracEnd - fracStart);
        }
        if (int32exp == 0 || int32val == 0) {
          // If exponent is 0, or mantissa is 0,
          // just return the integer
          return CBORObject.FromObject(int32val);
        }
        // Represent the CBOR Object as a decimal fraction
        return CBORObject.FromObjectAndTag(new CBORObject[]{
                                             CBORObject.FromObject(int32exp),CBORObject.FromObject(int32val)}, 4);
      } else if (fracStart < 0 && expStart < 0) {
        // Bigger integer
        String strsub = (numberStart == 0 && numberEnd == str.length()) ? str :
          str.substring(numberStart,(numberStart)+(numberEnd - numberStart));
        if (str.length() == 19 && strsub.charAt(0) >= '0' && strsub.charAt(0) < '9') {
          // Can fit in a 64-bit long (cases with 18 digits
          // or less are already handled above)
          long value = Long.parseLong(strsub);
          if (negative) value = -value;
          return CBORObject.FromObject(value);
        }
        BigInteger bigintValue = new BigInteger(strsub);
        if (negative) bigintValue=(bigintValue).negate();
        return CBORObject.FromObject(bigintValue);
      } else {
        BigInteger fracpart = (fracStart < 0) ? BigInteger.ZERO : new BigInteger(
          str.substring(fracStart,(fracStart)+(fracEnd - fracStart)));
        // Intval consists of the whole and fractional part
        String intvalString = str.substring(numberStart,(numberStart)+(numberEnd - numberStart)) +
          (fracpart.signum()==0 ? "" : str.substring(fracStart,(fracStart)+(fracEnd - fracStart)));
        BigInteger intval = new BigInteger(
          intvalString);
        if (negative) intval=intval.negate();
        if (fracpart.signum()==0 && expStart < 0) {
          // Zero fractional part and no exponent;
          // this is easy, just return the integer
          return CBORObject.FromObject(intval);
        }
        if (intval.signum()==0) {
          // Mantissa is 0, return 0 regardless of exponent
          return CBORObject.FromObject(0);
        }
        BigInteger exp = (expStart < 0) ? BigInteger.ZERO : new BigInteger(
          str.substring(expStart,(expStart)+(expEnd - expStart)));
        if (negExp) exp=exp.negate();
        if (fracpart.signum()!=0) {
          // If there is a nonzero fractional part,
          // decrease the exponent by that part's length
          exp=exp.subtract(BigInteger.valueOf(fracEnd - fracStart));
        }
        if (exp.signum()==0) {
          // If exponent is 0, this is also easy,
          // just return the integer
          return CBORObject.FromObject(intval);
        } else if (exp.compareTo(UInt64MaxValue) > 0 ||
                  exp.compareTo(LowestMajorType1) < 0) {
          // Exponent is lower than the lowest representable
          // integer of major type 1, or higher than the
          // highest representable integer of major type 0
          return null;
        }
        // Represent the CBOR Object as a decimal fraction
        return CBORObject.FromObjectAndTag(new CBORObject[]{
                                             CBORObject.FromObject(exp),
                                             CBORObject.FromObject(intval)}, 4);
      }
    }
  }
