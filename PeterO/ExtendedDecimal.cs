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
    /// <summary>Represents an arbitrary-precision decimal floating-point
    /// number. Consists of an integer mantissa and an integer exponent,
    /// both arbitrary-precision. The value of the number equals mantissa *
    /// 10^exponent.
    /// <para>The mantissa is the value of the digits that make up a
    /// number, ignoring the decimal point and exponent. For example, in
    /// the number 2356.78, the mantissa is 235678. The exponent is where
    /// the "floating" decimal point of the number is located. A positive
    /// exponent means "move it to the right", and a negative exponent
    /// means "move it to the left." In the example 2, 356.78, the exponent
    /// is -2, since it has 2 decimal places and the decimal point is
    /// "moved to the left by 2." Therefore, in the ExtendedDecimal
    /// representation, this number would be stored as 235678 *
    /// 10^-2.</para>
    /// <para>The mantissa and exponent format preserves trailing zeros in
    /// the number's value. This may give rise to multiple ways to store
    /// the same value. For example, 1.00 and 1 would be stored
    /// differently, even though they have the same value. In the first
    /// case, 100 * 10^-2 (100 with decimal point moved left by 2), and in
    /// the second case, 1 * 10^0 (1 with decimal point moved 0).</para>
    /// <para>This class also supports values for negative zero,
    /// not-a-number (NaN) values, and infinity. <b>Negative zero</b> is
    /// generally used when a negative number is rounded to 0; it has the
    /// same mathematical value as positive zero. <b>Infinity</b> is
    /// generally used when a non-zero number is divided by zero, or when a
    /// very high number can't be represented in a given exponent range.
    /// <b>Not-a-number</b> is generally used to signal errors.</para>
    /// <para>This class implements the General Decimal Arithmetic
    /// Specification version 1.70:
    /// <c>http://speleotrove.com/decimal/decarith.html</c></para>
    /// <para>Passing a signaling NaN to any arithmetic operation shown
    /// here will signal the flag FlagInvalid and return a quiet NaN, even
    /// if another operand to that operation is a quiet NaN, unless noted
    /// otherwise.</para>
    /// <para>Passing a quiet NaN to any arithmetic operation shown here
    /// will return a quiet NaN, unless noted otherwise. Invalid operations
    /// will also return a quiet NaN, as stated in the individual
    /// methods.</para>
    /// <para>Unless noted otherwise, passing a null ExtendedDecimal
    /// argument to any method here will throw an exception.</para>
    /// <para>When an arithmetic operation signals the flag FlagInvalid,
    /// FlagOverflow, or FlagDivideByZero, it will not throw an exception
    /// too, unless the flag's trap is enabled in the precision context
    /// (see PrecisionContext's Traps property).</para>
    /// <para>An ExtendedDecimal value can be serialized in one of the
    /// following ways:</para>
    /// <list>
    /// <item>By calling the toString() method, which will always return
    /// distinct strings for distinct ExtendedDecimal values.</item>
    /// <item>By calling the UnsignedMantissa, Exponent, and IsNegative
    /// properties, and calling the IsInfinity, IsQuietNaN, and
    /// IsSignalingNaN methods. The return values combined will uniquely
    /// identify a particular ExtendedDecimal
    /// value.</item></list></summary>
  public sealed class ExtendedDecimal : IComparable<ExtendedDecimal>,
  IEquatable<ExtendedDecimal> {
    /// <summary>Gets this object&#x27;s exponent. This object&#x27;s value
    /// will be an integer if the exponent is positive or zero.</summary>
    /// <value>This object&apos;s exponent. This object&apos;s value will
    /// be an integer if the exponent is positive or zero.</value>
    public BigInteger Exponent { get {
 return new BigInteger(this.ed.Exponent);
} }

    /// <summary>Gets the absolute value of this object&#x27;s un-scaled
    /// value.</summary>
    /// <value>The absolute value of this object&apos;s un-scaled
    /// value.</value>
    public BigInteger UnsignedMantissa { get {
 return new BigInteger(this.ed.UnsignedMantissa);
} }

    /// <summary>Gets this object&#x27;s un-scaled value.</summary>
    /// <value>This object&apos;s un-scaled value. Will be negative if this
    /// object&apos;s value is negative (including a negative NaN).</value>
    public BigInteger Mantissa { get {
 return new BigInteger(this.ed.Mantissa);
} }

    #region Equals and GetHashCode implementation

    /// <summary>Determines whether this object&#x27;s mantissa and
    /// exponent are equal to those of another object.</summary>
    /// <param name='other'>An ExtendedDecimal object.</param>
    /// <returns>True if this object's mantissa and exponent are equal to
    /// those of another object; otherwise, false.</returns>
    public bool Equals(ExtendedDecimal other) {
      return (other == null) ? (false) : (this.ed.Equals(other.ed));
 }

    /// <summary>Determines whether this object&#x27;s mantissa and
    /// exponent are equal to those of another object and that other object
    /// is a decimal fraction.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object obj) {
      var bi = obj as ExtendedDecimal;
      return (bi == null) ? (false) : (this.ed.Equals(bi.ed));
    }

    /// <summary>Calculates this object&#x27;s hash code.</summary>
    /// <returns>This object's hash code.</returns>
    public override int GetHashCode() {
return this.ed.GetHashCode();
}
    #endregion

    /// <summary>Creates a number with the value
    /// exponent*10^mantissa.</summary>
    /// <param name='mantissaSmall'>The un-scaled value.</param>
    /// <param name='exponentSmall'>The decimal exponent.</param>
    /// <returns>An ExtendedDecimal object.</returns>
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

    /// <summary>Creates a number with the value
    /// exponent*10^mantissa.</summary>
    /// <param name='mantissa'>The un-scaled value.</param>
    /// <param name='exponent'>The decimal exponent.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='mantissa'/> or <paramref name='exponent'/> is
    /// null.</exception>
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

    /// <summary>Creates a not-a-number ExtendedDecimal object.</summary>
    /// <param name='diag'>A number to use as diagnostic information
    /// associated with this object. If none is needed, should be
    /// zero.</param>
    /// <returns>A quiet not-a-number object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='diag'/> is null or is less than 0.</exception>
    public static ExtendedDecimal CreateNaN(BigInteger diag) {
      if ((diag) == null) {
  throw new ArgumentNullException("diag");
}
      return new ExtendedDecimal(EDecimal.CreateNaN(diag.ei));
 }

    /// <summary>Creates a not-a-number ExtendedDecimal object.</summary>
    /// <param name='diag'>A number to use as diagnostic information
    /// associated with this object. If none is needed, should be
    /// zero.</param>
    /// <param name='signaling'>Whether the return value will be signaling
    /// (true) or quiet (false).</param>
    /// <param name='negative'>Whether the return value is
    /// negative.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='diag'/> is null or is less than 0.</exception>
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
return new ExtendedDecimal(EDecimal.CreateNaN(diag.ei, signaling, negative,
  ctx.ec));
}

    /// <summary>Creates a decimal number from a string that represents a
    /// number. See <c>FromString(String, int, int, PrecisionContext)</c>
    /// for more information.</summary>
    /// <param name='str'>A string that represents a number.</param>
    /// <returns>An arbitrary-precision decimal number with the same value
    /// as the given string.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='FormatException'>The parameter <paramref
    /// name='str'/> is not a correctly formatted number
    /// string.</exception>
    public static ExtendedDecimal FromString(string str) {
return new ExtendedDecimal(EDecimal.FromString(str));
}

    /// <summary>Creates a decimal number from a string that represents a
    /// number. See <c>FromString(String, int, int, PrecisionContext)</c>
    /// for more information.</summary>
    /// <param name='str'>A string that represents a number.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An arbitrary-precision decimal number with the same value
    /// as the given string.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='FormatException'>The parameter <paramref
    /// name='str'/> is not a correctly formatted number
    /// string.</exception>
    public static ExtendedDecimal FromString(string str, PrecisionContext ctx) {
return new ExtendedDecimal(EDecimal.FromString(str, ctx == null ? null :
  ctx.ec));
}

    /// <summary>Creates a decimal number from a string that represents a
    /// number. See <c>FromString(String, int, int, PrecisionContext)</c>
    /// for more information.</summary>
    /// <param name='str'>A string that represents a number.</param>
    /// <param name='offset'>A zero-based index showing where the desired
    /// portion of "str" begins.</param>
    /// <param name='length'>The length, in code units, of the desired
    /// portion of "str" (but not more than "str" 's length).</param>
    /// <returns>An arbitrary-precision decimal number with the same value
    /// as the given string.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='FormatException'>The parameter <paramref
    /// name='str'/> is not a correctly formatted number
    /// string.</exception>
    public static ExtendedDecimal FromString(string str,
      int offset,
      int length) {
return new ExtendedDecimal(EDecimal.FromString(str, offset, length));
}

    /// <summary>
    /// <para>Creates a decimal number from a string that represents a
    /// number.</para>
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
    /// "INF" , quiet NaN ("NaN" /"-NaN") followed by any number of digits,
    /// or signaling NaN ("sNaN" /"-sNaN") followed by any number of
    /// digits, all in any combination of upper and lower case.</para>
    /// <para>All characters mentioned above are the corresponding
    /// characters in the Basic Latin range. In particular, the digits must
    /// be the basic digits 0 to 9 (U + 0030 to U + 0039). The string is
    /// not allowed to contain white space characters, including
    /// spaces.</para></summary>
    /// <param name='str'>A string object, a portion of which represents a
    /// number.</param>
    /// <param name='offset'>A zero-based index that identifies the start
    /// of the number.</param>
    /// <param name='length'>The length of the number within the
    /// string.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An arbitrary-precision decimal number with the same value
    /// as the given string.</returns>
    /// <exception cref='FormatException'>The parameter <paramref
    /// name='str'/> is not a correctly formatted number
    /// string.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public static ExtendedDecimal FromString(string str,
      int offset,
      int length,
      PrecisionContext ctx) {
return new ExtendedDecimal(EDecimal.FromString(str, offset, length,
  ctx == null ? null : ctx.ec));
}

    /// <summary>Compares a ExtendedFloat object with this
    /// instance.</summary>
    /// <param name='other'>The other object to compare. Can be
    /// null.</param>
    /// <returns>Zero if the values are equal; a negative number if this
    /// instance is less, or a positive number if this instance is greater.
    /// Returns 0 if both values are NaN (even signaling NaN) and 1 if this
    /// value is NaN (even signaling NaN) and the other isn't, or if the
    /// other value is null.</returns>
    public int CompareToBinary(ExtendedFloat other) {
  return ((other) == null) ? (1) : (this.ed.CompareToBinary(other.ef));
}

    /// <summary>Converts this value to an arbitrary-precision integer. Any
    /// fractional part in this value will be discarded when converting to
    /// a big integer.</summary>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='OverflowException'>This object's value is infinity
    /// or NaN.</exception>
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

    /// <summary>Converts this value to an arbitrary-precision integer,
    /// checking whether the fractional part of the integer would be
    /// lost.</summary>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='OverflowException'>This object's value is infinity
    /// or NaN.</exception>
    /// <exception cref='ArithmeticException'>This object's value is not an
    /// exact integer.</exception>
    public BigInteger ToBigIntegerExact() {
return new BigInteger(this.ed.ToBigIntegerExact());
}

    private static readonly BigInteger valueOneShift62 = BigInteger.One << 62;

    /// <summary>Creates a binary floating-point number from this
    /// object&#x27;s value. Note that if the binary floating-point number
    /// contains a negative exponent, the resulting value might not be
    /// exact. However, the resulting binary float will contain enough
    /// precision to accurately convert it to a 32-bit or 64-bit floating
    /// point number (float or double).</summary>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat ToExtendedFloat() {
