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
	/// <summary>
	/// Represents an object in Concise Binary Object
	/// Representation (CBOR) and contains methods for reading
	/// and writing CBOR data.  CBOR is defined in
	/// RFC 7049.
	/// </summary>
	public sealed class CBORObject
	{
		enum CBORObjectType {
			UInteger,
			SInteger,
			BigInteger, // Big integer decoded from a byte array
			BigIntegerType1, // Big integer decoded from major type 1
			ByteString,
			TextString,
			Array,
			Map,
			SimpleValue,
			Single,
			Double
		}
		
		Object item;
		CBORObjectType itemtype;
		bool tagged=false;
		ulong tag=0;
		
		private static Encoding utf8=new UTF8Encoding(false,true);
		
		public static readonly CBORObject Break=new CBORObject(CBORObjectType.SimpleValue,31);
		public static readonly CBORObject False=new CBORObject(CBORObjectType.SimpleValue,20);
		public static readonly CBORObject True=new CBORObject(CBORObjectType.SimpleValue,21);
		public static readonly CBORObject Null=new CBORObject(CBORObjectType.SimpleValue,22);
		public static readonly CBORObject Undefined=new CBORObject(CBORObjectType.SimpleValue,23);
		private CBORObject(CBORObjectType type, ulong tag, Object item){
			this.itemtype=type;
			this.tag=tag;
			this.tagged=true;
			this.item=item;
		}
		private CBORObject(CBORObjectType type, Object item){
			this.itemtype=type;
			this.item=item;
		}
		
		public bool IsBreak {
			get {
				return itemtype==CBORObjectType.SimpleValue && (int)item==31;
			}
		}
		public bool IsTrue {
			get {
				return itemtype==CBORObjectType.SimpleValue && (int)item==21;
			}
		}
		public bool IsFalse {
			get {
				return itemtype==CBORObjectType.SimpleValue && (int)item==20;
			}
		}
		public bool IsNull {
			get {
				return itemtype==CBORObjectType.SimpleValue && (int)item==22;
			}
		}
		public bool IsUndefined {
			get {
				return itemtype==CBORObjectType.SimpleValue && (int)item==23;
			}
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			CBORObject other = obj as CBORObject;
			if (other == null)
				return false;
			return object.Equals(this.item, other.item) && this.itemtype == other.itemtype && this.tagged == other.tagged && this.tag == other.tag;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (item != null)
					hashCode += 1000000007 * item.GetHashCode();
				hashCode += 1000000009 * itemtype.GetHashCode();
				hashCode += 1000000021 * tagged.GetHashCode();
				hashCode += 1000000033 * tag.GetHashCode();
			}
			return hashCode;
		}
		#endregion

		
		public static CBORObject FromBytes(byte[] data){
			using(MemoryStream ms=new MemoryStream(data)){
				CBORObject o=Read(ms);
				if(ms.Position!=ms.Length)
					throw new FormatException();
				return o;
			}
		}
		public int Count {
			get {
				if(itemtype== CBORObjectType.Array){
					return ((IList<CBORObject>)item).Count;
				} else if(itemtype== CBORObjectType.Map){
					return ((IList<CBORObject[]>)item).Count;
				} else {
					return 0;
				}
			}
		}
		
		/// <summary>
		/// Gets whether this data item has a tag.
		/// </summary>
		public bool IsTagged {
			get {
				return tagged;
			}
		}

		/// <summary>
		/// Gets the tag for this CBOR data item,
		/// or 0 if the item is untagged.
		/// </summary>
		public BigInteger Tag {
			get {
				return (tagged) ? tag : 0;
			}
		}

		/// <summary>
		/// Gets or sets the value of a CBOR object in this
		/// array.
		/// </summary>
		public CBORObject this[int index]{
			get {
				if(itemtype== CBORObjectType.Array){
					return ((IList<CBORObject>)item)[index];
				} else {
					throw new InvalidOperationException("Not an array");
				}
			}
			set {
				if(itemtype== CBORObjectType.Array){
					if(this.Tag==4)
						throw new InvalidOperationException("Read-only array");
					((IList<CBORObject>)item)[index]=value;
				} else {
					throw new InvalidOperationException("Not an array");
				}
			}
		}
		
		/// <summary>
		/// Converts this object to a 64-bit floating point
		/// number.
		/// </summary>
		/// <returns>The closest 64-bit floating point number
		/// to this object.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// This object's type is not an integer
		/// or a floating-point number.</exception>
		public double AsDouble(){
			if(itemtype== CBORObjectType.UInteger)
				return (double)(ulong)item;
			else if(itemtype== CBORObjectType.SInteger)
				return (double)(long)item;
			else if(itemtype== CBORObjectType.BigInteger || itemtype== CBORObjectType.BigIntegerType1)
				return (double)(BigInteger)item;
			else if(itemtype== CBORObjectType.Single)
				return (double)(float)item;
			else if(itemtype== CBORObjectType.Double)
				return (double)item;
			else if(this.Tag==4 && itemtype== CBORObjectType.Array &&
			        this.Count==2){
				StringBuilder sb=new StringBuilder();
				sb.Append(this[1].IntegerToString());
				sb.Append("e");
				sb.Append(this[0].IntegerToString());
				return Double.Parse(sb.ToString(),
				                    NumberStyles.AllowLeadingSign|
				                    NumberStyles.AllowDecimalPoint|
				                    NumberStyles.AllowExponent,
				                    CultureInfo.InvariantCulture);
			}
			else
				throw new InvalidOperationException("Not a number type");
		}
		

		/// <summary>
		/// Converts this object to a 32-bit floating point
		/// number.
		/// </summary>
		/// <returns>The closest 32-bit floating point number
		/// to this object.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// This object's type is not an integer
		/// or a floating-point number.</exception>
		public float AsSingle(){
			if(itemtype== CBORObjectType.UInteger)
				return (float)(ulong)item;
			else if(itemtype== CBORObjectType.SInteger)
				return (float)(long)item;
			else if(itemtype== CBORObjectType.BigInteger || itemtype== CBORObjectType.BigIntegerType1)
				return (float)(BigInteger)item;
			else if(itemtype== CBORObjectType.Single)
				return (float)item;
			else if(itemtype== CBORObjectType.Double)
				return (float)(double)item;
			else if(this.Tag==4 && itemtype== CBORObjectType.Array &&
			        this.Count==2){
				StringBuilder sb=new StringBuilder();
				sb.Append(this[1].IntegerToString());
				sb.Append("e");
				sb.Append(this[0].IntegerToString());
				return Single.Parse(sb.ToString(),
				                    NumberStyles.AllowLeadingSign|
				                    NumberStyles.AllowDecimalPoint|
				                    NumberStyles.AllowExponent,
				                    CultureInfo.InvariantCulture);
			}
			else
				throw new InvalidOperationException("Not a number type");
		}

		/// <summary>
		/// Converts this object to an arbitrary-length
		/// integer.  Floating point values are truncated
		/// to an integer.
		/// </summary>
		/// <returns>The closest big integer
		/// to this object.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// This object's type is not an integer
		/// or a floating-point number.</exception>
		public BigInteger AsBigInteger(){
			if(itemtype== CBORObjectType.UInteger)
				return (BigInteger)(ulong)item;
			else if(itemtype== CBORObjectType.SInteger)
				return (BigInteger)(long)item;
			else if(itemtype== CBORObjectType.BigInteger || itemtype== CBORObjectType.BigIntegerType1)
				return (BigInteger)item;
			else if(itemtype== CBORObjectType.Single)
				return (BigInteger)(float)item;
			else if(itemtype== CBORObjectType.Double)
				return (BigInteger)(double)item;
			else if(this.Tag==4 && itemtype== CBORObjectType.Array &&
			        this.Count==2){
				StringBuilder sb=new StringBuilder();
				sb.Append(this[1].IntegerToString());
				sb.Append("e");
				sb.Append(this[0].IntegerToString());
				return BigInteger.Parse(sb.ToString(),
				                        NumberStyles.AllowLeadingSign|
				                        NumberStyles.AllowDecimalPoint|
				                        NumberStyles.AllowExponent,
				                        CultureInfo.InvariantCulture);
			}
			else
				throw new InvalidOperationException("Not a number type");
		}
		
		public bool AsBoolean(){
			if(this.IsTrue)
				return true;
			return false;
		}
		
		public short AsInt16(){
			int v=AsInt32();
			if(v>Int16.MaxValue || v<Int16.MinValue)
				throw new OverflowException();
			return (short)v;
		}
		
		public short AsByte(){
			int v=AsInt32();
			if(v>Byte.MaxValue || v<Byte.MinValue)
				throw new OverflowException();
			return (byte)v;
		}

		[CLSCompliant(false)]
		public ushort AsUInt16(){
			int v=AsInt32();
			if(v>UInt16.MaxValue || v<UInt16.MinValue)
				throw new OverflowException();
			return (ushort)v;
		}

		[CLSCompliant(false)]
		public uint AsUInt32(){
			int v=AsUInt64();
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
		
		private static bool IsValidString(string s){
			if(s==null)return false;
			for(int i=0;i<s.Length;i++){
				int c=s[i];
				if(c<=0xD7FF || c>=0xE000) {
					continue;
				} else if(c<=0xDBFF){ // UTF-16 low surrogate
					i++;
					if(i>=s.Length || s[i]<0xDC00 || s[i]>0xDFFF)
						return false;
				} else
					return false;
			}
			return true;
		}

		
		/// <summary>
		/// Converts this object to a 64-bit signed
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
		/// signed integer.</exception>
		[CLSCompliant(false)]
		public ulong AsUInt64(){
			if(itemtype== CBORObjectType.UInteger){
				if((ulong)item>UInt64.MaxValue)
					throw new OverflowException();
				return (ulong)item;
			} else if(itemtype== CBORObjectType.SInteger){
				if((long)item<0)
					throw new OverflowException();
				return (ulong)(long)item;
			} else if(itemtype== CBORObjectType.BigInteger || itemtype== CBORObjectType.BigIntegerType1){
				if((BigInteger)item>UInt64.MaxValue || (BigInteger)item<UInt64.MinValue)
					throw new OverflowException();
				return (ulong)(BigInteger)item;
			} else if(itemtype== CBORObjectType.Single){
				if(Single.IsNaN((float)item) ||
				   (float)item>UInt64.MaxValue || (float)item<UInt64.MinValue)
					throw new OverflowException();
				return (ulong)(float)item;
			} else if(itemtype== CBORObjectType.Double){
				if(Double.IsNaN((double)item) ||
				   (double)item>UInt64.MinValue || (double)item<UInt64.MinValue)
					throw new OverflowException();
				return (ulong)(double)item;
			} else if(this.Tag==4 && itemtype== CBORObjectType.Array &&
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
				if(bi>UInt64.MaxValue || bi<UInt64.MinValue)
					throw new OverflowException();
				return (ulong)bi;
			} else
				throw new InvalidOperationException("Not a number type");
		}
		
		/// <summary>
		/// Converts this object to a 64-bit signed
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
		/// signed integer.</exception>
		public long AsInt64(){
			if(itemtype== CBORObjectType.UInteger){
				if((ulong)item>Int64.MaxValue)
					throw new OverflowException();
				return (long)(ulong)item;
			} else if(itemtype== CBORObjectType.SInteger){
				return (long)item;
			} else if(itemtype== CBORObjectType.BigInteger || itemtype== CBORObjectType.BigIntegerType1){
				if((BigInteger)item>Int64.MaxValue || (BigInteger)item<Int64.MinValue)
					throw new OverflowException();
				return (long)(BigInteger)item;
			} else if(itemtype== CBORObjectType.Single){
				if(Single.IsNaN((float)item) ||
				   (float)item>Int64.MaxValue || (float)item<Int64.MinValue)
					throw new OverflowException();
				return (long)(float)item;
			} else if(itemtype== CBORObjectType.Double){
				if(Double.IsNaN((double)item) ||
				   (double)item>Int64.MinValue || (double)item<Int64.MinValue)
					throw new OverflowException();
				return (long)(double)item;
			} else if(this.Tag==4 && itemtype== CBORObjectType.Array &&
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
				if(bi>Int64.MaxValue || bi<Int64.MinValue)
					throw new OverflowException();
				return (long)bi;
			} else
				throw new InvalidOperationException("Not a number type");
		}
		
		/// <summary>
		/// Converts this object to a 32-bit signed
		/// integer.  Floating point values are truncated
		/// to an integer.
		/// </summary>
		/// <returns>The closest big integer
		/// to this object.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// This object's type is not an integer
		/// or a floating-point number.</exception>
		/// <exception cref="System.OverflowException">
		/// This object's value exceeds the range of a 32-bit
		/// signed integer.</exception>
		public int AsInt32(){
			if(itemtype== CBORObjectType.UInteger){
				if((ulong)item>Int32.MaxValue)
					throw new OverflowException();
				return (int)(ulong)item;
			} else if(itemtype== CBORObjectType.SInteger){
				if((long)item>Int32.MaxValue || (long)item<Int32.MinValue)
					throw new OverflowException();
				return (int)(long)item;
			} else if(itemtype== CBORObjectType.BigInteger || itemtype== CBORObjectType.BigIntegerType1){
				if((BigInteger)item>Int32.MaxValue || (BigInteger)item<Int32.MinValue)
					throw new OverflowException();
				return (int)(BigInteger)item;
			} else if(itemtype== CBORObjectType.Single){
				if(Single.IsNaN((float)item) ||
				   (float)item>Int32.MaxValue || (float)item<Int32.MinValue)
					throw new OverflowException();
				return (int)(float)item;
			} else if(itemtype== CBORObjectType.Double){
				if(Double.IsNaN((double)item) ||
				   (double)item>Int32.MinValue || (double)item<Int32.MinValue)
					throw new OverflowException();
				return (int)(double)item;
			} else if(this.Tag==4 && itemtype== CBORObjectType.Array &&
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
				if(bi>Int32.MaxValue || bi<Int32.MinValue)
					throw new OverflowException();
				return (int)bi;
			} else
				throw new InvalidOperationException("Not a number type");
		}
		public string AsString(){
			if(itemtype== CBORObjectType.TextString){
				return (string)item;
			} else {
				throw new InvalidOperationException("Not a string type");
			}
		}
		/// <summary>
		/// Reads an object in CBOR format from a data
		/// stream.
		/// </summary>
		/// <param name="stream">A readable data stream.</param>
		/// <returns>a CBOR object that was read.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="stream"/> is null.</exception>
		public static CBORObject Read(Stream stream){
			return Read(stream,0,false,-1);
		}
		
		private static void WriteObjectArray(
			IList<CBORObject> list, Stream s){
			WriteUInt64(4,(ulong)list.Count,s);
			foreach(var i in list){
				if(i!=null && i.IsBreak)
					throw new ArgumentException();
				Write(i,s);
			}
			s.WriteByte(0xFF);
		}
		
		private static void WriteObjectMap(
			IList<CBORObject[]> list, Stream s){
			WriteUInt64(5,(ulong)list.Count,s);
			foreach(var i in list){
				if(i==null || i.Length<2)
					throw new ArgumentException();
				if(i[0]!=null && i[0].IsBreak)
					throw new ArgumentException();
				if(i[1]!=null && i[1].IsBreak)
					throw new ArgumentException();
				Write(i[0],s);
				Write(i[1],s);
			}
			s.WriteByte(0xFF);
		}
		
		private static void WriteUInt64(int type, ulong value, Stream s){
			if(value<24){
				s.WriteByte((byte)((byte)value|(byte)(type<<5)));
			} else if(value<=0xFF){
				s.WriteByte((byte)(24|(type<<5)));
				s.WriteByte((byte)(value&0xFF));
			} else if(value<=0xFFFF){
				s.WriteByte((byte)(25|(type<<5)));
				s.WriteByte((byte)((value>>8)&0xFF));
				s.WriteByte((byte)(value&0xFF));
			} else if(value<=0xFFFFFFFF){
				s.WriteByte((byte)(26|(type<<5)));
				s.WriteByte((byte)((value>>24)&0xFF));
				s.WriteByte((byte)((value>>16)&0xFF));
				s.WriteByte((byte)((value>>8)&0xFF));
				s.WriteByte((byte)(value&0xFF));
			} else {
				s.WriteByte((byte)(27|(type<<5)));
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
		/// <summary>
		/// Writes a string in CBOR format to a data stream.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="s"></param>
		public static void Write(string str, Stream s){
			ArgumentAssertInternal.NotNull(s,"s");
			if(str==null){
				s.WriteByte(0xf6); // Write null instead of string
			} else {
				byte[] data=utf8.GetBytes(str);
				WriteUInt64(3,(ulong)data.Length,s);
				s.Write(data,0,data.Length);
			}
		}
		
		private static string DateTimeToString(DateTime bi){
			DateTime dt=bi.ToUniversalTime();
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append(String.Format(
				CultureInfo.InvariantCulture,
				"{0:d4}-{1:d2}-{2:d2}T{3:d2}:{4:d2}:{5:d2}",
				dt.Year,dt.Month,dt.Day,dt.Hour,
				dt.Minute,dt.Second));
			if(dt.Millisecond>0){
				sb.Append(String.Format(CultureInfo.InvariantCulture,
				                        ".{0:d3}",dt.Millisecond));
			}
			sb.Append("Z");
			return sb.ToString();
		}
		
		/// <summary>
		/// Writes a date and time in CBOR format to a data stream
		/// </summary>
		/// <param name="bi"></param>
		/// <param name="s"></param>
		public static void Write(DateTime bi, Stream stream){
			ArgumentAssertInternal.NotNull(stream,"s");
			stream.WriteByte(0xC0);
			Write(DateTimeToString(bi),stream);
		}
		
		private static BigInteger LowestNegativeBigInt=
			BigInteger.Parse("-18446744073709551616",NumberStyles.AllowLeadingSign,
			                 CultureInfo.InvariantCulture);
		
		/// <summary>
		/// Writes a big integer in CBOR format to a data stream.
		/// </summary>
		/// <param name="bi">Big integer to write.</param>
		/// <param name="s">Stream to write to.</param>
		public static void Write(BigInteger bi, Stream s){
			ArgumentAssertInternal.NotNull(s,"s");
			int datatype=(bi<0) ? 1 : 0;
			if(bi<0){
				bi+=1;
				bi=-bi;
			}
			if(bi<=UInt64.MaxValue){
				// If the big integer is representable in
				// major type 0 or 1, write that major type
				// instead of as a bignum
				ulong ui=(ulong)bi;
				WriteUInt64(datatype,ui,s);
			} else {
				s.WriteByte((datatype==0) ?
				            (byte)0xC2 :
				            (byte)0xC3);
				List<byte> bytes=new List<byte>();
				while(bi>0){
					bytes.Add((byte)(bi&0xFF));
					bi>>=8;
				}
				WriteUInt64(2,(ulong)bytes.Count,s);
				for(var i=bytes.Count-1;i>=0;i--){
					s.WriteByte(bytes[i]);
				}
			}
		}
		
		/// <summary>
		/// Writes this CBOR object to a data stream.
		/// </summary>
		/// <param name="s">A readable data stream.</param>
		public void Write(Stream s){
			ArgumentAssertInternal.NotNull(s,"s");
			if(tagged)
				WriteUInt64(6,tag,s);
			if(itemtype==0){
				WriteUInt64(0,(ulong)item,s);
			} else if(itemtype== CBORObjectType.SInteger){
				Write((long)item,s);
			} else if(itemtype== CBORObjectType.BigInteger || itemtype== CBORObjectType.BigIntegerType1){
				Write((BigInteger)item,s);
			} else if(itemtype== CBORObjectType.ByteString){
				WriteUInt64((itemtype== CBORObjectType.ByteString) ? 2 : 3,
				            (ulong)((byte[])item).Length,s);
				s.Write(((byte[])item),0,((byte[])item).Length);
			} else if(itemtype== CBORObjectType.TextString ){
				Write((string)item,s);
			} else if(itemtype== CBORObjectType.Array){
				WriteObjectArray((IList<CBORObject>)item,s);
			} else if(itemtype== CBORObjectType.Map){
				WriteObjectMap((IList<CBORObject[]>)item,s);
			} else if(itemtype== CBORObjectType.SimpleValue){
				int value=(int)item;
				if(value<24 || value==31){
					s.WriteByte((byte)(0xE0+value));
				} else {
					s.WriteByte(0xF8);
					s.WriteByte((byte)value);
				}
			} else if(itemtype== CBORObjectType.Single){
				Write((float)item,s);
			} else if(itemtype== CBORObjectType.Double){
				Write((double)item,s);
			} else {
				throw new ArgumentException();
			}
		}
		
		public static void Write(long value, Stream s){
			ArgumentAssertInternal.NotNull(s,"s");
			if(((long)value)>=0){
				WriteUInt64(0,(ulong)(long)value,s);
			} else {
				long ov=(((long)value)+1);
				WriteUInt64(1,(ulong)(-ov),s);
			}
		}
		public static void Write(int value, Stream s){
			Write((long)value,s);
		}
		public static void Write(short value, Stream s){
			Write((long)value,s);
		}
		public static void Write(char value, Stream s){
			Write(new String(new char[]{value}),s);
		}
		public static void Write(bool value, Stream s){
			s.WriteByte(value ? (byte)0xf5 : (byte)0xf4);
		}
		[CLSCompliant(false)]
		public static void Write(sbyte value, Stream s){
			Write((long)value,s);
		}
		[CLSCompliant(false)]
		public static void Write(ulong value, Stream s){
			WriteUInt64(0,(ulong)value,s);
		}
		[CLSCompliant(false)]
		public static void Write(uint value, Stream s){
			WriteUInt64(0,(ulong)value,s);
		}
		[CLSCompliant(false)]
		public static void Write(ushort value, Stream s){
			WriteUInt64(0,(ulong)value,s);
		}
		public static void Write(byte value, Stream s){
			WriteUInt64(0,(ulong)value,s);
		}
		public static void Write(float value, Stream s){
			ArgumentAssertInternal.NotNull(s,"s");
			int bits=ConverterInternal.SingleToInt32Bits(
				value);
			ulong v=(ulong)unchecked((uint)bits);
			s.WriteByte(0xFA);
			s.WriteByte((byte)((v>>24)&0xFF));
			s.WriteByte((byte)((v>>16)&0xFF));
			s.WriteByte((byte)((v>>8)&0xFF));
			s.WriteByte((byte)(v&0xFF));
		}
		public static void Write(double value, Stream s){
			long bits=ConverterInternal.DoubleToInt64Bits(
				(double)value);
			ulong v=unchecked((ulong)bits);
			s.WriteByte(0xFB);
			s.WriteByte((byte)((v>>56)&0xFF));
			s.WriteByte((byte)((v>>48)&0xFF));
			s.WriteByte((byte)((v>>40)&0xFF));
			s.WriteByte((byte)((v>>32)&0xFF));
			s.WriteByte((byte)((v>>24)&0xFF));
			s.WriteByte((byte)((v>>16)&0xFF));
			s.WriteByte((byte)((v>>8)&0xFF));
			s.WriteByte((byte)(v&0xFF));
		}
		
		/// <summary>
		/// Gets the binary representation of this
		/// data item.
		/// </summary>
		/// <returns>A byte array in CBOR format.</returns>
		public byte[] ToBytes(){
			using(MemoryStream ms=new MemoryStream()){
				Write(ms);
				if(ms.Position>Int32.MaxValue)
					throw new OutOfMemoryException();
				byte[] data=new byte[(int)ms.Position];
				ms.Position=0;
				ms.Read(data,0,data.Length);
				return data;
			}
		}
		
		public static void Write(Object o, Stream s){
			ArgumentAssertInternal.NotNull(s,"s");
			if(o==null){
				s.WriteByte(0xf6);
			} else if(o is byte[]){
				byte[] data=(byte[])o;
				WriteUInt64(3,(ulong)data.Length,s);
				s.Write(data,0,data.Length);
			} else if(o is IList<CBORObject>){
				WriteObjectArray((IList<CBORObject>)o,s);
			} else if(o is IDictionary<CBORObject,CBORObject>){
				IDictionary<CBORObject,CBORObject> dic=
					(IDictionary<CBORObject,CBORObject>)o;
				WriteUInt64(5,(ulong)dic.Count,s);
				foreach(var i in dic.Keys){
					if(i!=null && i.IsBreak)
						throw new ArgumentException();
					var value=dic[i];
					if(value!=null && value.IsBreak)
						throw new ArgumentException();
					Write(i,s);
					Write(value,s);
				}
				s.WriteByte(0xFF);
			} else if(o is IList<CBORObject[]>){
				WriteObjectMap((IList<CBORObject[]>)o,s);
			} else {
				FromObject(o).Write(s);
			}
		}
		
		//-----------------------------------------------------------
		
		/// <summary>
		/// Generates a CBOR object from a string in JavaScript Object
		/// Notation (JSON) format.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static CBORObject FromJSONString(string str){
			JSONTokener tokener=new JSONTokener(str,0);
			return ParseJSONObject(tokener);
		}
		
		private static CBORObject ParseJSONNumber(string str){
			if(String.IsNullOrEmpty(str))
				return null;
			char c=str[0];
			bool negative=false;
			int index=0;
			if(index>=str.Length)
				return null;
			c=str[index];
			if(c=='-'){
				negative=true;
				index++;
			}
			int numberStart=index;
			if(index>=str.Length)
				return null;
			c=str[index];
			index++;
			int numberEnd=index;
			int fracStart=-1;
			int fracEnd=-1;
			bool negExp=false;
			int expStart=-1;
			int expEnd=-1;
			if(c>='1' && c<='9'){
				while(index<str.Length){
					c=str[index];
					if(c>='0' && c<='9'){
						index++;
						numberEnd=index;
					} else {
						break;
					}
				}
			} else if(c!='0'){
				return null;
			}
			if(index<str.Length && str[index]=='.'){
				// Fraction
				index++;
				fracStart=index;
				if(index>=str.Length)
					return null;
				c=str[index];
				index++;
				fracEnd=index;
				if(c>='0' && c<='9'){
					while(index<str.Length){
						c=str[index];
						if(c>='0' && c<='9'){
							index++;
							fracEnd=index;
						} else {
							break;
						}
					}
				} else {
					// Not a fraction
					return null;
				}
			}
			if(index<str.Length && (str[index]=='e' || str[index]=='E')){
				// Exponent
				index++;
				if(index>=str.Length)
					return null;
				c=str[index];
				if(c=='-'){
					negative=true;
					index++;
				}
				if(c=='+')index++;
				expStart=index;
				if(index>=str.Length)
					return null;
				c=str[index];
				index++;
				expEnd=index;
				if(c>='0' && c<='9'){
					while(index<str.Length){
						c=str[index];
						if(c>='0' && c<='9'){
							index++;
							expEnd=index;
						} else {
							break;
						}
					}
				} else {
					// Not an exponent
					return null;
				}
			}
			if(fracStart<0 && expStart<0 && (numberEnd-numberStart)<=9){
				// Common case: small integer
				int value=Int32.Parse(str.Substring(numberStart,numberEnd-numberStart),
				                      NumberStyles.None,
				                      CultureInfo.InvariantCulture);
				if(negative)value=-value;
				return FromObject(value);
			} else if(fracStart<0 && expStart<0){
				// Bigger integer
				BigInteger value=BigInteger.Parse(
					str.Substring(numberStart,numberEnd-numberStart),
					NumberStyles.None,
					CultureInfo.InvariantCulture);
				if(negative)value=-value;
				return FromObject(value);
			} else {
				BigInteger fracpart=(fracStart<0) ? BigInteger.Zero : BigInteger.Parse(
					str.Substring(fracStart,fracEnd-fracStart),
					NumberStyles.None,
					CultureInfo.InvariantCulture);
				// Intval consists of the whole and fractional part
				BigInteger intval=BigInteger.Parse(
					str.Substring(numberStart,numberEnd-numberStart)+
					(fracpart.IsZero ? String.Empty : str.Substring(fracStart,fracEnd-fracStart)),
					NumberStyles.None,
					CultureInfo.InvariantCulture);
				if(negative)intval=-intval;
				if(fracpart.IsZero && expStart<0){
					// Zero fractional part and no exponent;
					// this is easy, just return the integer
					return FromObject(intval);
				}
				BigInteger exp=(expStart<0) ? BigInteger.Zero : BigInteger.Parse(
					str.Substring(expStart,expEnd-expStart),
					NumberStyles.None,
					CultureInfo.InvariantCulture);
				if(negExp)exp=-exp;
				if(!fracpart.IsZero){
					// If there is a nonzero fractional part,
					// decrease the exponent by that part's length
					exp-=(fracEnd-fracStart);
				}
				if(exp.IsZero){
					// If exponent is 0, this is also easy,
					// just return the integer
					return FromObject(intval);
				}
				if(exp>UInt64.MaxValue || exp<LowestNegativeBigInt){
					// Exponent is lower than the lowest representable
					// integer of major type 1, or higher than the
					// highest representable integer of major type 0
					if(intval.IsZero){
						return FromObject(0);
					} else {
						return null;
					}
				}
				// Represent the CBOR object as a decimal fraction
				return FromObjectAndTag(new CBORObject[]{
				                        	FromObject(exp),FromObject(intval)},4);
			}
		}
		
		// Based on the json.org implementation for JSONTokener
		private static CBORObject NextJSONValue(JSONTokener tokener)  {
			int c = tokener.nextClean();
			string s;
			if (c == '"' || c == '\'')
				return FromObject(tokener.nextString(c));
			if (c == '{') {
				tokener.back();
				return ParseJSONObject(tokener);
			}
			if (c == '[') {
				tokener.back();
				return ParseJSONArray(tokener);
			}
			StringBuilder sb = new StringBuilder();
			int b = c;
			while (c >= ' ' && c != ':' && c != ',' && c != ']' && c != '}' &&
			       c != '/') {
				sb.Append((char)c);
				c = tokener.next();
			}
			tokener.back();
			s = JSONTokener.trimSpaces(sb.ToString());
			if (s.Equals("true"))
				return CBORObject.True;
			if (s.Equals("false"))
				return CBORObject.False;
			if (s.Equals("null"))
				return CBORObject.Null;
			if ((b >= '0' && b <= '9') || b == '.' || b == '-' || b == '+') {
				CBORObject obj=ParseJSONNumber(s);
				if(obj==null)
					throw tokener.syntaxError("JSON number can't be parsed.");
				return obj;
			}
			if (s.Length == 0)
				throw tokener.syntaxError("Missing value.");
			return FromObject(s);
		}

		// Based on the json.org implementation for JSONObject
		private static CBORObject ParseJSONObject(JSONTokener x) {
			int c;
			string key;
			CBORObject obj;
			c=x.nextClean();
			if(c=='['){
				x.back();
				return ParseJSONArray(x);
			}
			var myHashMap=new Dictionary<string,CBORObject>();
			if (c != '{')
				throw x.syntaxError("A JSONObject must begin with '{'");
			while (true) {
				c = x.nextClean();
				switch (c) {
					case -1:
						throw x.syntaxError("A JSONObject must end with '}'");
					case '}':
						return FromObject(myHashMap);
					default:
						x.back();
						obj=NextJSONValue(x);
						if(obj.itemtype!= CBORObjectType.TextString)
							throw x.syntaxError("Expected a string as a key");
						key = obj.AsString();
						if((x.getOptions() & JSONTokener.OPTION_NO_DUPLICATES)!=0 &&
						   myHashMap.ContainsKey(key)){
							throw x.syntaxError("Key already exists: "+key);
						}
						break;
				}
				
				if (x.nextClean() != ':')
					throw x.syntaxError("Expected a ':' after a key");
				// NOTE: Will overwrite existing value. --Peter O.
				myHashMap.Add(key, NextJSONValue(x));
				switch (x.nextClean()) {
					case ',':
						if (x.nextClean() == '}'){
							if((x.getOptions() & JSONTokener.OPTION_TRAILING_COMMAS)==0){
								// 2013-05-24 -- Peter O. Disallow trailing comma.
								throw x.syntaxError("Trailing comma");
							} else {
								return FromObject(myHashMap);
							}
						}
						x.back();
						break;
					case '}':
						return FromObject(myHashMap);
					default:
						throw x.syntaxError("Expected a ',' or '}'");
				}
			}
		}

		// Based on the json.org implementation for JSONArray
		private static CBORObject ParseJSONArray(JSONTokener x){
			var myArrayList=new List<CBORObject>();
			if (x.nextClean() != '[')
				throw x.syntaxError("A JSONArray must start with '['");
			if (x.nextClean() == ']')
				return FromObject(myArrayList);
			x.back();
			while (true) {
				if (x.nextClean() == ',') {
					if((x.getOptions() & JSONTokener.OPTION_EMPTY_ARRAY_ELEMENTS)==0){
						throw x.syntaxError("Two commas one after the other");
					}
					x.back();
					myArrayList.Add(CBORObject.Null);
				} else {
					x.back();
					myArrayList.Add(NextJSONValue(x));
				}
				switch (x.nextClean()) {
					case ',':
						if (x.nextClean() == ']'){
							if((x.getOptions() & JSONTokener.OPTION_TRAILING_COMMAS)==0){
								// 2013-05-24 -- Peter O. Disallow trailing comma.
								throw x.syntaxError("Trailing comma");
							} else {
								return FromObject(myArrayList);
							}
						}
						x.back();
						break;
					case ']':
						return FromObject(myArrayList);
					default:
						throw x.syntaxError("Expected a ',' or ']'");
				}
			}
		}

		private static string ReadUtf8(Stream stream, int byteLength)  {
			StringBuilder builder=new StringBuilder();
			int cp=0;
			int bytesSeen=0;
			int bytesNeeded=0;
			int lower=0x80;
			int upper=0xBF;
			int pointer=0;
			int markedPointer=-1;
			while(pointer<byteLength || byteLength<0){
				int b=stream.ReadByte();
				if(b<0 && bytesNeeded!=0){
					bytesNeeded=0;
					throw new FormatException("Invalid UTF-8");
				} else if(b<0){
					if(byteLength>0 && pointer>=byteLength)
						throw new FormatException("Premature end of stream");
					break; // end of stream
				}
				if(byteLength>0) {
					pointer++;
				}
				if(bytesNeeded==0){
					if(b<0x80){
						builder.Append((char)b);
					}
					else if(b>=0xc2 && b<=0xdf){
						markedPointer=pointer;
						bytesNeeded=1;
						cp=b-0xc0;
					} else if(b>=0xe0 && b<=0xef){
						markedPointer=pointer;
						lower=(b==0xe0) ? 0xa0 : 0x80;
						upper=(b==0xed) ? 0x9f : 0xbf;
						bytesNeeded=2;
						cp=b-0xe0;
					} else if(b>=0xf0 && b<=0xf4){
						markedPointer=pointer;
						lower=(b==0xf0) ? 0x90 : 0x80;
						upper=(b==0xf4) ? 0x8f : 0xbf;
						bytesNeeded=3;
						cp=b-0xf0;
					} else
						throw new FormatException("Invalid UTF-8");
					cp<<=(6*bytesNeeded);
					continue;
				}
				if(b<lower || b>upper){
					cp=bytesNeeded=bytesSeen=0;
					lower=0x80;
					upper=0xbf;
					throw new FormatException("Invalid UTF-8");
				}
				lower=0x80;
				upper=0xbf;
				bytesSeen++;
				cp+=(b-0x80)<<(6*(bytesNeeded-bytesSeen));
				markedPointer=pointer;
				if(bytesSeen!=bytesNeeded) {
					continue;
				}
				int ret=cp;
				cp=0;
				bytesSeen=0;
				bytesNeeded=0;
				if(ret<=0xFFFF){
					builder.Append((char)ret);
				} else {
					int ch=ret-0x10000;
					int lead=ch/0x400+0xd800;
					int trail=(ch&0x3FF)+0xdc00;
					builder.Append((char)lead);
					builder.Append((char)trail);
				}
			}
			return builder.ToString();
		}
		
		
		//-----------------------------------------------------------
		public static CBORObject FromObject(long value){
			if(((long)value)>=0){
				return new CBORObject(CBORObjectType.UInteger,(ulong)(long)value);
			} else {
				return new CBORObject(CBORObjectType.SInteger,value);
			}
		}
		public static CBORObject FromObject(CBORObject value){
			if(value==null)return CBORObject.Null;
			return value;
		}
		public static CBORObject FromObject(BigInteger value){
			return new CBORObject(CBORObjectType.BigInteger,value);
		}
		public static CBORObject FromObject(string value){
			if(value==null)return CBORObject.Null;
			if(!IsValidString(value))
				throw new ArgumentException("String contains an unpaired surrogate code point.");
			return new CBORObject(CBORObjectType.TextString,value);
		}
		public static CBORObject FromObject(int value){
			return FromObject((long)value);
		}
		public static CBORObject FromObject(short value){
			return FromObject((long)value);
		}
		public static CBORObject FromObject(char value){
			return FromObject(new String(new char[]{value}));
		}
		public static CBORObject FromObject(bool value){
			return (value ? CBORObject.True : CBORObject.False);
		}
		[CLSCompliant(false)]
		public static CBORObject FromObject(sbyte value){
			return FromObject((long)value);
		}
		[CLSCompliant(false)]
		public static CBORObject FromObject(ulong value){
			return new CBORObject(CBORObjectType.UInteger,(ulong)value);
		}
		[CLSCompliant(false)]
		public static CBORObject FromObject(uint value){
			return new CBORObject(CBORObjectType.UInteger,(ulong)value);
		}
		[CLSCompliant(false)]
		public static CBORObject FromObject(ushort value){
			return new CBORObject(CBORObjectType.UInteger,(ulong)value);
		}
		public static CBORObject FromObject(byte value){
			return new CBORObject(CBORObjectType.UInteger,(ulong)value);
		}
		public static CBORObject FromObject(float value){
			return new CBORObject(CBORObjectType.Single,value);
		}
		public static CBORObject FromObject(DateTime value){
			return new CBORObject(CBORObjectType.TextString,0,
			                      DateTimeToString(value));
		}
		public static CBORObject FromObject(double value){
			return new CBORObject(CBORObjectType.Double,value);
		}
		public static CBORObject FromObject(IList<CBORObject[]> value){
			if(value==null)return CBORObject.Null;
			if(!(value is List<CBORObject[]>))
				value=new List<CBORObject[]>(value);
			foreach(var i in value){
				if(i==null || i.Length<2)
					throw new ArgumentException();
				if(i[0]!=null && i[0].IsBreak)
					throw new ArgumentException();
				if(i[1]!=null && i[1].IsBreak)
					throw new ArgumentException();
			}
			return new CBORObject(CBORObjectType.Map,value);
		}
		public static CBORObject FromObject(IList<CBORObject> value){
			if(value==null)return CBORObject.Null;
			if(!(value is List<CBORObject>))
				value=new List<CBORObject>(value);
			foreach(var i in (IList<CBORObject>)value){
				if(i!=null && i.IsBreak)
					throw new ArgumentException();
			}
			return new CBORObject(CBORObjectType.Array,value);
		}
		public static CBORObject FromObject(byte[] value){
			if(value==null)return CBORObject.Null;
			return new CBORObject(CBORObjectType.ByteString,value);
		}
		
		public static CBORObject FromObject<T>(IList<T> value){
			if(value==null)return CBORObject.Null;
			IList<CBORObject> list=new List<CBORObject>();
			foreach(var i in (IList<CBORObject>)value){
				list.Add(FromObject(i));
			}
			return FromObject(list);
		}
		public static CBORObject FromObject<TKey, TValue>(IDictionary<TKey, TValue> dic){
			if(dic==null)return CBORObject.Null;
			IList<CBORObject[]> list=new List<CBORObject[]>();
			foreach(var i in dic.Keys){
				var key=FromObject(i);
				if(key!=null && key.IsBreak)
					throw new ArgumentException();
				var value=FromObject(dic[i]);
				if(value!=null && value.IsBreak)
					throw new ArgumentException();
				list.Add(new CBORObject[]{key,value});
			}
			return new CBORObject(CBORObjectType.Map,list);
		}
		public static CBORObject FromObject(Object o){
			if(o==null)return CBORObject.Null;
			if(o is long)return FromObject((long)o);
			if(o is CBORObject)return FromObject((CBORObject)o);
			if(o is BigInteger)return FromObject((BigInteger)o);
			if(o is string)return FromObject((string)o);
			if(o is int)return FromObject((int)o);
			if(o is short)return FromObject((short)o);
			if(o is char)return FromObject((char)o);
			if(o is bool)return FromObject((bool)o);
			if(o is sbyte)return FromObject((sbyte)o);
			if(o is ulong)return FromObject((ulong)o);
			if(o is uint)return FromObject((uint)o);
			if(o is ushort)return FromObject((ushort)o);
			if(o is byte)return FromObject((byte)o);
			if(o is float)return FromObject((float)o);
			if(o is DateTime)return FromObject((DateTime)o);
			if(o is double)return FromObject((double)o);
			if(o is IList<CBORObject>)return FromObject((IList<CBORObject>)o);
			if(o is IList<CBORObject[]>)return FromObject((IList<CBORObject[]>)o);
			if(o is byte[])return FromObject((byte[])o);
			if(o is IDictionary<CBORObject,CBORObject>)return FromObject(
				(IDictionary<CBORObject,CBORObject>)o);
			if(o is IDictionary<string,CBORObject>)return FromObject(
				(IDictionary<string,CBORObject>)o);
			throw new ArgumentException();
		}
		
		[CLSCompliant(false)]
		public static CBORObject FromObjectAndTag(Object o, ulong tag){
			CBORObject c=FromObject(o);
			return new CBORObject(c.itemtype,tag,c.item);
		}

		public static CBORObject FromObjectAndTag(Object o, int tag){
			ArgumentAssertInternal.GreaterOrEqual(tag,0,"tag");
			CBORObject c=FromObject(o);
			return new CBORObject(c.itemtype,(ulong)tag,c.item);
		}
		
		//-----------------------------------------------------------
		
		private string IntegerToString(){
			if(itemtype== CBORObjectType.UInteger){
				return String.Format(CultureInfo.InvariantCulture,
				                     "{0}",(ulong)item);
			} else if(itemtype== CBORObjectType.SInteger){
				return String.Format(CultureInfo.InvariantCulture,
				                     "{0}",(long)item);
			} else if(itemtype== CBORObjectType.BigInteger || itemtype== CBORObjectType.BigIntegerType1){
				return String.Format(CultureInfo.InvariantCulture,
				                     "{0}",(BigInteger)item);
			} else {
				throw new InvalidOperationException();
			}
		}
		
		/// <summary>
		/// Returns this CBOR object in string form.
		/// The format is intended to be human-readable, not machine-
		/// parsable, and the format may change at any time.
		/// </summary>
		/// <returns>A text representation of this object.</returns>
		public override string ToString(){
			StringBuilder sb=new StringBuilder();
			if(tagged){
				sb.Append(String.Format(CultureInfo.InvariantCulture,"{0}(",tag));
			}
			if(itemtype== CBORObjectType.SimpleValue){
				if(this.IsBreak)sb.Append("break");
				else if(this.IsTrue)sb.Append("true");
				else if(this.IsFalse)sb.Append("false");
				else if(this.IsNull)sb.Append("null");
				else if(this.IsUndefined)sb.Append("undefined");
				else {
					sb.Append(String.Format(CultureInfo.InvariantCulture,
					                        "simple({0})",(int)item));
				}
			} else if(itemtype== CBORObjectType.Single){
				float f=(float)item;
				if(Single.IsNegativeInfinity(f))
					sb.Append("-Infinity");
				else if(Single.IsPositiveInfinity(f))
					sb.Append("Infinity");
				else if(Single.IsNaN(f))
					sb.Append("NaN");
				else
					sb.Append(String.Format(CultureInfo.InvariantCulture,"{0}",f));
			} else if(itemtype== CBORObjectType.Double){
				double f=(double)item;
				if(Double.IsNegativeInfinity(f))
					sb.Append("-Infinity");
				else if(Double.IsPositiveInfinity(f))
					sb.Append("Infinity");
				else if(Double.IsNaN(f))
					sb.Append("NaN");
				else
					sb.Append(String.Format(CultureInfo.InvariantCulture,"{0}",f));
			} else if(itemtype== CBORObjectType.UInteger){
				sb.Append(String.Format(CultureInfo.InvariantCulture,
				                        "{0}",(ulong)item));
			} else if(itemtype== CBORObjectType.SInteger){
				sb.Append(String.Format(CultureInfo.InvariantCulture,
				                        "{0}",(long)item));
			} else if(itemtype== CBORObjectType.BigInteger || itemtype== CBORObjectType.BigIntegerType1){
				sb.Append(String.Format(CultureInfo.InvariantCulture,
				                        "{0}",(BigInteger)item));
			} else if(itemtype== CBORObjectType.ByteString){
				sb.Append("h'");
				byte[] data=(byte[])item;
				for(int i=0;i<data.Length;i++){
					sb.Append(String.Format(CultureInfo.InvariantCulture,
					                        "{0:X2}",data[i]));
				}
				sb.Append("'");
			} else if(itemtype== CBORObjectType.TextString){
				sb.Append("\"");
				sb.Append((string)item);
				sb.Append("\"");
			} else if(itemtype== CBORObjectType.Array){
				bool first=true;
				sb.Append("[");
				foreach(var i in (IList<CBORObject>)item){
					if(!first)sb.Append(", ");
					sb.Append(i);
					first=false;
				}
				sb.Append("]");
			} else if(itemtype== CBORObjectType.Map){
				bool first=true;
				sb.Append("{");
				foreach(var i in (IList<CBORObject[]>)item){
					if(!first)sb.Append(", ");
					sb.Append(i[0]);
					sb.Append(": ");
					sb.Append(i[1]);
					first=false;
				}
				sb.Append("}");
			}
			if(tagged){
				sb.Append(")");
			}
			return sb.ToString();
		}

		private static float HalfPrecisionToSingle(int value){
			int negvalue=(value>=0x8000) ? (1<<31) : 0;
			value&=0x7FFF;
			if(value>=0x7C00){
				return ConverterInternal.Int32BitsToSingle(
					(0x3FC00|(value&0x3FF))<<13|negvalue);
			} else if(value>0x400){
				return ConverterInternal.Int32BitsToSingle(
					((value+0x1c000)<<13)|negvalue);
			} else if((value&0x400)==value){
				return ConverterInternal.Int32BitsToSingle(
					((value==0) ? 0 : 0x38800000)|negvalue);
			} else {
				// denormalized
				int m=(value&0x3FF);
				value=0x1c400;
				while((m>>10)==0){
					value-=0x400;
					m<<=1;
				}
				return ConverterInternal.Int32BitsToSingle(
					((value|(m&0x3FF))<<13)|negvalue);
			}
		}
		
		private int MajorType {
			get {
				switch(itemtype){
					case CBORObjectType.UInteger:
						return 0;
					case CBORObjectType.SInteger:
					case CBORObjectType.BigIntegerType1:
						return 1;
					case CBORObjectType.BigInteger:
					case CBORObjectType.ByteString:
						return 2;
					case CBORObjectType.TextString:
						return 3;
					case CBORObjectType.Array:
						return 4;
					case CBORObjectType.Map:
						return 5;
					case CBORObjectType.SimpleValue:
					case CBORObjectType.Single:
					case CBORObjectType.Double:
						return 7;
					default:
						throw new InvalidOperationException();
				}
			}
		}
		
		private static CBORObject Read(
			Stream s,
			int depth,
			bool allowBreak,
			int allowOnlyType){
			if(depth>1000)
				throw new IOException();
			int c=s.ReadByte();
			if(c<0)
				throw new IOException();
			int type=(c>>5)&0x07;
			int additional=(c&0x1F);
			if(c==0xFF){
				if(allowBreak)return Break;
				throw new FormatException();
			}
			if(type!=6){
				if(allowOnlyType>=0 &&
				   (allowOnlyType!=type || additional>=28)){
					throw new FormatException();
				}
			}
			if(type==7){
				if(additional==20)return False;
				if(additional==21)return True;
				if(additional==22)return Null;
				if(additional==23)return Undefined;
				if(additional<20){
					return new CBORObject(CBORObjectType.SimpleValue,additional);
				}
				if(additional==24){
					c=s.ReadByte();
					if(c<0)
						throw new IOException();
					return new CBORObject(CBORObjectType.SimpleValue,c);
				}
				if(additional==25 || additional==26 ||
				   additional==27){
					int cslength=(additional==25) ? 2 :
						((additional==26) ? 4 : 8);
					byte[] cs=new byte[cslength];
					if(s.Read(cs,0,cs.Length)!=cs.Length)
						throw new IOException();
					ulong x=0;
					for(int i=0;i<cs.Length;i++){
						x<<=8;
						x|=cs[i];
					}
					if(additional==25){
						float f=HalfPrecisionToSingle(
							unchecked((int)x));
						return new CBORObject(CBORObjectType.Single,f);
					} else if(additional==26){
						float f=ConverterInternal.Int32BitsToSingle(
							unchecked((int)x));
						return new CBORObject(CBORObjectType.Single,f);
					} else if(additional==27){
						double f=ConverterInternal.Int64BitsToDouble(
							unchecked((long)x));
						return new CBORObject(CBORObjectType.Double,f);
					}
				}
				throw new FormatException();
			}
			ulong uadditional=0;
			if(additional<=23){
				uadditional=(uint)additional;
			} else if(additional==24){
				int c1=s.ReadByte();
				if(c1<0)
					throw new IOException();
				uadditional=(ulong)c1;
			} else if(additional==25 || additional==26){
				byte[] cs=new byte[(additional==25) ? 2 : 4];
				if(s.Read(cs,0,cs.Length)!=cs.Length)
					throw new IOException();
				uint x=0;
				for(int i=0;i<cs.Length;i++){
					x<<=8;
					x|=cs[i];
				}
				uadditional=x;
			} else if(additional==27){
				byte[] cs=new byte[8];
				if(s.Read(cs,0,cs.Length)!=cs.Length)
					throw new IOException();
				ulong x=0;
				for(int i=0;i<cs.Length;i++){
					x<<=8;
					x|=cs[i];
				}
				uadditional=x;
			} else if(additional==28 || additional==29 ||
			          additional==30){
				throw new FormatException();
			} else if(additional==31 && type<2){
				throw new FormatException();
			}
			if(type==0){
				return new CBORObject(CBORObjectType.UInteger,uadditional);
			} else if(type==1){
				if(uadditional<=Int64.MaxValue){
					return new CBORObject(
						CBORObjectType.SInteger,
						(long)((long)-1-(long)uadditional));
				} else {
					BigInteger bi=new BigInteger(-1);
					bi-=new BigInteger(uadditional);
					return new CBORObject(CBORObjectType.BigIntegerType1,
					                      bi);
				}
			} else if(type==6){ // Tagged item
				CBORObject o=Read(s,depth+1,allowBreak,-1);
				if(uadditional==2 || uadditional==3){
					// Big number
					if(o.MajorType!=2) // Requires major type 2 (byte string)
						throw new FormatException();
					byte[] data=(byte[])o.item;
					BigInteger bi=0;
					for(int i=0;i<data.Length;i++){
						bi<<=8;
						bi|=data[i];
					}
					if(uadditional==3){
						bi=-1-bi; // Convert to a negative
					}
					return new CBORObject(CBORObjectType.BigInteger,
					                      bi);
				} else if(uadditional==4){
					if(o.MajorType!=4) // Requires major type 4 (array)
						throw new FormatException();
					if(o.Count!=2) // Requires 2 items
						throw new FormatException();
					// check type of exponent
					if(o[0].MajorType!=0 && o[0].MajorType!=1)
						throw new FormatException();
					// check type of mantissa
					if(o[1].MajorType!=0 && o[1].MajorType!=1 &&
					   o.itemtype!=CBORObjectType.BigInteger)
						throw new FormatException();
				}
				return new CBORObject(o.itemtype,
				                      uadditional,o.item);
			} else if(type==2){ // Byte string
				if(additional==31){
					// Streaming byte string
					using(MemoryStream ms=new MemoryStream()){
						byte[] data;
						while(true){
							CBORObject o=Read(s,depth+1,true,type);
							if(o.IsBreak)
								break;
							if(o.MajorType!=type) // Requires same type as this one
								throw new FormatException();
							data=(byte[])o.item;
							ms.Write(data,0,data.Length);
						}
						if(ms.Position>Int32.MaxValue)
							throw new IOException();
						data=new byte[(int)ms.Position];
						ms.Position=0;
						ms.Read(data,0,data.Length);
						return new CBORObject(
							CBORObjectType.ByteString,
							data);
					}
				} else {
					if(uadditional>Int32.MaxValue){
						throw new IOException();
					}
					byte[] data=new byte[uadditional];
					if(s.Read(data,0,data.Length)!=data.Length)
						throw new IOException();
					return new CBORObject(
						CBORObjectType.ByteString,
						data);
				}
			} else if(type==3){ // Text string
				if(additional==31){
					// Streaming text string
					StringBuilder builder=new StringBuilder();
					while(true){
						CBORObject o=Read(s,depth+1,true,type);
						if(o.IsBreak)
							break;
						if(o.MajorType!=type) // Requires same type as this one
							throw new FormatException();
						builder.Append((string)o.item);
					}
					return new CBORObject(
						CBORObjectType.TextString,
						builder.ToString());
				} else {
					if(uadditional>Int32.MaxValue){
						throw new IOException();
					}
					string str=ReadUtf8(s,(int)uadditional);
					return new CBORObject(CBORObjectType.TextString,str);
				}
			} else if(type==4){ // Array
				IList<CBORObject> list=new List<CBORObject>();
				if(additional==31){
					while(true){
						CBORObject o=Read(s,depth+1,true,-1);
						if(o.IsBreak)
							break;
						list.Add(o);
					}
					return new CBORObject(CBORObjectType.Array,list);
				} else {
					if(uadditional>Int32.MaxValue){
						throw new IOException();
					}
					for(ulong i=0;i<uadditional;i++){
						list.Add(Read(s,depth+1,false,-1));
					}
					return new CBORObject(CBORObjectType.Array,list);
				}
			} else { // Map, type 5
				IList<CBORObject[]> list=new List<CBORObject[]>();
				if(additional==31){
					while(true){
						CBORObject key=Read(s,depth+1,true,-1);
						if(key.IsBreak)
							break;
						CBORObject value=Read(s,depth+1,false,-1);
						list.Add(new CBORObject[]{key,value});
					}
					return new CBORObject(CBORObjectType.Map,list);
				} else {
					if(uadditional>Int32.MaxValue){
						throw new IOException();
					}
					for(ulong i=0;i<uadditional;i++){
						CBORObject key=Read(s,depth+1,false,-1);
						CBORObject value=Read(s,depth+1,false,-1);
						list.Add(new CBORObject[]{key,value});
					}
					return new CBORObject(CBORObjectType.Map,list);
				}
			}
		}
	}
}