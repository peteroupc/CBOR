/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO {
    /// <summary>An arbitrary-precision integer.</summary>
  public sealed partial class BigInteger {
    /// <summary>Converts the value of a 64-bit signed integer to
    /// BigInteger.</summary>
    /// <param name='bigValue'>A 64-bit signed integer.</param>
    /// <returns>A BigInteger object with the same value as the Int64
    /// object.</returns>
    public static implicit operator BigInteger(long bigValue) {
      return valueOf(bigValue);
    }

    /// <summary>Adds a BigInteger object and a BigInteger object.</summary>
    /// <param name='bthis'>A BigInteger object.</param>
    /// <param name='augend'>Another BigInteger object.</param>
    /// <returns>The sum of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bthis'/> is null.</exception>
    public static BigInteger operator +(BigInteger bthis, BigInteger augend) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.add(augend);
    }

    /// <summary>Subtracts two BigInteger values.</summary>
    /// <param name='bthis'>A BigInteger value.</param>
    /// <param name='subtrahend'>A BigInteger object.</param>
    /// <returns>The difference of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bthis'/> is null.</exception>
 public static BigInteger operator -(
BigInteger bthis,
BigInteger subtrahend) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.subtract(subtrahend);
    }

    /// <summary>Multiplies a BigInteger object by the value of a BigInteger
    /// object.</summary>
    /// <param name='operand1'>A BigInteger object.</param>
    /// <param name='operand2'>Another BigInteger object.</param>
    /// <returns>The product of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='operand1'/> is null.</exception>
