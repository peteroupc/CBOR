/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

using System;
using System.Text;
using System.IO;
using System.Numerics;
using System.Globalization;

namespace PeterO
{
	/// <summary>
	/// Contains methods useful for reading and writing
	/// strings and parsing numbers.
	/// </summary>
	public static class CBORDataUtilities
	{
		private static BigInteger LowestMajorType1=
			BigInteger.Parse("-18446744073709551616",NumberStyles.AllowLeadingSign,
			                 CultureInfo.InvariantCulture);
		private static BigInteger UInt64MaxValue=
			BigInteger.Parse("18446744073709551615",NumberStyles.AllowLeadingSign,
			                 CultureInfo.InvariantCulture);

		private static int StreamedStringBufferLength=4096;
		
		/// <summary>
		/// Generates a text string from a UTF-8 byte array.
		/// </summary>
		/// <param name="bytes">A byte array containing text
		/// encoded in UTF-8.</param>
		/// <param name="replace">If true, replaces invalid encoding
		/// with the replacement character (U+FFFD).  If false,
		/// stops processing when invalid UTF-8 is seen.</param>
		/// <returns>A string represented by the UTF-8 byte array.</returns>
		/// <exception cref="System.ArgumentNullException">"bytes" is null.</exception>
		/// <exception cref="System.ArgumentException">The string
		/// is not valid UTF-8 and "replace" is false</exception>
		public static string GetUtf8String(byte[] bytes, bool replace){
			StringBuilder b=new StringBuilder();
			if(ReadUtf8FromBytes(bytes,0,bytes.Length,b,replace)!=0)
				throw new ArgumentException("Invalid UTF-8");
			return b.ToString();
		}

		/// <summary>
		/// Generates a text string from a portion of a UTF-8 byte array.
		/// </summary>
		/// <param name="bytes">A byte array containing text
		/// encoded in UTF-8.</param>
		/// <param name="offset">Offset into the byte array to start reading</param>
		/// <param name="byteLength">Length, in bytes, of the UTF-8 string</param>
		/// <param name="replace">If true, replaces invalid encoding
		/// with the replacement character (U+FFFD).  If false,
		/// stops processing when invalid UTF-8 is seen.</param>
		/// <returns>A string represented by the UTF-8 byte array.</returns>
		/// <exception cref="System.ArgumentNullException">"bytes" is null.</exception>
		/// <exception cref="System.ArgumentException">The portion of the byte array
		/// is not valid UTF-8 and "replace" is false</exception>
		public static string GetUtf8String(byte[] bytes, int offset, int byteLength, bool replace){
			StringBuilder b=new StringBuilder();
			if(ReadUtf8FromBytes(bytes,offset,byteLength,b,replace)!=0)
				throw new ArgumentException("Invalid UTF-8");
			return b.ToString();
		}
		
		/// <summary>
		/// Encodes a string in UTF-8 as a byte array.
		/// </summary>
		/// <param name="str">A text string.</param>
		/// <param name="replace">If true, replaces unpaired surrogate
		/// code points with the replacement character (U+FFFD).  If false,
		/// stops processing when an unpaired surrogate code point is seen.</param>
		/// <returns>The string encoded in UTF-8.</returns>
		/// <exception cref="System.ArgumentNullException">"str" is null.</exception>
		/// <exception cref="System.ArgumentException">The string contains
		/// an unpaired surrogate code point
		/// and "replace" is false, or an internal error occurred.</exception>
		public static byte[] GetUtf8Bytes(string str, bool replace){
			try {
				using(MemoryStream ms=new MemoryStream()){
					if(WriteUtf8(str,ms,replace)!=0)
						throw new ArgumentException("Unpaired surrogate code point");
					return ms.ToArray();
				}
			} catch(IOException ex){
				throw new ArgumentException("I/O error occurred",ex);
			}
		}
		
