package com.upokecenter.util;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Arbitrary-precision rational number. This class cannot be inherited; this is
     * a change in version 2.0 from previous versions, where the class was
     * inadvertently left inheritable.
     */
  public final class ExtendedRational implements Comparable<ExtendedRational> {
    private BigInteger unsignedNumerator;

    /**
     * Gets this object's numerator.
     * @return This object's numerator. If this object is a not-a-number value,
     * returns the diagnostic information (which will be negative if this
     * object is negative).
     */
    public final BigInteger getNumerator() {
        return this.isNegative() ? ((this.unsignedNumerator).negate()) :
          this.unsignedNumerator;
      }

    /**
     * Gets this object's numerator with the sign removed.
     * @return This object's numerator. If this object is a not-a-number value,
     * returns the diagnostic information.
     */
    public final BigInteger getUnsignedNumerator() {
        return this.unsignedNumerator;
      }

    private BigInteger denominator;

    /**
     * Gets this object's denominator.
     * @return This object's denominator.
     */
    public final BigInteger getDenominator() {
        return this.denominator;
      }

    private int flags;

    /**
     * Determines whether this object and another object are equal.
     * @param obj An arbitrary object.
     * @return True if the objects are equal; otherwise, false.
     */
    @Override public boolean equals(Object obj) {
      ExtendedRational other = ((obj instanceof ExtendedRational) ? (ExtendedRational)obj : null);
      return (
other != null) && (
(((
this.unsignedNumerator) == null) ? ((other.unsignedNumerator) == null) : (
this.unsignedNumerator).equals(other.unsignedNumerator)) && (((
this.denominator) == null) ? ((other.denominator) == null) : (
this.denominator).equals(other.denominator)) && this.flags == other.flags);
    }

    /**
     * Returns the hash code for this instance.
     * @return A 32-bit hash code.
     */
    @Override public int hashCode() {
      int valueHashCode = 1857066527;
      {
        if (this.unsignedNumerator != null) {
          valueHashCode += 1857066539 * this.unsignedNumerator.hashCode();
        }
        if (this.denominator != null) {
          valueHashCode += 1857066551 * this.denominator.hashCode();
        }
        valueHashCode += 1857066623 * this.flags;
      }
      return valueHashCode;
    }

    /**
     * Creates a number with the given numerator and denominator.
     * @param numeratorSmall A 32-bit signed integer.
     * @param denominatorSmall A 32-bit signed integer. (2).
     * @return An ExtendedRational object.
     */
    public static ExtendedRational Create(
int numeratorSmall,
int denominatorSmall) {
      return Create(BigInteger.valueOf(numeratorSmall), BigInteger.valueOf(denominatorSmall));
    }

    /**
     * Creates a number with the given numerator and denominator.
     * @param numerator A BigInteger object.
     * @param denominator A BigInteger object. (2).
     * @return An ExtendedRational object.
     */
    public static ExtendedRational Create(
BigInteger numerator,
BigInteger denominator) {
      return new ExtendedRational(numerator, denominator);
    }

    /**
     * Initializes a new instance of the ExtendedRational class.
     * @param numerator A BigInteger object.
     * @param denominator A BigInteger object. (2).
     * @throws NullPointerException The parameter {@code numerator} or {@code
     * denominator} is null.
     */
    public ExtendedRational (BigInteger numerator, BigInteger denominator) {
      if (numerator == null) {
        throw new NullPointerException("numerator");
      }
      if (denominator == null) {
        throw new NullPointerException("denominator");
      }
      if (denominator.signum() == 0) {
        throw new IllegalArgumentException("denominator is zero");
      }
      boolean numNegative = numerator.signum() < 0;
      boolean denNegative = denominator.signum() < 0;
   this.flags = (numNegative != denNegative) ? BigNumberFlags.FlagNegative :
        0;
      if (numNegative) {
        numerator = numerator.negate();
      }
      if (denNegative) {
        denominator = denominator.negate();
      }

      this.unsignedNumerator = numerator;
      this.denominator = denominator;
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object. The result can be Infinity,
     * NaN, or sNaN (with a minus sign before it for negative values), or a
     * number of the following form: [-]numerator/denominator.
     */
    @Override public String toString() {
      if (!this.isFinite()) {
        if (this.IsSignalingNaN()) {
          if (this.unsignedNumerator.signum() == 0) {
            return this.isNegative() ? "-sNaN" : "sNaN";
          }
          return this.isNegative() ? "-sNaN" + this.unsignedNumerator :
              "sNaN" + this.unsignedNumerator;
        }
        if (this.IsQuietNaN()) {
          if (this.unsignedNumerator.signum() == 0) {
            return this.isNegative() ? "-NaN" : "NaN";
          }
          return this.isNegative() ? "-NaN" + this.unsignedNumerator :
              "NaN" + this.unsignedNumerator;
        }
        if (this.IsInfinity()) {
          return this.isNegative() ? "-Infinity" : "Infinity";
        }
      }
      return this.getNumerator() + "/" + this.getDenominator();
    }

    /**
     * Not documented yet.
     * @param bigint A BigInteger object.
     * @return An ExtendedRational object.
     */
    public static ExtendedRational FromBigInteger(BigInteger bigint) {
      return new ExtendedRational(bigint, BigInteger.ONE);
    }

    /**
     * Converts this rational number to a decimal number.
     * @return The exact value of the rational number, or not-a-number (NaN) if the
     * result can't be exact because it has a nonterminating decimal
     * expansion.
     */
    public ExtendedDecimal ToExtendedDecimal() {
      return this.ToExtendedDecimal(null);
    }

    /**
     * Converts a 32-bit floating-point number to a rational number. This method
     * computes the exact value of the floating point number, not an
     * approximation, as is often the case by converting the number to a
     * string.
     * @param flt A 32-bit floating-point number.
     * @return A rational number with the same value as {@code flt} .
     */
    public static ExtendedRational FromSingle(float flt) {
      return FromExtendedFloat(ExtendedFloat.FromSingle(flt));
    }

    /**
     * Converts a 64-bit floating-point number to a rational number. This method
     * computes the exact value of the floating point number, not an
     * approximation, as is often the case by converting the number to a
     * string.
     * @param flt A 64-bit floating-point number.
     * @return A rational number with the same value as {@code flt} .
     */
    public static ExtendedRational FromDouble(double flt) {
      return FromExtendedFloat(ExtendedFloat.FromDouble(flt));
    }

    /**
     * Creates a not-a-number ExtendedRational object.
     * @param diag A number to use as diagnostic information associated with this
     * object. If none is needed, should be zero.
     * @return An ExtendedRational object.
     * @throws NullPointerException The parameter {@code diag} is null.
     * @throws IllegalArgumentException The parameter {@code diag} is less than 0.
     */
    public static ExtendedRational CreateNaN(BigInteger diag) {
      return CreateNaN(diag, false, false);
    }

    private static ExtendedRational CreateWithFlags(
BigInteger numerator,
BigInteger denominator,
int flags) {
      ExtendedRational er = new ExtendedRational(numerator, denominator);
      er.flags = flags;
      return er;
    }

    /**
     * Creates a not-a-number ExtendedRational object.
     * @param diag A number to use as diagnostic information associated with this
     * object. If none is needed, should be zero.
     * @param signaling Whether the return value will be signaling (true) or quiet
     * (false).
     * @param negative Whether the return value is negative.
     * @return An ExtendedRational object.
     * @throws NullPointerException The parameter {@code diag} is null.
     * @throws IllegalArgumentException The parameter {@code diag} is less than 0.
     */
    public static ExtendedRational CreateNaN(
BigInteger diag,
boolean signaling,
boolean negative) {
      if (diag == null) {
        throw new NullPointerException("diag");
      }
      if (diag.signum() < 0) {
        throw new
  IllegalArgumentException("Diagnostic information must be 0 or greater, was: " +
          diag);
      }
      if (diag.signum() == 0 && !negative) {
        return signaling ? SignalingNaN : NaN;
      }
      int flags = 0;
      if (negative) {
        flags |= BigNumberFlags.FlagNegative;
      }
      flags |= signaling ? BigNumberFlags.FlagSignalingNaN :
        BigNumberFlags.FlagQuietNaN;
      ExtendedRational er = new ExtendedRational(diag, BigInteger.ZERO);
      er.flags = flags;
      return er;
    }

    /**
     * Not documented yet.
     * @param ef An ExtendedFloat object.
     * @return An ExtendedRational object.
     * @throws NullPointerException The parameter {@code ef} is null.
     */
    public static ExtendedRational FromExtendedFloat(ExtendedFloat ef) {
      if (ef == null) {
        throw new NullPointerException("ef");
      }
      if (!ef.isFinite()) {
        ExtendedRational er = new ExtendedRational(ef.getMantissa(), BigInteger.ONE);
        int flags = 0;
        if (ef.isNegative()) {
          flags |= BigNumberFlags.FlagNegative;
        }
        if (ef.IsInfinity()) {
          flags |= BigNumberFlags.FlagInfinity;
        }
        if (ef.IsSignalingNaN()) {
          flags |= BigNumberFlags.FlagSignalingNaN;
        }
        if (ef.IsQuietNaN()) {
          flags |= BigNumberFlags.FlagQuietNaN;
        }
        er.flags = flags;
        return er;
      }
      BigInteger num = ef.getMantissa();
      BigInteger exp = ef.getExponent();
      if (exp.signum() == 0) {
        return FromBigInteger(num);
      }
      boolean neg = num.signum() < 0;
      num = (num).abs();
      BigInteger den = BigInteger.ONE;
      if (exp.signum() < 0) {
        exp = (exp).negate();
        den = DecimalUtility.ShiftLeft(den, exp);
      } else {
        num = DecimalUtility.ShiftLeft(num, exp);
      }
      if (neg) {
        num = (num).negate();
      }
      return new ExtendedRational(num, den);
    }

    /**
     * Not documented yet.
     * @param ef An ExtendedDecimal object.
     * @return An ExtendedRational object.
     * @throws NullPointerException The parameter {@code ef} is null.
     */
    public static ExtendedRational FromExtendedDecimal(ExtendedDecimal ef) {
      if (ef == null) {
        throw new NullPointerException("ef");
      }
      if (!ef.isFinite()) {
        ExtendedRational er = new ExtendedRational(ef.getMantissa(), BigInteger.ONE);
        int flags = 0;
        if (ef.isNegative()) {
          flags |= BigNumberFlags.FlagNegative;
        }
        if (ef.IsInfinity()) {
          flags |= BigNumberFlags.FlagInfinity;
        }
        if (ef.IsSignalingNaN()) {
          flags |= BigNumberFlags.FlagSignalingNaN;
        }
        if (ef.IsQuietNaN()) {
          flags |= BigNumberFlags.FlagQuietNaN;
        }
        er.flags = flags;
        return er;
      }
      BigInteger num = ef.getMantissa();
      BigInteger exp = ef.getExponent();
      if (exp.signum() == 0) {
        return FromBigInteger(num);
      }
      boolean neg = num.signum() < 0;
      num = (num).abs();
      BigInteger den = BigInteger.ONE;
      if (exp.signum() < 0) {
        exp = (exp).negate();
        den = DecimalUtility.FindPowerOfTenFromBig(exp);
      } else {
        BigInteger powerOfTen = DecimalUtility.FindPowerOfTenFromBig(exp);
        num = num.multiply(powerOfTen);
      }
      if (neg) {
        num = (num).negate();
      }
      return new ExtendedRational(num, den);
    }

    /**
     * Converts this rational number to a decimal number and rounds the result to
     * the given precision.
     * @param ctx A PrecisionContext object.
     * @return An ExtendedDecimal object.
     */
    public ExtendedDecimal ToExtendedDecimal(PrecisionContext ctx) {
      if (this.IsNaN()) {
        return ExtendedDecimal.CreateNaN(
this.unsignedNumerator,
this.IsSignalingNaN(),
this.isNegative(),
ctx);
      }
      if (this.IsPositiveInfinity()) {
        return ExtendedDecimal.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return ExtendedDecimal.NegativeInfinity;
      }
      ExtendedDecimal ef = (this.isNegative() && this.signum() == 0) ?
 ExtendedDecimal.NegativeZero :
          ExtendedDecimal.FromBigInteger(this.getNumerator());
      return ef.Divide(ExtendedDecimal.FromBigInteger(this.getDenominator()), ctx);
    }

    /**
     * Converts this rational number to a decimal number, but if the result would
     * have a nonterminating decimal expansion, rounds that result to the
     * given precision.
     * @param ctx A precision context object to control the precision. The rounding
     * and exponent range settings of this context are ignored. This context
     * will be used only if the exact result would have a nonterminating
     * decimal expansion. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null, in which case this
     * method is the same as ToExtendedDecimal.
     * @return An ExtendedDecimal object.
     */
public ExtendedDecimal ToExtendedDecimalExactIfPossible(PrecisionContext
      ctx) {
      if (ctx == null) {
        return this.ToExtendedDecimal(null);
      }
      if (this.IsNaN()) {
        return ExtendedDecimal.CreateNaN(
this.unsignedNumerator,
this.IsSignalingNaN(),
this.isNegative(),
ctx);
      }
      if (this.IsPositiveInfinity()) {
        return ExtendedDecimal.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return ExtendedDecimal.NegativeInfinity;
      }
      if (this.isNegative() && this.signum() == 0) {
        return ExtendedDecimal.NegativeZero;
      }
      ExtendedDecimal valueEdNum = (this.isNegative() && this.signum() == 0) ?
 ExtendedDecimal.NegativeZero :
          ExtendedDecimal.FromBigInteger(this.getNumerator());
 ExtendedDecimal valueEdDen =
        ExtendedDecimal.FromBigInteger(this.getDenominator());
      ExtendedDecimal ed = valueEdNum.Divide(valueEdDen, null);
      if (ed.IsNaN()) {
        // Result would be inexact, try again using the precision context
        ed = valueEdNum.Divide(valueEdDen, ctx);
      }
      return ed;
    }

    /**
     * Converts this rational number to a binary number.
     * @return The exact value of the rational number, or not-a-number (NaN) if the
     * result can't be exact because it has a nonterminating binary
     * expansion.
     */
    public ExtendedFloat ToExtendedFloat() {
      return this.ToExtendedFloat(null);
    }

    /**
     * Converts this rational number to a binary number and rounds the result to
     * the given precision.
     * @param ctx A PrecisionContext object.
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat ToExtendedFloat(PrecisionContext ctx) {
      if (this.IsNaN()) {
        return ExtendedFloat.CreateNaN(
this.unsignedNumerator,
this.IsSignalingNaN(),
this.isNegative(),
ctx);
      }
      if (this.IsPositiveInfinity()) {
        return ExtendedFloat.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return ExtendedFloat.NegativeInfinity;
      }
      ExtendedFloat ef = (this.isNegative() && this.signum() == 0) ?
     ExtendedFloat.NegativeZero :
          ExtendedFloat.FromBigInteger(this.getNumerator());
      return ef.Divide(ExtendedFloat.FromBigInteger(this.getDenominator()), ctx);
    }

    /**
     * Converts this rational number to a binary number, but if the result would
     * have a nonterminating binary expansion, rounds that result to the
     * given precision.
     * @param ctx A precision context object to control the precision. The rounding
     * and exponent range settings of this context are ignored. This context
     * will be used only if the exact result would have a nonterminating
     * binary expansion. If HasFlags of the context is true, will also store
     * the flags resulting from the operation (the flags are in addition to
     * the pre-existing flags). Can be null, in which case this method is
     * the same as ToExtendedFloat.
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat ToExtendedFloatExactIfPossible(PrecisionContext ctx) {
      if (ctx == null) {
        return this.ToExtendedFloat(null);
      }
      if (this.IsNaN()) {
        return ExtendedFloat.CreateNaN(
this.unsignedNumerator,
this.IsSignalingNaN(),
this.isNegative(),
ctx);
      }
      if (this.IsPositiveInfinity()) {
        return ExtendedFloat.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return ExtendedFloat.NegativeInfinity;
      }
      if (this.signum() == 0) {
      return this.isNegative() ? ExtendedFloat.NegativeZero :
          ExtendedFloat.Zero;
      }
      ExtendedFloat valueEdNum = (this.isNegative() && this.signum() == 0) ?
     ExtendedFloat.NegativeZero :
          ExtendedFloat.FromBigInteger(this.getNumerator());
      ExtendedFloat valueEdDen = ExtendedFloat.FromBigInteger(this.getDenominator());
      ExtendedFloat ed = valueEdNum.Divide(valueEdDen, null);
      if (ed.IsNaN()) {
        // Result would be inexact, try again using the precision context
        ed = valueEdNum.Divide(valueEdDen, ctx);
      }
      return ed;
    }

    /**
     * Gets a value indicating whether this object is finite (not infinity or NaN).
     * @return True if this object is finite (not infinity or NaN); otherwise,
     * false.
     */
    public final boolean isFinite() {
        return !this.IsNaN() && !this.IsInfinity();
      }

    /**
     * Converts this value to an arbitrary-precision integer. Any fractional part
     * in this value will be discarded when converting to a big integer.
     * @return A BigInteger object.
     * @throws ArithmeticException This object's value is infinity or NaN.
     */
    public BigInteger ToBigInteger() {
      if (!this.isFinite()) {
        throw new ArithmeticException("Value is infinity or NaN");
      }
      return this.getNumerator().divide(this.denominator);
    }

    /**
     * Converts this value to an arbitrary-precision integer, checking whether the
     * value is an exact integer.
     * @return A BigInteger object.
     * @throws ArithmeticException This object's value is infinity or NaN.
     * @throws ArithmeticException This object's value is not an exact integer.
     */
    public BigInteger ToBigIntegerExact() {
      if (!this.isFinite()) {
        throw new ArithmeticException("Value is infinity or NaN");
      }
      BigInteger rem;
 BigInteger quo;
{
BigInteger[] divrem = (this.getNumerator()).divideAndRemainder(this.denominator);
quo = divrem[0];
rem = divrem[1]; }
      if (rem.signum() != 0) {
        throw new ArithmeticException("Value is not an integral value");
      }
      return quo;
    }

    /**
     * Not documented yet.
     * @param smallint A 32-bit signed integer.
     * @return An ExtendedRational object.
     */
    public static ExtendedRational FromInt32(int smallint) {
      return new ExtendedRational(BigInteger.valueOf(smallint), BigInteger.ONE);
    }

    /**
     * Not documented yet.
     * @param longInt A 64-bit signed integer.
     * @return An ExtendedRational object.
     */
    public static ExtendedRational FromInt64(long longInt) {
      return new ExtendedRational(BigInteger.valueOf(longInt), BigInteger.ONE);
    }

    /**
     * Converts this value to a 64-bit floating-point number. The half-even
     * rounding mode is used.
     * @return The closest 64-bit floating-point number to this value. The return
     * value can be positive infinity or negative infinity if this value
     * exceeds the range of a 64-bit floating point number.
     */
    public double ToDouble() {
      return this.ToExtendedFloat(PrecisionContext.Binary64).ToDouble();
    }

    /**
     * Converts this value to a 32-bit floating-point number. The half-even
     * rounding mode is used.
     * @return The closest 32-bit floating-point number to this value. The return
     * value can be positive infinity or negative infinity if this value
     * exceeds the range of a 32-bit floating point number.
     */
    public float ToSingle() {
      return this.ToExtendedFloat(PrecisionContext.Binary32).ToSingle();
    }

    /**
     * Not documented yet.
     * @return An ExtendedRational object.
     */
    public ExtendedRational Abs() {
      if (this.isNegative()) {
        ExtendedRational er = new ExtendedRational(this.unsignedNumerator, this.denominator);
        er.flags = this.flags & ~BigNumberFlags.FlagNegative;
        return er;
      }
      return this;
    }

    /**
     * Not documented yet.
     * @return An ExtendedRational object.
     */
    public ExtendedRational Negate() {
      ExtendedRational er = new ExtendedRational(this.unsignedNumerator, this.denominator);
      er.flags = this.flags ^ BigNumberFlags.FlagNegative;
      return er;
    }

    /**
     * Gets a value indicating whether this object's value equals 0.
     * @return True if this object's value equals 0; otherwise, false.
     */
    public final boolean isZero() {
        return ((this.flags & (BigNumberFlags.FlagInfinity |
          BigNumberFlags.FlagNaN)) == 0) && this.unsignedNumerator.signum() == 0;
      }

    /**
     * Gets the sign of this rational number.
     * @return Zero if this value is zero or negative zero; -1 if this value is
     * less than 0; and 1 if this value is greater than 0.
     */
    public final int signum() {
        return ((this.flags & (BigNumberFlags.FlagInfinity |
          BigNumberFlags.FlagNaN)) != 0) ? (this.isNegative() ? -1 : 1) :
          (this.unsignedNumerator.signum() == 0 ? 0 : (this.isNegative() ? -1 : 1));
      }

    /**
     * Compares an ExtendedRational object with this instance.
     * @param other An ExtendedRational object.
     * @return Zero if the values are equal; a negative number if this instance is
     * less, or a positive number if this instance is greater.
     */
    public int compareTo(ExtendedRational other) {
      if (other == null) {
        return 1;
      }
      if (this == other) {
        return 0;
      }
      if (this.IsNaN()) {
        return other.IsNaN() ? 0 : 1;
      }
      if (other.IsNaN()) {
        return -1;
      }
      int signA = this.signum();
      int signB = other.signum();
      if (signA != signB) {
        return (signA < signB) ? -1 : 1;
      }
      if (signB == 0 || signA == 0) {
        // Special case: Either operand is zero
        return 0;
      }
      if (this.IsInfinity()) {
        if (other.IsInfinity()) {
          // if we get here, this only means that
          // both are positive infinity or both
          // are negative infinity
          return 0;
        }
        return this.isNegative() ? -1 : 1;
      }
      if (other.IsInfinity()) {
        return other.isNegative() ? 1 : -1;
      }
      // At this point, both numbers are finite and
      // have the same sign

      int dencmp = this.denominator.compareTo(other.denominator);
      // At this point, the signs are equal so we can compare
      // their absolute values instead
      int numcmp = this.unsignedNumerator.compareTo(other.unsignedNumerator);
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
      BigInteger ad = this.getNumerator().multiply(other.getDenominator());
      BigInteger bc = this.getDenominator().multiply(other.getNumerator());
      return ad.compareTo(bc);
    }

    /**
     * Compares an ExtendedFloat object with this instance.
     * @param other An ExtendedFloat object.
     * @return Zero if the values are equal; a negative number if this instance is
     * less, or a positive number if this instance is greater.
     */
    public int CompareToBinary(ExtendedFloat other) {
      if (other == null) {
        return 1;
      }
      if (this.IsNaN()) {
        return other.IsNaN() ? 0 : 1;
      }
      int signA = this.signum();
      int signB = other.signum();
      if (signA != signB) {
        return (signA < signB) ? -1 : 1;
      }
      if (signB == 0 || signA == 0) {
        // Special case: Either operand is zero
        return 0;
      }
      if (this.IsInfinity()) {
        if (other.IsInfinity()) {
          // if we get here, this only means that
          // both are positive infinity or both
          // are negative infinity
          return 0;
        }
        return this.isNegative() ? -1 : 1;
      }
      if (other.IsInfinity()) {
        return other.isNegative() ? 1 : -1;
      }
      // At this point, both numbers are finite and
      // have the same sign

      if (other.getExponent().signum() == 0) {
        // Special case: other has exponent 0
        BigInteger otherMant = other.getMantissa();
        BigInteger bcx = this.getDenominator().multiply(otherMant);
        return this.getNumerator().compareTo(bcx);
      }
      if ((other.getExponent()).abs().compareTo(BigInteger.valueOf(1000)) > 0) {
        // Other has a high absolute value of exponent, so try different
        // approaches to
        // comparison
        BigInteger thisRem;
        BigInteger thisInt;
{
BigInteger[] divrem = (this.getUnsignedNumerator()).divideAndRemainder(this.getDenominator());
thisInt = divrem[0];
thisRem = divrem[1]; }
        ExtendedFloat otherAbs = other.Abs();
        ExtendedFloat thisIntDec = ExtendedFloat.FromBigInteger(thisInt);
        if (thisRem.signum() == 0) {
          // This Object's value is an integer
          // System.out.println("Shortcircuit IV");
          int ret = thisIntDec.compareTo(otherAbs);
          return this.isNegative() ? -ret : ret;
        }
        if (thisIntDec.compareTo(otherAbs) > 0) {
          // Truncated absolute value is greater than other's untruncated
          // absolute value
          // System.out.println("Shortcircuit I");
          return this.isNegative() ? -1 : 1;
        }
        // Round up
        thisInt = thisInt.add(BigInteger.ONE);
        thisIntDec = ExtendedFloat.FromBigInteger(thisInt);
        if (thisIntDec.compareTo(otherAbs) < 0) {
          // Absolute value rounded up is less than other's unrounded
          // absolute value
          // System.out.println("Shortcircuit II");
          return this.isNegative() ? 1 : -1;
        }
      thisIntDec =
          ExtendedFloat.FromBigInteger(this.getUnsignedNumerator()).Divide(
          ExtendedFloat.FromBigInteger(this.getDenominator()),
          PrecisionContext.ForPrecisionAndRounding(256, Rounding.Down));
        if (thisIntDec.compareTo(otherAbs) > 0) {
          // Truncated absolute value is greater than other's untruncated
          // absolute value
          // System.out.println("Shortcircuit III");
          return this.isNegative() ? -1 : 1;
        }
        if (other.getExponent().signum() > 0) {
          // NOTE: if unsigned numerator is 0, bitLength will return
          // 0 instead of 1, but the possibility of 0 was already excluded
          int digitCount = this.getUnsignedNumerator().bitLength();
          --digitCount;
          BigInteger bigDigitCount = BigInteger.valueOf(digitCount);
          if (bigDigitCount.compareTo(other.getExponent()) < 0) {
            // Numerator's digit count minus 1 is less than the other' s
            // exponent,
            // and other's exponent is positive, so this value's absolute
            // value is less
            return this.isNegative() ? 1 : -1;
          }
        }
      }
      // Convert to rational number and use usual rational number
      // comparison
      // System.out.println("no shortcircuit");
      // System.out.println(this);
      // System.out.println(other);
    ExtendedRational otherRational =
        ExtendedRational.FromExtendedFloat(other);
      BigInteger ad = this.getNumerator().multiply(otherRational.getDenominator());
      BigInteger bc = this.getDenominator().multiply(otherRational.getNumerator());
      return ad.compareTo(bc);
    }

    /**
     * Compares an ExtendedDecimal object with this instance.
     * @param other An ExtendedDecimal object.
     * @return Zero if the values are equal; a negative number if this instance is
     * less, or a positive number if this instance is greater.
     */
    public int CompareToDecimal(ExtendedDecimal other) {
      if (other == null) {
        return 1;
      }
      if (this.IsNaN()) {
        return other.IsNaN() ? 0 : 1;
      }
      int signA = this.signum();
      int signB = other.signum();
      if (signA != signB) {
        return (signA < signB) ? -1 : 1;
      }
      if (signB == 0 || signA == 0) {
        // Special case: Either operand is zero
        return 0;
      }
      if (this.IsInfinity()) {
        if (other.IsInfinity()) {
          // if we get here, this only means that
          // both are positive infinity or both
          // are negative infinity
          return 0;
        }
        return this.isNegative() ? -1 : 1;
      }
      if (other.IsInfinity()) {
        return other.isNegative() ? 1 : -1;
      }
      // At this point, both numbers are finite and
      // have the same sign

      if (other.getExponent().signum() == 0) {
        // Special case: other has exponent 0
        BigInteger otherMant = other.getMantissa();
        BigInteger bcx = this.getDenominator().multiply(otherMant);
        return this.getNumerator().compareTo(bcx);
      }
      if ((other.getExponent()).abs().compareTo(BigInteger.valueOf(50)) > 0) {
        // Other has a high absolute value of exponent, so try different
        // approaches to
        // comparison
        BigInteger thisRem;
        BigInteger thisInt;
{
BigInteger[] divrem = (this.getUnsignedNumerator()).divideAndRemainder(this.getDenominator());
thisInt = divrem[0];
thisRem = divrem[1]; }
        ExtendedDecimal otherAbs = other.Abs();
        ExtendedDecimal thisIntDec = ExtendedDecimal.FromBigInteger(thisInt);
        if (thisRem.signum() == 0) {
          // This Object's value is an integer
          // System.out.println("Shortcircuit IV");
          int ret = thisIntDec.compareTo(otherAbs);
          return this.isNegative() ? -ret : ret;
        }
        if (thisIntDec.compareTo(otherAbs) > 0) {
          // Truncated absolute value is greater than other's untruncated
          // absolute value
          // System.out.println("Shortcircuit I");
          return this.isNegative() ? -1 : 1;
        }
        // Round up
        thisInt = thisInt.add(BigInteger.ONE);
        thisIntDec = ExtendedDecimal.FromBigInteger(thisInt);
        if (thisIntDec.compareTo(otherAbs) < 0) {
          // Absolute value rounded up is less than other's unrounded
          // absolute value
          // System.out.println("Shortcircuit II");
          return this.isNegative() ? 1 : -1;
        }
        // Conservative approximation of this rational number's absolute value,
        // as a decimal number. The true value will be greater or equal.
    thisIntDec =
          ExtendedDecimal.FromBigInteger(this.getUnsignedNumerator()).Divide(
          ExtendedDecimal.FromBigInteger(this.getDenominator()),
          PrecisionContext.ForPrecisionAndRounding(20, Rounding.Down));
        if (thisIntDec.compareTo(otherAbs) > 0) {
          // Truncated absolute value is greater than other's untruncated
          // absolute value
          // System.out.println("Shortcircuit III");
          return this.isNegative() ? -1 : 1;
        }
        // System.out.println("---" + this + " " + (other));
        if (other.getExponent().signum() > 0) {
          int digitCount = this.getUnsignedNumerator().getDigitCount();
          --digitCount;
          BigInteger bigDigitCount = BigInteger.valueOf(digitCount);
          if (bigDigitCount.compareTo(other.getExponent()) < 0) {
            // Numerator's digit count minus 1 is less than the other' s
            // exponent,
            // and other's exponent is positive, so this value's absolute
            // value is less
            return this.isNegative() ? 1 : -1;
          }
        }
      }
      // Convert to rational number and use usual rational number
      // comparison
      // System.out.println("no shortcircuit");
      // System.out.println(this);
      // System.out.println(other);
  ExtendedRational otherRational =
        ExtendedRational.FromExtendedDecimal(other);
      BigInteger ad = this.getNumerator().multiply(otherRational.getDenominator());
      BigInteger bc = this.getDenominator().multiply(otherRational.getNumerator());
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

    /**
     * Returns whether this object is negative infinity.
     * @return True if this object is negative infinity; otherwise, false.
     */
    public boolean IsNegativeInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity |
        BigNumberFlags.FlagNegative)) ==
        (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);
    }

    /**
     * Returns whether this object is positive infinity.
     * @return True if this object is positive infinity; otherwise, false.
     */
    public boolean IsPositiveInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity |
        BigNumberFlags.FlagNegative)) ==
        BigNumberFlags.FlagInfinity;
    }

    /**
     * Returns whether this object is a not-a-number value.
     * @return True if this object is a not-a-number value; otherwise, false.
     */
    public boolean IsNaN() {
      return (this.flags & BigNumberFlags.FlagNaN) != 0;
    }

    /**
     * Gets a value indicating whether this object's value is negative (including
     * negative zero).
     * @return True if this object's value is negative; otherwise, false.
     */
    public final boolean isNegative() {
        return (this.flags & BigNumberFlags.FlagNegative) != 0;
      }

    /**
     * Gets a value indicating whether this object's value is infinity.
     * @return True if this object's value is infinity; otherwise, false.
     */
    public boolean IsInfinity() {
      return (this.flags & BigNumberFlags.FlagInfinity) != 0;
    }

    /**
     * Returns whether this object is a quiet not-a-number value.
     * @return True if this object is a quiet not-a-number value; otherwise, false.
     */
    public boolean IsQuietNaN() {
      return (this.flags & BigNumberFlags.FlagQuietNaN) != 0;
    }

    /**
     * Returns whether this object is a signaling not-a-number value (which causes
     * an error if the value is passed to any arithmetic operation in this
     * class).
     * @return True if this object is a signaling not-a-number value (which causes
     * an error if the value is passed to any arithmetic operation in this
     * class); otherwise, false.
     */
    public boolean IsSignalingNaN() {
      return (this.flags & BigNumberFlags.FlagSignalingNaN) != 0;
    }

    /**
     * A not-a-number value.
     */
    public static final ExtendedRational NaN =
      CreateWithFlags(
BigInteger.ZERO,
BigInteger.ONE,
BigNumberFlags.FlagQuietNaN);

    /**
     * A signaling not-a-number value.
     */
    public static final ExtendedRational SignalingNaN =
      CreateWithFlags(
BigInteger.ZERO,
BigInteger.ONE,
BigNumberFlags.FlagSignalingNaN);

    /**
     * Positive infinity, greater than any other number.
     */
    public static final ExtendedRational PositiveInfinity =
      CreateWithFlags(
BigInteger.ZERO,
BigInteger.ONE,
BigNumberFlags.FlagInfinity);

    /**
     * Negative infinity, less than any other number.
     */
    public static final ExtendedRational NegativeInfinity =
      CreateWithFlags(
BigInteger.ZERO,
BigInteger.ONE,
BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);

    private ExtendedRational ChangeSign(boolean negative) {
      if (negative) {
        this.flags |= BigNumberFlags.FlagNegative;
      } else {
        this.flags &= ~BigNumberFlags.FlagNegative;
      }
      return this;
    }

    private ExtendedRational Simplify() {
      if ((this.flags & BigNumberFlags.FlagSpecial) == 0) {
        int lowBit = this.unsignedNumerator.getLowestSetBit();
        lowBit = Math.min(lowBit, this.denominator.getLowestSetBit());
        if (lowBit > 0) {
          this.unsignedNumerator = unsignedNumerator.shiftRight(lowBit);
          this.denominator = denominator.shiftRight(lowBit);
        }
      }
      return this;
    }

    /**
     * Adds two rational numbers.
     * @param otherValue An ExtendedRational object. (2).
     * @return The sum of the two numbers. Returns NaN if either operand is NaN.
     * @throws NullPointerException The parameter {@code otherValue} is null.
     */
    public ExtendedRational Add(ExtendedRational otherValue) {
      if (otherValue == null) {
        throw new NullPointerException("otherValue");
      }
      if (this.IsSignalingNaN()) {
        return CreateNaN(this.unsignedNumerator, false, this.isNegative());
      }
      if (otherValue.IsSignalingNaN()) {
  return CreateNaN(
otherValue.unsignedNumerator,
false,
otherValue.isNegative());
      }
      if (this.IsQuietNaN()) {
        return this;
      }
      if (otherValue.IsQuietNaN()) {
        return otherValue;
      }
      if (this.IsInfinity()) {
        return otherValue.IsInfinity() ? ((this.isNegative() ==
          otherValue.isNegative()) ? this : NaN) : this;
      }
      if (otherValue.IsInfinity()) {
        return otherValue;
      }
      BigInteger ad = this.getNumerator().multiply(otherValue.getDenominator());
      BigInteger bc = this.getDenominator().multiply(otherValue.getNumerator());
      BigInteger bd = this.getDenominator().multiply(otherValue.getDenominator());
      ad = ad.add(bc);
      return new ExtendedRational(ad, bd).Simplify();
    }

    /**
     * Subtracts an ExtendedRational object from this instance.
     * @param otherValue An ExtendedRational object.
     * @return The difference of the two objects.
     * @throws NullPointerException The parameter {@code otherValue} is null.
     */
    public ExtendedRational Subtract(ExtendedRational otherValue) {
      if (otherValue == null) {
        throw new NullPointerException("otherValue");
      }
      if (this.IsSignalingNaN()) {
        return CreateNaN(this.unsignedNumerator, false, this.isNegative());
      }
      if (otherValue.IsSignalingNaN()) {
  return CreateNaN(
otherValue.unsignedNumerator,
false,
otherValue.isNegative());
      }
      if (this.IsQuietNaN()) {
        return this;
      }
      if (otherValue.IsQuietNaN()) {
        return otherValue;
      }
      if (this.IsInfinity()) {
        if (otherValue.IsInfinity()) {
          return (this.isNegative() != otherValue.isNegative()) ?
            (this.isNegative() ? PositiveInfinity : NegativeInfinity) : NaN;
        }
        return this.isNegative() ? PositiveInfinity : NegativeInfinity;
      }
      if (otherValue.IsInfinity()) {
        return otherValue.isNegative() ? PositiveInfinity : NegativeInfinity;
      }
      BigInteger ad = this.getNumerator().multiply(otherValue.getDenominator());
      BigInteger bc = this.getDenominator().multiply(otherValue.getNumerator());
      BigInteger bd = this.getDenominator().multiply(otherValue.getDenominator());
      ad = ad.subtract(bc);
      return new ExtendedRational(ad, bd).Simplify();
    }

    /**
     * Multiplies this instance by the value of an ExtendedRational object.
     * @param otherValue An ExtendedRational object.
     * @return The product of the two objects.
     * @throws NullPointerException The parameter {@code otherValue} is null.
     */
    public ExtendedRational Multiply(ExtendedRational otherValue) {
      if (otherValue == null) {
        throw new NullPointerException("otherValue");
      }
      if (this.IsSignalingNaN()) {
        return CreateNaN(this.unsignedNumerator, false, this.isNegative());
      }
      if (otherValue.IsSignalingNaN()) {
  return CreateNaN(
otherValue.unsignedNumerator,
false,
otherValue.isNegative());
      }
      if (this.IsQuietNaN()) {
        return this;
      }
      if (otherValue.IsQuietNaN()) {
        return otherValue;
      }
      boolean resultNeg = this.isNegative() ^ otherValue.isNegative();
      if (this.IsInfinity()) {
        return otherValue.signum() == 0 ? NaN : (resultNeg ? NegativeInfinity :
          PositiveInfinity);
      }
      if (otherValue.IsInfinity()) {
  return this.signum() == 0 ? NaN : (resultNeg ? NegativeInfinity :
          PositiveInfinity);
      }
      BigInteger ac = this.getNumerator().multiply(otherValue.getNumerator());
      BigInteger bd = this.getDenominator().multiply(otherValue.getDenominator());
      return ac.signum() == 0 ? (resultNeg ? NegativeZero : Zero) : new
        ExtendedRational(ac, bd).Simplify().ChangeSign(resultNeg);
    }

    /**
     * Divides this instance by the value of an ExtendedRational object.
     * @param otherValue An ExtendedRational object.
     * @return The quotient of the two objects.
     * @throws NullPointerException The parameter {@code otherValue} is null.
     */
    public ExtendedRational Divide(ExtendedRational otherValue) {
      if (otherValue == null) {
        throw new NullPointerException("otherValue");
      }
      if (this.IsSignalingNaN()) {
        return CreateNaN(this.unsignedNumerator, false, this.isNegative());
      }
      if (otherValue.IsSignalingNaN()) {
  return CreateNaN(
otherValue.unsignedNumerator,
false,
otherValue.isNegative());
      }
      if (this.IsQuietNaN()) {
        return this;
      }
      if (otherValue.IsQuietNaN()) {
        return otherValue;
      }
      boolean resultNeg = this.isNegative() ^ otherValue.isNegative();
      if (this.IsInfinity()) {
        return otherValue.IsInfinity() ? NaN : (resultNeg ? NegativeInfinity :
          PositiveInfinity);
      }
      if (otherValue.IsInfinity()) {
        return resultNeg ? NegativeZero : Zero;
      }
      if (otherValue.signum() == 0) {
  return this.signum() == 0 ? NaN : (resultNeg ? NegativeInfinity :
          PositiveInfinity);
      }
      if (this.signum() == 0) {
        return resultNeg ? NegativeZero : Zero;
      }
      BigInteger ad = this.getNumerator().multiply(otherValue.getDenominator());
      BigInteger bc = this.getDenominator().multiply(otherValue.getNumerator());
      return new ExtendedRational(ad, bc).Simplify().ChangeSign(resultNeg);
    }

    /**
     * Finds the remainder that results when this instance is divided by the value
     * of a ExtendedRational object.
     * @param otherValue An ExtendedRational object.
     * @return The remainder of the two objects.
     * @throws NullPointerException The parameter {@code otherValue} is null.
     */
    public ExtendedRational Remainder(ExtendedRational otherValue) {
      if (otherValue == null) {
        throw new NullPointerException("otherValue");
      }
      if (this.IsSignalingNaN()) {
        return CreateNaN(this.unsignedNumerator, false, this.isNegative());
      }
      if (otherValue.IsSignalingNaN()) {
  return CreateNaN(
otherValue.unsignedNumerator,
false,
otherValue.isNegative());
      }
      if (this.IsQuietNaN()) {
        return this;
      }
      if (otherValue.IsQuietNaN()) {
        return otherValue;
      }
      boolean resultNeg = this.isNegative() ^ otherValue.isNegative();
      if (this.IsInfinity()) {
        return NaN;
      }
      if (otherValue.IsInfinity()) {
        return this;
      }
      if (otherValue.signum() == 0) {
        return NaN;
      }
      if (this.signum() == 0) {
        return this;
      }
      BigInteger ad = this.getNumerator().multiply(otherValue.getDenominator());
      BigInteger bc = this.getDenominator().multiply(otherValue.getNumerator());
      BigInteger quo = ad.divide(bc);  // Find the integer quotient
      BigInteger tnum = quo.multiply(otherValue.getNumerator());
      BigInteger tden = otherValue.getDenominator();
      BigInteger thisDen = this.getDenominator();
      ad = this.getNumerator().multiply(tden);
      bc = thisDen.multiply(tnum);
      tden = tden.multiply(thisDen);
      ad = ad.subtract(bc);
      return new ExtendedRational(ad, tden).Simplify().ChangeSign(resultNeg);
    }

    /**
     * A rational number for zero.
     */
public static final ExtendedRational Zero =
      FromBigInteger(BigInteger.ZERO);

    /**
     * A rational number for negative zero.
     */
    public static final ExtendedRational NegativeZero =
      FromBigInteger(BigInteger.ZERO).ChangeSign(false);

    /**
     * The rational number one.
     */
  public static final ExtendedRational One =
      FromBigInteger(BigInteger.ONE);

    /**
     * The rational number ten.
     */
  public static final ExtendedRational Ten =
      FromBigInteger(BigInteger.TEN);
  }
