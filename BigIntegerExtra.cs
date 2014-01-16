/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 12/1/2013
 * Time: 11:34 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO
{
    /// <summary>An arbitrary-precision integer.</summary>
  public sealed partial class BigInteger
  {
    /// <summary>Converts the value of a 64-bit signed integer to BigInteger.</summary>
    /// <returns>A BigInteger object with the same value as the Int64 object.</returns>
    /// <param name='bigValue'>A 64-bit signed integer.</param>
    public static implicit operator BigInteger(long bigValue) {
      return valueOf(bigValue);
    }

    /// <summary>Adds a BigInteger object and a BigInteger object.</summary>
    /// <param name='bthis'>A BigInteger object.</param>
    /// <returns>The sum of the two objects.</returns>
    /// <param name='augend'>A BigInteger object. (2).</param>
    public static BigInteger operator +(BigInteger bthis, BigInteger augend)
    {
      if (bthis == null) {
 throw new ArgumentNullException("bthis");
}
      return bthis.add(augend);
    }

    /// <summary>Subtracts two BigInteger values.</summary>
    /// <param name='bthis'>A BigInteger value.</param>
    /// <returns>The difference of the two objects.</returns>
    /// <param name='subtrahend'>A BigInteger object.</param>
    public static BigInteger operator -(BigInteger bthis, BigInteger subtrahend)
    {
      if (bthis == null) {
 throw new ArgumentNullException("bthis");
}
      return bthis.subtract(subtrahend);
    }

    /// <summary>Multiplies a BigInteger object by the value of a BigInteger
    /// object.</summary>
    /// <returns>The product of the two objects.</returns>
    /// <param name='operand1'>A BigInteger object.</param>
    /// <param name='operand2'>A BigInteger object. (2).</param>
    public static BigInteger operator *(BigInteger operand1, BigInteger operand2) {
      if (operand1 == null) {
 throw new ArgumentNullException("operand1");
}
      return operand1.multiply(operand2);
    }

    /// <summary>Divides a BigInteger object by the value of a BigInteger
    /// object.</summary>
    /// <returns>The quotient of the two objects.</returns>
    /// <param name='dividend'>A BigInteger object.</param>
    /// <param name='divisor'>A BigInteger object. (2).</param>
    public static BigInteger operator /(BigInteger dividend, BigInteger divisor) {
      if (dividend == null) {
 throw new ArgumentNullException("dividend");
}
      return dividend.divide(divisor);
    }

    /// <summary>Finds the remainder that results when a BigInteger object
    /// is divided by the value of a BigInteger object.</summary>
    /// <returns>The remainder of the two objects.</returns>
    /// <param name='dividend'>A BigInteger object.</param>
    /// <param name='divisor'>A BigInteger object. (2).</param>
    public static BigInteger operator %(BigInteger dividend, BigInteger divisor) {
      if (dividend == null) {
 throw new ArgumentNullException("dividend");
}
      return dividend.remainder(divisor);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bthis'>A BigInteger object. (2).</param>
    /// <returns>A BigInteger object.</returns>
    /// <param name='numBits'>A 32-bit signed integer.</param>
    public static BigInteger operator <<(BigInteger bthis, int numBits)
    {
      if (bthis == null) {
 throw new ArgumentNullException("bthis");
}
      return bthis.shiftLeft(numBits);
    }

    /// <summary>Calculates the remainder when a BigInteger raised to a
    /// certain power is divided by another BigInteger.</summary>
    /// <param name='bigintValue'>A BigInteger object.</param>
    /// <param name='pow'>A BigInteger object. (2).</param>
    /// <param name='mod'>A BigInteger object. (3).</param>
    /// <returns>The value (bigintValue^pow)%mod.</returns>
    public static BigInteger ModPow(BigInteger bigintValue, BigInteger pow, BigInteger mod) {
      if (bigintValue == null) {
 throw new ArgumentNullException("bigintValue");
}
      return bigintValue.ModPow(pow, mod);
    }

    /// <summary>Shifts the bits of a BigInteger instance to the right.</summary>
    /// <param name='bthis'>A BigInteger object. (2).</param>
    /// <param name='n'>The number of bits to shift to the right. If negative,
    /// this treated as shifting left the absolute value of this number of
    /// bits.</param>
    /// <returns>A BigInteger object.</returns>
    /// <remarks>For this operation, the BigInteger is treated as a two's
    /// complement representation. Thus, for negative values, the BigInteger
    /// is sign-extended.</remarks>
    public static BigInteger operator >>(BigInteger bthis, int n)
    {
      if (bthis == null) {
 throw new ArgumentNullException("bthis");
}
      return bthis.shiftRight(n);
    }

    /// <summary>Negates a BigInteger object.</summary>
    /// <param name='bigValue'>A BigInteger object. (2).</param>
    /// <returns>A BigInteger object.</returns>
    public static BigInteger operator -(BigInteger bigValue) {
      if (bigValue == null) {
 throw new ArgumentNullException("bigValue");
}
      return bigValue.negate();
    }

    /// <summary>Converts the value of a BigInteger object to Int64.</summary>
    /// <returns>A Int64 object with the same value as the BigInteger object.</returns>
    /// <param name='bigValue'>A BigInteger object.</param>
    public static explicit operator long(BigInteger bigValue) {
      return bigValue.longValue();
    }

    /// <summary>Converts the value of a BigInteger object to a 32-bit signed
    /// integer.</summary>
    /// <returns>A 32-bit signed integer with the same value as the BigInteger
    /// object.</returns>
    /// <param name='bigValue'>A BigInteger object.</param>
    public static explicit operator int(BigInteger bigValue) {
      return bigValue.intValue();
    }

    /// <summary>Determines whether a BigInteger instance is less than
    /// another BigInteger instance.</summary>
    /// <param name='thisValue'>A BigInteger object.</param>
    /// <param name='otherValue'>A BigInteger object. (2).</param>
    /// <returns>True if &apos;thisValue&apos; is less than &apos; otherValue&apos;
    /// ; otherwise, false.</returns>
    public static bool operator <(BigInteger thisValue, BigInteger otherValue) {
      if (thisValue == null) {
 return otherValue != null;
}
      return thisValue.CompareTo(otherValue) < 0;
    }

    /// <summary>Determines whether a BigInteger instance is less than
    /// or equal to another BigInteger instance.</summary>
    /// <param name='thisValue'>A BigInteger object.</param>
    /// <param name='otherValue'>A BigInteger object. (2).</param>
    /// <returns>True if &apos;thisValue&apos; is less than or equal to
    /// &apos; otherValue&apos; ; otherwise, false.</returns>
    public static bool operator <= (BigInteger thisValue, BigInteger otherValue) {
      if (thisValue == null) {
 return true;
}
      return thisValue.CompareTo(otherValue) <= 0;
    }

    /// <summary>Determines whether a BigInteger instance is greater than
    /// another BigInteger instance.</summary>
    /// <param name='thisValue'>A BigInteger object.</param>
    /// <param name='otherValue'>A BigInteger object. (2).</param>
    /// <returns>True if &apos;thisValue&apos; is greater than &apos;
    /// otherValue&apos; ; otherwise, false.</returns>
    public static bool operator >(BigInteger thisValue, BigInteger otherValue) {
      if (thisValue == null) {
 return false;
}
      return thisValue.CompareTo(otherValue) > 0;
    }

    /// <summary>Determines whether a BigInteger value is greater than
    /// another BigInteger value.</summary>
    /// <param name='thisValue'>A BigInteger object.</param>
    /// <param name='otherValue'>A BigInteger object. (2).</param>
    /// <returns>True if &apos;thisValue&apos; is greater than or equal
    /// to &apos; otherValue&apos; ; otherwise, false.</returns>
    public static bool operator >= (BigInteger thisValue, BigInteger otherValue) {
      if (thisValue == null) {
 return otherValue == null;
}
      return thisValue.CompareTo(otherValue) >= 0;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
    /// <value>Not documented yet.</value>
    public bool IsPowerOfTwo{
      get {
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
    public static BigInteger Zero {
      get {
 return ZERO;
}
    }

    /// <summary>Gets the BigInteger object for one.</summary>
    /// <value>The BigInteger object for one.</value>
    [CLSCompliant(false)]
    public static BigInteger One {
      get {
 return ONE;
}
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <returns>A 64-bit signed integer.</returns>
    /// <param name='numberBits'>A 32-bit signed integer. (2).</param>
    public long GetBits(int index, int numberBits)
    {
      if (numberBits < 0 || numberBits > 64) {
 throw new ArgumentOutOfRangeException("n");
}
      long v = 0;
      // DebugAssert.IsTrue(n <= 8*8,"{0} line {1}: n <= sizeof(v)*8","integer.cpp",2939);
      for (int j = 0; j < numberBits; ++j)
        v |= (long)(this.testBit((int)(index + j)) ? 1 : 0) << j;
      return v;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='dividend'>A BigInteger object. (2).</param>
    /// <param name='divisor'>A BigInteger object. (3).</param>
    /// <param name='remainder'>A BigInteger object. (4).</param>
    /// <returns>A BigInteger object.</returns>
    public static BigInteger DivRem(BigInteger dividend, BigInteger divisor, out BigInteger remainder)
    {
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
    public static BigInteger GreatestCommonDivisor(BigInteger bigintFirst, BigInteger bigintSecond) {
      if (bigintFirst == null) {
 throw new ArgumentNullException("bigintFirst");
}
      return bigintFirst.gcd(bigintSecond);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A byte[] object.</returns>
    [CLSCompliant(false)]
    public byte[] ToByteArray() {
      return this.toByteArray(true);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bigValue'>A BigInteger object. (2).</param>
    /// <param name='power'>A BigInteger object. (3).</param>
    /// <returns>A BigInteger object.</returns>
    [CLSCompliant(false)]
    public static BigInteger Pow(BigInteger bigValue, BigInteger power) {
      if (bigValue == null) {
 throw new ArgumentNullException("bigValue");
}
      if (power == null) {
 throw new ArgumentNullException("power");
}
      if (power.Sign < 0) {
 throw new ArgumentException("power.Sign" + " not greater or equal to " + "0" + " (" + Convert.ToString(power.Sign,System.Globalization.CultureInfo.InvariantCulture) + ")");
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
    [CLSCompliant(false)]
    public static BigInteger Pow(BigInteger bigValue, int power) {
      if (bigValue == null) {
 throw new ArgumentNullException("bigValue");
}
      if (power < 0) {
 throw new ArgumentException("power" + " not greater or equal to " + "0" + " (" + Convert.ToString(power,System.Globalization.CultureInfo.InvariantCulture) + ")");
}
      return bigValue.pow(power);
    }

    private static void OrWords(short[] r, short[] a, short[] b, int n)
    {
      for (int i = 0; i < n; ++i) {
        r[i] = unchecked((short)(a[i] | b[i]));
      }
    }

    private static void XorWords(short[] r, short[] a, short[] b, int n)
    {
      for (int i = 0; i < n; ++i) {
        r[i] = unchecked((short)(a[i] ^ b[i]));
      }
    }

    private static void NotWords(short[] r, int n)
    {
      for (int i = 0; i < n; ++i) {
        r[i] =unchecked((short)(~r[i]));
      }
    }

    private static void AndWords(short[] r, short[] a, short[] b, int n)
    {
      for (int i = 0; i < n; ++i) {
        r[i] = unchecked((short)(a[i] & b[i]));
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bytes'>A byte[] object.</param>
    public BigInteger(byte[] bytes) {
      this.fromByteArrayInternal(bytes, true);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='other'>A BigInteger object.</param>
    /// <returns>A Boolean object.</returns>
    public bool Equals(BigInteger other) {
      if (other == null) {
 return false;
}
      return this.CompareTo(other) == 0;
    }

    /// <summary>Returns a BigInteger with every bit flipped.</summary>
    /// <param name='a'>A BigInteger object. (2).</param>
    /// <returns>A BigInteger object.</returns>
    public static BigInteger Not(BigInteger a) {
      if (a == null) {
 throw new ArgumentNullException("a");
}
      BigInteger xa = new BigInteger().Allocate(a.wordCount);
      Array.Copy(a.reg, xa.reg, xa.reg.Length);
      xa.negative = a.negative;
      xa.wordCount = a.wordCount;
      if (xa.Sign < 0) {
  { TwosComplement(xa.reg, 0, (int)xa.reg.Length);
} }
      xa.negative = !(xa.Sign < 0);
      NotWords(xa.reg, (int)xa.reg.Length);
      if (xa.Sign < 0) {
  { TwosComplement(xa.reg, 0, (int)xa.reg.Length);
} }
      xa.wordCount = xa.CalcWordCount();
      if (xa.wordCount == 0) {
 xa.negative = false;
}
      return xa;
    }

    /// <summary>Does an AND operation between two BigInteger values.</summary>
    /// <param name='a'>A BigInteger instance.</param>
    /// <param name='b'>Another BigInteger instance.</param>
    /// <remarks>Each BigInteger instance is treated as a two's complement
    /// representation for the purposes of this operator.</remarks>
    /// <returns>A BigInteger object.</returns>
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
      BigInteger xa = new BigInteger().Allocate(a.wordCount);
      Array.Copy(a.reg, xa.reg, xa.reg.Length);
      BigInteger xb = new BigInteger().Allocate(b.wordCount);
      Array.Copy(b.reg, xb.reg, xb.reg.Length);
      xa.negative = a.negative;
      xa.wordCount = a.wordCount;
      xb.negative = b.negative;
      xb.wordCount = b.wordCount;
      xa.reg = CleanGrow(xa.reg, Math.Max(xa.reg.Length, xb.reg.Length));
      xb.reg = CleanGrow(xb.reg, Math.Max(xa.reg.Length, xb.reg.Length));
      if (xa.Sign < 0) {
  { TwosComplement(xa.reg, 0, (int)xa.reg.Length);
} }
      if (xb.Sign < 0) {
  { TwosComplement(xb.reg, 0, (int)xb.reg.Length);
} }
      xa.negative &= xb.Sign < 0;
      AndWords(xa.reg, xa.reg, xb.reg, (int)xa.reg.Length);
      if (xa.Sign < 0) {
  { TwosComplement(xa.reg, 0, (int)xa.reg.Length);
} }
      xa.wordCount = xa.CalcWordCount();
      if (xa.wordCount == 0) {
 xa.negative = false;
}
      return xa;
    }

    /// <summary>Does an OR operation between two BigInteger instances.</summary>
    /// <remarks>Each BigInteger instance is treated as a two's complement
    /// representation for the purposes of this operator.</remarks>
    /// <returns>A BigInteger object.</returns>
    /// <param name='first'>A BigInteger object. (2).</param>
    /// <param name='second'>A BigInteger object. (3).</param>
    public static BigInteger Or(BigInteger first, BigInteger second) {
      if (first == null) {
 throw new ArgumentNullException("first");
}
      if (second == null) {
 throw new ArgumentNullException("second");
}
      BigInteger xa = new BigInteger().Allocate(first.wordCount);
      Array.Copy(first.reg, xa.reg, xa.reg.Length);
      BigInteger xb = new BigInteger().Allocate(second.wordCount);
      Array.Copy(second.reg, xb.reg, xb.reg.Length);
      xa.negative = first.negative;
      xa.wordCount = first.wordCount;
      xb.negative = second.negative;
      xb.wordCount = second.wordCount;
      xa.reg = CleanGrow(xa.reg, Math.Max(xa.reg.Length, xb.reg.Length));
      xb.reg = CleanGrow(xb.reg, Math.Max(xa.reg.Length, xb.reg.Length));
      if (xa.Sign < 0) {
  { TwosComplement(xa.reg, 0, (int)xa.reg.Length);
} }
      if (xb.Sign < 0) {
  { TwosComplement(xb.reg, 0, (int)xb.reg.Length);
} }
      xa.negative |= xb.Sign < 0;
      OrWords(xa.reg, xa.reg, xb.reg, (int)xa.reg.Length);
      if (xa.Sign < 0) {
  { TwosComplement(xa.reg, 0, (int)xa.reg.Length);
} }
      xa.wordCount = xa.CalcWordCount();
      if (xa.wordCount == 0) {
 xa.negative = false;
}
      return xa;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='a'>A BigInteger instance.</param>
    /// <param name='b'>Another BigInteger instance.</param>
    /// <remarks>Each BigInteger instance is treated as a two's complement
    /// representation for the purposes of this operator.</remarks>
    /// <returns>A BigInteger object.</returns>
    public static BigInteger Xor(BigInteger a, BigInteger b) {
      if (a == null) {
 throw new ArgumentNullException("a");
}
      if (b == null) {
 throw new ArgumentNullException("b");
}
      BigInteger xa = new BigInteger().Allocate(a.wordCount);
      Array.Copy(a.reg, xa.reg, xa.reg.Length);
      BigInteger xb = new BigInteger().Allocate(b.wordCount);
      Array.Copy(b.reg, xb.reg, xb.reg.Length);
      xa.negative = a.negative;
      xa.wordCount = a.wordCount;
      xb.negative = b.negative;
      xb.wordCount = b.wordCount;
      xa.reg = CleanGrow(xa.reg, Math.Max(xa.reg.Length, xb.reg.Length));
      xb.reg = CleanGrow(xb.reg, Math.Max(xa.reg.Length, xb.reg.Length));
      if (xa.Sign < 0) {
  { TwosComplement(xa.reg, 0, (int)xa.reg.Length);
} }
      if (xb.Sign < 0) {
  { TwosComplement(xb.reg, 0, (int)xb.reg.Length);
} }
      xa.negative ^= xb.Sign < 0;
      XorWords(xa.reg, xa.reg, xb.reg, (int)xa.reg.Length);
      if (xa.Sign < 0) {
  { TwosComplement(xa.reg, 0, (int)xa.reg.Length);
} }
      xa.wordCount = xa.CalcWordCount();
      if (xa.wordCount == 0) {
 xa.negative = false;
}
      return xa;
    }
  }
}
