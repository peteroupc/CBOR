/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Text;
using PeterO;
using PeterO.Numbers;

namespace Test {
  /// <summary>Generates random objects of various kinds for purposes of
  /// testing code that uses them. The methods will not necessarily
  /// sample uniformly from all objects of a particular kind.</summary>
  public static class RandomObjects {
    private const int MaxExclusiveStringLength = 0x2000;
    private const int MaxExclusiveShortStringLength = 50;
    private const int MaxNumberLength = 50000;
    private const int MaxShortNumberLength = 40;

    public static byte[] RandomUtf8Bytes(
      IRandomGenExtended rg) {
      return RandomUtf8Bytes(rg, false);
    }

    public static byte[] RandomUtf8Bytes(
      IRandomGenExtended rg,
      bool jsonSafe) {
      using var ms = new Test.DelayingStream();
      if (rg == null) {
        throw new ArgumentNullException(nameof(rg));
      }
      int length = 1 + rg.GetInt32(6);
      for (int i = 0; i < length; ++i) {
        int v = rg.GetInt32(4);
        if (v == 0) {
          int b = 0xe0 + rg.GetInt32(0xee - 0xe1);
          ms.WriteByte((byte)b);
          if (b == 0xe0) {
            ms.WriteByte((byte)(0xa0 + rg.GetInt32(0x20)));
          } else if (b == 0xed) {
            ms.WriteByte((byte)(0x80 + rg.GetInt32(0x20)));
          } else {
            ms.WriteByte((byte)(0x80 + rg.GetInt32(0x40)));
          }
          ms.WriteByte((byte)(0x80 + rg.GetInt32(0x40)));
        } else if (v == 1) {
          int b = 0xf0 + rg.GetInt32(0xf5 - 0xf0);
          ms.WriteByte((byte)b);
          if (b == 0xf0) {
            ms.WriteByte((byte)(0x90 + rg.GetInt32(0x30)));
          } else if (b == 0xf4) {
            ms.WriteByte((byte)(0x80 + rg.GetInt32(0x10)));
          } else {
            ms.WriteByte((byte)(0x80 + rg.GetInt32(0x40)));
          }
          ms.WriteByte((byte)(0x80 + rg.GetInt32(0x40)));
          ms.WriteByte((byte)(0x80 + rg.GetInt32(0x40)));
        } else if (v == 2) {
          if (rg.GetInt32(100) < 5) {
            // 0x80, to help detect ASCII off-by-one errors
            ms.WriteByte(0xc2);
            ms.WriteByte(0x80);
          } else {
            ms.WriteByte((byte)(0xc2 + rg.GetInt32(0xe0 - 0xc2)));
            ms.WriteByte((byte)(0x80 + rg.GetInt32(0x40)));
          }
        } else {
          int ch = rg.GetInt32(0x80);
          if (jsonSafe && (ch == '\\' || ch == '\"' || ch < 0x20)) {
            ch = '?';
          }
          ms.WriteByte((byte)ch);
        }
      }
      return ms.ToArray();
    }

    public static byte[] RandomByteString(IRandomGenExtended rand) {
      if (rand == null) {
        throw new ArgumentNullException(nameof(rand));
      }
      int x = rand.GetInt32(MaxExclusiveStringLength);
      var bytes = new byte[x];
      _ = rand.GetBytes(bytes, 0, bytes.Length);
      return bytes;
    }

    public static byte[] RandomByteString(IRandomGenExtended rand, int length) {
      if (rand == null) {
        throw new ArgumentNullException(nameof(rand));
      }
      var bytes = new byte[length];
      _ = rand.GetBytes(bytes, 0, bytes.Length);
      return bytes;
    }

    public static byte[] RandomByteStringShort(IRandomGenExtended rand) {
      return rand == null ? throw new ArgumentNullException(nameof(rand)) :
        RandomByteString(
          rand,
          rand.GetInt32(MaxExclusiveShortStringLength));
    }

    public static string RandomTextString(IRandomGenExtended rand) {
      if (rand == null) {
        throw new ArgumentNullException(nameof(rand));
      }
      int length = rand.GetInt32(MaxExclusiveStringLength);
      var sb = new StringBuilder();
      for (int i = 0; i < length; ++i) {
        int x = rand.GetInt32(100);
        if (x < 95) {
          // ASCII
          _ = sb.Append((char)(0x20 + rand.GetInt32(0x60)));
        } else if (x < 98) {
          // Supplementary character
          x = rand.GetInt32(0x400) + 0xd800;
          _ = sb.Append((char)x);
          x = rand.GetInt32(0x400) + 0xdc00;
          _ = sb.Append((char)x);
        } else if (rand.GetInt32(100) < 5) {
          // 0x80, to help detect ASCII off-by-one errors
          _ = sb.Append((char)0x80);
        } else {
          // BMP character
          x = 0x20 + rand.GetInt32(0xffe0);
          if (x is >= 0xd800 and < 0xe000) {
            // surrogate code unit, generate ASCII instead
            x = 0x20 + rand.GetInt32(0x60);
          }
          _ = sb.Append((char)x);
        }
      }
      return sb.ToString();
    }

