/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Numbers {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Numbers.EInteger"]/*'/>
  internal sealed partial class EInteger {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Implicit(System.Int64)~PeterO.Numbers.EInteger"]/*'
    /// />
    public static implicit operator EInteger(long bigValue) {
      return FromInt64(bigValue);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Implicit(System.Int32)~PeterO.Numbers.EInteger"]/*'
    /// />
    public static implicit operator EInteger(int smallValue) {
      return FromInt64((long)smallValue);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Addition(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'
    /// />
    public static EInteger operator +(EInteger bthis, EInteger augend) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.Add(augend);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Subtraction(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'
    /// />
 public static EInteger operator -(
EInteger bthis,
EInteger subtrahend) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.Subtract(subtrahend);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Multiply(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'
    /// />
public static EInteger operator *(
EInteger operand1,
EInteger operand2) {
      if (operand1 == null) {
        throw new ArgumentNullException("operand1");
      }
      return operand1.Multiply(operand2);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Division(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'
    /// />
 public static EInteger operator /(
EInteger dividend,
EInteger divisor) {
      if (dividend == null) {
        throw new ArgumentNullException("dividend");
      }
      return dividend.Divide(divisor);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Modulus(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'
    /// />
 public static EInteger operator %(
EInteger dividend,
EInteger divisor) {
      if (dividend == null) {
        throw new ArgumentNullException("dividend");
      }
      return dividend.Remainder(divisor);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_LeftShift(PeterO.Numbers.EInteger,System.Int32)"]/*'
    /// />
    public static EInteger operator <<(EInteger bthis, int bitCount) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.ShiftLeft(bitCount);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.ModPow(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'
    /// />
    public static EInteger ModPow(
EInteger bigintValue,
EInteger pow,
EInteger mod) {
      if (bigintValue == null) {
        throw new ArgumentNullException("bigintValue");
      }
      return bigintValue.ModPow(pow, mod);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_RightShift(PeterO.Numbers.EInteger,System.Int32)"]/*'
    /// />
    public static EInteger operator >>(EInteger bthis, int smallValue) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.ShiftRight(smallValue);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_UnaryNegation(PeterO.Numbers.EInteger)"]/*'
    /// />
    public static EInteger operator -(EInteger bigValue) {
      if (bigValue == null) {
        throw new ArgumentNullException("bigValue");
      }
      return bigValue.Negate();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Explicit(PeterO.Numbers.EInteger)~System.Int64"]/*'
    /// />
    public static explicit operator long(EInteger bigValue) {
      return bigValue.AsInt64Checked();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Explicit(PeterO.Numbers.EInteger)~System.Int32"]/*'
    /// />
    public static explicit operator int(EInteger bigValue) {
      return bigValue.AsInt32Checked();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_LessThan(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'
    /// />
    public static bool operator <(EInteger thisValue, EInteger otherValue) {
      return (thisValue == null) ? (otherValue != null) :
        (thisValue.CompareTo(otherValue) < 0);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_LessThanOrEqual(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'
    /// />
  public static bool operator <=(
EInteger thisValue,
EInteger otherValue) {
      return (thisValue == null) || (thisValue.CompareTo(otherValue) <= 0);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_GreaterThan(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'
    /// />
    public static bool operator >(EInteger thisValue, EInteger otherValue) {
      return (thisValue != null) && (thisValue.CompareTo(otherValue) > 0);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_GreaterThanOrEqual(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'
    /// />
  public static bool operator >=(
EInteger thisValue,
EInteger otherValue) {
      return (thisValue == null) ? (otherValue == null) :
        (thisValue.CompareTo(otherValue) >= 0);
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.DivRem(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger,PeterO.Numbers.EInteger@)"]/*'
    /// />
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.GreatestCommonDivisor(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'
    /// />
    public static EInteger GreatestCommonDivisor(
EInteger bigintFirst,
EInteger bigintSecond) {
      if (bigintFirst == null) {
        throw new ArgumentNullException("bigintFirst");
      }
      return bigintFirst.gcd(bigintSecond);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.Pow(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'
    /// />
    /// <summary>Not documented yet.</summary>
    /// <returns>Not documented yet.</returns>
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.Pow(PeterO.Numbers.EInteger,System.Int32)"]/*'
    /// />
  /// <summary>Not documented yet.</summary>
  /// <returns>Not documented yet.</returns>
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.Not(PeterO.Numbers.EInteger)"]/*'
    /// />
    public static EInteger Not(EInteger valueA) {
      if (valueA == null) {
        throw new ArgumentNullException("valueA");
      }
      if (valueA.wordCount == 0) {
        return EInteger.One.Negate();
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.And(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'
    /// />
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.Or(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'
    /// />
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.Xor(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'
    /// />
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
