/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using PeterO;
using PeterO.Numbers;

// TODO: Consider making CBORTypeMapper and PODOptions obsolete
// TODO: Add ReadObject that combines Read and ToObject; similarly
// for ReadJSON, FromJSONString, FromJSONBytes
// TODO: Let certain methods support integers as map keys, such as Set/Add/Get
namespace PeterO.Cbor {
  /// <summary>
  /// <para>Represents an object in Concise Binary Object Representation
  /// (CBOR) and contains methods for reading and writing CBOR data. CBOR
  /// is an Internet Standard and defined in RFC 8949.</para></summary>
  /// <remarks>
  /// <para><b>Converting CBOR objects</b></para>
  /// <para>There are many ways to get a CBOR object, including from
  /// bytes, objects, streams and JSON, as described below.</para>
  /// <para><b>To and from byte arrays:</b> The
  /// CBORObject.DecodeFromBytes method converts a byte array in CBOR
  /// format to a CBOR object. The EncodeToBytes method converts a CBOR
  /// object to its corresponding byte array in CBOR format.</para>
  /// <para><b>To and from data streams:</b> The CBORObject.Write methods
  /// write many kinds of objects to a data stream, including numbers,
  /// CBOR objects, strings, and arrays of numbers and strings. The
  /// CBORObject.Read method reads a CBOR object from a data
  /// stream.</para>
  /// <para><b>To and from other objects:</b> The
  /// <c>CBORObject.From[Type]</c> method converts many kinds of objects
  /// to a CBOR object, including numbers, strings, and arrays and maps
  /// of numbers and strings. Methods like AsNumber and AsString convert
  /// a CBOR object to different types of object. The
  /// <c>CBORObject.ToObject</c> method converts a CBOR object to an
  /// object of a given type; for example, a CBOR array to a native
  /// <c>List</c> (or <c>ArrayList</c> in Java), or a CBOR integer to an
  /// <c>int</c> or <c>long</c>. Of these methods, the.NET versions of
  /// the methods <c>CBORObject.FromObject</c> and
  /// <c>CBORObject.ToObject</c> are not compatible with any context that
  /// disallows reflection, such as ahead-of-time compilation or
  /// self-contained app deployment.</para>
  /// <para><b>To and from JSON:</b> This class also doubles as a reader
  /// and writer of JavaScript Object Notation (JSON). The
  /// CBORObject.FromJSONString method converts JSON in text string form
  /// to a CBOR object, and the ToJSONString method converts a CBOR
  /// object to a JSON string. (Note that the conversion from CBOR to
  /// JSON is not always without loss and may make it impossible to
  /// recover the original object when converting the JSON back to CBOR.
  /// See the ToJSONString documentation.) Likewise, ToJSONBytes and
  /// FromJSONBytes work with JSON in the form of byte arrays rather than
  /// text strings.</para>
  /// <para>In addition, the CBORObject.WriteJSON method writes many
  /// kinds of objects as JSON to a data stream, including numbers, CBOR
  /// objects, strings, and arrays of numbers and strings. The
  /// CBORObject.Read method reads a CBOR object from a JSON data
  /// stream.</para>
  /// <para><b>Comparison Considerations:</b></para>
  /// <para>Instances of CBORObject should not be compared for equality
  /// using the "==" operator; it's possible to create two CBOR objects
  /// with the same value but not the same reference. (The "==" operator
  /// might only check if each side of the operator is the same
  /// instance.)</para>
  /// <para>This class's natural ordering (under the CompareTo method) is
  /// consistent with the Equals method, meaning that two values that
  /// compare as equal under the CompareTo method are also equal under
  /// the Equals method; this is a change in version 4.0. Two otherwise
  /// equal objects with different tags are not treated as equal by both
  /// CompareTo and Equals. To strip the tags from a CBOR object before
  /// comparing, use the <c>Untag</c> method.</para>
  /// <para><b>Thread Safety:</b></para>
  /// <para>Certain CBOR objects are immutable (their values can't be
  /// changed), so they are inherently safe for use by multiple
  /// threads.</para>
  /// <para>CBOR objects that are arrays, maps, and byte strings (whether
  /// or not they are tagged) are mutable, but this class doesn't attempt
  /// to synchronize reads and writes to those objects by multiple
  /// threads, so those objects are not thread safe without such
  /// synchronization.</para>
  /// <para>One kind of CBOR object is called a map, or a list of
  /// key-value pairs. Keys can be any kind of CBOR object, including
  /// numbers, strings, arrays, and maps. However, untagged text strings
  /// (which means GetTags returns an empty array and the Type property,
  /// or "getType()" in Java, returns TextString) are the most suitable
  /// to use as keys; other kinds of CBOR object are much better used as
  /// map values instead, keeping in mind that some of them are not
  /// thread safe without synchronizing reads and writes to them.</para>
  /// <para>To find the type of a CBOR object, call its Type property (or
  /// "getType()" in Java). The return value can be Integer,
  /// FloatingPoint, Boolean, SimpleValue, or TextString for immutable
  /// CBOR objects, and Array, Map, or ByteString for mutable CBOR
  /// objects.</para>
  /// <para><b>Nesting Depth:</b></para>
  /// <para>The DecodeFromBytes and Read methods can only read objects
  /// with a limited maximum depth of arrays and maps nested within other
  /// arrays and maps. The code sets this maximum depth to 500 (allowing
  /// more than enough nesting for most purposes), but it's possible that
  /// stack overflows in some runtimes might lower the effective maximum
  /// nesting depth. When the nesting depth goes above 500, the
  /// DecodeFromBytes and Read methods throw a CBORException.</para>
  /// <para>The ReadJSON and FromJSONString methods currently have
  /// nesting depths of 1000.</para></remarks>
  [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1036",
      Justification = "Mutable in some cases, and arbitrary size.")]
  public sealed partial class CBORObject : IComparable<CBORObject>,
    IEquatable<CBORObject>
  {
    private static CBORObject ConstructSimpleValue(int v) {
      return new CBORObject(CBORObjectTypeSimpleValue, v);
    }

    private static CBORObject ConstructIntegerValue(int v) {
      return new CBORObject(CBORObjectTypeInteger, (long)v);
    }

    /// <summary>Represents the value false.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Security",
        "CA2104",
        Justification = "This CBORObject is immutable")]
#endif
    public static readonly CBORObject False =
      CBORObject.ConstructSimpleValue(20);

    /// <summary>A not-a-number value.</summary>
    public static readonly CBORObject NaN = FromDouble(double.NaN);

    /// <summary>The value negative infinity.</summary>
    public static readonly CBORObject NegativeInfinity =
      FromDouble(double.NegativeInfinity);

    /// <summary>Represents the value null.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Security",
        "CA2104",
        Justification = "This CBORObject is immutable")]
#endif
    public static readonly CBORObject Null =
      CBORObject.ConstructSimpleValue(22);

    /// <summary>The value positive infinity.</summary>
    public static readonly CBORObject PositiveInfinity =
      FromDouble(double.PositiveInfinity);

    /// <summary>Represents the value true.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Security",
        "CA2104",
        Justification = "This CBORObject is immutable")]
#endif
    public static readonly CBORObject True =
      CBORObject.ConstructSimpleValue(21);

    /// <summary>Represents the value undefined.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Security",
        "CA2104",
        Justification = "This CBORObject is immutable")]
#endif
    public static readonly CBORObject Undefined =
      CBORObject.ConstructSimpleValue(23);

    /// <summary>Gets a CBOR object for the number zero.</summary>
    public static readonly CBORObject Zero =
      CBORObject.ConstructIntegerValue(0);

    private const int CBORObjectTypeInteger = 0; // -(2^63).. (2^63-1)
    private const int CBORObjectTypeEInteger = 1; // all other integers
    private const int CBORObjectTypeByteString = 2;
    private const int CBORObjectTypeTextString = 3;
    private const int CBORObjectTypeArray = 4;
    private const int CBORObjectTypeMap = 5;
    private const int CBORObjectTypeTagged = 6;
    private const int CBORObjectTypeSimpleValue = 7;
    private const int CBORObjectTypeDouble = 8;
    private const int CBORObjectTypeTextStringUtf8 = 9;
    private const int CBORObjectTypeTextStringAscii = 10;

    private const int StreamedStringBufferLength = 4096;

    private static readonly EInteger UInt64MaxValue =
      (EInteger.One << 64) - EInteger.One;

    private static readonly EInteger[] ValueEmptyTags = new EInteger[0];
    // Expected lengths for each head byte.
    // 0 means length varies. -1 means invalid.
    private static readonly int[] ValueExpectedLengths = {
      1, 1, 1, 1, 1, 1,
      1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, // major type 0
      1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 5, 9, -1, -1, -1, -1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // major type 1
      1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 5, 9, -1, -1, -1, -1,
      1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, // major type 2
      17, 18, 19, 20, 21, 22, 23, 24, 0, 0, 0, 0, -1, -1, -1, 0,
      1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, // major type 3
      17, 18, 19, 20, 21, 22, 23, 24, 0, 0, 0, 0, -1, -1, -1, 0,
      1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // major type 4
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, -1, -1, 0,
      1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // major type 5
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, -1, -1, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // major type 6
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, -1, -1, -1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // major type 7
      1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 5, 9, -1, -1, -1, -1,
    };

    private static readonly byte[] ValueFalseBytes = {
      0x66, 0x61, 0x6c,
      0x73, 0x65,
    };

    private static readonly byte[] ValueNullBytes = { 0x6e, 0x75, 0x6c, 0x6c };

    private static readonly byte[] ValueTrueBytes = { 0x74, 0x72, 0x75, 0x65 };

    private static readonly CBORObject[] FixedObjects =
      InitializeFixedObjects();

    private readonly int itemtypeValue;
    private readonly object itemValue;
    private readonly int tagHigh;
    private readonly int tagLow;

    internal CBORObject(CBORObject obj, int tagLow, int tagHigh) {
      this.itemtypeValue = CBORObjectTypeTagged;
      this.itemValue = obj;
      this.tagLow = tagLow;
      this.tagHigh = tagHigh;
    }

    internal CBORObject(int type, object item) {
#if DEBUG
      if (type == CBORObjectTypeDouble) {
        if (!(item is long)) {
          throw new ArgumentException("expected long for item type");
        }
      }
      // Check range in debug mode to ensure that Integer and EInteger
      // are unambiguous
      if ((type == CBORObjectTypeEInteger) &&
((EInteger)item).CanFitInInt64()) {
        throw new ArgumentException("arbitrary-precision integer is within" +
          "\u0020range for Integer");
      }
      if ((type == CBORObjectTypeEInteger) &&
        ((EInteger)item).GetSignedBitLengthAsInt64() > 64) {
        throw new ArgumentException("arbitrary-precision integer does not " +
          "fit major type 0 or 1");
      }
      if (type == CBORObjectTypeArray && !(item is IList<CBORObject>)) {
        throw new InvalidOperationException();
      }
#endif
      this.itemtypeValue = type;
      this.itemValue = item;
      this.tagLow = 0;
      this.tagHigh = 0;
    }

    /// <summary>Gets the number of keys in this map, or the number of
    /// items in this array, or 0 if this item is neither an array nor a
    /// map.</summary>
    /// <value>The number of keys in this map, or the number of items in
    /// this array, or 0 if this item is neither an array nor a
    /// map.</value>
    public int Count => (this.Type == CBORType.Array) ? this.AsList().Count :
          ((this.Type == CBORType.Map) ? this.AsMap().Count : 0);

    /// <summary>Gets the last defined tag for this CBOR data item, or -1
    /// if the item is untagged.</summary>
    /// <value>The last defined tag for this CBOR data item, or -1 if the
    /// item is untagged.</value>
    public EInteger MostInnerTag
    {
      get
      {
        if (!this.IsTagged) {
          return EInteger.FromInt32(-1);
        }
        CBORObject previtem = this;
        var curitem = (CBORObject)this.itemValue;
        while (curitem.IsTagged) {
          previtem = curitem;
          curitem = (CBORObject)curitem.itemValue;
        }
        return previtem.tagHigh == 0 && previtem.tagLow >= 0 &&
          previtem.tagLow < 0x10000 ? (EInteger)previtem.tagLow :
          LowHighToEInteger(
            previtem.tagLow,
            previtem.tagHigh);
      }
    }

    /// <summary>Gets a value indicating whether this value is a CBOR false
    /// value, whether tagged or not.</summary>
    /// <value><c>true</c> if this value is a CBOR false value; otherwise,
    /// <c>false</c>.</value>
    public bool IsFalse => this.ItemType == CBORObjectTypeSimpleValue &&
(int)this.ThisItem
          == 20;

    /// <summary>Gets a value indicating whether this CBOR object is a CBOR
    /// null value, whether tagged or not.</summary>
    /// <value><c>true</c> if this value is a CBOR null value; otherwise,
    /// <c>false</c>.</value>
    public bool IsNull => this.ItemType == CBORObjectTypeSimpleValue &&
(int)this.ThisItem
          == 22;

    /// <summary>Gets a value indicating whether this data item has at
    /// least one tag.</summary>
    /// <value><c>true</c> if this data item has at least one tag;
    /// otherwise, <c>false</c>.</value>
    public bool IsTagged => this.itemtypeValue == CBORObjectTypeTagged;

    /// <summary>Gets a value indicating whether this value is a CBOR true
    /// value, whether tagged or not.</summary>
    /// <value><c>true</c> if this value is a CBOR true value; otherwise,
    /// <c>false</c>.</value>
    public bool IsTrue => this.ItemType == CBORObjectTypeSimpleValue &&
(int)this.ThisItem
          == 21;

    /// <summary>Gets a value indicating whether this value is a CBOR
    /// undefined value, whether tagged or not.</summary>
    /// <value><c>true</c> if this value is a CBOR undefined value;
    /// otherwise, <c>false</c>.</value>
    public bool IsUndefined => this.ItemType == CBORObjectTypeSimpleValue &&
(int)this.ThisItem
          == 23;

    /// <summary>Gets a collection of the keys of this CBOR object. In
    /// general, the order in which those keys occur is undefined unless
    /// this is a map created using the NewOrderedMap method.</summary>
    /// <value>A read-only collection of the keys of this CBOR object. To
    /// avoid potential problems, the calling code should not modify the
    /// CBOR map while iterating over the returned collection.</value>
    /// <exception cref='InvalidOperationException'>This object is not a
    /// map.</exception>
    public ICollection<CBORObject> Keys
    {
      get
      {
        if (this.Type == CBORType.Map) {
          IDictionary<CBORObject, CBORObject> dict = this.AsMap();
          return PropertyMap.ReadOnlyKeys(dict);
        }
        throw new InvalidOperationException("Not a map");
      }
    }

    /// <summary>Gets the outermost tag for this CBOR data item, or -1 if
    /// the item is untagged.</summary>
    /// <value>The outermost tag for this CBOR data item, or -1 if the item
    /// is untagged.</value>
    public EInteger MostOuterTag => !this.IsTagged ?
          EInteger.FromInt32(-1) : this.tagHigh == 0 &&
          this.tagLow >= 0 && this.tagLow < 0x10000 ?
          (EInteger)this.tagLow : LowHighToEInteger(
            this.tagLow,
            this.tagHigh);

    /// <summary>Gets the simple value ID of this CBOR object, or -1 if the
    /// object is not a simple value. In this method, objects with a CBOR
    /// type of Boolean or SimpleValue are simple values, whether they are
    /// tagged or not.</summary>
    /// <value>The simple value ID of this object if it's a simple value,
    /// or -1 if this object is not a simple value.</value>
    public int SimpleValue => (this.ItemType == CBORObjectTypeSimpleValue) ?
          ((int)this.ThisItem) : -1;

    /// <summary>Gets a value indicating whether this CBOR object stores a
    /// number (including infinity or a not-a-number or NaN value).
    /// Currently, this is true if this item is untagged and has a CBORType
    /// of Integer or FloatingPoint, or if this item has only one tag and
    /// that tag is 2, 3, 4, 5, 30, 264, 265, 268, 269, or 270 with the
    /// right data type.</summary>
    /// <value>A value indicating whether this CBOR object stores a
    /// number.</value>
    public bool IsNumber => CBORNumber.IsNumber(this);

    /// <summary>Gets the general data type of this CBOR object. This
    /// method disregards the tags this object has, if any.</summary>
    /// <value>The general data type of this CBOR object.</value>
    public CBORType Type
    {
      get
      {
        switch (this.ItemType) {
          case CBORObjectTypeInteger:
          case CBORObjectTypeEInteger:
            return CBORType.Integer;
          case CBORObjectTypeDouble:
            return CBORType.FloatingPoint;
          case CBORObjectTypeSimpleValue:
            return ((int)this.ThisItem == 21 || (int)this.ThisItem == 20) ?
              CBORType.Boolean : CBORType.SimpleValue;
          case CBORObjectTypeArray:
            return CBORType.Array;
          case CBORObjectTypeMap:
            return CBORType.Map;
          case CBORObjectTypeByteString:
            return CBORType.ByteString;
          case CBORObjectTypeTextString:
          case CBORObjectTypeTextStringUtf8:
          case CBORObjectTypeTextStringAscii:
            return CBORType.TextString;
          default: throw new InvalidOperationException("Unexpected data type");
        }
      }
    }

    /// <summary>Gets a collection of the key/value pairs stored in this
    /// CBOR object, if it's a map. Returns one entry for each key/value
    /// pair in the map. In general, the order in which those entries occur
    /// is undefined unless this is a map created using the NewOrderedMap
    /// method.</summary>
    /// <value>A collection of the key/value pairs stored in this CBOR map,
    /// as a read-only view of those pairs. To avoid potential problems,
    /// the calling code should not modify the CBOR map while iterating
    /// over the returned collection.</value>
    /// <exception cref='InvalidOperationException'>This object is not a
    /// map.</exception>
    public ICollection<KeyValuePair<CBORObject, CBORObject>> Entries
    {
      get
      {
        if (this.Type == CBORType.Map) {
          IDictionary<CBORObject, CBORObject> dict = this.AsMap();
          return PropertyMap.GetEntries(dict);
        }
        throw new InvalidOperationException("Not a map");
      }
    }

    /// <summary>Gets a collection of the values of this CBOR object, if
    /// it's a map or an array. If this object is a map, returns one value
    /// for each key in the map; in general, the order in which those keys
    /// occur is undefined unless this is a map created using the
    /// NewOrderedMap method. If this is an array, returns all the values
    /// of the array in the order they are listed. (This method can't be
    /// used to get the bytes in a CBOR byte string; for that, use the
    /// GetByteString method instead.).</summary>
    /// <value>A read-only collection of the values of this CBOR map or
    /// array. To avoid potential problems, the calling code should not
    /// modify the CBOR map or array while iterating over the returned
    /// collection.</value>
    /// <exception cref='InvalidOperationException'>This object is not a
    /// map or an array.</exception>
    public ICollection<CBORObject> Values
    {
      get
      {
        if (this.Type == CBORType.Map) {
          IDictionary<CBORObject, CBORObject> dict = this.AsMap();
          return PropertyMap.ReadOnlyValues(dict);
        }
        if (this.Type == CBORType.Array) {
          IList<CBORObject> list = this.AsList();
          return new
            System.Collections.ObjectModel.ReadOnlyCollection<CBORObject>(
              list);
        }
        throw new InvalidOperationException("Not a map or array");
      }
    }

    private int ItemType
    {
      get
      {
        CBORObject curobject = this;
        while (curobject.itemtypeValue == CBORObjectTypeTagged) {
          curobject = (CBORObject)curobject.itemValue;
        }
        return curobject.itemtypeValue;
      }
    }

    private object ThisItem
    {
      get
      {
        CBORObject curobject = this;
        while (curobject.itemtypeValue == CBORObjectTypeTagged) {
          curobject = (CBORObject)curobject.itemValue;
        }
        return curobject.itemValue;
      }
    }

    /// <summary>Gets the value of a CBOR object by integer index in this
    /// array or by integer key in this map.</summary>
    /// <param name='index'>Index starting at 0 of the element, or the
    /// integer key to this map. (If this is a map, the given index can be
    /// any 32-bit signed integer, even a negative one.).</param>
    /// <returns>The CBOR object referred to by index or key in this array
    /// or map. If this is a CBOR map, returns <c>null</c> (not
    /// <c>CBORObject.Null</c> ) if an item with the given key doesn't
    /// exist (but this behavior may change to throwing an exception in
    /// version 5.0 or later).</returns>
    /// <exception cref='InvalidOperationException'>This object is not an
    /// array or map.</exception>
    /// <exception cref='ArgumentException'>This object is an array and the
    /// index is less than 0 or at least the size of the array.</exception>
    /// <exception cref='ArgumentNullException'>The parameter "value" is
    /// null (as opposed to CBORObject.Null).</exception>
    public CBORObject this[int index]
    {
      get
      {
        if (this.Type == CBORType.Array) {
          IList<CBORObject> list = this.AsList();
          return index < 0 || index >= list.Count ? throw new
ArgumentOutOfRangeException(nameof(index)) : list[index];
        }
        if (this.Type == CBORType.Map) {
          IDictionary<CBORObject, CBORObject> map = this.AsMap();
          var key = CBORObject.FromInt32(index);
          // TODO: In next major version, consider throwing an exception
          // instead if key does not exist.
          return PropertyMap.GetOrDefault(map, key, null);
        }
        throw new InvalidOperationException("Not an array or map");
      }

      set {
        if (this.Type == CBORType.Array) {
          if (value == null) {
            throw new ArgumentNullException(nameof(value));
          }
          IList<CBORObject> list = this.AsList();
          if (index < 0 || index >= list.Count) {
            throw new ArgumentOutOfRangeException(nameof(index));
          }
          list[index] = value;
        } else if (this.Type == CBORType.Map) {
          IDictionary<CBORObject, CBORObject> map = this.AsMap();
          var key = CBORObject.FromInt32(index);
          map[key] = value;
        } else {
          throw new InvalidOperationException("Not an array or map");
        }
      }
    }

    /// <summary>Gets the value of a CBOR object by integer index in this
    /// array or by CBOR object key in this map, or a default value if that
    /// value is not found.</summary>
    /// <param name='cborkey'>An arbitrary CBORObject. If this is a CBOR
    /// map, this parameter is converted to a CBOR object serving as the
    /// key to the map or index to the array, and can be null. If this is a
    /// CBOR array, the key must be an integer 0 or greater and less than
    /// the size of the array, and may be any object convertible to a CBOR
    /// integer.</param>
    /// <param name='defaultValue'>A value to return if an item with the
    /// given key doesn't exist, or if the CBOR object is an array and the
    /// key is not an integer 0 or greater and less than the size of the
    /// array.</param>
    /// <returns>The CBOR object referred to by index or key in this array
    /// or map. If this is a CBOR map, returns <c>null</c> (not
    /// <c>CBORObject.Null</c> ) if an item with the given key doesn't
    /// exist.</returns>
    public CBORObject GetOrDefault(CBORObject cborkey, CBORObject
defaultValue) {
      if (this.Type == CBORType.Array) {
        if (!cborkey.IsNumber || !cborkey.AsNumber().CanFitInInt32()) {
          return defaultValue;
        }
        int index = cborkey.AsNumber().ToInt32Checked();
        IList<CBORObject> list = this.AsList();
        return (index < 0 || index >= list.Count) ? defaultValue :
          list[index];
      }
      if (this.Type == CBORType.Map) {
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        return PropertyMap.GetOrDefault(map, cborkey, defaultValue);
      }
      return defaultValue;
    }

    /// <summary>Gets the value of a CBOR object by integer index in this
    /// array, or a default value if that value is not found.</summary>
    /// <param name='key'>An arbitrary object. If this is a CBOR map, this
    /// parameter is converted to a CBOR object serving as the key to the
    /// map or index to the array, and can be null. If this is a CBOR
    /// array, the key must be an integer 0 or greater and less than the
    /// size of the array, and may be any object convertible to a CBOR
    /// integer.</param>
    /// <param name='defaultValue'>A value to return if an item with the
    /// given key doesn't exist, or if the CBOR object is an array and the
    /// key is not an integer 0 or greater and less than the size of the
    /// array.</param>
    /// <returns>The CBOR object referred to by index or key in this array
    /// or map. If this is a CBOR map, returns <c>null</c> (not
    /// <c>CBORObject.Null</c> ) if an item with the given key doesn't
    /// exist.</returns>
    public CBORObject GetOrDefault(int key, CBORObject defaultValue) {
      if (this.Type == CBORType.Array) {
        int index = key;
        IList<CBORObject> list = this.AsList();
        return (index < 0 || index >= list.Count) ? defaultValue :
          list[index];
      }
      if (this.Type == CBORType.Map) {
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        return PropertyMap.GetOrDefault(
          map,
          FromInt32(key),
          defaultValue);
      }
      return defaultValue;
    }

    /// <summary>Gets the value of a CBOR object by string key in a map, or
    /// a default value if that value is not found.</summary>
    /// <param name='key'>An arbitrary string. If this is a CBOR map, this
    /// parameter is converted to a CBOR object serving as the key to the
    /// map or index to the array, and can be null. If this is a CBOR
    /// array, defaultValue is returned.</param>
    /// <param name='defaultValue'>A value to return if an item with the
    /// given key doesn't exist, or if the CBOR object is an array.</param>
    /// <returns>The CBOR object referred to by index or key in this array
    /// or map. If this is a CBOR map, returns <c>null</c> (not
    /// <c>CBORObject.Null</c> ) if an item with the given key doesn't
    /// exist.</returns>
    public CBORObject GetOrDefault(string key, CBORObject defaultValue) {
      if (this.Type == CBORType.Array) {
        return defaultValue;
      }
      if (this.Type == CBORType.Map) {
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        return PropertyMap.GetOrDefault(
          map,
          FromString(key),
          defaultValue);
      }
      return defaultValue;
    }

    /// <summary>Gets the value of a CBOR object by integer index in this
    /// array or by CBOR object key in this map.</summary>
    /// <param name='key'>A CBOR object serving as the key to the map or
    /// index to the array. If this is a CBOR array, the key must be an
    /// integer 0 or greater and less than the size of the array.</param>
    /// <returns>The CBOR object referred to by index or key in this array
    /// or map. If this is a CBOR map, returns <c>null</c> (not
    /// <c>CBORObject.Null</c> ) if an item with the given key doesn't
    /// exist.</returns>
    /// <exception cref='ArgumentNullException'>The key is null (as opposed
    /// to CBORObject.Null); or the set method is called and the value is
    /// null.</exception>
    /// <exception cref='ArgumentException'>This CBOR object is an array
    /// and the key is not an integer 0 or greater and less than the size
    /// of the array.</exception>
    /// <exception cref='InvalidOperationException'>This object is not a
    /// map or an array.</exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1043",
        Justification = "Represents a logical data store")]
    public CBORObject this[CBORObject key]
    {
      get
      {
        /* "The CBORObject class represents a logical data store." +
        " Also, an Object indexer is not included here because it's unusual
        for " +
        "CBOR map keys to be anything other than text strings or integers; " +
        "including an Object indexer would introduce the security issues
        present in the FromObject method because of the need to convert to
        CBORObject;" +
        " and this CBORObject indexer is included here because any CBOR
        object " +
        "can serve as a map key, not just integers or text strings." */
        if (key == null) {
          throw new ArgumentNullException(nameof(key));
        }
        if (this.Type == CBORType.Map) {
          IDictionary<CBORObject, CBORObject> map = this.AsMap();
          return PropertyMap.GetOrDefault(map, key, null);
        }
        if (this.Type == CBORType.Array) {
          if (!key.IsNumber || !key.AsNumber().IsInteger()) {
            throw new ArgumentException("Not an integer");
          }
          if (!key.AsNumber().CanFitInInt32()) {
            throw new ArgumentOutOfRangeException(nameof(key));
          }
          IList<CBORObject> list = this.AsList();
          int index = key.AsNumber().ToInt32Checked();
          return index < 0 || index >= list.Count ? throw new
ArgumentOutOfRangeException(nameof(key)) : list[index];
        }
        throw new InvalidOperationException("Not an array or map");
      }

      set {
        if (key == null) {
          throw new ArgumentNullException(nameof(key));
        }
        if (value == null) {
          throw new ArgumentNullException(nameof(value));
        }
        if (this.Type == CBORType.Map) {
          IDictionary<CBORObject, CBORObject> map = this.AsMap();
          map[key] = value;
          return;
        }
        if (this.Type == CBORType.Array) {
          if (!key.IsNumber || !key.AsNumber().IsInteger()) {
            throw new ArgumentException("Not an integer");
          }
          if (!key.AsNumber().CanFitInInt32()) {
            throw new ArgumentOutOfRangeException(nameof(key));
          }
          IList<CBORObject> list = this.AsList();
          int index = key.AsNumber().ToInt32Checked();
          if (index < 0 || index >= list.Count) {
            throw new ArgumentOutOfRangeException(nameof(key));
          }
          list[index] = value;
          return;
        }
        throw new InvalidOperationException("Not an array or map");
      }
    }

    /// <summary>Gets the value of a CBOR object in this map, using a
    /// string as the key.</summary>
    /// <param name='key'>A key that points to the desired value.</param>
    /// <returns>The CBOR object referred to by key in this map. Returns
    /// <c>null</c> if an item with the given key doesn't exist.</returns>
    /// <exception cref='ArgumentNullException'>The key is
    /// null.</exception>
    /// <exception cref='InvalidOperationException'>This object is not a
    /// map.</exception>
    public CBORObject this[string key]
    {
      get
      {
        if (key == null) {
          throw new ArgumentNullException(nameof(key));
        }
        return this[FromString(key)];
      }

      set {
        if (key == null) {
          throw new ArgumentNullException(nameof(key));
        }
        if (value == null) {
          throw new ArgumentNullException(nameof(value));
        }
        if (this.Type == CBORType.Map) {
          IDictionary<CBORObject, CBORObject> map = this.AsMap();
          CBORObject objkey = FromString(key);
          map[objkey] = value;
        } else {
          throw new InvalidOperationException("Not a map");
        }
      }
    }

    /// <summary>
    /// <para>Generates a CBOR object from an array of CBOR-encoded
    /// bytes.</para></summary>
    /// <param name='data'>A byte array in which a single CBOR object is
    /// encoded.</param>
    /// <returns>A CBOR object decoded from the given byte array.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data. This includes cases where not all of
    /// the byte array represents a CBOR object. This exception is also
    /// thrown if the parameter <paramref name='data'/> is
    /// empty.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='data'/> is null.</exception>
    public static CBORObject DecodeFromBytes(byte[] data) {
      return DecodeFromBytes(data, CBOREncodeOptions.Default);
    }

    private static readonly CBOREncodeOptions AllowEmptyOptions =
      new CBOREncodeOptions("allowempty=1");

    /// <summary>
    /// <para>Generates a sequence of CBOR objects from an array of
    /// CBOR-encoded bytes.</para></summary>
    /// <param name='data'>A byte array in which any number of CBOR objects
    /// (including zero) are encoded, one after the other. Can be empty,
    /// but cannot be null.</param>
    /// <returns>An array of CBOR objects decoded from the given byte
    /// array. Returns an empty array if <paramref name='data'/> is
    /// empty.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data. This includes cases where the last
    /// CBOR object in the data was read only partly.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='data'/> is null.</exception>
    public static CBORObject[] DecodeSequenceFromBytes(byte[] data) {
      return DecodeSequenceFromBytes(data, AllowEmptyOptions);
    }

    /// <summary>
    /// <para>Generates a sequence of CBOR objects from an array of
    /// CBOR-encoded bytes.</para></summary>
    /// <param name='data'>A byte array in which any number of CBOR objects
    /// (including zero) are encoded, one after the other. Can be empty,
    /// but cannot be null.</param>
    /// <param name='options'>Specifies options to control how the CBOR
    /// object is decoded. See
    /// <see cref='PeterO.Cbor.CBOREncodeOptions'/> for more information.
    /// In this method, the AllowEmpty property is treated as always set
    /// regardless of that value as specified in this parameter.</param>
    /// <returns>An array of CBOR objects decoded from the given byte
    /// array. Returns an empty array if <paramref name='data'/> is
    /// empty.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data. This includes cases where the last
    /// CBOR object in the data was read only partly.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='data'/> is null, or the parameter <paramref name='options'/>
    /// is null.</exception>
    public static CBORObject[] DecodeSequenceFromBytes(byte[] data,
      CBOREncodeOptions options) {
      if (data == null) {
        throw new ArgumentNullException(nameof(data));
      }
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      if (data.Length == 0) {
        return new CBORObject[0];
      }
      CBOREncodeOptions opt = options;
      if (!opt.AllowEmpty) {
        opt = new CBOREncodeOptions(opt.ToString() + ";allowempty=1");
      }
      var cborList = new List<CBORObject>();
      using (var ms = new MemoryStream(data)) {
        while (true) {
          CBORObject obj = Read(ms, opt);
          if (obj == null) {
            break;
          }
          cborList.Add(obj);
        }
      }
      return cborList.ToArray();
    }

    /// <summary>Generates a list of CBOR objects from an array of bytes in
    /// JavaScript Object Notation (JSON) text sequence format (RFC 7464).
    /// The byte array must be in UTF-8 encoding and may not begin with a
    /// byte-order mark (U+FEFF).</summary>
    /// <param name='bytes'>A byte array in which a JSON text sequence is
    /// encoded.</param>
    /// <returns>A list of CBOR objects read from the JSON sequence.
    /// Objects that could not be parsed are replaced with <c>null</c> (as
    /// opposed to <c>CBORObject.Null</c> ) in the given list.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The byte array is not
    /// empty and does not begin with a record separator byte (0x1e), or an
    /// I/O error occurred.</exception>
    /// <remarks>Generally, each JSON text in a JSON text sequence is
    /// written as follows: Write a record separator byte (0x1e), then
    /// write the JSON text in UTF-8 (without a byte order mark, U+FEFF),
    /// then write the line feed byte (0x0a). RFC 7464, however, uses a
    /// more liberal syntax for parsing JSON text sequences.</remarks>
    public static CBORObject[] FromJSONSequenceBytes(byte[] bytes) {
      return FromJSONSequenceBytes(bytes, JSONOptions.Default);
    }

    /// <summary>Converts this object to a byte array in JavaScript Object
    /// Notation (JSON) format. The JSON text will be written out in UTF-8
    /// encoding, without a byte order mark, to the byte array. See the
    /// overload to ToJSONString taking a JSONOptions argument for further
    /// information.</summary>
    /// <returns>A byte array containing the converted in JSON
    /// format.</returns>
    public byte[] ToJSONBytes() {
      return this.ToJSONBytes(JSONOptions.Default);
    }

    /// <summary>Converts this object to a byte array in JavaScript Object
    /// Notation (JSON) format. The JSON text will be written out in UTF-8
    /// encoding, without a byte order mark, to the byte array. See the
    /// overload to ToJSONString taking a JSONOptions argument for further
    /// information.</summary>
    /// <param name='jsonoptions'>Specifies options to control writing the
    /// CBOR object to JSON.</param>
    /// <returns>A byte array containing the converted object in JSON
    /// format.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='jsonoptions'/> is null.</exception>
    public byte[] ToJSONBytes(JSONOptions jsonoptions) {
      if (jsonoptions == null) {
        throw new ArgumentNullException(nameof(jsonoptions));
      }
      try {
        using (var ms = new MemoryStream()) {
          this.WriteJSONTo(ms);
          return ms.ToArray();
        }
      } catch (IOException ex) {
        throw new CBORException(ex.Message, ex);
      }
    }

    /// <summary>Generates a list of CBOR objects from an array of bytes in
    /// JavaScript Object Notation (JSON) text sequence format (RFC 7464),
    /// using the specified options to control the decoding process. The
    /// byte array must be in UTF-8 encoding and may not begin with a
    /// byte-order mark (U+FEFF).</summary>
    /// <param name='data'>A byte array in which a JSON text sequence is
    /// encoded.</param>
    /// <param name='options'>Specifies options to control the JSON
    /// decoding process.</param>
    /// <returns>A list of CBOR objects read from the JSON sequence.
    /// Objects that could not be parsed are replaced with <c>null</c> (as
    /// opposed to <c>CBORObject.Null</c> ) in the given list.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='data'/> is null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The byte array is not
    /// empty and does not begin with a record separator byte (0x1e), or an
    /// I/O error occurred.</exception>
    /// <remarks>Generally, each JSON text in a JSON text sequence is
    /// written as follows: Write a record separator byte (0x1e), then
    /// write the JSON text in UTF-8 (without a byte order mark, U+FEFF),
    /// then write the line feed byte (0x0a). RFC 7464, however, uses a
    /// more liberal syntax for parsing JSON text sequences.</remarks>
    public static CBORObject[] FromJSONSequenceBytes(byte[] data,
      JSONOptions options) {
      if (data == null) {
        throw new ArgumentNullException(nameof(data));
      }
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      try {
        using (var ms = new MemoryStream(data)) {
          return ReadJSONSequence(ms, options);
        }
      } catch (IOException ex) {
        throw new CBORException(ex.Message, ex);
      }
    }

    /// <summary>Generates a CBOR object from an array of CBOR-encoded
    /// bytes, using the given <c>CBOREncodeOptions</c>
    ///  object to control
    /// the decoding process.</summary>
    /// <param name='data'>A byte array in which a single CBOR object is
    /// encoded.</param>
    /// <param name='options'>Specifies options to control how the CBOR
    /// object is decoded. See <see cref='PeterO.Cbor.CBOREncodeOptions'/>
    /// for more information.</param>
    /// <returns>A CBOR object decoded from the given byte array. Returns
    /// null (as opposed to CBORObject.Null) if <paramref name='data'/> is
    /// empty and the AllowEmpty property is set on the given options
    /// object.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data. This includes cases where not all of
    /// the byte array represents a CBOR object. This exception is also
    /// thrown if the parameter <paramref name='data'/> is empty unless the
    /// AllowEmpty property is set on the given options object.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='data'/> is null, or the parameter <paramref name='options'/>
    /// is null.</exception>
    /// <example>
    /// <para>The following example (originally written in C# for the.NET
    /// version) implements a method that decodes a text string from a CBOR
    /// byte array. It's successful only if the CBOR object contains an
    /// untagged text string.</para>
    /// <code>private static String DecodeTextString(byte[] bytes) { if (bytes ==
    /// null) { throw new ArgumentNullException(nameof(mapObj));}
    /// if
    /// (bytes.Length == 0 || bytes[0]&lt;0x60 || bytes[0]&gt;0x7f) {throw new
    /// CBORException();} return CBORObject.DecodeFromBytes(bytes,
    /// CBOREncodeOptions.Default).AsString(); }</code>
    ///  .
    /// </example>
    public static CBORObject DecodeFromBytes(
      byte[] data,
      CBOREncodeOptions options) {
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      if (data == null) {
        throw new ArgumentNullException(nameof(data));
      }
      if (data.Length == 0) {
        return options.AllowEmpty ? (CBORObject)null : throw new
CBORException("data is empty.");
      }
      int firstbyte = data[0] & 0xff;
      int expectedLength = ValueExpectedLengths[firstbyte];
      // if invalid
      if (expectedLength == -1) {
        throw new CBORException("Unexpected data encountered");
      }
      if (expectedLength != 0) {
        // if fixed length
        CheckCBORLength(expectedLength, data.Length);
        if (!options.Ctap2Canonical ||
          (firstbyte >= 0x00 && firstbyte < 0x18) ||
          (firstbyte >= 0x20 && firstbyte < 0x38)) {
          return GetFixedLengthObject(firstbyte, data);
        }
      }
      if (firstbyte == 0xc0 && !options.Ctap2Canonical) {
        // value with tag 0
        string s = GetOptimizedStringIfShortAscii(data, 1);
        if (s != null) {
          return new CBORObject(FromString(s), 0, 0);
        }
      }
      // For objects with variable length,
      // read the object as though
      // the byte array were a stream
      using (var ms = new MemoryStream(data)) {
        CBORObject o = Read(ms, options);
        CheckCBORLength(
          data.Length,
          ms.Position);
        return o;
      }
    }

    /// <summary>
    /// <para>Generates a CBOR object from a text string in JavaScript
    /// Object Notation (JSON) format.</para>
    /// <para>If a JSON object has duplicate keys, a CBORException is
    /// thrown. This is a change in version 4.0.</para>
    /// <para>Note that if a CBOR object is converted to JSON with
    /// <c>ToJSONString</c>, then the JSON is converted back to CBOR with
    /// this method, the new CBOR object will not necessarily be the same
    /// as the old CBOR object, especially if the old CBOR object uses data
    /// types not supported in JSON, such as integers in map
    /// keys.</para></summary>
    /// <param name='str'>A text string in JSON format. The entire string
    /// must contain a single JSON object and not multiple objects. The
    /// string may not begin with a byte-order mark (U+FEFF).</param>
    /// <param name='offset'>An index, starting at 0, showing where the
    /// desired portion of <paramref name='str'/> begins.</param>
    /// <param name='count'>The length, in code units, of the desired
    /// portion of <paramref name='str'/> (but not more than <paramref
    /// name='str'/> 's length).</param>
    /// <returns>A CBOR object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The string is not in
    /// JSON format.</exception>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='count'/> is less than 0 or
    /// greater than <paramref name='str'/> 's length, or <paramref
    /// name='str'/> 's length minus <paramref name='offset'/> is less than
    /// <paramref name='count'/>.</exception>
    public static CBORObject FromJSONString(string str, int offset, int count) {
      return str == null ? throw new ArgumentNullException(nameof(str)) :
FromJSONString(str, offset, count, JSONOptions.Default);
    }

    /// <summary>Generates a CBOR object from a text string in JavaScript
    /// Object Notation (JSON) format, using the specified options to
    /// control the decoding process.
    /// <para>Note that if a CBOR object is converted to JSON with
    /// <c>ToJSONString</c>, then the JSON is converted back to CBOR with
    /// this method, the new CBOR object will not necessarily be the same
    /// as the old CBOR object, especially if the old CBOR object uses data
    /// types not supported in JSON, such as integers in map
    /// keys.</para></summary>
    /// <param name='str'>A text string in JSON format. The entire string
    /// must contain a single JSON object and not multiple objects. The
    /// string may not begin with a byte-order mark (U+FEFF).</param>
    /// <param name='jsonoptions'>Specifies options to control the JSON
    /// decoding process.</param>
    /// <returns>A CBOR object containing the JSON data decoded.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> or <paramref name='jsonoptions'/> is null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The string is not in
    /// JSON format.</exception>
    public static CBORObject FromJSONString(
      string str,
      JSONOptions jsonoptions) {
      return str == null ?
        throw new ArgumentNullException(nameof(str)) :
        jsonoptions == null ? throw new
          ArgumentNullException(nameof(jsonoptions)) :
        FromJSONString(str, 0, str.Length, jsonoptions);
    }

    /// <summary>
    /// <para>Generates a CBOR object from a text string in JavaScript
    /// Object Notation (JSON) format.</para>
    /// <para>If a JSON object has duplicate keys, a CBORException is
    /// thrown. This is a change in version 4.0.</para>
    /// <para>Note that if a CBOR object is converted to JSON with
    /// <c>ToJSONString</c>, then the JSON is converted back to CBOR with
    /// this method, the new CBOR object will not necessarily be the same
    /// as the old CBOR object, especially if the old CBOR object uses data
    /// types not supported in JSON, such as integers in map
    /// keys.</para></summary>
    /// <param name='str'>A text string in JSON format. The entire string
    /// must contain a single JSON object and not multiple objects. The
    /// string may not begin with a byte-order mark (U+FEFF).</param>
    /// <returns>A CBOR object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The string is not in
    /// JSON format.</exception>
    public static CBORObject FromJSONString(string str) {
      return FromJSONString(str, JSONOptions.Default);
    }

    /// <summary>Generates a CBOR object from a text string in JavaScript
    /// Object Notation (JSON) format, using the specified options to
    /// control the decoding process.
    /// <para>Note that if a CBOR object is converted to JSON with
    /// <c>ToJSONString</c>, then the JSON is converted back to CBOR with
    /// this method, the new CBOR object will not necessarily be the same
    /// as the old CBOR object, especially if the old CBOR object uses data
    /// types not supported in JSON, such as integers in map
    /// keys.</para></summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='offset'>An index, starting at 0, showing where the
    /// desired portion of <paramref name='str'/> begins.</param>
    /// <param name='count'>The length, in code units, of the desired
    /// portion of <paramref name='str'/> (but not more than <paramref
    /// name='str'/> 's length).</param>
    /// <param name='jsonoptions'>The parameter <paramref
    /// name='jsonoptions'/> is a Cbor.JSONOptions object.</param>
    /// <returns>A CBOR object containing the JSON data decoded.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> or <paramref name='jsonoptions'/> is null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The string is not in
    /// JSON format.</exception>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='count'/> is less than 0 or
    /// greater than <paramref name='str'/> 's length, or <paramref
    /// name='str'/> 's length minus <paramref name='offset'/> is less than
    /// <paramref name='count'/>.</exception>
    public static CBORObject FromJSONString(
      string str,
      int offset,
      int count,
      JSONOptions jsonoptions) {
      return str == null ?
        throw new ArgumentNullException(nameof(str)) :
        jsonoptions == null ?
        throw new ArgumentNullException(nameof(jsonoptions)) :
        count > 0 && str[offset] == 0xfeff ? throw new CBORException(
          "JSON object began with a byte order mark (U+FEFF) (offset 0)") :
        count == 0 ? throw new CBORException("String is empty") :
CBORJson3.ParseJSONValue(str, offset, offset + count, jsonoptions);
    }

    /// <summary>Converts this CBOR object to an object of an arbitrary
    /// type. See the documentation for the overload of this method taking
    /// a CBORTypeMapper parameter for more information. This method
    /// doesn't use a CBORTypeMapper parameter to restrict which data types
    /// are eligible for Plain-Old-Data serialization.</summary>
    /// <param name='t'>The type, class, or interface that this method's
    /// return value will belong to. To express a generic type in Java, see
    /// the example. <b>Note:</b>
    ///  For security reasons, an application
    /// should not base this parameter on user input or other externally
    /// supplied data. Whenever possible, this parameter should be either a
    /// type specially handled by this method (such as <c>int</c>
    ///  or
    /// <c>String</c>
    ///  ) or a plain-old-data type (POCO or POJO type) within
    /// the control of the application. If the plain-old-data type
    /// references other data types, those types should likewise meet
    /// either criterion above.</param>
    /// <returns>The converted object.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>The given type
    /// <paramref name='t'/> , or this object's CBOR type, is not
    /// supported, or the given object's nesting is too deep, or another
    /// error occurred when serializing the object.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='t'/> is null.</exception>
    /// <example>
    /// <para>Java offers no easy way to express a generic type, at least
    /// none as easy as C#'s <c>typeof</c>
    ///  operator. The following example,
    /// written in Java, is a way to specify that the return value will be
    /// an ArrayList of String objects.</para>
    /// <code>Type arrayListString = new ParameterizedType() { public Type[]
    /// getActualTypeArguments() { &#x2f;&#x2a; Contains one type parameter,
    /// String&#x2a;&#x2f;
    /// return new Type[] { String.class }; }
    /// public Type getRawType() { /* Raw type is
    /// ArrayList */ return ArrayList.class; }
    /// public Type getOwnerType() {
    /// return null; } };
    /// ArrayList&lt;String&gt; array = (ArrayList&lt;String&gt;)
    /// cborArray.ToObject(arrayListString);</code>
    /// <para>By comparison, the C# version is much shorter.</para>
    /// <code>var array = (List&lt;String&gt;)cborArray.ToObject(
    /// typeof(List&lt;String&gt;));</code>
    ///  .
    /// </example>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public object ToObject(Type t) {
      return this.ToObject(t, null, null, 0);
    }

    /// <summary>Converts this CBOR object to an object of an arbitrary
    /// type. See the documentation for the overload of this method taking
    /// a CBORTypeMapper and PODOptions parameters parameters for more
    /// information.</summary>
    /// <param name='t'>The type, class, or interface that this method's
    /// return value will belong to. To express a generic type in Java, see
    /// the example. <b>Note:</b> For security reasons, an application
    /// should not base this parameter on user input or other externally
    /// supplied data. Whenever possible, this parameter should be either a
    /// type specially handled by this method (such as <c>int</c> or
    /// <c>String</c> ) or a plain-old-data type (POCO or POJO type) within
    /// the control of the application. If the plain-old-data type
    /// references other data types, those types should likewise meet
    /// either criterion above.</param>
    /// <param name='mapper'>This parameter controls which data types are
    /// eligible for Plain-Old-Data deserialization and includes custom
    /// converters from CBOR objects to certain data types.</param>
    /// <returns>The converted object.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>The given type
    /// <paramref name='t'/>, or this object's CBOR type, is not
    /// supported, or the given object's nesting is too deep, or another
    /// error occurred when serializing the object.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='t'/> is null.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public object ToObject(Type t, CBORTypeMapper mapper) {
      return mapper == null ? throw new
