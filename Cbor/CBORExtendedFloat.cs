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
  internal class CBORExtendedFloat : ICBORNumber
  {
    public bool IsPositiveInfinity(object obj) {
      var ef = (ExtendedFloat)obj;
      return ef.IsPositiveInfinity();
    }

    public bool IsInfinity(object obj) {
      var ef = (ExtendedFloat)obj;
      return ef.IsInfinity();
    }

    public bool IsNegativeInfinity(object obj) {
      var ef = (ExtendedFloat)obj;
      return ef.IsNegativeInfinity();
    }

    public bool IsNaN(object obj) {
      var ef = (ExtendedFloat)obj;
      return ef.IsNaN();
    }

    public double AsDouble(object obj) {
      var ef = (ExtendedFloat)obj;
      return ef.ToDouble();
    }

    public ExtendedDecimal AsExtendedDecimal(object obj) {
      var ef = (ExtendedFloat)obj;
      return ef.ToExtendedDecimal();
    }

    public ExtendedFloat AsExtendedFloat(object obj) {
      var ef = (ExtendedFloat)obj;
      return ef;
    }

    public float AsSingle(object obj) {
      var ef = (ExtendedFloat)obj;
      return ef.ToSingle();
    }

    public BigInteger AsBigInteger(object obj) {
      var ef = (ExtendedFloat)obj;
      return ef.ToBigInteger();
    }

    public long AsInt64(object obj) {
      var ef = (ExtendedFloat)obj;
      if (this.CanTruncatedIntFitInInt64(obj)) {
        BigInteger bi = ef.ToBigInteger();
        return (long)bi;
      }
      throw new OverflowException("This object's value is out of range");
    }

    public bool CanFitInSingle(object obj) {
      var ef = (ExtendedFloat)obj;
      if (!ef.IsFinite) {
        return true;
      }
      return ef.CompareTo(ExtendedFloat.FromSingle(ef.ToSingle())) == 0;
    }

    public bool CanFitInDouble(object obj) {
      var ef = (ExtendedFloat)obj;
      if (!ef.IsFinite) {
        return true;
      }
      return ef.CompareTo(ExtendedFloat.FromDouble(ef.ToDouble())) == 0;
    }

    public bool CanFitInInt32(object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt32(obj);
    }

    public bool CanFitInInt64(object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt64(obj);
    }

    public bool CanTruncatedIntFitInInt64(object obj) {
      var ef = (ExtendedFloat)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.IsZero) {
        return true;
      }
      if (ef.Exponent.CompareTo((BigInteger)65) >= 0) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.bitLength() <= 63;
    }

    public bool CanTruncatedIntFitInInt32(object obj) {
      var ef = (ExtendedFloat)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.IsZero) {
        return true;
      }
      if (ef.Exponent.CompareTo((BigInteger)33) >= 0) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.canFitInInt();
    }

    public bool IsZero(object obj) {
      var ef = (ExtendedFloat)obj;
      return ef.IsZero;
    }

    public int Sign(object obj) {
      var ef = (ExtendedFloat)obj;
      if (ef.IsNaN()) {
        return 2;
      }
      return ef.Sign;
    }

    public bool IsIntegral(object obj) {
      var ef = (ExtendedFloat)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.Exponent.Sign >= 0) {
        return true;
      }
      ExtendedFloat ef2 = ExtendedFloat.FromBigInteger(ef.ToBigInteger());
      return ef2.CompareTo(ef) == 0;
    }

    public int AsInt32(object obj, int minValue, int maxValue) {
      var ef = (ExtendedFloat)obj;
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
      var ed = (ExtendedFloat)obj;
      return ed.Negate();
    }

    public object Abs(object obj) {
      var ed = (ExtendedFloat)obj;
      return ed.Abs();
    }

    public ExtendedRational AsExtendedRational(object obj) {
      return ExtendedRational.FromExtendedFloat((ExtendedFloat)obj);
    }
  }
}
