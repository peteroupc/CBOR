/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

 */
using System;
using System.IO;

namespace Test {
  /// <summary>Contains lightweight methods for reading and writing CBOR
  /// data.</summary>
  public static class MiniCBOR {
    private static float ToSingle(int value) {
      return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
    }

    private static double ToDouble(long value) {
      return BitConverter.ToDouble(BitConverter.GetBytes(value), 0);
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

    public static bool ReadBoolean(Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      int b = stream.ReadByte();
      if (b == 0xf4) {
        return false;
      }
      if (b == 0xf5) {
        return true;
      }
      while ((b >> 5) == 6) {
        // Skip tags until a tag character is no longer read
        if (b == 0xd8) {
          stream.ReadByte();
        } else if (b == 0xd9) {
          stream.Position += 2;
        } else if (b == 0xda) {
          stream.Position += 4;
        } else if (b == 0xdb) {
          stream.Position += 8;
        } else if (b > 0xdb) {
          throw new IOException("Not a boolean");
        }
        b = stream.ReadByte();
      }
      if (b == 0xf4) {
        return false;
      }
      if (b == 0xf5) {
        return true;
      }
      throw new IOException("Not a boolean");
    }

    public static void WriteBoolean(bool value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      stream.WriteByte(value ? (byte)0xf5 : (byte)0xf4);
    }

    public static void WriteInt32(int value, Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      var type = 0;
      byte[] bytes;
      if (value < 0) {
        ++value;
        value = -value;
        type = 0x20;
      }
      if (value < 24) {
        stream.WriteByte((byte)(value | type));
      } else if (value <= 0xff) {
        bytes = new[] { (byte)(24 | type), (byte)(value & 0xff) };
        stream.Write(bytes, 0, 2);
      } else if (value <= 0xffff) {
        bytes = new[] {
          (byte)(25 | type), (byte)((value >> 8) & 0xff),
          (byte)(value & 0xff),
        };
        stream.Write(bytes, 0, 3);
      } else {
        bytes = new[] {
          (byte)(26 | type), (byte)((value >> 24) & 0xff),
          (byte)((value >> 16) & 0xff), (byte)((value >> 8) & 0xff),
          (byte)(value & 0xff),
        };
        stream.Write(bytes, 0, 5);
      }
    }

    private static long ReadInteger(
      Stream stream,
      int headByte,
      bool check32bit) {
      int kind = headByte & 0x1f;
      if (kind == 0x18) {
        int b = stream.ReadByte();
        if (b < 0) {
          throw new IOException("Premature end of stream");
        }
        return (headByte != 0x38) ? b : -1 - b;
      }
      if (kind == 0x19) {
        var bytes = new byte[2];
        if (stream.Read(bytes, 0, bytes.Length) != bytes.Length) {
          throw new IOException("Premature end of stream");
        }
        int b = ((int)bytes[0]) & 0xff;
        b <<= 8;
        b |= ((int)bytes[1]) & 0xff;
        return (headByte != 0x39) ? b : -1 - b;
      }
      if (kind == 0x1a || kind == 0x3a) {
        var bytes = new byte[4];
        if (stream.Read(bytes, 0, bytes.Length) != bytes.Length) {
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
        var bytes = new byte[8];
        if (stream.Read(bytes, 0, bytes.Length) != bytes.Length) {
          throw new IOException("Premature end of stream");
        }
        long b;
        if (check32bit && (bytes[0] != 0 || bytes[1] != 0 || bytes[2] != 0 ||
            bytes[3] != 0)) {
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

    private static double ReadFP(Stream stream, int headByte) {
      int b;
      if (headByte == 0xf9) {
        // Half-precision
        var bytes = new byte[2];
        if (stream.Read(bytes, 0, bytes.Length) != bytes.Length) {
          throw new IOException("Premature end of stream");
        }
        b = ((int)bytes[0]) & 0xff;
        b <<= 8;
        b |= ((int)bytes[1]) & 0xff;
        return (double)HalfPrecisionToSingle(b);
      }
      if (headByte == 0xfa) {
        var bytes = new byte[4];
        if (stream.Read(bytes, 0, bytes.Length) != bytes.Length) {
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
        var bytes = new byte[8];
        if (stream.Read(bytes, 0, bytes.Length) != bytes.Length) {
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

    /// <summary>Reads a double-precision floating point number in CBOR
    /// format from a data stream.</summary>
    /// <param name='stream'>A data stream.</param>
    /// <returns>A 64-bit floating-point number.</returns>
    /// <exception cref='System.IO.IOException'>The end of the stream was
    /// reached, or the object read isn't a number.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    public static double ReadDouble(Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      int b = stream.ReadByte();
      if (b >= 0x00 && b < 0x18) {
        return (double)b;
      }
      if (b >= 0x20 && b < 0x38) {
        return (double)(-1 - (b & 0x1f));
      }
      while ((b >> 5) == 6) {
        // Skip tags until a tag character is no longer read
        if (b == 0xd8) {
          stream.ReadByte();
        } else if (b == 0xd9) {
          stream.Position += 2;
        } else if (b == 0xda) {
          stream.Position += 4;
        } else if (b == 0xdb) {
          stream.Position += 8;
        } else if (b > 0xdb) {
          throw new IOException("Not a 32-bit integer");
        }
        b = stream.ReadByte();
      }
      if (b >= 0x00 && b < 0x18) {
        return (double)b;
      }
      if (b >= 0x20 && b < 0x38) {
        return (double)(-1 - (b & 0x1f));
      }
      if (b == 0xf9 || b == 0xfa || b == 0xfb) {
        // Read a floating-point number
        return ReadFP(stream, b);
      }
      if (b == 0x18 || b == 0x19 || b == 0x1a || b == 0x38 ||
        b == 0x39 || b == 0x3a) { // covers headbytes 0x18-0x1a and 0x38-0x3A
        return (double)ReadInteger(stream, b, false);
      }
      throw new IOException("Not a double");
    }

    /// <summary>Reads a 32-bit integer in CBOR format from a data stream.
    /// If the object read is a floating-point number, it is converted to
    /// an integer by discarding the fractional part of the result of
    /// division.</summary>
    /// <param name='stream'>A data stream.</param>
    /// <returns>A 32-bit signed integer.</returns>
    /// <exception cref='System.IO.IOException'>The end of the stream was
    /// reached, or the object read isn't a number, or can't fit a 32-bit
    /// integer.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    public static int ReadInt32(Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      int b = stream.ReadByte();
      if (b >= 0x00 && b < 0x18) {
        return b;
      }
      if (b >= 0x20 && b < 0x38) {
        return -1 - (b & 0x1f);
      }
      while ((b >> 5) == 6) {
        // Skip tags until a tag character is no longer read
        if (b == 0xd8) {
          stream.ReadByte();
        } else if (b == 0xd9) {
          stream.Position += 2;
        } else if (b == 0xda) {
          stream.Position += 4;
        } else if (b == 0xdb) {
          stream.Position += 8;
        } else if (b > 0xdb) {
          throw new IOException("Not a 32-bit integer");
        }
        b = stream.ReadByte();
      }
      if (b >= 0x00 && b < 0x18) {
        return b;
      }
      if (b >= 0x20 && b < 0x38) {
        return -1 - (b & 0x1f);
      }
      if (b == 0xf9 || b == 0xfa || b == 0xfb) {
        // Read a floating-point number
        double dbl = ReadFP(stream, b);
        // Truncate to a 32-bit integer
        if (Double.IsInfinity(dbl) || Double.IsNaN(dbl)) {
          throw new IOException("Not a 32-bit integer");
        }
        dbl = (dbl < 0) ? Math.Ceiling(dbl) : Math.Floor(dbl);
        if (dbl < Int32.MinValue || dbl > Int32.MaxValue) {
          throw new IOException("Not a 32-bit integer");
        }
        return (int)dbl;
      }
      if ((b & 0xdc) == 0x18) { // covers headbytes 0x18-0x1b and 0x38-0x3B
        return (int)ReadInteger(stream, b, true);
      }
      throw new IOException("Not a 32-bit integer");
    }

    public static int ReadInt32MajorType1Or2(Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      int b = stream.ReadByte();
      if (b >= 0x00 && b < 0x18) {
        return b;
      }
      if (b >= 0x20 && b < 0x38) {
        return -1 - (b & 0x1f);
      }
      if (b == 0x18 || b == 0x38) {
        int b1 = stream.ReadByte();
        int b2 = stream.ReadByte();
        if (b1 < 0 || b2 < 0) {
          throw new IOException();
        }
        int c = (b1 << 8) | b2;
        return (b == 0x18) ? c : -1 - c;
      }
      if (b == 0x19 || b == 0x39 || b == 0x1a || b == 0x3a) {
        if ((b & 0x1f) == 0x1a && (stream.ReadByte() != 0 ||
            stream.ReadByte() != 0 || stream.ReadByte() != 0 ||
            stream.ReadByte() != 0)) {
          throw new IOException();
        }
        int b1 = stream.ReadByte();
        int b2 = stream.ReadByte();
        int b3 = stream.ReadByte();
        int b4 = stream.ReadByte();
        if (b1 < 0 || b2 < 0 || b3 < 0 || b4 < 0 || b1 >= 0x80) {
          throw new IOException();
        }
        int c = (b1 << 24) | (b2 << 16) | (b3 << 8) | b4;
        return (b < 0x20) ? c : -1 - c;
      }
      throw new IOException();
    }
  }
}
