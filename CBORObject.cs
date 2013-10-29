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
		private int ItemType {
			get {
				return itemtype_;
			}
		}
		
		int itemtype_;
		private const int CBORObjectType_Integer=0;
		private const int CBORObjectType_BigInteger=1;
		private const int CBORObjectType_ByteString=2;
		private const int CBORObjectType_TextString=3;
		private const int CBORObjectType_Array=4;
		private const int CBORObjectType_Map=5;
		private const int CBORObjectType_SimpleValue=6;
		private const int CBORObjectType_Single=7;
		private const int CBORObjectType_Double=8;
		Object item;
		bool tagged=false;
		int tagLow=0;
		int tagHigh=0;

		private static readonly BigInteger Int64MaxValue=(BigInteger)Int64.MaxValue;
		private static readonly BigInteger Int64MinValue=(BigInteger)Int64.MinValue;
		public static readonly CBORObject Break=new CBORObject(CBORObjectType_SimpleValue,31);
		public static readonly CBORObject False=new CBORObject(CBORObjectType_SimpleValue,20);
		public static readonly CBORObject True=new CBORObject(CBORObjectType_SimpleValue,21);
		public static readonly CBORObject Null=new CBORObject(CBORObjectType_SimpleValue,22);
		public static readonly CBORObject Undefined=new CBORObject(CBORObjectType_SimpleValue,23);
		
		private CBORObject(){}
		
		private CBORObject(int type, int tagLow, int tagHigh, Object item) : this(type,item) {
			this.itemtype_=type;
			this.tagLow=tagLow;
			this.tagHigh=tagHigh;
			this.tagged=true;
		}
		private CBORObject(int type, Object item){
			#if DEBUG
			// Check range in debug mode to ensure that Integer and BigInteger
			// are unambiguous
			if((type== CBORObjectType_BigInteger) &&
			   ((BigInteger)item).CompareTo(Int64MinValue)>=0 &&
			   ((BigInteger)item).CompareTo(Int64MaxValue)<=0){
				if(!(false))throw new ArgumentException("Big integer is within range for Integer");

			}
			#endif
			this.itemtype_=type;
			this.item=item;
		}
		
		public bool IsBreak {
			get {
				return this.ItemType==CBORObjectType_SimpleValue && (int)item==31;
			}
		}
		public bool IsTrue {
			get {
				return this.ItemType==CBORObjectType_SimpleValue && (int)item==21;
			}
		}
		public bool IsFalse {
			get {
				return this.ItemType==CBORObjectType_SimpleValue && (int)item==20;
			}
		}
		public bool IsNull {
			get {
				return this.ItemType==CBORObjectType_SimpleValue && (int)item==22;
			}
		}
		public bool IsUndefined {
			get {
				return this.ItemType==CBORObjectType_SimpleValue && (int)item==23;
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
				if(!CBORArrayEquals(AsList(),other.item as IList<CBORObject>))
					return false;
			} else if(item is IDictionary<CBORObject,CBORObject>){
				if(!CBORMapEquals(AsMap(),
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
						itemHashCode=CBORArrayHashCode(AsList());
					else if(item is IDictionary<CBORObject,CBORObject>)
						itemHashCode=CBORMapHashCode(AsMap());
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
		
		private static string GetOptimizedStringIfShortAscii(
			byte[] data, int offset
		){
			int length=data.Length;
			if(length>offset){
				int nextbyte=((int)(data[1]&(int)0xFF));
				int offsetp1=1+offset;
				if(nextbyte>=0x60 && nextbyte<0x78){
					// Check for type 3 string of short length
					if(length!=offsetp1+(nextbyte-0x60))
						throw new FormatException();
					bool issimple=true;
					// Check for all ASCII text
					for(int i=offsetp1;i<length;i++){
						if((data[i]&((byte)0x80))!=0){
							issimple=false;
							break;
						}
					}
					if(issimple){
						char[] c=new char[length-offsetp1];
						for(int i=offsetp1;i<length;i++){
							c[i-offsetp1]=((char)(data[i]&(int)0xFF));
						}
						return new String(c);
					}
				}
			}
			return null;
		}
		
		public static CBORObject FromBytes(byte[] data){
			if((data)==null)throw new ArgumentNullException("data");
			if((data).Length==0)throw new FormatException("data is empty.");
			int firstbyte=((int)(data[0]&(int)0xFF));
			int length=data.Length;
			// Check for simple cases
			if(firstbyte<0x18 && length==1){
				return new CBORObject(CBORObjectType_Integer,(long)firstbyte);
			}
			else if(firstbyte>=0x20 && firstbyte<0x38 && length==1){
				return new CBORObject(
					CBORObjectType_Integer,(long)(-1-(firstbyte-0x20)));
			}
			else if(firstbyte==56 || firstbyte==24){
				if(length!=2)throw new FormatException();
				int nextbyte=((int)(data[1]&(int)0xFF));
				return new CBORObject(
					CBORObjectType_Integer,
					(firstbyte==24) ? (long)nextbyte : (long)(-1-(nextbyte)));
			}
			else if(firstbyte==57 || firstbyte==25){
				if(length!=3)throw new FormatException();
				int v=(((int)(data[1]&(int)0xFF))<<8);
				v|=(((int)(data[2]&(int)0xFF)));
				return new CBORObject(
					CBORObjectType_Integer,
					(firstbyte==25) ? (long)v : (long)(-1-v));
			}
			else if(firstbyte==58 || firstbyte==26){
				if(length!=5)throw new FormatException();
				long v=(((long)(data[1]&(long)0xFF))<<24);
				v|=(((long)(data[2]&(long)0xFF))<<16);
				v|=(((long)(data[3]&(long)0xFF))<<8);
				v|=(((long)(data[4]&(long)0xFF)));
				return new CBORObject(
					CBORObjectType_Integer,
					(firstbyte==26) ? (long)v : (long)(-1-v));
			}
			else if(firstbyte==59 || firstbyte==27){
				if(length!=9)throw new FormatException();
				int topbyte=((int)(data[1]&(int)0xFF));
				if(topbyte==0){
					// Nonzero top bytes indicate that more
					// complicated processing may be necessary
					// for this integer; if the top byte is 0,
					// on the other hand, the whole integer
					// can fit comfortably in the type LONG.
					long v=(((long)(data[2]&(long)0xFF))<<48);
					v|=(((long)(data[3]&(long)0xFF))<<40);
					v|=(((long)(data[4]&(long)0xFF))<<32);
					v|=(((long)(data[5]&(long)0xFF))<<24);
					v|=(((long)(data[6]&(long)0xFF))<<16);
					v|=(((long)(data[7]&(long)0xFF))<<8);
					v|=(((long)(data[8]&(long)0xFF)));
					return new CBORObject(
						CBORObjectType_Integer,
						(firstbyte==27) ? (long)v : (long)(-1-v));
				}
			}
			else if(firstbyte>=0x60 && firstbyte<0x78){
				String s=GetOptimizedStringIfShortAscii(data,0);
				if(s!=null)return new CBORObject(CBORObjectType_TextString,s);
			}
			else if(firstbyte==0xc0){
				String s=GetOptimizedStringIfShortAscii(data,1);
				if(s!=null)return new CBORObject(CBORObjectType_TextString,0,0,s);
			}
			else if(firstbyte==0xF4 && length==1)return CBORObject.False;
			else if(firstbyte==0xF5 && length==1)return CBORObject.True;
			else if(firstbyte==0xF6 && length==1)return CBORObject.Null;
			else if(firstbyte==0xF7 && length==1)return CBORObject.Undefined;
			else if(firstbyte==0xf9){
				if(length!=3)throw new FormatException();
				int v=(((int)(data[1]&(int)0xFF))<<8);
				v|=(((int)(data[2]&(int)0xFF)));
				return new CBORObject(
					CBORObjectType_Single,HalfPrecisionToSingle(v));
			} else if(firstbyte==0xfa){
				if(length!=5)throw new FormatException();
				int v=(((int)(data[1]&(int)0xFF))<<24);
				v|=(((int)(data[2]&(int)0xFF))<<16);
				v|=(((int)(data[3]&(int)0xFF))<<8);
				v|=(((int)(data[4]&(int)0xFF)));
				return new CBORObject(
					CBORObjectType_Single,ConverterInternal.Int32BitsToSingle(v));
			} else if(firstbyte==0xfb){
				if(length!=9)throw new FormatException();
				long v=(((long)(data[1]&(long)0xFF))<<56);
				v|=(((long)(data[2]&(long)0xFF))<<48);
				v|=(((long)(data[3]&(long)0xFF))<<40);
				v|=(((long)(data[4]&(long)0xFF))<<32);
				v|=(((long)(data[5]&(long)0xFF))<<24);
				v|=(((long)(data[6]&(long)0xFF))<<16);
				v|=(((long)(data[7]&(long)0xFF))<<8);
				v|=(((long)(data[8]&(long)0xFF)));
				return new CBORObject(
					CBORObjectType_Double,ConverterInternal.Int64BitsToDouble(v));
			}
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
				if(this.ItemType== CBORObjectType_Array){
					return (AsList()).Count;
				} else if(this.ItemType== CBORObjectType_Map){
					return (AsMap()).Count;
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
					if(tagHigh==0 && tagLow>=0 && tagLow<0x10000){
						return (BigInteger)tagLow;
					}
					BigInteger bi;
					if(tagHigh!=0){
						bi=(BigInteger)((tagHigh>>16)&0xFFFF);
						bi<<=16;
						bi|=(BigInteger)((tagHigh)&0xFFFF);
						bi<<=16;
					} else {
						bi=BigInteger.Zero;
					}
					bi|=(BigInteger)((tagLow>>16)&0xFFFF);
					bi<<=16;
					bi|=(BigInteger)((tagLow)&0xFFFF);
					return bi;
				} else {
					return BigInteger.Zero;
				}
			}
		}

		private IDictionary<CBORObject,CBORObject> AsMap(){
			return (IDictionary<CBORObject,CBORObject>)item;
		}
		
		private IList<CBORObject> AsList(){
			return (IList<CBORObject>)item;
		}
		
		/// <summary>
		/// Gets or sets the value of a CBOR object in this
		/// array.
		/// </summary>
		public CBORObject this[int index]{
			get {
				if(this.ItemType== CBORObjectType_Array){
					IList<CBORObject> list=AsList();
					return list[index];
				} else {
					throw new InvalidOperationException("Not an array");
				}
			}
			set {
				if(this.ItemType== CBORObjectType_Array){
					if(this.Tag==(BigInteger)4)
						throw new InvalidOperationException("Read-only array");
					IList<CBORObject> list=AsList();
					list[index]=value;
				} else {
					throw new InvalidOperationException("Not an array");
				}
			}
		}

		public ICollection<CBORObject> Keys {
			get {
				if(this.ItemType== CBORObjectType_Map){
					IDictionary<CBORObject,CBORObject> dict=AsMap();
					return dict.Keys;
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
				if(this.ItemType== CBORObjectType_Map){
					IDictionary<CBORObject,CBORObject> map=AsMap();
					return map[key];
				} else {
					throw new InvalidOperationException("Not a map");
				}
			}
			set {
				if(this.ItemType== CBORObjectType_Map){
					IDictionary<CBORObject,CBORObject> map=AsMap();
					map[key]=value;
				} else {
					throw new InvalidOperationException("Not a map");
				}
			}
		}
		
		public void Add(CBORObject key, CBORObject value){
			if(this.ItemType== CBORObjectType_Map){
				IDictionary<CBORObject,CBORObject> map=AsMap();
				map.Add(key,value);
			} else {
				throw new InvalidOperationException("Not a map");
			}
		}

		public void ContainsKey(CBORObject key){
			if(this.ItemType== CBORObjectType_Map){
				IDictionary<CBORObject,CBORObject> map=AsMap();
				map.ContainsKey(key);
			} else {
				throw new InvalidOperationException("Not a map");
			}
		}

		public void Add(CBORObject obj){
			if(this.ItemType== CBORObjectType_Array){
				if(this.Tag==(BigInteger)4)
					throw new InvalidOperationException("Read-only array");
				IList<CBORObject> list=AsList();
				list.Add(obj);
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
			if(this.ItemType== CBORObjectType_Integer)
				return (double)(long)item;
			else if(this.ItemType== CBORObjectType_BigInteger)
				return (double)(BigInteger)item;
			else if(this.ItemType== CBORObjectType_Single)
				return (double)(float)item;
			else if(this.ItemType== CBORObjectType_Double)
				return (double)item;
			else if(this.Tag==(BigInteger)4 && this.ItemType== CBORObjectType_Array &&
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
			if(this.ItemType== CBORObjectType_Integer)
				return (float)(long)item;
			else if(this.ItemType== CBORObjectType_BigInteger)
				return (float)(BigInteger)item;
			else if(this.ItemType== CBORObjectType_Single)
				return (float)item;
			else if(this.ItemType== CBORObjectType_Double)
				return (float)(double)item;
			else if(this.Tag==(BigInteger)4 &&
			        this.ItemType== CBORObjectType_Array &&
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
			if(this.ItemType== CBORObjectType_Integer)
				return (BigInteger)(long)item;
			else if(this.ItemType== CBORObjectType_BigInteger)
				return (BigInteger)item;
			else if(this.ItemType== CBORObjectType_Single)
				return (BigInteger)(float)item;
			else if(this.ItemType== CBORObjectType_Double)
				return (BigInteger)(double)item;
			else if(this.Tag==(BigInteger)4 && this.ItemType== CBORObjectType_Array &&
			        this.Count==2){
				StringBuilder sb=new StringBuilder();
				sb.Append(this[1].IntegerToString());
				sb.Append("e");
				sb.Append(this[0].IntegerToString());
				return ParseBigIntegerWithExponent(sb.ToString());
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
			if(this.ItemType== CBORObjectType_Integer){
				return (long)item;
			} else if(this.ItemType== CBORObjectType_BigInteger){
				if(((BigInteger)item).CompareTo(Int64MaxValue)>0 ||
				   ((BigInteger)item).CompareTo(Int64MinValue)<0)
					throw new OverflowException();
				return (long)(BigInteger)item;
			} else if(this.ItemType== CBORObjectType_Single){
				if(Single.IsNaN((float)item) ||
				   (float)item>Int64.MaxValue || (float)item<Int64.MinValue)
					throw new OverflowException();
				return (long)(float)item;
			} else if(this.ItemType== CBORObjectType_Double){
				if(Double.IsNaN((double)item) ||
				   (double)item>Int64.MinValue || (double)item<Int64.MinValue)
					throw new OverflowException();
				return (long)(double)item;
			} else if(this.Tag==(BigInteger)4 && this.ItemType== CBORObjectType_Array &&
			          this.Count==2){
				StringBuilder sb=new StringBuilder();
				sb.Append(this[1].IntegerToString());
				sb.Append("e");
				sb.Append(this[0].IntegerToString());
				BigInteger bi=ParseBigIntegerWithExponent(sb.ToString());
				if(bi.CompareTo(Int64MaxValue)>0 ||
				   bi.CompareTo(Int64MinValue)<0)
					throw new OverflowException();
				return (long)bi;
			} else
				throw new InvalidOperationException("Not a number type");
		}
		
		/// <summary>
		/// Converts this object to a 32-bit signed
		/// integer.  Floating point values are truncatedf
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
			if(this.ItemType== CBORObjectType_Integer){
				if((long)item>Int32.MaxValue || (long)item<Int32.MinValue)
					throw new OverflowException();
				return (int)(long)item;
			} else if(this.ItemType== CBORObjectType_BigInteger){
				if(((BigInteger)item).CompareTo((BigInteger)Int32.MaxValue)>0 ||
				   ((BigInteger)item).CompareTo((BigInteger)Int32.MinValue)<0)
					throw new OverflowException();
				return (int)(BigInteger)item;
			} else if(this.ItemType== CBORObjectType_Single){
				if(Single.IsNaN((float)item) ||
				   (float)item>Int32.MaxValue || (float)item<Int32.MinValue)
					throw new OverflowException();
				return (int)(float)item;
			} else if(this.ItemType== CBORObjectType_Double){
				if(Double.IsNaN((double)item) ||
				   (double)item>Int32.MinValue || (double)item<Int32.MinValue)
					throw new OverflowException();
				return (int)(double)item;
			} else if(this.Tag==(BigInteger)4 && this.ItemType== CBORObjectType_Array &&
			          this.Count==2){
				StringBuilder sb=new StringBuilder();
				sb.Append(this[1].IntegerToString());
				sb.Append("e");
				sb.Append(this[0].IntegerToString());
				BigInteger bi=ParseBigIntegerWithExponent(sb.ToString());
				if(bi.CompareTo((BigInteger)Int32.MaxValue)>0 ||
				   bi.CompareTo((BigInteger)Int32.MinValue)<0)
					throw new OverflowException();
				return (int)bi;
			} else
				throw new InvalidOperationException("Not a number type");
		}
		public string AsString(){
			if(this.ItemType== CBORObjectType_TextString){
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
			foreach(CBORObject i in list){
				if(i!=null && i.IsBreak)
					throw new ArgumentException();
				Write(i,s);
			}
		}
		
		private static void WriteObjectMap(
			IDictionary<CBORObject,CBORObject> map, Stream s){
			WritePositiveInt(5,map.Count,s);
			foreach(CBORObject key in map.Keys){
				if(key!=null && key.IsBreak)
					throw new ArgumentException();
				CBORObject value=map[key];
				if(value!=null && value.IsBreak)
					throw new ArgumentException();
				Write(key,s);
				Write(value,s);
			}
		}

		private static byte[] GetPositiveIntBytes(int type, int value){
			if(value<0)
				throw new ArgumentException();
			if(value<24){
				return new byte[]{(byte)((byte)value|(byte)(type<<5))};
			} else if(value<=0xFF){
				return new byte[]{(byte)(24|(type<<5)),
					(byte)(value&0xFF)};
			} else if(value<=0xFFFF){
				return new byte[]{(byte)(25|(type<<5)),
					(byte)((value>>8)&0xFF),
					(byte)(value&0xFF)};
			} else {
				return new byte[]{(byte)(26|(type<<5)),
					(byte)((value>>24)&0xFF),
					(byte)((value>>16)&0xFF),
					(byte)((value>>8)&0xFF),
					(byte)(value&0xFF)};
			}
		}

		private static void WritePositiveInt(int type, int value, Stream s){
			byte[] bytes=GetPositiveIntBytes(type,value);
			s.Write(bytes,0,bytes.Length);
		}
		
		private static byte[] GetPositiveInt64Bytes(int type, long value){
			if(value<0)
				throw new ArgumentException();
			if(value<24){
				return new byte[]{(byte)((byte)value|(byte)(type<<5))};
			} else if(value<=0xFF){
				return new byte[]{(byte)(24|(type<<5)),
					(byte)(value&0xFF)};
			} else if(value<=0xFFFF){
				return new byte[]{(byte)(25|(type<<5)),
					(byte)((value>>8)&0xFF),
					(byte)(value&0xFF)};
			} else if(value<=0xFFFFFFFF){
				return new byte[]{(byte)(26|(type<<5)),
					(byte)((value>>24)&0xFF),
					(byte)((value>>16)&0xFF),
					(byte)((value>>8)&0xFF),
					(byte)(value&0xFF)};
			} else {
				return new byte[]{(byte)(27|(type<<5)),
					(byte)((value>>56)&0xFF),
					(byte)((value>>48)&0xFF),
					(byte)((value>>40)&0xFF),
					(byte)((value>>32)&0xFF),
					(byte)((value>>24)&0xFF),
					(byte)((value>>16)&0xFF),
					(byte)((value>>8)&0xFF),
					(byte)(value&0xFF)};
			}
		}
		private static void WritePositiveInt64(int type, long value, Stream s){
			byte[] bytes=GetPositiveInt64Bytes(type,value);
			s.Write(bytes,0,bytes.Length);
		}
		
		private const int StreamedStringBufferLength=4096;
		
		private static void WriteStreamedString(String str, Stream stream){
			byte[] bytes=new byte[StreamedStringBufferLength];
			int byteIndex=0;
			bool streaming=false;
			for(int index=0;index<str.Length;index++){
				int c=str[index];
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
				} else {
					if(c>=0xD800 && c<=0xDBFF && index+1<str.Length &&
					   str[index+1]>=0xDC00 && str[index+1]<=0xDFFF){
						// Get the Unicode code point for the surrogate pair
						c=0x10000+(c-0xD800)*0x400+(str[index+1]-0xDC00);
						index++;
					} else if(c>=0xD800 && c<=0xDFFF){
						// unpaired surrogate, write U+FFFD instead
						c=0xFFFD;
					}
					if(c<=0xFFFF){
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
		
		
		private static BigInteger LowestMajorType1=
			BigInteger.Parse("-18446744073709551616",NumberStyles.AllowLeadingSign,
			                 CultureInfo.InvariantCulture);
		private static BigInteger UInt64MaxValue=
			BigInteger.Parse("18446744073709551615",NumberStyles.AllowLeadingSign,
			                 CultureInfo.InvariantCulture);
		private static BigInteger FiftySixBitMask=(BigInteger)0xFFFFFFFFFFFFFFL;
		
		/// <summary>
		/// Writes a big integer in CBOR format to a data stream.
		/// </summary>
		/// <param name="bi">Big integer to write.</param>
		/// <param name="s">Stream to write to.</param>
		public static void Write(BigInteger bigint, Stream s){
			if((s)==null)throw new ArgumentNullException("s");
			int datatype=0;
			if(bigint.Sign<0){
				datatype=1;
				bigint+=(BigInteger)BigInteger.One;
				bigint=-(BigInteger)bigint;
			}
			if(bigint.CompareTo(Int64MaxValue)<=0){
				// If the big integer is representable in
				// major type 0 or 1, write that major type
				// instead of as a bignum
				long ui=(long)(BigInteger)bigint;
				WritePositiveInt64(datatype,ui,s);
			} else {
				using(MemoryStream ms=new MemoryStream()){
					long tmp=0;
					byte[] buffer=new byte[10];
					while(bigint.Sign>0){
						// To reduce the number of big integer
						// operations, extract the big int 56 bits at a time
						// (not 64, to avoid negative numbers)
						BigInteger tmpbigint=bigint&(BigInteger)FiftySixBitMask;
						tmp=(long)(BigInteger)tmpbigint;
						bigint>>=56;
						bool isNowZero=(bigint.IsZero);
						int bufferindex=0;
						for(int i=0;i<7 && (!isNowZero || tmp>0);i++){
							buffer[bufferindex]=(byte)(tmp&0xFF);
							tmp>>=8;
							bufferindex++;
						}
						ms.Write(buffer,0,bufferindex);
					}
					byte[] bytes=ms.ToArray();
					switch(bytes.Length){
						case 1: // Fits in 1 byte (won't normally happen though)
							buffer[0]=(byte)((datatype<<5)|24);
							buffer[1]=bytes[0];
							s.Write(buffer,0,2);
							break;
						case 2: // Fits in 2 bytes (won't normally happen though)
							buffer[0]=(byte)((datatype<<5)|25);
							buffer[1]=bytes[1];
							buffer[2]=bytes[0];
							s.Write(buffer,0,3);
							break;
						case 3:
						case 4:
							buffer[0]=(byte)((datatype<<5)|26);
							buffer[1]=(bytes.Length>3) ? bytes[3] : (byte)0;
							buffer[2]=bytes[2];
							buffer[3]=bytes[1];
							buffer[4]=bytes[0];
							s.Write(buffer,0,5);
							break;
						case 5:
						case 6:
						case 7:
						case 8:
							buffer[0]=(byte)((datatype<<5)|27);
							buffer[1]=(bytes.Length>7) ? bytes[7] : (byte)0;
							buffer[2]=(bytes.Length>6) ? bytes[6] : (byte)0;
							buffer[3]=(bytes.Length>5) ? bytes[5] : (byte)0;
							buffer[4]=bytes[4];
							buffer[5]=bytes[3];
							buffer[6]=bytes[2];
							buffer[7]=bytes[1];
							buffer[8]=bytes[0];
							s.Write(buffer,0,9);
							break;
						default:
							s.WriteByte((datatype==0) ?
							            (byte)0xC2 :
							            (byte)0xC3);
							WritePositiveInt(2,bytes.Length,s);
							for(int i=bytes.Length-1;i>=0;i--){
								s.WriteByte(bytes[i]);
							}
							break;
					}
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
					BigInteger mask=(BigInteger)0xFF;
					BigInteger tempBigInt=tag;
					byte[] intval=new byte[8];
					s.WriteByte((byte)(0xC0|27));
					for(int i=0;i<8;i++){
						BigInteger bi2=tempBigInt&(BigInteger)mask;
						intval[7-i]=(byte)(int)bi2;
						tempBigInt>>=8;
					}
					s.Write(intval,0,intval.Length);
				}
			}
			if(this.ItemType==CBORObjectType_Integer){
				Write((long)item,s);
			} else if(this.ItemType== CBORObjectType_BigInteger){
				Write((BigInteger)item,s);
			} else if(this.ItemType== CBORObjectType_ByteString){
				WritePositiveInt((this.ItemType== CBORObjectType_ByteString) ? 2 : 3,
				                 ((byte[])item).Length,s);
				s.Write(((byte[])item),0,((byte[])item).Length);
			} else if(this.ItemType== CBORObjectType_TextString ){
				Write((string)item,s);
			} else if(this.ItemType== CBORObjectType_Array){
				WriteObjectArray(AsList(),s);
			} else if(this.ItemType== CBORObjectType_Map){
				WriteObjectMap(AsMap(),s);
			} else if(this.ItemType== CBORObjectType_SimpleValue){
				int value=(int)item;
				if(value<24 || value==31){
					s.WriteByte((byte)(0xE0+value));
				} else {
					s.WriteByte(0xF8);
					s.WriteByte((byte)value);
				}
			} else if(this.ItemType== CBORObjectType_Single){
				Write((float)item,s);
			} else if(this.ItemType== CBORObjectType_Double){
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
			byte[] data=new byte[]{(byte)0xFA,
				(byte)((bits>>24)&0xFF),
				(byte)((bits>>16)&0xFF),
				(byte)((bits>>8)&0xFF),
				(byte)(bits&0xFF)};
			s.Write(data,0,5);
		}
		public static void Write(double value, Stream s){
			long bits=ConverterInternal.DoubleToInt64Bits(
				(double)value);
			byte[] data=new byte[]{(byte)0xFB,
				(byte)((bits>>56)&0xFF),
				(byte)((bits>>48)&0xFF),
				(byte)((bits>>40)&0xFF),
				(byte)((bits>>32)&0xFF),
				(byte)((bits>>24)&0xFF),
				(byte)((bits>>16)&0xFF),
				(byte)((bits>>8)&0xFF),
				(byte)(bits&0xFF)};
			s.Write(data,0,9);
		}
		
		/// <summary>
		/// Gets the binary representation of this
		/// data item.
		/// </summary>
		/// <returns>A byte array in CBOR format.</returns>
		public byte[] ToBytes(){
			// For some types, MemoryStream is a lot of
			// overhead since the amount of memory they
			// use is fixed and small
			if(this.ItemType== CBORObjectType_Integer && !this.IsTagged){
				long value=(long)item;
				if(value>=0){
					return GetPositiveInt64Bytes(0,value);
				} else {
					value+=1;
					value=-value; // Will never overflow
					return GetPositiveInt64Bytes(1,value);
				}
			} else if(this.ItemType==CBORObjectType_Single && !this.IsTagged){
				int bits=ConverterInternal.SingleToInt32Bits(
					(float)item);
				return new byte[]{(byte)0xFA,
					(byte)((bits>>24)&0xFF),
					(byte)((bits>>16)&0xFF),
					(byte)((bits>>8)&0xFF),
					(byte)(bits&0xFF)};
			} else if(this.ItemType==CBORObjectType_Double && !this.IsTagged){
				long bits=ConverterInternal.DoubleToInt64Bits(
					(double)item);
				return new byte[]{(byte)0xFB,
					(byte)((bits>>56)&0xFF),
					(byte)((bits>>48)&0xFF),
					(byte)((bits>>40)&0xFF),
					(byte)((bits>>32)&0xFF),
					(byte)((bits>>24)&0xFF),
					(byte)((bits>>16)&0xFF),
					(byte)((bits>>8)&0xFF),
					(byte)(bits&0xFF)};
			} else {
				using(MemoryStream ms=new MemoryStream()){
					WriteTo(ms);
					if(ms.Position>Int32.MaxValue)
						throw new OutOfMemoryException();
					return ms.ToArray();
				}
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
				foreach(CBORObject i in dic.Keys){
					if(i!=null && i.IsBreak)
						throw new ArgumentException();
					CBORObject value=dic[i];
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
		/// Notation (JSON) format.  This function only accepts
		/// maps and arrays.
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
			string alphabet="0123456789ABCDEF";
			int length = data.Length;
			for (int i = 0; i < length;i++) {
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
		
		private BigInteger ParseBigIntegerWithExponent(string str){
			return BigInteger.Parse(str,
			                        NumberStyles.AllowLeadingSign|
			                        NumberStyles.AllowDecimalPoint|
			                        NumberStyles.AllowExponent,
			                        CultureInfo.InvariantCulture);
		}
		
		private string ExponentAndMantissaToString(CBORObject ex, CBORObject ma){
			BigInteger exponent=ex.AsBigInteger();
			string mantissa=ma.IntegerToString();
			StringBuilder sb=new StringBuilder();
			BigInteger decimalPoint=(BigInteger)(mantissa.Length);
			decimalPoint+=(BigInteger)exponent;
			if(exponent.Sign<0 &&
			   decimalPoint.Sign>0){
				int pos=(int)decimalPoint;
				sb.Append(mantissa.Substring(0,pos));
				sb.Append(".");
				sb.Append(mantissa.Substring(pos));
			} else if(exponent.Sign<0 &&
			          decimalPoint.Sign==0){
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
		}
		
		/// <summary>
		/// Converts this object to a JSON string.  This function
		/// works not only with arrays and maps (the only proper
		/// JSON objects under RFC 4627), but also integers,
		/// strings, byte arrays, and other JSON data types.
		/// </summary>
		public string ToJSONString(){
			if(this.ItemType== CBORObjectType_SimpleValue){
				if(this.IsTrue)return "true";
				else if(this.IsFalse)return "false";
				else if(this.IsNull)return "null";
				else return "null";
			} else if(this.ItemType== CBORObjectType_Single){
				float f=(float)item;
				if(Single.IsNegativeInfinity(f) ||
				   Single.IsPositiveInfinity(f) ||
				   Single.IsNaN(f)) return "null";
				else
					return Convert.ToString((float)f,
					                        CultureInfo.InvariantCulture);
			} else if(this.ItemType== CBORObjectType_Double){
				double f=(double)item;
				if(Double.IsNegativeInfinity(f) ||
				   Double.IsPositiveInfinity(f) ||
				   Double.IsNaN(f)) return "null";
				else
					return Convert.ToString((double)f,
					                        CultureInfo.InvariantCulture);
			} else if(this.ItemType== CBORObjectType_Integer){
				return Convert.ToString((long)item,CultureInfo.InvariantCulture);
			} else if(this.ItemType== CBORObjectType_BigInteger){
				return ((BigInteger)item).ToString(CultureInfo.InvariantCulture);
			} else if(this.ItemType== CBORObjectType_ByteString){
				StringBuilder sb=new StringBuilder();
				sb.Append('\"');
				if(this.Tag==(BigInteger)22){
					ToBase64(sb,(byte[])item,Base64,false);
				} else if(this.Tag==(BigInteger)23){
					ToBase16(sb,(byte[])item);
				} else {
					ToBase64(sb,(byte[])item,Base64URL,false);
				}
				sb.Append('\"');
				return sb.ToString();
			} else if(this.ItemType== CBORObjectType_TextString){
				return StringToJSONString((string)item);
			} else if(this.ItemType== CBORObjectType_Array){
				if(this.Tag==(BigInteger)4 && this.Count==2){
					return ExponentAndMantissaToString(this[0],this[1]);
				} else {
					StringBuilder sb=new StringBuilder();
					bool first=true;
					sb.Append("[");
					foreach(CBORObject i in AsList()){
						if(!first)sb.Append(",");
						sb.Append(i.ToJSONString());
						first=false;
					}
					sb.Append("]");
					return sb.ToString();
				}
			} else if(this.ItemType== CBORObjectType_Map){
				var dict=new Dictionary<string,CBORObject>();
				StringBuilder sb=new StringBuilder();
				bool first=true;
				IDictionary<CBORObject,CBORObject> dictitem=AsMap();
				foreach(CBORObject key in dictitem.Keys){
					string str=(key.ItemType== CBORObjectType_TextString) ?
						key.AsString() : key.ToJSONString();
					dict[str]=dictitem[key];
				}
				sb.Append("{");
				foreach(string key in dict.Keys){
					if(!first)sb.Append(",");
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
				BigInteger bigintValue=BigInteger.Parse(
					str.Substring(numberStart,numberEnd-numberStart),
					NumberStyles.None,
					CultureInfo.InvariantCulture);
				if(negative)bigintValue=-(BigInteger)bigintValue;
				return FromObject(bigintValue);
			} else {
				BigInteger fracpart=(fracStart<0) ? BigInteger.Zero : BigInteger.Parse(
					str.Substring(fracStart,fracEnd-fracStart),
					NumberStyles.None,
					CultureInfo.InvariantCulture);
				// Intval consists of the whole and fractional part
				string intvalString=str.Substring(numberStart,numberEnd-numberStart)+
					(fracpart.IsZero ? String.Empty : str.Substring(fracStart,fracEnd-fracStart));
				BigInteger intval=BigInteger.Parse(
					intvalString,
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
					exp-=(BigInteger)(fracEnd-fracStart);
				}
				if(exp.IsZero){
					// If exponent is 0, this is also easy,
					// just return the integer
					return FromObject(intval);
				}
				if(exp.CompareTo(UInt64MaxValue)>0 ||
				   exp.CompareTo(LowestMajorType1)<0){
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
			string str;
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
			str = JSONTokener.trimSpaces(sb.ToString());
			if (str.Equals("true"))
				return CBORObject.True;
			if (str.Equals("false"))
				return CBORObject.False;
			if (str.Equals("null"))
				return CBORObject.Null;
			if ((b >= '0' && b <= '9') || b == '.' || b == '-' || b == '+') {
				CBORObject obj=ParseJSONNumber(str);
				if(obj==null)
					throw tokener.syntaxError("JSON number can't be parsed.");
				return obj;
			}
			if (str.Length == 0)
				throw tokener.syntaxError("Missing value.");
			return FromObject(str);
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
				throw x.syntaxError("A JSONObject must begin with '{' or '['");
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
						if(obj.ItemType!= CBORObjectType_TextString)
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
				if(b<0){
					if(bytesNeeded!=0){
						bytesNeeded=0;
						throw new FormatException("Invalid UTF-8");
					} else {
						if(byteLength>0 && pointer>=byteLength)
							throw new FormatException("Premature end of stream");
						break; // end of stream
					}
				}
				if(byteLength>0) {
					pointer++;
				}
				if(bytesNeeded==0){
					if(b<0x80){
						builder.Append((char)b);
					} else if(b>=0xc2 && b<=0xdf){
						bytesNeeded=1;
						cp=(b-0xc0)<<6;
					} else if(b>=0xe0 && b<=0xef){
						lower=(b==0xe0) ? 0xa0 : 0x80;
						upper=(b==0xed) ? 0x9f : 0xbf;
						bytesNeeded=2;
						cp=(b-0xe0)<<12;
					} else if(b>=0xf0 && b<=0xf4){
						lower=(b==0xf0) ? 0x90 : 0x80;
						upper=(b==0xf4) ? 0x8f : 0xbf;
						bytesNeeded=3;
						cp=(b-0xf0)<<18;
					} else
						throw new FormatException("Invalid UTF-8");
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
			return new CBORObject(CBORObjectType_Integer,value);
		}
		public static CBORObject FromObject(CBORObject value){
			if(value==null)return CBORObject.Null;
			return value;
		}

		public static CBORObject FromObject(BigInteger bigintValue){
			if(bigintValue.CompareTo(Int64MinValue)>=0 &&
			   bigintValue.CompareTo(Int64MaxValue)<=0){
				return new CBORObject(CBORObjectType_Integer,(long)(BigInteger)bigintValue);
			} else {
				return new CBORObject(CBORObjectType_BigInteger,bigintValue);
			}
		}
		public static CBORObject FromObject(string stringValue){
			if(stringValue==null)return CBORObject.Null;
			if(!IsValidString(stringValue))
				throw new ArgumentException("String contains an unpaired surrogate code point.");
			return new CBORObject(CBORObjectType_TextString,stringValue);
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
			return new CBORObject(CBORObjectType_Single,value);
		}
		public static CBORObject FromObject(double value){
			return new CBORObject(CBORObjectType_Double,value);
		}
		public static CBORObject FromObject(byte[] value){
			if(value==null)return CBORObject.Null;
			return new CBORObject(CBORObjectType_ByteString,value);
		}
		public static CBORObject FromObject(CBORObject[] array){
			if(array==null)return CBORObject.Null;
			IList<CBORObject> list=new List<CBORObject>();
			foreach(CBORObject i in array){
				CBORObject obj=FromObject(i);
				if(obj!=null && obj.IsBreak)
					throw new ArgumentException();
				list.Add(obj);
			}
			return new CBORObject(CBORObjectType_Array,list);
		}
		public static CBORObject FromObject<T>(IList<T> value){
			if(value==null)return CBORObject.Null;
			IList<CBORObject> list=new List<CBORObject>();
			foreach(T i in (IList<T>)value){
				CBORObject obj=FromObject(i);
				if(obj!=null && obj.IsBreak)
					throw new ArgumentException();
				list.Add(obj);
			}
			return new CBORObject(CBORObjectType_Array,list);
		}
		public static CBORObject FromObject<TKey, TValue>(IDictionary<TKey, TValue> dic){
			if(dic==null)return CBORObject.Null;
			var map=new Dictionary<CBORObject,CBORObject>();
			foreach(TKey i in dic.Keys){
				CBORObject key=FromObject(i);
				if(key!=null && key.IsBreak)
					throw new ArgumentException();
				CBORObject value=FromObject(dic[i]);
				if(value!=null && value.IsBreak)
					throw new ArgumentException();
				map[key]=value;
			}
			return new CBORObject(CBORObjectType_Map,map);
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
			if(o is byte)return FromObject((byte)o);
			if(o is float)return FromObject((float)o);
			
			if(o is sbyte)return FromObject((sbyte)o);
			if(o is ulong)return FromObject((ulong)o);
			if(o is uint)return FromObject((uint)o);
			if(o is ushort)return FromObject((ushort)o);
			if(o is DateTime)return FromObject((DateTime)o);
			
			if(o is double)return FromObject((double)o);
			if(o is IList<CBORObject>)return FromObject((IList<CBORObject>)o);
			if(o is byte[])return FromObject((byte[])o);
			if(o is CBORObject[])return FromObject((CBORObject[])o);
			if(o is IDictionary<CBORObject,CBORObject>)return FromObject(
				(IDictionary<CBORObject,CBORObject>)o);
			if(o is IDictionary<string,CBORObject>)return FromObject(
				(IDictionary<string,CBORObject>)o);
			throw new ArgumentException();
		}
		
		private static BigInteger BigInt65536=(BigInteger)65536;

		public static CBORObject FromObjectAndTag(Object o, BigInteger bigintTag){
			if((bigintTag).Sign<0)throw new ArgumentOutOfRangeException("tag"+" not greater or equal to 0 ("+Convert.ToString(bigintTag,System.Globalization.CultureInfo.InvariantCulture)+")");
			if((bigintTag).CompareTo(UInt64MaxValue)>0)throw new ArgumentOutOfRangeException("tag"+" not less or equal to 18446744073709551615 ("+Convert.ToString(bigintTag,System.Globalization.CultureInfo.InvariantCulture)+")");
			CBORObject c=FromObject(o);
			if(bigintTag.CompareTo(BigInt65536)<0){
				// Low-numbered, commonly used tags
				return new CBORObject(c.ItemType,(int)bigintTag,0,c.item);
			} else {
				BigInteger tmpbigint=bigintTag&(BigInteger)0xFFFF;
				int low=unchecked((int)tmpbigint);
				tmpbigint=(bigintTag>>16)&(BigInteger)0xFFFF;
				low=unchecked((int)tmpbigint)<<16;
				tmpbigint=(bigintTag>>32)&(BigInteger)0xFFFF;
				int high=unchecked((int)tmpbigint);
				tmpbigint=(bigintTag>>48)&(BigInteger)0xFFFF;
				high=unchecked((int)tmpbigint)<<16;
				return new CBORObject(c.ItemType,low,high,c.item);
			}
		}

		public static CBORObject FromObjectAndTag(Object o, int intTag){
			if(intTag<0)throw new ArgumentOutOfRangeException(
				"tag not greater or equal to 0 ("+
				Convert.ToString((int)intTag,System.Globalization.CultureInfo.InvariantCulture)+")");
			CBORObject c=FromObject(o);
			return new CBORObject(c.ItemType,intTag,0,c.item);
		}
		
		//-----------------------------------------------------------
		
		private string IntegerToString(){
			if(this.ItemType== CBORObjectType_Integer){
				long v=(long)item;
				return Convert.ToString((long)v,CultureInfo.InvariantCulture);
			} else if(this.ItemType== CBORObjectType_BigInteger){
				return ((BigInteger)item).ToString(CultureInfo.InvariantCulture);
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
			StringBuilder sb=null;
			string simvalue=null;
			if(tagged){
				if(sb==null)sb=new StringBuilder();
				sb.Append(this.Tag.ToString(CultureInfo.InvariantCulture));
				sb.Append('(');
			}
			if(this.ItemType== CBORObjectType_SimpleValue){
				if(this.IsBreak){
					simvalue="break";
				}
				else if(this.IsTrue){
					simvalue="true";
				}
				else if(this.IsFalse){
					simvalue="false";
				}
				else if(this.IsNull){
					simvalue="null";
				}
				else if(this.IsUndefined){
					simvalue="undefined";
				}
				else {
					if(sb==null)sb=new StringBuilder();
					sb.Append("simple(");
					sb.Append(Convert.ToString((int)item,CultureInfo.InvariantCulture));
					sb.Append(")");
				}
				if(simvalue!=null){
					if(sb==null)return simvalue;
					sb.Append(simvalue);
				}
			} else if(this.ItemType== CBORObjectType_Single){
				float f=(float)item;
				if(Single.IsNegativeInfinity(f))
					simvalue=("-Infinity");
				else if(Single.IsPositiveInfinity(f))
					simvalue=("Infinity");
				else if(Single.IsNaN(f))
					simvalue=("NaN");
				else
					simvalue=(Convert.ToString((float)f,CultureInfo.InvariantCulture));
				if(sb==null)return simvalue;
				sb.Append(simvalue);
			} else if(this.ItemType== CBORObjectType_Double){
				double f=(double)item;
				if(Double.IsNegativeInfinity(f))
					simvalue=("-Infinity");
				else if(Double.IsPositiveInfinity(f))
					simvalue=("Infinity");
				else if(Double.IsNaN(f))
					simvalue=("NaN");
				else
					simvalue=(Convert.ToString((double)f,CultureInfo.InvariantCulture));
				if(sb==null)return simvalue;
				sb.Append(simvalue);
			} else if(this.ItemType== CBORObjectType_Integer){
				long v=(long)item;
				simvalue=(Convert.ToString((long)v,CultureInfo.InvariantCulture));
				if(sb==null)return simvalue;
				sb.Append(simvalue);
			} else if(this.ItemType== CBORObjectType_BigInteger){
				simvalue=(((BigInteger)item).ToString(CultureInfo.InvariantCulture));
				if(sb==null)return simvalue;
				sb.Append(simvalue);
			} else if(this.ItemType== CBORObjectType_ByteString){
				if(sb==null)sb=new StringBuilder();
				sb.Append("h'");
				byte[] data=(byte[])item;
				ToBase16(sb,data);
				sb.Append("'");
			} else if(this.ItemType== CBORObjectType_TextString){
				simvalue="\""+((string)item)+"\"";
				if(sb==null)return simvalue;
				sb.Append(simvalue);
			} else if(this.ItemType== CBORObjectType_Array){
				if(sb==null)sb=new StringBuilder();
				bool first=true;
				sb.Append("[");
				foreach(CBORObject i in AsList()){
					if(!first)sb.Append(", ");
					sb.Append(i.ToString());
					first=false;
				}
				sb.Append("]");
			} else if(this.ItemType== CBORObjectType_Map){
				if(sb==null)sb=new StringBuilder();
				bool first=true;
				sb.Append("{");
				IDictionary<CBORObject,CBORObject> map=AsMap();
				foreach(CBORObject key in map.Keys){
					if(!first)sb.Append(", ");
					sb.Append(key.ToString());
					sb.Append(": ");
					sb.Append(map[key].ToString());
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
					return new CBORObject(CBORObjectType_SimpleValue,additional);
				}
				if(additional==24){
					c=s.ReadByte();
					if(c<0)
						throw new IOException();
					return new CBORObject(CBORObjectType_SimpleValue,c);
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
						return new CBORObject(CBORObjectType_Single,f);
					} else if(additional==26){
						float f=ConverterInternal.Int32BitsToSingle(
							unchecked((int)x));
						return new CBORObject(CBORObjectType_Single,f);
					} else if(additional==27){
						double f=ConverterInternal.Int64BitsToDouble(
							unchecked(x));
						return new CBORObject(CBORObjectType_Double,f);
					}
				}
				throw new FormatException();
			}
			long uadditional=0;
			BigInteger bigintAdditional=BigInteger.Zero;
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
					// Get low 7 bytes and convert to BigInteger,
					// to reduce the number of big integer
					// operations
					x&=0xFFFFFFFFFFFFFFL;
					bigintAdditional=(BigInteger)x;
					// Include the first and highest byte
					int firstByte=((int)cs[0])&0xFF;
					BigInteger bigintTemp=(BigInteger)firstByte;
					bigintTemp<<=56;
					bigintAdditional|=(BigInteger)bigintTemp;
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
					bigintAdditional=BigInteger.MinusOne-(BigInteger)bigintAdditional;
					return FromObject(bigintAdditional);
				} else if(uadditional<=Int64.MaxValue){
					return FromObject(((long)-1-(long)uadditional));
				} else {
					BigInteger bi=BigInteger.MinusOne;
					bi-=(BigInteger)(uadditional);
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
							CBORObjectType_ByteString,
							data);
					}
				} else {
					if(hasBigAdditional || uadditional>Int32.MaxValue){
						throw new IOException();
					}
					byte[] data=new byte[(int)uadditional];
					if(s.Read(data,0,data.Length)!=data.Length)
						throw new IOException();
					return new CBORObject(
						CBORObjectType_ByteString,
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
						CBORObjectType_TextString,
						builder.ToString());
				} else {
					if(hasBigAdditional || uadditional>=Int32.MaxValue){
						throw new IOException();
					}
					string str=ReadUtf8(s,(int)uadditional);
					return new CBORObject(CBORObjectType_TextString,str);
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
					return new CBORObject(CBORObjectType_Array,list);
				} else {
					if(hasBigAdditional || uadditional>=Int32.MaxValue){
						throw new IOException();
					}
					for(long i=0;i<uadditional;i++){
						list.Add(Read(s,depth+1,false,-1,validTypeFlags,vtindex));
						vtindex++;
					}
					return new CBORObject(CBORObjectType_Array,list);
				}
			} else if(type==5){ // Map, type 5
				var dict=new Dictionary<CBORObject,CBORObject>();
				if(additional==31){
					while(true){
						CBORObject key=Read(s,depth+1,true,-1,null,0);
						if(key.IsBreak)
							break;
						CBORObject value=Read(s,depth+1,false,-1,null,0);
						dict[key]=value;
					}
					return new CBORObject(CBORObjectType_Map,dict);
				} else {
					if(hasBigAdditional || uadditional>=Int32.MaxValue){
						throw new IOException();
					}
					for(long i=0;i<uadditional;i++){
						CBORObject key=Read(s,depth+1,false,-1,null,0);
						CBORObject value=Read(s,depth+1,false,-1,null,0);
						dict[key]=value;
					}
					return new CBORObject(CBORObjectType_Map,dict);
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
						BigInteger bi=BigInteger.Zero;
						for(int i=0;i<data.Length;i++){
							bi<<=8;
							int x=((int)data[i])&0xFF;
							bi|=(BigInteger)x;
						}
						if(uadditional==3){
							bi=BigInteger.MinusOne-(BigInteger)bi; // Convert to a negative
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
						IList<CBORObject> list=o.AsList();
						if(list[1].ItemType!=CBORObjectType_Integer &&
						   list[1].ItemType!=CBORObjectType_BigInteger)
							throw new FormatException();
					} else {
						o=Read(s,depth+1,allowBreak,-1,null,0);
					}
				} else {
					o=Read(s,depth+1,allowBreak,-1,null,0);
				}
				if(hasBigAdditional){
					return FromObjectAndTag(o,bigintAdditional);
				} else if(uadditional<65536){
					return FromObjectAndTag(
						o,(int)uadditional);
				} else {
					return FromObjectAndTag(
						o,(BigInteger)uadditional);
				}
			}
		}
	}
}