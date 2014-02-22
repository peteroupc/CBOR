package com.upokecenter.util;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

  class CBORBigInteger implements ICBORNumber
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
      return ExtendedFloat.FromBigInteger((BigInteger)obj).ToDouble();
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
      return ExtendedFloat.FromBigInteger((BigInteger)obj);
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 32-bit floating-point number.
     */
    public float AsSingle(Object obj) {
      return ExtendedFloat.FromBigInteger((BigInteger)obj).ToSingle();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A BigInteger object.
     */
    public BigInteger AsBigInteger(Object obj) {
      return (BigInteger)obj;
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
      return this.CanFitInInt64(obj);
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean CanTruncatedIntFitInInt32(Object obj) {
      return this.CanFitInInt32(obj);
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
      return ((BigInteger)obj).signum()==0;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A 32-bit signed integer.
     */
    public int Sign(Object obj) {
      return ((BigInteger)obj).signum();
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @return A Boolean object.
     */
    public boolean IsIntegral(Object obj) {
      return true;
    }

    private static BigInteger valueLowestMajorType1 = BigInteger.ZERO .subtract(BigInteger.ONE.shiftLeft(64));

    private static BigInteger valueUInt64MaxValue = (BigInteger.ONE.shiftLeft(64)).subtract(BigInteger.ONE);

    public boolean CanFitInTypeZeroOrOne(Object obj) {
      BigInteger bigint = (BigInteger)obj;
      return bigint.compareTo(valueLowestMajorType1) >= 0 &&
        bigint.compareTo(valueUInt64MaxValue) <= 0;
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object.
     * @param minValue A 32-bit signed integer. (2).
     * @param maxValue A 32-bit signed integer. (3).
     * @return A 32-bit signed integer.
     */
    public int AsInt32(Object obj, int minValue, int maxValue) {
      throw new UnsupportedOperationException();  // TODO: Implement
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object. (2).
     * @return An arbitrary object.
     */
public Object Negate(Object obj) {
      return.subtract(BigInteger.valueOf(obj));
    }

    /**
     * Not documented yet.
     * @param obj An arbitrary object. (2).
     * @return An arbitrary object.
     */
public Object Abs(Object obj) {
      return ((BigInteger)obj).abs();
    }
  }
