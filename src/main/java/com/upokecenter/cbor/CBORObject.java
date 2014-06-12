package com.upokecenter.cbor;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.util.*;

import java.io.*;

import com.upokecenter.util.*;

    /**
     * Represents an object in Concise Binary Object Representation (CBOR)
     * and contains methods for reading and writing CBOR data. CBOR is defined
     * in RFC 7049. <p>There are many ways to get a CBOR object, including
     * from bytes, objects, streams and JSON, as described below.</p> <p>
     * <b>To and from byte arrays:</b> The CBORObject.DecodeToBytes method
     * converts a byte array in CBOR format to a CBOR object. The EncodeToBytes
     * method converts a CBOR object to its corresponding byte array in CBOR
     * format. </p> <p> <b>To and from data streams:</b> The CBORObject.Write
     * methods write many kinds of objects to a data stream, including numbers,
     * CBOR objects, strings, and arrays of numbers and strings. The CBORObject.Read
     * method reads a CBOR object from a data stream. </p> <p> <b>To and from
     * other objects:</b> The CBORObject.FromObject methods converts
     * many kinds of objects to a CBOR object, including numbers, strings,
     * and arrays and maps of numbers and strings. Methods like AsDouble,
     * AsByte, and AsString convert a CBOR object to different types of object.
     * </p> <p> <b>To and from JSON:</b> This class also doubles as a reader
     * and writer of JavaScript Object Notation (JSON). The CBORObject.FromJSONString
     * method converts JSON to a CBOR object, and the ToJSONString method
     * converts a CBOR object to a JSON string. </p> <p> Thread Safety: CBOR
     * objects that are numbers, "simple values", and text strings are immutable
     * (their values can't be changed), so they are inherently safe for use
     * by multiple threads. CBOR objects that are arrays, maps, and byte
     * strings are mutable, but this class doesn't attempt to synchronize
     * reads and writes to those objects by multiple threads, so those objects
     * are not thread safe without such synchronization. </p> <p> One kind
     * of CBOR object is called a map, or a list of key-value pairs. Keys can
     * be any kind of CBOR object, including numbers, strings, arrays, and
     * maps. However, since byte strings, arrays, and maps are mutable,
     * it is not advisable to use these three kinds of object as keys; they
     * are much better used as map values instead, keeping in mind that they
     * are not thread safe without synchronizing reads and writes to them.
     * </p>
     */
  public final class CBORObject implements Comparable<CBORObject> {
    int getItemType(){
        CBORObject curobject = this;
        while (curobject.itemtypeValue == CBORObjectTypeTagged) {
          curobject = (CBORObject)curobject.itemValue;
        }
        return curobject.itemtypeValue;
      }

    Object getThisItem(){
        CBORObject curobject = this;
        while (curobject.itemtypeValue == CBORObjectTypeTagged) {
          curobject = (CBORObject)curobject.itemValue;
        }
        return curobject.itemValue;
      }

    private int itemtypeValue;
    private Object itemValue;
    private int tagLow;
    private int tagHigh;
    static final int CBORObjectTypeInteger = 0;  // -(2^63).. (2^63-1)
    private static final int CBORObjectTypeBigInteger = 1;  // all other integers
    static final int CBORObjectTypeByteString = 2;
    static final int CBORObjectTypeTextString = 3;
    private static final int CBORObjectTypeArray = 4;
    private static final int CBORObjectTypeMap = 5;
    private static final int CBORObjectTypeSimpleValue = 6;
    static final int CBORObjectTypeSingle = 7;
    static final int CBORObjectTypeDouble = 8;
    static final int CBORObjectTypeExtendedDecimal = 9;
    private static final int CBORObjectTypeTagged = 10;
    static final int CBORObjectTypeExtendedFloat = 11;
    static final int CBORObjectTypeExtendedRational = 12;
    static final BigInteger Int64MaxValue = BigInteger.valueOf(Long.MAX_VALUE);
    static final BigInteger Int64MinValue = BigInteger.valueOf(Long.MIN_VALUE);

    private static final BigInteger UInt64MaxValue = (BigInteger.ONE.shiftLeft(64)).subtract(BigInteger.ONE);

    private static final class ConverterInfo {
      private Object toObject;

    /**
     * Gets the converter's ToCBORObject method.
     * @return The converter's ToCBORObject method.
     */
      public Object getToObject() {
          return this.toObject;
        }
public void setToObject(Object value) {
          this.toObject = value;
        }

      private Object converter;

    /**
     * Gets the ICBORConverter object.
     * @return The ICBORConverter object.
     */
      public Object getConverter() {
          return this.converter;
        }
public void setConverter(Object value) {
          this.converter = value;
        }
    }

    private static Map<Object, ConverterInfo> converters = new HashMap<Object, ConverterInfo>();
    private static Map<BigInteger, ICBORTag> tagHandlers = new HashMap<BigInteger, ICBORTag>();

    private static int[] valueNumberTypeOrder = new int[] { 0, 0, 2, 3, 4, 5, 1, 0, 0, 0, 0, 0, 0 };

    private static final ICBORNumber[] NumberInterfaces = new ICBORNumber[] {
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

    /**
     * Represents the value false.
     */

    public static final CBORObject False = new CBORObject(CBORObjectTypeSimpleValue, 20);

    /**
     * Represents the value true.
     */

    public static final CBORObject True = new CBORObject(CBORObjectTypeSimpleValue, 21);

    /**
     * Represents the value null.
     */

    public static final CBORObject Null = new CBORObject(CBORObjectTypeSimpleValue, 22);

    /**
     * Represents the value undefined.
     */

    public static final CBORObject Undefined = new CBORObject(CBORObjectTypeSimpleValue, 23);

    CBORObject(CBORObject obj, int tagLow, int tagHigh){
 this(CBORObjectTypeTagged,obj);
      this.tagLow = tagLow;
      this.tagHigh = tagHigh;
    }

    /**
     * Registers an object that converts objects of a given type to CBOR objects
     * (called a CBOR converter).
     * @param type A Type object specifying the type that the converter converts
     * to CBOR objects.
     * @param converter An ICBORConverter object.
     * @param <T> Must be the same as the "type" parameter.
     * @throws java.lang.NullPointerException The parameter {@code type}
     * or {@code converter} is null.
     */
    public static <T> void AddConverter(Class<?> type, ICBORConverter<T> converter) {
      if (type == null) {
        throw new NullPointerException("type");
      }
      if (converter == null) {
        throw new NullPointerException("converter");
      }
      ConverterInfo ci = new CBORObject.ConverterInfo();
      ci.setConverter(converter);
      ci.setToObject(PropertyMap.FindOneArgumentMethod(converter, "ToCBORObject", type));
      if (ci.getToObject() == null) {
        throw new IllegalArgumentException("Converter doesn't contain a proper ToCBORObject method");
      }
      synchronized(converters) {
        converters.put(type,ci);
      }
    }

    private static boolean TagHandlersEmpty() {
      synchronized(tagHandlers) {
        return tagHandlers.size() == 0;
      }
    }

    static ICBORNumber GetNumberInterface(int type) {
      return NumberInterfaces[type];
    }

    /**
     * Not documented yet.
     * @param bigintTag A BigInteger object.
     * @param handler An ICBORTag object.
     * @throws java.lang.NullPointerException The parameter {@code bigintTag}
     * or {@code handler} is null.
     */
    public static void AddTagHandler(BigInteger bigintTag, ICBORTag handler) {
      if (bigintTag == null) {
        throw new NullPointerException("bigintTag");
      }
      if (handler == null) {
        throw new NullPointerException("handler");
      }
      if (bigintTag.signum() < 0) {
        throw new IllegalArgumentException("bigintTag.signum() (" + Long.toString((long)bigintTag.signum()) + ") is less than " + "0");
      }
      if (bigintTag.bitLength() > 64) {
        throw new IllegalArgumentException("bigintTag.bitLength (" + Long.toString((long)bigintTag.bitLength()) + ") is more than " + "64");
      }
      synchronized(tagHandlers) {
        tagHandlers.put(bigintTag,handler);
      }
    }

    private static CBORObject ConvertWithConverter(Object obj) {

      Object type = obj.getClass();
      ConverterInfo convinfo = null;
      synchronized(converters) {
        if (converters.size() == 0) {
          CBORTag0.AddConverter();
          CBORTag37.AddConverter();
          CBORTag32.AddConverter();
        }
        if (converters.containsKey(type)) {
          convinfo = converters.get(type);
        } else {
          return null;
        }
      }
      if (convinfo == null) {
        return null;
      }
      return (CBORObject)PropertyMap.InvokeOneArgumentMethod(
        convinfo.getToObject(),
        convinfo.getConverter(),
        obj);
    }

    CBORObject(int type, Object item) {

      this.itemtypeValue = type;
      this.itemValue = item;
    }

    /**
     * Gets the general data type of this CBOR object.
     * @return The general data type of this CBOR object.
     */
    public CBORType getType() {
        switch (this.getItemType()) {
          case CBORObjectTypeInteger:
          case CBORObjectTypeBigInteger:
          case CBORObjectTypeSingle:
          case CBORObjectTypeDouble:
          case CBORObjectTypeExtendedDecimal:
          case CBORObjectTypeExtendedFloat:
          case CBORObjectTypeExtendedRational:
            return CBORType.Number;
          case CBORObjectTypeSimpleValue:
            if (((Integer)this.getThisItem()).intValue() == 21 || ((Integer)this.getThisItem()).intValue() == 20) {
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
            throw new IllegalStateException("Unexpected data type");
        }
      }

    /**
     * Gets a value indicating whether this value is a CBOR true value.
     * @return True if this value is a CBOR true value; otherwise, false.
     */
    public boolean isTrue() {
        return this.getItemType() == CBORObjectTypeSimpleValue && ((Integer)this.getThisItem()).intValue() == 21;
      }

    /**
     * Gets a value indicating whether this value is a CBOR false value.
     * @return True if this value is a CBOR false value; otherwise, false.
     */
    public boolean isFalse() {
        return this.getItemType() == CBORObjectTypeSimpleValue && ((Integer)this.getThisItem()).intValue() == 20;
      }

    /**
     * Gets a value indicating whether this value is a CBOR null value.
     * @return True if this value is a CBOR null value; otherwise, false.
     */
    public boolean isNull() {
        return this.getItemType() == CBORObjectTypeSimpleValue && ((Integer)this.getThisItem()).intValue() == 22;
      }

    /**
     * Gets a value indicating whether this value is a CBOR undefined value.
     * @return True if this value is a CBOR undefined value; otherwise, false.
     */
    public boolean isUndefined() {
        return this.getItemType() == CBORObjectTypeSimpleValue && ((Integer)this.getThisItem()).intValue() == 23;
      }

    /**
     * Gets a value indicating whether this object&apos;s value equals
     * 0.
     * @return True if this object's value equals 0; otherwise, false.
     */
    public boolean isZero() {
        ICBORNumber cn = NumberInterfaces[this.getItemType()];
        return cn == null ? false : cn.IsZero(this.getThisItem());
      }

    /**
     * Gets this object&apos;s value with the sign reversed.
     * @return The reversed-sign form of this number.
     * @throws java.lang.IllegalStateException This object's type is
     * not a number type.
     */
    public CBORObject Negate() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      if (cn == null) {
        throw new IllegalStateException("This Object is not a number.");
      }
      return CBORObject.FromObject(cn.Negate(this.getThisItem()));
    }

    /**
     * Gets this object's absolute value.
     * @return This object's absolute without its negative sign.
     * @throws java.lang.IllegalStateException This object's type is
     * not a number type.
     */
    public CBORObject Abs() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      if (cn == null) {
        throw new IllegalStateException("This Object is not a number.");
      }
      Object oldItem = this.getThisItem();
      Object newItem = cn.Abs(oldItem);
      return (oldItem == newItem) ? this : CBORObject.FromObject(newItem);
    }

    private static int GetSignInternal(int type, Object obj) {
      ICBORNumber cn = NumberInterfaces[type];
      return cn == null ? 2 : cn.Sign(obj);
    }

    /**
     * Gets this value&apos;s sign: -1 if negative; 1 if positive; 0 if zero.
     * @return This value's sign: -1 if negative; 1 if positive; 0 if zero.
     * @throws java.lang.IllegalStateException This object's type is
     * not a number type, including the special not-a-number value (NaN).
     */
    public int signum() {
        int ret = GetSignInternal(this.getItemType(), this.getThisItem());
        if (ret == 2) {
          throw new IllegalStateException("This Object is not a number.");
        }
        return ret;
      }

    /**
     * The value positive infinity.
     */
    public static final CBORObject PositiveInfinity = CBORObject.FromObject(Double.POSITIVE_INFINITY);

    /**
     * The value negative infinity.
     */
    public static final CBORObject NegativeInfinity = CBORObject.FromObject(Double.NEGATIVE_INFINITY);

    /**
     * A not-a-number value.
     */
    public static final CBORObject NaN = CBORObject.FromObject(Double.NaN);

    /**
     * Gets a value indicating whether this CBOR object represents positive
     * infinity.
     * @return True if this CBOR object represents positive infinity; otherwise,
     * false.
     */
    public boolean IsPositiveInfinity() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      return cn == null ? false : cn.IsPositiveInfinity(this.getThisItem());
    }

    /**
     * Gets a value indicating whether this CBOR object represents infinity.
     * @return True if this CBOR object represents infinity; otherwise,
     * false.
     */
    public boolean IsInfinity() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      return cn == null ? false : cn.IsInfinity(this.getThisItem());
    }

    /**
     * Gets a value indicating whether this CBOR object represents a finite
     * number.
     * @return True if this CBOR object represents a finite number; otherwise,
     * false.
     */
    public boolean isFinite() {
        return this.getType() == CBORType.Number && !this.IsInfinity() && !this.IsNaN();
      }

    /**
     * Gets a value indicating whether this CBOR object represents negative
     * infinity.
     * @return True if this CBOR object represents negative infinity; otherwise,
     * false.
     */
    public boolean IsNegativeInfinity() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      return cn == null ? false : cn.IsNegativeInfinity(this.getThisItem());
    }

    /**
     * Gets a value indicating whether this CBOR object represents a not-a-number
     * value (as opposed to whether this object&apos;s type is not a number
     * type).
     * @return True if this CBOR object represents a not-a-number value
     * (as opposed to whether this object's type is not a number type); otherwise,
     * false.
     */
    public boolean IsNaN() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      return cn == null ? false : cn.IsNaN(this.getThisItem());
    }

    /**
     * Compares two CBOR objects.<p> In this implementation:</p> <ul>
     * <li>The null pointer (null reference) is considered less than any
     * other object.</li> <li> If either object is true, false, CBORObject.Null,
     * or the undefined value, it is treated as less than the other value.
     * If both objects have one of these four values, then undefined is less
     * than CBORObject.Null, which is less than false, which is less than
     * true.</li> <li> If both objects are numbers, their mathematical
     * values are compared. Here, NaN (not-a-number) is considered greater
     * than any number.</li> <li> If both objects are simple values other
     * than true, false, CBORObject.Null, and the undefined value, the
     * objects are compared according to their ordinal numbers.</li> <li>
     * If both objects are arrays, each element is compared. If one array
     * is shorter than the other and the other array begins with that array
     * (for the purposes of comparison), the shorter array is considered
     * less than the longer array.</li> <li> If both objects are strings,
     * compares each string code-point by code-point, as though by the DataUtilities.CodePointCompare
     * method.</li> <li> If both objects are maps, compares each map as though
     * each were an array with the sorted keys of that map as the array's elements.
     * If both maps have the same keys, their values are compared in the order
     * of the sorted keys.</li> <li> If each object is a different type, then
     * they are sorted by their type number, in the order given for the CBORType
     * enumeration.</li> <li> If each object has different tags and both
     * objects are otherwise equal under this method, each element is compared
     * as though each were an array with that object's tags listed in order
     * from outermost to innermost. </li> </ul> <p> This method is not consistent
     * with the Equals method.</p>
     * @param other A value to compare with.
     * @return Less than 0, if this value is less than the other object; or
     * 0, if both values are equal; or greater than 0, if this value is less
     * than the other object or if the other object is null.
     */
    @SuppressWarnings("unchecked")
