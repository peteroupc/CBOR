/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Globalization;
using System.Numerics;
using System.IO;
using PeterO;
using System.Text;

using NUnit.Framework;

namespace Test {

  [TestFixture]
  public class CBORTest {
    private void TestBigFloatDoubleCore(double d, string s) {
      double oldd = d;
      BigFloat bf = BigFloat.FromDouble(d);
      if (s != null) {
        Assert.AreEqual(s, bf.ToString());
      }
      d = bf.ToDouble();
      Assert.AreEqual((double)oldd, d);
      TestCommon.AssertRoundTrip(CBORObject.FromObject(bf));
      TestCommon.AssertRoundTrip(CBORObject.FromObject(d));
    }

    private void TestBigFloatSingleCore(float d, string s) {
      float oldd = d;
      BigFloat bf = BigFloat.FromSingle(d);
      if (s != null) {
        Assert.AreEqual(s, bf.ToString());
      }
      d = bf.ToSingle();
      Assert.AreEqual((float)oldd, d);
      TestCommon.AssertRoundTrip(CBORObject.FromObject(bf));
      TestCommon.AssertRoundTrip(CBORObject.FromObject(d));
    }

    private CBORObject RandomNumber(FastRandom rand) {
      switch (rand.NextValue(6)) {
        case 0:
          return CBORObject.FromObject(RandomDouble(rand, Int32.MaxValue));
        case 1:
          return CBORObject.FromObject(RandomSingle(rand, Int32.MaxValue));
        case 2:
          return CBORObject.FromObject(RandomBigInteger(rand));
        case 3:
          return CBORObject.FromObject(RandomBigFloat(rand));
        case 4:
          return CBORObject.FromObject(RandomDecimalFraction(rand));
        case 5:
          return CBORObject.FromObject(RandomInt64(rand));
        default:
          throw new ArgumentException();
      }
    }

