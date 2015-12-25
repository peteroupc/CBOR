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
    /// <summary>An arbitrary-precision integer.
    /// <para>Instances of this class are immutable, so they are inherently
    /// safe for use by multiple threads. Multiple instances of this object
    /// with the same value are interchangeable, so they should not be
    /// compared using the "==" operator (which only checks if each side of
    /// the operator is the same instance).</para></summary>
  public sealed partial class BigInteger : IComparable<BigInteger>,
    IEquatable<BigInteger> {
    /// <summary>BigInteger object for the number one.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "BigInteger is immutable")]
#endif

    public static readonly BigInteger ONE = new BigInteger(EInteger.ONE);

    internal EInteger ei;

    internal BigInteger(EInteger ei) {
      if ((ei) == null) {
  throw new ArgumentNullException("ei");
}
      this.ei = ei;
    }

    /// <summary>BigInteger object for the number ten.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "BigInteger is immutable")]
#endif

    public static readonly BigInteger TEN = BigInteger.valueOf(10);

    /// <summary>BigInteger object for the number zero.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "BigInteger is immutable")]
#endif
    public static readonly BigInteger ZERO = new BigInteger(EInteger.ZERO);

    /// <summary>Gets a value indicating whether this value is
    /// even.</summary>
    /// <value>True if this value is even; otherwise, false.</value>
    public bool IsEven { get {
 return this.ei.IsEven;
} }

    /// <summary>Gets a value indicating whether this value is 0.</summary>
    /// <value>True if this value is 0; otherwise, false.</value>
    public bool IsZero { get {
 return this.ei.IsZero;
} }

    /// <summary>Gets the sign of this object's value.</summary>
    /// <value>0 if this value is zero; -1 if this value is negative, or 1
    /// if this value is positive.</value>
    public int Sign { get {
 return this.ei.Sign;
} }

    /// <summary>Initializes a BigInteger object from an array of
    /// bytes.</summary>
    /// <param name='bytes'>A byte array.</param>
    /// <param name='littleEndian'>A Boolean object.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    [Obsolete("Renamed to 'fromBytes'.")]
    public static BigInteger fromByteArray(byte[] bytes, bool littleEndian) {
      return new BigInteger(EInteger.fromBytes(bytes, littleEndian));
 }

    /// <summary>Initializes a BigInteger object from an array of
    /// bytes.</summary>
    /// <param name='bytes'>A byte array.</param>
    /// <param name='littleEndian'>A Boolean object.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    public static BigInteger fromBytes(byte[] bytes, bool littleEndian) {
      return new BigInteger(EInteger.fromBytes(bytes, littleEndian));
    }

    /// <summary>Converts a string to an arbitrary-precision integer. The
    /// string portion can begin with a minus sign ("-" , U+002D) to
    /// indicate that it's negative.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='radix'>A base from 2 to 36. Depending on the radix,
    /// the string can use the basic digits 0 to 9 (U + 0030 to U + 0039)
    /// and then the basic letters A to Z (U + 0041 to U + 005A). For
    /// example, 0-9 in radix 10, and 0-9, then A-F in radix 16.</param>
    /// <returns>A BigInteger object with the same value as given in the
    /// string.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='radix'/> is less than 2 or greater than 36.</exception>
    /// <exception cref='FormatException'>The string is empty or in an
    /// invalid format.</exception>
    /// <example>
    ///  The following example (C#) converts a number in the
    /// form of a hex string to a big integer.
    /// <code>
    /// public static BigInteger HexToBigInteger(string hexString) {
    ///   // Parse the hexadecimal string as a big integer.  Will
    ///   // throw a FormatException if the parsing fails
    ///   var bigInteger = BigInteger.fromRadixString(hexString, 16);
    ///   // Optional: Check if the parsed integer is negative
    ///   if (bigInteger.Sign &lt; 0) {
    ///     throw new FormatException("negative hex string");
    ///   }
    ///   return bigInteger;
    /// }
    /// </code>
    /// </example>
    public static BigInteger fromRadixString(string str, int radix) {
      return new BigInteger(EInteger.fromRadixString(str, radix));
    }

    /// <summary>Converts a portion of a string to an arbitrary-precision
    /// integer in a given radix. The string portion can begin with a minus
    /// sign ("-" , U+002D) to indicate that it's negative.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='radix'>A base from 2 to 36. Depending on the radix,
    /// the string can use the basic digits 0 to 9 (U + 0030 to U + 0039)
    /// and then the basic letters A to Z (U + 0041 to U + 005A). For
    /// example, 0-9 in radix 10, and 0-9, then A-F in radix 16.</param>
    /// <param name='index'>The index of the string that starts the string
    /// portion.</param>
    /// <param name='endIndex'>The index of the string that ends the string
    /// portion. The length will be index + endIndex - 1.</param>
    /// <returns>A BigInteger object with the same value as given in the
    /// string portion.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='index'/> is less than 0, <paramref name='endIndex'/> is less
    /// than 0, or either is greater than the string's length, or <paramref
    /// name='endIndex'/> is less than <paramref name='index'/>
    /// .</exception>
    /// <exception cref='FormatException'>The string portion is empty or in
    /// an invalid format.</exception>
    public static BigInteger fromRadixSubstring(
      string str,
      int radix,
      int index,
      int endIndex) {
 return new BigInteger(EInteger.fromRadixSubstring(str,
        radix, index, endIndex));
    }

    /// <summary>Converts a string to an arbitrary-precision
    /// integer.</summary>
    /// <param name='str'>A string containing only basic digits 0 to 9 (U +
    /// 0030 to U + 0039), except that it may start with a minus sign ("-",
    /// U + 002D).</param>
    /// <returns>A BigInteger object with the same value as given in the
    /// string.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='FormatException'>The parameter <paramref
    /// name='str'/> is in an invalid format.</exception>
    public static BigInteger fromString(string str) {
return new BigInteger(EInteger.fromString(str));
}

    /// <summary>Converts a portion of a string to an arbitrary-precision
    /// integer. The string portion can begin with a minus sign ("-",
    /// U+002D) to indicate that it's negative.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>The index of the string that starts the string
    /// portion.</param>
    /// <param name='endIndex'>The index of the string that ends the string
    /// portion. The length will be index + endIndex - 1.</param>
    /// <returns>A BigInteger object with the same value as given in the
    /// string portion.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='index'/> is less than 0, <paramref name='endIndex'/> is less
    /// than 0, or either is greater than the string's length, or <paramref
    /// name='endIndex'/> is less than <paramref name='index'/>
    /// .</exception>
    /// <exception cref='FormatException'>The string portion is empty or in
    /// an invalid format.</exception>
    public static BigInteger fromSubstring(string str,
      int index,
      int endIndex) {
return new BigInteger(EInteger.fromSubstring(str, index, endIndex));
}

    /// <summary>Converts a 64-bit signed integer to a big
    /// integer.</summary>
    /// <param name='longerValue'>A 64-bit signed integer.</param>
    /// <returns>A BigInteger object with the same value as the 64-bit
    /// number.</returns>
    public static BigInteger valueOf(long longerValue) {
      return new BigInteger(EInteger.valueOf(longerValue));
 }

    /// <summary>Returns the absolute value of this object's
    /// value.</summary>
    /// <returns>This object's value with the sign removed.</returns>
    public BigInteger abs() {
      return new BigInteger(this.ei.abs());
 }

    /// <summary>Adds this object and another object.</summary>
    /// <param name='bigintAugend'>Another BigInteger object.</param>
    /// <returns>The sum of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintAugend'/> is null.</exception>
    public BigInteger add(BigInteger bigintAugend) {
      if ((bigintAugend) == null) {
  throw new ArgumentNullException("bigintAugend");
}
return new BigInteger(this.ei.add(bigintAugend.ei));
 }

    /// <summary>Finds the minimum number of bits needed to represent this
    /// object&#x27;s value, except for its sign. If the value is negative,
    /// finds the number of bits in a value equal to this object's absolute
    /// value minus 1.</summary>
    /// <returns>The number of bits in this object's value. Returns 0 if
    /// this object's value is 0 or negative 1.</returns>
    public int bitLength() {
return this.ei.bitLength();
 }

    /// <summary>Returns whether this object's value can fit in a 32-bit
    /// signed integer.</summary>
    /// <returns>True if this object's value is MinValue or greater, and
    /// MaxValue or less; otherwise, false.</returns>
    public bool canFitInInt() {
return this.ei.canFitInInt();
      }

    /// <summary>Compares a BigInteger object with this instance.</summary>
    /// <param name='other'>A BigInteger object.</param>
    /// <returns>Zero if the values are equal; a negative number if this
    /// instance is less, or a positive number if this instance is
    /// greater.</returns>
    public int CompareTo(BigInteger other) {
      return (other == null) ? (1) : (this.ei.CompareTo(other.ei));
 }

    /// <summary>Divides this instance by the value of a BigInteger object.
    /// The result is rounded down (the fractional part is discarded).
    /// Except if the result is 0, it will be negative if this object is
    /// positive and the other is negative, or vice versa, and will be
    /// positive if both are positive or both are negative.</summary>
    /// <param name='bigintDivisor'>Another BigInteger object.</param>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='DivideByZeroException'>The divisor is
    /// zero.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintDivisor'/> is null.</exception>
    /// <exception cref='System.DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    public BigInteger divide(BigInteger bigintDivisor) {
      if ((bigintDivisor) == null) {
        throw new ArgumentNullException("bigintDivisor");
      }
      return new BigInteger(this.ei.divide(bigintDivisor.ei));
 }

    /// <summary>Divides this object by another big integer and returns the
    /// quotient and remainder.</summary>
    /// <param name='divisor'>A BigInteger object.</param>
    /// <returns>An array with two big integers: the first is the quotient,
    /// and the second is the remainder.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    /// <exception cref='DivideByZeroException'>The parameter <paramref
    /// name='divisor'/> is 0.</exception>
    /// <exception cref='System.DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    public BigInteger[] divideAndRemainder(BigInteger divisor) {
      if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
      EInteger[] eia = this.ei.divideAndRemainder(divisor.ei);
    return new BigInteger[] { new BigInteger(eia[0]), new BigInteger(eia[1])
        };
 }

    /// <inheritdoc/>
    /// <summary>Determines whether this object and another object are
    /// equal.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>True if this object and another object are equal;
    /// otherwise, false.</returns>
    public override bool Equals(object obj) {
      var bi = obj as BigInteger;
      return (bi == null) ? (false) : (this.ei.Equals(bi.ei));
}

    /// <summary>Returns the greatest common divisor of two integers. The
    /// greatest common divisor (GCD) is also known as the greatest common
    /// factor (GCF).</summary>
    /// <param name='bigintSecond'>Another BigInteger object.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintSecond'/> is null.</exception>
    public BigInteger gcd(BigInteger bigintSecond) {
  if ((bigintSecond) == null) {
  throw new ArgumentNullException("bigintSecond");
}
return new BigInteger(this.ei.gcd(bigintSecond.ei));
}

    /// <summary>Finds the number of decimal digits this number
    /// has.</summary>
    /// <returns>The number of decimal digits. Returns 1 if this object' s
    /// value is 0.</returns>
    public int getDigitCount() {
      return this.ei.getDigitCount();
 }

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public override int GetHashCode() {
      return this.ei.GetHashCode();
 }

    /// <summary>Gets the lowest set bit in this number's absolute
    /// value.</summary>
    /// <returns>The lowest bit set in the number, starting at 0. Returns 0
    /// if this value is 0 or odd. (NOTE: In future versions, may return -1
    /// instead if this value is 0.).</returns>
    public int getLowBit() {
      return this.ei.getLowBit();
 }

    /// <summary>See <c>getLowBit()</c></summary>
    /// <returns>See getLowBit().</returns>
    [Obsolete("Renamed to getLowBit.")]
    public int getLowestSetBit() {
      return getLowBit();
 }

    /// <summary>Finds the minimum number of bits needed to represent this
    /// object&#x27;s absolute value.</summary>
    /// <returns>The number of bits in this object's value. Returns 0 if
    /// this object's value is 0, and returns 1 if the value is negative
    /// 1.</returns>
    public int getUnsignedBitLength() {
      return getUnsignedBitLength();
 }

    /// <summary>Converts this object's value to a 32-bit signed
    /// integer.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    /// <exception cref='OverflowException'>This object's value is too big
    /// to fit a 32-bit signed integer.</exception>
    [Obsolete(
  "To make the conversion intention clearer use the 'intValueChecked' and 'intValueUnchecked' methods instead. Replace 'intValue' with 'intValueChecked' in your code." )]
    public int intValue() {
return this.ei.intValueChecked();
}

    /// <summary>Converts this object's value to a 32-bit signed
    /// integer.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    /// <exception cref='OverflowException'>This object's value is too big
    /// to fit a 32-bit signed integer.</exception>
    public int intValueChecked() {
return this.ei.intValueChecked();
}

    /// <summary>Converts this object's value to a 32-bit signed integer.
    /// If the value can't fit in a 32-bit integer, returns the lower 32
    /// bits of this object's two's complement representation (in which
    /// case the return value might have a different sign than this
    /// object's value).</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int intValueUnchecked() {
