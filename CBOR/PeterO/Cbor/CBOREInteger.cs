/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal class CBOREInteger : ICBORNumber
  {
    public bool IsPositiveInfinity(object obj) {
      return false;
    }

    public bool IsInfinity(object obj) {
      return false;
    }

    public bool IsNegativeInfinity(object obj) {
      return false;
    }

    public bool IsNaN(object obj) {
      return false;
    }

    public double AsDouble(object obj) {
      return EFloat.FromEInteger((EInteger)obj).ToDouble();
    }

    public EDecimal AsEDecimal(object obj) {
      return EDecimal.FromEInteger((EInteger)obj);
    }

    public EFloat AsEFloat(object obj) {
      return EFloat.FromEInteger((EInteger)obj);
    }

    public float AsSingle(object obj) {
      return EFloat.FromEInteger((EInteger)obj).ToSingle();
    }

    public EInteger AsEInteger(object obj) {
      return (EInteger)obj;
    }

    public long AsInt64(object obj) {
      var bi = (EInteger)obj;
      return !bi.CanFitInInt64() ? throw new OverflowException("This" +
"\u0020object's value is out of range") : (long)bi;
    }

    public bool CanFitInSingle(object obj) {
      var bigintItem = (EInteger)obj;
      var ef = EFloat.FromEInteger(bigintItem);
      var ef2 = EFloat.FromSingle(ef.ToSingle());
      return ef.CompareTo(ef2) == 0;
    }

    public bool CanFitInDouble(object obj) {
      var bigintItem = (EInteger)obj;
      var ef = EFloat.FromEInteger(bigintItem);
      var ef2 = EFloat.FromDouble(ef.ToDouble());
      return ef.CompareTo(ef2) == 0;
    }

    public bool CanFitInInt32(object obj) {
      var bi = (EInteger)obj;
      return bi.CanFitInInt32();
    }

    public bool CanFitInInt64(object obj) {
      var bi = (EInteger)obj;
      return bi.CanFitInInt64();
    }

    public bool CanFitInUInt64(object obj) {
      var bi = (EInteger)obj;
      return bi.Sign >= 0 && bi.GetUnsignedBitLengthAsInt64() <= 64;
    }

    public bool CanTruncatedIntFitInInt64(object obj) {
      return this.CanFitInInt64(obj);
    }

    public bool CanTruncatedIntFitInUInt64(object obj) {
      return this.CanFitInUInt64(obj);
    }

    public bool CanTruncatedIntFitInInt32(object obj) {
      return this.CanFitInInt32(obj);
    }

    public bool IsNumberZero(object obj) {
      return ((EInteger)obj).IsZero;
    }

    public int Sign(object obj) {
      return ((EInteger)obj).Sign;
    }

    public bool IsIntegral(object obj) {
      return true;
    }

    public int AsInt32(object obj, int minValue, int maxValue) {
      var bi = (EInteger)obj;
      if (bi.CanFitInInt32()) {
        var ret = (int)bi;
        if (ret >= minValue && ret <= maxValue) {
          return ret;
        }
      }
      throw new OverflowException("This object's value is out of range");
    }

    public object Negate(object obj) {
      var bigobj = (EInteger)obj;
      bigobj = -bigobj;
      return bigobj;
    }

    public object Abs(object obj) {
      return ((EInteger)obj).Abs();
    }

    public ERational AsERational(object obj) {
      return ERational.FromEInteger((EInteger)obj);
    }

    public bool IsNegative(object obj) {
      return ((EInteger)obj).Sign < 0;
    }
  }
}
