/*
Written by Peter O. in 2013.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
#pragma warning disable SA1300
using System;
using PeterO.Numbers;

namespace PeterO {
    /// <xmlbegin id="329"/><summary><para><b>This class is largely obsolete. It will be replaced by a new version
    /// of this class in a different namespace/package and library, called
    /// <c>PeterO.Numbers.EInteger
    /// </c>
    /// in the
    /// <a href='https://www.nuget.org/packages/PeterO.Numbers'>
    /// <c>PeterO.Numbers
    /// </c>
    /// </a>
    /// library (in .NET), or
    /// <c>com.upokecenter.numbers.EInteger
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
    /// An arbitrary-precision integer.
    /// <para><b>Thread safety:
    /// </b>
    /// Instances of this class are immutable, so they are inherently safe for
    /// use by multiple threads. Multiple instances of this object with the same
    /// value are interchangeable, but they should be compared using the
    /// "Equals" method rather than the "==" operator.
    /// </para></summary>
    /// <summary><para><b>This class is largely obsolete. It will be replaced by a new version
    /// of this class in a different namespace/package and library, called
    /// <c>PeterO.Numbers.EInteger
    /// </c>
    /// in the
    /// <a href='https://www.nuget.org/packages/PeterO.Numbers'>
    /// <c>PeterO.Numbers
    /// </c>
    /// </a>
    /// library (in .NET), or
    /// <c>com.upokecenter.numbers.EInteger
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
    /// An arbitrary-precision integer.
    /// <para><b>Thread safety:
    /// </b>
    /// Instances of this class are immutable, so they are inherently safe for
    /// use by multiple threads. Multiple instances of this object with the same
    /// value are interchangeable, but they should be compared using the
    /// "Equals" method rather than the "==" operator.
    /// </para></summary>
    ///
[Obsolete(
  "Use EInteger from PeterO.Numbers/com.upokecenter.numbers and the output" +
"\u0020of this class's ToString method.")]
  public sealed partial class BigInteger : IComparable<BigInteger>,
    IEquatable<BigInteger> {
    /// <summary>BigInteger for the number one.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "BigInteger is immutable")]
#endif

    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly BigInteger ONE = new BigInteger(EInteger.One);

    private static readonly BigInteger ValueOneValue = new
      BigInteger(EInteger.One);

    private readonly EInteger ei;

    internal BigInteger(EInteger ei) {
      if (ei == null) {
        throw new ArgumentNullException(nameof(ei));
      }
      this.ei = ei;
    }

    internal static BigInteger ToLegacy(EInteger ei) {
      return new BigInteger(ei);
    }

    internal static EInteger FromLegacy(BigInteger bei) {
      return bei.Ei;
    }

  private static readonly BigInteger ValueZeroValue = new
      BigInteger(EInteger.Zero);

    internal EInteger Ei {
      get {
        return this.ei;
      }
    }

    /// <summary>Initializes an arbitrary-precision integer from an array of bytes.</summary><param name='bytes'>A byte array consisting of the two's-complement form of the
    /// arbitrary-precision integer to create. The last byte contains the lowest
    /// 8-bits, the next-to-last contains the next lowest 8 bits, and so on. To
    /// encode negative numbers, take the absolute value of the number, subtract
    /// by 1, encode the number into bytes, XOR each byte, and if the
    /// most-significant bit of the first byte isn't set, add an additional byte
    /// at the start with the value 255. For little-endian, the byte order is
    /// reversed from the byte order just discussed.
    /// </param><param name='littleEndian'>If true, the byte order is little-endian, or least-significant-byte first.
    /// If false, the byte order is big-endian, or most-significant-byte first.
    /// </param><returns>An arbitrary-precision integer. Returns 0 if the byte array's length is 0.
    /// </returns><exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='bytes'/>
    /// is null.</exception>
  public static BigInteger fromBytes(byte[] bytes, bool littleEndian) {
      return new BigInteger(EInteger.FromBytes(bytes, littleEndian));
    }

    /// <summary>Converts a string to an arbitrary-precision integer.</summary><param name='str'>A text string. The string must contain only characters allowed by the
    /// given radix, except that it may start with a minus sign ("-", U+002D) to
    /// indicate a negative number. The string is not allowed to contain white
    /// space characters, including spaces.
    /// </param><param name='radix'>A base from 2 to 36. Depending on the radix, the string can use the basic
    /// digits 0 to 9 (U+0030 to U+0039) and then the basic letters A to Z (U+0041
    /// to U+005A). For example, 0-9 in radix 10, and 0-9, then A-F in radix 16.
    /// </param><returns>An arbitrary-precision integer with the same value as given in the string.
    /// </returns><exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='str'/>
    /// is null.</exception><exception cref='System.ArgumentException'>The parameter
    /// <paramref name='radix'/>
    /// is less than 2 or greater than 36.</exception><exception cref='System.FormatException'>The string is empty or in an invalid format.</exception>
  public static BigInteger fromRadixString(string str, int radix) {
      return new BigInteger(EInteger.FromRadixString(str, radix));
    }

    /// <summary>Converts a string to an arbitrary-precision integer.</summary><param name='str'>A text string. The string must contain only basic digits 0 to 9 (U+0030 to
    /// U+0039), except that it may start with a minus sign ("-", U+002D) to
    /// indicate a negative number. The string is not allowed to contain white
    /// space characters, including spaces.
    /// </param><returns>An arbitrary-precision integer with the same value as given in the string.
    /// </returns><exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='str'/>
    /// is null.</exception><exception cref='System.FormatException'>The parameter
    /// <paramref name='str'/>
    /// is in an invalid format.</exception>
  public static BigInteger fromString(string str) {
return new BigInteger(EInteger.FromString(str));
}

    /// <summary>Converts a 64-bit signed integer to a big integer.</summary><param name='longerValue'>The parameter
    /// <paramref name='longerValue'/>
    /// is a 64-bit signed integer.
    /// </param><returns>An arbitrary-precision integer with the same value as the 64-bit number.
    /// </returns>
  public static BigInteger valueOf(long longerValue) {
      return new BigInteger(EInteger.FromInt64(longerValue));
 }

    /// <summary>Finds the minimum number of bits needed to represent this object's value,
    /// except for its sign. If the value is negative, finds the number of bits in
    /// a value equal to this object's absolute value minus 1.</summary><returns>The number of bits in this object's value. Returns 0 if this object's
    /// value is 0 or negative 1.
    /// </returns>
  public int bitLength() {
return this.Ei.GetSignedBitLength();
 }

    /// <inheritdoc/><summary>Determines whether this object and another object are equal.</summary><param name='obj'>The parameter
    /// <paramref name='obj'/>
    /// is an arbitrary object.
    /// </param><returns><c>true
    /// </c>
    /// if this object and another object are equal; otherwise,
    /// <c>false
    /// </c>
    /// .
    /// </returns>
  public override bool Equals(object obj) {
      var bi = obj as BigInteger;
      return (bi == null) ? false : this.Ei.Equals(bi.Ei);
}

    /// <summary>Returns the hash code for this instance. No application or process IDs are
    /// used in the hash code calculation.</summary><returns>A 32-bit signed integer.
    /// </returns>
  public override int GetHashCode() {
      return this.Ei.GetHashCode();
 }

    /// <summary>Returns a byte array of this object's value. The byte array will take the
    /// form of the number's two' s-complement representation, using the fewest
    /// bytes necessary to represent its value unambiguously. If this value is
    /// negative, the bits that appear "before" the most significant bit of the
    /// number will be all ones.</summary><param name='littleEndian'>If true, the least significant bits will appear first.
    /// </param><returns>A byte array. If this value is 0, returns a byte array with the single
    /// element 0.
    /// </returns>
  public byte[] toBytes(bool littleEndian) {
      return this.Ei.ToBytes(littleEndian);
 }

    /// <summary>Generates a string representing the value of this object, in the given
    /// radix.</summary><param name='radix'>A radix from 2 through 36. For example, to generate a hexadecimal
    /// (base-16) string, specify 16. To generate a decimal (base-10) string,
    /// specify 10.
    /// </param><returns>A string representing the value of this object. If this value is 0,
    /// returns "0". If negative, the string will begin with a hyphen/minus ("-").
    /// Depending on the radix, the string will use the basic digits 0 to 9
    /// (U+0030 to U+0039) and then the basic letters A to Z (U+0041 to U+005A).
    /// For example, 0-9 in radix 10, and 0-9, then A-F in radix 16.
    /// </returns><exception cref='System.ArgumentException'>The parameter "index" is less than 0, "endIndex" is less than 0, or either
    /// is greater than the string's length, or "endIndex" is less than "index" ;
    /// or radix is less than 2 or greater than 36.</exception>
  public string toRadixString(int radix) {
      return this.Ei.ToRadixString(radix);
 }

    /// <summary>Converts this object to a text string in base 10.</summary><returns>A string representation of this object. If negative, the string will begin
    /// with a minus sign ("-", U+002D). The string will use the basic digits 0 to
    /// 9 (U+0030 to U+0039).
    /// </returns>
  public override string ToString() {
      return this.Ei.ToString();
    }

    /// <summary>Compares this value to another.</summary><param name='other'>The parameter
    /// <paramref name='other'/>
    /// is an arbitrary-precision integer.
    /// </param><returns>Less than 0 if this value is less than, 0 if equal to, or greater than 0
    /// if greater than the other value.
    /// </returns>
    public int CompareTo(BigInteger other) {
      return this.Ei.CompareTo(other == null ? null : other.Ei);
    }
  }
}
