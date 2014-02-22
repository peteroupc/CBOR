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
      throw new UnsupportedOperationException();  // TODO: Implement
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedDecimal object.
     */
    public ExtendedDecimal AsExtendedDecimal(Object obj) {
      throw new UnsupportedOperationException();  // TODO: Implement
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat AsExtendedFloat(Object obj) {
      throw new UnsupportedOperationException();  // TODO: Implement
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 32-bit floating-point number.
     */
    public float AsSingle(Object obj) {
      throw new UnsupportedOperationException();  // TODO: Implement
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A BigInteger object.
     */
    public BigInteger AsBigInteger(Object obj) {
      throw new UnsupportedOperationException();  // TODO: Implement
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 64-bit signed integer.
     */
    public long AsInt64(Object obj) {
      throw new UnsupportedOperationException();  // TODO: Implement
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInSingle(Object obj) {
      throw new UnsupportedOperationException();  // TODO: Implement
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInDouble(Object obj) {
      throw new UnsupportedOperationException();  // TODO: Implement
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInInt32(Object obj) {
      throw new UnsupportedOperationException();  // TODO: Implement
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanFitInInt64(Object obj) {
      throw new UnsupportedOperationException();  // TODO: Implement
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanTruncatedIntFitInInt64(Object obj) {
      throw new UnsupportedOperationException();  // TODO: Implement
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanTruncatedIntFitInInt32(Object obj) {
      throw new UnsupportedOperationException();  // TODO: Implement
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 32-bit signed integer.
     */
    public int AsInt32(Object obj) {
      throw new UnsupportedOperationException();  // TODO: Implement
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsZero(Object obj) {
      throw new UnsupportedOperationException();  // TODO: Implement
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 32-bit signed integer.
     */
    public int Sign(Object obj) {
      throw new UnsupportedOperationException();  // TODO: Implement
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
      throw new UnsupportedOperationException();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @param minValue A 32-bit signed integer. (2).
     * @param maxValue A 32-bit signed integer. (3).
     * @return A 32-bit signed integer.
     */
    public int AsInt32(Object obj, int minValue, int maxValue) {
      throw new UnsupportedOperationException();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object. (2).
     * @return An arbitrary object.
     */
public Object Negate(Object obj) {
      throw new UnsupportedOperationException();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object. (2).
     * @return An arbitrary object.
     */
public Object Abs(Object obj) {
      throw new UnsupportedOperationException();
    }
  }
