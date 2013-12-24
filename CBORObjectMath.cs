/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
//using System.Numerics;
namespace PeterO {
    /// <summary> Implements arithmetic operations with CBOR objects.
    /// </summary>
  static class CBORObjectMath {
    private static CBORObject NaN = CBORObject.FromObject(Double.NaN);
    /// <summary> </summary>
    /// <param name='a'>A CBORObject object.</param>
    /// <param name='b'>A CBORObject object.</param>
    /// <returns>A CBORObject object.</returns>
    public static CBORObject Addition(CBORObject a, CBORObject b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      int combo = (a.ItemType << 4) | b.ItemType;
      DecimalFraction decfracValueA;
      DecimalFraction decfracValueB;
      BigFloat bigfloatValueA;
      BigFloat bigfloatValueB;
      BigInteger bvalueA;
      BigInteger bvalueB;
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
            bvalueA = (BigInteger)(long)a.ThisItem;
            bvalueB = (BigInteger)b.ThisItem;
            return CBORObject.FromObject(bvalueA + (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromInt64((long)a.ThisItem);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromInt64((long)a.ThisItem);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromInt64((long)a.ThisItem);
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = BigFloat.FromInt64((long)a.ThisItem);
            bigfloatValueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Integer: {
            bvalueA = (BigInteger)a.ThisItem;
            bvalueB = (BigInteger)(long)b.ThisItem;
            return CBORObject.FromObject(bvalueA + (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigInteger: {
            bvalueA = (BigInteger)a.ThisItem;
            bvalueB = (BigInteger)b.ThisItem;
            return CBORObject.FromObject(bvalueA + (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.ThisItem);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.ThisItem);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromBigInteger((BigInteger)a.ThisItem);
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.ThisItem);
            bigfloatValueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Integer: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromInt64((long)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigInteger: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = DecimalFraction.FromSingle((float)sa);
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigFloat: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Integer: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromInt64((long)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigInteger: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = DecimalFraction.FromDouble((double)sa);
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigFloat: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Integer: {
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = DecimalFraction.FromInt64((long)b.ThisItem);
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigInteger: {
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = DecimalFraction.FromBigInteger((BigInteger)b.ThisItem);
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = DecimalFraction.FromSingle((float)sb);
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = DecimalFraction.FromDouble((double)sb);
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigFloat: {
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = DecimalFraction.FromBigFloat((BigFloat)b.ThisItem);
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Integer: {
            bigfloatValueA = (BigFloat)a.ThisItem;
            bigfloatValueB = BigFloat.FromInt64((long)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigInteger: {
            bigfloatValueA = (BigFloat)a.ThisItem;
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = (BigFloat)a.ThisItem;
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = (BigFloat)a.ThisItem;
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromBigFloat((BigFloat)a.ThisItem);
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Add(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = (BigFloat)a.ThisItem;
            bigfloatValueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(bigfloatValueA.Add(bigfloatValueB));
          }
        default:
          throw new ArgumentException("a, b, or both are not numbers.");
      }
    }
    public static CBORObject Subtract(CBORObject a, CBORObject b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      int combo = (a.ItemType << 4) | b.ItemType;
      DecimalFraction decfracValueA;
      DecimalFraction decfracValueB;
      BigFloat bigfloatValueA;
      BigFloat bigfloatValueB;
      BigInteger bvalueA;
      BigInteger bvalueB;
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
            bvalueA = (BigInteger)(long)a.ThisItem;
            bvalueB = (BigInteger)b.ThisItem;
            return CBORObject.FromObject(bvalueA - (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromInt64((long)a.ThisItem);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromInt64((long)a.ThisItem);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromInt64((long)a.ThisItem);
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = BigFloat.FromInt64((long)a.ThisItem);
            bigfloatValueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Integer: {
            bvalueA = (BigInteger)a.ThisItem;
            bvalueB = (BigInteger)(long)b.ThisItem;
            return CBORObject.FromObject(bvalueA - (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigInteger: {
            bvalueA = (BigInteger)a.ThisItem;
            bvalueB = (BigInteger)b.ThisItem;
            return CBORObject.FromObject(bvalueA - (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.ThisItem);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.ThisItem);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromBigInteger((BigInteger)a.ThisItem);
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.ThisItem);
            bigfloatValueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Integer: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromInt64((long)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigInteger: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = DecimalFraction.FromSingle((float)sa);
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigFloat: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Integer: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromInt64((long)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigInteger: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = DecimalFraction.FromDouble((double)sa);
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigFloat: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              return a; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Integer: {
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = DecimalFraction.FromInt64((long)b.ThisItem);
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigInteger: {
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = DecimalFraction.FromBigInteger((BigInteger)b.ThisItem);
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = DecimalFraction.FromSingle((float)sb);
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = DecimalFraction.FromDouble((double)sb);
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigFloat: {
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = DecimalFraction.FromBigFloat((BigFloat)b.ThisItem);
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Integer: {
            bigfloatValueA = (BigFloat)a.ThisItem;
            bigfloatValueB = BigFloat.FromInt64((long)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigInteger: {
            bigfloatValueA = (BigFloat)a.ThisItem;
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = (BigFloat)a.ThisItem;
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              return b; // +/-infinity plus or minus any finite number is unchanged
            }
            bigfloatValueA = (BigFloat)a.ThisItem;
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromBigFloat((BigFloat)a.ThisItem);
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Subtract(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = (BigFloat)a.ThisItem;
            bigfloatValueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(bigfloatValueA.Subtract(bigfloatValueB));
          }
        default:
          throw new ArgumentException("a, b, or both are not numbers.");
      }
    }
    public static CBORObject Multiply(CBORObject a, CBORObject b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      int combo = (a.ItemType << 4) | b.ItemType;
      DecimalFraction decfracValueA;
      DecimalFraction decfracValueB;
      BigFloat bigfloatValueA;
      BigFloat bigfloatValueB;
      BigInteger bvalueA;
      BigInteger bvalueB;
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
            bvalueA = (BigInteger)(long)a.ThisItem;
            bvalueB = (BigInteger)b.ThisItem;
            return CBORObject.FromObject(bvalueA * (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              int s = Math.Sign((long)a.ThisItem);
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            bigfloatValueA = BigFloat.FromInt64((long)a.ThisItem);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              int s = Math.Sign((long)a.ThisItem);
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            bigfloatValueA = BigFloat.FromInt64((long)a.ThisItem);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromInt64((long)a.ThisItem);
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = BigFloat.FromInt64((long)a.ThisItem);
            bigfloatValueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Integer: {
            bvalueA = (BigInteger)a.ThisItem;
            bvalueB = (BigInteger)(long)b.ThisItem;
            return CBORObject.FromObject(bvalueA * (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigInteger: {
            bvalueA = (BigInteger)a.ThisItem;
            bvalueB = (BigInteger)b.ThisItem;
            return CBORObject.FromObject(bvalueA * (BigInteger)bvalueB);
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              int s = ((BigInteger)a.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.ThisItem);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              int s = ((BigInteger)a.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.ThisItem);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromBigInteger((BigInteger)a.ThisItem);
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigInteger << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = BigFloat.FromBigInteger((BigInteger)a.ThisItem);
            bigfloatValueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_Integer: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              int s = Math.Sign((long)b.ThisItem);
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromInt64((long)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigInteger: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              int s = ((BigInteger)b.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              int s = ((DecimalFraction)b.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            decfracValueA = DecimalFraction.FromSingle((float)sa);
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Single << 4) | CBORObject.CBORObjectType_BigFloat: {
            float sa = (float)a.ThisItem;
            if (Single.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sa)) {
              int s = ((BigFloat)b.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            bigfloatValueA = BigFloat.FromSingle((float)sa);
            bigfloatValueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_Integer: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              int s = Math.Sign((long)b.ThisItem);
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromInt64((long)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigInteger: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              int s = ((BigInteger)b.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
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
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              int s = ((DecimalFraction)b.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            decfracValueA = DecimalFraction.FromDouble((double)sa);
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_Double << 4) | CBORObject.CBORObjectType_BigFloat: {
            double sa = (double)a.ThisItem;
            if (Double.IsNaN(sa)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sa)) {
              int s = ((BigFloat)b.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sa : sa));
            }
            bigfloatValueA = BigFloat.FromDouble((double)sa);
            bigfloatValueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Integer: {
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = DecimalFraction.FromInt64((long)b.ThisItem);
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigInteger: {
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = DecimalFraction.FromBigInteger((BigInteger)b.ThisItem);
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              int s = ((DecimalFraction)a.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = DecimalFraction.FromSingle((float)sb);
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              int s = ((DecimalFraction)a.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = DecimalFraction.FromDouble((double)sb);
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_DecimalFraction << 4) | CBORObject.CBORObjectType_BigFloat: {
            decfracValueA = (DecimalFraction)a.ThisItem;
            decfracValueB = DecimalFraction.FromBigFloat((BigFloat)b.ThisItem);
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Integer: {
            bigfloatValueA = (BigFloat)a.ThisItem;
            bigfloatValueB = BigFloat.FromInt64((long)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigInteger: {
            bigfloatValueA = (BigFloat)a.ThisItem;
            bigfloatValueB = BigFloat.FromBigInteger((BigInteger)b.ThisItem);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Single: {
            float sb = (float)b.ThisItem;
            if (Single.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Single.IsInfinity(sb)) {
              int s = ((BigFloat)a.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            bigfloatValueA = (BigFloat)a.ThisItem;
            bigfloatValueB = BigFloat.FromSingle((float)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_Double: {
            double sb = (double)b.ThisItem;
            if (Double.IsNaN(sb)) return CBORObjectMath.NaN;
            if (Double.IsInfinity(sb)) {
              int s = ((BigFloat)a.ThisItem).Sign;
              return (s == 0 ? CBORObjectMath.NaN : CBORObject.FromObject(s < 0 ? -sb : sb));
            }
            bigfloatValueA = (BigFloat)a.ThisItem;
            bigfloatValueB = BigFloat.FromDouble((double)sb);
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_DecimalFraction: {
            decfracValueA = DecimalFraction.FromBigFloat((BigFloat)a.ThisItem);
            decfracValueB = (DecimalFraction)b.ThisItem;
            return CBORObject.FromObject(decfracValueA.Multiply(decfracValueB));
          }
        case (CBORObject.CBORObjectType_BigFloat << 4) | CBORObject.CBORObjectType_BigFloat: {
            bigfloatValueA = (BigFloat)a.ThisItem;
            bigfloatValueB = (BigFloat)b.ThisItem;
            return CBORObject.FromObject(bigfloatValueA.Multiply(bigfloatValueB));
          }
        default:
          throw new ArgumentException("a, b, or both are not numbers.");
      }
    }
  }
}