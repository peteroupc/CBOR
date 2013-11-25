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
     * Contains methods useful for reading and writing data, with a focus
     * on CBOR.
     */
  public final class CBORDataUtilities {
private CBORDataUtilities(){}
    private static BigInteger LowestMajorType1 = BigInteger.ZERO .subtract(BigInteger.ONE.shiftLeft(64));
    private static BigInteger UInt64MaxValue = (BigInteger.ONE.shiftLeft(64)).subtract(BigInteger.ONE);
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
     * @deprecated Use DataUtilities.GetUtf8String instead. 
 */
@Deprecated
    public static String GetUtf8String(byte[] bytes, boolean replace) {
      return DataUtilities.GetUtf8String(bytes, replace);
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
     * @deprecated Use DataUtilities.GetUtf8String instead. 
 */
@Deprecated
    public static String GetUtf8String(byte[] bytes, int offset, int byteLength, boolean replace) {
      return DataUtilities.GetUtf8String(bytes, offset, byteLength, replace);
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
     * @deprecated Use DataUtilities.GetUtf8Bytes instead. 
 */
@Deprecated
    public static byte[] GetUtf8Bytes(String str, boolean replace) {
      return DataUtilities.GetUtf8Bytes(str, replace);
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
     * @deprecated Use DataUtilities.GetUtf8Length instead. 
 */
@Deprecated
    public static long GetUtf8Length(String s, boolean replace) {
      return DataUtilities.GetUtf8Length(s, replace);
    }
    /**
     * Compares two strings in Unicode code point order. Unpaired surrogates
     * are treated as individual code points.
     * @param strA The first string.
     * @param strB The second string.
     * @return A value indicating which string is "less" or "greater". 0:
     * Both strings are equal or null. Less than 0: a is null and b isn't; or
     * the first code point that's different is less in A than in B; or b starts
     * with a and is longer than a. Greater than 0: b is null and a isn't; or the
     * first code point that's different is greater in A than in B; or a starts
     * with b and is longer than b.
     * @deprecated Use DataUtilities.CodePointCompare instead. 
 */
@Deprecated
    public static int CodePointCompare(String strA, String strB) {
      return DataUtilities.CodePointCompare(strA, strB);
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
     * @deprecated Use DataUtilities.WriteUtf8 instead. 
 */
@Deprecated
    public static int WriteUtf8(String str, int offset, int length, OutputStream stream, boolean replace) throws IOException {
      return DataUtilities.WriteUtf8(str, offset, length, stream, replace);
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
     * @deprecated Use DataUtilities.WriteUtf8 instead. 
 */
@Deprecated
    public static int WriteUtf8(String str, OutputStream stream, boolean replace) throws IOException {
      if ((str) == null) throw new NullPointerException("str");
      return DataUtilities.WriteUtf8(str, 0, str.length(), stream, replace);
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
     * @deprecated Use DataUtilities.ReadUtf8FromBytes instead. 
 */
@Deprecated
    public static int ReadUtf8FromBytes(byte[] data, int offset, int byteLength,
                                        StringBuilder builder,
                                        boolean replace) {
      return DataUtilities.ReadUtf8FromBytes(data, offset, byteLength, builder, replace);
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
     * @deprecated Use DataUtilities.ReadUtf8 instead. 
 */
@Deprecated
    public static int ReadUtf8(InputStream stream, int byteLength, StringBuilder builder,
                               boolean replace) throws IOException {
      return DataUtilities.ReadUtf8(stream, byteLength, builder, replace);
    }
    /**
     * Parses a number whose format follows the JSON specification. See
     * #ParseJSONNumber(str, integersOnly, parseOnly) for more information.
     * @param str A string to parse.
     * @return A CBOR object that represents the parsed number. This function
     * will return a CBOR object representing positive or negative infinity
     * if the exponent is greater than 2^64-1 (unless the value is 0), and
     * will return zero if the exponent is less than -(2^64).
     */
    public static CBORObject ParseJSONNumber(String str) {
      return ParseJSONNumber(str, false, false, false);
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
     * @param failOnExponentOverflow If true, this function will return
     * null if the exponent is less than -(2^64) or greater than 2^64-1 (unless
     * the value is 0). If false, this function will return a CBOR object representing
     * positive or negative infinity if the exponent is greater than 2^64-1
     * (unless the value is 0), and will return zero if the exponent is less
     * than -(2^64).
     * @return A CBOR object that represents the parsed number.
     */
    public static CBORObject ParseJSONNumber(String str,
                                             boolean integersOnly,
                                             boolean positiveOnly,
                                             boolean failOnExponentOverflow
                                            ) {
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
      FastInteger smallNumber = new FastInteger();
      FastInteger exponentAdjust = new FastInteger();
      FastInteger smallExponent = new FastInteger();
      if (c >= '1' && c <= '9') {
        smallNumber.Add((int)(c - '0'));
        while (index < str.length()) {
          c = str.charAt(index);
          if (c >= '0' && c <= '9') {
            index++;
            numberEnd = index;
            if (smallNumber.CanFitInInt64()) {
              smallNumber.Multiply(10);
              smallNumber.Add((int)(c - '0'));
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
            // Adjust the exponent for this
            // fractional digit
            exponentAdjust.Add(-1);
            if (smallNumber.CanFitInInt64()) {
              smallNumber.Multiply(10);
              smallNumber.Add((int)(c - '0'));
            }
            while (index < str.length()) {
              c = str.charAt(index);
              if (c >= '0' && c <= '9') {
                index++;
                fracEnd = index;
                // Adjust the exponent for this
                // fractional digit
                exponentAdjust.Add(-1);
                if (smallNumber.CanFitInInt64()) {
                  smallNumber.Multiply(10);
                  smallNumber.Add((int)(c - '0'));
                }
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
            if (smallExponent.CanFitInInt64()) {
              smallExponent.Add((int)(c - '0'));
            }
            while (index < str.length()) {
              c = str.charAt(index);
              if (c >= '0' && c <= '9') {
                index++;
                expEnd = index;
                if (smallExponent.CanFitInInt64()) {
                  smallExponent.Multiply(10);
                  smallExponent.Add((int)(c - '0'));
                }
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
      if (negExp && smallExponent.CanFitInInt64())
        smallExponent.Negate();
      if (negative && smallNumber.CanFitInInt64())
        smallNumber.Negate();
      if (smallExponent.CanFitInInt64())
        smallExponent.Add(exponentAdjust);
      if (index != str.length()) {
        // End of the String wasn't reached, so isn't a number
        return null;
      }
      if (smallNumber.CanFitInInt64() && smallExponent.CanFitInInt64()) {
        // Small whole/fractional part and small exponent
        long value = smallNumber.AsInt64();
        long exponent = smallExponent.AsInt64();
        if (exponent == 0) {
          return CBORObject.FromObject(value);
        }
        return CBORObject.FromObject(new DecimalFraction(value, exponent));
      } else if (fracStart < 0 && expStart < 0) {
        // Bigger integer
        String strsub = (numberStart == 0 && numberEnd == str.length()) ? str :
          str.substring(numberStart,(numberStart)+(numberEnd - numberStart));
        BigInteger bigintValue = new BigInteger(strsub);
        if (negative) bigintValue=(bigintValue).negate();
        return CBORObject.FromObject(bigintValue);
      } else {
        // Intval consists of the whole and fractional part
        String intvalString = str.substring(numberStart,(numberStart)+(numberEnd - numberStart)) +
          ((fracStart < 0) ? "" : str.substring(fracStart,(fracStart)+(fracEnd - fracStart)));
        BigInteger intval = new BigInteger(intvalString);
        if (negative) intval=intval.negate();
        if ((fracStart < 0) && expStart < 0) {
          // No fractional part and no exponent;
          // this is easy, just return the integer
          return CBORObject.FromObject(intval);
        }
        if (intval.signum()==0) {
          // Mantissa is 0, return 0 regardless of exponent
          return CBORObject.FromObject(0);
        }
        FastInteger exp = null;
        if (expStart < 0) {
          // Exponent zero
          exp = new FastInteger();
          if (fracStart >= 0) {
            // If there is a fractional part,
            // decrease the exponent by that part's length
            exp.Subtract(fracEnd - fracStart);
          }
        } else if (smallExponent.CanFitInInt64()) {
          // Use already parsed exponent
          exp = smallExponent;
        } else {
          exp = new FastInteger(new BigInteger(str.substring(expStart,(expStart)+(expEnd - expStart))));
          if (negExp) exp.Negate();
          if (fracStart >= 0) {
            // If there is a fractional part,
            // decrease the exponent by that part's length
            exp.Subtract(fracEnd - fracStart);
          }
        }
        if (exp.signum() == 0) {
          // If exponent is 0, this is also easy,
          // just return the integer
          return CBORObject.FromObject(intval);
        } else if (!exp.CanFitInInt64()) {
          if (exp.AsBigInteger().compareTo(UInt64MaxValue) > 0) {
            // Exponent is higher than the highest representable
            // integer of major type 0
            if (failOnExponentOverflow)
              return null;
            else
              return (exp.signum() < 0) ?
                CBORObject.FromObject(Double.NEGATIVE_INFINITY) :
                CBORObject.FromObject(Double.POSITIVE_INFINITY);
          }
          if (exp.AsBigInteger().compareTo(LowestMajorType1) < 0) {
            // Exponent is lower than the lowest representable
            // integer of major type 1
            if (failOnExponentOverflow)
              return null;
            else
              return CBORObject.FromObject(0);
          }
        }
        // Represent the CBOR Object as a decimal fraction
        if (exp.CanFitInInt64()) {
          return CBORObject.FromObjectAndTag(new CBORObject[]{
                                               CBORObject.FromObject(exp.AsInt64()),
                                               CBORObject.FromObject(intval)}, 4);
        } else {
          return CBORObject.FromObjectAndTag(new CBORObject[]{
                                               CBORObject.FromObject(exp.AsBigInteger()),
                                               CBORObject.FromObject(intval)}, 4);
        }
      }
    }
  }
