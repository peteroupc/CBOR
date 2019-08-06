/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using PeterO.Numbers;

#pragma warning disable CA1036 // This class is obsolete
namespace PeterO {
  /// <include file='../docs.xml'
  /// path='docs/doc[@name="T:PeterO.ExtendedFloat"]/*'/>
  [Obsolete(
"Use EFloat from PeterO.Numbers/com.upokecenter.numbers and the output of" +
"\u0020this class's ToString method.")]
  public sealed class ExtendedFloat : IComparable<ExtendedFloat>,
IEquatable<ExtendedFloat> {
    private readonly EFloat ef;

    internal ExtendedFloat(EFloat ef) {
      this.ef = ef;
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.Exponent"]/*'/>
    public BigInteger Exponent {
      get {
        return new BigInteger(this.Ef.Exponent);
      }
    }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.ExtendedFloat.UnsignedMantissa"]/*'/>
    public BigInteger UnsignedMantissa {
      get {
        return new BigInteger(this.Ef.UnsignedMantissa);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.Mantissa"]/*'/>
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

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.ExtendedFloat.EqualsInternal(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool EqualsInternal(ExtendedFloat otherValue) {
      if (otherValue == null) {
        throw new ArgumentNullException(nameof(otherValue));
      }
      return this.Ef.EqualsInternal(otherValue.Ef);
    }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.ExtendedFloat.Equals(PeterO.ExtendedFloat)"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool Equals(ExtendedFloat other) {
      return other!=null && this.Ef.Equals(other.Ef);
    }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.ExtendedFloat.Equals(System.Object)"]/*'/>
    public override bool Equals(object obj) {
      var bi = obj as ExtendedFloat;
      return (bi == null) ? false : this.Ef.Equals(bi.Ef);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.GetHashCode"]/*'/>
    public override int GetHashCode() {
      return this.Ef.GetHashCode();
    }
    #endregion

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.ExtendedFloat.Create(System.Int32,System.Int32)"]/*'/>
    public static ExtendedFloat Create(int mantissaSmall, int exponentSmall) {
      return new ExtendedFloat(EFloat.Create(mantissaSmall, exponentSmall));
    }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.ExtendedFloat.Create(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
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

    /// <summary>Creates a binary float from a text string that represents
    /// a number. Note that if the string contains a negative exponent, the
    /// resulting value might not be exact, in which case the resulting
    /// binary float will be an approximation of this decimal number's
    /// value. (NOTE: This documentation previously said the binary float
    /// will contain enough precision to accurately convert it to a 32-bit
    /// or 64-bit floating point number. Due to double rounding, this will
    /// generally not be the case for certain numbers converted from
    /// decimal to ExtendedFloat via this method and in turn converted to
    /// <c>double</c> or <c>float</c>.)
    /// <para>The format of the string generally consists of:</para>
    /// <list type=''>
    /// <item>An optional plus sign ("+" , U+002B) or minus sign ("-",
    /// U+002D) (if '-' , the value is negative.)</item>
    /// <item>One or more digits, with a single optional decimal point
    /// after the first digit and before the last digit.</item>
    /// <item>Optionally, "E+"/"e+" (positive exponent) or "E-"/"e-"
    /// (negative exponent) plus one or more digits specifying the
    /// exponent.</item></list>
    /// <para>The string can also be "-INF", "-Infinity", "Infinity",
    /// "INF", quiet NaN ("NaN") followed by any number of digits, or
    /// signaling NaN ("sNaN") followed by any number of digits, all in any
    /// combination of upper and lower case.</para>
    /// <para>All characters mentioned above are the corresponding
    /// characters in the Basic Latin range. In particular, the digits must
    /// be the basic digits 0 to 9 (U + 0030 to U + 0039). The string is
    /// not allowed to contain white space characters, including
    /// spaces.</para></summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='offset'>A zero-based index showing where the desired
    /// portion of <paramref name='str'/> begins.</param>
    /// <param name='length'>The length, in code units, of the desired
    /// portion of <paramref name='str'/> (but not more than <paramref
    /// name='str'/> 's length).</param>
    /// <param name='ctx'>A PrecisionContext object specifying the
    /// precision, rounding, and exponent range to apply to the parsed
    /// number. Can be null.</param>
    /// <returns>The parsed number, converted to arbitrary-precision binary
    /// float.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='length'/> is less than 0 or
    /// greater than <paramref name='str'/> 's length, or <paramref
    /// name='str'/> ' s length minus <paramref name='offset'/> is less
    /// than <paramref name='length'/>.</exception>
    /// <exception cref='ArgumentException'>Either &#x22;offset&#x22; or
    /// &#x22;length&#x22; is less than 0 or greater than
    /// &#x22;str&#x22;&#x27;s length, or &#x22;str&#x22;&#x27;s length
    /// minus &#x22;offset&#x22; is less than
    /// &#x22;length&#x22;.</exception>
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

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.ExtendedFloat.FromString(System.String)"]/*'/>
    public static ExtendedFloat FromString(string str) {
      return new ExtendedFloat(EFloat.FromString(str));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.ToString"]/*'/>
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

    /// <summary>A not-a-number value that signals an invalid operation
    /// flag when it's passed as an argument to any arithmetic operation in
    /// arbitrary-precision binary float.</summary>
    public static readonly ExtendedFloat SignalingNaN =
     new ExtendedFloat(EFloat.SignalingNaN);

    /// <summary>Positive infinity, greater than any other
    /// number.</summary>
    public static readonly ExtendedFloat PositiveInfinity =
     new ExtendedFloat(EFloat.PositiveInfinity);

    /// <summary>Negative infinity, less than any other number.</summary>
    public static readonly ExtendedFloat NegativeInfinity =
     new ExtendedFloat(EFloat.NegativeInfinity);

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.ExtendedFloat.IsNegativeInfinity"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsNegativeInfinity() {
      return this.Ef.IsNegativeInfinity();
    }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.ExtendedFloat.IsPositiveInfinity"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsPositiveInfinity() {
      return this.Ef.IsPositiveInfinity();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsNaN"]/*'/>
    public bool IsNaN() {
      return this.Ef.IsNaN();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsInfinity"]/*'/>
    public bool IsInfinity() {
      return this.Ef.IsInfinity();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.IsNegative"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsNegative {
      get {
        return this.Ef.IsNegative;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsQuietNaN"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsQuietNaN() {
      return this.Ef.IsQuietNaN();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedFloat.IsSignalingNaN"]/*'/>
    [Obsolete("Use EFloat from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsSignalingNaN() {
      return this.Ef.IsSignalingNaN();
    }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.ExtendedFloat.CompareTo(PeterO.ExtendedFloat)"]/*'/>
    public int CompareTo(ExtendedFloat other) {
      return this.Ef.CompareTo(other == null ? null : other.Ef);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedFloat.Sign"]/*'/>
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
