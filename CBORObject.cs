/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Numerics;

namespace PeterO
{
	/// <summary>
	/// Represents an object in Compact Binary Object
	/// Notation (CBOR) and contains methods for reading
	/// and writing CBOR data.  CBOR is defined in
	/// RFC 7049.
	/// </summary>
	public sealed class CBORObject
	{
		enum CBORObjectType {
			UInteger,
			SInteger,
			BigInteger,
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
		
		public bool IsBreak{
			get {
				return itemtype==CBORObjectType.SimpleValue && (int)item==31;
			}
		}
		public bool IsTrue{
			get {
				return itemtype==CBORObjectType.SimpleValue && (int)item==21;
			}
		}
		public bool IsFalse{
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

		
		public CBORObject this[int index]{
			get {
				if(itemtype== CBORObjectType.Array){
					return ((IList<CBORObject>)item)[index];
				} else {
					throw new InvalidOperationException("Not an array");
				}
			}
		}
		
		public static CBORObject Read(Stream s){
			return Read(s,0,false,-1);
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
		public static void Write(String str, Stream s){
			ArgumentAssert.NotNull(s,"s");
			if(str==null){
				s.WriteByte(0xf6); // Write null instead of string
			} else {
				byte[] data=utf8.GetBytes(str);
				WriteUInt64(3,(ulong)data.Length,s);
				s.Write(data,0,data.Length);
			}
		}
		
		
		private static String DateTimeToString(DateTime bi){
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
		
		public static void Write(DateTime bi, Stream s){
			ArgumentAssert.NotNull(s,"s");
			s.WriteByte(0xC0);
			Write(DateTimeToString(bi),s);
		}
		/// <summary>
		/// Writes a big integer as a CBOR data stream.
		/// </summary>
		/// <param name="bi">Big integer to write.</param>
		/// <param name="s">Stream to write to.</param>
		public static void Write(BigInteger bi, Stream s){
			ArgumentAssert.NotNull(s,"s");
			int datatype=(bi<0) ? 1 : 0;
			if(bi<0){
				bi+=1;
				bi=-bi;
			}
			if(bi<=Int64.MaxValue){
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
		
		public void Write(Stream s){
			ArgumentAssert.NotNull(s,"s");
			if(tagged)
				WriteUInt64(6,tag,s);
			if(itemtype==0){
				WriteUInt64(0,(ulong)item,s);
			} else if(itemtype== CBORObjectType.SInteger){
				if(((long)item)>=0){
					WriteUInt64(0,(ulong)(long)item,s);
				} else {
					long ov=(((long)item)+1);
					WriteUInt64(1,(ulong)(-ov),s);
				}
			} else if(itemtype== CBORObjectType.BigInteger){
				BigInteger bi=(BigInteger)item;
				Write(bi,s);
			} else if(itemtype== CBORObjectType.ByteString ||
			          itemtype== CBORObjectType.TextString ){
				if(item is String && itemtype== CBORObjectType.TextString){
					Write((string)item,s);
				} else {
					WriteUInt64((itemtype== CBORObjectType.ByteString) ? 2 : 3,
					            (ulong)((byte[])item).Length,s);
					s.Write(((byte[])item),0,((byte[])item).Length);
				}
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
			ArgumentAssert.NotNull(s,"s");
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
		//[CLSCompliantAttribute(false)]
		public static void Write(sbyte value, Stream s){
			Write((long)value,s);
		}
		//[CLSCompliantAttribute(false)]
		public static void Write(ulong value, Stream s){
			WriteUInt64(0,(ulong)value,s);
		}
		//[CLSCompliantAttribute(false)]
		public static void Write(uint value, Stream s){
			WriteUInt64(0,(ulong)value,s);
		}
		//[CLSCompliantAttribute(false)]
		public static void Write(ushort value, Stream s){
			WriteUInt64(0,(ulong)value,s);
		}
		public static void Write(byte value, Stream s){
			WriteUInt64(0,(ulong)value,s);
		}
		public static void Write(float value, Stream s){
			ArgumentAssert.NotNull(s,"s");
			int bits=Converter.SingleToInt32Bits(
				value);
			ulong v=(ulong)unchecked((uint)bits);
			s.WriteByte(0xFA);
			s.WriteByte((byte)((v>>24)&0xFF));
			s.WriteByte((byte)((v>>16)&0xFF));
			s.WriteByte((byte)((v>>8)&0xFF));
			s.WriteByte((byte)(v&0xFF));
		}
		public static void Write(double value, Stream s){
			long bits=Converter.DoubleToInt64Bits(
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
			ArgumentAssert.NotNull(s,"s");
			if(o==null){
				s.WriteByte(0xf6);
			}
			else if(o is CBORObject){
				((CBORObject)o).Write(s);
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
				if(o is long)Write((long)o,s);
				else if(o is int)Write((int)o,s);
				else if(o is short)Write((short)o,s);
				else if(o is sbyte)Write((sbyte)o,s);
				else if(o is bool)Write((bool)o,s);
				else if(o is char)Write((char)o,s);
				else if(o is ulong)Write((ulong)o,s);
				else if(o is uint)Write((uint)o,s);
				else if(o is ushort)Write((ushort)o,s);
				else if(o is byte)Write((byte)o,s);
				else if(o is float)Write((float)o,s);
				else if(o is double)Write((double)o,s);
				else if(o is BigInteger)Write((BigInteger)o,s);
				else if(o is DateTime)Write((DateTime)o,s);
				else if(o is String)Write((String)o,s);
				throw new ArgumentException();
			}
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
		public static CBORObject FromObject(String value){
			if(value==null)return CBORObject.Null;
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
		//[CLSCompliantAttribute(false)]
		public static CBORObject FromObject(sbyte value){
			return FromObject((long)value);
		}
		//[CLSCompliantAttribute(false)]
		public static CBORObject FromObject(ulong value){
			return new CBORObject(CBORObjectType.UInteger,(ulong)value);
		}
		//[CLSCompliantAttribute(false)]
		public static CBORObject FromObject(uint value){
			return new CBORObject(CBORObjectType.UInteger,(ulong)value);
		}
		//[CLSCompliantAttribute(false)]
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
		
		public static CBORObject FromObject(Object o){
			if(o==null){
				return CBORObject.Null;
			} else if(o is CBORObject){
				return (CBORObject)o;
			} else if(o is byte[]){
				return new CBORObject(CBORObjectType.ByteString,o);
			} else if(o is IList<CBORObject>){
				foreach(var i in (IList<CBORObject>)o){
					if(i!=null && i.IsBreak)
						throw new ArgumentException();
				}
				return new CBORObject(CBORObjectType.Array,o);
			} else if(o is IDictionary<CBORObject,CBORObject>){
				IDictionary<CBORObject,CBORObject> dic=
					(IDictionary<CBORObject,CBORObject>)o;
				IList<CBORObject[]> list=new List<CBORObject[]>();
				foreach(var i in dic.Keys){
					if(i!=null && i.IsBreak)
						throw new ArgumentException();
					var value=dic[i];
					if(value!=null && value.IsBreak)
						throw new ArgumentException();
					list.Add(new CBORObject[]{i,value});
				}
				return new CBORObject(CBORObjectType.Map,list);
			} else if(o is IList<CBORObject[]>){
				foreach(var i in (IList<CBORObject[]>)o){
					if(i==null || i.Length<2)
						throw new ArgumentException();
					if(i[0]!=null && i[0].IsBreak)
						throw new ArgumentException();
					if(i[1]!=null && i[1].IsBreak)
						throw new ArgumentException();
				}
				return new CBORObject(CBORObjectType.Map,o);
			} else {
				if(o is long)return FromObject((long)o);
				else if(o is int)return FromObject((int)o);
				else if(o is short)return FromObject((short)o);
				else if(o is sbyte)return FromObject((sbyte)o);
				else if(o is bool)return FromObject((bool)o);
				else if(o is char)return FromObject((char)o);
				else if(o is ulong)return FromObject((ulong)o);
				else if(o is uint)return FromObject((uint)o);
				else if(o is ushort)return FromObject((ushort)o);
				else if(o is byte)return FromObject((byte)o);
				else if(o is float)return FromObject((float)o);
				else if(o is double)return FromObject((double)o);
				else if(o is BigInteger)return FromObject((BigInteger)o);
				else if(o is DateTime)return FromObject((DateTime)o);
				else if(o is String)return FromObject((String)o);
				throw new ArgumentException();
			}
		}
		
		//-----------------------------------------------------------
		
		public override String ToString(){
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
			} else if(itemtype== CBORObjectType.BigInteger){
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
				if(item is String)
					sb.Append((String)item);
				else
					sb.Append(utf8.GetString((byte[])item));
				sb.Append("\"");
			} else if(itemtype== CBORObjectType.Array){
				bool first=true;
				sb.Append("[");
				foreach(var i in (List<CBORObject>)item){
					if(!first)sb.Append(", ");
					sb.Append(i);
					first=false;
				}
				sb.Append("]");
			} else if(itemtype== CBORObjectType.Map){
				bool first=true;
				sb.Append("{");
				foreach(var i in (List<CBORObject[]>)item){
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
				return Converter.Int32BitsToSingle(
					(0x3FC00|(value&0x3FF))<<13|negvalue);
			} else if(value>0x400){
				return Converter.Int32BitsToSingle(
					((value+0x1c000)<<13)|negvalue);
			} else if((value&0x400)==value){
				return Converter.Int32BitsToSingle(
					((value==0) ? 0 : 0x38800000)|negvalue);
			} else {
				// denormalized
				int m=(value&0x3FF);
				value=0x1c400;
				while((m>>10)==0){
					value-=0x400;
					m<<=1;
				}
				return Converter.Int32BitsToSingle(
					((value|(m&0x3FF))<<13)|negvalue);
			}
		}
		
		private static CBORObject Read(Stream s,
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
						float f=Converter.Int32BitsToSingle(
							unchecked((int)x));
						return new CBORObject(CBORObjectType.Single,f);
					} else if(additional==27){
						double f=Converter.Int64BitsToDouble(
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
					return new CBORObject(CBORObjectType.BigInteger,
					                      bi);
				}
			} else if(type==6){ // Tagged item
				CBORObject o=Read(s,depth+1,allowBreak,-1);
				if(uadditional==2 || uadditional==3){
					// Big number
					if(o.itemtype!=CBORObjectType.ByteString)
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
				}
				return new CBORObject(o.itemtype,
				                      uadditional,o.item);
			} else if(type==2 || // Byte string
			          type==3 // Text string
			         ){
				if(additional==31){ 
					// Streaming byte string or
					// text string
					using(MemoryStream ms=new MemoryStream()){
						byte[] data;
						while(true){
							CBORObject o=Read(s,depth+1,true,type);
							if(o.IsBreak)
								break;
							if(type==2 && o.itemtype!= CBORObjectType.ByteString)
								throw new FormatException();
							if(type==3 && o.itemtype!= CBORObjectType.TextString)
								throw new FormatException();
							if(o.item is String)
								data=utf8.GetBytes((string)o.item);
							else
								data=(byte[])o.item;
							ms.Write(data,0,data.Length);
						}
						if(ms.Position>Int32.MaxValue)
							throw new IOException();
						data=new byte[(int)ms.Position];
						ms.Position=0;
						ms.Read(data,0,data.Length);
						return new CBORObject(
							(type==2) ? CBORObjectType.ByteString :
							CBORObjectType.TextString,
							data);
					}
				} else {
					if(uadditional>Int32.MaxValue){
						throw new IOException();
					}
					byte[] data=new byte[uadditional];
					if(s.Read(data,0,data.Length)!=data.Length)
						throw new IOException();
					if(type==3){
						// Check string for UTF-8
						utf8.GetString(data);
					}
					return new CBORObject(
						(type==2) ? CBORObjectType.ByteString : CBORObjectType.TextString,
						data);
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