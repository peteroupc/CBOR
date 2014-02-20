package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

// import java.math.*;

    /**
     * Contains utility methods that may have use outside of the CBORObject
     * class.
     */
  final class CBORUtilities {
private CBORUtilities() {
}
    private static final String Base64URL = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
    private static final String Base64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

    /**
     * Not documented yet.
     * @param str A StringBuilder object.
     * @param data A byte array.
     * @param padding A Boolean object.
     */
    public static void ToBase64(StringBuilder str, byte[] data, boolean padding) {
      ToBase64(str, data, Base64, padding);
    }

    public static void ToBase64URL(StringBuilder str, byte[] data, boolean padding) {
      ToBase64(str, data, Base64URL, padding);
    }

    private static void ToBase64(StringBuilder str, byte[] data, String alphabet, boolean padding) {
      int length = data.length;
      int i = 0;
      for (i = 0; i < (length - 2); i += 3) {
        str.append(alphabet.charAt((data[i] >> 2) & 63));
        str.append(alphabet.charAt(((data[i] & 3) << 4) + ((data[i + 1] >> 4) & 15)));
        str.append(alphabet.charAt(((data[i + 1] & 15) << 2) + ((data[i + 2] >> 6) & 3)));
        str.append(alphabet.charAt(data[i + 2] & 63));
      }
      int lenmod3 = length % 3;
      if (lenmod3 != 0) {
        i = length - lenmod3;
        str.append(alphabet.charAt((data[i] >> 2) & 63));
        if (lenmod3 == 2) {
          str.append(alphabet.charAt(((data[i] & 3) << 4) + ((data[i + 1] >> 4) & 15)));
          str.append(alphabet.charAt((data[i + 1] & 15) << 2));
          if (padding) {
            str.append("=");
          }
        } else {
          str.append(alphabet.charAt((data[i] & 3) << 4));
          if (padding) {
            str.append("==");
          }
        }
      }
    }

    private static final String HexAlphabet = "0123456789ABCDEF";

    public static void ToBase16(StringBuilder str, byte[] data) {
      int length = data.length;
      for (int i = 0; i < length; ++i) {
        str.append(HexAlphabet.charAt((data[i] >> 4) & 15));
        str.append(HexAlphabet.charAt(data[i] & 15));
      }
    }

    public static boolean ByteArrayEquals(byte[] a, byte[] b) {
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

    public static int ByteArrayHashCode(byte[] a) {
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

    public static int ByteArrayCompare(byte[] a, byte[] b) {
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
      if (a.length != b.length) {
        return (a.length < b.length) ? -1 : 1;
      }
      return 0;
    }

    public static BigInteger BigIntegerFromSingle(float flt) {
      int value = Float.floatToRawIntBits(flt);
      int fpexponent = (int)((value >> 23) & 0xFF);
      if (fpexponent == 255) {
        throw new ArithmeticException("Value is infinity or NaN");
      }
      int mantissa = value & 0x7FFFFF;
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
      } else if (fpexponent > 0) {
        // Value is an integer
        BigInteger bigmantissa = BigInteger.valueOf(mantissa);
        bigmantissa=bigmantissa.shiftLeft(fpexponent);
        if (neg) {
          bigmantissa=(bigmantissa).negate();
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

    public static String BigIntToString(BigInteger bigint) {
      return bigint.toString();
    }

    public static BigInteger BigIntegerFromDouble(double dbl) {
      int[] value = Extras.DoubleToIntegers(dbl);
      int floatExponent = (int)((value[1] >> 20) & 0x7ff);
      boolean neg = (value[1] >> 31) != 0;
      if (floatExponent == 2047) {
        throw new ArithmeticException("Value is infinity or NaN");
      }
      value[1] &= 0xFFFFF;  // Mask out the exponent and sign
      if (floatExponent == 0) {
        ++floatExponent;
      } else {
        value[1] |= 0x100000;
      }
      if ((value[1] | value[0]) != 0) {
        floatExponent += DecimalUtility.ShiftAwayTrailingZerosTwoElements(value);
      }
      floatExponent -= 1075;
      BigInteger bigmantissa = FastInteger.WordsToBigInteger(value);
      if (floatExponent == 0) {
        if (neg) {
          bigmantissa=bigmantissa.negate();
        }
        return bigmantissa;
      } else if (floatExponent > 0) {
        // Value is an integer
        bigmantissa=bigmantissa.shiftLeft(floatExponent);
        if (neg) {
          bigmantissa=(bigmantissa).negate();
        }
        return bigmantissa;
      } else {
        // Value has a fractional part
        int exp = -floatExponent;
        bigmantissa=bigmantissa.shiftRight(exp);
        if (neg) {
          bigmantissa=(bigmantissa).negate();
        }
        return bigmantissa;
      }
    }

    public static float HalfPrecisionToSingle(int value) {
      int negvalue = (value >= 0x8000) ? (1 << 31) : 0;
      value &= 0x7FFF;
      if (value >= 0x7C00) {
        return Float.intBitsToFloat((0x3FC00 | (value & 0x3FF)) << 13 | negvalue);
      } else if (value > 0x400) {
        return Float.intBitsToFloat(((value + 0x1c000) << 13) | negvalue);
      } else if ((value & 0x400) == value) {
        return Float.intBitsToFloat(((value == 0) ? 0 : 0x38800000) | negvalue);
      } else {
        // denormalized
        int m = value & 0x3FF;
        value = 0x1c400;
        while ((m >> 10) == 0) {
          value -= 0x400;
          m <<= 1;
        }
        value = ((value | (m & 0x3FF)) << 13) | negvalue;
        return Float.intBitsToFloat(value);
      }
    }
  }
