/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;

namespace PeterO {
    /// <summary>Implements arithmetic operations with CBOR objects.</summary>
    /// <returns>A CBORObject object.</returns>
    /// <param name='a'>A CBORObject object. (2).</param>
    /// <param name='b'>A CBORObject object. (3).</param>
  internal static class CBORObjectMath {
    // TODO: Implement division and remainder
    public static CBORObject Addition(CBORObject a, CBORObject b) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      if (a.Type != CBORType.Number) {
        throw new ArgumentException("a.Type (" + a.Type + ") is not equal to " + CBORType.Number);
      }
      if (b.Type != CBORType.Number) {
        throw new ArgumentException("b.Type (" + b.Type + ") is not equal to " + CBORType.Number);
      }
      object objA = a.ThisItem;
      object objB = b.ThisItem;
      int typeA = a.ItemType;
      int typeB = b.ItemType;
      if (typeA == CBORObject.CBORObjectTypeInteger && typeB == CBORObject.CBORObjectTypeInteger) {
        long valueA = (long)objA;
        long valueB = (long)objB;
        if ((valueA < 0 && valueB < Int64.MinValue - valueA) ||
            (valueA > 0 && valueB > Int64.MaxValue - valueA)) {
          // would overflow, convert to BigInteger
          return CBORObject.FromObject(((BigInteger)valueA) + (BigInteger)valueB);
        }
        return CBORObject.FromObject(valueA + valueB);
      } else if (typeA == CBORObject.CBORObjectTypeExtendedRational ||
                 typeB == CBORObject.CBORObjectTypeExtendedRational) {
        ExtendedRational e1 = CBORObject.NumberInterfaces[typeA].AsExtendedRational(objA);
        ExtendedRational e2 = CBORObject.NumberInterfaces[typeB].AsExtendedRational(objB);
        return CBORObject.FromObject(e1.Add(e2));
      } else if (typeA == CBORObject.CBORObjectTypeExtendedDecimal ||
                 typeB == CBORObject.CBORObjectTypeExtendedDecimal) {
        ExtendedDecimal e1 = CBORObject.NumberInterfaces[typeA].AsExtendedDecimal(objA);
        ExtendedDecimal e2 = CBORObject.NumberInterfaces[typeB].AsExtendedDecimal(objB);
        return CBORObject.FromObject(e1.Add(e2));
      } else if (typeA == CBORObject.CBORObjectTypeExtendedFloat || typeB == CBORObject.CBORObjectTypeExtendedFloat ||
                 typeA == CBORObject.CBORObjectTypeDouble || typeB == CBORObject.CBORObjectTypeDouble ||
                 typeA == CBORObject.CBORObjectTypeSingle || typeB == CBORObject.CBORObjectTypeSingle) {
        ExtendedFloat e1 = CBORObject.NumberInterfaces[typeA].AsExtendedFloat(objA);
        ExtendedFloat e2 = CBORObject.NumberInterfaces[typeB].AsExtendedFloat(objB);
        return CBORObject.FromObject(e1.Add(e2));
      } else {
        BigInteger b1 = CBORObject.NumberInterfaces[typeA].AsBigInteger(objA);
        BigInteger b2 = CBORObject.NumberInterfaces[typeB].AsBigInteger(objB);
        return CBORObject.FromObject(b1 + (BigInteger)b2);
      }
    }

