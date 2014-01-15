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
     * Contains parameters for controlling the precision, rounding, and
     * exponent range of arbitrary-precision numbers.
     */
  public class PrecisionContext {
    BigInteger exponentMax;
    /**
     * Gets the highest exponent possible when a converted number is expressed
     * in scientific notation with one digit before the decimal point. For
     * example, with a precision of 3 and an EMax of 100, the maximum value
     * possible is 9.99E + 100. (This is not the same as the highest possible
     * Exponent property.) If HasExponentRange is false, this value will
     * be 0.
     */
    public BigInteger getEMax() { return this.hasExponentRange ? this.exponentMax : BigInteger.ZERO; }

    BigInteger exponentMin;

    boolean hasExponentRange;
    /**
     * Gets whether this context defines a minimum and maximum exponent.
     * If false, converted exponents can have any exponent.
     */
    public boolean getHasExponentRange() { return this.hasExponentRange; }

    /**
     * Gets the lowest exponent possible when a converted number is expressed
     * in scientific notation with one digit before the decimal point. For
     * example, with a precision of 3 and an EMin of -100, the next value that
     * comes after 0 is 0.001E-100. (This is not the same as the lowest possible
     * Exponent property.) If HasExponentRange is false, this value will
     * be 0.
     */
    public BigInteger getEMin() { return this.hasExponentRange ? this.exponentMin : BigInteger.ZERO; }

    BigInteger bigintPrecision;

    /**
     * Gets the maximum length of a converted number in digits, ignoring
     * the decimal point and exponent. For example, if precision is 3, a converted
     * number's mantissa can range from 0 to 999 (up to three digits long).
     * If 0, converted numbers can have any precision.
     */
    public BigInteger getPrecision() { return this.bigintPrecision; }

    Rounding rounding;

    boolean clampNormalExponents;

    /**
     * If true, a converted number's Exponent property will not be higher
     * than EMax + 1 - Precision. If a number's exponent is higher than that
     * value, but not high enough to cause overflow, the exponent is clamped
     * to that value and enough zeros are added to the number's mantissa to
     * account for the adjustment. If HasExponentRange is false, this value
     * is always false.
     */
    public boolean getClampNormalExponents() { return this.hasExponentRange ? this.clampNormalExponents : false; }

    /**
     * Gets the desired rounding mode when converting numbers that can't
     * be represented in the given precision and exponent range.
     */
    public Rounding getRounding() { return this.rounding; }

    int flags;
    boolean hasFlags;

    /**
     * Returns whether this context has a mutable Flags field.
     */
    public boolean getHasFlags() { return this.hasFlags; }
    /**
     * Signals that the result was rounded to a different mathematical value,
     * but as close as possible to the original.
     */
    public static final int FlagInexact = 1;
    /**
     * Signals that the result was rounded to fit the precision; either the
     * value or the exponent may have changed from the original.
     */
    public static final int FlagRounded = 2;
    /**
     * Signals that the result's exponent, before rounding, is lower than
     * the lowest exponent allowed.
     */
    public static final int FlagSubnormal = 4;
    /**
     * Signals that the result's exponent, before rounding, is lower than
     * the lowest exponent allowed, and the result was rounded to a different
     * mathematical value, but as close as possible to the original.
     */
    public static final int FlagUnderflow = 8;
    /**
     * Signals that the result is non-zero and the exponent is higher than
     * the highest exponent allowed.
     */
    public static final int FlagOverflow = 16;
    /**
     * Signals that the exponent was adjusted to fit the exponent range.
     */
    public static final int FlagClamped = 32;
    /**
     * Signals an invalid operation.
     */
    public static final int FlagInvalid = 64;
    /**
     * Signals a division of a nonzero number by zero.
     */
    public static final int FlagDivideByZero = 128;
    /**
     * Gets the flags that are set from converting numbers according to this
     * precision context. If HasFlags is false, this value will be 0.
     */
    public int getFlags() { return this.flags; }
    /**
     * Sets the flags that occur from converting numbers according to this
     * precision context.
     * @throws IllegalStateException HasFlags is false.
     */
      public void setFlags(int value) {
        if (!this.getHasFlags()) {
 throw new IllegalStateException("Can't set flags");
}
        this.flags = value;
      }

    /**
     * Not documented yet.
     * @param exponent A BigInteger object.
     * @return A Boolean object.
     */
    public boolean ExponentWithinRange(BigInteger exponent) {
      if (exponent == null) { throw new NullPointerException("exponent"); }
      if (!this.getHasExponentRange())
        return true;
      if (this.bigintPrecision.signum()==0) {
        // Only check EMax, since with an unlimited
        // precision, any exponent less than EMin will exceed EMin if
        // the mantissa is the right size
        return exponent.compareTo(this.getEMax()) <= 0;
      } else {
        BigInteger bigint = exponent;
        bigint=bigint.add(this.bigintPrecision);
        bigint=bigint.subtract(BigInteger.ONE);
        if (bigint.compareTo(this.getEMin()) < 0)
          return false;
        if (exponent.compareTo(this.getEMax()) > 0)
          return false;
        return true;
      }
    }

    /**
     * Copies this PrecisionContext with the specified rounding mode.
     * @param rounding A Rounding object.
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithRounding(Rounding rounding) {
      PrecisionContext pc = this.Copy();
      pc.rounding = rounding;
      return pc;
    }

    /**
     * Copies this PrecisionContext with HasFlags set to true and a Flags
     * value of 0.
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithBlankFlags() {
      PrecisionContext pc = this.Copy();
      pc.hasFlags = true;
      pc.flags = 0;
      return pc;
    }

    /**
     * Copies this precision context and sets the copy's "ClampNormalExponents"
     * flag to the given value.
     * @param clamp A Boolean object.
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithExponentClamp(boolean clamp) {
      PrecisionContext pc = this.Copy();
      pc.clampNormalExponents = clamp;
      return pc;
    }

    /**
     * Not documented yet.
     * @param exponentMin A BigInteger object.
     * @param exponentMax A BigInteger object. (2)
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithExponentRange(BigInteger exponentMin, BigInteger exponentMax) {
      if (exponentMin == null) { throw new NullPointerException("exponentMin"); }
      if (exponentMin.compareTo(exponentMax) > 0) {
 throw new IllegalArgumentException("exponentMin greater than exponentMax");
}
      PrecisionContext pc = this.Copy();
      pc.hasExponentRange = true;
      pc.exponentMin = exponentMin;
      pc.exponentMax = exponentMax;
      return pc;
    }

    /**
     * Copies this PrecisionContext with HasFlags set to false and a Flags
     * value of 0.
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithNoFlags() {
      PrecisionContext pc = this.Copy();
      pc.hasFlags = false;
      pc.flags = 0;
      return pc;
    }
    /**
     * Copies this PrecisionContext with an unlimited exponent range.
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithUnlimitedExponents() {
      PrecisionContext pc = this.Copy();
      pc.hasExponentRange = false;
      return pc;
    }
    /**
     * Copies this PrecisionContext and gives it a particular precision
     * value.
     * @param precision Desired precision. 0 means unlimited precision.
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithPrecision(int precision) {
      if (precision < 0) { throw new IllegalArgumentException("precision" + " not greater or equal to " + "0" + " ("+precision+")"); }
      PrecisionContext pc = this.Copy();
      pc.bigintPrecision = BigInteger.valueOf(precision);
      return pc;
    }

    /**
     * Not documented yet.
     * @param bigintPrecision A BigInteger object.
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithBigPrecision(BigInteger bigintPrecision) {
      if (bigintPrecision == null) { throw new NullPointerException("bigintPrecision"); }
      if (bigintPrecision.signum() < 0) throw new IllegalArgumentException(
        "precision" + " not greater or equal to " + "0" + " (" +
        bigintPrecision + ")");
      PrecisionContext pc = this.Copy();
      pc.bigintPrecision = bigintPrecision;
      return pc;
    }

    /**
     * Initializes a new PrecisionContext that is a copy of another PrecisionContext.
     * @return A PrecisionContext object.
     */
    public PrecisionContext Copy() {
      PrecisionContext pcnew = new PrecisionContext(
0, this.rounding,
                                                  0, 0, this.clampNormalExponents);
      pcnew.hasFlags = this.hasFlags;
      pcnew.flags = this.flags;
      pcnew.exponentMax = this.exponentMax;
      pcnew.exponentMin = this.exponentMin;
      pcnew.hasExponentRange = this.hasExponentRange;
      pcnew.bigintPrecision = this.bigintPrecision;
      pcnew.rounding = this.rounding;
      pcnew.clampNormalExponents = this.clampNormalExponents;
      return pcnew;
    }

    public static PrecisionContext ForPrecision(int precision) {
      return new PrecisionContext(precision, Rounding.HalfUp, 0, 0, false).WithUnlimitedExponents();
    }

    public static PrecisionContext ForRounding(Rounding rounding) {
      return new PrecisionContext(0, rounding, 0, 0, false).WithUnlimitedExponents();
    }

    public static PrecisionContext ForPrecisionAndRounding(int precision, Rounding rounding) {
      return new PrecisionContext(precision, rounding, 0, 0, false).WithUnlimitedExponents();
    }
    /**
     * Initializes a new PrecisionContext. HasFlags will be set to false.
     */
    public PrecisionContext (
int precision, Rounding rounding, int exponentMinSmall, int exponentMaxSmall,
                            boolean clampNormalExponents) {
      if (precision < 0) { throw new IllegalArgumentException("precision" + " not greater or equal to " + "0" + " ("+precision+")"); }
      if (exponentMinSmall > exponentMaxSmall) { throw new IllegalArgumentException("exponentMinSmall" + " not less or equal to "+exponentMaxSmall+" ("+exponentMinSmall+")"); }
      this.bigintPrecision = precision == 0 ? BigInteger.ZERO : BigInteger.valueOf(precision);
      this.rounding = rounding;
      this.clampNormalExponents = clampNormalExponents;
      this.hasExponentRange = true;
      this.exponentMax = exponentMaxSmall == 0 ? BigInteger.ZERO : BigInteger.valueOf(exponentMaxSmall);
      this.exponentMin = exponentMinSmall == 0 ? BigInteger.ZERO : BigInteger.valueOf(exponentMinSmall);
    }
    /**
     * No specific limit on precision. Rounding mode HalfUp.
     */

    public static final PrecisionContext Unlimited = PrecisionContext.ForPrecision(0);
    /**
     * Precision context for the IEEE-754-2008 decimal32 format.
     */

    public static final PrecisionContext Decimal32 =
      new PrecisionContext(7, Rounding.HalfEven, -95, 96, true);
    /**
     * Precision context for the IEEE-754-2008 decimal64 format.
     */

    public static final PrecisionContext Decimal64 =
      new PrecisionContext(16, Rounding.HalfEven, -383, 384, true);
    /**
     * Precision context for the IEEE-754-2008 decimal128 format.
     */

    public static final PrecisionContext Decimal128 =
      new PrecisionContext(34, Rounding.HalfEven, -6143, 6144, true);
    /**
     * Precision context for the Common Language Infrastructure (.NET
     * Framework) decimal format, 96 bits precision. Use RoundToBinaryPrecision
     * to round a decimal fraction to this format.
     */

    public static final PrecisionContext CliDecimal =
      new PrecisionContext(96, Rounding.HalfEven, 0, 28, true);

  }

