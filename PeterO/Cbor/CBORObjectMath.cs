/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using PeterO;

namespace PeterO.Cbor {
    /// <summary>Implements arithmetic operations with CBOR
    /// objects.</summary>
  internal static class CBORObjectMath {
    public static CBORObject Addition(CBORObject a, CBORObject b) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      if (a.Type != CBORType.Number) {
        throw new ArgumentException("a.Type (" + a.Type +
          ") is not equal to " + CBORType.Number);
      }
      if (b.Type != CBORType.Number) {
        throw new ArgumentException("b.Type (" + b.Type +
          ") is not equal to " + CBORType.Number);
      }
      object objA = a.ThisItem;
      object objB = b.ThisItem;
      int typeA = a.ItemType;
      int typeB = b.ItemType;
      if (typeA == CBORObject.CBORObjectTypeInteger && typeB ==
      CBORObject.CBORObjectTypeInteger) {
        var valueA = (long)objA;
        var valueB = (long)objB;
        if ((valueA < 0 && valueB < Int64.MinValue - valueA) ||
                (valueA > 0 && valueB > Int64.MaxValue - valueA)) {
          // would overflow, convert to BigInteger
          return CBORObject.FromObject(((BigInteger)valueA) +
          (BigInteger)valueB);
        }
        return CBORObject.FromObject(valueA + valueB);
      }
      if (typeA == CBORObject.CBORObjectTypeExtendedRational ||
             typeB == CBORObject.CBORObjectTypeExtendedRational) {
        ExtendedRational e1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedRational(objA);
        ExtendedRational e2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedRational(objB);
        return CBORObject.FromObject(e1.Add(e2));
      }
      if (typeA == CBORObject.CBORObjectTypeExtendedDecimal ||
             typeB == CBORObject.CBORObjectTypeExtendedDecimal) {
        ExtendedDecimal e1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedDecimal(objA);
        ExtendedDecimal e2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedDecimal(objB);
        return CBORObject.FromObject(e1.Add(e2));
      }
      if (typeA == CBORObject.CBORObjectTypeExtendedFloat || typeB ==
      CBORObject.CBORObjectTypeExtendedFloat ||
               typeA == CBORObject.CBORObjectTypeDouble || typeB ==
               CBORObject.CBORObjectTypeDouble ||
               typeA == CBORObject.CBORObjectTypeSingle || typeB ==
               CBORObject.CBORObjectTypeSingle) {
        ExtendedFloat e1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedFloat(objA);
        ExtendedFloat e2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedFloat(objB);
        return CBORObject.FromObject(e1.Add(e2));
      } else {
        BigInteger b1 = CBORObject.GetNumberInterface(typeA).AsBigInteger(objA);
        BigInteger b2 = CBORObject.GetNumberInterface(typeB).AsBigInteger(objB);
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
        throw new ArgumentException("a.Type (" + a.Type +
          ") is not equal to " + CBORType.Number);
      }
      if (b.Type != CBORType.Number) {
        throw new ArgumentException("b.Type (" + b.Type +
          ") is not equal to " + CBORType.Number);
      }
      object objA = a.ThisItem;
      object objB = b.ThisItem;
      int typeA = a.ItemType;
      int typeB = b.ItemType;
      if (typeA == CBORObject.CBORObjectTypeInteger && typeB ==
      CBORObject.CBORObjectTypeInteger) {
        var valueA = (long)objA;
        var valueB = (long)objB;
        if ((valueB < 0 && Int64.MaxValue + valueB < valueA) ||
                (valueB > 0 && Int64.MinValue + valueB > valueA)) {
          // would overflow, convert to BigInteger
          return CBORObject.FromObject(((BigInteger)valueA) -
          (BigInteger)valueB);
        }
        return CBORObject.FromObject(valueA - valueB);
      }
      if (typeA == CBORObject.CBORObjectTypeExtendedRational ||
             typeB == CBORObject.CBORObjectTypeExtendedRational) {
        ExtendedRational e1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedRational(objA);
        ExtendedRational e2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedRational(objB);
        return CBORObject.FromObject(e1.Subtract(e2));
      }
      if (typeA == CBORObject.CBORObjectTypeExtendedDecimal ||
             typeB == CBORObject.CBORObjectTypeExtendedDecimal) {
        ExtendedDecimal e1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedDecimal(objA);
        ExtendedDecimal e2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedDecimal(objB);
        return CBORObject.FromObject(e1.Subtract(e2));
      }
      if (typeA == CBORObject.CBORObjectTypeExtendedFloat || typeB ==
      CBORObject.CBORObjectTypeExtendedFloat ||
               typeA == CBORObject.CBORObjectTypeDouble || typeB ==
               CBORObject.CBORObjectTypeDouble ||
               typeA == CBORObject.CBORObjectTypeSingle || typeB ==
               CBORObject.CBORObjectTypeSingle) {
        ExtendedFloat e1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedFloat(objA);
        ExtendedFloat e2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedFloat(objB);
        return CBORObject.FromObject(e1.Subtract(e2));
      } else {
        BigInteger b1 = CBORObject.GetNumberInterface(typeA).AsBigInteger(objA);
        BigInteger b2 = CBORObject.GetNumberInterface(typeB).AsBigInteger(objB);
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
        throw new ArgumentException("a.Type (" + a.Type +
          ") is not equal to " + CBORType.Number);
      }
      if (b.Type != CBORType.Number) {
        throw new ArgumentException("b.Type (" + b.Type +
          ") is not equal to " + CBORType.Number);
      }
      object objA = a.ThisItem;
      object objB = b.ThisItem;
      int typeA = a.ItemType;
      int typeB = b.ItemType;
      if (typeA == CBORObject.CBORObjectTypeInteger && typeB ==
      CBORObject.CBORObjectTypeInteger) {
        var valueA = (long)objA;
        var valueB = (long)objB;
        bool apos = valueA > 0L;
        bool bpos = valueB > 0L;
        if (
          (apos && ((!bpos && (Int64.MinValue / valueA) > valueB) ||
          (bpos && valueA > (Int64.MaxValue / valueB)))) ||
          (!apos && ((!bpos && valueA != 0L &&
          (Int64.MaxValue / valueA) > valueB) ||
          (bpos && valueA < (Int64.MinValue / valueB))))) {
          // would overflow, convert to BigInteger
          var bvalueA = (BigInteger)valueA;
          var bvalueB = (BigInteger)valueB;
          return CBORObject.FromObject(bvalueA * (BigInteger)bvalueB);
        }
        return CBORObject.FromObject(valueA * valueB);
      }
      if (typeA == CBORObject.CBORObjectTypeExtendedRational ||
             typeB == CBORObject.CBORObjectTypeExtendedRational) {
        ExtendedRational e1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedRational(objA);
        ExtendedRational e2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedRational(objB);
        return CBORObject.FromObject(e1.Multiply(e2));
      }
      if (typeA == CBORObject.CBORObjectTypeExtendedDecimal ||
             typeB == CBORObject.CBORObjectTypeExtendedDecimal) {
        ExtendedDecimal e1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedDecimal(objA);
        ExtendedDecimal e2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedDecimal(objB);
        return CBORObject.FromObject(e1.Multiply(e2));
      }
      if (typeA == CBORObject.CBORObjectTypeExtendedFloat || typeB ==
      CBORObject.CBORObjectTypeExtendedFloat ||
               typeA == CBORObject.CBORObjectTypeDouble || typeB ==
               CBORObject.CBORObjectTypeDouble ||
               typeA == CBORObject.CBORObjectTypeSingle || typeB ==
               CBORObject.CBORObjectTypeSingle) {
        ExtendedFloat e1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedFloat(objA);
        ExtendedFloat e2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedFloat(objB);
        return CBORObject.FromObject(e1.Multiply(e2));
      } else {
        BigInteger b1 = CBORObject.GetNumberInterface(typeA).AsBigInteger(objA);
        BigInteger b2 = CBORObject.GetNumberInterface(typeB).AsBigInteger(objB);
        return CBORObject.FromObject(b1 * (BigInteger)b2);
      }
    }

