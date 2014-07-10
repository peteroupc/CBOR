package com.upokecenter.cbor;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import com.upokecenter.util.*;

  final class CBORSingle implements ICBORNumber {

    private static final float SingleOneLsh64 = 9223372036854775808f;

    public boolean IsPositiveInfinity(Object obj) {
      return ((((Float)obj).floatValue()) == Float.POSITIVE_INFINITY);
    }

    public boolean IsInfinity(Object obj) {
      return ((Float)(((Float)obj).floatValue())).isInfinite();
    }

    public boolean IsNegativeInfinity(Object obj) {
      return ((((Float)obj).floatValue()) == Float.NEGATIVE_INFINITY);
    }

    public boolean IsNaN(Object obj) {
      return Float.isNaN(((Float)obj).floatValue());
    }

    public double AsDouble(Object obj) {
      return ((Float)obj).doubleValue();
    }

    public ExtendedDecimal AsExtendedDecimal(Object obj) {
      return ExtendedDecimal.FromSingle(((Float)obj).floatValue());
    }

    public ExtendedFloat AsExtendedFloat(Object obj) {
      return ExtendedFloat.FromSingle(((Float)obj).floatValue());
    }

    public float AsSingle(Object obj) {
      return ((Float)obj).floatValue();
    }

    public BigInteger AsBigInteger(Object obj) {
      return CBORUtilities.BigIntegerFromSingle(((Float)obj).floatValue());
    }

    public long AsInt64(Object obj) {
      float fltItem = ((Float)obj).floatValue();
      if (Float.isNaN(fltItem)) {
        throw new ArithmeticException("This Object's value is out of range");
      }
      fltItem = (fltItem < 0) ? (float)Math.ceil(fltItem) :
        (float)Math.floor(fltItem);
      if (fltItem >= -SingleOneLsh64 && fltItem < SingleOneLsh64) {
        return (long)fltItem;
      }
      throw new ArithmeticException("This Object's value is out of range");
    }

    public boolean CanFitInSingle(Object obj) {
      return true;
    }

    public boolean CanFitInDouble(Object obj) {
      return true;
    }

    public boolean CanFitInInt32(Object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt32(obj);
    }

    public boolean CanFitInInt64(Object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt64(obj);
    }

    public boolean CanTruncatedIntFitInInt64(Object obj) {
      float fltItem = ((Float)obj).floatValue();
      if (Float.isNaN(fltItem) || ((Float)(fltItem)).isInfinite()) {
        return false;
      }
      float fltItem2 = (fltItem < 0) ? (float)Math.ceil(fltItem) :
        (float)Math.floor(fltItem);
      return fltItem2 >= -SingleOneLsh64 && fltItem2 <
        SingleOneLsh64;
    }

    public boolean CanTruncatedIntFitInInt32(Object obj) {
      float fltItem = ((Float)obj).floatValue();
      if (Float.isNaN(fltItem) || ((Float)(fltItem)).isInfinite()) {
        return false;
      }
      float fltItem2 = (fltItem < 0) ? (float)Math.ceil(fltItem) :
        (float)Math.floor(fltItem);
      // Convert float to double to avoid precision loss when
      // converting Integer.MIN_VALUE/MaxValue to float
      return (double)fltItem2 >= Integer.MIN_VALUE && (double)fltItem2 <=
        Integer.MAX_VALUE;
    }

    public int AsInt32(Object obj, int minValue, int maxValue) {
      float fltItem = ((Float)obj).floatValue();
      if (Float.isNaN(fltItem)) {
        throw new ArithmeticException("This Object's value is out of range");
      }
      fltItem = (fltItem < 0) ? (float)Math.ceil(fltItem) :
        (float)Math.floor(fltItem);
      // Convert float to double to avoid precision loss when
      // converting Integer.MIN_VALUE/MaxValue to float
      if ((double)fltItem >= Integer.MIN_VALUE && (double)fltItem <=
          Integer.MAX_VALUE) {
        int ret = (int)fltItem;
        return ret;
      }
      throw new ArithmeticException("This Object's value is out of range");
    }

    public boolean IsZero(Object obj) {
      return ((Float)obj).floatValue() == 0.0f;
    }

    public int Sign(Object obj) {
      float flt = ((Float)obj).floatValue();
      return Float.isNaN(flt) ? 2 : (flt == 0.0f ? 0 : (flt < 0.0f ? -1 : 1));
    }

    public boolean IsIntegral(Object obj) {
      float fltItem = ((Float)obj).floatValue();
      if (Float.isNaN(fltItem) || ((Float)(fltItem)).isInfinite()) {
        return false;
      }
      float fltItem2 = (fltItem < 0) ? (float)Math.ceil(fltItem) :
        (float)Math.floor(fltItem);
      return fltItem == fltItem2;
    }

    public Object Negate(Object obj) {
      float val = ((Float)obj).floatValue();
      return -val;
    }

    public Object Abs(Object obj) {
      float val = ((Float)obj).floatValue();
      return (val < 0) ? -val : obj;
    }

    public ExtendedRational AsExtendedRational(Object obj) {
      return ExtendedRational.FromSingle(((Float)obj).floatValue());
    }
  }
