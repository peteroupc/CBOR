/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
using System.Text;
//using System.Numerics;
namespace PeterO {
    /// <summary> Contains utility methods that may have use outside of the
    /// CBORObject class. </summary>
  internal static class CBORUtilities {
    private const string Base64URL = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
    private const string Base64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    /// <summary> </summary>
    /// <param name='str'>A StringBuilder object.</param>
    /// <param name='data'>A byte[] object.</param>
    /// <param name='padding'>A Boolean object.</param>
    /// <returns></returns>
    public static void ToBase64(StringBuilder str, byte[] data, bool padding) {
      ToBase64(str, data, Base64, padding);
    }
    public static void ToBase64URL(StringBuilder str, byte[] data, bool padding) {
      ToBase64(str, data, Base64URL, padding);
    }
    private static void ToBase64(StringBuilder str, byte[] data, string alphabet, bool padding) {
      int length = data.Length;
      int i = 0;
      for (i = 0; i < (length - 2); i += 3) {
        str.Append(alphabet[(data[i] >> 2) & 63]);
        str.Append(alphabet[((data[i] & 3) << 4) + ((data[i + 1] >> 4) & 63)]);
        str.Append(alphabet[((data[i + 1] & 15) << 2) + ((data[i + 2] >> 6) & 3)]);
        str.Append(alphabet[data[i + 2] & 63]);
      }
      int lenmod3 = (length % 3);
      if (lenmod3 != 0) {
        i = length - lenmod3;
        str.Append(alphabet[(data[i] >> 2) & 63]);
        if (lenmod3 == 2) {
          str.Append(alphabet[((data[i] & 3) << 4) + ((data[i + 1] >> 4) & 63)]);
          str.Append(alphabet[(data[i + 1] & 15) << 2]);
          if (padding) str.Append("=");
        } else {
          str.Append(alphabet[(data[i] & 3) << 4]);
          if (padding) str.Append("==");
        }
      }
    }
    private const string HexAlphabet = "0123456789ABCDEF";
    public static void ToBase16(StringBuilder str, byte[] data) {
      int length = data.Length;
      for (int i = 0; i < length; i++) {
        str.Append(HexAlphabet[(data[i] >> 4) & 15]);
        str.Append(HexAlphabet[data[i] & 15]);
      }
    }
    public static bool ByteArrayEquals(byte[] a, byte[] b) {
      if (a == null) return (b == null);
      if (b == null) return false;
      if (a.Length != b.Length) return false;
      for (int i = 0; i < a.Length; i++) {
        if (a[i] != b[i]) return false;
      }
      return true;
    }
    public static int ByteArrayHashCode(byte[] a) {
      if (a == null) return 0;
      int ret = 19;
      unchecked {
        ret = ret * 31 + a.Length;
        for (int i = 0; i < a.Length; i++) {
          ret = ret * 31 + a[i];
        }
      }
      return ret;
    }
    public static int ByteArrayCompare(byte[] a, byte[] b) {
      if (a == null) return (b == null) ? 0 : -1;
      if (b == null) return 1;
      int c = Math.Min(a.Length, b.Length);
      for (int i = 0; i < c; i++) {
        if (a[i] != b[i])
          return (a[i] < b[i]) ? -1 : 1;
      }
      if (a.Length != b.Length)
        return (a.Length < b.Length) ? -1 : 1;
      return 0;
    }
    public static BigInteger BigIntegerFromSingle(float flt) {
      int value = BitConverter.ToInt32(BitConverter.GetBytes((float)flt),0);
      int fpexponent = (int)((value >> 23) & 0xFF);
      if (fpexponent == 255)
        throw new OverflowException("Value is infinity or NaN");
      int mantissa = value & 0x7FFFFF;
      if (fpexponent == 0) fpexponent++;
      else mantissa |= (1 << 23);
      if (mantissa == 0) return BigInteger.Zero;
      fpexponent -= 150;
      while ((mantissa & 1) == 0) {
        fpexponent++;
        mantissa >>= 1;
      }
      bool neg = ((value >> 31) != 0);
      if (fpexponent == 0) {
        if (neg) mantissa = -mantissa;
        return (BigInteger)mantissa;
      } else if (fpexponent > 0) {
        // Value is an integer
        BigInteger bigmantissa = (BigInteger)mantissa;
        bigmantissa <<= fpexponent;
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        return bigmantissa;
      } else {
        // Value has a fractional part
        int exp = -fpexponent;
        for (int i = 0; i < exp && mantissa != 0; i++) {
          mantissa >>= 1;
        }
        return (BigInteger)mantissa;
      }
    }

    public static string BigIntToString(BigInteger bigint){
      return bigint.ToString();
    }

    public static BigInteger BigIntegerFromDouble(double dbl) {
      int[] value = Extras.DoubleToIntegers(dbl);
      int fpExponent = (int)((value[1] >> 20) & 0x7ff);
      bool neg=(value[1]>>31)!=0;
      if (fpExponent == 2047)
        throw new OverflowException("Value is infinity or NaN");
      value[1]&=0xFFFFF; // Mask out the exponent and sign
      if (fpExponent == 0) fpExponent++;
      else value[1]|=0x100000;
      if ((value[1]|value[0]) != 0) {
        fpExponent+=DecimalUtility.ShiftAwayTrailingZerosTwoElements(value);
      }
      fpExponent -= 1075;
      BigInteger bigmantissa=FastInteger.WordsToBigInteger(value);
      if (fpExponent == 0) {
        if (neg) bigmantissa = -bigmantissa;
        return bigmantissa;
      } else if (fpExponent > 0) {
        // Value is an integer
        bigmantissa <<= fpExponent;
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        return bigmantissa;
      } else {
        // Value has a fractional part
        int exp = -fpExponent;
        bigmantissa>>=exp;
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        return bigmantissa;
      }
    }
    public static float HalfPrecisionToSingle(int value) {
      int negvalue = (value >= 0x8000) ? (1 << 31) : 0;
      value &= 0x7FFF;
      if (value >= 0x7C00) {
        return BitConverter.ToSingle(BitConverter.GetBytes((int)(0x3FC00 | (value & 0x3FF)) << 13 | negvalue),0);
      } else if (value > 0x400) {
        return BitConverter.ToSingle(BitConverter.GetBytes((int)((value + 0x1c000) << 13) | negvalue),0);
      } else if ((value & 0x400) == value) {
        return BitConverter.ToSingle(BitConverter.GetBytes((int)((value == 0) ? 0 : 0x38800000) | negvalue),0);
      } else {
        // denormalized
        int m = (value & 0x3FF);
        value = 0x1c400;
        while ((m >> 10) == 0) {
          value -= 0x400;
          m <<= 1;
        }
        value = ((value | (m & 0x3FF)) << 13) | negvalue;
        return BitConverter.ToSingle(BitConverter.GetBytes((int)value),0);
      }
    }
  }
}