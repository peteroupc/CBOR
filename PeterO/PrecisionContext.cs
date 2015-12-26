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
    /// <summary>Contains parameters for controlling the precision,
    /// rounding, and exponent range of arbitrary-precision numbers. NOTE:
    /// This class is not designed for inheritance, and inheriting from
    /// this class is not recommended since it may break in future
    /// versions.</summary>
  public class PrecisionContext {
    // TODO: Add 'sealed' to PrecisionContext class in version 3

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

    internal readonly EContext ec;

    internal PrecisionContext(EContext ec) {
      this.ec = ec;
    }

    /// <summary>Basic precision context, 9 digits precision, rounding mode
    /// half-up, unlimited exponent range. The default rounding mode is
    /// HalfUp.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly PrecisionContext Basic =
      new PrecisionContext(EContext.Basic);

    /// <summary>Precision context for Java's BigDecimal format. The
    /// default rounding mode is HalfUp.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly PrecisionContext BigDecimalJava =
      new PrecisionContext(EContext.BigDecimalJava);

    /// <summary>Precision context for the IEEE-754-2008 binary128 format,
    /// 113 bits precision. The default rounding mode is
    /// HalfEven.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly PrecisionContext Binary128 =
      new PrecisionContext(EContext.Binary128);

    /// <summary>Precision context for the IEEE-754-2008 binary16 format,
    /// 11 bits precision. The default rounding mode is HalfEven.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly PrecisionContext Binary16 =
      new PrecisionContext(EContext.Binary16);

    /// <summary>Precision context for the IEEE-754-2008 binary32 format,
    /// 24 bits precision. The default rounding mode is HalfEven.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly PrecisionContext Binary32 =
      new PrecisionContext(EContext.Binary32);

    /// <summary>Precision context for the IEEE-754-2008 binary64 format,
    /// 53 bits precision. The default rounding mode is HalfEven.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly PrecisionContext Binary64 =
      new PrecisionContext(EContext.Binary64);

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

    public static readonly PrecisionContext CliDecimal =
      new PrecisionContext(EContext.CliDecimal);

    /// <summary>Precision context for the IEEE-754-2008 decimal128 format.
    /// The default rounding mode is HalfEven.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly PrecisionContext Decimal128 =
      new PrecisionContext(EContext.Decimal128);

    /// <summary>Precision context for the IEEE-754-2008 decimal32 format.
    /// The default rounding mode is HalfEven.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly PrecisionContext Decimal32 =
      new PrecisionContext(EContext.Decimal32);

    /// <summary>Precision context for the IEEE-754-2008 decimal64 format.
    /// The default rounding mode is HalfEven.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly PrecisionContext Decimal64 =
      new PrecisionContext(EContext.Decimal64);

    /// <summary>Precision context for Java's BigDecimal format. The
    /// default rounding mode is HalfUp.</summary>
    [Obsolete("This context had the wrong settings in previous " +
              "versions. Use BigDecimalJava instead.")]
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif
    public static readonly PrecisionContext JavaBigDecimal = BigDecimalJava;

    /// <summary>No specific limit on precision. Rounding mode
    /// HalfUp.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This PrecisionContext is immutable")]
#endif

    public static readonly PrecisionContext Unlimited =
      PrecisionContext.ForPrecision(0);

    /// <summary>Initializes a new instance of the PrecisionContext class.
    /// HasFlags will be set to false.</summary>
    /// <param name='precision'>A 32-bit signed integer.</param>
    /// <param name='rounding'>A Rounding object.</param>
    /// <param name='exponentMinSmall'>A 32-bit signed integer.
    /// (2).</param>
    /// <param name='exponentMaxSmall'>A 32-bit signed integer.
    /// (3).</param>
    /// <param name='clampNormalExponents'>A Boolean object.</param>
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
    public  bool AdjustExponent {
get {
return this.ec.AdjustExponent;
} }

    /// <summary>Gets a value indicating whether a converted number&#x27;s
    /// Exponent property will not be higher than EMax + 1 - Precision. If
    /// a number&#x27;s exponent is higher than that value, but not high
    /// enough to cause overflow, the exponent is clamped to that value and
    /// enough zeros are added to the number&#x27;s mantissa to account for
    /// the adjustment. If HasExponentRange is false, this value is always
    /// false.</summary>
    /// <value>If true, a converted number&apos;s Exponent property will
    /// not be higher than EMax + 1 - Precision.</value>
    public  bool ClampNormalExponents {
get {
return this.ec.ClampNormalExponents;
} }

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
    public  BigInteger EMax {
get {
return new BigInteger(this.ec.EMax);
} }

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
    public  BigInteger EMin {
get {
return new BigInteger(this.ec.EMin);
} }

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
    public  int Flags {
get {
return this.ec.Flags;
}
    set {
        this.ec.Flags = value;
} }

    /// <summary>Gets a value indicating whether this context defines a
    /// minimum and maximum exponent. If false, converted exponents can
    /// have any exponent and operations can't cause overflow or
    /// underflow.</summary>
    /// <value>True if this context defines a minimum and maximum exponent;
    /// otherwise, false.</value>
    public  bool HasExponentRange {
get {
return this.ec.HasExponentRange;
} }

    /// <summary>Gets a value indicating whether this context has a mutable
    /// Flags field.</summary>
    /// <value>True if this context has a mutable Flags field; otherwise,
    /// false.</value>
    public  bool HasFlags {
get {
return this.ec.HasFlags;
} }

    /// <summary>Gets a value indicating whether this context defines a
    /// maximum precision.</summary>
    /// <value>True if this context defines a maximum precision; otherwise,
    /// false.</value>
    public  bool HasMaxPrecision {
get {
return this.ec.HasMaxPrecision;
} }

    /// <summary>Gets a value indicating whether this context's Precision
    /// property is in bits, rather than digits. The default is
    /// false.</summary>
    /// <value>True if this context&apos;s Precision property is in bits,
    /// rather than digits; otherwise, false. The default is false.</value>
    public  bool IsPrecisionInBits {
get {
return this.ec.IsPrecisionInBits;
} }

    /// <summary>Gets a value indicating whether to use a "simplified"
    /// arithmetic. In the simplified arithmetic, infinity, not-a-number,
    /// and subnormal numbers are not allowed, and negative zero is treated
    /// the same as positive zero. For further details, see
    /// <c>http://speleotrove.com/decimal/dax3274.html</c></summary>
    /// <value>True if a &quot;simplified&quot; arithmetic will be used;
    /// otherwise, false.</value>
    public  bool IsSimplified {
get {
return this.ec.IsSimplified;
} }

    /// <summary>Gets the maximum length of a converted number in digits,
    /// ignoring the decimal point and exponent. For example, if precision
    /// is 3, a converted number&#x27;s mantissa can range from 0 to 999
    /// (up to three digits long). If 0, converted numbers can have any
    /// precision.</summary>
    /// <value>The maximum length of a converted number in digits, ignoring
    /// the decimal point and exponent.</value>
    public  BigInteger Precision {
get {
return new BigInteger(this.ec.Precision);
} }

    /// <summary>Gets the desired rounding mode when converting numbers
    /// that can&#x27;t be represented in the given precision and exponent
    /// range.</summary>
    /// <value>The desired rounding mode when converting numbers that
    /// can&apos;t be represented in the given precision and exponent
    /// range.</value>
    public  Rounding Rounding {
get {
        return ExtendedDecimal.ToRounding(this.ec.Rounding);
} }

    /// <summary>Gets the traps that are set for each flag in the context.
    /// Whenever a flag is signaled, even if HasFlags is false, and the
    /// flag's trap is enabled, the operation will throw a TrapException.
    /// <para>For example, if Traps equals FlagInexact and FlagSubnormal, a
    /// TrapException will be thrown if an operation's return value is not
    /// the same as the exact result (FlagInexact) or if the return value's
    /// exponent is lower than the lowest allowed
    /// (FlagSubnormal).</para></summary>
    /// <value>The traps that are set for each flag in the context.</value>
    public  int Traps {
get {
return this.ec.Traps;
} }

    /// <summary>Creates a new precision context using the given maximum
    /// number of digits, an unlimited exponent range, and the HalfUp
    /// rounding mode.</summary>
    /// <param name='precision'>Maximum number of digits
    /// (precision).</param>
    /// <returns>A PrecisionContext object.</returns>
    public static PrecisionContext ForPrecision(int precision) {
return new PrecisionContext(EContext.ForPrecision(precision));
}

    /// <summary>Creates a new PrecisionContext object initialized with an
    /// unlimited exponent range, and the given rounding mode and maximum
    /// precision.</summary>
    /// <param name='precision'>Maximum number of digits
    /// (precision).</param>
    /// <param name='rounding'>A Rounding object.</param>
    /// <returns>A PrecisionContext object.</returns>
    public static PrecisionContext ForPrecisionAndRounding(int precision,
      Rounding rounding) {
return new PrecisionContext(EContext.ForPrecisionAndRounding(precision,
  ExtendedDecimal.ToERounding(rounding)));
}

    /// <summary>Creates a new PrecisionContext object initialized with an
    /// unlimited precision, an unlimited exponent range, and the given
    /// rounding mode.</summary>
    /// <param name='rounding'>The rounding mode for the new precision
    /// context.</param>
    /// <returns>A PrecisionContext object.</returns>
    public static PrecisionContext ForRounding(Rounding rounding) {
return new
  PrecisionContext(EContext.ForRounding(ExtendedDecimal.ToERounding(rounding)));
}

    /// <summary>Initializes a new PrecisionContext that is a copy of
    /// another PrecisionContext.</summary>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext Copy() {
return new PrecisionContext(this.ec.Copy());
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
    public bool ExponentWithinRange(BigInteger exponent) {
  if ((exponent) == null) {
  throw new ArgumentNullException("exponent");
}
return this.ec.ExponentWithinRange(exponent.ei);
}

    /// <summary>Gets a string representation of this object. Note that the
    /// format is not intended to be parsed and may change at any
    /// time.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
return this.ec.ToString();
}

    /// <summary>Copies this PrecisionContext and sets the copy's
    /// "AdjustExponent" property to the given value.</summary>
    /// <param name='adjustExponent'>A Boolean object.</param>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithAdjustExponent(bool adjustExponent) {
