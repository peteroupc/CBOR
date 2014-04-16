/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

using System;
using PeterO;

namespace PeterO.Cbor
{
    /// <summary>Not documented yet.</summary>
  internal class CBORInteger : ICBORNumber
  {
    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsPositiveInfinity(object obj) {
      return false;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsInfinity(object obj) {
      return false;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsNegativeInfinity(object obj) {
      return false;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsNaN(object obj) {
      return false;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 64-bit floating-point number.</returns>
    public double AsDouble(object obj) {
      return (double)(long)obj;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal AsExtendedDecimal(object obj) {
      return ExtendedDecimal.FromInt64((long)obj);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat AsExtendedFloat(object obj) {
      return ExtendedFloat.FromInt64((long)obj);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 32-bit floating-point number.</returns>
    public float AsSingle(object obj) {
      return (float)(long)obj;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A BigInteger object.</returns>
    public BigInteger AsBigInteger(object obj) {
      return (BigInteger)(long)obj;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 64-bit signed integer.</returns>
    public long AsInt64(object obj) {
      return (long)obj;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInSingle(object obj) {
      long intItem = (long)obj;
      if (intItem == Int64.MinValue) {
        return true;
      }
      intItem = Math.Abs(intItem);
      while (intItem >= (1L << 24) && (intItem & 1) == 0) {
        intItem >>= 1;
      }
      return intItem < (1L << 24);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInDouble(object obj) {
      long intItem = (long)obj;
      if (intItem == Int64.MinValue) {
        return true;
      }
      intItem = Math.Abs(intItem);
      while (intItem >= (1L << 53) && (intItem & 1) == 0) {
        intItem >>= 1;
      }
      return intItem < (1L << 53);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInInt32(object obj) {
      long val = (long)obj;
      return val >= Int32.MinValue && val <= Int32.MaxValue;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInInt64(object obj) {
      return true;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object. (2).</param>
    /// <returns>An arbitrary object.</returns>
    public object Negate(object obj) {
      if (((long)obj) == Int64.MinValue) {
        return BigInteger.One << 63;
      }
      return -((long)obj);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanTruncatedIntFitInInt64(object obj) {
      return true;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanTruncatedIntFitInInt32(object obj) {
      long val = (long)obj;
      return val >= Int32.MinValue && val <= Int32.MaxValue;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsZero(object obj) {
      return ((long)obj) == 0;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int Sign(object obj) {
      long val = (long)obj;
      return (val == 0) ? 0 : ((val < 0) ? -1 : 1);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsIntegral(object obj) {
      return true;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <param name='minValue'>A 32-bit signed integer. (2).</param>
    /// <param name='maxValue'>A 32-bit signed integer. (3).</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int AsInt32(object obj, int minValue, int maxValue) {
      long val = (long)obj;
      if (val >= minValue && val <= maxValue) {
        return (int)val;
      }
      throw new OverflowException("This object's value is out of range");
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object. (2).</param>
    /// <returns>An arbitrary object.</returns>
    public object Abs(object obj) {
      long val = (long)obj;
      if (val == Int32.MinValue) {
        return BigInteger.One << 63;
      }
      return (val < 0) ? -val : obj;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>An ExtendedRational object.</returns>
public ExtendedRational AsExtendedRational(object obj) {
      return ExtendedRational.FromInt64((long)obj);
    }
  }
}
