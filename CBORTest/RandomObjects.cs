/*
Written by Peter O. in 2014-2016.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Text;
using PeterO;
using PeterO.Numbers;

namespace Test {
    /// <summary>Description of RandomObjects.</summary>
  public static class RandomObjects {
    public static byte[] RandomByteString(RandomGenerator rand) {
      int x = rand.UniformInt(0x2000);
      var bytes = new byte[x];
      for (var i = 0; i < x; ++i) {
        bytes[i] = unchecked((byte)rand.UniformInt(256));
      }
      return bytes;
    }

    public static byte[] RandomByteStringShort(RandomGenerator rand) {
      int x = rand.UniformInt(50);
      var bytes = new byte[x];
      for (var i = 0; i < x; ++i) {
        bytes[i] = unchecked((byte)rand.UniformInt(256));
      }
      return bytes;
    }

    public static ERational RandomERational(RandomGenerator rand) {
      EInteger bigintA = RandomEInteger(rand);
      EInteger bigintB = RandomEInteger(rand);
      if (bigintB.IsZero) {
        bigintB = EInteger.One;
      }
      return ERational.Create(bigintA, bigintB);
    }

    public static string RandomTextString(RandomGenerator rand) {
      int length = rand.UniformInt(0x2000);
      var sb = new StringBuilder();
      for (var i = 0; i < length; ++i) {
        int x = rand.UniformInt(100);
        if (x < 95) {
         // ASCII
          sb.Append((char)(0x20 + rand.UniformInt(0x60)));
        } else if (x < 98) {
         // Supplementary character
          x = rand.UniformInt(0x400) + 0xd800;
          sb.Append((char)x);
          x = rand.UniformInt(0x400) + 0xdc00;
          sb.Append((char)x);
        } else {
         // BMP character
          x = 0x20 + rand.UniformInt(0xffe0);
          if (x >= 0xd800 && x < 0xe000) {
           // surrogate code unit, generate ASCII instead
            x = 0x20 + rand.UniformInt(0x60);
          }
          sb.Append((char)x);
        }
      }
      return sb.ToString();
    }

    public static long RandomInt64(RandomGenerator rand) {
      long r = rand.UniformInt(0x10000);
      r |= ((long)rand.UniformInt(0x10000)) << 16;
      if (rand.UniformInt(2) == 0) {
        r |= ((long)rand.UniformInt(0x10000)) << 32;
        if (rand.UniformInt(2) == 0) {
          r |= ((long)rand.UniformInt(0x10000)) << 48;
        }
      }
      return r;
    }

    public static double RandomDouble(RandomGenerator rand, int exponent) {
      if (exponent == Int32.MaxValue) {
        exponent = rand.UniformInt(2047);
      }
      long r = rand.UniformInt(0x10000);
      r |= ((long)rand.UniformInt(0x10000)) << 16;
      if (rand.UniformInt(2) == 0) {
        r |= ((long)rand.UniformInt(0x10000)) << 32;
        if (rand.UniformInt(2) == 0) {
          r |= ((long)rand.UniformInt(0x10000)) << 48;
        }
      }
      r &= ~0x7ff0000000000000L; // clear exponent
      r |= ((long)exponent) << 52; // set exponent
      return BitConverter.ToDouble(BitConverter.GetBytes((long)r), 0);
    }

    public static float RandomSingle(RandomGenerator rand, int exponent) {
      if (exponent == Int32.MaxValue) {
        exponent = rand.UniformInt(255);
      }
      int r = rand.UniformInt(0x10000);
      if (rand.UniformInt(2) == 0) {
        r |= ((int)rand.UniformInt(0x10000)) << 16;
      }
      r &= ~0x7f800000; // clear exponent
      r |= ((int)exponent) << 23; // set exponent
      return BitConverter.ToSingle(BitConverter.GetBytes((int)r), 0);
    }

    public static EDecimal RandomEDecimal(RandomGenerator r) {
      if (r.UniformInt(100) == 0) {
        int x = r.UniformInt(3);
        if (x == 0) {
          return EDecimal.PositiveInfinity;
        }
        if (x == 1) {
          return EDecimal.NegativeInfinity;
        }
        if (x == 2) {
          return EDecimal.NaN;
        }
       // Signaling NaN currently not generated because
       // it doesn't round-trip as well
      }
      string str = RandomDecimalString(r);
      return EDecimal.FromString(str);
    }

    public static EInteger RandomEInteger(RandomGenerator r) {
      int selection = r.UniformInt(100);
      if (selection < 40) {
        StringAndBigInt sabi = StringAndBigInt.Generate(r, 16);
        return sabi.BigIntValue;
      }
      if (selection < 50) {
      StringAndBigInt sabi = StringAndBigInt.Generate(
  r,
  2 + r.UniformInt(35));
        return sabi.BigIntValue;
      } else {
        int count = r.UniformInt(60) + 1;
        var bytes = new byte[count];
        for (var i = 0; i < count; ++i) {
          bytes[i] = (byte)((int)r.UniformInt(256));
        }
        return EInteger.FromBytes(bytes, true);
      }
    }

    public static EFloat RandomEFloat(RandomGenerator r) {
      if (r.UniformInt(100) == 0) {
        int x = r.UniformInt(3);
        if (x == 0) {
          return EFloat.PositiveInfinity;
        }
        if (x == 1) {
          return EFloat.NegativeInfinity;
        }
        if (x == 2) {
          return EFloat.NaN;
        }
      }
      return EFloat.Create(
  RandomEInteger(r),
  (EInteger)(r.UniformInt(400) - 200));
    }

    public static String RandomBigIntString(RandomGenerator r) {
      int count = r.UniformInt(50) + 1;
      var sb = new StringBuilder();
      if (r.UniformInt(2) == 0) {
        sb.Append('-');
      }
      for (var i = 0; i < count; ++i) {
        if (i == 0) {
          sb.Append((char)('1' + r.UniformInt(9)));
        } else {
          sb.Append((char)('0' + r.UniformInt(10)));
        }
      }
      return sb.ToString();
    }

    public static EInteger RandomSmallIntegral(RandomGenerator r) {
      int count = r.UniformInt(20) + 1;
      var sb = new StringBuilder();
      if (r.UniformInt(2) == 0) {
        sb.Append('-');
      }
      for (var i = 0; i < count; ++i) {
        if (i == 0) {
          sb.Append((char)('1' + r.UniformInt(9)));
        } else {
          sb.Append((char)('0' + r.UniformInt(10)));
        }
      }
      return EInteger.FromString(sb.ToString());
    }

    public static String RandomDecimalString(RandomGenerator r) {
      int count = r.UniformInt(40) + 1;
      var sb = new StringBuilder();
      if (r.UniformInt(2) == 0) {
        sb.Append('-');
      }
      for (var i = 0; i < count; ++i) {
        if (i == 0 && count > 1) {
          sb.Append((char)('1' + r.UniformInt(9)));
        } else {
          sb.Append((char)('0' + r.UniformInt(10)));
        }
      }
      if (r.UniformInt(2) == 0) {
        sb.Append('.');
        count = r.UniformInt(30) + 1;
        for (var i = 0; i < count; ++i) {
          sb.Append((char)('0' + r.UniformInt(10)));
        }
      }
      if (r.UniformInt(2) == 0) {
        sb.Append('E');
     count = (r.UniformInt(100) < 10) ? r.UniformInt(5000) :
          r.UniformInt(20);
        if (count != 0) {
          sb.Append(r.UniformInt(2) == 0 ? '+' : '-');
        }
        sb.Append(TestCommon.IntToString(count));
      }
      return sb.ToString();
    }
  }
}
