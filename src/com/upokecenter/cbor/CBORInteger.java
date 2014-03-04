package com.upokecenter.cbor;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

import com.upokecenter.util.*;

    /**
     * Not documented yet.
     */
  class CBORInteger implements ICBORNumber
  {
    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsPositiveInfinity(Object obj) {
      return false;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsInfinity(Object obj) {
      return false;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsNegativeInfinity(Object obj) {
      return false;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsNaN(Object obj) {
      return false;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 64-bit floating-point number.
     */
    public double AsDouble(Object obj) {
      return ((Long)obj).doubleValue();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedDecimal object.
     */
    public ExtendedDecimal AsExtendedDecimal(Object obj) {
      return ExtendedDecimal.FromInt64((((Long)obj).longValue()));
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat AsExtendedFloat(Object obj) {
      return ExtendedFloat.FromInt64((((Long)obj).longValue()));
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 32-bit floating-point number.
     */
    public float AsSingle(Object obj) {
      return ((Long)obj).floatValue();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A BigInteger object.
     */
    public BigInteger AsBigInteger(Object obj) {
      return BigInteger.valueOf((((Long)obj).longValue()));
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 64-bit signed integer.
     */
    public long AsInt64(Object obj) {
      return (((Long)obj).longValue());
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInSingle(Object obj) {
      long intItem = (((Long)obj).longValue());
      if (intItem == Long.MIN_VALUE) {
        return true;
      }
      intItem = Math.abs(intItem);
      while (intItem >= (1L << 24) && (intItem & 1) == 0) {
        intItem >>= 1;
      }
      return intItem < (1L << 24);
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInDouble(Object obj) {
      long intItem = (((Long)obj).longValue());
      if (intItem == Long.MIN_VALUE) {
        return true;
      }
      intItem = Math.abs(intItem);
      while (intItem >= (1L << 53) && (intItem & 1) == 0) {
        intItem >>= 1;
      }
      return intItem < (1L << 53);
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInInt32(Object obj) {
      long val = (((Long)obj).longValue());
      return val >= Integer.MIN_VALUE && val <= Integer.MAX_VALUE;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInInt64(Object obj) {
      return true;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object. (2).
     * @return An arbitrary object.
     */
    public Object Negate(Object obj) {
      if (((((Long)obj).longValue())) == Long.MIN_VALUE) {
        return BigInteger.ONE.shiftLeft(63);
      }
      return -((((Long)obj).longValue()));
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanTruncatedIntFitInInt64(Object obj) {
      return true;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanTruncatedIntFitInInt32(Object obj) {
      long val = (((Long)obj).longValue());
      return val >= Integer.MIN_VALUE && val <= Integer.MAX_VALUE;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsZero(Object obj) {
      return ((((Long)obj).longValue())) == 0;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 32-bit signed integer.
     */
    public int Sign(Object obj) {
      long val = (((Long)obj).longValue());
      return (val == 0) ? 0 : ((val < 0) ? -1 : 1);
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsIntegral(Object obj) {
      return true;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @param minValue A 32-bit signed integer. (2).
     * @param maxValue A 32-bit signed integer. (3).
     * @return A 32-bit signed integer.
     */
    public int AsInt32(Object obj, int minValue, int maxValue) {
      long val = (((Long)obj).longValue());
      if (val >= minValue && val <= maxValue) {
        return (int)val;
      }
      throw new ArithmeticException("This Object's value is out of range");
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object. (2).
     * @return An arbitrary object.
     */
    public Object Abs(Object obj) {
      long val = (((Long)obj).longValue());
      if (val == Integer.MIN_VALUE) {
        return BigInteger.ONE.shiftLeft(63);
      }
      return (val < 0) ? -val : obj;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedRational object.
     */
public ExtendedRational AsExtendedRational(Object obj) {
      return ExtendedRational.FromInt64((((Long)obj).longValue()));
    }
  }