    public static CBORObject Divide(CBORObject a, CBORObject b) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      if (a.Type != CBORType.Number) {
        throw new ArgumentException("a.Type (" + a.Type +
          ") is not equal to " + CBORType.Number);
      }
      if (b.Type != CBORType.Number) {
        throw new ArgumentException("b.Type (" + b.Type +
          ") is not equal to " + CBORType.Number);
      }
      object objA = a.ThisItem;
      object objB = b.ThisItem;
      int typeA = a.ItemType;
      int typeB = b.ItemType;
      if (typeA == CBORObject.CBORObjectTypeInteger && typeB ==
      CBORObject.CBORObjectTypeInteger) {
        var valueA = (long)objA;
        var valueB = (long)objB;
        if (valueB == 0) {
          return (valueA == 0) ? CBORObject.NaN : ((valueA < 0) ?
            CBORObject.NegativeInfinity : CBORObject.PositiveInfinity);
        }
        if (valueA == Int64.MinValue && valueB == -1) {
          return CBORObject.FromObject(valueA).Negate();
        }
        long quo = valueA / valueB;
        long rem = valueA - (quo * valueB);
        return (rem == 0) ? CBORObject.FromObject(quo) :
        CBORObject.FromObject(
new ExtendedRational(
(BigInteger)valueA,
(BigInteger)valueB));
      }
      if (typeA == CBORObject.CBORObjectTypeExtendedRational ||
             typeB == CBORObject.CBORObjectTypeExtendedRational) {
        ExtendedRational e1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedRational(objA);
        ExtendedRational e2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedRational(objB);
        return CBORObject.FromObject(e1.Divide(e2));
      }
      if (typeA == CBORObject.CBORObjectTypeExtendedDecimal ||
             typeB == CBORObject.CBORObjectTypeExtendedDecimal) {
        ExtendedDecimal e1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedDecimal(objA);
        ExtendedDecimal e2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedDecimal(objB);
        if (e1.IsZero && e2.IsZero) {
          return CBORObject.NaN;
        }
        ExtendedDecimal eret = e1.Divide(e2, null);
        // If either operand is infinity or NaN, the result
        // is already exact. Likewise if the result is a finite number.
        if (!e1.IsFinite || !e2.IsFinite || eret.IsFinite) {
          return CBORObject.FromObject(eret);
        }
        ExtendedRational er1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedRational(objA);
        ExtendedRational er2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedRational(objB);
        return CBORObject.FromObject(er1.Divide(er2));
      }
      if (typeA == CBORObject.CBORObjectTypeExtendedFloat || typeB ==
      CBORObject.CBORObjectTypeExtendedFloat ||
               typeA == CBORObject.CBORObjectTypeDouble || typeB ==
               CBORObject.CBORObjectTypeDouble ||
               typeA == CBORObject.CBORObjectTypeSingle || typeB ==
               CBORObject.CBORObjectTypeSingle) {
        ExtendedFloat e1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedFloat(objA);
        ExtendedFloat e2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedFloat(objB);
        if (e1.IsZero && e2.IsZero) {
          return CBORObject.NaN;
        }
        ExtendedFloat eret = e1.Divide(e2, null);
        // If either operand is infinity or NaN, the result
        // is already exact. Likewise if the result is a finite number.
        if (!e1.IsFinite || !e2.IsFinite || eret.IsFinite) {
          return CBORObject.FromObject(eret);
        }
        ExtendedRational er1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedRational(objA);
        ExtendedRational er2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedRational(objB);
        return CBORObject.FromObject(er1.Divide(er2));
      } else {
        BigInteger b1 = CBORObject.GetNumberInterface(typeA).AsBigInteger(objA);
        BigInteger b2 = CBORObject.GetNumberInterface(typeB).AsBigInteger(objB);
        if (b2.IsZero) {
          return b1.IsZero ? CBORObject.NaN : ((b1.Sign < 0) ?
            CBORObject.NegativeInfinity : CBORObject.PositiveInfinity);
        }
        BigInteger bigrem;
        BigInteger bigquo = BigInteger.DivRem(b1, b2, out bigrem);
        return bigrem.IsZero ? CBORObject.FromObject(bigquo) :
        CBORObject.FromObject(new ExtendedRational(b1, b2));
      }
    }

    public static CBORObject Remainder(CBORObject a, CBORObject b) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      if (a.Type != CBORType.Number) {
        throw new ArgumentException("a.Type (" + a.Type +
          ") is not equal to " + CBORType.Number);
      }
      if (b.Type != CBORType.Number) {
        throw new ArgumentException("b.Type (" + b.Type +
          ") is not equal to " + CBORType.Number);
      }
      object objA = a.ThisItem;
      object objB = b.ThisItem;
      int typeA = a.ItemType;
      int typeB = b.ItemType;
      if (typeA == CBORObject.CBORObjectTypeInteger && typeB ==
      CBORObject.CBORObjectTypeInteger) {
        var valueA = (long)objA;
        var valueB = (long)objB;
        return (valueA == Int64.MinValue && valueB == -1) ?
        CBORObject.FromObject(0) : CBORObject.FromObject(valueA % valueB);
      }
      if (typeA == CBORObject.CBORObjectTypeExtendedRational ||
             typeB == CBORObject.CBORObjectTypeExtendedRational) {
        ExtendedRational e1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedRational(objA);
        ExtendedRational e2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedRational(objB);
        return CBORObject.FromObject(e1.Remainder(e2));
      }
      if (typeA == CBORObject.CBORObjectTypeExtendedDecimal ||
             typeB == CBORObject.CBORObjectTypeExtendedDecimal) {
        ExtendedDecimal e1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedDecimal(objA);
        ExtendedDecimal e2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedDecimal(objB);
        return CBORObject.FromObject(e1.Remainder(e2, null));
      }
      if (typeA == CBORObject.CBORObjectTypeExtendedFloat || typeB ==
      CBORObject.CBORObjectTypeExtendedFloat ||
               typeA == CBORObject.CBORObjectTypeDouble || typeB ==
               CBORObject.CBORObjectTypeDouble ||
               typeA == CBORObject.CBORObjectTypeSingle || typeB ==
               CBORObject.CBORObjectTypeSingle) {
        ExtendedFloat e1 =
        CBORObject.GetNumberInterface(typeA).AsExtendedFloat(objA);
        ExtendedFloat e2 =
        CBORObject.GetNumberInterface(typeB).AsExtendedFloat(objB);
        return CBORObject.FromObject(e1.Remainder(e2, null));
      } else {
        BigInteger b1 = CBORObject.GetNumberInterface(typeA).AsBigInteger(objA);
        BigInteger b2 = CBORObject.GetNumberInterface(typeB).AsBigInteger(objB);
        return CBORObject.FromObject(b1 % (BigInteger)b2);
      }
    }
  }
}