		/// <summary>
		/// Calculates the number of bytes needed to encode a string
		/// in UTF-8.
		/// </summary>
		/// <param name="s">A Unicode string.</param>
		/// <param name="replace">If true, treats unpaired
		/// surrogate code points as replacement characters (U+FFFD) instead,
		/// meaning each one takes 3 UTF-8 bytes.  If false, stops
		/// processing when an unpaired surrogate code point is reached.</param>
		/// <returns>The number of bytes needed to encode the given string
		/// in UTF-8, or -1 if the string contains an unpaired surrogate
		/// code point and "replace" is false.</returns>
		/// <exception cref="System.ArgumentNullException">"s" is null.</exception>
		public static long GetUtf8Length(String s, bool replace){
			if(s==null)throw new ArgumentNullException();
			long size=0;
			for(int i=0;i<s.Length;i++){
				int c=s[i];
				if(c<=0x7F) {
					size++;
				} else if(c<=0x7FF) {
					size+=2;
				} else if(c<=0xD7FF || c>=0xE000) {
					size+=3;
				} else if(c<=0xDBFF){ // UTF-16 low surrogate
					i++;
					if(i>=s.Length || s[i]<0xDC00 || s[i]>0xDFFF){
						if(replace)size+=3;
						else return -1;
					} else {
						size+=4;
					}
				} else {
					if(replace)size+=3;
					else return -1;
				}
			}
			return size;
		}

		
		/// <summary>
		/// Writes a string in UTF-8 encoding to a data stream.
		/// </summary>
		/// <param name="str">A string to write.</param>
		/// <param name="stream">A writable data stream.</param>
		/// <param name="replace">If true, replaces unpaired surrogate
		/// code points with the replacement character (U+FFFD).  If false,
		/// stops processing when an unpaired surrogate code point is seen.</param>
		/// <returns>0 if the entire string was written; or -1 if the
		/// string contains an unpaired surrogate code point and "replace"
		/// is false.</returns>
		public static int WriteUtf8(String str, Stream stream, bool replace){
			if((str)==null)throw new ArgumentNullException("str");
			if((stream)==null)throw new ArgumentNullException("stream");
			byte[] bytes;
			int retval=0;
			bytes=new byte[StreamedStringBufferLength];
			int byteIndex=0;
			for(int index=0;index<str.Length;index++){
				int c=str[index];
				if(c<=0x7F){
					if(byteIndex+1>StreamedStringBufferLength){
						// Write bytes retrieved so far
						stream.Write(bytes,0,byteIndex);
						byteIndex=0;
					}
					bytes[byteIndex++]=(byte)c;
				} else if(c<=0x7FF){
					if(byteIndex+2>StreamedStringBufferLength){
						// Write bytes retrieved so far
						stream.Write(bytes,0,byteIndex);
						byteIndex=0;
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
						// unpaired surrogate
						if(!replace){
							retval=-1;
							break; // write bytes read so far
						}
						c=0xFFFD;
					}
					if(c<=0xFFFF){
						if(byteIndex+3>StreamedStringBufferLength){
							// Write bytes retrieved so far
							stream.Write(bytes,0,byteIndex);
							byteIndex=0;
						}
						bytes[byteIndex++]=((byte)(0xE0|((c>>12)&0x0F)));
						bytes[byteIndex++]=((byte)(0x80|((c>>6 )&0x3F)));
						bytes[byteIndex++]=((byte)(0x80|(c      &0x3F)));
					} else {
						if(byteIndex+4>StreamedStringBufferLength){
							// Write bytes retrieved so far
							stream.Write(bytes,0,byteIndex);
							byteIndex=0;
						}
						bytes[byteIndex++]=((byte)(0xF0|((c>>18)&0x07)));
						bytes[byteIndex++]=((byte)(0x80|((c>>12)&0x3F)));
						bytes[byteIndex++]=((byte)(0x80|((c>>6 )&0x3F)));
						bytes[byteIndex++]=((byte)(0x80|(c      &0x3F)));
					}
				}
			}
			stream.Write(bytes,0,byteIndex);
			return retval;
		}
		