return new PrecisionContext(this.ec.WithAdjustExponent(adjustExponent));
}

    /// <summary>Copies this precision context and sets the copy&#x27;s
    /// exponent range.</summary>
    /// <param name='exponentMin'>Desired minimum exponent (EMin).</param>
    /// <param name='exponentMax'>Desired maximum exponent (EMax).</param>
    /// <returns>A PrecisionContext object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='exponentMin'/> is null.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='exponentMax'/> is null.</exception>
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

    /// <summary>Copies this PrecisionContext and gives it a particular
    /// precision value.</summary>
    /// <param name='bigintPrecision'>A BigInteger object.</param>
    /// <returns>A PrecisionContext object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintPrecision'/> is null.</exception>
    public PrecisionContext WithBigPrecision(BigInteger bigintPrecision) {
  if ((bigintPrecision) == null) {
  throw new ArgumentNullException("bigintPrecision");
}
return new PrecisionContext(this.ec.WithBigPrecision(bigintPrecision.ei));
}

    /// <summary>Copies this PrecisionContext with HasFlags set to true and
    /// a Flags value of 0.</summary>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithBlankFlags() {
return new PrecisionContext(this.ec.WithBlankFlags());
}

    /// <summary>Copies this precision context and sets the copy&#x27;s
    /// &#x22;ClampNormalExponents&#x22; flag to the given value.</summary>
    /// <param name='clamp'>A Boolean object.</param>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithExponentClamp(bool clamp) {
