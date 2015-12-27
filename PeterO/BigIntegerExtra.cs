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
    /// <include file='docs.xml'
    /// path='docs/doc[@name="T:PeterO.Numbers.EInteger"]'/>
  public sealed partial class BigInteger {
    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Implicit(System.Int64)~PeterO.Numbers.EInteger"]'
    /// />
    public static implicit operator BigInteger(long bigValue) {
      return new BigInteger(EInteger.FromInt64(bigValue));
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Implicit(System.Int32)~PeterO.Numbers.EInteger"]'
    /// />
    public static implicit operator BigInteger(int smallValue) {
      return new BigInteger(EInteger.FromInt64(smallValue));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Addition(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'
    /// />
    public static BigInteger operator +(BigInteger bthis, BigInteger augend) {
      if ((bthis) == null) {
  throw new ArgumentNullException("bthis");
}
      return bthis.add(augend);
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Subtraction(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'
    /// />
    public static BigInteger operator -(
BigInteger bthis,
BigInteger subtrahend) {
      if ((bthis) == null) {
  throw new ArgumentNullException("bthis");
}
      return bthis.subtract(subtrahend);
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Multiply(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'
    /// />
    public static BigInteger operator *(
BigInteger operand1,
BigInteger operand2) {
      if (operand1 == null) {
        throw new ArgumentNullException("operand1");
      }
      return operand1.multiply(operand2);
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Division(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'
    /// />
 public static BigInteger operator /(
BigInteger dividend,
BigInteger divisor) {
      if ((dividend) == null) {
  throw new ArgumentNullException("dividend");
}
      return dividend.divide(divisor);
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Modulus(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'
    /// />
    public static BigInteger operator %(
BigInteger dividend,
BigInteger divisor) {
      if ((dividend) == null) {
  throw new ArgumentNullException("dividend");
}
      return dividend.remainder(divisor);
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_LeftShift(PeterO.Numbers.EInteger,System.Int32)"]'
    /// />
    public static BigInteger operator <<(BigInteger bthis, int bitCount) {
      if ((bthis) == null) {
  throw new ArgumentNullException("bthis");
}
      return bthis.shiftLeft(bitCount);
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.ModPow(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'
    /// />
    public static BigInteger ModPow(
BigInteger bigintValue,
BigInteger pow,
BigInteger mod) {
      return bigintValue.ModPow(pow, mod);
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_RightShift(PeterO.Numbers.EInteger,System.Int32)"]'
    /// />
    public static BigInteger operator >>(BigInteger bthis, int smallValue) {
      if ((bthis) == null) {
  throw new ArgumentNullException("bthis");
}
      return bthis.shiftRight(smallValue);
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_UnaryNegation(PeterO.Numbers.EInteger)"]'
    /// />
    public static BigInteger operator -(BigInteger bigValue) {
      if ((bigValue) == null) {
  throw new ArgumentNullException("bigValue");
}
      return bigValue.negate();
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Explicit(PeterO.Numbers.EInteger)~System.Int64"]'
    /// />
    public static explicit operator long(BigInteger bigValue) {
      return bigValue.longValueChecked();
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_Explicit(PeterO.Numbers.EInteger)~System.Int32"]'
    /// />
    public static explicit operator int(BigInteger bigValue) {
      return bigValue.intValueChecked();
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_LessThan(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'
    /// />
    public static bool operator <(BigInteger thisValue, BigInteger otherValue) {
      return (thisValue == null) ? (otherValue != null) :
        (thisValue.CompareTo(otherValue) < 0);
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_LessThanOrEqual(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'
    /// />
    public static bool operator <=(
BigInteger thisValue,
BigInteger otherValue) {
      return (thisValue == null) || (thisValue.CompareTo(otherValue) <= 0);
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_GreaterThan(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'
    /// />
    public static bool operator >(BigInteger thisValue, BigInteger otherValue) {
      return otherValue < thisValue;
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.op_GreaterThanOrEqual(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'
    /// />
  public static bool operator >=(
BigInteger thisValue,
BigInteger otherValue) {
      return (thisValue == null) ? (otherValue == null) :
        (thisValue.CompareTo(otherValue) >= 0);
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EInteger.IsPowerOfTwo"]'/>
    public bool IsPowerOfTwo { get {
        return this.ei.IsPowerOfTwo;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.Abs(PeterO.BigInteger)"]'/>
    [CLSCompliant(false)]
    public static BigInteger Abs(BigInteger thisValue) {
      if ((thisValue) == null) {
  throw new ArgumentNullException("thisValue");
}
      return thisValue.abs();
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.BigInteger.Zero"]'/>
    [CLSCompliant(false)]
    public static BigInteger Zero { get {
 return BigInteger.ZERO;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="P:PeterO.BigInteger.One"]'/>
    [CLSCompliant(false)]
    public static BigInteger One { get {
 return BigInteger.ONE;
} }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.GetBits(System.Int32,System.Int32)"]'
    /// />
    public long GetBits(int index, int numberBits) {
      return this.ei.GetBits(index, numberBits);
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.DivRem(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger,PeterO.Numbers.EInteger@)"]'
    /// />
    public static BigInteger DivRem(
BigInteger dividend,
BigInteger divisor,
out BigInteger remainder) {
      if ((dividend) == null) {
  throw new ArgumentNullException("dividend");
}
      BigInteger[] ret = dividend.divideAndRemainder(divisor);
      remainder = ret[1];
      return ret[0];
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.GreatestCommonDivisor(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'
    /// />
    public static BigInteger GreatestCommonDivisor(
BigInteger bigintFirst,
BigInteger bigintSecond) {
      return bigintFirst.gcd(bigintSecond);
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.ToByteArray"]'/>
    [Obsolete("Use 'toBytes(true)' instead.")]
    [CLSCompliant(false)]
    public byte[] ToByteArray() {
      return toBytes(true);
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.(PeterO.Numbers.EInteger)
    /// .pow(PeterO.Numbers.EInteger)"]'/>
    [CLSCompliant(false)]
    public static BigInteger Pow(BigInteger bigValue, BigInteger power) {
      return bigValue.PowBigIntVar(power);
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.(PeterO.Numbers.EInteger)
    /// .pow(System.Int32)"]'/>
    [CLSCompliant(false)]
    public static BigInteger Pow(BigInteger bigValue, int power) {
      return bigValue.pow(power);
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.BigInteger.Equals(PeterO.BigInteger)"]'/>
    public bool Equals(BigInteger other) {
      return this.Equals((object)other);
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.Not(PeterO.Numbers.EInteger)"]'
    /// />
    public static BigInteger Not(BigInteger valueA) {
      return new BigInteger(EInteger.Not(valueA.ei));
 }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.And(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'
    /// />
    public static BigInteger And(BigInteger a, BigInteger b) {
      if ((a) == null) {
  throw new ArgumentNullException("a");
}
      if ((b) == null) {
  throw new ArgumentNullException("b");
}
      return new BigInteger(EInteger.And(a.ei, b.ei));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.Or(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'
    /// />
    public static BigInteger Or(BigInteger first, BigInteger second) {
      if ((first) == null) {
  throw new ArgumentNullException("first");
}
      if ((second) == null) {
  throw new ArgumentNullException("second");
}
      return new BigInteger(EInteger.Or(first.ei, second.ei));
    }

    /// <include file='docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EInteger.Xor(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]'
    /// />
    public static BigInteger Xor(BigInteger a, BigInteger b) {
      if ((a) == null) {
  throw new ArgumentNullException("a");
}
      if ((b) == null) {
  throw new ArgumentNullException("b");
}
      return new BigInteger(EInteger.Xor(a.ei, b.ei));
    }
  }
}
