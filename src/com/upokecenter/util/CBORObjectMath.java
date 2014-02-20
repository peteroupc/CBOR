package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

    /**
     * Implements arithmetic operations with CBOR objects.
     * @param a A CBORObject object. (2).
     * @param b A CBORObject object. (3).
     * @return A CBORObject object.
     */
  final class CBORObjectMath {
private CBORObjectMath() {
}
    public static CBORObject Addition(CBORObject a, CBORObject b) {
      if (a == null) {
        throw new NullPointerException("a");
      }
      if (b == null) {
        throw new NullPointerException("b");
      }
      int combo = (a.getItemType() << 4) | b.getItemType();
      BigInteger bvalueA;
      BigInteger bvalueB;
      switch (combo) {
          case (CBORObject.CBORObjectTypeInteger << 4) | CBORObject.CBORObjectTypeInteger: {
            long valueA = (((Long)a.getThisItem()).longValue());
            long valueB = (((Long)b.getThisItem()).longValue());
            if ((valueA < 0 && valueB < Long.MIN_VALUE - valueA) ||
                (valueA > 0 && valueB > Long.MAX_VALUE - valueA)) {
              // would overflow, convert to BigInteger
              return CBORObject.FromObject((BigInteger.valueOf(valueA)).add(BigInteger.valueOf(valueB)));
            }
            return CBORObject.FromObject(valueA + valueB);
          }
          case (CBORObject.CBORObjectTypeInteger << 4) | CBORObject.CBORObjectTypeBigInteger: {
            bvalueA = BigInteger.valueOf(((Long)a.getThisItem()).longValue());
            bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.add(bvalueB));
          }
          case (CBORObject.CBORObjectTypeBigInteger << 4) | CBORObject.CBORObjectTypeInteger: {
            bvalueA = (BigInteger)a.getThisItem();
            bvalueB = BigInteger.valueOf(((Long)b.getThisItem()).longValue());
            return CBORObject.FromObject(bvalueA.add(bvalueB));
          }
          case (CBORObject.CBORObjectTypeBigInteger << 4) | CBORObject.CBORObjectTypeBigInteger: {
            bvalueA = (BigInteger)a.getThisItem();
            bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.add(bvalueB));
          }
        default:
          return CBORObject.FromObject(a.AsExtendedDecimal().Add(b.AsExtendedDecimal()));
      }
    }

    public static CBORObject Subtract(CBORObject a, CBORObject b) {
      if (a == null) {
        throw new NullPointerException("a");
      }
      if (b == null) {
        throw new NullPointerException("b");
      }
      int combo = (a.getItemType() << 4) | b.getItemType();
      BigInteger bvalueA;
      BigInteger bvalueB;
      switch (combo) {
          case (CBORObject.CBORObjectTypeInteger << 4) | CBORObject.CBORObjectTypeInteger: {
            long valueA = (((Long)a.getThisItem()).longValue());
            long valueB = (((Long)b.getThisItem()).longValue());
            if ((valueB < 0 && Long.MAX_VALUE + valueB < valueA) ||
                (valueB > 0 && Long.MIN_VALUE + valueB > valueA)) {
              // would overflow, convert to BigInteger
              return CBORObject.FromObject((BigInteger.valueOf(valueA)).subtract(BigInteger.valueOf(valueB)));
            }
            return CBORObject.FromObject(valueA - valueB);
          }
          case (CBORObject.CBORObjectTypeInteger << 4) | CBORObject.CBORObjectTypeBigInteger: {
            bvalueA = BigInteger.valueOf(((Long)a.getThisItem()).longValue());
            bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.subtract(bvalueB));
          }
          case (CBORObject.CBORObjectTypeBigInteger << 4) | CBORObject.CBORObjectTypeInteger: {
            bvalueA = (BigInteger)a.getThisItem();
            bvalueB = BigInteger.valueOf(((Long)b.getThisItem()).longValue());
            return CBORObject.FromObject(bvalueA.subtract(bvalueB));
          }
          case (CBORObject.CBORObjectTypeBigInteger << 4) | CBORObject.CBORObjectTypeBigInteger: {
            bvalueA = (BigInteger)a.getThisItem();
            bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.subtract(bvalueB));
          }
        default:
          return CBORObject.FromObject(a.AsExtendedDecimal().Subtract(b.AsExtendedDecimal()));
      }
    }

    public static CBORObject Multiply(CBORObject a, CBORObject b) {
      if (a == null) {
        throw new NullPointerException("a");
      }
      if (b == null) {
        throw new NullPointerException("b");
      }
      int combo = (a.getItemType() << 4) | b.getItemType();
      BigInteger bvalueA;
      BigInteger bvalueB;
      switch (combo) {
          case (CBORObject.CBORObjectTypeInteger << 4) | CBORObject.CBORObjectTypeInteger: {
            long valueA = (((Long)a.getThisItem()).longValue());
            long valueB = (((Long)b.getThisItem()).longValue());
            boolean apos = valueA > 0L;
            boolean bpos = valueB > 0L;
            if (
              (apos && ((!bpos && (Long.MIN_VALUE / valueA) > valueB) ||
                        (bpos && valueA > (Long.MAX_VALUE / valueB)))) ||
              (!apos && ((!bpos && valueA != 0L &&
                          (Long.MAX_VALUE / valueA) > valueB) ||
                         (bpos && valueA < (Long.MIN_VALUE / valueB))))) {
              // would overflow, convert to BigInteger
              bvalueA = BigInteger.valueOf(valueA);
              bvalueB = BigInteger.valueOf(valueB);
              return CBORObject.FromObject(bvalueA.multiply(bvalueB));
            }
            return CBORObject.FromObject(valueA * valueB);
          }
          case (CBORObject.CBORObjectTypeInteger << 4) | CBORObject.CBORObjectTypeBigInteger: {
            bvalueA = BigInteger.valueOf(((Long)a.getThisItem()).longValue());
            bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.multiply(bvalueB));
          }
          case (CBORObject.CBORObjectTypeBigInteger << 4) | CBORObject.CBORObjectTypeInteger: {
            bvalueA = (BigInteger)a.getThisItem();
            bvalueB = BigInteger.valueOf(((Long)b.getThisItem()).longValue());
            return CBORObject.FromObject(bvalueA.multiply(bvalueB));
          }
          case (CBORObject.CBORObjectTypeBigInteger << 4) | CBORObject.CBORObjectTypeBigInteger: {
            bvalueA = (BigInteger)a.getThisItem();
            bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.multiply(bvalueB));
          }
        default:
          return CBORObject.FromObject(a.AsExtendedDecimal().Multiply(b.AsExtendedDecimal()));
      }
    }
  }
