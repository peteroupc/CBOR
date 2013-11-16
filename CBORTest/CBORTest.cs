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
    private static void TestBigFloatDoubleCore(double d, string s) {
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

    private static void TestBigFloatSingleCore(float d, string s) {
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

    private static CBORObject RandomNumber(FastRandom rand) {
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

    private static long RandomInt64(FastRandom rand) {
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

    private static double RandomDouble(FastRandom rand, int exponent) {
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
      return BitConverter.ToDouble(BitConverter.GetBytes((long)r),0);
    }

    private static float RandomSingle(FastRandom rand, int exponent) {
      if (exponent == Int32.MaxValue)
        exponent = rand.NextValue(255);
      int r = rand.NextValue(0x10000);
      if (rand.NextValue(2) == 0) {
        r |= ((int)rand.NextValue(0x10000)) << 16;
      }
      r &= ~0x7F800000; // clear exponent
      r |= ((int)exponent) << 23; // set exponent
      return BitConverter.ToSingle(BitConverter.GetBytes((int)r),0);
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

    private static void TestDecimalString(String r) {
      CBORObject o = CBORObject.FromObject(DecimalFraction.FromString(r));
      CBORObject o2 = CBORDataUtilities.ParseJSONNumber(r);
      CompareTestEqual(o,o2);
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

    private static string ObjectMessages(CBORObject o1, CBORObject o2, String s){
        if(o1.Type== CBORType.Number && o2.Type== CBORType.Number){
          return s+":\n" + o1.ToString() + " and\n" + o2.ToString()+"\nOR\n"+
                          o1.AsDecimalFraction().ToString() + " and\n" + o2.AsDecimalFraction().ToString();                  
        } else {
          return s+":\n" + o1.ToString() + " and\n" + o2.ToString();          
        }      
    }
    
    private static void CompareTestEqual(CBORObject o1, CBORObject o2){
      if(CompareTestReciprocal(o1,o2)!=0)
        Assert.Fail(ObjectMessages(o1,o2,"Not equal: "+CompareTestReciprocal(o1,o2)));
    }
    private static void CompareTestLess(CBORObject o1, CBORObject o2){
      if(CompareTestReciprocal(o1,o2)>=0)
        Assert.Fail(ObjectMessages(o1,o2,"Not less: "+CompareTestReciprocal(o1,o2)));
    }
    
    private static int CompareTestReciprocal(CBORObject o1, CBORObject o2){
      if((o1)==null)throw new ArgumentNullException("o1");
      if((o2)==null)throw new ArgumentNullException("o2");
      int cmp=o1.CompareTo(o2);
      int cmp2=o2.CompareTo(o1);
      if(-cmp2!=cmp){
        Assert.AreEqual(cmp,-cmp2,ObjectMessages(o1,o2,"Not reciprocal"));
      }
      return cmp;
    }
    
    [Test]
    public void TestCompare() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 3000; i++) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        int cmpDecFrac = o1.AsDecimalFraction().CompareTo(o2.AsDecimalFraction());
        int cmpCobj = CompareTestReciprocal(o1,o2);
        if (cmpDecFrac != cmpCobj) {
          Assert.AreEqual(cmpDecFrac, cmpCobj,ObjectMessages(o1,o2,"Results don't match"));
        }
        TestCommon.AssertRoundTrip(o1);
        TestCommon.AssertRoundTrip(o2);
      }
      for (int i = 0; i < 50; i++) {
        CBORObject o1 = CBORObject.FromObject(Single.NegativeInfinity);
        CBORObject o2 = RandomNumber(r);
        CompareTestLess(o1,o2);
        o1=CBORObject.FromObject(Double.NegativeInfinity);
        CompareTestLess(o1,o2);
        o1=CBORObject.FromObject(Single.PositiveInfinity);
        CompareTestLess(o2,o1);
        o1=CBORObject.FromObject(Double.PositiveInfinity);
        CompareTestLess(o2,o1);
        o1=CBORObject.FromObject(Single.NaN);
        CompareTestLess(o2,o1);
        o1=CBORObject.FromObject(Double.NaN);
        CompareTestLess(o2,o1);
      }
      CBORObject sp=CBORObject.FromObject(Single.PositiveInfinity);
      CBORObject sn=CBORObject.FromObject(Single.NegativeInfinity);
      CBORObject snan=CBORObject.FromObject(Single.NaN);
      CBORObject dp=CBORObject.FromObject(Double.PositiveInfinity);
      CBORObject dn=CBORObject.FromObject(Double.NegativeInfinity);
      CBORObject dnan=CBORObject.FromObject(Double.NaN);
      CompareTestEqual(sp,sp);
      CompareTestEqual(sp,dp);
      CompareTestEqual(dp,dp);
      CompareTestEqual(sn,sn);
      CompareTestEqual(sn,dn);
      CompareTestEqual(dn,dn);
      CompareTestEqual(snan,snan);
      CompareTestEqual(snan,dnan);
      CompareTestEqual(dnan,dnan);
      CompareTestLess(sn,sp);
      CompareTestLess(sn,dp);
      CompareTestLess(sn,snan);
      CompareTestLess(sn,dnan);
      CompareTestLess(sp,snan);
      CompareTestLess(sp,dnan);
      CompareTestLess(dn,dp);
      CompareTestLess(dp,dnan);
    }

    [Test]
    public void TestParseDecimalStrings() {
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 3000; i++) {
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
    public void TestJSONSurrogates(){
      try { CBORObject.FromJSONString("[\"\ud800\udc00\"]"); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromJSONString("[\"\\ud800\\udc00\"]"); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromJSONString("[\"\ud800\\udc00\"]"); } catch(CBORException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromJSONString("[\"\\ud800\udc00\"]"); } catch(CBORException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromJSONString("[\"\\udc00\ud800\udc00\"]"); } catch(CBORException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromJSONString("[\"\\ud800\ud800\udc00\"]"); } catch(CBORException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
      try { CBORObject.FromJSONString("[\"\\ud800\\udc00\ud800\udc00\"]"); } catch(Exception ex){ Assert.Fail(ex.ToString()); }
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

    private static bool ByteArrayEquals(byte[] arrayA, byte[] arrayB) {
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
      CompareTestEqual(ca,cb);
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
    public void TestDecFracToSingleDoubleHighExponents(){
      if(914323.0f!=DecimalFraction.FromString("914323").ToSingle())
        Assert.Fail("decfrac single 914323\nExpected: 914323.0f\nWas: "+DecimalFraction.FromString("914323").ToSingle());
      if(914323.0d!=DecimalFraction.FromString("914323").ToDouble())
        Assert.Fail("decfrac double 914323\nExpected: 914323.0d\nWas: "+DecimalFraction.FromString("914323").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-57318007483673759194E+106").ToSingle())
        Assert.Fail("decfrac single -57318007483673759194E+106\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-57318007483673759194E+106").ToSingle());
      if(-5.731800748367376E125d!=DecimalFraction.FromString("-57318007483673759194E+106").ToDouble())
        Assert.Fail("decfrac double -57318007483673759194E+106\nExpected: -5.731800748367376E125d\nWas: "+DecimalFraction.FromString("-57318007483673759194E+106").ToDouble());
      if(0.0f!=DecimalFraction.FromString("420685230629E-264").ToSingle())
        Assert.Fail("decfrac single 420685230629E-264\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("420685230629E-264").ToSingle());
      if(4.20685230629E-253d!=DecimalFraction.FromString("420685230629E-264").ToDouble())
        Assert.Fail("decfrac double 420685230629E-264\nExpected: 4.20685230629E-253d\nWas: "+DecimalFraction.FromString("420685230629E-264").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("1089152800893419E+168").ToSingle())
        Assert.Fail("decfrac single 1089152800893419E+168\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("1089152800893419E+168").ToSingle());
      if(1.089152800893419E183d!=DecimalFraction.FromString("1089152800893419E+168").ToDouble())
        Assert.Fail("decfrac double 1089152800893419E+168\nExpected: 1.089152800893419E183d\nWas: "+DecimalFraction.FromString("1089152800893419E+168").ToDouble());
      if(1.5936804E7f!=DecimalFraction.FromString("15936804").ToSingle())
        Assert.Fail("decfrac single 15936804\nExpected: 1.5936804E7f\nWas: "+DecimalFraction.FromString("15936804").ToSingle());
      if(1.5936804E7d!=DecimalFraction.FromString("15936804").ToDouble())
        Assert.Fail("decfrac double 15936804\nExpected: 1.5936804E7d\nWas: "+DecimalFraction.FromString("15936804").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-24681.2332E+61").ToSingle())
        Assert.Fail("decfrac single -24681.2332E+61\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-24681.2332E+61").ToSingle());
      if(-2.46812332E65d!=DecimalFraction.FromString("-24681.2332E+61").ToDouble())
        Assert.Fail("decfrac double -24681.2332E+61\nExpected: -2.46812332E65d\nWas: "+DecimalFraction.FromString("-24681.2332E+61").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-417509591569.6827833177512321E-93").ToSingle())
        Assert.Fail("decfrac single -417509591569.6827833177512321E-93\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-417509591569.6827833177512321E-93").ToSingle());
      if(-4.175095915696828E-82d!=DecimalFraction.FromString("-417509591569.6827833177512321E-93").ToDouble())
        Assert.Fail("decfrac double -417509591569.6827833177512321E-93\nExpected: -4.175095915696828E-82d\nWas: "+DecimalFraction.FromString("-417509591569.6827833177512321E-93").ToDouble());
      if(5.38988331E17f!=DecimalFraction.FromString("538988338119784732").ToSingle())
        Assert.Fail("decfrac single 538988338119784732\nExpected: 5.38988331E17f\nWas: "+DecimalFraction.FromString("538988338119784732").ToSingle());
      if(5.389883381197847E17d!=DecimalFraction.FromString("538988338119784732").ToDouble())
        Assert.Fail("decfrac double 538988338119784732\nExpected: 5.389883381197847E17d\nWas: "+DecimalFraction.FromString("538988338119784732").ToDouble());
      if(260.14423f!=DecimalFraction.FromString("260.1442248").ToSingle())
        Assert.Fail("decfrac single 260.1442248\nExpected: 260.14423f\nWas: "+DecimalFraction.FromString("260.1442248").ToSingle());
      if(260.1442248d!=DecimalFraction.FromString("260.1442248").ToDouble())
        Assert.Fail("decfrac double 260.1442248\nExpected: 260.1442248d\nWas: "+DecimalFraction.FromString("260.1442248").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-8457715957008143770.130850853640402959E-181").ToSingle())
        Assert.Fail("decfrac single -8457715957008143770.130850853640402959E-181\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-8457715957008143770.130850853640402959E-181").ToSingle());
      if(-8.457715957008144E-163d!=DecimalFraction.FromString("-8457715957008143770.130850853640402959E-181").ToDouble())
        Assert.Fail("decfrac double -8457715957008143770.130850853640402959E-181\nExpected: -8.457715957008144E-163d\nWas: "+DecimalFraction.FromString("-8457715957008143770.130850853640402959E-181").ToDouble());
      if(0.0f!=DecimalFraction.FromString("22.7178448747E-225").ToSingle())
        Assert.Fail("decfrac single 22.7178448747E-225\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("22.7178448747E-225").ToSingle());
      if(2.27178448747E-224d!=DecimalFraction.FromString("22.7178448747E-225").ToDouble())
        Assert.Fail("decfrac double 22.7178448747E-225\nExpected: 2.27178448747E-224d\nWas: "+DecimalFraction.FromString("22.7178448747E-225").ToDouble());
      if(-790581.44f!=DecimalFraction.FromString("-790581.4576317018014").ToSingle())
        Assert.Fail("decfrac single -790581.4576317018014\nExpected: -790581.44f\nWas: "+DecimalFraction.FromString("-790581.4576317018014").ToSingle());
      if(-790581.4576317018d!=DecimalFraction.FromString("-790581.4576317018014").ToDouble())
        Assert.Fail("decfrac double -790581.4576317018014\nExpected: -790581.4576317018d\nWas: "+DecimalFraction.FromString("-790581.4576317018014").ToDouble());
      if(-1.80151695E16f!=DecimalFraction.FromString("-18015168704168440").ToSingle())
        Assert.Fail("decfrac single -18015168704168440\nExpected: -1.80151695E16f\nWas: "+DecimalFraction.FromString("-18015168704168440").ToSingle());
      if(-1.801516870416844E16d!=DecimalFraction.FromString("-18015168704168440").ToDouble())
        Assert.Fail("decfrac double -18015168704168440\nExpected: -1.801516870416844E16d\nWas: "+DecimalFraction.FromString("-18015168704168440").ToDouble());
      if(-36.0f!=DecimalFraction.FromString("-36").ToSingle())
        Assert.Fail("decfrac single -36\nExpected: -36.0f\nWas: "+DecimalFraction.FromString("-36").ToSingle());
      if(-36.0d!=DecimalFraction.FromString("-36").ToDouble())
        Assert.Fail("decfrac double -36\nExpected: -36.0d\nWas: "+DecimalFraction.FromString("-36").ToDouble());
      if(0.0f!=DecimalFraction.FromString("653060307988076E-230").ToSingle())
        Assert.Fail("decfrac single 653060307988076E-230\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("653060307988076E-230").ToSingle());
      if(6.53060307988076E-216d!=DecimalFraction.FromString("653060307988076E-230").ToDouble())
        Assert.Fail("decfrac double 653060307988076E-230\nExpected: 6.53060307988076E-216d\nWas: "+DecimalFraction.FromString("653060307988076E-230").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-4446345.5911E+316").ToSingle())
        Assert.Fail("decfrac single -4446345.5911E+316\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-4446345.5911E+316").ToSingle());
      if(Double.NegativeInfinity!=DecimalFraction.FromString("-4446345.5911E+316").ToDouble())
        Assert.Fail("decfrac double -4446345.5911E+316\nExpected: Double.NegativeInfinity\nWas: "+DecimalFraction.FromString("-4446345.5911E+316").ToDouble());
      if(-5.3940226E15f!=DecimalFraction.FromString("-5394022706804125.84338479899885").ToSingle())
        Assert.Fail("decfrac single -5394022706804125.84338479899885\nExpected: -5.3940226E15f\nWas: "+DecimalFraction.FromString("-5394022706804125.84338479899885").ToSingle());
      if(-5.394022706804126E15d!=DecimalFraction.FromString("-5394022706804125.84338479899885").ToDouble())
        Assert.Fail("decfrac double -5394022706804125.84338479899885\nExpected: -5.394022706804126E15d\nWas: "+DecimalFraction.FromString("-5394022706804125.84338479899885").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("310504020304E+181").ToSingle())
        Assert.Fail("decfrac single 310504020304E+181\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("310504020304E+181").ToSingle());
      if(3.10504020304E192d!=DecimalFraction.FromString("310504020304E+181").ToDouble())
        Assert.Fail("decfrac double 310504020304E+181\nExpected: 3.10504020304E192d\nWas: "+DecimalFraction.FromString("310504020304E+181").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-164609450222646.21988340572652533E+317").ToSingle())
        Assert.Fail("decfrac single -164609450222646.21988340572652533E+317\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-164609450222646.21988340572652533E+317").ToSingle());
      if(Double.NegativeInfinity!=DecimalFraction.FromString("-164609450222646.21988340572652533E+317").ToDouble())
        Assert.Fail("decfrac double -164609450222646.21988340572652533E+317\nExpected: Double.NegativeInfinity\nWas: "+DecimalFraction.FromString("-164609450222646.21988340572652533E+317").ToDouble());
      if(7.1524661E18f!=DecimalFraction.FromString("7152466127871812565.075310").ToSingle())
        Assert.Fail("decfrac single 7152466127871812565.075310\nExpected: 7.1524661E18f\nWas: "+DecimalFraction.FromString("7152466127871812565.075310").ToSingle());
      if(7.1524661278718126E18d!=DecimalFraction.FromString("7152466127871812565.075310").ToDouble())
        Assert.Fail("decfrac double 7152466127871812565.075310\nExpected: 7.1524661278718126E18d\nWas: "+DecimalFraction.FromString("7152466127871812565.075310").ToDouble());
      if(925.0f!=DecimalFraction.FromString("925").ToSingle())
        Assert.Fail("decfrac single 925\nExpected: 925.0f\nWas: "+DecimalFraction.FromString("925").ToSingle());
      if(925.0d!=DecimalFraction.FromString("925").ToDouble())
        Assert.Fail("decfrac double 925\nExpected: 925.0d\nWas: "+DecimalFraction.FromString("925").ToDouble());
      if(34794.0f!=DecimalFraction.FromString("34794").ToSingle())
        Assert.Fail("decfrac single 34794\nExpected: 34794.0f\nWas: "+DecimalFraction.FromString("34794").ToSingle());
      if(34794.0d!=DecimalFraction.FromString("34794").ToDouble())
        Assert.Fail("decfrac double 34794\nExpected: 34794.0d\nWas: "+DecimalFraction.FromString("34794").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-337655705333269E-276").ToSingle())
        Assert.Fail("decfrac single -337655705333269E-276\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-337655705333269E-276").ToSingle());
      if(-3.37655705333269E-262d!=DecimalFraction.FromString("-337655705333269E-276").ToDouble())
        Assert.Fail("decfrac double -337655705333269E-276\nExpected: -3.37655705333269E-262d\nWas: "+DecimalFraction.FromString("-337655705333269E-276").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-564484627E-81").ToSingle())
        Assert.Fail("decfrac single -564484627E-81\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-564484627E-81").ToSingle());
      if(-5.64484627E-73d!=DecimalFraction.FromString("-564484627E-81").ToDouble())
        Assert.Fail("decfrac double -564484627E-81\nExpected: -5.64484627E-73d\nWas: "+DecimalFraction.FromString("-564484627E-81").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-249095219081.80985049618E+175").ToSingle())
        Assert.Fail("decfrac single -249095219081.80985049618E+175\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-249095219081.80985049618E+175").ToSingle());
      if(-2.4909521908180986E186d!=DecimalFraction.FromString("-249095219081.80985049618E+175").ToDouble())
        Assert.Fail("decfrac double -249095219081.80985049618E+175\nExpected: -2.4909521908180986E186d\nWas: "+DecimalFraction.FromString("-249095219081.80985049618E+175").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-1696361380616078392E+221").ToSingle())
        Assert.Fail("decfrac single -1696361380616078392E+221\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-1696361380616078392E+221").ToSingle());
      if(-1.6963613806160784E239d!=DecimalFraction.FromString("-1696361380616078392E+221").ToDouble())
        Assert.Fail("decfrac double -1696361380616078392E+221\nExpected: -1.6963613806160784E239d\nWas: "+DecimalFraction.FromString("-1696361380616078392E+221").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("61520501993928105481.8536829047214988E+205").ToSingle())
        Assert.Fail("decfrac single 61520501993928105481.8536829047214988E+205\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("61520501993928105481.8536829047214988E+205").ToSingle());
      if(6.15205019939281E224d!=DecimalFraction.FromString("61520501993928105481.8536829047214988E+205").ToDouble())
        Assert.Fail("decfrac double 61520501993928105481.8536829047214988E+205\nExpected: 6.15205019939281E224d\nWas: "+DecimalFraction.FromString("61520501993928105481.8536829047214988E+205").ToDouble());
      if(2.08756651E14f!=DecimalFraction.FromString("208756654290770").ToSingle())
        Assert.Fail("decfrac single 208756654290770\nExpected: 2.08756651E14f\nWas: "+DecimalFraction.FromString("208756654290770").ToSingle());
      if(2.0875665429077E14d!=DecimalFraction.FromString("208756654290770").ToDouble())
        Assert.Fail("decfrac double 208756654290770\nExpected: 2.0875665429077E14d\nWas: "+DecimalFraction.FromString("208756654290770").ToDouble());
      if(-1.31098592E13f!=DecimalFraction.FromString("-13109858687380").ToSingle())
        Assert.Fail("decfrac single -13109858687380\nExpected: -1.31098592E13f\nWas: "+DecimalFraction.FromString("-13109858687380").ToSingle());
      if(-1.310985868738E13d!=DecimalFraction.FromString("-13109858687380").ToDouble())
        Assert.Fail("decfrac double -13109858687380\nExpected: -1.310985868738E13d\nWas: "+DecimalFraction.FromString("-13109858687380").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("6650596004E+280").ToSingle())
        Assert.Fail("decfrac single 6650596004E+280\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("6650596004E+280").ToSingle());
      if(6.650596004E289d!=DecimalFraction.FromString("6650596004E+280").ToDouble())
        Assert.Fail("decfrac double 6650596004E+280\nExpected: 6.650596004E289d\nWas: "+DecimalFraction.FromString("6650596004E+280").ToDouble());
      if(-9.2917935E13f!=DecimalFraction.FromString("-92917937534357E0").ToSingle())
        Assert.Fail("decfrac single -92917937534357E0\nExpected: -9.2917935E13f\nWas: "+DecimalFraction.FromString("-92917937534357E0").ToSingle());
      if(-9.2917937534357E13d!=DecimalFraction.FromString("-92917937534357E0").ToDouble())
        Assert.Fail("decfrac double -92917937534357E0\nExpected: -9.2917937534357E13d\nWas: "+DecimalFraction.FromString("-92917937534357E0").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-46E-153").ToSingle())
        Assert.Fail("decfrac single -46E-153\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-46E-153").ToSingle());
      if(-4.6E-152d!=DecimalFraction.FromString("-46E-153").ToDouble())
        Assert.Fail("decfrac double -46E-153\nExpected: -4.6E-152d\nWas: "+DecimalFraction.FromString("-46E-153").ToDouble());
      if(1.05161414E13f!=DecimalFraction.FromString("10516141645281.77872161523035480").ToSingle())
        Assert.Fail("decfrac single 10516141645281.77872161523035480\nExpected: 1.05161414E13f\nWas: "+DecimalFraction.FromString("10516141645281.77872161523035480").ToSingle());
      if(1.051614164528178E13d!=DecimalFraction.FromString("10516141645281.77872161523035480").ToDouble())
        Assert.Fail("decfrac double 10516141645281.77872161523035480\nExpected: 1.051614164528178E13d\nWas: "+DecimalFraction.FromString("10516141645281.77872161523035480").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-8312147094254E+299").ToSingle())
        Assert.Fail("decfrac single -8312147094254E+299\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-8312147094254E+299").ToSingle());
      if(Double.NegativeInfinity!=DecimalFraction.FromString("-8312147094254E+299").ToDouble())
        Assert.Fail("decfrac double -8312147094254E+299\nExpected: Double.NegativeInfinity\nWas: "+DecimalFraction.FromString("-8312147094254E+299").ToDouble());
      if(5.10270368E8f!=DecimalFraction.FromString("510270376.1879").ToSingle())
        Assert.Fail("decfrac single 510270376.1879\nExpected: 5.10270368E8f\nWas: "+DecimalFraction.FromString("510270376.1879").ToSingle());
      if(5.102703761879E8d!=DecimalFraction.FromString("510270376.1879").ToDouble())
        Assert.Fail("decfrac double 510270376.1879\nExpected: 5.102703761879E8d\nWas: "+DecimalFraction.FromString("510270376.1879").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-693696E-143").ToSingle())
        Assert.Fail("decfrac single -693696E-143\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-693696E-143").ToSingle());
      if(-6.93696E-138d!=DecimalFraction.FromString("-693696E-143").ToDouble())
        Assert.Fail("decfrac double -693696E-143\nExpected: -6.93696E-138d\nWas: "+DecimalFraction.FromString("-693696E-143").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-91.43E+139").ToSingle())
        Assert.Fail("decfrac single -91.43E+139\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-91.43E+139").ToSingle());
      if(-9.143E140d!=DecimalFraction.FromString("-91.43E+139").ToDouble())
        Assert.Fail("decfrac double -91.43E+139\nExpected: -9.143E140d\nWas: "+DecimalFraction.FromString("-91.43E+139").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-4103819741762400.45807953367286162E+235").ToSingle())
        Assert.Fail("decfrac single -4103819741762400.45807953367286162E+235\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-4103819741762400.45807953367286162E+235").ToSingle());
      if(-4.1038197417624E250d!=DecimalFraction.FromString("-4103819741762400.45807953367286162E+235").ToDouble())
        Assert.Fail("decfrac double -4103819741762400.45807953367286162E+235\nExpected: -4.1038197417624E250d\nWas: "+DecimalFraction.FromString("-4103819741762400.45807953367286162E+235").ToDouble());
      if(-1.44700998E11f!=DecimalFraction.FromString("-144701002301.18954542331279957").ToSingle())
        Assert.Fail("decfrac single -144701002301.18954542331279957\nExpected: -1.44700998E11f\nWas: "+DecimalFraction.FromString("-144701002301.18954542331279957").ToSingle());
      if(-1.4470100230118954E11d!=DecimalFraction.FromString("-144701002301.18954542331279957").ToDouble())
        Assert.Fail("decfrac double -144701002301.18954542331279957\nExpected: -1.4470100230118954E11d\nWas: "+DecimalFraction.FromString("-144701002301.18954542331279957").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("73.01E+211").ToSingle())
        Assert.Fail("decfrac single 73.01E+211\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("73.01E+211").ToSingle());
      if(7.301E212d!=DecimalFraction.FromString("73.01E+211").ToDouble())
        Assert.Fail("decfrac double 73.01E+211\nExpected: 7.301E212d\nWas: "+DecimalFraction.FromString("73.01E+211").ToDouble());
      if(-4.4030403E9f!=DecimalFraction.FromString("-4403040441").ToSingle())
        Assert.Fail("decfrac single -4403040441\nExpected: -4.4030403E9f\nWas: "+DecimalFraction.FromString("-4403040441").ToSingle());
      if(-4.403040441E9d!=DecimalFraction.FromString("-4403040441").ToDouble())
        Assert.Fail("decfrac double -4403040441\nExpected: -4.403040441E9d\nWas: "+DecimalFraction.FromString("-4403040441").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-19E+64").ToSingle())
        Assert.Fail("decfrac single -19E+64\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-19E+64").ToSingle());
      if(-1.9E65d!=DecimalFraction.FromString("-19E+64").ToDouble())
        Assert.Fail("decfrac double -19E+64\nExpected: -1.9E65d\nWas: "+DecimalFraction.FromString("-19E+64").ToDouble());
      if(0.0f!=DecimalFraction.FromString("6454087684516815.5353496080253E-144").ToSingle())
        Assert.Fail("decfrac single 6454087684516815.5353496080253E-144\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("6454087684516815.5353496080253E-144").ToSingle());
      if(6.454087684516816E-129d!=DecimalFraction.FromString("6454087684516815.5353496080253E-144").ToDouble())
        Assert.Fail("decfrac double 6454087684516815.5353496080253E-144\nExpected: 6.454087684516816E-129d\nWas: "+DecimalFraction.FromString("6454087684516815.5353496080253E-144").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("1051852710343668.522107559786846776E+278").ToSingle())
        Assert.Fail("decfrac single 1051852710343668.522107559786846776E+278\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("1051852710343668.522107559786846776E+278").ToSingle());
      if(1.0518527103436685E293d!=DecimalFraction.FromString("1051852710343668.522107559786846776E+278").ToDouble())
        Assert.Fail("decfrac double 1051852710343668.522107559786846776E+278\nExpected: 1.0518527103436685E293d\nWas: "+DecimalFraction.FromString("1051852710343668.522107559786846776E+278").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("86077128802.374518623891E+218").ToSingle())
        Assert.Fail("decfrac single 86077128802.374518623891E+218\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("86077128802.374518623891E+218").ToSingle());
      if(8.607712880237452E228d!=DecimalFraction.FromString("86077128802.374518623891E+218").ToDouble())
        Assert.Fail("decfrac double 86077128802.374518623891E+218\nExpected: 8.607712880237452E228d\nWas: "+DecimalFraction.FromString("86077128802.374518623891E+218").ToDouble());
      if(0.0f!=DecimalFraction.FromString("367820230207102E-199").ToSingle())
        Assert.Fail("decfrac single 367820230207102E-199\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("367820230207102E-199").ToSingle());
      if(3.67820230207102E-185d!=DecimalFraction.FromString("367820230207102E-199").ToDouble())
        Assert.Fail("decfrac double 367820230207102E-199\nExpected: 3.67820230207102E-185d\nWas: "+DecimalFraction.FromString("367820230207102E-199").ToDouble());
      if(9.105086E-27f!=DecimalFraction.FromString("91050857573912688994E-46").ToSingle())
        Assert.Fail("decfrac single 91050857573912688994E-46\nExpected: 9.105086E-27f\nWas: "+DecimalFraction.FromString("91050857573912688994E-46").ToSingle());
      if(9.105085757391269E-27d!=DecimalFraction.FromString("91050857573912688994E-46").ToDouble())
        Assert.Fail("decfrac double 91050857573912688994E-46\nExpected: 9.105085757391269E-27d\nWas: "+DecimalFraction.FromString("91050857573912688994E-46").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("73.895899E+102").ToSingle())
        Assert.Fail("decfrac single 73.895899E+102\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("73.895899E+102").ToSingle());
      if(7.3895899E103d!=DecimalFraction.FromString("73.895899E+102").ToDouble())
        Assert.Fail("decfrac double 73.895899E+102\nExpected: 7.3895899E103d\nWas: "+DecimalFraction.FromString("73.895899E+102").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-796808893178.891470585829021E+330").ToSingle())
        Assert.Fail("decfrac single -796808893178.891470585829021E+330\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-796808893178.891470585829021E+330").ToSingle());
      if(Double.NegativeInfinity!=DecimalFraction.FromString("-796808893178.891470585829021E+330").ToDouble())
        Assert.Fail("decfrac double -796808893178.891470585829021E+330\nExpected: Double.NegativeInfinity\nWas: "+DecimalFraction.FromString("-796808893178.891470585829021E+330").ToDouble());
      if(0.0f!=DecimalFraction.FromString("275081E-206").ToSingle())
        Assert.Fail("decfrac single 275081E-206\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("275081E-206").ToSingle());
      if(2.75081E-201d!=DecimalFraction.FromString("275081E-206").ToDouble())
        Assert.Fail("decfrac double 275081E-206\nExpected: 2.75081E-201d\nWas: "+DecimalFraction.FromString("275081E-206").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-4322898910615499.82096E-95").ToSingle())
        Assert.Fail("decfrac single -4322898910615499.82096E-95\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-4322898910615499.82096E-95").ToSingle());
      if(-4.3228989106155E-80d!=DecimalFraction.FromString("-4322898910615499.82096E-95").ToDouble())
        Assert.Fail("decfrac double -4322898910615499.82096E-95\nExpected: -4.3228989106155E-80d\nWas: "+DecimalFraction.FromString("-4322898910615499.82096E-95").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("143343913109764E+63").ToSingle())
        Assert.Fail("decfrac single 143343913109764E+63\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("143343913109764E+63").ToSingle());
      if(1.43343913109764E77d!=DecimalFraction.FromString("143343913109764E+63").ToDouble())
        Assert.Fail("decfrac double 143343913109764E+63\nExpected: 1.43343913109764E77d\nWas: "+DecimalFraction.FromString("143343913109764E+63").ToDouble());
      if(-7.9102981E16f!=DecimalFraction.FromString("-79102983237104015").ToSingle())
        Assert.Fail("decfrac single -79102983237104015\nExpected: -7.9102981E16f\nWas: "+DecimalFraction.FromString("-79102983237104015").ToSingle());
      if(-7.9102983237104016E16d!=DecimalFraction.FromString("-79102983237104015").ToDouble())
        Assert.Fail("decfrac double -79102983237104015\nExpected: -7.9102983237104016E16d\nWas: "+DecimalFraction.FromString("-79102983237104015").ToDouble());
      if(-9.07E-10f!=DecimalFraction.FromString("-907E-12").ToSingle())
        Assert.Fail("decfrac single -907E-12\nExpected: -9.07E-10f\nWas: "+DecimalFraction.FromString("-907E-12").ToSingle());
      if(-9.07E-10d!=DecimalFraction.FromString("-907E-12").ToDouble())
        Assert.Fail("decfrac double -907E-12\nExpected: -9.07E-10d\nWas: "+DecimalFraction.FromString("-907E-12").ToDouble());
      if(0.0f!=DecimalFraction.FromString("191682103431.217475E-84").ToSingle())
        Assert.Fail("decfrac single 191682103431.217475E-84\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("191682103431.217475E-84").ToSingle());
      if(1.9168210343121748E-73d!=DecimalFraction.FromString("191682103431.217475E-84").ToDouble())
        Assert.Fail("decfrac double 191682103431.217475E-84\nExpected: 1.9168210343121748E-73d\nWas: "+DecimalFraction.FromString("191682103431.217475E-84").ToDouble());
      if(-5.6E-45f!=DecimalFraction.FromString("-492913.1840948615992120438E-50").ToSingle())
        Assert.Fail("decfrac single -492913.1840948615992120438E-50\nExpected: -5.6E-45f\nWas: "+DecimalFraction.FromString("-492913.1840948615992120438E-50").ToSingle());
      if(-4.929131840948616E-45d!=DecimalFraction.FromString("-492913.1840948615992120438E-50").ToDouble())
        Assert.Fail("decfrac double -492913.1840948615992120438E-50\nExpected: -4.929131840948616E-45d\nWas: "+DecimalFraction.FromString("-492913.1840948615992120438E-50").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-752873150058767E+272").ToSingle())
        Assert.Fail("decfrac single -752873150058767E+272\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-752873150058767E+272").ToSingle());
      if(-7.52873150058767E286d!=DecimalFraction.FromString("-752873150058767E+272").ToDouble())
        Assert.Fail("decfrac double -752873150058767E+272\nExpected: -7.52873150058767E286d\nWas: "+DecimalFraction.FromString("-752873150058767E+272").ToDouble());
      if(27.311937f!=DecimalFraction.FromString("27.311937404").ToSingle())
        Assert.Fail("decfrac single 27.311937404\nExpected: 27.311937f\nWas: "+DecimalFraction.FromString("27.311937404").ToSingle());
      if(27.311937404d!=DecimalFraction.FromString("27.311937404").ToDouble())
        Assert.Fail("decfrac double 27.311937404\nExpected: 27.311937404d\nWas: "+DecimalFraction.FromString("27.311937404").ToDouble());
      if(0.0f!=DecimalFraction.FromString("39147083343918E-143").ToSingle())
        Assert.Fail("decfrac single 39147083343918E-143\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("39147083343918E-143").ToSingle());
      if(3.9147083343918E-130d!=DecimalFraction.FromString("39147083343918E-143").ToDouble())
        Assert.Fail("decfrac double 39147083343918E-143\nExpected: 3.9147083343918E-130d\nWas: "+DecimalFraction.FromString("39147083343918E-143").ToDouble());
      if(-1.97684019E11f!=DecimalFraction.FromString("-197684018253").ToSingle())
        Assert.Fail("decfrac single -197684018253\nExpected: -1.97684019E11f\nWas: "+DecimalFraction.FromString("-197684018253").ToSingle());
      if(-1.97684018253E11d!=DecimalFraction.FromString("-197684018253").ToDouble())
        Assert.Fail("decfrac double -197684018253\nExpected: -1.97684018253E11d\nWas: "+DecimalFraction.FromString("-197684018253").ToDouble());
      if(6.400822E14f!=DecimalFraction.FromString("640082188903507").ToSingle())
        Assert.Fail("decfrac single 640082188903507\nExpected: 6.400822E14f\nWas: "+DecimalFraction.FromString("640082188903507").ToSingle());
      if(6.40082188903507E14d!=DecimalFraction.FromString("640082188903507").ToDouble())
        Assert.Fail("decfrac double 640082188903507\nExpected: 6.40082188903507E14d\nWas: "+DecimalFraction.FromString("640082188903507").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-913144352720144E-312").ToSingle())
        Assert.Fail("decfrac single -913144352720144E-312\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-913144352720144E-312").ToSingle());
      if(-9.13144352720144E-298d!=DecimalFraction.FromString("-913144352720144E-312").ToDouble())
        Assert.Fail("decfrac double -913144352720144E-312\nExpected: -9.13144352720144E-298d\nWas: "+DecimalFraction.FromString("-913144352720144E-312").ToDouble());
      if(-3.68781005E15f!=DecimalFraction.FromString("-3687809947210631").ToSingle())
        Assert.Fail("decfrac single -3687809947210631\nExpected: -3.68781005E15f\nWas: "+DecimalFraction.FromString("-3687809947210631").ToSingle());
      if(-3.687809947210631E15d!=DecimalFraction.FromString("-3687809947210631").ToDouble())
        Assert.Fail("decfrac double -3687809947210631\nExpected: -3.687809947210631E15d\nWas: "+DecimalFraction.FromString("-3687809947210631").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("53083788630724917310.06236692262351E+169").ToSingle())
        Assert.Fail("decfrac single 53083788630724917310.06236692262351E+169\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("53083788630724917310.06236692262351E+169").ToSingle());
      if(5.3083788630724916E188d!=DecimalFraction.FromString("53083788630724917310.06236692262351E+169").ToDouble())
        Assert.Fail("decfrac double 53083788630724917310.06236692262351E+169\nExpected: 5.3083788630724916E188d\nWas: "+DecimalFraction.FromString("53083788630724917310.06236692262351E+169").ToDouble());
      if(-7.0943446E19f!=DecimalFraction.FromString("-70943446332471357958").ToSingle())
        Assert.Fail("decfrac single -70943446332471357958\nExpected: -7.0943446E19f\nWas: "+DecimalFraction.FromString("-70943446332471357958").ToSingle());
      if(-7.094344633247136E19d!=DecimalFraction.FromString("-70943446332471357958").ToDouble())
        Assert.Fail("decfrac double -70943446332471357958\nExpected: -7.094344633247136E19d\nWas: "+DecimalFraction.FromString("-70943446332471357958").ToDouble());
      if(63367.23f!=DecimalFraction.FromString("63367.23157744207").ToSingle())
        Assert.Fail("decfrac single 63367.23157744207\nExpected: 63367.23f\nWas: "+DecimalFraction.FromString("63367.23157744207").ToSingle());
      if(63367.23157744207d!=DecimalFraction.FromString("63367.23157744207").ToDouble())
        Assert.Fail("decfrac double 63367.23157744207\nExpected: 63367.23157744207d\nWas: "+DecimalFraction.FromString("63367.23157744207").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("2100535E+120").ToSingle())
        Assert.Fail("decfrac single 2100535E+120\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("2100535E+120").ToSingle());
      if(2.100535E126d!=DecimalFraction.FromString("2100535E+120").ToDouble())
        Assert.Fail("decfrac double 2100535E+120\nExpected: 2.100535E126d\nWas: "+DecimalFraction.FromString("2100535E+120").ToDouble());
      if(0.0f!=DecimalFraction.FromString("914534543212037911E-174").ToSingle())
        Assert.Fail("decfrac single 914534543212037911E-174\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("914534543212037911E-174").ToSingle());
      if(9.14534543212038E-157d!=DecimalFraction.FromString("914534543212037911E-174").ToDouble())
        Assert.Fail("decfrac double 914534543212037911E-174\nExpected: 9.14534543212038E-157d\nWas: "+DecimalFraction.FromString("914534543212037911E-174").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-12437185743660570E-180").ToSingle())
        Assert.Fail("decfrac single -12437185743660570E-180\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-12437185743660570E-180").ToSingle());
      if(-1.243718574366057E-164d!=DecimalFraction.FromString("-12437185743660570E-180").ToDouble())
        Assert.Fail("decfrac double -12437185743660570E-180\nExpected: -1.243718574366057E-164d\nWas: "+DecimalFraction.FromString("-12437185743660570E-180").ToDouble());
      if(-3.3723915E19f!=DecimalFraction.FromString("-33723915695913879E+3").ToSingle())
        Assert.Fail("decfrac single -33723915695913879E+3\nExpected: -3.3723915E19f\nWas: "+DecimalFraction.FromString("-33723915695913879E+3").ToSingle());
      if(-3.3723915695913878E19d!=DecimalFraction.FromString("-33723915695913879E+3").ToDouble())
        Assert.Fail("decfrac double -33723915695913879E+3\nExpected: -3.3723915695913878E19d\nWas: "+DecimalFraction.FromString("-33723915695913879E+3").ToDouble());
      if(6.3664833E10f!=DecimalFraction.FromString("63664831787").ToSingle())
        Assert.Fail("decfrac single 63664831787\nExpected: 6.3664833E10f\nWas: "+DecimalFraction.FromString("63664831787").ToSingle());
      if(6.3664831787E10d!=DecimalFraction.FromString("63664831787").ToDouble())
        Assert.Fail("decfrac double 63664831787\nExpected: 6.3664831787E10d\nWas: "+DecimalFraction.FromString("63664831787").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("432187105445201137.3321724908E+97").ToSingle())
        Assert.Fail("decfrac single 432187105445201137.3321724908E+97\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("432187105445201137.3321724908E+97").ToSingle());
      if(4.321871054452011E114d!=DecimalFraction.FromString("432187105445201137.3321724908E+97").ToDouble())
        Assert.Fail("decfrac double 432187105445201137.3321724908E+97\nExpected: 4.321871054452011E114d\nWas: "+DecimalFraction.FromString("432187105445201137.3321724908E+97").ToDouble());
      if(-5.1953271E13f!=DecimalFraction.FromString("-51953270775979").ToSingle())
        Assert.Fail("decfrac single -51953270775979\nExpected: -5.1953271E13f\nWas: "+DecimalFraction.FromString("-51953270775979").ToSingle());
      if(-5.1953270775979E13d!=DecimalFraction.FromString("-51953270775979").ToDouble())
        Assert.Fail("decfrac double -51953270775979\nExpected: -5.1953270775979E13d\nWas: "+DecimalFraction.FromString("-51953270775979").ToDouble());
      if(2.14953088E9f!=DecimalFraction.FromString("2149530805").ToSingle())
        Assert.Fail("decfrac single 2149530805\nExpected: 2.14953088E9f\nWas: "+DecimalFraction.FromString("2149530805").ToSingle());
      if(2.149530805E9d!=DecimalFraction.FromString("2149530805").ToDouble())
        Assert.Fail("decfrac double 2149530805\nExpected: 2.149530805E9d\nWas: "+DecimalFraction.FromString("2149530805").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-4672759140.6362E-223").ToSingle())
        Assert.Fail("decfrac single -4672759140.6362E-223\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-4672759140.6362E-223").ToSingle());
      if(-4.6727591406362E-214d!=DecimalFraction.FromString("-4672759140.6362E-223").ToDouble())
        Assert.Fail("decfrac double -4672759140.6362E-223\nExpected: -4.6727591406362E-214d\nWas: "+DecimalFraction.FromString("-4672759140.6362E-223").ToDouble());
      if(-9.0f!=DecimalFraction.FromString("-9").ToSingle())
        Assert.Fail("decfrac single -9\nExpected: -9.0f\nWas: "+DecimalFraction.FromString("-9").ToSingle());
      if(-9.0d!=DecimalFraction.FromString("-9").ToDouble())
        Assert.Fail("decfrac double -9\nExpected: -9.0d\nWas: "+DecimalFraction.FromString("-9").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-1903960322936E+304").ToSingle())
        Assert.Fail("decfrac single -1903960322936E+304\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-1903960322936E+304").ToSingle());
      if(Double.NegativeInfinity!=DecimalFraction.FromString("-1903960322936E+304").ToDouble())
        Assert.Fail("decfrac double -1903960322936E+304\nExpected: Double.NegativeInfinity\nWas: "+DecimalFraction.FromString("-1903960322936E+304").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("405766405417980707E+316").ToSingle())
        Assert.Fail("decfrac single 405766405417980707E+316\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("405766405417980707E+316").ToSingle());
      if(Double.PositiveInfinity!=DecimalFraction.FromString("405766405417980707E+316").ToDouble())
        Assert.Fail("decfrac double 405766405417980707E+316\nExpected: Double.PositiveInfinity\nWas: "+DecimalFraction.FromString("405766405417980707E+316").ToDouble());
      if(-166174.94f!=DecimalFraction.FromString("-1661749343992047E-10").ToSingle())
        Assert.Fail("decfrac single -1661749343992047E-10\nExpected: -166174.94f\nWas: "+DecimalFraction.FromString("-1661749343992047E-10").ToSingle());
      if(-166174.9343992047d!=DecimalFraction.FromString("-1661749343992047E-10").ToDouble())
        Assert.Fail("decfrac double -1661749343992047E-10\nExpected: -166174.9343992047d\nWas: "+DecimalFraction.FromString("-1661749343992047E-10").ToDouble());
      if(5893094.0f!=DecimalFraction.FromString("5893094.099969899224047667").ToSingle())
        Assert.Fail("decfrac single 5893094.099969899224047667\nExpected: 5893094.0f\nWas: "+DecimalFraction.FromString("5893094.099969899224047667").ToSingle());
      if(5893094.099969899d!=DecimalFraction.FromString("5893094.099969899224047667").ToDouble())
        Assert.Fail("decfrac double 5893094.099969899224047667\nExpected: 5893094.099969899d\nWas: "+DecimalFraction.FromString("5893094.099969899224047667").ToDouble());
      if(-3.4023195E17f!=DecimalFraction.FromString("-340231946762317122").ToSingle())
        Assert.Fail("decfrac single -340231946762317122\nExpected: -3.4023195E17f\nWas: "+DecimalFraction.FromString("-340231946762317122").ToSingle());
      if(-3.4023194676231712E17d!=DecimalFraction.FromString("-340231946762317122").ToDouble())
        Assert.Fail("decfrac double -340231946762317122\nExpected: -3.4023194676231712E17d\nWas: "+DecimalFraction.FromString("-340231946762317122").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("3.10041643978E+236").ToSingle())
        Assert.Fail("decfrac single 3.10041643978E+236\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("3.10041643978E+236").ToSingle());
      if(3.10041643978E236d!=DecimalFraction.FromString("3.10041643978E+236").ToDouble())
        Assert.Fail("decfrac double 3.10041643978E+236\nExpected: 3.10041643978E236d\nWas: "+DecimalFraction.FromString("3.10041643978E+236").ToDouble());
      if(1.43429217E13f!=DecimalFraction.FromString("14342921940186").ToSingle())
        Assert.Fail("decfrac single 14342921940186\nExpected: 1.43429217E13f\nWas: "+DecimalFraction.FromString("14342921940186").ToSingle());
      if(1.4342921940186E13d!=DecimalFraction.FromString("14342921940186").ToDouble())
        Assert.Fail("decfrac double 14342921940186\nExpected: 1.4342921940186E13d\nWas: "+DecimalFraction.FromString("14342921940186").ToDouble());
      if(1.97766234E9f!=DecimalFraction.FromString("1977662368").ToSingle())
        Assert.Fail("decfrac single 1977662368\nExpected: 1.97766234E9f\nWas: "+DecimalFraction.FromString("1977662368").ToSingle());
      if(1.977662368E9d!=DecimalFraction.FromString("1977662368").ToDouble())
        Assert.Fail("decfrac double 1977662368\nExpected: 1.977662368E9d\nWas: "+DecimalFraction.FromString("1977662368").ToDouble());
      if(0.0f!=DecimalFraction.FromString("891.32009975058011674E-268").ToSingle())
        Assert.Fail("decfrac single 891.32009975058011674E-268\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("891.32009975058011674E-268").ToSingle());
      if(8.913200997505801E-266d!=DecimalFraction.FromString("891.32009975058011674E-268").ToDouble())
        Assert.Fail("decfrac double 891.32009975058011674E-268\nExpected: 8.913200997505801E-266d\nWas: "+DecimalFraction.FromString("891.32009975058011674E-268").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-895468936291.471679344983419E+316").ToSingle())
        Assert.Fail("decfrac single -895468936291.471679344983419E+316\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-895468936291.471679344983419E+316").ToSingle());
      if(Double.NegativeInfinity!=DecimalFraction.FromString("-895468936291.471679344983419E+316").ToDouble())
        Assert.Fail("decfrac double -895468936291.471679344983419E+316\nExpected: Double.NegativeInfinity\nWas: "+DecimalFraction.FromString("-895468936291.471679344983419E+316").ToDouble());
      if(0.0f!=DecimalFraction.FromString("61308E-104").ToSingle())
        Assert.Fail("decfrac single 61308E-104\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("61308E-104").ToSingle());
      if(6.1308E-100d!=DecimalFraction.FromString("61308E-104").ToDouble())
        Assert.Fail("decfrac double 61308E-104\nExpected: 6.1308E-100d\nWas: "+DecimalFraction.FromString("61308E-104").ToDouble());
      if(-5362.791f!=DecimalFraction.FromString("-5362.79122778669072").ToSingle())
        Assert.Fail("decfrac single -5362.79122778669072\nExpected: -5362.791f\nWas: "+DecimalFraction.FromString("-5362.79122778669072").ToSingle());
      if(-5362.791227786691d!=DecimalFraction.FromString("-5362.79122778669072").ToDouble())
        Assert.Fail("decfrac double -5362.79122778669072\nExpected: -5362.791227786691d\nWas: "+DecimalFraction.FromString("-5362.79122778669072").ToDouble());
      if(0.0f!=DecimalFraction.FromString("861664379590901308.23330613776542261919E-101").ToSingle())
        Assert.Fail("decfrac single 861664379590901308.23330613776542261919E-101\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("861664379590901308.23330613776542261919E-101").ToSingle());
      if(8.616643795909013E-84d!=DecimalFraction.FromString("861664379590901308.23330613776542261919E-101").ToDouble())
        Assert.Fail("decfrac double 861664379590901308.23330613776542261919E-101\nExpected: 8.616643795909013E-84d\nWas: "+DecimalFraction.FromString("861664379590901308.23330613776542261919E-101").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-1884773180.50192918329237967651E+204").ToSingle())
        Assert.Fail("decfrac single -1884773180.50192918329237967651E+204\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-1884773180.50192918329237967651E+204").ToSingle());
      if(-1.884773180501929E213d!=DecimalFraction.FromString("-1884773180.50192918329237967651E+204").ToDouble())
        Assert.Fail("decfrac double -1884773180.50192918329237967651E+204\nExpected: -1.884773180501929E213d\nWas: "+DecimalFraction.FromString("-1884773180.50192918329237967651E+204").ToDouble());
      if(1.89187207E13f!=DecimalFraction.FromString("18918720095123.6152").ToSingle())
        Assert.Fail("decfrac single 18918720095123.6152\nExpected: 1.89187207E13f\nWas: "+DecimalFraction.FromString("18918720095123.6152").ToSingle());
      if(1.8918720095123613E13d!=DecimalFraction.FromString("18918720095123.6152").ToDouble())
        Assert.Fail("decfrac double 18918720095123.6152\nExpected: 1.8918720095123613E13d\nWas: "+DecimalFraction.FromString("18918720095123.6152").ToDouble());
      if(94667.95f!=DecimalFraction.FromString("94667.95264211741602").ToSingle())
        Assert.Fail("decfrac single 94667.95264211741602\nExpected: 94667.95f\nWas: "+DecimalFraction.FromString("94667.95264211741602").ToSingle());
      if(94667.95264211742d!=DecimalFraction.FromString("94667.95264211741602").ToDouble())
        Assert.Fail("decfrac double 94667.95264211741602\nExpected: 94667.95264211742d\nWas: "+DecimalFraction.FromString("94667.95264211741602").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("1230618521424E+134").ToSingle())
        Assert.Fail("decfrac single 1230618521424E+134\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("1230618521424E+134").ToSingle());
      if(1.230618521424E146d!=DecimalFraction.FromString("1230618521424E+134").ToDouble())
        Assert.Fail("decfrac double 1230618521424E+134\nExpected: 1.230618521424E146d\nWas: "+DecimalFraction.FromString("1230618521424E+134").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("3022403935588782E+85").ToSingle())
        Assert.Fail("decfrac single 3022403935588782E+85\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("3022403935588782E+85").ToSingle());
      if(3.022403935588782E100d!=DecimalFraction.FromString("3022403935588782E+85").ToDouble())
        Assert.Fail("decfrac double 3022403935588782E+85\nExpected: 3.022403935588782E100d\nWas: "+DecimalFraction.FromString("3022403935588782E+85").ToDouble());
      if(Single.PositiveInfinity!=DecimalFraction.FromString("64543E+274").ToSingle())
        Assert.Fail("decfrac single 64543E+274\nExpected: Single.PositiveInfinity\nWas: "+DecimalFraction.FromString("64543E+274").ToSingle());
      if(6.4543E278d!=DecimalFraction.FromString("64543E+274").ToDouble())
        Assert.Fail("decfrac double 64543E+274\nExpected: 6.4543E278d\nWas: "+DecimalFraction.FromString("64543E+274").ToDouble());
      if(6.7181355E10f!=DecimalFraction.FromString("67181356837.903551518080873954").ToSingle())
        Assert.Fail("decfrac single 67181356837.903551518080873954\nExpected: 6.7181355E10f\nWas: "+DecimalFraction.FromString("67181356837.903551518080873954").ToSingle());
      if(6.718135683790355E10d!=DecimalFraction.FromString("67181356837.903551518080873954").ToDouble())
        Assert.Fail("decfrac double 67181356837.903551518080873954\nExpected: 6.718135683790355E10d\nWas: "+DecimalFraction.FromString("67181356837.903551518080873954").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-4508016E-321").ToSingle())
        Assert.Fail("decfrac single -4508016E-321\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-4508016E-321").ToSingle());
      if(-4.508016E-315d!=DecimalFraction.FromString("-4508016E-321").ToDouble())
        Assert.Fail("decfrac double -4508016E-321\nExpected: -4.508016E-315d\nWas: "+DecimalFraction.FromString("-4508016E-321").ToDouble());
      if(Single.NegativeInfinity!=DecimalFraction.FromString("-62855032520.512452348497E+39").ToSingle())
        Assert.Fail("decfrac single -62855032520.512452348497E+39\nExpected: Single.NegativeInfinity\nWas: "+DecimalFraction.FromString("-62855032520.512452348497E+39").ToSingle());
      if(-6.285503252051245E49d!=DecimalFraction.FromString("-62855032520.512452348497E+39").ToDouble())
        Assert.Fail("decfrac double -62855032520.512452348497E+39\nExpected: -6.285503252051245E49d\nWas: "+DecimalFraction.FromString("-62855032520.512452348497E+39").ToDouble());
      if(3177.2236f!=DecimalFraction.FromString("3177.2237286").ToSingle())
        Assert.Fail("decfrac single 3177.2237286\nExpected: 3177.2236f\nWas: "+DecimalFraction.FromString("3177.2237286").ToSingle());
      if(3177.2237286d!=DecimalFraction.FromString("3177.2237286").ToDouble())
        Assert.Fail("decfrac double 3177.2237286\nExpected: 3177.2237286d\nWas: "+DecimalFraction.FromString("3177.2237286").ToDouble());
      if(-7.950583E8f!=DecimalFraction.FromString("-795058316.9186492185346968").ToSingle())
        Assert.Fail("decfrac single -795058316.9186492185346968\nExpected: -7.950583E8f\nWas: "+DecimalFraction.FromString("-795058316.9186492185346968").ToSingle());
      if(-7.950583169186492E8d!=DecimalFraction.FromString("-795058316.9186492185346968").ToDouble())
        Assert.Fail("decfrac double -795058316.9186492185346968\nExpected: -7.950583169186492E8d\nWas: "+DecimalFraction.FromString("-795058316.9186492185346968").ToDouble());
    }
    
    [Test]
    public void TestDecFracIntegersToSingleDouble(){
      if(-5.7703064E7f!=DecimalFraction.FromString("-57703066").ToSingle())
        Assert.Fail("decfrac single -57703066\nExpected: -5.7703064E7f\nWas: "+DecimalFraction.FromString("-57703066").ToSingle());
      if(-5.7703066E7d!=DecimalFraction.FromString("-57703066").ToDouble())
        Assert.Fail("decfrac double -57703066\nExpected: -5.7703066E7d\nWas: "+DecimalFraction.FromString("-57703066").ToDouble());
      if(1590432.0f!=DecimalFraction.FromString("1590432").ToSingle())
        Assert.Fail("decfrac single 1590432\nExpected: 1590432.0f\nWas: "+DecimalFraction.FromString("1590432").ToSingle());
      if(1590432.0d!=DecimalFraction.FromString("1590432").ToDouble())
        Assert.Fail("decfrac double 1590432\nExpected: 1590432.0d\nWas: "+DecimalFraction.FromString("1590432").ToDouble());
      if(9.5464253E9f!=DecimalFraction.FromString("9546425267").ToSingle())
        Assert.Fail("decfrac single 9546425267\nExpected: 9.5464253E9f\nWas: "+DecimalFraction.FromString("9546425267").ToSingle());
      if(9.546425267E9d!=DecimalFraction.FromString("9546425267").ToDouble())
        Assert.Fail("decfrac double 9546425267\nExpected: 9.546425267E9d\nWas: "+DecimalFraction.FromString("9546425267").ToDouble());
      if(7.3227311E16f!=DecimalFraction.FromString("73227309698439247").ToSingle())
        Assert.Fail("decfrac single 73227309698439247\nExpected: 7.3227311E16f\nWas: "+DecimalFraction.FromString("73227309698439247").ToSingle());
      if(7.3227309698439248E16d!=DecimalFraction.FromString("73227309698439247").ToDouble())
        Assert.Fail("decfrac double 73227309698439247\nExpected: 7.3227309698439248E16d\nWas: "+DecimalFraction.FromString("73227309698439247").ToDouble());
      if(75114.0f!=DecimalFraction.FromString("75114").ToSingle())
        Assert.Fail("decfrac single 75114\nExpected: 75114.0f\nWas: "+DecimalFraction.FromString("75114").ToSingle());
      if(75114.0d!=DecimalFraction.FromString("75114").ToDouble())
        Assert.Fail("decfrac double 75114\nExpected: 75114.0d\nWas: "+DecimalFraction.FromString("75114").ToDouble());
      if(64.0f!=DecimalFraction.FromString("64").ToSingle())
        Assert.Fail("decfrac single 64\nExpected: 64.0f\nWas: "+DecimalFraction.FromString("64").ToSingle());
      if(64.0d!=DecimalFraction.FromString("64").ToDouble())
        Assert.Fail("decfrac double 64\nExpected: 64.0d\nWas: "+DecimalFraction.FromString("64").ToDouble());
      if(8.6352293E15f!=DecimalFraction.FromString("8635229353951207").ToSingle())
        Assert.Fail("decfrac single 8635229353951207\nExpected: 8.6352293E15f\nWas: "+DecimalFraction.FromString("8635229353951207").ToSingle());
      if(8.635229353951207E15d!=DecimalFraction.FromString("8635229353951207").ToDouble())
        Assert.Fail("decfrac double 8635229353951207\nExpected: 8.635229353951207E15d\nWas: "+DecimalFraction.FromString("8635229353951207").ToDouble());
      if(-8.056573E19f!=DecimalFraction.FromString("-80565729661447979457").ToSingle())
        Assert.Fail("decfrac single -80565729661447979457\nExpected: -8.056573E19f\nWas: "+DecimalFraction.FromString("-80565729661447979457").ToSingle());
      if(-8.056572966144798E19d!=DecimalFraction.FromString("-80565729661447979457").ToDouble())
        Assert.Fail("decfrac double -80565729661447979457\nExpected: -8.056572966144798E19d\nWas: "+DecimalFraction.FromString("-80565729661447979457").ToDouble());
      if(8.1540558E14f!=DecimalFraction.FromString("815405565228754").ToSingle())
        Assert.Fail("decfrac single 815405565228754\nExpected: 8.1540558E14f\nWas: "+DecimalFraction.FromString("815405565228754").ToSingle());
      if(8.15405565228754E14d!=DecimalFraction.FromString("815405565228754").ToDouble())
        Assert.Fail("decfrac double 815405565228754\nExpected: 8.15405565228754E14d\nWas: "+DecimalFraction.FromString("815405565228754").ToDouble());
      if(-6.1008438E16f!=DecimalFraction.FromString("-61008438357089231").ToSingle())
        Assert.Fail("decfrac single -61008438357089231\nExpected: -6.1008438E16f\nWas: "+DecimalFraction.FromString("-61008438357089231").ToSingle());
      if(-6.1008438357089232E16d!=DecimalFraction.FromString("-61008438357089231").ToDouble())
        Assert.Fail("decfrac double -61008438357089231\nExpected: -6.1008438357089232E16d\nWas: "+DecimalFraction.FromString("-61008438357089231").ToDouble());
      if(-46526.0f!=DecimalFraction.FromString("-46526").ToSingle())
        Assert.Fail("decfrac single -46526\nExpected: -46526.0f\nWas: "+DecimalFraction.FromString("-46526").ToSingle());
      if(-46526.0d!=DecimalFraction.FromString("-46526").ToDouble())
        Assert.Fail("decfrac double -46526\nExpected: -46526.0d\nWas: "+DecimalFraction.FromString("-46526").ToDouble());
      if(5.1199847E18f!=DecimalFraction.FromString("5119984668352258853").ToSingle())
        Assert.Fail("decfrac single 5119984668352258853\nExpected: 5.1199847E18f\nWas: "+DecimalFraction.FromString("5119984668352258853").ToSingle());
      if(5.1199846683522591E18d!=DecimalFraction.FromString("5119984668352258853").ToDouble())
        Assert.Fail("decfrac double 5119984668352258853\nExpected: 5.1199846683522591E18d\nWas: "+DecimalFraction.FromString("5119984668352258853").ToDouble());
      if(1851.0f!=DecimalFraction.FromString("1851").ToSingle())
        Assert.Fail("decfrac single 1851\nExpected: 1851.0f\nWas: "+DecimalFraction.FromString("1851").ToSingle());
      if(1851.0d!=DecimalFraction.FromString("1851").ToDouble())
        Assert.Fail("decfrac double 1851\nExpected: 1851.0d\nWas: "+DecimalFraction.FromString("1851").ToDouble());
      if(8.7587332E15f!=DecimalFraction.FromString("8758733009763848").ToSingle())
        Assert.Fail("decfrac single 8758733009763848\nExpected: 8.7587332E15f\nWas: "+DecimalFraction.FromString("8758733009763848").ToSingle());
      if(8.758733009763848E15d!=DecimalFraction.FromString("8758733009763848").ToDouble())
        Assert.Fail("decfrac double 8758733009763848\nExpected: 8.758733009763848E15d\nWas: "+DecimalFraction.FromString("8758733009763848").ToDouble());
      if(51.0f!=DecimalFraction.FromString("51").ToSingle())
        Assert.Fail("decfrac single 51\nExpected: 51.0f\nWas: "+DecimalFraction.FromString("51").ToSingle());
      if(51.0d!=DecimalFraction.FromString("51").ToDouble())
        Assert.Fail("decfrac double 51\nExpected: 51.0d\nWas: "+DecimalFraction.FromString("51").ToDouble());
      if(9.4281774E11f!=DecimalFraction.FromString("942817726107").ToSingle())
        Assert.Fail("decfrac single 942817726107\nExpected: 9.4281774E11f\nWas: "+DecimalFraction.FromString("942817726107").ToSingle());
      if(9.42817726107E11d!=DecimalFraction.FromString("942817726107").ToDouble())
        Assert.Fail("decfrac double 942817726107\nExpected: 9.42817726107E11d\nWas: "+DecimalFraction.FromString("942817726107").ToDouble());
      if(186575.0f!=DecimalFraction.FromString("186575").ToSingle())
        Assert.Fail("decfrac single 186575\nExpected: 186575.0f\nWas: "+DecimalFraction.FromString("186575").ToSingle());
      if(186575.0d!=DecimalFraction.FromString("186575").ToDouble())
        Assert.Fail("decfrac double 186575\nExpected: 186575.0d\nWas: "+DecimalFraction.FromString("186575").ToDouble());
      if(-3.47313997E9f!=DecimalFraction.FromString("-3473140020").ToSingle())
        Assert.Fail("decfrac single -3473140020\nExpected: -3.47313997E9f\nWas: "+DecimalFraction.FromString("-3473140020").ToSingle());
      if(-3.47314002E9d!=DecimalFraction.FromString("-3473140020").ToDouble())
        Assert.Fail("decfrac double -3473140020\nExpected: -3.47314002E9d\nWas: "+DecimalFraction.FromString("-3473140020").ToDouble());
      if(2.66134912E8f!=DecimalFraction.FromString("266134912").ToSingle())
        Assert.Fail("decfrac single 266134912\nExpected: 2.66134912E8f\nWas: "+DecimalFraction.FromString("266134912").ToSingle());
      if(2.66134912E8d!=DecimalFraction.FromString("266134912").ToDouble())
        Assert.Fail("decfrac double 266134912\nExpected: 2.66134912E8d\nWas: "+DecimalFraction.FromString("266134912").ToDouble());
      if(5209.0f!=DecimalFraction.FromString("5209").ToSingle())
        Assert.Fail("decfrac single 5209\nExpected: 5209.0f\nWas: "+DecimalFraction.FromString("5209").ToSingle());
      if(5209.0d!=DecimalFraction.FromString("5209").ToDouble())
        Assert.Fail("decfrac double 5209\nExpected: 5209.0d\nWas: "+DecimalFraction.FromString("5209").ToDouble());
      if(70489.0f!=DecimalFraction.FromString("70489").ToSingle())
        Assert.Fail("decfrac single 70489\nExpected: 70489.0f\nWas: "+DecimalFraction.FromString("70489").ToSingle());
      if(70489.0d!=DecimalFraction.FromString("70489").ToDouble())
        Assert.Fail("decfrac double 70489\nExpected: 70489.0d\nWas: "+DecimalFraction.FromString("70489").ToDouble());
      if(-6.1383652E14f!=DecimalFraction.FromString("-613836517344428").ToSingle())
        Assert.Fail("decfrac single -613836517344428\nExpected: -6.1383652E14f\nWas: "+DecimalFraction.FromString("-613836517344428").ToSingle());
      if(-6.13836517344428E14d!=DecimalFraction.FromString("-613836517344428").ToDouble())
        Assert.Fail("decfrac double -613836517344428\nExpected: -6.13836517344428E14d\nWas: "+DecimalFraction.FromString("-613836517344428").ToDouble());
      if(-3.47896388E16f!=DecimalFraction.FromString("-34789639317051875E0").ToSingle())
        Assert.Fail("decfrac single -34789639317051875E0\nExpected: -3.47896388E16f\nWas: "+DecimalFraction.FromString("-34789639317051875E0").ToSingle());
      if(-3.4789639317051876E16d!=DecimalFraction.FromString("-34789639317051875E0").ToDouble())
        Assert.Fail("decfrac double -34789639317051875E0\nExpected: -3.4789639317051876E16d\nWas: "+DecimalFraction.FromString("-34789639317051875E0").ToDouble());
      if(-8.4833942E13f!=DecimalFraction.FromString("-84833938642058").ToSingle())
        Assert.Fail("decfrac single -84833938642058\nExpected: -8.4833942E13f\nWas: "+DecimalFraction.FromString("-84833938642058").ToSingle());
      if(-8.4833938642058E13d!=DecimalFraction.FromString("-84833938642058").ToDouble())
        Assert.Fail("decfrac double -84833938642058\nExpected: -8.4833938642058E13d\nWas: "+DecimalFraction.FromString("-84833938642058").ToDouble());
      if(-359.0f!=DecimalFraction.FromString("-359").ToSingle())
        Assert.Fail("decfrac single -359\nExpected: -359.0f\nWas: "+DecimalFraction.FromString("-359").ToSingle());
      if(-359.0d!=DecimalFraction.FromString("-359").ToDouble())
        Assert.Fail("decfrac double -359\nExpected: -359.0d\nWas: "+DecimalFraction.FromString("-359").ToDouble());
      if(365981.0f!=DecimalFraction.FromString("365981").ToSingle())
        Assert.Fail("decfrac single 365981\nExpected: 365981.0f\nWas: "+DecimalFraction.FromString("365981").ToSingle());
      if(365981.0d!=DecimalFraction.FromString("365981").ToDouble())
        Assert.Fail("decfrac double 365981\nExpected: 365981.0d\nWas: "+DecimalFraction.FromString("365981").ToDouble());
      if(9103.0f!=DecimalFraction.FromString("9103").ToSingle())
        Assert.Fail("decfrac single 9103\nExpected: 9103.0f\nWas: "+DecimalFraction.FromString("9103").ToSingle());
      if(9103.0d!=DecimalFraction.FromString("9103").ToDouble())
        Assert.Fail("decfrac double 9103\nExpected: 9103.0d\nWas: "+DecimalFraction.FromString("9103").ToDouble());
      if(9.822906E11f!=DecimalFraction.FromString("982290625898").ToSingle())
        Assert.Fail("decfrac single 982290625898\nExpected: 9.822906E11f\nWas: "+DecimalFraction.FromString("982290625898").ToSingle());
      if(9.82290625898E11d!=DecimalFraction.FromString("982290625898").ToDouble())
        Assert.Fail("decfrac double 982290625898\nExpected: 9.82290625898E11d\nWas: "+DecimalFraction.FromString("982290625898").ToDouble());
      if(11.0f!=DecimalFraction.FromString("11").ToSingle())
        Assert.Fail("decfrac single 11\nExpected: 11.0f\nWas: "+DecimalFraction.FromString("11").ToSingle());
      if(11.0d!=DecimalFraction.FromString("11").ToDouble())
        Assert.Fail("decfrac double 11\nExpected: 11.0d\nWas: "+DecimalFraction.FromString("11").ToDouble());
      if(-2823.0f!=DecimalFraction.FromString("-2823").ToSingle())
        Assert.Fail("decfrac single -2823\nExpected: -2823.0f\nWas: "+DecimalFraction.FromString("-2823").ToSingle());
      if(-2823.0d!=DecimalFraction.FromString("-2823").ToDouble())
        Assert.Fail("decfrac double -2823\nExpected: -2823.0d\nWas: "+DecimalFraction.FromString("-2823").ToDouble());
      if(1.5945044E10f!=DecimalFraction.FromString("15945044029").ToSingle())
        Assert.Fail("decfrac single 15945044029\nExpected: 1.5945044E10f\nWas: "+DecimalFraction.FromString("15945044029").ToSingle());
      if(1.5945044029E10d!=DecimalFraction.FromString("15945044029").ToDouble())
        Assert.Fail("decfrac double 15945044029\nExpected: 1.5945044029E10d\nWas: "+DecimalFraction.FromString("15945044029").ToDouble());
      if(-1.69193578E18f!=DecimalFraction.FromString("-1691935711084975329").ToSingle())
        Assert.Fail("decfrac single -1691935711084975329\nExpected: -1.69193578E18f\nWas: "+DecimalFraction.FromString("-1691935711084975329").ToSingle());
      if(-1.69193571108497536E18d!=DecimalFraction.FromString("-1691935711084975329").ToDouble())
        Assert.Fail("decfrac double -1691935711084975329\nExpected: -1.69193571108497536E18d\nWas: "+DecimalFraction.FromString("-1691935711084975329").ToDouble());
      if(611.0f!=DecimalFraction.FromString("611").ToSingle())
        Assert.Fail("decfrac single 611\nExpected: 611.0f\nWas: "+DecimalFraction.FromString("611").ToSingle());
      if(611.0d!=DecimalFraction.FromString("611").ToDouble())
        Assert.Fail("decfrac double 611\nExpected: 611.0d\nWas: "+DecimalFraction.FromString("611").ToDouble());
      if(8.1338793E9f!=DecimalFraction.FromString("8133879260").ToSingle())
        Assert.Fail("decfrac single 8133879260\nExpected: 8.1338793E9f\nWas: "+DecimalFraction.FromString("8133879260").ToSingle());
      if(8.13387926E9d!=DecimalFraction.FromString("8133879260").ToDouble())
        Assert.Fail("decfrac double 8133879260\nExpected: 8.13387926E9d\nWas: "+DecimalFraction.FromString("8133879260").ToDouble());
      if(7.8632614E13f!=DecimalFraction.FromString("78632613962905").ToSingle())
        Assert.Fail("decfrac single 78632613962905\nExpected: 7.8632614E13f\nWas: "+DecimalFraction.FromString("78632613962905").ToSingle());
      if(7.8632613962905E13d!=DecimalFraction.FromString("78632613962905").ToDouble())
        Assert.Fail("decfrac double 78632613962905\nExpected: 7.8632613962905E13d\nWas: "+DecimalFraction.FromString("78632613962905").ToDouble());
      if(8.686342E19f!=DecimalFraction.FromString("86863421212032782386").ToSingle())
        Assert.Fail("decfrac single 86863421212032782386\nExpected: 8.686342E19f\nWas: "+DecimalFraction.FromString("86863421212032782386").ToSingle());
      if(8.686342121203278E19d!=DecimalFraction.FromString("86863421212032782386").ToDouble())
        Assert.Fail("decfrac double 86863421212032782386\nExpected: 8.686342121203278E19d\nWas: "+DecimalFraction.FromString("86863421212032782386").ToDouble());
      if(2.46595376E8f!=DecimalFraction.FromString("246595381").ToSingle())
        Assert.Fail("decfrac single 246595381\nExpected: 2.46595376E8f\nWas: "+DecimalFraction.FromString("246595381").ToSingle());
      if(2.46595381E8d!=DecimalFraction.FromString("246595381").ToDouble())
        Assert.Fail("decfrac double 246595381\nExpected: 2.46595381E8d\nWas: "+DecimalFraction.FromString("246595381").ToDouble());
      if(5.128928E16f!=DecimalFraction.FromString("51289277641921518E0").ToSingle())
        Assert.Fail("decfrac single 51289277641921518E0\nExpected: 5.128928E16f\nWas: "+DecimalFraction.FromString("51289277641921518E0").ToSingle());
      if(5.128927764192152E16d!=DecimalFraction.FromString("51289277641921518E0").ToDouble())
        Assert.Fail("decfrac double 51289277641921518E0\nExpected: 5.128927764192152E16d\nWas: "+DecimalFraction.FromString("51289277641921518E0").ToDouble());
      if(41105.0f!=DecimalFraction.FromString("41105").ToSingle())
        Assert.Fail("decfrac single 41105\nExpected: 41105.0f\nWas: "+DecimalFraction.FromString("41105").ToSingle());
      if(41105.0d!=DecimalFraction.FromString("41105").ToDouble())
        Assert.Fail("decfrac double 41105\nExpected: 41105.0d\nWas: "+DecimalFraction.FromString("41105").ToDouble());
      if(4.5854699E16f!=DecimalFraction.FromString("45854697039925162E0").ToSingle())
        Assert.Fail("decfrac single 45854697039925162E0\nExpected: 4.5854699E16f\nWas: "+DecimalFraction.FromString("45854697039925162E0").ToSingle());
      if(4.585469703992516E16d!=DecimalFraction.FromString("45854697039925162E0").ToDouble())
        Assert.Fail("decfrac double 45854697039925162E0\nExpected: 4.585469703992516E16d\nWas: "+DecimalFraction.FromString("45854697039925162E0").ToDouble());
      if(357.0f!=DecimalFraction.FromString("357").ToSingle())
        Assert.Fail("decfrac single 357\nExpected: 357.0f\nWas: "+DecimalFraction.FromString("357").ToSingle());
      if(357.0d!=DecimalFraction.FromString("357").ToDouble())
        Assert.Fail("decfrac double 357\nExpected: 357.0d\nWas: "+DecimalFraction.FromString("357").ToDouble());
      if(4055.0f!=DecimalFraction.FromString("4055").ToSingle())
        Assert.Fail("decfrac single 4055\nExpected: 4055.0f\nWas: "+DecimalFraction.FromString("4055").ToSingle());
      if(4055.0d!=DecimalFraction.FromString("4055").ToDouble())
        Assert.Fail("decfrac double 4055\nExpected: 4055.0d\nWas: "+DecimalFraction.FromString("4055").ToDouble());
      if(-75211.0f!=DecimalFraction.FromString("-75211").ToSingle())
        Assert.Fail("decfrac single -75211\nExpected: -75211.0f\nWas: "+DecimalFraction.FromString("-75211").ToSingle());
      if(-75211.0d!=DecimalFraction.FromString("-75211").ToDouble())
        Assert.Fail("decfrac double -75211\nExpected: -75211.0d\nWas: "+DecimalFraction.FromString("-75211").ToDouble());
      if(-8.718763E19f!=DecimalFraction.FromString("-87187631416675804676").ToSingle())
        Assert.Fail("decfrac single -87187631416675804676\nExpected: -8.718763E19f\nWas: "+DecimalFraction.FromString("-87187631416675804676").ToSingle());
      if(-8.718763141667581E19d!=DecimalFraction.FromString("-87187631416675804676").ToDouble())
        Assert.Fail("decfrac double -87187631416675804676\nExpected: -8.718763141667581E19d\nWas: "+DecimalFraction.FromString("-87187631416675804676").ToDouble());
      if(-5.6423271E13f!=DecimalFraction.FromString("-56423269820314").ToSingle())
        Assert.Fail("decfrac single -56423269820314\nExpected: -5.6423271E13f\nWas: "+DecimalFraction.FromString("-56423269820314").ToSingle());
      if(-5.6423269820314E13d!=DecimalFraction.FromString("-56423269820314").ToDouble())
        Assert.Fail("decfrac double -56423269820314\nExpected: -5.6423269820314E13d\nWas: "+DecimalFraction.FromString("-56423269820314").ToDouble());
      if(-884958.0f!=DecimalFraction.FromString("-884958").ToSingle())
        Assert.Fail("decfrac single -884958\nExpected: -884958.0f\nWas: "+DecimalFraction.FromString("-884958").ToSingle());
      if(-884958.0d!=DecimalFraction.FromString("-884958").ToDouble())
        Assert.Fail("decfrac double -884958\nExpected: -884958.0d\nWas: "+DecimalFraction.FromString("-884958").ToDouble());
      if(-9.5231607E11f!=DecimalFraction.FromString("-952316071356").ToSingle())
        Assert.Fail("decfrac single -952316071356\nExpected: -9.5231607E11f\nWas: "+DecimalFraction.FromString("-952316071356").ToSingle());
      if(-9.52316071356E11d!=DecimalFraction.FromString("-952316071356").ToDouble())
        Assert.Fail("decfrac double -952316071356\nExpected: -9.52316071356E11d\nWas: "+DecimalFraction.FromString("-952316071356").ToDouble());
      if(1.07800844E17f!=DecimalFraction.FromString("107800846902684870").ToSingle())
        Assert.Fail("decfrac single 107800846902684870\nExpected: 1.07800844E17f\nWas: "+DecimalFraction.FromString("107800846902684870").ToSingle());
      if(1.07800846902684864E17d!=DecimalFraction.FromString("107800846902684870").ToDouble())
        Assert.Fail("decfrac double 107800846902684870\nExpected: 1.07800846902684864E17d\nWas: "+DecimalFraction.FromString("107800846902684870").ToDouble());
      if(-8.1588551E18f!=DecimalFraction.FromString("-8158855313340166027").ToSingle())
        Assert.Fail("decfrac single -8158855313340166027\nExpected: -8.1588551E18f\nWas: "+DecimalFraction.FromString("-8158855313340166027").ToSingle());
      if(-8.1588553133401661E18d!=DecimalFraction.FromString("-8158855313340166027").ToDouble())
        Assert.Fail("decfrac double -8158855313340166027\nExpected: -8.1588553133401661E18d\nWas: "+DecimalFraction.FromString("-8158855313340166027").ToDouble());
      if(1.52743454E18f!=DecimalFraction.FromString("1527434477600178421").ToSingle())
        Assert.Fail("decfrac single 1527434477600178421\nExpected: 1.52743454E18f\nWas: "+DecimalFraction.FromString("1527434477600178421").ToSingle());
      if(1.52743447760017843E18d!=DecimalFraction.FromString("1527434477600178421").ToDouble())
        Assert.Fail("decfrac double 1527434477600178421\nExpected: 1.52743447760017843E18d\nWas: "+DecimalFraction.FromString("1527434477600178421").ToDouble());
      if(-1.25374015E15f!=DecimalFraction.FromString("-1253740164504924").ToSingle())
        Assert.Fail("decfrac single -1253740164504924\nExpected: -1.25374015E15f\nWas: "+DecimalFraction.FromString("-1253740164504924").ToSingle());
      if(-1.253740164504924E15d!=DecimalFraction.FromString("-1253740164504924").ToDouble())
        Assert.Fail("decfrac double -1253740164504924\nExpected: -1.253740164504924E15d\nWas: "+DecimalFraction.FromString("-1253740164504924").ToDouble());
      if(9.333153E10f!=DecimalFraction.FromString("93331529453").ToSingle())
        Assert.Fail("decfrac single 93331529453\nExpected: 9.333153E10f\nWas: "+DecimalFraction.FromString("93331529453").ToSingle());
      if(9.3331529453E10d!=DecimalFraction.FromString("93331529453").ToDouble())
        Assert.Fail("decfrac double 93331529453\nExpected: 9.3331529453E10d\nWas: "+DecimalFraction.FromString("93331529453").ToDouble());
      if(-26195.0f!=DecimalFraction.FromString("-26195").ToSingle())
        Assert.Fail("decfrac single -26195\nExpected: -26195.0f\nWas: "+DecimalFraction.FromString("-26195").ToSingle());
      if(-26195.0d!=DecimalFraction.FromString("-26195").ToDouble())
        Assert.Fail("decfrac double -26195\nExpected: -26195.0d\nWas: "+DecimalFraction.FromString("-26195").ToDouble());
      if(-369.0f!=DecimalFraction.FromString("-369").ToSingle())
        Assert.Fail("decfrac single -369\nExpected: -369.0f\nWas: "+DecimalFraction.FromString("-369").ToSingle());
      if(-369.0d!=DecimalFraction.FromString("-369").ToDouble())
        Assert.Fail("decfrac double -369\nExpected: -369.0d\nWas: "+DecimalFraction.FromString("-369").ToDouble());
      if(-831.0f!=DecimalFraction.FromString("-831").ToSingle())
        Assert.Fail("decfrac single -831\nExpected: -831.0f\nWas: "+DecimalFraction.FromString("-831").ToSingle());
      if(-831.0d!=DecimalFraction.FromString("-831").ToDouble())
        Assert.Fail("decfrac double -831\nExpected: -831.0d\nWas: "+DecimalFraction.FromString("-831").ToDouble());
      if(4.11190218E12f!=DecimalFraction.FromString("4111902130704").ToSingle())
        Assert.Fail("decfrac single 4111902130704\nExpected: 4.11190218E12f\nWas: "+DecimalFraction.FromString("4111902130704").ToSingle());
      if(4.111902130704E12d!=DecimalFraction.FromString("4111902130704").ToDouble())
        Assert.Fail("decfrac double 4111902130704\nExpected: 4.111902130704E12d\nWas: "+DecimalFraction.FromString("4111902130704").ToDouble());
      if(-7.419975E34f!=DecimalFraction.FromString("-7419975014712636689.1578027201500774E+16").ToSingle())
        Assert.Fail("decfrac single -7419975014712636689.1578027201500774E+16\nExpected: -7.419975E34f\nWas: "+DecimalFraction.FromString("-7419975014712636689.1578027201500774E+16").ToSingle());
      if(-7.419975014712636E34d!=DecimalFraction.FromString("-7419975014712636689.1578027201500774E+16").ToDouble())
        Assert.Fail("decfrac double -7419975014712636689.1578027201500774E+16\nExpected: -7.419975014712636E34d\nWas: "+DecimalFraction.FromString("-7419975014712636689.1578027201500774E+16").ToDouble());
      if(-1.7915818E7f!=DecimalFraction.FromString("-17915818").ToSingle())
        Assert.Fail("decfrac single -17915818\nExpected: -1.7915818E7f\nWas: "+DecimalFraction.FromString("-17915818").ToSingle());
      if(-1.7915818E7d!=DecimalFraction.FromString("-17915818").ToDouble())
        Assert.Fail("decfrac double -17915818\nExpected: -1.7915818E7d\nWas: "+DecimalFraction.FromString("-17915818").ToDouble());
      if(-122.0f!=DecimalFraction.FromString("-122").ToSingle())
        Assert.Fail("decfrac single -122\nExpected: -122.0f\nWas: "+DecimalFraction.FromString("-122").ToSingle());
      if(-122.0d!=DecimalFraction.FromString("-122").ToDouble())
        Assert.Fail("decfrac double -122\nExpected: -122.0d\nWas: "+DecimalFraction.FromString("-122").ToDouble());
      if(-363975.0f!=DecimalFraction.FromString("-363975").ToSingle())
        Assert.Fail("decfrac single -363975\nExpected: -363975.0f\nWas: "+DecimalFraction.FromString("-363975").ToSingle());
      if(-363975.0d!=DecimalFraction.FromString("-363975").ToDouble())
        Assert.Fail("decfrac double -363975\nExpected: -363975.0d\nWas: "+DecimalFraction.FromString("-363975").ToDouble());
      if(3.22466716E12f!=DecimalFraction.FromString("3224667065103").ToSingle())
        Assert.Fail("decfrac single 3224667065103\nExpected: 3.22466716E12f\nWas: "+DecimalFraction.FromString("3224667065103").ToSingle());
      if(3.224667065103E12d!=DecimalFraction.FromString("3224667065103").ToDouble())
        Assert.Fail("decfrac double 3224667065103\nExpected: 3.224667065103E12d\nWas: "+DecimalFraction.FromString("3224667065103").ToDouble());
      if(-9.6666224E7f!=DecimalFraction.FromString("-96666228").ToSingle())
        Assert.Fail("decfrac single -96666228\nExpected: -9.6666224E7f\nWas: "+DecimalFraction.FromString("-96666228").ToSingle());
      if(-9.6666228E7d!=DecimalFraction.FromString("-96666228").ToDouble())
        Assert.Fail("decfrac double -96666228\nExpected: -9.6666228E7d\nWas: "+DecimalFraction.FromString("-96666228").ToDouble());
      if(-6.3737765E19f!=DecimalFraction.FromString("-63737764614634686933").ToSingle())
        Assert.Fail("decfrac single -63737764614634686933\nExpected: -6.3737765E19f\nWas: "+DecimalFraction.FromString("-63737764614634686933").ToSingle());
      if(-6.3737764614634684E19d!=DecimalFraction.FromString("-63737764614634686933").ToDouble())
        Assert.Fail("decfrac double -63737764614634686933\nExpected: -6.3737764614634684E19d\nWas: "+DecimalFraction.FromString("-63737764614634686933").ToDouble());
      if(-45065.0f!=DecimalFraction.FromString("-45065").ToSingle())
        Assert.Fail("decfrac single -45065\nExpected: -45065.0f\nWas: "+DecimalFraction.FromString("-45065").ToSingle());
      if(-45065.0d!=DecimalFraction.FromString("-45065").ToDouble())
        Assert.Fail("decfrac double -45065\nExpected: -45065.0d\nWas: "+DecimalFraction.FromString("-45065").ToDouble());
      if(18463.0f!=DecimalFraction.FromString("18463").ToSingle())
        Assert.Fail("decfrac single 18463\nExpected: 18463.0f\nWas: "+DecimalFraction.FromString("18463").ToSingle());
      if(18463.0d!=DecimalFraction.FromString("18463").ToDouble())
        Assert.Fail("decfrac double 18463\nExpected: 18463.0d\nWas: "+DecimalFraction.FromString("18463").ToDouble());
      if(-5.2669409E15f!=DecimalFraction.FromString("-5266940927335870").ToSingle())
        Assert.Fail("decfrac single -5266940927335870\nExpected: -5.2669409E15f\nWas: "+DecimalFraction.FromString("-5266940927335870").ToSingle());
      if(-5.26694092733587E15d!=DecimalFraction.FromString("-5266940927335870").ToDouble())
        Assert.Fail("decfrac double -5266940927335870\nExpected: -5.26694092733587E15d\nWas: "+DecimalFraction.FromString("-5266940927335870").ToDouble());
      if(-3.61275925E15f!=DecimalFraction.FromString("-3612759343074710").ToSingle())
        Assert.Fail("decfrac single -3612759343074710\nExpected: -3.61275925E15f\nWas: "+DecimalFraction.FromString("-3612759343074710").ToSingle());
      if(-3.61275934307471E15d!=DecimalFraction.FromString("-3612759343074710").ToDouble())
        Assert.Fail("decfrac double -3612759343074710\nExpected: -3.61275934307471E15d\nWas: "+DecimalFraction.FromString("-3612759343074710").ToDouble());
      if(-1.49784412E11f!=DecimalFraction.FromString("-149784410976").ToSingle())
        Assert.Fail("decfrac single -149784410976\nExpected: -1.49784412E11f\nWas: "+DecimalFraction.FromString("-149784410976").ToSingle());
      if(-1.49784410976E11d!=DecimalFraction.FromString("-149784410976").ToDouble())
        Assert.Fail("decfrac double -149784410976\nExpected: -1.49784410976E11d\nWas: "+DecimalFraction.FromString("-149784410976").ToDouble());
      if(-1.01285276E17f!=DecimalFraction.FromString("-101285275020696035").ToSingle())
        Assert.Fail("decfrac single -101285275020696035\nExpected: -1.01285276E17f\nWas: "+DecimalFraction.FromString("-101285275020696035").ToSingle());
      if(-1.01285275020696032E17d!=DecimalFraction.FromString("-101285275020696035").ToDouble())
        Assert.Fail("decfrac double -101285275020696035\nExpected: -1.01285275020696032E17d\nWas: "+DecimalFraction.FromString("-101285275020696035").ToDouble());
      if(-34.0f!=DecimalFraction.FromString("-34").ToSingle())
        Assert.Fail("decfrac single -34\nExpected: -34.0f\nWas: "+DecimalFraction.FromString("-34").ToSingle());
      if(-34.0d!=DecimalFraction.FromString("-34").ToDouble())
        Assert.Fail("decfrac double -34\nExpected: -34.0d\nWas: "+DecimalFraction.FromString("-34").ToDouble());
      if(-6.9963739E17f!=DecimalFraction.FromString("-699637360432542026").ToSingle())
        Assert.Fail("decfrac single -699637360432542026\nExpected: -6.9963739E17f\nWas: "+DecimalFraction.FromString("-699637360432542026").ToSingle());
      if(-6.9963736043254208E17d!=DecimalFraction.FromString("-699637360432542026").ToDouble())
        Assert.Fail("decfrac double -699637360432542026\nExpected: -6.9963736043254208E17d\nWas: "+DecimalFraction.FromString("-699637360432542026").ToDouble());
      if(-8131.0f!=DecimalFraction.FromString("-8131").ToSingle())
        Assert.Fail("decfrac single -8131\nExpected: -8131.0f\nWas: "+DecimalFraction.FromString("-8131").ToSingle());
      if(-8131.0d!=DecimalFraction.FromString("-8131").ToDouble())
        Assert.Fail("decfrac double -8131\nExpected: -8131.0d\nWas: "+DecimalFraction.FromString("-8131").ToDouble());
      if(6.1692147E8f!=DecimalFraction.FromString("616921472").ToSingle())
        Assert.Fail("decfrac single 616921472\nExpected: 6.1692147E8f\nWas: "+DecimalFraction.FromString("616921472").ToSingle());
      if(6.16921472E8d!=DecimalFraction.FromString("616921472").ToDouble())
        Assert.Fail("decfrac double 616921472\nExpected: 6.16921472E8d\nWas: "+DecimalFraction.FromString("616921472").ToDouble());
      if(447272.0f!=DecimalFraction.FromString("447272").ToSingle())
        Assert.Fail("decfrac single 447272\nExpected: 447272.0f\nWas: "+DecimalFraction.FromString("447272").ToSingle());
      if(447272.0d!=DecimalFraction.FromString("447272").ToDouble())
        Assert.Fail("decfrac double 447272\nExpected: 447272.0d\nWas: "+DecimalFraction.FromString("447272").ToDouble());
      if(9.719524E17f!=DecimalFraction.FromString("971952376640924713").ToSingle())
        Assert.Fail("decfrac single 971952376640924713\nExpected: 9.719524E17f\nWas: "+DecimalFraction.FromString("971952376640924713").ToSingle());
      if(9.7195237664092467E17d!=DecimalFraction.FromString("971952376640924713").ToDouble())
        Assert.Fail("decfrac double 971952376640924713\nExpected: 9.7195237664092467E17d\nWas: "+DecimalFraction.FromString("971952376640924713").ToDouble());
      if(-8622.0f!=DecimalFraction.FromString("-8622").ToSingle())
        Assert.Fail("decfrac single -8622\nExpected: -8622.0f\nWas: "+DecimalFraction.FromString("-8622").ToSingle());
      if(-8622.0d!=DecimalFraction.FromString("-8622").ToDouble())
        Assert.Fail("decfrac double -8622\nExpected: -8622.0d\nWas: "+DecimalFraction.FromString("-8622").ToDouble());
      if(-9.8425534E13f!=DecimalFraction.FromString("-98425536547570").ToSingle())
        Assert.Fail("decfrac single -98425536547570\nExpected: -9.8425534E13f\nWas: "+DecimalFraction.FromString("-98425536547570").ToSingle());
      if(-9.842553654757E13d!=DecimalFraction.FromString("-98425536547570").ToDouble())
        Assert.Fail("decfrac double -98425536547570\nExpected: -9.842553654757E13d\nWas: "+DecimalFraction.FromString("-98425536547570").ToDouble());
      if(-1.3578545E14f!=DecimalFraction.FromString("-135785450228746").ToSingle())
        Assert.Fail("decfrac single -135785450228746\nExpected: -1.3578545E14f\nWas: "+DecimalFraction.FromString("-135785450228746").ToSingle());
      if(-1.35785450228746E14d!=DecimalFraction.FromString("-135785450228746").ToDouble())
        Assert.Fail("decfrac double -135785450228746\nExpected: -1.35785450228746E14d\nWas: "+DecimalFraction.FromString("-135785450228746").ToDouble());
      if(935.0f!=DecimalFraction.FromString("935").ToSingle())
        Assert.Fail("decfrac single 935\nExpected: 935.0f\nWas: "+DecimalFraction.FromString("935").ToSingle());
      if(935.0d!=DecimalFraction.FromString("935").ToDouble())
        Assert.Fail("decfrac double 935\nExpected: 935.0d\nWas: "+DecimalFraction.FromString("935").ToDouble());
      if(-7890.0f!=DecimalFraction.FromString("-7890E0").ToSingle())
        Assert.Fail("decfrac single -7890E0\nExpected: -7890.0f\nWas: "+DecimalFraction.FromString("-7890E0").ToSingle());
      if(-7890.0d!=DecimalFraction.FromString("-7890E0").ToDouble())
        Assert.Fail("decfrac double -7890E0\nExpected: -7890.0d\nWas: "+DecimalFraction.FromString("-7890E0").ToDouble());
      if(4.5492643E12f!=DecimalFraction.FromString("45.49264316782E+11").ToSingle())
        Assert.Fail("decfrac single 45.49264316782E+11\nExpected: 4.5492643E12f\nWas: "+DecimalFraction.FromString("45.49264316782E+11").ToSingle());
      if(4.549264316782E12d!=DecimalFraction.FromString("45.49264316782E+11").ToDouble())
        Assert.Fail("decfrac double 45.49264316782E+11\nExpected: 4.549264316782E12d\nWas: "+DecimalFraction.FromString("45.49264316782E+11").ToDouble());
      if(-7684.0f!=DecimalFraction.FromString("-7684").ToSingle())
        Assert.Fail("decfrac single -7684\nExpected: -7684.0f\nWas: "+DecimalFraction.FromString("-7684").ToSingle());
      if(-7684.0d!=DecimalFraction.FromString("-7684").ToDouble())
        Assert.Fail("decfrac double -7684\nExpected: -7684.0d\nWas: "+DecimalFraction.FromString("-7684").ToDouble());
      if(734069.0f!=DecimalFraction.FromString("734069").ToSingle())
        Assert.Fail("decfrac single 734069\nExpected: 734069.0f\nWas: "+DecimalFraction.FromString("734069").ToSingle());
      if(734069.0d!=DecimalFraction.FromString("734069").ToDouble())
        Assert.Fail("decfrac double 734069\nExpected: 734069.0d\nWas: "+DecimalFraction.FromString("734069").ToDouble());
      if(-3.51801573E12f!=DecimalFraction.FromString("-3518015796477").ToSingle())
        Assert.Fail("decfrac single -3518015796477\nExpected: -3.51801573E12f\nWas: "+DecimalFraction.FromString("-3518015796477").ToSingle());
      if(-3.518015796477E12d!=DecimalFraction.FromString("-3518015796477").ToDouble())
        Assert.Fail("decfrac double -3518015796477\nExpected: -3.518015796477E12d\nWas: "+DecimalFraction.FromString("-3518015796477").ToDouble());
      if(-411720.0f!=DecimalFraction.FromString("-411720").ToSingle())
        Assert.Fail("decfrac single -411720\nExpected: -411720.0f\nWas: "+DecimalFraction.FromString("-411720").ToSingle());
      if(-411720.0d!=DecimalFraction.FromString("-411720").ToDouble())
        Assert.Fail("decfrac double -411720\nExpected: -411720.0d\nWas: "+DecimalFraction.FromString("-411720").ToDouble());
      if(5.14432512E8f!=DecimalFraction.FromString("514432504").ToSingle())
        Assert.Fail("decfrac single 514432504\nExpected: 5.14432512E8f\nWas: "+DecimalFraction.FromString("514432504").ToSingle());
      if(5.14432504E8d!=DecimalFraction.FromString("514432504").ToDouble())
        Assert.Fail("decfrac double 514432504\nExpected: 5.14432504E8d\nWas: "+DecimalFraction.FromString("514432504").ToDouble());
      if(3970.0f!=DecimalFraction.FromString("3970").ToSingle())
        Assert.Fail("decfrac single 3970\nExpected: 3970.0f\nWas: "+DecimalFraction.FromString("3970").ToSingle());
      if(3970.0d!=DecimalFraction.FromString("3970").ToDouble())
        Assert.Fail("decfrac double 3970\nExpected: 3970.0d\nWas: "+DecimalFraction.FromString("3970").ToDouble());
      if(-1.89642527E10f!=DecimalFraction.FromString("-18964252847").ToSingle())
        Assert.Fail("decfrac single -18964252847\nExpected: -1.89642527E10f\nWas: "+DecimalFraction.FromString("-18964252847").ToSingle());
      if(-1.8964252847E10d!=DecimalFraction.FromString("-18964252847").ToDouble())
        Assert.Fail("decfrac double -18964252847\nExpected: -1.8964252847E10d\nWas: "+DecimalFraction.FromString("-18964252847").ToDouble());
      if(-9.5766118E10f!=DecimalFraction.FromString("-95766116842").ToSingle())
        Assert.Fail("decfrac single -95766116842\nExpected: -9.5766118E10f\nWas: "+DecimalFraction.FromString("-95766116842").ToSingle());
      if(-9.5766116842E10d!=DecimalFraction.FromString("-95766116842").ToDouble())
        Assert.Fail("decfrac double -95766116842\nExpected: -9.5766116842E10d\nWas: "+DecimalFraction.FromString("-95766116842").ToDouble());
      if(-4.5759559E15f!=DecimalFraction.FromString("-4575956051893063").ToSingle())
        Assert.Fail("decfrac single -4575956051893063\nExpected: -4.5759559E15f\nWas: "+DecimalFraction.FromString("-4575956051893063").ToSingle());
      if(-4.575956051893063E15d!=DecimalFraction.FromString("-4575956051893063").ToDouble())
        Assert.Fail("decfrac double -4575956051893063\nExpected: -4.575956051893063E15d\nWas: "+DecimalFraction.FromString("-4575956051893063").ToDouble());
      if(5.2050934E9f!=DecimalFraction.FromString("5205093392").ToSingle())
        Assert.Fail("decfrac single 5205093392\nExpected: 5.2050934E9f\nWas: "+DecimalFraction.FromString("5205093392").ToSingle());
      if(5.205093392E9d!=DecimalFraction.FromString("5205093392").ToDouble())
        Assert.Fail("decfrac double 5205093392\nExpected: 5.205093392E9d\nWas: "+DecimalFraction.FromString("5205093392").ToDouble());
      if(-7.0079627E12f!=DecimalFraction.FromString("-7007962583042").ToSingle())
        Assert.Fail("decfrac single -7007962583042\nExpected: -7.0079627E12f\nWas: "+DecimalFraction.FromString("-7007962583042").ToSingle());
      if(-7.007962583042E12d!=DecimalFraction.FromString("-7007962583042").ToDouble())
        Assert.Fail("decfrac double -7007962583042\nExpected: -7.007962583042E12d\nWas: "+DecimalFraction.FromString("-7007962583042").ToDouble());
      if(59.0f!=DecimalFraction.FromString("59").ToSingle())
        Assert.Fail("decfrac single 59\nExpected: 59.0f\nWas: "+DecimalFraction.FromString("59").ToSingle());
      if(59.0d!=DecimalFraction.FromString("59").ToDouble())
        Assert.Fail("decfrac double 59\nExpected: 59.0d\nWas: "+DecimalFraction.FromString("59").ToDouble());
      if(-5.5095849E16f!=DecimalFraction.FromString("-55095850956259910").ToSingle())
        Assert.Fail("decfrac single -55095850956259910\nExpected: -5.5095849E16f\nWas: "+DecimalFraction.FromString("-55095850956259910").ToSingle());
      if(-5.5095850956259912E16d!=DecimalFraction.FromString("-55095850956259910").ToDouble())
        Assert.Fail("decfrac double -55095850956259910\nExpected: -5.5095850956259912E16d\nWas: "+DecimalFraction.FromString("-55095850956259910").ToDouble());
      if(1.0f!=DecimalFraction.FromString("1").ToSingle())
        Assert.Fail("decfrac single 1\nExpected: 1.0f\nWas: "+DecimalFraction.FromString("1").ToSingle());
      if(1.0d!=DecimalFraction.FromString("1").ToDouble())
        Assert.Fail("decfrac double 1\nExpected: 1.0d\nWas: "+DecimalFraction.FromString("1").ToDouble());
      if(598.0f!=DecimalFraction.FromString("598").ToSingle())
        Assert.Fail("decfrac single 598\nExpected: 598.0f\nWas: "+DecimalFraction.FromString("598").ToSingle());
      if(598.0d!=DecimalFraction.FromString("598").ToDouble())
        Assert.Fail("decfrac double 598\nExpected: 598.0d\nWas: "+DecimalFraction.FromString("598").ToDouble());
      if(957.0f!=DecimalFraction.FromString("957").ToSingle())
        Assert.Fail("decfrac single 957\nExpected: 957.0f\nWas: "+DecimalFraction.FromString("957").ToSingle());
      if(957.0d!=DecimalFraction.FromString("957").ToDouble())
        Assert.Fail("decfrac double 957\nExpected: 957.0d\nWas: "+DecimalFraction.FromString("957").ToDouble());
      if(-1.4772274E7f!=DecimalFraction.FromString("-14772274").ToSingle())
        Assert.Fail("decfrac single -14772274\nExpected: -1.4772274E7f\nWas: "+DecimalFraction.FromString("-14772274").ToSingle());
      if(-1.4772274E7d!=DecimalFraction.FromString("-14772274").ToDouble())
        Assert.Fail("decfrac double -14772274\nExpected: -1.4772274E7d\nWas: "+DecimalFraction.FromString("-14772274").ToDouble());
      if(-3006.0f!=DecimalFraction.FromString("-3006").ToSingle())
        Assert.Fail("decfrac single -3006\nExpected: -3006.0f\nWas: "+DecimalFraction.FromString("-3006").ToSingle());
      if(-3006.0d!=DecimalFraction.FromString("-3006").ToDouble())
        Assert.Fail("decfrac double -3006\nExpected: -3006.0d\nWas: "+DecimalFraction.FromString("-3006").ToDouble());
      if(3.07120343E18f!=DecimalFraction.FromString("3071203450148698328").ToSingle())
        Assert.Fail("decfrac single 3071203450148698328\nExpected: 3.07120343E18f\nWas: "+DecimalFraction.FromString("3071203450148698328").ToSingle());
      if(3.0712034501486981E18d!=DecimalFraction.FromString("3071203450148698328").ToDouble())
        Assert.Fail("decfrac double 3071203450148698328\nExpected: 3.0712034501486981E18d\nWas: "+DecimalFraction.FromString("3071203450148698328").ToDouble());
    }
    
    [Test]
    public void TestDecFracToSingleDouble(){
      if(-4348.0f!=DecimalFraction.FromString("-4348").ToSingle())
        Assert.Fail("decfrac single -4348\nExpected: -4348.0f\nWas: "+DecimalFraction.FromString("-4348").ToSingle());
      if(-4348.0d!=DecimalFraction.FromString("-4348").ToDouble())
        Assert.Fail("decfrac double -4348\nExpected: -4348.0d\nWas: "+DecimalFraction.FromString("-4348").ToDouble());
      if(-9.85323f!=DecimalFraction.FromString("-9.85323086293411065").ToSingle())
        Assert.Fail("decfrac single -9.85323086293411065\nExpected: -9.85323f\nWas: "+DecimalFraction.FromString("-9.85323086293411065").ToSingle());
      if(-9.85323086293411d!=DecimalFraction.FromString("-9.85323086293411065").ToDouble())
        Assert.Fail("decfrac double -9.85323086293411065\nExpected: -9.85323086293411d\nWas: "+DecimalFraction.FromString("-9.85323086293411065").ToDouble());
      if(-5.2317E9f!=DecimalFraction.FromString("-5231.7E+6").ToSingle())
        Assert.Fail("decfrac single -5231.7E+6\nExpected: -5.2317E9f\nWas: "+DecimalFraction.FromString("-5231.7E+6").ToSingle());
      if(-5.2317E9d!=DecimalFraction.FromString("-5231.7E+6").ToDouble())
        Assert.Fail("decfrac double -5231.7E+6\nExpected: -5.2317E9d\nWas: "+DecimalFraction.FromString("-5231.7E+6").ToDouble());
      if(5.7991604E7f!=DecimalFraction.FromString("579916024.449917729730457E-1").ToSingle())
        Assert.Fail("decfrac single 579916024.449917729730457E-1\nExpected: 5.7991604E7f\nWas: "+DecimalFraction.FromString("579916024.449917729730457E-1").ToSingle());
      if(5.7991602444991775E7d!=DecimalFraction.FromString("579916024.449917729730457E-1").ToDouble())
        Assert.Fail("decfrac double 579916024.449917729730457E-1\nExpected: 5.7991602444991775E7d\nWas: "+DecimalFraction.FromString("579916024.449917729730457E-1").ToDouble());
      if(-515.02563f!=DecimalFraction.FromString("-515025607547098618E-15").ToSingle())
        Assert.Fail("decfrac single -515025607547098618E-15\nExpected: -515.02563f\nWas: "+DecimalFraction.FromString("-515025607547098618E-15").ToSingle());
      if(-515.0256075470986d!=DecimalFraction.FromString("-515025607547098618E-15").ToDouble())
        Assert.Fail("decfrac double -515025607547098618E-15\nExpected: -515.0256075470986d\nWas: "+DecimalFraction.FromString("-515025607547098618E-15").ToDouble());
      if(-9.3541843E10f!=DecimalFraction.FromString("-93541840706").ToSingle())
        Assert.Fail("decfrac single -93541840706\nExpected: -9.3541843E10f\nWas: "+DecimalFraction.FromString("-93541840706").ToSingle());
      if(-9.3541840706E10d!=DecimalFraction.FromString("-93541840706").ToDouble())
        Assert.Fail("decfrac double -93541840706\nExpected: -9.3541840706E10d\nWas: "+DecimalFraction.FromString("-93541840706").ToDouble());
      if(3.8568078E23f!=DecimalFraction.FromString("38568076767380659.6E+7").ToSingle())
        Assert.Fail("decfrac single 38568076767380659.6E+7\nExpected: 3.8568078E23f\nWas: "+DecimalFraction.FromString("38568076767380659.6E+7").ToSingle());
      if(3.8568076767380657E23d!=DecimalFraction.FromString("38568076767380659.6E+7").ToDouble())
        Assert.Fail("decfrac double 38568076767380659.6E+7\nExpected: 3.8568076767380657E23d\nWas: "+DecimalFraction.FromString("38568076767380659.6E+7").ToDouble());
      if(4682.1987f!=DecimalFraction.FromString("468219867826E-8").ToSingle())
        Assert.Fail("decfrac single 468219867826E-8\nExpected: 4682.1987f\nWas: "+DecimalFraction.FromString("468219867826E-8").ToSingle());
      if(4682.19867826d!=DecimalFraction.FromString("468219867826E-8").ToDouble())
        Assert.Fail("decfrac double 468219867826E-8\nExpected: 4682.19867826d\nWas: "+DecimalFraction.FromString("468219867826E-8").ToDouble());
      if(7.3869363E-4f!=DecimalFraction.FromString("73869365.3859328709200790828E-11").ToSingle())
        Assert.Fail("decfrac single 73869365.3859328709200790828E-11\nExpected: 7.3869363E-4f\nWas: "+DecimalFraction.FromString("73869365.3859328709200790828E-11").ToSingle());
      if(7.386936538593287E-4d!=DecimalFraction.FromString("73869365.3859328709200790828E-11").ToDouble())
        Assert.Fail("decfrac double 73869365.3859328709200790828E-11\nExpected: 7.386936538593287E-4d\nWas: "+DecimalFraction.FromString("73869365.3859328709200790828E-11").ToDouble());
      if(2.3f!=DecimalFraction.FromString("2.3E0").ToSingle())
        Assert.Fail("decfrac single 2.3E0\nExpected: 2.3f\nWas: "+DecimalFraction.FromString("2.3E0").ToSingle());
      if(2.3d!=DecimalFraction.FromString("2.3E0").ToDouble())
        Assert.Fail("decfrac double 2.3E0\nExpected: 2.3d\nWas: "+DecimalFraction.FromString("2.3E0").ToDouble());
      if(3.3713182E15f!=DecimalFraction.FromString("3371318258253373.59498533176159560").ToSingle())
        Assert.Fail("decfrac single 3371318258253373.59498533176159560\nExpected: 3.3713182E15f\nWas: "+DecimalFraction.FromString("3371318258253373.59498533176159560").ToSingle());
      if(3.3713182582533735E15d!=DecimalFraction.FromString("3371318258253373.59498533176159560").ToDouble())
        Assert.Fail("decfrac double 3371318258253373.59498533176159560\nExpected: 3.3713182582533735E15d\nWas: "+DecimalFraction.FromString("3371318258253373.59498533176159560").ToDouble());
      if(0.08044683f!=DecimalFraction.FromString("804468350612974.6118902086132089233E-16").ToSingle())
        Assert.Fail("decfrac single 804468350612974.6118902086132089233E-16\nExpected: 0.08044683f\nWas: "+DecimalFraction.FromString("804468350612974.6118902086132089233E-16").ToSingle());
      if(0.08044683506129746d!=DecimalFraction.FromString("804468350612974.6118902086132089233E-16").ToDouble())
        Assert.Fail("decfrac double 804468350612974.6118902086132089233E-16\nExpected: 0.08044683506129746d\nWas: "+DecimalFraction.FromString("804468350612974.6118902086132089233E-16").ToDouble());
      if(-7.222071E19f!=DecimalFraction.FromString("-72220708347127407337.28").ToSingle())
        Assert.Fail("decfrac single -72220708347127407337.28\nExpected: -7.222071E19f\nWas: "+DecimalFraction.FromString("-72220708347127407337.28").ToSingle());
      if(-7.222070834712741E19d!=DecimalFraction.FromString("-72220708347127407337.28").ToDouble())
        Assert.Fail("decfrac double -72220708347127407337.28\nExpected: -7.222070834712741E19d\nWas: "+DecimalFraction.FromString("-72220708347127407337.28").ToDouble());
      if(9715796.0f!=DecimalFraction.FromString("9715796.4299331966870989").ToSingle())
        Assert.Fail("decfrac single 9715796.4299331966870989\nExpected: 9715796.0f\nWas: "+DecimalFraction.FromString("9715796.4299331966870989").ToSingle());
      if(9715796.429933196d!=DecimalFraction.FromString("9715796.4299331966870989").ToDouble())
        Assert.Fail("decfrac double 9715796.4299331966870989\nExpected: 9715796.429933196d\nWas: "+DecimalFraction.FromString("9715796.4299331966870989").ToDouble());
      if(9.3596612E14f!=DecimalFraction.FromString("93596609961883873.8463754373628236E-2").ToSingle())
        Assert.Fail("decfrac single 93596609961883873.8463754373628236E-2\nExpected: 9.3596612E14f\nWas: "+DecimalFraction.FromString("93596609961883873.8463754373628236E-2").ToSingle());
      if(9.359660996188388E14d!=DecimalFraction.FromString("93596609961883873.8463754373628236E-2").ToDouble())
        Assert.Fail("decfrac double 93596609961883873.8463754373628236E-2\nExpected: 9.359660996188388E14d\nWas: "+DecimalFraction.FromString("93596609961883873.8463754373628236E-2").ToDouble());
      if(4.82799354E14f!=DecimalFraction.FromString("482799357899450").ToSingle())
        Assert.Fail("decfrac single 482799357899450\nExpected: 4.82799354E14f\nWas: "+DecimalFraction.FromString("482799357899450").ToSingle());
      if(4.8279935789945E14d!=DecimalFraction.FromString("482799357899450").ToDouble())
        Assert.Fail("decfrac double 482799357899450\nExpected: 4.8279935789945E14d\nWas: "+DecimalFraction.FromString("482799357899450").ToDouble());
      if(3.8193924E25f!=DecimalFraction.FromString("381939236989E+14").ToSingle())
        Assert.Fail("decfrac single 381939236989E+14\nExpected: 3.8193924E25f\nWas: "+DecimalFraction.FromString("381939236989E+14").ToSingle());
      if(3.81939236989E25d!=DecimalFraction.FromString("381939236989E+14").ToDouble())
        Assert.Fail("decfrac double 381939236989E+14\nExpected: 3.81939236989E25d\nWas: "+DecimalFraction.FromString("381939236989E+14").ToDouble());
      if(-3.1092332E27f!=DecimalFraction.FromString("-3109233371824024E+12").ToSingle())
        Assert.Fail("decfrac single -3109233371824024E+12\nExpected: -3.1092332E27f\nWas: "+DecimalFraction.FromString("-3109233371824024E+12").ToSingle());
      if(-3.109233371824024E27d!=DecimalFraction.FromString("-3109233371824024E+12").ToDouble())
        Assert.Fail("decfrac double -3109233371824024E+12\nExpected: -3.109233371824024E27d\nWas: "+DecimalFraction.FromString("-3109233371824024E+12").ToDouble());
      if(-0.006658507f!=DecimalFraction.FromString("-66585.07E-7").ToSingle())
        Assert.Fail("decfrac single -66585.07E-7\nExpected: -0.006658507f\nWas: "+DecimalFraction.FromString("-66585.07E-7").ToSingle());
      if(-0.006658507d!=DecimalFraction.FromString("-66585.07E-7").ToDouble())
        Assert.Fail("decfrac double -66585.07E-7\nExpected: -0.006658507d\nWas: "+DecimalFraction.FromString("-66585.07E-7").ToDouble());
      if(17.276796f!=DecimalFraction.FromString("17.276795549708").ToSingle())
        Assert.Fail("decfrac single 17.276795549708\nExpected: 17.276796f\nWas: "+DecimalFraction.FromString("17.276795549708").ToSingle());
      if(17.276795549708d!=DecimalFraction.FromString("17.276795549708").ToDouble())
        Assert.Fail("decfrac double 17.276795549708\nExpected: 17.276795549708d\nWas: "+DecimalFraction.FromString("17.276795549708").ToDouble());
      if(-3210939.5f!=DecimalFraction.FromString("-321093943510192.3307E-8").ToSingle())
        Assert.Fail("decfrac single -321093943510192.3307E-8\nExpected: -3210939.5f\nWas: "+DecimalFraction.FromString("-321093943510192.3307E-8").ToSingle());
      if(-3210939.4351019235d!=DecimalFraction.FromString("-321093943510192.3307E-8").ToDouble())
        Assert.Fail("decfrac double -321093943510192.3307E-8\nExpected: -3210939.4351019235d\nWas: "+DecimalFraction.FromString("-321093943510192.3307E-8").ToDouble());
      if(-976.9676f!=DecimalFraction.FromString("-976.967597776185553735").ToSingle())
        Assert.Fail("decfrac single -976.967597776185553735\nExpected: -976.9676f\nWas: "+DecimalFraction.FromString("-976.967597776185553735").ToSingle());
      if(-976.9675977761856d!=DecimalFraction.FromString("-976.967597776185553735").ToDouble())
        Assert.Fail("decfrac double -976.967597776185553735\nExpected: -976.9675977761856d\nWas: "+DecimalFraction.FromString("-976.967597776185553735").ToDouble());
      if(-3.49712614E9f!=DecimalFraction.FromString("-3497126138").ToSingle())
        Assert.Fail("decfrac single -3497126138\nExpected: -3.49712614E9f\nWas: "+DecimalFraction.FromString("-3497126138").ToSingle());
      if(-3.497126138E9d!=DecimalFraction.FromString("-3497126138").ToDouble())
        Assert.Fail("decfrac double -3497126138\nExpected: -3.497126138E9d\nWas: "+DecimalFraction.FromString("-3497126138").ToDouble());
      if(-2.63418028E14f!=DecimalFraction.FromString("-2634180.2455697965376217503E+8").ToSingle())
        Assert.Fail("decfrac single -2634180.2455697965376217503E+8\nExpected: -2.63418028E14f\nWas: "+DecimalFraction.FromString("-2634180.2455697965376217503E+8").ToSingle());
      if(-2.6341802455697966E14d!=DecimalFraction.FromString("-2634180.2455697965376217503E+8").ToDouble())
        Assert.Fail("decfrac double -2634180.2455697965376217503E+8\nExpected: -2.6341802455697966E14d\nWas: "+DecimalFraction.FromString("-2634180.2455697965376217503E+8").ToDouble());
      if(3.25314253E10f!=DecimalFraction.FromString("32531426161").ToSingle())
        Assert.Fail("decfrac single 32531426161\nExpected: 3.25314253E10f\nWas: "+DecimalFraction.FromString("32531426161").ToSingle());
      if(3.2531426161E10d!=DecimalFraction.FromString("32531426161").ToDouble())
        Assert.Fail("decfrac double 32531426161\nExpected: 3.2531426161E10d\nWas: "+DecimalFraction.FromString("32531426161").ToDouble());
      if(-83825.7f!=DecimalFraction.FromString("-83825.7").ToSingle())
        Assert.Fail("decfrac single -83825.7\nExpected: -83825.7f\nWas: "+DecimalFraction.FromString("-83825.7").ToSingle());
      if(-83825.7d!=DecimalFraction.FromString("-83825.7").ToDouble())
        Assert.Fail("decfrac double -83825.7\nExpected: -83825.7d\nWas: "+DecimalFraction.FromString("-83825.7").ToDouble());
      if(9347.0f!=DecimalFraction.FromString("9347").ToSingle())
        Assert.Fail("decfrac single 9347\nExpected: 9347.0f\nWas: "+DecimalFraction.FromString("9347").ToSingle());
      if(9347.0d!=DecimalFraction.FromString("9347").ToDouble())
        Assert.Fail("decfrac double 9347\nExpected: 9347.0d\nWas: "+DecimalFraction.FromString("9347").ToDouble());
      if(4039.426f!=DecimalFraction.FromString("403942604431E-8").ToSingle())
        Assert.Fail("decfrac single 403942604431E-8\nExpected: 4039.426f\nWas: "+DecimalFraction.FromString("403942604431E-8").ToSingle());
      if(4039.42604431d!=DecimalFraction.FromString("403942604431E-8").ToDouble())
        Assert.Fail("decfrac double 403942604431E-8\nExpected: 4039.42604431d\nWas: "+DecimalFraction.FromString("403942604431E-8").ToDouble());
      if(9.821772E-8f!=DecimalFraction.FromString("9821771729.481512E-17").ToSingle())
        Assert.Fail("decfrac single 9821771729.481512E-17\nExpected: 9.821772E-8f\nWas: "+DecimalFraction.FromString("9821771729.481512E-17").ToSingle());
      if(9.821771729481512E-8d!=DecimalFraction.FromString("9821771729.481512E-17").ToDouble())
        Assert.Fail("decfrac double 9821771729.481512E-17\nExpected: 9.821771729481512E-8d\nWas: "+DecimalFraction.FromString("9821771729.481512E-17").ToDouble());
      if(1.47027E24f!=DecimalFraction.FromString("1470270E+18").ToSingle())
        Assert.Fail("decfrac single 1470270E+18\nExpected: 1.47027E24f\nWas: "+DecimalFraction.FromString("1470270E+18").ToSingle());
      if(1.47027E24d!=DecimalFraction.FromString("1470270E+18").ToDouble())
        Assert.Fail("decfrac double 1470270E+18\nExpected: 1.47027E24d\nWas: "+DecimalFraction.FromString("1470270E+18").ToDouble());
      if(504.07468f!=DecimalFraction.FromString("504.074687047275").ToSingle())
        Assert.Fail("decfrac single 504.074687047275\nExpected: 504.07468f\nWas: "+DecimalFraction.FromString("504.074687047275").ToSingle());
      if(504.074687047275d!=DecimalFraction.FromString("504.074687047275").ToDouble())
        Assert.Fail("decfrac double 504.074687047275\nExpected: 504.074687047275d\nWas: "+DecimalFraction.FromString("504.074687047275").ToDouble());
      if(8.051101E-11f!=DecimalFraction.FromString("8051.10083245768396604E-14").ToSingle())
        Assert.Fail("decfrac single 8051.10083245768396604E-14\nExpected: 8.051101E-11f\nWas: "+DecimalFraction.FromString("8051.10083245768396604E-14").ToSingle());
      if(8.051100832457683E-11d!=DecimalFraction.FromString("8051.10083245768396604E-14").ToDouble())
        Assert.Fail("decfrac double 8051.10083245768396604E-14\nExpected: 8.051100832457683E-11d\nWas: "+DecimalFraction.FromString("8051.10083245768396604E-14").ToDouble());
      if(-9789.0f!=DecimalFraction.FromString("-9789").ToSingle())
        Assert.Fail("decfrac single -9789\nExpected: -9789.0f\nWas: "+DecimalFraction.FromString("-9789").ToSingle());
      if(-9789.0d!=DecimalFraction.FromString("-9789").ToDouble())
        Assert.Fail("decfrac double -9789\nExpected: -9789.0d\nWas: "+DecimalFraction.FromString("-9789").ToDouble());
      if(-2.95046595E10f!=DecimalFraction.FromString("-295046585154199748.8456E-7").ToSingle())
        Assert.Fail("decfrac single -295046585154199748.8456E-7\nExpected: -2.95046595E10f\nWas: "+DecimalFraction.FromString("-295046585154199748.8456E-7").ToSingle());
      if(-2.9504658515419975E10d!=DecimalFraction.FromString("-295046585154199748.8456E-7").ToDouble())
        Assert.Fail("decfrac double -295046585154199748.8456E-7\nExpected: -2.9504658515419975E10d\nWas: "+DecimalFraction.FromString("-295046585154199748.8456E-7").ToDouble());
      if(5.8642877E23f!=DecimalFraction.FromString("58642877210005207.915393764393974811E+7").ToSingle())
        Assert.Fail("decfrac single 58642877210005207.915393764393974811E+7\nExpected: 5.8642877E23f\nWas: "+DecimalFraction.FromString("58642877210005207.915393764393974811E+7").ToSingle());
      if(5.864287721000521E23d!=DecimalFraction.FromString("58642877210005207.915393764393974811E+7").ToDouble())
        Assert.Fail("decfrac double 58642877210005207.915393764393974811E+7\nExpected: 5.864287721000521E23d\nWas: "+DecimalFraction.FromString("58642877210005207.915393764393974811E+7").ToDouble());
      if(-5.13554645E11f!=DecimalFraction.FromString("-513554652569").ToSingle())
        Assert.Fail("decfrac single -513554652569\nExpected: -5.13554645E11f\nWas: "+DecimalFraction.FromString("-513554652569").ToSingle());
      if(-5.13554652569E11d!=DecimalFraction.FromString("-513554652569").ToDouble())
        Assert.Fail("decfrac double -513554652569\nExpected: -5.13554652569E11d\nWas: "+DecimalFraction.FromString("-513554652569").ToDouble());
      if(-1.66059725E10f!=DecimalFraction.FromString("-166059726561900E-4").ToSingle())
        Assert.Fail("decfrac single -166059726561900E-4\nExpected: -1.66059725E10f\nWas: "+DecimalFraction.FromString("-166059726561900E-4").ToSingle());
      if(-1.660597265619E10d!=DecimalFraction.FromString("-166059726561900E-4").ToDouble())
        Assert.Fail("decfrac double -166059726561900E-4\nExpected: -1.660597265619E10d\nWas: "+DecimalFraction.FromString("-166059726561900E-4").ToDouble());
      if(-3.66681318E9f!=DecimalFraction.FromString("-3666813090").ToSingle())
        Assert.Fail("decfrac single -3666813090\nExpected: -3.66681318E9f\nWas: "+DecimalFraction.FromString("-3666813090").ToSingle());
      if(-3.66681309E9d!=DecimalFraction.FromString("-3666813090").ToDouble())
        Assert.Fail("decfrac double -3666813090\nExpected: -3.66681309E9d\nWas: "+DecimalFraction.FromString("-3666813090").ToDouble());
      if(-741.0616f!=DecimalFraction.FromString("-741.061579731811").ToSingle())
        Assert.Fail("decfrac single -741.061579731811\nExpected: -741.0616f\nWas: "+DecimalFraction.FromString("-741.061579731811").ToSingle());
      if(-741.061579731811d!=DecimalFraction.FromString("-741.061579731811").ToDouble())
        Assert.Fail("decfrac double -741.061579731811\nExpected: -741.061579731811d\nWas: "+DecimalFraction.FromString("-741.061579731811").ToDouble());
      if(-2264.0f!=DecimalFraction.FromString("-2264").ToSingle())
        Assert.Fail("decfrac single -2264\nExpected: -2264.0f\nWas: "+DecimalFraction.FromString("-2264").ToSingle());
      if(-2264.0d!=DecimalFraction.FromString("-2264").ToDouble())
        Assert.Fail("decfrac double -2264\nExpected: -2264.0d\nWas: "+DecimalFraction.FromString("-2264").ToDouble());
      if(9.2388336E10f!=DecimalFraction.FromString("92388332924").ToSingle())
        Assert.Fail("decfrac single 92388332924\nExpected: 9.2388336E10f\nWas: "+DecimalFraction.FromString("92388332924").ToSingle());
      if(9.2388332924E10d!=DecimalFraction.FromString("92388332924").ToDouble())
        Assert.Fail("decfrac double 92388332924\nExpected: 9.2388332924E10d\nWas: "+DecimalFraction.FromString("92388332924").ToDouble());
      if(4991.7646f!=DecimalFraction.FromString("4991.764823290772791").ToSingle())
        Assert.Fail("decfrac single 4991.764823290772791\nExpected: 4991.7646f\nWas: "+DecimalFraction.FromString("4991.764823290772791").ToSingle());
      if(4991.764823290773d!=DecimalFraction.FromString("4991.764823290772791").ToDouble())
        Assert.Fail("decfrac double 4991.764823290772791\nExpected: 4991.764823290773d\nWas: "+DecimalFraction.FromString("4991.764823290772791").ToDouble());
      if(-31529.82f!=DecimalFraction.FromString("-3152982E-2").ToSingle())
        Assert.Fail("decfrac single -3152982E-2\nExpected: -31529.82f\nWas: "+DecimalFraction.FromString("-3152982E-2").ToSingle());
      if(-31529.82d!=DecimalFraction.FromString("-3152982E-2").ToDouble())
        Assert.Fail("decfrac double -3152982E-2\nExpected: -31529.82d\nWas: "+DecimalFraction.FromString("-3152982E-2").ToDouble());
      if(-2.96352045E15f!=DecimalFraction.FromString("-2963520450661169.515038656").ToSingle())
        Assert.Fail("decfrac single -2963520450661169.515038656\nExpected: -2.96352045E15f\nWas: "+DecimalFraction.FromString("-2963520450661169.515038656").ToSingle());
      if(-2.9635204506611695E15d!=DecimalFraction.FromString("-2963520450661169.515038656").ToDouble())
        Assert.Fail("decfrac double -2963520450661169.515038656\nExpected: -2.9635204506611695E15d\nWas: "+DecimalFraction.FromString("-2963520450661169.515038656").ToDouble());
      if(-9.0629749E13f!=DecimalFraction.FromString("-9062974752750092585.8070204683471E-5").ToSingle())
        Assert.Fail("decfrac single -9062974752750092585.8070204683471E-5\nExpected: -9.0629749E13f\nWas: "+DecimalFraction.FromString("-9062974752750092585.8070204683471E-5").ToSingle());
      if(-9.062974752750092E13d!=DecimalFraction.FromString("-9062974752750092585.8070204683471E-5").ToDouble())
        Assert.Fail("decfrac double -9062974752750092585.8070204683471E-5\nExpected: -9.062974752750092E13d\nWas: "+DecimalFraction.FromString("-9062974752750092585.8070204683471E-5").ToDouble());
      if(1.32708426E11f!=DecimalFraction.FromString("1327.08423724267788662E+8").ToSingle())
        Assert.Fail("decfrac single 1327.08423724267788662E+8\nExpected: 1.32708426E11f\nWas: "+DecimalFraction.FromString("1327.08423724267788662E+8").ToSingle());
      if(1.3270842372426779E11d!=DecimalFraction.FromString("1327.08423724267788662E+8").ToDouble())
        Assert.Fail("decfrac double 1327.08423724267788662E+8\nExpected: 1.3270842372426779E11d\nWas: "+DecimalFraction.FromString("1327.08423724267788662E+8").ToDouble());
      if(3.03766274E11f!=DecimalFraction.FromString("3037662626861314743.2222785E-7").ToSingle())
        Assert.Fail("decfrac single 3037662626861314743.2222785E-7\nExpected: 3.03766274E11f\nWas: "+DecimalFraction.FromString("3037662626861314743.2222785E-7").ToSingle());
      if(3.037662626861315E11d!=DecimalFraction.FromString("3037662626861314743.2222785E-7").ToDouble())
        Assert.Fail("decfrac double 3037662626861314743.2222785E-7\nExpected: 3.037662626861315E11d\nWas: "+DecimalFraction.FromString("3037662626861314743.2222785E-7").ToDouble());
      if(5.3666539E12f!=DecimalFraction.FromString("5366653818787.5E0").ToSingle())
        Assert.Fail("decfrac single 5366653818787.5E0\nExpected: 5.3666539E12f\nWas: "+DecimalFraction.FromString("5366653818787.5E0").ToSingle());
      if(5.3666538187875E12d!=DecimalFraction.FromString("5366653818787.5E0").ToDouble())
        Assert.Fail("decfrac double 5366653818787.5E0\nExpected: 5.3666538187875E12d\nWas: "+DecimalFraction.FromString("5366653818787.5E0").ToDouble());
      if(-0.09572517f!=DecimalFraction.FromString("-957251.70125291697919424260E-7").ToSingle())
        Assert.Fail("decfrac single -957251.70125291697919424260E-7\nExpected: -0.09572517f\nWas: "+DecimalFraction.FromString("-957251.70125291697919424260E-7").ToSingle());
      if(-0.09572517012529169d!=DecimalFraction.FromString("-957251.70125291697919424260E-7").ToDouble())
        Assert.Fail("decfrac double -957251.70125291697919424260E-7\nExpected: -0.09572517012529169d\nWas: "+DecimalFraction.FromString("-957251.70125291697919424260E-7").ToDouble());
      if(8.4375632E7f!=DecimalFraction.FromString("8437563497492.8514E-5").ToSingle())
        Assert.Fail("decfrac single 8437563497492.8514E-5\nExpected: 8.4375632E7f\nWas: "+DecimalFraction.FromString("8437563497492.8514E-5").ToSingle());
      if(8.437563497492851E7d!=DecimalFraction.FromString("8437563497492.8514E-5").ToDouble())
        Assert.Fail("decfrac double 8437563497492.8514E-5\nExpected: 8.437563497492851E7d\nWas: "+DecimalFraction.FromString("8437563497492.8514E-5").ToDouble());
      if(7.7747428E15f!=DecimalFraction.FromString("7774742890322348.749566199224594").ToSingle())
        Assert.Fail("decfrac single 7774742890322348.749566199224594\nExpected: 7.7747428E15f\nWas: "+DecimalFraction.FromString("7774742890322348.749566199224594").ToSingle());
      if(7.774742890322349E15d!=DecimalFraction.FromString("7774742890322348.749566199224594").ToDouble())
        Assert.Fail("decfrac double 7774742890322348.749566199224594\nExpected: 7.774742890322349E15d\nWas: "+DecimalFraction.FromString("7774742890322348.749566199224594").ToDouble());
      if(-6.3523806E18f!=DecimalFraction.FromString("-6352380631468114E+3").ToSingle())
        Assert.Fail("decfrac single -6352380631468114E+3\nExpected: -6.3523806E18f\nWas: "+DecimalFraction.FromString("-6352380631468114E+3").ToSingle());
      if(-6.3523806314681139E18d!=DecimalFraction.FromString("-6352380631468114E+3").ToDouble())
        Assert.Fail("decfrac double -6352380631468114E+3\nExpected: -6.3523806314681139E18d\nWas: "+DecimalFraction.FromString("-6352380631468114E+3").ToDouble());
      if(-8.1199685E23f!=DecimalFraction.FromString("-8119968851439E+11").ToSingle())
        Assert.Fail("decfrac single -8119968851439E+11\nExpected: -8.1199685E23f\nWas: "+DecimalFraction.FromString("-8119968851439E+11").ToSingle());
      if(-8.119968851439E23d!=DecimalFraction.FromString("-8119968851439E+11").ToDouble())
        Assert.Fail("decfrac double -8119968851439E+11\nExpected: -8.119968851439E23d\nWas: "+DecimalFraction.FromString("-8119968851439E+11").ToDouble());
      if(-3.201959E23f!=DecimalFraction.FromString("-3201959209367.08531737604446E+11").ToSingle())
        Assert.Fail("decfrac single -3201959209367.08531737604446E+11\nExpected: -3.201959E23f\nWas: "+DecimalFraction.FromString("-3201959209367.08531737604446E+11").ToSingle());
      if(-3.201959209367085E23d!=DecimalFraction.FromString("-3201959209367.08531737604446E+11").ToDouble())
        Assert.Fail("decfrac double -3201959209367.08531737604446E+11\nExpected: -3.201959209367085E23d\nWas: "+DecimalFraction.FromString("-3201959209367.08531737604446E+11").ToDouble());
      if(-6.0171188E7f!=DecimalFraction.FromString("-60171187").ToSingle())
        Assert.Fail("decfrac single -60171187\nExpected: -6.0171188E7f\nWas: "+DecimalFraction.FromString("-60171187").ToSingle());
      if(-6.0171187E7d!=DecimalFraction.FromString("-60171187").ToDouble())
        Assert.Fail("decfrac double -60171187\nExpected: -6.0171187E7d\nWas: "+DecimalFraction.FromString("-60171187").ToDouble());
      if(-6.6884155E-7f!=DecimalFraction.FromString("-66.884154716131E-8").ToSingle())
        Assert.Fail("decfrac single -66.884154716131E-8\nExpected: -6.6884155E-7f\nWas: "+DecimalFraction.FromString("-66.884154716131E-8").ToSingle());
      if(-6.6884154716131E-7d!=DecimalFraction.FromString("-66.884154716131E-8").ToDouble())
        Assert.Fail("decfrac double -66.884154716131E-8\nExpected: -6.6884154716131E-7d\nWas: "+DecimalFraction.FromString("-66.884154716131E-8").ToDouble());
      if(923595.4f!=DecimalFraction.FromString("923595.376427445").ToSingle())
        Assert.Fail("decfrac single 923595.376427445\nExpected: 923595.4f\nWas: "+DecimalFraction.FromString("923595.376427445").ToSingle());
      if(923595.376427445d!=DecimalFraction.FromString("923595.376427445").ToDouble())
        Assert.Fail("decfrac double 923595.376427445\nExpected: 923595.376427445d\nWas: "+DecimalFraction.FromString("923595.376427445").ToDouble());
      if(-5.0f!=DecimalFraction.FromString("-5").ToSingle())
        Assert.Fail("decfrac single -5\nExpected: -5.0f\nWas: "+DecimalFraction.FromString("-5").ToSingle());
      if(-5.0d!=DecimalFraction.FromString("-5").ToDouble())
        Assert.Fail("decfrac double -5\nExpected: -5.0d\nWas: "+DecimalFraction.FromString("-5").ToDouble());
      if(4.7380017E10f!=DecimalFraction.FromString("47380017776.35").ToSingle())
        Assert.Fail("decfrac single 47380017776.35\nExpected: 4.7380017E10f\nWas: "+DecimalFraction.FromString("47380017776.35").ToSingle());
      if(4.738001777635E10d!=DecimalFraction.FromString("47380017776.35").ToDouble())
        Assert.Fail("decfrac double 47380017776.35\nExpected: 4.738001777635E10d\nWas: "+DecimalFraction.FromString("47380017776.35").ToDouble());
      if(8139584.0f!=DecimalFraction.FromString("8139584.242987").ToSingle())
        Assert.Fail("decfrac single 8139584.242987\nExpected: 8139584.0f\nWas: "+DecimalFraction.FromString("8139584.242987").ToSingle());
      if(8139584.242987d!=DecimalFraction.FromString("8139584.242987").ToDouble())
        Assert.Fail("decfrac double 8139584.242987\nExpected: 8139584.242987d\nWas: "+DecimalFraction.FromString("8139584.242987").ToDouble());
      if(5.0f!=DecimalFraction.FromString("5").ToSingle())
        Assert.Fail("decfrac single 5\nExpected: 5.0f\nWas: "+DecimalFraction.FromString("5").ToSingle());
      if(5.0d!=DecimalFraction.FromString("5").ToDouble())
        Assert.Fail("decfrac double 5\nExpected: 5.0d\nWas: "+DecimalFraction.FromString("5").ToDouble());
      if(-3.6578223E27f!=DecimalFraction.FromString("-365782224812843E+13").ToSingle())
        Assert.Fail("decfrac single -365782224812843E+13\nExpected: -3.6578223E27f\nWas: "+DecimalFraction.FromString("-365782224812843E+13").ToSingle());
      if(-3.65782224812843E27d!=DecimalFraction.FromString("-365782224812843E+13").ToDouble())
        Assert.Fail("decfrac double -365782224812843E+13\nExpected: -3.65782224812843E27d\nWas: "+DecimalFraction.FromString("-365782224812843E+13").ToDouble());
      if(6.263606E23f!=DecimalFraction.FromString("626360584867890223E+6").ToSingle())
        Assert.Fail("decfrac single 626360584867890223E+6\nExpected: 6.263606E23f\nWas: "+DecimalFraction.FromString("626360584867890223E+6").ToSingle());
      if(6.263605848678903E23d!=DecimalFraction.FromString("626360584867890223E+6").ToDouble())
        Assert.Fail("decfrac double 626360584867890223E+6\nExpected: 6.263605848678903E23d\nWas: "+DecimalFraction.FromString("626360584867890223E+6").ToDouble());
      if(-1.26830412E18f!=DecimalFraction.FromString("-12683040859E+8").ToSingle())
        Assert.Fail("decfrac single -12683040859E+8\nExpected: -1.26830412E18f\nWas: "+DecimalFraction.FromString("-12683040859E+8").ToSingle());
      if(-1.2683040859E18d!=DecimalFraction.FromString("-12683040859E+8").ToDouble())
        Assert.Fail("decfrac double -12683040859E+8\nExpected: -1.2683040859E18d\nWas: "+DecimalFraction.FromString("-12683040859E+8").ToDouble());
      if(8.9906433E13f!=DecimalFraction.FromString("89906433733052.14691421345561385").ToSingle())
        Assert.Fail("decfrac single 89906433733052.14691421345561385\nExpected: 8.9906433E13f\nWas: "+DecimalFraction.FromString("89906433733052.14691421345561385").ToSingle());
      if(8.990643373305214E13d!=DecimalFraction.FromString("89906433733052.14691421345561385").ToDouble())
        Assert.Fail("decfrac double 89906433733052.14691421345561385\nExpected: 8.990643373305214E13d\nWas: "+DecimalFraction.FromString("89906433733052.14691421345561385").ToDouble());
      if(82.0f!=DecimalFraction.FromString("82").ToSingle())
        Assert.Fail("decfrac single 82\nExpected: 82.0f\nWas: "+DecimalFraction.FromString("82").ToSingle());
      if(82.0d!=DecimalFraction.FromString("82").ToDouble())
        Assert.Fail("decfrac double 82\nExpected: 82.0d\nWas: "+DecimalFraction.FromString("82").ToDouble());
      if(9.5523543E16f!=DecimalFraction.FromString("95523541216159667").ToSingle())
        Assert.Fail("decfrac single 95523541216159667\nExpected: 9.5523543E16f\nWas: "+DecimalFraction.FromString("95523541216159667").ToSingle());
      if(9.5523541216159664E16d!=DecimalFraction.FromString("95523541216159667").ToDouble())
        Assert.Fail("decfrac double 95523541216159667\nExpected: 9.5523541216159664E16d\nWas: "+DecimalFraction.FromString("95523541216159667").ToDouble());
      if(-0.040098447f!=DecimalFraction.FromString("-400984.46498769274346390686E-7").ToSingle())
        Assert.Fail("decfrac single -400984.46498769274346390686E-7\nExpected: -0.040098447f\nWas: "+DecimalFraction.FromString("-400984.46498769274346390686E-7").ToSingle());
      if(-0.04009844649876927d!=DecimalFraction.FromString("-400984.46498769274346390686E-7").ToDouble())
        Assert.Fail("decfrac double -400984.46498769274346390686E-7\nExpected: -0.04009844649876927d\nWas: "+DecimalFraction.FromString("-400984.46498769274346390686E-7").ToDouble());
      if(9.9082332E14f!=DecimalFraction.FromString("990823307532E+3").ToSingle())
        Assert.Fail("decfrac single 990823307532E+3\nExpected: 9.9082332E14f\nWas: "+DecimalFraction.FromString("990823307532E+3").ToSingle());
      if(9.90823307532E14d!=DecimalFraction.FromString("990823307532E+3").ToDouble())
        Assert.Fail("decfrac double 990823307532E+3\nExpected: 9.90823307532E14d\nWas: "+DecimalFraction.FromString("990823307532E+3").ToDouble());
      if(-8.969879E8f!=DecimalFraction.FromString("-896987890").ToSingle())
        Assert.Fail("decfrac single -896987890\nExpected: -8.969879E8f\nWas: "+DecimalFraction.FromString("-896987890").ToSingle());
      if(-8.9698789E8d!=DecimalFraction.FromString("-896987890").ToDouble())
        Assert.Fail("decfrac double -896987890\nExpected: -8.9698789E8d\nWas: "+DecimalFraction.FromString("-896987890").ToDouble());
      if(-5.1842734E9f!=DecimalFraction.FromString("-5184273642.760").ToSingle())
        Assert.Fail("decfrac single -5184273642.760\nExpected: -5.1842734E9f\nWas: "+DecimalFraction.FromString("-5184273642.760").ToSingle());
      if(-5.18427364276E9d!=DecimalFraction.FromString("-5184273642.760").ToDouble())
        Assert.Fail("decfrac double -5184273642.760\nExpected: -5.18427364276E9d\nWas: "+DecimalFraction.FromString("-5184273642.760").ToDouble());
      if(5.03393772E17f!=DecimalFraction.FromString("503393788336283974").ToSingle())
        Assert.Fail("decfrac single 503393788336283974\nExpected: 5.03393772E17f\nWas: "+DecimalFraction.FromString("503393788336283974").ToSingle());
      if(5.0339378833628397E17d!=DecimalFraction.FromString("503393788336283974").ToDouble())
        Assert.Fail("decfrac double 503393788336283974\nExpected: 5.0339378833628397E17d\nWas: "+DecimalFraction.FromString("503393788336283974").ToDouble());
      if(-5.50587E15f!=DecimalFraction.FromString("-550587E+10").ToSingle())
        Assert.Fail("decfrac single -550587E+10\nExpected: -5.50587E15f\nWas: "+DecimalFraction.FromString("-550587E+10").ToSingle());
      if(-5.50587E15d!=DecimalFraction.FromString("-550587E+10").ToDouble())
        Assert.Fail("decfrac double -550587E+10\nExpected: -5.50587E15d\nWas: "+DecimalFraction.FromString("-550587E+10").ToDouble());
      if(-4.0559753E-5f!=DecimalFraction.FromString("-405597523930.814E-16").ToSingle())
        Assert.Fail("decfrac single -405597523930.814E-16\nExpected: -4.0559753E-5f\nWas: "+DecimalFraction.FromString("-405597523930.814E-16").ToSingle());
      if(-4.05597523930814E-5d!=DecimalFraction.FromString("-405597523930.814E-16").ToDouble())
        Assert.Fail("decfrac double -405597523930.814E-16\nExpected: -4.05597523930814E-5d\nWas: "+DecimalFraction.FromString("-405597523930.814E-16").ToDouble());
      if(-5.326398E9f!=DecimalFraction.FromString("-5326397977").ToSingle())
        Assert.Fail("decfrac single -5326397977\nExpected: -5.326398E9f\nWas: "+DecimalFraction.FromString("-5326397977").ToSingle());
      if(-5.326397977E9d!=DecimalFraction.FromString("-5326397977").ToDouble())
        Assert.Fail("decfrac double -5326397977\nExpected: -5.326397977E9d\nWas: "+DecimalFraction.FromString("-5326397977").ToDouble());
      if(-9997.447f!=DecimalFraction.FromString("-9997.44701170").ToSingle())
        Assert.Fail("decfrac single -9997.44701170\nExpected: -9997.447f\nWas: "+DecimalFraction.FromString("-9997.44701170").ToSingle());
      if(-9997.4470117d!=DecimalFraction.FromString("-9997.44701170").ToDouble())
        Assert.Fail("decfrac double -9997.44701170\nExpected: -9997.4470117d\nWas: "+DecimalFraction.FromString("-9997.44701170").ToDouble());
      if(7.3258664E7f!=DecimalFraction.FromString("73258664.23970751611061").ToSingle())
        Assert.Fail("decfrac single 73258664.23970751611061\nExpected: 7.3258664E7f\nWas: "+DecimalFraction.FromString("73258664.23970751611061").ToSingle());
      if(7.325866423970751E7d!=DecimalFraction.FromString("73258664.23970751611061").ToDouble())
        Assert.Fail("decfrac double 73258664.23970751611061\nExpected: 7.325866423970751E7d\nWas: "+DecimalFraction.FromString("73258664.23970751611061").ToDouble());
      if(-7.9944785E13f!=DecimalFraction.FromString("-79944788804361.667255656660").ToSingle())
        Assert.Fail("decfrac single -79944788804361.667255656660\nExpected: -7.9944785E13f\nWas: "+DecimalFraction.FromString("-79944788804361.667255656660").ToSingle());
      if(-7.994478880436167E13d!=DecimalFraction.FromString("-79944788804361.667255656660").ToDouble())
        Assert.Fail("decfrac double -79944788804361.667255656660\nExpected: -7.994478880436167E13d\nWas: "+DecimalFraction.FromString("-79944788804361.667255656660").ToDouble());
      if(9.852337E19f!=DecimalFraction.FromString("98523363000987953313E0").ToSingle())
        Assert.Fail("decfrac single 98523363000987953313E0\nExpected: 9.852337E19f\nWas: "+DecimalFraction.FromString("98523363000987953313E0").ToSingle());
      if(9.852336300098796E19d!=DecimalFraction.FromString("98523363000987953313E0").ToDouble())
        Assert.Fail("decfrac double 98523363000987953313E0\nExpected: 9.852336300098796E19d\nWas: "+DecimalFraction.FromString("98523363000987953313E0").ToDouble());
      if(5.981638E15f!=DecimalFraction.FromString("5981637941716431.55749471240993").ToSingle())
        Assert.Fail("decfrac single 5981637941716431.55749471240993\nExpected: 5.981638E15f\nWas: "+DecimalFraction.FromString("5981637941716431.55749471240993").ToSingle());
      if(5.981637941716432E15d!=DecimalFraction.FromString("5981637941716431.55749471240993").ToDouble())
        Assert.Fail("decfrac double 5981637941716431.55749471240993\nExpected: 5.981637941716432E15d\nWas: "+DecimalFraction.FromString("5981637941716431.55749471240993").ToDouble());
      if(-1.995E-9f!=DecimalFraction.FromString("-1995E-12").ToSingle())
        Assert.Fail("decfrac single -1995E-12\nExpected: -1.995E-9f\nWas: "+DecimalFraction.FromString("-1995E-12").ToSingle());
      if(-1.995E-9d!=DecimalFraction.FromString("-1995E-12").ToDouble())
        Assert.Fail("decfrac double -1995E-12\nExpected: -1.995E-9d\nWas: "+DecimalFraction.FromString("-1995E-12").ToDouble());
      if(2.59017677E9f!=DecimalFraction.FromString("2590176810").ToSingle())
        Assert.Fail("decfrac single 2590176810\nExpected: 2.59017677E9f\nWas: "+DecimalFraction.FromString("2590176810").ToSingle());
      if(2.59017681E9d!=DecimalFraction.FromString("2590176810").ToDouble())
        Assert.Fail("decfrac double 2590176810\nExpected: 2.59017681E9d\nWas: "+DecimalFraction.FromString("2590176810").ToDouble());
      if(2.9604614f!=DecimalFraction.FromString("2.960461297").ToSingle())
        Assert.Fail("decfrac single 2.960461297\nExpected: 2.9604614f\nWas: "+DecimalFraction.FromString("2.960461297").ToSingle());
      if(2.960461297d!=DecimalFraction.FromString("2.960461297").ToDouble())
        Assert.Fail("decfrac double 2.960461297\nExpected: 2.960461297d\nWas: "+DecimalFraction.FromString("2.960461297").ToDouble());
      if(768802.0f!=DecimalFraction.FromString("768802").ToSingle())
        Assert.Fail("decfrac single 768802\nExpected: 768802.0f\nWas: "+DecimalFraction.FromString("768802").ToSingle());
      if(768802.0d!=DecimalFraction.FromString("768802").ToDouble())
        Assert.Fail("decfrac double 768802\nExpected: 768802.0d\nWas: "+DecimalFraction.FromString("768802").ToDouble());
      if(145417.38f!=DecimalFraction.FromString("145417.373").ToSingle())
        Assert.Fail("decfrac single 145417.373\nExpected: 145417.38f\nWas: "+DecimalFraction.FromString("145417.373").ToSingle());
      if(145417.373d!=DecimalFraction.FromString("145417.373").ToDouble())
        Assert.Fail("decfrac double 145417.373\nExpected: 145417.373d\nWas: "+DecimalFraction.FromString("145417.373").ToDouble());
      if(540905.0f!=DecimalFraction.FromString("540905").ToSingle())
        Assert.Fail("decfrac single 540905\nExpected: 540905.0f\nWas: "+DecimalFraction.FromString("540905").ToSingle());
      if(540905.0d!=DecimalFraction.FromString("540905").ToDouble())
        Assert.Fail("decfrac double 540905\nExpected: 540905.0d\nWas: "+DecimalFraction.FromString("540905").ToDouble());
      if(-6.811958E20f!=DecimalFraction.FromString("-681.1958019894E+18").ToSingle())
        Assert.Fail("decfrac single -681.1958019894E+18\nExpected: -6.811958E20f\nWas: "+DecimalFraction.FromString("-681.1958019894E+18").ToSingle());
      if(-6.811958019894E20d!=DecimalFraction.FromString("-681.1958019894E+18").ToDouble())
        Assert.Fail("decfrac double -681.1958019894E+18\nExpected: -6.811958019894E20d\nWas: "+DecimalFraction.FromString("-681.1958019894E+18").ToDouble());
      if(54846.0f!=DecimalFraction.FromString("54846.0").ToSingle())
        Assert.Fail("decfrac single 54846.0\nExpected: 54846.0f\nWas: "+DecimalFraction.FromString("54846.0").ToSingle());
      if(54846.0d!=DecimalFraction.FromString("54846.0").ToDouble())
        Assert.Fail("decfrac double 54846.0\nExpected: 54846.0d\nWas: "+DecimalFraction.FromString("54846.0").ToDouble());
      if(9.7245E9f!=DecimalFraction.FromString("97245E+5").ToSingle())
        Assert.Fail("decfrac single 97245E+5\nExpected: 9.7245E9f\nWas: "+DecimalFraction.FromString("97245E+5").ToSingle());
      if(9.7245E9d!=DecimalFraction.FromString("97245E+5").ToDouble())
        Assert.Fail("decfrac double 97245E+5\nExpected: 9.7245E9d\nWas: "+DecimalFraction.FromString("97245E+5").ToDouble());
      if(-26.0f!=DecimalFraction.FromString("-26").ToSingle())
        Assert.Fail("decfrac single -26\nExpected: -26.0f\nWas: "+DecimalFraction.FromString("-26").ToSingle());
      if(-26.0d!=DecimalFraction.FromString("-26").ToDouble())
        Assert.Fail("decfrac double -26\nExpected: -26.0d\nWas: "+DecimalFraction.FromString("-26").ToDouble());
      if(4.15749164E12f!=DecimalFraction.FromString("4157491532482.05").ToSingle())
        Assert.Fail("decfrac single 4157491532482.05\nExpected: 4.15749164E12f\nWas: "+DecimalFraction.FromString("4157491532482.05").ToSingle());
      if(4.15749153248205E12d!=DecimalFraction.FromString("4157491532482.05").ToDouble())
        Assert.Fail("decfrac double 4157491532482.05\nExpected: 4.15749153248205E12d\nWas: "+DecimalFraction.FromString("4157491532482.05").ToDouble());
      if(4.7747967E15f!=DecimalFraction.FromString("4774796769101.23808389660855287E+3").ToSingle())
        Assert.Fail("decfrac single 4774796769101.23808389660855287E+3\nExpected: 4.7747967E15f\nWas: "+DecimalFraction.FromString("4774796769101.23808389660855287E+3").ToSingle());
      if(4.774796769101238E15d!=DecimalFraction.FromString("4774796769101.23808389660855287E+3").ToDouble())
        Assert.Fail("decfrac double 4774796769101.23808389660855287E+3\nExpected: 4.774796769101238E15d\nWas: "+DecimalFraction.FromString("4774796769101.23808389660855287E+3").ToDouble());
      if(-9.8263879E14f!=DecimalFraction.FromString("-982638781021905").ToSingle())
        Assert.Fail("decfrac single -982638781021905\nExpected: -9.8263879E14f\nWas: "+DecimalFraction.FromString("-982638781021905").ToSingle());
      if(-9.82638781021905E14d!=DecimalFraction.FromString("-982638781021905").ToDouble())
        Assert.Fail("decfrac double -982638781021905\nExpected: -9.82638781021905E14d\nWas: "+DecimalFraction.FromString("-982638781021905").ToDouble());
      if(8.8043432E18f!=DecimalFraction.FromString("8804343287262864743").ToSingle())
        Assert.Fail("decfrac single 8804343287262864743\nExpected: 8.8043432E18f\nWas: "+DecimalFraction.FromString("8804343287262864743").ToSingle());
      if(8.8043432872628644E18d!=DecimalFraction.FromString("8804343287262864743").ToDouble())
        Assert.Fail("decfrac double 8804343287262864743\nExpected: 8.8043432872628644E18d\nWas: "+DecimalFraction.FromString("8804343287262864743").ToDouble());
      if(-6.5138669E13f!=DecimalFraction.FromString("-65138668135711").ToSingle())
        Assert.Fail("decfrac single -65138668135711\nExpected: -6.5138669E13f\nWas: "+DecimalFraction.FromString("-65138668135711").ToSingle());
      if(-6.5138668135711E13d!=DecimalFraction.FromString("-65138668135711").ToDouble())
        Assert.Fail("decfrac double -65138668135711\nExpected: -6.5138668135711E13d\nWas: "+DecimalFraction.FromString("-65138668135711").ToDouble());
      if(-5.9235733E15f!=DecimalFraction.FromString("-5923573055061163").ToSingle())
        Assert.Fail("decfrac single -5923573055061163\nExpected: -5.9235733E15f\nWas: "+DecimalFraction.FromString("-5923573055061163").ToSingle());
      if(-5.923573055061163E15d!=DecimalFraction.FromString("-5923573055061163").ToDouble())
        Assert.Fail("decfrac double -5923573055061163\nExpected: -5.923573055061163E15d\nWas: "+DecimalFraction.FromString("-5923573055061163").ToDouble());
      if(-8.6853E-8f!=DecimalFraction.FromString("-8.6853E-8").ToSingle())
        Assert.Fail("decfrac single -8.6853E-8\nExpected: -8.6853E-8f\nWas: "+DecimalFraction.FromString("-8.6853E-8").ToSingle());
      if(-8.6853E-8d!=DecimalFraction.FromString("-8.6853E-8").ToDouble())
        Assert.Fail("decfrac double -8.6853E-8\nExpected: -8.6853E-8d\nWas: "+DecimalFraction.FromString("-8.6853E-8").ToDouble());
      if(19707.0f!=DecimalFraction.FromString("19707").ToSingle())
        Assert.Fail("decfrac single 19707\nExpected: 19707.0f\nWas: "+DecimalFraction.FromString("19707").ToSingle());
      if(19707.0d!=DecimalFraction.FromString("19707").ToDouble())
        Assert.Fail("decfrac double 19707\nExpected: 19707.0d\nWas: "+DecimalFraction.FromString("19707").ToDouble());
      if(-8.8478554E14f!=DecimalFraction.FromString("-884785536200446.1859332080").ToSingle())
        Assert.Fail("decfrac single -884785536200446.1859332080\nExpected: -8.8478554E14f\nWas: "+DecimalFraction.FromString("-884785536200446.1859332080").ToSingle());
      if(-8.847855362004461E14d!=DecimalFraction.FromString("-884785536200446.1859332080").ToDouble())
        Assert.Fail("decfrac double -884785536200446.1859332080\nExpected: -8.847855362004461E14d\nWas: "+DecimalFraction.FromString("-884785536200446.1859332080").ToDouble());
      if(-1.0f!=DecimalFraction.FromString("-1").ToSingle())
        Assert.Fail("decfrac single -1\nExpected: -1.0f\nWas: "+DecimalFraction.FromString("-1").ToSingle());
      if(-1.0d!=DecimalFraction.FromString("-1").ToDouble())
        Assert.Fail("decfrac double -1\nExpected: -1.0d\nWas: "+DecimalFraction.FromString("-1").ToDouble());
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
      if(!CBORObject.FromObject(Double.PositiveInfinity).IsPositiveInfinity())
        Assert.Fail("Not positive infinity");
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
          CompareTestLess(oldobj,o);
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