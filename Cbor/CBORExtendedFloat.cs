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
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.IsPositiveInfinity();
    }

    public bool IsInfinity(object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.IsInfinity();
    }

    public bool IsNegativeInfinity(object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.IsNegativeInfinity();
    }

    public bool IsNaN(object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.IsNaN();
    }

    /// <returns>A 64-bit floating-point number.</returns>
    /// <param name='obj'>An arbitrary object.</param>
 /// <summary>Not documented yet.</summary>
    public double AsDouble(object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.ToDouble();
    }

    public ExtendedDecimal AsExtendedDecimal(object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.ToExtendedDecimal();
    }

    public ExtendedFloat AsExtendedFloat(object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef;
    }

    /// <returns>A 32-bit floating-point number.</returns>
    /// <param name='obj'>An arbitrary object.</param>
 /// <summary>Not documented yet.</summary>
    public float AsSingle(object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.ToSingle();
    }

    public BigInteger AsBigInteger(object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.ToBigInteger();
    }

    public long AsInt64(object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      if (this.CanTruncatedIntFitInInt64(obj)) {
        BigInteger bi = ef.ToBigInteger();
        return (long)bi;
      }
      throw new OverflowException("This object's value is out of range");
    }

    public bool CanFitInSingle(object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      if (!ef.IsFinite) {
        return true;
      }
      return ef.CompareTo(ExtendedFloat.FromSingle(ef.ToSingle())) == 0;
    }

    public bool CanFitInDouble(object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
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
      ExtendedFloat ef = (ExtendedFloat)obj;
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
      ExtendedFloat ef = (ExtendedFloat)obj;
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
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.IsZero;
    }

    public int Sign(object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      if (ef.IsNaN()) {
        return 2;
      }
      return ef.Sign;
    }

    public bool IsIntegral(object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
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
      ExtendedFloat ef = (ExtendedFloat)obj;
      if (this.CanTruncatedIntFitInInt32(obj)) {
        BigInteger bi = ef.ToBigInteger();
        int ret = (int)bi;
        if (ret >= minValue && ret <= maxValue) {
          return ret;
        }
      }
      throw new OverflowException("This object's value is out of range");
    }

    public object Negate(object obj) {
      ExtendedFloat ed = (ExtendedFloat)obj;
      return ed.Negate();
    }

    public object Abs(object obj) {
      ExtendedFloat ed = (ExtendedFloat)obj;
      return ed.Abs();
    }

    public ExtendedRational AsExtendedRational(object obj) {
      return ExtendedRational.FromExtendedFloat((ExtendedFloat)obj);
    }
  }
}
