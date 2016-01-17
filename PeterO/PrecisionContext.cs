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
    /// path='docs/doc[@name="T:PeterO.PrecisionContext"]/*'/>
  public class PrecisionContext {
    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.FlagClamped"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public const int FlagClamped = 32;

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.FlagDivideByZero"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public const int FlagDivideByZero = 128;

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.FlagInexact"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public const int FlagInexact = 1;

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.FlagInvalid"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public const int FlagInvalid = 64;

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.FlagLostDigits"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public const int FlagLostDigits = 256;

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.FlagOverflow"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public const int FlagOverflow = 16;

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.FlagRounded"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public const int FlagRounded = 2;

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.FlagSubnormal"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public const int FlagSubnormal = 4;

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.FlagUnderflow"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public const int FlagUnderflow = 8;

    private readonly EContext ec;

    internal PrecisionContext(EContext ec) {
      this.ec = ec;
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.Basic"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public static readonly PrecisionContext Basic =
      new PrecisionContext(EContext.Basic);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.BigDecimalJava"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public static readonly PrecisionContext BigDecimalJava =
      new PrecisionContext(EContext.BigDecimalJava);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.Binary128"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public static readonly PrecisionContext Binary128 =
      new PrecisionContext(EContext.Binary128);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.Binary16"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public static readonly PrecisionContext Binary16 =
      new PrecisionContext(EContext.Binary16);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.Binary32"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public static readonly PrecisionContext Binary32 =
      new PrecisionContext(EContext.Binary32);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.Binary64"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public static readonly PrecisionContext Binary64 =
      new PrecisionContext(EContext.Binary64);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.CliDecimal"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public static readonly PrecisionContext CliDecimal =
      new PrecisionContext(EContext.CliDecimal);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.Decimal128"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public static readonly PrecisionContext Decimal128 =
      new PrecisionContext(EContext.Decimal128);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.Decimal32"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public static readonly PrecisionContext Decimal32 =
      new PrecisionContext(EContext.Decimal32);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.Decimal64"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public static readonly PrecisionContext Decimal64 =
      new PrecisionContext(EContext.Decimal64);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.JavaBigDecimal"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public static readonly PrecisionContext JavaBigDecimal = BigDecimalJava;

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.PrecisionContext.Unlimited"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public static readonly PrecisionContext Unlimited =
      PrecisionContext.ForPrecision(0);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.#ctor(System.Int32,PeterO.Rounding,System.Int32,System.Int32,System.Boolean)"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public PrecisionContext(
int precision,
Rounding rounding,
int exponentMinSmall,
int exponentMaxSmall,
bool clampNormalExponents) {
      this.ec = new EContext(
precision,
        ExtendedDecimal.ToERounding(rounding),
        exponentMinSmall,
 exponentMaxSmall,
 clampNormalExponents);
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.PrecisionContext.AdjustExponent"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public bool AdjustExponent {
get {
return this.Ec.AdjustExponent;
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.PrecisionContext.ClampNormalExponents"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public bool ClampNormalExponents {
get {
return this.Ec.ClampNormalExponents;
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.PrecisionContext.EMax"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public BigInteger EMax {
get {
return new BigInteger(this.Ec.EMax);
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.PrecisionContext.EMin"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public BigInteger EMin {
get {
return new BigInteger(this.Ec.EMin);
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.PrecisionContext.Flags"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public int Flags {
get {
return this.Ec.Flags;
}

    set {
        this.Ec.Flags = value;
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.PrecisionContext.HasExponentRange"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public bool HasExponentRange {
get {
return this.Ec.HasExponentRange;
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.PrecisionContext.HasFlags"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public bool HasFlags {
get {
return this.Ec.HasFlags;
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.PrecisionContext.HasMaxPrecision"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public bool HasMaxPrecision {
get {
return this.Ec.HasMaxPrecision;
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.PrecisionContext.IsPrecisionInBits"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public bool IsPrecisionInBits {
get {
return this.Ec.IsPrecisionInBits;
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.PrecisionContext.IsSimplified"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public bool IsSimplified {
get {
return this.Ec.IsSimplified;
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.PrecisionContext.Precision"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public BigInteger Precision {
get {
return new BigInteger(this.Ec.Precision);
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.PrecisionContext.Rounding"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public Rounding Rounding {
get {
        return ExtendedDecimal.ToRounding(this.Ec.Rounding);
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.PrecisionContext.Traps"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public int Traps {
get {
return this.Ec.Traps;
} }

    internal EContext Ec {
      get {
        return this.ec;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.ForPrecision(System.Int32)"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public static PrecisionContext ForPrecision(int precision) {
return new PrecisionContext(EContext.ForPrecision(precision));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.ForPrecisionAndRounding(System.Int32,PeterO.Rounding)"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public static PrecisionContext ForPrecisionAndRounding(
int precision,
Rounding rounding) {
return new PrecisionContext(
EContext.ForPrecisionAndRounding(
precision,
ExtendedDecimal.ToERounding(rounding)));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.ForRounding(PeterO.Rounding)"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public static PrecisionContext ForRounding(Rounding rounding) {
return new
  PrecisionContext(EContext.ForRounding(ExtendedDecimal.ToERounding(rounding)));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.Copy"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public PrecisionContext Copy() {
return new PrecisionContext(this.Ec.Copy());
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.ExponentWithinRange(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public bool ExponentWithinRange(BigInteger exponent) {
  if (exponent == null) {
  throw new ArgumentNullException("exponent");
}
return this.Ec.ExponentWithinRange(exponent.Ei);
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.ToString"]/*'/>
    public override string ToString() {
return this.Ec.ToString();
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.WithAdjustExponent(System.Boolean)"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public PrecisionContext WithAdjustExponent(bool adjustExponent) {
return new PrecisionContext(this.Ec.WithAdjustExponent(adjustExponent));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.WithBigExponentRange(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public PrecisionContext WithBigExponentRange(
BigInteger exponentMin,
BigInteger exponentMax) {
  if (exponentMin == null) {
  throw new ArgumentNullException("exponentMin");
}
  if (exponentMax == null) {
  throw new ArgumentNullException("exponentMax");
}
return new PrecisionContext(
this.Ec.WithBigExponentRange(
exponentMin.Ei,
exponentMax.Ei));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.WithBigPrecision(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public PrecisionContext WithBigPrecision(BigInteger bigintPrecision) {
  if (bigintPrecision == null) {
  throw new ArgumentNullException("bigintPrecision");
}
return new PrecisionContext(this.Ec.WithBigPrecision(bigintPrecision.Ei));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.WithBlankFlags"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public PrecisionContext WithBlankFlags() {
return new PrecisionContext(this.Ec.WithBlankFlags());
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.WithExponentClamp(System.Boolean)"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public PrecisionContext WithExponentClamp(bool clamp) {
return new PrecisionContext(this.Ec.WithExponentClamp(clamp));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.WithExponentRange(System.Int32,System.Int32)"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public PrecisionContext WithExponentRange(
int exponentMinSmall,
int exponentMaxSmall) {
return new PrecisionContext(
this.Ec.WithExponentRange(
exponentMinSmall,
exponentMaxSmall));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.WithNoFlags"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public PrecisionContext WithNoFlags() {
return new PrecisionContext(this.Ec.WithNoFlags());
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.WithPrecision(System.Int32)"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public PrecisionContext WithPrecision(int precision) {
return new PrecisionContext(this.Ec.WithPrecision(precision));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.WithPrecisionInBits(System.Boolean)"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public PrecisionContext WithPrecisionInBits(bool isPrecisionBits) {
return new PrecisionContext(this.Ec.WithPrecisionInBits(isPrecisionBits));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.WithRounding(PeterO.Rounding)"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public PrecisionContext WithRounding(Rounding rounding) {
return new
  PrecisionContext(this.Ec.WithRounding(ExtendedDecimal.ToERounding(rounding)));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.WithSimplified(System.Boolean)"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public PrecisionContext WithSimplified(bool simplified) {
return new PrecisionContext(this.Ec.WithSimplified(simplified));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.WithTraps(System.Int32)"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public PrecisionContext WithTraps(int traps) {
return new PrecisionContext(this.Ec.WithTraps(traps));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.PrecisionContext.WithUnlimitedExponents"]/*'/>
    [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
   public PrecisionContext WithUnlimitedExponents() {
return new PrecisionContext(this.Ec.WithUnlimitedExponents());
}
  }
}