public int compareTo(CBORObject other) {
      if (other == null) {
        return 1;
      }
      if (this == other) {
        return 0;
      }
      int typeA = this.getItemType();
      int typeB = other.getItemType();
      Object objA = this.getThisItem();
      Object objB = other.getThisItem();
      int simpleValueA = -1;
      int simpleValueB = -1;
      if (typeA == CBORObjectTypeSimpleValue) {
        if (((Integer)objA).intValue() == 20) {  // false
          simpleValueA = 2;
        } else if (((Integer)objA).intValue() == 21) {  // true
          simpleValueA = 3;
        } else if (((Integer)objA).intValue() == 22) {  // null
          simpleValueA = 1;
        } else if (((Integer)objA).intValue() == 23) {  // undefined
          simpleValueA = 0;
        }
      }
      if (typeB == CBORObjectTypeSimpleValue) {
        if (((Integer)objB).intValue() == 20) {  // false
          simpleValueB = 2;
        } else if (((Integer)objB).intValue() == 21) {  // true
          simpleValueB = 3;
        } else if (((Integer)objB).intValue() == 22) {  // null
          simpleValueB = 1;
        } else if (((Integer)objB).intValue() == 23) {  // undefined
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
              long a = (((Long)objA).longValue());
              long b = (((Long)objB).longValue());
              if (a == b) {
                cmp = 0;
              } else {
                cmp = (a < b) ? -1 : 1;
              }
              break;
            }
          case CBORObjectTypeSingle: {
              float a = ((Float)objA).floatValue();
              float b = ((Float)objB).floatValue();
              // Treat NaN as greater than all other numbers
              if (Float.isNaN(a)) {
                cmp = Float.isNaN(b) ? 0 : 1;
              } else if (Float.isNaN(b)) {
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
              cmp = bigintA.compareTo(bigintB);
              break;
            }
          case CBORObjectTypeDouble: {
              double a = ((Double)objA).doubleValue();
              double b = ((Double)objB).doubleValue();
              // Treat NaN as greater than all other numbers
              if (Double.isNaN(a)) {
                cmp = Double.isNaN(b) ? 0 : 1;
              } else if (Double.isNaN(b)) {
                cmp = -1;
              } else if (a == b) {
                cmp = 0;
              } else {
                cmp = (a < b) ? -1 : 1;
              }
              break;
            }
          case CBORObjectTypeExtendedDecimal: {
              cmp = ((ExtendedDecimal)objA).compareTo(
                (ExtendedDecimal)objB);
              break;
            }
          case CBORObjectTypeExtendedFloat: {
              cmp = ((ExtendedFloat)objA).compareTo(
                (ExtendedFloat)objB);
              break;
            }
          case CBORObjectTypeExtendedRational: {
              cmp = ((ExtendedRational)objA).compareTo(
                (ExtendedRational)objB);
              break;
            }
          case CBORObjectTypeByteString: {
              cmp = CBORUtilities.ByteArrayCompare((byte[])objA, (byte[])objB);
              break;
            }
          case CBORObjectTypeTextString: {
              cmp = DataUtilities.CodePointCompare(
                (String)objA,
                (String)objB);
              break;
            }
          case CBORObjectTypeArray: {
              cmp = ListCompare(
                (ArrayList<CBORObject>)objA,
                (ArrayList<CBORObject>)objB);
              break;
            }
          case CBORObjectTypeMap: {
              cmp = MapCompare(
                (Map<CBORObject, CBORObject>)objA,
                (Map<CBORObject, CBORObject>)objB);
              break;
            }
          case CBORObjectTypeSimpleValue: {
              int valueA = ((Integer)objA).intValue();
              int valueB = ((Integer)objB).intValue();
              if (valueA == valueB) {
                cmp = 0;
              } else {
                cmp = (valueA < valueB) ? -1 : 1;
              }
              break;
            }
          default:
            throw new IllegalArgumentException("Unexpected data type");
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
          // first Object is NaN
          return 1;
        } else if (s2 == 2) {
          // second Object is NaN
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
              cmp = e1.compareTo(e2);
            }
          } else if (typeA == CBORObjectTypeExtendedFloat || typeB == CBORObjectTypeExtendedFloat ||
                     typeA == CBORObjectTypeDouble || typeB == CBORObjectTypeDouble ||
                     typeA == CBORObjectTypeSingle || typeB == CBORObjectTypeSingle) {
            ExtendedFloat e1 = NumberInterfaces[typeA].AsExtendedFloat(objA);
            ExtendedFloat e2 = NumberInterfaces[typeB].AsExtendedFloat(objB);
            cmp = e1.compareTo(e2);
          } else {
            BigInteger b1 = NumberInterfaces[typeA].AsBigInteger(objA);
            BigInteger b2 = NumberInterfaces[typeB].AsBigInteger(objB);
            cmp = b1.compareTo(b2);
          }
        }
      }
      if (cmp == 0) {
        if (!this.isTagged() && !other.isTagged()) {
          return 0;
        }
        return TagsCompare(this.GetTags(), other.GetTags());
      }
      return cmp;
    }

    private static int TagsCompare(BigInteger[] tagsA, BigInteger[] tagsB) {
      if (tagsA == null) {
        return (tagsB == null) ? 0 : -1;
      }
      if (tagsB == null) {
        return 1;
      }
      int listACount = tagsA.length;
      int listBCount = tagsB.length;
      int c = Math.min(listACount, listBCount);
      for (int i = 0; i < c; ++i) {
        int cmp = tagsA[i].compareTo(tagsB[i]);
        if (cmp != 0) {
          return cmp;
        }
      }
      if (listACount != listBCount) {
        return (listACount < listBCount) ? -1 : 1;
      }
      return 0;
    }

    private static int ListCompare(ArrayList<CBORObject> listA, ArrayList<CBORObject> listB) {
      if (listA == null) {
        return (listB == null) ? 0 : -1;
      }
      if (listB == null) {
        return 1;
      }
      int listACount = listA.size();
      int listBCount = listB.size();
      int c = Math.min(listACount, listBCount);
      for (int i = 0; i < c; ++i) {
        int cmp = listA.get(i).compareTo(listB.get(i));
        if (cmp != 0) {
          return cmp;
        }
      }
      if (listACount != listBCount) {
        return (listACount < listBCount) ? -1 : 1;
      }
      return 0;
    }

    /**
     * Gets an object with the same value as this one but without the tags it
     * has, if any. If this object is an array, map, or byte string, the data
     * will not be copied to the returned object, so changes to the returned
     * object will be reflected in this one.
     * @return A CBORObject object.
     */
    public CBORObject Untag() {
      CBORObject curobject = this;
      while (curobject.itemtypeValue == CBORObjectTypeTagged) {
        curobject = (CBORObject)curobject.itemValue;
      }
      return curobject;
    }

    /**
     * Gets an object with the same value as this one but without this object's
     * outermost tag, if any. If this object is an array, map, or byte string,
     * the data will not be copied to the returned object, so changes to the
     * returned object will be reflected in this one.
     * @return A CBORObject object.
     */
    public CBORObject UntagOne() {
      if (this.itemtypeValue == CBORObjectTypeTagged) {
        return (CBORObject)this.itemValue;
      }
      return this;
    }

    void Redefine(CBORObject cbor) {

      this.itemtypeValue = cbor.itemtypeValue;
      this.tagLow = cbor.tagLow;
      this.tagHigh = cbor.tagHigh;
      this.itemValue = cbor.itemValue;
    }

    private static int MapCompare(Map<CBORObject, CBORObject> mapA, Map<CBORObject, CBORObject> mapB) {
      if (mapA == null) {
        return (mapB == null) ? 0 : -1;
      }
      if (mapB == null) {
        return 1;
      }
      if (mapA == mapB) {
        return 0;
      }
      int listACount = mapA.size();
      int listBCount = mapB.size();
      if (listACount == 0 && listBCount == 0) {
        return 0;
      }
      if (listACount == 0) {
        return -1;
      }
      if (listBCount == 0) {
        return 1;
      }
      Map<CBORObject, CBORObject> sortedA =
        new TreeMap<CBORObject, CBORObject>(mapA);
      Map<CBORObject, CBORObject> sortedB =
        new TreeMap<CBORObject, CBORObject>(mapB);
      List<CBORObject> sortedASet = new ArrayList<CBORObject>(sortedA.keySet());
      List<CBORObject> sortedBSet = new ArrayList<CBORObject>(sortedB.keySet());
      listACount = sortedASet.size();
      listBCount = sortedBSet.size();
      int minCount = Math.min(listACount, listBCount);
      // Compare the keys
      for (int i = 0; i < minCount; ++i) {
        CBORObject itemA = sortedASet.get(i);
        CBORObject itemB = sortedBSet.get(i);
        if (itemA == null) {
          return -1;
        }
        int cmp = itemA.compareTo(itemB);
        if (cmp != 0) {
          return cmp;
        }
      }
      if (listACount == listBCount) {
        // Both maps have the same keys, so compare their values
        for (int i = 0; i < minCount; ++i) {
          CBORObject keyA = sortedASet.get(i);
          CBORObject keyB = sortedBSet.get(i);
          int cmp = mapA.get(keyA).compareTo(mapB.get(keyB));
          if (cmp != 0) {
            return cmp;
          }
        }
        return 0;
      }
      return (listACount > listBCount) ? 1 : -1;
    }

    private static boolean CBORArrayEquals(
      List<CBORObject> listA,
      List<CBORObject> listB) {
      if (listA == null) {
        return listB == null;
      }
      if (listB == null) {
        return false;
      }
      int listACount = listA.size();
      int listBCount = listB.size();
      if (listACount != listBCount) {
        return false;
      }
      for (int i = 0; i < listACount; ++i) {
        CBORObject itemA = listA.get(i);
        CBORObject itemB = listB.get(i);
        if (!(itemA == null ? itemB == null : itemA.equals(itemB))) {
          return false;
        }
      }
      return true;
    }

    private static int CBORArrayHashCode(List<CBORObject> list) {
      if (list == null) {
        return 0;
      }
      int ret = 19;
      int count = list.size();
      {
        ret = (ret * 31) + count;
        for (int i = 0; i < count; ++i) {
          ret = (ret * 31) + list.get(i).hashCode();
        }
      }
      return ret;
    }

    private static boolean CBORMapEquals(Map<CBORObject, CBORObject> mapA, Map<CBORObject, CBORObject> mapB) {
      if (mapA == null) {
        return mapB == null;
      }
      if (mapB == null) {
        return false;
      }
      if (mapA.size() != mapB.size()) {
        return false;
      }
      for(Map.Entry<CBORObject, CBORObject> kvp : mapA.entrySet()) {
        CBORObject valueB = null;
        boolean hasKey;
valueB=mapB.get(kvp.getKey());
hasKey=(valueB==null) ? mapB.containsKey(kvp.getKey()) : true;
        if (hasKey) {
          CBORObject valueA = kvp.getValue();
          if (!(valueA == null ? valueB == null : valueA.equals(valueB))) {
            return false;
          }
        } else {
          return false;
        }
      }
      return true;
    }

    private static int CBORMapHashCode(Map<CBORObject, CBORObject> a) {
      // To simplify matters, we use just the count of
      // the map as the basis for the hash code. More complicated
      // hash code calculation would generally involve defining
      // how CBORObjects ought to be compared (since a stable
      // sort order is necessary for two equal maps to have the
      // same hash code), which is much too difficult to do.
      return (a.size() * 19);
    }

    /**
     * Determines whether this object and another object are equal.
     * @param obj An arbitrary object.
     * @return True if the objects are equal; otherwise, false.
     */
    @Override public boolean equals(Object obj) {
      return this.equals(((obj instanceof CBORObject) ? (CBORObject)obj : null));
    }

    /**
     * Compares the equality of two CBOR objects.
     * @param other The object to compare.
     * @return True if the objects are equal; otherwise, false.
     */
    @SuppressWarnings("unchecked")
