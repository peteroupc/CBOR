/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.IO;
using PeterO;

namespace PeterO.Cbor {
  // Contains extra methods placed separately
  // because they are not CLS-compliant or they
  // are specific to the .NET framework.
  public sealed partial class CBORObject {
    /// <summary>Converts this object to a 16-bit unsigned integer. The
    /// return value will be truncated as necessary.</summary>
    /// <returns>A 16-bit unsigned integer.</returns>
    /// <exception cref='OverflowException'>This object's value is outside
    /// the range of a 16-bit unsigned integer.</exception>
    [CLSCompliant(false)]
    public ushort AsUInt16() {
      int v = this.AsInt32();
      if (v > UInt16.MaxValue || v < 0) {
        throw new OverflowException("This object's value is out of range");
      }
      return (ushort)v;
    }

    /// <summary>Converts this object to a 32-bit unsigned integer. The
    /// return value will be truncated as necessary.</summary>
    /// <returns>A 32-bit unsigned integer.</returns>
    /// <exception cref='OverflowException'>This object's value is outside
    /// the range of a 32-bit unsigned integer.</exception>
    [CLSCompliant(false)]
    public uint AsUInt32() {
      ulong v = this.AsUInt64();
      if (v > UInt32.MaxValue) {
        throw new OverflowException("This object's value is out of range");
      }
      return (uint)v;
    }

    /// <summary>Converts this object to an 8-bit signed integer.</summary>
    /// <returns>An 8-bit signed integer.</returns>
    [CLSCompliant(false)]
    public sbyte AsSByte() {
      int v = this.AsInt32();
      if (v > SByte.MaxValue || v < SByte.MinValue) {
        throw new OverflowException("This object's value is out of range");
      }
      return (sbyte)v;
    }

    private static decimal EncodeDecimal(
BigInteger bigmant,
int scale,
bool neg) {
      if (scale < 0) {
        throw new ArgumentException(
"scale (" + scale + ") is less than 0");
      }
      if (scale > 28) {
        throw new ArgumentException(
"scale (" + scale + ") is more than " + "28");
      }
      byte[] data = bigmant.toBytes(true);
      var a = 0;
      var b = 0;
      var c = 0;
      for (var i = 0; i < Math.Min(4, data.Length); ++i) {
        a |= (((int)data[i]) & 0xff) << (i * 8);
      }
      for (int i = 4; i < Math.Min(8, data.Length); ++i) {
        b |= (((int)data[i]) & 0xff) << ((i - 4) * 8);
      }
      for (int i = 8; i < Math.Min(12, data.Length); ++i) {
        c |= (((int)data[i]) & 0xff) << ((i - 8) * 8);
      }
      int d = scale << 16;
      if (neg) {
        d |= 1 << 31;
      }
      return new Decimal(new[] { a, b, c, d });
    }

    private static readonly BigInteger DecimalMaxValue = (BigInteger.One <<
      96) - BigInteger.One;

    private static readonly BigInteger DecimalMinValue = -((BigInteger.One <<
      96) - BigInteger.One);

    private static BigInteger DecimalToBigInteger(decimal dec) {
      int[] bits = Decimal.GetBits(dec);
      var data = new byte[13];
      data[0] = (byte)(bits[0] & 0xff);
      data[1] = (byte)((bits[0] >> 8) & 0xff);
      data[2] = (byte)((bits[0] >> 16) & 0xff);
      data[3] = (byte)((bits[0] >> 24) & 0xff);
      data[4] = (byte)(bits[1] & 0xff);
      data[5] = (byte)((bits[1] >> 8) & 0xff);
      data[6] = (byte)((bits[1] >> 16) & 0xff);
      data[7] = (byte)((bits[1] >> 24) & 0xff);
      data[8] = (byte)(bits[2] & 0xff);
      data[9] = (byte)((bits[2] >> 8) & 0xff);
      data[10] = (byte)((bits[2] >> 16) & 0xff);
      data[11] = (byte)((bits[2] >> 24) & 0xff);
      data[12] = 0;
      int scale = (bits[3] >> 16) & 0xff;
      var bigint = BigInteger.fromBytes(data, true);
      for (var i = 0; i < scale; ++i) {
        bigint /= (BigInteger)10;
      }
      if ((bits[3] >> 31) != 0) {
        bigint = -bigint;
      }
      return bigint;
    }

