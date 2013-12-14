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
//using System.Numerics;
using System.Text;
namespace PeterO {
  // Contains extra methods placed separately
  // because they are not CLS-compliant or they
  // are specific to the .NET framework.
  public sealed partial class CBORObject {
    /// <summary> </summary>
    /// <returns></returns>
    [CLSCompliant(false)]
    public ushort AsUInt16() {
      int v = AsInt32();
      if (v > UInt16.MaxValue || v < 0)
        throw new OverflowException("This object's value is out of range");
      return (ushort)v;
    }
    /// <summary> </summary>
    /// <returns></returns>
    [CLSCompliant(false)]
    public uint AsUInt32() {
      ulong v = AsUInt64();
      if (v > UInt32.MaxValue)
        throw new OverflowException("This object's value is out of range");
      return (uint)v;
    }
    /// <summary> </summary>
    /// <returns></returns>
    [CLSCompliant(false)]
    public sbyte AsSByte() {
      int v = AsInt32();
      if (v > SByte.MaxValue || v < SByte.MinValue)
        throw new OverflowException("This object's value is out of range");
      return (sbyte)v;
    }
    private static decimal EncodeDecimal(BigInteger bigmant,
                                         int scale, bool neg) {
      if((scale)<0)throw new ArgumentException("scale"+" not greater or equal to "+"0"+" ("+Convert.ToString((scale),System.Globalization.CultureInfo.InvariantCulture)+")");
if((scale)>28)throw new ArgumentException("scale"+" not less or equal to "+"28"+" ("+Convert.ToString((scale),System.Globalization.CultureInfo.InvariantCulture)+")");
      byte[] data=bigmant.ToByteArray();
      int a=0;
      int b=0;
      int c=0;
      for(int i=0;i<Math.Min(4,data.Length);i++){
        a|=(((int)data[i])&0xFF)<<((i)*8);
      }
      for(int i=4;i<Math.Min(8,data.Length);i++){
        b|=(((int)data[i])&0xFF)<<((i-4)*8);
      }
      for(int i=8;i<Math.Min(12,data.Length);i++){
        c|=(((int)data[i])&0xFF)<<((i-8)*8);
      }
      int d = (scale << 16);
      if (neg) d |= (1 << 31);
      return new Decimal(new int[] { a, b, c, d });
    }
    
    private static BigInteger DecimalMaxValue = (BigInteger.One<<96)-BigInteger.One;
    private static BigInteger DecimalMinValue = -((BigInteger.One<<96)-BigInteger.One);

    private static decimal BigIntegerToDecimal(BigInteger bi){
      if(bi.Sign<0){
        if(bi.CompareTo(DecimalMinValue)<0)
          throw new OverflowException();
        bi=-bi;
        return EncodeDecimal(bi,0,true);
      }
      if(bi.CompareTo(DecimalMaxValue)>0)
        throw new OverflowException();
      return EncodeDecimal(bi,0,false);
    }
    
    private static BigInteger DecimalToBigInteger(decimal dec){
      int[] bits=Decimal.GetBits(dec);
      byte[] data=new byte[13];
      data[0]=(byte)((bits[0])&0xFF);
      data[1]=(byte)((bits[0]>>8)&0xFF);
      data[2]=(byte)((bits[0]>>16)&0xFF);
      data[3]=(byte)((bits[0]>>24)&0xFF);
      data[4]=(byte)((bits[1])&0xFF);
      data[5]=(byte)((bits[1]>>8)&0xFF);
      data[6]=(byte)((bits[1]>>16)&0xFF);
      data[7]=(byte)((bits[1]>>24)&0xFF);
      data[8]=(byte)((bits[2])&0xFF);
      data[9]=(byte)((bits[2]>>8)&0xFF);
      data[10]=(byte)((bits[2]>>16)&0xFF);
      data[11]=(byte)((bits[2]>>24)&0xFF);
      data[12]=0;
      int scale=(bits[3]>>16)&0xFF;
      BigInteger bigint=new BigInteger((byte[])data);
      for(int i=0;i<scale;i++){
        bigint/=(BigInteger)10;
      }
      if((bits[3]>>31)!=0){
        bigint=-bigint;
      }
      return bigint;
    }
    
