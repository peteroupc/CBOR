/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using PeterO.Numbers;

namespace PeterO {
    /// <include file='../docs.xml'
    /// path='docs/doc[@name="T:PeterO.ExtendedRational"]/*'/>
  public sealed class ExtendedRational : IComparable<ExtendedRational>,
    IEquatable<ExtendedRational> {
    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedRational.NaN"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly ExtendedRational NaN =
      new ExtendedRational(ERational.NaN);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedRational.NegativeInfinity"]/*'/>
    public static readonly ExtendedRational NegativeInfinity = new
      ExtendedRational(ERational.NegativeInfinity);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedRational.NegativeZero"]/*'/>
    public static readonly ExtendedRational NegativeZero =
      new ExtendedRational(ERational.NegativeZero);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedRational.One"]/*'/>
    public static readonly ExtendedRational One =
      FromBigIntegerInternal(BigInteger.One);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedRational.PositiveInfinity"]/*'/>
    public static readonly ExtendedRational PositiveInfinity = new
      ExtendedRational(ERational.PositiveInfinity);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedRational.SignalingNaN"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly ExtendedRational SignalingNaN = new
      ExtendedRational(ERational.SignalingNaN);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedRational.Ten"]/*'/>
    public static readonly ExtendedRational Ten =
      FromBigIntegerInternal(BigInteger.valueOf(10));

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedRational.Zero"]/*'/>
    public static readonly ExtendedRational Zero =
      FromBigIntegerInternal(BigInteger.Zero);

    private readonly ERational er;

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.#ctor(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    public ExtendedRational(BigInteger numerator, BigInteger denominator) {
      this.er = new ERational(numerator.Ei, denominator.Ei);
    }

