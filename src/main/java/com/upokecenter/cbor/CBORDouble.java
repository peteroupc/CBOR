package com.upokecenter.cbor;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import com.upokecenter.util.*;

  class CBORDouble implements ICBORNumber {

    public boolean IsPositiveInfinity(final Object obj) {
      return ((((Double)obj).doubleValue()) == Double.POSITIVE_INFINITY);
    }

    public boolean IsInfinity(final Object obj) {
      return ((Double)(((Double)obj).doubleValue())).isInfinite();
    }

    public boolean IsNegativeInfinity(final Object obj) {
      return ((((Double)obj).doubleValue()) == Double.NEGATIVE_INFINITY);
    }

    public boolean IsNaN(final Object obj) {
      return Double.isNaN(((Double)obj).doubleValue());
    }

    public double AsDouble(final Object obj) {
      return ((Double)obj).doubleValue();
    }

    public ExtendedDecimal AsExtendedDecimal(final Object obj) {
      return ExtendedDecimal.FromDouble(((Double)obj).doubleValue());
    }

    public ExtendedFloat AsExtendedFloat(final Object obj) {
      return ExtendedFloat.FromDouble(((Double)obj).doubleValue());
    }

    public float AsSingle(final Object obj) {
      return ((Double)obj).floatValue();
    }

    public BigInteger AsBigInteger(final Object obj) {
      return CBORUtilities.BigIntegerFromDouble(((Double)obj).doubleValue());
    }

    public long AsInt64(final Object obj) {
      double fltItem = ((Double)obj).doubleValue();
      if (Double.isNaN(fltItem)) {
        throw new ArithmeticException("This Object's value is out of range");
      }
      fltItem = (fltItem < 0) ? Math.ceil(fltItem) : Math.floor(fltItem);
      if (fltItem >= -9223372036854775808.0 && fltItem <
      9223372036854775808.0) {
        return (long)fltItem;
      }
      throw new ArithmeticException("This Object's value is out of range");
    }

    public boolean CanFitInSingle(final Object obj) {
      double fltItem = ((Double)obj).doubleValue();
      if (Double.isNaN(fltItem)) {
        return true;
      }
      float sing = (float)fltItem;
      return (double)sing == (double)fltItem;
    }

    public boolean CanFitInDouble(final Object obj) {
      return true;
    }

    public boolean CanFitInInt32(final Object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt32(obj);
    }

    public boolean CanFitInInt64(final Object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt64(obj);
    }

    public boolean CanTruncatedIntFitInInt64(final Object obj) {
      double fltItem = ((Double)obj).doubleValue();
      if (Double.isNaN(fltItem) || ((Double)(fltItem)).isInfinite()) {
        return false;
      }
      double fltItem2 = (fltItem < 0) ? Math.ceil(fltItem) :
      Math.floor(fltItem);
      return fltItem2 >= -9223372036854775808.0 && fltItem2 <
      9223372036854775808.0;
    }

    public boolean CanTruncatedIntFitInInt32(final Object obj) {
      double fltItem = ((Double)obj).doubleValue();
      if (Double.isNaN(fltItem) || ((Double)(fltItem)).isInfinite()) {
        return false;
      }
      double fltItem2 = (fltItem < 0) ? Math.ceil(fltItem) :
      Math.floor(fltItem);
      return fltItem2 >= Integer.MIN_VALUE && fltItem2 <= Integer.MAX_VALUE;
    }

    public int AsInt32(final Object obj, int minValue, int maxValue) {
      double fltItem = ((Double)obj).doubleValue();
      if (Double.isNaN(fltItem)) {
        throw new ArithmeticException("This Object's value is out of range");
      }
      fltItem = (fltItem < 0) ? Math.ceil(fltItem) : Math.floor(fltItem);
      if (fltItem >= minValue && fltItem <= maxValue) {
        int ret = (int)fltItem;
        return ret;
      }
      throw new ArithmeticException("This Object's value is out of range");
    }

    public boolean IsZero(final Object obj) {
      return ((Double)obj).doubleValue() == 0.0;
    }

    public int Sign(final Object obj) {
      double flt = ((Double)obj).doubleValue();
      return Double.isNaN(flt) ? 2 : ((double)flt == 0.0 ? 0 : (flt < 0.0f ?
      -1 : 1));
    }

    public boolean IsIntegral(final Object obj) {
      double fltItem = ((Double)obj).doubleValue();
      if (Double.isNaN(fltItem) || ((Double)(fltItem)).isInfinite()) {
        return false;
      }
      double fltItem2 = (fltItem < 0) ? Math.ceil(fltItem) :
      Math.floor(fltItem);
      return fltItem == fltItem2;
    }

    public Object Negate(final Object obj) {
      double val = ((Double)obj).doubleValue();
      return -val;
    }

    public Object Abs(final Object obj) {
      double val = ((Double)obj).doubleValue();
      return (val < 0) ? -val : obj;
    }

    public ExtendedRational AsExtendedRational(final Object obj) {
      return ExtendedRational.FromDouble(((Double)obj).doubleValue());
    }
  }
