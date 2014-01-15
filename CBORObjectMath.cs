/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
// using System.Numerics;
namespace PeterO {
    /// <summary> Implements arithmetic operations with CBOR objects.
    /// </summary>
    /// <returns>A CBORObject object.</returns>
    /// <param name='a'>A CBORObject object.</param>
    /// <param name='b'>A CBORObject object.</param>
  static class CBORObjectMath {

    public static CBORObject Addition(CBORObject a, CBORObject b) {
      if (a == null) { throw new ArgumentNullException("a"); }
      if (b == null) { throw new ArgumentNullException("b"); }
      int combo = (a.ItemType << 4) | b.ItemType;
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
        default:
          return CBORObject.FromObject(a.AsExtendedDecimal().Add(b.AsExtendedDecimal()));
      }
    }

    public static CBORObject Subtract(CBORObject a, CBORObject b) {
      if (a == null) { throw new ArgumentNullException("a"); }
      if (b == null) { throw new ArgumentNullException("b"); }
      int combo = (a.ItemType << 4) | b.ItemType;
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
        default:
          return CBORObject.FromObject(a.AsExtendedDecimal().Subtract(b.AsExtendedDecimal()));
      }
    }

    public static CBORObject Multiply(CBORObject a, CBORObject b) {
      if (a == null) { throw new ArgumentNullException("a"); }
      if (b == null) { throw new ArgumentNullException("b"); }
      int combo = (a.ItemType << 4) | b.ItemType;
      BigInteger bvalueA;
      BigInteger bvalueB;
      switch (combo) {
        case (CBORObject.CBORObjectType_Integer << 4) | CBORObject.CBORObjectType_Integer: {
            long valueA = (long)a.ThisItem;
            long valueB = (long)b.ThisItem;
            bool apos = valueA > 0L;
            bool bpos = valueB > 0L;
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
        default:
          return CBORObject.FromObject(a.AsExtendedDecimal().Multiply(b.AsExtendedDecimal()));
      }
    }
  }
}
