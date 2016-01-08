/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Text;
using PeterO;

namespace Test {
    /// <summary>Description of RandomObjects.</summary>
  public static class RandomObjects {
    public static string RandomTextString(FastRandom rand) {
      int length = rand.NextValue(0x2000);
      var sb = new StringBuilder();
      for (var i = 0; i < length; ++i) {
        int x = rand.NextValue(100);
        if (x < 95) {
          // ASCII
          sb.Append((char)(0x20 + rand.NextValue(0x60)));
        } else if (x < 98) {
          // Supplementary character
          x = rand.NextValue(0x400) + 0xd800;
          sb.Append((char)x);
          x = rand.NextValue(0x400) + 0xdc00;
          sb.Append((char)x);
        } else {
          // BMP character
          x = 0x20 + rand.NextValue(0xffe0);
          if (x >= 0xd800 && x < 0xe000) {
            // surrogate code unit, generate ASCII instead
            x = 0x20 + rand.NextValue(0x60);
          }
          sb.Append((char)x);
        }
      }
      return sb.ToString();
    }

    public static ExtendedRational RandomRational(FastRandom rand) {
      BigInteger bigintA = RandomBigInteger(rand);
      BigInteger bigintB = RandomBigInteger(rand);
      if (bigintB.IsZero) {
        bigintB = BigInteger.One;
      }
      return new ExtendedRational(bigintA, bigintB);
    }

    public static long RandomInt64(FastRandom rand) {
      long r = rand.NextValue(0x10000);
      r |= ((long)rand.NextValue(0x10000)) << 16;
      if (rand.NextValue(2) == 0) {
        r |= ((long)rand.NextValue(0x10000)) << 32;
        if (rand.NextValue(2) == 0) {
          r |= ((long)rand.NextValue(0x10000)) << 48;
        }
      }
      return r;
    }

    public static double RandomDouble(FastRandom rand, int exponent) {
      if (exponent == Int32.MaxValue) {
        exponent = rand.NextValue(2047);
      }
      long r = rand.NextValue(0x10000);
      r |= ((long)rand.NextValue(0x10000)) << 16;
      if (rand.NextValue(2) == 0) {
        r |= ((long)rand.NextValue(0x10000)) << 32;
        if (rand.NextValue(2) == 0) {
          r |= ((long)rand.NextValue(0x10000)) << 48;
        }
      }
      r &= ~0x7FF0000000000000L;  // clear exponent
      r |= ((long)exponent) << 52;  // set exponent
      return BitConverter.ToDouble(BitConverter.GetBytes((long)r), 0);
    }

    public static float RandomSingle(FastRandom rand, int exponent) {
      if (exponent == Int32.MaxValue) {
        exponent = rand.NextValue(255);
      }
      int r = rand.NextValue(0x10000);
      if (rand.NextValue(2) == 0) {
        r |= ((int)rand.NextValue(0x10000)) << 16;
      }
      r &= ~0x7f800000;  // clear exponent
      r |= ((int)exponent) << 23;  // set exponent
      return BitConverter.ToSingle(BitConverter.GetBytes((int)r), 0);
    }

    public static ExtendedDecimal RandomExtendedDecimal(FastRandom r) {
      if (r.NextValue(100) == 0) {
        int x = r.NextValue(3);
        if (x == 0) {
          return ExtendedDecimal.PositiveInfinity;
        }
        if (x == 1) {
          return ExtendedDecimal.NegativeInfinity;
        }
        if (x == 2) {
          return ExtendedDecimal.NaN;
        }
        // Signaling NaN currently not generated because
        // it doesn't round-trip as well
      }
      string str = RandomDecimalString(r);
      return ExtendedDecimal.FromString(str);
    }

    public static BigInteger RandomBigInteger(FastRandom r) {
      int selection = r.NextValue(100);
      if (selection < 40) {
        StringAndBigInt sabi = StringAndBigInt.Generate(r, 16);
        return sabi.BigIntValue;
      }
      if (selection < 50) {
        StringAndBigInt sabi = StringAndBigInt.Generate(r, 2 + r.NextValue(35));
        return sabi.BigIntValue;
      } else {
        int count = r.NextValue(60) + 1;
        var bytes = new byte[count];
        for (var i = 0; i < count; ++i) {
          bytes[i] = (byte)((int)r.NextValue(256));
        }
        return BigInteger.fromBytes(bytes, true);
      }
    }

    public static ExtendedFloat RandomExtendedFloat(FastRandom r) {
      if (r.NextValue(100) == 0) {
        int x = r.NextValue(3);
        if (x == 0) {
          return ExtendedFloat.PositiveInfinity;
        }
        if (x == 1) {
          return ExtendedFloat.NegativeInfinity;
        }
        if (x == 2) {
          return ExtendedFloat.NaN;
        }
      }
      return ExtendedFloat.Create(
RandomBigInteger(r),
(BigInteger)(r.NextValue(400) - 200));
    }

    public static byte[] RandomByteString(FastRandom rand) {
      int x = rand.NextValue(0x2000);
      var bytes = new byte[x];
      for (var i = 0; i < x; ++i) {
        bytes[i] = unchecked((byte)rand.NextValue(256));
      }
      return bytes;
    }

    public static byte[] RandomByteStringShort(FastRandom rand) {
      int x = rand.NextValue(50);
      var bytes = new byte[x];
      for (var i = 0; i < x; ++i) {
        bytes[i] = unchecked((byte)rand.NextValue(256));
      }
      return bytes;
    }

    public static String RandomBigIntString(FastRandom r) {
      int count = r.NextValue(50) + 1;
      var sb = new StringBuilder();
      if (r.NextValue(2) == 0) {
        sb.Append('-');
      }
      for (var i = 0; i < count; ++i) {
        if (i == 0) {
          sb.Append((char)('1' + r.NextValue(9)));
        } else {
          sb.Append((char)('0' + r.NextValue(10)));
        }
      }
      return sb.ToString();
    }

    public static BigInteger RandomSmallIntegral(FastRandom r) {
      int count = r.NextValue(20) + 1;
      var sb = new StringBuilder();
      if (r.NextValue(2) == 0) {
        sb.Append('-');
      }
      for (var i = 0; i < count; ++i) {
        if (i == 0) {
          sb.Append((char)('1' + r.NextValue(9)));
        } else {
          sb.Append((char)('0' + r.NextValue(10)));
        }
      }
      return BigInteger.fromString(sb.ToString());
    }

    public static String RandomDecimalString(FastRandom r) {
      int count = r.NextValue(40) + 1;
      var sb = new StringBuilder();
      if (r.NextValue(2) == 0) {
        sb.Append('-');
      }
      for (var i = 0; i < count; ++i) {
        if (i == 0 && count > 1) {
          sb.Append((char)('1' + r.NextValue(9)));
        } else {
          sb.Append((char)('0' + r.NextValue(10)));
        }
      }
      if (r.NextValue(2) == 0) {
        sb.Append('.');
        count = r.NextValue(30) + 1;
        for (var i = 0; i < count; ++i) {
          sb.Append((char)('0' + r.NextValue(10)));
        }
      }
      if (r.NextValue(2) == 0) {
        sb.Append('E');
     count = (r.NextValue(100) < 10) ? r.NextValue(5000) :
          r.NextValue(20);
        if (count != 0) {
          sb.Append(r.NextValue(2) == 0 ? '+' : '-');
        }
        sb.Append(TestCommon.IntToString(count));
      }
      return sb.ToString();
    }
  }
}