public boolean equals(CBORObject other) {
      CBORObject otherValue = ((other instanceof CBORObject) ? (CBORObject)other : null);
      if (otherValue == null) {
        return false;
      }
      if (this == otherValue) {
        return true;
      }
      switch (this.itemtypeValue) {
        case CBORObjectTypeByteString:
          if (!CBORUtilities.ByteArrayEquals((byte[])this.getThisItem(), ((otherValue.itemValue instanceof byte[]) ? (byte[])otherValue.itemValue : null))) {
            return false;
          }
          break;
        case CBORObjectTypeMap: {
            Map<CBORObject, CBORObject> cbordict = ((otherValue.itemValue instanceof Map<?,?>) ? (Map<CBORObject,
            CBORObject>)otherValue.itemValue : null);
            if (!CBORMapEquals(this.AsMap(), cbordict)) {
              return false;
            }
            break;
          }
        case CBORObjectTypeArray:
          if (!CBORArrayEquals(this.AsList(), ((otherValue.itemValue instanceof List<?>) ? (List<CBORObject>)otherValue.itemValue : null))) {
            return false;
          }
          break;
        default:
          if (!(((this.itemValue)==null) ? ((otherValue.itemValue)==null) : (this.itemValue).equals(otherValue.itemValue))) {
            return false;
          }
          break;
      }
      return this.itemtypeValue == otherValue.itemtypeValue &&
        this.tagLow == otherValue.tagLow &&
        this.tagHigh == otherValue.tagHigh;
    }

    /**
     * Calculates the hash code of this object.
     * @return A 32-bit hash code.
     */
    @Override public int hashCode() {
      int valueHashCode = 651869431;
      {
        if (this.itemValue != null) {
          int itemHashCode = 0;
          switch (this.itemtypeValue) {
            case CBORObjectTypeByteString:
              itemHashCode = CBORUtilities.ByteArrayHashCode((byte[])this.getThisItem());
              break;
            case CBORObjectTypeMap:
              itemHashCode = CBORMapHashCode(this.AsMap());
              break;
            case CBORObjectTypeArray:
              itemHashCode = CBORArrayHashCode(this.AsList());
              break;
            default:
              itemHashCode = this.itemValue.hashCode();
              break;
          }
          valueHashCode += 651869479 * itemHashCode;
        }
        valueHashCode += 651869483 * (this.itemtypeValue + this.tagLow + this.tagHigh);
      }
      return valueHashCode;
    }

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

    private static String GetOptimizedStringIfShortAscii(
      byte[] data,
      int offset) {
      int length = data.length;
      if (length > offset) {
        int nextbyte = (int)(data[offset] & (int)0xff);
        if (nextbyte >= 0x60 && nextbyte < 0x78) {
          int offsetp1 = 1 + offset;
          // Check for type 3 String of short length
          int rightLength = offsetp1 + (nextbyte - 0x60);
          CheckCBORLength(rightLength, length);
          // Check for all ASCII text
          for (int i = offsetp1; i < length; ++i) {
            if ((data[i] & ((byte)0x80)) != 0) {
              return null;
            }
          }
          // All ASCII text, so convert to a String
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
      valueFixedObjects[0x60] = new CBORObject(CBORObjectTypeTextString, "");
      for (int i = 0xe0; i < 0xf8; ++i) {
        valueFixedObjects[i] = new CBORObject(CBORObjectTypeSimpleValue, (int)(i - 0xe0));
      }
      return valueFixedObjects;
    }

    static CBORObject GetFixedObject(int value) {
      return valueFixedObjects[value];
    }

    static int GetExpectedLength(int value) {
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

    // Generate a CBOR Object for head bytes with fixed length.
    // Note that this function assumes that the length of the data
    // was already checked.
    static CBORObject GetFixedLengthObject(int firstbyte, byte[] data) {
      CBORObject fixedObj = valueFixedObjects[firstbyte];
      if (fixedObj != null) {
        return fixedObj;
      }
      int majortype = firstbyte >> 5;
      if (firstbyte >= 0x61 && firstbyte < 0x78) {
        // text String length 1 to 23
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
              int low = ((int)((uadditional) & 0xFFFFFFFFL));
              int high = ((int)((uadditional >> 32) & 0xFFFFFFFFL));
              return FromObject(LowHighToBigInteger(low, high));
            }
          case 1:
            if ((uadditional >> 63) == 0) {
              // use only if additional's top bit isn't set
              // (additional is a signed long)
              return new CBORObject(CBORObjectTypeInteger, -1 - uadditional);
            } else {
              int low = ((int)((uadditional) & 0xFFFFFFFFL));
              int high = ((int)((uadditional >> 32) & 0xFFFFFFFFL));
              BigInteger bigintAdditional = LowHighToBigInteger(low, high);
              BigInteger minusOne =(BigInteger.ONE).negate();
              bigintAdditional = minusOne.subtract(bigintAdditional);
              return FromObject(bigintAdditional);
            }
          case 7:
            if (firstbyte == 0xf9) {
              return new CBORObject(
                CBORObjectTypeSingle,
                CBORUtilities.HalfPrecisionToSingle(((int)uadditional)));
            } else if (firstbyte == 0xfa) {
              return new CBORObject(
                CBORObjectTypeSingle,
                Float.intBitsToFloat(((int)uadditional)));
            } else if (firstbyte == 0xfb) {
              return new CBORObject(
                CBORObjectTypeDouble,
                Double.longBitsToDouble(uadditional));
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
      } else if (majortype == 2) {  // short byte String
        byte[] ret = new byte[firstbyte - 0x40];
        System.arraycopy(data, 1, ret, 0, firstbyte - 0x40);
        return new CBORObject(CBORObjectTypeByteString, ret);
      } else if (majortype == 3) {  // short text String
        StringBuilder ret = new StringBuilder(firstbyte - 0x60);
        DataUtilities.ReadUtf8FromBytes(data, 1, firstbyte - 0x60, ret, false);
        return new CBORObject(CBORObjectTypeTextString, ret.toString());
      } else if (firstbyte == 0x80) {
        // empty array
        return FromObject(new ArrayList<CBORObject>());
      } else if (firstbyte == 0xa0) {
        // empty map
        return FromObject(new HashMap<CBORObject, CBORObject>());
      } else {
        throw new CBORException("Unexpected data encountered");
      }
    }

    /**
     * Generates a CBOR object from an array of CBOR-encoded bytes.
     * @param data A byte array.
     * @return A CBOR object corresponding to the data.
     * @throws java.lang.IllegalArgumentException Data is null or empty.
     * @throws CBORException There was an error in reading or parsing the
     * data. This includes cases where not all of the byte array represents
     * a CBOR object.
     * @throws java.lang.NullPointerException The parameter {@code data}
     * is null.
     */
    public static CBORObject DecodeFromBytes(byte[] data) {
      if (data == null) {
        throw new NullPointerException("data");
      }
      if (data.length == 0) {
        throw new IllegalArgumentException("data is empty.");
      }
      int firstbyte = (int)(data[0] & (int)0xff);
      int expectedLength = valueExpectedLengths[firstbyte];
      // if invalid
      if (expectedLength == -1) {
        throw new CBORException("Unexpected data encountered");
      } else if (expectedLength != 0) {
        // if fixed length
        CheckCBORLength(expectedLength, data.length);
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
      // read the Object as though
      // the byte array were a stream
      java.io.ByteArrayInputStream ms=null;
try {
ms=new java.io.ByteArrayInputStream(data);
int startingAvailable=ms.available();

        CBORObject o = Read(ms);
        CheckCBORLength((long)data.length, (long)(startingAvailable-ms.available()));
        return o;
}
finally {
try { if(ms!=null)ms.close(); } catch (java.io.IOException ex){}
}
    }

    /**
     * Gets the number of keys in this map, or the number of items in this array,
     * or 0 if this item is neither an array nor a map.
     * @return The number of keys in this map, or the number of items in this
     * array, or 0 if this item is neither an array nor a map.
     */
    public int size() {
        if (this.getItemType() == CBORObjectTypeArray) {
          return this.AsList().size();
        } else if (this.getItemType() == CBORObjectTypeMap) {
          return this.AsMap().size();
        } else {
          return 0;
        }
      }

    /**
     * Gets a value indicating whether this data item has at least one tag.
     * @return True if this data item has at least one tag; otherwise, false.
     */
    public boolean isTagged() {
        return this.itemtypeValue == CBORObjectTypeTagged;
      }

    /**
     * Gets the byte array used in this object, if this object is a byte string,
     * without copying the data to a new one. This method's return value can
     * be used to modify the array's contents. Note, though, that the array's
     * length can't be changed.
     * @return A byte array.
     * @throws IllegalStateException This object is not a byte string.
     */
    public byte[] GetByteString() {
      if (this.getItemType() == CBORObjectTypeByteString) {
        return (byte[])this.getThisItem();
      } else {
        throw new IllegalStateException("Not a byte String");
      }
    }

    /**
     * Returns whether this object has a tag of the given number.
     * @param tagValue The tag value to search for.
     * @return True if this object has a tag of the given number; otherwise,
     * false.
     * @throws java.lang.IllegalArgumentException TagValue is less than 0.
     * @throws java.lang.NullPointerException The parameter "obj" is null.
     */
    public boolean HasTag(int tagValue) {
      if (tagValue < 0) {
        throw new IllegalArgumentException("tagValue (" + Long.toString((long)tagValue) + ") is less than " + "0");
      }
      CBORObject obj = this;
      while (true) {
        if (!obj.isTagged()) {
          return false;
        }
        if (obj.tagHigh == 0 && tagValue == obj.tagLow) {
          return true;
        }
        obj = (CBORObject)obj.itemValue;

      }
    }

    /**
     * Returns whether this object has a tag of the given number.
     * @param bigTagValue The tag value to search for.
     * @return True if this object has a tag of the given number; otherwise,
     * false.
     * @throws java.lang.NullPointerException BigTagValue is null.
     * @throws java.lang.IllegalArgumentException BigTagValue is less than 0.
     */
    public boolean HasTag(BigInteger bigTagValue) {
      if (bigTagValue == null) {
        throw new NullPointerException("bigTagValue");
      }
      if (!(bigTagValue.signum() >= 0)) {
        throw new IllegalArgumentException("doesn't satisfy bigTagValue.signum()>= 0");
      }
      BigInteger[] bigTags = this.GetTags();
      for(BigInteger bigTag : bigTags) {
        if (bigTagValue.equals(bigTag)) {
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
        return BigInteger.fromByteArray((byte[])uabytes,true);
      } else if (tagLow != 0) {
        uabytes = new byte[5];
        uabytes[3] = (byte)((tagLow >> 24) & 0xff);
        uabytes[2] = (byte)((tagLow >> 16) & 0xff);
        uabytes[1] = (byte)((tagLow >> 8) & 0xff);
        uabytes[0] = (byte)(tagLow & 0xff);
        uabytes[4] = 0;
        return BigInteger.fromByteArray((byte[])uabytes,true);
      } else {
        return BigInteger.ZERO;
      }
    }

    private static BigInteger[] valueEmptyTags = new BigInteger[0];

    /**
     * Gets a list of all tags, from outermost to innermost.
     * @return An array of tags, or the empty string if this object is untagged.
     */
    public BigInteger[] GetTags() {
      if (!this.isTagged()) {
        return valueEmptyTags;
      }
      CBORObject curitem = this;
      if (curitem.isTagged()) {
        ArrayList<BigInteger> list=new ArrayList<BigInteger>();
        while (curitem.isTagged()) {
          list.add(LowHighToBigInteger(
            curitem.tagLow,
            curitem.tagHigh));
          curitem = (CBORObject)curitem.itemValue;
        }
        return list.toArray(new BigInteger[]{});
      } else {
        return new BigInteger[] { LowHighToBigInteger(this.tagLow, this.tagHigh) };
      }
    }

    /**
     * Gets the outermost tag for this CBOR data item, or -1 if the item is untagged.
     * @return The outermost tag for this CBOR data item, or -1 if the item
     * is untagged.
     */
    public BigInteger getOutermostTag() {
        if (!this.isTagged()) {
          return BigInteger.ZERO;
        }
        if (this.tagHigh == 0 &&
            this.tagLow >= 0 &&
            this.tagLow < 0x10000) {
          return BigInteger.valueOf(this.tagLow);
        }
        return LowHighToBigInteger(
          this.tagLow,
          this.tagHigh);
      }

    /**
     * Gets the last defined tag for this CBOR data item, or -1 if the item is
     * untagged.
     * @return The last defined tag for this CBOR data item, or -1 if the item
     * is untagged.
     */
    public BigInteger getInnermostTag() {
        if (!this.isTagged()) {
          return BigInteger.ZERO.subtract(BigInteger.ONE);
        }
        CBORObject previtem = this;
        CBORObject curitem = (CBORObject)this.itemValue;
        while (curitem.isTagged()) {
          previtem = curitem;
          curitem = (CBORObject)curitem.itemValue;
        }
        if (previtem.tagHigh == 0 &&
            previtem.tagLow >= 0 &&
            previtem.tagLow < 0x10000) {
          return BigInteger.valueOf(previtem.tagLow);
        }
        return LowHighToBigInteger(
          previtem.tagLow,
          previtem.tagHigh);
      }

    @SuppressWarnings("unchecked")
private Map<CBORObject, CBORObject> AsMap() {
      return (Map<CBORObject, CBORObject>)this.getThisItem();
    }

    @SuppressWarnings("unchecked")
private List<CBORObject> AsList() {
      return (List<CBORObject>)this.getThisItem();
    }

    /**
     * Gets the value of a CBOR object by integer index in this array.
     * @param index Zero-based index of the element.
     * @return A CBORObject object.
     * @throws java.lang.IllegalStateException This object is not an
     * array.
     * @throws java.lang.NullPointerException The parameter "value" is
     * null.
     */
    public CBORObject get(int index) {
        if (this.getItemType() == CBORObjectTypeArray) {
          List<CBORObject> list = this.AsList();
          return list.get(index);
        } else {
          throw new IllegalStateException("Not an array");
        }
      }

    /**
     * Sets the value of a CBOR object by integer index in this array.
     * @throws java.lang.IllegalStateException This object is not an
     * array.
     * @throws java.lang.NullPointerException Value is null (as opposed
     * to CBORObject.Null).
     */
public void set(int index, CBORObject value) {
        if (this.getItemType() == CBORObjectTypeArray) {
          if (value == null) {
            throw new NullPointerException("value");
          }
          List<CBORObject> list = this.AsList();
          list.set(index,value);
        } else {
          throw new IllegalStateException("Not an array");
        }
      }

    /**
     * Gets a collection of the keys of this CBOR object in an undefined order.
     * @return A collection of the keys of this CBOR object.
     * @throws java.lang.IllegalStateException This object is not a map.
     */
    public Collection<CBORObject> getKeys() {
        if (this.getItemType() == CBORObjectTypeMap) {
          Map<CBORObject, CBORObject> dict = this.AsMap();
          return dict.keySet();
        } else {
          throw new IllegalStateException("Not a map");
        }
      }

    /**
     * Gets a collection of the values of this CBOR object. If this object
     * is a map, returns one value for each key in the map in an undefined order.
     * If this is an array, returns all the values of the array in the order
     * they are listed.
     * @return A collection of the values of this CBOR object.
     * @throws java.lang.IllegalStateException This object is not a map
     * or an array.
     */
    public Collection<CBORObject> getValues() {
        if (this.getItemType() == CBORObjectTypeMap) {
          Map<CBORObject, CBORObject> dict = this.AsMap();
          return dict.values();
        } else if (this.getItemType() == CBORObjectTypeArray) {
          List<CBORObject> list = this.AsList();
          return java.util.Collections.unmodifiableList(list);
        } else {
          throw new IllegalStateException("Not a map or array");
        }
      }

    /**
     * Gets the value of a CBOR object in this map, using a CBOR object as the
     * key.
     * @param key A CBORObject object. (2).
     * @return A CBORObject object.
     * @throws java.lang.NullPointerException The key is null (as opposed
     * to CBORObject.Null).
     * @throws java.lang.IllegalStateException This object is not a map.
     */
    public CBORObject get(CBORObject key) {
        if (key == null) {
          throw new NullPointerException("key");
        }
        if (this.getItemType() == CBORObjectTypeMap) {
          Map<CBORObject, CBORObject> map = this.AsMap();
          if (!map.containsKey(key)) {
            return null;
          }
          return map.get(key);
        } else {
          throw new IllegalStateException("Not a map");
        }
      }

    /**
     * Sets the value of a CBOR object in this map, using a CBOR object as the
     * key.
     * @throws java.lang.NullPointerException The key or value is null (as
     * opposed to CBORObject.Null).
     * @throws java.lang.IllegalStateException This object is not a map.
     */
public void set(CBORObject key, CBORObject value) {
        if (key == null) {
          throw new NullPointerException("key");
        }
        if (value == null) {
          throw new NullPointerException("value");
        }
        if (this.getItemType() == CBORObjectTypeMap) {
          Map<CBORObject, CBORObject> map = this.AsMap();
          map.put(key,value);
        } else {
          throw new IllegalStateException("Not a map");
        }
      }

    /**
     * Gets the value of a CBOR object in this map, using a string as the key.
     * @param key A key that points to the desired value.
     * @return A CBORObject object.
     * @throws java.lang.NullPointerException The key is null.
     * @throws java.lang.IllegalStateException This object is not a map.
     */
    public CBORObject get(String key) {
        if (key == null) {
          throw new NullPointerException("key");
        }
        CBORObject objkey = CBORObject.FromObject(key);
        return this.get(objkey);
      }

    /**
     * Sets the value of a CBOR object in this map, using a string as the key.
     * @throws java.lang.NullPointerException The key or value is null (as
     * opposed to CBORObject.Null).
     * @throws java.lang.IllegalStateException This object is not a map.
     */
public void set(String key, CBORObject value) {
        if (key == null) {
          throw new NullPointerException("key");
        }
        if (value == null) {
          throw new NullPointerException("value");
        }
        CBORObject objkey = CBORObject.FromObject(key);
        if (this.getItemType() == CBORObjectTypeMap) {
          Map<CBORObject, CBORObject> map = this.AsMap();
          map.put(objkey,value);
        } else {
          throw new IllegalStateException("Not a map");
        }
      }

    /**
     * Gets the simple value ID of this object, or -1 if this object is not a
     * simple value (including if the value is a floating-point number).
     * @return The simple value ID of this object, or -1 if this object is not
     * a simple value (including if the value is a floating-point number).
     */
    public int getSimpleValue() {
        if (this.getItemType() == CBORObjectTypeSimpleValue) {
          return ((Integer)this.getThisItem()).intValue();
        } else {
          return -1;
        }
      }

    /**
     * Adds a new object to this map.
     * @param key A CBOR object representing the key. Can be null, in which
     * case this value is converted to CBORObject.Null.
     * @param value A CBOR object representing the value. Can be null, in
     * which case this value is converted to CBORObject.Null.
     * @return This object.
     * @throws java.lang.IllegalArgumentException Key already exists in this map.
     * @throws IllegalStateException This object is not a map.
     */
    public CBORObject Add(CBORObject key, CBORObject value) {
      if (key == null) {
        key = CBORObject.Null;
      }
      if (value == null) {
        value = CBORObject.Null;
      }
      if (this.getItemType() == CBORObjectTypeMap) {
        Map<CBORObject, CBORObject> map = this.AsMap();
        if (map.containsKey(key)) {
          throw new IllegalArgumentException("Key already exists.");
        }
        map.put(key, value);
      } else {
        throw new IllegalStateException("Not a map");
      }
      return this;
    }

    /**
     * Converts an object to a CBOR object and adds it to this map.
     * @param key A string representing the key. Can be null, in which case
     * this value is converted to CBORObject.Null.
     * @param valueOb An arbitrary object. Can be null, in which case this
     * value is converted to CBORObject.Null.
     * @return This object.
     * @throws java.lang.IllegalArgumentException The parameter {@code key} or
     * "value" has an unsupported type.
     * @throws java.lang.IllegalArgumentException The parameter {@code key} already
     * exists in this map.
     * @throws IllegalStateException This object is not a map.
     */
    public CBORObject Add(Object key, Object valueOb) {
      return this.Add(CBORObject.FromObject(key), CBORObject.FromObject(valueOb));
    }

    /**
     * Determines whether a value of the given key exists in this object.
     * @param key An object that serves as the key.
     * @return True if the given key is found, or false if the given key is not
     * found or this object is not a map.
     * @throws java.lang.NullPointerException Key is null (as opposed to
     * CBORObject.Null).
     */
    public boolean ContainsKey(CBORObject key) {
      if (key == null) {
        throw new NullPointerException("key");
      }
      if (this.getItemType() == CBORObjectTypeMap) {
        Map<CBORObject, CBORObject> map = this.AsMap();
        return map.containsKey(key);
      } else {
        return false;
      }
    }

    /**
     * Determines whether a value of the given key exists in this object.
     * @param key A string that serves as the key.
     * @return True if the given key (as a CBOR object) is found, or false if
     * the given key is not found or this object is not a map.
     * @throws java.lang.NullPointerException Key is null (as opposed to
     * CBORObject.Null).
     */
    public boolean ContainsKey(String key) {
      if (key == null) {
        throw new NullPointerException("key");
      }
      if (this.getItemType() == CBORObjectTypeMap) {
        Map<CBORObject, CBORObject> map = this.AsMap();
        return map.containsKey(CBORObject.FromObject(key));
      } else {
        return false;
      }
    }

    /**
     * Adds a new object to the end of this array.
     * @param obj A CBOR object.
     * @return This object.
     * @throws java.lang.IllegalStateException This object is not an
     * array.
     * @throws java.lang.NullPointerException The parameter {@code obj}
     * is null (as opposed to CBORObject.Null).
     */
    public CBORObject Add(CBORObject obj) {
      if (obj == null) {
        throw new NullPointerException("obj");
      }
      if (this.getItemType() == CBORObjectTypeArray) {
        List<CBORObject> list = this.AsList();
        list.add(obj);
        return this;
      } else {
        throw new IllegalStateException("Not an array");
      }
    }

    /**
     * Converts an object to a CBOR object and adds it to the end of this array.
     * @param obj A CBOR object.
     * @return This object.
     * @throws java.lang.IllegalStateException This object is not an
     * array.
     * @throws java.lang.IllegalArgumentException The object's type is not supported.
     */
    public CBORObject Add(Object obj) {
      if (this.getItemType() == CBORObjectTypeArray) {
        List<CBORObject> list = this.AsList();
        list.add(CBORObject.FromObject(obj));
        return this;
      } else {
        throw new IllegalStateException("Not an array");
      }
    }

    /**
     * If this object is an array, removes the first instance of the specified
     * item from the array. If this object is a map, removes the item with the
     * given key from the map.
     * @param obj The item or key to remove.
     * @return True if the item was removed; otherwise, false.
     * @throws java.lang.NullPointerException The parameter {@code obj}
     * is null (as opposed to CBORObject.Null).
     * @throws java.lang.IllegalStateException The object is not an array
     * or map.
     */
    public boolean Remove(CBORObject obj) {
      if (obj == null) {
        throw new NullPointerException("obj");
      }
      if (this.getItemType() == CBORObjectTypeMap) {
        Map<CBORObject, CBORObject> dict = this.AsMap();
        boolean hasKey = dict.containsKey(obj);
        if (hasKey) {
          dict.remove(obj);
          return true;
        }
        return false;
      } else if (this.getItemType() == CBORObjectTypeArray) {
        List<CBORObject> list = this.AsList();
        return list.remove(obj);
      } else {
        throw new IllegalStateException("Not a map or array");
      }
    }

    /**
     * Converts this object to a 64-bit floating point number.
     * @return The closest 64-bit floating point number to this object.
     * The return value can be positive infinity or negative infinity if
     * this value exceeds the range of a 64-bit floating point number.
     * @throws java.lang.IllegalStateException This object's type is
     * not a number type.
     */
    public double AsDouble() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      if (cn == null) {
        throw new IllegalStateException("Not a number type");
      }
      return cn.AsDouble(this.getThisItem());
    }

    /**
     * Converts this object to a decimal number.
     * @return A decimal number for this object's value. If this object is
     * a rational number with a nonterminating decimal expansion, returns
     * a decimal number rounded to 34 digits.
     * @throws java.lang.IllegalStateException This object's type is
     * not a number type.
     */
    public ExtendedDecimal AsExtendedDecimal() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      if (cn == null) {
        throw new IllegalStateException("Not a number type");
      }
      return cn.AsExtendedDecimal(this.getThisItem());
    }

    /**
     * Converts this object to a rational number.
     * @return A rational number for this object's value.
     * @throws java.lang.IllegalStateException This object's type is
     * not a number type.
     */
    public ExtendedRational AsExtendedRational() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      if (cn == null) {
        throw new IllegalStateException("Not a number type");
      }
      return cn.AsExtendedRational(this.getThisItem());
    }

    /**
     * Converts this object to an arbitrary-precision binary floating
     * point number.
     * @return An arbitrary-precision binary floating point number for
     * this object's value. Note that if this object is a decimal number with
     * a fractional part, the conversion may lose information depending
     * on the number. If this object is a rational number with a nonterminating
     * binary expansion, returns a decimal number rounded to 113 digits.
     * @throws java.lang.IllegalStateException This object's type is
     * not a number type.
     */
    public ExtendedFloat AsExtendedFloat() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      if (cn == null) {
        throw new IllegalStateException("Not a number type");
      }
      return cn.AsExtendedFloat(this.getThisItem());
    }

    /**
     * Converts this object to a 32-bit floating point number.
     * @return The closest 32-bit floating point number to this object.
     * The return value can be positive infinity or negative infinity if
     * this object's value exceeds the range of a 32-bit floating point number.
     * @throws java.lang.IllegalStateException This object's type is
     * not a number type.
     */
    public float AsSingle() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      if (cn == null) {
        throw new IllegalStateException("Not a number type");
      }
      return cn.AsSingle(this.getThisItem());
    }

    /**
     * Converts this object to an arbitrary-precision integer. Fractional
     * values are truncated to an integer.
     * @return The closest big integer to this object.
     * @throws java.lang.IllegalStateException This object's type is
     * not a number type.
     * @throws ArithmeticException This object's value is infinity or not-a-number
     * (NaN).
     */
    public BigInteger AsBigInteger() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      if (cn == null) {
        throw new IllegalStateException("Not a number type");
      }
      return cn.AsBigInteger(this.getThisItem());
    }

    /**
     * Returns false if this object is False, Null, or Undefined; otherwise,
     * true.
     * @return A Boolean object.
     */
    public boolean AsBoolean() {
      if (this.isFalse() || this.isNull() || this.isUndefined()) {
        return false;
      }
      return true;
    }

    /**
     * Converts this object to a 16-bit signed integer. Floating point values
     * are truncated to an integer.
     * @return The closest 16-bit signed integer to this object.
     * @throws java.lang.IllegalStateException This object's type is
     * not a number type.
     * @throws java.lang.ArithmeticException This object's value exceeds
     * the range of a 16-bit signed integer.
     */
    public short AsInt16() {
      return (short)this.AsInt32(Short.MIN_VALUE, Short.MAX_VALUE);
    }

    /**
     * Converts this object to a byte (0 to 255). Floating point values are
     * truncated to an integer.
     * @return The closest byte-sized integer to this object.
     * @throws java.lang.IllegalStateException This object's type is
     * not a number type.
     * @throws java.lang.ArithmeticException This object's value exceeds
     * the range of a byte (would be less than 0 or greater than 255 when truncated
     * to an integer).
     */
    public byte AsByte() {
      return (byte)this.AsInt32(0, 255);
    }

    /**
     * Converts this object to a 64-bit signed integer. Floating point values
     * are truncated to an integer.
     * @return The closest 64-bit signed integer to this object.
     * @throws java.lang.IllegalStateException This object's type is
     * not a number type.
     * @throws java.lang.ArithmeticException This object's value exceeds
     * the range of a 64-bit signed integer.
     */
    public long AsInt64() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      if (cn == null) {
        throw new IllegalStateException("Not a number type");
      }
      return cn.AsInt64(this.getThisItem());
    }

    /**
     * Returns whether this object's value can be converted to a 32-bit floating
     * point number without loss of its numerical value.
     * @return Whether this object's value can be converted to a 32-bit floating
     * point number without loss of its numerical value. Returns true if
     * this is a not-a-number value, even if the value's diagnostic information
     * can't fit in a 32-bit floating point number.
     */
    public boolean CanFitInSingle() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      if (cn == null) {
        return false;
      }
      return cn.CanFitInSingle(this.getThisItem());
    }

    /**
     * Returns whether this object's value can be converted to a 64-bit floating
     * point number without loss of its numerical value.
     * @return Whether this object's value can be converted to a 64-bit floating
     * point number without loss of its numerical value. Returns true if
     * this is a not-a-number value, even if the value's diagnostic information
     * can't fit in a 64-bit floating point number.
     */
    public boolean CanFitInDouble() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      if (cn == null) {
        return false;
      }
      return cn.CanFitInDouble(this.getThisItem());
    }

    /**
     * Returns whether this object's value is an integral value, is -(2^32)
     * or greater, and is less than 2^32.
     * @return True if this object's value is an integral value, is -(2^32)
     * or greater, and is less than 2^32; otherwise, false.
     */
    public boolean CanFitInInt32() {
      if (!this.CanFitInInt64()) {
        return false;
      }
      long v = this.AsInt64();
      return v >= Integer.MIN_VALUE && v <= Integer.MAX_VALUE;
    }

    /**
     * Returns whether this object's value is an integral value, is -(2^63)
     * or greater, and is less than 2^63.
     * @return True if this object's value is an integral value, is -(2^63)
     * or greater, and is less than 2^63; otherwise, false.
     */
    public boolean CanFitInInt64() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      if (cn == null) {
        return false;
      }
      return cn.CanFitInInt64(this.getThisItem());
    }

    /**
     * Returns whether this object's value, truncated to an integer, would
     * be -(2^63) or greater, and less than 2^63.
     * @return True if this object's value, truncated to an integer, would
     * be -(2^63) or greater, and less than 2^63; otherwise, false.
     */
    public boolean CanTruncatedIntFitInInt64() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      if (cn == null) {
        return false;
      }
      return cn.CanTruncatedIntFitInInt64(this.getThisItem());
    }

    /**
     * Returns whether this object's value, truncated to an integer, would
     * be -(2^31) or greater, and less than 2^31.
     * @return True if this object's value, truncated to an integer, would
     * be -(2^31) or greater, and less than 2^31; otherwise, false.
     */
    public boolean CanTruncatedIntFitInInt32() {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      if (cn == null) {
        return false;
      }
      return cn.CanTruncatedIntFitInInt32(this.getThisItem());
    }

    /**
     * Gets a value indicating whether this object represents an integral
     * number, that is, a number without a fractional part. Infinity and
     * not-a-number are not considered integral.
     * @return True if this object represents an integral number, that is,
     * a number without a fractional part; otherwise, false.
     */
    public boolean isIntegral() {
        ICBORNumber cn = NumberInterfaces[this.getItemType()];
        if (cn == null) {
          return false;
        }
        return cn.IsIntegral(this.getThisItem());
      }

    private int AsInt32(int minValue, int maxValue) {
      ICBORNumber cn = NumberInterfaces[this.getItemType()];
      if (cn == null) {
        throw new IllegalStateException("not a number type");
      }
      return cn.AsInt32(this.getThisItem(), minValue, maxValue);
    }

    /**
     * Converts this object to a 32-bit signed integer. Floating point values
     * are truncated to an integer.
     * @return The closest big integer to this object.
     * @throws java.lang.IllegalStateException This object's type is
     * not a number type.
     * @throws java.lang.ArithmeticException This object's value exceeds
     * the range of a 32-bit signed integer.
     */
    public int AsInt32() {
      return this.AsInt32(Integer.MIN_VALUE, Integer.MAX_VALUE);
    }

    /**
     * Gets the value of this object as a string object.
     * @return Gets this object's string.
     * @throws IllegalStateException This object's type is not a string.
     */
    public String AsString() {
      int type = this.getItemType();
      switch (type) {
        case CBORObjectTypeTextString: {
            return (String)this.getThisItem();
          }
        default:
          throw new IllegalStateException("Not a String type");
      }
    }

    /**
     * Reads an object in CBOR format from a data stream.
     * @param stream A readable data stream.
     * @return A CBOR object that was read.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     * @throws CBORException There was an error in reading or parsing the
     * data.
     */
    public static CBORObject Read(InputStream stream) {
      try {
        return new CBORReader(stream).Read(null);
      } catch (IOException ex) {
        throw new CBORException("I/O error occurred.", ex);
      }
    }

    private static void WriteObjectArray(
      List<CBORObject> list,
      OutputStream outputStream) throws IOException {
      WriteObjectArray(list, outputStream, null);
    }

    private static void WriteObjectMap(Map<CBORObject, CBORObject> map, OutputStream outputStream) throws IOException {
      WriteObjectMap(map, outputStream, null);
    }

    private static List<Object> PushObject(List<Object> stack, Object parent, Object child) {
      if (stack == null) {
        stack = new ArrayList<Object>();
        stack.add(parent);
      }
      for(Object o : stack) {
        if (o == child) {
          throw new IllegalArgumentException("Circular reference in data structure");
        }
      }
      stack.add(child);
      return stack;
    }

    private static List<Object> WriteChildObject(
      Object parentThisItem,
      CBORObject child,
      OutputStream outputStream,
      List<Object> stack) throws IOException {
      if (child == null) {
        outputStream.write(0xf6);
      } else {
        int type = child.getItemType();
        if (type == CBORObjectTypeArray) {
          stack = PushObject(stack, parentThisItem, child.getThisItem());
          child.WriteTags(outputStream);
          WriteObjectArray(child.AsList(), outputStream, stack);
          stack.remove(stack.size() - 1);
        } else if (type == CBORObjectTypeMap) {
          stack = PushObject(stack, parentThisItem, child.getThisItem());
          child.WriteTags(outputStream);
          WriteObjectMap(child.AsMap(), outputStream, stack);
          stack.remove(stack.size() - 1);
        } else {
          child.WriteTo(outputStream);
        }
      }
      return stack;
    }

    private static void WriteObjectArray(
      List<CBORObject> list,
      OutputStream outputStream,
      List<Object> stack) throws IOException {
      Object thisObj = list;
      WritePositiveInt(4, list.size(), outputStream);
      for(CBORObject i : list) {
        stack = WriteChildObject(thisObj, i, outputStream, stack);
      }
    }

    private static void WriteObjectMap(Map<CBORObject, CBORObject> map, OutputStream outputStream, List<Object> stack) throws IOException {
      Object thisObj = map;
      WritePositiveInt(5, map.size(), outputStream);
      for(Map.Entry<CBORObject, CBORObject> entry : map.entrySet()) {
        CBORObject key = entry.getKey();
        CBORObject value = entry.getValue();
        stack = WriteChildObject(thisObj, key, outputStream, stack);
        stack = WriteChildObject(thisObj, value, outputStream, stack);
      }
    }

    private static byte[] GetPositiveIntBytes(int type, int value) {
      if (value < 0) {
        throw new IllegalArgumentException("value (" + Long.toString((long)value) + ") is less than " + "0");
      }
      if (value < 24) {
        return new byte[] {  (byte)((byte)value | (byte)(type << 5))  };
      } else if (value <= 0xff) {
        return new byte[] {  (byte)(24 | (type << 5)),
          (byte)(value & 0xff)  };
      } else if (value <= 0xffff) {
        return new byte[] {  (byte)(25 | (type << 5)),
          (byte)((value >> 8) & 0xff),
          (byte)(value & 0xff)  };
      } else {
        return new byte[] {  (byte)(26 | (type << 5)),
          (byte)((value >> 24) & 0xff),
          (byte)((value >> 16) & 0xff),
          (byte)((value >> 8) & 0xff),
          (byte)(value & 0xff)  };
      }
    }

    private static void WritePositiveInt(int type, int value, OutputStream s) throws IOException {
      byte[] bytes = GetPositiveIntBytes(type, value);
      s.write(bytes,0,bytes.length);
    }

    private static byte[] GetPositiveInt64Bytes(int type, long value) {
      if (value < 0) {
        throw new IllegalArgumentException("value (" + Long.toString((long)value) + ") is less than " + "0");
      }
      if (value < 24) {
        return new byte[] {  (byte)((byte)value | (byte)(type << 5))  };
      } else if (value <= 0xFFL) {
        return new byte[] {  (byte)(24 | (type << 5)),
          (byte)(value & 0xff)  };
      } else if (value <= 0xFFFFL) {
        return new byte[] {  (byte)(25 | (type << 5)),
          (byte)((value >> 8) & 0xff),
          (byte)(value & 0xff)  };
      } else if (value <= 0xFFFFFFFFL) {
        return new byte[] {  (byte)(26 | (type << 5)),
          (byte)((value >> 24) & 0xff),
          (byte)((value >> 16) & 0xff),
          (byte)((value >> 8) & 0xff),
          (byte)(value & 0xff)  };
      } else {
        return new byte[] {  (byte)(27 | (type << 5)),
          (byte)((value >> 56) & 0xff),
          (byte)((value >> 48) & 0xff),
          (byte)((value >> 40) & 0xff),
          (byte)((value >> 32) & 0xff),
          (byte)((value >> 24) & 0xff),
          (byte)((value >> 16) & 0xff),
          (byte)((value >> 8) & 0xff),
          (byte)(value & 0xff)  };
      }
    }

    private static void WritePositiveInt64(int type, long value, OutputStream s) throws IOException {
      byte[] bytes = GetPositiveInt64Bytes(type, value);
      s.write(bytes,0,bytes.length);
    }

    private static final int StreamedStringBufferLength = 4096;

    private static void WriteStreamedString(String str, OutputStream stream) throws IOException {
      byte[] bytes;
      bytes = GetOptimizedBytesIfShortAscii(str, -1);
      if (bytes != null) {
        stream.write(bytes,0,bytes.length);
        return;
      }
      bytes = new byte[StreamedStringBufferLength];
      int byteIndex = 0;
      boolean streaming = false;
      for (int index = 0; index < str.length(); ++index) {
        int c = str.charAt(index);
        if (c <= 0x7f) {
          if (byteIndex >= StreamedStringBufferLength) {
            // Write bytes retrieved so far
            if (!streaming) {
              stream.write((byte)0x7f);
            }
            WritePositiveInt(3, byteIndex, stream);
            stream.write(bytes,0,byteIndex);
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
              stream.write((byte)0x7f);
            }
            WritePositiveInt(3, byteIndex, stream);
            stream.write(bytes,0,byteIndex);
            byteIndex = 0;
            streaming = true;
          }
          bytes[byteIndex++] = (byte)(0xc0 | ((c >> 6) & 0x1f));
          bytes[byteIndex++] = (byte)(0x80 | (c & 0x3f));
        } else {
          if ((c & 0xfc00) == 0xd800 && index + 1 < str.length() &&
              str.charAt(index + 1) >= 0xdc00 && str.charAt(index + 1) <= 0xdfff) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c - 0xd800) << 10) + (str.charAt(index + 1) - 0xdc00);
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
                stream.write((byte)0x7f);
              }
              WritePositiveInt(3, byteIndex, stream);
              stream.write(bytes,0,byteIndex);
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
                stream.write((byte)0x7f);
              }
              WritePositiveInt(3, byteIndex, stream);
              stream.write(bytes,0,byteIndex);
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
      stream.write(bytes,0,byteIndex);
      if (streaming) {
        stream.write((byte)0xff);
      }
    }

    /**
     * Writes a string in CBOR format to a data stream.
     * @param str The string to write. Can be null.
     * @param stream A writable data stream.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static void Write(String str, OutputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      if (str == null) {
        stream.write(0xf6);  // Write null instead of String
      } else {
        WriteStreamedString(str, stream);
      }
    }

    /**
     * Writes a binary floating-point number in CBOR format to a data stream
     * as follows: <ul> <li>If the value is null, writes the byte 0xF6.</li>
     * <li>If the value is negative zero, infinity, or NaN, converts the
     * number to a <code>double</code> and writes that <code>double</code> . If negative
     * zero should not be written this way, use the Plus method to convert
     * the value beforehand.</li> <li>If the value has an exponent of zero,
     * writes the value as an unsigned integer or signed integer if the number
     * can fit either type or as a big integer otherwise.</li> <li>In all
     * other cases, writes the value as a big float.</li> </ul>
     * @param bignum An ExtendedFloat object.
     * @param stream A writable data stream.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static void Write(ExtendedFloat bignum, OutputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      if (bignum == null) {
        stream.write(0xf6);
        return;
      }
      if ((bignum.signum()==0 && bignum.isNegative()) || bignum.IsInfinity() || bignum.IsNaN()) {
        Write(bignum.ToDouble(), stream);
        return;
      }
      BigInteger exponent = bignum.getExponent();
      if (exponent.signum()==0) {
        Write(bignum.getMantissa(), stream);
      } else {
        if (!BigIntFits(exponent)) {
          stream.write(0xd9);  // tag 265
          stream.write(0x01);
          stream.write(0x09);
          stream.write(0x82);  // array, length 2
        } else {
          stream.write(0xc5);  // tag 5
          stream.write(0x82);  // array, length 2
        }
        Write(bignum.getExponent(), stream);
        Write(bignum.getMantissa(), stream);
      }
    }

    /**
     * Writes a rational number in CBOR format to a data stream.
     * @param rational An ExtendedRational object.
     * @param stream A writable data stream.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static void Write(ExtendedRational rational, OutputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      if (rational == null) {
        stream.write(0xf6);
        return;
      }
      if (!rational.isFinite()) {
        Write(rational.ToDouble(), stream);
        return;
      }
      if (rational.getDenominator().equals(BigInteger.ONE)) {
        Write(rational.getNumerator(), stream);
        return;
      }
      stream.write(0xd8);  // tag 30
      stream.write(0x1e);
      stream.write(0x82);  // array, length 2
      Write(rational.getNumerator(), stream);
      Write(rational.getDenominator(), stream);
    }

    /**
     * Writes a decimal floating-point number in CBOR format to a data stream,
     * as follows: <ul> <li>If the value is null, writes the byte 0xF6.</li>
     * <li>If the value is negative zero, infinity, or NaN, converts the
     * number to a <code>double</code> and writes that <code>double</code> . If negative
     * zero should not be written this way, use the Plus method to convert
     * the value beforehand.</li> <li>If the value has an exponent of zero,
     * writes the value as an unsigned integer or signed integer if the number
     * can fit either type or as a big integer otherwise.</li> <li>In all
     * other cases, writes the value as a decimal number.</li> </ul>
     * @param bignum Decimal fraction to write. Can be null.
     * @param stream InputStream to write to.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static void Write(ExtendedDecimal bignum, OutputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      if (bignum == null) {
        stream.write(0xf6);
        return;
      }
      if ((bignum.signum()==0 && bignum.isNegative()) || bignum.IsInfinity() || bignum.IsNaN()) {
        Write(bignum.ToDouble(), stream);
        return;
      }
      BigInteger exponent = bignum.getExponent();
      if (exponent.signum()==0) {
        Write(bignum.getMantissa(), stream);
      } else {
        if (!BigIntFits(exponent)) {
          stream.write(0xd9);  // tag 264
          stream.write(0x01);
          stream.write(0x08);
          stream.write(0x82);  // array, length 2
        } else {
          stream.write(0xc4);  // tag 4
          stream.write(0x82);  // array, length 2
        }
        Write(bignum.getExponent(), stream);
        Write(bignum.getMantissa(), stream);
      }
    }

    /**
     * Writes a big integer in CBOR format to a data stream.
     * @param bigint Big integer to write. Can be null.
     * @param stream A writable data stream.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static void Write(BigInteger bigint, OutputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      if ((Object)bigint == (Object)null) {
        stream.write(0xf6);
        return;
      }
      int datatype = 0;
      if (bigint.signum() < 0) {
        datatype = 1;
        bigint=bigint.add(BigInteger.ONE);
        bigint=(bigint).negate();
      }
      if (bigint.compareTo(Int64MaxValue) <= 0) {
        // If the big integer is representable as a long and in
        // major type 0 or 1, write that major type
        // instead of as a bignum
        long ui = bigint.longValue();
        WritePositiveInt64(datatype, ui, stream);
      } else {
        // Get a byte array of the big integer's value,
        // since shifting and doing AND operations is
        // slow with large BigIntegers
        byte[] bytes = bigint.toByteArray(true);
        int byteCount = bytes.length;
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
            stream.write((byte)(datatype << 5));
            return;
          case 1:
            WritePositiveInt(datatype, ((int)bytes[0]) & 0xff, stream);
            break;
          case 2:
            stream.write((byte)((datatype << 5) | 25));
            stream.write(bytes,0,byteCount);
            break;
          case 3:
            stream.write((byte)((datatype << 5) | 26));
            stream.write((byte)0);
            stream.write(bytes,0,byteCount);
            break;
          case 4:
            stream.write((byte)((datatype << 5) | 26));
            stream.write(bytes,0,byteCount);
            break;
          case 5:
            stream.write((byte)((datatype << 5) | 27));
            stream.write((byte)0);
            stream.write((byte)0);
            stream.write((byte)0);
            stream.write(bytes,0,byteCount);
            break;
          case 6:
            stream.write((byte)((datatype << 5) | 27));
            stream.write((byte)0);
            stream.write((byte)0);
            stream.write(bytes,0,byteCount);
            break;
          case 7:
            stream.write((byte)((datatype << 5) | 27));
            stream.write((byte)0);
            stream.write(bytes,0,byteCount);
            break;
          case 8:
            stream.write((byte)((datatype << 5) | 27));
            stream.write(bytes,0,byteCount);
            break;
          default:
            stream.write((datatype == 0) ?
                             (byte)0xc2 :
                             (byte)0xc3);
            WritePositiveInt(2, byteCount, stream);
            stream.write(bytes,0,byteCount);
            break;
        }
      }
    }

    /**
     * Writes this CBOR object to a data stream.
     * @param stream A writable data stream.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public void WriteTo(OutputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      this.WriteTags(stream);
      int type = this.getItemType();
      if (type == CBORObjectTypeInteger) {
        Write((((Long)this.getThisItem()).longValue()), stream);
      } else if (type == CBORObjectTypeBigInteger) {
        Write((BigInteger)this.getThisItem(), stream);
      } else if (type == CBORObjectTypeByteString) {
        byte[] arr = (byte[])this.getThisItem();
        WritePositiveInt(
          (this.getItemType() == CBORObjectTypeByteString) ? 2 : 3,
          arr.length,
          stream);
        stream.write(arr,0,arr.length);
      } else if (type == CBORObjectTypeTextString) {
        Write((String)this.getThisItem(), stream);
      } else if (type == CBORObjectTypeArray) {
        WriteObjectArray(this.AsList(), stream);
      } else if (type == CBORObjectTypeExtendedDecimal) {
        ExtendedDecimal dec = (ExtendedDecimal)this.getThisItem();
        Write(dec, stream);
      } else if (type == CBORObjectTypeExtendedFloat) {
        ExtendedFloat flo = (ExtendedFloat)this.getThisItem();
        Write(flo, stream);
      } else if (type == CBORObjectTypeExtendedRational) {
        ExtendedRational flo = (ExtendedRational)this.getThisItem();
        Write(flo, stream);
      } else if (type == CBORObjectTypeMap) {
        WriteObjectMap(this.AsMap(), stream);
      } else if (type == CBORObjectTypeSimpleValue) {
        int value = ((Integer)this.getThisItem()).intValue();
        if (value < 24) {
          stream.write((byte)(0xe0 + value));
        } else {

          stream.write(0xf8);
          stream.write((byte)value);
        }
      } else if (type == CBORObjectTypeSingle) {
        Write(((Float)this.getThisItem()).floatValue(), stream);
      } else if (type == CBORObjectTypeDouble) {
        Write(((Double)this.getThisItem()).doubleValue(), stream);
      } else {
        throw new IllegalArgumentException("Unexpected data type");
      }
    }

    /**
     * Writes a 64-bit signed integer in CBOR format to a data stream.
     * @param value The value to write.
     * @param stream A writable data stream.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static void Write(long value, OutputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      if (value >= 0) {
        WritePositiveInt64(0, value, stream);
      } else {
        ++value;
        value = -value;  // Will never overflow
        WritePositiveInt64(1, value, stream);
      }
    }

    /**
     * Writes a 32-bit signed integer in CBOR format to a data stream.
     * @param value The value to write.
     * @param stream A writable data stream.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static void Write(int value, OutputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      int type = 0;
      if (value < 0) {
        ++value;
        value = -value;
        type = 0x20;
      }
      if (value < 24) {
        stream.write((byte)(value | type));
      } else if (value <= 0xff) {
        byte[] bytes = new byte[] {  (byte)(24 | type), (byte)(value & 0xff)  };
        stream.write(bytes,0,2);
      } else if (value <= 0xffff) {
        byte[] bytes = new byte[] {  (byte)(25 | type), (byte)((value >> 8) & 0xff), (byte)(value & 0xff)  };
        stream.write(bytes,0,3);
      } else {
        byte[] bytes = new byte[] {  (byte)(26 | type), (byte)((value >> 24) & 0xff), (byte)((value >> 16) & 0xff), (byte)((value >> 8) & 0xff), (byte)(value & 0xff)  };
        stream.write(bytes,0,5);
      }
    }

    /**
     * Writes a 16-bit signed integer in CBOR format to a data stream.
     * @param value The value to write.
     * @param stream A writable data stream.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static void Write(short value, OutputStream stream) throws IOException {
      Write((long)value, stream);
    }

    /**
     * Writes a Unicode character as a string in CBOR format to a data stream.
     * @param value The value to write.
     * @param stream A writable data stream.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     * @throws java.lang.IllegalArgumentException The parameter {@code value}
     * is a surrogate code point.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static void Write(char value, OutputStream stream) throws IOException {
      if (value >= 0xd800 && value < 0xe000) {
        throw new IllegalArgumentException("Value is a surrogate code point.");
      }
      Write(new String(new char[] { value }), stream);
    }

    /**
     * Writes a Boolean value in CBOR format to a data stream.
     * @param value The value to write.
     * @param stream A writable data stream.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static void Write(boolean value, OutputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      stream.write(value ? (byte)0xf5 : (byte)0xf4);
    }

    /**
     * Writes a byte (0 to 255) in CBOR format to a data stream. If the value
     * is less than 24, writes that byte. If the value is 25 to 255, writes the
     * byte 24, then this byte's value.
     * @param value The value to write.
     * @param stream A writable data stream.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static void Write(byte value, OutputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      if ((((int)value) & 0xff) < 24) {
        stream.write(value);
      } else {
        stream.write((byte)24);
        stream.write(value);
      }
    }

    /**
     * Writes a 32-bit floating-point number in CBOR format to a data stream.
     * @param value The value to write.
     * @param s A writable data stream.
     * @throws java.lang.NullPointerException The parameter {@code s}
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static void Write(float value, OutputStream s) throws IOException {
      if (s == null) {
        throw new NullPointerException("s");
      }
      int bits = Float.floatToRawIntBits(value);
      byte[] data = new byte[] {  (byte)0xfa,
        (byte)((bits >> 24) & 0xff),
        (byte)((bits >> 16) & 0xff),
        (byte)((bits >> 8) & 0xff),
        (byte)(bits & 0xff)  };
      s.write(data,0,5);
    }

    /**
     * Writes a 64-bit floating-point number in CBOR format to a data stream.
     * @param value The value to write.
     * @param stream A writable data stream.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     */
    public static void Write(double value, OutputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      long bits = Double.doubleToRawLongBits((double)value);
      byte[] data = new byte[] {  (byte)0xfb,
        (byte)((bits >> 56) & 0xff),
        (byte)((bits >> 48) & 0xff),
        (byte)((bits >> 40) & 0xff),
        (byte)((bits >> 32) & 0xff),
        (byte)((bits >> 24) & 0xff),
        (byte)((bits >> 16) & 0xff),
        (byte)((bits >> 8) & 0xff),
        (byte)(bits & 0xff)  };
      stream.write(data,0,9);
    }

    private static byte[] GetOptimizedBytesIfShortAscii(String str, int tagbyte) {
      byte[] bytes;
      if (str.length() <= 255) {
        // The strings will usually be short ASCII strings, so
        // use this optimization
        int offset = 0;
        int length = str.length();
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
          bytes[offset] = (byte)(0x60 + str.length());
          ++offset;
        } else {
          bytes[offset] = (byte)0x78;
          bytes[offset + 1] = (byte)str.length();
          offset += 2;
        }
        boolean issimple = true;
        for (int i = 0; i < str.length(); ++i) {
          char c = str.charAt(i);
          if (c >= 0x80) {
            issimple = false;
            break;
          }
          bytes[i + offset] = ((byte)c);
        }
        if (issimple) {
          return bytes;
        }
      }
      return null;
    }

    /**
     * Gets the binary representation of this data item.
     * @return A byte array in CBOR format.
     */
    public byte[] EncodeToBytes() {
      // For some types, a memory stream is a lot of
      // overhead since the amount of memory the types
      // use is fixed and small
      boolean hasComplexTag = false;
      byte tagbyte = 0;
      boolean tagged = this.isTagged();
      if (this.isTagged()) {
        CBORObject taggedItem = (CBORObject)this.itemValue;
        if (taggedItem.isTagged() ||
            this.tagHigh != 0 ||
            (this.tagLow >> 16) != 0 ||
            this.tagLow >= 24) {
          hasComplexTag = true;
        } else {
          tagbyte = (byte)(0xc0 + (int)this.tagLow);
        }
      }
      if (!hasComplexTag) {
        if (this.getItemType() == CBORObjectTypeTextString) {
          byte[] ret = GetOptimizedBytesIfShortAscii(
            this.AsString(),
            tagged ? (((int)tagbyte) & 0xff) : -1);
          if (ret != null) {
            return ret;
          }
        } else if (this.getItemType() == CBORObjectTypeSimpleValue) {
          if (tagged) {
            if (this.isFalse()) {
              return new byte[] {  tagbyte, (byte)0xf4  };
            }
            if (this.isTrue()) {
              return new byte[] {  tagbyte, (byte)0xf5  };
            }
            if (this.isNull()) {
              return new byte[] {  tagbyte, (byte)0xf6  };
            }
            if (this.isUndefined()) {
              return new byte[] {  tagbyte, (byte)0xf7  };
            }
          } else {
            if (this.isFalse()) {
              return new byte[] {  (byte)0xf4  };
            }
            if (this.isTrue()) {
              return new byte[] {  (byte)0xf5  };
            }
            if (this.isNull()) {
              return new byte[] {  (byte)0xf6  };
            }
            if (this.isUndefined()) {
              return new byte[] {  (byte)0xf7  };
            }
          }
        } else if (this.getItemType() == CBORObjectTypeInteger) {
          long value = (((Long)this.getThisItem()).longValue());
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
          byte[] ret2 = new byte[intBytes.length + 1];
          System.arraycopy(intBytes, 0, ret2, 1, intBytes.length);
          ret2[0] = tagbyte;
          return ret2;
        } else if (this.getItemType() == CBORObjectTypeSingle) {
          float value = ((Float)this.getThisItem()).floatValue();
          int bits = Float.floatToRawIntBits(value);
          return tagged ?
            new byte[] {  tagbyte, (byte)0xfa,
            (byte)((bits >> 24) & 0xff),
            (byte)((bits >> 16) & 0xff),
            (byte)((bits >> 8) & 0xff),
            (byte)(bits & 0xff)  } :
            new byte[] {  (byte)0xfa,
            (byte)((bits >> 24) & 0xff),
            (byte)((bits >> 16) & 0xff),
            (byte)((bits >> 8) & 0xff),
            (byte)(bits & 0xff)  };
        } else if (this.getItemType() == CBORObjectTypeDouble) {
          double value = ((Double)this.getThisItem()).doubleValue();
          long bits = Double.doubleToRawLongBits(value);
          return tagged ?
            new byte[] {  tagbyte, (byte)0xfb,
            (byte)((bits >> 56) & 0xff),
            (byte)((bits >> 48) & 0xff),
            (byte)((bits >> 40) & 0xff),
            (byte)((bits >> 32) & 0xff),
            (byte)((bits >> 24) & 0xff),
            (byte)((bits >> 16) & 0xff),
            (byte)((bits >> 8) & 0xff),
            (byte)(bits & 0xff)  } :
            new byte[] {  (byte)0xfb,
            (byte)((bits >> 56) & 0xff),
            (byte)((bits >> 48) & 0xff),
            (byte)((bits >> 40) & 0xff),
            (byte)((bits >> 32) & 0xff),
            (byte)((bits >> 24) & 0xff),
            (byte)((bits >> 16) & 0xff),
            (byte)((bits >> 8) & 0xff),
            (byte)(bits & 0xff)  };
        }
      }
      try {
        java.io.ByteArrayOutputStream ms=null;
try {
ms=new java.io.ByteArrayOutputStream(16);

          this.WriteTo(ms);
          return ms.toByteArray();
}
finally {
try { if(ms!=null)ms.close(); } catch (java.io.IOException ex){}
}
      } catch (IOException ex) {
        throw new CBORException("I/O Error occurred", ex);
      }
    }

    /**
     * Writes a CBOR object to a CBOR data stream.
     * @param value The value to write. Can be null.
     * @param stream A writable data stream.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     */
    public static void Write(CBORObject value, OutputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      if (value == null) {
        stream.write(0xf6);
      } else {
        value.WriteTo(stream);
      }
    }

    /**
     * Writes an arbitrary object to a CBOR data stream. Currently, the following
     * objects are supported: <ul> <li>Lists of CBORObject.</li> <li>Maps
     * of CBORObject.</li> <li>Null.</li> <li>Any object accepted by
     * the FromObject static methods.</li> </ul>
     * @param objValue The value to write.
     * @param stream A writable data stream.
     * @throws java.lang.IllegalArgumentException The object's type is not supported.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     */
    @SuppressWarnings("unchecked")