    private long RandomInt64(FastRandom rand) {
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

    private double RandomDouble(FastRandom rand, int exponent) {
      if (exponent == Int32.MaxValue)
        exponent = rand.NextValue(2047);
      long r = rand.NextValue(0x10000);
      r |= ((long)rand.NextValue(0x10000)) << 16;
      if (rand.NextValue(2) == 0) {
        r |= ((long)rand.NextValue(0x10000)) << 32;
        if (rand.NextValue(2) == 0) {
          r |= ((long)rand.NextValue(0x10000)) << 48;
        }
      }
      r &= ~0x7FF0000000000000L; // clear exponent
      r |= ((long)exponent) << 52; // set exponent
      return ConverterInternal.Int64BitsToDouble(r);
    }

    private float RandomSingle(FastRandom rand, int exponent) {
      if (exponent == Int32.MaxValue)
        exponent = rand.NextValue(255);
      int r = rand.NextValue(0x10000);
      if (rand.NextValue(2) == 0) {
        r |= ((int)rand.NextValue(0x10000)) << 16;
      }
      r &= ~0x7F800000; // clear exponent
      r |= ((int)exponent) << 23; // set exponent
      return ConverterInternal.Int32BitsToSingle(r);
    }

    public static DecimalFraction RandomDecimalFraction(FastRandom r) {
      return DecimalFraction.FromString(RandomDecimalString(r));
    }

    public static BigInteger RandomBigInteger(FastRandom r) {
      return BigInteger.Parse(RandomBigIntString(r),CultureInfo.InvariantCulture);
    }

    public static BigFloat RandomBigFloat(FastRandom r) {
      return new BigFloat(RandomBigInteger(r),r.NextValue(400)-200);
    }

    public static String RandomBigIntString(FastRandom r) {
      int count = r.NextValue(50) + 1;
      StringBuilder sb = new StringBuilder();
      if (r.NextValue(2) == 0) sb.Append('-');
      for (int i = 0; i < count; i++) {
        if (i == 0)
          sb.Append((char)('1' + r.NextValue(9)));
        else
          sb.Append((char)('0' + r.NextValue(10)));
      }
      return sb.ToString();
    }

    public static String RandomDecimalString(FastRandom r) {
      int count = r.NextValue(20) + 1;
      StringBuilder sb = new StringBuilder();
      if (r.NextValue(2) == 0) sb.Append('-');
      for (int i = 0; i < count; i++) {
        if (i == 0)
          sb.Append((char)('1' + r.NextValue(9)));
        else
          sb.Append((char)('0' + r.NextValue(10)));
      }
      if (r.NextValue(2) == 0) {
        sb.Append('.');
        count = r.NextValue(20) + 1;
        for (int i = 0; i < count; i++) {
          sb.Append((char)('0' + r.NextValue(10)));
        }
      }
      if (r.NextValue(2) == 0) {
        sb.Append('E');
        count = r.NextValue(20);
        if (count != 0) {
          sb.Append(r.NextValue(2) == 0 ? '+' : '-');
        }
        sb.Append(Convert.ToString(
          (int)count, CultureInfo.InvariantCulture));
      }
      return sb.ToString();
    }

    private void TestDecimalString(String r) {
      CBORObject o = CBORObject.FromObject(DecimalFraction.FromString(r));
      CBORObject o2 = CBORDataUtilities.ParseJSONNumber(r);
      if (o.CompareTo(o2) != 0) {
        Assert.Fail("Expected: " + o + "\n was: " + o2);
      }
    }
    [Test]
    public void TestAdd() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 3000; i++) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        DecimalFraction cmpDecFrac = o1.AsDecimalFraction().Add(o2.AsDecimalFraction());
        DecimalFraction cmpCobj = CBORObject.Addition(o1,o2).AsDecimalFraction();
        if (cmpDecFrac.CompareTo(cmpCobj)!=0) {
          Assert.AreEqual(0,cmpDecFrac.CompareTo(cmpCobj),
                          "Results don't match:\n" + o1.ToString() + " and\n" + o2.ToString() + "\nOR\n" +
                          o1.AsDecimalFraction().ToString() + " and\n" + o2.AsDecimalFraction().ToString());
        }
        TestCommon.AssertRoundTrip(o1);
        TestCommon.AssertRoundTrip(o2);
      }
    }
    [Test]
    public void TestSubtract() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 3000; i++) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        DecimalFraction cmpDecFrac = o1.AsDecimalFraction().Subtract(o2.AsDecimalFraction());
        DecimalFraction cmpCobj = CBORObject.Subtract(o1,o2).AsDecimalFraction();
        if (cmpDecFrac.CompareTo(cmpCobj)!=0) {
          Assert.AreEqual(0,cmpDecFrac.CompareTo(cmpCobj),
                          "Results don't match:\n" + o1.ToString() + " and\n" + o2.ToString() + "\nOR\n" +
                          o1.AsDecimalFraction().ToString() + " and\n" + o2.AsDecimalFraction().ToString());
        }
        TestCommon.AssertRoundTrip(o1);
        TestCommon.AssertRoundTrip(o2);
      }
    }

    
    [Test]
    public void TestCompare() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 3000; i++) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        int cmpDecFrac = o1.AsDecimalFraction().CompareTo(o2.AsDecimalFraction());
        int cmpCobj = o1.CompareTo(o2);
        if (cmpDecFrac != cmpCobj) {
          Assert.AreEqual(cmpDecFrac, cmpCobj,
                          "Results don't match:\n" + o1.ToString() + " and\n" + o2.ToString()+"\nOR\n"+
                          o1.AsDecimalFraction().ToString() + " and\n" + o2.AsDecimalFraction().ToString());
        }
        TestCommon.AssertRoundTrip(o1);
        TestCommon.AssertRoundTrip(o2);
      }
    }

    [Test]
    public void TestParseDecimalStrings() {
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 2000; i++) {
        string r = RandomDecimalString(rand);
        TestDecimalString(r);
      }
    }

    [Test]
    public void TestRandomData() {
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 200; i++) {
        byte[] array = new byte[rand.NextValue(1000000) + 1];
        for (int j = 0; j < array.Length; j++) {
          if (j + 3 <= array.Length) {
            int r = rand.NextValue(0x1000000);
            array[j] = (byte)((r) & 0xFF);
            array[j + 1] = (byte)((r >> 8) & 0xFF);
            array[j + 2] = (byte)((r >> 16) & 0xFF);
            j += 2;
          } else {
            array[j] = (byte)rand.NextValue(256);
          }
        }
        using (MemoryStream ms = new MemoryStream(array)) {
          while (ms.Position != ms.Length) {
            try {
              CBORObject o = CBORObject.Read(ms);
              if (o == null) Assert.Fail("object read is null");
            } catch (CBORException) {
              // Expected exception
            }
          }
        }
      }
    }
    [Test]
    public void TestBigFloatSingle() {
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 255; i++) { // Try a random float with a given exponent
        TestBigFloatSingleCore(RandomSingle(rand, i), null);
        TestBigFloatSingleCore(RandomSingle(rand, i), null);
        TestBigFloatSingleCore(RandomSingle(rand, i), null);
        TestBigFloatSingleCore(RandomSingle(rand, i), null);
      }
    }

    [Test]
    public void TestBigFloatDouble() {
      TestBigFloatDoubleCore(3.5, "3.5");
      TestBigFloatDoubleCore(7, "7");
      TestBigFloatDoubleCore(1.75, "1.75");
      TestBigFloatDoubleCore(3.5, "3.5");
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 2047; i++) { // Try a random double with a given exponent
        TestBigFloatDoubleCore(RandomDouble(rand, i), null);
        TestBigFloatDoubleCore(RandomDouble(rand, i), null);
        TestBigFloatDoubleCore(RandomDouble(rand, i), null);
        TestBigFloatDoubleCore(RandomDouble(rand, i), null);
      }
    }


    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestTagThenBreak() {
      TestCommon.FromBytesTestAB(new byte[] { 0xD1, 0xFF });
    }

    [Test]
    public void TestJSONEscapedChars() {
      CBORObject o = CBORObject.FromJSONString(
        "[\"\\r\\n\\u0006\\\\\\\"\"]");
      Assert.AreEqual(1, o.Count);
      Assert.AreEqual("\r\n\u0006\\\"", o[0].AsString());
      Assert.AreEqual("[\"\\r\\n\\u0006\\\\\\\"\"]",
                      o.ToJSONString());
      TestCommon.AssertRoundTrip(o);
    }

    [Test]
    public void TestCBORFromArray() {
      CBORObject o = CBORObject.FromObject(new int[] { 1, 2, 3 });
      Assert.AreEqual(3, o.Count);
      Assert.AreEqual(1, o[0].AsInt32());
      Assert.AreEqual(2, o[1].AsInt32());
      Assert.AreEqual(3, o[2].AsInt32());
      TestCommon.AssertRoundTrip(o);
    }

    [Test]
    public void TestJSON() {
      CBORObject o;
      o = CBORObject.FromJSONString("[1,2,3]");
      try { CBORObject.FromJSONString("[\"\\d800\"]"); } catch (CBORException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromJSONString("[1,2,"); } catch (CBORException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromJSONString("[1,2,3"); } catch (CBORException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromJSONString("[\""); } catch (CBORException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      Assert.AreEqual(3, o.Count);
      Assert.AreEqual(1, o[0].AsInt32());
      Assert.AreEqual(2, o[1].AsInt32());
      Assert.AreEqual(3, o[2].AsInt32());
      o = CBORObject.FromJSONString("[1.5,2.6,3.7,4.0,222.22]");
      double actual = o[0].AsDouble();
      Assert.AreEqual((double)1.5, actual);
      Assert.AreEqual("true", CBORObject.True.ToJSONString());
      Assert.AreEqual("false", CBORObject.False.ToJSONString());
      Assert.AreEqual("null", CBORObject.Null.ToJSONString());
    }

    [Test]
    public void TestByte() {
      for (int i = 0; i <= 255; i++) {
        TestCommon.AssertSer(
          CBORObject.FromObject((byte)i),
          String.Format(CultureInfo.InvariantCulture, "{0}", i));
      }
    }

    public void DoTestReadUtf8(byte[] bytes,
                               int expectedRet, string expectedString,
                               int noReplaceRet, string noReplaceString
                              ) {
      DoTestReadUtf8(bytes, bytes.Length, expectedRet, expectedString,
                     noReplaceRet, noReplaceString);
    }

    public void DoTestReadUtf8(byte[] bytes, int length,
                               int expectedRet, string expectedString,
                               int noReplaceRet, string noReplaceString
                              ) {
      try {
        StringBuilder builder = new StringBuilder();
        int ret = 0;
        using (MemoryStream ms = new MemoryStream(bytes)) {
          ret = CBORDataUtilities.ReadUtf8(ms, length, builder, true);
          Assert.AreEqual(expectedRet, ret);
          if (expectedRet == 0) {
            Assert.AreEqual(expectedString, builder.ToString());
          }
          ms.Position = 0;
          builder.Clear();
          ret = CBORDataUtilities.ReadUtf8(ms, length, builder, false);
          Assert.AreEqual(noReplaceRet, ret);
          if (noReplaceRet == 0) {
            Assert.AreEqual(noReplaceString, builder.ToString());
          }
        }
        if (bytes.Length >= length) {
          builder.Clear();
          ret = CBORDataUtilities.ReadUtf8FromBytes(bytes, 0, length, builder, true);
          Assert.AreEqual(expectedRet, ret);
          if (expectedRet == 0) {
            Assert.AreEqual(expectedString, builder.ToString());
          }
          builder.Clear();
          ret = CBORDataUtilities.ReadUtf8FromBytes(bytes, 0, length, builder, false);
          Assert.AreEqual(noReplaceRet, ret);
          if (noReplaceRet == 0) {
            Assert.AreEqual(noReplaceString, builder.ToString());
          }
        }
      } catch (IOException ex) {
        throw new CBORException("", ex);
      }
    }

    [Test]
    public void TestDecFracOverflow() {
      try { CBORObject.FromObject(Single.PositiveInfinity).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Single.NegativeInfinity).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Single.NaN).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Double.PositiveInfinity).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Double.NegativeInfinity).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Double.NaN).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Single.PositiveInfinity).AsBigFloat(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Single.NegativeInfinity).AsBigFloat(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Single.NaN).AsBigFloat(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Double.PositiveInfinity).AsBigFloat(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Double.NegativeInfinity).AsBigFloat(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Double.NaN).AsBigFloat(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
    }

    [Test]
    public void TestFPToBigInteger() {
      Assert.AreEqual("0", CBORObject.FromObject((float)0.75).AsBigInteger().ToString());
      Assert.AreEqual("0", CBORObject.FromObject((float)0.99).AsBigInteger().ToString());
      Assert.AreEqual("0", CBORObject.FromObject((float)0.0000000000000001).AsBigInteger().ToString());
      Assert.AreEqual("0", CBORObject.FromObject((float)0.5).AsBigInteger().ToString());
      Assert.AreEqual("1", CBORObject.FromObject((float)1.5).AsBigInteger().ToString());
      Assert.AreEqual("2", CBORObject.FromObject((float)2.5).AsBigInteger().ToString());
      Assert.AreEqual("328323", CBORObject.FromObject((float)328323f).AsBigInteger().ToString());
      Assert.AreEqual("0", CBORObject.FromObject((double)0.75).AsBigInteger().ToString());
      Assert.AreEqual("0", CBORObject.FromObject((double)0.99).AsBigInteger().ToString());
      Assert.AreEqual("0", CBORObject.FromObject((double)0.0000000000000001).AsBigInteger().ToString());
      Assert.AreEqual("0", CBORObject.FromObject((double)0.5).AsBigInteger().ToString());
      Assert.AreEqual("1", CBORObject.FromObject((double)1.5).AsBigInteger().ToString());
      Assert.AreEqual("2", CBORObject.FromObject((double)2.5).AsBigInteger().ToString());
      Assert.AreEqual("328323", CBORObject.FromObject((double)328323).AsBigInteger().ToString());
      try { CBORObject.FromObject(Single.PositiveInfinity).AsBigInteger(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Single.NegativeInfinity).AsBigInteger(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Single.NaN).AsBigInteger(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Double.PositiveInfinity).AsBigInteger(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Double.NegativeInfinity).AsBigInteger(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(Double.NaN).AsBigInteger(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
    }

    [Test]
    public void TestDecFracFP() {
      Assert.AreEqual("0.75", DecimalFraction.FromDouble(0.75).ToString());
      Assert.AreEqual("0.5", DecimalFraction.FromDouble(0.5).ToString());
      Assert.AreEqual("0.25", DecimalFraction.FromDouble(0.25).ToString());
      Assert.AreEqual("0.875", DecimalFraction.FromDouble(0.875).ToString());
      Assert.AreEqual("0.125", DecimalFraction.FromDouble(0.125).ToString());
      Assert.AreEqual("0.75", DecimalFraction.FromSingle(0.75f).ToString());
      Assert.AreEqual("0.5", DecimalFraction.FromSingle(0.5f).ToString());
      Assert.AreEqual("0.25", DecimalFraction.FromSingle(0.25f).ToString());
      Assert.AreEqual("0.875", DecimalFraction.FromSingle(0.875f).ToString());
      Assert.AreEqual("0.125", DecimalFraction.FromSingle(0.125f).ToString());
    }


    [Test]
    public void ScaleTest() {
      Assert.AreEqual(-(BigInteger)7, DecimalFraction.FromString("1.265e-4").Exponent);
      Assert.AreEqual(-(BigInteger)4, DecimalFraction.FromString("0.000E-1").Exponent);
      Assert.AreEqual(-(BigInteger)16, DecimalFraction.FromString("0.57484848535648e-2").Exponent);
      Assert.AreEqual(-(BigInteger)22, DecimalFraction.FromString("0.485448e-16").Exponent);
      Assert.AreEqual(-(BigInteger)20, DecimalFraction.FromString("0.5657575351495151495649565150e+8").Exponent);
      Assert.AreEqual(-(BigInteger)10, DecimalFraction.FromString("0e-10").Exponent);
      Assert.AreEqual(-(BigInteger)17, DecimalFraction.FromString("0.504952e-11").Exponent);
      Assert.AreEqual(-(BigInteger)13, DecimalFraction.FromString("0e-13").Exponent);
      Assert.AreEqual(-(BigInteger)43, DecimalFraction.FromString("0.49495052535648555757515648e-17").Exponent);
      Assert.AreEqual((BigInteger)7, DecimalFraction.FromString("0.485654575150e+19").Exponent);
      Assert.AreEqual(-(BigInteger)0, DecimalFraction.FromString("0.48515648e+8").Exponent);
      Assert.AreEqual(-(BigInteger)45, DecimalFraction.FromString("0.49485251485649535552535451544956e-13").Exponent);
      Assert.AreEqual(-(BigInteger)6, DecimalFraction.FromString("0.565754515152575448505257e+18").Exponent);
      Assert.AreEqual((BigInteger)16, DecimalFraction.FromString("0e+16").Exponent);
      Assert.AreEqual((BigInteger)6, DecimalFraction.FromString("0.5650e+10").Exponent);
      Assert.AreEqual(-(BigInteger)5, DecimalFraction.FromString("0.49555554575756575556e+15").Exponent);
      Assert.AreEqual(-(BigInteger)37, DecimalFraction.FromString("0.57494855545057534955e-17").Exponent);
      Assert.AreEqual(-(BigInteger)25, DecimalFraction.FromString("0.4956504855525748575456e-3").Exponent);
      Assert.AreEqual(-(BigInteger)26, DecimalFraction.FromString("0.55575355495654484948525354545053494854e+12").Exponent);
      Assert.AreEqual(-(BigInteger)22, DecimalFraction.FromString("0.484853575350494950575749545057e+8").Exponent);
      Assert.AreEqual((BigInteger)11, DecimalFraction.FromString("0.52545451e+19").Exponent);
      Assert.AreEqual(-(BigInteger)29, DecimalFraction.FromString("0.48485654495751485754e-9").Exponent);
      Assert.AreEqual(-(BigInteger)38, DecimalFraction.FromString("0.56525456555549545257535556495655574848e+0").Exponent);
      Assert.AreEqual(-(BigInteger)15, DecimalFraction.FromString("0.485456485657545752495450554857e+15").Exponent);
      Assert.AreEqual(-(BigInteger)37, DecimalFraction.FromString("0.485448525554495048e-19").Exponent);
      Assert.AreEqual(-(BigInteger)29, DecimalFraction.FromString("0.494952485550514953565655e-5").Exponent);
      Assert.AreEqual(-(BigInteger)8, DecimalFraction.FromString("0.50495454554854505051534950e+18").Exponent);
      Assert.AreEqual(-(BigInteger)37, DecimalFraction.FromString("0.5156524853575655535351554949525449e-3").Exponent);
      Assert.AreEqual((BigInteger)3, DecimalFraction.FromString("0e+3").Exponent);
      Assert.AreEqual(-(BigInteger)8, DecimalFraction.FromString("0.51505056554957575255555250e+18").Exponent);
      Assert.AreEqual(-(BigInteger)14, DecimalFraction.FromString("0.5456e-10").Exponent);
      Assert.AreEqual(-(BigInteger)36, DecimalFraction.FromString("0.494850515656505252555154e-12").Exponent);
      Assert.AreEqual(-(BigInteger)42, DecimalFraction.FromString("0.535155525253485757525253555749575749e-6").Exponent);
      Assert.AreEqual(-(BigInteger)29, DecimalFraction.FromString("0.56554952554850525552515549564948e+3").Exponent);
      Assert.AreEqual(-(BigInteger)40, DecimalFraction.FromString("0.494855545257545656515554495057e-10").Exponent);
      Assert.AreEqual(-(BigInteger)18, DecimalFraction.FromString("0.5656504948515252555456e+4").Exponent);
      Assert.AreEqual(-(BigInteger)17, DecimalFraction.FromString("0e-17").Exponent);
      Assert.AreEqual(-(BigInteger)32, DecimalFraction.FromString("0.55535551515249535049495256e-6").Exponent);
      Assert.AreEqual(-(BigInteger)31, DecimalFraction.FromString("0.4948534853564853565654514855e-3").Exponent);
      Assert.AreEqual(-(BigInteger)38, DecimalFraction.FromString("0.5048485057535249555455e-16").Exponent);
      Assert.AreEqual(-(BigInteger)16, DecimalFraction.FromString("0e-16").Exponent);
      Assert.AreEqual((BigInteger)5, DecimalFraction.FromString("0.5354e+9").Exponent);
      Assert.AreEqual((BigInteger)1, DecimalFraction.FromString("0.54e+3").Exponent);
      Assert.AreEqual(-(BigInteger)38, DecimalFraction.FromString("0.4849525755545751574853494948e-10").Exponent);
      Assert.AreEqual(-(BigInteger)33, DecimalFraction.FromString("0.52514853565252565251565548e-7").Exponent);
      Assert.AreEqual(-(BigInteger)13, DecimalFraction.FromString("0.575151545652e-1").Exponent);
      Assert.AreEqual(-(BigInteger)22, DecimalFraction.FromString("0.49515354514852e-8").Exponent);
      Assert.AreEqual(-(BigInteger)24, DecimalFraction.FromString("0.54535357515356545554e-4").Exponent);
      Assert.AreEqual(-(BigInteger)11, DecimalFraction.FromString("0.574848e-5").Exponent);
      Assert.AreEqual(-(BigInteger)3, DecimalFraction.FromString("0.565055e+3").Exponent);
    }


    [Test]
    public void TestReadUtf8() {
      DoTestReadUtf8(new byte[] { 0x20, 0x20, 0x20 },
                     0, "   ", 0, "   ");
      DoTestReadUtf8(new byte[] { 0x20, 0xc2, 0x80 },
                     0, " \u0080", 0, " \u0080");
      DoTestReadUtf8(new byte[] { 0x20, 0xc2, 0x80, 0x20 },
                     0, " \u0080 ", 0, " \u0080 ");
      DoTestReadUtf8(new byte[] { 0x20, 0xc2, 0x80, 0xc2 },
                     0, " \u0080\ufffd", -1, null);
      DoTestReadUtf8(new byte[] { 0x20, 0xc2, 0x20, 0x20 },
                     0, " \ufffd  ", -1, null);
      DoTestReadUtf8(new byte[] { 0x20, 0xc2, 0xff, 0x20 },
                     0, " \ufffd\ufffd ", -1, null);
      DoTestReadUtf8(new byte[] { 0x20, 0xe0, 0xa0, 0x80 },
                     0, " \u0800", 0, " \u0800");
      DoTestReadUtf8(new byte[] { 0x20, 0xe0, 0xa0, 0x80, 0x20 },
                     0, " \u0800 ", 0, " \u0800 ");
      DoTestReadUtf8(new byte[] { 0x20, 0xf0, 0x90, 0x80, 0x80 },
                     0, " \ud800\udc00", 0, " \ud800\udc00");
      DoTestReadUtf8(new byte[] { 0x20, 0xf0, 0x90, 0x80, 0x80 }, 3,
                     0, " \ufffd", -1, null);
      DoTestReadUtf8(new byte[] { 0x20, 0xf0, 0x90 }, 5,
                     -2, null, -1, null);
      DoTestReadUtf8(new byte[] { 0x20, 0x20, 0x20 }, 5,
                     -2, null, -2, null);
      DoTestReadUtf8(new byte[] { 0x20, 0xf0, 0x90, 0x80, 0x80, 0x20 },
                     0, " \ud800\udc00 ", 0, " \ud800\udc00 ");
      DoTestReadUtf8(new byte[] { 0x20, 0xf0, 0x90, 0x80, 0x20 },
                     0, " \ufffd ", -1, null);
      DoTestReadUtf8(new byte[] { 0x20, 0xf0, 0x90, 0x20 },
                     0, " \ufffd ", -1, null);
      DoTestReadUtf8(new byte[] { 0x20, 0xf0, 0x90, 0x80, 0xff },
                     0, " \ufffd\ufffd", -1, null);
      DoTestReadUtf8(new byte[] { 0x20, 0xf0, 0x90, 0xff },
                     0, " \ufffd\ufffd", -1, null);
      DoTestReadUtf8(new byte[] { 0x20, 0xe0, 0xa0, 0x20 },
                     0, " \ufffd ", -1, null);
      DoTestReadUtf8(new byte[] { 0x20, 0xe0, 0x20 },
                     0, " \ufffd ", -1, null);
      DoTestReadUtf8(new byte[] { 0x20, 0xe0, 0xa0, 0xff },
                     0, " \ufffd\ufffd", -1, null);
      DoTestReadUtf8(new byte[] { 0x20, 0xe0, 0xff },
                     0, " \ufffd\ufffd", -1, null);
    }

    private bool ByteArrayEquals(byte[] arrayA, byte[] arrayB) {
      if (arrayA == null) return (arrayB == null);
      if (arrayB == null) return false;
      if (arrayA.Length != arrayB.Length) return false;
      for (int i = 0; i < arrayA.Length; i++) {
        if (arrayA[i] != arrayB[i]) return false;
      }
      return true;
    }


    [Test]
    public void TestArray() {
      CBORObject cbor = CBORObject.FromJSONString("[]");
      cbor.Add(CBORObject.FromObject(3));
      cbor.Add(CBORObject.FromObject(4));
      byte[] bytes = cbor.EncodeToBytes();
      bool isequal = ByteArrayEquals(new byte[] { (byte)(0x80 | 2), 3, 4 }, bytes);
      Assert.IsTrue(isequal,"array not equal");
    }
    [Test]
    public void TestMap() {
      CBORObject cbor = CBORObject.FromJSONString("{\"a\":2,\"b\":4}");
      Assert.AreEqual(2, cbor.Count);
      TestCommon.AssertEqualsHashCode(
        CBORObject.FromObject(2),
        cbor[CBORObject.FromObject("a")]);
      TestCommon.AssertEqualsHashCode(
        CBORObject.FromObject(4),
        cbor[CBORObject.FromObject("b")]);
      Assert.AreEqual(2, cbor[CBORObject.FromObject("a")].AsInt32());
      Assert.AreEqual(4, cbor[CBORObject.FromObject("b")].AsInt32());
    }


    private static String Repeat(char c, int num) {
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      for (int i = 0; i < num; i++) {
        sb.Append(c);
      }
      return sb.ToString();
    }

    private static String Repeat(String c, int num) {
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      for (int i = 0; i < num; i++) {
        sb.Append(c);
      }
      return sb.ToString();
    }

    [Test]
    public void TestTextStringStream() {
      CBORObject cbor = TestCommon.FromBytesTestAB(
        new byte[] { 0x7F, 0x61, 0x20, 0x61, 0x20, 0xFF });
      Assert.AreEqual("  ", cbor.AsString());
      // Test streaming of long strings
      string longString = Repeat('x', 200000);
      CBORObject cbor2;
      cbor = CBORObject.FromObject(longString);
      cbor2 = TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
      TestCommon.AssertEqualsHashCode(cbor, cbor2);
      Assert.AreEqual(longString, cbor2.AsString());
      longString = Repeat('\u00e0', 200000);
      cbor = CBORObject.FromObject(longString);
      cbor2 = TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
      TestCommon.AssertEqualsHashCode(cbor, cbor2);
      Assert.AreEqual(longString, cbor2.AsString());
      longString = Repeat('\u3000', 200000);
      cbor = CBORObject.FromObject(longString);
      cbor2 = TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
      TestCommon.AssertEqualsHashCode(cbor, cbor2);
      Assert.AreEqual(longString, cbor2.AsString());
      longString = Repeat("\ud800\udc00", 200000);
      cbor = CBORObject.FromObject(longString);
      cbor2 = TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
      TestCommon.AssertEqualsHashCode(cbor, cbor2);
      Assert.AreEqual(longString, cbor2.AsString());
    }

    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestTextStringStreamNoTagsBeforeDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0x7F, 0x61, 0x20, 0xC0, 0x61, 0x20, 0xFF });
    }

    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestTextStringStreamNoIndefiniteWithinDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0x7F, 0x61, 0x20, 0x7F, 0x61, 0x20, 0xFF, 0xFF });
    }
    [Test]
    public void TestByteStringStream() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0x5F, 0x41, 0x20, 0x41, 0x20, 0xFF });
    }
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestByteStringStreamNoTagsBeforeDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0x5F, 0x41, 0x20, 0xC2, 0x41, 0x20, 0xFF });
    }

    public static void AssertDecimalsEquivalent(string a, string b) {
      CBORObject ca = CBORDataUtilities.ParseJSONNumber(a);
      CBORObject cb = CBORDataUtilities.ParseJSONNumber(b);
      if (ca.CompareTo(cb) != 0)
        Assert.Fail(a + " not equal to " + b);
      TestCommon.AssertRoundTrip(ca);
      TestCommon.AssertRoundTrip(cb);
    }


    [Test]
    public void ZeroStringTests2() {
      Assert.AreEqual("0.0001265", DecimalFraction.FromString("1.265e-4").ToString());
      Assert.AreEqual("0.0001265", DecimalFraction.FromString("1.265e-4").ToEngineeringString());
      Assert.AreEqual("0.0001265", DecimalFraction.FromString("1.265e-4").ToPlainString());
      Assert.AreEqual("0.0000", DecimalFraction.FromString("0.000E-1").ToString());
      Assert.AreEqual("0.0000", DecimalFraction.FromString("0.000E-1").ToEngineeringString());
      Assert.AreEqual("0.0000", DecimalFraction.FromString("0.000E-1").ToPlainString());
      Assert.AreEqual("0E-16", DecimalFraction.FromString("0.0000000000000e-3").ToString());
      Assert.AreEqual("0.0E-15", DecimalFraction.FromString("0.0000000000000e-3").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000", DecimalFraction.FromString("0.0000000000000e-3").ToPlainString());
      Assert.AreEqual("0E-8", DecimalFraction.FromString("0.000000000e+1").ToString());
      Assert.AreEqual("0.00E-6", DecimalFraction.FromString("0.000000000e+1").ToEngineeringString());
      Assert.AreEqual("0.00000000", DecimalFraction.FromString("0.000000000e+1").ToPlainString());
      Assert.AreEqual("0.000", DecimalFraction.FromString("0.000000000000000e+12").ToString());
      Assert.AreEqual("0.000", DecimalFraction.FromString("0.000000000000000e+12").ToEngineeringString());
      Assert.AreEqual("0.000", DecimalFraction.FromString("0.000000000000000e+12").ToPlainString());
      Assert.AreEqual("0E-25", DecimalFraction.FromString("0.00000000000000e-11").ToString());
      Assert.AreEqual("0.0E-24", DecimalFraction.FromString("0.00000000000000e-11").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000", DecimalFraction.FromString("0.00000000000000e-11").ToPlainString());
      Assert.AreEqual("0E-7", DecimalFraction.FromString("0.000000000000e+5").ToString());
      Assert.AreEqual("0.0E-6", DecimalFraction.FromString("0.000000000000e+5").ToEngineeringString());
      Assert.AreEqual("0.0000000", DecimalFraction.FromString("0.000000000000e+5").ToPlainString());
      Assert.AreEqual("0E-8", DecimalFraction.FromString("0.0000e-4").ToString());
      Assert.AreEqual("0.00E-6", DecimalFraction.FromString("0.0000e-4").ToEngineeringString());
      Assert.AreEqual("0.00000000", DecimalFraction.FromString("0.0000e-4").ToPlainString());
      Assert.AreEqual("0.0000", DecimalFraction.FromString("0.000000e+2").ToString());
      Assert.AreEqual("0.0000", DecimalFraction.FromString("0.000000e+2").ToEngineeringString());
      Assert.AreEqual("0.0000", DecimalFraction.FromString("0.000000e+2").ToPlainString());
      Assert.AreEqual("0E+2", DecimalFraction.FromString("0.0e+3").ToString());
      Assert.AreEqual("0.0E+3", DecimalFraction.FromString("0.0e+3").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.0e+3").ToPlainString());
      Assert.AreEqual("0E-7", DecimalFraction.FromString("0.000000000000000e+8").ToString());
      Assert.AreEqual("0.0E-6", DecimalFraction.FromString("0.000000000000000e+8").ToEngineeringString());
      Assert.AreEqual("0.0000000", DecimalFraction.FromString("0.000000000000000e+8").ToPlainString());
      Assert.AreEqual("0E+7", DecimalFraction.FromString("0.000e+10").ToString());
      Assert.AreEqual("0.00E+9", DecimalFraction.FromString("0.000e+10").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.000e+10").ToPlainString());
      Assert.AreEqual("0E-31", DecimalFraction.FromString("0.0000000000000000000e-12").ToString());
      Assert.AreEqual("0.0E-30", DecimalFraction.FromString("0.0000000000000000000e-12").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000000000", DecimalFraction.FromString("0.0000000000000000000e-12").ToPlainString());
      Assert.AreEqual("0.00000", DecimalFraction.FromString("0.0000e-1").ToString());
      Assert.AreEqual("0.00000", DecimalFraction.FromString("0.0000e-1").ToEngineeringString());
      Assert.AreEqual("0.00000", DecimalFraction.FromString("0.0000e-1").ToPlainString());
      Assert.AreEqual("0E-22", DecimalFraction.FromString("0.00000000000e-11").ToString());
      Assert.AreEqual("0.0E-21", DecimalFraction.FromString("0.00000000000e-11").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000", DecimalFraction.FromString("0.00000000000e-11").ToPlainString());
      Assert.AreEqual("0E-28", DecimalFraction.FromString("0.00000000000e-17").ToString());
      Assert.AreEqual("0.0E-27", DecimalFraction.FromString("0.00000000000e-17").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000000", DecimalFraction.FromString("0.00000000000e-17").ToPlainString());
      Assert.AreEqual("0.00000", DecimalFraction.FromString("0.00000000000000e+9").ToString());
      Assert.AreEqual("0.00000", DecimalFraction.FromString("0.00000000000000e+9").ToEngineeringString());
      Assert.AreEqual("0.00000", DecimalFraction.FromString("0.00000000000000e+9").ToPlainString());
      Assert.AreEqual("0E-28", DecimalFraction.FromString("0.0000000000e-18").ToString());
      Assert.AreEqual("0.0E-27", DecimalFraction.FromString("0.0000000000e-18").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000000", DecimalFraction.FromString("0.0000000000e-18").ToPlainString());
      Assert.AreEqual("0E-14", DecimalFraction.FromString("0.0e-13").ToString());
      Assert.AreEqual("0.00E-12", DecimalFraction.FromString("0.0e-13").ToEngineeringString());
      Assert.AreEqual("0.00000000000000", DecimalFraction.FromString("0.0e-13").ToPlainString());
      Assert.AreEqual("0E-8", DecimalFraction.FromString("0.000000000000000000e+10").ToString());
      Assert.AreEqual("0.00E-6", DecimalFraction.FromString("0.000000000000000000e+10").ToEngineeringString());
      Assert.AreEqual("0.00000000", DecimalFraction.FromString("0.000000000000000000e+10").ToPlainString());
      Assert.AreEqual("0E+15", DecimalFraction.FromString("0.0000e+19").ToString());
      Assert.AreEqual("0E+15", DecimalFraction.FromString("0.0000e+19").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.0000e+19").ToPlainString());
      Assert.AreEqual("0E-13", DecimalFraction.FromString("0.00000e-8").ToString());
      Assert.AreEqual("0.0E-12", DecimalFraction.FromString("0.00000e-8").ToEngineeringString());
      Assert.AreEqual("0.0000000000000", DecimalFraction.FromString("0.00000e-8").ToPlainString());
      Assert.AreEqual("0E+3", DecimalFraction.FromString("0.00000000000e+14").ToString());
      Assert.AreEqual("0E+3", DecimalFraction.FromString("0.00000000000e+14").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.00000000000e+14").ToPlainString());
      Assert.AreEqual("0E-17", DecimalFraction.FromString("0.000e-14").ToString());
      Assert.AreEqual("0.00E-15", DecimalFraction.FromString("0.000e-14").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000", DecimalFraction.FromString("0.000e-14").ToPlainString());
      Assert.AreEqual("0E-25", DecimalFraction.FromString("0.000000e-19").ToString());
      Assert.AreEqual("0.0E-24", DecimalFraction.FromString("0.000000e-19").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000", DecimalFraction.FromString("0.000000e-19").ToPlainString());
      Assert.AreEqual("0E+7", DecimalFraction.FromString("0.000000000000e+19").ToString());
      Assert.AreEqual("0.00E+9", DecimalFraction.FromString("0.000000000000e+19").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.000000000000e+19").ToPlainString());
      Assert.AreEqual("0E+5", DecimalFraction.FromString("0.0000000000000e+18").ToString());
      Assert.AreEqual("0.0E+6", DecimalFraction.FromString("0.0000000000000e+18").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.0000000000000e+18").ToPlainString());
      Assert.AreEqual("0E-16", DecimalFraction.FromString("0.00000000000000e-2").ToString());
      Assert.AreEqual("0.0E-15", DecimalFraction.FromString("0.00000000000000e-2").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000", DecimalFraction.FromString("0.00000000000000e-2").ToPlainString());
      Assert.AreEqual("0E-31", DecimalFraction.FromString("0.0000000000000e-18").ToString());
      Assert.AreEqual("0.0E-30", DecimalFraction.FromString("0.0000000000000e-18").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000000000", DecimalFraction.FromString("0.0000000000000e-18").ToPlainString());
      Assert.AreEqual("0E-17", DecimalFraction.FromString("0e-17").ToString());
      Assert.AreEqual("0.00E-15", DecimalFraction.FromString("0e-17").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000", DecimalFraction.FromString("0e-17").ToPlainString());
      Assert.AreEqual("0E+17", DecimalFraction.FromString("0e+17").ToString());
      Assert.AreEqual("0.0E+18", DecimalFraction.FromString("0e+17").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0e+17").ToPlainString());
      Assert.AreEqual("0E-17", DecimalFraction.FromString("0.00000000000000000e+0").ToString());
      Assert.AreEqual("0.00E-15", DecimalFraction.FromString("0.00000000000000000e+0").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000", DecimalFraction.FromString("0.00000000000000000e+0").ToPlainString());
      Assert.AreEqual("0E-13", DecimalFraction.FromString("0.0000000000000e+0").ToString());
      Assert.AreEqual("0.0E-12", DecimalFraction.FromString("0.0000000000000e+0").ToEngineeringString());
      Assert.AreEqual("0.0000000000000", DecimalFraction.FromString("0.0000000000000e+0").ToPlainString());
      Assert.AreEqual("0E-31", DecimalFraction.FromString("0.0000000000000000000e-12").ToString());
      Assert.AreEqual("0.0E-30", DecimalFraction.FromString("0.0000000000000000000e-12").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000000000", DecimalFraction.FromString("0.0000000000000000000e-12").ToPlainString());
      Assert.AreEqual("0E-9", DecimalFraction.FromString("0.0000000000000000000e+10").ToString());
      Assert.AreEqual("0E-9", DecimalFraction.FromString("0.0000000000000000000e+10").ToEngineeringString());
      Assert.AreEqual("0.000000000", DecimalFraction.FromString("0.0000000000000000000e+10").ToPlainString());
      Assert.AreEqual("0E-7", DecimalFraction.FromString("0.00000e-2").ToString());
      Assert.AreEqual("0.0E-6", DecimalFraction.FromString("0.00000e-2").ToEngineeringString());
      Assert.AreEqual("0.0000000", DecimalFraction.FromString("0.00000e-2").ToPlainString());
      Assert.AreEqual("0E+9", DecimalFraction.FromString("0.000000e+15").ToString());
      Assert.AreEqual("0E+9", DecimalFraction.FromString("0.000000e+15").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.000000e+15").ToPlainString());
      Assert.AreEqual("0E-19", DecimalFraction.FromString("0.000000000e-10").ToString());
      Assert.AreEqual("0.0E-18", DecimalFraction.FromString("0.000000000e-10").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000", DecimalFraction.FromString("0.000000000e-10").ToPlainString());
      Assert.AreEqual("0E-8", DecimalFraction.FromString("0.00000000000000e+6").ToString());
      Assert.AreEqual("0.00E-6", DecimalFraction.FromString("0.00000000000000e+6").ToEngineeringString());
      Assert.AreEqual("0.00000000", DecimalFraction.FromString("0.00000000000000e+6").ToPlainString());
      Assert.AreEqual("0E+12", DecimalFraction.FromString("0.00000e+17").ToString());
      Assert.AreEqual("0E+12", DecimalFraction.FromString("0.00000e+17").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.00000e+17").ToPlainString());
      Assert.AreEqual("0E-18", DecimalFraction.FromString("0.000000000000000000e-0").ToString());
      Assert.AreEqual("0E-18", DecimalFraction.FromString("0.000000000000000000e-0").ToEngineeringString());
      Assert.AreEqual("0.000000000000000000", DecimalFraction.FromString("0.000000000000000000e-0").ToPlainString());
      Assert.AreEqual("0.00000", DecimalFraction.FromString("0.0000000000000000e+11").ToString());
      Assert.AreEqual("0.00000", DecimalFraction.FromString("0.0000000000000000e+11").ToEngineeringString());
      Assert.AreEqual("0.00000", DecimalFraction.FromString("0.0000000000000000e+11").ToPlainString());
      Assert.AreEqual("0E+3", DecimalFraction.FromString("0.000000000000e+15").ToString());
      Assert.AreEqual("0E+3", DecimalFraction.FromString("0.000000000000e+15").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.000000000000e+15").ToPlainString());
      Assert.AreEqual("0E-27", DecimalFraction.FromString("0.00000000e-19").ToString());
      Assert.AreEqual("0E-27", DecimalFraction.FromString("0.00000000e-19").ToEngineeringString());
      Assert.AreEqual("0.000000000000000000000000000", DecimalFraction.FromString("0.00000000e-19").ToPlainString());
      Assert.AreEqual("0E-11", DecimalFraction.FromString("0.00000e-6").ToString());
      Assert.AreEqual("0.00E-9", DecimalFraction.FromString("0.00000e-6").ToEngineeringString());
      Assert.AreEqual("0.00000000000", DecimalFraction.FromString("0.00000e-6").ToPlainString());
      Assert.AreEqual("0E-14", DecimalFraction.FromString("0e-14").ToString());
      Assert.AreEqual("0.00E-12", DecimalFraction.FromString("0e-14").ToEngineeringString());
      Assert.AreEqual("0.00000000000000", DecimalFraction.FromString("0e-14").ToPlainString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.000000000e+9").ToString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.000000000e+9").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.000000000e+9").ToPlainString());
      Assert.AreEqual("0E+8", DecimalFraction.FromString("0.00000e+13").ToString());
      Assert.AreEqual("0.0E+9", DecimalFraction.FromString("0.00000e+13").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.00000e+13").ToPlainString());
      Assert.AreEqual("0.000", DecimalFraction.FromString("0.000e-0").ToString());
      Assert.AreEqual("0.000", DecimalFraction.FromString("0.000e-0").ToEngineeringString());
      Assert.AreEqual("0.000", DecimalFraction.FromString("0.000e-0").ToPlainString());
      Assert.AreEqual("0E-9", DecimalFraction.FromString("0.000000000000000e+6").ToString());
      Assert.AreEqual("0E-9", DecimalFraction.FromString("0.000000000000000e+6").ToEngineeringString());
      Assert.AreEqual("0.000000000", DecimalFraction.FromString("0.000000000000000e+6").ToPlainString());
      Assert.AreEqual("0E+8", DecimalFraction.FromString("0.000000000e+17").ToString());
      Assert.AreEqual("0.0E+9", DecimalFraction.FromString("0.000000000e+17").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.000000000e+17").ToPlainString());
      Assert.AreEqual("0.00000", DecimalFraction.FromString("0.00000000000e+6").ToString());
      Assert.AreEqual("0.00000", DecimalFraction.FromString("0.00000000000e+6").ToEngineeringString());
      Assert.AreEqual("0.00000", DecimalFraction.FromString("0.00000000000e+6").ToPlainString());
      Assert.AreEqual("0E-11", DecimalFraction.FromString("0.00000000000000e+3").ToString());
      Assert.AreEqual("0.00E-9", DecimalFraction.FromString("0.00000000000000e+3").ToEngineeringString());
      Assert.AreEqual("0.00000000000", DecimalFraction.FromString("0.00000000000000e+3").ToPlainString());
      Assert.AreEqual("0", DecimalFraction.FromString("0e+0").ToString());
      Assert.AreEqual("0", DecimalFraction.FromString("0e+0").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0e+0").ToPlainString());
      Assert.AreEqual("0E+9", DecimalFraction.FromString("0.000e+12").ToString());
      Assert.AreEqual("0E+9", DecimalFraction.FromString("0.000e+12").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.000e+12").ToPlainString());
      Assert.AreEqual("0.00", DecimalFraction.FromString("0.00000000000e+9").ToString());
      Assert.AreEqual("0.00", DecimalFraction.FromString("0.00000000000e+9").ToEngineeringString());
      Assert.AreEqual("0.00", DecimalFraction.FromString("0.00000000000e+9").ToPlainString());
      Assert.AreEqual("0E-23", DecimalFraction.FromString("0.00000000000000e-9").ToString());
      Assert.AreEqual("0.00E-21", DecimalFraction.FromString("0.00000000000000e-9").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000000", DecimalFraction.FromString("0.00000000000000e-9").ToPlainString());
      Assert.AreEqual("0.0", DecimalFraction.FromString("0e-1").ToString());
      Assert.AreEqual("0.0", DecimalFraction.FromString("0e-1").ToEngineeringString());
      Assert.AreEqual("0.0", DecimalFraction.FromString("0e-1").ToPlainString());
      Assert.AreEqual("0E-17", DecimalFraction.FromString("0.0000e-13").ToString());
      Assert.AreEqual("0.00E-15", DecimalFraction.FromString("0.0000e-13").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000", DecimalFraction.FromString("0.0000e-13").ToPlainString());
      Assert.AreEqual("0E-18", DecimalFraction.FromString("0.00000000000e-7").ToString());
      Assert.AreEqual("0E-18", DecimalFraction.FromString("0.00000000000e-7").ToEngineeringString());
      Assert.AreEqual("0.000000000000000000", DecimalFraction.FromString("0.00000000000e-7").ToPlainString());
      Assert.AreEqual("0E-10", DecimalFraction.FromString("0.00000000000000e+4").ToString());
      Assert.AreEqual("0.0E-9", DecimalFraction.FromString("0.00000000000000e+4").ToEngineeringString());
      Assert.AreEqual("0.0000000000", DecimalFraction.FromString("0.00000000000000e+4").ToPlainString());
      Assert.AreEqual("0E-16", DecimalFraction.FromString("0.00000000e-8").ToString());
      Assert.AreEqual("0.0E-15", DecimalFraction.FromString("0.00000000e-8").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000", DecimalFraction.FromString("0.00000000e-8").ToPlainString());
      Assert.AreEqual("0E-8", DecimalFraction.FromString("0.00e-6").ToString());
      Assert.AreEqual("0.00E-6", DecimalFraction.FromString("0.00e-6").ToEngineeringString());
      Assert.AreEqual("0.00000000", DecimalFraction.FromString("0.00e-6").ToPlainString());
      Assert.AreEqual("0.00", DecimalFraction.FromString("0.0e-1").ToString());
      Assert.AreEqual("0.00", DecimalFraction.FromString("0.0e-1").ToEngineeringString());
      Assert.AreEqual("0.00", DecimalFraction.FromString("0.0e-1").ToPlainString());
      Assert.AreEqual("0E-26", DecimalFraction.FromString("0.0000000000000000e-10").ToString());
      Assert.AreEqual("0.00E-24", DecimalFraction.FromString("0.0000000000000000e-10").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000000000", DecimalFraction.FromString("0.0000000000000000e-10").ToPlainString());
      Assert.AreEqual("0E+12", DecimalFraction.FromString("0.00e+14").ToString());
      Assert.AreEqual("0E+12", DecimalFraction.FromString("0.00e+14").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.00e+14").ToPlainString());
      Assert.AreEqual("0E-13", DecimalFraction.FromString("0.000000000000000000e+5").ToString());
      Assert.AreEqual("0.0E-12", DecimalFraction.FromString("0.000000000000000000e+5").ToEngineeringString());
      Assert.AreEqual("0.0000000000000", DecimalFraction.FromString("0.000000000000000000e+5").ToPlainString());
      Assert.AreEqual("0E+6", DecimalFraction.FromString("0.0e+7").ToString());
      Assert.AreEqual("0E+6", DecimalFraction.FromString("0.0e+7").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.0e+7").ToPlainString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.00000000e+8").ToString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.00000000e+8").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.00000000e+8").ToPlainString());
      Assert.AreEqual("0E-9", DecimalFraction.FromString("0.000000000e+0").ToString());
      Assert.AreEqual("0E-9", DecimalFraction.FromString("0.000000000e+0").ToEngineeringString());
      Assert.AreEqual("0.000000000", DecimalFraction.FromString("0.000000000e+0").ToPlainString());
      Assert.AreEqual("0E+10", DecimalFraction.FromString("0.000e+13").ToString());
      Assert.AreEqual("0.00E+12", DecimalFraction.FromString("0.000e+13").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.000e+13").ToPlainString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.0000000000000000e+16").ToString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.0000000000000000e+16").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.0000000000000000e+16").ToPlainString());
      Assert.AreEqual("0E-9", DecimalFraction.FromString("0.00000000e-1").ToString());
      Assert.AreEqual("0E-9", DecimalFraction.FromString("0.00000000e-1").ToEngineeringString());
      Assert.AreEqual("0.000000000", DecimalFraction.FromString("0.00000000e-1").ToPlainString());
      Assert.AreEqual("0E-26", DecimalFraction.FromString("0.00000000000e-15").ToString());
      Assert.AreEqual("0.00E-24", DecimalFraction.FromString("0.00000000000e-15").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000000000", DecimalFraction.FromString("0.00000000000e-15").ToPlainString());
      Assert.AreEqual("0E+10", DecimalFraction.FromString("0.0e+11").ToString());
      Assert.AreEqual("0.00E+12", DecimalFraction.FromString("0.0e+11").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.0e+11").ToPlainString());
      Assert.AreEqual("0E+2", DecimalFraction.FromString("0.00000e+7").ToString());
      Assert.AreEqual("0.0E+3", DecimalFraction.FromString("0.00000e+7").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.00000e+7").ToPlainString());
      Assert.AreEqual("0E-38", DecimalFraction.FromString("0.0000000000000000000e-19").ToString());
      Assert.AreEqual("0.00E-36", DecimalFraction.FromString("0.0000000000000000000e-19").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000000000000000000000", DecimalFraction.FromString("0.0000000000000000000e-19").ToPlainString());
      Assert.AreEqual("0E-16", DecimalFraction.FromString("0.0000000000e-6").ToString());
      Assert.AreEqual("0.0E-15", DecimalFraction.FromString("0.0000000000e-6").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000", DecimalFraction.FromString("0.0000000000e-6").ToPlainString());
      Assert.AreEqual("0E-32", DecimalFraction.FromString("0.00000000000000000e-15").ToString());
      Assert.AreEqual("0.00E-30", DecimalFraction.FromString("0.00000000000000000e-15").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000000000000000", DecimalFraction.FromString("0.00000000000000000e-15").ToPlainString());
      Assert.AreEqual("0E-13", DecimalFraction.FromString("0.000000000000000e+2").ToString());
      Assert.AreEqual("0.0E-12", DecimalFraction.FromString("0.000000000000000e+2").ToEngineeringString());
      Assert.AreEqual("0.0000000000000", DecimalFraction.FromString("0.000000000000000e+2").ToPlainString());
      Assert.AreEqual("0E-19", DecimalFraction.FromString("0.0e-18").ToString());
      Assert.AreEqual("0.0E-18", DecimalFraction.FromString("0.0e-18").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000", DecimalFraction.FromString("0.0e-18").ToPlainString());
      Assert.AreEqual("0E-20", DecimalFraction.FromString("0.00000000000000e-6").ToString());
      Assert.AreEqual("0.00E-18", DecimalFraction.FromString("0.00000000000000e-6").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000", DecimalFraction.FromString("0.00000000000000e-6").ToPlainString());
      Assert.AreEqual("0E-20", DecimalFraction.FromString("0.000e-17").ToString());
      Assert.AreEqual("0.00E-18", DecimalFraction.FromString("0.000e-17").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000", DecimalFraction.FromString("0.000e-17").ToPlainString());
      Assert.AreEqual("0E-21", DecimalFraction.FromString("0.00000000000000e-7").ToString());
      Assert.AreEqual("0E-21", DecimalFraction.FromString("0.00000000000000e-7").ToEngineeringString());
      Assert.AreEqual("0.000000000000000000000", DecimalFraction.FromString("0.00000000000000e-7").ToPlainString());
      Assert.AreEqual("0E-15", DecimalFraction.FromString("0.000000e-9").ToString());
      Assert.AreEqual("0E-15", DecimalFraction.FromString("0.000000e-9").ToEngineeringString());
      Assert.AreEqual("0.000000000000000", DecimalFraction.FromString("0.000000e-9").ToPlainString());
      Assert.AreEqual("0E-11", DecimalFraction.FromString("0e-11").ToString());
      Assert.AreEqual("0.00E-9", DecimalFraction.FromString("0e-11").ToEngineeringString());
      Assert.AreEqual("0.00000000000", DecimalFraction.FromString("0e-11").ToPlainString());
      Assert.AreEqual("0E+2", DecimalFraction.FromString("0.000000000e+11").ToString());
      Assert.AreEqual("0.0E+3", DecimalFraction.FromString("0.000000000e+11").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.000000000e+11").ToPlainString());
      Assert.AreEqual("0.0", DecimalFraction.FromString("0.0000000000000000e+15").ToString());
      Assert.AreEqual("0.0", DecimalFraction.FromString("0.0000000000000000e+15").ToEngineeringString());
      Assert.AreEqual("0.0", DecimalFraction.FromString("0.0000000000000000e+15").ToPlainString());
      Assert.AreEqual("0.000000", DecimalFraction.FromString("0.0000000000000000e+10").ToString());
      Assert.AreEqual("0.000000", DecimalFraction.FromString("0.0000000000000000e+10").ToEngineeringString());
      Assert.AreEqual("0.000000", DecimalFraction.FromString("0.0000000000000000e+10").ToPlainString());
      Assert.AreEqual("0.00000", DecimalFraction.FromString("0.000000000e+4").ToString());
      Assert.AreEqual("0.00000", DecimalFraction.FromString("0.000000000e+4").ToEngineeringString());
      Assert.AreEqual("0.00000", DecimalFraction.FromString("0.000000000e+4").ToPlainString());
      Assert.AreEqual("0E-28", DecimalFraction.FromString("0.000000000000000e-13").ToString());
      Assert.AreEqual("0.0E-27", DecimalFraction.FromString("0.000000000000000e-13").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000000000000000", DecimalFraction.FromString("0.000000000000000e-13").ToPlainString());
      Assert.AreEqual("0E-27", DecimalFraction.FromString("0.0000000000000000000e-8").ToString());
      Assert.AreEqual("0E-27", DecimalFraction.FromString("0.0000000000000000000e-8").ToEngineeringString());
      Assert.AreEqual("0.000000000000000000000000000", DecimalFraction.FromString("0.0000000000000000000e-8").ToPlainString());
      Assert.AreEqual("0E-26", DecimalFraction.FromString("0.00000000000e-15").ToString());
      Assert.AreEqual("0.00E-24", DecimalFraction.FromString("0.00000000000e-15").ToEngineeringString());
      Assert.AreEqual("0.00000000000000000000000000", DecimalFraction.FromString("0.00000000000e-15").ToPlainString());
      Assert.AreEqual("0E+10", DecimalFraction.FromString("0.00e+12").ToString());
      Assert.AreEqual("0.00E+12", DecimalFraction.FromString("0.00e+12").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.00e+12").ToPlainString());
      Assert.AreEqual("0E+4", DecimalFraction.FromString("0.0e+5").ToString());
      Assert.AreEqual("0.00E+6", DecimalFraction.FromString("0.0e+5").ToEngineeringString());
      Assert.AreEqual("0", DecimalFraction.FromString("0.0e+5").ToPlainString());
      Assert.AreEqual("0E-9", DecimalFraction.FromString("0.0000000000000000e+7").ToString());
      Assert.AreEqual("0E-9", DecimalFraction.FromString("0.0000000000000000e+7").ToEngineeringString());
      Assert.AreEqual("0.000000000", DecimalFraction.FromString("0.0000000000000000e+7").ToPlainString());
      Assert.AreEqual("0E-16", DecimalFraction.FromString("0.0000000000000000e-0").ToString());
      Assert.AreEqual("0.0E-15", DecimalFraction.FromString("0.0000000000000000e-0").ToEngineeringString());
      Assert.AreEqual("0.0000000000000000", DecimalFraction.FromString("0.0000000000000000e-0").ToPlainString());
      Assert.AreEqual("0.00", DecimalFraction.FromString("0.000000000000000e+13").ToString());
      Assert.AreEqual("0.00", DecimalFraction.FromString("0.000000000000000e+13").ToEngineeringString());
      Assert.AreEqual("0.00", DecimalFraction.FromString("0.000000000000000e+13").ToPlainString());
      Assert.AreEqual("0E-24", DecimalFraction.FromString("0.00000000000e-13").ToString());
      Assert.AreEqual("0E-24", DecimalFraction.FromString("0.00000000000e-13").ToEngineeringString());
      Assert.AreEqual("0.000000000000000000000000", DecimalFraction.FromString("0.00000000000e-13").ToPlainString());
      Assert.AreEqual("0E-13", DecimalFraction.FromString("0.000e-10").ToString());
      Assert.AreEqual("0.0E-12", DecimalFraction.FromString("0.000e-10").ToEngineeringString());
      Assert.AreEqual("0.0000000000000", DecimalFraction.FromString("0.000e-10").ToPlainString());
    }


    [Test]
    public void AddTest() {
      Assert.AreEqual("0.0000249885", DecimalFraction.FromString("228.16E-7").Add(DecimalFraction.FromString("217.25E-8")).ToString());
      Assert.AreEqual("0.0000206435", DecimalFraction.FromString("228.16E-7").Subtract(DecimalFraction.FromString("217.25E-8")).ToString());
      Assert.AreEqual("376000.000008029", DecimalFraction.FromString("37.6E4").Add(DecimalFraction.FromString("80.29E-7")).ToString());
      Assert.AreEqual("375999.999991971", DecimalFraction.FromString("37.6E4").Subtract(DecimalFraction.FromString("80.29E-7")).ToString());
      Assert.AreEqual("8.129029518", DecimalFraction.FromString("81.29E-1").Add(DecimalFraction.FromString("295.18E-7")).ToString());
      Assert.AreEqual("8.128970482", DecimalFraction.FromString("81.29E-1").Subtract(DecimalFraction.FromString("295.18E-7")).ToString());
      Assert.AreEqual("1961.4007420", DecimalFraction.FromString("74.20E-5").Add(DecimalFraction.FromString("196.14E1")).ToString());
      Assert.AreEqual("-1961.3992580", DecimalFraction.FromString("74.20E-5").Subtract(DecimalFraction.FromString("196.14E1")).ToString());
      Assert.AreEqual("368300.0000021732", DecimalFraction.FromString("217.32E-8").Add(DecimalFraction.FromString("368.30E3")).ToString());
      Assert.AreEqual("-368299.9999978268", DecimalFraction.FromString("217.32E-8").Subtract(DecimalFraction.FromString("368.30E3")).ToString());
      Assert.AreEqual("26.94219", DecimalFraction.FromString("269.1E-1").Add(DecimalFraction.FromString("321.9E-4")).ToString());
      Assert.AreEqual("26.87781", DecimalFraction.FromString("269.1E-1").Subtract(DecimalFraction.FromString("321.9E-4")).ToString());
      Assert.AreEqual("7.502423E+8", DecimalFraction.FromString("242.3E3").Add(DecimalFraction.FromString("7.5E8")).ToString());
      Assert.AreEqual("-7.497577E+8", DecimalFraction.FromString("242.3E3").Subtract(DecimalFraction.FromString("7.5E8")).ToString());
      Assert.AreEqual("3.0706E+8", DecimalFraction.FromString("39.16E6").Add(DecimalFraction.FromString("267.9E6")).ToString());
      Assert.AreEqual("-2.2874E+8", DecimalFraction.FromString("39.16E6").Subtract(DecimalFraction.FromString("267.9E6")).ToString());
      Assert.AreEqual("3583800036.12", DecimalFraction.FromString("36.12E0").Add(DecimalFraction.FromString("358.38E7")).ToString());
      Assert.AreEqual("-3583799963.88", DecimalFraction.FromString("36.12E0").Subtract(DecimalFraction.FromString("358.38E7")).ToString());
      Assert.AreEqual("2.7525E+8", DecimalFraction.FromString("161.15E6").Add(DecimalFraction.FromString("114.1E6")).ToString());
      Assert.AreEqual("4.705E+7", DecimalFraction.FromString("161.15E6").Subtract(DecimalFraction.FromString("114.1E6")).ToString());
      Assert.AreEqual("3212600003.9235", DecimalFraction.FromString("392.35E-2").Add(DecimalFraction.FromString("321.26E7")).ToString());
      Assert.AreEqual("-3212599996.0765", DecimalFraction.FromString("392.35E-2").Subtract(DecimalFraction.FromString("321.26E7")).ToString());
      Assert.AreEqual("3901100000.0030320", DecimalFraction.FromString("390.11E7").Add(DecimalFraction.FromString("303.20E-5")).ToString());
      Assert.AreEqual("3901099999.9969680", DecimalFraction.FromString("390.11E7").Subtract(DecimalFraction.FromString("303.20E-5")).ToString());
      Assert.AreEqual("0.0162801", DecimalFraction.FromString("120.1E-6").Add(DecimalFraction.FromString("161.6E-4")).ToString());
      Assert.AreEqual("-0.0160399", DecimalFraction.FromString("120.1E-6").Subtract(DecimalFraction.FromString("161.6E-4")).ToString());
      Assert.AreEqual("164293.814", DecimalFraction.FromString("381.4E-2").Add(DecimalFraction.FromString("164.29E3")).ToString());
      Assert.AreEqual("-164286.186", DecimalFraction.FromString("381.4E-2").Subtract(DecimalFraction.FromString("164.29E3")).ToString());
      Assert.AreEqual("263160011.325", DecimalFraction.FromString("263.16E6").Add(DecimalFraction.FromString("113.25E-1")).ToString());
      Assert.AreEqual("263159988.675", DecimalFraction.FromString("263.16E6").Subtract(DecimalFraction.FromString("113.25E-1")).ToString());
      Assert.AreEqual("0.192317", DecimalFraction.FromString("189.14E-3").Add(DecimalFraction.FromString("317.7E-5")).ToString());
      Assert.AreEqual("0.185963", DecimalFraction.FromString("189.14E-3").Subtract(DecimalFraction.FromString("317.7E-5")).ToString());
      Assert.AreEqual("400000000491.3", DecimalFraction.FromString("40.0E10").Add(DecimalFraction.FromString("49.13E1")).ToString());
      Assert.AreEqual("399999999508.7", DecimalFraction.FromString("40.0E10").Subtract(DecimalFraction.FromString("49.13E1")).ToString());
      Assert.AreEqual("0.00308683", DecimalFraction.FromString("305.33E-6").Add(DecimalFraction.FromString("278.15E-5")).ToString());
      Assert.AreEqual("-0.00247617", DecimalFraction.FromString("305.33E-6").Subtract(DecimalFraction.FromString("278.15E-5")).ToString());
      Assert.AreEqual("18012.00000012526", DecimalFraction.FromString("180.12E2").Add(DecimalFraction.FromString("125.26E-9")).ToString());
      Assert.AreEqual("18011.99999987474", DecimalFraction.FromString("180.12E2").Subtract(DecimalFraction.FromString("125.26E-9")).ToString());
      Assert.AreEqual("1795661.4", DecimalFraction.FromString("179.23E4").Add(DecimalFraction.FromString("336.14E1")).ToString());
      Assert.AreEqual("1788938.6", DecimalFraction.FromString("179.23E4").Subtract(DecimalFraction.FromString("336.14E1")).ToString());
      Assert.AreEqual("241300000.0003170", DecimalFraction.FromString("317.0E-6").Add(DecimalFraction.FromString("241.30E6")).ToString());
      Assert.AreEqual("-241299999.9996830", DecimalFraction.FromString("317.0E-6").Subtract(DecimalFraction.FromString("241.30E6")).ToString());
      Assert.AreEqual("35015000015.719", DecimalFraction.FromString("350.15E8").Add(DecimalFraction.FromString("157.19E-1")).ToString());
      Assert.AreEqual("35014999984.281", DecimalFraction.FromString("350.15E8").Subtract(DecimalFraction.FromString("157.19E-1")).ToString());
      Assert.AreEqual("3870000000000.2475", DecimalFraction.FromString("247.5E-3").Add(DecimalFraction.FromString("387.0E10")).ToString());
      Assert.AreEqual("-3869999999999.7525", DecimalFraction.FromString("247.5E-3").Subtract(DecimalFraction.FromString("387.0E10")).ToString());
      Assert.AreEqual("3.6529005234E+11", DecimalFraction.FromString("52.34E3").Add(DecimalFraction.FromString("365.29E9")).ToString());
      Assert.AreEqual("-3.6528994766E+11", DecimalFraction.FromString("52.34E3").Subtract(DecimalFraction.FromString("365.29E9")).ToString());
      Assert.AreEqual("1105.0000011612", DecimalFraction.FromString("110.5E1").Add(DecimalFraction.FromString("116.12E-8")).ToString());
      Assert.AreEqual("1104.9999988388", DecimalFraction.FromString("110.5E1").Subtract(DecimalFraction.FromString("116.12E-8")).ToString());
      Assert.AreEqual("76.16000015118", DecimalFraction.FromString("151.18E-9").Add(DecimalFraction.FromString("76.16E0")).ToString());
      Assert.AreEqual("-76.15999984882", DecimalFraction.FromString("151.18E-9").Subtract(DecimalFraction.FromString("76.16E0")).ToString());
      Assert.AreEqual("29837000022.18", DecimalFraction.FromString("298.37E8").Add(DecimalFraction.FromString("221.8E-1")).ToString());
      Assert.AreEqual("29836999977.82", DecimalFraction.FromString("298.37E8").Subtract(DecimalFraction.FromString("221.8E-1")).ToString());
      Assert.AreEqual("2724.313", DecimalFraction.FromString("268.9E1").Add(DecimalFraction.FromString("353.13E-1")).ToString());
      Assert.AreEqual("2653.687", DecimalFraction.FromString("268.9E1").Subtract(DecimalFraction.FromString("353.13E-1")).ToString());
      Assert.AreEqual("1233600.00000005427", DecimalFraction.FromString("54.27E-9").Add(DecimalFraction.FromString("123.36E4")).ToString());
      Assert.AreEqual("-1233599.99999994573", DecimalFraction.FromString("54.27E-9").Subtract(DecimalFraction.FromString("123.36E4")).ToString());
      Assert.AreEqual("445.00000008138", DecimalFraction.FromString("81.38E-9").Add(DecimalFraction.FromString("44.5E1")).ToString());
      Assert.AreEqual("-444.99999991862", DecimalFraction.FromString("81.38E-9").Subtract(DecimalFraction.FromString("44.5E1")).ToString());
      Assert.AreEqual("7.279933E+11", DecimalFraction.FromString("72.4E10").Add(DecimalFraction.FromString("399.33E7")).ToString());
      Assert.AreEqual("7.200067E+11", DecimalFraction.FromString("72.4E10").Subtract(DecimalFraction.FromString("399.33E7")).ToString());
      Assert.AreEqual("0.007319931", DecimalFraction.FromString("7.31E-3").Add(DecimalFraction.FromString("99.31E-7")).ToString());
      Assert.AreEqual("0.007300069", DecimalFraction.FromString("7.31E-3").Subtract(DecimalFraction.FromString("99.31E-7")).ToString());
      Assert.AreEqual("0.01833034824", DecimalFraction.FromString("18.33E-3").Add(DecimalFraction.FromString("348.24E-9")).ToString());
      Assert.AreEqual("0.01832965176", DecimalFraction.FromString("18.33E-3").Subtract(DecimalFraction.FromString("348.24E-9")).ToString());
      Assert.AreEqual("0.29144435", DecimalFraction.FromString("164.35E-6").Add(DecimalFraction.FromString("291.28E-3")).ToString());
      Assert.AreEqual("-0.29111565", DecimalFraction.FromString("164.35E-6").Subtract(DecimalFraction.FromString("291.28E-3")).ToString());
      Assert.AreEqual("2.11200E+8", DecimalFraction.FromString("191.39E6").Add(DecimalFraction.FromString("198.10E5")).ToString());
      Assert.AreEqual("1.71580E+8", DecimalFraction.FromString("191.39E6").Subtract(DecimalFraction.FromString("198.10E5")).ToString());
      Assert.AreEqual("2152.9000029623", DecimalFraction.FromString("296.23E-8").Add(DecimalFraction.FromString("215.29E1")).ToString());
      Assert.AreEqual("-2152.8999970377", DecimalFraction.FromString("296.23E-8").Subtract(DecimalFraction.FromString("215.29E1")).ToString());
      Assert.AreEqual("2.5135917E+8", DecimalFraction.FromString("49.17E3").Add(DecimalFraction.FromString("251.31E6")).ToString());
      Assert.AreEqual("-2.5126083E+8", DecimalFraction.FromString("49.17E3").Subtract(DecimalFraction.FromString("251.31E6")).ToString());
      Assert.AreEqual("3.92190003033E+12", DecimalFraction.FromString("392.19E10").Add(DecimalFraction.FromString("303.3E2")).ToString());
      Assert.AreEqual("3.92189996967E+12", DecimalFraction.FromString("392.19E10").Subtract(DecimalFraction.FromString("303.3E2")).ToString());
      Assert.AreEqual("58379", DecimalFraction.FromString("382.4E2").Add(DecimalFraction.FromString("201.39E2")).ToString());
      Assert.AreEqual("18101", DecimalFraction.FromString("382.4E2").Subtract(DecimalFraction.FromString("201.39E2")).ToString());
      Assert.AreEqual("28036000000.007917", DecimalFraction.FromString("79.17E-4").Add(DecimalFraction.FromString("280.36E8")).ToString());
      Assert.AreEqual("-28035999999.992083", DecimalFraction.FromString("79.17E-4").Subtract(DecimalFraction.FromString("280.36E8")).ToString());
      Assert.AreEqual("276431.37", DecimalFraction.FromString("131.37E0").Add(DecimalFraction.FromString("276.30E3")).ToString());
      Assert.AreEqual("-276168.63", DecimalFraction.FromString("131.37E0").Subtract(DecimalFraction.FromString("276.30E3")).ToString());
      Assert.AreEqual("25170015.439", DecimalFraction.FromString("25.17E6").Add(DecimalFraction.FromString("154.39E-1")).ToString());
      Assert.AreEqual("25169984.561", DecimalFraction.FromString("25.17E6").Subtract(DecimalFraction.FromString("154.39E-1")).ToString());
      Assert.AreEqual("2.173885E+10", DecimalFraction.FromString("217.17E8").Add(DecimalFraction.FromString("218.5E5")).ToString());
      Assert.AreEqual("2.169515E+10", DecimalFraction.FromString("217.17E8").Subtract(DecimalFraction.FromString("218.5E5")).ToString());
      Assert.AreEqual("529.03", DecimalFraction.FromString("18.19E1").Add(DecimalFraction.FromString("347.13E0")).ToString());
      Assert.AreEqual("-165.23", DecimalFraction.FromString("18.19E1").Subtract(DecimalFraction.FromString("347.13E0")).ToString());
      Assert.AreEqual("9.8420E+7", DecimalFraction.FromString("203.10E5").Add(DecimalFraction.FromString("78.11E6")).ToString());
      Assert.AreEqual("-5.7800E+7", DecimalFraction.FromString("203.10E5").Subtract(DecimalFraction.FromString("78.11E6")).ToString());
      Assert.AreEqual("8.71502282E+11", DecimalFraction.FromString("228.2E4").Add(DecimalFraction.FromString("87.15E10")).ToString());
      Assert.AreEqual("-8.71497718E+11", DecimalFraction.FromString("228.2E4").Subtract(DecimalFraction.FromString("87.15E10")).ToString());
      Assert.AreEqual("3571032111", DecimalFraction.FromString("321.11E2").Add(DecimalFraction.FromString("357.10E7")).ToString());
      Assert.AreEqual("-3570967889", DecimalFraction.FromString("321.11E2").Subtract(DecimalFraction.FromString("357.10E7")).ToString());
      Assert.AreEqual("5437316", DecimalFraction.FromString("54.26E5").Add(DecimalFraction.FromString("113.16E2")).ToString());
      Assert.AreEqual("5414684", DecimalFraction.FromString("54.26E5").Subtract(DecimalFraction.FromString("113.16E2")).ToString());
      Assert.AreEqual("1.2632837E+10", DecimalFraction.FromString("126.0E8").Add(DecimalFraction.FromString("328.37E5")).ToString());
      Assert.AreEqual("1.2567163E+10", DecimalFraction.FromString("126.0E8").Subtract(DecimalFraction.FromString("328.37E5")).ToString());
      Assert.AreEqual("200300000.00024232", DecimalFraction.FromString("200.30E6").Add(DecimalFraction.FromString("242.32E-6")).ToString());
      Assert.AreEqual("200299999.99975768", DecimalFraction.FromString("200.30E6").Subtract(DecimalFraction.FromString("242.32E-6")).ToString());
      Assert.AreEqual("0.00000275430", DecimalFraction.FromString("237.20E-8").Add(DecimalFraction.FromString("382.30E-9")).ToString());
      Assert.AreEqual("0.00000198970", DecimalFraction.FromString("237.20E-8").Subtract(DecimalFraction.FromString("382.30E-9")).ToString());
      Assert.AreEqual("2121600.0000011618", DecimalFraction.FromString("116.18E-8").Add(DecimalFraction.FromString("212.16E4")).ToString());
      Assert.AreEqual("-2121599.9999988382", DecimalFraction.FromString("116.18E-8").Subtract(DecimalFraction.FromString("212.16E4")).ToString());
      Assert.AreEqual("2266000.030138", DecimalFraction.FromString("226.6E4").Add(DecimalFraction.FromString("301.38E-4")).ToString());
      Assert.AreEqual("2265999.969862", DecimalFraction.FromString("226.6E4").Subtract(DecimalFraction.FromString("301.38E-4")).ToString());
      Assert.AreEqual("3541200000011.831", DecimalFraction.FromString("118.31E-1").Add(DecimalFraction.FromString("354.12E10")).ToString());
      Assert.AreEqual("-3541199999988.169", DecimalFraction.FromString("118.31E-1").Subtract(DecimalFraction.FromString("354.12E10")).ToString());
      Assert.AreEqual("26034.034113", DecimalFraction.FromString("260.34E2").Add(DecimalFraction.FromString("341.13E-4")).ToString());
      Assert.AreEqual("26033.965887", DecimalFraction.FromString("260.34E2").Subtract(DecimalFraction.FromString("341.13E-4")).ToString());
      Assert.AreEqual("29534000000.0003890", DecimalFraction.FromString("389.0E-6").Add(DecimalFraction.FromString("295.34E8")).ToString());
      Assert.AreEqual("-29533999999.9996110", DecimalFraction.FromString("389.0E-6").Subtract(DecimalFraction.FromString("295.34E8")).ToString());
      Assert.AreEqual("1081.7", DecimalFraction.FromString("0.9E3").Add(DecimalFraction.FromString("18.17E1")).ToString());
      Assert.AreEqual("718.3", DecimalFraction.FromString("0.9E3").Subtract(DecimalFraction.FromString("18.17E1")).ToString());
      Assert.AreEqual("41550.24", DecimalFraction.FromString("290.24E0").Add(DecimalFraction.FromString("41.26E3")).ToString());
      Assert.AreEqual("-40969.76", DecimalFraction.FromString("290.24E0").Subtract(DecimalFraction.FromString("41.26E3")).ToString());
      Assert.AreEqual("161.370018036", DecimalFraction.FromString("161.37E0").Add(DecimalFraction.FromString("180.36E-7")).ToString());
      Assert.AreEqual("161.369981964", DecimalFraction.FromString("161.37E0").Subtract(DecimalFraction.FromString("180.36E-7")).ToString());
      Assert.AreEqual("1.3418722E+8", DecimalFraction.FromString("134.13E6").Add(DecimalFraction.FromString("57.22E3")).ToString());
      Assert.AreEqual("1.3407278E+8", DecimalFraction.FromString("134.13E6").Subtract(DecimalFraction.FromString("57.22E3")).ToString());
      Assert.AreEqual("0.0000389329", DecimalFraction.FromString("35.20E-6").Add(DecimalFraction.FromString("373.29E-8")).ToString());
      Assert.AreEqual("0.0000314671", DecimalFraction.FromString("35.20E-6").Subtract(DecimalFraction.FromString("373.29E-8")).ToString());
      Assert.AreEqual("179000000000.33714", DecimalFraction.FromString("337.14E-3").Add(DecimalFraction.FromString("17.9E10")).ToString());
      Assert.AreEqual("-178999999999.66286", DecimalFraction.FromString("337.14E-3").Subtract(DecimalFraction.FromString("17.9E10")).ToString());
      Assert.AreEqual("79150000000.0035124", DecimalFraction.FromString("79.15E9").Add(DecimalFraction.FromString("351.24E-5")).ToString());
      Assert.AreEqual("79149999999.9964876", DecimalFraction.FromString("79.15E9").Subtract(DecimalFraction.FromString("351.24E-5")).ToString());
      Assert.AreEqual("2.29225713E+12", DecimalFraction.FromString("229.20E10").Add(DecimalFraction.FromString("257.13E6")).ToString());
      Assert.AreEqual("2.29174287E+12", DecimalFraction.FromString("229.20E10").Subtract(DecimalFraction.FromString("257.13E6")).ToString());
      Assert.AreEqual("350160.05632", DecimalFraction.FromString("56.32E-3").Add(DecimalFraction.FromString("350.16E3")).ToString());
      Assert.AreEqual("-350159.94368", DecimalFraction.FromString("56.32E-3").Subtract(DecimalFraction.FromString("350.16E3")).ToString());
      Assert.AreEqual("101600.0000000955", DecimalFraction.FromString("10.16E4").Add(DecimalFraction.FromString("95.5E-9")).ToString());
      Assert.AreEqual("101599.9999999045", DecimalFraction.FromString("10.16E4").Subtract(DecimalFraction.FromString("95.5E-9")).ToString());
      Assert.AreEqual("1131000000.001075", DecimalFraction.FromString("107.5E-5").Add(DecimalFraction.FromString("113.10E7")).ToString());
      Assert.AreEqual("-1130999999.998925", DecimalFraction.FromString("107.5E-5").Subtract(DecimalFraction.FromString("113.10E7")).ToString());
      Assert.AreEqual("597.30", DecimalFraction.FromString("227.2E0").Add(DecimalFraction.FromString("370.10E0")).ToString());
      Assert.AreEqual("-142.90", DecimalFraction.FromString("227.2E0").Subtract(DecimalFraction.FromString("370.10E0")).ToString());
      Assert.AreEqual("371491.2", DecimalFraction.FromString("189.12E1").Add(DecimalFraction.FromString("369.6E3")).ToString());
      Assert.AreEqual("-367708.8", DecimalFraction.FromString("189.12E1").Subtract(DecimalFraction.FromString("369.6E3")).ToString());
      Assert.AreEqual("291260000.0003901", DecimalFraction.FromString("390.1E-6").Add(DecimalFraction.FromString("291.26E6")).ToString());
      Assert.AreEqual("-291259999.9996099", DecimalFraction.FromString("390.1E-6").Subtract(DecimalFraction.FromString("291.26E6")).ToString());
      Assert.AreEqual("26.13600029222", DecimalFraction.FromString("261.36E-1").Add(DecimalFraction.FromString("292.22E-9")).ToString());
      Assert.AreEqual("26.13599970778", DecimalFraction.FromString("261.36E-1").Subtract(DecimalFraction.FromString("292.22E-9")).ToString());
      Assert.AreEqual("327190000000.0000003319", DecimalFraction.FromString("33.19E-8").Add(DecimalFraction.FromString("327.19E9")).ToString());
      Assert.AreEqual("-327189999999.9999996681", DecimalFraction.FromString("33.19E-8").Subtract(DecimalFraction.FromString("327.19E9")).ToString());
      Assert.AreEqual("185.802104", DecimalFraction.FromString("210.4E-5").Add(DecimalFraction.FromString("185.8E0")).ToString());
      Assert.AreEqual("-185.797896", DecimalFraction.FromString("210.4E-5").Subtract(DecimalFraction.FromString("185.8E0")).ToString());
      Assert.AreEqual("2.243535637E+12", DecimalFraction.FromString("224.35E10").Add(DecimalFraction.FromString("356.37E5")).ToString());
      Assert.AreEqual("2.243464363E+12", DecimalFraction.FromString("224.35E10").Subtract(DecimalFraction.FromString("356.37E5")).ToString());
      Assert.AreEqual("472700.01048", DecimalFraction.FromString("47.27E4").Add(DecimalFraction.FromString("104.8E-4")).ToString());
      Assert.AreEqual("472699.98952", DecimalFraction.FromString("47.27E4").Subtract(DecimalFraction.FromString("104.8E-4")).ToString());
      Assert.AreEqual("1471800.02795", DecimalFraction.FromString("147.18E4").Add(DecimalFraction.FromString("279.5E-4")).ToString());
      Assert.AreEqual("1471799.97205", DecimalFraction.FromString("147.18E4").Subtract(DecimalFraction.FromString("279.5E-4")).ToString());
      Assert.AreEqual("0.33453", DecimalFraction.FromString("11.5E-4").Add(DecimalFraction.FromString("333.38E-3")).ToString());
      Assert.AreEqual("-0.33223", DecimalFraction.FromString("11.5E-4").Subtract(DecimalFraction.FromString("333.38E-3")).ToString());
      Assert.AreEqual("0.531437", DecimalFraction.FromString("5.28E-1").Add(DecimalFraction.FromString("343.7E-5")).ToString());
      Assert.AreEqual("0.524563", DecimalFraction.FromString("5.28E-1").Subtract(DecimalFraction.FromString("343.7E-5")).ToString());
      Assert.AreEqual("1.251214E+9", DecimalFraction.FromString("381.14E5").Add(DecimalFraction.FromString("121.31E7")).ToString());
      Assert.AreEqual("-1.174986E+9", DecimalFraction.FromString("381.14E5").Subtract(DecimalFraction.FromString("121.31E7")).ToString());
      Assert.AreEqual("15016.2", DecimalFraction.FromString("145.25E2").Add(DecimalFraction.FromString("49.12E1")).ToString());
      Assert.AreEqual("14033.8", DecimalFraction.FromString("145.25E2").Subtract(DecimalFraction.FromString("49.12E1")).ToString());
      Assert.AreEqual("173700000332.13", DecimalFraction.FromString("332.13E0").Add(DecimalFraction.FromString("173.7E9")).ToString());
      Assert.AreEqual("-173699999667.87", DecimalFraction.FromString("332.13E0").Subtract(DecimalFraction.FromString("173.7E9")).ToString());
      Assert.AreEqual("0.38234333", DecimalFraction.FromString("38.20E-2").Add(DecimalFraction.FromString("343.33E-6")).ToString());
      Assert.AreEqual("0.38165667", DecimalFraction.FromString("38.20E-2").Subtract(DecimalFraction.FromString("343.33E-6")).ToString());
      Assert.AreEqual("415000017.234", DecimalFraction.FromString("4.15E8").Add(DecimalFraction.FromString("172.34E-1")).ToString());
      Assert.AreEqual("414999982.766", DecimalFraction.FromString("4.15E8").Subtract(DecimalFraction.FromString("172.34E-1")).ToString());
      Assert.AreEqual("3.5335001591E+12", DecimalFraction.FromString("353.35E10").Add(DecimalFraction.FromString("159.1E3")).ToString());
      Assert.AreEqual("3.5334998409E+12", DecimalFraction.FromString("353.35E10").Subtract(DecimalFraction.FromString("159.1E3")).ToString());
      Assert.AreEqual("16414.6838", DecimalFraction.FromString("268.38E-2").Add(DecimalFraction.FromString("164.12E2")).ToString());
      Assert.AreEqual("-16409.3162", DecimalFraction.FromString("268.38E-2").Subtract(DecimalFraction.FromString("164.12E2")).ToString());
      Assert.AreEqual("1.4010003544E+12", DecimalFraction.FromString("354.4E3").Add(DecimalFraction.FromString("140.1E10")).ToString());
      Assert.AreEqual("-1.4009996456E+12", DecimalFraction.FromString("354.4E3").Subtract(DecimalFraction.FromString("140.1E10")).ToString());
      Assert.AreEqual("2083800000.0007613", DecimalFraction.FromString("76.13E-5").Add(DecimalFraction.FromString("208.38E7")).ToString());
      Assert.AreEqual("-2083799999.9992387", DecimalFraction.FromString("76.13E-5").Subtract(DecimalFraction.FromString("208.38E7")).ToString());
      Assert.AreEqual("14.91800012724", DecimalFraction.FromString("127.24E-9").Add(DecimalFraction.FromString("149.18E-1")).ToString());
      Assert.AreEqual("-14.91799987276", DecimalFraction.FromString("127.24E-9").Subtract(DecimalFraction.FromString("149.18E-1")).ToString());
      Assert.AreEqual("0.00023156", DecimalFraction.FromString("19.34E-5").Add(DecimalFraction.FromString("38.16E-6")).ToString());
      Assert.AreEqual("0.00015524", DecimalFraction.FromString("19.34E-5").Subtract(DecimalFraction.FromString("38.16E-6")).ToString());
      Assert.AreEqual("12538.0000020020", DecimalFraction.FromString("125.38E2").Add(DecimalFraction.FromString("200.20E-8")).ToString());
      Assert.AreEqual("12537.9999979980", DecimalFraction.FromString("125.38E2").Subtract(DecimalFraction.FromString("200.20E-8")).ToString());
      Assert.AreEqual("0.00051186", DecimalFraction.FromString("127.16E-6").Add(DecimalFraction.FromString("384.7E-6")).ToString());
      Assert.AreEqual("-0.00025754", DecimalFraction.FromString("127.16E-6").Subtract(DecimalFraction.FromString("384.7E-6")).ToString());
      Assert.AreEqual("707000.00009722", DecimalFraction.FromString("70.7E4").Add(DecimalFraction.FromString("97.22E-6")).ToString());
      Assert.AreEqual("706999.99990278", DecimalFraction.FromString("70.7E4").Subtract(DecimalFraction.FromString("97.22E-6")).ToString());
      Assert.AreEqual("2.8697E+10", DecimalFraction.FromString("109.7E8").Add(DecimalFraction.FromString("177.27E8")).ToString());
      Assert.AreEqual("-6.757E+9", DecimalFraction.FromString("109.7E8").Subtract(DecimalFraction.FromString("177.27E8")).ToString());
      Assert.AreEqual("276350.0000012426", DecimalFraction.FromString("124.26E-8").Add(DecimalFraction.FromString("276.35E3")).ToString());
      Assert.AreEqual("-276349.9999987574", DecimalFraction.FromString("124.26E-8").Subtract(DecimalFraction.FromString("276.35E3")).ToString());
      Assert.AreEqual("56352719", DecimalFraction.FromString("56.34E6").Add(DecimalFraction.FromString("127.19E2")).ToString());
      Assert.AreEqual("56327281", DecimalFraction.FromString("56.34E6").Subtract(DecimalFraction.FromString("127.19E2")).ToString());
      Assert.AreEqual("1.3220031539E+11", DecimalFraction.FromString("132.20E9").Add(DecimalFraction.FromString("315.39E3")).ToString());
      Assert.AreEqual("1.3219968461E+11", DecimalFraction.FromString("132.20E9").Subtract(DecimalFraction.FromString("315.39E3")).ToString());
      Assert.AreEqual("6.3272236E+8", DecimalFraction.FromString("22.36E3").Add(DecimalFraction.FromString("63.27E7")).ToString());
      Assert.AreEqual("-6.3267764E+8", DecimalFraction.FromString("22.36E3").Subtract(DecimalFraction.FromString("63.27E7")).ToString());
      Assert.AreEqual("151380000000.05331", DecimalFraction.FromString("151.38E9").Add(DecimalFraction.FromString("53.31E-3")).ToString());
      Assert.AreEqual("151379999999.94669", DecimalFraction.FromString("151.38E9").Subtract(DecimalFraction.FromString("53.31E-3")).ToString());
      Assert.AreEqual("24522000.00000004119", DecimalFraction.FromString("245.22E5").Add(DecimalFraction.FromString("41.19E-9")).ToString());
      Assert.AreEqual("24521999.99999995881", DecimalFraction.FromString("245.22E5").Subtract(DecimalFraction.FromString("41.19E-9")).ToString());
      Assert.AreEqual("32539.12334", DecimalFraction.FromString("123.34E-3").Add(DecimalFraction.FromString("325.39E2")).ToString());
      Assert.AreEqual("-32538.87666", DecimalFraction.FromString("123.34E-3").Subtract(DecimalFraction.FromString("325.39E2")).ToString());
    }

    [Test]
    public void MultiplyTest() {
      Assert.AreEqual("1.23885300E+9", DecimalFraction.FromString("51.15E8").Multiply(DecimalFraction.FromString("242.20E-3")).ToString());
      Assert.AreEqual("0.001106186758", DecimalFraction.FromString("373.22E-1").Multiply(DecimalFraction.FromString("296.39E-7")).ToString());
      Assert.AreEqual("192.9180", DecimalFraction.FromString("11.0E-4").Multiply(DecimalFraction.FromString("175.38E3")).ToString());
      Assert.AreEqual("0.000013640373", DecimalFraction.FromString("27.21E-6").Multiply(DecimalFraction.FromString("50.13E-2")).ToString());
      Assert.AreEqual("0.00000515564630", DecimalFraction.FromString("138.11E-2").Multiply(DecimalFraction.FromString("373.30E-8")).ToString());
      Assert.AreEqual("3.3450518E+8", DecimalFraction.FromString("221.38E9").Multiply(DecimalFraction.FromString("15.11E-4")).ToString());
      Assert.AreEqual("0.0000033748442", DecimalFraction.FromString("278.2E-5").Multiply(DecimalFraction.FromString("121.31E-5")).ToString());
      Assert.AreEqual("1.039277030E+15", DecimalFraction.FromString("369.35E0").Multiply(DecimalFraction.FromString("281.38E10")).ToString());
      Assert.AreEqual("237138.92", DecimalFraction.FromString("393.2E-1").Multiply(DecimalFraction.FromString("60.31E2")).ToString());
      Assert.AreEqual("6.5073942E+11", DecimalFraction.FromString("208.17E3").Multiply(DecimalFraction.FromString("31.26E5")).ToString());
      Assert.AreEqual("1685.5032", DecimalFraction.FromString("7.32E0").Multiply(DecimalFraction.FromString("230.26E0")).ToString());
      Assert.AreEqual("0.00441400570", DecimalFraction.FromString("170.30E-1").Multiply(DecimalFraction.FromString("259.19E-6")).ToString());
      Assert.AreEqual("4.41514794E+9", DecimalFraction.FromString("326.13E3").Multiply(DecimalFraction.FromString("135.38E2")).ToString());
      Assert.AreEqual("139070.220", DecimalFraction.FromString("82.12E5").Multiply(DecimalFraction.FromString("169.35E-4")).ToString());
      Assert.AreEqual("1.182023125E+17", DecimalFraction.FromString("319.25E3").Multiply(DecimalFraction.FromString("370.25E9")).ToString());
      Assert.AreEqual("18397.593", DecimalFraction.FromString("12.33E3").Multiply(DecimalFraction.FromString("149.21E-2")).ToString());
      Assert.AreEqual("8.0219160E+14", DecimalFraction.FromString("170.10E10").Multiply(DecimalFraction.FromString("47.16E1")).ToString());
      Assert.AreEqual("8.23380426E+11", DecimalFraction.FromString("219.34E-3").Multiply(DecimalFraction.FromString("375.39E10")).ToString());
      Assert.AreEqual("1036.89700", DecimalFraction.FromString("318.8E1").Multiply(DecimalFraction.FromString("325.25E-3")).ToString());
      Assert.AreEqual("1013.077141", DecimalFraction.FromString("319.19E-3").Multiply(DecimalFraction.FromString("317.39E1")).ToString());
      Assert.AreEqual("1.2831563E+13", DecimalFraction.FromString("14.39E6").Multiply(DecimalFraction.FromString("89.17E4")).ToString());
      Assert.AreEqual("0.036472384", DecimalFraction.FromString("386.36E1").Multiply(DecimalFraction.FromString("94.4E-7")).ToString());
      Assert.AreEqual("7.5994752E+16", DecimalFraction.FromString("280.32E6").Multiply(DecimalFraction.FromString("271.1E6")).ToString());
      Assert.AreEqual("4.1985417", DecimalFraction.FromString("107.3E-5").Multiply(DecimalFraction.FromString("391.29E1")).ToString());
      Assert.AreEqual("81530.63", DecimalFraction.FromString("31.37E-5").Multiply(DecimalFraction.FromString("259.9E6")).ToString());
      Assert.AreEqual("4.543341E-10", DecimalFraction.FromString("372.1E-7").Multiply(DecimalFraction.FromString("12.21E-6")).ToString());
      Assert.AreEqual("3.77698530E-9", DecimalFraction.FromString("306.30E-7").Multiply(DecimalFraction.FromString("123.31E-6")).ToString());
      Assert.AreEqual("3.708195E+9", DecimalFraction.FromString("318.3E10").Multiply(DecimalFraction.FromString("116.5E-5")).ToString());
      Assert.AreEqual("413.87661", DecimalFraction.FromString("252.21E-5").Multiply(DecimalFraction.FromString("164.1E3")).ToString());
      Assert.AreEqual("7.1053840E+8", DecimalFraction.FromString("124.22E-4").Multiply(DecimalFraction.FromString("57.20E9")).ToString());
      Assert.AreEqual("481.335452", DecimalFraction.FromString("178.18E-7").Multiply(DecimalFraction.FromString("270.14E5")).ToString());
      Assert.AreEqual("2.61361E-10", DecimalFraction.FromString("84.31E-3").Multiply(DecimalFraction.FromString("3.1E-9")).ToString());
      Assert.AreEqual("2.00365428E-7", DecimalFraction.FromString("84.12E-8").Multiply(DecimalFraction.FromString("238.19E-3")).ToString());
      Assert.AreEqual("0.0000259582890", DecimalFraction.FromString("153.30E-9").Multiply(DecimalFraction.FromString("169.33E0")).ToString());
      Assert.AreEqual("10263.70", DecimalFraction.FromString("98.5E-8").Multiply(DecimalFraction.FromString("104.2E8")).ToString());
      Assert.AreEqual("0.057940056", DecimalFraction.FromString("77.13E-7").Multiply(DecimalFraction.FromString("75.12E2")).ToString());
      Assert.AreEqual("169852062", DecimalFraction.FromString("89.33E-6").Multiply(DecimalFraction.FromString("190.14E10")).ToString());
      Assert.AreEqual("1384468.2", DecimalFraction.FromString("252.18E6").Multiply(DecimalFraction.FromString("54.9E-4")).ToString());
      Assert.AreEqual("1.4882985E+12", DecimalFraction.FromString("46.35E-1").Multiply(DecimalFraction.FromString("32.11E10")).ToString());
      Assert.AreEqual("2.7130378E+10", DecimalFraction.FromString("347.38E5").Multiply(DecimalFraction.FromString("78.1E1")).ToString());
      Assert.AreEqual("1.1816933E-10", DecimalFraction.FromString("31.27E-5").Multiply(DecimalFraction.FromString("377.9E-9")).ToString());
      Assert.AreEqual("3.9434566E+10", DecimalFraction.FromString("119.8E-4").Multiply(DecimalFraction.FromString("329.17E10")).ToString());
      Assert.AreEqual("5.72427", DecimalFraction.FromString("19.1E-2").Multiply(DecimalFraction.FromString("299.7E-1")).ToString());
      Assert.AreEqual("1.890600E+17", DecimalFraction.FromString("82.2E9").Multiply(DecimalFraction.FromString("230.0E4")).ToString());
      Assert.AreEqual("8.24813976E+11", DecimalFraction.FromString("398.23E5").Multiply(DecimalFraction.FromString("207.12E2")).ToString());
      Assert.AreEqual("9.923540E+14", DecimalFraction.FromString("47.30E8").Multiply(DecimalFraction.FromString("209.8E3")).ToString());
      Assert.AreEqual("13682832.2", DecimalFraction.FromString("383.38E-5").Multiply(DecimalFraction.FromString("356.9E7")).ToString());
      Assert.AreEqual("1.476482154E+10", DecimalFraction.FromString("375.38E-3").Multiply(DecimalFraction.FromString("393.33E8")).ToString());
      Assert.AreEqual("1.036217389E+19", DecimalFraction.FromString("285.31E9").Multiply(DecimalFraction.FromString("363.19E5")).ToString());
      Assert.AreEqual("951399862", DecimalFraction.FromString("252.14E8").Multiply(DecimalFraction.FromString("377.33E-4")).ToString());
      Assert.AreEqual("1.143972712E+16", DecimalFraction.FromString("307.28E4").Multiply(DecimalFraction.FromString("372.29E7")).ToString());
      Assert.AreEqual("602.640", DecimalFraction.FromString("2.16E8").Multiply(DecimalFraction.FromString("279.0E-8")).ToString());
      Assert.AreEqual("5711.3430", DecimalFraction.FromString("182.18E-9").Multiply(DecimalFraction.FromString("31.35E9")).ToString());
      Assert.AreEqual("366.054821", DecimalFraction.FromString("149.27E-4").Multiply(DecimalFraction.FromString("245.23E2")).ToString());
      Assert.AreEqual("12901.2750", DecimalFraction.FromString("372.6E-1").Multiply(DecimalFraction.FromString("346.25E0")).ToString());
      Assert.AreEqual("201642636", DecimalFraction.FromString("61.23E-1").Multiply(DecimalFraction.FromString("329.32E5")).ToString());
      Assert.AreEqual("1.64376210E+16", DecimalFraction.FromString("133.26E10").Multiply(DecimalFraction.FromString("123.35E2")).ToString());
      Assert.AreEqual("3.084818E+18", DecimalFraction.FromString("309.1E9").Multiply(DecimalFraction.FromString("99.8E5")).ToString());
      Assert.AreEqual("0.4925852", DecimalFraction.FromString("230.18E4").Multiply(DecimalFraction.FromString("2.14E-7")).ToString());
      Assert.AreEqual("322.455112", DecimalFraction.FromString("387.38E-3").Multiply(DecimalFraction.FromString("83.24E1")).ToString());
      Assert.AreEqual("0.9306528", DecimalFraction.FromString("377.7E-2").Multiply(DecimalFraction.FromString("246.4E-3")).ToString());
      Assert.AreEqual("2.251919", DecimalFraction.FromString("169.7E0").Multiply(DecimalFraction.FromString("13.27E-3")).ToString());
      Assert.AreEqual("682846382", DecimalFraction.FromString("385.31E3").Multiply(DecimalFraction.FromString("177.22E1")).ToString());
      Assert.AreEqual("11338.90625", DecimalFraction.FromString("306.25E-7").Multiply(DecimalFraction.FromString("370.25E6")).ToString());
      Assert.AreEqual("1.3389740E+9", DecimalFraction.FromString("49.0E9").Multiply(DecimalFraction.FromString("273.26E-4")).ToString());
      Assert.AreEqual("5.4483030E+18", DecimalFraction.FromString("160.15E6").Multiply(DecimalFraction.FromString("340.2E8")).ToString());
      Assert.AreEqual("9.3219568E+8", DecimalFraction.FromString("109.31E3").Multiply(DecimalFraction.FromString("85.28E2")).ToString());
      Assert.AreEqual("6.9666450", DecimalFraction.FromString("90.30E1").Multiply(DecimalFraction.FromString("77.15E-4")).ToString());
      Assert.AreEqual("1.25459658E-7", DecimalFraction.FromString("81.33E-3").Multiply(DecimalFraction.FromString("154.26E-8")).ToString());
      Assert.AreEqual("0.0001433757", DecimalFraction.FromString("378.3E-5").Multiply(DecimalFraction.FromString("37.9E-3")).ToString());
      Assert.AreEqual("275.60856", DecimalFraction.FromString("310.37E-5").Multiply(DecimalFraction.FromString("88.8E3")).ToString());
      Assert.AreEqual("70.4246032", DecimalFraction.FromString("188.12E-9").Multiply(DecimalFraction.FromString("374.36E6")).ToString());
      Assert.AreEqual("2.0905404E+9", DecimalFraction.FromString("75.4E1").Multiply(DecimalFraction.FromString("277.26E4")).ToString());
      Assert.AreEqual("8.5164440E+16", DecimalFraction.FromString("346.0E4").Multiply(DecimalFraction.FromString("246.14E8")).ToString());
      Assert.AreEqual("5836929.0", DecimalFraction.FromString("41.30E1").Multiply(DecimalFraction.FromString("141.33E2")).ToString());
      Assert.AreEqual("9.632727E-8", DecimalFraction.FromString("44.37E-8").Multiply(DecimalFraction.FromString("217.1E-3")).ToString());
      Assert.AreEqual("1.0707983E+14", DecimalFraction.FromString("7.27E1").Multiply(DecimalFraction.FromString("147.29E10")).ToString());
      Assert.AreEqual("650476.8", DecimalFraction.FromString("165.6E6").Multiply(DecimalFraction.FromString("392.8E-5")).ToString());
      Assert.AreEqual("5.9438181E+9", DecimalFraction.FromString("309.3E-1").Multiply(DecimalFraction.FromString("192.17E6")).ToString());
      Assert.AreEqual("5.07150E+14", DecimalFraction.FromString("48.30E3").Multiply(DecimalFraction.FromString("10.5E9")).ToString());
      Assert.AreEqual("687748.662", DecimalFraction.FromString("333.26E5").Multiply(DecimalFraction.FromString("206.37E-4")).ToString());
      Assert.AreEqual("18.3678360", DecimalFraction.FromString("49.20E3").Multiply(DecimalFraction.FromString("373.33E-6")).ToString());
      Assert.AreEqual("2.071383E+13", DecimalFraction.FromString("252.3E0").Multiply(DecimalFraction.FromString("8.21E10")).ToString());
      Assert.AreEqual("2.86793244E+21", DecimalFraction.FromString("96.12E8").Multiply(DecimalFraction.FromString("298.37E9")).ToString());
      Assert.AreEqual("1.346378792E+16", DecimalFraction.FromString("342.32E3").Multiply(DecimalFraction.FromString("393.31E8")).ToString());
      Assert.AreEqual("4.5974844E-8", DecimalFraction.FromString("40.23E-2").Multiply(DecimalFraction.FromString("114.28E-9")).ToString());
      Assert.AreEqual("0.74529156", DecimalFraction.FromString("320.28E5").Multiply(DecimalFraction.FromString("23.27E-9")).ToString());
      Assert.AreEqual("8398794.5", DecimalFraction.FromString("372.7E-1").Multiply(DecimalFraction.FromString("225.35E3")).ToString());
      Assert.AreEqual("5.9243200E+9", DecimalFraction.FromString("303.5E-5").Multiply(DecimalFraction.FromString("195.20E10")).ToString());
      Assert.AreEqual("0.14321792", DecimalFraction.FromString("131.2E-7").Multiply(DecimalFraction.FromString("109.16E2")).ToString());
      Assert.AreEqual("4.9518322E+11", DecimalFraction.FromString("230.2E2").Multiply(DecimalFraction.FromString("215.11E5")).ToString());
      Assert.AreEqual("14.1640814", DecimalFraction.FromString("170.18E4").Multiply(DecimalFraction.FromString("83.23E-7")).ToString());
      Assert.AreEqual("1.18653228E-7", DecimalFraction.FromString("102.12E-9").Multiply(DecimalFraction.FromString("116.19E-2")).ToString());
      Assert.AreEqual("20220.7104", DecimalFraction.FromString("319.14E3").Multiply(DecimalFraction.FromString("63.36E-3")).ToString());
      Assert.AreEqual("1.003818480E+23", DecimalFraction.FromString("263.20E8").Multiply(DecimalFraction.FromString("381.39E10")).ToString());
      Assert.AreEqual("0.0270150690", DecimalFraction.FromString("350.39E-6").Multiply(DecimalFraction.FromString("77.10E0")).ToString());
      Assert.AreEqual("3.338496E+19", DecimalFraction.FromString("124.2E8").Multiply(DecimalFraction.FromString("268.8E7")).ToString());
      Assert.AreEqual("15983.9650", DecimalFraction.FromString("60.26E4").Multiply(DecimalFraction.FromString("265.25E-4")).ToString());
      Assert.AreEqual("14.674005", DecimalFraction.FromString("139.5E3").Multiply(DecimalFraction.FromString("105.19E-6")).ToString());
      Assert.AreEqual("3469019.40", DecimalFraction.FromString("160.38E2").Multiply(DecimalFraction.FromString("216.30E0")).ToString());
    }
    
    // Tests whether AsInt32/64/16/AsByte properly truncate floats
    // and doubles before bounds checking
    [Test]
    public void FloatingPointCloseToEdge(){
      try { CBORObject.FromObject(2.147483647E9d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483647E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483647E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483647E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836470000002E9d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836470000002E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836470000002E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836470000002E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836469999998E9d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836469999998E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836469999998E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836469999998E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483648E9d).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483648E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483648E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483648E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836480000005E9d).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836480000005E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836480000005E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836480000005E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836479999998E9d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836479999998E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836479999998E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836479999998E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483646E9d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483646E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483646E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.147483646E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836460000002E9d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836460000002E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836460000002E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836460000002E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836459999998E9d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836459999998E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836459999998E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474836459999998E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483648E9d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483648E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483648E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483648E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836479999998E9d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836479999998E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836479999998E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836479999998E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836480000005E9d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836480000005E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836480000005E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836480000005E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483647E9d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483647E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483647E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483647E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836469999998E9d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836469999998E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836469999998E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836469999998E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836470000002E9d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836470000002E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836470000002E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836470000002E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483649E9d).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483649E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483649E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.147483649E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836489999995E9d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836489999995E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836489999995E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836489999995E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836490000005E9d).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836490000005E9d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836490000005E9d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474836490000005E9d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854776E18d).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854776E18d).AsInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854776E18d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854776E18d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854778E18d).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854778E18d).AsInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854778E18d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372036854778E18d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233720368547748E18d).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233720368547748E18d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233720368547748E18d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233720368547748E18d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854776E18d).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854776E18d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854776E18d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854776E18d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233720368547748E18d).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233720368547748E18d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233720368547748E18d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233720368547748E18d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854778E18d).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854778E18d).AsInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854778E18d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372036854778E18d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.000000000004d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.000000000004d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.000000000004d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.000000000004d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.999999999996d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.999999999996d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.999999999996d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.999999999996d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.00000000001d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.00000000001d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.00000000001d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.00000000001d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.999999999996d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.999999999996d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.999999999996d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.999999999996d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.000000000004d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.000000000004d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.000000000004d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.000000000004d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.999999999996d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.999999999996d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.999999999996d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.999999999996d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.999999999996d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.999999999996d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.999999999996d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.999999999996d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.00000000001d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.00000000001d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.00000000001d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.00000000001d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.999999999996d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.999999999996d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.999999999996d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.999999999996d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.000000000004d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.000000000004d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.000000000004d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.000000000004d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.99999999999d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.99999999999d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.99999999999d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.99999999999d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.00000000001d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.00000000001d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.00000000001d).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.00000000001d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0d).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.9E-324d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.9E-324d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.9E-324d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(4.9E-324d).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-4.9E-324d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-4.9E-324d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-4.9E-324d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-4.9E-324d).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0d).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000000000000002d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000000000000002d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000000000000002d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000000000000002d).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.9999999999999999d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.9999999999999999d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.9999999999999999d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.9999999999999999d).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.9999999999999999d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.9999999999999999d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.9999999999999999d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.9999999999999999d).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000000000000002d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000000000000002d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000000000000002d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000000000000002d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0d).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00000000000003d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00000000000003d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00000000000003d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00000000000003d).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99999999999997d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99999999999997d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99999999999997d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99999999999997d).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00000000000006d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00000000000006d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00000000000006d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00000000000006d).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99999999999997d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99999999999997d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99999999999997d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99999999999997d).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0d).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00000000000003d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00000000000003d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00000000000003d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00000000000003d).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99999999999997d).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99999999999997d).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99999999999997d).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99999999999997d).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748365E9f).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748365E9f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748365E9f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748365E9f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474839E9f).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474839E9f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474839E9f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.1474839E9f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748352E9f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748352E9f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748352E9f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(2.14748352E9f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748365E9f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748365E9f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748365E9f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748365E9f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748352E9f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748352E9f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748352E9f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.14748352E9f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474839E9f).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474839E9f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474839E9f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-2.1474839E9f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372E18f).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372E18f).AsInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372E18f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223372E18f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223373E18f).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223373E18f).AsInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223373E18f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.223373E18f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233715E18f).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233715E18f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233715E18f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(9.2233715E18f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372E18f).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372E18f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372E18f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223372E18f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233715E18f).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233715E18f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233715E18f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.2233715E18f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223373E18f).AsInt32(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223373E18f).AsInt64(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223373E18f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-9.223373E18f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.0f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.002f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.002f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.002f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.002f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.998f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.998f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.998f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.998f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.0f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.004f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.004f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.004f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32768.004f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.998f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.998f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.998f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32767.998f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.0f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.002f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.002f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.002f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32766.002f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.998f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.998f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.998f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(32765.998f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.0f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.998f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.998f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.998f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.998f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.004f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.004f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.004f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.004f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.0f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.998f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.998f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.998f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32766.998f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.002f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.002f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.002f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32767.002f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.0f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.996f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.996f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.996f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32768.996f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.004f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.004f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.004f).AsInt16(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-32769.004f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.0f).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.4E-45f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.4E-45f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.4E-45f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.4E-45f).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.4E-45f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.4E-45f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.4E-45f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.4E-45f).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0f).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000001f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000001f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000001f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(1.0000001f).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.99999994f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.99999994f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.99999994f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(0.99999994f).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.99999994f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.99999994f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.99999994f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-0.99999994f).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000001f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000001f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000001f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(-1.0000001f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.0f).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00002f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00002f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00002f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.00002f).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99998f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99998f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99998f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.99998f).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.0f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00003f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00003f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00003f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(256.00003f).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99998f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99998f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99998f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(255.99998f).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.0f).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00002f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00002f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00002f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(254.00002f).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99998f).AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99998f).AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99998f).AsInt16(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromObject(253.99998f).AsByte(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
    }
    
    [Test]
    public void FromDoubleTest() {
      Assert.AreEqual("0.213299999999999989608312489508534781634807586669921875", DecimalFraction.FromDouble(0.2133).ToString());
      Assert.AreEqual("2.29360000000000010330982488752915582352898127282969653606414794921875E-7", DecimalFraction.FromDouble(2.2936E-7).ToString());
      Assert.AreEqual("3893200000", DecimalFraction.FromDouble(3.8932E9).ToString());
      Assert.AreEqual("128230", DecimalFraction.FromDouble(128230.0).ToString());
      Assert.AreEqual("127210", DecimalFraction.FromDouble(127210.0).ToString());
      Assert.AreEqual("0.267230000000000023074875343809253536164760589599609375", DecimalFraction.FromDouble(0.26723).ToString());
      Assert.AreEqual("0.302329999999999987636556397774256765842437744140625", DecimalFraction.FromDouble(0.30233).ToString());
      Assert.AreEqual("0.0000019512000000000000548530838806460252499164198525249958038330078125", DecimalFraction.FromDouble(1.9512E-6).ToString());
      Assert.AreEqual("199500", DecimalFraction.FromDouble(199500.0).ToString());
      Assert.AreEqual("36214000", DecimalFraction.FromDouble(3.6214E7).ToString());
      Assert.AreEqual("1913300000000", DecimalFraction.FromDouble(1.9133E12).ToString());
      Assert.AreEqual("0.0002173499999999999976289799530349000633577816188335418701171875", DecimalFraction.FromDouble(2.1735E-4).ToString());
      Assert.AreEqual("0.0000310349999999999967797807698399736864303122274577617645263671875", DecimalFraction.FromDouble(3.1035E-5).ToString());
      Assert.AreEqual("1.274999999999999911182158029987476766109466552734375", DecimalFraction.FromDouble(1.275).ToString());
      Assert.AreEqual("214190", DecimalFraction.FromDouble(214190.0).ToString());
      Assert.AreEqual("3981300000", DecimalFraction.FromDouble(3.9813E9).ToString());
      Assert.AreEqual("1092700", DecimalFraction.FromDouble(1092700.0).ToString());
      Assert.AreEqual("0.023609999999999999042987752773115062154829502105712890625", DecimalFraction.FromDouble(0.02361).ToString());
      Assert.AreEqual("12.321999999999999175770426518283784389495849609375", DecimalFraction.FromDouble(12.322).ToString());
      Assert.AreEqual("0.002586999999999999889921387108415729016996920108795166015625", DecimalFraction.FromDouble(0.002587).ToString());
      Assert.AreEqual("1322000000", DecimalFraction.FromDouble(1.322E9).ToString());
      Assert.AreEqual("95310000000", DecimalFraction.FromDouble(9.531E10).ToString());
      Assert.AreEqual("142.3799999999999954525264911353588104248046875", DecimalFraction.FromDouble(142.38).ToString());
      Assert.AreEqual("2252.5", DecimalFraction.FromDouble(2252.5).ToString());
      Assert.AreEqual("363600000000", DecimalFraction.FromDouble(3.636E11).ToString());
      Assert.AreEqual("0.00000323700000000000009386523676380154057596882921643555164337158203125", DecimalFraction.FromDouble(3.237E-6).ToString());
      Assert.AreEqual("728000", DecimalFraction.FromDouble(728000.0).ToString());
      Assert.AreEqual("25818000", DecimalFraction.FromDouble(2.5818E7).ToString());
      Assert.AreEqual("1090000", DecimalFraction.FromDouble(1090000.0).ToString());
      Assert.AreEqual("1.5509999999999999342747969421907328069210052490234375", DecimalFraction.FromDouble(1.551).ToString());
      Assert.AreEqual("26.035000000000000142108547152020037174224853515625", DecimalFraction.FromDouble(26.035).ToString());
      Assert.AreEqual("833000000", DecimalFraction.FromDouble(8.33E8).ToString());
      Assert.AreEqual("812300000000", DecimalFraction.FromDouble(8.123E11).ToString());
      Assert.AreEqual("2622.90000000000009094947017729282379150390625", DecimalFraction.FromDouble(2622.9).ToString());
      Assert.AreEqual("1.290999999999999925393012745189480483531951904296875", DecimalFraction.FromDouble(1.291).ToString());
      Assert.AreEqual("286140", DecimalFraction.FromDouble(286140.0).ToString());
      Assert.AreEqual("0.06733000000000000095923269327613525092601776123046875", DecimalFraction.FromDouble(0.06733).ToString());
      Assert.AreEqual("0.000325160000000000010654532811571471029310487210750579833984375", DecimalFraction.FromDouble(3.2516E-4).ToString());
      Assert.AreEqual("383230000", DecimalFraction.FromDouble(3.8323E8).ToString());
      Assert.AreEqual("0.02843299999999999994049204588009160943329334259033203125", DecimalFraction.FromDouble(0.028433).ToString());
      Assert.AreEqual("837000000", DecimalFraction.FromDouble(8.37E8).ToString());
      Assert.AreEqual("0.0160800000000000005428990590417015482671558856964111328125", DecimalFraction.FromDouble(0.01608).ToString());
      Assert.AreEqual("3621000000000", DecimalFraction.FromDouble(3.621E12).ToString());
      Assert.AreEqual("78.1200000000000045474735088646411895751953125", DecimalFraction.FromDouble(78.12).ToString());
      Assert.AreEqual("1308000000", DecimalFraction.FromDouble(1.308E9).ToString());
      Assert.AreEqual("0.031937000000000000110578213252665591426193714141845703125", DecimalFraction.FromDouble(0.031937).ToString());
      Assert.AreEqual("1581500", DecimalFraction.FromDouble(1581500.0).ToString());
      Assert.AreEqual("244200", DecimalFraction.FromDouble(244200.0).ToString());
      Assert.AreEqual("2.28179999999999995794237200343046456652018605382181704044342041015625E-7", DecimalFraction.FromDouble(2.2818E-7).ToString());
      Assert.AreEqual("39.73400000000000176214598468504846096038818359375", DecimalFraction.FromDouble(39.734).ToString());
      Assert.AreEqual("1614", DecimalFraction.FromDouble(1614.0).ToString());
      Assert.AreEqual("0.0003831899999999999954607143859419693399104289710521697998046875", DecimalFraction.FromDouble(3.8319E-4).ToString());
      Assert.AreEqual("543.3999999999999772626324556767940521240234375", DecimalFraction.FromDouble(543.4).ToString());
      Assert.AreEqual("319310000", DecimalFraction.FromDouble(3.1931E8).ToString());
      Assert.AreEqual("1429000", DecimalFraction.FromDouble(1429000.0).ToString());
      Assert.AreEqual("2653700000000", DecimalFraction.FromDouble(2.6537E12).ToString());
      Assert.AreEqual("722000000", DecimalFraction.FromDouble(7.22E8).ToString());
      Assert.AreEqual("27.199999999999999289457264239899814128875732421875", DecimalFraction.FromDouble(27.2).ToString());
      Assert.AreEqual("0.00000380250000000000001586513038998038638283105683512985706329345703125", DecimalFraction.FromDouble(3.8025E-6).ToString());
      Assert.AreEqual("0.0000364159999999999982843446044711299691698513925075531005859375", DecimalFraction.FromDouble(3.6416E-5).ToString());
      Assert.AreEqual("2006000", DecimalFraction.FromDouble(2006000.0).ToString());
      Assert.AreEqual("2681200000", DecimalFraction.FromDouble(2.6812E9).ToString());
      Assert.AreEqual("27534000000", DecimalFraction.FromDouble(2.7534E10).ToString());
      Assert.AreEqual("3.911600000000000165617541382501176627783934236504137516021728515625E-7", DecimalFraction.FromDouble(3.9116E-7).ToString());
      Assert.AreEqual("0.0028135000000000000286437540353290387429296970367431640625", DecimalFraction.FromDouble(0.0028135).ToString());
      Assert.AreEqual("0.91190000000000004387601393318618647754192352294921875", DecimalFraction.FromDouble(0.9119).ToString());
      Assert.AreEqual("2241200", DecimalFraction.FromDouble(2241200.0).ToString());
      Assert.AreEqual("32.4500000000000028421709430404007434844970703125", DecimalFraction.FromDouble(32.45).ToString());
      Assert.AreEqual("13800000000", DecimalFraction.FromDouble(1.38E10).ToString());
      Assert.AreEqual("0.047300000000000001765254609153998899273574352264404296875", DecimalFraction.FromDouble(0.0473).ToString());
      Assert.AreEqual("205.340000000000003410605131648480892181396484375", DecimalFraction.FromDouble(205.34).ToString());
      Assert.AreEqual("3.981899999999999995026200849679298698902130126953125", DecimalFraction.FromDouble(3.9819).ToString());
      Assert.AreEqual("1152.799999999999954525264911353588104248046875", DecimalFraction.FromDouble(1152.8).ToString());
      Assert.AreEqual("1322000", DecimalFraction.FromDouble(1322000.0).ToString());
      Assert.AreEqual("0.00013414000000000001334814203612921801322954706847667694091796875", DecimalFraction.FromDouble(1.3414E-4).ToString());
      Assert.AreEqual("3.4449999999999999446924077266263264363033158588223159313201904296875E-7", DecimalFraction.FromDouble(3.445E-7).ToString());
      Assert.AreEqual("1.3610000000000000771138253079228785935583800892345607280731201171875E-7", DecimalFraction.FromDouble(1.361E-7).ToString());
      Assert.AreEqual("26090000", DecimalFraction.FromDouble(2.609E7).ToString());
      Assert.AreEqual("9.93599999999999994315658113919198513031005859375", DecimalFraction.FromDouble(9.936).ToString());
      Assert.AreEqual("0.00000600000000000000015200514458246772164784488268196582794189453125", DecimalFraction.FromDouble(6.0E-6).ToString());
      Assert.AreEqual("260.31000000000000227373675443232059478759765625", DecimalFraction.FromDouble(260.31).ToString());
      Assert.AreEqual("344.6000000000000227373675443232059478759765625", DecimalFraction.FromDouble(344.6).ToString());
      Assert.AreEqual("3.423700000000000187583282240666449069976806640625", DecimalFraction.FromDouble(3.4237).ToString());
      Assert.AreEqual("2342100000", DecimalFraction.FromDouble(2.3421E9).ToString());
      Assert.AreEqual("0.00023310000000000000099260877295392901942250318825244903564453125", DecimalFraction.FromDouble(2.331E-4).ToString());
      Assert.AreEqual("0.7339999999999999857891452847979962825775146484375", DecimalFraction.FromDouble(0.734).ToString());
      Assert.AreEqual("0.01541499999999999988287147090204598498530685901641845703125", DecimalFraction.FromDouble(0.015415).ToString());
      Assert.AreEqual("0.0035311000000000001240729741169843691750429570674896240234375", DecimalFraction.FromDouble(0.0035311).ToString());
      Assert.AreEqual("1221700000000", DecimalFraction.FromDouble(1.2217E12).ToString());
      Assert.AreEqual("0.48299999999999998490096686509787105023860931396484375", DecimalFraction.FromDouble(0.483).ToString());
      Assert.AreEqual("0.0002871999999999999878506906636488338335766457021236419677734375", DecimalFraction.FromDouble(2.872E-4).ToString());
      Assert.AreEqual("96.1099999999999994315658113919198513031005859375", DecimalFraction.FromDouble(96.11).ToString());
      Assert.AreEqual("36570", DecimalFraction.FromDouble(36570.0).ToString());
      Assert.AreEqual("0.00001830000000000000097183545932910675446692039258778095245361328125", DecimalFraction.FromDouble(1.83E-5).ToString());
      Assert.AreEqual("301310000", DecimalFraction.FromDouble(3.0131E8).ToString());
      Assert.AreEqual("382200", DecimalFraction.FromDouble(382200.0).ToString());
      Assert.AreEqual("248350000", DecimalFraction.FromDouble(2.4835E8).ToString());
      Assert.AreEqual("0.0015839999999999999046040866090834242640994489192962646484375", DecimalFraction.FromDouble(0.001584).ToString());
      Assert.AreEqual("0.000761999999999999982035203682784185730270110070705413818359375", DecimalFraction.FromDouble(7.62E-4).ToString());
      Assert.AreEqual("313300000000", DecimalFraction.FromDouble(3.133E11).ToString());
    }


    [Test]
    public void ToPlainStringTest() {
      Assert.AreEqual("277220000000", DecimalFraction.FromString("277.22E9").ToPlainString());
      Assert.AreEqual("3911900", DecimalFraction.FromString("391.19E4").ToPlainString());
      Assert.AreEqual("0.00000038327", DecimalFraction.FromString("383.27E-9").ToPlainString());
      Assert.AreEqual("47330000000", DecimalFraction.FromString("47.33E9").ToPlainString());
      Assert.AreEqual("322210", DecimalFraction.FromString("322.21E3").ToPlainString());
      Assert.AreEqual("1.913", DecimalFraction.FromString("191.3E-2").ToPlainString());
      Assert.AreEqual("11917", DecimalFraction.FromString("119.17E2").ToPlainString());
      Assert.AreEqual("0.0001596", DecimalFraction.FromString("159.6E-6").ToPlainString());
      Assert.AreEqual("70160000000", DecimalFraction.FromString("70.16E9").ToPlainString());
      Assert.AreEqual("166240000000", DecimalFraction.FromString("166.24E9").ToPlainString());
      Assert.AreEqual("235250", DecimalFraction.FromString("235.25E3").ToPlainString());
      Assert.AreEqual("372200000", DecimalFraction.FromString("37.22E7").ToPlainString());
      Assert.AreEqual("32026000000", DecimalFraction.FromString("320.26E8").ToPlainString());
      Assert.AreEqual("0.00000012711", DecimalFraction.FromString("127.11E-9").ToPlainString());
      Assert.AreEqual("0.000009729", DecimalFraction.FromString("97.29E-7").ToPlainString());
      Assert.AreEqual("175130000000", DecimalFraction.FromString("175.13E9").ToPlainString());
      Assert.AreEqual("0.000003821", DecimalFraction.FromString("38.21E-7").ToPlainString());
      Assert.AreEqual("62.8", DecimalFraction.FromString("6.28E1").ToPlainString());
      Assert.AreEqual("138290000", DecimalFraction.FromString("138.29E6").ToPlainString());
      Assert.AreEqual("1601.9", DecimalFraction.FromString("160.19E1").ToPlainString());
      Assert.AreEqual("35812", DecimalFraction.FromString("358.12E2").ToPlainString());
      Assert.AreEqual("2492800000000", DecimalFraction.FromString("249.28E10").ToPlainString());
      Assert.AreEqual("0.00031123", DecimalFraction.FromString("311.23E-6").ToPlainString());
      Assert.AreEqual("0.16433", DecimalFraction.FromString("164.33E-3").ToPlainString());
      Assert.AreEqual("29.920", DecimalFraction.FromString("299.20E-1").ToPlainString());
      Assert.AreEqual("105390", DecimalFraction.FromString("105.39E3").ToPlainString());
      Assert.AreEqual("3825000", DecimalFraction.FromString("382.5E4").ToPlainString());
      Assert.AreEqual("909", DecimalFraction.FromString("90.9E1").ToPlainString());
      Assert.AreEqual("32915000000", DecimalFraction.FromString("329.15E8").ToPlainString());
      Assert.AreEqual("24523000000", DecimalFraction.FromString("245.23E8").ToPlainString());
      Assert.AreEqual("0.0000009719", DecimalFraction.FromString("97.19E-8").ToPlainString());
      Assert.AreEqual("551200000", DecimalFraction.FromString("55.12E7").ToPlainString());
      Assert.AreEqual("1238", DecimalFraction.FromString("12.38E2").ToPlainString());
      Assert.AreEqual("0.0025020", DecimalFraction.FromString("250.20E-5").ToPlainString());
      Assert.AreEqual("5320", DecimalFraction.FromString("53.20E2").ToPlainString());
      Assert.AreEqual("14150000000", DecimalFraction.FromString("141.5E8").ToPlainString());
      Assert.AreEqual("0.0033834", DecimalFraction.FromString("338.34E-5").ToPlainString());
      Assert.AreEqual("160390000000", DecimalFraction.FromString("160.39E9").ToPlainString());
      Assert.AreEqual("152170000", DecimalFraction.FromString("152.17E6").ToPlainString());
      Assert.AreEqual("13300000000", DecimalFraction.FromString("13.3E9").ToPlainString());
      Assert.AreEqual("13.8", DecimalFraction.FromString("1.38E1").ToPlainString());
      Assert.AreEqual("0.00000034821", DecimalFraction.FromString("348.21E-9").ToPlainString());
      Assert.AreEqual("525000000", DecimalFraction.FromString("52.5E7").ToPlainString());
      Assert.AreEqual("2152100000000", DecimalFraction.FromString("215.21E10").ToPlainString());
      Assert.AreEqual("234280000000", DecimalFraction.FromString("234.28E9").ToPlainString());
      Assert.AreEqual("310240000000", DecimalFraction.FromString("310.24E9").ToPlainString());
      Assert.AreEqual("345390000000", DecimalFraction.FromString("345.39E9").ToPlainString());
      Assert.AreEqual("0.00000011638", DecimalFraction.FromString("116.38E-9").ToPlainString());
      Assert.AreEqual("2762500000000", DecimalFraction.FromString("276.25E10").ToPlainString());
      Assert.AreEqual("0.0000015832", DecimalFraction.FromString("158.32E-8").ToPlainString());
      Assert.AreEqual("27250", DecimalFraction.FromString("272.5E2").ToPlainString());
      Assert.AreEqual("0.00000038933", DecimalFraction.FromString("389.33E-9").ToPlainString());
      Assert.AreEqual("3811500000", DecimalFraction.FromString("381.15E7").ToPlainString());
      Assert.AreEqual("280000", DecimalFraction.FromString("280.0E3").ToPlainString());
      Assert.AreEqual("0.0002742", DecimalFraction.FromString("274.2E-6").ToPlainString());
      Assert.AreEqual("0.000038714", DecimalFraction.FromString("387.14E-7").ToPlainString());
      Assert.AreEqual("0.00002277", DecimalFraction.FromString("227.7E-7").ToPlainString());
      Assert.AreEqual("20121", DecimalFraction.FromString("201.21E2").ToPlainString());
      Assert.AreEqual("255400", DecimalFraction.FromString("255.4E3").ToPlainString());
      Assert.AreEqual("0.000018727", DecimalFraction.FromString("187.27E-7").ToPlainString());
      Assert.AreEqual("0.01697", DecimalFraction.FromString("169.7E-4").ToPlainString());
      Assert.AreEqual("69900000000", DecimalFraction.FromString("69.9E9").ToPlainString());
      Assert.AreEqual("0.0320", DecimalFraction.FromString("3.20E-2").ToPlainString());
      Assert.AreEqual("23630", DecimalFraction.FromString("236.30E2").ToPlainString());
      Assert.AreEqual("0.00000022022", DecimalFraction.FromString("220.22E-9").ToPlainString());
      Assert.AreEqual("28.730", DecimalFraction.FromString("287.30E-1").ToPlainString());
      Assert.AreEqual("0.0000001563", DecimalFraction.FromString("156.3E-9").ToPlainString());
      Assert.AreEqual("13.623", DecimalFraction.FromString("136.23E-1").ToPlainString());
      Assert.AreEqual("12527000000", DecimalFraction.FromString("125.27E8").ToPlainString());
      Assert.AreEqual("0.000018030", DecimalFraction.FromString("180.30E-7").ToPlainString());
      Assert.AreEqual("3515000000", DecimalFraction.FromString("351.5E7").ToPlainString());
      Assert.AreEqual("28280000000", DecimalFraction.FromString("28.28E9").ToPlainString());
      Assert.AreEqual("0.2884", DecimalFraction.FromString("288.4E-3").ToPlainString());
      Assert.AreEqual("122200", DecimalFraction.FromString("12.22E4").ToPlainString());
      Assert.AreEqual("0.002575", DecimalFraction.FromString("257.5E-5").ToPlainString());
      Assert.AreEqual("389200", DecimalFraction.FromString("389.20E3").ToPlainString());
      Assert.AreEqual("0.03949", DecimalFraction.FromString("394.9E-4").ToPlainString());
      Assert.AreEqual("0.000013426", DecimalFraction.FromString("134.26E-7").ToPlainString());
      Assert.AreEqual("5829000", DecimalFraction.FromString("58.29E5").ToPlainString());
      Assert.AreEqual("0.000885", DecimalFraction.FromString("88.5E-5").ToPlainString());
      Assert.AreEqual("0.019329", DecimalFraction.FromString("193.29E-4").ToPlainString());
      Assert.AreEqual("713500000000", DecimalFraction.FromString("71.35E10").ToPlainString());
      Assert.AreEqual("2520", DecimalFraction.FromString("252.0E1").ToPlainString());
      Assert.AreEqual("0.000000532", DecimalFraction.FromString("53.2E-8").ToPlainString());
      Assert.AreEqual("18.120", DecimalFraction.FromString("181.20E-1").ToPlainString());
      Assert.AreEqual("0.00000005521", DecimalFraction.FromString("55.21E-9").ToPlainString());
      Assert.AreEqual("57.31", DecimalFraction.FromString("57.31E0").ToPlainString());
      Assert.AreEqual("0.00000011313", DecimalFraction.FromString("113.13E-9").ToPlainString());
      Assert.AreEqual("532.3", DecimalFraction.FromString("53.23E1").ToPlainString());
      Assert.AreEqual("0.000036837", DecimalFraction.FromString("368.37E-7").ToPlainString());
      Assert.AreEqual("0.01874", DecimalFraction.FromString("187.4E-4").ToPlainString());
      Assert.AreEqual("526000000", DecimalFraction.FromString("5.26E8").ToPlainString());
      Assert.AreEqual("3083200", DecimalFraction.FromString("308.32E4").ToPlainString());
      Assert.AreEqual("0.7615", DecimalFraction.FromString("76.15E-2").ToPlainString());
      Assert.AreEqual("1173800000", DecimalFraction.FromString("117.38E7").ToPlainString());
      Assert.AreEqual("0.001537", DecimalFraction.FromString("15.37E-4").ToPlainString());
      Assert.AreEqual("145.3", DecimalFraction.FromString("145.3E0").ToPlainString());
      Assert.AreEqual("22629000000", DecimalFraction.FromString("226.29E8").ToPlainString());
      Assert.AreEqual("2242600000000", DecimalFraction.FromString("224.26E10").ToPlainString());
      Assert.AreEqual("0.00000026818", DecimalFraction.FromString("268.18E-9").ToPlainString());
    }

    [Test]
    public void ToEngineeringStringTest() {
      Assert.AreEqual("8.912", DecimalFraction.FromString("89.12E-1").ToEngineeringString());
      Assert.AreEqual("0.024231", DecimalFraction.FromString("242.31E-4").ToEngineeringString());
      Assert.AreEqual("22.918E+6", DecimalFraction.FromString("229.18E5").ToEngineeringString());
      Assert.AreEqual("0.000032618", DecimalFraction.FromString("326.18E-7").ToEngineeringString());
      Assert.AreEqual("55.0E+6", DecimalFraction.FromString("55.0E6").ToEngineeringString());
      Assert.AreEqual("224.36E+3", DecimalFraction.FromString("224.36E3").ToEngineeringString());
      Assert.AreEqual("230.12E+9", DecimalFraction.FromString("230.12E9").ToEngineeringString());
      Assert.AreEqual("0.000011320", DecimalFraction.FromString("113.20E-7").ToEngineeringString());
      Assert.AreEqual("317.7E-9", DecimalFraction.FromString("317.7E-9").ToEngineeringString());
      Assert.AreEqual("3.393", DecimalFraction.FromString("339.3E-2").ToEngineeringString());
      Assert.AreEqual("27.135E+9", DecimalFraction.FromString("271.35E8").ToEngineeringString());
      Assert.AreEqual("377.19E-9", DecimalFraction.FromString("377.19E-9").ToEngineeringString());
      Assert.AreEqual("3.2127E+9", DecimalFraction.FromString("321.27E7").ToEngineeringString());
      Assert.AreEqual("2.9422", DecimalFraction.FromString("294.22E-2").ToEngineeringString());
      Assert.AreEqual("0.0000011031", DecimalFraction.FromString("110.31E-8").ToEngineeringString());
      Assert.AreEqual("2.4324", DecimalFraction.FromString("243.24E-2").ToEngineeringString());
      Assert.AreEqual("0.0006412", DecimalFraction.FromString("64.12E-5").ToEngineeringString());
      Assert.AreEqual("1422.3", DecimalFraction.FromString("142.23E1").ToEngineeringString());
      Assert.AreEqual("293.0", DecimalFraction.FromString("293.0E0").ToEngineeringString());
      Assert.AreEqual("0.0000025320", DecimalFraction.FromString("253.20E-8").ToEngineeringString());
      Assert.AreEqual("36.66E+9", DecimalFraction.FromString("366.6E8").ToEngineeringString());
      Assert.AreEqual("3.4526E+12", DecimalFraction.FromString("345.26E10").ToEngineeringString());
      Assert.AreEqual("2.704", DecimalFraction.FromString("270.4E-2").ToEngineeringString());
      Assert.AreEqual("432E+6", DecimalFraction.FromString("4.32E8").ToEngineeringString());
      Assert.AreEqual("224.22", DecimalFraction.FromString("224.22E0").ToEngineeringString());
      Assert.AreEqual("0.000031530", DecimalFraction.FromString("315.30E-7").ToEngineeringString());
      Assert.AreEqual("11.532E+6", DecimalFraction.FromString("115.32E5").ToEngineeringString());
      Assert.AreEqual("39420", DecimalFraction.FromString("394.20E2").ToEngineeringString());
      Assert.AreEqual("67.24E-9", DecimalFraction.FromString("67.24E-9").ToEngineeringString());
      Assert.AreEqual("34933", DecimalFraction.FromString("349.33E2").ToEngineeringString());
      Assert.AreEqual("67.8E-9", DecimalFraction.FromString("67.8E-9").ToEngineeringString());
      Assert.AreEqual("19.231E+6", DecimalFraction.FromString("192.31E5").ToEngineeringString());
      Assert.AreEqual("1.7317E+9", DecimalFraction.FromString("173.17E7").ToEngineeringString());
      Assert.AreEqual("43.9", DecimalFraction.FromString("43.9E0").ToEngineeringString());
      Assert.AreEqual("0.0000016812", DecimalFraction.FromString("168.12E-8").ToEngineeringString());
      Assert.AreEqual("3.715E+12", DecimalFraction.FromString("371.5E10").ToEngineeringString());
      Assert.AreEqual("424E-9", DecimalFraction.FromString("42.4E-8").ToEngineeringString());
      Assert.AreEqual("1.6123E+12", DecimalFraction.FromString("161.23E10").ToEngineeringString());
      Assert.AreEqual("302.8E+6", DecimalFraction.FromString("302.8E6").ToEngineeringString());
      Assert.AreEqual("175.13", DecimalFraction.FromString("175.13E0").ToEngineeringString());
      Assert.AreEqual("298.20E-9", DecimalFraction.FromString("298.20E-9").ToEngineeringString());
      Assert.AreEqual("36.223E+9", DecimalFraction.FromString("362.23E8").ToEngineeringString());
      Assert.AreEqual("27739", DecimalFraction.FromString("277.39E2").ToEngineeringString());
      Assert.AreEqual("0.011734", DecimalFraction.FromString("117.34E-4").ToEngineeringString());
      Assert.AreEqual("190.13E-9", DecimalFraction.FromString("190.13E-9").ToEngineeringString());
      Assert.AreEqual("3.5019", DecimalFraction.FromString("350.19E-2").ToEngineeringString());
      Assert.AreEqual("383.27E-9", DecimalFraction.FromString("383.27E-9").ToEngineeringString());
      Assert.AreEqual("24.217E+6", DecimalFraction.FromString("242.17E5").ToEngineeringString());
      Assert.AreEqual("2.9923E+9", DecimalFraction.FromString("299.23E7").ToEngineeringString());
      Assert.AreEqual("3.0222", DecimalFraction.FromString("302.22E-2").ToEngineeringString());
      Assert.AreEqual("0.04521", DecimalFraction.FromString("45.21E-3").ToEngineeringString());
      Assert.AreEqual("15.00", DecimalFraction.FromString("150.0E-1").ToEngineeringString());
      Assert.AreEqual("290E+3", DecimalFraction.FromString("29.0E4").ToEngineeringString());
      Assert.AreEqual("263.37E+3", DecimalFraction.FromString("263.37E3").ToEngineeringString());
      Assert.AreEqual("28.321", DecimalFraction.FromString("283.21E-1").ToEngineeringString());
      Assert.AreEqual("21.32", DecimalFraction.FromString("21.32E0").ToEngineeringString());
      Assert.AreEqual("0.00006920", DecimalFraction.FromString("69.20E-6").ToEngineeringString());
      Assert.AreEqual("0.0728", DecimalFraction.FromString("72.8E-3").ToEngineeringString());
      Assert.AreEqual("1.646E+9", DecimalFraction.FromString("164.6E7").ToEngineeringString());
      Assert.AreEqual("1.1817", DecimalFraction.FromString("118.17E-2").ToEngineeringString());
      Assert.AreEqual("0.000026235", DecimalFraction.FromString("262.35E-7").ToEngineeringString());
      Assert.AreEqual("23.37E+6", DecimalFraction.FromString("233.7E5").ToEngineeringString());
      Assert.AreEqual("391.24", DecimalFraction.FromString("391.24E0").ToEngineeringString());
      Assert.AreEqual("2213.6", DecimalFraction.FromString("221.36E1").ToEngineeringString());
      Assert.AreEqual("353.32", DecimalFraction.FromString("353.32E0").ToEngineeringString());
      Assert.AreEqual("0.012931", DecimalFraction.FromString("129.31E-4").ToEngineeringString());
      Assert.AreEqual("0.0017626", DecimalFraction.FromString("176.26E-5").ToEngineeringString());
      Assert.AreEqual("207.5E+3", DecimalFraction.FromString("207.5E3").ToEngineeringString());
      Assert.AreEqual("314.10", DecimalFraction.FromString("314.10E0").ToEngineeringString());
      Assert.AreEqual("379.20E+9", DecimalFraction.FromString("379.20E9").ToEngineeringString());
      Assert.AreEqual("0.00037912", DecimalFraction.FromString("379.12E-6").ToEngineeringString());
      Assert.AreEqual("743.8E-9", DecimalFraction.FromString("74.38E-8").ToEngineeringString());
      Assert.AreEqual("234.17E-9", DecimalFraction.FromString("234.17E-9").ToEngineeringString());
      Assert.AreEqual("132.6E+6", DecimalFraction.FromString("13.26E7").ToEngineeringString());
      Assert.AreEqual("25.15E+6", DecimalFraction.FromString("251.5E5").ToEngineeringString());
      Assert.AreEqual("87.32", DecimalFraction.FromString("87.32E0").ToEngineeringString());
      Assert.AreEqual("3.3116E+9", DecimalFraction.FromString("331.16E7").ToEngineeringString());
      Assert.AreEqual("6.14E+9", DecimalFraction.FromString("61.4E8").ToEngineeringString());
      Assert.AreEqual("0.0002097", DecimalFraction.FromString("209.7E-6").ToEngineeringString());
      Assert.AreEqual("5.4E+6", DecimalFraction.FromString("5.4E6").ToEngineeringString());
      Assert.AreEqual("219.9", DecimalFraction.FromString("219.9E0").ToEngineeringString());
      Assert.AreEqual("0.00002631", DecimalFraction.FromString("26.31E-6").ToEngineeringString());
      Assert.AreEqual("482.8E+6", DecimalFraction.FromString("48.28E7").ToEngineeringString());
      Assert.AreEqual("267.8", DecimalFraction.FromString("267.8E0").ToEngineeringString());
      Assert.AreEqual("0.3209", DecimalFraction.FromString("320.9E-3").ToEngineeringString());
      Assert.AreEqual("0.30015", DecimalFraction.FromString("300.15E-3").ToEngineeringString());
      Assert.AreEqual("2.6011E+6", DecimalFraction.FromString("260.11E4").ToEngineeringString());
      Assert.AreEqual("1.1429", DecimalFraction.FromString("114.29E-2").ToEngineeringString());
      Assert.AreEqual("0.0003060", DecimalFraction.FromString("306.0E-6").ToEngineeringString());
      Assert.AreEqual("97.7E+3", DecimalFraction.FromString("97.7E3").ToEngineeringString());
      Assert.AreEqual("12.229E+9", DecimalFraction.FromString("122.29E8").ToEngineeringString());
      Assert.AreEqual("6.94E+3", DecimalFraction.FromString("69.4E2").ToEngineeringString());
      Assert.AreEqual("383.5", DecimalFraction.FromString("383.5E0").ToEngineeringString());
      Assert.AreEqual("315.30E+3", DecimalFraction.FromString("315.30E3").ToEngineeringString());
      Assert.AreEqual("130.38E+9", DecimalFraction.FromString("130.38E9").ToEngineeringString());
      Assert.AreEqual("206.16E+9", DecimalFraction.FromString("206.16E9").ToEngineeringString());
      Assert.AreEqual("304.28E-9", DecimalFraction.FromString("304.28E-9").ToEngineeringString());
      Assert.AreEqual("661.3E+3", DecimalFraction.FromString("66.13E4").ToEngineeringString());
      Assert.AreEqual("1.8533", DecimalFraction.FromString("185.33E-2").ToEngineeringString());
      Assert.AreEqual("70.7E+6", DecimalFraction.FromString("70.7E6").ToEngineeringString());
    }

    [Test]
    public void TestDecimalsEquivalent() {
      AssertDecimalsEquivalent("1.310E-7", "131.0E-9");
      AssertDecimalsEquivalent("0.001231", "123.1E-5");
      AssertDecimalsEquivalent("3.0324E+6", "303.24E4");
      AssertDecimalsEquivalent("3.726E+8", "372.6E6");
      AssertDecimalsEquivalent("2663.6", "266.36E1");
      AssertDecimalsEquivalent("34.24", "342.4E-1");
      AssertDecimalsEquivalent("3492.5", "349.25E1");
      AssertDecimalsEquivalent("0.31919", "319.19E-3");
      AssertDecimalsEquivalent("2.936E-7", "293.6E-9");
      AssertDecimalsEquivalent("6.735E+10", "67.35E9");
      AssertDecimalsEquivalent("7.39E+10", "7.39E10");
      AssertDecimalsEquivalent("0.0020239", "202.39E-5");
      AssertDecimalsEquivalent("1.6717E+6", "167.17E4");
      AssertDecimalsEquivalent("1.7632E+9", "176.32E7");
      AssertDecimalsEquivalent("39.526", "395.26E-1");
      AssertDecimalsEquivalent("0.002939", "29.39E-4");
      AssertDecimalsEquivalent("0.3165", "316.5E-3");
      AssertDecimalsEquivalent("3.7910E-7", "379.10E-9");
      AssertDecimalsEquivalent("0.000016035", "160.35E-7");
      AssertDecimalsEquivalent("0.001417", "141.7E-5");
      AssertDecimalsEquivalent("7.337E+5", "73.37E4");
      AssertDecimalsEquivalent("3.4232E+12", "342.32E10");
      AssertDecimalsEquivalent("2.828E+8", "282.8E6");
      AssertDecimalsEquivalent("4.822E-7", "48.22E-8");
      AssertDecimalsEquivalent("2.6328E+9", "263.28E7");
      AssertDecimalsEquivalent("2.9911E+8", "299.11E6");
      AssertDecimalsEquivalent("3.636E+9", "36.36E8");
      AssertDecimalsEquivalent("0.20031", "200.31E-3");
      AssertDecimalsEquivalent("1.922E+7", "19.22E6");
      AssertDecimalsEquivalent("3.0924E+8", "309.24E6");
      AssertDecimalsEquivalent("2.7236E+7", "272.36E5");
      AssertDecimalsEquivalent("0.01645", "164.5E-4");
      AssertDecimalsEquivalent("0.000292", "29.2E-5");
      AssertDecimalsEquivalent("1.9939", "199.39E-2");
      AssertDecimalsEquivalent("2.7929E+9", "279.29E7");
      AssertDecimalsEquivalent("1.213E+7", "121.3E5");
      AssertDecimalsEquivalent("2.765E+6", "276.5E4");
      AssertDecimalsEquivalent("270.11", "270.11E0");
      AssertDecimalsEquivalent("0.017718", "177.18E-4");
      AssertDecimalsEquivalent("0.003607", "360.7E-5");
      AssertDecimalsEquivalent("0.00038618", "386.18E-6");
      AssertDecimalsEquivalent("0.0004230", "42.30E-5");
      AssertDecimalsEquivalent("1.8410E+5", "184.10E3");
      AssertDecimalsEquivalent("0.00030427", "304.27E-6");
      AssertDecimalsEquivalent("6.513E+6", "65.13E5");
      AssertDecimalsEquivalent("0.06717", "67.17E-3");
      AssertDecimalsEquivalent("0.00031123", "311.23E-6");
      AssertDecimalsEquivalent("0.0031639", "316.39E-5");
      AssertDecimalsEquivalent("1.146E+5", "114.6E3");
      AssertDecimalsEquivalent("0.00039937", "399.37E-6");
      AssertDecimalsEquivalent("3.3817", "338.17E-2");
      AssertDecimalsEquivalent("0.00011128", "111.28E-6");
      AssertDecimalsEquivalent("7.818E+7", "78.18E6");
      AssertDecimalsEquivalent("2.6417E-7", "264.17E-9");
      AssertDecimalsEquivalent("1.852E+9", "185.2E7");
      AssertDecimalsEquivalent("0.0016216", "162.16E-5");
      AssertDecimalsEquivalent("2.2813E+6", "228.13E4");
      AssertDecimalsEquivalent("3.078E+12", "307.8E10");
      AssertDecimalsEquivalent("0.00002235", "22.35E-6");
      AssertDecimalsEquivalent("0.0032827", "328.27E-5");
      AssertDecimalsEquivalent("1.334E+9", "133.4E7");
      AssertDecimalsEquivalent("34.022", "340.22E-1");
      AssertDecimalsEquivalent("7.19E+6", "7.19E6");
      AssertDecimalsEquivalent("35.311", "353.11E-1");
      AssertDecimalsEquivalent("3.4330E+6", "343.30E4");
      AssertDecimalsEquivalent("0.000022923", "229.23E-7");
      AssertDecimalsEquivalent("2.899E+4", "289.9E2");
      AssertDecimalsEquivalent("0.00031", "3.1E-4");
      AssertDecimalsEquivalent("2.0418E+5", "204.18E3");
      AssertDecimalsEquivalent("3.3412E+11", "334.12E9");
      AssertDecimalsEquivalent("1.717E+10", "171.7E8");
      AssertDecimalsEquivalent("2.7024E+10", "270.24E8");
      AssertDecimalsEquivalent("1.0219E+9", "102.19E7");
      AssertDecimalsEquivalent("15.13", "151.3E-1");
      AssertDecimalsEquivalent("91.23", "91.23E0");
      AssertDecimalsEquivalent("3.4114E+6", "341.14E4");
      AssertDecimalsEquivalent("33.832", "338.32E-1");
      AssertDecimalsEquivalent("0.19234", "192.34E-3");
      AssertDecimalsEquivalent("16835", "168.35E2");
      AssertDecimalsEquivalent("0.00038610", "386.10E-6");
      AssertDecimalsEquivalent("1.6624E+9", "166.24E7");
      AssertDecimalsEquivalent("2.351E+9", "235.1E7");
      AssertDecimalsEquivalent("0.03084", "308.4E-4");
      AssertDecimalsEquivalent("0.00429", "42.9E-4");
      AssertDecimalsEquivalent("9.718E-8", "97.18E-9");
      AssertDecimalsEquivalent("0.00003121", "312.1E-7");
      AssertDecimalsEquivalent("3.175E+4", "317.5E2");
      AssertDecimalsEquivalent("376.6", "376.6E0");
      AssertDecimalsEquivalent("0.0000026110", "261.10E-8");
      AssertDecimalsEquivalent("7.020E+11", "70.20E10");
      AssertDecimalsEquivalent("2.1533E+9", "215.33E7");
      AssertDecimalsEquivalent("3.8113E+7", "381.13E5");
      AssertDecimalsEquivalent("7.531", "75.31E-1");
      AssertDecimalsEquivalent("991.0", "99.10E1");
      AssertDecimalsEquivalent("2.897E+8", "289.7E6");
      AssertDecimalsEquivalent("0.0000033211", "332.11E-8");
      AssertDecimalsEquivalent("0.03169", "316.9E-4");
      AssertDecimalsEquivalent("2.7321E+12", "273.21E10");
      AssertDecimalsEquivalent("394.38", "394.38E0");
      AssertDecimalsEquivalent("5.912E+7", "59.12E6");
    }
    
    [Test]
    public void TestSubtractNonFinite(){
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NaN), CBORObject.FromObject(99.74439f)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NaN), CBORObject.FromObject(0.04503661680757691d)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NaN), CBORObject.FromObject(DecimalFraction.FromString("961.056025725133"))).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NaN), CBORObject.FromObject(-2.66673f)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NaN), CBORObject.FromObject(-3249200021658530613L)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.PositiveInfinity), CBORObject.FromObject(-3082676751896642153L)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.PositiveInfinity), CBORObject.FromObject(0.37447542485458996d)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.PositiveInfinity), CBORObject.FromObject(DecimalFraction.FromString("6695270"))).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.PositiveInfinity), CBORObject.FromObject(8.645616f)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.PositiveInfinity), CBORObject.FromObject(10.918599534632621d)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NegativeInfinity), CBORObject.FromObject(1.1195766122143437E-7d)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NegativeInfinity), CBORObject.FromObject(-27.678854f)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NegativeInfinity), CBORObject.FromObject(DecimalFraction.FromString("51444344646435.890"))).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NegativeInfinity), CBORObject.FromObject(DecimalFraction.FromString("-795755897.41124405443"))).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NegativeInfinity), CBORObject.FromObject(DecimalFraction.FromString("282349190160173.8945458982215192141"))).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Single.NaN), CBORObject.FromObject(-4742894673080640195L)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Single.NaN), CBORObject.FromObject(-8.057984695058738E-10d)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Single.NaN), CBORObject.FromObject(-6832707275063219586L)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Single.NaN), CBORObject.FromObject(BigInteger.Parse("3037587108614072", NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Single.NaN), CBORObject.FromObject(DecimalFraction.FromString("-21687"))).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Single.PositiveInfinity), CBORObject.FromObject(21.02954f)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Single.PositiveInfinity), CBORObject.FromObject(-280.74258f)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Single.PositiveInfinity), CBORObject.FromObject(3.295564645540288E-15d)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Single.PositiveInfinity), CBORObject.FromObject(-1.8643148756498468E-14d)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Single.PositiveInfinity), CBORObject.FromObject(DecimalFraction.FromString("56E-9"))).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Single.NegativeInfinity), CBORObject.FromObject(BigInteger.Parse("06842884252556766213171069781", NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Single.NegativeInfinity), CBORObject.FromObject(-6381263349646471084L)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Single.NegativeInfinity), CBORObject.FromObject(9127378784365184230L)).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Single.NegativeInfinity), CBORObject.FromObject(BigInteger.Parse("300921783316", NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Single.NegativeInfinity), CBORObject.FromObject(BigInteger.Parse("-5806763724610384900094490266237212718", NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
    }

    [Test]
    public void TestAsByte(){
      for(int i=0;i<255;i++){
        Assert.AreEqual((byte)i,CBORObject.FromObject(i).AsByte());
      }
      for(int i=-200;i<0;i++){
        try { CBORObject.FromObject(i).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      }
      for(int i=256;i<512;i++){
        try { CBORObject.FromObject(i).AsByte(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      }
    }

    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestByteStringStreamNoIndefiniteWithinDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0x5F, 0x41, 0x20, 0x5F, 0x41, 0x20, 0xFF, 0xFF });
    }


    [Test]
    public void TestBigFloatDecFrac() {
      BigFloat bf;
      bf = new BigFloat(20);
      Assert.AreEqual("20", DecimalFraction.FromBigFloat(bf).ToString());
      bf = new BigFloat((BigInteger)3, -1);
      Assert.AreEqual("1.5", DecimalFraction.FromBigFloat(bf).ToString());
      bf = new BigFloat((BigInteger)(-3), -1);
      Assert.AreEqual("-1.5", DecimalFraction.FromBigFloat(bf).ToString());
      DecimalFraction df;
      df = new DecimalFraction(20);
      Assert.AreEqual("20", BigFloat.FromDecimalFraction(df).ToString());
      df = new DecimalFraction(-20);
      Assert.AreEqual("-20", BigFloat.FromDecimalFraction(df).ToString());
      df = new DecimalFraction((BigInteger)15, -1);
      Assert.AreEqual("1.5", BigFloat.FromDecimalFraction(df).ToString());
      df = new DecimalFraction((BigInteger)(-15), -1);
      Assert.AreEqual("-1.5", BigFloat.FromDecimalFraction(df).ToString());
    }

    [Test]
    public void TestDecimalFrac() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0xc4, 0x82, 0x3, 0x1a, 1, 2, 3, 4 });
    }
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestDecimalFracExponentMustNotBeBignum() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0xc4, 0x82, 0xc2, 0x41, 1, 0x1a, 1, 2, 3, 4 });
    }

    [Test]
    public void TestDoubleToOther() {
      CBORObject dbl1 = CBORObject.FromObject((double)Int32.MinValue);
      CBORObject dbl2 = CBORObject.FromObject((double)Int32.MaxValue);
      try { dbl1.AsInt16(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { dbl1.AsByte(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { dbl1.AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { dbl1.AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { dbl1.AsBigInteger(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { dbl2.AsInt16(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { dbl2.AsByte(); } catch (OverflowException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { dbl2.AsInt32(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { dbl2.AsInt64(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { dbl2.AsBigInteger(); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
    }

    [Test]
    public void TestBigTag() {
      CBORObject.FromObjectAndTag(CBORObject.Null, (BigInteger)UInt64.MaxValue);
    }

    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestDecimalFracExactlyTwoElements() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0xc4, 0x82, 0xc2, 0x41, 1 });
    }
    [Test]
    public void TestDecimalFracMantissaMayBeBignum() {
      TestCommon.FromBytesTestAB(
        new byte[] { 0xc4, 0x82, 0x3, 0xc2, 0x41, 1 });
    }

    [Test]
    public void TestShort() {
      for (int i = Int16.MinValue; i <= Int16.MaxValue; i++) {
        TestCommon.AssertSer(
          CBORObject.FromObject((short)i),
          String.Format(CultureInfo.InvariantCulture, "{0}", i));
      }
    }

    [Test]
    public void TestByteArray() {
      TestCommon.AssertSer(
        CBORObject.FromObject(new byte[] { 0x20, 0x78 }), "h'2078'");
    }
    [Test]
    public void TestBigInteger() {
      BigInteger bi = (BigInteger)3;
      BigInteger negseven = (BigInteger)(-7);
      for (int i = 0; i < 500; i++) {
        TestCommon.AssertSer(
          CBORObject.FromObject(bi),
          String.Format(CultureInfo.InvariantCulture, "{0}", bi));
        bi *= (BigInteger)negseven;
      }
      BigInteger[] ranges = new BigInteger[]{
        (BigInteger)Int64.MinValue-(BigInteger)512,
        (BigInteger)Int64.MinValue+(BigInteger)512,
        (BigInteger)UInt64.MinValue-(BigInteger)512,
        (BigInteger)UInt64.MinValue+(BigInteger)512,
        (BigInteger)Int64.MaxValue-(BigInteger)512,
        (BigInteger)Int64.MaxValue+(BigInteger)512,
        (BigInteger)UInt64.MaxValue-(BigInteger)512,
        (BigInteger)UInt64.MaxValue+(BigInteger)512,
      };
      for (int i = 0; i < ranges.Length; i += 2) {
        BigInteger bigintTemp = ranges[i];
        while (true) {
          TestCommon.AssertSer(
            CBORObject.FromObject(bigintTemp),
            String.Format(CultureInfo.InvariantCulture, "{0}", bigintTemp));
          if (bigintTemp.Equals(ranges[i + 1])) break;
          bigintTemp = bigintTemp + BigInteger.One;
        }
      }
    }
    [Test]
    public void TestLong() {
      long[] ranges = new long[]{
        -65539,65539,
        0xFFFFF000L,0x100000400L,
        Int64.MaxValue-1000,Int64.MaxValue,
        Int64.MinValue,Int64.MinValue+1000
      };
      for (int i = 0; i < ranges.Length; i += 2) {
        long j = ranges[i];
        while (true) {
          TestCommon.AssertSer(
            CBORObject.FromObject(j),
            String.Format(CultureInfo.InvariantCulture, "{0}", j));
          Assert.AreEqual(
            CBORObject.FromObject(j),
            CBORObject.FromObject((BigInteger)j));
          CBORObject obj = CBORObject.FromJSONString(
            String.Format(CultureInfo.InvariantCulture, "[{0}]", j));
          TestCommon.AssertSer(obj,
                               String.Format(CultureInfo.InvariantCulture, "[{0}]", j));
          if (j == ranges[i + 1]) break;
          j++;
        }
      }
    }

    [Test]
    public void TestFloat() {
      TestCommon.AssertSer(CBORObject.FromObject(Single.PositiveInfinity),
                           "Infinity");
      TestCommon.AssertSer(CBORObject.FromObject(Single.NegativeInfinity),
                           "-Infinity");
      TestCommon.AssertSer(CBORObject.FromObject(Single.NaN),
                           "NaN");
      for (int i = -65539; i <= 65539; i++) {
        TestCommon.AssertSer(
          CBORObject.FromObject((float)i),
          String.Format(CultureInfo.InvariantCulture, "{0}", i));
      }
    }

    [Test]
    public void TestCodePointCompare() {
      Assert.AreEqual(0, Math.Sign(CBORDataUtilities.CodePointCompare("abc", "abc")));
      Assert.AreEqual(0, Math.Sign(CBORDataUtilities.CodePointCompare("\ud800\udc00", "\ud800\udc00")));
      Assert.AreEqual(-1, Math.Sign(CBORDataUtilities.CodePointCompare("abc", "\ud800\udc00")));
      Assert.AreEqual(-1, Math.Sign(CBORDataUtilities.CodePointCompare("\uf000", "\ud800\udc00")));
      Assert.AreEqual(1, Math.Sign(CBORDataUtilities.CodePointCompare("\uf000", "\ud800")));
    }

    [Test]
    public void TestSimpleValues() {
      TestCommon.AssertSer(CBORObject.FromObject(true),
                           "true");
      TestCommon.AssertSer(CBORObject.FromObject(false),
                           "false");
      TestCommon.AssertSer(CBORObject.FromObject((Object)null),
                           "null");
    }

    [Test]
    public void TestGetUtf8Length() {
      try { CBORDataUtilities.GetUtf8Length(null, true); } catch (ArgumentNullException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      try { CBORDataUtilities.GetUtf8Length(null, false); } catch (ArgumentNullException) { } catch (Exception ex) { Assert.Fail(ex.ToString()); }
      Assert.AreEqual(3, CBORDataUtilities.GetUtf8Length("abc", true));
      Assert.AreEqual(4, CBORDataUtilities.GetUtf8Length("\u0300\u0300", true));
      Assert.AreEqual(6, CBORDataUtilities.GetUtf8Length("\u3000\u3000", true));
      Assert.AreEqual(6, CBORDataUtilities.GetUtf8Length("\ud800\ud800", true));
      Assert.AreEqual(-1, CBORDataUtilities.GetUtf8Length("\ud800\ud800", false));
    }

    [Test]
    public void TestDouble() {
      TestCommon.AssertSer(CBORObject.FromObject(Double.PositiveInfinity),
                           "Infinity");
      TestCommon.AssertSer(CBORObject.FromObject(Double.NegativeInfinity),
                           "-Infinity");
      TestCommon.AssertSer(CBORObject.FromObject(Double.NaN),
                           "NaN");
      CBORObject oldobj = null;
      for (int i = -65539; i <= 65539; i++) {
        CBORObject o = CBORObject.FromObject((double)i);
        TestCommon.AssertSer(o,
                             String.Format(CultureInfo.InvariantCulture, "{0}", i));
        if (oldobj != null) {
          if(1!=o.CompareTo(oldobj))
            Assert.AreEqual(1, o.CompareTo(oldobj));
          if(-1!=oldobj.CompareTo(o))
            Assert.AreEqual(-1, oldobj.CompareTo(o));
        }
        oldobj = o;
      }
    }


    [Test]
    public void TestTags() {
      BigInteger maxuint = (BigInteger)UInt64.MaxValue;
      BigInteger[] ranges = new BigInteger[]{
        (BigInteger)6,
        (BigInteger)65539,
        (BigInteger)Int32.MaxValue-(BigInteger)500,
        (BigInteger)Int32.MaxValue+(BigInteger)500,
        (BigInteger)Int64.MaxValue-(BigInteger)500,
        (BigInteger)Int64.MaxValue+(BigInteger)500,
        (BigInteger)UInt64.MaxValue-(BigInteger)500,
        maxuint,
      };
      for (int i = 0; i < ranges.Length; i += 2) {
        BigInteger bigintTemp = ranges[i];
        while (true) {
          CBORObject obj = CBORObject.FromObjectAndTag(0, bigintTemp);
          Assert.IsTrue(obj.IsTagged, "obj not tagged");
          BigInteger[] tags = obj.GetTags();
          Assert.AreEqual(1, tags.Length);
          Assert.AreEqual(bigintTemp, tags[0]);
          if (!obj.InnermostTag.Equals(bigintTemp))
            Assert.AreEqual(bigintTemp, obj.InnermostTag,
                            String.Format(CultureInfo.InvariantCulture,
                                          "obj tag doesn't match: {0}", obj));
          TestCommon.AssertSer(
            obj,
            String.Format(CultureInfo.InvariantCulture, "{0}(0)", bigintTemp));
          if (!(bigintTemp.Equals(maxuint))) {
            // Test multiple tags
            CBORObject obj2 = CBORObject.FromObjectAndTag(obj, bigintTemp + BigInteger.One);
            BigInteger[] bi = obj2.GetTags();
            if (bi.Length != 2)
              Assert.AreEqual(2, bi.Length,
                              String.Format(CultureInfo.InvariantCulture,
                                            "Expected 2 tags: {0}", obj2));
            if (!bi[0].Equals((BigInteger)bigintTemp + BigInteger.One))
              Assert.AreEqual(bigintTemp + BigInteger.One, bi[0],
                              String.Format(CultureInfo.InvariantCulture,
                                            "Outer tag doesn't match: {0}", obj2));
            if (!(bi[1] == (BigInteger)bigintTemp))
              Assert.AreEqual(bigintTemp, bi[1],
                              String.Format(CultureInfo.InvariantCulture,
                                            "Inner tag doesn't match: {0}", obj2));
            if (!(obj2.InnermostTag == (BigInteger)bigintTemp))
              Assert.AreEqual(bigintTemp, bi[0],
                              String.Format(CultureInfo.InvariantCulture,
                                            "Innermost tag doesn't match: {0}", obj2));
            TestCommon.AssertSer(
              obj2,
              String.Format(CultureInfo.InvariantCulture, "{0}({1}(0))",
                            bigintTemp + BigInteger.One, bigintTemp));
          }
          if (bigintTemp.Equals(ranges[i + 1])) break;
          bigintTemp = bigintTemp + BigInteger.One;
        }
      }
    }

  }
}