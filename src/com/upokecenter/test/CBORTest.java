package com.upokecenter.test;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

import java.io.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;

    /**
     * Contains CBOR tests.
     * @param r A FastRandom object.
     */

  public class CBORTest {
    private static void TestExtendedFloatDoubleCore(double d, String s) {
      double oldd = d;
      ExtendedFloat bf = ExtendedFloat.FromDouble(d);
      if (s != null) {
        Assert.assertEquals(s, bf.toString());
      }
      d = bf.ToDouble();
      Assert.assertEquals((double)oldd,d,0);
      TestCommon.AssertRoundTrip(CBORObject.FromObject(bf));
      TestCommon.AssertRoundTrip(CBORObject.FromObject(d));
    }

    private static void TestExtendedFloatSingleCore(float d, String s) {
      float oldd = d;
      ExtendedFloat bf = ExtendedFloat.FromSingle(d);
      if (s != null) {
        Assert.assertEquals(s, bf.toString());
      }
      d = bf.ToSingle();
      Assert.assertEquals((float)oldd,d,0f);
      TestCommon.AssertRoundTrip(CBORObject.FromObject(bf));
      TestCommon.AssertRoundTrip(CBORObject.FromObject(d));
    }

    public static CBORObject RandomNumber(FastRandom rand) {
      switch (rand.NextValue(6)) {
        case 0:
          return CBORObject.FromObject(RandomDouble(rand, Integer.MAX_VALUE));
        case 1:
          return CBORObject.FromObject(RandomSingle(rand, Integer.MAX_VALUE));
        case 2:
          return CBORObject.FromObject(RandomBigInteger(rand));
        case 3:
          return CBORObject.FromObject(RandomExtendedFloat(rand));
        case 4:
          return CBORObject.FromObject(RandomExtendedDecimal(rand));
        case 5:
          return CBORObject.FromObject(RandomInt64(rand));
        default:
          throw new IllegalArgumentException();
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
      if (exponent == Integer.MAX_VALUE) {
        exponent = rand.NextValue(2047);
      }
      long r = rand.NextValue(0x10000);
      r |= ((long)rand.NextValue(0x10000)) << 16;
      if (rand.NextValue(2) == 0) {
        r |= ((long)rand.NextValue(0x10000)) << 32;
        if (rand.NextValue(2) == 0) {
          r |= ((long)rand.NextValue(0x10000)) << 48;
        }
      }
      r &= ~0x7FF0000000000000L;  // clear exponent
      r |= ((long)exponent) << 52;  // set exponent
      return Double.longBitsToDouble(r);
    }

    private static float RandomSingle(FastRandom rand, int exponent) {
      if (exponent == Integer.MAX_VALUE) {
        exponent = rand.NextValue(255);
      }
      int r = rand.NextValue(0x10000);
      if (rand.NextValue(2) == 0) {
        r |= ((int)rand.NextValue(0x10000)) << 16;
      }
      r &= ~0x7F800000;  // clear exponent
      r |= ((int)exponent) << 23;  // set exponent
      return Float.intBitsToFloat(r);
    }

    public static ExtendedDecimal RandomExtendedDecimal(FastRandom r) {
      return ExtendedDecimal.FromString(RandomDecimalString(r));
    }

    public static BigInteger RandomBigInteger(FastRandom r) {
      int count = r.NextValue(60) + 1;
      byte[] bytes = new byte[count];
       for (int i = 0; i < count; ++i) {
        bytes[i] = (byte)((int)r.NextValue(256));
      }
      return BigInteger.fromByteArray((byte[])bytes,true);
    }

    public static ExtendedFloat RandomExtendedFloat(FastRandom r) {
      return ExtendedFloat.Create(RandomBigInteger(r), BigInteger.valueOf(r.NextValue(400) - 200));
    }

    public static String RandomBigIntString(FastRandom r) {
      int count = r.NextValue(50) + 1;
      StringBuilder sb = new StringBuilder();
      if (r.NextValue(2) == 0) {
        sb.append('-');
      }
      for (int i = 0; i < count; ++i) {
        if (i == 0) {
          sb.append((char)('1' + r.NextValue(9)));
        } else {
          sb.append((char)('0' + r.NextValue(10)));
        }
      }
      return sb.toString();
    }

    public static String RandomDecimalString(FastRandom r) {
      int count = r.NextValue(20) + 1;
      StringBuilder sb = new StringBuilder();
      if (r.NextValue(2) == 0) {
        sb.append('-');
      }
      for (int i = 0; i < count; ++i) {
        if (i == 0) {
          sb.append((char)('1' + r.NextValue(9)));
        } else {
          sb.append((char)('0' + r.NextValue(10)));
        }
      }
      if (r.NextValue(2) == 0) {
        sb.append('.');
        count = r.NextValue(20) + 1;
        for (int i = 0; i < count; ++i) {
          sb.append((char)('0' + r.NextValue(10)));
        }
      }
      if (r.NextValue(2) == 0) {
        sb.append('E');
        count = r.NextValue(20);
        if (count != 0) {
          sb.append(r.NextValue(2) == 0 ? '+' : '-');
        }
        sb.append(Integer.toString((int)count));
      }
      return sb.toString();
    }

    private static void TestDecimalString(String r) {
      CBORObject o = CBORObject.FromObject(ExtendedDecimal.FromString(r));
      CBORObject o2 = CBORDataUtilities.ParseJSONNumber(r);
      CompareTestEqual(o, o2);
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestAdd() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 3000; ++i) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        ExtendedDecimal cmpDecFrac = o1.AsExtendedDecimal().Add(o2.AsExtendedDecimal());
        ExtendedDecimal cmpCobj = CBORObject.Addition(o1, o2).AsExtendedDecimal();
        if (cmpDecFrac.compareTo(cmpCobj) != 0) {
          Assert.assertEquals(ObjectMessages(o1, o2, "Results don't match"),0,cmpDecFrac.compareTo(cmpCobj));
        }
        TestCommon.AssertRoundTrip(o1);
        TestCommon.AssertRoundTrip(o2);
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestSubtract() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 3000; ++i) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        ExtendedDecimal cmpDecFrac = o1.AsExtendedDecimal().Subtract(o2.AsExtendedDecimal());
        ExtendedDecimal cmpCobj = CBORObject.Subtract(o1, o2).AsExtendedDecimal();
        if (cmpDecFrac.compareTo(cmpCobj) != 0) {
          Assert.assertEquals(ObjectMessages(o1, o2, "Results don't match"),0,cmpDecFrac.compareTo(cmpCobj));
        }
        TestCommon.AssertRoundTrip(o1);
        TestCommon.AssertRoundTrip(o2);
      }
    }

    private static String ObjectMessages(CBORObject o1, CBORObject o2, String s) {
      if (o1.getType() == CBORType.Number && o2.getType() == CBORType.Number) {
        return s + ":\n" + o1.toString() + " and\n" + o2.toString() + "\nOR\n" +
          o1.AsExtendedDecimal().toString() + " and\n" + o2.AsExtendedDecimal().toString() + "\nOR\n" +
          "AddSubCompare(" + ToByteArrayString(o1) + ",\n" + ToByteArrayString(o2) + ");";
      } else {
        return s + ":\n" + o1.toString() + " and\n" + o2.toString() + "\nOR\n" +
          ToByteArrayString(o1) + " and\n" + ToByteArrayString(o2);
      }
    }

    private static void CompareTestEqual(CBORObject o1, CBORObject o2) {
      if (CompareTestReciprocal(o1, o2) != 0) {
        Assert.fail(ObjectMessages(o1, o2, "Not equal: " + CompareTestReciprocal(o1, o2)));
      }
    }

    private static void CompareTestLess(CBORObject o1, CBORObject o2) {
      if (CompareTestReciprocal(o1, o2) >= 0) {
        Assert.fail(ObjectMessages(o1, o2, "Not less: " + CompareTestReciprocal(o1, o2)));
      }
    }

    private static int CompareTestReciprocal(CBORObject o1, CBORObject o2) {
      if (o1 == null) {
        throw new NullPointerException("o1");
      }
      if (o2 == null) {
        throw new NullPointerException("o2");
      }
      int cmp = o1.compareTo(o2);
      int cmp2 = o2.compareTo(o1);
      if (-cmp2 != cmp) {
        Assert.assertEquals(ObjectMessages(o1, o2, "Not reciprocal"),cmp,-cmp2);
      }
      return cmp;
    }

    public static String ToByteArrayString(CBORObject obj) {
      byte[] bytes = obj.EncodeToBytes();
      StringBuilder sb = new StringBuilder();
      String hex = "0123456789ABCDEF";
      sb.append("CBORObject.DecodeFromBytes(new byte[] {  ");
      for (int i = 0; i < bytes.length; ++i) {
        if (i > 0) {
          sb.append(",");
         }
        if ((bytes[i] & 0x80) != 0) {
          sb.append("(byte)0x");
        } else {
          sb.append("0x");
        }
        sb.append(hex.charAt((bytes[i] >> 4) & 0xF));
        sb.append(hex.charAt(bytes[i] & 0xF));
      }
      sb.append("})");
      return sb.toString();
    }

    @Test
    public void TestDecFracCompareIntegerVsBigFraction() {
      ExtendedDecimal a = ExtendedDecimal.FromString("7.004689238424764477580371752455515117709288087566222056632084784688080253355047487262563521426272927783429622650146484375");
      ExtendedDecimal b = ExtendedDecimal.FromString("5");
      Assert.assertEquals(1, a.compareTo(b));
      Assert.assertEquals(-1, b.compareTo(a));
      CBORObject o1 = null;
      CBORObject o2 = null;
      o1 = CBORObject.DecodeFromBytes(new byte[] {  (byte)0xFB, (byte)0x8B, 0x44, (byte)0xF2, (byte)0xA9, 0x0C, 0x27, 0x42, 0x28  });
      o2 = CBORObject.DecodeFromBytes(new byte[] {  (byte)0xC5, (byte)0x82, 0x38, (byte)0xA4, (byte)0xC3, 0x50, 0x02, (byte)0x98,
                                        (byte)0xC5, (byte)0xA8, 0x02, (byte)0xC1, (byte)0xF6, (byte)0xC0, 0x1A, (byte)0xBE, 0x08,
                                        0x04, (byte)0x86, (byte)0x99, 0x3E, (byte)0xF1  });
      AddSubCompare(o1, o2);
    }

    private static void CompareDecimals(CBORObject o1, CBORObject o2) {
      int cmpDecFrac = o1.AsExtendedDecimal().compareTo(o2.AsExtendedDecimal());
      int cmpCobj = CompareTestReciprocal(o1, o2);
      if (cmpDecFrac != cmpCobj) {
        Assert.assertEquals(ObjectMessages(o1, o2, "Compare: Results don't match"),cmpDecFrac,cmpCobj);
      }
      TestCommon.AssertRoundTrip(o1);
      TestCommon.AssertRoundTrip(o2);
    }

    private static void AddSubCompare(CBORObject o1, CBORObject o2) {
      ExtendedDecimal cmpDecFrac = o1.AsExtendedDecimal().Add(o2.AsExtendedDecimal());
      ExtendedDecimal cmpCobj = CBORObject.Addition(o1, o2).AsExtendedDecimal();
      if (cmpDecFrac.compareTo(cmpCobj) != 0) {
        Assert.assertEquals(ObjectMessages(o1, o2, "Add: Results don't match:\n" + cmpDecFrac + " vs\n" + cmpCobj),0,cmpDecFrac.compareTo(cmpCobj));
      }
      cmpDecFrac = o1.AsExtendedDecimal().Subtract(o2.AsExtendedDecimal());
      cmpCobj = CBORObject.Subtract(o1, o2).AsExtendedDecimal();
      if (cmpDecFrac.compareTo(cmpCobj) != 0) {
        Assert.assertEquals(ObjectMessages(o1, o2, "Subtract: Results don't match:\n" + cmpDecFrac + " vs\n" + cmpCobj),0,cmpDecFrac.compareTo(cmpCobj));
      }
      CompareDecimals(o1, o2);
    }

    @Test
    public void TestCompareB() {
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] {  0x1A, (byte)0xFC, 0x1A, (byte)0xB0, 0x52  }),
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xC5, (byte)0x82, 0x38, 0x5F, (byte)0xC2, 0x50, 0x08, 0x70, (byte)0xF3, (byte)0xC4, (byte)0x90, 0x4C, 0x14, (byte)0xBA, 0x59, (byte)0xF0, (byte)0xC6, (byte)0xCB, (byte)0x8C, (byte)0x8D, 0x40, (byte)0x80  }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xC5, (byte)0x82, 0x38, (byte)0xC7, 0x3B, 0x00, 0x00, 0x08, (byte)0xBF, (byte)0xDA, (byte)0xAF, 0x73, 0x46  }),
        CBORObject.DecodeFromBytes(new byte[] {  0x3B, 0x5A, (byte)0x9B, (byte)0x9A, (byte)0x9C, (byte)0xB4, (byte)0x95, (byte)0xBF, 0x71  }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] {  0x1A, (byte)0xBB, 0x0C, (byte)0xF7, 0x52  }),
        CBORObject.DecodeFromBytes(new byte[] {  0x1A, (byte)0x82, 0x00, (byte)0xBF, (byte)0xF9  }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xFA, 0x1F, (byte)0x80, (byte)0xDB, (byte)0x9B  }),
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xFB, 0x31, (byte)0x90, (byte)0xEA, 0x16, (byte)0xBE, (byte)0x80, 0x0B, 0x37  }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xFB, 0x3C, 0x00, (byte)0xCF, (byte)0xB6, (byte)0xBD, (byte)0xFF, 0x37, 0x38  }),
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xFA, 0x30, (byte)0x80, 0x75, 0x63  }));
      AddSubCompare(
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xC5, (byte)0x82, 0x38, 0x7D, 0x3A, 0x06, (byte)0xBC, (byte)0xD5, (byte)0xB8  }),
        CBORObject.DecodeFromBytes(new byte[] {  0x38, 0x5C  }));
    }

    @Test
    public void ExtraDecimalTests() {
      Assert.assertEquals(
        ExtendedDecimal.NegativeInfinity,
        ExtendedDecimal.FromString("-79228162514264337593543950336").RoundToBinaryPrecision(PrecisionContext.CliDecimal));
      Assert.assertEquals(
        ExtendedDecimal.PositiveInfinity,
        ExtendedDecimal.FromString("8.782580686213340724E+28").RoundToBinaryPrecision(PrecisionContext.CliDecimal));
      Assert.assertEquals(
        ExtendedDecimal.NegativeInfinity,
        ExtendedDecimal.FromString("-9.3168444507547E+28").RoundToBinaryPrecision(PrecisionContext.CliDecimal));
      Assert.assertEquals(
        "-9344285899206687626894794544",
        ExtendedDecimal.FromString("-9344285899206687626894794544.04982268810272216796875").RoundToBinaryPrecision(new PrecisionContext(96, Rounding.HalfEven, 0, 28, false)).ToPlainString());
      Assert.assertEquals(
        ExtendedDecimal.PositiveInfinity,
        ExtendedDecimal.FromString("96148154858060747311034406200").RoundToBinaryPrecision(PrecisionContext.CliDecimal));
      Assert.assertEquals(
        ExtendedDecimal.PositiveInfinity,
        ExtendedDecimal.FromString("90246605365627217170000000000").RoundToBinaryPrecision(PrecisionContext.CliDecimal));
    }

    @Test
    public void TestFloatDecimalRoundTrip() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 5000; ++i) {
        ExtendedFloat ef = RandomExtendedFloat(r);
        ExtendedDecimal ed = ef.ToExtendedDecimal();
        ExtendedFloat ef2 = ed.ToExtendedFloat();
        // Tests that values converted from float to decimal and
        // back have the same numerical value
        if (ef.compareTo(ef2) != 0) {
          Assert.assertEquals("TestFloatDecimalRoundTrip " + ef + "; " + ef2,0,ef.compareTo(ef2));
        }
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestCompare() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 5000; ++i) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        CompareDecimals(o1, o2);
      }
      for (int i = 0; i < 50; ++i) {
        CBORObject o1 = CBORObject.FromObject(Float.NEGATIVE_INFINITY);
        CBORObject o2 = RandomNumber(r);
        CompareTestLess(o1, o2);
        o1 = CBORObject.FromObject(Double.NEGATIVE_INFINITY);
        CompareTestLess(o1, o2);
        o1 = CBORObject.FromObject(Float.POSITIVE_INFINITY);
        CompareTestLess(o2, o1);
        o1 = CBORObject.FromObject(Double.POSITIVE_INFINITY);
        CompareTestLess(o2, o1);
        o1 = CBORObject.FromObject(Float.NaN);
        CompareTestLess(o2, o1);
        o1 = CBORObject.FromObject(Double.NaN);
        CompareTestLess(o2, o1);
      }
      CBORObject sp = CBORObject.FromObject(Float.POSITIVE_INFINITY);
      CBORObject sn = CBORObject.FromObject(Float.NEGATIVE_INFINITY);
      CBORObject snan = CBORObject.FromObject(Float.NaN);
      CBORObject dp = CBORObject.FromObject(Double.POSITIVE_INFINITY);
      CBORObject dn = CBORObject.FromObject(Double.NEGATIVE_INFINITY);
      CBORObject dnan = CBORObject.FromObject(Double.NaN);
      CompareTestEqual(sp, sp);
      CompareTestEqual(sp, dp);
      CompareTestEqual(dp, dp);
      CompareTestEqual(sn, sn);
      CompareTestEqual(sn, dn);
      CompareTestEqual(dn, dn);
      CompareTestEqual(snan, snan);
      CompareTestEqual(snan, dnan);
      CompareTestEqual(dnan, dnan);
      CompareTestLess(sn, sp);
      CompareTestLess(sn, dp);
      CompareTestLess(sn, snan);
      CompareTestLess(sn, dnan);
      CompareTestLess(sp, snan);
      CompareTestLess(sp, dnan);
      CompareTestLess(dn, dp);
      CompareTestLess(dp, dnan);
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestParseDecimalStrings() {
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 3000; ++i) {
        String r = RandomDecimalString(rand);
        TestDecimalString(r);
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestRandomData() {
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 200; ++i) {
        byte[] array = new byte[rand.NextValue(1000000) + 1];
        for (int j = 0; j < array.length; ++j) {
          if (j + 3 <= array.length) {
            int r = rand.NextValue(0x1000000);
            array[j] = (byte)(r & 0xFF);
            array[j + 1] = (byte)((r >> 8) & 0xFF);
            array[j + 2] = (byte)((r >> 16) & 0xFF);
            j += 2;
          } else {
            array[j] = (byte)rand.NextValue(256);
          }
        }
        java.io.ByteArrayInputStream ms=null;
try {
ms=new ByteArrayInputStream(array);
int startingAvailable=ms.available();

          while ((startingAvailable-ms.available()) != startingAvailable) {
            try {
              CBORObject o = CBORObject.Read(ms);
              if (o == null) {
                Assert.fail("Object read is null");
              }
            } catch (CBORException ex) {
              // Expected exception
            }
          }
}
finally {
try { if(ms!=null)ms.close(); } catch (IOException ex){}
}
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestExtendedFloatSingle() {
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 255; ++i) {  // Try a random float with a given exponent
        TestExtendedFloatSingleCore(RandomSingle(rand, i), null);
        TestExtendedFloatSingleCore(RandomSingle(rand, i), null);
        TestExtendedFloatSingleCore(RandomSingle(rand, i), null);
        TestExtendedFloatSingleCore(RandomSingle(rand, i), null);
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestExtendedFloatDouble() {
      TestExtendedFloatDoubleCore(3.5, "3.5");
      TestExtendedFloatDoubleCore(7, "7");
      TestExtendedFloatDoubleCore(1.75, "1.75");
      TestExtendedFloatDoubleCore(3.5, "3.5");
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 2047; ++i) {  // Try a random double with a given exponent
        TestExtendedFloatDoubleCore(RandomDouble(rand, i), null);
        TestExtendedFloatDoubleCore(RandomDouble(rand, i), null);
        TestExtendedFloatDoubleCore(RandomDouble(rand, i), null);
        TestExtendedFloatDoubleCore(RandomDouble(rand, i), null);
      }
    }
    @Test(expected=CBORException.class)
    public void TestTagThenBreak() {
      TestCommon.FromBytesTestAB(new byte[] {  (byte)0xD1, (byte)0xFF  });
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestJSONSurrogates() {
      try {
        CBORObject.FromJSONString("[\"\ud800\udc00\"]");
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\\udc00\"]");
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\ud800\\udc00\"]");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\udc00\"]");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\udc00\ud800\udc00\"]");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\ud800\udc00\"]");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[\"\\ud800\\udc00\ud800\udc00\"]");
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestJSONEscapedChars() {
      CBORObject o = CBORObject.FromJSONString(
        "[\"\\r\\n\\u0006\\\\\\\"\"]");
      Assert.assertEquals(1, o.size());
      Assert.assertEquals("\r\n\u0006\\\"", o.get(0).AsString());
      Assert.assertEquals(
"[\"\\r\\n\\u0006\\\\\\\"\"]",
o.ToJSONString());
      TestCommon.AssertRoundTrip(o);
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestCBORFromArray() {
      CBORObject o = CBORObject.FromObject(new int[] { 1, 2, 3 });
      Assert.assertEquals(3, o.size());
      Assert.assertEquals(1, o.get(0).AsInt32());
      Assert.assertEquals(2, o.get(1).AsInt32());
      Assert.assertEquals(3, o.get(2).AsInt32());
      TestCommon.AssertRoundTrip(o);
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestJSON() {
      CBORObject o;
      o = CBORObject.FromJSONString("[1,2,null,true,false]");
      try {
        CBORObject.FromJSONString("[\"\\d800\"]"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[1,2,"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("[1,2,3"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromJSONString("{,\"0\"=>0,\"1\"=>1}");
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
} catch (Exception ex) {
  Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
}
      try { CBORObject.FromJSONString("{\"0\"=>0,,\"1\"=>1}"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
} catch (Exception ex) {
  Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
}
      try { CBORObject.FromJSONString("{\"0\"=>0,\"1\"=>1,}"); Assert.fail("Should have failed");
      } catch (CBORException ex) {
} catch (Exception ex) {
  Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
}
      try {
  CBORObject.FromJSONString("[,0,1,2]"); Assert.fail("Should have failed");
} catch (CBORException ex) {
} catch (Exception ex) {
  Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
}
      try {
  CBORObject.FromJSONString("[0,,1,2]"); Assert.fail("Should have failed");
} catch (CBORException ex) {
} catch (Exception ex) {
  Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
}
      try {
  CBORObject.FromJSONString("[0,1,,2]"); Assert.fail("Should have failed");
} catch (CBORException ex) {
} catch (Exception ex) {
  Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
}
      try {
  CBORObject.FromJSONString("[0,1,2,]"); Assert.fail("Should have failed");
} catch (CBORException ex) {
} catch (Exception ex) {
  Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
}
      Assert.assertEquals(5, o.size());
      Assert.assertEquals(1, o.get(0).AsInt32());
      Assert.assertEquals(2, o.get(1).AsInt32());
      Assert.assertEquals(CBORObject.Null, o.get(2));
      Assert.assertEquals(CBORObject.True, o.get(3));
      Assert.assertEquals(CBORObject.False, o.get(4));
      o = CBORObject.FromJSONString("[1.5,2.6,3.7,4.0,222.22]");
      double actual = o.get(0).AsDouble();
      Assert.assertEquals((double)1.5,actual,0);
      Assert.assertEquals("true", CBORObject.True.ToJSONString());
      Assert.assertEquals("false", CBORObject.False.ToJSONString());
      Assert.assertEquals("null", CBORObject.Null.ToJSONString());
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestByte() {
      for (int i = 0; i <= 255; ++i) {
        TestCommon.AssertSer(
          CBORObject.FromObject((byte)i),
          String.format(java.util.Locale.US,"%s", i));
      }
    }

    /**
     * Not documented yet.
     * @param bytes A byte array.
     * @param expectedRet A 32-bit signed integer.
     * @param expectedString A string object.
     * @param noReplaceRet A 32-bit signed integer. (2).
     * @param noReplaceString A string object. (2).
     */
    public void DoTestReadUtf8(
      byte[] bytes,
      int expectedRet,
      String expectedString,
      int noReplaceRet,
      String noReplaceString) {
      this.DoTestReadUtf8(
        bytes,
        bytes.length,
        expectedRet,
        expectedString,
        noReplaceRet,
        noReplaceString);
    }

    /**
     * Not documented yet.
     * @param bytes A byte array.
     * @param length A 32-bit signed integer.
     * @param expectedRet A 32-bit signed integer. (2).
     * @param expectedString A string object.
     * @param noReplaceRet A 32-bit signed integer. (3).
     * @param noReplaceString A string object. (2).
     */
    public void DoTestReadUtf8(
      byte[] bytes,
      int length,
      int expectedRet,
      String expectedString,
      int noReplaceRet,
      String noReplaceString) {
      try {
        StringBuilder builder = new StringBuilder();
        int ret = 0;
        java.io.ByteArrayInputStream ms=null;
try {
ms=new ByteArrayInputStream(bytes);

          ret = DataUtilities.ReadUtf8(ms, length, builder, true);
          Assert.assertEquals(expectedRet, ret);
          if (expectedRet == 0) {
            Assert.assertEquals(expectedString, builder.toString());
          }
          ms.reset();
          builder.setLength(0);
          ret = DataUtilities.ReadUtf8(ms, length, builder, false);
          Assert.assertEquals(noReplaceRet, ret);
          if (noReplaceRet == 0) {
            Assert.assertEquals(noReplaceString, builder.toString());
          }
}
finally {
try { if(ms!=null)ms.close(); } catch (IOException ex){}
}
        if (bytes.length >= length) {
          builder.setLength(0);
          ret = DataUtilities.ReadUtf8FromBytes(bytes, 0, length, builder, true);
          Assert.assertEquals(expectedRet, ret);
          if (expectedRet == 0) {
            Assert.assertEquals(expectedString, builder.toString());
          }
          builder.setLength(0);
          ret = DataUtilities.ReadUtf8FromBytes(bytes, 0, length, builder, false);
          Assert.assertEquals(noReplaceRet, ret);
          if (noReplaceRet == 0) {
            Assert.assertEquals(noReplaceString, builder.toString());
          }
        }
      } catch (IOException ex) {
        throw new CBORException("", ex);
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestDecFracOverflow() {
      Assert.assertEquals(ExtendedDecimal.PositiveInfinity, CBORObject.FromObject(Float.POSITIVE_INFINITY).AsExtendedDecimal());
      Assert.assertEquals(ExtendedDecimal.NegativeInfinity, CBORObject.FromObject(Float.NEGATIVE_INFINITY).AsExtendedDecimal());
      Assert.assertTrue(CBORObject.FromObject(Float.NaN).AsExtendedDecimal().IsNaN());
      Assert.assertEquals(ExtendedDecimal.PositiveInfinity, CBORObject.FromObject(Double.POSITIVE_INFINITY).AsExtendedDecimal());
      Assert.assertEquals(ExtendedDecimal.NegativeInfinity, CBORObject.FromObject(Double.NEGATIVE_INFINITY).AsExtendedDecimal());
      Assert.assertTrue(CBORObject.FromObject(Double.NaN).AsExtendedDecimal().IsNaN());
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestFPToBigInteger() {
      Assert.assertEquals("0", CBORObject.FromObject((float)0.75).AsBigInteger().toString());
      Assert.assertEquals("0", CBORObject.FromObject((float)0.99).AsBigInteger().toString());
      Assert.assertEquals("0", CBORObject.FromObject((float)0.0000000000000001).AsBigInteger().toString());
      Assert.assertEquals("0", CBORObject.FromObject((float)0.5).AsBigInteger().toString());
      Assert.assertEquals("1", CBORObject.FromObject((float)1.5).AsBigInteger().toString());
      Assert.assertEquals("2", CBORObject.FromObject((float)2.5).AsBigInteger().toString());
      Assert.assertEquals("328323", CBORObject.FromObject((float)328323f).AsBigInteger().toString());
      Assert.assertEquals("0", CBORObject.FromObject((double)0.75).AsBigInteger().toString());
      Assert.assertEquals("0", CBORObject.FromObject((double)0.99).AsBigInteger().toString());
      Assert.assertEquals("0", CBORObject.FromObject((double)0.0000000000000001).AsBigInteger().toString());
      Assert.assertEquals("0", CBORObject.FromObject((double)0.5).AsBigInteger().toString());
      Assert.assertEquals("1", CBORObject.FromObject((double)1.5).AsBigInteger().toString());
      Assert.assertEquals("2", CBORObject.FromObject((double)2.5).AsBigInteger().toString());
      Assert.assertEquals("328323", CBORObject.FromObject((double)328323).AsBigInteger().toString());
      try {
        CBORObject.FromObject(Float.POSITIVE_INFINITY).AsBigInteger(); Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(Float.NEGATIVE_INFINITY).AsBigInteger(); Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(Float.NaN).AsBigInteger(); Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(Double.POSITIVE_INFINITY).AsBigInteger(); Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(Double.NEGATIVE_INFINITY).AsBigInteger(); Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(Double.NaN).AsBigInteger(); Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestDecFracFP() {
      Assert.assertEquals("0.75", ExtendedDecimal.FromDouble(0.75).toString());
      Assert.assertEquals("0.5", ExtendedDecimal.FromDouble(0.5).toString());
      Assert.assertEquals("0.25", ExtendedDecimal.FromDouble(0.25).toString());
      Assert.assertEquals("0.875", ExtendedDecimal.FromDouble(0.875).toString());
      Assert.assertEquals("0.125", ExtendedDecimal.FromDouble(0.125).toString());
      Assert.assertEquals("0.75", ExtendedDecimal.FromSingle(0.75f).toString());
      Assert.assertEquals("0.5", ExtendedDecimal.FromSingle(0.5f).toString());
      Assert.assertEquals("0.25", ExtendedDecimal.FromSingle(0.25f).toString());
      Assert.assertEquals("0.875", ExtendedDecimal.FromSingle(0.875f).toString());
      Assert.assertEquals("0.125", ExtendedDecimal.FromSingle(0.125f).toString());
    }

    /**
     * Not documented yet.
     */
    @Test
    public void ScaleTest() {
      Assert.assertEquals((BigInteger.valueOf(7)).negate(), ExtendedDecimal.FromString("1.265e-4").getExponent());
      Assert.assertEquals((BigInteger.valueOf(4)).negate(), ExtendedDecimal.FromString("0.000E-1").getExponent());
      Assert.assertEquals((BigInteger.valueOf(16)).negate(), ExtendedDecimal.FromString("0.57484848535648e-2").getExponent());
      Assert.assertEquals((BigInteger.valueOf(22)).negate(), ExtendedDecimal.FromString("0.485448e-16").getExponent());
      Assert.assertEquals((BigInteger.valueOf(20)).negate(), ExtendedDecimal.FromString("0.5657575351495151495649565150e+8").getExponent());
      Assert.assertEquals((BigInteger.TEN).negate(), ExtendedDecimal.FromString("0e-10").getExponent());
      Assert.assertEquals((BigInteger.valueOf(17)).negate(), ExtendedDecimal.FromString("0.504952e-11").getExponent());
      Assert.assertEquals((BigInteger.valueOf(13)).negate(), ExtendedDecimal.FromString("0e-13").getExponent());
      Assert.assertEquals((BigInteger.valueOf(43)).negate(), ExtendedDecimal.FromString("0.49495052535648555757515648e-17").getExponent());
      Assert.assertEquals(BigInteger.valueOf(7), ExtendedDecimal.FromString("0.485654575150e+19").getExponent());
      Assert.assertEquals((BigInteger.ZERO).negate(), ExtendedDecimal.FromString("0.48515648e+8").getExponent());
      Assert.assertEquals((BigInteger.valueOf(45)).negate(), ExtendedDecimal.FromString("0.49485251485649535552535451544956e-13").getExponent());
      Assert.assertEquals((BigInteger.valueOf(6)).negate(), ExtendedDecimal.FromString("0.565754515152575448505257e+18").getExponent());
      Assert.assertEquals(BigInteger.valueOf(16), ExtendedDecimal.FromString("0e+16").getExponent());
      Assert.assertEquals(BigInteger.valueOf(6), ExtendedDecimal.FromString("0.5650e+10").getExponent());
      Assert.assertEquals((BigInteger.valueOf(5)).negate(), ExtendedDecimal.FromString("0.49555554575756575556e+15").getExponent());
      Assert.assertEquals((BigInteger.valueOf(37)).negate(), ExtendedDecimal.FromString("0.57494855545057534955e-17").getExponent());
      Assert.assertEquals((BigInteger.valueOf(25)).negate(), ExtendedDecimal.FromString("0.4956504855525748575456e-3").getExponent());
      Assert.assertEquals((BigInteger.valueOf(26)).negate(), ExtendedDecimal.FromString("0.55575355495654484948525354545053494854e+12").getExponent());
      Assert.assertEquals((BigInteger.valueOf(22)).negate(), ExtendedDecimal.FromString("0.484853575350494950575749545057e+8").getExponent());
      Assert.assertEquals(BigInteger.valueOf(11), ExtendedDecimal.FromString("0.52545451e+19").getExponent());
      Assert.assertEquals((BigInteger.valueOf(29)).negate(), ExtendedDecimal.FromString("0.48485654495751485754e-9").getExponent());
      Assert.assertEquals((BigInteger.valueOf(38)).negate(), ExtendedDecimal.FromString("0.56525456555549545257535556495655574848e+0").getExponent());
      Assert.assertEquals((BigInteger.valueOf(15)).negate(), ExtendedDecimal.FromString("0.485456485657545752495450554857e+15").getExponent());
      Assert.assertEquals((BigInteger.valueOf(37)).negate(), ExtendedDecimal.FromString("0.485448525554495048e-19").getExponent());
      Assert.assertEquals((BigInteger.valueOf(29)).negate(), ExtendedDecimal.FromString("0.494952485550514953565655e-5").getExponent());
      Assert.assertEquals((BigInteger.valueOf(8)).negate(), ExtendedDecimal.FromString("0.50495454554854505051534950e+18").getExponent());
      Assert.assertEquals((BigInteger.valueOf(37)).negate(), ExtendedDecimal.FromString("0.5156524853575655535351554949525449e-3").getExponent());
      Assert.assertEquals(BigInteger.valueOf(3), ExtendedDecimal.FromString("0e+3").getExponent());
      Assert.assertEquals((BigInteger.valueOf(8)).negate(), ExtendedDecimal.FromString("0.51505056554957575255555250e+18").getExponent());
      Assert.assertEquals((BigInteger.valueOf(14)).negate(), ExtendedDecimal.FromString("0.5456e-10").getExponent());
      Assert.assertEquals((BigInteger.valueOf(36)).negate(), ExtendedDecimal.FromString("0.494850515656505252555154e-12").getExponent());
      Assert.assertEquals((BigInteger.valueOf(42)).negate(), ExtendedDecimal.FromString("0.535155525253485757525253555749575749e-6").getExponent());
      Assert.assertEquals((BigInteger.valueOf(29)).negate(), ExtendedDecimal.FromString("0.56554952554850525552515549564948e+3").getExponent());
      Assert.assertEquals((BigInteger.valueOf(40)).negate(), ExtendedDecimal.FromString("0.494855545257545656515554495057e-10").getExponent());
      Assert.assertEquals((BigInteger.valueOf(18)).negate(), ExtendedDecimal.FromString("0.5656504948515252555456e+4").getExponent());
      Assert.assertEquals((BigInteger.valueOf(17)).negate(), ExtendedDecimal.FromString("0e-17").getExponent());
      Assert.assertEquals((BigInteger.valueOf(32)).negate(), ExtendedDecimal.FromString("0.55535551515249535049495256e-6").getExponent());
      Assert.assertEquals((BigInteger.valueOf(31)).negate(), ExtendedDecimal.FromString("0.4948534853564853565654514855e-3").getExponent());
      Assert.assertEquals((BigInteger.valueOf(38)).negate(), ExtendedDecimal.FromString("0.5048485057535249555455e-16").getExponent());
      Assert.assertEquals((BigInteger.valueOf(16)).negate(), ExtendedDecimal.FromString("0e-16").getExponent());
      Assert.assertEquals(BigInteger.valueOf(5), ExtendedDecimal.FromString("0.5354e+9").getExponent());
      Assert.assertEquals(BigInteger.ONE, ExtendedDecimal.FromString("0.54e+3").getExponent());
      Assert.assertEquals((BigInteger.valueOf(38)).negate(), ExtendedDecimal.FromString("0.4849525755545751574853494948e-10").getExponent());
      Assert.assertEquals((BigInteger.valueOf(33)).negate(), ExtendedDecimal.FromString("0.52514853565252565251565548e-7").getExponent());
      Assert.assertEquals((BigInteger.valueOf(13)).negate(), ExtendedDecimal.FromString("0.575151545652e-1").getExponent());
      Assert.assertEquals((BigInteger.valueOf(22)).negate(), ExtendedDecimal.FromString("0.49515354514852e-8").getExponent());
      Assert.assertEquals((BigInteger.valueOf(24)).negate(), ExtendedDecimal.FromString("0.54535357515356545554e-4").getExponent());
      Assert.assertEquals((BigInteger.valueOf(11)).negate(), ExtendedDecimal.FromString("0.574848e-5").getExponent());
      Assert.assertEquals((BigInteger.valueOf(3)).negate(), ExtendedDecimal.FromString("0.565055e+3").getExponent());
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestReadUtf8() {
      this.DoTestReadUtf8(
        new byte[] {  0x21,
        0x21,
        0x21  },
        0,
        "!!!",
        0,
        "!!!");
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xc2,
        (byte)0x80  },
        0,
        " \u0080",
        0,
        " \u0080");
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xc2,
        (byte)0x80,
        0x20  },
        0,
        " \u0080 ",
        0,
        " \u0080 ");
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xc2,
        (byte)0x80,
        (byte)0xc2  },
        0,
        " \u0080\ufffd",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xc2,
        0x21,
        0x21  },
        0,
        " \ufffd!!",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xc2,
        (byte)0xff,
        0x20  },
        0,
        " \ufffd\ufffd ",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xe0,
        (byte)0xa0,
        (byte)0x80  },
        0,
        " \u0800",
        0,
        " \u0800");
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xe0,
        (byte)0xa0,
        (byte)0x80,
        0x20  },
        0,
        " \u0800 ",
        0,
        " \u0800 ");
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xf0,
        (byte)0x90,
        (byte)0x80,
        (byte)0x80  },
        0,
        " \ud800\udc00",
        0,
        " \ud800\udc00");
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xf0,
        (byte)0x90,
        (byte)0x80,
        (byte)0x80  },
        3,
        0,
        " \ufffd",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xf0,
        (byte)0x90  },
        5,
        -2,
        null,
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        0x20,
        0x20  },
        5,
        -2,
        null,
        -2,
        null);
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xf0,
        (byte)0x90,
        (byte)0x80,
        (byte)0x80,
        0x20  },
        0,
        " \ud800\udc00 ",
        0,
        " \ud800\udc00 ");
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xf0,
        (byte)0x90,
        (byte)0x80,
        0x20  },
        0,
        " \ufffd ",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xf0,
        (byte)0x90,
        0x20  },
        0,
        " \ufffd ",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xf0,
        (byte)0x90,
        (byte)0x80,
        (byte)0xff  },
        0,
        " \ufffd\ufffd",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xf0,
        (byte)0x90,
        (byte)0xff  },
        0,
        " \ufffd\ufffd",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xe0,
        (byte)0xa0,
        0x20  },
        0,
        " \ufffd ",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xe0,
        0x20  },
        0,
        " \ufffd ",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xe0,
        (byte)0xa0,
        (byte)0xff  },
        0,
        " \ufffd\ufffd",
        -1,
        null);
      this.DoTestReadUtf8(
        new byte[] {  0x20,
        (byte)0xe0,
        (byte)0xff  },
        0,
        " \ufffd\ufffd",
        -1,
        null);
    }

    private static boolean ByteArrayEquals(byte[] arrayA, byte[] arrayB) {
      if (arrayA == null) {
        return arrayB == null;
      }
      if (arrayB == null) {
        return false;
      }
      if (arrayA.length != arrayB.length) {
        return false;
      }
      for (int i = 0; i < arrayA.length; ++i) {
        if (arrayA[i] != arrayB[i]) {
          return false;
        }
      }
      return true;
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestArray() {
      CBORObject cbor = CBORObject.FromJSONString("[]");
      cbor.Add(CBORObject.FromObject(3));
      cbor.Add(CBORObject.FromObject(4));
      byte[] bytes = cbor.EncodeToBytes();
      boolean isequal = ByteArrayEquals(new byte[] {  (byte)(0x80 | 2), 3, 4  }, bytes);
      if(!(isequal))Assert.fail( "array not equal");
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestMap() {
      CBORObject cbor = CBORObject.FromJSONString("{\"a\":2,\"b\":4}");
      Assert.assertEquals(2, cbor.size());
      TestCommon.AssertEqualsHashCode(
        CBORObject.FromObject(2),
        cbor.get(CBORObject.FromObject("a")));
      TestCommon.AssertEqualsHashCode(
        CBORObject.FromObject(4),
        cbor.get(CBORObject.FromObject("b")));
      Assert.assertEquals(2, cbor.get(CBORObject.FromObject("a")).AsInt32());
      Assert.assertEquals(4, cbor.get(CBORObject.FromObject("b")).AsInt32());
    }

    private static String Repeat(char c, int num) {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < num; ++i) {
        sb.append(c);
      }
      return sb.toString();
    }

    private static String Repeat(String c, int num) {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < num; ++i) {
        sb.append(c);
      }
      return sb.toString();
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestTextStringStream() {
      CBORObject cbor = TestCommon.FromBytesTestAB(
        new byte[] {  0x7F,
        0x61,
        0x2e,
        0x61,
        0x2e,
        (byte)0xFF  });
      Assert.assertEquals("..", cbor.AsString());
      // Test streaming of long strings
      String longString = Repeat('x', 200000);
      CBORObject cbor2;
      cbor = CBORObject.FromObject(longString);
      cbor2 = TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
      TestCommon.AssertEqualsHashCode(cbor, cbor2);
      Assert.assertEquals(longString, cbor2.AsString());
      longString = Repeat('\u00e0', 200000);
      cbor = CBORObject.FromObject(longString);
      cbor2 = TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
      TestCommon.AssertEqualsHashCode(cbor, cbor2);
      Assert.assertEquals(longString, cbor2.AsString());
      longString = Repeat('\u3000', 200000);
      cbor = CBORObject.FromObject(longString);
      cbor2 = TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
      TestCommon.AssertEqualsHashCode(cbor, cbor2);
      Assert.assertEquals(longString, cbor2.AsString());
      longString = Repeat("\ud800\udc00", 200000);
      cbor = CBORObject.FromObject(longString);
      cbor2 = TestCommon.FromBytesTestAB(cbor.EncodeToBytes());
      TestCommon.AssertEqualsHashCode(cbor, cbor2);
      Assert.assertEquals(longString, cbor2.AsString());
    }
    @Test(expected=CBORException.class)
    public void TestTextStringStreamNoTagsBeforeDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] {  0x7F,
        0x61,
        0x20,
        (byte)0xC0,
        0x61,
        0x20,
        (byte)0xFF  });
    }
    @Test(expected=CBORException.class)
    public void TestTextStringStreamNoIndefiniteWithinDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] {  0x7F,
        0x61,
        0x20,
        0x7F,
        0x61,
        0x20,
        (byte)0xFF,
        (byte)0xFF  });
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestByteStringStream() {
      TestCommon.FromBytesTestAB(
        new byte[] {  0x5F,
        0x41,
        0x20,
        0x41,
        0x20,
        (byte)0xFF  });
    }
    @Test(expected=CBORException.class)
    public void TestByteStringStreamNoTagsBeforeDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] {  0x5F,
        0x41,
        0x20,
        (byte)0xC2,
        0x41,
        0x20,
        (byte)0xFF  });
    }

    public static void AssertDecimalsEquivalent(String a, String b) {
      CBORObject ca = CBORDataUtilities.ParseJSONNumber(a);
      CBORObject cb = CBORDataUtilities.ParseJSONNumber(b);
      CompareTestEqual(ca, cb);
      TestCommon.AssertRoundTrip(ca);
      TestCommon.AssertRoundTrip(cb);
    }

    /**
     * Not documented yet.
     */
    @Test
    public void ZeroStringTests2() {
      Assert.assertEquals("0.0001265", ExtendedDecimal.FromString("1.265e-4").toString());
      Assert.assertEquals("0.0001265", ExtendedDecimal.FromString("1.265e-4").ToEngineeringString());
      Assert.assertEquals("0.0001265", ExtendedDecimal.FromString("1.265e-4").ToPlainString());
      Assert.assertEquals("0.0000", ExtendedDecimal.FromString("0.000E-1").toString());
      Assert.assertEquals("0.0000", ExtendedDecimal.FromString("0.000E-1").ToEngineeringString());
      Assert.assertEquals("0.0000", ExtendedDecimal.FromString("0.000E-1").ToPlainString());
      Assert.assertEquals("0E-16", ExtendedDecimal.FromString("0.0000000000000e-3").toString());
      Assert.assertEquals("0.0E-15", ExtendedDecimal.FromString("0.0000000000000e-3").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000", ExtendedDecimal.FromString("0.0000000000000e-3").ToPlainString());
      Assert.assertEquals("0E-8", ExtendedDecimal.FromString("0.000000000e+1").toString());
      Assert.assertEquals("0.00E-6", ExtendedDecimal.FromString("0.000000000e+1").ToEngineeringString());
      Assert.assertEquals("0.00000000", ExtendedDecimal.FromString("0.000000000e+1").ToPlainString());
      Assert.assertEquals("0.000", ExtendedDecimal.FromString("0.000000000000000e+12").toString());
      Assert.assertEquals("0.000", ExtendedDecimal.FromString("0.000000000000000e+12").ToEngineeringString());
      Assert.assertEquals("0.000", ExtendedDecimal.FromString("0.000000000000000e+12").ToPlainString());
      Assert.assertEquals("0E-25", ExtendedDecimal.FromString("0.00000000000000e-11").toString());
      Assert.assertEquals("0.0E-24", ExtendedDecimal.FromString("0.00000000000000e-11").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000", ExtendedDecimal.FromString("0.00000000000000e-11").ToPlainString());
      Assert.assertEquals("0E-7", ExtendedDecimal.FromString("0.000000000000e+5").toString());
      Assert.assertEquals("0.0E-6", ExtendedDecimal.FromString("0.000000000000e+5").ToEngineeringString());
      Assert.assertEquals("0.0000000", ExtendedDecimal.FromString("0.000000000000e+5").ToPlainString());
      Assert.assertEquals("0E-8", ExtendedDecimal.FromString("0.0000e-4").toString());
      Assert.assertEquals("0.00E-6", ExtendedDecimal.FromString("0.0000e-4").ToEngineeringString());
      Assert.assertEquals("0.00000000", ExtendedDecimal.FromString("0.0000e-4").ToPlainString());
      Assert.assertEquals("0.0000", ExtendedDecimal.FromString("0.000000e+2").toString());
      Assert.assertEquals("0.0000", ExtendedDecimal.FromString("0.000000e+2").ToEngineeringString());
      Assert.assertEquals("0.0000", ExtendedDecimal.FromString("0.000000e+2").ToPlainString());
      Assert.assertEquals("0E+2", ExtendedDecimal.FromString("0.0e+3").toString());
      Assert.assertEquals("0.0E+3", ExtendedDecimal.FromString("0.0e+3").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.0e+3").ToPlainString());
      Assert.assertEquals("0E-7", ExtendedDecimal.FromString("0.000000000000000e+8").toString());
      Assert.assertEquals("0.0E-6", ExtendedDecimal.FromString("0.000000000000000e+8").ToEngineeringString());
      Assert.assertEquals("0.0000000", ExtendedDecimal.FromString("0.000000000000000e+8").ToPlainString());
      Assert.assertEquals("0E+7", ExtendedDecimal.FromString("0.000e+10").toString());
      Assert.assertEquals("0.00E+9", ExtendedDecimal.FromString("0.000e+10").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.000e+10").ToPlainString());
      Assert.assertEquals("0E-31", ExtendedDecimal.FromString("0.0000000000000000000e-12").toString());
      Assert.assertEquals("0.0E-30", ExtendedDecimal.FromString("0.0000000000000000000e-12").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000000000", ExtendedDecimal.FromString("0.0000000000000000000e-12").ToPlainString());
      Assert.assertEquals("0.00000", ExtendedDecimal.FromString("0.0000e-1").toString());
      Assert.assertEquals("0.00000", ExtendedDecimal.FromString("0.0000e-1").ToEngineeringString());
      Assert.assertEquals("0.00000", ExtendedDecimal.FromString("0.0000e-1").ToPlainString());
      Assert.assertEquals("0E-22", ExtendedDecimal.FromString("0.00000000000e-11").toString());
      Assert.assertEquals("0.0E-21", ExtendedDecimal.FromString("0.00000000000e-11").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000", ExtendedDecimal.FromString("0.00000000000e-11").ToPlainString());
      Assert.assertEquals("0E-28", ExtendedDecimal.FromString("0.00000000000e-17").toString());
      Assert.assertEquals("0.0E-27", ExtendedDecimal.FromString("0.00000000000e-17").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000000", ExtendedDecimal.FromString("0.00000000000e-17").ToPlainString());
      Assert.assertEquals("0.00000", ExtendedDecimal.FromString("0.00000000000000e+9").toString());
      Assert.assertEquals("0.00000", ExtendedDecimal.FromString("0.00000000000000e+9").ToEngineeringString());
      Assert.assertEquals("0.00000", ExtendedDecimal.FromString("0.00000000000000e+9").ToPlainString());
      Assert.assertEquals("0E-28", ExtendedDecimal.FromString("0.0000000000e-18").toString());
      Assert.assertEquals("0.0E-27", ExtendedDecimal.FromString("0.0000000000e-18").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000000", ExtendedDecimal.FromString("0.0000000000e-18").ToPlainString());
      Assert.assertEquals("0E-14", ExtendedDecimal.FromString("0.0e-13").toString());
      Assert.assertEquals("0.00E-12", ExtendedDecimal.FromString("0.0e-13").ToEngineeringString());
      Assert.assertEquals("0.00000000000000", ExtendedDecimal.FromString("0.0e-13").ToPlainString());
      Assert.assertEquals("0E-8", ExtendedDecimal.FromString("0.000000000000000000e+10").toString());
      Assert.assertEquals("0.00E-6", ExtendedDecimal.FromString("0.000000000000000000e+10").ToEngineeringString());
      Assert.assertEquals("0.00000000", ExtendedDecimal.FromString("0.000000000000000000e+10").ToPlainString());
      Assert.assertEquals("0E+15", ExtendedDecimal.FromString("0.0000e+19").toString());
      Assert.assertEquals("0E+15", ExtendedDecimal.FromString("0.0000e+19").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.0000e+19").ToPlainString());
      Assert.assertEquals("0E-13", ExtendedDecimal.FromString("0.00000e-8").toString());
      Assert.assertEquals("0.0E-12", ExtendedDecimal.FromString("0.00000e-8").ToEngineeringString());
      Assert.assertEquals("0.0000000000000", ExtendedDecimal.FromString("0.00000e-8").ToPlainString());
      Assert.assertEquals("0E+3", ExtendedDecimal.FromString("0.00000000000e+14").toString());
      Assert.assertEquals("0E+3", ExtendedDecimal.FromString("0.00000000000e+14").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.00000000000e+14").ToPlainString());
      Assert.assertEquals("0E-17", ExtendedDecimal.FromString("0.000e-14").toString());
      Assert.assertEquals("0.00E-15", ExtendedDecimal.FromString("0.000e-14").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000", ExtendedDecimal.FromString("0.000e-14").ToPlainString());
      Assert.assertEquals("0E-25", ExtendedDecimal.FromString("0.000000e-19").toString());
      Assert.assertEquals("0.0E-24", ExtendedDecimal.FromString("0.000000e-19").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000", ExtendedDecimal.FromString("0.000000e-19").ToPlainString());
      Assert.assertEquals("0E+7", ExtendedDecimal.FromString("0.000000000000e+19").toString());
      Assert.assertEquals("0.00E+9", ExtendedDecimal.FromString("0.000000000000e+19").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.000000000000e+19").ToPlainString());
      Assert.assertEquals("0E+5", ExtendedDecimal.FromString("0.0000000000000e+18").toString());
      Assert.assertEquals("0.0E+6", ExtendedDecimal.FromString("0.0000000000000e+18").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.0000000000000e+18").ToPlainString());
      Assert.assertEquals("0E-16", ExtendedDecimal.FromString("0.00000000000000e-2").toString());
      Assert.assertEquals("0.0E-15", ExtendedDecimal.FromString("0.00000000000000e-2").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000", ExtendedDecimal.FromString("0.00000000000000e-2").ToPlainString());
      Assert.assertEquals("0E-31", ExtendedDecimal.FromString("0.0000000000000e-18").toString());
      Assert.assertEquals("0.0E-30", ExtendedDecimal.FromString("0.0000000000000e-18").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000000000", ExtendedDecimal.FromString("0.0000000000000e-18").ToPlainString());
      Assert.assertEquals("0E-17", ExtendedDecimal.FromString("0e-17").toString());
      Assert.assertEquals("0.00E-15", ExtendedDecimal.FromString("0e-17").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000", ExtendedDecimal.FromString("0e-17").ToPlainString());
      Assert.assertEquals("0E+17", ExtendedDecimal.FromString("0e+17").toString());
      Assert.assertEquals("0.0E+18", ExtendedDecimal.FromString("0e+17").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0e+17").ToPlainString());
      Assert.assertEquals("0E-17", ExtendedDecimal.FromString("0.00000000000000000e+0").toString());
      Assert.assertEquals("0.00E-15", ExtendedDecimal.FromString("0.00000000000000000e+0").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000", ExtendedDecimal.FromString("0.00000000000000000e+0").ToPlainString());
      Assert.assertEquals("0E-13", ExtendedDecimal.FromString("0.0000000000000e+0").toString());
      Assert.assertEquals("0.0E-12", ExtendedDecimal.FromString("0.0000000000000e+0").ToEngineeringString());
      Assert.assertEquals("0.0000000000000", ExtendedDecimal.FromString("0.0000000000000e+0").ToPlainString());
      Assert.assertEquals("0E-31", ExtendedDecimal.FromString("0.0000000000000000000e-12").toString());
      Assert.assertEquals("0.0E-30", ExtendedDecimal.FromString("0.0000000000000000000e-12").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000000000", ExtendedDecimal.FromString("0.0000000000000000000e-12").ToPlainString());
      Assert.assertEquals("0E-9", ExtendedDecimal.FromString("0.0000000000000000000e+10").toString());
      Assert.assertEquals("0E-9", ExtendedDecimal.FromString("0.0000000000000000000e+10").ToEngineeringString());
      Assert.assertEquals("0.000000000", ExtendedDecimal.FromString("0.0000000000000000000e+10").ToPlainString());
      Assert.assertEquals("0E-7", ExtendedDecimal.FromString("0.00000e-2").toString());
      Assert.assertEquals("0.0E-6", ExtendedDecimal.FromString("0.00000e-2").ToEngineeringString());
      Assert.assertEquals("0.0000000", ExtendedDecimal.FromString("0.00000e-2").ToPlainString());
      Assert.assertEquals("0E+9", ExtendedDecimal.FromString("0.000000e+15").toString());
      Assert.assertEquals("0E+9", ExtendedDecimal.FromString("0.000000e+15").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.000000e+15").ToPlainString());
      Assert.assertEquals("0E-19", ExtendedDecimal.FromString("0.000000000e-10").toString());
      Assert.assertEquals("0.0E-18", ExtendedDecimal.FromString("0.000000000e-10").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000", ExtendedDecimal.FromString("0.000000000e-10").ToPlainString());
      Assert.assertEquals("0E-8", ExtendedDecimal.FromString("0.00000000000000e+6").toString());
      Assert.assertEquals("0.00E-6", ExtendedDecimal.FromString("0.00000000000000e+6").ToEngineeringString());
      Assert.assertEquals("0.00000000", ExtendedDecimal.FromString("0.00000000000000e+6").ToPlainString());
      Assert.assertEquals("0E+12", ExtendedDecimal.FromString("0.00000e+17").toString());
      Assert.assertEquals("0E+12", ExtendedDecimal.FromString("0.00000e+17").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.00000e+17").ToPlainString());
      Assert.assertEquals("0E-18", ExtendedDecimal.FromString("0.000000000000000000e-0").toString());
      Assert.assertEquals("0E-18", ExtendedDecimal.FromString("0.000000000000000000e-0").ToEngineeringString());
      Assert.assertEquals("0.000000000000000000", ExtendedDecimal.FromString("0.000000000000000000e-0").ToPlainString());
      Assert.assertEquals("0.00000", ExtendedDecimal.FromString("0.0000000000000000e+11").toString());
      Assert.assertEquals("0.00000", ExtendedDecimal.FromString("0.0000000000000000e+11").ToEngineeringString());
      Assert.assertEquals("0.00000", ExtendedDecimal.FromString("0.0000000000000000e+11").ToPlainString());
      Assert.assertEquals("0E+3", ExtendedDecimal.FromString("0.000000000000e+15").toString());
      Assert.assertEquals("0E+3", ExtendedDecimal.FromString("0.000000000000e+15").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.000000000000e+15").ToPlainString());
      Assert.assertEquals("0E-27", ExtendedDecimal.FromString("0.00000000e-19").toString());
      Assert.assertEquals("0E-27", ExtendedDecimal.FromString("0.00000000e-19").ToEngineeringString());
      Assert.assertEquals("0.000000000000000000000000000", ExtendedDecimal.FromString("0.00000000e-19").ToPlainString());
      Assert.assertEquals("0E-11", ExtendedDecimal.FromString("0.00000e-6").toString());
      Assert.assertEquals("0.00E-9", ExtendedDecimal.FromString("0.00000e-6").ToEngineeringString());
      Assert.assertEquals("0.00000000000", ExtendedDecimal.FromString("0.00000e-6").ToPlainString());
      Assert.assertEquals("0E-14", ExtendedDecimal.FromString("0e-14").toString());
      Assert.assertEquals("0.00E-12", ExtendedDecimal.FromString("0e-14").ToEngineeringString());
      Assert.assertEquals("0.00000000000000", ExtendedDecimal.FromString("0e-14").ToPlainString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.000000000e+9").toString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.000000000e+9").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.000000000e+9").ToPlainString());
      Assert.assertEquals("0E+8", ExtendedDecimal.FromString("0.00000e+13").toString());
      Assert.assertEquals("0.0E+9", ExtendedDecimal.FromString("0.00000e+13").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.00000e+13").ToPlainString());
      Assert.assertEquals("0.000", ExtendedDecimal.FromString("0.000e-0").toString());
      Assert.assertEquals("0.000", ExtendedDecimal.FromString("0.000e-0").ToEngineeringString());
      Assert.assertEquals("0.000", ExtendedDecimal.FromString("0.000e-0").ToPlainString());
      Assert.assertEquals("0E-9", ExtendedDecimal.FromString("0.000000000000000e+6").toString());
      Assert.assertEquals("0E-9", ExtendedDecimal.FromString("0.000000000000000e+6").ToEngineeringString());
      Assert.assertEquals("0.000000000", ExtendedDecimal.FromString("0.000000000000000e+6").ToPlainString());
      Assert.assertEquals("0E+8", ExtendedDecimal.FromString("0.000000000e+17").toString());
      Assert.assertEquals("0.0E+9", ExtendedDecimal.FromString("0.000000000e+17").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.000000000e+17").ToPlainString());
      Assert.assertEquals("0.00000", ExtendedDecimal.FromString("0.00000000000e+6").toString());
      Assert.assertEquals("0.00000", ExtendedDecimal.FromString("0.00000000000e+6").ToEngineeringString());
      Assert.assertEquals("0.00000", ExtendedDecimal.FromString("0.00000000000e+6").ToPlainString());
      Assert.assertEquals("0E-11", ExtendedDecimal.FromString("0.00000000000000e+3").toString());
      Assert.assertEquals("0.00E-9", ExtendedDecimal.FromString("0.00000000000000e+3").ToEngineeringString());
      Assert.assertEquals("0.00000000000", ExtendedDecimal.FromString("0.00000000000000e+3").ToPlainString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0e+0").toString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0e+0").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0e+0").ToPlainString());
      Assert.assertEquals("0E+9", ExtendedDecimal.FromString("0.000e+12").toString());
      Assert.assertEquals("0E+9", ExtendedDecimal.FromString("0.000e+12").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.000e+12").ToPlainString());
      Assert.assertEquals("0.00", ExtendedDecimal.FromString("0.00000000000e+9").toString());
      Assert.assertEquals("0.00", ExtendedDecimal.FromString("0.00000000000e+9").ToEngineeringString());
      Assert.assertEquals("0.00", ExtendedDecimal.FromString("0.00000000000e+9").ToPlainString());
      Assert.assertEquals("0E-23", ExtendedDecimal.FromString("0.00000000000000e-9").toString());
      Assert.assertEquals("0.00E-21", ExtendedDecimal.FromString("0.00000000000000e-9").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000000", ExtendedDecimal.FromString("0.00000000000000e-9").ToPlainString());
      Assert.assertEquals("0.0", ExtendedDecimal.FromString("0e-1").toString());
      Assert.assertEquals("0.0", ExtendedDecimal.FromString("0e-1").ToEngineeringString());
      Assert.assertEquals("0.0", ExtendedDecimal.FromString("0e-1").ToPlainString());
      Assert.assertEquals("0E-17", ExtendedDecimal.FromString("0.0000e-13").toString());
      Assert.assertEquals("0.00E-15", ExtendedDecimal.FromString("0.0000e-13").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000", ExtendedDecimal.FromString("0.0000e-13").ToPlainString());
      Assert.assertEquals("0E-18", ExtendedDecimal.FromString("0.00000000000e-7").toString());
      Assert.assertEquals("0E-18", ExtendedDecimal.FromString("0.00000000000e-7").ToEngineeringString());
      Assert.assertEquals("0.000000000000000000", ExtendedDecimal.FromString("0.00000000000e-7").ToPlainString());
      Assert.assertEquals("0E-10", ExtendedDecimal.FromString("0.00000000000000e+4").toString());
      Assert.assertEquals("0.0E-9", ExtendedDecimal.FromString("0.00000000000000e+4").ToEngineeringString());
      Assert.assertEquals("0.0000000000", ExtendedDecimal.FromString("0.00000000000000e+4").ToPlainString());
      Assert.assertEquals("0E-16", ExtendedDecimal.FromString("0.00000000e-8").toString());
      Assert.assertEquals("0.0E-15", ExtendedDecimal.FromString("0.00000000e-8").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000", ExtendedDecimal.FromString("0.00000000e-8").ToPlainString());
      Assert.assertEquals("0E-8", ExtendedDecimal.FromString("0.00e-6").toString());
      Assert.assertEquals("0.00E-6", ExtendedDecimal.FromString("0.00e-6").ToEngineeringString());
      Assert.assertEquals("0.00000000", ExtendedDecimal.FromString("0.00e-6").ToPlainString());
      Assert.assertEquals("0.00", ExtendedDecimal.FromString("0.0e-1").toString());
      Assert.assertEquals("0.00", ExtendedDecimal.FromString("0.0e-1").ToEngineeringString());
      Assert.assertEquals("0.00", ExtendedDecimal.FromString("0.0e-1").ToPlainString());
      Assert.assertEquals("0E-26", ExtendedDecimal.FromString("0.0000000000000000e-10").toString());
      Assert.assertEquals("0.00E-24", ExtendedDecimal.FromString("0.0000000000000000e-10").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000000000", ExtendedDecimal.FromString("0.0000000000000000e-10").ToPlainString());
      Assert.assertEquals("0E+12", ExtendedDecimal.FromString("0.00e+14").toString());
      Assert.assertEquals("0E+12", ExtendedDecimal.FromString("0.00e+14").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.00e+14").ToPlainString());
      Assert.assertEquals("0E-13", ExtendedDecimal.FromString("0.000000000000000000e+5").toString());
      Assert.assertEquals("0.0E-12", ExtendedDecimal.FromString("0.000000000000000000e+5").ToEngineeringString());
      Assert.assertEquals("0.0000000000000", ExtendedDecimal.FromString("0.000000000000000000e+5").ToPlainString());
      Assert.assertEquals("0E+6", ExtendedDecimal.FromString("0.0e+7").toString());
      Assert.assertEquals("0E+6", ExtendedDecimal.FromString("0.0e+7").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.0e+7").ToPlainString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.00000000e+8").toString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.00000000e+8").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.00000000e+8").ToPlainString());
      Assert.assertEquals("0E-9", ExtendedDecimal.FromString("0.000000000e+0").toString());
      Assert.assertEquals("0E-9", ExtendedDecimal.FromString("0.000000000e+0").ToEngineeringString());
      Assert.assertEquals("0.000000000", ExtendedDecimal.FromString("0.000000000e+0").ToPlainString());
      Assert.assertEquals("0E+10", ExtendedDecimal.FromString("0.000e+13").toString());
      Assert.assertEquals("0.00E+12", ExtendedDecimal.FromString("0.000e+13").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.000e+13").ToPlainString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.0000000000000000e+16").toString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.0000000000000000e+16").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.0000000000000000e+16").ToPlainString());
      Assert.assertEquals("0E-9", ExtendedDecimal.FromString("0.00000000e-1").toString());
      Assert.assertEquals("0E-9", ExtendedDecimal.FromString("0.00000000e-1").ToEngineeringString());
      Assert.assertEquals("0.000000000", ExtendedDecimal.FromString("0.00000000e-1").ToPlainString());
      Assert.assertEquals("0E-26", ExtendedDecimal.FromString("0.00000000000e-15").toString());
      Assert.assertEquals("0.00E-24", ExtendedDecimal.FromString("0.00000000000e-15").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000000000", ExtendedDecimal.FromString("0.00000000000e-15").ToPlainString());
      Assert.assertEquals("0E+10", ExtendedDecimal.FromString("0.0e+11").toString());
      Assert.assertEquals("0.00E+12", ExtendedDecimal.FromString("0.0e+11").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.0e+11").ToPlainString());
      Assert.assertEquals("0E+2", ExtendedDecimal.FromString("0.00000e+7").toString());
      Assert.assertEquals("0.0E+3", ExtendedDecimal.FromString("0.00000e+7").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.00000e+7").ToPlainString());
      Assert.assertEquals("0E-38", ExtendedDecimal.FromString("0.0000000000000000000e-19").toString());
      Assert.assertEquals("0.00E-36", ExtendedDecimal.FromString("0.0000000000000000000e-19").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000000000000000000000", ExtendedDecimal.FromString("0.0000000000000000000e-19").ToPlainString());
      Assert.assertEquals("0E-16", ExtendedDecimal.FromString("0.0000000000e-6").toString());
      Assert.assertEquals("0.0E-15", ExtendedDecimal.FromString("0.0000000000e-6").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000", ExtendedDecimal.FromString("0.0000000000e-6").ToPlainString());
      Assert.assertEquals("0E-32", ExtendedDecimal.FromString("0.00000000000000000e-15").toString());
      Assert.assertEquals("0.00E-30", ExtendedDecimal.FromString("0.00000000000000000e-15").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000000000000000", ExtendedDecimal.FromString("0.00000000000000000e-15").ToPlainString());
      Assert.assertEquals("0E-13", ExtendedDecimal.FromString("0.000000000000000e+2").toString());
      Assert.assertEquals("0.0E-12", ExtendedDecimal.FromString("0.000000000000000e+2").ToEngineeringString());
      Assert.assertEquals("0.0000000000000", ExtendedDecimal.FromString("0.000000000000000e+2").ToPlainString());
      Assert.assertEquals("0E-19", ExtendedDecimal.FromString("0.0e-18").toString());
      Assert.assertEquals("0.0E-18", ExtendedDecimal.FromString("0.0e-18").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000", ExtendedDecimal.FromString("0.0e-18").ToPlainString());
      Assert.assertEquals("0E-20", ExtendedDecimal.FromString("0.00000000000000e-6").toString());
      Assert.assertEquals("0.00E-18", ExtendedDecimal.FromString("0.00000000000000e-6").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000", ExtendedDecimal.FromString("0.00000000000000e-6").ToPlainString());
      Assert.assertEquals("0E-20", ExtendedDecimal.FromString("0.000e-17").toString());
      Assert.assertEquals("0.00E-18", ExtendedDecimal.FromString("0.000e-17").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000", ExtendedDecimal.FromString("0.000e-17").ToPlainString());
      Assert.assertEquals("0E-21", ExtendedDecimal.FromString("0.00000000000000e-7").toString());
      Assert.assertEquals("0E-21", ExtendedDecimal.FromString("0.00000000000000e-7").ToEngineeringString());
      Assert.assertEquals("0.000000000000000000000", ExtendedDecimal.FromString("0.00000000000000e-7").ToPlainString());
      Assert.assertEquals("0E-15", ExtendedDecimal.FromString("0.000000e-9").toString());
      Assert.assertEquals("0E-15", ExtendedDecimal.FromString("0.000000e-9").ToEngineeringString());
      Assert.assertEquals("0.000000000000000", ExtendedDecimal.FromString("0.000000e-9").ToPlainString());
      Assert.assertEquals("0E-11", ExtendedDecimal.FromString("0e-11").toString());
      Assert.assertEquals("0.00E-9", ExtendedDecimal.FromString("0e-11").ToEngineeringString());
      Assert.assertEquals("0.00000000000", ExtendedDecimal.FromString("0e-11").ToPlainString());
      Assert.assertEquals("0E+2", ExtendedDecimal.FromString("0.000000000e+11").toString());
      Assert.assertEquals("0.0E+3", ExtendedDecimal.FromString("0.000000000e+11").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.000000000e+11").ToPlainString());
      Assert.assertEquals("0.0", ExtendedDecimal.FromString("0.0000000000000000e+15").toString());
      Assert.assertEquals("0.0", ExtendedDecimal.FromString("0.0000000000000000e+15").ToEngineeringString());
      Assert.assertEquals("0.0", ExtendedDecimal.FromString("0.0000000000000000e+15").ToPlainString());
      Assert.assertEquals("0.000000", ExtendedDecimal.FromString("0.0000000000000000e+10").toString());
      Assert.assertEquals("0.000000", ExtendedDecimal.FromString("0.0000000000000000e+10").ToEngineeringString());
      Assert.assertEquals("0.000000", ExtendedDecimal.FromString("0.0000000000000000e+10").ToPlainString());
      Assert.assertEquals("0.00000", ExtendedDecimal.FromString("0.000000000e+4").toString());
      Assert.assertEquals("0.00000", ExtendedDecimal.FromString("0.000000000e+4").ToEngineeringString());
      Assert.assertEquals("0.00000", ExtendedDecimal.FromString("0.000000000e+4").ToPlainString());
      Assert.assertEquals("0E-28", ExtendedDecimal.FromString("0.000000000000000e-13").toString());
      Assert.assertEquals("0.0E-27", ExtendedDecimal.FromString("0.000000000000000e-13").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000000", ExtendedDecimal.FromString("0.000000000000000e-13").ToPlainString());
      Assert.assertEquals("0E-27", ExtendedDecimal.FromString("0.0000000000000000000e-8").toString());
      Assert.assertEquals("0E-27", ExtendedDecimal.FromString("0.0000000000000000000e-8").ToEngineeringString());
      Assert.assertEquals("0.000000000000000000000000000", ExtendedDecimal.FromString("0.0000000000000000000e-8").ToPlainString());
      Assert.assertEquals("0E-26", ExtendedDecimal.FromString("0.00000000000e-15").toString());
      Assert.assertEquals("0.00E-24", ExtendedDecimal.FromString("0.00000000000e-15").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000000000", ExtendedDecimal.FromString("0.00000000000e-15").ToPlainString());
      Assert.assertEquals("0E+10", ExtendedDecimal.FromString("0.00e+12").toString());
      Assert.assertEquals("0.00E+12", ExtendedDecimal.FromString("0.00e+12").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.00e+12").ToPlainString());
      Assert.assertEquals("0E+4", ExtendedDecimal.FromString("0.0e+5").toString());
      Assert.assertEquals("0.00E+6", ExtendedDecimal.FromString("0.0e+5").ToEngineeringString());
      Assert.assertEquals("0", ExtendedDecimal.FromString("0.0e+5").ToPlainString());
      Assert.assertEquals("0E-9", ExtendedDecimal.FromString("0.0000000000000000e+7").toString());
      Assert.assertEquals("0E-9", ExtendedDecimal.FromString("0.0000000000000000e+7").ToEngineeringString());
      Assert.assertEquals("0.000000000", ExtendedDecimal.FromString("0.0000000000000000e+7").ToPlainString());
      Assert.assertEquals("0E-16", ExtendedDecimal.FromString("0.0000000000000000e-0").toString());
      Assert.assertEquals("0.0E-15", ExtendedDecimal.FromString("0.0000000000000000e-0").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000", ExtendedDecimal.FromString("0.0000000000000000e-0").ToPlainString());
      Assert.assertEquals("0.00", ExtendedDecimal.FromString("0.000000000000000e+13").toString());
      Assert.assertEquals("0.00", ExtendedDecimal.FromString("0.000000000000000e+13").ToEngineeringString());
      Assert.assertEquals("0.00", ExtendedDecimal.FromString("0.000000000000000e+13").ToPlainString());
      Assert.assertEquals("0E-24", ExtendedDecimal.FromString("0.00000000000e-13").toString());
      Assert.assertEquals("0E-24", ExtendedDecimal.FromString("0.00000000000e-13").ToEngineeringString());
      Assert.assertEquals("0.000000000000000000000000", ExtendedDecimal.FromString("0.00000000000e-13").ToPlainString());
      Assert.assertEquals("0E-13", ExtendedDecimal.FromString("0.000e-10").toString());
      Assert.assertEquals("0.0E-12", ExtendedDecimal.FromString("0.000e-10").ToEngineeringString());
      Assert.assertEquals("0.0000000000000", ExtendedDecimal.FromString("0.000e-10").ToPlainString());
    }
    // Tests whether AsInt32/64/16/AsByte properly truncate floats
    // and doubles before bounds checking
    @Test
    public void FloatingPointCloseToEdge() {
      try {
        CBORObject.FromObject(2.147483647E9d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.147483647E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.147483647E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.147483647E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836470000002E9d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836470000002E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836470000002E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836470000002E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836469999998E9d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836469999998E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836469999998E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836469999998E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.147483648E9d).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.147483648E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.147483648E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.147483648E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836480000005E9d).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836480000005E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836480000005E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836480000005E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836479999998E9d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836479999998E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836479999998E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836479999998E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.147483646E9d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.147483646E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.147483646E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.147483646E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836460000002E9d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836460000002E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836460000002E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836460000002E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836459999998E9d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836459999998E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836459999998E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474836459999998E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.147483648E9d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.147483648E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.147483648E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.147483648E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836479999998E9d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836479999998E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836479999998E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836479999998E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836480000005E9d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836480000005E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836480000005E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836480000005E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.147483647E9d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.147483647E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.147483647E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.147483647E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836469999998E9d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836469999998E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836469999998E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836469999998E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836470000002E9d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836470000002E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836470000002E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836470000002E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.147483649E9d).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.147483649E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.147483649E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.147483649E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836489999995E9d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836489999995E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836489999995E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836489999995E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836490000005E9d).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836490000005E9d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836490000005E9d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474836490000005E9d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223372036854776E18d).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223372036854776E18d).AsInt64();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223372036854776E18d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223372036854776E18d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223372036854778E18d).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223372036854778E18d).AsInt64();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223372036854778E18d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223372036854778E18d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.2233720368547748E18d).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.2233720368547748E18d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.2233720368547748E18d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.2233720368547748E18d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854776E18d).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854776E18d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854776E18d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854776E18d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.2233720368547748E18d).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.2233720368547748E18d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.2233720368547748E18d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.2233720368547748E18d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854778E18d).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854778E18d).AsInt64();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854778E18d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223372036854778E18d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.0d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.0d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.0d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.0d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.000000000004d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.000000000004d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.000000000004d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.000000000004d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.999999999996d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.999999999996d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.999999999996d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.999999999996d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.0d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.0d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.0d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.0d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.00000000001d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.00000000001d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.00000000001d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.00000000001d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.999999999996d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.999999999996d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.999999999996d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.999999999996d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.0d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.0d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.0d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.0d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.000000000004d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.000000000004d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.000000000004d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.000000000004d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32765.999999999996d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32765.999999999996d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32765.999999999996d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32765.999999999996d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.0d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.0d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.0d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.0d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.999999999996d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.999999999996d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.999999999996d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.999999999996d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.00000000001d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.00000000001d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.00000000001d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.00000000001d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.0d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.0d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.0d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.0d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32766.999999999996d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32766.999999999996d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32766.999999999996d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32766.999999999996d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.000000000004d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.000000000004d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.000000000004d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.000000000004d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.0d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.0d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.0d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.0d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.99999999999d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.99999999999d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.99999999999d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.99999999999d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.00000000001d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.00000000001d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.00000000001d).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.00000000001d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.0d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.0d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.0d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.0d).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(4.9E-324d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(4.9E-324d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(4.9E-324d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(4.9E-324d).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-4.9E-324d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-4.9E-324d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-4.9E-324d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-4.9E-324d).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0d).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0000000000000002d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0000000000000002d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0000000000000002d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0000000000000002d).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.9999999999999999d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.9999999999999999d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.9999999999999999d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.9999999999999999d).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-0.9999999999999999d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-0.9999999999999999d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-0.9999999999999999d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-0.9999999999999999d).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0000000000000002d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0000000000000002d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0000000000000002d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0000000000000002d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.0d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.0d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.0d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.0d).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.00000000000003d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.00000000000003d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.00000000000003d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.00000000000003d).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.99999999999997d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.99999999999997d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.99999999999997d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.99999999999997d).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.0d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.0d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.0d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.0d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.00000000000006d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.00000000000006d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.00000000000006d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.00000000000006d).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.99999999999997d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.99999999999997d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.99999999999997d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.99999999999997d).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.0d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.0d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.0d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.0d).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.00000000000003d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.00000000000003d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.00000000000003d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.00000000000003d).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(253.99999999999997d).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(253.99999999999997d).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(253.99999999999997d).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(253.99999999999997d).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.14748365E9f).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.14748365E9f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.14748365E9f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.14748365E9f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474839E9f).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474839E9f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474839E9f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.1474839E9f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.14748352E9f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.14748352E9f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.14748352E9f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(2.14748352E9f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.14748365E9f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.14748365E9f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.14748365E9f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.14748365E9f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.14748352E9f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.14748352E9f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.14748352E9f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.14748352E9f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474839E9f).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474839E9f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474839E9f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-2.1474839E9f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223372E18f).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223372E18f).AsInt64();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223372E18f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223372E18f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223373E18f).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223373E18f).AsInt64();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223373E18f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.223373E18f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.2233715E18f).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.2233715E18f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.2233715E18f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(9.2233715E18f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223372E18f).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223372E18f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223372E18f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223372E18f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.2233715E18f).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.2233715E18f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.2233715E18f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.2233715E18f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223373E18f).AsInt32();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223373E18f).AsInt64();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223373E18f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-9.223373E18f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.0f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.0f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.0f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.0f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.002f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.002f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.002f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.002f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.998f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.998f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.998f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.998f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.0f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.0f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.0f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.0f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.004f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.004f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.004f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32768.004f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.998f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.998f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.998f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32767.998f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.0f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.0f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.0f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.0f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.002f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.002f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.002f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32766.002f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32765.998f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32765.998f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32765.998f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(32765.998f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.0f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.0f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.0f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.0f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.998f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.998f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.998f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.998f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.004f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.004f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.004f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.004f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.0f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.0f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.0f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.0f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32766.998f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32766.998f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32766.998f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32766.998f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.002f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.002f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.002f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32767.002f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.0f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.0f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.0f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.0f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.996f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.996f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.996f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32768.996f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.004f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.004f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.004f).AsInt16();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-32769.004f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.0f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.0f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.0f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.0f).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.4E-45f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.4E-45f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.4E-45f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.4E-45f).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.4E-45f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.4E-45f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.4E-45f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.4E-45f).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0f).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0000001f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0000001f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0000001f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(1.0000001f).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.99999994f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.99999994f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.99999994f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(0.99999994f).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-0.99999994f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-0.99999994f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-0.99999994f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-0.99999994f).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0000001f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0000001f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0000001f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(-1.0000001f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.0f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.0f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.0f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.0f).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.00002f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.00002f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.00002f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.00002f).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.99998f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.99998f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.99998f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.99998f).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.0f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.0f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.0f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.0f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.00003f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.00003f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.00003f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(256.00003f).AsByte();
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.99998f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.99998f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.99998f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(255.99998f).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.0f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.0f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.0f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.0f).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.00002f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.00002f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.00002f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(254.00002f).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(253.99998f).AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(253.99998f).AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(253.99998f).AsInt16();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.FromObject(253.99998f).AsByte();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void FromDoubleTest() {
      Assert.assertEquals("0.213299999999999989608312489508534781634807586669921875", ExtendedDecimal.FromDouble(0.2133).toString());
      Assert.assertEquals("2.29360000000000010330982488752915582352898127282969653606414794921875E-7", ExtendedDecimal.FromDouble(2.2936E-7).toString());
      Assert.assertEquals("3893200000", ExtendedDecimal.FromDouble(3.8932E9).toString());
      Assert.assertEquals("128230", ExtendedDecimal.FromDouble(128230.0).toString());
      Assert.assertEquals("127210", ExtendedDecimal.FromDouble(127210.0).toString());
      Assert.assertEquals("0.267230000000000023074875343809253536164760589599609375", ExtendedDecimal.FromDouble(0.26723).toString());
      Assert.assertEquals("0.302329999999999987636556397774256765842437744140625", ExtendedDecimal.FromDouble(0.30233).toString());
      Assert.assertEquals("0.0000019512000000000000548530838806460252499164198525249958038330078125", ExtendedDecimal.FromDouble(1.9512E-6).toString());
      Assert.assertEquals("199500", ExtendedDecimal.FromDouble(199500.0).toString());
      Assert.assertEquals("36214000", ExtendedDecimal.FromDouble(3.6214E7).toString());
      Assert.assertEquals("1913300000000", ExtendedDecimal.FromDouble(1.9133E12).toString());
      Assert.assertEquals("0.0002173499999999999976289799530349000633577816188335418701171875", ExtendedDecimal.FromDouble(2.1735E-4).toString());
      Assert.assertEquals("0.0000310349999999999967797807698399736864303122274577617645263671875", ExtendedDecimal.FromDouble(3.1035E-5).toString());
      Assert.assertEquals("1.274999999999999911182158029987476766109466552734375", ExtendedDecimal.FromDouble(1.275).toString());
      Assert.assertEquals("214190", ExtendedDecimal.FromDouble(214190.0).toString());
      Assert.assertEquals("3981300000", ExtendedDecimal.FromDouble(3.9813E9).toString());
      Assert.assertEquals("1092700", ExtendedDecimal.FromDouble(1092700.0).toString());
      Assert.assertEquals("0.023609999999999999042987752773115062154829502105712890625", ExtendedDecimal.FromDouble(0.02361).toString());
      Assert.assertEquals("12.321999999999999175770426518283784389495849609375", ExtendedDecimal.FromDouble(12.322).toString());
      Assert.assertEquals("0.002586999999999999889921387108415729016996920108795166015625", ExtendedDecimal.FromDouble(0.002587).toString());
      Assert.assertEquals("1322000000", ExtendedDecimal.FromDouble(1.322E9).toString());
      Assert.assertEquals("95310000000", ExtendedDecimal.FromDouble(9.531E10).toString());
      Assert.assertEquals("142.3799999999999954525264911353588104248046875", ExtendedDecimal.FromDouble(142.38).toString());
      Assert.assertEquals("2252.5", ExtendedDecimal.FromDouble(2252.5).toString());
      Assert.assertEquals("363600000000", ExtendedDecimal.FromDouble(3.636E11).toString());
      Assert.assertEquals("0.00000323700000000000009386523676380154057596882921643555164337158203125", ExtendedDecimal.FromDouble(3.237E-6).toString());
      Assert.assertEquals("728000", ExtendedDecimal.FromDouble(728000.0).toString());
      Assert.assertEquals("25818000", ExtendedDecimal.FromDouble(2.5818E7).toString());
      Assert.assertEquals("1090000", ExtendedDecimal.FromDouble(1090000.0).toString());
      Assert.assertEquals("1.5509999999999999342747969421907328069210052490234375", ExtendedDecimal.FromDouble(1.551).toString());
      Assert.assertEquals("26.035000000000000142108547152020037174224853515625", ExtendedDecimal.FromDouble(26.035).toString());
      Assert.assertEquals("833000000", ExtendedDecimal.FromDouble(8.33E8).toString());
      Assert.assertEquals("812300000000", ExtendedDecimal.FromDouble(8.123E11).toString());
      Assert.assertEquals("2622.90000000000009094947017729282379150390625", ExtendedDecimal.FromDouble(2622.9).toString());
      Assert.assertEquals("1.290999999999999925393012745189480483531951904296875", ExtendedDecimal.FromDouble(1.291).toString());
      Assert.assertEquals("286140", ExtendedDecimal.FromDouble(286140.0).toString());
      Assert.assertEquals("0.06733000000000000095923269327613525092601776123046875", ExtendedDecimal.FromDouble(0.06733).toString());
      Assert.assertEquals("0.000325160000000000010654532811571471029310487210750579833984375", ExtendedDecimal.FromDouble(3.2516E-4).toString());
      Assert.assertEquals("383230000", ExtendedDecimal.FromDouble(3.8323E8).toString());
      Assert.assertEquals("0.02843299999999999994049204588009160943329334259033203125", ExtendedDecimal.FromDouble(0.028433).toString());
      Assert.assertEquals("837000000", ExtendedDecimal.FromDouble(8.37E8).toString());
      Assert.assertEquals("0.0160800000000000005428990590417015482671558856964111328125", ExtendedDecimal.FromDouble(0.01608).toString());
      Assert.assertEquals("3621000000000", ExtendedDecimal.FromDouble(3.621E12).toString());
      Assert.assertEquals("78.1200000000000045474735088646411895751953125", ExtendedDecimal.FromDouble(78.12).toString());
      Assert.assertEquals("1308000000", ExtendedDecimal.FromDouble(1.308E9).toString());
      Assert.assertEquals("0.031937000000000000110578213252665591426193714141845703125", ExtendedDecimal.FromDouble(0.031937).toString());
      Assert.assertEquals("1581500", ExtendedDecimal.FromDouble(1581500.0).toString());
      Assert.assertEquals("244200", ExtendedDecimal.FromDouble(244200.0).toString());
      Assert.assertEquals("2.28179999999999995794237200343046456652018605382181704044342041015625E-7", ExtendedDecimal.FromDouble(2.2818E-7).toString());
      Assert.assertEquals("39.73400000000000176214598468504846096038818359375", ExtendedDecimal.FromDouble(39.734).toString());
      Assert.assertEquals("1614", ExtendedDecimal.FromDouble(1614.0).toString());
      Assert.assertEquals("0.0003831899999999999954607143859419693399104289710521697998046875", ExtendedDecimal.FromDouble(3.8319E-4).toString());
      Assert.assertEquals("543.3999999999999772626324556767940521240234375", ExtendedDecimal.FromDouble(543.4).toString());
      Assert.assertEquals("319310000", ExtendedDecimal.FromDouble(3.1931E8).toString());
      Assert.assertEquals("1429000", ExtendedDecimal.FromDouble(1429000.0).toString());
      Assert.assertEquals("2653700000000", ExtendedDecimal.FromDouble(2.6537E12).toString());
      Assert.assertEquals("722000000", ExtendedDecimal.FromDouble(7.22E8).toString());
      Assert.assertEquals("27.199999999999999289457264239899814128875732421875", ExtendedDecimal.FromDouble(27.2).toString());
      Assert.assertEquals("0.00000380250000000000001586513038998038638283105683512985706329345703125", ExtendedDecimal.FromDouble(3.8025E-6).toString());
      Assert.assertEquals("0.0000364159999999999982843446044711299691698513925075531005859375", ExtendedDecimal.FromDouble(3.6416E-5).toString());
      Assert.assertEquals("2006000", ExtendedDecimal.FromDouble(2006000.0).toString());
      Assert.assertEquals("2681200000", ExtendedDecimal.FromDouble(2.6812E9).toString());
      Assert.assertEquals("27534000000", ExtendedDecimal.FromDouble(2.7534E10).toString());
      Assert.assertEquals("3.911600000000000165617541382501176627783934236504137516021728515625E-7", ExtendedDecimal.FromDouble(3.9116E-7).toString());
      Assert.assertEquals("0.0028135000000000000286437540353290387429296970367431640625", ExtendedDecimal.FromDouble(0.0028135).toString());
      Assert.assertEquals("0.91190000000000004387601393318618647754192352294921875", ExtendedDecimal.FromDouble(0.9119).toString());
      Assert.assertEquals("2241200", ExtendedDecimal.FromDouble(2241200.0).toString());
      Assert.assertEquals("32.4500000000000028421709430404007434844970703125", ExtendedDecimal.FromDouble(32.45).toString());
      Assert.assertEquals("13800000000", ExtendedDecimal.FromDouble(1.38E10).toString());
      Assert.assertEquals("0.047300000000000001765254609153998899273574352264404296875", ExtendedDecimal.FromDouble(0.0473).toString());
      Assert.assertEquals("205.340000000000003410605131648480892181396484375", ExtendedDecimal.FromDouble(205.34).toString());
      Assert.assertEquals("3.981899999999999995026200849679298698902130126953125", ExtendedDecimal.FromDouble(3.9819).toString());
      Assert.assertEquals("1152.799999999999954525264911353588104248046875", ExtendedDecimal.FromDouble(1152.8).toString());
      Assert.assertEquals("1322000", ExtendedDecimal.FromDouble(1322000.0).toString());
      Assert.assertEquals("0.00013414000000000001334814203612921801322954706847667694091796875", ExtendedDecimal.FromDouble(1.3414E-4).toString());
      Assert.assertEquals("3.4449999999999999446924077266263264363033158588223159313201904296875E-7", ExtendedDecimal.FromDouble(3.445E-7).toString());
      Assert.assertEquals("1.3610000000000000771138253079228785935583800892345607280731201171875E-7", ExtendedDecimal.FromDouble(1.361E-7).toString());
      Assert.assertEquals("26090000", ExtendedDecimal.FromDouble(2.609E7).toString());
      Assert.assertEquals("9.93599999999999994315658113919198513031005859375", ExtendedDecimal.FromDouble(9.936).toString());
      Assert.assertEquals("0.00000600000000000000015200514458246772164784488268196582794189453125", ExtendedDecimal.FromDouble(6.0E-6).toString());
      Assert.assertEquals("260.31000000000000227373675443232059478759765625", ExtendedDecimal.FromDouble(260.31).toString());
      Assert.assertEquals("344.6000000000000227373675443232059478759765625", ExtendedDecimal.FromDouble(344.6).toString());
      Assert.assertEquals("3.423700000000000187583282240666449069976806640625", ExtendedDecimal.FromDouble(3.4237).toString());
      Assert.assertEquals("2342100000", ExtendedDecimal.FromDouble(2.3421E9).toString());
      Assert.assertEquals("0.00023310000000000000099260877295392901942250318825244903564453125", ExtendedDecimal.FromDouble(2.331E-4).toString());
      Assert.assertEquals("0.7339999999999999857891452847979962825775146484375", ExtendedDecimal.FromDouble(0.734).toString());
      Assert.assertEquals("0.01541499999999999988287147090204598498530685901641845703125", ExtendedDecimal.FromDouble(0.015415).toString());
      Assert.assertEquals("0.0035311000000000001240729741169843691750429570674896240234375", ExtendedDecimal.FromDouble(0.0035311).toString());
      Assert.assertEquals("1221700000000", ExtendedDecimal.FromDouble(1.2217E12).toString());
      Assert.assertEquals("0.48299999999999998490096686509787105023860931396484375", ExtendedDecimal.FromDouble(0.483).toString());
      Assert.assertEquals("0.0002871999999999999878506906636488338335766457021236419677734375", ExtendedDecimal.FromDouble(2.872E-4).toString());
      Assert.assertEquals("96.1099999999999994315658113919198513031005859375", ExtendedDecimal.FromDouble(96.11).toString());
      Assert.assertEquals("36570", ExtendedDecimal.FromDouble(36570.0).toString());
      Assert.assertEquals("0.00001830000000000000097183545932910675446692039258778095245361328125", ExtendedDecimal.FromDouble(1.83E-5).toString());
      Assert.assertEquals("301310000", ExtendedDecimal.FromDouble(3.0131E8).toString());
      Assert.assertEquals("382200", ExtendedDecimal.FromDouble(382200.0).toString());
      Assert.assertEquals("248350000", ExtendedDecimal.FromDouble(2.4835E8).toString());
      Assert.assertEquals("0.0015839999999999999046040866090834242640994489192962646484375", ExtendedDecimal.FromDouble(0.001584).toString());
      Assert.assertEquals("0.000761999999999999982035203682784185730270110070705413818359375", ExtendedDecimal.FromDouble(7.62E-4).toString());
      Assert.assertEquals("313300000000", ExtendedDecimal.FromDouble(3.133E11).toString());
    }

    /**
     * Not documented yet.
     */
    @Test
    public void ToPlainStringTest() {
      Assert.assertEquals("277220000000", ExtendedDecimal.FromString("277.22E9").ToPlainString());
      Assert.assertEquals("3911900", ExtendedDecimal.FromString("391.19E4").ToPlainString());
      Assert.assertEquals("0.00000038327", ExtendedDecimal.FromString("383.27E-9").ToPlainString());
      Assert.assertEquals("47330000000", ExtendedDecimal.FromString("47.33E9").ToPlainString());
      Assert.assertEquals("322210", ExtendedDecimal.FromString("322.21E3").ToPlainString());
      Assert.assertEquals("1.913", ExtendedDecimal.FromString("191.3E-2").ToPlainString());
      Assert.assertEquals("11917", ExtendedDecimal.FromString("119.17E2").ToPlainString());
      Assert.assertEquals("0.0001596", ExtendedDecimal.FromString("159.6E-6").ToPlainString());
      Assert.assertEquals("70160000000", ExtendedDecimal.FromString("70.16E9").ToPlainString());
      Assert.assertEquals("166240000000", ExtendedDecimal.FromString("166.24E9").ToPlainString());
      Assert.assertEquals("235250", ExtendedDecimal.FromString("235.25E3").ToPlainString());
      Assert.assertEquals("372200000", ExtendedDecimal.FromString("37.22E7").ToPlainString());
      Assert.assertEquals("32026000000", ExtendedDecimal.FromString("320.26E8").ToPlainString());
      Assert.assertEquals("0.00000012711", ExtendedDecimal.FromString("127.11E-9").ToPlainString());
      Assert.assertEquals("0.000009729", ExtendedDecimal.FromString("97.29E-7").ToPlainString());
      Assert.assertEquals("175130000000", ExtendedDecimal.FromString("175.13E9").ToPlainString());
      Assert.assertEquals("0.000003821", ExtendedDecimal.FromString("38.21E-7").ToPlainString());
      Assert.assertEquals("62.8", ExtendedDecimal.FromString("6.28E1").ToPlainString());
      Assert.assertEquals("138290000", ExtendedDecimal.FromString("138.29E6").ToPlainString());
      Assert.assertEquals("1601.9", ExtendedDecimal.FromString("160.19E1").ToPlainString());
      Assert.assertEquals("35812", ExtendedDecimal.FromString("358.12E2").ToPlainString());
      Assert.assertEquals("2492800000000", ExtendedDecimal.FromString("249.28E10").ToPlainString());
      Assert.assertEquals("0.00031123", ExtendedDecimal.FromString("311.23E-6").ToPlainString());
      Assert.assertEquals("0.16433", ExtendedDecimal.FromString("164.33E-3").ToPlainString());
      Assert.assertEquals("29.920", ExtendedDecimal.FromString("299.20E-1").ToPlainString());
      Assert.assertEquals("105390", ExtendedDecimal.FromString("105.39E3").ToPlainString());
      Assert.assertEquals("3825000", ExtendedDecimal.FromString("382.5E4").ToPlainString());
      Assert.assertEquals("909", ExtendedDecimal.FromString("90.9E1").ToPlainString());
      Assert.assertEquals("32915000000", ExtendedDecimal.FromString("329.15E8").ToPlainString());
      Assert.assertEquals("24523000000", ExtendedDecimal.FromString("245.23E8").ToPlainString());
      Assert.assertEquals("0.0000009719", ExtendedDecimal.FromString("97.19E-8").ToPlainString());
      Assert.assertEquals("551200000", ExtendedDecimal.FromString("55.12E7").ToPlainString());
      Assert.assertEquals("1238", ExtendedDecimal.FromString("12.38E2").ToPlainString());
      Assert.assertEquals("0.0025020", ExtendedDecimal.FromString("250.20E-5").ToPlainString());
      Assert.assertEquals("5320", ExtendedDecimal.FromString("53.20E2").ToPlainString());
      Assert.assertEquals("14150000000", ExtendedDecimal.FromString("141.5E8").ToPlainString());
      Assert.assertEquals("0.0033834", ExtendedDecimal.FromString("338.34E-5").ToPlainString());
      Assert.assertEquals("160390000000", ExtendedDecimal.FromString("160.39E9").ToPlainString());
      Assert.assertEquals("152170000", ExtendedDecimal.FromString("152.17E6").ToPlainString());
      Assert.assertEquals("13300000000", ExtendedDecimal.FromString("13.3E9").ToPlainString());
      Assert.assertEquals("13.8", ExtendedDecimal.FromString("1.38E1").ToPlainString());
      Assert.assertEquals("0.00000034821", ExtendedDecimal.FromString("348.21E-9").ToPlainString());
      Assert.assertEquals("525000000", ExtendedDecimal.FromString("52.5E7").ToPlainString());
      Assert.assertEquals("2152100000000", ExtendedDecimal.FromString("215.21E10").ToPlainString());
      Assert.assertEquals("234280000000", ExtendedDecimal.FromString("234.28E9").ToPlainString());
      Assert.assertEquals("310240000000", ExtendedDecimal.FromString("310.24E9").ToPlainString());
      Assert.assertEquals("345390000000", ExtendedDecimal.FromString("345.39E9").ToPlainString());
      Assert.assertEquals("0.00000011638", ExtendedDecimal.FromString("116.38E-9").ToPlainString());
      Assert.assertEquals("2762500000000", ExtendedDecimal.FromString("276.25E10").ToPlainString());
      Assert.assertEquals("0.0000015832", ExtendedDecimal.FromString("158.32E-8").ToPlainString());
      Assert.assertEquals("27250", ExtendedDecimal.FromString("272.5E2").ToPlainString());
      Assert.assertEquals("0.00000038933", ExtendedDecimal.FromString("389.33E-9").ToPlainString());
      Assert.assertEquals("3811500000", ExtendedDecimal.FromString("381.15E7").ToPlainString());
      Assert.assertEquals("280000", ExtendedDecimal.FromString("280.0E3").ToPlainString());
      Assert.assertEquals("0.0002742", ExtendedDecimal.FromString("274.2E-6").ToPlainString());
      Assert.assertEquals("0.000038714", ExtendedDecimal.FromString("387.14E-7").ToPlainString());
      Assert.assertEquals("0.00002277", ExtendedDecimal.FromString("227.7E-7").ToPlainString());
      Assert.assertEquals("20121", ExtendedDecimal.FromString("201.21E2").ToPlainString());
      Assert.assertEquals("255400", ExtendedDecimal.FromString("255.4E3").ToPlainString());
      Assert.assertEquals("0.000018727", ExtendedDecimal.FromString("187.27E-7").ToPlainString());
      Assert.assertEquals("0.01697", ExtendedDecimal.FromString("169.7E-4").ToPlainString());
      Assert.assertEquals("69900000000", ExtendedDecimal.FromString("69.9E9").ToPlainString());
      Assert.assertEquals("0.0320", ExtendedDecimal.FromString("3.20E-2").ToPlainString());
      Assert.assertEquals("23630", ExtendedDecimal.FromString("236.30E2").ToPlainString());
      Assert.assertEquals("0.00000022022", ExtendedDecimal.FromString("220.22E-9").ToPlainString());
      Assert.assertEquals("28.730", ExtendedDecimal.FromString("287.30E-1").ToPlainString());
      Assert.assertEquals("0.0000001563", ExtendedDecimal.FromString("156.3E-9").ToPlainString());
      Assert.assertEquals("13.623", ExtendedDecimal.FromString("136.23E-1").ToPlainString());
      Assert.assertEquals("12527000000", ExtendedDecimal.FromString("125.27E8").ToPlainString());
      Assert.assertEquals("0.000018030", ExtendedDecimal.FromString("180.30E-7").ToPlainString());
      Assert.assertEquals("3515000000", ExtendedDecimal.FromString("351.5E7").ToPlainString());
      Assert.assertEquals("28280000000", ExtendedDecimal.FromString("28.28E9").ToPlainString());
      Assert.assertEquals("0.2884", ExtendedDecimal.FromString("288.4E-3").ToPlainString());
      Assert.assertEquals("122200", ExtendedDecimal.FromString("12.22E4").ToPlainString());
      Assert.assertEquals("0.002575", ExtendedDecimal.FromString("257.5E-5").ToPlainString());
      Assert.assertEquals("389200", ExtendedDecimal.FromString("389.20E3").ToPlainString());
      Assert.assertEquals("0.03949", ExtendedDecimal.FromString("394.9E-4").ToPlainString());
      Assert.assertEquals("0.000013426", ExtendedDecimal.FromString("134.26E-7").ToPlainString());
      Assert.assertEquals("5829000", ExtendedDecimal.FromString("58.29E5").ToPlainString());
      Assert.assertEquals("0.000885", ExtendedDecimal.FromString("88.5E-5").ToPlainString());
      Assert.assertEquals("0.019329", ExtendedDecimal.FromString("193.29E-4").ToPlainString());
      Assert.assertEquals("713500000000", ExtendedDecimal.FromString("71.35E10").ToPlainString());
      Assert.assertEquals("2520", ExtendedDecimal.FromString("252.0E1").ToPlainString());
      Assert.assertEquals("0.000000532", ExtendedDecimal.FromString("53.2E-8").ToPlainString());
      Assert.assertEquals("18.120", ExtendedDecimal.FromString("181.20E-1").ToPlainString());
      Assert.assertEquals("0.00000005521", ExtendedDecimal.FromString("55.21E-9").ToPlainString());
      Assert.assertEquals("57.31", ExtendedDecimal.FromString("57.31E0").ToPlainString());
      Assert.assertEquals("0.00000011313", ExtendedDecimal.FromString("113.13E-9").ToPlainString());
      Assert.assertEquals("532.3", ExtendedDecimal.FromString("53.23E1").ToPlainString());
      Assert.assertEquals("0.000036837", ExtendedDecimal.FromString("368.37E-7").ToPlainString());
      Assert.assertEquals("0.01874", ExtendedDecimal.FromString("187.4E-4").ToPlainString());
      Assert.assertEquals("526000000", ExtendedDecimal.FromString("5.26E8").ToPlainString());
      Assert.assertEquals("3083200", ExtendedDecimal.FromString("308.32E4").ToPlainString());
      Assert.assertEquals("0.7615", ExtendedDecimal.FromString("76.15E-2").ToPlainString());
      Assert.assertEquals("1173800000", ExtendedDecimal.FromString("117.38E7").ToPlainString());
      Assert.assertEquals("0.001537", ExtendedDecimal.FromString("15.37E-4").ToPlainString());
      Assert.assertEquals("145.3", ExtendedDecimal.FromString("145.3E0").ToPlainString());
      Assert.assertEquals("22629000000", ExtendedDecimal.FromString("226.29E8").ToPlainString());
      Assert.assertEquals("2242600000000", ExtendedDecimal.FromString("224.26E10").ToPlainString());
      Assert.assertEquals("0.00000026818", ExtendedDecimal.FromString("268.18E-9").ToPlainString());
    }

    /**
     * Not documented yet.
     */
    @Test
    public void ToEngineeringStringTest() {
      Assert.assertEquals("8.912", ExtendedDecimal.FromString("89.12E-1").ToEngineeringString());
      Assert.assertEquals("0.024231", ExtendedDecimal.FromString("242.31E-4").ToEngineeringString());
      Assert.assertEquals("22.918E+6", ExtendedDecimal.FromString("229.18E5").ToEngineeringString());
      Assert.assertEquals("0.000032618", ExtendedDecimal.FromString("326.18E-7").ToEngineeringString());
      Assert.assertEquals("55.0E+6", ExtendedDecimal.FromString("55.0E6").ToEngineeringString());
      Assert.assertEquals("224.36E+3", ExtendedDecimal.FromString("224.36E3").ToEngineeringString());
      Assert.assertEquals("230.12E+9", ExtendedDecimal.FromString("230.12E9").ToEngineeringString());
      Assert.assertEquals("0.000011320", ExtendedDecimal.FromString("113.20E-7").ToEngineeringString());
      Assert.assertEquals("317.7E-9", ExtendedDecimal.FromString("317.7E-9").ToEngineeringString());
      Assert.assertEquals("3.393", ExtendedDecimal.FromString("339.3E-2").ToEngineeringString());
      Assert.assertEquals("27.135E+9", ExtendedDecimal.FromString("271.35E8").ToEngineeringString());
      Assert.assertEquals("377.19E-9", ExtendedDecimal.FromString("377.19E-9").ToEngineeringString());
      Assert.assertEquals("3.2127E+9", ExtendedDecimal.FromString("321.27E7").ToEngineeringString());
      Assert.assertEquals("2.9422", ExtendedDecimal.FromString("294.22E-2").ToEngineeringString());
      Assert.assertEquals("0.0000011031", ExtendedDecimal.FromString("110.31E-8").ToEngineeringString());
      Assert.assertEquals("2.4324", ExtendedDecimal.FromString("243.24E-2").ToEngineeringString());
      Assert.assertEquals("0.0006412", ExtendedDecimal.FromString("64.12E-5").ToEngineeringString());
      Assert.assertEquals("1422.3", ExtendedDecimal.FromString("142.23E1").ToEngineeringString());
      Assert.assertEquals("293.0", ExtendedDecimal.FromString("293.0E0").ToEngineeringString());
      Assert.assertEquals("0.0000025320", ExtendedDecimal.FromString("253.20E-8").ToEngineeringString());
      Assert.assertEquals("36.66E+9", ExtendedDecimal.FromString("366.6E8").ToEngineeringString());
      Assert.assertEquals("3.4526E+12", ExtendedDecimal.FromString("345.26E10").ToEngineeringString());
      Assert.assertEquals("2.704", ExtendedDecimal.FromString("270.4E-2").ToEngineeringString());
      Assert.assertEquals("432E+6", ExtendedDecimal.FromString("4.32E8").ToEngineeringString());
      Assert.assertEquals("224.22", ExtendedDecimal.FromString("224.22E0").ToEngineeringString());
      Assert.assertEquals("0.000031530", ExtendedDecimal.FromString("315.30E-7").ToEngineeringString());
      Assert.assertEquals("11.532E+6", ExtendedDecimal.FromString("115.32E5").ToEngineeringString());
      Assert.assertEquals("39420", ExtendedDecimal.FromString("394.20E2").ToEngineeringString());
      Assert.assertEquals("67.24E-9", ExtendedDecimal.FromString("67.24E-9").ToEngineeringString());
      Assert.assertEquals("34933", ExtendedDecimal.FromString("349.33E2").ToEngineeringString());
      Assert.assertEquals("67.8E-9", ExtendedDecimal.FromString("67.8E-9").ToEngineeringString());
      Assert.assertEquals("19.231E+6", ExtendedDecimal.FromString("192.31E5").ToEngineeringString());
      Assert.assertEquals("1.7317E+9", ExtendedDecimal.FromString("173.17E7").ToEngineeringString());
      Assert.assertEquals("43.9", ExtendedDecimal.FromString("43.9E0").ToEngineeringString());
      Assert.assertEquals("0.0000016812", ExtendedDecimal.FromString("168.12E-8").ToEngineeringString());
      Assert.assertEquals("3.715E+12", ExtendedDecimal.FromString("371.5E10").ToEngineeringString());
      Assert.assertEquals("424E-9", ExtendedDecimal.FromString("42.4E-8").ToEngineeringString());
      Assert.assertEquals("1.6123E+12", ExtendedDecimal.FromString("161.23E10").ToEngineeringString());
      Assert.assertEquals("302.8E+6", ExtendedDecimal.FromString("302.8E6").ToEngineeringString());
      Assert.assertEquals("175.13", ExtendedDecimal.FromString("175.13E0").ToEngineeringString());
      Assert.assertEquals("298.20E-9", ExtendedDecimal.FromString("298.20E-9").ToEngineeringString());
      Assert.assertEquals("36.223E+9", ExtendedDecimal.FromString("362.23E8").ToEngineeringString());
      Assert.assertEquals("27739", ExtendedDecimal.FromString("277.39E2").ToEngineeringString());
      Assert.assertEquals("0.011734", ExtendedDecimal.FromString("117.34E-4").ToEngineeringString());
      Assert.assertEquals("190.13E-9", ExtendedDecimal.FromString("190.13E-9").ToEngineeringString());
      Assert.assertEquals("3.5019", ExtendedDecimal.FromString("350.19E-2").ToEngineeringString());
      Assert.assertEquals("383.27E-9", ExtendedDecimal.FromString("383.27E-9").ToEngineeringString());
      Assert.assertEquals("24.217E+6", ExtendedDecimal.FromString("242.17E5").ToEngineeringString());
      Assert.assertEquals("2.9923E+9", ExtendedDecimal.FromString("299.23E7").ToEngineeringString());
      Assert.assertEquals("3.0222", ExtendedDecimal.FromString("302.22E-2").ToEngineeringString());
      Assert.assertEquals("0.04521", ExtendedDecimal.FromString("45.21E-3").ToEngineeringString());
      Assert.assertEquals("15.00", ExtendedDecimal.FromString("150.0E-1").ToEngineeringString());
      Assert.assertEquals("290E+3", ExtendedDecimal.FromString("29.0E4").ToEngineeringString());
      Assert.assertEquals("263.37E+3", ExtendedDecimal.FromString("263.37E3").ToEngineeringString());
      Assert.assertEquals("28.321", ExtendedDecimal.FromString("283.21E-1").ToEngineeringString());
      Assert.assertEquals("21.32", ExtendedDecimal.FromString("21.32E0").ToEngineeringString());
      Assert.assertEquals("0.00006920", ExtendedDecimal.FromString("69.20E-6").ToEngineeringString());
      Assert.assertEquals("0.0728", ExtendedDecimal.FromString("72.8E-3").ToEngineeringString());
      Assert.assertEquals("1.646E+9", ExtendedDecimal.FromString("164.6E7").ToEngineeringString());
      Assert.assertEquals("1.1817", ExtendedDecimal.FromString("118.17E-2").ToEngineeringString());
      Assert.assertEquals("0.000026235", ExtendedDecimal.FromString("262.35E-7").ToEngineeringString());
      Assert.assertEquals("23.37E+6", ExtendedDecimal.FromString("233.7E5").ToEngineeringString());
      Assert.assertEquals("391.24", ExtendedDecimal.FromString("391.24E0").ToEngineeringString());
      Assert.assertEquals("2213.6", ExtendedDecimal.FromString("221.36E1").ToEngineeringString());
      Assert.assertEquals("353.32", ExtendedDecimal.FromString("353.32E0").ToEngineeringString());
      Assert.assertEquals("0.012931", ExtendedDecimal.FromString("129.31E-4").ToEngineeringString());
      Assert.assertEquals("0.0017626", ExtendedDecimal.FromString("176.26E-5").ToEngineeringString());
      Assert.assertEquals("207.5E+3", ExtendedDecimal.FromString("207.5E3").ToEngineeringString());
      Assert.assertEquals("314.10", ExtendedDecimal.FromString("314.10E0").ToEngineeringString());
      Assert.assertEquals("379.20E+9", ExtendedDecimal.FromString("379.20E9").ToEngineeringString());
      Assert.assertEquals("0.00037912", ExtendedDecimal.FromString("379.12E-6").ToEngineeringString());
      Assert.assertEquals("743.8E-9", ExtendedDecimal.FromString("74.38E-8").ToEngineeringString());
      Assert.assertEquals("234.17E-9", ExtendedDecimal.FromString("234.17E-9").ToEngineeringString());
      Assert.assertEquals("132.6E+6", ExtendedDecimal.FromString("13.26E7").ToEngineeringString());
      Assert.assertEquals("25.15E+6", ExtendedDecimal.FromString("251.5E5").ToEngineeringString());
      Assert.assertEquals("87.32", ExtendedDecimal.FromString("87.32E0").ToEngineeringString());
      Assert.assertEquals("3.3116E+9", ExtendedDecimal.FromString("331.16E7").ToEngineeringString());
      Assert.assertEquals("6.14E+9", ExtendedDecimal.FromString("61.4E8").ToEngineeringString());
      Assert.assertEquals("0.0002097", ExtendedDecimal.FromString("209.7E-6").ToEngineeringString());
      Assert.assertEquals("5.4E+6", ExtendedDecimal.FromString("5.4E6").ToEngineeringString());
      Assert.assertEquals("219.9", ExtendedDecimal.FromString("219.9E0").ToEngineeringString());
      Assert.assertEquals("0.00002631", ExtendedDecimal.FromString("26.31E-6").ToEngineeringString());
      Assert.assertEquals("482.8E+6", ExtendedDecimal.FromString("48.28E7").ToEngineeringString());
      Assert.assertEquals("267.8", ExtendedDecimal.FromString("267.8E0").ToEngineeringString());
      Assert.assertEquals("0.3209", ExtendedDecimal.FromString("320.9E-3").ToEngineeringString());
      Assert.assertEquals("0.30015", ExtendedDecimal.FromString("300.15E-3").ToEngineeringString());
      Assert.assertEquals("2.6011E+6", ExtendedDecimal.FromString("260.11E4").ToEngineeringString());
      Assert.assertEquals("1.1429", ExtendedDecimal.FromString("114.29E-2").ToEngineeringString());
      Assert.assertEquals("0.0003060", ExtendedDecimal.FromString("306.0E-6").ToEngineeringString());
      Assert.assertEquals("97.7E+3", ExtendedDecimal.FromString("97.7E3").ToEngineeringString());
      Assert.assertEquals("12.229E+9", ExtendedDecimal.FromString("122.29E8").ToEngineeringString());
      Assert.assertEquals("6.94E+3", ExtendedDecimal.FromString("69.4E2").ToEngineeringString());
      Assert.assertEquals("383.5", ExtendedDecimal.FromString("383.5E0").ToEngineeringString());
      Assert.assertEquals("315.30E+3", ExtendedDecimal.FromString("315.30E3").ToEngineeringString());
      Assert.assertEquals("130.38E+9", ExtendedDecimal.FromString("130.38E9").ToEngineeringString());
      Assert.assertEquals("206.16E+9", ExtendedDecimal.FromString("206.16E9").ToEngineeringString());
      Assert.assertEquals("304.28E-9", ExtendedDecimal.FromString("304.28E-9").ToEngineeringString());
      Assert.assertEquals("661.3E+3", ExtendedDecimal.FromString("66.13E4").ToEngineeringString());
      Assert.assertEquals("1.8533", ExtendedDecimal.FromString("185.33E-2").ToEngineeringString());
      Assert.assertEquals("70.7E+6", ExtendedDecimal.FromString("70.7E6").ToEngineeringString());
    }

    /**
     * Not documented yet.
     */
    @Test
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

    /**
     * Not documented yet.
     */
    @Test
    public void TestAsByte() {
      for (int i = 0; i < 255; ++i) {
        Assert.assertEquals((byte)i, CBORObject.FromObject(i).AsByte());
      }
      for (int i = -200; i < 0; ++i) {
        try {
          CBORObject.FromObject(i).AsByte();
        } catch (ArithmeticException ex) {
        } catch (Exception ex) {
          Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
        }
      }
      for (int i = 256; i < 512; ++i) {
        try {
          CBORObject.FromObject(i).AsByte();
        } catch (ArithmeticException ex) {
        } catch (Exception ex) {
          Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
        }
      }
    }
    @Test(expected=CBORException.class)
    public void TestByteStringStreamNoIndefiniteWithinDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[] {  0x5F,
        0x41,
        0x20,
        0x5F,
        0x41,
        0x20,
        (byte)0xFF,
        (byte)0xFF  });
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestExtendedFloatDecFrac() {
      ExtendedFloat bf;
      bf = ExtendedFloat.FromInt64(20);
      Assert.assertEquals("20", ExtendedDecimal.FromExtendedFloat(bf).toString());
      bf = ExtendedFloat.Create(BigInteger.valueOf(3), BigInteger.valueOf(-1));
      Assert.assertEquals("1.5", ExtendedDecimal.FromExtendedFloat(bf).toString());
      bf = ExtendedFloat.Create(BigInteger.valueOf(-3), BigInteger.valueOf(-1));
      Assert.assertEquals("-1.5", ExtendedDecimal.FromExtendedFloat(bf).toString());
      ExtendedDecimal df;
      df = ExtendedDecimal.FromInt64(20);
      Assert.assertEquals("20", df.ToExtendedFloat().toString());
      df = ExtendedDecimal.FromInt64(-20);
      Assert.assertEquals("-20", df.ToExtendedFloat().toString());
      df = ExtendedDecimal.Create(BigInteger.valueOf(15), BigInteger.valueOf(-1));
      Assert.assertEquals("1.5", df.ToExtendedFloat().toString());
      df = ExtendedDecimal.Create(BigInteger.valueOf(-15), BigInteger.valueOf(-1));
      Assert.assertEquals("-1.5", df.ToExtendedFloat().toString());
    }
    @Test
    public void TestDecFracToSingleDoubleHighExponents() {
      if (-5.731800748367376E125d != ExtendedDecimal.FromString("-57318007483673759194E+106").ToDouble()) {
        Assert.fail("numberObject double -57318007483673759194E+106\nExpected: -5.731800748367376E125d\nWas: " + ExtendedDecimal.FromString("-57318007483673759194E+106").ToDouble());
      }
      if (914323.0f != ExtendedDecimal.FromString("914323").ToSingle()) {
        Assert.fail("numberObject single 914323\nExpected: 914323.0f\nWas: " + ExtendedDecimal.FromString("914323").ToSingle());
      }
      if (914323.0d != ExtendedDecimal.FromString("914323").ToDouble()) {
        Assert.fail("numberObject double 914323\nExpected: 914323.0d\nWas: " + ExtendedDecimal.FromString("914323").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-57318007483673759194E+106").ToSingle()) {
        Assert.fail("numberObject single -57318007483673759194E+106\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-57318007483673759194E+106").ToSingle());
      }
      if (0.0f != ExtendedDecimal.FromString("420685230629E-264").ToSingle()) {
        Assert.fail("numberObject single 420685230629E-264\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("420685230629E-264").ToSingle());
      }
      if (4.20685230629E-253d != ExtendedDecimal.FromString("420685230629E-264").ToDouble()) {
        Assert.fail("numberObject double 420685230629E-264\nExpected: 4.20685230629E-253d\nWas: " + ExtendedDecimal.FromString("420685230629E-264").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("1089152800893419E+168").ToSingle()) {
        Assert.fail("numberObject single 1089152800893419E+168\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("1089152800893419E+168").ToSingle());
      }
      if (1.089152800893419E183d != ExtendedDecimal.FromString("1089152800893419E+168").ToDouble()) {
        Assert.fail("numberObject double 1089152800893419E+168\nExpected: 1.089152800893419E183d\nWas: " + ExtendedDecimal.FromString("1089152800893419E+168").ToDouble());
      }
      if (1.5936804E7f != ExtendedDecimal.FromString("15936804").ToSingle()) {
        Assert.fail("numberObject single 15936804\nExpected: 1.5936804E7f\nWas: " + ExtendedDecimal.FromString("15936804").ToSingle());
      }
      if (1.5936804E7d != ExtendedDecimal.FromString("15936804").ToDouble()) {
        Assert.fail("numberObject double 15936804\nExpected: 1.5936804E7d\nWas: " + ExtendedDecimal.FromString("15936804").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-24681.2332E+61").ToSingle()) {
        Assert.fail("numberObject single -24681.2332E+61\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-24681.2332E+61").ToSingle());
      }
      if (-2.46812332E65d != ExtendedDecimal.FromString("-24681.2332E+61").ToDouble()) {
        Assert.fail("numberObject double -24681.2332E+61\nExpected: -2.46812332E65d\nWas: " + ExtendedDecimal.FromString("-24681.2332E+61").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-417509591569.6827833177512321E-93").ToSingle()) {
        Assert.fail("numberObject single -417509591569.6827833177512321E-93\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-417509591569.6827833177512321E-93").ToSingle());
      }
      if (-4.175095915696828E-82d != ExtendedDecimal.FromString("-417509591569.6827833177512321E-93").ToDouble()) {
        Assert.fail("numberObject double -417509591569.6827833177512321E-93\nExpected: -4.175095915696828E-82d\nWas: " + ExtendedDecimal.FromString("-417509591569.6827833177512321E-93").ToDouble());
      }
      if (5.38988331E17f != ExtendedDecimal.FromString("538988338119784732").ToSingle()) {
        Assert.fail("numberObject single 538988338119784732\nExpected: 5.38988331E17f\nWas: " + ExtendedDecimal.FromString("538988338119784732").ToSingle());
      }
      if (5.389883381197847E17d != ExtendedDecimal.FromString("538988338119784732").ToDouble()) {
        Assert.fail("numberObject double 538988338119784732\nExpected: 5.389883381197847E17d\nWas: " + ExtendedDecimal.FromString("538988338119784732").ToDouble());
      }
      if (260.14423f != ExtendedDecimal.FromString("260.1442248").ToSingle()) {
        Assert.fail("numberObject single 260.1442248\nExpected: 260.14423f\nWas: " + ExtendedDecimal.FromString("260.1442248").ToSingle());
      }
      if (260.1442248d != ExtendedDecimal.FromString("260.1442248").ToDouble()) {
        Assert.fail("numberObject double 260.1442248\nExpected: 260.1442248d\nWas: " + ExtendedDecimal.FromString("260.1442248").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-8457715957008143770.130850853640402959E-181").ToSingle()) {
        Assert.fail("numberObject single -8457715957008143770.130850853640402959E-181\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-8457715957008143770.130850853640402959E-181").ToSingle());
      }
      if (-8.457715957008144E-163d != ExtendedDecimal.FromString("-8457715957008143770.130850853640402959E-181").ToDouble()) {
        Assert.fail("numberObject double -8457715957008143770.130850853640402959E-181\nExpected: -8.457715957008144E-163d\nWas: " + ExtendedDecimal.FromString("-8457715957008143770.130850853640402959E-181").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("22.7178448747E-225").ToSingle()) {
        Assert.fail("numberObject single 22.7178448747E-225\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("22.7178448747E-225").ToSingle());
      }
      if (2.27178448747E-224d != ExtendedDecimal.FromString("22.7178448747E-225").ToDouble()) {
        Assert.fail("numberObject double 22.7178448747E-225\nExpected: 2.27178448747E-224d\nWas: " + ExtendedDecimal.FromString("22.7178448747E-225").ToDouble());
      }
      if (-790581.44f != ExtendedDecimal.FromString("-790581.4576317018014").ToSingle()) {
        Assert.fail("numberObject single -790581.4576317018014\nExpected: -790581.44f\nWas: " + ExtendedDecimal.FromString("-790581.4576317018014").ToSingle());
      }
      if (-790581.4576317018d != ExtendedDecimal.FromString("-790581.4576317018014").ToDouble()) {
        Assert.fail("numberObject double -790581.4576317018014\nExpected: -790581.4576317018d\nWas: " + ExtendedDecimal.FromString("-790581.4576317018014").ToDouble());
      }
      if (-1.80151695E16f != ExtendedDecimal.FromString("-18015168704168440").ToSingle()) {
        Assert.fail("numberObject single -18015168704168440\nExpected: -1.80151695E16f\nWas: " + ExtendedDecimal.FromString("-18015168704168440").ToSingle());
      }
      if (-1.801516870416844E16d != ExtendedDecimal.FromString("-18015168704168440").ToDouble()) {
        Assert.fail("numberObject double -18015168704168440\nExpected: -1.801516870416844E16d\nWas: " + ExtendedDecimal.FromString("-18015168704168440").ToDouble());
      }
      if (-36.0f != ExtendedDecimal.FromString("-36").ToSingle()) {
        Assert.fail("numberObject single -36\nExpected: -36.0f\nWas: " + ExtendedDecimal.FromString("-36").ToSingle());
      }
      if (-36.0d != ExtendedDecimal.FromString("-36").ToDouble()) {
        Assert.fail("numberObject double -36\nExpected: -36.0d\nWas: " + ExtendedDecimal.FromString("-36").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("653060307988076E-230").ToSingle()) {
        Assert.fail("numberObject single 653060307988076E-230\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("653060307988076E-230").ToSingle());
      }
      if (6.53060307988076E-216d != ExtendedDecimal.FromString("653060307988076E-230").ToDouble()) {
        Assert.fail("numberObject double 653060307988076E-230\nExpected: 6.53060307988076E-216d\nWas: " + ExtendedDecimal.FromString("653060307988076E-230").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-4446345.5911E+316").ToSingle()) {
        Assert.fail("numberObject single -4446345.5911E+316\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-4446345.5911E+316").ToSingle());
      }
      if (Double.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-4446345.5911E+316").ToDouble()) {
        Assert.fail("numberObject double -4446345.5911E+316\nExpected: Double.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-4446345.5911E+316").ToDouble());
      }
      if (-5.3940226E15f != ExtendedDecimal.FromString("-5394022706804125.84338479899885").ToSingle()) {
        Assert.fail("numberObject single -5394022706804125.84338479899885\nExpected: -5.3940226E15f\nWas: " + ExtendedDecimal.FromString("-5394022706804125.84338479899885").ToSingle());
      }
      if (-5.394022706804126E15d != ExtendedDecimal.FromString("-5394022706804125.84338479899885").ToDouble()) {
        Assert.fail("numberObject double -5394022706804125.84338479899885\nExpected: -5.394022706804126E15d\nWas: " + ExtendedDecimal.FromString("-5394022706804125.84338479899885").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("310504020304E+181").ToSingle()) {
        Assert.fail("numberObject single 310504020304E+181\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("310504020304E+181").ToSingle());
      }
      if (3.10504020304E192d != ExtendedDecimal.FromString("310504020304E+181").ToDouble()) {
        Assert.fail("numberObject double 310504020304E+181\nExpected: 3.10504020304E192d\nWas: " + ExtendedDecimal.FromString("310504020304E+181").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-164609450222646.21988340572652533E+317").ToSingle()) {
        Assert.fail("numberObject single -164609450222646.21988340572652533E+317\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-164609450222646.21988340572652533E+317").ToSingle());
      }
      if (Double.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-164609450222646.21988340572652533E+317").ToDouble()) {
        Assert.fail("numberObject double -164609450222646.21988340572652533E+317\nExpected: Double.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-164609450222646.21988340572652533E+317").ToDouble());
      }
      if (7.1524661E18f != ExtendedDecimal.FromString("7152466127871812565.075310").ToSingle()) {
        Assert.fail("numberObject single 7152466127871812565.075310\nExpected: 7.1524661E18f\nWas: " + ExtendedDecimal.FromString("7152466127871812565.075310").ToSingle());
      }
      if (7.1524661278718126E18d != ExtendedDecimal.FromString("7152466127871812565.075310").ToDouble()) {
        Assert.fail("numberObject double 7152466127871812565.075310\nExpected: 7.1524661278718126E18d\nWas: " + ExtendedDecimal.FromString("7152466127871812565.075310").ToDouble());
      }
      if (925.0f != ExtendedDecimal.FromString("925").ToSingle()) {
        Assert.fail("numberObject single 925\nExpected: 925.0f\nWas: " + ExtendedDecimal.FromString("925").ToSingle());
      }
      if (925.0d != ExtendedDecimal.FromString("925").ToDouble()) {
        Assert.fail("numberObject double 925\nExpected: 925.0d\nWas: " + ExtendedDecimal.FromString("925").ToDouble());
      }
      if (34794.0f != ExtendedDecimal.FromString("34794").ToSingle()) {
        Assert.fail("numberObject single 34794\nExpected: 34794.0f\nWas: " + ExtendedDecimal.FromString("34794").ToSingle());
      }
      if (34794.0d != ExtendedDecimal.FromString("34794").ToDouble()) {
        Assert.fail("numberObject double 34794\nExpected: 34794.0d\nWas: " + ExtendedDecimal.FromString("34794").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-337655705333269E-276").ToSingle()) {
        Assert.fail("numberObject single -337655705333269E-276\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-337655705333269E-276").ToSingle());
      }
      if (-3.37655705333269E-262d != ExtendedDecimal.FromString("-337655705333269E-276").ToDouble()) {
        Assert.fail("numberObject double -337655705333269E-276\nExpected: -3.37655705333269E-262d\nWas: " + ExtendedDecimal.FromString("-337655705333269E-276").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-564484627E-81").ToSingle()) {
        Assert.fail("numberObject single -564484627E-81\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-564484627E-81").ToSingle());
      }
      if (-5.64484627E-73d != ExtendedDecimal.FromString("-564484627E-81").ToDouble()) {
        Assert.fail("numberObject double -564484627E-81\nExpected: -5.64484627E-73d\nWas: " + ExtendedDecimal.FromString("-564484627E-81").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-249095219081.80985049618E+175").ToSingle()) {
        Assert.fail("numberObject single -249095219081.80985049618E+175\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-249095219081.80985049618E+175").ToSingle());
      }
      if (-2.4909521908180986E186d != ExtendedDecimal.FromString("-249095219081.80985049618E+175").ToDouble()) {
        Assert.fail("numberObject double -249095219081.80985049618E+175\nExpected: -2.4909521908180986E186d\nWas: " + ExtendedDecimal.FromString("-249095219081.80985049618E+175").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-1696361380616078392E+221").ToSingle()) {
        Assert.fail("numberObject single -1696361380616078392E+221\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-1696361380616078392E+221").ToSingle());
      }
      if (-1.6963613806160784E239d != ExtendedDecimal.FromString("-1696361380616078392E+221").ToDouble()) {
        Assert.fail("numberObject double -1696361380616078392E+221\nExpected: -1.6963613806160784E239d\nWas: " + ExtendedDecimal.FromString("-1696361380616078392E+221").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("61520501993928105481.8536829047214988E+205").ToSingle()) {
        Assert.fail("numberObject single 61520501993928105481.8536829047214988E+205\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("61520501993928105481.8536829047214988E+205").ToSingle());
      }
      if (6.15205019939281E224d != ExtendedDecimal.FromString("61520501993928105481.8536829047214988E+205").ToDouble()) {
        Assert.fail("numberObject double 61520501993928105481.8536829047214988E+205\nExpected: 6.15205019939281E224d\nWas: " + ExtendedDecimal.FromString("61520501993928105481.8536829047214988E+205").ToDouble());
      }
      if (2.08756651E14f != ExtendedDecimal.FromString("208756654290770").ToSingle()) {
        Assert.fail("numberObject single 208756654290770\nExpected: 2.08756651E14f\nWas: " + ExtendedDecimal.FromString("208756654290770").ToSingle());
      }
      if (2.0875665429077E14d != ExtendedDecimal.FromString("208756654290770").ToDouble()) {
        Assert.fail("numberObject double 208756654290770\nExpected: 2.0875665429077E14d\nWas: " + ExtendedDecimal.FromString("208756654290770").ToDouble());
      }
      if (-1.31098592E13f != ExtendedDecimal.FromString("-13109858687380").ToSingle()) {
        Assert.fail("numberObject single -13109858687380\nExpected: -1.31098592E13f\nWas: " + ExtendedDecimal.FromString("-13109858687380").ToSingle());
      }
      if (-1.310985868738E13d != ExtendedDecimal.FromString("-13109858687380").ToDouble()) {
        Assert.fail("numberObject double -13109858687380\nExpected: -1.310985868738E13d\nWas: " + ExtendedDecimal.FromString("-13109858687380").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("6650596004E+280").ToSingle()) {
        Assert.fail("numberObject single 6650596004E+280\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("6650596004E+280").ToSingle());
      }
      if (6.650596004E289d != ExtendedDecimal.FromString("6650596004E+280").ToDouble()) {
        Assert.fail("numberObject double 6650596004E+280\nExpected: 6.650596004E289d\nWas: " + ExtendedDecimal.FromString("6650596004E+280").ToDouble());
      }
      if (-9.2917935E13f != ExtendedDecimal.FromString("-92917937534357E0").ToSingle()) {
        Assert.fail("numberObject single -92917937534357E0\nExpected: -9.2917935E13f\nWas: " + ExtendedDecimal.FromString("-92917937534357E0").ToSingle());
      }
      if (-9.2917937534357E13d != ExtendedDecimal.FromString("-92917937534357E0").ToDouble()) {
        Assert.fail("numberObject double -92917937534357E0\nExpected: -9.2917937534357E13d\nWas: " + ExtendedDecimal.FromString("-92917937534357E0").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-46E-153").ToSingle()) {
        Assert.fail("numberObject single -46E-153\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-46E-153").ToSingle());
      }
      if (-4.6E-152d != ExtendedDecimal.FromString("-46E-153").ToDouble()) {
        Assert.fail("numberObject double -46E-153\nExpected: -4.6E-152d\nWas: " + ExtendedDecimal.FromString("-46E-153").ToDouble());
      }
      if (1.05161414E13f != ExtendedDecimal.FromString("10516141645281.77872161523035480").ToSingle()) {
        Assert.fail("numberObject single 10516141645281.77872161523035480\nExpected: 1.05161414E13f\nWas: " + ExtendedDecimal.FromString("10516141645281.77872161523035480").ToSingle());
      }
      if (1.051614164528178E13d != ExtendedDecimal.FromString("10516141645281.77872161523035480").ToDouble()) {
        Assert.fail("numberObject double 10516141645281.77872161523035480\nExpected: 1.051614164528178E13d\nWas: " + ExtendedDecimal.FromString("10516141645281.77872161523035480").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-8312147094254E+299").ToSingle()) {
        Assert.fail("numberObject single -8312147094254E+299\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-8312147094254E+299").ToSingle());
      }
      if (Double.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-8312147094254E+299").ToDouble()) {
        Assert.fail("numberObject double -8312147094254E+299\nExpected: Double.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-8312147094254E+299").ToDouble());
      }
      if (5.10270368E8f != ExtendedDecimal.FromString("510270376.1879").ToSingle()) {
        Assert.fail("numberObject single 510270376.1879\nExpected: 5.10270368E8f\nWas: " + ExtendedDecimal.FromString("510270376.1879").ToSingle());
      }
      if (5.102703761879E8d != ExtendedDecimal.FromString("510270376.1879").ToDouble()) {
        Assert.fail("numberObject double 510270376.1879\nExpected: 5.102703761879E8d\nWas: " + ExtendedDecimal.FromString("510270376.1879").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-693696E-143").ToSingle()) {
        Assert.fail("numberObject single -693696E-143\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-693696E-143").ToSingle());
      }
      if (-6.93696E-138d != ExtendedDecimal.FromString("-693696E-143").ToDouble()) {
        Assert.fail("numberObject double -693696E-143\nExpected: -6.93696E-138d\nWas: " + ExtendedDecimal.FromString("-693696E-143").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-91.43E+139").ToSingle()) {
        Assert.fail("numberObject single -91.43E+139\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-91.43E+139").ToSingle());
      }
      if (-9.143E140d != ExtendedDecimal.FromString("-91.43E+139").ToDouble()) {
        Assert.fail("numberObject double -91.43E+139\nExpected: -9.143E140d\nWas: " + ExtendedDecimal.FromString("-91.43E+139").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-4103819741762400.45807953367286162E+235").ToSingle()) {
        Assert.fail("numberObject single -4103819741762400.45807953367286162E+235\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-4103819741762400.45807953367286162E+235").ToSingle());
      }
      if (-4.1038197417624E250d != ExtendedDecimal.FromString("-4103819741762400.45807953367286162E+235").ToDouble()) {
        Assert.fail("numberObject double -4103819741762400.45807953367286162E+235\nExpected: -4.1038197417624E250d\nWas: " + ExtendedDecimal.FromString("-4103819741762400.45807953367286162E+235").ToDouble());
      }
      if (-1.44700998E11f != ExtendedDecimal.FromString("-144701002301.18954542331279957").ToSingle()) {
        Assert.fail("numberObject single -144701002301.18954542331279957\nExpected: -1.44700998E11f\nWas: " + ExtendedDecimal.FromString("-144701002301.18954542331279957").ToSingle());
      }
      if (-1.4470100230118954E11d != ExtendedDecimal.FromString("-144701002301.18954542331279957").ToDouble()) {
        Assert.fail("numberObject double -144701002301.18954542331279957\nExpected: -1.4470100230118954E11d\nWas: " + ExtendedDecimal.FromString("-144701002301.18954542331279957").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("73.01E+211").ToSingle()) {
        Assert.fail("numberObject single 73.01E+211\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("73.01E+211").ToSingle());
      }
      if (7.301E212d != ExtendedDecimal.FromString("73.01E+211").ToDouble()) {
        Assert.fail("numberObject double 73.01E+211\nExpected: 7.301E212d\nWas: " + ExtendedDecimal.FromString("73.01E+211").ToDouble());
      }
      if (-4.4030403E9f != ExtendedDecimal.FromString("-4403040441").ToSingle()) {
        Assert.fail("numberObject single -4403040441\nExpected: -4.4030403E9f\nWas: " + ExtendedDecimal.FromString("-4403040441").ToSingle());
      }
      if (-4.403040441E9d != ExtendedDecimal.FromString("-4403040441").ToDouble()) {
        Assert.fail("numberObject double -4403040441\nExpected: -4.403040441E9d\nWas: " + ExtendedDecimal.FromString("-4403040441").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-19E+64").ToSingle()) {
        Assert.fail("numberObject single -19E+64\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-19E+64").ToSingle());
      }
      if (-1.9E65d != ExtendedDecimal.FromString("-19E+64").ToDouble()) {
        Assert.fail("numberObject double -19E+64\nExpected: -1.9E65d\nWas: " + ExtendedDecimal.FromString("-19E+64").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("6454087684516815.5353496080253E-144").ToSingle()) {
        Assert.fail("numberObject single 6454087684516815.5353496080253E-144\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("6454087684516815.5353496080253E-144").ToSingle());
      }
      if (6.454087684516816E-129d != ExtendedDecimal.FromString("6454087684516815.5353496080253E-144").ToDouble()) {
        Assert.fail("numberObject double 6454087684516815.5353496080253E-144\nExpected: 6.454087684516816E-129d\nWas: " + ExtendedDecimal.FromString("6454087684516815.5353496080253E-144").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("1051852710343668.522107559786846776E+278").ToSingle()) {
        Assert.fail("numberObject single 1051852710343668.522107559786846776E+278\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("1051852710343668.522107559786846776E+278").ToSingle());
      }
      if (1.0518527103436685E293d != ExtendedDecimal.FromString("1051852710343668.522107559786846776E+278").ToDouble()) {
        Assert.fail("numberObject double 1051852710343668.522107559786846776E+278\nExpected: 1.0518527103436685E293d\nWas: " + ExtendedDecimal.FromString("1051852710343668.522107559786846776E+278").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("86077128802.374518623891E+218").ToSingle()) {
        Assert.fail("numberObject single 86077128802.374518623891E+218\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("86077128802.374518623891E+218").ToSingle());
      }
      if (8.607712880237452E228d != ExtendedDecimal.FromString("86077128802.374518623891E+218").ToDouble()) {
        Assert.fail("numberObject double 86077128802.374518623891E+218\nExpected: 8.607712880237452E228d\nWas: " + ExtendedDecimal.FromString("86077128802.374518623891E+218").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("367820230207102E-199").ToSingle()) {
        Assert.fail("numberObject single 367820230207102E-199\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("367820230207102E-199").ToSingle());
      }
      if (3.67820230207102E-185d != ExtendedDecimal.FromString("367820230207102E-199").ToDouble()) {
        Assert.fail("numberObject double 367820230207102E-199\nExpected: 3.67820230207102E-185d\nWas: " + ExtendedDecimal.FromString("367820230207102E-199").ToDouble());
      }
      if (9.105086E-27f != ExtendedDecimal.FromString("91050857573912688994E-46").ToSingle()) {
        Assert.fail("numberObject single 91050857573912688994E-46\nExpected: 9.105086E-27f\nWas: " + ExtendedDecimal.FromString("91050857573912688994E-46").ToSingle());
      }
      if (9.105085757391269E-27d != ExtendedDecimal.FromString("91050857573912688994E-46").ToDouble()) {
        Assert.fail("numberObject double 91050857573912688994E-46\nExpected: 9.105085757391269E-27d\nWas: " + ExtendedDecimal.FromString("91050857573912688994E-46").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("73.895899E+102").ToSingle()) {
        Assert.fail("numberObject single 73.895899E+102\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("73.895899E+102").ToSingle());
      }
      if (7.3895899E103d != ExtendedDecimal.FromString("73.895899E+102").ToDouble()) {
        Assert.fail("numberObject double 73.895899E+102\nExpected: 7.3895899E103d\nWas: " + ExtendedDecimal.FromString("73.895899E+102").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-796808893178.891470585829021E+330").ToSingle()) {
        Assert.fail("numberObject single -796808893178.891470585829021E+330\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-796808893178.891470585829021E+330").ToSingle());
      }
      if (Double.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-796808893178.891470585829021E+330").ToDouble()) {
        Assert.fail("numberObject double -796808893178.891470585829021E+330\nExpected: Double.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-796808893178.891470585829021E+330").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("275081E-206").ToSingle()) {
        Assert.fail("numberObject single 275081E-206\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("275081E-206").ToSingle());
      }
      if (2.75081E-201d != ExtendedDecimal.FromString("275081E-206").ToDouble()) {
        Assert.fail("numberObject double 275081E-206\nExpected: 2.75081E-201d\nWas: " + ExtendedDecimal.FromString("275081E-206").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-4322898910615499.82096E-95").ToSingle()) {
        Assert.fail("numberObject single -4322898910615499.82096E-95\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-4322898910615499.82096E-95").ToSingle());
      }
      if (-4.3228989106155E-80d != ExtendedDecimal.FromString("-4322898910615499.82096E-95").ToDouble()) {
        Assert.fail("numberObject double -4322898910615499.82096E-95\nExpected: -4.3228989106155E-80d\nWas: " + ExtendedDecimal.FromString("-4322898910615499.82096E-95").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("143343913109764E+63").ToSingle()) {
        Assert.fail("numberObject single 143343913109764E+63\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("143343913109764E+63").ToSingle());
      }
      if (1.43343913109764E77d != ExtendedDecimal.FromString("143343913109764E+63").ToDouble()) {
        Assert.fail("numberObject double 143343913109764E+63\nExpected: 1.43343913109764E77d\nWas: " + ExtendedDecimal.FromString("143343913109764E+63").ToDouble());
      }
      if (-7.9102981E16f != ExtendedDecimal.FromString("-79102983237104015").ToSingle()) {
        Assert.fail("numberObject single -79102983237104015\nExpected: -7.9102981E16f\nWas: " + ExtendedDecimal.FromString("-79102983237104015").ToSingle());
      }
      if (-7.9102983237104016E16d != ExtendedDecimal.FromString("-79102983237104015").ToDouble()) {
        Assert.fail("numberObject double -79102983237104015\nExpected: -7.9102983237104016E16d\nWas: " + ExtendedDecimal.FromString("-79102983237104015").ToDouble());
      }
      if (-9.07E-10f != ExtendedDecimal.FromString("-907E-12").ToSingle()) {
        Assert.fail("numberObject single -907E-12\nExpected: -9.07E-10f\nWas: " + ExtendedDecimal.FromString("-907E-12").ToSingle());
      }
      if (-9.07E-10d != ExtendedDecimal.FromString("-907E-12").ToDouble()) {
        Assert.fail("numberObject double -907E-12\nExpected: -9.07E-10d\nWas: " + ExtendedDecimal.FromString("-907E-12").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("191682103431.217475E-84").ToSingle()) {
        Assert.fail("numberObject single 191682103431.217475E-84\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("191682103431.217475E-84").ToSingle());
      }
      if (1.9168210343121748E-73d != ExtendedDecimal.FromString("191682103431.217475E-84").ToDouble()) {
        Assert.fail("numberObject double 191682103431.217475E-84\nExpected: 1.9168210343121748E-73d\nWas: " + ExtendedDecimal.FromString("191682103431.217475E-84").ToDouble());
      }
      if (-5.6E-45f != ExtendedDecimal.FromString("-492913.1840948615992120438E-50").ToSingle()) {
        Assert.fail("numberObject single -492913.1840948615992120438E-50\nExpected: -5.6E-45f\nWas: " + ExtendedDecimal.FromString("-492913.1840948615992120438E-50").ToSingle());
      }
      if (-4.929131840948616E-45d != ExtendedDecimal.FromString("-492913.1840948615992120438E-50").ToDouble()) {
        Assert.fail("numberObject double -492913.1840948615992120438E-50\nExpected: -4.929131840948616E-45d\nWas: " + ExtendedDecimal.FromString("-492913.1840948615992120438E-50").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-752873150058767E+272").ToSingle()) {
        Assert.fail("numberObject single -752873150058767E+272\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-752873150058767E+272").ToSingle());
      }
      if (-7.52873150058767E286d != ExtendedDecimal.FromString("-752873150058767E+272").ToDouble()) {
        Assert.fail("numberObject double -752873150058767E+272\nExpected: -7.52873150058767E286d\nWas: " + ExtendedDecimal.FromString("-752873150058767E+272").ToDouble());
      }
      if (27.311937f != ExtendedDecimal.FromString("27.311937404").ToSingle()) {
        Assert.fail("numberObject single 27.311937404\nExpected: 27.311937f\nWas: " + ExtendedDecimal.FromString("27.311937404").ToSingle());
      }
      if (27.311937404d != ExtendedDecimal.FromString("27.311937404").ToDouble()) {
        Assert.fail("numberObject double 27.311937404\nExpected: 27.311937404d\nWas: " + ExtendedDecimal.FromString("27.311937404").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("39147083343918E-143").ToSingle()) {
        Assert.fail("numberObject single 39147083343918E-143\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("39147083343918E-143").ToSingle());
      }
      if (3.9147083343918E-130d != ExtendedDecimal.FromString("39147083343918E-143").ToDouble()) {
        Assert.fail("numberObject double 39147083343918E-143\nExpected: 3.9147083343918E-130d\nWas: " + ExtendedDecimal.FromString("39147083343918E-143").ToDouble());
      }
      if (-1.97684019E11f != ExtendedDecimal.FromString("-197684018253").ToSingle()) {
        Assert.fail("numberObject single -197684018253\nExpected: -1.97684019E11f\nWas: " + ExtendedDecimal.FromString("-197684018253").ToSingle());
      }
      if (-1.97684018253E11d != ExtendedDecimal.FromString("-197684018253").ToDouble()) {
        Assert.fail("numberObject double -197684018253\nExpected: -1.97684018253E11d\nWas: " + ExtendedDecimal.FromString("-197684018253").ToDouble());
      }
      if (6.400822E14f != ExtendedDecimal.FromString("640082188903507").ToSingle()) {
        Assert.fail("numberObject single 640082188903507\nExpected: 6.400822E14f\nWas: " + ExtendedDecimal.FromString("640082188903507").ToSingle());
      }
      if (6.40082188903507E14d != ExtendedDecimal.FromString("640082188903507").ToDouble()) {
        Assert.fail("numberObject double 640082188903507\nExpected: 6.40082188903507E14d\nWas: " + ExtendedDecimal.FromString("640082188903507").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-913144352720144E-312").ToSingle()) {
        Assert.fail("numberObject single -913144352720144E-312\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-913144352720144E-312").ToSingle());
      }
      if (-9.13144352720144E-298d != ExtendedDecimal.FromString("-913144352720144E-312").ToDouble()) {
        Assert.fail("numberObject double -913144352720144E-312\nExpected: -9.13144352720144E-298d\nWas: " + ExtendedDecimal.FromString("-913144352720144E-312").ToDouble());
      }
      if (-3.68781005E15f != ExtendedDecimal.FromString("-3687809947210631").ToSingle()) {
        Assert.fail("numberObject single -3687809947210631\nExpected: -3.68781005E15f\nWas: " + ExtendedDecimal.FromString("-3687809947210631").ToSingle());
      }
      if (-3.687809947210631E15d != ExtendedDecimal.FromString("-3687809947210631").ToDouble()) {
        Assert.fail("numberObject double -3687809947210631\nExpected: -3.687809947210631E15d\nWas: " + ExtendedDecimal.FromString("-3687809947210631").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("53083788630724917310.06236692262351E+169").ToSingle()) {
        Assert.fail("numberObject single 53083788630724917310.06236692262351E+169\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("53083788630724917310.06236692262351E+169").ToSingle());
      }
      if (5.3083788630724916E188d != ExtendedDecimal.FromString("53083788630724917310.06236692262351E+169").ToDouble()) {
        Assert.fail("numberObject double 53083788630724917310.06236692262351E+169\nExpected: 5.3083788630724916E188d\nWas: " + ExtendedDecimal.FromString("53083788630724917310.06236692262351E+169").ToDouble());
      }
      if (-7.0943446E19f != ExtendedDecimal.FromString("-70943446332471357958").ToSingle()) {
        Assert.fail("numberObject single -70943446332471357958\nExpected: -7.0943446E19f\nWas: " + ExtendedDecimal.FromString("-70943446332471357958").ToSingle());
      }
      if (-7.094344633247136E19d != ExtendedDecimal.FromString("-70943446332471357958").ToDouble()) {
        Assert.fail("numberObject double -70943446332471357958\nExpected: -7.094344633247136E19d\nWas: " + ExtendedDecimal.FromString("-70943446332471357958").ToDouble());
      }
      if (63367.23f != ExtendedDecimal.FromString("63367.23157744207").ToSingle()) {
        Assert.fail("numberObject single 63367.23157744207\nExpected: 63367.23f\nWas: " + ExtendedDecimal.FromString("63367.23157744207").ToSingle());
      }
      if (63367.23157744207d != ExtendedDecimal.FromString("63367.23157744207").ToDouble()) {
        Assert.fail("numberObject double 63367.23157744207\nExpected: 63367.23157744207d\nWas: " + ExtendedDecimal.FromString("63367.23157744207").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("2100535E+120").ToSingle()) {
        Assert.fail("numberObject single 2100535E+120\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("2100535E+120").ToSingle());
      }
      if (2.100535E126d != ExtendedDecimal.FromString("2100535E+120").ToDouble()) {
        Assert.fail("numberObject double 2100535E+120\nExpected: 2.100535E126d\nWas: " + ExtendedDecimal.FromString("2100535E+120").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("914534543212037911E-174").ToSingle()) {
        Assert.fail("numberObject single 914534543212037911E-174\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("914534543212037911E-174").ToSingle());
      }
      if (9.14534543212038E-157d != ExtendedDecimal.FromString("914534543212037911E-174").ToDouble()) {
        Assert.fail("numberObject double 914534543212037911E-174\nExpected: 9.14534543212038E-157d\nWas: " + ExtendedDecimal.FromString("914534543212037911E-174").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-12437185743660570E-180").ToSingle()) {
        Assert.fail("numberObject single -12437185743660570E-180\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-12437185743660570E-180").ToSingle());
      }
      if (-1.243718574366057E-164d != ExtendedDecimal.FromString("-12437185743660570E-180").ToDouble()) {
        Assert.fail("numberObject double -12437185743660570E-180\nExpected: -1.243718574366057E-164d\nWas: " + ExtendedDecimal.FromString("-12437185743660570E-180").ToDouble());
      }
      if (-3.3723915E19f != ExtendedDecimal.FromString("-33723915695913879E+3").ToSingle()) {
        Assert.fail("numberObject single -33723915695913879E+3\nExpected: -3.3723915E19f\nWas: " + ExtendedDecimal.FromString("-33723915695913879E+3").ToSingle());
      }
      if (-3.3723915695913878E19d != ExtendedDecimal.FromString("-33723915695913879E+3").ToDouble()) {
        Assert.fail("numberObject double -33723915695913879E+3\nExpected: -3.3723915695913878E19d\nWas: " + ExtendedDecimal.FromString("-33723915695913879E+3").ToDouble());
      }
      if (6.3664833E10f != ExtendedDecimal.FromString("63664831787").ToSingle()) {
        Assert.fail("numberObject single 63664831787\nExpected: 6.3664833E10f\nWas: " + ExtendedDecimal.FromString("63664831787").ToSingle());
      }
      if (6.3664831787E10d != ExtendedDecimal.FromString("63664831787").ToDouble()) {
        Assert.fail("numberObject double 63664831787\nExpected: 6.3664831787E10d\nWas: " + ExtendedDecimal.FromString("63664831787").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("432187105445201137.3321724908E+97").ToSingle()) {
        Assert.fail("numberObject single 432187105445201137.3321724908E+97\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("432187105445201137.3321724908E+97").ToSingle());
      }
      if (4.321871054452011E114d != ExtendedDecimal.FromString("432187105445201137.3321724908E+97").ToDouble()) {
        Assert.fail("numberObject double 432187105445201137.3321724908E+97\nExpected: 4.321871054452011E114d\nWas: " + ExtendedDecimal.FromString("432187105445201137.3321724908E+97").ToDouble());
      }
      if (-5.1953271E13f != ExtendedDecimal.FromString("-51953270775979").ToSingle()) {
        Assert.fail("numberObject single -51953270775979\nExpected: -5.1953271E13f\nWas: " + ExtendedDecimal.FromString("-51953270775979").ToSingle());
      }
      if (-5.1953270775979E13d != ExtendedDecimal.FromString("-51953270775979").ToDouble()) {
        Assert.fail("numberObject double -51953270775979\nExpected: -5.1953270775979E13d\nWas: " + ExtendedDecimal.FromString("-51953270775979").ToDouble());
      }
      if (2.14953088E9f != ExtendedDecimal.FromString("2149530805").ToSingle()) {
        Assert.fail("numberObject single 2149530805\nExpected: 2.14953088E9f\nWas: " + ExtendedDecimal.FromString("2149530805").ToSingle());
      }
      if (2.149530805E9d != ExtendedDecimal.FromString("2149530805").ToDouble()) {
        Assert.fail("numberObject double 2149530805\nExpected: 2.149530805E9d\nWas: " + ExtendedDecimal.FromString("2149530805").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-4672759140.6362E-223").ToSingle()) {
        Assert.fail("numberObject single -4672759140.6362E-223\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-4672759140.6362E-223").ToSingle());
      }
      if (-4.6727591406362E-214d != ExtendedDecimal.FromString("-4672759140.6362E-223").ToDouble()) {
        Assert.fail("numberObject double -4672759140.6362E-223\nExpected: -4.6727591406362E-214d\nWas: " + ExtendedDecimal.FromString("-4672759140.6362E-223").ToDouble());
      }
      if (-9.0f != ExtendedDecimal.FromString("-9").ToSingle()) {
        Assert.fail("numberObject single -9\nExpected: -9.0f\nWas: " + ExtendedDecimal.FromString("-9").ToSingle());
      }
      if (-9.0d != ExtendedDecimal.FromString("-9").ToDouble()) {
        Assert.fail("numberObject double -9\nExpected: -9.0d\nWas: " + ExtendedDecimal.FromString("-9").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-1903960322936E+304").ToSingle()) {
        Assert.fail("numberObject single -1903960322936E+304\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-1903960322936E+304").ToSingle());
      }
      if (Double.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-1903960322936E+304").ToDouble()) {
        Assert.fail("numberObject double -1903960322936E+304\nExpected: Double.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-1903960322936E+304").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("405766405417980707E+316").ToSingle()) {
        Assert.fail("numberObject single 405766405417980707E+316\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("405766405417980707E+316").ToSingle());
      }
      if (Double.POSITIVE_INFINITY != ExtendedDecimal.FromString("405766405417980707E+316").ToDouble()) {
        Assert.fail("numberObject double 405766405417980707E+316\nExpected: Double.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("405766405417980707E+316").ToDouble());
      }
      if (-166174.94f != ExtendedDecimal.FromString("-1661749343992047E-10").ToSingle()) {
        Assert.fail("numberObject single -1661749343992047E-10\nExpected: -166174.94f\nWas: " + ExtendedDecimal.FromString("-1661749343992047E-10").ToSingle());
      }
      if (-166174.9343992047d != ExtendedDecimal.FromString("-1661749343992047E-10").ToDouble()) {
        Assert.fail("numberObject double -1661749343992047E-10\nExpected: -166174.9343992047d\nWas: " + ExtendedDecimal.FromString("-1661749343992047E-10").ToDouble());
      }
      if (5893094.0f != ExtendedDecimal.FromString("5893094.099969899224047667").ToSingle()) {
        Assert.fail("numberObject single 5893094.099969899224047667\nExpected: 5893094.0f\nWas: " + ExtendedDecimal.FromString("5893094.099969899224047667").ToSingle());
      }
      if (5893094.099969899d != ExtendedDecimal.FromString("5893094.099969899224047667").ToDouble()) {
        Assert.fail("numberObject double 5893094.099969899224047667\nExpected: 5893094.099969899d\nWas: " + ExtendedDecimal.FromString("5893094.099969899224047667").ToDouble());
      }
      if (-3.4023195E17f != ExtendedDecimal.FromString("-340231946762317122").ToSingle()) {
        Assert.fail("numberObject single -340231946762317122\nExpected: -3.4023195E17f\nWas: " + ExtendedDecimal.FromString("-340231946762317122").ToSingle());
      }
      if (-3.4023194676231712E17d != ExtendedDecimal.FromString("-340231946762317122").ToDouble()) {
        Assert.fail("numberObject double -340231946762317122\nExpected: -3.4023194676231712E17d\nWas: " + ExtendedDecimal.FromString("-340231946762317122").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("3.10041643978E+236").ToSingle()) {
        Assert.fail("numberObject single 3.10041643978E+236\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("3.10041643978E+236").ToSingle());
      }
      if (3.10041643978E236d != ExtendedDecimal.FromString("3.10041643978E+236").ToDouble()) {
        Assert.fail("numberObject double 3.10041643978E+236\nExpected: 3.10041643978E236d\nWas: " + ExtendedDecimal.FromString("3.10041643978E+236").ToDouble());
      }
      if (1.43429217E13f != ExtendedDecimal.FromString("14342921940186").ToSingle()) {
        Assert.fail("numberObject single 14342921940186\nExpected: 1.43429217E13f\nWas: " + ExtendedDecimal.FromString("14342921940186").ToSingle());
      }
      if (1.4342921940186E13d != ExtendedDecimal.FromString("14342921940186").ToDouble()) {
        Assert.fail("numberObject double 14342921940186\nExpected: 1.4342921940186E13d\nWas: " + ExtendedDecimal.FromString("14342921940186").ToDouble());
      }
      if (1.97766234E9f != ExtendedDecimal.FromString("1977662368").ToSingle()) {
        Assert.fail("numberObject single 1977662368\nExpected: 1.97766234E9f\nWas: " + ExtendedDecimal.FromString("1977662368").ToSingle());
      }
      if (1.977662368E9d != ExtendedDecimal.FromString("1977662368").ToDouble()) {
        Assert.fail("numberObject double 1977662368\nExpected: 1.977662368E9d\nWas: " + ExtendedDecimal.FromString("1977662368").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("891.32009975058011674E-268").ToSingle()) {
        Assert.fail("numberObject single 891.32009975058011674E-268\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("891.32009975058011674E-268").ToSingle());
      }
      if (8.913200997505801E-266d != ExtendedDecimal.FromString("891.32009975058011674E-268").ToDouble()) {
        Assert.fail("numberObject double 891.32009975058011674E-268\nExpected: 8.913200997505801E-266d\nWas: " + ExtendedDecimal.FromString("891.32009975058011674E-268").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-895468936291.471679344983419E+316").ToSingle()) {
        Assert.fail("numberObject single -895468936291.471679344983419E+316\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-895468936291.471679344983419E+316").ToSingle());
      }
      if (Double.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-895468936291.471679344983419E+316").ToDouble()) {
        Assert.fail("numberObject double -895468936291.471679344983419E+316\nExpected: Double.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-895468936291.471679344983419E+316").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("61308E-104").ToSingle()) {
        Assert.fail("numberObject single 61308E-104\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("61308E-104").ToSingle());
      }
      if (6.1308E-100d != ExtendedDecimal.FromString("61308E-104").ToDouble()) {
        Assert.fail("numberObject double 61308E-104\nExpected: 6.1308E-100d\nWas: " + ExtendedDecimal.FromString("61308E-104").ToDouble());
      }
      if (-5362.791f != ExtendedDecimal.FromString("-5362.79122778669072").ToSingle()) {
        Assert.fail("numberObject single -5362.79122778669072\nExpected: -5362.791f\nWas: " + ExtendedDecimal.FromString("-5362.79122778669072").ToSingle());
      }
      if (-5362.791227786691d != ExtendedDecimal.FromString("-5362.79122778669072").ToDouble()) {
        Assert.fail("numberObject double -5362.79122778669072\nExpected: -5362.791227786691d\nWas: " + ExtendedDecimal.FromString("-5362.79122778669072").ToDouble());
      }
      if (0.0f != ExtendedDecimal.FromString("861664379590901308.23330613776542261919E-101").ToSingle()) {
        Assert.fail("numberObject single 861664379590901308.23330613776542261919E-101\nExpected: 0.0f\nWas: " + ExtendedDecimal.FromString("861664379590901308.23330613776542261919E-101").ToSingle());
      }
      if (8.616643795909013E-84d != ExtendedDecimal.FromString("861664379590901308.23330613776542261919E-101").ToDouble()) {
        Assert.fail("numberObject double 861664379590901308.23330613776542261919E-101\nExpected: 8.616643795909013E-84d\nWas: " + ExtendedDecimal.FromString("861664379590901308.23330613776542261919E-101").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-1884773180.50192918329237967651E+204").ToSingle()) {
        Assert.fail("numberObject single -1884773180.50192918329237967651E+204\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-1884773180.50192918329237967651E+204").ToSingle());
      }
      if (-1.884773180501929E213d != ExtendedDecimal.FromString("-1884773180.50192918329237967651E+204").ToDouble()) {
        Assert.fail("numberObject double -1884773180.50192918329237967651E+204\nExpected: -1.884773180501929E213d\nWas: " + ExtendedDecimal.FromString("-1884773180.50192918329237967651E+204").ToDouble());
      }
      if (1.89187207E13f != ExtendedDecimal.FromString("18918720095123.6152").ToSingle()) {
        Assert.fail("numberObject single 18918720095123.6152\nExpected: 1.89187207E13f\nWas: " + ExtendedDecimal.FromString("18918720095123.6152").ToSingle());
      }
      if (1.8918720095123613E13d != ExtendedDecimal.FromString("18918720095123.6152").ToDouble()) {
        Assert.fail("numberObject double 18918720095123.6152\nExpected: 1.8918720095123613E13d\nWas: " + ExtendedDecimal.FromString("18918720095123.6152").ToDouble());
      }
      if (94667.95f != ExtendedDecimal.FromString("94667.95264211741602").ToSingle()) {
        Assert.fail("numberObject single 94667.95264211741602\nExpected: 94667.95f\nWas: " + ExtendedDecimal.FromString("94667.95264211741602").ToSingle());
      }
      if (94667.95264211742d != ExtendedDecimal.FromString("94667.95264211741602").ToDouble()) {
        Assert.fail("numberObject double 94667.95264211741602\nExpected: 94667.95264211742d\nWas: " + ExtendedDecimal.FromString("94667.95264211741602").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("1230618521424E+134").ToSingle()) {
        Assert.fail("numberObject single 1230618521424E+134\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("1230618521424E+134").ToSingle());
      }
      if (1.230618521424E146d != ExtendedDecimal.FromString("1230618521424E+134").ToDouble()) {
        Assert.fail("numberObject double 1230618521424E+134\nExpected: 1.230618521424E146d\nWas: " + ExtendedDecimal.FromString("1230618521424E+134").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("3022403935588782E+85").ToSingle()) {
        Assert.fail("numberObject single 3022403935588782E+85\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("3022403935588782E+85").ToSingle());
      }
      if (3.022403935588782E100d != ExtendedDecimal.FromString("3022403935588782E+85").ToDouble()) {
        Assert.fail("numberObject double 3022403935588782E+85\nExpected: 3.022403935588782E100d\nWas: " + ExtendedDecimal.FromString("3022403935588782E+85").ToDouble());
      }
      if (Float.POSITIVE_INFINITY != ExtendedDecimal.FromString("64543E+274").ToSingle()) {
        Assert.fail("numberObject single 64543E+274\nExpected: Float.POSITIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("64543E+274").ToSingle());
      }
      if (6.4543E278d != ExtendedDecimal.FromString("64543E+274").ToDouble()) {
        Assert.fail("numberObject double 64543E+274\nExpected: 6.4543E278d\nWas: " + ExtendedDecimal.FromString("64543E+274").ToDouble());
      }
      if (6.7181355E10f != ExtendedDecimal.FromString("67181356837.903551518080873954").ToSingle()) {
        Assert.fail("numberObject single 67181356837.903551518080873954\nExpected: 6.7181355E10f\nWas: " + ExtendedDecimal.FromString("67181356837.903551518080873954").ToSingle());
      }
      if (6.718135683790355E10d != ExtendedDecimal.FromString("67181356837.903551518080873954").ToDouble()) {
        Assert.fail("numberObject double 67181356837.903551518080873954\nExpected: 6.718135683790355E10d\nWas: " + ExtendedDecimal.FromString("67181356837.903551518080873954").ToDouble());
      }
      if (-0.0f != ExtendedDecimal.FromString("-4508016E-321").ToSingle()) {
        Assert.fail("numberObject single -4508016E-321\nExpected: -0.0f\nWas: " + ExtendedDecimal.FromString("-4508016E-321").ToSingle());
      }
      if (-4.508016E-315d != ExtendedDecimal.FromString("-4508016E-321").ToDouble()) {
        Assert.fail("numberObject double -4508016E-321\nExpected: -4.508016E-315d\nWas: " + ExtendedDecimal.FromString("-4508016E-321").ToDouble());
      }
      if (Float.NEGATIVE_INFINITY != ExtendedDecimal.FromString("-62855032520.512452348497E+39").ToSingle()) {
        Assert.fail("numberObject single -62855032520.512452348497E+39\nExpected: Float.NEGATIVE_INFINITY\nWas: " + ExtendedDecimal.FromString("-62855032520.512452348497E+39").ToSingle());
      }
      if (-6.285503252051245E49d != ExtendedDecimal.FromString("-62855032520.512452348497E+39").ToDouble()) {
        Assert.fail("numberObject double -62855032520.512452348497E+39\nExpected: -6.285503252051245E49d\nWas: " + ExtendedDecimal.FromString("-62855032520.512452348497E+39").ToDouble());
      }
      if (3177.2236f != ExtendedDecimal.FromString("3177.2237286").ToSingle()) {
        Assert.fail("numberObject single 3177.2237286\nExpected: 3177.2236f\nWas: " + ExtendedDecimal.FromString("3177.2237286").ToSingle());
      }
      if (3177.2237286d != ExtendedDecimal.FromString("3177.2237286").ToDouble()) {
        Assert.fail("numberObject double 3177.2237286\nExpected: 3177.2237286d\nWas: " + ExtendedDecimal.FromString("3177.2237286").ToDouble());
      }
      if (-7.950583E8f != ExtendedDecimal.FromString("-795058316.9186492185346968").ToSingle()) {
        Assert.fail("numberObject single -795058316.9186492185346968\nExpected: -7.950583E8f\nWas: " + ExtendedDecimal.FromString("-795058316.9186492185346968").ToSingle());
      }
      if (-7.950583169186492E8d != ExtendedDecimal.FromString("-795058316.9186492185346968").ToDouble()) {
        Assert.fail("numberObject double -795058316.9186492185346968\nExpected: -7.950583169186492E8d\nWas: " + ExtendedDecimal.FromString("-795058316.9186492185346968").ToDouble());
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestDecFracIntegersToSingleDouble() {
      if (-5.7703064E7f != ExtendedDecimal.FromString("-57703066").ToSingle()) {
        Assert.fail("numberObject single -57703066\nExpected: -5.7703064E7f\nWas: " + ExtendedDecimal.FromString("-57703066").ToSingle());
      }
      if (-5.7703066E7d != ExtendedDecimal.FromString("-57703066").ToDouble()) {
        Assert.fail("numberObject double -57703066\nExpected: -5.7703066E7d\nWas: " + ExtendedDecimal.FromString("-57703066").ToDouble());
      }
      if (1590432.0f != ExtendedDecimal.FromString("1590432").ToSingle()) {
        Assert.fail("numberObject single 1590432\nExpected: 1590432.0f\nWas: " + ExtendedDecimal.FromString("1590432").ToSingle());
      }
      if (1590432.0d != ExtendedDecimal.FromString("1590432").ToDouble()) {
        Assert.fail("numberObject double 1590432\nExpected: 1590432.0d\nWas: " + ExtendedDecimal.FromString("1590432").ToDouble());
      }
      if (9.5464253E9f != ExtendedDecimal.FromString("9546425267").ToSingle()) {
        Assert.fail("numberObject single 9546425267\nExpected: 9.5464253E9f\nWas: " + ExtendedDecimal.FromString("9546425267").ToSingle());
      }
      if (9.546425267E9d != ExtendedDecimal.FromString("9546425267").ToDouble()) {
        Assert.fail("numberObject double 9546425267\nExpected: 9.546425267E9d\nWas: " + ExtendedDecimal.FromString("9546425267").ToDouble());
      }
      if (7.3227311E16f != ExtendedDecimal.FromString("73227309698439247").ToSingle()) {
        Assert.fail("numberObject single 73227309698439247\nExpected: 7.3227311E16f\nWas: " + ExtendedDecimal.FromString("73227309698439247").ToSingle());
      }
      if (7.3227309698439248E16d != ExtendedDecimal.FromString("73227309698439247").ToDouble()) {
        Assert.fail("numberObject double 73227309698439247\nExpected: 7.3227309698439248E16d\nWas: " + ExtendedDecimal.FromString("73227309698439247").ToDouble());
      }
      if (75114.0f != ExtendedDecimal.FromString("75114").ToSingle()) {
        Assert.fail("numberObject single 75114\nExpected: 75114.0f\nWas: " + ExtendedDecimal.FromString("75114").ToSingle());
      }
      if (75114.0d != ExtendedDecimal.FromString("75114").ToDouble()) {
        Assert.fail("numberObject double 75114\nExpected: 75114.0d\nWas: " + ExtendedDecimal.FromString("75114").ToDouble());
      }
      if (64.0f != ExtendedDecimal.FromString("64").ToSingle()) {
        Assert.fail("numberObject single 64\nExpected: 64.0f\nWas: " + ExtendedDecimal.FromString("64").ToSingle());
      }
      if (64.0d != ExtendedDecimal.FromString("64").ToDouble()) {
        Assert.fail("numberObject double 64\nExpected: 64.0d\nWas: " + ExtendedDecimal.FromString("64").ToDouble());
      }
      if (8.6352293E15f != ExtendedDecimal.FromString("8635229353951207").ToSingle()) {
        Assert.fail("numberObject single 8635229353951207\nExpected: 8.6352293E15f\nWas: " + ExtendedDecimal.FromString("8635229353951207").ToSingle());
      }
      if (8.635229353951207E15d != ExtendedDecimal.FromString("8635229353951207").ToDouble()) {
        Assert.fail("numberObject double 8635229353951207\nExpected: 8.635229353951207E15d\nWas: " + ExtendedDecimal.FromString("8635229353951207").ToDouble());
      }
      if (-8.056573E19f != ExtendedDecimal.FromString("-80565729661447979457").ToSingle()) {
        Assert.fail("numberObject single -80565729661447979457\nExpected: -8.056573E19f\nWas: " + ExtendedDecimal.FromString("-80565729661447979457").ToSingle());
      }
      if (-8.056572966144798E19d != ExtendedDecimal.FromString("-80565729661447979457").ToDouble()) {
        Assert.fail("numberObject double -80565729661447979457\nExpected: -8.056572966144798E19d\nWas: " + ExtendedDecimal.FromString("-80565729661447979457").ToDouble());
      }
      if (8.1540558E14f != ExtendedDecimal.FromString("815405565228754").ToSingle()) {
        Assert.fail("numberObject single 815405565228754\nExpected: 8.1540558E14f\nWas: " + ExtendedDecimal.FromString("815405565228754").ToSingle());
      }
      if (8.15405565228754E14d != ExtendedDecimal.FromString("815405565228754").ToDouble()) {
        Assert.fail("numberObject double 815405565228754\nExpected: 8.15405565228754E14d\nWas: " + ExtendedDecimal.FromString("815405565228754").ToDouble());
      }
      if (-6.1008438E16f != ExtendedDecimal.FromString("-61008438357089231").ToSingle()) {
        Assert.fail("numberObject single -61008438357089231\nExpected: -6.1008438E16f\nWas: " + ExtendedDecimal.FromString("-61008438357089231").ToSingle());
      }
      if (-6.1008438357089232E16d != ExtendedDecimal.FromString("-61008438357089231").ToDouble()) {
        Assert.fail("numberObject double -61008438357089231\nExpected: -6.1008438357089232E16d\nWas: " + ExtendedDecimal.FromString("-61008438357089231").ToDouble());
      }
      if (-46526.0f != ExtendedDecimal.FromString("-46526").ToSingle()) {
        Assert.fail("numberObject single -46526\nExpected: -46526.0f\nWas: " + ExtendedDecimal.FromString("-46526").ToSingle());
      }
      if (-46526.0d != ExtendedDecimal.FromString("-46526").ToDouble()) {
        Assert.fail("numberObject double -46526\nExpected: -46526.0d\nWas: " + ExtendedDecimal.FromString("-46526").ToDouble());
      }
      if (5.1199847E18f != ExtendedDecimal.FromString("5119984668352258853").ToSingle()) {
        Assert.fail("numberObject single 5119984668352258853\nExpected: 5.1199847E18f\nWas: " + ExtendedDecimal.FromString("5119984668352258853").ToSingle());
      }
      if (5.1199846683522591E18d != ExtendedDecimal.FromString("5119984668352258853").ToDouble()) {
        Assert.fail("numberObject double 5119984668352258853\nExpected: 5.1199846683522591E18d\nWas: " + ExtendedDecimal.FromString("5119984668352258853").ToDouble());
      }
      if (1851.0f != ExtendedDecimal.FromString("1851").ToSingle()) {
        Assert.fail("numberObject single 1851\nExpected: 1851.0f\nWas: " + ExtendedDecimal.FromString("1851").ToSingle());
      }
      if (1851.0d != ExtendedDecimal.FromString("1851").ToDouble()) {
        Assert.fail("numberObject double 1851\nExpected: 1851.0d\nWas: " + ExtendedDecimal.FromString("1851").ToDouble());
      }
      if (8.7587332E15f != ExtendedDecimal.FromString("8758733009763848").ToSingle()) {
        Assert.fail("numberObject single 8758733009763848\nExpected: 8.7587332E15f\nWas: " + ExtendedDecimal.FromString("8758733009763848").ToSingle());
      }
      if (8.758733009763848E15d != ExtendedDecimal.FromString("8758733009763848").ToDouble()) {
        Assert.fail("numberObject double 8758733009763848\nExpected: 8.758733009763848E15d\nWas: " + ExtendedDecimal.FromString("8758733009763848").ToDouble());
      }
      if (51.0f != ExtendedDecimal.FromString("51").ToSingle()) {
        Assert.fail("numberObject single 51\nExpected: 51.0f\nWas: " + ExtendedDecimal.FromString("51").ToSingle());
      }
      if (51.0d != ExtendedDecimal.FromString("51").ToDouble()) {
        Assert.fail("numberObject double 51\nExpected: 51.0d\nWas: " + ExtendedDecimal.FromString("51").ToDouble());
      }
      if (9.4281774E11f != ExtendedDecimal.FromString("942817726107").ToSingle()) {
        Assert.fail("numberObject single 942817726107\nExpected: 9.4281774E11f\nWas: " + ExtendedDecimal.FromString("942817726107").ToSingle());
      }
      if (9.42817726107E11d != ExtendedDecimal.FromString("942817726107").ToDouble()) {
        Assert.fail("numberObject double 942817726107\nExpected: 9.42817726107E11d\nWas: " + ExtendedDecimal.FromString("942817726107").ToDouble());
      }
      if (186575.0f != ExtendedDecimal.FromString("186575").ToSingle()) {
        Assert.fail("numberObject single 186575\nExpected: 186575.0f\nWas: " + ExtendedDecimal.FromString("186575").ToSingle());
      }
      if (186575.0d != ExtendedDecimal.FromString("186575").ToDouble()) {
        Assert.fail("numberObject double 186575\nExpected: 186575.0d\nWas: " + ExtendedDecimal.FromString("186575").ToDouble());
      }
      if (-3.47313997E9f != ExtendedDecimal.FromString("-3473140020").ToSingle()) {
        Assert.fail("numberObject single -3473140020\nExpected: -3.47313997E9f\nWas: " + ExtendedDecimal.FromString("-3473140020").ToSingle());
      }
      if (-3.47314002E9d != ExtendedDecimal.FromString("-3473140020").ToDouble()) {
        Assert.fail("numberObject double -3473140020\nExpected: -3.47314002E9d\nWas: " + ExtendedDecimal.FromString("-3473140020").ToDouble());
      }
      if (2.66134912E8f != ExtendedDecimal.FromString("266134912").ToSingle()) {
        Assert.fail("numberObject single 266134912\nExpected: 2.66134912E8f\nWas: " + ExtendedDecimal.FromString("266134912").ToSingle());
      }
      if (2.66134912E8d != ExtendedDecimal.FromString("266134912").ToDouble()) {
        Assert.fail("numberObject double 266134912\nExpected: 2.66134912E8d\nWas: " + ExtendedDecimal.FromString("266134912").ToDouble());
      }
      if (5209.0f != ExtendedDecimal.FromString("5209").ToSingle()) {
        Assert.fail("numberObject single 5209\nExpected: 5209.0f\nWas: " + ExtendedDecimal.FromString("5209").ToSingle());
      }
      if (5209.0d != ExtendedDecimal.FromString("5209").ToDouble()) {
        Assert.fail("numberObject double 5209\nExpected: 5209.0d\nWas: " + ExtendedDecimal.FromString("5209").ToDouble());
      }
      if (70489.0f != ExtendedDecimal.FromString("70489").ToSingle()) {
        Assert.fail("numberObject single 70489\nExpected: 70489.0f\nWas: " + ExtendedDecimal.FromString("70489").ToSingle());
      }
      if (70489.0d != ExtendedDecimal.FromString("70489").ToDouble()) {
        Assert.fail("numberObject double 70489\nExpected: 70489.0d\nWas: " + ExtendedDecimal.FromString("70489").ToDouble());
      }
      if (-6.1383652E14f != ExtendedDecimal.FromString("-613836517344428").ToSingle()) {
        Assert.fail("numberObject single -613836517344428\nExpected: -6.1383652E14f\nWas: " + ExtendedDecimal.FromString("-613836517344428").ToSingle());
      }
      if (-6.13836517344428E14d != ExtendedDecimal.FromString("-613836517344428").ToDouble()) {
        Assert.fail("numberObject double -613836517344428\nExpected: -6.13836517344428E14d\nWas: " + ExtendedDecimal.FromString("-613836517344428").ToDouble());
      }
      if (-3.47896388E16f != ExtendedDecimal.FromString("-34789639317051875E0").ToSingle()) {
        Assert.fail("numberObject single -34789639317051875E0\nExpected: -3.47896388E16f\nWas: " + ExtendedDecimal.FromString("-34789639317051875E0").ToSingle());
      }
      if (-3.4789639317051876E16d != ExtendedDecimal.FromString("-34789639317051875E0").ToDouble()) {
        Assert.fail("numberObject double -34789639317051875E0\nExpected: -3.4789639317051876E16d\nWas: " + ExtendedDecimal.FromString("-34789639317051875E0").ToDouble());
      }
      if (-8.4833942E13f != ExtendedDecimal.FromString("-84833938642058").ToSingle()) {
        Assert.fail("numberObject single -84833938642058\nExpected: -8.4833942E13f\nWas: " + ExtendedDecimal.FromString("-84833938642058").ToSingle());
      }
      if (-8.4833938642058E13d != ExtendedDecimal.FromString("-84833938642058").ToDouble()) {
        Assert.fail("numberObject double -84833938642058\nExpected: -8.4833938642058E13d\nWas: " + ExtendedDecimal.FromString("-84833938642058").ToDouble());
      }
      if (-359.0f != ExtendedDecimal.FromString("-359").ToSingle()) {
        Assert.fail("numberObject single -359\nExpected: -359.0f\nWas: " + ExtendedDecimal.FromString("-359").ToSingle());
      }
      if (-359.0d != ExtendedDecimal.FromString("-359").ToDouble()) {
        Assert.fail("numberObject double -359\nExpected: -359.0d\nWas: " + ExtendedDecimal.FromString("-359").ToDouble());
      }
      if (365981.0f != ExtendedDecimal.FromString("365981").ToSingle()) {
        Assert.fail("numberObject single 365981\nExpected: 365981.0f\nWas: " + ExtendedDecimal.FromString("365981").ToSingle());
      }
      if (365981.0d != ExtendedDecimal.FromString("365981").ToDouble()) {
        Assert.fail("numberObject double 365981\nExpected: 365981.0d\nWas: " + ExtendedDecimal.FromString("365981").ToDouble());
      }
      if (9103.0f != ExtendedDecimal.FromString("9103").ToSingle()) {
        Assert.fail("numberObject single 9103\nExpected: 9103.0f\nWas: " + ExtendedDecimal.FromString("9103").ToSingle());
      }
      if (9103.0d != ExtendedDecimal.FromString("9103").ToDouble()) {
        Assert.fail("numberObject double 9103\nExpected: 9103.0d\nWas: " + ExtendedDecimal.FromString("9103").ToDouble());
      }
      if (9.822906E11f != ExtendedDecimal.FromString("982290625898").ToSingle()) {
        Assert.fail("numberObject single 982290625898\nExpected: 9.822906E11f\nWas: " + ExtendedDecimal.FromString("982290625898").ToSingle());
      }
      if (9.82290625898E11d != ExtendedDecimal.FromString("982290625898").ToDouble()) {
        Assert.fail("numberObject double 982290625898\nExpected: 9.82290625898E11d\nWas: " + ExtendedDecimal.FromString("982290625898").ToDouble());
      }
      if (11.0f != ExtendedDecimal.FromString("11").ToSingle()) {
        Assert.fail("numberObject single 11\nExpected: 11.0f\nWas: " + ExtendedDecimal.FromString("11").ToSingle());
      }
      if (11.0d != ExtendedDecimal.FromString("11").ToDouble()) {
        Assert.fail("numberObject double 11\nExpected: 11.0d\nWas: " + ExtendedDecimal.FromString("11").ToDouble());
      }
      if (-2823.0f != ExtendedDecimal.FromString("-2823").ToSingle()) {
        Assert.fail("numberObject single -2823\nExpected: -2823.0f\nWas: " + ExtendedDecimal.FromString("-2823").ToSingle());
      }
      if (-2823.0d != ExtendedDecimal.FromString("-2823").ToDouble()) {
        Assert.fail("numberObject double -2823\nExpected: -2823.0d\nWas: " + ExtendedDecimal.FromString("-2823").ToDouble());
      }
      if (1.5945044E10f != ExtendedDecimal.FromString("15945044029").ToSingle()) {
        Assert.fail("numberObject single 15945044029\nExpected: 1.5945044E10f\nWas: " + ExtendedDecimal.FromString("15945044029").ToSingle());
      }
      if (1.5945044029E10d != ExtendedDecimal.FromString("15945044029").ToDouble()) {
        Assert.fail("numberObject double 15945044029\nExpected: 1.5945044029E10d\nWas: " + ExtendedDecimal.FromString("15945044029").ToDouble());
      }
      if (-1.69193578E18f != ExtendedDecimal.FromString("-1691935711084975329").ToSingle()) {
        Assert.fail("numberObject single -1691935711084975329\nExpected: -1.69193578E18f\nWas: " + ExtendedDecimal.FromString("-1691935711084975329").ToSingle());
      }
      if (-1.69193571108497536E18d != ExtendedDecimal.FromString("-1691935711084975329").ToDouble()) {
        Assert.fail("numberObject double -1691935711084975329\nExpected: -1.69193571108497536E18d\nWas: " + ExtendedDecimal.FromString("-1691935711084975329").ToDouble());
      }
      if (611.0f != ExtendedDecimal.FromString("611").ToSingle()) {
        Assert.fail("numberObject single 611\nExpected: 611.0f\nWas: " + ExtendedDecimal.FromString("611").ToSingle());
      }
      if (611.0d != ExtendedDecimal.FromString("611").ToDouble()) {
        Assert.fail("numberObject double 611\nExpected: 611.0d\nWas: " + ExtendedDecimal.FromString("611").ToDouble());
      }
      if (8.1338793E9f != ExtendedDecimal.FromString("8133879260").ToSingle()) {
        Assert.fail("numberObject single 8133879260\nExpected: 8.1338793E9f\nWas: " + ExtendedDecimal.FromString("8133879260").ToSingle());
      }
      if (8.13387926E9d != ExtendedDecimal.FromString("8133879260").ToDouble()) {
        Assert.fail("numberObject double 8133879260\nExpected: 8.13387926E9d\nWas: " + ExtendedDecimal.FromString("8133879260").ToDouble());
      }
      if (7.8632614E13f != ExtendedDecimal.FromString("78632613962905").ToSingle()) {
        Assert.fail("numberObject single 78632613962905\nExpected: 7.8632614E13f\nWas: " + ExtendedDecimal.FromString("78632613962905").ToSingle());
      }
      if (7.8632613962905E13d != ExtendedDecimal.FromString("78632613962905").ToDouble()) {
        Assert.fail("numberObject double 78632613962905\nExpected: 7.8632613962905E13d\nWas: " + ExtendedDecimal.FromString("78632613962905").ToDouble());
      }
      if (8.686342E19f != ExtendedDecimal.FromString("86863421212032782386").ToSingle()) {
        Assert.fail("numberObject single 86863421212032782386\nExpected: 8.686342E19f\nWas: " + ExtendedDecimal.FromString("86863421212032782386").ToSingle());
      }
      if (8.686342121203278E19d != ExtendedDecimal.FromString("86863421212032782386").ToDouble()) {
        Assert.fail("numberObject double 86863421212032782386\nExpected: 8.686342121203278E19d\nWas: " + ExtendedDecimal.FromString("86863421212032782386").ToDouble());
      }
      if (2.46595376E8f != ExtendedDecimal.FromString("246595381").ToSingle()) {
        Assert.fail("numberObject single 246595381\nExpected: 2.46595376E8f\nWas: " + ExtendedDecimal.FromString("246595381").ToSingle());
      }
      if (2.46595381E8d != ExtendedDecimal.FromString("246595381").ToDouble()) {
        Assert.fail("numberObject double 246595381\nExpected: 2.46595381E8d\nWas: " + ExtendedDecimal.FromString("246595381").ToDouble());
      }
      if (5.128928E16f != ExtendedDecimal.FromString("51289277641921518E0").ToSingle()) {
        Assert.fail("numberObject single 51289277641921518E0\nExpected: 5.128928E16f\nWas: " + ExtendedDecimal.FromString("51289277641921518E0").ToSingle());
      }
      if (5.128927764192152E16d != ExtendedDecimal.FromString("51289277641921518E0").ToDouble()) {
        Assert.fail("numberObject double 51289277641921518E0\nExpected: 5.128927764192152E16d\nWas: " + ExtendedDecimal.FromString("51289277641921518E0").ToDouble());
      }
      if (41105.0f != ExtendedDecimal.FromString("41105").ToSingle()) {
        Assert.fail("numberObject single 41105\nExpected: 41105.0f\nWas: " + ExtendedDecimal.FromString("41105").ToSingle());
      }
      if (41105.0d != ExtendedDecimal.FromString("41105").ToDouble()) {
        Assert.fail("numberObject double 41105\nExpected: 41105.0d\nWas: " + ExtendedDecimal.FromString("41105").ToDouble());
      }
      if (4.5854699E16f != ExtendedDecimal.FromString("45854697039925162E0").ToSingle()) {
        Assert.fail("numberObject single 45854697039925162E0\nExpected: 4.5854699E16f\nWas: " + ExtendedDecimal.FromString("45854697039925162E0").ToSingle());
      }
      if (4.585469703992516E16d != ExtendedDecimal.FromString("45854697039925162E0").ToDouble()) {
        Assert.fail("numberObject double 45854697039925162E0\nExpected: 4.585469703992516E16d\nWas: " + ExtendedDecimal.FromString("45854697039925162E0").ToDouble());
      }
      if (357.0f != ExtendedDecimal.FromString("357").ToSingle()) {
        Assert.fail("numberObject single 357\nExpected: 357.0f\nWas: " + ExtendedDecimal.FromString("357").ToSingle());
      }
      if (357.0d != ExtendedDecimal.FromString("357").ToDouble()) {
        Assert.fail("numberObject double 357\nExpected: 357.0d\nWas: " + ExtendedDecimal.FromString("357").ToDouble());
      }
      if (4055.0f != ExtendedDecimal.FromString("4055").ToSingle()) {
        Assert.fail("numberObject single 4055\nExpected: 4055.0f\nWas: " + ExtendedDecimal.FromString("4055").ToSingle());
      }
      if (4055.0d != ExtendedDecimal.FromString("4055").ToDouble()) {
        Assert.fail("numberObject double 4055\nExpected: 4055.0d\nWas: " + ExtendedDecimal.FromString("4055").ToDouble());
      }
      if (-75211.0f != ExtendedDecimal.FromString("-75211").ToSingle()) {
        Assert.fail("numberObject single -75211\nExpected: -75211.0f\nWas: " + ExtendedDecimal.FromString("-75211").ToSingle());
      }
      if (-75211.0d != ExtendedDecimal.FromString("-75211").ToDouble()) {
        Assert.fail("numberObject double -75211\nExpected: -75211.0d\nWas: " + ExtendedDecimal.FromString("-75211").ToDouble());
      }
      if (-8.718763E19f != ExtendedDecimal.FromString("-87187631416675804676").ToSingle()) {
        Assert.fail("numberObject single -87187631416675804676\nExpected: -8.718763E19f\nWas: " + ExtendedDecimal.FromString("-87187631416675804676").ToSingle());
      }
      if (-8.718763141667581E19d != ExtendedDecimal.FromString("-87187631416675804676").ToDouble()) {
        Assert.fail("numberObject double -87187631416675804676\nExpected: -8.718763141667581E19d\nWas: " + ExtendedDecimal.FromString("-87187631416675804676").ToDouble());
      }
      if (-5.6423271E13f != ExtendedDecimal.FromString("-56423269820314").ToSingle()) {
        Assert.fail("numberObject single -56423269820314\nExpected: -5.6423271E13f\nWas: " + ExtendedDecimal.FromString("-56423269820314").ToSingle());
      }
      if (-5.6423269820314E13d != ExtendedDecimal.FromString("-56423269820314").ToDouble()) {
        Assert.fail("numberObject double -56423269820314\nExpected: -5.6423269820314E13d\nWas: " + ExtendedDecimal.FromString("-56423269820314").ToDouble());
      }
      if (-884958.0f != ExtendedDecimal.FromString("-884958").ToSingle()) {
        Assert.fail("numberObject single -884958\nExpected: -884958.0f\nWas: " + ExtendedDecimal.FromString("-884958").ToSingle());
      }
      if (-884958.0d != ExtendedDecimal.FromString("-884958").ToDouble()) {
        Assert.fail("numberObject double -884958\nExpected: -884958.0d\nWas: " + ExtendedDecimal.FromString("-884958").ToDouble());
      }
      if (-9.5231607E11f != ExtendedDecimal.FromString("-952316071356").ToSingle()) {
        Assert.fail("numberObject single -952316071356\nExpected: -9.5231607E11f\nWas: " + ExtendedDecimal.FromString("-952316071356").ToSingle());
      }
      if (-9.52316071356E11d != ExtendedDecimal.FromString("-952316071356").ToDouble()) {
        Assert.fail("numberObject double -952316071356\nExpected: -9.52316071356E11d\nWas: " + ExtendedDecimal.FromString("-952316071356").ToDouble());
      }
      if (1.07800844E17f != ExtendedDecimal.FromString("107800846902684870").ToSingle()) {
        Assert.fail("numberObject single 107800846902684870\nExpected: 1.07800844E17f\nWas: " + ExtendedDecimal.FromString("107800846902684870").ToSingle());
      }
      if (1.07800846902684864E17d != ExtendedDecimal.FromString("107800846902684870").ToDouble()) {
        Assert.fail("numberObject double 107800846902684870\nExpected: 1.07800846902684864E17d\nWas: " + ExtendedDecimal.FromString("107800846902684870").ToDouble());
      }
      if (-8.1588551E18f != ExtendedDecimal.FromString("-8158855313340166027").ToSingle()) {
        Assert.fail("numberObject single -8158855313340166027\nExpected: -8.1588551E18f\nWas: " + ExtendedDecimal.FromString("-8158855313340166027").ToSingle());
      }
      if (-8.1588553133401661E18d != ExtendedDecimal.FromString("-8158855313340166027").ToDouble()) {
        Assert.fail("numberObject double -8158855313340166027\nExpected: -8.1588553133401661E18d\nWas: " + ExtendedDecimal.FromString("-8158855313340166027").ToDouble());
      }
      if (1.52743454E18f != ExtendedDecimal.FromString("1527434477600178421").ToSingle()) {
        Assert.fail("numberObject single 1527434477600178421\nExpected: 1.52743454E18f\nWas: " + ExtendedDecimal.FromString("1527434477600178421").ToSingle());
      }
      if (1.52743447760017843E18d != ExtendedDecimal.FromString("1527434477600178421").ToDouble()) {
        Assert.fail("numberObject double 1527434477600178421\nExpected: 1.52743447760017843E18d\nWas: " + ExtendedDecimal.FromString("1527434477600178421").ToDouble());
      }
      if (-1.25374015E15f != ExtendedDecimal.FromString("-1253740164504924").ToSingle()) {
        Assert.fail("numberObject single -1253740164504924\nExpected: -1.25374015E15f\nWas: " + ExtendedDecimal.FromString("-1253740164504924").ToSingle());
      }
      if (-1.253740164504924E15d != ExtendedDecimal.FromString("-1253740164504924").ToDouble()) {
        Assert.fail("numberObject double -1253740164504924\nExpected: -1.253740164504924E15d\nWas: " + ExtendedDecimal.FromString("-1253740164504924").ToDouble());
      }
      if (9.333153E10f != ExtendedDecimal.FromString("93331529453").ToSingle()) {
        Assert.fail("numberObject single 93331529453\nExpected: 9.333153E10f\nWas: " + ExtendedDecimal.FromString("93331529453").ToSingle());
      }
      if (9.3331529453E10d != ExtendedDecimal.FromString("93331529453").ToDouble()) {
        Assert.fail("numberObject double 93331529453\nExpected: 9.3331529453E10d\nWas: " + ExtendedDecimal.FromString("93331529453").ToDouble());
      }
      if (-26195.0f != ExtendedDecimal.FromString("-26195").ToSingle()) {
        Assert.fail("numberObject single -26195\nExpected: -26195.0f\nWas: " + ExtendedDecimal.FromString("-26195").ToSingle());
      }
      if (-26195.0d != ExtendedDecimal.FromString("-26195").ToDouble()) {
        Assert.fail("numberObject double -26195\nExpected: -26195.0d\nWas: " + ExtendedDecimal.FromString("-26195").ToDouble());
      }
      if (-369.0f != ExtendedDecimal.FromString("-369").ToSingle()) {
        Assert.fail("numberObject single -369\nExpected: -369.0f\nWas: " + ExtendedDecimal.FromString("-369").ToSingle());
      }
      if (-369.0d != ExtendedDecimal.FromString("-369").ToDouble()) {
        Assert.fail("numberObject double -369\nExpected: -369.0d\nWas: " + ExtendedDecimal.FromString("-369").ToDouble());
      }
      if (-831.0f != ExtendedDecimal.FromString("-831").ToSingle()) {
        Assert.fail("numberObject single -831\nExpected: -831.0f\nWas: " + ExtendedDecimal.FromString("-831").ToSingle());
      }
      if (-831.0d != ExtendedDecimal.FromString("-831").ToDouble()) {
        Assert.fail("numberObject double -831\nExpected: -831.0d\nWas: " + ExtendedDecimal.FromString("-831").ToDouble());
      }
      if (4.11190218E12f != ExtendedDecimal.FromString("4111902130704").ToSingle()) {
        Assert.fail("numberObject single 4111902130704\nExpected: 4.11190218E12f\nWas: " + ExtendedDecimal.FromString("4111902130704").ToSingle());
      }
      if (4.111902130704E12d != ExtendedDecimal.FromString("4111902130704").ToDouble()) {
        Assert.fail("numberObject double 4111902130704\nExpected: 4.111902130704E12d\nWas: " + ExtendedDecimal.FromString("4111902130704").ToDouble());
      }
      if (-7.419975E34f != ExtendedDecimal.FromString("-7419975014712636689.1578027201500774E+16").ToSingle()) {
        Assert.fail("numberObject single -7419975014712636689.1578027201500774E+16\nExpected: -7.419975E34f\nWas: " + ExtendedDecimal.FromString("-7419975014712636689.1578027201500774E+16").ToSingle());
      }
      if (-7.419975014712636E34d != ExtendedDecimal.FromString("-7419975014712636689.1578027201500774E+16").ToDouble()) {
        Assert.fail("numberObject double -7419975014712636689.1578027201500774E+16\nExpected: -7.419975014712636E34d\nWas: " + ExtendedDecimal.FromString("-7419975014712636689.1578027201500774E+16").ToDouble());
      }
      if (-1.7915818E7f != ExtendedDecimal.FromString("-17915818").ToSingle()) {
        Assert.fail("numberObject single -17915818\nExpected: -1.7915818E7f\nWas: " + ExtendedDecimal.FromString("-17915818").ToSingle());
      }
      if (-1.7915818E7d != ExtendedDecimal.FromString("-17915818").ToDouble()) {
        Assert.fail("numberObject double -17915818\nExpected: -1.7915818E7d\nWas: " + ExtendedDecimal.FromString("-17915818").ToDouble());
      }
      if (-122.0f != ExtendedDecimal.FromString("-122").ToSingle()) {
        Assert.fail("numberObject single -122\nExpected: -122.0f\nWas: " + ExtendedDecimal.FromString("-122").ToSingle());
      }
      if (-122.0d != ExtendedDecimal.FromString("-122").ToDouble()) {
        Assert.fail("numberObject double -122\nExpected: -122.0d\nWas: " + ExtendedDecimal.FromString("-122").ToDouble());
      }
      if (-363975.0f != ExtendedDecimal.FromString("-363975").ToSingle()) {
        Assert.fail("numberObject single -363975\nExpected: -363975.0f\nWas: " + ExtendedDecimal.FromString("-363975").ToSingle());
      }
      if (-363975.0d != ExtendedDecimal.FromString("-363975").ToDouble()) {
        Assert.fail("numberObject double -363975\nExpected: -363975.0d\nWas: " + ExtendedDecimal.FromString("-363975").ToDouble());
      }
      if (3.22466716E12f != ExtendedDecimal.FromString("3224667065103").ToSingle()) {
        Assert.fail("numberObject single 3224667065103\nExpected: 3.22466716E12f\nWas: " + ExtendedDecimal.FromString("3224667065103").ToSingle());
      }
      if (3.224667065103E12d != ExtendedDecimal.FromString("3224667065103").ToDouble()) {
        Assert.fail("numberObject double 3224667065103\nExpected: 3.224667065103E12d\nWas: " + ExtendedDecimal.FromString("3224667065103").ToDouble());
      }
      if (-9.6666224E7f != ExtendedDecimal.FromString("-96666228").ToSingle()) {
        Assert.fail("numberObject single -96666228\nExpected: -9.6666224E7f\nWas: " + ExtendedDecimal.FromString("-96666228").ToSingle());
      }
      if (-9.6666228E7d != ExtendedDecimal.FromString("-96666228").ToDouble()) {
        Assert.fail("numberObject double -96666228\nExpected: -9.6666228E7d\nWas: " + ExtendedDecimal.FromString("-96666228").ToDouble());
      }
      if (-6.3737765E19f != ExtendedDecimal.FromString("-63737764614634686933").ToSingle()) {
        Assert.fail("numberObject single -63737764614634686933\nExpected: -6.3737765E19f\nWas: " + ExtendedDecimal.FromString("-63737764614634686933").ToSingle());
      }
      if (-6.3737764614634684E19d != ExtendedDecimal.FromString("-63737764614634686933").ToDouble()) {
        Assert.fail("numberObject double -63737764614634686933\nExpected: -6.3737764614634684E19d\nWas: " + ExtendedDecimal.FromString("-63737764614634686933").ToDouble());
      }
      if (-45065.0f != ExtendedDecimal.FromString("-45065").ToSingle()) {
        Assert.fail("numberObject single -45065\nExpected: -45065.0f\nWas: " + ExtendedDecimal.FromString("-45065").ToSingle());
      }
      if (-45065.0d != ExtendedDecimal.FromString("-45065").ToDouble()) {
        Assert.fail("numberObject double -45065\nExpected: -45065.0d\nWas: " + ExtendedDecimal.FromString("-45065").ToDouble());
      }
      if (18463.0f != ExtendedDecimal.FromString("18463").ToSingle()) {
        Assert.fail("numberObject single 18463\nExpected: 18463.0f\nWas: " + ExtendedDecimal.FromString("18463").ToSingle());
      }
      if (18463.0d != ExtendedDecimal.FromString("18463").ToDouble()) {
        Assert.fail("numberObject double 18463\nExpected: 18463.0d\nWas: " + ExtendedDecimal.FromString("18463").ToDouble());
      }
      if (-5.2669409E15f != ExtendedDecimal.FromString("-5266940927335870").ToSingle()) {
        Assert.fail("numberObject single -5266940927335870\nExpected: -5.2669409E15f\nWas: " + ExtendedDecimal.FromString("-5266940927335870").ToSingle());
      }
      if (-5.26694092733587E15d != ExtendedDecimal.FromString("-5266940927335870").ToDouble()) {
        Assert.fail("numberObject double -5266940927335870\nExpected: -5.26694092733587E15d\nWas: " + ExtendedDecimal.FromString("-5266940927335870").ToDouble());
      }
      if (-3.61275925E15f != ExtendedDecimal.FromString("-3612759343074710").ToSingle()) {
        Assert.fail("numberObject single -3612759343074710\nExpected: -3.61275925E15f\nWas: " + ExtendedDecimal.FromString("-3612759343074710").ToSingle());
      }
      if (-3.61275934307471E15d != ExtendedDecimal.FromString("-3612759343074710").ToDouble()) {
        Assert.fail("numberObject double -3612759343074710\nExpected: -3.61275934307471E15d\nWas: " + ExtendedDecimal.FromString("-3612759343074710").ToDouble());
      }
      if (-1.49784412E11f != ExtendedDecimal.FromString("-149784410976").ToSingle()) {
        Assert.fail("numberObject single -149784410976\nExpected: -1.49784412E11f\nWas: " + ExtendedDecimal.FromString("-149784410976").ToSingle());
      }
      if (-1.49784410976E11d != ExtendedDecimal.FromString("-149784410976").ToDouble()) {
        Assert.fail("numberObject double -149784410976\nExpected: -1.49784410976E11d\nWas: " + ExtendedDecimal.FromString("-149784410976").ToDouble());
      }
      if (-1.01285276E17f != ExtendedDecimal.FromString("-101285275020696035").ToSingle()) {
        Assert.fail("numberObject single -101285275020696035\nExpected: -1.01285276E17f\nWas: " + ExtendedDecimal.FromString("-101285275020696035").ToSingle());
      }
      if (-1.01285275020696032E17d != ExtendedDecimal.FromString("-101285275020696035").ToDouble()) {
        Assert.fail("numberObject double -101285275020696035\nExpected: -1.01285275020696032E17d\nWas: " + ExtendedDecimal.FromString("-101285275020696035").ToDouble());
      }
      if (-34.0f != ExtendedDecimal.FromString("-34").ToSingle()) {
        Assert.fail("numberObject single -34\nExpected: -34.0f\nWas: " + ExtendedDecimal.FromString("-34").ToSingle());
      }
      if (-34.0d != ExtendedDecimal.FromString("-34").ToDouble()) {
        Assert.fail("numberObject double -34\nExpected: -34.0d\nWas: " + ExtendedDecimal.FromString("-34").ToDouble());
      }
      if (-6.9963739E17f != ExtendedDecimal.FromString("-699637360432542026").ToSingle()) {
        Assert.fail("numberObject single -699637360432542026\nExpected: -6.9963739E17f\nWas: " + ExtendedDecimal.FromString("-699637360432542026").ToSingle());
      }
      if (-6.9963736043254208E17d != ExtendedDecimal.FromString("-699637360432542026").ToDouble()) {
        Assert.fail("numberObject double -699637360432542026\nExpected: -6.9963736043254208E17d\nWas: " + ExtendedDecimal.FromString("-699637360432542026").ToDouble());
      }
      if (-8131.0f != ExtendedDecimal.FromString("-8131").ToSingle()) {
        Assert.fail("numberObject single -8131\nExpected: -8131.0f\nWas: " + ExtendedDecimal.FromString("-8131").ToSingle());
      }
      if (-8131.0d != ExtendedDecimal.FromString("-8131").ToDouble()) {
        Assert.fail("numberObject double -8131\nExpected: -8131.0d\nWas: " + ExtendedDecimal.FromString("-8131").ToDouble());
      }
      if (6.1692147E8f != ExtendedDecimal.FromString("616921472").ToSingle()) {
        Assert.fail("numberObject single 616921472\nExpected: 6.1692147E8f\nWas: " + ExtendedDecimal.FromString("616921472").ToSingle());
      }
      if (6.16921472E8d != ExtendedDecimal.FromString("616921472").ToDouble()) {
        Assert.fail("numberObject double 616921472\nExpected: 6.16921472E8d\nWas: " + ExtendedDecimal.FromString("616921472").ToDouble());
      }
      if (447272.0f != ExtendedDecimal.FromString("447272").ToSingle()) {
        Assert.fail("numberObject single 447272\nExpected: 447272.0f\nWas: " + ExtendedDecimal.FromString("447272").ToSingle());
      }
      if (447272.0d != ExtendedDecimal.FromString("447272").ToDouble()) {
        Assert.fail("numberObject double 447272\nExpected: 447272.0d\nWas: " + ExtendedDecimal.FromString("447272").ToDouble());
      }
      if (9.719524E17f != ExtendedDecimal.FromString("971952376640924713").ToSingle()) {
        Assert.fail("numberObject single 971952376640924713\nExpected: 9.719524E17f\nWas: " + ExtendedDecimal.FromString("971952376640924713").ToSingle());
      }
      if (9.7195237664092467E17d != ExtendedDecimal.FromString("971952376640924713").ToDouble()) {
        Assert.fail("numberObject double 971952376640924713\nExpected: 9.7195237664092467E17d\nWas: " + ExtendedDecimal.FromString("971952376640924713").ToDouble());
      }
      if (-8622.0f != ExtendedDecimal.FromString("-8622").ToSingle()) {
        Assert.fail("numberObject single -8622\nExpected: -8622.0f\nWas: " + ExtendedDecimal.FromString("-8622").ToSingle());
      }
      if (-8622.0d != ExtendedDecimal.FromString("-8622").ToDouble()) {
        Assert.fail("numberObject double -8622\nExpected: -8622.0d\nWas: " + ExtendedDecimal.FromString("-8622").ToDouble());
      }
      if (-9.8425534E13f != ExtendedDecimal.FromString("-98425536547570").ToSingle()) {
        Assert.fail("numberObject single -98425536547570\nExpected: -9.8425534E13f\nWas: " + ExtendedDecimal.FromString("-98425536547570").ToSingle());
      }
      if (-9.842553654757E13d != ExtendedDecimal.FromString("-98425536547570").ToDouble()) {
        Assert.fail("numberObject double -98425536547570\nExpected: -9.842553654757E13d\nWas: " + ExtendedDecimal.FromString("-98425536547570").ToDouble());
      }
      if (-1.3578545E14f != ExtendedDecimal.FromString("-135785450228746").ToSingle()) {
        Assert.fail("numberObject single -135785450228746\nExpected: -1.3578545E14f\nWas: " + ExtendedDecimal.FromString("-135785450228746").ToSingle());
      }
      if (-1.35785450228746E14d != ExtendedDecimal.FromString("-135785450228746").ToDouble()) {
        Assert.fail("numberObject double -135785450228746\nExpected: -1.35785450228746E14d\nWas: " + ExtendedDecimal.FromString("-135785450228746").ToDouble());
      }
      if (935.0f != ExtendedDecimal.FromString("935").ToSingle()) {
        Assert.fail("numberObject single 935\nExpected: 935.0f\nWas: " + ExtendedDecimal.FromString("935").ToSingle());
      }
      if (935.0d != ExtendedDecimal.FromString("935").ToDouble()) {
        Assert.fail("numberObject double 935\nExpected: 935.0d\nWas: " + ExtendedDecimal.FromString("935").ToDouble());
      }
      if (-7890.0f != ExtendedDecimal.FromString("-7890E0").ToSingle()) {
        Assert.fail("numberObject single -7890E0\nExpected: -7890.0f\nWas: " + ExtendedDecimal.FromString("-7890E0").ToSingle());
      }
      if (-7890.0d != ExtendedDecimal.FromString("-7890E0").ToDouble()) {
        Assert.fail("numberObject double -7890E0\nExpected: -7890.0d\nWas: " + ExtendedDecimal.FromString("-7890E0").ToDouble());
      }
      if (4.5492643E12f != ExtendedDecimal.FromString("45.49264316782E+11").ToSingle()) {
        Assert.fail("numberObject single 45.49264316782E+11\nExpected: 4.5492643E12f\nWas: " + ExtendedDecimal.FromString("45.49264316782E+11").ToSingle());
      }
      if (4.549264316782E12d != ExtendedDecimal.FromString("45.49264316782E+11").ToDouble()) {
        Assert.fail("numberObject double 45.49264316782E+11\nExpected: 4.549264316782E12d\nWas: " + ExtendedDecimal.FromString("45.49264316782E+11").ToDouble());
      }
      if (-7684.0f != ExtendedDecimal.FromString("-7684").ToSingle()) {
        Assert.fail("numberObject single -7684\nExpected: -7684.0f\nWas: " + ExtendedDecimal.FromString("-7684").ToSingle());
      }
      if (-7684.0d != ExtendedDecimal.FromString("-7684").ToDouble()) {
        Assert.fail("numberObject double -7684\nExpected: -7684.0d\nWas: " + ExtendedDecimal.FromString("-7684").ToDouble());
      }
      if (734069.0f != ExtendedDecimal.FromString("734069").ToSingle()) {
        Assert.fail("numberObject single 734069\nExpected: 734069.0f\nWas: " + ExtendedDecimal.FromString("734069").ToSingle());
      }
      if (734069.0d != ExtendedDecimal.FromString("734069").ToDouble()) {
        Assert.fail("numberObject double 734069\nExpected: 734069.0d\nWas: " + ExtendedDecimal.FromString("734069").ToDouble());
      }
      if (-3.51801573E12f != ExtendedDecimal.FromString("-3518015796477").ToSingle()) {
        Assert.fail("numberObject single -3518015796477\nExpected: -3.51801573E12f\nWas: " + ExtendedDecimal.FromString("-3518015796477").ToSingle());
      }
      if (-3.518015796477E12d != ExtendedDecimal.FromString("-3518015796477").ToDouble()) {
        Assert.fail("numberObject double -3518015796477\nExpected: -3.518015796477E12d\nWas: " + ExtendedDecimal.FromString("-3518015796477").ToDouble());
      }
      if (-411720.0f != ExtendedDecimal.FromString("-411720").ToSingle()) {
        Assert.fail("numberObject single -411720\nExpected: -411720.0f\nWas: " + ExtendedDecimal.FromString("-411720").ToSingle());
      }
      if (-411720.0d != ExtendedDecimal.FromString("-411720").ToDouble()) {
        Assert.fail("numberObject double -411720\nExpected: -411720.0d\nWas: " + ExtendedDecimal.FromString("-411720").ToDouble());
      }
      if (5.14432512E8f != ExtendedDecimal.FromString("514432504").ToSingle()) {
        Assert.fail("numberObject single 514432504\nExpected: 5.14432512E8f\nWas: " + ExtendedDecimal.FromString("514432504").ToSingle());
      }
      if (5.14432504E8d != ExtendedDecimal.FromString("514432504").ToDouble()) {
        Assert.fail("numberObject double 514432504\nExpected: 5.14432504E8d\nWas: " + ExtendedDecimal.FromString("514432504").ToDouble());
      }
      if (3970.0f != ExtendedDecimal.FromString("3970").ToSingle()) {
        Assert.fail("numberObject single 3970\nExpected: 3970.0f\nWas: " + ExtendedDecimal.FromString("3970").ToSingle());
      }
      if (3970.0d != ExtendedDecimal.FromString("3970").ToDouble()) {
        Assert.fail("numberObject double 3970\nExpected: 3970.0d\nWas: " + ExtendedDecimal.FromString("3970").ToDouble());
      }
      if (-1.89642527E10f != ExtendedDecimal.FromString("-18964252847").ToSingle()) {
        Assert.fail("numberObject single -18964252847\nExpected: -1.89642527E10f\nWas: " + ExtendedDecimal.FromString("-18964252847").ToSingle());
      }
      if (-1.8964252847E10d != ExtendedDecimal.FromString("-18964252847").ToDouble()) {
        Assert.fail("numberObject double -18964252847\nExpected: -1.8964252847E10d\nWas: " + ExtendedDecimal.FromString("-18964252847").ToDouble());
      }
      if (-9.5766118E10f != ExtendedDecimal.FromString("-95766116842").ToSingle()) {
        Assert.fail("numberObject single -95766116842\nExpected: -9.5766118E10f\nWas: " + ExtendedDecimal.FromString("-95766116842").ToSingle());
      }
      if (-9.5766116842E10d != ExtendedDecimal.FromString("-95766116842").ToDouble()) {
        Assert.fail("numberObject double -95766116842\nExpected: -9.5766116842E10d\nWas: " + ExtendedDecimal.FromString("-95766116842").ToDouble());
      }
      if (-4.5759559E15f != ExtendedDecimal.FromString("-4575956051893063").ToSingle()) {
        Assert.fail("numberObject single -4575956051893063\nExpected: -4.5759559E15f\nWas: " + ExtendedDecimal.FromString("-4575956051893063").ToSingle());
      }
      if (-4.575956051893063E15d != ExtendedDecimal.FromString("-4575956051893063").ToDouble()) {
        Assert.fail("numberObject double -4575956051893063\nExpected: -4.575956051893063E15d\nWas: " + ExtendedDecimal.FromString("-4575956051893063").ToDouble());
      }
      if (5.2050934E9f != ExtendedDecimal.FromString("5205093392").ToSingle()) {
        Assert.fail("numberObject single 5205093392\nExpected: 5.2050934E9f\nWas: " + ExtendedDecimal.FromString("5205093392").ToSingle());
      }
      if (5.205093392E9d != ExtendedDecimal.FromString("5205093392").ToDouble()) {
        Assert.fail("numberObject double 5205093392\nExpected: 5.205093392E9d\nWas: " + ExtendedDecimal.FromString("5205093392").ToDouble());
      }
      if (-7.0079627E12f != ExtendedDecimal.FromString("-7007962583042").ToSingle()) {
        Assert.fail("numberObject single -7007962583042\nExpected: -7.0079627E12f\nWas: " + ExtendedDecimal.FromString("-7007962583042").ToSingle());
      }
      if (-7.007962583042E12d != ExtendedDecimal.FromString("-7007962583042").ToDouble()) {
        Assert.fail("numberObject double -7007962583042\nExpected: -7.007962583042E12d\nWas: " + ExtendedDecimal.FromString("-7007962583042").ToDouble());
      }
      if (59.0f != ExtendedDecimal.FromString("59").ToSingle()) {
        Assert.fail("numberObject single 59\nExpected: 59.0f\nWas: " + ExtendedDecimal.FromString("59").ToSingle());
      }
      if (59.0d != ExtendedDecimal.FromString("59").ToDouble()) {
        Assert.fail("numberObject double 59\nExpected: 59.0d\nWas: " + ExtendedDecimal.FromString("59").ToDouble());
      }
      if (-5.5095849E16f != ExtendedDecimal.FromString("-55095850956259910").ToSingle()) {
        Assert.fail("numberObject single -55095850956259910\nExpected: -5.5095849E16f\nWas: " + ExtendedDecimal.FromString("-55095850956259910").ToSingle());
      }
      if (-5.5095850956259912E16d != ExtendedDecimal.FromString("-55095850956259910").ToDouble()) {
        Assert.fail("numberObject double -55095850956259910\nExpected: -5.5095850956259912E16d\nWas: " + ExtendedDecimal.FromString("-55095850956259910").ToDouble());
      }
      if (1.0f != ExtendedDecimal.FromString("1").ToSingle()) {
        Assert.fail("numberObject single 1\nExpected: 1.0f\nWas: " + ExtendedDecimal.FromString("1").ToSingle());
      }
      if (1.0d != ExtendedDecimal.FromString("1").ToDouble()) {
        Assert.fail("numberObject double 1\nExpected: 1.0d\nWas: " + ExtendedDecimal.FromString("1").ToDouble());
      }
      if (598.0f != ExtendedDecimal.FromString("598").ToSingle()) {
        Assert.fail("numberObject single 598\nExpected: 598.0f\nWas: " + ExtendedDecimal.FromString("598").ToSingle());
      }
      if (598.0d != ExtendedDecimal.FromString("598").ToDouble()) {
        Assert.fail("numberObject double 598\nExpected: 598.0d\nWas: " + ExtendedDecimal.FromString("598").ToDouble());
      }
      if (957.0f != ExtendedDecimal.FromString("957").ToSingle()) {
        Assert.fail("numberObject single 957\nExpected: 957.0f\nWas: " + ExtendedDecimal.FromString("957").ToSingle());
      }
      if (957.0d != ExtendedDecimal.FromString("957").ToDouble()) {
        Assert.fail("numberObject double 957\nExpected: 957.0d\nWas: " + ExtendedDecimal.FromString("957").ToDouble());
      }
      if (-1.4772274E7f != ExtendedDecimal.FromString("-14772274").ToSingle()) {
        Assert.fail("numberObject single -14772274\nExpected: -1.4772274E7f\nWas: " + ExtendedDecimal.FromString("-14772274").ToSingle());
      }
      if (-1.4772274E7d != ExtendedDecimal.FromString("-14772274").ToDouble()) {
        Assert.fail("numberObject double -14772274\nExpected: -1.4772274E7d\nWas: " + ExtendedDecimal.FromString("-14772274").ToDouble());
      }
      if (-3006.0f != ExtendedDecimal.FromString("-3006").ToSingle()) {
        Assert.fail("numberObject single -3006\nExpected: -3006.0f\nWas: " + ExtendedDecimal.FromString("-3006").ToSingle());
      }
      if (-3006.0d != ExtendedDecimal.FromString("-3006").ToDouble()) {
        Assert.fail("numberObject double -3006\nExpected: -3006.0d\nWas: " + ExtendedDecimal.FromString("-3006").ToDouble());
      }
      if (3.07120343E18f != ExtendedDecimal.FromString("3071203450148698328").ToSingle()) {
        Assert.fail("numberObject single 3071203450148698328\nExpected: 3.07120343E18f\nWas: " + ExtendedDecimal.FromString("3071203450148698328").ToSingle());
      }
      if (3.0712034501486981E18d != ExtendedDecimal.FromString("3071203450148698328").ToDouble()) {
        Assert.fail("numberObject double 3071203450148698328\nExpected: 3.0712034501486981E18d\nWas: " + ExtendedDecimal.FromString("3071203450148698328").ToDouble());
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestDecFracToSingleDouble() {
      if (-4348.0f != ExtendedDecimal.FromString("-4348").ToSingle()) {
        Assert.fail("numberObject single -4348\nExpected: -4348.0f\nWas: " + ExtendedDecimal.FromString("-4348").ToSingle());
      }
      if (-4348.0d != ExtendedDecimal.FromString("-4348").ToDouble()) {
        Assert.fail("numberObject double -4348\nExpected: -4348.0d\nWas: " + ExtendedDecimal.FromString("-4348").ToDouble());
      }
      if (-9.85323f != ExtendedDecimal.FromString("-9.85323086293411065").ToSingle()) {
        Assert.fail("numberObject single -9.85323086293411065\nExpected: -9.85323f\nWas: " + ExtendedDecimal.FromString("-9.85323086293411065").ToSingle());
      }
      if (-9.85323086293411d != ExtendedDecimal.FromString("-9.85323086293411065").ToDouble()) {
        Assert.fail("numberObject double -9.85323086293411065\nExpected: -9.85323086293411d\nWas: " + ExtendedDecimal.FromString("-9.85323086293411065").ToDouble());
      }
      if (-5.2317E9f != ExtendedDecimal.FromString("-5231.7E+6").ToSingle()) {
        Assert.fail("numberObject single -5231.7E+6\nExpected: -5.2317E9f\nWas: " + ExtendedDecimal.FromString("-5231.7E+6").ToSingle());
      }
      if (-5.2317E9d != ExtendedDecimal.FromString("-5231.7E+6").ToDouble()) {
        Assert.fail("numberObject double -5231.7E+6\nExpected: -5.2317E9d\nWas: " + ExtendedDecimal.FromString("-5231.7E+6").ToDouble());
      }
      if (5.7991604E7f != ExtendedDecimal.FromString("579916024.449917729730457E-1").ToSingle()) {
        Assert.fail("numberObject single 579916024.449917729730457E-1\nExpected: 5.7991604E7f\nWas: " + ExtendedDecimal.FromString("579916024.449917729730457E-1").ToSingle());
      }
      if (5.7991602444991775E7d != ExtendedDecimal.FromString("579916024.449917729730457E-1").ToDouble()) {
        Assert.fail("numberObject double 579916024.449917729730457E-1\nExpected: 5.7991602444991775E7d\nWas: " + ExtendedDecimal.FromString("579916024.449917729730457E-1").ToDouble());
      }
      if (-515.02563f != ExtendedDecimal.FromString("-515025607547098618E-15").ToSingle()) {
        Assert.fail("numberObject single -515025607547098618E-15\nExpected: -515.02563f\nWas: " + ExtendedDecimal.FromString("-515025607547098618E-15").ToSingle());
      }
      if (-515.0256075470986d != ExtendedDecimal.FromString("-515025607547098618E-15").ToDouble()) {
        Assert.fail("numberObject double -515025607547098618E-15\nExpected: -515.0256075470986d\nWas: " + ExtendedDecimal.FromString("-515025607547098618E-15").ToDouble());
      }
      if (-9.3541843E10f != ExtendedDecimal.FromString("-93541840706").ToSingle()) {
        Assert.fail("numberObject single -93541840706\nExpected: -9.3541843E10f\nWas: " + ExtendedDecimal.FromString("-93541840706").ToSingle());
      }
      if (-9.3541840706E10d != ExtendedDecimal.FromString("-93541840706").ToDouble()) {
        Assert.fail("numberObject double -93541840706\nExpected: -9.3541840706E10d\nWas: " + ExtendedDecimal.FromString("-93541840706").ToDouble());
      }
      if (3.8568078E23f != ExtendedDecimal.FromString("38568076767380659.6E+7").ToSingle()) {
        Assert.fail("numberObject single 38568076767380659.6E+7\nExpected: 3.8568078E23f\nWas: " + ExtendedDecimal.FromString("38568076767380659.6E+7").ToSingle());
      }
      if (3.8568076767380657E23d != ExtendedDecimal.FromString("38568076767380659.6E+7").ToDouble()) {
        Assert.fail("numberObject double 38568076767380659.6E+7\nExpected: 3.8568076767380657E23d\nWas: " + ExtendedDecimal.FromString("38568076767380659.6E+7").ToDouble());
      }
      if (4682.1987f != ExtendedDecimal.FromString("468219867826E-8").ToSingle()) {
        Assert.fail("numberObject single 468219867826E-8\nExpected: 4682.1987f\nWas: " + ExtendedDecimal.FromString("468219867826E-8").ToSingle());
      }
      if (4682.19867826d != ExtendedDecimal.FromString("468219867826E-8").ToDouble()) {
        Assert.fail("numberObject double 468219867826E-8\nExpected: 4682.19867826d\nWas: " + ExtendedDecimal.FromString("468219867826E-8").ToDouble());
      }
      if (7.3869363E-4f != ExtendedDecimal.FromString("73869365.3859328709200790828E-11").ToSingle()) {
        Assert.fail("numberObject single 73869365.3859328709200790828E-11\nExpected: 7.3869363E-4f\nWas: " + ExtendedDecimal.FromString("73869365.3859328709200790828E-11").ToSingle());
      }
      if (7.386936538593287E-4d != ExtendedDecimal.FromString("73869365.3859328709200790828E-11").ToDouble()) {
        Assert.fail("numberObject double 73869365.3859328709200790828E-11\nExpected: 7.386936538593287E-4d\nWas: " + ExtendedDecimal.FromString("73869365.3859328709200790828E-11").ToDouble());
      }
      if (2.3f != ExtendedDecimal.FromString("2.3E0").ToSingle()) {
        Assert.fail("numberObject single 2.3E0\nExpected: 2.3f\nWas: " + ExtendedDecimal.FromString("2.3E0").ToSingle());
      }
      if (2.3d != ExtendedDecimal.FromString("2.3E0").ToDouble()) {
        Assert.fail("numberObject double 2.3E0\nExpected: 2.3d\nWas: " + ExtendedDecimal.FromString("2.3E0").ToDouble());
      }
      if (3.3713182E15f != ExtendedDecimal.FromString("3371318258253373.59498533176159560").ToSingle()) {
        Assert.fail("numberObject single 3371318258253373.59498533176159560\nExpected: 3.3713182E15f\nWas: " + ExtendedDecimal.FromString("3371318258253373.59498533176159560").ToSingle());
      }
      if (3.3713182582533735E15d != ExtendedDecimal.FromString("3371318258253373.59498533176159560").ToDouble()) {
        Assert.fail("numberObject double 3371318258253373.59498533176159560\nExpected: 3.3713182582533735E15d\nWas: " + ExtendedDecimal.FromString("3371318258253373.59498533176159560").ToDouble());
      }
      if (0.08044683f != ExtendedDecimal.FromString("804468350612974.6118902086132089233E-16").ToSingle()) {
        Assert.fail("numberObject single 804468350612974.6118902086132089233E-16\nExpected: 0.08044683f\nWas: " + ExtendedDecimal.FromString("804468350612974.6118902086132089233E-16").ToSingle());
      }
      if (0.08044683506129746d != ExtendedDecimal.FromString("804468350612974.6118902086132089233E-16").ToDouble()) {
        Assert.fail("numberObject double 804468350612974.6118902086132089233E-16\nExpected: 0.08044683506129746d\nWas: " + ExtendedDecimal.FromString("804468350612974.6118902086132089233E-16").ToDouble());
      }
      if (-7.222071E19f != ExtendedDecimal.FromString("-72220708347127407337.28").ToSingle()) {
        Assert.fail("numberObject single -72220708347127407337.28\nExpected: -7.222071E19f\nWas: " + ExtendedDecimal.FromString("-72220708347127407337.28").ToSingle());
      }
      if (-7.222070834712741E19d != ExtendedDecimal.FromString("-72220708347127407337.28").ToDouble()) {
        Assert.fail("numberObject double -72220708347127407337.28\nExpected: -7.222070834712741E19d\nWas: " + ExtendedDecimal.FromString("-72220708347127407337.28").ToDouble());
      }
      if (9715796.0f != ExtendedDecimal.FromString("9715796.4299331966870989").ToSingle()) {
        Assert.fail("numberObject single 9715796.4299331966870989\nExpected: 9715796.0f\nWas: " + ExtendedDecimal.FromString("9715796.4299331966870989").ToSingle());
      }
      if (9715796.429933196d != ExtendedDecimal.FromString("9715796.4299331966870989").ToDouble()) {
        Assert.fail("numberObject double 9715796.4299331966870989\nExpected: 9715796.429933196d\nWas: " + ExtendedDecimal.FromString("9715796.4299331966870989").ToDouble());
      }
      if (9.3596612E14f != ExtendedDecimal.FromString("93596609961883873.8463754373628236E-2").ToSingle()) {
        Assert.fail("numberObject single 93596609961883873.8463754373628236E-2\nExpected: 9.3596612E14f\nWas: " + ExtendedDecimal.FromString("93596609961883873.8463754373628236E-2").ToSingle());
      }
      if (9.359660996188388E14d != ExtendedDecimal.FromString("93596609961883873.8463754373628236E-2").ToDouble()) {
        Assert.fail("numberObject double 93596609961883873.8463754373628236E-2\nExpected: 9.359660996188388E14d\nWas: " + ExtendedDecimal.FromString("93596609961883873.8463754373628236E-2").ToDouble());
      }
      if (4.82799354E14f != ExtendedDecimal.FromString("482799357899450").ToSingle()) {
        Assert.fail("numberObject single 482799357899450\nExpected: 4.82799354E14f\nWas: " + ExtendedDecimal.FromString("482799357899450").ToSingle());
      }
      if (4.8279935789945E14d != ExtendedDecimal.FromString("482799357899450").ToDouble()) {
        Assert.fail("numberObject double 482799357899450\nExpected: 4.8279935789945E14d\nWas: " + ExtendedDecimal.FromString("482799357899450").ToDouble());
      }
      if (3.8193924E25f != ExtendedDecimal.FromString("381939236989E+14").ToSingle()) {
        Assert.fail("numberObject single 381939236989E+14\nExpected: 3.8193924E25f\nWas: " + ExtendedDecimal.FromString("381939236989E+14").ToSingle());
      }
      if (3.81939236989E25d != ExtendedDecimal.FromString("381939236989E+14").ToDouble()) {
        Assert.fail("numberObject double 381939236989E+14\nExpected: 3.81939236989E25d\nWas: " + ExtendedDecimal.FromString("381939236989E+14").ToDouble());
      }
      if (-3.1092332E27f != ExtendedDecimal.FromString("-3109233371824024E+12").ToSingle()) {
        Assert.fail("numberObject single -3109233371824024E+12\nExpected: -3.1092332E27f\nWas: " + ExtendedDecimal.FromString("-3109233371824024E+12").ToSingle());
      }
      if (-3.109233371824024E27d != ExtendedDecimal.FromString("-3109233371824024E+12").ToDouble()) {
        Assert.fail("numberObject double -3109233371824024E+12\nExpected: -3.109233371824024E27d\nWas: " + ExtendedDecimal.FromString("-3109233371824024E+12").ToDouble());
      }
      if (-0.006658507f != ExtendedDecimal.FromString("-66585.07E-7").ToSingle()) {
        Assert.fail("numberObject single -66585.07E-7\nExpected: -0.006658507f\nWas: " + ExtendedDecimal.FromString("-66585.07E-7").ToSingle());
      }
      if (-0.006658507d != ExtendedDecimal.FromString("-66585.07E-7").ToDouble()) {
        Assert.fail("numberObject double -66585.07E-7\nExpected: -0.006658507d\nWas: " + ExtendedDecimal.FromString("-66585.07E-7").ToDouble());
      }
      if (17.276796f != ExtendedDecimal.FromString("17.276795549708").ToSingle()) {
        Assert.fail("numberObject single 17.276795549708\nExpected: 17.276796f\nWas: " + ExtendedDecimal.FromString("17.276795549708").ToSingle());
      }
      if (17.276795549708d != ExtendedDecimal.FromString("17.276795549708").ToDouble()) {
        Assert.fail("numberObject double 17.276795549708\nExpected: 17.276795549708d\nWas: " + ExtendedDecimal.FromString("17.276795549708").ToDouble());
      }
      if (-3210939.5f != ExtendedDecimal.FromString("-321093943510192.3307E-8").ToSingle()) {
        Assert.fail("numberObject single -321093943510192.3307E-8\nExpected: -3210939.5f\nWas: " + ExtendedDecimal.FromString("-321093943510192.3307E-8").ToSingle());
      }
      if (-3210939.4351019235d != ExtendedDecimal.FromString("-321093943510192.3307E-8").ToDouble()) {
        Assert.fail("numberObject double -321093943510192.3307E-8\nExpected: -3210939.4351019235d\nWas: " + ExtendedDecimal.FromString("-321093943510192.3307E-8").ToDouble());
      }
      if (-976.9676f != ExtendedDecimal.FromString("-976.967597776185553735").ToSingle()) {
        Assert.fail("numberObject single -976.967597776185553735\nExpected: -976.9676f\nWas: " + ExtendedDecimal.FromString("-976.967597776185553735").ToSingle());
      }
      if (-976.9675977761856d != ExtendedDecimal.FromString("-976.967597776185553735").ToDouble()) {
        Assert.fail("numberObject double -976.967597776185553735\nExpected: -976.9675977761856d\nWas: " + ExtendedDecimal.FromString("-976.967597776185553735").ToDouble());
      }
      if (-3.49712614E9f != ExtendedDecimal.FromString("-3497126138").ToSingle()) {
        Assert.fail("numberObject single -3497126138\nExpected: -3.49712614E9f\nWas: " + ExtendedDecimal.FromString("-3497126138").ToSingle());
      }
      if (-3.497126138E9d != ExtendedDecimal.FromString("-3497126138").ToDouble()) {
        Assert.fail("numberObject double -3497126138\nExpected: -3.497126138E9d\nWas: " + ExtendedDecimal.FromString("-3497126138").ToDouble());
      }
      if (-2.63418028E14f != ExtendedDecimal.FromString("-2634180.2455697965376217503E+8").ToSingle()) {
        Assert.fail("numberObject single -2634180.2455697965376217503E+8\nExpected: -2.63418028E14f\nWas: " + ExtendedDecimal.FromString("-2634180.2455697965376217503E+8").ToSingle());
      }
      if (-2.6341802455697966E14d != ExtendedDecimal.FromString("-2634180.2455697965376217503E+8").ToDouble()) {
        Assert.fail("numberObject double -2634180.2455697965376217503E+8\nExpected: -2.6341802455697966E14d\nWas: " + ExtendedDecimal.FromString("-2634180.2455697965376217503E+8").ToDouble());
      }
      if (3.25314253E10f != ExtendedDecimal.FromString("32531426161").ToSingle()) {
        Assert.fail("numberObject single 32531426161\nExpected: 3.25314253E10f\nWas: " + ExtendedDecimal.FromString("32531426161").ToSingle());
      }
      if (3.2531426161E10d != ExtendedDecimal.FromString("32531426161").ToDouble()) {
        Assert.fail("numberObject double 32531426161\nExpected: 3.2531426161E10d\nWas: " + ExtendedDecimal.FromString("32531426161").ToDouble());
      }
      if (-83825.7f != ExtendedDecimal.FromString("-83825.7").ToSingle()) {
        Assert.fail("numberObject single -83825.7\nExpected: -83825.7f\nWas: " + ExtendedDecimal.FromString("-83825.7").ToSingle());
      }
      if (-83825.7d != ExtendedDecimal.FromString("-83825.7").ToDouble()) {
        Assert.fail("numberObject double -83825.7\nExpected: -83825.7d\nWas: " + ExtendedDecimal.FromString("-83825.7").ToDouble());
      }
      if (9347.0f != ExtendedDecimal.FromString("9347").ToSingle()) {
        Assert.fail("numberObject single 9347\nExpected: 9347.0f\nWas: " + ExtendedDecimal.FromString("9347").ToSingle());
      }
      if (9347.0d != ExtendedDecimal.FromString("9347").ToDouble()) {
        Assert.fail("numberObject double 9347\nExpected: 9347.0d\nWas: " + ExtendedDecimal.FromString("9347").ToDouble());
      }
      if (4039.426f != ExtendedDecimal.FromString("403942604431E-8").ToSingle()) {
        Assert.fail("numberObject single 403942604431E-8\nExpected: 4039.426f\nWas: " + ExtendedDecimal.FromString("403942604431E-8").ToSingle());
      }
      if (4039.42604431d != ExtendedDecimal.FromString("403942604431E-8").ToDouble()) {
        Assert.fail("numberObject double 403942604431E-8\nExpected: 4039.42604431d\nWas: " + ExtendedDecimal.FromString("403942604431E-8").ToDouble());
      }
      if (9.821772E-8f != ExtendedDecimal.FromString("9821771729.481512E-17").ToSingle()) {
        Assert.fail("numberObject single 9821771729.481512E-17\nExpected: 9.821772E-8f\nWas: " + ExtendedDecimal.FromString("9821771729.481512E-17").ToSingle());
      }
      if (9.821771729481512E-8d != ExtendedDecimal.FromString("9821771729.481512E-17").ToDouble()) {
        Assert.fail("numberObject double 9821771729.481512E-17\nExpected: 9.821771729481512E-8d\nWas: " + ExtendedDecimal.FromString("9821771729.481512E-17").ToDouble());
      }
      if (1.47027E24f != ExtendedDecimal.FromString("1470270E+18").ToSingle()) {
        Assert.fail("numberObject single 1470270E+18\nExpected: 1.47027E24f\nWas: " + ExtendedDecimal.FromString("1470270E+18").ToSingle());
      }
      if (1.47027E24d != ExtendedDecimal.FromString("1470270E+18").ToDouble()) {
        Assert.fail("numberObject double 1470270E+18\nExpected: 1.47027E24d\nWas: " + ExtendedDecimal.FromString("1470270E+18").ToDouble());
      }
      if (504.07468f != ExtendedDecimal.FromString("504.074687047275").ToSingle()) {
        Assert.fail("numberObject single 504.074687047275\nExpected: 504.07468f\nWas: " + ExtendedDecimal.FromString("504.074687047275").ToSingle());
      }
      if (504.074687047275d != ExtendedDecimal.FromString("504.074687047275").ToDouble()) {
        Assert.fail("numberObject double 504.074687047275\nExpected: 504.074687047275d\nWas: " + ExtendedDecimal.FromString("504.074687047275").ToDouble());
      }
      if (8.051101E-11f != ExtendedDecimal.FromString("8051.10083245768396604E-14").ToSingle()) {
        Assert.fail("numberObject single 8051.10083245768396604E-14\nExpected: 8.051101E-11f\nWas: " + ExtendedDecimal.FromString("8051.10083245768396604E-14").ToSingle());
      }
      if (8.051100832457683E-11d != ExtendedDecimal.FromString("8051.10083245768396604E-14").ToDouble()) {
        Assert.fail("numberObject double 8051.10083245768396604E-14\nExpected: 8.051100832457683E-11d\nWas: " + ExtendedDecimal.FromString("8051.10083245768396604E-14").ToDouble());
      }
      if (-9789.0f != ExtendedDecimal.FromString("-9789").ToSingle()) {
        Assert.fail("numberObject single -9789\nExpected: -9789.0f\nWas: " + ExtendedDecimal.FromString("-9789").ToSingle());
      }
      if (-9789.0d != ExtendedDecimal.FromString("-9789").ToDouble()) {
        Assert.fail("numberObject double -9789\nExpected: -9789.0d\nWas: " + ExtendedDecimal.FromString("-9789").ToDouble());
      }
      if (-2.95046595E10f != ExtendedDecimal.FromString("-295046585154199748.8456E-7").ToSingle()) {
        Assert.fail("numberObject single -295046585154199748.8456E-7\nExpected: -2.95046595E10f\nWas: " + ExtendedDecimal.FromString("-295046585154199748.8456E-7").ToSingle());
      }
      if (-2.9504658515419975E10d != ExtendedDecimal.FromString("-295046585154199748.8456E-7").ToDouble()) {
        Assert.fail("numberObject double -295046585154199748.8456E-7\nExpected: -2.9504658515419975E10d\nWas: " + ExtendedDecimal.FromString("-295046585154199748.8456E-7").ToDouble());
      }
      if (5.8642877E23f != ExtendedDecimal.FromString("58642877210005207.915393764393974811E+7").ToSingle()) {
        Assert.fail("numberObject single 58642877210005207.915393764393974811E+7\nExpected: 5.8642877E23f\nWas: " + ExtendedDecimal.FromString("58642877210005207.915393764393974811E+7").ToSingle());
      }
      if (5.864287721000521E23d != ExtendedDecimal.FromString("58642877210005207.915393764393974811E+7").ToDouble()) {
        Assert.fail("numberObject double 58642877210005207.915393764393974811E+7\nExpected: 5.864287721000521E23d\nWas: " + ExtendedDecimal.FromString("58642877210005207.915393764393974811E+7").ToDouble());
      }
      if (-5.13554645E11f != ExtendedDecimal.FromString("-513554652569").ToSingle()) {
        Assert.fail("numberObject single -513554652569\nExpected: -5.13554645E11f\nWas: " + ExtendedDecimal.FromString("-513554652569").ToSingle());
      }
      if (-5.13554652569E11d != ExtendedDecimal.FromString("-513554652569").ToDouble()) {
        Assert.fail("numberObject double -513554652569\nExpected: -5.13554652569E11d\nWas: " + ExtendedDecimal.FromString("-513554652569").ToDouble());
      }
      if (-1.66059725E10f != ExtendedDecimal.FromString("-166059726561900E-4").ToSingle()) {
        Assert.fail("numberObject single -166059726561900E-4\nExpected: -1.66059725E10f\nWas: " + ExtendedDecimal.FromString("-166059726561900E-4").ToSingle());
      }
      if (-1.660597265619E10d != ExtendedDecimal.FromString("-166059726561900E-4").ToDouble()) {
        Assert.fail("numberObject double -166059726561900E-4\nExpected: -1.660597265619E10d\nWas: " + ExtendedDecimal.FromString("-166059726561900E-4").ToDouble());
      }
      if (-3.66681318E9f != ExtendedDecimal.FromString("-3666813090").ToSingle()) {
        Assert.fail("numberObject single -3666813090\nExpected: -3.66681318E9f\nWas: " + ExtendedDecimal.FromString("-3666813090").ToSingle());
      }
      if (-3.66681309E9d != ExtendedDecimal.FromString("-3666813090").ToDouble()) {
        Assert.fail("numberObject double -3666813090\nExpected: -3.66681309E9d\nWas: " + ExtendedDecimal.FromString("-3666813090").ToDouble());
      }
      if (-741.0616f != ExtendedDecimal.FromString("-741.061579731811").ToSingle()) {
        Assert.fail("numberObject single -741.061579731811\nExpected: -741.0616f\nWas: " + ExtendedDecimal.FromString("-741.061579731811").ToSingle());
      }
      if (-741.061579731811d != ExtendedDecimal.FromString("-741.061579731811").ToDouble()) {
        Assert.fail("numberObject double -741.061579731811\nExpected: -741.061579731811d\nWas: " + ExtendedDecimal.FromString("-741.061579731811").ToDouble());
      }
      if (-2264.0f != ExtendedDecimal.FromString("-2264").ToSingle()) {
        Assert.fail("numberObject single -2264\nExpected: -2264.0f\nWas: " + ExtendedDecimal.FromString("-2264").ToSingle());
      }
      if (-2264.0d != ExtendedDecimal.FromString("-2264").ToDouble()) {
        Assert.fail("numberObject double -2264\nExpected: -2264.0d\nWas: " + ExtendedDecimal.FromString("-2264").ToDouble());
      }
      if (9.2388336E10f != ExtendedDecimal.FromString("92388332924").ToSingle()) {
        Assert.fail("numberObject single 92388332924\nExpected: 9.2388336E10f\nWas: " + ExtendedDecimal.FromString("92388332924").ToSingle());
      }
      if (9.2388332924E10d != ExtendedDecimal.FromString("92388332924").ToDouble()) {
        Assert.fail("numberObject double 92388332924\nExpected: 9.2388332924E10d\nWas: " + ExtendedDecimal.FromString("92388332924").ToDouble());
      }
      if (4991.7646f != ExtendedDecimal.FromString("4991.764823290772791").ToSingle()) {
        Assert.fail("numberObject single 4991.764823290772791\nExpected: 4991.7646f\nWas: " + ExtendedDecimal.FromString("4991.764823290772791").ToSingle());
      }
      if (4991.764823290773d != ExtendedDecimal.FromString("4991.764823290772791").ToDouble()) {
        Assert.fail("numberObject double 4991.764823290772791\nExpected: 4991.764823290773d\nWas: " + ExtendedDecimal.FromString("4991.764823290772791").ToDouble());
      }
      if (-31529.82f != ExtendedDecimal.FromString("-3152982E-2").ToSingle()) {
        Assert.fail("numberObject single -3152982E-2\nExpected: -31529.82f\nWas: " + ExtendedDecimal.FromString("-3152982E-2").ToSingle());
      }
      if (-31529.82d != ExtendedDecimal.FromString("-3152982E-2").ToDouble()) {
        Assert.fail("numberObject double -3152982E-2\nExpected: -31529.82d\nWas: " + ExtendedDecimal.FromString("-3152982E-2").ToDouble());
      }
      if (-2.96352045E15f != ExtendedDecimal.FromString("-2963520450661169.515038656").ToSingle()) {
        Assert.fail("numberObject single -2963520450661169.515038656\nExpected: -2.96352045E15f\nWas: " + ExtendedDecimal.FromString("-2963520450661169.515038656").ToSingle());
      }
      if (-2.9635204506611695E15d != ExtendedDecimal.FromString("-2963520450661169.515038656").ToDouble()) {
        Assert.fail("numberObject double -2963520450661169.515038656\nExpected: -2.9635204506611695E15d\nWas: " + ExtendedDecimal.FromString("-2963520450661169.515038656").ToDouble());
      }
      if (-9.0629749E13f != ExtendedDecimal.FromString("-9062974752750092585.8070204683471E-5").ToSingle()) {
        Assert.fail("numberObject single -9062974752750092585.8070204683471E-5\nExpected: -9.0629749E13f\nWas: " + ExtendedDecimal.FromString("-9062974752750092585.8070204683471E-5").ToSingle());
      }
      if (-9.062974752750092E13d != ExtendedDecimal.FromString("-9062974752750092585.8070204683471E-5").ToDouble()) {
        Assert.fail("numberObject double -9062974752750092585.8070204683471E-5\nExpected: -9.062974752750092E13d\nWas: " + ExtendedDecimal.FromString("-9062974752750092585.8070204683471E-5").ToDouble());
      }
      if (1.32708426E11f != ExtendedDecimal.FromString("1327.08423724267788662E+8").ToSingle()) {
        Assert.fail("numberObject single 1327.08423724267788662E+8\nExpected: 1.32708426E11f\nWas: " + ExtendedDecimal.FromString("1327.08423724267788662E+8").ToSingle());
      }
      if (1.3270842372426779E11d != ExtendedDecimal.FromString("1327.08423724267788662E+8").ToDouble()) {
        Assert.fail("numberObject double 1327.08423724267788662E+8\nExpected: 1.3270842372426779E11d\nWas: " + ExtendedDecimal.FromString("1327.08423724267788662E+8").ToDouble());
      }
      if (3.03766274E11f != ExtendedDecimal.FromString("3037662626861314743.2222785E-7").ToSingle()) {
        Assert.fail("numberObject single 3037662626861314743.2222785E-7\nExpected: 3.03766274E11f\nWas: " + ExtendedDecimal.FromString("3037662626861314743.2222785E-7").ToSingle());
      }
      if (3.037662626861315E11d != ExtendedDecimal.FromString("3037662626861314743.2222785E-7").ToDouble()) {
        Assert.fail("numberObject double 3037662626861314743.2222785E-7\nExpected: 3.037662626861315E11d\nWas: " + ExtendedDecimal.FromString("3037662626861314743.2222785E-7").ToDouble());
      }
      if (5.3666539E12f != ExtendedDecimal.FromString("5366653818787.5E0").ToSingle()) {
        Assert.fail("numberObject single 5366653818787.5E0\nExpected: 5.3666539E12f\nWas: " + ExtendedDecimal.FromString("5366653818787.5E0").ToSingle());
      }
      if (5.3666538187875E12d != ExtendedDecimal.FromString("5366653818787.5E0").ToDouble()) {
        Assert.fail("numberObject double 5366653818787.5E0\nExpected: 5.3666538187875E12d\nWas: " + ExtendedDecimal.FromString("5366653818787.5E0").ToDouble());
      }
      if (-0.09572517f != ExtendedDecimal.FromString("-957251.70125291697919424260E-7").ToSingle()) {
        Assert.fail("numberObject single -957251.70125291697919424260E-7\nExpected: -0.09572517f\nWas: " + ExtendedDecimal.FromString("-957251.70125291697919424260E-7").ToSingle());
      }
      if (-0.09572517012529169d != ExtendedDecimal.FromString("-957251.70125291697919424260E-7").ToDouble()) {
        Assert.fail("numberObject double -957251.70125291697919424260E-7\nExpected: -0.09572517012529169d\nWas: " + ExtendedDecimal.FromString("-957251.70125291697919424260E-7").ToDouble());
      }
      if (8.4375632E7f != ExtendedDecimal.FromString("8437563497492.8514E-5").ToSingle()) {
        Assert.fail("numberObject single 8437563497492.8514E-5\nExpected: 8.4375632E7f\nWas: " + ExtendedDecimal.FromString("8437563497492.8514E-5").ToSingle());
      }
      if (8.437563497492851E7d != ExtendedDecimal.FromString("8437563497492.8514E-5").ToDouble()) {
        Assert.fail("numberObject double 8437563497492.8514E-5\nExpected: 8.437563497492851E7d\nWas: " + ExtendedDecimal.FromString("8437563497492.8514E-5").ToDouble());
      }
      if (7.7747428E15f != ExtendedDecimal.FromString("7774742890322348.749566199224594").ToSingle()) {
        Assert.fail("numberObject single 7774742890322348.749566199224594\nExpected: 7.7747428E15f\nWas: " + ExtendedDecimal.FromString("7774742890322348.749566199224594").ToSingle());
      }
      if (7.774742890322349E15d != ExtendedDecimal.FromString("7774742890322348.749566199224594").ToDouble()) {
        Assert.fail("numberObject double 7774742890322348.749566199224594\nExpected: 7.774742890322349E15d\nWas: " + ExtendedDecimal.FromString("7774742890322348.749566199224594").ToDouble());
      }
      if (-6.3523806E18f != ExtendedDecimal.FromString("-6352380631468114E+3").ToSingle()) {
        Assert.fail("numberObject single -6352380631468114E+3\nExpected: -6.3523806E18f\nWas: " + ExtendedDecimal.FromString("-6352380631468114E+3").ToSingle());
      }
      if (-6.3523806314681139E18d != ExtendedDecimal.FromString("-6352380631468114E+3").ToDouble()) {
        Assert.fail("numberObject double -6352380631468114E+3\nExpected: -6.3523806314681139E18d\nWas: " + ExtendedDecimal.FromString("-6352380631468114E+3").ToDouble());
      }
      if (-8.1199685E23f != ExtendedDecimal.FromString("-8119968851439E+11").ToSingle()) {
        Assert.fail("numberObject single -8119968851439E+11\nExpected: -8.1199685E23f\nWas: " + ExtendedDecimal.FromString("-8119968851439E+11").ToSingle());
      }
      if (-8.119968851439E23d != ExtendedDecimal.FromString("-8119968851439E+11").ToDouble()) {
        Assert.fail("numberObject double -8119968851439E+11\nExpected: -8.119968851439E23d\nWas: " + ExtendedDecimal.FromString("-8119968851439E+11").ToDouble());
      }
      if (-3.201959E23f != ExtendedDecimal.FromString("-3201959209367.08531737604446E+11").ToSingle()) {
        Assert.fail("numberObject single -3201959209367.08531737604446E+11\nExpected: -3.201959E23f\nWas: " + ExtendedDecimal.FromString("-3201959209367.08531737604446E+11").ToSingle());
      }
      if (-3.201959209367085E23d != ExtendedDecimal.FromString("-3201959209367.08531737604446E+11").ToDouble()) {
        Assert.fail("numberObject double -3201959209367.08531737604446E+11\nExpected: -3.201959209367085E23d\nWas: " + ExtendedDecimal.FromString("-3201959209367.08531737604446E+11").ToDouble());
      }
      if (-6.0171188E7f != ExtendedDecimal.FromString("-60171187").ToSingle()) {
        Assert.fail("numberObject single -60171187\nExpected: -6.0171188E7f\nWas: " + ExtendedDecimal.FromString("-60171187").ToSingle());
      }
      if (-6.0171187E7d != ExtendedDecimal.FromString("-60171187").ToDouble()) {
        Assert.fail("numberObject double -60171187\nExpected: -6.0171187E7d\nWas: " + ExtendedDecimal.FromString("-60171187").ToDouble());
      }
      if (-6.6884155E-7f != ExtendedDecimal.FromString("-66.884154716131E-8").ToSingle()) {
        Assert.fail("numberObject single -66.884154716131E-8\nExpected: -6.6884155E-7f\nWas: " + ExtendedDecimal.FromString("-66.884154716131E-8").ToSingle());
      }
      if (-6.6884154716131E-7d != ExtendedDecimal.FromString("-66.884154716131E-8").ToDouble()) {
        Assert.fail("numberObject double -66.884154716131E-8\nExpected: -6.6884154716131E-7d\nWas: " + ExtendedDecimal.FromString("-66.884154716131E-8").ToDouble());
      }
      if (923595.4f != ExtendedDecimal.FromString("923595.376427445").ToSingle()) {
        Assert.fail("numberObject single 923595.376427445\nExpected: 923595.4f\nWas: " + ExtendedDecimal.FromString("923595.376427445").ToSingle());
      }
      if (923595.376427445d != ExtendedDecimal.FromString("923595.376427445").ToDouble()) {
        Assert.fail("numberObject double 923595.376427445\nExpected: 923595.376427445d\nWas: " + ExtendedDecimal.FromString("923595.376427445").ToDouble());
      }
      if (-5.0f != ExtendedDecimal.FromString("-5").ToSingle()) {
        Assert.fail("numberObject single -5\nExpected: -5.0f\nWas: " + ExtendedDecimal.FromString("-5").ToSingle());
      }
      if (-5.0d != ExtendedDecimal.FromString("-5").ToDouble()) {
        Assert.fail("numberObject double -5\nExpected: -5.0d\nWas: " + ExtendedDecimal.FromString("-5").ToDouble());
      }
      if (4.7380017E10f != ExtendedDecimal.FromString("47380017776.35").ToSingle()) {
        Assert.fail("numberObject single 47380017776.35\nExpected: 4.7380017E10f\nWas: " + ExtendedDecimal.FromString("47380017776.35").ToSingle());
      }
      if (4.738001777635E10d != ExtendedDecimal.FromString("47380017776.35").ToDouble()) {
        Assert.fail("numberObject double 47380017776.35\nExpected: 4.738001777635E10d\nWas: " + ExtendedDecimal.FromString("47380017776.35").ToDouble());
      }
      if (8139584.0f != ExtendedDecimal.FromString("8139584.242987").ToSingle()) {
        Assert.fail("numberObject single 8139584.242987\nExpected: 8139584.0f\nWas: " + ExtendedDecimal.FromString("8139584.242987").ToSingle());
      }
      if (8139584.242987d != ExtendedDecimal.FromString("8139584.242987").ToDouble()) {
        Assert.fail("numberObject double 8139584.242987\nExpected: 8139584.242987d\nWas: " + ExtendedDecimal.FromString("8139584.242987").ToDouble());
      }
      if (5.0f != ExtendedDecimal.FromString("5").ToSingle()) {
        Assert.fail("numberObject single 5\nExpected: 5.0f\nWas: " + ExtendedDecimal.FromString("5").ToSingle());
      }
      if (5.0d != ExtendedDecimal.FromString("5").ToDouble()) {
        Assert.fail("numberObject double 5\nExpected: 5.0d\nWas: " + ExtendedDecimal.FromString("5").ToDouble());
      }
      if (-3.6578223E27f != ExtendedDecimal.FromString("-365782224812843E+13").ToSingle()) {
        Assert.fail("numberObject single -365782224812843E+13\nExpected: -3.6578223E27f\nWas: " + ExtendedDecimal.FromString("-365782224812843E+13").ToSingle());
      }
      if (-3.65782224812843E27d != ExtendedDecimal.FromString("-365782224812843E+13").ToDouble()) {
        Assert.fail("numberObject double -365782224812843E+13\nExpected: -3.65782224812843E27d\nWas: " + ExtendedDecimal.FromString("-365782224812843E+13").ToDouble());
      }
      if (6.263606E23f != ExtendedDecimal.FromString("626360584867890223E+6").ToSingle()) {
        Assert.fail("numberObject single 626360584867890223E+6\nExpected: 6.263606E23f\nWas: " + ExtendedDecimal.FromString("626360584867890223E+6").ToSingle());
      }
      if (6.263605848678903E23d != ExtendedDecimal.FromString("626360584867890223E+6").ToDouble()) {
        Assert.fail("numberObject double 626360584867890223E+6\nExpected: 6.263605848678903E23d\nWas: " + ExtendedDecimal.FromString("626360584867890223E+6").ToDouble());
      }
      if (-1.26830412E18f != ExtendedDecimal.FromString("-12683040859E+8").ToSingle()) {
        Assert.fail("numberObject single -12683040859E+8\nExpected: -1.26830412E18f\nWas: " + ExtendedDecimal.FromString("-12683040859E+8").ToSingle());
      }
      if (-1.2683040859E18d != ExtendedDecimal.FromString("-12683040859E+8").ToDouble()) {
        Assert.fail("numberObject double -12683040859E+8\nExpected: -1.2683040859E18d\nWas: " + ExtendedDecimal.FromString("-12683040859E+8").ToDouble());
      }
      if (8.9906433E13f != ExtendedDecimal.FromString("89906433733052.14691421345561385").ToSingle()) {
        Assert.fail("numberObject single 89906433733052.14691421345561385\nExpected: 8.9906433E13f\nWas: " + ExtendedDecimal.FromString("89906433733052.14691421345561385").ToSingle());
      }
      if (8.990643373305214E13d != ExtendedDecimal.FromString("89906433733052.14691421345561385").ToDouble()) {
        Assert.fail("numberObject double 89906433733052.14691421345561385\nExpected: 8.990643373305214E13d\nWas: " + ExtendedDecimal.FromString("89906433733052.14691421345561385").ToDouble());
      }
      if (82.0f != ExtendedDecimal.FromString("82").ToSingle()) {
        Assert.fail("numberObject single 82\nExpected: 82.0f\nWas: " + ExtendedDecimal.FromString("82").ToSingle());
      }
      if (82.0d != ExtendedDecimal.FromString("82").ToDouble()) {
        Assert.fail("numberObject double 82\nExpected: 82.0d\nWas: " + ExtendedDecimal.FromString("82").ToDouble());
      }
      if (9.5523543E16f != ExtendedDecimal.FromString("95523541216159667").ToSingle()) {
        Assert.fail("numberObject single 95523541216159667\nExpected: 9.5523543E16f\nWas: " + ExtendedDecimal.FromString("95523541216159667").ToSingle());
      }
      if (9.5523541216159664E16d != ExtendedDecimal.FromString("95523541216159667").ToDouble()) {
        Assert.fail("numberObject double 95523541216159667\nExpected: 9.5523541216159664E16d\nWas: " + ExtendedDecimal.FromString("95523541216159667").ToDouble());
      }
      if (-0.040098447f != ExtendedDecimal.FromString("-400984.46498769274346390686E-7").ToSingle()) {
        Assert.fail("numberObject single -400984.46498769274346390686E-7\nExpected: -0.040098447f\nWas: " + ExtendedDecimal.FromString("-400984.46498769274346390686E-7").ToSingle());
      }
      if (-0.04009844649876927d != ExtendedDecimal.FromString("-400984.46498769274346390686E-7").ToDouble()) {
        Assert.fail("numberObject double -400984.46498769274346390686E-7\nExpected: -0.04009844649876927d\nWas: " + ExtendedDecimal.FromString("-400984.46498769274346390686E-7").ToDouble());
      }
      if (9.9082332E14f != ExtendedDecimal.FromString("990823307532E+3").ToSingle()) {
        Assert.fail("numberObject single 990823307532E+3\nExpected: 9.9082332E14f\nWas: " + ExtendedDecimal.FromString("990823307532E+3").ToSingle());
      }
      if (9.90823307532E14d != ExtendedDecimal.FromString("990823307532E+3").ToDouble()) {
        Assert.fail("numberObject double 990823307532E+3\nExpected: 9.90823307532E14d\nWas: " + ExtendedDecimal.FromString("990823307532E+3").ToDouble());
      }
      if (-8.969879E8f != ExtendedDecimal.FromString("-896987890").ToSingle()) {
        Assert.fail("numberObject single -896987890\nExpected: -8.969879E8f\nWas: " + ExtendedDecimal.FromString("-896987890").ToSingle());
      }
      if (-8.9698789E8d != ExtendedDecimal.FromString("-896987890").ToDouble()) {
        Assert.fail("numberObject double -896987890\nExpected: -8.9698789E8d\nWas: " + ExtendedDecimal.FromString("-896987890").ToDouble());
      }
      if (-5.1842734E9f != ExtendedDecimal.FromString("-5184273642.760").ToSingle()) {
        Assert.fail("numberObject single -5184273642.760\nExpected: -5.1842734E9f\nWas: " + ExtendedDecimal.FromString("-5184273642.760").ToSingle());
      }
      if (-5.18427364276E9d != ExtendedDecimal.FromString("-5184273642.760").ToDouble()) {
        Assert.fail("numberObject double -5184273642.760\nExpected: -5.18427364276E9d\nWas: " + ExtendedDecimal.FromString("-5184273642.760").ToDouble());
      }
      if (5.03393772E17f != ExtendedDecimal.FromString("503393788336283974").ToSingle()) {
        Assert.fail("numberObject single 503393788336283974\nExpected: 5.03393772E17f\nWas: " + ExtendedDecimal.FromString("503393788336283974").ToSingle());
      }
      if (5.0339378833628397E17d != ExtendedDecimal.FromString("503393788336283974").ToDouble()) {
        Assert.fail("numberObject double 503393788336283974\nExpected: 5.0339378833628397E17d\nWas: " + ExtendedDecimal.FromString("503393788336283974").ToDouble());
      }
      if (-5.50587E15f != ExtendedDecimal.FromString("-550587E+10").ToSingle()) {
        Assert.fail("numberObject single -550587E+10\nExpected: -5.50587E15f\nWas: " + ExtendedDecimal.FromString("-550587E+10").ToSingle());
      }
      if (-5.50587E15d != ExtendedDecimal.FromString("-550587E+10").ToDouble()) {
        Assert.fail("numberObject double -550587E+10\nExpected: -5.50587E15d\nWas: " + ExtendedDecimal.FromString("-550587E+10").ToDouble());
      }
      if (-4.0559753E-5f != ExtendedDecimal.FromString("-405597523930.814E-16").ToSingle()) {
        Assert.fail("numberObject single -405597523930.814E-16\nExpected: -4.0559753E-5f\nWas: " + ExtendedDecimal.FromString("-405597523930.814E-16").ToSingle());
      }
      if (-4.05597523930814E-5d != ExtendedDecimal.FromString("-405597523930.814E-16").ToDouble()) {
        Assert.fail("numberObject double -405597523930.814E-16\nExpected: -4.05597523930814E-5d\nWas: " + ExtendedDecimal.FromString("-405597523930.814E-16").ToDouble());
      }
      if (-5.326398E9f != ExtendedDecimal.FromString("-5326397977").ToSingle()) {
        Assert.fail("numberObject single -5326397977\nExpected: -5.326398E9f\nWas: " + ExtendedDecimal.FromString("-5326397977").ToSingle());
      }
      if (-5.326397977E9d != ExtendedDecimal.FromString("-5326397977").ToDouble()) {
        Assert.fail("numberObject double -5326397977\nExpected: -5.326397977E9d\nWas: " + ExtendedDecimal.FromString("-5326397977").ToDouble());
      }
      if (-9997.447f != ExtendedDecimal.FromString("-9997.44701170").ToSingle()) {
        Assert.fail("numberObject single -9997.44701170\nExpected: -9997.447f\nWas: " + ExtendedDecimal.FromString("-9997.44701170").ToSingle());
      }
      if (-9997.4470117d != ExtendedDecimal.FromString("-9997.44701170").ToDouble()) {
        Assert.fail("numberObject double -9997.44701170\nExpected: -9997.4470117d\nWas: " + ExtendedDecimal.FromString("-9997.44701170").ToDouble());
      }
      if (7.3258664E7f != ExtendedDecimal.FromString("73258664.23970751611061").ToSingle()) {
        Assert.fail("numberObject single 73258664.23970751611061\nExpected: 7.3258664E7f\nWas: " + ExtendedDecimal.FromString("73258664.23970751611061").ToSingle());
      }
      if (7.325866423970751E7d != ExtendedDecimal.FromString("73258664.23970751611061").ToDouble()) {
        Assert.fail("numberObject double 73258664.23970751611061\nExpected: 7.325866423970751E7d\nWas: " + ExtendedDecimal.FromString("73258664.23970751611061").ToDouble());
      }
      if (-7.9944785E13f != ExtendedDecimal.FromString("-79944788804361.667255656660").ToSingle()) {
        Assert.fail("numberObject single -79944788804361.667255656660\nExpected: -7.9944785E13f\nWas: " + ExtendedDecimal.FromString("-79944788804361.667255656660").ToSingle());
      }
      if (-7.994478880436167E13d != ExtendedDecimal.FromString("-79944788804361.667255656660").ToDouble()) {
        Assert.fail("numberObject double -79944788804361.667255656660\nExpected: -7.994478880436167E13d\nWas: " + ExtendedDecimal.FromString("-79944788804361.667255656660").ToDouble());
      }
      if (9.852337E19f != ExtendedDecimal.FromString("98523363000987953313E0").ToSingle()) {
        Assert.fail("numberObject single 98523363000987953313E0\nExpected: 9.852337E19f\nWas: " + ExtendedDecimal.FromString("98523363000987953313E0").ToSingle());
      }
      if (9.852336300098796E19d != ExtendedDecimal.FromString("98523363000987953313E0").ToDouble()) {
        Assert.fail("numberObject double 98523363000987953313E0\nExpected: 9.852336300098796E19d\nWas: " + ExtendedDecimal.FromString("98523363000987953313E0").ToDouble());
      }
      if (5.981638E15f != ExtendedDecimal.FromString("5981637941716431.55749471240993").ToSingle()) {
        Assert.fail("numberObject single 5981637941716431.55749471240993\nExpected: 5.981638E15f\nWas: " + ExtendedDecimal.FromString("5981637941716431.55749471240993").ToSingle());
      }
      if (5.981637941716432E15d != ExtendedDecimal.FromString("5981637941716431.55749471240993").ToDouble()) {
        Assert.fail("numberObject double 5981637941716431.55749471240993\nExpected: 5.981637941716432E15d\nWas: " + ExtendedDecimal.FromString("5981637941716431.55749471240993").ToDouble());
      }
      if (-1.995E-9f != ExtendedDecimal.FromString("-1995E-12").ToSingle()) {
        Assert.fail("numberObject single -1995E-12\nExpected: -1.995E-9f\nWas: " + ExtendedDecimal.FromString("-1995E-12").ToSingle());
      }
      if (-1.995E-9d != ExtendedDecimal.FromString("-1995E-12").ToDouble()) {
        Assert.fail("numberObject double -1995E-12\nExpected: -1.995E-9d\nWas: " + ExtendedDecimal.FromString("-1995E-12").ToDouble());
      }
      if (2.59017677E9f != ExtendedDecimal.FromString("2590176810").ToSingle()) {
        Assert.fail("numberObject single 2590176810\nExpected: 2.59017677E9f\nWas: " + ExtendedDecimal.FromString("2590176810").ToSingle());
      }
      if (2.59017681E9d != ExtendedDecimal.FromString("2590176810").ToDouble()) {
        Assert.fail("numberObject double 2590176810\nExpected: 2.59017681E9d\nWas: " + ExtendedDecimal.FromString("2590176810").ToDouble());
      }
      if (2.9604614f != ExtendedDecimal.FromString("2.960461297").ToSingle()) {
        Assert.fail("numberObject single 2.960461297\nExpected: 2.9604614f\nWas: " + ExtendedDecimal.FromString("2.960461297").ToSingle());
      }
      if (2.960461297d != ExtendedDecimal.FromString("2.960461297").ToDouble()) {
        Assert.fail("numberObject double 2.960461297\nExpected: 2.960461297d\nWas: " + ExtendedDecimal.FromString("2.960461297").ToDouble());
      }
      if (768802.0f != ExtendedDecimal.FromString("768802").ToSingle()) {
        Assert.fail("numberObject single 768802\nExpected: 768802.0f\nWas: " + ExtendedDecimal.FromString("768802").ToSingle());
      }
      if (768802.0d != ExtendedDecimal.FromString("768802").ToDouble()) {
        Assert.fail("numberObject double 768802\nExpected: 768802.0d\nWas: " + ExtendedDecimal.FromString("768802").ToDouble());
      }
      if (145417.38f != ExtendedDecimal.FromString("145417.373").ToSingle()) {
        Assert.fail("numberObject single 145417.373\nExpected: 145417.38f\nWas: " + ExtendedDecimal.FromString("145417.373").ToSingle());
      }
      if (145417.373d != ExtendedDecimal.FromString("145417.373").ToDouble()) {
        Assert.fail("numberObject double 145417.373\nExpected: 145417.373d\nWas: " + ExtendedDecimal.FromString("145417.373").ToDouble());
      }
      if (540905.0f != ExtendedDecimal.FromString("540905").ToSingle()) {
        Assert.fail("numberObject single 540905\nExpected: 540905.0f\nWas: " + ExtendedDecimal.FromString("540905").ToSingle());
      }
      if (540905.0d != ExtendedDecimal.FromString("540905").ToDouble()) {
        Assert.fail("numberObject double 540905\nExpected: 540905.0d\nWas: " + ExtendedDecimal.FromString("540905").ToDouble());
      }
      if (-6.811958E20f != ExtendedDecimal.FromString("-681.1958019894E+18").ToSingle()) {
        Assert.fail("numberObject single -681.1958019894E+18\nExpected: -6.811958E20f\nWas: " + ExtendedDecimal.FromString("-681.1958019894E+18").ToSingle());
      }
      if (-6.811958019894E20d != ExtendedDecimal.FromString("-681.1958019894E+18").ToDouble()) {
        Assert.fail("numberObject double -681.1958019894E+18\nExpected: -6.811958019894E20d\nWas: " + ExtendedDecimal.FromString("-681.1958019894E+18").ToDouble());
      }
      if (54846.0f != ExtendedDecimal.FromString("54846.0").ToSingle()) {
        Assert.fail("numberObject single 54846.0\nExpected: 54846.0f\nWas: " + ExtendedDecimal.FromString("54846.0").ToSingle());
      }
      if (54846.0d != ExtendedDecimal.FromString("54846.0").ToDouble()) {
        Assert.fail("numberObject double 54846.0\nExpected: 54846.0d\nWas: " + ExtendedDecimal.FromString("54846.0").ToDouble());
      }
      if (9.7245E9f != ExtendedDecimal.FromString("97245E+5").ToSingle()) {
        Assert.fail("numberObject single 97245E+5\nExpected: 9.7245E9f\nWas: " + ExtendedDecimal.FromString("97245E+5").ToSingle());
      }
      if (9.7245E9d != ExtendedDecimal.FromString("97245E+5").ToDouble()) {
        Assert.fail("numberObject double 97245E+5\nExpected: 9.7245E9d\nWas: " + ExtendedDecimal.FromString("97245E+5").ToDouble());
      }
      if (-26.0f != ExtendedDecimal.FromString("-26").ToSingle()) {
        Assert.fail("numberObject single -26\nExpected: -26.0f\nWas: " + ExtendedDecimal.FromString("-26").ToSingle());
      }
      if (-26.0d != ExtendedDecimal.FromString("-26").ToDouble()) {
        Assert.fail("numberObject double -26\nExpected: -26.0d\nWas: " + ExtendedDecimal.FromString("-26").ToDouble());
      }
      if (4.15749164E12f != ExtendedDecimal.FromString("4157491532482.05").ToSingle()) {
        Assert.fail("numberObject single 4157491532482.05\nExpected: 4.15749164E12f\nWas: " + ExtendedDecimal.FromString("4157491532482.05").ToSingle());
      }
      if (4.15749153248205E12d != ExtendedDecimal.FromString("4157491532482.05").ToDouble()) {
        Assert.fail("numberObject double 4157491532482.05\nExpected: 4.15749153248205E12d\nWas: " + ExtendedDecimal.FromString("4157491532482.05").ToDouble());
      }
      if (4.7747967E15f != ExtendedDecimal.FromString("4774796769101.23808389660855287E+3").ToSingle()) {
        Assert.fail("numberObject single 4774796769101.23808389660855287E+3\nExpected: 4.7747967E15f\nWas: " + ExtendedDecimal.FromString("4774796769101.23808389660855287E+3").ToSingle());
      }
      if (4.774796769101238E15d != ExtendedDecimal.FromString("4774796769101.23808389660855287E+3").ToDouble()) {
        Assert.fail("numberObject double 4774796769101.23808389660855287E+3\nExpected: 4.774796769101238E15d\nWas: " + ExtendedDecimal.FromString("4774796769101.23808389660855287E+3").ToDouble());
      }
      if (-9.8263879E14f != ExtendedDecimal.FromString("-982638781021905").ToSingle()) {
        Assert.fail("numberObject single -982638781021905\nExpected: -9.8263879E14f\nWas: " + ExtendedDecimal.FromString("-982638781021905").ToSingle());
      }
      if (-9.82638781021905E14d != ExtendedDecimal.FromString("-982638781021905").ToDouble()) {
        Assert.fail("numberObject double -982638781021905\nExpected: -9.82638781021905E14d\nWas: " + ExtendedDecimal.FromString("-982638781021905").ToDouble());
      }
      if (8.8043432E18f != ExtendedDecimal.FromString("8804343287262864743").ToSingle()) {
        Assert.fail("numberObject single 8804343287262864743\nExpected: 8.8043432E18f\nWas: " + ExtendedDecimal.FromString("8804343287262864743").ToSingle());
      }
      if (8.8043432872628644E18d != ExtendedDecimal.FromString("8804343287262864743").ToDouble()) {
        Assert.fail("numberObject double 8804343287262864743\nExpected: 8.8043432872628644E18d\nWas: " + ExtendedDecimal.FromString("8804343287262864743").ToDouble());
      }
      if (-6.5138669E13f != ExtendedDecimal.FromString("-65138668135711").ToSingle()) {
        Assert.fail("numberObject single -65138668135711\nExpected: -6.5138669E13f\nWas: " + ExtendedDecimal.FromString("-65138668135711").ToSingle());
      }
      if (-6.5138668135711E13d != ExtendedDecimal.FromString("-65138668135711").ToDouble()) {
        Assert.fail("numberObject double -65138668135711\nExpected: -6.5138668135711E13d\nWas: " + ExtendedDecimal.FromString("-65138668135711").ToDouble());
      }
      if (-5.9235733E15f != ExtendedDecimal.FromString("-5923573055061163").ToSingle()) {
        Assert.fail("numberObject single -5923573055061163\nExpected: -5.9235733E15f\nWas: " + ExtendedDecimal.FromString("-5923573055061163").ToSingle());
      }
      if (-5.923573055061163E15d != ExtendedDecimal.FromString("-5923573055061163").ToDouble()) {
        Assert.fail("numberObject double -5923573055061163\nExpected: -5.923573055061163E15d\nWas: " + ExtendedDecimal.FromString("-5923573055061163").ToDouble());
      }
      if (-8.6853E-8f != ExtendedDecimal.FromString("-8.6853E-8").ToSingle()) {
        Assert.fail("numberObject single -8.6853E-8\nExpected: -8.6853E-8f\nWas: " + ExtendedDecimal.FromString("-8.6853E-8").ToSingle());
      }
      if (-8.6853E-8d != ExtendedDecimal.FromString("-8.6853E-8").ToDouble()) {
        Assert.fail("numberObject double -8.6853E-8\nExpected: -8.6853E-8d\nWas: " + ExtendedDecimal.FromString("-8.6853E-8").ToDouble());
      }
      if (19707.0f != ExtendedDecimal.FromString("19707").ToSingle()) {
        Assert.fail("numberObject single 19707\nExpected: 19707.0f\nWas: " + ExtendedDecimal.FromString("19707").ToSingle());
      }
      if (19707.0d != ExtendedDecimal.FromString("19707").ToDouble()) {
        Assert.fail("numberObject double 19707\nExpected: 19707.0d\nWas: " + ExtendedDecimal.FromString("19707").ToDouble());
      }
      if (-8.8478554E14f != ExtendedDecimal.FromString("-884785536200446.1859332080").ToSingle()) {
        Assert.fail("numberObject single -884785536200446.1859332080\nExpected: -8.8478554E14f\nWas: " + ExtendedDecimal.FromString("-884785536200446.1859332080").ToSingle());
      }
      if (-8.847855362004461E14d != ExtendedDecimal.FromString("-884785536200446.1859332080").ToDouble()) {
        Assert.fail("numberObject double -884785536200446.1859332080\nExpected: -8.847855362004461E14d\nWas: " + ExtendedDecimal.FromString("-884785536200446.1859332080").ToDouble());
      }
      if (-1.0f != ExtendedDecimal.FromString("-1").ToSingle()) {
        Assert.fail("numberObject single -1\nExpected: -1.0f\nWas: " + ExtendedDecimal.FromString("-1").ToSingle());
      }
      if (-1.0d != ExtendedDecimal.FromString("-1").ToDouble()) {
        Assert.fail("numberObject double -1\nExpected: -1.0d\nWas: " + ExtendedDecimal.FromString("-1").ToDouble());
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestDecimalFrac() {
      TestCommon.FromBytesTestAB(
        new byte[] {  (byte)0xc4,
        (byte)0x82,
        0x3,
        0x1a,
        1,
        2,
        3,
        4  });
    }
    @Test(expected=CBORException.class)
    public void TestDecimalFracExponentMustNotBeBignum() {
      TestCommon.FromBytesTestAB(
        new byte[] {  (byte)0xc4,
        (byte)0x82,
        (byte)0xc2,
        0x41,
        1,
        0x1a,
        1,
        2,
        3,
        4  });
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestDoubleToOther() {
      CBORObject dbl1 = CBORObject.FromObject((double)Integer.MIN_VALUE);
      CBORObject dbl2 = CBORObject.FromObject((double)Integer.MAX_VALUE);
      try {
        dbl1.AsInt16(); Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        dbl1.AsByte(); Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        dbl1.AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        dbl1.AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        dbl1.AsBigInteger();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        dbl2.AsInt16(); Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        dbl2.AsByte(); Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        dbl2.AsInt32();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        dbl2.AsInt64();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        dbl2.AsBigInteger();
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestBigTag() {
      CBORObject.FromObjectAndTag(CBORObject.Null, (BigInteger.ONE.shiftLeft(64)).subtract(BigInteger.ONE));
    }
    @Test(expected=CBORException.class)
    public void TestDecimalFracExactlyTwoElements() {
      TestCommon.FromBytesTestAB(
        new byte[] {  (byte)0xc4,
        (byte)0x82,
        (byte)0xc2,
        0x41,
        1  });
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestDecimalFracMantissaMayBeBignum() {
      CBORObject o = TestCommon.FromBytesTestAB(
        new byte[] {  (byte)0xc4,
        (byte)0x82,
        0x3,
        (byte)0xc2,
        0x41,
        1  });
      Assert.assertEquals(
        ExtendedDecimal.Create(BigInteger.ONE, BigInteger.valueOf(3)),
        o.AsExtendedDecimal());
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestShort() {
      for (int i = Short.MIN_VALUE; i <= Short.MAX_VALUE; ++i) {
        TestCommon.AssertSer(
          CBORObject.FromObject((short)i),
          String.format(java.util.Locale.US,"%s", i));
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestByteArray() {
      TestCommon.AssertSer(
        CBORObject.FromObject(new byte[] {  0x20, 0x78  }),
        "h'2078'");
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestBigNumBytes() {
      CBORObject o = null;
      o = TestCommon.FromBytesTestAB(new byte[] {  (byte)0xc2, 0x41, (byte)0x88  });
      Assert.assertEquals(BigInteger.valueOf(0x88L), o.AsBigInteger());
      o = TestCommon.FromBytesTestAB(new byte[] {  (byte)0xc2, 0x42, (byte)0x88, 0x77  });
      Assert.assertEquals(BigInteger.valueOf(0x8877L), o.AsBigInteger());
      o = TestCommon.FromBytesTestAB(new byte[] {  (byte)0xc2, 0x44, (byte)0x88, 0x77, 0x66, 0x55  });
      Assert.assertEquals(BigInteger.valueOf(0x88776655L), o.AsBigInteger());
      o = TestCommon.FromBytesTestAB(new byte[] {  (byte)0xc2, 0x47, (byte)0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22  });
      Assert.assertEquals(BigInteger.valueOf(0x88776655443322L), o.AsBigInteger());
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestTaggedUntagged() {
      for (int i = 200; i < 1000; ++i) {
        CBORObject o, o2;
        o = CBORObject.FromObject(0);
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObject(BigInteger.ONE.shiftLeft(100));
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObject(new byte[] {  1, 2, 3  });
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.NewArray();
        o.Add(CBORObject.FromObject(0));
        o.Add(CBORObject.FromObject(1));
        o.Add(CBORObject.FromObject(2));
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.NewMap();
        o.Add("a", CBORObject.FromObject(0));
        o.Add("b", CBORObject.FromObject(1));
        o.Add("c", CBORObject.FromObject(2));
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObject("a");
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.False;
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.True;
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.Null;
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.Undefined;
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
      }
    }

    public void AssertBigIntString(String s, BigInteger bi) {
      Assert.assertEquals(s, bi.toString());
    }

    public void AssertAdd(BigInteger bi, BigInteger bi2, String s) {
      this.AssertBigIntString(s, bi.add(bi2));
      this.AssertBigIntString(s, bi2.add(bi));
      BigInteger negbi = BigInteger.ZERO.subtract(bi);
      BigInteger negbi2 = BigInteger.ZERO.subtract(bi2);
      this.AssertBigIntString(s, bi.subtract(negbi2));
      this.AssertBigIntString(s, bi2.subtract(negbi));
    }

    @Test
    public void TestBigIntAddSub() {
      BigInteger posSmall = BigInteger.valueOf(5);
      BigInteger negSmall=(BigInteger.valueOf(5)).negate();
      BigInteger posLarge = BigInteger.valueOf(5555555);
      BigInteger negLarge=(BigInteger.valueOf(5555555)).negate();
      this.AssertAdd(posSmall, posSmall, "10");
      this.AssertAdd(posSmall, negSmall, "0");
      this.AssertAdd(posSmall, posLarge, "5555560");
      this.AssertAdd(posSmall, negLarge, "-5555550");
      this.AssertAdd(negSmall, negSmall, "-10");
      this.AssertAdd(negSmall, posLarge, "5555550");
      this.AssertAdd(negSmall, negLarge, "-5555560");
      this.AssertAdd(posLarge, posLarge, "11111110");
      this.AssertAdd(posLarge, negLarge, "0");
      this.AssertAdd(negLarge, negLarge, "-11111110");
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestBigInteger() {
      BigInteger bi = BigInteger.valueOf(3);
      this.AssertBigIntString("3", bi);
      BigInteger negseven = BigInteger.valueOf(-7);
      this.AssertBigIntString("-7", negseven);
      BigInteger other = BigInteger.valueOf(-898989);
      this.AssertBigIntString("-898989", other);
      other = BigInteger.valueOf(898989);
      this.AssertBigIntString("898989", other);
      for (int i = 0; i < 500; ++i) {
        TestCommon.AssertSer(
          CBORObject.FromObject(bi),
          String.format(java.util.Locale.US,"%s", bi));
        bi=bi.multiply(negseven);
      }
      BigInteger[] ranges = new BigInteger[] {
        BigInteger.valueOf(Long.MIN_VALUE).subtract(BigInteger.valueOf(512)),
        BigInteger.valueOf(Long.MIN_VALUE).add(BigInteger.valueOf(512)),
        BigInteger.ZERO.subtract(BigInteger.valueOf(512)),
        BigInteger.ZERO.add(BigInteger.valueOf(512)),
        BigInteger.valueOf(Long.MAX_VALUE).subtract(BigInteger.valueOf(512)),
        BigInteger.valueOf(Long.MAX_VALUE).add(BigInteger.valueOf(512)),
        ((BigInteger.ONE.shiftLeft(64)).subtract(BigInteger.ONE)).subtract(BigInteger.valueOf(512)),
        ((BigInteger.ONE.shiftLeft(64)).subtract(BigInteger.ONE)).add(BigInteger.valueOf(512)),
      };
      for (int i = 0; i < ranges.length; i += 2) {
        BigInteger bigintTemp = ranges[i];
        while (true) {
          TestCommon.AssertSer(
            CBORObject.FromObject(bigintTemp),
            String.format(java.util.Locale.US,"%s", bigintTemp));
          if (bigintTemp.equals(ranges[i + 1])) {
            break;
          }
          bigintTemp=bigintTemp.add(BigInteger.ONE);
        }
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestLong() {
      long[] ranges = new long[] {
        -65539, 65539,
        0xFFFFF000L, 0x100000400L,
        Long.MAX_VALUE - 1000, Long.MAX_VALUE,
        Long.MIN_VALUE, Long.MIN_VALUE + 1000
      };
      for (int i = 0; i < ranges.length; i += 2) {
        long j = ranges[i];
        while (true) {
          TestCommon.AssertSer(
            CBORObject.FromObject(j),
            String.format(java.util.Locale.US,"%s", j));
          Assert.assertEquals(
            CBORObject.FromObject(j),
            CBORObject.FromObject(BigInteger.valueOf(j)));
          CBORObject obj = CBORObject.FromJSONString(
            String.format(java.util.Locale.US,"[%s]", j));
          TestCommon.AssertSer(
            obj,
            String.format(java.util.Locale.US,"[%s]", j));
          if (j == ranges[i + 1]) {
            break;
          }
          ++j;
        }
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestFloat() {
      TestCommon.AssertSer(
        CBORObject.FromObject(Float.POSITIVE_INFINITY),
        "Infinity");
      TestCommon.AssertSer(
        CBORObject.FromObject(Float.NEGATIVE_INFINITY),
        "-Infinity");
      TestCommon.AssertSer(
        CBORObject.FromObject(Float.NaN),
        "NaN");
      for (int i = -65539; i <= 65539; ++i) {
        TestCommon.AssertSer(
          CBORObject.FromObject((float)i),
          String.format(java.util.Locale.US,"%s", i));
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestCodePointCompare() {
      Assert.assertEquals(0, ((DataUtilities.CodePointCompare("abc", "abc")==0) ? 0 : ((DataUtilities.CodePointCompare("abc", "abc")<0) ? -1 : 1)));
      Assert.assertEquals(0, ((DataUtilities.CodePointCompare("\ud800\udc00", "\ud800\udc00")==0) ? 0 : ((DataUtilities.CodePointCompare("\ud800\udc00", "\ud800\udc00")<0) ? -1 : 1)));
      Assert.assertEquals(-1, ((DataUtilities.CodePointCompare("abc", "\ud800\udc00")==0) ? 0 : ((DataUtilities.CodePointCompare("abc", "\ud800\udc00")<0) ? -1 : 1)));
      Assert.assertEquals(-1, ((DataUtilities.CodePointCompare("\uf000", "\ud800\udc00")==0) ? 0 : ((DataUtilities.CodePointCompare("\uf000", "\ud800\udc00")<0) ? -1 : 1)));
      Assert.assertEquals(1, ((DataUtilities.CodePointCompare("\uf000", "\ud800")==0) ? 0 : ((DataUtilities.CodePointCompare("\uf000", "\ud800")<0) ? -1 : 1)));
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestSimpleValues() {
      TestCommon.AssertSer(
        CBORObject.FromObject(true),
        "true");
      TestCommon.AssertSer(
        CBORObject.FromObject(false),
        "false");
      TestCommon.AssertSer(
        CBORObject.FromObject((Object)null),
        "null");
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestGetUtf8Length() {
      try {
        DataUtilities.GetUtf8Length(null, true);
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      try {
        DataUtilities.GetUtf8Length(null, false);
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString()); throw new IllegalStateException("", ex);
      }
      Assert.assertEquals(3, DataUtilities.GetUtf8Length("abc", true));
      Assert.assertEquals(4, DataUtilities.GetUtf8Length("\u0300\u0300", true));
      Assert.assertEquals(6, DataUtilities.GetUtf8Length("\u3000\u3000", true));
      Assert.assertEquals(6, DataUtilities.GetUtf8Length("\ud800\ud800", true));
      Assert.assertEquals(-1, DataUtilities.GetUtf8Length("\ud800\ud800", false));
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestDouble() {
      if (!CBORObject.FromObject(Double.POSITIVE_INFINITY).IsPositiveInfinity()) {
        Assert.fail("Not positive infinity");
      }
      TestCommon.AssertSer(
        CBORObject.FromObject(Double.POSITIVE_INFINITY),
        "Infinity");
      TestCommon.AssertSer(
        CBORObject.FromObject(Double.NEGATIVE_INFINITY),
        "-Infinity");
      TestCommon.AssertSer(
        CBORObject.FromObject(Double.NaN),
        "NaN");
      CBORObject oldobj = null;
      for (int i = -65539; i <= 65539; ++i) {
        CBORObject o = CBORObject.FromObject((double)i);
        TestCommon.AssertSer(
          o,
          String.format(java.util.Locale.US,"%s", i));
        if (oldobj != null) {
          CompareTestLess(oldobj, o);
        }
        oldobj = o;
      }
    }

    /**
     * Not documented yet.
     */
    @Test
    public void TestTags() {
      BigInteger maxuint = (BigInteger.ONE.shiftLeft(64)).subtract(BigInteger.ONE);
      BigInteger[] ranges = new BigInteger[] {
        BigInteger.valueOf(6),
        BigInteger.valueOf(65539),
        BigInteger.valueOf(Integer.MAX_VALUE).subtract(BigInteger.valueOf(500)),
        BigInteger.valueOf(Integer.MAX_VALUE).add(BigInteger.valueOf(500)),
        BigInteger.valueOf(Long.MAX_VALUE).subtract(BigInteger.valueOf(500)),
        BigInteger.valueOf(Long.MAX_VALUE).add(BigInteger.valueOf(500)),
        ((BigInteger.ONE.shiftLeft(64)).subtract(BigInteger.ONE)).subtract(BigInteger.valueOf(500)),
        maxuint,
      };
      for (int i = 0; i < ranges.length; i += 2) {
        BigInteger bigintTemp = ranges[i];
        while (true) {
          CBORObject obj = CBORObject.FromObjectAndTag(0, bigintTemp);
          if(!(obj.isTagged()))Assert.fail( "obj not tagged");
          BigInteger[] tags = obj.GetTags();
          Assert.assertEquals(1, tags.length);
          Assert.assertEquals(bigintTemp, tags[0]);
          if (!obj.getInnermostTag().equals(bigintTemp)) {
            Assert.assertEquals(String.format(java.util.Locale.US,"obj tag doesn't match: %s", obj),bigintTemp,obj.getInnermostTag());
          }
          TestCommon.AssertSer(
            obj,
            String.format(java.util.Locale.US,"%s(0)", bigintTemp));
          if (!bigintTemp.equals(maxuint)) {
            // Test multiple tags
            CBORObject obj2 = CBORObject.FromObjectAndTag(obj, bigintTemp .add(BigInteger.ONE));
            BigInteger[] bi = obj2.GetTags();
            if (bi.length != 2) {
              Assert.assertEquals(String.format(java.util.Locale.US,"Expected 2 tags: %s", obj2),2,bi.length);
            }
            if (!bi[0].equals((BigInteger)bigintTemp .add(BigInteger.ONE))) {
              Assert.assertEquals(String.format(java.util.Locale.US,"Outer tag doesn't match: %s", obj2),bigintTemp .add(BigInteger.ONE),bi[0]);
            }
            if (!bi[1].equals(bigintTemp)) {
              Assert.assertEquals(String.format(java.util.Locale.US,"Inner tag doesn't match: %s", obj2),bigintTemp,bi[1]);
            }
            if (!obj2.getInnermostTag().equals(bigintTemp)) {
              Assert.assertEquals(String.format(java.util.Locale.US,"Innermost tag doesn't match: %s", obj2),bigintTemp,obj2.getInnermostTag());
            }
            String str = String.format(java.util.Locale.US,"%s(%s(0))",
                bigintTemp .add(BigInteger.ONE),
                bigintTemp);
            TestCommon.AssertSer(
              obj2,
              str);
          }
          if (bigintTemp.equals(ranges[i + 1])) {
            break;
          }
          bigintTemp=bigintTemp.add(BigInteger.ONE);
        }
      }
    }
  }

