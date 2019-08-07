/*
Written by Peter O. in 2014.
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
  /// path='docs/doc[@name="T:PeterO.ExtendedRational"]/*'/>
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

    /// <summary>Positive infinity, greater than any other
    /// number.</summary>
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

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.ExtendedRational'/> class.</summary>
    /// <param name='numerator'>An arbitrary-precision integer.</param>
    /// <param name='denominator'>Another arbitrary-precision
    /// integer.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='numerator'/> or <paramref name='denominator'/> is
    /// null.</exception>
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

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedRational.Denominator"]/*'/>
    public BigInteger Denominator {
      get {
        return new BigInteger(this.Er.Denominator);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedRational.IsFinite"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsFinite {
      get {
        return this.Er.IsFinite;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedRational.IsNegative"]/*'/>
    public bool IsNegative {
      get {
        return this.Er.IsNegative;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedRational.IsZero"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsZero {
      get {
        return this.Er.IsZero;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedRational.Numerator"]/*'/>
    public BigInteger Numerator {
      get {
        return new BigInteger(this.Er.Numerator);
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.ExtendedRational.Sign"]/*'/>
    [Obsolete("Use ERational from PeterO.Numbers/com.upokecenter.numbers.")]
    public int Sign {
      get {
        return this.Er.Sign;
      }
    }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.ExtendedRational.UnsignedNumerator"]/*'/>
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

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.ExtendedRational.Create(System.Int32,System.Int32)"]/*'/>
    public static ExtendedRational Create(
      int numeratorSmall,
      int denominatorSmall) {
      return new ExtendedRational(
  ERational.Create(
    numeratorSmall,
    denominatorSmall));
    }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.ExtendedRational.Create(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
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

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.ToString"]/*'/>
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

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.ExtendedRational.CompareTo(PeterO.ExtendedRational)"]/*'/>
    public int CompareTo(ExtendedRational other) {
      return this.Er.CompareTo(other == null ? null : other.Er);
    }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.ExtendedRational.Equals(PeterO.ExtendedRational)"]/*'/>
    public bool Equals(ExtendedRational other) {
      return this.Er.Equals(other == null ? null : other.Er);
    }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.ExtendedRational.Equals(System.Object)"]/*'/>
    public override bool Equals(object obj) {
      var other = obj as ExtendedRational;
      return this.Er.Equals(other == null ? null : other.Er);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.ExtendedRational.GetHashCode"]/*'/>
    public override int GetHashCode() {
      return this.Er.GetHashCode();
    }
  }
}