public static void Write(Object objValue, OutputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      if (objValue == null) {
        stream.write(0xf6);
        return;
      }
      byte[] data = ((objValue instanceof byte[]) ? (byte[])objValue : null);
      if (data != null) {
        WritePositiveInt(3, data.length, stream);
        stream.write(data,0,data.length);
        return;
      } else if(objValue instanceof List<?>) {
        WriteObjectArray((List<CBORObject>)objValue, stream);
      } else if(objValue instanceof Map<?,?>) {
        WriteObjectMap((Map<CBORObject, CBORObject>)objValue, stream);
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
      boolean allowBOM = true;
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

    private static String NextJSONString(CharacterReader reader, int quote) {
      int c;
      StringBuilder sb = null;
      boolean surrogate = false;
      boolean surrogateEscaped = false;
      boolean escaped = false;
      while (true) {
        c = reader.NextChar();
        if (c == -1 || c < 0x20) {
          throw reader.NewError("Unterminated String");
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
            // No String builder created yet, so this
            // is an empty String
            return "";
          }
          return sb.toString();
        }
        if (sb == null) {
          sb = new StringBuilder();
        }
        if (c <= 0xffff) {
          sb.append((char)c);
        } else if (c <= 0x10ffff) {
          sb.append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
          sb.append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
        }
      }
    }

    private static CBORObject NextJSONValue(CharacterReader reader, int firstChar, boolean noDuplicates, int[] nextChar, int depth) {
      String str;
      int c = firstChar;
      CBORObject obj = null;
      if (c < 0) {
        throw reader.NewError("Unexpected end of data");
      }
      if (c == '"') {
        // Parse a String
        // The tokenizer already checked the String for invalid
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
          sb.append((char)c);
          c = reader.NextChar();
        }
        str = sb.toString();
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
      boolean noDuplicates,
      boolean skipByteOrderMark,
      boolean objectOrArrayOnly,
      int depth) {
      if (depth > 1000) {
        throw reader.NewError("Too deeply nested");
      }
      int c;
      c = skipByteOrderMark ?
        SkipWhitespaceOrByteOrderMarkJSON(reader) :
        SkipWhitespaceJSON(reader);
      if (!skipByteOrderMark && c == (char)0xfeff) {
        throw reader.NewError("JSON Object began with a byte order mark (U+FEFF)");
      }
      if (c == '[') {
        return ParseJSONArray(reader, noDuplicates, depth);
      }
      if (c == '{') {
        return ParseJSONObject(reader, noDuplicates, depth);
      }
      if (objectOrArrayOnly) {
        throw reader.NewError("A JSON Object must begin with '{' or '['");
      }
      int[] nextChar = new int[1];
      return NextJSONValue(reader, c, noDuplicates, nextChar, depth);
    }

    private static CBORObject ParseJSONObject(CharacterReader reader, boolean noDuplicates, int depth) {
      // Assumes that the last character read was '{'
      if (depth > 1000) {
        throw reader.NewError("Too deeply nested");
      }
      int c;
      CBORObject key;
      CBORObject obj;
      int[] nextchar = new int[1];
      boolean seenComma = false;
      HashMap<CBORObject, CBORObject> myHashMap=new HashMap<CBORObject, CBORObject>();
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
              // Read the next String
              if (c < 0) {
                throw reader.NewError("Unexpected end of data");
              }
              if (c != '"') {
                throw reader.NewError("Expected a String as a key");
              }
              // Parse a String that represents the Object's key
              // The tokenizer already checked the String for invalid
              // surrogate pairs, so just call the CBORObject
              // constructor directly
              obj = CBORObject.FromRaw(NextJSONString(reader, c));
              key = obj;
              if (noDuplicates && myHashMap.containsKey(obj)) {
                throw reader.NewError("Key already exists: " + key);
              }
              break;
            }
        }
        if (SkipWhitespaceJSON(reader) != ':') {
          throw reader.NewError("Expected a ':' after a key");
        }
        // NOTE: Will overwrite existing value
        myHashMap.put(key,NextJSONValue(reader, SkipWhitespaceJSON(reader), noDuplicates, nextchar, depth));
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

    private static CBORObject ParseJSONArray(CharacterReader reader, boolean noDuplicates, int depth) {
      // Assumes that the last character read was '['
      if (depth > 1000) {
        throw reader.NewError("Too deeply nested");
      }
      ArrayList<CBORObject> myArrayList=new ArrayList<CBORObject>();
      boolean seenComma = false;
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
          myArrayList.add(NextJSONValue(reader, c, noDuplicates, nextchar, depth));
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

    /**
     * Generates a CBOR object from a string in JavaScript Object Notation
     * (JSON) format. <p>If a JSON object has the same key, only the last given
     * value will be used for each duplicated key. The JSON string may not
     * begin with a byte order mark (U + FEFF).</p>
     * @param str A string in JSON format.
     * @return A CBORObject object.
     * @throws java.lang.NullPointerException The parameter {@code str}
     * is null.
     * @throws CBORException The string is not in JSON format.
     */
    public static CBORObject FromJSONString(String str) {
      CharacterReader reader = new CharacterReader(str);
      CBORObject obj = ParseJSONValue(reader, false, false, false, 0);
      if (SkipWhitespaceJSON(reader) != -1) {
        throw reader.NewError("End of String not reached");
      }
      return obj;
    }

    /**
     * Generates a CBOR object from a data stream in JavaScript Object Notation
     * (JSON) format and UTF-8 encoding. The JSON stream may begin with a
     * byte order mark (U + FEFF). <p>If a JSON object has the same key, only
     * the last given value will be used for each duplicated key.</p>
     * @param stream A readable data stream.
     * @return A CBORObject object.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     * @throws java.io.IOException An I/O error occurred.
     * @throws CBORException The data stream contains invalid UTF-8 or
     * is not in JSON format.
     */
    public static CBORObject ReadJSON(InputStream stream) throws IOException {
      CharacterReader reader = new CharacterReader(stream);
      try {
        CBORObject obj = ParseJSONValue(reader, false, true, false, 0);
        if (SkipWhitespaceJSON(reader) != -1) {
          throw reader.NewError("End of data stream not reached");
        }
        return obj;
      } catch (CBORException ex) {
        if (ex.getCause() != null && ex.getCause() instanceof IOException) {
          throw (IOException)ex.getCause();
        }
        throw ex;
      }
    }

    private static final String Hex16 = "0123456789ABCDEF";

    private static void StringToJSONStringUnquoted(String str, StringBuilder sb) {
      // Surrogates were already verified when this
      // String was added to the CBOR Object; that check
      // is not repeated here
      boolean first = true;
      for (int i = 0; i < str.length(); ++i) {
        char c = str.charAt(i);
        if (c == '\\' || c == '"') {
          if (first) {
            first = false;
            sb.append(str,0,(0)+(i));
          }
          sb.append('\\');
          sb.append(c);
        } else if (c < 0x20) {
          if (first) {
            first = false;
            sb.append(str,0,(0)+(i));
          }
          if (c == 0x0d) {
            sb.append("\\r");
          } else if (c == 0x0a) {
            sb.append("\\n");
          } else if (c == 0x08) {
            sb.append("\\b");
          } else if (c == 0x0c) {
            sb.append("\\f");
          } else if (c == 0x09) {
            sb.append("\\t");
          } else {
            sb.append("\\u00");
            sb.append(Hex16.charAt((int)(c >> 4)));
            sb.append(Hex16.charAt((int)(c & 15)));
          }
        } else if (!first) {
          sb.append(c);
        }
      }
      if (first) {
        sb.append(str);
      }
    }

    /**
     * Converts this object to a string in JavaScript Object Notation (JSON)
     * format. This function works not only with arrays and maps, but also
     * integers, strings, byte arrays, and other JSON data types. Notes:
     * <ul><li> If this object contains maps with non-string keys, the keys
     * are converted to JSON strings before writing the map as a JSON string.
     * </li> <li>If a number in the form of a big float has a very high binary
     * exponent, it will be converted to a double before being converted
     * to a JSON string. (The resulting double could overflow to infinity,
     * in which case the big float is converted to null.)</li> <li>The string
     * will not begin with a byte-order mark (U + FEFF); RFC 7159 (the JSON
     * specification) forbids placing a byte-order mark at the beginning
     * of a JSON string.</li> <li>Byte strings are converted to Base64 URL
     * by default.</li> <li>Rational numbers will be converted to their
     * exact form, if possible, otherwise to a high-precision approximation.
     * (The resulting approximation could overflow to infinity, in which
     * case the rational number is converted to null.)</li> <li>Simple
     * values other than true and false will be converted to null. (This doesn't
     * include floating-point numbers.)</li> <li>Infinity and not-a-number
     * will be converted to null.</li> </ul>
     * @return A string object containing the converted object.
     */
    public String ToJSONString() {
      int type = this.getItemType();
      switch (type) {
        case CBORObjectTypeSimpleValue: {
            if (this.isTrue()) {
              {
                return "true";
              }
            } else if (this.isFalse()) {
              {
                return "false";
              }
            } else if (this.isNull()) {
              {
                return "null";
              }
            } else {
              return "null";
            }
          }
        case CBORObjectTypeSingle: {
            float f = ((Float)this.getThisItem()).floatValue();
            if (((f)==Float.NEGATIVE_INFINITY) ||
                ((f)==Float.POSITIVE_INFINITY) ||
                Float.isNaN(f)) {
              return "null";
            } else {
              return TrimDotZero(
                Float.toString((float)f));
            }
          }
        case CBORObjectTypeDouble: {
            double f = ((Double)this.getThisItem()).doubleValue();
            if (((f)==Double.NEGATIVE_INFINITY) ||
                ((f)==Double.POSITIVE_INFINITY) ||
                Double.isNaN(f)) {
              return "null";
            } else {
              return TrimDotZero(
                Double.toString((double)f));
            }
          }
        case CBORObjectTypeInteger: {
            return Long.toString((((Long)this.getThisItem()).longValue()));
          }
        case CBORObjectTypeBigInteger: {
            return CBORUtilities.BigIntToString((BigInteger)this.getThisItem());
          }
        case CBORObjectTypeExtendedRational: {
            ExtendedRational dec = (ExtendedRational)this.getThisItem();
            ExtendedDecimal f = dec.ToExtendedDecimalExactIfPossible(
              PrecisionContext.Decimal128.WithUnlimitedExponents());
            if (!f.isFinite()) {
              return "null";
            } else {
              return f.toString();
            }
          }
        case CBORObjectTypeExtendedDecimal: {
            ExtendedDecimal dec = (ExtendedDecimal)this.getThisItem();
            if (dec.IsInfinity() || dec.IsNaN()) {
              return "null";
            }
            return dec.toString();
          }
        case CBORObjectTypeExtendedFloat: {
            ExtendedFloat flo = (ExtendedFloat)this.getThisItem();
            if (flo.IsInfinity() || flo.IsNaN()) {
              return "null";
            }
            if (flo.isFinite() &&
              (flo.getExponent()).abs().compareTo(BigInteger.valueOf(2500)) > 0) {
              // Too inefficient to convert to a decimal number
              // from a bigfloat with a very high exponent,
              // so convert to double instead
              double f = flo.ToDouble();
              if (((f)==Double.NEGATIVE_INFINITY) ||
                  ((f)==Double.POSITIVE_INFINITY) ||
                  Double.isNaN(f)) {
                return "null";
              } else {
                return TrimDotZero(
                  Double.toString((double)f));
              }
            }
            return flo.toString();
          }
        default: {
            StringBuilder sb = new StringBuilder();
            this.ToJSONStringInternal(sb);
            return sb.toString();
          }
      }
    }

    private void ToJSONStringInternal(StringBuilder sb) {
      int type = this.getItemType();
      switch (type) {
        case CBORObjectTypeByteString: {
            sb.append('\"');
            if (this.HasTag(22)) {
              Base64.ToBase64(sb, (byte[])this.getThisItem(), false);
            } else if (this.HasTag(23)) {
              CBORUtilities.ToBase16(sb, (byte[])this.getThisItem());
            } else {
              Base64.ToBase64URL(sb, (byte[])this.getThisItem(), false);
            }
            sb.append('\"');
            break;
          }
        case CBORObjectTypeTextString: {
            sb.append('\"');
            StringToJSONStringUnquoted((String)this.getThisItem(), sb);
            sb.append('\"');
            break;
          }
        case CBORObjectTypeArray: {
            boolean first = true;
            sb.append('[');
            for(CBORObject i : this.AsList()) {
              if (!first) {
                sb.append(',');
              }
              i.ToJSONStringInternal(sb);
              first = false;
            }
            sb.append(']');
            break;
          }
        case CBORObjectTypeExtendedRational: {
            ExtendedRational dec = (ExtendedRational)this.getThisItem();
            ExtendedDecimal f = dec.ToExtendedDecimalExactIfPossible(
              PrecisionContext.Decimal128.WithUnlimitedExponents());
            if (!f.isFinite()) {
              sb.append("null");
            } else {
              sb.append(f.toString());
            }
            break;
          }
        case CBORObjectTypeMap: {
            boolean first = true;
            boolean hasNonStringKeys = false;
            Map<CBORObject, CBORObject> objMap = this.AsMap();
            sb.append('{');
            int oldLength = sb.length();
            for(Map.Entry<CBORObject, CBORObject> entry : objMap.entrySet()) {
              CBORObject key = entry.getKey();
              CBORObject value = entry.getValue();
              if (key.getItemType() != CBORObjectTypeTextString) {
                hasNonStringKeys = true;
                break;
              }
              if (!first) {
                sb.append(',');
              }
              sb.append('\"');
              StringToJSONStringUnquoted((String)key.getThisItem(), sb);
              sb.append('\"');
              sb.append(':');
              value.ToJSONStringInternal(sb);
              first = false;
            }
            if (hasNonStringKeys) {
              sb.delete(oldLength,(oldLength)+(sb.length() - oldLength));
              Map<String, CBORObject> stringMap = new HashMap<String, CBORObject>();
              // Copy to a map with String keys, since
              // some keys could be duplicates
              // when serialized to strings
              for(Map.Entry<CBORObject, CBORObject> entry : objMap.entrySet()) {
                CBORObject key = entry.getKey();
                CBORObject value = entry.getValue();
                String str = (key.getItemType() == CBORObjectTypeTextString) ?
                  ((String)key.getThisItem()) : key.ToJSONString();
                stringMap.put(str,value);
              }
              first = true;
              for(Map.Entry<String, CBORObject> entry : stringMap.entrySet()) {
                String key = entry.getKey();
                CBORObject value = entry.getValue();
                if (!first) {
                  sb.append(',');
                }
                sb.append('\"');
                StringToJSONStringUnquoted(key, sb);
                sb.append('\"');
                sb.append(':');
                value.ToJSONStringInternal(sb);
                first = false;
              }
            }
            sb.append('}');
            break;
          }
        default:
          sb.append(this.ToJSONString());
          break;
      }
    }

    private static CBORObject FromRaw(String str) {
      return new CBORObject(CBORObjectTypeTextString, str);
    }

    private static CBORObject FromRaw(List<CBORObject> list) {
      return new CBORObject(CBORObjectTypeArray, list);
    }

    private static CBORObject FromRaw(Map<CBORObject, CBORObject> map) {
      return new CBORObject(CBORObjectTypeMap, map);
    }

    /**
     * Finds the sum of two CBOR number objects.
     * @param first A CBORObject object. (2).
     * @param second A CBORObject object. (3).
     * @return A CBORObject object.
     * @throws java.lang.IllegalArgumentException Either or both operands are not
     * numbers (as opposed to Not-a-Number, NaN).
     */
    public static CBORObject Addition(CBORObject first, CBORObject second) {
      return CBORObjectMath.Addition(first, second);
    }

    /**
     * Finds the difference between two CBOR number objects.
     * @param first A CBORObject object.
     * @param second A CBORObject object. (2).
     * @return The difference of the two objects.
     * @throws java.lang.IllegalArgumentException Either or both operands are not
     * numbers (as opposed to Not-a-Number, NaN).
     */
    public static CBORObject Subtract(CBORObject first, CBORObject second) {
      return CBORObjectMath.Subtract(first, second);
    }

    /**
     * Multiplies two CBOR number objects.
     * @param first A CBORObject object.
     * @param second A CBORObject object. (2).
     * @return The product of the two objects.
     * @throws java.lang.IllegalArgumentException Either or both operands are not
     * numbers (as opposed to Not-a-Number, NaN).
     */
    public static CBORObject Multiply(CBORObject first, CBORObject second) {
      return CBORObjectMath.Multiply(first, second);
    }

    /**
     * Divides a CBORObject object by the value of a CBORObject object.
     * @param first A CBORObject object.
     * @param second A CBORObject object. (2).
     * @return The quotient of the two objects.
     */
    public static CBORObject Divide(CBORObject first, CBORObject second) {
      return CBORObjectMath.Divide(first, second);
    }

    /**
     * Finds the remainder that results when a CBORObject object is divided
     * by the value of a CBORObject object.
     * @param first A CBORObject object.
     * @param second A CBORObject object. (2).
     * @return The remainder of the two objects.
     */
    public static CBORObject Remainder(CBORObject first, CBORObject second) {
      return CBORObjectMath.Remainder(first, second);
    }

    /**
     * Creates a new empty CBOR array.
     * @return A new CBOR array.
     */
    public static CBORObject NewArray() {
      return new CBORObject(CBORObjectTypeArray, new ArrayList<CBORObject>());
    }

    /**
     * Creates a new empty CBOR map.
     * @return A new CBOR map.
     */
    public static CBORObject NewMap() {
      return FromObject(new HashMap<CBORObject, CBORObject>());
    }

    /**
     * Creates a CBOR object from a simple value number.
     * @param simpleValue A 32-bit signed integer.
     * @return A CBORObject object.
     * @throws java.lang.IllegalArgumentException The parameter {@code simpleValue}
     * is less than 0, greater than 255, or from 24 through 31.
     */
    public static CBORObject FromSimpleValue(int simpleValue) {
      if (simpleValue < 0) {
        throw new IllegalArgumentException("simpleValue (" + Long.toString((long)simpleValue) + ") is less than " + "0");
      }
      if (simpleValue > 255) {
        throw new IllegalArgumentException("simpleValue (" + Long.toString((long)simpleValue) + ") is more than " + "255");
      }
      if (simpleValue >= 24 && simpleValue < 32) {
        throw new IllegalArgumentException("Simple value is from 24 to 31: " + simpleValue);
      }
      if (simpleValue < 32) {
        return valueFixedObjects[0xe0 + simpleValue];
      } else {
        return new CBORObject(
          CBORObjectTypeSimpleValue,
          simpleValue);
      }
    }

    /**
     * Generates a CBOR object from a 64-bit signed integer.
     * @param value A 64-bit signed integer.
     * @return A CBORObject object.
     */
    public static CBORObject FromObject(long value) {
      return new CBORObject(CBORObjectTypeInteger, value);
    }

    /**
     * Generates a CBOR object from a CBOR object.
     * @param value A CBOR object.
     * @return Same as.
     */
    public static CBORObject FromObject(CBORObject value) {
      if (value == null) {
        return CBORObject.Null;
      }
      return value;
    }

    /**
     * Generates a CBOR object from an arbitrary-precision integer.
     * @param bigintValue An arbitrary-precision value.
     * @return A CBOR number object.
     */
    public static CBORObject FromObject(BigInteger bigintValue) {
      if ((Object)bigintValue == (Object)null) {
        return CBORObject.Null;
      }
      if (bigintValue.compareTo(Int64MinValue) >= 0 &&
          bigintValue.compareTo(Int64MaxValue) <= 0) {
        return new CBORObject(CBORObjectTypeInteger, bigintValue.longValue());
      } else {
        return new CBORObject(CBORObjectTypeBigInteger, bigintValue);
      }
    }

    /**
     * Generates a CBOR object from an arbitrary-precision binary floating-point
     * number.
     * @param bigValue An arbitrary-precision binary floating-point
     * number.
     * @return A CBOR number object.
     */
    public static CBORObject FromObject(ExtendedFloat bigValue) {
      if ((Object)bigValue == (Object)null) {
        return CBORObject.Null;
      }
      if (bigValue.IsNaN() || bigValue.IsInfinity()) {
        return new CBORObject(CBORObjectTypeExtendedFloat, bigValue);
      }
      BigInteger bigintExponent = bigValue.getExponent();
      if (bigintExponent.signum()==0 && !(bigValue.signum()==0 && bigValue.isNegative())) {
        return FromObject(bigValue.getMantissa());
      } else {
        return new CBORObject(CBORObjectTypeExtendedFloat, bigValue);
      }
    }

    /**
     * Generates a CBOR object from a rational number.
     * @param bigValue A rational number.
     * @return A CBOR number object.
     */
    public static CBORObject FromObject(ExtendedRational bigValue) {
      if ((Object)bigValue == (Object)null) {
        return CBORObject.Null;
      }
      if (bigValue.isFinite() && bigValue.getDenominator().equals(BigInteger.ONE)) {
        return FromObject(bigValue.getNumerator());
      }
      return new CBORObject(CBORObjectTypeExtendedRational, bigValue);
    }

    /**
     * Generates a CBOR object from a decimal number.
     * @param otherValue An arbitrary-precision decimal number.
     * @return A CBOR number object.
     */
    public static CBORObject FromObject(ExtendedDecimal otherValue) {
      if ((Object)otherValue == (Object)null) {
        return CBORObject.Null;
      }
      if (otherValue.IsNaN() || otherValue.IsInfinity()) {
        return new CBORObject(CBORObjectTypeExtendedDecimal, otherValue);
      }
      BigInteger bigintExponent = otherValue.getExponent();
      if (bigintExponent.signum()==0 && !(otherValue.signum()==0 && otherValue.isNegative())) {
        return FromObject(otherValue.getMantissa());
      } else {
        return new CBORObject(CBORObjectTypeExtendedDecimal, otherValue);
      }
    }

    /**
     * Generates a CBOR object from a string.
     * @param strValue A string value. Can be null.
     * @return A CBOR object representing the string, or CBORObject.Null
     * if stringValue is null.
     * @throws java.lang.IllegalArgumentException The string contains an unpaired
     * surrogate code point.
     */
    public static CBORObject FromObject(String strValue) {
      if (strValue == null) {
        return CBORObject.Null;
      }
      if (DataUtilities.GetUtf8Length(strValue, false) < 0) {
        throw new IllegalArgumentException("String contains an unpaired surrogate code point.");
      }
      return new CBORObject(CBORObjectTypeTextString, strValue);
    }

    /**
     * Generates a CBOR object from a 32-bit signed integer.
     * @param value A 32-bit signed integer.
     * @return A CBORObject object.
     */
    public static CBORObject FromObject(int value) {
      return FromObject((long)value);
    }

    /**
     * Generates a CBOR object from a 16-bit signed integer.
     * @param value A 16-bit signed integer.
     * @return A CBORObject object.
     */
    public static CBORObject FromObject(short value) {
      return FromObject((long)value);
    }

    /**
     * Generates a CBOR string object from a Unicode character.
     * @param value A char object.
     * @return A CBORObject object.
     * @throws java.lang.IllegalArgumentException The parameter {@code value}
     * is a surrogate code point.
     */
    public static CBORObject FromObject(char value) {
      return FromObject(new String(new char[] { value }));
    }

    /**
     * Returns the CBOR true value or false value, depending on &quot;value&quot;.
     * @param value Either True or False.
     * @return CBORObject.True if value is true; otherwise CBORObject.False.
     */
    public static CBORObject FromObject(boolean value) {
      return value ? CBORObject.True : CBORObject.False;
    }

    /**
     * Generates a CBOR object from a byte (0 to 255).
     * @param value A Byte object.
     * @return A CBORObject object.
     */
    public static CBORObject FromObject(byte value) {
      return FromObject(((int)value) & 0xff);
    }

    /**
     * Generates a CBOR object from a 32-bit floating-point number.
     * @param value A 32-bit floating-point number.
     * @return A CBORObject object.
     */
    public static CBORObject FromObject(float value) {
      return new CBORObject(CBORObjectTypeSingle, value);
    }

    /**
     * Generates a CBOR object from a 64-bit floating-point number.
     * @param value A 64-bit floating-point number.
     * @return A CBORObject object.
     */
    public static CBORObject FromObject(double value) {
      return new CBORObject(CBORObjectTypeDouble, value);
    }

    /**
     * Generates a CBOR object from a byte array. The byte array is copied
     * to a new byte array. (This method can't be used to decode CBOR data from
     * a byte array; for that, use the DecodeFromBytes method instead.).
     * @param bytes A byte array. Can be null.
     * @return A CBOR byte string object where each byte of the given byte
     * array is copied to a new array, or CBORObject.Null if the value is null.
     */
    public static CBORObject FromObject(byte[] bytes) {
      if (bytes == null) {
        return CBORObject.Null;
      }
      byte[] newvalue = new byte[bytes.length];
      System.arraycopy(bytes, 0, newvalue, 0, bytes.length);
      return new CBORObject(CBORObjectTypeByteString, bytes);
    }

    /**
     * Generates a CBOR object from an array of CBOR objects.
     * @param array An array of CBOR objects.
     * @return A CBOR object where each element of the given array is copied
     * to a new array, or CBORObject.Null if the value is null.
     */
    public static CBORObject FromObject(CBORObject[] array) {
      if (array == null) {
        return CBORObject.Null;
      }
      List<CBORObject> list = new ArrayList<CBORObject>();
      for(CBORObject i : array) {
        list.add(FromObject(i));
      }
      return new CBORObject(CBORObjectTypeArray, list);
    }

    /**
     * Generates a CBOR object from an array of 32-bit integers.
     * @param array An array of 32-bit integers.
     * @return A CBOR array object where each element of the given array is
     * copied to a new array, or CBORObject.Null if the value is null.
     */
    public static CBORObject FromObject(int[] array) {
      if (array == null) {
        return CBORObject.Null;
      }
      List<CBORObject> list = new ArrayList<CBORObject>();
      for(int i : array) {
        list.add(FromObject(i));
      }
      return new CBORObject(CBORObjectTypeArray, list);
    }

    /**
     * Generates a CBOR object from an array of 64-bit integers.
     * @param array An array of 64-bit integers.
     * @return A CBOR array object where each element of the given array is
     * copied to a new array, or CBORObject.Null if the value is null.
     */
    public static CBORObject FromObject(long[] array) {
      if (array == null) {
        return CBORObject.Null;
      }
      List<CBORObject> list = new ArrayList<CBORObject>();
      for(long i : array) {
        // System.out.println(i);
        list.add(FromObject(i));
      }
      return new CBORObject(CBORObjectTypeArray, list);
    }

    /**
     * Generates a CBOR object from a list of objects.
     * @param value An array of CBOR objects. Can be null.
     * @param <T> A type convertible to CBORObject.
     * @return A CBOR object where each element of the given array is converted
     * to a CBOR object and copied to a new array, or CBORObject.Null if the
     * value is null.
     */
    public static <T> CBORObject FromObject(List<T> value) {
      if (value == null) {
        return CBORObject.Null;
      }
      if (value.size() == 0) {
        return new CBORObject(CBORObjectTypeArray, new ArrayList<T>());
      }
      CBORObject retCbor = CBORObject.NewArray();
      for(T i : (List<T>)value) {
        retCbor.Add(CBORObject.FromObject(i));
      }
      return retCbor;
    }

    /**
     * Generates a CBOR object from an enumerable set of objects.
     * @param value An object that implements the Iterable interface. In
     * the .NET version, this can be the return value of an iterator or the
     * result of a LINQ query.
     * @param <T> A type convertible to CBORObject.
     * @return A CBOR object where each element of the given enumerable object
     * is converted to a CBOR object and copied to a new array, or CBORObject.Null
     * if the value is null.
     */
    public static <T> CBORObject FromObject(Iterable<T> value) {
      if (value == null) {
        return CBORObject.Null;
      }
      CBORObject retCbor = CBORObject.NewArray();
      for(T i : (Iterable<T>)value) {
        retCbor.Add(CBORObject.FromObject(i));
      }
      return retCbor;
    }

    /**
     * Generates a CBOR object from a map of objects.
     * @param dic A map of CBOR objects.
     * @param <TKey> A type convertible to CBORObject; the type of the keys.
     * @param <TValue> A type convertible to CBORObject; the type of the
     * values.
     * @return A CBOR object where each key and value of the given map is converted
     * to a CBOR object and copied to a new map, or CBORObject.Null if {@code
     * dic} is null.
     */
    public static <TKey, TValue> CBORObject FromObject(Map<TKey, TValue> dic) {
      if (dic == null) {
        return CBORObject.Null;
      }
      HashMap<CBORObject, CBORObject> map=new HashMap<CBORObject, CBORObject>();
      for(Map.Entry<TKey, TValue> entry : dic.entrySet()) {
        CBORObject key = FromObject(entry.getKey());
        CBORObject value = FromObject(entry.getValue());
        map.put(key,value);
      }
      return new CBORObject(CBORObjectTypeMap, map);
    }

    /**
     *
     */
    public static CBORObject FromObject(Object obj) {
      if (obj == null) {
        return CBORObject.Null;
      }
      if(obj instanceof String) {
        return FromObject((String)obj);
      }
      if(obj instanceof Integer) {
        return FromObject(((Integer)obj).intValue());
      }
      if(obj instanceof Long) {
        return FromObject((((Long)obj).longValue()));
      }
      if(obj instanceof CBORObject) {
        return FromObject((CBORObject)obj);
      }
      if(obj instanceof BigInteger) {
        return FromObject((BigInteger)obj);
      }
      ExtendedDecimal df = ((obj instanceof ExtendedDecimal) ? (ExtendedDecimal)obj : null);
      if (df != null) {
        return FromObject(df);
      }
      ExtendedFloat bf = ((obj instanceof ExtendedFloat) ? (ExtendedFloat)obj : null);
      if (bf != null) {
        return FromObject(bf);
      }
      ExtendedRational rf = ((obj instanceof ExtendedRational) ? (ExtendedRational)obj : null);
      if (rf != null) {
        return FromObject(rf);
      }
      if(obj instanceof Short) {
        return FromObject(((Short)obj).shortValue());
      }
      if(obj instanceof Character) {
        return FromObject(((Character)obj).charValue());
      }
      if(obj instanceof Boolean) {
        return FromObject(((Boolean)obj).booleanValue());
      }
      if(obj instanceof Byte) {
        return FromObject(((Byte)obj).byteValue());
      }
      if(obj instanceof Float) {
        return FromObject(((Float)obj).floatValue());
      }

      if(obj instanceof Enum<?>) {
        return FromObject(PropertyMap.EnumToObject((Enum<?>)obj));
      }
      if(obj instanceof Double) {
        return FromObject(((Double)obj).doubleValue());
      }
      byte[] bytearr = ((obj instanceof byte[]) ? (byte[])obj : null);
      if (bytearr != null) {
        return FromObject(bytearr);
      }
      CBORObject objret;
      if(obj instanceof Map<?,?>) {
        // Map appears first because Map includes Iterable
        objret = CBORObject.NewMap();
        Map<?,?> objdic = (Map<?,?>)obj;
        for(Object key : objdic.keySet()) {
          objret.set(CBORObject.FromObject(key),CBORObject.FromObject(objdic.get(key)));
        }
        return objret;
      }
      if(obj.getClass().isArray()) {
        return PropertyMap.FromArray(obj);
      }
      if(obj instanceof Iterable<?>) {
        objret = CBORObject.NewArray();
        for(Object element : (Iterable<?>)obj) {
          objret.Add(CBORObject.FromObject(element));
        }
        return objret;
      }
      objret = ConvertWithConverter(obj);
      if (objret != null) {
        return objret;
      }
      objret = CBORObject.NewMap();
      for(Map.Entry<String, Object> key : PropertyMap.GetProperties(obj)) {
        objret.set(key.getKey(),CBORObject.FromObject(key.getValue()));
      }
      return objret;
    }

    /**
     * Generates a CBOR object from an arbitrary object and gives the resulting
     * object a tag.
     * @param valueOb An arbitrary object. If the tag number is 2 or 3, this
     * must be a byte string whose bytes represent an integer in little-endian
     * byte order, and the value of the number is 1 minus the integer&apos;s
     * value for tag 3. If the tag number is 4 or 5, this must be an array with
     * two elements: the first must be an integer representing the exponent,
     * and the second must be an integer representing a mantissa.
     * @param bigintTag Tag number. The tag number 55799 can be used to mark
     * a &quot;self-described CBOR&quot; object.
     * @return A CBOR object where the object {@code valueOb} is converted
     * to a CBOR object and given the tag {@code bigintTag}.
     * @throws java.lang.IllegalArgumentException The parameter {@code bigintTag}
     * is less than 0 or greater than 2^64-1, or {@code valueOb}'s type is
     * unsupported.
     * @throws java.lang.NullPointerException The parameter {@code bigintTag}
     * is null.
     */
    public static CBORObject FromObjectAndTag(Object valueOb, BigInteger bigintTag) {
      if (bigintTag == null) {
        throw new NullPointerException("bigintTag");
      }
      if (bigintTag.signum() < 0) {
        throw new IllegalArgumentException("bigintTag's sign (" + Long.toString((long)bigintTag.signum()) + ") is less than " + "0");
      }
      if (bigintTag.compareTo(UInt64MaxValue) > 0) {
        throw new IllegalArgumentException("tag more than 18446744073709551615 (" + (bigintTag) + ")");
      }
      CBORObject c = FromObject(valueOb);
      if (bigintTag.bitLength() <= 16) {
        // Low-numbered, commonly used tags
        return FromObjectAndTag(c, bigintTag.intValue());
      } else {
        int tagLow = 0;
        int tagHigh = 0;
        byte[] bytes = bigintTag.toByteArray(true);
        for (int i = 0; i < Math.min(4, bytes.length); ++i) {
          int b = ((int)bytes[i]) & 0xff;
          tagLow = (tagLow | (((int)b) << (i * 8)));
        }
        for (int i = 4; i < Math.min(8, bytes.length); ++i) {
          int b = ((int)bytes[i]) & 0xff;
          tagHigh = (tagHigh | (((int)b) << (i * 8)));
        }
        CBORObject c2 = new CBORObject(c, tagLow, tagHigh);
        ICBORTag tagconv = FindTagConverter(bigintTag);
        if (tagconv != null) {
          c2 = tagconv.ValidateObject(c2);
        }
        return c2;
      }
    }

    private static ICBORTag FindTagConverter(int tag) {
      return FindTagConverter(BigInteger.valueOf(tag));
    }

    static ICBORTag FindTagConverterLong(long tag) {
      return FindTagConverter(BigInteger.valueOf(tag));
    }

    static ICBORTag FindTagConverter(BigInteger bigintTag) {
      if (TagHandlersEmpty()) {
        AddTagHandler(BigInteger.valueOf(2), new CBORTag2());
        AddTagHandler(BigInteger.valueOf(3), new CBORTag3());
        AddTagHandler(BigInteger.valueOf(4), new CBORTag4());
        AddTagHandler(BigInteger.valueOf(5), new CBORTag5());
        AddTagHandler(BigInteger.valueOf(264), new CBORTag4(true));
        AddTagHandler(BigInteger.valueOf(265), new CBORTag5(true));
        AddTagHandler(BigInteger.valueOf(25), new CBORTagUnsigned());
        AddTagHandler(BigInteger.valueOf(28), new CBORTag28());
        AddTagHandler(BigInteger.valueOf(29), new CBORTagUnsigned());
        AddTagHandler(BigInteger.valueOf(256), new CBORTagAny());
        AddTagHandler(BigInteger.ZERO, new CBORTag0());
        AddTagHandler(BigInteger.valueOf(32), new CBORTag32());
        AddTagHandler(BigInteger.valueOf(33), new CBORTagGenericString());
        AddTagHandler(BigInteger.valueOf(34), new CBORTagGenericString());
        AddTagHandler(BigInteger.valueOf(35), new CBORTagGenericString());
        AddTagHandler(BigInteger.valueOf(36), new CBORTagGenericString());
        AddTagHandler(BigInteger.valueOf(37), new CBORTag37());
        AddTagHandler(BigInteger.valueOf(30), new CBORTag30());
      }
      synchronized(tagHandlers) {
        if (tagHandlers.containsKey(bigintTag)) {
          return tagHandlers.get(bigintTag);
        }

        return null;
      }
    }

    /**
     * Generates a CBOR object from an arbitrary object and gives the resulting
     * object a tag.
     * @param valueObValue An arbitrary object. If the tag number is 2 or
     * 3, this must be a byte string whose bytes represent an integer in little-endian
     * byte order, and the value of the number is 1 minus the integer&apos;s
     * value for tag 3. If the tag number is 4 or 5, this must be an array with
     * two elements: the first must be an integer representing the exponent,
     * and the second must be an integer representing a mantissa.
     * @param smallTag A 32-bit integer that specifies a tag number. The
     * tag number 55799 can be used to mark a &quot;self-described CBOR&quot;
     * object.
     * @return A CBOR object where the object {@code valueObValue} is converted
     * to a CBOR object and given the tag {@code smallTag}.
     * @throws java.lang.IllegalArgumentException The parameter {@code smallTag}
     * is less than 0 or {@code valueObValue} 's type is unsupported.
     */
    public static CBORObject FromObjectAndTag(Object valueObValue, int smallTag) {
      if (smallTag < 0) {
        throw new IllegalArgumentException("smallTag (" + Long.toString((long)smallTag) + ") is less than " + "0");
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
      while (curobject.isTagged()) {
        sb.append(')');
        curobject = (CBORObject)curobject.itemValue;
      }
    }

    private void WriteTags(OutputStream s) throws IOException {
      CBORObject curobject = this;
      while (curobject.isTagged()) {
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
          byte[] arrayToWrite = new byte[] {  (byte)0xdb,
            (byte)((high >> 24) & 0xff),
            (byte)((high >> 16) & 0xff),
            (byte)((high >> 8) & 0xff),
            (byte)(high & 0xff),
            (byte)((low >> 24) & 0xff),
            (byte)((low >> 16) & 0xff),
            (byte)((low >> 8) & 0xff),
            (byte)(low & 0xff)  };
          s.write(arrayToWrite,0,9);
        }
        curobject = (CBORObject)curobject.itemValue;
      }
    }

    private void AppendOpeningTags(StringBuilder sb) {
      CBORObject curobject = this;
      while (curobject.isTagged()) {
        int low = curobject.tagLow;
        int high = curobject.tagHigh;
        if (high == 0 && (low >> 16) == 0) {
          sb.append(Integer.toString((int)low));
        } else {
          BigInteger bi = LowHighToBigInteger(low, high);
          sb.append(CBORUtilities.BigIntToString(bi));
        }
        sb.append('(');
        curobject = (CBORObject)curobject.itemValue;
      }
    }

    private static String TrimDotZero(String str) {
      if (str.length() > 2 && str.charAt(str.length() - 1) == '0' && str.charAt(str.length() - 2) == '.') {
        return str.substring(0,str.length() - 2);
      }
      return str;
    }

    private static String ExtendedToString(ExtendedFloat ef) {
      if (ef.isFinite() && (ef.getExponent().compareTo(BigInteger.valueOf(2500)) > 0 ||
                          ef.getExponent().compareTo(BigInteger.valueOf(-2500)) < 0)) {
        // It can take very long to convert a number with a very high
        // or very low exponent to a decimal String, so do this instead
        return ef.getMantissa().toString() + "p" + ef.getExponent().toString();
      }
      return ef.toString();
    }

    /**
     * Returns this CBOR object in string form. The format is intended to
     * be human-readable, not machine-readable, and the format may change
     * at any time.
     * @return A text representation of this object.
     */
    @Override public String toString() {
      StringBuilder sb = null;
      String simvalue = null;
      int type = this.getItemType();
      if (this.isTagged()) {
        if (sb == null) {
          if (type == CBORObjectTypeTextString) {
            // The default capacity of StringBuilder may be too small
            // for many strings, so set a suggested capacity
            // explicitly
            String str = this.AsString();
            sb = new StringBuilder(Math.min(str.length(), 4096) + 16);
          } else {
            sb = new StringBuilder();
          }
        }
        this.AppendOpeningTags(sb);
      }
      if (type == CBORObjectTypeSimpleValue) {
        if (this.isTrue()) {
          simvalue = "true";
        } else if (this.isFalse()) {
          simvalue = "false";
        } else if (this.isNull()) {
          simvalue = "null";
        } else if (this.isUndefined()) {
          simvalue = "undefined";
        } else {
          if (sb == null) {
            sb = new StringBuilder();
          }
          sb.append("simple(");
          sb.append(Integer.toString(((Integer)this.getThisItem()).intValue()));
          sb.append(")");
        }
        if (simvalue != null) {
          if (sb == null) {
            return simvalue;
          }
          sb.append(simvalue);
        }
      } else if (type == CBORObjectTypeSingle) {
        float f = ((Float)this.getThisItem()).floatValue();
        if (((f)==Float.NEGATIVE_INFINITY)) {
          simvalue = "-Infinity";
        } else if (((f)==Float.POSITIVE_INFINITY)) {
          simvalue = "Infinity";
        } else if (Float.isNaN(f)) {
          simvalue = "NaN";
        } else {
          simvalue = TrimDotZero(Float.toString((float)f));
        }
        if (sb == null) {
          return simvalue;
        }
        sb.append(simvalue);
      } else if (type == CBORObjectTypeDouble) {
        double f = ((Double)this.getThisItem()).doubleValue();
        if (((f)==Double.NEGATIVE_INFINITY)) {
          simvalue = "-Infinity";
        } else if (((f)==Double.POSITIVE_INFINITY)) {
          simvalue = "Infinity";
        } else if (Double.isNaN(f)) {
          simvalue = "NaN";
        } else {
          simvalue = TrimDotZero(Double.toString((double)f));
        }
        if (sb == null) {
          return simvalue;
        }
        sb.append(simvalue);
      } else if (type == CBORObjectTypeExtendedFloat) {
        simvalue = ExtendedToString((ExtendedFloat)this.getThisItem());
        if (sb == null) {
          return simvalue;
        }
        sb.append(simvalue);
      } else if (type == CBORObjectTypeInteger) {
        long v = (((Long)this.getThisItem()).longValue());
        simvalue = Long.toString((long)v);
        if (sb == null) {
          return simvalue;
        }
        sb.append(simvalue);
      } else if (type == CBORObjectTypeBigInteger) {
        simvalue = CBORUtilities.BigIntToString((BigInteger)this.getThisItem());
        if (sb == null) {
          return simvalue;
        }
        sb.append(simvalue);
      } else if (type == CBORObjectTypeByteString) {
        if (sb == null) {
          sb = new StringBuilder();
        }
        sb.append("h'");
        byte[] data = (byte[])this.getThisItem();
        CBORUtilities.ToBase16(sb, data);
        sb.append("'");
      } else if (type == CBORObjectTypeTextString) {
        if (sb == null) {
          return "\"" + this.AsString() + "\"";
        } else {
          sb.append('\"');
          sb.append(this.AsString());
          sb.append('\"');
        }
      } else if (type == CBORObjectTypeArray) {
        if (sb == null) {
          sb = new StringBuilder();
        }
        boolean first = true;
        sb.append("[");
        for(CBORObject i : this.AsList()) {
          if (!first) {
            sb.append(", ");
          }
          sb.append(i.toString());
          first = false;
        }
        sb.append("]");
      } else if (type == CBORObjectTypeMap) {
        if (sb == null) {
          sb = new StringBuilder();
        }
        boolean first = true;
        sb.append("{");
        Map<CBORObject, CBORObject> map = this.AsMap();
        for(Map.Entry<CBORObject, CBORObject> entry : map.entrySet()) {
          CBORObject key = entry.getKey();
          CBORObject value = entry.getValue();
          if (!first) {
            sb.append(", ");
          }
          sb.append(key.toString());
          sb.append(": ");
          sb.append(value.toString());
          first = false;
        }
        sb.append("}");
      } else {
        if (sb == null) {
          return this.getThisItem().toString();
        }
        sb.append(this.getThisItem().toString());
      }
      if (this.isTagged()) {
        this.AppendClosingTags(sb);
      }
      return sb.toString();
    }

    private static boolean BigIntFits(BigInteger bigint) {
      return bigint.bitLength() <= 64;
    }
  }
