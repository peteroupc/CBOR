/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;

namespace PeterO {
    /// <summary>An arbitrary-precision integer.</summary>
  public sealed partial class BigInteger
  {
    /// <summary>Converts the value of a 64-bit signed integer to BigInteger.</summary>
    /// <param name='bigValue'>A 64-bit signed integer.</param>
    /// <returns>A BigInteger object with the same value as the Int64 object.</returns>
    public static implicit operator BigInteger(long bigValue) {
      return valueOf(bigValue);
    }

    /// <summary>Adds a BigInteger object and a BigInteger object.</summary>
    /// <param name='bthis'>A BigInteger object.</param>
    /// <param name='augend'>A BigInteger object. (2).</param>
    /// <returns>The sum of the two objects.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='bthis'/> is null.</exception>
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
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='bthis'/> is null.</exception>
    public static BigInteger operator -(BigInteger bthis, BigInteger subtrahend) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.subtract(subtrahend);
    }

    /// <summary>Multiplies a BigInteger object by the value of a BigInteger
    /// object.</summary>
    /// <param name='operand1'>A BigInteger object.</param>
    /// <param name='operand2'>A BigInteger object. (2).</param>
    /// <returns>The product of the two objects.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='operand1'/> is null.</exception>
    public static BigInteger operator *(BigInteger operand1, BigInteger operand2) {
      if (operand1 == null) {
        throw new ArgumentNullException("operand1");
      }
      return operand1.multiply(operand2);
    }

    /// <summary>Divides a BigInteger object by the value of a BigInteger
    /// object.</summary>
    /// <param name='dividend'>A BigInteger object.</param>
    /// <param name='divisor'>A BigInteger object. (2).</param>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='dividend'/> is null.</exception>
    public static BigInteger operator /(BigInteger dividend, BigInteger divisor) {
      if (dividend == null) {
        throw new ArgumentNullException("dividend");
      }
      return dividend.divide(divisor);
    }

    /// <summary>Finds the remainder that results when a BigInteger object
    /// is divided by the value of a BigInteger object.</summary>
    /// <param name='dividend'>A BigInteger object.</param>
    /// <param name='divisor'>A BigInteger object. (2).</param>
    /// <returns>The remainder of the two objects.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='dividend'/> is null.</exception>
    public static BigInteger operator %(BigInteger dividend, BigInteger divisor) {
      if (dividend == null) {
        throw new ArgumentNullException("dividend");
      }
      return dividend.remainder(divisor);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bthis'>A BigInteger object. (2).</param>
    /// <param name='bitCount'>A 32-bit signed integer.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='bthis'/> is null.</exception>
    public static BigInteger operator <<(BigInteger bthis, int bitCount) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.shiftLeft(bitCount);
    }

    /// <summary>Calculates the remainder when a BigInteger raised to a
    /// certain power is divided by another BigInteger.</summary>
    /// <param name='bigintValue'>A BigInteger object.</param>
    /// <param name='pow'>A BigInteger object. (2).</param>
    /// <param name='mod'>A BigInteger object. (3).</param>
    /// <returns>The value ( <paramref name='bigintValue'/> ^ <paramref
    /// name='pow'/> )% <paramref name='mod'/> .</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='bigintValue'/> is null.</exception>
    public static BigInteger ModPow(BigInteger bigintValue, BigInteger pow, BigInteger mod) {
      if (bigintValue == null) {
        throw new ArgumentNullException("bigintValue");
      }
      return bigintValue.ModPow(pow, mod);
    }

