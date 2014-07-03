package com.upokecenter.cbor;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.io.*;

import com.upokecenter.util.*;

    /**
     * Contains utility methods that may have use outside of the CBORObject class.
     */
  final class CBORUtilities {
private CBORUtilities() {
}
    private static final String HexAlphabet = "0123456789ABCDEF";

    public static void ToBase16(final StringBuilder str, byte[] data) {
      if (data == null) {
        throw new NullPointerException("data");
      }
      int length = data.length;
      for (int i = 0; i < length; ++i) {
        str.append(HexAlphabet.charAt((data[i] >> 4) & 15));
        str.append(HexAlphabet.charAt(data[i] & 15));
      }
    }

    public static void WriteBase16(final OutputStream outputStream, byte[] data) throws IOException {
      if (data == null) {
        throw new NullPointerException("data");
      }
      int length = data.length;
      for (int i = 0; i < length; ++i) {
        outputStream.write((byte)HexAlphabet.charAt((data[i] >> 4) & 15));
        outputStream.write((byte)HexAlphabet.charAt(data[i] & 15));
      }
    }

    public static boolean ByteArrayEquals(final byte[] a, byte[] b) {
      if (a == null) {
        return b == null;
      }
      if (b == null) {
        return false;
      }
      if (a.length != b.length) {
        return false;
      }
      for (int i = 0; i < a.length; ++i) {
        if (a[i] != b[i]) {
          return false;
        }
      }
      return true;
    }

    public static int ByteArrayHashCode(final byte[] a) {
      if (a == null) {
        return 0;
      }
      int ret = 19;
      {
        ret = (ret * 31) + a.length;
        for (int i = 0; i < a.length; ++i) {
          ret = (ret * 31) + a[i];
        }
      }
      return ret;
    }

    public static int ByteArrayCompare(final byte[] a, byte[] b) {
      if (a == null) {
        return (b == null) ? 0 : -1;
      }
      if (b == null) {
        return 1;
      }
      int c = Math.min(a.length, b.length);
      for (int i = 0; i < c; ++i) {
        if (a[i] != b[i]) {
          return (a[i] < b[i]) ? -1 : 1;
        }
      }
      return (a.length != b.length) ? ((a.length < b.length) ? -1 : 1) : 0;
    }

    public static BigInteger BigIntegerFromSingle(final float flt) {
      int value = Float.floatToRawIntBits(flt);
      int fpexponent = (int)((value >> 23) & 0xff);
      if (fpexponent == 255) {
        throw new ArithmeticException("Value is infinity or NaN");
      }
      int mantissa = value & 0x7fffff;
      if (fpexponent == 0) {
        ++fpexponent;
      } else {
        mantissa |= 1 << 23;
      }
      if (mantissa == 0) {
        return BigInteger.ZERO;
      }
      fpexponent -= 150;
      while ((mantissa & 1) == 0) {
        ++fpexponent;
        mantissa >>= 1;
      }
      boolean neg = (value >> 31) != 0;
      if (fpexponent == 0) {
        if (neg) {
          mantissa = -mantissa;
        }
        return BigInteger.valueOf(mantissa);
      }
      if (fpexponent > 0) {
        // Value is an integer
        BigInteger bigmantissa = BigInteger.valueOf(mantissa);
        bigmantissa = bigmantissa.shiftLeft(fpexponent);
        if (neg) {
          bigmantissa = (bigmantissa).negate();
        }
        return bigmantissa;
      } else {
        // Value has a fractional part
        int exp = -fpexponent;
        for (int i = 0; i < exp && mantissa != 0; ++i) {
          mantissa >>= 1;
        }
        return BigInteger.valueOf(mantissa);
      }
    }

    public static String BigIntToString(final BigInteger bigint) {
      return bigint.toString();
    }

    public static BigInteger BigIntegerFromDouble(final double dbl) {
      long lvalue = Double.doubleToRawLongBits(dbl);
      int value0 = ((int)(lvalue & 0xFFFFFFFFL));
      int value1 = ((int)((lvalue >> 32) & 0xFFFFFFFFL));
      int floatExponent = (int)((value1 >> 20) & 0x7ff);
      boolean neg = (value1 >> 31) != 0;
      if (floatExponent == 2047) {
        throw new ArithmeticException("Value is infinity or NaN");
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
          value0 = (value0 | (value1 << 31));
          value1 >>= 1;
          ++floatExponent;
        }
      }
      floatExponent -= 1075;
      byte[] bytes = new byte[9];
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
      bigmantissa = BigInteger.fromByteArray(bytes, true);
      if (floatExponent == 0) {
        if (neg) {
          bigmantissa = bigmantissa.negate();
        }
        return bigmantissa;
      }
      if (floatExponent > 0) {
        // Value is an integer
        bigmantissa = bigmantissa.shiftLeft(floatExponent);
        if (neg) {
          bigmantissa = (bigmantissa).negate();
        }
        return bigmantissa;
      } else {
        // Value has a fractional part
        int exp = -floatExponent;
        bigmantissa = bigmantissa.shiftRight(exp);
        if (neg) {
          bigmantissa = (bigmantissa).negate();
        }
        return bigmantissa;
      }
    }

    public static float HalfPrecisionToSingle(final int value) {
      int negvalue = (value >= 0x8000) ? (1 << 31) : 0;
      value &= 0x7fff;
      if (value >= 0x7c00) {
        value = (int)(0x3fc00 |
                      (value & 0x3ff)) << 13 | negvalue;
        return Float.intBitsToFloat(value);
      }
      if (value > 0x400) {
        value = (int)((value + 0x1c000) << 13) | negvalue;
        return Float.intBitsToFloat(value);
      }
      if ((value & 0x400) == value) {
        value = (int)((value ==
                       0) ? 0 : 0x38800000) | negvalue;
        return Float.intBitsToFloat(value);
      } else {
        // denormalized
        int m = value & 0x3ff;
        value = 0x1c400;
        while ((m >> 10) == 0) {
          value -= 0x400;
          m <<= 1;
        }
        value = ((value | (m & 0x3ff)) << 13) | negvalue;
        return Float.intBitsToFloat(value);
      }
    }
  }
