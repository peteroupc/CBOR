package com.upokecenter.util;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

    /**
     * Arbitrary-precision rational number.
     */
  public class ExtendedRational implements Comparable<ExtendedRational>
  {
    private BigInteger numerator;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public BigInteger getNumerator() {
        return this.numerator;
      }

    private BigInteger denominator;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public BigInteger getDenominator() {
        return this.denominator;
      }

    @Override public boolean equals(Object obj) {
      ExtendedRational other = ((obj instanceof ExtendedRational) ? (ExtendedRational)obj : null);
      if (other == null) {
        return false;
      }
      return (((this.numerator)==null) ? ((other.numerator)==null) : (this.numerator).equals(other.numerator)) && (((this.denominator)==null) ? ((other.denominator)==null) : (this.denominator).equals(other.denominator));
    }

    /**
     * Returns the hash code for this instance.
     * @return A 32-bit hash code.
     */
    @Override public int hashCode() {
      int hashCode_ = 456865663;
      {
        if (this.numerator != null) {
          hashCode_ += 456865807 * this.numerator.hashCode();
        }
        if (this.denominator != null) {
          hashCode_ += 456865823 * this.denominator.hashCode();
        }
      }
      return hashCode_;
    }

    public ExtendedRational (BigInteger numerator, BigInteger denominator) {
      if (numerator == null) {
        throw new NullPointerException("numerator");
      }
      if (denominator == null) {
        throw new NullPointerException("denominator");
      }
      if (denominator.signum()==0) {
        throw new IllegalArgumentException("denominator is zero");
      }
      boolean numNegative = numerator.signum() < 0;
      boolean denNegative = denominator.signum() < 0;
      if (numNegative == denNegative) {
        if (numNegative) {
          numerator=numerator.negate();
        }
      } else {
        if (!numNegative) {
          numerator=numerator.negate();
        }
      }
      if (denNegative) {
        denominator=denominator.negate();
      }

      this.numerator = numerator;
      this.denominator = denominator;
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return "(" + this.numerator + "/" + this.denominator + ")";
    }

    public static ExtendedRational FromBigInteger(BigInteger bigint) {
      return new ExtendedRational(bigint, BigInteger.ONE);
    }

    /**
     * Converts this rational number to a decimal number.
     * @return The exact value of the rational number, or not-a-number (NaN)
     * if the result can't be exact because it has a nonterminating decimal
     * expansion.
     */
    public ExtendedDecimal ToExtendedDecimal() {
      return this.ToExtendedDecimal(null);
    }

    /**
     * Converts this rational number to a decimal number and rounds the result
     * to the given precision.
     * @param ctx A PrecisionContext object.
     * @return An ExtendedDecimal object.
     */
    public ExtendedDecimal ToExtendedDecimal(PrecisionContext ctx) {
      return ExtendedDecimal.FromBigInteger(this.numerator).Divide(ExtendedDecimal.FromBigInteger(this.denominator), ctx);
    }

    /**
     * Converts this rational number to a decimal number, but if the result
     * would have a nonterminating decimal expansion, rounds that result
     * to the given precision.
     * @param ctx A PrecisionContext object.
     * @return An ExtendedDecimal object.
     */
    public ExtendedDecimal ToExtendedDecimalExactIfPossible(PrecisionContext ctx) {
      ExtendedDecimal valueEdNum = ExtendedDecimal.FromBigInteger(this.numerator);
      ExtendedDecimal valueEdDen = ExtendedDecimal.FromBigInteger(this.denominator);
      ExtendedDecimal ed = valueEdNum.Divide(valueEdDen, null);
      if (ed.IsNaN()) {
        // Result would be inexact, try again using the precision context
        ed = valueEdNum.Divide(valueEdDen, ctx);
      }
      return ed;
    }

    /**
     * Converts this rational number to a binary number.
     * @return The exact value of the rational number, or not-a-number (NaN)
     * if the result can't be exact because it has a nonterminating binary
     * expansion.
     */
    public ExtendedFloat ToExtendedFloat() {
      return this.ToExtendedFloat(null);
    }

    /**
     * Converts this rational number to a binary number and rounds the result
     * to the given precision.
     * @param ctx A PrecisionContext object.
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat ToExtendedFloat(PrecisionContext ctx) {
      return ExtendedFloat.FromBigInteger(this.numerator).Divide(ExtendedFloat.FromBigInteger(this.denominator), ctx);
    }

    /**
     * Converts this rational number to a binary number, but if the result
     * would have a nonterminating binary expansion, rounds that result
     * to the given precision.
     * @param ctx A PrecisionContext object.
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat ToExtendedFloatExactIfPossible(PrecisionContext ctx) {
      ExtendedFloat valueEdNum = ExtendedFloat.FromBigInteger(this.numerator);
      ExtendedFloat valueEdDen = ExtendedFloat.FromBigInteger(this.denominator);
      ExtendedFloat ed = valueEdNum.Divide(valueEdDen, null);
      if (ed.IsNaN()) {
        // Result would be inexact, try again using the precision context
        ed = valueEdNum.Divide(valueEdDen, ctx);
      }
      return ed;
    }

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public boolean isFinite() {
        return true;
      }

    /**
     * Converts this value to an arbitrary-precision integer. Any fractional
     * part in this value will be discarded when converting to a big integer.
     * @return A BigInteger object.
     */
    public BigInteger ToBigInteger() {
      return this.numerator.divide(this.denominator);
    }

    public static ExtendedRational FromInt32(int smallint) {
      return new ExtendedRational(BigInteger.valueOf(smallint), BigInteger.ONE);
    }

    public static final ExtendedRational Zero = FromBigInteger(BigInteger.ZERO);
    public static final ExtendedRational One = FromBigInteger(BigInteger.ONE);
    public static final ExtendedRational Ten = FromBigInteger(BigInteger.TEN);

    /**
     * Converts this value to a 64-bit floating-point number. The half-even
     * rounding mode is used.
     * @return The closest 64-bit floating-point number to this value.
     * The return value can be positive infinity or negative infinity if
     * this value exceeds the range of a 64-bit floating point number.
     */
    public double ToDouble() {
      return this.ToExtendedFloat(PrecisionContext.Binary64).ToDouble();
    }

    /**
     * Converts this value to a 32-bit floating-point number. The half-even
     * rounding mode is used.
     * @return The closest 32-bit floating-point number to this value.
     * The return value can be positive infinity or negative infinity if
     * this value exceeds the range of a 32-bit floating point number.
     */
    public float ToSingle() {
      return this.ToExtendedFloat(PrecisionContext.Binary32).ToSingle();
    }

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public boolean isZero() {
        return this.numerator.signum()==0;
      }

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public int signum() {
        return this.numerator.signum();
      }

    /**
     * Compares a ExtendedRational object with this instance.
     * @param other An ExtendedRational object.
     * @return Zero if the values are equal; a negative number if this instance
     * is less, or a positive number if this instance is greater.
     */
    public int compareTo(ExtendedRational other) {
      if (other == null) {
        return 1;
      }
      if (this == other) {
        return 0;
      }
      int signA = this.numerator.signum();
      int signB = other.numerator.signum();
      if (signA != signB) {
        return (signA < signB) ? -1 : 1;
      }
      if (signB == 0 || signA == 0) {
        // Special case: Either operand is zero
        return 0;
      }
      int dencmp = this.denominator.compareTo(other.denominator);
      // At this point, the signs are equal so we can compare
      // their absolute values instead
      int numcmp = (this.numerator).abs().compareTo((other.numerator).abs());
      if (signA < 0) {
        numcmp = -numcmp;
      }
      if (numcmp == 0) {
        // Special case: numerators are equal, so the
        // number with the lower denominator is greater
        return signA < 0 ? dencmp : -dencmp;
      }
      if (dencmp == 0) {
        // denominators are equal
        return numcmp;
      }
      BigInteger ad = this.numerator.multiply(other.denominator);
      BigInteger bc = this.denominator.multiply(other.numerator);
      return ad.compareTo(bc);
    }

    /**
     * Not documented yet.
     * @param other An ExtendedRational object.
     * @return A Boolean object.
     */
    public boolean equals(ExtendedRational other) {
      return this.equals((Object)other);
    }
  }
