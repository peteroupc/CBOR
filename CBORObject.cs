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
	/// <para>
	/// Thread Safety:
	/// CBOR objects that are numbers, "simple values", and text
	/// strings are immutable (their
	/// values can't be changed), so they are inherently safe
	/// for use by multiple threads.
	/// CBOR objects that are arrays, maps, and byte strings
	/// are mutable, but this class doesn't attempt to synchronize
	/// reads and writes to those objects by multiple threads, so
	/// those objects are not thread safe without such
	/// synchronization.
	/// </para>
	/// </summary>
	public sealed partial class CBORObject
	{
		private int ItemType {
			get {
				CBORObject curobject=this;
				while(curobject.itemtype_==CBORObjectType_Tagged) {
					curobject=((CBORObject)curobject.item_);
				}
				return curobject.itemtype_;
			}
		}

		private Object ThisItem {
			get {
				CBORObject curobject=this;
				while(curobject.itemtype_==CBORObjectType_Tagged) {
					curobject=((CBORObject)curobject.item_);
				}
				return curobject.item_;
			}
		}


		int itemtype_;
		Object item_;
		int tagLow;
		int tagHigh;
		
		private const int CBORObjectType_Integer=0; // -(2^63) .. (2^63-1)
		private const int CBORObjectType_BigInteger=1; // all other integers
		private const int CBORObjectType_ByteString=2;
		private const int CBORObjectType_TextString=3;
		private const int CBORObjectType_Array=4;
		private const int CBORObjectType_Map=5;
		private const int CBORObjectType_SimpleValue=6;
		private const int CBORObjectType_Single=7;
		private const int CBORObjectType_Double=8;
		private const int CBORObjectType_Tagged=9;
		private static readonly BigInteger Int64MaxValue=(BigInteger)Int64.MaxValue;
		private static readonly BigInteger Int64MinValue=(BigInteger)Int64.MinValue;
		/// <summary>
		/// Represents the value false.
		/// </summary>
		public static readonly CBORObject False=new CBORObject(CBORObjectType_SimpleValue,20);
		/// <summary>
		/// Represents the value true.
		/// </summary>
		public static readonly CBORObject True=new CBORObject(CBORObjectType_SimpleValue,21);
		/// <summary>
		/// Represents the value null.
		/// </summary>
		public static readonly CBORObject Null=new CBORObject(CBORObjectType_SimpleValue,22);
		/// <summary>
		/// Represents the value undefined.
		/// </summary>
		public static readonly CBORObject Undefined=new CBORObject(CBORObjectType_SimpleValue,23);
		
		private CBORObject(){}
		
		private CBORObject(CBORObject obj, int tagLow, int tagHigh) :
			this(CBORObjectType_Tagged,obj) {
			this.tagLow=tagLow;
			this.tagHigh=tagHigh;
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
			this.item_=item;
		}
		
		/// <summary>
		/// Gets the general data type of this CBOR object.
		/// </summary>
		public CBORType Type {
			get {
				switch(this.ItemType){
					case CBORObjectType_Integer:
					case CBORObjectType_BigInteger:
					case CBORObjectType_Single:
					case CBORObjectType_Double:
						return CBORType.Number;
					case CBORObjectType_SimpleValue:
						if((int)this.ThisItem==21 || (int)this.ThisItem==20){
							return CBORType.Boolean;
						}
						return CBORType.SimpleValue;
					case CBORObjectType_Array:
						if(this.Count==2 && this.HasTag(4)){
							return CBORType.Number;
						}
						return CBORType.Array;
					case CBORObjectType_Map:
						return CBORType.Map;
					case CBORObjectType_ByteString:
						return CBORType.ByteString;
					case CBORObjectType_TextString:
						return CBORType.TextString;
					default:
						throw new InvalidOperationException("Unexpected data type");
				}
			}
		}
		
		/// <summary>
		/// Gets whether this value is a CBOR true value.
		/// </summary>
		public bool IsTrue {
			get {
				return this.ItemType==CBORObjectType_SimpleValue && (int)this.ThisItem==21;
			}
		}
		/// <summary>
		/// Gets whether this value is a CBOR false value.
		/// </summary>
		public bool IsFalse {
			get {
				return this.ItemType==CBORObjectType_SimpleValue && (int)this.ThisItem==20;
			}
		}
		/// <summary>
		/// Gets whether this value is a CBOR null value.
		/// </summary>
		public bool IsNull {
			get {
				return this.ItemType==CBORObjectType_SimpleValue && (int)this.ThisItem==22;
			}
		}
		/// <summary>
		/// Gets whether this value is a CBOR undefined value.
		/// </summary>
		public bool IsUndefined {
			get {
				return this.ItemType==CBORObjectType_SimpleValue && (int)this.ThisItem==23;
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
			if(a==null)return 0;
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
			if(list==null)return 0;
			int ret=19;
			int count=list.Count;
			unchecked {
				ret=ret*31+count;
				for(int i=0;i<count;i++){
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
			// same hash code), which is much too difficult to do.
			return unchecked(a.Count.GetHashCode()*19);
		}

		/// <summary>
		/// Compares the equality of two CBOR objects.
		/// </summary>
		/// <param name="obj">The object to compare</param>
		/// <returns>true if the objects are equal; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			CBORObject other = obj as CBORObject;
			if (other == null)
				return false;
			if(item_ is byte[]){
				if(!ByteArrayEquals((byte[])this.ThisItem,other.item_ as byte[]))
					return false;
			} else if(item_ is IList<CBORObject>){
				if(!CBORArrayEquals(AsList(),other.item_ as IList<CBORObject>))
					return false;
			} else if(item_ is IDictionary<CBORObject,CBORObject>){
				if(!CBORMapEquals(AsMap(),
				                  other.item_ as IDictionary<CBORObject,CBORObject>))
					return false;
			} else {
				if(!object.Equals(this.item_, other.item_))
					return false;
			}
			return this.ItemType == other.ItemType &&
				this.tagLow == other.tagLow &&
				this.tagHigh == other.tagHigh;
		}
		
		/// <summary>
		/// Calculates the hash code of this object.
		/// </summary>
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (item_ != null){
					int itemHashCode=0;
					if(item_ is byte[])
						itemHashCode=ByteArrayHashCode((byte[])this.ThisItem);
					else if(item_ is IList<CBORObject>)
						itemHashCode=CBORArrayHashCode(AsList());
					else if(item_ is IDictionary<CBORObject,CBORObject>)
						itemHashCode=CBORMapHashCode(AsMap());
					else
						itemHashCode=item_.GetHashCode();
					hashCode += 1000000007 * itemHashCode;
				}
				hashCode += 1000000009 * this.ItemType.GetHashCode();
				hashCode += 1000000009 * this.tagLow;
				hashCode += 1000000009 * this.tagHigh;
			}
			return hashCode;
		}
		#endregion
		
		private static void CheckCBORLength(long expectedLength, long actualLength){
			if(actualLength<expectedLength)
				throw new CBORException("Premature end of data");
			else if(actualLength>expectedLength)
				throw new CBORException("Too many bytes");
		}

		private static void CheckCBORLength(int expectedLength, int actualLength){
			if(actualLength<expectedLength)
				throw new CBORException("Premature end of data");
			else if(actualLength>expectedLength)
				throw new CBORException("Too many bytes");
		}
		
		private static string GetOptimizedStringIfShortAscii(
			byte[] data, int offset
		){
			int length=data.Length;
			if(length>offset){
				int nextbyte=((int)(data[offset]&(int)0xFF));
				if(nextbyte>=0x60 && nextbyte<0x78){
					int offsetp1=1+offset;
					// Check for type 3 string of short length
					int rightLength=offsetp1+(nextbyte-0x60);
					CheckCBORLength(rightLength,length);
					// Check for all ASCII text
					for(int i=offsetp1;i<length;i++){
						if((data[i]&((byte)0x80))!=0){
							return null;
						}
					}
					// All ASCII text, so convert to a string
					// from a char array without having to
					// convert from UTF-8 first
					char[] c=new char[length-offsetp1];
					for(int i=offsetp1;i<length;i++){
						c[i-offsetp1]=((char)(data[i]&(int)0xFF));
					}
					return new String(c);
				}
			}
			return null;
		}
		
		private static CBORObject[] FixedObjects=InitializeFixedObjects();
		
		// Initialize fixed values for certain
		// head bytes
		private static CBORObject[] InitializeFixedObjects(){
			FixedObjects=new CBORObject[256];
			for(int i=0;i<0x18;i++){
				FixedObjects[i]=new CBORObject(CBORObjectType_Integer,(long)i);
			}
			for(int i=0x20;i<0x38;i++){
				FixedObjects[i]=new CBORObject(CBORObjectType_Integer,
				                               (long)(-1-(i-0x20)));
			}
			FixedObjects[0x60]=new CBORObject(CBORObjectType_TextString,String.Empty);
			for(int i=0xE0;i<0xf8;i++){
				FixedObjects[i]=new CBORObject(CBORObjectType_SimpleValue,(int)(i-0xe0));
			}
			return FixedObjects;
		}
		
		// Expected lengths for each head byte.
		// 0 means length varies. -1 means invalid.
		private static int[] ExpectedLengths=new int[]{
			1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1, // major type 0
			1,1,1,1,1,1,1,1, 2,3,5,9,-1,-1,-1,-1,
			1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1, // major type 1
			1,1,1,1,1,1,1,1, 2,3,5,9,-1,-1,-1,-1,
			1,2,3,4,5,6,7,8, 9,10,11,12,13,14,15,16, // major type 2
			17,18,19,20,21,22,23,24,  0,0,0,0,-1,-1,-1,0,
			1,2,3,4,5,6,7,8, 9,10,11,12,13,14,15,16, // major type 3
			17,18,19,20,21,22,23,24,  0,0,0,0,-1,-1,-1,0,
			1,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0, // major type 4
			0,0,0,0,0,0,0,0, 0,0,0,0,-1,-1,-1,0,
			1,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0, // major type 5
			0,0,0,0,0,0,0,0, 0,0,0,0,-1,-1,-1,0,
			0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0, // major type 6
			0,0,0,0,0,0,0,0, 0,0,0,0,-1,-1,-1,-1,
			1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1, // major type 7
			1,1,1,1,1,1,1,1, 2,3,5,9,-1,-1,-1,-1
		};
		
		// Generate a CBOR object for head bytes with fixed length.
		// Note that this function assumes that the length of the data
		// was already checked.
		private static CBORObject GetFixedLengthObject(int firstbyte, byte[] data){
			CBORObject fixedObj=FixedObjects[firstbyte];
			if(fixedObj!=null)
				return fixedObj;
			int majortype=(firstbyte>>5);
			if(firstbyte>=0x61 && firstbyte<0x78){
				// text string length 1 to 23
				String s=GetOptimizedStringIfShortAscii(data,0);
				if(s!=null)return new CBORObject(CBORObjectType_TextString,s);
			}
			if(((firstbyte&0x1C)==0x18)){
				// contains 1 to 8 extra bytes of additional information
				long uadditional=0;
				switch(firstbyte&0x1F){
					case 24:
						uadditional=((int)(data[1]&(int)0xFF));
						break;
					case 25:
						uadditional=(((long)(data[1]&(long)0xFF))<<8);
						uadditional|=(((long)(data[2]&(long)0xFF)));
						break;
					case 26:
						uadditional=(((long)(data[1]&(long)0xFF))<<24);
						uadditional|=(((long)(data[2]&(long)0xFF))<<16);
						uadditional|=(((long)(data[3]&(long)0xFF))<<8);
						uadditional|=(((long)(data[4]&(long)0xFF)));
						break;
					case 27:
						uadditional=(((long)(data[1]&(long)0xFF))<<56);
						uadditional|=(((long)(data[2]&(long)0xFF))<<48);
						uadditional|=(((long)(data[3]&(long)0xFF))<<40);
						uadditional|=(((long)(data[4]&(long)0xFF))<<32);
						uadditional|=(((long)(data[5]&(long)0xFF))<<24);
						uadditional|=(((long)(data[6]&(long)0xFF))<<16);
						uadditional|=(((long)(data[7]&(long)0xFF))<<8);
						uadditional|=(((long)(data[8]&(long)0xFF)));
						break;
					default:
						throw new CBORException("Unexpected data encountered");
				}
				switch(majortype){
					case 0:
						if((uadditional>>63)==0){
							// use only if additional's top bit isn't set
							// (additional is a signed long)
							return new CBORObject(CBORObjectType_Integer,uadditional);
						} else {
							long lowAdditional=uadditional&0xFFFFFFFFFFFFFFL;
							long highAdditional=(uadditional>>56)&0xFF;
							BigInteger bigintAdditional=(BigInteger)highAdditional;
							bigintAdditional<<=56;
							bigintAdditional|=(BigInteger)lowAdditional;
							return FromObject(bigintAdditional);
						}
					case 1:
						if((uadditional>>63)==0){
							// use only if additional's top bit isn't set
							// (additional is a signed long)
							return new CBORObject(CBORObjectType_Integer,-1-uadditional);
						} else {
							long lowAdditional=uadditional&0xFFFFFFFFFFFFFFL;
							long highAdditional=(uadditional>>56)&0xFF;
							BigInteger bigintAdditional=(BigInteger)highAdditional;
							bigintAdditional<<=56;
							bigintAdditional|=(BigInteger)lowAdditional;
							bigintAdditional=(BigInteger.MinusOne)-(BigInteger)bigintAdditional;
							return FromObject(bigintAdditional);
						}
					case 7:
						if(firstbyte==0xf9)
							return new CBORObject(
								CBORObjectType_Single,HalfPrecisionToSingle((int)uadditional));
						else if(firstbyte==0xfa)
							return new CBORObject(
								CBORObjectType_Single,
								ConverterInternal.Int32BitsToSingle((int)uadditional));
						else if(firstbyte==0xfb)
							return new CBORObject(
								CBORObjectType_Double,
								ConverterInternal.Int64BitsToDouble(uadditional));
						else if(firstbyte==0xf8)
							return new CBORObject(
								CBORObjectType_SimpleValue,(int)uadditional);
						else
							throw new CBORException("Unexpected data encountered");
					default:
						throw new CBORException("Unexpected data encountered");
				}
			}
			else if(majortype==2){ // short byte string
				byte[] ret=new byte[firstbyte-0x40];
				Array.Copy(data,1,ret,0,firstbyte-0x40);
				return new CBORObject(CBORObjectType_ByteString,ret);
			}
			else if(majortype==3){ // short text string
				StringBuilder ret=new StringBuilder(firstbyte-0x60);
				CBORDataUtilities.ReadUtf8FromBytes(data,1,firstbyte-0x60,ret,false);
				return new CBORObject(CBORObjectType_TextString,ret.ToString());
			}
			else if(firstbyte==0x80) // empty array
				return FromObject(new List<CBORObject>());
			else if(firstbyte==0xA0) // empty map
				return FromObject(new Dictionary<CBORObject,CBORObject>());
			else
				throw new CBORException("Unexpected data encountered");
		}
		
		/// <summary>
		/// Generates a CBOR object from an array of CBOR-encoded
		/// bytes.
		/// </summary>
		/// <param name="data"></param>
		/// <returns>A CBOR object corresponding to the data.</returns>
		/// <exception cref="System.ArgumentException">data is null or empty.</exception>
		/// <exception cref="CBORException">There was an
		/// error in reading or parsing the data.</exception>
		public static CBORObject FromBytes(byte[] data){
			if((data)==null)throw new ArgumentNullException("data");
			if((data).Length==0)throw new ArgumentException("data is empty.");
			int firstbyte=((int)(data[0]&(int)0xFF));
			int expectedLength=ExpectedLengths[firstbyte];
			if(expectedLength==-1) // if invalid
				throw new CBORException("Unexpected data encountered");
			else if(expectedLength!=0) // if fixed length
				CheckCBORLength(expectedLength,data.Length);
			if(firstbyte==0xc0){
				// value with tag 0
				String s=GetOptimizedStringIfShortAscii(data,1);
				if(s!=null)return new CBORObject(FromObject(s),0,0);
			}
			if(expectedLength!=0){
				return GetFixedLengthObject(firstbyte, data);
			}
			// For objects with variable length,
			// read the object as though
			// the byte array were a stream
			using(MemoryStream ms=new MemoryStream(data)){
				CBORObject o=Read(ms);
				CheckCBORLength((long)data.Length,(long)ms.Position);
				return o;
			}
		}
		
		/// <summary>
		/// Gets the number of keys in this map, or the number
		/// of items in this array, or 0 if this item is neither
		/// an array nor a map.
		/// </summary>
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
		/// Gets whether this data item has at least one tag.
		/// </summary>
		public bool IsTagged {
			get {
				return this.itemtype_==CBORObjectType_Tagged;
			}
		}
		
		/// <summary>
		/// Gets the byte array used in this object, if this object
		/// is a byte string.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// This object is not a byte string.</exception>
		public byte[] GetByteString(){
			if(this.itemtype_==CBORObjectType_ByteString)
				return ((byte[])this.ThisItem);
			else
				throw new InvalidOperationException("Not a byte string");
		}
		


		private bool HasTag(int tagValue){
			if(!this.IsTagged)return false;
			if(tagHigh==0 && tagValue==tagLow)return true;
			return ((CBORObject)item_).HasTag(tagValue);
		}
		
		private BigInteger LowHighToBigInteger(int tagLow, int tagHigh){
			
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
		}
		
		private static BigInteger[] EmptyTags=new BigInteger[0];
		
		/// <summary>
		/// Gets a list of all tags, from outermost to innermost.
		/// </summary>
		public BigInteger[] GetTags(){
			if(!this.IsTagged)return EmptyTags;
			CBORObject curitem=this;
			if(curitem.IsTagged){
				var list=new List<BigInteger>();
				while(curitem.IsTagged){
					list.Add(LowHighToBigInteger(
						curitem.tagLow,curitem.tagHigh));
					curitem=((CBORObject)curitem.item_);
				}
				return (BigInteger[])list.ToArray();
			} else {
				return new BigInteger[]{LowHighToBigInteger(tagLow,tagHigh)};
			}
		}
		
		/// <summary>
		/// Gets the last defined tag for this CBOR data item,
		/// or 0 if the item is untagged.
		/// </summary>
		public BigInteger InnermostTag {
			get {
				if(!this.IsTagged)return BigInteger.Zero;
				CBORObject previtem=this;
				CBORObject curitem=((CBORObject)item_);
				while(curitem.IsTagged){
					previtem=curitem;
					curitem=((CBORObject)curitem.item_);
				}
				if(previtem.tagHigh==0 &&
				   previtem.tagLow>=0 &&
				   previtem.tagLow<0x10000){
					return (BigInteger)previtem.tagLow;
				}
				return LowHighToBigInteger(
					previtem.tagLow,
					previtem.tagHigh);
			}
		}

		private IDictionary<CBORObject,CBORObject> AsMap(){
			return (IDictionary<CBORObject,CBORObject>)this.ThisItem;
		}
		
		private IList<CBORObject> AsList(){
			return (IList<CBORObject>)this.ThisItem;
		}
		
		/// <summary>
		/// Gets the value of a CBOR object by integer index in this
		/// array.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">This
		/// object is not an array.</exception>.
		public CBORObject this[int index]{
			get {
				if(this.ItemType== CBORObjectType_Array){
					IList<CBORObject> list=AsList();
					return list[index];
				} else {
					throw new InvalidOperationException("Not an array");
				}
			}
			/// <summary>
			/// Sets the value of a CBOR object by integer index in this
			/// array.
			/// </summary>
			/// <exception cref="System.InvalidOperationException">This
			/// object is not an array.</exception>.
			/// <exception cref="System.ArgumentNullException">value
			/// is null (as opposed to CBORObject.Null).</exception>.
			set {
				if(this.ItemType== CBORObjectType_Array){
					if((value)==null)throw new ArgumentNullException("value");
					if(this.HasTag(4))
						throw new InvalidOperationException("Read-only array");
					IList<CBORObject> list=AsList();
					list[index]=value;
				} else {
					throw new InvalidOperationException("Not an array");
				}
			}
		}

		/// <summary>
		/// Gets a collection of the keys of this CBOR object.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">This
		/// object is not a map.</exception>.
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
		/// Gets the value of a CBOR object in this
		/// map, using a CBOR object as the key.
		/// </summary>
		/// <exception cref="System.ArgumentNullException">The key is null
		/// (as opposed to CBORObject.Null).</exception>.
		/// <exception cref="System.InvalidOperationException">This
		/// object is not a map.</exception>.
		public CBORObject this[CBORObject key]{
			get {
				if((key)==null)throw new ArgumentNullException("key");
				if(this.ItemType== CBORObjectType_Map){
					IDictionary<CBORObject,CBORObject> map=AsMap();
					if(!map.ContainsKey(key))
						return null;
					return map[key];
				} else {
					throw new InvalidOperationException("Not a map");
				}
			}
			/// <summary>
			/// Sets the value of a CBOR object in this
			/// map, using a CBOR object as the key.
			/// </summary>
			/// <exception cref="System.ArgumentNullException">The key or value is null
			/// (as opposed to CBORObject.Null).</exception>.
			/// <exception cref="System.InvalidOperationException">This
			/// object is not a map.</exception>.
			set {
				if((key)==null)throw new ArgumentNullException("key");
				if((value)==null)throw new ArgumentNullException("value");
				if(this.ItemType== CBORObjectType_Map){
					IDictionary<CBORObject,CBORObject> map=AsMap();
					map[key]=value;
				} else {
					throw new InvalidOperationException("Not a map");
				}
			}
		}
		
		/// <summary>
		/// Gets the value of a CBOR object in this
		/// map, using a string as the key.
		/// </summary>
		/// <exception cref="System.ArgumentNullException">The key is null.
		/// </exception>.
		/// <exception cref="System.InvalidOperationException">This
		/// object is not a map.</exception>.
		public CBORObject this[string key]{
			get {
				if((key)==null)throw new ArgumentNullException("key");
				CBORObject objkey=CBORObject.FromObject(key);
				return this[objkey];
			}
			/// <summary>
			/// Sets the value of a CBOR object in this
			/// map, using a string as the key.
			/// </summary>
			/// <exception cref="System.ArgumentNullException">The key or value
			/// is null (as opposed to CBORObject.Null).
			/// </exception>.
			/// <exception cref="System.InvalidOperationException">This
			/// object is not a map.</exception>.
			set {
				if((key)==null)throw new ArgumentNullException("key");
				if((value)==null)throw new ArgumentNullException("value");
				CBORObject objkey=CBORObject.FromObject(key);
				if(this.ItemType== CBORObjectType_Map){
					IDictionary<CBORObject,CBORObject> map=AsMap();
					map[objkey]=value;
				} else {
					throw new InvalidOperationException("Not a map");
				}
			}
		}

		/// <summary>
		/// Returns the simple value ID of this object, or -1
		/// if this object is not a simple value (including if
		/// the value is a floating-point number).
		/// </summary>
		public int SimpleValue {
			get {
				if(this.ItemType== CBORObjectType_SimpleValue){
					return (int)this.ThisItem;
				} else {
					return -1;
				}
			}
		}
		
		/// <summary>
		/// Adds a new object to the end of this array.
		/// </summary>
		/// <param name="obj">A CBOR object.</param>
		/// <exception cref="System.ArgumentNullException">key or value is null
		/// (as opposed to CBORObject.Null).</exception>
		/// <exception cref="System.ArgumentException">key already exists in this map.</exception>
		/// <exception cref="InvalidOperationException">This object is not a map.</exception>
		public void Add(CBORObject key, CBORObject value){
			if((key)==null)throw new ArgumentNullException("key");
			if((value)==null)throw new ArgumentNullException("value");
			if(this.ItemType== CBORObjectType_Map){
				IDictionary<CBORObject,CBORObject> map=AsMap();
				if(map.ContainsKey(key))
					throw new ArgumentException("Key already exists.");
				map.Add(key,value);
			} else {
				throw new InvalidOperationException("Not a map");
			}
		}

		/// <summary>
		/// Determines whether a value of the given key exists in
		/// this object.
		/// </summary>
		/// <param name="key">An object that serves as the key.</param>
		/// <returns>True if the given key is found, or false if the
		/// given key is not found or this object is not a map.</returns>
		/// <exception cref="System.ArgumentNullException">key is null
		/// (as opposed to CBORObject.Null).</exception>
		public bool ContainsKey(CBORObject key){
			if((key)==null)throw new ArgumentNullException("key");
			if(this.ItemType== CBORObjectType_Map){
				IDictionary<CBORObject,CBORObject> map=AsMap();
				return map.ContainsKey(key);
			} else {
				return false;
			}
		}

		/// <summary>
		/// Adds a new object to the end of this array.
		/// </summary>
		/// <param name="obj">A CBOR object.</param>
		/// <exception cref="System.InvalidOperationException">
		/// This object is not an array.</exception>
		/// <exception cref="System.ArgumentNullException">obj
		/// is null (as opposed to CBORObject.Null).</exception>.
		public void Add(CBORObject obj){
			if((obj)==null)throw new ArgumentNullException("obj");
			if(this.ItemType== CBORObjectType_Array){
				if(this.HasTag(4))
					throw new InvalidOperationException("Read-only array");
				IList<CBORObject> list=AsList();
				list.Add(obj);
			} else {
				throw new InvalidOperationException("Not an array");
			}
		}
		
		/// <summary>
		/// If this object is an array, removes the first instance
		/// of the specified item from the array.  If this object
		/// is a map, removes the item with the given key from the map.
		/// </summary>
		/// <param name="obj">The item or key to remove.</param>
		/// <returns>True if the item was removed; otherwise, false.</returns>
		/// <exception cref="System.ArgumentNullException">obj
		/// is null (as opposed to CBORObject.Null).</exception>.
		/// <exception cref="System.InvalidOperationException">
		/// The object is not an array or map.</exception>
		public bool Remove(CBORObject obj){
			if((obj)==null)throw new ArgumentNullException("obj");
			if(this.ItemType== CBORObjectType_Map){
				IDictionary<CBORObject,CBORObject> dict=AsMap();
				bool hasKey=dict.ContainsKey(obj);
				if(hasKey){
					dict.Remove(obj);
					return true;
				}
				return false;
			} else if(this.ItemType== CBORObjectType_Array){
				if(this.HasTag(4))
					throw new InvalidOperationException("Read-only array");
				IList<CBORObject> list=AsList();
				return list.Remove(obj);
			} else {
				throw new InvalidOperationException("Not a map or array");
			}
		}

		/// <summary>
		/// Converts this object to a 64-bit floating point
		/// number.
		/// </summary>
		/// <returns>The closest 64-bit floating point number
		/// to this object.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// This object's type is not a number type.
		/// </exception>
		public double AsDouble(){
			if(this.ItemType== CBORObjectType_Integer)
				return (double)(long)this.ThisItem;
			else if(this.ItemType== CBORObjectType_BigInteger)
				return (double)(BigInteger)this.ThisItem;
			else if(this.ItemType== CBORObjectType_Single)
				return (double)(float)this.ThisItem;
			else if(this.ItemType== CBORObjectType_Double)
				return (double)this.ThisItem;
			else if(this.HasTag(4) && this.ItemType== CBORObjectType_Array &&
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
		/// This object's type is not a number type.
		/// </exception>
		public float AsSingle(){
			if(this.ItemType== CBORObjectType_Integer)
				return (float)(long)this.ThisItem;
			else if(this.ItemType== CBORObjectType_BigInteger)
				return (float)(BigInteger)this.ThisItem;
			else if(this.ItemType== CBORObjectType_Single)
				return (float)this.ThisItem;
			else if(this.ItemType== CBORObjectType_Double)
				return (float)(double)this.ThisItem;
			else if(this.HasTag(4) &&
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
		/// This object's type is not a number type.
		/// </exception>
		public BigInteger AsBigInteger(){
			if(this.ItemType== CBORObjectType_Integer)
				return (BigInteger)(long)this.ThisItem;
			else if(this.ItemType== CBORObjectType_BigInteger)
				return (BigInteger)this.ThisItem;
			else if(this.ItemType== CBORObjectType_Single)
				return (BigInteger)(float)this.ThisItem;
			else if(this.ItemType== CBORObjectType_Double)
				return (BigInteger)(double)this.ThisItem;
			else if(this.HasTag(4) && this.ItemType== CBORObjectType_Array &&
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
		
		/// <summary>
		/// Returns false if this object is
		/// False, Null, or Undefined;
		/// otherwise, true.</summary>
		public bool AsBoolean(){
			if(this.IsFalse || this.IsNull || this.IsUndefined)
				return false;
			return true;
		}
		
		/// <summary>
		/// Converts this object to a 16-bit signed
		/// integer.  Floating point values are truncated
		/// to an integer.
		/// </summary>
		/// <returns>The closest 16-bit signed
		/// integer to this object.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// This object's type is not a number type.
		/// </exception>
		/// <exception cref="System.OverflowException">
		/// This object's value exceeds the range of a 16-bit
		/// signed integer.</exception>
		public short AsInt16(){
			int v=AsInt32();
			if(v>Int16.MaxValue || v<0)
				throw new OverflowException();
			return (short)v;
		}
		
		/// <summary>
		/// Converts this object to a byte.
		/// Floating point values are truncated
		/// to an integer.
		/// </summary>
		/// <returns>The closest byte-sized
		/// integer to this object.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// This object's type is not a number type.
		/// </exception>
		/// <exception cref="System.OverflowException">
		/// This object's value exceeds the range of a byte (is
		/// less than 0 or would be greater than 255 when truncated
		/// to an integer).</exception>
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
		/// <returns>The closest  64-bit signed
		/// integer to this object.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// This object's type is not a number type.
		/// </exception>
		/// <exception cref="System.OverflowException">
		/// This object's value exceeds the range of a 64-bit
		/// signed integer.</exception>
		public long AsInt64(){
			if(this.ItemType== CBORObjectType_Integer){
				return (long)this.ThisItem;
			} else if(this.ItemType== CBORObjectType_BigInteger){
				if(((BigInteger)this.ThisItem).CompareTo(Int64MaxValue)>0 ||
				   ((BigInteger)this.ThisItem).CompareTo(Int64MinValue)<0)
					throw new OverflowException();
				return (long)(BigInteger)this.ThisItem;
			} else if(this.ItemType== CBORObjectType_Single){
				if(Single.IsNaN((float)this.ThisItem) ||
				   (float)this.ThisItem>Int64.MaxValue || (float)this.ThisItem<Int64.MinValue)
					throw new OverflowException();
				return (long)(float)this.ThisItem;
			} else if(this.ItemType== CBORObjectType_Double){
				if(Double.IsNaN((double)this.ThisItem) ||
				   (double)this.ThisItem>Int64.MaxValue || (double)this.ThisItem<Int64.MinValue)
					throw new OverflowException();
				return (long)(double)this.ThisItem;
			} else if(this.HasTag(4) && this.ItemType== CBORObjectType_Array &&
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
		/// integer.  Floating point values are truncated
		/// to an integer.
		/// </summary>
		/// <returns>The closest big integer
		/// to this object.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// This object's type is not a number type.
		/// </exception>
		/// <exception cref="System.OverflowException">
		/// This object's value exceeds the range of a 32-bit
		/// signed integer.</exception>
		public int AsInt32(){
			Object thisItem=this.ThisItem;
			if(this.ItemType== CBORObjectType_Integer){
				if((long)thisItem>Int32.MaxValue || (long)thisItem<Int32.MinValue)
					throw new OverflowException();
				return (int)(long)thisItem;
			} else if(this.ItemType== CBORObjectType_BigInteger){
				if(((BigInteger)thisItem).CompareTo((BigInteger)Int32.MaxValue)>0 ||
				   ((BigInteger)thisItem).CompareTo((BigInteger)Int32.MinValue)<0)
					throw new OverflowException();
				return (int)(BigInteger)thisItem;
			} else if(this.ItemType== CBORObjectType_Single){
				if(Single.IsNaN((float)thisItem) ||
				   (float)thisItem>Int32.MaxValue || (float)thisItem<Int32.MinValue)
					throw new OverflowException();
				return (int)(float)thisItem;
			} else if(this.ItemType== CBORObjectType_Double){
				if(Double.IsNaN((double)thisItem) ||
				   (double)thisItem>Int32.MaxValue || (double)thisItem<Int32.MinValue)
					throw new OverflowException();
				return (int)(double)thisItem;
			} else if(this.HasTag(4) && this.ItemType== CBORObjectType_Array &&
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
		/// <summary>
		/// Gets the value of this object as a string object.
		/// </summary>
		/// <returns>Gets this object's string.</returns>
		/// <exception cref="InvalidOperationException">
		/// This object's type is not a string.</exception>
		public string AsString(){
			if(this.ItemType== CBORObjectType_TextString){
				return (string)this.ThisItem;
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
		/// "stream" is null.</exception>
		/// <exception cref="CBORException">There was an
		/// error in reading or parsing the data.</exception>
		public static CBORObject Read(Stream stream){
			try {
				return Read(stream,0,false,-1,null,0);
			} catch(IOException ex){
				throw new CBORException("I/O error occurred.",ex);
			}
		}
		
		private static void WriteObjectArray(
			IList<CBORObject> list, Stream s){
			WritePositiveInt(4,list.Count,s);
			foreach(CBORObject i in list){
				Write(i,s);
			}
		}
		
		private static void WriteObjectMap(
			IDictionary<CBORObject,CBORObject> map, Stream s){
			WritePositiveInt(5,map.Count,s);
			foreach(CBORObject key in map.Keys){
				CBORObject value=map[key];
				Write(key,s);
				Write(value,s);
			}
		}

		private static byte[] GetPositiveIntBytes(int type, int value){
			if((value)<0)throw new ArgumentOutOfRangeException("value"+" not greater or equal to "+"0"+" ("+Convert.ToString((int)value,System.Globalization.CultureInfo.InvariantCulture)+")");
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
			if((value)<0)throw new ArgumentOutOfRangeException("value"+" not greater or equal to "+"0"+" ("+Convert.ToString((long)value,System.Globalization.CultureInfo.InvariantCulture)+")");
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
			byte[] bytes;
			bytes=GetOptimizedBytesIfShortAscii(str,-1);
			if(bytes!=null){
				stream.Write(bytes,0,bytes.Length);
				return;
			}
			bytes=new byte[StreamedStringBufferLength];
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
		/// <param name="str">The string to write.  Can be null.</param>
		/// <param name="s">A writable data stream.</param>
		/// <exception cref="System.ArgumentNullException">stream is null.</exception>
		/// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
		public static void Write(string str, Stream stream){
			if((stream)==null)throw new ArgumentNullException("stream");
			if(str==null){
				stream.WriteByte(0xf6); // Write null instead of string
			} else {
				WriteStreamedString(str,stream);
			}
		}
		
		
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
		
		/// <summary>
		/// Writes this CBOR object to a data stream.
		/// </summary>
		/// <param name="s">A writable data stream.</param>
		/// <exception cref="System.ArgumentNullException">s is null.</exception>
		/// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
		public void WriteTo(Stream s){
			if((s)==null)throw new ArgumentNullException("s");
			WriteTags(s);
			if(this.ItemType==CBORObjectType_Integer){
				Write((long)this.ThisItem,s);
			} else if(this.ItemType== CBORObjectType_BigInteger){
				Write((BigInteger)this.ThisItem,s);
			} else if(this.ItemType== CBORObjectType_ByteString){
				byte[] arr=(byte[])this.ThisItem;
				WritePositiveInt((this.ItemType== CBORObjectType_ByteString) ? 2 : 3,
				                 arr.Length,s);
				s.Write(arr,0,arr.Length);
			} else if(this.ItemType== CBORObjectType_TextString ){
				Write(this.AsString(),s);
			} else if(this.ItemType== CBORObjectType_Array){
				WriteObjectArray(AsList(),s);
			} else if(this.ItemType== CBORObjectType_Map){
				WriteObjectMap(AsMap(),s);
			} else if(this.ItemType== CBORObjectType_SimpleValue){
				int value=(int)this.ThisItem;
				if(value<24){
					s.WriteByte((byte)(0xE0+value));
				} else {
					s.WriteByte(0xF8);
					s.WriteByte((byte)value);
				}
			} else if(this.ItemType== CBORObjectType_Single){
				Write((float)this.ThisItem,s);
			} else if(this.ItemType== CBORObjectType_Double){
				Write((double)this.ThisItem,s);
			} else {
				throw new ArgumentException("Unexpected data type");
			}
		}
		
		/// <summary>
		/// Writes a 64-bit unsigned integer in CBOR format
		/// to a data stream.
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
		/// <exception cref="System.ArgumentNullException">s is null.</exception>
		/// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
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
		/// <summary>
		/// Writes a 32-bit signed integer in CBOR format to
		/// a data stream.
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
		/// <exception cref="System.ArgumentNullException">s is null.</exception>
		/// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
		public static void Write(int value, Stream s){
			Write((long)value,s);
		}
		/// <summary>
		/// Writes a 16-bit signed integer in CBOR format to
		/// a data stream.
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
		/// <exception cref="System.ArgumentNullException">s is null.</exception>
		/// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
		public static void Write(short value, Stream s){
			Write((long)value,s);
		}
		/// <summary>
		/// Writes a Unicode character as a string in CBOR format to
		/// a data stream.
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
		/// <exception cref="System.ArgumentNullException">s is null.</exception>
		/// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
		public static void Write(char value, Stream s){
			Write(new String(new char[]{value}),s);
		}
		/// <summary>
		/// Writes a Boolean value in CBOR format to
		/// a data stream.
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
		/// <exception cref="System.ArgumentNullException">s is null.</exception>
		/// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
		public static void Write(bool value, Stream s){
			if((s)==null)throw new ArgumentNullException("s");
			s.WriteByte(value ? (byte)0xf5 : (byte)0xf4);
		}
		/// <summary>
		/// Writes a byte in CBOR format to
		/// a data stream.
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
		/// <exception cref="System.ArgumentNullException">s is null.</exception>
		/// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
		public static void Write(byte value, Stream s){
			if((s)==null)throw new ArgumentNullException("s");
			if((((int)value)&0xFF)<24){
				s.WriteByte(value);
			} else {
				s.WriteByte((byte)(24));
				s.WriteByte(value);
			}
		}
		/// <summary>
		/// Writes a 32-bit floating-point number in CBOR format to
		/// a data stream.
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
		/// <exception cref="System.ArgumentNullException">s is null.</exception>
		/// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
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
		/// <summary>
		/// Writes a 64-bit floating-point number in CBOR format to
		/// a data stream.
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
		/// <exception cref="System.ArgumentNullException">s is null.</exception>
		/// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
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
		
		private static byte[] GetOptimizedBytesIfShortAscii(string str, int tagbyte){
			byte[] bytes;
			if(str.Length<=255){
				// The strings will usually be short ASCII strings, so
				// use this optimization
				int offset=0;
				int length=str.Length;
				int extra=(length<24) ? 1 : 2;
				if(tagbyte>=0)extra++;
				bytes=new byte[length+extra];
				if(tagbyte>=0){
					bytes[offset]=((byte)(tagbyte));
					offset++;
				}
				if(length<24){
					bytes[offset]=((byte)(0x60+str.Length));
					offset++;
				} else {
					bytes[offset]=((byte)(0x78));
					bytes[offset+1]=((byte)(str.Length));
					offset+=2;
				}
				bool issimple=true;
				for(int i=0;i<str.Length;i++){
					char c=str[i];
					if(c>=0x80){
						issimple=false;
						break;
					}
					bytes[i+offset]=unchecked((byte)c);
				}
				if(issimple){
					return bytes;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Gets the binary representation of this
		/// data item.
		/// </summary>
		/// <returns>A byte array in CBOR format.</returns>
		public byte[] ToBytes(){
			// For some types, a memory stream is a lot of
			// overhead since the amount of memory the types
			// use is fixed and small
			bool hasComplexTag=false;
			byte tagbyte=0;
			bool tagged=this.IsTagged;
			if(this.IsTagged){
				CBORObject taggedItem=(CBORObject)item_;
				if(taggedItem.IsTagged ||
				   this.tagHigh!=0 ||
				   ((this.tagLow)>>16)!=0 ||
				   this.tagLow>=24){
					hasComplexTag=true;
				} else {
					tagbyte=(byte)(0xC0+(int)this.tagLow);
				}
			}
			if(!hasComplexTag){
				if(this.ItemType== CBORObjectType_TextString){
					byte[] ret=GetOptimizedBytesIfShortAscii(
						this.AsString(),
						tagged ? (((int)tagbyte)&0xFF) : -1);
					if(ret!=null)return ret;
				} else if(this.ItemType== CBORObjectType_Integer){
					long value=(long)this.ThisItem;
					byte[] ret=null;
					if(value>=0){
						ret=GetPositiveInt64Bytes(0,value);
					} else {
						value+=1;
						value=-value; // Will never overflow
						ret=GetPositiveInt64Bytes(1,value);
					}
					if(!tagged)return ret;
					byte[] ret2=new byte[ret.Length+1];
					Array.Copy(ret,0,ret2,1,ret.Length);
					ret2[0]=tagbyte;
					return ret2;
				} else if(this.ItemType==CBORObjectType_Single){
					int bits=ConverterInternal.SingleToInt32Bits(
						(float)this.ThisItem);
					return tagged ?
						new byte[]{tagbyte,(byte)0xFA,
						(byte)((bits>>24)&0xFF),
						(byte)((bits>>16)&0xFF),
						(byte)((bits>>8)&0xFF),
						(byte)(bits&0xFF)} :
						new byte[]{(byte)0xFA,
						(byte)((bits>>24)&0xFF),
						(byte)((bits>>16)&0xFF),
						(byte)((bits>>8)&0xFF),
						(byte)(bits&0xFF)};
				} else if(this.ItemType==CBORObjectType_Double){
					long bits=ConverterInternal.DoubleToInt64Bits(
						(double)this.ThisItem);
					return tagged ?
						new byte[]{tagbyte,(byte)0xFB,
						(byte)((bits>>56)&0xFF),
						(byte)((bits>>48)&0xFF),
						(byte)((bits>>40)&0xFF),
						(byte)((bits>>32)&0xFF),
						(byte)((bits>>24)&0xFF),
						(byte)((bits>>16)&0xFF),
						(byte)((bits>>8)&0xFF),
						(byte)(bits&0xFF)} :
						new byte[]{(byte)0xFB,
						(byte)((bits>>56)&0xFF),
						(byte)((bits>>48)&0xFF),
						(byte)((bits>>40)&0xFF),
						(byte)((bits>>32)&0xFF),
						(byte)((bits>>24)&0xFF),
						(byte)((bits>>16)&0xFF),
						(byte)((bits>>8)&0xFF),
						(byte)(bits&0xFF)};
				}
			}
			try {
				using(MemoryStream ms=new MemoryStream()){
					WriteTo(ms);
					return ms.ToArray();
				}
			} catch(IOException ex){
				throw new CBORException("I/O Error occurred",ex);
			}
		}
		
		/// <summary>
		/// Writes an arbitrary object to a CBOR data stream.
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
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
					CBORObject value=dic[i];
					Write(i,s);
					Write(value,s);
				}
			} else if(o is IDictionary<CBORObject,CBORObject>){
				WriteObjectMap((IDictionary<CBORObject,CBORObject>)o,s);
			} else {
				FromObject(o).WriteTo(s);
			}
		}
		
		/// <summary>
		/// Generates a CBOR object from a string in JavaScript Object
		/// Notation (JSON) format.  This function only accepts
		/// maps and arrays.
		/// </summary>
		/// <param name="str">A string in JSON format.</param>
		public static CBORObject FromJSONString(string str){
			JSONTokener tokener=new JSONTokener(str,0);
			CBORObject obj=ParseJSONObject(tokener);
			if(tokener.nextClean()!=-1)
				throw tokener.syntaxError("End of string not reached");
			return obj;
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
				if(c=='\\' || c=='"'){
					sb.Append('\\');
				} else if(c<0x20){
					sb.Append("\\u00");
					sb.Append((char)('0'+(int)(c>>4)));
					sb.Append((char)('0'+(int)(c&15)));
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
			if(mantissa.Length>0 && mantissa[0]=='-'){
				sb.Append('-');
				mantissa=mantissa.Substring(1);
			}
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
				float f=(float)this.ThisItem;
				if(Single.IsNegativeInfinity(f) ||
				   Single.IsPositiveInfinity(f) ||
				   Single.IsNaN(f)) return "null";
				else
					return Convert.ToString((float)f,
					                        CultureInfo.InvariantCulture);
			} else if(this.ItemType== CBORObjectType_Double){
				double f=(double)this.ThisItem;
				if(Double.IsNegativeInfinity(f) ||
				   Double.IsPositiveInfinity(f) ||
				   Double.IsNaN(f)) return "null";
				else
					return Convert.ToString((double)f,
					                        CultureInfo.InvariantCulture);
			} else if(this.ItemType== CBORObjectType_Integer){
				return Convert.ToString((long)this.ThisItem,CultureInfo.InvariantCulture);
			} else if(this.ItemType== CBORObjectType_BigInteger){
				return ((BigInteger)this.ThisItem).ToString(CultureInfo.InvariantCulture);
			} else if(this.ItemType== CBORObjectType_ByteString){
				StringBuilder sb=new StringBuilder();
				sb.Append('\"');
				if(this.HasTag(22)){
					ToBase64(sb,(byte[])this.ThisItem,Base64,false);
				} else if(this.HasTag(23)){
					ToBase16(sb,(byte[])this.ThisItem);
				} else {
					ToBase64(sb,(byte[])this.ThisItem,Base64URL,false);
				}
				sb.Append('\"');
				return sb.ToString();
			} else if(this.ItemType== CBORObjectType_TextString){
				return StringToJSONString(this.AsString());
			} else if(this.ItemType== CBORObjectType_Array){
				if(this.HasTag(4) && this.Count==2){
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
				throw new InvalidOperationException("Unexpected data type");
			}
		}
		
		
		// Based on the json.org implementation for JSONTokener
		private static CBORObject NextJSONValue(JSONTokener tokener)  {
			int c = tokener.nextClean();
			string str;
			if (c == '"' || (c == '\'' && ((tokener.getOptions()&JSONTokener.OPTION_SINGLE_QUOTES)!=0)))
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
				CBORObject obj=CBORDataUtilities.ParseJSONNumber(str,false,false);
				if(obj==null)
					throw tokener.syntaxError("JSON number can't be parsed.");
				return obj;
			}
			if (str.Length == 0)
				throw tokener.syntaxError("Missing value.");
			// Value is unparseable
			throw tokener.syntaxError("Value can't be parsed.");
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
				return new CBORObject(CBORObjectType_Array,myArrayList);
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

		
		/// <summary>
		/// Creates a new empty CBOR array.
		/// </summary>
		/// <returns>A new CBOR array.</returns>
		public static CBORObject NewArray(){
			return FromObject(new List<CBORObject>());
		}
		
		/// <summary>
		/// Creates a new empty CBOR map.
		/// </summary>
		/// <returns>A new CBOR map.</returns>
		public static CBORObject NewMap(){
			return FromObject(new Dictionary<CBORObject,CBORObject>());
		}
		
		
		//-----------------------------------------------------------
		/// <summary>
		/// Generates a CBOR object from a 64-bit signed integer.
		/// </summary>
		public static CBORObject FromObject(long value){
			return new CBORObject(CBORObjectType_Integer,value);
		}
		/// <summary>
		/// Generates a CBOR object from a CBOR object.
		/// </summary>
		/// <param name="">A CBOR object.</param>
		/// <returns>Same as "value", or CBORObject.Null if "value"
		/// is null.</returns>
		public static CBORObject FromObject(CBORObject value){
			if(value==null)return CBORObject.Null;
			return value;
		}

		/// <summary>
		/// Generates a CBOR object from an arbitrary-precision integer.
		/// </summary>
		/// <param name="bigintValue">An arbitrary-precision value.</param>
		/// <returns>A CBOR number object.</returns>
		public static CBORObject FromObject(BigInteger bigintValue){
			if(bigintValue.CompareTo(Int64MinValue)>=0 &&
			   bigintValue.CompareTo(Int64MaxValue)<=0){
				return new CBORObject(CBORObjectType_Integer,(long)(BigInteger)bigintValue);
			} else {
				return new CBORObject(CBORObjectType_BigInteger,bigintValue);
			}
		}
		/// <summary>
		/// Generates a CBOR object from a string.
		/// </summary>
		/// <param name="stringValue">A string value.  Can be null.</param>
		/// <returns>A CBOR object representing the string, or CBORObject.Null
		/// if stringValue is null.</returns>
		/// <exception cref="System.ArgumentException">The string contains an unpaired
		/// surrogate code point.</exception>
		public static CBORObject FromObject(string stringValue){
			if(stringValue==null)return CBORObject.Null;
			if(!IsValidString(stringValue))
				throw new ArgumentException("String contains an unpaired surrogate code point.");
			return new CBORObject(CBORObjectType_TextString,stringValue);
		}
		/// <summary>
		/// Generates a CBOR object from a 32-bit signed integer.
		/// </summary>
		public static CBORObject FromObject(int value){
			return FromObject((long)value);
		}
		/// <summary>
		/// Generates a CBOR object from a 16-bit signed integer.
		/// </summary>
		public static CBORObject FromObject(short value){
			return FromObject((long)value);
		}
		/// <summary>
		/// Generates a CBOR string object from a Unicode character.
		/// </summary>
		public static CBORObject FromObject(char value){
			return FromObject(new String(new char[]{value}));
		}
		/// <summary>
		/// Returns the CBOR true value or false value, depending
		/// on "value".
		/// </summary>
		public static CBORObject FromObject(bool value){
			return (value ? CBORObject.True : CBORObject.False);
		}
		/// <summary>
		/// Generates a CBOR object from a byte.
		/// </summary>
		public static CBORObject FromObject(byte value){
			return FromObject(((int)value)&0xFF);
		}
		/// <summary>
		/// Generates a CBOR object from a 32-bit floating-point
		/// number.
		/// </summary>
		public static CBORObject FromObject(float value){
			return new CBORObject(CBORObjectType_Single,value);
		}
		/// <summary>
		/// Generates a CBOR object from a 64-bit floating-point
		/// number.
		/// </summary>
		public static CBORObject FromObject(double value){
			return new CBORObject(CBORObjectType_Double,value);
		}
		/// <summary>
		/// Generates a CBOR object from a byte array.  The byte
		/// array is not copied.
		/// </summary>
		/// <param name="stringValue">A byte array.  Can be null.</param>
		/// <returns>A CBOR object that uses the byte array, or CBORObject.Null
		/// if stringValue is null.</returns>
		/// <exception cref="System.ArgumentException">The string contains an unpaired
		/// surrogate code point.</exception>
		public static CBORObject FromObject(byte[] value){
			if(value==null)return CBORObject.Null;
			return new CBORObject(CBORObjectType_ByteString,value);
		}
		/// <summary>
		/// Generates a CBOR object from an array of CBOR objects.
		/// </summary>
		/// <param name="array">An array of CBOR objects.</param>
		/// <returns>A CBOR object where each element of the given
		/// array is copied to a new array, or CBORObject.Null if "array"
		/// is null.</returns>
		public static CBORObject FromObject(CBORObject[] array){
			if(array==null)return CBORObject.Null;
			IList<CBORObject> list=new List<CBORObject>();
			foreach(CBORObject i in array){
				list.Add(FromObject(i));
			}
			return new CBORObject(CBORObjectType_Array,list);
		}
		/// <summary>
		/// Generates a CBOR object from an list of objects.
		/// </summary>
		/// <param name="value">An array of CBOR objects.</param>
		/// <returns>A CBOR object where each element of the given
		/// array is converted to a CBOR object and
		/// copied to a new array, or CBORObject.Null if "value"
		/// is null.</returns>
		public static CBORObject FromObject<T>(IList<T> value){
			if(value==null)return CBORObject.Null;
			IList<CBORObject> list=new List<CBORObject>();
			foreach(T i in (IList<T>)value){
				CBORObject obj=FromObject(i);
				list.Add(obj);
			}
			return new CBORObject(CBORObjectType_Array,list);
		}
		/// <summary>
		/// Generates a CBOR object from a map of objects.
		/// </summary>
		/// <param name="dic">A map of CBOR objects.</param>
		/// <returns>A CBOR object where each key and value of the given
		/// map is converted to a CBOR object and
		/// copied to a new map, or CBORObject.Null if "dic"
		/// is null.</returns>
		public static CBORObject FromObject<TKey, TValue>(IDictionary<TKey, TValue> dic){
			if(dic==null)return CBORObject.Null;
			var map=new Dictionary<CBORObject,CBORObject>();
			foreach(TKey i in dic.Keys){
				CBORObject key=FromObject(i);
				CBORObject value=FromObject(dic[i]);
				map[key]=value;
			}
			return new CBORObject(CBORObjectType_Map,map);
		}
		
		/// <summary>
		/// Generates a CBORObject from an arbitrary object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>A CBOR object corresponding to the given object.
		/// Returns CBORObject.Null if the object is null.</returns>
		/// <exception cref="System.ArgumentException">The object's type
		/// is not supported.</exception>
		public static CBORObject FromObject(Object obj){
			if(obj==null)return CBORObject.Null;
			if(obj is long)return FromObject((long)obj);
			if(obj is CBORObject)return FromObject((CBORObject)obj);
			if(obj is BigInteger)return FromObject((BigInteger)obj);
			if(obj is string)return FromObject((string)obj);
			if(obj is int)return FromObject((int)obj);
			if(obj is short)return FromObject((short)obj);
			if(obj is char)return FromObject((char)obj);
			if(obj is bool)return FromObject((bool)obj);
			if(obj is byte)return FromObject((byte)obj);
			if(obj is float)return FromObject((float)obj);
			
			if(obj is sbyte)return FromObject((sbyte)obj);
			if(obj is ulong)return FromObject((ulong)obj);
			if(obj is uint)return FromObject((uint)obj);
			if(obj is ushort)return FromObject((ushort)obj);
			if(obj is decimal)return FromObject((decimal)obj);
			if(obj is DateTime)return FromObject((DateTime)obj);
			
			if(obj is double)return FromObject((double)obj);
			if(obj is IList<CBORObject>)return FromObject((IList<CBORObject>)obj);
			if(obj is byte[])return FromObject((byte[])obj);
			if(obj is CBORObject[])return FromObject((CBORObject[])obj);
			if(obj is IDictionary<CBORObject,CBORObject>)return FromObject(
				(IDictionary<CBORObject,CBORObject>)obj);
			if(obj is IDictionary<string,CBORObject>)return FromObject(
				(IDictionary<string,CBORObject>)obj);
			throw new ArgumentException("Unsupported object type.");
		}
		
		private static BigInteger BigInt65536=(BigInteger)65536;

		/// <summary>
		/// Generates a CBOR object from an arbitrary object and gives the
		/// resulting object a tag.
		/// </summary>
		/// <param name="o">An arbitrary object.</param>
		/// <param name="bigintTag">A big integer that specifies a tag number.</param>
		/// <returns>a CBOR object where the object "o" is converted
		/// to a CBOR object and given the tag "bigintTag".</returns>
		/// <exception cref="System.ArgumentException">"bigintTag" is less than 0 or greater than
		/// 2^64-1, or "o"'s type is unsupported.</exception>
		public static CBORObject FromObjectAndTag(Object o, BigInteger bigintTag){
			if((bigintTag).Sign<0)throw new ArgumentOutOfRangeException(
				"tag not greater or equal to 0 ("+Convert.ToString(bigintTag,System.Globalization.CultureInfo.InvariantCulture)+")");
			if((bigintTag).CompareTo(UInt64MaxValue)>0)throw new ArgumentOutOfRangeException(
				"tag not less or equal to 18446744073709551615 ("+Convert.ToString(bigintTag,System.Globalization.CultureInfo.InvariantCulture)+")");
			CBORObject c=FromObject(o);
			if(bigintTag.CompareTo(BigInt65536)<0){
				// Low-numbered, commonly used tags
				return new CBORObject(c,(int)bigintTag,0);
			} else {
				long tagLow=0;
				long tagHigh=0;
				BigInteger tmpbigint=bigintTag&(BigInteger)FiftySixBitMask;
				tagLow=(long)(BigInteger)tmpbigint;
				tmpbigint=(bigintTag>>56)&(BigInteger)0xFF;
				tagHigh=(long)(BigInteger)tmpbigint;
				int low=unchecked((int)(tagLow&0xFFFFFFFFL));
				tagHigh=(tagHigh<<24)|((tagLow>>32)&0xFFFFFFFFL);
				int high=unchecked((int)(tagHigh&0xFFFFFFFFL));
				return new CBORObject(c,low,high);
			}
		}

		/// <summary>
		/// Generates a CBOR object from an arbitrary object and gives the
		/// resulting object a tag.
		/// </summary>
		/// <param name="o">An arbitrary object.</param>
		/// <param name="bigintTag">A 32-bit integer that specifies a tag number.</param>
		/// <returns>a CBOR object where the object "o" is converted
		/// to a CBOR object and given the tag "bigintTag".</returns>
		/// <exception cref="System.ArgumentException">"intTag" is less than 0
		/// or "o"'s type is unsupported.</exception>
		public static CBORObject FromObjectAndTag(Object o, int intTag){
			if(intTag<0)throw new ArgumentOutOfRangeException(
				"tag not greater or equal to 0 ("+
				Convert.ToString((int)intTag,System.Globalization.CultureInfo.InvariantCulture)+")");
			CBORObject c=FromObject(o);
			return new CBORObject(c,intTag,0);
		}
		
		//-----------------------------------------------------------
		
		private string IntegerToString(){
			if(this.ItemType== CBORObjectType_Integer){
				long v=(long)this.ThisItem;
				return Convert.ToString((long)v,CultureInfo.InvariantCulture);
			} else if(this.ItemType== CBORObjectType_BigInteger){
				return ((BigInteger)this.ThisItem).ToString(CultureInfo.InvariantCulture);
			} else {
				throw new InvalidOperationException("Unsupported data type");
			}
		}
		
		private void AppendClosingTags(StringBuilder sb){
			CBORObject curobject=this;
			while(curobject.IsTagged) {
				sb.Append(')');
				curobject=((CBORObject)(curobject.item_));
			}
		}
		
		private void WriteTags(Stream s){
			CBORObject curobject=this;
			while(curobject.IsTagged) {
				int low=curobject.tagLow;
				int high=curobject.tagHigh;
				if(high==0 && (low>>16)==0){
					WritePositiveInt(6,low,s);
				} else if(high==0){
					long value=((long)low)&0xFFFFFFFFL;
					WritePositiveInt64(6,value,s);
				} else if((high>>16)==0){
					long value=((long)low)&0xFFFFFFFFL;
					long highValue=((long)high)&0xFFFFFFFFL;
					value|=(highValue<<32);
					WritePositiveInt64(6,value,s);
				} else {
					byte[] arrayToWrite=new byte[]{(byte)(0xDB),
							(byte)((high>>24)&0xFF),
							(byte)((high>>16)&0xFF),
							(byte)((high>>8)&0xFF),
							(byte)(high&0xFF),
							(byte)((low>>24)&0xFF),
							(byte)((low>>16)&0xFF),
							(byte)((low>>8)&0xFF),
							(byte)(low&0xFF)};
					s.Write(arrayToWrite,0,9);
				}
				curobject=((CBORObject)(curobject.item_));
			}
		}

		private void AppendOpeningTags(StringBuilder sb){
			CBORObject curobject=this;
			while(curobject.IsTagged) {
				int low=curobject.tagLow;
				int high=curobject.tagHigh;
				if(high==0 && (low>>16)==0){
					sb.Append(Convert.ToString((int)low,CultureInfo.InvariantCulture));
				} else {
					BigInteger bi=LowHighToBigInteger(low,high);
					sb.Append(bi.ToString(CultureInfo.InvariantCulture));
				}
				sb.Append('(');
				curobject=((CBORObject)(curobject.item_));
			}
		}
		
		private static string TrimDotZero(string str){
			if(str.Length>2 && str[str.Length-1]=='0' && str[str.Length-2]=='.'){
				return str.Substring(0,str.Length-2);
			}
			return str;
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
			if(this.IsTagged){
				if(sb==null){
					if(this.ItemType==CBORObjectType_TextString){
						// The default capacity of StringBuilder may be too small
						// for many strings, so set a suggested capacity
						// explicitly
						string str=this.AsString();
						sb=new StringBuilder(Math.Min(str.Length,4096)+16);
					} else {
						sb=new StringBuilder();
					}
				}
				AppendOpeningTags(sb);
			}
			if(this.ItemType== CBORObjectType_SimpleValue){
				if(this.IsTrue){
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
					sb.Append(Convert.ToString((int)this.ThisItem,CultureInfo.InvariantCulture));
					sb.Append(")");
				}
				if(simvalue!=null){
					if(sb==null)return simvalue;
					sb.Append(simvalue);
				}
			} else if(this.ItemType== CBORObjectType_Single){
				float f=(float)this.ThisItem;
				if(Single.IsNegativeInfinity(f))
					simvalue=("-Infinity");
				else if(Single.IsPositiveInfinity(f))
					simvalue=("Infinity");
				else if(Single.IsNaN(f))
					simvalue=("NaN");
				else
					simvalue=(TrimDotZero(
						Convert.ToString((float)f,CultureInfo.InvariantCulture)));
				if(sb==null)return simvalue;
				sb.Append(simvalue);
			} else if(this.ItemType== CBORObjectType_Double){
				double f=(double)this.ThisItem;
				if(Double.IsNegativeInfinity(f))
					simvalue=("-Infinity");
				else if(Double.IsPositiveInfinity(f))
					simvalue=("Infinity");
				else if(Double.IsNaN(f))
					simvalue=("NaN");
				else
					simvalue=(TrimDotZero(
						Convert.ToString((double)f,CultureInfo.InvariantCulture)));
				if(sb==null)return simvalue;
				sb.Append(simvalue);
			} else if(this.ItemType== CBORObjectType_Integer){
				long v=(long)this.ThisItem;
				simvalue=(Convert.ToString((long)v,CultureInfo.InvariantCulture));
				if(sb==null)return simvalue;
				sb.Append(simvalue);
			} else if(this.ItemType== CBORObjectType_BigInteger){
				simvalue=(((BigInteger)this.ThisItem).ToString(CultureInfo.InvariantCulture));
				if(sb==null)return simvalue;
				sb.Append(simvalue);
			} else if(this.ItemType== CBORObjectType_ByteString){
				if(sb==null)sb=new StringBuilder();
				sb.Append("h'");
				byte[] data=(byte[])this.ThisItem;
				ToBase16(sb,data);
				sb.Append("'");
			} else if(this.ItemType== CBORObjectType_TextString){
				if(sb==null){
					return "\""+(this.AsString())+"\"";
				} else {
					sb.Append('\"');
					sb.Append(this.AsString());
					sb.Append('\"');
				}
			} else if(this.ItemType== CBORObjectType_Array){
				if(this.HasTag(4)){
					return ToJSONString();
				}
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
			if(this.IsTagged){
				AppendClosingTags(sb);
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
				throw new CBORException("Too deeply nested");
			int firstbyte=s.ReadByte();
			if(firstbyte<0)
				throw new CBORException("Premature end of data");
			if(firstbyte==0xFF){
				if(allowBreak)return null;
				throw new CBORException("Unexpected break code encountered");
			}
			int type=(firstbyte>>5)&0x07;
			int additional=(firstbyte&0x1F);
			int expectedLength=ExpectedLengths[firstbyte];
			// Data checks
			if(expectedLength==-1) // if the head byte is invalid
				throw new CBORException("Unexpected data encountered");
			if(allowOnlyType>=0){
				if(allowOnlyType!=type){
					throw new CBORException("Expected major type "+
					                        Convert.ToString((int)allowOnlyType,CultureInfo.InvariantCulture)+
					                        ", instead got type "+
					                        Convert.ToString((int)type,CultureInfo.InvariantCulture));
				}
				if(additional>=28){
					throw new CBORException("Unexpected data encountered");
				}
			}
			if(validTypeFlags!=null){
				// Check for valid major types if asked
				if(!CheckMajorTypeIndex(type,validTypeIndex,validTypeFlags)){
					throw new CBORException("Unexpected data type encountered");
				}
			}
			// Check if this represents a fixed object
			CBORObject fixedObject=FixedObjects[firstbyte];
			if(fixedObject!=null)
				return fixedObject;
			// Read fixed-length data
			byte[] data=null;
			if(expectedLength!=0){
				data=new byte[expectedLength];
				// include the first byte because this function
				// will assume it exists for some head bytes
				data[0]=unchecked((byte)firstbyte);
				if(expectedLength>1 &&
				   s.Read(data,1,expectedLength-1)!=expectedLength-1)
					throw new CBORException("Premature end of data");
				return GetFixedLengthObject(firstbyte,data);
			}
			long uadditional=(long)additional;
			BigInteger bigintAdditional=BigInteger.Zero;
			bool hasBigAdditional=false;
			data=new byte[8];
			switch(firstbyte&0x1F){
					case 24:{
						int tmp=s.ReadByte();
						if(tmp<0)
							throw new CBORException("Premature end of data");
						uadditional=tmp;
						break;
					}
					case 25:{
						if(s.Read(data,0,2)!=2)
							throw new CBORException("Premature end of data");
						uadditional=(((long)(data[0]&(long)0xFF))<<8);
						uadditional|=(((long)(data[1]&(long)0xFF)));
						break;
					}
					case 26:{
						if(s.Read(data,0,4)!=4)
							throw new CBORException("Premature end of data");
						uadditional=(((long)(data[0]&(long)0xFF))<<24);
						uadditional|=(((long)(data[1]&(long)0xFF))<<16);
						uadditional|=(((long)(data[2]&(long)0xFF))<<8);
						uadditional|=(((long)(data[3]&(long)0xFF)));
						break;
					}
					case 27:{
						if(s.Read(data,0,8)!=8)
							throw new CBORException("Premature end of data");
						uadditional=(((long)(data[0]&(long)0xFF))<<56);
						uadditional|=(((long)(data[1]&(long)0xFF))<<48);
						uadditional|=(((long)(data[2]&(long)0xFF))<<40);
						uadditional|=(((long)(data[3]&(long)0xFF))<<32);
						uadditional|=(((long)(data[4]&(long)0xFF))<<24);
						uadditional|=(((long)(data[5]&(long)0xFF))<<16);
						uadditional|=(((long)(data[6]&(long)0xFF))<<8);
						uadditional|=(((long)(data[7]&(long)0xFF)));
						if((uadditional>>63)!=0){
							long lowAdditional=uadditional&0xFFFFFFFFFFFFFFL;
							long highAdditional=(uadditional>>56)&0xFF;
							hasBigAdditional=true;
							bigintAdditional=(BigInteger)highAdditional;
							bigintAdditional<<=56;
							bigintAdditional|=(BigInteger)lowAdditional;
						}
						break;
					}
				default:
					break;
			}
			if(type==2){ // Byte string
				if(additional==31){
					// Streaming byte string
					using(MemoryStream ms=new MemoryStream()){
						// Requires same type as this one
						int[] subFlags=new int[]{(1<<type)};
						while(true){
							CBORObject o=Read(s,depth+1,true,type,subFlags,0);
							if(o==null)break;//break if the "break" code was read
							data=(byte[])o.ThisItem;
							ms.Write(data,0,data.Length);
						}
						if(ms.Position>Int32.MaxValue)
							throw new CBORException("Length of bytes to be streamed is bigger than supported");
						data=ms.ToArray();
						return new CBORObject(
							CBORObjectType_ByteString,
							data);
					}
				} else {
					if(hasBigAdditional){
						throw new CBORException("Length of "+
						                        bigintAdditional.ToString(CultureInfo.InvariantCulture)+
						                        " is bigger than supported");
					} else if(uadditional>Int32.MaxValue){
						throw new CBORException("Length of "+
						                        Convert.ToString((long)uadditional,CultureInfo.InvariantCulture)+
						                        " is bigger than supported");
					}
					data=null;
					if(uadditional<=0x10000){
						// Simple case: small size
						data=new byte[(int)uadditional];
						if(s.Read(data,0,data.Length)!=data.Length)
							throw new CBORException("Premature end of stream");
					} else {
						byte[] tmpdata=new byte[0x10000];
						int total=(int)uadditional;
						using(var ms=new MemoryStream()){
							while(total>0){
								int bufsize=Math.Min(tmpdata.Length,total);
								if(s.Read(tmpdata,0,bufsize)!=bufsize)
									throw new CBORException("Premature end of stream");
								ms.Write(tmpdata,0,bufsize);
								total-=bufsize;
							}
							data=ms.ToArray();
						}
					}
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
						if(o==null)break;//break if the "break" code was read
						builder.Append((string)o.ThisItem);
					}
					return new CBORObject(
						CBORObjectType_TextString,
						builder.ToString());
				} else {
					if(hasBigAdditional){
						throw new CBORException("Length of "+
						                        bigintAdditional.ToString(CultureInfo.InvariantCulture)+
						                        " is bigger than supported");
					} else if(uadditional>Int32.MaxValue){
						throw new CBORException("Length of "+
						                        Convert.ToString((long)uadditional,CultureInfo.InvariantCulture)+
						                        " is bigger than supported");
					}
					StringBuilder builder=new StringBuilder();
					switch(CBORDataUtilities.ReadUtf8(s,(int)uadditional,builder,false)){
						case -1:
							throw new CBORException("Invalid UTF-8");
						case -2:
							throw new CBORException("Premature end of data");
						default: // No error
							break;
					}
					return new CBORObject(CBORObjectType_TextString,builder.ToString());
				}
			} else if(type==4){ // Array
				IList<CBORObject> list=new List<CBORObject>();
				int vtindex=1;
				if(additional==31){
					while(true){
						CBORObject o=Read(s,depth+1,true,-1,validTypeFlags,vtindex);
						if(o==null)break;//break if the "break" code was read
						list.Add(o);
						vtindex++;
					}
					return new CBORObject(CBORObjectType_Array,list);
				} else {
					if(hasBigAdditional){
						throw new CBORException("Length of "+
						                        bigintAdditional.ToString(CultureInfo.InvariantCulture)+
						                        " is bigger than supported");
					} else if(uadditional>Int32.MaxValue){
						throw new CBORException("Length of "+
						                        Convert.ToString((long)uadditional,CultureInfo.InvariantCulture)+
						                        " is bigger than supported");
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
						if(key==null)break;//break if the "break" code was read
						CBORObject value=Read(s,depth+1,false,-1,null,0);
						dict[key]=value;
					}
					return new CBORObject(CBORObjectType_Map,dict);
				} else {
					if(hasBigAdditional){
						throw new CBORException("Length of "+
						                        bigintAdditional.ToString(CultureInfo.InvariantCulture)+
						                        " is bigger than supported");
					} else if(uadditional>Int32.MaxValue){
						throw new CBORException("Length of "+
						                        Convert.ToString((long)uadditional,CultureInfo.InvariantCulture)+
						                        " is bigger than supported");
					}
					for(long i=0;i<uadditional;i++){
						CBORObject key=Read(s,depth+1,false,-1,null,0);
						CBORObject value=Read(s,depth+1,false,-1,null,0);
						dict[key]=value;
					}
					return new CBORObject(CBORObjectType_Map,dict);
				}
			} else if(type==6){ // Tagged item
				CBORObject o;
				if(!hasBigAdditional){
					if(uadditional==0){
						// Requires a text string
						int[] subFlags=new int[]{(1<<3)};
						o=Read(s,depth+1,false,-1,subFlags,0);
					} else if(uadditional==2 || uadditional==3){
						// Big number
						// Requires a byte string
						int[] subFlags=new int[]{(1<<2)};
						o=Read(s,depth+1,false,-1,subFlags,0);
						data=(byte[])o.ThisItem;
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
						o=Read(s,depth+1,false,-1,subFlags,0);
						if(o.Count!=2) // Requires 2 items
							throw new CBORException("Decimal fraction requires exactly 2 items");
						// check type of mantissa
						IList<CBORObject> list=o.AsList();
						if(list[1].ItemType!=CBORObjectType_Integer &&
						   list[1].ItemType!=CBORObjectType_BigInteger)
							throw new CBORException("Decimal fraction requires mantissa to be an integer or big integer");
					} else {
						o=Read(s,depth+1,false,-1,null,0);
					}
				} else {
					o=Read(s,depth+1,false,-1,null,0);
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
			} else {
				throw new CBORException("Unexpected data encountered");
			}
		}
	}
}