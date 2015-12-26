/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Text;
using PeterO.Numbers;

namespace PeterO {
    /// <include file='docs.xml' 
    /// path='docs/doc[@name="T:PeterO.ExtendedDecimal"]'/>
  public sealed class ExtendedDecimal : IComparable<ExtendedDecimal>,
  IEquatable<ExtendedDecimal> {
    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.ExtendedDecimal.Exponent"]'/>
    public BigInteger Exponent { get {
 return new BigInteger(this.ed.Exponent);
} }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.ExtendedDecimal.UnsignedMantissa"]'/>
    public BigInteger UnsignedMantissa { get {
 return new BigInteger(this.ed.UnsignedMantissa);
} }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.ExtendedDecimal.Mantissa"]'/>
    public BigInteger Mantissa { get {
 return new BigInteger(this.ed.Mantissa);
} }

    #region Equals and GetHashCode implementation

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Equals(PeterO.ExtendedDecimal)"]'/>
    public bool Equals(ExtendedDecimal other) {
      return (other == null) ? (false) : (this.ed.Equals(other.ed));
 }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Equals(System.Object)"]'/>
    public override bool Equals(object obj) {
      var bi = obj as ExtendedDecimal;
      return (bi == null) ? (false) : (this.ed.Equals(bi.ed));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.GetHashCode"]'/>
    public override int GetHashCode() {
return this.ed.GetHashCode();
}
    #endregion

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Create(System.Int32,System.Int32)"]'/>
    public static ExtendedDecimal Create(int mantissaSmall, int exponentSmall) {
      return new ExtendedDecimal(EDecimal.Create(mantissaSmall, exponentSmall));
 }

    internal readonly EDecimal ed;
    internal ExtendedDecimal(EDecimal ed) {
      if ((ed) == null) {
  throw new ArgumentNullException("ed");
}
      this.ed = ed;
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Create(PeterO.BigInteger,PeterO.BigInteger)"]'/>
    public static ExtendedDecimal Create(
      BigInteger mantissa,
      BigInteger exponent) {
      if ((mantissa) == null) {
  throw new ArgumentNullException("mantissa");
}
      if ((exponent) == null) {
  throw new ArgumentNullException("exponent");
}
      return new ExtendedDecimal(EDecimal.Create(mantissa.ei, exponent.ei));
 }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.CreateNaN(PeterO.BigInteger)"]'/>
    public static ExtendedDecimal CreateNaN(BigInteger diag) {
      if ((diag) == null) {
  throw new ArgumentNullException("diag");
}
      return new ExtendedDecimal(EDecimal.CreateNaN(diag.ei));
 }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.CreateNaN(PeterO.BigInteger,System.Boolean,System.Boolean,PeterO.PrecisionContext)"]'/>
    public static ExtendedDecimal CreateNaN(BigInteger diag,
      bool signaling,
      bool negative,
      PrecisionContext ctx) {
  if ((diag) == null) {
  throw new ArgumentNullException("diag");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(EDecimal.CreateNaN(diag.ei, signaling, negative,
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromString(System.String)"]'/>
    public static ExtendedDecimal FromString(string str) {
return new ExtendedDecimal(EDecimal.FromString(str));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromString(System.String,PeterO.PrecisionContext)"]'/>
    public static ExtendedDecimal FromString(string str, PrecisionContext ctx) {
return new ExtendedDecimal(EDecimal.FromString(str, ctx == null ? null :
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromString(System.String,System.Int32,System.Int32)"]'/>
    public static ExtendedDecimal FromString(string str,
      int offset,
      int length) {
return new ExtendedDecimal(EDecimal.FromString(str, offset, length));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromString(System.String,System.Int32,System.Int32,PeterO.PrecisionContext)"]'/>
    public static ExtendedDecimal FromString(string str,
      int offset,
      int length,
      PrecisionContext ctx) {
return new ExtendedDecimal(EDecimal.FromString(str, offset, length,
  ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.CompareToBinary(PeterO.ExtendedFloat)"]'/>
    public int CompareToBinary(ExtendedFloat other) {
  return ((other) == null) ? (1) : (this.ed.CompareToBinary(other.ef));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToBigInteger"]'/>
    public BigInteger ToBigInteger() {
return new BigInteger(this.ed.ToBigInteger());
}

    internal static ERounding ToERounding(Rounding r) {
      if (r == Rounding.Ceiling) {
 return ERounding.Ceiling;
}
      if (r == Rounding.Floor) {
 return ERounding.Floor;
}
      if (r == Rounding.HalfDown) {
 return ERounding.HalfDown;
}
      if (r == Rounding.HalfEven) {
 return ERounding.HalfEven;
}
      if (r == Rounding.HalfUp) {
 return ERounding.HalfUp;
}
      if (r == Rounding.Up) {
 return ERounding.Up;
}
      if (r == Rounding.ZeroFiveUp) {
 return ERounding.ZeroFiveUp;
}
      if (r == Rounding.OddOrZeroFiveUp) {
 return ERounding.OddOrZeroFiveUp;
}
      return (r == Rounding.Unnecessary) ? (ERounding.None) : ((r ==
        Rounding.Odd) ? (ERounding.Odd) : (ERounding.Down));
    }

    internal static Rounding ToRounding(ERounding r) {
      if (r == ERounding.Ceiling) {
 return Rounding.Ceiling;
}
      if (r == ERounding.Floor) {
 return Rounding.Floor;
}
      if (r == ERounding.HalfDown) {
 return Rounding.HalfDown;
}
      if (r == ERounding.HalfEven) {
 return Rounding.HalfEven;
}
      if (r == ERounding.HalfUp) {
 return Rounding.HalfUp;
}
      if (r == ERounding.Up) {
 return Rounding.Up;
}
      if (r == ERounding.ZeroFiveUp) {
 return Rounding.ZeroFiveUp;
}
      if (r == ERounding.OddOrZeroFiveUp) {
 return Rounding.OddOrZeroFiveUp;
}
      return (r == ERounding.None) ? (Rounding.Unnecessary) : ((r ==
        ERounding.Odd) ? (Rounding.Odd) : (Rounding.Down));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToBigIntegerExact"]'/>
    public BigInteger ToBigIntegerExact() {
return new BigInteger(this.ed.ToBigIntegerExact());
}

    private static readonly BigInteger valueOneShift62 = BigInteger.One << 62;

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToExtendedFloat"]'/>
    public ExtendedFloat ToExtendedFloat() {
return new ExtendedFloat(this.ed.ToExtendedFloat());
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToSingle"]'/>
    public float ToSingle() {
return this.ed.ToSingle();
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToDouble"]'/>
    public double ToDouble() {
return this.ed.ToDouble();
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromSingle(System.Single)"]'/>
    public static ExtendedDecimal FromSingle(float flt) {
return new ExtendedDecimal(EDecimal.FromSingle(flt));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromBigInteger(PeterO.BigInteger)"]'/>
    public static ExtendedDecimal FromBigInteger(BigInteger bigint) {
  if ((bigint) == null) {
  throw new ArgumentNullException("bigint");
}
return new ExtendedDecimal(EDecimal.FromBigInteger(bigint.ei));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromInt64(System.Int64)"]'/>
    public static ExtendedDecimal FromInt64(long valueSmall) {
return new ExtendedDecimal(EDecimal.FromInt64(valueSmall));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromInt32(System.Int32)"]'/>
    public static ExtendedDecimal FromInt32(int valueSmaller) {
return new ExtendedDecimal(EDecimal.FromInt32(valueSmaller));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromDouble(System.Double)"]'/>
    public static ExtendedDecimal FromDouble(double dbl) {
return new ExtendedDecimal(EDecimal.FromDouble(dbl));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromExtendedFloat(PeterO.ExtendedFloat)"]'/>
    public static ExtendedDecimal FromExtendedFloat(ExtendedFloat bigfloat) {
  if ((bigfloat) == null) {
  throw new ArgumentNullException("bigfloat");
}
return new ExtendedDecimal(EDecimal.FromExtendedFloat(bigfloat.ef));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToString"]'/>
    public override string ToString() {
return this.ed.ToString();
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToEngineeringString"]'/>
    public string ToEngineeringString() {
return this.ed.ToEngineeringString();
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToPlainString"]'/>
    public string ToPlainString() {
return this.ed.ToPlainString();
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.One"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    public static readonly ExtendedDecimal One =
      ExtendedDecimal.Create(BigInteger.One, BigInteger.Zero);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.Zero"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    public static readonly ExtendedDecimal Zero =
      ExtendedDecimal.Create(BigInteger.Zero, BigInteger.Zero);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.NegativeZero"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    public static readonly ExtendedDecimal NegativeZero =
      new ExtendedDecimal(EDecimal.NegativeZero);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.Ten"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif

    public static readonly ExtendedDecimal Ten =
      ExtendedDecimal.Create((BigInteger)10, BigInteger.Zero);

    //----------------------------------------------------------------

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.NaN"]'/>
    public static readonly ExtendedDecimal NaN =
      new ExtendedDecimal(EDecimal.NaN);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.SignalingNaN"]'/>
    public static readonly ExtendedDecimal SignalingNaN =
      new ExtendedDecimal(EDecimal.SignalingNaN);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.PositiveInfinity"]'/>
    public static readonly ExtendedDecimal PositiveInfinity =
      new ExtendedDecimal(EDecimal.PositiveInfinity);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.NegativeInfinity"]'/>
    public static readonly ExtendedDecimal NegativeInfinity=
      new ExtendedDecimal(EDecimal.NegativeInfinity);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.IsNegativeInfinity"]'/>
    public bool IsNegativeInfinity() {
return this.ed.IsNegativeInfinity();
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.IsPositiveInfinity"]'/>
    public bool IsPositiveInfinity() {
return this.ed.IsPositiveInfinity();
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.IsNaN"]'/>
    public bool IsNaN() {
return this.ed.IsNaN();
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.IsInfinity"]'/>
    public bool IsInfinity() {
return this.ed.IsInfinity();
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.ExtendedDecimal.IsFinite"]'/>
    public  bool IsFinite {
get {
return this.ed.IsFinite;
} }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.ExtendedDecimal.IsNegative"]'/>
    public  bool IsNegative {
get {
return this.ed.IsNegative;
} }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.IsQuietNaN"]'/>
    public bool IsQuietNaN() {
return this.ed.IsQuietNaN();
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.IsSignalingNaN"]'/>
    public bool IsSignalingNaN() {
return this.ed.IsSignalingNaN();
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.ExtendedDecimal.Sign"]'/>
    public  int Sign {
get {
return this.ed.Sign;
} }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.ExtendedDecimal.IsZero"]'/>
    public  bool IsZero {
get {
return this.ed.IsZero;
} }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Abs"]'/>
    public ExtendedDecimal Abs() {
return new ExtendedDecimal(this.ed.Abs());
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Negate"]'/>
    public ExtendedDecimal Negate() {
return new ExtendedDecimal(this.ed.Negate());
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Divide(PeterO.ExtendedDecimal)"]'/>
    public ExtendedDecimal Divide(ExtendedDecimal divisor) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
return new ExtendedDecimal(this.ed.Divide(divisor.ed));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToSameExponent(PeterO.ExtendedDecimal,PeterO.Rounding)"]'/>
    public ExtendedDecimal DivideToSameExponent(ExtendedDecimal divisor,
      Rounding rounding) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
return new ExtendedDecimal(this.ed.DivideToSameExponent(divisor.ed,
  ExtendedDecimal.ToERounding(rounding)));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToIntegerNaturalScale(PeterO.ExtendedDecimal)"]'/>
    public ExtendedDecimal DivideToIntegerNaturalScale(ExtendedDecimal
                    divisor) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
return new ExtendedDecimal(this.ed.DivideToIntegerNaturalScale(divisor.ed));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Reduce(PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Reduce(PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.Reduce(ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RemainderNaturalScale(PeterO.ExtendedDecimal)"]'/>
    public ExtendedDecimal RemainderNaturalScale(ExtendedDecimal divisor) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
return new ExtendedDecimal(this.ed.RemainderNaturalScale(divisor.ed));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RemainderNaturalScale(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal RemainderNaturalScale(ExtendedDecimal divisor,
      PrecisionContext ctx) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.RemainderNaturalScale(divisor.ed,
  ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToExponent(PeterO.ExtendedDecimal,System.Int64,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal DivideToExponent(ExtendedDecimal divisor,
      long desiredExponentSmall,
      PrecisionContext ctx) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.DivideToExponent(divisor.ed,
  desiredExponentSmall, ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Divide(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Divide(ExtendedDecimal divisor,
      PrecisionContext ctx) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
return new ExtendedDecimal(this.ed.Divide(divisor.ed, ctx == null ? null :
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToExponent(PeterO.ExtendedDecimal,System.Int64,PeterO.Rounding)"]'/>
    public ExtendedDecimal DivideToExponent(ExtendedDecimal divisor,
      long desiredExponentSmall,
      Rounding rounding) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
return new ExtendedDecimal(this.ed.DivideToExponent(divisor.ed,
  desiredExponentSmall, ExtendedDecimal.ToERounding(rounding)));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToExponent(PeterO.ExtendedDecimal,PeterO.BigInteger,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal DivideToExponent(ExtendedDecimal divisor,
      BigInteger exponent,
      PrecisionContext ctx) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
  if ((exponent) == null) {
  throw new ArgumentNullException("exponent");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.DivideToExponent(divisor.ed, exponent.ei,
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToExponent(PeterO.ExtendedDecimal,PeterO.BigInteger,PeterO.Rounding)"]'/>
    public ExtendedDecimal DivideToExponent(ExtendedDecimal divisor,
      BigInteger desiredExponent,
      Rounding rounding) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
  if ((desiredExponent) == null) {
  throw new ArgumentNullException("desiredExponent");
}
return new ExtendedDecimal(this.ed.DivideToExponent(divisor.ed,
  desiredExponent.ei, ExtendedDecimal.ToERounding(rounding)));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Abs(PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Abs(PrecisionContext context) {
return new ExtendedDecimal(this.ed.Abs(context == null ? null : context.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Negate(PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Negate(PrecisionContext context) {
  if ((context) == null) {
  throw new ArgumentNullException("context");
}
return new ExtendedDecimal(this.ed.Negate(context == null ? null : context.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Add(PeterO.ExtendedDecimal)"]'/>
    public ExtendedDecimal Add(ExtendedDecimal otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedDecimal(this.ed.Add(otherValue.ed));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Subtract(PeterO.ExtendedDecimal)"]'/>
    public ExtendedDecimal Subtract(ExtendedDecimal otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedDecimal(this.ed.Subtract(otherValue.ed));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Subtract(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Subtract(ExtendedDecimal otherValue,
      PrecisionContext ctx) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.Subtract(otherValue.ed, ctx == null ? null:
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Multiply(PeterO.ExtendedDecimal)"]'/>
    public ExtendedDecimal Multiply(ExtendedDecimal otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedDecimal(this.ed.Multiply(otherValue.ed));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MultiplyAndAdd(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal)"]'/>
    public ExtendedDecimal MultiplyAndAdd(ExtendedDecimal multiplicand,
      ExtendedDecimal augend) {
  if ((multiplicand) == null) {
  throw new ArgumentNullException("multiplicand");
}
  if ((augend) == null) {
  throw new ArgumentNullException("augend");
}
return new ExtendedDecimal(this.ed.MultiplyAndAdd(multiplicand.ed, augend.ed));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToIntegerNaturalScale(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal DivideToIntegerNaturalScale(ExtendedDecimal divisor,
      PrecisionContext ctx) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.DivideToIntegerNaturalScale(divisor.ed,
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToIntegerZeroScale(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal DivideToIntegerZeroScale(ExtendedDecimal divisor,
      PrecisionContext ctx) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.DivideToIntegerZeroScale(divisor.ed,
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Remainder(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Remainder(ExtendedDecimal divisor,
      PrecisionContext ctx) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.Remainder(divisor.ed, ctx == null ? null :
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RemainderNear(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal RemainderNear(ExtendedDecimal divisor,
      PrecisionContext ctx) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.RemainderNear(divisor.ed, ctx == null ?
  null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.NextMinus(PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal NextMinus(PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.NextMinus(ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.NextPlus(PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal NextPlus(PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.NextPlus(ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.NextToward(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal NextToward(ExtendedDecimal otherValue,
      PrecisionContext ctx) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.NextToward(otherValue.ed, ctx == null ?
  null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Max(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public static ExtendedDecimal Max(ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
  if ((first) == null) {
  throw new ArgumentNullException("first");
}
  if ((second) == null) {
  throw new ArgumentNullException("second");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(EDecimal.Max(first.ed, second.ed, ctx == null ?
  null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Min(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public static ExtendedDecimal Min(ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
  if ((first) == null) {
  throw new ArgumentNullException("first");
}
  if ((second) == null) {
  throw new ArgumentNullException("second");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(EDecimal.Min(first.ed, second.ed, ctx == null ?
  null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MaxMagnitude(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public static ExtendedDecimal MaxMagnitude(ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
  if ((first) == null) {
  throw new ArgumentNullException("first");
}
  if ((second) == null) {
  throw new ArgumentNullException("second");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(EDecimal.MaxMagnitude(first.ed, second.ed,
  ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MinMagnitude(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public static ExtendedDecimal MinMagnitude(ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
  if ((first) == null) {
  throw new ArgumentNullException("first");
}
  if ((second) == null) {
  throw new ArgumentNullException("second");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(EDecimal.MinMagnitude(first.ed, second.ed,
  ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Max(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal)"]'/>
    public static ExtendedDecimal Max(ExtendedDecimal first,
      ExtendedDecimal second) {
  if ((first) == null) {
  throw new ArgumentNullException("first");
}
  if ((second) == null) {
  throw new ArgumentNullException("second");
}
return new ExtendedDecimal(EDecimal.Max(first.ed, second.ed));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Min(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal)"]'/>
    public static ExtendedDecimal Min(ExtendedDecimal first,
      ExtendedDecimal second) {
  if ((first) == null) {
  throw new ArgumentNullException("first");
}
  if ((second) == null) {
  throw new ArgumentNullException("second");
}
return new ExtendedDecimal(EDecimal.Min(first.ed, second.ed));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MaxMagnitude(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal)"]'/>
    public static ExtendedDecimal MaxMagnitude(ExtendedDecimal first,
      ExtendedDecimal second) {
  if ((first) == null) {
  throw new ArgumentNullException("first");
}
  if ((second) == null) {
  throw new ArgumentNullException("second");
}
return new ExtendedDecimal(EDecimal.MaxMagnitude(first.ed, second.ed));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MinMagnitude(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal)"]'/>
    public static ExtendedDecimal MinMagnitude(ExtendedDecimal first,
      ExtendedDecimal second) {
  if ((first) == null) {
  throw new ArgumentNullException("first");
}
  if ((second) == null) {
  throw new ArgumentNullException("second");
}
return new ExtendedDecimal(EDecimal.MinMagnitude(first.ed, second.ed));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.CompareTo(PeterO.ExtendedDecimal)"]'/>
    public int CompareTo(ExtendedDecimal other) {
  if ((other) == null) {
  throw new ArgumentNullException("other");
}
return this.ed.CompareTo(other.ed);
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.CompareToWithContext(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal CompareToWithContext(ExtendedDecimal other,
      PrecisionContext ctx) {
  if ((other) == null) {
  throw new ArgumentNullException("other");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.CompareToWithContext(other.ed, ctx == null?
  null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.CompareToSignal(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal CompareToSignal(ExtendedDecimal other,
      PrecisionContext ctx) {
  if ((other) == null) {
  throw new ArgumentNullException("other");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.CompareToSignal(other.ed, ctx == null ?
  null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Add(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Add(ExtendedDecimal otherValue,
      PrecisionContext ctx) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.Add(otherValue.ed, ctx == null ? null :
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Quantize(PeterO.BigInteger,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Quantize(BigInteger desiredExponent,
      PrecisionContext ctx) {
  if ((desiredExponent) == null) {
  throw new ArgumentNullException("desiredExponent");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.Quantize(desiredExponent.ei, ctx == null ?
  null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Quantize(System.Int32,PeterO.Rounding)"]'/>
    public ExtendedDecimal Quantize(int desiredExponentSmall,
      Rounding rounding) {
return new ExtendedDecimal(this.ed.Quantize(desiredExponentSmall,
  ExtendedDecimal.ToERounding(rounding)));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Quantize(System.Int32,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Quantize(int desiredExponentSmall,
      PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.Quantize(desiredExponentSmall, ctx == null?
  null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Quantize(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Quantize(ExtendedDecimal otherValue,
      PrecisionContext ctx) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedDecimal(this.ed.Quantize(otherValue.ed, ctx == null ? null:
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToIntegralExact(PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal RoundToIntegralExact(PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.RoundToIntegralExact(ctx == null ? null :
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToIntegralNoRoundedFlag(PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal RoundToIntegralNoRoundedFlag(PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.RoundToIntegralNoRoundedFlag(ctx == null ?
  null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToExponentExact(PeterO.BigInteger,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal RoundToExponentExact(BigInteger exponent,
      PrecisionContext ctx) {
  if ((exponent) == null) {
  throw new ArgumentNullException("exponent");
}
return new ExtendedDecimal(this.ed.RoundToExponentExact(exponent.ei,
  ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToExponent(PeterO.BigInteger,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal RoundToExponent(BigInteger exponent,
      PrecisionContext ctx) {
  if ((exponent) == null) {
  throw new ArgumentNullException("exponent");
}
return new ExtendedDecimal(this.ed.RoundToExponent(exponent.ei, ctx == null ?
  null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToExponentExact(System.Int32,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal RoundToExponentExact(int exponentSmall,
      PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.RoundToExponentExact(exponentSmall,
  ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToExponent(System.Int32,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal RoundToExponent(int exponentSmall,
      PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.RoundToExponent(exponentSmall, ctx == null?
  null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Multiply(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Multiply(ExtendedDecimal op, PrecisionContext ctx) {
  if ((op) == null) {
  throw new ArgumentNullException("op");
}
return new ExtendedDecimal(this.ed.Multiply(op.ed, ctx == null ? null :
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MultiplyAndAdd(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal MultiplyAndAdd(ExtendedDecimal op,
      ExtendedDecimal augend,
      PrecisionContext ctx) {
  if ((op) == null) {
  throw new ArgumentNullException("op");
}
  if ((augend) == null) {
  throw new ArgumentNullException("augend");
}
return new ExtendedDecimal(this.ed.MultiplyAndAdd(op.ed, augend.ed,
  ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MultiplyAndSubtract(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal MultiplyAndSubtract(ExtendedDecimal op,
      ExtendedDecimal subtrahend,
      PrecisionContext ctx) {
  if ((op) == null) {
  throw new ArgumentNullException("op");
}
  if ((subtrahend) == null) {
  throw new ArgumentNullException("subtrahend");
}
return new ExtendedDecimal(this.ed.MultiplyAndSubtract(op.ed, subtrahend.ed,
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToPrecision(PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal RoundToPrecision(PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.RoundToPrecision(ctx == null ? null :
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Plus(PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Plus(PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.Plus(ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToBinaryPrecision(PeterO.PrecisionContext)"]'/>
    [Obsolete(
      "Instead of this method use RoundToPrecision and pass a " + "precision context with the IsPrecisionInBits property set.")]
    public ExtendedDecimal RoundToBinaryPrecision(PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.RoundToBinaryPrecision(ctx == null ? null :
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.SquareRoot(PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal SquareRoot(PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.SquareRoot(ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Exp(PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Exp(PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.Exp(ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Log(PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Log(PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.Log(ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Log10(PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Log10(PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.Log10(ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Pow(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Pow(ExtendedDecimal exponent, PrecisionContext ctx) {
  if ((exponent) == null) {
  throw new ArgumentNullException("exponent");
}
return new ExtendedDecimal(this.ed.Pow(exponent.ed, ctx == null ? null :
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Pow(System.Int32,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal Pow(int exponentSmall, PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.Pow(exponentSmall, ctx == null ? null :
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Pow(System.Int32)"]'/>
    public ExtendedDecimal Pow(int exponentSmall) {
return new ExtendedDecimal(this.ed.Pow(exponentSmall));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.PI(PeterO.PrecisionContext)"]'/>
    public static ExtendedDecimal PI(PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(EDecimal.PI(ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointLeft(System.Int32)"]'/>
    public ExtendedDecimal MovePointLeft(int places) {
return new ExtendedDecimal(this.ed.MovePointLeft(places));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointLeft(System.Int32,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal MovePointLeft(int places, PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.MovePointLeft(places, ctx == null ? null :
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointLeft(PeterO.BigInteger)"]'/>
    public ExtendedDecimal MovePointLeft(BigInteger bigPlaces) {
  if ((bigPlaces) == null) {
  throw new ArgumentNullException("bigPlaces");
}
return new ExtendedDecimal(this.ed.MovePointLeft(bigPlaces.ei));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointLeft(PeterO.BigInteger,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal MovePointLeft(BigInteger bigPlaces,
PrecisionContext ctx) {
  if ((bigPlaces) == null) {
  throw new ArgumentNullException("bigPlaces");
}
return new ExtendedDecimal(this.ed.MovePointLeft(bigPlaces.ei,
  ctx == null ? null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointRight(System.Int32)"]'/>
    public ExtendedDecimal MovePointRight(int places) {
return new ExtendedDecimal(this.ed.MovePointRight(places));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointRight(System.Int32,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal MovePointRight(int places, PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.MovePointRight(places, ctx == null ? null :
  ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointRight(PeterO.BigInteger)"]'/>
    public ExtendedDecimal MovePointRight(BigInteger bigPlaces) {
  if ((bigPlaces) == null) {
  throw new ArgumentNullException("bigPlaces");
}
return new ExtendedDecimal(this.ed.MovePointRight(bigPlaces.ei));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointRight(PeterO.BigInteger,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal MovePointRight(BigInteger bigPlaces,
PrecisionContext ctx) {
  if ((bigPlaces) == null) {
  throw new ArgumentNullException("bigPlaces");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.MovePointRight(bigPlaces.ei, ctx == null ?
  null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ScaleByPowerOfTen(System.Int32)"]'/>
    public ExtendedDecimal ScaleByPowerOfTen(int places) {
return new ExtendedDecimal(this.ed.ScaleByPowerOfTen(places));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ScaleByPowerOfTen(System.Int32,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal ScaleByPowerOfTen(int places, PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.ScaleByPowerOfTen(places, ctx == null ?
  null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ScaleByPowerOfTen(PeterO.BigInteger)"]'/>
    public ExtendedDecimal ScaleByPowerOfTen(BigInteger bigPlaces) {
  if ((bigPlaces) == null) {
  throw new ArgumentNullException("bigPlaces");
}
return new ExtendedDecimal(this.ed.ScaleByPowerOfTen(bigPlaces.ei));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ScaleByPowerOfTen(PeterO.BigInteger,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal ScaleByPowerOfTen(BigInteger bigPlaces,
PrecisionContext ctx) {
  if ((bigPlaces) == null) {
  throw new ArgumentNullException("bigPlaces");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.ScaleByPowerOfTen(bigPlaces.ei, ctx == null?
  null : ctx.ec));
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Precision"]'/>
    public BigInteger Precision() {
return new BigInteger(this.ed.Precision());
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Ulp"]'/>
    public ExtendedDecimal Ulp() {
return new ExtendedDecimal(this.ed.Ulp());
}

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideAndRemainderNaturalScale(PeterO.ExtendedDecimal)"]'/>
    public ExtendedDecimal[] DivideAndRemainderNaturalScale(ExtendedDecimal
      divisor) {
      EDecimal[] edec = this.ed.DivideAndRemainderNaturalScale(divisor == null ? null : divisor.ed);
      return new ExtendedDecimal[] {
        new ExtendedDecimal(edec[0]),new ExtendedDecimal(edec[1])
      };
 }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideAndRemainderNaturalScale(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]'/>
    public ExtendedDecimal[] DivideAndRemainderNaturalScale(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      EDecimal[] edec = this.ed.DivideAndRemainderNaturalScale(divisor == null ? null : divisor.ed,
        ctx==null ? null : ctx.ec);
      return new ExtendedDecimal[] {
        new ExtendedDecimal(edec[0]),new ExtendedDecimal(edec[1])
      };
    }
  }
}
