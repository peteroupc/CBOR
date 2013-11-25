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
     * Represents an arbitrary-precision binary floating-point number.
     * Consists of an integer mantissa and an integer exponent, both arbitrary-precision.
     * The value of the number is equal to mantissa * 2^exponent.
     */
  public final class BigFloat implements Comparable<BigFloat> {
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
     * @param obj A BigFloat object.
     */
    public boolean equals(BigFloat obj) {
      BigFloat other = ((obj instanceof BigFloat) ? (BigFloat)obj : null);
      if (other == null)
        return false;
      return this.exponent.equals(other.exponent) &&
        this.mantissa.equals(other.mantissa);
    }
    /**
     * Determines whether this object's mantissa and exponent are equal
     * to those of another object and that object is a Bigfloat.
     * @param obj A object object.
     * @return True if the objects are equal; false otherwise.
     */
    @Override public boolean equals(Object obj) {
      return equals(((obj instanceof BigFloat) ? (BigFloat)obj : null));
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
     * Creates a bigfloat with the value exponent*2^mantissa.
     * @param mantissa The unscaled value.
     * @param exponent The binary exponent.
     */
    public BigFloat(BigInteger mantissa, BigInteger exponent) {
      this.exponent = exponent;
      this.mantissa = mantissa;
    }
    /**
     * Creates a bigfloat with the value exponentLong*2^mantissa.
     * @param mantissa The unscaled value.
     * @param exponentLong The binary exponent.
     */
    public BigFloat(BigInteger mantissa, long exponentLong) {
      this.exponent = BigInteger.valueOf(exponentLong);
      this.mantissa = mantissa;
    }
    /**
     * Creates a bigfloat with the given mantissa and an exponent of 0.
     * @param mantissa The desired value of the bigfloat
     */
    public BigFloat(BigInteger mantissa) {
      this.exponent = BigInteger.ZERO;
      this.mantissa = mantissa;
    }
    /**
     * Creates a bigfloat with the given mantissa and an exponent of 0.
     * @param mantissaLong The desired value of the bigfloat
     */
    public BigFloat(long mantissaLong) {
      this.exponent = BigInteger.ZERO;
      this.mantissa = BigInteger.valueOf(mantissaLong);
    }
    private static BigInteger FastMaxExponent = BigInteger.valueOf(2000);
    private static BigInteger FastMinExponent = BigInteger.valueOf(-2000);
    private static BigInteger BigShiftIteration = BigInteger.valueOf(1000000);
    private static int ShiftIteration = 1000000;
    private static BigInteger ShiftLeft(BigInteger val, BigInteger bigShift) {
      while (bigShift.compareTo(BigShiftIteration) > 0) {
        val=val.shiftLeft(1000000);
        bigShift=bigShift.subtract(BigShiftIteration);
      }
      int lastshift = bigShift.intValue();
      val=val.shiftLeft(lastshift);
      return val;
    }
    private static BigInteger ShiftLeft(BigInteger val, long shift) {
      while (shift > ShiftIteration) {
        val=val.shiftLeft(1000000);
        shift -= ShiftIteration;
      }
      int lastshift = (int)shift;
      val=val.shiftLeft(lastshift);
      return val;
    }

    private static BigInteger
      RescaleByExponentDiff(BigInteger mantissa,
                            BigInteger e1,
                            BigInteger e2) {
      boolean negative = (mantissa.signum() < 0);
      if (negative) mantissa=mantissa.negate();
      if (e1.compareTo(FastMinExponent) >= 0 &&
          e1.compareTo(FastMaxExponent) <= 0 &&
          e2.compareTo(FastMinExponent) >= 0 &&
          e2.compareTo(FastMaxExponent) <= 0) {
        int e1long = e1.intValue();
        int e2long = e2.intValue();
        e1long = Math.abs(e1long - e2long);
        if (e1long != 0) {
          mantissa=mantissa.shiftLeft(e1long);
        }
      } else {
        BigInteger diff = (e1.subtract(e2)).abs();
        mantissa = ShiftLeft(mantissa, diff);
      }
      if (negative) mantissa=mantissa.negate();
      return mantissa;
    }
    /**
     * Gets an object with the same value as this one, but with the sign reversed.
     */
    public BigFloat Negate() {
      BigInteger neg=(this.mantissa).negate();
      return new BigFloat(neg, this.exponent);
    }
    /**
     * Gets the absolute value of this object.
     */
    public BigFloat Abs() {
      if (this.signum() < 0) {
        return Negate();
      } else {
        return this;
      }
    }

    private static boolean Round(ShiftAccumulator accum, Rounding rounding,
                              boolean neg, int lastDigit) {
      boolean incremented = false;
      if (rounding == Rounding.HalfUp) {
        if (accum.getBitLeftmost() != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.HalfEven) {
        if (accum.getBitLeftmost() != 0) {
          if (accum.getBitsAfterLeftmost() != 0 || lastDigit != 0) {
            incremented = true;
          }
        }
      } else if (rounding == Rounding.Ceiling) {
        if (!neg && (accum.getBitLeftmost() | accum.getBitsAfterLeftmost()) != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.Floor) {
        if (neg && (accum.getBitLeftmost() | accum.getBitsAfterLeftmost()) != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.HalfDown) {
        if (accum.getBitLeftmost() != 0 && accum.getBitsAfterLeftmost() != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.Up ||
                rounding == Rounding.ZeroFiveUp) {
        // NOTE: For binary floating point, ZeroFiveUp
        // is the same as Up except in overflow situations
        if ((accum.getBitLeftmost() | accum.getBitsAfterLeftmost()) != 0) {
          incremented = true;
        }
      }
      return incremented;
    }

    /**
     * Rounds this object's value to a given precision, using the given rounding
     * mode and range of exponent.
     * @param context A context for controlling the precision, rounding
     * mode, and exponent range.
     * @return The closest value to this object's value, rounded to the specified
     * precision. Returns null if the result of the rounding would cause
     * an overflow. The caller can handle a null return value by treating
     * it as positive or negative infinity depending on the sign of this object's
     * value.
     * @throws java.lang.IllegalArgumentException "precision" is null.
     */
    public BigFloat RoundToPrecision(
      PrecisionContext context
     ) {
      if ((context) == null) throw new NullPointerException("context");
      if (context.getPrecision() > 0 && context.getPrecision() <= 18) {
        // Check if rounding is necessary at all
        // for small precisions
        BigInteger mantabs = (this.mantissa).abs();
        if (mantabs.compareTo(ShiftLeft(BigInteger.ONE, context.getPrecision())) < 0) {
          FastInteger fastExp = new FastInteger(this.exponent)
            .Add(context.getPrecision()).Subtract(1);
          FastInteger fastNormalMin = new FastInteger(context.getEMin())
            .Add(context.getPrecision()).Subtract(1);
          if (fastExp.compareTo(new FastInteger(context.getEMax())) <= 0 &&
             fastExp.compareTo(fastNormalMin) >= 0) {
            return this;
          }
        }
      }
      int[] signals = new int[1];
      BigFloat dfrac = RoundToPrecision(
        context.getPrecision(),
        context.getRounding(),
        context.getEMin(),
        context.getEMax(), signals);
      if (context.getClampNormalExponents() && dfrac != null) {
        dfrac = dfrac.ClampExponent(context.getPrecision(), context.getEMax(), signals);
      }
      if (context.getHasFlags()) {
        context.setFlags(context.getFlags()|signals[0]);
      }
      return dfrac;
    }

    private BigFloat ClampExponent(long precision, BigInteger eMax, int[] signals) {
      if (signals != null && signals.length == 0)
        throw new IllegalArgumentException("signals has zero length");
      FastInteger exp = new FastInteger(this.exponent);
      FastInteger clamp = new FastInteger(eMax).Add(1).Subtract(precision);
      if (exp.compareTo(clamp) > 0) {
        BigInteger bigmantissa = this.mantissa;
        int sign = bigmantissa.signum();
        if (sign != 0) {
          if (sign < 0) bigmantissa=bigmantissa.negate();
          FastInteger expdiff = new FastInteger(exp).Subtract(clamp);
          if (expdiff.CanFitInInt64()) {
            bigmantissa = (ShiftLeft(bigmantissa, expdiff.AsInt64()));
          } else {
            bigmantissa = (ShiftLeft(bigmantissa, expdiff.AsBigInteger()));
          }
          if (sign < 0) bigmantissa=bigmantissa.negate();
        }
        if (signals != null)
          signals[0] |= PrecisionContext.SignalClamped;
        return new BigFloat(bigmantissa, clamp.AsBigInteger());
      }
      return this;
    }

    private BigFloat RoundToPrecision(
      long precision,
      Rounding rounding,
      BigInteger eMin,
      BigInteger eMax,
      int[] signals
     ) {
      if ((precision) < 0) throw new IllegalArgumentException("precision" + " not greater or equal to 1 (" + Long.toString((long)(precision)) + ")");
      if (eMin.compareTo(eMax) > 0) throw new IllegalArgumentException("eMin greater than eMax");
      if (signals != null && signals.length == 0)
        throw new IllegalArgumentException("signals has zero length");
      boolean neg = this.mantissa.signum() < 0;
      int lastDigit = 0;
      BigInteger bigmantissa = this.mantissa;
      if (neg) bigmantissa=bigmantissa.negate();
      // save mantissa in case result is subnormal
      // and must be rounded again
      BigInteger oldmantissa = bigmantissa;
      ShiftAccumulator accum = new ShiftAccumulator(bigmantissa);
      FastInteger exp = new FastInteger(this.exponent);
      FastInteger fastEMin = new FastInteger(eMin);
      FastInteger fastEMax = new FastInteger(eMax);
      int flags = 0;
      boolean unlimitedPrec = (precision == 0);
      if (precision == 0) {
        precision = accum.getKnownBitLength();
      } else {
        accum.ShiftToBits(precision);
      }
      FastInteger discardedBits = new FastInteger(accum.getDiscardedBitCount());
      exp.Add(discardedBits);
      FastInteger adjExponent = new FastInteger(exp);
      adjExponent.Add(accum.getKnownBitLength()).Subtract(1);
      FastInteger clamp = null;
      if (adjExponent.compareTo(fastEMax) > 0) {
        if (oldmantissa.signum()==0) {
          flags |= PrecisionContext.SignalClamped;
          if (signals != null) signals[0] = flags;
          return new BigFloat(oldmantissa, fastEMax.AsBigInteger());
        }
        // Overflow
        flags |= PrecisionContext.SignalOverflow | PrecisionContext.SignalInexact | PrecisionContext.SignalRounded;
        if (!unlimitedPrec &&
           (rounding == Rounding.Down ||
           rounding == Rounding.ZeroFiveUp ||
           (rounding == Rounding.Ceiling && neg) ||
           (rounding == Rounding.Floor && !neg))) {
          // Set to the highest possible value for
          // the given precision
          BigInteger overflowMant = BigInteger.ONE;
          overflowMant = ShiftLeft(overflowMant, precision);
          overflowMant=overflowMant.subtract(BigInteger.ONE);
          if (neg) overflowMant=overflowMant.negate();
          if (signals != null) signals[0] = flags;
          clamp = new FastInteger(fastEMax).Add(1).Subtract(precision);
          return new BigFloat(overflowMant, clamp.AsBigInteger());
        }
        if (signals != null) signals[0] = flags;
        return null;
      } else if (adjExponent.compareTo(fastEMin) < 0) {
        // Subnormal
        FastInteger fastETiny = new FastInteger(fastEMin)
          .Subtract(precision)
          .Add(1);
        if (oldmantissa.signum()!=0)
          flags |= PrecisionContext.SignalSubnormal;
        if (exp.compareTo(fastETiny) < 0) {
          FastInteger expdiff = new FastInteger(fastETiny).Subtract(exp);
          expdiff.Add(discardedBits);
          accum = new ShiftAccumulator(oldmantissa);
          accum.ShiftRight(expdiff);
          BigInteger newmantissa = accum.getShiftedInt();
          if ((accum.getDiscardedBitCount()).signum() != 0) {
            if (oldmantissa.signum()!=0)
              flags |= PrecisionContext.SignalRounded;
            lastDigit = 0;
            if (rounding == Rounding.HalfEven ||
               rounding == Rounding.ZeroFiveUp) {
              lastDigit = (int)(newmantissa.testBit(0)==false ? 0 : 1);
            }
            if ((accum.getBitLeftmost() | accum.getBitsAfterLeftmost()) != 0) {
              flags |= PrecisionContext.SignalInexact;
            }
            if (Round(accum, rounding, neg, lastDigit)) {
              newmantissa=newmantissa.add(BigInteger.ONE);
            }
          }
          if (newmantissa.signum()==0)
            flags |= PrecisionContext.SignalClamped;
          if ((flags & (PrecisionContext.SignalSubnormal | PrecisionContext.SignalInexact)) == (PrecisionContext.SignalSubnormal | PrecisionContext.SignalInexact))
            flags |= PrecisionContext.SignalUnderflow | PrecisionContext.SignalRounded;
          if (signals != null) signals[0] = flags;
          if (neg) newmantissa=newmantissa.negate();
          return new BigFloat(newmantissa, fastETiny.AsBigInteger());
        }
      }
      boolean expChanged = false;
      lastDigit = 0;
      if ((accum.getDiscardedBitCount()).signum() != 0) {
        if (bigmantissa.signum()!=0)
          flags |= PrecisionContext.SignalRounded;
        bigmantissa = accum.getShiftedInt();
        if ((accum.getBitLeftmost() | accum.getBitsAfterLeftmost()) != 0) {
          flags |= PrecisionContext.SignalInexact;
        }
      }
      if (rounding == Rounding.HalfEven ||
         rounding == Rounding.ZeroFiveUp) {
        lastDigit = (int)(bigmantissa.testBit(0)==false ? 0 : 1);
      }
      if (Round(accum, rounding, neg, lastDigit)) {
        bigmantissa=bigmantissa.add(BigInteger.ONE);
        accum = new ShiftAccumulator(bigmantissa);
        accum.ShiftToBits(precision);
        if ((accum.getDiscardedBitCount()).signum() != 0) {
          exp.Add(accum.getDiscardedBitCount());
          discardedBits.Add(accum.getDiscardedBitCount());
          bigmantissa = accum.getShiftedInt();
          expChanged = true;
        }
      }
      if (expChanged) {
        // If exponent changed, check for overflow again
        adjExponent = new FastInteger(exp);
        adjExponent.Add(accum.getKnownBitLength()).Subtract(1);
        if (adjExponent.compareTo(fastEMax) > 0) {
          flags |= PrecisionContext.SignalOverflow | PrecisionContext.SignalInexact | PrecisionContext.SignalRounded;
          if (!unlimitedPrec &&
             (rounding == Rounding.Down ||
             rounding == Rounding.ZeroFiveUp ||
             (rounding == Rounding.Ceiling && neg) ||
             (rounding == Rounding.Floor && !neg))) {
            // Set to the highest possible value for
            // the given precision
            BigInteger overflowMant = BigInteger.ONE;
            while (precision > 0) {
              int tmp = (int)Math.min(9999999, precision);
              overflowMant=overflowMant.shiftLeft(tmp);
              precision -= tmp;
            }
            overflowMant=overflowMant.subtract(BigInteger.ONE);
            if (neg) overflowMant=overflowMant.negate();
            if (signals != null) signals[0] = flags;
            clamp = new FastInteger(fastEMax).Add(1).Subtract(precision);
            return new BigFloat(overflowMant, clamp.AsBigInteger());
          }
          if (signals != null) signals[0] = flags;
          return null;
        }
      }
      if (signals != null) signals[0] = flags;
      if (neg) bigmantissa=bigmantissa.negate();
      return new BigFloat(bigmantissa, exp.AsBigInteger());
    }
    /**
     * Gets the greater value between two BigFloat values. If both values
     * are equal, returns "a".
     * @param a A BigFloat object.
     * @param b A BigFloat object.
     * @return The larger value of the two objects.
     */
    public static BigFloat Max(BigFloat a, BigFloat b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      int cmp = a.compareTo(b);
      if (cmp != 0)
        return cmp > 0 ? a : b;
      // Here the signs of both a and b can only be
      // equal (negative zeros are not supported)
      if (a.signum() >= 0) {
        return (a.getExponent()).compareTo(b.getExponent()) > 0 ? a : b;
      } else {
        return (a.getExponent()).compareTo(b.getExponent()) > 0 ? b : a;
      }
    }
    /**
     * Gets the lesser value between two BigFloat values. If both values
     * are equal, returns "a".
     * @param a A BigFloat object.
     * @param b A BigFloat object.
     * @return The smaller value of the two objects.
     */
    public static BigFloat Min(BigFloat a, BigFloat b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      int cmp = a.compareTo(b);
      if (cmp != 0)
        return cmp > 0 ? b : a;
      // Here the signs of both a and b can only be
      // equal (negative zeros are not supported)
      if (a.signum() >= 0) {
        return (a.getExponent()).compareTo(b.getExponent()) > 0 ? b : a;
      } else {
        return (a.getExponent()).compareTo(b.getExponent()) > 0 ? a : b;
      }
    }

    /**
     * Gets the lesser value between two values, ignoring their signs. If
     * the absolute values are equal, has the same effect as Min.
     * @param a A BigFloat object.
     * @param b A BigFloat object.
     */
    public static BigFloat MinMagnitude(BigFloat a, BigFloat b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      int cmp = a.Abs().compareTo(b.Abs());
      if (cmp == 0) return Min(a, b);
      return (cmp < 0) ? a : b;
    }
    /**
     * Gets the greater value between two values, ignoring their signs.
     * If the absolute values are equal, has the same effect as Max.
     * @param a A BigFloat object.
     * @param b A BigFloat object.
     */
    public static BigFloat MaxMagnitude(BigFloat a, BigFloat b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      int cmp = a.Abs().compareTo(b.Abs());
      if (cmp == 0) return Max(a, b);
      return (cmp > 0) ? a : b;
    }
    /**
     * Finds the sum of this object and another bigfloat. The result's exponent
     * is set to the lower of the exponents of the two operands.
     * @param decfrac A BigFloat object.
     */
    public BigFloat Add(BigFloat decfrac) {
      int expcmp = exponent.compareTo(decfrac.exponent);
      if (expcmp == 0) {
        return new BigFloat(
          mantissa.add(decfrac.mantissa), exponent);
      } else if (expcmp > 0) {
        BigInteger newmant = RescaleByExponentDiff(
          mantissa, exponent, decfrac.exponent);
        return new BigFloat(
          newmant.add(decfrac.mantissa), decfrac.exponent);
      } else {
        BigInteger newmant = RescaleByExponentDiff(
          decfrac.mantissa, exponent, decfrac.exponent);
        return new BigFloat(
          newmant.add(this.mantissa), exponent);
      }
    }
    /**
     * Finds the difference between this object and another bigfloat. The
     * result's exponent is set to the lower of the exponents of the two operands.
     * @param decfrac A BigFloat object.
     * @return The difference of the two objects.
     */
    public BigFloat Subtract(BigFloat decfrac) {
      int expcmp = exponent.compareTo(decfrac.exponent);
      if (expcmp == 0) {
        return new BigFloat(
          this.mantissa.subtract(decfrac.mantissa), exponent);
      } else if (expcmp > 0) {
        BigInteger newmant = RescaleByExponentDiff(
          mantissa, exponent, decfrac.exponent);
        return new BigFloat(
          newmant.subtract(decfrac.mantissa), decfrac.exponent);
      } else {
        BigInteger newmant = RescaleByExponentDiff(
          decfrac.mantissa, exponent, decfrac.exponent);
        return new BigFloat(
          this.mantissa.subtract(newmant), exponent);
      }
    }
    /**
     * Multiplies two bigfloats. The resulting scale will be the sum of the
     * scales of the two bigfloats.
     * @param decfrac Another bigfloat.
     * @return The product of the two bigfloats.
     */
    public BigFloat Multiply(BigFloat decfrac) {
      BigInteger newexp = (this.exponent.add(decfrac.exponent));
      return new BigFloat(
        mantissa.multiply(decfrac.mantissa), newexp);
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
     * Compares the mathematical values of two bigfloats. <p> This method
     * is not consistent with the Equals method because two different bigfloats
     * with the same mathematical value, but different exponents, will
     * compare as equal.</p>
     * @param other Another bigfloat.
     * @return Less than 0 if this value is less than the other value, or greater
     * than 0 if this value is greater than the other value or if "other" is
     * null, or 0 if both values are equal.
     */
    public int compareTo(BigFloat other) {
      if (other == null) return 1;
      int s = this.signum();
      int ds = other.signum();
      if (s != ds) return (s < ds) ? -1 : 1;
      int expcmp = exponent.compareTo((BigInteger)other.exponent);
      int mantcmp = mantissa.compareTo((BigInteger)other.mantissa);
      if (mantcmp == 0) {
        // Special case: Mantissas are equal
        return s == 0 ? 0 : expcmp * s;
      }
      if (expcmp == 0) {
        return mantcmp;
      } else if (expcmp > 0) {
        BigInteger newmant = RescaleByExponentDiff(
          mantissa, exponent, other.exponent);
        return newmant.compareTo((BigInteger)other.mantissa);
      } else {
        BigInteger newmant = RescaleByExponentDiff(
          other.mantissa, exponent, other.exponent);
        return this.mantissa.compareTo(newmant);
      }
    }

    /**
     * Creates a bigfloat from an arbitrary-precision decimal fraction.
     * Note that if the decimal fraction contains a negative exponent, the
     * resulting value might not be exact.
     * @param decfrac A DecimalFraction object.
     */
    public static BigFloat FromDecimalFraction(DecimalFraction decfrac) {
      BigInteger bigintExp = decfrac.getExponent();
      BigInteger bigintMant = decfrac.getMantissa();
      if (bigintMant.signum()==0)
        return new BigFloat(0);
      if (bigintExp.signum()==0) {
        // Integer
        return new BigFloat(bigintMant);
      } else if (bigintExp.signum() > 0) {
        // Scaled integer
        BigInteger bigmantissa = bigintMant;
        bigmantissa=bigmantissa.multiply(DecimalFraction.FindPowerOfTenFromBig(bigintExp));
        return new BigFloat(bigmantissa);
      } else {
        // Fractional number
        FastInteger scale = new FastInteger(bigintExp);
        BigInteger bigmantissa = bigintMant;
        boolean neg = (bigmantissa.signum() < 0);
        BigInteger remainder;
        if (neg) bigmantissa=(bigmantissa).negate();
        FastInteger negscale = new FastInteger(scale).Negate();
        BigInteger divisor = DecimalFraction.FindPowerOfFiveFromBig(
          negscale.AsBigInteger());
        while (true) {
          BigInteger quotient;
BigInteger[] divrem=(bigmantissa).divideAndRemainder(divisor);
quotient=divrem[0];
remainder=divrem[1];
          // Ensure that the quotient has enough precision
          // to be converted accurately to a single or double
          if (remainder.signum()!=0 &&
             quotient.compareTo(OneShift62) < 0) {
            long smallquot = quotient.longValue();
            int shift = 0;
            while (smallquot != 0 && smallquot < (1L << 62)) {
              smallquot <<= 1;
              shift++;
            }
            if (shift == 0) shift = 1;
            bigmantissa=bigmantissa.shiftLeft(shift);
            scale.Add(-shift);
          } else {
            bigmantissa = quotient;
            break;
          }
        }
        // Round half-even
        BigInteger halfDivisor = divisor;
        halfDivisor=halfDivisor.shiftRight(1);
        int cmp = remainder.compareTo(halfDivisor);
        // No need to check for exactly half since all powers
        // of five are odd
        if (cmp > 0) {
          // Greater than half
          bigmantissa=bigmantissa.add(BigInteger.ONE);
        }
        if (neg) bigmantissa=(bigmantissa).negate();
        return new BigFloat(bigmantissa, scale.AsBigInteger());
      }
    }
    /**
     * Creates a bigfloat from a 32-bit floating-point number.
     * @param flt A 32-bit floating-point number.
     * @return A bigfloat with the same value as "flt".
     * @throws ArithmeticException "flt" is infinity or not-a-number.
     */
    public static BigFloat FromSingle(float flt) {
      int value = Float.floatToRawIntBits(flt);
      int fpExponent = (int)((value >> 23) & 0xFF);
      if (fpExponent == 255)
        throw new ArithmeticException("Value is infinity or NaN");
      int fpMantissa = value & 0x7FFFFF;
      if (fpExponent == 0) fpExponent++;
      else fpMantissa |= (1 << 23);
      if (fpMantissa != 0) {
        while ((fpMantissa & 1) == 0) {
          fpExponent++;
          fpMantissa >>= 1;
        }
        if ((value >> 31) != 0)
          fpMantissa = -fpMantissa;
      }
      return new BigFloat(BigInteger.valueOf((long)fpMantissa), fpExponent - 150);
    }
    /**
     * Creates a bigfloat from a 64-bit floating-point number.
     * @param dbl A 64-bit floating-point number.
     * @return A bigfloat with the same value as "dbl"
     * @throws ArithmeticException "dbl" is infinity or not-a-number.
     */
    public static BigFloat FromDouble(double dbl) {
      long value = Double.doubleToRawLongBits(dbl);
      int fpExponent = (int)((value >> 52) & 0x7ffL);
      if (fpExponent == 2047)
        throw new ArithmeticException("Value is infinity or NaN");
      long fpMantissa = value & 0xFFFFFFFFFFFFFL;
      if (fpExponent == 0) fpExponent++;
      else fpMantissa |= (1L << 52);
      if (fpMantissa != 0) {
        while ((fpMantissa & 1) == 0) {
          fpExponent++;
          fpMantissa >>= 1;
        }
        if ((value >> 63) != 0)
          fpMantissa = -fpMantissa;
      }
      return new BigFloat(BigInteger.valueOf((long)fpMantissa), fpExponent - 1075);
    }
    /**
     * Converts this value to an arbitrary-precision integer. Any fractional
     * part in this value will be discarded when converting to a big integer.
     */
    public BigInteger ToBigInteger() {
      if (this.getExponent().signum()==0) {
        // Integer
        return this.getMantissa();
      } else if (this.getExponent().signum()>0) {
        // Integer with trailing zeros
        BigInteger curexp = this.getExponent();
        BigInteger bigmantissa = this.getMantissa();
        if (bigmantissa.signum()==0)
          return bigmantissa;
        boolean neg = (bigmantissa.signum() < 0);
        if (neg) bigmantissa=bigmantissa.negate();
        while (curexp.signum()>0 && bigmantissa.signum()!=0) {
          int shift = 4096;
          if (curexp.compareTo(BigInteger.valueOf(shift)) < 0) {
            shift = curexp.intValue();
          }
          bigmantissa=bigmantissa.shiftLeft(shift);
          curexp=curexp.subtract(BigInteger.valueOf(shift));
        }
        if (neg) bigmantissa=bigmantissa.negate();
        return bigmantissa;
      } else {
        // Has fractional parts,
        // shift right without rounding
        BigInteger curexp = this.getExponent();
        BigInteger bigmantissa = this.getMantissa();
        if (bigmantissa.signum()==0)
          return bigmantissa;
        boolean neg = (bigmantissa.signum() < 0);
        if (neg) bigmantissa=bigmantissa.negate();
        while (curexp.signum()<0 && bigmantissa.signum()!=0) {
          int shift = 4096;
          if (curexp.compareTo(BigInteger.valueOf(-4096)) > 0) {
            shift = -(curexp.intValue());
          }
          bigmantissa=bigmantissa.shiftRight(shift);
          curexp=curexp.add(BigInteger.valueOf(shift));
        }
        if (neg) bigmantissa=bigmantissa.negate();
        return bigmantissa;
      }
    }

    private static BigInteger OneShift23 = BigInteger.valueOf(1L << 23);
    private static BigInteger OneShift52 = BigInteger.valueOf(1L << 52);
    private static BigInteger OneShift62 = BigInteger.valueOf(1L << 62);

    /**
     * Converts this value to a 32-bit floating-point number. The half-even
     * rounding mode is used.
     * @return The closest 32-bit floating-point number to this value.
     * The return value can be positive infinity or negative infinity if
     * this value exceeds the range of a 32-bit floating point number.
     */
    public float ToSingle() {
      BigInteger bigmant = (this.mantissa).abs();
      FastInteger bigexponent = new FastInteger(this.exponent);
      int bitLeftmost = 0;
      int bitsAfterLeftmost = 0;
      if (this.mantissa.signum()==0) {
        return 0.0f;
      }
      long smallmant = 0;
      if (bigmant.compareTo(OneShift23) < 0) {
        smallmant = bigmant.longValue();
        int exponentchange = 0;
        while (smallmant < (1L << 23)) {
          smallmant <<= 1;
          exponentchange++;
        }
        bigexponent.Subtract(exponentchange);
      } else {
        ShiftAccumulator accum = new ShiftAccumulator(bigmant);
        accum.ShiftToBits(24);
        bitsAfterLeftmost = accum.getBitsAfterLeftmost();
        bitLeftmost = accum.getBitLeftmost();
        bigexponent.Add(accum.getDiscardedBitCount());
        smallmant = accum.getShiftedIntSmall();
      }
      // Round half-even
      if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 || (smallmant & 1) != 0)) {
        smallmant++;
        if (smallmant == (1 << 24)) {
          smallmant >>= 1;
          bigexponent.Add(1);
        }
      }
      boolean subnormal = false;
      if (bigexponent.compareTo(104) > 0) {
        // exponent too big
        return (this.mantissa.signum() < 0) ?
          Float.NEGATIVE_INFINITY :
          Float.POSITIVE_INFINITY;
      } else if (bigexponent.compareTo(-149) < 0) {
        // subnormal
        subnormal = true;
        // Shift while number remains subnormal
        ShiftAccumulator accum = new ShiftAccumulator(smallmant);
        FastInteger fi = new FastInteger(bigexponent).Subtract(-149).Abs();
        accum.ShiftRight(fi);
        bitsAfterLeftmost = accum.getBitsAfterLeftmost();
        bitLeftmost = accum.getBitLeftmost();
        bigexponent.Add(accum.getDiscardedBitCount());
        smallmant = accum.getShiftedIntSmall();
        // Round half-even
        if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 || (smallmant & 1) != 0)) {
          smallmant++;
          if (smallmant == (1 << 24)) {
            smallmant >>= 1;
            bigexponent.Add(1);
          }
        }
      }
      if (bigexponent.compareTo(-149) < 0) {
        // exponent too small, so return zero
        return (this.mantissa.signum() < 0) ?
          Float.intBitsToFloat(1 << 31) :
          Float.intBitsToFloat(0);
      } else {
        int smallexponent = bigexponent.AsInt32();
        smallexponent = smallexponent + 150;
        int smallmantissa = ((int)smallmant) & 0x7FFFFF;
        if (!subnormal) {
          smallmantissa |= (smallexponent << 23);
        }
        if (this.mantissa.signum() < 0) smallmantissa |= (1 << 31);
        return Float.intBitsToFloat(smallmantissa);
      }
    }

    /**
     * Converts this value to a 64-bit floating-point number. The half-even
     * rounding mode is used.
     * @return The closest 64-bit floating-point number to this value.
     * The return value can be positive infinity or negative infinity if
     * this value exceeds the range of a 64-bit floating point number.
     */
    public double ToDouble() {
      BigInteger bigmant = (this.mantissa).abs();
      FastInteger bigexponent = new FastInteger(this.exponent);
      int bitLeftmost = 0;
      int bitsAfterLeftmost = 0;
      if (this.mantissa.signum()==0) {
        return 0.0d;
      }
      long smallmant = 0;
      if (bigmant.compareTo(OneShift52) < 0) {
        smallmant = bigmant.longValue();
        int exponentchange = 0;
        while (smallmant < (1L << 52)) {
          smallmant <<= 1;
          exponentchange++;
        }
        bigexponent.Subtract(exponentchange);
      } else {
        ShiftAccumulator accum = new ShiftAccumulator(bigmant);
        accum.ShiftToBits(53);
        bitsAfterLeftmost = accum.getBitsAfterLeftmost();
        bitLeftmost = accum.getBitLeftmost();
        bigexponent.Add(accum.getDiscardedBitCount());
        smallmant = accum.getShiftedIntSmall();
      }
      // Round half-even
      if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 || (smallmant & 1) != 0)) {
        smallmant++;
        if (smallmant == (1 << 53)) {
          smallmant >>= 1;
          bigexponent.Add(1);
        }
      }
      boolean subnormal = false;
      if (bigexponent.compareTo(971) > 0) {
        // exponent too big
        return (this.mantissa.signum() < 0) ?
          Double.NEGATIVE_INFINITY :
          Double.POSITIVE_INFINITY;
      } else if (bigexponent.compareTo(-1074) < 0) {
        // subnormal
        subnormal = true;
        // Shift while number remains subnormal
        ShiftAccumulator accum = new ShiftAccumulator(smallmant);
        FastInteger fi = new FastInteger(bigexponent).Subtract(-1074).Abs();
        accum.ShiftRight(fi);
        bitsAfterLeftmost = accum.getBitsAfterLeftmost();
        bitLeftmost = accum.getBitLeftmost();
        bigexponent.Add(accum.getDiscardedBitCount());
        smallmant = accum.getShiftedIntSmall();
        // Round half-even
        if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 || (smallmant & 1) != 0)) {
          smallmant++;
          if (smallmant == (1 << 53)) {
            smallmant >>= 1;
            bigexponent.Add(1);
          }
        }
      }
      if (bigexponent.compareTo(-1074) < 0) {
        // exponent too small, so return zero
        return (this.mantissa.signum() < 0) ?
          Double.longBitsToDouble(1L << 63) :
          Double.longBitsToDouble(0);
      } else {
        long smallexponent = bigexponent.AsInt64();
        smallexponent = smallexponent + 1075;
        long smallmantissa = smallmant & 0xFFFFFFFFFFFFFL;
        if (!subnormal) {
          smallmantissa |= (smallexponent << 52);
        }
        if (this.mantissa.signum() < 0) smallmantissa |= (1L << 63);
        return Double.longBitsToDouble(smallmantissa);
      }
    }
    /**
     * Converts this value to a string.The format of the return value is exactly
     * the same as that of the java.math.BigDecimal.toString() method.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return DecimalFraction.FromBigFloat(this).toString();
    }
    /**
     * Same as toString(), except that when an exponent is used it will be
     * a multiple of 3. The format of the return value follows the format of
     * the java.math.BigDecimal.toEngineeringString() method.
     */
    public String ToEngineeringString() {
      return DecimalFraction.FromBigFloat(this).ToEngineeringString();
    }
    /**
     * Converts this value to a string, but without an exponent part.getThe()
     * format of the return value follows the format of the java.math.BigDecimal.toPlainString()
     * method.
     */
    public String ToPlainString() {
      return DecimalFraction.FromBigFloat(this).ToPlainString();
    }
  }
