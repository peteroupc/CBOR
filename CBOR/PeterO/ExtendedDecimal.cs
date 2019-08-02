/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Text;
using PeterO.Numbers;

namespace PeterO {
    /// <summary><para><b>This class is largely obsolete. It will be replaced by a new version
    /// of this class in a different namespace/package and library, called
    /// <c>PeterO.Numbers.EDecimal
    /// </c>
    /// in the
    /// <a href='https://www.nuget.org/packages/PeterO.Numbers'>
    /// <c>PeterO.Numbers
    /// </c>
    /// </a>
    /// library (in .NET), or
    /// <c>com.upokecenter.numbers.EDecimal
    /// </c>
    /// in the
    /// <a href='https://github.com/peteroupc/numbers-java'>
    /// <c>com.github.peteroupc/numbers
    /// </c>
    /// </a>
    /// artifact (in Java). This new class can be used in the
    /// <c>CBORObject.FromObject(object)
    /// </c>
    /// method (by including the new library in your code, among other
    /// things).
    /// </b>
    /// </para>
    /// Represents an arbitrary-precision decimal floating-point number.
    /// <para><b>About decimal arithmetic
    /// </b>
    /// </para>
    /// <para>Decimal (base-10) arithmetic, such as that provided by this class, is
    /// appropriate for calculations involving such real-world data as prices
    /// and other sums of money, tax rates, and measurements. These calculations
    /// often involve multiplying or dividing one decimal with another decimal,
    /// or performing other operations on decimal numbers. Many of these
    /// calculations also rely on rounding behavior in which the result after
    /// rounding is a decimal number (for example, multiplying a price by a
    /// premium rate, then rounding, should result in a decimal amount of
    /// money).
    /// </para>
    /// <para>On the other hand, most implementations of
    /// <c>float
    /// </c>
    /// and
    /// <c>double
    /// </c>
    /// , including in C# and Java, store numbers in a binary (base-2)
    /// floating-point format and use binary floating-point arithmetic. Many
    /// decimal numbers can't be represented exactly in binary floating-point
    /// format (regardless of its length). Applying binary arithmetic to numbers
    /// intended to be decimals can sometimes lead to unintuitive results, as is
    /// shown in the description for the FromDouble() method of this class.
    /// </para>
    /// <para><b>About ExtendedDecimal instances
    /// </b>
    /// </para>
    /// <para>Each instance of this class consists of an integer mantissa and an
    /// integer exponent, both arbitrary-precision. The value of the number
    /// equals mantissa * 10^exponent.
    /// </para>
    /// <para>The mantissa is the value of the digits that make up a number, ignoring
    /// the decimal point and exponent. For example, in the number 2356.78, the
    /// mantissa is 235678. The exponent is where the "floating" decimal point
    /// of the number is located. A positive exponent means "move it to the
    /// right", and a negative exponent means "move it to the left." In the
    /// example 2, 356.78, the exponent is -2, since it has 2 decimal places and
    /// the decimal point is "moved to the left by 2." Therefore, in the
    /// arbitrary-precision decimal representation, this number would be stored
    /// as 235678 * 10^-2.
    /// </para>
    /// <para>The mantissa and exponent format preserves trailing zeros in the
    /// number's value. This may give rise to multiple ways to store the same
    /// value. For example, 1.00 and 1 would be stored differently, even though
    /// they have the same value. In the first case, 100 * 10^-2 (100 with
    /// decimal point moved left by 2), and in the second case, 1 * 10^0 (1 with
    /// decimal point moved 0).
    /// </para>
    /// <para>This class also supports values for negative zero, not-a-number (NaN)
    /// values, and infinity.
    /// <b>Negative zero
    /// </b>
    /// is generally used when a negative number is rounded to 0; it has the
    /// same mathematical value as positive zero.
    /// <b>Infinity
    /// </b>
    /// is generally used when a non-zero number is divided by zero, or when a
    /// very high number can't be represented in a given exponent range.
    /// <b>Not-a-number
    /// </b>
    /// is generally used to signal errors.
    /// </para>
    /// <para><b>Errors and Exceptions
    /// </b>
    /// </para>
    /// <para>Passing a signaling NaN to any arithmetic operation shown here will
    /// signal the flag FlagInvalid and return a quiet NaN, even if another
    /// operand to that operation is a quiet NaN, unless noted otherwise.
    /// </para>
    /// <para>Passing a quiet NaN to any arithmetic operation shown here will return a
    /// quiet NaN, unless noted otherwise. Invalid operations will also return a
    /// quiet NaN, as stated in the individual methods.
    /// </para>
    /// <para>Unless noted otherwise,passing a null arbitrary-precision decimal
    /// argument to any method here will throw an exception.
    /// </para>
    /// <para>When an arithmetic operation signals the flag FlagInvalid, FlagOverflow,
    /// or FlagDivideByZero, it will not throw an exception too, unless the
    /// flag's trap is enabled in the precision context (see EContext's Traps
    /// property).
    /// </para>
    /// <para>If an operation requires creating an intermediate value that might be
    /// too big to fit in memory (or might require more than 2 gigabytes of
    /// memory to store -- due to the current use of a 32-bit integer internally
    /// as a length), the operation may signal an invalid-operation flag and
    /// return not-a-number (NaN). In certain rare cases, the CompareTo method
    /// may throw OutOfMemoryException (called OutOfMemoryError in Java) in the
    /// same circumstances.
    /// </para>
    /// <para><b>Serialization
    /// </b>
    /// </para>
    /// <para>An arbitrary-precision decimal value can be serialized (converted to a
    /// stable format) in one of the following ways:
    /// </para>
    /// <list>
    /// <item>By calling the toString() method, which will always return distinct
    /// strings for distinct arbitrary-precision decimal values.
    /// </item>
    /// <item>By calling the UnsignedMantissa, Exponent, and IsNegative properties,
    /// and calling the IsInfinity, IsQuietNaN, and IsSignalingNaN methods.
    /// The return values combined will uniquely identify a particular
    /// arbitrary-precision decimal value.
    /// </item>
    /// </list>
    /// <para><b>Thread safety
    /// </b>
    /// </para>
    /// <para>Instances of this class are immutable, so they are inherently safe for
    /// use by multiple threads. Multiple instances of this object with the same
    /// properties are interchangeable, so they should not be compared using the
    /// "==" operator (which might only check if each side of the operator is
    /// the same instance).
    /// </para>
    /// <para><b>Comparison considerations
    /// </b>
    /// </para>
    /// <para>This class's natural ordering (under the CompareTo method) is not
    /// consistent with the Equals method. This means that two values that
    /// compare as equal under the CompareTo method might not be equal under the
    /// Equals method. The CompareTo method compares the mathematical values of
    /// the two instances passed to it (and considers two different NaN values
    /// as equal), while two instances with the same mathematical value, but
    /// different exponents, will be considered unequal under the Equals method.
    /// </para></summary>
    [Obsolete(
  "Use EDecimal from PeterO.Numbers/com.upokecenter.numbers and the output" +
"\u0020of this class's ToString method.")]
  public sealed class ExtendedDecimal : IComparable<ExtendedDecimal>,
  IEquatable<ExtendedDecimal> {
    /// <summary>Gets this object's exponent. This object's value will be an integer if the
    /// exponent is positive or zero.</summary><value>This object's exponent. This object's value will be an integer if the
    /// exponent is positive or zero.
    /// </value>
    public BigInteger Exponent {
      get {
        return new BigInteger(this.Ed.Exponent);
      }
    }

    /// <summary>Gets the absolute value of this object's un-scaled value.</summary><value>The absolute value of this object's un-scaled value.
    /// </value>
    public BigInteger UnsignedMantissa {
      get {
        return new BigInteger(this.Ed.UnsignedMantissa);
      }
    }

    /// <summary>Gets this object's un-scaled value.</summary><value>This object's un-scaled value. Will be negative if this object's value is
    /// negative (including a negative NaN).
    /// </value>
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

    /// <summary>Determines whether this object's mantissa and exponent are equal to those
    /// of another object.</summary><param name='other'>An arbitrary-precision decimal number.
    /// </param><returns><c>true
    /// </c>
    /// if this object's mantissa and exponent are equal to those of another
    /// object; otherwise,
    /// <c>false
    /// </c>
    /// .
    /// </returns>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool Equals(ExtendedDecimal other) {
      return (other == null) ? false : this.Ed.Equals(other.Ed);
    }

