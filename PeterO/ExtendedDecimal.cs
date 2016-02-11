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
    /// <include file='../docs.xml'
    /// path='docs/doc[@name="T:PeterO.ExtendedDecimal"]/*'/>
  public sealed class ExtendedDecimal : IComparable<ExtendedDecimal>,
  IEquatable<ExtendedDecimal> {
    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedDecimal.Exponent"]/*'/>
    public BigInteger Exponent {
      get {
        return new BigInteger(this.Ed.Exponent);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedDecimal.UnsignedMantissa"]/*'/>
    public BigInteger UnsignedMantissa {
      get {
        return new BigInteger(this.Ed.UnsignedMantissa);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedDecimal.Mantissa"]/*'/>
    public BigInteger Mantissa {
      get {
        return new BigInteger(this.Ed.Mantissa);
      }
    }

    internal static ExtendedDecimal ToLegacy(EDecimal ei) {
      return new ExtendedDecimal(ei);
    }

    internal static EDecimal FromLegacy(ExtendedDecimal bei) {
      return bei.Ed;
    }

    #region Equals and GetHashCode implementation

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Equals(PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool Equals(ExtendedDecimal other) {
      return (other == null) ? false : this.Ed.Equals(other.Ed);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Equals(System.Object)"]/*'/>
    public override bool Equals(object obj) {
      var bi = obj as ExtendedDecimal;
      return (bi == null) ? false : this.Ed.Equals(bi.Ed);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.GetHashCode"]/*'/>
    public override int GetHashCode() {
      return this.Ed.GetHashCode();
    }
    #endregion

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Create(System.Int32,System.Int32)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal Create(int mantissaSmall, int exponentSmall) {
      return new ExtendedDecimal(EDecimal.Create(mantissaSmall, exponentSmall));
    }

    private readonly EDecimal ed;

    internal ExtendedDecimal(EDecimal ed) {
      if (ed == null) {
        throw new ArgumentNullException("ed");
      }
      this.ed = ed;
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Create(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    public static ExtendedDecimal Create(
      BigInteger mantissa,
      BigInteger exponent) {
      if (mantissa == null) {
        throw new ArgumentNullException("mantissa");
      }
      if (exponent == null) {
        throw new ArgumentNullException("exponent");
      }
      return new ExtendedDecimal(EDecimal.Create(mantissa.Ei, exponent.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.CreateNaN(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal CreateNaN(BigInteger diag) {
      if (diag == null) {
        throw new ArgumentNullException("diag");
      }
      return new ExtendedDecimal(EDecimal.CreateNaN(diag.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.CreateNaN(PeterO.BigInteger,System.Boolean,System.Boolean,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal CreateNaN(
BigInteger diag,
bool signaling,
bool negative,
PrecisionContext ctx) {
      if (diag == null) {
        throw new ArgumentNullException("diag");
      }
      return new ExtendedDecimal(
  EDecimal.CreateNaN(
  diag.Ei,
  signaling,
  negative,
  ctx == null ? null : ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromString(System.String)"]/*'/>
    public static ExtendedDecimal FromString(string str) {
      return new ExtendedDecimal(EDecimal.FromString(str));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromString(System.String,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal FromString(string str, PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(
  EDecimal.FromString(
  str,
  ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromString(System.String,System.Int32,System.Int32)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal FromString(
string str,
int offset,
int length) {
      return new ExtendedDecimal(EDecimal.FromString(str, offset, length));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromString(System.String,System.Int32,System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal FromString(
string str,
int offset,
int length,
PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(
  EDecimal.FromString(
  str,
  offset,
  length,
  ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.CompareToBinary(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public int CompareToBinary(ExtendedFloat other) {
      return (other == null) ? 1 : this.Ed.CompareToBinary(other.Ef);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToBigInteger"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public BigInteger ToBigInteger() {
      return new BigInteger(this.Ed.ToEInteger());
    }

  [Obsolete(
"Implements legacy behavior using an obsolete class for convenience.")]
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
      return (r == Rounding.Unnecessary) ? ERounding.None : ((r ==
        Rounding.Odd) ? ERounding.Odd : ERounding.Down);
    }

#pragma warning disable 618
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "",
      "CS0618",
      Justification = "Implements a conversion from a new class to a legacy class.")]
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
      return (r == ERounding.None) ? Rounding.Unnecessary : ((r ==
        ERounding.Odd) ? Rounding.Odd : Rounding.Down);
    }
#pragma warning restore 618

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToBigIntegerExact"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public BigInteger ToBigIntegerExact() {
      return new BigInteger(this.Ed.ToEIntegerExact());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToExtendedFloat"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedFloat ToExtendedFloat() {
      return new ExtendedFloat(this.Ed.ToExtendedFloat());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToSingle"]/*'/>
    public float ToSingle() {
      return this.Ed.ToSingle();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToDouble"]/*'/>
    public double ToDouble() {
      return this.Ed.ToDouble();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromSingle(System.Single)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal FromSingle(float flt) {
      return new ExtendedDecimal(EDecimal.FromSingle(flt));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromBigInteger(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal FromBigInteger(BigInteger bigint) {
      if (bigint == null) {
        throw new ArgumentNullException("bigint");
      }
      return new ExtendedDecimal(EDecimal.FromEInteger(bigint.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromInt64(System.Int64)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal FromInt64(long valueSmall) {
      return new ExtendedDecimal(EDecimal.FromInt64(valueSmall));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromInt32(System.Int32)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal FromInt32(int valueSmaller) {
      return new ExtendedDecimal(EDecimal.FromInt32(valueSmaller));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromDouble(System.Double)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal FromDouble(double dbl) {
      return new ExtendedDecimal(EDecimal.FromDouble(dbl));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.FromExtendedFloat(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal FromExtendedFloat(ExtendedFloat bigfloat) {
      if (bigfloat == null) {
        throw new ArgumentNullException("bigfloat");
      }
      return new ExtendedDecimal(EDecimal.FromExtendedFloat(bigfloat.Ef));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToString"]/*'/>
    public override string ToString() {
      return this.Ed.ToString();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToEngineeringString"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public string ToEngineeringString() {
      return this.Ed.ToEngineeringString();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ToPlainString"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public string ToPlainString() {
      return this.Ed.ToPlainString();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.One"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly ExtendedDecimal One =
      ExtendedDecimal.Create(BigInteger.One, BigInteger.Zero);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.Zero"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly ExtendedDecimal Zero =
      ExtendedDecimal.Create(BigInteger.Zero, BigInteger.Zero);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.NegativeZero"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    public static readonly ExtendedDecimal NegativeZero =
      new ExtendedDecimal(EDecimal.NegativeZero);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.Ten"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    public static readonly ExtendedDecimal Ten =
      new ExtendedDecimal(EDecimal.Ten);

    //----------------------------------------------------------------

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.NaN"]/*'/>
    public static readonly ExtendedDecimal NaN =
      new ExtendedDecimal(EDecimal.NaN);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.SignalingNaN"]/*'/>
    public static readonly ExtendedDecimal SignalingNaN =
      new ExtendedDecimal(EDecimal.SignalingNaN);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.PositiveInfinity"]/*'/>
    public static readonly ExtendedDecimal PositiveInfinity =
      new ExtendedDecimal(EDecimal.PositiveInfinity);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.ExtendedDecimal.NegativeInfinity"]/*'/>
    public static readonly ExtendedDecimal NegativeInfinity =
      new ExtendedDecimal(EDecimal.NegativeInfinity);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.IsNegativeInfinity"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsNegativeInfinity() {
      return this.Ed.IsNegativeInfinity();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.IsPositiveInfinity"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsPositiveInfinity() {
      return this.Ed.IsPositiveInfinity();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.IsNaN"]/*'/>
    public bool IsNaN() {
      return this.Ed.IsNaN();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.IsInfinity"]/*'/>
    public bool IsInfinity() {
      return this.Ed.IsInfinity();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedDecimal.IsFinite"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsFinite {
      get {
        return this.Ed.IsFinite;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedDecimal.IsNegative"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsNegative {
      get {
        return this.Ed.IsNegative;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.IsQuietNaN"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsQuietNaN() {
      return this.Ed.IsQuietNaN();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.IsSignalingNaN"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsSignalingNaN() {
      return this.Ed.IsSignalingNaN();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedDecimal.Sign"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public int Sign {
      get {
        return this.Ed.Sign;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedDecimal.IsZero"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsZero {
      get {
        return this.Ed.IsZero;
      }
    }

    internal EDecimal Ed {
      get {
        return this.ed;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Abs"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Abs() {
      return new ExtendedDecimal(this.Ed.Abs(null));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Negate"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Negate() {
      return new ExtendedDecimal(this.Ed.Negate(null));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Divide(PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Divide(ExtendedDecimal divisor) {
      if (divisor == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedDecimal(this.Ed.Divide(divisor.Ed));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToSameExponent(PeterO.ExtendedDecimal,PeterO.Rounding)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal DivideToSameExponent(
ExtendedDecimal divisor,
Rounding rounding) {
      if (divisor == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedDecimal(
this.Ed.DivideToSameExponent(
divisor.Ed,
ExtendedDecimal.ToERounding(rounding)));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToIntegerNaturalScale(PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal DivideToIntegerNaturalScale(ExtendedDecimal
                    divisor) {
      if (divisor == null) {
        throw new ArgumentNullException("divisor");
      }
   return new ExtendedDecimal(this.Ed.DivideToIntegerNaturalScale(divisor.Ed));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Reduce(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Reduce(PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.Ed.Reduce(ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RemainderNaturalScale(PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal RemainderNaturalScale(ExtendedDecimal divisor) {
      if (divisor == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedDecimal(this.Ed.RemainderNaturalScale(divisor.Ed));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RemainderNaturalScale(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal RemainderNaturalScale(
ExtendedDecimal divisor,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }
        return new ExtendedDecimal(
this.Ed.RemainderNaturalScale(
divisor.Ed,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToExponent(PeterO.ExtendedDecimal,System.Int64,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal DivideToExponent(
ExtendedDecimal divisor,
long desiredExponentSmall,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }
        return new ExtendedDecimal(
this.Ed.DivideToExponent(
divisor.Ed,
desiredExponentSmall,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Divide(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Divide(
ExtendedDecimal divisor,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }
        return new ExtendedDecimal(
   this.Ed.Divide(
   divisor.Ed,
   ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToExponent(PeterO.ExtendedDecimal,System.Int64,PeterO.Rounding)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal DivideToExponent(
ExtendedDecimal divisor,
long desiredExponentSmall,
Rounding rounding) {
      if (divisor == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedDecimal(
this.Ed.DivideToExponent(
divisor.Ed,
desiredExponentSmall,
ExtendedDecimal.ToERounding(rounding)));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToExponent(PeterO.ExtendedDecimal,PeterO.BigInteger,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal DivideToExponent(
ExtendedDecimal divisor,
BigInteger exponent,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }
        if (exponent == null) {
          throw new ArgumentNullException("exponent");
        }
        return new ExtendedDecimal(
     this.Ed.DivideToExponent(
     divisor.Ed,
     exponent.Ei,
     ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToExponent(PeterO.ExtendedDecimal,PeterO.BigInteger,PeterO.Rounding)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal DivideToExponent(
ExtendedDecimal divisor,
BigInteger desiredExponent,
Rounding rounding) {
      if (divisor == null) {
        throw new ArgumentNullException("divisor");
      }
      if (desiredExponent == null) {
        throw new ArgumentNullException("desiredExponent");
      }
      return new ExtendedDecimal(
this.Ed.DivideToExponent(
divisor.Ed,
desiredExponent.Ei,
ExtendedDecimal.ToERounding(rounding)));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Abs(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Abs(PrecisionContext context) {
      try {
        return new ExtendedDecimal(this.Ed.Abs(context == null ? null :
                context.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Negate(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Negate(PrecisionContext context) {
      try {
        if (context == null) {
          throw new ArgumentNullException("context");
        }
        return new ExtendedDecimal(this.Ed.Negate(context == null ? null :
          context.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Add(PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Add(ExtendedDecimal otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedDecimal(this.Ed.Add(otherValue.Ed));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Subtract(PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Subtract(ExtendedDecimal otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedDecimal(this.Ed.Subtract(otherValue.Ed));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Subtract(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Subtract(
ExtendedDecimal otherValue,
PrecisionContext ctx) {
      try {
        if (otherValue == null) {
          throw new ArgumentNullException("otherValue");
        }
        return new ExtendedDecimal(
        this.Ed.Subtract(
        otherValue.Ed,
        ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Multiply(PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Multiply(ExtendedDecimal otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedDecimal(this.Ed.Multiply(otherValue.Ed));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MultiplyAndAdd(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal MultiplyAndAdd(
ExtendedDecimal multiplicand,
ExtendedDecimal augend) {
      if (multiplicand == null) {
        throw new ArgumentNullException("multiplicand");
      }
      if (augend == null) {
        throw new ArgumentNullException("augend");
      }
      return new ExtendedDecimal(
      this.Ed.MultiplyAndAdd(
      multiplicand.Ed,
      augend.Ed));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToIntegerNaturalScale(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal DivideToIntegerNaturalScale(
ExtendedDecimal divisor,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }
        return new ExtendedDecimal(
   this.Ed.DivideToIntegerNaturalScale(
   divisor.Ed,
   ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideToIntegerZeroScale(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal DivideToIntegerZeroScale(
ExtendedDecimal divisor,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }
        return new ExtendedDecimal(
this.Ed.DivideToIntegerZeroScale(
divisor.Ed,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Remainder(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Remainder(
ExtendedDecimal divisor,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }
        return new ExtendedDecimal(
      this.Ed.Remainder(
      divisor.Ed,
      ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RemainderNear(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal RemainderNear(
ExtendedDecimal divisor,
PrecisionContext ctx) {
      try {
        if (divisor == null) {
          throw new ArgumentNullException("divisor");
        }
        return new ExtendedDecimal(
   this.Ed.RemainderNear(
   divisor.Ed,
   ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.NextMinus(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal NextMinus(PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.Ed.NextMinus(ctx == null ? null :
              ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.NextPlus(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal NextPlus(PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.Ed.NextPlus(ctx == null ? null :
             ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.NextToward(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal NextToward(
ExtendedDecimal otherValue,
PrecisionContext ctx) {
      try {
        if (otherValue == null) {
          throw new ArgumentNullException("otherValue");
        }
        return new ExtendedDecimal(
   this.Ed.NextToward(
   otherValue.Ed,
   ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Max(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal Max(
ExtendedDecimal first,
ExtendedDecimal second,
PrecisionContext ctx) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedDecimal(
EDecimal.Max(
first.Ed,
second.Ed,
ctx == null ? null : ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Min(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal Min(
ExtendedDecimal first,
ExtendedDecimal second,
PrecisionContext ctx) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedDecimal(
EDecimal.Min(
first.Ed,
second.Ed,
ctx == null ? null : ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MaxMagnitude(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal MaxMagnitude(
ExtendedDecimal first,
ExtendedDecimal second,
PrecisionContext ctx) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedDecimal(
EDecimal.MaxMagnitude(
first.Ed,
second.Ed,
ctx == null ? null : ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MinMagnitude(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal MinMagnitude(
ExtendedDecimal first,
ExtendedDecimal second,
PrecisionContext ctx) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedDecimal(
EDecimal.MinMagnitude(
first.Ed,
second.Ed,
ctx == null ? null : ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Max(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal Max(
ExtendedDecimal first,
ExtendedDecimal second) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedDecimal(EDecimal.Max(first.Ed, second.Ed));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Min(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal Min(
ExtendedDecimal first,
ExtendedDecimal second) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedDecimal(EDecimal.Min(first.Ed, second.Ed));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MaxMagnitude(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal MaxMagnitude(
ExtendedDecimal first,
ExtendedDecimal second) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedDecimal(EDecimal.MaxMagnitude(first.Ed, second.Ed));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MinMagnitude(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal MinMagnitude(
ExtendedDecimal first,
ExtendedDecimal second) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }
      return new ExtendedDecimal(EDecimal.MinMagnitude(first.Ed, second.Ed));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.CompareTo(PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public int CompareTo(ExtendedDecimal other) {
      if (other == null) {
        throw new ArgumentNullException("other");
      }
      return this.Ed.CompareTo(other.Ed);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.CompareToWithContext(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal CompareToWithContext(
ExtendedDecimal other,
PrecisionContext ctx) {
      try {
        if (other == null) {
          throw new ArgumentNullException("other");
        }
        return new ExtendedDecimal(
        this.Ed.CompareToWithContext(
        other.Ed,
        ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.CompareToSignal(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal CompareToSignal(
ExtendedDecimal other,
PrecisionContext ctx) {
      try {
        if (other == null) {
          throw new ArgumentNullException("other");
        }
        return new ExtendedDecimal(
   this.Ed.CompareToSignal(
   other.Ed,
   ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Add(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Add(
ExtendedDecimal otherValue,
PrecisionContext ctx) {
      try {
        if (otherValue == null) {
          throw new ArgumentNullException("otherValue");
        }
        return new ExtendedDecimal(
   this.Ed.Add(
   otherValue.Ed,
   ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Quantize(PeterO.BigInteger,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Quantize(
BigInteger desiredExponent,
PrecisionContext ctx) {
      try {
        if (desiredExponent == null) {
          throw new ArgumentNullException("desiredExponent");
        }
        return new ExtendedDecimal(
      this.Ed.Quantize(
      desiredExponent.Ei,
      ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Quantize(System.Int32,PeterO.Rounding)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Quantize(
int desiredExponentSmall,
Rounding rounding) {
      return new ExtendedDecimal(
this.Ed.Quantize(
desiredExponentSmall,
ExtendedDecimal.ToERounding(rounding)));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Quantize(System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Quantize(
int desiredExponentSmall,
PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(
        this.Ed.Quantize(
        desiredExponentSmall,
        ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Quantize(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Quantize(
ExtendedDecimal otherValue,
PrecisionContext ctx) {
      try {
        if (otherValue == null) {
          throw new ArgumentNullException("otherValue");
        }
        return new ExtendedDecimal(
        this.Ed.Quantize(
        otherValue.Ed,
        ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToIntegralExact(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal RoundToIntegralExact(PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.Ed.RoundToIntegralExact(ctx == null ?
               null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToIntegralNoRoundedFlag(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal RoundToIntegralNoRoundedFlag(PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.Ed.RoundToIntegralNoRoundedFlag(ctx ==
                null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToExponentExact(PeterO.BigInteger,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal RoundToExponentExact(
BigInteger exponent,
PrecisionContext ctx) {
      try {
        if (exponent == null) {
          throw new ArgumentNullException("exponent");
        }
        return new ExtendedDecimal(
this.Ed.RoundToExponentExact(
exponent.Ei,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToExponent(PeterO.BigInteger,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal RoundToExponent(
BigInteger exponent,
PrecisionContext ctx) {
      try {
        if (exponent == null) {
          throw new ArgumentNullException("exponent");
        }
        return new ExtendedDecimal(
      this.Ed.RoundToExponent(
      exponent.Ei,
      ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToExponentExact(System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal RoundToExponentExact(
int exponentSmall,
PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(
this.Ed.RoundToExponentExact(
exponentSmall,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToExponent(System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal RoundToExponent(
int exponentSmall,
PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(
        this.Ed.RoundToExponent(
        exponentSmall,
        ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Multiply(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Multiply(ExtendedDecimal op, PrecisionContext ctx) {
      try {
        if (op == null) {
          throw new ArgumentNullException("op");
        }
        return new ExtendedDecimal(
this.Ed.Multiply(
op.Ed,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MultiplyAndAdd(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal MultiplyAndAdd(
ExtendedDecimal op,
ExtendedDecimal augend,
PrecisionContext ctx) {
      try {
        if (op == null) {
          throw new ArgumentNullException("op");
        }
        if (augend == null) {
          throw new ArgumentNullException("augend");
        }
        return new ExtendedDecimal(
this.Ed.MultiplyAndAdd(
op.Ed,
augend.Ed,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MultiplyAndSubtract(PeterO.ExtendedDecimal,PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal MultiplyAndSubtract(
ExtendedDecimal op,
ExtendedDecimal subtrahend,
PrecisionContext ctx) {
      try {
        if (op == null) {
          throw new ArgumentNullException("op");
        }
        if (subtrahend == null) {
          throw new ArgumentNullException("subtrahend");
        }
        return new ExtendedDecimal(
     this.Ed.MultiplyAndSubtract(
     op.Ed,
     subtrahend.Ed,
     ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToPrecision(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal RoundToPrecision(PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.Ed.RoundToPrecision(ctx == null ? null :
          ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Plus(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Plus(PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.Ed.Plus(ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.RoundToBinaryPrecision(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal RoundToBinaryPrecision(PrecisionContext ctx) {
      if (ctx == null) {
        return this;
      }
      PrecisionContext ctx2 = ctx.Copy().WithPrecisionInBits(true);
      ExtendedDecimal ret = this.RoundToPrecision(ctx2);
      if (ctx2.HasFlags) {
        ctx.Flags = ctx2.Flags;
      }
      return ret;
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.SquareRoot(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal SquareRoot(PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.Ed.SquareRoot(ctx == null ? null :
               ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Exp(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Exp(PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.Ed.Exp(ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Log(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Log(PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.Ed.Log(ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Log10(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Log10(PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.Ed.Log10(ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Pow(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Pow(ExtendedDecimal exponent, PrecisionContext ctx) {
      try {
        if (exponent == null) {
          throw new ArgumentNullException("exponent");
        }
        return new ExtendedDecimal(
this.Ed.Pow(
exponent.Ed,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Pow(System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Pow(int exponentSmall, PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(
   this.Ed.Pow(
   exponentSmall,
   ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Pow(System.Int32)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Pow(int exponentSmall) {
      return new ExtendedDecimal(this.Ed.Pow(exponentSmall));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.PI(PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedDecimal PI(PrecisionContext ctx) {
      return new ExtendedDecimal(EDecimal.PI(ctx == null ? null : ctx.Ec));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointLeft(System.Int32)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal MovePointLeft(int places) {
      return new ExtendedDecimal(this.Ed.MovePointLeft(places));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointLeft(System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal MovePointLeft(int places, PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(
      this.Ed.MovePointLeft(
      places,
      ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointLeft(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal MovePointLeft(BigInteger bigPlaces) {
      if (bigPlaces == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedDecimal(this.Ed.MovePointLeft(bigPlaces.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointLeft(PeterO.BigInteger,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal MovePointLeft(
BigInteger bigPlaces,
PrecisionContext ctx) {
      try {
        if (bigPlaces == null) {
          throw new ArgumentNullException("bigPlaces");
        }
        return new ExtendedDecimal(
this.Ed.MovePointLeft(
bigPlaces.Ei,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointRight(System.Int32)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal MovePointRight(int places) {
      return new ExtendedDecimal(this.Ed.MovePointRight(places));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointRight(System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal MovePointRight(int places, PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(
       this.Ed.MovePointRight(
       places,
       ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointRight(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal MovePointRight(BigInteger bigPlaces) {
      if (bigPlaces == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedDecimal(this.Ed.MovePointRight(bigPlaces.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.MovePointRight(PeterO.BigInteger,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal MovePointRight(
BigInteger bigPlaces,
PrecisionContext ctx) {
      try {
        if (bigPlaces == null) {
          throw new ArgumentNullException("bigPlaces");
        }

        return new ExtendedDecimal(
      this.Ed.MovePointRight(
      bigPlaces.Ei,
      ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ScaleByPowerOfTen(System.Int32)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal ScaleByPowerOfTen(int places) {
      return new ExtendedDecimal(this.Ed.ScaleByPowerOfTen(places));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ScaleByPowerOfTen(System.Int32,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal ScaleByPowerOfTen(int places, PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(
   this.Ed.ScaleByPowerOfTen(
   places,
   ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ScaleByPowerOfTen(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal ScaleByPowerOfTen(BigInteger bigPlaces) {
      if (bigPlaces == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedDecimal(this.Ed.ScaleByPowerOfTen(bigPlaces.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.ScaleByPowerOfTen(PeterO.BigInteger,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal ScaleByPowerOfTen(
BigInteger bigPlaces,
PrecisionContext ctx) {
      try {
        if (bigPlaces == null) {
          throw new ArgumentNullException("bigPlaces");
        }

        return new ExtendedDecimal(
this.Ed.ScaleByPowerOfTen(
bigPlaces.Ei,
ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Precision"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public BigInteger Precision() {
      return new BigInteger(this.Ed.Precision());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.Ulp"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal Ulp() {
      return new ExtendedDecimal(this.Ed.Ulp());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideAndRemainderNaturalScale(PeterO.ExtendedDecimal)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal[] DivideAndRemainderNaturalScale(ExtendedDecimal
      divisor) {
      EDecimal[] edec = this.Ed.DivideAndRemainderNaturalScale(divisor ==
        null ? null : divisor.Ed);
      return new ExtendedDecimal[] {
        new ExtendedDecimal(edec[0]), new ExtendedDecimal(edec[1])
      };
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedDecimal.DivideAndRemainderNaturalScale(PeterO.ExtendedDecimal,PeterO.PrecisionContext)"]/*'/>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public ExtendedDecimal[] DivideAndRemainderNaturalScale(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      try {
        EDecimal[] edec = this.Ed.DivideAndRemainderNaturalScale(
divisor == null ? null : divisor.Ed,
ctx == null ? null : ctx.Ec);
        return new ExtendedDecimal[] {
        new ExtendedDecimal(edec[0]), new ExtendedDecimal(edec[1])
      };
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }
  }
}
