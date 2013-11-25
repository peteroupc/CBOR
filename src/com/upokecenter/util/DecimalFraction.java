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
    private static BigInteger
      RescaleByExponentDiff(BigInteger mantissa,
                            BigInteger e1,
                            BigInteger e2) {
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
     * Gets an object with the same value as this one, but with the sign reversed.
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
     * Gets the lesser value between two DecimalFraction values. If both
     * values are equal, returns "a".
     * @param a A DecimalFraction object.
     * @param b A DecimalFraction object.
     * @return The smaller value of the two objects.
     */
    public static DecimalFraction Min(DecimalFraction a, DecimalFraction b) {
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
     * @param a A DecimalFraction object.
     * @param b A DecimalFraction object.
     */
    public static DecimalFraction MinMagnitude(DecimalFraction a, DecimalFraction b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      int cmp = a.Abs().compareTo(b.Abs());
      if (cmp == 0) return Min(a, b);
      return (cmp < 0) ? a : b;
    }
    /**
     * Gets the greater value between two values, ignoring their signs.
     * If the absolute values are equal, has the same effect as Max.
     * @param a A DecimalFraction object.
     * @param b A DecimalFraction object.
     */
    public static DecimalFraction MaxMagnitude(DecimalFraction a, DecimalFraction b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      int cmp = a.Abs().compareTo(b.Abs());
      if (cmp == 0) return Max(a, b);
      return (cmp > 0) ? a : b;
    }
    /**
     * Gets the greater value between two DecimalFraction values. If both
     * values are equal, returns "a".
     * @param a A DecimalFraction object.
     * @param b A DecimalFraction object.
     * @return The larger value of the two objects.
     */
    public static DecimalFraction Max(DecimalFraction a, DecimalFraction b) {
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

    private static boolean Round(DigitShiftAccumulator accum, Rounding rounding,
                              boolean neg, BigInteger bigval) {
      boolean incremented = false;
      if (rounding == Rounding.HalfUp) {
        if (accum.getBitLeftmost() >= 5) {
          incremented = true;
        }
      } else if (rounding == Rounding.HalfEven) {
        if (accum.getBitLeftmost() >= 5) {
          if ((accum.getBitLeftmost() > 5 || accum.getBitsAfterLeftmost() != 0)) {
            incremented = true;
          } else if (bigval.testBit(0)) {
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
        if (accum.getBitLeftmost() > 5 || (accum.getBitLeftmost() == 5 && accum.getBitsAfterLeftmost() != 0)) {
          incremented = true;
        }
      } else if (rounding == Rounding.Up) {
        if ((accum.getBitLeftmost() | accum.getBitsAfterLeftmost()) != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.ZeroFiveUp) {
        if ((accum.getBitLeftmost() | accum.getBitsAfterLeftmost()) != 0) {
          BigInteger bigdigit = bigval.remainder(BigInteger.TEN);
          int lastDigit = bigdigit.intValue();
          if (lastDigit == 0 || lastDigit == 5) {
            incremented = true;
          }
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
    public DecimalFraction RoundToPrecision(
      PrecisionContext context
     ) {
      if ((context) == null) throw new NullPointerException("context");
      FastInteger fastExp = new FastInteger(this.exponent);
      FastInteger fastEMin = new FastInteger(context.getEMin());
      FastInteger fastEMax = new FastInteger(context.getEMax());
      if (context.getPrecision() > 0 && context.getPrecision() <= 18) {
        // Check if rounding is necessary at all
        // for small precisions
        BigInteger mantabs = (this.mantissa).abs();
        if (mantabs.compareTo(FindPowerOfTen(context.getPrecision())) < 0) {
          FastInteger fastAdjustedExp = new FastInteger(fastExp)
            .Add(context.getPrecision()).Subtract(1);
          FastInteger fastNormalMin = new FastInteger(fastEMin)
            .Add(context.getPrecision()).Subtract(1);
          if (fastAdjustedExp.compareTo(new FastInteger(fastEMax)) <= 0 &&
             fastAdjustedExp.compareTo(fastNormalMin) >= 0) {
            return this;
          }
        }
      }
      int[] signals = new int[1];
      DecimalFraction dfrac = RoundToPrecision(
        context.getPrecision(),
        context.getRounding(), fastEMin, fastEMax, signals);
      if (context.getClampNormalExponents() && dfrac != null) {
        // Clamp exponents to eMax + 1 - precision
        // if directed
        FastInteger clamp = new FastInteger(fastEMax).Add(1).Subtract(context.getPrecision());
        fastExp = new FastInteger(dfrac.getExponent());
        if (fastExp.compareTo(clamp) > 0) {
          BigInteger bigmantissa = dfrac.mantissa;
          int sign = bigmantissa.signum();
          if (sign != 0) {
            if (sign < 0) bigmantissa=bigmantissa.negate();
            FastInteger expdiff = new FastInteger(fastExp).Subtract(clamp);
            if (expdiff.CanFitInInt64()) {
              bigmantissa=bigmantissa.multiply(FindPowerOfTen(expdiff.AsInt64()));
            } else {
              bigmantissa=bigmantissa.multiply(FindPowerOfTenFromBig(expdiff.AsBigInteger()));
            }
            if (sign < 0) bigmantissa=bigmantissa.negate();
          }
          if (signals != null)
            signals[0] |= PrecisionContext.SignalClamped;
          dfrac = new DecimalFraction(bigmantissa, clamp.AsBigInteger());
        }
      }
      if (context.getHasFlags()) {
        context.setFlags(context.getFlags()|signals[0]);
      }
      return dfrac;
    }

    private DecimalFraction RoundToPrecision(
      long precision,
      Rounding rounding,
      FastInteger fastEMin,
      FastInteger fastEMax,
      int[] signals
     ) {
      if ((precision) < 0) throw new IllegalArgumentException("precision" + " not greater or equal to " + "0" + " (" + Long.toString((long)(precision)) + ")");
      boolean neg = this.mantissa.signum() < 0;
      BigInteger bigmantissa = this.mantissa;
      if (neg) bigmantissa=bigmantissa.negate();
      // save mantissa in case result is subnormal
      // and must be rounded again
      BigInteger oldmantissa = bigmantissa;
      FastInteger exp = new FastInteger(this.exponent);
      int flags = 0;
      DigitShiftAccumulator accum = new DigitShiftAccumulator(bigmantissa);
      boolean unlimitedPrec = (precision == 0);
      if (precision > 0) {
        accum.ShiftToBits(precision);
      } else {
        precision = accum.getKnownBitLength();
      }
      FastInteger discardedBits = new FastInteger(accum.getDiscardedBitCount());
      exp.Add(discardedBits);
      FastInteger adjExponent = new FastInteger(exp)
        .Add(accum.getKnownBitLength()).Subtract(1);
      FastInteger clamp = null;
      if (adjExponent.compareTo(fastEMax) > 0) {
        if (oldmantissa.signum()==0) {
          flags |= PrecisionContext.SignalClamped;
          if (signals != null) signals[0] = flags;
          return new DecimalFraction(oldmantissa, fastEMax.AsBigInteger());
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
          BigInteger overflowMant = FindPowerOfTen(precision);
          overflowMant=overflowMant.subtract(BigInteger.ONE);
          if (neg) overflowMant=overflowMant.negate();
          if (signals != null) signals[0] = flags;
          clamp = new FastInteger(fastEMax).Add(1).Subtract(precision);
          return new DecimalFraction(overflowMant, clamp.AsBigInteger());
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
          accum = new DigitShiftAccumulator(oldmantissa);
          accum.ShiftRight(expdiff);
          BigInteger newmantissa = accum.getShiftedInt();
          if ((accum.getDiscardedBitCount()).signum() != 0) {
            if (oldmantissa.signum()!=0)
              flags |= PrecisionContext.SignalRounded;
            if ((accum.getBitLeftmost() | accum.getBitsAfterLeftmost()) != 0) {
              flags |= PrecisionContext.SignalInexact;
            }
            if (Round(accum, rounding, neg, newmantissa)) {
              newmantissa=newmantissa.add(BigInteger.ONE);
            }
          }
          if (newmantissa.signum()==0)
            flags |= PrecisionContext.SignalClamped;
          if ((flags & (PrecisionContext.SignalSubnormal | PrecisionContext.SignalInexact)) == (PrecisionContext.SignalSubnormal | PrecisionContext.SignalInexact))
            flags |= PrecisionContext.SignalUnderflow | PrecisionContext.SignalRounded;
          if (signals != null) signals[0] = flags;
          if (neg) newmantissa=newmantissa.negate();
          return new DecimalFraction(newmantissa, fastETiny.AsBigInteger());
        }
      }
      boolean expChanged = false;
      if ((accum.getDiscardedBitCount()).signum() != 0) {
        if (bigmantissa.signum()!=0)
          flags |= PrecisionContext.SignalRounded;
        bigmantissa = accum.getShiftedInt();
        if ((accum.getBitLeftmost() | accum.getBitsAfterLeftmost()) != 0) {
          flags |= PrecisionContext.SignalInexact;
        }
        if (Round(accum, rounding, neg, bigmantissa)) {
          bigmantissa=bigmantissa.add(BigInteger.ONE);
          if (bigmantissa.testBit(0)==false) {
            accum = new DigitShiftAccumulator(bigmantissa);
            accum.ShiftToBits(precision);
            if ((accum.getDiscardedBitCount()).signum() != 0) {
              exp.Add(accum.getDiscardedBitCount());
              discardedBits.Add(accum.getDiscardedBitCount());
              bigmantissa = accum.getShiftedInt();
              expChanged = true;
            }
          }
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
            BigInteger overflowMant = FindPowerOfTen(precision);
            overflowMant=overflowMant.subtract(BigInteger.ONE);
            if (neg) overflowMant=overflowMant.negate();
            if (signals != null) signals[0] = flags;
            clamp = new FastInteger(fastEMax).Add(1).Subtract(precision);
            return new DecimalFraction(overflowMant, clamp.AsBigInteger());
          }
          if (signals != null) signals[0] = flags;
          return null;
        }
      }
      if (signals != null) signals[0] = flags;
      if (neg) bigmantissa=bigmantissa.negate();
      return new DecimalFraction(bigmantissa, exp.AsBigInteger());
    }

    /**
     * Finds the sum of this object and another decimal fraction. The result's
     * exponent is set to the lower of the exponents of the two operands.
     * @param decfrac The number to add to.
     * @param ctx A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true, will
     * also store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The sum of this and the other object.
     */
    public DecimalFraction Add(DecimalFraction decfrac, PrecisionContext ctx) {
      int expcmp = this.exponent.compareTo(decfrac.exponent);
      DecimalFraction retval = null;
      if (expcmp == 0) {
        retval = new DecimalFraction(
          this.mantissa.add(decfrac.mantissa), this.exponent);
      } else {
        // choose the minimum exponent
        BigInteger resultExponent = (expcmp < 0 ? this.exponent : decfrac.exponent);
        DecimalFraction op1 = this;
        DecimalFraction op2 = decfrac;
        BigInteger expdiff = (op1.exponent.subtract(op2.exponent)).abs();
        if (ctx != null && ctx.getPrecision() > 0) {
          // Check if exponent difference is too big for
          // power-of-ten calculation to work quickly
          if (expdiff.compareTo(BigInteger.valueOf(100)) >= 0) {
            FastInteger fastint = new FastInteger(expdiff).Add(3);
            // If exponent difference plus 3 is greater than the precision
            if (fastint.compareTo(ctx.getPrecision()) > 0) {
              int expcmp2 = op1.exponent.compareTo(op2.exponent);
              if (expcmp2 < 0 && !(op2.mantissa.signum()==0)) {
                // first operand's exponent is less
                // and second operand isn't zero
                // the 8 digits at the end are guard digits
                op1 = new DecimalFraction(
                  op1.mantissa, new FastInteger(op2.exponent).Subtract(ctx.getPrecision()).Subtract(8)
                  .AsBigInteger());
              } else if (expcmp2 > 0 && !(op1.mantissa.signum()==0)) {
                // first operand's exponent is greater
                // and first operand isn't zero
                // the 8 digits at the end are guard digits
                op2 = new DecimalFraction(
                  op2.mantissa, new FastInteger(op1.exponent).Subtract(ctx.getPrecision()).Subtract(8)
                  .AsBigInteger());
              }
              expcmp = op1.exponent.compareTo(op2.exponent);
              resultExponent = (expcmp < 0 ? op1.exponent : op2.exponent);
            }
          }
        }
        if (expcmp > 0) {
          BigInteger newmant = RescaleByExponentDiff(
            op1.mantissa, op1.exponent, op2.exponent);
          retval = new DecimalFraction(
            newmant.add(op2.mantissa), resultExponent);
        } else {
          BigInteger newmant = RescaleByExponentDiff(
            op2.mantissa, op1.exponent, op2.exponent);
          retval = new DecimalFraction(
            newmant.add(op1.mantissa), resultExponent);
        }
      }
      if (ctx != null) {
        retval = retval.RoundToPrecision(ctx);
      }
      return retval;
    }

    /**
     * Finds the sum of this object and another decimal fraction. The result's
     * exponent is set to the lower of the exponents of the two operands.
     * @param decfrac A DecimalFraction object.
     */
    public DecimalFraction Add(DecimalFraction decfrac) {
      return Add(decfrac, null);
    }
    /**
     * Finds the difference between this object and another decimal fraction.
     * The result's exponent is set to the lower of the exponents of the two
     * operands.
     * @param decfrac A DecimalFraction object.
     * @return The difference of the two objects.
     */
    public DecimalFraction Subtract(DecimalFraction decfrac) {
      return Add(decfrac.Negate());
    }
    /**
     * Finds the difference between this object and another decimal fraction.
     * The result's exponent is set to the lower of the exponents of the two
     * operands.
     * @param decfrac A DecimalFraction object.
     * @param ctx A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true, will
     * also store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The difference of the two objects.
     */
    public DecimalFraction Subtract(DecimalFraction decfrac, PrecisionContext ctx) {
      return Add(decfrac.Negate(), ctx);
    }
    /**
     * Multiplies two decimal fractions. The resulting scale will be the
     * sum of the scales of the two decimal fractions.
     * @param decfrac Another decimal fraction.
     * @return The product of the two decimal fractions.
     */
    public DecimalFraction Multiply(DecimalFraction decfrac) {
      return Multiply(decfrac, null);
    }
    /**
     * Multiplies two decimal fractions. The resulting scale will be the
     * sum of the scales of the two decimal fractions.
     * @param decfrac Another decimal fraction.
     * @param ctx A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true, will
     * also store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The product of the two decimal fractions.
     */
    public DecimalFraction Multiply(DecimalFraction decfrac, PrecisionContext ctx) {
      BigInteger newexp = (this.exponent.add(decfrac.exponent));
      DecimalFraction ret = new DecimalFraction(mantissa.multiply(decfrac.mantissa), newexp);
      if (ctx != null) {
        ret = ret.RoundToPrecision(ctx);
      }
      return ret;
    }
    /**
     * Multiplies by one decimal fraction, and then adds another decimal
     * fraction.
     * @param multiplicand The value to multiply.
     * @param augend The value to add.
     * @param ctx A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true, will
     * also store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The result this * multiplicand + augend.
     */
    public DecimalFraction MultiplyAndAdd(DecimalFraction multiplicand,
                                          DecimalFraction augend,
                                          PrecisionContext ctx) {
      BigInteger newexp = (this.exponent.add(multiplicand.exponent));
      DecimalFraction addend = new DecimalFraction(mantissa.multiply(multiplicand.mantissa),
                                                 newexp);
      return addend.Add(augend, ctx);
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
     * Compares the mathematical values of two decimal fractions. <p> This
     * method is not consistent with the Equals method because two different
     * decimal fractions with the same mathematical value, but different
     * exponents, will compare as equal.</p>
     * @param decfrac A DecimalFraction object.
     * @return Less than 0 if this value is less than the other value, or greater
     * than 0 if this value is greater than the other value or if "other" is
     * null, or 0 if both values are equal.
     */
    public int compareTo(DecimalFraction decfrac) {
      if (decfrac == null) return 1;
      int s = this.signum();
      int ds = decfrac.signum();
      if (s != ds) return (s < ds) ? -1 : 1;
      int expcmp = exponent.compareTo(decfrac.exponent);
      int mantcmp = mantissa.compareTo(decfrac.mantissa);
      if (mantcmp == 0) {
        // Special case: Mantissas are equal
        return s == 0 ? 0 : expcmp * s;
      }
      if (ds == 0) {
        // Special case: Second operand is zero
        return s;
      }
      if (s == 0) {
        // Special case: First operand is zero
        return -ds;
      }
      if (expcmp == 0) {
        return mantcmp;
      } else {
        BigInteger op1Exponent = this.exponent;
        BigInteger op2Exponent = decfrac.exponent;
        BigInteger expdiff = (op1Exponent.subtract(op2Exponent)).abs();
        // Check if exponent difference is too big for
        // power-of-ten calculation to work quickly
        if (expdiff.compareTo(BigInteger.valueOf(100)) >= 0) {
          FastInteger fastint = new FastInteger(expdiff).Add(3);
          long precision1 = new DigitShiftAccumulator(
            (this.mantissa).abs()).getKnownBitLength();
          long precision2 = new DigitShiftAccumulator(
            (decfrac.mantissa).abs()).getKnownBitLength();
          long maxPrecision = Math.max(precision1, precision2);
          // If exponent difference plus 3 is greater than the
          // maximum precision of the two operands
          if (fastint.compareTo(maxPrecision) > 0) {
            int expcmp2 = op1Exponent.compareTo(op2Exponent);
            if (expcmp2 < 0) {
              // first operand's exponent is less
              // (second operand won't be zero at this point)
              // the 8 digits at the end are guard digits
              op1Exponent = (new FastInteger(op2Exponent).Subtract(maxPrecision).Subtract(8)
                           .AsBigInteger());
            } else if (expcmp2 > 0) {
              // first operand's exponent is greater
              // (first operand won't be zero at this point)
              // the 8 digits at the end are guard digits
              op2Exponent = (new FastInteger(op1Exponent).Subtract(maxPrecision).Subtract(8)
                           .AsBigInteger());
            }
            expcmp = op1Exponent.compareTo(op2Exponent);
          }
        }
        if (expcmp > 0) {
          BigInteger newmant = RescaleByExponentDiff(
            mantissa, op1Exponent, op2Exponent);
          return newmant.compareTo(decfrac.mantissa);
        } else {
          BigInteger newmant = RescaleByExponentDiff(
            decfrac.mantissa, op1Exponent, op2Exponent);
          return this.mantissa.compareTo(newmant);
        }
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
     * @param dbl A 32-bit floating-point number.
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
  }
