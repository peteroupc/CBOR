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
    private const int MaxExclusiveStringLength = 0x2000;
    private const int MaxExclusiveShortStringLength = 50;
    private const int MaxNumberLength = 100000;
    private const int MaxShortNumberLength = 40;
    private const int MaxStringNumDigits = 50;

    public static byte[] RandomByteString(IRandomGenExtended rand) {
      if (rand == null) {
        throw new ArgumentNullException(nameof(rand));
      }
      int x = rand.GetInt32(MaxExclusiveStringLength);
      var bytes = new byte[x];
      rand.GetBytes(bytes, 0, bytes.Length);
      return bytes;
    }

    public static byte[] RandomByteStringShort(IRandomGenExtended rand) {
      if (rand == null) {
        throw new ArgumentNullException(nameof(rand));
      }
      int x = rand.GetInt32(MaxExclusiveShortStringLength);
      var bytes = new byte[x];
      rand.GetBytes(bytes, 0, bytes.Length);
      return bytes;
    }

    public static ERational RandomERational(IRandomGenExtended rand) {
      EInteger bigintA = RandomEInteger(rand);
      EInteger bigintB = RandomEInteger(rand);
      if (bigintB.IsZero) {
        bigintB = EInteger.One;
      }
      return ERational.Create(bigintA, bigintB);
    }

    public static string RandomTextString(IRandomGenExtended rand) {
      if (rand == null) {
        throw new ArgumentNullException(nameof(rand));
      }
      int length = rand.GetInt32(MaxExclusiveStringLength);
      var sb = new StringBuilder();
      for (var i = 0; i < length; ++i) {
        int x = rand.GetInt32(100);
        if (x < 95) {
          // ASCII
          sb.Append((char)(0x20 + rand.GetInt32(0x60)));
        } else if (x < 98) {
          // Supplementary character
          x = rand.GetInt32(0x400) + 0xd800;
          sb.Append((char)x);
          x = rand.GetInt32(0x400) + 0xdc00;
          sb.Append((char)x);
        } else {
          // BMP character
          x = 0x20 + rand.GetInt32(0xffe0);
          if (x >= 0xd800 && x < 0xe000) {
            // surrogate code unit, generate ASCII instead
            x = 0x20 + rand.GetInt32(0x60);
          }
          sb.Append((char)x);
        }
      }
      return sb.ToString();
    }

    public static long RandomInt64(IRandomGenExtended rand) {
      if (rand == null) {
        throw new ArgumentNullException(nameof(rand));
      }
      long r = rand.GetInt32(0x10000);
      r |= ((long)rand.GetInt32(0x10000)) << 16;
      if (rand.GetInt32(2) == 0) {
        r |= ((long)rand.GetInt32(0x10000)) << 32;
        if (rand.GetInt32(2) == 0) {
          r |= ((long)rand.GetInt32(0x10000)) << 48;
        }
      }
      return r;
    }

    public static double RandomDouble(IRandomGenExtended rand, int exponent) {
      if (exponent == Int32.MaxValue) {
        if (rand == null) {
          throw new ArgumentNullException(nameof(rand));
        }
        exponent = rand.GetInt32(2047);
      }
      if (rand == null) {
        throw new ArgumentNullException(nameof(rand));
      }
      long r = rand.GetInt32(0x10000);
      r |= ((long)rand.GetInt32(0x10000)) << 16;
      if (rand.GetInt32(2) == 0) {
        r |= ((long)rand.GetInt32(0x10000)) << 32;
        if (rand.GetInt32(2) == 0) {
          r |= ((long)rand.GetInt32(0x10000)) << 48;
        }
      }
      r &= ~0x7ff0000000000000L; // clear exponent
      r |= ((long)exponent) << 52; // set exponent
      return BitConverter.ToDouble(BitConverter.GetBytes((long)r), 0);
    }

    public static float RandomSingle(IRandomGenExtended rand, int exponent) {
      if (exponent == Int32.MaxValue) {
        if (rand == null) {
          throw new ArgumentNullException(nameof(rand));
        }
        exponent = rand.GetInt32(255);
      }
      if (rand == null) {
        throw new ArgumentNullException(nameof(rand));
      }
      int r = rand.GetInt32(0x10000);
      if (rand.GetInt32(2) == 0) {
        r |= ((int)rand.GetInt32(0x10000)) << 16;
      }
      r &= ~0x7f800000; // clear exponent
      r |= ((int)exponent) << 23; // set exponent
      return BitConverter.ToSingle(BitConverter.GetBytes((int)r), 0);
    }

    public static EDecimal RandomEDecimal(IRandomGenExtended r) {
      if (r == null) {
        throw new ArgumentNullException(nameof(r));
      }
      if (r.GetInt32(100) == 0) {
        int x = r.GetInt32(3);
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
      if (r.GetInt32(100) < 10) {
        string str = RandomDecimalString(r);
        if (str.Length < 500) {
          return EDecimal.FromString(str);
        }
      }
      return EDecimal.Create(RandomEInteger(r), RandomEInteger(r));
    }

    public static EInteger RandomEInteger(IRandomGenExtended r) {
      if (r == null) {
        throw new ArgumentNullException(nameof(r));
      }
      int selection = r.GetInt32(100);
      if (selection < 10) {
        int count = r.GetInt32(MaxNumberLength) + 1;
        var bytes = new byte[count];
        r.GetBytes(bytes, 0, bytes.Length);

        return EInteger.FromBytes(bytes, true);
      }
      if (selection < 50) {
        StringAndBigInt sabi = StringAndBigInt.Generate(
            r,
            2 + r.GetInt32(35),
            MaxStringNumDigits);
        return sabi.BigIntValue;
      } else {
        int count = r.GetInt32(MaxShortNumberLength) + 1;
        var bytes = new byte[count];
        r.GetBytes(bytes, 0, bytes.Length);
        return EInteger.FromBytes(bytes, true);
      }
    }

    public static EFloat RandomEFloat(IRandomGenExtended r) {
      if (r == null) {
        throw new ArgumentNullException(nameof(r));
      }
      if (r.GetInt32(100) == 0) {
        int x = r.GetInt32(3);
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
          (EInteger)(r.GetInt32(400) - 200));
    }

    public static String RandomBigIntString(IRandomGenExtended r) {
      if (r == null) {
        throw new ArgumentNullException(nameof(r));
      }
      int count = r.GetInt32(MaxShortNumberLength) + 1;
      var sb = new StringBuilder();
      if (r.GetInt32(2) == 0) {
        sb.Append('-');
      }
      for (var i = 0; i < count; ++i) {
        if (i == 0) {
          sb.Append((char)('1' + r.GetInt32(9)));
        } else {
          sb.Append((char)('0' + r.GetInt32(10)));
        }
      }
      return sb.ToString();
    }

    public static EInteger RandomSmallIntegral(IRandomGenExtended r) {
      if (r == null) {
        throw new ArgumentNullException(nameof(r));
      }
      int count = r.GetInt32(MaxShortNumberLength / 2) + 1;
      var sb = new StringBuilder();
      if (r.GetInt32(2) == 0) {
        sb.Append('-');
      }
      for (var i = 0; i < count; ++i) {
        if (i == 0) {
          sb.Append((char)('1' + r.GetInt32(9)));
        } else {
          sb.Append((char)('0' + r.GetInt32(10)));
        }
      }
      return EInteger.FromString(sb.ToString());
    }

    public static String RandomDecimalString(IRandomGenExtended r) {
      return RandomDecimalString(r, false, true);
    }

    public static String RandomDecimalString(
      IRandomGenExtended r,
      bool extended,
      bool limitedExponent) {
      if (r == null) {
        throw new ArgumentNullException(nameof(r));
      }
      long count = ((long)r.GetInt32(MaxNumberLength) *
r.GetInt32(MaxNumberLength)) / MaxNumberLength;
      count = Math.Max(1, count);
      var sb = new StringBuilder();
      if (r.GetInt32(2) == 0) {
        sb.Append('-');
      }
      for (var i = 0; i < count; ++i) {
        if (i == 0 && count > 1 && !extended) {
          sb.Append((char)('1' + r.GetInt32(9)));
        } else {
          sb.Append((char)('0' + r.GetInt32(10)));
        }
      }
      if (r.GetInt32(2) == 0) {
        sb.Append('.');
        count = ((long)r.GetInt32(MaxNumberLength) *
r.GetInt32(MaxNumberLength)) / MaxNumberLength;
        count = ((long)count *
r.GetInt32(MaxNumberLength)) / MaxNumberLength;
        count = Math.Max(1, count);
        for (var i = 0; i < count; ++i) {
          sb.Append((char)('0' + r.GetInt32(10)));
        }
      }
      if (r.GetInt32(2) == 0) {
        int rr = r.GetInt32(3);
        if (rr == 0) {
          sb.Append("E");
        } else if (rr == 1) {
   sb.Append("E+");
 } else if (rr == 2) {
   sb.Append("E-");
 }
        if (limitedExponent || r.GetInt32(10) > 0) {
   sb.Append(TestCommon.IntToString(r.GetInt32(10000)));
        } else {
          count = ((long)r.GetInt32(MaxNumberLength) *
r.GetInt32(MaxNumberLength)) / MaxNumberLength;
          count = ((long)count *
r.GetInt32(MaxNumberLength)) / MaxNumberLength;
          count = Math.Max(1, count);
          for (var i = 0; i < count; ++i) {
            sb.Append((char)(0x30 + r.GetInt32(10)));
          }
        }
      }
      return sb.ToString();
    }
  }
}
