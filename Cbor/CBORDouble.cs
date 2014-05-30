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
  internal class CBORDouble : ICBORNumber
  {
    public bool IsPositiveInfinity(object obj) {
      return Double.IsPositiveInfinity((double)obj);
    }

    public bool IsInfinity(object obj) {
      return Double.IsInfinity((double)obj);
    }

    public bool IsNegativeInfinity(object obj) {
      return Double.IsNegativeInfinity((double)obj);
    }

    public bool IsNaN(object obj) {
      return Double.IsNaN((double)obj);
    }

    public double AsDouble(object obj) {
      return (double)obj;
    }

    public ExtendedDecimal AsExtendedDecimal(object obj) {
      return ExtendedDecimal.FromDouble((double)obj);
    }

    public ExtendedFloat AsExtendedFloat(object obj) {
      return ExtendedFloat.FromDouble((double)obj);
    }

    /// <returns>A 32-bit floating-point number.</returns>
    /// <param name='obj'>An arbitrary object.</param>
    public float AsSingle(object obj) {
      return (float)(double)obj;
    }

    public BigInteger AsBigInteger(object obj) {
      return CBORUtilities.BigIntegerFromDouble((double)obj);
    }

    public long AsInt64(object obj) {
      double fltItem = (double)obj;
      if (Double.IsNaN(fltItem)) {
        throw new OverflowException("This object's value is out of range");
      }
      fltItem = (fltItem < 0) ? Math.Ceiling(fltItem) : Math.Floor(fltItem);
      if (fltItem >= Int64.MinValue && fltItem <= Int64.MaxValue) {
        return (long)fltItem;
      }
      throw new OverflowException("This object's value is out of range");
    }

    public bool CanFitInSingle(object obj) {
      double fltItem = (double)obj;
      if (Double.IsNaN(fltItem)) {
        return true;
      }
      float sing = (float)fltItem;
      return (double)sing == fltItem;
    }

    public bool CanFitInDouble(object obj) {
      return true;
    }

    public bool CanFitInInt32(object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt32(obj);
    }

    public bool CanFitInInt64(object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt64(obj);
    }

    public bool CanTruncatedIntFitInInt64(object obj) {
      double fltItem = (double)obj;
      if (Double.IsNaN(fltItem) || Double.IsInfinity(fltItem)) {
        return false;
      }
      double fltItem2 = (fltItem < 0) ? Math.Ceiling(fltItem) : Math.Floor(fltItem);
      return fltItem2 >= Int64.MinValue && fltItem2 <= Int64.MaxValue;
    }

    public bool CanTruncatedIntFitInInt32(object obj) {
      double fltItem = (double)obj;
      if (Double.IsNaN(fltItem) || Double.IsInfinity(fltItem)) {
        return false;
      }
      double fltItem2 = (fltItem < 0) ? Math.Ceiling(fltItem) : Math.Floor(fltItem);
      return fltItem2 >= Int32.MinValue && fltItem2 <= Int32.MaxValue;
    }

    public int AsInt32(object obj, int minValue, int maxValue) {
      double fltItem = (double)obj;
      if (Double.IsNaN(fltItem)) {
        throw new OverflowException("This object's value is out of range");
      }
      fltItem = (fltItem < 0) ? Math.Ceiling(fltItem) : Math.Floor(fltItem);
      if (fltItem >= minValue && fltItem <= maxValue) {
        int ret = (int)fltItem;
        return ret;
      }
      throw new OverflowException("This object's value is out of range");
    }

    public bool IsZero(object obj) {
      return ((double)obj) == 0.0;
    }

    public int Sign(object obj) {
      double flt = (double)obj;
      if (Double.IsNaN(flt)) {
        return 2;
      }
      return flt == 0.0f ? 0 : (flt < 0.0f ? -1 : 1);
    }

    public bool IsIntegral(object obj) {
      double fltItem = (double)obj;
      if (Double.IsNaN(fltItem) || Double.IsInfinity(fltItem)) {
        return false;
      }
      double fltItem2 = (fltItem < 0) ? Math.Ceiling(fltItem) : Math.Floor(fltItem);
      return fltItem2 == fltItem;
    }

    public object Negate(object obj) {
      double val = (double)obj;
      return -val;
    }

    public object Abs(object obj) {
      double val = (double)obj;
      return (val < 0) ? -val : obj;
    }

public ExtendedRational AsExtendedRational(object obj) {
      return ExtendedRational.FromDouble((double)obj);
    }
  }
}
