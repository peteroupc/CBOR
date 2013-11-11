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
   * Consists of a integer mantissa and an integer exponent, both arbitrary-precision.
   * The value of the number is equal to mantissa * 10^exponent. <p> Note:
   * This class doesn't yet implement certain operations, notably division,
   * that require results to be rounded. That's because I haven't decided
   * yet how to incorporate rounding into the API, since the results of
   * some divisions can't be represented exactly in a decimal fraction
   * (for example, 1/3). Should I include precision and rounding mode,
   * as is done in Java's Big Decimal class, or should I also include minimum
   * and maximum exponent in the rounding parameters, for better support
   * when converting to other decimal number formats? Or is there a better
   * approach to supporting rounding? </p>
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
     * Creates a decimal fraction from a string that represents a number.
     * <p> The format of the string generally consists of:<ul> <li> An optional
     * '-' or '+' character (if '-', the value is negative.)</li> <li> One
     * or more digits, with a single optional decimal point after the first
     * digit and before the last digit.</li> <li> Optionally, E+ (positive
     * exponent) or E- (negative exponent) plus one or more digits specifying
     * the exponent.</li> </ul> </p> <p>The format generally follows the
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
      BigInteger bigint = BigInteger.ZERO;
      boolean haveDecimalPoint = false;
      boolean haveDigits = false;
      boolean haveExponent = false;
      BigInteger newScale = BigInteger.ZERO;
      int i = offset;
      for (; i < s.length(); i++) {
        if (s.charAt(i) >= '0' && s.charAt(i) <= '9') {
          bigint=bigint.multiply(BigInteger.TEN);
          int thisdigit = (int)(s.charAt(i) - '0');
          bigint=bigint.add(BigInteger.valueOf(((long)thisdigit)));
          haveDigits = true;
          if (haveDecimalPoint) {
            newScale=newScale.subtract(BigInteger.ONE);
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
        BigInteger exponent = BigInteger.ZERO;
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
            exponent=exponent.multiply(BigInteger.TEN);
            int thisdigit = (int)(s.charAt(i) - '0') * offset;
            exponent=exponent.add(BigInteger.valueOf(((long)thisdigit)));
          } else {
            throw new NumberFormatException();
          }
        }
        if (!haveDigits)
          throw new NumberFormatException();
        newScale=newScale.add(exponent);
      } else if (i != s.length()) {
        throw new NumberFormatException();
      }
      if (negative)
        bigint=(bigint).negate();
      return new DecimalFraction(bigint, newScale);
    }

    private BigInteger
      RescaleByExponentDiff(BigInteger mantissa,
                            BigInteger e1,
                            BigInteger e2) {
      boolean negative = (mantissa.signum() < 0);
      if (negative) mantissa=mantissa.negate();
      BigInteger diff=e1.subtract(e2);
      diff=(diff).abs();
      while(diff.compareTo(BigInt5)>0){
        mantissa=mantissa.multiply(BigInteger.valueOf(100000));
        diff=diff.subtract(BigInt5);
      }
      while(diff.signum()>0){
        mantissa=mantissa.multiply(BigInteger.TEN);
        diff=diff.subtract(BigInteger.ONE);
      }
      if (negative) mantissa=mantissa.negate();
      return mantissa;
    }

    /**
     * Gets an object with the same ((value instanceof this one) ? (this one)value
     * : null), but with the sign reversed.
     */
    public DecimalFraction Negate() {
      BigInteger neg=(this.mantissa).negate();
      return new DecimalFraction(neg, this.exponent);
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
     * Gets the greater value between two DecimalFraction values.
     */
    public DecimalFraction Max(DecimalFraction a, DecimalFraction b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      return a.compareTo(b) > 0 ? a : b;
    }

    /**
     * Gets the lesser value between two DecimalFraction values.
     */
    public DecimalFraction Min(DecimalFraction a, DecimalFraction b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      return a.compareTo(b) > 0 ? b : a;
    }

    /**
     * Finds the sum of this object and another decimal fraction. The result's
     * exponent is set to the lower of the exponents of the two operands.
     */
    public DecimalFraction Add(DecimalFraction decfrac) {
      int expcmp = exponent.compareTo(decfrac.exponent);
      if (expcmp == 0) {
        return new DecimalFraction(
          mantissa.add(decfrac.mantissa), exponent);
      } else if (expcmp > 0) {
        BigInteger newmant = RescaleByExponentDiff(
          mantissa, exponent, decfrac.exponent);
        return new DecimalFraction(
          newmant.add(decfrac.mantissa), decfrac.exponent);
      } else {
        BigInteger newmant = RescaleByExponentDiff(
          decfrac.mantissa, exponent, decfrac.exponent);
        return new DecimalFraction(
          newmant.add(this.mantissa), exponent);
      }
    }

    /**
     * Finds the difference between this object and another decimal fraction.
     * The result's exponent is set to the lower of the exponents of the two
     * operands.
     */
    public DecimalFraction Subtract(DecimalFraction decfrac) {
      int expcmp = exponent.compareTo(decfrac.exponent);
      if (expcmp == 0) {
        return new DecimalFraction(
          mantissa.subtract(decfrac.mantissa), exponent);
      } else if (expcmp > 0) {
        BigInteger newmant = RescaleByExponentDiff(
          mantissa, exponent, decfrac.exponent);
        return new DecimalFraction(
          newmant.subtract(decfrac.mantissa), decfrac.exponent);
      } else {
        BigInteger newmant = RescaleByExponentDiff(
          decfrac.mantissa, exponent, decfrac.exponent);
        return new DecimalFraction(
          this.mantissa.subtract(newmant), exponent);
      }
    }

    /**
     * Multiplies two decimal fractions. The resulting scale will be the
     * sum of the scales of the two decimal fractions.
     * @param decfrac Another decimal fraction.
     * @return The product of the two decimal fractions.
     */
    public DecimalFraction Multiply(DecimalFraction decfrac) {
      BigInteger newexp = (this.exponent.add(decfrac.exponent));
      return new DecimalFraction(mantissa.multiply(decfrac.mantissa), newexp);
    }

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
     * Compares the mathematical values of two decimal fractions. <p>This
     * method is not consistent with the Equals method because two different
     * decimal fractions with the same mathematical value, but different
     * exponents, will compare as equal.</p>
     * @param other Another decimal fraction.
     * @return Less than 0 if this value is less than the other value, or greater
     * than 0 if this value is greater than the other value or if "other" is
     * null, or 0 if both values are equal.
     */
    public int compareTo(DecimalFraction decfrac) {
      if (decfrac == null) return 1;
      int s = this.signum();
      int ds = decfrac.signum();
      if (s != ds) return (s < ds) ? -1 : 1;
      return this.Subtract(decfrac).signum();
    }

    private boolean InsertString(StringBuilder builder, BigInteger index, char c) {
      if (index.compareTo(BigInteger.valueOf(Integer.MAX_VALUE)) > 0) {
        throw new UnsupportedOperationException();
      }
      int iindex = index.intValue();
      builder.insert(iindex, c);
      return true;
    }

    private boolean InsertString(StringBuilder builder, BigInteger index, char c, BigInteger count) {
      if (count.compareTo(BigInteger.valueOf(Integer.MAX_VALUE)) > 0) {
        throw new UnsupportedOperationException();
      }
      if (index.compareTo(BigInteger.valueOf(Integer.MAX_VALUE)) > 0) {
        throw new UnsupportedOperationException();
      }
      int icount = count.intValue();
      int iindex = index.intValue();
      for (int i = icount - 1; i >= 0; i--) {
        builder.insert(iindex, c);
      }
      return true;
    }

    private boolean AppendString(StringBuilder builder, char c, BigInteger count) {
      if (count.compareTo(BigInteger.valueOf(Integer.MAX_VALUE)) > 0) {
        throw new UnsupportedOperationException();
      }
      int icount = count.intValue();
      for (int i = icount - 1; i >= 0; i--) {
        builder.append(c);
      }
      return true;
    }

    private boolean InsertString(StringBuilder builder, BigInteger index, String c) {
      if (index.compareTo(BigInteger.valueOf(Integer.MAX_VALUE)) > 0) {
        throw new UnsupportedOperationException();
      }
      int iindex = index.intValue();
      builder.insert(iindex, c);
      return true;
    }

    private String ToStringInternal(int mode) {
      // Using Java's rules for converting DecimalFraction
      // values to a String
      StringBuilder builder = new StringBuilder();
      builder.append(this.mantissa.toString());
      BigInteger adjustedExponent = this.exponent;
      BigInteger scale=(this.exponent).negate();
      BigInteger sbLength = BigInteger.valueOf((builder.length()));
      BigInteger negaPos = BigInteger.ZERO;
      if (builder.charAt(0) == '-') {
        sbLength = sbLength.subtract(BigInteger.ONE);
        negaPos = BigInteger.ONE;
      }
      boolean iszero = (this.mantissa.signum()==0);
      if (mode == 2 && iszero && scale.signum()<0) {
        // special case for zero in plain
        return builder.toString();
      }
      adjustedExponent = adjustedExponent.add(sbLength);
      adjustedExponent = adjustedExponent.subtract(BigInteger.ONE);
      BigInteger decimalPointAdjust = BigInteger.ONE;
      BigInteger threshold = BigInteger.valueOf((-6));
      if (mode == 1) { // engineering String adjustments
        BigInteger newExponent = adjustedExponent;
        boolean adjExponentNegative = (adjustedExponent.signum() < 0);
        BigInteger phase = (adjustedExponent).abs().remainder(BigInteger.valueOf(3));
        int intphase = phase.intValue();
        if (iszero && (adjustedExponent.compareTo(threshold) < 0 ||
                       scale.signum() < 0)) {
          if (intphase == 1) {
            if (adjExponentNegative) {
              decimalPointAdjust=decimalPointAdjust.add(BigInteger.ONE);
              newExponent=newExponent.add(BigInteger.ONE);
            } else {
              decimalPointAdjust=decimalPointAdjust.add(BigInteger.valueOf(2));
              newExponent=newExponent.add(BigInteger.valueOf(2));
            }
          } else if (intphase == 2) {
            if (!adjExponentNegative) {
              decimalPointAdjust=decimalPointAdjust.add(BigInteger.ONE);
              newExponent=newExponent.add(BigInteger.ONE);
            } else {
              decimalPointAdjust=decimalPointAdjust.add(BigInteger.valueOf(2));
              newExponent=newExponent.add(BigInteger.valueOf(2));
            }
          }
          threshold=threshold.add(BigInteger.ONE);
        } else {
          if (intphase == 1) {
            if (!adjExponentNegative) {
              decimalPointAdjust=decimalPointAdjust.add(BigInteger.ONE);
              newExponent=newExponent.subtract(BigInteger.ONE);
            } else {
              decimalPointAdjust=decimalPointAdjust.add(BigInteger.valueOf(2));
              newExponent=newExponent.subtract(BigInteger.valueOf(2));
            }
          } else if (intphase == 2) {
            if (adjExponentNegative) {
              decimalPointAdjust=decimalPointAdjust.add(BigInteger.ONE);
              newExponent=newExponent.subtract(BigInteger.ONE);
            } else {
              decimalPointAdjust=decimalPointAdjust.add(BigInteger.valueOf(2));
              newExponent=newExponent.subtract(BigInteger.valueOf(2));
            }
          }
        }
        adjustedExponent = newExponent;
      }
      if (mode == 2 || ((adjustedExponent.compareTo(threshold) >= 0 &&
                         scale.signum() >= 0))) {
        BigInteger decimalPoint=(scale).negate();
        decimalPoint=decimalPoint.add(negaPos);
        decimalPoint=decimalPoint.add(sbLength);
        if (scale.signum() > 0) {
          int cmp = decimalPoint.compareTo(negaPos);
          if (cmp < 0) {
            InsertString(builder, negaPos, '0', (negaPos.subtract(decimalPoint)));
            InsertString(builder, negaPos, "0.");
          } else if (cmp == 0) {
            InsertString(builder, decimalPoint, "0.");
          } else if (decimalPoint.compareTo(
            (BigInteger.valueOf((builder.length())).add(negaPos))) > 0) {
            InsertString(builder, (sbLength.add(negaPos)), '.');
            InsertString(builder, (sbLength.add(negaPos)), '0',
                         (decimalPoint.subtract(BigInteger.valueOf((builder.length())))));
          } else {
            InsertString(builder, decimalPoint, '.');
          }
        }
        if (mode == 2 && scale.signum() < 0) {
          BigInteger negscale=(scale).negate();
          AppendString(builder, '0', negscale);
        }
        return builder.toString();
      } else {
        if (mode == 1 && iszero && decimalPointAdjust.compareTo(BigInteger.ONE) > 0) {
          builder.append('.');
          AppendString(builder, '0', (decimalPointAdjust.subtract(BigInteger.ONE)));
        } else {
          BigInteger tmp = negaPos.add(decimalPointAdjust);
          int cmp = tmp.compareTo(BigInteger.valueOf((builder.length())));
          if (cmp > 0) {
            AppendString(builder, '0', (tmp.subtract(BigInteger.valueOf((builder.length())))));
          }
          if (cmp < 0) {
            InsertString(builder, tmp, '.');
          }
        }
        if (adjustedExponent.signum()!=0) {
          builder.append('E');
          builder.append(adjustedExponent.signum()<0 ? '-' : '+');
          BigInteger sbPos = BigInteger.valueOf((builder.length()));
          adjustedExponent = (adjustedExponent).abs();
          while (adjustedExponent.signum()!=0) {
            BigInteger digit = (adjustedExponent.remainder(BigInteger.TEN));
            InsertString(builder, sbPos, (char)('0' + digit.intValue()));
            adjustedExponent=adjustedExponent.divide(BigInteger.TEN);
          }
        }
        return builder.toString();
      }
    }

    /**
     * Converts this value to an arbitrary-precision integer. Any fractional
     * part in this value will be discarded when converting to a big integer.
     */
    public BigInteger ToBigInteger() {
      if (this.getExponent().signum()==0) {
        return this.getMantissa();
      } else if (this.getExponent().signum()>0) {
        BigInteger curexp = this.getExponent();
        BigInteger bigmantissa = this.getMantissa();
        boolean neg = (bigmantissa.signum() < 0);
        if (neg) bigmantissa=bigmantissa.negate();
        while (curexp.signum()>0 && bigmantissa.signum()!=0) {
          bigmantissa=bigmantissa.multiply(BigInteger.TEN);
          curexp=curexp.subtract(BigInteger.ONE);
        }
        if (neg) bigmantissa=bigmantissa.negate();
        return bigmantissa;
      } else {
        BigInteger curexp = this.getExponent();
        BigInteger bigmantissa = this.getMantissa();
        boolean neg = (bigmantissa.signum() < 0);
        if (neg) bigmantissa=bigmantissa.negate();
        while (curexp.signum()<0 && bigmantissa.signum()!=0) {
          if (curexp.compareTo(BigIntNeg5)<=0) {
            bigmantissa=bigmantissa.divide(BigInteger.valueOf(100000));
            curexp=curexp.add(BigInt5);
          } else {
            bigmantissa=bigmantissa.divide(BigInteger.TEN);
            curexp=curexp.add(BigInteger.ONE);
          }
        }
        if (neg) bigmantissa=bigmantissa.negate();
        return bigmantissa;
      }
    }

    /**
     * Converts this value to a 32-bit floating-point number. The half-up
     * rounding mode is used.
     * @return The closest 32-bit floating-point number to this value.
     * The return value can be positive infinity or negative infinity if
     * this value exceeds the range of a 32-bit floating point number.
     */
    public float ToSingle() {
      return BigFloat.FromDecimalFraction(this).ToSingle();
    }

    /**
     * Converts this value to a 64-bit floating-point number. The half-up
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
     * @param dbl A 32-bit floating-point number.
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
        long scale = fpExponent;
        while (fpExponent < 0) {
          if (fpExponent <= -20) {
            bigmantissa=bigmantissa.multiply(BigInt5Pow20);
            fpExponent += 20;
          } else if (fpExponent <= -10) {
            bigmantissa=bigmantissa.multiply(BigInt5Pow10);
            fpExponent += 10;
          } else if (fpExponent <= -5) {
            bigmantissa=bigmantissa.multiply(BigInt5Pow5);
            fpExponent += 5;
          } else {
            bigmantissa=bigmantissa.multiply(BigInt5);
            fpExponent += 1;
          }
        }
        if (neg) bigmantissa=(bigmantissa).negate();
        return new DecimalFraction(bigmantissa, scale);
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
        long scale = fpExponent;
        while (fpExponent < 0) {
          if (fpExponent <= -20) {
            bigmantissa=bigmantissa.multiply(BigInt5Pow20);
            fpExponent += 20;
          } else if (fpExponent <= -10) {
            bigmantissa=bigmantissa.multiply(BigInt5Pow10);
            fpExponent += 10;
          } else if (fpExponent <= -5) {
            bigmantissa=bigmantissa.multiply(BigInt5Pow5);
            fpExponent += 5;
          } else {
            bigmantissa=bigmantissa.multiply(BigInt5);
            fpExponent += 1;
          }
        }
        if (neg) bigmantissa=(bigmantissa).negate();
        return new DecimalFraction(bigmantissa, scale);
      }
    }

    private static BigInteger BigInt5 = BigInteger.valueOf(5);
    private static BigInteger BigInt10 = BigInteger.TEN;
    private static BigInteger BigInt20 = BigInteger.valueOf(20);
    private static BigInteger BigIntNeg5 = BigInteger.valueOf((-5));
    private static BigInteger BigIntNeg10 = BigInteger.valueOf((-10));
    private static BigInteger BigIntNeg20 = BigInteger.valueOf((-20));
    private static BigInteger BigInt5Pow5 = BigInteger.valueOf(3125);
    private static BigInteger BigInt5Pow10 = BigInteger.valueOf(9765625);
    private static BigInteger BigInt5Pow20 = BigInteger.valueOf((95367431640625L));

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
        BigInteger curexp = bigintExp;
        BigInteger bigmantissa = bigintMant;
        boolean neg = (bigmantissa.signum() < 0);
        if (neg) bigmantissa=(bigmantissa).negate();
        while (curexp.signum()>0) {
          int shift = 64;
          if (curexp.compareTo(BigInteger.valueOf(64)) < 0) {
            shift = curexp.intValue();
          }
          bigmantissa=bigmantissa.shiftLeft(shift);
          curexp=curexp.subtract(BigInteger.valueOf(shift));
        }
        if (neg) bigmantissa=(bigmantissa).negate();
        return new DecimalFraction(bigmantissa);
      } else {
        // Fractional number
        BigInteger bigmantissa = bigintMant;
        BigInteger curexp = bigintExp;
        while (curexp.signum() < 0) {
          if (curexp.compareTo(BigIntNeg20) <= 0) {
            bigmantissa=bigmantissa.multiply(BigInt5Pow20);
            curexp=curexp.add(BigInt20);
          } else if (curexp.compareTo(BigIntNeg10) <= 0) {
            bigmantissa=bigmantissa.multiply(BigInt5Pow10);
            curexp=curexp.add(BigInt10);
          } else if (curexp.compareTo(BigIntNeg5) <= 0) {
            bigmantissa=bigmantissa.multiply(BigInt5Pow5);
            curexp=curexp.add(BigInt5);
          } else if (curexp.compareTo(BigInteger.valueOf((-4))) <= 0) {
            bigmantissa=bigmantissa.multiply(BigInteger.valueOf(625));
            curexp=curexp.add(BigInteger.valueOf(4));
          } else if (curexp.compareTo(BigInteger.valueOf((-3))) <= 0) {
            bigmantissa=bigmantissa.multiply(BigInteger.valueOf(125));
            curexp=curexp.add(BigInteger.valueOf(3));
          } else if (curexp.compareTo(BigInteger.valueOf((-2))) <= 0) {
            bigmantissa=bigmantissa.multiply(BigInteger.valueOf(25));
            curexp=curexp.add(BigInteger.valueOf(2));
          } else {
            bigmantissa=bigmantissa.multiply(BigInt5);
            curexp=curexp.add(BigInteger.ONE);
          }
        }
        return new DecimalFraction(bigmantissa, bigintExp);
      }
    }

    /*
    internal DecimalFraction MovePointLeft(BigInteger steps){
      if(steps.signum()==0)return this;
      return new DecimalFraction(this.getMantissa(),this.getExponent().subtract(steps));
    }
    
    internal DecimalFraction MovePointRight(BigInteger steps){
      if(steps.signum()==0)return this;
      return new DecimalFraction(this.getMantissa(),this.getExponent().add(steps));
    }

    internal DecimalFraction Rescale(BigInteger scale)
    {
      throw new UnsupportedOperationException();
    }
 
    internal DecimalFraction RoundToIntegralValue(BigInteger scale)
    {
      return Rescale(BigInteger.ZERO);
    }
    internal DecimalFraction Normalize()
    {
      if(this.getMantissa().signum()==0)
        return new DecimalFraction(0);
      BigInteger mant=this.getMantissa();
      BigInteger exp=this.getExponent();
      boolean changed=false;
      while((mant.remainder(BigInteger.TEN))==0){
        mant=mant.divide(BigInteger.TEN);
        exp=exp.add(BigInteger.ONE);
        changed=true;
      }
      if(!changed)return this;
      return new DecimalFraction(mant,exp);
    }
     */

    /**
     * Converts this value to a string. The format of the return value is exactly
     * the same as that of the java.math.BigDecimal.toString() method.
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
  }
