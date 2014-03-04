package com.upokecenter.cbor;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

import com.upokecenter.util.*;

  final class CBORSingle implements ICBORNumber
  {
    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsPositiveInfinity(Object obj) {
      return ((((Float)obj).floatValue())==Float.POSITIVE_INFINITY);
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsInfinity(Object obj) {
      return ((Float)(((Float)obj).floatValue())).isInfinite();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsNegativeInfinity(Object obj) {
      return ((((Float)obj).floatValue())==Float.NEGATIVE_INFINITY);
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsNaN(Object obj) {
      return Float.isNaN(((Float)obj).floatValue());
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 64-bit floating-point number.
     */
    public double AsDouble(Object obj) {
      return ((Float)obj).doubleValue();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedDecimal object.
     */
    public ExtendedDecimal AsExtendedDecimal(Object obj) {
      return ExtendedDecimal.FromSingle(((Float)obj).floatValue());
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat AsExtendedFloat(Object obj) {
      return ExtendedFloat.FromSingle(((Float)obj).floatValue());
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 32-bit floating-point number.
     */
    public float AsSingle(Object obj) {
      return ((Float)obj).floatValue();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A BigInteger object.
     */
    public BigInteger AsBigInteger(Object obj) {
      return CBORUtilities.BigIntegerFromSingle(((Float)obj).floatValue());
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 64-bit signed integer.
     */
    public long AsInt64(Object obj) {
      float fltItem = ((Float)obj).floatValue();
      if (Float.isNaN(fltItem)) {
        throw new ArithmeticException("This Object's value is out of range");
      }
      fltItem = (fltItem < 0) ? (float)Math.ceil(fltItem) : (float)Math.floor(fltItem);
      if (fltItem >= Long.MIN_VALUE && fltItem <= Long.MAX_VALUE) {
        return (long)fltItem;
      }
      throw new ArithmeticException("This Object's value is out of range");
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInSingle(Object obj) {
      return true;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInDouble(Object obj) {
      return true;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInInt32(Object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt32(obj);
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInInt64(Object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt64(obj);
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanTruncatedIntFitInInt64(Object obj) {
      float fltItem = ((Float)obj).floatValue();
      if (Float.isNaN(fltItem) || ((Float)(fltItem)).isInfinite()) {
        return false;
      }
      float fltItem2 = (fltItem < 0) ? (float)Math.ceil(fltItem) : (float)Math.floor(fltItem);
      return fltItem2 >= Long.MIN_VALUE && fltItem2 <= Long.MAX_VALUE;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanTruncatedIntFitInInt32(Object obj) {
      float fltItem = ((Float)obj).floatValue();
      if (Float.isNaN(fltItem) || ((Float)(fltItem)).isInfinite()) {
        return false;
      }
      float fltItem2 = (fltItem < 0) ? (float)Math.ceil(fltItem) : (float)Math.floor(fltItem);
      return fltItem2 >= Integer.MIN_VALUE && fltItem2 <= Integer.MAX_VALUE;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @param minValue A 32-bit signed integer. (2).
     * @param maxValue A 32-bit signed integer. (3).
     * @return A 32-bit signed integer.
     */
    public int AsInt32(Object obj, int minValue, int maxValue) {
      float fltItem = ((Float)obj).floatValue();
      if (Float.isNaN(fltItem)) {
        throw new ArithmeticException("This Object's value is out of range");
      }
      fltItem = (fltItem < 0) ? (float)Math.ceil(fltItem) : (float)Math.floor(fltItem);
      if (fltItem >= Integer.MIN_VALUE && fltItem <= Integer.MAX_VALUE) {
        int ret = (int)fltItem;
        return ret;
      }
      throw new ArithmeticException("This Object's value is out of range");
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsZero(Object obj) {
      return (((Float)obj).floatValue()) == 0.0f;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 32-bit signed integer.
     */
    public int Sign(Object obj) {
      float flt = ((Float)obj).floatValue();
      if (Float.isNaN(flt)) {
        return 2;
      }
      return flt == 0.0f ? 0 : (flt < 0.0f ? -1 : 1);
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsIntegral(Object obj) {
      float fltItem = ((Float)obj).floatValue();
      if (Float.isNaN(fltItem) || ((Float)(fltItem)).isInfinite()) {
        return false;
      }
      float fltItem2 = (fltItem < 0) ? (float)Math.ceil(fltItem) : (float)Math.floor(fltItem);
      return fltItem2 == fltItem;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object. (2).
     * @return An arbitrary object.
     */
    public Object Negate(Object obj) {
      float val = ((Float)obj).floatValue();
      return -val;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object. (2).
     * @return An arbitrary object.
     */
    public Object Abs(Object obj) {
      float val = ((Float)obj).floatValue();
      return (val < 0) ? -val : obj;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedRational object.
     */
public ExtendedRational AsExtendedRational(Object obj) {
      return ExtendedRational.FromSingle(((Float)obj).floatValue());
    }
  }