    private static decimal DecimalFractionToDecimal(DecimalFraction decfrac) {
      DecimalFraction newDecimal=decfrac.RoundToBinaryPrecision(PrecisionContext.CliDecimal);
      if(newDecimal==null)
        throw new OverflowException("This object's value is out of range");
      return EncodeDecimal(BigInteger.Abs(newDecimal.Mantissa),
                           -((int)(newDecimal.Exponent)),
                           newDecimal.Mantissa.Sign<0);
    }
    /// <summary> Converts this object to a .NET decimal. </summary>
    /// <returns> The closest big integer to this object.</returns>
    /// <exception cref='System.InvalidOperationException'> This object's
    /// type is not a number type. </exception>
    /// <exception cref='System.OverflowException'> This object's value
    /// exceeds the range of a .NET decimal.</exception>
    [CLSCompliant(false)]
    public decimal AsDecimal() {
      if (this.ItemType == CBORObjectType_Integer) {
        return (decimal)(long)this.ThisItem;
      } else if (this.ItemType == CBORObjectType_BigInteger) {
        if ((BigInteger)this.ThisItem > DecimalMaxValue ||
            (BigInteger)this.ThisItem < DecimalMinValue)
          throw new OverflowException("This object's value is out of range");
        return BigIntegerToDecimal((BigInteger)this.ThisItem);
      } else if (this.ItemType == CBORObjectType_Single) {
        if (Single.IsNaN((float)this.ThisItem) ||
            (float)this.ThisItem > (float)Decimal.MaxValue ||
            (float)this.ThisItem < (float)Decimal.MinValue)
          throw new OverflowException("This object's value is out of range");
        return (decimal)(float)this.ThisItem;
      } else if (this.ItemType == CBORObjectType_Double) {
        if (Double.IsNaN((double)this.ThisItem) ||
            (double)this.ThisItem > (double)Decimal.MaxValue ||
            (double)this.ThisItem < (double)Decimal.MinValue)
          throw new OverflowException("This object's value is out of range");
        return (decimal)(double)this.ThisItem;
      } else if (this.ItemType == CBORObjectType_DecimalFraction) {
        return DecimalFractionToDecimal((DecimalFraction)this.ThisItem);
      } else if (this.ItemType == CBORObjectType_BigFloat) {
        return DecimalFractionToDecimal(
          DecimalFraction.FromBigFloat((BigFloat)this.ThisItem));
      } else
        throw new InvalidOperationException("Not a number type");
    }
    /// <summary> Converts this object to a 64-bit unsigned integer. Floating
    /// point values are truncated to an integer. </summary>
    /// <returns> The closest big integer to this object.</returns>
    /// <exception cref='System.InvalidOperationException'> This object's
    /// type is not a number type. </exception>
    /// <exception cref='System.OverflowException'> This object's value
    /// exceeds the range of a 64-bit unsigned integer.</exception>
    [CLSCompliant(false)]
    public ulong AsUInt64() {
      if (this.ItemType == CBORObjectType_Integer) {
        if ((long)this.ThisItem < 0)
          throw new OverflowException("This object's value is out of range");
        return (ulong)(long)this.ThisItem;
      } else if (this.ItemType == CBORObjectType_BigInteger) {
        if (((BigInteger)this.ThisItem).CompareTo(UInt64MaxValue) > 0 ||
            ((BigInteger)this.ThisItem).Sign < 0)
          throw new OverflowException("This object's value is out of range");
        return (ulong)BigIntegerToDecimal((BigInteger)this.ThisItem);
      } else if (this.ItemType == CBORObjectType_Single) {
        float fltItem = (float)this.ThisItem;
        if (Single.IsNaN(fltItem))
          throw new OverflowException("This object's value is out of range");
        fltItem = (fltItem < 0) ? (float)Math.Ceiling(fltItem) : (float)Math.Floor(fltItem);
        if (fltItem >= 0 && fltItem <= UInt64.MaxValue)
          return (ulong)fltItem;
        throw new OverflowException("This object's value is out of range");
      } else if (this.ItemType == CBORObjectType_Double) {
        double fltItem = (double)this.ThisItem;
        if (Double.IsNaN(fltItem))
          throw new OverflowException("This object's value is out of range");
        fltItem = (fltItem < 0) ? Math.Ceiling(fltItem) : Math.Floor(fltItem);
        if (fltItem >= 0 && fltItem <= UInt64.MaxValue)
          return (ulong)fltItem;
        throw new OverflowException("This object's value is out of range");
      } else if (this.ItemType == CBORObjectType_DecimalFraction) {
        BigInteger bi = ((DecimalFraction)this.ThisItem).ToBigInteger();
        if (((BigInteger)this.ThisItem).CompareTo(UInt64MaxValue) > 0 ||
            bi.Sign < 0)
          throw new OverflowException("This object's value is out of range");
        return (ulong)BigIntegerToDecimal(bi);
      } else if (this.ItemType == CBORObjectType_BigFloat) {
        BigInteger bi = ((BigFloat)this.ThisItem).ToBigInteger();
        if (((BigInteger)this.ThisItem).CompareTo(UInt64MaxValue) > 0 ||
            bi.Sign < 0)
          throw new OverflowException("This object's value is out of range");
        return (ulong)BigIntegerToDecimal(bi);
      } else
        throw new InvalidOperationException("Not a number type");
    }
    /// <summary> </summary>
    /// <param name='value'>A SByte object.</param>
    /// <returns></returns>
    /// <param name='stream'>A Stream object.</param>
    [CLSCompliant(false)]
    public static void Write(sbyte value, Stream stream) {
      Write((long)value, stream);
    }
    /// <summary> </summary>
    /// <param name='value'>A 64-bit unsigned integer.</param>
    /// <returns></returns>
    /// <param name='stream'>A Stream object.</param>
    [CLSCompliant(false)]
    public static void Write(ulong value, Stream stream) {
      if((stream)==null)throw new ArgumentNullException("stream");
      if (value <= Int64.MaxValue) {
        Write((long)value, stream);
      } else {
        stream.WriteByte((byte)(27));
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
    /// <summary> </summary>
    /// <param name='value'>A Decimal object.</param>
    /// <returns></returns>
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
        byte[] data=new byte[13];
        data[0]=(byte)((bits[0])&0xFF);
        data[1]=(byte)((bits[0]>>8)&0xFF);
        data[2]=(byte)((bits[0]>>16)&0xFF);
        data[3]=(byte)((bits[0]>>24)&0xFF);
        data[4]=(byte)((bits[1])&0xFF);
        data[5]=(byte)((bits[1]>>8)&0xFF);
        data[6]=(byte)((bits[1]>>16)&0xFF);
        data[7]=(byte)((bits[1]>>24)&0xFF);
        data[8]=(byte)((bits[2])&0xFF);
        data[9]=(byte)((bits[2]>>8)&0xFF);
        data[10]=(byte)((bits[2]>>16)&0xFF);
        data[11]=(byte)((bits[2]>>24)&0xFF);
        data[12]=0;
        BigInteger mantissa=new BigInteger((byte[])data);
        bool negative = (bits[3] >> 31) != 0;
        int scale = (bits[3] >> 16) & 0xFF;
        if (negative) mantissa = -mantissa;
        return FromObjectAndTag(
          new CBORObject[]{
            FromObject(-scale),
            FromObject(mantissa)
          }, 4);
      }
    }
    /// <summary> </summary>
    /// <param name='value'>A 32-bit unsigned integer.</param>
    /// <returns></returns>
    /// <param name='stream'>A Stream object.</param>
    [CLSCompliant(false)]
    public static void Write(uint value, Stream stream) {
      Write((ulong)value, stream);
    }
    /// <summary> </summary>
    /// <param name='value'>A UInt16 object.</param>
    /// <returns></returns>
    /// <param name='stream'>A Stream object.</param>
    [CLSCompliant(false)]
    public static void Write(ushort value, Stream stream) {
      Write((ulong)value, stream);
    }
    /// <summary> </summary>
    /// <param name='value'>A SByte object.</param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public static CBORObject FromObject(sbyte value) {
      return FromObject((long)value);
    }
    /// <summary> </summary>
    /// <param name='value'>A 64-bit unsigned integer.</param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public static CBORObject FromObject(ulong value) {
      return FromObject(DecimalToBigInteger((decimal)value));
    }
    /// <summary> </summary>
    /// <param name='value'>A 32-bit unsigned integer.</param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public static CBORObject FromObject(uint value) {
      return FromObject((long)value);
    }
    /// <summary> </summary>
    /// <param name='value'>A UInt16 object.</param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public static CBORObject FromObject(ushort value) {
      return FromObject((long)value);
    }
    /// <summary> </summary>
    /// <param name='o'>An arbitrary object.</param>
    /// <param name='tag'>A 64-bit unsigned integer.</param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public static CBORObject FromObjectAndTag(Object o, ulong tag) {
      return FromObjectAndTag(o, DecimalToBigInteger((decimal)tag));
    }
    // .NET-specific
    private static string DateTimeToString(DateTime bi) {
      DateTime dt = bi.ToUniversalTime();
      int year = dt.Year;
      int month = dt.Month;
      int day = dt.Day;
      int hour = dt.Hour;
      int minute = dt.Minute;
      int second = dt.Second;
      int millisecond = dt.Millisecond;
      char[] charbuf = new char[millisecond > 0 ? 24 : 20];
      charbuf[0] = (char)('0' + ((year / 1000) % 10));
      charbuf[1] = (char)('0' + ((year / 100) % 10));
      charbuf[2] = (char)('0' + ((year / 10) % 10));
      charbuf[3] = (char)('0' + ((year) % 10));
      charbuf[4] = '-';
      charbuf[5] = (char)('0' + ((month / 10) % 10));
      charbuf[6] = (char)('0' + ((month) % 10));
      charbuf[7] = '-';
      charbuf[8] = (char)('0' + ((day / 10) % 10));
      charbuf[9] = (char)('0' + ((day) % 10));
      charbuf[10] = 'T';
      charbuf[11] = (char)('0' + ((hour / 10) % 10));
      charbuf[12] = (char)('0' + ((hour) % 10));
      charbuf[13] = ':';
      charbuf[14] = (char)('0' + ((minute / 10) % 10));
      charbuf[15] = (char)('0' + ((minute) % 10));
      charbuf[16] = ':';
      charbuf[17] = (char)('0' + ((second / 10) % 10));
      charbuf[18] = (char)('0' + ((second) % 10));
      if (millisecond > 0) {
        charbuf[19] = '.';
        charbuf[20] = (char)('0' + ((millisecond / 100) % 10));
        charbuf[21] = (char)('0' + ((millisecond / 10) % 10));
        charbuf[22] = (char)('0' + ((millisecond) % 10));
        charbuf[23] = 'Z';
      } else {
        charbuf[19] = 'Z';
      }
      return new String(charbuf);
    }
    /// <summary> </summary>
    /// <param name='value'> A DateTime object.</param>
    /// <returns></returns>
    public static CBORObject FromObject(DateTime value) {
      return new CBORObject(
        FromObject(DateTimeToString(value)), 0, 0);
    }
    /// <summary> Writes a date and time in CBOR format to a data stream. </summary>
    /// <param name='bi'> A DateTime object.</param>
    /// <returns></returns>
    /// <param name='stream'> A Stream object.</param>
    public static void Write(DateTime bi, Stream stream) {
      if ((stream) == null) throw new ArgumentNullException("stream");
      stream.WriteByte(0xC0);
      Write(DateTimeToString(bi), stream);
    }
  }
}