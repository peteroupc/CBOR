/*
Written by Peter O. in 2013.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
#pragma warning disable SA1300
#pragma warning disable CA1036 // This class is obsolete
using System;
using PeterO.Numbers;

namespace PeterO {
    /// <summary>
    /// <para><b>This class is largely obsolete. It will be replaced by a
    /// new version of this class in a different namespace/package and
    /// library, called <c>PeterO.Numbers.EInteger</c> in the
    /// <a
    ///   href='https://www.nuget.org/packages/PeterO.Numbers'><c>PeterO.Numbers</c></a>
    /// library (in .NET), or <c>com.upokecenter.numbers.EInteger</c> in
    /// the
    /// <a
    ///   href='https://github.com/peteroupc/numbers-java'><c>com.github.peteroupc/numbers</c></a>
    /// artifact (in Java). This new class can be used in the
    /// <c>CBORObject.FromObject(object)</c> method (by including the new
    /// library in your code, among other things).</b></para> An
    /// arbitrary-precision integer.
    /// <para><b>Thread safety:</b> Instances of this class are immutable,
    /// so they are inherently safe for use by multiple threads. Multiple
    /// instances of this object with the same value are interchangeable,
    /// but they should be compared using the "Equals" method rather than
    /// the "==" operator.</para></summary>
  [Obsolete(
    "Use EInteger from PeterO.Numbers/com.upokecenter.numbers and the output" +
    "\u0020of this class's ToString method.")]
  [System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Design",
    "CA1036",
    Justification = "This class is obsolete.")]
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

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.BigInteger.fromBytes(System.Byte[],System.Boolean)"]/*'/>
    public static BigInteger fromBytes(byte[] bytes, bool littleEndian) {
      return new BigInteger(EInteger.FromBytes(bytes, littleEndian));
    }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.BigInteger.fromRadixString(System.String,System.Int32)"]/*'/>
    public static BigInteger fromRadixString(string str, int radix) {
      return new BigInteger(EInteger.FromRadixString(str, radix));
    }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.BigInteger.fromString(System.String)"]/*'/>
    public static BigInteger fromString(string str) {
return new BigInteger(EInteger.FromString(str));
}

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.BigInteger.valueOf(System.Int64)"]/*'/>
    public static BigInteger valueOf(long longerValue) {
      return new BigInteger(EInteger.FromInt64(longerValue));
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.bitLength"]/*'/>
    public int bitLength() {
return this.Ei.GetSignedBitLength();
 }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.BigInteger.Equals(System.Object)"]/*'/>
    public override bool Equals(object obj) {
      var bi = obj as BigInteger;
      return (bi == null) ? false : this.Ei.Equals(bi.Ei);
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.GetHashCode"]/*'/>
    public override int GetHashCode() {
      return this.Ei.GetHashCode();
 }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.BigInteger.toBytes(System.Boolean)"]/*'/>
    public byte[] toBytes(bool littleEndian) {
      return this.Ei.ToBytes(littleEndian);
 }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.BigInteger.toRadixString(System.Int32)"]/*'/>
    public string toRadixString(int radix) {
      return this.Ei.ToRadixString(radix);
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.ToString"]/*'/>
    public override string ToString() {
      return this.Ei.ToString();
    }

    /// <include file='../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.BigInteger.CompareTo(PeterO.BigInteger)"]/*'/>
    public int CompareTo(BigInteger other) {
      return this.Ei.CompareTo(other == null ? null : other.Ei);
    }
  }
}
