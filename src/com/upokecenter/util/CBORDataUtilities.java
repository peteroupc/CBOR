package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */


//import java.math.*;


    /**
     * Contains methods useful for reading and writing data, with a focus
     * on CBOR.
     */
  public final class CBORDataUtilities {
private CBORDataUtilities(){}
    private static BigInteger LowestMajorType1 = BigInteger.ZERO .subtract(BigInteger.ONE.shiftLeft(64));
    private static BigInteger UInt64MaxValue = (BigInteger.ONE.shiftLeft(64)).subtract(BigInteger.ONE);
    

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
      if (index >= str.length())
        return null;
      c = str.charAt(index);
      index++;
      boolean negExp = false;
      FastInteger fastNumber = new FastInteger();
      FastInteger exponentAdjust = new FastInteger();
      FastInteger fastExponent = new FastInteger();
      if (c >= '1' && c <= '9') {
        fastNumber.Add((int)(c - '0'));
        while (index < str.length()) {
          c = str.charAt(index);
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
        if (index < str.length() && str.charAt(index) == '.') {
          // Fraction
          index++;
          if (index >= str.length())
            return null;
          c = str.charAt(index);
          index++;
          if (c >= '0' && c <= '9') {
            // Adjust the exponent for this
            // fractional digit
            exponentAdjust.Add(-1);
            fastNumber.Multiply(10);
            fastNumber.Add((int)(c - '0'));
            while (index < str.length()) {
              c = str.charAt(index);
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
          if (index >= str.length())
            return null;
          c = str.charAt(index);
          index++;
          if (c >= '0' && c <= '9') {
            fastExponent.Add((int)(c - '0'));
            while (index < str.length()) {
              c = str.charAt(index);
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
      if (index != str.length()) {
        // End of the String wasn't reached, so isn't a number
        return null;
      }
      // No fractional part
      if(fastExponent.signum()==0){
        if(fastNumber.CanFitInInt32())
          return CBORObject.FromObject(fastNumber.AsInt32());
        else
          return CBORObject.FromObject(fastNumber.AsBigInteger());
      } else {
        if(fastNumber.signum()==0){
          return CBORObject.FromObject(0);
        }
        if(fastNumber.CanFitInInt32() && fastExponent.CanFitInInt32()){
          return CBORObject.FromObject(new DecimalFraction(
            fastNumber.AsInt32(),fastExponent.AsInt32()));
        } else {
          BigInteger bigintExponent=fastExponent.AsBigInteger();
          if(!fastExponent.CanFitInInt32()){
            if (bigintExponent.compareTo(UInt64MaxValue) > 0) {
              // Exponent is higher than the highest representable
              // integer of major type 0
              if (failOnExponentOverflow)
                return null;
              else
                return (fastExponent.signum() < 0) ?
                  CBORObject.FromObject(Double.NEGATIVE_INFINITY) :
                  CBORObject.FromObject(Double.POSITIVE_INFINITY);
            }
            if (bigintExponent.compareTo(LowestMajorType1) < 0) {
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