ArgumentNullException(nameof(mapper)) : this.ToObject(t, mapper, null, 0);
    }

    /// <summary>Converts this CBOR object to an object of an arbitrary
    /// type. See the documentation for the overload of this method taking
    /// a CBORTypeMapper and PODOptions parameters for more information.
    /// This method (without a CBORTypeMapper parameter) allows all data
    /// types not otherwise handled to be eligible for Plain-Old-Data
    /// serialization.</summary>
    /// <param name='t'>The type, class, or interface that this method's
    /// return value will belong to. To express a generic type in Java, see
    /// the example. <b>Note:</b> For security reasons, an application
    /// should not base this parameter on user input or other externally
    /// supplied data. Whenever possible, this parameter should be either a
    /// type specially handled by this method (such as <c>int</c> or
    /// <c>String</c> ) or a plain-old-data type (POCO or POJO type) within
    /// the control of the application. If the plain-old-data type
    /// references other data types, those types should likewise meet
    /// either criterion above.</param>
    /// <param name='options'>Specifies options for controlling
    /// deserialization of CBOR objects.</param>
    /// <returns>The converted object.</returns>
    /// <exception cref='NotSupportedException'>The given type <paramref
    /// name='t'/>, or this object's CBOR type, is not
    /// supported.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='t'/> is null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The given object's
    /// nesting is too deep, or another error occurred when serializing the
    /// object.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public object ToObject(Type t, PODOptions options) {
      return options == null ? throw new
ArgumentNullException(nameof(options)) : this.ToObject(t, null, options, 0);
    }

    /// <summary><para>Converts this CBOR object to an object of an
    /// arbitrary type. The following cases are checked in the logical
    /// order given (rather than the strict order in which they are
    /// implemented by this library):</para>
    ///  <list><item>If the type is
    /// <c>CBORObject</c>
    ///  , return this object.</item>
    ///  <item>If the given
    /// object is <c>CBORObject.Null</c>
    ///  (with or without tags), returns
    /// <c>null</c>
    ///  .</item>
    ///  <item>If the object is of a type corresponding
    /// to a type converter mentioned in the <paramref name='mapper'/>
    /// parameter, that converter will be used to convert the CBOR object
    /// to an object of the given type. Type converters can be used to
    /// override the default conversion behavior of almost any
    /// object.</item>
    ///  <item>If the type is <c>object</c>
    ///  , return this
    /// object.</item>
    ///  <item>If the type is <c>char</c>
    ///  , converts
    /// single-character CBOR text strings and CBOR integers from 0 through
    /// 65535 to a <c>char</c>
    ///  object and returns that <c>char</c>
    /// object.</item>
    ///  <item>If the type is <c>bool</c>
    ///  ( <c>boolean</c>
    ///  in
    /// Java), returns the result of AsBoolean.</item>
    ///  <item>If the type is
    /// <c>short</c>
    ///  , returns this number as a 16-bit signed integer after
    /// converting its value to an integer by discarding its fractional
    /// part, and throws an exception if this object's value is infinity or
    /// a not-a-number value, or does not represent a number (currently
    /// InvalidOperationException, but may change in the next major
    /// version), or if the value, once converted to an integer by
    /// discarding its fractional part, is less than -32768 or greater than
    /// 32767 (currently OverflowException, but may change in the next
    /// major version).</item>
    ///  <item>If the type is <c>long</c>
    ///  , returns
    /// this number as a 64-bit signed integer after converting its value
    /// to an integer by discarding its fractional part, and throws an
    /// exception if this object's value is infinity or a not-a-number
    /// value, or does not represent a number (currently
    /// InvalidOperationException, but may change in the next major
    /// version), or if the value, once converted to an integer by
    /// discarding its fractional part, is less than -2^63 or greater than
    /// 2^63-1 (currently OverflowException, but may change in the next
    /// major version).</item>
    ///  <item>If the type is <c>short</c>
    ///  , the same
    /// rules as for <c>long</c>
    ///  are used, but the range is from -32768
    /// through 32767 and the return type is <c>short</c>
    ///  .</item>
    ///  <item>If
    /// the type is <c>byte</c>
    ///  , the same rules as for <c>long</c>
    ///  are
    /// used, but the range is from 0 through 255 and the return type is
    /// <c>byte</c>
    ///  .</item>
    ///  <item>If the type is <c>sbyte</c>
    ///  , the same
    /// rules as for <c>long</c>
    ///  are used, but the range is from -128
    /// through 127 and the return type is <c>sbyte</c>
    ///  .</item>
    ///  <item>If
    /// the type is <c>ushort</c>
    ///  , the same rules as for <c>long</c>
    ///  are
    /// used, but the range is from 0 through 65535 and the return type is
    /// <c>ushort</c>
    ///  .</item>
    ///  <item>If the type is <c>uint</c>
    ///  , the same
    /// rules as for <c>long</c>
    ///  are used, but the range is from 0 through
    /// 2^31-1 and the return type is <c>uint</c>
    ///  .</item>
    ///  <item>If the
    /// type is <c>ulong</c>
    ///  , the same rules as for <c>long</c>
    ///  are used,
    /// but the range is from 0 through 2^63-1 and the return type is
    /// <c>ulong</c>
    ///  .</item>
    ///  <item>If the type is <c>int</c>
    ///  or a
    /// primitive floating-point type ( <c>float</c>
    ///  , <c>double</c>
    ///  , as
    /// well as <c>decimal</c>
    ///  in.NET), returns the result of the
    /// corresponding As* method.</item>
    ///  <item>If the type is <c>String</c>
    /// , returns the result of AsString.</item>
    ///  <item>If the type is
    /// <c>EFloat</c>
    ///  , <c>EDecimal</c>
    ///  , <c>EInteger</c>
    ///  , or
    /// <c>ERational</c>
    ///  in the <a
    /// href='https://www.nuget.org/packages/PeterO.Numbers'><c>PeterO.Numbers</c>
    /// </a>
    ///  library (in .NET) or the <a
    /// href='https://github.com/peteroupc/numbers-java'><c>com.github.peteroupc/numbers</c>
    /// </a>
    ///  artifact (in Java), or if the type is <c>BigInteger</c>
    ///  or
    /// <c>BigDecimal</c>
    ///  in the Java version, converts the given object to
    /// a number of the corresponding type and throws an exception
    /// (currently InvalidOperationException) if the object does not
    /// represent a number (for this purpose, infinity and not-a-number
    /// values, but not <c>CBORObject.Null</c>
    ///  , are considered numbers).
    /// Currently, this is equivalent to the result of <c>AsEFloat()</c>
    ///  ,
    /// <c>AsEDecimal()</c>
    ///  , <c>AsEInteger</c>
    ///  , or <c>AsERational()</c>
    ///  ,
    /// respectively, but may change slightly in the next major version.
    /// Note that in the case of <c>EFloat</c>
    ///  , if this object represents
    /// a decimal number with a fractional part, the conversion may lose
    /// information depending on the number, and if the object is a
    /// rational number with a nonterminating binary expansion, the number
    /// returned is a binary floating-point number rounded to a high but
    /// limited precision. In the case of <c>EDecimal</c>
    ///  , if this object
    /// expresses a rational number with a nonterminating decimal
    /// expansion, returns a decimal number rounded to 34 digits of
    /// precision. In the case of <c>EInteger</c>
    ///  , if this CBOR object
    /// expresses a floating-point number, it is converted to an integer by
    /// discarding its fractional part, and if this CBOR object expresses a
    /// rational number, it is converted to an integer by dividing the
    /// numerator by the denominator and discarding the fractional part of
    /// the result, and this method throws an exception (currently
    /// OverflowException, but may change in the next major version) if
    /// this object expresses infinity or a not-a-number value.</item>
    /// <item>In the.NET version, if the type is a nullable (e.g.,
    /// <c>Nullable&lt;int&gt;</c>
    ///  or <c>int?</c>
    ///  , returns <c>null</c>
    ///  if
    /// this CBOR object is null, or this object's value converted to the
    /// nullable's underlying type, e.g., <c>int</c>
    ///  .</item>
    ///  <item>If the
    /// type is an enumeration ( <c>Enum</c>
    ///  ) type and this CBOR object is
    /// a text string or an integer, returns the appropriate enumerated
    /// constant. (For example, if <c>MyEnum</c>
    ///  includes an entry for
    /// <c>MyValue</c>
    ///  , this method will return <c>MyEnum.MyValue</c>
    ///  if
    /// the CBOR object represents <c>"MyValue"</c>
    ///  or the underlying value
    /// for <c>MyEnum.MyValue</c>
    ///  .) <b>Note:</b>
    ///  If an integer is
    /// converted to a.NET Enum constant, and that integer is shared by
    /// more than one constant of the same type, it is undefined which
    /// constant from among them is returned. (For example, if
    /// <c>MyEnum.Zero=0</c>
    ///  and <c>MyEnum.Null=0</c>
    ///  , converting 0 to
    /// <c>MyEnum</c>
    ///  may return either <c>MyEnum.Zero</c>
    ///  or
    /// <c>MyEnum.Null</c>
    ///  .) As a result, .NET Enum types with constants
    /// that share an underlying value should not be passed to this
    /// method.</item>
    ///  <item>If the type is <c>byte[]</c>
    ///  (a
    /// one-dimensional byte array) and this CBOR object is a byte string,
    /// returns a byte array which this CBOR byte string's data will be
    /// copied to. (This method can't be used to encode CBOR data to a byte
    /// array; for that, use the EncodeToBytes method instead.)</item>
    /// <item>If the type is a one-dimensional or multidimensional array
    /// type and this CBOR object is an array, returns an array containing
    /// the items in this CBOR object.</item>
    ///  <item>If the type is List,
    /// ReadOnlyCollection or the generic or non-generic IList,
    /// ICollection, IEnumerable, IReadOnlyCollection, or IReadOnlyList (or
    /// ArrayList, List, Collection, or Iterable in Java), and if this CBOR
    /// object is an array, returns an object conforming to the type,
    /// class, or interface passed to this method, where the object will
    /// contain all items in this CBOR array.</item>
    ///  <item>If the type is
    /// Dictionary, ReadOnlyDictionary or the generic or non-generic
    /// IDictionary or IReadOnlyDictionary (or HashMap or Map in Java), and
    /// if this CBOR object is a map, returns an object conforming to the
    /// type, class, or interface passed to this method, where the object
    /// will contain all keys and values in this CBOR map.</item>
    ///  <item>If
    /// the type is an enumeration constant ("enum"), and this CBOR object
    /// is an integer or text string, returns the enumeration constant with
    /// the given number or name, respectively. (Enumeration constants made
    /// up of multiple enumeration constants, as allowed by .NET, can only
    /// be matched by number this way.)</item>
    ///  <item>If the type is
    /// <c>DateTime</c>
    ///  (or <c>Date</c>
    ///  in Java) , returns a date/time
    /// object if the CBOR object's outermost tag is 0 or 1. For tag 1,
    /// this method treats the CBOR object as a number of seconds since the
    /// start of 1970, which is based on the POSIX definition of "seconds
    /// since the Epoch", a definition that does not count leap seconds. In
    /// this method, this number of seconds assumes the use of a proleptic
    /// Gregorian calendar, in which the rules regarding the number of days
    /// in each month and which years are leap years are the same for all
    /// years as they were in 1970 (including without regard to time zone
    /// differences or transitions from other calendars to the Gregorian).
    /// The string format used in tag 0 supports only years up to 4 decimal
    /// digits long. For tag 1, CBOR objects that express infinity or
    /// not-a-number (NaN) are treated as invalid by this method. This
    /// default behavior for <c>DateTime</c>
    ///  and <c>Date</c>
    ///  can be changed
    /// by passing a suitable CBORTypeMapper to this method, such as a
    /// CBORTypeMapper that registers a CBORDateConverter for
    /// <c>DateTime</c>
    ///  or <c>Date</c>
    ///  objects. See the examples.</item>
    /// <item>If the type is <c>Uri</c>
    ///  (or <c>URI</c>
    ///  in Java), returns a
    /// URI object if possible.</item>
    ///  <item>If the type is <c>Guid</c>
    ///  (or
    /// <c>UUID</c>
    ///  in Java), returns a UUID object if possible.</item>
    /// <item>Plain-Old-Data deserialization: If the object is a type not
    /// specially handled above, the type includes a zero-parameter
    /// constructor (default or not), this CBOR object is a CBOR map, and
    /// the "mapper" parameter (if any) allows this type to be eligible for
    /// Plain-Old-Data deserialization, then this method checks the given
    /// type for eligible setters as follows:</item>
    ///  <item>(*) In the .NET
    /// version, eligible setters are the public, nonstatic setters of
    /// properties with a public, nonstatic getter. Eligible setters also
    /// include public, nonstatic, non- <c>const</c>
    ///  , non- <c>readonly</c>
    /// fields. If a class has two properties and/or fields of the form "X"
    /// and "IsX", where "X" is any name, or has multiple properties and/or
    /// fields with the same name, those properties and fields are
    /// ignored.</item>
    ///  <item>(*) In the Java version, eligible setters are
    /// public, nonstatic methods starting with "set" followed by a
    /// character other than a basic digit or lower-case letter, that is,
    /// other than "a" to "z" or "0" to "9", that take one parameter. The
    /// class containing an eligible setter must have a public, nonstatic
    /// method with the same name, but starting with "get" or "is" rather
    /// than "set", that takes no parameters and does not return void. (For
    /// example, if a class has "public setValue(String)" and "public
    /// getValue()", "setValue" is an eligible setter. However,
    /// "setValue()" and "setValue(String, int)" are not eligible setters.)
    /// In addition, public, nonstatic, nonfinal fields are also eligible
    /// setters. If a class has two or more otherwise eligible setters
    /// (methods and/or fields) with the same name, but different parameter
    /// type, they are not eligible setters.</item>
    ///  <item>Then, the method
    /// creates an object of the given type and invokes each eligible
    /// setter with the corresponding value in the CBOR map, if any. Key
    /// names in the map are matched to eligible setters according to the
    /// rules described in the <see cref='PeterO.Cbor.PODOptions'/>
    /// documentation. Note that for security reasons, certain types are
    /// not supported even if they contain eligible setters. For the Java
    /// version, the object creation may fail in the case of a nested
    /// nonstatic class.</item>
    ///  </list>
    ///  </summary>
    /// <param name='t'>The type, class, or interface that this method's
    /// return value will belong to. To express a generic type in Java, see
    /// the example. <b>Note:</b>
    ///  For security reasons, an application
    /// should not base this parameter on user input or other externally
    /// supplied data. Whenever possible, this parameter should be either a
    /// type specially handled by this method, such as <c>int</c>
    ///  or
    /// <c>String</c>
    ///  , or a plain-old-data type (POCO or POJO type) within
    /// the control of the application. If the plain-old-data type
    /// references other data types, those types should likewise meet
    /// either criterion above.</param>
    /// <param name='mapper'>This parameter controls which data types are
    /// eligible for Plain-Old-Data deserialization and includes custom
    /// converters from CBOR objects to certain data types. Can be
    /// null.</param>
    /// <param name='options'>Specifies options for controlling
    /// deserialization of CBOR objects.</param>
    /// <returns>The converted object.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>The given type
    /// <paramref name='t'/> , or this object's CBOR type, is not
    /// supported, or the given object's nesting is too deep, or another
    /// error occurred when serializing the object.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='t'/> or <paramref name='options'/> is null.</exception>
    /// <example>
    /// <para>The following example (originally written in C# for the
    /// DotNet version) uses a CBORTypeMapper to change how CBOR objects
    /// are converted to DateTime objects. In this case, the ToObject
    /// method assumes the CBOR object is an untagged number giving the
    /// number of seconds since the start of 1970.</para>
    /// <code>var conv = new CBORTypeMapper().AddConverter(typeof(DateTime),
    /// CBORDateConverter.UntaggedNumber);
    /// var obj = CBORObject.FromObject().ToObject&lt;DateTime&gt;(conv);</code>
    /// <para>Java offers no easy way to express a generic type, at least
    /// none as easy as C#'s <c>typeof</c>
    ///  operator. The following example,
    /// written in Java, is a way to specify that the return value will be
    /// an ArrayList of String objects.</para>
    /// <code>Type arrayListString = new ParameterizedType() { public Type[]
    /// getActualTypeArguments() { &#x2f;&#x2a; Contains one type parameter,
    /// String&#x2a;&#x2f;
    /// return new Type[] { String.class }; }
    /// public Type getRawType() { /* Raw type is
    /// ArrayList */ return ArrayList.class; } public Type getOwnerType() {
    /// return null; } }; ArrayList&lt;String&gt; array =
    /// (ArrayList&lt;String&gt;) cborArray.ToObject(arrayListString);</code>
    /// <para>By comparison, the C# version is much shorter.</para>
    /// <code>var array = (List&lt;String&gt;)cborArray.ToObject(
    /// typeof(List&lt;String&gt;));</code>
    ///  .
    /// </example>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public object ToObject(Type t, CBORTypeMapper mapper, PODOptions
      options) {
      return options == null ?
        throw new ArgumentNullException(nameof(options)) :
        this.ToObject(t, mapper, options, 0);
    }

    /// <summary>Generates an object of an arbitrary type from an array of
    /// CBOR-encoded bytes, using the given <c>CBOREncodeOptions</c> object
    /// to control the decoding process. It is equivalent to
    /// DecodeFromBytes followed by ToObject. See the documentation for
    /// those methods for more information.</summary>
    /// <param name='data'>A byte array in which a single CBOR object is
    /// encoded.</param>
    /// <param name='enc'>Specifies options to control how the CBOR object
    /// is decoded. See
    /// <see cref='PeterO.Cbor.CBOREncodeOptions'/> for more
    /// information.</param>
    /// <param name='t'>The type, class, or interface that this method's
    /// return value will belong to. To express a generic type in Java, see
    /// the example. <b>Note:</b> For security reasons, an application
    /// should not base this parameter on user input or other externally
    /// supplied data. Whenever possible, this parameter should be either a
    /// type specially handled by this method, such as <c>int</c> or
    /// <c>String</c>, or a plain-old-data type (POCO or POJO type) within
    /// the control of the application. If the plain-old-data type
    /// references other data types, those types should likewise meet
    /// either criterion above.</param>
    /// <param name='mapper'>This parameter controls which data types are
    /// eligible for Plain-Old-Data deserialization and includes custom
    /// converters from CBOR objects to certain data types. Can be
    /// null.</param>
    /// <param name='pod'>Specifies options for controlling deserialization
    /// of CBOR objects.</param>
    /// <returns>An object of the given type decoded from the given byte
    /// array. Returns null (as opposed to CBORObject.Null) if <paramref
    /// name='data'/> is empty and the AllowEmpty property is set on the
    /// given CBOREncodeOptions object.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data. This includes cases where not all of
    /// the byte array represents a CBOR object. This exception is also
    /// thrown if the parameter <paramref name='data'/> is empty unless the
    /// AllowEmpty property is set on the given options object. Also thrown
    /// if the given type <paramref name='t'/>, or this object's CBOR
    /// type, is not supported, or the given object's nesting is too deep,
    /// or another error occurred when serializing the object.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='data'/> is null, or the parameter <paramref name='enc'/> is
    /// null, or the parameter <paramref name='t'/> or <paramref
    /// name='pod'/> is null.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static object DecodeObjectFromBytes(
      byte[] data,
      CBOREncodeOptions enc,
      Type t,
      CBORTypeMapper mapper,
      PODOptions pod) {
      return pod == null ? throw new ArgumentNullException(nameof(pod)) :
        enc == null ? throw new ArgumentNullException(nameof(enc)) :
DecodeFromBytes(data, enc).ToObject(t, mapper, pod);
    }

    /// <summary>Generates an object of an arbitrary type from an array of
    /// CBOR-encoded bytes, using the given <c>CBOREncodeOptions</c> object
    /// to control the decoding process. It is equivalent to
    /// DecodeFromBytes followed by ToObject. See the documentation for
    /// those methods for more information.</summary>
    /// <param name='data'>A byte array in which a single CBOR object is
    /// encoded.</param>
    /// <param name='enc'>Specifies options to control how the CBOR object
    /// is decoded. See
    /// <see cref='PeterO.Cbor.CBOREncodeOptions'/> for more
    /// information.</param>
    /// <param name='t'>The type, class, or interface that this method's
    /// return value will belong to. To express a generic type in Java, see
    /// the example. <b>Note:</b> For security reasons, an application
    /// should not base this parameter on user input or other externally
    /// supplied data. Whenever possible, this parameter should be either a
    /// type specially handled by this method, such as <c>int</c> or
    /// <c>String</c>, or a plain-old-data type (POCO or POJO type) within
    /// the control of the application. If the plain-old-data type
    /// references other data types, those types should likewise meet
    /// either criterion above.</param>
    /// <returns>An object of the given type decoded from the given byte
    /// array. Returns null (as opposed to CBORObject.Null) if <paramref
    /// name='data'/> is empty and the AllowEmpty property is set on the
    /// given CBOREncodeOptions object.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data. This includes cases where not all of
    /// the byte array represents a CBOR object. This exception is also
    /// thrown if the parameter <paramref name='data'/> is empty unless the
    /// AllowEmpty property is set on the given options object. Also thrown
    /// if the given type <paramref name='t'/>, or this object's CBOR
    /// type, is not supported, or the given object's nesting is too deep,
    /// or another error occurred when serializing the object.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='data'/> is null, or the parameter <paramref name='enc'/> is
    /// null, or the parameter <paramref name='t'/> is null.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static object DecodeObjectFromBytes(
      byte[] data,
      CBOREncodeOptions enc,
      Type t) {
      return DecodeFromBytes(data, enc).ToObject(t);
    }

    /// <summary>Generates an object of an arbitrary type from an array of
    /// CBOR-encoded bytes. It is equivalent to DecodeFromBytes followed by
    /// ToObject. See the documentation for those methods for more
    /// information.</summary>
    /// <param name='data'>A byte array in which a single CBOR object is
    /// encoded.</param>
    /// <param name='t'>The type, class, or interface that this method's
    /// return value will belong to. To express a generic type in Java, see
    /// the example. <b>Note:</b> For security reasons, an application
    /// should not base this parameter on user input or other externally
    /// supplied data. Whenever possible, this parameter should be either a
    /// type specially handled by this method, such as <c>int</c> or
    /// <c>String</c>, or a plain-old-data type (POCO or POJO type) within
    /// the control of the application. If the plain-old-data type
    /// references other data types, those types should likewise meet
    /// either criterion above.</param>
    /// <param name='mapper'>This parameter controls which data types are
    /// eligible for Plain-Old-Data deserialization and includes custom
    /// converters from CBOR objects to certain data types. Can be
    /// null.</param>
    /// <param name='pod'>Specifies options for controlling deserialization
    /// of CBOR objects.</param>
    /// <returns>An object of the given type decoded from the given byte
    /// array. Returns null (as opposed to CBORObject.Null) if <paramref
    /// name='data'/> is empty and the AllowEmpty property is set on the
    /// given CBOREncodeOptions object.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data. This includes cases where not all of
    /// the byte array represents a CBOR object. This exception is also
    /// thrown if the parameter <paramref name='data'/> is empty unless the
    /// AllowEmpty property is set on the given options object. Also thrown
    /// if the given type <paramref name='t'/>, or this object's CBOR
    /// type, is not supported, or the given object's nesting is too deep,
    /// or another error occurred when serializing the object.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='data'/> is null, or the parameter <paramref name='t'/> or
    /// <paramref name='pod'/> is null.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static object DecodeObjectFromBytes(
      byte[] data,
      Type t,
      CBORTypeMapper mapper,
      PODOptions pod) {
      return
DecodeObjectFromBytes(data, CBOREncodeOptions.Default, t, mapper, pod);
    }

    /// <summary>Generates an object of an arbitrary type from an array of
    /// CBOR-encoded bytes. It is equivalent to DecodeFromBytes followed by
    /// ToObject. See the documentation for those methods for more
    /// information.</summary>
    /// <param name='data'>A byte array in which a single CBOR object is
    /// encoded.</param>
    /// <param name='t'>The type, class, or interface that this method's
    /// return value will belong to. To express a generic type in Java, see
    /// the example. <b>Note:</b> For security reasons, an application
    /// should not base this parameter on user input or other externally
    /// supplied data. Whenever possible, this parameter should be either a
    /// type specially handled by this method, such as <c>int</c> or
    /// <c>String</c>, or a plain-old-data type (POCO or POJO type) within
    /// the control of the application. If the plain-old-data type
    /// references other data types, those types should likewise meet
    /// either criterion above.</param>
    /// <returns>An object of the given type decoded from the given byte
    /// array. Returns null (as opposed to CBORObject.Null) if <paramref
    /// name='data'/> is empty and the AllowEmpty property is set on the
    /// given CBOREncodeOptions object.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data. This includes cases where not all of
    /// the byte array represents a CBOR object. This exception is also
    /// thrown if the parameter <paramref name='data'/> is empty unless the
    /// AllowEmpty property is set on the given options object. Also thrown
    /// if the given type <paramref name='t'/>, or this object's CBOR
    /// type, is not supported, or the given object's nesting is too deep,
    /// or another error occurred when serializing the object.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='data'/> is null, or the parameter <paramref name='t'/> is
    /// null.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static object DecodeObjectFromBytes(byte[] data, Type t) {
      return DecodeObjectFromBytes(data, CBOREncodeOptions.Default, t);
    }
    internal EDecimal ToEDecimal() {
      CBORNumber cn = this.AsNumber();
      return cn.GetNumberInterface().AsEDecimal(cn.GetValue());
    }

    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    internal object ToObject(
      Type t,
      CBORTypeMapper mapper,
      PODOptions options,
      int depth) {
      ++depth;
      if (depth > 100) {
        throw new CBORException("Depth level too high");
      }
      if (t == null) {
        throw new ArgumentNullException(nameof(t));
      }
      if (t.Equals(typeof(CBORObject))) {
        return this;
      }
      if (this.IsNull) {
        // TODO: In next major version, consider returning null
        // here only if this object is untagged, to allow behavior
        // to be customizable by CBORTypeMapper
        return null;
      }
      if (mapper != null) {
        object obj = mapper.ConvertBackWithConverter(this, t);
        if (obj != null) {
          return obj;
        }
      }
      if (t.Equals(typeof(object))) {
        return this;
      }
      // TODO: In next major version, address inconsistent
      // implementations for EDecimal, EInteger, EFloat,
      // and ERational (perhaps
      // by using EDecimal implementation). Also, these operations
      // might throw InvalidOperationException rather than CBORException.
      // Make them throw CBORException in next major version.
      if (t.Equals(typeof(EDecimal))) {
        CBORNumber cn = this.AsNumber();
        return cn.GetNumberInterface().AsEDecimal(cn.GetValue());
      }
      if (t.Equals(typeof(EFloat))) {
        var cn = CBORNumber.FromCBORObject(this);
        return cn == null ? throw new InvalidOperationException("Not a" +
"\u0020number type") : (object)cn.GetNumberInterface().AsEFloat(cn.GetValue());
      }
      if (t.Equals(typeof(EInteger))) {
        var cn = CBORNumber.FromCBORObject(this);
        return cn == null ? throw new InvalidOperationException("Not a" +
"\u0020number type") :
(object)cn.GetNumberInterface().AsEInteger(cn.GetValue());
      }
      if (t.Equals(typeof(ERational))) {
        // NOTE: Will likely be simplified in version 5.0 and later
        if (this.HasMostInnerTag(30) && this.Count != 2) {
          EInteger num, den;
          num = (EInteger)this[0].ToObject(typeof(EInteger));
          den = (EInteger)this[1].ToObject(typeof(EInteger));
          return ERational.Create(num, den);
        }
        var cn = CBORNumber.FromCBORObject(this);
        return cn == null ? throw new InvalidOperationException("Not a" +
"\u0020number type") :
(object)cn.GetNumberInterface().AsERational(cn.GetValue());
      }
      return t.Equals(typeof(string)) ? this.AsString() :
        PropertyMap.TypeToObject(this, t, mapper, options, depth);
    }

    /// <summary>Generates a CBOR object from a 64-bit signed
    /// integer.</summary>
    /// <param name='value'>The parameter <paramref name='value'/> is a
    /// 64-bit signed integer.</param>
    /// <returns>A CBOR object.</returns>
    public static CBORObject FromInt64(long value) {
      return value >= 0L && value < 24L ? FixedObjects[(int)value] :
        (value >= -24L && value < 0L) ? FixedObjects[0x20 - (int)(value +
              1L)] : new CBORObject(CBORObjectTypeInteger, value);
    }

    /// <summary>Generates a CBOR object from a 64-bit signed
    /// integer.</summary>
    /// <param name='value'>The parameter <paramref name='value'/> is a
    /// 64-bit signed integer.</param>
    /// <returns>A CBOR object.</returns>
    [Obsolete("Use FromInt64 instead.")]
    public static CBORObject FromObject(long value) => FromInt64(value);

    /// <summary>Generates a CBOR object from a CBOR object.</summary>
    /// <param name='value'>The parameter <paramref name='value'/> is a
    /// CBOR object.</param>
    /// <returns>Same as <paramref name='value'/>, or "CBORObject.Null" is
    /// <paramref name='value'/> is null.</returns>
    [Obsolete("Don't use a function and use Nullable Reference Types to" +
"\u0020guard against nulls.")]
    public static CBORObject FromObject(CBORObject value) {
      return value ?? Null;
    }

    private static int IntegerByteLength(int intValue) {
      if (intValue < 0) {
        intValue = -(intValue + 1);
      }
      return intValue > 0xffff ? 5 : intValue > 0xff ? 3 : (intValue > 23) ?
2 : 1;
    }

    private static int IntegerByteLength(long longValue) {
      if (longValue < 0) {
        longValue = -(longValue + 1);
      }
      return longValue > 0xffffffffL ? 9 : longValue > 0xffffL ? 5 :
longValue > 0xffL ? 3 : (longValue > 23L) ? 2 : 1;
    }

    /// <summary>Calculates the number of bytes this CBOR object takes when
    /// serialized as a byte array using the <c>EncodeToBytes()</c> method.
    /// This calculation assumes that integers, lengths of maps and arrays,
    /// lengths of text and byte strings, and tag numbers are encoded in
    /// their shortest form; that floating-point numbers are encoded in
    /// their shortest value-preserving form; and that no indefinite-length
    /// encodings are used.</summary>
    /// <returns>The number of bytes this CBOR object takes when serialized
    /// as a byte array using the <c>EncodeToBytes()</c> method.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>The CBOR object has an
    /// extremely deep level of nesting, including if the CBOR object is or
    /// has an array or map that includes itself.</exception>
    public long CalcEncodedSize() {
      return this.CalcEncodedSize(0);
    }

    private long CalcEncodedSize(int depth) {
      if (depth > 1000) {
        throw new CBORException("Too deeply nested");
      }
      // DebugUtility.Log("type="+this.Type+" depth="+depth);
      long size = 0L;
      CBORObject cbor = this;
      while (cbor.IsTagged) {
        EInteger etag = cbor.MostOuterTag;
        if (etag.CanFitInInt64()) {
          long tag = etag.ToInt64Checked();
          size = checked(size + IntegerByteLength(tag));
        } else {
          size = checked(size + 9);
        }
        cbor = cbor.UntagOne();
      }
      if (cbor.ItemType == CBORObjectTypeTextStringUtf8) {
        byte[] bytes = (byte[])this.ThisItem;
        size = checked(size + IntegerByteLength(bytes.Length));
        return checked(size + bytes.Length);
      }
      if (cbor.ItemType == CBORObjectTypeTextStringAscii) {
        var str = (string)this.ThisItem;
        size = checked(size + IntegerByteLength(str.Length));
        return checked(size + str.Length);
      }
      switch (cbor.Type) {
        case CBORType.Integer:
          {
            if (cbor.CanValueFitInInt64()) {
              long tag = cbor.AsInt64Value();
              size = checked(size + IntegerByteLength(tag));
              return size;
            } else {
              return checked(size + 9);
            }
          }
        case CBORType.FloatingPoint:
          {
            long valueBits = cbor.AsDoubleBits();
            int bits =
CBORUtilities.DoubleToHalfPrecisionIfSameValue(valueBits);
            return bits != -1 ? size + 3 :
              CBORUtilities.DoubleRetainsSameValueInSingle(valueBits) ?
              checked(size + 5) : checked(size + 9);
          }
        case CBORType.Array:
          size = checked(size + IntegerByteLength(cbor.Count));
          for (int i = 0; i < cbor.Count; ++i) {
            long newsize = cbor[i].CalcEncodedSize(depth + 1);
            size = checked(size + newsize);
          }
          return size;
        case CBORType.Map:
          {
            ICollection<KeyValuePair<CBORObject, CBORObject>> entries =
              this.Entries;
            size = checked(size + IntegerByteLength(entries.Count));
            try {
              foreach (KeyValuePair<CBORObject, CBORObject> entry in entries) {
                CBORObject key = entry.Key;
                CBORObject value = entry.Value;
                size = checked(size + key.CalcEncodedSize(depth + 1));
                size = checked(size + value.CalcEncodedSize(depth + 1));
              }
            } catch (InvalidOperationException ex) {
              // Additional error that may occur in iteration
              throw new CBORException(ex.Message, ex);
            } catch (ArgumentException ex) {
              // Additional error that may occur in iteration
              throw new CBORException(ex.Message, ex);
            }
            return size;
          }
        case CBORType.TextString:
          {
            long ulength = DataUtilities.GetUtf8Length(this.AsString(), false);
            size = checked(size + IntegerByteLength(ulength));
            return checked(size + ulength);
          }
        case CBORType.ByteString:
          {
            byte[] bytes = cbor.GetByteString();
            size = checked(size + IntegerByteLength(bytes.Length));
            return checked(size + bytes.Length);
          }
        case CBORType.Boolean:
          return checked(size + 1);
        case CBORType.SimpleValue:
          return checked(size + (cbor.SimpleValue >= 24 ? 2 : 1));
        default: throw new InvalidOperationException();
      }
    }

    /// <summary>Generates a CBOR object from an arbitrary-precision
    /// integer. The CBOR object is generated as follows:
    /// <list>
    /// <item>If the number is null, returns CBORObject.Null.</item>
    /// <item>Otherwise, if the number is greater than or equal to -(2^64)
    /// and less than 2^64, the CBOR object will have the object type
    /// Integer and the appropriate value.</item>
    /// <item>Otherwise, the CBOR object will have tag 2 (zero or positive)
    /// or 3 (negative) and the appropriate value.</item></list></summary>
    /// <param name='bigintValue'>An arbitrary-precision integer. Can be
    /// null.</param>
    /// <returns>The given number encoded as a CBOR object. Returns
    /// CBORObject.Null if <paramref name='bigintValue'/> is
    /// null.</returns>
    public static CBORObject FromEInteger(EInteger bigintValue) {
      if (bigintValue == null) {
        return CBORObject.Null;
      }
      if (bigintValue.CanFitInInt64()) {
        return CBORObject.FromInt64(bigintValue.ToInt64Checked());
      } else {
        EInteger bitLength = bigintValue.GetSignedBitLengthAsEInteger();
        if (bitLength.CompareTo(64) <= 0) {
          // Fits in major type 0 or 1
          return new CBORObject(CBORObjectTypeEInteger, bigintValue);
        } else {
          int tag = (bigintValue.Sign < 0) ? 3 : 2;
          return FromByteArray(EIntegerBytes(bigintValue)).WithTag(tag);
        }
      }
    }

    /// <summary>Generates a CBOR object from an arbitrary-precision
    /// integer.</summary>
    /// <param name='bigintValue'>An arbitrary-precision integer. Can be
    /// null.</param>
    /// <returns>The given number encoded as a CBOR object. Returns
    /// CBORObject.Null if <paramref name='bigintValue'/> is
    /// null.</returns>
    [Obsolete("Use FromEInteger instead.")]
    public static CBORObject FromObject(EInteger bigintValue) =>
