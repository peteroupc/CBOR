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
    /// path='docs/doc[@name="T:PeterO.Numbers.EContext"]'/>
  public class PrecisionContext {
    // TODO: Add 'sealed' to PrecisionContext class in version 3

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagClamped"]'/>
    public const int FlagClamped = 32;

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagDivideByZero"]'/>
    public const int FlagDivideByZero = 128;

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagInexact"]'/>
    public const int FlagInexact = 1;

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagInvalid"]'/>
    public const int FlagInvalid = 64;

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagLostDigits"]'/>
    public const int FlagLostDigits = 256;

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagOverflow"]'/>
    public const int FlagOverflow = 16;

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagRounded"]'/>
    public const int FlagRounded = 2;

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagSubnormal"]'/>
    public const int FlagSubnormal = 4;

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagUnderflow"]'/>
    public const int FlagUnderflow = 8;

    internal readonly EContext ec;

    internal PrecisionContext(EContext ec) {
      this.ec = ec;
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Basic"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly PrecisionContext Basic =
      new PrecisionContext(EContext.Basic);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.BigDecimalJava"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly PrecisionContext BigDecimalJava =
      new PrecisionContext(EContext.BigDecimalJava);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Binary128"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly PrecisionContext Binary128 =
      new PrecisionContext(EContext.Binary128);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Binary16"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly PrecisionContext Binary16 =
      new PrecisionContext(EContext.Binary16);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Binary32"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly PrecisionContext Binary32 =
      new PrecisionContext(EContext.Binary32);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Binary64"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly PrecisionContext Binary64 =
      new PrecisionContext(EContext.Binary64);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.CliDecimal"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly PrecisionContext CliDecimal =
      new PrecisionContext(EContext.CliDecimal);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Decimal128"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly PrecisionContext Decimal128 =
      new PrecisionContext(EContext.Decimal128);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Decimal32"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly PrecisionContext Decimal32 =
      new PrecisionContext(EContext.Decimal32);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Decimal64"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly PrecisionContext Decimal64 =
      new PrecisionContext(EContext.Decimal64);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.JavaBigDecimal"]'/>
    [Obsolete("This context had the wrong settings in previous " +
              "versions. Use BigDecimalJava instead.")]
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly PrecisionContext JavaBigDecimal = BigDecimalJava;

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Unlimited"]'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly PrecisionContext Unlimited =
      PrecisionContext.ForPrecision(0);

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.#ctor(System.Int32,PeterO.Numbers.ERounding,System.Int32,System.Int32,System.Boolean)"]'/>
    public PrecisionContext(
int precision,
Rounding rounding,
int exponentMinSmall,
int exponentMaxSmall,
bool clampNormalExponents) {
      this.ec = new EContext(precision,
        ExtendedDecimal.ToERounding(rounding),
        exponentMinSmall, exponentMaxSmall, clampNormalExponents);
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.AdjustExponent"]'/>
    public  bool AdjustExponent {
get {
return this.ec.AdjustExponent;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.ClampNormalExponents"]'/>
    public  bool ClampNormalExponents {
get {
return this.ec.ClampNormalExponents;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.EMax"]'/>
    public  BigInteger EMax {
get {
return new BigInteger(this.ec.EMax);
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.EMin"]'/>
    public  BigInteger EMin {
get {
return new BigInteger(this.ec.EMin);
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.Flags"]'/>
    public  int Flags {
get {
return this.ec.Flags;
}
    set {
        this.ec.Flags = value;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.HasExponentRange"]'/>
    public  bool HasExponentRange {
get {
return this.ec.HasExponentRange;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.HasFlags"]'/>
    public  bool HasFlags {
get {
return this.ec.HasFlags;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.HasMaxPrecision"]'/>
    public  bool HasMaxPrecision {
get {
return this.ec.HasMaxPrecision;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.IsPrecisionInBits"]'/>
    public  bool IsPrecisionInBits {
get {
return this.ec.IsPrecisionInBits;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.IsSimplified"]'/>
    public  bool IsSimplified {
get {
return this.ec.IsSimplified;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.Precision"]'/>
    public  BigInteger Precision {
get {
return new BigInteger(this.ec.Precision);
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.Rounding"]'/>
    public  Rounding Rounding {
get {
        return ExtendedDecimal.ToRounding(this.ec.Rounding);
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.Traps"]'/>
    public  int Traps {
get {
return this.ec.Traps;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.ForPrecision(System.Int32)"]'/>
    public static PrecisionContext ForPrecision(int precision) {
return new PrecisionContext(EContext.ForPrecision(precision));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.ForPrecisionAndRounding(System.Int32,PeterO.Numbers.ERounding)"]'/>
    public static PrecisionContext ForPrecisionAndRounding(int precision,
      Rounding rounding) {
return new PrecisionContext(EContext.ForPrecisionAndRounding(precision,
  ExtendedDecimal.ToERounding(rounding)));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.ForRounding(PeterO.Numbers.ERounding)"]'/>
    public static PrecisionContext ForRounding(Rounding rounding) {
return new
  PrecisionContext(EContext.ForRounding(ExtendedDecimal.ToERounding(rounding)));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.Copy"]'/>
    public PrecisionContext Copy() {
return new PrecisionContext(this.ec.Copy());
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.ExponentWithinRange(PeterO.Numbers.EInteger)"]'/>
    public bool ExponentWithinRange(BigInteger exponent) {
  if ((exponent) == null) {
  throw new ArgumentNullException("exponent");
}
return this.ec.ExponentWithinRange(exponent.ei);
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.ToString"]'/>
    public override string ToString() {
return this.ec.ToString();
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithAdjustExponent(System.Boolean)"]'/>
    public PrecisionContext WithAdjustExponent(bool adjustExponent) {
return new PrecisionContext(this.ec.WithAdjustExponent(adjustExponent));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithBigExponentRange(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'/>
    public PrecisionContext WithBigExponentRange(BigInteger exponentMin,
      BigInteger exponentMax) {
  if ((exponentMin) == null) {
  throw new ArgumentNullException("exponentMin");
}
  if ((exponentMax) == null) {
  throw new ArgumentNullException("exponentMax");
}
return new PrecisionContext(this.ec.WithBigExponentRange(exponentMin.ei,
  exponentMax.ei));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithBigPrecision(PeterO.Numbers.EInteger)"]'/>
    public PrecisionContext WithBigPrecision(BigInteger bigintPrecision) {
  if ((bigintPrecision) == null) {
  throw new ArgumentNullException("bigintPrecision");
}
return new PrecisionContext(this.ec.WithBigPrecision(bigintPrecision.ei));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithBlankFlags"]'/>
    public PrecisionContext WithBlankFlags() {
return new PrecisionContext(this.ec.WithBlankFlags());
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithExponentClamp(System.Boolean)"]'/>
    public PrecisionContext WithExponentClamp(bool clamp) {
return new PrecisionContext(this.ec.WithExponentClamp(clamp));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithExponentRange(System.Int32,System.Int32)"]'/>
    public PrecisionContext WithExponentRange(int exponentMinSmall,
      int exponentMaxSmall) {
return new PrecisionContext(this.ec.WithExponentRange(exponentMinSmall,
  exponentMaxSmall));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithNoFlags"]'/>
    public PrecisionContext WithNoFlags() {
return new PrecisionContext(this.ec.WithNoFlags());
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithPrecision(System.Int32)"]'/>
    public PrecisionContext WithPrecision(int precision) {
return new PrecisionContext(this.ec.WithPrecision(precision));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithPrecisionInBits(System.Boolean)"]'/>
    public PrecisionContext WithPrecisionInBits(bool isPrecisionBits) {
return new PrecisionContext(this.ec.WithPrecisionInBits(isPrecisionBits));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithRounding(PeterO.Numbers.ERounding)"]'/>
    public PrecisionContext WithRounding(Rounding rounding) {
return new
  PrecisionContext(this.ec.WithRounding(ExtendedDecimal.ToERounding(rounding)));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithSimplified(System.Boolean)"]'/>
    public PrecisionContext WithSimplified(bool simplified) {
return new PrecisionContext(this.ec.WithSimplified(simplified));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithTraps(System.Int32)"]'/>
    public PrecisionContext WithTraps(int traps) {
return new PrecisionContext(this.ec.WithTraps(traps));
}

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithUnlimitedExponents"]'/>
    public PrecisionContext WithUnlimitedExponents() {
return new PrecisionContext(this.ec.WithUnlimitedExponents());
}
  }
}