return new ExtendedFloat(this.ed.ToExtendedFloat());
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
return this.ed.ToSingle();
}

    /// <summary>Converts this value to a 64-bit floating-point number. The
    /// half-even rounding mode is used.
    /// <para>If this value is a NaN, sets the high bit of the 64-bit
    /// floating point number's mantissa for a quiet NaN, and clears it for
    /// a signaling NaN. Then the next highest bit of the mantissa is
    /// cleared for a quiet NaN, and set for a signaling NaN. Then the
    /// other bits of the mantissa are set to the lowest bits of this
    /// object's unsigned mantissa.</para></summary>
    /// <returns>The closest 64-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 64-bit floating point
    /// number.</returns>
    public double ToDouble() {
return this.ed.ToDouble();
}

    /// <summary>Creates a decimal number from a 32-bit floating-point
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting
    /// the floating point number to a string first. Remember, though, that
    /// the exact value of a 32-bit floating-point number is not always the
    /// value you get when you pass a literal decimal number (for example,
    /// calling <c>ExtendedDecimal.FromSingle(0.1f)</c> ), since not all
    /// decimal numbers can be converted to exact binary numbers (in the
    /// example given, the resulting ExtendedDecimal will be the the value
    /// of the closest "float" to 0.1, not 0.1 exactly). To create an
    /// ExtendedDecimal number from a decimal number, use FromString
    /// instead in most cases (for example:
    /// <c>ExtendedDecimal.FromString("0.1")</c> ).</summary>
    /// <param name='flt'>A 32-bit floating-point number.</param>
    /// <returns>A decimal number with the same value as <paramref
    /// name='flt'/>.</returns>
    public static ExtendedDecimal FromSingle(float flt) {
return new ExtendedDecimal(EDecimal.FromSingle(flt));
}

    /// <summary>Converts a big integer to an arbitrary precision
    /// decimal.</summary>
    /// <param name='bigint'>A BigInteger object.</param>
    /// <returns>An ExtendedDecimal object with the exponent set to
    /// 0.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigint'/> is null.</exception>
    public static ExtendedDecimal FromBigInteger(BigInteger bigint) {
  if ((bigint) == null) {
  throw new ArgumentNullException("bigint");
}
return new ExtendedDecimal(EDecimal.FromBigInteger(bigint.ei));
}

    /// <summary>Creates a decimal number from a 64-bit signed
    /// integer.</summary>
    /// <param name='valueSmall'>A 64-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object with the exponent set to
    /// 0.</returns>
    public static ExtendedDecimal FromInt64(long valueSmall) {
return new ExtendedDecimal(EDecimal.FromInt64(valueSmall));
}

    /// <summary>Creates a decimal number from a 32-bit signed
    /// integer.</summary>
    /// <param name='valueSmaller'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public static ExtendedDecimal FromInt32(int valueSmaller) {
