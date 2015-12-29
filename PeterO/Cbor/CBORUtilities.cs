/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Text;
using PeterO;

namespace PeterO.Cbor {
    /// <include file='docs.xml'
    /// path='docs/doc[@name="T:PeterO.Cbor.CBORUtilities"]'/>
  internal static class CBORUtilities {
    private const string HexAlphabet = "0123456789ABCDEF";

    public static void ToBase16(StringBuilder str, byte[] data) {
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      int length = data.Length;
      for (var i = 0; i < length; ++i) {
        str.Append(HexAlphabet[(data[i] >> 4) & 15]);
        str.Append(HexAlphabet[data[i] & 15]);
      }
    }

    public static bool ByteArrayEquals(byte[] a, byte[] b) {
      if (a == null) {
        return b == null;
      }
      if (b == null) {
        return false;
      }
      if (a.Length != b.Length) {
        return false;
      }
      for (var i = 0; i < a.Length; ++i) {
        if (a[i] != b[i]) {
          return false;
        }
      }
      return true;
    }

    public static int ByteArrayHashCode(byte[] a) {
      if (a == null) {
        return 0;
      }
      var ret = 19;
      unchecked {
        ret = (ret * 31) + a.Length;
        for (var i = 0; i < a.Length; ++i) {
          ret = (ret * 31) + a[i];
        }
      }
      return ret;
    }

    public static int ByteArrayCompare(byte[] a, byte[] b) {
      if (a == null) {
        return (b == null) ? 0 : -1;
      }
      if (b == null) {
        return 1;
      }
      int c = Math.Min(a.Length, b.Length);
      for (var i = 0; i < c; ++i) {
        if (a[i] != b[i]) {
          return (a[i] < b[i]) ? -1 : 1;
        }
      }
      return (a.Length != b.Length) ? ((a.Length < b.Length) ? -1 : 1) : 0;
    }

    public static string DoubleToString(double dbl) {
      return Convert.ToString((double)dbl,
        System.Globalization.CultureInfo.InvariantCulture);
      // TODO: Use this version in version 3, and preserve
      // TODO: negative zeros in that version
      //return ExtendedFloat.FromDouble(dbl).ToString();
    }

    public static string SingleToString(float sing) {
      return Convert.ToString((float)sing,
        System.Globalization.CultureInfo.InvariantCulture);
      // TODO: Use this version in version 3, and preserve
      // TODO: negative zeros in that version
      //return ExtendedFloat.FromSingle(dbl).ToString();
    }

    public static BigInteger BigIntegerFromSingle(float flt) {
      int value = BitConverter.ToInt32(BitConverter.GetBytes((float)flt), 0);
      var fpexponent = (int)((value >> 23) & 0xff);
      if (fpexponent == 255) {
        throw new OverflowException("Value is infinity or NaN");
      }
      int mantissa = value & 0x7fffff;
      if (fpexponent == 0) {
        ++fpexponent;
      } else {
        mantissa |= 1 << 23;
      }
      if (mantissa == 0) {
        return BigInteger.Zero;
      }
      fpexponent -= 150;
      while ((mantissa & 1) == 0) {
        ++fpexponent;
        mantissa >>= 1;
      }
      bool neg = (value >> 31) != 0;
      if (fpexponent == 0) {
        if (neg) {
          mantissa = -mantissa;
        }
        return (BigInteger)mantissa;
      }
      if (fpexponent > 0) {
        // Value is an integer
        var bigmantissa = (BigInteger)mantissa;
        bigmantissa <<= fpexponent;
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        return bigmantissa;
      } else {
        // Value has a fractional part
        int exp = -fpexponent;
        for (var i = 0; i < exp && mantissa != 0; ++i) {
          mantissa >>= 1;
        }
        return (BigInteger)mantissa;
      }
    }

    private static void ReverseChars(char[] chars, int offset, int length) {
      int half = length >> 1;
      int right = offset + length - 1;
      for (var i = 0; i < half; i++, right--) {
        char value = chars[offset + i];
        chars[offset + i] = chars[right];
        chars[right] = value;
      }
    }

