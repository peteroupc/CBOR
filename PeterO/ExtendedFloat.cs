/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using PeterO.Numbers;
namespace PeterO {
    /// <include file='docs.xml' 
    /// path='docs/doc[@name="T:PeterO.ExtendedFloat"]'/>
  public sealed class ExtendedFloat : IComparable<ExtendedFloat>,
  IEquatable<ExtendedFloat> {
    internal readonly EFloat ef;
    internal ExtendedFloat(EFloat ef) {
      this.ef = ef;
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.Exponent"]'/>
    public BigInteger Exponent {
      get {
        return new BigInteger(this.ef.Exponent);
      }
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.UnsignedMantissa"]'/>
    public BigInteger UnsignedMantissa {
      get {
        return new BigInteger(this.ef.UnsignedMantissa);
      }
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.Mantissa"]'/>
    public BigInteger Mantissa {
      get {
        return new BigInteger(this.ef.Mantissa);
      }
    }

    #region Equals and GetHashCode implementation
    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.EqualsInternal(PeterO.ExtendedFloat)"]'/>
    public bool EqualsInternal(ExtendedFloat otherValue) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return this.ef.EqualsInternal(otherValue.ef);
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Equals(PeterO.ExtendedFloat)"]'/>
    public bool Equals(ExtendedFloat other) {
      if ((other) == null) {
        throw new ArgumentNullException("other");
      }
      return this.ef.Equals(other.ef);
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Equals(System.Object)"]'/>
    public override bool Equals(object obj) {
      var bi = obj as ExtendedFloat;
      return (bi == null) ? (false) : (this.ef.Equals(bi.ef));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.GetHashCode"]'/>
    public override int GetHashCode() {
      return this.ef.GetHashCode();
    }
    #endregion

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.CreateNaN(PeterO.BigInteger)"]'/>
    public static ExtendedFloat CreateNaN(BigInteger diag) {
      if ((diag) == null) {
        throw new ArgumentNullException("diag");
      }
      return new ExtendedFloat(EFloat.CreateNaN(diag.ei));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.CreateNaN(PeterO.BigInteger,System.Boolean,System.Boolean,PeterO.PrecisionContext)"]'/>
    public static ExtendedFloat CreateNaN(BigInteger diag,
      bool signaling,
      bool negative,
      PrecisionContext ctx) {
      if ((diag) == null) {
        throw new ArgumentNullException("diag");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(EFloat.CreateNaN(diag.ei, signaling, negative,
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Create(System.Int32,System.Int32)"]'/>
    public static ExtendedFloat Create(int mantissaSmall, int exponentSmall) {
      return new ExtendedFloat(EFloat.Create(mantissaSmall, exponentSmall));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Create(PeterO.BigInteger,PeterO.BigInteger)"]'/>
    public static ExtendedFloat Create(BigInteger mantissa,
      BigInteger exponent) {
      if ((mantissa) == null) {
        throw new ArgumentNullException("mantissa");
      }
      if ((exponent) == null) {
        throw new ArgumentNullException("exponent");
      }
      return new ExtendedFloat(EFloat.Create(mantissa.ei, exponent.ei));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromString(System.String,System.Int32,System.Int32,PeterO.PrecisionContext)"]'/>
    public static ExtendedFloat FromString(string str,
      int offset,
      int length,
      PrecisionContext ctx) {
      return new ExtendedFloat(EFloat.FromString(str, offset, length,
        ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromString(System.String)"]'/>
    public static ExtendedFloat FromString(string str) {
      return new ExtendedFloat(EFloat.FromString(str));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromString(System.String,PeterO.PrecisionContext)"]'/>
    public static ExtendedFloat FromString(string str, PrecisionContext ctx) {
   return new ExtendedFloat(EFloat.FromString(str, ctx == null ? null :
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromString(System.String,System.Int32,System.Int32)"]'/>
    public static ExtendedFloat FromString(string str, int offset, int length) {
      return new ExtendedFloat(EFloat.FromString(str, offset, length));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToBigInteger"]'/>
    public BigInteger ToBigInteger() {
      return new BigInteger(this.ef.ToBigInteger());
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToBigIntegerExact"]'/>
    public BigInteger ToBigIntegerExact() {
      return new BigInteger(this.ef.ToBigIntegerExact());
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToSingle"]'/>
    public float ToSingle() {
      return this.ef.ToSingle();
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToDouble"]'/>
    public double ToDouble() {
      return this.ef.ToDouble();
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromSingle(System.Single)"]'/>
    public static ExtendedFloat FromSingle(float flt) {
      return new ExtendedFloat(EFloat.FromSingle(flt));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromBigInteger(PeterO.BigInteger)"]'/>
    public static ExtendedFloat FromBigInteger(BigInteger bigint) {
      if ((bigint) == null) {
        throw new ArgumentNullException("bigint");
      }
      return new ExtendedFloat(EFloat.FromBigInteger(bigint.ei));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromInt64(System.Int64)"]'/>
    public static ExtendedFloat FromInt64(long valueSmall) {
      return new ExtendedFloat(EFloat.FromInt64(valueSmall));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromInt32(System.Int32)"]'/>
    public static ExtendedFloat FromInt32(int valueSmaller) {
      return new ExtendedFloat(EFloat.FromInt32(valueSmaller));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromDouble(System.Double)"]'/>
    public static ExtendedFloat FromDouble(double dbl) {
      return new ExtendedFloat(EFloat.FromDouble(dbl));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToExtendedDecimal"]'/>
    public ExtendedDecimal ToExtendedDecimal() {
      return new ExtendedDecimal(this.ef.ToExtendedDecimal());
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToString"]'/>
    public override string ToString() {
      return this.ef.ToString();
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToEngineeringString"]'/>
    public string ToEngineeringString() {
      return this.ef.ToEngineeringString();
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToPlainString"]'/>
    public string ToPlainString() {
      return this.ef.ToPlainString();
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.One"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif
    public static readonly ExtendedFloat One =
      new ExtendedFloat(EFloat.One);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.Zero"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif
    public static readonly ExtendedFloat Zero =
      new ExtendedFloat(EFloat.Zero);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.NegativeZero"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif
    public static readonly ExtendedFloat NegativeZero =
      new ExtendedFloat(EFloat.NegativeZero);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.Ten"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif

    public static readonly ExtendedFloat Ten =
      new ExtendedFloat(EFloat.Ten);

    //----------------------------------------------------------------

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.NaN"]'/>
    public static readonly ExtendedFloat NaN =
      new ExtendedFloat(EFloat.NaN);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.SignalingNaN"]'/>
    public static readonly ExtendedFloat SignalingNaN =
      new ExtendedFloat(EFloat.SignalingNaN);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.PositiveInfinity"]'/>
    public static readonly ExtendedFloat PositiveInfinity =
      new ExtendedFloat(EFloat.PositiveInfinity);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.NegativeInfinity"]'/>
    public static readonly ExtendedFloat NegativeInfinity =
      new ExtendedFloat(EFloat.NegativeInfinity);

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsNegativeInfinity"]'/>
    public bool IsNegativeInfinity() {
      return this.ef.IsNegativeInfinity();
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsPositiveInfinity"]'/>
    public bool IsPositiveInfinity() {
      return this.ef.IsPositiveInfinity();
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsNaN"]'/>
    public bool IsNaN() {
      return this.ef.IsNaN();
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsInfinity"]'/>
    public bool IsInfinity() {
      return this.ef.IsInfinity();
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.IsFinite"]'/>
    public bool IsFinite {
      get {
        return this.ef.IsFinite;
      }
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.IsNegative"]'/>
    public bool IsNegative {
      get {
        return this.ef.IsNegative;
      }
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsQuietNaN"]'/>
    public bool IsQuietNaN() {
      return this.ef.IsQuietNaN();
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsSignalingNaN"]'/>
    public bool IsSignalingNaN() {
      return this.ef.IsSignalingNaN();
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.Sign"]'/>
    public int Sign {
      get {
        return this.ef.Sign;
      }
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.IsZero"]'/>
    public bool IsZero {
      get {
        return this.ef.IsZero;
      }
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Abs"]'/>
    public ExtendedFloat Abs() {
      return new ExtendedFloat(this.ef.Abs());
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Negate"]'/>
    public ExtendedFloat Negate() {
      return new ExtendedFloat(this.ef.Negate());
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Divide(PeterO.ExtendedFloat)"]'/>
    public ExtendedFloat Divide(ExtendedFloat divisor) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.Divide(divisor.ef));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToSameExponent(PeterO.ExtendedFloat,PeterO.Rounding)"]'/>
    public ExtendedFloat DivideToSameExponent(ExtendedFloat divisor,
      Rounding rounding) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.DivideToSameExponent(divisor.ef,
        ExtendedDecimal.ToERounding(rounding)));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToIntegerNaturalScale(PeterO.ExtendedFloat)"]'/>
    public ExtendedFloat DivideToIntegerNaturalScale(ExtendedFloat divisor) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.DivideToIntegerNaturalScale(divisor.ef));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Reduce(PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Reduce(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.Reduce(ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RemainderNaturalScale(PeterO.ExtendedFloat)"]'/>
    public ExtendedFloat RemainderNaturalScale(ExtendedFloat divisor) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.RemainderNaturalScale(divisor.ef));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RemainderNaturalScale(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat RemainderNaturalScale(ExtendedFloat divisor,
      PrecisionContext ctx) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
   return new ExtendedFloat(this.ef.RemainderNaturalScale(divisor.ef,
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToExponent(PeterO.ExtendedFloat,System.Int64,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat DivideToExponent(ExtendedFloat divisor,
      long desiredExponentSmall,
      PrecisionContext ctx) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.DivideToExponent(divisor.ef,
        desiredExponentSmall, ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Divide(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Divide(ExtendedFloat divisor,
      PrecisionContext ctx) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.Divide(divisor.ef, ctx == null ? null:
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToExponent(PeterO.ExtendedFloat,System.Int64,PeterO.Rounding)"]'/>
    public ExtendedFloat DivideToExponent(ExtendedFloat divisor,
      long desiredExponentSmall,
      Rounding rounding) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.DivideToExponent(divisor.ef,
        desiredExponentSmall, ExtendedDecimal.ToERounding(rounding)));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToExponent(PeterO.ExtendedFloat,PeterO.BigInteger,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat DivideToExponent(ExtendedFloat divisor,
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
      return new ExtendedFloat(this.ef.DivideToExponent(divisor.ef, exponent.ei,
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToExponent(PeterO.ExtendedFloat,PeterO.BigInteger,PeterO.Rounding)"]'/>
    public ExtendedFloat DivideToExponent(ExtendedFloat divisor,
      BigInteger desiredExponent,
      Rounding rounding) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      if ((desiredExponent) == null) {
        throw new ArgumentNullException("desiredExponent");
      }
      return new ExtendedFloat(this.ef.DivideToExponent(divisor.ef,
        desiredExponent.ei, ExtendedDecimal.ToERounding(rounding)));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Abs(PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Abs(PrecisionContext context) {
    return new ExtendedFloat(this.ef.Abs(context == null ? null :
        context.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Negate(PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Negate(PrecisionContext context) {
      if ((context) == null) {
        throw new ArgumentNullException("context");
      }
   return new ExtendedFloat(this.ef.Negate(context == null ? null :
        context.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Add(PeterO.ExtendedFloat)"]'/>
    public ExtendedFloat Add(ExtendedFloat otherValue) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedFloat(this.ef.Add(otherValue.ef));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Subtract(PeterO.ExtendedFloat)"]'/>
    public ExtendedFloat Subtract(ExtendedFloat otherValue) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedFloat(this.ef.Subtract(otherValue.ef));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Subtract(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Subtract(ExtendedFloat otherValue,
      PrecisionContext ctx) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.Subtract(otherValue.ef, ctx == null ?
        null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Multiply(PeterO.ExtendedFloat)"]'/>
    public ExtendedFloat Multiply(ExtendedFloat otherValue) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedFloat(this.ef.Multiply(otherValue.ef));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MultiplyAndAdd(PeterO.ExtendedFloat,PeterO.ExtendedFloat)"]'/>
    public ExtendedFloat MultiplyAndAdd(ExtendedFloat multiplicand,
      ExtendedFloat augend) {
      if ((multiplicand) == null) {
        throw new ArgumentNullException("multiplicand");
      }
      if ((augend) == null) {
        throw new ArgumentNullException("augend");
      }
  return new ExtendedFloat(this.ef.MultiplyAndAdd(multiplicand.ef,
        augend.ef));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToIntegerNaturalScale(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat DivideToIntegerNaturalScale(ExtendedFloat divisor,
      PrecisionContext ctx) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.DivideToIntegerNaturalScale(divisor.ef,
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToIntegerZeroScale(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat DivideToIntegerZeroScale(ExtendedFloat divisor,
      PrecisionContext ctx) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
return new ExtendedFloat(this.ef.DivideToIntegerZeroScale(divisor.ef,
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Remainder(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Remainder(ExtendedFloat divisor,
      PrecisionContext ctx) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.Remainder(divisor.ef, ctx == null ?
        null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RemainderNear(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat RemainderNear(ExtendedFloat divisor,
      PrecisionContext ctx) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.RemainderNear(divisor.ef, ctx == null ?
        null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.NextMinus(PeterO.PrecisionContext)"]'/>
    public ExtendedFloat NextMinus(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.NextMinus(ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.NextPlus(PeterO.PrecisionContext)"]'/>
    public ExtendedFloat NextPlus(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.NextPlus(ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.NextToward(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat NextToward(ExtendedFloat otherValue,
      PrecisionContext ctx) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.NextToward(otherValue.ef, ctx == null ?
        null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Max(PeterO.ExtendedFloat,PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public static ExtendedFloat Max(ExtendedFloat first,
      ExtendedFloat second,
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
      return new ExtendedFloat(EFloat.Max(first.ef, second.ef, ctx == null ?
        null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Min(PeterO.ExtendedFloat,PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public static ExtendedFloat Min(ExtendedFloat first,
      ExtendedFloat second,
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
      return new ExtendedFloat(EFloat.Min(first.ef, second.ef, ctx == null ?
        null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MaxMagnitude(PeterO.ExtendedFloat,PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public static ExtendedFloat MaxMagnitude(ExtendedFloat first,
      ExtendedFloat second,
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
    return new ExtendedFloat(EFloat.MaxMagnitude(first.ef, second.ef,
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MinMagnitude(PeterO.ExtendedFloat,PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public static ExtendedFloat MinMagnitude(ExtendedFloat first,
      ExtendedFloat second,
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
    return new ExtendedFloat(EFloat.MinMagnitude(first.ef, second.ef,
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Max(PeterO.ExtendedFloat,PeterO.ExtendedFloat)"]'/>
    public static ExtendedFloat Max(ExtendedFloat first,
      ExtendedFloat second) {
      if ((first) == null) {
        throw new ArgumentNullException("first");
      }
      if ((second) == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedFloat(EFloat.Max(first.ef, second.ef));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Min(PeterO.ExtendedFloat,PeterO.ExtendedFloat)"]'/>
    public static ExtendedFloat Min(ExtendedFloat first,
      ExtendedFloat second) {
      if ((first) == null) {
        throw new ArgumentNullException("first");
      }
      if ((second) == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedFloat(EFloat.Min(first.ef, second.ef));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MaxMagnitude(PeterO.ExtendedFloat,PeterO.ExtendedFloat)"]'/>
    public static ExtendedFloat MaxMagnitude(ExtendedFloat first,
      ExtendedFloat second) {
      if ((first) == null) {
        throw new ArgumentNullException("first");
      }
      if ((second) == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedFloat(EFloat.MaxMagnitude(first.ef, second.ef));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MinMagnitude(PeterO.ExtendedFloat,PeterO.ExtendedFloat)"]'/>
    public static ExtendedFloat MinMagnitude(ExtendedFloat first,
      ExtendedFloat second) {
      if ((first) == null) {
        throw new ArgumentNullException("first");
      }
      if ((second) == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedFloat(EFloat.MinMagnitude(first.ef, second.ef));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.CompareTo(PeterO.ExtendedFloat)"]'/>
    public int CompareTo(ExtendedFloat other) {
      if ((other) == null) {
        throw new ArgumentNullException("other");
      }
      return this.ef.CompareTo(other.ef);
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.CompareToWithContext(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat CompareToWithContext(ExtendedFloat other,
      PrecisionContext ctx) {
      if ((other) == null) {
        throw new ArgumentNullException("other");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.CompareToWithContext(other.ef,
        ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.CompareToSignal(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat CompareToSignal(ExtendedFloat other,
      PrecisionContext ctx) {
      if ((other) == null) {
        throw new ArgumentNullException("other");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.CompareToSignal(other.ef, ctx == null ?
        null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Add(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Add(ExtendedFloat otherValue,
      PrecisionContext ctx) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.Add(otherValue.ef, ctx == null ? null :
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Quantize(PeterO.BigInteger,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Quantize(BigInteger desiredExponent,
      PrecisionContext ctx) {
      if ((desiredExponent) == null) {
        throw new ArgumentNullException("desiredExponent");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.Quantize(desiredExponent.ei,
        ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Quantize(System.Int32,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Quantize(int desiredExponentSmall,
      PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.Quantize(desiredExponentSmall,
        ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Quantize(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Quantize(ExtendedFloat otherValue,
      PrecisionContext ctx) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedFloat(this.ef.Quantize(otherValue.ef, ctx == null ?
        null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToIntegralExact(PeterO.PrecisionContext)"]'/>
    public ExtendedFloat RoundToIntegralExact(PrecisionContext ctx) {
      return new ExtendedFloat(this.ef.RoundToIntegralExact(ctx == null ? null:
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToIntegralNoRoundedFlag(PeterO.PrecisionContext)"]'/>
    public ExtendedFloat RoundToIntegralNoRoundedFlag(PrecisionContext ctx) {
      return new
        ExtendedFloat(this.ef.RoundToIntegralNoRoundedFlag(ctx == null ? null:
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToExponentExact(PeterO.BigInteger,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat RoundToExponentExact(BigInteger exponent,
      PrecisionContext ctx) {
      if ((exponent) == null) {
        throw new ArgumentNullException("exponent");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
   return new ExtendedFloat(this.ef.RoundToExponentExact(exponent.ei,
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToExponent(PeterO.BigInteger,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat RoundToExponent(BigInteger exponent,
      PrecisionContext ctx) {
      if ((exponent) == null) {
        throw new ArgumentNullException("exponent");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.RoundToExponent(exponent.ei,
        ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToExponentExact(System.Int32,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat RoundToExponentExact(int exponentSmall,
      PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
 return new ExtendedFloat(this.ef.RoundToExponentExact(exponentSmall,
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToExponent(System.Int32,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat RoundToExponent(int exponentSmall,
      PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.RoundToExponent(exponentSmall,
        ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Multiply(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Multiply(ExtendedFloat op,
      PrecisionContext ctx) {
      if ((op) == null) {
        throw new ArgumentNullException("op");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
  return new ExtendedFloat(this.ef.Multiply(op.ef, ctx == null ? null :
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MultiplyAndAdd(PeterO.ExtendedFloat,PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat MultiplyAndAdd(ExtendedFloat op,
      ExtendedFloat augend,
      PrecisionContext ctx) {
      if ((op) == null) {
        throw new ArgumentNullException("op");
      }
      if ((augend) == null) {
        throw new ArgumentNullException("augend");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
    return new ExtendedFloat(this.ef.MultiplyAndAdd(op.ef, augend.ef,
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MultiplyAndSubtract(PeterO.ExtendedFloat,PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat MultiplyAndSubtract(ExtendedFloat op,
      ExtendedFloat subtrahend,
      PrecisionContext ctx) {
      if ((op) == null) {
        throw new ArgumentNullException("op");
      }
      if ((subtrahend) == null) {
        throw new ArgumentNullException("subtrahend");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.MultiplyAndSubtract(op.ef, subtrahend.ef,
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToPrecision(PeterO.PrecisionContext)"]'/>
    public ExtendedFloat RoundToPrecision(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
 return new ExtendedFloat(this.ef.RoundToPrecision(ctx == null ? null :
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Plus(PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Plus(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.Plus(ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToBinaryPrecision(PeterO.PrecisionContext)"]'/>
    [Obsolete(
      "Instead of this method use RoundToPrecision and pass a precision " + "context with the IsPrecisionInBits property set.")]
    public ExtendedFloat RoundToBinaryPrecision(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.RoundToBinaryPrecision(ctx == null ?
        null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.SquareRoot(PeterO.PrecisionContext)"]'/>
    public ExtendedFloat SquareRoot(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.SquareRoot(ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Exp(PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Exp(PrecisionContext ctx) {
      return new ExtendedFloat(this.ef.Exp(ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Log(PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Log(PrecisionContext ctx) {
      return new ExtendedFloat(this.ef.Log(ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Log10(PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Log10(PrecisionContext ctx) {
      return new ExtendedFloat(this.ef.Log10(ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Pow(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Pow(ExtendedFloat exponent, PrecisionContext ctx) {
      if ((exponent) == null) {
        throw new ArgumentNullException("exponent");
      }
 return new ExtendedFloat(this.ef.Pow(exponent.ef, ctx == null ? null :
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Pow(System.Int32,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat Pow(int exponentSmall, PrecisionContext ctx) {
      return new ExtendedFloat(this.ef.Pow(exponentSmall, ctx == null ? null :
        ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Pow(System.Int32)"]'/>
    public ExtendedFloat Pow(int exponentSmall) {
      return new ExtendedFloat(this.ef.Pow(exponentSmall));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.PI(PeterO.PrecisionContext)"]'/>
    public static ExtendedFloat PI(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(EFloat.PI(ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointLeft(System.Int32)"]'/>
    public ExtendedFloat MovePointLeft(int places) {
      return new ExtendedFloat(this.ef.MovePointLeft(places));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointLeft(System.Int32,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat MovePointLeft(int places, PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.MovePointLeft(places, ctx == null ?
        null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointLeft(PeterO.BigInteger)"]'/>
    public ExtendedFloat MovePointLeft(BigInteger bigPlaces) {
      if ((bigPlaces) == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedFloat(this.ef.MovePointLeft(bigPlaces.ei));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointLeft(PeterO.BigInteger,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat MovePointLeft(BigInteger bigPlaces,
PrecisionContext ctx) {
      if ((bigPlaces) == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.MovePointLeft(bigPlaces.ei, ctx == null?
        null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointRight(System.Int32)"]'/>
    public ExtendedFloat MovePointRight(int places) {
      return new ExtendedFloat(this.ef.MovePointRight(places));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointRight(System.Int32,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat MovePointRight(int places, PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.MovePointRight(places, ctx == null ?
        null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointRight(PeterO.BigInteger)"]'/>
    public ExtendedFloat MovePointRight(BigInteger bigPlaces) {
      if ((bigPlaces) == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedFloat(this.ef.MovePointRight(bigPlaces.ei));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointRight(PeterO.BigInteger,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat MovePointRight(BigInteger bigPlaces,
PrecisionContext ctx) {
      if ((bigPlaces) == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.MovePointRight(bigPlaces.ei,
        ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ScaleByPowerOfTwo(System.Int32)"]'/>
    public ExtendedFloat ScaleByPowerOfTwo(int places) {
      return new ExtendedFloat(this.ef.ScaleByPowerOfTwo(places));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ScaleByPowerOfTwo(System.Int32,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat ScaleByPowerOfTwo(int places, PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.ScaleByPowerOfTwo(places, ctx == null ?
        null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ScaleByPowerOfTwo(PeterO.BigInteger)"]'/>
    public ExtendedFloat ScaleByPowerOfTwo(BigInteger bigPlaces) {
      if ((bigPlaces) == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedFloat(this.ef.ScaleByPowerOfTwo(bigPlaces.ei));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ScaleByPowerOfTwo(PeterO.BigInteger,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat ScaleByPowerOfTwo(BigInteger bigPlaces,
PrecisionContext ctx) {
      if ((bigPlaces) == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.ScaleByPowerOfTwo(bigPlaces.ei,
        ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Precision"]'/>
    public BigInteger Precision() {
      return new BigInteger(this.ef.Precision());
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Ulp"]'/>
    public ExtendedFloat Ulp() {
      return new ExtendedFloat(this.ef.Ulp());
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideAndRemainderNaturalScale(PeterO.ExtendedFloat)"]'/>
 public ExtendedFloat[] DivideAndRemainderNaturalScale(ExtendedFloat
      divisor) {
      EFloat[] edec = this.ef.DivideAndRemainderNaturalScale(divisor == null ? null : divisor.ef);
      return new ExtendedFloat[] {
        new ExtendedFloat(edec[0]),new ExtendedFloat(edec[1])
      };
    }

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideAndRemainderNaturalScale(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]'/>
    public ExtendedFloat[] DivideAndRemainderNaturalScale(
      ExtendedFloat divisor,
      PrecisionContext ctx) {
      EFloat[] edec = this.ef.DivideAndRemainderNaturalScale(divisor == null ? null : divisor.ef,
        ctx == null ? null : ctx.ec);
      return new ExtendedFloat[] {
        new ExtendedFloat(edec[0]),new ExtendedFloat(edec[1])
      };
    }
  }
}