return this.ei.intValueUnchecked();
}

    /// <summary>Converts this object's value to a 64-bit signed
    /// integer.</summary>
    /// <returns>A 64-bit signed integer.</returns>
    /// <exception cref='OverflowException'>This object's value is too big
    /// to fit a 64-bit signed integer.</exception>
    [Obsolete(
  "To make the conversion intention clearer use the 'longValueChecked' and 'longValueUnchecked' methods instead. Replace 'longValue' with 'longValueChecked' in your code." )]
    public long longValue() {
return this.ei.longValueChecked();
}

    /// <summary>Converts this object's value to a 64-bit signed
    /// integer.</summary>
    /// <returns>A 64-bit signed integer.</returns>
    /// <exception cref='OverflowException'>This object's value is too big
    /// to fit a 64-bit signed integer.</exception>
    public long longValueChecked() {
return this.ei.longValueChecked();
}

    /// <summary>Converts this object's value to a 64-bit signed integer.
    /// If the value can't fit in a 64-bit integer, returns the lower 64
    /// bits of this object's two's complement representation (in which
    /// case the return value might have a different sign than this
    /// object's value).</summary>
    /// <returns>A 64-bit signed integer.</returns>
    public long longValueUnchecked() {
      return this.ei.longValueUnchecked();
 }

    /// <summary>Finds the modulus remainder that results when this
    /// instance is divided by the value of a BigInteger object. The
    /// modulus remainder is the same as the normal remainder if the normal
    /// remainder is positive, and equals divisor plus normal remainder if
    /// the normal remainder is negative.</summary>
    /// <param name='divisor'>A divisor greater than 0 (the
    /// modulus).</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArithmeticException'>The parameter <paramref
    /// name='divisor'/> is negative.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    public BigInteger mod(BigInteger divisor) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
