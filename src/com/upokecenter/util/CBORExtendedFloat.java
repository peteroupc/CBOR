package com.upokecenter.util;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

  class CBORExtendedFloat implements ICBORNumber
  {
    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsPositiveInfinity(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.IsPositiveInfinity();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsInfinity(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.IsInfinity();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsNegativeInfinity(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.IsNegativeInfinity();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsNaN(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.IsNaN();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 64-bit floating-point number.
     */
    public double AsDouble(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.ToDouble();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedDecimal object.
     */
    public ExtendedDecimal AsExtendedDecimal(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.ToExtendedDecimal();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat AsExtendedFloat(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 32-bit floating-point number.
     */
    public float AsSingle(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.ToSingle();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A BigInteger object.
     */
    public BigInteger AsBigInteger(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.ToBigInteger();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 64-bit signed integer.
     */
    public long AsInt64(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      if (ef.isFinite()) {
        BigInteger bi = ef.ToBigInteger();
        if (bi.compareTo(CBORObject.Int64MaxValue) <= 0 &&
            bi.compareTo(CBORObject.Int64MinValue) >= 0) {
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
      ExtendedFloat ef = (ExtendedFloat)obj;
      if (!ef.isFinite()) {
        return true;
      }
      return ef.compareTo(ExtendedFloat.FromSingle(ef.ToSingle())) == 0;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInDouble(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      if (!ef.isFinite()) {
        return true;
      }
      return ef.compareTo(ExtendedFloat.FromDouble(ef.ToDouble())) == 0;
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
      ExtendedFloat ef = (ExtendedFloat)obj;
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
      ExtendedFloat ef = (ExtendedFloat)obj;
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
      ExtendedFloat ef = (ExtendedFloat)obj;
      return ef.signum()==0;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 32-bit signed integer.
     */
    public int Sign(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      if (ef.IsNaN()) {
 return 2;
}
      return ef.signum();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
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

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInTypeZeroOrOne(Object obj) {
      ExtendedFloat ef = (ExtendedFloat)obj;
      if (!ef.isFinite()) {
        return false;
      }
      return ef.compareTo(ExtendedFloat.FromBigInteger(CBORObject.LowestMajorType1)) >= 0 &&
        ef.compareTo(ExtendedFloat.FromBigInteger(CBORObject.UInt64MaxValue)) <= 0;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @param minValue A 32-bit signed integer. (2).
     * @param maxValue A 32-bit signed integer. (3).
     * @return A 32-bit signed integer.
     */
    public int AsInt32(Object obj, int minValue, int maxValue) {
      ExtendedFloat ef = (ExtendedFloat)obj;
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
      ExtendedFloat ed = (ExtendedFloat)obj;
      return ed.Negate();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object. (2).
     * @return An arbitrary object.
     */
    public Object Abs(Object obj) {
      ExtendedFloat ed = (ExtendedFloat)obj;
      return ed.Abs();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedRational object.
     */
public ExtendedRational AsExtendedRational(Object obj) {
      return ExtendedRational.FromExtendedFloat((ExtendedFloat)obj);
    }
  }
