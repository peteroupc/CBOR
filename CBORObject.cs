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
	public sealed partial class CBORObject
	{
		enum CBORObjectType {
			// Integer from Int64.MinValue..Int64.MaxValue
			Integer,
			// Big integers exceed the range Int64.MinValue..UInt64.MaxValue
			BigInteger,
			ByteString,
			TextString,
			Array,
			Map,
			SimpleValue,
			Single,
			Double
		}
		
		private CBORObjectType ItemType {
			get {
				return itemtype_;
			}
		}
		
		Object item;
		CBORObjectType itemtype_;
		bool tagged=false;
		int tagLow=0;
		int tagHigh=0;
		
		
		private static readonly BigInteger Int64MaxValue=(BigInteger)Int64.MaxValue;
		private static readonly BigInteger Int64MinValue=(BigInteger)Int64.MinValue;
		public static readonly CBORObject Break=new CBORObject(CBORObjectType.SimpleValue,31);
		public static readonly CBORObject False=new CBORObject(CBORObjectType.SimpleValue,20);
		public static readonly CBORObject True=new CBORObject(CBORObjectType.SimpleValue,21);
		public static readonly CBORObject Null=new CBORObject(CBORObjectType.SimpleValue,22);
		public static readonly CBORObject Undefined=new CBORObject(CBORObjectType.SimpleValue,23);
		private CBORObject(CBORObjectType type, int tagLow, int tagHigh, Object item) : this(type,item) {
			this.itemtype_=type;
			this.tagLow=tagLow;
			this.tagHigh=tagHigh;
			this.tagged=true;
		}
		private CBORObject(CBORObjectType type, Object item){
			// Check range in debug mode to ensure that Integer and BigInteger
			// are unambiguous
			if((type== CBORObjectType.BigInteger) &&
			   ((BigInteger)item).CompareTo(Int64MinValue)>=0 &&
			   ((BigInteger)item).CompareTo(Int64MaxValue)<=0){
				#if DEBUG
				if(!(false))throw new ArgumentException("Big integer is within range for Integer");
				#endif

			}
			this.itemtype_=type;
			this.item=item;
		}
		
		public bool IsBreak {
			get {
				return this.ItemType==CBORObjectType.SimpleValue && (int)item==31;
			}
		}
		public bool IsTrue {
			get {
				return this.ItemType==CBORObjectType.SimpleValue && (int)item==21;
			}
		}
		public bool IsFalse {
			get {
				return this.ItemType==CBORObjectType.SimpleValue && (int)item==20;
			}
		}
		public bool IsNull {
			get {
				return this.ItemType==CBORObjectType.SimpleValue && (int)item==22;
			}
		}
		public bool IsUndefined {
			get {
				return this.ItemType==CBORObjectType.SimpleValue && (int)item==23;
			}
		}
		

		#region Equals and GetHashCode implementation
		private bool ByteArrayEquals(byte[] a, byte[] b){
			if(a==null)return (b==null);
			if(b==null)return false;
			if(a.Length!=b.Length)return false;
			for(int i=0;i<a.Length;i++){
				if(a[i]!=b[i])return false;
			}
			return true;
		}
		
		private int ByteArrayHashCode(byte[] a){
			int ret=19;
			unchecked {
				ret=ret*31+a.Length;
				for(int i=0;i<a.Length;i++){
					ret=ret*31+a[i];
				}
			}
			return ret;
		}

		private bool CBORArrayEquals(
			IList<CBORObject> listA,
			IList<CBORObject> listB
		){
			if(listA==null)return (listB==null);
			if(listB==null)return false;
			if(listA.Count!=listB.Count)return false;
			for(int i=0;i<listA.Count;i++){
				if(!Object.Equals(listA[i],listB[i]))return false;
			}
			return true;
		}

		private int CBORArrayHashCode(IList<CBORObject> list){
			int ret=19;
			unchecked {
				ret=ret*31+list.Count;
				for(int i=0;i<list.Count;i++){
					ret=ret*31+list[i].GetHashCode();
				}
			}
			return ret;
		}

		private bool CBORMapEquals(
			IDictionary<CBORObject,CBORObject> mapA,
			IDictionary<CBORObject,CBORObject> mapB
		){
			if(mapA==null)return (mapB==null);
			if(mapB==null)return false;
			if(mapA.Count!=mapB.Count)return false;
			foreach(CBORObject k in mapA.Keys){
				if(mapB.ContainsKey(k)){
					if(!Object.Equals(mapA[k],mapB[k]))
						return false;
				} else {
					return false;
				}
			}
			return true;
		}

		private int CBORMapHashCode(IDictionary<CBORObject,CBORObject> a){
			// To simplify matters, we use just the count of
			// the map as the basis for the hash code.  More complicated
			// hash code calculation would generally involve defining
			// how CBORObjects ought to be compared (since a stable
			// sort order is necessary for two equal maps to have the
			// same hash code), which is much
			// too difficult for this version.
			return unchecked(a.Count.GetHashCode()*19);
		}

		public override bool Equals(object obj)
		{
			CBORObject other = obj as CBORObject;
			if (other == null)
				return false;
			if(item is byte[]){
				if(!ByteArrayEquals((byte[])item,other.item as byte[]))
					return false;
			} else if(item is IList<CBORObject>){
				if(!CBORArrayEquals((IList<CBORObject>)item,other.item as IList<CBORObject>))
					return false;
			} else if(item is IDictionary<CBORObject,CBORObject>){
				if(!CBORMapEquals((IDictionary<CBORObject,CBORObject>)item,
				                  other.item as IDictionary<CBORObject,CBORObject>))
					return false;
			} else {
				if(!object.Equals(this.item, other.item))
					return false;
			}
			return this.ItemType == other.ItemType && this.tagged == other.tagged &&
				this.tagLow == other.tagLow &&
				this.tagHigh == other.tagHigh;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (item != null){
					int itemHashCode=0;
					if(item is byte[])
						itemHashCode=ByteArrayHashCode((byte[])item);
					else if(item is IList<CBORObject>)
						itemHashCode=CBORArrayHashCode((IList<CBORObject>)item);
					else if(item is IDictionary<CBORObject,CBORObject>)
						itemHashCode=CBORMapHashCode((IDictionary<CBORObject,CBORObject>)item);
					else
						itemHashCode=item.GetHashCode();
					hashCode += 1000000007 * itemHashCode;
				}
				hashCode += 1000000009 * this.ItemType.GetHashCode();
				hashCode += 1000000021 * tagged.GetHashCode();
				hashCode += 1000000033 * tagLow.GetHashCode();
				hashCode += 1000000033 * tagHigh.GetHashCode();
			}
			return hashCode;
		}
		#endregion

		
		public static CBORObject FromBytes(byte[] data){
			using(MemoryStream ms=new MemoryStream(data)){
				CBORObject o=Read(ms);
				if(ms.Position!=data.Length){
					throw new FormatException();
				}
				return o;
			}
		}
		public int Count {
			get {
				if(this.ItemType== CBORObjectType.Array){
					return ((IList<CBORObject>)item).Count;
				} else if(this.ItemType== CBORObjectType.Map){
					return ((IDictionary<CBORObject,CBORObject>)item).Count;
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
				if(tagged){
					BigInteger bi=((tagHigh>>16)&0xFFFF);
					bi<<=16;
					bi|=((tagHigh)&0xFFFF);
					bi<<=16;
					bi|=((tagLow>>16)&0xFFFF);
					bi<<=16;
					bi|=((tagLow)&0xFFFF);
					return bi;
				} else {
					return BigInteger.Zero;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value of a CBOR object in this
		/// array.
		/// </summary>
		public CBORObject this[int index]{
			get {
				if(this.ItemType== CBORObjectType.Array){
					return ((IList<CBORObject>)item)[index];
				} else {
					throw new InvalidOperationException("Not an array");
				}
			}
			set {
				if(this.ItemType== CBORObjectType.Array){
					if(this.Tag==4)
						throw new InvalidOperationException("Read-only array");
					((IList<CBORObject>)item)[index]=value;
				} else {
					throw new InvalidOperationException("Not an array");
				}
			}
		}

		public ICollection<CBORObject> Keys {
			get {
				if(this.ItemType== CBORObjectType.Map){
					return ((IDictionary<CBORObject,CBORObject>)item).Keys;
				} else {
					throw new InvalidOperationException("Not a map");
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the value of a CBOR object in this
		/// map.
		/// </summary>
		public CBORObject this[CBORObject key]{
			get {
				if(this.ItemType== CBORObjectType.Map){
					return ((IDictionary<CBORObject,CBORObject>)item)[key];
				} else {
					throw new InvalidOperationException("Not a map");
				}
			}
			set {
				if(this.ItemType== CBORObjectType.Map){
					((IDictionary<CBORObject,CBORObject>)item)[key]=value;
				} else {
					throw new InvalidOperationException("Not a map");
				}
			}
		}
		
		public void Add(CBORObject key, CBORObject value){
			if(this.ItemType== CBORObjectType.Map){
				((IDictionary<CBORObject,CBORObject>)item).Add(key,value);
			} else {
				throw new InvalidOperationException("Not a map");
			}
		}

		public void ContainsKey(CBORObject key){
			if(this.ItemType== CBORObjectType.Map){
				((IDictionary<CBORObject,CBORObject>)item).ContainsKey(key);
			} else {
				throw new InvalidOperationException("Not a map");
			}
		}

		public void Add(CBORObject obj){
			if(this.ItemType== CBORObjectType.Array){
				if(this.Tag==(BigInteger)4)
					throw new InvalidOperationException("Read-only array");
				((IList<CBORObject>)item).Add(obj);
			} else {
				throw new InvalidOperationException("Not an array");
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
			if(this.ItemType== CBORObjectType.Integer)
				return (double)(long)item;
			else if(this.ItemType== CBORObjectType.BigInteger)
				return (double)(BigInteger)item;
			else if(this.ItemType== CBORObjectType.Single)
				return (double)(float)item;
			else if(this.ItemType== CBORObjectType.Double)
				return (double)item;
			else if(this.Tag==(BigInteger)4 && this.ItemType== CBORObjectType.Array &&
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
			if(this.ItemType== CBORObjectType.Integer)
				return (float)(long)item;
			else if(this.ItemType== CBORObjectType.BigInteger)
				return (float)(BigInteger)item;
			else if(this.ItemType== CBORObjectType.Single)
				return (float)item;
			else if(this.ItemType== CBORObjectType.Double)
				return (float)(double)item;
			else if(this.Tag==4 && this.ItemType== CBORObjectType.Array &&
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
			if(this.ItemType== CBORObjectType.Integer)
				return (BigInteger)(long)item;
			else if(this.ItemType== CBORObjectType.BigInteger)
				return (BigInteger)item;
			else if(this.ItemType== CBORObjectType.Single)
				return (BigInteger)(float)item;
			else if(this.ItemType== CBORObjectType.Double)
				return (BigInteger)(double)item;
			else if(this.Tag==4 && this.ItemType== CBORObjectType.Array &&
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
			if(this.IsFalse || this.IsNull || this.IsUndefined)
				return false;
			return true;
		}
		
		public short AsInt16(){
			int v=AsInt32();
			if(v>Int16.MaxValue || v<0)
				throw new OverflowException();
			return (short)v;
		}
		
		public byte AsByte(){
			int v=AsInt32();
			if(v<0 || v>255)
				throw new OverflowException();
			return (byte)v;
		}

		private static bool IsValidString(string str){
			if(str==null)return false;
			for(int i=0;i<str.Length;i++){
				int c=str[i];
				if(c<=0xD7FF || c>=0xE000) {
					continue;
				} else if(c<=0xDBFF){ // UTF-16 low surrogate
					i++;
					if(i>=str.Length || str[i]<0xDC00 || str[i]>0xDFFF)
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
		public long AsInt64(){
			if(this.ItemType== CBORObjectType.Integer){
				return (long)item;
			} else if(this.ItemType== CBORObjectType.BigInteger){
				if(((BigInteger)item).CompareTo(Int64MaxValue)>0 ||
				   ((BigInteger)item).CompareTo(Int64MinValue)<0)
					throw new OverflowException();
				return (long)(BigInteger)item;
			} else if(this.ItemType== CBORObjectType.Single){
				if(Single.IsNaN((float)item) ||
				   (float)item>Int64.MaxValue || (float)item<Int64.MinValue)
					throw new OverflowException();
				return (long)(float)item;
			} else if(this.ItemType== CBORObjectType.Double){
				if(Double.IsNaN((double)item) ||
				   (double)item>Int64.MinValue || (double)item<Int64.MinValue)
					throw new OverflowException();
				return (long)(double)item;
			} else if(this.Tag==4 && this.ItemType== CBORObjectType.Array &&
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
			if(this.ItemType== CBORObjectType.Integer){
				if((long)item>Int32.MaxValue || (long)item<Int32.MinValue)
					throw new OverflowException();
				return (int)(long)item;
			} else if(this.ItemType== CBORObjectType.BigInteger){
				if((BigInteger)item>Int32.MaxValue || (BigInteger)item<Int32.MinValue)
					throw new OverflowException();
				return (int)(BigInteger)item;
			} else if(this.ItemType== CBORObjectType.Single){
				if(Single.IsNaN((float)item) ||
				   (float)item>Int32.MaxValue || (float)item<Int32.MinValue)
					throw new OverflowException();
				return (int)(float)item;
			} else if(this.ItemType== CBORObjectType.Double){
				if(Double.IsNaN((double)item) ||
				   (double)item>Int32.MinValue || (double)item<Int32.MinValue)
					throw new OverflowException();
				return (int)(double)item;
			} else if(this.Tag==4 && this.ItemType== CBORObjectType.Array &&
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
			if(this.ItemType== CBORObjectType.TextString){
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
			return Read(stream,0,false,-1,null,0);
		}
		
		private static void WriteObjectArray(
			IList<CBORObject> list, Stream s){
			WritePositiveInt(4,list.Count,s);
			foreach(var i in list){
				if(i!=null && i.IsBreak)
					throw new ArgumentException();
				Write(i,s);
			}
		}
		
		private static void WriteObjectMap(
			IDictionary<CBORObject,CBORObject> list, Stream s){
			WritePositiveInt(5,list.Count,s);
			foreach(var key in list.Keys){
				if(key!=null && key.IsBreak)
					throw new ArgumentException();
				var value=list[key];
				if(value!=null && value.IsBreak)
					throw new ArgumentException();
				Write(key,s);
				Write(value,s);
			}
		}

		private static void WritePositiveInt(int type, int value, Stream s){
			if(value<0)
				throw new ArgumentException();
			if(value<24){
				s.WriteByte((byte)((byte)value|(byte)(type<<5)));
			} else if(value<=0xFF){
				s.WriteByte((byte)(24|(type<<5)));
				s.WriteByte((byte)(value&0xFF));
			} else if(value<=0xFFFF){
				s.WriteByte((byte)(25|(type<<5)));
				s.WriteByte((byte)((value>>8)&0xFF));
				s.WriteByte((byte)(value&0xFF));
			} else {
				s.WriteByte((byte)(26|(type<<5)));
				s.WriteByte((byte)((value>>24)&0xFF));
				s.WriteByte((byte)((value>>16)&0xFF));
				s.WriteByte((byte)((value>>8)&0xFF));
				s.WriteByte((byte)(value&0xFF));
			}
		}
		
		private static void WritePositiveInt64(int type, long value, Stream s){
			if(value<0)
				throw new ArgumentException();
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
		
		private const int StreamedStringBufferLength=4096;
		
		private static void WriteStreamedString(String str, Stream stream){
			byte[] bytes=new byte[StreamedStringBufferLength];
			int byteIndex=0;
			bool streaming=false;
			for(int index=0;index<str.Length;index++){
				int c=str[index];
				if(c>=0xD800 && c<=0xDBFF && index+1<str.Length &&
				   str[index+1]>=0xDC00 && str[index+1]<=0xDFFF){
					// Get the Unicode code point for the surrogate pair
					c=0x10000+(c-0xD800)*0x400+(str[index+1]-0xDC00);
					index++;
				} else if(c>=0xD800 && c<=0xDFFF){
					// unpaired surrogate, write U+FFFD instead
					c=0xFFFD;
				}
				if(c<=0x7F){
					if(byteIndex+1>StreamedStringBufferLength){
						// Write bytes retrieved so far
						if(!streaming)
							stream.WriteByte((byte)0x7F);
						WritePositiveInt(3,byteIndex,stream);
						stream.Write(bytes,0,byteIndex);
						byteIndex=0;
						streaming=true;
					}
					bytes[byteIndex++]=(byte)c;
				} else if(c<=0x7FF){
					if(byteIndex+2>StreamedStringBufferLength){
						// Write bytes retrieved so far
						if(!streaming)
							stream.WriteByte((byte)0x7F);
						WritePositiveInt(3,byteIndex,stream);
						stream.Write(bytes,0,byteIndex);
						byteIndex=0;
						streaming=true;
					}
					bytes[byteIndex++]=((byte)(0xC0|((c>>6)&0x1F)));
					bytes[byteIndex++]=((byte)(0x80|(c   &0x3F)));
				} else if(c<=0xFFFF){
					if(byteIndex+3>StreamedStringBufferLength){
						// Write bytes retrieved so far
						if(!streaming)
							stream.WriteByte((byte)0x7F);
						WritePositiveInt(3,byteIndex,stream);
						stream.Write(bytes,0,byteIndex);
						byteIndex=0;
						streaming=true;
					}
					bytes[byteIndex++]=((byte)(0xE0|((c>>12)&0x0F)));
					bytes[byteIndex++]=((byte)(0x80|((c>>6 )&0x3F)));
					bytes[byteIndex++]=((byte)(0x80|(c      &0x3F)));
				} else {
					if(byteIndex+4>StreamedStringBufferLength){
						// Write bytes retrieved so far
						if(!streaming)
							stream.WriteByte((byte)0x7F);
						WritePositiveInt(3,byteIndex,stream);
						stream.Write(bytes,0,byteIndex);
						byteIndex=0;
						streaming=true;
					}
					bytes[byteIndex++]=((byte)(0xF0|((c>>18)&0x07)));
					bytes[byteIndex++]=((byte)(0x80|((c>>12)&0x3F)));
					bytes[byteIndex++]=((byte)(0x80|((c>>6 )&0x3F)));
					bytes[byteIndex++]=((byte)(0x80|(c      &0x3F)));
				}
			}
			WritePositiveInt(3,byteIndex,stream);
			stream.Write(bytes,0,byteIndex);
			if(streaming){
				stream.WriteByte((byte)0xFF);
			}
		}

		
		/// <summary>
		/// Writes a string in CBOR format to a data stream.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="s"></param>
		public static void Write(string str, Stream s){
			if((s)==null)throw new ArgumentNullException("s");
			if(str==null){
				s.WriteByte(0xf6); // Write null instead of string
			} else {
				WriteStreamedString(str,s);
			}
		}
		
		
		private static BigInteger LowestNegativeBigInt=
			BigInteger.Parse("-18446744073709551616",NumberStyles.AllowLeadingSign,
			                 CultureInfo.InvariantCulture);
		
		/// <summary>
		/// Writes a big integer in CBOR format to a data stream.
		/// </summary>
		/// <param name="bi">Big integer to write.</param>
		/// <param name="s">Stream to write to.</param>
		public static void Write(BigInteger bigint, Stream s){
			if((s)==null)throw new ArgumentNullException("s");
			int datatype=(bigint<0) ? 1 : 0;
			if(bigint<0){
				bigint+=1;
				bigint=-bigint;
			}
			if(bigint.CompareTo(Int64MaxValue)<=0){
				// If the big integer is representable in
				// major type 0 or 1, write that major type
				// instead of as a bignum
				long ui=(long)bigint;
				WritePositiveInt64(datatype,ui,s);
			} else {
				List<byte> bytes=new List<byte>();
				while(bigint.CompareTo(BigInteger.Zero)>0){
					bytes.Add((byte)(bigint&0xFF));
					bigint>>=8;
				}
				switch(bytes.Count){
					case 1:
						s.WriteByte((byte)((datatype<<5)|24));
						s.WriteByte(bytes[0]);
						break;
					case 2:
						s.WriteByte((byte)((datatype<<5)|25));
						s.WriteByte(bytes[1]);
						s.WriteByte(bytes[0]);
						break;
					case 3:
						s.WriteByte((byte)((datatype<<5)|26));
						s.WriteByte(bytes[2]);
						s.WriteByte(bytes[1]);
						s.WriteByte(bytes[0]);
						break;
					case 4:
						s.WriteByte((byte)((datatype<<5)|27));
						s.WriteByte(bytes[3]);
						s.WriteByte(bytes[2]);
						s.WriteByte(bytes[1]);
						s.WriteByte(bytes[0]);
						break;
					default:
						s.WriteByte((datatype==0) ?
						            (byte)0xC2 :
						            (byte)0xC3);
						WritePositiveInt(2,bytes.Count,s);
						for(var i=bytes.Count-1;i>=0;i--){
							s.WriteByte(bytes[i]);
						}
						break;
				}
			}
		}
		[Obsolete("This method will be removed in the future.  Use WriteTo(Stream) instead.")]
		public void Write(Stream s){
			WriteTo(s);
		}
		
		/// <summary>
		/// Writes this CBOR object to a data stream.
		/// </summary>
		/// <param name="s">A readable data stream.</param>
		public void WriteTo(Stream s){
			if((s)==null)throw new ArgumentNullException("s");
			if(tagged){
				// Write the tag if this object has one
				BigInteger tag=this.Tag;
				if(tag.CompareTo(Int64MaxValue)<=0){
					WritePositiveInt64(6,(long)tag,s);
				} else {
					s.WriteByte((byte)(0xC0|27));
					s.WriteByte((byte)((tag>>56)&0xFF));
					s.WriteByte((byte)((tag>>48)&0xFF));
					s.WriteByte((byte)((tag>>40)&0xFF));
					s.WriteByte((byte)((tag>>32)&0xFF));
					s.WriteByte((byte)((tag>>24)&0xFF));
					s.WriteByte((byte)((tag>>16)&0xFF));
					s.WriteByte((byte)((tag>>8)&0xFF));
					s.WriteByte((byte)(tag&0xFF));
				}
			}
			if(this.ItemType==CBORObjectType.Integer){
				Write((long)item,s);
			} else if(this.ItemType== CBORObjectType.BigInteger){
				Write((BigInteger)item,s);
			} else if(this.ItemType== CBORObjectType.ByteString){
				WritePositiveInt((this.ItemType== CBORObjectType.ByteString) ? 2 : 3,
				                 ((byte[])item).Length,s);
				s.Write(((byte[])item),0,((byte[])item).Length);
			} else if(this.ItemType== CBORObjectType.TextString ){
				Write((string)item,s);
			} else if(this.ItemType== CBORObjectType.Array){
				WriteObjectArray((IList<CBORObject>)item,s);
			} else if(this.ItemType== CBORObjectType.Map){
				WriteObjectMap((IDictionary<CBORObject,CBORObject>)item,s);
			} else if(this.ItemType== CBORObjectType.SimpleValue){
				int value=(int)item;
				if(value<24 || value==31){
					s.WriteByte((byte)(0xE0+value));
				} else {
					s.WriteByte(0xF8);
					s.WriteByte((byte)value);
				}
			} else if(this.ItemType== CBORObjectType.Single){
				Write((float)item,s);
			} else if(this.ItemType== CBORObjectType.Double){
				Write((double)item,s);
			} else {
				throw new ArgumentException();
			}
		}
		
		public static void Write(long value, Stream s){
			if((s)==null)throw new ArgumentNullException("s");
			if(value>=0){
				WritePositiveInt64(0,value,s);
			} else {
				value+=1;
				value=-value; // Will never overflow
				WritePositiveInt64(1,value,s);
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
		public static void Write(byte value, Stream s){
			if((((int)value)&0xFF)<24){
				s.WriteByte(value);
			} else {
				s.WriteByte((byte)(24));
				s.WriteByte(value);
			}
		}
		public static void Write(float value, Stream s){
			if((s)==null)throw new ArgumentNullException("s");
			int bits=ConverterInternal.SingleToInt32Bits(
				value);
			s.WriteByte(0xFA);
			s.WriteByte((byte)((bits>>24)&0xFF));
			s.WriteByte((byte)((bits>>16)&0xFF));
			s.WriteByte((byte)((bits>>8)&0xFF));
			s.WriteByte((byte)(bits&0xFF));
		}
		public static void Write(double value, Stream s){
			long bits=ConverterInternal.DoubleToInt64Bits(
				(double)value);
			s.WriteByte(0xFB);
			s.WriteByte((byte)((bits>>56)&0xFF));
			s.WriteByte((byte)((bits>>48)&0xFF));
			s.WriteByte((byte)((bits>>40)&0xFF));
			s.WriteByte((byte)((bits>>32)&0xFF));
			s.WriteByte((byte)((bits>>24)&0xFF));
			s.WriteByte((byte)((bits>>16)&0xFF));
			s.WriteByte((byte)((bits>>8)&0xFF));
			s.WriteByte((byte)(bits&0xFF));
		}
		
		/// <summary>
		/// Gets the binary representation of this
		/// data item.
		/// </summary>
		/// <returns>A byte array in CBOR format.</returns>
		public byte[] ToBytes(){
			using(MemoryStream ms=new MemoryStream()){
				WriteTo(ms);
				if(ms.Position>Int32.MaxValue)
					throw new OutOfMemoryException();
				return ms.ToArray();
			}
		}
		
		public static void Write(Object o, Stream s){
			if((s)==null)throw new ArgumentNullException("s");
			if(o==null){
				s.WriteByte(0xf6);
			} else if(o is byte[]){
				byte[] data=(byte[])o;
				WritePositiveInt(3,data.Length,s);
				s.Write(data,0,data.Length);
			} else if(o is IList<CBORObject>){
				WriteObjectArray((IList<CBORObject>)o,s);
			} else if(o is IDictionary<CBORObject,CBORObject>){
				IDictionary<CBORObject,CBORObject> dic=
					(IDictionary<CBORObject,CBORObject>)o;
				WritePositiveInt(5,dic.Count,s);
				foreach(var i in dic.Keys){
					if(i!=null && i.IsBreak)
						throw new ArgumentException();
					var value=dic[i];
					if(value!=null && value.IsBreak)
						throw new ArgumentException();
					Write(i,s);
					Write(value,s);
				}
			} else if(o is IDictionary<CBORObject,CBORObject>){
				WriteObjectMap((IDictionary<CBORObject,CBORObject>)o,s);
			} else {
				FromObject(o).WriteTo(s);
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
		
		private static string Base64URL="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
		private static string Base64="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
		private static void ToBase64(StringBuilder str, byte[] data, string alphabet, bool padding){
			int length = data.Length;
			int i=0;
			for (i = 0; i < (length - 2); i += 3) {
				str.Append(alphabet[(data[i] >> 2)&63]);
				str.Append(alphabet[((data[i] & 3) << 4) + ((data[i+1] >> 4)&63)]);
				str.Append(alphabet[((data[i+1] & 15) << 2) + ((data[i+2] >> 6)&3)]);
				str.Append(alphabet[data[i+2] & 63]);
			}
			int lenmod3=(length%3);
			if (lenmod3!=0) {
				i = length - lenmod3;
				str.Append(alphabet[(data[i] >> 2)&63]);
				if (lenmod3 == 2) {
					str.Append(alphabet[((data[i] & 3) << 4) + ((data[i+1] >> 4)&63)]);
					str.Append(alphabet[(data[i+1] & 15) << 2]);
					if(padding)str.Append("=");
				} else {
					str.Append(alphabet[(data[i] & 3) << 4]);
					if(padding)str.Append("==");
				}
			}
		}
		private static void ToBase16(StringBuilder str, byte[] data){
			var alphabet="0123456789ABCDEF";
			var length = data.Length;
			for (var i = 0; i < length;i++) {
				str.Append(alphabet[(data[i]>>4)&15]);
				str.Append(alphabet[data[i]&15]);
			}
		}
		
		private static string StringToJSONString(string str){
			StringBuilder sb=new StringBuilder();
			sb.Append("\"");
			// Surrogates were already verified when this
			// string was added to the CBOR object; that check
			// is not repeated here
			for(int i=0;i<str.Length;i++){
				char c=str[i];
				if(c=='\\' || c=='"' || c<0x20){
					sb.Append('\\');
				}
				sb.Append(c);
			}
			sb.Append("\"");
			return sb.ToString();
		}
		
		/// <summary>
		/// Converts this object to a JSON string.  This function
		/// not only accepts arrays and maps (the only proper
		/// JSON objects under RFC 4627), but also integers,
		/// strings, byte arrays, and other JSON data types.
		/// </summary>
		public string ToJSONString(){
			if(this.ItemType== CBORObjectType.SimpleValue){
				if(this.IsTrue)return "true";
				else if(this.IsFalse)return "false";
				else if(this.IsNull)return "null";
				else return "null";
			} else if(this.ItemType== CBORObjectType.Single){
				float f=(float)item;
				if(Single.IsNegativeInfinity(f) ||
				   Single.IsPositiveInfinity(f) ||
				   Single.IsNaN(f)) return "null";
				else
					return Convert.ToString(f,CultureInfo.InvariantCulture);
			} else if(this.ItemType== CBORObjectType.Double){
				double f=(double)item;
				if(Double.IsNegativeInfinity(f) ||
				   Double.IsPositiveInfinity(f) ||
				   Double.IsNaN(f)) return "null";
				else
					return Convert.ToString(f,CultureInfo.InvariantCulture);
			} else if(this.ItemType== CBORObjectType.Integer){
				return String.Format(CultureInfo.InvariantCulture,
				                     "{0}",(long)item);
			} else if(this.ItemType== CBORObjectType.BigInteger){
				return String.Format(CultureInfo.InvariantCulture,
				                     "{0}",(BigInteger)item);
			} else if(this.ItemType== CBORObjectType.ByteString){
				StringBuilder sb=new StringBuilder();
				sb.Append('\"');
				if(this.Tag==22){
					ToBase64(sb,(byte[])item,Base64,false);
				} else if(this.Tag==23){
					ToBase16(sb,(byte[])item);
				} else {
					ToBase64(sb,(byte[])item,Base64URL,false);
				}
				sb.Append('\"');
				return sb.ToString();
			} else if(this.ItemType== CBORObjectType.TextString){
				return StringToJSONString((string)item);
			} else if(this.ItemType== CBORObjectType.Array){
				if(this.Tag==4 && this.Count==2){
					BigInteger exponent=this[0].AsBigInteger();
					string mantissa=this[1].IntegerToString();
					StringBuilder sb=new StringBuilder();
					if(exponent<0 && mantissa.Length+exponent>0){
						int pos=(int)(mantissa.Length+exponent);
						sb.Append(mantissa.Substring(0,pos));
						sb.Append(".");
						sb.Append(mantissa.Substring(pos));
					} else if(exponent<0 && mantissa.Length+exponent==0){
						sb.Append("0.");
						sb.Append(mantissa);
					} else {
						sb.Append(this[1].IntegerToString());
						if(!exponent.IsZero){
							sb.Append("e");
							sb.Append(this[0].IntegerToString());
						}
					}
					return sb.ToString();
				} else {
					StringBuilder sb=new StringBuilder();
					bool first=true;
					sb.Append("[");
					foreach(var i in (IList<CBORObject>)item){
						if(!first)sb.Append(", ");
						sb.Append(i.ToJSONString());
						first=false;
					}
					sb.Append("]");
					return sb.ToString();
				}
			} else if(this.ItemType== CBORObjectType.Map){
				var dict=new Dictionary<string,CBORObject>();
				StringBuilder sb=new StringBuilder();
				bool first=true;
				foreach(CBORObject key in ((IDictionary<CBORObject,CBORObject>)item).Keys){
					var str=(key.ItemType== CBORObjectType.TextString) ?
						key.AsString() : key.ToJSONString();
					dict[str]=(((IDictionary<CBORObject,CBORObject>)item)[key]);
				}
				sb.Append("{");
				foreach(var key in dict.Keys){
					if(!first)sb.Append(", ");
					sb.Append(StringToJSONString(key));
					sb.Append(':');
					sb.Append(dict[key].ToJSONString());
					first=false;
				}
				sb.Append("}");
				return sb.ToString();
			} else {
				throw new InvalidOperationException();
			}
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
						if(obj.ItemType!= CBORObjectType.TextString)
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
						bytesNeeded=1;
						cp=b-0xc0;
					} else if(b>=0xe0 && b<=0xef){
						lower=(b==0xe0) ? 0xa0 : 0x80;
						upper=(b==0xed) ? 0x9f : 0xbf;
						bytesNeeded=2;
						cp=b-0xe0;
					} else if(b>=0xf0 && b<=0xf4){
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
			return new CBORObject(CBORObjectType.Integer,value);
		}
		public static CBORObject FromObject(CBORObject value){
			if(value==null)return CBORObject.Null;
			return value;
		}

		public static CBORObject FromObject(BigInteger value){
			if(value.CompareTo(Int64MinValue)>=0 &&
			   value.CompareTo(Int64.MaxValue)<=0){
				return new CBORObject(CBORObjectType.Integer,(long)value);
			} else {
				return new CBORObject(CBORObjectType.BigInteger,value);
			}
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
		public static CBORObject FromObject(byte value){
			return FromObject(((int)value)&0xFF);
		}
		public static CBORObject FromObject(float value){
			return new CBORObject(CBORObjectType.Single,value);
		}
		public static CBORObject FromObject(double value){
			return new CBORObject(CBORObjectType.Double,value);
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
			var list=new Dictionary<CBORObject,CBORObject>();
			foreach(var i in dic.Keys){
				var key=FromObject(i);
				if(key!=null && key.IsBreak)
					throw new ArgumentException();
				var value=FromObject(dic[i]);
				if(value!=null && value.IsBreak)
					throw new ArgumentException();
				list[key]=value;
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
			if(o is byte[])return FromObject((byte[])o);
			if(o is IDictionary<CBORObject,CBORObject>)return FromObject(
				(IDictionary<CBORObject,CBORObject>)o);
			if(o is IDictionary<string,CBORObject>)return FromObject(
				(IDictionary<string,CBORObject>)o);
			throw new ArgumentException();
		}
		

		public static CBORObject FromObjectAndTag(Object o, BigInteger tag){
			if((tag)<BigInteger.Zero)throw new ArgumentOutOfRangeException("tag"+" not greater or equal to 0 ("+Convert.ToString(tag,System.Globalization.CultureInfo.InvariantCulture)+")");
			if((tag)>UInt64.MaxValue)throw new ArgumentOutOfRangeException("tag"+" not less or equal to "+Convert.ToString(UInt64.MaxValue)+" ("+Convert.ToString(tag,System.Globalization.CultureInfo.InvariantCulture)+")");
			CBORObject c=FromObject(o);
			int low=unchecked((int)(tag&(int)0xFFFFFFFF));
			int high=unchecked((int)((tag>>32)&(int)0xFFFFFFFF));
			return new CBORObject(c.ItemType,low,high,c.item);
		}

		public static CBORObject FromObjectAndTag(Object o, int tag){
			if((tag)<0)throw new ArgumentOutOfRangeException("tag"+" not greater or equal to "+"0"+" ("+Convert.ToString(tag,System.Globalization.CultureInfo.InvariantCulture)+")");
			CBORObject c=FromObject(o);
			return new CBORObject(c.ItemType,tag,0,c.item);
		}
		
		//-----------------------------------------------------------
		
		private string IntegerToString(){
			if(this.ItemType== CBORObjectType.Integer){
				return String.Format(CultureInfo.InvariantCulture,
				                     "{0}",(long)item);
			} else if(this.ItemType== CBORObjectType.BigInteger){
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
				sb.Append(String.Format(CultureInfo.InvariantCulture,"{0}",
				                        this.Tag));
				sb.Append('(');
			}
			if(this.ItemType== CBORObjectType.SimpleValue){
				if(this.IsBreak)sb.Append("break");
				else if(this.IsTrue)sb.Append("true");
				else if(this.IsFalse)sb.Append("false");
				else if(this.IsNull)sb.Append("null");
				else if(this.IsUndefined)sb.Append("undefined");
				else {
					sb.Append(String.Format(CultureInfo.InvariantCulture,
					                        "simple({0})",(int)item));
				}
			} else if(this.ItemType== CBORObjectType.Single){
				float f=(float)item;
				if(Single.IsNegativeInfinity(f))
					sb.Append("-Infinity");
				else if(Single.IsPositiveInfinity(f))
					sb.Append("Infinity");
				else if(Single.IsNaN(f))
					sb.Append("NaN");
				else
					sb.Append(Convert.ToString(f,CultureInfo.InvariantCulture));
			} else if(this.ItemType== CBORObjectType.Double){
				double f=(double)item;
				if(Double.IsNegativeInfinity(f))
					sb.Append("-Infinity");
				else if(Double.IsPositiveInfinity(f))
					sb.Append("Infinity");
				else if(Double.IsNaN(f))
					sb.Append("NaN");
				else
					sb.Append(Convert.ToString(f,CultureInfo.InvariantCulture));
			} else if(this.ItemType== CBORObjectType.Integer){
				sb.Append(Convert.ToString((long)item,CultureInfo.InvariantCulture));
			} else if(this.ItemType== CBORObjectType.BigInteger){
				sb.Append(String.Format(CultureInfo.InvariantCulture,
				                        "{0}",(BigInteger)item));
			} else if(this.ItemType== CBORObjectType.ByteString){
				sb.Append("h'");
				byte[] data=(byte[])item;
				ToBase16(sb,data);
				sb.Append("'");
			} else if(this.ItemType== CBORObjectType.TextString){
				sb.Append("\"");
				sb.Append((string)item);
				sb.Append("\"");
			} else if(this.ItemType== CBORObjectType.Array){
				bool first=true;
				sb.Append("[");
				foreach(var i in (IList<CBORObject>)item){
					if(!first)sb.Append(", ");
					sb.Append(i);
					first=false;
				}
				sb.Append("]");
			} else if(this.ItemType== CBORObjectType.Map){
				bool first=true;
				sb.Append("{");
				foreach(var key in ((IDictionary<CBORObject,CBORObject>)item).Keys){
					if(!first)sb.Append(", ");
					sb.Append(key);
					sb.Append(": ");
					sb.Append(((IDictionary<CBORObject,CBORObject>)item)[key]);
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

		private static bool CheckMajorTypeIndex(int type, int index, int[] validTypeFlags){
			if(validTypeFlags==null || index<0 || index>=validTypeFlags.Length)
				return false;
			if(type<0 || type>7)
				return false;
			return (validTypeFlags[index]&(1<<type))!=0;
		}
		
		private static CBORObject Read(
			Stream s,
			int depth,
			bool allowBreak,
			int allowOnlyType,
			int[] validTypeFlags,
			int validTypeIndex
		){
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
			if(allowOnlyType>=0 &&
			   (allowOnlyType!=type || additional>=28)){
				throw new FormatException();
			}
			if(validTypeFlags!=null){
				// Check for valid major types if asked
				if(!CheckMajorTypeIndex(type,validTypeIndex,validTypeFlags)){
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
					long x=0;
					for(int i=0;i<cs.Length;i++){
						x<<=8;
						x|=((long)cs[i])&0xFF;
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
							unchecked(x));
						return new CBORObject(CBORObjectType.Double,f);
					}
				}
				throw new FormatException();
			}
			long uadditional=0;
			BigInteger bigintAdditional=0;
			bool hasBigAdditional=false;
			if(additional<=23){
				uadditional=(long)additional;
			} else if(additional==24){
				int c1=s.ReadByte();
				if(c1<0)
					throw new IOException();
				uadditional=(long)c1;
			} else if(additional==25 || additional==26){
				byte[] cs=new byte[(additional==25) ? 2 : 4];
				if(s.Read(cs,0,cs.Length)!=cs.Length)
					throw new IOException();
				long x=0;
				for(int i=0;i<cs.Length;i++){
					x<<=8;
					x|=((long)cs[i])&0xFF;
				}
				uadditional=x;
			} else if(additional==27){
				byte[] cs=new byte[8];
				if(s.Read(cs,0,cs.Length)!=cs.Length)
					throw new IOException();
				long x=0;
				for(int i=0;i<cs.Length;i++){
					x<<=8;
					x|=((long)cs[i])&0xFF;
				}
				if(((x>>63)&1)!=0){
					// uadditional requires 64 bits to
					// remain unsigned, so convert to
					// big integer
					hasBigAdditional=true;
					// bigintAdditional already set to 0
					for(int i=0;i<cs.Length;i++){
						bigintAdditional<<=8;
						bigintAdditional|=cs[i];
					}
				} else {
					uadditional=x;
				}
			} else if(additional==28 || additional==29 ||
			          additional==30){
				throw new FormatException();
			} else if(additional==31 && type<2){
				throw new FormatException();
			}
			if(type==0){
				if(hasBigAdditional)
					return FromObject(bigintAdditional);
				else
					return FromObject(uadditional);
			} else if(type==1){
				if(hasBigAdditional){
					bigintAdditional=new BigInteger(-1)-bigintAdditional;
					return FromObject(bigintAdditional);
				} else if(uadditional<=Int64.MaxValue){
					return FromObject(((long)-1-(long)uadditional));
				} else {
					BigInteger bi=new BigInteger(-1);
					bi-=new BigInteger(uadditional);
					return FromObject(bi);
				}
			} else if(type==2){ // Byte string
				if(additional==31){
					// Streaming byte string
					using(MemoryStream ms=new MemoryStream()){
						byte[] data;
						// Requires same type as this one
						int[] subFlags=new int[]{(1<<type)};
						while(true){
							CBORObject o=Read(s,depth+1,true,type,subFlags,0);
							if(o.IsBreak)
								break;
							data=(byte[])o.item;
							ms.Write(data,0,data.Length);
						}
						if(ms.Position>Int32.MaxValue)
							throw new IOException();
						data=ms.ToArray();
						return new CBORObject(
							CBORObjectType.ByteString,
							data);
					}
				} else {
					if(hasBigAdditional || uadditional>Int32.MaxValue){
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
					// Requires same type as this one
					int[] subFlags=new int[]{(1<<type)};
					while(true){
						CBORObject o=Read(s,depth+1,true,type,subFlags,0);
						if(o.IsBreak)
							break;
						builder.Append((string)o.item);
					}
					return new CBORObject(
						CBORObjectType.TextString,
						builder.ToString());
				} else {
					if(hasBigAdditional || uadditional>=Int32.MaxValue){
						throw new IOException();
					}
					string str=ReadUtf8(s,(int)uadditional);
					return new CBORObject(CBORObjectType.TextString,str);
				}
			} else if(type==4){ // Array
				IList<CBORObject> list=new List<CBORObject>();
				int vtindex=1;
				if(additional==31){
					while(true){
						CBORObject o=Read(s,depth+1,true,-1,validTypeFlags,vtindex);
						if(o.IsBreak)
							break;
						list.Add(o);
						vtindex++;
					}
					return new CBORObject(CBORObjectType.Array,list);
				} else {
					if(hasBigAdditional || uadditional>=Int32.MaxValue){
						throw new IOException();
					}
					for(long i=0;i<uadditional;i++){
						list.Add(Read(s,depth+1,false,-1,validTypeFlags,vtindex));
						vtindex++;
					}
					return new CBORObject(CBORObjectType.Array,list);
				}
			} else if(type==5){ // Map, type 5
				var list=new Dictionary<CBORObject,CBORObject>();
				if(additional==31){
					while(true){
						CBORObject key=Read(s,depth+1,true,-1,null,0);
						if(key.IsBreak)
							break;
						CBORObject value=Read(s,depth+1,false,-1,null,0);
						list[key]=value;
					}
					return new CBORObject(CBORObjectType.Map,list);
				} else {
					if(hasBigAdditional || uadditional>=Int32.MaxValue){
						throw new IOException();
					}
					for(long i=0;i<uadditional;i++){
						CBORObject key=Read(s,depth+1,false,-1,null,0);
						CBORObject value=Read(s,depth+1,false,-1,null,0);
						list[key]=value;
					}
					return new CBORObject(CBORObjectType.Map,list);
				}
			} else { // Tagged item
				CBORObject o;
				if(!hasBigAdditional){
					if(uadditional==0){
						// Requires a text string
						int[] subFlags=new int[]{(1<<3)};
						o=Read(s,depth+1,allowBreak,-1,subFlags,0);
					} else if(uadditional==2 || uadditional==3){
						// Big number
						// Requires a byte string
						int[] subFlags=new int[]{(1<<2)};
						o=Read(s,depth+1,allowBreak,-1,subFlags,0);
						byte[] data=(byte[])o.item;
						BigInteger bi=0;
						for(int i=0;i<data.Length;i++){
							bi<<=8;
							bi|=data[i];
						}
						if(uadditional==3){
							bi=-1-bi; // Convert to a negative
						}
						return FromObject(bi);
					} else if(uadditional==4){
						// Requires an array with two elements of
						// a valid type
						int[] subFlags=new int[]{
							(1<<4), // array
							(1<<0)|(1<<1), // exponent
							(1<<0)|(1<<1)|(1<<6) // mantissa
						};
						o=Read(s,depth+1,allowBreak,-1,subFlags,0);
						if(o.Count!=2) // Requires 2 items
							throw new FormatException();
						// check type of mantissa
						if(o[1].ItemType!=CBORObjectType.Integer &&
						   o[1].ItemType!=CBORObjectType.BigInteger)
							throw new FormatException();
					} else {
						o=Read(s,depth+1,allowBreak,-1,null,0);
					}
				} else {
					o=Read(s,depth+1,allowBreak,-1,null,0);
				}
				if(hasBigAdditional){
					return FromObjectAndTag(o,bigintAdditional);
				} else {
					return FromObjectAndTag(
						o,(BigInteger)uadditional);
				}
			}
		}
	}
}