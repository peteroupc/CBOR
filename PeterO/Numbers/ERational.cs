/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Numbers {
    /// <summary>Arbitrary-precision rational number. This class cannot be
    /// inherited; this is a change in version 2.0 from previous versions,
    /// where the class was inadvertently left inheritable.</summary>
  internal sealed class ERational : IComparable<ERational>,
    IEquatable<ERational> {
    private EInteger unsignedNumerator;

    /// <summary>Gets this object's numerator.</summary>
    /// <value>This object&apos;s numerator. If this object is a
    /// not-a-number value, returns the diagnostic information (which will
    /// be negative if this object is negative).</value>
    public EInteger Numerator {
      get {
        return this.IsNegative ? (-(EInteger)this.unsignedNumerator) :
          this.unsignedNumerator;
      }
    }

    /// <summary>Gets this object's numerator with the sign
    /// removed.</summary>
    /// <value>This object&apos;s numerator. If this object is a
    /// not-a-number value, returns the diagnostic information.</value>
    public EInteger UnsignedNumerator {
      get {
        return this.unsignedNumerator;
      }
    }

    private EInteger denominator;

    /// <summary>Gets this object's denominator.</summary>
    /// <value>This object&apos;s denominator.</value>
    public EInteger Denominator {
      get {
        return this.denominator;
      }
    }

    private int flags;

    #region Equals and GetHashCode implementation
    /// <summary>Determines whether this object and another object are
    /// equal.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object obj) {
      var other = obj as ERational;
      return (
other != null) && (
Object.Equals(
this.unsignedNumerator,
other.unsignedNumerator) && Object.Equals(
this.denominator,
other.denominator) && this.flags == other.flags);
    }

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit hash code.</returns>
    public override int GetHashCode() {
      var hashCode = 1857066527;
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

    /// <summary>Creates a number with the given numerator and
    /// denominator.</summary>
    /// <param name='numeratorSmall'>A 32-bit signed integer.</param>
    /// <param name='denominatorSmall'>A 32-bit signed integer.
    /// (2).</param>
    /// <returns>An ERational object.</returns>
    public static ERational Create(
int numeratorSmall,
int denominatorSmall) {
      return Create((EInteger)numeratorSmall, (EInteger)denominatorSmall);
    }

    /// <summary>Creates a number with the given numerator and
    /// denominator.</summary>
    /// <param name='numerator'>A BigInteger object.</param>
    /// <param name='denominator'>Another BigInteger object.</param>
    /// <returns>An ERational object.</returns>
    public static ERational Create(
EInteger numerator,
EInteger denominator) {
      return new ERational(numerator, denominator);
    }

    /// <summary>Initializes a new instance of the ExtendedRational
    /// class.</summary>
    /// <param name='numerator'>A BigInteger object.</param>
    /// <param name='denominator'>Another BigInteger object.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='numerator'/> or <paramref name='denominator'/> is
    /// null.</exception>
    public ERational(EInteger numerator, EInteger denominator) {
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
   this.flags = (numNegative != denNegative) ? BigNumberFlags.FlagNegative :
        0;
      if (numNegative) {
        numerator = -numerator;
      }
      if (denNegative) {
        denominator = -denominator;
      }
      #if DEBUG
      if (denominator.IsZero) {
        throw new ArgumentException("doesn't satisfy !denominator.IsZero");
      }
      #endif

      this.unsignedNumerator = numerator;
      this.denominator = denominator;
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object. The result can be
    /// Infinity, NaN, or sNaN (with a minus sign before it for negative
    /// values), or a number of the following form:
    /// [-]numerator/denominator.</returns>
    public override string ToString() {
      if (!this.IsFinite) {
        if (this.IsSignalingNaN()) {
          if (this.unsignedNumerator.IsZero) {
            return this.IsNegative ? "-sNaN" : "sNaN";
          }
          return this.IsNegative ? "-sNaN" + this.unsignedNumerator :
              "sNaN" + this.unsignedNumerator;
        }
        if (this.IsQuietNaN()) {
          if (this.unsignedNumerator.IsZero) {
            return this.IsNegative ? "-NaN" : "NaN";
          }
          return this.IsNegative ? "-NaN" + this.unsignedNumerator :
              "NaN" + this.unsignedNumerator;
        }
        if (this.IsInfinity()) {
          return this.IsNegative ? "-Infinity" : "Infinity";
        }
      }
      return this.Numerator + "/" + this.Denominator;
    }

    /// <summary>Converts a big integer to a rational number.</summary>
    /// <param name='bigint'>A BigInteger object.</param>
    /// <returns>The exact value of the integer as a rational
    /// number.</returns>
    public static ERational FromBigInteger(EInteger bigint) {
      return new ERational(bigint, EInteger.One);
    }

    /// <summary>Converts this rational number to a decimal
    /// number.</summary>
    /// <returns>The exact value of the rational number, or not-a-number
    /// (NaN) if the result can't be exact because it has a nonterminating
    /// decimal expansion.</returns>
    public EDecimal ToExtendedDecimal() {
      return this.ToExtendedDecimal(null);
    }

    /// <summary>Converts a 32-bit floating-point number to a rational
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting
    /// the number to a string.</summary>
    /// <param name='flt'>A 32-bit floating-point number.</param>
    /// <returns>A rational number with the same value as <paramref
    /// name='flt'/>.</returns>
    public static ERational FromSingle(float flt) {
      return FromExtendedFloat(EFloat.FromSingle(flt));
    }

    /// <summary>Converts a 64-bit floating-point number to a rational
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting
    /// the number to a string.</summary>
    /// <param name='flt'>A 64-bit floating-point number.</param>
    /// <returns>A rational number with the same value as <paramref
    /// name='flt'/>.</returns>
    public static ERational FromDouble(double flt) {
      return FromExtendedFloat(EFloat.FromDouble(flt));
    }

    /// <summary>Creates a not-a-number ExtendedRational object.</summary>
    /// <param name='diag'>A number to use as diagnostic information
    /// associated with this object. If none is needed, should be
    /// zero.</param>
    /// <returns>An ERational object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='diag'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='diag'/> is less than 0.</exception>
    public static ERational CreateNaN(EInteger diag) {
      return CreateNaN(diag, false, false);
    }

    private static ERational CreateWithFlags(
EInteger numerator,
EInteger denominator,
int flags) {
      var er = new ERational(numerator, denominator);
      er.flags = flags;
      return er;
    }

    /// <summary>Creates a not-a-number ExtendedRational object.</summary>
    /// <param name='diag'>A number to use as diagnostic information
    /// associated with this object. If none is needed, should be
    /// zero.</param>
    /// <param name='signaling'>Whether the return value will be signaling
    /// (true) or quiet (false).</param>
    /// <param name='negative'>Whether the return value is
    /// negative.</param>
    /// <returns>An ERational object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='diag'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='diag'/> is less than 0.</exception>
    public static ERational CreateNaN(
EInteger diag,
bool signaling,
bool negative) {
      if (diag == null) {
        throw new ArgumentNullException("diag");
      }
      if (diag.Sign < 0) {
        throw new
  ArgumentException("Diagnostic information must be 0 or greater, was: " +
          diag);
      }
      if (diag.IsZero && !negative) {
        return signaling ? SignalingNaN : NaN;
      }
      var flags = 0;
      if (negative) {
        flags |= BigNumberFlags.FlagNegative;
      }
      flags |= signaling ? BigNumberFlags.FlagSignalingNaN :
        BigNumberFlags.FlagQuietNaN;
      var er = new ERational(diag, EInteger.Zero);
      er.flags = flags;
      return er;
    }

    /// <param name='ef'>An ExtendedFloat object.</param>
    /// <returns>An ERational object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ef'/> is null.</exception>
    public static ERational FromExtendedFloat(EFloat ef) {
      if (ef == null) {
        throw new ArgumentNullException("ef");
      }
      if (!ef.IsFinite) {
        var er = new ERational(ef.Mantissa, EInteger.One);
        var flags = 0;
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
      EInteger num = ef.Mantissa;
      EInteger exp = ef.Exponent;
      if (exp.IsZero) {
        return FromBigInteger(num);
      }
      bool neg = num.Sign < 0;
      num = num.Abs();
      EInteger den = EInteger.One;
      if (exp.Sign < 0) {
        exp = -(EInteger)exp;
        den = DecimalUtility.ShiftLeft(den, exp);
      } else {
        num = DecimalUtility.ShiftLeft(num, exp);
      }
      if (neg) {
        num = -(EInteger)num;
      }
      return new ERational(num, den);
    }

    /// <param name='ef'>An ExtendedDecimal object.</param>
    /// <returns>An ERational object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ef'/> is null.</exception>
    public static ERational FromExtendedDecimal(EDecimal ef) {
      if (ef == null) {
        throw new ArgumentNullException("ef");
      }
      if (!ef.IsFinite) {
        var er = new ERational(ef.Mantissa, EInteger.One);
        var flags = 0;
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
      EInteger num = ef.Mantissa;
      EInteger exp = ef.Exponent;
      if (exp.IsZero) {
        return FromBigInteger(num);
      }
      bool neg = num.Sign < 0;
      num = num.Abs();
      EInteger den = EInteger.One;
      if (exp.Sign < 0) {
        exp = -(EInteger)exp;
        den = DecimalUtility.FindPowerOfTenFromBig(exp);
      } else {
        EInteger powerOfTen = DecimalUtility.FindPowerOfTenFromBig(exp);
        num *= (EInteger)powerOfTen;
      }
      if (neg) {
        num = -(EInteger)num;
      }
      return new ERational(num, den);
    }

    /// <summary>Converts this rational number to a decimal number and
    /// rounds the result to the given precision.</summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An EDecimal object.</returns>
    public EDecimal ToExtendedDecimal(EContext ctx) {
      if (this.IsNaN()) {
        return EDecimal.CreateNaN(
this.unsignedNumerator,
this.IsSignalingNaN(),
this.IsNegative,
ctx);
      }
      if (this.IsPositiveInfinity()) {
        return EDecimal.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return EDecimal.NegativeInfinity;
      }
      EDecimal ef = (this.IsNegative && this.IsZero) ?
 EDecimal.NegativeZero : EDecimal.FromBigInteger(this.Numerator);
      return ef.Divide(EDecimal.FromBigInteger(this.Denominator), ctx);
    }

    /// <summary>Converts this rational number to a decimal number, but if
    /// the result would have a nonterminating decimal expansion, rounds
    /// that result to the given precision.</summary>
    /// <param name='ctx'>A precision context object to control the
    /// precision. The rounding and exponent range settings of this context
    /// are ignored. This context will be used only if the exact result
    /// would have a nonterminating decimal expansion. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null, in which case this method is the same as
    /// ToExtendedDecimal.</param>
    /// <returns>An EDecimal object.</returns>
public EDecimal ToExtendedDecimalExactIfPossible(EContext
      ctx) {
      if (ctx == null) {
        return this.ToExtendedDecimal(null);
      }
      if (this.IsNaN()) {
        return EDecimal.CreateNaN(
this.unsignedNumerator,
this.IsSignalingNaN(),
this.IsNegative,
ctx);
      }
      if (this.IsPositiveInfinity()) {
        return EDecimal.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return EDecimal.NegativeInfinity;
      }
      if (this.IsNegative && this.IsZero) {
        return EDecimal.NegativeZero;
      }
      EDecimal valueEdNum = (this.IsNegative && this.IsZero) ?
 EDecimal.NegativeZero : EDecimal.FromBigInteger(this.Numerator);
 EDecimal valueEdDen = EDecimal.FromBigInteger(this.Denominator);
      EDecimal ed = valueEdNum.Divide(valueEdDen, null);
      if (ed.IsNaN()) {
        // Result would be inexact, try again using the precision context
        ed = valueEdNum.Divide(valueEdDen, ctx);
      }
      return ed;
    }

    /// <summary>Converts this rational number to a binary
    /// number.</summary>
    /// <returns>The exact value of the rational number, or not-a-number
    /// (NaN) if the result can't be exact because it has a nonterminating
    /// binary expansion.</returns>
    public EFloat ToExtendedFloat() {
      return this.ToExtendedFloat(null);
    }

    /// <summary>Converts this rational number to a binary number and
    /// rounds the result to the given precision.</summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An EFloat object.</returns>
    public EFloat ToExtendedFloat(EContext ctx) {
      if (this.IsNaN()) {
        return EFloat.CreateNaN(
this.unsignedNumerator,
this.IsSignalingNaN(),
this.IsNegative,
ctx);
      }
      if (this.IsPositiveInfinity()) {
        return EFloat.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return EFloat.NegativeInfinity;
      }
      EFloat ef = (this.IsNegative && this.IsZero) ?
     EFloat.NegativeZero : EFloat.FromBigInteger(this.Numerator);
      return ef.Divide(EFloat.FromBigInteger(this.Denominator), ctx);
    }

    /// <summary>Converts this rational number to a binary number, but if
    /// the result would have a nonterminating binary expansion, rounds
    /// that result to the given precision.</summary>
    /// <param name='ctx'>A precision context object to control the
    /// precision. The rounding and exponent range settings of this context
    /// are ignored. This context will be used only if the exact result
    /// would have a nonterminating binary expansion. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null, in which case this method is the same as
    /// ToExtendedFloat.</param>
    /// <returns>An EFloat object.</returns>
    public EFloat ToExtendedFloatExactIfPossible(EContext ctx) {
      if (ctx == null) {
        return this.ToExtendedFloat(null);
      }
      if (this.IsNaN()) {
        return EFloat.CreateNaN(
this.unsignedNumerator,
this.IsSignalingNaN(),
this.IsNegative,
ctx);
      }
      if (this.IsPositiveInfinity()) {
        return EFloat.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return EFloat.NegativeInfinity;
      }
      if (this.IsZero) {
      return this.IsNegative ? EFloat.NegativeZero :
          EFloat.Zero;
      }
      EFloat valueEdNum = (this.IsNegative && this.IsZero) ?
     EFloat.NegativeZero : EFloat.FromBigInteger(this.Numerator);
      EFloat valueEdDen = EFloat.FromBigInteger(this.Denominator);
      EFloat ed = valueEdNum.Divide(valueEdDen, null);
      if (ed.IsNaN()) {
        // Result would be inexact, try again using the precision context
        ed = valueEdNum.Divide(valueEdDen, ctx);
      }
      return ed;
    }

    /// <summary>Gets a value indicating whether this object is finite (not
    /// infinity or NaN).</summary>
    /// <value>True if this object is finite (not infinity or NaN);
    /// otherwise, false.</value>
    public bool IsFinite {
      get {
        return !this.IsNaN() && !this.IsInfinity();
      }
    }

    /// <summary>Converts this value to an arbitrary-precision integer. Any
    /// fractional part in this value will be discarded when converting to
    /// a big integer.</summary>
    /// <returns>An EInteger object.</returns>
    /// <exception cref='OverflowException'>This object's value is infinity
    /// or NaN.</exception>
    public EInteger ToBigInteger() {
      if (!this.IsFinite) {
        throw new OverflowException("Value is infinity or NaN");
      }
      return this.Numerator / (EInteger)this.denominator;
    }

    /// <summary>Converts this value to an arbitrary-precision integer,
    /// checking whether the value is an exact integer.</summary>
    /// <returns>An EInteger object.</returns>
    /// <exception cref='OverflowException'>This object's value is infinity
    /// or NaN.</exception>
    /// <exception cref='ArithmeticException'>This object's value is not an
    /// exact integer.</exception>
    public EInteger ToBigIntegerExact() {
      if (!this.IsFinite) {
        throw new OverflowException("Value is infinity or NaN");
      }
      EInteger rem;
 EInteger quo;
{
EInteger[] divrem=(this.Numerator).DivRem(this.denominator);
quo = divrem[0];
rem = divrem[1]; }
      if (!rem.IsZero) {
        throw new ArithmeticException("Value is not an integral value");
      }
      return quo;
    }

    public static ERational FromInt32(int smallint) {
      return new ERational((EInteger)smallint, EInteger.One);
    }

    public static ERational FromInt64(long longInt) {
      return new ERational((EInteger)longInt, EInteger.One);
    }

    /// <summary>Converts this value to a 64-bit floating-point number. The
    /// half-even rounding mode is used.</summary>
    /// <returns>The closest 64-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 64-bit floating point
    /// number.</returns>
    public double ToDouble() {
      return
  this.ToExtendedFloat(EContext.Binary64.WithRounding(ERounding.Odd))
        .ToDouble();
    }

    /// <summary>Converts this value to a 32-bit floating-point number. The
    /// half-even rounding mode is used.</summary>
    /// <returns>The closest 32-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 32-bit floating point
    /// number.</returns>
    public float ToSingle() {
      return
  this.ToExtendedFloat(EContext.Binary32.WithRounding(ERounding.Odd))
        .ToSingle();
    }

    public ERational Abs() {
      if (this.IsNegative) {
        var er = new ERational(this.unsignedNumerator, this.denominator);
        er.flags = this.flags & ~BigNumberFlags.FlagNegative;
        return er;
      }
      return this;
    }

    public ERational Negate() {
      var er = new ERational(this.unsignedNumerator, this.denominator);
      er.flags = this.flags ^ BigNumberFlags.FlagNegative;
      return er;
    }

    /// <summary>Gets a value indicating whether this object's value equals
    /// 0.</summary>
    /// <value>True if this object&apos;s value equals 0; otherwise,
    /// false.</value>
    public bool IsZero {
      get {
        return ((this.flags & (BigNumberFlags.FlagInfinity |
          BigNumberFlags.FlagNaN)) == 0) && this.unsignedNumerator.IsZero;
      }
    }

    /// <summary>Gets the sign of this rational number.</summary>
    /// <value>Zero if this value is zero or negative zero; -1 if this
    /// value is less than 0; and 1 if this value is greater than
    /// 0.</value>
    public int Sign {
      get {
        return ((this.flags & (BigNumberFlags.FlagInfinity |
          BigNumberFlags.FlagNaN)) != 0) ? (this.IsNegative ? -1 : 1) :
          (this.unsignedNumerator.IsZero ? 0 : (this.IsNegative ? -1 : 1));
      }
    }

    /// <summary>Compares an ExtendedRational object with this
    /// instance.</summary>
    /// <param name='other'>An ExtendedRational object.</param>
    /// <returns>Zero if the values are equal; a negative number if this
    /// instance is less, or a positive number if this instance is
    /// greater.</returns>
    public int CompareTo(ERational other) {
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
      EInteger ad = this.Numerator * (EInteger)other.Denominator;
      EInteger bc = this.Denominator * (EInteger)other.Numerator;
      return ad.CompareTo(bc);
    }

    /// <summary>Compares an ExtendedFloat object with this
    /// instance.</summary>
    /// <param name='other'>An ExtendedFloat object.</param>
    /// <returns>Zero if the values are equal; a negative number if this
    /// instance is less, or a positive number if this instance is
    /// greater.</returns>
    public int CompareToBinary(EFloat other) {
      if (other == null) {
        return 1;
      }
      if (this.IsNaN()) {
        return other.IsNaN() ? 0 : 1;
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
      EInteger bigExponent = other.Exponent;
      if (bigExponent.IsZero) {
        // Special case: other has exponent 0
        EInteger otherMant = other.Mantissa;
        EInteger bcx = this.Denominator * (EInteger)otherMant;
        return this.Numerator.CompareTo(bcx);
      }
      if (bigExponent.Abs().CompareTo((EInteger)1000) > 0) {
        // Other has a high absolute value of exponent, so try different
        // approaches to
        // comparison
        EInteger thisRem;
        EInteger thisInt;
{
EInteger[] divrem=(this.UnsignedNumerator).DivRem(this.Denominator);
thisInt = divrem[0];
thisRem = divrem[1]; }
        EFloat otherAbs = other.Abs();
        EFloat thisIntDec = EFloat.FromBigInteger(thisInt);
        if (thisRem.IsZero) {
          // This object's value is an integer
          // Console.WriteLine("Shortcircuit IV");
          int ret = thisIntDec.CompareTo(otherAbs);
          return this.IsNegative ? -ret : ret;
        }
        if (thisIntDec.CompareTo(otherAbs) > 0) {
          // Truncated absolute value is greater than other's untruncated
          // absolute value
          // Console.WriteLine("Shortcircuit I");
          return this.IsNegative ? -1 : 1;
        }
        // Round up
        thisInt += EInteger.One;
        thisIntDec = EFloat.FromBigInteger(thisInt);
        if (thisIntDec.CompareTo(otherAbs) < 0) {
          // Absolute value rounded up is less than other's unrounded
          // absolute value
          // Console.WriteLine("Shortcircuit II");
          return this.IsNegative ? 1 : -1;
        }
      thisIntDec = EFloat.FromBigInteger(this.UnsignedNumerator).Divide(
          EFloat.FromBigInteger(this.Denominator),
          EContext.ForPrecisionAndRounding(256, ERounding.Down));
        if (thisIntDec.CompareTo(otherAbs) > 0) {
          // Truncated absolute value is greater than other's untruncated
          // absolute value
          // Console.WriteLine("Shortcircuit III");
          return this.IsNegative ? -1 : 1;
        }
        if (other.Exponent.Sign > 0) {
          // NOTE: if unsigned numerator is 0, bitLength will return
          // 0 instead of 1, but the possibility of 0 was already excluded
          int digitCount = this.UnsignedNumerator.bitLength();
          --digitCount;
          var bigDigitCount = (EInteger)digitCount;
          if (bigDigitCount.CompareTo(other.Exponent) < 0) {
            // Numerator's digit count minus 1 is less than the other' s
            // exponent,
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
    ERational otherRational = ERational.FromExtendedFloat(other);
      EInteger ad = this.Numerator * (EInteger)otherRational.Denominator;
      EInteger bc = this.Denominator * (EInteger)otherRational.Numerator;
      return ad.CompareTo(bc);
    }

    /// <summary>Compares an ExtendedDecimal object with this
    /// instance.</summary>
    /// <param name='other'>An ExtendedDecimal object.</param>
    /// <returns>Zero if the values are equal; a negative number if this
    /// instance is less, or a positive number if this instance is
    /// greater.</returns>
    public int CompareToDecimal(EDecimal other) {
      if (other == null) {
        return 1;
      }
      if (this.IsNaN()) {
        return other.IsNaN() ? 0 : 1;
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
        EInteger otherMant = other.Mantissa;
        EInteger bcx = this.Denominator * (EInteger)otherMant;
        return this.Numerator.CompareTo(bcx);
      }
      if (other.Exponent.Abs().CompareTo((EInteger)50) > 0) {
        // Other has a high absolute value of exponent, so try different
        // approaches to
        // comparison
        EInteger thisRem;
        EInteger thisInt;
{
EInteger[] divrem=(this.UnsignedNumerator).DivRem(this.Denominator);
thisInt = divrem[0];
thisRem = divrem[1]; }
        EDecimal otherAbs = other.Abs();
        EDecimal thisIntDec = EDecimal.FromBigInteger(thisInt);
        if (thisRem.IsZero) {
          // This object's value is an integer
          // Console.WriteLine("Shortcircuit IV");
          int ret = thisIntDec.CompareTo(otherAbs);
          return this.IsNegative ? -ret : ret;
        }
        if (thisIntDec.CompareTo(otherAbs) > 0) {
          // Truncated absolute value is greater than other's untruncated
          // absolute value
          // Console.WriteLine("Shortcircuit I");
          return this.IsNegative ? -1 : 1;
        }
        // Round up
        thisInt += EInteger.One;
        thisIntDec = EDecimal.FromBigInteger(thisInt);
        if (thisIntDec.CompareTo(otherAbs) < 0) {
          // Absolute value rounded up is less than other's unrounded
          // absolute value
          // Console.WriteLine("Shortcircuit II");
          return this.IsNegative ? 1 : -1;
        }
        // Conservative approximation of this rational number's absolute value,
        // as a decimal number. The true value will be greater or equal.
    thisIntDec = EDecimal.FromBigInteger(this.UnsignedNumerator).Divide(
          EDecimal.FromBigInteger(this.Denominator),
          EContext.ForPrecisionAndRounding(20, ERounding.Down));
        if (thisIntDec.CompareTo(otherAbs) > 0) {
          // Truncated absolute value is greater than other's untruncated
          // absolute value
          // Console.WriteLine("Shortcircuit III");
          return this.IsNegative ? -1 : 1;
        }
        // Console.WriteLine("---" + this + " " + other);
        if (other.Exponent.Sign > 0) {
          int digitCount = this.UnsignedNumerator.getDigitCount();
          --digitCount;
          var bigDigitCount = (EInteger)digitCount;
          if (bigDigitCount.CompareTo(other.Exponent) < 0) {
            // Numerator's digit count minus 1 is less than the other' s
            // exponent,
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
  ERational otherRational = ERational.FromExtendedDecimal(other);
      EInteger ad = this.Numerator * (EInteger)otherRational.Denominator;
      EInteger bc = this.Denominator * (EInteger)otherRational.Numerator;
      return ad.CompareTo(bc);
    }

    public bool Equals(ERational other) {
      return this.Equals((object)other);
    }

    /// <summary>Returns whether this object is negative
    /// infinity.</summary>
    /// <returns>True if this object is negative infinity; otherwise,
    /// false.</returns>
    public bool IsNegativeInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity |
        BigNumberFlags.FlagNegative)) ==
        (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);
    }

    /// <summary>Returns whether this object is positive
    /// infinity.</summary>
    /// <returns>True if this object is positive infinity; otherwise,
    /// false.</returns>
    public bool IsPositiveInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity |
        BigNumberFlags.FlagNegative)) == BigNumberFlags.FlagInfinity;
    }

    /// <summary>Returns whether this object is a not-a-number
    /// value.</summary>
    /// <returns>True if this object is a not-a-number value; otherwise,
    /// false.</returns>
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
    /// <returns>True if this object's value is infinity; otherwise,
    /// false.</returns>
    public bool IsInfinity() {
      return (this.flags & BigNumberFlags.FlagInfinity) != 0;
    }

    /// <summary>Returns whether this object is a quiet not-a-number
    /// value.</summary>
    /// <returns>True if this object is a quiet not-a-number value;
    /// otherwise, false.</returns>
    public bool IsQuietNaN() {
      return (this.flags & BigNumberFlags.FlagQuietNaN) != 0;
    }

    /// <summary>Returns whether this object is a signaling not-a-number
    /// value (which causes an error if the value is passed to any
    /// arithmetic operation in this class).</summary>
    /// <returns>True if this object is a signaling not-a-number value
    /// (which causes an error if the value is passed to any arithmetic
    /// operation in this class); otherwise, false.</returns>
    public bool IsSignalingNaN() {
      return (this.flags & BigNumberFlags.FlagSignalingNaN) != 0;
    }

    /// <summary>A not-a-number value.</summary>
    public static readonly ERational NaN = CreateWithFlags(
EInteger.Zero,
EInteger.One,
BigNumberFlags.FlagQuietNaN);

    /// <summary>A signaling not-a-number value.</summary>
    public static readonly ERational SignalingNaN =
      CreateWithFlags(
EInteger.Zero,
EInteger.One,
BigNumberFlags.FlagSignalingNaN);

    /// <summary>Positive infinity, greater than any other
    /// number.</summary>
    public static readonly ERational PositiveInfinity =
      CreateWithFlags(
EInteger.Zero,
EInteger.One,
BigNumberFlags.FlagInfinity);

    /// <summary>Negative infinity, less than any other number.</summary>
    public static readonly ERational NegativeInfinity =
      CreateWithFlags(
EInteger.Zero,
EInteger.One,
BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);

    private ERational ChangeSign(bool negative) {
      if (negative) {
        this.flags |= BigNumberFlags.FlagNegative;
      } else {
        this.flags &= ~BigNumberFlags.FlagNegative;
      }
      return this;
    }

    private ERational Simplify() {
      if ((this.flags & BigNumberFlags.FlagSpecial) == 0) {
        int lowBit = this.unsignedNumerator.getLowBit();
        lowBit = Math.Min(lowBit, this.denominator.getLowBit());
        if (lowBit > 0) {
          this.unsignedNumerator >>= lowBit;
          this.denominator >>= lowBit;
        }
      }
      return this;
    }

    /// <summary>Adds two rational numbers.</summary>
    /// <param name='otherValue'>Another ExtendedRational object.</param>
    /// <returns>The sum of the two numbers. Returns NaN if either operand
    /// is NaN.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ERational Add(ERational otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      if (this.IsSignalingNaN()) {
        return CreateNaN(this.unsignedNumerator, false, this.IsNegative);
      }
      if (otherValue.IsSignalingNaN()) {
  return CreateNaN(
otherValue.unsignedNumerator,
false,
otherValue.IsNegative);
      }
      if (this.IsQuietNaN()) {
        return this;
      }
      if (otherValue.IsQuietNaN()) {
        return otherValue;
      }
      if (this.IsInfinity()) {
        return otherValue.IsInfinity() ? ((this.IsNegative ==
          otherValue.IsNegative) ? this : NaN) : this;
      }
      if (otherValue.IsInfinity()) {
        return otherValue;
      }
      EInteger ad = this.Numerator * (EInteger)otherValue.Denominator;
      EInteger bc = this.Denominator * (EInteger)otherValue.Numerator;
      EInteger bd = this.Denominator * (EInteger)otherValue.Denominator;
      ad += (EInteger)bc;
      return new ERational(ad, bd).Simplify();
    }

    /// <summary>Subtracts an ExtendedRational object from this
    /// instance.</summary>
    /// <param name='otherValue'>An ExtendedRational object.</param>
    /// <returns>The difference of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ERational Subtract(ERational otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      if (this.IsSignalingNaN()) {
        return CreateNaN(this.unsignedNumerator, false, this.IsNegative);
      }
      if (otherValue.IsSignalingNaN()) {
  return CreateNaN(
otherValue.unsignedNumerator,
false,
otherValue.IsNegative);
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
      EInteger ad = this.Numerator * (EInteger)otherValue.Denominator;
      EInteger bc = this.Denominator * (EInteger)otherValue.Numerator;
      EInteger bd = this.Denominator * (EInteger)otherValue.Denominator;
      ad -= (EInteger)bc;
      return new ERational(ad, bd).Simplify();
    }

    /// <summary>Multiplies this instance by the value of an
    /// ExtendedRational object.</summary>
    /// <param name='otherValue'>An ExtendedRational object.</param>
    /// <returns>The product of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ERational Multiply(ERational otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      if (this.IsSignalingNaN()) {
        return CreateNaN(this.unsignedNumerator, false, this.IsNegative);
      }
      if (otherValue.IsSignalingNaN()) {
  return CreateNaN(
otherValue.unsignedNumerator,
false,
otherValue.IsNegative);
      }
      if (this.IsQuietNaN()) {
        return this;
      }
      if (otherValue.IsQuietNaN()) {
        return otherValue;
      }
      bool resultNeg = this.IsNegative ^ otherValue.IsNegative;
      if (this.IsInfinity()) {
        return otherValue.IsZero ? NaN : (resultNeg ? NegativeInfinity :
          PositiveInfinity);
      }
      if (otherValue.IsInfinity()) {
  return this.IsZero ? NaN : (resultNeg ? NegativeInfinity :
          PositiveInfinity);
      }
      EInteger ac = this.Numerator * (EInteger)otherValue.Numerator;
      EInteger bd = this.Denominator * (EInteger)otherValue.Denominator;
      return ac.IsZero ? (resultNeg ? NegativeZero : Zero) : new
        ERational(ac, bd).Simplify().ChangeSign(resultNeg);
    }

    /// <summary>Divides this instance by the value of an ExtendedRational
    /// object.</summary>
    /// <param name='otherValue'>An ExtendedRational object.</param>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ERational Divide(ERational otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      if (this.IsSignalingNaN()) {
        return CreateNaN(this.unsignedNumerator, false, this.IsNegative);
      }
      if (otherValue.IsSignalingNaN()) {
  return CreateNaN(
otherValue.unsignedNumerator,
false,
otherValue.IsNegative);
      }
      if (this.IsQuietNaN()) {
        return this;
      }
      if (otherValue.IsQuietNaN()) {
        return otherValue;
      }
      bool resultNeg = this.IsNegative ^ otherValue.IsNegative;
      if (this.IsInfinity()) {
        return otherValue.IsInfinity() ? NaN : (resultNeg ? NegativeInfinity :
          PositiveInfinity);
      }
      if (otherValue.IsInfinity()) {
        return resultNeg ? NegativeZero : Zero;
      }
      if (otherValue.IsZero) {
  return this.IsZero ? NaN : (resultNeg ? NegativeInfinity :
          PositiveInfinity);
      }
      if (this.IsZero) {
        return resultNeg ? NegativeZero : Zero;
      }
      EInteger ad = this.Numerator * (EInteger)otherValue.Denominator;
      EInteger bc = this.Denominator * (EInteger)otherValue.Numerator;
      return new ERational(ad, bc).Simplify().ChangeSign(resultNeg);
    }

    /// <summary>Finds the remainder that results when this instance is
    /// divided by the value of a ExtendedRational object.</summary>
    /// <param name='otherValue'>An ExtendedRational object.</param>
    /// <returns>The remainder of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ERational Remainder(ERational otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      if (this.IsSignalingNaN()) {
        return CreateNaN(this.unsignedNumerator, false, this.IsNegative);
      }
      if (otherValue.IsSignalingNaN()) {
  return CreateNaN(
otherValue.unsignedNumerator,
false,
otherValue.IsNegative);
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
      EInteger ad = this.Numerator * (EInteger)otherValue.Denominator;
      EInteger bc = this.Denominator * (EInteger)otherValue.Numerator;
      EInteger quo = ad / (EInteger)bc;  // Find the integer quotient
      EInteger tnum = quo * (EInteger)otherValue.Numerator;
      EInteger tden = otherValue.Denominator;
      EInteger thisDen = this.Denominator;
      ad = this.Numerator * (EInteger)tden;
      bc = thisDen * (EInteger)tnum;
      tden *= (EInteger)thisDen;
      ad -= (EInteger)bc;
      return new ERational(ad, tden).Simplify().ChangeSign(resultNeg);
    }

    /// <summary>A rational number for zero.</summary>
public static readonly ERational Zero =
      FromBigInteger(EInteger.Zero);

    /// <summary>A rational number for negative zero.</summary>
    public static readonly ERational NegativeZero =
      FromBigInteger(EInteger.Zero).ChangeSign(false);

    /// <summary>The rational number one.</summary>
  public static readonly ERational One = FromBigInteger(EInteger.One);

    /// <summary>The rational number ten.</summary>
  public static readonly ERational Ten = FromBigInteger((EInteger)10);
  }
}
