/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Text;
using PeterO;
using PeterO.Numbers;

namespace PeterO.Cbor {
  /// <summary>Contains utility methods that may have use outside of the
  /// CBORObject class.</summary>
  internal static class CBORUtilities {
    private const string HexAlphabet = "0123456789ABCDEF";

    public static int FastPathStringCompare(string strA, string strB) {
      if (strA == null) {
        return (strB == null) ? 0 : -1;
      }
      if (strB == null) {
        return 1;
      }
      if (strA.Length == 0) {
        return strB.Length == 0 ? 0 : -1;
      }
      if (strB.Length == 0) {
        return strA.Length == 0 ? 0 : 1;
      }
      if (strA.Length < 64 && strB.Length < 64) {
        if (strA.Length == strB.Length) {
          var equalStrings = true;
          for (int i = 0; i < strA.Length; ++i) {
            if (strA[i] != strB[i]) {
              equalStrings = false;
              break;
            }
          }
          if (equalStrings) {
            return 0;
          }
        }
        for (int i = 0; i < strA.Length; ++i) {
          if ((strA[i] & ((byte)0x80)) != 0) {
            return -2; // non-ASCII
          }
        }
        for (int i = 0; i < strB.Length; ++i) {
          if ((strB[i] & ((byte)0x80)) != 0) {
            return -2; // non-ASCII
          }
        }
        if (strA.Length != strB.Length) {
          return strA.Length < strB.Length ? -1 : 1;
        }
        for (int i = 0; i < strA.Length; ++i) {
          if (strA[i] != strB[i]) {
            return strA[i] < strB[i] ? -1 : 1;
          }
        }
        return 0;
      }
      return -2; // not short enough
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

    public static int ByteArrayCompareLengthFirst(byte[] a, byte[] b) {
      if (a == null) {
        return (b == null) ? 0 : -1;
      }
      if (b == null) {
        return 1;
      }
      if (a.Length != b.Length) {
        return a.Length < b.Length ? -1 : 1;
      }
      for (var i = 0; i < a.Length; ++i) {
        if (a[i] != b[i]) {
          return (a[i] < b[i]) ? -1 : 1;
        }
      }
      return 0;
    }

    public static string TrimDotZero(string str) {
      return (str.Length > 2 && str[str.Length - 1] == '0' && str[str.Length
            - 2] == '.') ? str.Substring(0, str.Length - 2) :
        str;
    }

    public static long DoubleToInt64Bits(double dbl) {
      return BitConverter.ToInt64(BitConverter.GetBytes((double)dbl), 0);
    }

    public static int SingleToInt32Bits(float flt) {
      return BitConverter.ToInt32(BitConverter.GetBytes((float)flt), 0);
    }

    public static double Int64BitsToDouble(long bits) {
      return BitConverter.ToDouble(
          BitConverter.GetBytes(bits),
          0);
    }

    public static float Int32BitsToSingle(int bits) {
      return BitConverter.ToSingle(
          BitConverter.GetBytes(bits),
          0);
    }

    public static string DoubleToString(double dbl) {
      return EFloat.FromDouble(dbl).ToShortestString(EContext.Binary64);
    }

    public static string SingleToString(float sing) {
      return EFloat.FromSingle(sing).ToShortestString(EContext.Binary32);
    }

    public static string LongToString(long longValue) {
      if (longValue == Int64.MinValue) {
        return "-9223372036854775808";
      }
      if (longValue == 0L) {
        return "0";
      }
      if (longValue == (long)Int32.MinValue) {
        return "-2147483648";
      }
      bool neg = longValue < 0;
      var count = 0;
      char[] chars;
      int intlongValue = unchecked((int)longValue);
      if ((long)intlongValue == longValue) {
        chars = new char[12];
        count = 11;
        if (neg) {
          intlongValue = -intlongValue;
        }
        while (intlongValue > 43698) {
          int intdivValue = intlongValue / 10;
          char digit = HexAlphabet[(int)(intlongValue - (intdivValue *
                  10))];
          chars[count--] = digit;
          intlongValue = intdivValue;
        }
        while (intlongValue > 9) {
          int intdivValue = (intlongValue * 26215) >> 18;
          char digit = HexAlphabet[(int)(intlongValue - (intdivValue *
                  10))];
          chars[count--] = digit;
          intlongValue = intdivValue;
        }
        if (intlongValue != 0) {
          chars[count--] = HexAlphabet[(int)intlongValue];
        }
        if (neg) {
          chars[count] = '-';
        } else {
          ++count;
        }
        return new String(chars, count, 12 - count);
      } else {
        chars = new char[24];
        count = 23;
        if (neg) {
          longValue = -longValue;
        }
        while (longValue > 43698) {
          long divValue = longValue / 10;
          char digit = HexAlphabet[(int)(longValue - (divValue * 10))];
          chars[count--] = digit;
          longValue = divValue;
        }
        while (longValue > 9) {
          long divValue = (longValue * 26215) >> 18;
          char digit = HexAlphabet[(int)(longValue - (divValue * 10))];
          chars[count--] = digit;
          longValue = divValue;
        }
        if (longValue != 0) {
          chars[count--] = HexAlphabet[(int)longValue];
        }
        if (neg) {
          chars[count] = '-';
        } else {
          ++count;
        }
        return new String(chars, count, 24 - count);
      }
    }

    private static EInteger FloorDiv(EInteger a, EInteger n) {
      return a.Sign >= 0 ? a.Divide(n) : EInteger.FromInt32(-1).Subtract(
          EInteger.FromInt32(-1).Subtract(a).Divide(n));
    }

    private static EInteger FloorMod(EInteger a, EInteger n) {
      return a.Subtract(FloorDiv(a, n).Multiply(n));
    }

    private static readonly int[] ValueNormalDays = {
      0, 31, 28, 31, 30, 31, 30,
      31, 31, 30,
      31, 30, 31,
    };

    private static readonly int[] ValueLeapDays = {
      0, 31, 29, 31, 30, 31, 30,
      31, 31, 30,
      31, 30, 31,
    };

    private static readonly int[] ValueNormalToMonth = {
      0, 0x1f, 0x3b, 90, 120,
      0x97, 0xb5,
      0xd4, 0xf3, 0x111, 0x130, 0x14e, 0x16d,
    };

    private static readonly int[] ValueLeapToMonth = {
      0, 0x1f, 60, 0x5b, 0x79,
      0x98, 0xb6,
      0xd5, 0xf4, 0x112, 0x131, 0x14f, 0x16e,
    };

    public static void GetNormalizedPartProlepticGregorian(
      EInteger year,
      int month,
      EInteger day,
      EInteger[] dest) {
      // NOTE: This method assumes month is 1 to 12
      if (month <= 0 || month > 12) {
        throw new ArgumentOutOfRangeException(nameof(month));
      }
      int[] dayArray = (year.Remainder(4).Sign != 0 || (
            year.Remainder(100).Sign == 0 && year.Remainder(400).Sign !=
0)) ?
        ValueNormalDays : ValueLeapDays;
      if (day.CompareTo(101) > 0) {
        EInteger count = day.Subtract(100).Divide(146097);
        day = day.Subtract(count.Multiply(146097));
        year = year.Add(count.Multiply(400));
      }
      while (true) {
        EInteger days = EInteger.FromInt32(dayArray[month]);
        if (day.Sign > 0 && day.CompareTo(days) <= 0) {
          break;
        }
        if (day.CompareTo(days) > 0) {
          day = day.Subtract(days);
          if (month == 12) {
            month = 1;
            year = year.Add(1);
            dayArray = (year.Remainder(4).Sign != 0 || (
                  year.Remainder(100).Sign == 0 &&
                  year.Remainder(400).Sign != 0)) ? ValueNormalDays :
              ValueLeapDays;
            } else {
            ++month;
          }
        }
        if (day.Sign <= 0) {
          --month;
          if (month <= 0) {
            year = year.Add(-1);
            month = 12;
          }
          dayArray = (year.Remainder(4).Sign != 0 || (
                year.Remainder(100).Sign == 0 &&
                year.Remainder(400).Sign != 0)) ? ValueNormalDays :
            ValueLeapDays;
          day = day.Add(dayArray[month]);
        }
      }
      dest[0] = year;
      dest[1] = EInteger.FromInt32(month);
      dest[2] = day;
    }

    /*
       // Example: Apple Time is a 32-bit unsigned integer
       // of the number of seconds since the start of 1904.
       // EInteger appleTime = GetNumberOfDaysProlepticGregorian(
         // year, // month
         //,
         day)
       // .Subtract(GetNumberOfDaysProlepticGregorian(
       // EInteger.FromInt32(1904),
       1 // ,
       s1));*/
    public static EInteger GetNumberOfDaysProlepticGregorian(
      EInteger year,
      int month,
      int mday) {
      // NOTE: month = 1 is January, year = 1 is year 1
      if (month <= 0 || month > 12) {
        throw new ArgumentOutOfRangeException(nameof(month));
      }
      if (mday <= 0 || mday > 31) {
        throw new ArgumentOutOfRangeException(nameof(mday));
      }
      EInteger numDays = EInteger.Zero;
      var startYear = 1970;
      if (year.CompareTo(startYear) < 0) {
        EInteger ei = EInteger.FromInt32(startYear - 1);
        EInteger diff = ei.Subtract(year);

        if (diff.CompareTo(401) > 0) {
          EInteger blocks = diff.Subtract(401).Divide(400);
          numDays = numDays.Subtract(blocks.Multiply(146097));
          diff = diff.Subtract(blocks.Multiply(400));
          ei = ei.Subtract(blocks.Multiply(400));
        }

        numDays = numDays.Subtract(diff.Multiply(365));
        var decrement = 1;
        for (;
          ei.CompareTo(year) > 0;
          ei = ei.Subtract(decrement)) {
          if (decrement == 1 && ei.Remainder(4).Sign == 0) {
            decrement = 4;
          }
          if (!(ei.Remainder(4).Sign != 0 || (
                ei.Remainder(100).Sign == 0 && ei.Remainder(400).Sign !=
                0))) {
            numDays = numDays.Subtract(1);
          }
        }
        if (year.Remainder(4).Sign != 0 || (
            year.Remainder(100).Sign == 0 && year.Remainder(400).Sign != 0)) {
          numDays = numDays.Subtract(365 - ValueNormalToMonth[month])
            .Subtract(ValueNormalDays[month] - mday + 1);
          } else {
          numDays = numDays
            .Subtract(366 - ValueLeapToMonth[month])
            .Subtract(ValueLeapDays[month] - mday + 1);
        }
      } else {
        bool isNormalYear = year.Remainder(4).Sign != 0 ||
(year.Remainder(100).Sign == 0 && year.Remainder(400).Sign != 0);

        EInteger ei = EInteger.FromInt32(startYear);
        if (ei.Add(401).CompareTo(year) < 0) {
          EInteger y2 = year.Subtract(2);
          numDays = numDays.Add(
              y2.Subtract(startYear).Divide(400).Multiply(146097));
          ei = y2.Subtract(
              y2.Subtract(startYear).Remainder(400));
        }

        EInteger diff = year.Subtract(ei);
        numDays = numDays.Add(diff.Multiply(365));
        EInteger eileap = ei;
        if (ei.Remainder(4).Sign != 0) {
          eileap = eileap.Add(4 - eileap.Remainder(4).ToInt32Checked());
        }
        numDays = numDays.Add(year.Subtract(eileap).Add(3).Divide(4));
        if (ei.Remainder(100).Sign != 0) {
          ei = ei.Add(100 - ei.Remainder(100).ToInt32Checked());
        }
        while (ei.CompareTo(year) < 0) {
          if (ei.Remainder(400).Sign != 0) {
            numDays = numDays.Subtract(1);
          }
          ei = ei.Add(100);
        }
        int yearToMonth = isNormalYear ? ValueNormalToMonth[month - 1] :
          ValueLeapToMonth[month - 1];
        numDays = numDays.Add(yearToMonth)
          .Add(mday - 1);
      }
      return numDays;
    }

    public static void BreakDownSecondsSinceEpoch(
      EDecimal edec,
      EInteger[] year,
      int[] lesserFields) {
      EInteger integerPart = edec.Quantize(0, ERounding.Floor)
        .ToEInteger();
      EDecimal fractionalPart = edec.Abs()
        .Subtract(EDecimal.FromEInteger(integerPart).Abs());
      int nanoseconds = fractionalPart.Multiply(1000000000)
        .ToInt32Checked();
      var normPart = new EInteger[3];
      EInteger days = FloorDiv(
          integerPart,
          EInteger.FromInt32(86400)).Add(1);
      int secondsInDay = FloorMod(
          integerPart,
          EInteger.FromInt32(86400)).ToInt32Checked();
      GetNormalizedPartProlepticGregorian(
        EInteger.FromInt32(1970),
        1,
        days,
        normPart);
      lesserFields[0] = normPart[1].ToInt32Checked();
      lesserFields[1] = normPart[2].ToInt32Checked();
      lesserFields[2] = secondsInDay / 3600;
      lesserFields[3] = (secondsInDay % 3600) / 60;
      lesserFields[4] = secondsInDay % 60;
      lesserFields[5] = nanoseconds / 100;
      lesserFields[6] = 0;
      year[0] = normPart[0];
    }

    public static bool NameStartsWithWord(String name, String word) {
      int wl = word.Length;
      return name.Length > wl && name.Substring(0, wl).Equals(word,
          StringComparison.Ordinal) && !(name[wl] >= 'a' && name[wl] <=
'z') &&
        !(name[wl] >= '0' && name[wl] <= '9');
    }

    public static String FirstCharLower(String name) {
      if (name.Length > 0 && name[0] >= 'A' && name[0] <= 'Z') {
        var sb = new StringBuilder();
        sb.Append((char)(name[0] + 0x20));
        sb.Append(name.Substring(1));
        return sb.ToString();
      }
      return name;
    }

    public static String FirstCharUpper(String name) {
      if (name.Length > 0 && name[0] >= 'a' && name[0] <= 'z') {
        var sb = new StringBuilder();
        sb.Append((char)(name[0] - 0x20));
        sb.Append(name.Substring(1));
        return sb.ToString();
      }
      return name;
    }

    private static bool IsValidDateTime(int[] dateTime) {
      if (dateTime == null || dateTime.Length < 8) {
        return false;
      }
      if (dateTime[1] < 1 || dateTime[1] > 12 || dateTime[2] < 1) {
        return false;
      }
      bool leap = IsLeapYear(dateTime[0]);
      if (dateTime[1] == 4 || dateTime[1] == 6 || dateTime[1] == 9 ||
        dateTime[1] == 11) {
        if (dateTime[2] > 30) {
          return false;
        }
      } else if (dateTime[1] == 2) {
        if (dateTime[2] > (leap ? 29 : 28)) {
          return false;
        }
      } else {
        if (dateTime[2] > 31) {
          return false;
        }
      }
      return !(dateTime[3] < 0 || dateTime[4] < 0 || dateTime[5] < 0 ||
          dateTime[3] >= 24 || dateTime[4] >= 60 || dateTime[5] >= 61 ||
          dateTime[6] < 0 ||
          dateTime[6] >= 1000000000 || dateTime[7] <= -1440 ||
          dateTime[7] >= 1440);
    }

    private static bool IsLeapYear(int yr) {
      yr %= 400;
      if (yr < 0) {
        yr += 400;
      }
      return (((yr % 4) == 0) && ((yr % 100) != 0)) || ((yr % 400) == 0);
    }

    public static void ParseAtomDateTimeString(
      string str,
      EInteger[] bigYearArray,
      int[] lf) {
      int[] d = ParseAtomDateTimeString(str);
      bigYearArray[0] = EInteger.FromInt32(d[0]);
      Array.Copy(d, 1, lf, 0, 7);
    }

    private static int[] ParseAtomDateTimeString(string str) {
      var bad = false;
      if (str.Length < 19) {
        throw new ArgumentException("Invalid date/time");
      }
      for (var i = 0; i < 19 && !bad; ++i) {
        if (i == 4 || i == 7) {
          bad |= str[i] != '-';
        } else if (i == 13 || i == 16) {
          bad |= str[i] != ':';
        } else if (i == 10) {
          bad |= str[i] != 'T';
          /*lowercase t not used to separate date/time,
          following RFC 4287 sec. 3.3*/ } else {
          bad |= str[i] < '0' || str[i] >
            '9';
        }
      }
      if (bad) {
        throw new ArgumentException("Invalid date/time");
      }
      int year = ((str[0] - '0') * 1000) + ((str[1] - '0') * 100) +
        ((str[2] - '0') * 10) + (str[3] - '0');
      int month = ((str[5] - '0') * 10) + (str[6] - '0');
      int day = ((str[8] - '0') * 10) + (str[9] - '0');
      int hour = ((str[11] - '0') * 10) + (str[12] - '0');
      int minute = ((str[14] - '0') * 10) + (str[15] - '0');
      int second = ((str[17] - '0') * 10) + (str[18] - '0');
      var index = 19;
      var nanoSeconds = 0;
      if (index <= str.Length && str[index] == '.') {
        var icount = 0;
        ++index;
        while (index < str.Length) {
          if (str[index] < '0' || str[index] > '9') {
            break;
          }
          if (icount < 9) {
            nanoSeconds = (nanoSeconds * 10) + (str[index] - '0');
            ++icount;
          }
          ++index;
        }
        while (icount < 9) {
          nanoSeconds *= 10;
          ++icount;
        }
      }
      var utcToLocal = 0;
      if (index + 1 == str.Length && str[index] == 'Z') {
        /*lowercase z not used to indicate UTC,
          following RFC 4287 sec. 3.3*/
        utcToLocal = 0;
      } else if (index + 6 == str.Length) {
        bad = false;
        for (var i = 0; i < 6 && !bad; ++i) {
          if (i == 0) {
            bad |= str[index + i] != '-' && str[index + i] != '+';
          } else if (i == 3) {
            bad |= str[index + i] != ':';
          } else {
            bad |= str[index + i] < '0' || str[index + i] > '9';
          }
        }
        if (bad) {
          throw new ArgumentException("Invalid date/time");
        }
        bool neg = str[index] == '-';
        int tzhour = ((str[index + 1] - '0') * 10) + (str[index + 2] - '0');
        int tzminute = ((str[index + 4] - '0') * 10) + (str[index + 5] - '0');
        if (tzminute >= 60) {
          throw new ArgumentException("Invalid date/time");
        }
        utcToLocal = ((neg ? -1 : 1) * (tzhour * 60)) + tzminute;
      } else {
        throw new ArgumentException("Invalid date/time");
      }
      int[] dt = {
        year, month, day, hour, minute, second,
        nanoSeconds, utcToLocal,
      };
      if (!IsValidDateTime(dt)) {
        throw new ArgumentException("Invalid date/time");
      }
      return dt;
    }

    public static string ToAtomDateTimeString(
      EInteger bigYear,
      int[] lesserFields) {
      if (lesserFields[6] != 0) {
        throw new NotSupportedException(
          "Local time offsets not supported");
      }
      int smallYear = bigYear.ToInt32Checked();
      if (smallYear < 0) {
        throw new ArgumentException("year(" + smallYear +
          ") is not greater or equal to 0");
      }
      if (smallYear > 9999) {
        throw new ArgumentException("year(" + smallYear +
          ") is not less or equal to 9999");
      }
      int month = lesserFields[0];
      int day = lesserFields[1];
      int hour = lesserFields[2];
      int minute = lesserFields[3];
      int second = lesserFields[4];
      int fracSeconds = lesserFields[5];
      var charbuf = new char[32];
      charbuf[0] = (char)('0' + ((smallYear / 1000) % 10));
      charbuf[1] = (char)('0' + ((smallYear / 100) % 10));
      charbuf[2] = (char)('0' + ((smallYear / 10) % 10));
      charbuf[3] = (char)('0' + (smallYear % 10));
      charbuf[4] = '-';
      charbuf[5] = (char)('0' + ((month / 10) % 10));
      charbuf[6] = (char)('0' + (month % 10));
      charbuf[7] = '-';
      charbuf[8] = (char)('0' + ((day / 10) % 10));
      charbuf[9] = (char)('0' + (day % 10));
      charbuf[10] = 'T';
      charbuf[11] = (char)('0' + ((hour / 10) % 10));
      charbuf[12] = (char)('0' + (hour % 10));
      charbuf[13] = ':';
      charbuf[14] = (char)('0' + ((minute / 10) % 10));
      charbuf[15] = (char)('0' + (minute % 10));
      charbuf[16] = ':';
      charbuf[17] = (char)('0' + ((second / 10) % 10));
      charbuf[18] = (char)('0' + (second % 10));
      var charbufLength = 19;
      if (fracSeconds > 0) {
        charbuf[19] = '.';
        ++charbufLength;
        var digitdiv = 100000000;
        var index = 20;
        while (digitdiv > 0 && fracSeconds != 0) {
          int digit = (fracSeconds / digitdiv) % 10;
          fracSeconds -= digit * digitdiv;
          charbuf[index++] = (char)('0' + digit);
          ++charbufLength;
          digitdiv /= 10;
        }
        charbuf[index] = 'Z';
        ++charbufLength;
      } else {
        charbuf[19] = 'Z';
        ++charbufLength;
      }
      return new String(charbuf, 0, charbufLength);
    }

    public static EInteger BigIntegerFromDouble(double dbl) {
      long lvalue = BitConverter.ToInt64(
          BitConverter.GetBytes((double)dbl),
          0);
      int value0 = unchecked((int)(lvalue & 0xffffffffL));
      int value1 = unchecked((int)((lvalue >> 32) & 0xffffffffL));
      var floatExponent = (int)((value1 >> 20) & 0x7ff);
      bool neg = (value1 >> 31) != 0;
      if (floatExponent == 2047) {
        throw new OverflowException("Value is infinity or NaN");
      }
      value1 &= 0xfffff; // Mask out the exponent and sign
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
      EInteger bigmantissa;
      bytes[0] = (byte)(value0 & 0xff);
      bytes[1] = (byte)((value0 >> 8) & 0xff);
      bytes[2] = (byte)((value0 >> 16) & 0xff);
      bytes[3] = (byte)((value0 >> 24) & 0xff);
      bytes[4] = (byte)(value1 & 0xff);
      bytes[5] = (byte)((value1 >> 8) & 0xff);
      bytes[6] = (byte)((value1 >> 16) & 0xff);
      bytes[7] = (byte)((value1 >> 24) & 0xff);
      bytes[8] = (byte)0;
      bigmantissa = EInteger.FromBytes(bytes, true);
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
          bigmantissa = -(EInteger)bigmantissa;
        }
        return bigmantissa;
      } else {
        // Value has a fractional part
        int exp = -floatExponent;
        bigmantissa >>= exp;
        if (neg) {
          bigmantissa = -(EInteger)bigmantissa;
        }
        return bigmantissa;
      }
    }