    internal ExtendedRational(ERational er) {
      if (er == null) {
        throw new ArgumentNullException("er");
      }
      this.er = er;
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedRational.Denominator"]/*'/>
    public BigInteger Denominator {
      get {
        return new BigInteger(this.Er.Denominator);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedRational.IsFinite"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsFinite {
      get {
        return this.Er.IsFinite;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedRational.IsNegative"]/*'/>
    public bool IsNegative {
      get {
        return this.Er.IsNegative;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedRational.IsZero"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsZero {
      get {
        return this.Er.IsZero;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedRational.Numerator"]/*'/>
    public BigInteger Numerator {
      get {
        return new BigInteger(this.Er.Numerator);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedRational.Sign"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public int Sign {
      get {
        return this.Er.Sign;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedRational.UnsignedNumerator"]/*'/>
    public BigInteger UnsignedNumerator {
      get {
        return new BigInteger(this.Er.UnsignedNumerator);
      }
    }

    internal ERational Er {
      get {
        return this.er;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.Create(System.Int32,System.Int32)"]/*'/>
    public static ExtendedRational Create(
int numeratorSmall,
int denominatorSmall) {
      return new ExtendedRational(
ERational.Create(
numeratorSmall,
denominatorSmall));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.Create(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    public static ExtendedRational Create(
BigInteger numerator,
BigInteger denominator) {
      if (numerator == null) {
        throw new ArgumentNullException("numerator");
      }
      if (denominator == null) {
        throw new ArgumentNullException("denominator");
      }
      return new ExtendedRational(
ERational.Create(
numerator.Ei,
denominator.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.CreateNaN(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedRational CreateNaN(BigInteger diag) {
      if (diag == null) {
        throw new ArgumentNullException("diag");
      }
      return new ExtendedRational(ERational.CreateNaN(diag.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.CreateNaN(PeterO.BigInteger,System.Boolean,System.Boolean)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedRational CreateNaN(
BigInteger diag,
bool signaling,
bool negative) {
      if (diag == null) {
        throw new ArgumentNullException("diag");
      }
      return new ExtendedRational(
ERational.CreateNaN(
diag.Ei,
signaling,
negative));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.FromBigInteger(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedRational FromBigInteger(BigInteger bigint) {
      return FromBigIntegerInternal(bigint);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.FromDouble(System.Double)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedRational FromDouble(double flt) {
      return new ExtendedRational(ERational.FromDouble(flt));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.FromExtendedDecimal(PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedRational FromExtendedDecimal(ExtendedDecimal ef) {
      if (ef == null) {
        throw new ArgumentNullException("ef");
      }
      return new ExtendedRational(ERational.FromExtendedDecimal(ef.Ed));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.FromExtendedFloat(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedRational FromExtendedFloat(ExtendedFloat ef) {
      if (ef == null) {
        throw new ArgumentNullException("ef");
      }
      return new ExtendedRational(ERational.FromExtendedFloat(ef.Ef));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.FromInt32(System.Int32)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedRational FromInt32(int smallint) {
      return new ExtendedRational(ERational.FromInt32(smallint));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.FromInt64(System.Int64)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedRational FromInt64(long longInt) {
      return new ExtendedRational(ERational.FromInt64(longInt));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.FromSingle(System.Single)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedRational FromSingle(float flt) {
      return new ExtendedRational(ERational.FromSingle(flt));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.Abs"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedRational Abs() {
      return new ExtendedRational(this.Er.Abs());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.Add(PeterO.ExtendedRational)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedRational Add(ExtendedRational otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedRational(this.Er.Add(otherValue.Er)).Simplify();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.CompareTo(PeterO.ExtendedRational)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public int CompareTo(ExtendedRational other) {
      if (other == null) {
        throw new ArgumentNullException("other");
      }
      return this.Er.CompareTo(other.Er);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.CompareToBinary(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public int CompareToBinary(ExtendedFloat other) {
      if (other == null) {
        throw new ArgumentNullException("other");
      }
      return this.Er.CompareToBinary(other.Ef);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.CompareToDecimal(PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public int CompareToDecimal(ExtendedDecimal other) {
      if (other == null) {
        throw new ArgumentNullException("other");
      }
      return this.Er.CompareToDecimal(other.Ed);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.Divide(PeterO.ExtendedRational)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedRational Divide(ExtendedRational otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedRational(this.Er.Divide(otherValue.Er)).Simplify();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.Equals(System.Object)"]/*'/>
    public override bool Equals(object obj) {
      var bi = obj as ExtendedRational;
      return (bi == null) ? false : this.Er.Equals(bi.Er);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.Equals(PeterO.ExtendedRational)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool Equals(ExtendedRational other) {
      if (other == null) {
        throw new ArgumentNullException("other");
      }
      return this.Er.Equals(other.Er);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.GetHashCode"]/*'/>
    public override int GetHashCode() {
      return this.Er.GetHashCode();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.IsInfinity"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsInfinity() {
      return this.Er.IsInfinity();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.IsNaN"]/*'/>
    public bool IsNaN() {
      return this.Er.IsNaN();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.IsNegativeInfinity"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsNegativeInfinity() {
      return this.Er.IsNegativeInfinity();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.IsPositiveInfinity"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsPositiveInfinity() {
      return this.Er.IsPositiveInfinity();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.IsQuietNaN"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsQuietNaN() {
      return this.Er.IsQuietNaN();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.IsSignalingNaN"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsSignalingNaN() {
      return this.Er.IsSignalingNaN();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.Multiply(PeterO.ExtendedRational)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedRational Multiply(ExtendedRational otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedRational(this.Er.Multiply(otherValue.Er)).Simplify();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.Negate"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedRational Negate() {
      return new ExtendedRational(this.Er.Negate());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.Remainder(PeterO.ExtendedRational)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedRational Remainder(ExtendedRational otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedRational(this.Er.Remainder(otherValue.Er)).Simplify();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.Subtract(PeterO.ExtendedRational)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedRational Subtract(ExtendedRational otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedRational(this.Er.Subtract(otherValue.Er)).Simplify();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.ToBigInteger"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public BigInteger ToBigInteger() {
      return new BigInteger(this.Er.ToEInteger());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.ToBigIntegerExact"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public BigInteger ToBigIntegerExact() {
      return new BigInteger(this.Er.ToEIntegerExact());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.ToDouble"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public double ToDouble() {
      return this.Er.ToDouble();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.ToExtendedDecimal"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal ToExtendedDecimal() {
      return new ExtendedDecimal(this.Er.ToExtendedDecimal());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.ToExtendedDecimal(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal ToExtendedDecimal(PrecisionContext ctx) {
      return new ExtendedDecimal(this.Er.ToExtendedDecimal(ctx == null ? null :
        ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.ToExtendedDecimalExactIfPossible(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal ToExtendedDecimalExactIfPossible(PrecisionContext
      ctx) {
      return new
  ExtendedDecimal(this.Er.ToExtendedDecimalExactIfPossible(ctx == null ?
          null : ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.ToExtendedFloat"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat ToExtendedFloat() {
      return new ExtendedFloat(this.Er.ToExtendedFloat());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.ToExtendedFloat(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat ToExtendedFloat(PrecisionContext ctx) {
      return new ExtendedFloat(this.Er.ToExtendedFloat(ctx == null ? null :
              ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.ToExtendedFloatExactIfPossible(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat ToExtendedFloatExactIfPossible(PrecisionContext ctx) {
      return new ExtendedFloat(this.Er.ToExtendedFloatExactIfPossible(ctx ==
            null ? null : ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.ToSingle"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public float ToSingle() {
      return this.Er.ToSingle();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.ToString"]/*'/>
    public override string ToString() {
      return this.Er.ToString();
    }

    internal static ERational FromLegacy(ExtendedRational bei) {
      return bei.Er;
    }

    internal static ExtendedRational ToLegacy(ERational ei) {
      return new ExtendedRational(ei);
    }

    private static ExtendedRational FromBigIntegerInternal(BigInteger bigint) {
      return new ExtendedRational(ERational.FromEInteger(bigint.Ei));
    }

#pragma warning disable 618
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "",
      "CS0618",
      Justification = "Implements legacy behavior using several calls to obsolete methods for convenience.")]
    private ExtendedRational Simplify() {
      // TODO: Don't simplify automatically in version 3.0
      if (this.IsFinite) {
        BigInteger num = BigInteger.Abs(this.Numerator);
        BigInteger den = BigInteger.Abs(this.Denominator);
        bool neg = this.IsNegative;
        int lowBit = num.getLowBit();
        lowBit = Math.Min(lowBit, den.getLowBit());
        if (lowBit > 0) {
          num >>= lowBit;
          den >>= lowBit;
          if (neg) {
            num = -num;
          }
          return Create(num, den);
        }
      }
      return this;
    }
#pragma warning restore 618
  }
}
