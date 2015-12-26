/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Numbers {
    /// <summary>An arbitrary-precision integer.</summary>
  internal sealed partial class EInteger {
    /// <summary>Converts the value of a 64-bit signed integer to
    /// BigInteger.</summary>
    /// <param name='bigValue'>Not documented yet.</param>
    /// <returns>A BigInteger object with the same value as the Int64
    /// object.</returns>
    public static implicit operator EInteger(long bigValue) {
      return FromInt64(bigValue);
    }

    /// <summary>Converts the value of a 32-bit signed integer to
    /// BigInteger.</summary>
    /// <param name='smallValue'>Not documented yet.</param>
    /// <returns>A BigInteger object with the same value as the Int32
    /// object.</returns>
    public static implicit operator EInteger(int smallValue) {
      return FromInt64((long)smallValue);
    }

    /// <summary>Adds a BigInteger object and a BigInteger
    /// object.</summary>
    /// <returns>The sum of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bthis'/> is null.</exception>
    public static EInteger operator +(EInteger bthis, EInteger augend) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.add(augend);
    }

    /// <summary>Subtracts two BigInteger values.</summary>
    /// <param name='bthis'>A BigInteger value.</param>
    /// <param name='subtrahend'>An EInteger object.</param>
    /// <returns>The difference of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bthis'/> is null.</exception>
 public static EInteger operator -(
EInteger bthis,
EInteger subtrahend) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.Subtract(subtrahend);
    }

    /// <summary>Multiplies a BigInteger object by the value of a
    /// BigInteger object.</summary>
    /// <returns>The product of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='operand1'/> is null.</exception>
