/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
// using System.Numerics;
using System.Text;

namespace PeterO {
    /// <summary>Represents an object in Concise Binary Object Representation
    /// (CBOR) and contains methods for reading and writing CBOR data. CBOR
    /// is defined in RFC 7049. <para>There are many ways to get a CBOR object,
    /// including from bytes, objects, streams and JSON, as described below.</para>
    /// <para> <b>To and from byte arrays:</b>
    /// The CBORObject.DecodeToBytes method converts a byte array to a CBOR
    /// object. The EncodeToBytes method converts a CBOR object to its corresponding
    /// byte array. </para>
    /// <para> <b>To and from data streams:</b>
    /// The CBORObject.Write methods write many kinds of objects to a data
    /// stream, including numbers, CBOR objects, strings, and arrays of
    /// numbers and strings. The CBORObject.Read method reads a CBOR object
    /// from a data stream. </para>
    /// <para> <b>To and from other objects:</b>
    /// The CBORObject.FromObject methods converts many kinds of objects
    /// to a CBOR object, including numbers, strings, and arrays and maps
    /// of numbers and strings. Methods like AsDouble, AsByte, and AsString
    /// convert a CBOR object to different types of object. </para>
    /// <para> <b>To and from JSON:</b>
    /// This class also doubles as a reader and writer of JavaScript Object
    /// Notation (JSON). The CBORObject.FromJSONString method converts
    /// JSON to a CBOR object, and the ToJSONString method converts a CBOR
    /// object to a JSON string. </para>
    /// <para> Thread Safety: CBOR objects that are numbers, "simple values",
    /// and text strings are immutable (their values can't be changed), so
    /// they are inherently safe for use by multiple threads. CBOR objects
    /// that are arrays, maps, and byte strings are mutable, but this class
    /// doesn't attempt to synchronize reads and writes to those objects
    /// by multiple threads, so those objects are not thread safe without
    /// such synchronization. </para>
    /// </summary>
  public sealed partial class CBORObject : IComparable<CBORObject>, IEquatable<CBORObject> {
    internal int ItemType {
      get {
        CBORObject curobject = this;
        while (curobject.itemtypeValue == CBORObjectTypeTagged) {
          curobject = (CBORObject)curobject.itemValue;
        }
        return curobject.itemtypeValue;
      }
    }

    internal object ThisItem {
      get {
        CBORObject curobject = this;
        while (curobject.itemtypeValue == CBORObjectTypeTagged) {
          curobject = (CBORObject)curobject.itemValue;
        }
        return curobject.itemValue;
      }
    }

    private int itemtypeValue;
    private object itemValue;
    private int tagLow;
    private int tagHigh;
    internal const int CBORObjectTypeInteger = 0;  // -(2^63).. (2^63-1)
    internal const int CBORObjectTypeBigInteger = 1;  // all other integers
    internal const int CBORObjectTypeByteString = 2;
    internal const int CBORObjectTypeTextString = 3;
    internal const int CBORObjectTypeArray = 4;
    internal const int CBORObjectTypeMap = 5;
    internal const int CBORObjectTypeSimpleValue = 6;
    internal const int CBORObjectTypeSingle = 7;
    internal const int CBORObjectTypeDouble = 8;
    internal const int CBORObjectTypeExtendedDecimal = 9;
    internal const int CBORObjectTypeTagged = 10;
    internal const int CBORObjectTypeExtendedFloat = 11;
    private static readonly BigInteger Int64MaxValue = (BigInteger)Int64.MaxValue;
    private static readonly BigInteger Int64MinValue = (BigInteger)Int64.MinValue;

    /// <summary>Represents the value false.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="This CBORObject is immutable")]
#endif

    public static readonly CBORObject False = new CBORObject(CBORObjectTypeSimpleValue, 20);

    /// <summary>Represents the value true.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="This CBORObject is immutable")]
#endif

    public static readonly CBORObject True = new CBORObject(CBORObjectTypeSimpleValue, 21);

    /// <summary>Represents the value null.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="This CBORObject is immutable")]
#endif

    public static readonly CBORObject Null = new CBORObject(CBORObjectTypeSimpleValue, 22);

    /// <summary>Represents the value undefined.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="This CBORObject is immutable")]
#endif

    public static readonly CBORObject Undefined = new CBORObject(CBORObjectTypeSimpleValue, 23);

    private CBORObject() {
    }

    private CBORObject(CBORObject obj, int tagLow, int tagHigh) :
      this(CBORObjectTypeTagged, obj) {
      this.tagLow = tagLow;
      this.tagHigh = tagHigh;
    }

    private CBORObject(int type, object item) {
#if DEBUG
      // Check range in debug mode to ensure that Integer and BigInteger
      // are unambiguous
      if ((type == CBORObjectTypeBigInteger) &&
          ((BigInteger)item).CompareTo(Int64MinValue) >= 0 &&
          ((BigInteger)item).CompareTo(Int64MaxValue) <= 0) {
        if (!false) {
          throw new ArgumentException("Big integer is within range for Integer");
        }
      }
#endif
      this.itemtypeValue = type;
      this.itemValue = item;
    }

    /// <summary>Gets the general data type of this CBOR object.</summary>
    /// <value>The general data type of this CBOR object.</value>
    public CBORType Type {
      get {
        switch (this.ItemType) {
          case CBORObjectTypeInteger:
          case CBORObjectTypeBigInteger:
          case CBORObjectTypeSingle:
          case CBORObjectTypeDouble:
          case CBORObjectTypeExtendedDecimal:
          case CBORObjectTypeExtendedFloat:
            return CBORType.Number;
          case CBORObjectTypeSimpleValue:
            if ((int)this.ThisItem == 21 || (int)this.ThisItem == 20) {
              return CBORType.Boolean;
            }
            return CBORType.SimpleValue;
          case CBORObjectTypeArray:
            return CBORType.Array;
          case CBORObjectTypeMap:
            return CBORType.Map;
          case CBORObjectTypeByteString:
            return CBORType.ByteString;
          case CBORObjectTypeTextString:
            return CBORType.TextString;
          default:
            throw new InvalidOperationException("Unexpected data type");
        }
      }
    }

    /// <summary>Gets a value indicating whether this value is a CBOR true
    /// value.</summary>
    /// <value>Whether this value is a CBOR true value.</value>
    public bool IsTrue {
      get {
        return this.ItemType == CBORObjectTypeSimpleValue && (int)this.ThisItem == 21;
      }
    }

    /// <summary>Gets a value indicating whether this value is a CBOR false
    /// value.</summary>
    /// <value>Whether this value is a CBOR false value.</value>
    public bool IsFalse {
      get {
        return this.ItemType == CBORObjectTypeSimpleValue && (int)this.ThisItem == 20;
      }
    }

    /// <summary>Gets a value indicating whether this value is a CBOR null
    /// value.</summary>
    /// <value>Whether this value is a CBOR null value.</value>
    public bool IsNull {
      get {
        return this.ItemType == CBORObjectTypeSimpleValue && (int)this.ThisItem == 22;
      }
    }

    /// <summary>Gets a value indicating whether this value is a CBOR undefined
    /// value.</summary>
    /// <value>Whether this value is a CBOR undefined value.</value>
    public bool IsUndefined {
      get {
        return this.ItemType == CBORObjectTypeSimpleValue && (int)this.ThisItem == 23;
      }
    }

    /// <summary>Gets a value indicating whether this object&apos;s value
    /// equals 0.</summary>
    /// <value>Whether this object&apos;s value equals 0.</value>
    public bool IsZero {
      get {
        switch (this.ItemType) {
          case CBORObject.CBORObjectTypeInteger:
            return ((long)this.ThisItem) == 0;
          case CBORObject.CBORObjectTypeBigInteger:
            return ((BigInteger)this.ThisItem).IsZero;
          case CBORObject.CBORObjectTypeSingle:
            return ((float)this.ThisItem) == 0;
          case CBORObject.CBORObjectTypeDouble:
            return ((double)this.ThisItem) == 0;
          case CBORObject.CBORObjectTypeExtendedDecimal:
            return ((ExtendedDecimal)this.ThisItem).IsZero;
          case CBORObject.CBORObjectTypeExtendedFloat:
            return ((ExtendedFloat)this.ThisItem).IsZero;
          default:
            return false;
        }
      }
    }

    /// <summary>Gets this object&apos;s value with the sign reversed.</summary>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    /// <returns>A CBORObject object.</returns>
    public CBORObject Negate() {
      switch (this.ItemType) {
        case CBORObject.CBORObjectTypeInteger:
          if (((long)this.ThisItem) == Int64.MinValue) {
            return CBORObject.FromObject(valueOneShift63);
          }
          return CBORObject.FromObject(-((long)this.ThisItem));
        case CBORObject.CBORObjectTypeBigInteger: {
            BigInteger bigint = (BigInteger)this.ThisItem;
            bigint = -bigint;
            return CBORObject.FromObject(bigint);
          }
        case CBORObject.CBORObjectTypeSingle:
          return CBORObject.FromObject(-((float)this.ThisItem));
        case CBORObject.CBORObjectTypeDouble:
          return CBORObject.FromObject(-((double)this.ThisItem));
        case CBORObject.CBORObjectTypeExtendedDecimal:
          return CBORObject.FromObject(((ExtendedDecimal)this.ThisItem).Negate());
        case CBORObject.CBORObjectTypeExtendedFloat:
          return CBORObject.FromObject(((ExtendedFloat)this.ThisItem).Negate());
        default:
          throw new InvalidOperationException("This object is not a number.");
      }
    }

    private static int GetSignInternal(int type, object obj, bool returnOnNaN) {
      switch (type) {
        case CBORObject.CBORObjectTypeInteger: {
            long value = (long)obj;
            return (value == 0) ? 0 : ((value < 0) ? -1 : 1);
          }
        case CBORObject.CBORObjectTypeBigInteger:
          return ((BigInteger)obj).Sign;
        case CBORObject.CBORObjectTypeSingle: {
            float value = (float)obj;
            if (Single.IsNaN(value)) {
              if (returnOnNaN) {
                return 2;
              } else {
                throw new InvalidOperationException("This object is not a number.");
              }
            }
            return (int)Math.Sign(value);
          }
        case CBORObject.CBORObjectTypeDouble: {
            double value = (double)obj;
            if (Double.IsNaN(value)) {
              if (returnOnNaN) {
                return 2;
              } else {
                throw new InvalidOperationException("This object is not a number.");
              }
            }
            return (int)Math.Sign(value);
          }
        case CBORObject.CBORObjectTypeExtendedDecimal:
          return ((ExtendedDecimal)obj).Sign;
        case CBORObject.CBORObjectTypeExtendedFloat:
          return ((ExtendedFloat)obj).Sign;
        default:
          if (returnOnNaN) {
            return 2;  // not a number type
          } else {
            throw new InvalidOperationException("This object is not a number.");
          }
      }
    }

    /// <summary>Gets this value&apos;s sign: -1 if negative; 1 if positive;
    /// 0 if zero.</summary>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type, including the special not-a-number value
    /// (NaN).</exception>
    /// <value>This value&apos;s sign: -1 if negative; 1 if positive; 0 if
    /// zero.</value>
    public int Sign {
      get {
        return GetSignInternal(this.ItemType, this.ThisItem, false);
      }
    }