		/// <summary>
		/// Reads a string in UTF-8 encoding from a byte array.
		/// </summary>
		/// <param name="data">A byte array containing a UTF-8 string</param>
		/// <param name="offset">Offset into the byte array to start reading</param>
		/// <param name="byteLength">Length, in bytes, of the UTF-8 string</param>
		/// <param name="builder">A string builder object where the resulting
		/// string will be stored.</param>
		/// <param name="replace">If true, replaces invalid encoding
		/// with the replacement character (U+FFFD).  If false,
		/// stops processing when invalid UTF-8 is seen.</param>
		/// <returns>0 if the entire string was read without errors, or -1 if the string
		/// is not valid UTF-8 and "replace" is false.</returns>
		/// <exception cref="System.ArgumentNullException">"data" is null or "builder" is null.</exception>
		/// <exception cref="System.ArgumentException">"offset" is less than 0, "byteLength"
		/// is less than 0, or offset plus byteLength is greater than the length of "data".</exception>
		public static int ReadUtf8FromBytes(byte[] data, int offset, int byteLength,
		                                    StringBuilder builder,
		                                    bool replace)  {
			if((data)==null)throw new ArgumentNullException("data");
			if((offset)<0)throw new ArgumentOutOfRangeException("offset"+" not greater or equal to "+"0"+" ("+Convert.ToString((int)offset,System.Globalization.CultureInfo.InvariantCulture)+")");
			if((byteLength)<0)throw new ArgumentOutOfRangeException("byteLength"+" not greater or equal to "+"0"+" ("+Convert.ToString((int)byteLength,System.Globalization.CultureInfo.InvariantCulture)+")");
			if((offset+byteLength)>data.Length)throw new ArgumentOutOfRangeException(
				"offset+byteLength not less or equal to "+Convert.ToString((int)data.Length)+" ("+Convert.ToString((int)(offset+byteLength))+")");
			if((builder)==null)throw new ArgumentNullException("builder");
			int cp=0;
			int bytesSeen=0;
			int bytesNeeded=0;
			int lower=0x80;
			int upper=0xBF;
			int pointer=offset;
			int endpointer=offset+byteLength;
			while(pointer<endpointer){
				int b=(data[pointer]&(int)0xFF);
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
					} else {
						if(replace)
							builder.Append((char)0xFFFD);
						else
							return -1;
					}
					continue;
				}
				if(b<lower || b>upper){
					cp=bytesNeeded=bytesSeen=0;
					lower=0x80;
					upper=0xbf;
					if(replace){
						pointer--;
						builder.Append((char)0xFFFD);
						continue;
					} else {
						return -1;
					}
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
			if(bytesNeeded!=0){
				if(replace)
					builder.Append((char)0xFFFD);
				else
					return -1;
			}
			return 0;
		}