    /// <summary>Determines whether this object's mantissa and exponent are equal to those
    /// of another object and that other object is an arbitrary-precision decimal
    /// number.</summary><param name='obj'>The parameter
    /// <paramref name='obj'/>
    /// is an arbitrary object.
    /// </param><returns><c>true
    /// </c>
    /// if the objects are equal; otherwise,
    /// <c>false
    /// </c>
    /// .
    /// </returns>
    public override bool Equals(object obj) {
      var bi = obj as ExtendedDecimal;
      return (bi == null) ? false : this.Ed.Equals(bi.Ed);
    }

    /// <summary>Calculates this object's hash code. No application or process IDs are used
    /// in the hash code calculation.</summary><returns>This object's hash code.
    /// </returns>
    public override int GetHashCode() {
      return this.Ed.GetHashCode();
    }
    #endregion
    private readonly EDecimal ed;

    internal ExtendedDecimal(EDecimal ed) {
      if (ed == null) {
        throw new ArgumentNullException(nameof(ed));
      }
      this.ed = ed;
    }

    /// <summary>Creates a number with the value exponent*10^mantissa.</summary><param name='mantissa'>The un-scaled value.
    /// </param><param name='exponent'>The decimal exponent.
    /// </param><returns>An arbitrary-precision decimal number.
    /// </returns><exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='mantissa'/>
    /// or
    /// <paramref name='exponent'/>
    /// is null.</exception>
    public static ExtendedDecimal Create(
      BigInteger mantissa,
      BigInteger exponent) {
      if (mantissa == null) {
        throw new ArgumentNullException(nameof(mantissa));
      }
      if (exponent == null) {
        throw new ArgumentNullException(nameof(exponent));
      }
      return new ExtendedDecimal(EDecimal.Create(mantissa.Ei, exponent.Ei));
    }