    /// <summary>Gets a value indicating whether this CBOR object represents
    /// positive infinity.</summary>
    /// <returns>A Boolean object.</returns>
    public bool IsPositiveInfinity() {
      switch (this.ItemType) {
        case CBORObject.CBORObjectTypeSingle: {
            float value = (float)this.ThisItem;
            return Single.IsInfinity(value) && value > 0;
          }
        case CBORObject.CBORObjectTypeDouble: {
            double value = (double)this.ThisItem;
            return Double.IsInfinity(value) && value > 0;
          }
        case CBORObject.CBORObjectTypeExtendedDecimal:
          return ((ExtendedDecimal)this.ThisItem).IsPositiveInfinity();
        case CBORObject.CBORObjectTypeExtendedFloat:
          return ((ExtendedFloat)this.ThisItem).IsPositiveInfinity();
        default:
          return false;
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
    public bool IsInfinity() {
      return this.IsPositiveInfinity() || this.IsNegativeInfinity();
    }

    /// <summary>Gets a value indicating whether this CBOR object represents
    /// negative infinity.</summary>
    /// <returns>A Boolean object.</returns>
    public bool IsNegativeInfinity() {
      switch (this.ItemType) {
        case CBORObject.CBORObjectTypeSingle: {
            float value = (float)this.ThisItem;
            return Single.IsInfinity(value) && value < 0;
          }
        case CBORObject.CBORObjectTypeDouble: {
            double value = (double)this.ThisItem;
            return Double.IsInfinity(value) && value < 0;
          }
        case CBORObject.CBORObjectTypeExtendedDecimal:
          return ((ExtendedDecimal)this.ThisItem).IsNegativeInfinity();
        case CBORObject.CBORObjectTypeExtendedFloat:
          return ((ExtendedFloat)this.ThisItem).IsNegativeInfinity();
        default:
          return false;
      }
    }

    /// <summary>Gets a value indicating whether this CBOR object represents
    /// a not-a-number value (as opposed to whether this object&apos;s type
    /// is not a number type).</summary>
    /// <returns>A Boolean object.</returns>
    public bool IsNaN() {
      switch (this.ItemType) {
        case CBORObject.CBORObjectTypeSingle:
          return Single.IsNaN((float)this.ThisItem);
        case CBORObject.CBORObjectTypeDouble:
          return Double.IsNaN((double)this.ThisItem);
        case CBORObject.CBORObjectTypeExtendedDecimal:
          return ((ExtendedDecimal)this.ThisItem).IsNaN();
        case CBORObject.CBORObjectTypeExtendedFloat:
          return ((ExtendedFloat)this.ThisItem).IsNaN();
        default:
          return false;
      }
    }

    /// <summary>Compares two CBOR objects.<para> In this implementation:</para>
    /// <list type=''> <item> If either value is true, it is treated as the
    /// number 1.</item>
    /// <item> If either value is false, CBORObject.Null, or the undefined
    /// value, it is treated as the number 0.</item>
    /// <item> If both objects are numbers, their mathematical values are
    /// compared. Here, NaN (not-a-number) is considered greater than any
    /// number.</item>
    /// <item> If both objects are arrays, each element is compared. If one
    /// array is shorter than the other and the other array begins with that
    /// array (for the purposes of comparison), the shorter array is considered
    /// less than the longer array.</item>
    /// <item> If both objects are strings, compares each string code-point
    /// by code-point.</item>
    /// <item> If both objects are maps, returns 0.</item>
    /// <item> If each object is a different type, then they are sorted by their
    /// type number, in the order given for the CBORType enumeration.</item>
    /// <para> This method is not consistent with the Equals method.</para>
    /// </list>
    /// </summary>
    /// <param name='other'>A value to compare with.</param>
    /// <returns>Less than 0, if this value is less than the other object;
    /// or 0, if both values are equal; or greater than 0, if this value is less
    /// than the other object or if the other object is null.</returns>
    public int CompareTo(CBORObject other) {
      if (other == null) {
        return 1;
      }
      int typeA = this.ItemType;
      int typeB = other.ItemType;
      object objA = this.ThisItem;
      object objB = other.ThisItem;
      if (typeA == CBORObjectTypeSimpleValue) {
        if ((int)objA == 20 || (int)objA == 22 || (int)objA == 23) {
          // Treat false, null, and undefined
          // as the number 0
          objA = (long)0;
          typeA = CBORObjectTypeInteger;
        } else if ((int)objA == 21) {
          // Treat true as the number 1
          objA = (long)1;
          typeA = CBORObjectTypeInteger;
        }
      }
      if (typeB == CBORObjectTypeSimpleValue) {
        if ((int)objB == 20 || (int)objB == 22 || (int)objB == 23) {
          // Treat false, null, and undefined
          // as the number 0
          objB = (long)0;
          typeB = CBORObjectTypeInteger;
        } else if ((int)objB == 21) {
          // Treat true as the number 1
          objB = (long)1;
          typeB = CBORObjectTypeInteger;
        }
      }
      if (typeA == typeB) {
        switch (typeA) {
          case CBORObjectTypeInteger: {
              long a = (long)objA;
              long b = (long)objB;
              if (a == b) {
                return 0;
              }
              return (a < b) ? -1 : 1;
            }
          case CBORObjectTypeSingle: {
              float a = (float)objA;
              float b = (float)objB;
              // Treat NaN as greater than all other numbers
              if (Single.IsNaN(a)) {
                return Single.IsNaN(b) ? 0 : 1;
              }
              if (Single.IsNaN(b)) {
                return -1;
              }
              if (a == b) {
                return 0;
              }
              return (a < b) ? -1 : 1;
            }
          case CBORObjectTypeBigInteger: {
              BigInteger bigintA = (BigInteger)objA;
              BigInteger bigintB = (BigInteger)objB;
              return bigintA.CompareTo(bigintB);
            }
          case CBORObjectTypeDouble: {
              double a = (double)objA;
              double b = (double)objB;
              // Treat NaN as greater than all other numbers
              if (Double.IsNaN(a)) {
                return Double.IsNaN(b) ? 0 : 1;
              }
              if (Double.IsNaN(b)) {
                return -1;
              }
              if (a == b) {
                return 0;
              }
              return (a < b) ? -1 : 1;
            }
          case CBORObjectTypeExtendedDecimal: {
              return ((ExtendedDecimal)objA).CompareTo(
                (ExtendedDecimal)objB);
            }
          case CBORObjectTypeExtendedFloat: {
              return ((ExtendedFloat)objA).CompareTo(
                (ExtendedFloat)objB);
            }
          case CBORObjectTypeByteString: {
              return CBORUtilities.ByteArrayCompare((byte[])objA, (byte[])objB);
            }
          case CBORObjectTypeTextString: {
              return DataUtilities.CodePointCompare(
                (string)objA,
                (string)objB);
            }
          case CBORObjectTypeArray: {
              return ListCompare(
                (List<CBORObject>)objA,
                (List<CBORObject>)objB);
            }
          case CBORObjectTypeMap: {
              return 0;
            }
          case CBORObjectTypeSimpleValue: {
              int valueA = (int)objA;
              int valueB = (int)objB;
              if (valueA == valueB) {
                return 0;
              }
              return (valueA < valueB) ? -1 : 1;
            }
          default:
            throw new ArgumentException("Unexpected data type");
        }
      } else {
        int combo = (typeA << 4) | typeB;
        int s1 = GetSignInternal(typeA, objA, true);
        int s2 = GetSignInternal(typeB, objB, true);
        if (s1 != s2 && s1 != 2 && s2 != 2) {
          // if both types are numbers
          // and their signs are different
          return (s1 < s2) ? -1 : 1;
        }
        BigInteger bigintXa;
        BigInteger bigintXb;
        ExtendedFloat bigfloatXa;
        ExtendedFloat bigfloatXb;
        ExtendedDecimal decfracXa;
        ExtendedDecimal decfracXb;
        switch (combo) {
          case (CBORObjectTypeInteger << 4) | CBORObjectTypeBigInteger: {
              bigintXa = (BigInteger)(long)objA;
              bigintXb = (BigInteger)objB;
              return bigintXa.CompareTo(bigintXb);
            }
          case (CBORObjectTypeInteger << 4) | CBORObjectTypeSingle: {
              float sf = (float)objB;
              if (Single.IsInfinity(sf)) {
                return sf < 0 ? 1 : -1;
              }
              if (Single.IsNaN(sf)) {
                return -1;
              }
              bigfloatXa = ExtendedFloat.FromInt64((long)objA);
              bigfloatXb = ExtendedFloat.FromSingle(sf);
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeInteger << 4) | CBORObjectTypeDouble: {
              double sf = (double)objB;
              if (Double.IsInfinity(sf)) {
                return sf < 0 ? 1 : -1;
              }
              if (Double.IsNaN(sf)) {
                return -1;
              }
              bigfloatXa = ExtendedFloat.FromInt64((long)objA);
              bigfloatXb = ExtendedFloat.FromDouble(sf);
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeInteger << 4) | CBORObjectTypeExtendedDecimal: {
              decfracXa = ExtendedDecimal.FromInt64((long)objA);
              decfracXb = (ExtendedDecimal)objB;
              return decfracXa.CompareTo(decfracXb);
            }
          case (CBORObjectTypeInteger << 4) | CBORObjectTypeExtendedFloat: {
              bigfloatXa = ExtendedFloat.FromInt64((long)objA);
              bigfloatXb = (ExtendedFloat)objB;
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeBigInteger << 4) | CBORObjectTypeInteger: {
              bigintXa = (BigInteger)objA;
              bigintXb = (BigInteger)(long)objB;
              return bigintXa.CompareTo(bigintXb);
            }
          case (CBORObjectTypeBigInteger << 4) | CBORObjectTypeSingle: {
              float sf = (float)objB;
              if (Single.IsInfinity(sf)) {
                return sf < 0 ? 1 : -1;
              }
              if (Single.IsNaN(sf)) {
                return -1;
              }
              bigfloatXa = ExtendedFloat.FromBigInteger((BigInteger)objA);
              bigfloatXb = ExtendedFloat.FromSingle(sf);
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeBigInteger << 4) | CBORObjectTypeDouble: {
              double sf = (double)objB;
              if (Double.IsInfinity(sf)) {
                return sf < 0 ? 1 : -1;
              }
              if (Double.IsNaN(sf)) {
                return -1;
              }
              bigfloatXa = ExtendedFloat.FromBigInteger((BigInteger)objA);
              bigfloatXb = ExtendedFloat.FromDouble(sf);
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeBigInteger << 4) | CBORObjectTypeExtendedDecimal: {
              decfracXa = ExtendedDecimal.FromBigInteger((BigInteger)objA);
              decfracXb = (ExtendedDecimal)objB;
              return decfracXa.CompareTo(decfracXb);
            }
          case (CBORObjectTypeBigInteger << 4) | CBORObjectTypeExtendedFloat: {
              bigfloatXa = ExtendedFloat.FromBigInteger((BigInteger)objA);
              bigfloatXb = (ExtendedFloat)objB;
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeSingle << 4) | CBORObjectTypeInteger: {
              float sf = (float)objA;
              if (Single.IsInfinity(sf)) {
                return sf < 0 ? -1 : 1;
              }

              if (Single.IsNaN(sf)) {
                return 1;
              }
              bigfloatXa = ExtendedFloat.FromSingle(sf);
              bigfloatXb = ExtendedFloat.FromInt64((long)objB);
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeSingle << 4) | CBORObjectTypeBigInteger: {
              float sf = (float)objA;
              if (Single.IsInfinity(sf)) {
                return sf < 0 ? -1 : 1;
              }
              if (Single.IsNaN(sf)) {
                return 1;
              }
              bigfloatXa = ExtendedFloat.FromSingle(sf);
              bigfloatXb = ExtendedFloat.FromBigInteger((BigInteger)objB);
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeSingle << 4) | CBORObjectTypeDouble: {
              double a = (double)(float)objA;
              double b = (double)objB;
              // Treat NaN as greater than all other numbers
              if (Double.IsNaN(a)) {
                return Double.IsNaN(b) ? 0 : 1;
              }
              if (Double.IsNaN(b)) {
                return -1;
              }
              if (a == b) {
                return 0;
              }
              return (a < b) ? -1 : 1;
            }
          case (CBORObjectTypeSingle << 4) | CBORObjectTypeExtendedDecimal: {
              float sf = (float)objA;
              if (Single.IsInfinity(sf)) {
                return sf < 0 ? -1 : 1;
              }
              if (Single.IsNaN(sf)) {
                return 1;
              }
              decfracXa = ExtendedDecimal.FromSingle(sf);
              decfracXb = (ExtendedDecimal)objB;
              return decfracXa.CompareTo(decfracXb);
            }
          case (CBORObjectTypeSingle << 4) | CBORObjectTypeExtendedFloat: {
              float sf = (float)objA;
              if (Single.IsInfinity(sf)) {
                return sf < 0 ? -1 : 1;
              }
              if (Single.IsNaN(sf)) {
                return 1;
              }
              bigfloatXa = ExtendedFloat.FromSingle(sf);
              bigfloatXb = (ExtendedFloat)objB;
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeDouble << 4) | CBORObjectTypeInteger: {
              double sf = (double)objA;
              if (Double.IsInfinity(sf)) {
                return sf < 0 ? -1 : 1;
              }
              if (Double.IsNaN(sf)) {
                return 1;
              }
              bigfloatXa = ExtendedFloat.FromDouble(sf);
              bigfloatXb = ExtendedFloat.FromInt64((long)objB);
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeDouble << 4) | CBORObjectTypeBigInteger: {
              double sf = (double)objA;
              if (Double.IsInfinity(sf)) {
                return sf < 0 ? -1 : 1;
              }
              if (Double.IsNaN(sf)) {
                return 1;
              }
              bigfloatXa = ExtendedFloat.FromDouble(sf);
              bigfloatXb = ExtendedFloat.FromBigInteger((BigInteger)objB);
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeDouble << 4) | CBORObjectTypeSingle: {
              double a = (double)objA;
              double b = (double)(float)objB;
              // Treat NaN as greater than all other numbers
              if (Double.IsNaN(a)) {
                return Double.IsNaN(b) ? 0 : 1;
              }
              if (Double.IsNaN(b)) {
                return -1;
              }
              if (a == b) {
                return 0;
              }
              return (a < b) ? -1 : 1;
            }
          case (CBORObjectTypeDouble << 4) | CBORObjectTypeExtendedDecimal: {
              double sf = (double)objA;
              if (Double.IsInfinity(sf)) {
                return sf < 0 ? -1 : 1;
              }
              if (Double.IsNaN(sf)) {
                return 1;
              }
              decfracXa = ExtendedDecimal.FromDouble(sf);
              decfracXb = (ExtendedDecimal)objB;
              return decfracXa.CompareTo(decfracXb);
            }
          case (CBORObjectTypeDouble << 4) | CBORObjectTypeExtendedFloat: {
              double sf = (double)objA;
              if (Double.IsInfinity(sf)) {
                return sf < 0 ? -1 : 1;
              }
              if (Double.IsNaN(sf)) {
                return 1;
              }
              bigfloatXa = ExtendedFloat.FromDouble(sf);
              bigfloatXb = (ExtendedFloat)objB;
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeExtendedDecimal << 4) | CBORObjectTypeInteger: {
              decfracXa = (ExtendedDecimal)objA;
              decfracXb = ExtendedDecimal.FromInt64((long)objB);
              return decfracXa.CompareTo(decfracXb);
            }
          case (CBORObjectTypeExtendedDecimal << 4) | CBORObjectTypeBigInteger: {
              decfracXa = (ExtendedDecimal)objA;
              decfracXb = ExtendedDecimal.FromBigInteger((BigInteger)objB);
              return decfracXa.CompareTo(decfracXb);
            }
          case (CBORObjectTypeExtendedDecimal << 4) | CBORObjectTypeSingle: {
              float sf = (float)objB;
              if (Single.IsInfinity(sf)) {
                return sf < 0 ? 1 : -1;
              }
              if (Single.IsNaN(sf)) {
                return -1;
              }
              decfracXa = (ExtendedDecimal)objA;
              decfracXb = ExtendedDecimal.FromSingle(sf);
              return decfracXa.CompareTo(decfracXb);
            }
          case (CBORObjectTypeExtendedDecimal << 4) | CBORObjectTypeDouble: {
              double sf = (double)objB;
              if (Double.IsInfinity(sf)) {
                return sf < 0 ? 1 : -1;
              }
              if (Double.IsNaN(sf)) {
                return -1;
              }
              decfracXa = (ExtendedDecimal)objA;
              decfracXb = ExtendedDecimal.FromDouble(sf);
              return decfracXa.CompareTo(decfracXb);
            }
          case (CBORObjectTypeExtendedDecimal << 4) | CBORObjectTypeExtendedFloat: {
              decfracXa = (ExtendedDecimal)objA;
              decfracXb = ExtendedDecimal.FromExtendedFloat((ExtendedFloat)objB);
              return decfracXa.CompareTo(decfracXb);
            }
          case (CBORObjectTypeExtendedFloat << 4) | CBORObjectTypeInteger: {
              bigfloatXa = (ExtendedFloat)objA;
              bigfloatXb = ExtendedFloat.FromInt64((long)objB);
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeExtendedFloat << 4) | CBORObjectTypeBigInteger: {
              bigfloatXa = (ExtendedFloat)objA;
              bigfloatXb = ExtendedFloat.FromBigInteger((BigInteger)objB);
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeExtendedFloat << 4) | CBORObjectTypeSingle: {
              float sf = (float)objB;
              if (Single.IsInfinity(sf)) {
                return sf < 0 ? 1 : -1;
              }
              if (Single.IsNaN(sf)) {
                return -1;
              }
              bigfloatXa = (ExtendedFloat)objA;
              bigfloatXb = ExtendedFloat.FromSingle(sf);
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeExtendedFloat << 4) | CBORObjectTypeDouble: {
              double sf = (double)objB;
              if (Double.IsInfinity(sf)) {
                return sf < 0 ? 1 : -1;
              }
              if (Double.IsNaN(sf)) {
                return -1;
              }
              bigfloatXa = (ExtendedFloat)objA;
              bigfloatXb = ExtendedFloat.FromDouble(sf);
              return bigfloatXa.CompareTo(bigfloatXb);
            }
          case (CBORObjectTypeExtendedFloat << 4) | CBORObjectTypeExtendedDecimal: {
              decfracXa = ExtendedDecimal.FromExtendedFloat((ExtendedFloat)objA);
              decfracXb = (ExtendedDecimal)objB;
              return decfracXa.CompareTo(decfracXb);
            }
          case (CBORObjectTypeExtendedFloat << 4) | CBORObjectTypeSimpleValue:
          case (CBORObjectTypeExtendedFloat << 4) | CBORObjectTypeByteString:
          case (CBORObjectTypeExtendedFloat << 4) | CBORObjectTypeTextString:
          case (CBORObjectTypeExtendedFloat << 4) | CBORObjectTypeArray:
          case (CBORObjectTypeExtendedFloat << 4) | CBORObjectTypeMap:
            return -1;
          case (CBORObjectTypeSimpleValue << 4) | CBORObjectTypeExtendedFloat:
          case (CBORObjectTypeByteString << 4) | CBORObjectTypeExtendedFloat:
          case (CBORObjectTypeTextString << 4) | CBORObjectTypeExtendedFloat:
          case (CBORObjectTypeArray << 4) | CBORObjectTypeExtendedFloat:
          case (CBORObjectTypeMap << 4) | CBORObjectTypeExtendedFloat:
            return 1;
          case (CBORObjectTypeInteger << 4) | CBORObjectTypeSimpleValue:
          case (CBORObjectTypeInteger << 4) | CBORObjectTypeByteString:
          case (CBORObjectTypeInteger << 4) | CBORObjectTypeTextString:
          case (CBORObjectTypeInteger << 4) | CBORObjectTypeArray:
          case (CBORObjectTypeInteger << 4) | CBORObjectTypeMap:
          case (CBORObjectTypeBigInteger << 4) | CBORObjectTypeSimpleValue:
          case (CBORObjectTypeBigInteger << 4) | CBORObjectTypeByteString:
          case (CBORObjectTypeBigInteger << 4) | CBORObjectTypeTextString:
          case (CBORObjectTypeBigInteger << 4) | CBORObjectTypeArray:
          case (CBORObjectTypeBigInteger << 4) | CBORObjectTypeMap:
          case (CBORObjectTypeSingle << 4) | CBORObjectTypeSimpleValue:
          case (CBORObjectTypeSingle << 4) | CBORObjectTypeByteString:
          case (CBORObjectTypeSingle << 4) | CBORObjectTypeTextString:
          case (CBORObjectTypeSingle << 4) | CBORObjectTypeArray:
          case (CBORObjectTypeSingle << 4) | CBORObjectTypeMap:
          case (CBORObjectTypeDouble << 4) | CBORObjectTypeSimpleValue:
          case (CBORObjectTypeDouble << 4) | CBORObjectTypeByteString:
          case (CBORObjectTypeDouble << 4) | CBORObjectTypeTextString:
          case (CBORObjectTypeDouble << 4) | CBORObjectTypeArray:
          case (CBORObjectTypeDouble << 4) | CBORObjectTypeMap:
          case (CBORObjectTypeExtendedDecimal << 4) | CBORObjectTypeSimpleValue:
          case (CBORObjectTypeExtendedDecimal << 4) | CBORObjectTypeByteString:
          case (CBORObjectTypeExtendedDecimal << 4) | CBORObjectTypeTextString:
          case (CBORObjectTypeExtendedDecimal << 4) | CBORObjectTypeArray:
          case (CBORObjectTypeExtendedDecimal << 4) | CBORObjectTypeMap:
          case (CBORObjectTypeSimpleValue << 4) | CBORObjectTypeByteString:
          case (CBORObjectTypeSimpleValue << 4) | CBORObjectTypeTextString:
          case (CBORObjectTypeSimpleValue << 4) | CBORObjectTypeArray:
          case (CBORObjectTypeSimpleValue << 4) | CBORObjectTypeMap:
          case (CBORObjectTypeByteString << 4) | CBORObjectTypeTextString:
          case (CBORObjectTypeByteString << 4) | CBORObjectTypeArray:
          case (CBORObjectTypeByteString << 4) | CBORObjectTypeMap:
          case (CBORObjectTypeTextString << 4) | CBORObjectTypeArray:
          case (CBORObjectTypeTextString << 4) | CBORObjectTypeMap:
          case (CBORObjectTypeArray << 4) | CBORObjectTypeMap:
            return -1;
          case (CBORObjectTypeSimpleValue << 4) | CBORObjectTypeInteger:
          case (CBORObjectTypeSimpleValue << 4) | CBORObjectTypeBigInteger:
          case (CBORObjectTypeSimpleValue << 4) | CBORObjectTypeSingle:
          case (CBORObjectTypeSimpleValue << 4) | CBORObjectTypeDouble:
          case (CBORObjectTypeSimpleValue << 4) | CBORObjectTypeExtendedDecimal:
          case (CBORObjectTypeByteString << 4) | CBORObjectTypeInteger:
          case (CBORObjectTypeByteString << 4) | CBORObjectTypeBigInteger:
          case (CBORObjectTypeByteString << 4) | CBORObjectTypeSingle:
          case (CBORObjectTypeByteString << 4) | CBORObjectTypeDouble:
          case (CBORObjectTypeByteString << 4) | CBORObjectTypeExtendedDecimal:
          case (CBORObjectTypeByteString << 4) | CBORObjectTypeSimpleValue:
          case (CBORObjectTypeTextString << 4) | CBORObjectTypeInteger:
          case (CBORObjectTypeTextString << 4) | CBORObjectTypeBigInteger:
          case (CBORObjectTypeTextString << 4) | CBORObjectTypeSingle:
          case (CBORObjectTypeTextString << 4) | CBORObjectTypeDouble:
          case (CBORObjectTypeTextString << 4) | CBORObjectTypeExtendedDecimal:
          case (CBORObjectTypeTextString << 4) | CBORObjectTypeSimpleValue:
          case (CBORObjectTypeTextString << 4) | CBORObjectTypeByteString:
          case (CBORObjectTypeArray << 4) | CBORObjectTypeInteger:
          case (CBORObjectTypeArray << 4) | CBORObjectTypeBigInteger:
          case (CBORObjectTypeArray << 4) | CBORObjectTypeSingle:
          case (CBORObjectTypeArray << 4) | CBORObjectTypeDouble:
          case (CBORObjectTypeArray << 4) | CBORObjectTypeExtendedDecimal:
          case (CBORObjectTypeArray << 4) | CBORObjectTypeSimpleValue:
          case (CBORObjectTypeArray << 4) | CBORObjectTypeByteString:
          case (CBORObjectTypeArray << 4) | CBORObjectTypeTextString:
          case (CBORObjectTypeMap << 4) | CBORObjectTypeInteger:
          case (CBORObjectTypeMap << 4) | CBORObjectTypeBigInteger:
          case (CBORObjectTypeMap << 4) | CBORObjectTypeSingle:
          case (CBORObjectTypeMap << 4) | CBORObjectTypeDouble:
          case (CBORObjectTypeMap << 4) | CBORObjectTypeExtendedDecimal:
          case (CBORObjectTypeMap << 4) | CBORObjectTypeSimpleValue:
          case (CBORObjectTypeMap << 4) | CBORObjectTypeByteString:
          case (CBORObjectTypeMap << 4) | CBORObjectTypeTextString:
          case (CBORObjectTypeMap << 4) | CBORObjectTypeArray:
            return 1;
          default:
            throw new ArgumentException("Unexpected data type");
        }
      }
    }
    #region Equals and GetHashCode implementation
    private static int ListCompare(List<CBORObject> listA, List<CBORObject> listB) {
      if (listA == null) {
        return (listB == null) ? 0 : -1;
      }
      if (listB == null) {
        return 1;
      }
      int listACount = listA.Count;
      int listBCount = listB.Count;
      int c = Math.Min(listACount, listBCount);
      for (int i = 0; i < c; ++i) {
        int cmp = listA[i].CompareTo(listB[i]);
        if (cmp != 0) {
          return cmp;
        }
      }
      if (listACount != listBCount) {
        return (listACount < listBCount) ? -1 : 1;
      }
      return 0;
    }

    private static bool CBORArrayEquals(
      IList<CBORObject> listA,
      IList<CBORObject> listB) {
      if (listA == null) {
        return listB == null;
      }
      if (listB == null) {
        return false;
      }
      int listACount = listA.Count;
      int listBCount = listB.Count;
      if (listACount != listBCount) {
        return false;
      }
      for (int i = 0; i < listACount; ++i) {
        CBORObject itemA = listA[i];
        CBORObject itemB = listB[i];
        if (!(itemA == null ? itemB == null : itemA.Equals(itemB))) {
          return false;
        }
      }
      return true;
    }

    private static int CBORArrayHashCode(IList<CBORObject> list) {
      if (list == null) {
        return 0;
      }
      int ret = 19;
      int count = list.Count;
      unchecked {
        ret = (ret * 31) + count;
        for (int i = 0; i < count; ++i) {
          ret = (ret * 31) + list[i].GetHashCode();
        }
      }
      return ret;
    }

    private static bool CBORMapEquals(IDictionary<CBORObject, CBORObject> mapA, IDictionary<CBORObject, CBORObject> mapB) {
      if (mapA == null) {
        return mapB == null;
      }
      if (mapB == null) {
        return false;
      }
      if (mapA.Count != mapB.Count) {
        return false;
      }
      foreach (KeyValuePair<CBORObject, CBORObject> kvp in mapA) {
        CBORObject valueB = null;
        bool hasKey = mapB.TryGetValue(kvp.Key, out valueB);
        if (hasKey) {
          CBORObject valueA = kvp.Value;
          if (!(valueA == null ? valueB == null : valueA.Equals(valueB))) {
            return false;
          }
        } else {
          return false;
        }
      }
      return true;
    }

    private static int CBORMapHashCode(IDictionary<CBORObject, CBORObject> a) {
      // To simplify matters, we use just the count of
      // the map as the basis for the hash code. More complicated
      // hash code calculation would generally involve defining
      // how CBORObjects ought to be compared (since a stable
      // sort order is necessary for two equal maps to have the
      // same hash code), which is much too difficult to do.
      return unchecked(a.Count.GetHashCode() * 19);
    }

    /// <summary>Determines whether this object and another object are
    /// equal.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>True if the objects are equal; false otherwise.</returns>
    public override bool Equals(object obj) {
      return this.Equals(obj as CBORObject);
    }

    /// <summary>Compares the equality of two CBOR objects.</summary>
    /// <param name='other'>The object to compare.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public bool Equals(CBORObject other) {
      CBORObject otherValue = other as CBORObject;
      if (otherValue == null) {
        return false;
      }
      switch (this.itemtypeValue) {
        case CBORObjectTypeByteString:
          if (!CBORUtilities.ByteArrayEquals((byte[])this.ThisItem, otherValue.itemValue as byte[])) {
            return false;
          }
          break;
        case CBORObjectTypeMap: {
            IDictionary<CBORObject, CBORObject> cbordict = otherValue.itemValue as IDictionary<CBORObject,
            CBORObject>;
            if (!CBORMapEquals(this.AsMap(), cbordict)) {
              return false;
            }
            break;
          }
        case CBORObjectTypeArray:
          if (!CBORArrayEquals(this.AsList(), otherValue.itemValue as IList<CBORObject>)) {
            return false;
          }
          break;
        default:
          if (!object.Equals(this.itemValue, otherValue.itemValue)) {
            return false;
          }
          break;
      }
      return this.itemtypeValue == otherValue.itemtypeValue &&
        this.tagLow == otherValue.tagLow &&
        this.tagHigh == otherValue.tagHigh;
    }

    /// <summary>Calculates the hash code of this object.</summary>
    /// <returns>A 32-bit hash code.</returns>
    public override int GetHashCode() {
      int hashCode = 13;
      unchecked {
        if (this.itemValue != null) {
          int itemHashCode = 0;
          switch (this.itemtypeValue) {
            case CBORObjectTypeByteString:
              itemHashCode = CBORUtilities.ByteArrayHashCode((byte[])this.ThisItem);
              break;
            case CBORObjectTypeMap:
              itemHashCode = CBORMapHashCode(this.AsMap());
              break;
            case CBORObjectTypeArray:
              itemHashCode = CBORArrayHashCode(this.AsList());
              break;
            default:
              itemHashCode = this.itemValue.GetHashCode();
              break;
          }
          hashCode += 17 * itemHashCode;
        }
        hashCode += 19 * (this.itemtypeValue.GetHashCode() + this.tagLow + this.tagHigh);
      }
      return hashCode;
    }
    #endregion
    private static void CheckCBORLength(long expectedLength, long actualLength) {
      if (actualLength < expectedLength) {
        throw new CBORException("Premature end of data");
      } else if (actualLength > expectedLength) {
        throw new CBORException("Too many bytes");
      }
    }

    private static void CheckCBORLength(int expectedLength, int actualLength) {
      if (actualLength < expectedLength) {
        throw new CBORException("Premature end of data");
      } else if (actualLength > expectedLength) {
        throw new CBORException("Too many bytes");
      }
    }

    private static string GetOptimizedStringIfShortAscii(
      byte[] data,
      int offset) {
      int length = data.Length;
      if (length > offset) {
        int nextbyte = (int)(data[offset] & (int)0xFF);
        if (nextbyte >= 0x60 && nextbyte < 0x78) {
          int offsetp1 = 1 + offset;
          // Check for type 3 string of short length
          int rightLength = offsetp1 + (nextbyte - 0x60);
          CheckCBORLength(rightLength, length);
          // Check for all ASCII text
          for (int i = offsetp1; i < length; ++i) {
            if ((data[i] & ((byte)0x80)) != 0) {
              return null;
            }
          }
          // All ASCII text, so convert to a string
          // from a char array without having to
          // convert from UTF-8 first
          char[] c = new char[length - offsetp1];
          for (int i = offsetp1; i < length; ++i) {
            c[i - offsetp1] = (char)(data[i] & (int)0xFF);
          }
          return new String(c);
        }
      }
      return null;
    }

    private static CBORObject[] valueFixedObjects = InitializeFixedObjects();
    // Initialize fixed values for certain
    // head bytes
    private static CBORObject[] InitializeFixedObjects() {
      valueFixedObjects = new CBORObject[256];
      for (int i = 0; i < 0x18; ++i) {
        valueFixedObjects[i] = new CBORObject(CBORObjectTypeInteger, (long)i);
      }
      for (int i = 0x20; i < 0x38; ++i) {
        valueFixedObjects[i] = new CBORObject(
          CBORObjectTypeInteger,
          (long)(-1 - (i - 0x20)));
      }
      valueFixedObjects[0x60] = new CBORObject(CBORObjectTypeTextString, String.Empty);
      for (int i = 0xE0; i < 0xf8; ++i) {
        valueFixedObjects[i] = new CBORObject(CBORObjectTypeSimpleValue, (int)(i - 0xe0));
      }
      return valueFixedObjects;
    }
    // Expected lengths for each head byte.
    // 0 means length varies. -1 means invalid.
    private static int[] valueExpectedLengths = new int[] {
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  // major type 0
      1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 5, 9, -1, -1, -1, -1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  // major type 1
      1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 5, 9, -1, -1, -1, -1,
      1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,  // major type 2
      17, 18, 19, 20, 21, 22, 23, 24, 0, 0, 0, 0, -1, -1, -1, 0,
      1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,  // major type 3
      17, 18, 19, 20, 21, 22, 23, 24, 0, 0, 0, 0, -1, -1, -1, 0,
      1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  // major type 4
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, -1, -1, 0,
      1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  // major type 5
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, -1, -1, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  // major type 6
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, -1, -1, -1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  // major type 7
      1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 5, 9, -1, -1, -1, -1
    };
    // Generate a CBOR object for head bytes with fixed length.
    // Note that this function assumes that the length of the data
    // was already checked.
    private static CBORObject GetFixedLengthObject(int firstbyte, byte[] data) {
      CBORObject fixedObj = valueFixedObjects[firstbyte];
      if (fixedObj != null) {
        return fixedObj;
      }
      int majortype = firstbyte >> 5;
      if (firstbyte >= 0x61 && firstbyte < 0x78) {
        // text string length 1 to 23
        String s = GetOptimizedStringIfShortAscii(data, 0);
        if (s != null) {
          return new CBORObject(CBORObjectTypeTextString, s);
        }
      }
      if ((firstbyte & 0x1C) == 0x18) {
        // contains 1 to 8 extra bytes of additional information
        long uadditional = 0;
        switch (firstbyte & 0x1F) {
          case 24:
            uadditional = (int)(data[1] & (int)0xFF);
            break;
          case 25:
            uadditional = ((long)(data[1] & (long)0xFF)) << 8;
            uadditional |= (long)(data[2] & (long)0xFF);
            break;
          case 26:
            uadditional = ((long)(data[1] & (long)0xFF)) << 24;
            uadditional |= ((long)(data[2] & (long)0xFF)) << 16;
            uadditional |= ((long)(data[3] & (long)0xFF)) << 8;
            uadditional |= (long)(data[4] & (long)0xFF);
            break;
          case 27:
            uadditional = ((long)(data[1] & (long)0xFF)) << 56;
            uadditional |= ((long)(data[2] & (long)0xFF)) << 48;
            uadditional |= ((long)(data[3] & (long)0xFF)) << 40;
            uadditional |= ((long)(data[4] & (long)0xFF)) << 32;
            uadditional |= ((long)(data[5] & (long)0xFF)) << 24;
            uadditional |= ((long)(data[6] & (long)0xFF)) << 16;
            uadditional |= ((long)(data[7] & (long)0xFF)) << 8;
            uadditional |= (long)(data[8] & (long)0xFF);
            break;
          default:
            throw new CBORException("Unexpected data encountered");
        }
        switch (majortype) {
          case 0:
            if ((uadditional >> 63) == 0) {
              // use only if additional's top bit isn't set
              // (additional is a signed long)
              return new CBORObject(CBORObjectTypeInteger, uadditional);
            } else {
              int low = unchecked((int)((uadditional) & 0xFFFFFFFFL));
              int high = unchecked((int)((uadditional >> 32) & 0xFFFFFFFFL));
              return FromObject(LowHighToBigInteger(low, high));
            }
          case 1:
            if ((uadditional >> 63) == 0) {
              // use only if additional's top bit isn't set
              // (additional is a signed long)
              return new CBORObject(CBORObjectTypeInteger, -1 - uadditional);
            } else {
              int low = unchecked((int)((uadditional) & 0xFFFFFFFFL));
              int high = unchecked((int)((uadditional >> 32) & 0xFFFFFFFFL));
              BigInteger bigintAdditional = LowHighToBigInteger(low, high);
              bigintAdditional = -BigInteger.One;
              bigintAdditional -= (BigInteger)bigintAdditional;
              return FromObject(bigintAdditional);
            }
          case 7:
            if (firstbyte == 0xf9) {
              return new CBORObject(
                CBORObjectTypeSingle,
                CBORUtilities.HalfPrecisionToSingle(unchecked((int)uadditional)));
            } else if (firstbyte == 0xfa) {
              return new CBORObject(
                CBORObjectTypeSingle,
                BitConverter.ToSingle(BitConverter.GetBytes((int)unchecked((int)uadditional)), 0));
            } else if (firstbyte == 0xfb) {
              return new CBORObject(
                CBORObjectTypeDouble,
                BitConverter.ToDouble(BitConverter.GetBytes((long)uadditional), 0));
            } else if (firstbyte == 0xf8) {
              return new CBORObject(
                CBORObjectTypeSimpleValue,
                (int)uadditional);
            } else {
              throw new CBORException("Unexpected data encountered");
            }
          default:
            throw new CBORException("Unexpected data encountered");
        }
      } else if (majortype == 2) {  // short byte string
        byte[] ret = new byte[firstbyte - 0x40];
        Array.Copy(data, 1, ret, 0, firstbyte - 0x40);
        return new CBORObject(CBORObjectTypeByteString, ret);
      } else if (majortype == 3) {  // short text string
        StringBuilder ret = new StringBuilder(firstbyte - 0x60);
        DataUtilities.ReadUtf8FromBytes(data, 1, firstbyte - 0x60, ret, false);
        return new CBORObject(CBORObjectTypeTextString, ret.ToString());
      } else if (firstbyte == 0x80) {
        // empty array
        return FromObject(new List<CBORObject>());
      } else if (firstbyte == 0xA0) {
        // empty map
        return FromObject(new Dictionary<CBORObject, CBORObject>());
      } else {
        throw new CBORException("Unexpected data encountered");
      }
    }

    /// <summary>Generates a CBOR object from an array of CBOR-encoded bytes.</summary>
    /// <param name='data'>A byte array.</param>
    /// <returns>A CBOR object corresponding to the data.</returns>
    /// <exception cref='System.ArgumentException'>Data is null or empty.</exception>
    /// <exception cref='CBORException'>There was an error in reading
    /// or parsing the data. This includes cases where not all of the byte array
    /// represents a CBOR object.</exception>
    public static CBORObject DecodeFromBytes(byte[] data) {
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      if (data.Length == 0) {
        throw new ArgumentException("data is empty.");
      }
      int firstbyte = (int)(data[0] & (int)0xFF);
      int expectedLength = valueExpectedLengths[firstbyte];
      // if invalid
      if (expectedLength == -1) {
        throw new CBORException("Unexpected data encountered");
      } else if (expectedLength != 0) {
        // if fixed length
        CheckCBORLength(expectedLength, data.Length);
      }
      if (firstbyte == 0xc0) {
        // value with tag 0
        String s = GetOptimizedStringIfShortAscii(data, 1);
        if (s != null) {
          return new CBORObject(FromObject(s), 0, 0);
        }
      }
      if (expectedLength != 0) {
        return GetFixedLengthObject(firstbyte, data);
      }
      // For objects with variable length,
      // read the object as though
      // the byte array were a stream
      using (MemoryStream ms = new MemoryStream(data)) {
        CBORObject o = Read(ms);
        CheckCBORLength((long)data.Length, (long)ms.Position);
        return o;
      }
    }

    /// <summary>Gets the number of keys in this map, or the number of items
    /// in this array, or 0 if this item is neither an array nor a map.</summary>
    /// <value>The number of keys in this map, or the number of items in this
    /// array, or 0 if this item is neither an array nor a map.</value>
    public int Count {
      get {
        if (this.ItemType == CBORObjectTypeArray) {
          return this.AsList().Count;
        } else if (this.ItemType == CBORObjectTypeMap) {
          return this.AsMap().Count;
        } else {
          return 0;
        }
      }
    }

    /// <summary>Gets a value indicating whether this data item has at least
    /// one tag.</summary>
    /// <value>Whether this data item has at least one tag.</value>
    public bool IsTagged {
      get {
        return this.itemtypeValue == CBORObjectTypeTagged;
      }
    }

    /// <summary>Gets the byte array used in this object, if this object is
    /// a byte string, without copying the data to a new one.</summary>
    /// <exception cref='InvalidOperationException'>This object is
    /// not a byte string.</exception>
    /// <returns>A byte array.</returns>
    public byte[] GetByteString() {
      if (this.itemtypeValue == CBORObjectTypeByteString) {
        return (byte[])this.ThisItem;
      } else {
        throw new InvalidOperationException("Not a byte string");
      }
    }

    private bool HasTag(int tagValue) {
      if (!this.IsTagged) {
        return false;
      }
      if (this.tagHigh == 0 && tagValue == this.tagLow) {
        return true;
      }
      return ((CBORObject)this.itemValue).HasTag(tagValue);
    }

    private static BigInteger LowHighToBigInteger(int tagLow, int tagHigh) {
      byte[] uabytes = null;
      if (tagHigh != 0) {
        uabytes = new byte[9];
        uabytes[7] = (byte)((tagHigh >> 24) & 0xFF);
        uabytes[6] = (byte)((tagHigh >> 16) & 0xFF);
        uabytes[5] = (byte)((tagHigh >> 8) & 0xFF);
        uabytes[4] = (byte)(tagHigh & 0xFF);
        uabytes[3] = (byte)((tagLow >> 24) & 0xFF);
        uabytes[2] = (byte)((tagLow >> 16) & 0xFF);
        uabytes[1] = (byte)((tagLow >> 8) & 0xFF);
        uabytes[0] = (byte)(tagLow & 0xFF);
        uabytes[8] = 0;
        return new BigInteger((byte[])uabytes);
      } else if (tagLow != 0) {
        uabytes = new byte[5];
        uabytes[3] = (byte)((tagLow >> 24) & 0xFF);
        uabytes[2] = (byte)((tagLow >> 16) & 0xFF);
        uabytes[1] = (byte)((tagLow >> 8) & 0xFF);
        uabytes[0] = (byte)(tagLow & 0xFF);
        uabytes[4] = 0;
        return new BigInteger((byte[])uabytes);
      } else {
        return BigInteger.Zero;
      }
    }

    private static BigInteger[] valueEmptyTags = new BigInteger[0];

    /// <summary>Gets a list of all tags, from outermost to innermost.</summary>
    /// <returns>A BigInteger[] object.</returns>
    public BigInteger[] GetTags() {
      if (!this.IsTagged) {
        return valueEmptyTags;
      }
      CBORObject curitem = this;
      if (curitem.IsTagged) {
        var list = new List<BigInteger>();
        while (curitem.IsTagged) {
          list.Add(LowHighToBigInteger(
            curitem.tagLow,
            curitem.tagHigh));
          curitem = (CBORObject)curitem.itemValue;
        }
        return (BigInteger[])list.ToArray();
      } else {
        return new BigInteger[] { LowHighToBigInteger(this.tagLow, this.tagHigh) };
      }
    }

    /// <summary>Gets the last defined tag for this CBOR data item, or 0 if
    /// the item is untagged.</summary>
    /// <value>The last defined tag for this CBOR data item, or 0 if the item
    /// is untagged.</value>
    public BigInteger InnermostTag {
      get {
        if (!this.IsTagged) {
          return BigInteger.Zero;
        }
        CBORObject previtem = this;
        CBORObject curitem = (CBORObject)this.itemValue;
        while (curitem.IsTagged) {
          previtem = curitem;
          curitem = (CBORObject)curitem.itemValue;
        }
        if (previtem.tagHigh == 0 &&
            previtem.tagLow >= 0 &&
            previtem.tagLow < 0x10000) {
          return (BigInteger)previtem.tagLow;
        }
        return LowHighToBigInteger(
          previtem.tagLow,
          previtem.tagHigh);
      }
    }

    private IDictionary<CBORObject, CBORObject> AsMap() {
      return (IDictionary<CBORObject, CBORObject>)this.ThisItem;
    }

    private IList<CBORObject> AsList() {
      return (IList<CBORObject>)this.ThisItem;
    }

    /// <summary>Gets the value of a CBOR object by integer index in this array.</summary>
    /// <exception cref='System.InvalidOperationException'>This object
    /// is not an array.</exception>
    /// <returns>A CBORObject object.</returns>
    /// <param name='index'>A 32-bit signed integer.</param>
    public CBORObject this[int index] {
      get {
        if (this.ItemType == CBORObjectTypeArray) {
          IList<CBORObject> list = this.AsList();
          return list[index];
        } else {
          throw new InvalidOperationException("Not an array");
        }
      }

    /// <summary>Sets the value of a CBOR object by integer index in this array.</summary>
    /// <exception cref='System.InvalidOperationException'>This object
    /// is not an array.</exception>
    /// <exception cref='System.ArgumentNullException'>Value is null
    /// (as opposed to CBORObject.Null).</exception>
      set {
        if (this.ItemType == CBORObjectTypeArray) {
          if (value == null) {
            throw new ArgumentNullException("value");
          }
          IList<CBORObject> list = this.AsList();
          list[index] = value;
        } else {
          throw new InvalidOperationException("Not an array");
        }
      }
    }

    /// <summary>Gets a collection of the keys of this CBOR object.</summary>
    /// <exception cref='System.InvalidOperationException'>This object
    /// is not a map.</exception>
    /// <value>A collection of the keys of this CBOR object.</value>
    public ICollection<CBORObject> Keys {
      get {
        if (this.ItemType == CBORObjectTypeMap) {
          IDictionary<CBORObject, CBORObject> dict = this.AsMap();
          return dict.Keys;
        } else {
          throw new InvalidOperationException("Not a map");
        }
      }
    }

    /// <summary>Gets the value of a CBOR object in this map, using a CBOR object
    /// as the key.</summary>
    /// <exception cref='System.ArgumentNullException'>The key is null
    /// (as opposed to CBORObject.Null).</exception>
    /// <exception cref='System.InvalidOperationException'>This object
    /// is not a map.</exception>
    /// <returns>A CBORObject object.</returns>
    /// <param name='key'>A CBORObject object. (2).</param>
    public CBORObject this[CBORObject key] {
      get {
        if (key == null) {
          throw new ArgumentNullException("key");
        }
        if (this.ItemType == CBORObjectTypeMap) {
          IDictionary<CBORObject, CBORObject> map = this.AsMap();
          if (!map.ContainsKey(key)) {
            return null;
          }
          return map[key];
        } else {
          throw new InvalidOperationException("Not a map");
        }
      }

    /// <summary>Sets the value of a CBOR object in this map, using a CBOR object
    /// as the key.</summary>
    /// <exception cref='System.ArgumentNullException'>The key or value
    /// is null (as opposed to CBORObject.Null).</exception>
    /// <exception cref='System.InvalidOperationException'>This object
    /// is not a map.</exception>
      set {
        if (key == null) {
          throw new ArgumentNullException("key");
        }
        if (value == null) {
          throw new ArgumentNullException("value");
        }
        if (this.ItemType == CBORObjectTypeMap) {
          IDictionary<CBORObject, CBORObject> map = this.AsMap();
          map[key] = value;
        } else {
          throw new InvalidOperationException("Not a map");
        }
      }
    }

    /// <summary>Gets the value of a CBOR object in this map, using a string
    /// as the key.</summary>
    /// <exception cref='System.ArgumentNullException'>The key is null.</exception>
    /// <exception cref='System.InvalidOperationException'>This object
    /// is not a map.</exception>
    /// <returns>A CBORObject object.</returns>
    /// <param name='key'>A string object.</param>
    public CBORObject this[string key] {
      get {
        if (key == null) {
          throw new ArgumentNullException("key");
        }
        CBORObject objkey = CBORObject.FromObject(key);
        return this[objkey];
      }

    /// <summary>Sets the value of a CBOR object in this map, using a string
    /// as the key.</summary>
    /// <exception cref='System.ArgumentNullException'>The key or value
    /// is null (as opposed to CBORObject.Null).</exception>
    /// <exception cref='System.InvalidOperationException'>This object
    /// is not a map.</exception>
      set {
        if (key == null) {
          throw new ArgumentNullException("key");
        }
        if (value == null) {
          throw new ArgumentNullException("value");
        }
        CBORObject objkey = CBORObject.FromObject(key);
        if (this.ItemType == CBORObjectTypeMap) {
          IDictionary<CBORObject, CBORObject> map = this.AsMap();
          map[objkey] = value;
        } else {
          throw new InvalidOperationException("Not a map");
        }
      }
    }

    /// <summary>Gets the simple value ID of this object, or -1 if this object
    /// is not a simple value (including if the value is a floating-point number).</summary>
    /// <value>The simple value ID of this object, or -1 if this object is not
    /// a simple value (including if the value is a floating-point number).</value>
    public int SimpleValue {
      get {
        if (this.ItemType == CBORObjectTypeSimpleValue) {
          return (int)this.ThisItem;
        } else {
          return -1;
        }
      }
    }

    /// <summary>Adds a new object to this map.</summary>
    /// <param name='key'>A CBOR object representing the key.</param>
    /// <param name='value'>A CBOR object representing the value.</param>
    /// <exception cref='System.ArgumentNullException'>Key or value
    /// is null (as opposed to CBORObject.Null).</exception>
    /// <exception cref='System.ArgumentException'>Key already exists
    /// in this map.</exception>
    /// <exception cref='InvalidOperationException'>This object is
    /// not a map.</exception>
    public void Add(CBORObject key, CBORObject value) {
      if (key == null) {
        throw new ArgumentNullException("key");
      }
      if (value == null) {
        throw new ArgumentNullException("value");
      }
      if (this.ItemType == CBORObjectTypeMap) {
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        if (map.ContainsKey(key)) {
          throw new ArgumentException("Key already exists.");
        }
        map.Add(key, value);
      } else {
        throw new InvalidOperationException("Not a map");
      }
    }

    /// <summary>Adds a new object to this map.</summary>
    /// <param name='key'>A string representing the key.</param>
    /// <param name='value'>A CBOR object representing the value.</param>
    /// <exception cref='System.ArgumentNullException'>Key or value
    /// is null (as opposed to CBORObject.Null).</exception>
    /// <exception cref='System.ArgumentException'>Key already exists
    /// in this map.</exception>
    /// <exception cref='InvalidOperationException'>This object is
    /// not a map.</exception>
    public void Add(string key, CBORObject value) {
      if (key == null) {
        throw new ArgumentNullException("key");
      }
      if (value == null) {
        throw new ArgumentNullException("value");
      }
      this.Add(CBORObject.FromObject(key), value);
    }

    /// <summary>Determines whether a value of the given key exists in this
    /// object.</summary>
    /// <param name='key'>An object that serves as the key.</param>
    /// <returns>True if the given key is found, or false if the given key is
    /// not found or this object is not a map.</returns>
    /// <exception cref='System.ArgumentNullException'>Key is null
    /// (as opposed to CBORObject.Null).</exception>
    public bool ContainsKey(CBORObject key) {
      if (key == null) {
        throw new ArgumentNullException("key");
      }
      if (this.ItemType == CBORObjectTypeMap) {
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        return map.ContainsKey(key);
      } else {
        return false;
      }
    }

    /// <summary>Adds a new object to the end of this array.</summary>
    /// <param name='obj'>A CBOR object.</param>
    /// <exception cref='System.InvalidOperationException'>This object
    /// is not an array.</exception>
    /// <exception cref='System.ArgumentNullException'>Obj is null
    /// (as opposed to CBORObject.Null).</exception>
    public void Add(CBORObject obj) {
      if (obj == null) {
        throw new ArgumentNullException("obj");
      }
      if (this.ItemType == CBORObjectTypeArray) {
        IList<CBORObject> list = this.AsList();
        list.Add(obj);
      } else {
        throw new InvalidOperationException("Not an array");
      }
    }

    /// <summary>If this object is an array, removes the first instance of
    /// the specified item from the array. If this object is a map, removes
    /// the item with the given key from the map.</summary>
    /// <param name='obj'>The item or key to remove.</param>
    /// <returns>True if the item was removed; otherwise, false.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='obj'/> is null (as opposed to CBORObject.Null).</exception>
    /// <exception cref='System.InvalidOperationException'>The object
    /// is not an array or map.</exception>
    public bool Remove(CBORObject obj) {
      if (obj == null) {
        throw new ArgumentNullException("obj");
      }
      if (this.ItemType == CBORObjectTypeMap) {
        IDictionary<CBORObject, CBORObject> dict = this.AsMap();
        bool hasKey = dict.ContainsKey(obj);
        if (hasKey) {
          dict.Remove(obj);
          return true;
        }
        return false;
      } else if (this.ItemType == CBORObjectTypeArray) {
        IList<CBORObject> list = this.AsList();
        return list.Remove(obj);
      } else {
        throw new InvalidOperationException("Not a map or array");
      }
    }

    /// <summary>Converts this object to a 64-bit floating point number.</summary>
    /// <returns>The closest 64-bit floating point number to this object.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 64-bit floating point number.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    public double AsDouble() {
      int type = this.ItemType;
      switch (type) {
        case CBORObjectTypeInteger:
          return (double)(long)this.ThisItem;
        case CBORObjectTypeBigInteger:
          return ExtendedFloat.FromBigInteger((BigInteger)this.ThisItem).ToDouble();
        case CBORObjectTypeSingle:
          return (double)(float)this.ThisItem;
        case CBORObjectTypeDouble:
          return (double)this.ThisItem;
        case CBORObjectTypeExtendedDecimal: {
            return ((ExtendedDecimal)this.ThisItem).ToDouble();
          }
        case CBORObjectTypeExtendedFloat: {
            return ((ExtendedFloat)this.ThisItem).ToDouble();
          }
        default:
          throw new InvalidOperationException("Not a number type");
      }
    }

