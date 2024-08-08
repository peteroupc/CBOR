/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using PeterO.Numbers;

namespace PeterO.Cbor {
  // Contains extra methods placed separately
  // because they are not CLS-compliant or they
  // are specific to the .NET version of the library.
  public sealed partial class CBORObject {
    /* The "==" and "!=" operators are not overridden in the .NET version to be
      consistent with Equals, for two reasons: (1) This type is mutable in
    certain cases, which can cause different results when comparing with another
      object. (2) Objects with this type can have arbitrary size (e.g., they
    can be byte strings, text strings, arrays, or maps of arbitrary size), and
    comparing
      two of them for equality can be much more complicated and take much
      more time than the default behavior of reference equality.
    */

    /// <summary>Returns whether one object's value is less than
    /// another's.</summary>
    /// <param name='a'>The left-hand side of the comparison.</param>
    /// <param name='b'>The right-hand side of the comparison.</param>
    /// <returns><c>true</c> if one object's value is less than another's;
    /// otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='a'/> is null.</exception>
    public static bool operator <(CBORObject a, CBORObject b) {
      return a == null ? b != null : a.CompareTo(b) < 0;
    }

    /// <summary>Returns whether one object's value is up to
    /// another's.</summary>
    /// <param name='a'>The left-hand side of the comparison.</param>
    /// <param name='b'>The right-hand side of the comparison.</param>
    /// <returns><c>true</c> if one object's value is up to another's;
    /// otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='a'/> is null.</exception>
    public static bool operator <=(CBORObject a, CBORObject b) {
      return a == null || a.CompareTo(b) <= 0;
    }

    /// <summary>Returns whether one object's value is greater than
    /// another's.</summary>
    /// <param name='a'>The left-hand side of the comparison.</param>
    /// <param name='b'>The right-hand side of the comparison.</param>
    /// <returns><c>true</c> if one object's value is greater than
    /// another's; otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='a'/> is null.</exception>
    public static bool operator >(CBORObject a, CBORObject b) {
      return a != null && a.CompareTo(b) > 0;
    }

    /// <summary>Returns whether one object's value is at least
    /// another's.</summary>
    /// <param name='a'>The left-hand side of the comparison.</param>
    /// <param name='b'>The right-hand side of the comparison.</param>
    /// <returns><c>true</c> if one object's value is at least another's;
    /// otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='a'/> is null.</exception>
    public static bool operator >=(CBORObject a, CBORObject b) {
      return a == null ? b == null : a.CompareTo(b) >= 0;
    }

    /// <summary>Writes a CBOR major type number and an integer 0 or
    /// greater associated with it to a data stream, where that integer is
    /// passed to this method as a 32-bit unsigned integer. This is a
    /// low-level method that is useful for implementing custom CBOR
    /// encoding methodologies. This method encodes the given major type
    /// and value in the shortest form allowed for the major
    /// type.</summary>
    /// <param name='outputStream'>A writable data stream.</param>
    /// <param name='majorType'>The CBOR major type to write. This is a
    /// number from 0 through 7 as follows. 0: integer 0 or greater; 1:
    /// negative integer; 2: byte string; 3: UTF-8 text string; 4: array;
    /// 5: map; 6: tag; 7: simple value. See RFC 7049 for details on these
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
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='outputStream'/> is null.</exception>
    [CLSCompliant(false)]
    public static int WriteValue(
      Stream outputStream,
      int majorType,
      uint value) {
      return outputStream == null ?
        throw new ArgumentNullException(nameof(outputStream)) :
        WriteValue(outputStream, majorType, (long)value);
    }