FromEInteger(bigintValue);

    /// <summary>Generates a CBOR object from an arbitrary-precision binary
    /// floating-point number. The CBOR object is generated as follows
    /// (this is a change in version 4.0):
    /// <list>
    /// <item>If the number is null, returns CBORObject.Null.</item>
    /// <item>Otherwise, if the number expresses infinity, not-a-number, or
    /// negative zero, the CBOR object will have tag 269 and the
    /// appropriate format.</item>
    /// <item>Otherwise, if the number's exponent is at least 2^64 or less
    /// than -(2^64), the CBOR object will have tag 265 and the appropriate
    /// format.</item>
    /// <item>Otherwise, the CBOR object will have tag 5 and the
    /// appropriate format.</item></list></summary>
    /// <param name='bigValue'>An arbitrary-precision binary floating-point
    /// number. Can be null.</param>
    /// <returns>The given number encoded as a CBOR object. Returns
    /// CBORObject.Null if <paramref name='bigValue'/> is null.</returns>
    public static CBORObject FromEFloat(EFloat bigValue) {
      if (bigValue == null) {
        return Null;
      }
      CBORObject cbor;
      int tag;
      if (bigValue.IsInfinity() || bigValue.IsNaN() ||
        (bigValue.IsNegative && bigValue.IsZero)) {
        int options = bigValue.IsNegative ? 1 : 0;
        if (bigValue.IsInfinity()) {
          options += 2;
        }
        if (bigValue.IsQuietNaN()) {
          options += 4;
        }
        if (bigValue.IsSignalingNaN()) {
          options += 6;
        }
        cbor = NewArray(
            FromEInteger(bigValue.Exponent),
            FromEInteger(bigValue.UnsignedMantissa),
            FromInt32(options));
        tag = 269;
      } else {
        EInteger exponent = bigValue.Exponent;
        if (exponent.CanFitInInt64()) {
          tag = 5;
          cbor = NewArray(
              FromInt64(exponent.ToInt64Checked()),
              FromEInteger(bigValue.Mantissa));
        } else {
          tag = (exponent.GetSignedBitLengthAsInt64() > 64) ?
            265 : 5;
          cbor = NewArray(
              FromEInteger(exponent),
              FromEInteger(bigValue.Mantissa));
        }
      }
      return cbor.WithTag(tag);
    }

    /// <summary>Generates a CBOR object from an arbitrary-precision binary
    /// floating-point number.</summary>
    /// <param name='bigValue'>An arbitrary-precision binary floating-point
    /// number.</param>
    /// <returns>The given number encoded as a CBOR object.</returns>
    [Obsolete("Use FromEFloat instead.")]
    public static CBORObject FromObject(EFloat bigValue) =>
FromEFloat(bigValue);

    /// <summary>Generates a CBOR object from an arbitrary-precision
    /// rational number. The CBOR object is generated as follows (this is a
    /// change in version 4.0):
    /// <list>
    /// <item>If the number is null, returns CBORObject.Null.</item>
    /// <item>Otherwise, if the number expresses infinity, not-a-number, or
    /// negative zero, the CBOR object will have tag 270 and the
    /// appropriate format.</item>
    /// <item>Otherwise, the CBOR object will have tag 30 and the
    /// appropriate format.</item></list></summary>
    /// <param name='bigValue'>An arbitrary-precision rational number. Can
    /// be null.</param>
    /// <returns>The given number encoded as a CBOR object. Returns
    /// CBORObject.Null if <paramref name='bigValue'/> is null.</returns>
    public static CBORObject FromERational(ERational bigValue) {
      if (bigValue == null) {
        return Null;
      }
      CBORObject cbor;
      int tag;
      if (bigValue.IsInfinity() || bigValue.IsNaN() ||
        (bigValue.IsNegative && bigValue.IsZero)) {
        int options = bigValue.IsNegative ? 1 : 0;
        if (bigValue.IsInfinity()) {
          options += 2;
        }
        if (bigValue.IsQuietNaN()) {
          options += 4;
        }
        if (bigValue.IsSignalingNaN()) {
          options += 6;
        }
#if DEBUG
        if (!(!bigValue.IsInfinity() || bigValue.UnsignedNumerator.IsZero)) {
          throw new InvalidOperationException("doesn't satisfy" +
            "\u0020!bigValue.IsInfinity() ||" +
            "\u0020bigValue.UnsignedNumerator.IsZero");
        }
        if (!(!bigValue.IsInfinity() || bigValue.Denominator.CompareTo(1) ==
            0)) {
          throw new InvalidOperationException("doesn't satisfy" +
            "\u0020!bigValue.IsInfinity() ||" +
            "\u0020bigValue.Denominator.CompareTo(1)==0");
        }
        if (!(!bigValue.IsNaN() || bigValue.Denominator.CompareTo(1) == 0)) {
          throw new InvalidOperationException("doesn't satisfy" +
            "\u0020!bigValue.IsNaN() ||" +
            "\u0020bigValue.Denominator.CompareTo(1)==0");
        }
#endif

        cbor = NewArray(
            FromEInteger(bigValue.UnsignedNumerator),
            FromEInteger(bigValue.Denominator),
            FromInt32(options));
        tag = 270;
      } else {
        tag = 30;
        cbor = NewArray(
            FromEInteger(bigValue.Numerator),
            FromEInteger(bigValue.Denominator));
      }
      return cbor.WithTag(tag);
    }

    /// <summary>Generates a CBOR object from an arbitrary-precision
    /// rational number.</summary>
    /// <param name='bigValue'>An arbitrary-precision rational
    /// number.</param>
    /// <returns>The given number encoded as a CBOR object.</returns>
    [Obsolete("Use FromERational instead.")]
    public static CBORObject FromObject(ERational bigValue) =>
FromERational(bigValue);

    /// <summary>Generates a CBOR object from a decimal number. The CBOR
    /// object is generated as follows (this is a change in version 4.0):
    /// <list>
    /// <item>If the number is null, returns CBORObject.Null.</item>
    /// <item>Otherwise, if the number expresses infinity, not-a-number, or
    /// negative zero, the CBOR object will have tag 268 and the
    /// appropriate format.</item>
    /// <item>If the number's exponent is at least 2^64 or less than
    /// -(2^64), the CBOR object will have tag 264 and the appropriate
    /// format.</item>
    /// <item>Otherwise, the CBOR object will have tag 4 and the
    /// appropriate format.</item></list></summary>
    /// <param name='bigValue'>An arbitrary-precision decimal number. Can
    /// be null.</param>
    /// <returns>The given number encoded as a CBOR object. Returns
    /// CBORObject.Null if <paramref name='bigValue'/> is null.</returns>
    public static CBORObject FromEDecimal(EDecimal bigValue) {
      if (bigValue == null) {
        return Null;
      }
      CBORObject cbor;
      int tag;
      if (bigValue.IsInfinity() || bigValue.IsNaN() ||
        (bigValue.IsNegative && bigValue.IsZero)) {
        int options = bigValue.IsNegative ? 1 : 0;
        if (bigValue.IsInfinity()) {
          options += 2;
        }
        if (bigValue.IsQuietNaN()) {
          options += 4;
        }
        if (bigValue.IsSignalingNaN()) {
          options += 6;
        }
        cbor = NewArray(
            FromEInteger(bigValue.Exponent),
            FromEInteger(bigValue.UnsignedMantissa),
            FromInt32(options));
        tag = 268;
      } else {
        EInteger exponent = bigValue.Exponent;
        if (exponent.CanFitInInt64()) {
          tag = 4;
          cbor = NewArray(
              FromInt64(exponent.ToInt64Checked()),
              FromEInteger(bigValue.Mantissa));
        } else {
          tag = (exponent.GetSignedBitLengthAsInt64() > 64) ?
            264 : 4;
          cbor = NewArray(
              FromEInteger(exponent),
              FromEInteger(bigValue.Mantissa));
        }
      }
      return cbor.WithTag(tag);
    }

    /// <summary>Generates a CBOR object from a decimal number.</summary>
    /// <param name='bigValue'>An arbitrary-precision decimal
    /// number.</param>
    /// <returns>The given number encoded as a CBOR object.</returns>
    [Obsolete("Use FromEDecimal instead.")]
    public static CBORObject FromObject(EDecimal bigValue) =>
FromEDecimal(bigValue);

    /// <summary>Generates a CBOR object from a text string.</summary>
    /// <param name='strValue'>A text string value. Can be null.</param>
    /// <returns>A CBOR object representing the string, or CBORObject.Null
    /// if stringValue is null.</returns>
    /// <exception cref='ArgumentException'>The string contains an unpaired
    /// surrogate code point.</exception>
    public static CBORObject FromString(string strValue) {
      if (strValue == null) {
        return CBORObject.Null;
      }
      if (strValue.Length == 0) {
        return GetFixedObject(0x60);
      }
      long utf8Length = DataUtilities.GetUtf8Length(strValue, false);
      return utf8Length < 0 ?
        throw new ArgumentException("String contains an unpaired " +
          "surrogate code point.") : new CBORObject(
            strValue.Length == utf8Length ? CBORObjectTypeTextStringAscii : CBORObjectTypeTextString,
            strValue);
    }

    /// <summary>Generates a CBOR object from a text string.</summary>
    /// <param name='strValue'>A text string value. Can be null.</param>
    /// <returns>A CBOR object representing the string, or CBORObject.Null
    /// if stringValue is null.</returns>
    [Obsolete("Use FromString instead.")]
    public static CBORObject FromObject(string strValue) =>
FromString(strValue);

    /// <summary>Generates a CBOR object from a 32-bit signed
    /// integer.</summary>
    /// <param name='value'>The parameter <paramref name='value'/> is a
    /// 32-bit signed integer.</param>
    /// <returns>A CBOR object.</returns>
    public static CBORObject FromInt32(int value) {
      return value >= 0 && value < 24 ? FixedObjects[value] :
        (value >= -24 && value < 0) ? FixedObjects[0x20 - (value + 1)] :
          FromInt64((long)value);
    }

    /// <summary>Generates a CBOR object from a Guid.</summary>
    /// <param name='value'>The parameter <paramref name='value'/> is a
    /// Guid.</param>
    /// <returns>A CBOR object.</returns>
    public static CBORObject FromGuid(Guid value) => new
CBORUuidConverter().ToCBORObject(value);

    /// <summary>Generates a CBOR object from a 32-bit signed
    /// integer.</summary>
    /// <param name='value'>The parameter <paramref name='value'/> is a
    /// 32-bit signed integer.</param>
    /// <returns>A CBOR object.</returns>
    [Obsolete("Use FromInt instead.")]
    public static CBORObject FromObject(int value) => FromInt32(value);

    /// <summary>Generates a CBOR object from a 16-bit signed
    /// integer.</summary>
    /// <param name='value'>The parameter <paramref name='value'/> is a
    /// 16-bit signed integer.</param>
    /// <returns>A CBOR object generated from the given integer.</returns>
    public static CBORObject FromInt16(short value) {
      return value >= 0 && value < 24 ? FixedObjects[value] :
        (value >= -24 && value < 0) ? FixedObjects[0x20 - (value + 1)] :
          FromInt64((long)value);
    }

    /// <summary>Generates a CBOR object from a 16-bit signed
    /// integer.</summary>
    /// <param name='value'>A 16-bit signed integer.</param>
    /// <returns>A CBOR object generated from the given integer.</returns>
    [Obsolete("Use FromInt16 instead.")]
    public static CBORObject FromObject(short value) => FromInt16(value);

    /// <summary>Returns the CBOR true value or false value, depending on
    /// "value".</summary>
    /// <param name='value'>Either <c>true</c> or <c>false</c>.</param>
    /// <returns>CBORObject.True if value is true; otherwise
    /// CBORObject.False.</returns>
    public static CBORObject FromBool(bool value) {
      return value ? True : False;
    }

    /// <summary>Returns the CBOR true value or false value, depending on
    /// "value".</summary>
    /// <param name='value'>Either <c>true</c> or <c>false</c>.</param>
    /// <returns>CBORObject.True if value is true; otherwise
    /// CBORObject.False.</returns>
    [Obsolete("Use FromBool instead.")]
    public static CBORObject FromObject(bool value) => FromBool(value);

    /// <summary>Generates a CBOR object from a byte (0 to 255).</summary>
    /// <param name='value'>The parameter <paramref name='value'/> is a
    /// byte (from 0 to 255).</param>
    /// <returns>A CBOR object generated from the given integer.</returns>
    public static CBORObject FromByte(byte value) {
      return FromInt32(value & 0xff);
    }

    /// <summary>Generates a CBOR object from a byte.</summary>
    /// <param name='value'>A byte.</param>
    /// <returns>A CBOR object.</returns>
    [Obsolete("Use FromByte instead.")]
    public static CBORObject FromObject(byte value) => FromByte(value);

    /// <summary>Generates a CBOR object from a 32-bit floating-point
    /// number. The input value can be a not-a-number (NaN) value (such as
    /// <c>Single.NaN</c> in DotNet or Float.NaN in Java); however, NaN
    /// values have multiple forms that are equivalent for many
    /// applications' purposes, and <c>Single.NaN</c> / <c>Float.NaN</c> is
    /// only one of these equivalent forms. In fact,
    /// <c>CBORObject.FromSingle(Single.NaN)</c> or
    /// <c>CBORObject.FromSingle(Float.NaN)</c> could produce a
    /// CBOR-encoded object that differs between DotNet and Java, because
    /// <c>Single.NaN</c> / <c>Float.NaN</c> may have a different form in
    /// DotNet and Java (for example, the NaN value's sign may be negative
    /// in DotNet, but positive in Java).</summary>
    /// <param name='value'>The parameter <paramref name='value'/> is a
    /// 32-bit floating-point number.</param>
    /// <returns>A CBOR object generated from the given number.</returns>
    public static CBORObject FromSingle(float value) {
      long doubleBits = CBORUtilities.SingleToDoublePrecision(
          CBORUtilities.SingleToInt32Bits(value));
      return new CBORObject(CBORObjectTypeDouble, doubleBits);
    }

    /// <summary>Generates a CBOR object from a 32-bit floating-point
    /// number.</summary>
    /// <param name='value'>A 32-bit floating-point number.</param>
    /// <returns>A CBOR object.</returns>
    [Obsolete("Use FromFloat instead.")]
    public static CBORObject FromObject(float value) => FromSingle(value);

    /// <summary>Generates a CBOR object from a 64-bit floating-point
    /// number. The input value can be a not-a-number (NaN) value (such as
    /// <c>Double.NaN</c> ); however, NaN values have multiple forms that
    /// are equivalent for many applications' purposes, and
    /// <c>Double.NaN</c> is only one of these equivalent forms. In fact,
    /// <c>CBORObject.FromDouble(Double.NaN)</c> could produce a
    /// CBOR-encoded object that differs between DotNet and Java, because
    /// <c>Double.NaN</c> may have a different form in DotNet and Java (for
    /// example, the NaN value's sign may be negative in DotNet, but
    /// positive in Java).</summary>
    /// <param name='value'>The parameter <paramref name='value'/> is a
    /// 64-bit floating-point number.</param>
    /// <returns>A CBOR object generated from the given number.</returns>
    public static CBORObject FromDouble(double value) {
      long doubleBits = CBORUtilities.DoubleToInt64Bits(value);
      return new CBORObject(CBORObjectTypeDouble, doubleBits);
    }

    /// <summary>Generates a CBOR object from a 64-bit floating-point
    /// number.</summary>
    /// <param name='value'>A 64-bit floating-point number.</param>
    /// <returns>A CBOR object generated from the given number.</returns>
    [Obsolete("Use FromDouble instead.")]
    public static CBORObject FromObject(double value) => FromDouble(value);

    /// <summary>Generates a CBOR object from an array of 8-bit bytes; the
    /// byte array is copied to a new byte array in this process. (This
    /// method can't be used to decode CBOR data from a byte array; for
    /// that, use the <b>DecodeFromBytes</b> method instead.).</summary>
    /// <param name='bytes'>An array of 8-bit bytes; can be null.</param>
    /// <returns>A CBOR object where each element of the given byte array
    /// is copied to a new array, or CBORObject.Null if the value is
    /// null.</returns>
    public static CBORObject FromByteArray(byte[] bytes) {
      if (bytes == null) {
        return CBORObject.Null;
      }
      var newvalue = new byte[bytes.Length];
      Array.Copy(bytes, 0, newvalue, 0, bytes.Length);
      return new CBORObject(CBORObjectTypeByteString, bytes);
    }

    /// <summary>Generates a CBOR object from an array of 8-bit
    /// bytes.</summary>
    /// <param name='bytes'>An array of 8-bit bytes; can be null.</param>
    /// <returns>A CBOR object where each element of the given byte array
    /// is copied to a new array, or CBORObject.Null if the value is
    /// null.</returns>
    [Obsolete("Use FromByteArray instead.")]
    public static CBORObject FromObject(byte[] bytes) => FromByteArray(bytes);

    /// <summary>Generates a CBOR object from an array of CBOR
    /// objects.</summary>
    /// <param name='array'>An array of CBOR objects.</param>
    /// <returns>A CBOR object where each element of the given array is
    /// copied to a new array, or CBORObject.Null if the value is
    /// null.</returns>
    public static CBORObject FromCBORArray(CBORObject[] array) {
      if (array == null) {
        return Null;
      }
      IList<CBORObject> list = new List<CBORObject>();
      foreach (CBORObject cbor in array) {
        list.Add(cbor);
      }
      return new CBORObject(CBORObjectTypeArray, list);
    }

    /// <summary>Generates a CBOR object from an array of CBOR
    /// objects.</summary>
    /// <param name='array'>An array of CBOR objects.</param>
    /// <returns>A CBOR object.</returns>
    [Obsolete("Use FromCBORArray instead.")]
    public static CBORObject FromObject(CBORObject[] array) =>