return new ExtendedDecimal(EDecimal.FromInt32(valueSmaller));
}

    /// <summary>Creates a decimal number from a 64-bit floating-point
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting
    /// the floating point number to a string first. Remember, though, that
    /// the exact value of a 64-bit floating-point number is not always the
    /// value you get when you pass a literal decimal number (for example,
    /// calling <c>ExtendedDecimal.FromDouble(0.1f)</c> ), since not all
    /// decimal numbers can be converted to exact binary numbers (in the
    /// example given, the resulting ExtendedDecimal will be the value of
    /// the closest "double" to 0.1, not 0.1 exactly). To create an
    /// ExtendedDecimal number from a decimal number, use FromString
    /// instead in most cases (for example:
    /// <c>ExtendedDecimal.FromString("0.1")</c> ).</summary>
    /// <param name='dbl'>A 64-bit floating-point number.</param>
    /// <returns>A decimal number with the same value as <paramref
    /// name='dbl'/>.</returns>
    public static ExtendedDecimal FromDouble(double dbl) {
return new ExtendedDecimal(EDecimal.FromDouble(dbl));
}

    /// <summary>Creates a decimal number from an arbitrary-precision
    /// binary floating-point number.</summary>
    /// <param name='bigfloat'>A big floating-point number.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigfloat'/> is null.</exception>
    public static ExtendedDecimal FromExtendedFloat(ExtendedFloat bigfloat) {
  if ((bigfloat) == null) {
  throw new ArgumentNullException("bigfloat");
}
return new ExtendedDecimal(EDecimal.FromExtendedFloat(bigfloat.ef));
}

    /// <summary>Converts this value to a string. Returns a value
    /// compatible with this class's FromString method.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
return this.ed.ToString();
}

    /// <summary>Same as toString(), except that when an exponent is used
    /// it will be a multiple of 3.</summary>
    /// <returns>A string object.</returns>
    public string ToEngineeringString() {
return this.ed.ToEngineeringString();
}

    /// <summary>Converts this value to a string, but without using
    /// exponential notation.</summary>
    /// <returns>A string object.</returns>
    public string ToPlainString() {
return this.ed.ToPlainString();
}

    /// <summary>Represents the number 1.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    public static readonly ExtendedDecimal One =
      ExtendedDecimal.Create(BigInteger.One, BigInteger.Zero);

    /// <summary>Represents the number 0.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    public static readonly ExtendedDecimal Zero =
      ExtendedDecimal.Create(BigInteger.Zero, BigInteger.Zero);

    /// <summary>Represents the number negative zero.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    public static readonly ExtendedDecimal NegativeZero =
      new ExtendedDecimal(EDecimal.NegativeZero);

    /// <summary>Represents the number 10.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif

    public static readonly ExtendedDecimal Ten =
      ExtendedDecimal.Create((BigInteger)10, BigInteger.Zero);

    //----------------------------------------------------------------

    /// <summary>A not-a-number value.</summary>
    public static readonly ExtendedDecimal NaN =
      new ExtendedDecimal(EDecimal.NaN);

    /// <summary>A not-a-number value that signals an invalid operation
    /// flag when it&#x27;s passed as an argument to any arithmetic
    /// operation in ExtendedDecimal.</summary>
    public static readonly ExtendedDecimal SignalingNaN =
      new ExtendedDecimal(EDecimal.SignalingNaN);

    /// <summary>Positive infinity, greater than any other
    /// number.</summary>
    public static readonly ExtendedDecimal PositiveInfinity =
      new ExtendedDecimal(EDecimal.PositiveInfinity);

    /// <summary>Negative infinity, less than any other number.</summary>
    public static readonly ExtendedDecimal NegativeInfinity=
      new ExtendedDecimal(EDecimal.NegativeInfinity);

    /// <summary>Returns whether this object is negative
    /// infinity.</summary>
    /// <returns>True if this object is negative infinity; otherwise,
    /// false.</returns>
    public bool IsNegativeInfinity() {
return this.ed.IsNegativeInfinity();
}

    /// <summary>Returns whether this object is positive
    /// infinity.</summary>
    /// <returns>True if this object is positive infinity; otherwise,
    /// false.</returns>
    public bool IsPositiveInfinity() {
return this.ed.IsPositiveInfinity();
}

    /// <summary>Gets a value indicating whether this object is not a
    /// number (NaN).</summary>
    /// <returns>True if this object is not a number (NaN); otherwise,
    /// false.</returns>
    public bool IsNaN() {
return this.ed.IsNaN();
}

    /// <summary>Gets a value indicating whether this object is positive or
    /// negative infinity.</summary>
    /// <returns>True if this object is positive or negative infinity;
    /// otherwise, false.</returns>
    public bool IsInfinity() {
return this.ed.IsInfinity();
}

    /// <summary>Gets a value indicating whether this object is finite (not
    /// infinity or NaN).</summary>
    /// <value>True if this object is finite (not infinity or NaN);
    /// otherwise, false.</value>
    public  bool IsFinite {
get {
return this.ed.IsFinite;
} }

    /// <summary>Gets a value indicating whether this object is negative,
    /// including negative zero.</summary>
    /// <value>True if this object is negative, including negative zero;
    /// otherwise, false.</value>
    public  bool IsNegative {
get {
return this.ed.IsNegative;
} }

    /// <summary>Gets a value indicating whether this object is a quiet
    /// not-a-number value.</summary>
    /// <returns>True if this object is a quiet not-a-number value;
    /// otherwise, false.</returns>
    public bool IsQuietNaN() {
return this.ed.IsQuietNaN();
}

    /// <summary>Gets a value indicating whether this object is a signaling
    /// not-a-number value.</summary>
    /// <returns>True if this object is a signaling not-a-number value;
    /// otherwise, false.</returns>
    public bool IsSignalingNaN() {
return this.ed.IsSignalingNaN();
}

    /// <summary>Gets this value&#x27;s sign: -1 if negative; 1 if
    /// positive; 0 if zero.</summary>
    /// <value>This value&apos;s sign: -1 if negative; 1 if positive; 0 if
    /// zero.</value>
    public  int Sign {
get {
return this.ed.Sign;
} }

    /// <summary>Gets a value indicating whether this object&#x27;s value
    /// equals 0.</summary>
    /// <value>True if this object&apos;s value equals 0; otherwise,
    /// false.</value>
    public  bool IsZero {
get {
return this.ed.IsZero;
} }

    /// <summary>Gets the absolute value of this object.</summary>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal Abs() {
return new ExtendedDecimal(this.ed.Abs());
}

    /// <summary>Gets an object with the same value as this one, but with
    /// the sign reversed.</summary>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal Negate() {
return new ExtendedDecimal(this.ed.Negate());
}

    /// <summary>Divides this object by another decimal number and returns
    /// the result. When possible, the result will be exact.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The quotient of the two numbers. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Returns NaN if the divisor and the dividend are 0. Returns
    /// NaN if the result can't be exact because it would have a
    /// nonterminating decimal expansion.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    public ExtendedDecimal Divide(ExtendedDecimal divisor) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
return new ExtendedDecimal(this.ed.Divide(divisor.ed));
}

    /// <summary>Divides this object by another decimal number and returns
    /// a result with the same exponent as this object (the
    /// dividend).</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two numbers. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Signals FlagInvalid and returns NaN if the divisor and the
    /// dividend are 0. Signals FlagInvalid and returns NaN if the rounding
    /// mode is Rounding.Unnecessary and the result is not exact.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    public ExtendedDecimal DivideToSameExponent(ExtendedDecimal divisor,
      Rounding rounding) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
