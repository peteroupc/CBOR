package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

//import java.math.*;

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
      DecimalFraction decfracValueA;
      DecimalFraction decfracValueB;
      BigFloat bigfloatValueA;
      BigFloat bigfloatValueB;
      BigInteger bvalueA;
      BigInteger bvalueB;
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
            bvalueA = BigInteger.valueOf(((Long)a.getThisItem()).longValue());
            bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.add(bvalueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromInt64((((Long)a.getThisItem()).longValue()));
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromInt64((((Long)a.getThisItem()).longValue()));
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromInt64((((Long)a.getThisItem()).longValue()));
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = BigFloat.FromInt64((((Long)a.getThisItem()).longValue()));
            bigfloatValueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Integer: {
            bvalueA = (BigInteger)a.getThisItem();
            bvalueB = BigInteger.valueOf(((Long)b.getThisItem()).longValue());
            return CBORObject.FromObject(bvalueA.add(bvalueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigInteger: {
            bvalueA = (BigInteger)a.getThisItem();
            bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.add(bvalueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.getThisItem());
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.getThisItem());
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromBigInteger((BigInteger)a.getThisItem());
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.getThisItem());
            bigfloatValueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Integer: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromInt64((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigInteger: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.getThisItem());
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = DecimalFraction.FromSingle((float)sa);
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigFloat: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Integer: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromInt64((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigInteger: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.getThisItem());
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = DecimalFraction.FromDouble((double)sa);
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigFloat: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Integer: {
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = DecimalFraction.FromInt64((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigInteger: {
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = DecimalFraction.FromBigInteger((BigInteger)b.getThisItem());
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = DecimalFraction.FromSingle((float)sb);
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = DecimalFraction.FromDouble((double)sb);
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigFloat: {
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = DecimalFraction.FromBigFloat((BigFloat)b.getThisItem());
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Integer: {
            bigfloatValueA = (BigFloat)a.getThisItem();
            bigfloatValueB = BigFloat.FromInt64((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigInteger: {
            bigfloatValueA = (BigFloat)a.getThisItem();
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.getThisItem());
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = (BigFloat)a.getThisItem();
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = (BigFloat)a.getThisItem();
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromBigFloat((BigFloat)a.getThisItem());
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = (BigFloat)a.getThisItem();
            bigfloatValueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        default:
          throw new IllegalArgumentException("a, b, or both are not numbers.");
      }
    }
    public static CBORObject Subtract(CBORObject a, CBORObject b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      int combo = (a.getItemType() << 4) | b.getItemType();
      DecimalFraction decfracValueA;
      DecimalFraction decfracValueB;
      BigFloat bigfloatValueA;
      BigFloat bigfloatValueB;
      BigInteger bvalueA;
      BigInteger bvalueB;
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
            bvalueA = BigInteger.valueOf(((Long)a.getThisItem()).longValue());
            bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.subtract(bvalueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromInt64((((Long)a.getThisItem()).longValue()));
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromInt64((((Long)a.getThisItem()).longValue()));
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromInt64((((Long)a.getThisItem()).longValue()));
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = BigFloat.FromInt64((((Long)a.getThisItem()).longValue()));
            bigfloatValueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Integer: {
            bvalueA = (BigInteger)a.getThisItem();
            bvalueB = BigInteger.valueOf(((Long)b.getThisItem()).longValue());
            return CBORObject.FromObject(bvalueA.subtract(bvalueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigInteger: {
            bvalueA = (BigInteger)a.getThisItem();
            bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.subtract(bvalueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.getThisItem());
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.getThisItem());
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromBigInteger((BigInteger)a.getThisItem());
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.getThisItem());
            bigfloatValueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Integer: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromInt64((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigInteger: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.getThisItem());
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = DecimalFraction.FromSingle((float)sa);
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigFloat: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Integer: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromInt64((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigInteger: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.getThisItem());
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = DecimalFraction.FromDouble((double)sa);
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigFloat: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Integer: {
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = DecimalFraction.FromInt64((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigInteger: {
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = DecimalFraction.FromBigInteger((BigInteger)b.getThisItem());
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = DecimalFraction.FromSingle((float)sb);
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = DecimalFraction.FromDouble((double)sb);
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigFloat: {
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = DecimalFraction.FromBigFloat((BigFloat)b.getThisItem());
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Integer: {
            bigfloatValueA = (BigFloat)a.getThisItem();
            bigfloatValueB = BigFloat.FromInt64((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigInteger: {
            bigfloatValueA = (BigFloat)a.getThisItem();
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.getThisItem());
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = (BigFloat)a.getThisItem();
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = (BigFloat)a.getThisItem();
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromBigFloat((BigFloat)a.getThisItem());
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = (BigFloat)a.getThisItem();
            bigfloatValueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        default:
          throw new IllegalArgumentException("a, b, or both are not numbers.");
      }
    }
    public static CBORObject Multiply(CBORObject a, CBORObject b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      int combo = (a.getItemType() << 4) | b.getItemType();
      DecimalFraction decfracValueA;
      DecimalFraction decfracValueB;
      BigFloat bigfloatValueA;
      BigFloat bigfloatValueB;
      BigInteger bvalueA;
      BigInteger bvalueB;
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
            bvalueA = BigInteger.valueOf(((Long)a.getThisItem()).longValue());
            bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.multiply(bvalueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              int s = (int)Math.signum((((Long)a.getThisItem()).longValue()));
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            bigfloatValueA = BigFloat.FromInt64((((Long)a.getThisItem()).longValue()));
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              int s = (int)Math.signum((((Long)a.getThisItem()).longValue()));
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            bigfloatValueA = BigFloat.FromInt64((((Long)a.getThisItem()).longValue()));
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromInt64((((Long)a.getThisItem()).longValue()));
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = BigFloat.FromInt64((((Long)a.getThisItem()).longValue()));
            bigfloatValueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Integer: {
            bvalueA = (BigInteger)a.getThisItem();
            bvalueB = BigInteger.valueOf(((Long)b.getThisItem()).longValue());
            return CBORObject.FromObject(bvalueA.multiply(bvalueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigInteger: {
            bvalueA = (BigInteger)a.getThisItem();
            bvalueB = (BigInteger)b.getThisItem();
            return CBORObject.FromObject(bvalueA.multiply(bvalueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              int s = ((BigInteger)a.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.getThisItem());
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              int s = ((BigInteger)a.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.getThisItem());
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromBigInteger((BigInteger)a.getThisItem());
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.getThisItem());
            bigfloatValueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Integer: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              int s = (int)Math.signum((((Long)b.getThisItem()).longValue()));
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromInt64((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigInteger: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              int s = ((BigInteger)b.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.getThisItem());
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              int s = ((DecimalFraction)b.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            decfracValueA = DecimalFraction.FromSingle((float)sa);
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigFloat: {
            float sa = ((Float)a.getThisItem()).floatValue();
            if (Float.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Float)(sa)).isInfinite()) {
              int s = ((BigFloat)b.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Integer: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              int s = (int)Math.signum((((Long)b.getThisItem()).longValue()));
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromInt64((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigInteger: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              int s = ((BigInteger)b.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.getThisItem());
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              int s = ((DecimalFraction)b.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            decfracValueA = DecimalFraction.FromDouble((double)sa);
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigFloat: {
            double sa = ((Double)a.getThisItem()).doubleValue();
            if (Double.isNaN(sa)) return CBORObjectMath.NaN;
            if (((Double)(sa)).isInfinite()) {
              int s = ((BigFloat)b.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Integer: {
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = DecimalFraction.FromInt64((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigInteger: {
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = DecimalFraction.FromBigInteger((BigInteger)b.getThisItem());
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              int s = ((DecimalFraction)a.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = DecimalFraction.FromSingle((float)sb);
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              int s = ((DecimalFraction)a.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = DecimalFraction.FromDouble((double)sb);
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigFloat: {
            decfracValueA = (DecimalFraction)a.getThisItem();
            decfracValueB = DecimalFraction.FromBigFloat((BigFloat)b.getThisItem());
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Integer: {
            bigfloatValueA = (BigFloat)a.getThisItem();
            bigfloatValueB = BigFloat.FromInt64((((Long)b.getThisItem()).longValue()));
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigInteger: {
            bigfloatValueA = (BigFloat)a.getThisItem();
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.getThisItem());
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Single: {
            float sb = ((Float)b.getThisItem()).floatValue();
            if (Float.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Float)(sb)).isInfinite()) {
              int s = ((BigFloat)a.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            bigfloatValueA = (BigFloat)a.getThisItem();
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Double: {
            double sb = ((Double)b.getThisItem()).doubleValue();
            if (Double.isNaN(sb)) return CBORObjectMath.NaN;
            if (((Double)(sb)).isInfinite()) {
              int s = ((BigFloat)a.getThisItem()).signum();
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            bigfloatValueA = (BigFloat)a.getThisItem();
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromBigFloat((BigFloat)a.getThisItem());
            decfracValueB = (DecimalFraction)b.getThisItem();
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = (BigFloat)a.getThisItem();
            bigfloatValueB = (BigFloat)b.getThisItem();
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        default:
          throw new IllegalArgumentException("a, b, or both are not numbers.");
      }
    }
  }
