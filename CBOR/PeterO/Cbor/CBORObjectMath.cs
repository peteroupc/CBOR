/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using PeterO;
using PeterO.Numbers;

namespace PeterO.Cbor {
    /// <summary>Implements arithmetic operations with CBOR
    /// objects.</summary>
  internal static class CBORObjectMath {
    // TODO: Move these methods to CBORNumber class
    public static CBORObject Remainder(CBORObject a, CBORObject b) {
      throw new NotImplementedException();
/*
      object objA = a.ThisItem;
      object objB = b.ThisItem;
      int typeA = a.ItemType;
      int typeB = b.ItemType;
      if (typeA == Kind.Integer && typeB == Kind.Integer) {
        var valueA = (long)objA;
        var valueB = (long)objB;
        return (valueA == Int64.MinValue && valueB == -1) ?
        CBORObject.FromObject(0) : CBORObject.FromObject(valueA % valueB);
      }
      if (typeA == Kind.ERational ||
             typeB == Kind.ERational) {
        ERational e1 =
        GetNumberInterface(typeA).AsExtendedRational(objA);
        ERational e2 = GetNumberInterface(typeB).AsExtendedRational(objB);
        return CBORObject.FromObject(e1.Remainder(e2));
      }
      if (typeA == Kind.EDecimal ||
             typeB == Kind.EDecimal) {
        EDecimal e1 =
        GetNumberInterface(typeA).AsExtendedDecimal(objA);
        EDecimal e2 = GetNumberInterface(typeB).AsExtendedDecimal(objB);
        return CBORObject.FromObject(e1.Remainder(e2, null));
      }
      if (typeA == Kind.EFloat ||
               typeB == Kind.EFloat || typeA == Kind.IEEEBinary64 || typeB ==
               Kind.IEEEBinary64) {
        EFloat e1 =
        GetNumberInterface(typeA).AsExtendedFloat(objA);
        EFloat e2 = GetNumberInterface(typeB).AsExtendedFloat(objB);
        return CBORObject.FromObject(e1.Remainder(e2, null));
      } else {
        EInteger b1 = GetNumberInterface(typeA).AsEInteger(objA);
        EInteger b2 = GetNumberInterface(typeB).AsEInteger(objB);
        return CBORObject.FromObject(b1 % (EInteger)b2);
      }
*/
    }
  }
}
