/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;

namespace PeterO {
    /// <summary>Arbitrary-precision rational number.</summary>
  public class ExtendedRational : IComparable<ExtendedRational>, IEquatable<ExtendedRational> {
    private BigInteger unsignedNumerator;

    /// <summary>Gets this object's numerator.</summary>
    /// <value>This object&apos;s numerator. If this object is a not-a-number
    /// value, returns the diagnostic information (which will be negative
    /// if this object is negative).</value>
    public BigInteger Numerator {
      get {
        return this.IsNegative ? (-(BigInteger)this.unsignedNumerator) : this.unsignedNumerator;
      }
    }

    /// <summary>Gets this object's numerator with the sign removed.</summary>
    /// <value>This object&apos;s numerator. If this object is a not-a-number
    /// value, returns the diagnostic information.</value>
    public BigInteger UnsignedNumerator {
      get {
        return this.unsignedNumerator;
      }
    }

    private BigInteger denominator;

    /// <summary>Gets this object's denominator.</summary>
    /// <value>This object&apos;s denominator.</value>
    public BigInteger Denominator {
      get {
        return this.denominator;
      }
    }

    private int flags;

    #region Equals and GetHashCode implementation
    /// <summary>Determines whether this object and another object are
    /// equal.</summary>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    /// <param name='obj'>An arbitrary object.</param>
    public override bool Equals(object obj) {
      ExtendedRational other = obj as ExtendedRational;
      if (other == null) {
        return false;
      }
      return object.Equals(this.unsignedNumerator, other.unsignedNumerator) && object.Equals(this.denominator, other.denominator) && this.flags == other.flags;
    }

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit hash code.</returns>
    public override int GetHashCode() {
      int hashCode = 1857066527;
      unchecked {
        if (this.unsignedNumerator != null) {
          hashCode += 1857066539 * this.unsignedNumerator.GetHashCode();
        }
        if (this.denominator != null) {
          hashCode += 1857066551 * this.denominator.GetHashCode();
        }
        hashCode += 1857066623 * this.flags;
      }
      return hashCode;
    }
    #endregion

