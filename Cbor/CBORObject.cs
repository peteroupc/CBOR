/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using PeterO;

namespace PeterO.Cbor {
    /// <summary>Represents an object in Concise Binary Object Representation
    /// (CBOR) and contains methods for reading and writing CBOR data. CBOR
    /// is defined in RFC 7049. <para>There are many ways to get a CBOR object,
    /// including from bytes, objects, streams and JSON, as described below.</para>
    /// <para> <b>To and from byte arrays:</b>
    /// The CBORObject.DecodeToBytes method converts a byte array in CBOR
    /// format to a CBOR object. The EncodeToBytes method converts a CBOR
    /// object to its corresponding byte array in CBOR format. </para>
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
    /// <para> One kind of CBOR object is called a map, or a list of key-value
    /// pairs. Keys can be any kind of CBOR object, including numbers, strings,
    /// arrays, and maps. However, since byte strings, arrays, and maps are
    /// mutable, it is not advisable to use these three kinds of object as keys;
    /// they are much better used as map values instead, keeping in mind that
    /// they are not thread safe without synchronizing reads and writes to
    /// them. </para>
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
    internal const int CBORObjectTypeExtendedRational = 12;
    internal static readonly BigInteger Int64MaxValue = (BigInteger)Int64.MaxValue;
    internal static readonly BigInteger Int64MinValue = (BigInteger)Int64.MinValue;
    internal static readonly BigInteger LowestMajorType1 = BigInteger.Zero - (BigInteger.One << 64);

    internal static readonly BigInteger UInt64MaxValue = (BigInteger.One << 64) - BigInteger.One;

    private sealed class ConverterInfo {
      private object toObject;

    /// <summary>Gets or sets the converter's ToCBORObject method.</summary>
    /// <value>The converter&apos;s ToCBORObject method.</value>
      public object ToObject {
        get {
          return this.toObject;
        }

        set {
          this.toObject = value;
        }
      }

      private object converter;

    /// <summary>Gets or sets the ICBORConverter object.</summary>
    /// <value>The ICBORConverter object.</value>
      public object Converter {
        get {
          return this.converter;
        }

        set {
          this.converter = value;
        }
      }
    }

    private static IDictionary<Object, ConverterInfo> converters = new Dictionary<Object, ConverterInfo>();
    private static IDictionary<BigInteger, ICBORTag> tagHandlers = new Dictionary<BigInteger, ICBORTag>();

    private static int[] valueNumberTypeOrder = new int[] { 0, 0, 2, 3, 4, 5, 1, 0, 0, 0, 0, 0, 0 };

    internal static readonly ICBORNumber[] NumberInterfaces = new ICBORNumber[] {
      new CBORInteger(),
      new CBORBigInteger(),
      null,
      null,
      null,
      null,
      null,
      new CBORSingle(),
      new CBORDouble(),
      new CBORExtendedDecimal(),
      null,
      new CBORExtendedFloat(),
      new CBORExtendedRational()
    };

    /// <summary>Represents the value false.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This CBORObject is immutable")]
    #endif

    public static readonly CBORObject False = new CBORObject(CBORObjectTypeSimpleValue, 20);

    /// <summary>Represents the value true.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This CBORObject is immutable")]
    #endif

    public static readonly CBORObject True = new CBORObject(CBORObjectTypeSimpleValue, 21);

    /// <summary>Represents the value null.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This CBORObject is immutable")]
    #endif

    public static readonly CBORObject Null = new CBORObject(CBORObjectTypeSimpleValue, 22);

    /// <summary>Represents the value undefined.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This CBORObject is immutable")]
    #endif

    public static readonly CBORObject Undefined = new CBORObject(CBORObjectTypeSimpleValue, 23);

    private CBORObject() {
    }

    internal CBORObject(CBORObject obj, int tagLow, int tagHigh) :
      this(CBORObjectTypeTagged, obj) {
      this.tagLow = tagLow;
      this.tagHigh = tagHigh;
    }

    /// <summary>Registers an object that converts objects of a given type
    /// to CBOR objects (called a CBOR converter).</summary>
    /// <param name='type'>A Type object specifying the type that the converter
    /// converts to CBOR objects.</param>
    /// <param name='converter'>An ICBORConverter object.</param>
    /// <typeparam name='T'>Must be the same as the "type" parameter.</typeparam>
    public static void AddConverter<T>(Type type, ICBORConverter<T> converter) {
      if (type == null) {
        throw new ArgumentNullException("type");
      }
      if (converter == null) {
        throw new ArgumentNullException("converter");
      }
      ConverterInfo ci = new CBORObject.ConverterInfo();
      ci.Converter = converter;
      ci.ToObject = PropertyMap.FindOneArgumentMethod(converter, "ToCBORObject", type);
      if (ci.ToObject == null) {
        throw new ArgumentException("Converter doesn't contain a proper ToCBORObject method");
      }
      lock (converters) {
        converters[type] = ci;
      }
    }

    private static bool TagHandlersEmpty() {
      lock (tagHandlers) {
        return tagHandlers.Count == 0;
      }
    }

    internal static ICBORNumber GetNumberInterface(int type) {
      return NumberInterfaces[type];
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bigintTag'>A BigInteger object.</param>
    /// <param name='handler'>An ICBORTag object.</param>
    public static void AddTagHandler(BigInteger bigintTag, ICBORTag handler) {
      if (bigintTag == null) {
        throw new ArgumentNullException("bigintTag");
      }
      if (handler == null) {
        throw new ArgumentNullException("handler");
      }
      if (bigintTag.Sign < 0) {
        throw new ArgumentException("bigintTag.Sign (" + Convert.ToString((long)bigintTag.Sign, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (bigintTag.bitLength() > 64) {
        throw new ArgumentException("bigintTag.bitLength (" + Convert.ToString((long)bigintTag.bitLength(), System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + "64");
      }
      lock (tagHandlers) {
        tagHandlers[bigintTag] = handler;
      }
    }

    private static CBORObject ConvertWithConverter(object obj) {
      #if DEBUG
      if (obj == null) {
        throw new ArgumentNullException("obj");
      }
      #endif
      Object type = obj.GetType();
      ConverterInfo convinfo = null;
      lock (converters) {
        if (converters.Count == 0) {
          CBORTag0.AddConverter();
          CBORTag37.AddConverter();
          CBORTag32.AddConverter();
        }
        if (converters.ContainsKey(type)) {
          convinfo = converters[type];
        } else {
          return null;
        }
      }
      if (convinfo == null) {
        return null;
      }
      return (CBORObject)PropertyMap.InvokeOneArgumentMethod(
        convinfo.ToObject,
        convinfo.Converter,
        obj);
    }

    internal CBORObject(int type, object item) {
      #if DEBUG
      // Check range in debug mode to ensure that Integer and BigInteger
      // are unambiguous
      if ((type == CBORObjectTypeBigInteger) &&
          ((BigInteger)item).CompareTo(Int64MinValue) >= 0 &&
          ((BigInteger)item).CompareTo(Int64MaxValue) <= 0) {
        // if (!false) {
        throw new ArgumentException("Big integer is within range for Integer");
        // }
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
          case CBORObjectTypeExtendedRational:
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
    /// <value>True if this value is a CBOR true value; otherwise, false.</value>
    public bool IsTrue {
      get {
        return this.ItemType == CBORObjectTypeSimpleValue && (int)this.ThisItem == 21;
      }
    }

    /// <summary>Gets a value indicating whether this value is a CBOR false
    /// value.</summary>
    /// <value>True if this value is a CBOR false value; otherwise, false.</value>
    public bool IsFalse {
      get {
        return this.ItemType == CBORObjectTypeSimpleValue && (int)this.ThisItem == 20;
      }
    }

    /// <summary>Gets a value indicating whether this value is a CBOR null
    /// value.</summary>
    /// <value>True if this value is a CBOR null value; otherwise, false.</value>
    public bool IsNull {
      get {
        return this.ItemType == CBORObjectTypeSimpleValue && (int)this.ThisItem == 22;
      }
    }

    /// <summary>Gets a value indicating whether this value is a CBOR undefined
    /// value.</summary>
    /// <value>True if this value is a CBOR undefined value; otherwise, false.</value>
    public bool IsUndefined {
      get {
        return this.ItemType == CBORObjectTypeSimpleValue && (int)this.ThisItem == 23;
      }
    }

    /// <summary>Gets a value indicating whether this object&apos;s value
    /// equals 0.</summary>
    /// <value>True if this object&apos;s value equals 0; otherwise, false.</value>
    public bool IsZero {
      get {
        ICBORNumber cn = NumberInterfaces[this.ItemType];
        return cn == null ? false : cn.IsZero(this.ThisItem);
      }
    }

    /// <summary>Gets this object&apos;s value with the sign reversed.</summary>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    /// <returns>The reversed-sign form of this number.</returns>
    public CBORObject Negate() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        throw new InvalidOperationException("This object is not a number.");
      }
      return CBORObject.FromObject(cn.Negate(this.ThisItem));
    }

    /// <summary>Gets this object's absolute value.</summary>
    /// <returns>This object's absolute without its negative sign.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    public CBORObject Abs() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        throw new InvalidOperationException("This object is not a number.");
      }
      object oldItem = this.ThisItem;
      object newItem = cn.Abs(oldItem);
      return (oldItem == newItem) ? this : CBORObject.FromObject(newItem);
    }

    private static int GetSignInternal(int type, object obj) {
      ICBORNumber cn = NumberInterfaces[type];
      return cn == null ? 2 : cn.Sign(obj);
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
        int ret = GetSignInternal(this.ItemType, this.ThisItem);
        if (ret == 2) {
          throw new InvalidOperationException("This object is not a number.");
        }
        return ret;
      }
    }

    /// <summary>The value positive infinity.</summary>
    public static readonly CBORObject PositiveInfinity = CBORObject.FromObject(Double.PositiveInfinity);

    /// <summary>The value negative infinity.</summary>
    public static readonly CBORObject NegativeInfinity = CBORObject.FromObject(Double.NegativeInfinity);

    /// <summary>A not-a-number value.</summary>
    public static readonly CBORObject NaN = CBORObject.FromObject(Double.NaN);

    /// <summary>Gets a value indicating whether this CBOR object represents
    /// positive infinity.</summary>
    /// <returns>True if this CBOR object represents positive infinity;
    /// otherwise, false.</returns>
    public bool IsPositiveInfinity() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      return cn == null ? false : cn.IsPositiveInfinity(this.ThisItem);
    }

    /// <summary>Gets a value indicating whether this CBOR object represents
    /// infinity.</summary>
    /// <returns>True if this CBOR object represents infinity; otherwise,
    /// false.</returns>
    public bool IsInfinity() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      return cn == null ? false : cn.IsInfinity(this.ThisItem);
    }

    /// <summary>Gets a value indicating whether this CBOR object represents
    /// a finite number.</summary>
    /// <value>True if this CBOR object represents a finite number; otherwise,
    /// false.</value>
    public bool IsFinite {
      get {
        return this.Type == CBORType.Number && !this.IsInfinity() && !this.IsNaN();
      }
    }

