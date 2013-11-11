/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 11/2/2013
 * Time: 3:26 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;
using System.Numerics;
using System.Globalization;
using PeterO;

namespace Test {
  [TestFixture]
  public class CBORExtraTest {

    private decimal RandomDecimal(System.Random rand, int exponent) {
      int[] x = new int[4];
      int r = rand.Next(0x10000);
      r |= ((int)rand.Next(0x10000)) << 16;
      x[0] = r;
      if (rand.Next(2) == 0) {
        r = rand.Next(0x10000);
        r |= ((int)rand.Next(0x10000)) << 16;
        x[1] = r;
        if (rand.Next(2) == 0) {
          r = rand.Next(0x10000);
          r |= ((int)rand.Next(0x10000)) << 16;
          x[2] = r;
        }
      }
      x[3] = (exponent << 16);
      if (rand.Next(2) == 0) {
        x[3] |= (1 << 31);
      }
      return new Decimal(x);
    }

    [Test]
    public void TestCBORObjectDecimal() {
      System.Random rand = new System.Random();
      for (int i = 0; i <= 28; i++) { // Try a random decimal with a given exponent
        for (int j = 0; j < 8; j++) {
          decimal d = RandomDecimal(rand, i);
          CBORObject obj = CBORObject.FromObject(d);
          TestCommon.AssertRoundTrip(obj);
          Assert.AreEqual(d, obj.AsDecimal());
        }
      }
    }


    [Test]
    public void TestSByte() {
      for (int i = SByte.MinValue; i <= SByte.MaxValue; i++) {
        TestCommon.AssertSer(
          CBORObject.FromObject((sbyte)i),
          String.Format(CultureInfo.InvariantCulture, "{0}", i));
      }
    }


    private static string DateTimeToString(DateTime bi) {
      DateTime dt = bi.ToUniversalTime();
      int year = dt.Year;
      int month = dt.Month;
      int day = dt.Day;
      int hour = dt.Hour;
      int minute = dt.Minute;
      int second = dt.Second;
      int millisecond = dt.Millisecond;
      char[] charbuf = new char[millisecond > 0 ? 24 : 20];
      charbuf[0] = (char)('0' + ((year / 1000) % 10));
      charbuf[1] = (char)('0' + ((year / 100) % 10));
      charbuf[2] = (char)('0' + ((year / 10) % 10));
      charbuf[3] = (char)('0' + ((year) % 10));
      charbuf[4] = '-';
      charbuf[5] = (char)('0' + ((month / 10) % 10));
      charbuf[6] = (char)('0' + ((month) % 10));
      charbuf[7] = '-';
      charbuf[8] = (char)('0' + ((day / 10) % 10));
      charbuf[9] = (char)('0' + ((day) % 10));
      charbuf[10] = 'T';
      charbuf[11] = (char)('0' + ((hour / 10) % 10));
      charbuf[12] = (char)('0' + ((hour) % 10));
      charbuf[13] = ':';
      charbuf[14] = (char)('0' + ((minute / 10) % 10));
      charbuf[15] = (char)('0' + ((minute) % 10));
      charbuf[16] = ':';
      charbuf[17] = (char)('0' + ((second / 10) % 10));
      charbuf[18] = (char)('0' + ((second) % 10));
      if (millisecond > 0) {
        charbuf[19] = '.';
        charbuf[20] = (char)('0' + ((millisecond / 100) % 10));
        charbuf[21] = (char)('0' + ((millisecond / 10) % 10));
        charbuf[22] = (char)('0' + ((millisecond) % 10));
        charbuf[23] = 'Z';
      } else {
        charbuf[19] = 'Z';
      }
      return new String(charbuf);
    }


