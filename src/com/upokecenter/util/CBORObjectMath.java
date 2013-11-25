package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.math.*;

    /**
     * Implements arithmetic operations with CBOR objects.
     */
  final class CBORObjectMath {
private CBORObjectMath(){}
    private static CBORObject NaN = CBORObject.FromObject(Double.NaN);
    /**
     * 
     * @param a A CBORObject object.
     * @param b A CBORObject object.
     */
    public static CBORObject Addition(CBORObject a, CBORObject b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      int combo = (a.getItemType() << 4) | b.getItemType();
      switch (combo) {
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Integer: {
            long valueA = (((Long)a.getThisItem()).longValue());
            long valueB = (((Long)b.getThisItem()).longValue());
            if ((valueA < 0 && valueB < Long.MIN_VALUE - valueA) ||
                (valueA > 0 && valueB > Long.MAX_VALUE - valueA)) {
              // would overflow, convert to BigInteger
              return CBORObject.FromObject((BigInteger.valueOf(valueA)).add(BigInteger.valueOf(valueB)));
            }
            return CBORObject.FromObject(valueA + valueB);
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigInteger bvalueA = BigInteger.valueOf(((Long)a.getThisItem()).longValue());
            BigInteger bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.add(bvalueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((((Long)a.getThisItem()).longValue()));
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((((Long)a.getThisItem()).longValue()));
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = new DecimalFraction((((Long)a.getThisItem()).longValue()));
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = new BigFloat((((Long)a.getThisItem()).longValue()));
            BigFloat valueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Integer: {
            BigInteger bvalueA = (BigInteger)a.getThisItem();
            BigInteger bvalueB = BigInteger.valueOf(((Long)b.getThisItem()).longValue());
            return CBORObject.FromObject(bvalueA.add(bvalueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigInteger bvalueA = (BigInteger)a.getThisItem();
            BigInteger bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.add(bvalueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((BigInteger)a.getThisItem());
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((BigInteger)a.getThisItem());
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = new DecimalFraction((BigInteger)a.getThisItem());
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = new BigFloat((BigInteger)a.getThisItem());
            BigFloat valueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Integer: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = new BigFloat((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigInteger: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = new BigFloat((BigInteger)b.getThisItem());
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Single: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + ((Float)b.getThisItem()).floatValue());
            }
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + ((Float)a.getThisItem()).floatValue());
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Double: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + ((Double)b.getThisItem()).doubleValue());
            }
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + ((Float)a.getThisItem()).floatValue());
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = DecimalFraction.FromSingle((float)sa);
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigFloat: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Integer: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = new BigFloat((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigInteger: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = new BigFloat((BigInteger)b.getThisItem());
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Single: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + ((Float)b.getThisItem()).floatValue());
            }
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + ((Double)a.getThisItem()).doubleValue());
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Double: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + ((Double)b.getThisItem()).doubleValue());
            }
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + ((Double)a.getThisItem()).doubleValue());
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = DecimalFraction.FromDouble((double)sa);
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigFloat: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Integer: {
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = new DecimalFraction((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigInteger: {
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = new DecimalFraction((BigInteger)b.getThisItem());
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = DecimalFraction.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = DecimalFraction.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigFloat: {
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = DecimalFraction.FromBigFloat((BigFloat)b.getThisItem());
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Integer: {
            BigFloat valueA = (BigFloat)a.getThisItem();
            BigFloat valueB = new BigFloat((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigFloat valueA = (BigFloat)a.getThisItem();
            BigFloat valueB = new BigFloat((BigInteger)b.getThisItem());
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = (BigFloat)a.getThisItem();
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = (BigFloat)a.getThisItem();
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = DecimalFraction.FromBigFloat((BigFloat)a.getThisItem());
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = (BigFloat)a.getThisItem();
            BigFloat valueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        default:
          throw new IllegalArgumentException("a, b, or both are not numbers.");
      }
    }
    public static CBORObject Subtract(CBORObject a, CBORObject b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      int combo = (a.getItemType() << 4) | b.getItemType();
      switch (combo) {
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Integer: {
            long valueA = (((Long)a.getThisItem()).longValue());
            long valueB = (((Long)b.getThisItem()).longValue());
            if ((valueB < 0 && Long.MAX_VALUE + valueB < valueA) ||
                (valueB > 0 && Long.MIN_VALUE + valueB > valueA)) {
              // would overflow, convert to BigInteger
              return CBORObject.FromObject((BigInteger.valueOf(valueA)).subtract(BigInteger.valueOf(valueB)));
            }
            return CBORObject.FromObject(valueA - valueB);
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigInteger bvalueA = BigInteger.valueOf(((Long)a.getThisItem()).longValue());
            BigInteger bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.subtract(bvalueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((((Long)a.getThisItem()).longValue()));
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((((Long)a.getThisItem()).longValue()));
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = new DecimalFraction((((Long)a.getThisItem()).longValue()));
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = new BigFloat((((Long)a.getThisItem()).longValue()));
            BigFloat valueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Integer: {
            BigInteger bvalueA = (BigInteger)a.getThisItem();
            BigInteger bvalueB = BigInteger.valueOf(((Long)b.getThisItem()).longValue());
            return CBORObject.FromObject(bvalueA.subtract(bvalueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigInteger bvalueA = (BigInteger)a.getThisItem();
            BigInteger bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.subtract(bvalueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((BigInteger)a.getThisItem());
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((BigInteger)a.getThisItem());
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = new DecimalFraction((BigInteger)a.getThisItem());
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = new BigFloat((BigInteger)a.getThisItem());
            BigFloat valueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Integer: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = new BigFloat((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigInteger: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = new BigFloat((BigInteger)b.getThisItem());
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Single: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + ((Float)b.getThisItem()).floatValue());
            }
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + ((Float)a.getThisItem()).floatValue());
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Double: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + ((Double)b.getThisItem()).doubleValue());
            }
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + ((Float)a.getThisItem()).floatValue());
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = DecimalFraction.FromSingle((float)sa);
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigFloat: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Integer: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = new BigFloat((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigInteger: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = new BigFloat((BigInteger)b.getThisItem());
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Single: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + ((Float)b.getThisItem()).floatValue());
            }
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + ((Double)a.getThisItem()).doubleValue());
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Double: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + ((Double)b.getThisItem()).doubleValue());
            }
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + ((Double)a.getThisItem()).doubleValue());
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = DecimalFraction.FromDouble((double)sa);
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigFloat: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Integer: {
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = new DecimalFraction((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigInteger: {
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = new DecimalFraction((BigInteger)b.getThisItem());
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = DecimalFraction.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = DecimalFraction.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigFloat: {
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = DecimalFraction.FromBigFloat((BigFloat)b.getThisItem());
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Integer: {
            BigFloat valueA = (BigFloat)a.getThisItem();
            BigFloat valueB = new BigFloat((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigFloat valueA = (BigFloat)a.getThisItem();
            BigFloat valueB = new BigFloat((BigInteger)b.getThisItem());
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = (BigFloat)a.getThisItem();
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = (BigFloat)a.getThisItem();
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = DecimalFraction.FromBigFloat((BigFloat)a.getThisItem());
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = (BigFloat)a.getThisItem();
            BigFloat valueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        default:
          throw new IllegalArgumentException("a, b, or both are not numbers.");
      }
    }
    public static CBORObject Multiply(CBORObject a, CBORObject b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      int combo = (a.getItemType() << 4) | b.getItemType();
      switch (combo) {
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Integer: {
            long valueA = (((Long)a.getThisItem()).longValue());
            long valueB = (((Long)b.getThisItem()).longValue());
            boolean apos = (valueA > 0L);
            boolean bpos = (valueB > 0L);
            if (
              (apos && ((!bpos && (Long.MIN_VALUE / valueA) > valueB) ||
                        (bpos && valueA > (Long.MAX_VALUE / valueB)))) ||
              (!apos && ((!bpos && valueA != 0L &&
                           (Long.MAX_VALUE / valueA) > valueB) ||
                         (bpos && valueA < (Long.MIN_VALUE / valueB))))) {
              // would overflow, convert to BigInteger
              return CBORObject.FromObject((BigInteger.valueOf(valueA)).multiply(BigInteger.valueOf(valueB)));
            }
            return CBORObject.FromObject(valueA * valueB);
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigInteger bvalueA = BigInteger.valueOf(((Long)a.getThisItem()).longValue());
            BigInteger bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.multiply(bvalueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              int s = (int)Math.signum((((Long)a.getThisItem()).longValue()));
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            BigFloat valueA = new BigFloat((((Long)a.getThisItem()).longValue()));
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              int s = (int)Math.signum((((Long)a.getThisItem()).longValue()));
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            BigFloat valueA = new BigFloat((((Long)a.getThisItem()).longValue()));
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = new DecimalFraction((((Long)a.getThisItem()).longValue()));
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = new BigFloat((((Long)a.getThisItem()).longValue()));
            BigFloat valueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Integer: {
            BigInteger bvalueA = (BigInteger)a.getThisItem();
            BigInteger bvalueB = BigInteger.valueOf(((Long)b.getThisItem()).longValue());
            return CBORObject.FromObject(bvalueA.multiply(bvalueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigInteger bvalueA = (BigInteger)a.getThisItem();
            BigInteger bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.multiply(bvalueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              int s = ((BigInteger)a.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            BigFloat valueA = new BigFloat((BigInteger)a.getThisItem());
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              int s = ((BigInteger)a.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            BigFloat valueA = new BigFloat((BigInteger)a.getThisItem());
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = new DecimalFraction((BigInteger)a.getThisItem());
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = new BigFloat((BigInteger)a.getThisItem());
            BigFloat valueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Integer: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              int s = (int)Math.signum((((Long)b.getThisItem()).longValue()));
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = new BigFloat((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigInteger: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              int s = ((BigInteger)b.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = new BigFloat((BigInteger)b.getThisItem());
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Single: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sa * ((Float)b.getThisItem()).floatValue());
            }
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sb * ((Float)a.getThisItem()).floatValue());
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Double: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sa * ((Double)b.getThisItem()).doubleValue());
            }
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sb * ((Double)a.getThisItem()).doubleValue());
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              int s = ((DecimalFraction)b.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            DecimalFraction valueA = DecimalFraction.FromSingle((float)sa);
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigFloat: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              int s = ((BigFloat)b.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Integer: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              int s = (int)Math.signum((((Long)b.getThisItem()).longValue()));
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = new BigFloat((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigInteger: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              int s = ((BigInteger)b.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = new BigFloat((BigInteger)b.getThisItem());
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Single: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sa * ((Float)b.getThisItem()).floatValue());
            }
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sb * ((Float)a.getThisItem()).floatValue());
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Double: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sa * ((Double)b.getThisItem()).doubleValue());
            }
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sb * ((Double)a.getThisItem()).doubleValue());
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              int s = ((DecimalFraction)b.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            DecimalFraction valueA = DecimalFraction.FromDouble((double)sa);
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigFloat: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              int s = ((BigFloat)b.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Integer: {
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = new DecimalFraction((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigInteger: {
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = new DecimalFraction((BigInteger)b.getThisItem());
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              int s = ((DecimalFraction)a.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = DecimalFraction.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              int s = ((DecimalFraction)a.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = DecimalFraction.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigFloat: {
            DecimalFraction valueA = (DecimalFraction)a.getThisItem();
            DecimalFraction valueB = DecimalFraction.FromBigFloat((BigFloat)b.getThisItem());
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Integer: {
            BigFloat valueA = (BigFloat)a.getThisItem();
            BigFloat valueB = new BigFloat((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigFloat valueA = (BigFloat)a.getThisItem();
            BigFloat valueB = new BigFloat((BigInteger)b.getThisItem());
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              int s = ((BigFloat)a.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            BigFloat valueA = (BigFloat)a.getThisItem();
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              int s = ((BigFloat)a.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            BigFloat valueA = (BigFloat)a.getThisItem();
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = DecimalFraction.FromBigFloat((BigFloat)a.getThisItem());
            DecimalFraction valueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = (BigFloat)a.getThisItem();
            BigFloat valueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        default:
          throw new IllegalArgumentException("a, b, or both are not numbers.");
      }
    }
  }
