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
      EInteger year,
      int[] lesserFields) {
       try {
       CBORObject obj = dtc.DateTimeFieldsToCBORObject(year, lesserFields);
       var newYear = new EInteger[1];
       var newLesserFields = new int[7];
       string fieldsString = (String.Empty + year + "," + lesserFields[0] +
"," + lesserFields[1] + "," + lesserFields[2] + "," + lesserFields[3] + ","+
lesserFields[4] + "," + lesserFields[5] + "," + lesserFields[6]);
       if (dtc.TryGetDateTimeFields(obj, newYear, newLesserFields)) {
         Assert.AreEqual(
           lesserFields,
           newLesserFields,
           "lesserFields\n" + fieldsString);
         Assert.AreEqual(year, newYear[0], "year\n" + fieldsString);
       } else {
         Assert.Fail(fieldsString);
       }
       } catch (Exception ex) {
         throw new InvalidOperationException(ex.Message+ "\n" +
fieldsString, ex);
       }
    }

    private static EInteger RandomYear(IRandomGenExtended irg) {
       return EInteger.FromInt32(irg.GetInt32(9998) + 1);
    }

    private static bool IsLeapYear(EInteger year) {
      year = year.Remainder(400);
      if (year.Sign < 0) {
        year = year.Add(400);
      }
      return ((year.Remainder(4).Sign == 0) && (year.Remainder(100).Sign !=
0)) ||
          (year.Remainder(400).Sign == 0);
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
         CBORDateConverter.TaggedNumber,
         CBORDateConverter.UntaggedNumber,
       };
       var rg = new RandomGenerator();
       for (var i = 0; i < 1000; ++i) {
          EInteger year = RandomYear(rg);
          int[] lesserFields = RandomLesserFields(rg, year);
          for (var j = 0; j < dtcs.Length; ++j) {
            DateConverterRoundTripOne(dtcs[j], year, lesserFields);
          }
       }
    }
  }
}