    /// <summary>Converts this object to a decimal fraction.</summary>
    /// <returns>A decimal fraction for this object's value.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    public ExtendedDecimal AsExtendedDecimal() {
      int type = this.ItemType;
      switch (type) {
        case CBORObjectTypeInteger:
          return ExtendedDecimal.FromInt64((long)this.ThisItem);
        case CBORObjectTypeBigInteger:
          return ExtendedDecimal.FromBigInteger((BigInteger)this.ThisItem);
        case CBORObjectTypeSingle:
          return ExtendedDecimal.FromSingle((float)this.ThisItem);
        case CBORObjectTypeDouble:
          return ExtendedDecimal.FromDouble((double)this.ThisItem);
        case CBORObjectTypeExtendedDecimal:
          return (ExtendedDecimal)this.ThisItem;
        case CBORObjectTypeExtendedFloat: {
            return ExtendedDecimal.FromExtendedFloat((ExtendedFloat)this.ThisItem);
          }
        default:
          throw new InvalidOperationException("Not a number type");
      }
    }

    /// <summary>Converts this object to an arbitrary-precision binary
    /// floating point number.</summary>
    /// <returns>An arbitrary-precision binary floating point number
    /// for this object's value. Note that if this object is a decimal fraction
    /// with a fractional part, the conversion may lose information depending
    /// on the number.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    public ExtendedFloat AsExtendedFloat() {
      int type = this.ItemType;
      switch (type) {
        case CBORObjectTypeInteger:
          return ExtendedFloat.FromInt64((long)this.ThisItem);
        case CBORObjectTypeBigInteger:
          return ExtendedFloat.FromBigInteger((BigInteger)this.ThisItem);
        case CBORObjectTypeSingle:
          return ExtendedFloat.FromSingle((float)this.ThisItem);
        case CBORObjectTypeDouble:
          return ExtendedFloat.FromDouble((double)this.ThisItem);
        case CBORObjectTypeExtendedDecimal:
          return ((ExtendedDecimal)this.ThisItem).ToExtendedFloat();
        case CBORObjectTypeExtendedFloat: {
            return (ExtendedFloat)this.ThisItem;
          }
        default:
          throw new InvalidOperationException("Not a number type");
      }
    }

    /// <summary>Converts this object to a 32-bit floating point number.</summary>
    /// <returns>The closest 32-bit floating point number to this object.
    /// The return value can be positive infinity or negative infinity if
    /// this object's value exceeds the range of a 32-bit floating point number.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    public float AsSingle() {
      int type = this.ItemType;
      switch (type) {
        case CBORObjectTypeInteger:
          return (float)(long)this.ThisItem;
        case CBORObjectTypeBigInteger:
          return ExtendedFloat.FromBigInteger((BigInteger)this.ThisItem).ToSingle();
        case CBORObjectTypeSingle:
          return (float)this.ThisItem;
        case CBORObjectTypeDouble:
          return (float)(double)this.ThisItem;
        case CBORObjectTypeExtendedDecimal: {
            return ((ExtendedDecimal)this.ThisItem).ToSingle();
          }
        case CBORObjectTypeExtendedFloat: {
            return ((ExtendedFloat)this.ThisItem).ToSingle();
          }
        default:
          throw new InvalidOperationException("Not a number type");
      }
    }

    /// <summary>Converts this object to an arbitrary-precision integer.
    /// Fractional values are truncated to an integer.</summary>
    /// <returns>The closest big integer to this object.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    public BigInteger AsBigInteger() {
      int type = this.ItemType;
      switch (type) {
        case CBORObjectTypeInteger:
          return (BigInteger)(long)this.ThisItem;
        case CBORObjectTypeBigInteger:
          return (BigInteger)this.ThisItem;
        case CBORObjectTypeSingle:
          return CBORUtilities.BigIntegerFromSingle((float)this.ThisItem);
        case CBORObjectTypeDouble:
          return CBORUtilities.BigIntegerFromDouble((double)this.ThisItem);
        case CBORObjectTypeExtendedDecimal: {
            return ((ExtendedDecimal)this.ThisItem).ToBigInteger();
          }
        case CBORObjectTypeExtendedFloat: {
            return ((ExtendedFloat)this.ThisItem).ToBigInteger();
          }
        default:
          throw new InvalidOperationException("Not a number type");
      }
    }

    /// <summary>Returns false if this object is False, Null, or Undefined;
    /// otherwise, true.</summary>
    /// <returns>A Boolean object.</returns>
    public bool AsBoolean() {
      if (this.IsFalse || this.IsNull || this.IsUndefined) {
        return false;
      }
      return true;
    }

    /// <summary>Converts this object to a 16-bit signed integer. Floating
    /// point values are truncated to an integer.</summary>
    /// <returns>The closest 16-bit signed integer to this object.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    /// <exception cref='System.OverflowException'>This object's value
    /// exceeds the range of a 16-bit signed integer.</exception>
    public short AsInt16() {
      return (short)this.AsInt32(Int16.MinValue, Int16.MaxValue);
    }

    /// <summary>Converts this object to a byte (0 to 255). Floating point
    /// values are truncated to an integer.</summary>
    /// <returns>The closest byte-sized integer to this object.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    /// <exception cref='System.OverflowException'>This object's value
    /// exceeds the range of a byte (is less than 0 or would be greater than 255
    /// when truncated to an integer).</exception>
    public byte AsByte() {
      return (byte)this.AsInt32(0, 255);
    }

    /// <summary>Converts this object to a 64-bit signed integer. Floating
    /// point values are truncated to an integer.</summary>
    /// <returns>The closest 64-bit signed integer to this object.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    /// <exception cref='System.OverflowException'>This object's value
    /// exceeds the range of a 64-bit signed integer.</exception>
    public long AsInt64() {
      int type = this.ItemType;
      switch (type) {
        case CBORObjectTypeInteger: {
            return (long)this.ThisItem;
          }
        case CBORObjectTypeBigInteger: {
            if (((BigInteger)this.ThisItem).CompareTo(Int64MaxValue) > 0 ||
                ((BigInteger)this.ThisItem).CompareTo(Int64MinValue) < 0) {
              throw new OverflowException("This object's value is out of range");
            }
            return (long)(BigInteger)this.ThisItem;
          }
        case CBORObjectTypeSingle: {
            float fltItem = (float)this.ThisItem;
            if (Single.IsNaN(fltItem)) {
              throw new OverflowException("This object's value is out of range");
            }
            fltItem = (fltItem < 0) ? (float)Math.Ceiling(fltItem) : (float)Math.Floor(fltItem);
            if (fltItem >= Int64.MinValue && fltItem <= Int64.MaxValue) {
              return (long)fltItem;
            }
            throw new OverflowException("This object's value is out of range");
          }
        case CBORObjectTypeDouble: {
            double fltItem = (double)this.ThisItem;
            if (Double.IsNaN(fltItem)) {
              throw new OverflowException("This object's value is out of range");
            }
            fltItem = (fltItem < 0) ? Math.Ceiling(fltItem) : Math.Floor(fltItem);
            if (fltItem >= Int64.MinValue && fltItem <= Int64.MaxValue) {
              return (long)fltItem;
            }
            throw new OverflowException("This object's value is out of range");
          }
        case CBORObjectTypeExtendedDecimal: {
            BigInteger bi = ((ExtendedDecimal)this.ThisItem).ToBigInteger();
            if (bi.CompareTo(Int64MaxValue) > 0 ||
                bi.CompareTo(Int64MinValue) < 0) {
              throw new OverflowException("This object's value is out of range");
            }
            return (long)bi;
          }
        case CBORObjectTypeExtendedFloat: {
            BigInteger bi = ((ExtendedFloat)this.ThisItem).ToBigInteger();
            if (bi.CompareTo(Int64MaxValue) > 0 ||
                bi.CompareTo(Int64MinValue) < 0) {
              throw new OverflowException("This object's value is out of range");
            }
            return (long)bi;
          }
        default:
          throw new InvalidOperationException("Not a number type");
      }
    }

    private int AsInt32(int minValue, int maxValue) {
      object thisItem = this.ThisItem;
      int type = this.ItemType;
      switch (type) {
        case CBORObjectTypeInteger: {
            long longItem = (long)thisItem;
            if (longItem > maxValue || longItem < minValue) {
              throw new OverflowException("This object's value is out of range");
            }
            return (int)longItem;
          }
        case CBORObjectTypeBigInteger: {
            BigInteger bigintItem = (BigInteger)thisItem;
            if (bigintItem.CompareTo((BigInteger)maxValue) > 0 ||
                bigintItem.CompareTo((BigInteger)minValue) < 0) {
              throw new OverflowException("This object's value is out of range");
            }
            return (int)bigintItem;
          }
        case CBORObjectTypeSingle: {
            float fltItem = (float)thisItem;
            if (Single.IsNaN(fltItem)) {
              throw new OverflowException("This object's value is out of range");
            }
            fltItem = (fltItem < 0) ? (float)Math.Ceiling(fltItem) : (float)Math.Floor(fltItem);
            if (fltItem >= minValue && fltItem <= maxValue) {
              return (int)fltItem;
            }
            throw new OverflowException("This object's value is out of range");
          }
        case CBORObjectTypeDouble: {
            double fltItem = (double)thisItem;
            if (Double.IsNaN(fltItem)) {
              throw new OverflowException("This object's value is out of range");
            }
            fltItem = (fltItem < 0) ? Math.Ceiling(fltItem) : Math.Floor(fltItem);
            if (fltItem >= minValue && fltItem <= maxValue) {
              return (int)fltItem;
            }
            throw new OverflowException("This object's value is out of range");
          }
        case CBORObjectTypeExtendedDecimal: {
            BigInteger bi = ((ExtendedDecimal)this.ThisItem).ToBigInteger();
            if (bi.CompareTo((BigInteger)maxValue) > 0 ||
                bi.CompareTo((BigInteger)minValue) < 0) {
              throw new OverflowException("This object's value is out of range");
            }
            return (int)bi;
          }
        case CBORObjectTypeExtendedFloat: {
            BigInteger bi = ((ExtendedFloat)this.ThisItem).ToBigInteger();
            if (bi.CompareTo((BigInteger)maxValue) > 0 ||
                bi.CompareTo((BigInteger)minValue) < 0) {
              throw new OverflowException("This object's value is out of range");
            }
            return (int)bi;
          }
        default:
          throw new InvalidOperationException("Not a number type");
      }
    }

    /// <summary>Converts this object to a 32-bit signed integer. Floating
    /// point values are truncated to an integer.</summary>
    /// <returns>The closest big integer to this object.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    /// <exception cref='System.OverflowException'>This object's value
    /// exceeds the range of a 32-bit signed integer.</exception>
    public int AsInt32() {
      return this.AsInt32(Int32.MinValue, Int32.MaxValue);
    }

    /// <summary>Gets the value of this object as a string object.</summary>
    /// <returns>Gets this object's string.</returns>
    /// <exception cref='InvalidOperationException'>This object's
    /// type is not a string.</exception>
    public string AsString() {
      int type = this.ItemType;
      switch (type) {
        case CBORObjectTypeTextString: {
            return (string)this.ThisItem;
          }
        default:
          throw new InvalidOperationException("Not a string type");
      }
    }

    /// <summary>Reads an object in CBOR format from a data stream.</summary>
    /// <param name='stream'>A readable data stream.</param>
    /// <returns>A CBOR object that was read.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='CBORException'>There was an error in reading
    /// or parsing the data.</exception>
    public static CBORObject Read(Stream stream) {
      try {
        return Read(stream, 0, false, -1, null, 0);
      } catch (IOException ex) {
        throw new CBORException("I/O error occurred.", ex);
      }
    }

    private static void WriteObjectArray(
      IList<CBORObject> list,
      Stream s) {
      WritePositiveInt(4, list.Count, s);
      foreach (CBORObject i in list) {
        if (i == null) {
          s.WriteByte(0xf6);
        } else {
          i.WriteTo(s);
        }
      }
    }

    private static void WriteObjectMap(IDictionary<CBORObject, CBORObject> map, Stream s) {
      WritePositiveInt(5, map.Count, s);
      foreach (KeyValuePair<CBORObject, CBORObject> entry in map) {
        CBORObject key = entry.Key;
        CBORObject value = entry.Value;
        if (key == null) {
          s.WriteByte(0xf6);
        } else {
          key.WriteTo(s);
        }
        if (value == null) {
          s.WriteByte(0xf6);
        } else {
          value.WriteTo(s);
        }
      }
    }

    private static byte[] GetPositiveIntBytes(int type, int value) {
      if (value < 0) {
        throw new ArgumentException("value" +
                                    " not greater or equal to " + "0" + " (" +
                                    Convert.ToString((int)value, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      if (value < 24) {
        return new byte[] { (byte)((byte)value | (byte)(type << 5)) };
      } else if (value <= 0xFF) {
        return new byte[] { (byte)(24 | (type << 5)),
          (byte)(value & 0xFF) };
      } else if (value <= 0xFFFF) {
        return new byte[] { (byte)(25 | (type << 5)),
          (byte)((value >> 8) & 0xFF),
          (byte)(value & 0xFF) };
      } else {
        return new byte[] { (byte)(26 | (type << 5)),
          (byte)((value >> 24) & 0xFF),
          (byte)((value >> 16) & 0xFF),
          (byte)((value >> 8) & 0xFF),
          (byte)(value & 0xFF) };
      }
    }

    private static void WritePositiveInt(int type, int value, Stream s) {
      byte[] bytes = GetPositiveIntBytes(type, value);
      s.Write(bytes, 0, bytes.Length);
    }

    private static byte[] GetPositiveInt64Bytes(int type, long value) {
      if (value < 0) {
        throw new ArgumentException("value" + " not greater or equal to " + "0" + " (" + Convert.ToString((long)value, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      if (value < 24) {
        return new byte[] { (byte)((byte)value | (byte)(type << 5)) };
      } else if (value <= 0xFF) {
        return new byte[] { (byte)(24 | (type << 5)),
          (byte)(value & 0xFF) };
      } else if (value <= 0xFFFF) {
        return new byte[] { (byte)(25 | (type << 5)),
          (byte)((value >> 8) & 0xFF),
          (byte)(value & 0xFF) };
      } else if (value <= 0xFFFFFFFF) {
        return new byte[] { (byte)(26 | (type << 5)),
          (byte)((value >> 24) & 0xFF),
          (byte)((value >> 16) & 0xFF),
          (byte)((value >> 8) & 0xFF),
          (byte)(value & 0xFF) };
      } else {
        return new byte[] { (byte)(27 | (type << 5)),
          (byte)((value >> 56) & 0xFF),
          (byte)((value >> 48) & 0xFF),
          (byte)((value >> 40) & 0xFF),
          (byte)((value >> 32) & 0xFF),
          (byte)((value >> 24) & 0xFF),
          (byte)((value >> 16) & 0xFF),
          (byte)((value >> 8) & 0xFF),
          (byte)(value & 0xFF) };
      }
    }

    private static void WritePositiveInt64(int type, long value, Stream s) {
      byte[] bytes = GetPositiveInt64Bytes(type, value);
      s.Write(bytes, 0, bytes.Length);
    }

    private const int StreamedStringBufferLength = 4096;

    private static void WriteStreamedString(String str, Stream stream) {
      byte[] bytes;
      bytes = GetOptimizedBytesIfShortAscii(str, -1);
      if (bytes != null) {
        stream.Write(bytes, 0, bytes.Length);
        return;
      }
      bytes = new byte[StreamedStringBufferLength];
      int byteIndex = 0;
      bool streaming = false;
      for (int index = 0; index < str.Length; ++index) {
        int c = str[index];
        if (c <= 0x7F) {
          if (byteIndex >= StreamedStringBufferLength) {
            // Write bytes retrieved so far
            if (!streaming) {
              stream.WriteByte((byte)0x7F);
            }
            WritePositiveInt(3, byteIndex, stream);
            stream.Write(bytes, 0, byteIndex);
            byteIndex = 0;
            streaming = true;
          }
          bytes[byteIndex++] = (byte)c;
        } else if (c <= 0x7FF) {
          if (byteIndex + 2 > StreamedStringBufferLength) {
            // Write bytes retrieved so far
            if (!streaming) {
              stream.WriteByte((byte)0x7F);
            }
            WritePositiveInt(3, byteIndex, stream);
            stream.Write(bytes, 0, byteIndex);
            byteIndex = 0;
            streaming = true;
          }
          bytes[byteIndex++] = (byte)(0xC0 | ((c >> 6) & 0x1F));
          bytes[byteIndex++] = (byte)(0x80 | (c & 0x3F));
        } else {
          if (c >= 0xD800 && c <= 0xDBFF && index + 1 < str.Length &&
              str[index + 1] >= 0xDC00 && str[index + 1] <= 0xDFFF) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c - 0xD800) * 0x400) + (str[index + 1] - 0xDC00);
            ++index;
          } else if (c >= 0xD800 && c <= 0xDFFF) {
            // unpaired surrogate, write U + FFFD instead
            c = 0xFFFD;
          }
          if (c <= 0xFFFF) {
            if (byteIndex + 3 > StreamedStringBufferLength) {
              // Write bytes retrieved so far
              if (!streaming) {
                stream.WriteByte((byte)0x7F);
              }
              WritePositiveInt(3, byteIndex, stream);
              stream.Write(bytes, 0, byteIndex);
              byteIndex = 0;
              streaming = true;
            }
            bytes[byteIndex++] = (byte)(0xE0 | ((c >> 12) & 0x0F));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 6) & 0x3F));
            bytes[byteIndex++] = (byte)(0x80 | (c & 0x3F));
          } else {
            if (byteIndex + 4 > StreamedStringBufferLength) {
              // Write bytes retrieved so far
              if (!streaming) {
                stream.WriteByte((byte)0x7F);
              }
              WritePositiveInt(3, byteIndex, stream);
              stream.Write(bytes, 0, byteIndex);
              byteIndex = 0;
              streaming = true;
            }
            bytes[byteIndex++] = (byte)(0xF0 | ((c >> 18) & 0x07));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 12) & 0x3F));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 6) & 0x3F));
            bytes[byteIndex++] = (byte)(0x80 | (c & 0x3F));
          }
        }
      }
      WritePositiveInt(3, byteIndex, stream);
      stream.Write(bytes, 0, byteIndex);
      if (streaming) {
        stream.WriteByte((byte)0xFF);
      }
    }

    /// <summary>Writes a string in CBOR format to a data stream.</summary>
    /// <param name='str'>The string to write. Can be null.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    public static void Write(string str, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      if (str == null) {
        stream.WriteByte(0xf6);  // Write null instead of string
      } else {
        WriteStreamedString(str, stream);
      }
    }

    private static BigInteger valueOneShift63 = BigInteger.One << 63;

    private static BigInteger valueLowestMajorType1 = BigInteger.Zero - (BigInteger.One << 64);

    private static BigInteger valueUInt64MaxValue = (BigInteger.One << 64) - BigInteger.One;

    /// <summary>Writes a binary floating-point number in CBOR format to
    /// a data stream.</summary>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.ArgumentException'>The value's exponent
    /// is less than -(2^64) or greater than (2^64-1).</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    /// <param name='bignum'>An ExtendedFloat object.</param>
    /// <param name='stream'>A writable data stream.</param>
    public static void Write(ExtendedFloat bignum, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      if (bignum == null) {
        stream.WriteByte(0xf6);
        return;
      }
      if ((bignum.IsZero && bignum.IsNegative) || bignum.IsInfinity() || bignum.IsNaN()) {
        Write(bignum.ToDouble(), stream);
        return;
      }
      BigInteger exponent = bignum.Exponent;
      if (exponent.IsZero) {
        Write(bignum.Mantissa, stream);
      } else {
        if (!BigIntFits(exponent)) {
          throw new ArgumentException("Exponent is too low or too high");
        }
        stream.WriteByte(0xC5);  // tag 5
        stream.WriteByte(0x82);  // array, length 2
        Write(bignum.Exponent, stream);
        Write(bignum.Mantissa, stream);
      }
    }

    /// <summary>Writes a decimal floating-point number in CBOR format
    /// to a data stream.</summary>
    /// <param name='bignum'>Decimal fraction to write.</param>
    /// <param name='stream'>Stream to write to.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    /// <exception cref='System.ArgumentException'>The value's exponent
    /// is less than -(2^64) or greater than (2^64-1).</exception>
    public static void Write(ExtendedDecimal bignum, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      if (bignum == null) {
        stream.WriteByte(0xf6);
        return;
      }
      if ((bignum.IsZero && bignum.IsNegative) || bignum.IsInfinity() || bignum.IsNaN()) {
        Write(bignum.ToDouble(), stream);
        return;
      }
      BigInteger exponent = bignum.Exponent;
      if (exponent.IsZero) {
        Write(bignum.Mantissa, stream);
      } else {
        if (!BigIntFits(exponent)) {
          throw new ArgumentException("Exponent is too low or too high");
        }
        stream.WriteByte(0xC4);  // tag 4
        stream.WriteByte(0x82);  // array, length 2
        Write(bignum.Exponent, stream);
        Write(bignum.Mantissa, stream);
      }
    }

    /// <summary>Writes a big integer in CBOR format to a data stream.</summary>
    /// <param name='bigint'>Big integer to write.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    /// <param name='stream'>A writable data stream.</param>
    public static void Write(BigInteger bigint, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      if ((object)bigint == (object)null) {
        stream.WriteByte(0xf6);
        return;
      }
      int datatype = 0;
      if (bigint.Sign < 0) {
        datatype = 1;
        bigint += (BigInteger)BigInteger.One;
        bigint = -(BigInteger)bigint;
      }
      if (bigint.CompareTo(Int64MaxValue) <= 0) {
        // If the big integer is representable as a long and in
        // major type 0 or 1, write that major type
        // instead of as a bignum
        long ui = (long)(BigInteger)bigint;
        WritePositiveInt64(datatype, ui, stream);
      } else {
        // Get a byte array of the big integer's value,
        // since shifting and doing AND operations is
        // slow with large BigIntegers
        byte[] bytes = bigint.ToByteArray();
        int byteCount = bytes.Length;
        while (byteCount > 0 && bytes[byteCount - 1] == 0) {
          // Ignore trailing zero bytes
          --byteCount;
        }
        if (byteCount == 0) {
          WritePositiveInt64(datatype, 0, stream);
          return;
        }
        int half = byteCount >> 1;
        int right = byteCount - 1;
        for (int i = 0; i < half; i++, right--) {
          byte value = bytes[i];
          bytes[i] = bytes[right];
          bytes[right] = value;
        }
        switch (byteCount) {
          case 1:
            WritePositiveInt(datatype, ((int)bytes[0]) & 0xFF, stream);
            break;
          case 2:
            stream.WriteByte((byte)((datatype << 5) | 24));
            stream.Write(bytes, 0, byteCount);
            break;
          case 3:
            stream.WriteByte((byte)((datatype << 5) | 24));
            stream.Write(bytes, 0, byteCount);
            break;
          case 4:
            stream.WriteByte((byte)((datatype << 5) | 24));
            stream.Write(bytes, 0, byteCount);
            break;
          default:
            stream.WriteByte((datatype == 0) ?
                             (byte)0xC2 :
                             (byte)0xC3);
            WritePositiveInt(2, byteCount, stream);
            stream.Write(bytes, 0, byteCount);
            break;
        }
      }
    }

    /// <summary>Writes this CBOR object to a data stream.</summary>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    /// <param name='stream'>A writable data stream.</param>
    public void WriteTo(Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      this.WriteTags(stream);
      int type = this.ItemType;
      if (type == CBORObjectTypeInteger) {
        Write((long)this.ThisItem, stream);
      } else if (type == CBORObjectTypeBigInteger) {
        Write((BigInteger)this.ThisItem, stream);
      } else if (type == CBORObjectTypeByteString) {
        byte[] arr = (byte[])this.ThisItem;
        WritePositiveInt(
          (this.ItemType == CBORObjectTypeByteString) ? 2 : 3,
          arr.Length,
          stream);
        stream.Write(arr, 0, arr.Length);
      } else if (type == CBORObjectTypeTextString) {
        Write((string)this.ThisItem, stream);
      } else if (type == CBORObjectTypeArray) {
        WriteObjectArray(this.AsList(), stream);
      } else if (type == CBORObjectTypeExtendedDecimal) {
        ExtendedDecimal dec = (ExtendedDecimal)this.ThisItem;
        Write(dec, stream);
      } else if (type == CBORObjectTypeExtendedFloat) {
        ExtendedFloat dec = (ExtendedFloat)this.ThisItem;
        Write(dec, stream);
      } else if (type == CBORObjectTypeMap) {
        WriteObjectMap(this.AsMap(), stream);
      } else if (type == CBORObjectTypeSimpleValue) {
        int value = (int)this.ThisItem;
        if (value < 24) {
          stream.WriteByte((byte)(0xE0 + value));
        } else {
          stream.WriteByte(0xF8);
          stream.WriteByte((byte)value);
        }
      } else if (type == CBORObjectTypeSingle) {
        Write((float)this.ThisItem, stream);
      } else if (type == CBORObjectTypeDouble) {
        Write((double)this.ThisItem, stream);
      } else {
        throw new ArgumentException("Unexpected data type");
      }
    }

    /// <summary>Writes a 64-bit unsigned integer in CBOR format to a data
    /// stream.</summary>
    /// <param name='value'>The value to write.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    /// <param name='stream'>A writable data stream.</param>
    public static void Write(long value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      if (value >= 0) {
        WritePositiveInt64(0, value, stream);
      } else {
        ++value;
        value = -value;  // Will never overflow
        WritePositiveInt64(1, value, stream);
      }
    }

    /// <summary>Writes a 32-bit signed integer in CBOR format to a data stream.</summary>
    /// <param name='value'>The value to write.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    /// <param name='stream'>A writable data stream.</param>
    public static void Write(int value, Stream stream) {
      Write((long)value, stream);
    }

    /// <summary>Writes a 16-bit signed integer in CBOR format to a data stream.</summary>
    /// <param name='value'>The value to write.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    /// <param name='stream'>A writable data stream.</param>
    public static void Write(short value, Stream stream) {
      Write((long)value, stream);
    }

    /// <summary>Writes a Unicode character as a string in CBOR format to
    /// a data stream.</summary>
    /// <param name='value'>The value to write.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='value'/> is a surrogate code point.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    /// <param name='stream'>A writable data stream.</param>
    public static void Write(char value, Stream stream) {
      if (value >= 0xd800 && value < 0xe000) {
        throw new ArgumentException("Value is a surrogate code point.");
      }
      Write(new String(new char[] { value }), stream);
    }

    /// <summary>Writes a Boolean value in CBOR format to a data stream.</summary>
    /// <param name='value'>The value to write.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    /// <param name='stream'>A writable data stream.</param>
    public static void Write(bool value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      stream.WriteByte(value ? (byte)0xf5 : (byte)0xf4);
    }

    /// <summary>Writes a byte (0 to 255) in CBOR format to a data stream.</summary>
    /// <param name='value'>The value to write.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    /// <param name='stream'>A writable data stream.</param>
    public static void Write(byte value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      if ((((int)value) & 0xFF) < 24) {
        stream.WriteByte(value);
      } else {
        stream.WriteByte((byte)24);
        stream.WriteByte(value);
      }
    }

    /// <summary>Writes a 32-bit floating-point number in CBOR format to
    /// a data stream.</summary>
    /// <param name='value'>The value to write.</param>
    /// <param name='s'>A writable data stream.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='s'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    public static void Write(float value, Stream s) {
      if (s == null) {
        throw new ArgumentNullException("s");
      }
      int bits = BitConverter.ToInt32(BitConverter.GetBytes((float)value), 0);
      byte[] data = new byte[] { (byte)0xFA,
        (byte)((bits >> 24) & 0xFF),
        (byte)((bits >> 16) & 0xFF),
        (byte)((bits >> 8) & 0xFF),
        (byte)(bits & 0xFF) };
      s.Write(data, 0, 5);
    }

    /// <summary>Writes a 64-bit floating-point number in CBOR format to
    /// a data stream.</summary>
    /// <param name='value'>The value to write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    public static void Write(double value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      long bits = BitConverter.ToInt64(BitConverter.GetBytes((double)(double)value), 0);
      byte[] data = new byte[] { (byte)0xFB,
        (byte)((bits >> 56) & 0xFF),
        (byte)((bits >> 48) & 0xFF),
        (byte)((bits >> 40) & 0xFF),
        (byte)((bits >> 32) & 0xFF),
        (byte)((bits >> 24) & 0xFF),
        (byte)((bits >> 16) & 0xFF),
        (byte)((bits >> 8) & 0xFF),
        (byte)(bits & 0xFF) };
      stream.Write(data, 0, 9);
    }

    private static byte[] GetOptimizedBytesIfShortAscii(string str, int tagbyte) {
      byte[] bytes;
      if (str.Length <= 255) {
        // The strings will usually be short ASCII strings, so
        // use this optimization
        int offset = 0;
        int length = str.Length;
        int extra = (length < 24) ? 1 : 2;
        if (tagbyte >= 0) {
          ++extra;
        }
        bytes = new byte[length + extra];
        if (tagbyte >= 0) {
          bytes[offset] = (byte)tagbyte;
          ++offset;
        }
        if (length < 24) {
          bytes[offset] = (byte)(0x60 + str.Length);
          ++offset;
        } else {
          bytes[offset] = (byte)0x78;
          bytes[offset + 1] = (byte)str.Length;
          offset += 2;
        }
        bool issimple = true;
        for (int i = 0; i < str.Length; ++i) {
          char c = str[i];
          if (c >= 0x80) {
            issimple = false;
            break;
          }
          bytes[i + offset] = unchecked((byte)c);
        }
        if (issimple) {
          return bytes;
        }
      }
      return null;
    }

    /// <summary>Gets the binary representation of this data item.</summary>
    /// <returns>A byte array in CBOR format.</returns>
    public byte[] EncodeToBytes() {
      // For some types, a memory stream is a lot of
      // overhead since the amount of memory the types
      // use is fixed and small
      bool hasComplexTag = false;
      byte tagbyte = 0;
      bool tagged = this.IsTagged;
      if (this.IsTagged) {
        CBORObject taggedItem = (CBORObject)this.itemValue;
        if (taggedItem.IsTagged ||
            this.tagHigh != 0 ||
            (this.tagLow >> 16) != 0 ||
            this.tagLow >= 24) {
          hasComplexTag = true;
        } else {
          tagbyte = (byte)(0xC0 + (int)this.tagLow);
        }
      }
      if (!hasComplexTag) {
        if (this.ItemType == CBORObjectTypeTextString) {
          byte[] ret = GetOptimizedBytesIfShortAscii(
            this.AsString(),
            tagged ? (((int)tagbyte) & 0xFF) : -1);
          if (ret != null) {
            return ret;
          }
        } else if (this.ItemType == CBORObjectTypeSimpleValue) {
          if (tagged) {
            if (this.IsFalse) {
              return new byte[] { tagbyte, (byte)0xf4 };
            }
            if (this.IsTrue) {
              return new byte[] { tagbyte, (byte)0xf5 };
            }
            if (this.IsNull) {
              return new byte[] { tagbyte, (byte)0xf6 };
            }
            if (this.IsUndefined) {
              return new byte[] { tagbyte, (byte)0xf7 };
            }
          } else {
            if (this.IsFalse) {
              return new byte[] { (byte)0xf4 };
            }
            if (this.IsTrue) {
              return new byte[] { (byte)0xf5 };
            }
            if (this.IsNull) {
              return new byte[] { (byte)0xf6 };
            }
            if (this.IsUndefined) {
              return new byte[] { (byte)0xf7 };
            }
          }
        } else if (this.ItemType == CBORObjectTypeInteger) {
          long value = (long)this.ThisItem;
          byte[] intBytes = null;
          if (value >= 0) {
            intBytes = GetPositiveInt64Bytes(0, value);
          } else {
            ++value;
            value = -value;  // Will never overflow
            intBytes = GetPositiveInt64Bytes(1, value);
          }
          if (!tagged) {
            return intBytes;
          }
          byte[] ret2 = new byte[intBytes.Length + 1];
          Array.Copy(intBytes, 0, ret2, 1, intBytes.Length);
          ret2[0] = tagbyte;
          return ret2;
        } else if (this.ItemType == CBORObjectTypeSingle) {
          float value = (float)this.ThisItem;
          int bits = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
          return tagged ?
            new byte[] { tagbyte, (byte)0xFA,
            (byte)((bits >> 24) & 0xFF),
            (byte)((bits >> 16) & 0xFF),
            (byte)((bits >> 8) & 0xFF),
            (byte)(bits & 0xFF) } :
            new byte[] { (byte)0xFA,
            (byte)((bits >> 24) & 0xFF),
            (byte)((bits >> 16) & 0xFF),
            (byte)((bits >> 8) & 0xFF),
            (byte)(bits & 0xFF) };
        } else if (this.ItemType == CBORObjectTypeDouble) {
          double value = (double)this.ThisItem;
          long bits = BitConverter.ToInt64(BitConverter.GetBytes(value), 0);
          return tagged ?
            new byte[] { tagbyte, (byte)0xFB,
            (byte)((bits >> 56) & 0xFF),
            (byte)((bits >> 48) & 0xFF),
            (byte)((bits >> 40) & 0xFF),
            (byte)((bits >> 32) & 0xFF),
            (byte)((bits >> 24) & 0xFF),
            (byte)((bits >> 16) & 0xFF),
            (byte)((bits >> 8) & 0xFF),
            (byte)(bits & 0xFF) } :
            new byte[] { (byte)0xFB,
            (byte)((bits >> 56) & 0xFF),
            (byte)((bits >> 48) & 0xFF),
            (byte)((bits >> 40) & 0xFF),
            (byte)((bits >> 32) & 0xFF),
            (byte)((bits >> 24) & 0xFF),
            (byte)((bits >> 16) & 0xFF),
            (byte)((bits >> 8) & 0xFF),
            (byte)(bits & 0xFF) };
        }
      }
      try {
        using (MemoryStream ms = new MemoryStream(16)) {
          this.WriteTo(ms);
          return ms.ToArray();
        }
      } catch (IOException ex) {
        throw new CBORException("I/O Error occurred", ex);
      }
    }

    /// <summary>Writes a CBOR object to a CBOR data stream.</summary>
    /// <param name='value'>The value to write.</param>
    /// <param name='stream'>A writable data stream.</param>
    public static void Write(CBORObject value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      if (value == null) {
        stream.WriteByte(0xf6);
      } else {
        value.WriteTo(stream);
      }
    }

    /// <summary>Writes an arbitrary object to a CBOR data stream.</summary>
    /// <param name='objValue'>The value to write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='System.ArgumentException'>The object's type
    /// is not supported.</exception>
    public static void Write(object objValue, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      if (objValue == null) {
        stream.WriteByte(0xf6);
        return;
      }
      byte[] data = objValue as byte[];
      if (data != null) {
        WritePositiveInt(3, data.Length, stream);
        stream.Write(data, 0, data.Length);
        return;
      } else if (objValue is IList<CBORObject>) {
        WriteObjectArray((IList<CBORObject>)objValue, stream);
      } else if (objValue is IDictionary<CBORObject, CBORObject>) {
        WriteObjectMap((IDictionary<CBORObject, CBORObject>)objValue, stream);
      } else {
        FromObject(objValue).WriteTo(stream);
      }
    }

    // JSON parsing methods
    private static int SkipWhitespaceJSON(CharacterReader reader) {
      while (true) {
        int c = reader.NextChar();
        if (c == -1 || (c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09)) {
          return c;
        }
      }
    }

    private static string NextJSONString(CharacterReader reader, int quote) {
      int c;
      StringBuilder sb = new StringBuilder();
      bool surrogate = false;
      bool surrogateEscaped = false;
      bool escaped = false;
      while (true) {
        c = reader.NextChar();
        if (c == -1 || c < 0x20) {
          throw reader.NewError("Unterminated string");
        }
        switch (c) {
          case '\\':
            c = reader.NextChar();
            escaped = true;
            switch (c) {
              case '\\':
                c = '\\';
                break;
              case '/':
                // Not allowed to be escaped by RFC 4627,
                // but will be allowed in the revision to RFC 4627
                c = '/';
                break;
              case '\"':
                c = '\"';
                break;
              case 'b':
                c = '\b';
                break;
              case 'f':
                c = '\f';
                break;
              case 'n':
                c = '\n';
                break;
              case 'r':
                c = '\r';
                break;
              case 't':
                c = '\t';
                break;
                case 'u': { // Unicode escape
                  c = 0;
                  // Consists of 4 hex digits
                  for (int i = 0; i < 4; ++i) {
                    int ch = reader.NextChar();
                    if (ch >= '0' && ch <= '9') {
                      c <<= 4;
                      c |= ch - '0';
                    } else if (ch >= 'A' && ch <= 'F') {
                      c <<= 4;
                      c |= ch + 10 - 'A';
                    } else if (ch >= 'a' && ch <= 'f') {
                      c <<= 4;
                      c |= ch + 10 - 'a';
                    } else {
                      throw reader.NewError("Invalid Unicode escaped character");
                    }
                  }
                  break;
                }
              default:
                throw reader.NewError("Invalid escaped character");
            }
            break;
          default:
            escaped = false;
            break;
        }
        if (surrogate) {
          if ((c & 0x1FFC00) != 0xDC00) {
            // Note: this includes the ending quote
            // and supplementary characters
            throw reader.NewError("Unpaired surrogate code point");
          }
          if (escaped != surrogateEscaped) {
            throw reader.NewError("Pairing escaped surrogate with unescaped surrogate");
          }
          surrogate = false;
        } else if ((c & 0x1FFC00) == 0xD800) {
          surrogate = true;
          surrogateEscaped = escaped;
        } else if ((c & 0x1FFC00) == 0xDC00) {
          throw reader.NewError("Unpaired surrogate code point");
        }
        if (c == quote && !escaped) {
          // End quote reached
          return sb.ToString();
        }
        if (c <= 0xFFFF) {
          sb.Append((char)c);
        } else if (c <= 0x10FFFF) {
          sb.Append((char)((((c - 0x10000) >> 10) & 0x3FF) + 0xD800));
          sb.Append((char)(((c - 0x10000) & 0x3FF) + 0xDC00));
        }
      }
    }

    private static CBORObject NextJSONValue(CharacterReader reader, int firstChar, bool noDuplicates, int[] nextChar) {
      string str;
      int c = firstChar;
      CBORObject obj = null;
      if (c < 0) {
        throw reader.NewError("Unexpected end of data");
      }
      if (c == '"') {
        // Parse a string
        // The tokenizer already checked the string for invalid
        // surrogate pairs, so just call the CBORObject
        // constructor directly
        obj = CBORObject.FromRaw(NextJSONString(reader, c));
        nextChar[0] = SkipWhitespaceJSON(reader);
        return obj;
      } else if (c == '{') {
        // Parse an object
        obj = ParseJSONObject(reader, noDuplicates);
        nextChar[0] = SkipWhitespaceJSON(reader);
        return obj;
      } else if (c == '[') {
        // Parse an array
        obj = ParseJSONArray(reader, noDuplicates);
        nextChar[0] = SkipWhitespaceJSON(reader);
        return obj;
      } else if (c == 't') {
        // Parse true
        if (reader.NextChar() != 'r' ||
            reader.NextChar() != 'u' ||
            reader.NextChar() != 'e') {
          throw reader.NewError("Value can't be parsed.");
        }
        nextChar[0] = SkipWhitespaceJSON(reader);
        return CBORObject.True;
      } else if (c == 'f') {
        // Parse false
        if (reader.NextChar() != 'a' ||
            reader.NextChar() != 'l' ||
            reader.NextChar() != 's' ||
            reader.NextChar() != 'e') {
          throw reader.NewError("Value can't be parsed.");
        }
        nextChar[0] = SkipWhitespaceJSON(reader);
        return CBORObject.False;
      } else if (c == 'n') {
        // Parse null
        if (reader.NextChar() != 'u' ||
            reader.NextChar() != 'l' ||
            reader.NextChar() != 'l') {
          throw reader.NewError("Value can't be parsed.");
        }
        nextChar[0] = SkipWhitespaceJSON(reader);
        return CBORObject.Null;
      } else if (c == '-' || (c >= '0' && c <= '9')) {
        // Parse a number
        StringBuilder sb = new StringBuilder();
        while (c == '-' || c == '+' || c == '.' || (c >= '0' && c <= '9') || c == 'e' || c == 'E') {
          sb.Append((char)c);
          c = reader.NextChar();
        }
        str = sb.ToString();
        obj = CBORDataUtilities.ParseJSONNumber(str);
        if (obj == null) {
          throw reader.NewError("JSON number can't be parsed.");
        }
        if (c == -1 || (c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09)) {
          nextChar[0] = c;
        } else {
          nextChar[0] = SkipWhitespaceJSON(reader);
        }
        return obj;
      } else {
        throw reader.NewError("Value can't be parsed.");
      }
    }

    private static CBORObject ParseJSONObjectOrArray(CharacterReader reader, bool noDuplicates) {
      int c;
      c = SkipWhitespaceJSON(reader);
      if (c == '[') {
        return ParseJSONArray(reader, noDuplicates);
      }
      if (c == '{') {
        return ParseJSONObject(reader, noDuplicates);
      }
      throw reader.NewError("A JSON object must begin with '{' or '['");
    }

    private static CBORObject ParseJSONObject(CharacterReader reader, bool noDuplicates) {
      // Assumes that the last character read was '{'
      int c;
      CBORObject key;
      CBORObject obj;
      int[] nextchar = new int[1];
      bool seenComma = false;
      var myHashMap = new Dictionary<CBORObject, CBORObject>();
      while (true) {
        c = SkipWhitespaceJSON(reader);
        switch (c) {
          case -1:
            throw reader.NewError("A JSONObject must end with '}'");
          case '}':
            if (seenComma) {
              // Situation like '{"0"=>1,}'
              throw reader.NewError("Trailing comma");
            }
            return CBORObject.FromRaw(myHashMap);
            default: {
              // Read the next string
              if (c < 0) {
                throw reader.NewError("Unexpected end of data");
              }
              if (c != '"') {
                throw reader.NewError("Expected a string as a key");
              }
              // Parse a string that represents the object's key
              // The tokenizer already checked the string for invalid
              // surrogate pairs, so just call the CBORObject
              // constructor directly
              obj = CBORObject.FromRaw(NextJSONString(reader, c));
              key = obj;
              if (noDuplicates && myHashMap.ContainsKey(obj)) {
                throw reader.NewError("Key already exists: " + key);
              }
              break;
            }
        }
        if (SkipWhitespaceJSON(reader) != ':') {
          throw reader.NewError("Expected a ':' after a key");
        }
        // NOTE: Will overwrite existing value
        myHashMap[key] = NextJSONValue(reader, SkipWhitespaceJSON(reader), noDuplicates, nextchar);
        switch (nextchar[0]) {
          case ',':
            seenComma = true;
            break;
          case '}':
            return CBORObject.FromRaw(myHashMap);
          default:
            throw reader.NewError("Expected a ',' or '}'");
        }
      }
    }

    private static CBORObject ParseJSONArray(CharacterReader reader, bool noDuplicates) {
      var myArrayList = new List<CBORObject>();
      bool seenComma = false;
      int[] nextchar = new int[1];
      // This method assumes that the last character read was '['
      while (true) {
        int c = SkipWhitespaceJSON(reader);
        if (c == ']') {
          if (seenComma) {
            // Situation like '[0,1,]'
            throw reader.NewError("Trailing comma");
          }
          return CBORObject.FromRaw(myArrayList);
        } else if (c == ',') {
          // Situation like '[,0,1,2]' or '[0,,1]'
          throw reader.NewError("Empty array element");
        } else {
          myArrayList.Add(NextJSONValue(reader, c, noDuplicates, nextchar));
          c = nextchar[0];
        }
        switch (c) {
          case ',':
            seenComma = true;
            break;
          case ']':
            return CBORObject.FromRaw(myArrayList);
          default:
            throw reader.NewError("Expected a ',' or ']'");
        }
      }
    }

    /// <summary>Generates a CBOR object from a string in JavaScript Object
    /// Notation (JSON) format. This function only accepts maps and arrays.
    /// <para>If a JSON object has the same key, only the last given value will
    /// be used for each duplicated key.</para>
    /// </summary>
    /// <param name='str'>A string in JSON format.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='str'/> is null.</exception>
    /// <exception cref='CBORException'>The string is not in JSON format.</exception>
    /// <returns>A CBORObject object.</returns>
    public static CBORObject FromJSONString(string str) {
      CharacterReader reader = new CharacterReader(str);
      CBORObject obj = ParseJSONObjectOrArray(reader, false);
      if (SkipWhitespaceJSON(reader) != -1) {
        throw reader.NewError("End of string not reached");
      }
      return obj;
    }

    /// <summary>Generates a CBOR object from a data stream in JavaScript
    /// Object Notation (JSON) format and UTF-8 encoding. This function
    /// only accepts maps and arrays. <para>If a JSON object has the same key,
    /// only the last given value will be used for each duplicated key.</para>
    /// </summary>
    /// <param name='stream'>A readable data stream.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    /// <exception cref='CBORException'>The data stream contains invalid
    /// UTF-8 or is not in JSON format.</exception>
    /// <returns>A CBORObject object.</returns>
    public static CBORObject ReadJSON(Stream stream) {
      CharacterReader reader = new CharacterReader(stream);
      try {
        CBORObject obj = ParseJSONObjectOrArray(reader, false);
        if (SkipWhitespaceJSON(reader) != -1) {
          throw reader.NewError("End of data stream not reached");
        }
        return obj;
      } catch (CBORException ex) {
        if (ex.InnerException != null && ex.InnerException is IOException) {
          throw (IOException)ex.InnerException;
        }
        throw;
      }
    }

    private static void StringToJSONStringUnquoted(string str, StringBuilder sb) {
      // Surrogates were already verified when this
      // string was added to the CBOR object; that check
      // is not repeated here
      bool first = true;
      for (int i = 0; i < str.Length; ++i) {
        char c = str[i];
        if (c == '\\' || c == '"') {
          if (first) {
            first = false;
            sb.Append(str, 0, i);
          }
          sb.Append('\\');
          sb.Append(c);
        } else if (c < 0x20) {
          if (first) {
            first = false;
            sb.Append(str, 0, i);
          }
          if (c == 0x0d) {
            sb.Append("\\r");
          } else if (c == 0x0a) {
            sb.Append("\\n");
          } else if (c == 0x08) {
            sb.Append("\\b");
          } else if (c == 0x0c) {
            sb.Append("\\f");
          } else if (c == 0x09) {
            sb.Append("\\t");
          } else {
            sb.Append("\\u00");
            sb.Append((char)('0' + (int)(c >> 4)));
            sb.Append((char)('0' + (int)(c & 15)));
          }
        } else if (!first) {
          sb.Append(c);
        }
      }
      if (first) {
        sb.Append(str);
      }
    }

    /// <summary>Converts this object to a JSON string. This function works
    /// not only with arrays and maps (the only proper JSON objects under RFC
    /// 4627), but also integers, strings, byte arrays, and other JSON data
    /// types.</summary>
    /// <returns>A string object.</returns>
    public string ToJSONString() {
      int type = this.ItemType;
      switch (type) {
        case CBORObjectTypeSimpleValue: {
            if (this.IsTrue) {
              {
                return "true";
              }
            } else if (this.IsFalse) {
              {
                return "false";
              }
            } else if (this.IsNull) {
              {
                return "null";
              }
            } else {
              return "null";
            }
          }
        case CBORObjectTypeSingle: {
            float f = (float)this.ThisItem;
            if (Single.IsNegativeInfinity(f) ||
                Single.IsPositiveInfinity(f) ||
                Single.IsNaN(f)) {
              return "null";
            } else {
              return TrimDotZero(
                Convert.ToString(
                  (float)f,
                  CultureInfo.InvariantCulture));
            }
          }
        case CBORObjectTypeDouble: {
            double f = (double)this.ThisItem;
            if (Double.IsNegativeInfinity(f) ||
                Double.IsPositiveInfinity(f) ||
                Double.IsNaN(f)) {
              return "null";
            } else {
              return TrimDotZero(
                Convert.ToString(
                  (double)f,
                  CultureInfo.InvariantCulture));
            }
          }
        case CBORObjectTypeInteger: {
            return Convert.ToString((long)this.ThisItem, CultureInfo.InvariantCulture);
          }
        case CBORObjectTypeBigInteger: {
            return CBORUtilities.BigIntToString((BigInteger)this.ThisItem);
          }
        case CBORObjectTypeExtendedDecimal: {
            ExtendedDecimal dec = (ExtendedDecimal)this.ThisItem;
            if (dec.IsInfinity() || dec.IsNaN()) {
              return "null";
            }
            return dec.ToString();
          }
        case CBORObjectTypeExtendedFloat: {
            ExtendedFloat flo = (ExtendedFloat)this.ThisItem;
            if (flo.IsInfinity() || flo.IsNaN()) {
              return "null";
            }
            return flo.ToString();
          }
        default: {
            StringBuilder sb = new StringBuilder();
            this.ToJSONString(sb);
            return sb.ToString();
          }
      }
    }

    private void ToJSONString(StringBuilder sb) {
      int type = this.ItemType;
      switch (type) {
        case CBORObjectTypeSimpleValue:
        case CBORObjectTypeSingle:
        case CBORObjectTypeDouble:
        case CBORObjectTypeInteger:
        case CBORObjectTypeBigInteger:
        case CBORObjectTypeExtendedDecimal:
        case CBORObjectTypeExtendedFloat:
          sb.Append(this.ToJSONString());
          break;
        case CBORObjectTypeByteString: {
            sb.Append('\"');
            if (this.HasTag(22)) {
              CBORUtilities.ToBase64(sb, (byte[])this.ThisItem, false);
            } else if (this.HasTag(23)) {
              CBORUtilities.ToBase16(sb, (byte[])this.ThisItem);
            } else {
              CBORUtilities.ToBase64URL(sb, (byte[])this.ThisItem, false);
            }
            sb.Append('\"');
            break;
          }
        case CBORObjectTypeTextString: {
            sb.Append('\"');
            StringToJSONStringUnquoted((string)this.ThisItem, sb);
            sb.Append('\"');
            break;
          }
        case CBORObjectTypeArray: {
            bool first = true;
            sb.Append('[');
            foreach (CBORObject i in this.AsList()) {
              if (!first) {
                sb.Append(',');
              }
              i.ToJSONString(sb);
              first = false;
            }
            sb.Append(']');
            break;
          }
        case CBORObjectTypeMap: {
            bool first = true;
            bool hasNonStringKeys = false;
            IDictionary<CBORObject, CBORObject> objMap = this.AsMap();
            sb.Append('{');
            int oldLength = sb.Length;
            foreach (KeyValuePair<CBORObject, CBORObject> entry in objMap) {
              CBORObject key = entry.Key;
              CBORObject value = entry.Value;
              if (key.ItemType != CBORObjectTypeTextString) {
                hasNonStringKeys = true;
                break;
              }
              if (!first) {
                sb.Append(',');
              }
              sb.Append('\"');
              StringToJSONStringUnquoted((string)key.ThisItem, sb);
              sb.Append('\"');
              sb.Append(':');
              value.ToJSONString(sb);
              first = false;
            }
            if (hasNonStringKeys) {
              sb.Remove(oldLength, sb.Length - oldLength);
              var stringMap = new Dictionary<String, CBORObject>();
              // Copy to a map with String keys, since
              // some keys could be duplicates
              // when serialized to strings
              foreach (KeyValuePair<CBORObject, CBORObject> entry in objMap) {
                CBORObject key = entry.Key;
                CBORObject value = entry.Value;
                string str = (key.ItemType == CBORObjectTypeTextString) ?
                  ((string)key.ThisItem) : key.ToJSONString();
                stringMap[str] = value;
              }
              first = true;
              foreach (KeyValuePair<string, CBORObject> entry in stringMap) {
                string key = entry.Key;
                CBORObject value = entry.Value;
                if (!first) {
                  sb.Append(',');
                }
                sb.Append('\"');
                StringToJSONStringUnquoted(key, sb);
                sb.Append('\"');
                sb.Append(':');
                value.ToJSONString(sb);
                first = false;
              }
            }
            sb.Append('}');
            break;
          }
        default:
          throw new InvalidOperationException("Unexpected data type");
      }
    }

    internal static CBORObject FromRaw(String str) {
      return new CBORObject(CBORObjectTypeTextString, str);
    }

    internal static CBORObject FromRaw(IList<CBORObject> list) {
      return new CBORObject(CBORObjectTypeArray, list);
    }

    internal static CBORObject FromRaw(IDictionary<CBORObject, CBORObject> map) {
      return new CBORObject(CBORObjectTypeMap, map);
    }

    /// <summary>Finds the sum of two CBOR number objects.</summary>
    /// <exception cref='System.ArgumentException'>Either or both operands
    /// are not numbers (as opposed to Not-a-Number, NaN).</exception>
    /// <returns>A CBORObject object.</returns>
    /// <param name='first'>A CBORObject object. (2).</param>
    /// <param name='second'>A CBORObject object. (3).</param>
    public static CBORObject Addition(CBORObject first, CBORObject second) {
      return CBORObjectMath.Addition(first, second);
    }

    /// <summary>Finds the difference between two CBOR number objects.</summary>
    /// <exception cref='System.ArgumentException'>Either or both operands
    /// are not numbers (as opposed to Not-a-Number, NaN).</exception>
    /// <returns>The difference of the two objects.</returns>
    /// <param name='first'>A CBORObject object.</param>
    /// <param name='second'>A CBORObject object. (2).</param>
    public static CBORObject Subtract(CBORObject first, CBORObject second) {
      return CBORObjectMath.Subtract(first, second);
    }

    /// <summary>Multiplies two CBOR number objects.</summary>
    /// <exception cref='System.ArgumentException'>Either or both operands
    /// are not numbers (as opposed to Not-a-Number, NaN).</exception>
    /// <returns>The product of the two objects.</returns>
    /// <param name='first'>A CBORObject object.</param>
    /// <param name='second'>A CBORObject object. (2).</param>
    public static CBORObject Multiply(CBORObject first, CBORObject second) {
      return CBORObjectMath.Multiply(first, second);
    }

    /// <summary>Creates a new empty CBOR array.</summary>
    /// <returns>A new CBOR array.</returns>
    public static CBORObject NewArray() {
      return FromObject(new List<CBORObject>());
    }

    /// <summary>Creates a new empty CBOR map.</summary>
    /// <returns>A new CBOR map.</returns>
    public static CBORObject NewMap() {
      return FromObject(new Dictionary<CBORObject, CBORObject>());
    }

    /// <summary>Generates a CBOR object from a 64-bit signed integer.</summary>
    /// <returns>A CBORObject object.</returns>
    /// <param name='value'>A 64-bit signed integer.</param>
    public static CBORObject FromObject(long value) {
      return new CBORObject(CBORObjectTypeInteger, value);
    }

    /// <summary>Generates a CBOR object from a CBOR object.</summary>
    /// <param name='value'>A CBOR object.</param>
    /// <returns>Same as.</returns>
    public static CBORObject FromObject(CBORObject value) {
      if (value == null) {
        return CBORObject.Null;
      }
      return value;
    }

    /// <summary>Generates a CBOR object from an arbitrary-precision integer.</summary>
    /// <param name='bigintValue'>An arbitrary-precision value.</param>
    /// <returns>A CBOR number object.</returns>
    public static CBORObject FromObject(BigInteger bigintValue) {
      if ((object)bigintValue == (object)null) {
        return CBORObject.Null;
      }
      if (bigintValue.CompareTo(Int64MinValue) >= 0 &&
          bigintValue.CompareTo(Int64MaxValue) <= 0) {
        return new CBORObject(CBORObjectTypeInteger, (long)(BigInteger)bigintValue);
      } else {
        return new CBORObject(CBORObjectTypeBigInteger, bigintValue);
      }
    }

    /// <summary>Generates a CBOR object from an arbitrary-precision binary
    /// floating-point number.</summary>
    /// <param name='bigValue'>An arbitrary-precision binary floating-point
    /// number.</param>
    /// <returns>A CBOR number object.</returns>
    /// <exception cref='System.ArgumentException'>The value's exponent
    /// is less than -(2^64) or greater than (2^64-1).</exception>
    public static CBORObject FromObject(ExtendedFloat bigValue) {
      if ((object)bigValue == (object)null) {
        return CBORObject.Null;
      }
      if (bigValue.IsNaN() || bigValue.IsInfinity()) {
        return new CBORObject(CBORObjectTypeExtendedFloat, bigValue);
      }
      BigInteger bigintExponent = bigValue.Exponent;
      if (bigintExponent.IsZero && !(bigValue.IsZero && bigValue.IsNegative)) {
        return FromObject(bigValue.Mantissa);
      } else {
        if (!BigIntFits(bigintExponent)) {
          throw new ArgumentException("Exponent is too low or too high");
        }
        return new CBORObject(CBORObjectTypeExtendedFloat, bigValue);
      }
    }

    /// <summary>Generates a CBOR object from a decimal fraction.</summary>
    /// <param name='numberObject'>An arbitrary-precision decimal number.</param>
    /// <returns>A CBOR number object.</returns>
    /// <exception cref='System.ArgumentException'>The value's exponent
    /// is less than -(2^64) or greater than (2^64-1).</exception>
    public static CBORObject FromObject(ExtendedDecimal numberObject) {
      if ((object)numberObject == (object)null) {
        return CBORObject.Null;
      }
      if (numberObject.IsNaN() || numberObject.IsInfinity()) {
        return new CBORObject(CBORObjectTypeExtendedDecimal, numberObject);
      }
      BigInteger bigintExponent = numberObject.Exponent;
      if (bigintExponent.IsZero && !(numberObject.IsZero && numberObject.IsNegative)) {
        return FromObject(numberObject.Mantissa);
      } else {
        if (!BigIntFits(bigintExponent)) {
          throw new ArgumentException("Exponent is too low or too high");
        }
        return new CBORObject(CBORObjectTypeExtendedDecimal, numberObject);
      }
    }

    /// <summary>Generates a CBOR object from a string.</summary>
    /// <param name='strValue'>A string value. Can be null.</param>
    /// <returns>A CBOR object representing the string, or CBORObject.Null
    /// if stringValue is null.</returns>
    /// <exception cref='System.ArgumentException'>The string contains
    /// an unpaired surrogate code point.</exception>
    public static CBORObject FromObject(string strValue) {
      if (strValue == null) {
        return CBORObject.Null;
      }
      if (DataUtilities.GetUtf8Length(strValue, false) < 0) {
        throw new ArgumentException("String contains an unpaired surrogate code point.");
      }
      return new CBORObject(CBORObjectTypeTextString, strValue);
    }

    /// <summary>Generates a CBOR object from a 32-bit signed integer.</summary>
    /// <returns>A CBORObject object.</returns>
    /// <param name='value'>A 32-bit signed integer.</param>
    public static CBORObject FromObject(int value) {
      return FromObject((long)value);
    }

    /// <summary>Generates a CBOR object from a 16-bit signed integer.</summary>
    /// <returns>A CBORObject object.</returns>
    /// <param name='value'>A 16-bit signed integer.</param>
    public static CBORObject FromObject(short value) {
      return FromObject((long)value);
    }

    /// <summary>Generates a CBOR string object from a Unicode character.</summary>
    /// <returns>A CBORObject object.</returns>
    /// <param name='value'>A char object.</param>
    public static CBORObject FromObject(char value) {
      return FromObject(new String(new char[] { value }));
    }

    /// <summary>Returns the CBOR true value or false value, depending on
    /// &quot;value&quot;.</summary>
    /// <returns>A CBORObject object.</returns>
    /// <param name='value'>A Boolean object.</param>
    public static CBORObject FromObject(bool value) {
      return value ? CBORObject.True : CBORObject.False;
    }

    /// <summary>Generates a CBOR object from a byte (0 to 255).</summary>
    /// <returns>A CBORObject object.</returns>
    /// <param name='value'>A Byte object.</param>
    public static CBORObject FromObject(byte value) {
      return FromObject(((int)value) & 0xFF);
    }

    /// <summary>Generates a CBOR object from a 32-bit floating-point number.</summary>
    /// <returns>A CBORObject object.</returns>
    /// <param name='value'>A 32-bit floating-point number.</param>
    public static CBORObject FromObject(float value) {
      return new CBORObject(CBORObjectTypeSingle, value);
    }

    /// <summary>Generates a CBOR object from a 64-bit floating-point number.</summary>
    /// <returns>A CBORObject object.</returns>
    /// <param name='value'>A 64-bit floating-point number.</param>
    public static CBORObject FromObject(double value) {
      return new CBORObject(CBORObjectTypeDouble, value);
    }

    /// <summary>Generates a CBOR object from a byte array. The byte array
    /// is copied to a new byte array.</summary>
    /// <param name='bytes'>A byte array. Can be null.</param>
    /// <returns>A CBOR byte string object where each byte of the given byte
    /// array is copied to a new array, or CBORObject.Null if the value is null.</returns>
    public static CBORObject FromObject(byte[] bytes) {
      if (bytes == null) {
        return CBORObject.Null;
      }
      byte[] newvalue = new byte[bytes.Length];
      Array.Copy(bytes, 0, newvalue, 0, bytes.Length);
      return new CBORObject(CBORObjectTypeByteString, bytes);
    }

    /// <summary>Generates a CBOR object from an array of CBOR objects.</summary>
    /// <param name='array'>An array of CBOR objects.</param>
    /// <returns>A CBOR object where each element of the given array is copied
    /// to a new array, or CBORObject.Null if the value is null.</returns>
    public static CBORObject FromObject(CBORObject[] array) {
      if (array == null) {
        return CBORObject.Null;
      }
      IList<CBORObject> list = new List<CBORObject>();
      foreach (CBORObject i in array) {
        list.Add(FromObject(i));
      }
      return new CBORObject(CBORObjectTypeArray, list);
    }

    /// <summary>Generates a CBOR object from an array of 32-bit integers.</summary>
    /// <param name='array'>An array of 32-bit integers.</param>
    /// <returns>A CBOR array object where each element of the given array
    /// is copied to a new array, or CBORObject.Null if the value is null.</returns>
    public static CBORObject FromObject(int[] array) {
      if (array == null) {
        return CBORObject.Null;
      }
      IList<CBORObject> list = new List<CBORObject>();
      foreach (int i in array) {
        list.Add(FromObject(i));
      }
      return new CBORObject(CBORObjectTypeArray, list);
    }

    /// <summary>Generates a CBOR object from an array of 64-bit integers.</summary>
    /// <param name='array'>An array of 64-bit integers.</param>
    /// <returns>A CBOR array object where each element of the given array
    /// is copied to a new array, or CBORObject.Null if the value is null.</returns>
    public static CBORObject FromObject(long[] array) {
      if (array == null) {
        return CBORObject.Null;
      }
      IList<CBORObject> list = new List<CBORObject>();
      foreach (long i in array) {
        list.Add(FromObject(i));
      }
      return new CBORObject(CBORObjectTypeArray, list);
    }

    /// <summary>Generates a CBOR object from an list of objects.</summary>
    /// <param name='value'>An array of CBOR objects.</param>
    /// <returns>A CBOR object where each element of the given array is converted
    /// to a CBOR object and copied to a new array, or CBORObject.Null if the
    /// value is null.</returns>
    /// <typeparam name='T'>A type convertible to CBORObject.</typeparam>
    public static CBORObject FromObject<T>(IList<T> value) {
      if (value == null) {
        return CBORObject.Null;
      }
      IList<CBORObject> list = new List<CBORObject>();
      foreach (T i in (IList<T>)value) {
        CBORObject obj = FromObject(i);
        list.Add(obj);
      }
      return new CBORObject(CBORObjectTypeArray, list);
    }

    /// <summary>Generates a CBOR object from a map of objects.</summary>
    /// <param name='dic'>A map of CBOR objects.</param>
    /// <returns>A CBOR object where each key and value of the given map is
    /// converted to a CBOR object and copied to a new map, or CBORObject.Null
    /// if.</returns>
    /// <typeparam name='TKey'>A type convertible to CBORObject; the type
    /// of the keys.</typeparam>
    /// <typeparam name='TValue'>A type convertible to CBORObject; the
    /// type of the values.</typeparam>
    public static CBORObject FromObject<TKey, TValue>(IDictionary<TKey, TValue> dic) {
      if (dic == null) {
        return CBORObject.Null;
      }
      var map = new Dictionary<CBORObject, CBORObject>();
      foreach (KeyValuePair<TKey, TValue> entry in dic) {
        CBORObject key = FromObject(entry.Key);
        CBORObject value = FromObject(entry.Value);
        map[key] = value;
      }
      return new CBORObject(CBORObjectTypeMap, map);
    }

    /// <summary>Generates a CBORObject from an arbitrary object.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A CBOR object corresponding to the given object. Returns
    /// CBORObject.Null if the object is null.</returns>
    /// <exception cref='System.ArgumentException'>The object's type
    /// is not supported.</exception>
    public static CBORObject FromObject(object obj) {
      if (obj == null) {
        return CBORObject.Null;
      }
      if (obj is long) {
        return FromObject((long)obj);
      }
      if (obj is CBORObject) {
        return FromObject((CBORObject)obj);
      }
      if (obj is BigInteger) {
        return FromObject((BigInteger)obj);
      }
      ExtendedDecimal df = obj as ExtendedDecimal;
      if (df != null) {
        return FromObject(df);
      }
      ExtendedFloat bf = obj as ExtendedFloat;
      if (bf != null) {
        return FromObject(bf);
      }
      if (obj is string) {
        return FromObject((string)obj);
      }
      if (obj is int) {
        return FromObject((int)obj);
      }
      if (obj is short) {
        return FromObject((short)obj);
      }
      if (obj is char) {
        return FromObject((char)obj);
      }
      if (obj is bool) {
        return FromObject((bool)obj);
      }
      if (obj is byte) {
        return FromObject((byte)obj);
      }
      if (obj is float) {
        return FromObject((float)obj);
      }
      if (obj is sbyte) {
        return FromObject((sbyte)obj);
      }
      if (obj is ulong) {
        return FromObject((ulong)obj);
      }
      if (obj is uint) {
        return FromObject((uint)obj);
      }
      if (obj is ushort) {
        return FromObject((ushort)obj);
      }
      if (obj is decimal) {
        return FromObject((decimal)obj);
      }
      if (obj is DateTime) {
        return FromObject((DateTime)obj);
      }
      if (obj is double) {
        return FromObject((double)obj);
      }
      if (obj is IList<CBORObject>) {
        return FromObject((IList<CBORObject>)obj);
      }
      byte[] bytearr = obj as byte[];
      if (bytearr != null) {
        return FromObject(bytearr);
      }
      int[] intarr = obj as int[];
      if (intarr != null) {
        return FromObject(intarr);
      }
      long[] longarr = obj as long[];
      if (longarr != null) {
        return FromObject(longarr);
      }
      if (obj is CBORObject[]) {
        return FromObject((CBORObject[])obj);
      }
      if (obj is IDictionary<CBORObject, CBORObject>) {
        return FromObject((IDictionary<CBORObject, CBORObject>)obj);
      }
      if (obj is IDictionary<string, CBORObject>) {
        return FromObject((IDictionary<string, CBORObject>)obj);
      }
      throw new ArgumentException("Unsupported object type.");
    }

    private static BigInteger valueBigInt65536 = (BigInteger)65536;

    /// <summary>Generates a CBOR object from an arbitrary object and gives
    /// the resulting object a tag.</summary>
    /// <param name='o'>An arbitrary object.</param>
    /// <param name='bigintTag'>A big integer that specifies a tag number.</param>
    /// <returns>A CBOR object where the object.</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='bigintTag'/> is less than 0 or greater than 2^64-1, or <paramref
    /// name='o'/>'s type is unsupported.</exception>
    public static CBORObject FromObjectAndTag(object o, BigInteger bigintTag) {
      if (bigintTag == null) {
        throw new ArgumentNullException("bigintTag");
      }
      if (bigintTag.Sign < 0) {
        throw new ArgumentException("tag not greater or equal to 0 (" + Convert.ToString(bigintTag, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      if (bigintTag.CompareTo(valueUInt64MaxValue) > 0) {
        throw new ArgumentException("tag not less or equal to 18446744073709551615 (" + Convert.ToString(bigintTag, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      CBORObject c = FromObject(o);
      if (bigintTag.CompareTo(valueBigInt65536) < 0) {
        // Low-numbered, commonly used tags
        return new CBORObject(c, (int)bigintTag, 0);
      } else if (bigintTag.CompareTo((BigInteger)2) == 0) {
        return ConvertToBigNum(c, false);
      } else if (bigintTag.CompareTo((BigInteger)3) == 0) {
        return ConvertToBigNum(c, true);
      } else if (bigintTag.CompareTo((BigInteger)4) == 0) {
        return ConvertToDecimalFrac(c, true);
      } else if (bigintTag.CompareTo((BigInteger)5) == 0) {
        return ConvertToDecimalFrac(c, false);
      } else {
        int tagLow = 0;
        int tagHigh = 0;
        byte[] bytes = bigintTag.ToByteArray();
        for (int i = 0; i < Math.Min(4, bytes.Length); ++i) {
          int b = ((int)bytes[i]) & 0xFF;
          tagLow = unchecked(tagLow | (((int)b) << (i * 8)));
        }
        for (int i = 4; i < Math.Min(8, bytes.Length); ++i) {
          int b = ((int)bytes[i]) & 0xFF;
          tagHigh = unchecked(tagHigh | (((int)b) << (i * 8)));
        }
        return new CBORObject(c, tagLow, tagHigh);
      }
    }

    /// <summary>Generates a CBOR object from an arbitrary object and gives
    /// the resulting object a tag.</summary>
    /// <param name='valueObValue'>An arbitrary object.</param>
    /// <param name='smallTag'>A 32-bit integer that specifies a tag number.</param>
    /// <returns>A CBOR object where the object <paramref name='valueObValue'/>
    /// is converted to a CBOR object and given the tag.</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='smallTag'/> is less than 0 or <paramref name='valueObValue'/>
    /// 's type is unsupported.</exception>
    public static CBORObject FromObjectAndTag(object valueObValue, int smallTag) {
      if (smallTag < 0) {
        throw new ArgumentException("tag not greater or equal to 0 (" + Convert.ToString((int)smallTag, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      CBORObject c = FromObject(valueObValue);
      if (smallTag == 2 || smallTag == 3) {
        return ConvertToBigNum(c, smallTag == 3);
      }
      if (smallTag == 4 || smallTag == 5) {
        return ConvertToDecimalFrac(c, smallTag == 4);
      }
      return new CBORObject(c, smallTag, 0);
    }
    //-----------------------------------------------------------
    private void AppendClosingTags(StringBuilder sb) {
      CBORObject curobject = this;
      while (curobject.IsTagged) {
        sb.Append(')');
        curobject = (CBORObject)curobject.itemValue;
      }
    }

    private void WriteTags(Stream s) {
      CBORObject curobject = this;
      while (curobject.IsTagged) {
        int low = curobject.tagLow;
        int high = curobject.tagHigh;
        if (high == 0 && (low >> 16) == 0) {
          WritePositiveInt(6, low, s);
        } else if (high == 0) {
          long value = ((long)low) & 0xFFFFFFFFL;
          WritePositiveInt64(6, value, s);
        } else if ((high >> 16) == 0) {
          long value = ((long)low) & 0xFFFFFFFFL;
          long highValue = ((long)high) & 0xFFFFFFFFL;
          value |= highValue << 32;
          WritePositiveInt64(6, value, s);
        } else {
          byte[] arrayToWrite = new byte[] { (byte)0xDB,
            (byte)((high >> 24) & 0xFF),
            (byte)((high >> 16) & 0xFF),
            (byte)((high >> 8) & 0xFF),
            (byte)(high & 0xFF),
            (byte)((low >> 24) & 0xFF),
            (byte)((low >> 16) & 0xFF),
            (byte)((low >> 8) & 0xFF),
            (byte)(low & 0xFF) };
          s.Write(arrayToWrite, 0, 9);
        }
        curobject = (CBORObject)curobject.itemValue;
      }
    }

    private void AppendOpeningTags(StringBuilder sb) {
      CBORObject curobject = this;
      while (curobject.IsTagged) {
        int low = curobject.tagLow;
        int high = curobject.tagHigh;
        if (high == 0 && (low >> 16) == 0) {
          sb.Append(Convert.ToString((int)low, CultureInfo.InvariantCulture));
        } else {
          BigInteger bi = LowHighToBigInteger(low, high);
          sb.Append(CBORUtilities.BigIntToString(bi));
        }
        sb.Append('(');
        curobject = (CBORObject)curobject.itemValue;
      }
    }

    private static string TrimDotZero(string str) {
      if (str.Length > 2 && str[str.Length - 1] == '0' && str[str.Length - 2] == '.') {
        return str.Substring(0, str.Length - 2);
      }
      return str;
    }

    /// <summary>Returns this CBOR object in string form. The format is intended
    /// to be human-readable, not machine-readable, and the format may change
    /// at any time.</summary>
    /// <returns>A text representation of this object.</returns>
    public override string ToString() {
      StringBuilder sb = null;
      string simvalue = null;
      int type = this.ItemType;
      if (this.IsTagged) {
        if (sb == null) {
          if (type == CBORObjectTypeTextString) {
            // The default capacity of StringBuilder may be too small
            // for many strings, so set a suggested capacity
            // explicitly
            string str = this.AsString();
            sb = new StringBuilder(Math.Min(str.Length, 4096) + 16);
          } else {
            sb = new StringBuilder();
          }
        }
        this.AppendOpeningTags(sb);
      }
      if (type == CBORObjectTypeSimpleValue) {
        if (this.IsTrue) {
          simvalue = "true";
        } else if (this.IsFalse) {
          simvalue = "false";
        } else if (this.IsNull) {
          simvalue = "null";
        } else if (this.IsUndefined) {
          simvalue = "undefined";
        } else {
          if (sb == null) {
            sb = new StringBuilder();
          }
          sb.Append("simple(");
          sb.Append(Convert.ToString((int)this.ThisItem, CultureInfo.InvariantCulture));
          sb.Append(")");
        }
        if (simvalue != null) {
          if (sb == null) {
            return simvalue;
          }
          sb.Append(simvalue);
        }
      } else if (type == CBORObjectTypeSingle) {
        float f = (float)this.ThisItem;
        if (Single.IsNegativeInfinity(f)) {
          simvalue = "-Infinity";
        } else if (Single.IsPositiveInfinity(f)) {
          simvalue = "Infinity";
        } else if (Single.IsNaN(f)) {
          simvalue = "NaN";
        } else {
          simvalue = TrimDotZero(Convert.ToString((float)f, CultureInfo.InvariantCulture));
        }
        if (sb == null) {
          return simvalue;
        }
        sb.Append(simvalue);
      } else if (type == CBORObjectTypeDouble) {
        double f = (double)this.ThisItem;
        if (Double.IsNegativeInfinity(f)) {
          simvalue = "-Infinity";
        } else if (Double.IsPositiveInfinity(f)) {
          simvalue = "Infinity";
        } else if (Double.IsNaN(f)) {
          simvalue = "NaN";
        } else {
          simvalue = TrimDotZero(Convert.ToString((double)f, CultureInfo.InvariantCulture));
        }
        if (sb == null) {
          return simvalue;
        }
        sb.Append(simvalue);
      } else if (type == CBORObjectTypeInteger) {
        long v = (long)this.ThisItem;
        simvalue = Convert.ToString((long)v, CultureInfo.InvariantCulture);
        if (sb == null) {
          return simvalue;
        }
        sb.Append(simvalue);
      } else if (type == CBORObjectTypeBigInteger) {
        simvalue = CBORUtilities.BigIntToString((BigInteger)this.ThisItem);
        if (sb == null) {
          return simvalue;
        }
        sb.Append(simvalue);
      } else if (type == CBORObjectTypeByteString) {
        if (sb == null) {
          sb = new StringBuilder();
        }
        sb.Append("h'");
        byte[] data = (byte[])this.ThisItem;
        CBORUtilities.ToBase16(sb, data);
        sb.Append("'");
      } else if (type == CBORObjectTypeTextString) {
        if (sb == null) {
          return "\"" + this.AsString() + "\"";
        } else {
          sb.Append('\"');
          sb.Append(this.AsString());
          sb.Append('\"');
        }
      } else if (type == CBORObjectTypeExtendedDecimal) {
        return this.ToJSONString();
      } else if (type == CBORObjectTypeExtendedFloat) {
        return this.ToJSONString();
      } else if (type == CBORObjectTypeArray) {
        if (sb == null) {
          sb = new StringBuilder();
        }
        bool first = true;
        sb.Append("[");
        foreach (CBORObject i in this.AsList()) {
          if (!first) {
            sb.Append(", ");
          }
          sb.Append(i.ToString());
          first = false;
        }
        sb.Append("]");
      } else if (type == CBORObjectTypeMap) {
        if (sb == null) {
          sb = new StringBuilder();
        }
        bool first = true;
        sb.Append("{");
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        foreach (KeyValuePair<CBORObject, CBORObject> entry in map) {
          CBORObject key = entry.Key;
          CBORObject value = entry.Value;
          if (!first) {
            sb.Append(", ");
          }
          sb.Append(key.ToString());
          sb.Append(": ");
          sb.Append(value.ToString());
          first = false;
        }
        sb.Append("}");
      }
      if (this.IsTagged) {
        this.AppendClosingTags(sb);
      }
      return sb.ToString();
    }

    private static CBORObject ConvertToBigNum(CBORObject o, bool negative) {
      if (o.ItemType != CBORObjectTypeByteString) {
        throw new CBORException("Byte array expected");
      }
      byte[] data = (byte[])o.ThisItem;
      if (data.Length <= 7) {
        long x = 0;
        for (int i = 0; i < data.Length; ++i) {
          x <<= 8;
          x |= ((long)data[i]) & 0xFF;
        }
        if (negative) {
          x = -x;
        }
        return FromObject(x);
      }
      int neededLength = data.Length;
      byte[] bytes;
      bool extended = false;
      if (((data[0] >> 7) & 1) != 0) {
        // Increase the needed length
        // if the highest bit is set, to
        // distinguish negative and positive
        // values
        ++neededLength;
        extended = true;
      }
      bytes = new byte[neededLength];
      for (int i = 0; i < data.Length; ++i) {
        bytes[i] = data[data.Length - 1 - i];
        if (negative) {
          bytes[i] = (byte)((~((int)bytes[i])) & 0xFF);
        }
      }
      if (extended) {
        if (negative) {
          bytes[bytes.Length - 1] = (byte)0xFF;
        } else {
          bytes[bytes.Length - 1] = 0;
        }
      }
      BigInteger bi = new BigInteger((byte[])bytes);
      return RewrapObject(o, FromObject(bi));
    }

    private static bool BigIntFits(BigInteger bigint) {
      int sign = bigint.Sign;
      if (sign < 0) {
        return bigint.CompareTo(valueLowestMajorType1) >= 0;
      } else if (sign > 0) {
        return bigint.CompareTo(valueUInt64MaxValue) <= 0;
      } else {
        return true;
      }
    }

    private bool CanFitInTypeZeroOrOne() {
      switch (this.ItemType) {
        case CBORObjectTypeInteger:
          return true;
        case CBORObjectTypeBigInteger: {
            BigInteger bigint = (BigInteger)this.ThisItem;
            return bigint.CompareTo(valueLowestMajorType1) >= 0 &&
              bigint.CompareTo(valueUInt64MaxValue) <= 0;
          }
        case CBORObjectTypeSingle: {
            float value = (float)this.ThisItem;
            return value >= -18446744073709551616.0f &&
              value <= 18446742974197923840.0f;  // Highest float less or eq. to UInt64.MaxValue
          }
        case CBORObjectTypeDouble: {
            double value = (double)this.ThisItem;
            return value >= -18446744073709551616.0 &&
              value <= 18446744073709549568.0;  // Highest double less or eq. to UInt64.MaxValue
          }
        case CBORObjectTypeExtendedDecimal: {
            ExtendedDecimal value = (ExtendedDecimal)this.ThisItem;
            return value.CompareTo(ExtendedDecimal.FromBigInteger(valueLowestMajorType1)) >= 0 &&
              value.CompareTo(ExtendedDecimal.FromBigInteger(valueUInt64MaxValue)) <= 0;
          }
        case CBORObjectTypeExtendedFloat: {
            ExtendedFloat value = (ExtendedFloat)this.ThisItem;
            return value.CompareTo(ExtendedFloat.FromBigInteger(valueLowestMajorType1)) >= 0 &&
              value.CompareTo(ExtendedFloat.FromBigInteger(valueUInt64MaxValue)) <= 0;
          }
        default:
          return false;
      }
    }
    // Wrap a new object in another one to retain its tags
    private static CBORObject RewrapObject(CBORObject original, CBORObject newObject) {
      if (!original.IsTagged) {
        return newObject;
      }
      BigInteger[] tags = original.GetTags();
      for (int i = tags.Length - 1; i >= 0; ++i) {
        newObject = FromObjectAndTag(newObject, tags[i]);
      }
      return newObject;
    }

    private static CBORObject ConvertToDecimalFrac(CBORObject o, Boolean isDecimal) {
      if (o.ItemType != CBORObjectTypeArray) {
        throw new CBORException("Big fraction must be an array");
      }
      if (o.Count != 2) {
        throw new CBORException("Big fraction requires exactly 2 items");
      }
      IList<CBORObject> list = o.AsList();
      if (!list[0].CanFitInTypeZeroOrOne()) {
        throw new CBORException("Exponent is too big");
      }
      // check type of mantissa
      if (list[1].ItemType != CBORObjectTypeInteger &&
          list[1].ItemType != CBORObjectTypeBigInteger) {
        throw new CBORException("Big fraction requires mantissa to be an integer or big integer");
      }
      if (list[0].IsZero) {
        // Exponent is 0, so return mantissa instead
        return RewrapObject(o, list[1]);
      }
      if (isDecimal) {
        CBORObject objectToWrap = new CBORObject(
          CBORObjectTypeExtendedDecimal,
          ExtendedDecimal.Create(list[1].AsBigInteger(), list[0].AsBigInteger()));
        return RewrapObject(
          o,
          objectToWrap);
      } else {
        CBORObject objectToWrap = new CBORObject(
          CBORObjectTypeExtendedFloat,
          ExtendedFloat.Create(list[1].AsBigInteger(), list[0].AsBigInteger()));
        return RewrapObject(
          o,
          objectToWrap);
      }
    }

    private static bool CheckMajorTypeIndex(int type, int index, int[] validTypeFlags) {
      if (validTypeFlags == null || index < 0 || index >= validTypeFlags.Length) {
        return false;
      }
      if (type < 0 || type > 7) {
        return false;
      }
      return (validTypeFlags[index] & (1 << type)) != 0;
    }

    private static CBORObject Read(
      Stream s,
      int depth,
      bool allowBreak,
      int allowOnlyType,
      int[] validTypeFlags,
      int validTypeIndex) {
      if (depth > 1000) {
        throw new CBORException("Too deeply nested");
      }
      int firstbyte = s.ReadByte();
      if (firstbyte < 0) {
        throw new CBORException("Premature end of data");
      }
      if (firstbyte == 0xFF) {
        if (allowBreak) {
          return null;
        }
        throw new CBORException("Unexpected break code encountered");
      }
      int type = (firstbyte >> 5) & 0x07;
      int additional = firstbyte & 0x1F;
      int expectedLength = valueExpectedLengths[firstbyte];
      // Data checks
      if (expectedLength == -1) {
        // if the head byte is invalid
        throw new CBORException("Unexpected data encountered");
      }
      if (allowOnlyType >= 0) {
        if (allowOnlyType != type) {
          throw new CBORException("Expected major type " +
                                  Convert.ToString((int)allowOnlyType, CultureInfo.InvariantCulture) +
                                  ", instead got type " +
                                  Convert.ToString((int)type, CultureInfo.InvariantCulture));
        }
        if (additional == 31) {
          throw new CBORException("Indefinite-length data not allowed here");
        }
        if (additional >= 28) {
          throw new CBORException("Unexpected data encountered");
        }
      }
      if (validTypeFlags != null) {
        // Check for valid major types if asked
        if (!CheckMajorTypeIndex(type, validTypeIndex, validTypeFlags)) {
          throw new CBORException("Unexpected data type encountered");
        }
      }
      // Check if this represents a fixed object
      CBORObject fixedObject = valueFixedObjects[firstbyte];
      if (fixedObject != null) {
        return fixedObject;
      }
      // Read fixed-length data
      byte[] data = null;
      if (expectedLength != 0) {
        data = new byte[expectedLength];
        // include the first byte because GetFixedLengthObject
        // will assume it exists for some head bytes
        data[0] = unchecked((byte)firstbyte);
        if (expectedLength > 1 &&
            s.Read(data, 1, expectedLength - 1) != expectedLength - 1) {
          throw new CBORException("Premature end of data");
        }
        return GetFixedLengthObject(firstbyte, data);
      }
      long uadditional = (long)additional;
      BigInteger bigintAdditional = BigInteger.Zero;
      bool hasBigAdditional = false;
      data = new byte[8];
      int lowAdditional = 0;
      switch (firstbyte & 0x1F) {
        case 24: {
            int tmp = s.ReadByte();
            if (tmp < 0) {
              throw new CBORException("Premature end of data");
            }
            lowAdditional = tmp;
            uadditional = lowAdditional;
            break;
          }
        case 25: {
            if (s.Read(data, 0, 2) != 2) {
              throw new CBORException("Premature end of data");
            }
            lowAdditional = ((int)(data[0] & (int)0xFF)) << 8;
            lowAdditional |= (int)(data[1] & (int)0xFF);
            uadditional = lowAdditional;
            break;
          }
        case 26: {
            if (s.Read(data, 0, 4) != 4) {
              throw new CBORException("Premature end of data");
            }
            uadditional = ((long)(data[0] & (long)0xFF)) << 24;
            uadditional |= ((long)(data[1] & (long)0xFF)) << 16;
            uadditional |= ((long)(data[2] & (long)0xFF)) << 8;
            uadditional |= (long)(data[3] & (long)0xFF);
            break;
          }
        case 27: {
            if (s.Read(data, 0, 8) != 8) {
              throw new CBORException("Premature end of data");
            }
            if ((((int)data[0]) & 0x80) != 0) {
              // Won't fit in a signed 64-bit number
              byte[] uabytes = new byte[9];
              uabytes[0] = data[7];
              uabytes[1] = data[6];
              uabytes[2] = data[5];
              uabytes[3] = data[4];
              uabytes[4] = data[3];
              uabytes[5] = data[2];
              uabytes[6] = data[1];
              uabytes[7] = data[0];
              uabytes[8] = 0;
              hasBigAdditional = true;
              bigintAdditional = new BigInteger((byte[])uabytes);
            } else {
              uadditional = ((long)(data[0] & (long)0xFF)) << 56;
              uadditional |= ((long)(data[1] & (long)0xFF)) << 48;
              uadditional |= ((long)(data[2] & (long)0xFF)) << 40;
              uadditional |= ((long)(data[3] & (long)0xFF)) << 32;
              uadditional |= ((long)(data[4] & (long)0xFF)) << 24;
              uadditional |= ((long)(data[5] & (long)0xFF)) << 16;
              uadditional |= ((long)(data[6] & (long)0xFF)) << 8;
              uadditional |= (long)(data[7] & (long)0xFF);
            }
            break;
          }
        default:
          break;
      }
      // The following doesn't check for major types 0 and 1,
      // since all of them are fixed-length types and are
      // handled in the call to GetFixedLengthObject.
      if (type == 2) {  // Byte string
        if (additional == 31) {
          // Streaming byte string
          using (MemoryStream ms = new MemoryStream()) {
            // Requires same type as this one
            int[] subFlags = new int[] { (1 << type) };
            while (true) {
              CBORObject o = Read(s, depth + 1, true, type, subFlags, 0);
              // break if the "break" code was read
              if (o == null) {
                break;
              }
              data = (byte[])o.ThisItem;
              ms.Write(data, 0, data.Length);
            }
            if (ms.Position > Int32.MaxValue) {
              throw new CBORException("Length of bytes to be streamed is bigger than supported");
            }
            data = ms.ToArray();
            return new CBORObject(
              CBORObjectTypeByteString,
              data);
          }
        } else {
          if (hasBigAdditional) {
            throw new CBORException("Length of " +
                                    CBORUtilities.BigIntToString(bigintAdditional) +
                                    " is bigger than supported");
          } else if (uadditional > Int32.MaxValue) {
            throw new CBORException("Length of " +
                                    Convert.ToString((long)uadditional, CultureInfo.InvariantCulture) +
                                    " is bigger than supported");
          }
          data = null;
          if (uadditional <= 0x10000) {
            // Simple case: small size
            data = new byte[(int)uadditional];
            if (s.Read(data, 0, data.Length) != data.Length) {
              throw new CBORException("Premature end of stream");
            }
          } else {
            byte[] tmpdata = new byte[0x10000];
            int total = (int)uadditional;
            using (var ms = new MemoryStream()) {
              while (total > 0) {
                int bufsize = Math.Min(tmpdata.Length, total);
                if (s.Read(tmpdata, 0, bufsize) != bufsize) {
                  throw new CBORException("Premature end of stream");
                }
                ms.Write(tmpdata, 0, bufsize);
                total -= bufsize;
              }
              data = ms.ToArray();
            }
          }
          return new CBORObject(
            CBORObjectTypeByteString,
            data);
        }
      } else if (type == 3) {  // Text string
        if (additional == 31) {
          // Streaming text string
          StringBuilder builder = new StringBuilder();
          // Requires same type as this one
          int[] subFlags = new int[] { (1 << type) };
          while (true) {
            CBORObject o = Read(s, depth + 1, true, type, subFlags, 0);
            if (o == null) {
              // break if the "break" code was read
              break;
            }
            builder.Append((string)o.ThisItem);
          }
          return new CBORObject(
            CBORObjectTypeTextString,
            builder.ToString());
        } else {
          if (hasBigAdditional) {
            throw new CBORException("Length of " +
                                    CBORUtilities.BigIntToString(bigintAdditional) +
                                    " is bigger than supported");
          } else if (uadditional > Int32.MaxValue) {
            throw new CBORException("Length of " +
                                    Convert.ToString((long)uadditional, CultureInfo.InvariantCulture) +
                                    " is bigger than supported");
          }
          StringBuilder builder = new StringBuilder();
          switch (DataUtilities.ReadUtf8(s, (int)uadditional, builder, false)) {
            case -1:
              throw new CBORException("Invalid UTF-8");
            case -2:
              throw new CBORException("Premature end of data");
            default:
              break;  // No error
          }
          return new CBORObject(CBORObjectTypeTextString, builder.ToString());
        }
      } else if (type == 4) {  // Array
        IList<CBORObject> list = new List<CBORObject>();
        int vtindex = 1;
        if (additional == 31) {
          while (true) {
            CBORObject o = Read(s, depth + 1, true, -1, validTypeFlags, vtindex);
            // break if the "break" code was read
            if (o == null) {
              break;
            }
            list.Add(o);
            ++vtindex;
          }
          return new CBORObject(CBORObjectTypeArray, list);
        } else {
          if (hasBigAdditional) {
            throw new CBORException("Length of " +
                                    CBORUtilities.BigIntToString(bigintAdditional) +
                                    " is bigger than supported");
          } else if (uadditional > Int32.MaxValue) {
            throw new CBORException("Length of " +
                                    Convert.ToString((long)uadditional, CultureInfo.InvariantCulture) +
                                    " is bigger than supported");
          }
          for (long i = 0; i < uadditional; ++i) {
            list.Add(Read(s, depth + 1, false, -1, validTypeFlags, vtindex));
            ++vtindex;
          }
          return new CBORObject(CBORObjectTypeArray, list);
        }
      } else if (type == 5) {  // Map, type 5
        var dict = new Dictionary<CBORObject, CBORObject>();
        if (additional == 31) {
          while (true) {
            CBORObject key = Read(s, depth + 1, true, -1, null, 0);
            if (key == null) {
              // break if the "break" code was read
              break;
            }
            CBORObject value = Read(s, depth + 1, false, -1, null, 0);
            dict[key] = value;
          }
          return new CBORObject(CBORObjectTypeMap, dict);
        } else {
          if (hasBigAdditional) {
            throw new CBORException("Length of " +
                                    CBORUtilities.BigIntToString(bigintAdditional) +
                                    " is bigger than supported");
          } else if (uadditional > Int32.MaxValue) {
            throw new CBORException("Length of " +
                                    Convert.ToString((long)uadditional, CultureInfo.InvariantCulture) +
                                    " is bigger than supported");
          }
          for (long i = 0; i < uadditional; ++i) {
            CBORObject key = Read(s, depth + 1, false, -1, null, 0);
            CBORObject value = Read(s, depth + 1, false, -1, null, 0);
            dict[key] = value;
          }
          return new CBORObject(CBORObjectTypeMap, dict);
        }
      } else if (type == 6) {  // Tagged item
        CBORObject o;
        if (!hasBigAdditional) {
          if (uadditional == 0) {
            // Requires a text string
            int[] subFlags = new int[] { (1 << 3) };
            o = Read(s, depth + 1, false, -1, subFlags, 0);
          } else if (uadditional == 2 || uadditional == 3) {
            // Big number
            // Requires a byte string
            int[] subFlags = new int[] { (1 << 2) };
            o = Read(s, depth + 1, false, -1, subFlags, 0);
            return ConvertToBigNum(o, uadditional == 3);
          } else if (uadditional == 4 || uadditional == 5) {
            // Requires an array with two elements of
            // a valid type
            int[] subFlags = new int[] {
              (1 << 4),  // array
              (1 << 0) | (1 << 1),  // exponent
              (1 << 0) | (1 << 1) | (1 << 6)  // mantissa
            };
            o = Read(s, depth + 1, false, -1, subFlags, 0);
            return ConvertToDecimalFrac(o, uadditional == 4);
          } else {
            o = Read(s, depth + 1, false, -1, null, 0);
          }
        } else {
          o = Read(s, depth + 1, false, -1, null, 0);
        }
        if (hasBigAdditional) {
          return FromObjectAndTag(o, bigintAdditional);
        } else if (uadditional < 65536) {
          return FromObjectAndTag(
            o,
            (int)uadditional);
        } else {
          return FromObjectAndTag(
            o,
            (BigInteger)uadditional);
        }
      } else {
        throw new CBORException("Unexpected data encountered");
      }
    }
  }
}
