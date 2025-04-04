/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal class CBORExtendedDecimal : ICBORNumber {
    public bool IsPositiveInfinity(object obj) {
      var ed = (EDecimal)obj;
      return ed.IsPositiveInfinity();
    }

    public bool IsInfinity(object obj) {
      var ed = (EDecimal)obj;
      return ed.IsInfinity();
    }

    public bool IsNegativeInfinity(object obj) {
      var ed = (EDecimal)obj;
      return ed.IsNegativeInfinity();
    }

    public bool IsNaN(object obj) {
      var ed = (EDecimal)obj;
      return ed.IsNaN();
    }

    public double AsDouble(object obj) {
      var ed = (EDecimal)obj;
      return ed.ToDouble();
    }

    public EDecimal AsEDecimal(object obj) {
      var ed = (EDecimal)obj;
      return ed;
    }

    public EFloat AsEFloat(object obj) {
      var ed = (EDecimal)obj;
      return ed.ToEFloat();
    }

    public float AsSingle(object obj) {
      var ed = (EDecimal)obj;
      return ed.ToSingle();
    }

    public EInteger AsEInteger(object obj) {
      var ed = (EDecimal)obj;
      return ed.ToEInteger();
    }

    public long AsInt64(object obj) {
      var ef = (EDecimal)obj;
      if (this.CanTruncatedIntFitInInt64(obj)) {
        var bi = ef.ToEInteger();
        return (long)bi;
      }
      throw new OverflowException("This object's value is out of range");
    }

    public bool CanFitInSingle(object obj) {
      var ef = (EDecimal)obj;
      return (!ef.IsFinite) || (ef.CompareTo(EDecimal.FromSingle(
        ef.ToSingle())) == 0);
    }

    public bool CanFitInDouble(object obj) {
      var ef = (EDecimal)obj;
      return (!ef.IsFinite) || (ef.CompareTo(EDecimal.FromDouble(
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
      var ef = (EDecimal)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.IsZero) {
        return true;
      }
      if (ef.Exponent.CompareTo((EInteger)21) >= 0) {
        return false;
      }
      var bi = ef.ToEInteger();
      return bi.CanFitInInt64();
    }

    public bool CanTruncatedIntFitInUInt64(object obj) {
      var ef = (EDecimal)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.IsZero) {
        return true;
      }
      if (ef.Exponent.CompareTo((EInteger)21) >= 0) {
        return false;
      }
      var bi = ef.ToEInteger();
      return bi.Sign >= 0 && bi.GetUnsignedBitLengthAsInt64() <= 64;
    }

    public bool CanTruncatedIntFitInInt32(object obj) {
      var ef = (EDecimal)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.IsZero) {
        return true;
      }
      if (ef.Exponent.CompareTo((EInteger)11) >= 0) {
        return false;
      }
      var bi = ef.ToEInteger();
      return bi.CanFitInInt32();
    }

    public bool IsNumberZero(object obj) {
      var ed = (EDecimal)obj;
      return ed.IsZero;
    }

    public int Sign(object obj) {
      var ed = (EDecimal)obj;
      return ed.IsNaN() ? 2 : ed.Sign;
    }

    public bool IsIntegral(object obj) {
      var ed = (EDecimal)obj;
      return ed.IsFinite && ((ed.Exponent.Sign >= 0) ||
        (ed.CompareTo(EDecimal.FromEInteger(ed.ToEInteger())) ==

        0));
    }

    public int AsInt32(object obj, int minValue, int maxValue) {
      var ef = (EDecimal)obj;
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
      var ed = (EDecimal)obj;
      return ed.Negate();
    }

    public object Abs(object obj) {
      var ed = (EDecimal)obj;
      return ed.Abs();
    }

    public ERational AsERational(object obj) {
      return ERational.FromEDecimal((EDecimal)obj);
    }

    public bool IsNegative(object obj) {
      return ((EDecimal)obj).IsNegative;
    }
  }
}
