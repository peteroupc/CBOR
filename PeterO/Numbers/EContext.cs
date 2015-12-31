/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Numbers {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Numbers.EContext"]/*'/>
  internal sealed class EContext {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagClamped"]/*'/>
    public const int FlagClamped = 32;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagDivideByZero"]/*'/>
    public const int FlagDivideByZero = 128;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagInexact"]/*'/>
    public const int FlagInexact = 1;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagInvalid"]/*'/>
    public const int FlagInvalid = 64;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagLostDigits"]/*'/>
    public const int FlagLostDigits = 256;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagOverflow"]/*'/>
    public const int FlagOverflow = 16;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagRounded"]/*'/>
    public const int FlagRounded = 2;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagSubnormal"]/*'/>
    public const int FlagSubnormal = 4;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.FlagUnderflow"]/*'/>
    public const int FlagUnderflow = 8;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Basic"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly EContext Basic =
      EContext.ForPrecisionAndRounding(9, ERounding.HalfUp);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.BigDecimalJava"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly EContext BigDecimalJava =
      new EContext(0, ERounding.HalfUp, 0, 0, true)
      .WithExponentClamp(true).WithAdjustExponent(false)
      .WithBigExponentRange(
EInteger.Zero - (EInteger)Int32.MaxValue,
EInteger.One + (EInteger)Int32.MaxValue);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Binary128"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly EContext Binary128 =
      EContext.ForPrecisionAndRounding(113, ERounding.HalfEven)
      .WithExponentClamp(true).WithExponentRange(-16382, 16383);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Binary16"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly EContext Binary16 =
      EContext.ForPrecisionAndRounding(11, ERounding.HalfEven)
      .WithExponentClamp(true).WithExponentRange(-14, 15);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Binary32"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly EContext Binary32 =
      EContext.ForPrecisionAndRounding(24, ERounding.HalfEven)
      .WithExponentClamp(true).WithExponentRange(-126, 127);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Binary64"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly EContext Binary64 =
      EContext.ForPrecisionAndRounding(53, ERounding.HalfEven)
      .WithExponentClamp(true).WithExponentRange(-1022, 1023);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.CliDecimal"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly EContext CliDecimal =
      new EContext(96, ERounding.HalfEven, 0, 28, true)
      .WithPrecisionInBits(true);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Decimal128"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly EContext Decimal128 =
      new EContext(34, ERounding.HalfEven, -6143, 6144, true);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Decimal32"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly EContext Decimal32 =
      new EContext(7, ERounding.HalfEven, -95, 96, true);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Decimal64"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly EContext Decimal64 =
      new EContext(16, ERounding.HalfEven, -383, 384, true);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EContext.Unlimited"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly EContext Unlimited =
      EContext.ForPrecision(0);

    private bool adjustExponent;

    private EInteger bigintPrecision;

    private bool clampNormalExponents;
    private EInteger exponentMax;

    private EInteger exponentMin;

    private int flags;

    private bool hasExponentRange;
    private bool hasFlags;

    private bool precisionInBits;

    private ERounding rounding;

    private bool simplified;

    private int traps;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.#ctor(System.Int32,PeterO.Numbers.ERounding,System.Int32,System.Int32,System.Boolean)"]/*'/>
    public EContext(
int precision,
ERounding rounding,
int exponentMinSmall,
int exponentMaxSmall,
bool clampNormalExponents) {
      if (precision < 0) {
        throw new ArgumentException("precision (" + precision +
          ") is less than 0");
      }
      if (exponentMinSmall > exponentMaxSmall) {
        throw new ArgumentException("exponentMinSmall (" + exponentMinSmall +
          ") is more than " + exponentMaxSmall);
      }
      this.bigintPrecision = precision == 0 ? EInteger.Zero :
        (EInteger)precision;
      this.rounding = rounding;
      this.clampNormalExponents = clampNormalExponents;
      this.hasExponentRange = true;
      this.adjustExponent = true;
      this.exponentMax = exponentMaxSmall == 0 ? EInteger.Zero :
        (EInteger)exponentMaxSmall;
      this.exponentMin = exponentMinSmall == 0 ? EInteger.Zero :
        (EInteger)exponentMinSmall;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.AdjustExponent"]/*'/>
    public bool AdjustExponent {
      get {
        return this.adjustExponent;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.ClampNormalExponents"]/*'/>
    public bool ClampNormalExponents {
      get {
        return this.hasExponentRange && this.clampNormalExponents;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.EMax"]/*'/>
    public EInteger EMax {
      get {
        return this.hasExponentRange ? this.exponentMax : EInteger.Zero;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.EMin"]/*'/>
    public EInteger EMin {
      get {
        return this.hasExponentRange ? this.exponentMin : EInteger.Zero;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.Flags"]/*'/>
    public int Flags {
      get {
        return this.flags;
      }

      set {
        if (!this.HasFlags) {
          throw new InvalidOperationException("Can't set flags");
        }
        this.flags = value;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.HasExponentRange"]/*'/>
    public bool HasExponentRange {
      get {
        return this.hasExponentRange;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.HasFlags"]/*'/>
    public bool HasFlags {
      get {
        return this.hasFlags;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.HasMaxPrecision"]/*'/>
    public bool HasMaxPrecision {
      get {
        return !this.bigintPrecision.IsZero;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.IsPrecisionInBits"]/*'/>
    public bool IsPrecisionInBits {
      get {
        return this.precisionInBits;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.IsSimplified"]/*'/>
    public bool IsSimplified {
      get {
        return this.simplified;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.Precision"]/*'/>
    public EInteger Precision {
      get {
        return this.bigintPrecision;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.Rounding"]/*'/>
    public ERounding Rounding {
      get {
        return this.rounding;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EContext.Traps"]/*'/>
    public int Traps {
      get {
        return this.traps;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.ForPrecision(System.Int32)"]/*'/>
    public static EContext ForPrecision(int precision) {
      return new EContext(
precision,
ERounding.HalfUp,
0,
0,
false).WithUnlimitedExponents();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.ForPrecisionAndRounding(System.Int32,PeterO.Numbers.ERounding)"]/*'/>
    public static EContext ForPrecisionAndRounding(
      int precision,
      ERounding rounding) {
      return new EContext(
precision,
rounding,
0,
0,
false).WithUnlimitedExponents();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.ForRounding(PeterO.Numbers.ERounding)"]/*'/>
    public static EContext ForRounding(ERounding rounding) {
      return new EContext(
0,
rounding,
0,
0,
false).WithUnlimitedExponents();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.Copy"]/*'/>
    public EContext Copy() {
      var pcnew = new EContext(
        0,
        this.rounding,
        0,
        0,
        this.clampNormalExponents);
      pcnew.hasFlags = this.hasFlags;
      pcnew.precisionInBits = this.precisionInBits;
      pcnew.adjustExponent = this.adjustExponent;
      pcnew.simplified = this.simplified;
      pcnew.flags = this.flags;
      pcnew.exponentMax = this.exponentMax;
      pcnew.exponentMin = this.exponentMin;
      pcnew.hasExponentRange = this.hasExponentRange;
      pcnew.bigintPrecision = this.bigintPrecision;
      pcnew.rounding = this.rounding;
      pcnew.clampNormalExponents = this.clampNormalExponents;
      return pcnew;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.ExponentWithinRange(PeterO.Numbers.EInteger)"]/*'/>
    public bool ExponentWithinRange(EInteger exponent) {
      if (exponent == null) {
        throw new ArgumentNullException("exponent");
      }
      if (!this.HasExponentRange) {
        return true;
      }
      if (this.bigintPrecision.IsZero) {
        // Only check EMax, since with an unlimited
        // precision, any exponent less than EMin will exceed EMin if
        // the mantissa is the right size
        return exponent.CompareTo(this.EMax) <= 0;
      } else {
        EInteger bigint = exponent;
        if (this.adjustExponent) {
          bigint += (EInteger)this.bigintPrecision;
          bigint -= EInteger.One;
        }
        return (bigint.CompareTo(this.EMin) >= 0) &&
          (exponent.CompareTo(this.EMax) <= 0);
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.ToString"]/*'/>
    public override string ToString() {
      return "[PrecisionContext ExponentMax=" + this.exponentMax +
        ", Traps=" + this.traps + ", ExponentMin=" + this.exponentMin +
        ", HasExponentRange=" + this.hasExponentRange + ", BigintPrecision=" +
        this.bigintPrecision + ", Rounding=" + this.rounding +
        ", ClampNormalExponents=" + this.clampNormalExponents + ", Flags=" +
        this.flags + ", HasFlags=" + this.hasFlags + "]";
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithAdjustExponent(System.Boolean)"]/*'/>
    public EContext WithAdjustExponent(bool adjustExponent) {
      EContext pc = this.Copy();
      pc.adjustExponent = adjustExponent;
      return pc;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithBigExponentRange(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'/>
    public EContext WithBigExponentRange(
      EInteger exponentMin,
      EInteger exponentMax) {
      if (exponentMin == null) {
        throw new ArgumentNullException("exponentMin");
      }
      if (exponentMax == null) {
        throw new ArgumentNullException("exponentMax");
      }
      if (exponentMin.CompareTo(exponentMax) > 0) {
        throw new ArgumentException("exponentMin greater than exponentMax");
      }
      EContext pc = this.Copy();
      pc.hasExponentRange = true;
      pc.exponentMin = exponentMin;
      pc.exponentMax = exponentMax;
      return pc;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithBigPrecision(PeterO.Numbers.EInteger)"]/*'/>
    public EContext WithBigPrecision(EInteger bigintPrecision) {
      if (bigintPrecision == null) {
        throw new ArgumentNullException("bigintPrecision");
      }
      if (bigintPrecision.Sign < 0) {
        throw new ArgumentException("bigintPrecision's sign (" +
          bigintPrecision.Sign + ") is less than 0");
      }
      EContext pc = this.Copy();
      pc.bigintPrecision = bigintPrecision;
      return pc;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithBlankFlags"]/*'/>
    public EContext WithBlankFlags() {
      EContext pc = this.Copy();
      pc.hasFlags = true;
      pc.flags = 0;
      return pc;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithExponentClamp(System.Boolean)"]/*'/>
    public EContext WithExponentClamp(bool clamp) {
      EContext pc = this.Copy();
      pc.clampNormalExponents = clamp;
      return pc;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithExponentRange(System.Int32,System.Int32)"]/*'/>
    public EContext WithExponentRange(
      int exponentMinSmall,
      int exponentMaxSmall) {
      if (exponentMinSmall > exponentMaxSmall) {
        throw new ArgumentException("exponentMinSmall (" + exponentMinSmall +
          ") is more than " + exponentMaxSmall);
      }
      EContext pc = this.Copy();
      pc.hasExponentRange = true;
      pc.exponentMin = (EInteger)exponentMinSmall;
      pc.exponentMax = (EInteger)exponentMaxSmall;
      return pc;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithNoFlags"]/*'/>
    public EContext WithNoFlags() {
      EContext pc = this.Copy();
      pc.hasFlags = false;
      pc.flags = 0;
      return pc;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithPrecision(System.Int32)"]/*'/>
    public EContext WithPrecision(int precision) {
      if (precision < 0) {
        throw new ArgumentException("precision (" + precision +
          ") is less than 0");
      }
      EContext pc = this.Copy();
      pc.bigintPrecision = (EInteger)precision;
      return pc;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithPrecisionInBits(System.Boolean)"]/*'/>
    public EContext WithPrecisionInBits(bool isPrecisionBits) {
      EContext pc = this.Copy();
      pc.precisionInBits = isPrecisionBits;
      return pc;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithRounding(PeterO.Numbers.ERounding)"]/*'/>
    public EContext WithRounding(ERounding rounding) {
      EContext pc = this.Copy();
      pc.rounding = rounding;
      return pc;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithSimplified(System.Boolean)"]/*'/>
    public EContext WithSimplified(bool simplified) {
      EContext pc = this.Copy();
      pc.simplified = simplified;
      return pc;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithTraps(System.Int32)"]/*'/>
    public EContext WithTraps(int traps) {
      EContext pc = this.Copy();
      pc.hasFlags = true;
      pc.traps = traps;
      return pc;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EContext.WithUnlimitedExponents"]/*'/>
    public EContext WithUnlimitedExponents() {
      EContext pc = this.Copy();
      pc.hasExponentRange = false;
      return pc;
    }
  }
}
