/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using PeterO.Numbers;

namespace PeterO {
    /// <include file='../docs.xml' path='docs/doc[@name="blank"]/*'/>
  public sealed partial class BigInteger {
    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_Implicit(System.Int64)~PeterO.BigInteger"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static implicit operator BigInteger(long bigValue) {
      return new BigInteger(EInteger.FromInt64(bigValue));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_Implicit(System.Int32)~PeterO.BigInteger"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static implicit operator BigInteger(int smallValue) {
      return new BigInteger(EInteger.FromInt64(smallValue));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_Addition(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger operator +(BigInteger bthis, BigInteger augend) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.add(augend);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_Subtraction(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger operator -(
BigInteger bthis,
BigInteger subtrahend) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.subtract(subtrahend);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_Multiply(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger operator *(
BigInteger operand1,
BigInteger operand2) {
      if (operand1 == null) {
        throw new ArgumentNullException("operand1");
      }
      return operand1.multiply(operand2);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_Division(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger operator /(
BigInteger dividend,
BigInteger divisor) {
      if (dividend == null) {
        throw new ArgumentNullException("dividend");
      }
      return dividend.divide(divisor);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_Modulus(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger operator %(
BigInteger dividend,
BigInteger divisor) {
      if (dividend == null) {
        throw new ArgumentNullException("dividend");
      }
      return dividend.remainder(divisor);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_LeftShift(PeterO.BigInteger,System.Int32)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger operator <<(BigInteger bthis, int bitCount) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.shiftLeft(bitCount);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.ModPow(PeterO.BigInteger,PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger ModPow(
BigInteger bigintValue,
BigInteger pow,
BigInteger mod) {
      return bigintValue.ModPow(pow, mod);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_RightShift(PeterO.BigInteger,System.Int32)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger operator >>(BigInteger bthis, int smallValue) {
      if (bthis == null) {
        throw new ArgumentNullException("bthis");
      }
      return bthis.shiftRight(smallValue);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_UnaryNegation(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger operator -(BigInteger bigValue) {
      if (bigValue == null) {
        throw new ArgumentNullException("bigValue");
      }
      return bigValue.negate();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_Explicit(PeterO.BigInteger)~System.Int64"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static explicit operator long (BigInteger bigValue) {
      return bigValue.longValueChecked();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_Explicit(PeterO.BigInteger)~System.Int32"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static explicit operator int (BigInteger bigValue) {
      return bigValue.intValueChecked();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_LessThan(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static bool operator <(BigInteger thisValue, BigInteger otherValue) {
      return (thisValue == null) ? (otherValue != null) :
        (thisValue.CompareTo(otherValue) < 0);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_LessThanOrEqual(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static bool operator <=(
BigInteger thisValue,
BigInteger otherValue) {
      return (thisValue == null) || (thisValue.CompareTo(otherValue) <= 0);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_GreaterThan(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static bool operator >(BigInteger thisValue, BigInteger otherValue) {
      return otherValue < thisValue;
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.op_GreaterThanOrEqual(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static bool operator >=(
  BigInteger thisValue,
  BigInteger otherValue) {
      return (thisValue == null) ? (otherValue == null) :
        (thisValue.CompareTo(otherValue) >= 0);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.BigInteger.IsPowerOfTwo"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public bool IsPowerOfTwo {
      get {
        return this.Ei.Abs().IsPowerOfTwo;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.Abs(PeterO.BigInteger)"]/*'/>
    [CLSCompliant(false)]
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger Abs(BigInteger thisValue) {
      if (thisValue == null) {
        throw new ArgumentNullException("thisValue");
      }
      return thisValue.abs();
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.BigInteger.Zero"]/*'/>
    [CLSCompliant(false)]
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger Zero {
      get {
        return BigInteger.ZERO;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="P:PeterO.BigInteger.One"]/*'/>
    [CLSCompliant(false)]
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger One {
      get {
        return BigInteger.ONE;
      }
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.GetBits(System.Int32,System.Int32)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public long GetBits(int index, int numberBits) {
      return this.Ei.GetBits(index, numberBits);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.DivRem(PeterO.BigInteger,PeterO.BigInteger,PeterO.BigInteger@)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger DivRem(
BigInteger dividend,
BigInteger divisor,
out BigInteger remainder) {
      if (dividend == null) {
        throw new ArgumentNullException("dividend");
      }
      BigInteger[] ret = dividend.divideAndRemainder(divisor);
      remainder = ret[1];
      return ret[0];
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.GreatestCommonDivisor(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger GreatestCommonDivisor(
BigInteger bigintFirst,
BigInteger bigintSecond) {
      return bigintFirst.gcd(bigintSecond);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.ToByteArray"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    [CLSCompliant(false)]
    public byte[] ToByteArray() {
      return this.toBytes(true);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.Pow(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [CLSCompliant(false)]
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger Pow(BigInteger bigValue, BigInteger power) {
      return bigValue.PowBigIntVar(power);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.Pow(PeterO.BigInteger,System.Int32)"]/*'/>
    [CLSCompliant(false)]
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger Pow(BigInteger bigValue, int power) {
      return bigValue.pow(power);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.Equals(PeterO.BigInteger)"]/*'/>
    public bool Equals(BigInteger other) {
      return this.Equals((object)other);
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.Not(PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger Not(BigInteger valueA) {
      return new BigInteger(EInteger.Not(valueA.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.And(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger And(BigInteger a, BigInteger b) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      return new BigInteger(EInteger.And(a.Ei, b.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.Or(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger Or(BigInteger first, BigInteger second) {
      if (first == null) {
        throw new ArgumentNullException("first");
      }
      if (second == null) {
        throw new ArgumentNullException("second");
      }
      return new BigInteger(EInteger.Or(first.Ei, second.Ei));
    }

    /// <include file='../docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.Xor(PeterO.BigInteger,PeterO.BigInteger)"]/*'/>
    [Obsolete("Use EInteger from PeterO.Numbers/com.upokecenter.numbers.")]
    public static BigInteger Xor(BigInteger a, BigInteger b) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      return new BigInteger(EInteger.Xor(a.Ei, b.Ei));
    }
  }
}