    /// <summary>Shifts the bits of a BigInteger instance to the right.</summary>
    /// <param name='bthis'>A BigInteger object. (2).</param>
    /// <param name='bigValue'>A 32-bit signed integer.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='bthis'/> is null.</exception>
    /// <remarks>For this operation, the BigInteger is treated as a two's
    /// complement representation. Thus, for negative values, the BigInteger
    /// is sign-extended.</remarks>
    public static BigInteger operator >>(BigInteger bthis, int bigValue) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.shiftRight(bigValue);
    }

    /// <summary>Negates a BigInteger object.</summary>
    /// <param name='bigValue'>A BigInteger object. (2).</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='bigValue'/> is null.</exception>
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
    /// <exception cref='OverflowException'>This object's value is too
    /// big to fit a 64-bit signed integer.</exception>
    public static explicit operator long(BigInteger bigValue) {
      return bigValue.longValueChecked();
    }

    /// <summary>Converts the value of a BigInteger object to a 32-bit signed
    /// integer.</summary>
    /// <param name='bigValue'>A BigInteger object.</param>
    /// <returns>A 32-bit signed integer with the same value as the BigInteger
    /// object.</returns>
    /// <exception cref='OverflowException'>This object's value is too
    /// big to fit a 32-bit signed integer.</exception>
    public static explicit operator int(BigInteger bigValue) {
      return bigValue.intValueChecked();
    }

    /// <summary>Determines whether a BigInteger instance is less than
    /// another BigInteger instance.</summary>
    /// <param name='thisValue'>The first BigInteger object.</param>
    /// <param name='otherValue'>The second BigInteger object.</param>
    /// <returns>True if <paramref name='thisValue'/> is less than <paramref
    /// name='otherValue'/> ; otherwise, false.</returns>
    public static bool operator <(BigInteger thisValue, BigInteger otherValue) {
      return (thisValue == null) ? (otherValue != null) : (thisValue.CompareTo(otherValue) < 0);
    }

    /// <summary>Determines whether a BigInteger instance is less than
    /// or equal to another BigInteger instance.</summary>
    /// <param name='thisValue'>The first BigInteger object.</param>
    /// <param name='otherValue'>The second BigInteger object.</param>
    /// <returns>True if <paramref name='thisValue'/> is up to <paramref
    /// name='otherValue'/> ; otherwise, false.</returns>
    public static bool operator <=(BigInteger thisValue, BigInteger otherValue) {
      return (thisValue == null) || (thisValue.CompareTo(otherValue) <= 0);
    }

    /// <summary>Determines whether a BigInteger instance is greater than
    /// another BigInteger instance.</summary>
    /// <param name='thisValue'>The first BigInteger object.</param>
    /// <param name='otherValue'>The second BigInteger object.</param>
    /// <returns>True if <paramref name='thisValue'/> is greater than
    /// <paramref name='otherValue'/> ; otherwise, false.</returns>
    public static bool operator >(BigInteger thisValue, BigInteger otherValue) {
      return (thisValue != null) && (thisValue.CompareTo(otherValue) > 0);
    }

    /// <summary>Determines whether a BigInteger value is greater than
    /// another BigInteger value.</summary>
    /// <param name='thisValue'>The first BigInteger object.</param>
    /// <param name='otherValue'>The second BigInteger object.</param>
    /// <returns>True if <paramref name='thisValue'/> is at least <paramref
    /// name='otherValue'/> ; otherwise, false.</returns>
    public static bool operator >=(BigInteger thisValue, BigInteger otherValue) {
      return (thisValue == null) ? (otherValue == null) : (thisValue.CompareTo(otherValue) >= 0);
    }

    /// <summary>Gets a value indicating whether this object&apos;s value
    /// is a power of two.</summary>
    /// <value>True if this object&apos;s value is a power of two; otherwise,
    /// false.</value>
    public bool IsPowerOfTwo
    {
      get
      {
        int bits = this.bitLength();
        int ret = 0;
        for (int i = 0; i < bits; ++i) {
          ret += this.GetUnsignedBit(i) ? 1 : 0;
          if (ret >= 2) {
            return false;
          }
        }
        return ret == 1;
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A BigInteger object. (2).</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='thisValue'/> is null.</exception>
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
      // DebugAssert.IsTrue(n <= 8*8,"{0} line {1}: n <= sizeof(v)*8","integer.cpp",2939);
      for (int j = 0; j < numberBits; ++j) {
        v |= (long)(this.testBit((int)(index + j)) ? 1 : 0) << j;
      }
      return v;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='dividend'>A BigInteger object. (2).</param>
    /// <param name='divisor'>A BigInteger object. (3).</param>
    /// <param name='remainder'>A BigInteger object. (4).</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='dividend'/> is null.</exception>
    public static BigInteger DivRem(BigInteger dividend, BigInteger divisor, out BigInteger remainder) {
      if (dividend == null) {
        throw new ArgumentNullException("dividend");
      }
      BigInteger[] result = dividend.divideAndRemainder(divisor);
      remainder = result[1];
      return result[0];
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bigintFirst'>A BigInteger object. (2).</param>
    /// <param name='bigintSecond'>A BigInteger object. (3).</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='bigintFirst'/> is null.</exception>
    public static BigInteger GreatestCommonDivisor(BigInteger bigintFirst, BigInteger bigintSecond) {
      if (bigintFirst == null) {
        throw new ArgumentNullException("bigintFirst");
      }
      return bigintFirst.gcd(bigintSecond);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A byte array.</returns>
    [CLSCompliant(false)]
    public byte[] ToByteArray() {
      return this.toByteArray(true);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bigValue'>A BigInteger object. (2).</param>
    /// <param name='power'>A BigInteger object. (3).</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='bigValue'/> or <paramref name='power'/> is null.</exception>
    [CLSCompliant(false)]
    public static BigInteger Pow(BigInteger bigValue, BigInteger power) {
      if (bigValue == null) {
        throw new ArgumentNullException("bigValue");
      }
      if (power == null) {
        throw new ArgumentNullException("power");
      }
      if (power.Sign < 0) {
        throw new ArgumentException("power's sign (" + Convert.ToString((long)power.Sign, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
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
    /// <param name='bigValue'>A BigInteger object. (2).</param>
    /// <param name='power'>A 32-bit signed integer.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='bigValue'/> is null.</exception>
    [CLSCompliant(false)]
    public static BigInteger Pow(BigInteger bigValue, int power) {
      if (bigValue == null) {
        throw new ArgumentNullException("bigValue");
      }
      if (power < 0) {
        throw new ArgumentException("power (" + Convert.ToString((long)power, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      return bigValue.pow(power);
    }

    private static void OrWords(short[] r, short[] a, short[] b, int n) {
      for (int i = 0; i < n; ++i) {
        r[i] = unchecked((short)(a[i] | b[i]));
      }
    }

    private static void XorWords(short[] r, short[] a, short[] b, int n) {
      for (int i = 0; i < n; ++i) {
        r[i] = unchecked((short)(a[i] ^ b[i]));
      }
    }

    private static void NotWords(short[] r, int n) {
      for (int i = 0; i < n; ++i) {
        r[i] = unchecked((short)(~r[i]));
      }
    }

    private static void AndWords(short[] r, short[] a, short[] b, int n) {
      for (int i = 0; i < n; ++i) {
        r[i] = unchecked((short)(a[i] & b[i]));
      }
    }

    /// <summary>Initializes a new instance of the BigInteger class.</summary>
    /// <param name='bytes'>A byte array.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='bytes'/> is null.</exception>
    public BigInteger(byte[] bytes) {
      if (bytes == null) {
        throw new ArgumentNullException("bytes");
      }
      if (bytes.Length == 0) {
        this.InitializeInt(0);
      } else {
        this.fromByteArrayInternal(bytes, true);
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='other'>A BigInteger object.</param>
    /// <returns>A Boolean object.</returns>
    public bool Equals(BigInteger other) {
      return (other != null) && (this.CompareTo(other) == 0);
    }

    /// <summary>Returns a BigInteger with every bit flipped.</summary>
    /// <param name='valueA'>A BigInteger object. (2).</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='valueA'/> is null.</exception>
    public static BigInteger Not(BigInteger valueA) {
      if (valueA == null) {
        throw new ArgumentNullException("valueA");
      }
      var xa = new BigInteger().Allocate(valueA.wordCount);
      Array.Copy(valueA.reg, xa.reg, xa.reg.Length);
      xa.negative = valueA.negative;
      xa.wordCount = valueA.wordCount;
      if (xa.Sign < 0) {
        {
          TwosComplement(xa.reg, 0, (int)xa.reg.Length);
        }
      }
      xa.negative = xa.Sign >= 0;
      NotWords(xa.reg, (int)xa.reg.Length);
      if (xa.Sign < 0) {
        {
          TwosComplement(xa.reg, 0, (int)xa.reg.Length);
        }
      }
      xa.wordCount = xa.CalcWordCount();
      if (xa.wordCount == 0) {
        xa.negative = false;
      }
      return xa;
    }

    /// <summary>Does an AND operation between two BigInteger values.</summary>
    /// <param name='a'>A BigInteger instance.</param>
    /// <param name='b'>Another BigInteger instance.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='a'/> or <paramref name='b'/> is null.</exception>
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
      var xa = new BigInteger().Allocate(a.wordCount);
      Array.Copy(a.reg, xa.reg, xa.reg.Length);
      var xb = new BigInteger().Allocate(b.wordCount);
      Array.Copy(b.reg, xb.reg, xb.reg.Length);
      xa.negative = a.negative;
      xa.wordCount = a.wordCount;
      xb.negative = b.negative;
      xb.wordCount = b.wordCount;
      xa.reg = CleanGrow(xa.reg, Math.Max(xa.reg.Length, xb.reg.Length));
      xb.reg = CleanGrow(xb.reg, Math.Max(xa.reg.Length, xb.reg.Length));
      if (xa.Sign < 0) {
        {
          TwosComplement(xa.reg, 0, (int)xa.reg.Length);
        }
      }
      if (xb.Sign < 0) {
        {
          TwosComplement(xb.reg, 0, (int)xb.reg.Length);
        }
      }
      xa.negative &= xb.Sign < 0;
      AndWords(xa.reg, xa.reg, xb.reg, (int)xa.reg.Length);
      if (xa.Sign < 0) {
        {
          TwosComplement(xa.reg, 0, (int)xa.reg.Length);
        }
      }
      xa.wordCount = xa.CalcWordCount();
      if (xa.wordCount == 0) {
        xa.negative = false;
      }
      return xa;
    }

    /// <summary>Does an OR operation between two BigInteger instances.</summary>
    /// <param name='first'>A BigInteger object. (2).</param>
    /// <param name='second'>A BigInteger object. (3).</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='first'/> or <paramref name='second'/> is null.</exception>
    /// <remarks>Each BigInteger instance is treated as a two's complement
    /// representation for the purposes of this operator.</remarks>
    public static BigInteger Or(BigInteger first, BigInteger second) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }
      var xa = new BigInteger().Allocate(first.wordCount);
      Array.Copy(first.reg, xa.reg, xa.reg.Length);
      var xb = new BigInteger().Allocate(second.wordCount);
      Array.Copy(second.reg, xb.reg, xb.reg.Length);
      xa.negative = first.negative;
      xa.wordCount = first.wordCount;
      xb.negative = second.negative;
      xb.wordCount = second.wordCount;
      xa.reg = CleanGrow(xa.reg, Math.Max(xa.reg.Length, xb.reg.Length));
      xb.reg = CleanGrow(xb.reg, Math.Max(xa.reg.Length, xb.reg.Length));
      if (xa.Sign < 0) {
        TwosComplement(xa.reg, 0, (int)xa.reg.Length);
      }
      if (xb.Sign < 0) {
        TwosComplement(xb.reg, 0, (int)xb.reg.Length);
      }
      xa.negative |= xb.Sign < 0;
      OrWords(xa.reg, xa.reg, xb.reg, (int)xa.reg.Length);
      if (xa.Sign < 0) {
        TwosComplement(xa.reg, 0, (int)xa.reg.Length);
      }
      xa.wordCount = xa.CalcWordCount();
      if (xa.wordCount == 0) {
        xa.negative = false;
      }
      return xa;
    }

    /// <summary>Finds the exclusive "or" of two BigInteger objects.</summary>
    /// <param name='a'>A BigInteger instance.</param>
    /// <param name='b'>Another BigInteger instance.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='a'/> or <paramref name='b'/> is null.</exception>
    /// <remarks>Each BigInteger instance is treated as a two's complement
    /// representation for the purposes of this operator.</remarks>
    public static BigInteger Xor(BigInteger a, BigInteger b) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      var xa = new BigInteger().Allocate(a.wordCount);
      Array.Copy(a.reg, xa.reg, xa.reg.Length);
      var xb = new BigInteger().Allocate(b.wordCount);
      Array.Copy(b.reg, xb.reg, xb.reg.Length);
      xa.negative = a.negative;
      xa.wordCount = a.wordCount;
      xb.negative = b.negative;
      xb.wordCount = b.wordCount;
      xa.reg = CleanGrow(xa.reg, Math.Max(xa.reg.Length, xb.reg.Length));
      xb.reg = CleanGrow(xb.reg, Math.Max(xa.reg.Length, xb.reg.Length));
      if (xa.Sign < 0) {
        {
          TwosComplement(xa.reg, 0, (int)xa.reg.Length);
        }
      }
      if (xb.Sign < 0) {
        {
          TwosComplement(xb.reg, 0, (int)xb.reg.Length);
        }
      }
      xa.negative ^= xb.Sign < 0;
      XorWords(xa.reg, xa.reg, xb.reg, (int)xa.reg.Length);
      if (xa.Sign < 0) {
        TwosComplement(xa.reg, 0, (int)xa.reg.Length);
      }
      xa.wordCount = xa.CalcWordCount();
      if (xa.wordCount == 0) {
        xa.negative = false;
      }
      return xa;
    }
  }
}
