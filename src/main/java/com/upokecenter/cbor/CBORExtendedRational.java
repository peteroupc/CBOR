package com.upokecenter.cbor;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import com.upokecenter.util.*;

  class CBORExtendedRational implements ICBORNumber {

    public boolean IsPositiveInfinity(Object obj) {
      return ((ExtendedRational)obj).IsPositiveInfinity();
    }

    public boolean IsInfinity(Object obj) {
      return ((ExtendedRational)obj).IsInfinity();
    }

    public boolean IsNegativeInfinity(Object obj) {
      return ((ExtendedRational)obj).IsNegativeInfinity();
    }

    public boolean IsNaN(Object obj) {
      return ((ExtendedRational)obj).IsNaN();
    }

    public double AsDouble(Object obj) {
      ExtendedRational er = (ExtendedRational)obj;
      return er.ToDouble();
    }

    public ExtendedDecimal AsExtendedDecimal(Object obj) {
      ExtendedRational er = (ExtendedRational)obj;
      return

  er.ToExtendedDecimalExactIfPossible(PrecisionContext.Decimal128.WithUnlimitedExponents());
    }

    public ExtendedFloat AsExtendedFloat(Object obj) {
      ExtendedRational er = (ExtendedRational)obj;
      return

  er.ToExtendedFloatExactIfPossible(PrecisionContext.Binary128.WithUnlimitedExponents());
    }

    public float AsSingle(Object obj) {
      ExtendedRational er = (ExtendedRational)obj;
      return er.ToSingle();
    }

    public BigInteger AsBigInteger(Object obj) {
      ExtendedRational er = (ExtendedRational)obj;
      return er.ToBigInteger();
    }

    public long AsInt64(Object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      if (ef.isFinite()) {
        BigInteger bi = ef.ToBigInteger();
        if (bi.bitLength() <= 63) {
          return bi.longValue();
        }
      }
      throw new ArithmeticException("This Object's value is out of range");
    }

    public boolean CanFitInSingle(Object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      return (!ef.isFinite()) ||
      (ef.compareTo(ExtendedRational.FromSingle(ef.ToSingle())) == 0);
    }

    public boolean CanFitInDouble(Object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      return (!ef.isFinite()) ||
      (ef.compareTo(ExtendedRational.FromDouble(ef.ToDouble())) == 0);
    }

    public boolean CanFitInInt32(Object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt32(obj);
    }

    public boolean CanFitInInt64(Object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt64(obj);
    }

    public boolean CanTruncatedIntFitInInt64(Object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      if (!ef.isFinite()) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.bitLength() <= 63;
    }

    public boolean CanTruncatedIntFitInInt32(Object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      if (!ef.isFinite()) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.canFitInInt();
    }

    public boolean IsZero(Object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      return ef.signum() == 0;
    }

    public int Sign(Object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      return ef.signum();
    }

    public boolean IsIntegral(Object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      if (!ef.isFinite()) {
        return false;
      }
      if (ef.getDenominator().equals(BigInteger.ONE)) {
        return true;
      }
      // A rational number is integral if the remainder
      // of the numerator divided by the denominator is 0
      BigInteger denom = ef.getDenominator();
      BigInteger rem = ef.getNumerator().remainder(denom);
      return rem.signum() == 0;
    }

    public int AsInt32(Object obj, int minValue, int maxValue) {
      ExtendedRational ef = (ExtendedRational)obj;
      if (ef.isFinite()) {
        BigInteger bi = ef.ToBigInteger();
        if (bi.canFitInInt()) {
          int ret = bi.intValue();
          if (ret >= minValue && ret <= maxValue) {
            return ret;
          }
        }
      }
      throw new ArithmeticException("This Object's value is out of range");
    }

    public Object Negate(Object obj) {
      ExtendedRational ed = (ExtendedRational)obj;
      return ed.Negate();
    }

    public Object Abs(Object obj) {
      ExtendedRational ed = (ExtendedRational)obj;
      return ed.Abs();
    }

    public ExtendedRational AsExtendedRational(Object obj) {
      return (ExtendedRational)obj;
    }
  }