FromCBORArray(array);

    internal static CBORObject FromArrayBackedObject(CBORObject[] array) {
      if (array == null) {
        return CBORObject.Null;
      }
      IList<CBORObject> list = PropertyMap.ListFromArray(array);
      return new CBORObject(CBORObjectTypeArray, list);
    }

    /// <summary>Generates a CBOR object from an array of 32-bit
    /// integers.</summary>
    /// <param name='array'>An array of 32-bit integers.</param>
    /// <returns>A CBOR array object where each element of the given array
    /// is copied to a new array, or CBORObject.Null if the value is
    /// null.</returns>
    public static CBORObject FromObject(int[] array) {
      if (array == null) {
        return CBORObject.Null;
      }
      IList<CBORObject> list = new List<CBORObject>(array.Length ==
        Int32.MaxValue ? array.Length : (array.Length + 1));
      foreach (int i in array) {
        list.Add(FromInt32(i));
      }
      return new CBORObject(CBORObjectTypeArray, list);
    }

    /// <summary>Generates a CBOR object from an array of 64-bit
    /// integers.</summary>
    /// <param name='array'>An array of 64-bit integers.</param>
    /// <returns>A CBOR array object where each element of the given array
    /// is copied to a new array, or CBORObject.Null if the value is
    /// null.</returns>
    public static CBORObject FromObject(long[] array) {
      if (array == null) {
        return CBORObject.Null;
      }
      IList<CBORObject> list = new List<CBORObject>(array.Length ==
        Int32.MaxValue ? array.Length : (array.Length + 1));
      foreach (long i in array) {
        list.Add(FromInt64(i));
      }
      return new CBORObject(CBORObjectTypeArray, list);
    }

    /// <summary>Generates a CBORObject from an arbitrary object. See the
    /// overload of this method that takes CBORTypeMapper and PODOptions
    /// arguments.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object, which can be null.
    /// <para><b>NOTE:</b> For security reasons, whenever possible, an
    /// application should not base this parameter on user input or other
    /// externally supplied data, and whenever possible, the application
    /// should limit this parameter's inputs to types specially handled by
    /// this method (such as <c>int</c> or <c>String</c> ) and/or to
    /// plain-old-data types (POCO or POJO types) within the control of the
    /// application. If the plain-old-data type references other data
    /// types, those types should likewise meet either criterion
    /// above.</para>.</param>
    /// <returns>A CBOR object corresponding to the given object. Returns
    /// CBORObject.Null if the object is null.</returns>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static CBORObject FromObject(object obj) {
      return FromObject(obj, PODOptions.Default);
    }

    /// <summary>Generates a CBORObject from an arbitrary object. See the
    /// overload of this method that takes CBORTypeMapper and PODOptions
    /// arguments.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.
    /// <para><b>NOTE:</b> For security reasons, whenever possible, an
    /// application should not base this parameter on user input or other
    /// externally supplied data, and whenever possible, the application
    /// should limit this parameter's inputs to types specially handled by
    /// this method (such as <c>int</c> or <c>String</c> ) and/or to
    /// plain-old-data types (POCO or POJO types) within the control of the
    /// application. If the plain-old-data type references other data
    /// types, those types should likewise meet either criterion
    /// above.</para>.</param>
    /// <param name='options'>An object containing options to control how
    /// certain objects are converted to CBOR objects.</param>
    /// <returns>A CBOR object corresponding to the given object. Returns
    /// CBORObject.Null if the object is null.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='options'/> is null.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static CBORObject FromObject(
      object obj,
      PODOptions options) {
      return FromObject(obj, options, null, 0);
    }

    /// <summary>Generates a CBORObject from an arbitrary object. See the
    /// overload of this method that takes CBORTypeMapper and PODOptions
    /// arguments.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.
    /// <para><b>NOTE:</b> For security reasons, whenever possible, an
    /// application should not base this parameter on user input or other
    /// externally supplied data, and whenever possible, the application
    /// should limit this parameter's inputs to types specially handled by
    /// this method (such as <c>int</c> or <c>String</c> ) and/or to
    /// plain-old-data types (POCO or POJO types) within the control of the
    /// application. If the plain-old-data type references other data
    /// types, those types should likewise meet either criterion
    /// above.</para>.</param>
    /// <param name='mapper'>An object containing optional converters to
    /// convert objects of certain types to CBOR objects.</param>
    /// <returns>A CBOR object corresponding to the given object. Returns
    /// CBORObject.Null if the object is null.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='mapper'/> is null.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static CBORObject FromObject(
      object obj,
      CBORTypeMapper mapper) {
      return mapper == null ? throw new
        ArgumentNullException(nameof(mapper)) :
        FromObject(obj, PODOptions.Default, mapper, 0);
    }

    /// <summary><para>Generates a CBORObject from an arbitrary object,
    /// using the given options to control how certain objects are
    /// converted to CBOR objects. The following cases are checked in the
    /// logical order given (rather than the strict order in which they are
    /// implemented by this library):</para>
    ///  <list><item><c>null</c>
    ///  is
    /// converted to <c>CBORObject.Null</c>
    ///  .</item>
    ///  <item>A
    /// <c>CBORObject</c>
    ///  is returned as itself.</item>
    ///  <item>If the object
    /// is of a type corresponding to a type converter mentioned in the
    /// <paramref name='mapper'/> parameter, that converter will be used to
    /// convert the object to a CBOR object. Type converters can be used to
    /// override the default conversion behavior of almost any
    /// object.</item>
    ///  <item>A <c>char</c>
    ///  is converted to an integer (from
    /// 0 through 65535), and returns a CBOR object of that integer. (This
    /// is a change in version 4.0 from previous versions, which converted
    /// <c>char</c>
    ///  , except surrogate code points from 0xd800 through
    /// 0xdfff, into single-character text strings.)</item>
    ///  <item>A
    /// <c>bool</c>
    ///  ( <c>boolean</c>
    ///  in Java) is converted to
    /// <c>CBORObject.True</c>
    ///  or <c>CBORObject.False</c>
    ///  .</item>
    ///  <item>A
    /// <c>byte</c>
    ///  is converted to a CBOR integer from 0 through
    /// 255.</item>
    ///  <item>A primitive integer type ( <c>int</c>
    ///  ,
    /// <c>short</c>
    ///  , <c>long</c>
    ///  , as well as <c>sbyte</c>
    ///  ,
    /// <c>ushort</c>
    ///  , <c>uint</c>
    ///  , and <c>ulong</c>
    ///  in.NET) is converted
    /// to the corresponding CBOR integer.</item>
    ///  <item>A primitive
    /// floating-point type ( <c>float</c>
    ///  , <c>double</c>
    ///  , as well as
    /// <c>decimal</c>
    ///  in.NET) is converted to the corresponding CBOR
    /// number.</item>
    ///  <item>A <c>String</c>
    ///  is converted to a CBOR text
    /// string. To create a CBOR byte string object from <c>String</c>
    ///  ,
    /// see the example given in <see
    /// cref='PeterO.Cbor.CBORObject.FromObject(System.Byte[])'/>.</item>
    /// <item>In the.NET version, a nullable is converted to
    /// <c>CBORObject.Null</c>
    ///  if the nullable's value is <c>null</c>
    ///  , or
    /// converted according to the nullable's underlying type, if that type
    /// is supported by this method.</item>
    ///  <item>In the Java version, a
    /// number of type <c>BigInteger</c>
    ///  or <c>BigDecimal</c>
    ///  is converted
    /// to the corresponding CBOR number.</item>
    ///  <item>A number of type
    /// <c>EDecimal</c>
    ///  , <c>EFloat</c>
    ///  , <c>EInteger</c>
    ///  , and
    /// <c>ERational</c>
    ///  in the <a
    /// href='https://www.nuget.org/packages/PeterO.Numbers'><c>PeterO.Numbers</c>
    /// </a>
    ///  library (in .NET) or the <a
    /// href='https://github.com/peteroupc/numbers-java'><c>com.github.peteroupc/numbers</c>
    /// </a>
    ///  artifact (in Java) is converted to the corresponding CBOR
    /// number.</item>
    ///  <item>An array other than <c>byte[]</c>
    ///  is converted
    /// to a CBOR array. In the.NET version, a multidimensional array is
    /// converted to an array of arrays.</item>
    ///  <item>A <c>byte[]</c>
    /// (1-dimensional byte array) is converted to a CBOR byte string; the
    /// byte array is copied to a new byte array in this process. (This
    /// method can't be used to decode CBOR data from a byte array; for
    /// that, use the <b>DecodeFromBytes</b>
    ///  method instead.)</item>
    /// <item>An object implementing IDictionary (Map in Java) is converted
    /// to a CBOR map containing the keys and values enumerated.</item>
    /// <item>An object implementing IEnumerable (Iterable in Java) is
    /// converted to a CBOR array containing the items enumerated.</item>
    /// <item>An enumeration ( <c>Enum</c>
    ///  ) object is converted to its
    /// <i>underlying value</i>
    ///  in the.NET version, or the result of its
    /// <c>ordinal()</c>
    ///  method in the Java version.</item>
    ///  <item>An object
    /// of type <c>DateTime</c>
    ///  , <c>Uri</c>
    ///  , or <c>Guid</c>
    ///  ( <c>Date</c>
    /// , <c>URI</c>
    ///  , or <c>UUID</c>
    ///  , respectively, in Java) will be
    /// converted to a tagged CBOR object of the appropriate kind. By
    /// default, <c>DateTime</c>
    ///  / <c>Date</c>
    ///  will be converted to a tag-0
    /// string following the date format used in the Atom syndication
    /// format, but this behavior can be changed by passing a suitable
    /// CBORTypeMapper to this method, such as a CBORTypeMapper that
    /// registers a CBORDateConverter for <c>DateTime</c>
    ///  or <c>Date</c>
    /// objects. See the examples.</item>
    ///  <item>If the object is a type not
    /// specially handled above, this method checks the <paramref
    /// name='obj'/> parameter for eligible getters as follows:</item>
    /// <item>(*) In the .NET version, eligible getters are the public,
    /// nonstatic getters of read/write properties (and also those of
    /// read-only properties in the case of a compiler-generated type or an
    /// F# type). Eligible getters also include public, nonstatic, non-
    /// <c>const</c>
    ///  , non- <c>readonly</c>
    ///  fields. If a class has two
    /// properties and/or fields of the form "X" and "IsX", where "X" is
    /// any name, or has multiple properties and/or fields with the same
    /// name, those properties and fields are ignored.</item>
    ///  <item>(*) In
    /// the Java version, eligible getters are public, nonstatic methods
    /// starting with "get" or "is" (either word followed by a character
    /// other than a basic digit or lower-case letter, that is, other than
    /// "a" to "z" or "0" to "9"), that take no parameters and do not
    /// return void, except that methods named "getClass" are not eligible
    /// getters. In addition, public, nonstatic, nonfinal fields are also
    /// eligible getters. If a class has two otherwise eligible getters
    /// (methods and/or fields) of the form "isX" and "getX", where "X" is
    /// the same in both, or two such getters with the same name but
    /// different return type, they are not eligible getters.</item>
    /// <item>Then, the method returns a CBOR map with each eligible
    /// getter's name or property name as each key, and with the
    /// corresponding value returned by that getter as that key's value.
    /// Before adding a key-value pair to the map, the key's name is
    /// adjusted according to the rules described in the <see
    /// cref='PeterO.Cbor.PODOptions'/> documentation. Note that for
    /// security reasons, certain types are not supported even if they
    /// contain eligible getters.</item>
    ///  </list>
    ///  <para><b>REMARK:</b>
    ///  .NET
    /// enumeration ( <c>Enum</c>
    ///  ) constants could also have been
    /// converted to text strings with <c>ToString()</c>
    ///  , but that method
    /// will return multiple names if the given Enum object is a
    /// combination of Enum objects (e.g. if the object is
    /// <c>FileAccess.Read | FileAccess.Write</c>
    ///  ). More generally, if
    /// Enums are converted to text strings, constants from Enum types with
    /// the <c>Flags</c>
    ///  attribute, and constants from the same Enum type
    /// that share an underlying value, should not be passed to this
    /// method.</para>
    ///  </summary>
    /// <param name='obj'>An arbitrary object to convert to a CBOR object.
    /// <para><b>NOTE:</b>
    ///  For security reasons, whenever possible, an
    /// application should not base this parameter on user input or other
    /// externally supplied data, and whenever possible, the application
    /// should limit this parameter's inputs to types specially handled by
    /// this method (such as <c>int</c>
    ///  or <c>String</c>
    ///  ) and/or to
    /// plain-old-data types (POCO or POJO types) within the control of the
    /// application. If the plain-old-data type references other data
    /// types, those types should likewise meet either criterion
    /// above.</para>
    /// .</param>
    /// <param name='mapper'>An object containing optional converters to
    /// convert objects of certain types to CBOR objects. Can be
    /// null.</param>
    /// <param name='options'>An object containing options to control how
    /// certain objects are converted to CBOR objects.</param>
    /// <returns>A CBOR object corresponding to the given object. Returns
    /// CBORObject.Null if the object is null.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='options'/> is null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>An error occurred while
    /// converting the given object to a CBOR object.</exception>
    /// <example>
    /// <para>The following example (originally written in C# for the
    /// DotNet version) uses a CBORTypeMapper to change how DateTime
    /// objects are converted to CBOR. In this case, such objects are
    /// converted to CBOR objects with tag 1 that store numbers giving the
    /// number of seconds since the start of 1970.</para>
    /// <code>var conv = new CBORTypeMapper().AddConverter(typeof(DateTime),
    /// CBORDateConverter.TaggedNumber);
    /// CBORObject obj = CBORObject.FromObject(DateTime.Now, conv);</code>
    /// <para>The following example generates a CBOR object from a 64-bit
    /// signed integer that is treated as a 64-bit unsigned integer (such
    /// as DotNet's UInt64, which has no direct equivalent in the Java
    /// language), in the sense that the value is treated as 2^64 plus the
    /// original value if it's negative.</para>
    /// <code>long x = -40L; &#x2f;&#x2a; Example 64-bit value treated as 2^64-40.&#x2a;&#x2f;
    /// CBORObject obj = CBORObject.FromObject(
    /// v &lt; 0 ? EInteger.FromInt32(1).ShiftLeft(64).Add(v) :
    /// EInteger.FromInt64(v));</code>
    /// <para>In the Java version, which has java.math.BigInteger, the
    /// following can be used instead:</para>
    /// <code>long x = -40L; &#x2f;&#x2a; Example 64-bit value treated as 2^64-40.&#x2a;&#x2f;
    /// CBORObject obj = CBORObject.FromObject(
    /// v &lt; 0 ? BigInteger.valueOf(1).shiftLeft(64).add(BigInteger.valueOf(v)) :
    /// BigInteger.valueOf(v));</code>
    /// </example>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static CBORObject FromObject(
      object obj,
      CBORTypeMapper mapper,
      PODOptions options) {
      return mapper == null ? throw new
ArgumentNullException(nameof(mapper)) : FromObject(obj, options, mapper, 0);
    }

    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    internal static CBORObject FromObject(
      object obj,
      PODOptions options,
      CBORTypeMapper mapper,
      int depth) {
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      if (depth >= 100) {
        throw new CBORException("Nesting depth too high");
      }
      if (obj == null) {
        return Null;
      }
      if (obj is CBORObject) {
        return (CBORObject)obj;
      }
      CBORObject objret;
      if (mapper != null) {
        objret = mapper.ConvertWithConverter(obj);
        if (objret != null) {
          return objret;
        }
      }
      if (obj is string) {
        return FromString((string)obj);
      }
      if (obj is int) {
        return FromInt32((int)obj);
      }
      if (obj is long) {
        return FromInt64((long)obj);
      }
      if (obj is EInteger eif) {
        return FromEInteger(eif);
      }
      if (obj is EDecimal edf) {
        return FromEDecimal(edf);
      }
      if (obj is EFloat eff) {
        return FromEFloat(eff);
      }
      if (obj is ERational erf) {
        return FromERational(erf);
      }
      if (obj is short) {
        return FromInt16((short)obj);
      }
      if (obj is char) {
        return FromInt32((int)(char)obj);
      }
      if (obj is bool) {
        return FromBool((bool)obj);
      }
      if (obj is byte) {
        return FromByte((byte)obj);
      }
      if (obj is float) {
        return FromSingle((float)obj);
      }
      if (obj is sbyte) {
        return FromSbyte((sbyte)obj);
      }
      if (obj is ulong) {
        return FromUInt64((ulong)obj);
      }
      if (obj is uint) {
        return FromUInt((uint)obj);
      }
      if (obj is ushort) {
        return FromUShort((ushort)obj);
      }
      if (obj is decimal) {
        return FromDecimal((decimal)obj);
      }
      if (obj is double) {
        return FromDouble((double)obj);
      }
      if (obj is byte[] bytearr) {
        return FromByteArray(bytearr);
      }
      if (obj is System.Collections.IDictionary) {
        // IDictionary appears first because IDictionary includes IEnumerable
        objret = CBORObject.NewMap();
        var objdic =
          (System.Collections.IDictionary)obj;
        foreach (object keyPair in objdic) {
          var kvp = (System.Collections.DictionaryEntry)keyPair;
          var objKey = CBORObject.FromObject(
              kvp.Key,
              options,
              mapper,
              depth + 1);
          objret[objKey] = CBORObject.FromObject(
              kvp.Value,
              options,
              mapper,
              depth + 1);
        }
        return objret;
      }
      if (obj is Array) {
        return PropertyMap.FromArray(obj, options, mapper, depth);
      }
      if (obj is System.Collections.IEnumerable) {
        objret = CBORObject.NewArray();
        foreach (object element in (System.Collections.IEnumerable)obj) {
          _ = objret.Add(
            CBORObject.FromObject(
              element,
              options,
              mapper,
              depth + 1));
        }
        return objret;
      }
      if (obj is Enum) {
        return FromObject(PropertyMap.EnumToObjectAsInteger((Enum)obj));
      }
      if (obj is DateTime) {
        return new CBORDateConverter().ToCBORObject((DateTime)obj);
      }
      if (obj is Uri) {
        return new CBORUriConverter().ToCBORObject((Uri)obj);
      }
      if (obj is Guid) {
        return new CBORUuidConverter().ToCBORObject((Guid)obj);
      }
      objret = PropertyMap.FromObjectOther(obj);
      if (objret != null) {
        return objret;
      }
      objret = CBORObject.NewMap();
      foreach (KeyValuePair<string, object> key in
        PropertyMap.GetProperties(
          obj,
          options.UseCamelCase)) {
        objret[key.Key] = CBORObject.FromObject(
            key.Value,
            options,
            mapper,
            depth + 1);
      }
      return objret;
    }

    /// <summary>Generates a CBOR object from this one, but gives the
    /// resulting object a tag in addition to its existing tags (the new
    /// tag is made the outermost tag).</summary>
    /// <param name='bigintTag'>Tag number. The tag number 55799 can be
    /// used to mark a "self-described CBOR" object. This document does not
    /// attempt to list all CBOR tags and their meanings. An up-to-date
    /// list can be found at the CBOR Tags registry maintained by the
    /// Internet Assigned Numbers Authority(
    /// <i>iana.org/assignments/cbor-tags</i> ).</param>
    /// <returns>A CBOR object with the same value as this one but given
    /// the tag <paramref name='bigintTag'/> in addition to its existing
    /// tags (the new tag is made the outermost tag).</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='bigintTag'/> is less than 0 or greater than
    /// 2^64-1.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintTag'/> is null.</exception>
    public CBORObject WithTag(EInteger bigintTag) {
      if (bigintTag == null) {
        throw new ArgumentNullException(nameof(bigintTag));
      }
      if (bigintTag.Sign < 0) {
        throw new ArgumentException("tagEInt's sign(" + bigintTag.Sign +
          ") is less than 0");
      }
      if (bigintTag.CanFitInInt32()) {
        // Low-numbered, commonly used tags
        return this.WithTag(bigintTag.ToInt32Checked());
      } else {
        if (bigintTag.CompareTo(UInt64MaxValue) > 0) {
          throw new ArgumentException(
            "tag more than 18446744073709551615");
        }
        var tagLow = 0;
        var tagHigh = 0;
        byte[] bytes = bigintTag.ToBytes(true);
        for (int i = 0; i < Math.Min(4, bytes.Length); ++i) {
          int b = bytes[i] & 0xff;
          tagLow = unchecked(tagLow | (b << (i * 8)));
        }
        for (int i = 4; i < Math.Min(8, bytes.Length); ++i) {
          int b = bytes[i] & 0xff;
          tagHigh = unchecked(tagHigh | (b << (i * 8)));
        }
        return new CBORObject(this, tagLow, tagHigh);
      }
    }

    /// <summary>Generates a CBOR object from an arbitrary object and gives
    /// the resulting object a tag in addition to its existing tags (the
    /// new tag is made the outermost tag).</summary>
    /// <param name='cborObj'>The parameter <paramref name='cborObj'/> is a
    /// CBORObject.
    /// <para><b>NOTE:</b> For security reasons, whenever possible, an
    /// application should not base this parameter on user input or other
    /// externally supplied data, and whenever possible, the application
    /// should limit this parameter's inputs to types specially handled by
    /// this method (such as <c>int</c> or <c>String</c> ) and/or to
    /// plain-old-data types (POCO or POJO types) within the control of the
    /// application. If the plain-old-data type references other data
    /// types, those types should likewise meet either criterion
    /// above.</para>.</param>
    /// <param name='bigintTag'>Tag number. The tag number 55799 can be
    /// used to mark a "self-described CBOR" object. This document does not
    /// attempt to list all CBOR tags and their meanings. An up-to-date
    /// list can be found at the CBOR Tags registry maintained by the
    /// Internet Assigned Numbers Authority(
    /// <i>iana.org/assignments/cbor-tags</i> ).</param>
    /// <returns>A CBOR object where the object <paramref name='cborObj'/>
    /// is given the tag <paramref name='bigintTag'/>.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='bigintTag'/> is less than 0 or greater than
    /// 2^64-1.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintTag'/> is null.</exception>
    public static CBORObject FromCBORObjectAndTag(
      CBORObject cborObj,
      EInteger bigintTag) {
      if (cborObj == null) {
        throw new ArgumentNullException(nameof(cborObj));
      }
      return bigintTag == null ?
        throw new ArgumentNullException(nameof(bigintTag)) :
        bigintTag.Sign < 0 ?
        throw new ArgumentException("tagEInt's sign(" + bigintTag.Sign +
          ") is less than 0") : bigintTag.CompareTo(UInt64MaxValue) > 0 ?
        throw new ArgumentException(
          "tag more than 18446744073709551615") :
          cborObj.WithTag(bigintTag);
    }

    /// <summary>Generates a CBOR object from an arbitrary object and gives
    /// the resulting object a tag in addition to its existing tags (the
    /// new tag is made the outermost tag).</summary>
    /// <param name='valueObValue'>An arbitrary object, which can be
    /// null.</param>
    /// <param name='bigintTag'>Tag number.</param>
    /// <returns>A CBOR object where the object <paramref
    /// name='valueObValue'/> is given the tag <paramref
    /// name='bigintTag'/>.</returns>
    [Obsolete("Use FromCBORObjectAndTag instead.")]
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static CBORObject FromObjectAndTag(object valueObValue, EInteger
bigintTag) =>
      FromCBORObjectAndTag(FromObject(valueObValue), bigintTag);

    /// <summary>Generates a CBOR object from an arbitrary object and gives
    /// the resulting object a tag in addition to its existing tags (the
    /// new tag is made the outermost tag).</summary>
    /// <param name='smallTag'>A 32-bit integer that specifies a tag
    /// number. The tag number 55799 can be used to mark a "self-described
    /// CBOR" object. This document does not attempt to list all CBOR tags
    /// and their meanings. An up-to-date list can be found at the CBOR
    /// Tags registry maintained by the Internet Assigned Numbers Authority
    /// (
    /// <i>iana.org/assignments/cbor-tags</i> ).</param>
    /// <returns>A CBOR object with the same value as this one but given
    /// the tag <paramref name='smallTag'/> in addition to its existing
    /// tags (the new tag is made the outermost tag).</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='smallTag'/> is less than 0.</exception>
    public CBORObject WithTag(int smallTag) {
      return smallTag < 0 ? throw new ArgumentException("smallTag(" + smallTag +
          ") is less than 0") : new CBORObject(this, smallTag, 0);
    }

    /// <summary>Generates a CBOR object from an arbitrary object and gives
    /// the resulting object a tag in addition to its existing tags (the
    /// new tag is made the outermost tag).</summary>
    /// <param name='cborObj'>The parameter <paramref name='cborObj'/> is a
    /// CBORObject.
    /// <para><b>NOTE:</b> For security reasons, whenever possible, an
    /// application should not base this parameter on user input or other
    /// externally supplied data, and whenever possible, the application
    /// should limit this parameter's inputs to types specially handled by
    /// this method (such as <c>int</c> or <c>String</c> ) and/or to
    /// plain-old-data types (POCO or POJO types) within the control of the
    /// application. If the plain-old-data type references other data
    /// types, those types should likewise meet either criterion
    /// above.</para>.</param>
    /// <param name='smallTag'>A 32-bit integer that specifies a tag
    /// number. The tag number 55799 can be used to mark a "self-described
    /// CBOR" object. This document does not attempt to list all CBOR tags
    /// and their meanings. An up-to-date list can be found at the CBOR
    /// Tags registry maintained by the Internet Assigned Numbers Authority
    /// (
    /// <i>iana.org/assignments/cbor-tags</i> ).</param>
    /// <returns>A CBOR object where the object <paramref name='cborObj'/>
    /// is given the tag <paramref name='smallTag'/>. If "valueOb" is
    /// null, returns a version of CBORObject.Null with the given
    /// tag.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='smallTag'/> is less than 0.</exception>
    public static CBORObject FromCBORObjectAndTag(
      CBORObject cborObj,
      int smallTag) {
      return smallTag < 0 ? throw new ArgumentException("smallTag(" + smallTag +
          ") is less than 0") : cborObj.WithTag(smallTag);
    }

    /// <summary>Generates a CBOR object from an arbitrary object and gives
    /// the resulting object a tag in addition to its existing tags (the
    /// new tag is made the outermost tag).</summary>
    /// <param name='valueObValue'>The parameter, an arbitrary object,
    /// which can be null.</param>
    /// <param name='smallTag'>A 32-bit integer that specifies a tag
    /// number.</param>
    /// <returns>A CBOR object where the object <paramref
    /// name='valueObValue'/> is given the tag <paramref
    /// name='smallTag'/>.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='smallTag'/> is less than 0.</exception>
    [Obsolete("Use FromCBORObjectAndTag instead.")]
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static CBORObject FromObjectAndTag(object valueObValue, int
smallTag) =>
      FromCBORObjectAndTag(FromObject(valueObValue), smallTag);

    /// <summary>Creates a CBOR object from a simple value
    /// number.</summary>
    /// <param name='simpleValue'>The parameter <paramref
    /// name='simpleValue'/> is a 32-bit signed integer.</param>
    /// <returns>A CBOR object.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='simpleValue'/> is less than 0, greater than 255, or from 24
    /// through 31.</exception>
    public static CBORObject FromSimpleValue(int simpleValue) {
      if (simpleValue < 0) {
        throw new ArgumentException("simpleValue(" + simpleValue +
          ") is less than 0");
      }
      if (simpleValue > 255) {
        throw new ArgumentException("simpleValue(" + simpleValue +
          ") is more than " + "255");
      }
      return simpleValue >= 24 && simpleValue < 32 ?
        throw new ArgumentException("Simple value is from 24 to 31 - " +
          simpleValue) : simpleValue < 32 ?
        FixedObjects[0xe0 + simpleValue] : new CBORObject(
          CBORObjectTypeSimpleValue,
          simpleValue);
    }

    /// <summary>Creates a new empty CBOR array.</summary>
    /// <returns>A new CBOR array.</returns>
    public static CBORObject NewArray() {
      return new CBORObject(CBORObjectTypeArray, new List<CBORObject>());
    }

    internal static CBORObject NewArray(CBORObject o1, CBORObject o2) {
      var list = new List<CBORObject>(2) {
        o1,
        o2,
      };
      return new CBORObject(CBORObjectTypeArray, list);
    }

    internal static CBORObject NewArray(
      CBORObject o1,
      CBORObject o2,
      CBORObject o3) {
      var list = new List<CBORObject>(2) {
        o1,
        o2,
        o3,
      };
      return new CBORObject(CBORObjectTypeArray, list);
    }

    /// <summary>Creates a new empty CBOR map that stores its keys in an
    /// undefined order.</summary>
    /// <returns>A new CBOR map.</returns>
    public static CBORObject NewMap() {
      return new CBORObject(
          CBORObjectTypeMap,
          new SortedDictionary<CBORObject, CBORObject>());
    }

    /// <summary>Creates a new CBOR map that stores its keys in an
    /// undefined order.</summary>
    /// <param name='keysAndValues'>A sequence of key-value pairs.</param>
    /// <returns>A new CBOR map.</returns>
    public static CBORObject FromMap(
      IEnumerable<Tuple<CBORObject, CBORObject>> keysAndValues) {
      var sd = new SortedDictionary<CBORObject, CBORObject>();
      foreach (Tuple<CBORObject, CBORObject> kv in keysAndValues) {
        sd.Add(kv.Item1, kv.Item2);
      }
      return new CBORObject(
          CBORObjectTypeMap,
          sd);
    }

    /// <summary>Creates a new empty CBOR map that ensures that keys are
    /// stored in the order in which they are first inserted.</summary>
    /// <returns>A new CBOR map.</returns>
    public static CBORObject NewOrderedMap() {
      return new CBORObject(
          CBORObjectTypeMap,
          PropertyMap.NewOrderedDict());
    }

    /// <summary>Creates a new CBOR map that ensures that keys are stored
    /// in order.</summary>
    /// <param name='keysAndValues'>A sequence of key-value pairs.</param>
    /// <returns>A new CBOR map.</returns>
    public static CBORObject FromOrderedMap(
      IEnumerable<Tuple<CBORObject, CBORObject>> keysAndValues) {
      IDictionary<CBORObject, CBORObject> oDict;
      oDict = PropertyMap.NewOrderedDict();
      foreach (Tuple<CBORObject, CBORObject> kv in keysAndValues) {
        oDict.Add(kv.Item1, kv.Item2);
      }
      return new CBORObject(
          CBORObjectTypeMap,
          oDict);
    }

    /// <summary>
    /// <para>Reads a sequence of objects in CBOR format from a data
    /// stream. This method will read CBOR objects from the stream until
    /// the end of the stream is reached or an error occurs, whichever
    /// happens first.</para></summary>
    /// <param name='stream'>A readable data stream.</param>
    /// <returns>An array containing the CBOR objects that were read from
    /// the data stream. Returns an empty array if there is no unread data
    /// in the stream.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null, or the parameter "options" is
    /// null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data, including if the last CBOR object was
    /// read only partially.</exception>
    public static CBORObject[] ReadSequence(Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      var cborList = new List<CBORObject>();
      while (true) {
        CBORObject obj = Read(stream, AllowEmptyOptions);
        if (obj == null) {
          break;
        }
        cborList.Add(obj);
      }
      return cborList.ToArray();
    }

    /// <summary>
    /// <para>Reads a sequence of objects in CBOR format from a data
    /// stream. This method will read CBOR objects from the stream until
    /// the end of the stream is reached or an error occurs, whichever
    /// happens first.</para></summary>
    /// <param name='stream'>A readable data stream.</param>
    /// <param name='options'>Specifies the options to use when decoding
    /// the CBOR data stream. See CBOREncodeOptions for more information.
    /// In this method, the AllowEmpty property is treated as set
    /// regardless of the value of that property specified in this
    /// parameter.</param>
    /// <returns>An array containing the CBOR objects that were read from
    /// the data stream. Returns an empty array if there is no unread data
    /// in the stream.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null, or the parameter <paramref
    /// name='options'/> is null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data, including if the last CBOR object was
    /// read only partially.</exception>
    public static CBORObject[] ReadSequence(Stream stream, CBOREncodeOptions
      options) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      CBOREncodeOptions opt = options;
      if (!opt.AllowEmpty) {
        opt = new CBOREncodeOptions(opt.ToString() + ";allowempty=1");
      }
      var cborList = new List<CBORObject>();
      while (true) {
        CBORObject obj = Read(stream, opt);
        if (obj == null) {
          break;
        }
        cborList.Add(obj);
      }
      return cborList.ToArray();
    }

    /// <summary>
    /// <para>Reads an object in CBOR format from a data stream. This
    /// method will read from the stream until the end of the CBOR object
    /// is reached or an error occurs, whichever happens
    /// first.</para></summary>
    /// <param name='stream'>A readable data stream.</param>
    /// <returns>A CBOR object that was read.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data.</exception>
    public static CBORObject Read(Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      try {
        var reader = new CBORReader(stream);
        return reader.Read();
      } catch (IOException ex) {
        throw new CBORException("I/O error occurred.", ex);
      }
    }

    /// <summary>Reads an object in CBOR format from a data stream, using
    /// the specified options to control the decoding process. This method
    /// will read from the stream until the end of the CBOR object is
    /// reached or an error occurs, whichever happens first.</summary>
    /// <param name='stream'>A readable data stream.</param>
    /// <param name='options'>Specifies the options to use when decoding
    /// the CBOR data stream. See CBOREncodeOptions for more
    /// information.</param>
    /// <returns>A CBOR object that was read.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data.</exception>
    public static CBORObject Read(Stream stream, CBOREncodeOptions options) {
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      try {
        var reader = new CBORReader(stream, options);
        return reader.Read();
      } catch (IOException ex) {
        throw new CBORException("I/O error occurred.", ex);
      }
    }

    /// <summary>Generates a CBOR object from a data stream in JavaScript
    /// Object Notation (JSON) format. The JSON stream may begin with a
    /// byte-order mark (U+FEFF). Since version 2.0, the JSON stream can be
    /// in UTF-8, UTF-16, or UTF-32 encoding; the encoding is detected by
    /// assuming that the first character read must be a byte-order mark or
    /// a nonzero basic character (U+0001 to U+007F). (In previous
    /// versions, only UTF-8 was allowed.). (This behavior may change to
    /// supporting only UTF-8, with or without a byte order mark, in
    /// version 5.0 or later, perhaps with an option to restore the
    /// previous behavior of also supporting UTF-16 and UTF-32.).</summary>
    /// <param name='stream'>A readable data stream. The sequence of bytes
    /// read from the data stream must contain a single JSON object and not
    /// multiple objects.</param>
    /// <returns>A CBOR object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The data stream
    /// contains invalid encoding or is not in JSON format.</exception>
    public static CBORObject ReadJSON(Stream stream) {
      return ReadJSON(stream, JSONOptions.Default);
    }

    /// <summary>Generates a list of CBOR objects from a data stream in
    /// JavaScript Object Notation (JSON) text sequence format (RFC 7464).
    /// The data stream must be in UTF-8 encoding and may not begin with a
    /// byte-order mark (U+FEFF).</summary>
    /// <param name='stream'>A readable data stream. The sequence of bytes
    /// read from the data stream must either be empty or begin with a
    /// record separator byte (0x1e).</param>
    /// <returns>A list of CBOR objects read from the JSON sequence.
    /// Objects that could not be parsed are replaced with <c>null</c> (as
    /// opposed to <c>CBORObject.Null</c> ) in the given list.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The data stream is not
    /// empty and does not begin with a record separator byte
    /// (0x1e).</exception>
    /// <remarks>Generally, each JSON text in a JSON text sequence is
    /// written as follows: Write a record separator byte (0x1e), then
    /// write the JSON text in UTF-8 (without a byte order mark, U+FEFF),
    /// then write the line feed byte (0x0a). RFC 7464, however, uses a
    /// more liberal syntax for parsing JSON text sequences.</remarks>
    public static CBORObject[] ReadJSONSequence(Stream stream) {
      return ReadJSONSequence(stream, JSONOptions.Default);
    }

    /// <summary>Generates a list of CBOR objects from a data stream in
    /// JavaScript Object Notation (JSON) text sequence format (RFC 7464).
    /// The data stream must be in UTF-8 encoding and may not begin with a
    /// byte-order mark (U+FEFF).</summary>
    /// <param name='stream'>A readable data stream. The sequence of bytes
    /// read from the data stream must either be empty or begin with a
    /// record separator byte (0x1e).</param>
    /// <param name='jsonoptions'>Specifies options to control how JSON
    /// texts in the stream are decoded to CBOR. See the JSONOptions
    /// class.</param>
    /// <returns>A list of CBOR objects read from the JSON sequence.
    /// Objects that could not be parsed are replaced with <c>null</c> (as
    /// opposed to <c>CBORObject.Null</c> ) in the given list.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The data stream is not
    /// empty and does not begin with a record separator byte
    /// (0x1e).</exception>
    /// <remarks>Generally, each JSON text in a JSON text sequence is
    /// written as follows: Write a record separator byte (0x1e), then
    /// write the JSON text in UTF-8 (without a byte order mark, U+FEFF),
    /// then write the line feed byte (0x0a). RFC 7464, however, uses a
    /// more liberal syntax for parsing JSON text sequences.</remarks>
    public static CBORObject[] ReadJSONSequence(Stream stream, JSONOptions
      jsonoptions) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (jsonoptions == null) {
        throw new ArgumentNullException(nameof(jsonoptions));
      }
      CharacterInputWithCount reader;
      reader = new CharacterInputWithCount(
        new CharacterReader(stream, 0, true, true));
      try {
        var nextchar = new int[1];
        CBORObject[] objlist = CBORJson.ParseJSONSequence(
            reader,
            jsonoptions,
            nextchar);
        if (nextchar[0] != -1) {
          reader.RaiseError("End of data stream not reached");
        }
        return objlist;
      } catch (CBORException ex) {
        if (ex.InnerException is IOException ioex) {
          throw ioex;
        }
        throw;
      }
    }

    /// <summary>Generates a CBOR object from a data stream in JavaScript
    /// Object Notation (JSON) format, using the specified options to
    /// control the decoding process. The JSON stream may begin with a
    /// byte-order mark (U+FEFF). Since version 2.0, the JSON stream can be
    /// in UTF-8, UTF-16, or UTF-32 encoding; the encoding is detected by
    /// assuming that the first character read must be a byte-order mark or
    /// a nonzero basic character (U+0001 to U+007F). (In previous
    /// versions, only UTF-8 was allowed.). (This behavior may change to
    /// supporting only UTF-8, with or without a byte order mark, in
    /// version 5.0 or later, perhaps with an option to restore the
    /// previous behavior of also supporting UTF-16 and UTF-32.).</summary>
    /// <param name='stream'>A readable data stream. The sequence of bytes
    /// read from the data stream must contain a single JSON object and not
    /// multiple objects.</param>
    /// <param name='jsonoptions'>Specifies options to control how the JSON
    /// stream is decoded to CBOR. See the JSONOptions class.</param>
    /// <returns>A CBOR object containing the JSON data decoded.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The data stream
    /// contains invalid encoding or is not in JSON format.</exception>
    public static CBORObject ReadJSON(
      Stream stream,
      JSONOptions jsonoptions) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (jsonoptions == null) {
        throw new ArgumentNullException(nameof(jsonoptions));
      }
      CharacterInputWithCount reader;
      reader = new CharacterInputWithCount(
        new CharacterReader(stream, 2, true));
      try {
        var nextchar = new int[1];
        CBORObject obj = CBORJson.ParseJSONValue(
            reader,
            jsonoptions,
            nextchar);
        if (nextchar[0] != -1) {
          reader.RaiseError("End of data stream not reached");
        }
        return obj;
      } catch (CBORException ex) {
        if (ex.InnerException is IOException ioex) {
          throw ioex;
        }
        throw;
      }
    }

    /// <summary>
    /// <para>Generates a CBOR object from a byte array in JavaScript
    /// Object Notation (JSON) format.</para>
    /// <para>If a JSON object has duplicate keys, a CBORException is
    /// thrown.</para>
    /// <para>Note that if a CBOR object is converted to JSON with
    /// <c>ToJSONBytes</c>, then the JSON is converted back to CBOR with
    /// this method, the new CBOR object will not necessarily be the same
    /// as the old CBOR object, especially if the old CBOR object uses data
    /// types not supported in JSON, such as integers in map
    /// keys.</para></summary>
    /// <param name='bytes'>A byte array in JSON format. The entire byte
    /// array must contain a single JSON object and not multiple objects.
    /// The byte array may begin with a byte-order mark (U+FEFF). The byte
    /// array can be in UTF-8, UTF-16, or UTF-32 encoding; the encoding is
    /// detected by assuming that the first character read must be a
    /// byte-order mark or a nonzero basic character (U+0001 to U+007F).
    /// (This behavior may change to supporting only UTF-8, with or without
    /// a byte order mark, in version 5.0 or later, perhaps with an option
    /// to restore the previous behavior of also supporting UTF-16 and
    /// UTF-32.).</param>
    /// <returns>A CBOR object containing the JSON data decoded.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The byte array contains
    /// invalid encoding or is not in JSON format.</exception>
    public static CBORObject FromJSONBytes(byte[] bytes) {
      // TODO: In next major version, consider supporting UTF-8 only
      return FromJSONBytes(bytes, JSONOptions.Default);
    }

    /// <summary>Generates a CBOR object from a byte array in JavaScript
    /// Object Notation (JSON) format, using the specified options to
    /// control the decoding process.
    /// <para>Note that if a CBOR object is converted to JSON with
    /// <c>ToJSONBytes</c>, then the JSON is converted back to CBOR with
    /// this method, the new CBOR object will not necessarily be the same
    /// as the old CBOR object, especially if the old CBOR object uses data
    /// types not supported in JSON, such as integers in map
    /// keys.</para></summary>
    /// <param name='bytes'>A byte array in JSON format. The entire byte
    /// array must contain a single JSON object and not multiple objects.
    /// The byte array may begin with a byte-order mark (U+FEFF). The byte
    /// array can be in UTF-8, UTF-16, or UTF-32 encoding; the encoding is
    /// detected by assuming that the first character read must be a
    /// byte-order mark or a nonzero basic character (U+0001 to U+007F).
    /// (This behavior may change to supporting only UTF-8, with or without
    /// a byte order mark, in version 5.0 or later, perhaps with an option
    /// to restore the previous behavior of also supporting UTF-16 and
    /// UTF-32.).</param>
    /// <param name='jsonoptions'>Specifies options to control how the JSON
    /// data is decoded to CBOR. See the JSONOptions class.</param>
    /// <returns>A CBOR object containing the JSON data decoded.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> or <paramref name='jsonoptions'/> is
    /// null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The byte array contains
    /// invalid encoding or is not in JSON format.</exception>
    public static CBORObject FromJSONBytes(
      byte[] bytes,
      JSONOptions jsonoptions) {
      // TODO: In next major version, consider supporting UTF-8 only
      return bytes == null ? throw new ArgumentNullException(nameof(bytes)) :
        jsonoptions == null ?
        throw new ArgumentNullException(nameof(jsonoptions)) :
        bytes.Length == 0 ? throw new CBORException("Byte array is empty") :
FromJSONBytes(bytes, 0, bytes.Length, jsonoptions);
    }

    /// <summary>
    /// <para>Generates a CBOR object from a byte array in JavaScript
    /// Object Notation (JSON) format.</para>
    /// <para>If a JSON object has duplicate keys, a CBORException is
    /// thrown.</para>
    /// <para>Note that if a CBOR object is converted to JSON with
    /// <c>ToJSONBytes</c>, then the JSON is converted back to CBOR with
    /// this method, the new CBOR object will not necessarily be the same
    /// as the old CBOR object, especially if the old CBOR object uses data
    /// types not supported in JSON, such as integers in map
    /// keys.</para></summary>
    /// <param name='bytes'>A byte array, the specified portion of which is
    /// in JSON format. The specified portion of the byte array must
    /// contain a single JSON object and not multiple objects. The portion
    /// may begin with a byte-order mark (U+FEFF). The portion can be in
    /// UTF-8, UTF-16, or UTF-32 encoding; the encoding is detected by
    /// assuming that the first character read must be a byte-order mark or
    /// a nonzero basic character (U+0001 to U+007F). (This behavior may
    /// change to supporting only UTF-8, with or without a byte order mark,
    /// in version 5.0 or later, perhaps with an option to restore the
    /// previous behavior of also supporting UTF-16 and UTF-32.).</param>
    /// <param name='offset'>An index, starting at 0, showing where the
    /// desired portion of <paramref name='bytes'/> begins.</param>
    /// <param name='count'>The length, in bytes, of the desired portion of
    /// <paramref name='bytes'/> (but not more than <paramref
    /// name='bytes'/> 's length).</param>
    /// <returns>A CBOR object containing the JSON data decoded.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The byte array contains
    /// invalid encoding or is not in JSON format.</exception>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='count'/> is less than 0 or
    /// greater than <paramref name='bytes'/> 's length, or <paramref
    /// name='bytes'/> 's length minus <paramref name='offset'/> is less
    /// than <paramref name='count'/>.</exception>
    public static CBORObject FromJSONBytes(byte[] bytes, int offset, int
      count) {
      return FromJSONBytes(bytes, offset, count, JSONOptions.Default);
    }

    /// <summary>Generates a CBOR object from a byte array in JavaScript
    /// Object Notation (JSON) format, using the specified options to
    /// control the decoding process.
    /// <para>Note that if a CBOR object is converted to JSON with
    /// <c>ToJSONBytes</c>, then the JSON is converted back to CBOR with
    /// this method, the new CBOR object will not necessarily be the same
    /// as the old CBOR object, especially if the old CBOR object uses data
    /// types not supported in JSON, such as integers in map
    /// keys.</para></summary>
    /// <param name='bytes'>A byte array, the specified portion of which is
    /// in JSON format. The specified portion of the byte array must
    /// contain a single JSON object and not multiple objects. The portion
    /// may begin with a byte-order mark (U+FEFF). The portion can be in
    /// UTF-8, UTF-16, or UTF-32 encoding; the encoding is detected by
    /// assuming that the first character read must be a byte-order mark or
    /// a nonzero basic character (U+0001 to U+007F). (This behavior may
    /// change to supporting only UTF-8, with or without a byte order mark,
    /// in version 5.0 or later, perhaps with an option to restore the
    /// previous behavior of also supporting UTF-16 and UTF-32.).</param>
    /// <param name='offset'>An index, starting at 0, showing where the
    /// desired portion of <paramref name='bytes'/> begins.</param>
    /// <param name='count'>The length, in bytes, of the desired portion of
    /// <paramref name='bytes'/> (but not more than <paramref
    /// name='bytes'/> 's length).</param>
    /// <param name='jsonoptions'>Specifies options to control how the JSON
    /// data is decoded to CBOR. See the JSONOptions class.</param>
    /// <returns>A CBOR object containing the JSON data decoded.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> or <paramref name='jsonoptions'/> is
    /// null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The byte array contains
    /// invalid encoding or is not in JSON format.</exception>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='count'/> is less than 0 or
    /// greater than <paramref name='bytes'/> 's length, or <paramref
    /// name='bytes'/> 's length minus <paramref name='offset'/> is less
    /// than <paramref name='count'/>.</exception>
    public static CBORObject FromJSONBytes(
      byte[] bytes,
      int offset,
      int count,
      JSONOptions jsonoptions) {
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      if (jsonoptions == null) {
        throw new ArgumentNullException(nameof(jsonoptions));
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + offset + ") is not greater" +
          "\u0020or equal to 0");
      }
      if (offset > bytes.Length) {
        throw new ArgumentException("offset (" + offset + ") is not less or" +
          "\u0020equal to " + bytes.Length);
      }
      if (count < 0) {
        throw new ArgumentException("count (" + count + ") is not greater or" +
          "\u0020equal to 0");
      }
      if (count > bytes.Length) {
        throw new ArgumentException("count (" + count + ") is not less or" +
          "\u0020equal to " + bytes.Length);
      }
      if (bytes.Length - offset < count) {
        throw new ArgumentException("bytes's length minus " + offset + " (" +
          (bytes.Length - offset) + ") is not greater or equal to " + count);
      }
      if (count == 0) {
        throw new CBORException("Byte array is empty");
      }
      if (bytes[offset] >= 0x01 && bytes[offset] <= 0x7f && count >= 2 &&
        bytes[offset + 1] != 0) {
        // UTF-8 JSON bytes
        return CBORJson2.ParseJSONValue(
            bytes,
            offset,
            offset + count,
            jsonoptions);
      } else {
        // Other than UTF-8 without byte order mark
        try {
          using (var ms = new MemoryStream(bytes, offset, count)) {
            return ReadJSON(ms, jsonoptions);
          }
        } catch (IOException ex) {
          throw new CBORException(ex.Message, ex);
        }
      }
    }

    /// <summary>
    /// <para>Writes a text string in CBOR format to a data stream. The
    /// string will be encoded using definite-length encoding regardless of
    /// its length.</para></summary>
    /// <param name='str'>The string to write. Can be null.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static void Write(string str, Stream stream) {
      Write(str, stream, CBOREncodeOptions.Default);
    }

    /// <summary>Writes a text string in CBOR format to a data stream,
    /// using the given options to control the encoding process.</summary>
    /// <param name='str'>The string to write. Can be null.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <param name='options'>Options for encoding the data to
    /// CBOR.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static void Write(
      string str,
      Stream stream,
      CBOREncodeOptions options) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      if (str == null) {
        stream.WriteByte(0xf6); // Write null instead of string
      } else {
        if (!options.UseIndefLengthStrings || options.Ctap2Canonical) {
          // NOTE: Length of a String object won't be higher than the maximum
          // allowed for definite-length strings
          long codePointLength = DataUtilities.GetUtf8Length(str, true);
          _ = WritePositiveInt64(3, codePointLength, stream);
          _ = DataUtilities.WriteUtf8(str, stream, true);
        } else {
          WriteStreamedString(str, stream);
        }
      }
    }

    /// <summary>Writes a binary floating-point number in CBOR format to a
    /// data stream, as though it were converted to a CBOR object via
    /// CBORObject.FromEFloat(EFloat) and then written out.</summary>
    /// <param name='bignum'>An arbitrary-precision binary floating-point
    /// number. Can be null.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static void Write(EFloat bignum, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (bignum == null) {
        stream.WriteByte(0xf6);
        return;
      }
      if ((bignum.IsZero && bignum.IsNegative) || bignum.IsInfinity() ||
        bignum.IsNaN()) {
        Write(FromEFloat(bignum), stream);
        return;
      }
      EInteger exponent = bignum.Exponent;
      if (exponent.CanFitInInt64()) {
        stream.WriteByte(0xc5); // tag 5
        stream.WriteByte(0x82); // array, length 2
      } else if (exponent.GetSignedBitLengthAsInt64() > 64) {
        stream.WriteByte(0xd9); // tag 265
        stream.WriteByte(0x01);
        stream.WriteByte(0x09);
        stream.WriteByte(0x82); // array, length 2
      } else {
        stream.WriteByte(0xc5); // tag 5
        stream.WriteByte(0x82); // array, length 2
      }
      Write(
        bignum.Exponent,
        stream);
      Write(bignum.Mantissa, stream);
    }

    /// <summary>Writes a rational number in CBOR format to a data stream,
    /// as though it were converted to a CBOR object via
    /// CBORObject.FromERational(ERational) and then written out.</summary>
    /// <param name='rational'>An arbitrary-precision rational number. Can
    /// be null.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static void Write(ERational rational, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (rational == null) {
        stream.WriteByte(0xf6);
        return;
      }
      if (!rational.IsFinite || (rational.IsNegative && rational.IsZero)) {
        Write(FromERational(rational), stream);
        return;
      }
      stream.WriteByte(0xd8); // tag 30
      stream.WriteByte(0x1e);
      stream.WriteByte(0x82); // array, length 2
      Write(rational.Numerator, stream);
      Write(
        rational.Denominator,
        stream);
    }

    /// <summary>Writes a decimal floating-point number in CBOR format to a
    /// data stream, as though it were converted to a CBOR object via
    /// CBORObject.FromEDecimal(EDecimal) and then written out.</summary>
    /// <param name='bignum'>The arbitrary-precision decimal number to
    /// write. Can be null.</param>
    /// <param name='stream'>Stream to write to.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static void Write(EDecimal bignum, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (bignum == null) {
        stream.WriteByte(0xf6);
        return;
      }
      if (!bignum.IsFinite || (bignum.IsNegative && bignum.IsZero)) {
        Write(FromEDecimal(bignum), stream);
        return;
      }
      EInteger exponent = bignum.Exponent;
      if (exponent.CanFitInInt64()) {
        stream.WriteByte(0xc4); // tag 4
        stream.WriteByte(0x82); // array, length 2
      } else if (exponent.GetSignedBitLengthAsInt64() > 64) {
        stream.WriteByte(0xd9); // tag 264
        stream.WriteByte(0x01);
        stream.WriteByte(0x08);
        stream.WriteByte(0x82); // array, length 2
      } else {
        stream.WriteByte(0xc4); // tag 4
        stream.WriteByte(0x82); // array, length 2
      }
      Write(exponent, stream);
      Write(bignum.Mantissa, stream);
    }

    private static byte[] EIntegerBytes(EInteger ei) {
      if (ei.IsZero) {
        return new byte[] { 0 };
      }
      if (ei.Sign < 0) {
        ei = ei.Add(1).Negate();
      }
      byte[] bytes = ei.ToBytes(false);
      var index = 0;
      while (index < bytes.Length && bytes[index] == 0) {
        ++index;
      }
      if (index > 0) {
        var newBytes = new byte[bytes.Length - index];
        Array.Copy(bytes, index, newBytes, 0, newBytes.Length);
        return newBytes;
      }
      return bytes;
    }

    /// <summary>Writes a arbitrary-precision integer in CBOR format to a
    /// data stream.</summary>
    /// <param name='bigint'>Arbitrary-precision integer to write. Can be
    /// null.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static void Write(EInteger bigint, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (bigint == null) {
        stream.WriteByte(0xf6);
        return;
      }
      var datatype = 0;
      if (bigint.Sign < 0) {
        datatype = 1;
        bigint = bigint.Add(EInteger.One);
        bigint = -bigint;
      }
      if (bigint.CanFitInInt64()) {
        // If the arbitrary-precision integer is representable as a long and in
        // major type 0 or 1, write that major type
        // instead of as a bignum
        WritePositiveInt64(datatype, bigint.ToInt64Checked(), stream);
      } else {
        // Get a byte array of the arbitrary-precision integer's value,
        // since shifting and doing AND operations is
        // slow with large EIntegers
        byte[] bytes = bigint.ToBytes(true);
        int byteCount = bytes.Length;
        while (byteCount > 0 && bytes[byteCount - 1] == 0) {
          // Ignore trailing zero bytes
          --byteCount;
        }
        if (byteCount != 0) {
          int half = byteCount >> 1;
          int right = byteCount - 1;
          for (int i = 0; i < half; ++i, --right) {
            // NOTE: Swapping syntax can't be used in netstandard1.0
            // because it relies on System.ValueTuple
            byte tmp = bytes[i];
            bytes[i] = bytes[right];
            bytes[right] = tmp;
          }
        }
        switch (byteCount) {
          case 0:
            stream.WriteByte((byte)(datatype << 5));
            return;
          case 1:
            WritePositiveInt(datatype, bytes[0] & 0xff, stream);
            break;
          case 2:
            stream.WriteByte((byte)((datatype << 5) | 25));
            stream.Write(bytes, 0, byteCount);
            break;
          case 3:
            stream.WriteByte((byte)((datatype << 5) | 26));
            stream.WriteByte(0);
            stream.Write(bytes, 0, byteCount);
            break;
          case 4:
            stream.WriteByte((byte)((datatype << 5) | 26));
            stream.Write(bytes, 0, byteCount);
            break;
          case 5:
            stream.WriteByte((byte)((datatype << 5) | 27));
            stream.WriteByte(0);
            stream.WriteByte(0);
            stream.WriteByte(0);
            stream.Write(bytes, 0, byteCount);
            break;
          case 6:
            stream.WriteByte((byte)((datatype << 5) | 27));
            stream.WriteByte(0);
            stream.WriteByte(0);
            stream.Write(bytes, 0, byteCount);
            break;
          case 7:
            stream.WriteByte((byte)((datatype << 5) | 27));
            stream.WriteByte(0);
            stream.Write(bytes, 0, byteCount);
            break;
          case 8:
            stream.WriteByte((byte)((datatype << 5) | 27));
            stream.Write(bytes, 0, byteCount);
            break;
          default: stream.WriteByte((datatype == 0) ?
              (byte)0xc2 : (byte)0xc3);
            WritePositiveInt(2, byteCount, stream);
            stream.Write(bytes, 0, byteCount);
            break;
        }
      }
    }

    /// <summary>Writes a 64-bit signed integer in CBOR format to a data
    /// stream.</summary>
    /// <param name='value'>The value to write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static void Write(long value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (value >= 0) {
        _ = WritePositiveInt64(0, value, stream);
      } else {
        ++value;
        value = -value; // Will never overflow
        _ = WritePositiveInt64(1, value, stream);
      }
    }

    /// <summary>Writes a 32-bit signed integer in CBOR format to a data
    /// stream.</summary>
    /// <param name='value'>The value to write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static void Write(int value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      var type = 0;
      if (value < 0) {
        ++value;
        value = -value;
        type = 0x20;
      }
      if (value < 24) {
        stream.WriteByte((byte)(value | type));
      } else if (value <= 0xff) {
        byte[] bytes = { (byte)(24 | type), (byte)(value & 0xff) };
        stream.Write(bytes, 0, 2);
      } else if (value <= 0xffff) {
        byte[] bytes = {
          (byte)(25 | type), (byte)((value >> 8) & 0xff),
          (byte)(value & 0xff),
        };
        stream.Write(bytes, 0, 3);
      } else {
        byte[] bytes = {
          (byte)(26 | type), (byte)((value >> 24) & 0xff),
          (byte)((value >> 16) & 0xff), (byte)((value >> 8) & 0xff),
          (byte)(value & 0xff),
        };
        stream.Write(bytes, 0, 5);
      }
    }

    /// <summary>Writes a 16-bit signed integer in CBOR format to a data
    /// stream.</summary>
    /// <param name='value'>The value to write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static void Write(short value, Stream stream) {
      Write((long)value, stream);
    }

    /// <summary>Writes a Boolean value in CBOR format to a data
    /// stream.</summary>
    /// <param name='value'>The value to write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static void Write(bool value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      stream.WriteByte(value ? (byte)0xf5 : (byte)0xf4);
    }

    /// <summary>Writes a byte (0 to 255) in CBOR format to a data stream.
    /// If the value is less than 24, writes that byte. If the value is 25
    /// to 255, writes the byte 24, then this byte's value.</summary>
    /// <param name='value'>The value to write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static void Write(byte value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if ((value & 0xff) < 24) {
        stream.WriteByte(value);
      } else {
        stream.WriteByte(24);
        stream.WriteByte(value);
      }
    }

    /// <summary>Writes a 32-bit floating-point number in CBOR format to a
    /// data stream. The number is written using the shortest
    /// floating-point encoding possible; this is a change from previous
    /// versions.</summary>
    /// <param name='value'>The value to write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static void Write(float value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      _ = WriteFloatingPointBits(
        stream,
        CBORUtilities.SingleToInt32Bits(value),
        4,
        true);
    }

    /// <summary>Writes a 64-bit floating-point number in CBOR format to a
    /// data stream. The number is written using the shortest
    /// floating-point encoding possible; this is a change from previous
    /// versions.</summary>
    /// <param name='value'>The value to write.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    public static void Write(double value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      _ = WriteFloatingPointBits(
        stream,
        CBORUtilities.DoubleToInt64Bits(value),
        8,
        true);
    }

    /// <summary>Writes a CBOR object to a CBOR data stream.</summary>
    /// <param name='value'>The value to write. Can be null.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    public static void Write(CBORObject value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (value == null) {
        stream.WriteByte(0xf6);
      } else {
        value.WriteTo(stream);
      }
    }

    /// <summary>
    /// <para>Writes a CBOR object to a CBOR data stream. See the
    /// three-parameter Write method that takes a
    /// CBOREncodeOptions.</para></summary>
    /// <param name='objValue'>The arbitrary object to be serialized. Can
    /// be null.</param>
    /// <param name='stream'>A writable data stream.</param>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static void Write(object objValue, Stream stream) {
      Write(objValue, stream, CBOREncodeOptions.Default);
    }

    /// <summary>Writes an arbitrary object to a CBOR data stream, using
    /// the specified options for controlling how the object is encoded to
    /// CBOR data format. If the object is convertible to a CBOR map or a
    /// CBOR object that contains CBOR maps, the order in which the keys to
    /// those maps are written out to the data stream is undefined unless
    /// the map was created using the NewOrderedMap method. The example
    /// code given in
    /// <see cref='PeterO.Cbor.CBORObject.WriteTo(System.IO.Stream)'/> can
    /// be used to write out certain keys of a CBOR map in a given order.
    /// Currently, the following objects are supported:
    /// <list type=''>
    /// <item>Lists of CBORObject.</item>
    /// <item>Maps of CBORObject. The order in which the keys to the map
    /// are written out to the data stream is undefined unless the map was
    /// created using the NewOrderedMap method.</item>
    /// <item>Null.</item>
    /// <item>Byte arrays, which will always be written as definite-length
    /// byte strings.</item>
    /// <item>String objects. The strings will be encoded using
    /// definite-length encoding regardless of their length.</item>
    /// <item>Any object accepted by the FromObject
    /// method.</item></list></summary>
    /// <param name='objValue'>The arbitrary object to be serialized. Can
    /// be null.
    /// <para><b>NOTE:</b> For security reasons, whenever possible, an
    /// application should not base this parameter on user input or other
    /// externally supplied data, and whenever possible, the application
    /// should limit this parameter's inputs to types specially handled by
    /// this method (such as <c>int</c> or <c>String</c> ) and/or to
    /// plain-old-data types (POCO or POJO types) within the control of the
    /// application. If the plain-old-data type references other data
    /// types, those types should likewise meet either criterion
    /// above.</para>.</param>
    /// <param name='output'>A writable data stream.</param>
    /// <param name='options'>CBOR options for encoding the CBOR object to
    /// bytes.</param>
    /// <exception cref='ArgumentException'>The object's type is not
    /// supported.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='options'/> or <paramref name='output'/> is null.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static void Write(
      object objValue,
      Stream output,
      CBOREncodeOptions options) {
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      if (output == null) {
        throw new ArgumentNullException(nameof(output));
      }
      if (objValue == null) {
        output.WriteByte(0xf6);
        return;
      }
      if (options.Ctap2Canonical) {
        FromObject(objValue).WriteTo(output, options);
        return;
      }
      if (objValue is byte[] data) {
        _ = WritePositiveInt(3, data.Length, output);
        output.Write(data, 0, data.Length);
        return;
      }
      if (objValue is IList<CBORObject>) {
        WriteObjectArray(
          (IList<CBORObject>)objValue,
          output,
          options);
        return;
      }
      if (objValue is IDictionary<CBORObject, CBORObject>) {
        WriteObjectMap(
          (IDictionary<CBORObject, CBORObject>)objValue,
          output,
          options);
        return;
      }
      FromObject(objValue).WriteTo(output, options);
    }

    /// <summary>Converts an arbitrary object to a text string in
    /// JavaScript Object Notation (JSON) format, as in the ToJSONString
    /// method, and writes that string to a data stream in UTF-8. If the
    /// object is convertible to a CBOR map, or to a CBOR object that
    /// contains CBOR maps, the order in which the keys to those maps are
    /// written out to the JSON string is undefined unless the map was
    /// created using the NewOrderedMap method. The example code given in
    /// <b>PeterO.Cbor.CBORObject.ToJSONString(PeterO.Cbor.JSONOptions)</b>
    /// can be used to write out certain keys of a CBOR map in a given
    /// order to a JSON string.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object. Can be null.
    /// <para><b>NOTE:</b> For security reasons, whenever possible, an
    /// application should not base this parameter on user input or other
    /// externally supplied data, and whenever possible, the application
    /// should limit this parameter's inputs to types specially handled by
    /// this method (such as <c>int</c> or <c>String</c> ) and/or to
    /// plain-old-data types (POCO or POJO types) within the control of the
    /// application. If the plain-old-data type references other data
    /// types, those types should likewise meet either criterion
    /// above.</para>.</param>
    /// <param name='outputStream'>A writable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='outputStream'/> is null.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static void WriteJSON(object obj, Stream outputStream) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      if (obj == null) {
        outputStream.Write(ValueNullBytes, 0, ValueNullBytes.Length);
        return;
      }
      if (obj is bool) {
        if ((bool)obj) {
          outputStream.Write(ValueTrueBytes, 0, ValueTrueBytes.Length);
          return;
        }
        outputStream.Write(ValueFalseBytes, 0, ValueFalseBytes.Length);
        return;
      }
      CBORObject.FromObject(obj).WriteJSONTo(outputStream);
    }

    /// <summary>
    /// <para>Adds a new key and its value to this CBOR map, or adds the
    /// value if the key doesn't exist.</para>
    /// <para>NOTE: This method can't be used to add a tag to an existing
    /// CBOR object. To create a CBOR object with a given tag, call the
    /// <c>CBORObject.FromCBORObjectAndTag</c> method and pass the CBOR
    /// object and the desired tag number to that method.</para></summary>
    /// <param name='key'>An object representing the key, which will be
    /// converted to a CBORObject. Can be null, in which case this value is
    /// converted to CBORObject.Null.</param>
    /// <param name='valueOb'>An object representing the value, which will
    /// be converted to a CBORObject. Can be null, in which case this value
    /// is converted to CBORObject.Null.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='key'/> already exists in this map.</exception>
    /// <exception cref='InvalidOperationException'>This object is not a
    /// map.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='key'/> or <paramref name='valueOb'/> has an unsupported
    /// type.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public CBORObject Add(object key, object valueOb) {
      if (this.Type == CBORType.Map) {
        CBORObject mapKey;
        CBORObject mapValue;
        if (key == null) {
          mapKey = CBORObject.Null;
        } else {
          mapKey = key as CBORObject;
          mapKey = mapKey ?? FromObject(key);
        }
        if (valueOb == null) {
          mapValue = Null;
        } else {
          mapValue = valueOb as CBORObject;
          mapValue = mapValue ?? FromObject(valueOb);
        }
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        if (map.ContainsKey(mapKey)) {
          throw new ArgumentException("Key already exists");
        }
        map.Add(
          mapKey,
          mapValue);
      } else {
        throw new InvalidOperationException("Not a map");
      }
      return this;
    }

    /// <summary><para>Adds a new object to the end of this array. (Used to
    /// throw ArgumentNullException on a null reference, but now converts
    /// the null reference to CBORObject.Null, for convenience with the
    /// Object overload of this method).</para>
    ///  <para>NOTE: This method
    /// can't be used to add a tag to an existing CBOR object. To create a
    /// CBOR object with a given tag, call the
    /// <c>CBORObject.FromCBORObjectAndTag</c>
    ///  method and pass the CBOR
    /// object and the desired tag number to that method.</para>
    ///  </summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is a CBOR
    /// object.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='InvalidOperationException'>This object is not an
    /// array.</exception>
    /// <example>
    /// <para>The following example creates a CBOR array and adds several
    /// CBOR objects, one of which has a custom CBOR tag, to that array.
    /// Note the chaining behavior made possible by this method.</para>
    /// <code>CBORObject obj = CBORObject.NewArray() .Add(CBORObject.False)
    /// .Add(CBORObject.FromObject(5)) .Add(CBORObject.FromObject("text
    /// string")) .Add(CBORObject.FromCBORObjectAndTag(9999, 1));</code>
    ///  .
    /// </example>
    public CBORObject Add(CBORObject obj) {
      if (this.Type == CBORType.Array) {
        IList<CBORObject> list = this.AsList();
        list.Add(obj);
        return this;
      }
      throw new InvalidOperationException("Not an array");
    }

    /// <summary><para>Converts an object to a CBOR object and adds it to
    /// the end of this array.</para>
    ///  <para>NOTE: This method can't be used
    /// to add a tag to an existing CBOR object. To create a CBOR object
    /// with a given tag, call the <c>CBORObject.FromCBORObjectAndTag</c>
    /// method and pass the CBOR object and the desired tag number to that
    /// method.</para>
    ///  </summary>
    /// <param name='obj'>A CBOR object (or an object convertible to a CBOR
    /// object) to add to this CBOR array.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='InvalidOperationException'>This instance is not an
    /// array.</exception>
    /// <exception cref='ArgumentException'>The type of <paramref
    /// name='obj'/> is not supported.</exception>
    /// <example>
    /// <para>The following example creates a CBOR array and adds several
    /// CBOR objects, one of which has a custom CBOR tag, to that array.
    /// Note the chaining behavior made possible by this method.</para>
    /// <code>CBORObject obj = CBORObject.NewArray() .Add(CBORObject.False) .Add(5)
    /// .Add("text string") .Add(CBORObject.FromCBORObjectAndTag(9999, 1));</code>
    ///  .
    /// </example>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public CBORObject Add(object obj) {
      if (this.Type == CBORType.Array) {
        IList<CBORObject> list = this.AsList();
        list.Add(FromObject(obj));
        return this;
      }
      throw new InvalidOperationException("Not an array");
    }

    /// <summary>Returns false if this object is a CBOR false, null, or
    /// undefined value (whether or not the object has tags); otherwise,
    /// true.</summary>
    /// <returns>False if this object is a CBOR false, null, or undefined
    /// value; otherwise, true.</returns>
    public bool AsBoolean() {
      return !this.IsFalse && !this.IsNull && !this.IsUndefined;
    }

    /// <summary>Converts this object to a 64-bit floating point
    /// number.</summary>
    /// <returns>The closest 64-bit floating point number to this object.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 64-bit floating point
    /// number.</returns>
    /// <exception cref='InvalidOperationException'>This object does not
    /// represent a number (for this purpose, infinities and not-a-number
    /// or NaN values, but not CBORObject.Null, are considered
    /// numbers).</exception>
    public double AsDouble() {
      var cn = CBORNumber.FromCBORObject(this);
      return cn == null ? throw new InvalidOperationException("Not a number" +
"\u0020type") : cn.GetNumberInterface().AsDouble(cn.GetValue());
    }

    /// <summary>Converts this object to a 32-bit signed integer if this
    /// CBOR object's type is Integer. This method disregards the tags this
    /// object has, if any.</summary>
    /// <returns>The 32-bit signed integer stored by this object.</returns>
    /// <exception cref='InvalidOperationException'>This object's type is
    /// not <c>CBORType.Integer</c>
    /// .</exception>
    /// <exception cref='OverflowException'>This object's value exceeds the
    /// range of a 32-bit signed integer.</exception>
    /// <example>
    /// <para>The following example code (originally written in C# for
    /// the.NET Framework) shows a way to check whether a given CBOR object
    /// stores a 32-bit signed integer before getting its value.</para>
    /// <code>CBORObject obj = CBORObject.FromInt32(99999);
    /// if (obj.CanValueFitInInt32()) { /* Not an Int32;
    /// handle the error */ Console.WriteLine("Not a 32-bit integer."); } else {
    /// Console.WriteLine("The value is " + obj.AsInt32Value()); }</code>
    ///  .
    /// </example>
    public int AsInt32Value() {
      switch (this.ItemType) {
        case CBORObjectTypeInteger: {
            var longValue = (long)this.ThisItem;
            return longValue < Int32.MinValue || longValue > Int32.MaxValue ?
throw new OverflowException() : (int)longValue;
          }
        case CBORObjectTypeEInteger: {
            var ei = (EInteger)this.ThisItem;
            return ei.ToInt32Checked();
          }
        default: throw new InvalidOperationException("Not an integer type");
      }
    }

    /// <summary>Converts this object to a 64-bit signed integer if this
    /// CBOR object's type is Integer. This method disregards the tags this
    /// object has, if any.</summary>
    /// <returns>The 64-bit signed integer stored by this object.</returns>
    /// <exception cref='InvalidOperationException'>This object's type is
    /// not <c>CBORType.Integer</c>
    /// .</exception>
    /// <exception cref='OverflowException'>This object's value exceeds the
    /// range of a 64-bit signed integer.</exception>
    /// <example>
    /// <para>The following example code (originally written in C# for
    /// the.NET Framework) shows a way to check whether a given CBOR object
    /// stores a 64-bit signed integer before getting its value.</para>
    /// <code>CBORObject obj = CBORObject.FromInt64(99999);
    /// if (obj.CanValueFitInInt64()) {
    /// &#x2f;&#x2a; Not an Int64; handle the error&#x2a;&#x2f;
    /// Console.WriteLine("Not a 64-bit integer."); } else {
    /// Console.WriteLine("The value is " + obj.AsInt64Value()); }</code>
    ///  .
    /// </example>
    public long AsInt64Value() {
      switch (this.ItemType) {
        case CBORObjectTypeInteger:
          return (long)this.ThisItem;
        case CBORObjectTypeEInteger: {
            var ei = (EInteger)this.ThisItem;
            return ei.ToInt64Checked();
          }
        default: throw new InvalidOperationException("Not an integer type");
      }
    }

    /// <summary>Returns whether this CBOR object stores an integer
    /// (CBORType.Integer) within the range of a 64-bit signed integer.
    /// This method disregards the tags this object has, if any.</summary>
    /// <returns><c>true</c> if this CBOR object stores an integer
    /// (CBORType.Integer) whose value is at least -(2^63) and less than
    /// 2^63; otherwise, <c>false</c>.</returns>
    public bool CanValueFitInInt64() {
      switch (this.ItemType) {
        case CBORObjectTypeInteger:
          return true;
        case CBORObjectTypeEInteger: {
            var ei = (EInteger)this.ThisItem;
            return ei.CanFitInInt64();
          }
        default: return false;
      }
    }

    /// <summary>Returns whether this CBOR object stores an integer
    /// (CBORType.Integer) within the range of a 32-bit signed integer.
    /// This method disregards the tags this object has, if any.</summary>
    /// <returns><c>true</c> if this CBOR object stores an integer
    /// (CBORType.Integer) whose value is at least -(2^31) and less than
    /// 2^31; otherwise, <c>false</c>.</returns>
    public bool CanValueFitInInt32() {
      switch (this.ItemType) {
        case CBORObjectTypeInteger: {
            var elong = (long)this.ThisItem;
            return elong >= Int32.MinValue && elong <= Int32.MaxValue;
          }
        case CBORObjectTypeEInteger: {
            var ei = (EInteger)this.ThisItem;
            return ei.CanFitInInt32();
          }
        default: return false;
      }
    }

    /// <summary>Converts this object to an arbitrary-precision integer if
    /// this CBOR object's type is Integer. This method disregards the tags
    /// this object has, if any. (Note that CBOR stores untagged integers
    /// at least -(2^64) and less than 2^64.).</summary>
    /// <returns>The integer stored by this object.</returns>
    /// <exception cref='InvalidOperationException'>This object's type is
    /// not <c>CBORType.Integer</c>.</exception>
    public EInteger AsEIntegerValue() {
      switch (this.ItemType) {
        case CBORObjectTypeInteger:
          return EInteger.FromInt64((long)this.ThisItem);
        case CBORObjectTypeEInteger:
          return (EInteger)this.ThisItem;
        default: throw new InvalidOperationException("Not an integer type");
      }
    }

    /// <summary>Converts this object to the bits of a 64-bit
    /// floating-point number if this CBOR object's type is FloatingPoint.
    /// This method disregards the tags this object has, if any.</summary>
    /// <returns>The bits of a 64-bit floating-point number stored by this
    /// object. The most significant bit is the sign (set means negative,
    /// clear means nonnegative); the next most significant 11 bits are the
    /// exponent area; and the remaining bits are the significand area. If
    /// all the bits of the exponent area are set and the significand area
    /// is 0, this indicates infinity. If all the bits of the exponent area
    /// are set and the significand area is other than 0, this indicates
    /// not-a-number (NaN).</returns>
    /// <exception cref='InvalidOperationException'>This object's type is
    /// not <c>CBORType.FloatingPoint</c>.</exception>
    public long AsDoubleBits() {
      switch (this.Type) {
        case CBORType.FloatingPoint:
          return (long)this.ThisItem;
        default: throw new InvalidOperationException("Not a floating-point" +
            "\u0020type");
      }
    }

    /// <summary>Converts this object to a 64-bit floating-point number if
    /// this CBOR object's type is FloatingPoint. This method disregards
    /// the tags this object has, if any.</summary>
    /// <returns>The 64-bit floating-point number stored by this
    /// object.</returns>
    /// <exception cref='InvalidOperationException'>This object's type is
    /// not <c>CBORType.FloatingPoint</c>.</exception>
    public double AsDoubleValue() {
      switch (this.Type) {
        case CBORType.FloatingPoint:
          return CBORUtilities.Int64BitsToDouble((long)this.ThisItem);
        default: throw new InvalidOperationException("Not a floating-point" +
            "\u0020type");
      }
    }

    /// <summary>Converts this object to a CBOR number. (NOTE: To determine
    /// whether this method call can succeed, call the <b>IsNumber</b>
    /// property (isNumber() method in Java) before calling this
    /// method.).</summary>
    /// <returns>The number represented by this object.</returns>
    /// <exception cref='InvalidOperationException'>This object does not
    /// represent a number (for this purpose, infinities and not-a-number
    /// or NaN values, but not CBORObject.Null, are considered
    /// numbers).</exception>
    public CBORNumber AsNumber() {
      var num = CBORNumber.FromCBORObject(this);
      return num ?? throw new InvalidOperationException("Not a number type");
    }

    /// <summary>Converts this object to a 32-bit signed integer.
    /// Non-integer number values are converted to integers by discarding
    /// their fractional parts. (NOTE: To determine whether this method
    /// call can succeed, call <b>AsNumber().CanTruncatedIntFitInInt32</b>
    /// before calling this method. See the example.).</summary>
    /// <returns>The closest 32-bit signed integer to this
    /// object.</returns>
    /// <exception cref='InvalidOperationException'>This object does not
    /// represent a number (for this purpose, infinities and not-a-number
    /// or NaN values, but not CBORObject.Null, are considered
    /// numbers).</exception>
    /// <exception cref='OverflowException'>This object's value exceeds the
    /// range of a 32-bit signed integer.</exception>
    /// <example>
    /// <para>The following example code (originally written in C# for
    /// the.NET Framework) shows a way to check whether a given CBOR object
    /// stores a 32-bit signed integer before getting its value.</para>
    /// <code>CBORObject obj = CBORObject.FromInt32(99999);
    /// if (obj.AsNumber().CanTruncatedIntFitInInt32()) {
    /// &#x2f;&#x2a; Not an Int32; handle the error &#x2a;&#x2f;
    /// Console.WriteLine("Not a 32-bit integer."); } else {
    /// Console.WriteLine("The value is " + obj.AsInt32()); }</code>
    ///  .
    /// </example>
    public int AsInt32() {
      return this.AsInt32(Int32.MinValue, Int32.MaxValue);
    }

    /// <summary>Converts this object to a 32-bit floating point
    /// number.</summary>
    /// <returns>The closest 32-bit floating point number to this object.
    /// The return value can be positive infinity or negative infinity if
    /// this object's value exceeds the range of a 32-bit floating point
    /// number.</returns>
    /// <exception cref='InvalidOperationException'>This object does not
    /// represent a number (for this purpose, infinities and not-a-number
    /// or NaN values, but not CBORObject.Null, are considered
    /// numbers).</exception>
    public float AsSingle() {
      CBORNumber cn = this.AsNumber();
      return cn.GetNumberInterface().AsSingle(cn.GetValue());
    }

    /// <summary>Converts this object to a Guid.</summary>
    /// <returns>A Guid.</returns>
    /// <exception cref='InvalidOperationException'>This object does not
    /// represent a Guid.</exception>
    /// <exception cref='CBORException'>This object does not have the
    /// expected tag.</exception>
    public Guid AsGuid() {
      return new CBORUuidConverter().FromCBORObject(this);
    }

    /// <summary>Gets the value of this object as a text string.</summary>
    /// <returns>Gets this object's string.</returns>
    /// <exception cref='InvalidOperationException'>This object's type is
    /// not a text string (for the purposes of this method, infinity and
    /// not-a-number values, but not <c>CBORObject.Null</c>, are
    /// considered numbers). To check the CBOR object for null before
    /// conversion, use the following idiom (originally written in C# for
    /// the.NET version): <c>(cbor == null || cbor.IsNull) ? null :
    /// cbor.AsString()</c>.</exception>
    /// <remarks>This method is not the "reverse" of the <c>FromString</c>
    /// method in the sense that FromString can take either a text string
    /// or <c>null</c>, but this method can accept only text strings. The
    /// <c>ToObject</c> method is closer to a "reverse" version to
    /// <c>FromString</c> than the <c>AsString</c> method:
    /// <c>ToObject&lt;String&gt;(cbor)</c> in DotNet, or
    /// <c>ToObject(String.class)</c> in Java, will convert a CBOR object
    /// to a DotNet or Java String if it represents a text string, or to
    /// <c>null</c> if <c>IsNull</c> returns <c>true</c> for the CBOR
    /// object, and will fail in other cases.</remarks>
    public string AsString() {
      int type = this.ItemType;
      switch (type) {
        case CBORObjectTypeTextString:
        case CBORObjectTypeTextStringAscii: {
            return (string)this.ThisItem;
          }
        case CBORObjectTypeTextStringUtf8: {
            return DataUtilities.GetUtf8String((byte[])this.ThisItem, false);
          }
        default:
          throw new InvalidOperationException("Not a text string type");
      }
    }

    private static string Chop(string str) {
      return str.Substring(0, Math.Min(100, str.Length));
    }

    /// <summary>Compares two CBOR objects. This implementation was changed
    /// in version 4.0.
    /// <para>In this implementation:</para>
    /// <list type=''>
    /// <item>The null pointer (null reference) is considered less than any
    /// other object.</item>
    /// <item>If the two objects are both integers (CBORType.Integer) both
    /// floating-point values, both byte strings, both simple values
    /// (including True and False), or both text strings, their CBOR
    /// encodings (as though EncodeToBytes were called on each integer) are
    /// compared as though by a byte-by-byte comparison. (This means, for
    /// example, that positive integers sort before negative
    /// integers).</item>
    /// <item>If both objects have a tag, they are compared first by the
    /// tag's value then by the associated item (which itself can have a
    /// tag).</item>
    /// <item>If both objects are arrays, they are compared item by item.
    /// In this case, if the arrays have different numbers of items, the
    /// array with more items is treated as greater than the other
    /// array.</item>
    /// <item>If both objects are maps, their key-value pairs, sorted by
    /// key in accordance with this method, are compared, where each pair
    /// is compared first by key and then by value. In this case, if the
    /// maps have different numbers of key-value pairs, the map with more
    /// pairs is treated as greater than the other map.</item>
    /// <item>If the two objects have different types, the object whose
    /// type comes first in the order of untagged integers, untagged byte
    /// strings, untagged text strings, untagged arrays, untagged maps,
    /// tagged objects, untagged simple values (including True and False)
    /// and untagged floating point values sorts before the other
    /// object.</item></list>
    /// <para>This method is consistent with the Equals
    /// method.</para></summary>
    /// <param name='other'>A value to compare with.</param>
    /// <returns>A negative number, if this value is less than the other
    /// object; or 0, if both values are equal; or a positive number, if
    /// this value is less than the other object or if the other object is
    /// null.
    /// <para>This implementation returns a positive number if <paramref
    /// name='other'/> is null, to conform to the.NET definition of
    /// CompareTo. This is the case even in the Java version of this
    /// library, for consistency's sake, even though implementations of
    /// <c>Comparable.compareTo()</c> in Java ought to throw an exception
    /// if they receive a null argument rather than treating null as less
    /// or greater than any object.</para>.</returns>
    public int CompareTo(CBORObject other) {
      if (other == null) {
        return 1;
      }
      if (this == other) {
        return 0;
      }
      int typeA = this.itemtypeValue;
      int typeB = other.itemtypeValue;
      object objA = this.itemValue;
      object objB = other.itemValue;
      // DebugUtility.Log("typeA=" + typeA);
      // DebugUtility.Log("typeB=" + typeB);
      // DebugUtility.Log("objA=" + Chop(this.ItemType ==
      // CBORObjectTypeMap ? "(map)" :
      // this.ToString()));
      // DebugUtility.Log("objB=" + Chop(other.ItemType ==
      // CBORObjectTypeMap ? "(map)" :
      // other.ToString()));
      int cmp;
      if (typeA == typeB) {
        switch (typeA) {
          case CBORObjectTypeInteger: {
              var a = (long)objA;
              var b = (long)objB;
              if (a >= 0 && b >= 0) {
                cmp = (a == b) ? 0 : ((a < b) ? -1 : 1);
              } else if (a <= 0 && b <= 0) {
                cmp = (a == b) ? 0 : ((a < b) ? 1 : -1);
              } else if (a < 0 && b >= 0) {
                // NOTE: Negative integers sort after
                // nonnegative integers in the bytewise
                // ordering of CBOR encodings
                cmp = 1;
              } else {
#if DEBUG
                if (!(a >= 0 && b < 0)) {
                  throw new InvalidOperationException(
                "doesn't satisfy a>= 0" + "\u0020b<0");
                }
#endif
                cmp = -1;
              }
              break;
            }
          case CBORObjectTypeEInteger: {
              cmp = CBORUtilities.ByteArrayCompare(
                  this.EncodeToBytes(),
                  other.EncodeToBytes());
              break;
            }
          case CBORObjectTypeByteString:
          case CBORObjectTypeTextStringUtf8: {
              cmp = CBORUtilities.ByteArrayCompareLengthFirst((byte[])objA,
                  (byte[])objB);
              break;
            }
          case CBORObjectTypeTextStringAscii: {
              var strA = (string)objA;
              var strB = (string)objB;
              int alen = strA.Length;
              int blen = strB.Length;
              cmp = (alen < blen) ? (-1) : ((alen > blen) ? 1 :
  String.CompareOrdinal(strA, strB));
              break;
            }
          case CBORObjectTypeTextString: {
              var strA = (string)objA;
              var strB = (string)objB;
              cmp = CBORUtilities.CompareStringsAsUtf8LengthFirst(
                  strA,
                  strB);
              break;
            }
          case CBORObjectTypeArray: {
              cmp = ListCompare(
                  (List<CBORObject>)objA,
                  (List<CBORObject>)objB);
              break;
            }
          case CBORObjectTypeMap:
            cmp = MapCompare(
                (IDictionary<CBORObject, CBORObject>)objA,
                (IDictionary<CBORObject, CBORObject>)objB);
            break;
          case CBORObjectTypeTagged:
            cmp = this.MostOuterTag.CompareTo(other.MostOuterTag);
            if (cmp == 0) {
              cmp = ((CBORObject)objA).CompareTo((CBORObject)objB);
            }
            break;
          case CBORObjectTypeSimpleValue: {
              var valueA = (int)objA;
              var valueB = (int)objB;
              cmp = (valueA == valueB) ? 0 : ((valueA < valueB) ? -1 : 1);
              break;
            }
          case CBORObjectTypeDouble: {
              cmp = CBORUtilities.ByteArrayCompare(
                  GetDoubleBytes(this.AsDoubleBits(), 0),
                  GetDoubleBytes(other.AsDoubleBits(), 0));
              break;
            }
          default: throw new InvalidOperationException("Unexpected data " +
              "type");
        }
      } else if ((typeB == CBORObjectTypeInteger && typeA ==
          CBORObjectTypeEInteger) || (typeA == CBORObjectTypeInteger && typeB ==
          CBORObjectTypeEInteger)) {
        cmp = CBORUtilities.ByteArrayCompare(
            this.EncodeToBytes(),
            other.EncodeToBytes());
      } else if ((typeB == CBORObjectTypeTextString || typeB ==
CBORObjectTypeTextStringAscii) && typeA ==
        CBORObjectTypeTextStringUtf8) {
        cmp = -CBORUtilities.CompareUtf16Utf8LengthFirst(
            (string)objB,
            (byte[])objA);
      } else if ((typeA == CBORObjectTypeTextString || typeA ==
CBORObjectTypeTextStringAscii) && typeB ==
        CBORObjectTypeTextStringUtf8) {
        cmp = CBORUtilities.CompareUtf16Utf8LengthFirst(
            (string)objA,
            (byte[])objB);
      } else if ((typeA == CBORObjectTypeTextString && typeB ==
CBORObjectTypeTextStringAscii) ||
         (typeB == CBORObjectTypeTextString && typeA ==
CBORObjectTypeTextStringAscii)) {
        cmp = -CBORUtilities.CompareStringsAsUtf8LengthFirst(
            (string)objB,
            (string)objA);
      } else if ((typeA == CBORObjectTypeTextString || typeA ==
CBORObjectTypeTextStringAscii) && typeB ==
        CBORObjectTypeTextStringUtf8) {
        cmp = CBORUtilities.CompareUtf16Utf8LengthFirst(
            (string)objA,
            (byte[])objB);
      } else {
        int ta = (typeA == CBORObjectTypeTextStringUtf8 || typeA ==
CBORObjectTypeTextStringAscii) ?
          CBORObjectTypeTextString : typeA;
        int tb = (typeB == CBORObjectTypeTextStringUtf8 || typeB ==
CBORObjectTypeTextStringAscii) ?
          CBORObjectTypeTextString : typeB;
        /* NOTE: itemtypeValue numbers are ordered such that they
        // correspond to the lexicographical order of their CBOR encodings
        // (with the exception of Integer and EInteger together,
        // and TextString/TextStringUtf8) */
        cmp = (ta < tb) ? -1 : 1;
      }
      // DebugUtility.Log(" -> " + (cmp));
      return cmp;
    }

    /// <summary>Compares this object and another CBOR object, ignoring the
    /// tags they have, if any. See the CompareTo method for more
    /// information on the comparison function.</summary>
    /// <param name='other'>A value to compare with.</param>
    /// <returns>Less than 0, if this value is less than the other object;
    /// or 0, if both values are equal; or greater than 0, if this value is
    /// less than the other object or if the other object is
    /// null.</returns>
    public int CompareToIgnoreTags(CBORObject other) {
      return (other == null) ? 1 : ((this == other) ? 0 :
          this.Untag().CompareTo(other.Untag()));
    }

    /// <summary>Determines whether a value of the given key exists in this
    /// object.</summary>
    /// <param name='objKey'>The parameter <paramref name='objKey'/> is an
    /// arbitrary object.</param>
    /// <returns><c>true</c> if the given key is found, or <c>false</c> if
    /// the given key is not found or this object is not a map.</returns>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public bool ContainsKey(object objKey) {
      return (this.Type == CBORType.Map) &&
