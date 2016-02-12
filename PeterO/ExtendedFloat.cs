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
    /// <include file='../docs.xml'
    /// path='docs/doc[@name="T:PeterO.ExtendedFloat"]/*'/>
  public sealed class ExtendedFloat : IComparable<ExtendedFloat>,
 IEquatable<ExtendedFloat> {
    private readonly EFloat ef;

    internal ExtendedFloat(EFloat ef) {
      this.ef = ef;
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.Exponent"]/*'/>
    public BigInteger Exponent {
      get {
        return new BigInteger(this.Ef.Exponent);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.UnsignedMantissa"]/*'/>
    public BigInteger UnsignedMantissa {
      get {
        return new BigInteger(this.Ef.UnsignedMantissa);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.Mantissa"]/*'/>
    public BigInteger Mantissa {
      get {
        return new BigInteger(this.Ef.Mantissa);
      }
    }

    internal static ExtendedFloat ToLegacy(EFloat ei) {
      return new ExtendedFloat(ei);
    }

    internal static EFloat FromLegacy(ExtendedFloat bei) {
      return bei.Ef;
    }

    #region Equals and GetHashCode implementation
    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.EqualsInternal(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool EqualsInternal(ExtendedFloat otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      return this.Ef.EqualsInternal(otherValue.Ef);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Equals(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool Equals(ExtendedFloat other) {
      if (other == null) {
        throw new ArgumentNullException("other");
      }
      return this.Ef.Equals(other.Ef);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Equals(System.Object)"]/*'/>
    public override bool Equals(object obj) {
      var bi = obj as ExtendedFloat;
      return (bi == null) ? false : this.Ef.Equals(bi.Ef);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.GetHashCode"]/*'/>
    public override int GetHashCode() {
      return this.Ef.GetHashCode();
    }
    #endregion

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.CreateNaN(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat CreateNaN(BigInteger diag) {
      if (diag == null) {
        throw new ArgumentNullException("diag");
      }
      return new ExtendedFloat(EFloat.CreateNaN(diag.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.CreateNaN(PeterO.BigInteger,System.Boolean,System.Boolean,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat CreateNaN(
BigInteger diag,
bool signaling,
bool negative,
PrecisionContext ctx) {
      if (diag == null) {
        throw new ArgumentNullException("diag");
      }

      return new ExtendedFloat(
EFloat.CreateNaN(
diag.Ei,
signaling,
negative,
ctx == null ? null : ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Create(System.Int32,System.Int32)"]/*'/>
    public static ExtendedFloat Create(int mantissaSmall, int exponentSmall) {
      return new ExtendedFloat(EFloat.Create(mantissaSmall, exponentSmall));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Create(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    public static ExtendedFloat Create(
BigInteger mantissa,
BigInteger exponent) {
      if (mantissa == null) {
        throw new ArgumentNullException("mantissa");
      }
      if (exponent == null) {
        throw new ArgumentNullException("exponent");
      }
      return new ExtendedFloat(EFloat.Create(mantissa.Ei, exponent.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromString(System.String,System.Int32,System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat FromString(
string str,
int offset,
int length,
PrecisionContext ctx) {
      try {
        return new ExtendedFloat(
  EFloat.FromString(
  str,
  offset,
  length,
  ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromString(System.String)"]/*'/>
    public static ExtendedFloat FromString(string str) {
      return new ExtendedFloat(EFloat.FromString(str));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromString(System.String,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat FromString(string str, PrecisionContext ctx) {
      try {
        return new ExtendedFloat(
  EFloat.FromString(
  str,
  ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromString(System.String,System.Int32,System.Int32)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat FromString(string str, int offset, int length) {
      return new ExtendedFloat(EFloat.FromString(str, offset, length));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToBigInteger"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public BigInteger ToBigInteger() {
      return new BigInteger(this.Ef.ToEInteger());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToBigIntegerExact"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public BigInteger ToBigIntegerExact() {
      return new BigInteger(this.Ef.ToEIntegerExact());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToSingle"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public float ToSingle() {
      return this.Ef.ToSingle();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToDouble"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    /// <summary>Not documented yet.</summary>
    /// <returns>A 64-bit floating-point number.</returns>
    public double ToDouble() {
      return this.Ef.ToDouble();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromSingle(System.Single)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat FromSingle(float flt) {
      return new ExtendedFloat(EFloat.FromSingle(flt));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromBigInteger(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat FromBigInteger(BigInteger bigint) {
      if (bigint == null) {
        throw new ArgumentNullException("bigint");
      }
      return new ExtendedFloat(EFloat.FromEInteger(bigint.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromInt64(System.Int64)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat FromInt64(long valueSmall) {
      return new ExtendedFloat(EFloat.FromInt64(valueSmall));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromInt32(System.Int32)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat FromInt32(int valueSmaller) {
      return new ExtendedFloat(EFloat.FromInt32(valueSmaller));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.FromDouble(System.Double)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat FromDouble(double dbl) {
      return new ExtendedFloat(EFloat.FromDouble(dbl));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToExtendedDecimal"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal ToExtendedDecimal() {
      return new ExtendedDecimal(this.Ef.ToExtendedDecimal());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToString"]/*'/>
    public override string ToString() {
      return this.Ef.ToString();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToEngineeringString"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public string ToEngineeringString() {
      return this.Ef.ToEngineeringString();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToPlainString"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public string ToPlainString() {
      return this.Ef.ToPlainString();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.One"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly ExtendedFloat One =
     new ExtendedFloat(EFloat.One);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.Zero"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly ExtendedFloat Zero =
     new ExtendedFloat(EFloat.Zero);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.NegativeZero"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly ExtendedFloat NegativeZero =
     new ExtendedFloat(EFloat.NegativeZero);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.Ten"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif

    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly ExtendedFloat Ten =
     new ExtendedFloat(EFloat.Ten);

    //----------------------------------------------------------------

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.NaN"]/*'/>
    public static readonly ExtendedFloat NaN =
     new ExtendedFloat(EFloat.NaN);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.SignalingNaN"]/*'/>
    public static readonly ExtendedFloat SignalingNaN =
     new ExtendedFloat(EFloat.SignalingNaN);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.PositiveInfinity"]/*'/>
    public static readonly ExtendedFloat PositiveInfinity =
     new ExtendedFloat(EFloat.PositiveInfinity);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedFloat.NegativeInfinity"]/*'/>
    public static readonly ExtendedFloat NegativeInfinity =
     new ExtendedFloat(EFloat.NegativeInfinity);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsNegativeInfinity"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsNegativeInfinity() {
      return this.Ef.IsNegativeInfinity();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsPositiveInfinity"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsPositiveInfinity() {
      return this.Ef.IsPositiveInfinity();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsNaN"]/*'/>
    public bool IsNaN() {
      return this.Ef.IsNaN();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsInfinity"]/*'/>
    public bool IsInfinity() {
      return this.Ef.IsInfinity();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.IsFinite"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsFinite {
      get {
        return this.Ef.IsFinite;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.IsNegative"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsNegative {
      get {
        return this.Ef.IsNegative;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsQuietNaN"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsQuietNaN() {
      return this.Ef.IsQuietNaN();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsSignalingNaN"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsSignalingNaN() {
      return this.Ef.IsSignalingNaN();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.Sign"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public int Sign {
      get {
        return this.Ef.Sign;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.IsZero"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsZero {
      get {
        return this.Ef.IsZero;
      }
    }

    internal EFloat Ef {
      get {
        return this.ef;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Abs"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Abs() {
      return new ExtendedFloat(this.Ef.Abs(null));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Negate"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Negate() {
      return new ExtendedFloat(this.Ef.Negate(null));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Divide(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Divide(ExtendedFloat divisor) {
      if (divisor == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.Ef.Divide(divisor.Ef));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToSameExponent(PeterO.ExtendedFloat,PeterO.Rounding)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat DivideToSameExponent(
ExtendedFloat divisor,
Rounding rounding) {
      if (divisor == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(
this.Ef.DivideToSameExponent(
divisor.Ef,
ExtendedDecimal.ToERounding(rounding)));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToIntegerNaturalScale(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat DivideToIntegerNaturalScale(ExtendedFloat divisor) {
      if (divisor == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.Ef.DivideToIntegerNaturalScale(divisor.Ef));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Reduce(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Reduce(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.Ef.Reduce(ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RemainderNaturalScale(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat RemainderNaturalScale(ExtendedFloat divisor) {
      if (divisor == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.Ef.RemainderNaturalScale(divisor.Ef));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RemainderNaturalScale(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat RemainderNaturalScale(
ExtendedFloat divisor,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }

        return new ExtendedFloat(
this.Ef.RemainderNaturalScale(
divisor.Ef,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToExponent(PeterO.ExtendedFloat,System.Int64,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat DivideToExponent(
ExtendedFloat divisor,
long desiredExponentSmall,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }

        return new ExtendedFloat(
this.Ef.DivideToExponent(
divisor.Ef,
desiredExponentSmall,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Divide(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Divide(
ExtendedFloat divisor,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }
        return new ExtendedFloat(
this.Ef.Divide(
divisor.Ef,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToExponent(PeterO.ExtendedFloat,System.Int64,PeterO.Rounding)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat DivideToExponent(
ExtendedFloat divisor,
long desiredExponentSmall,
Rounding rounding) {
      if (divisor == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(
this.Ef.DivideToExponent(
divisor.Ef,
desiredExponentSmall,
ExtendedDecimal.ToERounding(rounding)));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToExponent(PeterO.ExtendedFloat,PeterO.BigInteger,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat DivideToExponent(
ExtendedFloat divisor,
BigInteger exponent,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }
        if (exponent == null) {
          throw new ArgumentNullException("exponent");
        }

        return new ExtendedFloat(
   this.Ef.DivideToExponent(
   divisor.Ef,
   exponent.Ei,
   ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToExponent(PeterO.ExtendedFloat,PeterO.BigInteger,PeterO.Rounding)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat DivideToExponent(
ExtendedFloat divisor,
BigInteger desiredExponent,
Rounding rounding) {
      if (divisor == null) {
        throw new ArgumentNullException("divisor");
      }
      if (desiredExponent == null) {
        throw new ArgumentNullException("desiredExponent");
      }
      return new ExtendedFloat(
this.Ef.DivideToExponent(
divisor.Ef,
desiredExponent.Ei,
ExtendedDecimal.ToERounding(rounding)));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Abs(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Abs(PrecisionContext context) {
      try {
        return new ExtendedFloat(this.Ef.Abs(context == null ? null :
            context.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Negate(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Negate(PrecisionContext context) {
      try {
        if (context == null) {
          throw new ArgumentNullException("context");
        }
        return new ExtendedFloat(this.Ef.Negate(context == null ? null :
             context.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Add(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Add(ExtendedFloat otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedFloat(this.Ef.Add(otherValue.Ef));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Subtract(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Subtract(ExtendedFloat otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedFloat(this.Ef.Subtract(otherValue.Ef));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Subtract(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Subtract(
ExtendedFloat otherValue,
PrecisionContext ctx) {
      try {
        if (otherValue == null) {
          throw new ArgumentNullException("otherValue");
        }

        return new ExtendedFloat(
this.Ef.Subtract(
otherValue.Ef,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Multiply(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Multiply(ExtendedFloat otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedFloat(this.Ef.Multiply(otherValue.Ef));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MultiplyAndAdd(PeterO.ExtendedFloat,PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat MultiplyAndAdd(
ExtendedFloat multiplicand,
ExtendedFloat augend) {
      if (multiplicand == null) {
        throw new ArgumentNullException("multiplicand");
      }
      if (augend == null) {
        throw new ArgumentNullException("augend");
      }
      return new ExtendedFloat(
this.Ef.MultiplyAndAdd(
multiplicand.Ef,
augend.Ef));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToIntegerNaturalScale(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat DivideToIntegerNaturalScale(
ExtendedFloat divisor,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }

        return new ExtendedFloat(
this.Ef.DivideToIntegerNaturalScale(
divisor.Ef,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideToIntegerZeroScale(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat DivideToIntegerZeroScale(
ExtendedFloat divisor,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }

        return new ExtendedFloat(
this.Ef.DivideToIntegerZeroScale(
divisor.Ef,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Remainder(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Remainder(
ExtendedFloat divisor,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }

        return new ExtendedFloat(
this.Ef.Remainder(
divisor.Ef,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RemainderNear(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat RemainderNear(
ExtendedFloat divisor,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }

        return new ExtendedFloat(
this.Ef.RemainderNear(
divisor.Ef,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.NextMinus(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat NextMinus(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.Ef.NextMinus(ctx == null ? null :
            ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.NextPlus(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat NextPlus(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.Ef.NextPlus(ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.NextToward(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat NextToward(
ExtendedFloat otherValue,
PrecisionContext ctx) {
      try {
        if (otherValue == null) {
          throw new ArgumentNullException("otherValue");
        }

        return new ExtendedFloat(
this.Ef.NextToward(
otherValue.Ef,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Max(PeterO.ExtendedFloat,PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat Max(
ExtendedFloat first,
ExtendedFloat second,
PrecisionContext ctx) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }

      return new ExtendedFloat(
EFloat.Max(
first.Ef,
second.Ef,
ctx == null ? null : ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Min(PeterO.ExtendedFloat,PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat Min(
ExtendedFloat first,
ExtendedFloat second,
PrecisionContext ctx) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }

      return new ExtendedFloat(
EFloat.Min(
first.Ef,
second.Ef,
ctx == null ? null : ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MaxMagnitude(PeterO.ExtendedFloat,PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat MaxMagnitude(
ExtendedFloat first,
ExtendedFloat second,
PrecisionContext ctx) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }

      return new ExtendedFloat(
EFloat.MaxMagnitude(
first.Ef,
second.Ef,
ctx == null ? null : ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MinMagnitude(PeterO.ExtendedFloat,PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat MinMagnitude(
ExtendedFloat first,
ExtendedFloat second,
PrecisionContext ctx) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }

      return new ExtendedFloat(
EFloat.MinMagnitude(
first.Ef,
second.Ef,
ctx == null ? null : ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Max(PeterO.ExtendedFloat,PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat Max(
ExtendedFloat first,
ExtendedFloat second) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedFloat(EFloat.Max(first.Ef, second.Ef));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Min(PeterO.ExtendedFloat,PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat Min(
ExtendedFloat first,
ExtendedFloat second) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedFloat(EFloat.Min(first.Ef, second.Ef));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MaxMagnitude(PeterO.ExtendedFloat,PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat MaxMagnitude(
ExtendedFloat first,
ExtendedFloat second) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedFloat(EFloat.MaxMagnitude(first.Ef, second.Ef));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MinMagnitude(PeterO.ExtendedFloat,PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat MinMagnitude(
ExtendedFloat first,
ExtendedFloat second) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedFloat(EFloat.MinMagnitude(first.Ef, second.Ef));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.CompareTo(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public int CompareTo(ExtendedFloat other) {
      if (other == null) {
        throw new ArgumentNullException("other");
      }
      return this.Ef.CompareTo(other.Ef);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.CompareToWithContext(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat CompareToWithContext(
ExtendedFloat other,
PrecisionContext ctx) {
      try {
        if (other == null) {
          throw new ArgumentNullException("other");
        }

        return new ExtendedFloat(
this.Ef.CompareToWithContext(
other.Ef,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.CompareToSignal(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat CompareToSignal(
ExtendedFloat other,
PrecisionContext ctx) {
      try {
        if (other == null) {
          throw new ArgumentNullException("other");
        }

        return new ExtendedFloat(
this.Ef.CompareToSignal(
other.Ef,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Add(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Add(
ExtendedFloat otherValue,
PrecisionContext ctx) {
      try {
        if (otherValue == null) {
          throw new ArgumentNullException("otherValue");
        }

        return new ExtendedFloat(
this.Ef.Add(
otherValue.Ef,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Quantize(PeterO.BigInteger,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Quantize(
BigInteger desiredExponent,
PrecisionContext ctx) {
      try {
        if (desiredExponent == null) {
          throw new ArgumentNullException("desiredExponent");
        }

        return new ExtendedFloat(
this.Ef.Quantize(
desiredExponent.Ei,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Quantize(System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Quantize(
int desiredExponentSmall,
PrecisionContext ctx) {
      try {
        return new ExtendedFloat(
this.Ef.Quantize(
desiredExponentSmall,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Quantize(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Quantize(
ExtendedFloat otherValue,
PrecisionContext ctx) {
      try {
        if (otherValue == null) {
          throw new ArgumentNullException("otherValue");
        }
        return new ExtendedFloat(
this.Ef.Quantize(
otherValue.Ef,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToIntegralExact(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat RoundToIntegralExact(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.Ef.RoundToIntegralExact(ctx == null ?
             null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToIntegralNoRoundedFlag(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat RoundToIntegralNoRoundedFlag(PrecisionContext ctx) {
      try {
        return new
        ExtendedFloat(this.Ef.RoundToIntegralNoRoundedFlag(ctx == null ?
            null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToExponentExact(PeterO.BigInteger,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat RoundToExponentExact(
BigInteger exponent,
PrecisionContext ctx) {
      try {
        if (exponent == null) {
          throw new ArgumentNullException("exponent");
        }

        return new ExtendedFloat(
this.Ef.RoundToExponentExact(
exponent.Ei,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToExponent(PeterO.BigInteger,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat RoundToExponent(
BigInteger exponent,
PrecisionContext ctx) {
      try {
        if (exponent == null) {
          throw new ArgumentNullException("exponent");
        }

        return new ExtendedFloat(
this.Ef.RoundToExponent(
exponent.Ei,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToExponentExact(System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat RoundToExponentExact(
int exponentSmall,
PrecisionContext ctx) {
      try {
        return new ExtendedFloat(
this.Ef.RoundToExponentExact(
exponentSmall,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToExponent(System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat RoundToExponent(
int exponentSmall,
PrecisionContext ctx) {
      try {
        return new ExtendedFloat(
this.Ef.RoundToExponent(
exponentSmall,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Multiply(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Multiply(
ExtendedFloat op,
PrecisionContext ctx) {
      try {
        if (op == null) {
          throw new ArgumentNullException("op");
        }

        return new ExtendedFloat(
this.Ef.Multiply(
op.Ef,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MultiplyAndAdd(PeterO.ExtendedFloat,PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat MultiplyAndAdd(
ExtendedFloat op,
ExtendedFloat augend,
PrecisionContext ctx) {
      try {
        if (op == null) {
          throw new ArgumentNullException("op");
        }
        if (augend == null) {
          throw new ArgumentNullException("augend");
        }

        return new ExtendedFloat(
this.Ef.MultiplyAndAdd(
op.Ef,
augend.Ef,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MultiplyAndSubtract(PeterO.ExtendedFloat,PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat MultiplyAndSubtract(
ExtendedFloat op,
ExtendedFloat subtrahend,
PrecisionContext ctx) {
      try {
        if (op == null) {
          throw new ArgumentNullException("op");
        }
        if (subtrahend == null) {
          throw new ArgumentNullException("subtrahend");
        }

        return new ExtendedFloat(
   this.Ef.MultiplyAndSubtract(
   op.Ef,
   subtrahend.Ef,
   ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToPrecision(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat RoundToPrecision(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.Ef.RoundToPrecision(ctx == null ? null :
               ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Plus(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Plus(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.Ef.Plus(ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.RoundToBinaryPrecision(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat RoundToBinaryPrecision(PrecisionContext ctx) {
      if (ctx == null) {
        return this;
      }
      PrecisionContext ctx2 = ctx.Copy().WithPrecisionInBits(true);
      ExtendedFloat ret = this.RoundToPrecision(ctx2);
      if (ctx2.HasFlags) {
        ctx.Flags = ctx2.Flags;
      }
      return ret;
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.SquareRoot(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat SquareRoot(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.Ef.SquareRoot(ctx == null ? null :
             ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Exp(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Exp(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.Ef.Exp(ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Log(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Log(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.Ef.Log(ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Log10(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Log10(PrecisionContext ctx) {
      try {
        return new ExtendedFloat(this.Ef.Log10(ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Pow(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Pow(ExtendedFloat exponent, PrecisionContext ctx) {
      try {
        if (exponent == null) {
          throw new ArgumentNullException("exponent");
        }
        return new ExtendedFloat(
this.Ef.Pow(
exponent.Ef,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Pow(System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Pow(int exponentSmall, PrecisionContext ctx) {
      try {
        return new ExtendedFloat(
this.Ef.Pow(
exponentSmall,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Pow(System.Int32)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Pow(int exponentSmall) {
      return new ExtendedFloat(this.Ef.Pow(exponentSmall));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.PI(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat PI(PrecisionContext ctx) {
      return new ExtendedFloat(EFloat.PI(ctx == null ? null : ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointLeft(System.Int32)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat MovePointLeft(int places) {
      return new ExtendedFloat(this.Ef.MovePointLeft(places));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointLeft(System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat MovePointLeft(int places, PrecisionContext ctx) {
      try {
        return new ExtendedFloat(
this.Ef.MovePointLeft(
places,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointLeft(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat MovePointLeft(BigInteger bigPlaces) {
      if (bigPlaces == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedFloat(this.Ef.MovePointLeft(bigPlaces.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointLeft(PeterO.BigInteger,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat MovePointLeft(
BigInteger bigPlaces,
PrecisionContext ctx) {
      try {
        if (bigPlaces == null) {
          throw new ArgumentNullException("bigPlaces");
        }

        return new ExtendedFloat(
   this.Ef.MovePointLeft(
   bigPlaces.Ei,
   ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointRight(System.Int32)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat MovePointRight(int places) {
      return new ExtendedFloat(this.Ef.MovePointRight(places));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointRight(System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat MovePointRight(int places, PrecisionContext ctx) {
      try {
        return new ExtendedFloat(
this.Ef.MovePointRight(
places,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointRight(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat MovePointRight(BigInteger bigPlaces) {
      if (bigPlaces == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedFloat(this.Ef.MovePointRight(bigPlaces.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.MovePointRight(PeterO.BigInteger,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat MovePointRight(
BigInteger bigPlaces,
PrecisionContext ctx) {
      try {
        if (bigPlaces == null) {
          throw new ArgumentNullException("bigPlaces");
        }

        return new ExtendedFloat(
this.Ef.MovePointRight(
bigPlaces.Ei,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ScaleByPowerOfTwo(System.Int32)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat ScaleByPowerOfTwo(int places) {
      return new ExtendedFloat(this.Ef.ScaleByPowerOfTwo(places));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ScaleByPowerOfTwo(System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat ScaleByPowerOfTwo(int places, PrecisionContext ctx) {
      try {
        return new ExtendedFloat(
this.Ef.ScaleByPowerOfTwo(
places,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ScaleByPowerOfTwo(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat ScaleByPowerOfTwo(BigInteger bigPlaces) {
      if (bigPlaces == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedFloat(this.Ef.ScaleByPowerOfTwo(bigPlaces.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ScaleByPowerOfTwo(PeterO.BigInteger,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat ScaleByPowerOfTwo(
BigInteger bigPlaces,
PrecisionContext ctx) {
      try {
        if (bigPlaces == null) {
          throw new ArgumentNullException("bigPlaces");
        }

        return new ExtendedFloat(
this.Ef.ScaleByPowerOfTwo(
bigPlaces.Ei,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Precision"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public BigInteger Precision() {
      return new BigInteger(this.Ef.Precision());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.Ulp"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat Ulp() {
      return new ExtendedFloat(this.Ef.Ulp());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideAndRemainderNaturalScale(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat[] DivideAndRemainderNaturalScale(ExtendedFloat
        divisor) {
      EFloat[] edec = this.Ef.DivideAndRemainderNaturalScale(divisor == null ?
        null : divisor.Ef);
      return new ExtendedFloat[] {
        new ExtendedFloat(edec[0]), new ExtendedFloat(edec[1])
      };
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.DivideAndRemainderNaturalScale(PeterO.ExtendedFloat,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat[] DivideAndRemainderNaturalScale(
     ExtendedFloat divisor,
     PrecisionContext ctx) {
      try {
        EFloat[] edec = this.Ef.DivideAndRemainderNaturalScale(
divisor == null ? null : divisor.Ef,
ctx == null ? null : ctx.Ec);
        return new ExtendedFloat[] {
        new ExtendedFloat(edec[0]), new ExtendedFloat(edec[1])
      };
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }
  }
}