    public static int RandomInt32(IRandomGenExtended rand) {
      byte[] bytes = RandomByteString(rand, 4);
      int ret = bytes[0] & 0xff;
      ret |= (bytes[1] & 0xff) << 8;
      ret |= (bytes[2] & 0xff) << 16;
      ret |= (bytes[3] & 0xff) << 24;
      return ret;
    }

    public static long RandomInt64(IRandomGenExtended rand) {
      byte[] bytes = RandomByteString(rand, 8);
      long ret = ((long)bytes[0]) & 0xff;
      ret |= (((long)bytes[1]) & 0xff) << 8;
      ret |= (((long)bytes[2]) & 0xff) << 16;
      ret |= (((long)bytes[3]) & 0xff) << 24;
      ret |= (((long)bytes[4]) & 0xff) << 32;
      ret |= (((long)bytes[5]) & 0xff) << 40;
      ret |= (((long)bytes[6]) & 0xff) << 48;
      ret |= (((long)bytes[7]) & 0xff) << 56;
      return ret;
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
      return BitConverter.ToDouble(BitConverter.GetBytes(r), 0);
    }

    public static double RandomFiniteDouble(IRandomGenExtended rand) {
      long r;
      do {
        r = RandomInt64(rand);
      } while (((r >> 52) & 0x7ff) == 0x7ff);
      return BitConverter.ToDouble(BitConverter.GetBytes(r), 0);
    }

    public static double RandomDouble(IRandomGenExtended rand) {
      long r = RandomInt64(rand);
      return BitConverter.ToDouble(BitConverter.GetBytes(r), 0);
    }

    public static float RandomSingle(IRandomGenExtended rand) {
      int r = RandomInt32(rand);
      return BitConverter.ToSingle(BitConverter.GetBytes(r), 0);
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
        r |= rand.GetInt32(0x10000) << 16;
      }
      r &= ~0x7f800000; // clear exponent
      r |= exponent << 23; // set exponent
      return BitConverter.ToSingle(BitConverter.GetBytes(r), 0);
    }

    public static string RandomDecimalStringShort(
      IRandomGenExtended wrapper,
      bool extended) {
      var sb = new StringBuilder();
      if (wrapper == null) {
        throw new ArgumentNullException(nameof(wrapper));
      }
      int len = 1 + wrapper.GetInt32(4);
      if (!extended) {
        _ = sb.Append((char)('1' + wrapper.GetInt32(9)));
        --len;
      }
      AppendRandomDecimals(wrapper, sb, len);
      _ = sb.Append('.');
      len = 1 + wrapper.GetInt32(36);
      AppendRandomDecimals(wrapper, sb, len);
      _ = sb.Append('E');
      len = wrapper.GetInt32(25) - 12;
      _ = sb.Append(TestCommon.IntToString(len));
      return sb.ToString();
    }

    public static string RandomBigIntString(IRandomGenExtended r) {
      if (r == null) {
        throw new ArgumentNullException(nameof(r));
      }
      int count = r.GetInt32(MaxShortNumberLength) + 1;
      var sb = new StringBuilder();
      if (r.GetInt32(2) == 0) {
        _ = sb.Append('-');
      }
      _ = sb.Append((char)('1' + r.GetInt32(9)));
      --count;
      AppendRandomDecimals(r, sb, count);
      return sb.ToString();
    }

    public static string RandomSmallIntegralString(IRandomGenExtended r) {
      if (r == null) {
        throw new ArgumentNullException(nameof(r));
      }
      int count = r.GetInt32(MaxShortNumberLength / 2) + 1;
      var sb = new StringBuilder();
      if (r.GetInt32(2) == 0) {
        _ = sb.Append('-');
      }
      _ = sb.Append((char)('1' + r.GetInt32(9)));
      --count;
      AppendRandomDecimals(r, sb, count);
      return sb.ToString();
    }

    public static string RandomDecimalString(IRandomGenExtended r) {
      return RandomDecimalString(r, false, true);
    }

    private static readonly char[] CharTable = {
      '0', '0', '0', '1', '1', '1', '2', '2', '2', '3', '3', '3', '4', '4', '4',
      '5', '5', '5', '6', '6', '6', '7', '7', '7', '8', '8', '8', '9', '9', '9',
    };

    // Special 10-digit-long strings
    private static readonly string[] ValueSpecialDecimals = {
      "1000000000",
      "0000000001",
      "4999999999",
      "5000000000",
      "5000000001",
      "5500000000",
      "0000000000",
      "9999999999",
    };