    /// <summary>Writes a CBOR major type number and an integer 0 or
    /// greater associated with it to a data stream, where that integer is
    /// passed to this method as a 64-bit unsigned integer. This is a
    /// low-level method that is useful for implementing custom CBOR
    /// encoding methodologies. This method encodes the given major type
    /// and value in the shortest form allowed for the major
    /// type.</summary>
    /// <param name='outputStream'>A writable data stream.</param>
    /// <param name='majorType'>The CBOR major type to write. This is a
    /// number from 0 through 7 as follows. 0: integer 0 or greater; 1:
    /// negative integer; 2: byte string; 3: UTF-8 text string; 4: array;
    /// 5: map; 6: tag; 7: simple value. See RFC 7049 for details on these
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
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='majorType'/> is 7 and value is greater than 255.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='outputStream'/> is null.</exception>
    [CLSCompliant(false)]
    public static int WriteValue(
      Stream outputStream,
      int majorType,
      ulong value) {
      if (outputStream == null) {
        throw new ArgumentNullException(nameof(outputStream));
      }
      if (value <= Int64.MaxValue) {
        return WriteValue(outputStream, majorType, (long)value);
      } else {
        if (majorType < 0) {
          throw new ArgumentException("majorType(" + majorType +
            ") is less than 0");
        }
        if (majorType > 7) {
          throw new ArgumentException("majorType(" + majorType +
            ") is more than 7");
        }
        if (majorType == 7) {
          throw new ArgumentException("majorType is 7 and value is greater" +
            "\u0020than 255");
        }
        byte[] bytes = {
          (byte)(27 | (majorType << 5)), (byte)((value >>
          56) & 0xff),
          (byte)((value >> 48) & 0xff), (byte)((value >> 40) & 0xff),
          (byte)((value >> 32) & 0xff), (byte)((value >> 24) & 0xff),
          (byte)((value >> 16) & 0xff), (byte)((value >> 8) & 0xff),
          (byte)(value & 0xff),
        };
        outputStream.Write(bytes, 0, bytes.Length);
        return bytes.Length;
      }
    }

    private static EInteger DecimalToEInteger(decimal dec) {
      return ((EDecimal)dec).ToEInteger();
    }

    /// <summary>Writes an 8-bit signed integer in CBOR format to a data
    /// stream.</summary>
    /// <param name='value'>The parameter <paramref name='value'/> is an
    /// 8-bit signed integer.</param>
    /// <param name='stream'>A writable data stream.</param>
    [CLSCompliant(false)]
    public static void Write(sbyte value, Stream stream) {
      Write((long)value, stream);
    }

    /// <summary>Writes a 64-bit unsigned integer in CBOR format to a data
    /// stream.</summary>
    /// <param name='value'>A 64-bit unsigned integer.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    [CLSCompliant(false)]
    public static void Write(ulong value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (value <= Int64.MaxValue) {
        Write((long)value, stream);
      } else {
        stream.WriteByte(27);
        stream.WriteByte((byte)((value >> 56) & 0xff));
        stream.WriteByte((byte)((value >> 48) & 0xff));
        stream.WriteByte((byte)((value >> 40) & 0xff));
        stream.WriteByte((byte)((value >> 32) & 0xff));
        stream.WriteByte((byte)((value >> 24) & 0xff));
        stream.WriteByte((byte)((value >> 16) & 0xff));
        stream.WriteByte((byte)((value >> 8) & 0xff));
        stream.WriteByte((byte)(value & 0xff));
      }
    }

    /// <summary>Converts a.NET decimal to a CBOR object.</summary>
    /// <param name='value'>The parameter <paramref name='value'/> is a
    /// Decimal object.</param>
    /// <returns>A CBORObject object with the same value as the.NET
    /// decimal.</returns>
    public static CBORObject FromDecimal(decimal value) =>
FromEDecimal((EDecimal)value);

    /// <summary>Converts a.NET decimal to a CBOR object.</summary>
    /// <param name='value'>A Decimal.</param>
    /// <returns>A CBORObject object.</returns>
    [Obsolete("Use FromDecimal instead")]
    public static CBORObject FromObject(decimal value) => FromDecimal(value);

    /// <summary>Writes a 32-bit unsigned integer in CBOR format to a data
    /// stream.</summary>
    /// <param name='value'>A 32-bit unsigned integer.</param>
    /// <param name='stream'>A writable data stream.</param>
    [CLSCompliant(false)]
    public static void Write(uint value, Stream stream) {
      Write((ulong)value, stream);
    }

    /// <summary>Writes a 16-bit unsigned integer in CBOR format to a data
    /// stream.</summary>
    /// <param name='value'>A 16-bit unsigned integer.</param>
    /// <param name='stream'>A writable data stream.</param>
    [CLSCompliant(false)]
    public static void Write(ushort value, Stream stream) {
      Write((ulong)value, stream);
    }