    /// <summary>Initializes a new instance of the ExtendedRational class.</summary>
    /// <param name='numerator'>A BigInteger object.</param>
    /// <param name='denominator'>A BigInteger object. (2).</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='numerator'/> or <paramref name='denominator'/>
    /// is null.</exception>
    public ExtendedRational(BigInteger numerator, BigInteger denominator) {
      if (numerator == null) {
        throw new ArgumentNullException("numerator");
      }
      if (denominator == null) {
        throw new ArgumentNullException("denominator");
      }
      if (denominator.IsZero) {
        throw new ArgumentException("denominator is zero");
      }
      bool numNegative = numerator.Sign < 0;
      bool denNegative = denominator.Sign < 0;
      this.flags = (numNegative != denNegative) ? BigNumberFlags.FlagNegative : 0;
      if (numNegative) {
        numerator = -numerator;
      }
      if (denNegative) {
        denominator = -denominator;
      }
      #if DEBUG
      if (!(!denominator.IsZero)) {
        throw new ArgumentException("doesn't satisfy !denominator.IsZero");
      }
      #endif

      this.unsignedNumerator = numerator;
      this.denominator = denominator;
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object. The result can
    /// be Infinity, NaN, or sNaN (with a minus sign before it for negative
    /// values), or a number of the following form: [-]numerator/denominator.</returns>
    public override string ToString() {
      if (!this.IsFinite) {
        if (this.IsSignalingNaN()) {
          if (this.unsignedNumerator.IsZero) {
            return this.IsNegative ? "-sNaN" : "sNaN";
          } else {
            return this.IsNegative ? "-sNaN" + this.unsignedNumerator.ToString() :
              "sNaN" + this.unsignedNumerator.ToString();
          }
        }
        if (this.IsQuietNaN()) {
          if (this.unsignedNumerator.IsZero) {
            return this.IsNegative ? "-NaN" : "NaN";
          } else {
            return this.IsNegative ? "-NaN" + this.unsignedNumerator.ToString() :
              "NaN" + this.unsignedNumerator.ToString();
          }
        }
        if (this.IsInfinity()) {
          return this.IsNegative ? "-Infinity" : "Infinity";
        }
      }
      return this.Numerator + "/" + this.Denominator;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An ExtendedRational object.</returns>
    /// <param name='bigint'>A BigInteger object.</param>
    public static ExtendedRational FromBigInteger(BigInteger bigint) {
      return new ExtendedRational(bigint, BigInteger.One);
    }

    /// <summary>Converts this rational number to a decimal number.</summary>
    /// <returns>The exact value of the rational number, or not-a-number
    /// (NaN) if the result can't be exact because it has a nonterminating
    /// decimal expansion.</returns>
    public ExtendedDecimal ToExtendedDecimal() {
      return this.ToExtendedDecimal(null);
    }

    /// <summary>Converts a 32-bit floating-point number to a rational
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting the
    /// number to a string.</summary>
    /// <returns>A rational number with the same value as <paramref name='flt'/>.</returns>
    /// <param name='flt'>A 32-bit floating-point number.</param>
    public static ExtendedRational FromSingle(float flt) {
      return FromExtendedFloat(ExtendedFloat.FromSingle(flt));
    }

    /// <summary>Converts a 64-bit floating-point number to a rational
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting the
    /// number to a string.</summary>
    /// <returns>A rational number with the same value as <paramref name='flt'/>.</returns>
    /// <param name='flt'>A 64-bit floating-point number.</param>
    public static ExtendedRational FromDouble(double flt) {
      return FromExtendedFloat(ExtendedFloat.FromDouble(flt));
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An ExtendedRational object.</returns>
    /// <param name='diag'>A BigInteger object.</param>
    public static ExtendedRational CreateNaN(BigInteger diag) {
      return CreateNaN(diag, false, false);
    }

    private static ExtendedRational CreateWithFlags(BigInteger numerator, BigInteger denominator, int flags) {
      ExtendedRational er = new ExtendedRational(numerator, denominator);
      er.flags = flags;
      return er;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An ExtendedRational object.</returns>
    /// <param name='diag'>A BigInteger object.</param>
    /// <param name='signaling'>A Boolean object.</param>
    /// <param name='negative'>A Boolean object. (2).</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='diag'/> is null.</exception>
    public static ExtendedRational CreateNaN(BigInteger diag, bool signaling, bool negative) {
      if (diag == null) {
        throw new ArgumentNullException("diag");
      }
      if (diag.Sign < 0) {
        throw new ArgumentException("Diagnostic information must be 0 or greater, was: " + diag);
      }
      if (diag.IsZero && !negative) {
        return signaling ? SignalingNaN : NaN;
      }
      int flags = 0;
      if (negative) {
        flags |= BigNumberFlags.FlagNegative;
      }
      flags |= signaling ? BigNumberFlags.FlagSignalingNaN : BigNumberFlags.FlagQuietNaN;
      ExtendedRational er = new ExtendedRational(diag, BigInteger.Zero);
      er.flags = flags;
      return er;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An ExtendedRational object.</returns>
    /// <param name='ef'>An ExtendedFloat object.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='ef'/> is null.</exception>
    public static ExtendedRational FromExtendedFloat(ExtendedFloat ef) {
      if (ef == null) {
        throw new ArgumentNullException("ef");
      }
      if (!ef.IsFinite) {
        ExtendedRational er = new ExtendedRational(ef.Mantissa, BigInteger.One);
        int flags = 0;
        if (ef.IsNegative) {
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
      BigInteger num = ef.Mantissa;
      BigInteger exp = ef.Exponent;
      if (exp.IsZero) {
        return FromBigInteger(num);
      }
      bool neg = num.Sign < 0;
      num = BigInteger.Abs(num);
      BigInteger den = BigInteger.One;
      if (exp.Sign < 0) {
        exp = -(BigInteger)exp;
        den = DecimalUtility.ShiftLeft(den, exp);
      } else {
        num = DecimalUtility.ShiftLeft(num, exp);
      }
      if (neg) {
        num = -(BigInteger)num;
      }
      return new ExtendedRational(num, den);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An ExtendedRational object.</returns>
    /// <param name='ef'>An ExtendedDecimal object.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='ef'/> is null.</exception>
    public static ExtendedRational FromExtendedDecimal(ExtendedDecimal ef) {
      if (ef == null) {
        throw new ArgumentNullException("ef");
      }
      if (!ef.IsFinite) {
        ExtendedRational er = new ExtendedRational(ef.Mantissa, BigInteger.One);
        int flags = 0;
        if (ef.IsNegative) {
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
      BigInteger num = ef.Mantissa;
      BigInteger exp = ef.Exponent;
      if (exp.IsZero) {
        return FromBigInteger(num);
      }
      bool neg = num.Sign < 0;
      num = BigInteger.Abs(num);
      BigInteger den = BigInteger.One;
      if (exp.Sign < 0) {
        exp = -(BigInteger)exp;
        den = DecimalUtility.FindPowerOfTenFromBig(exp);
      } else {
        BigInteger powerOfTen = DecimalUtility.FindPowerOfTenFromBig(exp);
        num *= (BigInteger)powerOfTen;
      }
      if (neg) {
        num = -(BigInteger)num;
      }
      return new ExtendedRational(num, den);
    }

    /// <summary>Converts this rational number to a decimal number and rounds
    /// the result to the given precision.</summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal ToExtendedDecimal(PrecisionContext ctx) {
      if (this.IsNaN()) {
        return ExtendedDecimal.CreateNaN(this.unsignedNumerator, this.IsSignalingNaN(), this.IsNegative, ctx);
      }
      if (this.IsPositiveInfinity()) {
        return ExtendedDecimal.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return ExtendedDecimal.NegativeInfinity;
      }
      ExtendedDecimal ef = (this.IsNegative && this.IsZero) ?
        ExtendedDecimal.NegativeZero : ExtendedDecimal.FromBigInteger(this.Numerator);
      return ef.Divide(ExtendedDecimal.FromBigInteger(this.Denominator), ctx);
    }

    /// <summary>Converts this rational number to a decimal number, but
    /// if the result would have a nonterminating decimal expansion, rounds
    /// that result to the given precision.</summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal ToExtendedDecimalExactIfPossible(PrecisionContext ctx) {
      if (this.IsNaN()) {
        return ExtendedDecimal.CreateNaN(this.unsignedNumerator, this.IsSignalingNaN(), this.IsNegative, ctx);
      }
      if (this.IsPositiveInfinity()) {
        return ExtendedDecimal.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return ExtendedDecimal.NegativeInfinity;
      }
      if (this.IsNegative && this.IsZero) {
        return ExtendedDecimal.NegativeZero;
      }
      ExtendedDecimal valueEdNum = (this.IsNegative && this.IsZero) ?
        ExtendedDecimal.NegativeZero : ExtendedDecimal.FromBigInteger(this.Numerator);
      ExtendedDecimal valueEdDen = ExtendedDecimal.FromBigInteger(this.Denominator);
      ExtendedDecimal ed = valueEdNum.Divide(valueEdDen, null);
      if (ed.IsNaN()) {
        // Result would be inexact, try again using the precision context
        ed = valueEdNum.Divide(valueEdDen, ctx);
      }
      return ed;
    }

    /// <summary>Converts this rational number to a binary number.</summary>
    /// <returns>The exact value of the rational number, or not-a-number
    /// (NaN) if the result can't be exact because it has a nonterminating
    /// binary expansion.</returns>
    public ExtendedFloat ToExtendedFloat() {
      return this.ToExtendedFloat(null);
    }

    /// <summary>Converts this rational number to a binary number and rounds
    /// the result to the given precision.</summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat ToExtendedFloat(PrecisionContext ctx) {
      if (this.IsNaN()) {
        return ExtendedFloat.CreateNaN(this.unsignedNumerator, this.IsSignalingNaN(), this.IsNegative, ctx);
      }
      if (this.IsPositiveInfinity()) {
        return ExtendedFloat.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return ExtendedFloat.NegativeInfinity;
      }
      ExtendedFloat ef = (this.IsNegative && this.IsZero) ?
        ExtendedFloat.NegativeZero : ExtendedFloat.FromBigInteger(this.Numerator);
      return ef.Divide(ExtendedFloat.FromBigInteger(this.Denominator), ctx);
    }

    /// <summary>Converts this rational number to a binary number, but if
    /// the result would have a nonterminating binary expansion, rounds
    /// that result to the given precision.</summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat ToExtendedFloatExactIfPossible(PrecisionContext ctx) {
      if (this.IsNaN()) {
        return ExtendedFloat.CreateNaN(this.unsignedNumerator, this.IsSignalingNaN(), this.IsNegative, ctx);
      }
      if (this.IsPositiveInfinity()) {
        return ExtendedFloat.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return ExtendedFloat.NegativeInfinity;
      }
      if (this.IsZero) {
        return this.IsNegative ? ExtendedFloat.NegativeZero : ExtendedFloat.Zero;
      }
      ExtendedFloat valueEdNum = (this.IsNegative && this.IsZero) ?
        ExtendedFloat.NegativeZero : ExtendedFloat.FromBigInteger(this.Numerator);
      ExtendedFloat valueEdDen = ExtendedFloat.FromBigInteger(this.Denominator);
      ExtendedFloat ed = valueEdNum.Divide(valueEdDen, null);
      if (ed.IsNaN()) {
        // Result would be inexact, try again using the precision context
        ed = valueEdNum.Divide(valueEdDen, ctx);
      }
      return ed;
    }

    /// <summary>Gets a value indicating whether this object is finite (not
    /// infinity or NaN).</summary>
    /// <value>True if this object is finite (not infinity or NaN); otherwise,
    /// false.</value>
    public bool IsFinite {
      get {
        return !this.IsNaN() && !this.IsInfinity();
      }
    }

    /// <summary>Converts this value to an arbitrary-precision integer.
    /// Any fractional part in this value will be discarded when converting
    /// to a big integer.</summary>
    /// <returns>A BigInteger object.</returns>
    public BigInteger ToBigInteger() {
      if (!this.IsFinite) {
        throw new OverflowException("Value is infinity or NaN");
      }
      return this.Numerator / (BigInteger)this.denominator;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An ExtendedRational object.</returns>
    /// <param name='smallint'>A 32-bit signed integer.</param>
    public static ExtendedRational FromInt32(int smallint) {
      return new ExtendedRational((BigInteger)smallint, BigInteger.One);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An ExtendedRational object.</returns>
    /// <param name='longInt'>A 64-bit signed integer.</param>
    public static ExtendedRational FromInt64(long longInt) {
      return new ExtendedRational((BigInteger)longInt, BigInteger.One);
    }

    /// <summary>Converts this value to a 64-bit floating-point number.
    /// The half-even rounding mode is used.</summary>
    /// <returns>The closest 64-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 64-bit floating point number.</returns>
    public double ToDouble() {
      return this.ToExtendedFloat(PrecisionContext.Binary64).ToDouble();
    }

    /// <summary>Converts this value to a 32-bit floating-point number.
    /// The half-even rounding mode is used.</summary>
    /// <returns>The closest 32-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 32-bit floating point number.</returns>
    public float ToSingle() {
      return this.ToExtendedFloat(PrecisionContext.Binary32).ToSingle();
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An ExtendedRational object.</returns>
    public ExtendedRational Abs() {
      if (this.IsNegative) {
        ExtendedRational er = new ExtendedRational(this.unsignedNumerator, this.denominator);
        er.flags = this.flags & ~BigNumberFlags.FlagNegative;
        return er;
      }
      return this;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An ExtendedRational object.</returns>
    public ExtendedRational Negate() {
      ExtendedRational er = new ExtendedRational(this.unsignedNumerator, this.denominator);
      er.flags = this.flags ^ BigNumberFlags.FlagNegative;
      return er;
    }

    /// <summary>Gets a value indicating whether this object's value equals
    /// 0.</summary>
    /// <value>True if this object&apos;s value equals 0; otherwise, false.</value>
    public bool IsZero {
      get {
        if ((this.flags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNaN)) != 0) {
          return false;
        }
        return this.unsignedNumerator.IsZero;
      }
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public int Sign {
      get {
        if ((this.flags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNaN)) != 0) {
          return this.IsNegative ? -1 : 1;
        }
        if (this.unsignedNumerator.IsZero) {
          return 0;
        }
        return this.IsNegative ? -1 : 1;
      }
    }

    /// <summary>Compares an ExtendedRational object with this instance.</summary>
    /// <param name='other'>An ExtendedRational object.</param>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    public int CompareTo(ExtendedRational other) {
      if (other == null) {
        return 1;
      }
      if (this == other) {
        return 0;
      }
      if (this.IsNaN()) {
        if (other.IsNaN()) {
          return 0;
        }
        return 1;
      }
      if (other.IsNaN()) {
        return -1;
      }
      int signA = this.Sign;
      int signB = other.Sign;
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
        return this.IsNegative ? -1 : 1;
      }
      if (other.IsInfinity()) {
        return other.IsNegative ? 1 : -1;
      }
      // At this point, both numbers are finite and
      // have the same sign
      #if DEBUG
      if (!this.IsFinite) {
        throw new ArgumentException("doesn't satisfy this.IsFinite");
      }
      if (!other.IsFinite) {
        throw new ArgumentException("doesn't satisfy other.IsFinite");
      }
      #endif

      int dencmp = this.denominator.CompareTo(other.denominator);
      // At this point, the signs are equal so we can compare
      // their absolute values instead
      int numcmp = this.unsignedNumerator.CompareTo(other.unsignedNumerator);
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
      BigInteger ad = this.Numerator * (BigInteger)other.Denominator;
      BigInteger bc = this.Denominator * (BigInteger)other.Numerator;
      return ad.CompareTo(bc);
    }

    /// <summary>Compares an ExtendedFloat object with this instance.</summary>
    /// <param name='other'>An ExtendedFloat object.</param>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    public int CompareToBinary(ExtendedFloat other) {
      if (other == null) {
        return 1;
      }
      if (this.IsNaN()) {
        if (other.IsNaN()) {
          return 0;
        }
        return 1;
      }
      int signA = this.Sign;
      int signB = other.Sign;
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
        return this.IsNegative ? -1 : 1;
      }
      if (other.IsInfinity()) {
        return other.IsNegative ? 1 : -1;
      }
      // At this point, both numbers are finite and
      // have the same sign
      #if DEBUG
      if (!this.IsFinite) {
        throw new ArgumentException("doesn't satisfy this.IsFinite");
      }
      if (!other.IsFinite) {
        throw new ArgumentException("doesn't satisfy other.IsFinite");
      }
      #endif
      if (other.Exponent.IsZero) {
        // Special case: other has exponent 0
        BigInteger otherMant = other.Mantissa;
        BigInteger bcx = this.Denominator * (BigInteger)otherMant;
        return this.Numerator.CompareTo(bcx);
      }
      if (BigInteger.Abs(other.Exponent).CompareTo((BigInteger)1000) > 0) {
        // Other has a high absolute value of exponent, so try different approaches to
        // comparison
        BigInteger thisRem;
        BigInteger thisInt = BigInteger.DivRem(this.UnsignedNumerator, this.Denominator, out thisRem);
        ExtendedFloat otherAbs = other.Abs();
        ExtendedFloat thisIntDec = ExtendedFloat.FromBigInteger(thisInt);
        if (thisRem.IsZero) {
          // This object's value is an integer
          // Console.WriteLine("Shortcircuit IV");
          int ret = thisIntDec.CompareTo(otherAbs);
          return this.IsNegative ? -ret : ret;
        }
        if (thisIntDec.CompareTo(otherAbs) > 0) {
          // Truncated absolute value is greater than other's untruncated absolute value
          // Console.WriteLine("Shortcircuit I");
          return this.IsNegative ? -1 : 1;
        }
        // Round up
        thisInt += BigInteger.One;
        thisIntDec = ExtendedFloat.FromBigInteger(thisInt);
        if (thisIntDec.CompareTo(otherAbs) < 0) {
          // Absolute value rounded up is less than other's unrounded absolute value
          // Console.WriteLine("Shortcircuit II");
          return this.IsNegative ? 1 : -1;
        }
        thisIntDec = ExtendedFloat.FromBigInteger(this.UnsignedNumerator).Divide(
          ExtendedFloat.FromBigInteger(this.Denominator),
          PrecisionContext.ForPrecisionAndRounding(256, Rounding.Down));
        if (thisIntDec.CompareTo(otherAbs) > 0) {
          // Truncated absolute value is greater than other's untruncated absolute value
          // Console.WriteLine("Shortcircuit III");
          return this.IsNegative ? -1 : 1;
        }
        if (other.Exponent.Sign > 0) {
          // NOTE: if unsigned numerator is 0, bitLength will return
          // 0 instead of 1, but the possibility of 0 was already excluded
          int digitCount = this.UnsignedNumerator.bitLength();
          --digitCount;
          BigInteger bigDigitCount = (BigInteger)digitCount;
          if (bigDigitCount.CompareTo(other.Exponent) < 0) {
            // Numerator's digit count minus 1 is less than the other's exponent,
            // and other's exponent is positive, so this value's absolute
            // value is less
            return this.IsNegative ? 1 : -1;
          }
        }
      }
      // Convert to rational number and use usual rational number
      // comparison
      // Console.WriteLine("no shortcircuit");
      // Console.WriteLine(this);
      // Console.WriteLine(other);
      ExtendedRational otherRational = ExtendedRational.FromExtendedFloat(other);
      BigInteger ad = this.Numerator * (BigInteger)otherRational.Denominator;
      BigInteger bc = this.Denominator * (BigInteger)otherRational.Numerator;
      return ad.CompareTo(bc);
    }

    /// <summary>Compares an ExtendedDecimal object with this instance.</summary>
    /// <param name='other'>An ExtendedDecimal object.</param>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    public int CompareToDecimal(ExtendedDecimal other) {
      if (other == null) {
        return 1;
      }
      if (this.IsNaN()) {
        if (other.IsNaN()) {
          return 0;
        }
        return 1;
      }
      int signA = this.Sign;
      int signB = other.Sign;
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
        return this.IsNegative ? -1 : 1;
      }
      if (other.IsInfinity()) {
        return other.IsNegative ? 1 : -1;
      }
      // At this point, both numbers are finite and
      // have the same sign
      #if DEBUG
      if (!this.IsFinite) {
        throw new ArgumentException("doesn't satisfy this.IsFinite");
      }
      if (!other.IsFinite) {
        throw new ArgumentException("doesn't satisfy other.IsFinite");
      }
      #endif

      if (other.Exponent.IsZero) {
        // Special case: other has exponent 0
        BigInteger otherMant = other.Mantissa;
        BigInteger bcx = this.Denominator * (BigInteger)otherMant;
        return this.Numerator.CompareTo(bcx);
      }
      if (BigInteger.Abs(other.Exponent).CompareTo((BigInteger)50) > 0) {
        // Other has a high absolute value of exponent, so try different approaches to
        // comparison
        BigInteger thisRem;
        BigInteger thisInt = BigInteger.DivRem(this.UnsignedNumerator, this.Denominator, out thisRem);
        ExtendedDecimal otherAbs = other.Abs();
        ExtendedDecimal thisIntDec = ExtendedDecimal.FromBigInteger(thisInt);
        if (thisRem.IsZero) {
          // This object's value is an integer
          // Console.WriteLine("Shortcircuit IV");
          int ret = thisIntDec.CompareTo(otherAbs);
          return this.IsNegative ? -ret : ret;
        }
        if (thisIntDec.CompareTo(otherAbs) > 0) {
          // Truncated absolute value is greater than other's untruncated absolute value
          // Console.WriteLine("Shortcircuit I");
          return this.IsNegative ? -1 : 1;
        }
        // Round up
        thisInt += BigInteger.One;
        thisIntDec = ExtendedDecimal.FromBigInteger(thisInt);
        if (thisIntDec.CompareTo(otherAbs) < 0) {
          // Absolute value rounded up is less than other's unrounded absolute value
          // Console.WriteLine("Shortcircuit II");
          return this.IsNegative ? 1 : -1;
        }
        // Conservative approximation of this rational number's absolute value,
        // as a decimal number. The true value will be greater or equal.
        thisIntDec = ExtendedDecimal.FromBigInteger(this.UnsignedNumerator).Divide(
          ExtendedDecimal.FromBigInteger(this.Denominator),
          PrecisionContext.ForPrecisionAndRounding(20, Rounding.Down));
        if (thisIntDec.CompareTo(otherAbs) > 0) {
          // Truncated absolute value is greater than other's untruncated absolute value
          // Console.WriteLine("Shortcircuit III");
          return this.IsNegative ? -1 : 1;
        }
        // Console.WriteLine("---" + this + " " + (other));
        if (other.Exponent.Sign > 0) {
          int digitCount = this.UnsignedNumerator.getDigitCount();
          --digitCount;
          BigInteger bigDigitCount = (BigInteger)digitCount;
          if (bigDigitCount.CompareTo(other.Exponent) < 0) {
            // Numerator's digit count minus 1 is less than the other's exponent,
            // and other's exponent is positive, so this value's absolute
            // value is less
            return this.IsNegative ? 1 : -1;
          }
        }
      }
      // Convert to rational number and use usual rational number
      // comparison
      // Console.WriteLine("no shortcircuit");
      // Console.WriteLine(this);
      // Console.WriteLine(other);
      ExtendedRational otherRational = ExtendedRational.FromExtendedDecimal(other);
      BigInteger ad = this.Numerator * (BigInteger)otherRational.Denominator;
      BigInteger bc = this.Denominator * (BigInteger)otherRational.Numerator;
      return ad.CompareTo(bc);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='other'>An ExtendedRational object.</param>
    /// <returns>A Boolean object.</returns>
    public bool Equals(ExtendedRational other) {
      return this.Equals((object)other);
    }

    /// <summary>Returns whether this object is negative infinity.</summary>
    /// <returns>True if this object is negative infinity; otherwise, false.</returns>
    public bool IsNegativeInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) ==
        (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);
    }

    /// <summary>Returns whether this object is positive infinity.</summary>
    /// <returns>True if this object is positive infinity; otherwise, false.</returns>
    public bool IsPositiveInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) ==
        BigNumberFlags.FlagInfinity;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
    public bool IsNaN() {
      return (this.flags & BigNumberFlags.FlagNaN) != 0;
    }

    /// <summary>Gets a value indicating whether this object's value is
    /// negative (including negative zero).</summary>
    /// <value>True if this object&apos;s value is negative; otherwise,
    /// false.</value>
    public bool IsNegative {
      get {
        return (this.flags & BigNumberFlags.FlagNegative) != 0;
      }
    }

    /// <summary>Gets a value indicating whether this object's value is
    /// infinity.</summary>
    /// <returns>True if this object's value is infinity; otherwise, false.</returns>
    public bool IsInfinity() {
      return (this.flags & BigNumberFlags.FlagInfinity) != 0;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
    public bool IsQuietNaN() {
      return (this.flags & BigNumberFlags.FlagQuietNaN) != 0;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
    public bool IsSignalingNaN() {
      return (this.flags & BigNumberFlags.FlagSignalingNaN) != 0;
    }

    /// <summary>A not-a-number value.</summary>
    public static readonly ExtendedRational NaN = CreateWithFlags(BigInteger.Zero, BigInteger.One, BigNumberFlags.FlagQuietNaN);

    /// <summary>A signaling not-a-number value.</summary>
    public static readonly ExtendedRational SignalingNaN = CreateWithFlags(BigInteger.Zero, BigInteger.One, BigNumberFlags.FlagSignalingNaN);

    /// <summary>Positive infinity, greater than any other number.</summary>
    public static readonly ExtendedRational PositiveInfinity = CreateWithFlags(BigInteger.Zero, BigInteger.One, BigNumberFlags.FlagInfinity);

    /// <summary>Negative infinity, less than any other number.</summary>
    public static readonly ExtendedRational NegativeInfinity = CreateWithFlags(BigInteger.Zero, BigInteger.One, BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);

    private ExtendedRational ChangeSign(bool negative) {
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
        lowBit = Math.Min(lowBit, this.denominator.getLowestSetBit());
        if (lowBit > 0) {
          this.unsignedNumerator >>= lowBit;
          this.denominator >>= lowBit;
        }
      }
      return this;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='otherValue'>An ExtendedRational object. (2).</param>
    /// <returns>An ExtendedRational object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='otherValue'/> is null.</exception>
    public ExtendedRational Add(ExtendedRational otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      if (this.IsSignalingNaN()) {
        return CreateNaN(this.unsignedNumerator, false, this.IsNegative);
      }
      if (otherValue.IsSignalingNaN()) {
        return CreateNaN(otherValue.unsignedNumerator, false, otherValue.IsNegative);
      }
      if (this.IsQuietNaN()) {
        return this;
      }
      if (otherValue.IsQuietNaN()) {
        return otherValue;
      }
      if (this.IsInfinity()) {
        if (otherValue.IsInfinity()) {
          return (this.IsNegative == otherValue.IsNegative) ? this : NaN;
        }
        return this;
      }
      if (otherValue.IsInfinity()) {
        return otherValue;
      }
      BigInteger ad = this.Numerator * (BigInteger)otherValue.Denominator;
      BigInteger bc = this.Denominator * (BigInteger)otherValue.Numerator;
      BigInteger bd = this.Denominator * (BigInteger)otherValue.Denominator;
      ad += (BigInteger)bc;
      return new ExtendedRational(ad, bd).Simplify();
    }

    /// <summary>Subtracts an ExtendedRational object from this instance.</summary>
    /// <param name='otherValue'>An ExtendedRational object.</param>
    /// <returns>The difference of the two objects.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='otherValue'/> is null.</exception>
    public ExtendedRational Subtract(ExtendedRational otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      if (this.IsSignalingNaN()) {
        return CreateNaN(this.unsignedNumerator, false, this.IsNegative);
      }
      if (otherValue.IsSignalingNaN()) {
        return CreateNaN(otherValue.unsignedNumerator, false, otherValue.IsNegative);
      }
      if (this.IsQuietNaN()) {
        return this;
      }
      if (otherValue.IsQuietNaN()) {
        return otherValue;
      }
      if (this.IsInfinity()) {
        if (otherValue.IsInfinity()) {
          return (this.IsNegative != otherValue.IsNegative) ?
            (this.IsNegative ? PositiveInfinity : NegativeInfinity) : NaN;
        }
        return this.IsNegative ? PositiveInfinity : NegativeInfinity;
      }
      if (otherValue.IsInfinity()) {
        return otherValue.IsNegative ? PositiveInfinity : NegativeInfinity;
      }
      BigInteger ad = this.Numerator * (BigInteger)otherValue.Denominator;
      BigInteger bc = this.Denominator * (BigInteger)otherValue.Numerator;
      BigInteger bd = this.Denominator * (BigInteger)otherValue.Denominator;
      ad -= (BigInteger)bc;
      return new ExtendedRational(ad, bd).Simplify();
    }

    /// <summary>Multiplies this instance by the value of an ExtendedRational
    /// object.</summary>
    /// <param name='otherValue'>An ExtendedRational object.</param>
    /// <returns>The product of the two objects.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='otherValue'/> is null.</exception>
    public ExtendedRational Multiply(ExtendedRational otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      if (this.IsSignalingNaN()) {
        return CreateNaN(this.unsignedNumerator, false, this.IsNegative);
      }
      if (otherValue.IsSignalingNaN()) {
        return CreateNaN(otherValue.unsignedNumerator, false, otherValue.IsNegative);
      }
      if (this.IsQuietNaN()) {
        return this;
      }
      if (otherValue.IsQuietNaN()) {
        return otherValue;
      }
      bool resultNeg = this.IsNegative ^ otherValue.IsNegative;
      if (this.IsInfinity()) {
        return otherValue.IsZero ? NaN : (resultNeg ? NegativeInfinity : PositiveInfinity);
      }
      if (otherValue.IsInfinity()) {
        return this.IsZero ? NaN : (resultNeg ? NegativeInfinity : PositiveInfinity);
      }
      BigInteger ac = this.Numerator * (BigInteger)otherValue.Numerator;
      BigInteger bd = this.Denominator * (BigInteger)otherValue.Denominator;
      if (ac.IsZero) {
        return resultNeg ? NegativeZero : Zero;
      }
      return new ExtendedRational(ac, bd).Simplify().ChangeSign(resultNeg);
    }

    /// <summary>Divides this instance by the value of an ExtendedRational
    /// object.</summary>
    /// <param name='otherValue'>An ExtendedRational object.</param>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='otherValue'/> is null.</exception>
    public ExtendedRational Divide(ExtendedRational otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      if (this.IsSignalingNaN()) {
        return CreateNaN(this.unsignedNumerator, false, this.IsNegative);
      }
      if (otherValue.IsSignalingNaN()) {
        return CreateNaN(otherValue.unsignedNumerator, false, otherValue.IsNegative);
      }
      if (this.IsQuietNaN()) {
        return this;
      }
      if (otherValue.IsQuietNaN()) {
        return otherValue;
      }
      bool resultNeg = this.IsNegative ^ otherValue.IsNegative;
      if (this.IsInfinity()) {
        return otherValue.IsInfinity() ? NaN : (resultNeg ? NegativeInfinity : PositiveInfinity);
      }
      if (otherValue.IsInfinity()) {
        return resultNeg ? NegativeZero : Zero;
      }
      if (otherValue.IsZero) {
        return this.IsZero ? NaN : (resultNeg ? NegativeInfinity : PositiveInfinity);
      }
      if (this.IsZero) {
        return resultNeg ? NegativeZero : Zero;
      }
      BigInteger ad = this.Numerator * (BigInteger)otherValue.Denominator;
      BigInteger bc = this.Denominator * (BigInteger)otherValue.Numerator;
      return new ExtendedRational(ad, bc).Simplify().ChangeSign(resultNeg);
    }

    /// <summary>Finds the remainder that results when this instance is
    /// divided by the value of a ExtendedRational object.</summary>
    /// <param name='otherValue'>An ExtendedRational object.</param>
    /// <returns>The remainder of the two objects.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='otherValue'/> is null.</exception>
    public ExtendedRational Remainder(ExtendedRational otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      if (this.IsSignalingNaN()) {
        return CreateNaN(this.unsignedNumerator, false, this.IsNegative);
      }
      if (otherValue.IsSignalingNaN()) {
        return CreateNaN(otherValue.unsignedNumerator, false, otherValue.IsNegative);
      }
      if (this.IsQuietNaN()) {
        return this;
      }
      if (otherValue.IsQuietNaN()) {
        return otherValue;
      }
      bool resultNeg = this.IsNegative ^ otherValue.IsNegative;
      if (this.IsInfinity()) {
        return NaN;
      }
      if (otherValue.IsInfinity()) {
        return this;
      }
      if (otherValue.IsZero) {
        return NaN;
      }
      if (this.IsZero) {
        return this;
      }
      BigInteger ad = this.Numerator * (BigInteger)otherValue.Denominator;
      BigInteger bc = this.Denominator * (BigInteger)otherValue.Numerator;
      BigInteger quo = ad / (BigInteger)bc;  // Find the integer quotient
      BigInteger tnum = quo * (BigInteger)otherValue.Numerator;
      BigInteger tden = otherValue.Denominator;
      BigInteger thisDen = this.Denominator;
      ad = this.Numerator * (BigInteger)tden;
      bc = thisDen * (BigInteger)tnum;
      tden *= (BigInteger)thisDen;
      ad -= (BigInteger)bc;
      return new ExtendedRational(ad, tden).Simplify().ChangeSign(resultNeg);
    }

    /// <summary>A rational number for zero.</summary>
    public static readonly ExtendedRational Zero = FromBigInteger(BigInteger.Zero);

    /// <summary>A rational number for negative zero.</summary>
    public static readonly ExtendedRational NegativeZero = FromBigInteger(BigInteger.Zero).ChangeSign(false);

    /// <summary>The rational number one.</summary>
    public static readonly ExtendedRational One = FromBigInteger(BigInteger.One);

    /// <summary>The rational number ten.</summary>
    public static readonly ExtendedRational Ten = FromBigInteger((BigInteger)10);
  }
}
