/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.IO;
using PeterO;

namespace PeterO.Cbor {
  // Contains extra methods placed separately
  // because they are not CLS-compliant or they
  // are specific to the .NET framework.
  public sealed partial class CBORObject
  {
    /// <returns>A 16-bit unsigned integer.</returns>
    /// <summary>Converts this object to a 16-bit unsigned integer. The
    /// return value will be truncated as necessary.</summary>
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

    /// <returns>A 32-bit unsigned integer.</returns>
    /// <summary>Converts this object to a 32-bit unsigned integer. The
    /// return value will be truncated as necessary.</summary>
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

    /// <returns>An 8-bit signed integer.</returns>
    /// <summary>Converts this object to an 8-bit signed integer.</summary>
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
        throw new ArgumentException("scale (" + Convert.ToString((int)scale, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (scale > 28) {
        throw new ArgumentException("scale (" + Convert.ToString((int)scale, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + "28");
      }
      byte[] data = bigmant.ToByteArray();
      int a = 0;
      int b = 0;
      int c = 0;
      for (int i = 0; i < Math.Min(4, data.Length); ++i) {
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
      return new Decimal(new int[] { a, b, c, d });
    }

    private static readonly BigInteger DecimalMaxValue = (BigInteger.One << 96) - BigInteger.One;
    private static readonly BigInteger DecimalMinValue = -((BigInteger.One << 96) - BigInteger.One);

    private static decimal BigIntegerToDecimal(BigInteger bi) {
      if (bi.Sign < 0) {
        if (bi.CompareTo(DecimalMinValue) < 0) {
          throw new OverflowException();
        }
        bi = -bi;
        return EncodeDecimal(bi, 0, true);
      }
      if (bi.CompareTo(DecimalMaxValue) > 0) {
        throw new OverflowException();
      }
      return EncodeDecimal(bi, 0, false);
    }

    private static BigInteger DecimalToBigInteger(decimal dec) {
      int[] bits = Decimal.GetBits(dec);
      byte[] data = new byte[13];
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
      BigInteger bigint = new BigInteger((byte[])data);
      for (int i = 0; i < scale; ++i) {
        bigint /= (BigInteger)10;
      }
      if ((bits[3] >> 31) != 0) {
        bigint = -bigint;
      }
      return bigint;
    }

    private static decimal ExtendedRationalToDecimal(ExtendedRational extendedNumber) {
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

    private static decimal ExtendedDecimalToDecimal(ExtendedDecimal extendedNumber) {
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
      if (this.ItemType == CBORObjectTypeInteger) {
        return (decimal)(long)this.ThisItem;
  } else if (this.ItemType == CBORObjectTypeExtendedRational) {
        return ExtendedRationalToDecimal((ExtendedRational)this.ThisItem);
      } else {
        return ExtendedDecimalToDecimal(this.AsExtendedDecimal());
      }
    }

    /// <summary>Converts this object to a 64-bit unsigned integer. Floating
    /// point values are truncated to an integer.</summary>
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
      byte[] data = bigint.ToByteArray();
      int a = 0;
      int b = 0;
      for (int i = 0; i < Math.Min(4, data.Length); ++i) {
        a |= (((int)data[i]) & 0xff) << (i * 8);
      }
      for (int i = 4; i < Math.Min(8, data.Length); ++i) {
        b |= (((int)data[i]) & 0xff) << ((i - 4) * 8);
      }
      unchecked
      {
        ulong ret = (ulong)a;
        ret &= 0xFFFFFFFFL;
        ulong retb = (ulong)b;
        retb &= 0xFFFFFFFFL;
        ret |= retb << 32;
        return ret;
      }
    }

    /// <param name='value'>An 8-bit signed integer.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <summary>Writes an 8-bit signed integer in CBOR format to a data stream.</summary>
    [CLSCompliant(false)]
    public static void Write(sbyte value, Stream stream) {
      Write((long)value, stream);
    }

    /// <param name='value'>A 64-bit unsigned integer.</param>
    /// <param name='stream'>A writable data stream.</param>
    /// <summary>Writes a 64-bit unsigned integer in CBOR format to a data
    /// stream.</summary>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='stream'/> is null.</exception>
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
    /// <returns>A CBORObject object with the same value as the .NET decimal.</returns>
    /// <param name='value'>A Decimal object.</param>
    public static CBORObject FromObject(decimal value) {
      int[] bits = Decimal.GetBits(value);
      int scale = (bits[3] >> 16) & 0xff;
      if (scale == 0 && Math.Round(value) == value) {
        // This is an integer
        if (value >= 0 && value <= UInt64.MaxValue) {
          return FromObject((ulong)value);
        } else if (value >= Int64.MinValue && value <= Int64.MaxValue) {
          return FromObject((long)value);
        } else {
          return FromObject(DecimalToBigInteger(value));
        }
      }
      byte[] data = new byte[13];
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
      BigInteger mantissa = new BigInteger((byte[])data);
      bool negative = (bits[3] >> 31) != 0;
      if (negative) {
        mantissa = -mantissa;
      }
      return FromObjectAndTag(
        new CBORObject[] { FromObject(-scale),
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

    /// <summary>Converts a signed 8-bit integer to a CBOR object.</summary>
    /// <returns>A CBORObject object.</returns>
    /// <param name='value'>An 8-bit signed integer.</param>
    [CLSCompliant(false)]
    public static CBORObject FromObject(sbyte value) {
      return FromObject((long)value);
    }

    private static BigInteger UInt64ToBigInteger(ulong value) {
      byte[] data = new byte[9];
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
      return BigInteger.fromByteArray(data, true);
    }

    /// <param name='value'>A 64-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    /// <summary>Converts a 64-bit unsigned integer to a CBOR object.</summary>
    [CLSCompliant(false)]
    public static CBORObject FromObject(ulong value) {
      return CBORObject.FromObject(UInt64ToBigInteger(value));
    }

    /// <param name='value'>A 32-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    /// <summary>Converts a 32-bit unsigned integer to a CBOR object.</summary>
    [CLSCompliant(false)]
    public static CBORObject FromObject(uint value) {
      return FromObject((long)value);
    }

    /// <param name='value'>A 16-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    /// <summary>Converts a 16-bit unsigned integer to a CBOR object.</summary>
    [CLSCompliant(false)]
    public static CBORObject FromObject(ushort value) {
      return FromObject((long)value);
    }

    /// <summary>Generates a CBOR object from an arbitrary object and gives
    /// the resulting object a tag.</summary>
    /// <returns>A CBOR object where the object <paramref name='o'/> is
    /// converted to a CBOR object and given the tag <paramref name='tag'/>
    /// .</returns>
    /// <param name='o'>An arbitrary object.</param>
    /// <param name='tag'>A 64-bit unsigned integer.</param>
    [CLSCompliant(false)]
    public static CBORObject FromObjectAndTag(Object o, ulong tag) {
      return FromObjectAndTag(o, UInt64ToBigInteger(tag));
    }

    /// <summary>Adds a CBORObject object and a CBORObject object.</summary>
    /// <returns>The sum of the two objects.</returns>
    /// <param name='a'>A CBORObject object.</param>
    /// <param name='b'>A CBORObject object. (2).</param>
    public static CBORObject operator +(CBORObject a, CBORObject b) {
      return Addition(a, b);
    }

    /// <summary>Subtracts a CBORObject object from a CBORObject object.</summary>
    /// <returns>The difference of the two objects.</returns>
    /// <param name='a'>A CBORObject object.</param>
    /// <param name='b'>A CBORObject object. (2).</param>
    public static CBORObject operator -(CBORObject a, CBORObject b) {
      return Subtract(a, b);
    }

    /// <summary>Multiplies a CBORObject object by the value of a CBORObject
    /// object.</summary>
    /// <returns>The product of the two objects.</returns>
    /// <param name='a'>A CBORObject object.</param>
    /// <param name='b'>A CBORObject object. (2).</param>
    public static CBORObject operator *(CBORObject a, CBORObject b) {
      return Multiply(a, b);
    }

    /// <summary>Divides a CBORObject object by the value of a CBORObject
    /// object.</summary>
    /// <returns>The quotient of the two objects.</returns>
    /// <param name='a'>A CBORObject object.</param>
    /// <param name='b'>A CBORObject object. (2).</param>
    public static CBORObject operator /(CBORObject a, CBORObject b) {
      return Divide(a, b);
    }

    /// <summary>Finds the remainder that results when a CBORObject object
    /// is divided by the value of a CBORObject object.</summary>
    /// <returns>The remainder of the two objects.</returns>
    /// <param name='a'>A CBORObject object.</param>
    /// <param name='b'>A CBORObject object. (2).</param>
    public static CBORObject operator %(CBORObject a, CBORObject b) {
      return Remainder(a, b);
    }
  }
}
