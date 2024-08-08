/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal class CBORExtendedRational : ICBORNumber
  {
    public bool IsPositiveInfinity(object obj) {
      return ((ERational)obj).IsPositiveInfinity();
    }

    public bool IsInfinity(object obj) {
      return ((ERational)obj).IsInfinity();
    }

    public bool IsNegativeInfinity(object obj) {
      return ((ERational)obj).IsNegativeInfinity();
    }

    public bool IsNaN(object obj) {
      return ((ERational)obj).IsNaN();
    }

    public double AsDouble(object obj) {
      var er = (ERational)obj;
      return er.ToDouble();
    }

    public EDecimal AsEDecimal(object obj) {
      var er = (ERational)obj;
      return

        er.ToEDecimalExactIfPossible(
          EContext.Decimal128.WithUnlimitedExponents());
    }

    public EFloat AsEFloat(object obj) {
      var er = (ERational)obj;
      return

        er.ToEFloatExactIfPossible(
          EContext.Binary128.WithUnlimitedExponents());
    }

    public float AsSingle(object obj) {
      var er = (ERational)obj;
      return er.ToSingle();
    }

    public EInteger AsEInteger(object obj) {
      var er = (ERational)obj;
      return er.ToEInteger();
    }

    public long AsInt64(object obj) {
      var ef = (ERational)obj;
      if (ef.IsFinite) {
        var bi = ef.ToEInteger();
        if (bi.CanFitInInt64()) {
          return (long)bi;
        }
      }
      throw new OverflowException("This object's value is out of range");
    }

    public bool CanFitInSingle(object obj) {
      var ef = (ERational)obj;
      return (!ef.IsFinite) || (ef.CompareTo(ERational.FromSingle(
            ef.ToSingle())) == 0);
    }

    public bool CanFitInDouble(object obj) {
      var ef = (ERational)obj;
      return (!ef.IsFinite) || (ef.CompareTo(ERational.FromDouble(
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
      var ef = (ERational)obj;
      if (!ef.IsFinite) {
        return false;
      }
      var bi = ef.ToEInteger();
      return bi.CanFitInInt64();
    }

    public bool CanTruncatedIntFitInInt32(object obj) {
      var ef = (ERational)obj;
      if (!ef.IsFinite) {
        return false;
      }
      var bi = ef.ToEInteger();
      return bi.CanFitInInt32();
    }

    public bool CanTruncatedIntFitInUInt64(object obj) {
      var ef = (ERational)obj;
      if (!ef.IsFinite) {
        return false;
      }
      var bi = ef.ToEInteger();
      return bi.Sign >= 0 && bi.GetUnsignedBitLengthAsInt64() <= 64;
    }

    public bool IsNumberZero(object obj) {
      var ef = (ERational)obj;
      return ef.IsZero;
    }

    public int Sign(object obj) {
      var ef = (ERational)obj;
      return ef.Sign;
    }

    public bool IsIntegral(object obj) {
      var ef = (ERational)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.Denominator.Equals(EInteger.One)) {
        return true;
      }
      // A rational number is integral if the remainder
      // of the numerator divided by the denominator is 0
      EInteger denom = ef.Denominator;
      EInteger rem = ef.Numerator % denom;
      return rem.IsZero;
    }

    public int AsInt32(object obj, int minValue, int maxValue) {
      var ef = (ERational)obj;
      if (ef.IsFinite) {
        var bi = ef.ToEInteger();
        if (bi.CanFitInInt32()) {
          var ret = (int)bi;
          if (ret >= minValue && ret <= maxValue) {
            return ret;
          }
        }
      }
      throw new OverflowException("This object's value is out of range");
    }

    public object Negate(object obj) {
      var ed = (ERational)obj;
      return ed.Negate();
    }

    public object Abs(object obj) {
      var ed = (ERational)obj;
      return ed.Abs();
    }

    public ERational AsERational(object obj) {
      return (ERational)obj;
    }

    public bool IsNegative(object obj) {
      return ((ERational)obj).IsNegative;
    }
  }
}
