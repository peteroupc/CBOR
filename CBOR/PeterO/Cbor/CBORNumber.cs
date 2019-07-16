using System;
using PeterO;
namespace PeterO.Cbor {
  internal sealed class CBORNumber {
    internal enum Kind {
      Integer, IEEEBinary64, EInteger,
      EDecimal, EFloat, ERational
    }
    private readonly int kind;
    private readonly object value;
    public CBORNumber(Kind kind, object value) {
      this.kind=kind;
      this.value=value;
    }

/*
    /// <summary>Compares two CBOR numbers.
    /// In this implementation, the two numbers' mathematical values are
    /// compared. Here, NaN (not-a-number) is considered greater than any
    /// number.  This method is not consistent with the Equals
    /// method.</summary>
    /// <param name='other'>A value to compare with.</param>
    /// <returns>Less than 0, if this value is less than the other object;
    /// or 0, if both values are equal; or greater than 0, if this value is
    /// less than the other object or if the other object is
    /// null.</returns>
    /// <exception cref='ArgumentException'>An internal error
    /// occurred.</exception>
*/
    public int CompareTo(CBORObject other) {
      if (other == null) {
        return 1;
      }
      if (this == other) {
        return 0;
      }
      int typeA = (int)this.kind;
      int typeB = (int)other.kind;
      object objA = this.value;
      object objB = other.value;
      if (typeA == typeB) {
        switch (typeA) {
          case Kind.Integer: {
              var a = (long)objA;
              var b = (long)objB;
              cmp = (a == b) ? 0 : ((a < b) ? -1 : 1);
              break;
            }
          case Kind.Single: {
              var a = (float)objA;
              var b = (float)objB;
              // Treat NaN as greater than all other numbers
              cmp = Single.IsNaN(a) ? (Single.IsNaN(b) ? 0 : 1) :
                (Single.IsNaN(b) ? (-1) : ((a == b) ? 0 : ((a < b) ? -1 :
                    1)));
              break;
            }
          case Kind.EInteger: {
              var bigintA = (EInteger)objA;
              var bigintB = (EInteger)objB;
              cmp = bigintA.CompareTo(bigintB);
              break;
            }
          case Kind.Double: {
              var a = (double)objA;
              var b = (double)objB;
              // Treat NaN as greater than all other numbers
              cmp = Double.IsNaN(a) ? (Double.IsNaN(b) ? 0 : 1) :
                (Double.IsNaN(b) ? (-1) : ((a == b) ? 0 : ((a < b) ? -1 :
                    1)));
              break;
            }
          case Kind.EDecimal: {
              cmp = ((EDecimal)objA).CompareTo((EDecimal)objB);
              break;
            }
          case Kind.EFloat: {
              cmp = ((EFloat)objA).CompareTo(
                (EFloat)objB);
              break;
            }
          case Kind.ERational: {
              cmp = ((ERational)objA).CompareTo(
                (ERational)objB);
              break;
            }
          default: throw new ArgumentException("Unexpected data type");
        }
      } else {
        int s1 = CBORObject.GetSignInternal(typeA, objA);
        int s2 = CBORObject.GetSignInternal(typeB, objB);
        if (s1 != s2 && s1 != 2 && s2 != 2) {
          // if both types are numbers
          // and their signs are different
          return (s1 < s2) ? -1 : 1;
        }
        if (s1 == 2 && s2 == 2) {
          // both are NaN
          cmp = 0;
        } else if (s1 == 2) {
          // first object is NaN
          return 1;
        } else if (s2 == 2) {
          // second object is NaN
          return -1;
        } else {
          // DebugUtility.Log("a=" + this + " b=" + other);
          if (typeA == Kind.ERational) {
            ERational e1 = CBORObject.GetNumberInterface(typeA).AsExtendedRational(objA);
            if (typeB == Kind.EDecimal) {
              EDecimal e2 = CBORObject.GetNumberInterface(typeB).AsExtendedDecimal(objB);
              cmp = e1.CompareToDecimal(e2);
            } else {
              EFloat e2 = CBORObject.GetNumberInterface(typeB).AsExtendedFloat(objB);
              cmp = e1.CompareToBinary(e2);
            }
          } else if (typeB == Kind.ERational) {
            ERational e2 = CBORObject.GetNumberInterface(typeB).AsExtendedRational(objB);
            if (typeA == Kind.EDecimal) {
              EDecimal e1 = CBORObject.GetNumberInterface(typeA).AsExtendedDecimal(objA);
              cmp = e2.CompareToDecimal(e1);
              cmp = -cmp;
            } else {
              EFloat e1 = CBORObject.GetNumberInterface(typeA).AsExtendedFloat(objA);
              cmp = e2.CompareToBinary(e1);
              cmp = -cmp;
            }
          } else if (typeA == Kind.EDecimal ||
                    typeB == Kind.EDecimal) {
            EDecimal e1 = null;
            EDecimal e2 = null;
            if (typeA == Kind.EFloat) {
              var ef1 = (EFloat)objA;
              e2 = (EDecimal)objB;
              cmp = e2.CompareToBinary(ef1);
              cmp = -cmp;
            } else if (typeB == Kind.EFloat) {
              var ef1 = (EFloat)objB;
              e2 = (EDecimal)objA;
              cmp = e2.CompareToBinary(ef1);
            } else {
              e1 = CBORObject.GetNumberInterface(typeA).AsExtendedDecimal(objA);
              e2 = CBORObject.GetNumberInterface(typeB).AsExtendedDecimal(objB);
              cmp = e1.CompareTo(e2);
            }
          } else if (typeA == Kind.EFloat || typeB ==
                    Kind.EFloat ||
                    typeA == Kind.Double || typeB ==
                    Kind.Double ||
                    typeA == Kind.Single || typeB ==
                    Kind.Single) {
            EFloat e1 = CBORObject.GetNumberInterface(typeA).AsExtendedFloat(objA);
            EFloat e2 = CBORObject.GetNumberInterface(typeB).AsExtendedFloat(objB);
            cmp = e1.CompareTo(e2);
          } else {
            EInteger b1 = CBORObject.GetNumberInterface(typeA).AsEInteger(objA);
            EInteger b2 = CBORObject.GetNumberInterface(typeB).AsEInteger(objB);
            cmp = b1.CompareTo(b2);
          }
        }
      }
      return cmp;
    }
  }
}
