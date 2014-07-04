package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Contains parameters for controlling the precision, rounding, and exponent
     * range of arbitrary-precision numbers.
     */
  public class PrecisionContext {
    private BigInteger exponentMax;

    /**
     * Gets the highest exponent possible when a converted number is expressed in
     * scientific notation with one digit before the decimal point. For
     * example, with a precision of 3 and an EMax of 100, the maximum value
     * possible is 9.99E + 100. (This is not the same as the highest
     * possible Exponent property.) If HasExponentRange is false, this value
     * will be 0.
     * @return The highest exponent possible when a converted number is expressed
     * in scientific notation with one digit before the decimal point. For
     * example, with a precision of 3 and an EMax of 100, the maximum value
     * possible is 9.99E + 100. (This is not the same as the highest
     * possible Exponent property.) If HasExponentRange is false, this value
     * will be 0.
     */
    public final BigInteger getEMax() {
        return this.hasExponentRange ? this.exponentMax : BigInteger.ZERO;
      }

    private int traps;

    /**
     * Gets the traps that are set for each flag in the context. Whenever a flag is
     * signaled, even if HasFlags is false, and the flag's trap is enabled,
     * the operation will throw a TrapException. <p>For example, if Traps
     * equals FlagInexact and FlagSubnormal, a TrapException will be thrown
     * if an operation's return value is not the same as the exact result
     * (FlagInexact) or if the return value's exponent is lower than the
     * lowest allowed (FlagSubnormal).</p>
     * @return The traps that are set for each flag in the context.
     */
    public final int getTraps() {
        return this.traps;
      }

    private BigInteger exponentMin;

    private boolean hasExponentRange;

    /**
     * Gets a value indicating whether this context defines a minimum and maximum
     * exponent. If false, converted exponents can have any exponent and
     * operations can't cause overflow or underflow.
     * @return True if this context defines a minimum and maximum exponent;
     * otherwise, false.
     */
    public final boolean getHasExponentRange() {
        return this.hasExponentRange;
      }

    /**
     * Gets a value indicating whether this context defines a maximum precision.
     * @return True if this context defines a maximum precision; otherwise, false.
     */
    public final boolean getHasMaxPrecision() {
        return this.bigintPrecision.signum() != 0;
      }

    /**
     * Gets the lowest exponent possible when a converted number is expressed in
     * scientific notation with one digit before the decimal point. For
     * example, with a precision of 3 and an EMin of -100, the next value
     * that comes after 0 is 0.001E-100. (If AdjustExponent is false, this
     * property specifies the lowest possible Exponent property instead.) If
     * HasExponentRange is false, this value will be 0.
     * @return The lowest exponent possible when a converted number is expressed in
     * scientific notation with one digit before the decimal point.
     */
    public final BigInteger getEMin() {
        return this.hasExponentRange ? this.exponentMin : BigInteger.ZERO;
      }

    private BigInteger bigintPrecision;

    /**
     * Gets the maximum length of a converted number in digits, ignoring the
     * decimal point and exponent. For example, if precision is 3, a
     * converted number&apos;s mantissa can range from 0 to 999 (up to three
     * digits long). If 0, converted numbers can have any precision.
     * @return The maximum length of a converted number in digits, ignoring the
     * decimal point and exponent.
     */
    public final BigInteger getPrecision() {
        return this.bigintPrecision;
      }

    private Rounding rounding;

    private boolean clampNormalExponents;

    /**
     * Gets a value indicating whether a converted number&apos;s Exponent property
     * will not be higher than EMax + 1 - Precision. If a number&apos;s
     * exponent is higher than that value, but not high enough to cause
     * overflow, the exponent is clamped to that value and enough zeros are
     * added to the number&apos;s mantissa to account for the adjustment. If
     * HasExponentRange is false, this value is always false.
     * @return If true, a converted number's Exponent property will not be higher
     * than EMax + 1 - Precision.
     */
    public final boolean getClampNormalExponents() {
        return this.hasExponentRange && this.clampNormalExponents;
      }

    /**
     * Gets the desired rounding mode when converting numbers that can&apos;t be
     * represented in the given precision and exponent range.
     * @return The desired rounding mode when converting numbers that can't be
     * represented in the given precision and exponent range.
     */
    public final Rounding getRounding() {
        return this.rounding;
      }

    private int flags;
    private boolean hasFlags;

    /**
     * Gets a value indicating whether this context has a mutable Flags field.
     * @return True if this context has a mutable Flags field; otherwise, false.
     */
    public final boolean getHasFlags() {
        return this.hasFlags;
      }

    /**
     * Signals that the result was rounded to a different mathematical value, but
     * as close as possible to the original.
     */
    public static final int FlagInexact = 1;

    /**
     * Signals that the result was rounded to fit the precision; either the value
     * or the exponent may have changed from the original.
     */
    public static final int FlagRounded = 2;

    /**
     * Signals that the result&apos;s exponent, before rounding, is lower than the
     * lowest exponent allowed.
     */
    public static final int FlagSubnormal = 4;

    /**
     * Signals that the result&apos;s exponent, before rounding, is lower than the
     * lowest exponent allowed, and the result was rounded to a different
     * mathematical value, but as close as possible to the original.
     */
    public static final int FlagUnderflow = 8;

    /**
     * Signals that the result is non-zero and the exponent is higher than the
     * highest exponent allowed.
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
     * Signals that an operand was rounded to a different mathematical value before
     * an operation.
     */
    public static final int FlagLostDigits = 256;

    /**
     * Gets the flags that are set from converting numbers according to this
     * precision context. If HasFlags is false, this value will be 0. This
     * value is a combination of bit fields. To retrieve a particular flag,
     * use the AND operation on the return value of this method. For
     * example: <code>(this.getFlags() &amp; PrecisionContext.FlagInexact) !=
     * 0</code> returns TRUE if the Inexact flag is set.
     * @return The flags that are set from converting numbers according to this
     * precision context. If HasFlags is false, this value will be 0.
     */
    public final int getFlags() {
        return this.flags;
      }
public final void setFlags(int value) {
        if (!this.getHasFlags()) {
          throw new IllegalStateException("Can't set flags");
        }
        this.flags = value;
      }

    /**
     * Determines whether a number can have the given Exponent property under this
     * precision context.
     * @param exponent A BigInteger object indicating the desired exponent.
     * @return True if a number can have the given Exponent property under this
     * precision context; otherwise, false. If this context allows unlimited
     * precision, returns true for the exponent EMax and any exponent less
     * than EMax.
     * @throws NullPointerException The parameter {@code exponent} is null.
     */
    public boolean ExponentWithinRange(BigInteger exponent) {
      if (exponent == null) {
        throw new NullPointerException("exponent");
      }
      if (!this.getHasExponentRange()) {
        return true;
      }
      if (this.bigintPrecision.signum() == 0) {
        // Only check EMax, since with an unlimited
        // precision, any exponent less than EMin will exceed EMin if
        // the mantissa is the right size
        return exponent.compareTo(this.getEMax()) <= 0;
      } else {
        BigInteger bigint = exponent;
        if (this.adjustExponent) {
          bigint = bigint.add(this.bigintPrecision);
          bigint = bigint.subtract(BigInteger.ONE);
        }
        return (bigint.compareTo(this.getEMin()) >= 0) &&
          (exponent.compareTo(this.getEMax()) <= 0);
      }
    }

    /**
     * Gets a string representation of this object. Note that the format is not
     * intended to be parsed and may change at any time.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return "[PrecisionContext ExponentMax=" + this.exponentMax +
        ", Traps=" + this.traps + ", ExponentMin=" + this.exponentMin +
        ", HasExponentRange=" + this.hasExponentRange + ", BigintPrecision=" +
        this.bigintPrecision + ", Rounding=" + this.rounding +
        ", ClampNormalExponents=" + this.clampNormalExponents + ", Flags=" +
        this.flags + ", HasFlags=" + this.hasFlags + "]";
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
     * Copies this PrecisionContext with HasFlags set to true and a Flags value of
     * 0.
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithBlankFlags() {
      PrecisionContext pc = this.Copy();
      pc.hasFlags = true;
      pc.flags = 0;
      return pc;
    }

    /**
     * Copies this PrecisionContext with Traps set to the given value.
     * @param traps Flags representing the traps to enable. See the property
     * &quot;Traps&quot;.
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithTraps(int traps) {
      PrecisionContext pc = this.Copy();
      pc.hasFlags = true;
      pc.traps = traps;
      return pc;
    }

    /**
     * Copies this precision context and sets the copy&apos;s
     * &quot;ClampNormalExponents&quot; flag to the given value.
     * @param clamp A Boolean object.
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithExponentClamp(boolean clamp) {
      PrecisionContext pc = this.Copy();
      pc.clampNormalExponents = clamp;
      return pc;
    }

    /**
     * Copies this precision context and sets the copy&apos;s exponent range.
     * @param exponentMinSmall Desired minimum exponent (EMin).
     * @param exponentMaxSmall Desired maximum exponent (EMax).
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithExponentRange(
      int exponentMinSmall,
      int exponentMaxSmall) {
      if (exponentMinSmall > exponentMaxSmall) {
        throw new IllegalArgumentException("exponentMinSmall (" + exponentMinSmall +
          ") is more than " + exponentMaxSmall);
      }
      PrecisionContext pc = this.Copy();
      pc.hasExponentRange = true;
      pc.exponentMin = BigInteger.valueOf(exponentMinSmall);
      pc.exponentMax = BigInteger.valueOf(exponentMaxSmall);
      return pc;
    }

    /**
     * Copies this precision context and sets the copy&apos;s exponent range.
     * @param exponentMin Desired minimum exponent (EMin).
     * @param exponentMax Desired maximum exponent (EMax).
     * @return A PrecisionContext object.
     * @throws NullPointerException The parameter {@code exponentMin} is null.
     * @throws NullPointerException The parameter {@code exponentMax} is null.
     */
    public PrecisionContext WithBigExponentRange(
      BigInteger exponentMin,
      BigInteger exponentMax) {
      if (exponentMin == null) {
        throw new NullPointerException("exponentMin");
      }
      if (exponentMax == null) {
        throw new NullPointerException("exponentMax");
      }
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
     * Copies this PrecisionContext with HasFlags set to false and a Flags value of
     * 0.
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithNoFlags() {
      PrecisionContext pc = this.Copy();
      pc.hasFlags = false;
      pc.flags = 0;
      return pc;
    }

    private boolean precisionInBits;

    /**
     * Gets a value indicating whether this context's Precision property is in
     * bits, rather than digits. The default is false.
     * @return True if this context's Precision property is in bits, rather than
     * digits; otherwise, false.. The default is false.
     */
    public final boolean isPrecisionInBits() {
        return this.precisionInBits;
      }

    /**
     * Copies this PrecisionContext and sets the copy's "IsPrecisionInBits"
     * property to the given value.
     * @param isPrecisionBits A Boolean object.
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithPrecisionInBits(boolean isPrecisionBits) {
      PrecisionContext pc = this.Copy();
      pc.precisionInBits = isPrecisionBits;
      return pc;
    }

    private boolean simplified;

    /**
     * Gets a value indicating whether to use a "simplified" arithmetic. In the
     * simplified arithmetic, infinity, not-a-number, and subnormal numbers
     * are not allowed, and negative zero is treated the same as positive
     * zero. For further details, see
     * <code>http://speleotrove.com/decimal/dax3274.html</code>
     * @return True if a "simplified" arithmetic will be used; otherwise, false.
     */
    public final boolean isSimplified() {
        return this.simplified;
      }

    /**
     * Copies this PrecisionContext and sets the copy's "IsSimplified" property to
     * the given value.
     * @param simplified A Boolean object.
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithSimplified(boolean simplified) {
      PrecisionContext pc = this.Copy();
      pc.simplified = simplified;
      return pc;
    }

    private boolean adjustExponent;

    /**
     * Gets a value indicating whether the EMax and EMin properties refer to the
     * number's Exponent property adjusted to the number's precision, or
     * just the number's Exponent property. The default value is true,
     * meaning that EMax and EMin refer to the adjusted exponent. Setting
     * this value to false (using WithAdjustExponent) is useful for modeling
     * floating point representations with an integer mantissa and an
     * integer exponent, such as Java's BigDecimal.
     * @return True if the EMax and EMin properties refer to the number's Exponent
     * property adjusted to the number's precision, or false if they refer
     * to just the number's Exponent property.
     */
    public final boolean getAdjustExponent() {
        return this.adjustExponent;
      }

    /**
     * Copies this PrecisionContext and sets the copy's "AdjustExponent" property
     * to the given value.
     * @param adjustExponent A Boolean object.
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithAdjustExponent(boolean adjustExponent) {
      PrecisionContext pc = this.Copy();
      pc.adjustExponent = adjustExponent;
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
     * Copies this PrecisionContext and gives it a particular precision value.
     * @param precision Desired precision. 0 means unlimited precision.
     * @return A PrecisionContext object.
     */
    public PrecisionContext WithPrecision(int precision) {
      if (precision < 0) {
        throw new IllegalArgumentException("precision (" + precision +
          ") is less than " + "0");
      }
      PrecisionContext pc = this.Copy();
      pc.bigintPrecision = BigInteger.valueOf(precision);
      return pc;
    }

    /**
     * Copies this PrecisionContext and gives it a particular precision value.
     * @param bigintPrecision A BigInteger object.
     * @return A PrecisionContext object.
     * @throws NullPointerException The parameter {@code bigintPrecision} is null.
     */
    public PrecisionContext WithBigPrecision(BigInteger bigintPrecision) {
      if (bigintPrecision == null) {
        throw new NullPointerException("bigintPrecision");
      }
      if (bigintPrecision.signum() < 0) {
        throw new IllegalArgumentException("bigintPrecision's sign (" +
          bigintPrecision.signum() + ") is less than " + "0");
      }
      PrecisionContext pc = this.Copy();
      pc.bigintPrecision = bigintPrecision;
      return pc;
    }

    /**
     * Initializes a new PrecisionContext that is a copy of another
     * PrecisionContext.
     * @return A PrecisionContext object.
     */
    public PrecisionContext Copy() {
      PrecisionContext pcnew = new PrecisionContext(
        0,
        this.rounding,
        0,
        0,
        this.clampNormalExponents);
      pcnew.hasFlags = this.hasFlags;
      pcnew.precisionInBits = this.precisionInBits;
      pcnew.adjustExponent = this.adjustExponent;
      pcnew.simplified = this.simplified;
      pcnew.flags = this.flags;
      pcnew.exponentMax = this.exponentMax;
      pcnew.exponentMin = this.exponentMin;
      pcnew.hasExponentRange = this.hasExponentRange;
      pcnew.bigintPrecision = this.bigintPrecision;
      pcnew.rounding = this.rounding;
      pcnew.clampNormalExponents = this.clampNormalExponents;
      return pcnew;
    }

    /**
     * Creates a new precision context using the given maximum number of digits and
     * an unlimited exponent range.
     * @param precision Maximum number of digits (precision).
     * @return A PrecisionContext object.
     */
    public static PrecisionContext ForPrecision(int precision) {
      return new PrecisionContext(
        precision,
        Rounding.HalfUp,
        0,
        0,
        false).WithUnlimitedExponents();
    }

    /**
     * Creates a new PrecisionContext object initialized with unlimited precision
     * and exponent range, and the given rounding mode.
     * @param rounding The rounding mode for the new precision context.
     * @return A PrecisionContext object.
     */
    public static PrecisionContext ForRounding(Rounding rounding) {
      return new PrecisionContext(
        0,
        rounding,
        0,
        0,
        false).WithUnlimitedExponents();
    }

    /**
     * Creates a new PrecisionContext object initialized with unlimited and
     * exponent range, and the given rounding mode and maximum precision.
     * @param precision Maximum number of digits (precision).
     * @param rounding A Rounding object.
     * @return A PrecisionContext object.
     */
    public static PrecisionContext ForPrecisionAndRounding(
      int precision,
      Rounding rounding) {
      return new PrecisionContext(
        precision,
        rounding,
        0,
        0,
        false).WithUnlimitedExponents();
    }

    /**
     * Initializes a new instance of the PrecisionContext class. HasFlags will be
     * set to false.
     * @param precision A 32-bit signed integer.
     * @param rounding A Rounding object.
     * @param exponentMinSmall A 32-bit signed integer. (2).
     * @param exponentMaxSmall A 32-bit signed integer. (3).
     * @param clampNormalExponents A Boolean object.
     */
    public PrecisionContext (
      int precision,
      Rounding rounding,
      int exponentMinSmall,
      int exponentMaxSmall,
      boolean clampNormalExponents) {
      if (precision < 0) {
        throw new IllegalArgumentException("precision (" + precision +
          ") is less than " + "0");
      }
      if (exponentMinSmall > exponentMaxSmall) {
        throw new IllegalArgumentException("exponentMinSmall (" + exponentMinSmall +
          ") is more than " + exponentMaxSmall);
      }
      this.bigintPrecision = precision == 0 ? BigInteger.ZERO :
        BigInteger.valueOf(precision);
      this.rounding = rounding;
      this.clampNormalExponents = clampNormalExponents;
      this.hasExponentRange = true;
      this.adjustExponent = true;
      this.exponentMax = exponentMaxSmall == 0 ? BigInteger.ZERO :
        BigInteger.valueOf(exponentMaxSmall);
      this.exponentMin = exponentMinSmall == 0 ? BigInteger.ZERO :
        BigInteger.valueOf(exponentMinSmall);
    }

    /**
     * No specific limit on precision. Rounding mode HalfUp.
     */

    public static final PrecisionContext Unlimited =
      PrecisionContext.ForPrecision(0);

    /**
     * Precision context for the IEEE-754-2008 binary16 format, 11 bits precision.
     * The default rounding mode is HalfEven.
     */

    public static final PrecisionContext Binary16 =
      PrecisionContext.ForPrecisionAndRounding(11, Rounding.HalfEven)
      .WithExponentClamp(true)
      .WithExponentRange(-14, 15);

    /**
     * Precision context for the IEEE-754-2008 binary32 format, 24 bits precision.
     * The default rounding mode is HalfEven.
     */

    public static final PrecisionContext Binary32 =
      PrecisionContext.ForPrecisionAndRounding(24, Rounding.HalfEven)
      .WithExponentClamp(true)
      .WithExponentRange(-126, 127);

    /**
     * Precision context for the IEEE-754-2008 binary64 format, 53 bits precision.
     * The default rounding mode is HalfEven.
     */

    public static final PrecisionContext Binary64 =
      PrecisionContext.ForPrecisionAndRounding(53, Rounding.HalfEven)
      .WithExponentClamp(true)
      .WithExponentRange(-1022, 1023);

    /**
     * Precision context for the IEEE-754-2008 binary128 format, 113 bits
     * precision. The default rounding mode is HalfEven.
     */

    public static final PrecisionContext Binary128 =
      PrecisionContext.ForPrecisionAndRounding(113, Rounding.HalfEven)
      .WithExponentClamp(true)
      .WithExponentRange(-16382, 16383);

    /**
     * Precision context for the IEEE-754-2008 decimal32 format. The default
     * rounding mode is HalfEven.
     */

    public static final PrecisionContext Decimal32 =
      new PrecisionContext(7, Rounding.HalfEven, -95, 96, true);

    /**
     * Precision context for the IEEE-754-2008 decimal64 format. The default
     * rounding mode is HalfEven.
     */

    public static final PrecisionContext Decimal64 =
      new PrecisionContext(16, Rounding.HalfEven, -383, 384, true);

    /**
     * Precision context for the IEEE-754-2008 decimal128 format. The default
     * rounding mode is HalfEven.
     */

    public static final PrecisionContext Decimal128 =
      new PrecisionContext(34, Rounding.HalfEven, -6143, 6144, true);

    /**
     * Basic precision context, 9 digits precision, rounding mode half-up,
     * unlimited exponent range. The default rounding mode is HalfUp.
     */

    public static final PrecisionContext Basic =
      PrecisionContext.ForPrecisionAndRounding(9, Rounding.HalfUp);

    /**
     * Precision context for the Common Language Infrastructure (.NET Framework)
     * decimal format, 96 bits precision, and a valid exponent range of -28
     * to 0. The default rounding mode is HalfEven.
     */

    public static final PrecisionContext CliDecimal =
      new PrecisionContext(96, Rounding.HalfEven, 0, 28, true)
      .WithPrecisionInBits(true);

    /**
     * Precision context for Java's BigDecimal format. The default rounding mode is
     * HalfUp.
     */

    public static final PrecisionContext BigDecimalJava =
      new PrecisionContext(0, Rounding.HalfUp, 0, 0, true)
      .WithExponentClamp(true)
      .WithAdjustExponent(false)
      .WithBigExponentRange(
        BigInteger.ZERO.subtract(BigInteger.valueOf(Integer.MAX_VALUE)),
        BigInteger.ONE.add(BigInteger.valueOf(Integer.MAX_VALUE)));

    /**
     * Precision context for Java's BigDecimal format. The default rounding mode is
     * HalfUp.
     * @deprecated This context had the wrong settings in previous versions. Use BigDecimalJava
* instead.
 */
@Deprecated

    public static final PrecisionContext JavaBigDecimal = BigDecimalJava;
  }
