package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */




    /**
     * Represents an arbitrary-precision decimal floating-point number,
     * supporting only finite decimal numbers. Consists of an integer mantissa
     * and an integer exponent, both arbitrary-precision. The value of
     * the number is equal to mantissa * 10^exponent.
     * @deprecated Use ExtendedDecimal instead, which supports more kinds of values than DecimalFraction. 
 */
@Deprecated
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
     * @param other A DecimalFraction object.
     * @return A Boolean object.
     */
    public boolean EqualsInternal(DecimalFraction other) {
      DecimalFraction otherValue = ((other instanceof DecimalFraction) ? (DecimalFraction)other : null);
      if (otherValue == null)
        return false;
      return this.exponent.equals(otherValue.exponent) &&
        this.mantissa.equals(otherValue.mantissa);
    }
    
    
    /**
     * 
     * @param other A DecimalFraction object.
     * @return A Boolean object.
     */
    public boolean equals(DecimalFraction other) {
      return EqualsInternal(other);
    }
    /**
     * Determines whether this object's mantissa and exponent are equal
     * to those of another object and that other object is a decimal fraction.
     * @param obj An arbitrary object.
     * @return True if the objects are equal; false otherwise.
     */
    @Override public boolean equals(Object obj) {
      return EqualsInternal(((obj instanceof DecimalFraction) ? (DecimalFraction)obj : null));
    }
    /**
     * Calculates this object's hash code.
     * @return This object&apos;s hash code.
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
    public DecimalFraction (BigInteger mantissa, BigInteger exponent) {
      this.exponent = exponent;
      this.mantissa = mantissa;
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
     * @param str A string that represents a number.
     * @return A DecimalFraction object.
     */
    public static DecimalFraction FromString(String str) {
      ExtendedDecimal ed=ExtendedDecimal.FromString(str);
      return new DecimalFraction(ed.getMantissa(),ed.getExponent());
    }
    

    private static DecimalFraction MinusOne=new DecimalFraction(BigInteger.ZERO.subtract(BigInteger.ONE),BigInteger.ZERO);
    
    private static final class DecimalMathHelper implements IRadixMathHelper<DecimalFraction> {

    /**
     * 
     * @return A 32-bit signed integer.
     */
      public int GetRadix() {
        return 10;
      }

    /**
     * 
     * @param value A DecimalFraction object.
     * @return A 32-bit signed integer.
     */
      public int GetSign(DecimalFraction value) {
        return value.signum();
      }

    /**
     * 
     * @param value A DecimalFraction object.
     * @return A BigInteger object.
     */
      public BigInteger GetMantissa(DecimalFraction value) {
        return value.mantissa;
      }

    /**
     * 
     * @param value A DecimalFraction object.
     * @return A BigInteger object.
     */
      public BigInteger GetExponent(DecimalFraction value) {
        return value.exponent;
      }

    /**
     * 
     * @param mantissa A BigInteger object.
     * @param e1 A BigInteger object.
     * @param e2 A BigInteger object.
     * @return A BigInteger object.
     */
      public BigInteger RescaleByExponentDiff(BigInteger mantissa, BigInteger e1, BigInteger e2) {
        if (mantissa.signum() == 0) return BigInteger.ZERO;
        FastInteger diff = FastInteger.FromBig(e1).SubtractBig(e2).Abs();
        if (diff.CanFitInInt32()) {
          mantissa=mantissa.multiply(DecimalUtility.FindPowerOfTen(diff.AsInt32()));
        } else {
          mantissa=mantissa.multiply(DecimalUtility.FindPowerOfTenFromBig(diff.AsBigInteger()));
        }
        return mantissa;
      }

    /**
     * 
     * @param lastDigit A 32-bit signed integer.
     * @param olderDigits A 32-bit signed integer.
     * @param bigint A BigInteger object.
     * @return An IShiftAccumulator object.
     */
      public IShiftAccumulator CreateShiftAccumulatorWithDigits(BigInteger bigint, int lastDigit, int olderDigits) {
        return new DigitShiftAccumulator(bigint, lastDigit, olderDigits);
      }

    /**
     * 
     * @param bigint A BigInteger object.
     * @return An IShiftAccumulator object.
     */
      public IShiftAccumulator CreateShiftAccumulator(BigInteger bigint) {
        return new DigitShiftAccumulator(bigint,0,0);
      }

    /**
     * 
     * @param numerator A BigInteger object.
     * @param denominator A BigInteger object.
     * @return A Boolean object.
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
        while(true){
          BigInteger bigrem;
          BigInteger bigquo;
BigInteger[] divrem=(denominator).divideAndRemainder(BigInteger.valueOf(5));
bigquo=divrem[0];
bigrem=divrem[1];
          if(bigrem.signum()!=0)
            break;
          denominator=bigquo;
        }
        return denominator.compareTo(BigInteger.ONE) == 0;
      }

    /**
     * 
     * @param bigint A BigInteger object.
     * @param power A FastInteger object.
     * @return A BigInteger object.
     */
      public BigInteger MultiplyByRadixPower(BigInteger bigint, FastInteger power) {
        if (power.signum() <= 0) return bigint;
        if (bigint.signum()==0) return bigint;
        if(bigint.compareTo(BigInteger.ONE)!=0){
          if (power.CanFitInInt32()) {
            bigint=bigint.multiply(DecimalUtility.FindPowerOfTen(power.AsInt32()));
          } else {
            bigint=bigint.multiply(DecimalUtility.FindPowerOfTenFromBig(power.AsBigInteger()));
          }
          return bigint;
        } else {
          if (power.CanFitInInt32()) {
            return (DecimalUtility.FindPowerOfTen(power.AsInt32()));
          } else {
            return (DecimalUtility.FindPowerOfTenFromBig(power.AsBigInteger()));
          }
        }
      }
      
    /**
     * 
     * @param value A DecimalFraction object.
     * @return A 32-bit signed integer.
     */
      public int GetFlags(DecimalFraction value) {
        return value.mantissa.signum()<0 ? BigNumberFlags.FlagNegative : 0;
      }
      
    /**
     * 
     * @param mantissa A BigInteger object.
     * @param exponent A BigInteger object.
     * @param flags A 32-bit signed integer.
     * @return A DecimalFraction object.
     */
      public DecimalFraction CreateNewWithFlags(BigInteger mantissa, BigInteger exponent, int flags) {
        boolean neg=(flags&BigNumberFlags.FlagNegative)!=0;
        if((neg && mantissa.signum()>0) || (!neg && mantissa.signum()<0))
          mantissa=mantissa.negate();
        return new DecimalFraction(mantissa,exponent);
      }
      
    /**
     * 
     * @return A 32-bit signed integer.
     */
      public int GetArithmeticSupport() {
        return BigNumberFlags.FiniteOnly;
      }
      
    /**
     * 
     * @param val A 32-bit signed integer.
     * @return A DecimalFraction object.
     */
      public DecimalFraction ValueOf(int val) {
        if(val==0)return Zero;
        if(val==1)return One;
        if(val==-1)return MinusOne;
        return FromInt64(val);
      }
    }

    
    private String ToStringInternal(int mode) {
      switch(mode){
        case 0:
          return new ExtendedDecimal(this.getMantissa(),this.getExponent()).toString();
        case 1:
          return new ExtendedDecimal(this.getMantissa(),this.getExponent()).ToEngineeringString();
        case 2:
          return new ExtendedDecimal(this.getMantissa(),this.getExponent()).ToPlainString();
        default:
          throw new IllegalArgumentException();
      }
    }

    /**
     * Converts this value to an arbitrary-precision integer. Any fractional
     * part in this value will be discarded when converting to a big integer.
     * @return A BigInteger object.
     */
    public BigInteger ToBigInteger() {
      return new ExtendedDecimal(this.getMantissa(),this.getExponent()).ToBigInteger();
    }
    /**
     * Converts this value to a 32-bit floating-point number. The half-even
     * rounding mode is used.
     * @return The closest 32-bit floating-point number to this value.
     * The return value can be positive infinity or negative infinity if
     * this value exceeds the range of a 32-bit floating point number.
     */
    public float ToSingle() {
      return new ExtendedDecimal(this.getMantissa(),this.getExponent()).ToSingle();
    }
    /**
     * Converts this value to a 64-bit floating-point number. The half-even
     * rounding mode is used.
     * @return The closest 64-bit floating-point number to this value.
     * The return value can be positive infinity or negative infinity if
     * this value exceeds the range of a 64-bit floating point number.
     */
    public double ToDouble() {
      return new ExtendedDecimal(this.getMantissa(),this.getExponent()).ToDouble();
    }
    /**
     * Creates a decimal fraction from a 32-bit floating-point number.
     * This method computes the exact value of the floating point number,
     * not an approximation, as is often the case by converting the number
     * to a string.
     * @param flt A 32-bit floating-point number.
     * @return A decimal fraction with the same value as &quot;flt&quot;.
     * @throws ArithmeticException "flt" is infinity or not-a-number.
     */
    public static DecimalFraction FromSingle(float flt) {
      ExtendedDecimal ed=ExtendedDecimal.FromSingle(flt);
      return new DecimalFraction(ed.getMantissa(),ed.getExponent());
    }
    
    public static DecimalFraction FromBigInteger(BigInteger bigint) {
      return new DecimalFraction(bigint,BigInteger.ZERO);
    }
    
    public static DecimalFraction FromInt64(long valueSmall) {
      BigInteger bigint=BigInteger.valueOf(valueSmall);
      return new DecimalFraction(bigint,BigInteger.ZERO);
    }

    /**
     * Creates a decimal fraction from a 64-bit floating-point number.
     * This method computes the exact value of the floating point number,
     * not an approximation, as is often the case by converting the number
     * to a string.
     * @param dbl A 64-bit floating-point number.
     * @return A decimal fraction with the same value as &quot;dbl&quot;
     * @throws ArithmeticException "dbl" is infinity or not-a-number.
     */
    public static DecimalFraction FromDouble(double dbl) {
      ExtendedDecimal ed=ExtendedDecimal.FromDouble(dbl);
      return new DecimalFraction(ed.getMantissa(),ed.getExponent());
    }

    /**
     * Creates a decimal fraction from an arbitrary-precision binary floating-point
     * number.
     * @param bigfloat A bigfloat.
     * @return A DecimalFraction object.
     */
    public static DecimalFraction FromBigFloat(BigFloat bigfloat) {
      ExtendedDecimal ed=ExtendedDecimal.FromExtendedFloat(
        new ExtendedFloat(bigfloat.getMantissa(),bigfloat.getExponent()));
      return new DecimalFraction(ed.getMantissa(),ed.getExponent());
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
     * @return A string object.
     */
    public String ToEngineeringString() {
      return ToStringInternal(1);
    }
    /**
     * Converts this value to a string, but without an exponent part. The
     * format of the return value follows the format of the java.math.BigDecimal.toPlainString()
     * method.
     * @return A string object.
     */
    public String ToPlainString() {
      return ToStringInternal(2);
    }

    /**
     * Represents the number 1.
     */
    
    public static final DecimalFraction One = new DecimalFraction(BigInteger.ONE,BigInteger.ZERO);

    /**
     * Represents the number 0.
     */
    
    public static final DecimalFraction Zero = new DecimalFraction(BigInteger.ZERO,BigInteger.ZERO);
    /**
     * Represents the number 10.
     */
    
    public static final DecimalFraction Ten = new DecimalFraction(BigInteger.TEN,BigInteger.ZERO);

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
     * @return A DecimalFraction object.
     */
    public DecimalFraction Abs() {
      return Abs(null);
    }

    /**
     * Gets an object with the same value as this one, but with the sign reversed.
     * @return A DecimalFraction object.
     */
    public DecimalFraction Negate() {
      return Negate(null);
    }

    /**
     * Divides this object by another decimal fraction and returns the result.
     * When possible, the result will be exact.
     * @param divisor The divisor.
     * @return The quotient of the two numbers.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The result would have a nonterminating
     * decimal expansion.
     */
    public DecimalFraction Divide(DecimalFraction divisor) {
      return Divide(divisor, PrecisionContext.ForRounding(Rounding.Unnecessary));
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
    public DecimalFraction DivideToSameExponent(DecimalFraction divisor, Rounding rounding) {
      return DivideToExponent(divisor, this.exponent, PrecisionContext.ForRounding(rounding));
    }

    /**
     * Divides two DecimalFraction objects, and returns the integer part
     * of the result, rounded down, with the preferred exponent set to this
     * value's exponent minus the divisor's exponent.
     * @param divisor The divisor.
     * @return The integer part of the quotient of the two objects.
     * @throws ArithmeticException Attempted to divide by zero.
     */
    public DecimalFraction DivideToIntegerNaturalScale(
      DecimalFraction divisor
     ) {
      return DivideToIntegerNaturalScale(divisor, PrecisionContext.ForRounding(Rounding.Down));
    }

    /**
     * Removes trailing zeros from this object's mantissa. For example,
     * 1.000 becomes 1.
     * @param ctx A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true, will
     * also store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return This value with trailing zeros removed. Note that if the result
     * has a very high exponent and the context says to clamp high exponents,
     * there may still be some trailing zeros in the mantissa. If a precision
     * context is given, returns null if the result of the rounding overflowed
     * the exponent range.
     */
    public DecimalFraction Reduce(
      PrecisionContext ctx) {
      return math.Reduce(this, ctx);
    }
    /**
     * 
     * @param divisor A DecimalFraction object.
     * @return A DecimalFraction object.
     */
    public DecimalFraction RemainderNaturalScale(
      DecimalFraction divisor
     ) {
      return RemainderNaturalScale(divisor,null);
    }

    /**
     * 
     * @param divisor A DecimalFraction object.
     * @param ctx A PrecisionContext object.
     * @return A DecimalFraction object.
     */
    public DecimalFraction RemainderNaturalScale(
      DecimalFraction divisor,
      PrecisionContext ctx
     ) {
      return Subtract(this.DivideToIntegerNaturalScale(divisor,null)
                      .Multiply(divisor,null),ctx);
    }

    /**
     * Divides two DecimalFraction objects, and gives a particular exponent
     * to the result.
     * @param divisor A DecimalFraction object.
     * @param desiredExponentSmall The desired exponent. A negative number
     * places the cutoff point to the right of the usual decimal point. A positive
     * number places the cutoff point to the left of the usual decimal point.
     * @param ctx A precision context object to control the rounding mode
     * to use if the result must be scaled down to have the same exponent as
     * this value. The precision setting of this context is ignored. If HasFlags
     * of the context is true, will also store the flags resulting from the
     * operation (the flags are in addition to the pre-existing flags).
     * Can be null, in which case the default rounding mode is HalfEven.
     * @return The quotient of the two objects.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The desired exponent is outside of
     * the valid range of the precision context, if it defines an exponent
     * range.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary
     * and the result is not exact.
     */
    public DecimalFraction DivideToExponent(
      DecimalFraction divisor,
      long desiredExponentSmall,
      PrecisionContext ctx
     ) {
      return DivideToExponent(divisor, (BigInteger.valueOf(desiredExponentSmall)), ctx);
    }

    /**
     * Divides two DecimalFraction objects, and gives a particular exponent
     * to the result.
     * @param divisor A DecimalFraction object.
     * @param desiredExponentSmall The desired exponent. A negative number
     * places the cutoff point to the right of the usual decimal point. A positive
     * number places the cutoff point to the left of the usual decimal point.
     * @param rounding The rounding mode to use if the result must be scaled
     * down to have the same exponent as this value.
     * @return The quotient of the two objects.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary
     * and the result is not exact.
     */
    public DecimalFraction DivideToExponent(
      DecimalFraction divisor,
      long desiredExponentSmall,
      Rounding rounding
     ) {
      return DivideToExponent(divisor, (BigInteger.valueOf(desiredExponentSmall)), PrecisionContext.ForRounding(rounding));
    }

    /**
     * Divides two DecimalFraction objects, and gives a particular exponent
     * to the result.
     * @param divisor A DecimalFraction object.
     * @param exponent The desired exponent. A negative number places the
     * cutoff point to the right of the usual decimal point. A positive number
     * places the cutoff point to the left of the usual decimal point.
     * @param ctx A precision context object to control the rounding mode
     * to use if the result must be scaled down to have the same exponent as
     * this value. The precision setting of this context is ignored. If HasFlags
     * of the context is true, will also store the flags resulting from the
     * operation (the flags are in addition to the pre-existing flags).
     * Can be null, in which case the default rounding mode is HalfEven.
     * @return The quotient of the two objects.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The desired exponent is outside of
     * the valid range of the precision context, if it defines an exponent
     * range.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary
     * and the result is not exact.
     */
    public DecimalFraction DivideToExponent(
      DecimalFraction divisor, BigInteger exponent, PrecisionContext ctx) {
      return math.DivideToExponent(this, divisor, exponent, ctx);
    }

    /**
     * Divides two DecimalFraction objects, and gives a particular exponent
     * to the result.
     * @param divisor A DecimalFraction object.
     * @param desiredExponent The desired exponent. A negative number
     * places the cutoff point to the right of the usual decimal point. A positive
     * number places the cutoff point to the left of the usual decimal point.
     * @param rounding The rounding mode to use if the result must be scaled
     * down to have the same exponent as this value.
     * @return The quotient of the two objects.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary
     * and the result is not exact.
     */
    public DecimalFraction DivideToExponent(
      DecimalFraction divisor,
      BigInteger desiredExponent,
      Rounding rounding
     ) {
      return DivideToExponent(divisor, desiredExponent, PrecisionContext.ForRounding(rounding));
    }

    /**
     * Finds the absolute value of this object (if it's negative, it becomes
     * positive).
     * @param context A precision context to control precision, rounding,
     * and exponent range of the result. If HasFlags of the context is true,
     * will also store the flags resulting from the operation (the flags
     * are in addition to the pre-existing flags). Can be null.
     * @return The absolute value of this object.
     */
    public DecimalFraction Abs(PrecisionContext context) {
      if (this.signum() < 0) {
        return Negate(context);
      } else {
        return RoundToPrecision(context);
      }
    }

    /**
     * Returns a decimal fraction with the same value as this object but with
     * the sign reversed.
     * @param context A precision context to control precision, rounding,
     * and exponent range of the result. If HasFlags of the context is true,
     * will also store the flags resulting from the operation (the flags
     * are in addition to the pre-existing flags). Can be null.
     * @return A DecimalFraction object.
     */
    public DecimalFraction Negate(PrecisionContext context) {
      BigInteger neg=(this.mantissa).negate();
      return new DecimalFraction(neg, this.exponent).RoundToPrecision(context);
    }

    /**
     * Adds this object and another decimal fraction and returns the result.
     * @param decfrac A DecimalFraction object.
     * @return The sum of the two objects.
     */
    public DecimalFraction Add(DecimalFraction decfrac) {
      if((decfrac)==null)throw new NullPointerException("decfrac");
      return Add(decfrac, PrecisionContext.Unlimited);
    }

    /**
     * Subtracts a DecimalFraction object from this instance and returns
     * the result.
     * @param decfrac A DecimalFraction object.
     * @return The difference of the two objects.
     */
    public DecimalFraction Subtract(DecimalFraction decfrac) {
      return Subtract(decfrac,null);
    }

    /**
     * Subtracts a DecimalFraction object from this instance.
     * @param decfrac A DecimalFraction object.
     * @param ctx A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true, will
     * also store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The difference of the two objects. If a precision context
     * is given, returns null if the result of the rounding overflowed the
     * exponent range.
     */
    public DecimalFraction Subtract(DecimalFraction decfrac, PrecisionContext ctx) {
      if((decfrac)==null)throw new NullPointerException("decfrac");
      return Add(decfrac.Negate(null), ctx);
    }
    /**
     * Multiplies two decimal fractions. The resulting scale will be the
     * sum of the scales of the two decimal fractions.
     * @param decfrac Another decimal fraction.
     * @return The product of the two decimal fractions. If a precision context
     * is given, returns null if the result of the rounding overflowed the
     * exponent range.
     */
    public DecimalFraction Multiply(DecimalFraction decfrac) {
      if((decfrac)==null)throw new NullPointerException("decfrac");
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
      return MultiplyAndAdd(multiplicand,augend,null);
    }
    //----------------------------------------------------------------

    private static RadixMath<DecimalFraction> math = new RadixMath<DecimalFraction>(
      new DecimalMathHelper());

    /**
     * Divides this object by another object, and returns the integer part
     * of the result, with the preferred exponent set to this value's exponent
     * minus the divisor's exponent.
     * @param divisor The divisor.
     * @param ctx A precision context object to control the precision, rounding,
     * and exponent range of the integer part of the result. Flags will be
     * set on the given context only if the context&apos;s HasFlags is true
     * and the integer part of the result doesn&apos;t fit the precision
     * and exponent range without rounding.
     * @return The integer part of the quotient of the two objects. If a precision
     * context is given, returns null if the result of the rounding overflowed
     * the exponent range.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary
     * and the integer part of the result is not exact.
     */
    public DecimalFraction DivideToIntegerNaturalScale(
      DecimalFraction divisor, PrecisionContext ctx) {
      return math.DivideToIntegerNaturalScale(this, divisor, ctx);
    }

    /**
     * Divides this object by another object, and returns the integer part
     * of the result, with the exponent set to 0.
     * @param divisor The divisor.
     * @param ctx A precision context object to control the precision. The
     * rounding and exponent range settings of this context are ignored.
     * No flags will be set from this operation even if HasFlags of the context
     * is true. Can be null.
     * @return The integer part of the quotient of the two objects. The exponent
     * will be set to 0.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The result doesn't fit the given precision.
     */
    public DecimalFraction DivideToIntegerZeroScale(
      DecimalFraction divisor, PrecisionContext ctx) {
      return math.DivideToIntegerZeroScale(this, divisor, ctx);
    }
    
    /**
     * Finds the remainder that results when dividing two DecimalFraction
     * objects. The remainder is the value that remains when the absolute
     * value of this object is divided by the absolute value of the other object;
     * the remainder has the same sign (positive or negative) as this object.
     * @param divisor The divisor.
     * @param ctx A precision context object to control the precision. The
     * rounding and exponent range settings of this context are ignored.
     * No flags will be set from this operation even if HasFlags of the context
     * is true. Can be null.
     * @return The remainder of the two objects.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The result of integer division (the
     * quotient, not the remainder) wouldn't fit the given precision.
     */
    public DecimalFraction Remainder(
      DecimalFraction divisor, PrecisionContext ctx) {
      return math.Remainder(this, divisor, ctx);
    }
    /**
     * Finds the distance to the closest multiple of the given divisor, based
     * on the result of dividing this object's value by another object's
     * value. <ul> <li> If this and the other object divide evenly, the result
     * is 0.</li> <li>If the remainder's absolute value is less than half
     * of the divisor's absolute value, the result has the same sign as this
     * object and will be the distance to the closest multiple.</li> <li>If
     * the remainder's absolute value is more than half of the divisor's
     * absolute value, the result has the opposite sign of this object and
     * will be the distance to the closest multiple.</li> <li>If the remainder's
     * absolute value is exactly half of the divisor's absolute value, the
     * result has the opposite sign of this object if the quotient, rounded
     * down, is odd, and has the same sign as this object if the quotient, rounded
     * down, is even, and the result's absolute value is half of the divisor's
     * absolute value.</li> </ul> This function is also known as the "IEEE
     * Remainder" function.
     * @param divisor The divisor.
     * @param ctx A precision context object to control the precision. The
     * rounding and exponent range settings of this context are ignored
     * (the rounding mode is always ((treated instanceof HalfEven) ? (HalfEven)treated
     * : null)). No flags will be set from this operation even if HasFlags
     * of the context is true. Can be null.
     * @return The distance of the closest multiple.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException Either the result of integer division
     * (the quotient) or the remainder wouldn't fit the given precision.
     */
    public DecimalFraction RemainderNear(
      DecimalFraction divisor, PrecisionContext ctx) {
      return math.RemainderNear(this, divisor, ctx);
    }

    /**
     * Finds the largest value that's smaller than the given value.
     * @param ctx A precision context object to control the precision and
     * exponent range of the result. The rounding mode from this context
     * is ignored. No flags will be set from this operation even if HasFlags
     * of the context is true.
     * @return Returns the largest value that&apos;s less than the given
     * value. Returns null if the result is negative infinity.
     * @throws java.lang.IllegalArgumentException "ctx" is null, the precision
     * is 0, or "ctx" has an unlimited exponent range.
     */
    public DecimalFraction NextMinus(
      PrecisionContext ctx
     ) {
      return math.NextMinus(this,ctx);
    }

    /**
     * Finds the smallest value that's greater than the given value.
     * @param ctx A precision context object to control the precision and
     * exponent range of the result. The rounding mode from this context
     * is ignored. No flags will be set from this operation even if HasFlags
     * of the context is true.
     * @return Returns the smallest value that&apos;s greater than the
     * given value. Returns null if the result is positive infinity.
     * @throws java.lang.IllegalArgumentException "ctx" is null, the precision
     * is 0, or "ctx" has an unlimited exponent range.
     */
    public DecimalFraction NextPlus(
      PrecisionContext ctx
     ) {
      return math.NextPlus(this,ctx);
    }
    
    /**
     * Finds the next value that is closer to the other object's value than
     * this object's value.
     * @param otherValue A DecimalFraction object.
     * @param ctx A precision context object to control the precision and
     * exponent range of the result. The rounding mode from this context
     * is ignored. No flags will be set from this operation even if HasFlags
     * of the context is true.
     * @return Returns the next value that is closer to the other object&apos;s
     * value than this object&apos;s value. Returns null if the result is
     * infinity.
     * @throws java.lang.IllegalArgumentException "ctx" is null, the precision
     * is 0, or "ctx" has an unlimited exponent range.
     */
    public DecimalFraction NextToward(
      DecimalFraction otherValue,
      PrecisionContext ctx
     ) {
      return math.NextToward(this,otherValue,ctx);
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
     * @return The quotient of the two objects. If a precision context is
     * given, returns null if the result of the rounding overflowed the exponent
     * range.
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
     * Gets the greater value between two decimal fractions.
     * @param first A DecimalFraction object.
     * @param second A DecimalFraction object.
     * @return The larger value of the two objects.
     */
    public static DecimalFraction Max(
      DecimalFraction first, DecimalFraction second) {
      return math.Max(first, second, null);
    }

    /**
     * Gets the lesser value between two decimal fractions.
     * @param first A DecimalFraction object.
     * @param second A DecimalFraction object.
     * @return The smaller value of the two objects.
     */
    public static DecimalFraction Min(
      DecimalFraction first, DecimalFraction second) {
      return math.Min(first, second, null);
    }
    /**
     * Gets the greater value between two values, ignoring their signs.
     * If the absolute values are equal, has the same effect as Max.
     * @param first A DecimalFraction object.
     * @param second A DecimalFraction object.
     * @return A DecimalFraction object.
     */
    public static DecimalFraction MaxMagnitude(
      DecimalFraction first, DecimalFraction second) {
      return math.MaxMagnitude(first, second, null);
    }
    
    /**
     * Gets the lesser value between two values, ignoring their signs. If
     * the absolute values are equal, has the same effect as Min.
     * @param first A DecimalFraction object.
     * @param second A DecimalFraction object.
     * @return A DecimalFraction object.
     */
    public static DecimalFraction MinMagnitude(
      DecimalFraction first, DecimalFraction second) {
      return math.MinMagnitude(first, second, null);
    }
    /**
     * Compares the mathematical values of this object and another object.
     * <p> This method is not consistent with the Equals method because two
     * different decimal fractions with the same mathematical value, but
     * different exponents, will compare as equal.</p>
     * @param other A DecimalFraction object.
     * @return Less than 0 if this object&apos;s value is less than the other
     * value, or greater than 0 if this object&apos;s value is greater than
     * the other value or if &quot;other&quot; is null, or 0 if both values
     * are equal.
     */
    public int compareTo(
      DecimalFraction other) {
      return math.compareTo(this, other);
    }

    /**
     * Compares a DecimalFraction object with this instance.
     * @param other A DecimalFraction object.
     * @param ctx A PrecisionContext object.
     * @return A DecimalFraction object.
     */
    public DecimalFraction CompareToWithContext(
      DecimalFraction other, PrecisionContext ctx) {
      return math.CompareToWithContext(this, other, false, ctx);
    }

    /**
     * Compares a DecimalFraction object with this instance.
     * @param other A DecimalFraction object.
     * @param ctx A PrecisionContext object.
     * @return A DecimalFraction object.
     */
    public DecimalFraction CompareToSignal(
      DecimalFraction other, PrecisionContext ctx) {
      return math.CompareToWithContext(this, other, true, ctx);
    }

    /**
     * Finds the sum of this object and another object. The result's exponent
     * is set to the lower of the exponents of the two operands.
     * @param decfrac The number to add to.
     * @param ctx A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true, will
     * also store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The sum of thisValue and the other object. If a precision context
     * is given, returns null if the result of the rounding overflowed the
     * exponent range.
     */
    public DecimalFraction Add(
      DecimalFraction decfrac, PrecisionContext ctx) {
      return math.Add(this, decfrac, ctx);
    }

    /**
     * Returns a decimal fraction with the same value but a new exponent.
     * @param desiredExponent The desired exponent of the result.
     * @param ctx A precision context to control precision and rounding
     * of the result. If HasFlags of the context is true, will also store the
     * flags resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null, in which case the default rounding
     * mode is HalfEven.
     * @return A decimal fraction with the same value as this object but with
     * the exponent changed.
     * @throws ArithmeticException An overflow error occurred, or the
     * result can't fit the given precision without rounding
     * @throws java.lang.IllegalArgumentException The exponent is outside of the
     * valid range of the precision context, if it defines an exponent range.
     */
    public DecimalFraction Quantize(
      BigInteger desiredExponent, PrecisionContext ctx) {
      return Quantize(new DecimalFraction(BigInteger.ONE,desiredExponent), ctx);
    }

    /**
     * Returns a decimal fraction with the same value but a new exponent.
     * @param ctx A precision context to control precision and rounding
     * of the result. If HasFlags of the context is true, will also store the
     * flags resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null, in which case the default rounding
     * mode is HalfEven.
     * @param desiredExponentSmall A 32-bit signed integer.
     * @return A decimal fraction with the same value as this object but with
     * the exponent changed.
     * @throws ArithmeticException An overflow error occurred, or the
     * result can't fit the given precision without rounding
     * @throws java.lang.IllegalArgumentException The exponent is outside of the
     * valid range of the precision context, if it defines an exponent range.
     */
    public DecimalFraction Quantize(
      int desiredExponentSmall, PrecisionContext ctx) {
      return Quantize(new DecimalFraction(BigInteger.ONE,BigInteger.valueOf(desiredExponentSmall)), ctx);
    }

    /**
     * Returns a decimal fraction with the same value as this object but with
     * the same exponent as another decimal fraction.
     * @param otherValue A decimal fraction containing the desired exponent
     * of the result. The mantissa is ignored.
     * @param ctx A precision context to control precision and rounding
     * of the result. If HasFlags of the context is true, will also store the
     * flags resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null, in which case the default rounding
     * mode is HalfEven.
     * @return A decimal fraction with the same value as this object but with
     * the exponent changed.
     * @throws ArithmeticException An overflow error occurred, or the
     * result can't fit the given precision without rounding.
     * @throws java.lang.IllegalArgumentException The new exponent is outside of
     * the valid range of the precision context, if it defines an exponent
     * range.
     */
    public DecimalFraction Quantize(
      DecimalFraction otherValue, PrecisionContext ctx) {
      return math.Quantize(this, otherValue, ctx);
    }
    /**
     * Returns a decimal fraction with the same value as this object but rounded
     * to an integer.
     * @param ctx A precision context to control precision and rounding
     * of the result. If HasFlags of the context is true, will also store the
     * flags resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null, in which case the default rounding
     * mode is HalfEven.
     * @return A decimal fraction with the same value as this object but rounded
     * to an integer.
     * @throws ArithmeticException An overflow error occurred, or the
     * result can't fit the given precision without rounding.
     * @throws java.lang.IllegalArgumentException The new exponent must be changed
     * to 0 when rounding and 0 is outside of the valid range of the precision
     * context, if it defines an exponent range.
     */
    public DecimalFraction RoundToIntegralExact(
      PrecisionContext ctx) {
      return math.RoundToExponentExact(this, BigInteger.ZERO, ctx);
    }
    /**
     * Returns a decimal fraction with the same value as this object but rounded
     * to an integer, without adding the FlagInexact or FlagRounded flags.
     * @param ctx A precision context to control precision and rounding
     * of the result. If HasFlags of the context is true, will also store the
     * flags resulting from the operation (the flags are in addition to the
     * pre-existing flags), except that this function will never add the
     * FlagRounded and FlagInexact flags (the only difference between
     * this and RoundToExponentExact). Can be null, in which case the default
     * rounding mode is HalfEven.
     * @return A decimal fraction with the same value as this object but rounded
     * to an integer.
     * @throws ArithmeticException An overflow error occurred, or the
     * result can't fit the given precision without rounding.
     * @throws java.lang.IllegalArgumentException The new exponent must be changed
     * to 0 when rounding and 0 is outside of the valid range of the precision
     * context, if it defines an exponent range.
     */
    public DecimalFraction RoundToIntegralNoRoundedFlag(
      PrecisionContext ctx) {
      return math.RoundToExponentNoRoundedFlag(this, BigInteger.ZERO, ctx);
    }
    /**
     * Returns a decimal fraction with the same value as this object but rounded
     * to a given exponent.
     * @param exponent The minimum exponent the result can have. This is
     * the maximum number of fractional digits in the result, expressed
     * as a negative number. Can also be positive, which eliminates lower-order
     * places from the number. For example, -3 means round to the thousandth
     * (10^-3), and 3 means round to the thousand (10^3). A value of 0 rounds
     * the number to an integer.
     * @param ctx A precision context to control precision and rounding
     * of the result. If HasFlags of the context is true, will also store the
     * flags resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null, in which case the default rounding
     * mode is HalfEven.
     * @return A decimal fraction rounded to the given exponent.
     * @throws ArithmeticException An overflow error occurred, or the
     * result can't fit the given precision without rounding.
     * @throws java.lang.IllegalArgumentException The new exponent must be changed
     * when rounding and the new exponent is outside of the valid range of
     * the precision context, if it defines an exponent range.
     */
    public DecimalFraction RoundToExponentExact(
      BigInteger exponent, PrecisionContext ctx) {
      return math.RoundToExponentExact(this, exponent, ctx);
    }
    /**
     * Returns a decimal fraction with the same value as this object but rounded
     * to a given exponent, without throwing an exception if the result overflows
     * or doesn't fit the precision range.
     * @param exponent The minimum exponent the result can have. This is
     * the maximum number of fractional digits in the result, expressed
     * as a negative number. Can also be positive, which eliminates lower-order
     * places from the number. For example, -3 means round to the thousandth
     * (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A
     * value of 0 rounds the number to an integer.
     * @param ctx A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true, will
     * also store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null, in which case the
     * default rounding mode is HalfEven.
     * @return A decimal fraction rounded to the closest value representable
     * in the given precision, meaning if the result can&apos;t fit the precision,
     * additional digits are discarded to make it fit. If a precision context
     * is given, returns null if the result of the rounding overflowed the
     * exponent range.
     * @throws java.lang.IllegalArgumentException The new exponent must be changed
     * when rounding and the new exponent is outside of the valid range of
     * the precision context, if it defines an exponent range.
     */
    public DecimalFraction RoundToExponent(
      BigInteger exponent, PrecisionContext ctx) {
      return math.RoundToExponentSimple(this, exponent, ctx);
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
     * is given, returns null if the result of the rounding overflowed the
     * exponent range.
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
     * context is given, returns null if the result of the rounding overflowed
     * the exponent range.
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
     * @return The closest value to this object&apos;s value, rounded to
     * the specified precision. Returns the same value as this object if
     * &quot;context&quot; is null or the precision and exponent range
     * are unlimited. If a precision context is given, returns null if the
     * result of the rounding overflowed the exponent range.
     */
    public DecimalFraction RoundToPrecision(
      PrecisionContext ctx) {
      return math.RoundToPrecision(this, ctx);
    }

    /**
     * Rounds this object's value to a given maximum bit length, using the
     * given rounding mode and range of exponent.
     * @param ctx A context for controlling the precision, rounding mode,
     * and exponent range. The precision is interpreted as the maximum bit
     * length of the mantissa. Can be null.
     * @return The closest value to this object&apos;s value, rounded to
     * the specified precision. Returns the same value as this object if
     * &quot;context&quot; is null or the precision and exponent range
     * are unlimited. Returns null if the result of the rounding overflowed
     * the exponent range.
     */
    public DecimalFraction RoundToBinaryPrecision(
      PrecisionContext ctx) {
      return math.RoundToBinaryPrecision(this, ctx);
    }

  }
