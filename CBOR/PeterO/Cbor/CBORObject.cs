/*
Written by Peter O. in 2013-2018.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PeterO;
using PeterO.Numbers;

namespace PeterO.Cbor {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Cbor.CBORObject"]/*'/>
  public sealed partial class CBORObject : IComparable<CBORObject>,
  IEquatable<CBORObject> {
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
    public static readonly CBORObject NaN = CBORObject.FromObject(Double.NaN);

    /// <summary>The value negative infinity.</summary>
    public static readonly CBORObject NegativeInfinity =
      CBORObject.FromObject(Double.NegativeInfinity);

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
      CBORObject.FromObject(Double.PositiveInfinity);

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
          ((EInteger)item).GetSignedBitLengthAsEInteger().CompareTo(64) > 0) {
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.Count"]/*'/>
    public int Count {
      get {
        return (this.ItemType == CBORObjectTypeArray) ? this.AsList().Count :
          ((this.ItemType == CBORObjectTypeMap) ? this.AsMap().Count : 0);
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.MostInnerTag"]/*'/>
    public EInteger MostInnerTag {
      get {
        if (!this.IsTagged) {
          return EInteger.FromInt32(-1);
        }
        CBORObject previtem = this;
        var curitem = (CBORObject)this.itemValue;
        while (curitem.IsTagged) {
          previtem = curitem;
          curitem = (CBORObject)curitem.itemValue;
        }
        if (previtem.tagHigh == 0 && previtem.tagLow >= 0 &&
            previtem.tagLow < 0x10000) {
          return (EInteger)previtem.tagLow;
        }
        return LowHighToEInteger(
          previtem.tagLow,
          previtem.tagHigh);
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.IsFalse"]/*'/>
    public bool IsFalse {
      get {
        return this.ItemType == CBORObjectTypeSimpleValue && (int)this.ThisItem
          == 20;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.IsFinite"]/*'/>
    public bool IsFinite {
      get {
        return this.IsNumber && !this.IsInfinity() &&
          !this.IsNaN();
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.IsIntegral"]/*'/>
    public bool IsIntegral {
      get {
        CBORNumber cn = CBORNumber.FromCBORObject(this);
        return (cn != null) &&
cn.GetNumberInterface().IsIntegral(cn.GetValue());
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.IsNull"]/*'/>
    public bool IsNull {
      get {
        return this.ItemType == CBORObjectTypeSimpleValue && (int)this.ThisItem
          == 22;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.IsTagged"]/*'/>
    public bool IsTagged {
      get {
        return this.itemtypeValue == CBORObjectTypeTagged;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.IsTrue"]/*'/>
    public bool IsTrue {
      get {
        return this.ItemType == CBORObjectTypeSimpleValue && (int)this.ThisItem
          == 21;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.IsUndefined"]/*'/>
    public bool IsUndefined {
      get {
        return this.ItemType == CBORObjectTypeSimpleValue && (int)this.ThisItem
          == 23;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.IsZero"]/*'/>
    public bool IsZero {
      get {
        CBORNumber cn = CBORNumber.FromCBORObject(this);
        return cn != null &&
cn.GetNumberInterface().IsNumberZero(cn.GetValue());
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.Keys"]/*'/>
    public ICollection<CBORObject> Keys {
      get {
        if (this.ItemType == CBORObjectTypeMap) {
          IDictionary<CBORObject, CBORObject> dict = this.AsMap();
          return dict.Keys;
        }
        throw new InvalidOperationException("Not a map");
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.IsNegative"]/*'/>
    public bool IsNegative {
      get {
        CBORNumber cn = CBORNumber.FromCBORObject(this);
        return (cn != null) &&
cn.GetNumberInterface().IsNegative(cn.GetValue());
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.MostOuterTag"]/*'/>
    public EInteger MostOuterTag {
      get {
        if (!this.IsTagged) {
          return EInteger.FromInt32(-1);
        }
        if (this.tagHigh == 0 &&
            this.tagLow >= 0 && this.tagLow < 0x10000) {
          return (EInteger)this.tagLow;
        }
        return LowHighToEInteger(
          this.tagLow,
          this.tagHigh);
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.Sign"]/*'/>
    public int Sign {
      get {
        CBORNumber cn = CBORNumber.FromCBORObject(this);
        int ret = cn == null ? 2 : cn.GetNumberInterface().Sign(cn.GetValue());
        if (ret == 2) {
          throw new InvalidOperationException("This object is not a number.");
        }
        return ret;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.SimpleValue"]/*'/>
    public int SimpleValue {
      get {
        return (this.ItemType == CBORObjectTypeSimpleValue) ?
          ((int)this.ThisItem) : (-1);
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.IsNumber"]/*'/>
    public bool IsNumber {
      get {
        return CBORNumber.IsNumber(this);
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.Type"]/*'/>
    public CBORType Type {
      get {
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
            return CBORType.TextString;
          default:
            throw new InvalidOperationException("Unexpected data type");
        }
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.Values"]/*'/>
    public ICollection<CBORObject> Values {
      get {
        if (this.ItemType == CBORObjectTypeMap) {
          IDictionary<CBORObject, CBORObject> dict = this.AsMap();
          return dict.Values;
        }
        if (this.ItemType == CBORObjectTypeArray) {
          IList<CBORObject> list = this.AsList();
          return new
            System.Collections.ObjectModel.ReadOnlyCollection<CBORObject>(list);
        }
        throw new InvalidOperationException("Not a map or array");
      }
    }

    private int ItemType {
      get {
        CBORObject curobject = this;
        while (curobject.itemtypeValue == CBORObjectTypeTagged) {
          curobject = (CBORObject)curobject.itemValue;
        }
        return curobject.itemtypeValue;
      }
    }

    private object ThisItem {
      get {
        CBORObject curobject = this;
        while (curobject.itemtypeValue == CBORObjectTypeTagged) {
          curobject = (CBORObject)curobject.itemValue;
        }
        return curobject.itemValue;
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Cbor.CBORObject.Item(System.Int32)"]/*'/>
    public CBORObject this[int index] {
      get {
        if (this.ItemType == CBORObjectTypeArray) {
          IList<CBORObject> list = this.AsList();
          if (index < 0 || index >= list.Count) {
            throw new ArgumentOutOfRangeException(nameof(index));
          }
          return list[index];
        }
        if (this.ItemType == CBORObjectTypeMap) {
          IDictionary<CBORObject, CBORObject> map = this.AsMap();
          CBORObject key = CBORObject.FromObject(index);
          return (!map.ContainsKey(key)) ? null : map[key];
        }
        throw new InvalidOperationException("Not an array or map");
      }

      set {
        if (this.ItemType == CBORObjectTypeArray) {
          if (value == null) {
            throw new ArgumentNullException(nameof(value));
          }
          IList<CBORObject> list = this.AsList();
          if (index < 0 || index >= list.Count) {
            throw new ArgumentOutOfRangeException(nameof(index));
          }
          list[index] = value;
        } else if (this.ItemType == CBORObjectTypeMap) {
          IDictionary<CBORObject, CBORObject> map = this.AsMap();
          CBORObject key = CBORObject.FromObject(index);
          map[key] = value;
        } else {
          throw new InvalidOperationException("Not an array or map");
        }
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.GetOrDefault(System.Object,PeterO.Cbor.CBORObject)"]/*'/>
    public CBORObject GetOrDefault(object key, CBORObject defaultValue) {
      if (this.ItemType == CBORObjectTypeArray) {
        var index = 0;
        if (key is int) {
          index = (int)key;
        } else {
          CBORObject cborkey = CBORObject.FromObject(key);
          if (!cborkey.IsIntegral) {
            return defaultValue;
          }
          if (!cborkey.CanTruncatedIntFitInInt32()) {
            return defaultValue;
          }
          index = cborkey.AsInt32();
        }
        IList<CBORObject> list = this.AsList();
        return (index < 0 || index >= list.Count) ? defaultValue :
                    list[index];
      }
      if (this.ItemType == CBORObjectTypeMap) {
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        CBORObject ckey = CBORObject.FromObject(key);
        return (!map.ContainsKey(ckey)) ? defaultValue : map[ckey];
      }
      return defaultValue;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Cbor.CBORObject.Item(PeterO.Cbor.CBORObject)"]/*'/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1043",
      Justification = "Represents a logical data store")]
    public CBORObject this[CBORObject key] {
/* "The CBORObject class represents a logical data store." +
" Also, an Object indexer is not included here because it's unusual for " +
"CBOR map keys to be anything other than text strings or integers; " +
"including an Object indexer would introduce the security issues present " +
"in the FromObject method because of the need to convert to CBORObject;" +
" and this CBORObject indexer is included here because any CBOR object " +
"can serve as a map key, not just integers or text strings." */
      get {
        if (key == null) {
          throw new ArgumentNullException(nameof(key));
        }
        if (this.ItemType == CBORObjectTypeMap) {
          IDictionary<CBORObject, CBORObject> map = this.AsMap();
          return (!map.ContainsKey(key)) ? null : map[key];
        }
        if (this.ItemType == CBORObjectTypeArray) {
          if (!key.IsIntegral) {
            throw new ArgumentException("Not an integer");
          }
          if (!key.CanTruncatedIntFitInInt32()) {
            throw new ArgumentOutOfRangeException(nameof(key));
          }
          IList<CBORObject> list = this.AsList();
          int index = key.AsInt32();
          if (index < 0 || index >= list.Count) {
            throw new ArgumentOutOfRangeException(nameof(key));
          }
          return list[index];
        }
        throw new InvalidOperationException("Not an array or map");
      }

      set {
        if (key == null) {
          throw new ArgumentNullException(nameof(value));
        }
        if (value == null) {
          throw new ArgumentNullException(nameof(value));
        }
        if (this.ItemType == CBORObjectTypeMap) {
          IDictionary<CBORObject, CBORObject> map = this.AsMap();
          map[key] = value;
          return;
        }
        if (this.ItemType == CBORObjectTypeArray) {
          if (!key.IsIntegral) {
            throw new ArgumentException("Not an integer");
          }
          if (!key.CanTruncatedIntFitInInt32()) {
            throw new ArgumentOutOfRangeException(nameof(key));
          }
          IList<CBORObject> list = this.AsList();
          int index = key.AsInt32();
          if (index < 0 || index >= list.Count) {
            throw new ArgumentOutOfRangeException(nameof(key));
          }
          list[index] = value;
          return;
        }
        throw new InvalidOperationException("Not an array or map");
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Cbor.CBORObject.Item(System.String)"]/*'/>
    public CBORObject this[string key] {
      get {
        if (key == null) {
          throw new ArgumentNullException(nameof(key));
        }
        CBORObject objkey = CBORObject.FromObject(key);
        return this[objkey];
      }

      set {
        if (key == null) {
          throw new ArgumentNullException(nameof(value));
        }
        if (value == null) {
          throw new ArgumentNullException(nameof(value));
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Addition(PeterO.Cbor.CBORObject,PeterO.Cbor.CBORObject)"]/*'/>
    public static CBORObject Addition(CBORObject first, CBORObject second) {
      if (first == null) {
        throw new ArgumentNullException(nameof(first));
      }
      if (second == null) {
        throw new ArgumentNullException(nameof(second));
      }
      CBORNumber a = CBORNumber.FromCBORObject(first);
      if (a == null) {
        throw new ArgumentException(nameof(first) + "does not represent a" +
           "\u0020number");
      }
      CBORNumber b = CBORNumber.FromCBORObject(second);
      if (b == null) {
        throw new ArgumentException(nameof(second) + "does not represent a" +
           "\u0020number");
      }
      return a.Add(b).ToCBORObject();
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.DecodeFromBytes(System.Byte[])"]/*'/>
    public static CBORObject DecodeFromBytes(byte[] data) {
      return DecodeFromBytes(data, CBOREncodeOptions.Default);
    }

    private static readonly CBOREncodeOptions AllowEmptyOptions =
           new CBOREncodeOptions("allowempty=1");

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.DecodeSequenceFromBytes(System.Byte[])"]/*'/>
    public static CBORObject[] DecodeSequenceFromBytes(byte[] data) {
      return DecodeSequenceFromBytes(data, AllowEmptyOptions);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.DecodeSequenceFromBytes(System.Byte[],PeterO.Cbor.CBOREncodeOptions)"]/*'/>
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
      return (CBORObject[])cborList.ToArray();
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.DecodeFromBytes(System.Byte[],PeterO.Cbor.CBOREncodeOptions)"]/*'/>
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
        if (options.AllowEmpty) {
          return null;
        }
        throw new CBORException("data is empty.");
      }
      var firstbyte = (int)(data[0] & (int)0xff);
      int expectedLength = ValueExpectedLengths[firstbyte];
      // if invalid
      if (expectedLength == -1) {
        throw new CBORException("Unexpected data encountered");
      }
      if (expectedLength != 0) {
        // if fixed length
        CheckCBORLength(expectedLength, data.Length);
      }
      if (firstbyte == 0xc0) {
        // value with tag 0
        string s = GetOptimizedStringIfShortAscii(data, 1);
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
      using (var ms = new MemoryStream(data)) {
        CBORObject o = Read(ms, options);
        CheckCBORLength((long)data.Length, (long)ms.Position);
        return o;
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Divide(PeterO.Cbor.CBORObject,PeterO.Cbor.CBORObject)"]/*'/>
    public static CBORObject Divide(CBORObject first, CBORObject second) {
      if (first == null) {
        throw new ArgumentNullException(nameof(first));
      }
      if (second == null) {
        throw new ArgumentNullException(nameof(second));
      }
      CBORNumber a = CBORNumber.FromCBORObject(first);
      if (a == null) {
        throw new ArgumentException(nameof(first) + "does not represent a" +
           "\u0020number");
      }
      CBORNumber b = CBORNumber.FromCBORObject(second);
      if (b == null) {
        throw new ArgumentException(nameof(second) + "does not represent a" +
           "\u0020number");
      }
      return a.Divide(b).ToCBORObject();
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromJSONString(System.String)"]/*'/>
    public static CBORObject FromJSONString(string str) {
      return FromJSONString(str, CBOREncodeOptions.Default);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromJSONString(System.String,PeterO.Cbor.CBOREncodeOptions)"]/*'/>
    public static CBORObject FromJSONString(
      string str,
      CBOREncodeOptions options) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      if (str.Length > 0 && str[0] == 0xfeff) {
        throw new CBORException(
          "JSON object began with a byte order mark (U+FEFF) (offset 0)");
      }
      var reader = new CharacterInputWithCount(
        new CharacterReader(str, false, true));
      var nextchar = new int[1];
      CBORObject obj = CBORJson.ParseJSONValue(
        reader,
        !options.AllowDuplicateKeys,
        false,
        nextchar);
      if (nextchar[0] != -1) {
        reader.RaiseError("End of string not reached");
      }
      return obj;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.ToObject(System.Type)"]/*'/>
    public object ToObject(Type t) {
      return this.ToObject(t, null, null, 0);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.ToObject(System.Type,PeterO.Cbor.CBORTypeMapper)"]/*'/>
    public object ToObject(Type t, CBORTypeMapper mapper) {
      if (mapper == null) {
        throw new ArgumentNullException(nameof(mapper));
      }
      return this.ToObject(t, mapper, null, 0);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.ToObject(System.Type,PeterO.Cbor.PODOptions)"]/*'/>
    public object ToObject(Type t, PODOptions options) {
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      return this.ToObject(t, null, options, 0);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.ToObject(System.Type,PeterO.Cbor.CBORTypeMapper,PeterO.Cbor.PODOptions)"]/*'/>
    public object ToObject(Type t, CBORTypeMapper mapper, PODOptions options) {
      if (mapper == null) {
        throw new ArgumentNullException(nameof(mapper));
      }
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      return this.ToObject(t, mapper, options, 0);
    }

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
      return t.Equals(typeof(string)) ? this.AsString() :
        PropertyMap.TypeToObject(this, t, mapper, options, depth);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.Int64)"]/*'/>
    public static CBORObject FromObject(long value) {
      return (value >= 0L && value < 24L) ? FixedObjects[(int)value] :
        new CBORObject(CBORObjectTypeInteger, value);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(PeterO.Cbor.CBORObject)"]/*'/>
    public static CBORObject FromObject(CBORObject value) {
      return value ?? CBORObject.Null;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(PeterO.Numbers.EInteger)"]/*'/>
    public static CBORObject FromObject(EInteger bigintValue) {
      if ((object)bigintValue == (object)null) {
        return CBORObject.Null;
      }
      if (bigintValue.CanFitInInt64()) {
        return CBORObject.FromObject(bigintValue.ToInt64Checked());
      } else {
        EInteger bitLength = bigintValue.GetSignedBitLengthAsEInteger();
        if (bitLength.CompareTo(64) <= 0) {
          // Fits in major type 0 or 1
          return new CBORObject(CBORObjectTypeEInteger, bigintValue);
        } else {
          int tag = (bigintValue.Sign < 0) ? 3 : 2;
          return CBORObject.FromObjectAndTag(
             EIntegerBytes(bigintValue),
             tag);
        }
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(PeterO.Numbers.EFloat)"]/*'/>
    public static CBORObject FromObject(EFloat bigValue) {
      if ((object)bigValue == (object)null) {
        return CBORObject.Null;
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
        cbor = CBORObject.NewArray().Add(bigValue.Exponent)
            .Add(bigValue.UnsignedMantissa).Add(options);
        tag = 269;
      } else {
        EInteger bitLength = bigValue.Exponent.GetSignedBitLengthAsEInteger();
        tag = bitLength.CompareTo(64) > 0 ? 265 : 5;
        cbor = CBORObject.NewArray()
            .Add(bigValue.Exponent).Add(bigValue.Mantissa);
      }
      return CBORObject.FromObjectAndTag(cbor, tag);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(PeterO.Numbers.ERational)"]/*'/>
    public static CBORObject FromObject(ERational bigValue) {
      if ((object)bigValue == (object)null) {
        return CBORObject.Null;
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
"\u0020!bigValue.IsInfinity() || bigValue.UnsignedNumerator.IsZero");
        }
        if (!(!bigValue.IsInfinity() || bigValue.Denominator.CompareTo(1) ==
0)) {
          throw new InvalidOperationException("doesn't satisfy" +
"\u0020!bigValue.IsInfinity() || bigValue.Denominator.CompareTo(1)==0");
        }
        if (!(!bigValue.IsNaN() || bigValue.Denominator.CompareTo(1) == 0)) {
          throw new InvalidOperationException("doesn't satisfy" +
"\u0020!bigValue.IsNaN() ||" +
"\u0020bigValue.Denominator.CompareTo(1)==0");
        }
#endif

        cbor = CBORObject.NewArray().Add(bigValue.UnsignedNumerator)
            .Add(bigValue.Denominator).Add(options);
        tag = 270;
      } else {
        tag = 30;
        cbor = CBORObject.NewArray()
            .Add(bigValue.Numerator).Add(bigValue.Denominator);
      }
      return CBORObject.FromObjectAndTag(cbor, tag);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(PeterO.Numbers.EDecimal)"]/*'/>
    public static CBORObject FromObject(EDecimal bigValue) {
      if ((object)bigValue == (object)null) {
        return CBORObject.Null;
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
        cbor = CBORObject.NewArray().Add(bigValue.Exponent)
            .Add(bigValue.UnsignedMantissa).Add(options);
        tag = 268;
      } else {
        EInteger bitLength = bigValue.Exponent.GetSignedBitLengthAsEInteger();
        tag = bitLength.CompareTo(64) > 0 ? 264 : 4;
        cbor = CBORObject.NewArray()
            .Add(bigValue.Exponent).Add(bigValue.Mantissa);
      }
      return CBORObject.FromObjectAndTag(cbor, tag);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.String)"]/*'/>
    public static CBORObject FromObject(string strValue) {
      if (strValue == null) {
        return CBORObject.Null;
      }
      if (DataUtilities.GetUtf8Length(strValue, false) < 0) {
        throw new
        ArgumentException("String contains an unpaired surrogate code point.");
      }
      return new CBORObject(CBORObjectTypeTextString, strValue);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.Int32)"]/*'/>
    public static CBORObject FromObject(int value) {
      return (value >= 0 && value < 24) ? FixedObjects[value] :
        FromObject((long)value);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.Int16)"]/*'/>
    public static CBORObject FromObject(short value) {
      return (value >= 0 && value < 24) ? FixedObjects[value] :
        FromObject((long)value);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.Boolean)"]/*'/>
    public static CBORObject FromObject(bool value) {
      return value ? CBORObject.True : CBORObject.False;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.Byte)"]/*'/>
    public static CBORObject FromObject(byte value) {
      return FromObject(((int)value) & 0xff);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.Single)"]/*'/>
    public static CBORObject FromObject(float value) {
      long doubleBits = CBORUtilities.SingleToDoublePrecision(
          CBORUtilities.SingleToInt32Bits(value));
      return new CBORObject(CBORObjectTypeDouble, doubleBits);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.Double)"]/*'/>
    public static CBORObject FromObject(double value) {
      long doubleBits = CBORUtilities.DoubleToInt64Bits(value);
      return new CBORObject(CBORObjectTypeDouble, doubleBits);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.Byte[])"]/*'/>
    public static CBORObject FromObject(byte[] bytes) {
      if (bytes == null) {
        return CBORObject.Null;
      }
      var newvalue = new byte[bytes.Length];
      Array.Copy(bytes, 0, newvalue, 0, bytes.Length);
      return new CBORObject(CBORObjectTypeByteString, bytes);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(PeterO.Cbor.CBORObject[])"]/*'/>
    public static CBORObject FromObject(CBORObject[] array) {
      if (array == null) {
        return CBORObject.Null;
      }
      CBORObject cbor = CBORObject.NewArray();
      foreach (CBORObject i in array) {
        cbor.Add(i);
      }
      return cbor;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.Int32[])"]/*'/>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.Int64[])"]/*'/>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.Object)"]/*'/>
    public static CBORObject FromObject(object obj) {
      return FromObject(obj, PODOptions.Default);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.Object,PeterO.Cbor.PODOptions)"]/*'/>
    public static CBORObject FromObject(
      object obj,
      PODOptions options) {
      return FromObject(obj, options, null, 0);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.Object,PeterO.Cbor.CBORTypeMapper)"]/*'/>
    public static CBORObject FromObject(
      object obj,
      CBORTypeMapper mapper) {
      if (mapper == null) {
        throw new ArgumentNullException(nameof(mapper));
      }
      return FromObject(obj, PODOptions.Default, mapper, 0);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.Object,PeterO.Cbor.CBORTypeMapper,PeterO.Cbor.PODOptions)"]/*'/>
    public static CBORObject FromObject(
      object obj,
      CBORTypeMapper mapper,
      PODOptions options) {
      if (mapper == null) {
        throw new ArgumentNullException(nameof(mapper));
      }
      return FromObject(obj, options, mapper, 0);
    }

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
        return CBORObject.Null;
      }
      if (obj is CBORObject) {
        return FromObject((CBORObject)obj);
      }
      CBORObject objret;
      if (mapper != null) {
        objret = mapper.ConvertWithConverter(obj);
        if (objret != null) {
          return objret;
        }
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
      var eif = obj as EInteger;
      if (eif != null) {
        return FromObject(eif);
      }
      var edf = obj as EDecimal;
      if (edf != null) {
        return FromObject(edf);
      }
      var eff = obj as EFloat;
      if (eff != null) {
        return FromObject(eff);
      }
      var erf = obj as ERational;
      if (erf != null) {
        return FromObject(erf);
      }
      if (obj is short) {
        return FromObject((short)obj);
      }
      if (obj is char) {
        return FromObject((int)(char)obj);
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
      if (obj is double) {
        return FromObject((double)obj);
      }
      byte[] bytearr = obj as byte[];
      if (bytearr != null) {
        return FromObject(bytearr);
      }
      if (obj is System.Collections.IDictionary) {
        // IDictionary appears first because IDictionary includes IEnumerable
        objret = CBORObject.NewMap();
        System.Collections.IDictionary objdic =
          (System.Collections.IDictionary)obj;
        foreach (object keyPair in (System.Collections.IDictionary)objdic) {
          System.Collections.DictionaryEntry
            kvp = (System.Collections.DictionaryEntry)keyPair;
          CBORObject objKey = CBORObject.FromObject(
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
          objret.Add(
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObjectAndTag(System.Object,PeterO.Numbers.EInteger)"]/*'/>
    public static CBORObject FromObjectAndTag(
      object valueOb,
      EInteger bigintTag) {
      if (bigintTag == null) {
        throw new ArgumentNullException(nameof(bigintTag));
      }
      if (bigintTag.Sign < 0) {
        throw new ArgumentException("tagEInt's sign (" + bigintTag.Sign +
                    ") is less than 0");
      }
      if (bigintTag.CompareTo(UInt64MaxValue) > 0) {
        throw new ArgumentException(
          "tag more than 18446744073709551615 (" + bigintTag + ")");
      }
      CBORObject c = FromObject(valueOb);
      if (bigintTag.CanFitInInt32()) {
        // Low-numbered, commonly used tags
        return FromObjectAndTag(c, bigintTag.ToInt32Checked());
      } else {
        var tagLow = 0;
        var tagHigh = 0;
        byte[] bytes = bigintTag.ToBytes(true);
        for (var i = 0; i < Math.Min(4, bytes.Length); ++i) {
          int b = ((int)bytes[i]) & 0xff;
          tagLow = unchecked(tagLow | (((int)b) << (i * 8)));
        }
        for (int i = 4; i < Math.Min(8, bytes.Length); ++i) {
          int b = ((int)bytes[i]) & 0xff;
          tagHigh = unchecked(tagHigh | (((int)b) << (i * 8)));
        }
        var c2 = new CBORObject(c, tagLow, tagHigh);
        return c2;
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObjectAndTag(System.Object,System.Int32)"]/*'/>
    public static CBORObject FromObjectAndTag(
      object valueObValue,
      int smallTag) {
      if (smallTag < 0) {
        throw new ArgumentException("smallTag (" + smallTag +
                    ") is less than 0");
      }
      CBORObject c = FromObject(valueObValue);
      c = new CBORObject(c, smallTag, 0);
      return c;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromSimpleValue(System.Int32)"]/*'/>
    public static CBORObject FromSimpleValue(int simpleValue) {
      if (simpleValue < 0) {
        throw new ArgumentException("simpleValue (" + simpleValue +
                    ") is less than 0");
      }
      if (simpleValue > 255) {
        throw new ArgumentException("simpleValue (" + simpleValue +
                    ") is more than " + "255");
      }
      if (simpleValue >= 24 && simpleValue < 32) {
        throw new ArgumentException("Simple value is from 24 to 31: " +
                    simpleValue);
      }
      if (simpleValue < 32) {
        return FixedObjects[0xe0 + simpleValue];
      }
      return new CBORObject(
        CBORObjectTypeSimpleValue,
        simpleValue);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Multiply(PeterO.Cbor.CBORObject,PeterO.Cbor.CBORObject)"]/*'/>
    public static CBORObject Multiply(CBORObject first, CBORObject second) {
      if (first == null) {
        throw new ArgumentNullException(nameof(first));
      }
      if (second == null) {
        throw new ArgumentNullException(nameof(second));
      }
      CBORNumber a = CBORNumber.FromCBORObject(first);
      if (a == null) {
        throw new ArgumentException(nameof(first) + "does not represent a" +
           "\u0020number");
      }
      CBORNumber b = CBORNumber.FromCBORObject(second);
      if (b == null) {
        throw new ArgumentException(nameof(second) + "does not represent a" +
            "\u0020number");
      }
      return a.Multiply(b).ToCBORObject();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.NewArray"]/*'/>
    public static CBORObject NewArray() {
      return new CBORObject(CBORObjectTypeArray, new List<CBORObject>());
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.NewMap"]/*'/>
    public static CBORObject NewMap() {
      return new CBORObject(
        CBORObjectTypeMap,
        new Dictionary<CBORObject, CBORObject>());
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.ReadSequence(System.IO.Stream)"]/*'/>
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
      return (CBORObject[])cborList.ToArray();
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.ReadSequence(System.IO.Stream,PeterO.Cbor.CBOREncodeOptions)"]/*'/>
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
      return (CBORObject[])cborList.ToArray();
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Read(System.IO.Stream)"]/*'/>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Read(System.IO.Stream,PeterO.Cbor.CBOREncodeOptions)"]/*'/>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.ReadJSON(System.IO.Stream)"]/*'/>
    public static CBORObject ReadJSON(Stream stream) {
      return ReadJSON(stream, CBOREncodeOptions.Default);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.ReadJSON(System.IO.Stream,PeterO.Cbor.CBOREncodeOptions)"]/*'/>
    public static CBORObject ReadJSON(
      Stream stream,
      CBOREncodeOptions options) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      var reader = new CharacterInputWithCount(
        new CharacterReader(stream, 2, true));
      try {
        var nextchar = new int[1];
        CBORObject obj = CBORJson.ParseJSONValue(
          reader,
          !options.AllowDuplicateKeys,
          false,
          nextchar);
        if (nextchar[0] != -1) {
          reader.RaiseError("End of data stream not reached");
        }
        return obj;
      } catch (CBORException ex) {
        var ioex = ex.InnerException as IOException;
        if (ioex != null) {
          throw ioex;
        }
        throw;
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Remainder(PeterO.Cbor.CBORObject,PeterO.Cbor.CBORObject)"]/*'/>
    public static CBORObject Remainder(CBORObject first, CBORObject second) {
      if (first == null) {
        throw new ArgumentNullException(nameof(first));
      }
      if (second == null) {
        throw new ArgumentNullException(nameof(second));
      }
      CBORNumber a = CBORNumber.FromCBORObject(first);
      if (a == null) {
        throw new ArgumentException(nameof(first) + "does not represent a" +
          "\u0020number");
      }
      CBORNumber b = CBORNumber.FromCBORObject(second);
      if (b == null) {
        throw new ArgumentException(nameof(second) + "does not represent a" +
          "\u0020number");
      }
      return a.Remainder(b).ToCBORObject();
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Subtract(PeterO.Cbor.CBORObject,PeterO.Cbor.CBORObject)"]/*'/>
    public static CBORObject Subtract(CBORObject first, CBORObject second) {
      if (first == null) {
        throw new ArgumentNullException(nameof(first));
      }
      if (second == null) {
        throw new ArgumentNullException(nameof(second));
      }
      CBORNumber a = CBORNumber.FromCBORObject(first);
      if (a == null) {
        throw new ArgumentException(nameof(first) + "does not represent a" +
"\u0020number");
      }
      CBORNumber b = CBORNumber.FromCBORObject(second);
      if (b == null) {
        throw new ArgumentException(nameof(second) + "does not represent a" +
"\u0020number");
      }
      return a.Subtract(b).ToCBORObject();
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(System.String,System.IO.Stream)"]/*'/>
    public static void Write(string str, Stream stream) {
      Write(str, stream, CBOREncodeOptions.Default);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(System.String,System.IO.Stream,PeterO.Cbor.CBOREncodeOptions)"]/*'/>
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
          WritePositiveInt64(3, codePointLength, stream);
          DataUtilities.WriteUtf8(str, stream, true);
        } else {
          WriteStreamedString(str, stream);
        }
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(PeterO.Numbers.EFloat,System.IO.Stream)"]/*'/>
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
        Write(CBORObject.FromObject(bignum), stream);
        return;
      }
      EInteger exponent = bignum.Exponent;
      if (exponent.GetSignedBitLengthAsEInteger().CompareTo(64) > 0) {
        stream.WriteByte(0xd9); // tag 265
        stream.WriteByte(0x01);
        stream.WriteByte(0x09);
        stream.WriteByte(0x82); // array, length 2
      } else {
        stream.WriteByte(0xc5); // tag 5
        stream.WriteByte(0x82); // array, length 2
      }
      Write(bignum.Exponent, stream);
      Write(bignum.Mantissa, stream);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(PeterO.Numbers.ERational,System.IO.Stream)"]/*'/>
    public static void Write(ERational rational, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (rational == null) {
        stream.WriteByte(0xf6);
        return;
      }
      if (!rational.IsFinite || (rational.IsNegative && rational.IsZero)) {
        Write(CBORObject.FromObject(rational), stream);
        return;
      }
      stream.WriteByte(0xd8); // tag 30
      stream.WriteByte(0x1e);
      stream.WriteByte(0x82); // array, length 2
      Write(rational.Numerator, stream);
      Write(rational.Denominator, stream);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(PeterO.Numbers.EDecimal,System.IO.Stream)"]/*'/>
    public static void Write(EDecimal bignum, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (bignum == null) {
        stream.WriteByte(0xf6);
        return;
      }
      if (!bignum.IsFinite || (bignum.IsNegative && bignum.IsZero)) {
        Write(CBORObject.FromObject(bignum), stream);
        return;
      }
      EInteger exponent = bignum.Exponent;
      if (exponent.GetSignedBitLengthAsEInteger().CompareTo(64) > 0) {
        stream.WriteByte(0xd9); // tag 264
        stream.WriteByte(0x01);
        stream.WriteByte(0x08);
        stream.WriteByte(0x82); // array, length 2
      } else {
        stream.WriteByte(0xc4); // tag 4
        stream.WriteByte(0x82); // array, length 2
      }
      Write(bignum.Exponent, stream);
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
        var ret = new byte[bytes.Length - index];
        Array.Copy(bytes, index, ret, 0, ret.Length);
        return ret;
      }
      return bytes;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(PeterO.Numbers.EInteger,System.IO.Stream)"]/*'/>
    public static void Write(EInteger bigint, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if ((object)bigint == (object)null) {
        stream.WriteByte(0xf6);
        return;
      }
      var datatype = 0;
      if (bigint.Sign < 0) {
        datatype = 1;
        bigint = bigint.Add(EInteger.One);
        bigint = -(EInteger)bigint;
      }
      if (bigint.CanFitInInt64()) {
        // If the arbitrary-precision integer is representable as a long and in
        // major type 0 or 1, write that major type
        // instead of as a bignum
        var ui = bigint.ToInt64Checked();
        WritePositiveInt64(datatype, ui, stream);
      } else {
        // Get a byte array of the arbitrary-precision integer's value,
        // since shifting and doing AND operations is
        // slow with large BigIntegers
        byte[] bytes = bigint.ToBytes(true);
        int byteCount = bytes.Length;
        while (byteCount > 0 && bytes[byteCount - 1] == 0) {
          // Ignore trailing zero bytes
          --byteCount;
        }
        if (byteCount != 0) {
          int half = byteCount >> 1;
          int right = byteCount - 1;
          for (var i = 0; i < half; ++i, --right) {
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
          default: stream.WriteByte((datatype == 0) ?
(byte)0xc2 : (byte)0xc3);
            WritePositiveInt(2, byteCount, stream);
            stream.Write(bytes, 0, byteCount);
            break;
        }
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(System.Int64,System.IO.Stream)"]/*'/>
    public static void Write(long value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (value >= 0) {
        WritePositiveInt64(0, value, stream);
      } else {
        ++value;
        value = -value; // Will never overflow
        WritePositiveInt64(1, value, stream);
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(System.Int32,System.IO.Stream)"]/*'/>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(System.Int16,System.IO.Stream)"]/*'/>
    public static void Write(short value, Stream stream) {
      Write((long)value, stream);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(System.Boolean,System.IO.Stream)"]/*'/>
    public static void Write(bool value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      stream.WriteByte(value ? (byte)0xf5 : (byte)0xf4);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(System.Byte,System.IO.Stream)"]/*'/>
    public static void Write(byte value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if ((((int)value) & 0xff) < 24) {
        stream.WriteByte(value);
      } else {
        stream.WriteByte((byte)24);
        stream.WriteByte(value);
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(System.Single,System.IO.Stream)"]/*'/>
    public static void Write(float value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      byte[] data = GetDoubleBytes(value, 0);
      stream.Write(data, 0, data.Length);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(System.Double,System.IO.Stream)"]/*'/>
    public static void Write(double value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      byte[] data = GetDoubleBytes(
        CBORUtilities.DoubleToInt64Bits(value),
        0);
      stream.Write(data, 0, data.Length);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(PeterO.Cbor.CBORObject,System.IO.Stream)"]/*'/>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(System.Object,System.IO.Stream)"]/*'/>
    public static void Write(object objValue, Stream stream) {
      Write(objValue, stream, CBOREncodeOptions.Default);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(System.Object,System.IO.Stream,PeterO.Cbor.CBOREncodeOptions)"]/*'/>
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
      byte[] data = objValue as byte[];
      if (data != null) {
        WritePositiveInt(3, data.Length, output);
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.WriteJSON(System.Object,System.IO.Stream)"]/*'/>
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Abs"]/*'/>
    public CBORObject Abs() {
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      if (cn == null) {
        throw new InvalidOperationException("This object is not a number.");
      }
      object oldItem = cn.GetValue();
      object newItem = cn.GetNumberInterface().Abs(oldItem);
      if (oldItem == newItem) {
        return this;
      }
      if (newItem is EDecimal) {
        return CBORObject.FromObject((EDecimal)newItem);
      }
      if (newItem is EInteger) {
        return CBORObject.FromObject((EInteger)newItem);
      }
      if (newItem is EFloat) {
        return CBORObject.FromObject((EFloat)newItem);
      }
      var rat = newItem as ERational;
      return (rat != null) ? CBORObject.FromObject(rat) : ((oldItem ==
          newItem) ? this : CBORObject.FromObject(newItem));
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Add(System.Object,System.Object)"]/*'/>
    public CBORObject Add(object key, object valueOb) {
      if (this.ItemType == CBORObjectTypeMap) {
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
        if (map.ContainsKey(mapKey)) {
          throw new ArgumentException("Key already exists");
        }
        map.Add(mapKey, mapValue);
      } else {
        throw new InvalidOperationException("Not a map");
      }
      return this;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Add(PeterO.Cbor.CBORObject)"]/*'/>
    public CBORObject Add(CBORObject obj) {
      if (this.ItemType == CBORObjectTypeArray) {
        IList<CBORObject> list = this.AsList();
        list.Add(obj);
        return this;
      }
      throw new InvalidOperationException("Not an array");
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Add(System.Object)"]/*'/>
    public CBORObject Add(object obj) {
      if (this.ItemType == CBORObjectTypeArray) {
        IList<CBORObject> list = this.AsList();
        list.Add(CBORObject.FromObject(obj));
        return this;
      }
      throw new InvalidOperationException("Not an array");
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsEInteger"]/*'/>
    public EInteger AsEInteger() {
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      if (cn == null) {
        throw new InvalidOperationException("Not a number type");
      }
      return cn.GetNumberInterface().AsEInteger(cn.GetValue());
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsBoolean"]/*'/>
    public bool AsBoolean() {
      return !this.IsFalse && !this.IsNull && !this.IsUndefined;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsByte"]/*'/>
    public byte AsByte() {
      return (byte)this.AsInt32(0, 255);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsDouble"]/*'/>
    public double AsDouble() {
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      if (cn == null) {
        throw new InvalidOperationException("Not a number type");
      }
      return cn.GetNumberInterface().AsDouble(cn.GetValue());
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsEDecimal"]/*'/>
    public EDecimal AsEDecimal() {
      CBORNumber cn = this.AsNumber();
      return cn.GetNumberInterface().AsExtendedDecimal(cn.GetValue());
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsEFloat"]/*'/>
    public EFloat AsEFloat() {
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      if (cn == null) {
        throw new InvalidOperationException("Not a number type");
      }
      return cn.GetNumberInterface().AsExtendedFloat(cn.GetValue());
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsERational"]/*'/>
    public ERational AsERational() {
      ERational ret = this.GetERational();
      if (ret != null) {
        return ret;
      }
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      if (cn == null) {
        throw new InvalidOperationException("Not a number type");
      }
      return cn.GetNumberInterface().AsExtendedRational(cn.GetValue());
    }

    private ERational GetERational() {
      return (this.HasMostInnerTag(30) && this.Count == 2) ?
         ERational.Create(this[0].AsEInteger(), this[1].AsEInteger()) :
         null;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsInt16"]/*'/>
    public short AsInt16() {
      return (short)this.AsInt32(Int16.MinValue, Int16.MaxValue);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsInt32Value"]/*'/>
    public int AsInt32Value() {
      switch (this.ItemType) {
        case CBORObjectTypeInteger: {
            var longValue = (long)this.ThisItem;
            if (longValue < Int32.MinValue || longValue > Int32.MaxValue) {
              throw new OverflowException();
            }
            return checked((int)longValue);
          }
        case CBORObjectTypeEInteger: {
            var ei = (EInteger)this.ThisItem;
            return ei.ToInt32Checked();
          }
        default: throw new InvalidOperationException("Not an integer type");
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsInt64Value"]/*'/>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.CanValueFitInInt64"]/*'/>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.CanValueFitInInt32"]/*'/>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsEIntegerValue"]/*'/>
    public EInteger AsEIntegerValue() {
      switch (this.ItemType) {
        case CBORObjectTypeInteger:
          return EInteger.FromInt64((long)this.ThisItem);
        case CBORObjectTypeEInteger:
          return (EInteger)this.ThisItem;
        default: throw new InvalidOperationException("Not an integer type");
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsDoubleBits"]/*'/>
    public long AsDoubleBits() {
      switch (this.Type) {
        case CBORType.FloatingPoint:
          return (long)this.ThisItem;
        default: throw new InvalidOperationException("Not a floating-point" +
"\u0020type");
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsDoubleValue"]/*'/>
    public double AsDoubleValue() {
      switch (this.Type) {
        case CBORType.FloatingPoint:
          return CBORUtilities.Int64BitsToDouble((long)this.ThisItem);
        default: throw new InvalidOperationException("Not a floating-point" +
"\u0020type");
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsNumber"]/*'/>
    public CBORNumber AsNumber() {
      CBORNumber num = CBORNumber.FromCBORObject(this);
      if (num == null) {
        throw new InvalidOperationException("Not a number type");
      }
      return num;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsInt32"]/*'/>
    public int AsInt32() {
      return this.AsInt32(Int32.MinValue, Int32.MaxValue);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsInt64"]/*'/>
    public long AsInt64() {
      CBORNumber cn = this.AsNumber();
      return cn.GetNumberInterface().AsInt64(cn.GetValue());
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsSingle"]/*'/>
    public float AsSingle() {
      CBORNumber cn = this.AsNumber();
      return cn.GetNumberInterface().AsSingle(cn.GetValue());
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsString"]/*'/>
    public string AsString() {
      int type = this.ItemType;
      switch (type) {
        case CBORObjectTypeTextString: {
            return (string)this.ThisItem;
          }
        default: throw new InvalidOperationException("Not a string type");
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.CanFitInDouble"]/*'/>
    public bool CanFitInDouble() {
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      return (cn != null) &&
cn.GetNumberInterface().CanFitInDouble(cn.GetValue());
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.CanFitInInt32"]/*'/>
    public bool CanFitInInt32() {
      if (!this.CanFitInInt64()) {
        return false;
      }
      long v = this.AsInt64();
      return v >= Int32.MinValue && v <= Int32.MaxValue;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.CanFitInInt64"]/*'/>
    public bool CanFitInInt64() {
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      return (cn != null) &&
cn.GetNumberInterface().CanFitInInt64(cn.GetValue());
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.CanFitInSingle"]/*'/>
    public bool CanFitInSingle() {
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      return (cn != null) &&
cn.GetNumberInterface().CanFitInSingle(cn.GetValue());
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.CanTruncatedIntFitInInt32"]/*'/>
    public bool CanTruncatedIntFitInInt32() {
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      return (cn != null) &&
cn.GetNumberInterface().CanTruncatedIntFitInInt32(cn.GetValue());
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.CanTruncatedIntFitInInt64"]/*'/>
    public bool CanTruncatedIntFitInInt64() {
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      return cn != null &&
cn.GetNumberInterface().CanTruncatedIntFitInInt64(cn.GetValue());
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.CompareTo(PeterO.Cbor.CBORObject)"]/*'/>
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
                    "doesn't satisfy a>= 0" +
"\u0020b<0");
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
          case CBORObjectTypeByteString: {
              cmp = CBORUtilities.ByteArrayCompareLengthFirst((byte[])objA,
  (byte[])objB);
              break;
            }
          case CBORObjectTypeTextString: {
              cmp = CBORUtilities.FastPathStringCompare(
                  (string)objA,
                  (string)objB);
              if (cmp < -1) {
                cmp = CBORUtilities.ByteArrayCompare(
                    this.EncodeToBytes(),
                    other.EncodeToBytes());
              }
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
          default: throw new InvalidOperationException("Unexpected data type");
        }
      } else if ((typeB == CBORObjectTypeInteger && typeA ==
CBORObjectTypeEInteger) ||
                (typeA == CBORObjectTypeInteger && typeB ==
CBORObjectTypeEInteger)) {
        cmp = CBORUtilities.ByteArrayCompare(
                    this.EncodeToBytes(),
                    other.EncodeToBytes());
      } else {
        /* NOTE: itemtypeValue numbers are ordered such that they
        // correspond to the lexicographical order of their CBOR encodings
        // (with the exception of Integer and BigInteger together, which
        // are handled above) */
        cmp = (typeA < typeB) ? -1 : 1;
      }
      // DebugUtility.Log("" + this + " " + other + " -> " + (cmp));
      return cmp;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.CompareToIgnoreTags(PeterO.Cbor.CBORObject)"]/*'/>
    public int CompareToIgnoreTags(CBORObject other) {
      return (other == null) ? 1 : ((this == other) ? 0 :
                    this.Untag().CompareTo(other.Untag()));
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.ContainsKey(System.Object)"]/*'/>
    public bool ContainsKey(object objKey) {
      return (this.ItemType == CBORObjectTypeMap) ?
        this.ContainsKey(CBORObject.FromObject(objKey)) : false;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.ContainsKey(PeterO.Cbor.CBORObject)"]/*'/>
    public bool ContainsKey(CBORObject key) {
      key = key ?? CBORObject.Null;
      if (this.ItemType == CBORObjectTypeMap) {
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        return map.ContainsKey(key);
      }
      return false;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.ContainsKey(System.String)"]/*'/>
    public bool ContainsKey(string key) {
      if (this.ItemType == CBORObjectTypeMap) {
        CBORObject ckey = key == null ? CBORObject.Null :
                  CBORObject.FromObject(key);
        IDictionary<CBORObject, CBORObject> map = this.AsMap();
        return map.ContainsKey(ckey);
      }
      return false;
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

    private static byte[] GetDoubleBytes(float value, int tagbyte) {
      int bits = CBORUtilities.SingleToHalfPrecisionIfSameValue(value);
      if (bits != -1) {
        return tagbyte != 0 ? new[] {
          (byte)tagbyte, (byte)0xf9,
          (byte)((bits >> 8) & 0xff), (byte)(bits & 0xff),
        } : new[] {
   (byte)0xf9, (byte)((bits >> 8) & 0xff),
   (byte)(bits & 0xff),
 };
      }
      bits = CBORUtilities.SingleToInt32Bits(value);
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.EncodeToBytes"]/*'/>
    public byte[] EncodeToBytes() {
      return this.EncodeToBytes(CBOREncodeOptions.Default);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.EncodeToBytes(PeterO.Cbor.CBOREncodeOptions)"]/*'/>
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
          tagbyte = (byte)(0xc0 + (int)this.tagLow);
        }
      }
      if (!hasComplexTag) {
        switch (this.ItemType) {
          case CBORObjectTypeTextString: {
              byte[] ret = GetOptimizedBytesIfShortAscii(
                this.AsString(), tagged ? (((int)tagbyte) & 0xff) : -1);
              if (ret != null) {
                return ret;
              }
              break;
            }
          case CBORObjectTypeSimpleValue: {
              if (tagged) {
                if (this.IsFalse) {
                  return new[] { tagbyte, (byte)0xf4 };
                }
                if (this.IsTrue) {
                  return new[] { tagbyte, (byte)0xf5 };
                }
                if (this.IsNull) {
                  return new[] { tagbyte, (byte)0xf6 };
                }
                if (this.IsUndefined) {
                  return new[] { tagbyte, (byte)0xf7 };
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
              byte[] intBytes = null;
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
              return GetDoubleBytes(
                  this.AsDoubleBits(),
                  ((int)tagbyte) & 0xff);
            }
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Equals(System.Object)"]/*'/>
    public override bool Equals(object obj) {
      return this.Equals(obj as CBORObject);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Equals(PeterO.Cbor.CBORObject)"]/*'/>
    public bool Equals(CBORObject other) {
      var otherValue = other as CBORObject;
      if (otherValue == null) {
        return false;
      }
      if (this == otherValue) {
        return true;
      }
      if (this.itemtypeValue != otherValue.itemtypeValue) {
        return false;
      }
      switch (this.itemtypeValue) {
        case CBORObjectTypeByteString:
          return CBORUtilities.ByteArrayEquals(
            (byte[])this.itemValue,
            otherValue.itemValue as byte[]);
        case CBORObjectTypeMap: {
            IDictionary<CBORObject, CBORObject> cbordict =
              otherValue.itemValue as IDictionary<CBORObject, CBORObject>;
            return CBORMapEquals(this.AsMap(), cbordict);
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.GetByteString"]/*'/>
    public byte[] GetByteString() {
      if (this.ItemType == CBORObjectTypeByteString) {
        return (byte[])this.ThisItem;
      }
      throw new InvalidOperationException("Not a byte string");
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.GetHashCode"]/*'/>
    public override int GetHashCode() {
      var hashCode = 651869431;
      unchecked {
        if (this.itemValue != null) {
          var itemHashCode = 0;
          long longValue = 0L;
          switch (this.itemtypeValue) {
            case CBORObjectTypeByteString:
              itemHashCode =
                CBORUtilities.ByteArrayHashCode((byte[])this.ThisItem);
              break;
            case CBORObjectTypeMap:
              itemHashCode = CBORMapHashCode(this.AsMap());
              break;
            case CBORObjectTypeArray:
              itemHashCode = CBORArrayHashCode(this.AsList());
              break;
            case CBORObjectTypeTextString:
              itemHashCode = StringHashCode((string)this.itemValue);
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.GetAllTags"]/*'/>
    public EInteger[] GetAllTags() {
      if (!this.IsTagged) {
        return ValueEmptyTags;
      }
      CBORObject curitem = this;
      if (curitem.IsTagged) {
        var list = new List<EInteger>();
        while (curitem.IsTagged) {
          list.Add(
            LowHighToEInteger(
              curitem.tagLow,
              curitem.tagHigh));
          curitem = (CBORObject)curitem.itemValue;
        }
        return (EInteger[])list.ToArray();
      }
      return new[] { LowHighToEInteger(this.tagLow, this.tagHigh) };
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.HasOneTag"]/*'/>
    public bool HasOneTag() {
      return this.IsTagged && !((CBORObject)this.itemValue).IsTagged;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.HasOneTag(System.Int32)"]/*'/>
    public bool HasOneTag(int tagValue) {
      return this.HasOneTag() && this.HasMostOuterTag(tagValue);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.HasOneTag(PeterO.Numbers.EInteger)"]/*'/>
    public bool HasOneTag(EInteger bigTagValue) {
      return this.HasOneTag() && this.HasMostOuterTag(bigTagValue);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBORObject.TagCount"]/*'/>
    public int TagCount {
      get {
        var count = 0;
        CBORObject curitem = this;
        while (curitem.IsTagged) {
          count = checked(count + 1);
          curitem = (CBORObject)curitem.itemValue;
        }
        return count;
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.HasMostInnerTag(System.Int32)"]/*'/>
    public bool HasMostInnerTag(int tagValue) {
      if (tagValue < 0) {
        throw new ArgumentException("tagValue (" + tagValue +
                    ") is less than 0");
      }
      return this.IsTagged && this.HasMostInnerTag(
        EInteger.FromInt32(tagValue));
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.HasMostInnerTag(PeterO.Numbers.EInteger)"]/*'/>
    public bool HasMostInnerTag(EInteger bigTagValue) {
      if (bigTagValue == null) {
        throw new ArgumentNullException(nameof(bigTagValue));
      }
      if (bigTagValue.Sign < 0) {
        throw new ArgumentException("bigTagValue (" + bigTagValue +
                    ") is less than 0");
      }
      return (!this.IsTagged) ? false : this.MostInnerTag.Equals(bigTagValue);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.HasMostOuterTag(System.Int32)"]/*'/>
    public bool HasMostOuterTag(int tagValue) {
      if (tagValue < 0) {
        throw new ArgumentException("tagValue (" + tagValue +
                    ") is less than 0");
      }
      return this.IsTagged && this.tagHigh == 0 && this.tagLow == tagValue;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.HasMostOuterTag(PeterO.Numbers.EInteger)"]/*'/>
    public bool HasMostOuterTag(EInteger bigTagValue) {
      if (bigTagValue == null) {
        throw new ArgumentNullException(nameof(bigTagValue));
      }
      if (bigTagValue.Sign < 0) {
        throw new ArgumentException("bigTagValue (" + bigTagValue +
                    ") is less than 0");
      }
      return (!this.IsTagged) ? false : this.MostOuterTag.Equals(bigTagValue);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.HasTag(System.Int32)"]/*'/>
    public bool HasTag(int tagValue) {
      if (tagValue < 0) {
        throw new ArgumentException("tagValue (" + tagValue +
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.HasTag(PeterO.Numbers.EInteger)"]/*'/>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Insert(System.Int32,System.Object)"]/*'/>
    public CBORObject Insert(int index, object valueOb) {
      if (this.ItemType == CBORObjectTypeArray) {
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
        list.Insert(index, mapValue);
      } else {
        throw new InvalidOperationException("Not an array");
      }
      return this;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.IsInfinity"]/*'/>
    public bool IsInfinity() {
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      return cn != null && cn.GetNumberInterface().IsInfinity(cn.GetValue());
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.IsNaN"]/*'/>
    public bool IsNaN() {
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      return cn != null && cn.GetNumberInterface().IsNaN(cn.GetValue());
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.IsNegativeInfinity"]/*'/>
    public bool IsNegativeInfinity() {
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      return cn != null &&
cn.GetNumberInterface().IsNegativeInfinity(cn.GetValue());
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.IsPositiveInfinity"]/*'/>
    public bool IsPositiveInfinity() {
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      return cn != null &&
cn.GetNumberInterface().IsPositiveInfinity(cn.GetValue());
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Negate"]/*'/>
    public CBORObject Negate() {
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      if (cn == null) {
        throw new InvalidOperationException("This object is not a number.");
      }
      object newItem = cn.GetNumberInterface().Negate(cn.GetValue());
      if (newItem is EDecimal) {
        return CBORObject.FromObject((EDecimal)newItem);
      }
      if (newItem is EInteger) {
        return CBORObject.FromObject((EInteger)newItem);
      }
      if (newItem is EFloat) {
        return CBORObject.FromObject((EFloat)newItem);
      }
      var rat = newItem as ERational;
      return (rat != null) ? CBORObject.FromObject(rat) :
        CBORObject.FromObject(newItem);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Clear"]/*'/>
    public void Clear() {
      if (this.ItemType == CBORObjectTypeArray) {
        IList<CBORObject> list = this.AsList();
        list.Clear();
      } else if (this.ItemType == CBORObjectTypeMap) {
        IDictionary<CBORObject, CBORObject> dict = this.AsMap();
        dict.Clear();
      } else {
        throw new InvalidOperationException("Not a map or array");
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Remove(System.Object)"]/*'/>
    public bool Remove(object obj) {
      return this.Remove(CBORObject.FromObject(obj));
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.RemoveAt(System.Int32)"]/*'/>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Remove(PeterO.Cbor.CBORObject)"]/*'/>
    public bool Remove(CBORObject obj) {
      if (obj == null) {
        throw new ArgumentNullException(nameof(obj));
      }
      if (this.ItemType == CBORObjectTypeMap) {
        IDictionary<CBORObject, CBORObject> dict = this.AsMap();
        bool hasKey = dict.ContainsKey(obj);
        if (hasKey) {
          dict.Remove(obj);
          return true;
        }
        return false;
      }
      if (this.ItemType == CBORObjectTypeArray) {
        IList<CBORObject> list = this.AsList();
        return list.Remove(obj);
      }
      throw new InvalidOperationException("Not a map or array");
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Set(System.Object,System.Object)"]/*'/>
    public CBORObject Set(object key, object valueOb) {
      if (this.ItemType == CBORObjectTypeMap) {
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
        if (map.ContainsKey(mapKey)) {
          map[mapKey] = mapValue;
        } else {
          map.Add(mapKey, mapValue);
        }
      } else if (this.ItemType == CBORObjectTypeArray) {
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.ToJSONString"]/*'/>
    public string ToJSONString() {
      return this.ToJSONString(JSONOptions.Default);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.ToJSONString(PeterO.Cbor.JSONOptions)"]/*'/>
    public string ToJSONString(JSONOptions options) {
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      CBORType type = this.Type;
      switch (type) {
        case CBORType.SimpleValue: {
            return this.IsTrue ? "true" : (this.IsFalse ? "false" : "null");
          }
        case CBORType.Integer: {
            return this.AsEIntegerValue().ToString();
          }
        case CBORType.FloatingPoint: {
            return CBORNumber.FromObject(this.AsDoubleValue()).ToJSONString();
          }
        default: {
            var sb = new StringBuilder();
            try {
              CBORJson.WriteJSONToInternal(this, new StringOutput(sb), options);
            } catch (IOException ex) {
              // This is truly exceptional
              throw new InvalidOperationException("Internal error", ex);
            }
            return sb.ToString();
          }
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.ToString"]/*'/>
    public override string ToString() {
      StringBuilder sb = null;
      string simvalue = null;
      CBORType type = this.Type;
      if (this.IsTagged) {
        if (sb == null) {
          if (type == CBORType.TextString) {
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
      switch (type) {
        case CBORType.SimpleValue: {
            if (this.IsTrue) {
              simvalue = "true";
              if (sb == null) {
                return simvalue;
              }
              sb.Append(simvalue);
            } else if (this.IsFalse) {
              simvalue = "false";
              if (sb == null) {
                return simvalue;
              }
              sb.Append(simvalue);
            } else if (this.IsNull) {
              simvalue = "null";
              if (sb == null) {
                return simvalue;
              }
              sb.Append(simvalue);
            } else if (this.IsUndefined) {
              simvalue = "undefined";
              if (sb == null) {
                return simvalue;
              }
              sb.Append(simvalue);
            } else {
              sb = sb ?? new StringBuilder();
              sb.Append("simple(");
              var thisItemInt = (int)this.ThisItem;
              sb.Append(
                CBORUtilities.LongToString(thisItemInt));
              sb.Append(")");
            }

            break;
          }
        case CBORType.FloatingPoint: {
            double f = this.AsDoubleValue();
            simvalue = Double.IsNegativeInfinity(f) ? "-Infinity" :
              (Double.IsPositiveInfinity(f) ? "Infinity" : (Double.IsNaN(f) ?
                    "NaN" : CBORUtilities.TrimDotZero(
                       CBORUtilities.DoubleToString(f))));
            if (sb == null) {
              return simvalue;
            }
            sb.Append(simvalue);
            break;
          }
        case CBORType.Integer: {
            if (this.CanValueFitInInt64()) {
              long v = this.AsInt64Value();
              simvalue = CBORUtilities.LongToString(v);
            } else {
              simvalue = this.AsEIntegerValue().ToString();
            }
            if (sb == null) {
              return simvalue;
            }
            sb.Append(simvalue);
            break;
          }
        case CBORType.ByteString: {
            sb = sb ?? new StringBuilder();
            sb.Append("h'");
            CBORUtilities.ToBase16(sb, (byte[])this.ThisItem);
            sb.Append("'");
            break;
          }
        case CBORType.TextString: {
            if (sb == null) {
              return "\"" + this.AsString() + "\"";
            }
            sb.Append('\"');
            sb.Append(this.AsString());
            sb.Append('\"');
            break;
          }
        case CBORType.Array: {
            sb = sb ?? new StringBuilder();
            var first = true;
            sb.Append("[");
            foreach (CBORObject i in this.AsList()) {
              if (!first) {
                sb.Append(", ");
              }
              sb.Append(i.ToString());
              first = false;
            }
            sb.Append("]");
            break;
          }
        case CBORType.Map: {
            sb = sb ?? new StringBuilder();
            var first = true;
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
            break;
          }
        default: {
            if (sb == null) {
              return this.ThisItem.ToString();
            }
            sb.Append(this.ThisItem.ToString());
            break;
          }
      }
      if (this.IsTagged) {
        this.AppendClosingTags(sb);
      }
      return sb.ToString();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Untag"]/*'/>
    public CBORObject Untag() {
      CBORObject curobject = this;
      while (curobject.itemtypeValue == CBORObjectTypeTagged) {
        curobject = (CBORObject)curobject.itemValue;
      }
      return curobject;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.UntagOne"]/*'/>
    public CBORObject UntagOne() {
      return (this.itemtypeValue == CBORObjectTypeTagged) ?
        ((CBORObject)this.itemValue) : this;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.WriteJSONTo(System.IO.Stream)"]/*'/>
    public void WriteJSONTo(Stream outputStream) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      CBORJson.WriteJSONToInternal(
  this,
  new StringOutput(outputStream),
  JSONOptions.Default);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.WriteJSONTo(System.IO.Stream,PeterO.Cbor.JSONOptions)"]/*'/>
    public void WriteJSONTo(Stream outputStream, JSONOptions options) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      if (options == null) {
        throw new ArgumentNullException(nameof(options));
      }
      CBORJson.WriteJSONToInternal(
  this,
  new StringOutput(outputStream),
  options);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.WriteFloatingPointBits(System.IO.Stream,System.Int64,System.Int32)"]/*'/>
    public static int WriteFloatingPointBits(
      Stream outputStream,
      long floatingBits,
      int byteCount) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      byte[] bytes;
      switch (byteCount) {
        case 2:
          bytes = new byte[] {
            (byte)0xf9,
            (byte)((floatingBits >> 8) & 0xffL),
            (byte)(floatingBits & 0xffL),
          };
          outputStream.Write(bytes, 0, 3);
          return 3;
        case 4:
          bytes = new byte[] {
            (byte)0xfa,
            (byte)((floatingBits >> 24) & 0xffL),
            (byte)((floatingBits >> 16) & 0xffL),
            (byte)((floatingBits >> 8) & 0xffL),
            (byte)(floatingBits & 0xffL),
          };
          outputStream.Write(bytes, 0, 5);
          return 5;
        case 8:
          bytes = new byte[] {
            (byte)0xfb,
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
        default:
          throw new ArgumentOutOfRangeException(nameof(byteCount));
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.WriteFloatingPointValue(System.IO.Stream,System.Double,System.Int32)"]/*'/>
    public static int WriteFloatingPointValue(
      Stream outputStream,
      double doubleVal,
      int byteCount) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      long bits = 0;
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
          // DebugUtility.Log("dbl " + doubleVal + " -> " + (bits));
          return WriteFloatingPointBits(outputStream, bits, 8);
        default: throw new ArgumentOutOfRangeException(nameof(byteCount));
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.WriteFloatingPointValue(System.IO.Stream,System.Single,System.Int32)"]/*'/>
    public static int WriteFloatingPointValue(
      Stream outputStream,
      float singleVal,
      int byteCount) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      var bits = 0;
      long longbits = 0L;
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
          longbits = ((long)bits) & 0xffffffffL;
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.WriteValue(System.IO.Stream,System.Int32,System.Int64)"]/*'/>
    public static int WriteValue(
      Stream outputStream,
      int majorType,
      long value) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      if (majorType < 0) {
        throw new ArgumentException("majorType (" + majorType +
          ") is less than 0");
      }
      if (majorType > 7) {
        throw new ArgumentException("majorType (" + majorType +
          ") is more than 7");
      }
      if (value < 0) {
        throw new ArgumentException("value (" + value +
          ") is less than 0");
      }
      if (majorType == 7) {
        if (value > 255) {
          throw new ArgumentException("value (" + value +
            ") is more than 255");
        }
        if (value <= 23) {
          outputStream.WriteByte((byte)(0xe0 + (int)value));
          return 1;
        } else if (value < 32) {
          throw new ArgumentException("value is from 24 to 31 and major type" +
"\u0020is 7");
        } else {
          outputStream.WriteByte((byte)0xf8);
          outputStream.WriteByte((byte)value);
          return 2;
        }
      } else {
        return WritePositiveInt64(majorType, value, outputStream);
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.WriteValue(System.IO.Stream,System.Int32,System.Int32)"]/*'/>
    public static int WriteValue(
      Stream outputStream,
      int majorType,
      int value) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      if (majorType < 0) {
        throw new ArgumentException("majorType (" + majorType +
          ") is less than 0");
      }
      if (majorType > 7) {
        throw new ArgumentException("majorType (" + majorType +
          ") is more than 7");
      }
      if (value < 0) {
        throw new ArgumentException("value (" + value +
          ") is less than 0");
      }
      if (majorType == 7) {
        if (value > 255) {
          throw new ArgumentException("value (" + value +
            ") is more than 255");
        }
        if (value <= 23) {
          outputStream.WriteByte((byte)(0xe0 + value));
          return 1;
        } else if (value < 32) {
          throw new ArgumentException("value is from 24 to 31 and major type" +
"\u0020is 7");
        } else {
          outputStream.WriteByte((byte)0xf8);
          outputStream.WriteByte((byte)value);
          return 2;
        }
      } else {
        return WritePositiveInt(majorType, value, outputStream);
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.WriteValue(System.IO.Stream,System.Int32,PeterO.Numbers.EInteger)"]/*'/>
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
        throw new ArgumentException("tagEInt's sign (" + bigintValue.Sign +
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
        throw new ArgumentException("majorType (" + majorType +
          ") is less than 0");
      }
      if (majorType > 7) {
        throw new ArgumentException("majorType (" + majorType +
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.WriteTo(System.IO.Stream)"]/*'/>
    public void WriteTo(Stream stream) {
      this.WriteTo(stream, CBOREncodeOptions.Default);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBORObject.WriteTo(System.IO.Stream,PeterO.Cbor.CBOREncodeOptions)"]/*'/>
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
        case CBORObjectTypeByteString: {
            var arr = (byte[])this.ThisItem;
            WritePositiveInt(
              (this.ItemType == CBORObjectTypeByteString) ? 2 : 3,
              arr.Length,
              stream);
            stream.Write(arr, 0, arr.Length);
            break;
          }
        case CBORObjectTypeTextString: {
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
            var value = (int)this.ThisItem;
            if (value < 24) {
              stream.WriteByte((byte)(0xe0 + value));
            } else {
#if DEBUG
              if (value < 32) {
                throw new ArgumentException("value (" + value +
                    ") is less than " + "32");
              }
#endif

              stream.WriteByte(0xf8);
              stream.WriteByte((byte)value);
            }

            break;
          }
        case CBORObjectTypeDouble: {
            byte[] data = GetDoubleBytes(this.AsDoubleBits(), 0);
            stream.Write(data, 0, data.Length);
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

    internal static CBORObject FromRaw(string str) {
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
      if (firstbyte >= 0x61 && firstbyte < 0x78) {
        // text string length 1 to 23
        string s = GetOptimizedStringIfShortAscii(data, 0);
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
            uadditional = (data[1] & 0xffL) << 8;
            uadditional |= (long)(data[2] & 0xffL);
            break;
          case 26:
            uadditional = (data[1] & 0xffL) << 24;
            uadditional |= (data[2] & 0xffL) << 16;
            uadditional |= (data[3] & 0xffL) << 8;
            uadditional |= (long)(data[4] & 0xffL);
            break;
          case 27:
            uadditional = (data[1] & 0xffL) << 56;
            uadditional |= (data[2] & 0xffL) << 48;
            uadditional |= (data[3] & 0xffL) << 40;
            uadditional |= (data[4] & 0xffL) << 32;
            uadditional |= (data[5] & 0xffL) << 24;
            uadditional |= (data[6] & 0xffL) << 16;
            uadditional |= (data[7] & 0xffL) << 8;
            uadditional |= (long)(data[8] & 0xffL);
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
              int low = unchecked((int)(uadditional & 0xffffffffL));
              int high = unchecked((int)((uadditional >> 32) & 0xffffffffL));
              return FromObject(LowHighToEInteger(low, high));
            }
          case 1:
            if ((uadditional >> 63) == 0) {
              // use only if additional's top bit isn't set
              // (additional is a signed long)
              return new CBORObject(CBORObjectTypeInteger, -1 - uadditional);
            } else {
              int low = unchecked((int)(uadditional & 0xffffffffL));
              int high = unchecked((int)((uadditional >> 32) & 0xffffffffL));
              EInteger bigintAdditional = LowHighToEInteger(low, high);
              EInteger minusOne = -EInteger.One;
              bigintAdditional = minusOne - (EInteger)bigintAdditional;
              return FromObject(bigintAdditional);
            }
          case 7:
            if (firstbyte >= 0xf9 && firstbyte <= 0xfb) {
              var dblbits = (long)uadditional;
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
              if ((int)uadditional < 32) {
                throw new CBORException("Invalid overlong simple value");
              }
              return new CBORObject(
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
        var ret = new StringBuilder(firstbyte - 0x60);
        DataUtilities.ReadUtf8FromBytes(data, 1, firstbyte - 0x60, ret, false);
        return new CBORObject(CBORObjectTypeTextString, ret.ToString());
      }
      if (firstbyte == 0x80) {
        // empty array
        return CBORObject.NewArray();
      }
      if (firstbyte == 0xa0) {
        // empty map
        return CBORObject.NewMap();
      }
      throw new CBORException("Unexpected data encountered");
    }

    internal static CBORObject GetFixedObject(int value) {
      return FixedObjects[value];
    }

    internal IList<CBORObject> AsList() {
      return (IList<CBORObject>)this.ThisItem;
    }

    internal IDictionary<CBORObject, CBORObject> AsMap() {
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
      for (var i = 0; i < listACount; ++i) {
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
      unchecked {
        ret = (ret * 31) + count;
        for (var i = 0; i < count; ++i) {
          ret = (ret * 31) + list[i].GetHashCode();
        }
      }
      return ret;
    }

    private static int StringHashCode(string str) {
      if (str == null) {
        return 0;
      }
      var ret = 19;
      int count = str.Length;
      unchecked {
        ret = (ret * 31) + count;
        for (var i = 0; i < count; ++i) {
          ret = (ret * 31) + (int)str[i];
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
      for (var i = 0; i < count; ++i) {
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

    private static void CheckCBORLength(
      long expectedLength,
      long actualLength) {
      if (actualLength < expectedLength) {
        throw new CBORException("Premature end of data");
      }
      if (actualLength > expectedLength) {
        throw new CBORException("Too many bytes");
      }
    }

    private static void CheckCBORLength(int expectedLength, int actualLength) {
      if (actualLength < expectedLength) {
        throw new CBORException("Premature end of data");
      }
      if (actualLength > expectedLength) {
        throw new CBORException("Too many bytes");
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
          bytes[offset] = (byte)0x78;
          bytes[offset + 1] = (byte)str.Length;
          offset += 2;
        }
        var issimple = true;
        for (var i = 0; i < str.Length; ++i) {
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
        var nextbyte = (int)(data[offset] & (int)0xff);
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
          var c = new char[length - offsetp1];
          for (int i = offsetp1; i < length; ++i) {
            c[i - offsetp1] = (char)(data[i] & (int)0xff);
          }
          return new String(c);
        }
      }
      return null;
    }

    private static byte[] GetPositiveInt64Bytes(int type, long value) {
      if (value < 0) {
        throw new ArgumentException("value (" + value + ") is less than " +
                    "0");
      }
      if (value < 24) {
        return new[] { (byte)((byte)value | (byte)(type << 5)) };
      }
      if (value <= 0xffL) {
        return new[] {
          (byte)(24 | (type << 5)), (byte)(value & 0xff),
        };
      }
      if (value <= 0xffffL) {
        return new[] {
          (byte)(25 | (type << 5)),
          (byte)((value >> 8) & 0xff), (byte)(value & 0xff),
        };
      }
      if (value <= 0xffffffffL) {
        return new[] {
          (byte)(26 | (type << 5)),
          (byte)((value >> 24) & 0xff), (byte)((value >> 16) & 0xff),
          (byte)((value >> 8) & 0xff), (byte)(value & 0xff),
        };
      }
      return new[] {
        (byte)(27 | (type << 5)), (byte)((value >> 56) & 0xff),
        (byte)((value >> 48) & 0xff), (byte)((value >> 40) & 0xff),
        (byte)((value >> 32) & 0xff), (byte)((value >> 24) & 0xff),
        (byte)((value >> 16) & 0xff), (byte)((value >> 8) & 0xff),
        (byte)(value & 0xff),
      };
    }

    private static byte[] GetPositiveIntBytes(int type, int value) {
      if (value < 0) {
        throw new ArgumentException("value (" + value + ") is less than " +
                    "0");
      }
      if (value < 24) {
        return new[] { (byte)((byte)value | (byte)(type << 5)) };
      }
      if (value <= 0xff) {
        return new[] {
          (byte)(24 | (type << 5)), (byte)(value & 0xff),
        };
      }
      if (value <= 0xffff) {
        return new[] {
          (byte)(25 | (type << 5)),
          (byte)((value >> 8) & 0xff), (byte)(value & 0xff),
        };
      }
      return new[] {
        (byte)(26 | (type << 5)), (byte)((value >> 24) & 0xff),
        (byte)((value >> 16) & 0xff), (byte)((value >> 8) & 0xff),
        (byte)(value & 0xff),
      };
    }

    // Initialize fixed values for certain
    // head bytes
    private static CBORObject[] InitializeFixedObjects() {
      var fixedObjects = new CBORObject[256];
      for (var i = 0; i < 0x18; ++i) {
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
          (int)(i - 0xe0));
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
      for (var i = 0; i < listACount; ++i) {
        int cmp = listA[i].CompareTo(listB[i]);
        if (cmp != 0) {
          return cmp;
        }
      }
      return 0;
    }

    private static EInteger LowHighToEInteger(int tagLow, int tagHigh) {
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
      var sortedASet = new List<CBORObject>(mapA.Keys);
      var sortedBSet = new List<CBORObject>(mapB.Keys);
      sortedASet.Sort();
      sortedBSet.Sort();
      listACount = sortedASet.Count;
      listBCount = sortedBSet.Count;
      // Compare the keys
      for (var i = 0; i < listACount; ++i) {
        CBORObject itemA = sortedASet[i];
        CBORObject itemB = sortedBSet[i];
        if (itemA == null) {
          return -1;
        }
        int cmp = itemA.CompareTo(itemB);
        // DebugUtility.Log("" + itemA + ", " + itemB + " -> cmp=" + (cmp));
        if (cmp != 0) {
          return cmp;
        }
        // Both maps have the same key, so compare
        // the value under that key
        cmp = mapA[itemA].CompareTo(mapB[itemB]);
        // DebugUtility.Log("{0}, {1} -> {2}, {3} ->
        // cmp={4}",itemA,itemB,mapA[itemA],mapB[itemB],cmp);
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
      for (var i = 0; i < c; ++i) {
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
      WritePositiveInt(4, list.Count, outputStream);
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
      WritePositiveInt(5, map.Count, outputStream);
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
      bytes = new byte[StreamedStringBufferLength];
      var byteIndex = 0;
      var streaming = false;
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
            // Write bytes retrieved so far - the next three bytes
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
              (str[index + 1] & 0xfc00) == 0xdc00) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c & 0x3ff) << 10) + (str[index + 1] & 0x3ff);
            ++index;
          } else if ((c & 0xf800) == 0xd800) {
            // unpaired surrogate, write U + FFFD instead
            c = 0xfffd;
          }
          if (c <= 0xffff) {
            if (byteIndex + 3 > StreamedStringBufferLength) {
              // Write bytes retrieved so far - the next three bytes
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
              // Write bytes retrieved so far - the next four bytes
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

    //-----------------------------------------------------------
    private void AppendClosingTags(StringBuilder sb) {
      CBORObject curobject = this;
      while (curobject.IsTagged) {
        sb.Append(')');
        curobject = (CBORObject)curobject.itemValue;
      }
    }

    private void AppendOpeningTags(StringBuilder sb) {
      CBORObject curobject = this;
      while (curobject.IsTagged) {
        int low = curobject.tagLow;
        int high = curobject.tagHigh;
        if (high == 0 && (low >> 16) == 0) {
          sb.Append(CBORUtilities.LongToString(low));
        } else {
          EInteger bi = LowHighToEInteger(low, high);
          sb.Append(bi.ToString());
        }
        sb.Append('(');
        curobject = (CBORObject)curobject.itemValue;
      }
    }

    private int AsInt32(int minValue, int maxValue) {
      CBORNumber cn = CBORNumber.FromCBORObject(this);
      if (cn == null) {
        throw new InvalidOperationException("not a number type");
      }
      return cn.GetNumberInterface().AsInt32(cn.GetValue(), minValue, maxValue);
    }

    private void WriteTags(Stream s) {
      CBORObject curobject = this;
      while (curobject.IsTagged) {
        int low = curobject.tagLow;
        int high = curobject.tagHigh;
        if (high == 0 && (low >> 16) == 0) {
          WritePositiveInt(6, low, s);
        } else if (high == 0) {
          long value = ((long)low) & 0xffffffffL;
          WritePositiveInt64(6, value, s);
        } else if ((high >> 16) == 0) {
          long value = ((long)low) & 0xffffffffL;
          long highValue = ((long)high) & 0xffffffffL;
          value |= highValue << 32;
          WritePositiveInt64(6, value, s);
        } else {
          byte[] arrayToWrite = {
            (byte)0xdb,
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