public static BigInteger operator *(
BigInteger operand1,
BigInteger operand2) {
      if (operand1 == null) {
        throw new ArgumentNullException("operand1");
      }
      return operand1.multiply(operand2);
    }

    /// <summary>Divides a BigInteger object by the value of a BigInteger
    /// object.</summary>
    /// <param name='dividend'>A BigInteger object.</param>
    /// <param name='divisor'>Another BigInteger object.</param>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='dividend'/> is null.</exception>
 public static BigInteger operator /(
BigInteger dividend,
BigInteger divisor) {
      if (dividend == null) {
        throw new ArgumentNullException("dividend");
      }
      return dividend.divide(divisor);
    }

    /// <summary>Finds the remainder that results when a BigInteger object is
    /// divided by the value of a BigInteger object.</summary>
    /// <param name='dividend'>A BigInteger object.</param>
    /// <param name='divisor'>Another BigInteger object.</param>
    /// <returns>The remainder of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='dividend'/> is null.</exception>
 public static BigInteger operator %(
BigInteger dividend,
BigInteger divisor) {
      if (dividend == null) {
        throw new ArgumentNullException("dividend");
      }
      return dividend.remainder(divisor);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bthis'>Another BigInteger object.</param>
    /// <param name='bitCount'>A 32-bit signed integer.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bthis'/> is null.</exception>
    public static BigInteger operator <<(BigInteger bthis, int bitCount) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.shiftLeft(bitCount);
    }

    /// <summary>Calculates the remainder when a BigInteger raised to a certain
    /// power is divided by another BigInteger.</summary>
    /// <param name='bigintValue'>A BigInteger object.</param>
    /// <param name='pow'>Another BigInteger object.</param>
    /// <param name='mod'>A BigInteger object. (3).</param>
    /// <returns>The value ( <paramref name='bigintValue'/> ^ <paramref name='pow'/>
    /// )% <paramref name='mod'/> .</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintValue'/> is null.</exception>
    public static BigInteger ModPow(
BigInteger bigintValue,
BigInteger pow,
BigInteger mod) {
      if (bigintValue == null) {
        throw new ArgumentNullException("bigintValue");
      }
      return bigintValue.ModPow(pow, mod);
    }

    /// <summary>Shifts the bits of a BigInteger instance to the right.</summary>
    /// <param name='bthis'>Another BigInteger object.</param>
    /// <param name='bigValue'>A 32-bit signed integer.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bthis'/> is null.</exception>
    /// <remarks>For this operation, the BigInteger is treated as a two's complement
    /// representation. Thus, for negative values, the BigInteger is
    /// sign-extended.</remarks>
    public static BigInteger operator >>(BigInteger bthis, int bigValue) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.shiftRight(bigValue);
    }

    /// <summary>Negates a BigInteger object.</summary>
    /// <param name='bigValue'>Another BigInteger object.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigValue'/> is null.</exception>
    public static BigInteger operator -(BigInteger bigValue) {
      if (bigValue == null) {
        throw new ArgumentNullException("bigValue");
      }
      return bigValue.negate();
    }

    /// <summary>Converts the value of a BigInteger object to a 64-bit signed
    /// integer.</summary>
    /// <param name='bigValue'>A BigInteger object.</param>
    /// <returns>A 64-bit signed integer with the same value as the BigInteger
    /// object.</returns>
    /// <exception cref='OverflowException'>This object's value is too big to fit a
    /// 64-bit signed integer.</exception>
    public static explicit operator long(BigInteger bigValue) {
      return bigValue.longValueChecked();
    }

    /// <summary>Converts the value of a BigInteger object to a 32-bit signed
    /// integer.</summary>
    /// <param name='bigValue'>A BigInteger object.</param>
    /// <returns>A 32-bit signed integer with the same value as the BigInteger
    /// object.</returns>
    /// <exception cref='OverflowException'>This object's value is too big to fit a
    /// 32-bit signed integer.</exception>
    public static explicit operator int(BigInteger bigValue) {
      return bigValue.intValueChecked();
    }

    /// <summary>Determines whether a BigInteger instance is less than another
    /// BigInteger instance.</summary>
    /// <param name='thisValue'>The first BigInteger object.</param>
    /// <param name='otherValue'>The second BigInteger object.</param>
    /// <returns>True if <paramref name='thisValue'/> is less than <paramref
    /// name='otherValue'/> ; otherwise, false.</returns>
    public static bool operator <(BigInteger thisValue, BigInteger otherValue) {
      return (thisValue == null) ? (otherValue != null) :
        (thisValue.CompareTo(otherValue) < 0);
    }

    /// <summary>Determines whether a BigInteger instance is less than or equal to
    /// another BigInteger instance.</summary>
    /// <param name='thisValue'>The first BigInteger object.</param>
    /// <param name='otherValue'>The second BigInteger object.</param>
    /// <returns>True if <paramref name='thisValue'/> is up to <paramref
    /// name='otherValue'/> ; otherwise, false.</returns>
  public static bool operator <=(
BigInteger thisValue,
BigInteger otherValue) {
      return (thisValue == null) || (thisValue.CompareTo(otherValue) <= 0);
    }

    /// <summary>Determines whether a BigInteger instance is greater than another
    /// BigInteger instance.</summary>
    /// <param name='thisValue'>The first BigInteger object.</param>
    /// <param name='otherValue'>The second BigInteger object.</param>
    /// <returns>True if <paramref name='thisValue'/> is greater than <paramref
    /// name='otherValue'/> ; otherwise, false.</returns>
    public static bool operator >(BigInteger thisValue, BigInteger otherValue) {
      return (thisValue != null) && (thisValue.CompareTo(otherValue) > 0);
    }

    /// <summary>Determines whether a BigInteger value is greater than another
    /// BigInteger value.</summary>
    /// <param name='thisValue'>The first BigInteger object.</param>
    /// <param name='otherValue'>The second BigInteger object.</param>
    /// <returns>True if <paramref name='thisValue'/> is at least <paramref
    /// name='otherValue'/> ; otherwise, false.</returns>
  public static bool operator >=(
BigInteger thisValue,
BigInteger otherValue) {
      return (thisValue == null) ? (otherValue == null) :
        (thisValue.CompareTo(otherValue) >= 0);
    }

    /// <summary>Gets a value indicating whether this object&#x27;s value is a power
    /// of two.</summary>
    /// <value>True if this object&apos;s value is a power of two; otherwise,
    /// false.</value>
    public bool IsPowerOfTwo
    {
      get
      {
        int bits = this.bitLength();
        int ret = 0;
        for (var i = 0; i < bits; ++i) {
          ret += this.GetUnsignedBit(i) ? 1 : 0;
          if (ret >= 2) {
            return false;
          }
        }
        return ret == 1;
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>Another BigInteger object.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='thisValue'/> is null.</exception>
    [CLSCompliant(false)]
    public static BigInteger Abs(BigInteger thisValue) {
      if (thisValue == null) {
        throw new ArgumentNullException("thisValue");
      }
      return thisValue.abs();
    }

    /// <summary>Gets the BigInteger object for zero.</summary>
    /// <value>The BigInteger object for zero.</value>
    [CLSCompliant(false)]
    public static BigInteger Zero
    {
      get
      {
        return ZERO;
      }
    }

    /// <summary>Gets the BigInteger object for one.</summary>
    /// <value>The BigInteger object for one.</value>
    [CLSCompliant(false)]
    public static BigInteger One
    {
      get
      {
        return ONE;
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <param name='numberBits'>A 32-bit signed integer. (2).</param>
    /// <returns>A 64-bit signed integer.</returns>
    public long GetBits(int index, int numberBits) {
      if (numberBits < 0 || numberBits > 64) {
        throw new ArgumentOutOfRangeException("numberBits");
      }
      long v = 0;
      // DebugAssert.IsTrue(n <= 8*8,"{0} line {1}: n <= sizeof(v)*8"
      // ,"integer.cpp" ,2939);
      for (int j = 0; j < numberBits; ++j) {
        v |= (long)(this.testBit((int)(index + j)) ? 1 : 0) << j;
      }
      return v;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='dividend'>Another BigInteger object.</param>
    /// <param name='divisor'>A BigInteger object. (3).</param>
    /// <param name='remainder'>A BigInteger object. (4).</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='dividend'/> is null.</exception>
    public static BigInteger DivRem(
BigInteger dividend,
BigInteger divisor,
out BigInteger remainder) {
      if (dividend == null) {
        throw new ArgumentNullException("dividend");
      }
      BigInteger[] result = dividend.divideAndRemainder(divisor);
      remainder = result[1];
      return result[0];
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bigintFirst'>Another BigInteger object.</param>
    /// <param name='bigintSecond'>A BigInteger object. (3).</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintFirst'/> is null.</exception>
    public static BigInteger GreatestCommonDivisor(
BigInteger bigintFirst,
BigInteger bigintSecond) {
      if (bigintFirst == null) {
        throw new ArgumentNullException("bigintFirst");
      }
      return bigintFirst.gcd(bigintSecond);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A byte array.</returns>
    [Obsolete("Use 'toBytes(true)' instead.")]
    [CLSCompliant(false)]
    public byte[] ToByteArray() {
      return this.toBytes(true);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bigValue'>Another BigInteger object.</param>
    /// <param name='power'>A BigInteger object. (3).</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigValue'/> or <paramref name='power'/> is null.</exception>
    [CLSCompliant(false)]
    public static BigInteger Pow(BigInteger bigValue, BigInteger power) {
      if (bigValue == null) {
        throw new ArgumentNullException("bigValue");
      }
      if (power == null) {
        throw new ArgumentNullException("power");
      }
      if (power.Sign < 0) {
        throw new ArgumentException("power's sign (" + power.Sign +
          ") is less than 0");
      }
      BigInteger val = BigInteger.One;
      while (power.Sign > 0) {
        BigInteger p = (power > (BigInteger)5000000) ?
          (BigInteger)5000000 : power;
        val *= bigValue.pow((int)p);
        power -= p;
      }
      return val;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bigValue'>Another BigInteger object.</param>
    /// <param name='power'>A 32-bit signed integer.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigValue'/> is null.</exception>
    [CLSCompliant(false)]
    public static BigInteger Pow(BigInteger bigValue, int power) {
      if (bigValue == null) {
        throw new ArgumentNullException("bigValue");
      }
      if (power < 0) {
      throw new ArgumentException("power (" + power + ") is less than " +
          "0");
      }
      return bigValue.pow(power);
    }

    private static void OrWords(short[] r, short[] a, short[] b, int n) {
      for (var i = 0; i < n; ++i) {
        r[i] = unchecked((short)(a[i] | b[i]));
      }
    }

    private static void XorWords(short[] r, short[] a, short[] b, int n) {
      for (var i = 0; i < n; ++i) {
        r[i] = unchecked((short)(a[i] ^ b[i]));
      }
    }

    private static void NotWords(short[] r, int n) {
      for (var i = 0; i < n; ++i) {
        r[i] = unchecked((short)(~r[i]));
      }
    }

    private static void AndWords(short[] r, short[] a, short[] b, int n) {
      for (var i = 0; i < n; ++i) {
        r[i] = unchecked((short)(a[i] & b[i]));
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='other'>A BigInteger object.</param>
    /// <returns>A Boolean object.</returns>
    public bool Equals(BigInteger other) {
      return (other != null) && (this.CompareTo(other) == 0);
    }

    /// <summary>Returns a BigInteger with every bit flipped.</summary>
    /// <param name='valueA'>Another BigInteger object.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='valueA'/> is null.</exception>
    public static BigInteger Not(BigInteger valueA) {
      if (valueA == null) {
        throw new ArgumentNullException("valueA");
      }
      if (valueA.wordCount == 0) {
        return BigInteger.One.negate();
      }
      bool valueXaNegative = false; int valueXaWordCount = 0;
      var valueXaReg = new short[valueA.wordCount];
      Array.Copy(valueA.words, valueXaReg, valueXaReg.Length);
      valueXaWordCount = valueA.wordCount;
      if (valueA.negative) {
        TwosComplement(valueXaReg, 0, (int)valueXaReg.Length);
      }
      NotWords(valueXaReg, (int)valueXaReg.Length);
      if (valueA.negative) {
        TwosComplement(valueXaReg, 0, (int)valueXaReg.Length);
      }
      valueXaNegative = !valueA.negative;
      valueXaWordCount = CountWords(valueXaReg, valueXaReg.Length);
      return (valueXaWordCount == 0) ? BigInteger.Zero : (new
        BigInteger(valueXaWordCount, valueXaReg, valueXaNegative));
    }

    /// <summary>Does an AND operation between two BigInteger values.</summary>
    /// <param name='a'>A BigInteger instance.</param>
    /// <param name='b'>Another BigInteger instance.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref name='a'/>
    /// or <paramref name='b'/> is null.</exception>
    /// <remarks>Each BigInteger instance is treated as a two's complement
    /// representation for the purposes of this operator.</remarks>
    public static BigInteger And(BigInteger a, BigInteger b) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      if (b.IsZero || a.IsZero) {
        return Zero;
      }
      bool valueXaNegative = false; int valueXaWordCount = 0;
      var valueXaReg = new short[a.wordCount];
      Array.Copy(a.words, valueXaReg, valueXaReg.Length);
      bool valueXbNegative = false; int valueXbWordCount = 0;
      var valueXbReg = new short[b.wordCount];
      Array.Copy(b.words, valueXbReg, valueXbReg.Length);
      valueXaNegative = a.negative;
      valueXaWordCount = a.wordCount;
      valueXbNegative = b.negative;
      valueXbWordCount = b.wordCount;
      valueXaReg = CleanGrow(
valueXaReg,
Math.Max(valueXaReg.Length, valueXbReg.Length));
      valueXbReg = CleanGrow(
valueXbReg,
Math.Max(valueXaReg.Length, valueXbReg.Length));
      if (valueXaNegative) {
        {
          TwosComplement(valueXaReg, 0, (int)valueXaReg.Length);
        }
      }
      if (valueXbNegative) {
        {
          TwosComplement(valueXbReg, 0, (int)valueXbReg.Length);
        }
      }
      valueXaNegative &= valueXbNegative;
      AndWords(valueXaReg, valueXaReg, valueXbReg, (int)valueXaReg.Length);
      if (valueXaNegative) {
        {
          TwosComplement(valueXaReg, 0, (int)valueXaReg.Length);
        }
      }
      valueXaWordCount = CountWords(valueXaReg, valueXaReg.Length);
      return (valueXaWordCount == 0) ? BigInteger.Zero : (new
        BigInteger(valueXaWordCount, valueXaReg, valueXaNegative));
    }

    /// <summary>Does an OR operation between two BigInteger instances.</summary>
    /// <param name='first'>Another BigInteger object.</param>
    /// <param name='second'>A BigInteger object. (3).</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> is null.</exception>
    /// <remarks>Each BigInteger instance is treated as a two's complement
    /// representation for the purposes of this operator.</remarks>
    public static BigInteger Or(BigInteger first, BigInteger second) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }
      if (first.wordCount == 0) {
        return second;
      }
      if (second.wordCount == 0) {
        return first;
      }
      bool valueXaNegative = false; int valueXaWordCount = 0;
      var valueXaReg = new short[first.wordCount];
      Array.Copy(first.words, valueXaReg, valueXaReg.Length);
      bool valueXbNegative = false; int valueXbWordCount = 0;
      var valueXbReg = new short[second.wordCount];
      Array.Copy(second.words, valueXbReg, valueXbReg.Length);
      valueXaNegative = first.negative;
      valueXaWordCount = first.wordCount;
      valueXbNegative = second.negative;
      valueXbWordCount = second.wordCount;
      valueXaReg = CleanGrow(
valueXaReg,
Math.Max(valueXaReg.Length, valueXbReg.Length));
      valueXbReg = CleanGrow(
valueXbReg,
Math.Max(valueXaReg.Length, valueXbReg.Length));
      if (valueXaNegative) {
        TwosComplement(valueXaReg, 0, (int)valueXaReg.Length);
      }
      if (valueXbNegative) {
        TwosComplement(valueXbReg, 0, (int)valueXbReg.Length);
      }
      valueXaNegative |= valueXbNegative;
      OrWords(valueXaReg, valueXaReg, valueXbReg, (int)valueXaReg.Length);
      if (valueXaNegative) {
        TwosComplement(valueXaReg, 0, (int)valueXaReg.Length);
      }
      valueXaWordCount = CountWords(valueXaReg, valueXaReg.Length);
      return (valueXaWordCount == 0) ? BigInteger.Zero : (new
        BigInteger(valueXaWordCount, valueXaReg, valueXaNegative));
    }

    /// <summary>Finds the exclusive "or" of two BigInteger objects.</summary>
    /// <param name='a'>A BigInteger instance.</param>
    /// <param name='b'>Another BigInteger instance.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref name='a'/>
    /// or <paramref name='b'/> is null.</exception>
    /// <remarks>Each BigInteger instance is treated as a two's complement
    /// representation for the purposes of this operator.</remarks>
    public static BigInteger Xor(BigInteger a, BigInteger b) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      if (a == b) {
        return BigInteger.Zero;
      }
      if (a.wordCount == 0) {
        return b;
      }
      if (b.wordCount == 0) {
        return a;
      }
      bool valueXaNegative = false; int valueXaWordCount = 0;
      var valueXaReg = new short[a.wordCount];
      Array.Copy(a.words, valueXaReg, valueXaReg.Length);
      bool valueXbNegative = false; int valueXbWordCount = 0;
      var valueXbReg = new short[b.wordCount];
      Array.Copy(b.words, valueXbReg, valueXbReg.Length);
      valueXaNegative = a.negative;
      valueXaWordCount = a.wordCount;
      valueXbNegative = b.negative;
      valueXbWordCount = b.wordCount;
      valueXaReg = CleanGrow(
valueXaReg,
Math.Max(valueXaReg.Length, valueXbReg.Length));
      valueXbReg = CleanGrow(
valueXbReg,
Math.Max(valueXaReg.Length, valueXbReg.Length));
      if (valueXaNegative) {
        TwosComplement(valueXaReg, 0, (int)valueXaReg.Length);
      }
      if (valueXbNegative) {
        TwosComplement(valueXbReg, 0, (int)valueXbReg.Length);
      }
      valueXaNegative ^= valueXbNegative;
      XorWords(valueXaReg, valueXaReg, valueXbReg, (int)valueXaReg.Length);
      if (valueXaNegative) {
        TwosComplement(valueXaReg, 0, (int)valueXaReg.Length);
      }
      valueXaWordCount = CountWords(valueXaReg, valueXaReg.Length);
      return (valueXaWordCount == 0) ? BigInteger.Zero : (new
        BigInteger(valueXaWordCount, valueXaReg, valueXaNegative));
    }
  }
}
