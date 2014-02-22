package com.upokecenter.util;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

  class CBORExtendedRational implements ICBORNumber
  {
    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsPositiveInfinity(Object obj) {
      return false;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsInfinity(Object obj) {
      return false;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsNegativeInfinity(Object obj) {
      return false;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsNaN(Object obj) {
      return false;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 64-bit floating-point number.
     */
    public double AsDouble(Object obj) {
      ExtendedRational er = (ExtendedRational)obj;
      return er.ToDouble();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedDecimal object.
     */
    public ExtendedDecimal AsExtendedDecimal(Object obj) {
      ExtendedRational er = (ExtendedRational)obj;
      return er.ToExtendedDecimalExactIfPossible(PrecisionContext.Decimal128.WithUnlimitedExponents());
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat AsExtendedFloat(Object obj) {
      ExtendedRational er = (ExtendedRational)obj;
      return er.ToExtendedFloatExactIfPossible(PrecisionContext.Binary128.WithUnlimitedExponents());
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 32-bit floating-point number.
     */
    public float AsSingle(Object obj) {
      ExtendedRational er = (ExtendedRational)obj;
      return er.ToSingle();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A BigInteger object.
     */
    public BigInteger AsBigInteger(Object obj) {
      ExtendedRational er = (ExtendedRational)obj;
      return er.ToBigInteger();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 64-bit signed integer.
     */
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

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInSingle(Object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      if (!ef.isFinite()) {
        return true;
      }
      return ef.compareTo(ExtendedRational.FromSingle(ef.ToSingle())) == 0;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInDouble(Object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      if (!ef.isFinite()) {
        return true;
      }
      return ef.compareTo(ExtendedRational.FromDouble(ef.ToDouble())) == 0;
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
      ExtendedRational ef = (ExtendedRational)obj;
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
      ExtendedRational ef = (ExtendedRational)obj;
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
      ExtendedRational ef = (ExtendedRational)obj;
      return ef.signum()==0;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 32-bit signed integer.
     */
    public int Sign(Object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      return ef.signum();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
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
      return rem.signum()==0;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInTypeZeroOrOne(Object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      if (!ef.isFinite()) {
        return false;
      }
      return ef.compareTo(ExtendedRational.FromBigInteger(CBORObject.LowestMajorType1)) >= 0 &&
        ef.compareTo(ExtendedRational.FromBigInteger(CBORObject.UInt64MaxValue)) <= 0;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @param minValue A 32-bit signed integer. (2).
     * @param maxValue A 32-bit signed integer. (3).
     * @return A 32-bit signed integer.
     */
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

    /**
     * Not documented yet.
     * @param obj An arbitrary object. (2).
     * @return An arbitrary object.
     */
    public Object Negate(Object obj) {
      ExtendedRational ed = (ExtendedRational)obj;
      return ed.Negate();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object. (2).
     * @return An arbitrary object.
     */
    public Object Abs(Object obj) {
      ExtendedRational ed = (ExtendedRational)obj;
      return ed.Abs();
    }
  }
