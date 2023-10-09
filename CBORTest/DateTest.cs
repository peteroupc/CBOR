using System;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;
using PeterO.Numbers;

namespace Test {
  [TestFixture]
  public class DateTest {
    public static void DateConverterRoundTripOne(
      CBORDateConverter dtc,
      int smallYear,
      int[] lesserFields) {
      DateConverterRoundTripOne(
        dtc,
        EInteger.FromInt32(smallYear),
        lesserFields);
    }

    public static void DateConverterRoundTripOne(
      CBORDateConverter dtc,
      EInteger year,
      int[] lesserFields) {
      if (lesserFields == null) {
        throw new ArgumentNullException(nameof(lesserFields));
      }
      if (year == null) {
        throw new ArgumentNullException(nameof(year));
      }
      string yearString = year.ToString();
      string fieldsString = yearString + "," + lesserFields[0] + "," +
lesserFields[1] + "," + lesserFields[2] + "," + lesserFields[3] + "," +
lesserFields[4] + "," + lesserFields[5] + "," + lesserFields[6];
      try {
        if (dtc == null) {
          throw new ArgumentNullException(nameof(dtc));
        }
        CBORObject obj = dtc.DateTimeFieldsToCBORObject(year, lesserFields);
        fieldsString += "\n" + obj.ToString();
        var newYear = new EInteger[1];
        var newLesserFields = new int[7];
        if (dtc.TryGetDateTimeFields(obj, newYear, newLesserFields)) {
          for (int i = 0; i < lesserFields.Length; ++i) {
            Assert.AreEqual(
              lesserFields[i],
              newLesserFields[i],
              fieldsString);
          }
          Assert.AreEqual(year, newYear[0], "year\n" + fieldsString);
        } else {
          Assert.Fail(fieldsString);
        }
      } catch (Exception ex) {
        throw new InvalidOperationException(
          ex.Message + "\n" + fieldsString,
          ex);
      }
    }

    private static EInteger RandomYear(IRandomGenExtended irg) {
      return EInteger.FromInt32(irg.GetInt32(9998) + 1);
    }

    private static EInteger RandomExpandedYear(IRandomGenExtended irg) {
      return EInteger.FromInt32(irg.GetInt32(1000000) - 500000);
    }

    private static bool IsLeapYear(EInteger bigYear) {
      bigYear = bigYear.Remainder(400);
      if (bigYear.Sign < 0) {
        bigYear = bigYear.Add(400);
      }
      return ((bigYear.Remainder(4).Sign == 0) &&
(bigYear.Remainder(100).Sign !=
0)) ||
          (bigYear.Remainder(400).Sign == 0);
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

    private static int[] RandomLesserFields(IRandomGenExtended irg, EInteger
year) {
      int month = irg.GetInt32(12) + 1;
      int days = IsLeapYear(year) ? ValueLeapDays[month] :
ValueNormalDays[month];
      return new int[] {
        month,
        irg.GetInt32(days) + 1,
        irg.GetInt32(24),
        irg.GetInt32(60),
        irg.GetInt32(60),
        irg.GetInt32(1000000000),
        0,
      };
    }

    [Test]
    public void DateConverterRoundTrip() {
      var dtcs = new CBORDateConverter[] {
        CBORDateConverter.TaggedString,
      };
      int[] lesserFields;
      var rg = new RandomGenerator();
      for (int i = 0; i < 10000; ++i) {
        EInteger year = RandomYear(rg);
        lesserFields = RandomLesserFields(rg, year);
        for (int j = 0; j < dtcs.Length; ++j) {
          DateConverterRoundTripOne(dtcs[j], year, lesserFields);
        }
      }
      dtcs = new CBORDateConverter[] {
        CBORDateConverter.TaggedNumber,
        CBORDateConverter.UntaggedNumber,
      };
      for (int i = 0; i < 30000; ++i) {
        EInteger year = RandomExpandedYear(rg);
        lesserFields = RandomLesserFields(rg, year);
        // Don't check fractional seconds because conversion is lossy
        lesserFields[5] = 0;
        for (int j = 0; j < dtcs.Length; ++j) {
          DateConverterRoundTripOne(dtcs[j], year, lesserFields);
        }
      }
      lesserFields = new int[] {
        2,
        11,
        7,
        59,
        3,
        0,
        0,
      };
      DateConverterRoundTripOne(
        CBORDateConverter.TaggedString,
        9328,
        lesserFields);
      DateConverterRoundTripOne(
        CBORDateConverter.UntaggedNumber,
        9328,
        lesserFields);
    }
  }
}