return new ExtendedDecimal(this.ed.DivideToSameExponent(divisor.ed,
  ExtendedDecimal.ToERounding(rounding)));
}

    /// <summary>Divides two ExtendedDecimal objects, and returns the
    /// integer part of the result, rounded down, with the preferred
    /// exponent set to this value&#x27;s exponent minus the divisor&#x27;s
    /// exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The integer part of the quotient of the two objects.
    /// Signals FlagDivideByZero and returns infinity if the divisor is 0
    /// and the dividend is nonzero. Signals FlagInvalid and returns NaN if
    /// the divisor and the dividend are 0.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    public ExtendedDecimal DivideToIntegerNaturalScale(ExtendedDecimal
                    divisor) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
return new ExtendedDecimal(this.ed.DivideToIntegerNaturalScale(divisor.ed));
}

    /// <summary>Removes trailing zeros from this object&#x27;s mantissa.
    /// For example, 1.000 becomes 1.
    /// <para>If this object's value is 0, changes the exponent to
    /// 0.</para></summary>
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
    public ExtendedDecimal Reduce(PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.Reduce(ctx == null ? null : ctx.ec));
}

    /// <summary>Calculates the remainder of a number by the formula "this"
    /// - (("this" / "divisor") * "divisor").</summary>
    /// <param name='divisor'>The number to divide by.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    public ExtendedDecimal RemainderNaturalScale(ExtendedDecimal divisor) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
return new ExtendedDecimal(this.ed.RemainderNaturalScale(divisor.ed));
}

    /// <summary>Calculates the remainder of a number by the formula "this"
    /// - (("this" / "divisor") * "divisor").</summary>
    /// <param name='divisor'>The number to divide by.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision, ExtendedDecimal.ToERounding(rounding), and exponent
    /// range of the result. This context will be used only in the division
    /// portion of the remainder calculation; as a result, it's possible
    /// for the return value to have a higher precision than given in this
    /// context. Flags will be set on the given context only if the
    /// context's HasFlags is true and the integer part of the division
    /// result doesn't fit the precision and exponent range without
    /// rounding.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> or <paramref name='ctx'/> is null.</exception>
    public ExtendedDecimal RemainderNaturalScale(ExtendedDecimal divisor,
      PrecisionContext ctx) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.RemainderNaturalScale(divisor.ed,
  ctx == null ? null : ctx.ec));
}

    /// <summary>Divides two ExtendedDecimal objects, and gives a
    /// particular exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedDecimal object.</param>
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
    /// range. Signals FlagInvalid and returns NaN if the rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> or <paramref name='ctx'/> is null.</exception>
    public ExtendedDecimal DivideToExponent(ExtendedDecimal divisor,
      long desiredExponentSmall,
      PrecisionContext ctx) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.DivideToExponent(divisor.ed,
  desiredExponentSmall, ctx == null ? null : ctx.ec));
}

    /// <summary>Divides this ExtendedDecimal object by another
    /// ExtendedDecimal object. The preferred exponent for the result is
    /// this object&#x27;s exponent minus the divisor&#x27;s
    /// exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Signals FlagInvalid and returns NaN if the divisor and the
    /// dividend are 0; or, either <paramref name='ctx'/> is null or
    /// <paramref name='ctx'/> 's precision is 0, and the result would have
    /// a nonterminating decimal expansion; or, the rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    public ExtendedDecimal Divide(ExtendedDecimal divisor,
      PrecisionContext ctx) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
return new ExtendedDecimal(this.ed.Divide(divisor.ed, ctx == null ? null :
  ctx.ec));
}

    /// <summary>Divides two ExtendedDecimal objects, and gives a
    /// particular exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedDecimal object.</param>
    /// <param name='desiredExponentSmall'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal
    /// point. A positive number places the cutoff point to the left of the
    /// usual decimal point.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Signals FlagInvalid and returns NaN if the divisor and the
    /// dividend are 0. Signals FlagInvalid and returns NaN if the rounding
    /// mode is Rounding.Unnecessary and the result is not exact.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    public ExtendedDecimal DivideToExponent(ExtendedDecimal divisor,
      long desiredExponentSmall,
      Rounding rounding) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