    public static string LongToString(long value) {
      if (value == Int64.MinValue) {
        return "-9223372036854775808";
      }
      if (value == 0) {
        return "0";
      }
      bool neg = value < 0;
      var chars = new char[24];
      var count = 0;
      if (neg) {
        chars[0] = '-';
        ++count;
        value = -value;
      }
      while (value != 0) {
        char digit = HexAlphabet[(int)(value % 10)];
        chars[count++] = digit;
        value /= 10;
      }
      if (neg) {
        ReverseChars(chars, 1, count - 1);
      } else {
        ReverseChars(chars, 0, count);
      }
      return new String(chars, 0, count);
    }

    public static string BigIntToString(BigInteger bigint) {
      return bigint.ToString();
    }

    public static BigInteger BigIntegerFromDouble(double dbl) {
      long lvalue = BitConverter.ToInt64(
BitConverter.GetBytes((double)dbl),
0);
      int value0 = unchecked((int)(lvalue & 0xFFFFFFFFL));
      int value1 = unchecked((int)((lvalue >> 32) & 0xFFFFFFFFL));
      var floatExponent = (int)((value1 >> 20) & 0x7ff);
      bool neg = (value1 >> 31) != 0;
      if (floatExponent == 2047) {
        throw new OverflowException("Value is infinity or NaN");
      }
      value1 &= 0xfffff;  // Mask out the exponent and sign
      if (floatExponent == 0) {
        ++floatExponent;
      } else {
        value1 |= 0x100000;
      }
      if ((value1 | value0) != 0) {
        while ((value0 & 1) == 0) {
          value0 >>= 1;
          value0 &= 0x7fffffff;
          value0 = unchecked(value0 | (value1 << 31));
          value1 >>= 1;
          ++floatExponent;
        }
      }
      floatExponent -= 1075;
      var bytes = new byte[9];
      BigInteger bigmantissa;
      bytes[0] = (byte)(value0 & 0xff);
      bytes[1] = (byte)((value0 >> 8) & 0xff);
      bytes[2] = (byte)((value0 >> 16) & 0xff);
      bytes[3] = (byte)((value0 >> 24) & 0xff);
      bytes[4] = (byte)(value1 & 0xff);
      bytes[5] = (byte)((value1 >> 8) & 0xff);
      bytes[6] = (byte)((value1 >> 16) & 0xff);
      bytes[7] = (byte)((value1 >> 24) & 0xff);
      bytes[8] = (byte)0;
      bigmantissa = BigInteger.fromBytes(bytes, true);
      if (floatExponent == 0) {
        if (neg) {
          bigmantissa = -bigmantissa;
        }
        return bigmantissa;
      }
      if (floatExponent > 0) {
        // Value is an integer
        bigmantissa <<= floatExponent;
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        return bigmantissa;
      } else {
        // Value has a fractional part
        int exp = -floatExponent;
        bigmantissa >>= exp;
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        return bigmantissa;
      }
    }

    public static float HalfPrecisionToSingle(int value) {
      int negvalue = (value >= 0x8000) ? (1 << 31) : 0;
      value &= 0x7fff;
      if (value >= 0x7c00) {
        value = (int)(0x3fc00 | (value & 0x3ff)) << 13 | negvalue;
        return BitConverter.ToSingle(
BitConverter.GetBytes(value),
0);
      }
      if (value > 0x400) {
        value = (int)((value + 0x1c000) << 13) | negvalue;
        return BitConverter.ToSingle(
BitConverter.GetBytes(value),
0);
      }
      if ((value & 0x400) == value) {
        value = (int)((value == 0) ? 0 : 0x38800000) | negvalue;
        return BitConverter.ToSingle(
BitConverter.GetBytes(value),
0);
      } else {
        // denormalized
        int m = value & 0x3ff;
        value = 0x1c400;
        while ((m >> 10) == 0) {
          value -= 0x400;
          m <<= 1;
        }
        value = ((value | (m & 0x3ff)) << 13) | negvalue;
        return BitConverter.ToSingle(BitConverter.GetBytes((int)value), 0);
      }
    }
  }
}
