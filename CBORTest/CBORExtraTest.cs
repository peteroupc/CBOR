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
//using System.Numerics;
using System.Globalization;
using PeterO;
namespace Test {
    /// <summary>
    /// </summary>
    /// <remarks/>
[TestFixture]
  public class CBORExtraTest{
  
  
    [Test]
    public void GenerateDecimalTests(){
      FastRandom r = new FastRandom();
      for(int i=0;i<5000;i++){
        CBORObject o1=CBORTest.RandomNumber(r);
        var df=o1.AsDecimalFraction();
        try {
          decimal s=Decimal.Parse(df.ToPlainString());
          try {
            var df2=df.RoundToBinaryPrecision(
              new PrecisionContext(96,Rounding.HalfEven,0,28,true));
            if(df2.Exponent<-28 || df2.Exponent>0){
              Console.WriteLine(df2);
            }
            Assert.AreEqual(s.ToString(),df2.ToPlainString());
          } catch(Exception){
            Console.WriteLine(
              "Assert.AreEqual(\""+s.ToString()+"\",DecimalFraction.FromString(\""+df.ToString()+"\")"+
              ".RoundToBinaryPrecision(new PrecisionContext(96,Rounding.HalfEven,0,28,false)).ToPlainString());"
             );
            throw;
          }
        } catch(OverflowException){
          try {
            Assert.AreEqual(null,df.RoundToBinaryPrecision(
              new PrecisionContext(96,Rounding.HalfEven,0,28,false)));
          } catch(Exception){
            Console.WriteLine(
              "Assert.AreEqual(null,DecimalFraction.FromString(\""+df.ToString()+"\")"+
              ".RoundToBinaryPrecision(new PrecisionContext(96,Rounding.HalfEven,0,28,false)));"
             );
            throw;
          }
        }
      }
    }
    private decimal RandomDecimal(FastRandom rand, int exponent) {
      int[] x = new int[4];
      int r = rand.NextValue(0x10000);
      r |= ((int)rand.NextValue(0x10000)) << 16;
      x[0] = r;
      if (rand.NextValue(2) == 0) {
        r = rand.NextValue(0x10000);
        r |= ((int)rand.NextValue(0x10000)) << 16;
        x[1] = r;
        if (rand.NextValue(2) == 0) {
          r = rand.NextValue(0x10000);
          r |= ((int)rand.NextValue(0x10000)) << 16;
          x[2] = r;
        }
      }
      x[3] = (exponent << 16);
      if (rand.NextValue(2) == 0) {
        x[3] |= (1 << 31);
      }
      return new Decimal(x);
    }
  
  
    /// <summary>
    /// </summary>
    /// <returns>
    /// </returns>
    /// <remarks/>
[Test]
    public void TestCBORObjectDecimal() {
      FastRandom rand = new FastRandom();
      for (int i = 0; i <= 28; i++) { // Try a random decimal with a given exponent
        for (int j = 0; j < 8; j++) {
          decimal d = RandomDecimal(rand, i);
          CBORObject obj = CBORObject.FromObject(d);
          TestCommon.AssertRoundTrip(obj);
          Assert.AreEqual(d, obj.AsDecimal());
        }
      }
    }
    /// <summary>
    /// </summary>
    /// <returns>
    /// </returns>
    /// <remarks/>
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
    /// <summary>
    /// </summary>
    /// <returns>
    /// </returns>
    /// <remarks/>
[Test]
    public void TestFloatCloseToEdge(){
      try { CBORObject.FromObject(2.147483647E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483647E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483647E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483647E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836470000002E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836470000002E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836470000002E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836470000002E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836469999998E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836469999998E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836469999998E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836469999998E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483648E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483648E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483648E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483648E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836480000005E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836480000005E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836480000005E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836480000005E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836479999998E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836479999998E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836479999998E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836479999998E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483646E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483646E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483646E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483646E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836460000002E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836460000002E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836460000002E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836460000002E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836459999998E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836459999998E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836459999998E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836459999998E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483648E9d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483648E9d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483648E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483648E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836479999998E9d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836479999998E9d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836479999998E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836479999998E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836480000005E9d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836480000005E9d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836480000005E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836480000005E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483647E9d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483647E9d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483647E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483647E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836469999998E9d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836469999998E9d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836469999998E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836469999998E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836470000002E9d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836470000002E9d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836470000002E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836470000002E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483649E9d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483649E9d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483649E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483649E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836489999995E9d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836489999995E9d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836489999995E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836489999995E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836490000005E9d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836490000005E9d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836490000005E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836490000005E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854776E18d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854776E18d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854776E18d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854776E18d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854778E18d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854778E18d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854778E18d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854778E18d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233720368547748E18d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233720368547748E18d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233720368547748E18d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233720368547748E18d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854776E18d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854776E18d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854776E18d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854776E18d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233720368547748E18d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233720368547748E18d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233720368547748E18d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233720368547748E18d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854778E18d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854778E18d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854778E18d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854778E18d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.000000000004d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.000000000004d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.000000000004d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.000000000004d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.999999999996d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.999999999996d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.999999999996d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.999999999996d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.00000000001d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.00000000001d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.00000000001d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.00000000001d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.999999999996d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.999999999996d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.999999999996d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.999999999996d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.000000000004d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.000000000004d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.000000000004d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.000000000004d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.999999999996d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.999999999996d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.999999999996d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.999999999996d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.999999999996d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.999999999996d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.999999999996d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.999999999996d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.00000000001d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.00000000001d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.00000000001d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.00000000001d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.999999999996d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.999999999996d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.999999999996d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.999999999996d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.000000000004d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.000000000004d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.000000000004d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.000000000004d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.99999999999d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.99999999999d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.99999999999d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.99999999999d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.00000000001d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.00000000001d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.00000000001d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.00000000001d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.9E-324d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.9E-324d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.9E-324d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.9E-324d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-4.9E-324d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-4.9E-324d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-4.9E-324d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-4.9E-324d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000000000000002d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000000000000002d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000000000000002d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000000000000002d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.9999999999999999d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.9999999999999999d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.9999999999999999d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.9999999999999999d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.9999999999999999d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.9999999999999999d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.9999999999999999d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.9999999999999999d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000000000000002d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000000000000002d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000000000000002d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000000000000002d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00000000000003d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00000000000003d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00000000000003d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00000000000003d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99999999999997d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99999999999997d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99999999999997d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99999999999997d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00000000000006d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00000000000006d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00000000000006d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00000000000006d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99999999999997d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99999999999997d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99999999999997d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99999999999997d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00000000000003d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00000000000003d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00000000000003d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00000000000003d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99999999999997d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99999999999997d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99999999999997d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99999999999997d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748365E9f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748365E9f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748365E9f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748365E9f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474839E9f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474839E9f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474839E9f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474839E9f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748352E9f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748352E9f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748352E9f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748352E9f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748365E9f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748365E9f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748365E9f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748365E9f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748352E9f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748352E9f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748352E9f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748352E9f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474839E9f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474839E9f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474839E9f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474839E9f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372E18f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372E18f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372E18f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372E18f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223373E18f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223373E18f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223373E18f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223373E18f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233715E18f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233715E18f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233715E18f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233715E18f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372E18f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372E18f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372E18f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372E18f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233715E18f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233715E18f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233715E18f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233715E18f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223373E18f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223373E18f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223373E18f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223373E18f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.002f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.002f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.002f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.002f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.998f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.998f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.998f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.998f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.004f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.004f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.004f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.004f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.998f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.998f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.998f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.998f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.002f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.002f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.002f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.002f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.998f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.998f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.998f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.998f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.998f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.998f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.998f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.998f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.004f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.004f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.004f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.004f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.998f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.998f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.998f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.998f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.002f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.002f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.002f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.002f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.996f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.996f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.996f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.996f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.004f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.004f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.004f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.004f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.4E-45f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.4E-45f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.4E-45f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.4E-45f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.4E-45f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.4E-45f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.4E-45f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.4E-45f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000001f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000001f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000001f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000001f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.99999994f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.99999994f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.99999994f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.99999994f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.99999994f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.99999994f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.99999994f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.99999994f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000001f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000001f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000001f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000001f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00002f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00002f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00002f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00002f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99998f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99998f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99998f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99998f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00003f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00003f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00003f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00003f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99998f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99998f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99998f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99998f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00002f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00002f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00002f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00002f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99998f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99998f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99998f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99998f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.0d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.0d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.0d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.0d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.00000000001d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.00000000001d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.00000000001d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.00000000001d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.99999999999d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.99999999999d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.99999999999d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.99999999999d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.0d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.0d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.0d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.0d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.00000000001d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.00000000001d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.00000000001d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.00000000001d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.99999999999d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.99999999999d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.99999999999d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.99999999999d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.0d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.0d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.0d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.0d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.00000000001d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.00000000001d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.00000000001d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.00000000001d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65533.99999999999d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65533.99999999999d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65533.99999999999d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65533.99999999999d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967295E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967295E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967295E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967295E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672950000005E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672950000005E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672950000005E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672950000005E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672949999995E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672949999995E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672949999995E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672949999995E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967296E9d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967296E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967296E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967296E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967296000001E9d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967296000001E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967296000001E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967296000001E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672959999995E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672959999995E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672959999995E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672959999995E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967294E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967294E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967294E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.294967294E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672940000005E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672940000005E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672940000005E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672940000005E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672939999995E9d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672939999995E9d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672939999995E9d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949672939999995E9d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446744073709552E19d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446744073709552E19d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446744073709552E19d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446744073709552E19d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446744073709556E19d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446744073709556E19d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446744073709556E19d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446744073709556E19d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.844674407370955E19d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.844674407370955E19d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.844674407370955E19d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.844674407370955E19d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.0d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.0d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.0d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.0d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.99999999999999d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.99999999999999d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.99999999999999d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.99999999999999d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.00000000000003d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.00000000000003d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.00000000000003d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.00000000000003d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.0d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.0d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.0d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.0d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-126.99999999999999d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-126.99999999999999d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-126.99999999999999d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-126.99999999999999d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.00000000000001d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.00000000000001d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.00000000000001d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.00000000000001d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.0d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.0d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.0d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.0d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.99999999999997d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.99999999999997d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.99999999999997d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.99999999999997d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.00000000000003d).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.00000000000003d).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.00000000000003d).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.00000000000003d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.0d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.0d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.0d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.0d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.00000000000001d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.00000000000001d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.00000000000001d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.00000000000001d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.99999999999999d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.99999999999999d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.99999999999999d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.99999999999999d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.0d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.0d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.0d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.0d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.00000000000003d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.00000000000003d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.00000000000003d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.00000000000003d).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.99999999999999d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.99999999999999d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.99999999999999d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.99999999999999d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.0d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.0d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.0d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.0d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.00000000000001d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.00000000000001d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.00000000000001d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.00000000000001d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(125.99999999999999d).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(125.99999999999999d).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(125.99999999999999d).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(125.99999999999999d).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.0f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.0f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.0f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.0f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.004f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.004f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.004f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.004f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.996f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.996f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.996f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.996f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.0f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.0f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.0f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.0f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.01f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.01f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.01f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65536.01f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.996f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.996f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.996f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65535.996f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.0f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.0f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.0f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.0f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.004f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.004f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.004f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65534.004f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65533.996f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65533.996f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65533.996f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(65533.996f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949673E9f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949673E9f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949673E9f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949673E9f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949678E9f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949678E9f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949678E9f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.2949678E9f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.29496704E9f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.29496704E9f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.29496704E9f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.29496704E9f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446744E19f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446744E19f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446744E19f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446744E19f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446746E19f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446746E19f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446746E19f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446746E19f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446743E19f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446743E19f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446743E19f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.8446743E19f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.0f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.0f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.0f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.0f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.99999f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.99999f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.99999f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.99999f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.00002f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.00002f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.00002f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.00002f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.0f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.0f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.0f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.0f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-126.99999f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-126.99999f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-126.99999f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-126.99999f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.00001f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.00001f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.00001f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-127.00001f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.0f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.0f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.0f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.0f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.99998f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.99998f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.99998f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-128.99998f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.00002f).AsUInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.00002f).AsUInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.00002f).AsUInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-129.00002f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.0f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.0f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.0f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.0f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.00001f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.00001f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.00001f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.00001f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.99999f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.99999f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.99999f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.99999f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.0f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.0f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.0f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.0f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.00002f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.00002f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.00002f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(128.00002f).AsSByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.99999f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.99999f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.99999f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(127.99999f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.0f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.0f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.0f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.0f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.00001f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.00001f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.00001f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(126.00001f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(125.99999f).AsUInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(125.99999f).AsUInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(125.99999f).AsUInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(125.99999f).AsSByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
    }
    
    /// <summary>
    /// </summary>
    /// <returns>
    /// </returns>
    /// <remarks/>
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
          if (j == ranges[i + 1]) break;
          j++;
        }
      }
    }
    /// <summary>
    /// </summary>
    /// <returns>
    /// </returns>
    /// <remarks/>
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
    /// <summary>
    /// </summary>
    /// <returns>
    /// </returns>
    /// <remarks/>
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
    /// <summary>
    /// </summary>
    /// <returns>
    /// </returns>
    /// <remarks/>
[Test]
    public void TestUShort() {
      for (int i = UInt16.MinValue; i <= UInt16.MaxValue; i++) {
        TestCommon.AssertSer(
          CBORObject.FromObject((UInt16)i),
          String.Format(CultureInfo.InvariantCulture, "{0}", i));
      }
    }
    /// <summary>
    /// </summary>
    /// <returns>
    /// </returns>
    /// <remarks/>
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
    /// <summary>
    /// </summary>
    /// <returns>
    /// </returns>
    /// <remarks/>
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