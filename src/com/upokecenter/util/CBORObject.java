package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.util.*;

import java.io.*;
import java.math.*;



	/**
	 * Represents an object in Concise Binary Object Representation (CBOR)
	 * and contains methods for reading and writing CBOR data. CBOR is defined
	 * in RFC 7049. <p> Thread Safety: CBOR objects that are numbers, "simple
	 * values", and text strings are immutable (their values can't be changed),
	 * so they are inherently safe for use by multiple threads. CBOR objects
	 * that are arrays, maps, and byte strings are mutable, but this class
	 * doesn't attempt to synchronize reads and writes to those objects
	 * by multiple threads, so those objects are not thread safe without
	 * such synchronization. </p>
	 */
	public final class CBORObject implements Comparable<CBORObject>
	{
		private int getItemType() {
				CBORObject curobject=this;
				while(curobject.itemtype_==CBORObjectType_Tagged) {
					curobject=((CBORObject)curobject.item_);
				}
				return curobject.itemtype_;
			}

		private Object getThisItem() {
				CBORObject curobject=this;
				while(curobject.itemtype_==CBORObjectType_Tagged) {
					curobject=((CBORObject)curobject.item_);
				}
				return curobject.item_;
			}

		int itemtype_;
		Object item_;
		int tagLow;
		int tagHigh;
		
		private static final int CBORObjectType_Integer=0; // -(2^63) .. (2^63-1)
		private static final int CBORObjectType_BigInteger=1; // all other integers
		private static final int CBORObjectType_ByteString=2;
		private static final int CBORObjectType_TextString=3;
		private static final int CBORObjectType_Array=4;
		private static final int CBORObjectType_Map=5;
		private static final int CBORObjectType_SimpleValue=6;
		private static final int CBORObjectType_Single=7;
		private static final int CBORObjectType_Double=8;
		private static final int CBORObjectType_DecimalFraction=9;
		private static final int CBORObjectType_Tagged=10;
		private static final BigInteger Int64MaxValue=BigInteger.valueOf(Long.MAX_VALUE);
		private static final BigInteger Int64MinValue=BigInteger.valueOf(Long.MIN_VALUE);
		/**
		 * Represents the value false.
		 */
		public static final CBORObject False=new CBORObject(CBORObjectType_SimpleValue,20);
		/**
		 * Represents the value true.
		 */
		public static final CBORObject True=new CBORObject(CBORObjectType_SimpleValue,21);
		/**
		 * Represents the value null.
		 */
		public static final CBORObject Null=new CBORObject(CBORObjectType_SimpleValue,22);
		/**
		 * Represents the value undefined.
		 */
		public static final CBORObject Undefined=new CBORObject(CBORObjectType_SimpleValue,23);
		
		private CBORObject(){}
		
		private CBORObject(CBORObject obj, int tagLow, int tagHigh){
 this(CBORObjectType_Tagged,obj);
			this.tagLow=tagLow;
			this.tagHigh=tagHigh;
		}
		
		
		
		private CBORObject(int type, Object item){
			
			this.itemtype_=type;
			this.item_=item;
		}
		
		/**
		 * Gets the general data type of this CBOR object.
		 */
		public CBORType getType() {
				switch(this.getItemType()){
					case CBORObjectType_Integer:
					case CBORObjectType_BigInteger:
					case CBORObjectType_Single:
					case CBORObjectType_Double:
					case CBORObjectType_DecimalFraction:
						return CBORType.Number;
					case CBORObjectType_SimpleValue:
						if(((Integer)this.getThisItem()).intValue()==21 || ((Integer)this.getThisItem()).intValue()==20){
							return CBORType.Boolean;
						}
						return CBORType.SimpleValue;
					case CBORObjectType_Array:
						return CBORType.Array;
					case CBORObjectType_Map:
						return CBORType.Map;
					case CBORObjectType_ByteString:
						return CBORType.ByteString;
					case CBORObjectType_TextString:
						return CBORType.TextString;
					default:
						throw new IllegalStateException("Unexpected data type");
				}
			}
		
		/**
		 * Gets whether this value is a CBOR true value.
		 */
		public boolean isTrue() {
				return this.getItemType()==CBORObjectType_SimpleValue && ((Integer)this.getThisItem()).intValue()==21;
			}
		/**
		 * Gets whether this value is a CBOR false value.
		 */
		public boolean isFalse() {
				return this.getItemType()==CBORObjectType_SimpleValue && ((Integer)this.getThisItem()).intValue()==20;
			}
		/**
		 * Gets whether this value is a CBOR null value.
		 */
		public boolean isNull() {
				return this.getItemType()==CBORObjectType_SimpleValue && ((Integer)this.getThisItem()).intValue()==22;
			}
		/**
		 * Gets whether this value is a CBOR undefined value.
		 */
		public boolean isUndefined() {
				return this.getItemType()==CBORObjectType_SimpleValue && ((Integer)this.getThisItem()).intValue()==23;
			}
		
		
		/**
		 * Compares two CBOR objects. <p>In this implementation:</p> <ul>
		 * <li>If either value is true, it is treated as the number 1.</li> <li>If
		 * either value is false, null, or the undefined value, it is treated
		 * as the number 0.</li> <li>If both objects are numbers, their mathematical
		 * values are compared.</li> <li>If both objects are arrays, each element
		 * is compared. If one array is shorter than the other and the other array
		 * begins with that array (for the purposes of comparison), the shorter
		 * array is considered less than the longer array.</li> <li>If both
		 * objects are maps, returns 0.</li> </ul>
		 * @param other A value to compare with.
		 * @return Less than 0, if this value is less than the other object; or
		 * 0, if both values are equal; or greater than 0, if this value is less
		 * than the other object or if the other object is null.
		 */
		@SuppressWarnings("unchecked")
public int compareTo(CBORObject other) {
			if(other==null)return 1;
			int typeA=this.getItemType();
			int typeB=other.getItemType();
			Object objA=this.getThisItem();
			Object objB=other.getThisItem();
			if(typeA==CBORObjectType_SimpleValue){
				if(((Integer)objA).intValue()==20 || ((Integer)objA).intValue()==22 || ((Integer)objA).intValue()==23){
					// Treat false, null, and undefined
					// as the number 0
					objA=(long)0;
					typeA=CBORObjectType_Integer;
				}
				else if(((Integer)objA).intValue()==21){
					// Treat true as the number 1
					objA=(long)1;
					typeA=CBORObjectType_Integer;
				}
			}
			if(typeB==CBORObjectType_SimpleValue){
				if(((Integer)objB).intValue()==20 || ((Integer)objB).intValue()==22 || ((Integer)objB).intValue()==23){
					// Treat false, null, and undefined
					// as the number 0
					objA=(long)0;
					typeA=CBORObjectType_Integer;
				}
				else if(((Integer)objB).intValue()==21){
					// Treat true as the number 1
					objA=(long)1;
					typeA=CBORObjectType_Integer;
				}
			}
			if(typeA==typeB){
				switch(typeA){
						case CBORObjectType_Integer:{
							long a=((Long)this.getThisItem()).longValue();
							long b=((Long)other.getThisItem()).longValue();
							if(a==b)return 0;
							return (a<b) ? -1 : 1;
						}
						case CBORObjectType_Single:{
							float a=((Float)this.getThisItem()).floatValue();
							float b=((Float)other.getThisItem()).floatValue();
							if(Float.isNaN(a)){
								return (Float.isNaN(b)) ? 1 : 0;
							}
							if(Float.isNaN(b)){
								return -1;
							}
							if(a==b)return 0;
							return (a<b) ? -1 : 1;
						}
						case CBORObjectType_BigInteger:{
							BigInteger bigintA=(BigInteger)this.getThisItem();
							BigInteger bigintB=(BigInteger)other.getThisItem();
							return bigintA.compareTo(bigintB);
						}
						case CBORObjectType_Double:{
							double a=((Double)this.getThisItem()).doubleValue();
							double b=((Double)other.getThisItem()).doubleValue();
							if(Double.isNaN(a)){
								return (Double.isNaN(b)) ? 1 : 0;
							}
							if(Double.isNaN(b)){
								return -1;
							}
							if(a==b)return 0;
							return (a<b) ? -1 : 1;
						}
						case CBORObjectType_DecimalFraction:{
							return ((DecimalFraction)objA).compareTo(
								((DecimalFraction)objB));
						}
						case CBORObjectType_ByteString:{
							return ByteArrayCompare((byte[])objA,(byte[])objB);
						}
						case CBORObjectType_TextString:{
							throw new UnsupportedOperationException();
						}
						case CBORObjectType_Array:{
							return ListCompare((ArrayList<CBORObject>)objA,
							                   (ArrayList<CBORObject>)objB);
						}
						case CBORObjectType_Map:{
							return 0;
						}
						case CBORObjectType_SimpleValue:{
							int valueA=((Integer)objA).intValue();
							int valueB=((Integer)objB).intValue();
							if(valueA==valueB)return 0;
							return (valueA<valueB) ? -1 : 1;
						}
					default:
						throw new IllegalArgumentException("Unexpected data type");
				}
			} else {
				int combo=(typeA<<4)|typeB;
				switch(combo){
						case (CBORObjectType_Integer<<4)|CBORObjectType_BigInteger:{
							BigInteger bigint=BigInteger.valueOf(((Long)objA).longValue());
							return bigint.compareTo((BigInteger)objB);
						}
						case (CBORObjectType_BigInteger<<4)|CBORObjectType_Integer:{
							BigInteger bigint=BigInteger.valueOf(((Long)objB).longValue());
							return (BigInteger.valueOf(((Long)objA).longValue())).compareTo(bigint);
						}
						case (CBORObjectType_Integer<<4)|CBORObjectType_DecimalFraction:{
							DecimalFraction xa=new DecimalFraction(((Long)objA).longValue());
							DecimalFraction xb=(DecimalFraction)objB;
							return xa.compareTo(xb);
						}
						case (CBORObjectType_DecimalFraction<<4)|CBORObjectType_Integer:{
							DecimalFraction xb=new DecimalFraction(((Long)objB).longValue());
							DecimalFraction xa=(DecimalFraction)objA;
							return xa.compareTo(xb);
						}
						case (CBORObjectType_BigInteger<<4)|CBORObjectType_DecimalFraction:{
							DecimalFraction xa=new DecimalFraction((BigInteger)objA);
							DecimalFraction xb=(DecimalFraction)objB;
							return xa.compareTo(xb);
						}
						case (CBORObjectType_DecimalFraction<<4)|CBORObjectType_BigInteger:{
							DecimalFraction xb=new DecimalFraction((BigInteger)objB);
							DecimalFraction xa=(DecimalFraction)objA;
							return xa.compareTo(xb);
						}
					default:
						throw new UnsupportedOperationException();
				}
			}
		}

		
		private boolean ByteArrayEquals(byte[] a, byte[] b) {
			if(a==null)return (b==null);
			if(b==null)return false;
			if(a.length!=b.length)return false;
			for(int i=0;i<a.length;i++){
				if(a[i]!=b[i])return false;
			}
			return true;
		}
		
		private int ByteArrayCompare(byte[] a, byte[] b) {
			if(a==null)return (b==null) ? 0 : -1;
			if(b==null)return 1;
			int c=Math.min(a.length,b.length);
			for(int i=0;i<c;i++){
				if(a[i]!=b[i])
					return (a[i]<b[i]) ? -1 : 1;
			}
			if(a.length!=b.length)
				return (a.length<b.length) ? -1 : 1;
			return 0;
		}
		
		private int ListCompare(ArrayList<CBORObject> listA, ArrayList<CBORObject> listB) {
			if(listA==null)return (listB==null) ? 0 : -1;
			if(listB==null)return 1;
			int c=Math.min(listA.size(),listB.size());
			for(int i=0;i<c;i++){
				int cmp=listA.get(i).compareTo(listB.get(i));
				if(cmp!=0)return cmp;
			}
			if(listA.size()!=listB.size())
				return (listA.size()<listB.size()) ? -1 : 1;
			return 0;
		}
		private int ByteArrayHashCode(byte[] a) {
			if(a==null)return 0;
			int ret=19;
			{
				ret=ret*31+a.length;
				for(int i=0;i<a.length;i++){
					ret=ret*31+a[i];
				}
			}
			return ret;
		}

		private boolean CBORArrayEquals(
			List<CBORObject> listA,
			List<CBORObject> listB
		) {
			if(listA==null)return (listB==null);
			if(listB==null)return false;
			if(listA.size()!=listB.size())return false;
			for(int i=0;i<listA.size();i++){
				if(!(((listA.get(i))==null) ? ((listB.get(i))==null) : (listA.get(i)).equals(listB.get(i))))return false;
			}
			return true;
		}

		private int CBORArrayHashCode(List<CBORObject> list) {
			if(list==null)return 0;
			int ret=19;
			int count=list.size();
			{
				ret=ret*31+count;
				for(int i=0;i<count;i++){
					ret=ret*31+list.get(i).hashCode();
				}
			}
			return ret;
		}
		
		private boolean CBORMapEquals(
			Map<CBORObject,CBORObject> mapA,
			Map<CBORObject,CBORObject> mapB
		) {
			if(mapA==null)return (mapB==null);
			if(mapB==null)return false;
			if(mapA.size()!=mapB.size())return false;
			for(CBORObject k : mapA.keySet()){
				if(mapB.containsKey(k)){
					if(!(((mapA.get(k))==null) ? ((mapB.get(k))==null) : (mapA.get(k)).equals(mapB.get(k))))
						return false;
				} else {
					return false;
				}
			}
			return true;
		}

		private int CBORMapHashCode(Map<CBORObject,CBORObject> a) {
			// To simplify matters, we use just the count of
			// the map as the basis for the hash code.  More complicated
			// hash code calculation would generally involve defining
			// how CBORObjects ought to be compared (since a stable
			// sort order is necessary for two equal maps to have the
			// same hash code), which is much too difficult to do.
			return (a.size()*19);
		}

		/**
		 * Compares the equality of two CBOR objects.
		 * @param obj The object to compare
		 * @return true if the objects are equal; otherwise, false.
		 */
		@Override @SuppressWarnings("unchecked")
public boolean equals(Object obj) {
			CBORObject other = ((obj instanceof CBORObject) ? (CBORObject)obj : null);
			if (other == null)
				return false;
			if(item_ instanceof byte[]){
				if(!ByteArrayEquals((byte[])this.getThisItem(),((other.item_ instanceof byte[]) ? (byte[])other.item_ : null)))
					return false;
			} else if(item_ instanceof List<?>){
				if(!CBORArrayEquals(AsList(),((other.item_ instanceof List<?>) ? (List<CBORObject>)other.item_ : null)))
					return false;
			} else if(item_ instanceof Map<?,?>){
				if(!CBORMapEquals(AsMap(),
				                  ((other.item_ instanceof Map<?,?>) ? (Map<CBORObject,CBORObject>)other.item_ : null)))
					return false;
			} else {
				if(!(((this.item_)==null) ? ((other.item_)==null) : (this.item_).equals(other.item_)))
					return false;
			}
			return this.getItemType() == other.getItemType() &&
				this.tagLow == other.tagLow &&
				this.tagHigh == other.tagHigh;
		}
		
		/**
		 * Calculates the hash code of this object.
		 */
		@Override public int hashCode() {
			int hashCode_ = 0;
			{
				if (item_ != null){
					int itemHashCode=0;
					if(item_ instanceof byte[])
						itemHashCode=ByteArrayHashCode((byte[])this.getThisItem());
					else if(item_ instanceof List<?>)
						itemHashCode=CBORArrayHashCode(AsList());
					else if(item_ instanceof Map<?,?>)
						itemHashCode=CBORMapHashCode(AsMap());
					else
						itemHashCode=item_.hashCode();
					hashCode_ += 1000000007 * itemHashCode;
				}
				hashCode_ += 1000000009 * this.getItemType();
				hashCode_ += 1000000009 * this.tagLow;
				hashCode_ += 1000000009 * this.tagHigh;
			}
			return hashCode_;
		}
		
		
		private static void CheckCBORLength(long expectedLength, long actualLength) {
			if(actualLength<expectedLength)
				throw new CBORException("Premature end of data");
			else if(actualLength>expectedLength)
				throw new CBORException("Too many bytes");
		}

		private static void CheckCBORLength(int expectedLength, int actualLength) {
			if(actualLength<expectedLength)
				throw new CBORException("Premature end of data");
			else if(actualLength>expectedLength)
				throw new CBORException("Too many bytes");
		}
		
		private static String GetOptimizedStringIfShortAscii(
			byte[] data, int offset
		) {
			int length=data.length;
			if(length>offset){
				int nextbyte=((int)(data[offset]&(int)0xFF));
				if(nextbyte>=0x60 && nextbyte<0x78){
					int offsetp1=1+offset;
					// Check for type 3 String of short length
					int rightLength=offsetp1+(nextbyte-0x60);
					CheckCBORLength(rightLength,length);
					// Check for all ASCII text
					for(int i=offsetp1;i<length;i++){
						if((data[i]&((byte)0x80))!=0){
							return null;
						}
					}
					// All ASCII text, so convert to a String
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
		private static CBORObject[] InitializeFixedObjects() {
			FixedObjects=new CBORObject[256];
			for(int i=0;i<0x18;i++){
				FixedObjects[i]=new CBORObject(CBORObjectType_Integer,(long)i);
			}
			for(int i=0x20;i<0x38;i++){
				FixedObjects[i]=new CBORObject(CBORObjectType_Integer,
				                               (long)(-1-(i-0x20)));
			}
			FixedObjects[0x60]=new CBORObject(CBORObjectType_TextString,"");
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
		
		// Generate a CBOR Object for head bytes with fixed length.
		// Note that this function assumes that the length of the data
		// was already checked.
		private static CBORObject GetFixedLengthObject(int firstbyte, byte[] data) {
			CBORObject fixedObj=FixedObjects[firstbyte];
			if(fixedObj!=null)
				return fixedObj;
			int majortype=(firstbyte>>5);
			if(firstbyte>=0x61 && firstbyte<0x78){
				// text String length 1 to 23
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
							BigInteger bigintAdditional=BigInteger.valueOf(highAdditional);
							bigintAdditional=bigintAdditional.shiftLeft(56);
							bigintAdditional=bigintAdditional.or(BigInteger.valueOf(lowAdditional));
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
							BigInteger bigintAdditional=BigInteger.valueOf(highAdditional);
							bigintAdditional=bigintAdditional.shiftLeft(56);
							bigintAdditional=bigintAdditional.or(BigInteger.valueOf(lowAdditional));
							bigintAdditional=(BigInteger.valueOf(-1)).subtract(bigintAdditional);
							return FromObject(bigintAdditional);
						}
					case 7:
						if(firstbyte==0xf9)
							return new CBORObject(
								CBORObjectType_Single,HalfPrecisionToSingle(
									((int)uadditional)));
						else if(firstbyte==0xfa)
							return new CBORObject(
								CBORObjectType_Single,
								Float.intBitsToFloat(
									((int)uadditional)));
						else if(firstbyte==0xfb)
							return new CBORObject(
								CBORObjectType_Double,
								Double.longBitsToDouble(uadditional));
						else if(firstbyte==0xf8)
							return new CBORObject(
								CBORObjectType_SimpleValue,(int)uadditional);
						else
							throw new CBORException("Unexpected data encountered");
					default:
						throw new CBORException("Unexpected data encountered");
				}
			}
			else if(majortype==2){ // short byte String
				byte[] ret=new byte[firstbyte-0x40];
				System.arraycopy(data,1,ret,0,firstbyte-0x40);
				return new CBORObject(CBORObjectType_ByteString,ret);
			}
			else if(majortype==3){ // short text String
				StringBuilder ret=new StringBuilder(firstbyte-0x60);
				CBORDataUtilities.ReadUtf8FromBytes(data,1,firstbyte-0x60,ret,false);
				return new CBORObject(CBORObjectType_TextString,ret.toString());
			}
			else if(firstbyte==0x80) // empty array
				return FromObject(new ArrayList<CBORObject>());
			else if(firstbyte==0xA0) // empty map
				return FromObject(new HashMap<CBORObject,CBORObject>());
			else
				throw new CBORException("Unexpected data encountered");
		}
		
		/**
		 * Generates a CBOR object from an array of CBOR-encoded bytes.
		 * @param data
		 * @return A CBOR object corresponding to the data.
		 * @throws java.lang.IllegalArgumentException data is null or empty.
		 * @throws CBORException There was an error in reading or parsing the
		 * data.
		 */
		public static CBORObject DecodeFromBytes(byte[] data) {
			if((data)==null)throw new NullPointerException("data");
			if((data).length==0)throw new IllegalArgumentException("data is empty.");
			int firstbyte=((int)(data[0]&(int)0xFF));
			int expectedLength=ExpectedLengths[firstbyte];
			if(expectedLength==-1) // if invalid
				throw new CBORException("Unexpected data encountered");
			else if(expectedLength!=0) // if fixed length
				CheckCBORLength(expectedLength,data.length);
			if(firstbyte==0xc0){
				// value with tag 0
				String s=GetOptimizedStringIfShortAscii(data,1);
				if(s!=null)return new CBORObject(FromObject(s),0,0);
			}
			if(expectedLength!=0){
				return GetFixedLengthObject(firstbyte, data);
			}
			// For objects with variable length,
			// read the Object as though
			// the byte array were a stream
			java.io.ByteArrayInputStream ms=null;
try {
ms=new ByteArrayInputStream(data);
int startingAvailable=ms.available();

				CBORObject o=Read(ms);
				CheckCBORLength((long)data.length,(long)(startingAvailable-ms.available()));
				return o;
}
finally {
try { if(ms!=null)ms.close(); } catch(IOException ex){}
}
		}
		
		/**
		 * Gets the number of keys in this map, or the number of items in this array,
		 * or 0 if this item is neither an array nor a map.
		 */
		public int size() {
				if(this.getItemType()== CBORObjectType_Array){
					return (AsList()).size();
				} else if(this.getItemType()== CBORObjectType_Map){
					return (AsMap()).size();
				} else {
					return 0;
				}
			}
		
		/**
		 * Gets whether this data item has at least one tag.
		 */
		public boolean isTagged() {
				return this.itemtype_==CBORObjectType_Tagged;
			}
		
		/**
		 * Gets the byte array used in this object, if this object is a byte string.
		 * @throws IllegalStateException This object is not a byte string.
		 */
		public byte[] GetByteString() {
			if(this.itemtype_==CBORObjectType_ByteString)
				return ((byte[])this.getThisItem());
			else
				throw new IllegalStateException("Not a byte String");
		}
		


		private boolean HasTag(int tagValue) {
			if(!this.isTagged())return false;
			if(tagHigh==0 && tagValue==tagLow)return true;
			return ((CBORObject)item_).HasTag(tagValue);
		}
		
		private BigInteger LowHighToBigInteger(int tagLow, int tagHigh) {
			
			BigInteger bi;
			if(tagHigh!=0){
				bi=BigInteger.valueOf(((tagHigh>>16)&0xFFFF));
				bi=bi.shiftLeft(16);
				bi=bi.or(BigInteger.valueOf(((tagHigh)&0xFFFF)));
				bi=bi.shiftLeft(16);
			} else {
				bi=BigInteger.ZERO;
			}
			bi=bi.or(BigInteger.valueOf(((tagLow>>16)&0xFFFF)));
			bi=bi.shiftLeft(16);
			bi=bi.or(BigInteger.valueOf(((tagLow)&0xFFFF)));
			return bi;
		}
		
		private static BigInteger[] EmptyTags=new BigInteger[0];
		
		/**
		 * Gets a list of all tags, from outermost to innermost.
		 */
		public BigInteger[] GetTags() {
			if(!this.isTagged())return EmptyTags;
			CBORObject curitem=this;
			if(curitem.isTagged()){
				ArrayList<BigInteger> list=new ArrayList<BigInteger>();
				while(curitem.isTagged()){
					list.add(LowHighToBigInteger(
						curitem.tagLow,curitem.tagHigh));
					curitem=((CBORObject)curitem.item_);
				}
				return list.toArray(new BigInteger[]{});
			} else {
				return new BigInteger[]{LowHighToBigInteger(tagLow,tagHigh)};
			}
		}
		
		/**
		 * Gets the last defined tag for this CBOR data item, or 0 if the item is
		 * untagged.
		 */
		public BigInteger getInnermostTag() {
				if(!this.isTagged())return BigInteger.ZERO;
				CBORObject previtem=this;
				CBORObject curitem=((CBORObject)item_);
				while(curitem.isTagged()){
					previtem=curitem;
					curitem=((CBORObject)curitem.item_);
				}
				if(previtem.tagHigh==0 &&
				   previtem.tagLow>=0 &&
				   previtem.tagLow<0x10000){
					return BigInteger.valueOf(previtem.tagLow);
				}
				return LowHighToBigInteger(
					previtem.tagLow,
					previtem.tagHigh);
			}

		@SuppressWarnings("unchecked")
private Map<CBORObject,CBORObject> AsMap() {
			return (Map<CBORObject,CBORObject>)this.getThisItem();
		}
		
		@SuppressWarnings("unchecked")
private List<CBORObject> AsList() {
			return (List<CBORObject>)this.getThisItem();
		}
		
		/**
		 * Gets the value of a CBOR object by integer index in this array.
		 * @throws java.lang.IllegalStateException This object is not an
		 * array.
		 */
		public CBORObject get(int index) {
				if(this.getItemType()== CBORObjectType_Array){
					List<CBORObject> list=AsList();
					return list.get(index);
				} else {
					throw new IllegalStateException("Not an array");
				}
			}
			/**
			 * Sets the value of a CBOR object by integer index in this array.
			 * @throws java.lang.IllegalStateException This object is not an
			 * array.
			 * @throws java.lang.NullPointerException value is null (as opposed
			 * to CBORObject.Null).
			 */
public void set(int index, CBORObject value) {
				if(this.getItemType()== CBORObjectType_Array){
					if((value)==null)throw new NullPointerException("value");
					List<CBORObject> list=AsList();
					list.set(index,value);
				} else {
					throw new IllegalStateException("Not an array");
				}
			}

		/**
		 * Gets a collection of the keys of this CBOR object.
		 * @throws java.lang.IllegalStateException This object is not a map.
		 */
		public Collection<CBORObject> getKeys() {
				if(this.getItemType()== CBORObjectType_Map){
					Map<CBORObject,CBORObject> dict=AsMap();
					return dict.keySet();
				} else {
					throw new IllegalStateException("Not a map");
				}
			}
		
		/**
		 * Gets the value of a CBOR object in this map, using a CBOR object as the
		 * key.
		 * @throws java.lang.NullPointerException The key is null (as opposed
		 * to CBORObject.Null).
		 * @throws java.lang.IllegalStateException This object is not a map.
		 */
		public CBORObject get(CBORObject key) {
				if((key)==null)throw new NullPointerException("key");
				if(this.getItemType()== CBORObjectType_Map){
					Map<CBORObject,CBORObject> map=AsMap();
					if(!map.containsKey(key))
						return null;
					return map.get(key);
				} else {
					throw new IllegalStateException("Not a map");
				}
			}
			/**
			 * Sets the value of a CBOR object in this map, using a CBOR object as the
			 * key.
			 * @throws java.lang.NullPointerException The key or value is null (as
			 * opposed to CBORObject.Null).
			 * @throws java.lang.IllegalStateException This object is not a map.
			 */
public void set(CBORObject key, CBORObject value) {
				if((key)==null)throw new NullPointerException("key");
				if((value)==null)throw new NullPointerException("value");
				if(this.getItemType()== CBORObjectType_Map){
					Map<CBORObject,CBORObject> map=AsMap();
					map.put(key,value);
				} else {
					throw new IllegalStateException("Not a map");
				}
			}
		
		/**
		 * Gets the value of a CBOR object in this map, using a string as the key.
		 * @throws java.lang.NullPointerException The key is null.
		 * @throws java.lang.IllegalStateException This object is not a map.
		 */
		public CBORObject get(String key) {
				if((key)==null)throw new NullPointerException("key");
				CBORObject objkey=CBORObject.FromObject(key);
				return this.get(objkey);
			}
			/**
			 * Sets the value of a CBOR object in this map, using a string as the key.
			 * @throws java.lang.NullPointerException The key or value is null (as
			 * opposed to CBORObject.Null).
			 * @throws java.lang.IllegalStateException This object is not a map.
			 */
public void set(String key, CBORObject value) {
				if((key)==null)throw new NullPointerException("key");
				if((value)==null)throw new NullPointerException("value");
				CBORObject objkey=CBORObject.FromObject(key);
				if(this.getItemType()== CBORObjectType_Map){
					Map<CBORObject,CBORObject> map=AsMap();
					map.put(objkey,value);
				} else {
					throw new IllegalStateException("Not a map");
				}
			}

		/**
		 * Returns the simple value ID of this object, or -1 if this object is not
		 * a simple value (including if the value is a floating-point number).
		 */
		public int getSimpleValue() {
				if(this.getItemType()== CBORObjectType_SimpleValue){
					return ((Integer)this.getThisItem()).intValue();
				} else {
					return -1;
				}
			}
		
		/**
		 * Adds a new object to the end of this array.
		 * @param obj A CBOR object.
		 * @throws java.lang.NullPointerException key or value is null (as opposed
		 * to CBORObject.Null).
		 * @throws java.lang.IllegalArgumentException key already exists in this map.
		 * @throws IllegalStateException This object is not a map.
		 */
		public void Add(CBORObject key, CBORObject value) {
			if((key)==null)throw new NullPointerException("key");
			if((value)==null)throw new NullPointerException("value");
			if(this.getItemType()== CBORObjectType_Map){
				Map<CBORObject,CBORObject> map=AsMap();
				if(map.containsKey(key))
					throw new IllegalArgumentException("Key already exists.");
				map.put(key,value);
			} else {
				throw new IllegalStateException("Not a map");
			}
		}

		/**
		 * Determines whether a value of the given key exists in this object.
		 * @param key An object that serves as the key.
		 * @return True if the given key is found, or false if the given key is not
		 * found or this object is not a map.
		 * @throws java.lang.NullPointerException key is null (as opposed to
		 * CBORObject.Null).
		 */
		public boolean ContainsKey(CBORObject key) {
			if((key)==null)throw new NullPointerException("key");
			if(this.getItemType()== CBORObjectType_Map){
				Map<CBORObject,CBORObject> map=AsMap();
				return map.containsKey(key);
			} else {
				return false;
			}
		}

		/**
		 * Adds a new object to the end of this array.
		 * @param obj A CBOR object.
		 * @throws java.lang.IllegalStateException This object is not an
		 * array.
		 * @throws java.lang.NullPointerException obj is null (as opposed to
		 * CBORObject.Null).
		 */
		public void Add(CBORObject obj) {
			if((obj)==null)throw new NullPointerException("obj");
			if(this.getItemType()== CBORObjectType_Array){
				List<CBORObject> list=AsList();
				list.add(obj);
			} else {
				throw new IllegalStateException("Not an array");
			}
		}
		
		/**
		 * If this object is an array, removes the first instance of the specified
		 * item from the array. If this object is a map, removes the item with the
		 * given key from the map.
		 * @param obj The item or key to remove.
		 * @return True if the item was removed; otherwise, false.
		 * @throws java.lang.NullPointerException obj is null (as opposed to
		 * CBORObject.Null).
		 * @throws java.lang.IllegalStateException The object is not an array
		 * or map.
		 */
		public boolean Remove(CBORObject obj) {
			if((obj)==null)throw new NullPointerException("obj");
			if(this.getItemType()== CBORObjectType_Map){
				Map<CBORObject,CBORObject> dict=AsMap();
				boolean hasKey=dict.containsKey(obj);
				if(hasKey){
					dict.remove(obj);
					return true;
				}
				return false;
			} else if(this.getItemType()== CBORObjectType_Array){
				List<CBORObject> list=AsList();
				return list.remove(obj);
			} else {
				throw new IllegalStateException("Not a map or array");
			}
		}

		/**
		 * Converts this object to a 64-bit floating point number.
		 * @return The closest 64-bit floating point number to this object.
		 * @throws java.lang.IllegalStateException This object's type is
		 * not a number type.
		 */
		public double AsDouble() {
			if(this.getItemType()== CBORObjectType_Integer)
				return ((Long)this.getThisItem()).doubleValue();
			else if(this.getItemType()== CBORObjectType_BigInteger)
				return ((BigInteger)this.getThisItem()).doubleValue();
			else if(this.getItemType()== CBORObjectType_Single)
				return ((Float)this.getThisItem()).doubleValue();
			else if(this.getItemType()== CBORObjectType_Double)
				return ((Double)this.getThisItem()).doubleValue();
			else if(this.getItemType()==CBORObjectType_DecimalFraction){
				String decstring=((DecimalFraction)this.getThisItem()).toString();
				return Double.parseDouble(decstring);
			}
			else
				throw new IllegalStateException("Not a number type");
		}
		
		
		/**
		 * Converts this object to a decimal fraction.
		 * @return A decimal fraction for this object's value.
		 * @throws java.lang.IllegalStateException This object's type is
		 * not a number type.
		 */
		public DecimalFraction AsDecimalFraction() {
			if(this.getItemType()== CBORObjectType_Integer)
				return new DecimalFraction(((Long)this.getThisItem()).longValue());
			else if(this.getItemType()== CBORObjectType_BigInteger)
				return new DecimalFraction((BigInteger)this.getThisItem());
			else if(this.getItemType()== CBORObjectType_Single)
				throw new UnsupportedOperationException();
			else if(this.getItemType()== CBORObjectType_Double)
				throw new UnsupportedOperationException();
			else if(this.getItemType()== CBORObjectType_DecimalFraction){
				return (DecimalFraction)this.getThisItem();
			}
			else
				throw new IllegalStateException("Not a number type");
		}

		/**
		 * Converts this object to a 32-bit floating point number.
		 * @return The closest 32-bit floating point number to this object.
		 * @throws java.lang.IllegalStateException This object's type is
		 * not a number type.
		 */
		public float AsSingle() {
			if(this.getItemType()== CBORObjectType_Integer)
				return ((Long)this.getThisItem()).floatValue();
			else if(this.getItemType()== CBORObjectType_BigInteger)
				return ((BigInteger)this.getThisItem()).floatValue();
			else if(this.getItemType()== CBORObjectType_Single)
				return ((Float)this.getThisItem()).floatValue();
			else if(this.getItemType()== CBORObjectType_Double)
				return ((Double)this.getThisItem()).floatValue();
			else if(this.getItemType()== CBORObjectType_DecimalFraction){
				String decstring=((DecimalFraction)this.getThisItem()).toString();
				return Float.parseFloat(decstring);
			}
			else
				throw new IllegalStateException("Not a number type");
		}

		/**
		 * Converts this object to an arbitrary-precision integer. Floating
		 * point values are truncated to an integer.
		 * @return The closest big integer to this object.
		 * @throws java.lang.IllegalStateException This object's type is
		 * not a number type.
		 */
		public BigInteger AsBigInteger() {
			if(this.getItemType()== CBORObjectType_Integer)
				return BigInteger.valueOf(((Long)this.getThisItem()).longValue());
			else if(this.getItemType()== CBORObjectType_BigInteger)
				return (BigInteger)this.getThisItem();
			else if(this.getItemType()== CBORObjectType_Single)
				return new BigDecimal(((Float)this.getThisItem()).floatValue()).toBigInteger();
			else if(this.getItemType()== CBORObjectType_Double)
				return new BigDecimal(((Double)this.getThisItem()).doubleValue()).toBigInteger();
			else if(this.getItemType()== CBORObjectType_DecimalFraction){
				return ParseBigIntegerWithExponent(((DecimalFraction)this.getThisItem()).toString());
			}
			else
				throw new IllegalStateException("Not a number type");
		}
		
		/**
		 * Returns false if this object is False, Null, or Undefined; otherwise,
		 * true.
		 */
		public boolean AsBoolean() {
			if(this.isFalse() || this.isNull() || this.isUndefined())
				return false;
			return true;
		}
		
		/**
		 * Converts this object to a 16-bit signed integer. Floating point values
		 * are truncated to an integer.
		 * @return The closest 16-bit signed integer to this object.
		 * @throws java.lang.IllegalStateException This object's type is
		 * not a number type.
		 * @throws java.lang.ArithmeticException This object's value exceeds
		 * the range of a 16-bit signed integer.
		 */
		public short AsInt16() {
			int v=AsInt32();
			if(v>Short.MAX_VALUE || v<0)
				throw new ArithmeticException();
			return (short)v;
		}
		
		/**
		 * Converts this object to a byte. Floating point values are truncated
		 * to an integer.
		 * @return The closest byte-sized integer to this object.
		 * @throws java.lang.IllegalStateException This object's type is
		 * not a number type.
		 * @throws java.lang.ArithmeticException This object's value exceeds
		 * the range of a byte (is less than 0 or would be greater than 255 when truncated
		 * to an integer).
		 */
		public byte AsByte() {
			int v=AsInt32();
			if(v<0 || v>255)
				throw new ArithmeticException();
			return (byte)v;
		}

		private static boolean IsValidString(String str) {
			if(str==null)return false;
			for(int i=0;i<str.length();i++){
				int c=str.charAt(i);
				if(c<=0xD7FF || c>=0xE000) {
					continue;
				} else if(c<=0xDBFF){ // UTF-16 low surrogate
					i++;
					if(i>=str.length() || str.charAt(i)<0xDC00 || str.charAt(i)>0xDFFF)
						return false;
				} else
					return false;
			}
			return true;
		}

		
		/**
		 * Converts this object to a 64-bit signed integer. Floating point values
		 * are truncated to an integer.
		 * @return The closest 64-bit signed integer to this object.
		 * @throws java.lang.IllegalStateException This object's type is
		 * not a number type.
		 * @throws java.lang.ArithmeticException This object's value exceeds
		 * the range of a 64-bit signed integer.
		 */
		public long AsInt64() {
			if(this.getItemType()== CBORObjectType_Integer){
				return ((Long)this.getThisItem()).longValue();
			} else if(this.getItemType()== CBORObjectType_BigInteger){
				if(((BigInteger)this.getThisItem()).compareTo(Int64MaxValue)>0 ||
				   ((BigInteger)this.getThisItem()).compareTo(Int64MinValue)<0)
					throw new ArithmeticException();
				return ((BigInteger)this.getThisItem()).longValue();
			} else if(this.getItemType()== CBORObjectType_Single){
				if(Float.isNaN(((Float)this.getThisItem()).floatValue()) ||
				   ((Float)this.getThisItem()).floatValue()>Long.MAX_VALUE || ((Float)this.getThisItem()).floatValue()<Long.MIN_VALUE)
					throw new ArithmeticException();
				return ((Float)this.getThisItem()).longValue();
			} else if(this.getItemType()== CBORObjectType_Double){
				if(Double.isNaN(((Double)this.getThisItem()).doubleValue()) ||
				   ((Double)this.getThisItem()).doubleValue()>Long.MAX_VALUE || ((Double)this.getThisItem()).doubleValue()<Long.MIN_VALUE)
					throw new ArithmeticException();
				return ((Double)this.getThisItem()).longValue();
			} else if(this.getItemType()== CBORObjectType_DecimalFraction){
				BigInteger bi=ParseBigIntegerWithExponent(((DecimalFraction)this.getThisItem()).toString());
				if(bi.compareTo(Int64MaxValue)>0 ||
				   bi.compareTo(Int64MinValue)<0)
					throw new ArithmeticException();
				return bi.longValue();
			} else
				throw new IllegalStateException("Not a number type");
		}
		
		/**
		 * Converts this object to a 32-bit signed integer. Floating point values
		 * are truncated to an integer.
		 * @return The closest big integer to this object.
		 * @throws java.lang.IllegalStateException This object's type is
		 * not a number type.
		 * @throws java.lang.ArithmeticException This object's value exceeds
		 * the range of a 32-bit signed integer.
		 */
		public int AsInt32() {
			Object thisItem=this.getThisItem();
			if(this.getItemType()== CBORObjectType_Integer){
				if(((Long)thisItem).longValue()>Integer.MAX_VALUE || ((Long)thisItem).longValue()<Integer.MIN_VALUE)
					throw new ArithmeticException();
				return ((Long)thisItem).intValue();
			} else if(this.getItemType()== CBORObjectType_BigInteger){
				if(((BigInteger)thisItem).compareTo(BigInteger.valueOf(Integer.MAX_VALUE))>0 ||
				   ((BigInteger)thisItem).compareTo(BigInteger.valueOf(Integer.MIN_VALUE))<0)
					throw new ArithmeticException();
				return ((BigInteger)thisItem).intValue();
			} else if(this.getItemType()== CBORObjectType_Single){
				if(Float.isNaN(((Float)thisItem).floatValue()) ||
				   ((Float)thisItem).floatValue()>Integer.MAX_VALUE || ((Float)thisItem).floatValue()<Integer.MIN_VALUE)
					throw new ArithmeticException();
				return ((Float)thisItem).intValue();
			} else if(this.getItemType()== CBORObjectType_Double){
				if(Double.isNaN(((Double)thisItem).doubleValue()) ||
				   ((Double)thisItem).doubleValue()>Integer.MAX_VALUE || ((Double)thisItem).doubleValue()<Integer.MIN_VALUE)
					throw new ArithmeticException();
				return ((Double)thisItem).intValue();
			} else if(this.getItemType()== CBORObjectType_DecimalFraction){
				BigInteger bi=ParseBigIntegerWithExponent(((DecimalFraction)this.getThisItem()).toString());
				if(bi.compareTo(BigInteger.valueOf(Integer.MAX_VALUE))>0 ||
				   bi.compareTo(BigInteger.valueOf(Integer.MIN_VALUE))<0)
					throw new ArithmeticException();
				return bi.intValue();
			} else
				throw new IllegalStateException("Not a number type");
		}
		/**
		 * Gets the value of this object as a string object.
		 * @return Gets this object's string.
		 * @throws IllegalStateException This object's type is not a string.
		 */
		public String AsString() {
			if(this.getItemType()== CBORObjectType_TextString){
				return (String)this.getThisItem();
			} else {
				throw new IllegalStateException("Not a String type");
			}
		}
		/**
		 * Reads an object in CBOR format from a data stream.
		 * @param stream A readable data stream.
		 * @return a CBOR object that was read.
		 * @throws java.lang.NullPointerException "stream" is null.
		 * @throws CBORException There was an error in reading or parsing the
		 * data.
		 */
		public static CBORObject Read(InputStream stream) {
			try {
				return Read(stream,0,false,-1,null,0);
			} catch(IOException ex){
				throw new CBORException("I/O error occurred.",ex);
			}
		}
		
		private static void WriteObjectArray(
			List<CBORObject> list, OutputStream s) throws IOException {
			WritePositiveInt(4,list.size(),s);
			for(CBORObject i : list){
				Write(i,s);
			}
		}
		
		private static void WriteObjectMap(
			Map<CBORObject,CBORObject> map, OutputStream s) throws IOException {
			WritePositiveInt(5,map.size(),s);
			for(Map.Entry<CBORObject,CBORObject> entry : map.entrySet()){
				CBORObject key=entry.getKey();
				CBORObject value=entry.getValue();
				Write(key,s);
				Write(value,s);
			}
		}

		private static byte[] GetPositiveIntBytes(int type, int value) {
			if((value)<0)throw new IllegalArgumentException("value"+" not greater or equal to "+"0"+" ("+Integer.toString((int)value)+")");
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

		private static void WritePositiveInt(int type, int value, OutputStream s) throws IOException {
			byte[] bytes=GetPositiveIntBytes(type,value);
			s.write(bytes,0,bytes.length);
		}
		
		private static byte[] GetPositiveInt64Bytes(int type, long value) {
			if((value)<0)throw new IllegalArgumentException("value"+" not greater or equal to "+"0"+" ("+Long.toString((long)value)+")");
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
		private static void WritePositiveInt64(int type, long value, OutputStream s) throws IOException {
			byte[] bytes=GetPositiveInt64Bytes(type,value);
			s.write(bytes,0,bytes.length);
		}
		
		private static final int StreamedStringBufferLength=4096;
		
		private static void WriteStreamedString(String str, OutputStream stream) throws IOException {
			byte[] bytes;
			bytes=GetOptimizedBytesIfShortAscii(str,-1);
			if(bytes!=null){
				stream.write(bytes,0,bytes.length);
				return;
			}
			bytes=new byte[StreamedStringBufferLength];
			int byteIndex=0;
			boolean streaming=false;
			for(int index=0;index<str.length();index++){
				int c=str.charAt(index);
				if(c<=0x7F){
					if(byteIndex+1>StreamedStringBufferLength){
						// Write bytes retrieved so far
						if(!streaming)
							stream.write((byte)0x7F);
						WritePositiveInt(3,byteIndex,stream);
						stream.write(bytes,0,byteIndex);
						byteIndex=0;
						streaming=true;
					}
					bytes[byteIndex++]=(byte)c;
				} else if(c<=0x7FF){
					if(byteIndex+2>StreamedStringBufferLength){
						// Write bytes retrieved so far
						if(!streaming)
							stream.write((byte)0x7F);
						WritePositiveInt(3,byteIndex,stream);
						stream.write(bytes,0,byteIndex);
						byteIndex=0;
						streaming=true;
					}
					bytes[byteIndex++]=((byte)(0xC0|((c>>6)&0x1F)));
					bytes[byteIndex++]=((byte)(0x80|(c   &0x3F)));
				} else {
					if(c>=0xD800 && c<=0xDBFF && index+1<str.length() &&
					   str.charAt(index+1)>=0xDC00 && str.charAt(index+1)<=0xDFFF){
						// Get the Unicode code point for the surrogate pair
						c=0x10000+(c-0xD800)*0x400+(str.charAt(index+1)-0xDC00);
						index++;
					} else if(c>=0xD800 && c<=0xDFFF){
						// unpaired surrogate, write U+FFFD instead
						c=0xFFFD;
					}
					if(c<=0xFFFF){
						if(byteIndex+3>StreamedStringBufferLength){
							// Write bytes retrieved so far
							if(!streaming)
								stream.write((byte)0x7F);
							WritePositiveInt(3,byteIndex,stream);
							stream.write(bytes,0,byteIndex);
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
								stream.write((byte)0x7F);
							WritePositiveInt(3,byteIndex,stream);
							stream.write(bytes,0,byteIndex);
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
			stream.write(bytes,0,byteIndex);
			if(streaming){
				stream.write((byte)0xFF);
			}
		}

		
		/**
		 * Writes a string in CBOR format to a data stream.
		 * @param str The string to write. Can be null.
		 * @param s A writable data stream.
		 * @throws java.lang.NullPointerException stream is null.
		 * @throws java.io.IOException An I/O error occurred.
		 */
		public static void Write(String str, OutputStream stream) throws IOException {
			if((stream)==null)throw new NullPointerException("stream");
			if(str==null){
				stream.write(0xf6); // Write null instead of String
			} else {
				WriteStreamedString(str,stream);
			}
		}
		
		
		private static BigInteger LowestMajorType1=
			new BigInteger("-18446744073709551616");
		private static BigInteger UInt64MaxValue=
			new BigInteger("18446744073709551615");
		private static BigInteger FiftySixBitMask=BigInteger.valueOf(0xFFFFFFFFFFFFFFL);
		
		/**
		 * Writes a big integer in CBOR format to a data stream.
		 * @param bi Big integer to write.
		 * @param s Stream to write to.
		 */
		public static void Write(BigInteger bigint, OutputStream s) throws IOException {
			if((s)==null)throw new NullPointerException("s");
			int datatype=0;
			if(bigint.signum()<0){
				datatype=1;
				bigint=bigint.add(BigInteger.ONE);
				bigint=(bigint).negate();
			}
			if(bigint.compareTo(Int64MaxValue)<=0){
				// If the big integer is representable in
				// major type 0 or 1, write that major type
				// instead of as a bignum
				long ui=bigint.longValue();
				WritePositiveInt64(datatype,ui,s);
			} else {
				java.io.ByteArrayOutputStream ms=null;
try {
ms=new ByteArrayOutputStream();

					long tmp=0;
					byte[] buffer=new byte[10];
					while(bigint.signum()>0){
						// To reduce the number of big integer
						// operations, extract the big int 56 bits at a time
						// (not 64, to avoid negative numbers)
						BigInteger tmpbigint=bigint.and(FiftySixBitMask);
						tmp=tmpbigint.longValue();
						bigint=bigint.shiftRight(56);
						boolean isNowZero=(bigint.equals(BigInteger.ZERO));
						int bufferindex=0;
						for(int i=0;i<7 && (!isNowZero || tmp>0);i++){
							buffer[bufferindex]=(byte)(tmp&0xFF);
							tmp>>=8;
							bufferindex++;
						}
						ms.write(buffer,0,bufferindex);
					}
					byte[] bytes=ms.toByteArray();
					switch(bytes.length){
						case 1: // Fits in 1 byte (won't normally happen though)
							buffer[0]=(byte)((datatype<<5)|24);
							buffer[1]=bytes[0];
							s.write(buffer,0,2);
							break;
						case 2: // Fits in 2 bytes (won't normally happen though)
							buffer[0]=(byte)((datatype<<5)|25);
							buffer[1]=bytes[1];
							buffer[2]=bytes[0];
							s.write(buffer,0,3);
							break;
						case 3:
						case 4:
							buffer[0]=(byte)((datatype<<5)|26);
							buffer[1]=(bytes.length>3) ? bytes[3] : (byte)0;
							buffer[2]=bytes[2];
							buffer[3]=bytes[1];
							buffer[4]=bytes[0];
							s.write(buffer,0,5);
							break;
						case 5:
						case 6:
						case 7:
						case 8:
							buffer[0]=(byte)((datatype<<5)|27);
							buffer[1]=(bytes.length>7) ? bytes[7] : (byte)0;
							buffer[2]=(bytes.length>6) ? bytes[6] : (byte)0;
							buffer[3]=(bytes.length>5) ? bytes[5] : (byte)0;
							buffer[4]=bytes[4];
							buffer[5]=bytes[3];
							buffer[6]=bytes[2];
							buffer[7]=bytes[1];
							buffer[8]=bytes[0];
							s.write(buffer,0,9);
							break;
						default:
							s.write((datatype==0) ?
							            (byte)0xC2 :
							            (byte)0xC3);
							WritePositiveInt(2,bytes.length,s);
							for(int i=bytes.length-1;i>=0;i--){
								s.write(bytes[i]);
							}
							break;
					}
}
finally {
try { if(ms!=null)ms.close(); } catch(IOException ex){}
}
			}
		}
		
		/**
		 * Writes this CBOR object to a data stream.
		 * @param s A writable data stream.
		 * @throws java.lang.NullPointerException s is null.
		 * @throws java.io.IOException An I/O error occurred.
		 */
		public void WriteTo(OutputStream s) throws IOException {
			if((s)==null)throw new NullPointerException("s");
			WriteTags(s);
			if(this.getItemType()==CBORObjectType_Integer){
				Write(((Long)this.getThisItem()).longValue(),s);
			} else if(this.getItemType()== CBORObjectType_BigInteger){
				Write((BigInteger)this.getThisItem(),s);
			} else if(this.getItemType()== CBORObjectType_ByteString){
				byte[] arr=(byte[])this.getThisItem();
				WritePositiveInt((this.getItemType()== CBORObjectType_ByteString) ? 2 : 3,
				                 arr.length,s);
				s.write(arr,0,arr.length);
			} else if(this.getItemType()== CBORObjectType_TextString ){
				Write(this.AsString(),s);
			} else if(this.getItemType()== CBORObjectType_Array){
				WriteObjectArray(AsList(),s);
			} else if(this.getItemType()== CBORObjectType_DecimalFraction){
				DecimalFraction dec=(DecimalFraction)this.getThisItem();
				BigInteger exponent=dec.getExponent();
				if(exponent.equals(BigInteger.ZERO)){
					Write(dec.getMantissa(),s);
				} else {
					s.write(0xC4); // tag 4
					s.write(0x82); // array, length 2
					Write(dec.getExponent(),s);
					Write(dec.getMantissa(),s);
				}
			} else if(this.getItemType()== CBORObjectType_Map){
				WriteObjectMap(AsMap(),s);
			} else if(this.getItemType()== CBORObjectType_SimpleValue){
				int value=((Integer)this.getThisItem()).intValue();
				if(value<24){
					s.write((byte)(0xE0+value));
				} else {
					s.write(0xF8);
					s.write((byte)value);
				}
			} else if(this.getItemType()== CBORObjectType_Single){
				Write(((Float)this.getThisItem()).floatValue(),s);
			} else if(this.getItemType()== CBORObjectType_Double){
				Write(((Double)this.getThisItem()).doubleValue(),s);
			} else {
				throw new IllegalArgumentException("Unexpected data type");
			}
		}
		
		/**
		 * Writes a 64-bit unsigned integer in CBOR format to a data stream.
		 * @param value The value to write
		 * @param s A writable data stream.
		 * @throws java.lang.NullPointerException s is null.
		 * @throws java.io.IOException An I/O error occurred.
		 */
		public static void Write(long value, OutputStream s) throws IOException {
			if((s)==null)throw new NullPointerException("s");
			if(value>=0){
				WritePositiveInt64(0,value,s);
			} else {
				value+=1;
				value=-value; // Will never overflow
				WritePositiveInt64(1,value,s);
			}
		}
		/**
		 * Writes a 32-bit signed integer in CBOR format to a data stream.
		 * @param value The value to write
		 * @param s A writable data stream.
		 * @throws java.lang.NullPointerException s is null.
		 * @throws java.io.IOException An I/O error occurred.
		 */
		public static void Write(int value, OutputStream s) throws IOException {
			Write((long)value,s);
		}
		/**
		 * Writes a 16-bit signed integer in CBOR format to a data stream.
		 * @param value The value to write
		 * @param s A writable data stream.
		 * @throws java.lang.NullPointerException s is null.
		 * @throws java.io.IOException An I/O error occurred.
		 */
		public static void Write(short value, OutputStream s) throws IOException {
			Write((long)value,s);
		}
		/**
		 * Writes a Unicode character as a string in CBOR format to a data stream.
		 * @param value The value to write
		 * @param s A writable data stream.
		 * @throws java.lang.NullPointerException s is null.
		 * @throws java.io.IOException An I/O error occurred.
		 */
		public static void Write(char value, OutputStream s) throws IOException {
			Write(new String(new char[]{value}),s);
		}
		/**
		 * Writes a Boolean value in CBOR format to a data stream.
		 * @param value The value to write
		 * @param s A writable data stream.
		 * @throws java.lang.NullPointerException s is null.
		 * @throws java.io.IOException An I/O error occurred.
		 */
		public static void Write(boolean value, OutputStream s) throws IOException {
			if((s)==null)throw new NullPointerException("s");
			s.write(value ? (byte)0xf5 : (byte)0xf4);
		}
		/**
		 * Writes a byte in CBOR format to a data stream.
		 * @param value The value to write
		 * @param s A writable data stream.
		 * @throws java.lang.NullPointerException s is null.
		 * @throws java.io.IOException An I/O error occurred.
		 */
		public static void Write(byte value, OutputStream s) throws IOException {
			if((s)==null)throw new NullPointerException("s");
			if((((int)value)&0xFF)<24){
				s.write(value);
			} else {
				s.write((byte)(24));
				s.write(value);
			}
		}
		/**
		 * Writes a 32-bit floating-point number in CBOR format to a data stream.
		 * @param value The value to write
		 * @param s A writable data stream.
		 * @throws java.lang.NullPointerException s is null.
		 * @throws java.io.IOException An I/O error occurred.
		 */
		public static void Write(float value, OutputStream s) throws IOException {
			if((s)==null)throw new NullPointerException("s");
			int bits=Float.floatToRawIntBits(
				value);
			byte[] data=new byte[]{(byte)0xFA,
				(byte)((bits>>24)&0xFF),
				(byte)((bits>>16)&0xFF),
				(byte)((bits>>8)&0xFF),
				(byte)(bits&0xFF)};
			s.write(data,0,5);
		}
		/**
		 * Writes a 64-bit floating-point number in CBOR format to a data stream.
		 * @param value The value to write
		 * @param s A writable data stream.
		 * @throws java.lang.NullPointerException s is null.
		 * @throws java.io.IOException An I/O error occurred.
		 */
		public static void Write(double value, OutputStream s) throws IOException {
			long bits=Double.doubleToRawLongBits(
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
			s.write(data,0,9);
		}
		
		private static byte[] GetOptimizedBytesIfShortAscii(String str, int tagbyte) {
			byte[] bytes;
			if(str.length()<=255){
				// The strings will usually be short ASCII strings, so
				// use this optimization
				int offset=0;
				int length=str.length();
				int extra=(length<24) ? 1 : 2;
				if(tagbyte>=0)extra++;
				bytes=new byte[length+extra];
				if(tagbyte>=0){
					bytes[offset]=((byte)(tagbyte));
					offset++;
				}
				if(length<24){
					bytes[offset]=((byte)(0x60+str.length()));
					offset++;
				} else {
					bytes[offset]=((byte)(0x78));
					bytes[offset+1]=((byte)(str.length()));
					offset+=2;
				}
				boolean issimple=true;
				for(int i=0;i<str.length();i++){
					char c=str.charAt(i);
					if(c>=0x80){
						issimple=false;
						break;
					}
					bytes[i+offset]=((byte)c);
				}
				if(issimple){
					return bytes;
				}
			}
			return null;
		}
		
		/**
		 * Gets the binary representation of this data item.
		 * @return A byte array in CBOR format.
		 */
		public byte[] EncodeToBytes() {
			// For some types, a memory stream is a lot of
			// overhead since the amount of memory the types
			// use is fixed and small
			boolean hasComplexTag=false;
			byte tagbyte=0;
			boolean tagged=this.isTagged();
			if(this.isTagged()){
				CBORObject taggedItem=(CBORObject)item_;
				if(taggedItem.isTagged() ||
				   this.tagHigh!=0 ||
				   ((this.tagLow)>>16)!=0 ||
				   this.tagLow>=24){
					hasComplexTag=true;
				} else {
					tagbyte=(byte)(0xC0+(int)this.tagLow);
				}
			}
			if(!hasComplexTag){
				if(this.getItemType()== CBORObjectType_TextString){
					byte[] ret=GetOptimizedBytesIfShortAscii(
						this.AsString(),
						tagged ? (((int)tagbyte)&0xFF) : -1);
					if(ret!=null)return ret;
				} else if(this.getItemType()== CBORObjectType_Integer){
					long value=((Long)this.getThisItem()).longValue();
					byte[] ret=null;
					if(value>=0){
						ret=GetPositiveInt64Bytes(0,value);
					} else {
						value+=1;
						value=-value; // Will never overflow
						ret=GetPositiveInt64Bytes(1,value);
					}
					if(!tagged)return ret;
					byte[] ret2=new byte[ret.length+1];
					System.arraycopy(ret,0,ret2,1,ret.length);
					ret2[0]=tagbyte;
					return ret2;
				} else if(this.getItemType()==CBORObjectType_Single){
					int bits=Float.floatToRawIntBits(
						((Float)this.getThisItem()).floatValue());
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
				} else if(this.getItemType()==CBORObjectType_Double){
					long bits=Double.doubleToRawLongBits(
						((Double)this.getThisItem()).doubleValue());
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
				java.io.ByteArrayOutputStream ms=null;
try {
ms=new ByteArrayOutputStream();

					WriteTo(ms);
					return ms.toByteArray();
}
finally {
try { if(ms!=null)ms.close(); } catch(IOException ex){}
}
			} catch(IOException ex){
				throw new CBORException("I/O Error occurred",ex);
			}
		}
		
		/**
		 * Writes an arbitrary object to a CBOR data stream.
		 * @param value The value to write
		 * @param s A writable data stream.
		 */
		@SuppressWarnings("unchecked")
public static void Write(Object o, OutputStream s) throws IOException {
			if((s)==null)throw new NullPointerException("s");
			if(o==null){
				s.write(0xf6);
			} else if(o instanceof byte[]){
				byte[] data=(byte[])o;
				WritePositiveInt(3,data.length,s);
				s.write(data,0,data.length);
			} else if(o instanceof List<?>){
				WriteObjectArray((List<CBORObject>)o,s);
			} else if(o instanceof Map<?,?>){
				WriteObjectMap((Map<CBORObject,CBORObject>)o,s);
			} else {
				FromObject(o).WriteTo(s);
			}
		}
		
		/**
		 * Generates a CBOR object from a string in JavaScript Object Notation
		 * (JSON) format. This function only accepts maps and arrays.
		 * @param str A string in JSON format.
		 */
		public static CBORObject FromJSONString(String str) {
			JSONTokener tokener=new JSONTokener(str,0);
			CBORObject obj=ParseJSONObject(tokener);
			if(tokener.nextClean()!=-1)
				throw tokener.syntaxError("End of String not reached");
			return obj;
		}
		
		private static String Base64URL="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
		private static String Base64="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
		private static void ToBase64(StringBuilder str, byte[] data, String alphabet, boolean padding) {
			int length = data.length;
			int i=0;
			for (i = 0; i < (length - 2); i += 3) {
				str.append(alphabet.charAt((data[i] >> 2)&63));
				str.append(alphabet.charAt(((data[i] & 3) << 4) + ((data[i+1] >> 4)&63)));
				str.append(alphabet.charAt(((data[i+1] & 15) << 2) + ((data[i+2] >> 6)&3)));
				str.append(alphabet.charAt(data[i+2] & 63));
			}
			int lenmod3=(length%3);
			if (lenmod3!=0) {
				i = length - lenmod3;
				str.append(alphabet.charAt((data[i] >> 2)&63));
				if (lenmod3 == 2) {
					str.append(alphabet.charAt(((data[i] & 3) << 4) + ((data[i+1] >> 4)&63)));
					str.append(alphabet.charAt((data[i+1] & 15) << 2));
					if(padding)str.append("=");
				} else {
					str.append(alphabet.charAt((data[i] & 3) << 4));
					if(padding)str.append("==");
				}
			}
		}
		private static void ToBase16(StringBuilder str, byte[] data) {
			String alphabet="0123456789ABCDEF";
			int length = data.length;
			for (int i = 0; i < length;i++) {
				str.append(alphabet.charAt((data[i]>>4)&15));
				str.append(alphabet.charAt(data[i]&15));
			}
		}
		
		private static String StringToJSONString(String str) {
			StringBuilder sb=new StringBuilder();
			sb.append("\"");
			// Surrogates were already verified when this
			// String was added to the CBOR Object; that check
			// is not repeated here
			for(int i=0;i<str.length();i++){
				char c=str.charAt(i);
				if(c=='\\' || c=='"'){
					sb.append('\\');
				} else if(c<0x20){
					sb.append("\\u00");
					sb.append((char)('0'+(int)(c>>4)));
					sb.append((char)('0'+(int)(c&15)));
				}
				sb.append(c);
			}
			sb.append("\"");
			return sb.toString();
		}
		
		private BigInteger ParseBigIntegerWithExponent(String str) {
			return new BigDecimal(str).toBigInteger();
		}
		
		/**
		 * Converts this object to a JSON string. This function works not only
		 * with arrays and maps (the only proper JSON objects under RFC 4627),
		 * but also integers, strings, byte arrays, and other JSON data types.
		 */
		public String ToJSONString() {
			if(this.getItemType()== CBORObjectType_SimpleValue){
				if(this.isTrue())return "true";
				else if(this.isFalse())return "false";
				else if(this.isNull())return "null";
				else return "null";
			} else if(this.getItemType()== CBORObjectType_Single){
				float f=((Float)this.getThisItem()).floatValue();
				if(((f)==Float.NEGATIVE_INFINITY) ||
				   ((f)==Float.POSITIVE_INFINITY) ||
				   Float.isNaN(f)) return "null";
				else
					return TrimDotZero(Float.toString((float)f));
			} else if(this.getItemType()== CBORObjectType_Double){
				double f=((Double)this.getThisItem()).doubleValue();
				if(((f)==Double.NEGATIVE_INFINITY) ||
				   ((f)==Double.POSITIVE_INFINITY) ||
				   Double.isNaN(f)) return "null";
				else
					return TrimDotZero(Double.toString((double)f));
			} else if(this.getItemType()== CBORObjectType_Integer){
				return Long.toString(((Long)this.getThisItem()).longValue());
			} else if(this.getItemType()== CBORObjectType_BigInteger){
				return ((BigInteger)this.getThisItem()).toString();
			} else if(this.getItemType()== CBORObjectType_ByteString){
				StringBuilder sb=new StringBuilder();
				sb.append('\"');
				if(this.HasTag(22)){
					ToBase64(sb,(byte[])this.getThisItem(),Base64,false);
				} else if(this.HasTag(23)){
					ToBase16(sb,(byte[])this.getThisItem());
				} else {
					ToBase64(sb,(byte[])this.getThisItem(),Base64URL,false);
				}
				sb.append('\"');
				return sb.toString();
			} else if(this.getItemType()== CBORObjectType_TextString){
				return StringToJSONString(this.AsString());
			} else if(this.getItemType()== CBORObjectType_DecimalFraction){
				return ((DecimalFraction)this.getThisItem()).toString();
			} else if(this.getItemType()== CBORObjectType_Array){
				StringBuilder sb=new StringBuilder();
				boolean first=true;
				sb.append("[");
				for(CBORObject i : AsList()){
					if(!first)sb.append(",");
					sb.append(i.ToJSONString());
					first=false;
				}
				sb.append("]");
				return sb.toString();
			} else if(this.getItemType()== CBORObjectType_Map){
				StringBuilder builder=new StringBuilder();
				boolean first=true;
				boolean hasNonStringKeys=false;
				Map<CBORObject,CBORObject> objMap=AsMap();
				builder.append('{');
				for(Map.Entry<CBORObject,CBORObject> entry : objMap.entrySet()){
					CBORObject key=entry.getKey();
					CBORObject value=entry.getValue();
					if(key.getItemType()!=CBORObjectType_TextString){
						hasNonStringKeys=true;
						break;
					}
					if(!first)builder.append(",");
					builder.append(StringToJSONString(key.AsString()));
					builder.append(':');
					builder.append(value.ToJSONString());
					first=false;
				}
				if(hasNonStringKeys){
					builder.setLength(0);
					HashMap<String,CBORObject> sMap=new HashMap<String,CBORObject>();
					// Copy to a map with String keys, since 
					// some keys could be duplicates
					// when serialized to strings
					for(Map.Entry<CBORObject,CBORObject> entry : objMap.entrySet()){
						CBORObject key=entry.getKey();
						CBORObject value=entry.getValue();
						String str=(key.getItemType()== CBORObjectType_TextString) ?
							key.AsString() : key.ToJSONString();
						sMap.put(str,value);
					}
					first=true;
					for(Map.Entry<String,CBORObject> entry : sMap.entrySet()){
						String key=entry.getKey();
						CBORObject value=entry.getValue();
						sMap.put(key,value);
						if(!first)builder.append(",");
						builder.append(StringToJSONString(key));
						builder.append(':');
						builder.append(value.ToJSONString());
						first=false;
					}
				}
				builder.append('}');
				return builder.toString();
			} else {
				throw new IllegalStateException("Unexpected data type");
			}
		}
		
		
		// Based on the json.org implementation for JSONTokener
		private static CBORObject NextJSONValue(JSONTokener tokener) {
			int c = tokener.nextClean();
			String str;
			if (c == '"' || (c == '\'' && ((tokener.getOptions()&JSONTokener.OPTION_SINGLE_QUOTES)!=0))){
				// The tokenizer already checked the String for invalid
				// surrogate pairs, so just call the CBORObject
				// constructor directly
				return new CBORObject(CBORObjectType_TextString,
				                      tokener.nextString(c));
			}
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
				sb.append((char)c);
				c = tokener.next();
			}
			tokener.back();
			str = JSONTokener.trimSpaces(sb.toString());
			if (str.equals("true"))
				return CBORObject.True;
			if (str.equals("false"))
				return CBORObject.False;
			if (str.equals("null"))
				return CBORObject.Null;
			if ((b >= '0' && b <= '9') || b == '.' || b == '-' || b == '+') {
				CBORObject obj=CBORDataUtilities.ParseJSONNumber(str,false,false);
				if(obj==null)
					throw tokener.syntaxError("JSON number can't be parsed.");
				return obj;
			}
			if (str.length() == 0)
				throw tokener.syntaxError("Missing value.");
			// Value is unparseable
			throw tokener.syntaxError("Value can't be parsed.");
		}

		// Based on the json.org implementation for JSONObject
		private static CBORObject ParseJSONObject(JSONTokener x) {
			int c;
			CBORObject key;
			CBORObject obj;
			c=x.nextClean();
			if(c=='['){
				x.back();
				return ParseJSONArray(x);
			}
			HashMap<CBORObject,CBORObject> myHashMap=new HashMap<CBORObject,CBORObject>();
			if (c != '{')
				throw x.syntaxError("A JSONObject must begin with '{' or '['");
			while (true) {
				c = x.nextClean();
				switch (c) {
					case -1:
						throw x.syntaxError("A JSONObject must end with '}'");
					case '}':
						return new CBORObject(CBORObjectType_Map,myHashMap);
					default:
						x.back();
						obj=NextJSONValue(x);
						if(obj.getItemType()!= CBORObjectType_TextString)
							throw x.syntaxError("Expected a String as a key");
						key=obj;
						if((x.getOptions() & JSONTokener.OPTION_NO_DUPLICATES)!=0 &&
						   myHashMap.containsKey(obj)){
							throw x.syntaxError("Key already exists: "+key);
						}
						break;
				}
				
				if (x.nextClean() != ':')
					throw x.syntaxError("Expected a ':' after a key");
				// NOTE: Will overwrite existing value. --Peter O.
				myHashMap.put(key,NextJSONValue(x));
				switch (x.nextClean()) {
					case ',':
						if (x.nextClean() == '}'){
							if((x.getOptions() & JSONTokener.OPTION_TRAILING_COMMAS)==0){
								// 2013-05-24 -- Peter O. Disallow trailing comma.
								throw x.syntaxError("Trailing comma");
							} else {
								return new CBORObject(CBORObjectType_Map,myHashMap);
							}
						}
						x.back();
						break;
					case '}':
						return new CBORObject(CBORObjectType_Map,myHashMap);
					default:
						throw x.syntaxError("Expected a ',' or '}'");
				}
			}
		}

		// Based on the json.org implementation for JSONArray
		private static CBORObject ParseJSONArray(JSONTokener x) {
			ArrayList<CBORObject> myArrayList=new ArrayList<CBORObject>();
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
					myArrayList.add(CBORObject.Null);
				} else {
					x.back();
					myArrayList.add(NextJSONValue(x));
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

		
		/**
		 * Creates a new empty CBOR array.
		 * @return A new CBOR array.
		 */
		public static CBORObject NewArray() {
			return FromObject(new ArrayList<CBORObject>());
		}
		
		/**
		 * Creates a new empty CBOR map.
		 * @return A new CBOR map.
		 */
		public static CBORObject NewMap() {
			return FromObject(new HashMap<CBORObject,CBORObject>());
		}
		
		
		//-----------------------------------------------------------
		/**
		 * Generates a CBOR object from a 64-bit signed integer.
		 */
		public static CBORObject FromObject(long value) {
			return new CBORObject(CBORObjectType_Integer,value);
		}
		/**
		 * Generates a CBOR object from a CBOR object.
		 * @param A CBOR object.
		 * @return Same as "value", or CBORObject.Null if "value" is null.
		 */
		public static CBORObject FromObject(CBORObject value) {
			if(value==null)return CBORObject.Null;
			return value;
		}

		/**
		 * Generates a CBOR object from an arbitrary-precision integer.
		 * @param bigintValue An arbitrary-precision value.
		 * @return A CBOR number object.
		 */
		public static CBORObject FromObject(BigInteger bigintValue) {
			if(bigintValue.compareTo(Int64MinValue)>=0 &&
			   bigintValue.compareTo(Int64MaxValue)<=0){
				return new CBORObject(CBORObjectType_Integer,bigintValue.longValue());
			} else {
				return new CBORObject(CBORObjectType_BigInteger,bigintValue);
			}
		}
		/**
		 * Generates a CBOR object from a string.
		 * @param stringValue A string value. Can be null.
		 * @return A CBOR object representing the string, or CBORObject.Null
		 * if stringValue is null.
		 * @throws java.lang.IllegalArgumentException The string contains an unpaired
		 * surrogate code point.
		 */
		public static CBORObject FromObject(String stringValue) {
			if(stringValue==null)return CBORObject.Null;
			if(!IsValidString(stringValue))
				throw new IllegalArgumentException("String contains an unpaired surrogate code point.");
			return new CBORObject(CBORObjectType_TextString,stringValue);
		}
		/**
		 * Generates a CBOR object from a 32-bit signed integer.
		 */
		public static CBORObject FromObject(int value) {
			return FromObject((long)value);
		}
		/**
		 * Generates a CBOR object from a 16-bit signed integer.
		 */
		public static CBORObject FromObject(short value) {
			return FromObject((long)value);
		}
		/**
		 * Generates a CBOR string object from a Unicode character.
		 */
		public static CBORObject FromObject(char value) {
			return FromObject(new String(new char[]{value}));
		}
		/**
		 * Returns the CBOR true value or false value, depending on "value".
		 */
		public static CBORObject FromObject(boolean value) {
			return (value ? CBORObject.True : CBORObject.False);
		}
		/**
		 * Generates a CBOR object from a byte.
		 */
		public static CBORObject FromObject(byte value) {
			return FromObject(((int)value)&0xFF);
		}
		/**
		 * Generates a CBOR object from a 32-bit floating-point number.
		 */
		public static CBORObject FromObject(float value) {
			return new CBORObject(CBORObjectType_Single,value);
		}
		/**
		 * Generates a CBOR object from a 64-bit floating-point number.
		 */
		public static CBORObject FromObject(double value) {
			return new CBORObject(CBORObjectType_Double,value);
		}
		/**
		 * Generates a CBOR object from a byte array. The byte array is not copied.
		 * @param stringValue A byte array. Can be null.
		 * @return A CBOR object that uses the byte array, or CBORObject.Null
		 * if stringValue is null.
		 * @throws java.lang.IllegalArgumentException The string contains an unpaired
		 * surrogate code point.
		 */
		public static CBORObject FromObject(byte[] value) {
			if(value==null)return CBORObject.Null;
			return new CBORObject(CBORObjectType_ByteString,value);
		}
		/**
		 * Generates a CBOR object from an array of CBOR objects.
		 * @param array An array of CBOR objects.
		 * @return A CBOR object where each element of the given array is copied
		 * to a new array, or CBORObject.Null if "array" is null.
		 */
		public static CBORObject FromObject(CBORObject[] array) {
			if(array==null)return CBORObject.Null;
			List<CBORObject> list=new ArrayList<CBORObject>();
			for(CBORObject i : array){
				list.add(FromObject(i));
			}
			return new CBORObject(CBORObjectType_Array,list);
		}
		/**
		 * Generates a CBOR object from an list of objects.
		 * @param value An array of CBOR objects.
		 * @return A CBOR object where each element of the given array is converted
		 * to a CBOR object and copied to a new array, or CBORObject.Null if "value"
		 * is null.
		 */
		public static <T> CBORObject FromObject(List<T> value){
			if(value==null)return CBORObject.Null;
			List<CBORObject> list=new ArrayList<CBORObject>();
			for(T i : (List<T>)value){
				CBORObject obj=FromObject(i);
				list.add(obj);
			}
			return new CBORObject(CBORObjectType_Array,list);
		}
		/**
		 * Generates a CBOR object from a map of objects.
		 * @param dic A map of CBOR objects.
		 * @return A CBOR object where each key and value of the given map is converted
		 * to a CBOR object and copied to a new map, or CBORObject.Null if "dic"
		 * is null.
		 */
		public static <TKey, TValue> CBORObject FromObject(Map<TKey, TValue> dic){
			if(dic==null)return CBORObject.Null;
			HashMap<CBORObject,CBORObject> map=new HashMap<CBORObject,CBORObject>();
			for(Map.Entry<TKey,TValue> entry : dic.entrySet()){
				CBORObject key=FromObject(entry.getKey());
				CBORObject value=FromObject(entry.getValue());
				map.put(key,value);
			}
			return new CBORObject(CBORObjectType_Map,map);
		}
		
		/**
		 * Generates a CBORObject from an arbitrary object.
		 * @param obj
		 * @return A CBOR object corresponding to the given object. Returns
		 * CBORObject.Null if the object is null.
		 * @throws java.lang.IllegalArgumentException The object's type is not supported.
		 */
		@SuppressWarnings("unchecked")
public static CBORObject FromObject(Object obj) {
			if(obj==null)return CBORObject.Null;
			if(obj instanceof Long)return FromObject(((Long)obj).longValue());
			if(obj instanceof CBORObject)return FromObject((CBORObject)obj);
			if(obj instanceof BigInteger)return FromObject((BigInteger)obj);
			if(obj instanceof String)return FromObject((String)obj);
			if(obj instanceof Integer)return FromObject(((Integer)obj).intValue());
			if(obj instanceof Short)return FromObject(((Short)obj).shortValue());
			if(obj instanceof Character)return FromObject(((Character)obj).charValue());
			if(obj instanceof Boolean)return FromObject(((Boolean)obj).booleanValue());
			if(obj instanceof Byte)return FromObject(((Byte)obj).byteValue());
			if(obj instanceof Float)return FromObject(((Float)obj).floatValue());
			
			
			
			
			
			
			
			
			if(obj instanceof Double)return FromObject(((Double)obj).doubleValue());
			if(obj instanceof List<?>)return FromObject((List<CBORObject>)obj);
			if(obj instanceof byte[])return FromObject((byte[])obj);
			if(obj instanceof CBORObject[])return FromObject((CBORObject[])obj);
			if(obj instanceof Map<?,?>)return FromObject(
				(Map<CBORObject,CBORObject>)obj);
			if(obj instanceof Map<?,?>)return FromObject(
				(Map<String,CBORObject>)obj);
			throw new IllegalArgumentException("Unsupported Object type.");
		}
		
		private static BigInteger BigInt65536=BigInteger.valueOf(65536);

		/**
		 * Generates a CBOR object from an arbitrary object and gives the resulting
		 * object a tag.
		 * @param o An arbitrary object.
		 * @param bigintTag A big integer that specifies a tag number.
		 * @return a CBOR object where the object "o" is converted to a CBOR object
		 * and given the tag "bigintTag".
		 * @throws java.lang.IllegalArgumentException "bigintTag" is less than 0 or
		 * greater than 2^64-1, or "o"'s type is unsupported.
		 */
		public static CBORObject FromObjectAndTag(Object o, BigInteger bigintTag) {
			if((bigintTag).signum()<0)throw new IllegalArgumentException(
				"tag not greater or equal to 0 ("+bigintTag.toString()+")");
			if((bigintTag).compareTo(UInt64MaxValue)>0)throw new IllegalArgumentException(
				"tag not less or equal to 18446744073709551615 ("+bigintTag.toString()+")");
			CBORObject c=FromObject(o);
			if(bigintTag.compareTo(BigInt65536)<0){
				// Low-numbered, commonly used tags
				return new CBORObject(c,bigintTag.intValue(),0);
			} else if(bigintTag.compareTo(BigInteger.valueOf(2))==0){
				return ConvertToBigNum(c,false);
			} else if(bigintTag.compareTo(BigInteger.valueOf(3))==0){
				return ConvertToBigNum(c,true);
			} else if(bigintTag.compareTo(BigInteger.valueOf(4))==0){
				return ConvertToDecimalFrac(c);
			} else {
				long tagLow=0;
				long tagHigh=0;
				BigInteger tmpbigint=bigintTag.and(FiftySixBitMask);
				tagLow=tmpbigint.longValue();
				tmpbigint=(bigintTag.shiftRight(56)).and(BigInteger.valueOf(0xFF));
				tagHigh=tmpbigint.longValue();
				int low=((int)(tagLow&0xFFFFFFFFL));
				tagHigh=(tagHigh<<24)|((tagLow>>32)&0xFFFFFFFFL);
				int high=((int)(tagHigh&0xFFFFFFFFL));
				return new CBORObject(c,low,high);
			}
		}

		/**
		 * Generates a CBOR object from an arbitrary object and gives the resulting
		 * object a tag.
		 * @param o An arbitrary object.
		 * @param bigintTag A 32-bit integer that specifies a tag number.
		 * @return a CBOR object where the object "o" is converted to a CBOR object
		 * and given the tag "bigintTag".
		 * @throws java.lang.IllegalArgumentException "intTag" is less than 0 or "o"'s
		 * type is unsupported.
		 */
		public static CBORObject FromObjectAndTag(Object o, int intTag) {
			if(intTag<0)throw new IllegalArgumentException(
				"tag not greater or equal to 0 ("+
				Integer.toString((int)intTag)+")");
			CBORObject c=FromObject(o);
			if(intTag==2 || intTag==3){
				return ConvertToBigNum(c,intTag==3);
			}
			if(intTag==4){
				return ConvertToDecimalFrac(c);
			}
			return new CBORObject(c,intTag,0);
		}
		
		//-----------------------------------------------------------
		
		private void AppendClosingTags(StringBuilder sb) {
			CBORObject curobject=this;
			while(curobject.isTagged()) {
				sb.append(')');
				curobject=((CBORObject)(curobject.item_));
			}
		}
		
		private void WriteTags(OutputStream s) throws IOException {
			CBORObject curobject=this;
			while(curobject.isTagged()) {
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
					s.write(arrayToWrite,0,9);
				}
				curobject=((CBORObject)(curobject.item_));
			}
		}

		private void AppendOpeningTags(StringBuilder sb) {
			CBORObject curobject=this;
			while(curobject.isTagged()) {
				int low=curobject.tagLow;
				int high=curobject.tagHigh;
				if(high==0 && (low>>16)==0){
					sb.append(Integer.toString((int)low));
				} else {
					BigInteger bi=LowHighToBigInteger(low,high);
					sb.append(bi.toString());
				}
				sb.append('(');
				curobject=((CBORObject)(curobject.item_));
			}
		}
		
		private static String TrimDotZero(String str) {
			if(str.length()>2 && str.charAt(str.length()-1)=='0' && str.charAt(str.length()-2)=='.'){
				return str.substring(0,str.length()-2);
			}
			return str;
		}
		
		/**
		 * Returns this CBOR object in string form. The format is intended to
		 * be human-readable, not machine- parsable, and the format may change
		 * at any time.
		 * @return A text representation of this object.
		 */
		@Override public String toString() {
			StringBuilder sb=null;
			String simvalue=null;
			if(this.isTagged()){
				if(sb==null){
					if(this.getItemType()==CBORObjectType_TextString){
						// The default capacity of StringBuilder may be too small
						// for many strings, so set a suggested capacity
						// explicitly
						String str=this.AsString();
						sb=new StringBuilder(Math.min(str.length(),4096)+16);
					} else {
						sb=new StringBuilder();
					}
				}
				AppendOpeningTags(sb);
			}
			if(this.getItemType()== CBORObjectType_SimpleValue){
				if(this.isTrue()){
					simvalue="true";
				}
				else if(this.isFalse()){
					simvalue="false";
				}
				else if(this.isNull()){
					simvalue="null";
				}
				else if(this.isUndefined()){
					simvalue="undefined";
				}
				else {
					if(sb==null)sb=new StringBuilder();
					sb.append("simple(");
					sb.append(Integer.toString(((Integer)this.getThisItem()).intValue()));
					sb.append(")");
				}
				if(simvalue!=null){
					if(sb==null)return simvalue;
					sb.append(simvalue);
				}
			} else if(this.getItemType()== CBORObjectType_Single){
				float f=((Float)this.getThisItem()).floatValue();
				if(((f)==Float.NEGATIVE_INFINITY))
					simvalue=("-Infinity");
				else if(((f)==Float.POSITIVE_INFINITY))
					simvalue=("Infinity");
				else if(Float.isNaN(f))
					simvalue=("NaN");
				else
					simvalue=(TrimDotZero(
						Float.toString((float)f)));
				if(sb==null)return simvalue;
				sb.append(simvalue);
			} else if(this.getItemType()== CBORObjectType_Double){
				double f=((Double)this.getThisItem()).doubleValue();
				if(((f)==Double.NEGATIVE_INFINITY))
					simvalue=("-Infinity");
				else if(((f)==Double.POSITIVE_INFINITY))
					simvalue=("Infinity");
				else if(Double.isNaN(f))
					simvalue=("NaN");
				else
					simvalue=(TrimDotZero(
						Double.toString((double)f)));
				if(sb==null)return simvalue;
				sb.append(simvalue);
			} else if(this.getItemType()== CBORObjectType_Integer){
				long v=((Long)this.getThisItem()).longValue();
				simvalue=(Long.toString((long)v));
				if(sb==null)return simvalue;
				sb.append(simvalue);
			} else if(this.getItemType()== CBORObjectType_BigInteger){
				simvalue=(((BigInteger)this.getThisItem()).toString());
				if(sb==null)return simvalue;
				sb.append(simvalue);
			} else if(this.getItemType()== CBORObjectType_ByteString){
				if(sb==null)sb=new StringBuilder();
				sb.append("h'");
				byte[] data=(byte[])this.getThisItem();
				ToBase16(sb,data);
				sb.append("'");
			} else if(this.getItemType()== CBORObjectType_TextString){
				if(sb==null){
					return "\""+(this.AsString())+"\"";
				} else {
					sb.append('\"');
					sb.append(this.AsString());
					sb.append('\"');
				}
			} else if(this.getItemType()== CBORObjectType_DecimalFraction){
				return ToJSONString();
			} else if(this.getItemType()== CBORObjectType_Array){
				if(sb==null)sb=new StringBuilder();
				boolean first=true;
				sb.append("[");
				for(CBORObject i : AsList()){
					if(!first)sb.append(", ");
					sb.append(i.toString());
					first=false;
				}
				sb.append("]");
			} else if(this.getItemType()== CBORObjectType_Map){
				if(sb==null)sb=new StringBuilder();
				boolean first=true;
				sb.append("{");
				Map<CBORObject,CBORObject> map=AsMap();
				for(Map.Entry<CBORObject,CBORObject> entry : map.entrySet()){
					CBORObject key=entry.getKey();
					CBORObject value=entry.getValue();
					if(!first)sb.append(", ");
					sb.append(key.toString());
					sb.append(": ");
					sb.append(value.toString());
					first=false;
				}
				sb.append("}");
			}
			if(this.isTagged()){
				AppendClosingTags(sb);
			}
			return sb.toString();
		}

		private static float HalfPrecisionToSingle(int value) {
			int negvalue=(value>=0x8000) ? (1<<31) : 0;
			value&=0x7FFF;
			if(value>=0x7C00){
				return Float.intBitsToFloat(
					(0x3FC00|(value&0x3FF))<<13|negvalue);
			} else if(value>0x400){
				return Float.intBitsToFloat(
					((value+0x1c000)<<13)|negvalue);
			} else if((value&0x400)==value){
				return Float.intBitsToFloat(
					((value==0) ? 0 : 0x38800000)|negvalue);
			} else {
				// denormalized
				int m=(value&0x3FF);
				value=0x1c400;
				while((m>>10)==0){
					value-=0x400;
					m<<=1;
				}
				return Float.intBitsToFloat(
					((value|(m&0x3FF))<<13)|negvalue);
			}
		}

		private static CBORObject ConvertToBigNum(CBORObject o, boolean negative) {
			if(o.getItemType()!=CBORObjectType_ByteString)
				throw new CBORException("Byte array expected");
			byte[] data=(byte[])o.getThisItem();
			BigInteger bi=BigInteger.ZERO;
			for(int i=0;i<data.length;i++){
				bi=bi.shiftLeft(8);
				int x=((int)data[i])&0xFF;
				bi=bi.or(BigInteger.valueOf(x));
			}
			if(negative){
				bi=BigInteger.valueOf(-1).subtract(bi); // Convert to a negative
			}
			return RewrapObject(o,FromObject(bi));
		}
		
		private boolean IsZero() {
			switch(this.getItemType()){
				case CBORObjectType_Integer:
					return (((Long)this.getThisItem()).longValue())==0;
				case CBORObjectType_BigInteger:
					return ((BigInteger)this.getThisItem()).equals(BigInteger.ZERO);
				case CBORObjectType_Single:
					return (((Float)this.getThisItem()).floatValue())==0;
				case CBORObjectType_Double:
					return (((Double)this.getThisItem()).doubleValue())==0;
				case CBORObjectType_DecimalFraction:
					return ((DecimalFraction)this.getThisItem()).equals(BigInteger.ZERO);
				default:
					return false;
			}
		}
		
		private boolean CanFitInTypeZeroOrOne() {
			switch(this.getItemType()){
				case CBORObjectType_Integer:
					return true;
					case CBORObjectType_BigInteger:{
						BigInteger bigint=(BigInteger)this.getThisItem();
						return bigint.compareTo(LowestMajorType1)>=0 &&
							bigint.compareTo(UInt64MaxValue)<=0;
					}
					case CBORObjectType_Single:{
						float value=((Float)this.getThisItem()).floatValue();
						return value>=-18446744073709551616.0f &&
							value<=18446742974197923840.0f; // Highest float less or eq. to ULong.MAX_VALUE
					}
					case CBORObjectType_Double:{
						double value=((Double)this.getThisItem()).doubleValue();
						return value>=-18446744073709551616.0 &&
							value<=18446744073709549568.0; // Highest double less or eq. to ULong.MAX_VALUE
					}
					case CBORObjectType_DecimalFraction:{
						DecimalFraction value=(DecimalFraction)this.getThisItem();
						return value.compareTo(new DecimalFraction(LowestMajorType1))>=0 &&
							value.compareTo(new DecimalFraction(UInt64MaxValue))<=0;
					}
				default:
					return false;
			}
		}
		
		// Wrap a new Object in another one to retain its tags
		private static CBORObject RewrapObject(CBORObject original, CBORObject newObject) {
			if(!original.isTagged())
				return newObject;
			BigInteger[] tags=original.GetTags();
			for(int i=tags.length-1;i>=0;i++){
				newObject=FromObjectAndTag(newObject,tags[i]);
			}
			return newObject;
		}


		private static CBORObject ConvertToDecimalFrac(CBORObject o) {
			if(o.getItemType()!=CBORObjectType_Array)
				throw new CBORException("Decimal fraction must be an array");
			if(o.size()!=2) // Requires 2 items
				throw new CBORException("Decimal fraction requires exactly 2 items");
			List<CBORObject> list=o.AsList();
			if(!list.get(1).CanFitInTypeZeroOrOne())
				throw new CBORException("Exponent is too big");
			// check type of mantissa
			if(list.get(1).getItemType()!=CBORObjectType_Integer &&
			   list.get(1).getItemType()!=CBORObjectType_BigInteger)
				throw new CBORException("Decimal fraction requires mantissa to be an integer or big integer");
			if(list.get(0).IsZero() && !o.isTagged()){
				// Exponent is 0, so return mantissa instead
				return list.get(1);
			}
			return RewrapObject(o,new CBORObject(
				CBORObjectType_DecimalFraction,
				new DecimalFraction(list.get(0).AsBigInteger(),list.get(1).AsBigInteger())));
		}
		
		private static boolean CheckMajorTypeIndex(int type, int index, int[] validTypeFlags) {
			if(validTypeFlags==null || index<0 || index>=validTypeFlags.length)
				return false;
			if(type<0 || type>7)
				return false;
			return (validTypeFlags[index]&(1<<type))!=0;
		}
		
		private static CBORObject Read(
			InputStream s,
			int depth,
			boolean allowBreak,
			int allowOnlyType,
			int[] validTypeFlags,
			int validTypeIndex
		) throws IOException {
			if(depth>1000)
				throw new CBORException("Too deeply nested");
			int firstbyte=s.read();
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
					                        Integer.toString((int)allowOnlyType)+
					                        ", instead got type "+
					                        Integer.toString(((Integer)type).intValue()));
				}
				if(additional==31){
					throw new CBORException("Indefinite-length data not allowed here");
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
			// Check if this represents a fixed Object
			CBORObject fixedObject=FixedObjects[firstbyte];
			if(fixedObject!=null)
				return fixedObject;
			// Read fixed-length data
			byte[] data=null;
			if(expectedLength!=0){
				data=new byte[expectedLength];
				// include the first byte because this function
				// will assume it exists for some head bytes
				data[0]=((byte)firstbyte);
				if(expectedLength>1 &&
				   s.read(data,1,expectedLength-1)!=expectedLength-1)
					throw new CBORException("Premature end of data");
				return GetFixedLengthObject(firstbyte,data);
			}
			long uadditional=(long)additional;
			BigInteger bigintAdditional=BigInteger.ZERO;
			boolean hasBigAdditional=false;
			data=new byte[8];
			switch(firstbyte&0x1F){
					case 24:{
						int tmp=s.read();
						if(tmp<0)
							throw new CBORException("Premature end of data");
						uadditional=tmp;
						break;
					}
					case 25:{
						if(s.read(data,0,2)!=2)
							throw new CBORException("Premature end of data");
						uadditional=(((long)(data[0]&(long)0xFF))<<8);
						uadditional|=(((long)(data[1]&(long)0xFF)));
						break;
					}
					case 26:{
						if(s.read(data,0,4)!=4)
							throw new CBORException("Premature end of data");
						uadditional=(((long)(data[0]&(long)0xFF))<<24);
						uadditional|=(((long)(data[1]&(long)0xFF))<<16);
						uadditional|=(((long)(data[2]&(long)0xFF))<<8);
						uadditional|=(((long)(data[3]&(long)0xFF)));
						break;
					}
					case 27:{
						if(s.read(data,0,8)!=8)
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
							bigintAdditional=BigInteger.valueOf(highAdditional);
							bigintAdditional=bigintAdditional.shiftLeft(56);
							bigintAdditional=bigintAdditional.or(BigInteger.valueOf(lowAdditional));
						}
						break;
					}
				default:
					break;
			}
			if(type==2){ // Byte String
				if(additional==31){
					// Streaming byte String
					java.io.ByteArrayOutputStream ms=null;
try {
ms=new ByteArrayOutputStream();

						// Requires same type as this one
						int[] subFlags=new int[]{(1<<type)};
						while(true){
							CBORObject o=Read(s,depth+1,true,type,subFlags,0);
							if(o==null)break;//break if the "break" code was read
							data=(byte[])o.getThisItem();
							ms.write(data,0,data.length);
						}
						if(ms.size()>Integer.MAX_VALUE)
							throw new CBORException("Length of bytes to be streamed is bigger than supported");
						data=ms.toByteArray();
						return new CBORObject(
							CBORObjectType_ByteString,
							data);
}
finally {
try { if(ms!=null)ms.close(); } catch(IOException ex){}
}
				} else {
					if(hasBigAdditional){
						throw new CBORException("Length of "+
						                        bigintAdditional.toString()+
						                        " is bigger than supported");
					} else if(uadditional>Integer.MAX_VALUE){
						throw new CBORException("Length of "+
						                        Long.toString((long)uadditional)+
						                        " is bigger than supported");
					}
					data=null;
					if(uadditional<=0x10000){
						// Simple case: small size
						data=new byte[(int)uadditional];
						if(s.read(data,0,data.length)!=data.length)
							throw new CBORException("Premature end of stream");
					} else {
						byte[] tmpdata=new byte[0x10000];
						int total=(int)uadditional;
						java.io.ByteArrayOutputStream ms=null;
try {
ms=new ByteArrayOutputStream();

							while(total>0){
								int bufsize=Math.min(tmpdata.length,total);
								if(s.read(tmpdata,0,bufsize)!=bufsize)
									throw new CBORException("Premature end of stream");
								ms.write(tmpdata,0,bufsize);
								total-=bufsize;
							}
							data=ms.toByteArray();
}
finally {
try { if(ms!=null)ms.close(); } catch(IOException ex){}
}
					}
					return new CBORObject(
						CBORObjectType_ByteString,
						data);
				}
			} else if(type==3){ // Text String
				if(additional==31){
					// Streaming text String
					StringBuilder builder=new StringBuilder();
					// Requires same type as this one
					int[] subFlags=new int[]{(1<<type)};
					while(true){
						CBORObject o=Read(s,depth+1,true,type,subFlags,0);
						if(o==null)break;//break if the "break" code was read
						builder.append((String)o.getThisItem());
					}
					return new CBORObject(
						CBORObjectType_TextString,
						builder.toString());
				} else {
					if(hasBigAdditional){
						throw new CBORException("Length of "+
						                        bigintAdditional.toString()+
						                        " is bigger than supported");
					} else if(uadditional>Integer.MAX_VALUE){
						throw new CBORException("Length of "+
						                        Long.toString((long)uadditional)+
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
					return new CBORObject(CBORObjectType_TextString,builder.toString());
				}
			} else if(type==4){ // Array
				List<CBORObject> list=new ArrayList<CBORObject>();
				int vtindex=1;
				if(additional==31){
					while(true){
						CBORObject o=Read(s,depth+1,true,-1,validTypeFlags,vtindex);
						if(o==null)break;//break if the "break" code was read
						list.add(o);
						vtindex++;
					}
					return new CBORObject(CBORObjectType_Array,list);
				} else {
					if(hasBigAdditional){
						throw new CBORException("Length of "+
						                        bigintAdditional.toString()+
						                        " is bigger than supported");
					} else if(uadditional>Integer.MAX_VALUE){
						throw new CBORException("Length of "+
						                        Long.toString((long)uadditional)+
						                        " is bigger than supported");
					}
					for(long i=0;i<uadditional;i++){
						list.add(Read(s,depth+1,false,-1,validTypeFlags,vtindex));
						vtindex++;
					}
					return new CBORObject(CBORObjectType_Array,list);
				}
			} else if(type==5){ // Map, type 5
				HashMap<CBORObject,CBORObject> dict=new HashMap<CBORObject,CBORObject>();
				if(additional==31){
					while(true){
						CBORObject key=Read(s,depth+1,true,-1,null,0);
						if(key==null)break;//break if the "break" code was read
						CBORObject value=Read(s,depth+1,false,-1,null,0);
						dict.put(key,value);
					}
					return new CBORObject(CBORObjectType_Map,dict);
				} else {
					if(hasBigAdditional){
						throw new CBORException("Length of "+
						                        bigintAdditional.toString()+
						                        " is bigger than supported");
					} else if(uadditional>Integer.MAX_VALUE){
						throw new CBORException("Length of "+
						                        Long.toString((long)uadditional)+
						                        " is bigger than supported");
					}
					for(long i=0;i<uadditional;i++){
						CBORObject key=Read(s,depth+1,false,-1,null,0);
						CBORObject value=Read(s,depth+1,false,-1,null,0);
						dict.put(key,value);
					}
					return new CBORObject(CBORObjectType_Map,dict);
				}
			} else if(type==6){ // Tagged item
				CBORObject o;
				if(!hasBigAdditional){
					if(uadditional==0){
						// Requires a text String
						int[] subFlags=new int[]{(1<<3)};
						o=Read(s,depth+1,false,-1,subFlags,0);
					} else if(uadditional==2 || uadditional==3){
						// Big number
						// Requires a byte String
						int[] subFlags=new int[]{(1<<2)};
						o=Read(s,depth+1,false,-1,subFlags,0);
						return ConvertToBigNum(o,uadditional==3);
					} else if(uadditional==4){
						// Requires an array with two elements of
						// a valid type
						int[] subFlags=new int[]{
							(1<<4), // array
							(1<<0)|(1<<1), // exponent
							(1<<0)|(1<<1)|(1<<6) // mantissa
						};
						o=Read(s,depth+1,false,-1,subFlags,0);
						return ConvertToDecimalFrac(o);
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
						o,BigInteger.valueOf(uadditional));
				}
			} else {
				throw new CBORException("Unexpected data encountered");
			}
		}
	}