    /// <summary>Creates a decimal number from a text string that represents a number. See
    /// <c>FromString(String, int, int, EContext)
    /// </c>
    /// for more information.</summary><param name='str'>A string that represents a number.
    /// </param><returns>An arbitrary-precision decimal number with the same value as the given
    /// string.
    /// </returns><exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='str'/>
    /// is null.</exception><exception cref='System.FormatException'>The parameter
    /// <paramref name='str'/>
    /// is not a correctly formatted number string.</exception>
    public static ExtendedDecimal FromString(string str) {
      return new ExtendedDecimal(EDecimal.FromString(str));
    }

    /// <summary>Converts this value to a 32-bit floating-point number. The half-even
    /// rounding mode is used.
    /// <para>If this value is a NaN, sets the high bit of the 32-bit floating point
    /// number's mantissa for a quiet NaN, and clears it for a signaling NaN.
    /// Then the next highest bit of the mantissa is cleared for a quiet NaN,
    /// and set for a signaling NaN. Then the other bits of the mantissa are set
    /// to the lowest bits of this object's unsigned mantissa.
    /// </para></summary><returns>The closest 32-bit floating-point number to this value. The return value
    /// can be positive infinity or negative infinity if this value exceeds the
    /// range of a 32-bit floating point number.
    /// </returns>
    public float ToSingle() {
      return this.Ed.ToSingle();
    }

