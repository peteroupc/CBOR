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
  internal sealed class CBORSingle : ICBORNumber
  {
    public bool IsPositiveInfinity(object obj) {
      return Single.IsPositiveInfinity((float)obj);
    }

    public bool IsInfinity(object obj) {
      return Single.IsInfinity((float)obj);
    }

    public bool IsNegativeInfinity(object obj) {
      return Single.IsNegativeInfinity((float)obj);
    }

    public bool IsNaN(object obj) {
      return Single.IsNaN((float)obj);
    }

    public double AsDouble(object obj) {
      return (double)(float)obj;
    }

    public ExtendedDecimal AsExtendedDecimal(object obj) {
      return ExtendedDecimal.FromSingle((float)obj);
    }

    public ExtendedFloat AsExtendedFloat(object obj) {
      return ExtendedFloat.FromSingle((float)obj);
    }

    public float AsSingle(object obj) {
      return (float)obj;
    }

    public BigInteger AsBigInteger(object obj) {
      return CBORUtilities.BigIntegerFromSingle((float)obj);
    }

    public long AsInt64(object obj) {
      var fltItem = (float)obj;
      if (Single.IsNaN(fltItem)) {
        throw new OverflowException("This object's value is out of range");
      }
      fltItem = (fltItem < 0) ? (float)Math.Ceiling(fltItem) : (float)Math.Floor(fltItem);
      if (fltItem >= Int64.MinValue && fltItem <= Int64.MaxValue) {
        return (long)fltItem;
      }
      throw new OverflowException("This object's value is out of range");
    }

    public bool CanFitInSingle(object obj) {
      return true;
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
      var fltItem = (float)obj;
      if (Single.IsNaN(fltItem) || Single.IsInfinity(fltItem)) {
        return false;
      }
      float fltItem2 = (fltItem < 0) ? (float)Math.Ceiling(fltItem) : (float)Math.Floor(fltItem);
      return fltItem2 >= Int64.MinValue && fltItem2 <= Int64.MaxValue;
    }

    public bool CanTruncatedIntFitInInt32(object obj) {
      var fltItem = (float)obj;
      if (Single.IsNaN(fltItem) || Single.IsInfinity(fltItem)) {
        return false;
      }
      float fltItem2 = (fltItem < 0) ? (float)Math.Ceiling(fltItem) : (float)Math.Floor(fltItem);
      return fltItem2 >= Int32.MinValue && fltItem2 <= Int32.MaxValue;
    }

    public int AsInt32(object obj, int minValue, int maxValue) {
      var fltItem = (float)obj;
      if (Single.IsNaN(fltItem)) {
        throw new OverflowException("This object's value is out of range");
      }
      fltItem = (fltItem < 0) ? (float)Math.Ceiling(fltItem) : (float)Math.Floor(fltItem);
      if (fltItem >= Int32.MinValue && fltItem <= Int32.MaxValue) {
        var ret = (int)fltItem;
        return ret;
      }
      throw new OverflowException("This object's value is out of range");
    }

    public bool IsZero(object obj) {
      return (float)obj == 0.0f;
    }

    public int Sign(object obj) {
      var flt = (float)obj;
      return Single.IsNaN(flt) ? 2 : (flt == 0.0f ? 0 : (flt < 0.0f ? -1 : 1));
    }

    public bool IsIntegral(object obj) {
      var fltItem = (float)obj;
      if (Single.IsNaN(fltItem) || Single.IsInfinity(fltItem)) {
        return false;
      }
      float fltItem2 = (fltItem < 0) ? (float)Math.Ceiling(fltItem) : (float)Math.Floor(fltItem);
      return fltItem == fltItem2;
    }

    public object Negate(object obj) {
      var val = (float)obj;
      return -val;
    }

    public object Abs(object obj) {
      var val = (float)obj;
      return (val < 0) ? -val : obj;
    }

public ExtendedRational AsExtendedRational(object obj) {
      return ExtendedRational.FromSingle((float)obj);
    }
  }
}
