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
    /// <summary>Represents an arbitrary-precision binary floating-point
    /// number. Consists of an integer mantissa and an integer exponent,
    /// both arbitrary-precision. The value of the number equals mantissa *
    /// 2^exponent. This class also supports values for negative zero,
    /// not-a-number (NaN) values, and infinity.
    /// <para>Passing a signaling NaN to any arithmetic operation shown
    /// here will signal the flag FlagInvalid and return a quiet NaN, even
    /// if another operand to that operation is a quiet NaN, unless noted
    /// otherwise.</para>
    /// <para>Passing a quiet NaN to any arithmetic operation shown here
    /// will return a quiet NaN, unless noted otherwise.</para>
    /// <para>Unless noted otherwise, passing a null ExtendedFloat argument
    /// to any method here will throw an exception.</para>
    /// <para>When an arithmetic operation signals the flag FlagInvalid,
    /// FlagOverflow, or FlagDivideByZero, it will not throw an exception
    /// too, unless the operation's trap is enabled in the precision
    /// context (see PrecisionContext's Traps property).</para>
    /// <para>An ExtendedFloat value can be serialized in one of the
    /// following ways:</para>
    /// <list>
    /// <item>By calling the toString() method. However, not all strings
    /// can be converted back to an ExtendedFloat without loss, especially
    /// if the string has a fractional part.</item>
    /// <item>By calling the UnsignedMantissa, Exponent, and IsNegative
    /// properties, and calling the IsInfinity, IsQuietNaN, and
    /// IsSignalingNaN methods. The return values combined will uniquely
    /// identify a particular ExtendedFloat value.</item></list></summary>
  public sealed class ExtendedFloat : IComparable<ExtendedFloat>,
  IEquatable<ExtendedFloat> {
    internal readonly EFloat ef;
    internal ExtendedFloat(EFloat ef) {
      this.ef = ef;
    }

    /// <summary>Gets this object&#x27;s exponent. This object&#x27;s value
    /// will be an integer if the exponent is positive or zero.</summary>
    /// <value>This object&apos;s exponent. This object&apos;s value will
    /// be an integer if the exponent is positive or zero.</value>
    public BigInteger Exponent {
      get {
        return new BigInteger(this.ef.Exponent);
      }
    }

    /// <summary>Gets the absolute value of this object&#x27;s un-scaled
    /// value.</summary>
    /// <value>The absolute value of this object&apos;s un-scaled
    /// value.</value>
    public BigInteger UnsignedMantissa {
      get {
        return new BigInteger(this.ef.UnsignedMantissa);
      }
    }

    /// <summary>Gets this object&#x27;s un-scaled value.</summary>
    /// <value>This object&apos;s un-scaled value. Will be negative if this
    /// object&apos;s value is negative (including a negative NaN).</value>
    public BigInteger Mantissa {
      get {
        return new BigInteger(this.ef.Mantissa);
      }
    }

    #region Equals and GetHashCode implementation
    /// <summary>Determines whether this object&#x27;s mantissa and
    /// exponent are equal to those of another object.</summary>
    /// <param name='otherValue'>An ExtendedFloat object.</param>
    /// <returns>True if this object's mantissa and exponent are equal to
    /// those of another object; otherwise, false.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public bool EqualsInternal(ExtendedFloat otherValue) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return this.ef.EqualsInternal(otherValue.ef);
    }

    /// <summary>Determines whether this object&#x27;s mantissa and
    /// exponent are equal to those of another object.</summary>
    /// <param name='other'>An ExtendedFloat object.</param>
    /// <returns>True if this object's mantissa and exponent are equal to
    /// those of another object; otherwise, false.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='other'/> is null.</exception>
    public bool Equals(ExtendedFloat other) {
      if ((other) == null) {
        throw new ArgumentNullException("other");
      }
      return this.ef.Equals(other.ef);
    }

    /// <summary>Determines whether this object&#x27;s mantissa and
    /// exponent are equal to those of another object and that other object
    /// is a decimal fraction.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object obj) {
      var bi = obj as ExtendedFloat;
      return (bi == null) ? (false) : (this.ef.Equals(bi.ef));
    }

    /// <summary>Calculates this object&#x27;s hash code.</summary>
    /// <returns>This object's hash code.</returns>
    public override int GetHashCode() {
      return this.ef.GetHashCode();
    }
    #endregion

    /// <summary>Creates a not-a-number ExtendedFloat object.</summary>
    /// <param name='diag'>A number to use as diagnostic information
    /// associated with this object. If none is needed, should be
    /// zero.</param>
    /// <returns>A quiet not-a-number object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='diag'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='diag'/> is less than 0.</exception>
    public static ExtendedFloat CreateNaN(BigInteger diag) {
      if ((diag) == null) {
        throw new ArgumentNullException("diag");
      }
      return new ExtendedFloat(EFloat.CreateNaN(diag.ei));
    }

    /// <summary>Creates a not-a-number ExtendedFloat object.</summary>
    /// <param name='diag'>A number to use as diagnostic information
    /// associated with this object. If none is needed, should be
    /// zero.</param>
    /// <param name='signaling'>Whether the return value will be signaling
    /// (true) or quiet (false).</param>
    /// <param name='negative'>Whether the return value is
    /// negative.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='diag'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='diag'/> is less than 0.</exception>
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

    /// <summary>Creates a number with the value
    /// exponent*2^mantissa.</summary>
    /// <param name='mantissaSmall'>The un-scaled value.</param>
    /// <param name='exponentSmall'>The binary exponent.</param>
    /// <returns>An ExtendedFloat object.</returns>
    public static ExtendedFloat Create(int mantissaSmall, int exponentSmall) {
      return new ExtendedFloat(EFloat.Create(mantissaSmall, exponentSmall));
    }

    /// <summary>Creates a number with the value
    /// exponent*2^mantissa.</summary>
    /// <param name='mantissa'>The un-scaled value.</param>
    /// <param name='exponent'>The binary exponent.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='mantissa'/> or <paramref name='exponent'/> is
    /// null.</exception>
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

    /// <summary>Creates a binary float from a string that represents a
    /// number. Note that if the string contains a negative exponent, the
    /// resulting value might not be exact. However, the resulting binary
    /// float will contain enough precision to accurately convert it to a
    /// 32-bit or 64-bit floating point number (float or double).
    /// <para>The format of the string generally consists of:</para>
    /// <list type=''>
    /// <item>An optional plus sign ("+" , U+002B) or minus sign ("-",
    /// U+002D) (if '-' , the value is negative.)</item>
    /// <item>One or more digits, with a single optional decimal point
    /// after the first digit and before the last digit.</item>
    /// <item>Optionally, "E+" (positive exponent) or "E-" (negative
    /// exponent) plus one or more digits specifying the
    /// exponent.</item></list>
    /// <para>The string can also be "-INF", "-Infinity" , "Infinity",
    /// "INF", quiet NaN ("NaN") followed by any number of digits, or
    /// signaling NaN ("sNaN") followed by any number of digits, all in any
    /// combination of upper and lower case.</para>
    /// <para>All characters mentioned above are the corresponding
    /// characters in the Basic Latin range. In particular, the digits must
    /// be the basic digits 0 to 9 (U + 0030 to U + 0039). The string is
    /// not allowed to contain white space characters, including
    /// spaces.</para></summary>
    /// <param name='str'>A String object.</param>
    /// <param name='offset'>A zero-based index showing where the desired
    /// portion of <paramref name='str'/> begins.</param>
    /// <param name='length'>The length, in code units, of the desired
    /// portion of <paramref name='str'/> (but not more than <paramref
    /// name='str'/> 's length).</param>
    /// <param name='ctx'>A PrecisionContext object specifying the
    /// precision, rounding, and exponent range to apply to the parsed
    /// number. Can be null.</param>
    /// <returns>The parsed number, converted to ExtendedFloat.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='length'/> is less than 0 or
    /// greater than <paramref name='str'/> 's length, or <paramref
    /// name='str'/> 's length minus <paramref name='offset'/> is less than
    /// <paramref name='length'/>.</exception>
    public static ExtendedFloat FromString(string str,
      int offset,
      int length,
      PrecisionContext ctx) {
      return new ExtendedFloat(EFloat.FromString(str, offset, length,
        ctx == null ? null : ctx.ec));
    }

    /// <summary>Creates a binary float from a string that represents a
    /// number. See the four-parameter FromString method.</summary>
    /// <param name='str'>A String object.</param>
    /// <returns>The parsed number, converted to ExtendedFloat.</returns>
    /// <example>
    ///  The following example converts a number in the form of
    /// a string to a
    /// <c>double</c>
    ///  , or a 64-bit floating point number.
    /// <code>
    /// public static double StringToDouble(String str) {
    ///  return ExtendedFloat.FromString(str).ToDouble();
    /// }
    /// </code>
    /// </example>
    public static ExtendedFloat FromString(string str) {
      return new ExtendedFloat(EFloat.FromString(str));
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A String object.</param>
    /// <param name='ctx'>A PrecisionContext object specifying the
    /// precision, rounding, and exponent range to apply to the parsed
    /// number. Can be null.</param>
    /// <returns>The parsed number, converted to ExtendedFloat.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public static ExtendedFloat FromString(string str, PrecisionContext ctx) {
   return new ExtendedFloat(EFloat.FromString(str, ctx == null ? null :
        ctx.ec));
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A String object.</param>
    /// <param name='offset'>A zero-based index showing where the desired
    /// portion of <paramref name='str'/> begins.</param>
    /// <param name='length'>The length, in code units, of the desired
    /// portion of <paramref name='str'/> (but not more than <paramref
    /// name='str'/> 's length).</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='length'/> is less than 0 or
    /// greater than <paramref name='str'/> 's length, or <paramref
    /// name='str'/> 's length minus <paramref name='offset'/> is less than
    /// <paramref name='length'/>.</exception>
    public static ExtendedFloat FromString(string str, int offset, int length) {
      return new ExtendedFloat(EFloat.FromString(str, offset, length));
    }

    /// <summary>Converts this value to an arbitrary-precision integer. Any
    /// fractional part of this value will be discarded when converting to
    /// a big integer.</summary>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='OverflowException'>This object's value is infinity
    /// or NaN.</exception>
    public BigInteger ToBigInteger() {
      return new BigInteger(this.ef.ToBigInteger());
    }

    /// <summary>Converts this value to an arbitrary-precision integer,
    /// checking whether the value contains a fractional part.</summary>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='OverflowException'>This object's value is infinity
    /// or NaN.</exception>
    /// <exception cref='ArithmeticException'>This object's value is not an
    /// exact integer.</exception>
    public BigInteger ToBigIntegerExact() {
      return new BigInteger(this.ef.ToBigIntegerExact());
    }

    /// <summary>Converts this value to a 32-bit floating-point number. The
    /// half-even rounding mode is used.
    /// <para>If this value is a NaN, sets the high bit of the 32-bit
    /// floating point number's mantissa for a quiet NaN, and clears it for
    /// a signaling NaN. Then the next highest bit of the mantissa is
    /// cleared for a quiet NaN, and set for a signaling NaN. Then the
    /// other bits of the mantissa are set to the lowest bits of this
    /// object's unsigned mantissa.</para></summary>
    /// <returns>The closest 32-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 32-bit floating point
    /// number.</returns>
    public float ToSingle() {
      return this.ef.ToSingle();
    }

    /// <summary>Converts this value to a 64-bit floating-point number. The
    /// half-even rounding mode is used. <para>If this value is a NaN, sets
    /// the high bit of the 64-bit floating point number's mantissa for a
    /// quiet NaN, and clears it for a signaling NaN. Then the next highest
    /// bit of the mantissa is cleared for a quiet NaN, and set for a
    /// signaling NaN. Then the other bits of the mantissa are set to the
    /// lowest bits of this object's unsigned mantissa.</para>
    ///  </summary>
    /// <returns>The closest 64-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 64-bit floating point
    /// number.</returns>
    /// <example>
    ///  The following example converts a number in the form of
    /// a string to a
    /// <c>double</c>
    ///  , or a 64-bit floating point number.
    /// <code>
    /// public static double StringToDouble(String str) {
    ///  return ExtendedFloat.FromString(str).ToDouble();
    /// }
    /// </code>
    /// </example>
    /// <example>
    ///  The following example converts a big integer to a
    /// <c>double</c>
    ///  , or a 64-bit floating point number.
    /// <code>
    /// public static double BigIntegerToDouble(BigInteger bigInteger) {
    ///  return ExtendedFloat.FromBigInteger(bigInteger).ToDouble();
    /// }
    /// </code>
    /// </example>
    public double ToDouble() {
      return this.ef.ToDouble();
    }

    /// <summary>Creates a binary float from a 32-bit floating-point
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting
    /// the floating point number to a string first.</summary>
    /// <param name='flt'>A 32-bit floating-point number.</param>
    /// <returns>A binary float with the same value as <paramref
    /// name='flt'/>.</returns>
    public static ExtendedFloat FromSingle(float flt) {
      return new ExtendedFloat(EFloat.FromSingle(flt));
    }

    /// <summary>Converts a big integer to the same value as a binary
    /// float.</summary>
    /// <param name='bigint'>A BigInteger object.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigint'/> is null.</exception>
    public static ExtendedFloat FromBigInteger(BigInteger bigint) {
      if ((bigint) == null) {
        throw new ArgumentNullException("bigint");
      }
      return new ExtendedFloat(EFloat.FromBigInteger(bigint.ei));
    }

    /// <summary>Converts a 64-bit integer to the same value as a binary
    /// float.</summary>
    /// <param name='valueSmall'>A 64-bit signed integer.</param>
    /// <returns>An ExtendedFloat object.</returns>
    public static ExtendedFloat FromInt64(long valueSmall) {
      return new ExtendedFloat(EFloat.FromInt64(valueSmall));
    }

    /// <summary>Creates a binary float from a 32-bit signed
    /// integer.</summary>
    /// <param name='valueSmaller'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public static ExtendedFloat FromInt32(int valueSmaller) {
      return new ExtendedFloat(EFloat.FromInt32(valueSmaller));
    }

    /// <summary>Creates a binary float from a 64-bit floating-point
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting
    /// the floating point number to a string first.</summary>
    /// <param name='dbl'>A 64-bit floating-point number.</param>
    /// <returns>A binary float with the same value as <paramref
    /// name='dbl'/>.</returns>
    public static ExtendedFloat FromDouble(double dbl) {
      return new ExtendedFloat(EFloat.FromDouble(dbl));
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal ToExtendedDecimal() {
      return new ExtendedDecimal(this.ef.ToExtendedDecimal());
    }

    /// <summary>Converts this value to a string.</summary>
    /// <returns>A string representation of this object. The value is
    /// converted to decimal and the decimal form of this number's value is
    /// returned.</returns>
    public override string ToString() {
      return this.ef.ToString();
    }

    /// <summary>Converts this value to an extended decimal, then returns
    /// the value of that decimal's ToEngineeringString method.</summary>
    /// <returns>A string representation of this object.</returns>
    public string ToEngineeringString() {
      return this.ef.ToEngineeringString();
    }

    /// <summary>Converts this value to a string in decimal form, but
    /// without exponential notation.</summary>
    /// <returns>A string representation of this object.</returns>
    public string ToPlainString() {
      return this.ef.ToPlainString();
    }

    /// <summary>Represents the number 1.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif
    public static readonly ExtendedFloat One =
      new ExtendedFloat(EFloat.One);

    /// <summary>Represents the number 0.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif
    public static readonly ExtendedFloat Zero =
      new ExtendedFloat(EFloat.Zero);

    /// <summary>Represents the number negative zero.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif
    public static readonly ExtendedFloat NegativeZero =
      new ExtendedFloat(EFloat.NegativeZero);

    /// <summary>Represents the number 10.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif

    public static readonly ExtendedFloat Ten =
      new ExtendedFloat(EFloat.Ten);

    //----------------------------------------------------------------

    /// <summary>A not-a-number value.</summary>
    public static readonly ExtendedFloat NaN =
      new ExtendedFloat(EFloat.NaN);

    /// <summary>A not-a-number value that signals an invalid operation
    /// flag when it&#x27;s passed as an argument to any arithmetic
    /// operation in ExtendedFloat.</summary>
    public static readonly ExtendedFloat SignalingNaN =
      new ExtendedFloat(EFloat.SignalingNaN);

    /// <summary>Positive infinity, greater than any other
    /// number.</summary>
    public static readonly ExtendedFloat PositiveInfinity =
      new ExtendedFloat(EFloat.PositiveInfinity);

    /// <summary>Negative infinity, less than any other number.</summary>
    public static readonly ExtendedFloat NegativeInfinity =
      new ExtendedFloat(EFloat.NegativeInfinity);

    /// <summary>Returns whether this object is negative
    /// infinity.</summary>
    /// <returns>True if this object is negative infinity; otherwise,
    /// false.</returns>
    public bool IsNegativeInfinity() {
      return this.ef.IsNegativeInfinity();
    }

    /// <summary>Returns whether this object is positive
    /// infinity.</summary>
    /// <returns>True if this object is positive infinity; otherwise,
    /// false.</returns>
    public bool IsPositiveInfinity() {
      return this.ef.IsPositiveInfinity();
    }

    /// <summary>Returns whether this object is a not-a-number
    /// value.</summary>
    /// <returns>True if this object is a not-a-number value; otherwise,
    /// false.</returns>
    public bool IsNaN() {
      return this.ef.IsNaN();
    }

    /// <summary>Gets a value indicating whether this object is positive or
    /// negative infinity.</summary>
    /// <returns>True if this object is positive or negative infinity;
    /// otherwise, false.</returns>
    public bool IsInfinity() {
      return this.ef.IsInfinity();
    }

    /// <summary>Gets a value indicating whether this object is finite (not
    /// infinity or NaN).</summary>
    /// <value>True if this object is finite (not infinity or NaN);
    /// otherwise, false.</value>
    public bool IsFinite {
      get {
        return this.ef.IsFinite;
      }
    }

    /// <summary>Gets a value indicating whether this object is negative,
    /// including negative zero.</summary>
    /// <value>True if this object is negative, including negative zero;
    /// otherwise, false.</value>
    public bool IsNegative {
      get {
        return this.ef.IsNegative;
      }
    }

    /// <summary>Gets a value indicating whether this object is a quiet
    /// not-a-number value.</summary>
    /// <returns>True if this object is a quiet not-a-number value;
    /// otherwise, false.</returns>
    public bool IsQuietNaN() {
      return this.ef.IsQuietNaN();
    }

    /// <summary>Gets a value indicating whether this object is a signaling
    /// not-a-number value.</summary>
    /// <returns>True if this object is a signaling not-a-number value;
    /// otherwise, false.</returns>
    public bool IsSignalingNaN() {
      return this.ef.IsSignalingNaN();
    }

    /// <summary>Gets this value&#x27;s sign: -1 if negative; 1 if
    /// positive; 0 if zero.</summary>
    /// <value>This value&apos;s sign: -1 if negative; 1 if positive; 0 if
    /// zero.</value>
    public int Sign {
      get {
        return this.ef.Sign;
      }
    }

    /// <summary>Gets a value indicating whether this object&#x27;s value
    /// equals 0.</summary>
    /// <value>True if this object&apos;s value equals 0; otherwise,
    /// false.</value>
    public bool IsZero {
      get {
        return this.ef.IsZero;
      }
    }

    /// <summary>Gets the absolute value of this object.</summary>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat Abs() {
      return new ExtendedFloat(this.ef.Abs());
    }

    /// <summary>Gets an object with the same value as this one, but with
    /// the sign reversed.</summary>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat Negate() {
      return new ExtendedFloat(this.ef.Negate());
    }

    /// <summary>Divides this object by another binary float and returns
    /// the result. When possible, the result will be exact.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The quotient of the two numbers. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Signals FlagInvalid and returns NaN if the divisor and the
    /// dividend are 0.</returns>
    /// <exception cref='ArithmeticException'>The result can't be exact
    /// because it would have a nonterminating binary
    /// expansion.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    public ExtendedFloat Divide(ExtendedFloat divisor) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.Divide(divisor.ef));
    }

    /// <summary>Divides this object by another binary float and returns a
    /// result with the same exponent as this object (the
    /// dividend).</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two numbers. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Signals FlagInvalid and returns NaN if the divisor and the
    /// dividend are 0.</returns>
    /// <exception cref='ArithmeticException'>The rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    public ExtendedFloat DivideToSameExponent(ExtendedFloat divisor,
      Rounding rounding) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.DivideToSameExponent(divisor.ef,
        ExtendedDecimal.ToERounding(rounding)));
    }

    /// <summary>Divides two ExtendedFloat objects, and returns the integer
    /// part of the result, rounded down, with the preferred exponent set
    /// to this value&#x27;s exponent minus the divisor&#x27;s
    /// exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The integer part of the quotient of the two objects.
    /// Signals FlagDivideByZero and returns infinity if the divisor is 0
    /// and the dividend is nonzero. Signals FlagInvalid and returns NaN if
    /// the divisor and the dividend are 0.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    public ExtendedFloat DivideToIntegerNaturalScale(ExtendedFloat divisor) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.DivideToIntegerNaturalScale(divisor.ef));
    }

    /// <summary>Removes trailing zeros from this object&#x27;s mantissa.
    /// For example, 1.000 becomes 1.
    /// <para>If this object's value is 0, changes the exponent to 0. (This
    /// is unlike the behavior in Java's BigDecimal method
    /// "stripTrailingZeros" in Java 7 and earlier.)</para></summary>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>This value with trailing zeros removed. Note that if the
    /// result has a very high exponent and the context says to clamp high
    /// exponents, there may still be some trailing zeros in the
    /// mantissa.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat Reduce(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.Reduce(ctx == null ? null : ctx.ec));
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='divisor'>Another ExtendedFloat object.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    public ExtendedFloat RemainderNaturalScale(ExtendedFloat divisor) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.RemainderNaturalScale(divisor.ef));
    }

    /// <summary>Calculates the remainder of a number by the formula this -
    /// ((this / divisor) * divisor). This is meant to be similar to the
    /// remainder operation in Java's BigDecimal.</summary>
    /// <param name='divisor'>Another ExtendedFloat object.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision, rounding, and exponent range of the integer part of the
    /// result. This context will be used only in the division portion of
    /// the remainder calculation. Flags will be set on the given context
    /// only if the context's HasFlags is true and the integer part of the
    /// division result doesn't fit the precision and exponent range
    /// without rounding.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Divides two ExtendedFloat objects, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedFloat object.</param>
    /// <param name='desiredExponentSmall'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal
    /// point. A positive number places the cutoff point to the left of the
    /// usual decimal point.</param>
    /// <param name='ctx'>A precision context object to control the
    /// rounding mode to use if the result must be scaled down to have the
    /// same exponent as this value. If the precision given in the context
    /// is other than 0, calls the Quantize method with both arguments
    /// equal to the result of the operation (and can signal FlagInvalid
    /// and return NaN if the result doesn't fit the given precision). If
    /// HasFlags of the context is true, will also store the flags
    /// resulting from the operation (the flags are in addition to the
    /// pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Signals FlagInvalid and returns NaN if the divisor and the
    /// dividend are 0. Signals FlagInvalid and returns NaN if the context
    /// defines an exponent range and the desired exponent is outside that
    /// range.</returns>
    /// <exception cref='ArithmeticException'>The rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Divides this ExtendedFloat object by another ExtendedFloat
    /// object. The preferred exponent for the result is this object&#x27;s
    /// exponent minus the divisor&#x27;s exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Signals FlagInvalid and returns NaN if the divisor and the
    /// dividend are 0.</returns>
    /// <exception cref='ArithmeticException'>Either <paramref name='ctx'/>
    /// is null or <paramref name='ctx'/> 's precision is 0, and the result
    /// would have a nonterminating binary expansion; or, the rounding mode
    /// is Rounding.Unnecessary and the result is not exact.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> or <paramref name='ctx'/> is null.</exception>
    public ExtendedFloat Divide(ExtendedFloat divisor,
      PrecisionContext ctx) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.Divide(divisor.ef, ctx == null ? null:
        ctx.ec));
    }

    /// <summary>Divides two ExtendedFloat objects, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedFloat object.</param>
    /// <param name='desiredExponentSmall'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal
    /// point. A positive number places the cutoff point to the left of the
    /// usual decimal point.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Signals FlagInvalid and returns NaN if the divisor and the
    /// dividend are 0.</returns>
    /// <exception cref='ArithmeticException'>The rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    public ExtendedFloat DivideToExponent(ExtendedFloat divisor,
      long desiredExponentSmall,
      Rounding rounding) {
      if ((divisor) == null) {
        throw new ArgumentNullException("divisor");
      }
      return new ExtendedFloat(this.ef.DivideToExponent(divisor.ef,
        desiredExponentSmall, ExtendedDecimal.ToERounding(rounding)));
    }

    /// <summary>Divides two ExtendedFloat objects, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedFloat object.</param>
    /// <param name='exponent'>The desired exponent. A negative number
    /// places the cutoff point to the right of the usual decimal point. A
    /// positive number places the cutoff point to the left of the usual
    /// decimal point.</param>
    /// <param name='ctx'>A precision context object to control the
    /// rounding mode to use if the result must be scaled down to have the
    /// same exponent as this value. If the precision given in the context
    /// is other than 0, calls the Quantize method with both arguments
    /// equal to the result of the operation (and can signal FlagInvalid
    /// and return NaN if the result doesn't fit the given precision). If
    /// HasFlags of the context is true, will also store the flags
    /// resulting from the operation (the flags are in addition to the
    /// pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Signals FlagInvalid and returns NaN if the divisor and the
    /// dividend are 0. Signals FlagInvalid and returns NaN if the context
    /// defines an exponent range and the desired exponent is outside that
    /// range.</returns>
    /// <exception cref='ArithmeticException'>The rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> or <paramref name='exponent'/> or <paramref
    /// name='ctx'/> is null.</exception>
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

    /// <summary>Divides two ExtendedFloat objects, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedFloat object.</param>
    /// <param name='desiredExponent'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal
    /// point. A positive number places the cutoff point to the left of the
    /// usual decimal point.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Signals FlagInvalid and returns NaN if the divisor and the
    /// dividend are 0.</returns>
    /// <exception cref='ArithmeticException'>The rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> or <paramref name='desiredExponent'/> is
    /// null.</exception>
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

    /// <summary>Finds the absolute value of this object (if it&#x27;s
    /// negative, it becomes positive).</summary>
    /// <param name='context'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The absolute value of this object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='context'/> is null.</exception>
    public ExtendedFloat Abs(PrecisionContext context) {
    return new ExtendedFloat(this.ef.Abs(context == null ? null :
        context.ec));
    }

    /// <summary>Returns a binary float with the same value as this object
    /// but with the sign reversed.</summary>
    /// <param name='context'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='context'/> is null.</exception>
    public ExtendedFloat Negate(PrecisionContext context) {
      if ((context) == null) {
        throw new ArgumentNullException("context");
      }
   return new ExtendedFloat(this.ef.Negate(context == null ? null :
        context.ec));
    }

    /// <summary>Adds this object and another binary float and returns the
    /// result.</summary>
    /// <param name='otherValue'>An ExtendedFloat object.</param>
    /// <returns>The sum of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ExtendedFloat Add(ExtendedFloat otherValue) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedFloat(this.ef.Add(otherValue.ef));
    }

    /// <summary>Subtracts an ExtendedFloat object from this instance and
    /// returns the result..</summary>
    /// <param name='otherValue'>An ExtendedFloat object.</param>
    /// <returns>The difference of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ExtendedFloat Subtract(ExtendedFloat otherValue) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedFloat(this.ef.Subtract(otherValue.ef));
    }

    /// <summary>Subtracts an ExtendedFloat object from this
    /// instance.</summary>
    /// <param name='otherValue'>An ExtendedFloat object.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The difference of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
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

    /// <summary>Multiplies two binary floats. The resulting exponent will
    /// be the sum of the exponents of the two binary floats.</summary>
    /// <param name='otherValue'>Another binary float.</param>
    /// <returns>The product of the two binary floats.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ExtendedFloat Multiply(ExtendedFloat otherValue) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedFloat(this.ef.Multiply(otherValue.ef));
    }

    /// <summary>Multiplies by one binary float, and then adds another
    /// binary float.</summary>
    /// <param name='multiplicand'>The value to multiply.</param>
    /// <param name='augend'>The value to add.</param>
    /// <returns>The result this * multiplicand + augend.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='multiplicand'/> or <paramref name='augend'/> is
    /// null.</exception>
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

    /// <summary>Divides this object by another object, and returns the
    /// integer part of the result, with the preferred exponent set to this
    /// value&#x27;s exponent minus the divisor&#x27;s exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision, rounding, and exponent range of the integer part of the
    /// result. Flags will be set on the given context only if the
    /// context's HasFlags is true and the integer part of the result
    /// doesn't fit the precision and exponent range without
    /// rounding.</param>
    /// <returns>The integer part of the quotient of the two objects.
    /// Returns null if the return value would overflow the exponent range.
    /// A caller can handle a null return value by treating it as positive
    /// infinity if both operands have the same sign or as negative
    /// infinity if both operands have different signs. Signals
    /// FlagDivideByZero and returns infinity if the divisor is 0 and the
    /// dividend is nonzero. Signals FlagInvalid and returns NaN if the
    /// divisor and the dividend are 0.</returns>
    /// <exception cref='ArithmeticException'>The rounding mode is
    /// Rounding.Unnecessary and the integer part of the result is not
    /// exact.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Divides this object by another object, and returns the
    /// integer part of the result, with the exponent set to 0.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision. The rounding and exponent range settings of this context
    /// are ignored. If HasFlags of the context is true, will also store
    /// the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags). Can be null.</param>
    /// <returns>The integer part of the quotient of the two objects. The
    /// exponent will be set to 0. Signals FlagDivideByZero and returns
    /// infinity if the divisor is 0 and the dividend is nonzero. Signals
    /// FlagInvalid and returns NaN if the divisor and the dividend are 0,
    /// or if the result doesn't fit the given precision.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Finds the remainder that results when dividing two
    /// ExtendedFloat objects.</summary>
    /// <param name='divisor'>An ExtendedFloat object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The remainder of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Finds the distance to the closest multiple of the given
    /// divisor, based on the result of dividing this object&#x27;s value
    /// by another object&#x27;s value.
    /// <list type=''>
    /// <item>If this and the other object divide evenly, the result is
    /// 0.</item>
    /// <item>If the remainder's absolute value is less than half of the
    /// divisor's absolute value, the result has the same sign as this
    /// object and will be the distance to the closest multiple.</item>
    /// <item>If the remainder's absolute value is more than half of the
    /// divisor' s absolute value, the result has the opposite sign of this
    /// object and will be the distance to the closest multiple.</item>
    /// <item>If the remainder's absolute value is exactly half of the
    /// divisor's absolute value, the result has the opposite sign of this
    /// object if the quotient, rounded down, is odd, and has the same sign
    /// as this object if the quotient, rounded down, is even, and the
    /// result's absolute value is half of the divisor's absolute
    /// value.</item></list> This function is also known as the "IEEE
    /// Remainder" function.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision. The rounding and exponent range settings of this context
    /// are ignored (the rounding mode is always treated as HalfEven). If
    /// HasFlags of the context is true, will also store the flags
    /// resulting from the operation (the flags are in addition to the
    /// pre-existing flags). Can be null.</param>
    /// <returns>The distance of the closest multiple. Signals FlagInvalid
    /// and returns NaN if the divisor is 0, or either the result of
    /// integer division (the quotient) or the remainder wouldn't fit the
    /// given precision.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Finds the largest value that&#x27;s smaller than the given
    /// value.</summary>
    /// <param name='ctx'>A precision context object to control the
    /// precision and exponent range of the result. The rounding mode from
    /// this context is ignored. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags).</param>
    /// <returns>Returns the largest value that's less than the given
    /// value. Returns negative infinity if the result is negative
    /// infinity.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null, the precision is 0, or <paramref name='ctx'/>
    /// has an unlimited exponent range.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat NextMinus(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.NextMinus(ctx == null ? null : ctx.ec));
    }

    /// <summary>Finds the smallest value that&#x27;s greater than the
    /// given value.</summary>
    /// <param name='ctx'>A precision context object to control the
    /// precision and exponent range of the result. The rounding mode from
    /// this context is ignored. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags).</param>
    /// <returns>Returns the smallest value that's greater than the given
    /// value.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null, the precision is 0, or <paramref name='ctx'/>
    /// has an unlimited exponent range.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat NextPlus(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.NextPlus(ctx == null ? null : ctx.ec));
    }

    /// <summary>Finds the next value that is closer to the other
    /// object&#x27;s value than this object&#x27;s value.</summary>
    /// <param name='otherValue'>An ExtendedFloat object.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision and exponent range of the result. The rounding mode from
    /// this context is ignored. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags).</param>
    /// <returns>Returns the next value that is closer to the other object'
    /// s value than this object's value.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null, the precision is 0, or <paramref name='ctx'/>
    /// has an unlimited exponent range.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Gets the greater value between two binary
    /// floats.</summary>
    /// <param name='first'>An ExtendedFloat object.</param>
    /// <param name='second'>Another ExtendedFloat object.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The larger value of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> or <paramref
    /// name='ctx'/> is null.</exception>
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

    /// <summary>Gets the lesser value between two binary floats.</summary>
    /// <param name='first'>An ExtendedFloat object.</param>
    /// <param name='second'>Another ExtendedFloat object.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The smaller value of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> or <paramref
    /// name='ctx'/> is null.</exception>
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

    /// <summary>Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as
    /// Max.</summary>
    /// <param name='first'>Another ExtendedFloat object.</param>
    /// <param name='second'>An ExtendedFloat object. (3).</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> or <paramref
    /// name='ctx'/> is null.</exception>
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

    /// <summary>Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as
    /// Min.</summary>
    /// <param name='first'>Another ExtendedFloat object.</param>
    /// <param name='second'>An ExtendedFloat object. (3).</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> or <paramref
    /// name='ctx'/> is null.</exception>
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

    /// <summary>Gets the greater value between two binary
    /// floats.</summary>
    /// <param name='first'>An ExtendedFloat object.</param>
    /// <param name='second'>Another ExtendedFloat object.</param>
    /// <returns>The larger value of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> is null.</exception>
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

    /// <summary>Gets the lesser value between two binary floats.</summary>
    /// <param name='first'>An ExtendedFloat object.</param>
    /// <param name='second'>Another ExtendedFloat object.</param>
    /// <returns>The smaller value of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> is null.</exception>
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

    /// <summary>Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as
    /// Max.</summary>
    /// <param name='first'>Another ExtendedFloat object.</param>
    /// <param name='second'>An ExtendedFloat object. (3).</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> is null.</exception>
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

    /// <summary>Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as
    /// Min.</summary>
    /// <param name='first'>Another ExtendedFloat object.</param>
    /// <param name='second'>An ExtendedFloat object. (3).</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> is null.</exception>
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

    /// <summary>Compares the mathematical values of this object and
    /// another object, accepting NaN values.
    /// <para>This method is not consistent with the Equals method because
    /// two different numbers with the same mathematical value, but
    /// different exponents, will compare as equal.</para>
    /// <para>In this method, negative zero and positive zero are
    /// considered equal.</para>
    /// <para>If this object or the other object is a quiet NaN or
    /// signaling NaN, this method will not trigger an error. Instead, NaN
    /// will compare greater than any other number, including infinity. Two
    /// different NaN values will be considered equal.</para></summary>
    /// <param name='other'>An ExtendedFloat object.</param>
    /// <returns>Less than 0 if this object's value is less than the other
    /// value, or greater than 0 if this object's value is greater than the
    /// other value or if <paramref name='other'/> is null, or 0 if both
    /// values are equal.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='other'/> is null.</exception>
    public int CompareTo(ExtendedFloat other) {
      if ((other) == null) {
        throw new ArgumentNullException("other");
      }
      return this.ef.CompareTo(other.ef);
    }

    /// <summary>Compares the mathematical values of this object and
    /// another object.
    /// <para>In this method, negative zero and positive zero are
    /// considered equal.</para>
    /// <para>If this object or the other object is a quiet NaN or
    /// signaling NaN, this method returns a quiet NaN, and will signal a
    /// FlagInvalid flag if either is a signaling NaN.</para></summary>
    /// <param name='other'>An ExtendedFloat object.</param>
    /// <param name='ctx'>A precision context. The precision, rounding, and
    /// exponent range are ignored. If HasFlags of the context is true,
    /// will store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null.</param>
    /// <returns>Quiet NaN if this object or the other object is NaN, or 0
    /// if both objects have the same value, or -1 if this object is less
    /// than the other value, or 1 if this object is greater.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='other'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Compares the mathematical values of this object and
    /// another object, treating quiet NaN as signaling.
    /// <para>In this method, negative zero and positive zero are
    /// considered equal.</para>
    /// <para>If this object or the other object is a quiet NaN or
    /// signaling NaN, this method will return a quiet NaN and will signal
    /// a FlagInvalid flag.</para></summary>
    /// <param name='other'>An ExtendedFloat object.</param>
    /// <param name='ctx'>A precision context. The precision, rounding, and
    /// exponent range are ignored. If HasFlags of the context is true,
    /// will store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null.</param>
    /// <returns>Quiet NaN if this object or the other object is NaN, or 0
    /// if both objects have the same value, or -1 if this object is less
    /// than the other value, or 1 if this object is greater.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='other'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Finds the sum of this object and another object. The
    /// result&#x27;s exponent is set to the lower of the exponents of the
    /// two operands.</summary>
    /// <param name='otherValue'>The number to add to.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The sum of thisValue and the other object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Returns a binary float with the same value but a new
    /// exponent.</summary>
    /// <param name='desiredExponent'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A binary float with the same value as this object but with
    /// the exponent changed. Signals FlagInvalid and returns NaN if an
    /// overflow error occurred, or the rounded result can't fit the given
    /// precision, or if the context defines an exponent range and the
    /// given exponent is outside that range.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='desiredExponent'/> or <paramref name='ctx'/> is
    /// null.</exception>
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

    /// <summary>Returns a binary float with the same value but a new
    /// exponent.</summary>
    /// <param name='desiredExponentSmall'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A binary float with the same value as this object but with
    /// the exponent changed. Signals FlagInvalid and returns NaN if an
    /// overflow error occurred, or the rounded result can't fit the given
    /// precision, or if the context defines an exponent range and the
    /// given exponent is outside that range.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat Quantize(int desiredExponentSmall,
      PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.Quantize(desiredExponentSmall,
        ctx == null ? null : ctx.ec));
    }

    /// <summary>Returns a binary float with the same value as this object
    /// but with the same exponent as another binary float.</summary>
    /// <param name='otherValue'>A binary float containing the desired
    /// exponent of the result. The mantissa is ignored. The exponent is
    /// the number of fractional digits in the result, expressed as a
    /// negative number. Can also be positive, which eliminates lower-order
    /// places from the number. For example, -3 means round to the
    /// thousandth (10^-3, 0.0001), and 3 means round to the thousand
    /// (10^3, 1000). A value of 0 rounds the number to an integer.</param>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null, in which case the
    /// default rounding mode is HalfEven.</param>
    /// <returns>A binary float with the same value as this object but with
    /// the exponent changed. Signals FlagInvalid and returns NaN if an
    /// overflow error occurred, or the result can't fit the given
    /// precision without rounding. Signals FlagInvalid and returns NaN if
    /// the new exponent is outside of the valid range of the precision
    /// context, if it defines an exponent range.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> or <paramref name='ctx'/> is null.</exception>
    public ExtendedFloat Quantize(ExtendedFloat otherValue,
      PrecisionContext ctx) {
      if ((otherValue) == null) {
        throw new ArgumentNullException("otherValue");
      }
      return new ExtendedFloat(this.ef.Quantize(otherValue.ef, ctx == null ?
        null : ctx.ec));
    }

    /// <summary>Returns a binary number with the same value as this object
    /// but rounded to an integer, and signals an invalid operation if the
    /// result would be inexact.</summary>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null, in which case the
    /// default rounding mode is HalfEven.</param>
    /// <returns>A binary number rounded to the closest integer
    /// representable in the given precision. Signals FlagInvalid and
    /// returns NaN if the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the precision
    /// context defines an exponent range, the new exponent must be changed
    /// to 0 when rounding, and 0 is outside of the valid range of the
    /// precision context.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat RoundToIntegralExact(PrecisionContext ctx) {
      return new ExtendedFloat(this.ef.RoundToIntegralExact(ctx == null ? null:
        ctx.ec));
    }

    /// <summary>Returns a binary number with the same value as this object
    /// but rounded to an integer, without adding the FlagInexact or
    /// FlagRounded flags.</summary>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags), except that this function will
    /// never add the FlagRounded and FlagInexact flags (the only
    /// difference between this and RoundToExponentExact). Can be null, in
    /// which case the default rounding mode is HalfEven.</param>
    /// <returns>A binary number rounded to the closest integer
    /// representable in the given precision, meaning if the result can't
    /// fit the precision, additional digits are discarded to make it fit.
    /// Signals FlagInvalid and returns NaN if the precision context
    /// defines an exponent range, the new exponent must be changed to 0
    /// when rounding, and 0 is outside of the valid range of the precision
    /// context.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat RoundToIntegralNoRoundedFlag(PrecisionContext ctx) {
      return new
        ExtendedFloat(this.ef.RoundToIntegralNoRoundedFlag(ctx == null ? null:
        ctx.ec));
    }

    /// <summary>Returns a binary number with the same value as this object
    /// but rounded to an integer, and signals an invalid operation if the
    /// result would be inexact.</summary>
    /// <param name='exponent'>The minimum exponent the result can have.
    /// This is the maximum number of fractional digits in the result,
    /// expressed as a negative number. Can also be positive, which
    /// eliminates lower-order places from the number. For example, -3
    /// means round to the sixteenth (10b^-3, 0.0001b), and 3 means round
    /// to the sixteen-place (10b^3, 1000b). A value of 0 rounds the number
    /// to an integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null, in which case the default rounding mode is
    /// HalfEven.</param>
    /// <returns>A binary number rounded to the closest value representable
    /// in the given precision. Signals FlagInvalid and returns NaN if the
    /// result can't fit the given precision without rounding. Signals
    /// FlagInvalid and returns NaN if the precision context defines an
    /// exponent range, the new exponent must be changed to the given
    /// exponent when rounding, and the given exponent is outside of the
    /// valid range of the precision context.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='exponent'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Returns a binary number with the same value as this
    /// object, and rounds it to a new exponent if necessary.</summary>
    /// <param name='exponent'>The minimum exponent the result can have.
    /// This is the maximum number of fractional digits in the result,
    /// expressed as a negative number. Can also be positive, which
    /// eliminates lower-order places from the number. For example, -3
    /// means round to the sixteenth (10b^-3, 0.0001b), and 3 means round
    /// to the sixteen-place (10b^3, 1000b). A value of 0 rounds the number
    /// to an integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null, in which case the default rounding mode is
    /// HalfEven.</param>
    /// <returns>A binary number rounded to the closest value representable
    /// in the given precision, meaning if the result can't fit the
    /// precision, additional digits are discarded to make it fit. Signals
    /// FlagInvalid and returns NaN if the precision context defines an
    /// exponent range, the new exponent must be changed to the given
    /// exponent when rounding, and the given exponent is outside of the
    /// valid range of the precision context.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='exponent'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Returns a binary number with the same value as this object
    /// but rounded to an integer, and signals an invalid operation if the
    /// result would be inexact.</summary>
    /// <param name='exponentSmall'>The minimum exponent the result can
    /// have. This is the maximum number of fractional digits in the
    /// result, expressed as a negative number. Can also be positive, which
    /// eliminates lower-order places number. For example, -3 means round
    /// to the sixteenth (10b^-3, 0.0001b), and 3 means round to the
    /// sixteen-place (10b^3, 1000b). A value of 0 rounds the number to an
    /// integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>A binary number rounded to the closest value representable
    /// in the given precision. Signals FlagInvalid and returns NaN if the
    /// result can't fit the given precision without rounding. Signals
    /// FlagInvalid and returns NaN if the precision context defines an
    /// exponent range, the new exponent must be changed to the given
    /// exponent when rounding, and the given exponent is outside of the
    /// valid range of the precision context.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat RoundToExponentExact(int exponentSmall,
      PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
 return new ExtendedFloat(this.ef.RoundToExponentExact(exponentSmall,
        ctx.ec));
    }

    /// <summary>Returns a binary number with the same value as this
    /// object, and rounds it to a new exponent if necessary.</summary>
    /// <param name='exponentSmall'>The minimum exponent the result can
    /// have. This is the maximum number of fractional digits in the
    /// result, expressed as a negative number. Can also be positive, which
    /// eliminates lower-order places number. For example, -3 means round
    /// to the sixteenth (10b^-3, 0.0001b), and 3 means round to the
    /// sixteen-place (10b^3, 1000b). A value of 0 rounds the number to an
    /// integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null, in which case the default rounding mode is
    /// HalfEven.</param>
    /// <returns>A binary number rounded to the closest value representable
    /// in the given precision, meaning if the result can't fit the
    /// precision, additional digits are discarded to make it fit. Signals
    /// FlagInvalid and returns NaN if the precision context defines an
    /// exponent range, the new exponent must be changed to the given
    /// exponent when rounding, and the given exponent is outside of the
    /// valid range of the precision context.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat RoundToExponent(int exponentSmall,
      PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.RoundToExponent(exponentSmall,
        ctx == null ? null : ctx.ec));
    }

    /// <summary>Multiplies two binary floats. The resulting scale will be
    /// the sum of the scales of the two binary floats. The result&#x27;s
    /// sign is positive if both operands have the same sign, and negative
    /// if they have different signs.</summary>
    /// <param name='op'>Another binary float.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The product of the two binary floats.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='op'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Multiplies by one value, and then adds another
    /// value.</summary>
    /// <param name='op'>The value to multiply.</param>
    /// <param name='augend'>The value to add.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The result thisValue * multiplicand + augend.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='op'/> or <paramref name='augend'/> or <paramref name='ctx'/>
    /// is null.</exception>
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

    /// <summary>Multiplies by one value, and then subtracts another
    /// value.</summary>
    /// <param name='op'>The value to multiply.</param>
    /// <param name='subtrahend'>The value to subtract.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The result thisValue * multiplicand -
    /// subtrahend.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='op'/> or <paramref name='subtrahend'/> is null.</exception>
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

    /// <summary>Rounds this object&#x27;s value to a given precision,
    /// using the given rounding mode and range of exponent.</summary>
    /// <param name='ctx'>A context for controlling the precision, rounding
    /// mode, and exponent range. Can be null.</param>
    /// <returns>The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if
    /// <paramref name='ctx'/> is null or the precision and exponent range
    /// are unlimited.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat RoundToPrecision(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
 return new ExtendedFloat(this.ef.RoundToPrecision(ctx == null ? null :
        ctx.ec));
    }

    /// <summary>Rounds this object&#x27;s value to a given precision,
    /// using the given rounding mode and range of exponent, and also
    /// converts negative zero to positive zero.</summary>
    /// <param name='ctx'>A context for controlling the precision, rounding
    /// mode, and exponent range. Can be null.</param>
    /// <returns>The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if
    /// <paramref name='ctx'/> is null or the precision and exponent range
    /// are unlimited.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat Plus(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.Plus(ctx == null ? null : ctx.ec));
    }

    /// <summary>Rounds this object&#x27;s value to a given maximum bit
    /// length, using the given rounding mode and range of
    /// exponent.</summary>
    /// <param name='ctx'>A context for controlling the precision, rounding
    /// mode, and exponent range. The precision is interpreted as the
    /// maximum bit length of the mantissa. Can be null.</param>
    /// <returns>The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if
    /// <paramref name='ctx'/> is null or the precision and exponent range
    /// are unlimited.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    [Obsolete(
      "Instead of this method use RoundToPrecision and pass a precision " + "context with the IsPrecisionInBits property set.")]
    public ExtendedFloat RoundToBinaryPrecision(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.RoundToBinaryPrecision(ctx == null ?
        null : ctx.ec));
    }

    /// <summary>Finds the square root of this object&#x27;s
    /// value.</summary>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// --This parameter cannot be null, as the square root function's
    /// results are generally not exact for many inputs.--.</param>
    /// <returns>The square root. Signals the flag FlagInvalid and returns
    /// NaN if this object is less than 0 (the square root would be a
    /// complex number, but the return value is still NaN).</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null or the precision is unlimited (the context's
    /// Precision property is 0).</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat SquareRoot(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.SquareRoot(ctx == null ? null : ctx.ec));
    }

    /// <summary>Finds e (the base of natural logarithms) raised to the
    /// power of this object&#x27;s value.</summary>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// --This parameter cannot be null, as the exponential function's
    /// results are generally not exact.--.</param>
    /// <returns>Exponential of this object. If this object's value is 1,
    /// returns an approximation to " e" within the given
    /// precision.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null or the precision is unlimited (the context's
    /// Precision property is 0).</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat Exp(PrecisionContext ctx) {
      return new ExtendedFloat(this.ef.Exp(ctx == null ? null : ctx.ec));
    }

    /// <summary>Finds the natural logarithm of this object, that is, the
    /// power (exponent) that e (the base of natural logarithms) must be
    /// raised to in order to equal this object&#x27;s value.</summary>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// --This parameter cannot be null, as the ln function's results are
    /// generally not exact.--.</param>
    /// <returns>Ln(this object). Signals the flag FlagInvalid and returns
    /// NaN if this object is less than 0 (the result would be a complex
    /// number with a real part equal to Ln of this object's absolute value
    /// and an imaginary part equal to pi, but the return value is still
    /// NaN.).</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null or the precision is unlimited (the context's
    /// Precision property is 0).</exception>
    public ExtendedFloat Log(PrecisionContext ctx) {
      return new ExtendedFloat(this.ef.Log(ctx == null ? null : ctx.ec));
    }

    /// <summary>Finds the base-10 logarithm of this object, that is, the
    /// power (exponent) that the number 10 must be raised to in order to
    /// equal this object&#x27;s value.</summary>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// --This parameter cannot be null, as the ln function's results are
    /// generally not exact.--.</param>
    /// <returns>Ln(this object)/Ln(10). Signals the flag FlagInvalid and
    /// returns NaN if this object is less than 0. Signals FlagInvalid and
    /// returns NaN if the parameter <paramref name='ctx'/> is null or the
    /// precision is unlimited (the context's Precision property is
    /// 0).</returns>
    public ExtendedFloat Log10(PrecisionContext ctx) {
      return new ExtendedFloat(this.ef.Log10(ctx == null ? null : ctx.ec));
    }

    /// <summary>Raises this object&#x27;s value to the given
    /// exponent.</summary>
    /// <param name='exponent'>An ExtendedFloat object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>This^exponent. Signals the flag FlagInvalid and returns
    /// NaN if this object and exponent are both 0; or if this value is
    /// less than 0 and the exponent either has a fractional part or is
    /// infinity.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null or the precision is unlimited (the context's
    /// Precision property is 0), and the exponent has a fractional
    /// part.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='exponent'/> is null.</exception>
    public ExtendedFloat Pow(ExtendedFloat exponent, PrecisionContext ctx) {
      if ((exponent) == null) {
        throw new ArgumentNullException("exponent");
      }
 return new ExtendedFloat(this.ef.Pow(exponent.ef, ctx == null ? null :
        ctx.ec));
    }

    /// <summary>Raises this object&#x27;s value to the given
    /// exponent.</summary>
    /// <param name='exponentSmall'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing
    /// flags).</param>
    /// <returns>This^exponent. Signals the flag FlagInvalid and returns
    /// NaN if this object and exponent are both 0.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat Pow(int exponentSmall, PrecisionContext ctx) {
      return new ExtendedFloat(this.ef.Pow(exponentSmall, ctx == null ? null :
        ctx.ec));
    }

    /// <summary>Raises this object&#x27;s value to the given
    /// exponent.</summary>
    /// <param name='exponentSmall'>A 32-bit signed integer.</param>
    /// <returns>This^exponent. Returns NaN if this object and exponent are
    /// both 0.</returns>
    public ExtendedFloat Pow(int exponentSmall) {
      return new ExtendedFloat(this.ef.Pow(exponentSmall));
    }

    /// <summary>Finds the constant pi.</summary>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// --This parameter cannot be null, as pi can never be represented
    /// exactly.--.</param>
    /// <returns>Pi rounded to the given precision.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null or the precision is unlimited (the context's
    /// Precision property is 0).</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public static ExtendedFloat PI(PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(EFloat.PI(ctx == null ? null : ctx.ec));
    }

    /// <summary>Returns a number similar to this number but with the radix
    /// point moved to the left.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat MovePointLeft(int places) {
      return new ExtendedFloat(this.ef.MovePointLeft(places));
    }

    /// <summary>Returns a number similar to this number but with the radix
    /// point moved to the left.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat MovePointLeft(int places, PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.MovePointLeft(places, ctx == null ?
        null : ctx.ec));
    }

    /// <summary>Returns a number similar to this number but with the radix
    /// point moved to the left.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigPlaces'/> is null.</exception>
    public ExtendedFloat MovePointLeft(BigInteger bigPlaces) {
      if ((bigPlaces) == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedFloat(this.ef.MovePointLeft(bigPlaces.ei));
    }

    /// <summary>Returns a number similar to this number but with the radix
    /// point moved to the left.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigPlaces'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Returns a number similar to this number but with the radix
    /// point moved to the right.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat MovePointRight(int places) {
      return new ExtendedFloat(this.ef.MovePointRight(places));
    }

    /// <summary>Returns a number similar to this number but with the radix
    /// point moved to the right.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat MovePointRight(int places, PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.MovePointRight(places, ctx == null ?
        null : ctx.ec));
    }

    /// <summary>Returns a number similar to this number but with the radix
    /// point moved to the right.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigPlaces'/> is null.</exception>
    public ExtendedFloat MovePointRight(BigInteger bigPlaces) {
      if ((bigPlaces) == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedFloat(this.ef.MovePointRight(bigPlaces.ei));
    }

    /// <summary>Returns a number similar to this number but with the radix
    /// point moved to the right.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>A number whose scale is increased by <paramref
    /// name='bigPlaces'/>, but not to more than 0.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigPlaces'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Returns a number similar to this number but with the scale
    /// adjusted.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedFloat ScaleByPowerOfTwo(int places) {
      return new ExtendedFloat(this.ef.ScaleByPowerOfTwo(places));
    }

    /// <summary>Returns a number similar to this number but with the scale
    /// adjusted.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat ScaleByPowerOfTwo(int places, PrecisionContext ctx) {
      if ((ctx) == null) {
        throw new ArgumentNullException("ctx");
      }
      return new ExtendedFloat(this.ef.ScaleByPowerOfTwo(places, ctx == null ?
        null : ctx.ec));
    }

    /// <summary>Returns a number similar to this number but with the scale
    /// adjusted.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigPlaces'/> is null.</exception>
    public ExtendedFloat ScaleByPowerOfTwo(BigInteger bigPlaces) {
      if ((bigPlaces) == null) {
        throw new ArgumentNullException("bigPlaces");
      }
      return new ExtendedFloat(this.ef.ScaleByPowerOfTwo(bigPlaces.ei));
    }

    /// <summary>Returns a number similar to this number but with its scale
    /// adjusted.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>A number whose scale is increased by <paramref
    /// name='bigPlaces'/>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigPlaces'/> or <paramref name='ctx'/> is null.</exception>
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

    /// <summary>Finds the number of digits in this number's mantissa.
    /// Returns 1 if this value is 0, and 0 if this value is infinity or
    /// NaN.</summary>
    /// <returns>A BigInteger object.</returns>
    public BigInteger Precision() {
      return new BigInteger(this.ef.Precision());
    }

    /// <summary>Returns the unit in the last place. The mantissa will be 1
    /// and the exponent will be this number's exponent. Returns 1 with an
    /// exponent of 0 if this number is infinity or NaN.</summary>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat Ulp() {
      return new ExtendedFloat(this.ef.Ulp());
    }

    /// <summary>Calculates the quotient and remainder using the
    /// DivideToIntegerNaturalScale and the formula in
    /// RemainderNaturalScale.</summary>
    /// <param name='divisor'>The number to divide by.</param>
    /// <returns>A 2 element array consisting of the quotient and remainder
    /// in that order.</returns>
 public ExtendedFloat[] DivideAndRemainderNaturalScale(ExtendedFloat
      divisor) {
      EFloat[] edec = this.ef.DivideAndRemainderNaturalScale(divisor == null ? null : divisor.ef);
      return new ExtendedFloat[] {
        new ExtendedFloat(edec[0]),new ExtendedFloat(edec[1])
      };
    }

    /// <summary>Calculates the quotient and remainder using the
    /// DivideToIntegerNaturalScale and the formula in
    /// RemainderNaturalScale.</summary>
    /// <param name='divisor'>The number to divide by.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision, rounding, and exponent range of the result. This context
    /// will be used only in the division portion of the remainder
    /// calculation; as a result, it's possible for the remainder to have a
    /// higher precision than given in this context. Flags will be set on
    /// the given context only if the context's HasFlags is true and the
    /// integer part of the division result doesn't fit the precision and
    /// exponent range without rounding.</param>
    /// <returns>A 2 element array consisting of the quotient and remainder
    /// in that order.</returns>
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
