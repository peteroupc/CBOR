/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Numbers {
    /// <summary>Contains parameters for controlling the precision,
    /// rounding, and exponent range of arbitrary-precision
    /// numbers.</summary>
  public sealed class EContext {
    /// <summary>Signals that the exponent was adjusted to fit the exponent
    /// range.</summary>
    public const int FlagClamped = 32;

    /// <summary>Signals a division of a nonzero number by zero.</summary>
    public const int FlagDivideByZero = 128;

    /// <summary>Signals that the result was rounded to a different
    /// mathematical value, but as close as possible to the
    /// original.</summary>
    public const int FlagInexact = 1;

    /// <summary>Signals an invalid operation.</summary>
    public const int FlagInvalid = 64;

    /// <summary>Signals that an operand was rounded to a different
    /// mathematical value before an operation.</summary>
    public const int FlagLostDigits = 256;

    /// <summary>Signals that the result is non-zero and the exponent is
    /// higher than the highest exponent allowed.</summary>
    public const int FlagOverflow = 16;

    /// <summary>Signals that the result was rounded to fit the precision;
    /// either the value or the exponent may have changed from the
    /// original.</summary>
    public const int FlagRounded = 2;

    /// <summary>Signals that the result&#x27;s exponent, before rounding,
    /// is lower than the lowest exponent allowed.</summary>
    public const int FlagSubnormal = 4;

    /// <summary>Signals that the result&#x27;s exponent, before rounding,
    /// is lower than the lowest exponent allowed, and the result was
    /// rounded to a different mathematical value, but as close as possible
    /// to the original.</summary>
    public const int FlagUnderflow = 8;

    /// <summary>Basic precision context, 9 digits precision, rounding mode
    /// half-up, unlimited exponent range. The default rounding mode is
    /// HalfUp.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly EContext Basic =
      EContext.ForPrecisionAndRounding(9, ERounding.HalfUp);

    /// <summary>Precision context for Java's BigDecimal format. The
    /// default rounding mode is HalfUp.</summary>
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

    /// <summary>Precision context for the IEEE-754-2008 binary128 format,
    /// 113 bits precision. The default rounding mode is
    /// HalfEven.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly EContext Binary128 =
      EContext.ForPrecisionAndRounding(113, ERounding.HalfEven)
      .WithExponentClamp(true).WithExponentRange(-16382, 16383);

    /// <summary>Precision context for the IEEE-754-2008 binary16 format,
    /// 11 bits precision. The default rounding mode is HalfEven.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly EContext Binary16 =
      EContext.ForPrecisionAndRounding(11, ERounding.HalfEven)
      .WithExponentClamp(true).WithExponentRange(-14, 15);

    /// <summary>Precision context for the IEEE-754-2008 binary32 format,
    /// 24 bits precision. The default rounding mode is HalfEven.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly EContext Binary32 =
      EContext.ForPrecisionAndRounding(24, ERounding.HalfEven)
      .WithExponentClamp(true).WithExponentRange(-126, 127);

    /// <summary>Precision context for the IEEE-754-2008 binary64 format,
    /// 53 bits precision. The default rounding mode is HalfEven.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly EContext Binary64 =
      EContext.ForPrecisionAndRounding(53, ERounding.HalfEven)
      .WithExponentClamp(true).WithExponentRange(-1022, 1023);

    /// <summary>Precision context for the Common Language Infrastructure
    /// (.NET Framework) decimal format, 96 bits precision, and a valid
    /// exponent range of -28 to 0. The default rounding mode is
    /// HalfEven.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly EContext CliDecimal =
      new EContext(96, ERounding.HalfEven, 0, 28, true)
      .WithPrecisionInBits(true);

    /// <summary>Precision context for the IEEE-754-2008 decimal128 format.
    /// The default rounding mode is HalfEven.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly EContext Decimal128 =
      new EContext(34, ERounding.HalfEven, -6143, 6144, true);

    /// <summary>Precision context for the IEEE-754-2008 decimal32 format.
    /// The default rounding mode is HalfEven.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly EContext Decimal32 =
      new EContext(7, ERounding.HalfEven, -95, 96, true);

    /// <summary>Precision context for the IEEE-754-2008 decimal64 format.
    /// The default rounding mode is HalfEven.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly EContext Decimal64 =
      new EContext(16, ERounding.HalfEven, -383, 384, true);

    /// <summary>No specific limit on precision. Rounding mode
    /// HalfUp.</summary>
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

    /// <summary>Initializes a new instance of the PrecisionContext class.
    /// HasFlags will be set to false.</summary>
    /// <param name='precision'>Not documented yet.</param>
    /// <param name='rounding'>Not documented yet.</param>
    /// <param name='exponentMinSmall'>Not documented yet. (3).</param>
    /// <param name='exponentMaxSmall'>Not documented yet. (4).</param>
    /// <param name='clampNormalExponents'>Not documented yet. (5).</param>
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

    /// <summary>Gets a value indicating whether the EMax and EMin
    /// properties refer to the number's Exponent property adjusted to the
    /// number's precision, or just the number's Exponent property. The
    /// default value is true, meaning that EMax and EMin refer to the
    /// adjusted exponent. Setting this value to false (using
    /// WithAdjustExponent) is useful for modeling floating point
    /// representations with an integer mantissa and an integer exponent,
    /// such as Java's BigDecimal.</summary>
    /// <value>True if the EMax and EMin properties refer to the
    /// number&apos;s Exponent property adjusted to the number&apos;s
    /// precision, or false if they refer to just the number&apos;s
    /// Exponent property.</value>
    public bool AdjustExponent {
      get {
        return this.adjustExponent;
      }
    }

    /// <summary>Gets a value indicating whether a converted number&#x27;s
    /// Exponent property will not be higher than EMax + 1 - Precision. If
    /// a number&#x27;s exponent is higher than that value, but not high
    /// enough to cause overflow, the exponent is clamped to that value and
    /// enough zeros are added to the number&#x27;s mantissa to account for
    /// the adjustment. If HasExponentRange is false, this value is always
    /// false.</summary>
    /// <value>If true, a converted number&apos;s Exponent property will
    /// not be higher than EMax + 1 - Precision.</value>
    public bool ClampNormalExponents {
      get {
        return this.hasExponentRange && this.clampNormalExponents;
      }
    }

    /// <summary>Gets the highest exponent possible when a converted number
    /// is expressed in scientific notation with one digit before the
    /// decimal point. For example, with a precision of 3 and an EMax of
    /// 100, the maximum value possible is 9.99E + 100. (This is not the
    /// same as the highest possible Exponent property.) If
    /// HasExponentRange is false, this value will be 0.</summary>
    /// <value>The highest exponent possible when a converted number is
    /// expressed in scientific notation with one digit before the decimal
    /// point. For example, with a precision of 3 and an EMax of 100, the
    /// maximum value possible is 9.99E + 100. (This is not the same as the
    /// highest possible Exponent property.) If HasExponentRange is false,
    /// this value will be 0.</value>
    public EInteger EMax {
      get {
        return this.hasExponentRange ? this.exponentMax : EInteger.Zero;
      }
    }

    /// <summary>Gets the lowest exponent possible when a converted number
    /// is expressed in scientific notation with one digit before the
    /// decimal point. For example, with a precision of 3 and an EMin of
    /// -100, the next value that comes after 0 is 0.001E-100. (If
    /// AdjustExponent is false, this property specifies the lowest
    /// possible Exponent property instead.) If HasExponentRange is false,
    /// this value will be 0.</summary>
    /// <value>The lowest exponent possible when a converted number is
    /// expressed in scientific notation with one digit before the decimal
    /// point.</value>
    public EInteger EMin {
      get {
        return this.hasExponentRange ? this.exponentMin : EInteger.Zero;
      }
    }

    /// <summary>Gets or sets the flags that are set from converting
    /// numbers according to this precision context. If HasFlags is false,
    /// this value will be 0. This value is a combination of bit fields. To
    /// retrieve a particular flag, use the AND operation on the return
    /// value of this method. For example: <c>(this.Flags &amp;
    /// PrecisionContext.FlagInexact) != 0</c> returns TRUE if the Inexact
    /// flag is set.</summary>
    /// <value>The flags that are set from converting numbers according to
    /// this precision context. If HasFlags is false, this value will be
    /// 0.</value>
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

    /// <summary>Gets a value indicating whether this context defines a
    /// minimum and maximum exponent. If false, converted exponents can
    /// have any exponent and operations can't cause overflow or
    /// underflow.</summary>
    /// <value>True if this context defines a minimum and maximum exponent;
    /// otherwise, false.</value>
    public bool HasExponentRange {
      get {
        return this.hasExponentRange;
      }
    }

    /// <summary>Gets a value indicating whether this context has a mutable
    /// Flags field.</summary>
    /// <value>True if this context has a mutable Flags field; otherwise,
    /// false.</value>
    public bool HasFlags {
      get {
        return this.hasFlags;
      }
    }

    /// <summary>Gets a value indicating whether this context defines a
    /// maximum precision.</summary>
    /// <value>True if this context defines a maximum precision; otherwise,
    /// false.</value>
    public bool HasMaxPrecision {
      get {
        return !this.bigintPrecision.IsZero;
      }
    }

    /// <summary>Gets a value indicating whether this context's Precision
    /// property is in bits, rather than digits. The default is
    /// false.</summary>
    /// <value>True if this context&apos;s Precision property is in bits,
    /// rather than digits; otherwise, false. The default is false.</value>
    public bool IsPrecisionInBits {
      get {
        return this.precisionInBits;
      }
    }

    /// <summary>Gets a value indicating whether to use a "simplified"
    /// arithmetic. In the simplified arithmetic, infinity, not-a-number,
    /// and subnormal numbers are not allowed, and negative zero is treated
    /// the same as positive zero. For further details, see
    /// <c>http://speleotrove.com/decimal/dax3274.html</c></summary>
    /// <value>True if a &quot;simplified&quot; arithmetic will be used;
    /// otherwise, false.</value>
    public bool IsSimplified {
      get {
        return this.simplified;
      }
    }

    /// <summary>Gets the maximum length of a converted number in digits,
    /// ignoring the decimal point and exponent. For example, if precision
    /// is 3, a converted number&#x27;s mantissa can range from 0 to 999
    /// (up to three digits long). If 0, converted numbers can have any
    /// precision.</summary>
    /// <value>The maximum length of a converted number in digits, ignoring
    /// the decimal point and exponent.</value>
    public EInteger Precision {
      get {
        return this.bigintPrecision;
      }
    }

    /// <summary>Gets the desired rounding mode when converting numbers
    /// that can&#x27;t be represented in the given precision and exponent
    /// range.</summary>
    /// <value>The desired rounding mode when converting numbers that
    /// can&apos;t be represented in the given precision and exponent
    /// range.</value>
    public ERounding Rounding {
      get {
        return this.rounding;
      }
    }

    /// <summary>Gets the traps that are set for each flag in the context.
    /// Whenever a flag is signaled, even if HasFlags is false, and the
    /// flag's trap is enabled, the operation will throw a TrapException.
    /// <para>For example, if Traps equals FlagInexact and FlagSubnormal, a
    /// TrapException will be thrown if an operation's return value is not
    /// the same as the exact result (FlagInexact) or if the return value's
    /// exponent is lower than the lowest allowed
    /// (FlagSubnormal).</para></summary>
    /// <value>The traps that are set for each flag in the context.</value>
    public int Traps {
      get {
        return this.traps;
      }
    }

    /// <summary>Creates a new precision context using the given maximum
    /// number of digits, an unlimited exponent range, and the HalfUp
    /// rounding mode.</summary>
    /// <param name='precision'>Maximum number of digits
    /// (precision).</param>
    /// <returns>An EContext object.</returns>
    public static EContext ForPrecision(int precision) {
      return new EContext(
precision,
ERounding.HalfUp,
0,
0,
false).WithUnlimitedExponents();
    }

    /// <summary>Creates a new PrecisionContext object initialized with an
    /// unlimited exponent range, and the given rounding mode and maximum
    /// precision.</summary>
    /// <param name='precision'>Maximum number of digits
    /// (precision).</param>
    /// <param name='rounding'>An ERounding object.</param>
    /// <returns>An EContext object.</returns>
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

    /// <summary>Creates a new PrecisionContext object initialized with an
    /// unlimited precision, an unlimited exponent range, and the given
    /// rounding mode.</summary>
    /// <param name='rounding'>The rounding mode for the new precision
    /// context.</param>
    /// <returns>An EContext object.</returns>
    public static EContext ForRounding(ERounding rounding) {
      return new EContext(
0,
rounding,
0,
0,
false).WithUnlimitedExponents();
    }

    /// <summary>Initializes a new PrecisionContext that is a copy of
    /// another PrecisionContext.</summary>
    /// <returns>An EContext object.</returns>
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

    /// <summary>Determines whether a number can have the given Exponent
    /// property under this precision context.</summary>
    /// <param name='exponent'>A BigInteger object indicating the desired
    /// exponent.</param>
    /// <returns>True if a number can have the given Exponent property
    /// under this precision context; otherwise, false. If this context
    /// allows unlimited precision, returns true for the exponent EMax and
    /// any exponent less than EMax.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='exponent'/> is null.</exception>
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

    /// <summary>Gets a string representation of this object. Note that the
    /// format is not intended to be parsed and may change at any
    /// time.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      return "[PrecisionContext ExponentMax=" + this.exponentMax +
        ", Traps=" + this.traps + ", ExponentMin=" + this.exponentMin +
        ", HasExponentRange=" + this.hasExponentRange + ", BigintPrecision=" +
        this.bigintPrecision + ", Rounding=" + this.rounding +
        ", ClampNormalExponents=" + this.clampNormalExponents + ", Flags=" +
        this.flags + ", HasFlags=" + this.hasFlags + "]";
    }

    /// <summary>Copies this PrecisionContext and sets the copy's
    /// "AdjustExponent" property to the given value.</summary>
    /// <param name='adjustExponent'>Not documented yet.</param>
    /// <returns>An EContext object.</returns>
    public EContext WithAdjustExponent(bool adjustExponent) {
      EContext pc = this.Copy();
      pc.adjustExponent = adjustExponent;
      return pc;
    }

    /// <summary>Copies this precision context and sets the copy&#x27;s
    /// exponent range.</summary>
    /// <param name='exponentMin'>Desired minimum exponent (EMin).</param>
    /// <param name='exponentMax'>Desired maximum exponent (EMax).</param>
    /// <returns>An EContext object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='exponentMin'/> is null.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='exponentMax'/> is null.</exception>
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

    /// <summary>Copies this PrecisionContext and gives it a particular
    /// precision value.</summary>
    /// <param name='bigintPrecision'>Not documented yet.</param>
    /// <returns>An EContext object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintPrecision'/> is null.</exception>
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

    /// <summary>Copies this PrecisionContext with HasFlags set to true and
    /// a Flags value of 0.</summary>
    /// <returns>An EContext object.</returns>
    public EContext WithBlankFlags() {
      EContext pc = this.Copy();
      pc.hasFlags = true;
      pc.flags = 0;
      return pc;
    }

    /// <summary>Copies this precision context and sets the copy&#x27;s
    /// &#x22;ClampNormalExponents&#x22; flag to the given value.</summary>
    /// <param name='clamp'>Not documented yet.</param>
    /// <returns>An EContext object.</returns>
    public EContext WithExponentClamp(bool clamp) {
      EContext pc = this.Copy();
      pc.clampNormalExponents = clamp;
      return pc;
    }

    /// <summary>Copies this precision context and sets the copy&#x27;s
    /// exponent range.</summary>
    /// <param name='exponentMinSmall'>Desired minimum exponent
    /// (EMin).</param>
    /// <param name='exponentMaxSmall'>Desired maximum exponent
    /// (EMax).</param>
    /// <returns>An EContext object.</returns>
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

    /// <summary>Copies this PrecisionContext with HasFlags set to false
    /// and a Flags value of 0.</summary>
    /// <returns>An EContext object.</returns>
    public EContext WithNoFlags() {
      EContext pc = this.Copy();
      pc.hasFlags = false;
      pc.flags = 0;
      return pc;
    }

    /// <summary>Copies this PrecisionContext and gives it a particular
    /// precision value.</summary>
    /// <param name='precision'>Desired precision. 0 means unlimited
    /// precision.</param>
    /// <returns>An EContext object.</returns>
    public EContext WithPrecision(int precision) {
      if (precision < 0) {
        throw new ArgumentException("precision (" + precision +
          ") is less than 0");
      }
      EContext pc = this.Copy();
      pc.bigintPrecision = (EInteger)precision;
      return pc;
    }

    /// <summary>Copies this PrecisionContext and sets the copy's
    /// "IsPrecisionInBits" property to the given value.</summary>
    /// <param name='isPrecisionBits'>Not documented yet.</param>
    /// <returns>An EContext object.</returns>
    public EContext WithPrecisionInBits(bool isPrecisionBits) {
      EContext pc = this.Copy();
      pc.precisionInBits = isPrecisionBits;
      return pc;
    }

    /// <summary>Copies this PrecisionContext with the specified rounding
    /// mode.</summary>
    /// <param name='rounding'>Not documented yet.</param>
    /// <returns>An EContext object.</returns>
    public EContext WithRounding(ERounding rounding) {
      EContext pc = this.Copy();
      pc.rounding = rounding;
      return pc;
    }

    /// <summary>Copies this PrecisionContext and sets the copy's
    /// "IsSimplified" property to the given value.</summary>
    /// <param name='simplified'>Not documented yet.</param>
    /// <returns>An EContext object.</returns>
    public EContext WithSimplified(bool simplified) {
      EContext pc = this.Copy();
      pc.simplified = simplified;
      return pc;
    }

    /// <summary>Copies this PrecisionContext with Traps set to the given
    /// value.</summary>
    /// <param name='traps'>Flags representing the traps to enable. See the
    /// property "Traps".</param>
    /// <returns>An EContext object.</returns>
    public EContext WithTraps(int traps) {
      EContext pc = this.Copy();
      pc.hasFlags = true;
      pc.traps = traps;
      return pc;
    }

    /// <summary>Copies this PrecisionContext with an unlimited exponent
    /// range.</summary>
    /// <returns>An EContext object.</returns>
    public EContext WithUnlimitedExponents() {
      EContext pc = this.Copy();
      pc.hasExponentRange = false;
      return pc;
    }
  }
}
