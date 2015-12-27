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
    /// path='docs/doc[@name="T:PeterO.Numbers.EFloat"]'/>
  public sealed class ExtendedFloat : IComparable<ExtendedFloat>,
  IEquatable<ExtendedFloat> {
    internal readonly EFloat ef;
    internal ExtendedFloat(EFloat ef) {
      this.ef = ef;
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EFloat.Exponent"]'/>
    public BigInteger Exponent {
      get {
        return new BigInteger(this.ef.Exponent);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EFloat.UnsignedMantissa"]'/>
    public BigInteger UnsignedMantissa {
      get {
        return new BigInteger(this.ef.UnsignedMantissa);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EFloat.Mantissa"]'/>
    public BigInteger Mantissa {
      get {
        return new BigInteger(this.ef.Mantissa);
      }
    }

    #region Equals and GetHashCode implementation
    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.EqualsInternal(PeterO.Numbers.EFloat)"]'/>
    public bool EqualsInternal(ExtendedFloat otherValue) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return this.ef.EqualsInternal(otherValue.ef);
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Equals(PeterO.Numbers.EFloat)"]'/>
    public bool Equals(ExtendedFloat other) {
      if ((other) == null) {
        throw new ArgumentNullException("other");
      }
      return this.ef.Equals(other.ef);
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Equals(System.Object)"]'/>
    public override bool Equals(object obj) {
      var bi = obj as ExtendedFloat;
      return (bi == null) ? (false) : (this.ef.Equals(bi.ef));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.GetHashCode"]'/>
    public override int GetHashCode() {
      return this.ef.GetHashCode();
    }
    #endregion

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.CreateNaN(PeterO.Numbers.EInteger)"]'/>
    public static ExtendedFloat CreateNaN(BigInteger diag) {
      if ((diag) == null) {
        throw new ArgumentNullException("diag");
      }
      return new ExtendedFloat(EFloat.CreateNaN(diag.ei));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.CreateNaN(PeterO.Numbers.EInteger,System.Boolean,System.Boolean,PeterO.Numbers.EContext)"]'/>
    public static ExtendedFloat CreateNaN(BigInteger diag,
      bool signaling,
      bool negative,
      PrecisionContext ctx) {
      if ((diag) == null) {
        throw new ArgumentNullException("diag");
      }

      return new ExtendedFloat(EFloat.CreateNaN(diag.ei, signaling,
        negative, ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Create(System.Int32,System.Int32)"]'/>
    public static ExtendedFloat Create(int mantissaSmall, int exponentSmall) {
      return new ExtendedFloat(EFloat.Create(mantissaSmall, exponentSmall));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Create(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromString(System.String,System.Int32,System.Int32,PeterO.Numbers.EContext)"]'/>
    public static ExtendedFloat FromString(string str,
      int offset,
      int length,
      PrecisionContext ctx) {
try {
      return new ExtendedFloat(EFloat.FromString(str, offset, length,
        ctx == null ? null : ctx.ec));
    } catch (ETrapException ex) {
 throw TrapException.Create(ex);
}
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromString(System.String)"]'/>
    public static ExtendedFloat FromString(string str) {
      return new ExtendedFloat(EFloat.FromString(str));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromString(System.String,PeterO.Numbers.EContext)"]'/>
    public static ExtendedFloat FromString(string str, PrecisionContext ctx) {
try {
      return new ExtendedFloat(EFloat.FromString(str, ctx == null ? null :
           ctx.ec));
    } catch (ETrapException ex) {
 throw TrapException.Create(ex);
}
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromString(System.String,System.Int32,System.Int32)"]'/>
    public static ExtendedFloat FromString(string str, int offset, int length) {
      return new ExtendedFloat(EFloat.FromString(str, offset, length));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ToBigInteger"]'/>
    public BigInteger ToBigInteger() {
      return new BigInteger(this.ef.ToBigInteger());
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ToBigIntegerExact"]'/>
    public BigInteger ToBigIntegerExact() {
      return new BigInteger(this.ef.ToBigIntegerExact());
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ToSingle"]'/>
    public float ToSingle() {
      return this.ef.ToSingle();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ToDouble"]'/>
    public double ToDouble() {
      return this.ef.ToDouble();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromSingle(System.Single)"]'/>
    public static ExtendedFloat FromSingle(float flt) {
      return new ExtendedFloat(EFloat.FromSingle(flt));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromBigInteger(PeterO.Numbers.EInteger)"]'/>
    public static ExtendedFloat FromBigInteger(BigInteger bigint) {
      if ((bigint) == null) {
        throw new ArgumentNullException("bigint");
      }
      return new ExtendedFloat(EFloat.FromBigInteger(bigint.ei));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromInt64(System.Int64)"]'/>
    public static ExtendedFloat FromInt64(long valueSmall) {
      return new ExtendedFloat(EFloat.FromInt64(valueSmall));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromInt32(System.Int32)"]'/>
    public static ExtendedFloat FromInt32(int valueSmaller) {
      return new ExtendedFloat(EFloat.FromInt32(valueSmaller));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromDouble(System.Double)"]'/>
    public static ExtendedFloat FromDouble(double dbl) {
      return new ExtendedFloat(EFloat.FromDouble(dbl));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToExtendedDecimal"]'/>
    public ExtendedDecimal ToExtendedDecimal() {
      return new ExtendedDecimal(this.ef.ToExtendedDecimal());
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ToString"]'/>
    public override string ToString() {
      return this.ef.ToString();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ToEngineeringString"]'/>
    public string ToEngineeringString() {
      return this.ef.ToEngineeringString();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ToPlainString"]'/>
    public string ToPlainString() {
      return this.ef.ToPlainString();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.One"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif
    public static readonly ExtendedFloat One =
      new ExtendedFloat(EFloat.One);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.Zero"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif
    public static readonly ExtendedFloat Zero =
      new ExtendedFloat(EFloat.Zero);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.NegativeZero"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif
    public static readonly ExtendedFloat NegativeZero =
      new ExtendedFloat(EFloat.NegativeZero);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.Ten"]'/>
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
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.NaN"]'/>
    public static readonly ExtendedFloat NaN =
      new ExtendedFloat(EFloat.NaN);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.SignalingNaN"]'/>
    public static readonly ExtendedFloat SignalingNaN =
      new ExtendedFloat(EFloat.SignalingNaN);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.PositiveInfinity"]'/>
    public static readonly ExtendedFloat PositiveInfinity =
      new ExtendedFloat(EFloat.PositiveInfinity);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.NegativeInfinity"]'/>
    public static readonly ExtendedFloat NegativeInfinity =
      new ExtendedFloat(EFloat.NegativeInfinity);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.IsNegativeInfinity"]'/>
    public bool IsNegativeInfinity() {
      return this.ef.IsNegativeInfinity();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.IsPositiveInfinity"]'/>
    public bool IsPositiveInfinity() {
      return this.ef.IsPositiveInfinity();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.IsNaN"]'/>
    public bool IsNaN() {
      return this.ef.IsNaN();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.IsInfinity"]'/>
    public bool IsInfinity() {
      return this.ef.IsInfinity();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EFloat.IsFinite"]'/>
    public bool IsFinite {
      get {
        return this.ef.IsFinite;
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EFloat.IsNegative"]'/>
    public bool IsNegative {
      get {
        return this.ef.IsNegative;
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.IsQuietNaN"]'/>
    public bool IsQuietNaN() {
      return this.ef.IsQuietNaN();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.IsSignalingNaN"]'/>
    public bool IsSignalingNaN() {
      return this.ef.IsSignalingNaN();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EFloat.Sign"]'/>
    public int Sign {
      get {
        return this.ef.Sign;
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EFloat.IsZero"]'/>
    public bool IsZero {
      get {
        return this.ef.IsZero;
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Abs"]'/>
    public ExtendedFloat Abs() {
      return new ExtendedFloat(this.ef.Abs());
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Negate"]'/>
    public ExtendedFloat Negate() {
      return new ExtendedFloat(this.ef.Negate());
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Divide(PeterO.Numbers.EFloat)"]'/>
    public ExtendedFloat Divide(ExtendedFloat divisor) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.Divide(divisor.ef));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToSameExponent(PeterO.Numbers.EFloat,PeterO.Numbers.ERounding)"]'/>
    public ExtendedFloat DivideToSameExponent(ExtendedFloat divisor,
      Rounding rounding) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.DivideToSameExponent(divisor.ef,
        ExtendedDecimal.ToERounding(rounding)));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToIntegerNaturalScale(PeterO.Numbers.EFloat)"]'/>
    public ExtendedFloat DivideToIntegerNaturalScale(ExtendedFloat divisor) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.DivideToIntegerNaturalScale(divisor.ef));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Reduce(PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Reduce(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.ef.Reduce(ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RemainderNaturalScale(PeterO.Numbers.EFloat)"]'/>
    public ExtendedFloat RemainderNaturalScale(ExtendedFloat divisor) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.RemainderNaturalScale(divisor.ef));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RemainderNaturalScale(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat RemainderNaturalScale(ExtendedFloat divisor,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }

        return new ExtendedFloat(this.ef.RemainderNaturalScale(divisor.ef,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToExponent(PeterO.Numbers.EFloat,System.Int64,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat DivideToExponent(ExtendedFloat divisor,
      long desiredExponentSmall,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }

        return new ExtendedFloat(this.ef.DivideToExponent(divisor.ef,
          desiredExponentSmall, ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Divide(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Divide(ExtendedFloat divisor,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }
        return new ExtendedFloat(this.ef.Divide(divisor.ef, ctx == null ? null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToExponent(PeterO.Numbers.EFloat,System.Int64,PeterO.Numbers.ERounding)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToExponent(PeterO.Numbers.EFloat,PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat DivideToExponent(ExtendedFloat divisor,
      BigInteger exponent,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }
        if ((exponent) == null) {
          throw new ArgumentNullException("exponent");
        }

     return new ExtendedFloat(this.ef.DivideToExponent(divisor.ef,
          exponent.ei, ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToExponent(PeterO.Numbers.EFloat,PeterO.Numbers.EInteger,PeterO.Numbers.ERounding)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Abs(PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Abs(PrecisionContext context) {
      try {
        return new ExtendedFloat(this.ef.Abs(context == null ? null :
            context.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Negate(PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Negate(PrecisionContext context) {
      try {
        if ((context) == null) {
          throw new ArgumentNullException("context");
        }
        return new ExtendedFloat(this.ef.Negate(context == null ? null :
             context.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Add(PeterO.Numbers.EFloat)"]'/>
    public ExtendedFloat Add(ExtendedFloat otherValue) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedFloat(this.ef.Add(otherValue.ef));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Subtract(PeterO.Numbers.EFloat)"]'/>
    public ExtendedFloat Subtract(ExtendedFloat otherValue) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedFloat(this.ef.Subtract(otherValue.ef));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Subtract(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Subtract(ExtendedFloat otherValue,
      PrecisionContext ctx) {
      try {
        if ((otherValue) == null) {
          throw new ArgumentNullException("otherValue");
        }

        return new ExtendedFloat(this.ef.Subtract(otherValue.ef, ctx == null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Multiply(PeterO.Numbers.EFloat)"]'/>
    public ExtendedFloat Multiply(ExtendedFloat otherValue) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedFloat(this.ef.Multiply(otherValue.ef));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MultiplyAndAdd(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToIntegerNaturalScale(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat DivideToIntegerNaturalScale(ExtendedFloat divisor,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }

        return new ExtendedFloat(this.ef.DivideToIntegerNaturalScale(divisor.ef,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToIntegerZeroScale(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat DivideToIntegerZeroScale(ExtendedFloat divisor,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }

        return new ExtendedFloat(this.ef.DivideToIntegerZeroScale(divisor.ef,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Remainder(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Remainder(ExtendedFloat divisor,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }

        return new ExtendedFloat(this.ef.Remainder(divisor.ef, ctx == null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RemainderNear(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat RemainderNear(ExtendedFloat divisor,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }

        return new ExtendedFloat(this.ef.RemainderNear(divisor.ef, ctx == null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.NextMinus(PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat NextMinus(PrecisionContext ctx) {
      try {
      return new ExtendedFloat(this.ef.NextMinus(ctx == null ? null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.NextPlus(PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat NextPlus(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.ef.NextPlus(ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.NextToward(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat NextToward(ExtendedFloat otherValue,
      PrecisionContext ctx) {
      try {
        if ((otherValue) == null) {
          throw new ArgumentNullException("otherValue");
        }

        return new ExtendedFloat(this.ef.NextToward(otherValue.ef, ctx == null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Max(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public static ExtendedFloat Max(ExtendedFloat first,
      ExtendedFloat second,
      PrecisionContext ctx) {
      if ((first) == null) {
        throw new ArgumentNullException("first");
      }
      if ((second) == null) {
        throw new ArgumentNullException("second");
      }

      return new ExtendedFloat(EFloat.Max(first.ef, second.ef, ctx == null ?
        null : ctx.ec));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Min(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public static ExtendedFloat Min(ExtendedFloat first,
      ExtendedFloat second,
      PrecisionContext ctx) {
      if ((first) == null) {
        throw new ArgumentNullException("first");
      }
      if ((second) == null) {
        throw new ArgumentNullException("second");
      }

      return new ExtendedFloat(EFloat.Min(first.ef, second.ef, ctx == null ?
        null : ctx.ec));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MaxMagnitude(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public static ExtendedFloat MaxMagnitude(ExtendedFloat first,
      ExtendedFloat second,
      PrecisionContext ctx) {
      if ((first) == null) {
        throw new ArgumentNullException("first");
      }
      if ((second) == null) {
        throw new ArgumentNullException("second");
      }

      return new ExtendedFloat(EFloat.MaxMagnitude(first.ef, second.ef,
        ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MinMagnitude(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public static ExtendedFloat MinMagnitude(ExtendedFloat first,
      ExtendedFloat second,
      PrecisionContext ctx) {
      if ((first) == null) {
        throw new ArgumentNullException("first");
      }
      if ((second) == null) {
        throw new ArgumentNullException("second");
      }

      return new ExtendedFloat(EFloat.MinMagnitude(first.ef, second.ef,
        ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Max(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Min(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MaxMagnitude(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MinMagnitude(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.CompareTo(PeterO.Numbers.EFloat)"]'/>
    public int CompareTo(ExtendedFloat other) {
      if ((other) == null) {
        throw new ArgumentNullException("other");
      }
      return this.ef.CompareTo(other.ef);
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.CompareToWithContext(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat CompareToWithContext(ExtendedFloat other,
      PrecisionContext ctx) {
      try {
        if ((other) == null) {
          throw new ArgumentNullException("other");
        }

        return new ExtendedFloat(this.ef.CompareToWithContext(other.ef,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.CompareToSignal(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat CompareToSignal(ExtendedFloat other,
      PrecisionContext ctx) {
      try {
        if ((other) == null) {
          throw new ArgumentNullException("other");
        }

        return new ExtendedFloat(this.ef.CompareToSignal(other.ef, ctx == null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Add(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Add(ExtendedFloat otherValue,
      PrecisionContext ctx) {
      try {
        if ((otherValue) == null) {
          throw new ArgumentNullException("otherValue");
        }

        return new ExtendedFloat(this.ef.Add(otherValue.ef, ctx == null ? null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Quantize(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Quantize(BigInteger desiredExponent,
      PrecisionContext ctx) {
      try {
        if ((desiredExponent) == null) {
          throw new ArgumentNullException("desiredExponent");
        }

        return new ExtendedFloat(this.ef.Quantize(desiredExponent.ei,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Quantize(System.Int32,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Quantize(int desiredExponentSmall,
      PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.ef.Quantize(desiredExponentSmall,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Quantize(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Quantize(ExtendedFloat otherValue,
      PrecisionContext ctx) {
      try {
        if ((otherValue) == null) {
          throw new ArgumentNullException("otherValue");
        }
        return new ExtendedFloat(this.ef.Quantize(otherValue.ef, ctx == null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RoundToIntegralExact(PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat RoundToIntegralExact(PrecisionContext ctx) {
      try {
     return new ExtendedFloat(this.ef.RoundToIntegralExact(ctx == null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RoundToIntegralNoRoundedFlag(PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat RoundToIntegralNoRoundedFlag(PrecisionContext ctx) {
      try {
        return new
        ExtendedFloat(this.ef.RoundToIntegralNoRoundedFlag(ctx == null ?
            null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RoundToExponentExact(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat RoundToExponentExact(BigInteger exponent,
      PrecisionContext ctx) {
      try {
        if ((exponent) == null) {
          throw new ArgumentNullException("exponent");
        }

        return new ExtendedFloat(this.ef.RoundToExponentExact(exponent.ei,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RoundToExponent(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat RoundToExponent(BigInteger exponent,
      PrecisionContext ctx) {
      try {
        if ((exponent) == null) {
          throw new ArgumentNullException("exponent");
        }

        return new ExtendedFloat(this.ef.RoundToExponent(exponent.ei,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RoundToExponentExact(System.Int32,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat RoundToExponentExact(int exponentSmall,
      PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.ef.RoundToExponentExact(exponentSmall,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RoundToExponent(System.Int32,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat RoundToExponent(int exponentSmall,
      PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.ef.RoundToExponent(exponentSmall,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Multiply(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Multiply(ExtendedFloat op,
      PrecisionContext ctx) {
      try {
        if ((op) == null) {
          throw new ArgumentNullException("op");
        }

        return new ExtendedFloat(this.ef.Multiply(op.ef, ctx == null ? null :
              ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MultiplyAndAdd(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat MultiplyAndAdd(ExtendedFloat op,
      ExtendedFloat augend,
      PrecisionContext ctx) {
      try {
        if ((op) == null) {
          throw new ArgumentNullException("op");
        }
        if ((augend) == null) {
          throw new ArgumentNullException("augend");
        }

        return new ExtendedFloat(this.ef.MultiplyAndAdd(op.ef, augend.ef,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MultiplyAndSubtract(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat MultiplyAndSubtract(ExtendedFloat op,
      ExtendedFloat subtrahend,
      PrecisionContext ctx) {
      try {
        if ((op) == null) {
          throw new ArgumentNullException("op");
        }
        if ((subtrahend) == null) {
          throw new ArgumentNullException("subtrahend");
        }

     return new ExtendedFloat(this.ef.MultiplyAndSubtract(op.ef,
          subtrahend.ef, ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RoundToPrecision(PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat RoundToPrecision(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.ef.RoundToPrecision(ctx == null ? null :
               ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Plus(PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Plus(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.ef.Plus(ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RoundToBinaryPrecision(PeterO.Numbers.EContext)"]'/>
    [Obsolete(
      "Instead of this method use RoundToPrecision and pass a precision " + "context with the IsPrecisionInBits property set.")]
    public ExtendedFloat RoundToBinaryPrecision(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.ef.RoundToBinaryPrecision(ctx == null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.SquareRoot(PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat SquareRoot(PrecisionContext ctx) {
      try {
     return new ExtendedFloat(this.ef.SquareRoot(ctx == null ? null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Exp(PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Exp(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.ef.Exp(ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Log(PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Log(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.ef.Log(ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Log10(PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Log10(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.ef.Log10(ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Pow(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Pow(ExtendedFloat exponent, PrecisionContext ctx) {
      try {
        if ((exponent) == null) {
          throw new ArgumentNullException("exponent");
        }
        return new ExtendedFloat(this.ef.Pow(exponent.ef, ctx == null ? null :
               ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Pow(System.Int32,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat Pow(int exponentSmall, PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.ef.Pow(exponentSmall, ctx == null ? null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Pow(System.Int32)"]'/>
    public ExtendedFloat Pow(int exponentSmall) {
      return new ExtendedFloat(this.ef.Pow(exponentSmall));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.PI(PeterO.Numbers.EContext)"]'/>
    public static ExtendedFloat PI(PrecisionContext ctx) {
      return new ExtendedFloat(EFloat.PI(ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointLeft(System.Int32)"]'/>
    public ExtendedFloat MovePointLeft(int places) {
      return new ExtendedFloat(this.ef.MovePointLeft(places));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointLeft(System.Int32,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat MovePointLeft(int places, PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.ef.MovePointLeft(places, ctx == null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointLeft(PeterO.Numbers.EInteger)"]'/>
    public ExtendedFloat MovePointLeft(BigInteger bigPlaces) {
      if ((bigPlaces) == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedFloat(this.ef.MovePointLeft(bigPlaces.ei));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointLeft(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat MovePointLeft(BigInteger bigPlaces,
PrecisionContext ctx) {
      try {
        if ((bigPlaces) == null) {
          throw new ArgumentNullException("bigPlaces");
        }

     return new ExtendedFloat(this.ef.MovePointLeft(bigPlaces.ei, ctx ==
          null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointRight(System.Int32)"]'/>
    public ExtendedFloat MovePointRight(int places) {
      return new ExtendedFloat(this.ef.MovePointRight(places));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointRight(System.Int32,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat MovePointRight(int places, PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.ef.MovePointRight(places, ctx == null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointRight(PeterO.Numbers.EInteger)"]'/>
    public ExtendedFloat MovePointRight(BigInteger bigPlaces) {
      if ((bigPlaces) == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedFloat(this.ef.MovePointRight(bigPlaces.ei));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointRight(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat MovePointRight(BigInteger bigPlaces,
PrecisionContext ctx) {
      try {
        if ((bigPlaces) == null) {
          throw new ArgumentNullException("bigPlaces");
        }

        return new ExtendedFloat(this.ef.MovePointRight(bigPlaces.ei,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ScaleByPowerOfTwo(System.Int32)"]'/>
    public ExtendedFloat ScaleByPowerOfTwo(int places) {
      return new ExtendedFloat(this.ef.ScaleByPowerOfTwo(places));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ScaleByPowerOfTwo(System.Int32,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat ScaleByPowerOfTwo(int places, PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.ef.ScaleByPowerOfTwo(places, ctx == null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ScaleByPowerOfTwo(PeterO.Numbers.EInteger)"]'/>
    public ExtendedFloat ScaleByPowerOfTwo(BigInteger bigPlaces) {
      if ((bigPlaces) == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedFloat(this.ef.ScaleByPowerOfTwo(bigPlaces.ei));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ScaleByPowerOfTwo(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat ScaleByPowerOfTwo(BigInteger bigPlaces,
PrecisionContext ctx) {
      try {
        if ((bigPlaces) == null) {
          throw new ArgumentNullException("bigPlaces");
        }

        return new ExtendedFloat(this.ef.ScaleByPowerOfTwo(bigPlaces.ei,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Precision"]'/>
    public BigInteger Precision() {
      return new BigInteger(this.ef.Precision());
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Ulp"]'/>
    public ExtendedFloat Ulp() {
      return new ExtendedFloat(this.ef.Ulp());
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideAndRemainderNaturalScale(PeterO.Numbers.EFloat)"]'/>
    public ExtendedFloat[] DivideAndRemainderNaturalScale(ExtendedFloat
         divisor) {
      EFloat[] edec = this.ef.DivideAndRemainderNaturalScale(divisor == null?
        null : divisor.ef);
      return new ExtendedFloat[] {
        new ExtendedFloat(edec[0]), new ExtendedFloat(edec[1])
      };
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideAndRemainderNaturalScale(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]'/>
    public ExtendedFloat[] DivideAndRemainderNaturalScale(
      ExtendedFloat divisor,
      PrecisionContext ctx) {
      try {
        EFloat[] edec = this.ef.DivideAndRemainderNaturalScale(divisor ==
          null ? null : divisor.ef,
          ctx == null ? null : ctx.ec);
        return new ExtendedFloat[] {
        new ExtendedFloat(edec[0]), new ExtendedFloat(edec[1])
      };
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }
  }
}