    /// <summary>Converts this value to a 64-bit floating-point number. The half-even
    /// rounding mode is used.
    /// <para>If this value is a NaN, sets the high bit of the 64-bit floating point
    /// number's mantissa for a quiet NaN, and clears it for a signaling NaN.
    /// Then the next highest bit of the mantissa is cleared for a quiet NaN,
    /// and set for a signaling NaN. Then the other bits of the mantissa are set
    /// to the lowest bits of this object's unsigned mantissa.
    /// </para></summary><returns>The closest 64-bit floating-point number to this value. The return value
    /// can be positive infinity or negative infinity if this value exceeds the
    /// range of a 64-bit floating point number.
    /// </returns>
    public double ToDouble() {
      return this.Ed.ToDouble();
    }

    /// <summary>Converts this value to a string. Returns a value compatible with this
    /// class's FromString method.</summary><returns>A string representation of this object.
    /// </returns>
    public override string ToString() {
      return this.Ed.ToString();
    }

    /// <summary>Represents the number 1.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly ExtendedDecimal One =
      ExtendedDecimal.Create(BigInteger.One, BigInteger.Zero);

    /// <summary>Represents the number 0.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
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
      new ExtendedDecimal(EDecimal.Ten);

    //----------------------------------------------------------------

    /// <summary>A not-a-number value.</summary>
    public static readonly ExtendedDecimal NaN =
      new ExtendedDecimal(EDecimal.NaN);

    /// <summary>A not-a-number value that signals an invalid operation flag when it's
    /// passed as an argument to any arithmetic operation in arbitrary-precision
    /// decimal.</summary>
    public static readonly ExtendedDecimal SignalingNaN =
      new ExtendedDecimal(EDecimal.SignalingNaN);

    /// <summary>Positive infinity, greater than any other number.</summary>
    public static readonly ExtendedDecimal PositiveInfinity =
      new ExtendedDecimal(EDecimal.PositiveInfinity);

    /// <summary>Negative infinity, less than any other number.</summary>
    public static readonly ExtendedDecimal NegativeInfinity =
      new ExtendedDecimal(EDecimal.NegativeInfinity);

    /// <summary>Gets a value indicating whether this object is not a number (NaN).</summary><returns><c>true
    /// </c>
    /// if this object is not a number (NaN); otherwise,
    /// <c>false
    /// </c>
    /// .
    /// </returns>
    public bool IsNaN() {
      return this.Ed.IsNaN();
    }

    /// <summary>Gets a value indicating whether this object is positive or negative
    /// infinity.</summary><returns><c>true
    /// </c>
    /// if this object is positive or negative infinity; otherwise,
    /// <c>false
    /// </c>
    /// .
    /// </returns>
    public bool IsInfinity() {
      return this.Ed.IsInfinity();
    }

    /// <summary>Gets a value indicating whether this object is negative, including
    /// negative zero.</summary><value><c>true
    /// </c>
    /// If this object is negative, including negative zero; otherwise, .
    /// <c>false
    /// </c>
    /// .
    /// </value>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsNegative {
      get {
        return this.Ed.IsNegative;
      }
    }

    /// <summary>Gets a value indicating whether this object is a quiet not-a-number value.</summary><returns><c>true
    /// </c>
    /// if this object is a quiet not-a-number value; otherwise,
    /// <c>false
    /// </c>
    /// .
    /// </returns>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsQuietNaN() {
      return this.Ed.IsQuietNaN();
    }

    /// <summary>Compares this extended decimal to another.</summary><param name='other'>The parameter
    /// <paramref name='other'/>
    /// is an ExtendedDecimal object.
    /// </param><returns>Less than 0 if this value is less than, 0 if equal to, or greater than 0
    /// if greater than the other extended decimal.
    /// </returns>
    public int CompareTo(ExtendedDecimal other) {
      return this.Ed.CompareTo(other == null ? null : other.Ed);
    }

    /// <summary>Gets this value's sign: -1 if negative; 1 if positive; 0 if zero.</summary><value>This value's sign: -1 if negative; 1 if positive; 0 if zero.
    /// </value>
    [Obsolete("Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.")]
    public int Sign {
      get {
        return this.Ed.Sign;
      }
    }

    internal EDecimal Ed {
      get {
        return this.ed;
      }
    }
  }
}
