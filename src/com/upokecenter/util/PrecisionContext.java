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
     * Exponent property.)
     */
    public BigInteger getEMax() { return eMax; }
    BigInteger eMin;

    /**
     * Gets the lowest exponent possible when a converted number is expressed
     * in scientific notation with one digit before the decimal point. For
     * example, with a precision of 3 and an EMin of -100, the next value that
     * comes after 0 is 0.001E-100. (This is not the same as the lowest possible
     * Exponent property.)
     */
    public BigInteger getEMin() { return eMin; }
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
     * account for the adjustment.
     */
    public boolean getClampNormalExponents() { return clampNormalExponents; }

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
    public static final int SignalInexact = 1;
    /**
     * Signals that the result was rounded to fit the precision; either the
     * value or the exponent may have changed from the original.
     */
    public static final int SignalRounded = 2;
    /**
     * Signals that the result is non-zero and the exponent is lower than
     * the lowest exponent allowed.
     */
    public static final int SignalSubnormal = 4;
    /**
     * Signals that the result is non-zero, the exponent is lower than the
     * lowest exponent allowed, and the result was rounded to a different
     * mathematical value, but as close as possible to the original.
     */
    public static final int SignalUnderflow = 8;
    /**
     * Signals that the result is non-zero and the exponent is higher than
     * the highest exponent allowed.
     */
    public static final int SignalOverflow = 16;
    /**
     * Signals that the exponent was adjusted to fit the exponent range.
     */
    public static final int SignalClamped = 32;
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
     * Initializes a new PrecisionContext that is a copy of another PrecisionContext.
     */
    public PrecisionContext(PrecisionContext pc) {
      if ((pc) == null) throw new NullPointerException("pc");
      this.hasFlags = pc.hasFlags;
      this.flags = pc.flags;
      this.eMax = pc.eMax;
      this.eMin = pc.eMin;
      this.precision = pc.precision;
      this.rounding = pc.rounding;
      this.clampNormalExponents = pc.clampNormalExponents;
    }
    /**
     * Initializes a new PrecisionContext. HasFlags will be set to false.
     */
    public PrecisionContext(long precision, Rounding rounding, long eMinLong, long eMaxLong) {
      if ((precision) < 0) throw new IllegalArgumentException("precision" + " not greater or equal to " + "0" + " (" + Long.toString((long)(precision)) + ")");
      if ((eMinLong) > eMaxLong) throw new IllegalArgumentException("eMinLong" + " not less or equal to " + Long.toString((long)(eMaxLong)) + " (" + Long.toString((long)(eMinLong)) + ")");
      this.precision = precision;
      this.rounding = rounding;
      this.hasFlags = false;
      this.clampNormalExponents = false;
      eMax = BigInteger.valueOf(eMaxLong);
      eMin = BigInteger.valueOf(eMinLong);
    }
    /**
     * Initializes a new PrecisionContext. HasFlags will be set to false.
     */
    public PrecisionContext(long precision, Rounding rounding, long eMinLong, long eMaxLong,
                            boolean clampNormalExponents) {
      if ((precision) < 0) throw new IllegalArgumentException("precision" + " not greater or equal to " + "0" + " (" + Long.toString((long)(precision)) + ")");
      if ((eMinLong) > eMaxLong) throw new IllegalArgumentException("eMinLong" + " not less or equal to " + Long.toString((long)(eMaxLong)) + " (" + Long.toString((long)(eMinLong)) + ")");
      this.precision = precision;
      this.rounding = rounding;
      this.hasFlags = false;
      this.clampNormalExponents = clampNormalExponents;
      eMax = BigInteger.valueOf(eMaxLong);
      eMin = BigInteger.valueOf(eMinLong);
    }
    /**
     * Initializes a new PrecisionContext. HasFlags will be set to false.
     */
    public PrecisionContext(long precision, Rounding rounding, BigInteger eMin, BigInteger eMax) {
      if ((precision) < 0) throw new IllegalArgumentException("precision" + " not greater or equal to " + "0" + " (" + Long.toString((long)(precision)) + ")");
      if (eMin.compareTo(eMax) > 0) throw new IllegalArgumentException("eMin" + " not less or equal to " + eMax + " (" + eMin + ")");
      this.precision = precision;
      this.rounding = rounding;
      this.hasFlags = false;
      this.clampNormalExponents = false;
      this.eMax = eMax;
      this.eMin = eMin;
    }
    /**
     * Initializes a new PrecisionContext. HasFlags will be set to false.
     */
    public PrecisionContext(long precision, Rounding rounding, BigInteger eMin, BigInteger eMax,
                            boolean clampNormalExponents) {
      if ((precision) < 0) throw new IllegalArgumentException("precision" + " not greater or equal to " + "0" + " (" + Long.toString((long)(precision)) + ")");
      if (eMin.compareTo(eMax) > 0) throw new IllegalArgumentException("eMin" + " not less or equal to " + eMax + " (" + eMin + ")");
      this.precision = precision;
      this.rounding = rounding;
      this.hasFlags = false;
      this.clampNormalExponents = clampNormalExponents;
      this.eMax = eMax;
      this.eMin = eMin;
    }

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
  }
