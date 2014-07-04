package com.upokecenter.cbor;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import com.upokecenter.util.*;

  class CBORExtendedDecimal implements ICBORNumber {

    public boolean IsPositiveInfinity(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.IsPositiveInfinity();
    }

    public boolean IsInfinity(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.IsInfinity();
    }

    public boolean IsNegativeInfinity(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.IsNegativeInfinity();
    }

    public boolean IsNaN(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.IsNaN();
    }

    public double AsDouble(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.ToDouble();
    }

    public ExtendedDecimal AsExtendedDecimal(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed;
    }

    public ExtendedFloat AsExtendedFloat(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.ToExtendedFloat();
    }

    public float AsSingle(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.ToSingle();
    }

    public BigInteger AsBigInteger(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.ToBigInteger();
    }

    public long AsInt64(Object obj) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
      if (this.CanTruncatedIntFitInInt64(obj)) {
        BigInteger bi = ef.ToBigInteger();
        return bi.longValue();
      }
      throw new ArithmeticException("This Object's value is out of range");
    }

    public boolean CanFitInSingle(Object obj) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
      return (!ef.isFinite()) ||
      (ef.compareTo(ExtendedDecimal.FromSingle(ef.ToSingle())) == 0);
    }

    public boolean CanFitInDouble(Object obj) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
      return (!ef.isFinite()) ||
      (ef.compareTo(ExtendedDecimal.FromDouble(ef.ToDouble())) == 0);
    }

    public boolean CanFitInInt32(Object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt32(obj);
    }

    public boolean CanFitInInt64(Object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt64(obj);
    }

    public boolean CanTruncatedIntFitInInt64(Object obj) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
      if (!ef.isFinite()) {
        return false;
      }
      if (ef.signum() == 0) {
        return true;
      }
      if (ef.getExponent().compareTo(BigInteger.valueOf(21)) >= 0) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.bitLength() <= 63;
    }

    public boolean CanTruncatedIntFitInInt32(Object obj) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
      if (!ef.isFinite()) {
        return false;
      }
      if (ef.signum() == 0) {
        return true;
      }
      if (ef.getExponent().compareTo(BigInteger.valueOf(11)) >= 0) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.canFitInInt();
    }

    public boolean IsZero(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.signum() == 0;
    }

    public int Sign(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.IsNaN() ? 2 : ed.signum();
    }

    public boolean IsIntegral(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.isFinite() && ((ed.getExponent().signum() >= 0) ||
      (ed.compareTo(ExtendedDecimal.FromBigInteger(ed.ToBigInteger())) ==
      0));
    }

    public int AsInt32(Object obj, int minValue, int maxValue) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
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
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.Negate();
    }

    public Object Abs(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.Abs();
    }

    public ExtendedRational AsExtendedRational(Object obj) {
      return ExtendedRational.FromExtendedDecimal((ExtendedDecimal)obj);
    }
  }
