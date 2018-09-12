/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Cbor {
  internal class CBORTag0 : ICBORTag, ICBORObjectConverter<DateTime> {
    private static string DateTimeToString(DateTime bi) {
#if NET20
      DateTime dt = bi.ToUniversalTime();
#else
      DateTime dt = TimeZoneInfo.ConvertTime(bi, TimeZoneInfo.Utc);
#endif

      int year = dt.Year;
      int month = dt.Month;
      int day = dt.Day;
      int hour = dt.Hour;
      int minute = dt.Minute;
      int second = dt.Second;
      int millisecond = dt.Millisecond;
      var charbuf = new char[millisecond > 0 ? 24 : 20];
      charbuf[0] = (char)('0' + ((year / 1000) % 10));
      charbuf[1] = (char)('0' + ((year / 100) % 10));
      charbuf[2] = (char)('0' + ((year / 10) % 10));
      charbuf[3] = (char)('0' + (year % 10));
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
      if (millisecond > 0) {
        charbuf[19] = '.';
        charbuf[20] = (char)('0' + ((millisecond / 100) % 10));
        charbuf[21] = (char)('0' + ((millisecond / 10) % 10));
        charbuf[22] = (char)('0' + (millisecond % 10));
        charbuf[23] = 'Z';
      } else {
        charbuf[19] = 'Z';
      }
      return new String(charbuf);
    }

    internal static void AddConverter() {
      CBORObject.AddConverter(typeof(DateTime), new CBORTag0());
    }

    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.TextString;
    }

    public CBORObject ValidateObject(CBORObject obj) {
      if (obj.Type != CBORType.TextString) {
        throw new CBORException("Not a text string");
      }
      return obj;
    }

    public DateTime FromCBORObject(CBORObject obj) {
      if (!obj.HasTag(0)) {
        throw new CBORException("Not tag 0");
      }
      return StringToDateTime(obj.AsString());
    }

    public static DateTime StringToDateTime(string str) {
      var bad = false;
      if (str.Length < 19) {
        throw new ArgumentException("Invalid date time");
      }
      for (var i = 0; i < 19 && !bad; ++i) {
        if (i == 4 || i == 7) {
            bad |= str[i] != '-';
        } else if (i == 13 || i == 16) {
          {
            bad |= str[i] != ':';
          }
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
      int year = (str[0] - '0') * 1000 + (str[1] - '0') * 100 +
        (str[2] - '0') * 10 + (str[3] - '0');
      int month = (str[5] - '0') * 10 + (str[6] - '0');
      if (month >= 12) {
        throw new ArgumentException("Invalid date/time");
      }
      int day = (str[8] - '0') * 10 + (str[9] - '0');
      int hour = (str[11] - '0') * 10 + (str[12] - '0');
      int minute = (str[14] - '0') * 10 + (str[15] - '0');
      if (minute >= 60) {
        throw new ArgumentException("Invalid date/time");
      }
      int second = (str[17] - '0') * 10 + (str[18] - '0');
      var index = 19;
      var tenthsMicrosec = 0;
      if (index <= str.Length && str[index] == '.') {
        var count = 0;
        ++index;
        while (index < str.Length) {
          if (str[index] < '0' || str[index] > '9') {
            break;
          }
          if (count < 7) {
            {
              tenthsMicrosec = tenthsMicrosec * 10 + (str[index] - '0');
            }
            ++count;
          }
          ++index;
        }
        while (count < 7) {
          {
            tenthsMicrosec *= 10;
          }
          ++count;
        }
      }
      if (index + 1 == str.Length && str[index] == 'Z') {
        /*lowercase z not used to indicate UTC,
          following RFC 4287 sec. 3.3*/
        return new DateTime(
  year,
  month,
  day,
  hour,
  minute,
  second,
  DateTimeKind.Utc).AddTicks((long)tenthsMicrosec);
      } else if (index + 6 == str.Length) {
        bad = false;
        for (var i = 0; i < 6 && !bad; ++i) {
          if (i == 0) {
            {
              bad |= str[index + i] != '-' && str[index + i] != '+';
            }
          } else if (i == 3) {
            {
              bad |= str[index + i] != ':';
            }
          } else {
            bad |= str[index + i] < '0' || str[index + i] > '9';
          }
        }
        if (bad) {
          throw new ArgumentException("Invalid date/time");
        }
        bool neg = str[index] == '-';
        int tzhour = (str[index + 1] - '0') * 10 + (str[index + 2] - '0');
        int tzminute = (str[index + 4] - '0') * 10 + (str[index + 5] - '0');
        if (tzminute >= 60) {
          throw new ArgumentException("Invalid date/time");
        }
        int localToUtc = (neg ? 1 : -1) * (tzhour * 60) + tzminute;
        return new DateTime(
  year,
  month,
  day,
  hour,
  minute,
  second,
  DateTimeKind.Utc).AddMinutes(localToUtc).AddTicks((long)tenthsMicrosec);
      } else {
        throw new ArgumentException("Invalid date/time");
      }
    }

    public CBORObject ToCBORObject(DateTime obj) {
      return CBORObject.FromObjectAndTag(DateTimeToString(obj), 0);
    }
  }
}
