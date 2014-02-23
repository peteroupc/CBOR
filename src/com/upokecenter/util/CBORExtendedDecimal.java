package com.upokecenter.util;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

  class CBORExtendedDecimal implements ICBORNumber
  {
    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsPositiveInfinity(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.IsPositiveInfinity();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsInfinity(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.IsInfinity();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsNegativeInfinity(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.IsNegativeInfinity();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsNaN(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.IsNaN();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 64-bit floating-point number.
     */
    public double AsDouble(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.ToDouble();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedDecimal object.
     */
    public ExtendedDecimal AsExtendedDecimal(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat AsExtendedFloat(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.ToExtendedFloat();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 32-bit floating-point number.
     */
    public float AsSingle(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.ToSingle();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A BigInteger object.
     */
    public BigInteger AsBigInteger(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.ToBigInteger();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 64-bit signed integer.
     */
    public long AsInt64(Object obj) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
      if (ef.isFinite()) {
        BigInteger bi = ef.ToBigInteger();
        if (bi.bitLength() <= 63) {
          return bi.longValue();
        }
      }
      throw new ArithmeticException("This Object's value is out of range");
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInSingle(Object obj) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
      if (!ef.isFinite()) {
        return true;
      }
      return ef.compareTo(ExtendedDecimal.FromSingle(ef.ToSingle())) == 0;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInDouble(Object obj) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
      if (!ef.isFinite()) {
        return true;
      }
      return ef.compareTo(ExtendedDecimal.FromDouble(ef.ToDouble())) == 0;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInInt32(Object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt32(obj);
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInInt64(Object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt64(obj);
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanTruncatedIntFitInInt64(Object obj) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
      if (!ef.isFinite()) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.bitLength() <= 63;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanTruncatedIntFitInInt32(Object obj) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
      if (!ef.isFinite()) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.canFitInInt();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsZero(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.signum()==0;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 32-bit signed integer.
     */
    public int Sign(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      if (ed.IsNaN()) {
        return 2;
      }
      return ed.signum();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsIntegral(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      if (!ed.isFinite()) {
        return false;
      }
      if (ed.getExponent().signum() >= 0) {
        return true;
      }
      return ed.compareTo(ExtendedDecimal.FromBigInteger(ed.ToBigInteger())) == 0;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInTypeZeroOrOne(Object obj) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
      if (!ef.isFinite()) {
        return false;
      }
      return ef.compareTo(ExtendedDecimal.FromBigInteger(CBORObject.LowestMajorType1)) >= 0 &&
        ef.compareTo(ExtendedDecimal.FromBigInteger(CBORObject.UInt64MaxValue)) <= 0;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @param minValue A 32-bit signed integer. (2).
     * @param maxValue A 32-bit signed integer. (3).
     * @return A 32-bit signed integer.
     */
    public int AsInt32(Object obj, int minValue, int maxValue) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
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

    /**
     * Not documented yet.
     * @param obj An arbitrary object. (2).
     * @return An arbitrary object.
     */
    public Object Negate(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.Negate();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object. (2).
     * @return An arbitrary object.
     */
    public Object Abs(Object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.Abs();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedRational object.
     */
public ExtendedRational AsExtendedRational(Object obj) {
      return ExtendedRational.FromExtendedDecimal((ExtendedDecimal)obj);
    }
  }