    private static decimal ExtendedRationalToDecimal(ExtendedRational
      extendedNumber) {
      if (extendedNumber.IsInfinity() || extendedNumber.IsNaN()) {
        throw new OverflowException("This object's value is out of range");
      }
      try {
        ExtendedDecimal newDecimal =
          ExtendedDecimal.FromBigInteger(extendedNumber.Numerator)
          .Divide(
ExtendedDecimal.FromBigInteger(extendedNumber.Denominator),
PrecisionContext.CliDecimal.WithTraps(PrecisionContext.FlagOverflow));
        return EncodeDecimal(
BigInteger.Abs(newDecimal.Mantissa),
-((int)newDecimal.Exponent),
newDecimal.Mantissa.Sign < 0);
      } catch (TrapException ex) {
        throw new OverflowException("This object's value is out of range", ex);
      }
    }

    private static decimal ExtendedDecimalToDecimal(ExtendedDecimal
      extendedNumber) {
      if (extendedNumber.IsInfinity() || extendedNumber.IsNaN()) {
        throw new OverflowException("This object's value is out of range");
      }
      try {
        ExtendedDecimal newDecimal = extendedNumber.RoundToPrecision(
          PrecisionContext.CliDecimal.WithTraps(PrecisionContext.FlagOverflow));
        return EncodeDecimal(
BigInteger.Abs(newDecimal.Mantissa),
-((int)newDecimal.Exponent),
newDecimal.Mantissa.Sign < 0);
      } catch (TrapException ex) {
        throw new OverflowException("This object's value is out of range", ex);
      }
    }

    /// <summary>Converts this object to a .NET decimal.</summary>
    /// <returns>The closest big integer to this object.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    /// <exception cref='System.OverflowException'>This object's value
    /// exceeds the range of a .NET decimal.</exception>
    [CLSCompliant(false)]
    public decimal AsDecimal() {
      return (this.ItemType == CBORObjectTypeInteger) ?
        ((decimal)(long)this.ThisItem) : ((this.ItemType ==
        CBORObjectTypeExtendedRational) ?
        ExtendedRationalToDecimal((ExtendedRational)this.ThisItem) :
        ExtendedDecimalToDecimal(this.AsExtendedDecimal()));
    }

    /// <summary>Converts this object to a 64-bit unsigned integer.
    /// Floating point values are truncated to an integer.</summary>
    /// <returns>The closest big integer to this object.</returns>
    /// <exception cref='System.InvalidOperationException'>This object's
    /// type is not a number type.</exception>
    /// <exception cref='System.OverflowException'>This object's value
    /// exceeds the range of a 64-bit unsigned integer.</exception>
    [CLSCompliant(false)]
    public ulong AsUInt64() {
      BigInteger bigint = this.AsBigInteger();
      if (bigint.Sign < 0 || bigint.bitLength() > 64) {
        throw new OverflowException("This object's value is out of range");
      }
      byte[] data = bigint.toBytes(true);
      var a = 0;
      var b = 0;
      for (var i = 0; i < Math.Min(4, data.Length); ++i) {
        a |= (((int)data[i]) & 0xff) << (i * 8);
      }
      for (int i = 4; i < Math.Min(8, data.Length); ++i) {
        b |= (((int)data[i]) & 0xff) << ((i - 4) * 8);
      }
      unchecked
      {
        var ret = (ulong)a;
        ret &= 0xFFFFFFFFL;
        var retb = (ulong)b;
        retb &= 0xFFFFFFFFL;
        ret |= retb << 32;
        return ret;
      }
    }

    /// <summary>Writes an 8-bit signed integer in CBOR format to a data
    /// stream.</summary>
    /// <param name='value'>An 8-bit signed integer.</param>
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
        throw new ArgumentNullException("stream");
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

