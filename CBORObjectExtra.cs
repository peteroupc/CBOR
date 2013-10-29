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
using System.Numerics;
using System.Text;

namespace PeterO
{
	// Contains extra methods placed separately
	// because they are not CLS-compliant.
	public sealed partial class CBORObject
	{
		[CLSCompliant(false)]
		public ushort AsUInt16(){
			int v=AsInt32();
			if(v>UInt16.MaxValue || v<0)
				throw new OverflowException();
			return (ushort)v;
		}

		[CLSCompliant(false)]
		public uint AsUInt32(){
			ulong v=AsUInt64();
			if(v>UInt32.MaxValue)
				throw new OverflowException();
			return (uint)v;
		}

		[CLSCompliant(false)]
		public sbyte AsSByte(){
			int v=AsInt32();
			if(v>Byte.MaxValue || v<Byte.MinValue)
				throw new OverflowException();
			return (sbyte)v;
		}
		/// <summary>
		/// Converts this object to a 64-bit unsigned
		/// integer.  Floating point values are truncated
		/// to an integer.
		/// </summary>
		/// <returns>The closest big integer
		/// to this object.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// This object's type is not an integer
		/// or a floating-point number.</exception>
		/// <exception cref="System.OverflowException">
		/// This object's value exceeds the range of a 64-bit
		/// unsigned integer.</exception>
		[CLSCompliant(false)]
		public ulong AsUInt64(){
			if(this.ItemType== CBORObjectType_Integer){
				if((long)item<0)
					throw new OverflowException();
				return (ulong)(long)item;
			} else if(this.ItemType== CBORObjectType_BigInteger){
				if((BigInteger)item>UInt64.MaxValue || (BigInteger)item<0)
					throw new OverflowException();
				return (ulong)(BigInteger)item;
			} else if(this.ItemType== CBORObjectType_Single){
				if(Single.IsNaN((float)item) ||
				   (float)item>UInt64.MaxValue || (float)item<0)
					throw new OverflowException();
				return (ulong)(float)item;
			} else if(this.ItemType== CBORObjectType_Double){
				if(Double.IsNaN((double)item) ||
				   (double)item>UInt64.MinValue || (double)item<0)
					throw new OverflowException();
				return (ulong)(double)item;
			} else if(this.Tag==4 && ItemType== CBORObjectType_Array &&
			          this.Count==2){
				StringBuilder sb=new StringBuilder();
				sb.Append(this[1].IntegerToString());
				sb.Append("e");
				sb.Append(this[0].IntegerToString());
				BigInteger bi=BigInteger.Parse(
					sb.ToString(),
					NumberStyles.AllowLeadingSign|
					NumberStyles.AllowDecimalPoint|
					NumberStyles.AllowExponent,
					CultureInfo.InvariantCulture);
				if(bi>UInt64.MaxValue || bi<0)
					throw new OverflowException();
				return (ulong)bi;
			} else
				throw new InvalidOperationException("Not a number type");
		}
		

		[CLSCompliant(false)]
		public static void Write(sbyte value, Stream s){
			Write((long)value,s);
		}
		[CLSCompliant(false)]
		public static void Write(ulong value, Stream s){
			if(value<=Int64.MaxValue){
				Write((long)value,s);
			} else {
				s.WriteByte((byte)(27));
				s.WriteByte((byte)((value>>56)&0xFF));
				s.WriteByte((byte)((value>>48)&0xFF));
				s.WriteByte((byte)((value>>40)&0xFF));
				s.WriteByte((byte)((value>>32)&0xFF));
				s.WriteByte((byte)((value>>24)&0xFF));
				s.WriteByte((byte)((value>>16)&0xFF));
				s.WriteByte((byte)((value>>8)&0xFF));
				s.WriteByte((byte)(value&0xFF));
			}
		}
		[CLSCompliant(false)]
		public static void Write(uint value, Stream s){
			Write((ulong)value,s);
		}
		[CLSCompliant(false)]
		public static void Write(ushort value, Stream s){
			Write((ulong)value,s);
		}
		
		[CLSCompliant(false)]
		public static CBORObject FromObject(sbyte value){
			return FromObject((long)value);
		}
		[CLSCompliant(false)]
		public static CBORObject FromObject(ulong value){
			return FromObject((BigInteger)value);
		}
		[CLSCompliant(false)]
		public static CBORObject FromObject(uint value){
			return FromObject((long)value);
		}
		[CLSCompliant(false)]
		public static CBORObject FromObject(ushort value){
			return FromObject((long)value);
		}

		[CLSCompliant(false)]
		public static CBORObject FromObjectAndTag(Object o, ulong tag){
			return FromObjectAndTag(o,(BigInteger)tag);
		}
		
		
		// .NET-specific
		
		private static string DateTimeToString(DateTime bi){
			DateTime dt=bi.ToUniversalTime();
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			int year=dt.Year;
			int month=dt.Month;
			int day=dt.Day;
			int hour=dt.Hour;
			int minute=dt.Minute;
			int second=dt.Second;
			sb.Append(
				new char[]{
					(char)('0'+((year/1000)%10)),
					(char)('0'+((year/100)%10)),
					(char)('0'+((year/10)%10)),
					(char)('0'+((year)%10)),
					'-',
					(char)('0'+((month/10)%10)),
					(char)('0'+((month)%10)),
					'-',
					(char)('0'+((day/10)%10)),
					(char)('0'+((day)%10)),
					'T',
					(char)('0'+((hour/10)%10)),
					(char)('0'+((hour)%10)),
					':',
					(char)('0'+((minute/10)%10)),
					(char)('0'+((minute)%10)),
					':',
					(char)('0'+((second/10)%10)),
					(char)('0'+((second)%10))
				},0,19);
			int millisecond=dt.Millisecond;
			if(millisecond>0){
				sb.Append(
					new char[]{
						'.',
						(char)('0'+((millisecond/100)%10)),
						(char)('0'+((millisecond/10)%10)),
						(char)('0'+((millisecond)%10))
					},0,4);
			}
			sb.Append('Z');
			return sb.ToString();
		}
		
		public static CBORObject FromObject(DateTime value){
			return new CBORObject(CBORObjectType_TextString,0,0,
			                      DateTimeToString(value));
		}
		/// <summary>
		/// Writes a date and time in CBOR format to a data stream
		/// </summary>
		/// <param name="bi"></param>
		/// <param name="s"></param>
		public static void Write(DateTime bi, Stream stream){
			if((stream)==null)throw new ArgumentNullException("s");
			stream.WriteByte(0xC0);
			Write(DateTimeToString(bi),stream);
		}
		
	}
}
