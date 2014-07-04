package com.upokecenter.cbor;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import com.upokecenter.util.*;

  class CBORBigInteger implements ICBORNumber {

    public boolean IsPositiveInfinity(Object obj) {
      return false;
    }

    public boolean IsInfinity(Object obj) {
      return false;
    }

    public boolean IsNegativeInfinity(Object obj) {
      return false;
    }

    public boolean IsNaN(Object obj) {
      return false;
    }

    public double AsDouble(Object obj) {
      return ExtendedFloat.FromBigInteger((BigInteger)obj).ToDouble();
    }

    public ExtendedDecimal AsExtendedDecimal(Object obj) {
      return ExtendedDecimal.FromBigInteger((BigInteger)obj);
    }

    public ExtendedFloat AsExtendedFloat(Object obj) {
      return ExtendedFloat.FromBigInteger((BigInteger)obj);
    }

    public float AsSingle(Object obj) {
      return ExtendedFloat.FromBigInteger((BigInteger)obj).ToSingle();
    }

    public BigInteger AsBigInteger(Object obj) {
      return (BigInteger)obj;
    }

    public long AsInt64(Object obj) {
      BigInteger bi = (BigInteger)obj;
      if (bi.compareTo(CBORObject.Int64MaxValue) > 0 ||
          bi.compareTo(CBORObject.Int64MinValue) < 0) {
        throw new ArithmeticException("This Object's value is out of range");
      }
      return bi.longValue();
    }

    public boolean CanFitInSingle(Object obj) {
      BigInteger bigintItem = (BigInteger)obj;
      ExtendedFloat ef = ExtendedFloat.FromBigInteger(bigintItem);
      ExtendedFloat ef2 = ExtendedFloat.FromSingle(ef.ToSingle());
      return ef.compareTo(ef2) == 0;
    }

    public boolean CanFitInDouble(Object obj) {
      BigInteger bigintItem = (BigInteger)obj;
      ExtendedFloat ef = ExtendedFloat.FromBigInteger(bigintItem);
      ExtendedFloat ef2 = ExtendedFloat.FromDouble(ef.ToDouble());
      return ef.compareTo(ef2) == 0;
    }

    public boolean CanFitInInt32(Object obj) {
      BigInteger bi = (BigInteger)obj;
      return bi.canFitInInt();
    }

    public boolean CanFitInInt64(Object obj) {
      BigInteger bi = (BigInteger)obj;
      return bi.bitLength() <= 63;
    }

    public boolean CanTruncatedIntFitInInt64(Object obj) {
      return this.CanFitInInt64(obj);
    }

    public boolean CanTruncatedIntFitInInt32(Object obj) {
      return this.CanFitInInt32(obj);
    }

    public boolean IsZero(Object obj) {
      return ((BigInteger)obj).signum() == 0;
    }

    public int Sign(Object obj) {
      return ((BigInteger)obj).signum();
    }

    public boolean IsIntegral(Object obj) {
      return true;
    }

    public int AsInt32(Object obj, int minValue, int maxValue) {
      BigInteger bi = (BigInteger)obj;
      if (bi.canFitInInt()) {
        int ret = bi.intValue();
        if (ret >= minValue && ret <= maxValue) {
          return ret;
        }
      }
      throw new ArithmeticException("This Object's value is out of range");
    }

    public Object Negate(Object obj) {
      BigInteger bigobj = (BigInteger)obj;
      bigobj = (bigobj).negate();
      return bigobj;
    }

    public Object Abs(Object obj) {
      return ((BigInteger)obj).abs();
    }

    public ExtendedRational AsExtendedRational(Object obj) {
      return ExtendedRational.FromBigInteger((BigInteger)obj);
    }
  }