return new ExtendedDecimal(this.ed.DivideToExponent(divisor.ed,
  desiredExponentSmall, ExtendedDecimal.ToERounding(rounding)));
}

    /// <summary>Divides two ExtendedDecimal objects, and gives a
    /// particular exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedDecimal object.</param>
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
    /// range. Signals FlagInvalid and returns NaN if the rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> or <paramref name='exponent'/> or <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedDecimal DivideToExponent(ExtendedDecimal divisor,
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
return new ExtendedDecimal(this.ed.DivideToExponent(divisor.ed, exponent.ei,
  ctx.ec));
}

    /// <summary>Divides two ExtendedDecimal objects, and gives a
    /// particular exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedDecimal object.</param>
    /// <param name='desiredExponent'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal
    /// point. A positive number places the cutoff point to the left of the
    /// usual decimal point.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Returns NaN if the divisor and the dividend are 0. Returns
    /// NaN if the rounding mode is Rounding.Unnecessary and the result is
    /// not exact.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> or <paramref name='desiredExponent'/> is
    /// null.</exception>
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
    public ExtendedDecimal Abs(PrecisionContext context) {
return new ExtendedDecimal(this.ed.Abs(context == null ? null : context.ec));
}

    /// <summary>Returns a decimal number with the same value as this
    /// object but with the sign reversed.</summary>
    /// <param name='context'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='context'/> is null.</exception>
    public ExtendedDecimal Negate(PrecisionContext context) {
  if ((context) == null) {
  throw new ArgumentNullException("context");
}
return new ExtendedDecimal(this.ed.Negate(context == null ? null : context.ec));
}

    /// <summary>Adds this object and another decimal number and returns
    /// the result.</summary>
    /// <param name='otherValue'>An ExtendedDecimal object.</param>
    /// <returns>The sum of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ExtendedDecimal Add(ExtendedDecimal otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedDecimal(this.ed.Add(otherValue.ed));
}

    /// <summary>Subtracts an ExtendedDecimal object from this instance and
    /// returns the result.</summary>
    /// <param name='otherValue'>An ExtendedDecimal object.</param>
    /// <returns>The difference of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ExtendedDecimal Subtract(ExtendedDecimal otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedDecimal(this.ed.Subtract(otherValue.ed));
}

    /// <summary>Subtracts an ExtendedDecimal object from this
    /// instance.</summary>
    /// <param name='otherValue'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The difference of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ExtendedDecimal Subtract(ExtendedDecimal otherValue,
      PrecisionContext ctx) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.Subtract(otherValue.ed, ctx == null ? null:
  ctx.ec));
}

    /// <summary>Multiplies two decimal numbers. The resulting exponent
    /// will be the sum of the exponents of the two decimal
    /// numbers.</summary>
    /// <param name='otherValue'>Another decimal number.</param>
    /// <returns>The product of the two decimal numbers.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ExtendedDecimal Multiply(ExtendedDecimal otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedDecimal(this.ed.Multiply(otherValue.ed));
}

    /// <summary>Multiplies by one decimal number, and then adds another
    /// decimal number.</summary>
    /// <param name='multiplicand'>The value to multiply.</param>
    /// <param name='augend'>The value to add.</param>
    /// <returns>The result this * <paramref name='multiplicand'/> +
    /// <paramref name='augend'/>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='multiplicand'/> or <paramref name='augend'/> is
    /// null.</exception>
    public ExtendedDecimal MultiplyAndAdd(ExtendedDecimal multiplicand,
      ExtendedDecimal augend) {
  if ((multiplicand) == null) {
  throw new ArgumentNullException("multiplicand");
}
  if ((augend) == null) {
  throw new ArgumentNullException("augend");
}
return new ExtendedDecimal(this.ed.MultiplyAndAdd(multiplicand.ed, augend.ed));
}

    /// <summary>Divides this object by another object, and returns the
    /// integer part of the result, with the preferred exponent set to this
    /// value&#x27;s exponent minus the divisor&#x27;s exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision, ExtendedDecimal.ToERounding(rounding), and exponent
    /// range of the integer part of the result. Flags will be set on the
    /// given context only if the context's HasFlags is true and the
    /// integer part of the result doesn't fit the precision and exponent
    /// range without rounding.</param>
    /// <returns>The integer part of the quotient of the two objects.
    /// Signals FlagInvalid and returns NaN if the return value would
    /// overflow the exponent range. Signals FlagDivideByZero and returns
    /// infinity if the divisor is 0 and the dividend is nonzero. Signals
    /// FlagInvalid and returns NaN if the divisor and the dividend are 0.
    /// Signals FlagInvalid and returns NaN if the rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> or <paramref name='ctx'/> is null.</exception>
    public ExtendedDecimal DivideToIntegerNaturalScale(ExtendedDecimal divisor,
      PrecisionContext ctx) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.DivideToIntegerNaturalScale(divisor.ed,
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
    public ExtendedDecimal DivideToIntegerZeroScale(ExtendedDecimal divisor,
      PrecisionContext ctx) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.DivideToIntegerZeroScale(divisor.ed,
  ctx.ec));
}

    /// <summary>Finds the remainder that results when dividing two
    /// ExtendedDecimal objects.</summary>
    /// <param name='divisor'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The remainder of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> or <paramref name='ctx'/> is null.</exception>
    public ExtendedDecimal Remainder(ExtendedDecimal divisor,
      PrecisionContext ctx) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.Remainder(divisor.ed, ctx == null ? null :
  ctx.ec));
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
    public ExtendedDecimal RemainderNear(ExtendedDecimal divisor,
      PrecisionContext ctx) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.RemainderNear(divisor.ed, ctx == null ?
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
    /// infinity. Signals FlagInvalid and returns NaN if the parameter
    /// <paramref name='ctx'/> is null, the precision is 0, or <paramref
    /// name='ctx'/> has an unlimited exponent range.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedDecimal NextMinus(PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.NextMinus(ctx == null ? null : ctx.ec));
}

    /// <summary>Finds the smallest value that&#x27;s greater than the
    /// given value.</summary>
    /// <param name='ctx'>A precision context object to control the
    /// precision and exponent range of the result. The rounding mode from
    /// this context is ignored. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags).</param>
    /// <returns>Returns the smallest value that's greater than the given
    /// value.Signals FlagInvalid and returns NaN if the parameter
    /// <paramref name='ctx'/> is null, the precision is 0, or <paramref
    /// name='ctx'/> has an unlimited exponent range.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedDecimal NextPlus(PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.NextPlus(ctx == null ? null : ctx.ec));
}

    /// <summary>Finds the next value that is closer to the other
    /// object&#x27;s value than this object&#x27;s value. Returns a copy
    /// of this value with the same sign as the other value if both values
    /// are equal.</summary>
    /// <param name='otherValue'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision and exponent range of the result. The rounding mode from
    /// this context is ignored. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags).</param>
    /// <returns>Returns the next value that is closer to the other object'
    /// s value than this object's value. Signals FlagInvalid and returns
    /// NaN if the parameter <paramref name='ctx'/> is null, the precision
    /// is 0, or <paramref name='ctx'/> has an unlimited exponent
    /// range.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> or <paramref name='ctx'/> is null.</exception>
    public ExtendedDecimal NextToward(ExtendedDecimal otherValue,
      PrecisionContext ctx) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.NextToward(otherValue.ed, ctx == null ?
  null : ctx.ec));
}

    /// <summary>Gets the greater value between two decimal
    /// numbers.</summary>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The larger value of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> or <paramref
    /// name='ctx'/> is null.</exception>
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

    /// <summary>Gets the lesser value between two decimal
    /// numbers.</summary>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The smaller value of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> or <paramref
    /// name='ctx'/> is null.</exception>
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

    /// <summary>Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as
    /// Max.</summary>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> or <paramref
    /// name='ctx'/> is null.</exception>
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

    /// <summary>Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as
    /// Min.</summary>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> or <paramref
    /// name='ctx'/> is null.</exception>
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

    /// <summary>Gets the greater value between two decimal
    /// numbers.</summary>
    /// <param name='first'>An ExtendedDecimal object.</param>
    /// <param name='second'>Another ExtendedDecimal object.</param>
    /// <returns>The larger value of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> is null.</exception>
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

    /// <summary>Gets the lesser value between two decimal
    /// numbers.</summary>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <returns>The smaller value of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> is null.</exception>
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

    /// <summary>Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as
    /// Max.</summary>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> is null.</exception>
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

    /// <summary>Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as
    /// Min.</summary>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> is null.</exception>
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
    /// <param name='other'>An ExtendedDecimal object.</param>
    /// <returns>Less than 0 if this object's value is less than the other
    /// value, or greater than 0 if this object's value is greater than the
    /// other value or if <paramref name='other'/> is null, or 0 if both
    /// values are equal.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='other'/> is null.</exception>
    public int CompareTo(ExtendedDecimal other) {
  if ((other) == null) {
  throw new ArgumentNullException("other");
}
return this.ed.CompareTo(other.ed);
}

    /// <summary>Compares the mathematical values of this object and
    /// another object.
    /// <para>In this method, negative zero and positive zero are
    /// considered equal.</para>
    /// <para>If this object or the other object is a quiet NaN or
    /// signaling NaN, this method returns a quiet NaN, and will signal a
    /// FlagInvalid flag if either is a signaling NaN.</para></summary>
    /// <param name='other'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A precision context. The precision,
    /// ExtendedDecimal.ToERounding(rounding), and exponent range are
    /// ignored. If HasFlags of the context is true, will store the flags
    /// resulting from the operation (the flags are in addition to the
    /// pre-existing flags). Can be null.</param>
    /// <returns>Quiet NaN if this object or the other object is NaN, or 0
    /// if both objects have the same value, or -1 if this object is less
    /// than the other value, or 1 if this object is greater.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='other'/> or <paramref name='ctx'/> is null.</exception>
    public ExtendedDecimal CompareToWithContext(ExtendedDecimal other,
      PrecisionContext ctx) {
  if ((other) == null) {
  throw new ArgumentNullException("other");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.CompareToWithContext(other.ed, ctx == null?
  null : ctx.ec));
}

    /// <summary>Compares the mathematical values of this object and
    /// another object, treating quiet NaN as signaling.
    /// <para>In this method, negative zero and positive zero are
    /// considered equal.</para>
    /// <para>If this object or the other object is a quiet NaN or
    /// signaling NaN, this method will return a quiet NaN and will signal
    /// a FlagInvalid flag.</para></summary>
    /// <param name='other'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A precision context. The precision,
    /// ExtendedDecimal.ToERounding(rounding), and exponent range are
    /// ignored. If HasFlags of the context is true, will store the flags
    /// resulting from the operation (the flags are in addition to the
    /// pre-existing flags). Can be null.</param>
    /// <returns>Quiet NaN if this object or the other object is NaN, or 0
    /// if both objects have the same value, or -1 if this object is less
    /// than the other value, or 1 if this object is greater.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='other'/> or <paramref name='ctx'/> is null.</exception>
    public ExtendedDecimal CompareToSignal(ExtendedDecimal other,
      PrecisionContext ctx) {
  if ((other) == null) {
  throw new ArgumentNullException("other");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.CompareToSignal(other.ed, ctx == null ?
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
    public ExtendedDecimal Add(ExtendedDecimal otherValue,
      PrecisionContext ctx) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.Add(otherValue.ed, ctx == null ? null :
  ctx.ec));
}

    /// <summary>Returns a decimal number with the same value but a new
    /// exponent.
    /// <para>Note that this is not always the same as rounding to a given
    /// number of decimal places, since it can fail if the difference
    /// between this value's exponent and the desired exponent is too big,
    /// depending on the maximum precision. If rounding to a number of
    /// decimal places is desired, it's better to use the RoundToExponent
    /// and RoundToIntegral methods instead.</para></summary>
    /// <param name='desiredExponent'>A BigInteger object.</param>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null, in which case the
    /// default rounding mode is HalfEven.</param>
    /// <returns>A decimal number with the same value as this object but
    /// with the exponent changed. Signals FlagInvalid and returns NaN if
    /// the rounded result can't fit the given precision, or if the context
    /// defines an exponent range and the given exponent is outside that
    /// range.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='desiredExponent'/> or <paramref name='ctx'/> is
    /// null.</exception>
    public ExtendedDecimal Quantize(BigInteger desiredExponent,
      PrecisionContext ctx) {
  if ((desiredExponent) == null) {
  throw new ArgumentNullException("desiredExponent");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.Quantize(desiredExponent.ei, ctx == null ?
  null : ctx.ec));
}

    /// <summary>Returns a decimal number with the same value as this one
    /// but a new exponent.</summary>
    /// <param name='desiredExponentSmall'>A 32-bit signed integer.</param>
    /// <param name='rounding'>A Rounding object.</param>
    /// <returns>A decimal number with the same value as this object but
    /// with the exponent changed. Returns NaN if the rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</returns>
    public ExtendedDecimal Quantize(int desiredExponentSmall,
      Rounding rounding) {
return new ExtendedDecimal(this.ed.Quantize(desiredExponentSmall,
  ExtendedDecimal.ToERounding(rounding)));
}

    /// <summary>Returns a decimal number with the same value but a new
    /// exponent.
    /// <para>Note that this is not always the same as rounding to a given
    /// number of decimal places, since it can fail if the difference
    /// between this value's exponent and the desired exponent is too big,
    /// depending on the maximum precision. If rounding to a number of
    /// decimal places is desired, it's better to use the RoundToExponent
    /// and RoundToIntegral methods instead.</para></summary>
    /// <param name='desiredExponentSmall'>The desired exponent for the
    /// result. The exponent is the number of fractional digits in the
    /// result, expressed as a negative number. Can also be positive, which
    /// eliminates lower-order places from the number. For example, -3
    /// means round to the thousandth (10^-3, 0.0001), and 3 means round to
    /// the thousand (10^3, 1000). A value of 0 rounds the number to an
    /// integer.</param>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null, in which case the
    /// default rounding mode is HalfEven.</param>
    /// <returns>A decimal number with the same value as this object but
    /// with the exponent changed. Signals FlagInvalid and returns NaN if
    /// the rounded result can't fit the given precision, or if the context
    /// defines an exponent range and the given exponent is outside that
    /// range.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedDecimal Quantize(int desiredExponentSmall,
      PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.Quantize(desiredExponentSmall, ctx == null?
  null : ctx.ec));
}

    /// <summary>Returns a decimal number with the same value as this
    /// object but with the same exponent as another decimal number.
    /// <para>Note that this is not always the same as rounding to a given
    /// number of decimal places, since it can fail if the difference
    /// between this value's exponent and the desired exponent is too big,
    /// depending on the maximum precision. If rounding to a number of
    /// decimal places is desired, it's better to use the RoundToExponent
    /// and RoundToIntegral methods instead.</para></summary>
    /// <param name='otherValue'>A decimal number containing the desired
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
    /// <returns>A decimal number with the same value as this object but
    /// with the exponent changed. Signals FlagInvalid and returns NaN if
    /// the result can't fit the given precision without rounding, or if
    /// the precision context defines an exponent range and the given
    /// exponent is outside that range.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> or <paramref name='ctx'/> is null.</exception>
    public ExtendedDecimal Quantize(ExtendedDecimal otherValue,
      PrecisionContext ctx) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedDecimal(this.ed.Quantize(otherValue.ed, ctx == null ? null:
  ctx.ec));
}

    /// <summary>Returns a decimal number with the same value as this
    /// object but rounded to an integer, and signals an invalid operation
    /// if the result would be inexact.</summary>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null, in which case the
    /// default rounding mode is HalfEven.</param>
    /// <returns>A decimal number rounded to the closest integer
    /// representable in the given precision. Signals FlagInvalid and
    /// returns NaN if the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the precision
    /// context defines an exponent range, the new exponent must be changed
    /// to 0 when rounding, and 0 is outside of the valid range of the
    /// precision context.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedDecimal RoundToIntegralExact(PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.RoundToIntegralExact(ctx == null ? null :
  ctx.ec));
}

    /// <summary>Returns a decimal number with the same value as this
    /// object but rounded to an integer, without adding the FlagInexact or
    /// FlagRounded flags.</summary>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags), except that this function will
    /// never add the FlagRounded and FlagInexact flags (the only
    /// difference between this and RoundToExponentExact). Can be null, in
    /// which case the default rounding mode is HalfEven.</param>
    /// <returns>A decimal number rounded to the closest integer
    /// representable in the given precision, meaning if the result can't
    /// fit the precision, additional digits are discarded to make it fit.
    /// Signals FlagInvalid and returns NaN if the precision context
    /// defines an exponent range, the new exponent must be changed to 0
    /// when rounding, and 0 is outside of the valid range of the precision
    /// context.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedDecimal RoundToIntegralNoRoundedFlag(PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.RoundToIntegralNoRoundedFlag(ctx == null ?
  null : ctx.ec));
}

    /// <summary>Returns a decimal number with the same value as this
    /// object but rounded to an integer, and signals an invalid operation
    /// if the result would be inexact.</summary>
    /// <param name='exponent'>The minimum exponent the result can have.
    /// This is the maximum number of fractional digits in the result,
    /// expressed as a negative number. Can also be positive, which
    /// eliminates lower-order places from the number. For example, -3
    /// means round to the thousandth (10^-3, 0.0001), and 3 means round to
    /// the thousand (10^3, 1000). A value of 0 rounds the number to an
    /// integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null, in which case the default rounding mode is
    /// HalfEven.</param>
    /// <returns>A decimal number rounded to the closest value
    /// representable in the given precision. Signals FlagInvalid and
    /// returns NaN if the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the precision
    /// context defines an exponent range, the new exponent must be changed
    /// to the given exponent when rounding, and the given exponent is
    /// outside of the valid range of the precision context.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='exponent'/> or <paramref name='ctx'/> is null.</exception>
    public ExtendedDecimal RoundToExponentExact(BigInteger exponent,
      PrecisionContext ctx) {
  if ((exponent) == null) {
  throw new ArgumentNullException("exponent");
}
return new ExtendedDecimal(this.ed.RoundToExponentExact(exponent.ei,
  ctx == null ? null : ctx.ec));
}

    /// <summary>Returns a decimal number with the same value as this
    /// object, and rounds it to a new exponent if necessary.</summary>
    /// <param name='exponent'>The minimum exponent the result can have.
    /// This is the maximum number of fractional digits in the result,
    /// expressed as a negative number. Can also be positive, which
    /// eliminates lower-order places from the number. For example, -3
    /// means round to the thousandth (10^-3, 0.0001), and 3 means round to
    /// the thousand (10^3, 1000). A value of 0 rounds the number to an
    /// integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null, in which case the default rounding mode is
    /// HalfEven.</param>
    /// <returns>A decimal number rounded to the closest value
    /// representable in the given precision, meaning if the result can't
    /// fit the precision, additional digits are discarded to make it fit.
    /// Signals FlagInvalid and returns NaN if the precision context
    /// defines an exponent range, the new exponent must be changed to the
    /// given exponent when rounding, and the given exponent is outside of
    /// the valid range of the precision context.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='exponent'/> or <paramref name='ctx'/> is null.</exception>
    public ExtendedDecimal RoundToExponent(BigInteger exponent,
      PrecisionContext ctx) {
  if ((exponent) == null) {
  throw new ArgumentNullException("exponent");
}
return new ExtendedDecimal(this.ed.RoundToExponent(exponent.ei, ctx == null ?
  null : ctx.ec));
}

    /// <summary>Returns a decimal number with the same value as this
    /// object but rounded to an integer, and signals an invalid operation
    /// if the result would be inexact.</summary>
    /// <param name='exponentSmall'>The minimum exponent the result can
    /// have. This is the maximum number of fractional digits in the
    /// result, expressed as a negative number. Can also be positive, which
    /// eliminates lower-order places from the number. For example, -3
    /// means round to the thousandth (10^-3, 0.0001), and 3 means round to
    /// the thousand (10^3, 1000). A value of 0 rounds the number to an
    /// integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>A decimal number rounded to the closest value
    /// representable in the given precision. Signals FlagInvalid and
    /// returns NaN if the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the precision
    /// context defines an exponent range, the new exponent must be changed
    /// to the given exponent when rounding, and the given exponent is
    /// outside of the valid range of the precision context.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedDecimal RoundToExponentExact(int exponentSmall,
      PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.RoundToExponentExact(exponentSmall,
  ctx == null ? null : ctx.ec));
}

    /// <summary>Returns a decimal number with the same value as this
    /// object, and rounds it to a new exponent if necessary.</summary>
    /// <param name='exponentSmall'>The minimum exponent the result can
    /// have. This is the maximum number of fractional digits in the
    /// result, expressed as a negative number. Can also be positive, which
    /// eliminates lower-order places from the number. For example, -3
    /// means round to the thousandth (10^-3, 0.0001), and 3 means round to
    /// the thousand (10^3, 1000). A value of 0 rounds the number to an
    /// integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null, in which case the default rounding mode is
    /// HalfEven.</param>
    /// <returns>A decimal number rounded to the closest value
    /// representable in the given precision, meaning if the result can't
    /// fit the precision, additional digits are discarded to make it fit.
    /// Signals FlagInvalid and returns NaN if the precision context
    /// defines an exponent range, the new exponent must be changed to the
    /// given exponent when rounding, and the given exponent is outside of
    /// the valid range of the precision context.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedDecimal RoundToExponent(int exponentSmall,
      PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.RoundToExponent(exponentSmall, ctx == null?
  null : ctx.ec));
}

    /// <summary>Multiplies two decimal numbers. The resulting scale will
    /// be the sum of the scales of the two decimal numbers. The
    /// result&#x27;s sign is positive if both operands have the same sign,
    /// and negative if they have different signs.</summary>
    /// <param name='op'>Another decimal number.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The product of the two decimal numbers.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='op'/> or <paramref name='ctx'/> is null.</exception>
    public ExtendedDecimal Multiply(ExtendedDecimal op, PrecisionContext ctx) {
  if ((op) == null) {
  throw new ArgumentNullException("op");
}
return new ExtendedDecimal(this.ed.Multiply(op.ed, ctx == null ? null :
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
    public ExtendedDecimal MultiplyAndAdd(ExtendedDecimal op,
      ExtendedDecimal augend,
      PrecisionContext ctx) {
  if ((op) == null) {
  throw new ArgumentNullException("op");
}
  if ((augend) == null) {
  throw new ArgumentNullException("augend");
}
return new ExtendedDecimal(this.ed.MultiplyAndAdd(op.ed, augend.ed,
  ctx == null ? null : ctx.ec));
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
    public ExtendedDecimal MultiplyAndSubtract(ExtendedDecimal op,
      ExtendedDecimal subtrahend,
      PrecisionContext ctx) {
  if ((op) == null) {
  throw new ArgumentNullException("op");
}
  if ((subtrahend) == null) {
  throw new ArgumentNullException("subtrahend");
}
return new ExtendedDecimal(this.ed.MultiplyAndSubtract(op.ed, subtrahend.ed,
  ctx.ec));
}

    /// <summary>Rounds this object&#x27;s value to a given precision,
    /// using the given rounding mode and range of exponent.</summary>
    /// <param name='ctx'>A context for controlling the precision,
    /// ExtendedDecimal.ToERounding(rounding) mode, and exponent range. Can
    /// be null.</param>
    /// <returns>The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if
    /// <paramref name='ctx'/> is null or the precision and exponent range
    /// are unlimited.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedDecimal RoundToPrecision(PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.RoundToPrecision(ctx == null ? null :
  ctx.ec));
}

    /// <summary>Rounds this object&#x27;s value to a given precision,
    /// using the given rounding mode and range of exponent, and also
    /// converts negative zero to positive zero.</summary>
    /// <param name='ctx'>A context for controlling the precision,
    /// ExtendedDecimal.ToERounding(rounding) mode, and exponent range. Can
    /// be null.</param>
    /// <returns>The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if
    /// <paramref name='ctx'/> is null or the precision and exponent range
    /// are unlimited.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedDecimal Plus(PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.Plus(ctx == null ? null : ctx.ec));
}

    /// <summary>Rounds this object&#x27;s value to a given maximum bit
    /// length, using the given rounding mode and range of
    /// exponent.</summary>
    /// <param name='ctx'>A context for controlling the precision,
    /// ExtendedDecimal.ToERounding(rounding) mode, and exponent range. The
    /// precision is interpreted as the maximum bit length of the mantissa.
    /// Can be null.</param>
    /// <returns>The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if
    /// <paramref name='ctx'/> is null or the precision and exponent range
    /// are unlimited.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    [Obsolete(
      "Instead of this method use RoundToPrecision and pass a " + "precision context with the IsPrecisionInBits property set.")]
    public ExtendedDecimal RoundToBinaryPrecision(PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.RoundToBinaryPrecision(ctx == null ? null :
  ctx.ec));
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
    /// complex number, but the return value is still NaN). Signals
    /// FlagInvalid and returns NaN if the parameter <paramref name='ctx'/>
    /// is null or the precision is unlimited (the context's Precision
    /// property is 0).</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedDecimal SquareRoot(PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.SquareRoot(ctx == null ? null : ctx.ec));
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
    /// returns an approximation to " e" within the given precision.
    /// Signals FlagInvalid and returns NaN if the parameter <paramref
    /// name='ctx'/> is null or the precision is unlimited (the context's
    /// Precision property is 0).</returns>
    public ExtendedDecimal Exp(PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.Exp(ctx == null ? null : ctx.ec));
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
    /// NaN.). Signals FlagInvalid and returns NaN if the parameter
    /// <paramref name='ctx'/> is null or the precision is unlimited (the
    /// context's Precision property is 0). Signals no flags and returns
    /// negative infinity if this object's value is 0.</returns>
    public ExtendedDecimal Log(PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.Log(ctx == null ? null : ctx.ec));
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
    public ExtendedDecimal Log10(PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.Log10(ctx == null ? null : ctx.ec));
}

    /// <summary>Raises this object&#x27;s value to the given
    /// exponent.</summary>
    /// <param name='exponent'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing
    /// flags).</param>
    /// <returns>This^exponent. Signals the flag FlagInvalid and returns
    /// NaN if this object and exponent are both 0; or if this value is
    /// less than 0 and the exponent either has a fractional part or is
    /// infinity. Signals FlagInvalid and returns NaN if the parameter
    /// <paramref name='ctx'/> is null or the precision is unlimited (the
    /// context's Precision property is 0), and the exponent has a
    /// fractional part.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='exponent'/> is null.</exception>
    public ExtendedDecimal Pow(ExtendedDecimal exponent, PrecisionContext ctx) {
  if ((exponent) == null) {
  throw new ArgumentNullException("exponent");
}
return new ExtendedDecimal(this.ed.Pow(exponent.ed, ctx == null ? null :
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
    public ExtendedDecimal Pow(int exponentSmall, PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.Pow(exponentSmall, ctx == null ? null :
  ctx.ec));
}

    /// <summary>Raises this object&#x27;s value to the given
    /// exponent.</summary>
    /// <param name='exponentSmall'>A 32-bit signed integer.</param>
    /// <returns>This^exponent. Returns NaN if this object and exponent are
    /// both 0.</returns>
    public ExtendedDecimal Pow(int exponentSmall) {
return new ExtendedDecimal(this.ed.Pow(exponentSmall));
}

    /// <summary>Finds the constant pi.</summary>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// --This parameter cannot be null, as pi can never be represented
    /// exactly.--.</param>
    /// <returns>Pi rounded to the given precision. Signals FlagInvalid and
    /// returns NaN if the parameter <paramref name='ctx'/> is null or the
    /// precision is unlimited (the context's Precision property is
    /// 0).</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public static ExtendedDecimal PI(PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(EDecimal.PI(ctx == null ? null : ctx.ec));
}

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the left.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal MovePointLeft(int places) {
return new ExtendedDecimal(this.ed.MovePointLeft(places));
}

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the left.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedDecimal MovePointLeft(int places, PrecisionContext ctx) {
return new ExtendedDecimal(this.ed.MovePointLeft(places, ctx == null ? null :
  ctx.ec));
}

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the left.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigPlaces'/> is null.</exception>
    public ExtendedDecimal MovePointLeft(BigInteger bigPlaces) {
  if ((bigPlaces) == null) {
  throw new ArgumentNullException("bigPlaces");
}
return new ExtendedDecimal(this.ed.MovePointLeft(bigPlaces.ei));
}

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the left.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigPlaces'/> is null.</exception>
    public ExtendedDecimal MovePointLeft(BigInteger bigPlaces,
PrecisionContext ctx) {
  if ((bigPlaces) == null) {
  throw new ArgumentNullException("bigPlaces");
}
return new ExtendedDecimal(this.ed.MovePointLeft(bigPlaces.ei,
  ctx == null ? null : ctx.ec));
}

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the right.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal MovePointRight(int places) {
return new ExtendedDecimal(this.ed.MovePointRight(places));
}

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the right.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedDecimal MovePointRight(int places, PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.MovePointRight(places, ctx == null ? null :
  ctx.ec));
}

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the right.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigPlaces'/> is null.</exception>
    public ExtendedDecimal MovePointRight(BigInteger bigPlaces) {
  if ((bigPlaces) == null) {
  throw new ArgumentNullException("bigPlaces");
}
return new ExtendedDecimal(this.ed.MovePointRight(bigPlaces.ei));
}

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the right.</summary>
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
    public ExtendedDecimal MovePointRight(BigInteger bigPlaces,
PrecisionContext ctx) {
  if ((bigPlaces) == null) {
  throw new ArgumentNullException("bigPlaces");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.MovePointRight(bigPlaces.ei, ctx == null ?
  null : ctx.ec));
}

    /// <summary>Returns a number similar to this number but with the scale
    /// adjusted.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal ScaleByPowerOfTen(int places) {