return new PrecisionContext(this.ec.WithExponentClamp(clamp));
}

    /// <summary>Copies this precision context and sets the copy&#x27;s
    /// exponent range.</summary>
    /// <param name='exponentMinSmall'>Desired minimum exponent
    /// (EMin).</param>
    /// <param name='exponentMaxSmall'>Desired maximum exponent
    /// (EMax).</param>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithExponentRange(int exponentMinSmall,
      int exponentMaxSmall) {
return new PrecisionContext(this.ec.WithExponentRange(exponentMinSmall,
  exponentMaxSmall));
}

    /// <summary>Copies this PrecisionContext with HasFlags set to false
    /// and a Flags value of 0.</summary>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithNoFlags() {
return new PrecisionContext(this.ec.WithNoFlags());
}

    /// <summary>Copies this PrecisionContext and gives it a particular
    /// precision value.</summary>
    /// <param name='precision'>Desired precision. 0 means unlimited
    /// precision.</param>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithPrecision(int precision) {
return new PrecisionContext(this.ec.WithPrecision(precision));
}

    /// <summary>Copies this PrecisionContext and sets the copy's
    /// "IsPrecisionInBits" property to the given value.</summary>
    /// <param name='isPrecisionBits'>A Boolean object.</param>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithPrecisionInBits(bool isPrecisionBits) {
return new PrecisionContext(this.ec.WithPrecisionInBits(isPrecisionBits));
}

    /// <summary>Copies this PrecisionContext with the specified rounding
    /// mode.</summary>
    /// <param name='rounding'>A Rounding object.</param>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithRounding(Rounding rounding) {
return new
  PrecisionContext(this.ec.WithRounding(ExtendedDecimal.ToERounding(rounding)));
}

    /// <summary>Copies this PrecisionContext and sets the copy's
    /// "IsSimplified" property to the given value.</summary>
    /// <param name='simplified'>A Boolean object.</param>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithSimplified(bool simplified) {
return new PrecisionContext(this.ec.WithSimplified(simplified));
}

    /// <summary>Copies this PrecisionContext with Traps set to the given
    /// value.</summary>
    /// <param name='traps'>Flags representing the traps to enable. See the
    /// property "Traps".</param>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithTraps(int traps) {
return new PrecisionContext(this.ec.WithTraps(traps));
}

    /// <summary>Copies this PrecisionContext with an unlimited exponent
    /// range.</summary>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithUnlimitedExponents() {
return new PrecisionContext(this.ec.WithUnlimitedExponents());
}
  }
}
