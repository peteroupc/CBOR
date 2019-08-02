/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using PeterO.Numbers;

namespace PeterO {
    /// <summary><para><b>This class is largely obsolete. It will be replaced by a new version
    /// of this class in a different namespace/package and library, called
    /// <c>PeterO.Numbers.EFloat
    /// </c>
    /// in the
    /// <a href='https://www.nuget.org/packages/PeterO.Numbers'>
    /// <c>PeterO.Numbers
    /// </c>
    /// </a>
    /// library (in .NET), or
    /// <c>com.upokecenter.numbers.EFloat
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
    /// <para>Represents an arbitrary-precision binary floating-point number. Consists
    /// of an integer mantissa and an integer exponent, both
    /// arbitrary-precision. The value of the number equals mantissa *
    /// 2^exponent. This class also supports values for negative zero,
    /// not-a-number (NaN) values, and infinity.
    /// </para>
    /// <para>Passing a signaling NaN to any arithmetic operation shown here will
    /// signal the flag FlagInvalid and return a quiet NaN, even if another
    /// operand to that operation is a quiet NaN, unless noted otherwise.
    /// </para>
    /// <para>Passing a quiet NaN to any arithmetic operation shown here will return a
    /// quiet NaN, unless noted otherwise.
    /// </para>
    /// <para>Unless noted otherwise,passing a null arbitrary-precision binary float
    /// argument to any method here will throw an exception.
    /// </para>
    /// <para>When an arithmetic operation signals the flag FlagInvalid, FlagOverflow,
    /// or FlagDivideByZero, it will not throw an exception too, unless the
    /// operation's trap is enabled in the precision context (see
    /// PrecisionContext's Traps property).
    /// </para>
    /// <para>An arbitrary-precision binary float value can be serialized in one of
    /// the following ways:
    /// </para>
    /// <list>
    /// <item>By calling the toString() method. However, not all strings can be
    /// converted back to an arbitrary-precision binary float without loss,
    /// especially if the string has a fractional part.
    /// </item>
    /// <item>By calling the UnsignedMantissa, Exponent, and IsNegative properties,
    /// and calling the IsInfinity, IsQuietNaN, and IsSignalingNaN methods.
    /// The return values combined will uniquely identify a particular
    /// arbitrary-precision binary float value.
    /// </item>
    /// </list>
    /// <para>If an operation requires creating an intermediate value that might be
    /// too big to fit in memory (or might require more than 2 gigabytes of
    /// memory to store -- due to the current use of a 32-bit integer internally
    /// as a length), the operation may signal an invalid-operation flag and
    /// return not-a-number (NaN). In certain rare cases, the CompareTo method
    /// may throw OutOfMemoryException (called OutOfMemoryError in Java) in the
    /// same circumstances.
    /// </para>
    /// <para><b>Thread safety:
    /// </b>
    /// Instances of this class are immutable, so they are inherently safe for
    /// use by multiple threads. Multiple instances of this object with the same
    /// properties are interchangeable, so they should not be compared using the
    /// "==" operator (which might only check if each side of the operator is
    /// the same instance).
    /// </para></summary>
    [Obsolete(
  "Use EFloat from PeterO.Numbers/com.upokecenter.numbers and the output of" +
"\u0020this class's ToString method.")]
  public sealed class ExtendedFloat : IComparable<ExtendedFloat>,
 IEquatable<ExtendedFloat> {
    private readonly EFloat ef;

    internal ExtendedFloat(EFloat ef) {
      this.ef = ef;
    }

    /// <summary>Gets this object's exponent. This object's value will be an integer if the
    /// exponent is positive or zero.</summary><value>This object's exponent. This object's value will be an integer if the
    /// exponent is positive or zero.
    /// </value>
    public BigInteger Exponent {
      get {
        return new BigInteger(this.Ef.Exponent);
      }
    }

    /// <summary>Gets the absolute value of this object's un-scaled value.</summary><value>The absolute value of this object's un-scaled value.
    /// </value>
    public BigInteger UnsignedMantissa {
      get {
        return new BigInteger(this.Ef.UnsignedMantissa);
      }
    }

    /// <summary>Gets this object's un-scaled value.</summary><value>This object's un-scaled value. Will be negative if this object's value is
    /// negative (including a negative NaN).
    /// </value>
    public BigInteger Mantissa {
      get {
        return new BigInteger(this.Ef.Mantissa);
      }
    }

    internal static ExtendedFloat ToLegacy(EFloat ei) {
      return new ExtendedFloat(ei);
    }

    internal static EFloat FromLegacy(ExtendedFloat bei) {
      return bei.Ef;
    }

    #region Equals and GetHashCode implementation

    /// <summary>Determines whether this object's mantissa and exponent are equal to those
    /// of another object.</summary><param name='otherValue'>An arbitrary-precision binary float.
    /// </param><returns><c>true
    /// </c>
    /// if this object's mantissa and exponent are equal to those of another
    /// object; otherwise,
    /// <c>false
    /// </c>
    /// .
    /// </returns><exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='otherValue'/>
    /// is null.</exception>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool EqualsInternal(ExtendedFloat otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException(nameof(otherValue));
      }
      return this.Ef.EqualsInternal(otherValue.Ef);
    }

    /// <summary>Determines whether this object's mantissa and exponent are equal to those
    /// of another object.</summary><param name='other'>An arbitrary-precision binary float.
    /// </param><returns><c>true
    /// </c>
    /// if this object's mantissa and exponent are equal to those of another
    /// object; otherwise,
    /// <c>false
    /// </c>
    /// .
    /// </returns><exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='other'/>
    /// is null.</exception>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool Equals(ExtendedFloat other) {
      if (other == null) {
        throw new ArgumentNullException(nameof(other));
      }
      return this.Ef.Equals(other.Ef);
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
      var bi = obj as ExtendedFloat;
      return (bi == null) ? false : this.Ef.Equals(bi.Ef);
    }

    /// <summary>Calculates this object's hash code. No application or process IDs are used
    /// in the hash code calculation.</summary><returns>This object's hash code.
    /// </returns>
    public override int GetHashCode() {
      return this.Ef.GetHashCode();
    }
    #endregion

    /// <summary>Creates a number with the value exponent*2^mantissa.</summary><param name='mantissaSmall'>The un-scaled value.
    /// </param><param name='exponentSmall'>The binary exponent.
    /// </param><returns>An arbitrary-precision binary float.
    /// </returns>
    public static ExtendedFloat Create(int mantissaSmall, int exponentSmall) {
      return new ExtendedFloat(EFloat.Create(mantissaSmall, exponentSmall));
    }

    /// <summary>Creates a number with the value exponent*2^mantissa.</summary><param name='mantissa'>The un-scaled value.
    /// </param><param name='exponent'>The binary exponent.
    /// </param><returns>An arbitrary-precision binary float.
    /// </returns><exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='mantissa'/>
    /// or
    /// <paramref name='exponent'/>
    /// is null.</exception>
    public static ExtendedFloat Create(
      BigInteger mantissa,
      BigInteger exponent) {
      if (mantissa == null) {
        throw new ArgumentNullException(nameof(mantissa));
      }
      if (exponent == null) {
        throw new ArgumentNullException(nameof(exponent));
      }
      return new ExtendedFloat(EFloat.Create(mantissa.Ei, exponent.Ei));
    }

    /// <summary>Creates a binary float from a text string that represents a number. Note
    /// that if the string contains a negative exponent, the resulting value might
    /// not be exact, in which case the resulting binary float will be an
    /// approximation of this decimal number's value. (NOTE: This documentation
    /// previously said the binary float will contain enough precision to
    /// accurately convert it to a 32-bit or 64-bit floating point number. Due to
    /// double rounding, this will generally not be the case for certain numbers
    /// converted from decimal to ExtendedFloat via this method and in turn
    /// converted to
    /// <c>double
    /// </c>
    /// or
    /// <c>float
    /// </c>
    /// .)
    /// <para>The format of the string generally consists of:
    /// </para>
    /// <list type=''>
    /// <item>An optional plus sign ("+" , U+002B) or minus sign ("-", U+002D) (if
    /// '-' , the value is negative.)
    /// </item>
    /// <item>One or more digits, with a single optional decimal point after the
    /// first digit and before the last digit.
    /// </item>
    /// <item>Optionally, "E+"/"e+" (positive exponent) or "E-"/"e-" (negative
    /// exponent) plus one or more digits specifying the exponent.
    /// </item>
    /// </list>
    /// <para>The string can also be "-INF", "-Infinity", "Infinity", "INF", quiet NaN
    /// ("NaN") followed by any number of digits, or signaling NaN ("sNaN")
    /// followed by any number of digits, all in any combination of upper and
    /// lower case.
    /// </para>
    /// <para>All characters mentioned above are the corresponding characters in the
    /// Basic Latin range. In particular, the digits must be the basic digits 0
    /// to 9 (U+0030 to U+0039). The string is not allowed to contain white
    /// space characters, including spaces.
    /// </para></summary><param name='str'>The parameter
    /// <paramref name='str'/>
    /// is a text string.
    /// </param><param name='offset'>A zero-based index showing where the desired portion of
    /// <paramref name='str'/>
    /// begins.
    /// </param><param name='length'>The length, in code units, of the desired portion of
    /// <paramref name='str'/>
    /// (but not more than
    /// <paramref name='str'/>
    /// 's length).
    /// </param><param name='ctx'>A PrecisionContext object specifying the precision, rounding, and exponent
    /// range to apply to the parsed number. Can be null.
    /// </param><returns>The parsed number, converted to arbitrary-precision binary float.
    /// </returns><exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='str'/>
    /// is null.</exception><exception cref='System.ArgumentException'>Either
    /// <paramref name='offset'/>
    /// or
    /// <paramref name='length'/>
    /// is less than 0 or greater than
    /// <paramref name='str'/>
    /// 's length, or
    /// <paramref name='str'/>
    /// ' s length minus
    /// <paramref name='offset'/>
    /// is less than
    /// <paramref name='length'/>
    /// .</exception>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static ExtendedFloat FromString(
      string str,
      int offset,
      int length,
      PrecisionContext ctx) {
      try {
        return new ExtendedFloat(
  EFloat.FromString(
    str,
    offset,
    length,
    ctx == null ? null : ctx.Ec));
      } catch (ETrapException ex) {
        throw TrapException.Create(ex);
      }
    }

    /// <summary>Creates a binary float from a text string that represents a number.</summary><param name='str'>A text string containing the number to convert.
    /// </param><returns>The parsed number, converted to arbitrary-precision binary float.
    /// </returns>
    public static ExtendedFloat FromString(string str) {
      return new ExtendedFloat(EFloat.FromString(str));
    }

    /// <summary>Converts this value to a string.</summary><returns>A string representation of this object. The value is converted to decimal
    /// and the decimal form of this number's value is returned.
    /// </returns>
    public override string ToString() {
      return this.Ef.ToString();
    }

    /// <summary>Represents the number 1.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly ExtendedFloat One =
     new ExtendedFloat(EFloat.One);

    /// <summary>Represents the number 0.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly ExtendedFloat Zero =
     new ExtendedFloat(EFloat.Zero);

    /// <summary>Represents the number negative zero.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly ExtendedFloat NegativeZero =
     new ExtendedFloat(EFloat.NegativeZero);

    /// <summary>Represents the number 10.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
