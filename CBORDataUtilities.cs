/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;
using System.IO;
using System.Numerics;
using System.Globalization;
namespace PeterO {
    /// <summary> Contains methods useful for reading and writing data,
    /// with a focus on CBOR. </summary>
  public static class CBORDataUtilities {
    private static BigInteger LowestMajorType1 = BigInteger.Zero - (BigInteger.One << 64);
    private static BigInteger UInt64MaxValue = (BigInteger.One << 64) - BigInteger.One;
    /// <summary> Generates a text string from a UTF-8 byte array. </summary>
    /// <param name='bytes'> A byte array containing text encoded in UTF-8.</param>
    /// <param name='replace'> If true, replaces invalid encoding with
    /// the replacement character (U+FFFD). If false, stops processing
    /// when invalid UTF-8 is seen.</param>
    /// <returns> A string represented by the UTF-8 byte array.</returns>
    /// <exception cref='System.ArgumentNullException'> "bytes" is
    /// null.</exception>
    /// <exception cref='System.ArgumentException'> The string is not
    /// valid UTF-8 and "replace" is false</exception>
    [Obsolete("Use DataUtilities.GetUtf8String instead.")]
    public static string GetUtf8String(byte[] bytes, bool replace) {
      return DataUtilities.GetUtf8String(bytes, replace);
    }
    /// <summary> Generates a text string from a portion of a UTF-8 byte array.
    /// </summary>
    /// <param name='bytes'> A byte array containing text encoded in UTF-8.</param>
    /// <param name='offset'> Offset into the byte array to start reading</param>
    /// <param name='byteLength'> Length, in bytes, of the UTF-8 string</param>
    /// <param name='replace'> If true, replaces invalid encoding with
    /// the replacement character (U+FFFD). If false, stops processing
    /// when invalid UTF-8 is seen.</param>
    /// <returns> A string represented by the UTF-8 byte array.</returns>
    /// <exception cref='System.ArgumentNullException'> "bytes" is
    /// null.</exception>
    /// <exception cref='System.ArgumentException'> The portion of the
    /// byte array is not valid UTF-8 and "replace" is false</exception>
    [Obsolete("Use DataUtilities.GetUtf8String instead.")]
    public static string GetUtf8String(byte[] bytes, int offset, int byteLength, bool replace) {
      return DataUtilities.GetUtf8String(bytes, offset, byteLength, replace);
    }
    /// <summary> Encodes a string in UTF-8 as a byte array. </summary>
    /// <param name='str'> A text string.</param>
    /// <param name='replace'> If true, replaces unpaired surrogate code
    /// points with the replacement character (U+FFFD). If false, stops
    /// processing when an unpaired surrogate code point is seen.</param>
    /// <returns> The string encoded in UTF-8.</returns>
    /// <exception cref='System.ArgumentNullException'> "str" is null.</exception>
    /// <exception cref='System.ArgumentException'> The string contains
    /// an unpaired surrogate code point and "replace" is false, or an internal
    /// error occurred.</exception>
    [Obsolete("Use DataUtilities.GetUtf8Bytes instead.")]
    public static byte[] GetUtf8Bytes(string str, bool replace) {
      return DataUtilities.GetUtf8Bytes(str, replace);
    }
    /// <summary> Calculates the number of bytes needed to encode a string
    /// in UTF-8. </summary>
    /// <param name='s'> A Unicode string.</param>
    /// <param name='replace'> If true, treats unpaired surrogate code
    /// points as replacement characters (U+FFFD) instead, meaning each
    /// one takes 3 UTF-8 bytes. If false, stops processing when an unpaired
    /// surrogate code point is reached.</param>
    /// <returns> The number of bytes needed to encode the given string in
    /// UTF-8, or -1 if the string contains an unpaired surrogate code point
    /// and "replace" is false.</returns>
    /// <exception cref='System.ArgumentNullException'> "s" is null.</exception>
    [Obsolete("Use DataUtilities.GetUtf8Length instead.")]
    public static long GetUtf8Length(String s, bool replace) {
      return DataUtilities.GetUtf8Length(s, replace);
    }
    /// <summary> Compares two strings in Unicode code point order. Unpaired
    /// surrogates are treated as individual code points.</summary>
    /// <returns> A value indicating which string is "less" or "greater".
    /// 0: Both strings are equal or null. Less than 0: a is null and b isn't;
    /// or the first code point that's different is less in A than in B; or b starts
    /// with a and is longer than a. Greater than 0: b is null and a isn't; or the
    /// first code point that's different is greater in A than in B; or a starts
    /// with b and is longer than b.</returns>
    /// <param name='strA'> The first string.</param>
    /// <param name='strB'> The second string.</param>
    [Obsolete("Use DataUtilities.CodePointCompare instead.")]
    public static int CodePointCompare(String strA, String strB) {
      return DataUtilities.CodePointCompare(strA, strB);
    }
    /// <summary> Writes a portion of a string in UTF-8 encoding to a data stream.
    /// </summary>
    /// <param name='str'> A string to write.</param>
    /// <param name='offset'> The zero-based index where the string portion
    /// to write begins.</param>
    /// <param name='length'> The length of the string portion to write.</param>
    /// <param name='stream'> A writable data stream.</param>
    /// <param name='replace'> If true, replaces unpaired surrogate code
    /// points with the replacement character (U+FFFD). If false, stops
    /// processing when an unpaired surrogate code point is seen.</param>
    /// <returns> 0 if the entire string portion was written; or -1 if the string
    /// portion contains an unpaired surrogate code point and "replace"
    /// is false.</returns>
    /// <exception cref='System.ArgumentNullException'> "str" is null
    /// or "stream" is null.</exception>
    /// <exception cref='System.ArgumentException'> "offset" is less
    /// than 0, "length" is less than 0, or "offset" plus "length" is greater
    /// than the string's length.</exception>
    /// <exception cref='System.IO.IOException'> An I/O error occurred.</exception>
    [Obsolete("Use DataUtilities.WriteUtf8 instead.")]
    public static int WriteUtf8(String str, int offset, int length, Stream stream, bool replace) {
      return DataUtilities.WriteUtf8(str, offset, length, stream, replace);
    }
    /// <summary> Writes a string in UTF-8 encoding to a data stream. </summary>
    /// <param name='str'> A string to write.</param>
    /// <param name='stream'> A writable data stream.</param>
    /// <param name='replace'> If true, replaces unpaired surrogate code
    /// points with the replacement character (U+FFFD). If false, stops
    /// processing when an unpaired surrogate code point is seen.</param>
    /// <returns> 0 if the entire string was written; or -1 if the string contains
    /// an unpaired surrogate code point and "replace" is false.</returns>
    /// <exception cref='System.ArgumentNullException'> "str" is null
    /// or "stream" is null.</exception>
    /// <exception cref='System.IO.IOException'> An I/O error occurred.</exception>
    [Obsolete("Use DataUtilities.WriteUtf8 instead.")]
    public static int WriteUtf8(String str, Stream stream, bool replace) {
      if ((str) == null) throw new ArgumentNullException("str");
      return DataUtilities.WriteUtf8(str, 0, str.Length, stream, replace);
    }
    /// <summary> Reads a string in UTF-8 encoding from a byte array. </summary>
    /// <param name='data'> A byte array containing a UTF-8 string</param>
    /// <param name='offset'> Offset into the byte array to start reading</param>
    /// <param name='byteLength'> Length, in bytes, of the UTF-8 string</param>
    /// <param name='builder'> A string builder object where the resulting
    /// string will be stored.</param>
    /// <param name='replace'> If true, replaces invalid encoding with
    /// the replacement character (U+FFFD). If false, stops processing
    /// when invalid UTF-8 is seen.</param>
    /// <returns> 0 if the entire string was read without errors, or -1 if the
    /// string is not valid UTF-8 and "replace" is false.</returns>
    /// <exception cref='System.ArgumentNullException'> "data" is null
    /// or "builder" is null.</exception>
    /// <exception cref='System.ArgumentException'> "offset" is less
    /// than 0, "byteLength" is less than 0, or offset plus byteLength is greater
    /// than the length of "data".</exception>
    [Obsolete("Use DataUtilities.ReadUtf8FromBytes instead.")]
    public static int ReadUtf8FromBytes(byte[] data, int offset, int byteLength,
                                        StringBuilder builder,
                                        bool replace) {
      return DataUtilities.ReadUtf8FromBytes(data, offset, byteLength, builder, replace);
    }
    /// <summary> Reads a string in UTF-8 encoding from a data stream. </summary>
    /// <param name='stream'> A readable data stream.</param>
    /// <param name='byteLength'> The length, in bytes, of the string. If
    /// this is less than 0, this function will read until the end of the stream.</param>
    /// <param name='builder'> A string builder object where the resulting
    /// string will be stored.</param>
    /// <param name='replace'> If true, replaces invalid encoding with
    /// the replacement character (U+FFFD). If false, stops processing
    /// when an unpaired surrogate code point is seen.</param>
    /// <returns> 0 if the entire string was read without errors, -1 if the
    /// string is not valid UTF-8 and "replace" is false (even if the end of
    /// the stream is reached), or -2 if the end of the stream was reached before
    /// the entire string was read. </returns>
    /// <exception cref='System.IO.IOException'> An I/O error occurred.</exception>
    /// <exception cref='System.ArgumentNullException'> "stream" is
    /// null or "builder" is null.</exception>
    [Obsolete("Use DataUtilities.ReadUtf8 instead.")]
    public static int ReadUtf8(Stream stream, int byteLength, StringBuilder builder,
                               bool replace) {
      return DataUtilities.ReadUtf8(stream, byteLength, builder, replace);
    }
    /// <summary> Parses a number whose format follows the JSON specification.
    /// See #ParseJSONNumber(str, integersOnly, parseOnly) for more information.
    /// </summary>
    /// <param name='str'> A string to parse.</param>
    /// <returns> A CBOR object that represents the parsed number. This function
    /// will return a CBOR object representing positive or negative infinity
    /// if the exponent is greater than 2^64-1 (unless the value is 0), and
    /// will return zero if the exponent is less than -(2^64).</returns>
    public static CBORObject ParseJSONNumber(string str) {
      return ParseJSONNumber(str, false, false, false);
    }
    /// <summary> Parses a number whose format follows the JSON specification
    /// (RFC 4627). Roughly speaking, a valid number consists of an optional
    /// minus sign, one or more digits (starting with 1 to 9 unless the only
    /// digit is 0), an optional decimal point with one or more digits, and
    /// an optional letter E or e with one or more digits (the exponent). </summary>
    /// <param name='str'> A string to parse.</param>
    /// <param name='integersOnly'> If true, no decimal points or exponents
    /// are allowed in the string.</param>
    /// <param name='positiveOnly'> If true, only positive numbers are
    /// allowed (the leading minus is disallowed).</param>
    /// <param name='failOnExponentOverflow'> If true, this function
    /// will return null if the exponent is less than -(2^64) or greater than
    /// 2^64-1 (unless the value is 0). If false, this function will return
    /// a CBOR object representing positive or negative infinity if the exponent
    /// is greater than 2^64-1 (unless the value is 0), and will return zero
    /// if the exponent is less than -(2^64).</param>
    /// <returns> A CBOR object that represents the parsed number.</returns>
    public static CBORObject ParseJSONNumber(string str,
                                             bool integersOnly,
                                             bool positiveOnly,
                                             bool failOnExponentOverflow
                                            ) {
      if (String.IsNullOrEmpty(str))
        return null;
      char c = str[0];
      bool negative = false;
      int index = 0;
      if (index >= str.Length)
        return null;
      c = str[index];
      if (c == '-' && !positiveOnly) {
        negative = true;
        index++;
      }
      int numberStart = index;
      if (index >= str.Length)
        return null;
      c = str[index];
      index++;
      int numberEnd = index;
      int fracStart = -1;
      int fracEnd = -1;
      bool negExp = false;
      int expStart = -1;
      int expEnd = -1;
      FastInteger smallNumber = new FastInteger();
      FastInteger exponentAdjust = new FastInteger();
      FastInteger smallExponent = new FastInteger();
      if (c >= '1' && c <= '9') {
        smallNumber.Add((int)(c - '0'));
        while (index < str.Length) {
          c = str[index];
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
        if (index < str.Length && str[index] == '.') {
          // Fraction
          index++;
          fracStart = index;
          if (index >= str.Length)
            return null;
          c = str[index];
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
            while (index < str.Length) {
              c = str[index];
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
        if (index < str.Length && (str[index] == 'e' || str[index] == 'E')) {
          // Exponent
          index++;
          if (index >= str.Length)
            return null;
          c = str[index];
          if (c == '-') {
            negExp = true;
            index++;
          }
          if (c == '+') index++;
          expStart = index;
          if (index >= str.Length)
            return null;
          c = str[index];
          index++;
          expEnd = index;
          if (c >= '0' && c <= '9') {
            if (smallExponent.CanFitInInt64()) {
              smallExponent.Add((int)(c - '0'));
            }
            while (index < str.Length) {
              c = str[index];
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
      if (index != str.Length) {
        // End of the string wasn't reached, so isn't a number
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
        string strsub = (numberStart == 0 && numberEnd == str.Length) ? str :
          str.Substring(numberStart, numberEnd - numberStart);
        BigInteger bigintValue = BigInteger.Parse(strsub,
                                                  NumberStyles.None,
                                                  CultureInfo.InvariantCulture);
        if (negative) bigintValue = -(BigInteger)bigintValue;
        return CBORObject.FromObject(bigintValue);
      } else {
        // Intval consists of the whole and fractional part
        string intvalString = str.Substring(numberStart, numberEnd - numberStart) +
          ((fracStart < 0) ? String.Empty : str.Substring(fracStart, fracEnd - fracStart));
        BigInteger intval = BigInteger.Parse(
          intvalString,
          NumberStyles.None,
          CultureInfo.InvariantCulture);
        if (negative) intval = -intval;
        if ((fracStart < 0) && expStart < 0) {
          // No fractional part and no exponent;
          // this is easy, just return the integer
          return CBORObject.FromObject(intval);
        }
        if (intval.IsZero) {
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
          exp = new FastInteger(BigInteger.Parse(
            str.Substring(expStart, expEnd - expStart),
            NumberStyles.None,
            CultureInfo.InvariantCulture));
          if (negExp) exp.Negate();
          if (fracStart >= 0) {
            // If there is a fractional part,
            // decrease the exponent by that part's length
            exp.Subtract(fracEnd - fracStart);
          }
        }
        if (exp.Sign == 0) {
          // If exponent is 0, this is also easy,
          // just return the integer
          return CBORObject.FromObject(intval);
        } else if (!exp.CanFitInInt64()) {
          if (exp.AsBigInteger().CompareTo(UInt64MaxValue) > 0) {
            // Exponent is higher than the highest representable
            // integer of major type 0
            if (failOnExponentOverflow)
              return null;
            else
              return (exp.Sign < 0) ?
                CBORObject.FromObject(Double.NegativeInfinity) :
                CBORObject.FromObject(Double.PositiveInfinity);
          }
          if (exp.AsBigInteger().CompareTo(LowestMajorType1) < 0) {
            // Exponent is lower than the lowest representable
            // integer of major type 1
            if (failOnExponentOverflow)
              return null;
            else
              return CBORObject.FromObject(0);
          }
        }
        // Represent the CBOR object as a decimal fraction
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
}