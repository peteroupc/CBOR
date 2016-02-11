/*
Written in 2013 by Peter O.

Parts of the code were adapted by Peter O. from
the public-domain code from the library
CryptoPP by Wei Dai.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using PeterO.Numbers;

namespace PeterO {
    /// <include file='../docs.xml'
    /// path='docs/doc[@name="T:PeterO.BigInteger"]/*'/>
  public sealed partial class BigInteger : IComparable<BigInteger>,
    IEquatable<BigInteger> {
    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.BigInteger.ONE"]/*'/>
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
  throw new ArgumentNullException("ei");
}
      this.ei = ei;
    }

    internal static BigInteger ToLegacy(EInteger ei) {
      return new BigInteger(ei);
    }

    internal static EInteger FromLegacy(BigInteger bei) {
      return bei.Ei;
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.BigInteger.TEN"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "BigInteger is immutable")]
#endif

    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public static readonly BigInteger TEN = BigInteger.valueOf(10);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="F:PeterO.BigInteger.ZERO"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "BigInteger is immutable")]
#endif
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static readonly BigInteger ZERO = new
      BigInteger(EInteger.Zero);

  private static readonly BigInteger ValueZeroValue = new
      BigInteger(EInteger.Zero);

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.BigInteger.IsEven"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public bool IsEven { get {
 return this.Ei.IsEven;
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.BigInteger.IsZero"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public bool IsZero { get {
 return this.Ei.IsZero;
} }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.BigInteger.Sign"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public int Sign { get {
 return this.Ei.Sign;
} }

    internal EInteger Ei {
      get {
        return this.ei;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.fromByteArray(System.Byte[],System.Boolean)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public static BigInteger fromByteArray(byte[] bytes, bool littleEndian) {
      return new BigInteger(EInteger.FromBytes(bytes, littleEndian));
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.fromBytes(System.Byte[],System.Boolean)"]/*'/>
  public static BigInteger fromBytes(byte[] bytes, bool littleEndian) {
      return new BigInteger(EInteger.FromBytes(bytes, littleEndian));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.fromRadixString(System.String,System.Int32)"]/*'/>
  public static BigInteger fromRadixString(string str, int radix) {
      return new BigInteger(EInteger.FromRadixString(str, radix));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.fromRadixSubstring(System.String,System.Int32,System.Int32,System.Int32)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public static BigInteger fromRadixSubstring(
      string str,
      int radix,
      int index,
      int endIndex) {
 return new BigInteger(
EInteger.FromRadixSubstring(
str,
radix,
index,
endIndex));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.fromString(System.String)"]/*'/>
  public static BigInteger fromString(string str) {
return new BigInteger(EInteger.FromString(str));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.fromSubstring(System.String,System.Int32,System.Int32)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public static BigInteger fromSubstring(
string str,
int index,
int endIndex) {
return new BigInteger(EInteger.FromSubstring(str, index, endIndex));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.valueOf(System.Int64)"]/*'/>
  public static BigInteger valueOf(long longerValue) {
      return new BigInteger(EInteger.FromInt64(longerValue));
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.abs"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger abs() {
      return new BigInteger(this.Ei.Abs());
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.add(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger add(BigInteger bigintAugend) {
      if (bigintAugend == null) {
  throw new ArgumentNullException("bigintAugend");
}
return new BigInteger(this.Ei.Add(bigintAugend.Ei));
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.bitLength"]/*'/>
  public int bitLength() {
return this.Ei.GetSignedBitLength();
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.canFitInInt"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public bool canFitInInt() {
return this.Ei.CanFitInInt32();
      }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.CompareTo(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public int CompareTo(BigInteger other) {
      return (other == null) ? 1 : this.Ei.CompareTo(other.Ei);
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.divide(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger divide(BigInteger bigintDivisor) {
      if (bigintDivisor == null) {
        throw new ArgumentNullException("bigintDivisor");
      }
      return new BigInteger(this.Ei.Divide(bigintDivisor.Ei));
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.divideAndRemainder(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger[] divideAndRemainder(BigInteger divisor) {
      if (divisor == null) {
  throw new ArgumentNullException("divisor");
}
      EInteger[] eia = this.Ei.DivRem(divisor.Ei);
    return new BigInteger[] { new BigInteger(eia[0]), new BigInteger(eia[1])
        };
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.Equals(System.Object)"]/*'/>
  public override bool Equals(object obj) {
      var bi = obj as BigInteger;
      return (bi == null) ? false : this.Ei.Equals(bi.Ei);
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.gcd(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger gcd(BigInteger bigintSecond) {
  if (bigintSecond == null) {
  throw new ArgumentNullException("bigintSecond");
}
return new BigInteger(this.Ei.Gcd(bigintSecond.Ei));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.getDigitCount"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public int getDigitCount() {
      return this.Ei.GetDigitCount();
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.GetHashCode"]/*'/>
  public override int GetHashCode() {
      return this.Ei.GetHashCode();
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.getLowBit"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public int getLowBit() {
      return this.IsZero ? 0 : this.Ei.GetLowBit();
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.getLowestSetBit"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public int getLowestSetBit() {
      return this.getLowBit();
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.getUnsignedBitLength"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public int getUnsignedBitLength() {
      return this.getUnsignedBitLength();
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.intValue"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public int intValue() {
return this.Ei.AsInt32Checked();
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.intValueChecked"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public int intValueChecked() {
return this.Ei.AsInt32Checked();
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.intValueUnchecked"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public int intValueUnchecked() {
return this.Ei.AsInt32Unchecked();
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.longValue"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public long longValue() {
return this.Ei.AsInt64Checked();
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.longValueChecked"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public long longValueChecked() {
return this.Ei.AsInt64Checked();
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.longValueUnchecked"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public long longValueUnchecked() {
      return this.Ei.AsInt64Unchecked();
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.mod(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger mod(BigInteger divisor) {
  if (divisor == null) {
  throw new ArgumentNullException("divisor");
}
return new BigInteger(this.Ei.Mod(divisor.Ei));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.ModPow(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger ModPow(BigInteger pow, BigInteger mod) {
  if (pow == null) {
  throw new ArgumentNullException("pow");
}
  if (mod == null) {
  throw new ArgumentNullException("mod");
}
return new BigInteger(this.Ei.ModPow(pow.Ei, mod.Ei));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.multiply(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger multiply(BigInteger bigintMult) {
      if (bigintMult == null) {
        throw new ArgumentNullException("bigintMult");
      }
      return new BigInteger(this.Ei.Multiply(bigintMult.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.negate"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger negate() {
      return new BigInteger(this.Ei.Negate());
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.pow(System.Int32)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger pow(int powerSmall) {
return new BigInteger(this.Ei.Pow(powerSmall));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.PowBigIntVar(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger PowBigIntVar(BigInteger power) {
  if (power == null) {
  throw new ArgumentNullException("power");
}
return new BigInteger(this.Ei.PowBigIntVar(power.Ei));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.remainder(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger remainder(BigInteger divisor) {
  if (divisor == null) {
  throw new ArgumentNullException("divisor");
}
return new BigInteger(this.Ei.Remainder(divisor.Ei));
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.shiftLeft(System.Int32)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger shiftLeft(int numberBits) {
      return new BigInteger(this.Ei.ShiftLeft(numberBits));
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.shiftRight(System.Int32)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger shiftRight(int numberBits) {
      return new BigInteger(this.Ei.ShiftRight(numberBits));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.sqrt"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger sqrt() {
      return new BigInteger(this.Ei.Sqrt());
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.sqrtWithRemainder"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger[] sqrtWithRemainder() {
      EInteger[] eia = this.Ei.SqrtRem();
      return new BigInteger[] { new BigInteger(eia[0]), new BigInteger(eia[1])
        };
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.subtract(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public BigInteger subtract(BigInteger subtrahend) {
      if (subtrahend == null) {
  throw new ArgumentNullException("subtrahend");
}
      return new BigInteger(this.Ei.Subtract(subtrahend.Ei));
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.testBit(System.Int32)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public bool testBit(int index) {
return this.Ei.GetSignedBit(index);
}

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.toByteArray(System.Boolean)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
  public byte[] toByteArray(bool littleEndian) {
      return this.toBytes(littleEndian);
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.toBytes(System.Boolean)"]/*'/>
  public byte[] toBytes(bool littleEndian) {
      return this.Ei.ToBytes(littleEndian);
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.toRadixString(System.Int32)"]/*'/>
  public string toRadixString(int radix) {
      return this.Ei.ToRadixString(radix);
 }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.ToString"]/*'/>
  public override string ToString() {
      return this.Ei.ToString();
    }
  }
}