#endif

    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly ExtendedFloat Ten =
     new ExtendedFloat(EFloat.Ten);

    //----------------------------------------------------------------

    /// <summary>A not-a-number value.</summary>
    public static readonly ExtendedFloat NaN =
     new ExtendedFloat(EFloat.NaN);

    /// <summary>A not-a-number value that signals an invalid operation flag when it's
    /// passed as an argument to any arithmetic operation in arbitrary-precision
    /// binary float.</summary>
    public static readonly ExtendedFloat SignalingNaN =
     new ExtendedFloat(EFloat.SignalingNaN);

    /// <summary>Positive infinity, greater than any other number.</summary>
    public static readonly ExtendedFloat PositiveInfinity =
     new ExtendedFloat(EFloat.PositiveInfinity);

    /// <summary>Negative infinity, less than any other number.</summary>
    public static readonly ExtendedFloat NegativeInfinity =
     new ExtendedFloat(EFloat.NegativeInfinity);

    /// <summary>Returns whether this object is negative infinity.</summary><returns><c>true
    /// </c>
    /// if this object is negative infinity; otherwise,
    /// <c>false
    /// </c>
    /// .
    /// </returns>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsNegativeInfinity() {
      return this.Ef.IsNegativeInfinity();
    }

    /// <summary>Returns whether this object is positive infinity.</summary><returns><c>true
    /// </c>
    /// if this object is positive infinity; otherwise,
    /// <c>false
    /// </c>
    /// .
    /// </returns>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsPositiveInfinity() {
      return this.Ef.IsPositiveInfinity();
    }

    /// <summary>Returns whether this object is a not-a-number value.</summary><returns><c>true
    /// </c>
    /// if this object is a not-a-number value; otherwise,
    /// <c>false
    /// </c>
    /// .
    /// </returns>
    public bool IsNaN() {
      return this.Ef.IsNaN();
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
      return this.Ef.IsInfinity();
    }

    /// <summary>Gets a value indicating whether this object is negative, including
    /// negative zero.</summary><value><c>true
    /// </c>
    /// If this object is negative, including negative zero; otherwise, .
    /// <c>false
    /// </c>
    /// .
    /// </value>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsNegative {
      get {
        return this.Ef.IsNegative;
      }
    }

    /// <summary>Gets a value indicating whether this object is a quiet not-a-number value.</summary><returns><c>true
    /// </c>
    /// if this object is a quiet not-a-number value; otherwise,
    /// <c>false
    /// </c>
    /// .
    /// </returns>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsQuietNaN() {
      return this.Ef.IsQuietNaN();
    }

    /// <summary>Gets a value indicating whether this object is a signaling not-a-number
    /// value.</summary><returns><c>true
    /// </c>
    /// if this object is a signaling not-a-number value; otherwise,
    /// <c>false
    /// </c>
    /// .
    /// </returns>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsSignalingNaN() {
      return this.Ef.IsSignalingNaN();
    }

    /// <summary>Compares this extended float to another.</summary><param name='other'>An extended float to compare this one with.
    /// </param><returns>Less than 0 if this value is less than, 0 if equal to, or greater than 0
    /// if greater than the other value.
    /// </returns>
    public int CompareTo(ExtendedFloat other) {
      return this.Ef.CompareTo(other == null ? null : other.Ef);
    }

    /// <summary>Gets this value's sign: -1 if negative; 1 if positive; 0 if zero.</summary><value>This value's sign: -1 if negative; 1 if positive; 0 if zero.
    /// </value>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public int Sign {
      get {
        return this.Ef.Sign;
      }
    }

    internal EFloat Ef {
      get {
        return this.ef;
      }
    }
  }
}