    private static int RoundedShift(long mant, int shift) {
      long mask = (1L << shift) - 1;
      long half = 1L << (shift - 1);
      long shifted = mant >> shift;
      long masked = mant & mask;
      return (masked > half || (masked == half && (shifted & 1L) != 0)) ?
        unchecked((int)shifted) + 1 : unchecked((int)shifted);
    }

    private static int RoundedShift(int mant, int shift) {
      int mask = (1 << shift) - 1;
      int half = 1 << (shift - 1);
      int shifted = mant >> shift;
      int masked = mant & mask;
      return (masked > half || (masked == half && (shifted & 1) != 0)) ?
        shifted + 1 : shifted;
    }

    public static int DoubleToHalfPrecisionIfSameValue(long bits) {
      int exp = unchecked((int)((bits >> 52) & 0x7ffL));
      long mant = bits & 0xfffffffffffffL;
      int sign = unchecked((int)(bits >> 48)) & (1 << 15);
      int sexp = exp - 1008;
      // DebugUtility.Log("bits={0:X8}, exp=" + exp + " sexp=" + (sexp));
      if (exp == 2047) { // Infinity and NaN
        int newmant = unchecked((int)(mant >> 42));
        return ((mant & ((1L << 42) - 1)) == 0) ? (sign | 0x7c00 | newmant) :
          -1;
        } else if (sexp >= 31) { // overflow
        return -1;
      } else if (sexp < -10) { // underflow
        return -1;
      } else if (sexp > 0) { // normal
        return ((mant & ((1L << 42) - 1)) == 0) ? (sign | (sexp << 10) |
            RoundedShift(mant, 42)) : -1;
      } else { // subnormal and zero
        return ((mant & ((1L << (42 - (sexp - 1))) - 1)) == 0) ? (sign |
            RoundedShift(mant | (1L << 52), 42 - (sexp - 1))) : -1;
      }
    }

