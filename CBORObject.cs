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
	///
	/// </summary>
	public sealed partial class CBORObject
	{
		private int ItemType {
			get {
				return itemtype_;
			}
		}
		
		int itemtype_;
		Object item;
		int[] tagArray=null;
		
		private const int CBORObjectType_Integer=0; // -(2^63) .. (2^63-1)
		private const int CBORObjectType_BigInteger=1; // all other integers
		private const int CBORObjectType_ByteString=2;
		private const int CBORObjectType_TextString=3;
		private const int CBORObjectType_Array=4;
		private const int CBORObjectType_Map=5;
		private const int CBORObjectType_SimpleValue=6;
		private const int CBORObjectType_Single=7;
		private const int CBORObjectType_Double=8;
		private static readonly BigInteger Int64MaxValue=(BigInteger)Int64.MaxValue;
		private static readonly BigInteger Int64MinValue=(BigInteger)Int64.MinValue;
		public static readonly CBORObject False=new CBORObject(CBORObjectType_SimpleValue,20);
		public static readonly CBORObject True=new CBORObject(CBORObjectType_SimpleValue,21);
		public static readonly CBORObject Null=new CBORObject(CBORObjectType_SimpleValue,22);
		public static readonly CBORObject Undefined=new CBORObject(CBORObjectType_SimpleValue,23);
		
		private CBORObject(){}
		
		private CBORObject(int type, int tagLow, int tagHigh, Object item) : this(type,item) {
			tagArray=new int[]{tagLow,tagHigh};
		}
		private CBORObject(CBORObject obj, int tagLow, int tagHigh) :
			this(obj.itemtype_,obj.item) {
			if(obj.tagArray!=null){
				tagArray=new int[2+obj.tagArray.Length];
				tagArray[0]=tagLow;
				tagArray[1]=tagHigh;
				for(int i=0;i<obj.tagArray.Length;i++){
					tagArray[i+2]=(int)obj.tagArray[i];
				}
			} else {
				tagArray=new int[]{tagLow,tagHigh};
			}
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
						if((int)item==21 || (int)item==20){
							return CBORType.Boolean;
						}
						return CBORType.SimpleValue;
					case CBORObjectType_Array:
						if(this.Count==2 && this.HasTag(4)){
							return CBORType.Number;
						}
						return CBORType.Array;
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
				return this.ItemType==CBORObjectType_SimpleValue && (int)item==21;
			}
		}
		/// <summary>
		/// Gets whether this value is a CBOR false value.
		/// </summary>
		public bool IsFalse {
			get {
				return this.ItemType==CBORObjectType_SimpleValue && (int)item==20;
			}
		}
		/// <summary>
		/// Gets whether this value is a CBOR null value.
		/// </summary>
		public bool IsNull {
			get {
				return this.ItemType==CBORObjectType_SimpleValue && (int)item==22;
			}
		}
		/// <summary>
		/// Gets whether this value is a CBOR undefined value.
		/// </summary>
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

		private bool TagListEquals(
			int[] arrayA,
			int[] arrayB
		){
			if(arrayA==null)return (arrayB==null);
			if(arrayB==null)return false;
			if(arrayA.Length!=arrayB.Length)return false;
			int count=arrayA.Length;
			for(int i=0;i<count;i++){
				if(arrayA[i]!=arrayB[i])return false;
			}
			return true;
		}

		private int TagListHashCode(int[] array){
			if(array==null)return 0;
			int ret=19;
			unchecked {
				int count=array.Length;
				ret=ret*31+count;
				for(int i=0;i<count;i++){
					ret=ret*31+(int)array[i];
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
			return this.ItemType == other.ItemType && TagListEquals(tagArray,other.tagArray);
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
				hashCode += 1000000009 * TagListHashCode(this.tagArray);
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
			0,0,0,0,0,0,0,0, 0,0,0,0,-1,-1,-1,0,
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
				ReadUtf8FromBytes(data,1,firstbyte-0x60,ret);
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
		/// <returns></returns>
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
				if(s!=null)return new CBORObject(CBORObjectType_TextString,0,0,s);
			}
			if(expectedLength!=0){
				return GetFixedLengthObject(firstbyte, data);
			}
			// For complex cases, such as arrays and maps,
			// read the object as though
			// the byte array were a stream
			try {
				using(MemoryStream ms=new MemoryStream(data)){
					CBORObject o=Read(ms);
					CheckCBORLength((long)data.Length,(long)ms.Position);
					return o;
				}
			} catch(IOException ex){
				throw new CBORException("I/O error occurred.",ex);
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
		/// Gets whether this data item has at least one tag.
		/// </summary>
		public bool IsTagged {
			get {
				return tagArray!=null && tagArray.Length>0;
			}
		}

		private bool HasTag(int tagValue){
			if(tagArray==null)return false;
			if((tagValue)<0)throw new ArgumentOutOfRangeException(
				"tagValue"+" not greater or equal to "+"0"+" ("+Convert.ToString((int)tagValue,System.Globalization.CultureInfo.InvariantCulture)+")");
			int length=tagArray.Length;
			for(int i=0;i<length;i+=2){
				int low=(int)tagArray[i];
				int high=(int)tagArray[i+1];
				if(high==0 && tagValue==low)return true;
			}
			return false;
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
		
		public BigInteger[] GetTags(){
			if(!this.IsTagged)return EmptyTags;
			BigInteger[] ret=new BigInteger[tagArray.Length/2];
			for(int i=0;i<tagArray.Length;i+=2){
				int tagLow=(int)tagArray[i];
				int tagHigh=(int)tagArray[i+1];
				ret[i/2]=LowHighToBigInteger(tagLow,tagHigh);
			}
			return ret;
		}
		
		/// <summary>
		/// Gets the last defined tag for this CBOR data item,
		/// or 0 if the item is untagged.
		/// </summary>
		public BigInteger InnermostTag {
			get {
				if(this.IsTagged){
					int tagLow=(int)tagArray[tagArray.Length-2];
					int tagHigh=(int)tagArray[tagArray.Length-1];
					if(tagHigh==0 && tagLow>=0 && tagLow<0x10000){
						return (BigInteger)tagLow;
					}
					return LowHighToBigInteger(tagLow,tagHigh);
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
			set {
				if(this.ItemType== CBORObjectType_Array){
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
		/// Gets or sets the value of a CBOR object in this
		/// map.
		/// </summary>
		/// <exception cref="System.ArgumentNullException">The key is null
		/// (as opposed to CBORObject.Null</exception>.
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
			set {
				if((key)==null)throw new ArgumentNullException("key");
				if(this.ItemType== CBORObjectType_Map){
					IDictionary<CBORObject,CBORObject> map=AsMap();
					map[key]=value;
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
					return (int)item;
				} else {
					return -1;
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
				if(this.HasTag(4))
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
		/// Returns false if this object isFalse, Null, or Undefined;
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
		/// This object's type is not an integer
		/// or a floating-point number.</exception>
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
		/// This object's type is not an integer
		/// or a floating-point number.</exception>
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
		
		public static void WriteStreamedString(String str, Stream stream){
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
		/// <param name="str"></param>
		/// <param name="s"></param>
		public static void Write(string str, Stream s){
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
		
		/// <summary>
		/// Writes this CBOR object to a data stream.
		/// </summary>
		/// <param name="s">A writable data stream.</param>
		public void WriteTo(Stream s){
			if((s)==null)throw new ArgumentNullException("s");
			WriteTags(s);
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
				if(value<24){
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
				throw new ArgumentException("Unexpected data type");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
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
		/// 
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
		public static void Write(int value, Stream s){
			Write((long)value,s);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
		public static void Write(short value, Stream s){
			Write((long)value,s);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
		public static void Write(char value, Stream s){
			Write(new String(new char[]{value}),s);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
		public static void Write(bool value, Stream s){
			s.WriteByte(value ? (byte)0xf5 : (byte)0xf4);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
		public static void Write(byte value, Stream s){
			if((((int)value)&0xFF)<24){
				s.WriteByte(value);
			} else {
				s.WriteByte((byte)(24));
				s.WriteByte(value);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
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
		/// 
		/// </summary>
		/// <param name="value">The value to write</param>
		/// <param name="s">A writable data stream.</param>
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
			// overhead since the amount of memory they
			// use is fixed and small
			bool hasComplexTag=false;
			byte tagbyte=0;
			bool tagged=this.IsTagged;
			if(this.IsTagged){
				if(this.tagArray.Length>2 ||
				   (int)this.tagArray[1]!=0 ||
				   (((int)this.tagArray[0])>>16)!=0 ||
				   this.tagArray[0]>=24){
					hasComplexTag=true;
				} else {
					tagbyte=(byte)(0xC0+(int)this.tagArray[0]);
				}
			}
			if(!hasComplexTag){
				if(this.ItemType== CBORObjectType_TextString){
					byte[] ret=GetOptimizedBytesIfShortAscii(
						(string)item,
						tagged ? (((int)tagbyte)&0xFF) : -1);
					if(ret!=null)return ret;
				} else if(this.ItemType== CBORObjectType_Integer){
					long value=(long)item;
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
						(float)item);
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
						(double)item);
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
		/// <param name="str"></param>
		/// <returns></returns>
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
				if(this.HasTag(22)){
					ToBase64(sb,(byte[])item,Base64,false);
				} else if(this.HasTag(23)){
					ToBase16(sb,(byte[])item);
				} else {
					ToBase64(sb,(byte[])item,Base64URL,false);
				}
				sb.Append('\"');
				return sb.ToString();
			} else if(this.ItemType== CBORObjectType_TextString){
				return StringToJSONString((string)item);
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
			int smallNumber=0; // for small numbers (9 digits or less)
			int smallNumberCount=0;
			if(c>='1' && c<='9'){
				smallNumber=(int)(c-'0');
				smallNumberCount++;
				while(index<str.Length){
					c=str[index];
					if(c>='0' && c<='9'){
						index++;
						numberEnd=index;
						if(smallNumberCount<9){
							smallNumber*=10;
							smallNumber+=(int)(c-'0');
							smallNumberCount++;
						}
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
			if(index!=str.Length){
				// End of the string wasn't reached, so isn't a number
				return null;
			}
			if(fracStart<0 && expStart<0 && (numberEnd-numberStart)<=9){
				// Common case: small integer
				int value=smallNumber;
				if(negative)value=-value;
				return FromObject(value);
			} if(fracStart<0 && expStart<0 && (numberEnd-numberStart)<=18){
				// Common case: long-sized integer
				string strsub=(numberStart==0 && numberEnd==str.Length) ? str :
					str.Substring(numberStart,numberEnd-numberStart);
				long value=Int64.Parse(strsub,
				                       NumberStyles.None,
				                       CultureInfo.InvariantCulture);
				if(negative)value=-value;
				return FromObject(value);
			} else if(fracStart>=0 && expStart<0 &&
			          (numberEnd-numberStart)+(fracEnd-fracStart)<=9){
				// Small whole part and small fractional part
				int fracpart=(fracStart<0) ? 0 : Int32.Parse(
					str.Substring(fracStart,fracEnd-fracStart),
					NumberStyles.None,
					CultureInfo.InvariantCulture);
				// Intval consists of the whole and fractional part
				string intvalString=str.Substring(numberStart,numberEnd-numberStart)+
					(fracpart==0 ? String.Empty : str.Substring(fracStart,fracEnd-fracStart));
				int int32val=Int32.Parse(
					intvalString,
					NumberStyles.None,
					CultureInfo.InvariantCulture);
				if(negative)int32val=-int32val;
				int exp=0;
				if(fracpart!=0){
					// If there is a nonzero fractional part,
					// decrease the exponent by that part's length
					exp-=(int)(fracEnd-fracStart);
				}
				if(exp==0){
					// If exponent is 0, just return the integer
					return FromObject(int32val);
				}
				// Represent the CBOR object as a decimal fraction
				return FromObjectAndTag(new CBORObject[]{
				                        	FromObject(exp),FromObject(int32val)},4);
			} else if(fracStart<0 && expStart<0){
				// Bigger integer
				string strsub=(numberStart==0 && numberEnd==str.Length) ? str :
					str.Substring(numberStart,numberEnd-numberStart);
				BigInteger bigintValue=BigInteger.Parse(strsub,
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
				CBORObject obj=ParseJSONNumber(str);
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

		private static int ReadUtf8FromBytes(byte[] data, int offset, int byteLength, StringBuilder builder)  {
			int cp=0;
			int bytesSeen=0;
			int bytesNeeded=0;
			int lower=0x80;
			int upper=0xBF;
			int pointer=offset;
			int endpointer=offset+byteLength;
			while(pointer<endpointer){
				int b=data[pointer];
				pointer++;
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
						return -1;
					continue;
				}
				if(b<lower || b>upper){
					cp=bytesNeeded=bytesSeen=0;
					lower=0x80;
					upper=0xbf;
					return -1;
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
			if(bytesNeeded!=0)
				return -1;
			return 0;
		}


		private static int ReadUtf8(Stream stream, int byteLength, StringBuilder builder)  {
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
						return -1;
					} else {
						if(byteLength>0 && pointer>=byteLength)
							return -2;
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
						return -1;
					continue;
				}
				if(b<lower || b>upper){
					cp=bytesNeeded=bytesSeen=0;
					lower=0x80;
					upper=0xbf;
					return -1;
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
			return 0;
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
		/// <summary>
		/// 
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
				list.Add(FromObject(i));
			}
			return new CBORObject(CBORObjectType_Array,list);
		}
		public static CBORObject FromObject<T>(IList<T> value){
			if(value==null)return CBORObject.Null;
			IList<CBORObject> list=new List<CBORObject>();
			foreach(T i in (IList<T>)value){
				CBORObject obj=FromObject(i);
				list.Add(obj);
			}
			return new CBORObject(CBORObjectType_Array,list);
		}
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
			if(o is decimal)return FromObject((decimal)o);
			if(o is DateTime)return FromObject((DateTime)o);
			
			if(o is double)return FromObject((double)o);
			if(o is IList<CBORObject>)return FromObject((IList<CBORObject>)o);
			if(o is byte[])return FromObject((byte[])o);
			if(o is CBORObject[])return FromObject((CBORObject[])o);
			if(o is IDictionary<CBORObject,CBORObject>)return FromObject(
				(IDictionary<CBORObject,CBORObject>)o);
			if(o is IDictionary<string,CBORObject>)return FromObject(
				(IDictionary<string,CBORObject>)o);
			throw new ArgumentException("Unsupported object type.");
		}
		
		private static BigInteger BigInt65536=(BigInteger)65536;

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
				BigInteger tmpbigint=bigintTag&(BigInteger)0xFFFFFFFFFFFFFFL;
				tagLow=(long)(BigInteger)tmpbigint;
				tmpbigint=(bigintTag>>56)&(BigInteger)0xFF;
				tagHigh=(long)(BigInteger)tmpbigint;
				int low=unchecked((int)(tagLow&0xFFFFFFFFL));
				tagHigh=(tagHigh<<24)|((tagLow>>32)&0xFFFFFFFFL);
				int high=unchecked((int)(tagHigh&0xFFFFFFFFL));
				return new CBORObject(c,low,high);
			}
		}

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
				long v=(long)item;
				return Convert.ToString((long)v,CultureInfo.InvariantCulture);
			} else if(this.ItemType== CBORObjectType_BigInteger){
				return ((BigInteger)item).ToString(CultureInfo.InvariantCulture);
			} else {
				throw new InvalidOperationException("Unsupported data type");
			}
		}
		
		private void AppendClosingTags(StringBuilder sb){
			if(this.tagArray==null)return;
			int count=this.tagArray.Length;
			for(int i=0;i<count;i+=2){
				sb.Append(')');
			}
		}
		
		private void WriteTags(Stream s){
			if(this.tagArray==null)return;
			int count=this.tagArray.Length;
			// Tags are ordered from top to bottom
			for(int i=0;i<count;i+=2){
				int low=this.tagArray[i];
				int high=this.tagArray[i+1];
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
					s.Write(
						new byte[]{(byte)(0xDB),
							(byte)((high>>24)&0xFF),
							(byte)((high>>16)&0xFF),
							(byte)((high>>8)&0xFF),
							(byte)(high&0xFF),
							(byte)((low>>24)&0xFF),
							(byte)((low>>16)&0xFF),
							(byte)((low>>8)&0xFF),
							(byte)(low&0xFF)},0,9);
				}
			}
		}

		private void AppendOpeningTags(StringBuilder sb){
			if(this.tagArray==null)return;
			int count=this.tagArray.Length;
			// Tags are ordered from top to bottom
			for(int i=0;i<count;i+=2){
				int low=this.tagArray[i];
				int high=this.tagArray[i+1];
				if(high==0 && (low>>16)==0){
					sb.Append(Convert.ToString((int)low,CultureInfo.InvariantCulture));
				} else {
					BigInteger bi=LowHighToBigInteger(low,high);
					sb.Append(bi.ToString(CultureInfo.InvariantCulture));
				}
				sb.Append('(');
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
			if(this.IsTagged){
				if(sb==null){
					if(this.ItemType==CBORObjectType_TextString){
						// The default capacity of StringBuilder may be too small
						// for many strings, so set a suggested capacity
						// explicitly
						string str=(string)item;
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
				if(sb==null){
					return "\""+((string)item)+"\"";
				} else {
					sb.Append('\"');
					sb.Append((string)item);
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
							data=(byte[])o.item;
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
						builder.Append((string)o.item);
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
					switch(ReadUtf8(s,(int)uadditional,builder)){
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
						data=(byte[])o.item;
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