this.ContainsKey(CBORObject.FromObject(objKey));
    }

    /// <summary>Determines whether a value of the given key exists in this
    /// object.</summary>
    /// <param name='key'>An object that serves as the key. If this is
    /// <c>null</c>, checks for <c>CBORObject.Null</c>.</param>
    /// <returns><c>true</c> if the given key is found, or <c>false</c> if
    /// the given key is not found or this object is not a map.</returns>
    public bool ContainsKey(CBORObject key) {
      key = key ?? CBORObject.Null;
      if (this.Type == CBORType.Map) {
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        return map.ContainsKey(key);
      }
      return false;
    }

    /// <summary>Determines whether a value of the given key exists in this
    /// object.</summary>
    /// <param name='key'>A text string that serves as the key. If this is
    /// <c>null</c>, checks for <c>CBORObject.Null</c>.</param>
    /// <returns><c>true</c> if the given key (as a CBOR object) is found,
    /// or <c>false</c> if the given key is not found or this object is not
    /// a map.</returns>
    public bool ContainsKey(string key) {
      if (this.Type == CBORType.Map) {
        CBORObject ckey = key == null ? Null : FromString(key);
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        return map.ContainsKey(ckey);
      }
      return false;
    }

    private static byte[] GetDoubleBytes64(long valueBits, int tagbyte) {
      // Encode as double precision
      return tagbyte != 0 ? new[] {
        (byte)tagbyte, (byte)0xfb,
        (byte)((valueBits >> 56) & 0xff), (byte)((valueBits >> 48) & 0xff),
        (byte)((valueBits >> 40) & 0xff), (byte)((valueBits >> 32) & 0xff),
        (byte)((valueBits >> 24) & 0xff), (byte)((valueBits >> 16) & 0xff),
        (byte)((valueBits >> 8) & 0xff), (byte)(valueBits & 0xff),
      } : new[] {
   (byte)0xfb, (byte)((valueBits >> 56) & 0xff),
   (byte)((valueBits >> 48) & 0xff), (byte)((valueBits >> 40) & 0xff),
   (byte)((valueBits >> 32) & 0xff), (byte)((valueBits >> 24) & 0xff),
   (byte)((valueBits >> 16) & 0xff), (byte)((valueBits >> 8) & 0xff),
   (byte)(valueBits & 0xff),
 };
    }

    private static byte[] GetDoubleBytes(long valueBits, int tagbyte) {
      int bits = CBORUtilities.DoubleToHalfPrecisionIfSameValue(valueBits);
      if (bits != -1) {
        return tagbyte != 0 ? new[] {
          (byte)tagbyte, (byte)0xf9,
          (byte)((bits >> 8) & 0xff), (byte)(bits & 0xff),
        } : new[] {
   (byte)0xf9, (byte)((bits >> 8) & 0xff),
   (byte)(bits & 0xff),
 };
      }
      if (CBORUtilities.DoubleRetainsSameValueInSingle(valueBits)) {
        bits = CBORUtilities.DoubleToRoundedSinglePrecision(valueBits);
        return tagbyte != 0 ? new[] {
          (byte)tagbyte, (byte)0xfa,
          (byte)((bits >> 24) & 0xff), (byte)((bits >> 16) & 0xff),
          (byte)((bits >> 8) & 0xff), (byte)(bits & 0xff),
        } : new[] {
   (byte)0xfa, (byte)((bits >> 24) & 0xff),
   (byte)((bits >> 16) & 0xff), (byte)((bits >> 8) & 0xff),
   (byte)(bits & 0xff),
 };
      }
      return GetDoubleBytes64(valueBits, tagbyte);
    }

    /// <summary>
    /// <para>Writes the binary representation of this CBOR object and
    /// returns a byte array of that representation. If the CBOR object
    /// contains CBOR maps, or is a CBOR map itself, the order in which the
    /// keys to the map are written out to the byte array is undefined
    /// unless the map was created using the NewOrderedMap method. The
    /// example code given in
    /// <see cref='PeterO.Cbor.CBORObject.WriteTo(System.IO.Stream)'/> can
    /// be used to write out certain keys of a CBOR map in a given order.
    /// For the CTAP2 (FIDO Client-to-Authenticator Protocol 2) canonical
    /// ordering, which is useful for implementing Web Authentication, call
    /// <c>EncodeToBytes(new CBOREncodeOptions("ctap2canonical=true"))</c>
    /// rather than this method.</para></summary>
    /// <returns>A byte array in CBOR format.</returns>
    public byte[] EncodeToBytes() {
      return this.EncodeToBytes(CBOREncodeOptions.Default);
    }

    /// <summary>Writes the binary representation of this CBOR object and
    /// returns a byte array of that representation, using the specified
    /// options for encoding the object to CBOR format. For the CTAP2 (FIDO
    /// Client-to-Authenticator Protocol 2) canonical ordering, which is
    /// useful for implementing Web Authentication, call this method as
    /// follows: <c>EncodeToBytes(new
    /// CBOREncodeOptions("ctap2canonical=true"))</c>.</summary>
    /// <param name='options'>Options for encoding the data to
    /// CBOR.</param>
    /// <returns>A byte array in CBOR format.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='options'/> is null.</exception>
    public byte[] EncodeToBytes(CBOREncodeOptions options) {
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      if (options.Ctap2Canonical) {
        return CBORCanonical.CtapCanonicalEncode(this);
      }
      // For some types, a memory stream is a lot of
      // overhead since the amount of memory the types
      // use is fixed and small
      var hasComplexTag = false;
      byte tagbyte = 0;
      bool tagged = this.IsTagged;
      if (this.IsTagged) {
        var taggedItem = (CBORObject)this.itemValue;
        if (taggedItem.IsTagged || this.tagHigh != 0 ||
          (this.tagLow >> 16) != 0 || this.tagLow >= 24) {
          hasComplexTag = true;
        } else {
          tagbyte = (byte)(0xc0 + this.tagLow);
        }
      }
      if (!hasComplexTag) {
        switch (this.ItemType) {
          case CBORObjectTypeTextString:
          case CBORObjectTypeTextStringAscii: {
              byte[] ret = GetOptimizedBytesIfShortAscii(
                this.AsString(),
                tagged ? (tagbyte & 0xff) : -1);
              if (ret != null) {
                return ret;
              }
              break;
            }
          case CBORObjectTypeTextStringUtf8: {
              if (!tagged && !options.UseIndefLengthStrings) {
                byte[] bytes = (byte[])this.ThisItem;
                return SerializeUtf8(bytes);
              }
              break;
            }
          case CBORObjectTypeSimpleValue: {
              if (tagged) {
                if (this.IsFalse) {
                  return new[] { (byte)tagbyte, (byte)0xf4 };
                }
                if (this.IsTrue) {
                  return new[] { (byte)tagbyte, (byte)0xf5 };
                }
                if (this.IsNull) {
                  return new[] { (byte)tagbyte, (byte)0xf6 };
                }
                if (this.IsUndefined) {
                  return new[] { (byte)tagbyte, (byte)0xf7 };
                }
              } else {
                if (this.IsFalse) {
                  return new[] { (byte)0xf4 };
                }
                if (this.IsTrue) {
                  return new[] { (byte)0xf5 };
                }
                if (this.IsNull) {
                  return new[] { (byte)0xf6 };
                }
                if (this.IsUndefined) {
                  return new[] { (byte)0xf7 };
                }
              }
              break;
            }
          case CBORObjectTypeInteger: {
              var value = (long)this.ThisItem;
              byte[] intBytes;
              if (value >= 0) {
                intBytes = GetPositiveInt64Bytes(0, value);
              } else {
                ++value;
                value = -value; // Will never overflow
                intBytes = GetPositiveInt64Bytes(1, value);
              }
              if (!tagged) {
                return intBytes;
              }
              var ret2 = new byte[intBytes.Length + 1];
              Array.Copy(intBytes, 0, ret2, 1, intBytes.Length);
              ret2[0] = tagbyte;
              return ret2;
            }
          case CBORObjectTypeDouble: {
              return options.Float64 ?
                GetDoubleBytes64(this.AsDoubleBits(), tagbyte & 0xff) :
                GetDoubleBytes(this.AsDoubleBits(), tagbyte & 0xff);
            }
          default:
            break;
        }
      }
      try {
        using (var ms = new MemoryStream(16)) {
          this.WriteTo(ms, options);
          return ms.ToArray();
        }
      } catch (IOException ex) {
        throw new CBORException("I/O Error occurred", ex);
      }
    }

    /// <summary>Gets the CBOR object referred to by a JSON Pointer
    /// according to RFC6901. For more information, see the overload taking
    /// a default value parameter.</summary>
    /// <param name='pointer'>A JSON pointer according to RFC 6901.</param>
    /// <returns>An object within this CBOR object. Returns this object if
    /// pointer is the empty string (even if this object has a CBOR type
    /// other than array or map).</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>Thrown if the pointer
    /// is null, or if the pointer is invalid, or if there is no object at
    /// the given pointer, or the special key "-" appears in the pointer in
    /// the context of an array (not a map), or if the pointer is non-empty
    /// and this object has a CBOR type other than array or
    /// map.</exception>
    public CBORObject AtJSONPointer(string pointer) {
      CBORObject ret = this.AtJSONPointer(pointer, null);
      return ret ?? throw new CBORException("Invalid JSON pointer");
    }

    /// <summary>Gets the CBOR object referred to by a JSON Pointer
    /// according to RFC6901, or a default value if the operation fails.
    /// The syntax for a JSON Pointer is:
    /// <pre>'/' KEY '/' KEY [...]</pre> where KEY represents a key into
    /// the JSON object or its sub-objects in the hierarchy. For example,
    /// <pre>/foo/2/bar</pre> means the same as
    /// <pre>obj['foo'][2]['bar']</pre> in JavaScript. If "~" and/or "/"
    /// occurs in a key, it must be escaped with "~0" or "~1",
    /// respectively, in a JSON pointer. JSON pointers also support the
    /// special key "-" (as in "/foo/-") to indicate the end of an array,
    /// but this method treats this key as an error since it refers to a
    /// nonexistent item. Indices to arrays (such as 2 in the example) must
    /// contain only basic digits 0 to 9 and no leading zeros. (Note that
    /// RFC 6901 was published before JSON was extended to support
    /// top-level values other than arrays and key-value
    /// dictionaries.).</summary>
    /// <param name='pointer'>A JSON pointer according to RFC 6901.</param>
    /// <param name='defaultValue'>The parameter <paramref
    /// name='defaultValue'/> is a Cbor.CBORObject object.</param>
    /// <returns>An object within the specified JSON object. Returns this
    /// object if pointer is the empty string (even if this object has a
    /// CBOR type other than array or map). Returns <paramref
    /// name='defaultValue'/> if the pointer is null, or if the pointer is
    /// invalid, or if there is no object at the given pointer, or the
    /// special key "-" appears in the pointer in the context of an array
    /// (not a map), or if the pointer is non-empty and this object has a
    /// CBOR type other than array or map.</returns>
    public CBORObject AtJSONPointer(string pointer, CBORObject defaultValue) {
      return JSONPointer.GetObject(this, pointer, null);
    }

    /// <summary>Returns a copy of this object after applying the
    /// operations in a JSON patch, in the form of a CBOR object. JSON
    /// patches are specified in RFC 6902 and their format is summarized in
    /// the remarks below.</summary>
    /// <param name='patch'>A JSON patch in the form of a CBOR object; it
    /// has the form summarized in the remarks.</param>
    /// <returns>The result of the patch operation.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>The parameter <paramref
    /// name='patch'/> is null or the patch operation failed.</exception>
    /// <remarks><b>Remarks:</b> A JSON patch is an array with one or more
    /// maps. Each map has the following keys:
    /// <list>
    /// <item>"op" - Required. This key's value is the patch operation and
    /// must be "add", "remove", "move", "copy", "test", or "replace", in
    /// basic lower case letters and no other case combination.</item>
    /// <item>"value" - Required if the operation is "add", "replace", or
    /// "test" and specifies the item to add (insert), or that will replace
    /// the existing item, or to check an existing item for equality,
    /// respectively. (For "test", the operation fails if the existing item
    /// doesn't match the specified value.)</item>
    /// <item>"path" - Required for all operations. A JSON Pointer (RFC
    /// 6901) specifying the destination path in the CBOR object for the
    /// operation. For more information, see RFC 6901 or the documentation
    /// for AtJSONPointer(pointer, defaultValue).</item>
    /// <item>"from" - Required if the operation is "move" or "copy". A
    /// JSON Pointer (RFC 6901) specifying the path in the CBOR object
    /// where the source value is located.</item></list></remarks>
    public CBORObject ApplyJSONPatch(CBORObject patch) {
      return JSONPatch.Patch(this, patch);
    }

    /// <summary>Determines whether this object and another object are
    /// equal and have the same type. Not-a-number values can be considered
    /// equal by this method.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise,
    /// <c>false</c>. In this method, two objects are not equal if they
    /// don't have the same type or if one is null and the other
    /// isn't.</returns>
    public override bool Equals(object obj) {
      return this.Equals(obj as CBORObject);
    }

    /// <summary>Compares the equality of two CBOR objects. Not-a-number
    /// values can be considered equal by this method.</summary>
    /// <param name='other'>The object to compare.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise,
    /// <c>false</c>. In this method, two objects are not equal if they
    /// don't have the same type or if one is null and the other
    /// isn't.</returns>
    public bool Equals(CBORObject other) {
      CBORObject otherValue = other;
      if (otherValue == null) {
        return false;
      }
      if (this == otherValue) {
        return true;
      }
      if ((this.itemtypeValue == CBORObjectTypeTextString ||
this.itemtypeValue == CBORObjectTypeTextStringAscii) &&
        otherValue.itemtypeValue == CBORObjectTypeTextStringUtf8) {
        return CBORUtilities.StringEqualsUtf8(
            (string)this.itemValue,
            (byte[])otherValue.itemValue);
      }
      if ((otherValue.itemtypeValue == CBORObjectTypeTextString ||
otherValue.itemtypeValue == CBORObjectTypeTextStringAscii) &&
        this.itemtypeValue == CBORObjectTypeTextStringUtf8) {
        return CBORUtilities.StringEqualsUtf8(
            (string)otherValue.itemValue,
            (byte[])this.itemValue);
      }
      if ((otherValue.itemtypeValue == CBORObjectTypeTextString &&
this.itemtypeValue == CBORObjectTypeTextStringAscii) || (this.itemtypeValue
== CBORObjectTypeTextString && otherValue.itemtypeValue ==
CBORObjectTypeTextStringAscii)) {
        return Object.Equals(this.itemValue, otherValue.itemValue);
      }
      if (this.itemtypeValue != otherValue.itemtypeValue) {
        return false;
      }
      switch (this.itemtypeValue) {
        case CBORObjectTypeByteString:
        case CBORObjectTypeTextStringUtf8:
          return CBORUtilities.ByteArrayEquals(
              (byte[])this.itemValue,
              otherValue.itemValue as byte[]);
        case CBORObjectTypeMap: {
            return CBORMapEquals(
              this.AsMap(),
              otherValue.itemValue as IDictionary<CBORObject, CBORObject>);
          }
        case CBORObjectTypeArray:
          return CBORArrayEquals(
              this.AsList(),
              otherValue.itemValue as IList<CBORObject>);
        case CBORObjectTypeTagged:
          return this.tagLow == otherValue.tagLow &&
            this.tagHigh == otherValue.tagHigh &&
            Object.Equals(this.itemValue, otherValue.itemValue);
        case CBORObjectTypeDouble:
          return this.AsDoubleBits() == otherValue.AsDoubleBits();
        default: return Object.Equals(this.itemValue, otherValue.itemValue);
      }
    }

    /// <summary>Gets the backing byte array used in this CBOR object, if
    /// this object is a byte string, without copying the data to a new
    /// byte array. Any changes in the returned array's contents will be
    /// reflected in this CBOR object. Note, though, that the array's
    /// length can't be changed.</summary>
    /// <returns>The byte array held by this CBOR object.</returns>
    /// <exception cref='InvalidOperationException'>This object is not a
    /// byte string.</exception>
    public byte[] GetByteString() {
      return this.ItemType == CBORObjectTypeByteString ?
(byte[])this.ThisItem : throw new InvalidOperationException("Not a byte" +
"\u0020string");
    }

    /// <summary>Calculates the hash code of this object. The hash code for
    /// a given instance of this class is not guaranteed to be the same
    /// across versions of this class, and no application or process IDs
    /// are used in the hash code calculation.</summary>
    /// <returns>A 32-bit hash code.</returns>
    public override int GetHashCode() {
      var hashCode = 651869431;
      unchecked {
        if (this.itemValue != null) {
          int itemHashCode;
          long longValue;
          switch (this.itemtypeValue) {
            case CBORObjectTypeByteString:
              itemHashCode =
                CBORUtilities.ByteArrayHashCode(this.GetByteString());
              break;
            case CBORObjectTypeTextStringUtf8:
              itemHashCode = CBORUtilities.Utf8HashCode(
                  (byte[])this.itemValue);
              break;
            case CBORObjectTypeMap:
              itemHashCode = CBORMapHashCode(this.AsMap());
              break;
            case CBORObjectTypeArray:
              itemHashCode = CBORArrayHashCode(this.AsList());
              break;
            case CBORObjectTypeTextString:
            case CBORObjectTypeTextStringAscii:
              itemHashCode = CBORUtilities.StringHashCode(
                  (string)this.itemValue);
              break;
            case CBORObjectTypeSimpleValue:
              itemHashCode = (int)this.itemValue;
              break;
            case CBORObjectTypeDouble:
              longValue = this.AsDoubleBits();
              longValue |= longValue >> 32;
              itemHashCode = unchecked((int)longValue);
              break;
            case CBORObjectTypeInteger:
              longValue = (long)this.itemValue;
              longValue |= longValue >> 32;
              itemHashCode = unchecked((int)longValue);
              break;
            case CBORObjectTypeTagged:
              itemHashCode = unchecked(this.tagLow + this.tagHigh);
              itemHashCode += 651869483 * this.itemValue.GetHashCode();
              break;
            default:
              // EInteger, CBORObject
              itemHashCode = this.itemValue.GetHashCode();
              break;
          }
          hashCode += 651869479 * itemHashCode;
        }
      }
      return hashCode;
    }

    /// <summary>Gets a list of all tags, from outermost to
    /// innermost.</summary>
    /// <returns>An array of tags, or the empty string if this object is
    /// untagged.</returns>
    public EInteger[] GetAllTags() {
      if (!this.IsTagged) {
        return ValueEmptyTags;
      }
      CBORObject curitem = this;
      if (curitem.IsTagged) {
        var extIntegerList = new List<EInteger>();
        while (curitem.IsTagged) {
          extIntegerList.Add(
            LowHighToEInteger(
              curitem.tagLow,
              curitem.tagHigh));
          curitem = (CBORObject)curitem.itemValue;
        }
        return extIntegerList.ToArray();
      }
      return new[] { LowHighToEInteger(this.tagLow, this.tagHigh) };
    }

    /// <summary>Returns whether this object has only one tag.</summary>
    /// <returns><c>true</c> if this object has only one tag; otherwise,
    /// <c>false</c>.</returns>
    public bool HasOneTag() {
      return this.IsTagged && !((CBORObject)this.itemValue).IsTagged;
    }

    /// <summary>Returns whether this object has only one tag and that tag
    /// is the given number.</summary>
    /// <param name='tagValue'>The tag number.</param>
    /// <returns><c>true</c> if this object has only one tag and that tag
    /// is the given number; otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='tagValue'/> is less than 0.</exception>
    public bool HasOneTag(int tagValue) {
      return this.HasOneTag() && this.HasMostOuterTag(tagValue);
    }

    /// <summary>Returns whether this object has only one tag and that tag
    /// is the given number, expressed as an arbitrary-precision
    /// integer.</summary>
    /// <param name='bigTagValue'>An arbitrary-precision integer.</param>
    /// <returns><c>true</c> if this object has only one tag and that tag
    /// is the given number; otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigTagValue'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='bigTagValue'/> is less than 0.</exception>
    public bool HasOneTag(EInteger bigTagValue) {
      return this.HasOneTag() && this.HasMostOuterTag(bigTagValue);
    }

    /// <summary>Gets the number of tags this object has.</summary>
    /// <value>The number of tags this object has.</value>
    public int TagCount
    {
      get
      {
        var count = 0;
        CBORObject curitem = this;
        while (curitem.IsTagged) {
          count = checked(count + 1);
          curitem = (CBORObject)curitem.itemValue;
        }
        return count;
      }
    }

    /// <summary>Returns whether this object has an innermost tag and that
    /// tag is of the given number.</summary>
    /// <param name='tagValue'>The tag number.</param>
    /// <returns><c>true</c> if this object has an innermost tag and that
    /// tag is of the given number; otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='tagValue'/> is less than 0.</exception>
    public bool HasMostInnerTag(int tagValue) {
      return tagValue < 0 ? throw new ArgumentException("tagValue(" + tagValue +
          ") is less than 0") : this.IsTagged && this.HasMostInnerTag(
          EInteger.FromInt32(tagValue));
    }

    /// <summary>Returns whether this object has an innermost tag and that
    /// tag is of the given number, expressed as an arbitrary-precision
    /// number.</summary>
    /// <param name='bigTagValue'>The tag number.</param>
    /// <returns><c>true</c> if this object has an innermost tag and that
    /// tag is of the given number; otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigTagValue'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='bigTagValue'/> is less than 0.</exception>
    public bool HasMostInnerTag(EInteger bigTagValue) {
      return bigTagValue == null ?
        throw new ArgumentNullException(nameof(bigTagValue)) :
        bigTagValue.Sign < 0 ?
        throw new ArgumentException("bigTagValue(" + bigTagValue +
          ") is less than 0") :
        this.IsTagged && this.MostInnerTag.Equals(bigTagValue);
    }

    /// <summary>Returns whether this object has an outermost tag and that
    /// tag is of the given number.</summary>
    /// <param name='tagValue'>The tag number.</param>
    /// <returns><c>true</c> if this object has an outermost tag and that
    /// tag is of the given number; otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='tagValue'/> is less than 0.</exception>
    public bool HasMostOuterTag(int tagValue) {
      return tagValue < 0 ? throw new ArgumentException("tagValue(" + tagValue +
          ") is less than 0") :
        this.IsTagged && this.tagHigh == 0 && this.tagLow == tagValue;
    }

    /// <summary>Returns whether this object has an outermost tag and that
    /// tag is of the given number.</summary>
    /// <param name='bigTagValue'>The tag number.</param>
    /// <returns><c>true</c> if this object has an outermost tag and that
    /// tag is of the given number; otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigTagValue'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='bigTagValue'/> is less than 0.</exception>
    public bool HasMostOuterTag(EInteger bigTagValue) {
      return bigTagValue == null ?
        throw new ArgumentNullException(nameof(bigTagValue)) :
        bigTagValue.Sign < 0 ?
        throw new ArgumentException("bigTagValue(" + bigTagValue +
          ") is less than 0") :
        this.IsTagged && this.MostOuterTag.Equals(bigTagValue);
    }

    /// <summary>Returns whether this object has a tag of the given
    /// number.</summary>
    /// <param name='tagValue'>The tag value to search for.</param>
    /// <returns><c>true</c> if this object has a tag of the given number;
    /// otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='tagValue'/> is less than 0.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='tagValue'/> is null.</exception>
    public bool HasTag(int tagValue) {
      if (tagValue < 0) {
        throw new ArgumentException("tagValue(" + tagValue +
          ") is less than 0");
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
          throw new ArgumentNullException(nameof(tagValue));
        }
#endif
      }
    }

    /// <summary>Returns whether this object has a tag of the given
    /// number.</summary>
    /// <param name='bigTagValue'>The tag value to search for.</param>
    /// <returns><c>true</c> if this object has a tag of the given number;
    /// otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigTagValue'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='bigTagValue'/> is less than 0.</exception>
    public bool HasTag(EInteger bigTagValue) {
      if (bigTagValue == null) {
        throw new ArgumentNullException(nameof(bigTagValue));
      }
      if (bigTagValue.Sign < 0) {
        throw new ArgumentException("doesn't satisfy bigTagValue.Sign>= 0");
      }
      EInteger[] bigTags = this.GetAllTags();
      foreach (EInteger bigTag in bigTags) {
        if (bigTagValue.Equals(bigTag)) {
          return true;
        }
      }
      return false;
    }

    /// <summary>Inserts an object at the specified position in this CBOR
    /// array.</summary>
    /// <param name='index'>Index starting at 0 to insert at.</param>
    /// <param name='valueOb'>An object representing the value, which will
    /// be converted to a CBORObject. Can be null, in which case this value
    /// is converted to CBORObject.Null.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='InvalidOperationException'>This object is not an
    /// array.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='valueOb'/> has an unsupported type; or <paramref
    /// name='index'/> is not a valid index into this array.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    [Obsolete("Use the CBORObject overload instead.")]
    public CBORObject Insert(int index, object valueOb) {
      if (this.Type == CBORType.Array) {
        CBORObject mapValue;
        IList<CBORObject> list = this.AsList();
        if (index < 0 || index > list.Count) {
          throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (valueOb == null) {
          mapValue = CBORObject.Null;
        } else {
          mapValue = valueOb as CBORObject;
          mapValue = mapValue ?? CBORObject.FromObject(valueOb);
        }
        list.Insert(
          index,
          mapValue);
      } else {
        throw new InvalidOperationException("Not an array");
      }
      return this;
    }

    /// <summary>Inserts a CBORObject at the specified position in this
    /// CBOR array.</summary>
    /// <param name='index'>Index starting at 0 to insert at.</param>
    /// <param name='cborObj'>A CBORObject representing the value.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='InvalidOperationException'>This object is not an
    /// array.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='index'/> is not a valid index into this array.</exception>
    public CBORObject Insert(int index, CBORObject cborObj) {
      if (this.Type == CBORType.Array) {
        IList<CBORObject> list = this.AsList();
        if (index < 0 || index > list.Count) {
          throw new ArgumentOutOfRangeException(nameof(index));
        }
        list.Insert(
          index,
          cborObj);
      } else {
        throw new InvalidOperationException("Not an array");
      }
      return this;
    }

    /// <summary>Removes all items from this CBOR array or all keys and
    /// values from this CBOR map.</summary>
    /// <exception cref='InvalidOperationException'>This object is not a
    /// CBOR array or CBOR map.</exception>
    public void Clear() {
      if (this.Type == CBORType.Array) {
        IList<CBORObject> list = this.AsList();
        list.Clear();
      } else if (this.Type == CBORType.Map) {
        IDictionary<CBORObject, CBORObject> dict = this.AsMap();
        dict.Clear();
      } else {
        throw new InvalidOperationException("Not a map or array");
      }
    }

    /// <summary>If this object is an array, removes the first instance of
    /// the specified item (once converted to a CBOR object) from the
    /// array. If this object is a map, removes the item with the given key
    /// (once converted to a CBOR object) from the map.</summary>
    /// <param name='obj'>The item or key (once converted to a CBOR object)
    /// to remove.</param>
    /// <returns><c>true</c> if the item was removed; otherwise,
    /// <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='obj'/> is null (as opposed to CBORObject.Null).</exception>
    /// <exception cref='InvalidOperationException'>The object is not an
    /// array or map.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public bool Remove(object obj) {
      return this.Remove(CBORObject.FromObject(obj));
    }

    /// <summary>Removes the item at the given index of this CBOR
    /// array.</summary>
    /// <param name='index'>The index, starting at 0, of the item to
    /// remove.</param>
    /// <returns>Returns "true" if the object was removed. Returns "false"
    /// if the given index is less than 0, or is at least as high as the
    /// number of items in the array.</returns>
    /// <exception cref='InvalidOperationException'>This object is not a
    /// CBOR array.</exception>
    public bool RemoveAt(int index) {
      if (this.ItemType != CBORObjectTypeArray) {
        throw new InvalidOperationException("Not an array");
      }
      if (index < 0 || index >= this.Count) {
        return false;
      }
      IList<CBORObject> list = this.AsList();
      list.RemoveAt(index);
      return true;
    }

    /// <summary>If this object is an array, removes the first instance of
    /// the specified item from the array. If this object is a map, removes
    /// the item with the given key from the map.</summary>
    /// <param name='obj'>The item or key to remove.</param>
    /// <returns><c>true</c> if the item was removed; otherwise,
    /// <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='obj'/> is null (as opposed to CBORObject.Null).</exception>
    /// <exception cref='InvalidOperationException'>The object is not an
    /// array or map.</exception>
    public bool Remove(CBORObject obj) {
      if (obj == null) {
        throw new ArgumentNullException(nameof(obj));
      }
      if (this.Type == CBORType.Map) {
        IDictionary<CBORObject, CBORObject> dict = this.AsMap();
        return PropertyMap.DictRemove(dict, obj);
      }
      if (this.Type == CBORType.Array) {
        IList<CBORObject> list = this.AsList();
        return list.Remove(obj);
      }
      throw new InvalidOperationException("Not a map or array");
    }

    /// <summary>Maps an object to a key in this CBOR map, or adds the
    /// value if the key doesn't exist. If this is a CBOR array, instead
    /// sets the value at the given index to the given value.</summary>
    /// <param name='key'>If this instance is a CBOR map, this parameter is
    /// an object representing the key, which will be converted to a
    /// CBORObject; in this case, this parameter can be null, in which case
    /// this value is converted to CBORObject.Null. If this instance is a
    /// CBOR array, this parameter must be a 32-bit signed integer(
    /// <c>int</c> ) identifying the index (starting from 0) of the item to
    /// set in the array.</param>
    /// <param name='valueOb'>An object representing the value, which will
    /// be converted to a CBORObject. Can be null, in which case this value
    /// is converted to CBORObject.Null.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='InvalidOperationException'>This object is not a
    /// map or an array.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='key'/> or <paramref name='valueOb'/> has an unsupported type,
    /// or this instance is a CBOR array and <paramref name='key'/> is less
    /// than 0, is the size of this array or greater, or is not a 32-bit
    /// signed integer ( <c>int</c> ).</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    [Obsolete("Use the CBORObject overload instead.")]
    public CBORObject Set(object key, object valueOb) {
      if (this.Type == CBORType.Map) {
        CBORObject mapKey;
        CBORObject mapValue;
        if (key == null) {
          mapKey = CBORObject.Null;
        } else {
          mapKey = key as CBORObject;
          mapKey = mapKey ?? CBORObject.FromObject(key);
        }
        if (valueOb == null) {
          mapValue = CBORObject.Null;
        } else {
          mapValue = valueOb as CBORObject;
          mapValue = mapValue ?? CBORObject.FromObject(valueOb);
        }
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        map[mapKey] = mapValue;
      } else if (this.Type == CBORType.Array) {
        if (key is int) {
          IList<CBORObject> list = this.AsList();
          var index = (int)key;
          if (index < 0 || index >= this.Count) {
            throw new ArgumentOutOfRangeException(nameof(key));
          }
          CBORObject mapValue;
          if (valueOb == null) {
            mapValue = CBORObject.Null;
          } else {
            mapValue = valueOb as CBORObject;
            mapValue = mapValue ?? CBORObject.FromObject(valueOb);
          }
          list[index] = mapValue;
        } else {
          throw new ArgumentException("Is an array, but key is not int");
        }
      } else {
        throw new InvalidOperationException("Not a map or array");
      }
      return this;
    }

    /// <summary>Maps an object to a key in this CBOR map, or adds the
    /// value if the key doesn't exist.</summary>
    /// <param name='mapKey'>If this instance is a CBOR map, this parameter
    /// is an object representing the key, which will be converted to a
    /// CBORObject; in this case, this parameter can be null, in which case
    /// this value is converted to CBORObject.Null.</param>
    /// <param name='mapValue'>A CBORObject representing the value, which
    /// should be of type CBORType.Map.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='InvalidOperationException'>This object is not a
    /// map.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='mapValue'/> or this instance is a CBOR array.</exception>
    public CBORObject Set(CBORObject mapKey, CBORObject mapValue) {
      if (this.Type == CBORType.Map) {
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        map[mapKey] = mapValue;
      } else if (this.Type == CBORType.Array) {
        throw new ArgumentException("mapValue is an array, but key is not int");
      } else {
        throw new ArgumentException("mapValue i not a map or array");
      }
      return this;
    }

    /// <summary>Sets the value of a CBORObject of type Array at the given
    /// index to the given value.</summary>
    /// <param name='key'>This parameter must be a 32-bit signed integer(
    /// <c>int</c> ) identifying the index (starting from 0) of the item to
    /// set in the array.</param>
    /// <param name='mapValue'>An CBORObject representing the
    /// value.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='InvalidOperationException'>MapValue is not a an
    /// array.</exception>
    public CBORObject Set(int key, CBORObject mapValue) {
      if (this.Type == CBORType.Map) {
        CBORObject mapKey = CBORObject.FromInt32(key);
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        map[mapKey] = mapValue;
      } else if (this.Type == CBORType.Array) {
        IList<CBORObject> list = this.AsList();
        var index = (int)key;
        if (index < 0 || index >= this.Count) {
          throw new ArgumentOutOfRangeException(nameof(key));
        }
        list[index] = mapValue;
      } else {
        throw new InvalidOperationException("Not an array");
      }
      return this;
    }

    /// <summary>Converts this object to a text string in JavaScript Object
    /// Notation (JSON) format. See the overload to ToJSONString taking a
    /// JSONOptions argument for further information.
    /// <para>If the CBOR object contains CBOR maps, or is a CBOR map
    /// itself, the order in which the keys to the map are written out to
    /// the JSON string is undefined unless the map was created using the
    /// NewOrderedMap method. Map keys other than untagged text strings are
    /// converted to JSON strings before writing them out (for example,
    /// <c>22("Test")</c> is converted to <c>"Test"</c> and <c>true</c> is
    /// converted to <c>"true"</c> ). After such conversion, if two or more
    /// keys for the same map are identical, this method throws a
    /// CBORException. The example code given in
    /// <b>PeterO.Cbor.CBORObject.ToJSONString(PeterO.Cbor.JSONOptions)</b>
    /// can be used to write out certain keys of a CBOR map in a given
    /// order to a JSON string, or to write out a CBOR object as part of a
    /// JSON text sequence.</para>
    /// <para><b>Warning:</b> In general, if this CBOR object contains
    /// integer map keys or uses other features not supported in JSON, and
    /// the application converts this CBOR object to JSON and back to CBOR,
    /// the application
    /// <i>should not</i> expect the new CBOR object to be exactly the same
    /// as the original. This is because the conversion in many cases may
    /// have to convert unsupported features in JSON to supported features
    /// which correspond to a different feature in CBOR (such as converting
    /// integer map keys, which are supported in CBOR but not JSON, to text
    /// strings, which are supported in both).</para></summary>
    /// <returns>A text string containing the converted object in JSON
    /// format.</returns>
    public string ToJSONString() {
      return this.ToJSONString(JSONOptions.Default);
    }

    /// <summary>
    ///  Converts this object to a text string in JavaScript
    /// Object Notation (JSON) format, using the specified
    /// options to control the encoding process. This function
    /// works not only with arrays and maps, but also integers,
    /// strings, byte arrays, and other JSON data types. Notes:
    ///
    /// <list type=''><item>If this object contains maps with non-string
    /// keys, the keys are converted to JSON strings before writing the map
    /// as a JSON string.</item>
    ///  <item>If this object represents a number
    /// (the IsNumber property, or isNumber() method in Java, returns
    /// true), then it is written out as a number.</item>
    ///  <item>If the CBOR
    /// object contains CBOR maps, or is a CBOR map itself, the order in
    /// which the keys to the map are written out to the JSON string is
    /// undefined unless the map was created using the NewOrderedMap
    /// method. Map keys other than untagged text strings are converted to
    /// JSON strings before writing them out (for example,
    /// <c>22("Test")</c>
    ///  is converted to <c>"Test"</c>
    ///  and <c>true</c>
    ///  is
    /// converted to <c>"true"</c>
    ///  ). After such conversion, if two or more
    /// keys for the same map are identical, this method throws a
    /// CBORException.</item>
    ///  <item>If a number in the form of an
    /// arbitrary-precision binary floating-point number has a very high
    /// binary exponent, it will be converted to a double before being
    /// converted to a JSON string. (The resulting double could overflow to
    /// infinity, in which case the arbitrary-precision binary
    /// floating-point number is converted to null.)</item>
    ///  <item>The
    /// string will not begin with a byte-order mark (U+FEFF); RFC 8259
    /// (the JSON specification) forbids placing a byte-order mark at the
    /// beginning of a JSON string.</item>
    ///  <item>Byte strings are converted
    /// to Base64 URL without whitespace or padding by default (see section
    /// 3.4.5.3 of RFC 8949). A byte string will instead be converted to
    /// traditional base64 without whitespace and with padding if it has
    /// tag 22, or base16 for tag 23. (To create a CBOR object with a given
    /// tag, call the <c>CBORObject.FromCBORObjectAndTag</c>
    ///  method and
    /// pass the CBOR object and the desired tag number to that
    /// method.)</item>
    ///  <item>Rational numbers will be converted to their
    /// exact form, if possible, otherwise to a high-precision
    /// approximation. (The resulting approximation could overflow to
    /// infinity, in which case the rational number is converted to
    /// null.)</item>
    ///  <item>Simple values other than true and false will be
    /// converted to null. (This doesn't include floating-point
    /// numbers.)</item>
    ///  <item>Infinity and not-a-number will be converted
    /// to null.</item>
    ///  </list>
    /// <para><b>Warning:</b>
    ///  In general, if this CBOR object contains
    /// integer map keys or uses other features not supported in JSON, and
    /// the application converts this CBOR object to JSON and back to CBOR,
    /// the application <i>should not</i>
    ///  expect the new CBOR object to be
    /// exactly the same as the original. This is because the conversion in
    /// many cases may have to convert unsupported features in JSON to
    /// supported features which correspond to a different feature in CBOR
    /// (such as converting integer map keys, which are supported in CBOR
    /// but not JSON, to text strings, which are supported in both).</para>
    /// <para>The example code given below (originally written in C# for
    /// the.NET version) can be used to write out certain keys of a CBOR
    /// map in a given order to a JSON string.</para>
    /// <code>/* Generates a JSON string of 'mapObj' whose keys are in the order
    /// given
    /// in 'keys' . Only keys found in 'keys' will be written if they exist in
    /// 'mapObj'. */ private static string KeysToJSONMap(CBORObject mapObj,
    /// IList&lt;CBORObject&gt; keys) { if (mapObj == null) { throw new
    /// ArgumentNullException)nameof(mapObj));}
    /// if (keys == null) { throw new
    /// ArgumentNullException)nameof(keys));}
    /// if (obj.Type != CBORType.Map) {
    /// throw new ArgumentException("'obj' is not a map."); } StringBuilder
    /// builder = new StringBuilder(); var first = true; builder.Append("{");
    /// for (CBORObject key in keys) { if (mapObj.ContainsKey(key)) { if
    /// (!first) {builder.Append(", ");} var keyString=(key.CBORType ==
    /// CBORType.String) ? key.AsString() : key.ToJSONString();
    /// builder.Append(CBORObject.FromObject(keyString) .ToJSONString())
    /// .Append(":").Append(mapObj[key].ToJSONString()); first=false; } } return
    /// builder.Append("}").ToString(); }</code>
    ///  .
    /// </summary>
    /// <param name='options'>Specifies options to control writing the CBOR
    /// object to JSON.</param>
    /// <returns>A text string containing the converted object in JSON
    /// format.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='options'/> is null.</exception>
    public string ToJSONString(JSONOptions options) {
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      CBORType type = this.Type;
      switch (type) {
        case CBORType.Boolean:
        case CBORType.SimpleValue:
          {
            return this.IsTrue ? "true" : (this.IsFalse ? "false" : "null");
          }
        case CBORType.Integer:
          {
            return this.AsEIntegerValue().ToString();
          }
        case CBORType.FloatingPoint:
          {
            long dblbits = this.AsDoubleBits();
            return CBORUtilities.DoubleBitsFinite(dblbits) ?
                 CBORUtilities.DoubleBitsToString(dblbits) : "null";
          }
        default:
          {
            var sb = new StringBuilder();
            try {
              CBORJsonWriter.WriteJSONToInternal(
                this,
                new StringOutput(sb),
                options);
            } catch (IOException ex) {
              // This is truly exceptional
              throw new InvalidOperationException("Internal error", ex);
            }
            return sb.ToString();
          }
      }
    }

    /// <summary>Returns this CBOR object in a text form intended to be
    /// read by humans. The value returned by this method is not intended
    /// to be parsed by computer programs, and the exact text of the value
    /// may change at any time between versions of this library.
    /// <para>The returned string is not necessarily in JavaScript Object
    /// Notation (JSON); to convert CBOR objects to JSON strings, use the
    /// <b>PeterO.Cbor.CBORObject.ToJSONString(PeterO.Cbor.JSONOptions)</b>
    /// method instead.</para></summary>
    /// <returns>A text representation of this object.</returns>
    public override string ToString() {
      return CBORDataUtilities.ToStringHelper(this, 0);
    }

    /// <summary>Gets an object with the same value as this one but without
    /// the tags it has, if any. If this object is an array, map, or byte
    /// string, the data will not be copied to the returned object, so
    /// changes to the returned object will be reflected in this
    /// one.</summary>
    /// <returns>A CBOR object.</returns>
    public CBORObject Untag() {
      CBORObject curobject = this;
      while (curobject.itemtypeValue == CBORObjectTypeTagged) {
        curobject = (CBORObject)curobject.itemValue;
      }
      return curobject;
    }

    /// <summary>Gets an object with the same value as this one but without
    /// this object's outermost tag, if any. If this object is an array,
    /// map, or byte string, the data will not be copied to the returned
    /// object, so changes to the returned object will be reflected in this
    /// one.</summary>
    /// <returns>A CBOR object.</returns>
    public CBORObject UntagOne() {
      return (this.itemtypeValue == CBORObjectTypeTagged) ?
        ((CBORObject)this.itemValue) : this;
    }

    /// <summary>Converts this object to a text string in JavaScript Object
    /// Notation (JSON) format, as in the ToJSONString method, and writes
    /// that string to a data stream in UTF-8. If the CBOR object contains
    /// CBOR maps, or is a CBOR map, the order in which the keys to the map
    /// are written out to the JSON string is undefined unless the map was
    /// created using the NewOrderedMap method. The example code given in
    /// <b>PeterO.Cbor.CBORObject.ToJSONString(PeterO.Cbor.JSONOptions)</b>
    /// can be used to write out certain keys of a CBOR map in a given
    /// order to a JSON string.</summary>
    /// <param name='outputStream'>A writable data stream.</param>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='outputStream'/> is null.</exception>
    /// <example>
    /// <para>The following example (originally written in C# for the.NET
    /// version) writes out a CBOR object as part of a JSON text sequence
    /// (RFC 7464).</para>
    /// <code>
    /// stream.WriteByte(0x1e); &#x2f;&#x2a; RS &#x2a;&#x2f;
    /// cborObject.WriteJSONTo(stream); &#x2f;&#x2a; JSON &#x2a;&#x2f;
    /// stream.WriteByte(0x0a); &#x2f;&#x2a; LF &#x2a;&#x2f;
    /// </code>
    /// <para>The following example (originally written in C# for the.NET
    /// version) shows how to use the <c>LimitedMemoryStream</c>
    ///  class
    /// (implemented in <i>LimitedMemoryStream.cs</i>
    ///  in the peteroupc/CBOR
    /// open-source repository) to limit the size of supported JSON
    /// serializations of CBOR objects.</para>
    /// <code>
    /// &#x2f;&#x2a; maximum supported JSON size in bytes&#x2a;&#x2f;
    /// var maxSize = 20000;
    /// using (var ms = new LimitedMemoryStream(maxSize)) {
    /// cborObject.WriteJSONTo(ms);
    /// var bytes = ms.ToArray();
    /// }
    /// </code>
    /// <para>The following example (written in Java for the Java version)
    /// shows how to use a subclassed <c>OutputStream</c>
    ///  together with a
    /// <c>ByteArrayOutputStream</c>
    ///  to limit the size of supported JSON
    /// serializations of CBOR objects.</para>
    /// <code>
    /// &#x2f;&#x2a; maximum supported JSON size in bytes&#x2a;&#x2f;
    /// final int maxSize = 20000;
    /// ByteArrayOutputStream ba = new ByteArrayOutputStream();
    /// &#x2f;&#x2a; throws UnsupportedOperationException if too big&#x2a;&#x2f;
    /// cborObject.WriteJSONTo(new FilterOutputStream(ba) {
    /// private int size = 0;
    /// public void write(byte[] b, int off, int len) throws IOException {
    /// if (len&gt;(maxSize-size)) {
    /// throw new UnsupportedOperationException();
    /// }
    /// size+=len; out.write(b, off, len);
    /// }
    /// public void write(byte b) throws IOException {
    /// if (size &gt;= maxSize) {
    /// throw new UnsupportedOperationException();
    /// }
    /// size++; out.write(b);
    /// }
    /// });
    /// byte[] bytes = ba.toByteArray();
    /// </code>
    /// <para>The following example (originally written in C# for the.NET
    /// version) shows how to use a.NET MemoryStream to limit the size of
    /// supported JSON serializations of CBOR objects. The disadvantage is
    /// that the extra memory needed to do so can be wasteful, especially
    /// if the average serialized object is much smaller than the maximum
    /// size given (for example, if the maximum size is 20000 bytes, but
    /// the average serialized object has a size of 50 bytes).</para>
    /// <code>
    /// var backing = new byte[20000]; &#x2f;&#x2a; maximum supported JSON size in
    /// bytes&#x2a;&#x2f;
    /// byte[] bytes1, bytes2;
    /// using (var ms = new MemoryStream(backing)) {
    /// &#x2f;&#x2a; throws NotSupportedException if too big&#x2a;&#x2f;
    /// cborObject.WriteJSONTo(ms);
    /// bytes1 = new byte[ms.Position];
    /// &#x2f;&#x2a; Copy serialized data if successful&#x2a;&#x2f;
    /// System.ArrayCopy(backing, 0, bytes1, 0, (int)ms.Position);
    /// &#x2f;&#x2a; Reset memory stream&#x2a;&#x2f;
    /// ms.Position = 0;
    /// cborObject2.WriteJSONTo(ms);
    /// bytes2 = new byte[ms.Position];
    /// &#x2f;&#x2a; Copy serialized data if successful&#x2a;&#x2f;
    /// System.ArrayCopy(backing, 0, bytes2, 0, (int)ms.Position);
    /// }
    /// </code>
    /// </example>
    public void WriteJSONTo(Stream outputStream) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      CBORJsonWriter.WriteJSONToInternal(
        this,
        new StringOutput(outputStream),
        JSONOptions.Default);
    }

    /// <summary>Converts this object to a text string in JavaScript Object
    /// Notation (JSON) format, as in the ToJSONString method, and writes
    /// that string to a data stream in UTF-8, using the given JSON options
    /// to control the encoding process. If the CBOR object contains CBOR
    /// maps, or is a CBOR map, the order in which the keys to the map are
    /// written out to the JSON string is undefined unless the map was
    /// created using the NewOrderedMap method. The example code given in
    /// <b>PeterO.Cbor.CBORObject.ToJSONString(PeterO.Cbor.JSONOptions)</b>
    /// can be used to write out certain keys of a CBOR map in a given
    /// order to a JSON string.</summary>
    /// <param name='outputStream'>A writable data stream.</param>
    /// <param name='options'>An object containing the options to control
    /// writing the CBOR object to JSON.</param>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='outputStream'/> is null.</exception>
    public void WriteJSONTo(Stream outputStream, JSONOptions options) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      CBORJsonWriter.WriteJSONToInternal(
        this,
        new StringOutput(outputStream),
        options);
    }

    /// <summary>Generates a CBOR object from a floating-point number
    /// represented by its bits.</summary>
    /// <param name='floatingBits'>The bits of a floating-point number
    /// number to write.</param>
    /// <param name='byteCount'>The number of bytes of the stored
    /// floating-point number; this also specifies the format of the
    /// "floatingBits" parameter. This value can be 2 if "floatingBits"'s
    /// lowest (least significant) 16 bits identify the floating-point
    /// number in IEEE 754r binary16 format; or 4 if "floatingBits"'s
    /// lowest (least significant) 32 bits identify the floating-point
    /// number in IEEE 754r binary32 format; or 8 if "floatingBits"
    /// identifies the floating point number in IEEE 754r binary64 format.
    /// Any other values for this parameter are invalid.</param>
    /// <returns>A CBOR object storing the given floating-point
    /// number.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='byteCount'/> is other than 2, 4, or 8.</exception>
    public static CBORObject FromFloatingPointBits(
      long floatingBits,
      int byteCount) {
      long value;
      switch (byteCount) {
        case 2:
          value = CBORUtilities.HalfToDoublePrecision(
              unchecked((int)(floatingBits & 0xffffL)));
          return new CBORObject(CBORObjectTypeDouble, value);
        case 4:

          value = CBORUtilities.SingleToDoublePrecision(
              unchecked((int)(floatingBits & 0xffffffffL)));
          return new CBORObject(CBORObjectTypeDouble, value);
        case 8:
          return new CBORObject(CBORObjectTypeDouble, floatingBits);
        default: throw new ArgumentOutOfRangeException(nameof(byteCount));
      }
    }

    /// <summary>Writes the bits of a floating-point number in CBOR format
    /// to a data stream.</summary>
    /// <param name='outputStream'>A writable data stream.</param>
    /// <param name='floatingBits'>The bits of a floating-point number
    /// number to write.</param>
    /// <param name='byteCount'>The number of bytes of the stored
    /// floating-point number; this also specifies the format of the
    /// "floatingBits" parameter. This value can be 2 if "floatingBits"'s
    /// lowest (least significant) 16 bits identify the floating-point
    /// number in IEEE 754r binary16 format; or 4 if "floatingBits"'s
    /// lowest (least significant) 32 bits identify the floating-point
    /// number in IEEE 754r binary32 format; or 8 if "floatingBits"
    /// identifies the floating point number in IEEE 754r binary64 format.
    /// Any other values for this parameter are invalid. This method will
    /// write one plus this many bytes to the data stream.</param>
    /// <returns>The number of 8-bit bytes ordered to be written to the
    /// data stream.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='byteCount'/> is other than 2, 4, or 8.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='outputStream'/> is null.</exception>
    public static int WriteFloatingPointBits(
      Stream outputStream,
      long floatingBits,
      int byteCount) {
      return WriteFloatingPointBits(
          outputStream,
          floatingBits,
          byteCount,
          false);
    }

    /// <summary>Writes the bits of a floating-point number in CBOR format
    /// to a data stream.</summary>
    /// <param name='outputStream'>A writable data stream.</param>
    /// <param name='floatingBits'>The bits of a floating-point number
    /// number to write.</param>
    /// <param name='byteCount'>The number of bytes of the stored
    /// floating-point number; this also specifies the format of the
    /// "floatingBits" parameter. This value can be 2 if "floatingBits"'s
    /// lowest (least significant) 16 bits identify the floating-point
    /// number in IEEE 754r binary16 format; or 4 if "floatingBits"'s
    /// lowest (least significant) 32 bits identify the floating-point
    /// number in IEEE 754r binary32 format; or 8 if "floatingBits"
    /// identifies the floating point number in IEEE 754r binary64 format.
    /// Any other values for this parameter are invalid.</param>
    /// <param name='shortestForm'>If true, writes the shortest form of the
    /// floating-point number that preserves its value. If false, this
    /// method will write the number in the form given by 'floatingBits' by
    /// writing one plus the number of bytes given by 'byteCount' to the
    /// data stream.</param>
    /// <returns>The number of 8-bit bytes ordered to be written to the
    /// data stream.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='byteCount'/> is other than 2, 4, or 8.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='outputStream'/> is null.</exception>
    public static int WriteFloatingPointBits(
      Stream outputStream,
      long floatingBits,
      int byteCount,
      bool shortestForm) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      if (shortestForm) {
        if (byteCount == 8) {
          int bits =
            CBORUtilities.DoubleToHalfPrecisionIfSameValue(floatingBits);
          if (bits != -1) {
            return WriteFloatingPointBits(outputStream, bits, 2, false);
          }
          if (CBORUtilities.DoubleRetainsSameValueInSingle(floatingBits)) {
            bits = CBORUtilities.DoubleToRoundedSinglePrecision(floatingBits);
            return WriteFloatingPointBits(outputStream, bits, 4, false);
          }
        } else if (byteCount == 4) {
          int bits =

            CBORUtilities.SingleToHalfPrecisionIfSameValue(
              unchecked((int)floatingBits));
          if (bits != -1) {
            return WriteFloatingPointBits(outputStream, bits, 2, false);
          }
        }
      }
      byte[] bytes;
      switch (byteCount) {
        case 2:
          bytes = new byte[] {
            0xf9,
            (byte)((floatingBits >> 8) & 0xffL),
            (byte)(floatingBits & 0xffL),
          };
          outputStream.Write(bytes, 0, 3);
          return 3;
        case 4:
          bytes = new byte[] {
            0xfa,
            (byte)((floatingBits >> 24) & 0xffL),
            (byte)((floatingBits >> 16) & 0xffL),
            (byte)((floatingBits >> 8) & 0xffL),
            (byte)(floatingBits & 0xffL),
          };
          outputStream.Write(bytes, 0, 5);
          return 5;
        case 8:
          bytes = new byte[] {
            0xfb,
            (byte)((floatingBits >> 56) & 0xffL),
            (byte)((floatingBits >> 48) & 0xffL),
            (byte)((floatingBits >> 40) & 0xffL),
            (byte)((floatingBits >> 32) & 0xffL),
            (byte)((floatingBits >> 24) & 0xffL),
            (byte)((floatingBits >> 16) & 0xffL),
            (byte)((floatingBits >> 8) & 0xffL),
            (byte)(floatingBits & 0xffL),
          };
          outputStream.Write(bytes, 0, 9);
          return 9;
        default: throw new ArgumentOutOfRangeException(nameof(byteCount));
      }
    }

    /// <summary>Writes a 64-bit binary floating-point number in CBOR
    /// format to a data stream, either in its 64-bit form, or its rounded
    /// 32-bit or 16-bit equivalent.</summary>
    /// <param name='outputStream'>A writable data stream.</param>
    /// <param name='doubleVal'>The double-precision floating-point number
    /// to write.</param>
    /// <param name='byteCount'>The number of 8-bit bytes of the stored
    /// number. This value can be 2 to store the number in IEEE 754r
    /// binary16, rounded to nearest, ties to even; or 4 to store the
    /// number in IEEE 754r binary32, rounded to nearest, ties to even; or
    /// 8 to store the number in IEEE 754r binary64. Any other values for
    /// this parameter are invalid.</param>
    /// <returns>The number of 8-bit bytes ordered to be written to the
    /// data stream.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='byteCount'/> is other than 2, 4, or 8.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='outputStream'/> is null.</exception>
    public static int WriteFloatingPointValue(
      Stream outputStream,
      double doubleVal,
      int byteCount) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      long bits;
      switch (byteCount) {
        case 2:
          bits = CBORUtilities.DoubleToInt64Bits(doubleVal);
          bits = CBORUtilities.DoubleToRoundedHalfPrecision(bits);
          bits &= 0xffffL;
          return WriteFloatingPointBits(outputStream, bits, 2);
        case 4:
          bits = CBORUtilities.DoubleToInt64Bits(doubleVal);
          bits = CBORUtilities.DoubleToRoundedSinglePrecision(bits);
          bits &= 0xffffffffL;
          return WriteFloatingPointBits(outputStream, bits, 4);
        case 8:
          bits = CBORUtilities.DoubleToInt64Bits(doubleVal);
          return WriteFloatingPointBits(outputStream, bits, 8);
        default: throw new ArgumentOutOfRangeException(nameof(byteCount));
      }
    }

    /// <summary>Writes a 32-bit binary floating-point number in CBOR
    /// format to a data stream, either in its 64- or 32-bit form, or its
    /// rounded 16-bit equivalent.</summary>
    /// <param name='outputStream'>A writable data stream.</param>
    /// <param name='singleVal'>The single-precision floating-point number
    /// to write.</param>
    /// <param name='byteCount'>The number of 8-bit bytes of the stored
    /// number. This value can be 2 to store the number in IEEE 754r
    /// binary16, rounded to nearest, ties to even; or 4 to store the
    /// number in IEEE 754r binary32; or 8 to store the number in IEEE 754r
    /// binary64. Any other values for this parameter are invalid.</param>
    /// <returns>The number of 8-bit bytes ordered to be written to the
    /// data stream.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='byteCount'/> is other than 2, 4, or 8.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='outputStream'/> is null.</exception>
    public static int WriteFloatingPointValue(
      Stream outputStream,
      float singleVal,
      int byteCount) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }

      int bits;
      long longbits;
      switch (byteCount) {
        case 2:
          bits = BitConverter.ToInt32(
              BitConverter.GetBytes((float)singleVal),
              0);
          bits = CBORUtilities.SingleToRoundedHalfPrecision(bits);
          bits &= 0xffff;
          return WriteFloatingPointBits(outputStream, bits, 2);
        case 4:
          bits = BitConverter.ToInt32(
              BitConverter.GetBytes((float)singleVal),
              0);
          longbits = bits & 0xffffffffL;
          return WriteFloatingPointBits(outputStream, longbits, 4);
        case 8:
          bits = BitConverter.ToInt32(
              BitConverter.GetBytes((float)singleVal),
              0);
          longbits = CBORUtilities.SingleToDoublePrecision(bits);
          return WriteFloatingPointBits(outputStream, longbits, 8);
        default: throw new ArgumentOutOfRangeException(nameof(byteCount));
      }
    }

    /// <summary>Writes a CBOR major type number and an integer 0 or
    /// greater associated with it to a data stream, where that integer is
    /// passed to this method as a 64-bit signed integer. This is a
    /// low-level method that is useful for implementing custom CBOR
    /// encoding methodologies. This method encodes the given major type
    /// and value in the shortest form allowed for the major
    /// type.</summary>
    /// <param name='outputStream'>A writable data stream.</param>
    /// <param name='majorType'>The CBOR major type to write. This is a
    /// number from 0 through 7 as follows. 0: integer 0 or greater; 1:
    /// negative integer; 2: byte string; 3: UTF-8 text string; 4: array;
    /// 5: map; 6: tag; 7: simple value. See RFC 8949 for details on these
    /// major types.</param>
    /// <param name='value'>An integer 0 or greater associated with the
    /// major type, as follows. 0: integer 0 or greater; 1: the negative
    /// integer's absolute value is 1 plus this number; 2: length in bytes
    /// of the byte string; 3: length in bytes of the UTF-8 text string; 4:
    /// number of items in the array; 5: number of key-value pairs in the
    /// map; 6: tag number; 7: simple value number, which must be in the
    /// interval [0, 23] or [32, 255].</param>
    /// <returns>The number of bytes ordered to be written to the data
    /// stream.</returns>
    /// <exception cref='ArgumentException'>Value is from 24 to 31 and
    /// major type is 7.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='outputStream'/> is null.</exception>
    /// <remarks>There are other useful things to note when encoding CBOR
    /// that are not covered by this WriteValue method. To mark the start
    /// of an indefinite-length array, write the 8-bit byte 0x9f to the
    /// output stream. To mark the start of an indefinite-length map, write
    /// the 8-bit byte 0xbf to the output stream. To mark the end of an
    /// indefinite-length array or map, write the 8-bit byte 0xff to the
    /// output stream. For examples, see the WriteValue(Stream, int, int)
    /// overload.</remarks>
    public static int WriteValue(
      Stream outputStream,
      int majorType,
      long value) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      if (majorType < 0) {
        throw new ArgumentException("majorType(" + majorType +
          ") is less than 0");
      }
      if (majorType > 7) {
        throw new ArgumentException("majorType(" + majorType +
          ") is more than 7");
      }
      if (value < 0) {
        throw new ArgumentException("value(" + value +
          ") is less than 0");
      }
      if (majorType == 7) {
        if (value > 255) {
          throw new ArgumentException("value(" + value +
            ") is more than 255");
        }
        if (value <= 23) {
          outputStream.WriteByte((byte)(0xe0 + (int)value));
          return 1;
        } else if (value < 32) {
          throw new ArgumentException("value is from 24 to 31 and major" +
            " type is 7");
        } else {
          outputStream.WriteByte(0xf8);
          outputStream.WriteByte((byte)value);
          return 2;
        }
      } else {
        return WritePositiveInt64(majorType, value, outputStream);
      }
    }

    /// <summary>Writes a CBOR major type number and an integer 0 or
    /// greater associated with it to a data stream, where that integer is
    /// passed to this method as a 32-bit signed integer. This is a
    /// low-level method that is useful for implementing custom CBOR
    /// encoding methodologies. This method encodes the given major type
    /// and value in the shortest form allowed for the major
    /// type.</summary>
    /// <param name='outputStream'>A writable data stream.</param>
    /// <param name='majorType'>The CBOR major type to write. This is a
    /// number from 0 through 7 as follows. 0: integer 0 or greater; 1:
    /// negative integer; 2: byte string; 3: UTF-8 text string; 4: array;
    /// 5: map; 6: tag; 7: simple value. See RFC 8949 for details on these
    /// major types.</param>
    /// <param name='value'>An integer 0 or greater associated with the
    /// major type, as follows. 0: integer 0 or greater; 1: the negative
    /// integer's absolute value is 1 plus this number; 2: length in bytes
    /// of the byte string; 3: length in bytes of the UTF-8 text string; 4:
    /// number of items in the array; 5: number of key-value pairs in the
    /// map; 6: tag number; 7: simple value number, which must be in the
    /// interval [0, 23] or [32, 255].</param>
    /// <returns>The number of bytes ordered to be written to the data
    /// stream.</returns>
    /// <exception cref='ArgumentException'>Value is from 24 to 31 and
    /// major type is 7.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='outputStream'/> is null.</exception>
    /// <remarks>There are other useful things to note when encoding CBOR
    /// that are not covered by this WriteValue method. To mark the start
    /// of an indefinite-length array, write the 8-bit byte 0x9f to the
    /// output stream. To mark the start of an indefinite-length map, write
    /// the 8-bit byte 0xbf to the output stream. To mark the end of an
    /// indefinite-length array or map, write the 8-bit byte 0xff to the
    /// output stream.</remarks>
    /// <example>
    /// <para>In the following example, an array of three objects is
    /// written as CBOR to a data stream.</para>
    /// <code>&#x2f;&#x2a; array, length 3&#x2a;&#x2f;
    /// CBORObject.WriteValue(stream, 4, 3);
    /// &#x2f;&#x2a; item 1 */
    /// CBORObject.Write("hello world", stream);
    /// CBORObject.Write(25, stream); &#x2f;&#x2a; item 2&#x2a;&#x2f;
    /// CBORObject.Write(false, stream); &#x2f;&#x2a; item 3&#x2a;&#x2f;</code>
    /// <para>In the following example, a map consisting of two key-value
    /// pairs is written as CBOR to a data stream.</para>
    /// <code>CBORObject.WriteValue(stream, 5, 2); &#x2f;&#x2a; map, 2
    /// pairs&#x2a;&#x2f;
    /// CBORObject.Write("number", stream); &#x2f;&#x2a; key 1 */
    /// CBORObject.Write(25, stream); &#x2f;&#x2a; value 1 */
    /// CBORObject.Write("string", stream); &#x2f;&#x2a; key 2&#x2a;&#x2f;
    /// CBORObject.Write("hello", stream); &#x2f;&#x2a; value 2&#x2a;&#x2f;</code>
    /// <para>In the following example (originally written in C# for
    /// the.NET Framework version), a text string is written as CBOR to a
    /// data stream.</para>
    /// <code>string str = "hello world"; byte[] bytes =
    /// DataUtilities.GetUtf8Bytes(str, true); CBORObject.WriteValue(stream, 4,
    /// bytes.Length); stream.Write(bytes, 0, bytes.Length);</code>
    ///  .
    /// </example>
    public static int WriteValue(
      Stream outputStream,
      int majorType,
      int value) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      if (majorType < 0) {
        throw new ArgumentException("majorType(" + majorType +
          ") is less than 0");
      }
      if (majorType > 7) {
        throw new ArgumentException("majorType(" + majorType +
          ") is more than 7");
      }
      if (value < 0) {
        throw new ArgumentException("value(" + value +
          ") is less than 0");
      }
      if (majorType == 7) {
        if (value > 255) {
          throw new ArgumentException("value(" + value +
            ") is more than 255");
        }
        if (value <= 23) {
          outputStream.WriteByte((byte)(0xe0 + value));
          return 1;
        } else if (value < 32) {
          throw new ArgumentException("value is from 24 to 31 and major" +
            "\u0020type" + "\u0020is 7");
        } else {
          outputStream.WriteByte(0xf8);
          outputStream.WriteByte((byte)value);
          return 2;
        }
      } else {
        return WritePositiveInt(majorType, value, outputStream);
      }
    }

    /// <summary>Writes a CBOR major type number and an integer 0 or
    /// greater associated with it to a data stream, where that integer is
    /// passed to this method as an arbitrary-precision integer. This is a
    /// low-level method that is useful for implementing custom CBOR
    /// encoding methodologies. This method encodes the given major type
    /// and value in the shortest form allowed for the major
    /// type.</summary>
    /// <param name='outputStream'>A writable data stream.</param>
    /// <param name='majorType'>The CBOR major type to write. This is a
    /// number from 0 through 7 as follows. 0: integer 0 or greater; 1:
    /// negative integer; 2: byte string; 3: UTF-8 text string; 4: array;
    /// 5: map; 6: tag; 7: simple value. See RFC 8949 for details on these
    /// major types.</param>
    /// <param name='bigintValue'>An integer 0 or greater associated with
    /// the major type, as follows. 0: integer 0 or greater; 1: the
    /// negative integer's absolute value is 1 plus this number; 2: length
    /// in bytes of the byte string; 3: length in bytes of the UTF-8 text
    /// string; 4: number of items in the array; 5: number of key-value
    /// pairs in the map; 6: tag number; 7: simple value number, which must
    /// be in the interval [0, 23] or [32, 255]. For major types 0 to 6,
    /// this number may not be greater than 2^64 - 1.</param>
    /// <returns>The number of bytes ordered to be written to the data
    /// stream.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='majorType'/> is 7 and value is greater than 255.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='outputStream'/> or <paramref name='bigintValue'/> is
    /// null.</exception>
    /// <remarks>There are other useful things to note when encoding CBOR
    /// that are not covered by this WriteValue method. To mark the start
    /// of an indefinite-length array, write the 8-bit byte 0x9f to the
    /// output stream. To mark the start of an indefinite-length map, write
    /// the 8-bit byte 0xbf to the output stream. To mark the end of an
    /// indefinite-length array or map, write the 8-bit byte 0xff to the
    /// output stream.</remarks>
    public static int WriteValue(
      Stream outputStream,
      int majorType,
      EInteger bigintValue) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      if (bigintValue == null) {
        throw new ArgumentNullException(nameof(bigintValue));
      }
      if (bigintValue.Sign < 0) {
        throw new ArgumentException("tagEInt's sign(" + bigintValue.Sign +
          ") is less than 0");
      }
      if (bigintValue.CompareTo(UInt64MaxValue) > 0) {
        throw new ArgumentException(
          "tag more than 18446744073709551615 (" + bigintValue + ")");
      }
      if (bigintValue.CanFitInInt64()) {
        return WriteValue(
            outputStream,
            majorType,
            bigintValue.ToInt64Checked());
      }
      long longVal = bigintValue.ToInt64Unchecked();
      var highbyte = (int)((longVal >> 56) & 0xff);
      if (majorType < 0) {
        throw new ArgumentException("majorType(" + majorType +
          ") is less than 0");
      }
      if (majorType > 7) {
        throw new ArgumentException("majorType(" + majorType +
          ") is more than 7");
      }
      if (majorType == 7) {
        throw new ArgumentException(
          "majorType is 7 and value is greater" + "\u0020than 255");
      }
      byte[] bytes = new[] {
        (byte)(27 | (majorType << 5)), (byte)highbyte,
        (byte)((longVal >> 48) & 0xff), (byte)((longVal >> 40) & 0xff),
        (byte)((longVal >> 32) & 0xff), (byte)((longVal >> 24) & 0xff),
        (byte)((longVal >> 16) & 0xff), (byte)((longVal >> 8) & 0xff),
        (byte)(longVal & 0xff),
      };
      outputStream.Write(bytes, 0, bytes.Length);
      return bytes.Length;
    }

    /// <summary><para>Writes this CBOR object to a data stream. If the
    /// CBOR object contains CBOR maps, or is a CBOR map, the order in
    /// which the keys to the map are written out to the data stream is
    /// undefined unless the map was created using the NewOrderedMap
    /// method. See the examples (originally written in C# for the.NET
    /// version) for ways to write out certain keys of a CBOR map in a
    /// given order. In the case of CBOR objects of type FloatingPoint, the
    /// number is written using the shortest floating-point encoding
    /// possible; this is a change from previous versions.</para>
    /// </summary>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    /// <example>
    /// <para>The following example shows a method that writes each key of
    /// 'mapObj' to 'outputStream', in the order given in 'keys', where
    /// 'mapObj' is written out in the form of a CBOR <b>definite-length
    /// map</b>
    /// . Only keys found in 'keys' will be written if they exist
    /// in 'mapObj'.</para>
    /// <code>private static void WriteKeysToMap(CBORObject mapObj,
    /// IList&lt;CBORObject&gt; keys, Stream outputStream) {
    /// if (mapObj == null) {
    /// throw new ArgumentNullException(nameof(mapObj));}
    /// if (keys == null)
    /// {throw new ArgumentNullException(nameof(keys));}
    /// if (outputStream ==
    /// null) {throw new ArgumentNullException(nameof(outputStream));}
    /// if
    /// (obj.Type!=CBORType.Map) { throw new ArgumentException("'obj' is not a
    /// map."); } int keyCount = 0; for (CBORObject key in keys) { if
    /// (mapObj.ContainsKey(key)) { keyCount++; } }
    /// CBORObject.WriteValue(outputStream, 5, keyCount); for (CBORObject key in
    /// keys) { if (mapObj.ContainsKey(key)) { key.WriteTo(outputStream);
    /// mapObj[key].WriteTo(outputStream); } } }</code>
    /// <para>The following example shows a method that writes each key of
    /// 'mapObj' to 'outputStream', in the order given in 'keys', where
    /// 'mapObj' is written out in the form of a CBOR <b>indefinite-length
    /// map</b>
    /// . Only keys found in 'keys' will be written if they exist
    /// in 'mapObj'.</para>
    /// <code>private static void WriteKeysToIndefMap(CBORObject mapObj,
    /// IList&lt;CBORObject&gt; keys, Stream outputStream) { if (mapObj == null)
    /// { throw new ArgumentNullException(nameof(mapObj));}
    /// if (keys == null)
    /// {throw new ArgumentNullException(nameof(keys));}
    /// if (outputStream ==
    /// null) {throw new ArgumentNullException(nameof(outputStream));}
    /// if
    /// (obj.Type!=CBORType.Map) { throw new ArgumentException("'obj' is not a
    /// map."); } outputStream.WriteByte((byte)0xBF); for (CBORObject key in
    /// keys) { if (mapObj.ContainsKey(key)) { key.WriteTo(outputStream);
    /// mapObj[key].WriteTo(outputStream); } }
    /// outputStream.WriteByte((byte)0xff); }</code>
    /// <para>The following example shows a method that writes out a list
    /// of objects to 'outputStream' as an <b>indefinite-length CBOR
    /// array</b>
    /// .</para>
    /// <code>private static void WriteToIndefArray(IList&lt;object&gt; list,
    /// Stream
    /// outputStream) { if (list == null) { throw new
    /// ArgumentNullException(nameof(list));}
    /// if (outputStream == null) {throw
    /// new ArgumentNullException(nameof(outputStream));}
    /// outputStream.WriteByte((byte)0x9f); for (object item in list) { new
    /// CBORObject(item).WriteTo(outputStream); }
    /// outputStream.WriteByte((byte)0xff); }</code>
    /// <para>The following example (originally written in C# for the.NET
    /// version) shows how to use the <c>LimitedMemoryStream</c>
    ///  class
    /// (implemented in <i>LimitedMemoryStream.cs</i>
    ///  in the peteroupc/CBOR
    /// open-source repository) to limit the size of supported CBOR
    /// serializations.</para>
    /// <code>
    /// &#x2f;&#x2a; maximum supported CBOR size in bytes&#x2a;&#x2f;
    /// var maxSize = 20000;
    /// using (var ms = new LimitedMemoryStream(maxSize)) {
    /// cborObject.WriteTo(ms);
    /// var bytes = ms.ToArray();
    /// }
    /// </code>
    /// <para>The following example (written in Java for the Java version)
    /// shows how to use a subclassed <c>OutputStream</c>
    ///  together with a
    /// <c>ByteArrayOutputStream</c>
    ///  to limit the size of supported CBOR
    /// serializations.</para>
    /// <code>
    /// &#x2f;&#x2a; maximum supported CBOR size in bytes&#x2a;&#x2f;
    /// final int maxSize = 20000;
    /// ByteArrayOutputStream ba = new ByteArrayOutputStream();
    /// &#x2f;&#x2a; throws UnsupportedOperationException if too big&#x2a;&#x2f;
    /// cborObject.WriteTo(new FilterOutputStream(ba) {
    /// private int size = 0;
    /// public void write(byte[] b, int off, int len) throws IOException {
    /// if (len&gt;(maxSize-size)) {
    /// throw new UnsupportedOperationException();
    /// }
    /// size+=len; out.write(b, off, len);
    /// }
    /// public void write(byte b) throws IOException {
    /// if (size &gt;= maxSize) {
    /// throw new UnsupportedOperationException();
    /// }
    /// size++; out.write(b);
    /// }
    /// });
    /// byte[] bytes = ba.toByteArray();
    /// </code>
    /// <para>The following example (originally written in C# for the.NET
    /// version) shows how to use a.NET MemoryStream to limit the size of
    /// supported CBOR serializations. The disadvantage is that the extra
    /// memory needed to do so can be wasteful, especially if the average
    /// serialized object is much smaller than the maximum size given (for
    /// example, if the maximum size is 20000 bytes, but the average
    /// serialized object has a size of 50 bytes).</para>
    /// <code>
    /// var backing = new byte[20000]; &#x2f;&#x2a; maximum supported CBOR size in
    /// bytes&#x2a;&#x2f;
    /// byte[] bytes1, bytes2;
    /// using (var ms = new MemoryStream(backing)) {
    /// &#x2f;&#x2a; throws NotSupportedException if too big&#x2a;&#x2f;
    /// cborObject.WriteTo(ms);
    /// bytes1 = new byte[ms.Position];
    /// &#x2f;&#x2a; Copy serialized data if successful&#x2a;&#x2f;
    /// System.ArrayCopy(backing, 0, bytes1, 0, (int)ms.Position);
    /// &#x2f;&#x2a; Reset memory stream&#x2a;&#x2f;
    /// ms.Position = 0;
    /// cborObject2.WriteTo(ms);
    /// bytes2 = new byte[ms.Position];
    /// &#x2f;&#x2a; Copy serialized data if successful&#x2a;&#x2f;
    /// System.ArrayCopy(backing, 0, bytes2, 0, (int)ms.Position);
    /// }
    /// </code>
    /// </example>
    public void WriteTo(Stream stream) {
      this.WriteTo(stream, CBOREncodeOptions.Default);
    }

    /// <summary>Writes this CBOR object to a data stream, using the
    /// specified options for encoding the data to CBOR format. If the CBOR
    /// object contains CBOR maps, or is a CBOR map, the order in which the
    /// keys to the map are written out to the data stream is undefined
    /// unless the map was created using the NewOrderedMap method. The
    /// example code given in
    /// <see cref='PeterO.Cbor.CBORObject.WriteTo(System.IO.Stream)'/> can
    /// be used to write out certain keys of a CBOR map in a given order.
    /// In the case of CBOR objects of type FloatingPoint, the number is
    /// written using the shortest floating-point encoding possible; this
    /// is a change from previous versions.</summary>
    /// <param name='stream'>A writable data stream.</param>
    /// <param name='options'>Options for encoding the data to
    /// CBOR.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='System.IO.IOException'>An I/O error
    /// occurred.</exception>
    /// <exception cref='ArgumentException'>Unexpected data
    /// type".</exception>
    public void WriteTo(Stream stream, CBOREncodeOptions options) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      if (options.Ctap2Canonical) {
        byte[] bytes = CBORCanonical.CtapCanonicalEncode(this);
        stream.Write(bytes, 0, bytes.Length);
        return;
      }
      this.WriteTags(stream);
      int type = this.ItemType;
      switch (type) {
        case CBORObjectTypeInteger: {
            Write((long)this.ThisItem, stream);
            break;
          }
        case CBORObjectTypeEInteger: {
            Write((EInteger)this.ThisItem, stream);
            break;
          }
        case CBORObjectTypeByteString:
        case CBORObjectTypeTextStringUtf8: {
            byte[] arr = (byte[])this.ThisItem;
            WritePositiveInt(
              (this.Type == CBORType.ByteString) ? 2 : 3,
              arr.Length,
              stream);
            stream.Write(arr, 0, arr.Length);
            break;
          }
        case CBORObjectTypeTextString:
        case CBORObjectTypeTextStringAscii: {
            Write((string)this.ThisItem, stream, options);
            break;
          }
        case CBORObjectTypeArray: {
            WriteObjectArray(this.AsList(), stream, options);
            break;
          }
        case CBORObjectTypeMap: {
            WriteObjectMap(this.AsMap(), stream, options);
            break;
          }
        case CBORObjectTypeSimpleValue: {
            int value = this.SimpleValue;
            if (value < 24) {
              stream.WriteByte((byte)(0xe0 + value));
            } else {
#if DEBUG
              if (value < 32) {
                throw new ArgumentException("value(" + value +
                  ") is less than " + "32");
              }
#endif

              stream.WriteByte(0xf8);
              stream.WriteByte((byte)value);
            }

            break;
          }
        case CBORObjectTypeDouble: {
            WriteFloatingPointBits(
               stream,
               this.AsDoubleBits(),
               8,
               !options.Float64);
            break;
          }
        default: {
            throw new ArgumentException("Unexpected data type");
          }
      }
    }

    internal static CBORObject FromRaw(byte[] bytes) {
      return new CBORObject(CBORObjectTypeByteString, bytes);
    }

    internal static CBORObject FromRawUtf8(byte[] bytes) {
      return new CBORObject(CBORObjectTypeTextStringUtf8, bytes);
    }

    internal static CBORObject FromRaw(string str) {
#if DEBUG
      if (!CBORUtilities.CheckUtf16(str)) {
        throw new InvalidOperationException();
      }
#endif
      return new CBORObject(CBORObjectTypeTextString, str);
    }

    internal static CBORObject FromRaw(IList<CBORObject> list) {
      return new CBORObject(CBORObjectTypeArray, list);
    }

    internal static CBORObject FromRaw(IDictionary<CBORObject, CBORObject>
      map) {
      return new CBORObject(CBORObjectTypeMap, map);
    }

    internal static int GetExpectedLength(int value) {
      return ValueExpectedLengths[value];
    }

    // Generate a CBOR object for head bytes with fixed length.
    // Note that this function assumes that the length of the data
    // was already checked.
    internal static CBORObject GetFixedLengthObject(
      int firstbyte,
      byte[] data) {
      CBORObject fixedObj = FixedObjects[firstbyte];
      if (fixedObj != null) {
        return fixedObj;
      }
      int majortype = firstbyte >> 5;
      if ((firstbyte & 0x1c) == 0x18) {
        // contains 1 to 8 extra bytes of additional information
        long uadditional;
        switch (firstbyte & 0x1f) {
          case 24:
            uadditional = data[1] & 0xff;
            break;
          case 25:
            uadditional = (data[1] & 0xffL) << 8;
            uadditional |= data[2] & 0xffL;
            break;
          case 26:
            uadditional = (data[1] & 0xffL) << 24;
            uadditional |= (data[2] & 0xffL) << 16;
            uadditional |= (data[3] & 0xffL) << 8;
            uadditional |= data[4] & 0xffL;
            break;
          case 27:
            uadditional = (data[1] & 0xffL) << 56;
            uadditional |= (data[2] & 0xffL) << 48;
            uadditional |= (data[3] & 0xffL) << 40;
            uadditional |= (data[4] & 0xffL) << 32;
            uadditional |= (data[5] & 0xffL) << 24;
            uadditional |= (data[6] & 0xffL) << 16;
            uadditional |= (data[7] & 0xffL) << 8;
            uadditional |= data[8] & 0xffL;
            break;
          default: throw new CBORException("Unexpected data encountered");
        }
        switch (majortype) {
          case 0:
            if ((uadditional >> 63) == 0) {
              // use only if additional's top bit isn't set
              // (additional is a signed long)
              return new CBORObject(CBORObjectTypeInteger, uadditional);
            } else {
              int low = unchecked((int)(uadditional & 0xffffffffL));
              int high = unchecked((int)((uadditional >> 32) & 0xffffffffL));
              return FromEInteger(LowHighToEInteger(low, high));
            }
          case 1:
            if ((uadditional >> 63) == 0) {
              // use only if additional's top bit isn't set
              // (additional is a signed long)
              return new CBORObject(
                  CBORObjectTypeInteger,
                  -1 - uadditional);
            } else {
              int low = unchecked((int)(uadditional & 0xffffffffL));
              int high = unchecked((int)((uadditional >> 32) & 0xffffffffL));
              EInteger bigintAdditional = LowHighToEInteger(low, high);
              EInteger minusOne = EInteger.FromInt32(-1);
              bigintAdditional = minusOne.Subtract(bigintAdditional);
              return FromEInteger(bigintAdditional);
            }
          case 7:
            if (firstbyte >= 0xf9 && firstbyte <= 0xfb) {
              long dblbits = uadditional;
              if (firstbyte == 0xf9) {
                dblbits = CBORUtilities.HalfToDoublePrecision(
                    unchecked((int)uadditional));
              } else if (firstbyte == 0xfa) {
                dblbits = CBORUtilities.SingleToDoublePrecision(
                    unchecked((int)uadditional));
              }
              return new CBORObject(
                  CBORObjectTypeDouble,
                  dblbits);
            }
            if (firstbyte == 0xf8) {
              return (int)uadditional < 32 ?
                throw new CBORException("Invalid overlong simple value") :
                new CBORObject(
                  CBORObjectTypeSimpleValue,
                  (int)uadditional);
            }
            throw new CBORException("Unexpected data encountered");
          default: throw new CBORException("Unexpected data encountered");
        }
      }
      if (majortype == 2) { // short byte string
        var ret = new byte[firstbyte - 0x40];
        Array.Copy(data, 1, ret, 0, firstbyte - 0x40);
        return new CBORObject(CBORObjectTypeByteString, ret);
      }
      if (majortype == 3) { // short text string
        var ret = new byte[firstbyte - 0x60];
        Array.Copy(data, 1, ret, 0, firstbyte - 0x60);
        return !CBORUtilities.CheckUtf8(ret) ?
          throw new CBORException("Invalid encoding") :
          new CBORObject(CBORObjectTypeTextStringUtf8, ret);
      }
      if (firstbyte == 0x80) {
        // empty array
        return CBORObject.NewArray();
      }
      if (firstbyte == 0xa0) {
        // empty map
        return CBORObject.NewOrderedMap();
      }
      throw new CBORException("Unexpected data encountered");
    }

    internal static CBORObject GetFixedObject(int value) {
      return FixedObjects[value];
    }

    private IList<CBORObject> AsList() {
      return (IList<CBORObject>)this.ThisItem;
    }

    private IDictionary<CBORObject, CBORObject> AsMap() {
      return (IDictionary<CBORObject, CBORObject>)this.ThisItem;
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
      var ret = 19;
      int count = list.Count;
      unchecked
      {
        ret = (ret * 31) + count;
        for (int i = 0; i < count; ++i) {
          ret = (ret * 31) + list[i].GetHashCode();
        }
      }
      return ret;
    }

    private static bool StringEquals(string str, string str2) {
      if (str == str2) {
        return true;
      }
      if (str.Length != str2.Length) {
        return false;
      }
      int count = str.Length;
      for (int i = 0; i < count; ++i) {
        if (str[i] != str2[i]) {
          return false;
        }
      }
      return true;
    }

    private static bool CBORMapEquals(
      IDictionary<CBORObject, CBORObject> mapA,
      IDictionary<CBORObject, CBORObject> mapB) {
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
        CBORObject valueB = PropertyMap.GetOrDefault(mapB, kvp.Key, null);
        if (valueB == null) {
          return false;
        }
        if (kvp.Value == null) {
          // Null (as opposed to CBORObject.Null) values not supported in CBOR maps.
          throw new InvalidOperationException();
        }
        if (!kvp.Value.Equals(valueB)) {
          return false;
        }
      }
      return true;
    }

    private static int CBORMapHashCode(IDictionary<CBORObject, CBORObject>
      a) {
      // To simplify matters, we use just the count of
      // the map as the basis for the hash code. More complicated
      // hash code calculation would involve the sum of the hash codes of
      // the map's key-value pairs (an approach that works regardless of the order
      // in which map keys are iterated, because wraparound addition
      // is commutative and associative), but this could take much more time
      // to calculate, especially if the keys and values are very big.
      return unchecked(a.Count.GetHashCode() * 19);
    }

    private static void CheckCBORLength(
      long expectedLength,
      long actualLength) {
      if (actualLength < expectedLength) {
        throw new CBORException("Premature end of data");
      }
      if (actualLength > expectedLength) {
        throw new CBORException(
            "Too many bytes. There is data beyond the decoded CBOR object.");
      }
    }

    private static void CheckCBORLength(int expectedLength, int
      actualLength) {
      if (actualLength < expectedLength) {
        throw new CBORException("Premature end of data");
      }
      if (actualLength > expectedLength) {
        throw new CBORException(
            "Too many bytes. There is data beyond the decoded CBOR object.");
      }
    }

    private static string ExtendedToString(EFloat ef) {
      if (ef.IsFinite && (ef.Exponent.CompareTo((EInteger)2500) > 0 ||
          ef.Exponent.CompareTo((EInteger)(-2500)) < 0)) {
        // It can take very long to convert a number with a very high
        // or very low exponent to a decimal string, so do this instead
        return ef.Mantissa + "p" + ef.Exponent;
      }
      return ef.ToString();
    }

    private static byte[] GetOptimizedBytesIfShortAscii(
      string str,
      int tagbyteInt) {
      byte[] bytes;
      if (str.Length <= 255) {
        // The strings will usually be short ASCII strings, so
        // use this optimization
        var offset = 0;
        int length = str.Length;
        int extra = (length < 24) ? 1 : 2;
        if (tagbyteInt >= 0) {
          ++extra;
        }
        bytes = new byte[length + extra];
        if (tagbyteInt >= 0) {
          bytes[offset] = (byte)tagbyteInt;
          ++offset;
        }
        if (length < 24) {
          bytes[offset] = (byte)(0x60 + str.Length);
          ++offset;
        } else {
          bytes[offset] = 0x78;
          bytes[offset + 1] = (byte)str.Length;
          offset += 2;
        }
        var issimple = true;
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

    private static string GetOptimizedStringIfShortAscii(
      byte[] data,
      int offset) {
      int length = data.Length;
      if (length > offset) {
        int nextbyte = data[offset] & 0xff;
        if (nextbyte >= 0x60 && nextbyte < 0x78) {
          int offsetp1 = 1 + offset;
          // Check for type 3 string of short length
          int rightLength = offsetp1 + (nextbyte - 0x60);
          CheckCBORLength(
            rightLength,
            length);
          // Check for all ASCII text
          for (int i = offsetp1; i < length; ++i) {
            if ((data[i] & 0x80) != 0) {
              return null;
            }
          }
          // All ASCII text, so convert to a text string
          // from a char array without having to
          // convert from UTF-8 first
          var c = new char[length - offsetp1];
          for (int i = offsetp1; i < length; ++i) {
            c[i - offsetp1] = (char)(data[i] & 0xff);
          }
          return new String(c);
        }
      }
      return null;
    }

    private static byte[] SerializeUtf8(byte[] utf8) {
      byte[] bytes;
      if (utf8.Length < 24) {
        bytes = new byte[utf8.Length + 1];
        bytes[0] = (byte)(utf8.Length | 0x60);
        Array.Copy(utf8, 0, bytes, 1, utf8.Length);
        return bytes;
      }
      if (utf8.Length <= 0xffL) {
        bytes = new byte[utf8.Length + 2];
        bytes[0] = 0x78;
        bytes[1] = (byte)utf8.Length;
        Array.Copy(utf8, 0, bytes, 2, utf8.Length);
        return bytes;
      }
      if (utf8.Length <= 0xffffL) {
        bytes = new byte[utf8.Length + 3];
        bytes[0] = 0x79;
        bytes[1] = (byte)((utf8.Length >> 8) & 0xff);
        bytes[2] = (byte)(utf8.Length & 0xff);
        Array.Copy(utf8, 0, bytes, 3, utf8.Length);
        return bytes;
      }
      byte[] posbytes = GetPositiveInt64Bytes(3, utf8.Length);
      bytes = new byte[utf8.Length + posbytes.Length];
      Array.Copy(posbytes, 0, bytes, 0, posbytes.Length);
      Array.Copy(utf8, 0, bytes, posbytes.Length, utf8.Length);
      return bytes;
    }

    private static byte[] GetPositiveInt64Bytes(int type, long value) {
      if (value < 0) {
        throw new ArgumentException("value(" + value + ") is less than " +
          "0");
      }
      return value < 24 ? new[] { (byte)((byte)value | (byte)(type << 5)) } :
        value <= 0xffL ? new[] {
          (byte)(24 | (type << 5)), (byte)(value & 0xff),
        } : value <= 0xffffL ? new[] {
   (byte)(25 | (type << 5)),
   (byte)((value >> 8) & 0xff), (byte)(value & 0xff),
 } : value <= 0xffffffffL ? new[] {
   (byte)(26 | (type << 5)),
   (byte)((value >> 24) & 0xff), (byte)((value >> 16) & 0xff),
   (byte)((value >> 8) & 0xff), (byte)(value & 0xff),
 } : new[] {
   (byte)(27 | (type << 5)), (byte)((value >> 56) & 0xff),
   (byte)((value >> 48) & 0xff), (byte)((value >> 40) & 0xff),
   (byte)((value >> 32) & 0xff), (byte)((value >> 24) & 0xff),
   (byte)((value >> 16) & 0xff), (byte)((value >> 8) & 0xff),
   (byte)(value & 0xff),
 };
    }

    private static byte[] GetPositiveIntBytes(int type, int value) {
      return value < 0 ?
        throw new ArgumentException("value(" + value + ") is less than " +
          "0") : value < 24 ?
        new[] { (byte)((byte)value | (byte)(type << 5)) } :
        value <= 0xff ? new[] {
          (byte)(24 | (type << 5)), (byte)(value & 0xff),
        } : value <= 0xffff ? new[] {
   (byte)(25 | (type << 5)),
   (byte)((value >> 8) & 0xff), (byte)(value & 0xff),
 } : new[] {
   (byte)(26 | (type << 5)), (byte)((value >> 24) & 0xff),
   (byte)((value >> 16) & 0xff), (byte)((value >> 8) & 0xff),
   (byte)(value & 0xff),
 };
    }

    // Initialize fixed values for certain
    // head bytes
    private static CBORObject[] InitializeFixedObjects() {
      var fixedObjects = new CBORObject[256];
      for (int i = 0; i < 0x18; ++i) {
        fixedObjects[i] = new CBORObject(CBORObjectTypeInteger, (long)i);
      }
      for (int i = 0x20; i < 0x38; ++i) {
        fixedObjects[i] = new CBORObject(
          CBORObjectTypeInteger,
          (long)(-1 - (i - 0x20)));
      }
      fixedObjects[0x60] = new CBORObject(
        CBORObjectTypeTextString,
        String.Empty);
      for (int i = 0xe0; i < 0xf8; ++i) {
        fixedObjects[i] = new CBORObject(
          CBORObjectTypeSimpleValue,
          i - 0xe0);
      }
      return fixedObjects;
    }

    private static int ListCompare(
      IList<CBORObject> listA,
      IList<CBORObject> listB) {
      if (listA == null) {
        return (listB == null) ? 0 : -1;
      }
      if (listB == null) {
        return 1;
      }
      int listACount = listA.Count;
      int listBCount = listB.Count;
      // NOTE: Compare list counts to conform
      // to bytewise lexicographical ordering
      if (listACount != listBCount) {
        return listACount < listBCount ? -1 : 1;
      }
      for (int i = 0; i < listACount; ++i) {
        int cmp = listA[i].CompareTo(listB[i]);
        if (cmp != 0) {
          return cmp;
        }
      }
      return 0;
    }

    private static EInteger LowHighToEInteger(int tagLow, int tagHigh) {
      byte[] uabytes;
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
        return EInteger.FromBytes(uabytes, true);
      }
      if (tagLow != 0) {
        uabytes = new byte[5];
        uabytes[3] = (byte)((tagLow >> 24) & 0xff);
        uabytes[2] = (byte)((tagLow >> 16) & 0xff);
        uabytes[1] = (byte)((tagLow >> 8) & 0xff);
        uabytes[0] = (byte)(tagLow & 0xff);
        uabytes[4] = 0;
        return EInteger.FromBytes(uabytes, true);
      }
      return EInteger.Zero;
    }

    private static int MapCompare(
      IDictionary<CBORObject, CBORObject> mapA,
      IDictionary<CBORObject, CBORObject> mapB) {
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
      // NOTE: Compare map key counts to conform
      // to bytewise lexicographical ordering
      if (listACount != listBCount) {
        return listACount < listBCount ? -1 : 1;
      }
      var sortedASet = new List<CBORObject>(PropertyMap.GetSortedKeys(mapA));
      var sortedBSet = new List<CBORObject>(PropertyMap.GetSortedKeys(mapB));
      // DebugUtility.Log("---done sorting");
      listACount = sortedASet.Count;
      _ = sortedBSet.Count;
      // Compare the keys
      /* for (var i = 0; i < listACount; ++i) {
        string str = sortedASet[i].ToString();
        str = str.Substring(0, Math.Min(100, str.Length));
        DebugUtility.Log("A " + i + "=" + str);
      }
      for (var i = 0; i < listBCount; ++i) {
        string str = sortedBSet[i].ToString();
        str = str.Substring(0, Math.Min(100, str.Length));
        DebugUtility.Log("B " + i + "=" + str);
      }*/
      for (int i = 0; i < listACount; ++i) {
        CBORObject itemA = sortedASet[i];
        CBORObject itemB = sortedBSet[i];
        if (itemA == null) {
          return -1;
        }
        int cmp = itemA.CompareTo(itemB);
        // string ot = itemA + "/" +
        // (cmp != 0 ? itemB.ToString() : "~") +
        // " -> cmp=" + (cmp);
        // DebugUtility.Log(ot);
        if (cmp != 0) {
          return cmp;
        }
        // Both maps have the same key, so compare
        // the value under that key
        cmp = mapA[itemA].CompareTo(mapB[itemB]);
        // DebugUtility.Log(itemA + "/~" +
        // " -> "+mapA[itemA]+", "+(cmp != 0 ? mapB[itemB].ToString() :
        // "~") + " -> cmp=" + cmp);
        if (cmp != 0) {
          return cmp;
        }
      }
      return 0;
    }

    private static IList<object> PushObject(
      IList<object> stack,
      object parent,
      object child) {
      if (stack == null) {
        stack = new List<object>(4) {
          parent,
        };
      }
      foreach (object o in stack) {
        if (o == child) {
          throw new ArgumentException("Circular reference in data" +
            "\u0020structure");
        }
      }
      stack.Add(child);
      return stack;
    }

    private static int TagsCompare(EInteger[] tagsA, EInteger[] tagsB) {
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
      return (listACount != listBCount) ? ((listACount < listBCount) ? -1 : 1) :
        0;
    }

    private static IList<object> WriteChildObject(
      object parentThisItem,
      CBORObject child,
      Stream outputStream,
      IList<object> stack,
      CBOREncodeOptions options) {
      if (child == null) {
        outputStream.WriteByte(0xf6);
      } else {
        int type = child.ItemType;
        if (type == CBORObjectTypeArray) {
          stack = PushObject(stack, parentThisItem, child.ThisItem);
          child.WriteTags(outputStream);
          WriteObjectArray(child.AsList(), outputStream, stack, options);
          stack.RemoveAt(stack.Count - 1);
        } else if (type == CBORObjectTypeMap) {
          stack = PushObject(stack, parentThisItem, child.ThisItem);
          child.WriteTags(outputStream);
          WriteObjectMap(child.AsMap(), outputStream, stack, options);
          stack.RemoveAt(stack.Count - 1);
        } else {
          child.WriteTo(outputStream, options);
        }
      }
      return stack;
    }

    private static void WriteObjectArray(
      IList<CBORObject> list,
      Stream outputStream,
      CBOREncodeOptions options) {
      WriteObjectArray(list, outputStream, null, options);
    }

    private static void WriteObjectArray(
      IList<CBORObject> list,
      Stream outputStream,
      IList<object> stack,
      CBOREncodeOptions options) {
      object thisObj = list;
      _ = WritePositiveInt(4, list.Count, outputStream);
      foreach (CBORObject i in list) {
        stack = WriteChildObject(thisObj, i, outputStream, stack, options);
      }
    }

    private static void WriteObjectMap(
      IDictionary<CBORObject, CBORObject> map,
      Stream outputStream,
      CBOREncodeOptions options) {
      WriteObjectMap(map, outputStream, null, options);
    }

    private static void WriteObjectMap(
      IDictionary<CBORObject, CBORObject> map,
      Stream outputStream,
      IList<object> stack,
      CBOREncodeOptions options) {
      object thisObj = map;
      _ = WritePositiveInt(5, map.Count, outputStream);
      foreach (KeyValuePair<CBORObject, CBORObject> entry in map) {
        CBORObject key = entry.Key;
        CBORObject value = entry.Value;
        stack = WriteChildObject(
            thisObj,
            key,
            outputStream,
            stack,
            options);
        stack = WriteChildObject(
            thisObj,
            value,
            outputStream,
            stack,
            options);
      }
    }

    private static int WritePositiveInt(int type, int value, Stream s) {
      byte[] bytes = GetPositiveIntBytes(type, value);
      s.Write(bytes, 0, bytes.Length);
      return bytes.Length;
    }

    private static int WritePositiveInt64(int type, long value, Stream s) {
      byte[] bytes = GetPositiveInt64Bytes(type, value);
      s.Write(bytes, 0, bytes.Length);
      return bytes.Length;
    }

    private static void WriteStreamedString(string str, Stream stream) {
      byte[] bytes;
      bytes = GetOptimizedBytesIfShortAscii(str, -1);
      if (bytes != null) {
        stream.Write(bytes, 0, bytes.Length);
        return;
      }
      // Take string's length into account when allocating
      // stream buffer, in case it's much smaller than the usual stream
      // string buffer length and to improve performance on small strings
      int bufferLength = Math.Min(StreamedStringBufferLength, str.Length);
      if (bufferLength < StreamedStringBufferLength) {
        bufferLength = Math.Min(
            StreamedStringBufferLength,
            bufferLength * 3);
      }
      bytes = new byte[bufferLength];
      var byteIndex = 0;
      var streaming = false;
      for (int index = 0; index < str.Length; ++index) {
        int c = str[index];
        if (c <= 0x7f) {
          if (byteIndex >= StreamedStringBufferLength) {
            // Write bytes retrieved so far
            if (!streaming) {
              stream.WriteByte(0x7f);
            }
            _ = WritePositiveInt(3, byteIndex, stream);
            stream.Write(bytes, 0, byteIndex);
            byteIndex = 0;
            streaming = true;
          }
          bytes[byteIndex++] = (byte)c;
        } else if (c <= 0x7ff) {
          if (byteIndex + 2 > StreamedStringBufferLength) {
            // Write bytes retrieved so far - the next two bytes
            // would exceed the length, and the CBOR spec forbids
            // splitting characters when generating text strings
            if (!streaming) {
              stream.WriteByte(0x7f);
            }
            _ = WritePositiveInt(3, byteIndex, stream);
            stream.Write(bytes, 0, byteIndex);
            byteIndex = 0;
            streaming = true;
          }
          bytes[byteIndex++] = (byte)(0xc0 | ((c >> 6) & 0x1f));
          bytes[byteIndex++] = (byte)(0x80 | (c & 0x3f));
        } else {
          if ((c & 0xfc00) == 0xd800 && index + 1 < str.Length &&
            (str[index + 1] & 0xfc00) == 0xdc00) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c & 0x3ff) << 10) + (str[index + 1] & 0x3ff);
            ++index;
          } else if ((c & 0xf800) == 0xd800) {
            // unpaired surrogate, write U+FFFD instead
            c = 0xfffd;
          }
          if (c <= 0xffff) {
            if (byteIndex + 3 > StreamedStringBufferLength) {
              // Write bytes retrieved so far - the next three bytes
              // would exceed the length, and the CBOR spec forbids
              // splitting characters when generating text strings
              if (!streaming) {
                stream.WriteByte(0x7f);
              }
              _ = WritePositiveInt(3, byteIndex, stream);
              stream.Write(bytes, 0, byteIndex);
              byteIndex = 0;
              streaming = true;
            }
            bytes[byteIndex++] = (byte)(0xe0 | ((c >> 12) & 0x0f));
            bytes[byteIndex++] = (byte)(0x80 | ((c >> 6) & 0x3f));
            bytes[byteIndex++] = (byte)(0x80 | (c & 0x3f));
          } else {
            if (byteIndex + 4 > StreamedStringBufferLength) {
              // Write bytes retrieved so far - the next four bytes
              // would exceed the length, and the CBOR spec forbids
              // splitting characters when generating text strings
              if (!streaming) {
                stream.WriteByte(0x7f);
              }
              _ = WritePositiveInt(3, byteIndex, stream);
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
      _ = WritePositiveInt(3, byteIndex, stream);
      stream.Write(bytes, 0, byteIndex);
      if (streaming) {
        stream.WriteByte(0xff);
      }
    }

    private int AsInt32(int minValue, int maxValue) {
      var cn = CBORNumber.FromCBORObject(this);
      return cn == null ?
        throw new InvalidOperationException("not a number type") :
        cn.GetNumberInterface().AsInt32(
          cn.GetValue(),
          minValue,
          maxValue);
    }

    private void WriteTags(Stream s) {
      CBORObject curobject = this;
      while (curobject.IsTagged) {
        int low = curobject.tagLow;
        int high = curobject.tagHigh;
        if (high == 0 && (low >> 16) == 0) {
          _ = WritePositiveInt(6, low, s);
        } else if (high == 0) {
          long value = low & 0xffffffffL;
          _ = WritePositiveInt64(6, value, s);
        } else if ((high >> 16) == 0) {
          long value = low & 0xffffffffL;
          long highValue = high & 0xffffffffL;
          value |= highValue << 32;
          _ = WritePositiveInt64(6, value, s);
        } else {
          byte[] arrayToWrite = {
            0xdb,
            (byte)((high >> 24) & 0xff), (byte)((high >> 16) & 0xff),
            (byte)((high >> 8) & 0xff), (byte)(high & 0xff),
            (byte)((low >> 24) & 0xff), (byte)((low >> 16) & 0xff),
            (byte)((low >> 8) & 0xff), (byte)(low & 0xff),
          };
          s.Write(arrayToWrite, 0, 9);
        }
        curobject = (CBORObject)curobject.itemValue;
      }
    }
  }
}
