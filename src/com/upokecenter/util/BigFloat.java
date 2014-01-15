package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

// import java.math.*;

    /**
     * Represents an arbitrary-precision binary floating-point number.
     * Consists of an integer mantissa and an integer exponent, both arbitrary-precision.
     * The value of the number is equal to mantissa * 2^exponent.
     * @deprecated Use ExtendedFloat instead, which supports more kinds of values than BigFloat.
 */
@Deprecated
  public final class BigFloat implements Comparable<BigFloat> {
    BigInteger exponent;
    BigInteger mantissa;
    /**
     * Gets this object's exponent. This object's value will be an integer
     * if the exponent is positive or zero.
     */
    public BigInteger getExponent() { return this.exponent; }
    /**
     * Gets this object's unscaled value.
     */
    public BigInteger getMantissa() { return this.mantissa; }

    /**
     * Determines whether this object's mantissa and exponent are equal
     * to those of another object.
     * @param other A BigFloat object.
     */
    private boolean EqualsInternal(BigFloat other) {
      BigFloat otherValue = ((other instanceof BigFloat) ? (BigFloat)other : null);
      if (otherValue == null)
        return false;
      return this.exponent.equals(otherValue.exponent) &&
        this.mantissa.equals(otherValue.mantissa);
    }
    /**
     * Not documented yet.
     * @param other A BigFloat object.
     * @return A Boolean object.
     */
    public boolean equals(BigFloat other) {
      return this.EqualsInternal(other);
    }
    /**
     * Determines whether this object's mantissa and exponent are equal
     * to those of another object and that object is a Bigfloat.
     * @param obj An arbitrary object.
     * @return True if the objects are equal; false otherwise.
     */
    @Override public boolean equals(Object obj) {
      return this.EqualsInternal(((obj instanceof BigFloat) ? (BigFloat)obj : null));
    }
    /**
     * Calculates this object's hash code.
     * @return This object&apos; s hash code.
     */
    @Override public int hashCode() {
      int hashCode_ = 0;
      {
        hashCode_ += 1000000007 * this.exponent.hashCode();
        hashCode_ += 1000000009 * this.mantissa.hashCode();
      }
      return hashCode_;
    }

    /**
     * Creates a bigfloat with the value exponent*2^mantissa.
     * @param mantissa The unscaled value.
     * @param exponent The binary exponent.
     */
    public BigFloat (BigInteger mantissa, BigInteger exponent) {
      this.exponent = exponent;
      this.mantissa = mantissa;
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

    private static BigInteger ShiftLeftInt(BigInteger val, int shift) {
      while (shift > ShiftIteration) {
        val=val.shiftLeft(1000000);
        shift -= ShiftIteration;
      }
      int lastshift = (int)shift;
      val=val.shiftLeft(lastshift);
      return val;
    }

    /**
     * Not documented yet.
     * @param bigint A BigInteger object.
     * @return A BigFloat object.
     */
    public static BigFloat FromBigInteger(BigInteger bigint) {
      return new BigFloat(bigint, BigInteger.ZERO);
    }

    /**
     * Not documented yet.
     * @param numberValue A 64-bit signed integer.
     * @return A BigFloat object.
     */
    public static BigFloat FromInt64(long numberValue) {
      BigInteger bigint = BigInteger.valueOf(numberValue);
      return new BigFloat(bigint, BigInteger.ZERO);
    }

    public static BigFloat FromExtendedFloat(ExtendedFloat ef) {
      if (ef.IsNaN() || ef.IsInfinity())throw new ArithmeticException("Is NaN or infinity");
      return new BigFloat(ef.getMantissa(), ef.getExponent());
    }

    /**
     * Creates a bigfloat from its string representation. Note that if the
     * bigfloat contains a negative exponent, the resulting value might
     * not be exact. However, it will contain enough precision to accurately
     * convert it to a 32-bit or 64-bit floating point number (float or double).
     * @param str A string object.
     * @return A BigFloat object.
     */
    public static BigFloat FromString(String str) {
      return BigFloat.FromExtendedFloat(ExtendedDecimal.FromString(str).ToExtendedFloat());
    }

    /**
     * Creates a bigfloat from a 32-bit floating-point number.
     * @param flt A 32-bit floating-point number.
     * @return A bigfloat with the same value as &quot; flt&quot; .
     * @throws ArithmeticException &quot;flt&quot; is infinity or not-a-number.
     */
    public static BigFloat FromSingle(float flt) {
      return BigFloat.FromExtendedFloat(ExtendedFloat.FromSingle(flt));
    }
    /**
     * Creates a bigfloat from a 64-bit floating-point number.
     * @param dbl A 64-bit floating-point number.
     * @return A bigfloat with the same value as &quot; dbl&quot;.
     * @throws ArithmeticException &quot;dbl&quot; is infinity or not-a-number.
     */
    public static BigFloat FromDouble(double dbl) {
      return BigFloat.FromExtendedFloat(ExtendedFloat.FromDouble(dbl));
    }
    /**
     * Converts this value to an arbitrary-precision integer. Any fractional
     * part in this value will be discarded when converting to a big integer.
     * @return A BigInteger object.
     */
    public BigInteger ToBigInteger() {
      return ExtendedFloat.Create(this.getMantissa(), this.getExponent()).ToBigInteger();
    }

    /**
     * Converts this value to a 32-bit floating-point number. The half-even
     * rounding mode is used.
     * @return The closest 32-bit floating-point number to this value.
     * The return value can be positive infinity or negative infinity if
     * this value exceeds the range of a 32-bit floating point number.
     */
    public float ToSingle() {
      return ExtendedFloat.Create(this.getMantissa(), this.getExponent()).ToSingle();
    }

    /**
     * Converts this value to a 64-bit floating-point number. The half-even
     * rounding mode is used.
     * @return The closest 64-bit floating-point number to this value.
     * The return value can be positive infinity or negative infinity if
     * this value exceeds the range of a 64-bit floating point number.
     */
    public double ToDouble() {
      return ExtendedFloat.Create(this.getMantissa(), this.getExponent()).ToDouble();
    }
    /**
     * Converts this value to a string.The format of the return value is exactly
     * the same as that of the java.math.BigDecimal.toString() method.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return ExtendedFloat.Create(this.getMantissa(), this.getExponent()).toString();
    }
    /**
     * Same as toString(), except that when an exponent is used it will be
     * a multiple of 3. The format of the return value follows the format of
     * the java.math.BigDecimal.toEngineeringString() method.
     * @return A string object.
     */
    public String ToEngineeringString() {
      return ExtendedFloat.Create(this.getMantissa(), this.getExponent()).ToEngineeringString();
    }
    /**
     * Converts this value to a string, but without an exponent part. The
     * format of the return value follows the format of the java.math.BigDecimal.toPlainString()
     * method.
     * @return A string object.
     */
    public String ToPlainString() {
      return ExtendedFloat.Create(this.getMantissa(), this.getExponent()).ToPlainString();
    }

    /**
     * Represents the number 1.
     */

    public static final BigFloat One = new BigFloat(BigInteger.ONE, BigInteger.ZERO);

    /**
     * Represents the number 0.
     */

    public static final BigFloat Zero = new BigFloat(BigInteger.ZERO, BigInteger.ZERO);
    /**
     * Represents the number 10.
     */

    public static final BigFloat Ten = FromInt64((long)10);

    private static final class BinaryMathHelper implements IRadixMathHelper<BigFloat> {

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int GetRadix() {
        return 2;
      }

    /**
     * Not documented yet.
     * @param value A BigFloat object.
     * @return A 32-bit signed integer.
     */
      public int GetSign(BigFloat value) {
        return value.signum();
      }

    /**
     * Not documented yet.
     * @param value A BigFloat object.
     * @return A BigInteger object.
     */
      public BigInteger GetMantissa(BigFloat value) {
        return value.mantissa;
      }

    /**
     * Not documented yet.
     * @param value A BigFloat object.
     * @return A BigInteger object.
     */
      public BigInteger GetExponent(BigFloat value) {
        return value.exponent;
      }

    /**
     * Not documented yet.
     * @param mantissa A BigInteger object. (2)
     * @param e1 A BigInteger object. (3)
     * @param e2 A BigInteger object. (4)
     * @return A BigInteger object.
     */
      public BigInteger RescaleByExponentDiff(BigInteger mantissa, BigInteger e1, BigInteger e2) {
        boolean negative = mantissa.signum() < 0;
        if (negative) mantissa=mantissa.negate();
        BigInteger diff = (e1.subtract(e2)).abs();
        mantissa = ShiftLeft(mantissa, diff);
        if (negative) mantissa=mantissa.negate();
        return mantissa;
      }

    /**
     * Not documented yet.
     * @param lastDigit A 32-bit signed integer.
     * @param olderDigits A 32-bit signed integer. (2)
     * @param bigint A BigInteger object.
     * @return An IShiftAccumulator object.
     */
      public IShiftAccumulator CreateShiftAccumulatorWithDigits(BigInteger bigint, int lastDigit, int olderDigits) {
        return new BitShiftAccumulator(bigint, lastDigit, olderDigits);
      }

    /**
     * Not documented yet.
     * @param bigint A BigInteger object.
     * @return An IShiftAccumulator object.
     */
      public IShiftAccumulator CreateShiftAccumulator(BigInteger bigint) {
        return new BitShiftAccumulator(bigint, 0, 0);
      }

    /**
     * Not documented yet.
     * @param num A BigInteger object.
     * @param den A BigInteger object. (2)
     * @return A Boolean object.
     */
      public boolean HasTerminatingRadixExpansion(BigInteger num, BigInteger den) {
        BigInteger gcd = num.gcd(den);
        if (gcd.signum()==0) { return false; }
        den=den.divide(gcd);
        while (den.testBit(0)==false) {
          den=den.shiftRight(1);
        }
        return den.equals(BigInteger.ONE);
      }

    /**
     * Not documented yet.
     * @param bigint A BigInteger object. (2)
     * @param power A FastInteger object.
     * @return A BigInteger object.
     */
      public BigInteger MultiplyByRadixPower(BigInteger bigint, FastInteger power) {
        if (power.signum() <= 0) { return bigint; }
        if (power.CanFitInInt32()) {
          return ShiftLeftInt(bigint, power.AsInt32());
        } else {
          return ShiftLeft(bigint, power.AsBigInteger());
        }
      }

    /**
     * Not documented yet.
     * @param value A BigFloat object.
     * @return A 32-bit signed integer.
     */
      public int GetFlags(BigFloat value) {
        return value.mantissa.signum() < 0 ? BigNumberFlags.FlagNegative : 0;
      }

    /**
     * Not documented yet.
     * @param mantissa A BigInteger object.
     * @param exponent A BigInteger object. (2)
     * @param flags A 32-bit signed integer.
     * @return A BigFloat object.
     */
      public BigFloat CreateNewWithFlags(BigInteger mantissa, BigInteger exponent, int flags) {
        boolean neg = (flags & BigNumberFlags.FlagNegative) != 0;
        if ((neg && mantissa.signum() > 0) || (!neg && mantissa.signum() < 0))
          mantissa=mantissa.negate();
        return new BigFloat(mantissa, exponent);
      }
    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int GetArithmeticSupport() {
        return BigNumberFlags.FiniteOnly;
      }

    /**
     * Not documented yet.
     * @param val A 32-bit signed integer.
     * @return A BigFloat object.
     */
      public BigFloat ValueOf(int val) {
        return FromInt64(val);
      }
    }

    //----------------------------
    /**
     * Gets this value's sign: -1 if negative; 1 if positive; 0 if zero.
     */
    public int signum() {
        return this.mantissa.signum();
      }
    /**
     * Gets whether this object's value equals 0.
     */
    public boolean isZero() {
        return this.mantissa.signum()==0;
      }
    /**
     * Gets the absolute value of this object.
     * @return A BigFloat object.
     */
    public BigFloat Abs() {
      return this.Abs(null);
    }

    /**
     * Gets an object with the same value as this one, but with the sign reversed.
     * @return A BigFloat object.
     */
    public BigFloat Negate() {
      return this.Negate(null);
    }

    /**
     * Divides this object by another bigfloat and returns the result. When
     * possible, the result will be exact.
     * @param divisor The divisor.
     * @return The quotient of the two numbers.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The result would have a nonterminating
     * decimal expansion.
     */
    public BigFloat Divide(BigFloat divisor) {
      return this.Divide(divisor, PrecisionContext.ForRounding(Rounding.Unnecessary));
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
    public BigFloat DivideToSameExponent(BigFloat divisor, Rounding rounding) {
      return this.DivideToExponent(divisor, this.exponent, PrecisionContext.ForRounding(rounding));
    }

    /**
     * Divides two BigFloat objects, and returns the integer part of the
     * result, rounded down, with the preferred exponent set to this value's
     * exponent minus the divisor's exponent.
     * @param divisor The divisor.
     * @return The integer part of the quotient of the two objects.
     * @throws ArithmeticException Attempted to divide by zero.
     */
    public BigFloat DivideToIntegerNaturalScale(
      BigFloat divisor) {
      return this.DivideToIntegerNaturalScale(divisor, PrecisionContext.ForRounding(Rounding.Down));
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
    public BigFloat Reduce(
      PrecisionContext ctx) {
      return math.Reduce(this, ctx);
    }
    /**
     * Not documented yet.
     * @param divisor A BigFloat object. (2)
     * @return A BigFloat object.
     */
    public BigFloat RemainderNaturalScale(
      BigFloat divisor) {
      return this.RemainderNaturalScale(divisor, null);
    }

    /**
     * Not documented yet.
     * @param divisor A BigFloat object. (2)
     * @param ctx A PrecisionContext object.
     * @return A BigFloat object.
     */
    public BigFloat RemainderNaturalScale(
      BigFloat divisor,
      PrecisionContext ctx) {
      return this.Subtract(
this.DivideToIntegerNaturalScale(divisor, null)
                      .Multiply(divisor, null), ctx);
    }

    /**
     * Divides two BigFloat objects, and gives a particular exponent to
     * the result.
     * @param divisor A BigFloat object.
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
    public BigFloat DivideToExponent(
      BigFloat divisor,
      long desiredExponentSmall,
      PrecisionContext ctx) {
      return this.DivideToExponent(divisor, (BigInteger)desiredExponentSmall, ctx);
    }

    /**
     * Divides two BigFloat objects, and gives a particular exponent to
     * the result.
     * @param divisor A BigFloat object.
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
    public BigFloat DivideToExponent(
      BigFloat divisor,
      long desiredExponentSmall,
      Rounding rounding) {
      return this.DivideToExponent(divisor, (BigInteger)desiredExponentSmall, PrecisionContext.ForRounding(rounding));
    }

    /**
     * Divides two BigFloat objects, and gives a particular exponent to
     * the result.
     * @param divisor A BigFloat object.
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
    public BigFloat DivideToExponent(
      BigFloat divisor, BigInteger exponent, PrecisionContext ctx) {
      return math.DivideToExponent(this, divisor, exponent, ctx);
    }

    /**
     * Divides two BigFloat objects, and gives a particular exponent to
     * the result.
     * @param divisor A BigFloat object.
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
    public BigFloat DivideToExponent(
      BigFloat divisor,
      BigInteger desiredExponent,
      Rounding rounding) {
      return this.DivideToExponent(divisor, desiredExponent, PrecisionContext.ForRounding(rounding));
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
    public BigFloat Abs(PrecisionContext context) {
      if (this.signum() < 0) {
        return this.Negate(context);
      } else {
        return this.RoundToPrecision(context);
      }
    }

    /**
     * Returns a bigfloat with the same value as this object but with the sign
     * reversed.
     * @param context A precision context to control precision, rounding,
     * and exponent range of the result. If HasFlags of the context is true,
     * will also store the flags resulting from the operation (the flags
     * are in addition to the pre-existing flags). Can be null.
     * @return A BigFloat object.
     */
    public BigFloat Negate(PrecisionContext context) {
      BigInteger neg=(this.mantissa).negate();
      return new BigFloat(neg, this.exponent).RoundToPrecision(context);
    }

    /**
     * Adds this object and another bigfloat and returns the result.
     * @param decfrac A BigFloat object.
     * @return The sum of the two objects.
     */
    public BigFloat Add(BigFloat decfrac) {
      if (decfrac == null) { throw new NullPointerException("decfrac"); }
      return this.Add(decfrac, PrecisionContext.Unlimited);
    }

    /**
     * Subtracts a BigFloat object from this instance and returns the result.
     * @param decfrac A BigFloat object.
     * @return The difference of the two objects.
     */
    public BigFloat Subtract(BigFloat decfrac) {
      return this.Subtract(decfrac, null);
    }

    /**
     * Subtracts a BigFloat object from this instance.
     * @param decfrac A BigFloat object.
     * @param ctx A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true, will
     * also store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The difference of the two objects. If a precision context
     * is given, returns null if the result of the rounding overflowed the
     * exponent range.
     */
    public BigFloat Subtract(BigFloat decfrac, PrecisionContext ctx) {
      if (decfrac == null) { throw new NullPointerException("decfrac"); }
      return this.Add(decfrac.Negate(null), ctx);
    }
    /**
     * Multiplies two bigfloats. The resulting scale will be the sum of the
     * scales of the two bigfloats.
     * @param decfrac Another bigfloat.
     * @return The product of the two bigfloats. If a precision context is
     * given, returns null if the result of the rounding overflowed the exponent
     * range.
     */
    public BigFloat Multiply(BigFloat decfrac) {
      if (decfrac == null) { throw new NullPointerException("decfrac"); }
      return this.Multiply(decfrac, PrecisionContext.Unlimited);
    }

    /**
     * Multiplies by one bigfloat, and then adds another bigfloat.
     * @param multiplicand The value to multiply.
     * @param augend The value to add.
     * @return The result this * multiplicand + augend.
     */
    public BigFloat MultiplyAndAdd(
BigFloat multiplicand,
                                   BigFloat augend) {
      return this.MultiplyAndAdd(multiplicand, augend, null);
    }
    //----------------------------------------------------------------

    private static RadixMath<BigFloat> math = new RadixMath<BigFloat>(
      new BinaryMathHelper());

    /**
     * Divides this object by another object, and returns the integer part
     * of the result, with the preferred exponent set to this value's exponent
     * minus the divisor's exponent.
     * @param divisor The divisor.
     * @param ctx A precision context object to control the precision, rounding,
     * and exponent range of the integer part of the result. Flags will be
     * set on the given context only if the context&apos; s HasFlags is true
     * and the integer part of the result doesn&apos; t fit the precision
     * and exponent range without rounding.
     * @return The integer part of the quotient of the two objects. If a precision
     * context is given, returns null if the result of the rounding overflowed
     * the exponent range.
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
     * rounding and exponent range settings of this context are ignored.
     * No flags will be set from this operation even if HasFlags of the context
     * is true. Can be null.
     * @return The integer part of the quotient of the two objects. The exponent
     * will be set to 0.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The result doesn&apos;t fit the given
     * precision.
     */
    public BigFloat DivideToIntegerZeroScale(
      BigFloat divisor, PrecisionContext ctx) {
      return math.DivideToIntegerZeroScale(this, divisor, ctx);
    }

    /**
     * Finds the remainder that results when dividing two BigFloat objects.
     * The remainder is the value that remains when the absolute value of
     * this object is divided by the absolute value of the other object; the
     * remainder has the same sign (positive or negative) as this object.
     * @param divisor The divisor.
     * @param ctx A precision context object to control the precision. The
     * rounding and exponent range settings of this context are ignored.
     * No flags will be set from this operation even if HasFlags of the context
     * is true. Can be null.
     * @return The remainder of the two objects.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException The result of integer division (the
     * quotient, not the remainder) wouldn&apos;t fit the given precision.
     */
    public BigFloat Remainder(
      BigFloat divisor, PrecisionContext ctx) {
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
     * (the quotient) or the remainder wouldn&apos;t fit the given precision.
     */
    public BigFloat RemainderNear(
      BigFloat divisor, PrecisionContext ctx) {
      return math.RemainderNear(this, divisor, ctx);
    }

    /**
     * Finds the largest value that's smaller than the given value.
     * @param ctx A precision context object to control the precision and
     * exponent range of the result. The rounding mode from this context
     * is ignored. No flags will be set from this operation even if HasFlags
     * of the context is true.
     * @return Returns the largest value that&apos; s less than the given
     * value. Returns null if the result is negative infinity.
     * @throws java.lang.IllegalArgumentException &quot;ctx&quot; is null, the
     * precision is 0, or &quot;ctx&quot; has an unlimited exponent range.
     */
    public BigFloat NextMinus(
      PrecisionContext ctx) {
      return math.NextMinus(this, ctx);
    }

    /**
     * Finds the smallest value that's greater than the given value.
     * @param ctx A precision context object to control the precision and
     * exponent range of the result. The rounding mode from this context
     * is ignored. No flags will be set from this operation even if HasFlags
     * of the context is true.
     * @return Returns the smallest value that&apos; s greater than the
     * given value. Returns null if the result is positive infinity.
     * @throws java.lang.IllegalArgumentException &quot;ctx&quot; is null, the
     * precision is 0, or &quot;ctx&quot; has an unlimited exponent range.
     */
    public BigFloat NextPlus(
      PrecisionContext ctx) {
      return math.NextPlus(this, ctx);
    }

    /**
     * Finds the next value that is closer to the other object's value than
     * this object's value.
     * @param otherValue A BigFloat object.
     * @param ctx A precision context object to control the precision and
     * exponent range of the result. The rounding mode from this context
     * is ignored. No flags will be set from this operation even if HasFlags
     * of the context is true.
     * @return Returns the next value that is closer to the other object&apos;
     * s value than this object&apos; s value. Returns null if the result
     * is infinity.
     * @throws java.lang.IllegalArgumentException &quot;ctx&quot; is null, the
     * precision is 0, or &quot;ctx&quot; has an unlimited exponent range.
     */
    public BigFloat NextToward(
      BigFloat otherValue,
      PrecisionContext ctx) {
      return math.NextToward(this, otherValue, ctx);
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
     * @return The quotient of the two objects. If a precision context is
     * given, returns null if the result of the rounding overflowed the exponent
     * range.
     * @throws ArithmeticException Attempted to divide by zero.
     * @throws ArithmeticException Either ctx is null or ctx&apos;s precision
     * is 0, and the result would have a nonterminating decimal expansion;
     * or, the rounding mode is Rounding.Unnecessary and the result is not
     * exact.
     */
    public BigFloat Divide(
      BigFloat divisor,
      PrecisionContext ctx) {
      return math.Divide(this, divisor, ctx);
    }

    /**
     * Gets the greater value between two bigfloats.
     * @param first A BigFloat object.
     * @param second A BigFloat object. (2)
     * @return The larger value of the two objects.
     */
    public static BigFloat Max(
      BigFloat first, BigFloat second) {
      return math.Max(first, second, null);
    }

    /**
     * Gets the lesser value between two bigfloats.
     * @param first A BigFloat object.
     * @param second A BigFloat object. (2)
     * @return The smaller value of the two objects.
     */
    public static BigFloat Min(
      BigFloat first, BigFloat second) {
      return math.Min(first, second, null);
    }
    /**
     * Gets the greater value between two values, ignoring their signs.
     * If the absolute values are equal, has the same effect as Max.
     * @param first A BigFloat object. (2)
     * @param second A BigFloat object. (3)
     * @return A BigFloat object.
     */
    public static BigFloat MaxMagnitude(
      BigFloat first, BigFloat second) {
      return math.MaxMagnitude(first, second, null);
    }

    /**
     * Gets the lesser value between two values, ignoring their signs. If
     * the absolute values are equal, has the same effect as Min.
     * @param first A BigFloat object. (2)
     * @param second A BigFloat object. (3)
     * @return A BigFloat object.
     */
    public static BigFloat MinMagnitude(
      BigFloat first, BigFloat second) {
      return math.MinMagnitude(first, second, null);
    }
    /**
     * Compares the mathematical values of this object and another object.
     * <p> This method is not consistent with the Equals method because two
     * different bigfloats with the same mathematical value, but different
     * exponents, will compare as equal.</p>
     * @param other A BigFloat object.
     * @return Less than 0 if this object&apos; s value is less than the other
     * value, or greater than 0 if this object&apos; s value is greater than
     * the other value or if &quot; other&quot; is null, or 0 if both values
     * are equal.
     */
    public int compareTo(
      BigFloat other) {
      return math.compareTo(this, other);
    }

    /**
     * Compares a BigFloat object with this instance.
     * @param other A BigFloat object. (2)
     * @param ctx A PrecisionContext object.
     * @return A BigFloat object.
     */
    public BigFloat CompareToWithContext(
      BigFloat other, PrecisionContext ctx) {
      return math.CompareToWithContext(this, other, false, ctx);
    }

    /**
     * Compares a BigFloat object with this instance.
     * @param other A BigFloat object. (2)
     * @param ctx A PrecisionContext object.
     * @return A BigFloat object.
     */
    public BigFloat CompareToSignal(
      BigFloat other, PrecisionContext ctx) {
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
    public BigFloat Add(
      BigFloat decfrac, PrecisionContext ctx) {
      return math.Add(this, decfrac, ctx);
    }

    /**
     * Returns a bigfloat with the same value but a new exponent.
     * @param desiredExponent The desired exponent of the result.
     * @param ctx A precision context to control precision and rounding
     * of the result. If HasFlags of the context is true, will also store the
     * flags resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null, in which case the default rounding
     * mode is HalfEven.
     * @return A bigfloat with the same value as this object but with the exponent
     * changed.
     * @throws ArithmeticException An overflow error occurred, or the
     * result can&apos;t fit the given precision without rounding.
     * @throws java.lang.IllegalArgumentException The exponent is outside of the
     * valid range of the precision context, if it defines an exponent range.
     */
    public BigFloat Quantize(
      BigInteger desiredExponent, PrecisionContext ctx) {
      return this.Quantize(new BigFloat(BigInteger.ONE, desiredExponent), ctx);
    }

    /**
     * Returns a bigfloat with the same value but a new exponent.
     * @param ctx A precision context to control precision and rounding
     * of the result. If HasFlags of the context is true, will also store the
     * flags resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null, in which case the default rounding
     * mode is HalfEven.
     * @param desiredExponentSmall A 32-bit signed integer.
     * @return A bigfloat with the same value as this object but with the exponent
     * changed.
     * @throws ArithmeticException An overflow error occurred, or the
     * result can&apos;t fit the given precision without rounding.
     * @throws java.lang.IllegalArgumentException The exponent is outside of the
     * valid range of the precision context, if it defines an exponent range.
     */
    public BigFloat Quantize(
      int desiredExponentSmall, PrecisionContext ctx) {
      return this.Quantize(new BigFloat(BigInteger.ONE, BigInteger.valueOf(desiredExponentSmall)), ctx);
    }

    /**
     * Returns a bigfloat with the same value as this object but with the same
     * exponent as another bigfloat.
     * @param otherValue A bigfloat containing the desired exponent of
     * the result. The mantissa is ignored.
     * @param ctx A precision context to control precision and rounding
     * of the result. If HasFlags of the context is true, will also store the
     * flags resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null, in which case the default rounding
     * mode is HalfEven.
     * @return A bigfloat with the same value as this object but with the exponent
     * changed.
     * @throws ArithmeticException An overflow error occurred, or the
     * result can&apos;t fit the given precision without rounding.
     * @throws java.lang.IllegalArgumentException The new exponent is outside of
     * the valid range of the precision context, if it defines an exponent
     * range.
     */
    public BigFloat Quantize(
      BigFloat otherValue, PrecisionContext ctx) {
      return math.Quantize(this, otherValue, ctx);
    }
    /**
     * Returns a bigfloat with the same value as this object but rounded to
     * an integer.
     * @param ctx A precision context to control precision and rounding
     * of the result. If HasFlags of the context is true, will also store the
     * flags resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null, in which case the default rounding
     * mode is HalfEven.
     * @return A bigfloat with the same value as this object but rounded to
     * an integer.
     * @throws ArithmeticException An overflow error occurred, or the
     * result can&apos;t fit the given precision without rounding.
     * @throws java.lang.IllegalArgumentException The new exponent must be changed
     * to 0 when rounding and 0 is outside of the valid range of the precision
     * context, if it defines an exponent range.
     */
    public BigFloat RoundToIntegralExact(
      PrecisionContext ctx) {
      return math.RoundToExponentExact(this, BigInteger.ZERO, ctx);
    }
    /**
     * Returns a bigfloat with the same value as this object but rounded to
     * an integer, without adding the FlagInexact or FlagRounded flags.
     * @param ctx A precision context to control precision and rounding
     * of the result. If HasFlags of the context is true, will also store the
     * flags resulting from the operation (the flags are in addition to the
     * pre-existing flags), except that this function will never add the
     * FlagRounded and FlagInexact flags (the only difference between
     * this and RoundToExponentExact). Can be null, in which case the default
     * rounding mode is HalfEven.
     * @return A bigfloat with the same value as this object but rounded to
     * an integer.
     * @throws ArithmeticException An overflow error occurred, or the
     * result can&apos;t fit the given precision without rounding.
     * @throws java.lang.IllegalArgumentException The new exponent must be changed
     * to 0 when rounding and 0 is outside of the valid range of the precision
     * context, if it defines an exponent range.
     */
    public BigFloat RoundToIntegralNoRoundedFlag(
      PrecisionContext ctx) {
      return math.RoundToExponentNoRoundedFlag(this, BigInteger.ZERO, ctx);
    }
    /**
     * Returns a bigfloat with the same value as this object but rounded to
     * a given exponent.
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
     * @return A bigfloat rounded to the given exponent.
     * @throws ArithmeticException An overflow error occurred, or the
     * result can&apos;t fit the given precision without rounding.
     * @throws java.lang.IllegalArgumentException The new exponent must be changed
     * when rounding and the new exponent is outside of the valid range of
     * the precision context, if it defines an exponent range.
     */
    public BigFloat RoundToExponentExact(
      BigInteger exponent, PrecisionContext ctx) {
      return math.RoundToExponentExact(this, exponent, ctx);
    }
    /**
     * Returns a bigfloat with the same value as this object but rounded to
     * a given exponent, without throwing an exception if the result overflows
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
     * @return A bigfloat rounded to the closest value representable in
     * the given precision, meaning if the result can&apos; t fit the precision,
     * additional digits are discarded to make it fit. If a precision context
     * is given, returns null if the result of the rounding overflowed the
     * exponent range.
     * @throws java.lang.IllegalArgumentException The new exponent must be changed
     * when rounding and the new exponent is outside of the valid range of
     * the precision context, if it defines an exponent range.
     */
    public BigFloat RoundToExponent(
      BigInteger exponent, PrecisionContext ctx) {
      return math.RoundToExponentSimple(this, exponent, ctx);
    }

    /**
     * Multiplies two bigfloats. The resulting scale will be the sum of the
     * scales of the two bigfloats.
     * @param op Another bigfloat.
     * @param ctx A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true, will
     * also store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The product of the two bigfloats. If a precision context is
     * given, returns null if the result of the rounding overflowed the exponent
     * range.
     */
    public BigFloat Multiply(
      BigFloat op, PrecisionContext ctx) {
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
    public BigFloat MultiplyAndAdd(
      BigFloat op, BigFloat augend, PrecisionContext ctx) {
      return math.MultiplyAndAdd(this, op, augend, ctx);
    }
    /**
     * Rounds this object's value to a given precision, using the given rounding
     * mode and range of exponent.
     * @param ctx A context for controlling the precision, rounding mode,
     * and exponent range. Can be null.
     * @return The closest value to this object&apos; s value, rounded to
     * the specified precision. Returns the same value as this object if
     * &quot; context&quot; is null or the precision and exponent range
     * are unlimited. If a precision context is given, returns null if the
     * result of the rounding overflowed the exponent range.
     */
    public BigFloat RoundToPrecision(
      PrecisionContext ctx) {
      return math.RoundToPrecision(this, ctx);
    }

    /**
     * Rounds this object's value to a given maximum bit length, using the
     * given rounding mode and range of exponent.
     * @param ctx A context for controlling the precision, rounding mode,
     * and exponent range. The precision is interpreted as the maximum bit
     * length of the mantissa. Can be null.
     * @return The closest value to this object&apos; s value, rounded to
     * the specified precision. Returns the same value as this object if
     * &quot; context&quot; is null or the precision and exponent range
     * are unlimited. Returns null if the result of the rounding overflowed
     * the exponent range.
     */
    public BigFloat RoundToBinaryPrecision(
      PrecisionContext ctx) {
      return math.RoundToBinaryPrecision(this, ctx);
    }

  }

