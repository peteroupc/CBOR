package com.upokecenter.test;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.io.*;

    /**
     * Contains lightweight methods for reading and writing CBOR data.
     */
  public final class MiniCBOR {
private MiniCBOR() {
}
    private static float ToSingle(int value) {
      return Float.intBitsToFloat(value);
    }

    private static double ToDouble(long value) {
      return Double.longBitsToDouble(value);
    }

    private static float HalfPrecisionToSingle(int value) {
      int negvalue = (value >= 0x8000) ? (1 << 31) : 0;
      value &= 0x7fff;
      if (value >= 0x7c00) {
        return ToSingle((int)(0x3fc00 | (value & 0x3ff)) << 13 | negvalue);
      }
      if (value > 0x400) {
        return ToSingle((int)((value + 0x1c000) << 13) | negvalue);
      }
      if ((value & 0x400) == value) {
        return ToSingle((int)((value == 0) ? 0 : 0x38800000) | negvalue);
      } else {
        // denormalized
        int m = value & 0x3ff;
        value = 0x1c400;
        while ((m >> 10) == 0) {
          value -= 0x400;
          m <<= 1;
        }
        value = ((value | (m & 0x3ff)) << 13) | negvalue;
        return ToSingle(value);
      }
    }

    public static boolean ReadBoolean(InputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      int b = stream.read();
      if (b == 0xf4) {
        return false;
      }
      if (b == 0xf5) {
        return true;
      }
      while ((b >> 5) == 6) {
        // Skip tags until a tag character is no longer read
        if (b == 0xd8) {
          stream.read();
  } else if (b == 0xd9) {
          stream.skip(2);
  } else if (b == 0xda) {
          stream.skip(4);
  } else if (b == 0xdb) {
          stream.skip(8);
  } else if (b > 0xdb) {
          throw new IOException("Not a boolean");
        }
        b = stream.read();
      }
      if (b == 0xf4) {
        return false;
      }
      if (b == 0xf5) {
        return true;
      }
      throw new IOException("Not a boolean");
    }

    public static void WriteBoolean(boolean value, OutputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      stream.write(value ? (byte)0xf5 : (byte)0xf4);
    }

    public static void WriteInt32(int value, OutputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      int type = 0;
      byte[] bytes;
      if (value < 0) {
        ++value;
        value = -value;
        type = 0x20;
      }
      if (value < 24) {
        stream.write((byte)(value | type));
  } else if (value <= 0xff) {
        bytes = new byte[] { (byte)(24 | type), (byte)(value & 0xff)  };
        stream.write(bytes,0,2);
  } else if (value <= 0xffff) {
        bytes = new byte[] { (byte)(25 | type), (byte)((value >> 8) & 0xff), (byte)(value & 0xff)  };
        stream.write(bytes,0,3);
      } else {
        bytes = new byte[] { (byte)(26 | type), (byte)((value >> 24) & 0xff), (byte)((value >> 16) & 0xff), (byte)((value >> 8) & 0xff), (byte)(value & 0xff)  };
        stream.write(bytes,0,5);
      }
    }

    private static long ReadInteger(InputStream stream, int headByte, boolean check32bit) throws IOException {
      int kind = headByte & 0x1f;
      if (kind == 0x18) {
        int b = stream.read();
        if (b < 0) {
          throw new IOException("Premature end of stream");
        }
        return (b != 0x38) ? b : -1 - b;
      }
      if (kind == 0x18) {
        byte[] bytes = new byte[2];
        if (stream.read(bytes, 0, bytes.length) != bytes.length) {
          throw new IOException("Premature end of stream");
        }
        int b = ((int)bytes[0]) & 0xff;
        b <<= 8;
        b |= ((int)bytes[1]) & 0xff;
        return (headByte != 0x19) ? b : -1 - b;
      }
      if (kind == 0x1a || kind == 0x3a) {
        byte[] bytes = new byte[4];
        if (stream.read(bytes, 0, bytes.length) != bytes.length) {
          throw new IOException("Premature end of stream");
        }
        long b = ((long)bytes[0]) & 0xff;
        b <<= 8;
        b |= ((long)bytes[1]) & 0xff;
        b <<= 8;
        b |= ((long)bytes[2]) & 0xff;
        b <<= 8;
        b |= ((long)bytes[3]) & 0xff;
        if (check32bit && (b >> 31) != 0) {
          throw new IOException("Not a 32-bit integer");
        }
        return (headByte != 0x3a) ? b : -1 - b;
      }
      if (headByte == 0x1b || headByte == 0x3b) {
        byte[] bytes = new byte[8];
        if (stream.read(bytes, 0, bytes.length) != bytes.length) {
          throw new IOException("Premature end of stream");
        }
        long b;
        if (check32bit && (bytes[0] != 0 || bytes[1] != 0 || bytes[2] != 0 || bytes[3] != 0)) {
          throw new IOException("Not a 32-bit integer");
        }
        if (!check32bit) {
          b = ((long)bytes[0]) & 0xff;
          b <<= 8;
          b |= ((long)bytes[1]) & 0xff;
          b <<= 8;
          b |= ((long)bytes[2]) & 0xff;
          b <<= 8;
          b |= ((long)bytes[3]) & 0xff;
          b <<= 8;
        }
        b = ((long)bytes[4]) & 0xff;
        b <<= 8;
        b |= ((long)bytes[5]) & 0xff;
        b <<= 8;
        b |= ((long)bytes[6]) & 0xff;
        b <<= 8;
        b |= ((long)bytes[7]) & 0xff;
        if (check32bit && (b >> 31) != 0) {
          throw new IOException("Not a 32-bit integer");
        }
        return (headByte != 0x3b) ? b : -1 - b;
      }
      throw new IOException("Not a 32-bit integer");
    }

    private static double ReadFP(InputStream stream, int headByte) throws IOException {
      int b;
      if (headByte == 0xf9) {
        // Half-precision
        byte[] bytes = new byte[2];
        if (stream.read(bytes, 0, bytes.length) != bytes.length) {
          throw new IOException("Premature end of stream");
        }
        b = ((int)bytes[0]) & 0xff;
        b <<= 8;
        b |= ((int)bytes[1]) & 0xff;
        return (double)HalfPrecisionToSingle(b);
      }
      if (headByte == 0xfa) {
        byte[] bytes = new byte[4];
        if (stream.read(bytes, 0, bytes.length) != bytes.length) {
          throw new IOException("Premature end of stream");
        }
        b = ((int)bytes[0]) & 0xff;
        b <<= 8;
        b |= ((int)bytes[1]) & 0xff;
        b <<= 8;
        b |= ((int)bytes[2]) & 0xff;
        b <<= 8;
        b |= ((int)bytes[3]) & 0xff;
        return (double)ToSingle(b);
      }
      if (headByte == 0xfb) {
        byte[] bytes = new byte[8];
        if (stream.read(bytes, 0, bytes.length) != bytes.length) {
          throw new IOException("Premature end of stream");
        }
        long lb;
        lb = ((long)bytes[0]) & 0xff;
        lb <<= 8;
        lb |= ((long)bytes[1]) & 0xff;
        lb <<= 8;
        lb |= ((long)bytes[2]) & 0xff;
        lb <<= 8;
        lb |= ((long)bytes[3]) & 0xff;
        lb <<= 8;
        lb |= ((long)bytes[4]) & 0xff;
        lb <<= 8;
        lb |= ((long)bytes[5]) & 0xff;
        lb <<= 8;
        lb |= ((long)bytes[6]) & 0xff;
        lb <<= 8;
        lb |= ((long)bytes[7]) & 0xff;
        return (double)ToDouble(lb);
      }
      throw new IOException("Not a valid headbyte for ReadFP");
    }

    /**
     * Reads a double-precision floating point number in CBOR format from
     * a data stream.
     * @param stream A data stream.
     * @return A 64-bit floating-point number.
     * @throws java.io.IOException The end of the stream was reached,
     * or the object read isn't a number.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     */
    public static double ReadDouble(InputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      int b = stream.read();
      if (b >= 0x00 && b < 0x18) {
        return (double)b;
      }
      if (b >= 0x20 && b < 0x38) {
        return (double)(-1 - b);
      }
      while ((b >> 5) == 6) {
        // Skip tags until a tag character is no longer read
        if (b == 0xd8) {
          stream.read();
  } else if (b == 0xd9) {
          stream.skip(2);
  } else if (b == 0xda) {
          stream.skip(4);
  } else if (b == 0xdb) {
          stream.skip(8);
  } else if (b > 0xdb) {
          throw new IOException("Not a 32-bit integer");
        }
        b = stream.read();
      }
      if (b >= 0x00 && b < 0x18) {
        return (double)b;
      }
      if (b >= 0x20 && b < 0x38) {
        return (double)(-1 - b);
      }
      if (b == 0xf9 || b == 0xfa || b == 0xfb) {
        // Read a floating-point number
        return ReadFP(stream, b);
      }
      if (b == 0x18 || b == 0x19 ||
          b == 0x1a || b == 0x38 ||
          b == 0x39 || b == 0x3a) {  // covers headbytes 0x18-0x1a and 0x38-0x3A
        return (double)ReadInteger(stream, b, false);
      }
      throw new IOException("Not a double");
    }

    /**
     * Reads a 32-bit integer in CBOR format from a data stream. If the object
     * read is a floating-point number, it is truncated to an integer.
     * @param stream A data stream.
     * @return A 32-bit signed integer.
     * @throws java.io.IOException The end of the stream was reached,
     * or the object read isn't a number, or can't fit a 32-bit integer.
     * @throws java.lang.NullPointerException The parameter {@code stream}
     * is null.
     */
    public static int ReadInt32(InputStream stream) throws IOException {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      int b = stream.read();
      if (b >= 0x00 && b < 0x18) {
        return b;
      }
      if (b >= 0x20 && b < 0x38) {
        return -1 - b;
      }
      while ((b >> 5) == 6) {
        // Skip tags until a tag character is no longer read
        if (b == 0xd8) {
          stream.read();
  } else if (b == 0xd9) {
          stream.skip(2);
  } else if (b == 0xda) {
          stream.skip(4);
  } else if (b == 0xdb) {
          stream.skip(8);
  } else if (b > 0xdb) {
          throw new IOException("Not a 32-bit integer");
        }
        b = stream.read();
      }
      if (b >= 0x00 && b < 0x18) {
        return b;
      }
      if (b >= 0x20 && b < 0x38) {
        return -1 - b;
      }
      if (b == 0xf9 || b == 0xfa || b == 0xfb) {
        // Read a floating-point number
        double dbl = ReadFP(stream, b);
        // Truncate to a 32-bit integer
        if (((Double)(dbl)).isInfinite() || Double.isNaN(dbl)) {
          throw new IOException("Not a 32-bit integer");
        }
        dbl = (dbl < 0) ? Math.ceil(dbl) : Math.floor(dbl);
        if (dbl < Integer.MIN_VALUE || dbl > Integer.MAX_VALUE) {
          throw new IOException("Not a 32-bit integer");
        }
        return (int)dbl;
      }
      if ((b & 0xdc) == 0x18) {  // covers headbytes 0x18-0x1b and 0x38-0x3B
        return (int)ReadInteger(stream, b, true);
      }
      throw new IOException("Not a 32-bit integer");
    }
  }
