/*
Written by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using PeterO;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal class CBORDoubleBits : ICBORNumber {
    public bool IsPositiveInfinity(object obj) {
       return ((long)obj) == (0x7ffL << 52);
    }

    public bool IsInfinity(object obj) {
       return (((long)obj) & ~(1L << 63)) == (0x7ffL << 52);
    }

    public bool IsNegativeInfinity(object obj) {
       return ((long)obj) == (0xfffL << 52);
    }

    public bool IsNaN(object obj) {
       return CBORUtilities.DoubleBitsNaN((long)obj);
    }

    public double AsDouble(object obj) {
       return CBORUtilities.Int64BitsToDouble((long)obj);
    }

    public EDecimal AsEDecimal(object obj) {
      // TODO: Use FromDoubleBits once available
      return EDecimal.FromDouble(this.AsDouble(obj));
    }

    public EFloat AsEFloat(object obj) {
      // TODO: Use FromDoubleBits once available
      return EFloat.FromDouble(this.AsDouble(obj));
    }

    public float AsSingle(object obj) {
       return CBORUtilities.Int32BitsToSingle(
         CBORUtilities.DoubleToRoundedSinglePrecision((long)obj));
    }

    public EInteger AsEInteger(object obj) {
      return CBORUtilities.BigIntegerFromDoubleBits((long)obj);
    }

    public long AsInt64(object obj) {
       throw new NotImplementedException();
    }

    public bool CanFitInSingle(object obj) {
       throw new NotImplementedException();
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
      if (this.IsNaN(obj) || this.IsInfinity(obj)) {
        return false;
      }
      throw new NotImplementedException();
    }

    public bool CanTruncatedIntFitInInt32(object obj) {
      if (this.IsNaN(obj) || this.IsInfinity(obj)) {
        return false;
      }
      throw new NotImplementedException();
    }

    public int AsInt32(object obj, int minValue, int maxValue) {
       throw new NotImplementedException();
    }

    public bool IsNumberZero(object obj) {
       return (((long)obj) & ~(1L << 63)) == 0;
    }

    public int Sign(object obj) {
return this.IsNaN(obj) ? (-2) : ((((long)obj) >> 63) != 0 ? -1 : 1);
    }

    public bool IsIntegral(object obj) {
      if (this.IsNaN(obj) || this.IsInfinity(obj)) {
        return false;
      }
      throw new NotImplementedException();
    }

    public object Negate(object obj) {
       throw new NotImplementedException();
    }

    public object Abs(object obj) {
       throw new NotImplementedException();
    }

    public ERational AsERational(object obj) {
      // TODO: Use FromDoubleBits once available
      return ERational.FromDouble(this.AsDouble(obj));
    }

    public bool IsNegative(object obj) {
       return (((long)obj) >> 63) != 0;
    }
  }
}
