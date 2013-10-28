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



	/// <summary>
	/// Represents an Object in Concise Binary Object
	/// Representation (CBOR) and contains methods for reading
	/// and writing CBOR data.  CBOR is defined in
	/// RFC 7049.
	/// </summary>
	public final class CBORObject
	{
		enum CBORObjectType {
			// Integer from Long.MIN_VALUE..Long.MAX_VALUE
			Integer,
			// Big integers exceed the range Long.MIN_VALUE..Long.MAX_VALUE
			BigInteger,
			ByteString,
			TextString,
			Array,
			Map,
			SimpleValue,
			Single,
			Double
		}
		
		private CBORObjectType getItemType() {
				return itemtype_;
			}
		
		Object item;
		CBORObjectType itemtype_;
		boolean tagged=false;
		int tagLow=0;
		int tagHigh=0;
		
		
		private static final BigInteger Int64MaxValue=BigInteger.valueOf(Long.MAX_VALUE);
		private static final BigInteger Int64MinValue=BigInteger.valueOf(Long.MIN_VALUE);
		public static final CBORObject Break=new CBORObject(CBORObjectType.SimpleValue,31);
		public static final CBORObject False=new CBORObject(CBORObjectType.SimpleValue,20);
		public static final CBORObject True=new CBORObject(CBORObjectType.SimpleValue,21);
		public static final CBORObject Null=new CBORObject(CBORObjectType.SimpleValue,22);
		public static final CBORObject Undefined=new CBORObject(CBORObjectType.SimpleValue,23);
		
		private CBORObject(){}
		
		private CBORObject(CBORObjectType type, int tagLow, int tagHigh, Object item){
 this(type,item);
			this.itemtype_=type;
			this.tagLow=tagLow;
			this.tagHigh=tagHigh;
			this.tagged=true;
		}
		private CBORObject(CBORObjectType type, Object item){
				
			this.itemtype_=type;
			this.item=item;
		}
		
		public boolean isBreak() {
				return this.getItemType()==CBORObjectType.SimpleValue && ((Integer)item).intValue()==31;
			}
		public boolean isTrue() {
				return this.getItemType()==CBORObjectType.SimpleValue && ((Integer)item).intValue()==21;
			}
		public boolean isFalse() {
				return this.getItemType()==CBORObjectType.SimpleValue && ((Integer)item).intValue()==20;
			}
		public boolean isNull() {
				return this.getItemType()==CBORObjectType.SimpleValue && ((Integer)item).intValue()==22;
			}
		public boolean isUndefined() {
				return this.getItemType()==CBORObjectType.SimpleValue && ((Integer)item).intValue()==23;
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
		
		private int ByteArrayHashCode(byte[] a) {
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
			int ret=19;
			{
				ret=ret*31+list.size();
				for(int i=0;i<list.size();i++){
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
			// same hash code), which is much
			// too difficult for this version.
			return (a.size()*19);
		}

		@Override @SuppressWarnings("unchecked")
public boolean equals(Object obj) {
			CBORObject other = ((obj instanceof CBORObject) ? (CBORObject)obj : null);
			if (other == null)
				return false;
			if(item instanceof byte[]){
				if(!ByteArrayEquals((byte[])item,((other.item instanceof byte[]) ? (byte[])other.item : null)))
					return false;
			} else if(item instanceof List<?>){
				if(!CBORArrayEquals(AsList(),((other.item instanceof List<?>) ? (List<CBORObject>)other.item : null)))
					return false;
			} else if(item instanceof Map<?,?>){
				if(!CBORMapEquals(AsMap(),
				                  ((other.item instanceof Map<?,?>) ? (Map<CBORObject,CBORObject>)other.item : null)))
					return false;
			} else {
				if(!(((this.item)==null) ? ((other.item)==null) : (this.item).equals(other.item)))
					return false;
			}
			return this.getItemType() == other.getItemType() && this.tagged == other.tagged &&
				this.tagLow == other.tagLow &&
				this.tagHigh == other.tagHigh;
		}
		
		@Override public int hashCode() {
			int hashCode_ = 0;
			{
				if (item != null){
					int itemHashCode=0;
					if(item instanceof byte[])
						itemHashCode=ByteArrayHashCode((byte[])item);
					else if(item instanceof List<?>)
						itemHashCode=CBORArrayHashCode(AsList());
					else if(item instanceof Map<?,?>)
						itemHashCode=CBORMapHashCode(AsMap());
					else
						itemHashCode=item.hashCode();
					hashCode_ += 1000000007 * itemHashCode;
				}
				hashCode_ += 1000000009 * this.getItemType().hashCode();
				hashCode_ += 1000000021 * ((Object)tagged).hashCode();
				hashCode_ += 1000000033 * tagLow;
				hashCode_ += 1000000033 * tagHigh;
			}
			return hashCode_;
		}
		

		
		public static CBORObject FromBytes(byte[] data) throws IOException {
			java.io.ByteArrayInputStream ms=null;
try {
ms=new ByteArrayInputStream(data);
int startingAvailable=ms.available();

				CBORObject o=Read(ms);
				if((startingAvailable-ms.available())!=data.length){
					throw new NumberFormatException();
				}
				return o;
}
finally {
if(ms!=null)ms.close();
}
		}
		public int size() {
				if(this.getItemType()== CBORObjectType.Array){
					return (AsList()).size();
				} else if(this.getItemType()== CBORObjectType.Map){
					return (AsMap()).size();
				} else {
					return 0;
				}
			}
		
		/// <summary>
		/// Gets whether this data item has a tag.
		/// </summary>
		public boolean isTagged() {
				return tagged;
			}

		/// <summary>
		/// Gets the tag for this CBOR data item,
		/// or 0 if the item is untagged.
		/// </summary>
		public BigInteger getTag() {
				if(tagged){
					if(tagHigh==0 && tagLow>=0 && tagLow<0x10000){
						return BigInteger.valueOf(tagLow);
					}
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
				} else {
					return BigInteger.ZERO;
				}
			}

		@SuppressWarnings("unchecked")
private Map<CBORObject,CBORObject> AsMap() {
			return (Map<CBORObject,CBORObject>)item;
		}
		
		@SuppressWarnings("unchecked")
private List<CBORObject> AsList() {
			return (List<CBORObject>)item;
		}
		
		/// <summary>
		/// Gets or sets the value of a CBOR Object in this
		/// array.
		/// </summary>
		public CBORObject get(int index) {
				if(this.getItemType()== CBORObjectType.Array){
					List<CBORObject> list=AsList();
					return list.get(index);
				} else {
					throw new IllegalStateException("Not an array");
				}
			}
public void set(int index, CBORObject value) {
				if(this.getItemType()== CBORObjectType.Array){
					if(this.getTag().equals(BigInteger.valueOf(4)))
						throw new IllegalStateException("Read-only array");
					List<CBORObject> list=AsList();
					list.set(index,value);
				} else {
					throw new IllegalStateException("Not an array");
				}
			}

		public Collection<CBORObject> getKeys() {
				if(this.getItemType()== CBORObjectType.Map){
					Map<CBORObject,CBORObject> dict=AsMap();
					return dict.keySet();
				} else {
					throw new IllegalStateException("Not a map");
				}
			}
		
		/// <summary>
		/// Gets or sets the value of a CBOR Object in this
		/// map.
		/// </summary>
		public CBORObject get(CBORObject key) {
				if(this.getItemType()== CBORObjectType.Map){
					Map<CBORObject,CBORObject> map=AsMap();
					return map.get(key);
				} else {
					throw new IllegalStateException("Not a map");
				}
			}
public void set(CBORObject key, CBORObject value) {
				if(this.getItemType()== CBORObjectType.Map){
					Map<CBORObject,CBORObject> map=AsMap();
					map.put(key,value);
				} else {
					throw new IllegalStateException("Not a map");
				}
			}
		
		public void Add(CBORObject key, CBORObject value) {
			if(this.getItemType()== CBORObjectType.Map){
				Map<CBORObject,CBORObject> map=AsMap();
				map.put(key,value);
			} else {
				throw new IllegalStateException("Not a map");
			}
		}

		public void ContainsKey(CBORObject key) {
			if(this.getItemType()== CBORObjectType.Map){
				Map<CBORObject,CBORObject> map=AsMap();
				map.containsKey(key);
			} else {
				throw new IllegalStateException("Not a map");
			}
		}

		public void Add(CBORObject obj) {
			if(this.getItemType()== CBORObjectType.Array){
				if(this.getTag().equals(BigInteger.valueOf(4)))
					throw new IllegalStateException("Read-only array");
				List<CBORObject> list=AsList();
				list.add(obj);
			} else {
				throw new IllegalStateException("Not an array");
			}
		}
		
		/// <summary>
		/// Converts this Object to a 64-bit floating point
		/// number.
		/// </summary>
		/// <returns>The closest 64-bit floating point number
		/// to this Object.</returns>
		/// <exception cref="System.IllegalStateException">
		/// This Object's type is not an integer
		/// or a floating-point number.</exception>
		public double AsDouble() {
			if(this.getItemType()== CBORObjectType.Integer)
				return ((Long)item).doubleValue();
			else if(this.getItemType()== CBORObjectType.BigInteger)
				return ((BigInteger)item).doubleValue();
			else if(this.getItemType()== CBORObjectType.Single)
				return ((Float)item).doubleValue();
			else if(this.getItemType()== CBORObjectType.Double)
				return ((Double)item).doubleValue();
			else if(this.getTag().equals(BigInteger.valueOf(4)) && this.getItemType()== CBORObjectType.Array &&
			        this.size()==2){
				StringBuilder sb=new StringBuilder();
				sb.append(this.get(1).IntegerToString());
				sb.append("e");
				sb.append(this.get(0).IntegerToString());
				return Double.parseDouble(sb.toString());
			}
			else
				throw new IllegalStateException("Not a number type");
		}
		

		/// <summary>
		/// Converts this Object to a 32-bit floating point
		/// number.
		/// </summary>
		/// <returns>The closest 32-bit floating point number
		/// to this Object.</returns>
		/// <exception cref="System.IllegalStateException">
		/// This Object's type is not an integer
		/// or a floating-point number.</exception>
		public float AsSingle() {
			if(this.getItemType()== CBORObjectType.Integer)
				return ((Long)item).floatValue();
			else if(this.getItemType()== CBORObjectType.BigInteger)
				return ((BigInteger)item).floatValue();
			else if(this.getItemType()== CBORObjectType.Single)
				return ((Float)item).floatValue();
			else if(this.getItemType()== CBORObjectType.Double)
				return ((Double)item).floatValue();
			else if(this.getTag().equals(BigInteger.valueOf(4)) &&
			        this.getItemType()== CBORObjectType.Array &&
			        this.size()==2){
				StringBuilder sb=new StringBuilder();
				sb.append(this.get(1).IntegerToString());
				sb.append("e");
				sb.append(this.get(0).IntegerToString());
				return Float.parseFloat(sb.toString());
			}
			else
				throw new IllegalStateException("Not a number type");
		}

		/// <summary>
		/// Converts this Object to an arbitrary-length
		/// integer.  Floating point values are truncated
		/// to an integer.
		/// </summary>
		/// <returns>The closest big integer
		/// to this Object.</returns>
		/// <exception cref="System.IllegalStateException">
		/// This Object's type is not an integer
		/// or a floating-point number.</exception>
		public BigInteger AsBigInteger() {
			if(this.getItemType()== CBORObjectType.Integer)
				return BigInteger.valueOf(((Long)item).longValue());
			else if(this.getItemType()== CBORObjectType.BigInteger)
				return (BigInteger)item;
			else if(this.getItemType()== CBORObjectType.Single)
				return new BigDecimal(((Float)item).floatValue()).toBigInteger();
			else if(this.getItemType()== CBORObjectType.Double)
				return new BigDecimal(((Double)item).doubleValue()).toBigInteger();
			else if(this.getTag().equals(BigInteger.valueOf(4)) && this.getItemType()== CBORObjectType.Array &&
			        this.size()==2){
				StringBuilder sb=new StringBuilder();
				sb.append(this.get(1).IntegerToString());
				sb.append("e");
				sb.append(this.get(0).IntegerToString());
				return ParseBigIntegerWithExponent(sb.toString());
			}
			else
				throw new IllegalStateException("Not a number type");
		}
		
		public boolean AsBoolean() {
			if(this.isFalse() || this.isNull() || this.isUndefined())
				return false;
			return true;
		}
		
		public short AsInt16() {
			int v=AsInt32();
			if(v>Short.MAX_VALUE || v<0)
				throw new ArithmeticException();
			return (short)v;
		}
		
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

		
		/// <summary>
		/// Converts this Object to a 64-bit signed
		/// integer.  Floating point values are truncated
		/// to an integer.
		/// </summary>
		/// <returns>The closest big integer
		/// to this Object.</returns>
		/// <exception cref="System.IllegalStateException">
		/// This Object's type is not an integer
		/// or a floating-point number.</exception>
		/// <exception cref="System.ArithmeticException">
		/// This Object's value exceeds the range of a 64-bit
		/// signed integer.</exception>
		public long AsInt64() {
			if(this.getItemType()== CBORObjectType.Integer){
				return ((Long)item).longValue();
			} else if(this.getItemType()== CBORObjectType.BigInteger){
				if(((BigInteger)item).compareTo(Int64MaxValue)>0 ||
				   ((BigInteger)item).compareTo(Int64MinValue)<0)
					throw new ArithmeticException();
				return ((BigInteger)item).longValue();
			} else if(this.getItemType()== CBORObjectType.Single){
				if(Float.isNaN(((Float)item).floatValue()) ||
				   ((Float)item).floatValue()>Long.MAX_VALUE || ((Float)item).floatValue()<Long.MIN_VALUE)
					throw new ArithmeticException();
				return ((Float)item).longValue();
			} else if(this.getItemType()== CBORObjectType.Double){
				if(Double.isNaN(((Double)item).doubleValue()) ||
				   ((Double)item).doubleValue()>Long.MIN_VALUE || ((Double)item).doubleValue()<Long.MIN_VALUE)
					throw new ArithmeticException();
				return ((Double)item).longValue();
			} else if(this.getTag().equals(BigInteger.valueOf(4)) && this.getItemType()== CBORObjectType.Array &&
			          this.size()==2){
				StringBuilder sb=new StringBuilder();
				sb.append(this.get(1).IntegerToString());
				sb.append("e");
				sb.append(this.get(0).IntegerToString());
				BigInteger bi=ParseBigIntegerWithExponent(sb.toString());
				if(bi.compareTo(Int64MaxValue)>0 ||
				   bi.compareTo(Int64MinValue)<0)
					throw new ArithmeticException();
				return bi.longValue();
			} else
				throw new IllegalStateException("Not a number type");
		}
		
		/// <summary>
		/// Converts this Object to a 32-bit signed
		/// integer.  Floating point values are truncated
		/// to an integer.
		/// </summary>
		/// <returns>The closest big integer
		/// to this Object.</returns>
		/// <exception cref="System.IllegalStateException">
		/// This Object's type is not an integer
		/// or a floating-point number.</exception>
		/// <exception cref="System.ArithmeticException">
		/// This Object's value exceeds the range of a 32-bit
		/// signed integer.</exception>
		public int AsInt32() {
			if(this.getItemType()== CBORObjectType.Integer){
				if(((Long)item).longValue()>Integer.MAX_VALUE || ((Long)item).longValue()<Integer.MIN_VALUE)
					throw new ArithmeticException();
				return ((Long)item).intValue();
			} else if(this.getItemType()== CBORObjectType.BigInteger){
				if(((BigInteger)item).compareTo(BigInteger.valueOf(Integer.MAX_VALUE))>0 ||
				   ((BigInteger)item).compareTo(BigInteger.valueOf(Integer.MIN_VALUE))<0)
					throw new ArithmeticException();
				return ((BigInteger)item).intValue();
			} else if(this.getItemType()== CBORObjectType.Single){
				if(Float.isNaN(((Float)item).floatValue()) ||
				   ((Float)item).floatValue()>Integer.MAX_VALUE || ((Float)item).floatValue()<Integer.MIN_VALUE)
					throw new ArithmeticException();
				return ((Float)item).intValue();
			} else if(this.getItemType()== CBORObjectType.Double){
				if(Double.isNaN(((Double)item).doubleValue()) ||
				   ((Double)item).doubleValue()>Integer.MIN_VALUE || ((Double)item).doubleValue()<Integer.MIN_VALUE)
					throw new ArithmeticException();
				return ((Double)item).intValue();
			} else if(this.getTag().equals(BigInteger.valueOf(4)) && this.getItemType()== CBORObjectType.Array &&
			          this.size()==2){
				StringBuilder sb=new StringBuilder();
				sb.append(this.get(1).IntegerToString());
				sb.append("e");
				sb.append(this.get(0).IntegerToString());
				BigInteger bi=ParseBigIntegerWithExponent(sb.toString());
				if(bi.compareTo(BigInteger.valueOf(Integer.MAX_VALUE))>0 ||
				   bi.compareTo(BigInteger.valueOf(Integer.MIN_VALUE))<0)
					throw new ArithmeticException();
				return bi.intValue();
			} else
				throw new IllegalStateException("Not a number type");
		}
		public String AsString() {
			if(this.getItemType()== CBORObjectType.TextString){
				return (String)item;
			} else {
				throw new IllegalStateException("Not a String type");
			}
		}
		/// <summary>
		/// Reads an Object in CBOR format from a data
		/// stream.
		/// </summary>
		/// <param name="stream">A readable data stream.</param>
		/// <returns>a CBOR Object that was read.</returns>
		/// <exception cref="System.NullPointerException">
		/// <paramref name="stream"/> is null.</exception>
		public static CBORObject Read(InputStream stream) throws IOException {
			return Read(stream,0,false,-1,null,0);
		}
		
		private static void WriteObjectArray(
			List<CBORObject> list, OutputStream s) throws IOException {
			WritePositiveInt(4,list.size(),s);
			for(CBORObject i : list){
				if(i!=null && i.isBreak())
					throw new IllegalArgumentException();
				Write(i,s);
			}
		}
		
		private static void WriteObjectMap(
			Map<CBORObject,CBORObject> map, OutputStream s) throws IOException {
			WritePositiveInt(5,map.size(),s);
			for(CBORObject key : map.keySet()){
				if(key!=null && key.isBreak())
					throw new IllegalArgumentException();
				CBORObject value=map.get(key);
				if(value!=null && value.isBreak())
					throw new IllegalArgumentException();
				Write(key,s);
				Write(value,s);
			}
		}

		private static void WritePositiveInt(int type, int value, OutputStream s) throws IOException {
			if(value<0)
				throw new IllegalArgumentException();
			if(value<24){
				s.write((byte)((byte)value|(byte)(type<<5)));
			} else if(value<=0xFF){
				s.write((byte)(24|(type<<5)));
				s.write((byte)(value&0xFF));
			} else if(value<=0xFFFF){
				s.write((byte)(25|(type<<5)));
				s.write((byte)((value>>8)&0xFF));
				s.write((byte)(value&0xFF));
			} else {
				s.write((byte)(26|(type<<5)));
				s.write((byte)((value>>24)&0xFF));
				s.write((byte)((value>>16)&0xFF));
				s.write((byte)((value>>8)&0xFF));
				s.write((byte)(value&0xFF));
			}
		}
		
		private static void WritePositiveInt64(int type, long value, OutputStream s) throws IOException {
			if(value<0)
				throw new IllegalArgumentException();
			if(value<24){
				s.write((byte)((byte)value|(byte)(type<<5)));
			} else if(value<=0xFF){
				s.write((byte)(24|(type<<5)));
				s.write((byte)(value&0xFF));
			} else if(value<=0xFFFF){
				s.write((byte)(25|(type<<5)));
				s.write((byte)((value>>8)&0xFF));
				s.write((byte)(value&0xFF));
			} else if(value<=0xFFFFFFFF){
				s.write((byte)(26|(type<<5)));
				s.write((byte)((value>>24)&0xFF));
				s.write((byte)((value>>16)&0xFF));
				s.write((byte)((value>>8)&0xFF));
				s.write((byte)(value&0xFF));
			} else {
				s.write((byte)(27|(type<<5)));
				s.write((byte)((value>>56)&0xFF));
				s.write((byte)((value>>48)&0xFF));
				s.write((byte)((value>>40)&0xFF));
				s.write((byte)((value>>32)&0xFF));
				s.write((byte)((value>>24)&0xFF));
				s.write((byte)((value>>16)&0xFF));
				s.write((byte)((value>>8)&0xFF));
				s.write((byte)(value&0xFF));
			}
		}
		
		private static final int StreamedStringBufferLength=4096;
		
		private static void WriteStreamedString(String str, OutputStream stream) throws IOException {
			byte[] bytes=new byte[StreamedStringBufferLength];
			int byteIndex=0;
			boolean streaming=false;
			for(int index=0;index<str.length();index++){
				int c=str.charAt(index);
				if(c>=0xD800 && c<=0xDBFF && index+1<str.length() &&
				   str.charAt(index+1)>=0xDC00 && str.charAt(index+1)<=0xDFFF){
					// Get the Unicode code point for the surrogate pair
					c=0x10000+(c-0xD800)*0x400+(str.charAt(index+1)-0xDC00);
					index++;
				} else if(c>=0xD800 && c<=0xDFFF){
					// unpaired surrogate, write U+FFFD instead
					c=0xFFFD;
				}
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
				} else if(c<=0xFFFF){
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
			WritePositiveInt(3,byteIndex,stream);
			stream.write(bytes,0,byteIndex);
			if(streaming){
				stream.write((byte)0xFF);
			}
		}

		
		/// <summary>
		/// Writes a String in CBOR format to a data stream.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="s"></param>
		public static void Write(String str, OutputStream s) throws IOException {
			if((s)==null)throw new NullPointerException("s");
			if(str==null){
				s.write(0xf6); // Write null instead of String
			} else {
				WriteStreamedString(str,s);
			}
		}
		
		
		private static BigInteger LowestMajorType1=
			new BigInteger("-18446744073709551616");
		private static BigInteger UInt64MaxValue=
			new BigInteger("18446744073709551615");
		private static BigInteger FiftySixBitMask=BigInteger.valueOf(0xFFFFFFFFFFFFFFL);
		
		/// <summary>
		/// Writes a big integer in CBOR format to a data stream.
		/// </summary>
		/// <param name="bi">Big integer to write.</param>
		/// <param name="s">Stream to write to.</param>
		public static void Write(BigInteger bigint, OutputStream s) throws IOException {
			if((s)==null)throw new NullPointerException("s");
			int datatype=(bigint.signum()<0) ? 1 : 0;
			if(bigint.signum()<0){
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
if(ms!=null)ms.close();
}
			}
		}
		
/**
 * @deprecated This method will be removed in the future.  Use WriteTo(Stream) instead. 
 */
		public void Write(OutputStream s) throws IOException {
			WriteTo(s);
		}
		
		/// <summary>
		/// Writes this CBOR Object to a data stream.
		/// </summary>
		/// <param name="s">A readable data stream.</param>
		public void WriteTo(OutputStream s) throws IOException {
			if((s)==null)throw new NullPointerException("s");
			if(tagged){
				// Write the tag if this Object has one
				BigInteger tag=this.getTag();
				if(tag.compareTo(Int64MaxValue)<=0){
					WritePositiveInt64(6,tag.longValue(),s);
				} else {
					BigInteger mask=BigInteger.valueOf(0xFF);
					BigInteger tempBigInt=tag;
					byte[] intval=new byte[8];
					s.write((byte)(0xC0|27));
					for(int i=0;i<8;i++){
						BigInteger bi2=tempBigInt.and(mask);
						intval[7-i]=(byte)bi2.intValue();
						tempBigInt=tempBigInt.shiftRight(8);
					}
					s.write(intval,0,intval.length);
				}
			}
			if(this.getItemType()==CBORObjectType.Integer){
				Write(((Long)item).longValue(),s);
			} else if(this.getItemType()== CBORObjectType.BigInteger){
				Write((BigInteger)item,s);
			} else if(this.getItemType()== CBORObjectType.ByteString){
				WritePositiveInt((this.getItemType()== CBORObjectType.ByteString) ? 2 : 3,
				                 ((byte[])item).length,s);
				s.write(((byte[])item),0,((byte[])item).length);
			} else if(this.getItemType()== CBORObjectType.TextString ){
				Write((String)item,s);
			} else if(this.getItemType()== CBORObjectType.Array){
				WriteObjectArray(AsList(),s);
			} else if(this.getItemType()== CBORObjectType.Map){
				WriteObjectMap(AsMap(),s);
			} else if(this.getItemType()== CBORObjectType.SimpleValue){
				int value=((Integer)item).intValue();
				if(value<24 || value==31){
					s.write((byte)(0xE0+value));
				} else {
					s.write(0xF8);
					s.write((byte)value);
				}
			} else if(this.getItemType()== CBORObjectType.Single){
				Write(((Float)item).floatValue(),s);
			} else if(this.getItemType()== CBORObjectType.Double){
				Write(((Double)item).doubleValue(),s);
			} else {
				throw new IllegalArgumentException();
			}
		}
		
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
		public static void Write(int value, OutputStream s) throws IOException {
			Write((long)value,s);
		}
		public static void Write(short value, OutputStream s) throws IOException {
			Write((long)value,s);
		}
		public static void Write(char value, OutputStream s) throws IOException {
			Write(new String(new char[]{value}),s);
		}
		public static void Write(boolean value, OutputStream s) throws IOException {
			s.write(value ? (byte)0xf5 : (byte)0xf4);
		}
		public static void Write(byte value, OutputStream s) throws IOException {
			if((((int)value)&0xFF)<24){
				s.write(value);
			} else {
				s.write((byte)(24));
				s.write(value);
			}
		}
		public static void Write(float value, OutputStream s) throws IOException {
			if((s)==null)throw new NullPointerException("s");
			int bits=Float.floatToRawIntBits(
				value);
			s.write(0xFA);
			s.write((byte)((bits>>24)&0xFF));
			s.write((byte)((bits>>16)&0xFF));
			s.write((byte)((bits>>8)&0xFF));
			s.write((byte)(bits&0xFF));
		}
		public static void Write(double value, OutputStream s) throws IOException {
			long bits=Double.doubleToRawLongBits(
				(double)value);
			s.write(0xFB);
			s.write((byte)((bits>>56)&0xFF));
			s.write((byte)((bits>>48)&0xFF));
			s.write((byte)((bits>>40)&0xFF));
			s.write((byte)((bits>>32)&0xFF));
			s.write((byte)((bits>>24)&0xFF));
			s.write((byte)((bits>>16)&0xFF));
			s.write((byte)((bits>>8)&0xFF));
			s.write((byte)(bits&0xFF));
		}
		
		/// <summary>
		/// Gets the binary representation of this
		/// data item.
		/// </summary>
		/// <returns>A byte array in CBOR format.</returns>
		public byte[] ToBytes() throws IOException {
			java.io.ByteArrayOutputStream ms=null;
try {
ms=new ByteArrayOutputStream();

				WriteTo(ms);
				if(ms.size()>Integer.MAX_VALUE)
					throw new OutOfMemoryError();
				return ms.toByteArray();
}
finally {
if(ms!=null)ms.close();
}
		}
		
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
				Map<CBORObject,CBORObject> dic=
					(Map<CBORObject,CBORObject>)o;
				WritePositiveInt(5,dic.size(),s);
				for(CBORObject i : dic.keySet()){
					if(i!=null && i.isBreak())
						throw new IllegalArgumentException();
					CBORObject value=dic.get(i);
					if(value!=null && value.isBreak())
						throw new IllegalArgumentException();
					Write(i,s);
					Write(value,s);
				}
			} else if(o instanceof Map<?,?>){
				WriteObjectMap((Map<CBORObject,CBORObject>)o,s);
			} else {
				FromObject(o).WriteTo(s);
			}
		}
		
		//-----------------------------------------------------------
		
		/// <summary>
		/// Generates a CBOR Object from a String in JavaScript Object
		/// Notation (JSON) format.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static CBORObject FromJSONString(String str) {
			JSONTokener tokener=new JSONTokener(str,0);
			return ParseJSONObject(tokener);
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
				if(c=='\\' || c=='"' || c<0x20){
					sb.append('\\');
				}
				sb.append(c);
			}
			sb.append("\"");
			return sb.toString();
		}
		
		private BigInteger ParseBigIntegerWithExponent(String str) {
			return new BigDecimal(str).toBigInteger();
		}
		
		private String ExponentAndMantissaToString(CBORObject ex, CBORObject ma) {
			BigInteger exponent=ex.AsBigInteger();
			String mantissa=ma.IntegerToString();
			StringBuilder sb=new StringBuilder();
			BigInteger decimalPoint=BigInteger.valueOf((mantissa.length()));
			decimalPoint=decimalPoint.add(exponent);
			if(exponent.signum()<0 &&
			   decimalPoint.signum()>0){
				int pos=decimalPoint.intValue();
				sb.append(mantissa.substring(0,(0)+(pos)));
				sb.append(".");
				sb.append(mantissa.substring(pos));
			} else if(exponent.signum()<0 &&
			          decimalPoint.signum()==0){
				sb.append("0.");
				sb.append(mantissa);
			} else {
				sb.append(this.get(1).IntegerToString());
				if(!exponent.equals(BigInteger.ZERO)){
					sb.append("e");
					sb.append(this.get(0).IntegerToString());
				}
			}
			return sb.toString();
		}
		
		/// <summary>
		/// Converts this Object to a JSON String.  This function
		/// not only accepts arrays and maps (the only proper
		/// JSON objects under RFC 4627), but also integers,
		/// strings, byte arrays, and other JSON data types.
		/// </summary>
		public String ToJSONString() {
			if(this.getItemType()== CBORObjectType.SimpleValue){
				if(this.isTrue())return "true";
				else if(this.isFalse())return "false";
				else if(this.isNull())return "null";
				else return "null";
			} else if(this.getItemType()== CBORObjectType.Single){
				float f=((Float)item).floatValue();
				if(((f)==Float.NEGATIVE_INFINITY) ||
				   ((f)==Float.POSITIVE_INFINITY) ||
				   Float.isNaN(f)) return "null";
				else
					return Float.toString((float)f);
			} else if(this.getItemType()== CBORObjectType.Double){
				double f=((Double)item).doubleValue();
				if(((f)==Double.NEGATIVE_INFINITY) ||
				   ((f)==Double.POSITIVE_INFINITY) ||
				   Double.isNaN(f)) return "null";
				else
					return Double.toString((double)f);
			} else if(this.getItemType()== CBORObjectType.Integer){
				return Long.toString(((Long)item).longValue());
			} else if(this.getItemType()== CBORObjectType.BigInteger){
				return ((BigInteger)item).toString();
			} else if(this.getItemType()== CBORObjectType.ByteString){
				StringBuilder sb=new StringBuilder();
				sb.append('\"');
				if(this.getTag().equals(BigInteger.valueOf(22))){
					ToBase64(sb,(byte[])item,Base64,false);
				} else if(this.getTag().equals(BigInteger.valueOf(23))){
					ToBase16(sb,(byte[])item);
				} else {
					ToBase64(sb,(byte[])item,Base64URL,false);
				}
				sb.append('\"');
				return sb.toString();
			} else if(this.getItemType()== CBORObjectType.TextString){
				return StringToJSONString((String)item);
			} else if(this.getItemType()== CBORObjectType.Array){
				if(this.getTag().equals(BigInteger.valueOf(4)) && this.size()==2){
					return ExponentAndMantissaToString(this.get(0),this.get(1));
				} else {
					StringBuilder sb=new StringBuilder();
					boolean first=true;
					sb.append("[");
					for(CBORObject i : AsList()){
						if(!first)sb.append(", ");
						sb.append(i.ToJSONString());
						first=false;
					}
					sb.append("]");
					return sb.toString();
				}
			} else if(this.getItemType()== CBORObjectType.Map){
				HashMap<String,CBORObject> dict=new HashMap<String,CBORObject>();
				StringBuilder sb=new StringBuilder();
				boolean first=true;
				Map<CBORObject,CBORObject> dictitem=AsMap();
				for(CBORObject key : dictitem.keySet()){
					String str=(key.getItemType()== CBORObjectType.TextString) ?
						key.AsString() : key.ToJSONString();
					dict.put(str,dictitem.get(key));
				}
				sb.append("{");
				for(String key : dict.keySet()){
					if(!first)sb.append(", ");
					sb.append(StringToJSONString(key));
					sb.append(':');
					sb.append(dict.get(key).ToJSONString());
					first=false;
				}
				sb.append("}");
				return sb.toString();
			} else {
				throw new IllegalStateException();
			}
		}
		
		private static CBORObject ParseJSONNumber(String str) {
			if(((str)==null || (str).length()==0))
				return null;
			char c=str.charAt(0);
			boolean negative=false;
			int index=0;
			if(index>=str.length())
				return null;
			c=str.charAt(index);
			if(c=='-'){
				negative=true;
				index++;
			}
			int numberStart=index;
			if(index>=str.length())
				return null;
			c=str.charAt(index);
			index++;
			int numberEnd=index;
			int fracStart=-1;
			int fracEnd=-1;
			boolean negExp=false;
			int expStart=-1;
			int expEnd=-1;
			if(c>='1' && c<='9'){
				while(index<str.length()){
					c=str.charAt(index);
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
			if(index<str.length() && str.charAt(index)=='.'){
				// Fraction
				index++;
				fracStart=index;
				if(index>=str.length())
					return null;
				c=str.charAt(index);
				index++;
				fracEnd=index;
				if(c>='0' && c<='9'){
					while(index<str.length()){
						c=str.charAt(index);
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
			if(index<str.length() && (str.charAt(index)=='e' || str.charAt(index)=='E')){
				// Exponent
				index++;
				if(index>=str.length())
					return null;
				c=str.charAt(index);
				if(c=='-'){
					negative=true;
					index++;
				}
				if(c=='+')index++;
				expStart=index;
				if(index>=str.length())
					return null;
				c=str.charAt(index);
				index++;
				expEnd=index;
				if(c>='0' && c<='9'){
					while(index<str.length()){
						c=str.charAt(index);
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
				int value=Integer.parseInt(str.substring(numberStart,(numberStart)+(numberEnd-numberStart)));
				if(negative)value=-value;
				return FromObject(value);
			} else if(fracStart<0 && expStart<0){
				// Bigger integer
				BigInteger bigintValue=new BigInteger(
					str.substring(numberStart,(numberStart)+(numberEnd-numberStart)));
				if(negative)bigintValue=(bigintValue).negate();
				return FromObject(bigintValue);
			} else {
				BigInteger fracpart=(fracStart<0) ? BigInteger.ZERO : new BigInteger(
					str.substring(fracStart,(fracStart)+(fracEnd-fracStart)));
				// Intval consists of the whole and fractional part
				String intvalString=str.substring(numberStart,(numberStart)+(numberEnd-numberStart))+
					(fracpart.equals(BigInteger.ZERO) ? "" : str.substring(fracStart,(fracStart)+(fracEnd-fracStart)));
				BigInteger intval=new BigInteger(
					intvalString);
				if(negative)intval=intval.negate();
				if(fracpart.equals(BigInteger.ZERO) && expStart<0){
					// Zero fractional part and no exponent;
					// this is easy, just return the integer
					return FromObject(intval);
				}
				BigInteger exp=(expStart<0) ? BigInteger.ZERO : new BigInteger(
					str.substring(expStart,(expStart)+(expEnd-expStart)));
				if(negExp)exp=exp.negate();
				if(!fracpart.equals(BigInteger.ZERO)){
					// If there is a nonzero fractional part,
					// decrease the exponent by that part's length
					exp=exp.subtract(BigInteger.valueOf((fracEnd-fracStart)));
				}
				if(exp.equals(BigInteger.ZERO)){
					// If exponent is 0, this is also easy,
					// just return the integer
					return FromObject(intval);
				}
				if(exp.compareTo(UInt64MaxValue)>0 ||
				   exp.compareTo(LowestMajorType1)<0){
					// Exponent is lower than the lowest representable
					// integer of major type 1, or higher than the
					// highest representable integer of major type 0
					if(intval.equals(BigInteger.ZERO)){
						return FromObject(0);
					} else {
						return null;
					}
				}
				// Represent the CBOR Object as a decimal fraction
				return FromObjectAndTag(new CBORObject[]{
				                        	FromObject(exp),FromObject(intval)},4);
			}
		}
		
		// Based on the json.org implementation for JSONTokener
		private static CBORObject NextJSONValue(JSONTokener tokener) {
			int c = tokener.nextClean();
			String str;
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
				CBORObject obj=ParseJSONNumber(str);
				if(obj==null)
					throw tokener.syntaxError("JSON number can't be parsed.");
				return obj;
			}
			if (str.length() == 0)
				throw tokener.syntaxError("Missing value.");
			return FromObject(str);
		}

		// Based on the json.org implementation for JSONObject
		private static CBORObject ParseJSONObject(JSONTokener x) {
			int c;
			String key;
			CBORObject obj;
			c=x.nextClean();
			if(c=='['){
				x.back();
				return ParseJSONArray(x);
			}
			HashMap<String,CBORObject> myHashMap=new HashMap<String,CBORObject>();
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
						if(obj.getItemType()!= CBORObjectType.TextString)
							throw x.syntaxError("Expected a String as a key");
						key = obj.AsString();
						if((x.getOptions() & JSONTokener.OPTION_NO_DUPLICATES)!=0 &&
						   myHashMap.containsKey(key)){
							throw x.syntaxError("Key already exists: "+key);
						}
						break;
				}
				
				if (x.nextClean() != ':')
					throw x.syntaxError("Expected a ':' after a key");
				// NOTE: Will overwrite existing value. --Peter O.
				myHashMap.put(key, NextJSONValue(x));
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
		private static CBORObject ParseJSONArray(JSONTokener x) {
			ArrayList<CBORObject> myArrayList=new ArrayList<CBORObject>();
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

		private static String ReadUtf8(InputStream stream, int byteLength) throws IOException {
			StringBuilder builder=new StringBuilder();
			int cp=0;
			int bytesSeen=0;
			int bytesNeeded=0;
			int lower=0x80;
			int upper=0xBF;
			int pointer=0;
			while(pointer<byteLength || byteLength<0){
				int b=stream.read();
				if(b<0 && bytesNeeded!=0){
					bytesNeeded=0;
					throw new NumberFormatException("Invalid UTF-8");
				} else if(b<0){
					if(byteLength>0 && pointer>=byteLength)
						throw new NumberFormatException("Premature end of stream");
					break; // end of stream
				}
				if(byteLength>0) {
					pointer++;
				}
				if(bytesNeeded==0){
					if(b<0x80){
						builder.append((char)b);
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
						throw new NumberFormatException("Invalid UTF-8");
					cp<<=(6*bytesNeeded);
					continue;
				}
				if(b<lower || b>upper){
					cp=bytesNeeded=bytesSeen=0;
					lower=0x80;
					upper=0xbf;
					throw new NumberFormatException("Invalid UTF-8");
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
					builder.append((char)ret);
				} else {
					int ch=ret-0x10000;
					int lead=ch/0x400+0xd800;
					int trail=(ch&0x3FF)+0xdc00;
					builder.append((char)lead);
					builder.append((char)trail);
				}
			}
			return builder.toString();
		}
		
		
		//-----------------------------------------------------------
		public static CBORObject FromObject(long value) {
			return new CBORObject(CBORObjectType.Integer,value);
		}
		public static CBORObject FromObject(CBORObject value) {
			if(value==null)return CBORObject.Null;
			return value;
		}

		public static CBORObject FromObject(BigInteger bigintValue) {
			if(bigintValue.compareTo(Int64MinValue)>=0 &&
			   bigintValue.compareTo(Int64MaxValue)<=0){
				return new CBORObject(CBORObjectType.Integer,bigintValue.longValue());
			} else {
				return new CBORObject(CBORObjectType.BigInteger,bigintValue);
			}
		}
		public static CBORObject FromObject(String stringValue) {
			if(stringValue==null)return CBORObject.Null;
			if(!IsValidString(stringValue))
				throw new IllegalArgumentException("String contains an unpaired surrogate code point.");
			return new CBORObject(CBORObjectType.TextString,stringValue);
		}
		public static CBORObject FromObject(int value) {
			return FromObject((long)value);
		}
		public static CBORObject FromObject(short value) {
			return FromObject((long)value);
		}
		public static CBORObject FromObject(char value) {
			return FromObject(new String(new char[]{value}));
		}
		public static CBORObject FromObject(boolean value) {
			return (value ? CBORObject.True : CBORObject.False);
		}
		public static CBORObject FromObject(byte value) {
			return FromObject(((int)value)&0xFF);
		}
		public static CBORObject FromObject(float value) {
			return new CBORObject(CBORObjectType.Single,value);
		}
		public static CBORObject FromObject(double value) {
			return new CBORObject(CBORObjectType.Double,value);
		}
		public static CBORObject FromObject(byte[] value) {
			if(value==null)return CBORObject.Null;
			return new CBORObject(CBORObjectType.ByteString,value);
		}
		public static CBORObject FromObject(CBORObject[] array) {
			if(array==null)return CBORObject.Null;
			List<CBORObject> list=new ArrayList<CBORObject>();
			for(CBORObject i : array){
				CBORObject obj=FromObject(i);
				if(obj!=null && obj.isBreak())
					throw new IllegalArgumentException();
				list.add(obj);
			}
			return new CBORObject(CBORObjectType.Array,list);
		}
		public static <T> CBORObject FromObject(List<T> value){
			if(value==null)return CBORObject.Null;
			List<CBORObject> list=new ArrayList<CBORObject>();
			for(T i : (List<T>)value){
				CBORObject obj=FromObject(i);
				if(obj!=null && obj.isBreak())
					throw new IllegalArgumentException();
				list.add(obj);
			}
			return new CBORObject(CBORObjectType.Array,list);
		}
		public static <TKey, TValue> CBORObject FromObject(Map<TKey, TValue> dic){
			if(dic==null)return CBORObject.Null;
			HashMap<CBORObject,CBORObject> map=new HashMap<CBORObject,CBORObject>();
			for(TKey i : dic.keySet()){
				CBORObject key=FromObject(i);
				if(key!=null && key.isBreak())
					throw new IllegalArgumentException();
				CBORObject value=FromObject(dic.get(i));
				if(value!=null && value.isBreak())
					throw new IllegalArgumentException();
				map.put(key,value);
			}
			return new CBORObject(CBORObjectType.Map,map);
		}
		@SuppressWarnings("unchecked")
public static CBORObject FromObject(Object o) {
			if(o==null)return CBORObject.Null;
			if(o instanceof Long)return FromObject(((Long)o).longValue());
			if(o instanceof CBORObject)return FromObject((CBORObject)o);
			if(o instanceof BigInteger)return FromObject((BigInteger)o);
			if(o instanceof String)return FromObject((String)o);
			if(o instanceof Integer)return FromObject(((Integer)o).intValue());
			if(o instanceof Short)return FromObject(((Short)o).shortValue());
			if(o instanceof Character)return FromObject(((Character)o).charValue());
			if(o instanceof Boolean)return FromObject(((Boolean)o).booleanValue());
			if(o instanceof Byte)return FromObject(((Byte)o).byteValue());
			if(o instanceof Float)return FromObject(((Float)o).floatValue());
			
			
			
			
			
			
			
			if(o instanceof Double)return FromObject(((Double)o).doubleValue());
			if(o instanceof List<?>)return FromObject((List<CBORObject>)o);
			if(o instanceof byte[])return FromObject((byte[])o);
			if(o instanceof CBORObject[])return FromObject((CBORObject[])o);
			if(o instanceof Map<?,?>)return FromObject(
				(Map<CBORObject,CBORObject>)o);
			if(o instanceof Map<?,?>)return FromObject(
				(Map<String,CBORObject>)o);
			throw new IllegalArgumentException();
		}
		
		private static BigInteger BigInt65536=BigInteger.valueOf(65536);

		public static CBORObject FromObjectAndTag(Object o, BigInteger bigintTag) {
			if((bigintTag).signum()<0)throw new IllegalArgumentException("tag"+" not greater or equal to 0 ("+bigintTag.toString()+")");
			if((bigintTag).compareTo(UInt64MaxValue)>0)throw new IllegalArgumentException("tag"+" not less or equal to 18446744073709551615 ("+bigintTag.toString()+")");
			CBORObject c=FromObject(o);
			if(bigintTag.compareTo(BigInt65536)<0){
				// Low-numbered, commonly used tags
				return new CBORObject(c.getItemType(),bigintTag.intValue(),0,c.item);
			} else {
				BigInteger tmpbigint=bigintTag.and(BigInteger.valueOf(0xFFFF));
				int low=(tmpbigint.intValue());
				tmpbigint=(bigintTag.shiftRight(16)).and(BigInteger.valueOf(0xFFFF));
				low=(tmpbigint.intValue())<<16;
				tmpbigint=(bigintTag.shiftRight(32)).and(BigInteger.valueOf(0xFFFF));
				int high=(tmpbigint.intValue());
				tmpbigint=(bigintTag.shiftRight(48)).and(BigInteger.valueOf(0xFFFF));
				high=(tmpbigint.intValue())<<16;
				return new CBORObject(c.getItemType(),low,high,c.item);
			}
		}

		public static CBORObject FromObjectAndTag(Object o, int intTag) {
			if(intTag<0)throw new IllegalArgumentException(
				"tag not greater or equal to 0 ("+
				Integer.toString((int)intTag)+")");
			CBORObject c=FromObject(o);
			return new CBORObject(c.getItemType(),intTag,0,c.item);
		}
		
		//-----------------------------------------------------------
		
		private String IntegerToString() {
			if(this.getItemType()== CBORObjectType.Integer){
				long v=((Long)item).longValue();
				return Long.toString((long)v);
			} else if(this.getItemType()== CBORObjectType.BigInteger){
				return ((BigInteger)item).toString();
			} else {
				throw new IllegalStateException();
			}
		}
		
		/// <summary>
		/// Returns this CBOR Object in String form.
		/// The format is intended to be human-readable, not machine-
		/// parsable, and the format may change at any time.
		/// </summary>
		/// <returns>A text representation of this Object.</returns>
		@Override public String toString() {
			StringBuilder sb=new StringBuilder();
			if(tagged){
				sb.append(this.getTag().toString());
				sb.append('(');
			}
			if(this.getItemType()== CBORObjectType.SimpleValue){
				if(this.isBreak())sb.append("break");
				else if(this.isTrue())sb.append("true");
				else if(this.isFalse())sb.append("false");
				else if(this.isNull())sb.append("null");
				else if(this.isUndefined())sb.append("undefined");
				else {
					sb.append("simple(");
					sb.append(Integer.toString(((Integer)item).intValue()));
					sb.append(")");
				}
			} else if(this.getItemType()== CBORObjectType.Single){
				float f=((Float)item).floatValue();
				if(((f)==Float.NEGATIVE_INFINITY))
					sb.append("-Infinity");
				else if(((f)==Float.POSITIVE_INFINITY))
					sb.append("Infinity");
				else if(Float.isNaN(f))
					sb.append("NaN");
				else
					sb.append(Float.toString((float)f));
			} else if(this.getItemType()== CBORObjectType.Double){
				double f=((Double)item).doubleValue();
				if(((f)==Double.NEGATIVE_INFINITY))
					sb.append("-Infinity");
				else if(((f)==Double.POSITIVE_INFINITY))
					sb.append("Infinity");
				else if(Double.isNaN(f))
					sb.append("NaN");
				else
					sb.append(Double.toString((double)f));
			} else if(this.getItemType()== CBORObjectType.Integer){
				long v=((Long)item).longValue();
				sb.append(Long.toString((long)v));
			} else if(this.getItemType()== CBORObjectType.BigInteger){
				sb.append(((BigInteger)item).toString());
			} else if(this.getItemType()== CBORObjectType.ByteString){
				sb.append("h'");
				byte[] data=(byte[])item;
				ToBase16(sb,data);
				sb.append("'");
			} else if(this.getItemType()== CBORObjectType.TextString){
				sb.append("\"");
				sb.append((String)item);
				sb.append("\"");
			} else if(this.getItemType()== CBORObjectType.Array){
				boolean first=true;
				sb.append("[");
				for(CBORObject i : AsList()){
					if(!first)sb.append(", ");
					sb.append(i.toString());
					first=false;
				}
				sb.append("]");
			} else if(this.getItemType()== CBORObjectType.Map){
				boolean first=true;
				sb.append("{");
				Map<CBORObject,CBORObject> map=AsMap();
				for(CBORObject key : map.keySet()){
					if(!first)sb.append(", ");
					sb.append(key.toString());
					sb.append(": ");
					sb.append(map.get(key).toString());
					first=false;
				}
				sb.append("}");
			}
			if(tagged){
				sb.append(")");
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
				throw new IOException();
			int c=s.read();
			if(c<0)
				throw new IOException();
			int type=(c>>5)&0x07;
			int additional=(c&0x1F);
			if(c==0xFF){
				if(allowBreak)return Break;
				throw new NumberFormatException();
			}
			if(allowOnlyType>=0 &&
			   (allowOnlyType!=type || additional>=28)){
				throw new NumberFormatException();
			}
			if(validTypeFlags!=null){
				// Check for valid major types if asked
				if(!CheckMajorTypeIndex(type,validTypeIndex,validTypeFlags)){
					throw new NumberFormatException();
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
					c=s.read();
					if(c<0)
						throw new IOException();
					return new CBORObject(CBORObjectType.SimpleValue,c);
				}
				if(additional==25 || additional==26 ||
				   additional==27){
					int cslength=(additional==25) ? 2 :
						((additional==26) ? 4 : 8);
					byte[] cs=new byte[cslength];
					if(s.read(cs,0,cs.length)!=cs.length)
						throw new IOException();
					long x=0;
					for(int i=0;i<cs.length;i++){
						x<<=8;
						x|=((long)cs[i])&0xFF;
					}
					if(additional==25){
						float f=HalfPrecisionToSingle(
							((int)x));
						return new CBORObject(CBORObjectType.Single,f);
					} else if(additional==26){
						float f=Float.intBitsToFloat(
							((int)x));
						return new CBORObject(CBORObjectType.Single,f);
					} else if(additional==27){
						double f=Double.longBitsToDouble(
							(x));
						return new CBORObject(CBORObjectType.Double,f);
					}
				}
				throw new NumberFormatException();
			}
			long uadditional=0;
			BigInteger bigintAdditional=BigInteger.ZERO;
			boolean hasBigAdditional=false;
			if(additional<=23){
				uadditional=(long)additional;
			} else if(additional==24){
				int c1=s.read();
				if(c1<0)
					throw new IOException();
				uadditional=(long)c1;
			} else if(additional==25 || additional==26){
				byte[] cs=new byte[(additional==25) ? 2 : 4];
				if(s.read(cs,0,cs.length)!=cs.length)
					throw new IOException();
				long x=0;
				for(int i=0;i<cs.length;i++){
					x<<=8;
					x|=((long)cs[i])&0xFF;
				}
				uadditional=x;
			} else if(additional==27){
				byte[] cs=new byte[8];
				if(s.read(cs,0,cs.length)!=cs.length)
					throw new IOException();
				long x=0;
				for(int i=0;i<cs.length;i++){
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
					bigintAdditional=BigInteger.valueOf(x);
					// Include the first and highest byte
					int firstByte=((int)cs[0])&0xFF;
					BigInteger bigintTemp=BigInteger.valueOf(firstByte);
					bigintTemp=bigintTemp.shiftLeft(56);
					bigintAdditional=bigintAdditional.or(bigintTemp);
				} else {
					uadditional=x;
				}
			} else if(additional==28 || additional==29 ||
			          additional==30){
				throw new NumberFormatException();
			} else if(additional==31 && type<2){
				throw new NumberFormatException();
			}
			if(type==0){
				if(hasBigAdditional)
					return FromObject(bigintAdditional);
				else
					return FromObject(uadditional);
			} else if(type==1){
				if(hasBigAdditional){
					bigintAdditional=BigInteger.valueOf(-1).subtract(bigintAdditional);
					return FromObject(bigintAdditional);
				} else if(uadditional<=Long.MAX_VALUE){
					return FromObject(((long)-1-(long)uadditional));
				} else {
					BigInteger bi=BigInteger.valueOf(-1);
					bi=bi.subtract(BigInteger.valueOf((uadditional)));
					return FromObject(bi);
				}
			} else if(type==2){ // Byte String
				if(additional==31){
					// Streaming byte String
					java.io.ByteArrayOutputStream ms=null;
try {
ms=new ByteArrayOutputStream();

						byte[] data;
						// Requires same type as this one
						int[] subFlags=new int[]{(1<<type)};
						while(true){
							CBORObject o=Read(s,depth+1,true,type,subFlags,0);
							if(o.isBreak())
								break;
							data=(byte[])o.item;
							ms.write(data,0,data.length);
						}
						if(ms.size()>Integer.MAX_VALUE)
							throw new IOException();
						data=ms.toByteArray();
						return new CBORObject(
							CBORObjectType.ByteString,
							data);
}
finally {
if(ms!=null)ms.close();
}
				} else {
					if(hasBigAdditional || uadditional>Integer.MAX_VALUE){
						throw new IOException();
					}
					byte[] data=new byte[(int)uadditional];
					if(s.read(data,0,data.length)!=data.length)
						throw new IOException();
					return new CBORObject(
						CBORObjectType.ByteString,
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
						if(o.isBreak())
							break;
						builder.append((String)o.item);
					}
					return new CBORObject(
						CBORObjectType.TextString,
						builder.toString());
				} else {
					if(hasBigAdditional || uadditional>=Integer.MAX_VALUE){
						throw new IOException();
					}
					String str=ReadUtf8(s,(int)uadditional);
					return new CBORObject(CBORObjectType.TextString,str);
				}
			} else if(type==4){ // Array
				List<CBORObject> list=new ArrayList<CBORObject>();
				int vtindex=1;
				if(additional==31){
					while(true){
						CBORObject o=Read(s,depth+1,true,-1,validTypeFlags,vtindex);
						if(o.isBreak())
							break;
						list.add(o);
						vtindex++;
					}
					return new CBORObject(CBORObjectType.Array,list);
				} else {
					if(hasBigAdditional || uadditional>=Integer.MAX_VALUE){
						throw new IOException();
					}
					for(long i=0;i<uadditional;i++){
						list.add(Read(s,depth+1,false,-1,validTypeFlags,vtindex));
						vtindex++;
					}
					return new CBORObject(CBORObjectType.Array,list);
				}
			} else if(type==5){ // Map, type 5
				HashMap<CBORObject,CBORObject> dict=new HashMap<CBORObject,CBORObject>();
				if(additional==31){
					while(true){
						CBORObject key=Read(s,depth+1,true,-1,null,0);
						if(key.isBreak())
							break;
						CBORObject value=Read(s,depth+1,false,-1,null,0);
						dict.put(key,value);
					}
					return new CBORObject(CBORObjectType.Map,dict);
				} else {
					if(hasBigAdditional || uadditional>=Integer.MAX_VALUE){
						throw new IOException();
					}
					for(long i=0;i<uadditional;i++){
						CBORObject key=Read(s,depth+1,false,-1,null,0);
						CBORObject value=Read(s,depth+1,false,-1,null,0);
						dict.put(key,value);
					}
					return new CBORObject(CBORObjectType.Map,dict);
				}
			} else { // Tagged item
				CBORObject o;
				if(!hasBigAdditional){
					if(uadditional==0){
						// Requires a text String
						int[] subFlags=new int[]{(1<<3)};
						o=Read(s,depth+1,allowBreak,-1,subFlags,0);
					} else if(uadditional==2 || uadditional==3){
						// Big number
						// Requires a byte String
						int[] subFlags=new int[]{(1<<2)};
						o=Read(s,depth+1,allowBreak,-1,subFlags,0);
						byte[] data=(byte[])o.item;
						BigInteger bi=BigInteger.ZERO;
						for(int i=0;i<data.length;i++){
							bi=bi.shiftLeft(8);
							int x=((int)data[i])&0xFF;
							bi=bi.or(BigInteger.valueOf(x));
						}
						if(uadditional==3){
							bi=BigInteger.valueOf(-1).subtract(bi); // Convert to a negative
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
						if(o.size()!=2) // Requires 2 items
							throw new NumberFormatException();
						// check type of mantissa
						List<CBORObject> list=o.AsList();
						if(list.get(1).getItemType()!=CBORObjectType.Integer &&
						   list.get(1).getItemType()!=CBORObjectType.BigInteger)
							throw new NumberFormatException();
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
						o,BigInteger.valueOf(uadditional));
				}
			}
		}
	}