return new ExtendedDecimal(this.ed.ScaleByPowerOfTen(places));
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
    public ExtendedDecimal ScaleByPowerOfTen(int places, PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.ScaleByPowerOfTen(places, ctx == null ?
  null : ctx.ec));
}

    /// <summary>Returns a number similar to this number but with the scale
    /// adjusted.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigPlaces'/> is null.</exception>
    public ExtendedDecimal ScaleByPowerOfTen(BigInteger bigPlaces) {
  if ((bigPlaces) == null) {
  throw new ArgumentNullException("bigPlaces");
}
return new ExtendedDecimal(this.ed.ScaleByPowerOfTen(bigPlaces.ei));
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
    public ExtendedDecimal ScaleByPowerOfTen(BigInteger bigPlaces,
PrecisionContext ctx) {
  if ((bigPlaces) == null) {
  throw new ArgumentNullException("bigPlaces");
}
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.ed.ScaleByPowerOfTen(bigPlaces.ei, ctx == null?
  null : ctx.ec));
}

    /// <summary>Finds the number of digits in this number's mantissa.
    /// Returns 1 if this value is 0, and 0 if this value is infinity or
    /// NaN.</summary>
    /// <returns>A BigInteger object.</returns>
    public BigInteger Precision() {
return new BigInteger(this.ed.Precision());
}

    /// <summary>Returns the unit in the last place. The mantissa will be 1
    /// and the exponent will be this number's exponent. Returns 1 with an
    /// exponent of 0 if this number is infinity or NaN.</summary>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal Ulp() {
