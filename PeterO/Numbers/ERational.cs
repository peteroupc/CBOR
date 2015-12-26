/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Numbers {
    /// <include file='docs.xml' 
    /// path='docs/doc[@name="T:PeterO.Numbers.ERational"]'/>
  internal sealed class ERational : IComparable<ERational>,
    IEquatable<ERational> {
    private EInteger unsignedNumerator;

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.Numbers.ERational.Numerator"]'/>
    public EInteger Numerator {
      get {
        return this.IsNegative ? (-(EInteger)this.unsignedNumerator) :
          this.unsignedNumerator;
      }
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.Numbers.ERational.UnsignedNumerator"]'/>
    public EInteger UnsignedNumerator {
      get {
        return this.unsignedNumerator;
      }
    }

    private EInteger denominator;

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.Numbers.ERational.Denominator"]'/>
    public EInteger Denominator {
      get {
        return this.denominator;
      }
    }

    private int flags;

    #region Equals and GetHashCode implementation
    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Equals(System.Object)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.GetHashCode"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Create(System.Int32,System.Int32)"]'/>
    public static ERational Create(
int numeratorSmall,
int denominatorSmall) {
      return Create((EInteger)numeratorSmall, (EInteger)denominatorSmall);
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Create(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'/>
    public static ERational Create(
EInteger numerator,
EInteger denominator) {
      return new ERational(numerator, denominator);
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.#ctor(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToString"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.FromBigInteger(PeterO.Numbers.EInteger)"]'/>
    public static ERational FromBigInteger(EInteger bigint) {
      return new ERational(bigint, EInteger.One);
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToExtendedDecimal"]'/>
    public EDecimal ToExtendedDecimal() {
      return this.ToExtendedDecimal(null);
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.FromSingle(System.Single)"]'/>
    public static ERational FromSingle(float flt) {
      return FromExtendedFloat(EFloat.FromSingle(flt));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.FromDouble(System.Double)"]'/>
    public static ERational FromDouble(double flt) {
      return FromExtendedFloat(EFloat.FromDouble(flt));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.CreateNaN(PeterO.Numbers.EInteger)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.CreateNaN(PeterO.Numbers.EInteger,System.Boolean,System.Boolean)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.FromExtendedFloat(PeterO.Numbers.EFloat)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.FromExtendedDecimal(PeterO.Numbers.EDecimal)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToExtendedDecimal(PeterO.Numbers.EContext)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToExtendedDecimalExactIfPossible(PeterO.Numbers.EContext)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToExtendedFloat"]'/>
    public EFloat ToExtendedFloat() {
      return this.ToExtendedFloat(null);
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToExtendedFloat(PeterO.Numbers.EContext)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToExtendedFloatExactIfPossible(PeterO.Numbers.EContext)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.Numbers.ERational.IsFinite"]'/>
    public bool IsFinite {
      get {
        return !this.IsNaN() && !this.IsInfinity();
      }
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToBigInteger"]'/>
    public EInteger ToBigInteger() {
      if (!this.IsFinite) {
        throw new OverflowException("Value is infinity or NaN");
      }
      return this.Numerator / (EInteger)this.denominator;
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToBigIntegerExact"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToDouble"]'/>
    public double ToDouble() {
      return
  this.ToExtendedFloat(EContext.Binary64.WithRounding(ERounding.Odd))
        .ToDouble();
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToSingle"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.Numbers.ERational.IsZero"]'/>
    public bool IsZero {
      get {
        return ((this.flags & (BigNumberFlags.FlagInfinity |
          BigNumberFlags.FlagNaN)) == 0) && this.unsignedNumerator.IsZero;
      }
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.Numbers.ERational.Sign"]'/>
    public int Sign {
      get {
        return ((this.flags & (BigNumberFlags.FlagInfinity |
          BigNumberFlags.FlagNaN)) != 0) ? (this.IsNegative ? -1 : 1) :
          (this.unsignedNumerator.IsZero ? 0 : (this.IsNegative ? -1 : 1));
      }
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.CompareTo(PeterO.Numbers.ERational)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.CompareToBinary(PeterO.Numbers.EFloat)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.CompareToDecimal(PeterO.Numbers.EDecimal)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.IsNegativeInfinity"]'/>
    public bool IsNegativeInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity |
        BigNumberFlags.FlagNegative)) ==
        (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.IsPositiveInfinity"]'/>
    public bool IsPositiveInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity |
        BigNumberFlags.FlagNegative)) == BigNumberFlags.FlagInfinity;
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.IsNaN"]'/>
    public bool IsNaN() {
      return (this.flags & BigNumberFlags.FlagNaN) != 0;
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.Numbers.ERational.IsNegative"]'/>
    public bool IsNegative {
      get {
        return (this.flags & BigNumberFlags.FlagNegative) != 0;
      }
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.IsInfinity"]'/>
    public bool IsInfinity() {
      return (this.flags & BigNumberFlags.FlagInfinity) != 0;
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.IsQuietNaN"]'/>
    public bool IsQuietNaN() {
      return (this.flags & BigNumberFlags.FlagQuietNaN) != 0;
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.IsSignalingNaN"]'/>
    public bool IsSignalingNaN() {
      return (this.flags & BigNumberFlags.FlagSignalingNaN) != 0;
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.NaN"]'/>
    public static readonly ERational NaN = CreateWithFlags(
EInteger.Zero,
EInteger.One,
BigNumberFlags.FlagQuietNaN);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.SignalingNaN"]'/>
    public static readonly ERational SignalingNaN =
      CreateWithFlags(
EInteger.Zero,
EInteger.One,
BigNumberFlags.FlagSignalingNaN);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.PositiveInfinity"]'/>
    public static readonly ERational PositiveInfinity =
      CreateWithFlags(
EInteger.Zero,
EInteger.One,
BigNumberFlags.FlagInfinity);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.NegativeInfinity"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Add(PeterO.Numbers.ERational)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Subtract(PeterO.Numbers.ERational)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Multiply(PeterO.Numbers.ERational)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Divide(PeterO.Numbers.ERational)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Remainder(PeterO.Numbers.ERational)"]'/>
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

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.Zero"]'/>
public static readonly ERational Zero =
      FromBigInteger(EInteger.Zero);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.NegativeZero"]'/>
    public static readonly ERational NegativeZero =
      FromBigInteger(EInteger.Zero).ChangeSign(false);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.One"]'/>
  public static readonly ERational One = FromBigInteger(EInteger.One);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.Ten"]'/>
  public static readonly ERational Ten = FromBigInteger((EInteger)10);
  }
}
