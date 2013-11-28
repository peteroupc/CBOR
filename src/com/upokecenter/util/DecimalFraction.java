package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */


import java.math.*;


    /**
     * Represents an arbitrary-precision decimal floating-point number.
     * Consists of an integer mantissa and an integer exponent, both arbitrary-precision.
     * The value of the number is equal to mantissa * 10^exponent.
     */
  public final class DecimalFraction implements Comparable<DecimalFraction> {
    BigInteger exponent;
    BigInteger mantissa;
    /**
     * Gets this object's exponent. This object's value will be an integer
     * if the exponent is positive or zero.
     */
    public BigInteger getExponent() { return exponent; }
    /**
     * Gets this object's unscaled value.
     */
    public BigInteger getMantissa() { return mantissa; }
    
    /**
     * Determines whether this object's mantissa and exponent are equal
     * to those of another object.
     * @param obj A DecimalFraction object.
     */
    public boolean equals(DecimalFraction obj) {
      DecimalFraction other = ((obj instanceof DecimalFraction) ? (DecimalFraction)obj : null);
      if (other == null)
        return false;
      return this.exponent.equals(other.exponent) &&
        this.mantissa.equals(other.mantissa);
    }
    /**
     * Determines whether this object's mantissa and exponent are equal
     * to those of another object and that other object is a decimal fraction.
     * @param obj A object object.
     * @return True if the objects are equal; false otherwise.
     */
    @Override public boolean equals(Object obj) {
      return equals(((obj instanceof DecimalFraction) ? (DecimalFraction)obj : null));
    }
    /**
     * Calculates this object's hash code.
     * @return This object's hash code.
     */
    @Override public int hashCode() {
      int hashCode_ = 0;
      {
        hashCode_ += 1000000007 * exponent.hashCode();
        hashCode_ += 1000000009 * mantissa.hashCode();
      }
      return hashCode_;
    }
    
    /**
     * Creates a decimal fraction with the value exponent*10^mantissa.
     * @param mantissa The unscaled value.
     * @param exponent The decimal exponent.
     */
    public DecimalFraction(BigInteger mantissa, BigInteger exponent) {
      this.exponent = exponent;
      this.mantissa = mantissa;
    }
    /**
     * Creates a decimal fraction with the value exponentLong*10^mantissa.
     * @param mantissa The unscaled value.
     * @param exponentLong The decimal exponent.
     */
    public DecimalFraction(BigInteger mantissa, long exponentLong) {
      this.exponent = BigInteger.valueOf(exponentLong);
      this.mantissa = mantissa;
    }
    /**
     * Creates a decimal fraction with the given mantissa and an exponent
     * of 0.
     * @param mantissa The desired value of the bigfloat
     */
    public DecimalFraction(BigInteger mantissa) {
      this.exponent = BigInteger.ZERO;
      this.mantissa = mantissa;
    }
    /**
     * Creates a decimal fraction with the given mantissa and an exponent
     * of 0.
     * @param mantissaLong The desired value of the bigfloat
     */
    public DecimalFraction(long mantissaLong) {
      this.exponent = BigInteger.ZERO;
      this.mantissa = BigInteger.valueOf(mantissaLong);
    }
    /**
     * Creates a decimal fraction with the given mantissa and an exponent
     * of 0.
     * @param mantissaLong The unscaled value.
     * @param exponentLong The decimal exponent.
     */
    public DecimalFraction(long mantissaLong, long exponentLong) {
      this.exponent = BigInteger.valueOf(exponentLong);
      this.mantissa = BigInteger.valueOf(mantissaLong);
    }

    /**
     * Creates a decimal fraction from a string that represents a number.
     * <p> The format of the string generally consists of:<ul> <li> An optional
     * '-' or '+' character (if '-', the value is negative.)</li> <li> One
     * or more digits, with a single optional decimal point after the first
     * digit and before the last digit.</li> <li> Optionally, E+ (positive
     * exponent) or E- (negative exponent) plus one or more digits specifying
     * the exponent.</li> </ul> </p> <p> The format generally follows the
     * definition in java.math.BigDecimal(), except that the digits must
     * be ASCII digits ('0' through '9').</p>
     * @param s A string that represents a number.
     */
    public static DecimalFraction FromString(String s) {
      if (s == null)
        throw new NullPointerException("s");
      if (s.length() == 0)
        throw new NumberFormatException();
      int offset = 0;
      boolean negative = false;
      if (s.charAt(0) == '+' || s.charAt(0) == '-') {
        negative = (s.charAt(0) == '-');
        offset++;
      }
      FastInteger2 mant = new FastInteger2();
      boolean haveDecimalPoint = false;
      boolean haveDigits = false;
      boolean haveExponent = false;
      FastInteger newScale = new FastInteger();
      int i = offset;
      for (; i < s.length(); i++) {
        if (s.charAt(i) >= '0' && s.charAt(i) <= '9') {
          int thisdigit = (int)(s.charAt(i) - '0');
          mant.MultiplyByTen();
          mant.Add(thisdigit);
          haveDigits = true;
          if (haveDecimalPoint) {
            newScale.Add(-1);
          }
        } else if (s.charAt(i) == '.') {
          if (haveDecimalPoint)
            throw new NumberFormatException();
          haveDecimalPoint = true;
        } else if (s.charAt(i) == 'E' || s.charAt(i) == 'e') {
          haveExponent = true;
          i++;
          break;
        } else {
          throw new NumberFormatException();
        }
      }
      if (!haveDigits)
        throw new NumberFormatException();
      if (haveExponent) {
        FastInteger2 exp = new FastInteger2();
        offset = 1;
        haveDigits = false;
        if (i == s.length()) throw new NumberFormatException();
        if (s.charAt(i) == '+' || s.charAt(i) == '-') {
          if (s.charAt(i) == '-') offset = -1;
          i++;
        }
        for (; i < s.length(); i++) {
          if (s.charAt(i) >= '0' && s.charAt(i) <= '9') {
            haveDigits = true;
            int thisdigit = (int)(s.charAt(i) - '0');
            exp.MultiplyByTen();
            exp.Add(thisdigit);
          } else {
            throw new NumberFormatException();
          }
        }
        if (!haveDigits)
          throw new NumberFormatException();
        if (offset < 0)
          exp.SubtractThisFrom(newScale);
        else
          exp.AddThisTo(newScale);
      } else if (i != s.length()) {
        throw new NumberFormatException();
      }
      return new DecimalFraction(
        (negative) ? mant.AsNegatedBigInteger() :
        mant.AsBigInteger(),
        newScale.AsBigInteger());
    }
    static BigInteger FindPowerOfFiveFromBig(BigInteger diff) {
      if (diff.signum() <= 0) return BigInteger.ONE;
      BigInteger bigpow = BigInteger.ZERO;
      FastInteger intcurexp = new FastInteger(diff);
      if (intcurexp.compareTo(54) <= 0) {
        return FindPowerOfFive(intcurexp.AsInt32());
      }
      BigInteger mantissa = BigInteger.ONE;
      while (intcurexp.signum() > 0) {
        if (intcurexp.compareTo(27) <= 0) {
          bigpow = FindPowerOfFive(intcurexp.AsInt32());
          mantissa=mantissa.multiply(bigpow);
          break;
        } else if (intcurexp.compareTo(9999999) <= 0) {
          bigpow = (FindPowerOfFive(1)).pow(intcurexp.AsInt32());
          mantissa=mantissa.multiply(bigpow);
          break;
        } else {
          if (bigpow.signum()==0)
            bigpow = (FindPowerOfFive(1)).pow(9999999);
          mantissa=mantissa.multiply(bigpow);
          intcurexp.Add(-9999999);
        }
      }
      return mantissa;
    }

    private static BigInteger BigInt36 = BigInteger.valueOf(36);

    static BigInteger FindPowerOfTenFromBig(BigInteger bigintExponent) {
      if (bigintExponent.signum() <= 0) return BigInteger.ONE;
      if (bigintExponent.compareTo(BigInt36) <= 0) {
        return FindPowerOfTen(bigintExponent.intValue());
      }
      FastInteger intcurexp = new FastInteger(bigintExponent);
      BigInteger mantissa = BigInteger.ONE;
      BigInteger bigpow = BigInteger.ZERO;
      while (intcurexp.signum() > 0) {
        if (intcurexp.compareTo(18) <= 0) {
          bigpow = FindPowerOfTen(intcurexp.AsInt32());
          mantissa=mantissa.multiply(bigpow);
          break;
        } else if (intcurexp.compareTo(9999999) <= 0) {
          int val = intcurexp.AsInt32();
          bigpow = FindPowerOfFive(val);
          bigpow=bigpow.shiftLeft(val);
          mantissa=mantissa.multiply(bigpow);
          break;
        } else {
          if (bigpow.signum()==0) {
            bigpow = FindPowerOfFive(9999999);
            bigpow=bigpow.shiftLeft(9999999);
          }
          mantissa=mantissa.multiply(bigpow);
          intcurexp.Add(-9999999);
        }
      }
      return mantissa;
    }

    static BigInteger FindPowerOfFive(long precision) {
      if (precision <= 0) return BigInteger.ONE;
      BigInteger bigpow;
      BigInteger ret;
      if (precision <= 27)
        return BigIntPowersOfFive[(int)precision];
      if (precision <= 54) {
        ret = BigIntPowersOfFive[27];
        bigpow = BigIntPowersOfFive[((int)precision) - 27];
        ret=ret.multiply(bigpow);
        return ret;
      }
      ret = BigInteger.ONE;
      boolean first = true;
      bigpow = BigInteger.ZERO;
      while (precision > 0) {
        if (precision <= 27) {
          bigpow = BigIntPowersOfFive[(int)precision];
          if (first)
            ret = bigpow;
          else
            ret=ret.multiply(bigpow);
          first = false;
          break;
        } else if (precision <= 9999999) {
          bigpow = (BigIntPowersOfFive[1]).pow((int)precision);
          if (first)
            ret = bigpow;
          else
            ret=ret.multiply(bigpow);
          first = false;
          break;
        } else {
          if (bigpow.signum()==0)
            bigpow = (BigIntPowersOfFive[1]).pow(9999999);
          if (first)
            ret = bigpow;
          else
            ret=ret.multiply(bigpow);
          first = false;
          precision -= 9999999;
        }
      }
      return ret;
    }

    static BigInteger FindPowerOfTen(long precision) {
      if (precision <= 0) return BigInteger.ONE;
      BigInteger ret;
      BigInteger bigpow;
      if (precision <= 18)
        return BigIntPowersOfTen[(int)precision];
      if (precision <= 27) {
        int prec = (int)precision;
        ret = BigIntPowersOfFive[prec];
        ret=ret.shiftLeft(prec);
        return ret;
      }
      if (precision <= 36) {
        ret = BigIntPowersOfTen[18];
        bigpow = BigIntPowersOfTen[((int)precision) - 18];
        ret=ret.multiply(bigpow);
        return ret;
      }
      ret = BigInteger.ONE;
      boolean first = true;
      bigpow = BigInteger.ZERO;
      while (precision > 0) {
        if (precision <= 18) {
          bigpow = BigIntPowersOfTen[(int)precision];
          if (first)
            ret = bigpow;
          else
            ret=ret.multiply(bigpow);
          first = false;
          break;
        } else if (precision <= 9999999) {
          int prec = (int)precision;
          bigpow = FindPowerOfFive(prec);
          bigpow=bigpow.shiftLeft(prec);
          if (first)
            ret = bigpow;
          else
            ret=ret.multiply(bigpow);
          first = false;
          break;
        } else {
          if (bigpow.signum()==0)
            bigpow = (BigIntPowersOfTen[1]).pow(9999999);
          if (first)
            ret = bigpow;
          else
            ret=ret.multiply(bigpow);
          first = false;
          precision -= 9999999;
        }
      }
      return ret;
    }

    private static final class DecimalMathHelper implements IRadixMathHelper<DecimalFraction> {

    /**
     * 
     */
      public int GetRadix() {
        return 10;
      }

    /**
     * 
     * @param value A DecimalFraction object.
     */
      public DecimalFraction Abs(DecimalFraction value) {
        return value.Abs();
      }

    /**
     * 
     * @param value A DecimalFraction object.
     */
      public int GetSign(DecimalFraction value) {
        return value.signum();
      }

    /**
     * 
     * @param value A DecimalFraction object.
     */
      public BigInteger GetMantissa(DecimalFraction value) {
        return value.mantissa;
      }

    /**
     * 
     * @param value A DecimalFraction object.
     */
      public BigInteger GetExponent(DecimalFraction value) {
        return value.exponent;
      }

    /**
     * 
     * @param mantissa A BigInteger object.
     * @param e1 A BigInteger object.
     * @param e2 A BigInteger object.
     */
      public BigInteger RescaleByExponentDiff(BigInteger mantissa, BigInteger e1, BigInteger e2) {
        boolean negative = (mantissa.signum() < 0);
        if (mantissa.signum() == 0) return BigInteger.ZERO;
        if (negative) mantissa=mantissa.negate();
        FastInteger diff = new FastInteger(e1).Subtract(e2).Abs();
        if (diff.CanFitInInt64()) {
          mantissa=mantissa.multiply(FindPowerOfTen(diff.AsInt64()));
        } else {
          mantissa=mantissa.multiply(FindPowerOfTenFromBig(diff.AsBigInteger()));
        }
        if (negative) mantissa=mantissa.negate();
        return mantissa;
      }

    /**
     * 
     * @param mantissa A BigInteger object.
     * @param exponent A BigInteger object.
     */
      public DecimalFraction CreateNew(BigInteger mantissa, BigInteger exponent) {
        return new DecimalFraction(mantissa, exponent);
      }

    /**
     * 
     * @param value A BigInteger object.
     * @param lastDigit A 32-bit signed integer.
     * @param olderDigits A 32-bit signed integer.
     * @param bigint A BigInteger object.
     */
      public IShiftAccumulator CreateShiftAccumulator(BigInteger bigint, int lastDigit, int olderDigits) {
        return new DigitShiftAccumulator(bigint, lastDigit, olderDigits);
      }

    /**
     * 
     * @param value A BigInteger object.
     * @param bigint A BigInteger object.
     */
      public IShiftAccumulator CreateShiftAccumulator(BigInteger bigint) {
        return new DigitShiftAccumulator(bigint);
      }

    /**
     * 
     * @param value A 64-bit signed integer.
     */
      public IShiftAccumulator CreateShiftAccumulator(long value) {
        return new DigitShiftAccumulator(value);
      }

    /**
     * 
     * @param numerator A BigInteger object.
     * @param denominator A BigInteger object.
     */
      public boolean HasTerminatingRadixExpansion(BigInteger numerator, BigInteger denominator) {
        // Simplify denominator based on numerator
        BigInteger gcd = numerator.gcd(denominator);
        denominator=denominator.divide(gcd);
        if (denominator.signum()==0)
          return false;
        // Eliminate factors of 2
        while (denominator.testBit(0)==false) {
          denominator=denominator.shiftRight(1);
        }
        // Eliminate factors of 5
        while ((denominator.remainder(BigInteger.valueOf(5))).signum()==0) {
          denominator=denominator.divide(BigInteger.valueOf(5));
        }
        return denominator.compareTo(BigInteger.ONE) == 0;
      }

    /**
     * 
     * @param bigint A BigInteger object.
     * @param power A 64-bit signed integer.
     */
      public BigInteger MultiplyByRadixPower(BigInteger bigint, long power) {
        if (power <= 0) return bigint;
        if (bigint.equals(BigInteger.ONE)) {
          bigint = FindPowerOfTen(power);
        } else if (bigint.signum()==0) {
          return bigint;
        } else {
          bigint=bigint.multiply(FindPowerOfTen(power));
        }
        return bigint;
      }

    /**
     * 
     * @param bigint A BigInteger object.
     * @param power A FastInteger object.
     */
      public BigInteger MultiplyByRadixPower(BigInteger bigint, FastInteger power) {
        if (power.signum() <= 0) return bigint;
        if (bigint.signum()==0) return bigint;
        if (power.CanFitInInt64()) {
          bigint=bigint.multiply(FindPowerOfTen(power.AsInt64()));
        } else {
          bigint=bigint.multiply(FindPowerOfTenFromBig(power.AsBigInteger()));
        }
        return bigint;
      }
    }


    private static boolean AppendString(StringBuilder builder, char c, FastInteger count) {
      if (count.compareTo(Integer.MAX_VALUE) > 0 || count.signum() < 0) {
        throw new UnsupportedOperationException();
      }
      int icount = count.AsInt32();
      for (int i = icount - 1; i >= 0; i--) {
        builder.append(c);
      }
      return true;
    }
    private String ToStringInternal(int mode) {
      // Using Java's rules for converting DecimalFraction
      // values to a String
      String mantissaString = this.mantissa.toString();
      int scaleSign = -this.exponent.signum();
      if (scaleSign == 0)
        return mantissaString;
      boolean iszero = (this.mantissa.signum()==0);
      if (mode == 2 && iszero && scaleSign < 0) {
        // special case for zero in plain
        return mantissaString;
      }
      FastInteger sbLength = new FastInteger(mantissaString.length());
      int negaPos = 0;
      if (mantissaString.charAt(0) == '-') {
        sbLength.Add(-1);
        negaPos = 1;
      }
      FastInteger adjustedExponent = new FastInteger(this.exponent);
      FastInteger thisExponent = new FastInteger(adjustedExponent);
      adjustedExponent.Add(sbLength).Add(-1);
      FastInteger decimalPointAdjust = new FastInteger(1);
      FastInteger threshold = new FastInteger(-6);
      if (mode == 1) { // engineering String adjustments
        FastInteger newExponent = new FastInteger(adjustedExponent);
        boolean adjExponentNegative = (adjustedExponent.signum() < 0);
        int intphase = new FastInteger(adjustedExponent).Abs().Mod(3).AsInt32();
        if (iszero && (adjustedExponent.compareTo(threshold) < 0 ||
                       scaleSign < 0)) {
          if (intphase == 1) {
            if (adjExponentNegative) {
              decimalPointAdjust.Add(1);
              newExponent.Add(1);
            } else {
              decimalPointAdjust.Add(2);
              newExponent.Add(2);
            }
          } else if (intphase == 2) {
            if (!adjExponentNegative) {
              decimalPointAdjust.Add(1);
              newExponent.Add(1);
            } else {
              decimalPointAdjust.Add(2);
              newExponent.Add(2);
            }
          }
          threshold.Add(1);
        } else {
          if (intphase == 1) {
            if (!adjExponentNegative) {
              decimalPointAdjust.Add(1);
              newExponent.Add(-1);
            } else {
              decimalPointAdjust.Add(2);
              newExponent.Add(-2);
            }
          } else if (intphase == 2) {
            if (adjExponentNegative) {
              decimalPointAdjust.Add(1);
              newExponent.Add(-1);
            } else {
              decimalPointAdjust.Add(2);
              newExponent.Add(-2);
            }
          }
        }
        adjustedExponent = newExponent;
      }
      if (mode == 2 || ((adjustedExponent.compareTo(threshold) >= 0 &&
                         scaleSign >= 0))) {
        if (scaleSign > 0) {
          FastInteger decimalPoint = new FastInteger(thisExponent).Add(negaPos).Add(sbLength);
          int cmp = decimalPoint.compareTo(negaPos);
          StringBuilder builder = null;
          if (cmp < 0) {
            builder = new StringBuilder((int)Math.min(Integer.MAX_VALUE, (long)mantissaString.length() + 6));
            builder.append(mantissaString,0,(0)+(negaPos));
            builder.append("0.");
            AppendString(builder, '0', new FastInteger(negaPos).Subtract(decimalPoint));
            builder.append(mantissaString,negaPos,(negaPos)+(mantissaString.length() - negaPos));
          } else if (cmp == 0) {
            if (!decimalPoint.CanFitInInt32())
              throw new UnsupportedOperationException();
            int tmpInt = decimalPoint.AsInt32();
            if (tmpInt < 0) tmpInt = 0;
            builder = new StringBuilder((int)Math.min(Integer.MAX_VALUE, (long)mantissaString.length() + 6));
            builder.append(mantissaString,0,(0)+(tmpInt));
            builder.append("0.");
            builder.append(mantissaString,tmpInt,(tmpInt)+(mantissaString.length() - tmpInt));
          } else if (decimalPoint.compareTo(new FastInteger(negaPos).Add(mantissaString.length())) > 0) {
            FastInteger insertionPoint = new FastInteger(negaPos).Add(sbLength);
            if (!insertionPoint.CanFitInInt32())
              throw new UnsupportedOperationException();
            int tmpInt = insertionPoint.AsInt32();
            if (tmpInt < 0) tmpInt = 0;
            builder = new StringBuilder((int)Math.min(Integer.MAX_VALUE, (long)mantissaString.length() + 6));
            builder.append(mantissaString,0,(0)+(tmpInt));
            AppendString(builder, '0',
                         new FastInteger(decimalPoint).Subtract(builder.length()));
            builder.append('.');
            builder.append(mantissaString,tmpInt,(tmpInt)+(mantissaString.length() - tmpInt));
          } else {
            if (!decimalPoint.CanFitInInt32())
              throw new UnsupportedOperationException();
            int tmpInt = decimalPoint.AsInt32();
            if (tmpInt < 0) tmpInt = 0;
            builder = new StringBuilder((int)Math.min(Integer.MAX_VALUE, (long)mantissaString.length() + 6));
            builder.append(mantissaString,0,(0)+(tmpInt));
            builder.append('.');
            builder.append(mantissaString,tmpInt,(tmpInt)+(mantissaString.length() - tmpInt));
          }
          return builder.toString();
        } else if (mode == 2 && scaleSign < 0) {
          FastInteger negscale = new FastInteger(thisExponent);
          StringBuilder builder = new StringBuilder();
          builder.append(mantissaString);
          AppendString(builder, '0', negscale);
          return builder.toString();
        } else {
          return mantissaString;
        }
      } else {
        StringBuilder builder = null;
        if (mode == 1 && iszero && decimalPointAdjust.compareTo(1) > 0) {
          builder = new StringBuilder();
          builder.append(mantissaString);
          builder.append('.');
          AppendString(builder, '0', new FastInteger(decimalPointAdjust).Add(-1));
        } else {
          FastInteger tmp = new FastInteger(negaPos).Add(decimalPointAdjust);
          int cmp = tmp.compareTo(mantissaString.length());
          if (cmp > 0) {
            tmp.Subtract(mantissaString.length());
            builder = new StringBuilder();
            builder.append(mantissaString);
            AppendString(builder, '0', tmp);
          } else if (cmp < 0) {
            // Insert a decimal point at the right place
            if (!tmp.CanFitInInt32())
              throw new UnsupportedOperationException();
            int tmpInt = tmp.AsInt32();
            if (tmp.signum() < 0) tmpInt = 0;
            builder = new StringBuilder(
              (int)Math.min(Integer.MAX_VALUE, (long)mantissaString.length() + 6));
            builder.append(mantissaString,0,(0)+(tmpInt));
            builder.append('.');
            builder.append(mantissaString,tmpInt,(tmpInt)+(mantissaString.length() - tmpInt));
          } else if (adjustedExponent.signum() == 0) {
            return mantissaString;
          } else {
            builder = new StringBuilder();
            builder.append(mantissaString);
          }
        }
        if (adjustedExponent.signum() != 0) {
          builder.append(adjustedExponent.signum() < 0 ? "E-" : "E+");
          adjustedExponent.Abs();
          StringBuilder builderReversed = new StringBuilder();
          while (adjustedExponent.signum() != 0) {
            int digit = new FastInteger(adjustedExponent).Mod(10).AsInt32();
            // Each digit is retrieved from right to left
            builderReversed.append((char)('0' + digit));
            adjustedExponent.Divide(10);
          }
          int count = builderReversed.length();
          for (int i = 0; i < count; i++) {
            builder.append(builderReversed.charAt(count - 1 - i));
          }
        }
        return builder.toString();
      }
    }

    private static BigInteger[] BigIntPowersOfTen = new BigInteger[]{
      BigInteger.ONE, BigInteger.TEN, BigInteger.valueOf(100), BigInteger.valueOf(1000), BigInteger.valueOf(10000), BigInteger.valueOf(100000), BigInteger.valueOf(1000000), BigInteger.valueOf(10000000), BigInteger.valueOf(100000000), BigInteger.valueOf(1000000000),
      BigInteger.valueOf(10000000000L), BigInteger.valueOf(100000000000L), BigInteger.valueOf(1000000000000L), BigInteger.valueOf(10000000000000L),
      BigInteger.valueOf(100000000000000L), BigInteger.valueOf(1000000000000000L), BigInteger.valueOf(10000000000000000L),
      BigInteger.valueOf(100000000000000000L), BigInteger.valueOf(1000000000000000000L)
    };

    private static BigInteger[] BigIntPowersOfFive = new BigInteger[]{
      BigInteger.ONE, BigInteger.valueOf(5), BigInteger.valueOf(25), BigInteger.valueOf(125), BigInteger.valueOf(625), BigInteger.valueOf(3125), BigInteger.valueOf(15625), BigInteger.valueOf(78125), BigInteger.valueOf(390625),
      BigInteger.valueOf(1953125), BigInteger.valueOf(9765625), BigInteger.valueOf(48828125), BigInteger.valueOf(244140625), BigInteger.valueOf(1220703125),
      BigInteger.valueOf(6103515625L), BigInteger.valueOf(30517578125L), BigInteger.valueOf(152587890625L), BigInteger.valueOf(762939453125L),
      BigInteger.valueOf(3814697265625L), BigInteger.valueOf(19073486328125L), BigInteger.valueOf(95367431640625L),
      BigInteger.valueOf(476837158203125L), BigInteger.valueOf(2384185791015625L), BigInteger.valueOf(11920928955078125L),
      BigInteger.valueOf(59604644775390625L), BigInteger.valueOf(298023223876953125L), BigInteger.valueOf(1490116119384765625L),
      BigInteger.valueOf(7450580596923828125L)
    };

    /**
     * Converts this value to an arbitrary-precision integer. Any fractional
     * part in this value will be discarded when converting to a big integer.
     */
    public BigInteger ToBigInteger() {
      int sign = this.getExponent().signum();
      if (sign == 0) {
        return this.getMantissa();
      } else if (sign > 0) {
        BigInteger bigmantissa = this.getMantissa();
        bigmantissa=bigmantissa.multiply(FindPowerOfTenFromBig(this.getExponent()));
        return bigmantissa;
      } else {
        BigInteger bigmantissa = this.getMantissa();
        BigInteger bigexponent = this.getExponent();
        bigexponent=bigexponent.negate();
        bigmantissa=bigmantissa.divide(FindPowerOfTenFromBig(bigexponent));
        return bigmantissa;
      }
    }
    /**
     * Converts this value to a 32-bit floating-point number. The half-even
     * rounding mode is used.
     * @return The closest 32-bit floating-point number to this value.
     * The return value can be positive infinity or negative infinity if
     * this value exceeds the range of a 32-bit floating point number.
     */
    public float ToSingle() {
      return BigFloat.FromDecimalFraction(this).ToSingle();
    }
    /**
     * Converts this value to a 64-bit floating-point number. The half-even
     * rounding mode is used.
     * @return The closest 64-bit floating-point number to this value.
     * The return value can be positive infinity or negative infinity if
     * this value exceeds the range of a 64-bit floating point number.
     */
    public double ToDouble() {
      return BigFloat.FromDecimalFraction(this).ToDouble();
    }
    /**
     * Creates a decimal fraction from a 32-bit floating-point number.
     * This method computes the exact value of the floating point number,
     * not an approximation, as is often the case by converting the number
     * to a string.
     * @param flt A 32-bit floating-point number.
     * @return A decimal fraction with the same value as "flt".
     * @throws ArithmeticException "flt" is infinity or not-a-number.
     */
    public static DecimalFraction FromSingle(float flt) {
      int value = Float.floatToRawIntBits(flt);
      int fpExponent = (int)((value >> 23) & 0xFF);
      if (fpExponent == 255)
        throw new ArithmeticException("Value is infinity or NaN");
      int fpMantissa = value & 0x7FFFFF;
      if (fpExponent == 0) fpExponent++;
      else fpMantissa |= (1 << 23);
      if (fpMantissa == 0) return new DecimalFraction(0);
      fpExponent -= 150;
      while ((fpMantissa & 1) == 0) {
        fpExponent++;
        fpMantissa >>= 1;
      }
      boolean neg = ((value >> 31) != 0);
      if (fpExponent == 0) {
        if (neg) fpMantissa = -fpMantissa;
        return new DecimalFraction(fpMantissa);
      } else if (fpExponent > 0) {
        // Value is an integer
        BigInteger bigmantissa = BigInteger.valueOf(fpMantissa);
        bigmantissa=bigmantissa.shiftLeft(fpExponent);
        if (neg) bigmantissa=(bigmantissa).negate();
        return new DecimalFraction(bigmantissa);
      } else {
        // Value has a fractional part
        BigInteger bigmantissa = BigInteger.valueOf(fpMantissa);
        bigmantissa=bigmantissa.multiply(FindPowerOfFive(-fpExponent));
        if (neg) bigmantissa=(bigmantissa).negate();
        return new DecimalFraction(bigmantissa, fpExponent);
      }
    }
    /**
     * Creates a decimal fraction from a 64-bit floating-point number.
     * This method computes the exact value of the floating point number,
     * not an approximation, as is often the case by converting the number
     * to a string.
     * @param dbl A 64-bit floating-point number.
     * @return A decimal fraction with the same value as "dbl"
     * @throws ArithmeticException "dbl" is infinity or not-a-number.
     */
    public static DecimalFraction FromDouble(double dbl) {
      long value = Double.doubleToRawLongBits(dbl);
      int fpExponent = (int)((value >> 52) & 0x7ffL);
      if (fpExponent == 2047)
        throw new ArithmeticException("Value is infinity or NaN");
      long fpMantissa = value & 0xFFFFFFFFFFFFFL;
      if (fpExponent == 0) fpExponent++;
      else fpMantissa |= (1L << 52);
      if (fpMantissa == 0) return new DecimalFraction(0);
      fpExponent -= 1075;
      while ((fpMantissa & 1) == 0) {
        fpExponent++;
        fpMantissa >>= 1;
      }
      boolean neg = ((value >> 63) != 0);
      if (fpExponent == 0) {
        if (neg) fpMantissa = -fpMantissa;
        return new DecimalFraction(fpMantissa);
      } else if (fpExponent > 0) {
        // Value is an integer
        BigInteger bigmantissa = BigInteger.valueOf(fpMantissa);
        bigmantissa=bigmantissa.shiftLeft(fpExponent);
        if (neg) bigmantissa=(bigmantissa).negate();
        return new DecimalFraction(bigmantissa);
      } else {
        // Value has a fractional part
        BigInteger bigmantissa = BigInteger.valueOf(fpMantissa);
        bigmantissa=bigmantissa.multiply(FindPowerOfFive(-fpExponent));
        if (neg) bigmantissa=(bigmantissa).negate();
        return new DecimalFraction(bigmantissa, fpExponent);
      }
    }

    /**
     * Creates a decimal fraction from an arbitrary-precision binary floating-point
     * number.
     * @param bigfloat A bigfloat.
     */
    public static DecimalFraction FromBigFloat(BigFloat bigfloat) {
      BigInteger bigintExp = bigfloat.getExponent();
      BigInteger bigintMant = bigfloat.getMantissa();
      if (bigintExp.signum()==0) {
        // Integer
        return new DecimalFraction(bigintMant);
      } else if (bigintExp.signum()>0) {
        // Scaled integer
        FastInteger intcurexp = new FastInteger(bigintExp);
        BigInteger bigmantissa = bigintMant;
        boolean neg = (bigmantissa.signum() < 0);
        if (neg) bigmantissa=(bigmantissa).negate();
        while (intcurexp.signum() > 0) {
          int shift = 512;
          if (intcurexp.compareTo(512) < 0) {
            shift = intcurexp.AsInt32();
          }
          bigmantissa=bigmantissa.shiftLeft(shift);
          intcurexp.Add(-shift);
        }
        if (neg) bigmantissa=(bigmantissa).negate();
        return new DecimalFraction(bigmantissa);
      } else {
        // Fractional number
        BigInteger bigmantissa = bigintMant;
        BigInteger negbigintExp=(bigintExp).negate();
        bigmantissa=bigmantissa.multiply(FindPowerOfFiveFromBig(negbigintExp));
        return new DecimalFraction(bigmantissa, bigintExp);
      }
    }
    /**
     * Converts this value to a string.The format of the return value is exactly
     * the same as that of the java.math.BigDecimal.toString() method.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return ToStringInternal(0);
    }
    /**
     * Same as toString(), except that when an exponent is used it will be
     * a multiple of 3. The format of the return value follows the format of
     * the java.math.BigDecimal.toEngineeringString() method.
     */
    public String ToEngineeringString() {
      return ToStringInternal(1);
    }
    /**
     * Converts this value to a string, but without an exponent part. The
     * format of the return value follows the format of the java.math.BigDecimal.toPlainString()
     * method.
     */
    public String ToPlainString() {
      return ToStringInternal(2);
    }

    /**
     * Represents the number 1.
     */
    public static final DecimalFraction One = new DecimalFraction(1);

    /**
     * Represents the number 0.
     */
    public static final DecimalFraction Zero = new DecimalFraction(0);
    /**
     * Represents the number 10.
     */
    public static final DecimalFraction Ten = new DecimalFraction(10);

    //----------------------------------------------------------------

    /**
     * Gets this value's sign: -1 if negative; 1 if positive; 0 if zero.
     */
    public int signum() {
        return mantissa.signum();
      }
    /**
     * Gets whether this object's value equals 0.
     */
    public boolean isZero() {
        return mantissa.signum()==0;
      }
    /**
     * Gets the absolute value of this object.
     */
    public DecimalFraction Abs() {
      if (this.signum() < 0) {
        return Negate();
      } else {
        return this;
      }
    }

    /**
     * Gets an object with the same value as this one, but with the sign reversed.
     */
    public DecimalFraction Negate() {
      BigInteger neg=(this.mantissa).negate();
      return new DecimalFraction(neg, this.exponent);
    }

    /**
     * Divides this object by another decimal fraction and returns the result.
     * @param divisor The divisor.
     * @return The quotient of the two numbers.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The result would have a nonterminating
     * decimal expansion.
     */
    public DecimalFraction Divide(DecimalFraction divisor) {
      return Divide(divisor, PrecisionContext.Unlimited);
    }

    /**
     * Divides this object by another decimal fraction and returns a result
     * with the same exponent as this object (the dividend).
     * @param divisor The divisor.
     * @param rounding The rounding mode to use if the result must be scaled
     * down to have the same exponent as this value.
     * @return The quotient of the two numbers.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary
     * and the result is not exact.
     */
    public DecimalFraction Divide(DecimalFraction divisor, Rounding rounding) {
      return Divide(divisor, this.exponent, rounding);
    }

    /**
     * Divides two DecimalFraction objects, and returns the integer part
     * of the result, with the preferred exponent set to this value's exponent
     * minus the divisor's exponent.
     * @param divisor The divisor.
     * @return The integer part of the quotient of the two objects.
     * @throws ArithmeticException Attempted to divide by zero.
     */
    public DecimalFraction DivideToIntegerNaturalScale(
      DecimalFraction divisor
      ) {
      return DivideToIntegerNaturalScale(divisor, PrecisionContext.Unlimited);
    }


    /**
     * 
     * @param divisor A DecimalFraction object.
     */
    public DecimalFraction RemainderNaturalScale(
          DecimalFraction divisor
          ) {
      return Subtract(this.DivideToIntegerNaturalScale(divisor).Multiply(divisor));
    }


    /**
     * Divides two DecimalFraction objects, and gives a particular exponent
     * to the result.
     * @param divisor A DecimalFraction object.
     * @param desiredExponentLong The desired exponent. A negative number
     * places the cutoff point to the right of the usual decimal point. A positive
     * number places the cutoff point to the left of the usual decimal point.
     * @param ctx A precision context object to control the rounding mode.
     * The precision and exponent range settings of this context are ignored.
     * If HasFlags of the context is true, will also store the flags resulting
     * from the operation (the flags are in addition to the pre-existing
     * flags). Can be null, in which case the default rounding mode is HalfEven.
     * @return The quotient of the two objects.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary
     * and the result is not exact.
     */
    public DecimalFraction Divide(
          DecimalFraction divisor,
          long desiredExponentLong,
          PrecisionContext ctx
        ) {
      return Divide(divisor, BigInteger.valueOf(desiredExponentLong), ctx);
    }









    /**
     * Divides two DecimalFraction objects.
     * @param divisor A DecimalFraction object.
     * @param desiredExponentLong A 64-bit signed integer.
     * @param rounding A Rounding object.
     * @return The quotient of the two objects.
     */
    public DecimalFraction Divide(
              DecimalFraction divisor,
              long desiredExponentLong,
              Rounding rounding
            ) {
      return Divide(divisor, BigInteger.valueOf(desiredExponentLong), new PrecisionContext(rounding));
    }









    /**
     * Divides two DecimalFraction objects.
     * @param divisor A DecimalFraction object.
     * @param desiredExponent A BigInteger object.
     * @param rounding A Rounding object.
     * @return The quotient of the two objects.
     */
    public DecimalFraction Divide(
              DecimalFraction divisor,
              BigInteger desiredExponent,
              Rounding rounding
            ) {
      return Divide(divisor, desiredExponent, new PrecisionContext(rounding));
    }


    /**
     * 
     * @param context A PrecisionContext object.
     */
    public DecimalFraction Abs(PrecisionContext context) {
      return Abs().RoundToPrecision(context);
    }










    /**
     * 
     * @param context A PrecisionContext object.
     */
    public DecimalFraction Negate(PrecisionContext context) {
      return Negate().RoundToPrecision(context);
    }







    /**
     * 
     * @param decfrac A DecimalFraction object.
     */
    public DecimalFraction Add(DecimalFraction decfrac) {
      return Add(decfrac, PrecisionContext.Unlimited);
    }






    /**
     * Subtracts a DecimalFraction object from this instance.
     * @param decfrac A DecimalFraction object.
     * @return The difference of the two objects.
     */
    public DecimalFraction Subtract(DecimalFraction decfrac) {
      return Add(decfrac.Negate());
    }










    /**
     * Subtracts a DecimalFraction object from another DecimalFraction
     * object.
     * @param decfrac A DecimalFraction object.
     * @param ctx A PrecisionContext object.
     * @return The difference of the two objects.
     */
    public DecimalFraction Subtract(DecimalFraction decfrac, PrecisionContext ctx) {
      return Add(decfrac.Negate(), ctx);
    }
    /**
     * Multiplies two decimal fractions. The resulting scale will be the
     * sum of the scales of the two decimal fractions.
     * @param decfrac Another decimal fraction.
     * @return The product of the two decimal fractions. If a precision context
     * is given, returns null if the result of rounding would cause an overflow.
     */
    public DecimalFraction Multiply(DecimalFraction decfrac) {
      return Multiply(decfrac, PrecisionContext.Unlimited);
    }

    /**
     * Multiplies by one decimal fraction, and then adds another decimal
     * fraction.
     * @param multiplicand The value to multiply.
     * @param augend The value to add.
     * @return The result this * multiplicand + augend.
     */
    public DecimalFraction MultiplyAndAdd(DecimalFraction multiplicand,
                                          DecimalFraction augend) {
      return this.Multiply(multiplicand).Add(augend);
    }
    //----------------------------------------------------------------

    private static RadixMath<DecimalFraction> math = new RadixMath<DecimalFraction>(
  new DecimalMathHelper());

    /**
     * Divides this object by another object, and returns the integer part
     * of the result, with the preferred exponent set to thisValue value's
     * exponent minus the divisor's exponent.
     * @param divisor The divisor.
     * @param ctx A precision context object to control the precision, rounding,
     * and exponent range of the integer part of the result. Flags will be
     * set on the given context only if the context's HasFlags is true and
     * the integer part of the result doesn't fit the precision and exponent
     * range without rounding.
     * @return The integer part of the quotient of the two objects. Returns
     * null if the return value would overflow the exponent range. A caller
     * can handle a null return value by treating it as positive infinity
     * if both operands have the same sign or as negative infinity if both
     * operands have different signs.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary
     * and the integer part of the result is not exact.
     */
    public DecimalFraction DivideToIntegerNaturalScale(
  DecimalFraction divisor, PrecisionContext ctx) {
      return math.DivideToIntegerNaturalScale(this, divisor, ctx);
    }

    /**
     * 
     * @param divisor A DecimalFraction object.
     * @param ctx A PrecisionContext object.
     */
    public DecimalFraction DivideToIntegerZeroScale(
          DecimalFraction divisor, PrecisionContext ctx) {
      return math.DivideToIntegerZeroScale(this, divisor, ctx);
    }

    /**
     * Divides this DecimalFraction object by another DecimalFraction
     * object. The preferred exponent for the result is this object's exponent
     * minus the divisor's exponent.
     * @param divisor The divisor.
     * @param ctx A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true, will
     * also store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The quotient of the two objects. Returns null if the return
     * value would overflow the exponent range. A caller can handle a null
     * return value by treating it as positive infinity if both operands
     * have the same sign or as negative infinity if both operands have different
     * signs.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException Either ctx is null or ctx's precision
     * is 0, and the result would have a nonterminating decimal expansion;
     * or, the rounding mode is Rounding.Unnecessary and the result is not
     * exact.
     */
    public DecimalFraction Divide(
          DecimalFraction divisor,
          PrecisionContext ctx
        ) {
      return math.Divide(this, divisor, ctx);
    }

    /**
     * Divides two DecimalFraction objects.
     * @param divisor A DecimalFraction object.
     * @param exponent A BigInteger object.
     * @param ctx A PrecisionContext object.
     * @return The quotient of the two objects.
     */
    public DecimalFraction Divide(
          DecimalFraction divisor, BigInteger exponent, PrecisionContext ctx) {
      return math.Divide(this, divisor, exponent, ctx);
    }

    /**
     * Gets the greater value between two decimal fractions.
     * @param a A DecimalFraction object.
     * @param b A DecimalFraction object.
     * @return The larger value of the two objects.
     */
    public static DecimalFraction Max(
       DecimalFraction a, DecimalFraction b) {
      return math.Max(a, b);
    }

    /**
     * Gets the lesser value between two decimal fractions.
     * @param a A DecimalFraction object.
     * @param b A DecimalFraction object.
     * @return The smaller value of the two objects.
     */
    public static DecimalFraction Min(
       DecimalFraction a, DecimalFraction b) {
      return math.Min(a, b);
    }
    /**
     * Gets the lesser value between two values, ignoring their signs. If
     * the absolute values are equal, has the same effect as Min.
     * @param a A DecimalFraction object.
     * @param b A DecimalFraction object.
     */
    public static DecimalFraction MaxMagnitude(
       DecimalFraction a, DecimalFraction b) {
      return math.MaxMagnitude(a, b);
    }
    public static DecimalFraction MinMagnitude(
       DecimalFraction a, DecimalFraction b) {
      return math.MinMagnitude(a, b);
    }
    /**
     * Compares the mathematical values of this object and another object.
     * <p> This method is not consistent with the Equals method because two
     * different decimal fractions with the same mathematical value, but
     * different exponents, will compare as equal.</p>
     * @param op A DecimalFraction object.
     * @return Less than 0 if this object's value is less than the other value,
     * or greater than 0 if this object's value is greater than the other value
     * or if "other" is null, or 0 if both values are equal.
     */
    public int compareTo(
           DecimalFraction op) {
      return math.compareTo(this, op);
    }
    /**
     * Finds the sum of this object and another object. The result's exponent
     * is set to the lower of the exponents of the two operands.
     * @param decfrac The number to add to.
     * @param ctx A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true, will
     * also store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @param op A DecimalFraction object.
     * @return The sum of thisValue and the other object. Returns null if
     * the result would overflow the exponent range.
     */
    public DecimalFraction Add(
           DecimalFraction op, PrecisionContext ctx) {
      return math.Add(this, op, ctx);
    }
    /**
     * Returns a decimal fraction with the same value but a new exponent.
     * @param otherValue A decimal fraction containing the desired exponent
     * of the result. The mantissa is ignored.
     * @param ctx A PrecisionContext object.
     * @param op A DecimalFraction object.
     * @return A decimal fraction with the same value as this object but with
     * the exponent changed.
     */
    public DecimalFraction Quantize(
           DecimalFraction op, PrecisionContext ctx) {
      return math.Quantize(this, op, ctx);
    }
    /**
     * 
     * @param ctx A PrecisionContext object.
     */
    public DecimalFraction RoundToIntegralExact(
          PrecisionContext ctx) {
      return math.RoundToIntegralExact(this, ctx);
    }
    /**
     * 
     * @param ctx A PrecisionContext object.
     */
public DecimalFraction RoundToIntegralValue(
          PrecisionContext ctx) {
      return math.RoundToIntegralValue(this, ctx);
    }


    /**
     * Multiplies two decimal fractions. The resulting scale will be the
     * sum of the scales of the two decimal fractions.
     * @param op Another decimal fraction.
     * @param ctx A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true, will
     * also store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The product of the two decimal fractions. If a precision context
     * is given, returns null if the result of rounding would cause an overflow.
     * The caller can handle a null return value by treating it as negative
     * infinity if this value and the other value have different signs, or
     * as positive infinity if this value and the other value have the same
     * sign.
     */
    public DecimalFraction Multiply(
           DecimalFraction op, PrecisionContext ctx) {
      return math.Multiply(this, op, ctx);
    }

    /**
     * Multiplies by one value, and then adds another value.
     * @param op The value to multiply.
     * @param augend The value to add.
     * @param ctx A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true, will
     * also store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The result thisValue * multiplicand + augend. If a precision
     * context is given, returns null if the result of rounding would cause
     * an overflow. The caller can handle a null return value by treating
     * it as negative infinity if this value and the other value have different
     * signs, or as positive infinity if this value and the other value have
     * the same sign.
     */
    public DecimalFraction MultiplyAndAdd(
       DecimalFraction op, DecimalFraction augend, PrecisionContext ctx) {
      return math.MultiplyAndAdd(this, op, augend, ctx);
    }
    /**
     * Rounds this object's value to a given precision, using the given rounding
     * mode and range of exponent.
     * @param ctx A context for controlling the precision, rounding mode,
     * and exponent range. Can be null.
     * @return The closest value to this object's value, rounded to the specified
     * precision. Returns the same value as this object if "context" is null
     * or the precision and exponent range are unlimited. Returns null if
     * the result of the rounding would cause an overflow. The caller can
     * handle a null return value by treating it as positive or negative infinity
     * depending on the sign of this object's value.
     */
    public DecimalFraction RoundToPrecision(
       PrecisionContext ctx) {
      return math.RoundToPrecision(this, ctx);
    }
  }