public static EInteger operator *(
EInteger operand1,
EInteger operand2) {
      if (operand1 == null) {
        throw new ArgumentNullException("operand1");
      }
      return operand1.multiply(operand2);
    }

    /// <summary>Divides a BigInteger object by the value of a BigInteger
    /// object.</summary>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='dividend'/> is null.</exception>
 public static EInteger operator /(
EInteger dividend,
EInteger divisor) {
      if (dividend == null) {
        throw new ArgumentNullException("dividend");
      }
      return dividend.divide(divisor);
    }

    /// <summary>Finds the remainder that results when a BigInteger object
    /// is divided by the value of a BigInteger object.</summary>
    /// <returns>The remainder of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='dividend'/> is null.</exception>
 public static EInteger operator %(
EInteger dividend,
EInteger divisor) {
      if (dividend == null) {
        throw new ArgumentNullException("dividend");
      }
      return dividend.remainder(divisor);
    }

    /// <param name='bthis'>Another BigInteger object.</param>
    /// <param name='bitCount'>A 32-bit signed integer.</param>
    /// <returns>An EInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bthis'/> is null.</exception>
    public static EInteger operator <<(EInteger bthis, int bitCount) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.ShiftLeft(bitCount);
    }

    /// <summary>Calculates the remainder when a BigInteger raised to a
    /// certain power is divided by another BigInteger.</summary>
    /// <param name='pow'>Another BigInteger object.</param>
    /// <param name='bigintValue'>Not documented yet.</param>
    /// <param name='pow'>Not documented yet.</param>
    /// <param name='mod'>Not documented yet. (3).</param>
    /// <returns>The value (<paramref name='bigintValue'/> ^ <paramref
    /// name='pow'/>)% <paramref name='mod'/>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintValue'/> is null.</exception>
    public static EInteger ModPow(
EInteger bigintValue,
EInteger pow,
EInteger mod) {
      if (bigintValue == null) {
        throw new ArgumentNullException("bigintValue");
      }
      return bigintValue.ModPow(pow, mod);
    }

    /// <summary>Shifts the bits of a BigInteger instance to the
    /// right.</summary>
    /// <param name='bthis'>Another BigInteger object.</param>
    /// <param name='bigValue'>A 32-bit signed integer.</param>
    /// <returns>An EInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bthis'/> is null.</exception>
    /// <remarks>For this operation, the BigInteger is treated as a two's
    /// complement representation. Thus, for negative values, the
    /// BigInteger is sign-extended.</remarks>
    public static EInteger operator >>(EInteger bthis, int bigValue) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.ShiftRight(bigValue);
    }

    /// <summary>Negates a BigInteger object.</summary>
    /// <param name='bigValue'>Another BigInteger object.</param>
    /// <returns>An EInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigValue'/> is null.</exception>
    public static EInteger operator -(EInteger bigValue) {
      if (bigValue == null) {
        throw new ArgumentNullException("bigValue");
      }
      return bigValue.negate();
    }

    /// <summary>Converts the value of a BigInteger object to a 64-bit
    /// signed integer.</summary>
    /// <param name='bigValue'>Not documented yet.</param>
    /// <returns>A 64-bit signed integer with the same value as the
    /// BigInteger object.</returns>
    /// <exception cref='OverflowException'>This object's value is too big
    /// to fit a 64-bit signed integer.</exception>
    public static explicit operator long(EInteger bigValue) {
      return bigValue.longValueChecked();
    }

    /// <summary>Converts the value of a BigInteger object to a 32-bit
    /// signed integer.</summary>
    /// <param name='bigValue'>Not documented yet.</param>
    /// <returns>A 32-bit signed integer with the same value as the
    /// BigInteger object.</returns>
    /// <exception cref='OverflowException'>This object's value is too big
    /// to fit a 32-bit signed integer.</exception>
    public static explicit operator int(EInteger bigValue) {
      return bigValue.AsInt32Checked();
    }

    /// <summary>Determines whether a BigInteger instance is less than
    /// another BigInteger instance.</summary>
    /// <param name='thisValue'>The first BigInteger object.</param>
    /// <param name='otherValue'>The second BigInteger object.</param>
    /// <returns>True if <paramref name='thisValue'/> is less than
    /// <paramref name='otherValue'/> ; otherwise, false.</returns>
    public static bool operator <(EInteger thisValue, EInteger otherValue) {
      return (thisValue == null) ? (otherValue != null) :
        (thisValue.CompareTo(otherValue) < 0);
    }

    /// <summary>Determines whether a BigInteger instance is less than or
    /// equal to another BigInteger instance.</summary>
    /// <param name='thisValue'>The first BigInteger object.</param>
    /// <param name='otherValue'>The second BigInteger object.</param>
    /// <returns>True if <paramref name='thisValue'/> is up to <paramref
    /// name='otherValue'/> ; otherwise, false.</returns>
  public static bool operator <=(
EInteger thisValue,
EInteger otherValue) {
      return (thisValue == null) || (thisValue.CompareTo(otherValue) <= 0);
    }

    /// <summary>Determines whether a BigInteger instance is greater than
    /// another BigInteger instance.</summary>
    /// <param name='thisValue'>The first BigInteger object.</param>
    /// <param name='otherValue'>The second BigInteger object.</param>
    /// <returns>True if <paramref name='thisValue'/> is greater than
    /// <paramref name='otherValue'/> ; otherwise, false.</returns>
    public static bool operator >(EInteger thisValue, EInteger otherValue) {
      return (thisValue != null) && (thisValue.CompareTo(otherValue) > 0);
    }

    /// <summary>Determines whether a BigInteger value is greater than
    /// another BigInteger value.</summary>
    /// <param name='thisValue'>The first BigInteger object.</param>
    /// <param name='otherValue'>The second BigInteger object.</param>
    /// <returns>True if <paramref name='thisValue'/> is at least <paramref
    /// name='otherValue'/> ; otherwise, false.</returns>
  public static bool operator >=(
EInteger thisValue,
EInteger otherValue) {
      return (thisValue == null) ? (otherValue == null) :
        (thisValue.CompareTo(otherValue) >= 0);
    }

    /// <summary>Gets a value indicating whether this object&#x27;s value
    /// is a power of two.</summary>
    /// <value>True if this object&#x27;s value is a power of two;
    /// otherwise, false.</value>
    public bool IsPowerOfTwo
    {
      get
      {
        int bits = this.bitLength();
        var ret = 0;
        for (var i = 0; i < bits; ++i) {
          ret += this.GetUnsignedBit(i) ? 1 : 0;
          if (ret >= 2) {
            return false;
          }
        }
        return ret == 1;
      }
    }

    /// <param name='thisValue'>Another BigInteger object.</param>
    /// <returns>An EInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='thisValue'/> is null.</exception>
    [CLSCompliant(false)]
    public static EInteger Abs(EInteger thisValue) {
      if (thisValue == null) {
        throw new ArgumentNullException("thisValue");
      }
      return thisValue.Abs();
    }

    public long GetBits(int index, int numberBits) {
      if (numberBits < 0 || numberBits > 64) {
        throw new ArgumentOutOfRangeException("numberBits");
      }
      long v = 0;
      for (int j = 0; j < numberBits; ++j) {
        v |= (long)(this.testBit((int)(index + j)) ? 1 : 0) << j;
      }
      return v;
    }

    /// <param name='dividend'>Big integer to be divided.</param>
    /// <param name='divisor'>An EInteger object.</param>
    /// <param name='remainder'>An EInteger object.</param>
    /// <returns>An array of two big integers: the first is the quotient,
    /// and the second is the remainder.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='dividend'/> or <paramref name='divisor'/> is
    /// null.</exception>
    public static EInteger DivRem(
EInteger dividend,
EInteger divisor,
out EInteger remainder) {
      if (dividend == null) {
        throw new ArgumentNullException("dividend");
      }
      if (divisor == null) {
  throw new ArgumentNullException("divisor");
}
      EInteger[] result = dividend.DivRem(divisor);
      remainder = result[1];
      return result[0];
    }

    /// <param name='bigintFirst'>Another BigInteger object.</param>
    /// <param name='bigintSecond'>An EInteger object.</param>
    /// <returns>An EInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintFirst'/> is null.</exception>
    public static EInteger GreatestCommonDivisor(
EInteger bigintFirst,
EInteger bigintSecond) {
      if (bigintFirst == null) {
        throw new ArgumentNullException("bigintFirst");
      }
      return bigintFirst.gcd(bigintSecond);
    }

    /// <param name='bigValue'>Another BigInteger object.</param>
    /// <param name='power'>An EInteger object.</param>
    /// <returns>An EInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigValue'/> or <paramref name='power'/> is null.</exception>
    [CLSCompliant(false)]
    public static EInteger Pow(EInteger bigValue, EInteger power) {
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
      EInteger val = EInteger.One;
      while (power.Sign > 0) {
        EInteger p = (power > (EInteger)5000000) ?
          (EInteger)5000000 : power;
        val *= bigValue.pow((int)p);
        power -= p;
      }
      return val;
    }

    /// <param name='bigValue'>Another BigInteger object.</param>
    /// <param name='power'>A 32-bit signed integer.</param>
    /// <returns>An EInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigValue'/> is null.</exception>
    [CLSCompliant(false)]
    public static EInteger Pow(EInteger bigValue, int power) {
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

    public bool Equals(EInteger other) {
      return (other != null) && (this.CompareTo(other) == 0);
    }

    /// <summary>Returns a BigInteger with every bit flipped.</summary>
    /// <param name='valueA'>Another BigInteger object.</param>
    /// <returns>An EInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='valueA'/> is null.</exception>
    public static EInteger Not(EInteger valueA) {
      if (valueA == null) {
        throw new ArgumentNullException("valueA");
      }
      if (valueA.wordCount == 0) {
        return EInteger.One.negate();
      }
      var valueXaNegative = false; int valueXaWordCount = 0;
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
      return (valueXaWordCount == 0) ? EInteger.Zero : (new
        EInteger(valueXaWordCount, valueXaReg, valueXaNegative));
    }

    /// <summary>Does an AND operation between two BigInteger
    /// values.</summary>
    /// <param name='a'>A BigInteger instance.</param>
    /// <param name='b'>Another BigInteger instance.</param>
    /// <returns>An EInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='a'/> or <paramref name='b'/> is null.</exception>
    /// <remarks>Each BigInteger instance is treated as a two's complement
    /// representation for the purposes of this operator.</remarks>
    public static EInteger And(EInteger a, EInteger b) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      if (b.IsZero || a.IsZero) {
        return Zero;
      }
      var valueXaNegative = false; int valueXaWordCount = 0;
      var valueXaReg = new short[a.wordCount];
      Array.Copy(a.words, valueXaReg, valueXaReg.Length);
      var valueXbNegative = false; int valueXbWordCount = 0;
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
      return (valueXaWordCount == 0) ? EInteger.Zero : (new
        EInteger(valueXaWordCount, valueXaReg, valueXaNegative));
    }

    /// <summary>Does an OR operation between two BigInteger
    /// instances.</summary>
    /// <param name='first'>Another BigInteger object.</param>
    /// <param name='second'>An EInteger object.</param>
    /// <returns>An EInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='first'/> or <paramref name='second'/> is null.</exception>
    /// <remarks>Each BigInteger instance is treated as a two's complement
    /// representation for the purposes of this operator.</remarks>
    public static EInteger Or(EInteger first, EInteger second) {
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
      var valueXaNegative = false; int valueXaWordCount = 0;
      var valueXaReg = new short[first.wordCount];
      Array.Copy(first.words, valueXaReg, valueXaReg.Length);
      var valueXbNegative = false; int valueXbWordCount = 0;
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
      return (valueXaWordCount == 0) ? EInteger.Zero : (new
        EInteger(valueXaWordCount, valueXaReg, valueXaNegative));
    }

    /// <summary>Finds the exclusive "or" of two BigInteger
    /// objects.</summary>
    /// <param name='a'>A BigInteger instance.</param>
    /// <param name='b'>Another BigInteger instance.</param>
    /// <returns>An EInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='a'/> or <paramref name='b'/> is null.</exception>
    /// <remarks>Each BigInteger instance is treated as a two's complement
    /// representation for the purposes of this operator.</remarks>
    public static EInteger Xor(EInteger a, EInteger b) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      if (a == b) {
        return EInteger.Zero;
      }
      if (a.wordCount == 0) {
        return b;
      }
      if (b.wordCount == 0) {
        return a;
      }
      var valueXaNegative = false; int valueXaWordCount = 0;
      var valueXaReg = new short[a.wordCount];
      Array.Copy(a.words, valueXaReg, valueXaReg.Length);
      var valueXbNegative = false; int valueXbWordCount = 0;
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
      return (valueXaWordCount == 0) ? EInteger.Zero : (new
        EInteger(valueXaWordCount, valueXaReg, valueXaNegative));
    }
  }
}
