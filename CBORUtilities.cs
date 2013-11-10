/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;
using System.Numerics;

namespace PeterO
{
	/// <summary>
	/// Contains utility methods that may
	/// have use outside of the CBORObject class.
	/// </summary>
	internal static class CBORUtilities
	{
		private static readonly string Base64URL="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
		private static readonly string Base64="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
		public static void ToBase64(StringBuilder str, byte[] data, bool padding){
			ToBase64(str,data,Base64,padding);
		}
		public static void ToBase64URL(StringBuilder str, byte[] data, bool padding){
			ToBase64(str,data,Base64URL,padding);
		}
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
		private static readonly string HexAlphabet="0123456789ABCDEF";
		public static void ToBase16(StringBuilder str, byte[] data){
			int length = data.Length;
			for (int i = 0; i < length;i++) {
				str.Append(HexAlphabet[(data[i]>>4)&15]);
				str.Append(HexAlphabet[data[i]&15]);
			}
		}
		
				
		public static BigInteger BigIntegerFromSingle(float flt){
			int value=ConverterInternal.SingleToInt32Bits(flt);
			int fpexponent=(int)((value>>23) & 0xFF);
			if(fpexponent==255)
				throw new OverflowException("Value is infinity or NaN");
			int mantissa = value & 0x7FFFFF;
			if (fpexponent==0)fpexponent++;
			else mantissa|=(1<<23);
			if(mantissa==0)return BigInteger.Zero;
			fpexponent-=150;
			while((mantissa&1)==0){
				fpexponent++;
				mantissa>>=1;
			}
			bool neg=((value>>31)!=0);
			if(fpexponent==0){
				if(neg)mantissa=-mantissa;
				return (BigInteger)mantissa;
			} else if(fpexponent>0){
				// Value is an integer
				BigInteger bigmantissa=(BigInteger)mantissa;
				bigmantissa<<=fpexponent;
				if(neg)bigmantissa=-(BigInteger)bigmantissa;
				return bigmantissa;
			} else {
				// Value has a fractional part
				int exp=-fpexponent;
				for(int i=0;i<exp && mantissa!=0;i++){
					mantissa>>=1;
				}
				return (BigInteger)mantissa;
			}
		}

		public static BigInteger BigIntegerFromDouble(double dbl){
			long value=ConverterInternal.DoubleToInt64Bits(dbl);
			int fpexponent=(int)((value>>52) & 0x7ffL);
			if(fpexponent==2047)
				throw new OverflowException("Value is infinity or NaN");
			long mantissa = value & 0xFFFFFFFFFFFFFL;
			if (fpexponent==0)fpexponent++;
			else mantissa|=(1L<<52);
			if(mantissa==0)return BigInteger.Zero;
			fpexponent-=1075;
			while((mantissa&1)==0){
				fpexponent++;
				mantissa>>=1;
			}
			bool neg=((value>>63)!=0);
			if(fpexponent==0){
				if(neg)mantissa=-mantissa;
				return (BigInteger)mantissa;
			} else if(fpexponent>0){
				// Value is an integer
				BigInteger bigmantissa=(BigInteger)mantissa;
				bigmantissa<<=fpexponent;
				if(neg)bigmantissa=-(BigInteger)bigmantissa;
				return bigmantissa;
			} else {
				// Value has a fractional part
				int exp=-fpexponent;
				for(int i=0;i<exp && mantissa!=0;i++){
					mantissa>>=1;
				}
				return (BigInteger)mantissa;
			}
		}
		
		public static float HalfPrecisionToSingle(int value){
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
	}
}