    public static CBORObject Subtract(CBORObject a, CBORObject b) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      if (a.Type != CBORType.Number) {
        throw new ArgumentException("a.Type (" + a.Type + ") is not equal to " + CBORType.Number);
      }
      if (b.Type != CBORType.Number) {
        throw new ArgumentException("b.Type (" + b.Type + ") is not equal to " + CBORType.Number);
      }
      object objA = a.ThisItem;
      object objB = b.ThisItem;
      int typeA = a.ItemType;
      int typeB = b.ItemType;
      if (typeA == CBORObject.CBORObjectTypeInteger && typeB == CBORObject.CBORObjectTypeInteger) {
        long valueA = (long)objA;
        long valueB = (long)objB;
        if ((valueB < 0 && Int64.MaxValue + valueB < valueA) ||
            (valueB > 0 && Int64.MinValue + valueB > valueA)) {
          // would overflow, convert to BigInteger
          return CBORObject.FromObject(((BigInteger)valueA) - (BigInteger)valueB);
        }
        return CBORObject.FromObject(valueA - valueB);
      } else if (typeA == CBORObject.CBORObjectTypeExtendedRational ||
                 typeB == CBORObject.CBORObjectTypeExtendedRational) {
        ExtendedRational e1 = CBORObject.NumberInterfaces[typeA].AsExtendedRational(objA);
        ExtendedRational e2 = CBORObject.NumberInterfaces[typeB].AsExtendedRational(objB);
        return CBORObject.FromObject(e1.Subtract(e2));
      } else if (typeA == CBORObject.CBORObjectTypeExtendedDecimal ||
                 typeB == CBORObject.CBORObjectTypeExtendedDecimal) {
        ExtendedDecimal e1 = CBORObject.NumberInterfaces[typeA].AsExtendedDecimal(objA);
        ExtendedDecimal e2 = CBORObject.NumberInterfaces[typeB].AsExtendedDecimal(objB);
        return CBORObject.FromObject(e1.Subtract(e2));
      } else if (typeA == CBORObject.CBORObjectTypeExtendedFloat || typeB == CBORObject.CBORObjectTypeExtendedFloat ||
                 typeA == CBORObject.CBORObjectTypeDouble || typeB == CBORObject.CBORObjectTypeDouble ||
                 typeA == CBORObject.CBORObjectTypeSingle || typeB == CBORObject.CBORObjectTypeSingle) {
        ExtendedFloat e1 = CBORObject.NumberInterfaces[typeA].AsExtendedFloat(objA);
        ExtendedFloat e2 = CBORObject.NumberInterfaces[typeB].AsExtendedFloat(objB);
        return CBORObject.FromObject(e1.Subtract(e2));
      } else {
        BigInteger b1 = CBORObject.NumberInterfaces[typeA].AsBigInteger(objA);
        BigInteger b2 = CBORObject.NumberInterfaces[typeB].AsBigInteger(objB);
        return CBORObject.FromObject(b1 - (BigInteger)b2);
      }
    }

    public static CBORObject Multiply(CBORObject a, CBORObject b) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      if (a.Type != CBORType.Number) {
        throw new ArgumentException("a.Type (" + a.Type + ") is not equal to " + CBORType.Number);
      }
      if (b.Type != CBORType.Number) {
        throw new ArgumentException("b.Type (" + b.Type + ") is not equal to " + CBORType.Number);
      }
      object objA = a.ThisItem;
      object objB = b.ThisItem;
      int typeA = a.ItemType;
      int typeB = b.ItemType;
      if (typeA == CBORObject.CBORObjectTypeInteger && typeB == CBORObject.CBORObjectTypeInteger) {
        long valueA = (long)objA;
        long valueB = (long)objB;
        bool apos = valueA > 0L;
        bool bpos = valueB > 0L;
        if (
          (apos && ((!bpos && (Int64.MinValue / valueA) > valueB) ||
                    (bpos && valueA > (Int64.MaxValue / valueB)))) ||
          (!apos && ((!bpos && valueA != 0L &&
                      (Int64.MaxValue / valueA) > valueB) ||
                     (bpos && valueA < (Int64.MinValue / valueB))))) {
          // would overflow, convert to BigInteger
          BigInteger bvalueA = (BigInteger)valueA;
          BigInteger bvalueB = (BigInteger)valueB;
          return CBORObject.FromObject(bvalueA * (BigInteger)bvalueB);
        }
        return CBORObject.FromObject(valueA * valueB);
      } else if (typeA == CBORObject.CBORObjectTypeExtendedRational ||
                 typeB == CBORObject.CBORObjectTypeExtendedRational) {
        ExtendedRational e1 = CBORObject.NumberInterfaces[typeA].AsExtendedRational(objA);
        ExtendedRational e2 = CBORObject.NumberInterfaces[typeB].AsExtendedRational(objB);
        return CBORObject.FromObject(e1.Multiply(e2));
      } else if (typeA == CBORObject.CBORObjectTypeExtendedDecimal ||
                 typeB == CBORObject.CBORObjectTypeExtendedDecimal) {
        ExtendedDecimal e1 = CBORObject.NumberInterfaces[typeA].AsExtendedDecimal(objA);
        ExtendedDecimal e2 = CBORObject.NumberInterfaces[typeB].AsExtendedDecimal(objB);
        return CBORObject.FromObject(e1.Multiply(e2));
      } else if (typeA == CBORObject.CBORObjectTypeExtendedFloat || typeB == CBORObject.CBORObjectTypeExtendedFloat ||
                 typeA == CBORObject.CBORObjectTypeDouble || typeB == CBORObject.CBORObjectTypeDouble ||
                 typeA == CBORObject.CBORObjectTypeSingle || typeB == CBORObject.CBORObjectTypeSingle) {
        ExtendedFloat e1 = CBORObject.NumberInterfaces[typeA].AsExtendedFloat(objA);
        ExtendedFloat e2 = CBORObject.NumberInterfaces[typeB].AsExtendedFloat(objB);
        return CBORObject.FromObject(e1.Multiply(e2));
      } else {
        BigInteger b1 = CBORObject.NumberInterfaces[typeA].AsBigInteger(objA);
        BigInteger b2 = CBORObject.NumberInterfaces[typeB].AsBigInteger(objB);
        return CBORObject.FromObject(b1 - (BigInteger)b2);
      }
    }
  }
}
