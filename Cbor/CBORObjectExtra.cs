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

using System.Text;

namespace PeterO.Cbor {
  // Contains extra methods placed separately
  // because they are not CLS-compliant or they
  // are specific to the .NET framework.
  public sealed partial class CBORObject {
    /// <summary>Not documented yet.</summary>
    /// <returns>A 16-bit unsigned integer.</returns>
    [CLSCompliant(false)]
    public ushort AsUInt16() {
      int v = this.AsInt32();
      if (v > UInt16.MaxValue || v < 0) {
        throw new OverflowException("This object's value is out of range");
      }
      return (ushort)v;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit unsigned integer.</returns>
    [CLSCompliant(false)]
    public uint AsUInt32() {
      ulong v = this.AsUInt64();
      if (v > UInt32.MaxValue) {
        throw new OverflowException("This object's value is out of range");
      }
      return (uint)v;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A SByte object.</returns>
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
        throw new ArgumentException("scale (" + Convert.ToString((long)scale, System.Globalization.CultureInfo.InvariantCulture) + ") is not greater or equal to " + "0");
      }
      if (scale > 28) {
        throw new ArgumentException("scale (" + Convert.ToString((long)scale, System.Globalization.CultureInfo.InvariantCulture) + ") is not less or equal to " + "28");
      }
      byte[] data = bigmant.ToByteArray();
      int a = 0;
      int b = 0;
      int c = 0;
      for (int i = 0; i < Math.Min(4, data.Length); ++i) {
        a |= (((int)data[i]) & 0xFF) << (i * 8);
      }
      for (int i = 4; i < Math.Min(8, data.Length); ++i) {
        b |= (((int)data[i]) & 0xFF) << ((i - 4) * 8);
      }
      for (int i = 8; i < Math.Min(12, data.Length); ++i) {
        c |= (((int)data[i]) & 0xFF) << ((i - 8) * 8);
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
      data[0] = (byte)(bits[0] & 0xFF);
      data[1] = (byte)((bits[0] >> 8) & 0xFF);
      data[2] = (byte)((bits[0] >> 16) & 0xFF);
      data[3] = (byte)((bits[0] >> 24) & 0xFF);
      data[4] = (byte)(bits[1] & 0xFF);
      data[5] = (byte)((bits[1] >> 8) & 0xFF);
      data[6] = (byte)((bits[1] >> 16) & 0xFF);
      data[7] = (byte)((bits[1] >> 24) & 0xFF);
      data[8] = (byte)(bits[2] & 0xFF);
      data[9] = (byte)((bits[2] >> 8) & 0xFF);
      data[10] = (byte)((bits[2] >> 16) & 0xFF);
      data[11] = (byte)((bits[2] >> 24) & 0xFF);
      data[12] = 0;
      int scale = (bits[3] >> 16) & 0xFF;
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
            PrecisionContext.ForPrecisionAndRounding(29, Rounding.HalfEven).WithTraps(PrecisionContext.FlagOverflow))
          .RoundToBinaryPrecision(PrecisionContext.CliDecimal.WithTraps(PrecisionContext.FlagOverflow));
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
        ExtendedDecimal newDecimal = extendedNumber.RoundToBinaryPrecision(
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
      if (bigint.Sign < 0 || bigint.bitLength() >64) {
        throw new OverflowException("This object's value is out of range");
      }
      byte[] data = bigint.ToByteArray();
      int a = 0;
      int b = 0;
      for (int i = 0; i < Math.Min(4, data.Length); ++i) {
        a |= (((int)data[i]) & 0xFF) << (i * 8);
      }
      for (int i = 4; i < Math.Min(8, data.Length); ++i) {
        b |= (((int)data[i]) & 0xFF) << ((i - 4) * 8);
      }
      unchecked {
        ulong ret = (ulong)a;
        ret &= 0xFFFFFFFFL;
        ulong retb = (ulong)b;
        retb &= 0xFFFFFFFFL;
        ret |= retb << 32;
        return ret;
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A SByte object.</param>
    /// <param name='stream'>A writable data stream.</param>
    [CLSCompliant(false)]
    public static void Write(sbyte value, Stream stream) {
      Write((long)value, stream);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A 64-bit unsigned integer.</param>
    /// <param name='stream'>A writable data stream.</param>
    [CLSCompliant(false)]
    public static void Write(ulong value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      if (value <= Int64.MaxValue) {
        Write((long)value, stream);
      } else {
        stream.WriteByte((byte)27);
        stream.WriteByte((byte)((value >> 56) & 0xFF));
        stream.WriteByte((byte)((value >> 48) & 0xFF));
        stream.WriteByte((byte)((value >> 40) & 0xFF));
        stream.WriteByte((byte)((value >> 32) & 0xFF));
        stream.WriteByte((byte)((value >> 24) & 0xFF));
        stream.WriteByte((byte)((value >> 16) & 0xFF));
        stream.WriteByte((byte)((value >> 8) & 0xFF));
        stream.WriteByte((byte)(value & 0xFF));
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A Decimal object.</param>
    /// <returns>A CBORObject object.</returns>
    public static CBORObject FromObject(decimal value) {
      if (Math.Round(value) == value) {
        // This is an integer
        if (value >= 0 && value <= UInt64.MaxValue) {
          return FromObject((ulong)value);
        } else if (value >= Int64.MinValue && value <= Int64.MaxValue) {
          return FromObject((long)value);
        } else {
          return FromObject(DecimalToBigInteger(value));
        }
      } else {
        int[] bits = Decimal.GetBits(value);
        byte[] data = new byte[13];
        data[0] = (byte)(bits[0] & 0xFF);
        data[1] = (byte)((bits[0] >> 8) & 0xFF);
        data[2] = (byte)((bits[0] >> 16) & 0xFF);
        data[3] = (byte)((bits[0] >> 24) & 0xFF);
        data[4] = (byte)(bits[1] & 0xFF);
        data[5] = (byte)((bits[1] >> 8) & 0xFF);
        data[6] = (byte)((bits[1] >> 16) & 0xFF);
        data[7] = (byte)((bits[1] >> 24) & 0xFF);
        data[8] = (byte)(bits[2] & 0xFF);
        data[9] = (byte)((bits[2] >> 8) & 0xFF);
        data[10] = (byte)((bits[2] >> 16) & 0xFF);
        data[11] = (byte)((bits[2] >> 24) & 0xFF);
        data[12] = 0;
        BigInteger mantissa = new BigInteger((byte[])data);
        bool negative = (bits[3] >> 31) != 0;
        int scale = (bits[3] >> 16) & 0xFF;
        if (negative) {
          mantissa = -mantissa;
        }
        return FromObjectAndTag(
          new CBORObject[] { FromObject(-scale),
          FromObject(mantissa) },
          4);
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A 32-bit unsigned integer.</param>
    /// <param name='stream'>A writable data stream.</param>
    [CLSCompliant(false)]
    public static void Write(uint value, Stream stream) {
      Write((ulong)value, stream);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A 16-bit unsigned integer.</param>
    /// <param name='stream'>A writable data stream.</param>
    [CLSCompliant(false)]
    public static void Write(ushort value, Stream stream) {
      Write((ulong)value, stream);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A SByte object.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    public static CBORObject FromObject(sbyte value) {
      return FromObject((long)value);
    }

    private static BigInteger UInt64ToBigInteger(ulong value) {
      byte[] data = new byte[9];
      ulong uvalue = value;
      data[0] = (byte)(uvalue & 0xFF);
      data[1] = (byte)((uvalue >> 8) & 0xFF);
      data[2] = (byte)((uvalue >> 16) & 0xFF);
      data[3] = (byte)((uvalue >> 24) & 0xFF);
      data[4] = (byte)((uvalue >> 32) & 0xFF);
      data[5] = (byte)((uvalue >> 40) & 0xFF);
      data[6] = (byte)((uvalue >> 48) & 0xFF);
      data[7] = (byte)((uvalue >> 56) & 0xFF);
      data[8] = (byte)0;
      return BigInteger.fromByteArray(data, true);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A 64-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    public static CBORObject FromObject(ulong value) {
      return CBORObject.FromObject(UInt64ToBigInteger(value));
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A 32-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    public static CBORObject FromObject(uint value) {
      return FromObject((long)value);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A 16-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    public static CBORObject FromObject(ushort value) {
      return FromObject((long)value);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='o'>An arbitrary object.</param>
    /// <param name='tag'>A 64-bit unsigned integer.</param>
    /// <returns>A CBORObject object.</returns>
    [CLSCompliant(false)]
    public static CBORObject FromObjectAndTag(Object o, ulong tag) {
      return FromObjectAndTag(o, UInt64ToBigInteger(tag));
    }
  }
}
