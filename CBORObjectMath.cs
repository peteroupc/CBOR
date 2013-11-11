/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Numerics;

namespace PeterO {
  /// <summary>
  /// Implements arithmetic operations with CBOR objects.
  /// </summary>
  static class CBORObjectMath {
    private static CBORObject NaN = CBORObject.FromObject(Double.NaN);


    public static CBORObject Addition(CBORObject a, CBORObject b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      int combo = (a.ItemType << 4) | b.ItemType;
      switch (combo) {
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Integer: {
            long valueA = (long)a.ThisItem;
            long valueB = (long)b.ThisItem;
            if ((valueA < 0 && valueB < Int64.MinValue - valueA) ||
                (valueA > 0 && valueB > Int64.MaxValue - valueA)) {
              // would overflow, convert to BigInteger
              return CBORObject.FromObject(((BigInteger)valueA) + (BigInteger)valueB);
            }
            return CBORObject.FromObject(valueA + valueB);
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigInteger bvalueA = (BigInteger)(long)a.ThisItem;
            BigInteger bvalueB = (BigInteger)b.ThisItem;
            return CBORObject.FromObject(bvalueA + (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((long)a.ThisItem);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((long)a.ThisItem);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = new DecimalFraction((long)a.ThisItem);
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = new BigFloat((long)a.ThisItem);
            BigFloat valueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Integer: {
            BigInteger bvalueA = (BigInteger)a.ThisItem;
            BigInteger bvalueB = (BigInteger)(long)b.ThisItem;
            return CBORObject.FromObject(bvalueA + (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigInteger bvalueA = (BigInteger)a.ThisItem;
            BigInteger bvalueB = (BigInteger)b.ThisItem;
            return CBORObject.FromObject(bvalueA + (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((BigInteger)a.ThisItem);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((BigInteger)a.ThisItem);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = new DecimalFraction((BigInteger)a.ThisItem);
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = new BigFloat((BigInteger)a.ThisItem);
            BigFloat valueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Integer: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = new BigFloat((long)b.ThisItem);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigInteger: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = new BigFloat((BigInteger)b.ThisItem);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Single: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + (float)b.ThisItem);
            }
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + (float)a.ThisItem);
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Double: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + (double)b.ThisItem);
            }
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + (float)a.ThisItem);
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = DecimalFraction.FromSingle((float)sa);
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigFloat: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Integer: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = new BigFloat((long)b.ThisItem);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigInteger: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = new BigFloat((BigInteger)b.ThisItem);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Single: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + (float)b.ThisItem);
            }
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + (double)a.ThisItem);
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Double: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + (double)b.ThisItem);
            }
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + (double)a.ThisItem);
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = DecimalFraction.FromDouble((double)sa);
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigFloat: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Integer: {
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = new DecimalFraction((long)b.ThisItem);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigInteger: {
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = new DecimalFraction((BigInteger)b.ThisItem);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = DecimalFraction.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = DecimalFraction.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigFloat: {
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = DecimalFraction.FromBigFloat((BigFloat)b.ThisItem);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Integer: {
            BigFloat valueA = (BigFloat)a.ThisItem;
            BigFloat valueB = new BigFloat((long)b.ThisItem);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigFloat valueA = (BigFloat)a.ThisItem;
            BigFloat valueB = new BigFloat((BigInteger)b.ThisItem);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = (BigFloat)a.ThisItem;
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = (BigFloat)a.ThisItem;
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = DecimalFraction.FromBigFloat((BigFloat)a.ThisItem);
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = (BigFloat)a.ThisItem;
            BigFloat valueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(valueA.Add(valueB));
          }
        default:
          throw new ArgumentException("a, b, or both are not numbers.");
      }
    }

    public static CBORObject Subtract(CBORObject a, CBORObject b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      int combo = (a.ItemType << 4) | b.ItemType;
      switch (combo) {
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Integer: {
            long valueA = (long)a.ThisItem;
            long valueB = (long)b.ThisItem;
            if ((valueB < 0 && Int64.MaxValue + valueB < valueA) ||
                (valueB > 0 && Int64.MinValue + valueB > valueA)) {
              // would overflow, convert to BigInteger
              return CBORObject.FromObject(((BigInteger)valueA) - (BigInteger)valueB);
            }
            return CBORObject.FromObject(valueA - valueB);
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigInteger bvalueA = (BigInteger)(long)a.ThisItem;
            BigInteger bvalueB = (BigInteger)b.ThisItem;
            return CBORObject.FromObject(bvalueA - (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((long)a.ThisItem);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((long)a.ThisItem);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = new DecimalFraction((long)a.ThisItem);
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = new BigFloat((long)a.ThisItem);
            BigFloat valueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Integer: {
            BigInteger bvalueA = (BigInteger)a.ThisItem;
            BigInteger bvalueB = (BigInteger)(long)b.ThisItem;
            return CBORObject.FromObject(bvalueA - (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigInteger bvalueA = (BigInteger)a.ThisItem;
            BigInteger bvalueB = (BigInteger)b.ThisItem;
            return CBORObject.FromObject(bvalueA - (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((BigInteger)a.ThisItem);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = new BigFloat((BigInteger)a.ThisItem);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = new DecimalFraction((BigInteger)a.ThisItem);
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = new BigFloat((BigInteger)a.ThisItem);
            BigFloat valueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Integer: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = new BigFloat((long)b.ThisItem);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigInteger: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = new BigFloat((BigInteger)b.ThisItem);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Single: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + (float)b.ThisItem);
            }
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + (float)a.ThisItem);
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Double: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + (double)b.ThisItem);
            }
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + (float)a.ThisItem);
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = DecimalFraction.FromSingle((float)sa);
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigFloat: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Integer: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = new BigFloat((long)b.ThisItem);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigInteger: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = new BigFloat((BigInteger)b.ThisItem);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Single: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + (float)b.ThisItem);
            }
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + (double)a.ThisItem);
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Double: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sa + (double)b.ThisItem);
            }
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              // result for addition/subtraction will always be infinity or NaN
              return CBORObject.FromObject(sb + (double)a.ThisItem);
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = DecimalFraction.FromDouble((double)sa);
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigFloat: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Integer: {
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = new DecimalFraction((long)b.ThisItem);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigInteger: {
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = new DecimalFraction((BigInteger)b.ThisItem);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = DecimalFraction.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = DecimalFraction.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigFloat: {
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = DecimalFraction.FromBigFloat((BigFloat)b.ThisItem);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Integer: {
            BigFloat valueA = (BigFloat)a.ThisItem;
            BigFloat valueB = new BigFloat((long)b.ThisItem);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigFloat valueA = (BigFloat)a.ThisItem;
            BigFloat valueB = new BigFloat((BigInteger)b.ThisItem);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = (BigFloat)a.ThisItem;
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            BigFloat valueA = (BigFloat)a.ThisItem;
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = DecimalFraction.FromBigFloat((BigFloat)a.ThisItem);
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = (BigFloat)a.ThisItem;
            BigFloat valueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(valueA.Subtract(valueB));
          }
        default:
          throw new ArgumentException("a, b, or both are not numbers.");
      }
    }

    public static CBORObject Multiply(CBORObject a, CBORObject b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      int combo = (a.ItemType << 4) | b.ItemType;
      switch (combo) {
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Integer: {
            long valueA = (long)a.ThisItem;
            long valueB = (long)b.ThisItem;
            bool apos = (valueA > 0L);
            bool bpos = (valueB > 0L);
            if (
              (apos && ((!bpos && (Int64.MinValue / valueA) > valueB) ||
                        (bpos && valueA > (Int64.MaxValue / valueB)))) ||
              (!apos && ((!bpos && valueA != 0L &&
                           (Int64.MaxValue / valueA) > valueB) ||
                         (bpos && valueA < (Int64.MinValue / valueB))))) {
              // would overflow, convert to BigInteger
              return CBORObject.FromObject(((BigInteger)valueA) * (BigInteger)valueB);
            }
            return CBORObject.FromObject(valueA * valueB);
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigInteger bvalueA = (BigInteger)(long)a.ThisItem;
            BigInteger bvalueB = (BigInteger)b.ThisItem;
            return CBORObject.FromObject(bvalueA * (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              int s = Math.Sign((long)a.ThisItem);
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            BigFloat valueA = new BigFloat((long)a.ThisItem);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              int s = Math.Sign((long)a.ThisItem);
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            BigFloat valueA = new BigFloat((long)a.ThisItem);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = new DecimalFraction((long)a.ThisItem);
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = new BigFloat((long)a.ThisItem);
            BigFloat valueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Integer: {
            BigInteger bvalueA = (BigInteger)a.ThisItem;
            BigInteger bvalueB = (BigInteger)(long)b.ThisItem;
            return CBORObject.FromObject(bvalueA * (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigInteger bvalueA = (BigInteger)a.ThisItem;
            BigInteger bvalueB = (BigInteger)b.ThisItem;
            return CBORObject.FromObject(bvalueA * (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              int s = ((BigInteger)a.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            BigFloat valueA = new BigFloat((BigInteger)a.ThisItem);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              int s = ((BigInteger)a.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            BigFloat valueA = new BigFloat((BigInteger)a.ThisItem);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = new DecimalFraction((BigInteger)a.ThisItem);
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = new BigFloat((BigInteger)a.ThisItem);
            BigFloat valueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Integer: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              int s = Math.Sign((long)b.ThisItem);
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = new BigFloat((long)b.ThisItem);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigInteger: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              int s = ((BigInteger)b.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = new BigFloat((BigInteger)b.ThisItem);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Single: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sa * (float)b.ThisItem);
            }
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sb * (float)a.ThisItem);
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Double: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sa * (double)b.ThisItem);
            }
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sb * (double)a.ThisItem);
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              int s = ((DecimalFraction)b.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            DecimalFraction valueA = DecimalFraction.FromSingle((float)sa);
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigFloat: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              int s = ((BigFloat)b.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            BigFloat valueA = BigFloat.FromSingle((float)sa);
            BigFloat valueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Integer: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              int s = Math.Sign((long)b.ThisItem);
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = new BigFloat((long)b.ThisItem);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigInteger: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              int s = ((BigInteger)b.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = new BigFloat((BigInteger)b.ThisItem);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Single: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sa * (float)b.ThisItem);
            }
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sb * (float)a.ThisItem);
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Double: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sa * (double)b.ThisItem);
            }
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              // result for multiplication will always be infinity or NaN
              return CBORObject.FromObject(sb * (double)a.ThisItem);
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              int s = ((DecimalFraction)b.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            DecimalFraction valueA = DecimalFraction.FromDouble((double)sa);
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigFloat: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              int s = ((BigFloat)b.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            BigFloat valueA = BigFloat.FromDouble((double)sa);
            BigFloat valueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Integer: {
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = new DecimalFraction((long)b.ThisItem);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigInteger: {
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = new DecimalFraction((BigInteger)b.ThisItem);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              int s = ((DecimalFraction)a.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = DecimalFraction.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              int s = ((DecimalFraction)a.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = DecimalFraction.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigFloat: {
            DecimalFraction valueA = (DecimalFraction)a.ThisItem;
            DecimalFraction valueB = DecimalFraction.FromBigFloat((BigFloat)b.ThisItem);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Integer: {
            BigFloat valueA = (BigFloat)a.ThisItem;
            BigFloat valueB = new BigFloat((long)b.ThisItem);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigInteger: {
            BigFloat valueA = (BigFloat)a.ThisItem;
            BigFloat valueB = new BigFloat((BigInteger)b.ThisItem);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              int s = ((BigFloat)a.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            BigFloat valueA = (BigFloat)a.ThisItem;
            BigFloat valueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              int s = ((BigFloat)a.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            BigFloat valueA = (BigFloat)a.ThisItem;
            BigFloat valueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            DecimalFraction valueA = DecimalFraction.FromBigFloat((BigFloat)a.ThisItem);
            DecimalFraction valueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigFloat: {
            BigFloat valueA = (BigFloat)a.ThisItem;
            BigFloat valueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(valueA.Multiply(valueB));
          }
        default:
          throw new ArgumentException("a, b, or both are not numbers.");
      }
    }
  }
}
