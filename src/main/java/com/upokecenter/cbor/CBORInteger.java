package com.upokecenter.cbor;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import com.upokecenter.util.*;

  class CBORInteger implements ICBORNumber {

    public boolean IsPositiveInfinity(Object obj) {
      return false;
    }

    public boolean IsInfinity(Object obj) {
      return false;
    }

    public boolean IsNegativeInfinity(Object obj) {
      return false;
    }

    public boolean IsNaN(Object obj) {
      return false;
    }

    public double AsDouble(Object obj) {
      return ((Long)obj).doubleValue();
    }

    public ExtendedDecimal AsExtendedDecimal(Object obj) {
      return ExtendedDecimal.FromInt64((((Long)obj).longValue()));
    }

    public ExtendedFloat AsExtendedFloat(Object obj) {
      return ExtendedFloat.FromInt64((((Long)obj).longValue()));
    }

    public float AsSingle(Object obj) {
      return ((Long)obj).floatValue();
    }

    public BigInteger AsBigInteger(Object obj) {
      return BigInteger.valueOf((((Long)obj).longValue()));
    }

    public long AsInt64(Object obj) {
      return (((Long)obj).longValue());
    }

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

    public boolean CanFitInInt32(Object obj) {
      long val = (((Long)obj).longValue());
      return val >= Integer.MIN_VALUE && val <= Integer.MAX_VALUE;
    }

    public boolean CanFitInInt64(Object obj) {
      return true;
    }

    public Object Negate(Object obj) {
      return (((((Long)obj).longValue())) == Long.MIN_VALUE) ? (BigInteger.ONE.shiftLeft(63)) :
      (-((((Long)obj).longValue())));
    }

    public boolean CanTruncatedIntFitInInt64(Object obj) {
      return true;
    }

    public boolean CanTruncatedIntFitInInt32(Object obj) {
      long val = (((Long)obj).longValue());
      return val >= Integer.MIN_VALUE && val <= Integer.MAX_VALUE;
    }

    public boolean IsZero(Object obj) {
      return ((((Long)obj).longValue())) == 0;
    }

    public int Sign(Object obj) {
      long val = (((Long)obj).longValue());
      return (val == 0) ? 0 : ((val < 0) ? -1 : 1);
    }

    public boolean IsIntegral(Object obj) {
      return true;
    }

    public int AsInt32(Object obj, int minValue, int maxValue) {
      long val = (((Long)obj).longValue());
      if (val >= minValue && val <= maxValue) {
        return (int)val;
      }
      throw new ArithmeticException("This Object's value is out of range");
    }

    public Object Abs(Object obj) {
      long val = (((Long)obj).longValue());
      return (val == Integer.MIN_VALUE) ? (BigInteger.ONE.shiftLeft(63)) : ((val < 0) ?
      -val : obj);
    }

public ExtendedRational AsExtendedRational(Object obj) {
      return ExtendedRational.FromInt64((((Long)obj).longValue()));
    }
  }
