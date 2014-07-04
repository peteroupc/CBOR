package com.upokecenter.cbor;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import com.upokecenter.util.*;

  class CBORExtendedFloat implements ICBORNumber {

    public boolean IsPositiveInfinity(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.IsPositiveInfinity();
    }

    public boolean IsInfinity(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.IsInfinity();
    }

    public boolean IsNegativeInfinity(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.IsNegativeInfinity();
    }

    public boolean IsNaN(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.IsNaN();
    }

    public double AsDouble(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.ToDouble();
    }

    public ExtendedDecimal AsExtendedDecimal(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.ToExtendedDecimal();
    }

    public ExtendedFloat AsExtendedFloat(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef;
    }

    public float AsSingle(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.ToSingle();
    }

    public BigInteger AsBigInteger(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.ToBigInteger();
    }

    public long AsInt64(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      if (this.CanTruncatedIntFitInInt64(obj)) {
        BigInteger bi = ef.ToBigInteger();
        return bi.longValue();
      }
      throw new ArithmeticException("This Object's value is out of range");
    }

    public boolean CanFitInSingle(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return (!ef.isFinite()) ||
      (ef.compareTo(ExtendedFloat.FromSingle(ef.ToSingle())) == 0);
    }

    public boolean CanFitInDouble(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return (!ef.isFinite()) ||
      (ef.compareTo(ExtendedFloat.FromDouble(ef.ToDouble())) == 0);
    }

    public boolean CanFitInInt32(Object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt32(obj);
    }

    public boolean CanFitInInt64(Object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt64(obj);
    }

    public boolean CanTruncatedIntFitInInt64(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      if (!ef.isFinite()) {
        return false;
      }
      if (ef.signum() == 0) {
        return true;
      }
      if (ef.getExponent().compareTo(BigInteger.valueOf(65)) >= 0) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.bitLength() <= 63;
    }

    public boolean CanTruncatedIntFitInInt32(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      if (!ef.isFinite()) {
        return false;
      }
      if (ef.signum() == 0) {
        return true;
      }
      if (ef.getExponent().compareTo(BigInteger.valueOf(33)) >= 0) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.canFitInInt();
    }

    public boolean IsZero(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.signum() == 0;
    }

    public int Sign(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.IsNaN() ? 2 : ef.signum();
    }

    public boolean IsIntegral(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      if (!ef.isFinite()) {
        return false;
      }
      if (ef.getExponent().signum() >= 0) {
        return true;
      }
      ExtendedFloat ef2 = ExtendedFloat.FromBigInteger(ef.ToBigInteger());
      return ef2.compareTo(ef) == 0;
    }

    public int AsInt32(Object obj, int minValue, int maxValue) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      if (this.CanTruncatedIntFitInInt32(obj)) {
        BigInteger bi = ef.ToBigInteger();
        int ret = bi.intValue();
        if (ret >= minValue && ret <= maxValue) {
          return ret;
        }
      }
      throw new ArithmeticException("This Object's value is out of range");
    }

    public Object Negate(Object obj) {
      ExtendedFloat ed = (ExtendedFloat)obj;
      return ed.Negate();
    }

    public Object Abs(Object obj) {
      ExtendedFloat ed = (ExtendedFloat)obj;
      return ed.Abs();
    }

    public ExtendedRational AsExtendedRational(Object obj) {
      return ExtendedRational.FromExtendedFloat((ExtendedFloat)obj);
    }
  }
