package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

//import java.math.*;

    /**
     * Contains parameters for controlling the precision, rounding, and
     * exponent range of arbitrary-precision numbers.
     */
  public class PrecisionContext {
    BigInteger eMax;
    /**
     * Gets the highest exponent possible when a converted number is expressed
     * in scientific notation with one digit before the decimal point. For
     * example, with a precision of 3 and an EMax of 100, the maximum value
     * possible is 9.99E+100. (This is not the same as the highest possible
     * Exponent property.) If HasExponentRange is false, this value will
     * be 0.
     */
    public BigInteger getEMax() { return hasExponentRange ? eMax : BigInteger.ZERO; }
    BigInteger eMin;

    boolean hasExponentRange;
    /**
     * Gets whether this context defines a minimum and maximum exponent.
     * If false, converted exponents can have any exponent.
     */
    public boolean getHasExponentRange() { return hasExponentRange; }

    /**
     * Gets the lowest exponent possible when a converted number is expressed
     * in scientific notation with one digit before the decimal point. For
     * example, with a precision of 3 and an EMin of -100, the next value that
     * comes after 0 is 0.001E-100. (This is not the same as the lowest possible
     * Exponent property.) If HasExponentRange is false, this value will
     * be 0.
     */
    public BigInteger getEMin() { return hasExponentRange ? eMin : BigInteger.ZERO; }
    long precision;

    /**
     * Gets the maximum length of a converted number in digits, ignoring
     * the decimal point and exponent. For example, if precision is 3, a converted
     * number's mantissa can range from 0 to 999 (up to three digits long).
     * If 0, converted numbers can have any precision.
     */
    public long getPrecision() { return precision; }
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
    public boolean getClampNormalExponents() { return hasExponentRange ? clampNormalExponents : false; }

    /**
     * Gets the desired rounding mode when converting numbers that can't
     * be represented in the given precision and exponent range.
     */
    public Rounding getRounding() { return rounding; }

    int flags;
    boolean hasFlags;

    /**
     * Returns whether this context has a mutable Flags field.
     */
    public boolean getHasFlags() { return hasFlags; }

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
     * Signals that the result is non-zero and the exponent is lower than
     * the lowest exponent allowed.
     */
    public static final int FlagSubnormal = 4;
    /**
     * Signals that the result is non-zero, the exponent is lower than the
     * lowest exponent allowed, and the result was rounded to a different
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
     * Gets the flags that are set from converting numbers according to this
     * precision context. If HasFlags is false, this value will be 0.
     */
    public int getFlags() { return flags; }
    /**
     * Sets the flags that occur from converting numbers according to this
     * precision context.
     * @throws IllegalStateException HasFlags is false.
     */
      public void setFlags(int value) {
        if (!this.getHasFlags())
          throw new IllegalStateException("Can't set flags");
        flags = value;
      }
    /**
     * Copies this PrecisionContext with HasFlags set to true and a Flags
     * value of 0.
     * @param rounding A Rounding object.
     */
    public PrecisionContext WithRounding(Rounding rounding) {
      PrecisionContext pc = new PrecisionContext(this);
      pc.rounding = rounding;
      return pc;
    }

    /**
     * Copies this PrecisionContext with HasFlags set to true and a Flags
     * value of 0.
     */
    public PrecisionContext WithBlankFlags() {
      PrecisionContext pc = new PrecisionContext(this);
      pc.hasFlags = true;
      pc.flags = 0;
      return pc;
    }
    /**
     * Copies this PrecisionContext with HasFlags set to false and a Flags
     * value of 0.
     */
    public PrecisionContext WithNoFlags() {
      PrecisionContext pc = new PrecisionContext(this);
      pc.hasFlags = false;
      pc.flags = 0;
      return pc;
    }
    /**
     * Copies this PrecisionContext with an unlimited exponent range.
     */
    public PrecisionContext WithUnlimitedExponents() {
      PrecisionContext pc = new PrecisionContext(this);
      pc.hasExponentRange = false;
      return pc;
    }
    /**
     * Copies this PrecisionContext with a particular precision.
     * @param precision Desired precision. 0 means unlimited precision.
     */
    public PrecisionContext WithPrecision(long precision) {
      if ((precision) < 0) throw new IllegalArgumentException("precision" + " not greater or equal to " + "0" + " (" + Long.toString((long)(long)(precision)) + ")");
      PrecisionContext pc = new PrecisionContext(this);
      pc.precision = precision;
      return pc;
    }
    /**
     * Initializes a new PrecisionContext that is a copy of another PrecisionContext.
     */
    public PrecisionContext(PrecisionContext pc) {
      if ((pc) == null) throw new NullPointerException("pc");
      this.hasFlags = pc.hasFlags;
      this.flags = pc.flags;
      this.eMax = pc.eMax;
      this.eMin = pc.eMin;
      this.hasExponentRange = pc.hasExponentRange;
      this.precision = pc.precision;
      this.rounding = pc.rounding;
      this.clampNormalExponents = pc.clampNormalExponents;
    }

    /**
     * Initializes a new PrecisionContext from a desired maximum precision.
     * @param precision A 64-bit signed integer.
     */
    public PrecisionContext(long precision){
 this(precision, Rounding.HalfEven);
    }

    /**
     * Initializes a new PrecisionContext from a desired rounding mode.
     * @param rounding A Rounding object.
     */
    public PrecisionContext(Rounding rounding){
 this(0,rounding);
    }

    /**
     * Initializes a new PrecisionContext. HasFlags will be set to false.
     */
    public PrecisionContext(long precision, Rounding rounding){
      this.precision = precision;
      this.rounding = rounding;
      eMax = BigInteger.ZERO;
      eMin = BigInteger.ZERO;
    }
    /**
     * Initializes a new PrecisionContext. HasFlags will be set to false.
     */
    public PrecisionContext(long precision, Rounding rounding, long eMinSmall, long eMaxSmall){
 this(precision,rounding,eMinSmall,eMaxSmall,false);
    }
    /**
     * Initializes a new PrecisionContext. HasFlags will be set to false.
     */
    public PrecisionContext(long precision, Rounding rounding, long eMinSmall, long eMaxSmall,
                            boolean clampNormalExponents) {
      if ((precision) < 0) throw new IllegalArgumentException("precision" + " not greater or equal to " + "0" + " (" + Long.toString((long)(long)(precision)) + ")");
      if ((eMinSmall) > eMaxSmall) throw new IllegalArgumentException("eMinSmall" + " not less or equal to " + Long.toString((long)(long)(eMaxSmall)) + " (" + Long.toString((long)(long)(eMinSmall)) + ")");
      this.precision = precision;
      this.rounding = rounding;
      this.clampNormalExponents = clampNormalExponents;
      this.hasExponentRange=true;
      eMax = BigInteger.valueOf(eMaxSmall);
      eMin = BigInteger.valueOf(eMinSmall);
    }
    /**
     * Initializes a new PrecisionContext. HasFlags will be set to false.
     */
    public PrecisionContext(long precision, Rounding rounding, BigInteger eMin, BigInteger eMax){
 this(precision,rounding,eMin,eMax,false);
    }
    /**
     * Initializes a new PrecisionContext. HasFlags will be set to false.
     */
    public PrecisionContext(long precision, Rounding rounding, BigInteger eMin, BigInteger eMax,
                            boolean clampNormalExponents) {
      if((eMin)==null)throw new NullPointerException("eMin");
      if ((precision) < 0) throw new IllegalArgumentException("precision" + " not greater or equal to " + "0" + " (" + Long.toString((long)(long)(precision)) + ")");
      if (eMin.compareTo(eMax) > 0) throw new IllegalArgumentException("eMin" + " not less or equal to " + eMax + " (" + eMin + ")");
      this.precision = precision;
      this.rounding = rounding;
      this.hasExponentRange=true;
      this.clampNormalExponents = clampNormalExponents;
      this.eMax = eMax;
      this.eMin = eMin;
    }

    /**
     * Unlimited precision context. Rounding mode HalfUp.
     */
    public static final PrecisionContext Unlimited =
      new PrecisionContext(0,Rounding.HalfUp);
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
      new PrecisionContext(96,Rounding.HalfEven,0,28,true);
    
  }