    public static bool DoubleRetainsSameValueInSingle(long bits) {
      int exp = unchecked((int)((bits >> 52) & 0x7ffL));
      long mant = bits & 0xfffffffffffffL;
      int sexp = exp - 896;
      // DebugUtility.Log("sng mant={0:X8}, exp=" + exp + " sexp=" + (sexp));
      if (exp == 2047) { // Infinity and NaN
        return (mant & ((1L << 29) - 1)) == 0;
      } else if (sexp < -23 || sexp >= 255) { // underflow or overflow
        return false;
      } else if (sexp > 0) { // normal
        return (mant & ((1L << 29) - 1)) == 0;
      } else { // subnormal and zero
        return (mant & ((1L << (29 - (sexp - 1))) - 1)) == 0;
      }
    }

    // NOTE: Rounds to nearest, ties to even
    public static int SingleToRoundedHalfPrecision(int bits) {
      int exp = unchecked((int)((bits >> 23) & 0xff));
      int mant = bits & 0x7fffff;
      int sign = (bits >> 16) & (1 << 15);
      int sexp = exp - 112;
      if (exp == 255) { // Infinity and NaN
        int newmant = unchecked((int)(mant >> 13));
        return (mant != 0 && newmant == 0) ?
          // signaling NaN truncated to have mantissa 0
          (sign | 0x7c01) : (sign | 0x7c00 | newmant);
        } else if (sexp >= 31) { // overflow
        return sign | 0x7c00;
      } else if (sexp < -10) { // underflow
        return sign;
      } else if (sexp > 0) { // normal
        return sign | (sexp << 10) | RoundedShift(mant, 13);
      } else { // subnormal and zero
        return sign | RoundedShift(mant | (1 << 23), 13 - (sexp - 1));
      }
    }

