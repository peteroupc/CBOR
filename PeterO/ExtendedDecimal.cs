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
    /// path='docs/doc[@name="T:PeterO.Numbers.EDecimal"]'/>
  public sealed class ExtendedDecimal : IComparable<ExtendedDecimal>,
  IEquatable<ExtendedDecimal> {
    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EDecimal.Exponent"]'/>
    public BigInteger Exponent {
      get {
        return new BigInteger(this.ed.Exponent);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EDecimal.UnsignedMantissa"]'/>
    public BigInteger UnsignedMantissa {
      get {
        return new BigInteger(this.ed.UnsignedMantissa);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EDecimal.Mantissa"]'/>
    public BigInteger Mantissa {
      get {
        return new BigInteger(this.ed.Mantissa);
      }
    }

    #region Equals and GetHashCode implementation

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Equals(PeterO.Numbers.EDecimal)"]'/>
    public bool Equals(ExtendedDecimal other) {
      return (other == null) ? (false) : (this.ed.Equals(other.ed));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Equals(System.Object)"]'/>
    public override bool Equals(object obj) {
      var bi = obj as ExtendedDecimal;
      return (bi == null) ? (false) : (this.ed.Equals(bi.ed));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.GetHashCode"]'/>
    public override int GetHashCode() {
      return this.ed.GetHashCode();
    }
    #endregion

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Create(System.Int32,System.Int32)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Create(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.CreateNaN(PeterO.Numbers.EInteger)"]'/>
    public static ExtendedDecimal CreateNaN(BigInteger diag) {
      if ((diag) == null) {
        throw new ArgumentNullException("diag");
      }
      return new ExtendedDecimal(EDecimal.CreateNaN(diag.ei));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.CreateNaN(PeterO.Numbers.EInteger,System.Boolean,System.Boolean,PeterO.Numbers.EContext)"]'/>
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
    return new ExtendedDecimal(EDecimal.CreateNaN(diag.ei, signaling,
        negative,
        ctx.ec));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.FromString(System.String)"]'/>
    public static ExtendedDecimal FromString(string str) {
      return new ExtendedDecimal(EDecimal.FromString(str));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.FromString(System.String,PeterO.Numbers.EContext)"]'/>
    public static ExtendedDecimal FromString(string str, PrecisionContext ctx) {
try {
      return new ExtendedDecimal(EDecimal.FromString(str, ctx == null ? null :
        ctx.ec));
    } catch (ETrapException ex) {
 throw TrapException.Create(ex);
}
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.FromString(System.String,System.Int32,System.Int32)"]'/>
    public static ExtendedDecimal FromString(string str,
      int offset,
      int length) {
      return new ExtendedDecimal(EDecimal.FromString(str, offset, length));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.FromString(System.String,System.Int32,System.Int32,PeterO.Numbers.EContext)"]'/>
    public static ExtendedDecimal FromString(string str,
      int offset,
      int length,
      PrecisionContext ctx) {
try {
      return new ExtendedDecimal(EDecimal.FromString(str, offset, length,
        ctx == null ? null : ctx.ec));
    } catch (ETrapException ex) {
 throw TrapException.Create(ex);
}
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.CompareToBinary(PeterO.Numbers.EFloat)"]'/>
    public int CompareToBinary(ExtendedFloat other) {
      return ((other) == null) ? (1) : (this.ed.CompareToBinary(other.ef));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.ToBigInteger"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.ToBigIntegerExact"]'/>
    public BigInteger ToBigIntegerExact() {
      return new BigInteger(this.ed.ToBigIntegerExact());
    }

    private static readonly BigInteger valueOneShift62 = BigInteger.One << 62;

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.ToExtendedFloat"]'/>
    public ExtendedFloat ToExtendedFloat() {
      return new ExtendedFloat(this.ed.ToExtendedFloat());
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.ToSingle"]'/>
    public float ToSingle() {
      return this.ed.ToSingle();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.ToDouble"]'/>
    public double ToDouble() {
      return this.ed.ToDouble();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.FromSingle(System.Single)"]'/>
    public static ExtendedDecimal FromSingle(float flt) {
      return new ExtendedDecimal(EDecimal.FromSingle(flt));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.FromBigInteger(PeterO.Numbers.EInteger)"]'/>
    public static ExtendedDecimal FromBigInteger(BigInteger bigint) {
      if ((bigint) == null) {
        throw new ArgumentNullException("bigint");
      }
      return new ExtendedDecimal(EDecimal.FromBigInteger(bigint.ei));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.FromInt64(System.Int64)"]'/>
    public static ExtendedDecimal FromInt64(long valueSmall) {
      return new ExtendedDecimal(EDecimal.FromInt64(valueSmall));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.FromInt32(System.Int32)"]'/>
    public static ExtendedDecimal FromInt32(int valueSmaller) {
      return new ExtendedDecimal(EDecimal.FromInt32(valueSmaller));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.FromDouble(System.Double)"]'/>
    public static ExtendedDecimal FromDouble(double dbl) {
      return new ExtendedDecimal(EDecimal.FromDouble(dbl));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.FromExtendedFloat(PeterO.Numbers.EFloat)"]'/>
    public static ExtendedDecimal FromExtendedFloat(ExtendedFloat bigfloat) {
      if ((bigfloat) == null) {
        throw new ArgumentNullException("bigfloat");
      }
      return new ExtendedDecimal(EDecimal.FromExtendedFloat(bigfloat.ef));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.ToString"]'/>
    public override string ToString() {
      return this.ed.ToString();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.ToEngineeringString"]'/>
    public string ToEngineeringString() {
      return this.ed.ToEngineeringString();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.ToPlainString"]'/>
    public string ToPlainString() {
      return this.ed.ToPlainString();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EDecimal.One"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    public static readonly ExtendedDecimal One =
      ExtendedDecimal.Create(BigInteger.One, BigInteger.Zero);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EDecimal.Zero"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    public static readonly ExtendedDecimal Zero =
      ExtendedDecimal.Create(BigInteger.Zero, BigInteger.Zero);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EDecimal.NegativeZero"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    public static readonly ExtendedDecimal NegativeZero =
      new ExtendedDecimal(EDecimal.NegativeZero);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EDecimal.Ten"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif

    public static readonly ExtendedDecimal Ten =
      ExtendedDecimal.Create((BigInteger)10, BigInteger.Zero);

    //----------------------------------------------------------------

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EDecimal.NaN"]'/>
    public static readonly ExtendedDecimal NaN =
      new ExtendedDecimal(EDecimal.NaN);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EDecimal.SignalingNaN"]'/>
    public static readonly ExtendedDecimal SignalingNaN =
      new ExtendedDecimal(EDecimal.SignalingNaN);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EDecimal.PositiveInfinity"]'/>
    public static readonly ExtendedDecimal PositiveInfinity =
      new ExtendedDecimal(EDecimal.PositiveInfinity);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EDecimal.NegativeInfinity"]'/>
    public static readonly ExtendedDecimal NegativeInfinity =
      new ExtendedDecimal(EDecimal.NegativeInfinity);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.IsNegativeInfinity"]'/>
    public bool IsNegativeInfinity() {
      return this.ed.IsNegativeInfinity();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.IsPositiveInfinity"]'/>
    public bool IsPositiveInfinity() {
      return this.ed.IsPositiveInfinity();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.IsNaN"]'/>
    public bool IsNaN() {
      return this.ed.IsNaN();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.IsInfinity"]'/>
    public bool IsInfinity() {
      return this.ed.IsInfinity();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EDecimal.IsFinite"]'/>
    public bool IsFinite {
      get {
        return this.ed.IsFinite;
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EDecimal.IsNegative"]'/>
    public bool IsNegative {
      get {
        return this.ed.IsNegative;
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.IsQuietNaN"]'/>
    public bool IsQuietNaN() {
      return this.ed.IsQuietNaN();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.IsSignalingNaN"]'/>
    public bool IsSignalingNaN() {
      return this.ed.IsSignalingNaN();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EDecimal.Sign"]'/>
    public int Sign {
      get {
        return this.ed.Sign;
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EDecimal.IsZero"]'/>
    public bool IsZero {
      get {
        return this.ed.IsZero;
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Abs"]'/>
    public ExtendedDecimal Abs() {
      return new ExtendedDecimal(this.ed.Abs());
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Negate"]'/>
    public ExtendedDecimal Negate() {
      return new ExtendedDecimal(this.ed.Negate());
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Divide(PeterO.Numbers.EDecimal)"]'/>
    public ExtendedDecimal Divide(ExtendedDecimal divisor) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedDecimal(this.ed.Divide(divisor.ed));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.DivideToSameExponent(PeterO.Numbers.EDecimal,PeterO.Numbers.ERounding)"]'/>
    public ExtendedDecimal DivideToSameExponent(ExtendedDecimal divisor,
      Rounding rounding) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedDecimal(this.ed.DivideToSameExponent(divisor.ed,
        ExtendedDecimal.ToERounding(rounding)));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.DivideToIntegerNaturalScale(PeterO.Numbers.EDecimal)"]'/>
    public ExtendedDecimal DivideToIntegerNaturalScale(ExtendedDecimal
                    divisor) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
   return new
        ExtendedDecimal(this.ed.DivideToIntegerNaturalScale(divisor.ed));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Reduce(PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Reduce(PrecisionContext ctx) {
      try {
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
        return new ExtendedDecimal(this.ed.Reduce(ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.RemainderNaturalScale(PeterO.Numbers.EDecimal)"]'/>
    public ExtendedDecimal RemainderNaturalScale(ExtendedDecimal divisor) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedDecimal(this.ed.RemainderNaturalScale(divisor.ed));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.RemainderNaturalScale(PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal RemainderNaturalScale(ExtendedDecimal divisor,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
        return new ExtendedDecimal(this.ed.RemainderNaturalScale(divisor.ed,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.DivideToExponent(PeterO.Numbers.EDecimal,System.Int64,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal DivideToExponent(ExtendedDecimal divisor,
      long desiredExponentSmall,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
        return new ExtendedDecimal(this.ed.DivideToExponent(divisor.ed,
          desiredExponentSmall, ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Divide(PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Divide(ExtendedDecimal divisor,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }
     return new ExtendedDecimal(this.ed.Divide(divisor.ed, ctx == null ?
          null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.DivideToExponent(PeterO.Numbers.EDecimal,System.Int64,PeterO.Numbers.ERounding)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.DivideToExponent(PeterO.Numbers.EDecimal,PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal DivideToExponent(ExtendedDecimal divisor,
      BigInteger exponent,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }
        if ((exponent) == null) {
          throw new ArgumentNullException("exponent");
        }
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
   return new ExtendedDecimal(this.ed.DivideToExponent(divisor.ed,
          exponent.ei,
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.DivideToExponent(PeterO.Numbers.EDecimal,PeterO.Numbers.EInteger,PeterO.Numbers.ERounding)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Abs(PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Abs(PrecisionContext context) {
      try {
  return new ExtendedDecimal(this.ed.Abs(context == null ? null :
          context.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Negate(PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Negate(PrecisionContext context) {
      try {
        if ((context) == null) {
          throw new ArgumentNullException("context");
        }
        return new ExtendedDecimal(this.ed.Negate(context == null ? null :
          context.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Add(PeterO.Numbers.EDecimal)"]'/>
    public ExtendedDecimal Add(ExtendedDecimal otherValue) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedDecimal(this.ed.Add(otherValue.ed));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Subtract(PeterO.Numbers.EDecimal)"]'/>
    public ExtendedDecimal Subtract(ExtendedDecimal otherValue) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedDecimal(this.ed.Subtract(otherValue.ed));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Subtract(PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Subtract(ExtendedDecimal otherValue,
      PrecisionContext ctx) {
      try {
        if ((otherValue) == null) {
          throw new ArgumentNullException("otherValue");
        }
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
return new ExtendedDecimal(this.ed.Subtract(otherValue.ed, ctx == null ?
          null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Multiply(PeterO.Numbers.EDecimal)"]'/>
    public ExtendedDecimal Multiply(ExtendedDecimal otherValue) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedDecimal(this.ed.Multiply(otherValue.ed));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.MultiplyAndAdd(PeterO.Numbers.EDecimal,PeterO.Numbers.EDecimal)"]'/>
    public ExtendedDecimal MultiplyAndAdd(ExtendedDecimal multiplicand,
      ExtendedDecimal augend) {
      if ((multiplicand) == null) {
        throw new ArgumentNullException("multiplicand");
      }
      if ((augend) == null) {
        throw new ArgumentNullException("augend");
      }
return new ExtendedDecimal(this.ed.MultiplyAndAdd(multiplicand.ed,
        augend.ed));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.DivideToIntegerNaturalScale(PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal DivideToIntegerNaturalScale(ExtendedDecimal divisor,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
     return new
          ExtendedDecimal(this.ed.DivideToIntegerNaturalScale(divisor.ed,
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.DivideToIntegerZeroScale(PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal DivideToIntegerZeroScale(ExtendedDecimal divisor,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
        return new ExtendedDecimal(this.ed.DivideToIntegerZeroScale(divisor.ed,
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Remainder(PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Remainder(ExtendedDecimal divisor,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
  return new ExtendedDecimal(this.ed.Remainder(divisor.ed, ctx == null ?
          null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.RemainderNear(PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal RemainderNear(ExtendedDecimal divisor,
      PrecisionContext ctx) {
      try {
        if ((divisor) == null) {
          throw new ArgumentNullException("divisor");
        }
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
     return new ExtendedDecimal(this.ed.RemainderNear(divisor.ed, ctx ==
          null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.NextMinus(PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal NextMinus(PrecisionContext ctx) {
      try {
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
    return new ExtendedDecimal(this.ed.NextMinus(ctx == null ? null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.NextPlus(PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal NextPlus(PrecisionContext ctx) {
      try {
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
     return new ExtendedDecimal(this.ed.NextPlus(ctx == null ? null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.NextToward(PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal NextToward(ExtendedDecimal otherValue,
      PrecisionContext ctx) {
      try {
        if ((otherValue) == null) {
          throw new ArgumentNullException("otherValue");
        }
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
     return new ExtendedDecimal(this.ed.NextToward(otherValue.ed, ctx ==
          null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Max(PeterO.Numbers.EDecimal,PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Min(PeterO.Numbers.EDecimal,PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.MaxMagnitude(PeterO.Numbers.EDecimal,PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.MinMagnitude(PeterO.Numbers.EDecimal,PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Max(PeterO.Numbers.EDecimal,PeterO.Numbers.EDecimal)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Min(PeterO.Numbers.EDecimal,PeterO.Numbers.EDecimal)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.MaxMagnitude(PeterO.Numbers.EDecimal,PeterO.Numbers.EDecimal)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.MinMagnitude(PeterO.Numbers.EDecimal,PeterO.Numbers.EDecimal)"]'/>
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
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.CompareTo(PeterO.Numbers.EDecimal)"]'/>
    public int CompareTo(ExtendedDecimal other) {
      if ((other) == null) {
        throw new ArgumentNullException("other");
      }
      return this.ed.CompareTo(other.ed);
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.CompareToWithContext(PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal CompareToWithContext(ExtendedDecimal other,
      PrecisionContext ctx) {
      try {
        if ((other) == null) {
          throw new ArgumentNullException("other");
        }
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
return new ExtendedDecimal(this.ed.CompareToWithContext(other.ed, ctx ==
          null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.CompareToSignal(PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal CompareToSignal(ExtendedDecimal other,
      PrecisionContext ctx) {
      try {
        if ((other) == null) {
          throw new ArgumentNullException("other");
        }
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
     return new ExtendedDecimal(this.ed.CompareToSignal(other.ed, ctx ==
          null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Add(PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Add(ExtendedDecimal otherValue,
      PrecisionContext ctx) {
      try {
        if ((otherValue) == null) {
          throw new ArgumentNullException("otherValue");
        }
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
     return new ExtendedDecimal(this.ed.Add(otherValue.ed, ctx == null ?
          null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Quantize(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Quantize(BigInteger desiredExponent,
      PrecisionContext ctx) {
      try {
        if ((desiredExponent) == null) {
          throw new ArgumentNullException("desiredExponent");
        }
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
  return new ExtendedDecimal(this.ed.Quantize(desiredExponent.ei, ctx ==
          null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Quantize(System.Int32,PeterO.Numbers.ERounding)"]'/>
    public ExtendedDecimal Quantize(int desiredExponentSmall,
      Rounding rounding) {
      return new ExtendedDecimal(this.ed.Quantize(desiredExponentSmall,
        ExtendedDecimal.ToERounding(rounding)));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Quantize(System.Int32,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Quantize(int desiredExponentSmall,
      PrecisionContext ctx) {
      try {
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
return new ExtendedDecimal(this.ed.Quantize(desiredExponentSmall, ctx ==
          null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Quantize(PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Quantize(ExtendedDecimal otherValue,
      PrecisionContext ctx) {
      try {
        if ((otherValue) == null) {
          throw new ArgumentNullException("otherValue");
        }
return new ExtendedDecimal(this.ed.Quantize(otherValue.ed, ctx == null ?
          null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.RoundToIntegralExact(PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal RoundToIntegralExact(PrecisionContext ctx) {
      try {
   return new ExtendedDecimal(this.ed.RoundToIntegralExact(ctx == null ?
          null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.RoundToIntegralNoRoundedFlag(PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal RoundToIntegralNoRoundedFlag(PrecisionContext ctx) {
      try {
  return new ExtendedDecimal(this.ed.RoundToIntegralNoRoundedFlag(ctx ==
          null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.RoundToExponentExact(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal RoundToExponentExact(BigInteger exponent,
      PrecisionContext ctx) {
      try {
        if ((exponent) == null) {
          throw new ArgumentNullException("exponent");
        }
        return new ExtendedDecimal(this.ed.RoundToExponentExact(exponent.ei,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.RoundToExponent(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal RoundToExponent(BigInteger exponent,
      PrecisionContext ctx) {
      try {
        if ((exponent) == null) {
          throw new ArgumentNullException("exponent");
        }
  return new ExtendedDecimal(this.ed.RoundToExponent(exponent.ei, ctx ==
          null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.RoundToExponentExact(System.Int32,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal RoundToExponentExact(int exponentSmall,
      PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.ed.RoundToExponentExact(exponentSmall,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.RoundToExponent(System.Int32,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal RoundToExponent(int exponentSmall,
      PrecisionContext ctx) {
      try {
return new ExtendedDecimal(this.ed.RoundToExponent(exponentSmall, ctx ==
          null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Multiply(PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Multiply(ExtendedDecimal op, PrecisionContext ctx) {
      try {
        if ((op) == null) {
          throw new ArgumentNullException("op");
        }
        return new ExtendedDecimal(this.ed.Multiply(op.ed, ctx == null ? null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.MultiplyAndAdd(PeterO.Numbers.EDecimal,PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal MultiplyAndAdd(ExtendedDecimal op,
      ExtendedDecimal augend,
      PrecisionContext ctx) {
      try {
        if ((op) == null) {
          throw new ArgumentNullException("op");
        }
        if ((augend) == null) {
          throw new ArgumentNullException("augend");
        }
        return new ExtendedDecimal(this.ed.MultiplyAndAdd(op.ed, augend.ed,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.MultiplyAndSubtract(PeterO.Numbers.EDecimal,PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal MultiplyAndSubtract(ExtendedDecimal op,
      ExtendedDecimal subtrahend,
      PrecisionContext ctx) {
      try {
        if ((op) == null) {
          throw new ArgumentNullException("op");
        }
        if ((subtrahend) == null) {
          throw new ArgumentNullException("subtrahend");
        }
   return new ExtendedDecimal(this.ed.MultiplyAndSubtract(op.ed,
          subtrahend.ed,
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.RoundToPrecision(PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal RoundToPrecision(PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.ed.RoundToPrecision(ctx == null ? null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Plus(PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Plus(PrecisionContext ctx) {
      try {
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
        return new ExtendedDecimal(this.ed.Plus(ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.RoundToBinaryPrecision(PeterO.Numbers.EContext)"]'/>
    [Obsolete(
      "Instead of this method use RoundToPrecision and pass a " + "precision context with the IsPrecisionInBits property set.")]
    public ExtendedDecimal RoundToBinaryPrecision(PrecisionContext ctx) {
      try {
 return new ExtendedDecimal(this.ed.RoundToBinaryPrecision(ctx == null ?
          null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.SquareRoot(PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal SquareRoot(PrecisionContext ctx) {
      try {
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
   return new ExtendedDecimal(this.ed.SquareRoot(ctx == null ? null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Exp(PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Exp(PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.ed.Exp(ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Log(PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Log(PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.ed.Log(ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Log10(PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Log10(PrecisionContext ctx) {
      try {
        return new ExtendedDecimal(this.ed.Log10(ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Pow(PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Pow(ExtendedDecimal exponent, PrecisionContext ctx) {
      try {
        if ((exponent) == null) {
          throw new ArgumentNullException("exponent");
        }
        return new ExtendedDecimal(this.ed.Pow(exponent.ed, ctx == null ? null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Pow(System.Int32,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal Pow(int exponentSmall, PrecisionContext ctx) {
      try {
     return new ExtendedDecimal(this.ed.Pow(exponentSmall, ctx == null ?
          null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Pow(System.Int32)"]'/>
    public ExtendedDecimal Pow(int exponentSmall) {
      return new ExtendedDecimal(this.ed.Pow(exponentSmall));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.PI(PeterO.Numbers.EContext)"]'/>
    public static ExtendedDecimal PI(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedDecimal(EDecimal.PI(ctx == null ? null : ctx.ec));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.MovePointLeft(System.Int32)"]'/>
    public ExtendedDecimal MovePointLeft(int places) {
      return new ExtendedDecimal(this.ed.MovePointLeft(places));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.MovePointLeft(System.Int32,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal MovePointLeft(int places, PrecisionContext ctx) {
      try {
  return new ExtendedDecimal(this.ed.MovePointLeft(places, ctx == null ?
          null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.MovePointLeft(PeterO.Numbers.EInteger)"]'/>
    public ExtendedDecimal MovePointLeft(BigInteger bigPlaces) {
      if ((bigPlaces) == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedDecimal(this.ed.MovePointLeft(bigPlaces.ei));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.MovePointLeft(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal MovePointLeft(BigInteger bigPlaces,
PrecisionContext ctx) {
      try {
        if ((bigPlaces) == null) {
          throw new ArgumentNullException("bigPlaces");
        }
        return new ExtendedDecimal(this.ed.MovePointLeft(bigPlaces.ei,
          ctx == null ? null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.MovePointRight(System.Int32)"]'/>
    public ExtendedDecimal MovePointRight(int places) {
      return new ExtendedDecimal(this.ed.MovePointRight(places));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.MovePointRight(System.Int32,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal MovePointRight(int places, PrecisionContext ctx) {
      try {
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
 return new ExtendedDecimal(this.ed.MovePointRight(places, ctx == null ?
          null :
          ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.MovePointRight(PeterO.Numbers.EInteger)"]'/>
    public ExtendedDecimal MovePointRight(BigInteger bigPlaces) {
      if ((bigPlaces) == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedDecimal(this.ed.MovePointRight(bigPlaces.ei));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.MovePointRight(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal MovePointRight(BigInteger bigPlaces,
PrecisionContext ctx) {
      try {
        if ((bigPlaces) == null) {
          throw new ArgumentNullException("bigPlaces");
        }
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
  return new ExtendedDecimal(this.ed.MovePointRight(bigPlaces.ei, ctx ==
          null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.ScaleByPowerOfTen(System.Int32)"]'/>
    public ExtendedDecimal ScaleByPowerOfTen(int places) {
      return new ExtendedDecimal(this.ed.ScaleByPowerOfTen(places));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.ScaleByPowerOfTen(System.Int32,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal ScaleByPowerOfTen(int places, PrecisionContext ctx) {
      try {
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
     return new ExtendedDecimal(this.ed.ScaleByPowerOfTen(places, ctx ==
          null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.ScaleByPowerOfTen(PeterO.Numbers.EInteger)"]'/>
    public ExtendedDecimal ScaleByPowerOfTen(BigInteger bigPlaces) {
      if ((bigPlaces) == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedDecimal(this.ed.ScaleByPowerOfTen(bigPlaces.ei));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.ScaleByPowerOfTen(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal ScaleByPowerOfTen(BigInteger bigPlaces,
PrecisionContext ctx) {
      try {
        if ((bigPlaces) == null) {
          throw new ArgumentNullException("bigPlaces");
        }
        if ((ctx) == null) {
          throw new ArgumentNullException("ctx");
        }
        return new ExtendedDecimal(this.ed.ScaleByPowerOfTen(bigPlaces.ei,
          ctx == null ?
          null : ctx.ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Precision"]'/>
    public BigInteger Precision() {
      return new BigInteger(this.ed.Precision());
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.Ulp"]'/>
    public ExtendedDecimal Ulp() {
      return new ExtendedDecimal(this.ed.Ulp());
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.DivideAndRemainderNaturalScale(PeterO.Numbers.EDecimal)"]'/>
    public ExtendedDecimal[] DivideAndRemainderNaturalScale(ExtendedDecimal
      divisor) {
      EDecimal[] edec = this.ed.DivideAndRemainderNaturalScale(divisor ==
        null ? null : divisor.ed);
      return new ExtendedDecimal[] {
        new ExtendedDecimal(edec[0]), new ExtendedDecimal(edec[1])
      };
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EDecimal.DivideAndRemainderNaturalScale(PeterO.Numbers.EDecimal,PeterO.Numbers.EContext)"]'/>
    public ExtendedDecimal[] DivideAndRemainderNaturalScale(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      try {
        EDecimal[] edec = this.ed.DivideAndRemainderNaturalScale(divisor ==
          null ? null : divisor.ed,
          ctx == null ? null : ctx.ec);
        return new ExtendedDecimal[] {
        new ExtendedDecimal(edec[0]), new ExtendedDecimal(edec[1])
      };
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }
  }
}