    /// <summary>Converts a signed 8-bit integer to a CBOR
    /// object.</summary>
    /// <param name='value'>The parameter <paramref name='value'/> is an
    /// 8-bit signed integer.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    public static CBORObject FromSbyte(sbyte value) {
      return FromInt64((long)value);
    }

    /// <summary>Converts a signed 8-bit integer to a CBOR
    /// object.</summary>
    /// <param name='value'>The input.</param>
    /// <returns>A CBORObject object.</returns>
    [Obsolete("Use FromSbyte instead")]
    [CLSCompliant(false)]
    public static CBORObject FromObject(sbyte value) => FromSbyte(value);

    private static EInteger UInt64ToEInteger(ulong value) {
      var data = new byte[9];
      ulong uvalue = value;
      data[0] = (byte)(uvalue & 0xff);
      data[1] = (byte)((uvalue >> 8) & 0xff);
      data[2] = (byte)((uvalue >> 16) & 0xff);
      data[3] = (byte)((uvalue >> 24) & 0xff);
      data[4] = (byte)((uvalue >> 32) & 0xff);
      data[5] = (byte)((uvalue >> 40) & 0xff);
      data[6] = (byte)((uvalue >> 48) & 0xff);
      data[7] = (byte)((uvalue >> 56) & 0xff);
      data[8] = 0;
      return EInteger.FromBytes(data, true);
    }

