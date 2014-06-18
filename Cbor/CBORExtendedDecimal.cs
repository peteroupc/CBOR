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
  internal class CBORExtendedDecimal : ICBORNumber
  {
    public bool IsPositiveInfinity(object obj) {
      var ed = (ExtendedDecimal)obj;
      return ed.IsPositiveInfinity();
    }

    public bool IsInfinity(object obj) {
      var ed = (ExtendedDecimal)obj;
      return ed.IsInfinity();
    }

    public bool IsNegativeInfinity(object obj) {
      var ed = (ExtendedDecimal)obj;
      return ed.IsNegativeInfinity();
    }

    public bool IsNaN(object obj) {
      var ed = (ExtendedDecimal)obj;
      return ed.IsNaN();
    }

    public double AsDouble(object obj) {
      var ed = (ExtendedDecimal)obj;
      return ed.ToDouble();
    }

    public ExtendedDecimal AsExtendedDecimal(object obj) {
      var ed = (ExtendedDecimal)obj;
      return ed;
    }

    public ExtendedFloat AsExtendedFloat(object obj) {
      var ed = (ExtendedDecimal)obj;
      return ed.ToExtendedFloat();
    }

    public float AsSingle(object obj) {
      var ed = (ExtendedDecimal)obj;
      return ed.ToSingle();
    }

    public BigInteger AsBigInteger(object obj) {
      var ed = (ExtendedDecimal)obj;
      return ed.ToBigInteger();
    }

    public long AsInt64(object obj) {
      var ef = (ExtendedDecimal)obj;
      if (this.CanTruncatedIntFitInInt64(obj)) {
        BigInteger bi = ef.ToBigInteger();
        return (long)bi;
      }
      throw new OverflowException("This object's value is out of range");
    }

    public bool CanFitInSingle(object obj) {
      var ef = (ExtendedDecimal)obj;
      return (!ef.IsFinite) || (ef.CompareTo(ExtendedDecimal.FromSingle(ef.ToSingle())) == 0);
    }

    public bool CanFitInDouble(object obj) {
      var ef = (ExtendedDecimal)obj;
      return (!ef.IsFinite) || (ef.CompareTo(ExtendedDecimal.FromDouble(ef.ToDouble())) == 0);
    }

    public bool CanFitInInt32(object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt32(obj);
    }

    public bool CanFitInInt64(object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt64(obj);
    }

    public bool CanTruncatedIntFitInInt64(object obj) {
      var ef = (ExtendedDecimal)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.IsZero) {
        return true;
      }
      if (ef.Exponent.CompareTo((BigInteger)21) >= 0) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.bitLength() <= 63;
    }

    public bool CanTruncatedIntFitInInt32(object obj) {
      var ef = (ExtendedDecimal)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.IsZero) {
        return true;
      }
      if (ef.Exponent.CompareTo((BigInteger)11) >= 0) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.canFitInInt();
    }

    public bool IsZero(object obj) {
      var ed = (ExtendedDecimal)obj;
      return ed.IsZero;
    }

    public int Sign(object obj) {
      var ed = (ExtendedDecimal)obj;
      return ed.IsNaN() ? 2 : ed.Sign;
    }

    public bool IsIntegral(object obj) {
      var ed = (ExtendedDecimal)obj;
      return ed.IsFinite && ((ed.Exponent.Sign >= 0) || (ed.CompareTo(ExtendedDecimal.FromBigInteger(ed.ToBigInteger())) == 0));
    }

    public int AsInt32(object obj, int minValue, int maxValue) {
      var ef = (ExtendedDecimal)obj;
      if (this.CanTruncatedIntFitInInt32(obj)) {
        BigInteger bi = ef.ToBigInteger();
        var ret = (int)bi;
        if (ret >= minValue && ret <= maxValue) {
          return ret;
        }
      }
      throw new OverflowException("This object's value is out of range");
    }

    public object Negate(object obj) {
      var ed = (ExtendedDecimal)obj;
      return ed.Negate();
    }

    public object Abs(object obj) {
      var ed = (ExtendedDecimal)obj;
      return ed.Abs();
    }

    public ExtendedRational AsExtendedRational(object obj) {
      return ExtendedRational.FromExtendedDecimal((ExtendedDecimal)obj);
    }
  }
}
