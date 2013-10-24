/*
Written by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
*/

using System;
namespace PeterO
{
	/// <summary>
	/// Contains static methods for converting between different representations of data types.
	/// </summary>
	public static class Converter
	{
		/// <summary>
		/// Converts a single-precision floating point number to its bit form as a 32-bit integer.
		/// </summary>
		/// <param name="value">A single-precision number.</param>
		/// <returns>A 32-bit form of the single-precision number.</returns>
		/// <remarks>The internal representation of a 32-bit floating point 
		/// number consists of three fields: a 23 bit mantissa (m), an 8 bit 
		/// exponent (e), and a 1-bit negative bit (n), where the value is 
		/// negative if the bit is set. The bits are ordered as follows: 
		/// neeee...eeemmmmmm...mmmmm (high bit first). The value of a 
		/// 32-bit floating point number is equal to m * 2^(e - 151). If e 
		/// is equal to 255, then the value is negative or positive infinity 
		/// if the mantissa is 0, or not-a-number (NaN) if the mantissa is 
		/// anything else.</remarks>
		public static int SingleToInt32Bits(float value){
			byte[] bytes = BitConverter.GetBytes(value);
			int bits;
			if(BitConverter.IsLittleEndian){
				bits=((((int)bytes[0])&0xFF)|
				      ((((int)bytes[1])&0xFF)<<8)|
				      ((((int)bytes[2])&0xFF)<<16)|
				      ((((int)bytes[3])&0xFF)<<24));
			} else {
				bits=((((int)bytes[3])&0xFF)|
				      ((((int)bytes[2])&0xFF)<<8)|
				      ((((int)bytes[1])&0xFF)<<16)|
				      ((((int)bytes[0])&0xFF)<<24));
			}
			return bits;
		}
		/// <summary>
		/// Converts a 32-bit integer to its form as a single-precision floating point number.
		/// </summary>
		/// <param name="bits">A 32-bit integer.</param>
		/// <returns>A single-precision number represented by the 32-bit integer.</returns>
		/// <remarks>The internal representation of a 32-bit floating point 
		/// number consists of three fields: a 23 bit mantissa (m), an 8 bit 
		/// exponent (e), and a 1-bit negative bit (n), where the value is 
		/// negative if the bit is set. The bits are ordered as follows: 
		/// neeee...eeemmmmmm...mmmmm (high bit first). The value of a 
		/// 32-bit floating point number is equal to m * 2^(e - 151). If e 
		/// is equal to 255, then the value is negative or positive infinity 
		/// if the mantissa is 0, or not-a-number (NaN) if the mantissa is 
		/// anything else.</remarks>
		public static float Int32BitsToSingle(int bits){
			byte[] bytes = new byte[4];
			if(BitConverter.IsLittleEndian){
				bytes[0]=(byte)(bits&0xFF);
				bytes[1]=(byte)((bits>>8)&0xFF);
				bytes[2]=(byte)((bits>>16)&0xFF);
				bytes[3]=(byte)((bits>>24)&0xFF);
			} else {
				bytes[3]=(byte)(bits&0xFF);
				bytes[2]=(byte)((bits>>8)&0xFF);
				bytes[1]=(byte)((bits>>16)&0xFF);
				bytes[0]=(byte)((bits>>24)&0xFF);
			}
			return BitConverter.ToSingle(bytes,0);
		}

		public static long DoubleToInt64Bits(double value){
			byte[] bytes = BitConverter.GetBytes(value);
			long bits;
			if(BitConverter.IsLittleEndian){
				bits=((((long)bytes[0])&0xFF)|
				      ((((long)bytes[1])&0xFF)<<8)|
				      ((((long)bytes[2])&0xFF)<<16)|
				      ((((long)bytes[3])&0xFF)<<24)|
				      ((((long)bytes[4])&0xFF)<<32)|
				      ((((long)bytes[5])&0xFF)<<40)|
				      ((((long)bytes[6])&0xFF)<<48)|
				      ((((long)bytes[7])&0xFF)<<56));
			} else {
				bits=((((long)bytes[7])&0xFF)|
				      ((((long)bytes[6])&0xFF)<<8)|
				      ((((long)bytes[5])&0xFF)<<16)|
				      ((((long)bytes[4])&0xFF)<<24)|
				      ((((long)bytes[3])&0xFF)<<32)|
				      ((((long)bytes[2])&0xFF)<<40)|
				      ((((long)bytes[1])&0xFF)<<48)|
				      ((((long)bytes[0])&0xFF)<<56));
			}
			return bits;
		}
		
		public static double Int64BitsToDouble(long bits){
			byte[] bytes = new byte[8];
			if(BitConverter.IsLittleEndian){
				bytes[0]=(byte)(bits&0xFF);
				bytes[1]=(byte)((bits>>8)&0xFF);
				bytes[2]=(byte)((bits>>16)&0xFF);
				bytes[3]=(byte)((bits>>24)&0xFF);
				bytes[4]=(byte)((bits>>32)&0xFF);
				bytes[5]=(byte)((bits>>40)&0xFF);
				bytes[6]=(byte)((bits>>48)&0xFF);
				bytes[7]=(byte)((bits>>56)&0xFF);
			} else {
				bytes[7]=(byte)(bits&0xFF);
				bytes[6]=(byte)((bits>>8)&0xFF);
				bytes[5]=(byte)((bits>>16)&0xFF);
				bytes[4]=(byte)((bits>>24)&0xFF);
				bytes[3]=(byte)((bits>>32)&0xFF);
				bytes[2]=(byte)((bits>>40)&0xFF);
				bytes[1]=(byte)((bits>>48)&0xFF);
				bytes[0]=(byte)((bits>>56)&0xFF);
			}
			return BitConverter.ToDouble(bytes,0);
		}
	}
}
