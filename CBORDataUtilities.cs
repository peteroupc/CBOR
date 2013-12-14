/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;
//using System.Numerics;
using System.Globalization;
namespace PeterO {
    /// <summary> Contains methods useful for reading and writing data,
    /// with a focus on CBOR. </summary>
  public static class CBORDataUtilities {
    private static BigInteger LowestMajorType1 = BigInteger.Zero - (BigInteger.One << 64);
    private static BigInteger UInt64MaxValue = (BigInteger.One << 64) - BigInteger.One;
    

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
      if (index >= str.Length)
        return null;
      c = str[index];
      index++;
      bool negExp = false;
      FastInteger fastNumber = new FastInteger();
      FastInteger exponentAdjust = new FastInteger();
      FastInteger fastExponent = new FastInteger();
      if (c >= '1' && c <= '9') {
        fastNumber.Add((int)(c - '0'));
        while (index < str.Length) {
          c = str[index];
          if (c >= '0' && c <= '9') {
            index++;
            fastNumber.Multiply(10);
            fastNumber.Add((int)(c - '0'));
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
          if (index >= str.Length)
            return null;
          c = str[index];
          index++;
          if (c >= '0' && c <= '9') {
            // Adjust the exponent for this
            // fractional digit
            exponentAdjust.Add(-1);
            fastNumber.Multiply(10);
            fastNumber.Add((int)(c - '0'));
            while (index < str.Length) {
              c = str[index];
              if (c >= '0' && c <= '9') {
                index++;
                // Adjust the exponent for this
                // fractional digit
                exponentAdjust.Add(-1);
                fastNumber.Multiply(10);
                fastNumber.Add((int)(c - '0'));
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
          if (index >= str.Length)
            return null;
          c = str[index];
          index++;
          if (c >= '0' && c <= '9') {
            fastExponent.Add((int)(c - '0'));
            while (index < str.Length) {
              c = str[index];
              if (c >= '0' && c <= '9') {
                index++;
                fastExponent.Multiply(10);
                fastExponent.Add((int)(c - '0'));
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
      if (negExp)
        fastExponent.Negate();
      if (negative)
        fastNumber.Negate();
      fastExponent.Add(exponentAdjust);
      if (index != str.Length) {
        // End of the string wasn't reached, so isn't a number
        return null;
      }
      // No fractional part
      if(fastExponent.Sign==0){
        if(fastNumber.CanFitInInt32())
          return CBORObject.FromObject(fastNumber.AsInt32());
        else
          return CBORObject.FromObject(fastNumber.AsBigInteger());
      } else {
        if(fastNumber.Sign==0){
          return CBORObject.FromObject(0);
        }
        if(fastNumber.CanFitInInt32() && fastExponent.CanFitInInt32()){
          return CBORObject.FromObject(new DecimalFraction(
            fastNumber.AsInt32(),fastExponent.AsInt32()));
        } else {
          BigInteger bigintExponent=fastExponent.AsBigInteger();
          if(!fastExponent.CanFitInInt32()){
            if (bigintExponent.CompareTo(UInt64MaxValue) > 0) {
              // Exponent is higher than the highest representable
              // integer of major type 0
              if (failOnExponentOverflow)
                return null;
              else
                return (fastExponent.Sign < 0) ?
                  CBORObject.FromObject(Double.NegativeInfinity) :
                  CBORObject.FromObject(Double.PositiveInfinity);
            }
            if (bigintExponent.CompareTo(LowestMajorType1) < 0) {
              // Exponent is lower than the lowest representable
              // integer of major type 1
              if (failOnExponentOverflow)
                return null;
              else
                return CBORObject.FromObject(0);
            }
          }
          return CBORObject.FromObject(new DecimalFraction(
            fastNumber.AsBigInteger(),bigintExponent));
        }
      }
    }
  }
}