		/// <summary>
		/// Reads a string in UTF-8 encoding from a data stream.
		/// </summary>
		/// <param name="stream">A readable data stream.</param>
		/// <param name="byteLength">The length, in bytes, of the string.
		/// If this is less than 0, this function will read until the end
		/// of the stream.</param>
		/// <param name="builder">A string builder object where the resulting
		/// string will be stored.</param>
		/// <param name="replace">If true, replaces invalid encoding
		/// with the replacement character (U+FFFD).  If false,
		/// stops processing when an unpaired surrogate code point is seen.</param>
		/// <returns>0 if the entire string was read without errors,
		/// -1 if the string is not valid UTF-8 and "replace" is false (even if
		/// the end of the stream is reached), or -2 if the end
		/// of the stream was reached before the entire string was
		/// read.
		/// </returns>
		/// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
		/// <exception cref="System.ArgumentNullException">"stream" is null
		/// or "builder" is null.</exception>
		public static int ReadUtf8(Stream stream, int byteLength, StringBuilder builder,
		                           bool replace)  {
			if((stream)==null)throw new ArgumentNullException("stream");
			if((builder)==null)throw new ArgumentNullException("builder");
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
						if(replace){
							builder.Append((char)0xFFFD);
							if(byteLength>=0)
								return -2;
							break; // end of stream
						}
						return -1;
					} else {
						if(byteLength>=0)
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
					} else {
						if(replace)
							builder.Append((char)0xFFFD);
						else
							return -1;
					}
					continue;
				}
				if(b<lower || b>upper){
					cp=bytesNeeded=bytesSeen=0;
					lower=0x80;
					upper=0xbf;
					if(replace){
						builder.Append((char)0xFFFD);
						// "Read" the last byte again
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
						} else {
							builder.Append((char)0xFFFD);
						}
						continue;
					} else {
						return -1;
					}
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
			if(bytesNeeded!=0){
				if(replace)
					builder.Append((char)0xFFFD);
				else
					return -1;
			}
			return 0;
		}
		
		/// <summary>
		/// Parses a number whose format follows the JSON specification
		/// (RFC 4627).  Roughly speaking, a valid number consists of
		/// an optional minus sign, one or more digits (starting
		/// with 1 to 9 unless the only digit is 0), an optional
		/// decimal point with one or more digits, and an optional
		/// letter E or e with one or more digits (the exponent).
		/// </summary>
		/// <param name="str">A string to parse.</param>
		/// <param name="integersOnly">If true, no decimal points
		/// are allowed in the string.</param>
		/// <param name="positiveOnly">If true, only positive numbers
		/// are allowed (the leading minus is disallowed).</param>
		/// <returns>A CBOR object that represents the parsed
		/// number, or null if the exponent is less than -(2^64)
		/// or greater than 2^64-1 or if the entire string does
		/// not represent a valid number.</returns>
		public static CBORObject ParseJSONNumber(string str,
		                                         bool integersOnly,
		                                         bool positiveOnly){
			if(String.IsNullOrEmpty(str))
				return null;
			char c=str[0];
			bool negative=false;
			int index=0;
			if(index>=str.Length)
				return null;
			c=str[index];
			if(c=='-' && !positiveOnly){
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
			if(!integersOnly){
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
			}
			if(index!=str.Length){
				// End of the string wasn't reached, so isn't a number
				return null;
			}
			if(fracStart<0 && expStart<0 && (numberEnd-numberStart)<=9){
				// Common case: small integer
				int value=smallNumber;
				if(negative)value=-value;
				return CBORObject.FromObject(value);
			} if(fracStart<0 && expStart<0 && (numberEnd-numberStart)<=18){
				// Common case: long-sized integer
				string strsub=(numberStart==0 && numberEnd==str.Length) ? str :
					str.Substring(numberStart,numberEnd-numberStart);
				long value=Int64.Parse(strsub,
				                       NumberStyles.None,
				                       CultureInfo.InvariantCulture);
				if(negative)value=-value;
				return CBORObject.FromObject(value);
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
					return CBORObject.FromObject(int32val);
				}
				// Represent the CBOR object as a decimal fraction
				return CBORObject.FromObjectAndTag(new CBORObject[]{
				                                   	CBORObject.FromObject(exp),CBORObject.FromObject(int32val)},4);
			} else if(fracStart<0 && expStart<0){
				// Bigger integer
				string strsub=(numberStart==0 && numberEnd==str.Length) ? str :
					str.Substring(numberStart,numberEnd-numberStart);
				BigInteger bigintValue=BigInteger.Parse(strsub,
				                                        NumberStyles.None,
				                                        CultureInfo.InvariantCulture);
				if(negative)bigintValue=-(BigInteger)bigintValue;
				return CBORObject.FromObject(bigintValue);
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
					return CBORObject.FromObject(intval);
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
					return CBORObject.FromObject(intval);
				}
				if(exp.CompareTo(UInt64MaxValue)>0 ||
				   exp.CompareTo(LowestMajorType1)<0){
					// Exponent is lower than the lowest representable
					// integer of major type 1, or higher than the
					// highest representable integer of major type 0
					if(intval.IsZero){
						return CBORObject.FromObject(0);
					} else {
						return null;
					}
				}
				// Represent the CBOR object as a decimal fraction
				return CBORObject.FromObjectAndTag(new CBORObject[]{
				                                   	CBORObject.FromObject(exp),CBORObject.FromObject(intval)},4);
			}
		}
	}
}