    /// <summary>Converts a 64-bit unsigned integer to a CBOR
    /// object.</summary>
    /// <param name='value'>A 64-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    public static CBORObject FromUInt64(ulong value) {
      return CBORObject.FromEInteger(UInt64ToEInteger(value));
    }

    /// <summary>Converts a 64-bit unsigned integer to a CBOR
    /// object.</summary>
    /// <param name='value'>A 64-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    [Obsolete("Use FromUInt64 instead")]
    public static CBORObject FromObject(ulong value) => FromUInt64(value);

    /// <summary>Converts a 32-bit unsigned integer to a CBOR
    /// object.</summary>
    /// <param name='value'>A 32-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    public static CBORObject FromUInt(uint value) {
      return FromInt64((long)value);
    }

    /// <summary>Converts a 32-bit unsigned integer to a CBOR
    /// object.</summary>
    /// <param name='value'>A 32-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    [Obsolete("Use FromUInt instead")]
    public static CBORObject FromObject(uint value) => FromUInt(value);

    /// <summary>Converts a 16-bit unsigned integer to a CBOR
    /// object.</summary>
    /// <param name='value'>A 16-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    public static CBORObject FromUShort(ushort value) {
      return FromInt64((long)value);
    }

    /// <summary>Converts a 16-bit unsigned integer to a CBOR
    /// object.</summary>
    /// <param name='value'>A 16-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    [Obsolete("Use FromUShort instead")]
    public static CBORObject FromObject(ushort value) => FromInt64((long)value);

    /// <summary>Generates a CBOR object from this one, but gives the
    /// resulting object a tag in addition to its existing tags (the new
    /// tag is made the outermost tag).</summary>
    /// <param name='tag'>A 64-bit integer that specifies a tag number. The
    /// tag number 55799 can be used to mark a "self-described CBOR"
    /// object. This document does not attempt to list all CBOR tags and
    /// their meanings. An up-to-date list can be found at the CBOR Tags
    /// registry maintained by the Internet Assigned Numbers Authority(
    /// <i>iana.org/assignments/cbor-tags</i> ).</param>
    /// <returns>A CBOR object with the same value as this one but given
    /// the tag <paramref name='tag'/> in addition to its existing tags
    /// (the new tag is made the outermost tag).</returns>
    [CLSCompliant(false)]
    public CBORObject WithTag(ulong tag) {
      return FromCBORObjectAndTag(this, UInt64ToEInteger(tag));
    }

    /// <summary>Generates a CBOR object from an arbitrary object and gives
    /// the resulting object a tag.</summary>
    /// <param name='o'>The parameter <paramref name='o'/> is an arbitrary
    /// CBORObject.</param>
    /// <param name='tag'>A 64-bit integer that specifies a tag number. The
    /// tag number 55799 can be used to mark a "self-described CBOR"
    /// object. This document does not attempt to list all CBOR tags and
    /// their meanings. An up-to-date list can be found at the CBOR Tags
    /// registry maintained by the Internet Assigned Numbers Authority(
    /// <i>iana.org/assignments/cbor-tags</i> ).</param>
    /// <returns>A CBOR object where the object <paramref name='o'/> is
    /// given the tag <paramref name='tag'/>. If "valueOb" is null,
    /// returns a version of CBORObject.Null with the given tag.</returns>
    [CLSCompliant(false)]
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static CBORObject FromCBORObjectAndTag(CBORObject o, ulong tag) {
      return FromCBORObjectAndTag(o, UInt64ToEInteger(tag));
    }

    /// <summary>
    /// <para>Converts this CBOR object to an object of an arbitrary type.
    /// See
    /// <see cref='PeterO.Cbor.CBORObject.ToObject(System.Type)'/> for
    /// further information.</para></summary>
    /// <typeparam name='T'>The type, class, or interface that this
    /// method's return value will belong to. <b>Note:</b> For security
    /// reasons, an application should not base this parameter on user
    /// input or other externally supplied data. Whenever possible, this
    /// parameter should be either a type specially handled by this method
    /// (such as <c>int</c> or <c>String</c> ) or a plain-old-data type
    /// (POCO or POJO type) within the control of the application. If the
    /// plain-old-data type references other data types, those types should
    /// likewise meet either criterion above.</typeparam>
    /// <returns>The converted object.</returns>
    /// <exception cref='NotSupportedException'>The given type "T", or this
    /// object's CBOR type, is not supported.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public T ToObject<T>() {
      return (T)this.ToObject(typeof(T));
    }

    /// <summary>
    /// <para>Converts this CBOR object to an object of an arbitrary type.
    /// See
    /// <see cref='PeterO.Cbor.CBORObject.ToObject(System.Type)'/> for
    /// further information.</para></summary>
    /// <param name='mapper'>This parameter controls which data types are
    /// eligible for Plain-Old-Data deserialization and includes custom
    /// converters from CBOR objects to certain data types.</param>
    /// <typeparam name='T'>The type, class, or interface that this
    /// method's return value will belong to. <b>Note:</b> For security
    /// reasons, an application should not base this parameter on user
    /// input or other externally supplied data. Whenever possible, this
    /// parameter should be either a type specially handled by this method
    /// (such as <c>int</c> or <c>String</c> ) or a plain-old-data type
    /// (POCO or POJO type) within the control of the application. If the
    /// plain-old-data type references other data types, those types should
    /// likewise meet either criterion above.</typeparam>
    /// <returns>The converted object.</returns>
    /// <exception cref='NotSupportedException'>The given type "T", or this
    /// object's CBOR type, is not supported.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public T ToObject<T>(CBORTypeMapper mapper) {
      return (T)this.ToObject(typeof(T), mapper);
    }

    /// <summary>
    /// <para>Converts this CBOR object to an object of an arbitrary type.
    /// See
    /// <see cref='PeterO.Cbor.CBORObject.ToObject(System.Type)'/> for
    /// further information.</para></summary>
    /// <param name='options'>Specifies options for controlling
    /// deserialization of CBOR objects.</param>
    /// <typeparam name='T'>The type, class, or interface that this
    /// method's return value will belong to. <b>Note:</b> For security
    /// reasons, an application should not base this parameter on user
    /// input or other externally supplied data. Whenever possible, this
    /// parameter should be either a type specially handled by this method
    /// (such as <c>int</c> or <c>String</c> ) or a plain-old-data type
    /// (POCO or POJO type) within the control of the application. If the
    /// plain-old-data type references other data types, those types should
    /// likewise meet either criterion above.</typeparam>
    /// <returns>The converted object.</returns>
    /// <exception cref='NotSupportedException'>The given type "T", or this
    /// object's CBOR type, is not supported.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public T ToObject<T>(PODOptions options) {
      return (T)this.ToObject(typeof(T), options);
    }

    /// <summary>
    /// <para>Converts this CBOR object to an object of an arbitrary type.
    /// See
    /// <see cref='PeterO.Cbor.CBORObject.ToObject(System.Type)'/> for
    /// further information.</para></summary>
    /// <param name='mapper'>This parameter controls which data types are
    /// eligible for Plain-Old-Data deserialization and includes custom
    /// converters from CBOR objects to certain data types.</param>
    /// <param name='options'>Specifies options for controlling
    /// deserialization of CBOR objects.</param>
    /// <typeparam name='T'>The type, class, or interface that this
    /// method's return value will belong to. <b>Note:</b> For security
    /// reasons, an application should not base this parameter on user
    /// input or other externally supplied data. Whenever possible, this
    /// parameter should be either a type specially handled by this method
    /// (such as <c>int</c> or <c>String</c> ) or a plain-old-data type
    /// (POCO or POJO type) within the control of the application. If the
    /// plain-old-data type references other data types, those types should
    /// likewise meet either criterion above.</typeparam>
    /// <returns>The converted object.</returns>
    /// <exception cref='NotSupportedException'>The given type "T", or this
    /// object's CBOR type, is not supported.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public T ToObject<T>(CBORTypeMapper mapper, PODOptions options) {
      return (T)this.ToObject(typeof(T), mapper, options);
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
    /// <param name='mapper'>This parameter controls which data types are
    /// eligible for Plain-Old-Data deserialization and includes custom
    /// converters from CBOR objects to certain data types. Can be
    /// null.</param>
    /// <param name='pod'>Specifies options for controlling deserialization
    /// of CBOR objects.</param>
    /// <typeparam name='T'>The type, class, or interface that this
    /// method's return value will belong to. To express a generic type in
    /// Java, see the example. <b>Note:</b> For security reasons, an
    /// application should not base this parameter on user input or other
    /// externally supplied data. Whenever possible, this parameter should
    /// be either a type specially handled by this method, such as
    /// <c>int</c> or <c>String</c>, or a plain-old-data type (POCO or
    /// POJO type) within the control of the application. If the
    /// plain-old-data type references other data types, those types should
    /// likewise meet either criterion above.</typeparam>
    /// <returns>An object of the given type decoded from the given byte
    /// array. Returns null (as opposed to CBORObject.Null) if <paramref
    /// name='data'/> is empty and the AllowEmpty property is set on the
    /// given CBOREncodeOptions object.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data. This includes cases where not all of
    /// the byte array represents a CBOR object. This exception is also
    /// thrown if the parameter <paramref name='data'/> is empty unless the
    /// AllowEmpty property is set on the given options object. Also thrown
    /// if the given type "T", or this object's CBOR type, is not
    /// supported, or the given object's nesting is too deep, or another
    /// error occurred when serializing the object.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='data'/> is null, or the parameter <paramref name='enc'/> is
    /// null, or the parameter "T" or <paramref name='pod'/> is
    /// null.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static T DecodeObjectFromBytes<T>(
      byte[] data,
      CBOREncodeOptions enc,
      CBORTypeMapper mapper,
      PODOptions pod) {
      return (T)DecodeObjectFromBytes(data, enc, typeof(T), mapper, pod);
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
    /// <typeparam name='T'>The type, class, or interface that this
    /// method's return value will belong to. <b>Note:</b> For security
    /// reasons, an application should not base this parameter on user
    /// input or other externally supplied data. Whenever possible, this
    /// parameter should be either a type specially handled by this method,
    /// such as <c>int</c> or <c>String</c>, or a plain-old-data type
    /// (POCO or POJO type) within the control of the application. If the
    /// plain-old-data type references other data types, those types should
    /// likewise meet either criterion above.</typeparam>
    /// <returns>An object of the given type decoded from the given byte
    /// array. Returns null (as opposed to CBORObject.Null) if <paramref
    /// name='data'/> is empty and the AllowEmpty property is set on the
    /// given CBOREncodeOptions object.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data. This includes cases where not all of
    /// the byte array represents a CBOR object. This exception is also
    /// thrown if the parameter <paramref name='data'/> is empty unless the
    /// AllowEmpty property is set on the given options object. Also thrown
    /// if the given type "T", or this object's CBOR type, is not
    /// supported, or the given object's nesting is too deep, or another
    /// error occurred when serializing the object.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='data'/> is null, or the parameter <paramref name='enc'/> is
    /// null.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static T DecodeObjectFromBytes<T>(byte[] data, CBOREncodeOptions
enc) {
      return (T)DecodeObjectFromBytes(data, enc, typeof(T));
    }

    /// <summary>Generates an object of an arbitrary type from an array of
    /// CBOR-encoded bytes. It is equivalent to DecodeFromBytes followed by
    /// ToObject. See the documentation for those methods for more
    /// information.</summary>
    /// <param name='data'>A byte array in which a single CBOR object is
    /// encoded.</param>
    /// <param name='mapper'>This parameter controls which data types are
    /// eligible for Plain-Old-Data deserialization and includes custom
    /// converters from CBOR objects to certain data types. Can be
    /// null.</param>
    /// <param name='pod'>Specifies options for controlling deserialization
    /// of CBOR objects.</param>
    /// <typeparam name='T'>The type, class, or interface that this
    /// method's return value will belong to. To express a generic type in
    /// Java, see the example. <b>Note:</b> For security reasons, an
    /// application should not base this parameter on user input or other
    /// externally supplied data. Whenever possible, this parameter should
    /// be either a type specially handled by this method, such as
    /// <c>int</c> or <c>String</c>, or a plain-old-data type (POCO or
    /// POJO type) within the control of the application. If the
    /// plain-old-data type references other data types, those types should
    /// likewise meet either criterion above.</typeparam>
    /// <returns>An object of the given type decoded from the given byte
    /// array. Returns null (as opposed to CBORObject.Null) if <paramref
    /// name='data'/> is empty and the AllowEmpty property is set on the
    /// given CBOREncodeOptions object.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data. This includes cases where not all of
    /// the byte array represents a CBOR object. This exception is also
    /// thrown if the parameter <paramref name='data'/> is empty unless the
    /// AllowEmpty property is set on the given options object. Also thrown
    /// if the given type "T", or this object's CBOR type, is not
    /// supported, or the given object's nesting is too deep, or another
    /// error occurred when serializing the object.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='data'/> is null, or the parameter "T" or <paramref
    /// name='pod'/> is null.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static T DecodeObjectFromBytes<T>(
      byte[] data,
      CBORTypeMapper mapper,
      PODOptions pod) {
      return (T)DecodeObjectFromBytes(data, typeof(T), mapper, pod);
    }

    /// <summary>Generates an object of an arbitrary type from an array of
    /// CBOR-encoded bytes. It is equivalent to DecodeFromBytes followed by
    /// ToObject. See the documentation for those methods for more
    /// information.</summary>
    /// <param name='data'>A byte array in which a single CBOR object is
    /// encoded.</param>
    /// <typeparam name='T'>The type, class, or interface that this
    /// method's return value will belong to. To express a generic type in
    /// Java, see the example. <b>Note:</b> For security reasons, an
    /// application should not base this parameter on user input or other
    /// externally supplied data. Whenever possible, this parameter should
    /// be either a type specially handled by this method, such as
    /// <c>int</c> or <c>String</c>, or a plain-old-data type (POCO or
    /// POJO type) within the control of the application. If the
    /// plain-old-data type references other data types, those types should
    /// likewise meet either criterion above.</typeparam>
    /// <returns>An object of the given type decoded from the given byte
    /// array. Returns null (as opposed to CBORObject.Null) if <paramref
    /// name='data'/> is empty and the AllowEmpty property is set on the
    /// given CBOREncodeOptions object.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>There was an error in
    /// reading or parsing the data. This includes cases where not all of
    /// the byte array represents a CBOR object. This exception is also
    /// thrown if the parameter <paramref name='data'/> is empty unless the
    /// AllowEmpty property is set on the given options object. Also thrown
    /// if the given type "T", or this object's CBOR type, is not
    /// supported, or the given object's nesting is too deep, or another
    /// error occurred when serializing the object.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='data'/> is null.</exception>
    [RequiresUnreferencedCode("Do not use in AOT or reflection-free contexts.")]
    public static T DecodeObjectFromBytes<T>(byte[] data) {
      return (T)DecodeObjectFromBytes(data, typeof(T));
    }
  }
}
