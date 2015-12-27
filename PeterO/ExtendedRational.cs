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
    /// <include file='docs.xml'
    /// path='docs/doc[@name="T:PeterO.Numbers.ERational"]'/>
  public sealed class ExtendedRational : IComparable<ExtendedRational>,
    IEquatable<ExtendedRational> {
    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.ERational.Numerator"]'/>
    public  BigInteger Numerator {
get {
return new BigInteger(this.er.Numerator);
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.ERational.UnsignedNumerator"]'/>
    public  BigInteger UnsignedNumerator {
get {
return new BigInteger(this.er.UnsignedNumerator);
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.ERational.Denominator"]'/>
    public  BigInteger Denominator {
get {
return new BigInteger(this.er.Denominator);
} }

    #region Equals and GetHashCode implementation
    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Equals(System.Object)"]'/>
    public override bool Equals(object obj) {
      var bi = obj as ExtendedRational;
      return (bi == null) ? (false) : (this.er.Equals(bi.er));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.GetHashCode"]'/>
    public override int GetHashCode() {
return this.er.GetHashCode();
}
    #endregion

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Create(System.Int32,System.Int32)"]'/>
    public static ExtendedRational Create(int numeratorSmall,
int denominatorSmall) {
return new ExtendedRational(ERational.Create(numeratorSmall, denominatorSmall));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Create(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'/>
    public static ExtendedRational Create(BigInteger numerator,
BigInteger denominator) {
  if ((numerator) == null) {
  throw new ArgumentNullException("numerator");
}
  if ((denominator) == null) {
  throw new ArgumentNullException("denominator");
}
return new ExtendedRational(ERational.Create(numerator.ei, denominator.ei));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.#ctor(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'/>
    public ExtendedRational(BigInteger numerator, BigInteger denominator) {
      this.er = new ERational(numerator.ei, denominator.ei);
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToString"]'/>
    public override string ToString() {
return this.er.ToString();
}

    internal readonly ERational er;
    internal ExtendedRational(ERational er) {
      if ((er) == null) {
  throw new ArgumentNullException("er");
}
      this.er = er;
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.FromBigInteger(PeterO.Numbers.EInteger)"]'/>
    public static ExtendedRational FromBigInteger(BigInteger bigint) {
      return new ExtendedRational(ERational.FromBigInteger(bigint.ei));
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToExtendedDecimal"]'/>
    public ExtendedDecimal ToExtendedDecimal() {
return new ExtendedDecimal(this.er.ToExtendedDecimal());
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.FromSingle(System.Single)"]'/>
    public static ExtendedRational FromSingle(float flt) {
return new ExtendedRational(ERational.FromSingle(flt));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.FromDouble(System.Double)"]'/>
    public static ExtendedRational FromDouble(double flt) {
return new ExtendedRational(ERational.FromDouble(flt));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.CreateNaN(PeterO.Numbers.EInteger)"]'/>
    public static ExtendedRational CreateNaN(BigInteger diag) {
  if ((diag) == null) {
  throw new ArgumentNullException("diag");
}
return new ExtendedRational(ERational.CreateNaN(diag.ei));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.CreateNaN(PeterO.Numbers.EInteger,System.Boolean,System.Boolean)"]'/>
    public static ExtendedRational CreateNaN(BigInteger diag,
bool signaling,
bool negative) {
  if ((diag) == null) {
  throw new ArgumentNullException("diag");
}
return new ExtendedRational(ERational.CreateNaN(diag.ei, signaling, negative));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.FromExtendedFloat(PeterO.Numbers.EFloat)"]'/>
    public static ExtendedRational FromExtendedFloat(ExtendedFloat ef) {
  if ((ef) == null) {
  throw new ArgumentNullException("ef");
}
return new ExtendedRational(ERational.FromExtendedFloat(ef.ef));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.FromExtendedDecimal(PeterO.Numbers.EDecimal)"]'/>
    public static ExtendedRational FromExtendedDecimal(ExtendedDecimal ef) {
  if ((ef) == null) {
  throw new ArgumentNullException("ef");
}
return new ExtendedRational(ERational.FromExtendedDecimal(ef.ed));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToExtendedDecimal(PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal ToExtendedDecimal(PrecisionContext ctx) {
return new ExtendedDecimal(this.er.ToExtendedDecimal(ctx == null ? null :
  ctx.ec));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToExtendedDecimalExactIfPossible(PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal ToExtendedDecimalExactIfPossible(PrecisionContext
      ctx) {
return new
  ExtendedDecimal(this.er.ToExtendedDecimalExactIfPossible(ctx == null ? null:
  ctx.ec));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToExtendedFloat"]'/>
    public ExtendedFloat ToExtendedFloat() {
return new ExtendedFloat(this.er.ToExtendedFloat());
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToExtendedFloat(PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat ToExtendedFloat(PrecisionContext ctx) {
return new ExtendedFloat(this.er.ToExtendedFloat(ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToExtendedFloatExactIfPossible(PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat ToExtendedFloatExactIfPossible(PrecisionContext ctx) {
return new ExtendedFloat(this.er.ToExtendedFloatExactIfPossible(ctx == null ?
  null : ctx.ec));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.ERational.IsFinite"]'/>
    public  bool IsFinite {
get {
return this.er.IsFinite;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToBigInteger"]'/>
    public BigInteger ToBigInteger() {
return new BigInteger(this.er.ToBigInteger());
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToBigIntegerExact"]'/>
    public BigInteger ToBigIntegerExact() {
return new BigInteger(this.er.ToBigIntegerExact());
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.FromInt32(System.Int32)"]'/>
    public static ExtendedRational FromInt32(int smallint) {
return new ExtendedRational(ERational.FromInt32(smallint));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.FromInt64(System.Int64)"]'/>
    public static ExtendedRational FromInt64(long longInt) {
return new ExtendedRational(ERational.FromInt64(longInt));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToDouble"]'/>
    public double ToDouble() {
return this.er.ToDouble();
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.ToSingle"]'/>
    public float ToSingle() {
return this.er.ToSingle();
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.Abs"]'/>
    public ExtendedRational Abs() {
return new ExtendedRational(this.er.Abs());
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.Negate"]'/>
    public ExtendedRational Negate() {
return new ExtendedRational(this.er.Negate());
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.ERational.IsZero"]'/>
    public  bool IsZero {
get {
return this.er.IsZero;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.ERational.Sign"]'/>
    public  int Sign {
get {
return this.er.Sign;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.CompareTo(PeterO.Numbers.ERational)"]'/>
    public int CompareTo(ExtendedRational other) {
  if ((other) == null) {
  throw new ArgumentNullException("other");
}
return this.er.CompareTo(other.er);
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.CompareToBinary(PeterO.Numbers.EFloat)"]'/>
    public int CompareToBinary(ExtendedFloat other) {
  if ((other) == null) {
  throw new ArgumentNullException("other");
}
return this.er.CompareToBinary(other.ef);
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.CompareToDecimal(PeterO.Numbers.EDecimal)"]'/>
    public int CompareToDecimal(ExtendedDecimal other) {
  if ((other) == null) {
  throw new ArgumentNullException("other");
}
return this.er.CompareToDecimal(other.ed);
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.Equals(PeterO.ExtendedRational)"]'/>
    public bool Equals(ExtendedRational other) {
  if ((other) == null) {
  throw new ArgumentNullException("other");
}
return this.er.Equals(other.er);
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.IsNegativeInfinity"]'/>
    public bool IsNegativeInfinity() {
return this.er.IsNegativeInfinity();
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.IsPositiveInfinity"]'/>
    public bool IsPositiveInfinity() {
return this.er.IsPositiveInfinity();
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.IsNaN"]'/>
    public bool IsNaN() {
return this.er.IsNaN();
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.ERational.IsNegative"]'/>
    public  bool IsNegative {
get {
return this.er.IsNegative;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.IsInfinity"]'/>
    public bool IsInfinity() {
return this.er.IsInfinity();
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.IsQuietNaN"]'/>
    public bool IsQuietNaN() {
return this.er.IsQuietNaN();
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.IsSignalingNaN"]'/>
    public bool IsSignalingNaN() {
return this.er.IsSignalingNaN();
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.NaN"]'/>
    public static readonly ExtendedRational NaN =
      new ExtendedRational(ERational.NaN);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.SignalingNaN"]'/>
    public static readonly ExtendedRational SignalingNaN = new
      ExtendedRational(ERational.SignalingNaN);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.PositiveInfinity"]'/>
    public static readonly ExtendedRational PositiveInfinity = new
      ExtendedRational(ERational.PositiveInfinity);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.NegativeInfinity"]'/>
    public static readonly ExtendedRational NegativeInfinity = new
      ExtendedRational(ERational.NegativeInfinity);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Add(PeterO.Numbers.ERational)"]'/>
    public ExtendedRational Add(ExtendedRational otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedRational(this.er.Add(otherValue.er));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Subtract(PeterO.Numbers.ERational)"]'/>
    public ExtendedRational Subtract(ExtendedRational otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedRational(this.er.Subtract(otherValue.er));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Multiply(PeterO.Numbers.ERational)"]'/>
    public ExtendedRational Multiply(ExtendedRational otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedRational(this.er.Multiply(otherValue.er));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Divide(PeterO.Numbers.ERational)"]'/>
    public ExtendedRational Divide(ExtendedRational otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedRational(this.er.Divide(otherValue.er));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.ERational.Remainder(PeterO.Numbers.ERational)"]'/>
    public ExtendedRational Remainder(ExtendedRational otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedRational(this.er.Remainder(otherValue.er));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.Zero"]'/>
public static readonly ExtendedRational Zero =
      FromBigInteger(BigInteger.Zero);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.NegativeZero"]'/>
    public static readonly ExtendedRational NegativeZero =
      new ExtendedRational(ERational.NegativeZero);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.One"]'/>
  public static readonly ExtendedRational One =
      FromBigInteger(BigInteger.One);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.ERational.Ten"]'/>
  public static readonly ExtendedRational Ten =
      FromBigInteger((BigInteger)10);
  }
}