    [Test]
    public void TestULong() {
      ulong[] ranges = new ulong[]{
        0,65539,
        0xFFFFF000UL,0x100000400UL,
        0x7FFFFFFFFFFFF000UL,0x8000000000000400UL,
        UInt64.MaxValue-1000,UInt64.MaxValue
      };
      for (int i = 0; i < ranges.Length; i += 2) {
        ulong j = ranges[i];
        while (true) {
          TestCommon.AssertSer(
            CBORObject.FromObject(j),
            String.Format(CultureInfo.InvariantCulture, "{0}", j));
          Assert.AreEqual(
            CBORObject.FromObject(j),
            CBORObject.FromObject((BigInteger)j));

          if (j == ranges[i + 1]) break;
          j++;
        }
      }
    }

    [Test]
    public void TestUInt() {
      uint[] ranges = new uint[]{
        0,65539,
        0x7FFFF000U,0x80000400U,
        UInt32.MaxValue-1000,UInt32.MaxValue
      };
      for (int i = 0; i < ranges.Length; i += 2) {
        uint j = ranges[i];
        while (true) {
          TestCommon.AssertSer(
            CBORObject.FromObject(j),
            String.Format(CultureInfo.InvariantCulture, "{0}", j));
          Assert.AreEqual(
            CBORObject.FromObject(j),
            CBORObject.FromObject((BigInteger)j));

          if (j == ranges[i + 1]) break;
          j++;
        }
      }
    }



    [Test]
    public void TestDecimal() {
      TestCommon.AssertSer(
        CBORObject.FromObject(Decimal.MinValue),
        String.Format(CultureInfo.InvariantCulture, "{0}", Decimal.MinValue));
      TestCommon.AssertSer(
        CBORObject.FromObject(Decimal.MaxValue),
        String.Format(CultureInfo.InvariantCulture, "{0}", Decimal.MaxValue));
      for (int i = -100; i <= 100; i++) {
        TestCommon.AssertSer(
          CBORObject.FromObject((decimal)i),
          String.Format(CultureInfo.InvariantCulture, "{0}", i));
        TestCommon.AssertSer(
          CBORObject.FromObject((decimal)i + 0.1m),
          String.Format(CultureInfo.InvariantCulture, "{0}", (decimal)i + 0.1m));
        TestCommon.AssertSer(
          CBORObject.FromObject((decimal)i + 0.1111m),
          String.Format(CultureInfo.InvariantCulture, "{0}", (decimal)i + 0.1111m));
      }
    }

    [Test]
    public void TestUShort() {
      for (int i = UInt16.MinValue; i <= UInt16.MaxValue; i++) {
        TestCommon.AssertSer(
          CBORObject.FromObject((UInt16)i),
          String.Format(CultureInfo.InvariantCulture, "{0}", i));
      }
    }

    [Test]
    public void TestDoubleToOtherII() {
      CBORObject dbl1 = CBORObject.FromObject((double)Int32.MinValue);
      CBORObject dbl2 = CBORObject.FromObject((double)Int32.MaxValue);
      try { dbl1.AsUInt16(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { dbl1.AsSByte(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { dbl1.AsUInt32(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { dbl1.AsUInt64(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { dbl2.AsUInt16(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { dbl2.AsSByte(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { dbl2.AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { dbl2.AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
    }


    [Test]
    public void TestDateTime() {
      DateTime[] ranges = new DateTime[]{
        new DateTime(1,1,1,0,0,0,DateTimeKind.Utc),
        new DateTime(100,1,1,0,0,0,DateTimeKind.Utc),
        new DateTime(1998,1,1,0,0,0,DateTimeKind.Utc),
        new DateTime(2030,1,1,0,0,0,DateTimeKind.Utc),
        new DateTime(9998,1,1,0,0,0,DateTimeKind.Utc),
        new DateTime(9999,12,31,23,59,59,DateTimeKind.Utc)
      };
      for (int i = 0; i < ranges.Length; i += 2) {
        DateTime j = ranges[i];
        while (true) {
          TestCommon.AssertSer(
            CBORObject.FromObject(j),
            "0(\"" + DateTimeToString(j) + "\")");
          if (j >= ranges[i + 1]) break;
          try {
            j = j.AddHours(10);
          } catch (ArgumentOutOfRangeException) {
            // Can't add more hours, so break
            break;
          }
        }
      }
    }
  }
}
