/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using PeterO;

namespace PeterO.Cbor {
  internal class CBORBigInteger : ICBORNumber
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
      return ExtendedFloat.FromBigInteger((BigInteger)obj).ToDouble();
    }

    public ExtendedDecimal AsExtendedDecimal(object obj) {
      return ExtendedDecimal.FromBigInteger((BigInteger)obj);
    }

    public ExtendedFloat AsExtendedFloat(object obj) {
      return ExtendedFloat.FromBigInteger((BigInteger)obj);
    }

    public float AsSingle(object obj) {
      return ExtendedFloat.FromBigInteger((BigInteger)obj).ToSingle();
    }

    public BigInteger AsBigInteger(object obj) {
      return (BigInteger)obj;
    }

    public long AsInt64(object obj) {
      var bi = (BigInteger)obj;
      if (bi.CompareTo(CBORObject.Int64MaxValue) > 0 ||
          bi.CompareTo(CBORObject.Int64MinValue) < 0) {
        throw new OverflowException("This object's value is out of range");
      }
      return (long)bi;
    }

    public bool CanFitInSingle(object obj) {
      var bigintItem = (BigInteger)obj;
      ExtendedFloat ef = ExtendedFloat.FromBigInteger(bigintItem);
      ExtendedFloat ef2 = ExtendedFloat.FromSingle(ef.ToSingle());
      return ef.CompareTo(ef2) == 0;
    }

    public bool CanFitInDouble(object obj) {
      var bigintItem = (BigInteger)obj;
      ExtendedFloat ef = ExtendedFloat.FromBigInteger(bigintItem);
      ExtendedFloat ef2 = ExtendedFloat.FromDouble(ef.ToDouble());
      return ef.CompareTo(ef2) == 0;
    }

    public bool CanFitInInt32(object obj) {
      var bi = (BigInteger)obj;
      return bi.canFitInInt();
    }

    public bool CanFitInInt64(object obj) {
      var bi = (BigInteger)obj;
      return bi.bitLength() <= 63;
    }

    public bool CanTruncatedIntFitInInt64(object obj) {
      return this.CanFitInInt64(obj);
    }

    public bool CanTruncatedIntFitInInt32(object obj) {
      return this.CanFitInInt32(obj);
    }

    public bool IsZero(object obj) {
      return ((BigInteger)obj).IsZero;
    }

    public int Sign(object obj) {
      return ((BigInteger)obj).Sign;
    }

    public bool IsIntegral(object obj) {
      return true;
    }

    public int AsInt32(object obj, int minValue, int maxValue) {
      var bi = (BigInteger)obj;
      if (bi.canFitInInt()) {
        var ret = (int)bi;
        if (ret >= minValue && ret <= maxValue) {
          return ret;
        }
      }
      throw new OverflowException("This object's value is out of range");
    }

    public object Negate(object obj) {
      var bigobj = (BigInteger)obj;
      bigobj = -(BigInteger)bigobj;
      return bigobj;
    }

    public object Abs(object obj) {
      return BigInteger.Abs((BigInteger)obj);
    }

    public ExtendedRational AsExtendedRational(object obj) {
      return ExtendedRational.FromBigInteger((BigInteger)obj);
    }
  }
}