return new BigInteger(this.ei.mod(divisor.ei));
}

    /// <summary>Calculates the remainder when a BigInteger raised to a
    /// certain power is divided by another BigInteger.</summary>
    /// <param name='pow'>Another BigInteger object.</param>
    /// <param name='mod'>A BigInteger object. (3).</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='pow'/> or <paramref name='mod'/> is null.</exception>
    public BigInteger ModPow(BigInteger pow, BigInteger mod) {
  if ((pow) == null) {
  throw new ArgumentNullException("pow");
}
  if ((mod) == null) {
  throw new ArgumentNullException("mod");
}
return new BigInteger(this.ei.ModPow(pow.ei, mod.ei));
}

    /// <summary>Multiplies this instance by the value of a BigInteger
    /// object.</summary>
    /// <param name='bigintMult'>Another BigInteger object.</param>
    /// <returns>The product of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintMult'/> is null.</exception>
    public BigInteger multiply(BigInteger bigintMult) {
      if ((bigintMult) == null) {
        throw new ArgumentNullException("bigintMult");
      }
      return new BigInteger(this.ei.multiply(bigintMult.ei));
    }

    /// <summary>Gets the value of this object with the sign
    /// reversed.</summary>
    /// <returns>This object's value with the sign reversed.</returns>
    public BigInteger negate() {
      return new BigInteger(this.ei.negate());
 }

    /// <summary>Raises a big integer to a power.</summary>
    /// <param name='powerSmall'>The exponent to raise to.</param>
    /// <returns>The result. Returns 1 if <paramref name='powerSmall'/> is
    /// 0.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='powerSmall'/> is less than 0.</exception>
    public BigInteger pow(int powerSmall) {
return new BigInteger(this.ei.pow(powerSmall));
}

    /// <summary>Raises a big integer to a power, which is given as another
    /// big integer.</summary>
    /// <param name='power'>The exponent to raise to.</param>
    /// <returns>The result. Returns 1 if <paramref name='power'/> is
    /// 0.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='power'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='power'/> is less than 0.</exception>
    public BigInteger PowBigIntVar(BigInteger power) {
  if ((power) == null) {
  throw new ArgumentNullException("power");
}
return new BigInteger(this.ei.PowBigIntVar(power.ei));
}

    /// <summary>Finds the remainder that results when this instance is
    /// divided by the value of a BigInteger object. The remainder is the
    /// value that remains when the absolute value of this object is
    /// divided by the absolute value of the other object; the remainder
    /// has the same sign (positive or negative) as this object.</summary>
    /// <param name='divisor'>Another BigInteger object.</param>
    /// <returns>The remainder of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    /// <exception cref='System.DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    public BigInteger remainder(BigInteger divisor) {
  if ((divisor) == null) {
  throw new ArgumentNullException("divisor");
}
return new BigInteger(this.ei.remainder(divisor.ei));
}

    /// <summary>Returns a big integer with the bits shifted to the left by
    /// a number of bits. A value of 1 doubles this value, a value of 2
    /// multiplies it by 4, a value of 3 by 8, a value of 4 by 16, and so
    /// on.</summary>
    /// <param name='numberBits'>The number of bits to shift. Can be
    /// negative, in which case this is the same as shiftRight with the
    /// absolute value of numberBits.</param>
    /// <returns>A BigInteger object.</returns>
    public BigInteger shiftLeft(int numberBits) {
      return new BigInteger(this.ei.shiftLeft(numberBits));
 }

    /// <summary>Returns a big integer with the bits shifted to the right.
    /// For this operation, the BigInteger is treated as a two's complement
    /// representation. Thus, for negative values, the BigInteger is
    /// sign-extended.</summary>
    /// <param name='numberBits'>Number of bits to shift right.</param>
    /// <returns>A BigInteger object.</returns>
    public BigInteger shiftRight(int numberBits) {
      return new BigInteger(this.ei.shiftRight(numberBits));
    }

    /// <summary>Finds the square root of this instance&#x27;s value,
    /// rounded down.</summary>
    /// <returns>The square root of this object's value. Returns 0 if this
    /// value is 0 or less.</returns>
    public BigInteger sqrt() {
      return new BigInteger(this.ei.sqrt());
    }

    /// <summary>Calculates the square root and the remainder.</summary>
    /// <returns>An array of two big integers: the first integer is the
    /// square root, and the second is the difference between this value
    /// and the square of the first integer. Returns two zeros if this
    /// value is 0 or less, or one and zero if this value equals
    /// 1.</returns>
    public BigInteger[] sqrtWithRemainder() {
      EInteger[] eia = this.ei.sqrtWithRemainder();
      return new BigInteger[] { new BigInteger(eia[0]), new BigInteger(eia[1])
        };
    }

    /// <summary>Subtracts a BigInteger from this BigInteger.</summary>
    /// <param name='subtrahend'>Another BigInteger object.</param>
    /// <returns>The difference of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='subtrahend'/> is null.</exception>
    public BigInteger subtract(BigInteger subtrahend) {
      if ((subtrahend) == null) {
  throw new ArgumentNullException("subtrahend");
}
      return new BigInteger(this.ei.subtract(subtrahend.ei));
 }

    /// <summary>Returns whether a bit is set in the two's-complement
    /// representation of this object's value.</summary>
    /// <param name='index'>Zero based index of the bit to test. 0 means
    /// the least significant bit.</param>
    /// <returns>True if a bit is set in the two's-complement
    /// representation of this object's value; otherwise, false.</returns>
    public bool testBit(int index) {
return this.ei.testBit(index);
}

    /// <summary>Returns a byte array of this object&#x27;s
    /// value.</summary>
    /// <param name='littleEndian'>A Boolean object.</param>
    /// <returns>A byte array.</returns>
    [Obsolete("Renamed to 'toBytes'.")]
    public byte[] toByteArray(bool littleEndian) {
      return toBytes(littleEndian);
 }

    /// <summary>Returns a byte array of this object&#x27;s value. The byte
    /// array will take the form of the number's two' s-complement
    /// representation, using the fewest bytes necessary to represent its
    /// value unambiguously. If this value is negative, the bits that
    /// appear "before" the most significant bit of the number will be all
    /// ones.</summary>
    /// <param name='littleEndian'>If true, the least significant bits will
    /// appear first.</param>
    /// <returns>A byte array. If this value is 0, returns a byte array
    /// with the single element 0.</returns>
    public byte[] toBytes(bool littleEndian) {
      return this.ei.toBytes(littleEndian);
 }

    /// <summary>Generates a string representing the value of this object,
    /// in the given radix.</summary>
    /// <param name='radix'>A radix from 2 through 36. For example, to
    /// generate a hexadecimal (base-16) string, specify 16. To generate a
    /// decimal (base-10) string, specify 10.</param>
    /// <returns>A string representing the value of this object. If this
    /// value is 0, returns "0". If negative, the string will begin with a
    /// hyphen/minus ("-"). Depending on the radix, the string will use the
    /// basic digits 0 to 9 (U + 0030 to U + 0039) and then the basic
    /// letters A to Z (U + 0041 to U + 005A). For example, 0-9 in radix
    /// 10, and 0-9, then A-F in radix 16.</returns>
    /// <exception cref='ArgumentException'>The parameter "index" is less
    /// than 0, "endIndex" is less than 0, or either is greater than the
    /// string's length, or "endIndex" is less than "index" ; or radix is
    /// less than 2 or greater than 36.</exception>
    public string toRadixString(int radix) {
      return this.ei.toRadixString(radix);
 }

    /// <summary>Converts this object to a text string in base
    /// 10.</summary>
    /// <returns>A string representation of this object. If negative, the
    /// string will begin with a minus sign ("-", U+002D). The string will
    /// use the basic digits 0 to 9 (U + 0030 to U + 0039).</returns>
    public override string ToString() {
      return this.ei.ToString();
    }
  }
}