    /// <summary>Gets a value indicating whether this CBOR object represents
    /// negative infinity.</summary>
    /// <returns>True if this CBOR object represents negative infinity;
    /// otherwise, false.</returns>
    public bool IsNegativeInfinity() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      return cn == null ? false : cn.IsNegativeInfinity(this.ThisItem);
    }

    /// <summary>Gets a value indicating whether this CBOR object represents
    /// a not-a-number value (as opposed to whether this object&apos;s type
    /// is not a number type).</summary>
    /// <returns>True if this CBOR object represents a not-a-number value
    /// (as opposed to whether this object's type is not a number type); otherwise,
    /// false.</returns>
    public bool IsNaN() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      return cn == null ? false : cn.IsNaN(this.ThisItem);
    }

    /// <summary>Compares two CBOR objects.<para> In this implementation:</para>
    /// <list type=''> <item>The null pointer (null reference) is considered
    /// less than any other object.</item>
    /// <item> If either object is true, false, CBORObject.Null, or the undefined
    /// value, it is treated as less than the other value. If both objects have
    /// one of these four values, then undefined is less than CBORObject.Null,
    /// which is less than false, which is less than true.</item>
    /// <item> If both objects are numbers, their mathematical values are
    /// compared. Here, NaN (not-a-number) is considered greater than any
    /// number.</item>
    /// <item> If both objects are simple values other than true, false, CBORObject.Null,
    /// and the undefined value, the objects are compared according to their
    /// ordinal numbers.</item>
    /// <item> If both objects are arrays, each element is compared. If one
    /// array is shorter than the other and the other array begins with that
    /// array (for the purposes of comparison), the shorter array is considered
    /// less than the longer array.</item>
    /// <item> If both objects are strings, compares each string code-point
    /// by code-point, as though by the DataUtilities.CodePointCompare
    /// method.</item>
    /// <item> If both objects are maps, compares each map as though each were
    /// an array with the sorted keys of that map as the array's elements. If
    /// both maps have the same keys, their values are compared in the order
    /// of the sorted keys.</item>
    /// <item> If each object is a different type, then they are sorted by their
    /// type number, in the order given for the CBORType enumeration.</item>
    /// <item> If each object has different tags and both objects are otherwise
    /// equal under this method, each element is compared as though each were
    /// an array with that object's tags listed in order from outermost to
    /// innermost. </item>
    /// </list>
    /// <para> This method is not consistent with the Equals method.</para>
    /// </summary>
    /// <param name='other'>A value to compare with.</param>
    /// <returns>Less than 0, if this value is less than the other object;
    /// or 0, if both values are equal; or greater than 0, if this value is less
    /// than the other object or if the other object is null.</returns>
    public int CompareTo(CBORObject other) {
      if (other == null) {
        return 1;
      }
      if (this == other) {
        return 0;
      }
      int typeA = this.ItemType;
      int typeB = other.ItemType;
      object objA = this.ThisItem;
      object objB = other.ThisItem;
      int simpleValueA = -1;
      int simpleValueB = -1;
      if (typeA == CBORObjectTypeSimpleValue) {
        if ((int)objA == 20) {  // false
          simpleValueA = 2;
        } else if ((int)objA == 21) {  // true
          simpleValueA = 3;
        } else if ((int)objA == 22) {  // null
          simpleValueA = 1;
        } else if ((int)objA == 23) {  // undefined
          simpleValueA = 0;
        }
      }
      if (typeB == CBORObjectTypeSimpleValue) {
        if ((int)objB == 20) {  // false
          simpleValueB = 2;
        } else if ((int)objB == 21) {  // true
          simpleValueB = 3;
        } else if ((int)objB == 22) {  // null
          simpleValueB = 1;
        } else if ((int)objB == 23) {  // undefined
          simpleValueB = 0;
        }
      }
      int cmp = 0;
      if (simpleValueA >= 0 || simpleValueB >= 0) {
        if (simpleValueB < 0) {
          return -1;  // B is not true, false, null, or undefined, so A is less
        } else if (simpleValueA < 0) {
          return 1;
        }
        if (simpleValueA == simpleValueB) {
          cmp = 0;
        } else {
          cmp = (simpleValueA < simpleValueB) ? -1 : 1;
        }
      } else if (typeA == typeB) {
        switch (typeA) {
            case CBORObjectTypeInteger: {
              long a = (long)objA;
              long b = (long)objB;
              if (a == b) {
                cmp = 0;
              } else {
                cmp = (a < b) ? -1 : 1;
              }
              break;
            }
            case CBORObjectTypeSingle: {
              float a = (float)objA;
              float b = (float)objB;
              // Treat NaN as greater than all other numbers
              if (Single.IsNaN(a)) {
                cmp = Single.IsNaN(b) ? 0 : 1;
              } else if (Single.IsNaN(b)) {
                cmp = -1;
              } else if (a == b) {
                cmp = 0;
              } else {
                cmp = (a < b) ? -1 : 1;
              }
              break;
            }
            case CBORObjectTypeBigInteger: {
              BigInteger bigintA = (BigInteger)objA;
              BigInteger bigintB = (BigInteger)objB;
              cmp = bigintA.CompareTo(bigintB);
              break;
            }
            case CBORObjectTypeDouble: {
              double a = (double)objA;
              double b = (double)objB;
              // Treat NaN as greater than all other numbers
              if (Double.IsNaN(a)) {
                cmp = Double.IsNaN(b) ? 0 : 1;
              } else if (Double.IsNaN(b)) {
                cmp = -1;
              } else if (a == b) {
                cmp = 0;
              } else {
                cmp = (a < b) ? -1 : 1;
              }
              break;
            }
            case CBORObjectTypeExtendedDecimal: {
              cmp = ((ExtendedDecimal)objA).CompareTo(
                (ExtendedDecimal)objB);
              break;
            }
            case CBORObjectTypeExtendedFloat: {
              cmp = ((ExtendedFloat)objA).CompareTo(
                (ExtendedFloat)objB);
              break;
            }
            case CBORObjectTypeExtendedRational: {
              cmp = ((ExtendedRational)objA).CompareTo(
                (ExtendedRational)objB);
              break;
            }
            case CBORObjectTypeByteString: {
              cmp = CBORUtilities.ByteArrayCompare((byte[])objA, (byte[])objB);
              break;
            }
            case CBORObjectTypeTextString: {
              cmp = DataUtilities.CodePointCompare(
                (string)objA,
                (string)objB);
              break;
            }
            case CBORObjectTypeArray: {
              cmp = ListCompare(
                (List<CBORObject>)objA,
                (List<CBORObject>)objB);
              break;
            }
            case CBORObjectTypeMap: {
              cmp = MapCompare(
                (IDictionary<CBORObject, CBORObject>)objA,
                (IDictionary<CBORObject, CBORObject>)objB);
              break;
            }
            case CBORObjectTypeSimpleValue: {
              int valueA = (int)objA;
              int valueB = (int)objB;
              if (valueA == valueB) {
                cmp = 0;
              } else {
                cmp = (valueA < valueB) ? -1 : 1;
              }
              break;
            }
          default:
            throw new ArgumentException("Unexpected data type");
        }
      } else {
        int typeOrderA = valueNumberTypeOrder[typeA];
        int typeOrderB = valueNumberTypeOrder[typeB];
        // Check whether general types are different
        // (treating number types the same)
        if (typeOrderA != typeOrderB) {
          return (typeOrderA < typeOrderB) ? -1 : 1;
        }
        // At this point, both types should be number types.
        #if DEBUG
        if (!(typeOrderA == 0)) {
          throw new ArgumentException("doesn't satisfy typeOrderA == 0");
        }
        if (!(typeOrderB == 0)) {
          throw new ArgumentException("doesn't satisfy typeOrderB == 0");
        }
        #endif
        int s1 = GetSignInternal(typeA, objA);
        int s2 = GetSignInternal(typeB, objB);
        if (s1 != s2 && s1 != 2 && s2 != 2) {
          // if both types are numbers
          // and their signs are different
          return (s1 < s2) ? -1 : 1;
        } else if (s1 == 2 && s2 == 2) {
          // both are NaN
          cmp = 0;
        } else if (s1 == 2) {
          // first object is NaN
          return 1;
        } else if (s2 == 2) {
          // second object is NaN
          return -1;
        } else {
          // DebugUtility.Log("a=" + this + " b=" + (other));
          if (typeA == CBORObjectTypeExtendedRational) {
            ExtendedRational e1 = NumberInterfaces[typeA].AsExtendedRational(objA);
            if (typeB == CBORObjectTypeExtendedDecimal) {
              ExtendedDecimal e2 = NumberInterfaces[typeB].AsExtendedDecimal(objB);
              cmp = e1.CompareToDecimal(e2);
            } else {
              ExtendedFloat e2 = NumberInterfaces[typeB].AsExtendedFloat(objB);
              cmp = e1.CompareToBinary(e2);
            }
          } else if (typeB == CBORObjectTypeExtendedRational) {
            ExtendedRational e2 = NumberInterfaces[typeB].AsExtendedRational(objB);
            if (typeA == CBORObjectTypeExtendedDecimal) {
              ExtendedDecimal e1 = NumberInterfaces[typeA].AsExtendedDecimal(objA);
              cmp = e2.CompareToDecimal(e1);
              cmp = -cmp;
            } else {
              ExtendedFloat e1 = NumberInterfaces[typeA].AsExtendedFloat(objA);
              cmp = e2.CompareToBinary(e1);
              cmp = -cmp;
            }
          } else if (typeA == CBORObjectTypeExtendedDecimal ||
                     typeB == CBORObjectTypeExtendedDecimal) {
            ExtendedDecimal e1 = null;
            ExtendedDecimal e2 = null;
            if (typeA == CBORObjectTypeExtendedFloat) {
              ExtendedFloat ef1 = (ExtendedFloat)objA;
              e2 = (ExtendedDecimal)objB;
              cmp = e2.CompareToBinary(ef1);
              cmp = -cmp;
            } else if (typeB == CBORObjectTypeExtendedFloat) {
              ExtendedFloat ef1 = (ExtendedFloat)objB;
              e2 = (ExtendedDecimal)objA;
              cmp = e2.CompareToBinary(ef1);
            } else {
              e1 = NumberInterfaces[typeA].AsExtendedDecimal(objA);
              e2 = NumberInterfaces[typeB].AsExtendedDecimal(objB);
              cmp = e1.CompareTo(e2);
            }
          } else if (typeA == CBORObjectTypeExtendedFloat || typeB == CBORObjectTypeExtendedFloat ||
                     typeA == CBORObjectTypeDouble || typeB == CBORObjectTypeDouble ||
                     typeA == CBORObjectTypeSingle || typeB == CBORObjectTypeSingle) {
            ExtendedFloat e1 = NumberInterfaces[typeA].AsExtendedFloat(objA);
            ExtendedFloat e2 = NumberInterfaces[typeB].AsExtendedFloat(objB);
            cmp = e1.CompareTo(e2);
          } else {
            BigInteger b1 = NumberInterfaces[typeA].AsBigInteger(objA);
            BigInteger b2 = NumberInterfaces[typeB].AsBigInteger(objB);
            cmp = b1.CompareTo(b2);
          }
        }
      }
      if (cmp == 0) {
        if (!this.IsTagged && !other.IsTagged) {
          return 0;
        }
        return TagsCompare(this.GetTags(), other.GetTags());
      }
      return cmp;
    }

    #region Equals and GetHashCode implementation
    private static int TagsCompare(BigInteger[] tagsA, BigInteger[] tagsB) {
      if (tagsA == null) {
        return (tagsB == null) ? 0 : -1;
      }
      if (tagsB == null) {
        return 1;
      }
      int listACount = tagsA.Length;
      int listBCount = tagsB.Length;
      int c = Math.Min(listACount, listBCount);
      for (int i = 0; i < c; ++i) {
        int cmp = tagsA[i].CompareTo(tagsB[i]);
        if (cmp != 0) {
          return cmp;
        }
      }
      if (listACount != listBCount) {
        return (listACount < listBCount) ? -1 : 1;
      }
      return 0;
    }

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

    /// <summary>Gets an object with the same value as this one but without
    /// the tags it has, if any. If this object is an array, map, or byte string,
    /// the data will not be copied to the returned object, so changes to the
    /// returned object will be reflected in this one.</summary>
    /// <returns>A CBORObject object.</returns>
    public CBORObject Untag() {
      CBORObject curobject = this;
      while (curobject.itemtypeValue == CBORObjectTypeTagged) {
        curobject = (CBORObject)curobject.itemValue;
      }
      return curobject;
    }

    /// <summary>Gets an object with the same value as this one but without
    /// this object's outermost tag, if any. If this object is an array, map,
    /// or byte string, the data will not be copied to the returned object,
    /// so changes to the returned object will be reflected in this one.</summary>
    /// <returns>A CBORObject object.</returns>
    public CBORObject UntagOne() {
      if (this.itemtypeValue == CBORObjectTypeTagged) {
        return (CBORObject)this.itemValue;
      }
      return this;
    }

    internal void Redefine(CBORObject cbor) {
      if (this.itemtypeValue != CBORObjectTypeTagged ||
          cbor == null || cbor.itemtypeValue != CBORObjectTypeTagged) {
        throw new InvalidOperationException();
      }
      this.tagLow = cbor.tagLow;
      this.tagHigh = cbor.tagHigh;
      this.itemValue = cbor.itemValue;
    }

    private static int MapCompare(IDictionary<CBORObject, CBORObject> mapA, IDictionary<CBORObject, CBORObject> mapB) {
      if (mapA == null) {
        return (mapB == null) ? 0 : -1;
      }
      if (mapB == null) {
        return 1;
      }
      if (mapA == mapB) {
        return 0;
      }
      int listACount = mapA.Count;
      int listBCount = mapB.Count;
      if (listACount == 0 && listBCount == 0) {
        return 0;
      }
      if (listACount == 0) {
        return -1;
      }
      if (listBCount == 0) {
        return 1;
      }
      IDictionary<CBORObject, CBORObject> sortedA =
        new SortedMap<CBORObject, CBORObject>(mapA);
      IDictionary<CBORObject, CBORObject> sortedB =
        new SortedMap<CBORObject, CBORObject>(mapB);
      IList<CBORObject> sortedASet = new List<CBORObject>(sortedA.Keys);
      IList<CBORObject> sortedBSet = new List<CBORObject>(sortedB.Keys);
      listACount = sortedASet.Count;
      listBCount = sortedBSet.Count;
      int minCount = Math.Min(listACount, listBCount);
      // Compare the keys
      for (int i = 0; i < minCount; ++i) {
        CBORObject itemA = sortedASet[i];
        CBORObject itemB = sortedBSet[i];
        if (itemA == null) {
          return -1;
        }
        int cmp = itemA.CompareTo(itemB);
        if (cmp != 0) {
          return cmp;
        }
      }
      if (listACount == listBCount) {
        // Both maps have the same keys, so compare their values
        for (int i = 0; i < minCount; ++i) {
          CBORObject keyA = sortedASet[i];
          CBORObject keyB = sortedBSet[i];
          int cmp = mapA[keyA].CompareTo(mapB[keyB]);
          if (cmp != 0) {
            return cmp;
          }
        }
        return 0;
      }
      return (listACount > listBCount) ? 1 : -1;
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
    /// <returns>True if the objects are equal; otherwise, false.</returns>
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
      if (this == otherValue) {
        return true;
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
      int hashCode = 651869431;
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
          hashCode += 651869479 * itemHashCode;
        }
        hashCode += 651869483 * (this.itemtypeValue.GetHashCode() + this.tagLow + this.tagHigh);
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
        int nextbyte = (int)(data[offset] & (int)0xff);
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
            c[i - offsetp1] = (char)(data[i] & (int)0xff);
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
      for (int i = 0xe0; i < 0xf8; ++i) {
        valueFixedObjects[i] = new CBORObject(CBORObjectTypeSimpleValue, (int)(i - 0xe0));
      }
      return valueFixedObjects;
    }

    internal static CBORObject GetFixedObject(int value) {
      return valueFixedObjects[value];
    }

    internal static int GetExpectedLength(int value) {
      return valueExpectedLengths[value];
    }
    // Expected lengths for each head byte.
    // 0 means length varies. -1 means invalid.
    private static int[] valueExpectedLengths = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  // major type 0
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
      1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 5, 9, -1, -1, -1, -1 };

    // Generate a CBOR object for head bytes with fixed length.
    // Note that this function assumes that the length of the data
    // was already checked.
    internal static CBORObject GetFixedLengthObject(int firstbyte, byte[] data) {
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
      if ((firstbyte & 0x1c) == 0x18) {
        // contains 1 to 8 extra bytes of additional information
        long uadditional = 0;
        switch (firstbyte & 0x1f) {
          case 24:
            uadditional = (int)(data[1] & (int)0xff);
            break;
          case 25:
            uadditional = ((long)(data[1] & (long)0xff)) << 8;
            uadditional |= (long)(data[2] & (long)0xff);
            break;
          case 26:
            uadditional = ((long)(data[1] & (long)0xff)) << 24;
            uadditional |= ((long)(data[2] & (long)0xff)) << 16;
            uadditional |= ((long)(data[3] & (long)0xff)) << 8;
            uadditional |= (long)(data[4] & (long)0xff);
            break;
          case 27:
            uadditional = ((long)(data[1] & (long)0xff)) << 56;
            uadditional |= ((long)(data[2] & (long)0xff)) << 48;
            uadditional |= ((long)(data[3] & (long)0xff)) << 40;
            uadditional |= ((long)(data[4] & (long)0xff)) << 32;
            uadditional |= ((long)(data[5] & (long)0xff)) << 24;
            uadditional |= ((long)(data[6] & (long)0xff)) << 16;
            uadditional |= ((long)(data[7] & (long)0xff)) << 8;
            uadditional |= (long)(data[8] & (long)0xff);
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
              BigInteger minusOne = -BigInteger.One;
              bigintAdditional = minusOne - (BigInteger)bigintAdditional;
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
              if ((int)uadditional < 32) {
                throw new CBORException("Invalid overlong simple value");
              }
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
      } else if (firstbyte == 0xa0) {
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
      int firstbyte = (int)(data[0] & (int)0xff);
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
    /// <value>True if this data item has at least one tag; otherwise, false.</value>
    public bool IsTagged {
      get {
        return this.itemtypeValue == CBORObjectTypeTagged;
      }
    }

    /// <summary>Gets the byte array used in this object, if this object is
    /// a byte string, without copying the data to a new one. This method's
    /// return value can be used to modify the array's contents. Note, though,
    /// that the array's length can't be changed.</summary>
    /// <exception cref='InvalidOperationException'>This object is
    /// not a byte string.</exception>
    /// <returns>A byte array.</returns>
    public byte[] GetByteString() {
      if (this.ItemType == CBORObjectTypeByteString) {
        return (byte[])this.ThisItem;
      } else {
        throw new InvalidOperationException("Not a byte string");
      }
    }

    /// <summary>Returns whether this object has a tag of the given number.</summary>
    /// <param name='tagValue'>The tag value to search for.</param>
    /// <returns>True if this object has a tag of the given number; otherwise,
    /// false.</returns>
    /// <exception cref='System.ArgumentException'>TagValue is less
    /// than 0.</exception>
    public bool HasTag(int tagValue) {
      if (tagValue < 0) {
        throw new ArgumentException("tagValue (" + Convert.ToString((long)tagValue, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      CBORObject obj = this;
      while (true) {
        if (!obj.IsTagged) {
          return false;
        }
        if (obj.tagHigh == 0 && tagValue == obj.tagLow) {
          return true;
        }
        obj = (CBORObject)obj.itemValue;
        #if DEBUG
        if (obj == null) {
          throw new ArgumentNullException("obj");
        }
        #endif
      }
    }

    /// <summary>Returns whether this object has a tag of the given number.</summary>
    /// <param name='bigTagValue'>The tag value to search for.</param>
    /// <returns>True if this object has a tag of the given number; otherwise,
    /// false.</returns>
    /// <exception cref='System.ArgumentNullException'>BigTagValue
    /// is null.</exception>
    /// <exception cref='System.ArgumentException'>BigTagValue is
    /// less than 0.</exception>
    public bool HasTag(BigInteger bigTagValue) {
      if (bigTagValue == null) {
        throw new ArgumentNullException("bigTagValue");
      }
      if (!(bigTagValue.Sign >= 0)) {
        throw new ArgumentException("doesn't satisfy bigTagValue.Sign>= 0");
      }
      BigInteger[] bigTags = this.GetTags();
      foreach (BigInteger bigTag in bigTags) {
        if (bigTagValue.Equals(bigTag)) {
          return true;
        }
      }
      return false;
    }

    private static BigInteger LowHighToBigInteger(int tagLow, int tagHigh) {
      byte[] uabytes = null;
      if (tagHigh != 0) {
        uabytes = new byte[9];
        uabytes[7] = (byte)((tagHigh >> 24) & 0xff);
        uabytes[6] = (byte)((tagHigh >> 16) & 0xff);
        uabytes[5] = (byte)((tagHigh >> 8) & 0xff);
        uabytes[4] = (byte)(tagHigh & 0xff);
        uabytes[3] = (byte)((tagLow >> 24) & 0xff);
        uabytes[2] = (byte)((tagLow >> 16) & 0xff);
        uabytes[1] = (byte)((tagLow >> 8) & 0xff);
        uabytes[0] = (byte)(tagLow & 0xff);
        uabytes[8] = 0;
        return new BigInteger((byte[])uabytes);
      } else if (tagLow != 0) {
        uabytes = new byte[5];
        uabytes[3] = (byte)((tagLow >> 24) & 0xff);
        uabytes[2] = (byte)((tagLow >> 16) & 0xff);
        uabytes[1] = (byte)((tagLow >> 8) & 0xff);
        uabytes[0] = (byte)(tagLow & 0xff);
        uabytes[4] = 0;
        return new BigInteger((byte[])uabytes);
      } else {
        return BigInteger.Zero;
      }
    }

    private static BigInteger[] valueEmptyTags = new BigInteger[0];

    /// <summary>Gets a list of all tags, from outermost to innermost.</summary>
    /// <returns>An array of tags, or the empty string if this object is untagged.</returns>
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

    /// <summary>Gets the outermost tag for this CBOR data item, or -1 if the
    /// item is untagged.</summary>
    /// <value>The outermost tag for this CBOR data item, or -1 if the item
    /// is untagged.</value>
    public BigInteger OutermostTag {
      get {
        if (!this.IsTagged) {
          return BigInteger.Zero;
        }
        if (this.tagHigh == 0 &&
            this.tagLow >= 0 &&
            this.tagLow < 0x10000) {
          return (BigInteger)this.tagLow;
        }
        return LowHighToBigInteger(
          this.tagLow,
          this.tagHigh);
      }
    }

    /// <summary>Gets the last defined tag for this CBOR data item, or -1 if
    /// the item is untagged.</summary>
    /// <value>The last defined tag for this CBOR data item, or -1 if the item
    /// is untagged.</value>
    public BigInteger InnermostTag {
      get {
        if (!this.IsTagged) {
          return BigInteger.Zero - BigInteger.One;
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
    /// <param name='index'>Zero-based index of the element.</param>
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

    /// <summary>Gets a collection of the keys of this CBOR object in an undefined
    /// order.</summary>
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

    /// <summary>Gets a collection of the values of this CBOR object. If this
    /// object is a map, returns one value for each key in the map in an undefined
    /// order. If this is an array, returns all the values of the array in the
    /// order they are listed.</summary>
    /// <exception cref='System.InvalidOperationException'>This object
    /// is not a map or an array.</exception>
    /// <value>A collection of the values of this CBOR object.</value>
    public ICollection<CBORObject> Values {
      get {
        if (this.ItemType == CBORObjectTypeMap) {
          IDictionary<CBORObject, CBORObject> dict = this.AsMap();
          return dict.Values;
        } else if (this.ItemType == CBORObjectTypeArray) {
          IList<CBORObject> list = this.AsList();
          return new System.Collections.ObjectModel.ReadOnlyCollection<CBORObject>(list);
        } else {
          throw new InvalidOperationException("Not a map or array");
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
    /// <param name='key'>A key that points to the desired value.</param>
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
    /// <param name='key'>A CBOR object representing the key. Can be null,
    /// in which case this value is converted to CBORObject.Null.</param>
    /// <param name='value'>A CBOR object representing the value. Can be
    /// null, in which case this value is converted to CBORObject.Null.</param>
    /// <exception cref='System.ArgumentException'>Key already exists
    /// in this map.</exception>
    /// <exception cref='InvalidOperationException'>This object is
    /// not a map.</exception>
    /// <returns>This object.</returns>
    public CBORObject Add(CBORObject key, CBORObject value) {
      if (key == null) {
        key = CBORObject.Null;
      }
      if (value == null) {
        value = CBORObject.Null;
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
      return this;
    }

    /// <summary>Converts an object to a CBOR object and adds it to this map.</summary>
    /// <param name='key'>A string representing the key. Can be null, in
    /// which case this value is converted to CBORObject.Null.</param>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='key'/> or "value" has an unsupported type.</exception>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='key'/> already exists in this map.</exception>
    /// <exception cref='InvalidOperationException'>This object is
    /// not a map.</exception>
    /// <returns>This object.</returns>
    /// <param name='valueOb'>An arbitrary object. Can be null, in which
    /// case this value is converted to CBORObject.Null.</param>
    public CBORObject Add(object key, object valueOb) {
      return this.Add(CBORObject.FromObject(key), CBORObject.FromObject(valueOb));
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

    /// <summary>Determines whether a value of the given key exists in this
    /// object.</summary>
    /// <param name='key'>A string that serves as the key.</param>
    /// <returns>True if the given key (as a CBOR object) is found, or false
    /// if the given key is not found or this object is not a map.</returns>
    /// <exception cref='System.ArgumentNullException'>Key is null
    /// (as opposed to CBORObject.Null).</exception>
    public bool ContainsKey(string key) {
      if (key == null) {
        throw new ArgumentNullException("key");
      }
      if (this.ItemType == CBORObjectTypeMap) {
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        return map.ContainsKey(CBORObject.FromObject(key));
      } else {
        return false;
      }
    }

    /// <summary>Adds a new object to the end of this array.</summary>
    /// <param name='obj'>A CBOR object.</param>
    /// <exception cref='System.InvalidOperationException'>This object
    /// is not an array.</exception>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='obj'/> is null (as opposed to CBORObject.Null).</exception>
    /// <returns>This object.</returns>
    public CBORObject Add(CBORObject obj) {
      if (obj == null) {
        throw new ArgumentNullException("obj");
      }
      if (this.ItemType == CBORObjectTypeArray) {
        IList<CBORObject> list = this.AsList();
        list.Add(obj);
        return this;
      } else {
        throw new InvalidOperationException("Not an array");
      }
    }

    /// <summary>Converts an object to a CBOR object and adds it to the end
    /// of this array.</summary>
    /// <param name='obj'>A CBOR object.</param>
    /// <exception cref='System.InvalidOperationException'>This object
    /// is not an array.</exception>
    /// <exception cref='System.ArgumentException'>The object's type
    /// is not supported.</exception>
    /// <returns>This object.</returns>
    public CBORObject Add(object obj) {
      if (this.ItemType == CBORObjectTypeArray) {
        IList<CBORObject> list = this.AsList();
        list.Add(CBORObject.FromObject(obj));
        return this;
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
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        throw new InvalidOperationException("Not a number type");
      }
      return cn.AsDouble(this.ThisItem);
    }

    /// <summary>Converts this object to a decimal number.</summary>
    /// <returns>A decimal number for this object's value. If this object
    /// is a rational number with a nonterminating decimal expansion, returns
    /// a decimal number rounded to 34 digits.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    public ExtendedDecimal AsExtendedDecimal() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        throw new InvalidOperationException("Not a number type");
      }
      return cn.AsExtendedDecimal(this.ThisItem);
    }

    /// <summary>Converts this object to a rational number.</summary>
    /// <returns>A rational number for this object's value.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    public ExtendedRational AsExtendedRational() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        throw new InvalidOperationException("Not a number type");
      }
      return cn.AsExtendedRational(this.ThisItem);
    }

    /// <summary>Converts this object to an arbitrary-precision binary
    /// floating point number.</summary>
    /// <returns>An arbitrary-precision binary floating point number
    /// for this object's value. Note that if this object is a decimal number
    /// with a fractional part, the conversion may lose information depending
    /// on the number. If this object is a rational number with a nonterminating
    /// binary expansion, returns a decimal number rounded to 113 digits.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    public ExtendedFloat AsExtendedFloat() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        throw new InvalidOperationException("Not a number type");
      }
      return cn.AsExtendedFloat(this.ThisItem);
    }

    /// <summary>Converts this object to a 32-bit floating point number.</summary>
    /// <returns>The closest 32-bit floating point number to this object.
    /// The return value can be positive infinity or negative infinity if
    /// this object's value exceeds the range of a 32-bit floating point number.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    public float AsSingle() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        throw new InvalidOperationException("Not a number type");
      }
      return cn.AsSingle(this.ThisItem);
    }

    /// <summary>Converts this object to an arbitrary-precision integer.
    /// Fractional values are truncated to an integer.</summary>
    /// <returns>The closest big integer to this object.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    /// <exception cref='OverflowException'>This object's value is infinity
    /// or not-a-number (NaN).</exception>
    public BigInteger AsBigInteger() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        throw new InvalidOperationException("Not a number type");
      }
      return cn.AsBigInteger(this.ThisItem);
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
    /// exceeds the range of a byte (would be less than 0 or greater than 255
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
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        throw new InvalidOperationException("Not a number type");
      }
      return cn.AsInt64(this.ThisItem);
    }

    /// <summary>Returns whether this object's value can be converted to
    /// a 32-bit floating point number without loss of its numerical value.</summary>
    /// <returns>Whether this object's value can be converted to a 32-bit
    /// floating point number without loss of its numerical value. Returns
    /// true if this is a not-a-number value, even if the value's diagnostic
    /// information can't fit in a 32-bit floating point number.</returns>
    public bool CanFitInSingle() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        return false;
      }
      return cn.CanFitInSingle(this.ThisItem);
    }

    /// <summary>Returns whether this object's value can be converted to
    /// a 64-bit floating point number without loss of its numerical value.</summary>
    /// <returns>Whether this object's value can be converted to a 64-bit
    /// floating point number without loss of its numerical value. Returns
    /// true if this is a not-a-number value, even if the value's diagnostic
    /// information can't fit in a 64-bit floating point number.</returns>
    public bool CanFitInDouble() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        return false;
      }
      return cn.CanFitInDouble(this.ThisItem);
    }

    /// <summary>Returns whether this object's value is an integral value,
    /// is -(2^32) or greater, and is less than 2^32.</summary>
    /// <returns>True if this object's value is an integral value, is -(2^32)
    /// or greater, and is less than 2^32; otherwise, false.</returns>
    public bool CanFitInInt32() {
      if (!this.CanFitInInt64()) {
        return false;
      }
      long v = this.AsInt64();
      return v >= Int32.MinValue && v <= Int32.MaxValue;
    }

    /// <summary>Returns whether this object's value is an integral value,
    /// is -(2^63) or greater, and is less than 2^63.</summary>
    /// <returns>True if this object's value is an integral value, is -(2^63)
    /// or greater, and is less than 2^63; otherwise, false.</returns>
    public bool CanFitInInt64() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        return false;
      }
      return cn.CanFitInInt64(this.ThisItem);
    }

    /// <summary>Returns whether this object's value, truncated to an integer,
    /// would be -(2^63) or greater, and less than 2^63.</summary>
    /// <returns>True if this object's value, truncated to an integer, would
    /// be -(2^63) or greater, and less than 2^63; otherwise, false.</returns>
    public bool CanTruncatedIntFitInInt64() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        return false;
      }
      return cn.CanTruncatedIntFitInInt64(this.ThisItem);
    }

    /// <summary>Returns whether this object's value, truncated to an integer,
    /// would be -(2^31) or greater, and less than 2^31.</summary>
    /// <returns>True if this object's value, truncated to an integer, would
    /// be -(2^31) or greater, and less than 2^31; otherwise, false.</returns>
    public bool CanTruncatedIntFitInInt32() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        return false;
      }
      return cn.CanTruncatedIntFitInInt32(this.ThisItem);
    }

    /// <summary>Gets a value indicating whether this object represents
    /// an integral number, that is, a number without a fractional part. Infinity
    /// and not-a-number are not considered integral.</summary>
    /// <value>True if this object represents an integral number, that is,
    /// a number without a fractional part; otherwise, false.</value>
    public bool IsIntegral {
      get {
        ICBORNumber cn = NumberInterfaces[this.ItemType];
        if (cn == null) {
          return false;
        }
        return cn.IsIntegral(this.ThisItem);
      }
    }

    private int AsInt32(int minValue, int maxValue) {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        throw new InvalidOperationException("not a number type");
      }
      return cn.AsInt32(this.ThisItem, minValue, maxValue);
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
        return new CBORReader(stream).Read(null);
      } catch (IOException ex) {
        throw new CBORException("I/O error occurred.", ex);
      }
    }

    private static void WriteObjectArray(
      IList<CBORObject> list,
      Stream outputStream) {
      WriteObjectArray(list, outputStream, null);
    }

    private static void WriteObjectMap(IDictionary<CBORObject, CBORObject> map, Stream outputStream) {
      WriteObjectMap(map, outputStream, null);
    }

    private static IList<object> PushObject(IList<object> stack, object parent, object child) {
      if (stack == null) {
        stack = new List<object>();
        stack.Add(parent);
      }
      foreach (object o in stack) {
        if (o == child) {
          throw new ArgumentException("Circular reference in data structure");
        }
      }
      stack.Add(child);
      return stack;
    }

    private static IList<object> WriteChildObject(
      object parentThisItem,
      CBORObject child,
      Stream outputStream,
      IList<object> stack) {
      if (child == null) {
        outputStream.WriteByte(0xf6);
      } else {
        int type = child.ItemType;
        if (type == CBORObjectTypeArray) {
          stack = PushObject(stack, parentThisItem, child.ThisItem);
          child.WriteTags(outputStream);
          WriteObjectArray(child.AsList(), outputStream, stack);
          stack.RemoveAt(stack.Count - 1);
        } else if (type == CBORObjectTypeMap) {
          stack = PushObject(stack, parentThisItem, child.ThisItem);
          child.WriteTags(outputStream);
          WriteObjectMap(child.AsMap(), outputStream, stack);
          stack.RemoveAt(stack.Count - 1);
        } else {
          child.WriteTo(outputStream);
        }
      }
      return stack;
    }

    private static void WriteObjectArray(
      IList<CBORObject> list,
      Stream outputStream,
      IList<object> stack) {
      object thisObj = list;
      WritePositiveInt(4, list.Count, outputStream);
      foreach (CBORObject i in list) {
        stack = WriteChildObject(thisObj, i, outputStream, stack);
      }
    }

    private static void WriteObjectMap(IDictionary<CBORObject, CBORObject> map, Stream outputStream, IList<object> stack) {
      object thisObj = map;
      WritePositiveInt(5, map.Count, outputStream);
      foreach (KeyValuePair<CBORObject, CBORObject> entry in map) {
        CBORObject key = entry.Key;
        CBORObject value = entry.Value;
        stack = WriteChildObject(thisObj, key, outputStream, stack);
        stack = WriteChildObject(thisObj, value, outputStream, stack);
      }
    }

    private static byte[] GetPositiveIntBytes(int type, int value) {
      if (value < 0) {
        throw new ArgumentException("value (" + Convert.ToString((long)value, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (value < 24) {
        return new byte[] { (byte)((byte)value | (byte)(type << 5)) };
      } else if (value <= 0xff) {
        return new byte[] { (byte)(24 | (type << 5)),
          (byte)(value & 0xff) };
      } else if (value <= 0xffff) {
        return new byte[] { (byte)(25 | (type << 5)),
          (byte)((value >> 8) & 0xff),
          (byte)(value & 0xff) };
      } else {
        return new byte[] { (byte)(26 | (type << 5)),
          (byte)((value >> 24) & 0xff),
          (byte)((value >> 16) & 0xff),
          (byte)((value >> 8) & 0xff),
          (byte)(value & 0xff) };
      }
    }

    private static void WritePositiveInt(int type, int value, Stream s) {
      byte[] bytes = GetPositiveIntBytes(type, value);
      s.Write(bytes, 0, bytes.Length);
    }

    private static byte[] GetPositiveInt64Bytes(int type, long value) {
      if (value < 0) {
        throw new ArgumentException("value (" + Convert.ToString((long)value, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (value < 24) {
        return new byte[] { (byte)((byte)value | (byte)(type << 5)) };
      } else if (value <= 0xFFL) {
        return new byte[] { (byte)(24 | (type << 5)),
          (byte)(value & 0xff) };
      } else if (value <= 0xFFFFL) {
        return new byte[] { (byte)(25 | (type << 5)),
          (byte)((value >> 8) & 0xff),
          (byte)(value & 0xff) };
      } else if (value <= 0xFFFFFFFFL) {
        return new byte[] { (byte)(26 | (type << 5)),
          (byte)((value >> 24) & 0xff),
          (byte)((value >> 16) & 0xff),
          (byte)((value >> 8) & 0xff),
          (byte)(value & 0xff) };
      } else {
        return new byte[] { (byte)(27 | (type << 5)),
          (byte)((value >> 56) & 0xff),
          (byte)((value >> 48) & 0xff),
          (byte)((value >> 40) & 0xff),
          (byte)((value >> 32) & 0xff),
          (byte)((value >> 24) & 0xff),
          (byte)((value >> 16) & 0xff),
          (byte)((value >> 8) & 0xff),
          (byte)(value & 0xff) };
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
        if (c <= 0x7f) {
          if (byteIndex >= StreamedStringBufferLength) {
            // Write bytes retrieved so far
            if (!streaming) {
              stream.WriteByte((byte)0x7f);
            }
            WritePositiveInt(3, byteIndex, stream);
            stream.Write(bytes, 0, byteIndex);
            byteIndex = 0;
            streaming = true;
          }
          bytes[byteIndex++] = (byte)c;
        } else if (c <= 0x7ff) {
          if (byteIndex + 2 > StreamedStringBufferLength) {
            // Write bytes retrieved so far, the next three bytes
            // would exceed the length, and the CBOR spec forbids
            // splitting characters when generating text strings
            if (!streaming) {
              stream.WriteByte((byte)0x7f);
            }
            WritePositiveInt(3, byteIndex, stream);
            stream.Write(bytes, 0, byteIndex);
            byteIndex = 0;
            streaming = true;
          }
          bytes[byteIndex++] = (byte)(0xc0 | ((c >> 6) & 0x1f));
          bytes[byteIndex++] = (byte)(0x80 | (c & 0x3f));
        } else {
          if ((c & 0xfc00) == 0xd800 && index + 1 < str.Length &&
              str[index + 1] >= 0xdc00 && str[index + 1] <= 0xdfff) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c - 0xd800) << 10) + (str[index + 1] - 0xdc00);
            ++index;
          } else if ((c & 0xf800) == 0xd800) {
            // unpaired surrogate, write U + FFFD instead
            c = 0xfffd;
          }
          if (c <= 0xffff) {
            if (byteIndex + 3 > StreamedStringBufferLength) {
              // Write bytes retrieved so far, the next three bytes
              // would exceed the length, and the CBOR spec forbids
              // splitting characters when generating text strings
              if (!streaming) {
                stream.WriteByte((byte)0x7f);
              }
              WritePositiveInt(3, byteIndex, stream);
              stream.Write(bytes, 0, byteIndex);
              byteIndex = 0;
              streaming = true;
            }
            bytes[byteIndex++] = (byte)(0xe0 | ((c >> 12) & 0x0f));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 6) & 0x3f));
            bytes[byteIndex++] = (byte)(0x80 | (c & 0x3f));
          } else {
            if (byteIndex + 4 > StreamedStringBufferLength) {
              // Write bytes retrieved so far, the next four bytes
              // would exceed the length, and the CBOR spec forbids
              // splitting characters when generating text strings
              if (!streaming) {
                stream.WriteByte((byte)0x7f);
              }
              WritePositiveInt(3, byteIndex, stream);
              stream.Write(bytes, 0, byteIndex);
              byteIndex = 0;
              streaming = true;
            }
            bytes[byteIndex++] = (byte)(0xf0 | ((c >> 18) & 0x07));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 12) & 0x3f));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 6) & 0x3f));
            bytes[byteIndex++] = (byte)(0x80 | (c & 0x3f));
          }
        }
      }
      WritePositiveInt(3, byteIndex, stream);
      stream.Write(bytes, 0, byteIndex);
      if (streaming) {
        stream.WriteByte((byte)0xff);
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

    /// <summary>Writes a binary floating-point number in CBOR format to
    /// a data stream as follows: <list type=''> <item>If the value is null,
    /// writes the byte 0xF6.</item>
    /// <item>If the value is negative zero, infinity, or NaN, converts the
    /// number to a <c>double</c>
    /// and writes that <c>double</c>
    /// . If negative zero should not be written this way, use the Plus method
    /// to convert the value beforehand.</item>
    /// <item>If the value has an exponent of zero, writes the value as an unsigned
    /// integer or signed integer if the number can fit either type or as a big
    /// integer otherwise.</item>
    /// <item>In all other cases, writes the value as a big float.</item>
    /// </list>
    /// </summary>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
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
          stream.WriteByte(0xd9);  // tag 265
          stream.WriteByte(0x01);
          stream.WriteByte(0x09);
          stream.WriteByte(0x82);  // array, length 2
        } else {
          stream.WriteByte(0xc5);  // tag 5
          stream.WriteByte(0x82);  // array, length 2
        }
        Write(bignum.Exponent, stream);
        Write(bignum.Mantissa, stream);
      }
    }

    /// <summary>Writes a rational number in CBOR format to a data stream.</summary>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    /// <param name='rational'>An ExtendedRational object.</param>
    /// <param name='stream'>A writable data stream.</param>
    public static void Write(ExtendedRational rational, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      if (rational == null) {
        stream.WriteByte(0xf6);
        return;
      }
      if (!rational.IsFinite) {
        Write(rational.ToDouble(), stream);
        return;
      }
      if (rational.Denominator.Equals(BigInteger.One)) {
        Write(rational.Numerator, stream);
        return;
      }
      stream.WriteByte(0xd8);  // tag 30
      stream.WriteByte(0x1e);
      stream.WriteByte(0x82);  // array, length 2
      Write(rational.Numerator, stream);
      Write(rational.Denominator, stream);
    }

    /// <summary>Writes a decimal floating-point number in CBOR format
    /// to a data stream, as follows: <list type=''> <item>If the value is
    /// null, writes the byte 0xF6.</item>
    /// <item>If the value is negative zero, infinity, or NaN, converts the
    /// number to a <c>double</c>
    /// and writes that <c>double</c>
    /// . If negative zero should not be written this way, use the Plus method
    /// to convert the value beforehand.</item>
    /// <item>If the value has an exponent of zero, writes the value as an unsigned
    /// integer or signed integer if the number can fit either type or as a big
    /// integer otherwise.</item>
    /// <item>In all other cases, writes the value as a decimal number.</item>
    /// </list>
    /// </summary>
    /// <param name='bignum'>Decimal fraction to write. Can be null.</param>
    /// <param name='stream'>Stream to write to.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
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
          stream.WriteByte(0xd9);  // tag 264
          stream.WriteByte(0x01);
          stream.WriteByte(0x08);
          stream.WriteByte(0x82);  // array, length 2
        } else {
          stream.WriteByte(0xc4);  // tag 4
          stream.WriteByte(0x82);  // array, length 2
        }
        Write(bignum.Exponent, stream);
        Write(bignum.Mantissa, stream);
      }
    }

    /// <summary>Writes a big integer in CBOR format to a data stream.</summary>
    /// <param name='bigint'>Big integer to write. Can be null.</param>
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
        if (byteCount != 0) {
          int half = byteCount >> 1;
          int right = byteCount - 1;
          for (int i = 0; i < half; ++i, --right) {
            byte value = bytes[i];
            bytes[i] = bytes[right];
            bytes[right] = value;
          }
        }
        switch (byteCount) {
          case 0:
            stream.WriteByte((byte)(datatype << 5));
            return;
          case 1:
            WritePositiveInt(datatype, ((int)bytes[0]) & 0xff, stream);
            break;
          case 2:
            stream.WriteByte((byte)((datatype << 5) | 25));
            stream.Write(bytes, 0, byteCount);
            break;
          case 3:
            stream.WriteByte((byte)((datatype << 5) | 26));
            stream.WriteByte((byte)0);
            stream.Write(bytes, 0, byteCount);
            break;
          case 4:
            stream.WriteByte((byte)((datatype << 5) | 26));
            stream.Write(bytes, 0, byteCount);
            break;
          case 5:
            stream.WriteByte((byte)((datatype << 5) | 27));
            stream.WriteByte((byte)0);
            stream.WriteByte((byte)0);
            stream.WriteByte((byte)0);
            stream.Write(bytes, 0, byteCount);
            break;
          case 6:
            stream.WriteByte((byte)((datatype << 5) | 27));
            stream.WriteByte((byte)0);
            stream.WriteByte((byte)0);
            stream.Write(bytes, 0, byteCount);
            break;
          case 7:
            stream.WriteByte((byte)((datatype << 5) | 27));
            stream.WriteByte((byte)0);
            stream.Write(bytes, 0, byteCount);
            break;
          case 8:
            stream.WriteByte((byte)((datatype << 5) | 27));
            stream.Write(bytes, 0, byteCount);
            break;
          default:
            stream.WriteByte((datatype == 0) ?
                             (byte)0xc2 :
                             (byte)0xc3);
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
        ExtendedFloat flo = (ExtendedFloat)this.ThisItem;
        Write(flo, stream);
      } else if (type == CBORObjectTypeExtendedRational) {
        ExtendedRational flo = (ExtendedRational)this.ThisItem;
        Write(flo, stream);
      } else if (type == CBORObjectTypeMap) {
        WriteObjectMap(this.AsMap(), stream);
      } else if (type == CBORObjectTypeSimpleValue) {
        int value = (int)this.ThisItem;
        if (value < 24) {
          stream.WriteByte((byte)(0xe0 + value));
        } else {
          #if DEBUG
          if (value < 32) {
            throw new ArgumentException("value (" + Convert.ToString((long)value, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "32");
          }
          #endif

          stream.WriteByte(0xf8);
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

    /// <summary>Writes a 64-bit signed integer in CBOR format to a data stream.</summary>
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
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      int type = 0;
      if (value < 0) {
        ++value;
        value = -value;
        type = 0x20;
      }
      if (value < 24) {
        stream.WriteByte((byte)(value | type));
      } else if (value <= 0xff) {
        byte[] bytes = new byte[] { (byte)(24 | type), (byte)(value & 0xff) };
        stream.Write(bytes, 0, 2);
      } else if (value <= 0xffff) {
        byte[] bytes = new byte[] { (byte)(25 | type), (byte)((value >> 8) & 0xff), (byte)(value & 0xff) };
        stream.Write(bytes, 0, 3);
      } else {
        byte[] bytes = new byte[] { (byte)(26 | type), (byte)((value >> 24) & 0xff), (byte)((value >> 16) & 0xff), (byte)((value >> 8) & 0xff), (byte)(value & 0xff) };
        stream.Write(bytes, 0, 5);
      }
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

    /// <summary>Writes a byte (0 to 255) in CBOR format to a data stream. If
    /// the value is less than 24, writes that byte. If the value is 25 to 255,
    /// writes the byte 24, then this byte's value.</summary>
    /// <param name='value'>The value to write.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error occurred.</exception>
    /// <param name='stream'>A writable data stream.</param>
    public static void Write(byte value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      if ((((int)value) & 0xff) < 24) {
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
      byte[] data = new byte[] { (byte)0xfa,
        (byte)((bits >> 24) & 0xff),
        (byte)((bits >> 16) & 0xff),
        (byte)((bits >> 8) & 0xff),
        (byte)(bits & 0xff) };
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
      byte[] data = new byte[] { (byte)0xfb,
        (byte)((bits >> 56) & 0xff),
        (byte)((bits >> 48) & 0xff),
        (byte)((bits >> 40) & 0xff),
        (byte)((bits >> 32) & 0xff),
        (byte)((bits >> 24) & 0xff),
        (byte)((bits >> 16) & 0xff),
        (byte)((bits >> 8) & 0xff),
        (byte)(bits & 0xff) };
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
          tagbyte = (byte)(0xc0 + (int)this.tagLow);
        }
      }
      if (!hasComplexTag) {
        if (this.ItemType == CBORObjectTypeTextString) {
          byte[] ret = GetOptimizedBytesIfShortAscii(
            this.AsString(),
            tagged ? (((int)tagbyte) & 0xff) : -1);
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
            new byte[] { tagbyte, (byte)0xfa,
            (byte)((bits >> 24) & 0xff),
            (byte)((bits >> 16) & 0xff),
            (byte)((bits >> 8) & 0xff),
            (byte)(bits & 0xff) } :
            new byte[] { (byte)0xfa,
            (byte)((bits >> 24) & 0xff),
            (byte)((bits >> 16) & 0xff),
            (byte)((bits >> 8) & 0xff),
            (byte)(bits & 0xff) };
        } else if (this.ItemType == CBORObjectTypeDouble) {
          double value = (double)this.ThisItem;
          long bits = BitConverter.ToInt64(BitConverter.GetBytes(value), 0);
          return tagged ?
            new byte[] { tagbyte, (byte)0xfb,
            (byte)((bits >> 56) & 0xff),
            (byte)((bits >> 48) & 0xff),
            (byte)((bits >> 40) & 0xff),
            (byte)((bits >> 32) & 0xff),
            (byte)((bits >> 24) & 0xff),
            (byte)((bits >> 16) & 0xff),
            (byte)((bits >> 8) & 0xff),
            (byte)(bits & 0xff) } :
            new byte[] { (byte)0xfb,
            (byte)((bits >> 56) & 0xff),
            (byte)((bits >> 48) & 0xff),
            (byte)((bits >> 40) & 0xff),
            (byte)((bits >> 32) & 0xff),
            (byte)((bits >> 24) & 0xff),
            (byte)((bits >> 16) & 0xff),
            (byte)((bits >> 8) & 0xff),
            (byte)(bits & 0xff) };
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
    /// <param name='value'>The value to write. Can be null.</param>
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

    /// <summary>Writes an arbitrary object to a CBOR data stream. Currently,
    /// the following objects are supported: <list type=''> <item>Lists
    /// of CBORObject.</item>
    /// <item>Maps of CBORObject.</item>
    /// <item>Null.</item>
    /// <item>Any object accepted by the FromObject static methods.</item>
    /// </list>
    /// </summary>
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

    private static int SkipWhitespaceOrByteOrderMarkJSON(CharacterReader reader) {
      bool allowBOM = true;
      while (true) {
        int c = reader.NextChar();
        if (c == -1 || (c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09)) {
          if (!allowBOM || c != 0xfeff) {
            return c;
          }
        }
        allowBOM = false;
      }
    }

    private static string NextJSONString(CharacterReader reader, int quote) {
      int c;
      StringBuilder sb = null;
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
                // Now allowed to be escaped under RFC 7159
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
          if ((c & 0x1ffc00) != 0xdc00) {
            // Note: this includes the ending quote
            // and supplementary characters
            throw reader.NewError("Unpaired surrogate code point");
          }
          if (escaped != surrogateEscaped) {
            throw reader.NewError("Pairing escaped surrogate with unescaped surrogate");
          }
          surrogate = false;
        } else if ((c & 0x1ffc00) == 0xd800) {
          surrogate = true;
          surrogateEscaped = escaped;
        } else if ((c & 0x1ffc00) == 0xdc00) {
          throw reader.NewError("Unpaired surrogate code point");
        }
        if (c == quote && !escaped) {
          // End quote reached
          if (sb == null) {
            // No string builder created yet, so this
            // is an empty string
            return String.Empty;
          }
          return sb.ToString();
        }
        if (sb == null) {
          sb = new StringBuilder();
        }
        if (c <= 0xffff) {
          sb.Append((char)c);
        } else if (c <= 0x10ffff) {
          sb.Append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
          sb.Append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
        }
      }
    }

    private static CBORObject NextJSONValue(CharacterReader reader, int firstChar, bool noDuplicates, int[] nextChar, int depth) {
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
        obj = ParseJSONObject(reader, noDuplicates, depth + 1);
        nextChar[0] = SkipWhitespaceJSON(reader);
        return obj;
      } else if (c == '[') {
        // Parse an array
        obj = ParseJSONArray(reader, noDuplicates, depth + 1);
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
          throw reader.NewError("JSON number can't be parsed. " + str);
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

    private static CBORObject ParseJSONValue(
      CharacterReader reader,
      bool noDuplicates,
      bool skipByteOrderMark,
      bool objectOrArrayOnly,
      int depth) {
      if (depth > 1000) {
        throw reader.NewError("Too deeply nested");
      }
      int c;
      c = skipByteOrderMark ?
        SkipWhitespaceOrByteOrderMarkJSON(reader) :
        SkipWhitespaceJSON(reader);
      if (!skipByteOrderMark && c == (char)0xfeff) {
        throw reader.NewError("JSON object began with a byte order mark (U+FEFF)");
      }
      if (c == '[') {
        return ParseJSONArray(reader, noDuplicates, depth);
      }
      if (c == '{') {
        return ParseJSONObject(reader, noDuplicates, depth);
      }
      if (objectOrArrayOnly) {
        throw reader.NewError("A JSON object must begin with '{' or '['");
      }
      int[] nextChar = new int[1];
      return NextJSONValue(reader, c, noDuplicates, nextChar, depth);
    }

    private static CBORObject ParseJSONObject(CharacterReader reader, bool noDuplicates, int depth) {
      // Assumes that the last character read was '{'
      if (depth > 1000) {
        throw reader.NewError("Too deeply nested");
      }
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
        myHashMap[key] = NextJSONValue(reader, SkipWhitespaceJSON(reader), noDuplicates, nextchar, depth);
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

    private static CBORObject ParseJSONArray(CharacterReader reader, bool noDuplicates, int depth) {
      // Assumes that the last character read was '['
      if (depth > 1000) {
        throw reader.NewError("Too deeply nested");
      }
      var myArrayList = new List<CBORObject>();
      bool seenComma = false;
      int[] nextchar = new int[1];
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
          myArrayList.Add(NextJSONValue(reader, c, noDuplicates, nextchar, depth));
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
    /// Notation (JSON) format. <para>If a JSON object has the same key, only
    /// the last given value will be used for each duplicated key. The JSON
    /// string may not begin with a byte order mark (U + FEFF).</para>
    /// </summary>
    /// <param name='str'>A string in JSON format.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='str'/> is null.</exception>
    /// <exception cref='CBORException'>The string is not in JSON format.</exception>
    /// <returns>A CBORObject object.</returns>
    public static CBORObject FromJSONString(string str) {
      CharacterReader reader = new CharacterReader(str);
      CBORObject obj = ParseJSONValue(reader, false, false, false, 0);
      if (SkipWhitespaceJSON(reader) != -1) {
        throw reader.NewError("End of string not reached");
      }
      return obj;
    }

    /// <summary>Generates a CBOR object from a data stream in JavaScript
    /// Object Notation (JSON) format and UTF-8 encoding. The JSON stream
    /// may begin with a byte order mark (U + FEFF); however, this implementation's
    /// ToJSONString method will not place this character at the beginning
    /// of a JSON text, since doing so is forbidden under RFC 7159. <para>If
    /// a JSON object has the same key, only the last given value will be used
    /// for each duplicated key.</para>
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
        CBORObject obj = ParseJSONValue(reader, false, true, false, 0);
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

    private const string Hex16 = "0123456789ABCDEF";

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
            sb.Append(Hex16[(int)(c >> 4)]);
            sb.Append(Hex16[(int)(c & 15)]);
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
    /// not only with arrays and maps, but also integers, strings, byte arrays,
    /// and other JSON data types.</summary>
    /// <returns>A string object containing the converted object.</returns>
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
          case CBORObjectTypeExtendedRational: {
            ExtendedRational dec = (ExtendedRational)this.ThisItem;
            ExtendedDecimal f = dec.ToExtendedDecimalExactIfPossible(
              PrecisionContext.Decimal128.WithUnlimitedExponents());
            if (!f.IsFinite) {
              return "null";
            } else {
              return f.ToString();
            }
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
            if (flo.IsFinite &&
              BigInteger.Abs(flo.Exponent).CompareTo((BigInteger)1000) > 0) {
              // Too inefficient to convert to a decimal number
              // from a bigfloat with a very high exponent,
              // so convert to double instead
              double f = flo.ToDouble();
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
            return flo.ToString();
          }
          default: {
            StringBuilder sb = new StringBuilder();
            this.ToJSONStringInternal(sb);
            return sb.ToString();
          }
      }
    }

    private void ToJSONStringInternal(StringBuilder sb) {
      int type = this.ItemType;
      switch (type) {
          case CBORObjectTypeByteString: {
            sb.Append('\"');
            if (this.HasTag(22)) {
              Base64.ToBase64(sb, (byte[])this.ThisItem, false);
            } else if (this.HasTag(23)) {
              CBORUtilities.ToBase16(sb, (byte[])this.ThisItem);
            } else {
              Base64.ToBase64URL(sb, (byte[])this.ThisItem, false);
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
              i.ToJSONStringInternal(sb);
              first = false;
            }
            sb.Append(']');
            break;
          }
          case CBORObjectTypeExtendedRational: {
            ExtendedRational dec = (ExtendedRational)this.ThisItem;
            ExtendedDecimal f = dec.ToExtendedDecimalExactIfPossible(
              PrecisionContext.Decimal128.WithUnlimitedExponents());
            if (!f.IsFinite) {
              sb.Append("null");
            } else {
              sb.Append(f.ToString());
            }
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
              value.ToJSONStringInternal(sb);
              first = false;
            }
            if (hasNonStringKeys) {
              sb.Remove(oldLength, sb.Length - oldLength);
              IDictionary<string, CBORObject> stringMap = new Dictionary<String, CBORObject>();
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
                value.ToJSONStringInternal(sb);
                first = false;
              }
            }
            sb.Append('}');
            break;
          }
        default:
          sb.Append(this.ToJSONString());
          break;
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

    /// <summary>Divides a CBORObject object by the value of a CBORObject
    /// object.</summary>
    /// <returns>The quotient of the two objects.</returns>
    /// <param name='first'>A CBORObject object.</param>
    /// <param name='second'>A CBORObject object. (2).</param>
    public static CBORObject Divide(CBORObject first, CBORObject second) {
      return CBORObjectMath.Divide(first, second);
    }

    /// <summary>Finds the remainder that results when a CBORObject object
    /// is divided by the value of a CBORObject object.</summary>
    /// <returns>The remainder of the two objects.</returns>
    /// <param name='first'>A CBORObject object.</param>
    /// <param name='second'>A CBORObject object. (2).</param>
    public static CBORObject Remainder(CBORObject first, CBORObject second) {
      return CBORObjectMath.Remainder(first, second);
    }

    /// <summary>Creates a new empty CBOR array.</summary>
    /// <returns>A new CBOR array.</returns>
    public static CBORObject NewArray() {
      return new CBORObject(CBORObjectTypeArray, new List<CBORObject>());
    }

    /// <summary>Creates a new empty CBOR map.</summary>
    /// <returns>A new CBOR map.</returns>
    public static CBORObject NewMap() {
      return FromObject(new Dictionary<CBORObject, CBORObject>());
    }

    /// <summary>Creates a CBOR object from a simple value number.</summary>
    /// <param name='simpleValue'>A 32-bit signed integer.</param>
    /// <returns>A CBORObject object.</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='simpleValue'/> is less than 0, greater than 255, or from 24
    /// through 31.</exception>
    public static CBORObject FromSimpleValue(int simpleValue) {
      if (simpleValue < 0) {
        throw new ArgumentException("simpleValue (" + Convert.ToString((long)simpleValue, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (simpleValue > 255) {
        throw new ArgumentException("simpleValue (" + Convert.ToString((long)simpleValue, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + "255");
      }
      if (simpleValue >= 24 && simpleValue < 32) {
        throw new ArgumentException("Simple value is from 24 to 31: " + simpleValue);
      }
      if (simpleValue < 32) {
        return valueFixedObjects[0xe0 + simpleValue];
      } else {
        return new CBORObject(
          CBORObjectTypeSimpleValue,
          simpleValue);
      }
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
        return new CBORObject(CBORObjectTypeExtendedFloat, bigValue);
      }
    }

    /// <summary>Generates a CBOR object from a rational number.</summary>
    /// <param name='bigValue'>A rational number.</param>
    /// <returns>A CBOR number object.</returns>
    public static CBORObject FromObject(ExtendedRational bigValue) {
      if ((object)bigValue == (object)null) {
        return CBORObject.Null;
      }
      if (bigValue.IsFinite && bigValue.Denominator.Equals(BigInteger.One)) {
        return FromObject(bigValue.Numerator);
      }
      return new CBORObject(CBORObjectTypeExtendedRational, bigValue);
    }

    /// <summary>Generates a CBOR object from a decimal number.</summary>
    /// <param name='otherValue'>An arbitrary-precision decimal number.</param>
    /// <returns>A CBOR number object.</returns>
    public static CBORObject FromObject(ExtendedDecimal otherValue) {
      if ((object)otherValue == (object)null) {
        return CBORObject.Null;
      }
      if (otherValue.IsNaN() || otherValue.IsInfinity()) {
        return new CBORObject(CBORObjectTypeExtendedDecimal, otherValue);
      }
      BigInteger bigintExponent = otherValue.Exponent;
      if (bigintExponent.IsZero && !(otherValue.IsZero && otherValue.IsNegative)) {
        return FromObject(otherValue.Mantissa);
      } else {
        return new CBORObject(CBORObjectTypeExtendedDecimal, otherValue);
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
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='value'/> is a surrogate code point.</exception>
    public static CBORObject FromObject(char value) {
      return FromObject(new String(new char[] { value }));
    }

    /// <summary>Returns the CBOR true value or false value, depending on
    /// &quot;value&quot;.</summary>
    /// <returns>CBORObject.True if value is true; otherwise CBORObject.False.</returns>
    /// <param name='value'>Either True or False.</param>
    public static CBORObject FromObject(bool value) {
      return value ? CBORObject.True : CBORObject.False;
    }

    /// <summary>Generates a CBOR object from a byte (0 to 255).</summary>
    /// <returns>A CBORObject object.</returns>
    /// <param name='value'>A Byte object.</param>
    public static CBORObject FromObject(byte value) {
      return FromObject(((int)value) & 0xff);
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
    /// is copied to a new byte array. (This method can't be used to decode CBOR
    /// data from a byte array; for that, use the DecodeFromBytes method instead.).</summary>
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
        // Console.WriteLine(i);
        list.Add(FromObject(i));
      }
      return new CBORObject(CBORObjectTypeArray, list);
    }

    /// <summary>Generates a CBOR object from a list of objects.</summary>
    /// <param name='value'>An array of CBOR objects. Can be null.</param>
    /// <returns>A CBOR object where each element of the given array is converted
    /// to a CBOR object and copied to a new array, or CBORObject.Null if the
    /// value is null.</returns>
    /// <typeparam name='T'>A type convertible to CBORObject.</typeparam>
    public static CBORObject FromObject<T>(IList<T> value) {
      if (value == null) {
        return CBORObject.Null;
      }
      if (value.Count == 0) {
        return new CBORObject(CBORObjectTypeArray, new List<T>());
      }
      CBORObject retCbor = CBORObject.NewArray();
      foreach (T i in (IList<T>)value) {
        retCbor.Add(CBORObject.FromObject(i));
      }
      return retCbor;
    }

    /// <summary>Generates a CBOR object from an enumerable set of objects.</summary>
    /// <param name='value'>An object that implements the IEnumerable
    /// interface. In the .NET version, this can be the return value of an iterator
    /// or the result of a LINQ query.</param>
    /// <returns>A CBOR object where each element of the given enumerable
    /// object is converted to a CBOR object and copied to a new array, or CBORObject.Null
    /// if the value is null.</returns>
    /// <typeparam name='T'>A type convertible to CBORObject.</typeparam>
    public static CBORObject FromObject<T>(IEnumerable<T> value) {
      if (value == null) {
        return CBORObject.Null;
      }
      CBORObject retCbor = CBORObject.NewArray();
      foreach (T i in (IEnumerable<T>)value) {
        retCbor.Add(CBORObject.FromObject(i));
      }
      return retCbor;
    }

    /// <summary>Generates a CBOR object from a map of objects.</summary>
    /// <param name='dic'>A map of CBOR objects.</param>
    /// <returns>A CBOR object where each key and value of the given map is
    /// converted to a CBOR object and copied to a new map, or CBORObject.Null
    /// if <paramref name='dic'/> is null.</returns>
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

    /// <summary>Generates a CBORObject from an arbitrary object. The following
    /// types are specially handled by this method: <c>null</c>
    /// , primitive types, strings, <c>CBORObject</c>
    /// , <c>ExtendedDecimal</c>
    /// , <c>ExtendedFloat</c>
    /// , the custom <c>BigInteger</c>
    /// , lists, arrays, enumerations (<c>Enum</c>
    /// objects), and maps.<para>In the .NET version, if the object is a type
    /// not specially handled by this method, returns a CBOR map with the values
    /// of each of its read/write properties (or all properties in the case
    /// of an anonymous type). Properties are converted to their camel-case
    /// names (meaning if a name starts with A to Z, that letter is lower-cased).
    /// If the property name begins with the word "Is", that word is deleted
    /// from the name. Also, .NET <c>Enum</c>
    /// objects will be converted to their integer values, and a multidimensional
    /// array is converted to an array of arrays.</para>
    /// <para>In the Java version, if the object is a type not specially handled
    /// by this method, this method checks the CBOR object for methods starting
    /// with the word "get" or "is" that take no parameters, and returns a CBOR
    /// map with one entry for each such method found. For each method found,
    /// the starting word "get" or "is" is deleted from its name, and the name
    /// is converted to camel case (meaning if a name starts with A to Z, that
    /// letter is lower-cased). Also, Java <c>Enum</c>
    /// objects will be converted to the result of their <c>name</c>
    /// method.</para>
    /// </summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A CBOR object corresponding to the given object. Returns
    /// CBORObject.Null if the object is null.</returns>
    /// <exception cref='System.ArgumentException'>The object's type
    /// is not supported.</exception>
    public static CBORObject FromObject(object obj) {
      if (obj == null) {
        return CBORObject.Null;
      }
      if (obj is string) {
        return FromObject((string)obj);
      }
      if (obj is int) {
        return FromObject((int)obj);
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
      ExtendedRational rf = obj as ExtendedRational;
      if (rf != null) {
        return FromObject(rf);
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
      if (obj is Enum) {
        return FromObject(PropertyMap.EnumToObject((Enum)obj));
      }
      if (obj is double) {
        return FromObject((double)obj);
      }
      byte[] bytearr = obj as byte[];
      if (bytearr != null) {
        return FromObject(bytearr);
      }
      CBORObject objret;
      if (obj is System.Collections.IDictionary) {
        // IDictionary appears first because IDictionary includes IEnumerable
        objret = CBORObject.NewMap();
        System.Collections.IDictionary objdic = (System.Collections.IDictionary)obj;
        foreach (object key in (System.Collections.IDictionary)objdic) {
          objret[CBORObject.FromObject(key)] = CBORObject.FromObject(objdic[key]);
        }
        return objret;
      }
      if (obj is Array) {
        return PropertyMap.FromArray(obj);
      }
      if (obj is System.Collections.IEnumerable) {
        objret = CBORObject.NewArray();
        foreach (object element in (System.Collections.IEnumerable)obj) {
          objret.Add(CBORObject.FromObject(element));
        }
        return objret;
      }
      objret = ConvertWithConverter(obj);
      if (objret != null) {
        return objret;
      }
      objret = CBORObject.NewMap();
      foreach (KeyValuePair<string, object> key in PropertyMap.GetProperties(obj)) {
        objret[key.Key] = CBORObject.FromObject(key.Value);
      }
      return objret;
    }

    /// <summary>Generates a CBOR object from an arbitrary object and gives
    /// the resulting object a tag.</summary>
    /// <returns>A CBOR object where the object <paramref name='valueOb'/>
    /// is converted to a CBOR object and given the tag <paramref name='bigintTag'/>.</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='bigintTag'/> is less than 0 or greater than 2^64-1, or <paramref
    /// name='valueOb'/>'s type is unsupported.</exception>
    /// <param name='valueOb'>An arbitrary object. If the tag number is
    /// 2 or 3, this must be a byte string whose bytes represent an integer in
    /// little-endian byte order, and the value of the number is 1 minus the
    /// integer&apos;s value for tag 3. If the tag number is 4 or 5, this must
    /// be an array with two elements: the first must be an integer representing
    /// the exponent, and the second must be an integer representing a mantissa.</param>
    /// <param name='bigintTag'>Tag number. The tag number 55799 can be
    /// used to mark a &quot;self-described CBOR&quot; object.</param>
    public static CBORObject FromObjectAndTag(object valueOb, BigInteger bigintTag) {
      if (bigintTag == null) {
        throw new ArgumentNullException("bigintTag");
      }
      if (bigintTag.Sign < 0) {
        throw new ArgumentException("bigintTag's sign (" + Convert.ToString((long)bigintTag.Sign, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (bigintTag.CompareTo(UInt64MaxValue) > 0) {
        throw new ArgumentException("tag more than 18446744073709551615 (" + Convert.ToString(bigintTag, System.Globalization.CultureInfo.InvariantCulture) + ")");
      }
      CBORObject c = FromObject(valueOb);
      if (bigintTag.bitLength() <= 16) {
        // Low-numbered, commonly used tags
        return FromObjectAndTag(c, (int)bigintTag);
      } else {
        int tagLow = 0;
        int tagHigh = 0;
        byte[] bytes = bigintTag.ToByteArray();
        for (int i = 0; i < Math.Min(4, bytes.Length); ++i) {
          int b = ((int)bytes[i]) & 0xff;
          tagLow = unchecked(tagLow | (((int)b) << (i * 8)));
        }
        for (int i = 4; i < Math.Min(8, bytes.Length); ++i) {
          int b = ((int)bytes[i]) & 0xff;
          tagHigh = unchecked(tagHigh | (((int)b) << (i * 8)));
        }
        CBORObject c2 = new CBORObject(c, tagLow, tagHigh);
        ICBORTag tagconv = FindTagConverter(bigintTag);
        if (tagconv != null) {
          c2 = tagconv.ValidateObject(c2);
        }
        return c2;
      }
    }

    internal static ICBORTag FindTagConverter(int tag) {
      return FindTagConverter((BigInteger)tag);
    }

    internal static ICBORTag FindTagConverter(long tag) {
      return FindTagConverter((BigInteger)tag);
    }

    internal static ICBORTag FindTagConverter(BigInteger bigintTag) {
      if (TagHandlersEmpty()) {
        AddTagHandler((BigInteger)2, new CBORTag2());
        AddTagHandler((BigInteger)3, new CBORTag3());
        AddTagHandler((BigInteger)4, new CBORTag4());
        AddTagHandler((BigInteger)5, new CBORTag5());
        AddTagHandler((BigInteger)264, new CBORTag4(true));
        AddTagHandler((BigInteger)265, new CBORTag5(true));
        AddTagHandler((BigInteger)25, new CBORTagUnsigned());
        AddTagHandler((BigInteger)28, new CBORTag28());
        AddTagHandler((BigInteger)29, new CBORTagUnsigned());
        AddTagHandler((BigInteger)256, new CBORTagAny());
        AddTagHandler(BigInteger.Zero, new CBORTag0());
        AddTagHandler((BigInteger)32, new CBORTag32());
        AddTagHandler((BigInteger)33, new CBORTagGenericString());
        AddTagHandler((BigInteger)34, new CBORTagGenericString());
        AddTagHandler((BigInteger)35, new CBORTagGenericString());
        AddTagHandler((BigInteger)36, new CBORTagGenericString());
        AddTagHandler((BigInteger)37, new CBORTag37());
        AddTagHandler((BigInteger)30, new CBORTag30());
      }
      lock (tagHandlers) {
        if (tagHandlers.ContainsKey(bigintTag)) {
          return tagHandlers[bigintTag];
        }
        #if DEBUG
        if (bigintTag.Equals((BigInteger)2)) {
          throw new InvalidOperationException("Expected valid tag handler");
        }
        #endif
        return null;
      }
    }

    /// <summary>Generates a CBOR object from an arbitrary object and gives
    /// the resulting object a tag.</summary>
    /// <param name='valueObValue'>An arbitrary object. If the tag number
    /// is 2 or 3, this must be a byte string whose bytes represent an integer
    /// in little-endian byte order, and the value of the number is 1 minus
    /// the integer&apos;s value for tag 3. If the tag number is 4 or 5, this
    /// must be an array with two elements: the first must be an integer representing
    /// the exponent, and the second must be an integer representing a mantissa.</param>
    /// <param name='smallTag'>A 32-bit integer that specifies a tag number.
    /// The tag number 55799 can be used to mark a &quot;self-described CBOR&quot;
    /// object.</param>
    /// <returns>A CBOR object where the object <paramref name='valueObValue'/>
    /// is converted to a CBOR object and given the tag <paramref name='smallTag'/>.</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='smallTag'/> is less than 0 or <paramref name='valueObValue'/>
    /// 's type is unsupported.</exception>
    public static CBORObject FromObjectAndTag(object valueObValue, int smallTag) {
      if (smallTag < 0) {
        throw new ArgumentException("smallTag (" + Convert.ToString((long)smallTag, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      ICBORTag tagconv = FindTagConverter(smallTag);
      CBORObject c = FromObject(valueObValue);
      c = new CBORObject(c, smallTag, 0);
      if (tagconv != null) {
        return tagconv.ValidateObject(c);
      }
      return c;
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
          byte[] arrayToWrite = new byte[] { (byte)0xdb,
            (byte)((high >> 24) & 0xff),
            (byte)((high >> 16) & 0xff),
            (byte)((high >> 8) & 0xff),
            (byte)(high & 0xff),
            (byte)((low >> 24) & 0xff),
            (byte)((low >> 16) & 0xff),
            (byte)((low >> 8) & 0xff),
            (byte)(low & 0xff) };
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

    private static string ExtendedToString(ExtendedFloat ef) {
      if (ef.IsFinite && (ef.Exponent.CompareTo((BigInteger)1000) > 0 ||
                          ef.Exponent.CompareTo((BigInteger)(-1000)) < 0)) {
        // It can take very long to convert a number with a very high
        // or very low exponent to a decimal string, so do this instead
        return ef.Mantissa.ToString() + "p" + ef.Exponent.ToString();
      }
      return ef.ToString();
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
      } else if (type == CBORObjectTypeExtendedFloat) {
        simvalue = ExtendedToString((ExtendedFloat)this.ThisItem);
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
      } else {
        if (sb == null) {
          return this.ThisItem.ToString();
        }
        sb.Append(this.ThisItem.ToString());
      }
      if (this.IsTagged) {
        this.AppendClosingTags(sb);
      }
      return sb.ToString();
    }

    private static bool BigIntFits(BigInteger bigint) {
      return bigint.bitLength() <= 64;
    }
  }
}
