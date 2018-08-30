/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.IO;
using PeterO.Numbers;

namespace PeterO.Cbor {
  // Contains extra methods placed separately
  // because they are not CLS-compliant or they
  // are specific to the .NET framework.
  public sealed partial class CBORObject {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsUInt16"]/*'/>
    [CLSCompliant(false)]
    public ushort AsUInt16() {
      int v = this.AsInt32();
      if (v > UInt16.MaxValue || v < 0) {
        throw new OverflowException("This object's value is out of range");
      }
      return (ushort)v;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsUInt32"]/*'/>
    [CLSCompliant(false)]
    public uint AsUInt32() {
      ulong v = this.AsUInt64();
      if (v > UInt32.MaxValue) {
        throw new OverflowException("This object's value is out of range");
      }
      return (uint)v;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsSByte"]/*'/>
    [CLSCompliant(false)]
    public sbyte AsSByte() {
      int v = this.AsInt32();
      if (v > SByte.MaxValue || v < SByte.MinValue) {
        throw new OverflowException("This object's value is out of range");
      }
      return (sbyte)v;
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
   if (outputStream == null) {
  throw new ArgumentNullException(nameof(outputStream));
}
      return WriteValue(outputStream, majorType, (long)value);
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
    /// <exception cref='T:System.ArgumentException'>The parameter
    /// <paramref name='majorType'/> is 7 and value is greater than
    /// 255.</exception>
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
  throw new ArgumentException("majorType (" + majorType +
    ") is less than 0");
}
if (majorType > 7) {
  throw new ArgumentException("majorType (" + majorType +
    ") is more than 7");
}
        if (majorType == 7) {
   throw new ArgumentException("majorType is 7 and value is greater than 255");
        }
        byte[] bytes = new[] { (byte)(27 | (majorType << 5)), (byte)((value >>
          56) & 0xff),
        (byte)((value >> 48) & 0xff), (byte)((value >> 40) & 0xff),
        (byte)((value >> 32) & 0xff), (byte)((value >> 24) & 0xff),
        (byte)((value >> 16) & 0xff), (byte)((value >> 8) & 0xff),
        (byte)(value & 0xff) };
        outputStream.Write(bytes, 0, bytes.Length);
        return bytes.Length;
      }
    }

    private static EInteger DecimalToEInteger(decimal dec) {
      return ((EDecimal)dec).ToEInteger();
    }

    private static decimal ExtendedRationalToDecimal(ERational
      extendedNumber) {
      return (decimal)extendedNumber;
    }

    private static decimal ExtendedDecimalToDecimal(EDecimal
      extendedNumber) {
 return (decimal)extendedNumber;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsDecimal"]/*'/>
    [CLSCompliant(false)]
    public decimal AsDecimal() {
      return (this.ItemType == CBORObjectTypeInteger) ?
        ((decimal)(long)this.ThisItem) : ((this.ItemType ==
        CBORObjectTypeExtendedRational) ?
        ExtendedRationalToDecimal((ERational)this.ThisItem) :
        ExtendedDecimalToDecimal(this.AsEDecimal()));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.AsUInt64"]/*'/>
    [CLSCompliant(false)]
    public ulong AsUInt64() {
      ICBORNumber cn = NumberInterfaces[this.ItemType];
      if (cn == null) {
        throw new InvalidOperationException("Not a number type");
      }
      EInteger bigint = cn.AsEInteger(this.ThisItem);
      if (bigint.Sign < 0 || bigint.GetSignedBitLength() > 64) {
        throw new OverflowException("This object's value is out of range");
      }
             return (ulong)bigint;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(System.SByte,System.IO.Stream)"]/*'/>
    [CLSCompliant(false)]
    public static void Write(sbyte value, Stream stream) {
      Write((long)value, stream);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(System.UInt64,System.IO.Stream)"]/*'/>
    [CLSCompliant(false)]
    public static void Write(ulong value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      if (value <= Int64.MaxValue) {
        Write((long)value, stream);
      } else {
        stream.WriteByte((byte)27);
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.Decimal)"]/*'/>
    public static CBORObject FromObject(decimal value) {
      return FromObject((EDecimal)value);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(System.UInt32,System.IO.Stream)"]/*'/>
    [CLSCompliant(false)]
    public static void Write(uint value, Stream stream) {
      Write((ulong)value, stream);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.Write(System.UInt16,System.IO.Stream)"]/*'/>
    [CLSCompliant(false)]
    public static void Write(ushort value, Stream stream) {
      Write((ulong)value, stream);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.SByte)"]/*'/>
    [CLSCompliant(false)]
    public static CBORObject FromObject(sbyte value) {
      return FromObject((long)value);
    }

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
      data[8] = (byte)0;
      return EInteger.FromBytes(data, true);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.UInt64)"]/*'/>
    [CLSCompliant(false)]
    public static CBORObject FromObject(ulong value) {
      return CBORObject.FromObject(UInt64ToEInteger(value));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.UInt32)"]/*'/>
    [CLSCompliant(false)]
    public static CBORObject FromObject(uint value) {
      return FromObject((long)(Int64)value);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObject(System.UInt16)"]/*'/>
    [CLSCompliant(false)]
    public static CBORObject FromObject(ushort value) {
      return FromObject((long)(Int64)value);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.FromObjectAndTag(System.Object,System.UInt64)"]/*'/>
    [CLSCompliant(false)]
    public static CBORObject FromObjectAndTag(Object o, ulong tag) {
      return FromObjectAndTag(o, UInt64ToEInteger(tag));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.op_Addition(PeterO.Cbor.CBORObject,PeterO.Cbor.CBORObject)"]/*'/>
    public static CBORObject operator +(CBORObject a, CBORObject b) {
      return Addition(a, b);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.op_Subtraction(PeterO.Cbor.CBORObject,PeterO.Cbor.CBORObject)"]/*'/>
    public static CBORObject operator -(CBORObject a, CBORObject b) {
      return Subtract(a, b);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.op_Multiply(PeterO.Cbor.CBORObject,PeterO.Cbor.CBORObject)"]/*'/>
    public static CBORObject operator *(CBORObject a, CBORObject b) {
      return Multiply(a, b);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.op_Division(PeterO.Cbor.CBORObject,PeterO.Cbor.CBORObject)"]/*'/>
    public static CBORObject operator /(CBORObject a, CBORObject b) {
      return Divide(a, b);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORObject.op_Modulus(PeterO.Cbor.CBORObject,PeterO.Cbor.CBORObject)"]/*'/>
    public static CBORObject operator %(CBORObject a, CBORObject b) {
      return Remainder(a, b);
    }
  }
}
