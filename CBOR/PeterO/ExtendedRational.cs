/*
Written by Peter O. in 2014.
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
    /// <c>PeterO.Numbers.ERational
    /// </c>
    /// in the
    /// <a href='https://www.nuget.org/packages/PeterO.Numbers'>
    /// <c>PeterO.Numbers
    /// </c>
    /// </a>
    /// library (in .NET), or
    /// <c>com.upokecenter.numbers.ERational
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
    /// Arbitrary-precision rational number. This class can't be inherited; this
    /// is a change in version 2.0 from previous versions, where the class was
    /// inadvertently left inheritable.
    /// <para><b>Thread safety:
    /// </b>
    /// Instances of this class are immutable, so they are inherently safe for
    /// use by multiple threads. Multiple instances of this object with the same
    /// properties are interchangeable, so they should not be compared using the
    /// "==" operator (which might only check if each side of the operator is
    /// the same instance).
    /// </para></summary>
    [Obsolete(
  "Use ERational from PeterO.Numbers/com.upokecenter.numbers and the output" +
"\u0020of this class's ToString method.")]
  public sealed class ExtendedRational : IComparable<ExtendedRational>,
    IEquatable<ExtendedRational> {
    /// <summary>A not-a-number value.</summary>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly ExtendedRational NaN =
      new ExtendedRational(ERational.NaN);

    /// <summary>Negative infinity, less than any other number.</summary>
    public static readonly ExtendedRational NegativeInfinity = new
      ExtendedRational(ERational.NegativeInfinity);

    /// <summary>A rational number for negative zero.</summary>
    public static readonly ExtendedRational NegativeZero =
      new ExtendedRational(ERational.NegativeZero);

    /// <summary>The rational number one.</summary>
    public static readonly ExtendedRational One =
      FromBigIntegerInternal(BigInteger.One);

    /// <summary>Positive infinity, greater than any other number.</summary>
    public static readonly ExtendedRational PositiveInfinity = new
      ExtendedRational(ERational.PositiveInfinity);

    /// <summary>A signaling not-a-number value.</summary>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly ExtendedRational SignalingNaN = new
      ExtendedRational(ERational.SignalingNaN);

    /// <summary>The rational number ten.</summary>
    public static readonly ExtendedRational Ten =
      FromBigIntegerInternal(BigInteger.valueOf(10));

    /// <summary>A rational number for zero.</summary>
    public static readonly ExtendedRational Zero =
      FromBigIntegerInternal(BigInteger.Zero);

    private readonly ERational er;

    /// <summary>Initializes a new instance of the <see cref='PeterO.ExtendedRational'/> class.</summary><param name='numerator'>An arbitrary-precision integer.
    /// </param><param name='denominator'>Another arbitrary-precision integer.
    /// </param><exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='numerator'/>
    /// or
    /// <paramref name='denominator'/>
    /// is null.</exception>
    public ExtendedRational(BigInteger numerator, BigInteger denominator) {
      if (denominator == null) {
        throw new ArgumentNullException(nameof(denominator));
      }
      if (numerator == null) {
        throw new ArgumentNullException(nameof(numerator));
      }
      this.er = new ERational(numerator.Ei, denominator.Ei);
    }

    internal ExtendedRational(ERational er) {
      if (er == null) {
        throw new ArgumentNullException(nameof(er));
      }
      this.er = er;
    }

    /// <summary>Gets this object's denominator.</summary><value>This object's denominator.
    /// </value>
    public BigInteger Denominator {
      get {
        return new BigInteger(this.Er.Denominator);
      }
    }

    /// <summary>Gets a value indicating whether this object is finite (not infinity or
    /// NaN).</summary><value><c>true
    /// </c>
    /// If this object is finite (not infinity or NaN); otherwise, .
    /// <c>false
    /// </c>
    /// .
    /// </value>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsFinite {
      get {
        return this.Er.IsFinite;
      }
    }

    /// <summary>Gets a value indicating whether this object's value is negative (including
    /// negative zero).</summary><value><c>true
    /// </c>
    /// If this object's value is negative; otherwise, .
    /// <c>false
    /// </c>
    /// .
    /// </value>
    public bool IsNegative {
      get {
        return this.Er.IsNegative;
      }
    }

    /// <summary>Gets a value indicating whether this object's value equals 0.</summary><value><c>true
    /// </c>
    /// If this object's value equals 0; otherwise, .
    /// <c>false
    /// </c>
    /// .
    /// </value>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsZero {
      get {
        return this.Er.IsZero;
      }
    }

    /// <summary>Gets this object's numerator.</summary><value>This object's numerator. If this object is a not-a-number value, returns
    /// the diagnostic information (which will be negative if this object is
    /// negative).
    /// </value>
    public BigInteger Numerator {
      get {
        return new BigInteger(this.Er.Numerator);
      }
    }

    /// <summary>Gets the sign of this rational number.</summary><value>Zero if this value is zero or negative zero; -1 if this value is less than
    /// 0; and 1 if this value is greater than 0.
    /// </value>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public int Sign {
      get {
        return this.Er.Sign;
      }
    }

    /// <summary>Gets this object's numerator with the sign removed.</summary><value>This object's numerator. If this object is a not-a-number value, returns
    /// the diagnostic information.
    /// </value>
    public BigInteger UnsignedNumerator {
      get {
        return new BigInteger(this.Er.UnsignedNumerator);
      }
    }

    internal ERational Er {
      get {
        return this.er;
      }
    }

    /// <summary>Creates a rational number with the given numerator and denominator.</summary><param name='numeratorSmall'>The parameter
    /// <paramref name='numeratorSmall'/>
    /// is a 32-bit signed integer.
    /// </param><param name='denominatorSmall'>The parameter
    /// <paramref name='denominatorSmall'/>
    /// is a 32-bit signed integer.
    /// </param><returns>An arbitrary-precision rational number.
    /// </returns>
    public static ExtendedRational Create(
      int numeratorSmall,
      int denominatorSmall) {
      return new ExtendedRational(
  ERational.Create(
    numeratorSmall,
    denominatorSmall));
    }

    /// <summary>Creates a rational number with the given numerator and denominator.</summary><param name='numerator'>An arbitrary-precision integer.
    /// </param><param name='denominator'>Another arbitrary-precision integer.
    /// </param><returns>An arbitrary-precision rational number.
    /// </returns><exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='numerator'/>
    /// or
    /// <paramref name='denominator'/>
    /// is null.</exception>
    public static ExtendedRational Create(
      BigInteger numerator,
      BigInteger denominator) {
      if (numerator == null) {
        throw new ArgumentNullException(nameof(numerator));
      }
      if (denominator == null) {
        throw new ArgumentNullException(nameof(denominator));
      }
      return new ExtendedRational(
  ERational.Create(
    numerator.Ei,
    denominator.Ei));
    }

    /// <summary>Converts this object to a text string.</summary><returns>A string representation of this object. The result can be Infinity, NaN,
    /// or sNaN (with a minus sign before it for negative values), or a number of
    /// the following form: [-]numerator/denominator.
    /// </returns>
    public override string ToString() {
      return this.Er.ToString();
    }

    internal static ERational FromLegacy(ExtendedRational bei) {
      return bei.Er;
    }

    internal static ExtendedRational ToLegacy(ERational ei) {
      return new ExtendedRational(ei);
    }

    private static ExtendedRational FromBigIntegerInternal(BigInteger bigint) {
      return new ExtendedRational(ERational.FromEInteger(bigint.Ei));
    }

    /// <summary>Compares this value to another.</summary><param name='other'>The parameter
    /// <paramref name='other'/>
    /// is an ExtendedRational object.
    /// </param><returns>Less than 0 if this value is less than, 0 if equal to, or greater than 0
    /// if greater than the other value.
    /// </returns>
    public int CompareTo(ExtendedRational other) {
      return this.Er.CompareTo(other == null ? null : other.Er);
    }

    /// <summary>Checks whether this and another value are equal.</summary><param name='other'>The parameter
    /// <paramref name='other'/>
    /// is an ExtendedRational object.
    /// </param><returns>Either
    /// <c>true
    /// </c>
    /// or
    /// <c>false
    /// </c>
    /// .
    /// </returns>
    public bool Equals(ExtendedRational other) {
      return this.Er.Equals(other == null ? null : other.Er);
    }

    /// <summary>Checks whether this and another value are equal.</summary><param name='obj'>The parameter
    /// <paramref name='obj'/>
    /// is an arbitrary object.
    /// </param><returns>Either
    /// <c>true
    /// </c>
    /// or
    /// <c>false
    /// </c>
    /// .
    /// </returns>
    public override bool Equals(object obj) {
      var other = obj as ExtendedRational;
      return this.Er.Equals(other == null ? null : other.Er);
    }

    /// <summary>Calculates the hash code for this object. No application or process IDs
    /// are used in the hash code calculation.</summary><returns>A 32-bit signed integer.
    /// </returns>
    public override int GetHashCode() {
      return this.Er.GetHashCode();
    }
  }
}
