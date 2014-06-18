/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

using System;
using PeterO;

namespace PeterO.Cbor {
  internal class CBORInteger : ICBORNumber
  {
    public bool IsPositiveInfinity(object obj) {
      return false;
    }

    public bool IsInfinity(object obj) {
      return false;
    }

    public bool IsNegativeInfinity(object obj) {
      return false;
    }

    public bool IsNaN(object obj) {
      return false;
    }

    public double AsDouble(object obj) {
      return (double)(long)obj;
    }

    public ExtendedDecimal AsExtendedDecimal(object obj) {
      return ExtendedDecimal.FromInt64((long)obj);
    }

    public ExtendedFloat AsExtendedFloat(object obj) {
      return ExtendedFloat.FromInt64((long)obj);
    }

    public float AsSingle(object obj) {
      return (float)(long)obj;
    }

    public BigInteger AsBigInteger(object obj) {
      return (BigInteger)(long)obj;
    }

    public long AsInt64(object obj) {
      return (long)obj;
    }

    public bool CanFitInSingle(object obj) {
      var intItem = (long)obj;
      if (intItem == Int64.MinValue) {
        return true;
      }
      intItem = Math.Abs(intItem);
      while (intItem >= (1L << 24) && (intItem & 1) == 0) {
        intItem >>= 1;
      }
      return intItem < (1L << 24);
    }

    public bool CanFitInDouble(object obj) {
      var intItem = (long)obj;
      if (intItem == Int64.MinValue) {
        return true;
      }
      intItem = Math.Abs(intItem);
      while (intItem >= (1L << 53) && (intItem & 1) == 0) {
        intItem >>= 1;
      }
      return intItem < (1L << 53);
    }

    public bool CanFitInInt32(object obj) {
      var val = (long)obj;
      return val >= Int32.MinValue && val <= Int32.MaxValue;
    }

    public bool CanFitInInt64(object obj) {
      return true;
    }

    public object Negate(object obj) {
      if (((long)obj) == Int64.MinValue) {
        return BigInteger.One << 63;
      }
      return -((long)obj);
    }

    public bool CanTruncatedIntFitInInt64(object obj) {
      return true;
    }

    public bool CanTruncatedIntFitInInt32(object obj) {
      var val = (long)obj;
      return val >= Int32.MinValue && val <= Int32.MaxValue;
    }

    public bool IsZero(object obj) {
      return ((long)obj) == 0;
    }

    public int Sign(object obj) {
      var val = (long)obj;
      return (val == 0) ? 0 : ((val < 0) ? -1 : 1);
    }

    public bool IsIntegral(object obj) {
      return true;
    }

    public int AsInt32(object obj, int minValue, int maxValue) {
      var val = (long)obj;
      if (val >= minValue && val <= maxValue) {
        return (int)val;
      }
      throw new OverflowException("This object's value is out of range");
    }

    public object Abs(object obj) {
      var val = (long)obj;
      if (val == Int32.MinValue) {
        return BigInteger.One << 63;
      }
      return (val < 0) ? -val : obj;
    }

public ExtendedRational AsExtendedRational(object obj) {
      return ExtendedRational.FromInt64((long)obj);
    }
  }
}
