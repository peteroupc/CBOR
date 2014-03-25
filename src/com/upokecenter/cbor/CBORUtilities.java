package com.upokecenter.cbor;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

import com.upokecenter.util.*;

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
      if (data == null) {
 throw new NullPointerException("data");
}
      ToBase64(str, data, 0, data.length, Base64, padding);
    }

    public static void ToBase64URL(StringBuilder str, byte[] data, boolean padding) {
      if (data == null) {
 throw new NullPointerException("data");
}
      ToBase64(str, data, 0, data.length, Base64URL, padding);
    }

    public static void ToBase64(StringBuilder str, byte[] data, int offset, int count, boolean padding) {
      ToBase64(str, data, offset, count, Base64, padding);
    }

    public static void ToBase64URL(StringBuilder str, byte[] data, int offset, int count, boolean padding) {
      ToBase64(str, data, offset, count, Base64URL, padding);
    }

    public static String ToBase64String(byte[] data, boolean padding) {
      if (data == null) {
 throw new NullPointerException("data");
}
      return ToBase64String(data, 0, data.length, padding);
    }

    public static String ToBase64URLString(byte[] data, boolean padding) {
      if (data == null) {
 throw new NullPointerException("data");
}
      return ToBase64String(data, 0, data.length, padding);
    }

    public static String ToBase64String(byte[] data, int offset, int count, boolean padding) {
      StringBuilder builder = new StringBuilder();
      ToBase64(builder, data, offset, count, Base64, padding);
      return builder.toString();
    }

    public static String ToBase64URLString(byte[] data, int offset, int count, boolean padding) {
      StringBuilder builder = new StringBuilder();
      ToBase64(builder, data, offset, count, Base64, padding);
      return builder.toString();
    }

    private static void ToBase64(StringBuilder str, byte[] data, int offset, int count, String alphabet, boolean padding) {
      if (data == null) {
 throw new NullPointerException("data");
}
if (offset < 0) {
 throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is less than " + "0");
}
if (offset > data.length) {
 throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is more than " + Long.toString((long)data.length));
}
if (count < 0) {
 throw new IllegalArgumentException("count (" + Long.toString((long)count) + ") is less than " + "0");
}
if (count > data.length) {
 throw new IllegalArgumentException("count (" + Long.toString((long)count) + ") is more than " + Long.toString((long)data.length));
}
if (data.length - offset < count) {
 throw new IllegalArgumentException("data's length minus " + offset + " (" + Long.toString((long)(data.length - offset)) + ") is less than " + Long.toString((long)count));
}
      int length = offset + count;
      int i = offset;
      for (i = offset; i < (length - 2); i += 3) {
        str.append(alphabet.charAt((data[i] >> 2) & 63));
        str.append(alphabet.charAt(((data[i] & 3) << 4) + ((data[i + 1] >> 4) & 15)));
        str.append(alphabet.charAt(((data[i + 1] & 15) << 2) + ((data[i + 2] >> 6) & 3)));
        str.append(alphabet.charAt(data[i + 2] & 63));
      }
      int lenmod3 = count % 3;
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

    private static void ToQEncodingRfc2047(StringBuilder str, byte[] data, int offset, int count) {
      if (data == null) {
 throw new NullPointerException("data");
}
if (offset < 0) {
 throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is less than " + "0");
}
if (offset > data.length) {
 throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is more than " + Long.toString((long)data.length));
}
if (count < 0) {
 throw new IllegalArgumentException("count (" + Long.toString((long)count) + ") is less than " + "0");
}
if (count > data.length) {
 throw new IllegalArgumentException("count (" + Long.toString((long)count) + ") is more than " + Long.toString((long)data.length));
}
if (data.length - offset < count) {
 throw new IllegalArgumentException("data's length minus " + offset + " (" + Long.toString((long)(data.length - offset)) + ") is less than " + Long.toString((long)count));
}
      int length = offset + count;
      int i = offset;
      for (i = offset; i < length; i += 3) {
        if (data[i] == 0x20) {
          str.append('_');
        } else if (data[i] == (byte)'(' || data[i] == (byte)')' || data[i]==(byte)'"' ||
                       data[i] == (byte)'=' || data[i] == (byte)'?' || data[i]==(byte)'_' ||
                       data[i] < 0x20 || data[i] >= 0x7f) {
          str.append('=');
          str.append(HexAlphabet.charAt((data[i] >> 4) & 15));
          str.append(HexAlphabet.charAt(data[i] & 15));
        } else {
          str.append((char)data[i]);
        }
      }
    }

    private static final String HexAlphabet = "0123456789ABCDEF";

    public static void ToBase16(StringBuilder str, byte[] data) {
      if (data == null) {
 throw new NullPointerException("data");
}
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
      bytes[0] = (byte)(value0 & 0xff);
      bytes[1] = (byte)((value0 >> 8) & 0xff);
      bytes[2] = (byte)((value0 >> 16) & 0xff);
      bytes[3] = (byte)((value0 >> 24) & 0xff);
      bytes[4] = (byte)(value1 & 0xff);
      bytes[5] = (byte)((value1 >> 8) & 0xff);
      bytes[6] = (byte)((value1 >> 16) & 0xff);
      bytes[7] = (byte)((value1 >> 24) & 0xff);
      bytes[8] = (byte)0;
      BigInteger bigmantissa = BigInteger.fromByteArray((byte[])bytes,true);
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
      value &= 0x7fff;
      if (value >= 0x7c00) {
        return Float.intBitsToFloat((0x3fc00 | (value & 0x3ff)) << 13 | negvalue);
      } else if (value > 0x400) {
        return Float.intBitsToFloat(((value + 0x1c000) << 13) | negvalue);
      } else if ((value & 0x400) == value) {
        return Float.intBitsToFloat(((value == 0) ? 0 : 0x38800000) | negvalue);
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