    // NOTE: Rounds to nearest, ties to even
    public static int DoubleToRoundedHalfPrecision(long bits) {
      int exp = unchecked((int)((bits >> 52) & 0x7ffL));
      long mant = bits & 0xfffffffffffffL;
      int sign = unchecked((int)(bits >> 48)) & (1 << 15);
      int sexp = exp - 1008;
      if (exp == 2047) { // Infinity and NaN
        int newmant = unchecked((int)(mant >> 42));
        return (mant != 0 && newmant == 0) ?
          // signaling NaN truncated to have mantissa 0
          (sign | 0x7c01) : (sign | 0x7c00 | newmant);
        } else if (sexp >= 31) { // overflow
        return sign | 0x7c00;
      } else if (sexp < -10) { // underflow
        return sign;
      } else if (sexp > 0) { // normal
        return sign | (sexp << 10) | RoundedShift(mant, 42);
      } else { // subnormal and zero
        return sign | RoundedShift(mant | (1L << 52), 42 - (sexp - 1));
      }
    }

    // NOTE: Rounds to nearest, ties to even
    public static int DoubleToRoundedSinglePrecision(long bits) {
      int exp = unchecked((int)((bits >> 52) & 0x7ffL));
      long mant = bits & 0xfffffffffffffL;
      int sign = unchecked((int)(bits >> 32)) & (1 << 31);
      int sexp = exp - 896;
      if (exp == 2047) { // Infinity and NaN
        int newmant = unchecked((int)(mant >> 29));
        return (mant != 0 && newmant == 0) ?
          // signaling NaN truncated to have mantissa 0
          (sign | 0x7f800001) : (sign | 0x7f800000 | newmant);
        } else if (sexp >= 255) { // overflow
        return sign | 0x7f800000;
      } else if (sexp < -23) { // underflow
        return sign;
      } else if (sexp > 0) { // normal
        return sign | (sexp << 23) | RoundedShift(mant, 29);
      } else { // subnormal and zero
        return sign | RoundedShift(mant | (1L << 52), 29 - (sexp - 1));
      }
    }

