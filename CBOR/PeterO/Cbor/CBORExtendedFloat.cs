/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal class CBORExtendedFloat : ICBORNumber {
    public bool IsPositiveInfinity(object obj) {
      var ef = (EFloat)obj;
      return ef.IsPositiveInfinity();
    }

    public bool IsInfinity(object obj) {
      var ef = (EFloat)obj;
      return ef.IsInfinity();
    }

    public bool IsNegativeInfinity(object obj) {
      var ef = (EFloat)obj;
      return ef.IsNegativeInfinity();
    }

    public bool IsNaN(object obj) {
      var ef = (EFloat)obj;
      return ef.IsNaN();
    }

    public double AsDouble(object obj) {
      var ef = (EFloat)obj;
      return ef.ToDouble();
    }

    public EDecimal AsEDecimal(object obj) {
      var ef = (EFloat)obj;
      return ef.ToEDecimal();
    }

    public EFloat AsEFloat(object obj) {
      var ef = (EFloat)obj;
      return ef;
    }

    public float AsSingle(object obj) {
      var ef = (EFloat)obj;
      return ef.ToSingle();
    }

    public EInteger AsEInteger(object obj) {
      var ef = (EFloat)obj;
      return ef.ToEInteger();
    }

    public long AsInt64(object obj) {
      var ef = (EFloat)obj;
      if (this.CanTruncatedIntFitInInt64(obj)) {
        var bi = ef.ToEInteger();
        return (long)bi;
      }
      throw new OverflowException("This object's value is out of range");
    }

    public bool CanFitInSingle(object obj) {
      var ef = (EFloat)obj;
      return (!ef.IsFinite) || (ef.CompareTo(EFloat.FromSingle(
        ef.ToSingle())) == 0);
    }

    public bool CanFitInDouble(object obj) {
      var ef = (EFloat)obj;
      return (!ef.IsFinite) || (ef.CompareTo(EFloat.FromDouble(
        ef.ToDouble())) == 0);
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

    public bool CanTruncatedIntFitInInt64(object obj) {
      var ef = (EFloat)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.IsZero) {
        return true;
      }
      if (ef.Exponent.CompareTo((EInteger)65) >= 0) {
        return false;
      }
      var bi = ef.ToEInteger();
      return bi.CanFitInInt64();
    }

    public bool CanTruncatedIntFitInUInt64(object obj) {
      var ef = (EFloat)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.IsZero) {
        return true;
      }
      if (ef.Exponent.CompareTo((EInteger)65) >= 0) {
        return false;
      }
      var bi = ef.ToEInteger();
      return bi.Sign >= 0 && bi.GetUnsignedBitLengthAsInt64() <= 64;
    }

    public bool CanTruncatedIntFitInInt32(object obj) {
      var ef = (EFloat)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.IsZero) {
        return true;
      }
      if (ef.Exponent.CompareTo((EInteger)33) >= 0) {
        return false;
      }
      var bi = ef.ToEInteger();
      return bi.CanFitInInt32();
    }

    public bool IsNumberZero(object obj) {
      var ef = (EFloat)obj;
      return ef.IsZero;
    }

    public int Sign(object obj) {
      var ef = (EFloat)obj;
      return ef.IsNaN() ? 2 : ef.Sign;
    }

    public bool IsIntegral(object obj) {
      var ef = (EFloat)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.Exponent.Sign >= 0) {
        return true;
      }
      var ef2 = EFloat.FromEInteger(ef.ToEInteger());
      return ef2.CompareTo(ef) == 0;
    }

    public int AsInt32(object obj, int minValue, int maxValue) {
      var ef = (EFloat)obj;
      if (this.CanTruncatedIntFitInInt32(obj)) {
        var bi = ef.ToEInteger();
        var ret = (int)bi;
        if (ret >= minValue && ret <= maxValue) {
          return ret;
        }
      }
      throw new OverflowException("This object's value is out of range");
    }

    public object Negate(object obj) {
      var ed = (EFloat)obj;
      return ed.Negate();
    }

    public object Abs(object obj) {
      var ed = (EFloat)obj;
      return ed.Abs();
    }

    public ERational AsERational(object obj) {
      return ERational.FromEFloat((EFloat)obj);
    }

    public bool IsNegative(object obj) {
      return ((EFloat)obj).IsNegative;
    }
  }
}
