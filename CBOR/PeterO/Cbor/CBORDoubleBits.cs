/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal class CBORDoubleBits : ICBORNumber
  {
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
      return EDecimal.FromDoubleBits((long)obj);
    }

    public EFloat AsEFloat(object obj) {
      return EFloat.FromDoubleBits((long)obj);
    }

    public float AsSingle(object obj) {
      return CBORUtilities.Int32BitsToSingle(
        CBORUtilities.DoubleToRoundedSinglePrecision((long)obj));
    }

    public EInteger AsEInteger(object obj) {
      return CBORUtilities.EIntegerFromDoubleBits((long)obj);
    }

    public long AsInt64(object obj) {
      if (this.IsNaN(obj) || this.IsInfinity(obj)) {
        throw new OverflowException("This object's value is out of range");
      }
      long b = DoubleBitsRoundDown((long)obj);
      bool neg = (b >> 63) != 0;
      b &= ~(1L << 63);
      if (b == 0) {
        return 0;
      }
      if (neg && b == (0x43eL << 52)) {
        return Int64.MinValue;
      }
      if ((b >> 52) >= 0x43e) {
        throw new OverflowException("This object's value is out of range");
      }
      var exp = (int)(b >> 52);
      long mant = b & ((1L << 52) - 1);
      mant |= 1L << 52;
      int shift = 52 - (exp - 0x3ff);
      if (shift < 0) {
        mant <<= -shift;
      } else {
        mant >>= shift;
      }
      if (neg) {
        mant = -mant;
      }
      return mant;
    }

    public bool CanFitInSingle(object obj) {
      return this.IsNaN(obj) ||
CBORUtilities.DoubleRetainsSameValueInSingle((long)obj);
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

    public bool CanFitInUInt64(object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInUInt64(obj);
    }

    private static long DoubleBitsRoundDown(long bits) {
      long origbits = bits;
      bits &= ~(1L << 63);
      if (bits == 0) {
        return origbits;
      }
      // Infinity and NaN
      if (bits >= unchecked(0x7ffL << 52)) {
        return origbits;
      }
      // Beyond non-integer range
      if ((bits >> 52) >= 0x433) {
        return origbits;
      }
      // Less than 1
      if ((bits >> 52) <= 0x3fe) {
        return (origbits >> 63) != 0 ? (1L << 63) : 0;
      }
      var exp = (int)(bits >> 52);
      long mant = bits & ((1L << 52) - 1);
      int shift = 52 - (exp - 0x3ff);
      return ((mant >> shift) << shift) | (origbits & (0xfffL << 52));
    }

    public bool CanTruncatedIntFitInInt64(object obj) {
      if (this.IsNaN(obj) || this.IsInfinity(obj)) {
        return false;
      }
      long b = DoubleBitsRoundDown((long)obj);
      bool neg = (b >> 63) != 0;
      b &= ~(1L << 63);
      return (neg && b == (0x43eL << 52)) || ((b >> 52) < 0x43e);
    }

    public bool CanTruncatedIntFitInUInt64(object obj) {
      if (this.IsNaN(obj) || this.IsInfinity(obj)) {
        return false;
      }
      long b = DoubleBitsRoundDown((long)obj);
      bool neg = (b >> 63) != 0;
      b &= ~(1L << 63);
      return (neg && b == 0) || (!neg && (b >> 52) < 0x43f);
    }

    public bool CanTruncatedIntFitInInt32(object obj) {
      if (this.IsNaN(obj) || this.IsInfinity(obj)) {
        return false;
      }
      long b = DoubleBitsRoundDown((long)obj);
      bool neg = (b >> 63) != 0;
      b &= ~(1L << 63);
      return (neg && b == (0x41eL << 52)) || ((b >> 52) < 0x41e);
    }

    public int AsInt32(object obj, int minValue, int maxValue) {
      if (this.IsNaN(obj) || this.IsInfinity(obj)) {
        throw new OverflowException("This object's value is out of range");
      }
      long b = DoubleBitsRoundDown((long)obj);
      bool neg = (b >> 63) != 0;
      b &= ~(1L << 63);
      if (b == 0) {
        return 0;
      }
      // Beyond non-integer range (thus beyond int32 range)
      if ((b >> 52) >= 0x433) {
        throw new OverflowException("This object's value is out of range");
      }
      var exp = (int)(b >> 52);
      long mant = b & ((1L << 52) - 1);
      mant |= 1L << 52;
      int shift = 52 - (exp - 0x3ff);
      mant >>= shift;
      if (neg) {
        mant = -mant;
      }
      return mant < minValue || mant > maxValue ? throw new
OverflowException("This object's value is out of range") : (int)mant;
    }

    public bool IsNumberZero(object obj) {
      return (((long)obj) & ~(1L << 63)) == 0;
    }

    public int Sign(object obj) {
      return this.IsNaN(obj) ? (-2) : ((((long)obj) >> 63) != 0 ? -1 : 1);
    }

    public bool IsIntegral(object obj) {
      return CBORUtilities.IsIntegerValue((long)obj);
    }

    public object Negate(object obj) {
      return ((long)obj) ^ (1L << 63);
    }

    public object Abs(object obj) {
      return ((long)obj) & ~(1L << 63);
    }

    public ERational AsERational(object obj) {
      return ERational.FromDoubleBits((long)obj);
    }

    public bool IsNegative(object obj) {
      return (((long)obj) >> 63) != 0;
    }
  }
}