    // Special 40-digit-long strings
    private static readonly string[] ValueSpecialDecimals2 = {
      "1000000000000000000000000000000000000000",
      "0000000000000000000000000000000000000001",
      "4999999999999999999999999999999999999999",
      "5000000000000000000000000000000000000000",
      "5000000000000000000000000000000000000001",
      "5500000000000000000000000000000000000000",
      "0000000000000000000000000000000000000000",
      "9999999999999999999999999999999999999999",
    };

    private static void AppendRandomDecimalsLong(
      IRandomGenExtended r,
      StringBuilder sb,
      long count) {
      if (sb == null) {
        throw new ArgumentNullException(nameof(sb));
      }
      if (r == null) {
        throw new ArgumentNullException(nameof(r));
      }
      if (count > 0) {
        var buflen = (int)Math.Min(0x10000, Math.Max(count + 8, 64));
        var buffer = new byte[buflen];
        while (count > 0) {
          _ = r.GetBytes(buffer, 0, buflen);
          var i = 0;
          while (i < buflen && count > 0) {
            int x = buffer[i] & 31;
            if (x < 30) {
              _ = sb.Append(CharTable[x]);
              --count;
              ++i;
            } else if (count >= 40 && i + 1 < buflen) {
              int y = (buffer[i + 1] & 0xff) % ValueSpecialDecimals2.Length;
              _ = sb.Append(ValueSpecialDecimals2[y]);
              count -= 40;
              i += 2;
            } else if (count >= 10 && i + 1 < buflen) {
              int y = (buffer[i + 1] & 0xff) % ValueSpecialDecimals.Length;
              _ = sb.Append(ValueSpecialDecimals[y]);
              count -= 10;
              i += 2;
            } else {
              ++i;
            }
          }
        }
      }
    }

    private static void AppendRandomDecimals(
      IRandomGenExtended r,
      StringBuilder sb,
      int smallCount) {
      AppendRandomDecimalsLong(r, sb, smallCount);
    }

    public static string RandomDecimalStringShort(
      IRandomGenExtended r) {
      return RandomDecimalStringShort(r, false);
    }

    public static string RandomDecimalString(
      IRandomGenExtended r,
      bool extended,
      bool limitedExponent) {
      if (r == null) {
        throw new ArgumentNullException(nameof(r));
      }
      if (r.GetInt32(100) < 95) {
        return RandomDecimalStringShort(r, extended);
      }
      var count = (long)r.GetInt32(MaxNumberLength) *
        r.GetInt32(MaxNumberLength) / MaxNumberLength;
      count *= r.GetInt32(MaxNumberLength) / MaxNumberLength;
      count = Math.Max(1, count);
      long afterPointCount = 0;
      long exponentCount = 0;
      var smallExponent = false;
      if (r.GetInt32(2) == 0) {
        afterPointCount = (long)r.GetInt32(MaxNumberLength) *
          r.GetInt32(MaxNumberLength) / MaxNumberLength;
        afterPointCount = afterPointCount *
          r.GetInt32(MaxNumberLength) / MaxNumberLength;
        afterPointCount = Math.Max(1, afterPointCount);
      }
      if (r.GetInt32(2) == 0) {
        if (limitedExponent || r.GetInt32(10) > 0) {
          exponentCount = 5;
        } else {
          exponentCount = (long)r.GetInt32(MaxNumberLength) *
            r.GetInt32(MaxNumberLength) / MaxNumberLength;
          exponentCount = exponentCount *
            r.GetInt32(MaxNumberLength) / MaxNumberLength;
          exponentCount = exponentCount *
            r.GetInt32(MaxNumberLength) / MaxNumberLength;
          exponentCount = Math.Max(1, exponentCount);
        }
      }
      var bufferSize = (int)Math.Min(
        Int32.MaxValue,
        8 + count + afterPointCount + exponentCount);
      var sb = new StringBuilder(bufferSize);
      if (r.GetInt32(2) == 0) {
        _ = sb.Append('-');
      }
      if (!extended) {
        _ = sb.Append((char)('1' + r.GetInt32(9)));
        --count;
      }
      AppendRandomDecimalsLong(r, sb, count);
      if (afterPointCount > 0) {
        _ = sb.Append('.');
        AppendRandomDecimalsLong(r, sb, afterPointCount);
      }
      if (exponentCount > 0) {
        int rr = r.GetInt32(3);
        if (rr == 0) {
          _ = sb.Append('E');
        } else if (rr == 1) {
          _ = sb.Append("E+");
        } else if (rr == 2) {
          _ = sb.Append("E-");
        }
        if (smallExponent) {
          _ = sb.Append(TestCommon.IntToString(r.GetInt32(10000)));
        } else {
          AppendRandomDecimalsLong(r, sb, exponentCount);
        }
      }
      return sb.ToString();
    }
  }
}