    /// <summary>Converts a .NET decimal to a CBOR object.</summary>
    /// <param name='value'>A Decimal object.</param>
    /// <returns>A CBORObject object with the same value as the .NET
    /// decimal.</returns>
    public static CBORObject FromObject(decimal value) {
      int[] bits = Decimal.GetBits(value);
      int scale = (bits[3] >> 16) & 0xff;
      if (scale == 0 && Math.Round(value) == value) {
        // This is an integer
        return (value >= 0 && value <= UInt64.MaxValue) ?
          FromObject((ulong)value) : ((value >= Int64.MinValue && value <=
          Int64.MaxValue) ? FromObject((long)value) :
          FromObject(DecimalToBigInteger(value)));
      }
      var data = new byte[13];
      data[0] = (byte)(bits[0] & 0xff);
      data[1] = (byte)((bits[0] >> 8) & 0xff);
      data[2] = (byte)((bits[0] >> 16) & 0xff);
      data[3] = (byte)((bits[0] >> 24) & 0xff);
      data[4] = (byte)(bits[1] & 0xff);
      data[5] = (byte)((bits[1] >> 8) & 0xff);
      data[6] = (byte)((bits[1] >> 16) & 0xff);
      data[7] = (byte)((bits[1] >> 24) & 0xff);
      data[8] = (byte)(bits[2] & 0xff);
      data[9] = (byte)((bits[2] >> 8) & 0xff);
      data[10] = (byte)((bits[2] >> 16) & 0xff);
      data[11] = (byte)((bits[2] >> 24) & 0xff);
      data[12] = 0;
      var mantissa = BigInteger.fromBytes(data, true);
      bool negative = (bits[3] >> 31) != 0;
      if (negative) {
        mantissa = -mantissa;
      }
      return FromObjectAndTag(
new[] { FromObject(-scale),
        FromObject(mantissa) },
 4);
    }

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
    /// <param name='value'>An 8-bit signed integer.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    public static CBORObject FromObject(sbyte value) {
      return FromObject((long)value);
    }

    private static BigInteger UInt64ToBigInteger(ulong value) {
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
      return BigInteger.fromBytes(data, true);
    }

    /// <summary>Converts a 64-bit unsigned integer to a CBOR
    /// object.</summary>
    /// <param name='value'>A 64-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    public static CBORObject FromObject(ulong value) {
      return CBORObject.FromObject(UInt64ToBigInteger(value));
    }

    /// <summary>Converts a 32-bit unsigned integer to a CBOR
    /// object.</summary>
    /// <param name='value'>A 32-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    public static CBORObject FromObject(uint value) {
      return FromObject((long)value);
    }

    /// <summary>Converts a 16-bit unsigned integer to a CBOR
    /// object.</summary>
    /// <param name='value'>A 16-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    public static CBORObject FromObject(ushort value) {
      return FromObject((long)value);
    }

    /// <summary>Generates a CBOR object from an arbitrary object and gives
    /// the resulting object a tag.</summary>
    /// <param name='o'>An arbitrary object.</param>
    /// <param name='tag'>A 64-bit unsigned integer.</param>
    /// <returns>A CBOR object where the object <paramref name='o'/> is
    /// converted to a CBOR object and given the tag <paramref name='tag'/>
    /// .</returns>
    [CLSCompliant(false)]
    public static CBORObject FromObjectAndTag(Object o, ulong tag) {
      return FromObjectAndTag(o, UInt64ToBigInteger(tag));
    }

    /// <summary>Adds a CBORObject object and a CBORObject
    /// object.</summary>
    /// <param name='a'>A CBORObject object.</param>
    /// <param name='b'>Another CBORObject object.</param>
    /// <returns>The sum of the two objects.</returns>
    public static CBORObject operator +(CBORObject a, CBORObject b) {
      return Addition(a, b);
    }

    /// <summary>Subtracts a CBORObject object from a CBORObject
    /// object.</summary>
    /// <param name='a'>A CBORObject object.</param>
    /// <param name='b'>Another CBORObject object.</param>
    /// <returns>The difference of the two objects.</returns>
    public static CBORObject operator -(CBORObject a, CBORObject b) {
      return Subtract(a, b);
    }

    /// <summary>Multiplies a CBORObject object by the value of a
    /// CBORObject object.</summary>
    /// <param name='a'>A CBORObject object.</param>
    /// <param name='b'>Another CBORObject object.</param>
    /// <returns>The product of the two objects.</returns>
    public static CBORObject operator *(CBORObject a, CBORObject b) {
      return Multiply(a, b);
    }

    /// <summary>Divides a CBORObject object by the value of a CBORObject
    /// object.</summary>
    /// <param name='a'>A CBORObject object.</param>
    /// <param name='b'>Another CBORObject object.</param>
    /// <returns>The quotient of the two objects.</returns>
    public static CBORObject operator /(CBORObject a, CBORObject b) {
      return Divide(a, b);
    }

    /// <summary>Finds the remainder that results when a CBORObject object
    /// is divided by the value of a CBORObject object.</summary>
    /// <param name='a'>A CBORObject object.</param>
    /// <param name='b'>Another CBORObject object.</param>
    /// <returns>The remainder of the two objects.</returns>
    public static CBORObject operator %(CBORObject a, CBORObject b) {
      return Remainder(a, b);
    }
  }
}