    public static int SingleToHalfPrecisionIfSameValue(float f) {
      int bits = BitConverter.ToInt32(BitConverter.GetBytes((float)f), 0);
      int exp = (bits >> 23) & 0xff;
      int mant = bits & 0x7fffff;
      int sign = (bits >> 16) & 0x8000;
      if (exp == 255) { // Infinity and NaN
        return (bits & 0x1fff) == 0 ? sign + 0x7c00 + (mant >> 13) : -1;
      } else if (exp == 0) { // Subnormal
        return (bits & 0x1fff) == 0 ? sign + (mant >> 13) : -1;
      }
      if (exp <= 102 || exp >= 143) { // Overflow or underflow
        return -1;
      } else if (exp <= 112) { // Subnormal
        int shift = 126 - exp;
        return (bits & ((1 << shift) - 1)) == 0 ? sign +
          (1024 >> (145 - exp)) + (mant >> shift) : -1;
        } else {
        return (bits & 0x1fff) == 0 ? sign + ((exp - 112) << 10) +
          (mant >> 13) : -1;
      }
    }

    public static long SingleToDoublePrecision(int bits) {
      var negvalue = (long)((bits >> 31) & 1) << 63;
      int exp = (bits >> 23) & 0xff;
      int mant = bits & 0x7fffff;
      long value = 0;
      if (exp == 255) {
        value = 0x7ff0000000000000L | ((long)mant << 29) | negvalue;
      } else if (exp == 0) {
        if (mant == 0) {
          value = negvalue;
        } else {
          ++exp;
          while (mant < 0x800000) {
            mant <<= 1;
            --exp;
          }
          value = ((long)(exp + 896) << 52) | ((long)(mant & 0x7fffff) <<
              29) | negvalue;
        }
      } else {
        value = ((long)(exp + 896) << 52) | ((long)mant << 29) | negvalue;
      }
      return value;
    }

    public static long HalfToDoublePrecision(int bits) {
      var negvalue = (long)(bits & 0x8000) << 48;
      int exp = (bits >> 10) & 31;
      int mant = bits & 0x3ff;
      long value = 0;
      if (exp == 31) {
        value = 0x7ff0000000000000L | ((long)mant << 42) | negvalue;
      } else if (exp == 0) {
        if (mant == 0) {
          value = negvalue;
        } else {
          ++exp;
          while (mant < 0x400) {
            mant <<= 1;
            --exp;
          }
          value = ((long)(exp + 1008) << 52) | ((long)(mant & 0x3ff) << 42) |
            negvalue;
        }
      } else {
        value = ((long)(exp + 1008) << 52) | ((long)mant << 42) | negvalue;
      }
      return value;
    }
  }
}
