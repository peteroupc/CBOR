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
        BitShiftAccumulator accum = new BitShiftAccumulator(bigmant);
        accum.ShiftToDigits(24);
        bitsAfterLeftmost = accum.getOlderDiscardedDigits();
        bitLeftmost = accum.getLastDiscardedDigit();
        bigexponent.Add(accum.getDiscardedDigitCount());
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
        BitShiftAccumulator accum = new BitShiftAccumulator(smallmant);
        FastInteger fi = new FastInteger(bigexponent).Subtract(-149).Abs();
        accum.ShiftRight(fi);
        bitsAfterLeftmost = accum.getOlderDiscardedDigits();
        bitLeftmost = accum.getLastDiscardedDigit();
        bigexponent.Add(accum.getDiscardedDigitCount());
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
        BitShiftAccumulator accum = new BitShiftAccumulator(bigmant);
        accum.ShiftToDigits(53);
        bitsAfterLeftmost = accum.getOlderDiscardedDigits();
        bitLeftmost = accum.getLastDiscardedDigit();
        bigexponent.Add(accum.getDiscardedDigitCount());
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
        BitShiftAccumulator accum = new BitShiftAccumulator(smallmant);
        FastInteger fi = new FastInteger(bigexponent).Subtract(-1074).Abs();
        accum.ShiftRight(fi);
        bitsAfterLeftmost = accum.getOlderDiscardedDigits();
        bitLeftmost = accum.getLastDiscardedDigit();
        bigexponent.Add(accum.getDiscardedDigitCount());
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


    private static final class BinaryMathHelper implements IRadixMathHelper<BigFloat> {

    /**
     * 
     */
public int GetRadix() {
        return 2;
      }

    /**
     * 
     * @param value A BigFloat object.
     */
public BigFloat Abs(BigFloat value) {
        return value.Abs();
      }
      
    /**
     * 
     * @param value A BigFloat object.
     */
public int GetSign(BigFloat value) {
        return value.signum();
      }

    /**
     * 
     * @param value A BigFloat object.
     */
public BigInteger GetMantissa(BigFloat value) {
        return value.mantissa;
      }

    /**
     * 
     * @param value A BigFloat object.
     */
public BigInteger GetExponent(BigFloat value) {
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
        if (negative) mantissa=mantissa.negate();
        BigInteger diff = (e1.subtract(e2)).abs();
        mantissa = ShiftLeft(mantissa, diff);
        if (negative) mantissa=mantissa.negate();
        return mantissa;
      }

    /**
     * 
     * @param mantissa A BigInteger object.
     * @param exponent A BigInteger object.
     */
public BigFloat CreateNew(BigInteger mantissa, BigInteger exponent) {
        return new BigFloat(mantissa, exponent);
      }

    /**
     * 
     * @param value A BigInteger object.
     * @param lastDigit A 32-bit signed integer.
     * @param olderDigits A 32-bit signed integer.
     * @param bigint A BigInteger object.
     */
public IShiftAccumulator CreateShiftAccumulator(BigInteger bigint, int lastDigit, int olderDigits) {
        return new BitShiftAccumulator(bigint, lastDigit, olderDigits);
      }

    /**
     * 
     * @param value A BigInteger object.
     * @param bigint A BigInteger object.
     */
public IShiftAccumulator CreateShiftAccumulator(BigInteger bigint) {
        return new BitShiftAccumulator(bigint);
      }

    /**
     * 
     * @param value A 64-bit signed integer.
     */
public IShiftAccumulator CreateShiftAccumulator(long value) {
        return new BitShiftAccumulator(value);
      }

    /**
     * 
     * @param num A BigInteger object.
     * @param den A BigInteger object.
     */
public boolean HasTerminatingRadixExpansion(BigInteger num, BigInteger den) {
        BigInteger gcd = num.gcd(den);
        if (gcd.signum()==0) return false;
        den=den.divide(gcd);
        while (den.testBit(0)==false) {
          den=den.shiftRight(1);
        }
        return den.equals(BigInteger.ONE);
      }

    /**
     * 
     * @param bigint A BigInteger object.
     * @param power A 64-bit signed integer.
     */
public BigInteger MultiplyByRadixPower(BigInteger bigint, long power) {
        if (power <= 0) return bigint;
        return ShiftLeft(bigint, power);
      }

    /**
     * 
     * @param bigint A BigInteger object.
     * @param power A FastInteger object.
     */
public BigInteger MultiplyByRadixPower(BigInteger bigint, FastInteger power) {
        if (power.signum() <= 0) return bigint;
        if (power.CanFitInInt64()) {
          return ShiftLeft(bigint, power.AsInt64());
        } else {
          return ShiftLeft(bigint, power.AsBigInteger());
        }
      }
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
     * Gets the absolute value of this object.
     */
    public BigFloat Abs() {
      if (this.signum() < 0) {
        return Negate();
      } else {
        return this;
      }
    }

    /**
     * Gets an object with the same value as this one, but with the sign reversed.
     */
    public BigFloat Negate() {
      BigInteger neg=(this.mantissa).negate();
      return new BigFloat(neg, this.exponent);
    }

    /**
     * Divides this object by another bigfloat and returns the result.
     * @param divisor The divisor.
     * @return The quotient of the two numbers.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The result would have a nonterminating
     * decimal expansion.
     */
    public BigFloat Divide(BigFloat divisor) {
      return Divide(divisor, PrecisionContext.Unlimited);
    }

    /**
     * Divides this object by another bigfloat and returns a result with
     * the same exponent as this object (the dividend).
     * @param divisor The divisor.
     * @param rounding The rounding mode to use if the result must be scaled
     * down to have the same exponent as this value.
     * @return The quotient of the two numbers.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary
     * and the result is not exact.
     */
    public BigFloat Divide(BigFloat divisor, Rounding rounding) {
      return Divide(divisor, this.exponent, rounding);
    }

    /**
     * Divides two BigFloat objects, and returns the integer part of the
     * result, with the preferred exponent set to this value's exponent
     * minus the divisor's exponent.
     * @param divisor The divisor.
     * @return The integer part of the quotient of the two objects.
     * @throws ArithmeticException Attempted to divide by zero.
     */
    public BigFloat DivideToIntegerNaturalScale(
      BigFloat divisor
      ) {
      return DivideToIntegerNaturalScale(divisor, PrecisionContext.Unlimited);
    }


    /**
     * 
     * @param divisor A BigFloat object.
     */
    public BigFloat RemainderNaturalScale(
          BigFloat divisor
          ) {
      return Subtract(this.DivideToIntegerNaturalScale(divisor).Multiply(divisor));
    }


    /**
     * Divides two BigFloat objects, and gives a particular exponent to
     * the result.
     * @param divisor A BigFloat object.
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
    public BigFloat Divide(
          BigFloat divisor,
          long desiredExponentLong,
          PrecisionContext ctx
        ) {
      return Divide(divisor, BigInteger.valueOf(desiredExponentLong), ctx);
    }









    /**
     * Divides two BigFloat objects.
     * @param divisor A BigFloat object.
     * @param desiredExponentLong A 64-bit signed integer.
     * @param rounding A Rounding object.
     * @return The quotient of the two objects.
     */
    public BigFloat Divide(
              BigFloat divisor,
              long desiredExponentLong,
              Rounding rounding
            ) {
      return Divide(divisor, BigInteger.valueOf(desiredExponentLong), new PrecisionContext(rounding));
    }









    /**
     * Divides two BigFloat objects.
     * @param divisor A BigFloat object.
     * @param desiredExponentLong A BigInteger object.
     * @param rounding A Rounding object.
     * @param desiredExponent A BigInteger object.
     * @return The quotient of the two objects.
     */
    public BigFloat Divide(
              BigFloat divisor,
              BigInteger desiredExponent,
              Rounding rounding
            ) {
      return Divide(divisor, desiredExponent, new PrecisionContext(rounding));
    }


    /**
     * 
     * @param context A PrecisionContext object.
     */
    public BigFloat Abs(PrecisionContext context) {
      return Abs().RoundToPrecision(context);
    }










    /**
     * 
     * @param context A PrecisionContext object.
     */
    public BigFloat Negate(PrecisionContext context) {
      return Negate().RoundToPrecision(context);
    }







    /**
     * 
     * @param decfrac A BigFloat object.
     */
    public BigFloat Add(BigFloat decfrac) {
      return Add(decfrac, PrecisionContext.Unlimited);
    }






    /**
     * Subtracts a BigFloat object from this instance.
     * @param decfrac A BigFloat object.
     * @return The difference of the two objects.
     */
    public BigFloat Subtract(BigFloat decfrac) {
      return Add(decfrac.Negate());
    }










    /**
     * Subtracts a BigFloat object from another BigFloat object.
     * @param decfrac A BigFloat object.
     * @param ctx A PrecisionContext object.
     * @return The difference of the two objects.
     */
    public BigFloat Subtract(BigFloat decfrac, PrecisionContext ctx) {
      return Add(decfrac.Negate(), ctx);
    }
    /**
     * Multiplies two bigfloats. The resulting scale will be the sum of the
     * scales of the two bigfloats.
     * @param decfrac Another bigfloat.
     * @return The product of the two bigfloats. If a precision context is
     * given, returns null if the result of rounding would cause an overflow.
     */
    public BigFloat Multiply(BigFloat decfrac) {
      return Multiply(decfrac, PrecisionContext.Unlimited);
    }

    /**
     * Multiplies by one bigfloat, and then adds another bigfloat.
     * @param multiplicand The value to multiply.
     * @param augend The value to add.
     * @return The result this * multiplicand + augend.
     */
    public BigFloat MultiplyAndAdd(BigFloat multiplicand,
                                          BigFloat augend) {
      return this.Multiply(multiplicand).Add(augend);
    }
    //----------------------------------------------------------------

    private static RadixMath<BigFloat> math = new RadixMath<BigFloat>(
  new BinaryMathHelper());

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
    public BigFloat DivideToIntegerNaturalScale(
  BigFloat divisor, PrecisionContext ctx) {
      return math.DivideToIntegerNaturalScale(this, divisor, ctx);
    }

    /**
     * Divides this object by another object, and returns the integer part
     * of the result, with the exponent set to 0.
     * @param divisor The divisor.
     * @param ctx A precision context object to control the precision. The
     * rounding and exponent range settings of thisValue context are ignored.
     * No flags will be set from thisValue operation even if HasFlags of the
     * context is true. Can be null.
     * @return The integer part of the quotient of the two objects. The exponent
     * will be set to 0.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The result doesn't fit the given precision.
     */
    public BigFloat DivideToIntegerZeroScale(
      BigFloat divisor, PrecisionContext ctx) {
      return math.DivideToIntegerZeroScale(this, divisor, ctx);
    }

    /**
     * Divides this BigFloat object by another BigFloat object. The preferred
     * exponent for the result is this object's exponent minus the divisor's
     * exponent.
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
    public BigFloat Divide(
          BigFloat divisor,
          PrecisionContext ctx
        ) {
      return math.Divide(this, divisor, ctx);
    }

    /**
     * Divides this object by another object, and gives a particular exponent
     * to the result.
     * @param divisor The divisor.
     * @param ctx A precision context object to control the rounding mode.
     * The precision and exponent range settings of thisValue context are
     * ignored. If HasFlags of the context is true, will also store the flags
     * resulting from the operation (the flags are in addition to the pre-existing
     * flags). Can be null, in which case the default rounding mode is HalfEven.
     * @param desiredExponent A BigInteger object.
     * @param thisValue A T object.
     * @param exponent A BigInteger object.
     * @return The quotient of the two objects.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary
     * and the result is not exact.
     */
   public BigFloat Divide(
      BigFloat divisor, BigInteger exponent, PrecisionContext ctx) {
      return math.Divide(this, divisor, exponent, ctx);
    }

    /**
     * Gets the greater value between two bigfloats.
     * @param a A BigFloat object.
     * @param b A BigFloat object.
     * @return The larger value of the two objects.
     */
    public static BigFloat Max(
       BigFloat a, BigFloat b) {
      return math.Max(a, b);
    }

    /**
     * Gets the lesser value between two decimal fractions.
     * @param a A BigFloat object.
     * @param b A BigFloat object.
     * @return The smaller value of the two objects.
     */
    public static BigFloat Min(
       BigFloat a, BigFloat b) {
      return math.Min(a, b);
    }
    /**
     * Gets the greater value between two values, ignoring their signs.
     * If the absolute values are equal, has the same effect as Max.
     * @param a A BigFloat object.
     * @param b A BigFloat object.
     */
    public static BigFloat MaxMagnitude(
       BigFloat a, BigFloat b) {
      return math.MaxMagnitude(a, b);
    }
    /**
     * Gets the lesser value between two values, ignoring their signs. If
     * the absolute values are equal, has the same effect as Min.
     * @param a A BigFloat object.
     * @param b A BigFloat object.
     */
    public static BigFloat MinMagnitude(
       BigFloat a, BigFloat b) {
      return math.MinMagnitude(a, b);
    }
    /**
     * Compares the mathematical values of this object and another object.
     * <p> This method is not consistent with the Equals method because two
     * different decimal fractions with the same mathematical value, but
     * different exponents, will compare as equal.</p>
     * @param op A BigFloat object.
     * @return Less than 0 if this object's value is less than the other value,
     * or greater than 0 if this object's value is greater than the other value
     * or if "other" is null, or 0 if both values are equal.
     */
public int compareTo(
       BigFloat op) {
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
     * @param op A BigFloat object.
     * @return The sum of thisValue and the other object. Returns null if
     * the result would overflow the exponent range.
     */
public BigFloat Add(
       BigFloat op, PrecisionContext ctx) {
      return math.Add(this, op, ctx);
    }
    /**
     * Returns a decimal fraction with the same value but a new exponent.
     * @param otherValue A decimal fraction containing the desired exponent
     * of the result. The mantissa is ignored.
     * @param ctx A PrecisionContext object.
     * @param op A BigFloat object.
     * @return A decimal fraction with the same value as this object but with
     * the exponent changed.
     */
public BigFloat Quantize(
       BigFloat op, PrecisionContext ctx) {
      return math.Quantize(this, op, ctx);
    }
    /**
     * 
     * @param ctx A PrecisionContext object.
     */
public BigFloat RoundToIntegralExact(
      PrecisionContext ctx) {
  return math.RoundToIntegralExact(this, ctx);
}
    /**
     * 
     * @param ctx A PrecisionContext object.
     */
public BigFloat RoundToIntegralValue(
      PrecisionContext ctx) {
        return math.RoundToIntegralValue(this, ctx);
}
    /**
     * Multiplies two BigFloat objects.
     * @param op A BigFloat object.
     * @param ctx A PrecisionContext object.
     * @return The product of the two objects.
     */
public BigFloat Multiply(
       BigFloat op, PrecisionContext ctx) {
      return math.Multiply(this, op, ctx);
    }

    /**
     * Multiplies by one value, and then adds another value.
     * @param multiplicand The value to multiply.
     * @param augend The value to add.
     * @param ctx A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true, will
     * also store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @param op A BigFloat object.
     * @return The result thisValue * multiplicand + augend. If a precision
     * context is given, returns null if the result of rounding would cause
     * an overflow. The caller can handle a null return value by treating
     * it as positive or negative infinity depending on the sign of this object's
     * value.
     */
    public BigFloat MultiplyAndAdd(
       BigFloat op, BigFloat augend, PrecisionContext ctx) {
      return math.MultiplyAndAdd(this, op, augend, ctx);
    }
    /**
     * Rounds this object's value to a given precision, using the given rounding
     * mode and range of exponent.
     * @param context A context for controlling the precision, rounding
     * mode, and exponent range. Can be null.
     * @param ctx A PrecisionContext object.
     * @return The closest value to this object's value, rounded to the specified
     * precision. Returns the same value as this object if "context" is null
     * or the precision and exponent range are unlimited. Returns null if
     * the result of the rounding would cause an overflow. The caller can
     * handle a null return value by treating it as positive or negative infinity
     * depending on the sign of this object's value.
     */
    public BigFloat RoundToPrecision(
       PrecisionContext ctx) {
      return math.RoundToPrecision(this, ctx);
    }

  }

