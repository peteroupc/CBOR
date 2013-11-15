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
   * Consists of a integer mantissa and an integer exponent, both arbitrary-precision.
   * The value of the number is equal to mantissa * 2^exponent. <p> Note:
   * This class doesn't yet implement certain operations, notably division,
   * that require results to be rounded. That's because I haven't decided
   * yet how to incorporate rounding into the API, since the results of
   * some divisions can't be represented exactly in a bigfloat (for example,
   * 1/10). Should I include precision and rounding mode, as is done in
   * Java's Big Decimal class, or should I also include minimum and maximum
   * exponent in the rounding parameters, for better support when converting
   * to other decimal number formats? Or is there a better approach to supporting
   * rounding? </p>
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
        if (e1.compareTo(e2) > 0) {
          while (e1.compareTo(e2) > 0) {
            mantissa=mantissa.shiftLeft(1);
            e1 = e1.subtract(BigInteger.ONE);
          }
        } else {
          while (e1.compareTo(e2) < 0) {
            mantissa=mantissa.shiftLeft(1);
            e1 = e1 .add(BigInteger.ONE);
          }
        }
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

    /**
     * Gets the greater value between two BigFloat values.
     */
    public static BigFloat Max(BigFloat a, BigFloat b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      return a.compareTo(b) > 0 ? a : b;
    }

    /**
     * Gets the lesser value between two BigFloat values.
     */
    public static BigFloat Min(BigFloat a, BigFloat b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      return a.compareTo(b) > 0 ? b : a;
    }

    /**
     * Finds the sum of this object and another bigfloat. The result's exponent
     * is set to the lower of the exponents of the two operands.
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
     * Compares the mathematical values of two bigfloats. <p>This method
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
      if (expcmp == 0) {
        return mantissa.compareTo((BigInteger)other.mantissa);
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
        bigmantissa=bigmantissa.multiply(DecimalFraction.FindPowerOfTen(bigintExp));
        return new BigFloat(bigmantissa);
      } else {
        // Fractional number
        FastInteger scale = new FastInteger(bigintExp);
        BigInteger bigmantissa = bigintMant;
        boolean neg = (bigmantissa.signum() < 0);
        BigInteger remainder;
        if (neg) bigmantissa=(bigmantissa).negate();
        FastInteger negscale=new FastInteger(scale).Negate();
        BigInteger divisor=DecimalFraction.FindPowerOfFive(negscale.AsBigInteger());
        while(true){
          BigInteger quotient;
BigInteger[] divrem=(bigmantissa).divideAndRemainder(divisor);
quotient=divrem[0];
remainder=divrem[1];
          // Ensure that the quotient has enough precision
          // to be converted accurately to a single or double
          if(remainder.signum()!=0 &&
             quotient.compareTo(OneShift62)<0){
            long smallquot=quotient.longValue();
            int shift=0;
            while(smallquot!=0 && smallquot<(1L<<62)){
              smallquot<<=1;
              shift++;
            }
            if(shift==0)shift=1;
            bigmantissa=bigmantissa.shiftLeft(shift);
            scale.Add(-shift);
          } else {
            bigmantissa=quotient;
            break;
          }
        }
        // Round half-even
        BigInteger halfDivisor=divisor;
        halfDivisor=halfDivisor.shiftRight(1);
        int cmp=remainder.compareTo(halfDivisor);
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
     * @param dbl A 32-bit floating-point number.
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
        if(bigmantissa.signum()==0)
          return bigmantissa;
        boolean neg = (bigmantissa.signum() < 0);
        if (neg) bigmantissa=bigmantissa.negate();
        while(curexp.signum()>0 && bigmantissa.signum()!=0){
          int shift=4096;
          if(curexp.compareTo(BigInteger.valueOf(shift))<0){
            shift=curexp.intValue();
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
        if(bigmantissa.signum()==0)
          return bigmantissa;
        boolean neg = (bigmantissa.signum() < 0);
        if (neg) bigmantissa=bigmantissa.negate();
        while(curexp.signum()<0 && bigmantissa.signum()!=0){
          int shift=4096;
          if(curexp.compareTo(BigInteger.valueOf(-4096))>0){
            shift=-(curexp.intValue());
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
      int bitsAfterLeftmost=0;
      if (this.mantissa.signum()==0) {
        return 0.0f;
      }
      long smallmant=0;
      if(bigmant.compareTo(OneShift23) < 0){
        smallmant=bigmant.longValue();
        int exponentchange=0;
        while(smallmant<(1L<<23)){
          smallmant<<=1;
          exponentchange++;
        }
        bigexponent.Subtract(exponentchange);
      } else {
        BigShiftAccumulator accum=new BigShiftAccumulator(bigmant);
        accum.ShiftToBits(24);
        bitsAfterLeftmost=accum.getBitsAfterLeftmost();
        bitLeftmost=accum.getBitLeftmost();
        bigexponent.Add(accum.getDiscardedBitCount());
        bigmant=accum.getShiftedInt();
        smallmant=bigmant.longValue();
      }
      // Round half-even
      if (bitLeftmost>0 && (bitsAfterLeftmost>0 || (smallmant&1)!=0)){
        smallmant++;
        if (smallmant==(1<<24)) {
          smallmant >>= 1;
          bigexponent.Add(1);
        }
      }
      boolean subnormal = false;
      if (bigexponent.compareTo(104)>0) {
        // exponent too big
        return (this.mantissa.signum() < 0) ?
          Float.NEGATIVE_INFINITY :
          Float.POSITIVE_INFINITY;
      } else if (bigexponent.compareTo(-149)<0) {
        // subnormal
        subnormal = true;
        // Shift while number remains subnormal
        SmallShiftAccumulator accum=new SmallShiftAccumulator(smallmant);
        FastInteger fi=new FastInteger(bigexponent).Subtract(-149).Abs();
        accum.ShiftRight(fi);
        bitsAfterLeftmost=accum.getBitsAfterLeftmost();
        bitLeftmost=accum.getBitLeftmost();
        bigexponent.Add(accum.getDiscardedBitCount());
        smallmant=accum.getShiftedInt();
        // Round half-even
        if (bitLeftmost>0 && (bitsAfterLeftmost>0 || (smallmant&1)!=0)){
          smallmant++;
          if (smallmant==(1<<24)) {
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
      int bitsAfterLeftmost=0;
      if (this.mantissa.signum()==0) {
        return 0.0d;
      }
      long smallmant=0;
      if(bigmant.compareTo(OneShift52) < 0){
        smallmant=bigmant.longValue();
        int exponentchange=0;
        while(smallmant<(1L<<52)){
          smallmant<<=1;
          exponentchange++;
        }
        bigexponent.Subtract(exponentchange);
      } else {
        BigShiftAccumulator accum=new BigShiftAccumulator(bigmant);
        accum.ShiftToBits(53);
        bitsAfterLeftmost=accum.getBitsAfterLeftmost();
        bitLeftmost=accum.getBitLeftmost();
        bigexponent.Add(accum.getDiscardedBitCount());
        bigmant=accum.getShiftedInt();
        smallmant=bigmant.longValue();
      }
      // Round half-even
      if (bitLeftmost>0 && (bitsAfterLeftmost>0 || (smallmant&1)!=0)){
        smallmant++;
        if (smallmant==(1<<53)) {
          smallmant >>= 1;
          bigexponent.Add(1);
        }
      }
      boolean subnormal = false;
      if (bigexponent.compareTo(971)>0) {
        // exponent too big
        return (this.mantissa.signum() < 0) ?
          Double.NEGATIVE_INFINITY :
          Double.POSITIVE_INFINITY;
      } else if (bigexponent.compareTo(-1074)<0) {
        // subnormal
        subnormal = true;
        // Shift while number remains subnormal
        SmallShiftAccumulator accum=new SmallShiftAccumulator(smallmant);
        FastInteger fi=new FastInteger(bigexponent).Subtract(-1074).Abs();
        accum.ShiftRight(fi);
        bitsAfterLeftmost=accum.getBitsAfterLeftmost();
        bitLeftmost=accum.getBitLeftmost();
        bigexponent.Add(accum.getDiscardedBitCount());
        smallmant=accum.getShiftedInt();
        // Round half-even
        if (bitLeftmost>0 && (bitsAfterLeftmost>0 || (smallmant&1)!=0)){
          smallmant++;
          if (smallmant==(1<<53)) {
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
     * Converts this value to a string. The format of the return value is exactly
     * the same as that of the java.math.BigDecimal.toString() method.
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
     * Converts this value to a string, but without an exponent part. The
     * format of the return value follows the format of the java.math.BigDecimal.toPlainString()
     * method.
     */
    public String ToPlainString() {
      return DecimalFraction.FromBigFloat(this).ToPlainString();
    }
  }