return new ExtendedDecimal(this.ed.Ulp());
}

    /// <summary>Calculates the quotient and remainder using the
    /// DivideToIntegerNaturalScale and the formula in
    /// RemainderNaturalScale.</summary>
    /// <param name='divisor'>The number to divide by.</param>
    /// <returns>A 2 element array consisting of the quotient and remainder
    /// in that order.</returns>
    public ExtendedDecimal[] DivideAndRemainderNaturalScale(ExtendedDecimal
      divisor) {
      EDecimal[] edec = this.ed.DivideAndRemainderNaturalScale(divisor == null ? null : divisor.ed);
      return new ExtendedDecimal[] {
        new ExtendedDecimal(edec[0]),new ExtendedDecimal(edec[1])
      };
 }

    /// <summary>Calculates the quotient and remainder using the
    /// DivideToIntegerNaturalScale and the formula in
    /// RemainderNaturalScale.</summary>
    /// <param name='divisor'>The number to divide by.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision, ExtendedDecimal.ToERounding(rounding), and exponent
    /// range of the result. This context will be used only in the division
    /// portion of the remainder calculation; as a result, it's possible
    /// for the remainder to have a higher precision than given in this
    /// context. Flags will be set on the given context only if the
    /// context's HasFlags is true and the integer part of the division
    /// result doesn't fit the precision and exponent range without
    /// rounding.</param>
    /// <returns>A 2 element array consisting of the quotient and remainder
    /// in that order.</returns>
    public ExtendedDecimal[] DivideAndRemainderNaturalScale(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      EDecimal[] edec = this.ed.DivideAndRemainderNaturalScale(divisor == null ? null : divisor.ed,
        ctx==null ? null : ctx.ec);
      return new ExtendedDecimal[] {
        new ExtendedDecimal(edec[0]),new ExtendedDecimal(edec[1])
      };
    }
  }
}
