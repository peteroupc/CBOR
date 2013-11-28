package com.upokecenter.test;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */


import java.math.*;
import java.io.*;
import com.upokecenter.util.*;

import org.junit.Assert;
import org.junit.Test;

    /**
     * 
     * @param r A FastRandom object.
     */

  public class CBORTest{
    private static void TestBigFloatDoubleCore(double d, String s) {
      double oldd = d;
      BigFloat bf = BigFloat.FromDouble(d);
      if (s != null) {
        Assert.assertEquals(s, bf.toString());
      }
      d = bf.ToDouble();
      Assert.assertEquals((double)oldd,d,0);
      TestCommon.AssertRoundTrip(CBORObject.FromObject(bf));
      TestCommon.AssertRoundTrip(CBORObject.FromObject(d));
    }
    private static void TestBigFloatSingleCore(float d, String s) {
      float oldd = d;
      BigFloat bf = BigFloat.FromSingle(d);
      if (s != null) {
        Assert.assertEquals(s, bf.toString());
      }
      d = bf.ToSingle();
      Assert.assertEquals((float)oldd,d,0f);
      TestCommon.AssertRoundTrip(CBORObject.FromObject(bf));
      TestCommon.AssertRoundTrip(CBORObject.FromObject(d));
    }
    private static CBORObject RandomNumber(FastRandom rand) {
      switch (rand.NextValue(6)) {
        case 0:
          return CBORObject.FromObject(RandomDouble(rand, Integer.MAX_VALUE));
        case 1:
          return CBORObject.FromObject(RandomSingle(rand, Integer.MAX_VALUE));
        case 2:
          return CBORObject.FromObject(RandomBigInteger(rand));
        case 3:
          return CBORObject.FromObject(RandomBigFloat(rand));
        case 4:
          return CBORObject.FromObject(RandomDecimalFraction(rand));
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
      if (exponent == Integer.MAX_VALUE)
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
      return Double.longBitsToDouble(r);
    }
    private static float RandomSingle(FastRandom rand, int exponent) {
      if (exponent == Integer.MAX_VALUE)
        exponent = rand.NextValue(255);
      int r = rand.NextValue(0x10000);
      if (rand.NextValue(2) == 0) {
        r |= ((int)rand.NextValue(0x10000)) << 16;
      }
      r &= ~0x7F800000; // clear exponent
      r |= ((int)exponent) << 23; // set exponent
      return Float.intBitsToFloat(r);
    }
    public static DecimalFraction RandomDecimalFraction(FastRandom r) {
      return DecimalFraction.FromString(RandomDecimalString(r));
    }
    public static BigInteger RandomBigInteger(FastRandom r) {
      return new BigInteger(RandomBigIntString(r));
    }
    public static BigFloat RandomBigFloat(FastRandom r) {
      return new BigFloat(RandomBigInteger(r),r.NextValue(400)-200);
    }
    public static String RandomBigIntString(FastRandom r) {
      int count = r.NextValue(50) + 1;
      StringBuilder sb = new StringBuilder();
      if (r.NextValue(2) == 0) sb.append('-');
      for (int i = 0; i < count; i++) {
        if (i == 0)
          sb.append((char)('1' + r.NextValue(9)));
        else
          sb.append((char)('0' + r.NextValue(10)));
      }
      return sb.toString();
    }
    public static String RandomDecimalString(FastRandom r) {
      int count = r.NextValue(20) + 1;
      StringBuilder sb = new StringBuilder();
      if (r.NextValue(2) == 0) sb.append('-');
      for (int i = 0; i < count; i++) {
        if (i == 0)
          sb.append((char)('1' + r.NextValue(9)));
        else
          sb.append((char)('0' + r.NextValue(10)));
      }
      if (r.NextValue(2) == 0) {
        sb.append('.');
        count = r.NextValue(20) + 1;
        for (int i = 0; i < count; i++) {
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
      CBORObject o = CBORObject.FromObject(DecimalFraction.FromString(r));
      CBORObject o2 = CBORDataUtilities.ParseJSONNumber(r);
      CompareTestEqual(o,o2);
    }
    
    /**
     * 
     */
@Test
    public void TestAdd() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 3000; i++) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        DecimalFraction cmpDecFrac = o1.AsDecimalFraction().Add(o2.AsDecimalFraction());
        DecimalFraction cmpCobj = CBORObject.Addition(o1,o2).AsDecimalFraction();
        if (cmpDecFrac.compareTo(cmpCobj)!=0) {
          Assert.assertEquals("Results don't match:\n" + o1.toString() + " and\n" + o2.toString() + "\nOR\n" +
                          o1.AsDecimalFraction().toString() + " and\n" + o2.AsDecimalFraction().toString(),0,cmpDecFrac.compareTo(cmpCobj));
        }
        TestCommon.AssertRoundTrip(o1);
        TestCommon.AssertRoundTrip(o2);
      }
    }
    /**
     * 
     */
@Test
    public void TestSubtract() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 3000; i++) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        DecimalFraction cmpDecFrac = o1.AsDecimalFraction().Subtract(o2.AsDecimalFraction());
        DecimalFraction cmpCobj = CBORObject.Subtract(o1,o2).AsDecimalFraction();
        if (cmpDecFrac.compareTo(cmpCobj)!=0) {
          Assert.assertEquals("Results don't match:\n" + o1.toString() + " and\n" + o2.toString() + "\nOR\n" +
                          o1.AsDecimalFraction().toString() + " and\n" + o2.AsDecimalFraction().toString(),0,cmpDecFrac.compareTo(cmpCobj));
        }
        TestCommon.AssertRoundTrip(o1);
        TestCommon.AssertRoundTrip(o2);
      }
    }
    private static String ObjectMessages(CBORObject o1, CBORObject o2, String s) {
      if(o1.getType()== CBORType.Number && o2.getType()== CBORType.Number){
        return s+":\n" + o1.toString() + " and\n" + o2.toString()+"\nOR\n"+
          o1.AsDecimalFraction().toString() + " and\n" + o2.AsDecimalFraction().toString();
      } else {
        return s+":\n" + o1.toString() + " and\n" + o2.toString();
      }
    }
    
    private static void CompareTestEqual(CBORObject o1, CBORObject o2) {
      if(CompareTestReciprocal(o1,o2)!=0)
        Assert.fail(ObjectMessages(o1,o2,"Not equal: "+CompareTestReciprocal(o1,o2)));
    }
    private static void CompareTestLess(CBORObject o1, CBORObject o2) {
      if(CompareTestReciprocal(o1,o2)>=0)
        Assert.fail(ObjectMessages(o1,o2,"Not less: "+CompareTestReciprocal(o1,o2)));
    }
    
    private static int CompareTestReciprocal(CBORObject o1, CBORObject o2) {
      if((o1)==null)throw new NullPointerException("o1");
      if((o2)==null)throw new NullPointerException("o2");
      int cmp=o1.compareTo(o2);
      int cmp2=o2.compareTo(o1);
      if(-cmp2!=cmp){
        Assert.assertEquals(ObjectMessages(o1,o2,"Not reciprocal"),cmp,-cmp2);
      }
      return cmp;
    }
    
    /**
     * 
     */
@Test
    public void TestCompare() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 3000; i++) {
        CBORObject o1 = RandomNumber(r);
        CBORObject o2 = RandomNumber(r);
        int cmpDecFrac = o1.AsDecimalFraction().compareTo(o2.AsDecimalFraction());
        int cmpCobj = CompareTestReciprocal(o1,o2);
        if (cmpDecFrac != cmpCobj) {
          Assert.assertEquals(ObjectMessages(o1,o2,"Results don't match"),cmpDecFrac,cmpCobj);
        }
        TestCommon.AssertRoundTrip(o1);
        TestCommon.AssertRoundTrip(o2);
      }
      for (int i = 0; i < 50; i++) {
        CBORObject o1 = CBORObject.FromObject(Float.NEGATIVE_INFINITY);
        CBORObject o2 = RandomNumber(r);
        CompareTestLess(o1,o2);
        o1=CBORObject.FromObject(Double.NEGATIVE_INFINITY);
        CompareTestLess(o1,o2);
        o1=CBORObject.FromObject(Float.POSITIVE_INFINITY);
        CompareTestLess(o2,o1);
        o1=CBORObject.FromObject(Double.POSITIVE_INFINITY);
        CompareTestLess(o2,o1);
        o1=CBORObject.FromObject(Float.NaN);
        CompareTestLess(o2,o1);
        o1=CBORObject.FromObject(Double.NaN);
        CompareTestLess(o2,o1);
      }
      CBORObject sp=CBORObject.FromObject(Float.POSITIVE_INFINITY);
      CBORObject sn=CBORObject.FromObject(Float.NEGATIVE_INFINITY);
      CBORObject snan=CBORObject.FromObject(Float.NaN);
      CBORObject dp=CBORObject.FromObject(Double.POSITIVE_INFINITY);
      CBORObject dn=CBORObject.FromObject(Double.NEGATIVE_INFINITY);
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
    /**
     * 
     */
@Test
    public void TestParseDecimalStrings() {
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 3000; i++) {
        String r = RandomDecimalString(rand);
        TestDecimalString(r);
      }
    }
    /**
     * 
     */
@Test
    public void TestRandomData() {
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 200; i++) {
        byte[] array = new byte[rand.NextValue(1000000) + 1];
        for (int j = 0; j < array.length; j++) {
          if (j + 3 <= array.length) {
            int r = rand.NextValue(0x1000000);
            array[j] = (byte)((r) & 0xFF);
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
              if (o == null) Assert.fail("Object read is null");
            } catch(CBORException ex) {
              // Expected exception
            }
          }
}
finally {
try { if(ms!=null)ms.close(); } catch(IOException ex){}
}
      }
    }
    /**
     * 
     */
@Test
    public void TestBigFloatSingle() {
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 255; i++) { // Try a random float with a given exponent
        TestBigFloatSingleCore(RandomSingle(rand, i), null);
        TestBigFloatSingleCore(RandomSingle(rand, i), null);
        TestBigFloatSingleCore(RandomSingle(rand, i), null);
        TestBigFloatSingleCore(RandomSingle(rand, i), null);
      }
    }
    /**
     * 
     */
@Test
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
    @Test(expected=CBORException.class)
    public void TestTagThenBreak() {
      TestCommon.FromBytesTestAB(new byte[]{ (byte)0xD1, (byte)0xFF });
    }
    
    /**
     * 
     */
@Test
    public void TestJSONSurrogates() {
      try { CBORObject.FromJSONString("[\"\ud800\udc00\"]"); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromJSONString("[\"\\ud800\\udc00\"]"); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromJSONString("[\"\ud800\\udc00\"]"); } catch(CBORException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromJSONString("[\"\\ud800\udc00\"]"); } catch(CBORException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromJSONString("[\"\\udc00\ud800\udc00\"]"); } catch(CBORException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromJSONString("[\"\\ud800\ud800\udc00\"]"); } catch(CBORException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromJSONString("[\"\\ud800\\udc00\ud800\udc00\"]"); } catch(Exception ex){ Assert.fail(ex.toString()); }
    }
    /**
     * 
     */
@Test
    public void TestJSONEscapedChars() {
      CBORObject o = CBORObject.FromJSONString(
        "[\"\\r\\n\\u0006\\\\\\\"\"]");
      Assert.assertEquals(1, o.size());
      Assert.assertEquals("\r\n\u0006\\\"", o.get(0).AsString());
      Assert.assertEquals("[\"\\r\\n\\u0006\\\\\\\"\"]",
                      o.ToJSONString());
      TestCommon.AssertRoundTrip(o);
    }
    /**
     * 
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
     * 
     */
@Test
    public void TestJSON() {
      CBORObject o;
      o = CBORObject.FromJSONString("[1,2,3]");
      try { CBORObject.FromJSONString("[\"\\d800\"]"); Assert.fail("Should have failed");} catch(CBORException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromJSONString("[1,2,"); Assert.fail("Should have failed");} catch(CBORException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromJSONString("[1,2,3"); Assert.fail("Should have failed");} catch(CBORException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromJSONString("[\""); Assert.fail("Should have failed");} catch(CBORException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      Assert.assertEquals(3, o.size());
      Assert.assertEquals(1, o.get(0).AsInt32());
      Assert.assertEquals(2, o.get(1).AsInt32());
      Assert.assertEquals(3, o.get(2).AsInt32());
      o = CBORObject.FromJSONString("[1.5,2.6,3.7,4.0,222.22]");
      double actual = o.get(0).AsDouble();
      Assert.assertEquals((double)1.5,actual,0);
      Assert.assertEquals("true", CBORObject.True.ToJSONString());
      Assert.assertEquals("false", CBORObject.False.ToJSONString());
      Assert.assertEquals("null", CBORObject.Null.ToJSONString());
    }
    /**
     * 
     */
@Test
    public void TestByte() {
      for (int i = 0; i <= 255; i++) {
        TestCommon.AssertSer(
          CBORObject.FromObject((byte)i),
          String.format(java.util.Locale.US,"%s", i));
      }
    }
    /**
     * 
     * @param bytes A byte[] object.
     * @param expectedRet A 32-bit signed integer.
     * @param expectedString A string object.
     * @param noReplaceRet A 32-bit signed integer.
     * @param noReplaceString A string object.
     */
public void DoTestReadUtf8(byte[] bytes,
                               int expectedRet, String expectedString,
                               int noReplaceRet, String noReplaceString
                              ) {
      DoTestReadUtf8(bytes, bytes.length, expectedRet, expectedString,
                     noReplaceRet, noReplaceString);
    }
    /**
     * 
     * @param bytes A byte[] object.
     * @param length A 32-bit signed integer.
     * @param expectedRet A 32-bit signed integer.
     * @param expectedString A string object.
     * @param noReplaceRet A 32-bit signed integer.
     * @param noReplaceString A string object.
     */
public void DoTestReadUtf8(byte[] bytes, int length,
                               int expectedRet, String expectedString,
                               int noReplaceRet, String noReplaceString
                              ) {
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
try { if(ms!=null)ms.close(); } catch(IOException ex){}
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
     * 
     */
@Test
    public void TestDecFracOverflow() {
      try { CBORObject.FromObject(Float.POSITIVE_INFINITY).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Float.NEGATIVE_INFINITY).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Float.NaN).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Double.POSITIVE_INFINITY).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Double.NEGATIVE_INFINITY).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Double.NaN).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Float.POSITIVE_INFINITY).AsBigFloat(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Float.NEGATIVE_INFINITY).AsBigFloat(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Float.NaN).AsBigFloat(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Double.POSITIVE_INFINITY).AsBigFloat(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Double.NEGATIVE_INFINITY).AsBigFloat(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Double.NaN).AsBigFloat(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
    }
    /**
     * 
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
      try { CBORObject.FromObject(Float.POSITIVE_INFINITY).AsBigInteger(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Float.NEGATIVE_INFINITY).AsBigInteger(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Float.NaN).AsBigInteger(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Double.POSITIVE_INFINITY).AsBigInteger(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Double.NEGATIVE_INFINITY).AsBigInteger(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(Double.NaN).AsBigInteger(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
    }
    /**
     * 
     */
@Test
    public void TestDecFracFP() {
      Assert.assertEquals("0.75", DecimalFraction.FromDouble(0.75).toString());
      Assert.assertEquals("0.5", DecimalFraction.FromDouble(0.5).toString());
      Assert.assertEquals("0.25", DecimalFraction.FromDouble(0.25).toString());
      Assert.assertEquals("0.875", DecimalFraction.FromDouble(0.875).toString());
      Assert.assertEquals("0.125", DecimalFraction.FromDouble(0.125).toString());
      Assert.assertEquals("0.75", DecimalFraction.FromSingle(0.75f).toString());
      Assert.assertEquals("0.5", DecimalFraction.FromSingle(0.5f).toString());
      Assert.assertEquals("0.25", DecimalFraction.FromSingle(0.25f).toString());
      Assert.assertEquals("0.875", DecimalFraction.FromSingle(0.875f).toString());
      Assert.assertEquals("0.125", DecimalFraction.FromSingle(0.125f).toString());
    }
    /**
     * 
     */
@Test
    public void ScaleTest() {
      Assert.assertEquals(BigInteger.valueOf(-7), DecimalFraction.FromString("1.265e-4").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-4), DecimalFraction.FromString("0.000E-1").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-16), DecimalFraction.FromString("0.57484848535648e-2").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-22), DecimalFraction.FromString("0.485448e-16").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-20), DecimalFraction.FromString("0.5657575351495151495649565150e+8").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-10), DecimalFraction.FromString("0e-10").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-17), DecimalFraction.FromString("0.504952e-11").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-13), DecimalFraction.FromString("0e-13").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-43), DecimalFraction.FromString("0.49495052535648555757515648e-17").getExponent());
      Assert.assertEquals(BigInteger.valueOf(7), DecimalFraction.FromString("0.485654575150e+19").getExponent());
      Assert.assertEquals(BigInteger.ZERO, DecimalFraction.FromString("0.48515648e+8").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-45), DecimalFraction.FromString("0.49485251485649535552535451544956e-13").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-6), DecimalFraction.FromString("0.565754515152575448505257e+18").getExponent());
      Assert.assertEquals(BigInteger.valueOf(16), DecimalFraction.FromString("0e+16").getExponent());
      Assert.assertEquals(BigInteger.valueOf(6), DecimalFraction.FromString("0.5650e+10").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-5), DecimalFraction.FromString("0.49555554575756575556e+15").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-37), DecimalFraction.FromString("0.57494855545057534955e-17").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-25), DecimalFraction.FromString("0.4956504855525748575456e-3").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-26), DecimalFraction.FromString("0.55575355495654484948525354545053494854e+12").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-22), DecimalFraction.FromString("0.484853575350494950575749545057e+8").getExponent());
      Assert.assertEquals(BigInteger.valueOf(11), DecimalFraction.FromString("0.52545451e+19").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-29), DecimalFraction.FromString("0.48485654495751485754e-9").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-38), DecimalFraction.FromString("0.56525456555549545257535556495655574848e+0").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-15), DecimalFraction.FromString("0.485456485657545752495450554857e+15").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-37), DecimalFraction.FromString("0.485448525554495048e-19").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-29), DecimalFraction.FromString("0.494952485550514953565655e-5").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-8), DecimalFraction.FromString("0.50495454554854505051534950e+18").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-37), DecimalFraction.FromString("0.5156524853575655535351554949525449e-3").getExponent());
      Assert.assertEquals(BigInteger.valueOf(3), DecimalFraction.FromString("0e+3").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-8), DecimalFraction.FromString("0.51505056554957575255555250e+18").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-14), DecimalFraction.FromString("0.5456e-10").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-36), DecimalFraction.FromString("0.494850515656505252555154e-12").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-42), DecimalFraction.FromString("0.535155525253485757525253555749575749e-6").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-29), DecimalFraction.FromString("0.56554952554850525552515549564948e+3").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-40), DecimalFraction.FromString("0.494855545257545656515554495057e-10").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-18), DecimalFraction.FromString("0.5656504948515252555456e+4").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-17), DecimalFraction.FromString("0e-17").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-32), DecimalFraction.FromString("0.55535551515249535049495256e-6").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-31), DecimalFraction.FromString("0.4948534853564853565654514855e-3").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-38), DecimalFraction.FromString("0.5048485057535249555455e-16").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-16), DecimalFraction.FromString("0e-16").getExponent());
      Assert.assertEquals(BigInteger.valueOf(5), DecimalFraction.FromString("0.5354e+9").getExponent());
      Assert.assertEquals(BigInteger.ONE, DecimalFraction.FromString("0.54e+3").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-38), DecimalFraction.FromString("0.4849525755545751574853494948e-10").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-33), DecimalFraction.FromString("0.52514853565252565251565548e-7").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-13), DecimalFraction.FromString("0.575151545652e-1").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-22), DecimalFraction.FromString("0.49515354514852e-8").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-24), DecimalFraction.FromString("0.54535357515356545554e-4").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-11), DecimalFraction.FromString("0.574848e-5").getExponent());
      Assert.assertEquals(BigInteger.valueOf(-3), DecimalFraction.FromString("0.565055e+3").getExponent());
    }
    /**
     * 
     */
@Test
    public void TestReadUtf8() {
      DoTestReadUtf8(new byte[]{ 0x20, 0x20, 0x20 },
                     0, "   ", 0, "   ");
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xc2, (byte)0x80 },
                     0, " \u0080", 0, " \u0080");
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xc2, (byte)0x80, 0x20 },
                     0, " \u0080 ", 0, " \u0080 ");
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xc2, (byte)0x80, (byte)0xc2 },
                     0, " \u0080\ufffd", -1, null);
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xc2, 0x20, 0x20 },
                     0, " \ufffd  ", -1, null);
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xc2, (byte)0xff, 0x20 },
                     0, " \ufffd\ufffd ", -1, null);
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xe0, (byte)0xa0, (byte)0x80 },
                     0, " \u0800", 0, " \u0800");
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xe0, (byte)0xa0, (byte)0x80, 0x20 },
                     0, " \u0800 ", 0, " \u0800 ");
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xf0, (byte)0x90, (byte)0x80, (byte)0x80 },
                     0, " \ud800\udc00", 0, " \ud800\udc00");
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xf0, (byte)0x90, (byte)0x80, (byte)0x80 }, 3,
                     0, " \ufffd", -1, null);
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xf0, (byte)0x90 }, 5,
                     -2, null, -1, null);
      DoTestReadUtf8(new byte[]{ 0x20, 0x20, 0x20 }, 5,
                     -2, null, -2, null);
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xf0, (byte)0x90, (byte)0x80, (byte)0x80, 0x20 },
                     0, " \ud800\udc00 ", 0, " \ud800\udc00 ");
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xf0, (byte)0x90, (byte)0x80, 0x20 },
                     0, " \ufffd ", -1, null);
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xf0, (byte)0x90, 0x20 },
                     0, " \ufffd ", -1, null);
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xf0, (byte)0x90, (byte)0x80, (byte)0xff },
                     0, " \ufffd\ufffd", -1, null);
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xf0, (byte)0x90, (byte)0xff },
                     0, " \ufffd\ufffd", -1, null);
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xe0, (byte)0xa0, 0x20 },
                     0, " \ufffd ", -1, null);
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xe0, 0x20 },
                     0, " \ufffd ", -1, null);
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xe0, (byte)0xa0, (byte)0xff },
                     0, " \ufffd\ufffd", -1, null);
      DoTestReadUtf8(new byte[]{ 0x20, (byte)0xe0, (byte)0xff },
                     0, " \ufffd\ufffd", -1, null);
    }
    private static boolean ByteArrayEquals(byte[] arrayA, byte[] arrayB) {
      if (arrayA == null) return (arrayB == null);
      if (arrayB == null) return false;
      if (arrayA.length != arrayB.length) return false;
      for (int i = 0; i < arrayA.length; i++) {
        if (arrayA[i] != arrayB[i]) return false;
      }
      return true;
    }
    /**
     * 
     */
@Test
    public void TestArray() {
      CBORObject cbor = CBORObject.FromJSONString("[]");
      cbor.Add(CBORObject.FromObject(3));
      cbor.Add(CBORObject.FromObject(4));
      byte[] bytes = cbor.EncodeToBytes();
      boolean isequal = ByteArrayEquals(new byte[]{ (byte)(0x80 | 2), 3, 4 }, bytes);
      if(!(isequal))Assert.fail("array not equal");
    }
    /**
     * 
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
      for (int i = 0; i < num; i++) {
        sb.append(c);
      }
      return sb.toString();
    }
    private static String Repeat(String c, int num) {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < num; i++) {
        sb.append(c);
      }
      return sb.toString();
    }
    
    /**
     * 
     */
@Test
    public void TestTextStringStream() {
      CBORObject cbor = TestCommon.FromBytesTestAB(
        new byte[]{ 0x7F, 0x61, 0x20, 0x61, 0x20, (byte)0xFF });
      Assert.assertEquals("  ", cbor.AsString());
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
        new byte[]{ 0x7F, 0x61, 0x20, (byte)0xC0, 0x61, 0x20, (byte)0xFF });
    }
    @Test(expected=CBORException.class)
    public void TestTextStringStreamNoIndefiniteWithinDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[]{ 0x7F, 0x61, 0x20, 0x7F, 0x61, 0x20, (byte)0xFF, (byte)0xFF });
    }
    /**
     * 
     */
@Test
    public void TestByteStringStream() {
      TestCommon.FromBytesTestAB(
        new byte[]{ 0x5F, 0x41, 0x20, 0x41, 0x20, (byte)0xFF });
    }
    @Test(expected=CBORException.class)
    public void TestByteStringStreamNoTagsBeforeDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[]{ 0x5F, 0x41, 0x20, (byte)0xC2, 0x41, 0x20, (byte)0xFF });
    }
    public static void AssertDecimalsEquivalent(String a, String b) {
      CBORObject ca = CBORDataUtilities.ParseJSONNumber(a);
      CBORObject cb = CBORDataUtilities.ParseJSONNumber(b);
      CompareTestEqual(ca,cb);
      TestCommon.AssertRoundTrip(ca);
      TestCommon.AssertRoundTrip(cb);
    }
    /**
     * 
     */
@Test
    public void ZeroStringTests2() {
      Assert.assertEquals("0.0001265", DecimalFraction.FromString("1.265e-4").toString());
      Assert.assertEquals("0.0001265", DecimalFraction.FromString("1.265e-4").ToEngineeringString());
      Assert.assertEquals("0.0001265", DecimalFraction.FromString("1.265e-4").ToPlainString());
      Assert.assertEquals("0.0000", DecimalFraction.FromString("0.000E-1").toString());
      Assert.assertEquals("0.0000", DecimalFraction.FromString("0.000E-1").ToEngineeringString());
      Assert.assertEquals("0.0000", DecimalFraction.FromString("0.000E-1").ToPlainString());
      Assert.assertEquals("0E-16", DecimalFraction.FromString("0.0000000000000e-3").toString());
      Assert.assertEquals("0.0E-15", DecimalFraction.FromString("0.0000000000000e-3").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000", DecimalFraction.FromString("0.0000000000000e-3").ToPlainString());
      Assert.assertEquals("0E-8", DecimalFraction.FromString("0.000000000e+1").toString());
      Assert.assertEquals("0.00E-6", DecimalFraction.FromString("0.000000000e+1").ToEngineeringString());
      Assert.assertEquals("0.00000000", DecimalFraction.FromString("0.000000000e+1").ToPlainString());
      Assert.assertEquals("0.000", DecimalFraction.FromString("0.000000000000000e+12").toString());
      Assert.assertEquals("0.000", DecimalFraction.FromString("0.000000000000000e+12").ToEngineeringString());
      Assert.assertEquals("0.000", DecimalFraction.FromString("0.000000000000000e+12").ToPlainString());
      Assert.assertEquals("0E-25", DecimalFraction.FromString("0.00000000000000e-11").toString());
      Assert.assertEquals("0.0E-24", DecimalFraction.FromString("0.00000000000000e-11").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000", DecimalFraction.FromString("0.00000000000000e-11").ToPlainString());
      Assert.assertEquals("0E-7", DecimalFraction.FromString("0.000000000000e+5").toString());
      Assert.assertEquals("0.0E-6", DecimalFraction.FromString("0.000000000000e+5").ToEngineeringString());
      Assert.assertEquals("0.0000000", DecimalFraction.FromString("0.000000000000e+5").ToPlainString());
      Assert.assertEquals("0E-8", DecimalFraction.FromString("0.0000e-4").toString());
      Assert.assertEquals("0.00E-6", DecimalFraction.FromString("0.0000e-4").ToEngineeringString());
      Assert.assertEquals("0.00000000", DecimalFraction.FromString("0.0000e-4").ToPlainString());
      Assert.assertEquals("0.0000", DecimalFraction.FromString("0.000000e+2").toString());
      Assert.assertEquals("0.0000", DecimalFraction.FromString("0.000000e+2").ToEngineeringString());
      Assert.assertEquals("0.0000", DecimalFraction.FromString("0.000000e+2").ToPlainString());
      Assert.assertEquals("0E+2", DecimalFraction.FromString("0.0e+3").toString());
      Assert.assertEquals("0.0E+3", DecimalFraction.FromString("0.0e+3").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.0e+3").ToPlainString());
      Assert.assertEquals("0E-7", DecimalFraction.FromString("0.000000000000000e+8").toString());
      Assert.assertEquals("0.0E-6", DecimalFraction.FromString("0.000000000000000e+8").ToEngineeringString());
      Assert.assertEquals("0.0000000", DecimalFraction.FromString("0.000000000000000e+8").ToPlainString());
      Assert.assertEquals("0E+7", DecimalFraction.FromString("0.000e+10").toString());
      Assert.assertEquals("0.00E+9", DecimalFraction.FromString("0.000e+10").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.000e+10").ToPlainString());
      Assert.assertEquals("0E-31", DecimalFraction.FromString("0.0000000000000000000e-12").toString());
      Assert.assertEquals("0.0E-30", DecimalFraction.FromString("0.0000000000000000000e-12").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000000000", DecimalFraction.FromString("0.0000000000000000000e-12").ToPlainString());
      Assert.assertEquals("0.00000", DecimalFraction.FromString("0.0000e-1").toString());
      Assert.assertEquals("0.00000", DecimalFraction.FromString("0.0000e-1").ToEngineeringString());
      Assert.assertEquals("0.00000", DecimalFraction.FromString("0.0000e-1").ToPlainString());
      Assert.assertEquals("0E-22", DecimalFraction.FromString("0.00000000000e-11").toString());
      Assert.assertEquals("0.0E-21", DecimalFraction.FromString("0.00000000000e-11").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000", DecimalFraction.FromString("0.00000000000e-11").ToPlainString());
      Assert.assertEquals("0E-28", DecimalFraction.FromString("0.00000000000e-17").toString());
      Assert.assertEquals("0.0E-27", DecimalFraction.FromString("0.00000000000e-17").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000000", DecimalFraction.FromString("0.00000000000e-17").ToPlainString());
      Assert.assertEquals("0.00000", DecimalFraction.FromString("0.00000000000000e+9").toString());
      Assert.assertEquals("0.00000", DecimalFraction.FromString("0.00000000000000e+9").ToEngineeringString());
      Assert.assertEquals("0.00000", DecimalFraction.FromString("0.00000000000000e+9").ToPlainString());
      Assert.assertEquals("0E-28", DecimalFraction.FromString("0.0000000000e-18").toString());
      Assert.assertEquals("0.0E-27", DecimalFraction.FromString("0.0000000000e-18").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000000", DecimalFraction.FromString("0.0000000000e-18").ToPlainString());
      Assert.assertEquals("0E-14", DecimalFraction.FromString("0.0e-13").toString());
      Assert.assertEquals("0.00E-12", DecimalFraction.FromString("0.0e-13").ToEngineeringString());
      Assert.assertEquals("0.00000000000000", DecimalFraction.FromString("0.0e-13").ToPlainString());
      Assert.assertEquals("0E-8", DecimalFraction.FromString("0.000000000000000000e+10").toString());
      Assert.assertEquals("0.00E-6", DecimalFraction.FromString("0.000000000000000000e+10").ToEngineeringString());
      Assert.assertEquals("0.00000000", DecimalFraction.FromString("0.000000000000000000e+10").ToPlainString());
      Assert.assertEquals("0E+15", DecimalFraction.FromString("0.0000e+19").toString());
      Assert.assertEquals("0E+15", DecimalFraction.FromString("0.0000e+19").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.0000e+19").ToPlainString());
      Assert.assertEquals("0E-13", DecimalFraction.FromString("0.00000e-8").toString());
      Assert.assertEquals("0.0E-12", DecimalFraction.FromString("0.00000e-8").ToEngineeringString());
      Assert.assertEquals("0.0000000000000", DecimalFraction.FromString("0.00000e-8").ToPlainString());
      Assert.assertEquals("0E+3", DecimalFraction.FromString("0.00000000000e+14").toString());
      Assert.assertEquals("0E+3", DecimalFraction.FromString("0.00000000000e+14").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.00000000000e+14").ToPlainString());
      Assert.assertEquals("0E-17", DecimalFraction.FromString("0.000e-14").toString());
      Assert.assertEquals("0.00E-15", DecimalFraction.FromString("0.000e-14").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000", DecimalFraction.FromString("0.000e-14").ToPlainString());
      Assert.assertEquals("0E-25", DecimalFraction.FromString("0.000000e-19").toString());
      Assert.assertEquals("0.0E-24", DecimalFraction.FromString("0.000000e-19").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000", DecimalFraction.FromString("0.000000e-19").ToPlainString());
      Assert.assertEquals("0E+7", DecimalFraction.FromString("0.000000000000e+19").toString());
      Assert.assertEquals("0.00E+9", DecimalFraction.FromString("0.000000000000e+19").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.000000000000e+19").ToPlainString());
      Assert.assertEquals("0E+5", DecimalFraction.FromString("0.0000000000000e+18").toString());
      Assert.assertEquals("0.0E+6", DecimalFraction.FromString("0.0000000000000e+18").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.0000000000000e+18").ToPlainString());
      Assert.assertEquals("0E-16", DecimalFraction.FromString("0.00000000000000e-2").toString());
      Assert.assertEquals("0.0E-15", DecimalFraction.FromString("0.00000000000000e-2").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000", DecimalFraction.FromString("0.00000000000000e-2").ToPlainString());
      Assert.assertEquals("0E-31", DecimalFraction.FromString("0.0000000000000e-18").toString());
      Assert.assertEquals("0.0E-30", DecimalFraction.FromString("0.0000000000000e-18").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000000000", DecimalFraction.FromString("0.0000000000000e-18").ToPlainString());
      Assert.assertEquals("0E-17", DecimalFraction.FromString("0e-17").toString());
      Assert.assertEquals("0.00E-15", DecimalFraction.FromString("0e-17").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000", DecimalFraction.FromString("0e-17").ToPlainString());
      Assert.assertEquals("0E+17", DecimalFraction.FromString("0e+17").toString());
      Assert.assertEquals("0.0E+18", DecimalFraction.FromString("0e+17").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0e+17").ToPlainString());
      Assert.assertEquals("0E-17", DecimalFraction.FromString("0.00000000000000000e+0").toString());
      Assert.assertEquals("0.00E-15", DecimalFraction.FromString("0.00000000000000000e+0").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000", DecimalFraction.FromString("0.00000000000000000e+0").ToPlainString());
      Assert.assertEquals("0E-13", DecimalFraction.FromString("0.0000000000000e+0").toString());
      Assert.assertEquals("0.0E-12", DecimalFraction.FromString("0.0000000000000e+0").ToEngineeringString());
      Assert.assertEquals("0.0000000000000", DecimalFraction.FromString("0.0000000000000e+0").ToPlainString());
      Assert.assertEquals("0E-31", DecimalFraction.FromString("0.0000000000000000000e-12").toString());
      Assert.assertEquals("0.0E-30", DecimalFraction.FromString("0.0000000000000000000e-12").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000000000", DecimalFraction.FromString("0.0000000000000000000e-12").ToPlainString());
      Assert.assertEquals("0E-9", DecimalFraction.FromString("0.0000000000000000000e+10").toString());
      Assert.assertEquals("0E-9", DecimalFraction.FromString("0.0000000000000000000e+10").ToEngineeringString());
      Assert.assertEquals("0.000000000", DecimalFraction.FromString("0.0000000000000000000e+10").ToPlainString());
      Assert.assertEquals("0E-7", DecimalFraction.FromString("0.00000e-2").toString());
      Assert.assertEquals("0.0E-6", DecimalFraction.FromString("0.00000e-2").ToEngineeringString());
      Assert.assertEquals("0.0000000", DecimalFraction.FromString("0.00000e-2").ToPlainString());
      Assert.assertEquals("0E+9", DecimalFraction.FromString("0.000000e+15").toString());
      Assert.assertEquals("0E+9", DecimalFraction.FromString("0.000000e+15").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.000000e+15").ToPlainString());
      Assert.assertEquals("0E-19", DecimalFraction.FromString("0.000000000e-10").toString());
      Assert.assertEquals("0.0E-18", DecimalFraction.FromString("0.000000000e-10").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000", DecimalFraction.FromString("0.000000000e-10").ToPlainString());
      Assert.assertEquals("0E-8", DecimalFraction.FromString("0.00000000000000e+6").toString());
      Assert.assertEquals("0.00E-6", DecimalFraction.FromString("0.00000000000000e+6").ToEngineeringString());
      Assert.assertEquals("0.00000000", DecimalFraction.FromString("0.00000000000000e+6").ToPlainString());
      Assert.assertEquals("0E+12", DecimalFraction.FromString("0.00000e+17").toString());
      Assert.assertEquals("0E+12", DecimalFraction.FromString("0.00000e+17").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.00000e+17").ToPlainString());
      Assert.assertEquals("0E-18", DecimalFraction.FromString("0.000000000000000000e-0").toString());
      Assert.assertEquals("0E-18", DecimalFraction.FromString("0.000000000000000000e-0").ToEngineeringString());
      Assert.assertEquals("0.000000000000000000", DecimalFraction.FromString("0.000000000000000000e-0").ToPlainString());
      Assert.assertEquals("0.00000", DecimalFraction.FromString("0.0000000000000000e+11").toString());
      Assert.assertEquals("0.00000", DecimalFraction.FromString("0.0000000000000000e+11").ToEngineeringString());
      Assert.assertEquals("0.00000", DecimalFraction.FromString("0.0000000000000000e+11").ToPlainString());
      Assert.assertEquals("0E+3", DecimalFraction.FromString("0.000000000000e+15").toString());
      Assert.assertEquals("0E+3", DecimalFraction.FromString("0.000000000000e+15").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.000000000000e+15").ToPlainString());
      Assert.assertEquals("0E-27", DecimalFraction.FromString("0.00000000e-19").toString());
      Assert.assertEquals("0E-27", DecimalFraction.FromString("0.00000000e-19").ToEngineeringString());
      Assert.assertEquals("0.000000000000000000000000000", DecimalFraction.FromString("0.00000000e-19").ToPlainString());
      Assert.assertEquals("0E-11", DecimalFraction.FromString("0.00000e-6").toString());
      Assert.assertEquals("0.00E-9", DecimalFraction.FromString("0.00000e-6").ToEngineeringString());
      Assert.assertEquals("0.00000000000", DecimalFraction.FromString("0.00000e-6").ToPlainString());
      Assert.assertEquals("0E-14", DecimalFraction.FromString("0e-14").toString());
      Assert.assertEquals("0.00E-12", DecimalFraction.FromString("0e-14").ToEngineeringString());
      Assert.assertEquals("0.00000000000000", DecimalFraction.FromString("0e-14").ToPlainString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.000000000e+9").toString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.000000000e+9").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.000000000e+9").ToPlainString());
      Assert.assertEquals("0E+8", DecimalFraction.FromString("0.00000e+13").toString());
      Assert.assertEquals("0.0E+9", DecimalFraction.FromString("0.00000e+13").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.00000e+13").ToPlainString());
      Assert.assertEquals("0.000", DecimalFraction.FromString("0.000e-0").toString());
      Assert.assertEquals("0.000", DecimalFraction.FromString("0.000e-0").ToEngineeringString());
      Assert.assertEquals("0.000", DecimalFraction.FromString("0.000e-0").ToPlainString());
      Assert.assertEquals("0E-9", DecimalFraction.FromString("0.000000000000000e+6").toString());
      Assert.assertEquals("0E-9", DecimalFraction.FromString("0.000000000000000e+6").ToEngineeringString());
      Assert.assertEquals("0.000000000", DecimalFraction.FromString("0.000000000000000e+6").ToPlainString());
      Assert.assertEquals("0E+8", DecimalFraction.FromString("0.000000000e+17").toString());
      Assert.assertEquals("0.0E+9", DecimalFraction.FromString("0.000000000e+17").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.000000000e+17").ToPlainString());
      Assert.assertEquals("0.00000", DecimalFraction.FromString("0.00000000000e+6").toString());
      Assert.assertEquals("0.00000", DecimalFraction.FromString("0.00000000000e+6").ToEngineeringString());
      Assert.assertEquals("0.00000", DecimalFraction.FromString("0.00000000000e+6").ToPlainString());
      Assert.assertEquals("0E-11", DecimalFraction.FromString("0.00000000000000e+3").toString());
      Assert.assertEquals("0.00E-9", DecimalFraction.FromString("0.00000000000000e+3").ToEngineeringString());
      Assert.assertEquals("0.00000000000", DecimalFraction.FromString("0.00000000000000e+3").ToPlainString());
      Assert.assertEquals("0", DecimalFraction.FromString("0e+0").toString());
      Assert.assertEquals("0", DecimalFraction.FromString("0e+0").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0e+0").ToPlainString());
      Assert.assertEquals("0E+9", DecimalFraction.FromString("0.000e+12").toString());
      Assert.assertEquals("0E+9", DecimalFraction.FromString("0.000e+12").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.000e+12").ToPlainString());
      Assert.assertEquals("0.00", DecimalFraction.FromString("0.00000000000e+9").toString());
      Assert.assertEquals("0.00", DecimalFraction.FromString("0.00000000000e+9").ToEngineeringString());
      Assert.assertEquals("0.00", DecimalFraction.FromString("0.00000000000e+9").ToPlainString());
      Assert.assertEquals("0E-23", DecimalFraction.FromString("0.00000000000000e-9").toString());
      Assert.assertEquals("0.00E-21", DecimalFraction.FromString("0.00000000000000e-9").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000000", DecimalFraction.FromString("0.00000000000000e-9").ToPlainString());
      Assert.assertEquals("0.0", DecimalFraction.FromString("0e-1").toString());
      Assert.assertEquals("0.0", DecimalFraction.FromString("0e-1").ToEngineeringString());
      Assert.assertEquals("0.0", DecimalFraction.FromString("0e-1").ToPlainString());
      Assert.assertEquals("0E-17", DecimalFraction.FromString("0.0000e-13").toString());
      Assert.assertEquals("0.00E-15", DecimalFraction.FromString("0.0000e-13").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000", DecimalFraction.FromString("0.0000e-13").ToPlainString());
      Assert.assertEquals("0E-18", DecimalFraction.FromString("0.00000000000e-7").toString());
      Assert.assertEquals("0E-18", DecimalFraction.FromString("0.00000000000e-7").ToEngineeringString());
      Assert.assertEquals("0.000000000000000000", DecimalFraction.FromString("0.00000000000e-7").ToPlainString());
      Assert.assertEquals("0E-10", DecimalFraction.FromString("0.00000000000000e+4").toString());
      Assert.assertEquals("0.0E-9", DecimalFraction.FromString("0.00000000000000e+4").ToEngineeringString());
      Assert.assertEquals("0.0000000000", DecimalFraction.FromString("0.00000000000000e+4").ToPlainString());
      Assert.assertEquals("0E-16", DecimalFraction.FromString("0.00000000e-8").toString());
      Assert.assertEquals("0.0E-15", DecimalFraction.FromString("0.00000000e-8").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000", DecimalFraction.FromString("0.00000000e-8").ToPlainString());
      Assert.assertEquals("0E-8", DecimalFraction.FromString("0.00e-6").toString());
      Assert.assertEquals("0.00E-6", DecimalFraction.FromString("0.00e-6").ToEngineeringString());
      Assert.assertEquals("0.00000000", DecimalFraction.FromString("0.00e-6").ToPlainString());
      Assert.assertEquals("0.00", DecimalFraction.FromString("0.0e-1").toString());
      Assert.assertEquals("0.00", DecimalFraction.FromString("0.0e-1").ToEngineeringString());
      Assert.assertEquals("0.00", DecimalFraction.FromString("0.0e-1").ToPlainString());
      Assert.assertEquals("0E-26", DecimalFraction.FromString("0.0000000000000000e-10").toString());
      Assert.assertEquals("0.00E-24", DecimalFraction.FromString("0.0000000000000000e-10").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000000000", DecimalFraction.FromString("0.0000000000000000e-10").ToPlainString());
      Assert.assertEquals("0E+12", DecimalFraction.FromString("0.00e+14").toString());
      Assert.assertEquals("0E+12", DecimalFraction.FromString("0.00e+14").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.00e+14").ToPlainString());
      Assert.assertEquals("0E-13", DecimalFraction.FromString("0.000000000000000000e+5").toString());
      Assert.assertEquals("0.0E-12", DecimalFraction.FromString("0.000000000000000000e+5").ToEngineeringString());
      Assert.assertEquals("0.0000000000000", DecimalFraction.FromString("0.000000000000000000e+5").ToPlainString());
      Assert.assertEquals("0E+6", DecimalFraction.FromString("0.0e+7").toString());
      Assert.assertEquals("0E+6", DecimalFraction.FromString("0.0e+7").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.0e+7").ToPlainString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.00000000e+8").toString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.00000000e+8").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.00000000e+8").ToPlainString());
      Assert.assertEquals("0E-9", DecimalFraction.FromString("0.000000000e+0").toString());
      Assert.assertEquals("0E-9", DecimalFraction.FromString("0.000000000e+0").ToEngineeringString());
      Assert.assertEquals("0.000000000", DecimalFraction.FromString("0.000000000e+0").ToPlainString());
      Assert.assertEquals("0E+10", DecimalFraction.FromString("0.000e+13").toString());
      Assert.assertEquals("0.00E+12", DecimalFraction.FromString("0.000e+13").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.000e+13").ToPlainString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.0000000000000000e+16").toString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.0000000000000000e+16").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.0000000000000000e+16").ToPlainString());
      Assert.assertEquals("0E-9", DecimalFraction.FromString("0.00000000e-1").toString());
      Assert.assertEquals("0E-9", DecimalFraction.FromString("0.00000000e-1").ToEngineeringString());
      Assert.assertEquals("0.000000000", DecimalFraction.FromString("0.00000000e-1").ToPlainString());
      Assert.assertEquals("0E-26", DecimalFraction.FromString("0.00000000000e-15").toString());
      Assert.assertEquals("0.00E-24", DecimalFraction.FromString("0.00000000000e-15").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000000000", DecimalFraction.FromString("0.00000000000e-15").ToPlainString());
      Assert.assertEquals("0E+10", DecimalFraction.FromString("0.0e+11").toString());
      Assert.assertEquals("0.00E+12", DecimalFraction.FromString("0.0e+11").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.0e+11").ToPlainString());
      Assert.assertEquals("0E+2", DecimalFraction.FromString("0.00000e+7").toString());
      Assert.assertEquals("0.0E+3", DecimalFraction.FromString("0.00000e+7").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.00000e+7").ToPlainString());
      Assert.assertEquals("0E-38", DecimalFraction.FromString("0.0000000000000000000e-19").toString());
      Assert.assertEquals("0.00E-36", DecimalFraction.FromString("0.0000000000000000000e-19").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000000000000000000000", DecimalFraction.FromString("0.0000000000000000000e-19").ToPlainString());
      Assert.assertEquals("0E-16", DecimalFraction.FromString("0.0000000000e-6").toString());
      Assert.assertEquals("0.0E-15", DecimalFraction.FromString("0.0000000000e-6").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000", DecimalFraction.FromString("0.0000000000e-6").ToPlainString());
      Assert.assertEquals("0E-32", DecimalFraction.FromString("0.00000000000000000e-15").toString());
      Assert.assertEquals("0.00E-30", DecimalFraction.FromString("0.00000000000000000e-15").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000000000000000", DecimalFraction.FromString("0.00000000000000000e-15").ToPlainString());
      Assert.assertEquals("0E-13", DecimalFraction.FromString("0.000000000000000e+2").toString());
      Assert.assertEquals("0.0E-12", DecimalFraction.FromString("0.000000000000000e+2").ToEngineeringString());
      Assert.assertEquals("0.0000000000000", DecimalFraction.FromString("0.000000000000000e+2").ToPlainString());
      Assert.assertEquals("0E-19", DecimalFraction.FromString("0.0e-18").toString());
      Assert.assertEquals("0.0E-18", DecimalFraction.FromString("0.0e-18").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000", DecimalFraction.FromString("0.0e-18").ToPlainString());
      Assert.assertEquals("0E-20", DecimalFraction.FromString("0.00000000000000e-6").toString());
      Assert.assertEquals("0.00E-18", DecimalFraction.FromString("0.00000000000000e-6").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000", DecimalFraction.FromString("0.00000000000000e-6").ToPlainString());
      Assert.assertEquals("0E-20", DecimalFraction.FromString("0.000e-17").toString());
      Assert.assertEquals("0.00E-18", DecimalFraction.FromString("0.000e-17").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000", DecimalFraction.FromString("0.000e-17").ToPlainString());
      Assert.assertEquals("0E-21", DecimalFraction.FromString("0.00000000000000e-7").toString());
      Assert.assertEquals("0E-21", DecimalFraction.FromString("0.00000000000000e-7").ToEngineeringString());
      Assert.assertEquals("0.000000000000000000000", DecimalFraction.FromString("0.00000000000000e-7").ToPlainString());
      Assert.assertEquals("0E-15", DecimalFraction.FromString("0.000000e-9").toString());
      Assert.assertEquals("0E-15", DecimalFraction.FromString("0.000000e-9").ToEngineeringString());
      Assert.assertEquals("0.000000000000000", DecimalFraction.FromString("0.000000e-9").ToPlainString());
      Assert.assertEquals("0E-11", DecimalFraction.FromString("0e-11").toString());
      Assert.assertEquals("0.00E-9", DecimalFraction.FromString("0e-11").ToEngineeringString());
      Assert.assertEquals("0.00000000000", DecimalFraction.FromString("0e-11").ToPlainString());
      Assert.assertEquals("0E+2", DecimalFraction.FromString("0.000000000e+11").toString());
      Assert.assertEquals("0.0E+3", DecimalFraction.FromString("0.000000000e+11").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.000000000e+11").ToPlainString());
      Assert.assertEquals("0.0", DecimalFraction.FromString("0.0000000000000000e+15").toString());
      Assert.assertEquals("0.0", DecimalFraction.FromString("0.0000000000000000e+15").ToEngineeringString());
      Assert.assertEquals("0.0", DecimalFraction.FromString("0.0000000000000000e+15").ToPlainString());
      Assert.assertEquals("0.000000", DecimalFraction.FromString("0.0000000000000000e+10").toString());
      Assert.assertEquals("0.000000", DecimalFraction.FromString("0.0000000000000000e+10").ToEngineeringString());
      Assert.assertEquals("0.000000", DecimalFraction.FromString("0.0000000000000000e+10").ToPlainString());
      Assert.assertEquals("0.00000", DecimalFraction.FromString("0.000000000e+4").toString());
      Assert.assertEquals("0.00000", DecimalFraction.FromString("0.000000000e+4").ToEngineeringString());
      Assert.assertEquals("0.00000", DecimalFraction.FromString("0.000000000e+4").ToPlainString());
      Assert.assertEquals("0E-28", DecimalFraction.FromString("0.000000000000000e-13").toString());
      Assert.assertEquals("0.0E-27", DecimalFraction.FromString("0.000000000000000e-13").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000000000000000", DecimalFraction.FromString("0.000000000000000e-13").ToPlainString());
      Assert.assertEquals("0E-27", DecimalFraction.FromString("0.0000000000000000000e-8").toString());
      Assert.assertEquals("0E-27", DecimalFraction.FromString("0.0000000000000000000e-8").ToEngineeringString());
      Assert.assertEquals("0.000000000000000000000000000", DecimalFraction.FromString("0.0000000000000000000e-8").ToPlainString());
      Assert.assertEquals("0E-26", DecimalFraction.FromString("0.00000000000e-15").toString());
      Assert.assertEquals("0.00E-24", DecimalFraction.FromString("0.00000000000e-15").ToEngineeringString());
      Assert.assertEquals("0.00000000000000000000000000", DecimalFraction.FromString("0.00000000000e-15").ToPlainString());
      Assert.assertEquals("0E+10", DecimalFraction.FromString("0.00e+12").toString());
      Assert.assertEquals("0.00E+12", DecimalFraction.FromString("0.00e+12").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.00e+12").ToPlainString());
      Assert.assertEquals("0E+4", DecimalFraction.FromString("0.0e+5").toString());
      Assert.assertEquals("0.00E+6", DecimalFraction.FromString("0.0e+5").ToEngineeringString());
      Assert.assertEquals("0", DecimalFraction.FromString("0.0e+5").ToPlainString());
      Assert.assertEquals("0E-9", DecimalFraction.FromString("0.0000000000000000e+7").toString());
      Assert.assertEquals("0E-9", DecimalFraction.FromString("0.0000000000000000e+7").ToEngineeringString());
      Assert.assertEquals("0.000000000", DecimalFraction.FromString("0.0000000000000000e+7").ToPlainString());
      Assert.assertEquals("0E-16", DecimalFraction.FromString("0.0000000000000000e-0").toString());
      Assert.assertEquals("0.0E-15", DecimalFraction.FromString("0.0000000000000000e-0").ToEngineeringString());
      Assert.assertEquals("0.0000000000000000", DecimalFraction.FromString("0.0000000000000000e-0").ToPlainString());
      Assert.assertEquals("0.00", DecimalFraction.FromString("0.000000000000000e+13").toString());
      Assert.assertEquals("0.00", DecimalFraction.FromString("0.000000000000000e+13").ToEngineeringString());
      Assert.assertEquals("0.00", DecimalFraction.FromString("0.000000000000000e+13").ToPlainString());
      Assert.assertEquals("0E-24", DecimalFraction.FromString("0.00000000000e-13").toString());
      Assert.assertEquals("0E-24", DecimalFraction.FromString("0.00000000000e-13").ToEngineeringString());
      Assert.assertEquals("0.000000000000000000000000", DecimalFraction.FromString("0.00000000000e-13").ToPlainString());
      Assert.assertEquals("0E-13", DecimalFraction.FromString("0.000e-10").toString());
      Assert.assertEquals("0.0E-12", DecimalFraction.FromString("0.000e-10").ToEngineeringString());
      Assert.assertEquals("0.0000000000000", DecimalFraction.FromString("0.000e-10").ToPlainString());
    }


@Test
public void TestDivideScale() {
  Assert.assertEquals("1E+45", DecimalFraction.FromString("-45291999767448722989E-135").Divide(DecimalFraction.FromString("-1435346506662653"), 45, new PrecisionContext(Rounding.Ceiling)).toString());
  Assert.assertEquals("-150281048237549130218328063448008562496310709023253949072748919942664241665543637212110007139126086322169492431523975101044807204614481622597505872237486760318041941530445978199492353561599743392410251384110599653178804480386336982343749477930522158577698973903765471354952671269416808019878178219762571154589366600250147850.076124690795650590453618512277403496502", DecimalFraction.FromString("-269865389E+321").Divide(DecimalFraction.FromString("1795738"), -39, new PrecisionContext(Rounding.Floor)).toString());

  Assert.assertEquals("-1E-30", DecimalFraction.FromString("1007112E-22").Divide(DecimalFraction.FromString("-39490310826.6932439729E+298"), -30, new PrecisionContext(Rounding.Up)).toString());

  Assert.assertEquals("0E+11", DecimalFraction.FromString("7386483598E-224").Divide(DecimalFraction.FromString("-4390352444753"), 11, new PrecisionContext(Rounding.Down)).toString());

  {
    PrecisionContext ctx = new PrecisionContext(Rounding.Down).WithBlankFlags();
    DecimalFraction.FromString("93863134.47E+72").Divide(DecimalFraction.FromString("-6405000E+258"), 38, ctx);
    Assert.assertEquals(PrecisionContext.FlagInexact, ctx.getFlags() & PrecisionContext.FlagInexact);
  }
  Assert.assertEquals("-1E+18", DecimalFraction.FromString("-26669453070149435").Divide(DecimalFraction.FromString("49339084389861E+265"), 18, new PrecisionContext(Rounding.Floor)).toString());

  Assert.assertEquals("7.103E-34", DecimalFraction.FromString("-4542243282240.41931057893769451").Divide(DecimalFraction.FromString("-639555.15865952707790717004E+40"), -37, new PrecisionContext(Rounding.Ceiling)).toString());

  Assert.assertEquals("0E-7", DecimalFraction.FromString("3935634526184971598.078256150").Divide(DecimalFraction.FromString("5407243E+288"), -7, new PrecisionContext(Rounding.HalfDown)).toString());

  Assert.assertEquals("-1E+15", DecimalFraction.FromString("1128").Divide(DecimalFraction.FromString("-30527367869582.751781E+324"), 15, new PrecisionContext(Rounding.Floor)).toString());
  Assert.assertEquals("0E-651", DecimalFraction.FromString("-78113790E-328").DivideToIntegerNaturalScale(DecimalFraction.FromString("74436533544.0E+324"), new PrecisionContext(46, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-302463158127322.6E-305").Divide(DecimalFraction.FromString("630126062.01930394"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-182", DecimalFraction.FromString("-188259486952195939E-182").DivideToIntegerNaturalScale(DecimalFraction.FromString("-9"), new PrecisionContext(25, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-24589685066381400E+338").Divide(DecimalFraction.FromString("-32578992751974430297E-32"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-951929522728411654E-144").Divide(DecimalFraction.FromString("707733291E-139"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("2.4664674706811481948269592838078885E+149", DecimalFraction.FromString("31214454569707094E-197").Divide(DecimalFraction.FromString("126555306083.508987740853746E-341"), new PrecisionContext(35, Rounding.HalfDown)).toString());

  Assert.assertEquals("1E+31", DecimalFraction.FromString("-80619811028609627942.659E-58").Divide(DecimalFraction.FromString("-2192167.6569230012206"), 31, new PrecisionContext(Rounding.Ceiling)).toString());

  Assert.assertEquals("0E+10", DecimalFraction.FromString("939.15203").DivideToIntegerNaturalScale(DecimalFraction.FromString("2385531592.188194898747647"), new PrecisionContext(44, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("181.257221710870584E+273").DivideToIntegerNaturalScale(DecimalFraction.FromString("-518114882"), new PrecisionContext(26, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-74.5515151249").Divide(DecimalFraction.FromString("19760406509587061437.37690890330776422"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-299649E-145").Divide(DecimalFraction.FromString("-3296112203345202967.644501270E+299"), -7, new PrecisionContext(Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("6171505998846485E-166").Divide(DecimalFraction.FromString("1126829906715E-259"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E+7", DecimalFraction.FromString("45383867142").DivideToIntegerNaturalScale(DecimalFraction.FromString("-2812913313744064.1210296"), new PrecisionContext(10, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("7160786358320330424E+323").Divide(DecimalFraction.FromString("555494.85940316873300743388"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-7479249576360588316.48506514").Divide(DecimalFraction.FromString("1738944089.2538945979"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("5.351688914683185001036110847622345844884801777E-57", DecimalFraction.FromString("-429556332.16E-58").Divide(DecimalFraction.FromString("-8026556.4573726598"), new PrecisionContext(46, Rounding.HalfUp)).toString());

  Assert.assertEquals("-1.16942901899751144664915078774183951053497372202372222006960620984426667979662440441664399599494577288562604030E+119", DecimalFraction.FromString("-9E+123").Divide(DecimalFraction.FromString("76960.63509451147"), 9, new PrecisionContext(Rounding.HalfEven)).toString());

  try { DecimalFraction.FromString("33487E+202").Divide(DecimalFraction.FromString("20957697750E+16"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-23", DecimalFraction.FromString("67503804436319664222").Divide(DecimalFraction.FromString("-4679857327.02930083042905249E+290"), -23, new PrecisionContext(Rounding.HalfUp)).toString());

  try { DecimalFraction.FromString("-944.87409988703E+102").DivideToIntegerNaturalScale(DecimalFraction.FromString("-2104618710"), new PrecisionContext(12, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("6360575191737455.42474031E+119").DivideToIntegerNaturalScale(DecimalFraction.FromString("-212691297025225.1466"), new PrecisionContext(49, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0.00", DecimalFraction.FromString("746819323286256E-345").Divide(DecimalFraction.FromString("1069371617278763.530E-325"), -2, new PrecisionContext(Rounding.HalfEven)).toString());

  Assert.assertEquals("0E-27", DecimalFraction.FromString("-3633122765089.310291E+69").Divide(DecimalFraction.FromString("44515944787E+246"), -27, new PrecisionContext(Rounding.HalfDown)).toString());

  try { DecimalFraction.FromString("4617.80").Divide(DecimalFraction.FromString("4100262.5"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-1.094807582668295802747769891703390325E-261", DecimalFraction.FromString("-593013935.21196654773773E-252").Divide(DecimalFraction.FromString("541660420150412479"), new PrecisionContext(37, Rounding.HalfEven)).toString());

  Assert.assertEquals("0E-205", DecimalFraction.FromString("6952007351.767205866E-199").DivideToIntegerNaturalScale(DecimalFraction.FromString("-527157272985963049.004"), new PrecisionContext(28, Rounding.Unnecessary)).toString());

  Assert.assertEquals("0E+4", DecimalFraction.FromString("574568E-10").DivideToIntegerNaturalScale(DecimalFraction.FromString("67142816.45116023331755"), new PrecisionContext(51, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("3571772918909605233").DivideToIntegerNaturalScale(DecimalFraction.FromString("-5494400432458534696.440134708175E-241"), new PrecisionContext(21, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("75361998972586985004").Divide(DecimalFraction.FromString("85533795088144284.4934600702927"), 22, new PrecisionContext(Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("30250752141437").Divide(DecimalFraction.FromString("-30.885E+33"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("179507852310663261.06465759999032E-1").Divide(DecimalFraction.FromString("530178517368206.82"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("5.54303786109766000632320465594172E-190", DecimalFraction.FromString("-5E-172").Divide(DecimalFraction.FromString("-902032445978977141.5421976187963514"), new PrecisionContext(33, Rounding.HalfUp)).toString());

  Assert.assertEquals("8", DecimalFraction.FromString("3983").DivideToIntegerNaturalScale(DecimalFraction.FromString("458.352631483833874"), new PrecisionContext(48, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("9959122662069122E-279").Divide(DecimalFraction.FromString("74E-24"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("180509014672851.4485744097149244545E-176").Divide(DecimalFraction.FromString("-73216204E+290"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("33282", DecimalFraction.FromString("275664").DivideToIntegerNaturalScale(DecimalFraction.FromString("82824703.1995782E-7"), new PrecisionContext(36, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("6791.85807E+299").DivideToIntegerNaturalScale(DecimalFraction.FromString("-12239625E-4"), new PrecisionContext(39, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("513380.6750517361").Divide(DecimalFraction.FromString("-38725348318085588E-148"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1.0650442351130979580553937E-57", DecimalFraction.FromString("-66.076782156134E-52").Divide(DecimalFraction.FromString("-6204135"), new PrecisionContext(26, Rounding.Down)).toString());

  Assert.assertEquals("0E-554", DecimalFraction.FromString("299417E-322").DivideToIntegerNaturalScale(DecimalFraction.FromString("360397563.048532E+238"), new PrecisionContext(39, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-22.79155E-93").Divide(DecimalFraction.FromString("4005.81"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("5.40746471888320493E+80", DecimalFraction.FromString("-1168012379.27877226472653").Divide(DecimalFraction.FromString("-2160E-75"), new PrecisionContext(18, Rounding.Up)).toString());

  Assert.assertEquals("0", DecimalFraction.FromString("-244979587").DivideToIntegerNaturalScale(DecimalFraction.FromString("547487243824815"), new PrecisionContext(30, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("47092551017281E-299").Divide(DecimalFraction.FromString("-92372481E-346"), new PrecisionContext(4, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-44", DecimalFraction.FromString("-1567692241753E+126").DivideToIntegerNaturalScale(DecimalFraction.FromString("-86429174400373154E+170"), new PrecisionContext(33, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("252137169288968E-112").Divide(DecimalFraction.FromString("53544930948716814238"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-1.26434943840005411048297349575202103E-33", DecimalFraction.FromString("38254352476552884585.4972759209644625E-39").Divide(DecimalFraction.FromString("-30256154916287.300508"), new PrecisionContext(36, Rounding.HalfDown)).toString());

  try { DecimalFraction.FromString("-10018120405693838844.593079922896796240E+185").Divide(DecimalFraction.FromString("-9221272449661242228.2944199320905604"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("84.6368525111601242456E-349").Divide(DecimalFraction.FromString("-63170620798.8742263E-97"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-197", DecimalFraction.FromString("8542993019842643374.08414854E-200").DivideToIntegerNaturalScale(DecimalFraction.FromString("-398693716506268.37006039427"), new PrecisionContext(30, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-562720.011563708834180397862465", DecimalFraction.FromString("-4136895811582693.465669419107").Divide(DecimalFraction.FromString("73516059968916375E-7"), -24, new PrecisionContext(Rounding.Ceiling)).toString());

  Assert.assertEquals("-1.92515056386644381397720431008316639349238147E-175", DecimalFraction.FromString("317130052385.719289476464866").Divide(DecimalFraction.FromString("-1647.3E+183"), new PrecisionContext(45, Rounding.HalfUp)).toString());

  try { DecimalFraction.FromString("-61925E-81").DivideToIntegerNaturalScale(DecimalFraction.FromString("-9.6022742502734013486E-131"), new PrecisionContext(36, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("9.471642767241675800097857665E-320", DecimalFraction.FromString("-40769.189E-20").Divide(DecimalFraction.FromString("-4304341918489897870E+285"), new PrecisionContext(28, Rounding.HalfDown)).toString());

  Assert.assertEquals("0E-558", DecimalFraction.FromString("110691193854.481868307770756010E-228").DivideToIntegerNaturalScale(DecimalFraction.FromString("-6133129221.221037203E+321"), new PrecisionContext(11, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-8.13978742437132531022104534262E-20", DecimalFraction.FromString("3.8588208649488E-10").Divide(DecimalFraction.FromString("-4740689976"), new PrecisionContext(30, Rounding.HalfEven)).toString());

  Assert.assertEquals("0.00", DecimalFraction.FromString("47E-159").Divide(DecimalFraction.FromString("33856869132226.82579485518918602"), -2, new PrecisionContext(Rounding.HalfUp)).toString());

  Assert.assertEquals("0E-248", DecimalFraction.FromString("-18654720267").DivideToIntegerNaturalScale(DecimalFraction.FromString("151.3738312817271376E+264"), new PrecisionContext(37, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-7.14014935925752", DecimalFraction.FromString("-386813002.5387923786096306E+43").Divide(DecimalFraction.FromString("5417.43572965E+47"), -14, new PrecisionContext(Rounding.Floor)).toString());

  Assert.assertEquals("-161602.6944444444", DecimalFraction.FromString("5817697").Divide(DecimalFraction.FromString("-36"), new PrecisionContext(16, Rounding.HalfEven)).toString());

  Assert.assertEquals("1E-35", DecimalFraction.FromString("-9554E-176").Divide(DecimalFraction.FromString("-1970741060.05498853E+225"), -35, new PrecisionContext(Rounding.Ceiling)).toString());

  try { DecimalFraction.FromString("11732111.828296E-302").Divide(DecimalFraction.FromString("2635432.2302"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-9872156579438658159.06517552119E-175").Divide(DecimalFraction.FromString("-388037983E-119"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-36", DecimalFraction.FromString("-9695.9213287805152E-165").Divide(DecimalFraction.FromString("-600884381578445446.072182285481719898"), -36, new PrecisionContext(Rounding.Down)).toString());

  try { DecimalFraction.FromString("792859429755863E+329").DivideToIntegerNaturalScale(DecimalFraction.FromString("-57.2011677257905"), new PrecisionContext(12, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-1.099986892257671442046483049595111E-53", DecimalFraction.FromString("8299576E-312").Divide(DecimalFraction.FromString("-7545159E-259"), new PrecisionContext(34, Rounding.HalfUp)).toString());

  Assert.assertEquals("-1E-21", DecimalFraction.FromString("-64275475779303278076.795414E-339").Divide(DecimalFraction.FromString("9E+21"), -21, new PrecisionContext(Rounding.Floor)).toString());

  Assert.assertEquals("-8.72E+114", DecimalFraction.FromString("8700404238927.44655079629593732219E+117").Divide(DecimalFraction.FromString("-998318293729849"), new PrecisionContext(3, Rounding.HalfUp)).toString());

  Assert.assertEquals("0E+30", DecimalFraction.FromString("181.6001120200947541").Divide(DecimalFraction.FromString("1308.5"), 30, new PrecisionContext(Rounding.HalfUp)).toString());

  Assert.assertEquals("2.74413627630991195643204884891493E-267", DecimalFraction.FromString("199537125195598938E-279").Divide(DecimalFraction.FromString("72714"), new PrecisionContext(33, Rounding.Up)).toString());

  try { DecimalFraction.FromString("5.32636383486554091E-64").DivideToIntegerNaturalScale(DecimalFraction.FromString("1946161134952375211.68157529090261043945E-254"), new PrecisionContext(27, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-1.071523014369E-231", DecimalFraction.FromString("-46008307").Divide(DecimalFraction.FromString("4293730175E+229"), new PrecisionContext(13, Rounding.Up)).toString());

  try { DecimalFraction.FromString("-1030215899492980.76300E-257").Divide(DecimalFraction.FromString("401071770627.050"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-25", DecimalFraction.FromString("-2024E-25").DivideToIntegerNaturalScale(DecimalFraction.FromString("716"), new PrecisionContext(27, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-630908.5574805058048341").Divide(DecimalFraction.FromString("336830903652.6012324368876E+117"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-322", DecimalFraction.FromString("-97.477526").DivideToIntegerNaturalScale(DecimalFraction.FromString("-8034230440866128.5973345E+323"), new PrecisionContext(44, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-58050E-48").Divide(DecimalFraction.FromString("635891"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("59").Divide(DecimalFraction.FromString("-7790000E+189"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("812341411", DecimalFraction.FromString("735696911884006.471").DivideToIntegerNaturalScale(DecimalFraction.FromString("905649.8918183138461166"), new PrecisionContext(35, Rounding.Unnecessary)).toString());

  Assert.assertEquals("1.404416935E+458", DecimalFraction.FromString("-280883387E+156").Divide(DecimalFraction.FromString("-2E-294"), PrecisionContext.Unlimited).toString());

  Assert.assertEquals("0E-8", DecimalFraction.FromString("61763005").Divide(DecimalFraction.FromString("6.840543906570230479E+23"), -8, new PrecisionContext(Rounding.HalfDown)).toString());

  Assert.assertEquals("0", DecimalFraction.FromString("272696245").DivideToIntegerNaturalScale(DecimalFraction.FromString("4167262972"), new PrecisionContext(34, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-5.318431787930").Divide(DecimalFraction.FromString("18.0922196782880E+173"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-25029553952265806824").Divide(DecimalFraction.FromString("-4650955.4288747309E-332"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1.209958059483669669367419858084645966483820E+269", DecimalFraction.FromString("-48807445498E+263").Divide(DecimalFraction.FromString("-40338.13"), new PrecisionContext(43, Rounding.Floor)).toString());

  Assert.assertEquals("-1084309684953072", DecimalFraction.FromString("3366182.278269945205142464E+226").DivideToIntegerNaturalScale(DecimalFraction.FromString("-31044473041072485E+201"), new PrecisionContext(39, Rounding.Unnecessary)).toString());

  Assert.assertEquals("0E+16", DecimalFraction.FromString("-89").Divide(DecimalFraction.FromString("879005900719277E+141"), 16, new PrecisionContext(Rounding.HalfDown)).toString());

  try { DecimalFraction.FromString("35314192982809373.4809473985883000E+54").Divide(DecimalFraction.FromString("-34322E+211"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-921738465.5169940E+239").DivideToIntegerNaturalScale(DecimalFraction.FromString("303336341.676178"), new PrecisionContext(15, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-197922945317131.8591762566628607623").DivideToIntegerNaturalScale(DecimalFraction.FromString("-776478213387219447E-295"), new PrecisionContext(12, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("21811E-325").Divide(DecimalFraction.FromString("188497.92882983500801153024"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1.273936312390747133932918333070524119069563372351215E+103", DecimalFraction.FromString("-1056632E+294").Divide(DecimalFraction.FromString("-8294229387472.74961E+184"), new PrecisionContext(52, Rounding.HalfEven)).toString());

  Assert.assertEquals("13.1322002285816860055", DecimalFraction.FromString("864890582").Divide(DecimalFraction.FromString("65860295.072078"), new PrecisionContext(21, Rounding.Down)).toString());

  try { DecimalFraction.FromString("-455536.3092216E-243").Divide(DecimalFraction.FromString("-8797862592E+157"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0.011838670955277357252864195685410364761844313306867", DecimalFraction.FromString("75314442275251").Divide(DecimalFraction.FromString("6361731190922058"), new PrecisionContext(50, Rounding.Ceiling)).toString());

  try { DecimalFraction.FromString("90295127071862289094.429565E-155").Divide(DecimalFraction.FromString("36409431365607402277"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1.189572486448841949063732184087519551939E-297", DecimalFraction.FromString("-7502554469854670604.954728113106284246").Divide(DecimalFraction.FromString("-6306933.41962841457E+309"), new PrecisionContext(40, Rounding.Floor)).toString());

  Assert.assertEquals("5.103834265173766759360033838531E+439", DecimalFraction.FromString("-77504939871770434E+215").Divide(DecimalFraction.FromString("-151856302232673843E-225"), new PrecisionContext(31, Rounding.Up)).toString());

  Assert.assertEquals("-3.847697E-144", DecimalFraction.FromString("-689117904251.99167").Divide(DecimalFraction.FromString("1790987681793E+143"), new PrecisionContext(7, Rounding.Ceiling)).toString());

  try { DecimalFraction.FromString("-13.126285646442465042E-269").Divide(DecimalFraction.FromString("-782E+302"), -47, new PrecisionContext(Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-7.6342878608").Divide(DecimalFraction.FromString("59572941893693189E+155"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("476.14306653651827").Divide(DecimalFraction.FromString("944777055594948400"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("4E-293").Divide(DecimalFraction.FromString("-80272880"), 42, new PrecisionContext(Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-7.3498641642285931908929501518008397417543056576229518759350250187049930816019784800071753246704836409832364563493466969120785448500479054544513460583204054013002E+172", DecimalFraction.FromString("57109212524090E+173").Divide(DecimalFraction.FromString("-77701044873778.169353469257"), 12, new PrecisionContext(Rounding.HalfUp)).toString());

  try { DecimalFraction.FromString("-9061098855517395280E-33").Divide(DecimalFraction.FromString("2808899"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E+47", DecimalFraction.FromString("8196568801311818230.834372E-308").Divide(DecimalFraction.FromString("49E+12"), 47, new PrecisionContext(Rounding.HalfUp)).toString());

  try { DecimalFraction.FromString("-49660760135509070E+253").Divide(DecimalFraction.FromString("-18472.81334100301E+95"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-788.91659749290182236934").Divide(DecimalFraction.FromString("34670332192.69985065589611E-285"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-82567442580377E+107").Divide(DecimalFraction.FromString("558619520921686782"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-650703775987533271236368018879470537730446229291020199576034059282359634854895834987747216146954928627417273856025352004775923069520884853728013423408189138512045845871844195786686647315084797859429785429137054604533576738489249968935946388581881575419318010891571037651501491541701973805295.18324988858982276205682339455", DecimalFraction.FromString("5571.9452").Divide(DecimalFraction.FromString("-8562952E-294"), -29, new PrecisionContext(Rounding.HalfUp)).toString());

  try { DecimalFraction.FromString("-432E+186").DivideToIntegerNaturalScale(DecimalFraction.FromString("-56605217127949161.59659785E-280"), new PrecisionContext(10, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("2.338018090867071094528E-7", DecimalFraction.FromString("52983250").Divide(DecimalFraction.FromString("226616082257732.978241927877"), new PrecisionContext(22, Rounding.HalfEven)).toString());

  try { DecimalFraction.FromString("-380161995.0213752E-99").Divide(DecimalFraction.FromString("4288662278130.2487675861539E-2"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("9369810481978852.56").DivideToIntegerNaturalScale(DecimalFraction.FromString("338766285.72206E-205"), new PrecisionContext(46, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-24856144196E+140").Divide(DecimalFraction.FromString("3774790672874623"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("8915016381E+225").DivideToIntegerNaturalScale(DecimalFraction.FromString("-6472812"), new PrecisionContext(32, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1.25324780E+19", DecimalFraction.FromString("17502805503307E+20").Divide(DecimalFraction.FromString("139659575968309.2731726057938279111"), 11, new PrecisionContext(Rounding.Ceiling)).toString());

  try { DecimalFraction.FromString("-11474769").Divide(DecimalFraction.FromString("-59226234659277448247.556436528492E-225"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0", DecimalFraction.FromString("69").DivideToIntegerNaturalScale(DecimalFraction.FromString("4377231537"), new PrecisionContext(21, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("5E+50").Divide(DecimalFraction.FromString("6.966031951"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-50273337224051563145").DivideToIntegerNaturalScale(DecimalFraction.FromString("1896538.96599506269690431E-306"), new PrecisionContext(25, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1.457375480969746E+431", DecimalFraction.FromString("82035665.82378698525760E+196").Divide(DecimalFraction.FromString("5629E-231"), new PrecisionContext(16, Rounding.HalfDown)).toString());

  Assert.assertEquals("0E-18", DecimalFraction.FromString("57007").Divide(DecimalFraction.FromString("-3239E+137"), -18, new PrecisionContext(Rounding.HalfEven)).toString());

  try { DecimalFraction.FromString("-51304279450020989").Divide(DecimalFraction.FromString("98772338.09717945208572550877E-192"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-484702639.92309995631").Divide(DecimalFraction.FromString("3622414027656944165"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0.000107968498021899999170506", DecimalFraction.FromString("775091633299").Divide(DecimalFraction.FromString("7178868350486665"), new PrecisionContext(24, Rounding.HalfDown)).toString());

  try { DecimalFraction.FromString("5245998168716679.04796917581465905875").Divide(DecimalFraction.FromString("48.47957634639464945103"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-15", DecimalFraction.FromString("72093027488E-260").Divide(DecimalFraction.FromString("-6878471091589976089.8923"), -15, new PrecisionContext(Rounding.Ceiling)).toString());

  try { DecimalFraction.FromString("-677128192623140177.77E+156").DivideToIntegerNaturalScale(DecimalFraction.FromString("644101263521075826E+132"), new PrecisionContext(17, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-8670280703232.0037255244478678E+254").DivideToIntegerNaturalScale(DecimalFraction.FromString("-2608021874834.25560"), new PrecisionContext(11, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-139", DecimalFraction.FromString("7110190E-152").DivideToIntegerNaturalScale(DecimalFraction.FromString("-3863633968976.3441124404082"), new PrecisionContext(30, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-2.5504104178353979942125567909267060862896952489022E-51", DecimalFraction.FromString("8111447633521032927E+18").Divide(DecimalFraction.FromString("-3180447969E+78"), new PrecisionContext(50, Rounding.HalfEven)).toString());

  try { DecimalFraction.FromString("-343537037E-96").Divide(DecimalFraction.FromString("-56378813039E-33"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("71982928.77398423001395E-71").Divide(DecimalFraction.FromString("41820021240627.95713298953665181E-160"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("82999698245").Divide(DecimalFraction.FromString("-50252475"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-3063371E-244").Divide(DecimalFraction.FromString("-3347231"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1.06366262330E-166", DecimalFraction.FromString("-917E-160").Divide(DecimalFraction.FromString("-862115467.74"), new PrecisionContext(12, Rounding.HalfDown)).toString());

  Assert.assertEquals("991", DecimalFraction.FromString("8919").Divide(DecimalFraction.FromString("9"), new PrecisionContext(28, Rounding.Ceiling)).toString());

  Assert.assertEquals("0E-216", DecimalFraction.FromString("-806049E-187").DivideToIntegerNaturalScale(DecimalFraction.FromString("1252967E+29"), new PrecisionContext(21, Rounding.Unnecessary)).toString());

  Assert.assertEquals("0E+36", DecimalFraction.FromString("-85426").Divide(DecimalFraction.FromString("3.787609773547278380"), 36, new PrecisionContext(Rounding.Ceiling)).toString());

  try { DecimalFraction.FromString("3631.40410907978621861160").Divide(DecimalFraction.FromString("399.8834771"), 20, new PrecisionContext(Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-1668090E-25").Divide(DecimalFraction.FromString("-74382047004225777535.05245766332137"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-440.6997213E+238").Divide(DecimalFraction.FromString("767"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-913344395E+32").Divide(DecimalFraction.FromString("-7907615478.15941962320895728"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("22422516115.1").Divide(DecimalFraction.FromString("-455999123.04898783888774619794E-281"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-9526786853238391.7600081943901E+16").Divide(DecimalFraction.FromString("-4533683900E-258"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("99575E-334").Divide(DecimalFraction.FromString("328"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-11409.805676074", DecimalFraction.FromString("680086893297280600").Divide(DecimalFraction.FromString("-59605475553665.22926774"), -9, new PrecisionContext(Rounding.Up)).toString());

  try { DecimalFraction.FromString("613031672126.274527247").DivideToIntegerNaturalScale(DecimalFraction.FromString("8418440789465.9844221801299E-213"), new PrecisionContext(7, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1E+20", DecimalFraction.FromString("-753692040975.9952671508").Divide(DecimalFraction.FromString("-1"), 20, new PrecisionContext(Rounding.Up)).toString());

  try { DecimalFraction.FromString("-69.92614").Divide(DecimalFraction.FromString("-71308662.24325405763262"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("4.316507318").Divide(DecimalFraction.FromString("60"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("416483").DivideToIntegerNaturalScale(DecimalFraction.FromString("46062603989812603137E-145"), new PrecisionContext(10, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("5694459273135413658E+166").Divide(DecimalFraction.FromString("41592671262542656671.8266849"), new PrecisionContext(32, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-7", DecimalFraction.FromString("-43823437500504872E-27").DivideToIntegerNaturalScale(DecimalFraction.FromString("-9.23853493040223050364"), new PrecisionContext(52, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-7142156904666803191.461447640193E-159").Divide(DecimalFraction.FromString("291068624855.522214579014540"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("3.8844517748861850512599138E+349", DecimalFraction.FromString("-2191978545011511.44407840").Divide(DecimalFraction.FromString("-5642954712897E-347"), new PrecisionContext(26, Rounding.HalfDown)).toString());

  try { DecimalFraction.FromString("-470772819.4921276459861729").DivideToIntegerNaturalScale(DecimalFraction.FromString("-9099555984462887.27428470984847107E-133"), new PrecisionContext(15, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-604725183799525.2139").DivideToIntegerNaturalScale(DecimalFraction.FromString("33E-97"), new PrecisionContext(48, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-4818681215129").DivideToIntegerNaturalScale(DecimalFraction.FromString("1094930642018045627.728E-103"), new PrecisionContext(37, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("6.16119629822222124E+331").Divide(DecimalFraction.FromString("-949"), new PrecisionContext(36, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1E+7", DecimalFraction.FromString("32854194263457E-318").Divide(DecimalFraction.FromString("82235533196709441686.9"), 7, new PrecisionContext(Rounding.Up)).toString());

  try { DecimalFraction.FromString("39431264446185.900427727523E-247").Divide(DecimalFraction.FromString("-776438053757"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-5.5493184699192900858842528811468950656E-175", DecimalFraction.FromString("897716.7E-171").Divide(DecimalFraction.FromString("-1617706219"), new PrecisionContext(38, Rounding.Floor)).toString());

  try { DecimalFraction.FromString("-847849856646026.0875160E+216").Divide(DecimalFraction.FromString("-7142"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-183941458466061.3303328496939648E+283").Divide(DecimalFraction.FromString("32001117415442E+289"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("73341593324178093592").Divide(DecimalFraction.FromString("-4818E-330"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-82205448043527.813947317567089388").Divide(DecimalFraction.FromString("-1217.35907082"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("596247214315421E+344").Divide(DecimalFraction.FromString("27189044435669.92361807460954828041"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("50338123017058129.93697498896").Divide(DecimalFraction.FromString("-310747175.9884088152E+272"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("26258126359766700205E-2").DivideToIntegerNaturalScale(DecimalFraction.FromString("14053.05424545318"), new PrecisionContext(11, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E+21", DecimalFraction.FromString("-9372491285204058874.15050383926").Divide(DecimalFraction.FromString("-1064.5492883917462622"), 21, new PrecisionContext(Rounding.Floor)).toString());

  Assert.assertEquals("591957.000000000", DecimalFraction.FromString("6349570715848.26265264367894382").DivideToIntegerNaturalScale(DecimalFraction.FromString("10726395.981525"), new PrecisionContext(15, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-901.64017461185", DecimalFraction.FromString("-75771487805").Divide(DecimalFraction.FromString("84037390.9"), -11, new PrecisionContext(Rounding.HalfUp)).toString());

  try { DecimalFraction.FromString("6.3597204588E+147").Divide(DecimalFraction.FromString("-6311510937.258839007"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-97602147891.8E-294").Divide(DecimalFraction.FromString("455252533E+242"), new PrecisionContext(14, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-55674").Divide(DecimalFraction.FromString("-2291"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-761139E+106").DivideToIntegerNaturalScale(DecimalFraction.FromString("1600974326302200.302143398068033"), new PrecisionContext(37, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E+38", DecimalFraction.FromString("-29203640350E-112").Divide(DecimalFraction.FromString("545119676790686E+99"), 38, new PrecisionContext(Rounding.HalfDown)).toString());

  Assert.assertEquals("-1.75540290124E-289", DecimalFraction.FromString("411633763.9572983722009663139").Divide(DecimalFraction.FromString("-23449531937515322113.25205633020E+278"), new PrecisionContext(12, Rounding.HalfUp)).toString());

  Assert.assertEquals("0E-119", DecimalFraction.FromString("-92837744005084.1519044428600E-112").DivideToIntegerNaturalScale(DecimalFraction.FromString("1899.542363"), new PrecisionContext(33, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-452960015").Divide(DecimalFraction.FromString("-7940279228898798E+243"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-297", DecimalFraction.FromString("-881449").DivideToIntegerNaturalScale(DecimalFraction.FromString("8844154675261E+297"), new PrecisionContext(19, Rounding.Unnecessary)).toString());

  Assert.assertEquals("0.0001067385419676", DecimalFraction.FromString("-427").Divide(DecimalFraction.FromString("-4000429.386882370"), new PrecisionContext(13, Rounding.HalfDown)).toString());

  try { DecimalFraction.FromString("4259").Divide(DecimalFraction.FromString("-2205729973.0856861397052"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-76534210505.2599466178E-149").Divide(DecimalFraction.FromString("-92976146"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("636093.1817").Divide(DecimalFraction.FromString("7605203325621"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-2341408844534351224.22165").Divide(DecimalFraction.FromString("69078859919204550E+189"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-2.01088252061567331633412814386752791517674573107E-204", DecimalFraction.FromString("9014496564199802").Divide(DecimalFraction.FromString("-448285589624590662E+202"), new PrecisionContext(48, Rounding.HalfEven)).toString());

  try { DecimalFraction.FromString("-5144066737291").Divide(DecimalFraction.FromString("7084466356E-344"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("2240222E+259").Divide(DecimalFraction.FromString("93966956"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-33").Divide(DecimalFraction.FromString("-1761712364439.6962077971303"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-6.682660553827353232606249464576887340097692917E+282", DecimalFraction.FromString("-44285801551092936826.77281816").Divide(DecimalFraction.FromString("6626971577.32023E-273"), new PrecisionContext(46, Rounding.HalfDown)).toString());

  try { DecimalFraction.FromString("-68950102637348431.43396833130007970312E+263").Divide(DecimalFraction.FromString("-933674640E+131"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1.029084700041611065673740547831202559797971101832321754E+98", DecimalFraction.FromString("7.172").Divide(DecimalFraction.FromString("696.93E-100"), 44, new PrecisionContext(Rounding.Up)).toString());

  Assert.assertEquals("7.49102233711089493078115", DecimalFraction.FromString("899344522890711").Divide(DecimalFraction.FromString("120056313066283.860885072"), new PrecisionContext(24, Rounding.Ceiling)).toString());

  Assert.assertEquals("0.000001546174", DecimalFraction.FromString("3679786109463.391962").Divide(DecimalFraction.FromString("2379929733468976907"), -12, new PrecisionContext(Rounding.HalfDown)).toString());

  Assert.assertEquals("-1E-7", DecimalFraction.FromString("-1539792356").Divide(DecimalFraction.FromString("33706848.30494E+219"), -7, new PrecisionContext(Rounding.Up)).toString());

  Assert.assertEquals("-1.293010325460184E-204", DecimalFraction.FromString("557552636708580785.84698E-206").Divide(DecimalFraction.FromString("-4312050922796360.06823675745732"), new PrecisionContext(16, Rounding.Up)).toString());

  Assert.assertEquals("0E-143", DecimalFraction.FromString("9207408048569.322241712652E+164").DivideToIntegerNaturalScale(DecimalFraction.FromString("730689581296609.50E+297"), new PrecisionContext(44, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-9.0914796E+348", DecimalFraction.FromString("-6165702626018050140.51601").Divide(DecimalFraction.FromString("678184.7213784967896463E-336"), new PrecisionContext(8, Rounding.Down)).toString());

  Assert.assertEquals("110380845818880268", DecimalFraction.FromString("9051229357148182049").DivideToIntegerNaturalScale(DecimalFraction.FromString("82"), new PrecisionContext(28, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("68058476942002866.0488373064133363869").Divide(DecimalFraction.FromString("9"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-1E+42", DecimalFraction.FromString("931.506995E-251").Divide(DecimalFraction.FromString("-811316224"), 42, new PrecisionContext(Rounding.Floor)).toString());

  Assert.assertEquals("1129284646", DecimalFraction.FromString("81911532535203").DivideToIntegerNaturalScale(DecimalFraction.FromString("72534"), new PrecisionContext(17, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-361118181190.5506118238976E-179").Divide(DecimalFraction.FromString("71419.8"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1.26164117624046729903066733362210503E-96", DecimalFraction.FromString("-80134561E-13").Divide(DecimalFraction.FromString("-635161268585026338.18164671417324E+73"), new PrecisionContext(36, Rounding.Down)).toString());

  Assert.assertEquals("0E-146", DecimalFraction.FromString("-933911795439320.3741E-144").DivideToIntegerNaturalScale(DecimalFraction.FromString("34614288180347.80"), new PrecisionContext(15, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-5407942140741.531763827360124895E+191").Divide(DecimalFraction.FromString("372.1E-223"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("450").Divide(DecimalFraction.FromString("-3174093030837841698E+325"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-1.145284054160498406531544216797645345540094661246246953878008966623426423041531150540420104726923072086655058243521958193716639457345224819241865655430395485267410876670659923139056856524698988566560965121306866580875621848698303011330481293737930493279041558694401935700415565108916926383482408590161472E+329", DecimalFraction.FromString("-27103942307995869463E-8").Divide(DecimalFraction.FromString("23665694296.130978209715232E-328"), 26, new PrecisionContext(Rounding.Ceiling)).toString());

  Assert.assertEquals("0E+41", DecimalFraction.FromString("2882158193366").Divide(DecimalFraction.FromString("-86931E-12"), 41, new PrecisionContext(Rounding.Ceiling)).toString());

  Assert.assertEquals("-3.102906910490249196007585985745694785231351312221115458710178046324551282487439557678808920303546428048776522288948978798270798096845415513478126038509719828339372997402599232173969634273959785928781478848850974428715083065183189947496869157859933351141835215541502036984473404631940949543302090872688102888071E+327", DecimalFraction.FromString("6").Divide(DecimalFraction.FromString("-1933.67064274964649907867E-330"), 18, new PrecisionContext(Rounding.HalfUp)).toString());

  Assert.assertEquals("0E-36", DecimalFraction.FromString("57014.3506530157313E-333").Divide(DecimalFraction.FromString("-2862748443692"), -36, new PrecisionContext(Rounding.HalfEven)).toString());

  try { DecimalFraction.FromString("-64.16896322E-338").Divide(DecimalFraction.FromString("-4666506809"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("89344411054432").DivideToIntegerNaturalScale(DecimalFraction.FromString("-941215451.9E-257"), new PrecisionContext(48, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("9301871154011781113E-209").Divide(DecimalFraction.FromString("-2630"), 6, new PrecisionContext(Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-1853267575867.157").DivideToIntegerNaturalScale(DecimalFraction.FromString("-5818942372396.221482093098516E-260"), new PrecisionContext(23, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0.25004485497883062670264953010785511113356", DecimalFraction.FromString("43764685676").Divide(DecimalFraction.FromString("175027339313.60123"), -41, new PrecisionContext(Rounding.Ceiling)).toString());

  try { DecimalFraction.FromString("194877710421.987786487969386E-31").Divide(DecimalFraction.FromString("44441563497882731854E+156"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("24178375749521875723E-223").Divide(DecimalFraction.FromString("-524134531102926005.405171499506E+162"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("1.594319396E-134").Divide(DecimalFraction.FromString("-12105354798256981420"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("980").Divide(DecimalFraction.FromString("-48327380527705.4657661394E+32"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("2863794895545E-314").Divide(DecimalFraction.FromString("8214858.13593055435514821985"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-2832536E+113").Divide(DecimalFraction.FromString("-4451372009506570.704973841930912E+5"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-4.388315643671309908135264627857854185229833848734210388385135489281408878872058283978E+87", DecimalFraction.FromString("16744934833120984347.46254276698E+73").Divide(DecimalFraction.FromString("-38158"), 3, new PrecisionContext(Rounding.Up)).toString());

  try { DecimalFraction.FromString("2596620").Divide(DecimalFraction.FromString("-799544355.071628862237252368E+168"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-18165584358.3674662970150700719E-3").DivideToIntegerNaturalScale(DecimalFraction.FromString("-9711E-115"), new PrecisionContext(49, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("1105411E+99").Divide(DecimalFraction.FromString("-77304201E+93"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("62346159830E-233").Divide(DecimalFraction.FromString("8.27104257839E+263"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1.230334603241E+57", DecimalFraction.FromString("-2304E+63").Divide(DecimalFraction.FromString("-1872661302"), 45, new PrecisionContext(Rounding.Up)).toString());

  Assert.assertEquals("-4.8135910761782798064594037843739467671816E+258", DecimalFraction.FromString("-696.75910818E+266").Divide(DecimalFraction.FromString("14474829647"), new PrecisionContext(41, Rounding.Up)).toString());

  try { DecimalFraction.FromString("512.8444608213307267E-250").Divide(DecimalFraction.FromString("5535892"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-73809.0187194194074325218E+302").Divide(DecimalFraction.FromString("-9593130498E+283"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E+36", DecimalFraction.FromString("-687.93391470670E-345").Divide(DecimalFraction.FromString("6141170960E+120"), 36, new PrecisionContext(Rounding.HalfEven)).toString());

  try { DecimalFraction.FromString("56700319893118806.018699E+84").Divide(DecimalFraction.FromString("-9.50874397140904E+311"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-421", DecimalFraction.FromString("-9055746479E-312").DivideToIntegerNaturalScale(DecimalFraction.FromString("97322E+109"), new PrecisionContext(36, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("706740812.59913741787E+38").DivideToIntegerNaturalScale(DecimalFraction.FromString("6778708666436826E-264"), new PrecisionContext(23, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-277", DecimalFraction.FromString("1.1479E-263").DivideToIntegerNaturalScale(DecimalFraction.FromString("7118282661206.54631224953E+21"), new PrecisionContext(10, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("3900332998340395").Divide(DecimalFraction.FromString("-729.86326634547094495479E-265"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-145", DecimalFraction.FromString("-1962417047408081.193224759087E-133").DivideToIntegerNaturalScale(DecimalFraction.FromString("-222808"), new PrecisionContext(10, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-91373").Divide(DecimalFraction.FromString("895603795857952797.611921E+179"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("4621").DivideToIntegerNaturalScale(DecimalFraction.FromString("6484554893019.8E-321"), new PrecisionContext(13, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-187", DecimalFraction.FromString("30038465E-187").DivideToIntegerNaturalScale(DecimalFraction.FromString("-720033965480650897"), new PrecisionContext(17, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-7657047.3228195992368216123318813979862930835837616", DecimalFraction.FromString("-4171668352993774933").Divide(DecimalFraction.FromString("544814231533"), new PrecisionContext(50, Rounding.HalfEven)).toString());

  Assert.assertEquals("-6.24499826838410674108520420768095896263432393623113E+57", DecimalFraction.FromString("9498148780876").Divide(DecimalFraction.FromString("-15209209630947817591E-64"), 7, new PrecisionContext(Rounding.HalfEven)).toString());

  try { DecimalFraction.FromString("2756356493").Divide(DecimalFraction.FromString("-29082487137063683.60861E+246"), new PrecisionContext(31, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-47", DecimalFraction.FromString("-52442848.328660E-259").Divide(DecimalFraction.FromString("-47814379792676731812E+201"), -47, new PrecisionContext(Rounding.Floor)).toString());

  Assert.assertEquals("8.9261475128932379668311101E-326", DecimalFraction.FromString("-470.35863885544").Divide(DecimalFraction.FromString("-526944729712384453.02979E+310"), new PrecisionContext(26, Rounding.Up)).toString());

  Assert.assertEquals("-5.271206695447558890047630040180725E+264", DecimalFraction.FromString("210848267817902355.601905201607229E+248").DivideToIntegerNaturalScale(DecimalFraction.FromString("-4"), new PrecisionContext(34, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-709781022918.7673").Divide(DecimalFraction.FromString("3E+336"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-260", DecimalFraction.FromString("4817478758.0989241").DivideToIntegerNaturalScale(DecimalFraction.FromString("-16445934305.753761705814415E+268"), new PrecisionContext(51, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("53478641436567709.1854586").Divide(DecimalFraction.FromString("30430173E-253"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-5.98503285513352963276430295161949806991118074085581E+189", DecimalFraction.FromString("115196E+189").Divide(DecimalFraction.FromString("-19247.346303403026"), new PrecisionContext(51, Rounding.Down)).toString());

  try { DecimalFraction.FromString("488120102169295064E-284").Divide(DecimalFraction.FromString("-65426.9"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("607991466450E+217").Divide(DecimalFraction.FromString("-39467954426316305872.04918096928149341394E-265"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-838316021466").Divide(DecimalFraction.FromString("707264876E-181"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("7870").Divide(DecimalFraction.FromString("-5330269E+256"), new PrecisionContext(11, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("585331888461").Divide(DecimalFraction.FromString("-819741322707228E+49"), 35, new PrecisionContext(Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E+19", DecimalFraction.FromString("96601").DivideToIntegerNaturalScale(DecimalFraction.FromString("-9320796029315785.9796330216021360528"), new PrecisionContext(23, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-8.97157693191301019215E+356", DecimalFraction.FromString("-85727E+228").Divide(DecimalFraction.FromString("9555399307234210355.840952018616347060E-143"), new PrecisionContext(21, Rounding.Up)).toString());

  Assert.assertEquals("0E-8", DecimalFraction.FromString("73E-8").DivideToIntegerNaturalScale(DecimalFraction.FromString("-67889136"), new PrecisionContext(24, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-755.8E-297").Divide(DecimalFraction.FromString("-5551804"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-985266E+217").DivideToIntegerNaturalScale(DecimalFraction.FromString("-10841153583325826347.224253208174E-116"), new PrecisionContext(41, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("62E+38").Divide(DecimalFraction.FromString("-8007115179080810.262326"), new PrecisionContext(8, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1447467089844539611862222747215659804252446844414444819439757003037462031724603442456969287883901451231859601754978062774215322308471144110698616267296658791765102936213297333783327708403644.954438069524130948363145460681741478", DecimalFraction.FromString("6433267480814056304.921649").Divide(DecimalFraction.FromString("44445E-175"), -36, new PrecisionContext(Rounding.HalfEven)).toString());

  try { DecimalFraction.FromString("82828370985.53326458980E+214").Divide(DecimalFraction.FromString("581044353658670011E+291"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("844444296950381322.54314E-23").Divide(DecimalFraction.FromString("48175434750301E-56"), new PrecisionContext(20, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-167472967626672044.434637903802").Divide(DecimalFraction.FromString("108392829"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("43736187113304556590.58696458656542470544").Divide(DecimalFraction.FromString("33164982503904E+171"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("10932491672540595655782439346025669414932290473757266028602429192983803285644404095676073769880927471297970752692146020005635483963910147307528778883706922022306352220127649190210051031433168762369792172628042788298122760021764893106033288140677422695238173939954616758869840234122756089.98097444918785602775725607595880188920789851786", DecimalFraction.FromString("7869").Divide(DecimalFraction.FromString("7197810193411588E-298"), -47, new PrecisionContext(Rounding.Floor)).toString());

  Assert.assertEquals("2.06235140626057719382959871631622011E-272", DecimalFraction.FromString("-930230859376E-265").Divide(DecimalFraction.FromString("-4510535190812509571.5664"), new PrecisionContext(36, Rounding.HalfDown)).toString());

  try { DecimalFraction.FromString("80214209380612782469E+331").Divide(DecimalFraction.FromString("-97128350434253684854.89516"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("8233928240505213").Divide(DecimalFraction.FromString("-91567082"), new PrecisionContext(51, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("4596849550.774007").Divide(DecimalFraction.FromString("6176399097760"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-1966686.897875725441635E+221").DivideToIntegerNaturalScale(DecimalFraction.FromString("-535282876116868724"), new PrecisionContext(28, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-178", DecimalFraction.FromString("-6961").DivideToIntegerNaturalScale(DecimalFraction.FromString("-1.7927823852203783E+194"), new PrecisionContext(25, Rounding.Unnecessary)).toString());

  Assert.assertEquals("0E-170", DecimalFraction.FromString("98621443.8040899093607496493").DivideToIntegerNaturalScale(DecimalFraction.FromString("-54926890795595505E+151"), new PrecisionContext(38, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-2369006E+27").DivideToIntegerNaturalScale(DecimalFraction.FromString("693465.2792474552189401204E-96"), new PrecisionContext(23, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("52925949778.2009192466631692").DivideToIntegerNaturalScale(DecimalFraction.FromString("73520926823E-147"), new PrecisionContext(18, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-7633787.7391400723653E+55").Divide(DecimalFraction.FromString("-1954002961162293991E+139"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("6670757772E-137").Divide(DecimalFraction.FromString("-25987273481897928E-328"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-7111.00000000000000000000", DecimalFraction.FromString("33777909962590.50120968205938547775").DivideToIntegerNaturalScale(DecimalFraction.FromString("-4749774353"), new PrecisionContext(26, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-337897947217265556E+114").DivideToIntegerNaturalScale(DecimalFraction.FromString("383616937938E+26"), new PrecisionContext(45, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("811084309030769228E+22").Divide(DecimalFraction.FromString("-287688421866344E+177"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1589", DecimalFraction.FromString("-15375").DivideToIntegerNaturalScale(DecimalFraction.FromString("-9.67106532903307206991"), new PrecisionContext(6, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("244474439E-128").Divide(DecimalFraction.FromString("8437402195605517670"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-65348610019552903E+268").Divide(DecimalFraction.FromString("-94768574014891"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-181", DecimalFraction.FromString("-7203082863401").DivideToIntegerNaturalScale(DecimalFraction.FromString("39770251278.2457980938534"), new PrecisionContext(46, Rounding.Unnecessary)).toString());

  Assert.assertEquals("0E-182", DecimalFraction.FromString("441383417959965E-141").DivideToIntegerNaturalScale(DecimalFraction.FromString("8790881363537316.08507103E+49"), new PrecisionContext(23, Rounding.Unnecessary)).toString());

  Assert.assertEquals("8.246242792690274246321118537928704261212422141100244159010121913407024914307E+83", DecimalFraction.FromString("66880201808E+86").Divide(DecimalFraction.FromString("8110384752106.09713004259940518151"), 8, new PrecisionContext(Rounding.HalfDown)).toString());

  try { DecimalFraction.FromString("3294222617760372E+273").Divide(DecimalFraction.FromString("245387447344"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-1E+9", DecimalFraction.FromString("-3744497146328885463.2861367").Divide(DecimalFraction.FromString("4268082102.25598248731603196"), 9, new PrecisionContext(Rounding.Floor)).toString());

  try { DecimalFraction.FromString("-8314815473E+86").Divide(DecimalFraction.FromString("87033948310"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-875284001").Divide(DecimalFraction.FromString("-346.5763532222312"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("1849179559946929").Divide(DecimalFraction.FromString("-83127597.76"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("67104548543616240660E+177").Divide(DecimalFraction.FromString("-205830.34932E+282"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("1075363036").DivideToIntegerNaturalScale(DecimalFraction.FromString("-1.136697052603835675E-30"), new PrecisionContext(23, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-1.271521410051950E-42", DecimalFraction.FromString("9500663500216.70168E-35").Divide(DecimalFraction.FromString("-74718863757304223868"), new PrecisionContext(16, Rounding.HalfUp)).toString());

  Assert.assertEquals("-5.955926146515783204288266E+276", DecimalFraction.FromString("-10E+279").Divide(DecimalFraction.FromString("1679"), new PrecisionContext(25, Rounding.Down)).toString());

  Assert.assertEquals("1E+47", DecimalFraction.FromString("-8936325148723.75413099744E-155").Divide(DecimalFraction.FromString("-196050334"), 47, new PrecisionContext(Rounding.Ceiling)).toString());

  Assert.assertEquals("-4.3159320947474915845049076004245111033378786346889E-66", DecimalFraction.FromString("236805305109326857.01081E+73").Divide(DecimalFraction.FromString("-5486770.873840206126E+149"), new PrecisionContext(50, Rounding.Down)).toString());

  try { DecimalFraction.FromString("8764154").Divide(DecimalFraction.FromString("-58879505.7448464446572400313E+273"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("690858977725.11930E-130").Divide(DecimalFraction.FromString("-34612437741353074408.235198557248998445E-7"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E+19", DecimalFraction.FromString("-4").Divide(DecimalFraction.FromString("96"), 19, new PrecisionContext(Rounding.HalfEven)).toString());

  try { DecimalFraction.FromString("80914").Divide(DecimalFraction.FromString("-7444537792535655E+128"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("5.3561715184858927979852711229811E-17", DecimalFraction.FromString("-489").Divide(DecimalFraction.FromString("-9129655357605739431"), new PrecisionContext(32, Rounding.Ceiling)).toString());

  Assert.assertEquals("-866812993", DecimalFraction.FromString("-859472820841491").DivideToIntegerNaturalScale(DecimalFraction.FromString("991532"), new PrecisionContext(30, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("244.57055588551159078542E+16").Divide(DecimalFraction.FromString("-594206679765786816E-12"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("13333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333.33333333333333333333333333333", DecimalFraction.FromString("-8E+133").Divide(DecimalFraction.FromString("-6"), -29, new PrecisionContext(Rounding.HalfDown)).toString());

  Assert.assertEquals("0", DecimalFraction.FromString("30310").DivideToIntegerNaturalScale(DecimalFraction.FromString("-5560771953"), new PrecisionContext(50, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("48593123.340608").Divide(DecimalFraction.FromString("-78742398.6236117567411588246E-278"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-13748386841", DecimalFraction.FromString("9940083686099").DivideToIntegerNaturalScale(DecimalFraction.FromString("-723"), new PrecisionContext(43, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("1754250660495").DivideToIntegerNaturalScale(DecimalFraction.FromString("8369459812166078.050709852515299E-258"), new PrecisionContext(28, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("445540.80814223505179202").DivideToIntegerNaturalScale(DecimalFraction.FromString("-51466666469195902E-205"), new PrecisionContext(11, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-3890705.7841233").Divide(DecimalFraction.FromString("7601454734E+179"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("7158336806323E-340").Divide(DecimalFraction.FromString("447534636549712011"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-1.8654595436458E+230", DecimalFraction.FromString("-9484119510780198.060897824592404326E+229").Divide(DecimalFraction.FromString("508406603782183"), new PrecisionContext(14, Rounding.HalfDown)).toString());

  Assert.assertEquals("8.9695009431541547212899666573033707865168539325842696629213483146067415730337078651685393258426966292134831460674157303370786516853932584269662921348314606741573033707865168539325842696629213483146067415730337078651685393258426966292134831460674157303370786516853932584269662921348314606741573033707865168539325842696629213483146067415730337078651685393258426966E+367", DecimalFraction.FromString("-79828558.39407197701948070325E+221").Divide(DecimalFraction.FromString("-8.90E-140"), 6, new PrecisionContext(Rounding.HalfUp)).toString());

  try { DecimalFraction.FromString("10").Divide(DecimalFraction.FromString("-750063.6137365464782181"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("3765517974015841137E-96").Divide(DecimalFraction.FromString("11849E+208"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-56472918392188014722.05514998587760").Divide(DecimalFraction.FromString("826176E+141"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("5.3130729E+503", DecimalFraction.FromString("-10626.1458E+333").Divide(DecimalFraction.FromString("-2E-167"), new PrecisionContext(41, Rounding.Up)).toString());

  try { DecimalFraction.FromString("-6104516773338920431.319794391265E-232").Divide(DecimalFraction.FromString("88501780765893814.85792434134173"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-44766548892692600").Divide(DecimalFraction.FromString("-5166250025.759E+177"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("960918768").Divide(DecimalFraction.FromString("-432997E-15"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-2881449067768519233.4374027465748E-225").Divide(DecimalFraction.FromString("3.3702101117216602E+29"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-8536722661785382E-308").Divide(DecimalFraction.FromString("-9876424915616791049"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-8742367").Divide(DecimalFraction.FromString("7405076517839634.0"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-7749797767051.3064162693E+116").Divide(DecimalFraction.FromString("71510921524E+202"), new PrecisionContext(6, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-185", DecimalFraction.FromString("-3440473557E-166").DivideToIntegerNaturalScale(DecimalFraction.FromString("-93290022200532462553E+19"), new PrecisionContext(43, Rounding.Unnecessary)).toString());

  Assert.assertEquals("0E-254", DecimalFraction.FromString("58430407").DivideToIntegerNaturalScale(DecimalFraction.FromString("857290216.51532086341277595E+271"), new PrecisionContext(3, Rounding.Unnecessary)).toString());

  Assert.assertEquals("9.314748208124558778796308275510677528E+210", DecimalFraction.FromString("738705276E+205").Divide(DecimalFraction.FromString("793.0491082471586194433"), new PrecisionContext(37, Rounding.Ceiling)).toString());

  try { DecimalFraction.FromString("-875953308725E-211").Divide(DecimalFraction.FromString("14.904893170E+60"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-233", DecimalFraction.FromString("9062264451E+77").DivideToIntegerNaturalScale(DecimalFraction.FromString("-4175423426622991.83021773103548236772E+330"), new PrecisionContext(14, Rounding.Unnecessary)).toString());

  Assert.assertEquals("0E+36", DecimalFraction.FromString("-11927.61E-45").Divide(DecimalFraction.FromString("-41694151.79"), 36, new PrecisionContext(Rounding.Floor)).toString());

  try { DecimalFraction.FromString("82054").Divide(DecimalFraction.FromString("47923"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1.878192886539191579996843596674026354667E+226", DecimalFraction.FromString("5633734803889595924.9224725E-21").Divide(DecimalFraction.FromString("2999550708697692E-244"), new PrecisionContext(40, Rounding.Ceiling)).toString());

  Assert.assertEquals("6.66490403297034E+369", DecimalFraction.FromString("655739E+102").Divide(DecimalFraction.FromString("98386863E-270"), new PrecisionContext(15, Rounding.HalfDown)).toString());

  try { DecimalFraction.FromString("-35703607822842365259.525949286527E-349").Divide(DecimalFraction.FromString("2115842066632664824E+223"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("8.26903525232E-217", DecimalFraction.FromString("5704353665538500437E-137").Divide(DecimalFraction.FromString("689845126E+89"), new PrecisionContext(12, Rounding.HalfDown)).toString());

  try { DecimalFraction.FromString("2535").DivideToIntegerNaturalScale(DecimalFraction.FromString("-915446E-190"), new PrecisionContext(36, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-134", DecimalFraction.FromString("4247876123712743496.60673348159985979448").DivideToIntegerNaturalScale(DecimalFraction.FromString("-1784E+114"), new PrecisionContext(5, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-1243290641668958", DecimalFraction.FromString("-11224316016829611E+5").DivideToIntegerNaturalScale(DecimalFraction.FromString("902791"), new PrecisionContext(26, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("247").Divide(DecimalFraction.FromString("709474513E-336"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-249499226533319.56396818527233").Divide(DecimalFraction.FromString("496507199511164"), new PrecisionContext(42, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("8.69104223324633").Divide(DecimalFraction.FromString("559588758313561.03640479"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-92E+136").Divide(DecimalFraction.FromString("-70882758080445002216.80042038489162808"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-5.7034836131263785966177659950079431492784797692393397141585561850024061122757455317432250666541962345432046745714607594410283397801779765052556107826109643097999649698567047479708857378632190188995934485799339593502453282542365653029052098237659E+267", DecimalFraction.FromString("-34388E+271").Divide(DecimalFraction.FromString("60292975.89434141122"), 23, new PrecisionContext(Rounding.Floor)).toString());

  try { DecimalFraction.FromString("23435E+252").Divide(DecimalFraction.FromString("-1112467607244399.31631062489271676745"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-1E-17", DecimalFraction.FromString("-20959457984E+88").Divide(DecimalFraction.FromString("381.700845E+158"), -17, new PrecisionContext(Rounding.Floor)).toString());

  try { DecimalFraction.FromString("48778559E+150").DivideToIntegerNaturalScale(DecimalFraction.FromString("1774.5018171824932210045E-3"), new PrecisionContext(32, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-822286.2920916877").Divide(DecimalFraction.FromString("-9952720632.003022906567603"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-90", DecimalFraction.FromString("-46444554E-65").DivideToIntegerNaturalScale(DecimalFraction.FromString("-775.86599532751718E+39"), new PrecisionContext(30, Rounding.Unnecessary)).toString());

  Assert.assertEquals("215186.715241741367378372017", DecimalFraction.FromString("-184697770306").Divide(DecimalFraction.FromString("-858314"), new PrecisionContext(27, Rounding.Down)).toString());

  Assert.assertEquals("0E-261", DecimalFraction.FromString("68023624").DivideToIntegerNaturalScale(DecimalFraction.FromString("881008335765698754E+261"), new PrecisionContext(49, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("2072440.06434201002343").Divide(DecimalFraction.FromString("-77014929638020E+147"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-208912017462.22826032724168956E-323").Divide(DecimalFraction.FromString("30464107.84110"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-7163463032035452725832079143328494378187919386821532843913446693819648248843045080783014716036153679875486627448633810373214841824067832030686775450065702961554596319317032149443492085577540092369844827460940326965537435808258490218352635909645332760778526381579367771824.08231517168764776508464179665261390726734438780", DecimalFraction.FromString("-99023575431687428.706116625726354815E+265").Divide(DecimalFraction.FromString("138234224129.9324551344285604"), -47, new PrecisionContext(Rounding.HalfDown)).toString());

  Assert.assertEquals("0E-27", DecimalFraction.FromString("841728.872938107063").DivideToIntegerNaturalScale(DecimalFraction.FromString("-9E+15"), new PrecisionContext(18, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("23518885406465295").Divide(DecimalFraction.FromString("30123.38"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-269", DecimalFraction.FromString("5.9E-271").DivideToIntegerNaturalScale(DecimalFraction.FromString("-928320712599399.740"), new PrecisionContext(13, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-6").Divide(DecimalFraction.FromString("-93032.667706275"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-9051078928912323105.889162497125506E-236").Divide(DecimalFraction.FromString("186021991100185"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("634.0848669E+167").Divide(DecimalFraction.FromString("35.38332155047962840341"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("5887234169.89964510").Divide(DecimalFraction.FromString("-306516008874.55688751737128245108E-150"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("667771059752877196.98E-103").Divide(DecimalFraction.FromString("310614170E-139"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-67931529963.47015723672286294").Divide(DecimalFraction.FromString("410961.672040"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0.0000", DecimalFraction.FromString("4.0585").DivideToIntegerNaturalScale(DecimalFraction.FromString("407435"), new PrecisionContext(52, Rounding.Unnecessary)).toString());

  Assert.assertEquals("0E-107", DecimalFraction.FromString("8").DivideToIntegerNaturalScale(DecimalFraction.FromString("-5E+107"), new PrecisionContext(10, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-35046E-4").Divide(DecimalFraction.FromString("297.08462012E+51"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("7.730032908459620863771684757385232300496371E+306", DecimalFraction.FromString("-76317.402521604830911").Divide(DecimalFraction.FromString("-987284315932.01008155696361482810E-314"), new PrecisionContext(43, Rounding.Up)).toString());

  Assert.assertEquals("0E-461", DecimalFraction.FromString("-654228199956E-296").DivideToIntegerNaturalScale(DecimalFraction.FromString("-74731229386218.3975511879261563E+181"), new PrecisionContext(49, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-6.41675E-209", DecimalFraction.FromString("513.34E-209").Divide(DecimalFraction.FromString("-80"), PrecisionContext.Unlimited).toString());

  try { DecimalFraction.FromString("883789670324").Divide(DecimalFraction.FromString("667061997442.34E-152"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-28061.603921").Divide(DecimalFraction.FromString("-2046647E-223"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-0.0364787", DecimalFraction.FromString("28181672795299").Divide(DecimalFraction.FromString("-772551819831695.76368889497454657"), new PrecisionContext(6, Rounding.HalfUp)).toString());

  try { DecimalFraction.FromString("809392.548556444419458113").Divide(DecimalFraction.FromString("589.15639575554"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-616529593073737").DivideToIntegerNaturalScale(DecimalFraction.FromString("-88083969051784724564E-343"), new PrecisionContext(35, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-812.8385614", DecimalFraction.FromString("-812.8385614715570470").Divide(DecimalFraction.FromString("1"), new PrecisionContext(10, Rounding.Down)).toString());

  try { DecimalFraction.FromString("49456368E+41").Divide(DecimalFraction.FromString("-78436766242.159"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-47492992536514781438.7746072506").Divide(DecimalFraction.FromString("1.57608637817899781644"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E+2", DecimalFraction.FromString("-63353016457886667742E-263").Divide(DecimalFraction.FromString("-738412E+151"), 2, new PrecisionContext(Rounding.HalfDown)).toString());

  try { DecimalFraction.FromString("-7212.555461491E-187").Divide(DecimalFraction.FromString("816966"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("847272157398E-6").Divide(DecimalFraction.FromString("-491443636031232.20997801961128783E+187"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("47084975140408907.20729762455E+133").Divide(DecimalFraction.FromString("7729200E+212"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("758982450004").Divide(DecimalFraction.FromString("-6401E+312"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-2.7900927666839957403009074280342531222061E-17", DecimalFraction.FromString("-20").Divide(DecimalFraction.FromString("716822044012889561.15711959"), new PrecisionContext(41, Rounding.HalfDown)).toString());

  try { DecimalFraction.FromString("195993").DivideToIntegerNaturalScale(DecimalFraction.FromString("8435808326144778318E-78"), new PrecisionContext(44, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-2722138138458783.6919326062646554568E+285").Divide(DecimalFraction.FromString("4923593389671.3598488130468602"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-29", DecimalFraction.FromString("-5525611132249.9597225034E+25").DivideToIntegerNaturalScale(DecimalFraction.FromString("2E+44"), new PrecisionContext(33, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-504.0000", DecimalFraction.FromString("-3533672006979167.5308").DivideToIntegerNaturalScale(DecimalFraction.FromString("7010610980226"), new PrecisionContext(28, Rounding.Unnecessary)).toString());

  Assert.assertEquals("0.02823477746093814323405637796", DecimalFraction.FromString("25545734748").Divide(DecimalFraction.FromString("904761327881.6047832173"), -29, new PrecisionContext(Rounding.Ceiling)).toString());

  Assert.assertEquals("8.5988380580364865007976310978589025820195205470227E-59", DecimalFraction.FromString("-7606746220334.04359045319656611").Divide(DecimalFraction.FromString("-8846248957119E+58"), new PrecisionContext(50, Rounding.HalfDown)).toString());

  Assert.assertEquals("0E-272", DecimalFraction.FromString("-75540340122E-209").DivideToIntegerNaturalScale(DecimalFraction.FromString("-37095800E+63"), new PrecisionContext(46, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-34459563249743634342").Divide(DecimalFraction.FromString("-1.0833E-50"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-6937046437034923").Divide(DecimalFraction.FromString("-91235"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-7.0584273275345028757E-163", DecimalFraction.FromString("-39159123457560203E-160").Divide(DecimalFraction.FromString("5547853883088489740.798270722200"), new PrecisionContext(20, Rounding.HalfDown)).toString());

  try { DecimalFraction.FromString("-70").Divide(DecimalFraction.FromString("7549985807607808903.07762E-41"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("265175879.674464384173E+203").Divide(DecimalFraction.FromString("1850008587.85975"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-345").Divide(DecimalFraction.FromString("465798569234.5225020"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("73150745209904691016377760388567851794155084509311437770753200489061245583294932442310641594171190592028259351444269509620948782512727495294265357079.09750988450", DecimalFraction.FromString("68019470.12612793200875934E+156").Divide(DecimalFraction.FromString("929853413399239.3184603633882660"), -11, new PrecisionContext(Rounding.HalfEven)).toString());

  try { DecimalFraction.FromString("-8394558").DivideToIntegerNaturalScale(DecimalFraction.FromString("-706621095382329078E-144"), new PrecisionContext(14, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-630", DecimalFraction.FromString("-349500328442.86800E-323").DivideToIntegerNaturalScale(DecimalFraction.FromString("80210E+302"), new PrecisionContext(18, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-595653402426443626").DivideToIntegerNaturalScale(DecimalFraction.FromString("-94824.960342413060509664"), new PrecisionContext(7, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-8719957738724853795E+160").Divide(DecimalFraction.FromString("35.69010582252048"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-572525996897579653").Divide(DecimalFraction.FromString("-6600602259201.88714E-134"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1.66292469E+163", DecimalFraction.FromString("-78447657").Divide(DecimalFraction.FromString("-4717451E-162"), new PrecisionContext(9, Rounding.Ceiling)).toString());

  Assert.assertEquals("4.0219033790506909270349563046192259675405742821473158551810237203495630461922596754057428214731585518102372034956304619225967540574282147315855181023720349563046192259675405742821473158551810237203495630461922596754057428214731585518102372034956304619225967540574282E+287", DecimalFraction.FromString("12886178426478413.73022E+275").Divide(DecimalFraction.FromString("3204"), 22, new PrecisionContext(Rounding.HalfDown)).toString());

  Assert.assertEquals("0E-323", DecimalFraction.FromString("-910738129511.666479772325074866E-63").DivideToIntegerNaturalScale(DecimalFraction.FromString("1395E+242"), new PrecisionContext(9, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-8623606950512722195.9").Divide(DecimalFraction.FromString("26"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1.104955891453606922007510077052564E-522", DecimalFraction.FromString("-8293705E-287").Divide(DecimalFraction.FromString("-7505915E+235"), new PrecisionContext(34, Rounding.Down)).toString());

  try { DecimalFraction.FromString("81660644162.431391753292479931E+191").DivideToIntegerNaturalScale(DecimalFraction.FromString("-184830230.228636093257017"), new PrecisionContext(6, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-61897091914E+251").DivideToIntegerNaturalScale(DecimalFraction.FromString("-99519432.64386099906"), new PrecisionContext(17, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("68").Divide(DecimalFraction.FromString("831228257314120697E-235"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-37359253493426.839584E+349").Divide(DecimalFraction.FromString("5980749885"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-1E+32", DecimalFraction.FromString("2819751738").Divide(DecimalFraction.FromString("-380712663242199"), 32, new PrecisionContext(Rounding.Floor)).toString());

  try { DecimalFraction.FromString("-173148928487154.9447E-190").Divide(DecimalFraction.FromString("-437732.4166875673550207E+188"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-0.0000523547568562099303900073156685654806041", DecimalFraction.FromString("314").Divide(DecimalFraction.FromString("-5997544.805"), -43, new PrecisionContext(Rounding.Ceiling)).toString());

  Assert.assertEquals("0E-263", DecimalFraction.FromString("7124000798873218285.018E-260").DivideToIntegerNaturalScale(DecimalFraction.FromString("8777964263053"), new PrecisionContext(23, Rounding.Unnecessary)).toString());

  Assert.assertEquals("6.6980808605022148064469518843016458905279197152202183136518278185711095640294762836763913883014357209473393844658408753563031170775919820305008603817205007290257326380223042467390876013083056391125589459995533896412667971469479436220100093262751382521772254988243639086287748427012045344087010206360256932312259454347226418316278947576E+356", DecimalFraction.FromString("-509918197829173111E+344").Divide(DecimalFraction.FromString("-76129"), 22, new PrecisionContext(Rounding.HalfUp)).toString());

  try { DecimalFraction.FromString("227698755.1E+166").Divide(DecimalFraction.FromString("-1475679E-111"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-334", DecimalFraction.FromString("-678.9697740E-260").DivideToIntegerNaturalScale(DecimalFraction.FromString("4.2505312791440E+80"), new PrecisionContext(35, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-7444857799493600772478445275456016152363481781316235969234869629372644167810072743705558851973147886888786480733824743580485240941655393911148600106163672220778747015542629628002790332703250201569524921289240914853905832971637325241269229137088867034095215264340843457719.535828005917172979037513893965868304", DecimalFraction.FromString("1").Divide(DecimalFraction.FromString("-1343209E-277"), -36, new PrecisionContext(Rounding.Down)).toString());

  Assert.assertEquals("-4.0961767762799263E-138", DecimalFraction.FromString("-2482042086821.3960449951496398").Divide(DecimalFraction.FromString("605.941154980020624E+147"), new PrecisionContext(17, Rounding.Up)).toString());

  try { DecimalFraction.FromString("5710940650054556").Divide(DecimalFraction.FromString("542154445632783746.0835E+87"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-289", DecimalFraction.FromString("-839114113623E-289").DivideToIntegerNaturalScale(DecimalFraction.FromString("291361698007552814"), new PrecisionContext(14, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("7.949578417").Divide(DecimalFraction.FromString("41294830"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("1.380757236831E+171", DecimalFraction.FromString("-4.142271710493").DivideToIntegerNaturalScale(DecimalFraction.FromString("-3E-171"), new PrecisionContext(23, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-137666820151", DecimalFraction.FromString("826000920909").DivideToIntegerNaturalScale(DecimalFraction.FromString("-6"), new PrecisionContext(33, Rounding.Unnecessary)).toString());

  Assert.assertEquals("0E-11", DecimalFraction.FromString("-333413923.7881792692").Divide(DecimalFraction.FromString("627340730866763120E+96"), -11, new PrecisionContext(Rounding.Ceiling)).toString());

  Assert.assertEquals("0E+13", DecimalFraction.FromString("-9454401689E-109").Divide(DecimalFraction.FromString("-93471156621449069209.647411"), 13, new PrecisionContext(Rounding.Down)).toString());

  try { DecimalFraction.FromString("59E-299").Divide(DecimalFraction.FromString("101750323"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("50.860640287240420269305100904290274025", DecimalFraction.FromString("-3737224208328815526").Divide(DecimalFraction.FromString("-73479692493497482"), new PrecisionContext(38, Rounding.HalfEven)).toString());

  try { DecimalFraction.FromString("-7188021105858618").Divide(DecimalFraction.FromString("-9510018423639421245.3361E-50"), new PrecisionContext(49, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-1.47472106107E+287", DecimalFraction.FromString("8304E+296").Divide(DecimalFraction.FromString("-5630895373520.17497013125747"), new PrecisionContext(12, Rounding.HalfDown)).toString());

  Assert.assertEquals("-1.7080299235915815E-207", DecimalFraction.FromString("-900898162759479").Divide(DecimalFraction.FromString("5274487E+215"), new PrecisionContext(17, Rounding.Ceiling)).toString());

  Assert.assertEquals("1.075648506817710007447028696112214542898119E+67", DecimalFraction.FromString("-72521150405212.8522E+41").Divide(DecimalFraction.FromString("-67420862805607E-26"), new PrecisionContext(43, Rounding.HalfEven)).toString());

  try { DecimalFraction.FromString("-980E+23").Divide(DecimalFraction.FromString("25372843982297571962.2151069162855"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("510213511009465068E-122").Divide(DecimalFraction.FromString("835122284.36131795"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-1.66797821280820348794238353032281264E+632", DecimalFraction.FromString("-3E+331").Divide(DecimalFraction.FromString("179858464395E-312"), new PrecisionContext(36, Rounding.HalfUp)).toString());

  Assert.assertEquals("-936836021366821712653355785422179456338705797450084195333172961270146740437815732499398604763050276641808996872744767861438537406783738272792879480394515275439018522973298051479432282896319461149867693047871060861197979312003848929516478229492422420014433485686793360596584075054125571325475102.23719028145297089247053163338", DecimalFraction.FromString("-7788854681643755719").Divide(DecimalFraction.FromString("8314E-279"), -29, new PrecisionContext(Rounding.Down)).toString());

  Assert.assertEquals("-0.000130374", DecimalFraction.FromString("79").Divide(DecimalFraction.FromString("-605953"), -9, new PrecisionContext(Rounding.Up)).toString());

  try { DecimalFraction.FromString("-62028345317557E-125").Divide(DecimalFraction.FromString("-96060778052930281098"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-2.45732807E+298", DecimalFraction.FromString("-737198421E+290").Divide(DecimalFraction.FromString("3"), PrecisionContext.Unlimited).toString());

  try { DecimalFraction.FromString("-59127.4146E-209").Divide(DecimalFraction.FromString("-7833960183E+198"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("2.4478920745452197145891579124506836699098206130E-161", DecimalFraction.FromString("9604570035.87953274605965484209").Divide(DecimalFraction.FromString("392360845306626.2130116795343231425E+156"), new PrecisionContext(47, Rounding.Down)).toString());

  try { DecimalFraction.FromString("-49245048784.36167800E-130").Divide(DecimalFraction.FromString("-1113753E-213"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("454.6102326157675400").DivideToIntegerNaturalScale(DecimalFraction.FromString("39E-41"), new PrecisionContext(26, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("95875523415796032.5899").Divide(DecimalFraction.FromString("-71481607.731"), -44, new PrecisionContext(Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0", DecimalFraction.FromString("-457901").DivideToIntegerNaturalScale(DecimalFraction.FromString("-8512562754821"), new PrecisionContext(37, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("54E+60").Divide(DecimalFraction.FromString("385834151488064"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-204", DecimalFraction.FromString("346980.368710E-198").DivideToIntegerNaturalScale(DecimalFraction.FromString("-31072262892537"), new PrecisionContext(22, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("201098692216576498E+203").Divide(DecimalFraction.FromString("-91134023689817.9363E-281"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-61488282318608852408.96535277757783E-84").Divide(DecimalFraction.FromString("-79E-199"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("925205443.1E+121").Divide(DecimalFraction.FromString("9993602E+126"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-334", DecimalFraction.FromString("32375909492.305780466187E-322").DivideToIntegerNaturalScale(DecimalFraction.FromString("1552"), new PrecisionContext(48, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("195472727064.399213538432627").Divide(DecimalFraction.FromString("-35233462387053"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("6.17E+191", DecimalFraction.FromString("4361.774").Divide(DecimalFraction.FromString("7064E-192"), new PrecisionContext(3, Rounding.HalfEven)).toString());

  Assert.assertEquals("0E-219", DecimalFraction.FromString("-96684739586.5E-1").DivideToIntegerNaturalScale(DecimalFraction.FromString("2928399161555001.7409250E+224"), new PrecisionContext(22, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("7198511330246836654").Divide(DecimalFraction.FromString("1113383549284125.3276"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-2530832856030442318300506528470787641158136154124043609461347884580589587334314743142352967650378924284478637591252589.3582", DecimalFraction.FromString("-10811450738121373.5E+121").Divide(DecimalFraction.FromString("42718944130822233685"), -4, new PrecisionContext(Rounding.Up)).toString());

  Assert.assertEquals("1.086987314630753579372061513385894094007E-330", DecimalFraction.FromString("-759332281524001586E-85").Divide(DecimalFraction.FromString("-69.85659090069E+261"), new PrecisionContext(40, Rounding.HalfUp)).toString());

  Assert.assertEquals("0E-296", DecimalFraction.FromString("-8105848807").DivideToIntegerNaturalScale(DecimalFraction.FromString("-27091628442E+296"), new PrecisionContext(17, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-59").Divide(DecimalFraction.FromString("-993152E-238"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("-9.2444194525201983594198444473046488489E-25", DecimalFraction.FromString("5163686125848847465E-297").Divide(DecimalFraction.FromString("-558573326575E-266"), new PrecisionContext(38, Rounding.Ceiling)).toString());

  try { DecimalFraction.FromString("-592.87379201798").Divide(DecimalFraction.FromString("862"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-27", DecimalFraction.FromString("-49905787790010").Divide(DecimalFraction.FromString("-228138693.1E+186"), -27, new PrecisionContext(Rounding.Floor)).toString());

  Assert.assertEquals("-1.4293391342200064052606650239921905835E-359", DecimalFraction.FromString("-99445005137623790E-177").Divide(DecimalFraction.FromString("69574114887640821.4849714074976291528E+182"), new PrecisionContext(38, Rounding.Floor)).toString());

  try { DecimalFraction.FromString("-7050545264660812").Divide(DecimalFraction.FromString("-91"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("4261196136E-107").Divide(DecimalFraction.FromString("-5587958166142930073"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("2177873155985").Divide(DecimalFraction.FromString("38320.959757257335"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-6966205343").Divide(DecimalFraction.FromString("22058734686522431.1544572181685430E-310"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-17", DecimalFraction.FromString("-6.236872E-11").DivideToIntegerNaturalScale(DecimalFraction.FromString("-515763120567"), new PrecisionContext(48, Rounding.Unnecessary)).toString());

  Assert.assertEquals("0E-184", DecimalFraction.FromString("57481089279325670.03405325592789317").DivideToIntegerNaturalScale(DecimalFraction.FromString("167534308751434340.4E+168"), new PrecisionContext(4, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-6080238326114886.5807829530014798").Divide(DecimalFraction.FromString("4382112991776743.7724491636514607"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-30", DecimalFraction.FromString("81185285995658.7539151730709442E-165").Divide(DecimalFraction.FromString("-52358353114095762E-80"), -30, new PrecisionContext(Rounding.Down)).toString());

  Assert.assertEquals("4.44561272466523826342718582559474725807443E-7", DecimalFraction.FromString("30488058161").Divide(DecimalFraction.FromString("68580103687047547.44569427"), new PrecisionContext(42, Rounding.Up)).toString());

  try { DecimalFraction.FromString("6852125921158948E+166").Divide(DecimalFraction.FromString("-859576846440249.312220024"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-99.528338003E-239").Divide(DecimalFraction.FromString("322055690624740772E-222"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-6612005E-291").Divide(DecimalFraction.FromString("850954E-254"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-1866E+93").Divide(DecimalFraction.FromString("-838616"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-1").Divide(DecimalFraction.FromString("14392665097929.038763814138E-183"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("0E-128", DecimalFraction.FromString("6255645.683856011353644E+144").DivideToIntegerNaturalScale(DecimalFraction.FromString("4277E+257"), new PrecisionContext(3, Rounding.Unnecessary)).toString());

  try { DecimalFraction.FromString("-49497488214996857949E-283").Divide(DecimalFraction.FromString("-1.828507203107816E-271"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-41").Divide(DecimalFraction.FromString("-38021611855527.693294222053164"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("64916509490617E-302").Divide(DecimalFraction.FromString("-2658524.985901233"), new PrecisionContext(27, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("-51553107200639527").Divide(DecimalFraction.FromString("-201253070357"), 5, new PrecisionContext(Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("72284829545982535.77E+345").Divide(DecimalFraction.FromString("-4499867442E-94"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  Assert.assertEquals("211427", DecimalFraction.FromString("-107682478250924719").DivideToIntegerNaturalScale(DecimalFraction.FromString("-509310730904.121635696027070"), new PrecisionContext(44, Rounding.Unnecessary)).toString());

  Assert.assertEquals("0E-124", DecimalFraction.FromString("-88617058928020488.42E-122").DivideToIntegerNaturalScale(DecimalFraction.FromString("-172915224007542"), new PrecisionContext(29, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-206.0000000000000", DecimalFraction.FromString("94309556583212565.2072374249007").DivideToIntegerNaturalScale(DecimalFraction.FromString("-456259850813049"), new PrecisionContext(18, Rounding.Unnecessary)).toString());

  Assert.assertEquals("0E-297", DecimalFraction.FromString("-73497.047457615527371E-300").DivideToIntegerNaturalScale(DecimalFraction.FromString("9403959774849.001351242421338471"), new PrecisionContext(10, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-2.057387119199338974853E+22", DecimalFraction.FromString("773317.99357898125051642E+29").DivideToIntegerNaturalScale(DecimalFraction.FromString("-3758738384052.52961484422539901122"), new PrecisionContext(52, Rounding.Unnecessary)).toString());

  Assert.assertEquals("-4.519749165652840292796539677258359673931126268507735817667609382798203294E+103", DecimalFraction.FromString("27168212234739223").Divide(DecimalFraction.FromString("-6011E-91"), 31, new PrecisionContext(Rounding.HalfUp)).toString());

  try { DecimalFraction.FromString("892460456048.022350019515158").Divide(DecimalFraction.FromString("-9060136714E-43"), new PrecisionContext(35, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("17285503665E+60").DivideToIntegerNaturalScale(DecimalFraction.FromString("42580064.430920E-248"), new PrecisionContext(28, Rounding.Unnecessary)); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("42773871145.40609675852965516").Divide(DecimalFraction.FromString("18169601017481413.522285571199"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("6323652E+32").Divide(DecimalFraction.FromString("-856767879752263565E+118"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }

  try { DecimalFraction.FromString("4097245371399313168E-247").Divide(DecimalFraction.FromString("6046.794253191E+222"), PrecisionContext.Unlimited); Assert.fail("Should have failed"); } catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }


}

    /**
     * 
     */
@Test
    public void AddTest() {
      Assert.assertEquals("0.0000249885", DecimalFraction.FromString("228.16E-7").Add(DecimalFraction.FromString("217.25E-8")).toString());
      Assert.assertEquals("0.0000206435", DecimalFraction.FromString("228.16E-7").Subtract(DecimalFraction.FromString("217.25E-8")).toString());
      Assert.assertEquals("376000.000008029", DecimalFraction.FromString("37.6E4").Add(DecimalFraction.FromString("80.29E-7")).toString());
      Assert.assertEquals("375999.999991971", DecimalFraction.FromString("37.6E4").Subtract(DecimalFraction.FromString("80.29E-7")).toString());
      Assert.assertEquals("8.129029518", DecimalFraction.FromString("81.29E-1").Add(DecimalFraction.FromString("295.18E-7")).toString());
      Assert.assertEquals("8.128970482", DecimalFraction.FromString("81.29E-1").Subtract(DecimalFraction.FromString("295.18E-7")).toString());
      Assert.assertEquals("1961.4007420", DecimalFraction.FromString("74.20E-5").Add(DecimalFraction.FromString("196.14E1")).toString());
      Assert.assertEquals("-1961.3992580", DecimalFraction.FromString("74.20E-5").Subtract(DecimalFraction.FromString("196.14E1")).toString());
      Assert.assertEquals("368300.0000021732", DecimalFraction.FromString("217.32E-8").Add(DecimalFraction.FromString("368.30E3")).toString());
      Assert.assertEquals("-368299.9999978268", DecimalFraction.FromString("217.32E-8").Subtract(DecimalFraction.FromString("368.30E3")).toString());
      Assert.assertEquals("26.94219", DecimalFraction.FromString("269.1E-1").Add(DecimalFraction.FromString("321.9E-4")).toString());
      Assert.assertEquals("26.87781", DecimalFraction.FromString("269.1E-1").Subtract(DecimalFraction.FromString("321.9E-4")).toString());
      Assert.assertEquals("7.502423E+8", DecimalFraction.FromString("242.3E3").Add(DecimalFraction.FromString("7.5E8")).toString());
      Assert.assertEquals("-7.497577E+8", DecimalFraction.FromString("242.3E3").Subtract(DecimalFraction.FromString("7.5E8")).toString());
      Assert.assertEquals("3.0706E+8", DecimalFraction.FromString("39.16E6").Add(DecimalFraction.FromString("267.9E6")).toString());
      Assert.assertEquals("-2.2874E+8", DecimalFraction.FromString("39.16E6").Subtract(DecimalFraction.FromString("267.9E6")).toString());
      Assert.assertEquals("3583800036.12", DecimalFraction.FromString("36.12E0").Add(DecimalFraction.FromString("358.38E7")).toString());
      Assert.assertEquals("-3583799963.88", DecimalFraction.FromString("36.12E0").Subtract(DecimalFraction.FromString("358.38E7")).toString());
      Assert.assertEquals("2.7525E+8", DecimalFraction.FromString("161.15E6").Add(DecimalFraction.FromString("114.1E6")).toString());
      Assert.assertEquals("4.705E+7", DecimalFraction.FromString("161.15E6").Subtract(DecimalFraction.FromString("114.1E6")).toString());
      Assert.assertEquals("3212600003.9235", DecimalFraction.FromString("392.35E-2").Add(DecimalFraction.FromString("321.26E7")).toString());
      Assert.assertEquals("-3212599996.0765", DecimalFraction.FromString("392.35E-2").Subtract(DecimalFraction.FromString("321.26E7")).toString());
      Assert.assertEquals("3901100000.0030320", DecimalFraction.FromString("390.11E7").Add(DecimalFraction.FromString("303.20E-5")).toString());
      Assert.assertEquals("3901099999.9969680", DecimalFraction.FromString("390.11E7").Subtract(DecimalFraction.FromString("303.20E-5")).toString());
      Assert.assertEquals("0.0162801", DecimalFraction.FromString("120.1E-6").Add(DecimalFraction.FromString("161.6E-4")).toString());
      Assert.assertEquals("-0.0160399", DecimalFraction.FromString("120.1E-6").Subtract(DecimalFraction.FromString("161.6E-4")).toString());
      Assert.assertEquals("164293.814", DecimalFraction.FromString("381.4E-2").Add(DecimalFraction.FromString("164.29E3")).toString());
      Assert.assertEquals("-164286.186", DecimalFraction.FromString("381.4E-2").Subtract(DecimalFraction.FromString("164.29E3")).toString());
      Assert.assertEquals("263160011.325", DecimalFraction.FromString("263.16E6").Add(DecimalFraction.FromString("113.25E-1")).toString());
      Assert.assertEquals("263159988.675", DecimalFraction.FromString("263.16E6").Subtract(DecimalFraction.FromString("113.25E-1")).toString());
      Assert.assertEquals("0.192317", DecimalFraction.FromString("189.14E-3").Add(DecimalFraction.FromString("317.7E-5")).toString());
      Assert.assertEquals("0.185963", DecimalFraction.FromString("189.14E-3").Subtract(DecimalFraction.FromString("317.7E-5")).toString());
      Assert.assertEquals("400000000491.3", DecimalFraction.FromString("40.0E10").Add(DecimalFraction.FromString("49.13E1")).toString());
      Assert.assertEquals("399999999508.7", DecimalFraction.FromString("40.0E10").Subtract(DecimalFraction.FromString("49.13E1")).toString());
      Assert.assertEquals("0.00308683", DecimalFraction.FromString("305.33E-6").Add(DecimalFraction.FromString("278.15E-5")).toString());
      Assert.assertEquals("-0.00247617", DecimalFraction.FromString("305.33E-6").Subtract(DecimalFraction.FromString("278.15E-5")).toString());
      Assert.assertEquals("18012.00000012526", DecimalFraction.FromString("180.12E2").Add(DecimalFraction.FromString("125.26E-9")).toString());
      Assert.assertEquals("18011.99999987474", DecimalFraction.FromString("180.12E2").Subtract(DecimalFraction.FromString("125.26E-9")).toString());
      Assert.assertEquals("1795661.4", DecimalFraction.FromString("179.23E4").Add(DecimalFraction.FromString("336.14E1")).toString());
      Assert.assertEquals("1788938.6", DecimalFraction.FromString("179.23E4").Subtract(DecimalFraction.FromString("336.14E1")).toString());
      Assert.assertEquals("241300000.0003170", DecimalFraction.FromString("317.0E-6").Add(DecimalFraction.FromString("241.30E6")).toString());
      Assert.assertEquals("-241299999.9996830", DecimalFraction.FromString("317.0E-6").Subtract(DecimalFraction.FromString("241.30E6")).toString());
      Assert.assertEquals("35015000015.719", DecimalFraction.FromString("350.15E8").Add(DecimalFraction.FromString("157.19E-1")).toString());
      Assert.assertEquals("35014999984.281", DecimalFraction.FromString("350.15E8").Subtract(DecimalFraction.FromString("157.19E-1")).toString());
      Assert.assertEquals("3870000000000.2475", DecimalFraction.FromString("247.5E-3").Add(DecimalFraction.FromString("387.0E10")).toString());
      Assert.assertEquals("-3869999999999.7525", DecimalFraction.FromString("247.5E-3").Subtract(DecimalFraction.FromString("387.0E10")).toString());
      Assert.assertEquals("3.6529005234E+11", DecimalFraction.FromString("52.34E3").Add(DecimalFraction.FromString("365.29E9")).toString());
      Assert.assertEquals("-3.6528994766E+11", DecimalFraction.FromString("52.34E3").Subtract(DecimalFraction.FromString("365.29E9")).toString());
      Assert.assertEquals("1105.0000011612", DecimalFraction.FromString("110.5E1").Add(DecimalFraction.FromString("116.12E-8")).toString());
      Assert.assertEquals("1104.9999988388", DecimalFraction.FromString("110.5E1").Subtract(DecimalFraction.FromString("116.12E-8")).toString());
      Assert.assertEquals("76.16000015118", DecimalFraction.FromString("151.18E-9").Add(DecimalFraction.FromString("76.16E0")).toString());
      Assert.assertEquals("-76.15999984882", DecimalFraction.FromString("151.18E-9").Subtract(DecimalFraction.FromString("76.16E0")).toString());
      Assert.assertEquals("29837000022.18", DecimalFraction.FromString("298.37E8").Add(DecimalFraction.FromString("221.8E-1")).toString());
      Assert.assertEquals("29836999977.82", DecimalFraction.FromString("298.37E8").Subtract(DecimalFraction.FromString("221.8E-1")).toString());
      Assert.assertEquals("2724.313", DecimalFraction.FromString("268.9E1").Add(DecimalFraction.FromString("353.13E-1")).toString());
      Assert.assertEquals("2653.687", DecimalFraction.FromString("268.9E1").Subtract(DecimalFraction.FromString("353.13E-1")).toString());
      Assert.assertEquals("1233600.00000005427", DecimalFraction.FromString("54.27E-9").Add(DecimalFraction.FromString("123.36E4")).toString());
      Assert.assertEquals("-1233599.99999994573", DecimalFraction.FromString("54.27E-9").Subtract(DecimalFraction.FromString("123.36E4")).toString());
      Assert.assertEquals("445.00000008138", DecimalFraction.FromString("81.38E-9").Add(DecimalFraction.FromString("44.5E1")).toString());
      Assert.assertEquals("-444.99999991862", DecimalFraction.FromString("81.38E-9").Subtract(DecimalFraction.FromString("44.5E1")).toString());
      Assert.assertEquals("7.279933E+11", DecimalFraction.FromString("72.4E10").Add(DecimalFraction.FromString("399.33E7")).toString());
      Assert.assertEquals("7.200067E+11", DecimalFraction.FromString("72.4E10").Subtract(DecimalFraction.FromString("399.33E7")).toString());
      Assert.assertEquals("0.007319931", DecimalFraction.FromString("7.31E-3").Add(DecimalFraction.FromString("99.31E-7")).toString());
      Assert.assertEquals("0.007300069", DecimalFraction.FromString("7.31E-3").Subtract(DecimalFraction.FromString("99.31E-7")).toString());
      Assert.assertEquals("0.01833034824", DecimalFraction.FromString("18.33E-3").Add(DecimalFraction.FromString("348.24E-9")).toString());
      Assert.assertEquals("0.01832965176", DecimalFraction.FromString("18.33E-3").Subtract(DecimalFraction.FromString("348.24E-9")).toString());
      Assert.assertEquals("0.29144435", DecimalFraction.FromString("164.35E-6").Add(DecimalFraction.FromString("291.28E-3")).toString());
      Assert.assertEquals("-0.29111565", DecimalFraction.FromString("164.35E-6").Subtract(DecimalFraction.FromString("291.28E-3")).toString());
      Assert.assertEquals("2.11200E+8", DecimalFraction.FromString("191.39E6").Add(DecimalFraction.FromString("198.10E5")).toString());
      Assert.assertEquals("1.71580E+8", DecimalFraction.FromString("191.39E6").Subtract(DecimalFraction.FromString("198.10E5")).toString());
      Assert.assertEquals("2152.9000029623", DecimalFraction.FromString("296.23E-8").Add(DecimalFraction.FromString("215.29E1")).toString());
      Assert.assertEquals("-2152.8999970377", DecimalFraction.FromString("296.23E-8").Subtract(DecimalFraction.FromString("215.29E1")).toString());
      Assert.assertEquals("2.5135917E+8", DecimalFraction.FromString("49.17E3").Add(DecimalFraction.FromString("251.31E6")).toString());
      Assert.assertEquals("-2.5126083E+8", DecimalFraction.FromString("49.17E3").Subtract(DecimalFraction.FromString("251.31E6")).toString());
      Assert.assertEquals("3.92190003033E+12", DecimalFraction.FromString("392.19E10").Add(DecimalFraction.FromString("303.3E2")).toString());
      Assert.assertEquals("3.92189996967E+12", DecimalFraction.FromString("392.19E10").Subtract(DecimalFraction.FromString("303.3E2")).toString());
      Assert.assertEquals("58379", DecimalFraction.FromString("382.4E2").Add(DecimalFraction.FromString("201.39E2")).toString());
      Assert.assertEquals("18101", DecimalFraction.FromString("382.4E2").Subtract(DecimalFraction.FromString("201.39E2")).toString());
      Assert.assertEquals("28036000000.007917", DecimalFraction.FromString("79.17E-4").Add(DecimalFraction.FromString("280.36E8")).toString());
      Assert.assertEquals("-28035999999.992083", DecimalFraction.FromString("79.17E-4").Subtract(DecimalFraction.FromString("280.36E8")).toString());
      Assert.assertEquals("276431.37", DecimalFraction.FromString("131.37E0").Add(DecimalFraction.FromString("276.30E3")).toString());
      Assert.assertEquals("-276168.63", DecimalFraction.FromString("131.37E0").Subtract(DecimalFraction.FromString("276.30E3")).toString());
      Assert.assertEquals("25170015.439", DecimalFraction.FromString("25.17E6").Add(DecimalFraction.FromString("154.39E-1")).toString());
      Assert.assertEquals("25169984.561", DecimalFraction.FromString("25.17E6").Subtract(DecimalFraction.FromString("154.39E-1")).toString());
      Assert.assertEquals("2.173885E+10", DecimalFraction.FromString("217.17E8").Add(DecimalFraction.FromString("218.5E5")).toString());
      Assert.assertEquals("2.169515E+10", DecimalFraction.FromString("217.17E8").Subtract(DecimalFraction.FromString("218.5E5")).toString());
      Assert.assertEquals("529.03", DecimalFraction.FromString("18.19E1").Add(DecimalFraction.FromString("347.13E0")).toString());
      Assert.assertEquals("-165.23", DecimalFraction.FromString("18.19E1").Subtract(DecimalFraction.FromString("347.13E0")).toString());
      Assert.assertEquals("9.8420E+7", DecimalFraction.FromString("203.10E5").Add(DecimalFraction.FromString("78.11E6")).toString());
      Assert.assertEquals("-5.7800E+7", DecimalFraction.FromString("203.10E5").Subtract(DecimalFraction.FromString("78.11E6")).toString());
      Assert.assertEquals("8.71502282E+11", DecimalFraction.FromString("228.2E4").Add(DecimalFraction.FromString("87.15E10")).toString());
      Assert.assertEquals("-8.71497718E+11", DecimalFraction.FromString("228.2E4").Subtract(DecimalFraction.FromString("87.15E10")).toString());
      Assert.assertEquals("3571032111", DecimalFraction.FromString("321.11E2").Add(DecimalFraction.FromString("357.10E7")).toString());
      Assert.assertEquals("-3570967889", DecimalFraction.FromString("321.11E2").Subtract(DecimalFraction.FromString("357.10E7")).toString());
      Assert.assertEquals("5437316", DecimalFraction.FromString("54.26E5").Add(DecimalFraction.FromString("113.16E2")).toString());
      Assert.assertEquals("5414684", DecimalFraction.FromString("54.26E5").Subtract(DecimalFraction.FromString("113.16E2")).toString());
      Assert.assertEquals("1.2632837E+10", DecimalFraction.FromString("126.0E8").Add(DecimalFraction.FromString("328.37E5")).toString());
      Assert.assertEquals("1.2567163E+10", DecimalFraction.FromString("126.0E8").Subtract(DecimalFraction.FromString("328.37E5")).toString());
      Assert.assertEquals("200300000.00024232", DecimalFraction.FromString("200.30E6").Add(DecimalFraction.FromString("242.32E-6")).toString());
      Assert.assertEquals("200299999.99975768", DecimalFraction.FromString("200.30E6").Subtract(DecimalFraction.FromString("242.32E-6")).toString());
      Assert.assertEquals("0.00000275430", DecimalFraction.FromString("237.20E-8").Add(DecimalFraction.FromString("382.30E-9")).toString());
      Assert.assertEquals("0.00000198970", DecimalFraction.FromString("237.20E-8").Subtract(DecimalFraction.FromString("382.30E-9")).toString());
      Assert.assertEquals("2121600.0000011618", DecimalFraction.FromString("116.18E-8").Add(DecimalFraction.FromString("212.16E4")).toString());
      Assert.assertEquals("-2121599.9999988382", DecimalFraction.FromString("116.18E-8").Subtract(DecimalFraction.FromString("212.16E4")).toString());
      Assert.assertEquals("2266000.030138", DecimalFraction.FromString("226.6E4").Add(DecimalFraction.FromString("301.38E-4")).toString());
      Assert.assertEquals("2265999.969862", DecimalFraction.FromString("226.6E4").Subtract(DecimalFraction.FromString("301.38E-4")).toString());
      Assert.assertEquals("3541200000011.831", DecimalFraction.FromString("118.31E-1").Add(DecimalFraction.FromString("354.12E10")).toString());
      Assert.assertEquals("-3541199999988.169", DecimalFraction.FromString("118.31E-1").Subtract(DecimalFraction.FromString("354.12E10")).toString());
      Assert.assertEquals("26034.034113", DecimalFraction.FromString("260.34E2").Add(DecimalFraction.FromString("341.13E-4")).toString());
      Assert.assertEquals("26033.965887", DecimalFraction.FromString("260.34E2").Subtract(DecimalFraction.FromString("341.13E-4")).toString());
      Assert.assertEquals("29534000000.0003890", DecimalFraction.FromString("389.0E-6").Add(DecimalFraction.FromString("295.34E8")).toString());
      Assert.assertEquals("-29533999999.9996110", DecimalFraction.FromString("389.0E-6").Subtract(DecimalFraction.FromString("295.34E8")).toString());
      Assert.assertEquals("1081.7", DecimalFraction.FromString("0.9E3").Add(DecimalFraction.FromString("18.17E1")).toString());
      Assert.assertEquals("718.3", DecimalFraction.FromString("0.9E3").Subtract(DecimalFraction.FromString("18.17E1")).toString());
      Assert.assertEquals("41550.24", DecimalFraction.FromString("290.24E0").Add(DecimalFraction.FromString("41.26E3")).toString());
      Assert.assertEquals("-40969.76", DecimalFraction.FromString("290.24E0").Subtract(DecimalFraction.FromString("41.26E3")).toString());
      Assert.assertEquals("161.370018036", DecimalFraction.FromString("161.37E0").Add(DecimalFraction.FromString("180.36E-7")).toString());
      Assert.assertEquals("161.369981964", DecimalFraction.FromString("161.37E0").Subtract(DecimalFraction.FromString("180.36E-7")).toString());
      Assert.assertEquals("1.3418722E+8", DecimalFraction.FromString("134.13E6").Add(DecimalFraction.FromString("57.22E3")).toString());
      Assert.assertEquals("1.3407278E+8", DecimalFraction.FromString("134.13E6").Subtract(DecimalFraction.FromString("57.22E3")).toString());
      Assert.assertEquals("0.0000389329", DecimalFraction.FromString("35.20E-6").Add(DecimalFraction.FromString("373.29E-8")).toString());
      Assert.assertEquals("0.0000314671", DecimalFraction.FromString("35.20E-6").Subtract(DecimalFraction.FromString("373.29E-8")).toString());
      Assert.assertEquals("179000000000.33714", DecimalFraction.FromString("337.14E-3").Add(DecimalFraction.FromString("17.9E10")).toString());
      Assert.assertEquals("-178999999999.66286", DecimalFraction.FromString("337.14E-3").Subtract(DecimalFraction.FromString("17.9E10")).toString());
      Assert.assertEquals("79150000000.0035124", DecimalFraction.FromString("79.15E9").Add(DecimalFraction.FromString("351.24E-5")).toString());
      Assert.assertEquals("79149999999.9964876", DecimalFraction.FromString("79.15E9").Subtract(DecimalFraction.FromString("351.24E-5")).toString());
      Assert.assertEquals("2.29225713E+12", DecimalFraction.FromString("229.20E10").Add(DecimalFraction.FromString("257.13E6")).toString());
      Assert.assertEquals("2.29174287E+12", DecimalFraction.FromString("229.20E10").Subtract(DecimalFraction.FromString("257.13E6")).toString());
      Assert.assertEquals("350160.05632", DecimalFraction.FromString("56.32E-3").Add(DecimalFraction.FromString("350.16E3")).toString());
      Assert.assertEquals("-350159.94368", DecimalFraction.FromString("56.32E-3").Subtract(DecimalFraction.FromString("350.16E3")).toString());
      Assert.assertEquals("101600.0000000955", DecimalFraction.FromString("10.16E4").Add(DecimalFraction.FromString("95.5E-9")).toString());
      Assert.assertEquals("101599.9999999045", DecimalFraction.FromString("10.16E4").Subtract(DecimalFraction.FromString("95.5E-9")).toString());
      Assert.assertEquals("1131000000.001075", DecimalFraction.FromString("107.5E-5").Add(DecimalFraction.FromString("113.10E7")).toString());
      Assert.assertEquals("-1130999999.998925", DecimalFraction.FromString("107.5E-5").Subtract(DecimalFraction.FromString("113.10E7")).toString());
      Assert.assertEquals("597.30", DecimalFraction.FromString("227.2E0").Add(DecimalFraction.FromString("370.10E0")).toString());
      Assert.assertEquals("-142.90", DecimalFraction.FromString("227.2E0").Subtract(DecimalFraction.FromString("370.10E0")).toString());
      Assert.assertEquals("371491.2", DecimalFraction.FromString("189.12E1").Add(DecimalFraction.FromString("369.6E3")).toString());
      Assert.assertEquals("-367708.8", DecimalFraction.FromString("189.12E1").Subtract(DecimalFraction.FromString("369.6E3")).toString());
      Assert.assertEquals("291260000.0003901", DecimalFraction.FromString("390.1E-6").Add(DecimalFraction.FromString("291.26E6")).toString());
      Assert.assertEquals("-291259999.9996099", DecimalFraction.FromString("390.1E-6").Subtract(DecimalFraction.FromString("291.26E6")).toString());
      Assert.assertEquals("26.13600029222", DecimalFraction.FromString("261.36E-1").Add(DecimalFraction.FromString("292.22E-9")).toString());
      Assert.assertEquals("26.13599970778", DecimalFraction.FromString("261.36E-1").Subtract(DecimalFraction.FromString("292.22E-9")).toString());
      Assert.assertEquals("327190000000.0000003319", DecimalFraction.FromString("33.19E-8").Add(DecimalFraction.FromString("327.19E9")).toString());
      Assert.assertEquals("-327189999999.9999996681", DecimalFraction.FromString("33.19E-8").Subtract(DecimalFraction.FromString("327.19E9")).toString());
      Assert.assertEquals("185.802104", DecimalFraction.FromString("210.4E-5").Add(DecimalFraction.FromString("185.8E0")).toString());
      Assert.assertEquals("-185.797896", DecimalFraction.FromString("210.4E-5").Subtract(DecimalFraction.FromString("185.8E0")).toString());
      Assert.assertEquals("2.243535637E+12", DecimalFraction.FromString("224.35E10").Add(DecimalFraction.FromString("356.37E5")).toString());
      Assert.assertEquals("2.243464363E+12", DecimalFraction.FromString("224.35E10").Subtract(DecimalFraction.FromString("356.37E5")).toString());
      Assert.assertEquals("472700.01048", DecimalFraction.FromString("47.27E4").Add(DecimalFraction.FromString("104.8E-4")).toString());
      Assert.assertEquals("472699.98952", DecimalFraction.FromString("47.27E4").Subtract(DecimalFraction.FromString("104.8E-4")).toString());
      Assert.assertEquals("1471800.02795", DecimalFraction.FromString("147.18E4").Add(DecimalFraction.FromString("279.5E-4")).toString());
      Assert.assertEquals("1471799.97205", DecimalFraction.FromString("147.18E4").Subtract(DecimalFraction.FromString("279.5E-4")).toString());
      Assert.assertEquals("0.33453", DecimalFraction.FromString("11.5E-4").Add(DecimalFraction.FromString("333.38E-3")).toString());
      Assert.assertEquals("-0.33223", DecimalFraction.FromString("11.5E-4").Subtract(DecimalFraction.FromString("333.38E-3")).toString());
      Assert.assertEquals("0.531437", DecimalFraction.FromString("5.28E-1").Add(DecimalFraction.FromString("343.7E-5")).toString());
      Assert.assertEquals("0.524563", DecimalFraction.FromString("5.28E-1").Subtract(DecimalFraction.FromString("343.7E-5")).toString());
      Assert.assertEquals("1.251214E+9", DecimalFraction.FromString("381.14E5").Add(DecimalFraction.FromString("121.31E7")).toString());
      Assert.assertEquals("-1.174986E+9", DecimalFraction.FromString("381.14E5").Subtract(DecimalFraction.FromString("121.31E7")).toString());
      Assert.assertEquals("15016.2", DecimalFraction.FromString("145.25E2").Add(DecimalFraction.FromString("49.12E1")).toString());
      Assert.assertEquals("14033.8", DecimalFraction.FromString("145.25E2").Subtract(DecimalFraction.FromString("49.12E1")).toString());
      Assert.assertEquals("173700000332.13", DecimalFraction.FromString("332.13E0").Add(DecimalFraction.FromString("173.7E9")).toString());
      Assert.assertEquals("-173699999667.87", DecimalFraction.FromString("332.13E0").Subtract(DecimalFraction.FromString("173.7E9")).toString());
      Assert.assertEquals("0.38234333", DecimalFraction.FromString("38.20E-2").Add(DecimalFraction.FromString("343.33E-6")).toString());
      Assert.assertEquals("0.38165667", DecimalFraction.FromString("38.20E-2").Subtract(DecimalFraction.FromString("343.33E-6")).toString());
      Assert.assertEquals("415000017.234", DecimalFraction.FromString("4.15E8").Add(DecimalFraction.FromString("172.34E-1")).toString());
      Assert.assertEquals("414999982.766", DecimalFraction.FromString("4.15E8").Subtract(DecimalFraction.FromString("172.34E-1")).toString());
      Assert.assertEquals("3.5335001591E+12", DecimalFraction.FromString("353.35E10").Add(DecimalFraction.FromString("159.1E3")).toString());
      Assert.assertEquals("3.5334998409E+12", DecimalFraction.FromString("353.35E10").Subtract(DecimalFraction.FromString("159.1E3")).toString());
      Assert.assertEquals("16414.6838", DecimalFraction.FromString("268.38E-2").Add(DecimalFraction.FromString("164.12E2")).toString());
      Assert.assertEquals("-16409.3162", DecimalFraction.FromString("268.38E-2").Subtract(DecimalFraction.FromString("164.12E2")).toString());
      Assert.assertEquals("1.4010003544E+12", DecimalFraction.FromString("354.4E3").Add(DecimalFraction.FromString("140.1E10")).toString());
      Assert.assertEquals("-1.4009996456E+12", DecimalFraction.FromString("354.4E3").Subtract(DecimalFraction.FromString("140.1E10")).toString());
      Assert.assertEquals("2083800000.0007613", DecimalFraction.FromString("76.13E-5").Add(DecimalFraction.FromString("208.38E7")).toString());
      Assert.assertEquals("-2083799999.9992387", DecimalFraction.FromString("76.13E-5").Subtract(DecimalFraction.FromString("208.38E7")).toString());
      Assert.assertEquals("14.91800012724", DecimalFraction.FromString("127.24E-9").Add(DecimalFraction.FromString("149.18E-1")).toString());
      Assert.assertEquals("-14.91799987276", DecimalFraction.FromString("127.24E-9").Subtract(DecimalFraction.FromString("149.18E-1")).toString());
      Assert.assertEquals("0.00023156", DecimalFraction.FromString("19.34E-5").Add(DecimalFraction.FromString("38.16E-6")).toString());
      Assert.assertEquals("0.00015524", DecimalFraction.FromString("19.34E-5").Subtract(DecimalFraction.FromString("38.16E-6")).toString());
      Assert.assertEquals("12538.0000020020", DecimalFraction.FromString("125.38E2").Add(DecimalFraction.FromString("200.20E-8")).toString());
      Assert.assertEquals("12537.9999979980", DecimalFraction.FromString("125.38E2").Subtract(DecimalFraction.FromString("200.20E-8")).toString());
      Assert.assertEquals("0.00051186", DecimalFraction.FromString("127.16E-6").Add(DecimalFraction.FromString("384.7E-6")).toString());
      Assert.assertEquals("-0.00025754", DecimalFraction.FromString("127.16E-6").Subtract(DecimalFraction.FromString("384.7E-6")).toString());
      Assert.assertEquals("707000.00009722", DecimalFraction.FromString("70.7E4").Add(DecimalFraction.FromString("97.22E-6")).toString());
      Assert.assertEquals("706999.99990278", DecimalFraction.FromString("70.7E4").Subtract(DecimalFraction.FromString("97.22E-6")).toString());
      Assert.assertEquals("2.8697E+10", DecimalFraction.FromString("109.7E8").Add(DecimalFraction.FromString("177.27E8")).toString());
      Assert.assertEquals("-6.757E+9", DecimalFraction.FromString("109.7E8").Subtract(DecimalFraction.FromString("177.27E8")).toString());
      Assert.assertEquals("276350.0000012426", DecimalFraction.FromString("124.26E-8").Add(DecimalFraction.FromString("276.35E3")).toString());
      Assert.assertEquals("-276349.9999987574", DecimalFraction.FromString("124.26E-8").Subtract(DecimalFraction.FromString("276.35E3")).toString());
      Assert.assertEquals("56352719", DecimalFraction.FromString("56.34E6").Add(DecimalFraction.FromString("127.19E2")).toString());
      Assert.assertEquals("56327281", DecimalFraction.FromString("56.34E6").Subtract(DecimalFraction.FromString("127.19E2")).toString());
      Assert.assertEquals("1.3220031539E+11", DecimalFraction.FromString("132.20E9").Add(DecimalFraction.FromString("315.39E3")).toString());
      Assert.assertEquals("1.3219968461E+11", DecimalFraction.FromString("132.20E9").Subtract(DecimalFraction.FromString("315.39E3")).toString());
      Assert.assertEquals("6.3272236E+8", DecimalFraction.FromString("22.36E3").Add(DecimalFraction.FromString("63.27E7")).toString());
      Assert.assertEquals("-6.3267764E+8", DecimalFraction.FromString("22.36E3").Subtract(DecimalFraction.FromString("63.27E7")).toString());
      Assert.assertEquals("151380000000.05331", DecimalFraction.FromString("151.38E9").Add(DecimalFraction.FromString("53.31E-3")).toString());
      Assert.assertEquals("151379999999.94669", DecimalFraction.FromString("151.38E9").Subtract(DecimalFraction.FromString("53.31E-3")).toString());
      Assert.assertEquals("24522000.00000004119", DecimalFraction.FromString("245.22E5").Add(DecimalFraction.FromString("41.19E-9")).toString());
      Assert.assertEquals("24521999.99999995881", DecimalFraction.FromString("245.22E5").Subtract(DecimalFraction.FromString("41.19E-9")).toString());
      Assert.assertEquals("32539.12334", DecimalFraction.FromString("123.34E-3").Add(DecimalFraction.FromString("325.39E2")).toString());
      Assert.assertEquals("-32538.87666", DecimalFraction.FromString("123.34E-3").Subtract(DecimalFraction.FromString("325.39E2")).toString());
    }
    /**
     * 
     */
@Test
    public void MultiplyTest() {
      Assert.assertEquals("1.23885300E+9", DecimalFraction.FromString("51.15E8").Multiply(DecimalFraction.FromString("242.20E-3")).toString());
      Assert.assertEquals("0.001106186758", DecimalFraction.FromString("373.22E-1").Multiply(DecimalFraction.FromString("296.39E-7")).toString());
      Assert.assertEquals("192.9180", DecimalFraction.FromString("11.0E-4").Multiply(DecimalFraction.FromString("175.38E3")).toString());
      Assert.assertEquals("0.000013640373", DecimalFraction.FromString("27.21E-6").Multiply(DecimalFraction.FromString("50.13E-2")).toString());
      Assert.assertEquals("0.00000515564630", DecimalFraction.FromString("138.11E-2").Multiply(DecimalFraction.FromString("373.30E-8")).toString());
      Assert.assertEquals("3.3450518E+8", DecimalFraction.FromString("221.38E9").Multiply(DecimalFraction.FromString("15.11E-4")).toString());
      Assert.assertEquals("0.0000033748442", DecimalFraction.FromString("278.2E-5").Multiply(DecimalFraction.FromString("121.31E-5")).toString());
      Assert.assertEquals("1.039277030E+15", DecimalFraction.FromString("369.35E0").Multiply(DecimalFraction.FromString("281.38E10")).toString());
      Assert.assertEquals("237138.92", DecimalFraction.FromString("393.2E-1").Multiply(DecimalFraction.FromString("60.31E2")).toString());
      Assert.assertEquals("6.5073942E+11", DecimalFraction.FromString("208.17E3").Multiply(DecimalFraction.FromString("31.26E5")).toString());
      Assert.assertEquals("1685.5032", DecimalFraction.FromString("7.32E0").Multiply(DecimalFraction.FromString("230.26E0")).toString());
      Assert.assertEquals("0.00441400570", DecimalFraction.FromString("170.30E-1").Multiply(DecimalFraction.FromString("259.19E-6")).toString());
      Assert.assertEquals("4.41514794E+9", DecimalFraction.FromString("326.13E3").Multiply(DecimalFraction.FromString("135.38E2")).toString());
      Assert.assertEquals("139070.220", DecimalFraction.FromString("82.12E5").Multiply(DecimalFraction.FromString("169.35E-4")).toString());
      Assert.assertEquals("1.182023125E+17", DecimalFraction.FromString("319.25E3").Multiply(DecimalFraction.FromString("370.25E9")).toString());
      Assert.assertEquals("18397.593", DecimalFraction.FromString("12.33E3").Multiply(DecimalFraction.FromString("149.21E-2")).toString());
      Assert.assertEquals("8.0219160E+14", DecimalFraction.FromString("170.10E10").Multiply(DecimalFraction.FromString("47.16E1")).toString());
      Assert.assertEquals("8.23380426E+11", DecimalFraction.FromString("219.34E-3").Multiply(DecimalFraction.FromString("375.39E10")).toString());
      Assert.assertEquals("1036.89700", DecimalFraction.FromString("318.8E1").Multiply(DecimalFraction.FromString("325.25E-3")).toString());
      Assert.assertEquals("1013.077141", DecimalFraction.FromString("319.19E-3").Multiply(DecimalFraction.FromString("317.39E1")).toString());
      Assert.assertEquals("1.2831563E+13", DecimalFraction.FromString("14.39E6").Multiply(DecimalFraction.FromString("89.17E4")).toString());
      Assert.assertEquals("0.036472384", DecimalFraction.FromString("386.36E1").Multiply(DecimalFraction.FromString("94.4E-7")).toString());
      Assert.assertEquals("7.5994752E+16", DecimalFraction.FromString("280.32E6").Multiply(DecimalFraction.FromString("271.1E6")).toString());
      Assert.assertEquals("4.1985417", DecimalFraction.FromString("107.3E-5").Multiply(DecimalFraction.FromString("391.29E1")).toString());
      Assert.assertEquals("81530.63", DecimalFraction.FromString("31.37E-5").Multiply(DecimalFraction.FromString("259.9E6")).toString());
      Assert.assertEquals("4.543341E-10", DecimalFraction.FromString("372.1E-7").Multiply(DecimalFraction.FromString("12.21E-6")).toString());
      Assert.assertEquals("3.77698530E-9", DecimalFraction.FromString("306.30E-7").Multiply(DecimalFraction.FromString("123.31E-6")).toString());
      Assert.assertEquals("3.708195E+9", DecimalFraction.FromString("318.3E10").Multiply(DecimalFraction.FromString("116.5E-5")).toString());
      Assert.assertEquals("413.87661", DecimalFraction.FromString("252.21E-5").Multiply(DecimalFraction.FromString("164.1E3")).toString());
      Assert.assertEquals("7.1053840E+8", DecimalFraction.FromString("124.22E-4").Multiply(DecimalFraction.FromString("57.20E9")).toString());
      Assert.assertEquals("481.335452", DecimalFraction.FromString("178.18E-7").Multiply(DecimalFraction.FromString("270.14E5")).toString());
      Assert.assertEquals("2.61361E-10", DecimalFraction.FromString("84.31E-3").Multiply(DecimalFraction.FromString("3.1E-9")).toString());
      Assert.assertEquals("2.00365428E-7", DecimalFraction.FromString("84.12E-8").Multiply(DecimalFraction.FromString("238.19E-3")).toString());
      Assert.assertEquals("0.0000259582890", DecimalFraction.FromString("153.30E-9").Multiply(DecimalFraction.FromString("169.33E0")).toString());
      Assert.assertEquals("10263.70", DecimalFraction.FromString("98.5E-8").Multiply(DecimalFraction.FromString("104.2E8")).toString());
      Assert.assertEquals("0.057940056", DecimalFraction.FromString("77.13E-7").Multiply(DecimalFraction.FromString("75.12E2")).toString());
      Assert.assertEquals("169852062", DecimalFraction.FromString("89.33E-6").Multiply(DecimalFraction.FromString("190.14E10")).toString());
      Assert.assertEquals("1384468.2", DecimalFraction.FromString("252.18E6").Multiply(DecimalFraction.FromString("54.9E-4")).toString());
      Assert.assertEquals("1.4882985E+12", DecimalFraction.FromString("46.35E-1").Multiply(DecimalFraction.FromString("32.11E10")).toString());
      Assert.assertEquals("2.7130378E+10", DecimalFraction.FromString("347.38E5").Multiply(DecimalFraction.FromString("78.1E1")).toString());
      Assert.assertEquals("1.1816933E-10", DecimalFraction.FromString("31.27E-5").Multiply(DecimalFraction.FromString("377.9E-9")).toString());
      Assert.assertEquals("3.9434566E+10", DecimalFraction.FromString("119.8E-4").Multiply(DecimalFraction.FromString("329.17E10")).toString());
      Assert.assertEquals("5.72427", DecimalFraction.FromString("19.1E-2").Multiply(DecimalFraction.FromString("299.7E-1")).toString());
      Assert.assertEquals("1.890600E+17", DecimalFraction.FromString("82.2E9").Multiply(DecimalFraction.FromString("230.0E4")).toString());
      Assert.assertEquals("8.24813976E+11", DecimalFraction.FromString("398.23E5").Multiply(DecimalFraction.FromString("207.12E2")).toString());
      Assert.assertEquals("9.923540E+14", DecimalFraction.FromString("47.30E8").Multiply(DecimalFraction.FromString("209.8E3")).toString());
      Assert.assertEquals("13682832.2", DecimalFraction.FromString("383.38E-5").Multiply(DecimalFraction.FromString("356.9E7")).toString());
      Assert.assertEquals("1.476482154E+10", DecimalFraction.FromString("375.38E-3").Multiply(DecimalFraction.FromString("393.33E8")).toString());
      Assert.assertEquals("1.036217389E+19", DecimalFraction.FromString("285.31E9").Multiply(DecimalFraction.FromString("363.19E5")).toString());
      Assert.assertEquals("951399862", DecimalFraction.FromString("252.14E8").Multiply(DecimalFraction.FromString("377.33E-4")).toString());
      Assert.assertEquals("1.143972712E+16", DecimalFraction.FromString("307.28E4").Multiply(DecimalFraction.FromString("372.29E7")).toString());
      Assert.assertEquals("602.640", DecimalFraction.FromString("2.16E8").Multiply(DecimalFraction.FromString("279.0E-8")).toString());
      Assert.assertEquals("5711.3430", DecimalFraction.FromString("182.18E-9").Multiply(DecimalFraction.FromString("31.35E9")).toString());
      Assert.assertEquals("366.054821", DecimalFraction.FromString("149.27E-4").Multiply(DecimalFraction.FromString("245.23E2")).toString());
      Assert.assertEquals("12901.2750", DecimalFraction.FromString("372.6E-1").Multiply(DecimalFraction.FromString("346.25E0")).toString());
      Assert.assertEquals("201642636", DecimalFraction.FromString("61.23E-1").Multiply(DecimalFraction.FromString("329.32E5")).toString());
      Assert.assertEquals("1.64376210E+16", DecimalFraction.FromString("133.26E10").Multiply(DecimalFraction.FromString("123.35E2")).toString());
      Assert.assertEquals("3.084818E+18", DecimalFraction.FromString("309.1E9").Multiply(DecimalFraction.FromString("99.8E5")).toString());
      Assert.assertEquals("0.4925852", DecimalFraction.FromString("230.18E4").Multiply(DecimalFraction.FromString("2.14E-7")).toString());
      Assert.assertEquals("322.455112", DecimalFraction.FromString("387.38E-3").Multiply(DecimalFraction.FromString("83.24E1")).toString());
      Assert.assertEquals("0.9306528", DecimalFraction.FromString("377.7E-2").Multiply(DecimalFraction.FromString("246.4E-3")).toString());
      Assert.assertEquals("2.251919", DecimalFraction.FromString("169.7E0").Multiply(DecimalFraction.FromString("13.27E-3")).toString());
      Assert.assertEquals("682846382", DecimalFraction.FromString("385.31E3").Multiply(DecimalFraction.FromString("177.22E1")).toString());
      Assert.assertEquals("11338.90625", DecimalFraction.FromString("306.25E-7").Multiply(DecimalFraction.FromString("370.25E6")).toString());
      Assert.assertEquals("1.3389740E+9", DecimalFraction.FromString("49.0E9").Multiply(DecimalFraction.FromString("273.26E-4")).toString());
      Assert.assertEquals("5.4483030E+18", DecimalFraction.FromString("160.15E6").Multiply(DecimalFraction.FromString("340.2E8")).toString());
      Assert.assertEquals("9.3219568E+8", DecimalFraction.FromString("109.31E3").Multiply(DecimalFraction.FromString("85.28E2")).toString());
      Assert.assertEquals("6.9666450", DecimalFraction.FromString("90.30E1").Multiply(DecimalFraction.FromString("77.15E-4")).toString());
      Assert.assertEquals("1.25459658E-7", DecimalFraction.FromString("81.33E-3").Multiply(DecimalFraction.FromString("154.26E-8")).toString());
      Assert.assertEquals("0.0001433757", DecimalFraction.FromString("378.3E-5").Multiply(DecimalFraction.FromString("37.9E-3")).toString());
      Assert.assertEquals("275.60856", DecimalFraction.FromString("310.37E-5").Multiply(DecimalFraction.FromString("88.8E3")).toString());
      Assert.assertEquals("70.4246032", DecimalFraction.FromString("188.12E-9").Multiply(DecimalFraction.FromString("374.36E6")).toString());
      Assert.assertEquals("2.0905404E+9", DecimalFraction.FromString("75.4E1").Multiply(DecimalFraction.FromString("277.26E4")).toString());
      Assert.assertEquals("8.5164440E+16", DecimalFraction.FromString("346.0E4").Multiply(DecimalFraction.FromString("246.14E8")).toString());
      Assert.assertEquals("5836929.0", DecimalFraction.FromString("41.30E1").Multiply(DecimalFraction.FromString("141.33E2")).toString());
      Assert.assertEquals("9.632727E-8", DecimalFraction.FromString("44.37E-8").Multiply(DecimalFraction.FromString("217.1E-3")).toString());
      Assert.assertEquals("1.0707983E+14", DecimalFraction.FromString("7.27E1").Multiply(DecimalFraction.FromString("147.29E10")).toString());
      Assert.assertEquals("650476.8", DecimalFraction.FromString("165.6E6").Multiply(DecimalFraction.FromString("392.8E-5")).toString());
      Assert.assertEquals("5.9438181E+9", DecimalFraction.FromString("309.3E-1").Multiply(DecimalFraction.FromString("192.17E6")).toString());
      Assert.assertEquals("5.07150E+14", DecimalFraction.FromString("48.30E3").Multiply(DecimalFraction.FromString("10.5E9")).toString());
      Assert.assertEquals("687748.662", DecimalFraction.FromString("333.26E5").Multiply(DecimalFraction.FromString("206.37E-4")).toString());
      Assert.assertEquals("18.3678360", DecimalFraction.FromString("49.20E3").Multiply(DecimalFraction.FromString("373.33E-6")).toString());
      Assert.assertEquals("2.071383E+13", DecimalFraction.FromString("252.3E0").Multiply(DecimalFraction.FromString("8.21E10")).toString());
      Assert.assertEquals("2.86793244E+21", DecimalFraction.FromString("96.12E8").Multiply(DecimalFraction.FromString("298.37E9")).toString());
      Assert.assertEquals("1.346378792E+16", DecimalFraction.FromString("342.32E3").Multiply(DecimalFraction.FromString("393.31E8")).toString());
      Assert.assertEquals("4.5974844E-8", DecimalFraction.FromString("40.23E-2").Multiply(DecimalFraction.FromString("114.28E-9")).toString());
      Assert.assertEquals("0.74529156", DecimalFraction.FromString("320.28E5").Multiply(DecimalFraction.FromString("23.27E-9")).toString());
      Assert.assertEquals("8398794.5", DecimalFraction.FromString("372.7E-1").Multiply(DecimalFraction.FromString("225.35E3")).toString());
      Assert.assertEquals("5.9243200E+9", DecimalFraction.FromString("303.5E-5").Multiply(DecimalFraction.FromString("195.20E10")).toString());
      Assert.assertEquals("0.14321792", DecimalFraction.FromString("131.2E-7").Multiply(DecimalFraction.FromString("109.16E2")).toString());
      Assert.assertEquals("4.9518322E+11", DecimalFraction.FromString("230.2E2").Multiply(DecimalFraction.FromString("215.11E5")).toString());
      Assert.assertEquals("14.1640814", DecimalFraction.FromString("170.18E4").Multiply(DecimalFraction.FromString("83.23E-7")).toString());
      Assert.assertEquals("1.18653228E-7", DecimalFraction.FromString("102.12E-9").Multiply(DecimalFraction.FromString("116.19E-2")).toString());
      Assert.assertEquals("20220.7104", DecimalFraction.FromString("319.14E3").Multiply(DecimalFraction.FromString("63.36E-3")).toString());
      Assert.assertEquals("1.003818480E+23", DecimalFraction.FromString("263.20E8").Multiply(DecimalFraction.FromString("381.39E10")).toString());
      Assert.assertEquals("0.0270150690", DecimalFraction.FromString("350.39E-6").Multiply(DecimalFraction.FromString("77.10E0")).toString());
      Assert.assertEquals("3.338496E+19", DecimalFraction.FromString("124.2E8").Multiply(DecimalFraction.FromString("268.8E7")).toString());
      Assert.assertEquals("15983.9650", DecimalFraction.FromString("60.26E4").Multiply(DecimalFraction.FromString("265.25E-4")).toString());
      Assert.assertEquals("14.674005", DecimalFraction.FromString("139.5E3").Multiply(DecimalFraction.FromString("105.19E-6")).toString());
      Assert.assertEquals("3469019.40", DecimalFraction.FromString("160.38E2").Multiply(DecimalFraction.FromString("216.30E0")).toString());
    }
    
    // Tests whether AsInt32/64/16/AsByte properly truncate floats
    // and doubles before bounds checking
    @Test
    public void FloatingPointCloseToEdge() {
      try { CBORObject.FromObject(2.147483647E9d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.147483647E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.147483647E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.147483647E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836470000002E9d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836470000002E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836470000002E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836470000002E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836469999998E9d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836469999998E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836469999998E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836469999998E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.147483648E9d).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.147483648E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.147483648E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.147483648E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836480000005E9d).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836480000005E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836480000005E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836480000005E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836479999998E9d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836479999998E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836479999998E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836479999998E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.147483646E9d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.147483646E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.147483646E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.147483646E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836460000002E9d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836460000002E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836460000002E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836460000002E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836459999998E9d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836459999998E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836459999998E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474836459999998E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.147483648E9d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.147483648E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.147483648E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.147483648E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836479999998E9d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836479999998E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836479999998E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836479999998E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836480000005E9d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836480000005E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836480000005E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836480000005E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.147483647E9d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.147483647E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.147483647E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.147483647E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836469999998E9d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836469999998E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836469999998E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836469999998E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836470000002E9d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836470000002E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836470000002E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836470000002E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.147483649E9d).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.147483649E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.147483649E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.147483649E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836489999995E9d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836489999995E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836489999995E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836489999995E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836490000005E9d).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836490000005E9d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836490000005E9d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474836490000005E9d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223372036854776E18d).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223372036854776E18d).AsInt64(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223372036854776E18d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223372036854776E18d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223372036854778E18d).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223372036854778E18d).AsInt64(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223372036854778E18d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223372036854778E18d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.2233720368547748E18d).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.2233720368547748E18d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.2233720368547748E18d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.2233720368547748E18d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223372036854776E18d).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223372036854776E18d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223372036854776E18d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223372036854776E18d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.2233720368547748E18d).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.2233720368547748E18d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.2233720368547748E18d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.2233720368547748E18d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223372036854778E18d).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223372036854778E18d).AsInt64(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223372036854778E18d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223372036854778E18d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.0d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.0d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.0d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.0d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.000000000004d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.000000000004d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.000000000004d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.000000000004d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.999999999996d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.999999999996d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.999999999996d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.999999999996d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.0d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.0d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.0d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.0d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.00000000001d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.00000000001d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.00000000001d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.00000000001d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.999999999996d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.999999999996d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.999999999996d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.999999999996d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.0d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.0d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.0d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.0d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.000000000004d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.000000000004d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.000000000004d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.000000000004d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32765.999999999996d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32765.999999999996d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32765.999999999996d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32765.999999999996d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.0d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.0d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.0d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.0d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.999999999996d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.999999999996d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.999999999996d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.999999999996d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.00000000001d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.00000000001d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.00000000001d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.00000000001d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.0d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.0d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.0d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.0d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32766.999999999996d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32766.999999999996d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32766.999999999996d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32766.999999999996d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.000000000004d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.000000000004d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.000000000004d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.000000000004d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.0d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.0d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.0d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.0d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.99999999999d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.99999999999d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.99999999999d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.99999999999d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.00000000001d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.00000000001d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.00000000001d).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.00000000001d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.0d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.0d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.0d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.0d).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(4.9E-324d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(4.9E-324d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(4.9E-324d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(4.9E-324d).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-4.9E-324d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-4.9E-324d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-4.9E-324d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-4.9E-324d).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0d).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0000000000000002d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0000000000000002d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0000000000000002d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0000000000000002d).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.9999999999999999d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.9999999999999999d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.9999999999999999d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.9999999999999999d).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-0.9999999999999999d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-0.9999999999999999d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-0.9999999999999999d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-0.9999999999999999d).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0000000000000002d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0000000000000002d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0000000000000002d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0000000000000002d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.0d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.0d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.0d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.0d).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.00000000000003d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.00000000000003d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.00000000000003d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.00000000000003d).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.99999999999997d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.99999999999997d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.99999999999997d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.99999999999997d).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.0d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.0d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.0d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.0d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.00000000000006d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.00000000000006d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.00000000000006d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.00000000000006d).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.99999999999997d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.99999999999997d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.99999999999997d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.99999999999997d).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.0d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.0d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.0d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.0d).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.00000000000003d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.00000000000003d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.00000000000003d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.00000000000003d).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(253.99999999999997d).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(253.99999999999997d).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(253.99999999999997d).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(253.99999999999997d).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.14748365E9f).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.14748365E9f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.14748365E9f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.14748365E9f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474839E9f).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474839E9f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474839E9f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.1474839E9f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.14748352E9f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.14748352E9f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.14748352E9f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(2.14748352E9f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.14748365E9f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.14748365E9f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.14748365E9f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.14748365E9f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.14748352E9f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.14748352E9f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.14748352E9f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.14748352E9f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474839E9f).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474839E9f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474839E9f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-2.1474839E9f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223372E18f).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223372E18f).AsInt64(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223372E18f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223372E18f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223373E18f).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223373E18f).AsInt64(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223373E18f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.223373E18f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.2233715E18f).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.2233715E18f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.2233715E18f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(9.2233715E18f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223372E18f).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223372E18f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223372E18f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223372E18f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.2233715E18f).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.2233715E18f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.2233715E18f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.2233715E18f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223373E18f).AsInt32(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223373E18f).AsInt64(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223373E18f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-9.223373E18f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.0f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.0f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.0f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.0f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.002f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.002f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.002f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.002f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.998f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.998f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.998f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.998f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.0f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.0f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.0f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.0f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.004f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.004f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.004f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32768.004f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.998f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.998f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.998f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32767.998f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.0f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.0f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.0f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.0f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.002f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.002f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.002f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32766.002f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32765.998f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32765.998f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32765.998f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(32765.998f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.0f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.0f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.0f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.0f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.998f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.998f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.998f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.998f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.004f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.004f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.004f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.004f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.0f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.0f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.0f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.0f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32766.998f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32766.998f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32766.998f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32766.998f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.002f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.002f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.002f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32767.002f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.0f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.0f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.0f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.0f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.996f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.996f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.996f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32768.996f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.004f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.004f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.004f).AsInt16(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-32769.004f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.0f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.0f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.0f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.0f).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.4E-45f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.4E-45f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.4E-45f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.4E-45f).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.4E-45f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.4E-45f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.4E-45f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.4E-45f).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0f).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0000001f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0000001f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0000001f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(1.0000001f).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.99999994f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.99999994f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.99999994f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(0.99999994f).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-0.99999994f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-0.99999994f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-0.99999994f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-0.99999994f).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0000001f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0000001f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0000001f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(-1.0000001f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.0f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.0f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.0f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.0f).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.00002f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.00002f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.00002f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.00002f).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.99998f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.99998f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.99998f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.99998f).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.0f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.0f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.0f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.0f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.00003f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.00003f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.00003f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(256.00003f).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.99998f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.99998f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.99998f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(255.99998f).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.0f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.0f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.0f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.0f).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.00002f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.00002f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.00002f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(254.00002f).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(253.99998f).AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(253.99998f).AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(253.99998f).AsInt16(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { CBORObject.FromObject(253.99998f).AsByte(); } catch(Exception ex){ Assert.fail(ex.toString()); }
    }
    
    /**
     * 
     */
@Test
    public void FromDoubleTest() {
      Assert.assertEquals("0.213299999999999989608312489508534781634807586669921875", DecimalFraction.FromDouble(0.2133).toString());
      Assert.assertEquals("2.29360000000000010330982488752915582352898127282969653606414794921875E-7", DecimalFraction.FromDouble(2.2936E-7).toString());
      Assert.assertEquals("3893200000", DecimalFraction.FromDouble(3.8932E9).toString());
      Assert.assertEquals("128230", DecimalFraction.FromDouble(128230.0).toString());
      Assert.assertEquals("127210", DecimalFraction.FromDouble(127210.0).toString());
      Assert.assertEquals("0.267230000000000023074875343809253536164760589599609375", DecimalFraction.FromDouble(0.26723).toString());
      Assert.assertEquals("0.302329999999999987636556397774256765842437744140625", DecimalFraction.FromDouble(0.30233).toString());
      Assert.assertEquals("0.0000019512000000000000548530838806460252499164198525249958038330078125", DecimalFraction.FromDouble(1.9512E-6).toString());
      Assert.assertEquals("199500", DecimalFraction.FromDouble(199500.0).toString());
      Assert.assertEquals("36214000", DecimalFraction.FromDouble(3.6214E7).toString());
      Assert.assertEquals("1913300000000", DecimalFraction.FromDouble(1.9133E12).toString());
      Assert.assertEquals("0.0002173499999999999976289799530349000633577816188335418701171875", DecimalFraction.FromDouble(2.1735E-4).toString());
      Assert.assertEquals("0.0000310349999999999967797807698399736864303122274577617645263671875", DecimalFraction.FromDouble(3.1035E-5).toString());
      Assert.assertEquals("1.274999999999999911182158029987476766109466552734375", DecimalFraction.FromDouble(1.275).toString());
      Assert.assertEquals("214190", DecimalFraction.FromDouble(214190.0).toString());
      Assert.assertEquals("3981300000", DecimalFraction.FromDouble(3.9813E9).toString());
      Assert.assertEquals("1092700", DecimalFraction.FromDouble(1092700.0).toString());
      Assert.assertEquals("0.023609999999999999042987752773115062154829502105712890625", DecimalFraction.FromDouble(0.02361).toString());
      Assert.assertEquals("12.321999999999999175770426518283784389495849609375", DecimalFraction.FromDouble(12.322).toString());
      Assert.assertEquals("0.002586999999999999889921387108415729016996920108795166015625", DecimalFraction.FromDouble(0.002587).toString());
      Assert.assertEquals("1322000000", DecimalFraction.FromDouble(1.322E9).toString());
      Assert.assertEquals("95310000000", DecimalFraction.FromDouble(9.531E10).toString());
      Assert.assertEquals("142.3799999999999954525264911353588104248046875", DecimalFraction.FromDouble(142.38).toString());
      Assert.assertEquals("2252.5", DecimalFraction.FromDouble(2252.5).toString());
      Assert.assertEquals("363600000000", DecimalFraction.FromDouble(3.636E11).toString());
      Assert.assertEquals("0.00000323700000000000009386523676380154057596882921643555164337158203125", DecimalFraction.FromDouble(3.237E-6).toString());
      Assert.assertEquals("728000", DecimalFraction.FromDouble(728000.0).toString());
      Assert.assertEquals("25818000", DecimalFraction.FromDouble(2.5818E7).toString());
      Assert.assertEquals("1090000", DecimalFraction.FromDouble(1090000.0).toString());
      Assert.assertEquals("1.5509999999999999342747969421907328069210052490234375", DecimalFraction.FromDouble(1.551).toString());
      Assert.assertEquals("26.035000000000000142108547152020037174224853515625", DecimalFraction.FromDouble(26.035).toString());
      Assert.assertEquals("833000000", DecimalFraction.FromDouble(8.33E8).toString());
      Assert.assertEquals("812300000000", DecimalFraction.FromDouble(8.123E11).toString());
      Assert.assertEquals("2622.90000000000009094947017729282379150390625", DecimalFraction.FromDouble(2622.9).toString());
      Assert.assertEquals("1.290999999999999925393012745189480483531951904296875", DecimalFraction.FromDouble(1.291).toString());
      Assert.assertEquals("286140", DecimalFraction.FromDouble(286140.0).toString());
      Assert.assertEquals("0.06733000000000000095923269327613525092601776123046875", DecimalFraction.FromDouble(0.06733).toString());
      Assert.assertEquals("0.000325160000000000010654532811571471029310487210750579833984375", DecimalFraction.FromDouble(3.2516E-4).toString());
      Assert.assertEquals("383230000", DecimalFraction.FromDouble(3.8323E8).toString());
      Assert.assertEquals("0.02843299999999999994049204588009160943329334259033203125", DecimalFraction.FromDouble(0.028433).toString());
      Assert.assertEquals("837000000", DecimalFraction.FromDouble(8.37E8).toString());
      Assert.assertEquals("0.0160800000000000005428990590417015482671558856964111328125", DecimalFraction.FromDouble(0.01608).toString());
      Assert.assertEquals("3621000000000", DecimalFraction.FromDouble(3.621E12).toString());
      Assert.assertEquals("78.1200000000000045474735088646411895751953125", DecimalFraction.FromDouble(78.12).toString());
      Assert.assertEquals("1308000000", DecimalFraction.FromDouble(1.308E9).toString());
      Assert.assertEquals("0.031937000000000000110578213252665591426193714141845703125", DecimalFraction.FromDouble(0.031937).toString());
      Assert.assertEquals("1581500", DecimalFraction.FromDouble(1581500.0).toString());
      Assert.assertEquals("244200", DecimalFraction.FromDouble(244200.0).toString());
      Assert.assertEquals("2.28179999999999995794237200343046456652018605382181704044342041015625E-7", DecimalFraction.FromDouble(2.2818E-7).toString());
      Assert.assertEquals("39.73400000000000176214598468504846096038818359375", DecimalFraction.FromDouble(39.734).toString());
      Assert.assertEquals("1614", DecimalFraction.FromDouble(1614.0).toString());
      Assert.assertEquals("0.0003831899999999999954607143859419693399104289710521697998046875", DecimalFraction.FromDouble(3.8319E-4).toString());
      Assert.assertEquals("543.3999999999999772626324556767940521240234375", DecimalFraction.FromDouble(543.4).toString());
      Assert.assertEquals("319310000", DecimalFraction.FromDouble(3.1931E8).toString());
      Assert.assertEquals("1429000", DecimalFraction.FromDouble(1429000.0).toString());
      Assert.assertEquals("2653700000000", DecimalFraction.FromDouble(2.6537E12).toString());
      Assert.assertEquals("722000000", DecimalFraction.FromDouble(7.22E8).toString());
      Assert.assertEquals("27.199999999999999289457264239899814128875732421875", DecimalFraction.FromDouble(27.2).toString());
      Assert.assertEquals("0.00000380250000000000001586513038998038638283105683512985706329345703125", DecimalFraction.FromDouble(3.8025E-6).toString());
      Assert.assertEquals("0.0000364159999999999982843446044711299691698513925075531005859375", DecimalFraction.FromDouble(3.6416E-5).toString());
      Assert.assertEquals("2006000", DecimalFraction.FromDouble(2006000.0).toString());
      Assert.assertEquals("2681200000", DecimalFraction.FromDouble(2.6812E9).toString());
      Assert.assertEquals("27534000000", DecimalFraction.FromDouble(2.7534E10).toString());
      Assert.assertEquals("3.911600000000000165617541382501176627783934236504137516021728515625E-7", DecimalFraction.FromDouble(3.9116E-7).toString());
      Assert.assertEquals("0.0028135000000000000286437540353290387429296970367431640625", DecimalFraction.FromDouble(0.0028135).toString());
      Assert.assertEquals("0.91190000000000004387601393318618647754192352294921875", DecimalFraction.FromDouble(0.9119).toString());
      Assert.assertEquals("2241200", DecimalFraction.FromDouble(2241200.0).toString());
      Assert.assertEquals("32.4500000000000028421709430404007434844970703125", DecimalFraction.FromDouble(32.45).toString());
      Assert.assertEquals("13800000000", DecimalFraction.FromDouble(1.38E10).toString());
      Assert.assertEquals("0.047300000000000001765254609153998899273574352264404296875", DecimalFraction.FromDouble(0.0473).toString());
      Assert.assertEquals("205.340000000000003410605131648480892181396484375", DecimalFraction.FromDouble(205.34).toString());
      Assert.assertEquals("3.981899999999999995026200849679298698902130126953125", DecimalFraction.FromDouble(3.9819).toString());
      Assert.assertEquals("1152.799999999999954525264911353588104248046875", DecimalFraction.FromDouble(1152.8).toString());
      Assert.assertEquals("1322000", DecimalFraction.FromDouble(1322000.0).toString());
      Assert.assertEquals("0.00013414000000000001334814203612921801322954706847667694091796875", DecimalFraction.FromDouble(1.3414E-4).toString());
      Assert.assertEquals("3.4449999999999999446924077266263264363033158588223159313201904296875E-7", DecimalFraction.FromDouble(3.445E-7).toString());
      Assert.assertEquals("1.3610000000000000771138253079228785935583800892345607280731201171875E-7", DecimalFraction.FromDouble(1.361E-7).toString());
      Assert.assertEquals("26090000", DecimalFraction.FromDouble(2.609E7).toString());
      Assert.assertEquals("9.93599999999999994315658113919198513031005859375", DecimalFraction.FromDouble(9.936).toString());
      Assert.assertEquals("0.00000600000000000000015200514458246772164784488268196582794189453125", DecimalFraction.FromDouble(6.0E-6).toString());
      Assert.assertEquals("260.31000000000000227373675443232059478759765625", DecimalFraction.FromDouble(260.31).toString());
      Assert.assertEquals("344.6000000000000227373675443232059478759765625", DecimalFraction.FromDouble(344.6).toString());
      Assert.assertEquals("3.423700000000000187583282240666449069976806640625", DecimalFraction.FromDouble(3.4237).toString());
      Assert.assertEquals("2342100000", DecimalFraction.FromDouble(2.3421E9).toString());
      Assert.assertEquals("0.00023310000000000000099260877295392901942250318825244903564453125", DecimalFraction.FromDouble(2.331E-4).toString());
      Assert.assertEquals("0.7339999999999999857891452847979962825775146484375", DecimalFraction.FromDouble(0.734).toString());
      Assert.assertEquals("0.01541499999999999988287147090204598498530685901641845703125", DecimalFraction.FromDouble(0.015415).toString());
      Assert.assertEquals("0.0035311000000000001240729741169843691750429570674896240234375", DecimalFraction.FromDouble(0.0035311).toString());
      Assert.assertEquals("1221700000000", DecimalFraction.FromDouble(1.2217E12).toString());
      Assert.assertEquals("0.48299999999999998490096686509787105023860931396484375", DecimalFraction.FromDouble(0.483).toString());
      Assert.assertEquals("0.0002871999999999999878506906636488338335766457021236419677734375", DecimalFraction.FromDouble(2.872E-4).toString());
      Assert.assertEquals("96.1099999999999994315658113919198513031005859375", DecimalFraction.FromDouble(96.11).toString());
      Assert.assertEquals("36570", DecimalFraction.FromDouble(36570.0).toString());
      Assert.assertEquals("0.00001830000000000000097183545932910675446692039258778095245361328125", DecimalFraction.FromDouble(1.83E-5).toString());
      Assert.assertEquals("301310000", DecimalFraction.FromDouble(3.0131E8).toString());
      Assert.assertEquals("382200", DecimalFraction.FromDouble(382200.0).toString());
      Assert.assertEquals("248350000", DecimalFraction.FromDouble(2.4835E8).toString());
      Assert.assertEquals("0.0015839999999999999046040866090834242640994489192962646484375", DecimalFraction.FromDouble(0.001584).toString());
      Assert.assertEquals("0.000761999999999999982035203682784185730270110070705413818359375", DecimalFraction.FromDouble(7.62E-4).toString());
      Assert.assertEquals("313300000000", DecimalFraction.FromDouble(3.133E11).toString());
    }
    /**
     * 
     */
@Test
    public void ToPlainStringTest() {
      Assert.assertEquals("277220000000", DecimalFraction.FromString("277.22E9").ToPlainString());
      Assert.assertEquals("3911900", DecimalFraction.FromString("391.19E4").ToPlainString());
      Assert.assertEquals("0.00000038327", DecimalFraction.FromString("383.27E-9").ToPlainString());
      Assert.assertEquals("47330000000", DecimalFraction.FromString("47.33E9").ToPlainString());
      Assert.assertEquals("322210", DecimalFraction.FromString("322.21E3").ToPlainString());
      Assert.assertEquals("1.913", DecimalFraction.FromString("191.3E-2").ToPlainString());
      Assert.assertEquals("11917", DecimalFraction.FromString("119.17E2").ToPlainString());
      Assert.assertEquals("0.0001596", DecimalFraction.FromString("159.6E-6").ToPlainString());
      Assert.assertEquals("70160000000", DecimalFraction.FromString("70.16E9").ToPlainString());
      Assert.assertEquals("166240000000", DecimalFraction.FromString("166.24E9").ToPlainString());
      Assert.assertEquals("235250", DecimalFraction.FromString("235.25E3").ToPlainString());
      Assert.assertEquals("372200000", DecimalFraction.FromString("37.22E7").ToPlainString());
      Assert.assertEquals("32026000000", DecimalFraction.FromString("320.26E8").ToPlainString());
      Assert.assertEquals("0.00000012711", DecimalFraction.FromString("127.11E-9").ToPlainString());
      Assert.assertEquals("0.000009729", DecimalFraction.FromString("97.29E-7").ToPlainString());
      Assert.assertEquals("175130000000", DecimalFraction.FromString("175.13E9").ToPlainString());
      Assert.assertEquals("0.000003821", DecimalFraction.FromString("38.21E-7").ToPlainString());
      Assert.assertEquals("62.8", DecimalFraction.FromString("6.28E1").ToPlainString());
      Assert.assertEquals("138290000", DecimalFraction.FromString("138.29E6").ToPlainString());
      Assert.assertEquals("1601.9", DecimalFraction.FromString("160.19E1").ToPlainString());
      Assert.assertEquals("35812", DecimalFraction.FromString("358.12E2").ToPlainString());
      Assert.assertEquals("2492800000000", DecimalFraction.FromString("249.28E10").ToPlainString());
      Assert.assertEquals("0.00031123", DecimalFraction.FromString("311.23E-6").ToPlainString());
      Assert.assertEquals("0.16433", DecimalFraction.FromString("164.33E-3").ToPlainString());
      Assert.assertEquals("29.920", DecimalFraction.FromString("299.20E-1").ToPlainString());
      Assert.assertEquals("105390", DecimalFraction.FromString("105.39E3").ToPlainString());
      Assert.assertEquals("3825000", DecimalFraction.FromString("382.5E4").ToPlainString());
      Assert.assertEquals("909", DecimalFraction.FromString("90.9E1").ToPlainString());
      Assert.assertEquals("32915000000", DecimalFraction.FromString("329.15E8").ToPlainString());
      Assert.assertEquals("24523000000", DecimalFraction.FromString("245.23E8").ToPlainString());
      Assert.assertEquals("0.0000009719", DecimalFraction.FromString("97.19E-8").ToPlainString());
      Assert.assertEquals("551200000", DecimalFraction.FromString("55.12E7").ToPlainString());
      Assert.assertEquals("1238", DecimalFraction.FromString("12.38E2").ToPlainString());
      Assert.assertEquals("0.0025020", DecimalFraction.FromString("250.20E-5").ToPlainString());
      Assert.assertEquals("5320", DecimalFraction.FromString("53.20E2").ToPlainString());
      Assert.assertEquals("14150000000", DecimalFraction.FromString("141.5E8").ToPlainString());
      Assert.assertEquals("0.0033834", DecimalFraction.FromString("338.34E-5").ToPlainString());
      Assert.assertEquals("160390000000", DecimalFraction.FromString("160.39E9").ToPlainString());
      Assert.assertEquals("152170000", DecimalFraction.FromString("152.17E6").ToPlainString());
      Assert.assertEquals("13300000000", DecimalFraction.FromString("13.3E9").ToPlainString());
      Assert.assertEquals("13.8", DecimalFraction.FromString("1.38E1").ToPlainString());
      Assert.assertEquals("0.00000034821", DecimalFraction.FromString("348.21E-9").ToPlainString());
      Assert.assertEquals("525000000", DecimalFraction.FromString("52.5E7").ToPlainString());
      Assert.assertEquals("2152100000000", DecimalFraction.FromString("215.21E10").ToPlainString());
      Assert.assertEquals("234280000000", DecimalFraction.FromString("234.28E9").ToPlainString());
      Assert.assertEquals("310240000000", DecimalFraction.FromString("310.24E9").ToPlainString());
      Assert.assertEquals("345390000000", DecimalFraction.FromString("345.39E9").ToPlainString());
      Assert.assertEquals("0.00000011638", DecimalFraction.FromString("116.38E-9").ToPlainString());
      Assert.assertEquals("2762500000000", DecimalFraction.FromString("276.25E10").ToPlainString());
      Assert.assertEquals("0.0000015832", DecimalFraction.FromString("158.32E-8").ToPlainString());
      Assert.assertEquals("27250", DecimalFraction.FromString("272.5E2").ToPlainString());
      Assert.assertEquals("0.00000038933", DecimalFraction.FromString("389.33E-9").ToPlainString());
      Assert.assertEquals("3811500000", DecimalFraction.FromString("381.15E7").ToPlainString());
      Assert.assertEquals("280000", DecimalFraction.FromString("280.0E3").ToPlainString());
      Assert.assertEquals("0.0002742", DecimalFraction.FromString("274.2E-6").ToPlainString());
      Assert.assertEquals("0.000038714", DecimalFraction.FromString("387.14E-7").ToPlainString());
      Assert.assertEquals("0.00002277", DecimalFraction.FromString("227.7E-7").ToPlainString());
      Assert.assertEquals("20121", DecimalFraction.FromString("201.21E2").ToPlainString());
      Assert.assertEquals("255400", DecimalFraction.FromString("255.4E3").ToPlainString());
      Assert.assertEquals("0.000018727", DecimalFraction.FromString("187.27E-7").ToPlainString());
      Assert.assertEquals("0.01697", DecimalFraction.FromString("169.7E-4").ToPlainString());
      Assert.assertEquals("69900000000", DecimalFraction.FromString("69.9E9").ToPlainString());
      Assert.assertEquals("0.0320", DecimalFraction.FromString("3.20E-2").ToPlainString());
      Assert.assertEquals("23630", DecimalFraction.FromString("236.30E2").ToPlainString());
      Assert.assertEquals("0.00000022022", DecimalFraction.FromString("220.22E-9").ToPlainString());
      Assert.assertEquals("28.730", DecimalFraction.FromString("287.30E-1").ToPlainString());
      Assert.assertEquals("0.0000001563", DecimalFraction.FromString("156.3E-9").ToPlainString());
      Assert.assertEquals("13.623", DecimalFraction.FromString("136.23E-1").ToPlainString());
      Assert.assertEquals("12527000000", DecimalFraction.FromString("125.27E8").ToPlainString());
      Assert.assertEquals("0.000018030", DecimalFraction.FromString("180.30E-7").ToPlainString());
      Assert.assertEquals("3515000000", DecimalFraction.FromString("351.5E7").ToPlainString());
      Assert.assertEquals("28280000000", DecimalFraction.FromString("28.28E9").ToPlainString());
      Assert.assertEquals("0.2884", DecimalFraction.FromString("288.4E-3").ToPlainString());
      Assert.assertEquals("122200", DecimalFraction.FromString("12.22E4").ToPlainString());
      Assert.assertEquals("0.002575", DecimalFraction.FromString("257.5E-5").ToPlainString());
      Assert.assertEquals("389200", DecimalFraction.FromString("389.20E3").ToPlainString());
      Assert.assertEquals("0.03949", DecimalFraction.FromString("394.9E-4").ToPlainString());
      Assert.assertEquals("0.000013426", DecimalFraction.FromString("134.26E-7").ToPlainString());
      Assert.assertEquals("5829000", DecimalFraction.FromString("58.29E5").ToPlainString());
      Assert.assertEquals("0.000885", DecimalFraction.FromString("88.5E-5").ToPlainString());
      Assert.assertEquals("0.019329", DecimalFraction.FromString("193.29E-4").ToPlainString());
      Assert.assertEquals("713500000000", DecimalFraction.FromString("71.35E10").ToPlainString());
      Assert.assertEquals("2520", DecimalFraction.FromString("252.0E1").ToPlainString());
      Assert.assertEquals("0.000000532", DecimalFraction.FromString("53.2E-8").ToPlainString());
      Assert.assertEquals("18.120", DecimalFraction.FromString("181.20E-1").ToPlainString());
      Assert.assertEquals("0.00000005521", DecimalFraction.FromString("55.21E-9").ToPlainString());
      Assert.assertEquals("57.31", DecimalFraction.FromString("57.31E0").ToPlainString());
      Assert.assertEquals("0.00000011313", DecimalFraction.FromString("113.13E-9").ToPlainString());
      Assert.assertEquals("532.3", DecimalFraction.FromString("53.23E1").ToPlainString());
      Assert.assertEquals("0.000036837", DecimalFraction.FromString("368.37E-7").ToPlainString());
      Assert.assertEquals("0.01874", DecimalFraction.FromString("187.4E-4").ToPlainString());
      Assert.assertEquals("526000000", DecimalFraction.FromString("5.26E8").ToPlainString());
      Assert.assertEquals("3083200", DecimalFraction.FromString("308.32E4").ToPlainString());
      Assert.assertEquals("0.7615", DecimalFraction.FromString("76.15E-2").ToPlainString());
      Assert.assertEquals("1173800000", DecimalFraction.FromString("117.38E7").ToPlainString());
      Assert.assertEquals("0.001537", DecimalFraction.FromString("15.37E-4").ToPlainString());
      Assert.assertEquals("145.3", DecimalFraction.FromString("145.3E0").ToPlainString());
      Assert.assertEquals("22629000000", DecimalFraction.FromString("226.29E8").ToPlainString());
      Assert.assertEquals("2242600000000", DecimalFraction.FromString("224.26E10").ToPlainString());
      Assert.assertEquals("0.00000026818", DecimalFraction.FromString("268.18E-9").ToPlainString());
    }
    /**
     * 
     */
@Test
    public void ToEngineeringStringTest() {
      Assert.assertEquals("8.912", DecimalFraction.FromString("89.12E-1").ToEngineeringString());
      Assert.assertEquals("0.024231", DecimalFraction.FromString("242.31E-4").ToEngineeringString());
      Assert.assertEquals("22.918E+6", DecimalFraction.FromString("229.18E5").ToEngineeringString());
      Assert.assertEquals("0.000032618", DecimalFraction.FromString("326.18E-7").ToEngineeringString());
      Assert.assertEquals("55.0E+6", DecimalFraction.FromString("55.0E6").ToEngineeringString());
      Assert.assertEquals("224.36E+3", DecimalFraction.FromString("224.36E3").ToEngineeringString());
      Assert.assertEquals("230.12E+9", DecimalFraction.FromString("230.12E9").ToEngineeringString());
      Assert.assertEquals("0.000011320", DecimalFraction.FromString("113.20E-7").ToEngineeringString());
      Assert.assertEquals("317.7E-9", DecimalFraction.FromString("317.7E-9").ToEngineeringString());
      Assert.assertEquals("3.393", DecimalFraction.FromString("339.3E-2").ToEngineeringString());
      Assert.assertEquals("27.135E+9", DecimalFraction.FromString("271.35E8").ToEngineeringString());
      Assert.assertEquals("377.19E-9", DecimalFraction.FromString("377.19E-9").ToEngineeringString());
      Assert.assertEquals("3.2127E+9", DecimalFraction.FromString("321.27E7").ToEngineeringString());
      Assert.assertEquals("2.9422", DecimalFraction.FromString("294.22E-2").ToEngineeringString());
      Assert.assertEquals("0.0000011031", DecimalFraction.FromString("110.31E-8").ToEngineeringString());
      Assert.assertEquals("2.4324", DecimalFraction.FromString("243.24E-2").ToEngineeringString());
      Assert.assertEquals("0.0006412", DecimalFraction.FromString("64.12E-5").ToEngineeringString());
      Assert.assertEquals("1422.3", DecimalFraction.FromString("142.23E1").ToEngineeringString());
      Assert.assertEquals("293.0", DecimalFraction.FromString("293.0E0").ToEngineeringString());
      Assert.assertEquals("0.0000025320", DecimalFraction.FromString("253.20E-8").ToEngineeringString());
      Assert.assertEquals("36.66E+9", DecimalFraction.FromString("366.6E8").ToEngineeringString());
      Assert.assertEquals("3.4526E+12", DecimalFraction.FromString("345.26E10").ToEngineeringString());
      Assert.assertEquals("2.704", DecimalFraction.FromString("270.4E-2").ToEngineeringString());
      Assert.assertEquals("432E+6", DecimalFraction.FromString("4.32E8").ToEngineeringString());
      Assert.assertEquals("224.22", DecimalFraction.FromString("224.22E0").ToEngineeringString());
      Assert.assertEquals("0.000031530", DecimalFraction.FromString("315.30E-7").ToEngineeringString());
      Assert.assertEquals("11.532E+6", DecimalFraction.FromString("115.32E5").ToEngineeringString());
      Assert.assertEquals("39420", DecimalFraction.FromString("394.20E2").ToEngineeringString());
      Assert.assertEquals("67.24E-9", DecimalFraction.FromString("67.24E-9").ToEngineeringString());
      Assert.assertEquals("34933", DecimalFraction.FromString("349.33E2").ToEngineeringString());
      Assert.assertEquals("67.8E-9", DecimalFraction.FromString("67.8E-9").ToEngineeringString());
      Assert.assertEquals("19.231E+6", DecimalFraction.FromString("192.31E5").ToEngineeringString());
      Assert.assertEquals("1.7317E+9", DecimalFraction.FromString("173.17E7").ToEngineeringString());
      Assert.assertEquals("43.9", DecimalFraction.FromString("43.9E0").ToEngineeringString());
      Assert.assertEquals("0.0000016812", DecimalFraction.FromString("168.12E-8").ToEngineeringString());
      Assert.assertEquals("3.715E+12", DecimalFraction.FromString("371.5E10").ToEngineeringString());
      Assert.assertEquals("424E-9", DecimalFraction.FromString("42.4E-8").ToEngineeringString());
      Assert.assertEquals("1.6123E+12", DecimalFraction.FromString("161.23E10").ToEngineeringString());
      Assert.assertEquals("302.8E+6", DecimalFraction.FromString("302.8E6").ToEngineeringString());
      Assert.assertEquals("175.13", DecimalFraction.FromString("175.13E0").ToEngineeringString());
      Assert.assertEquals("298.20E-9", DecimalFraction.FromString("298.20E-9").ToEngineeringString());
      Assert.assertEquals("36.223E+9", DecimalFraction.FromString("362.23E8").ToEngineeringString());
      Assert.assertEquals("27739", DecimalFraction.FromString("277.39E2").ToEngineeringString());
      Assert.assertEquals("0.011734", DecimalFraction.FromString("117.34E-4").ToEngineeringString());
      Assert.assertEquals("190.13E-9", DecimalFraction.FromString("190.13E-9").ToEngineeringString());
      Assert.assertEquals("3.5019", DecimalFraction.FromString("350.19E-2").ToEngineeringString());
      Assert.assertEquals("383.27E-9", DecimalFraction.FromString("383.27E-9").ToEngineeringString());
      Assert.assertEquals("24.217E+6", DecimalFraction.FromString("242.17E5").ToEngineeringString());
      Assert.assertEquals("2.9923E+9", DecimalFraction.FromString("299.23E7").ToEngineeringString());
      Assert.assertEquals("3.0222", DecimalFraction.FromString("302.22E-2").ToEngineeringString());
      Assert.assertEquals("0.04521", DecimalFraction.FromString("45.21E-3").ToEngineeringString());
      Assert.assertEquals("15.00", DecimalFraction.FromString("150.0E-1").ToEngineeringString());
      Assert.assertEquals("290E+3", DecimalFraction.FromString("29.0E4").ToEngineeringString());
      Assert.assertEquals("263.37E+3", DecimalFraction.FromString("263.37E3").ToEngineeringString());
      Assert.assertEquals("28.321", DecimalFraction.FromString("283.21E-1").ToEngineeringString());
      Assert.assertEquals("21.32", DecimalFraction.FromString("21.32E0").ToEngineeringString());
      Assert.assertEquals("0.00006920", DecimalFraction.FromString("69.20E-6").ToEngineeringString());
      Assert.assertEquals("0.0728", DecimalFraction.FromString("72.8E-3").ToEngineeringString());
      Assert.assertEquals("1.646E+9", DecimalFraction.FromString("164.6E7").ToEngineeringString());
      Assert.assertEquals("1.1817", DecimalFraction.FromString("118.17E-2").ToEngineeringString());
      Assert.assertEquals("0.000026235", DecimalFraction.FromString("262.35E-7").ToEngineeringString());
      Assert.assertEquals("23.37E+6", DecimalFraction.FromString("233.7E5").ToEngineeringString());
      Assert.assertEquals("391.24", DecimalFraction.FromString("391.24E0").ToEngineeringString());
      Assert.assertEquals("2213.6", DecimalFraction.FromString("221.36E1").ToEngineeringString());
      Assert.assertEquals("353.32", DecimalFraction.FromString("353.32E0").ToEngineeringString());
      Assert.assertEquals("0.012931", DecimalFraction.FromString("129.31E-4").ToEngineeringString());
      Assert.assertEquals("0.0017626", DecimalFraction.FromString("176.26E-5").ToEngineeringString());
      Assert.assertEquals("207.5E+3", DecimalFraction.FromString("207.5E3").ToEngineeringString());
      Assert.assertEquals("314.10", DecimalFraction.FromString("314.10E0").ToEngineeringString());
      Assert.assertEquals("379.20E+9", DecimalFraction.FromString("379.20E9").ToEngineeringString());
      Assert.assertEquals("0.00037912", DecimalFraction.FromString("379.12E-6").ToEngineeringString());
      Assert.assertEquals("743.8E-9", DecimalFraction.FromString("74.38E-8").ToEngineeringString());
      Assert.assertEquals("234.17E-9", DecimalFraction.FromString("234.17E-9").ToEngineeringString());
      Assert.assertEquals("132.6E+6", DecimalFraction.FromString("13.26E7").ToEngineeringString());
      Assert.assertEquals("25.15E+6", DecimalFraction.FromString("251.5E5").ToEngineeringString());
      Assert.assertEquals("87.32", DecimalFraction.FromString("87.32E0").ToEngineeringString());
      Assert.assertEquals("3.3116E+9", DecimalFraction.FromString("331.16E7").ToEngineeringString());
      Assert.assertEquals("6.14E+9", DecimalFraction.FromString("61.4E8").ToEngineeringString());
      Assert.assertEquals("0.0002097", DecimalFraction.FromString("209.7E-6").ToEngineeringString());
      Assert.assertEquals("5.4E+6", DecimalFraction.FromString("5.4E6").ToEngineeringString());
      Assert.assertEquals("219.9", DecimalFraction.FromString("219.9E0").ToEngineeringString());
      Assert.assertEquals("0.00002631", DecimalFraction.FromString("26.31E-6").ToEngineeringString());
      Assert.assertEquals("482.8E+6", DecimalFraction.FromString("48.28E7").ToEngineeringString());
      Assert.assertEquals("267.8", DecimalFraction.FromString("267.8E0").ToEngineeringString());
      Assert.assertEquals("0.3209", DecimalFraction.FromString("320.9E-3").ToEngineeringString());
      Assert.assertEquals("0.30015", DecimalFraction.FromString("300.15E-3").ToEngineeringString());
      Assert.assertEquals("2.6011E+6", DecimalFraction.FromString("260.11E4").ToEngineeringString());
      Assert.assertEquals("1.1429", DecimalFraction.FromString("114.29E-2").ToEngineeringString());
      Assert.assertEquals("0.0003060", DecimalFraction.FromString("306.0E-6").ToEngineeringString());
      Assert.assertEquals("97.7E+3", DecimalFraction.FromString("97.7E3").ToEngineeringString());
      Assert.assertEquals("12.229E+9", DecimalFraction.FromString("122.29E8").ToEngineeringString());
      Assert.assertEquals("6.94E+3", DecimalFraction.FromString("69.4E2").ToEngineeringString());
      Assert.assertEquals("383.5", DecimalFraction.FromString("383.5E0").ToEngineeringString());
      Assert.assertEquals("315.30E+3", DecimalFraction.FromString("315.30E3").ToEngineeringString());
      Assert.assertEquals("130.38E+9", DecimalFraction.FromString("130.38E9").ToEngineeringString());
      Assert.assertEquals("206.16E+9", DecimalFraction.FromString("206.16E9").ToEngineeringString());
      Assert.assertEquals("304.28E-9", DecimalFraction.FromString("304.28E-9").ToEngineeringString());
      Assert.assertEquals("661.3E+3", DecimalFraction.FromString("66.13E4").ToEngineeringString());
      Assert.assertEquals("1.8533", DecimalFraction.FromString("185.33E-2").ToEngineeringString());
      Assert.assertEquals("70.7E+6", DecimalFraction.FromString("70.7E6").ToEngineeringString());
    }
    /**
     * 
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
     * 
     */
@Test
    public void TestSubtractNonFinite() {
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NaN), CBORObject.FromObject(99.74439f)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NaN), CBORObject.FromObject(0.04503661680757691d)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NaN), CBORObject.FromObject(DecimalFraction.FromString("961.056025725133"))).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NaN), CBORObject.FromObject(-2.66673f)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NaN), CBORObject.FromObject(-3249200021658530613L)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.POSITIVE_INFINITY), CBORObject.FromObject(-3082676751896642153L)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.POSITIVE_INFINITY), CBORObject.FromObject(0.37447542485458996d)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.POSITIVE_INFINITY), CBORObject.FromObject(DecimalFraction.FromString("6695270"))).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.POSITIVE_INFINITY), CBORObject.FromObject(8.645616f)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.POSITIVE_INFINITY), CBORObject.FromObject(10.918599534632621d)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NEGATIVE_INFINITY), CBORObject.FromObject(1.1195766122143437E-7d)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NEGATIVE_INFINITY), CBORObject.FromObject(-27.678854f)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NEGATIVE_INFINITY), CBORObject.FromObject(DecimalFraction.FromString("51444344646435.890"))).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NEGATIVE_INFINITY), CBORObject.FromObject(DecimalFraction.FromString("-795755897.41124405443"))).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Double.NEGATIVE_INFINITY), CBORObject.FromObject(DecimalFraction.FromString("282349190160173.8945458982215192141"))).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Float.NaN), CBORObject.FromObject(-4742894673080640195L)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Float.NaN), CBORObject.FromObject(-8.057984695058738E-10d)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Float.NaN), CBORObject.FromObject(-6832707275063219586L)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Float.NaN), CBORObject.FromObject(new BigInteger("3037587108614072"))).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Float.NaN), CBORObject.FromObject(DecimalFraction.FromString("-21687"))).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Float.POSITIVE_INFINITY), CBORObject.FromObject(21.02954f)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Float.POSITIVE_INFINITY), CBORObject.FromObject(-280.74258f)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Float.POSITIVE_INFINITY), CBORObject.FromObject(3.295564645540288E-15d)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Float.POSITIVE_INFINITY), CBORObject.FromObject(-1.8643148756498468E-14d)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Float.POSITIVE_INFINITY), CBORObject.FromObject(DecimalFraction.FromString("56E-9"))).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Float.NEGATIVE_INFINITY), CBORObject.FromObject(new BigInteger("06842884252556766213171069781"))).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Float.NEGATIVE_INFINITY), CBORObject.FromObject(-6381263349646471084L)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Float.NEGATIVE_INFINITY), CBORObject.FromObject(9127378784365184230L)).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Float.NEGATIVE_INFINITY), CBORObject.FromObject(new BigInteger("300921783316"))).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { CBORObject.Subtract(CBORObject.FromObject(Float.NEGATIVE_INFINITY), CBORObject.FromObject(new BigInteger("-5806763724610384900094490266237212718"))).AsDecimalFraction(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
    }
    /**
     * 
     */
@Test
    public void TestAsByte() {
      for(int i=0;i<255;i++){
        Assert.assertEquals((byte)i,CBORObject.FromObject(i).AsByte());
      }
      for(int i=-200;i<0;i++){
        try { CBORObject.FromObject(i).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      }
      for(int i=256;i<512;i++){
        try { CBORObject.FromObject(i).AsByte(); } catch(ArithmeticException ex){ } catch(Exception ex){ Assert.fail(ex.toString()); }
      }
    }
    @Test(expected=CBORException.class)
    public void TestByteStringStreamNoIndefiniteWithinDefinite() {
      TestCommon.FromBytesTestAB(
        new byte[]{ 0x5F, 0x41, 0x20, 0x5F, 0x41, 0x20, (byte)0xFF, (byte)0xFF });
    }
    /**
     * 
     */
@Test
    public void TestBigFloatDecFrac() {
      BigFloat bf;
      bf = new BigFloat(20);
      Assert.assertEquals("20", DecimalFraction.FromBigFloat(bf).toString());
      bf = new BigFloat(BigInteger.valueOf(3), -1);
      Assert.assertEquals("1.5", DecimalFraction.FromBigFloat(bf).toString());
      bf = new BigFloat(BigInteger.valueOf(-3), -1);
      Assert.assertEquals("-1.5", DecimalFraction.FromBigFloat(bf).toString());
      DecimalFraction df;
      df = new DecimalFraction(20);
      Assert.assertEquals("20", BigFloat.FromDecimalFraction(df).toString());
      df = new DecimalFraction(-20);
      Assert.assertEquals("-20", BigFloat.FromDecimalFraction(df).toString());
      df = new DecimalFraction(BigInteger.valueOf(15), -1);
      Assert.assertEquals("1.5", BigFloat.FromDecimalFraction(df).toString());
      df = new DecimalFraction(BigInteger.valueOf(-15), -1);
      Assert.assertEquals("-1.5", BigFloat.FromDecimalFraction(df).toString());
    }
    /*
    @Test
    public void TestMutableBigInt() {
      FastRandom r=new FastRandom();
      for(int i=0;i<1000;i++){
        MutableBigInteger mbi=new MutableBigInteger();
        BigInteger control=BigInteger.ZERO;
        for(int j=0;j<100;j++){
          if(j==0 || r.NextValue(2)==0){
            int x=r.NextValue(25);
            control+=x;
            mbi.Add(x);
            Assert.assertEquals(control,mbi.ToBigInteger());
          } else {
            int x=r.NextValue(25)+1;
            control=control.multiply(x);
            mbi.Multiply(x);
            Assert.assertEquals(control,mbi.ToBigInteger());
          }
        }
      }
    }
    */
    @Test
    public void TestDecFracToSingleDoubleHighExponents() {
      if(914323.0f!=DecimalFraction.FromString("914323").ToSingle())
        Assert.fail("decfrac single 914323\nExpected: 914323.0f\nWas: "+DecimalFraction.FromString("914323").ToSingle());
      if(914323.0d!=DecimalFraction.FromString("914323").ToDouble())
        Assert.fail("decfrac double 914323\nExpected: 914323.0d\nWas: "+DecimalFraction.FromString("914323").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-57318007483673759194E+106").ToSingle())
        Assert.fail("decfrac single -57318007483673759194E+106\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-57318007483673759194E+106").ToSingle());
      if(-5.731800748367376E125d!=DecimalFraction.FromString("-57318007483673759194E+106").ToDouble())
        Assert.fail("decfrac double -57318007483673759194E+106\nExpected: -5.731800748367376E125d\nWas: "+DecimalFraction.FromString("-57318007483673759194E+106").ToDouble());
      if(0.0f!=DecimalFraction.FromString("420685230629E-264").ToSingle())
        Assert.fail("decfrac single 420685230629E-264\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("420685230629E-264").ToSingle());
      if(4.20685230629E-253d!=DecimalFraction.FromString("420685230629E-264").ToDouble())
        Assert.fail("decfrac double 420685230629E-264\nExpected: 4.20685230629E-253d\nWas: "+DecimalFraction.FromString("420685230629E-264").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("1089152800893419E+168").ToSingle())
        Assert.fail("decfrac single 1089152800893419E+168\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("1089152800893419E+168").ToSingle());
      if(1.089152800893419E183d!=DecimalFraction.FromString("1089152800893419E+168").ToDouble())
        Assert.fail("decfrac double 1089152800893419E+168\nExpected: 1.089152800893419E183d\nWas: "+DecimalFraction.FromString("1089152800893419E+168").ToDouble());
      if(1.5936804E7f!=DecimalFraction.FromString("15936804").ToSingle())
        Assert.fail("decfrac single 15936804\nExpected: 1.5936804E7f\nWas: "+DecimalFraction.FromString("15936804").ToSingle());
      if(1.5936804E7d!=DecimalFraction.FromString("15936804").ToDouble())
        Assert.fail("decfrac double 15936804\nExpected: 1.5936804E7d\nWas: "+DecimalFraction.FromString("15936804").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-24681.2332E+61").ToSingle())
        Assert.fail("decfrac single -24681.2332E+61\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-24681.2332E+61").ToSingle());
      if(-2.46812332E65d!=DecimalFraction.FromString("-24681.2332E+61").ToDouble())
        Assert.fail("decfrac double -24681.2332E+61\nExpected: -2.46812332E65d\nWas: "+DecimalFraction.FromString("-24681.2332E+61").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-417509591569.6827833177512321E-93").ToSingle())
        Assert.fail("decfrac single -417509591569.6827833177512321E-93\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-417509591569.6827833177512321E-93").ToSingle());
      if(-4.175095915696828E-82d!=DecimalFraction.FromString("-417509591569.6827833177512321E-93").ToDouble())
        Assert.fail("decfrac double -417509591569.6827833177512321E-93\nExpected: -4.175095915696828E-82d\nWas: "+DecimalFraction.FromString("-417509591569.6827833177512321E-93").ToDouble());
      if(5.38988331E17f!=DecimalFraction.FromString("538988338119784732").ToSingle())
        Assert.fail("decfrac single 538988338119784732\nExpected: 5.38988331E17f\nWas: "+DecimalFraction.FromString("538988338119784732").ToSingle());
      if(5.389883381197847E17d!=DecimalFraction.FromString("538988338119784732").ToDouble())
        Assert.fail("decfrac double 538988338119784732\nExpected: 5.389883381197847E17d\nWas: "+DecimalFraction.FromString("538988338119784732").ToDouble());
      if(260.14423f!=DecimalFraction.FromString("260.1442248").ToSingle())
        Assert.fail("decfrac single 260.1442248\nExpected: 260.14423f\nWas: "+DecimalFraction.FromString("260.1442248").ToSingle());
      if(260.1442248d!=DecimalFraction.FromString("260.1442248").ToDouble())
        Assert.fail("decfrac double 260.1442248\nExpected: 260.1442248d\nWas: "+DecimalFraction.FromString("260.1442248").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-8457715957008143770.130850853640402959E-181").ToSingle())
        Assert.fail("decfrac single -8457715957008143770.130850853640402959E-181\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-8457715957008143770.130850853640402959E-181").ToSingle());
      if(-8.457715957008144E-163d!=DecimalFraction.FromString("-8457715957008143770.130850853640402959E-181").ToDouble())
        Assert.fail("decfrac double -8457715957008143770.130850853640402959E-181\nExpected: -8.457715957008144E-163d\nWas: "+DecimalFraction.FromString("-8457715957008143770.130850853640402959E-181").ToDouble());
      if(0.0f!=DecimalFraction.FromString("22.7178448747E-225").ToSingle())
        Assert.fail("decfrac single 22.7178448747E-225\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("22.7178448747E-225").ToSingle());
      if(2.27178448747E-224d!=DecimalFraction.FromString("22.7178448747E-225").ToDouble())
        Assert.fail("decfrac double 22.7178448747E-225\nExpected: 2.27178448747E-224d\nWas: "+DecimalFraction.FromString("22.7178448747E-225").ToDouble());
      if(-790581.44f!=DecimalFraction.FromString("-790581.4576317018014").ToSingle())
        Assert.fail("decfrac single -790581.4576317018014\nExpected: -790581.44f\nWas: "+DecimalFraction.FromString("-790581.4576317018014").ToSingle());
      if(-790581.4576317018d!=DecimalFraction.FromString("-790581.4576317018014").ToDouble())
        Assert.fail("decfrac double -790581.4576317018014\nExpected: -790581.4576317018d\nWas: "+DecimalFraction.FromString("-790581.4576317018014").ToDouble());
      if(-1.80151695E16f!=DecimalFraction.FromString("-18015168704168440").ToSingle())
        Assert.fail("decfrac single -18015168704168440\nExpected: -1.80151695E16f\nWas: "+DecimalFraction.FromString("-18015168704168440").ToSingle());
      if(-1.801516870416844E16d!=DecimalFraction.FromString("-18015168704168440").ToDouble())
        Assert.fail("decfrac double -18015168704168440\nExpected: -1.801516870416844E16d\nWas: "+DecimalFraction.FromString("-18015168704168440").ToDouble());
      if(-36.0f!=DecimalFraction.FromString("-36").ToSingle())
        Assert.fail("decfrac single -36\nExpected: -36.0f\nWas: "+DecimalFraction.FromString("-36").ToSingle());
      if(-36.0d!=DecimalFraction.FromString("-36").ToDouble())
        Assert.fail("decfrac double -36\nExpected: -36.0d\nWas: "+DecimalFraction.FromString("-36").ToDouble());
      if(0.0f!=DecimalFraction.FromString("653060307988076E-230").ToSingle())
        Assert.fail("decfrac single 653060307988076E-230\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("653060307988076E-230").ToSingle());
      if(6.53060307988076E-216d!=DecimalFraction.FromString("653060307988076E-230").ToDouble())
        Assert.fail("decfrac double 653060307988076E-230\nExpected: 6.53060307988076E-216d\nWas: "+DecimalFraction.FromString("653060307988076E-230").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-4446345.5911E+316").ToSingle())
        Assert.fail("decfrac single -4446345.5911E+316\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-4446345.5911E+316").ToSingle());
      if(Double.NEGATIVE_INFINITY!=DecimalFraction.FromString("-4446345.5911E+316").ToDouble())
        Assert.fail("decfrac double -4446345.5911E+316\nExpected: Double.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-4446345.5911E+316").ToDouble());
      if(-5.3940226E15f!=DecimalFraction.FromString("-5394022706804125.84338479899885").ToSingle())
        Assert.fail("decfrac single -5394022706804125.84338479899885\nExpected: -5.3940226E15f\nWas: "+DecimalFraction.FromString("-5394022706804125.84338479899885").ToSingle());
      if(-5.394022706804126E15d!=DecimalFraction.FromString("-5394022706804125.84338479899885").ToDouble())
        Assert.fail("decfrac double -5394022706804125.84338479899885\nExpected: -5.394022706804126E15d\nWas: "+DecimalFraction.FromString("-5394022706804125.84338479899885").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("310504020304E+181").ToSingle())
        Assert.fail("decfrac single 310504020304E+181\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("310504020304E+181").ToSingle());
      if(3.10504020304E192d!=DecimalFraction.FromString("310504020304E+181").ToDouble())
        Assert.fail("decfrac double 310504020304E+181\nExpected: 3.10504020304E192d\nWas: "+DecimalFraction.FromString("310504020304E+181").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-164609450222646.21988340572652533E+317").ToSingle())
        Assert.fail("decfrac single -164609450222646.21988340572652533E+317\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-164609450222646.21988340572652533E+317").ToSingle());
      if(Double.NEGATIVE_INFINITY!=DecimalFraction.FromString("-164609450222646.21988340572652533E+317").ToDouble())
        Assert.fail("decfrac double -164609450222646.21988340572652533E+317\nExpected: Double.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-164609450222646.21988340572652533E+317").ToDouble());
      if(7.1524661E18f!=DecimalFraction.FromString("7152466127871812565.075310").ToSingle())
        Assert.fail("decfrac single 7152466127871812565.075310\nExpected: 7.1524661E18f\nWas: "+DecimalFraction.FromString("7152466127871812565.075310").ToSingle());
      if(7.1524661278718126E18d!=DecimalFraction.FromString("7152466127871812565.075310").ToDouble())
        Assert.fail("decfrac double 7152466127871812565.075310\nExpected: 7.1524661278718126E18d\nWas: "+DecimalFraction.FromString("7152466127871812565.075310").ToDouble());
      if(925.0f!=DecimalFraction.FromString("925").ToSingle())
        Assert.fail("decfrac single 925\nExpected: 925.0f\nWas: "+DecimalFraction.FromString("925").ToSingle());
      if(925.0d!=DecimalFraction.FromString("925").ToDouble())
        Assert.fail("decfrac double 925\nExpected: 925.0d\nWas: "+DecimalFraction.FromString("925").ToDouble());
      if(34794.0f!=DecimalFraction.FromString("34794").ToSingle())
        Assert.fail("decfrac single 34794\nExpected: 34794.0f\nWas: "+DecimalFraction.FromString("34794").ToSingle());
      if(34794.0d!=DecimalFraction.FromString("34794").ToDouble())
        Assert.fail("decfrac double 34794\nExpected: 34794.0d\nWas: "+DecimalFraction.FromString("34794").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-337655705333269E-276").ToSingle())
        Assert.fail("decfrac single -337655705333269E-276\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-337655705333269E-276").ToSingle());
      if(-3.37655705333269E-262d!=DecimalFraction.FromString("-337655705333269E-276").ToDouble())
        Assert.fail("decfrac double -337655705333269E-276\nExpected: -3.37655705333269E-262d\nWas: "+DecimalFraction.FromString("-337655705333269E-276").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-564484627E-81").ToSingle())
        Assert.fail("decfrac single -564484627E-81\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-564484627E-81").ToSingle());
      if(-5.64484627E-73d!=DecimalFraction.FromString("-564484627E-81").ToDouble())
        Assert.fail("decfrac double -564484627E-81\nExpected: -5.64484627E-73d\nWas: "+DecimalFraction.FromString("-564484627E-81").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-249095219081.80985049618E+175").ToSingle())
        Assert.fail("decfrac single -249095219081.80985049618E+175\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-249095219081.80985049618E+175").ToSingle());
      if(-2.4909521908180986E186d!=DecimalFraction.FromString("-249095219081.80985049618E+175").ToDouble())
        Assert.fail("decfrac double -249095219081.80985049618E+175\nExpected: -2.4909521908180986E186d\nWas: "+DecimalFraction.FromString("-249095219081.80985049618E+175").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-1696361380616078392E+221").ToSingle())
        Assert.fail("decfrac single -1696361380616078392E+221\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-1696361380616078392E+221").ToSingle());
      if(-1.6963613806160784E239d!=DecimalFraction.FromString("-1696361380616078392E+221").ToDouble())
        Assert.fail("decfrac double -1696361380616078392E+221\nExpected: -1.6963613806160784E239d\nWas: "+DecimalFraction.FromString("-1696361380616078392E+221").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("61520501993928105481.8536829047214988E+205").ToSingle())
        Assert.fail("decfrac single 61520501993928105481.8536829047214988E+205\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("61520501993928105481.8536829047214988E+205").ToSingle());
      if(6.15205019939281E224d!=DecimalFraction.FromString("61520501993928105481.8536829047214988E+205").ToDouble())
        Assert.fail("decfrac double 61520501993928105481.8536829047214988E+205\nExpected: 6.15205019939281E224d\nWas: "+DecimalFraction.FromString("61520501993928105481.8536829047214988E+205").ToDouble());
      if(2.08756651E14f!=DecimalFraction.FromString("208756654290770").ToSingle())
        Assert.fail("decfrac single 208756654290770\nExpected: 2.08756651E14f\nWas: "+DecimalFraction.FromString("208756654290770").ToSingle());
      if(2.0875665429077E14d!=DecimalFraction.FromString("208756654290770").ToDouble())
        Assert.fail("decfrac double 208756654290770\nExpected: 2.0875665429077E14d\nWas: "+DecimalFraction.FromString("208756654290770").ToDouble());
      if(-1.31098592E13f!=DecimalFraction.FromString("-13109858687380").ToSingle())
        Assert.fail("decfrac single -13109858687380\nExpected: -1.31098592E13f\nWas: "+DecimalFraction.FromString("-13109858687380").ToSingle());
      if(-1.310985868738E13d!=DecimalFraction.FromString("-13109858687380").ToDouble())
        Assert.fail("decfrac double -13109858687380\nExpected: -1.310985868738E13d\nWas: "+DecimalFraction.FromString("-13109858687380").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("6650596004E+280").ToSingle())
        Assert.fail("decfrac single 6650596004E+280\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("6650596004E+280").ToSingle());
      if(6.650596004E289d!=DecimalFraction.FromString("6650596004E+280").ToDouble())
        Assert.fail("decfrac double 6650596004E+280\nExpected: 6.650596004E289d\nWas: "+DecimalFraction.FromString("6650596004E+280").ToDouble());
      if(-9.2917935E13f!=DecimalFraction.FromString("-92917937534357E0").ToSingle())
        Assert.fail("decfrac single -92917937534357E0\nExpected: -9.2917935E13f\nWas: "+DecimalFraction.FromString("-92917937534357E0").ToSingle());
      if(-9.2917937534357E13d!=DecimalFraction.FromString("-92917937534357E0").ToDouble())
        Assert.fail("decfrac double -92917937534357E0\nExpected: -9.2917937534357E13d\nWas: "+DecimalFraction.FromString("-92917937534357E0").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-46E-153").ToSingle())
        Assert.fail("decfrac single -46E-153\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-46E-153").ToSingle());
      if(-4.6E-152d!=DecimalFraction.FromString("-46E-153").ToDouble())
        Assert.fail("decfrac double -46E-153\nExpected: -4.6E-152d\nWas: "+DecimalFraction.FromString("-46E-153").ToDouble());
      if(1.05161414E13f!=DecimalFraction.FromString("10516141645281.77872161523035480").ToSingle())
        Assert.fail("decfrac single 10516141645281.77872161523035480\nExpected: 1.05161414E13f\nWas: "+DecimalFraction.FromString("10516141645281.77872161523035480").ToSingle());
      if(1.051614164528178E13d!=DecimalFraction.FromString("10516141645281.77872161523035480").ToDouble())
        Assert.fail("decfrac double 10516141645281.77872161523035480\nExpected: 1.051614164528178E13d\nWas: "+DecimalFraction.FromString("10516141645281.77872161523035480").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-8312147094254E+299").ToSingle())
        Assert.fail("decfrac single -8312147094254E+299\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-8312147094254E+299").ToSingle());
      if(Double.NEGATIVE_INFINITY!=DecimalFraction.FromString("-8312147094254E+299").ToDouble())
        Assert.fail("decfrac double -8312147094254E+299\nExpected: Double.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-8312147094254E+299").ToDouble());
      if(5.10270368E8f!=DecimalFraction.FromString("510270376.1879").ToSingle())
        Assert.fail("decfrac single 510270376.1879\nExpected: 5.10270368E8f\nWas: "+DecimalFraction.FromString("510270376.1879").ToSingle());
      if(5.102703761879E8d!=DecimalFraction.FromString("510270376.1879").ToDouble())
        Assert.fail("decfrac double 510270376.1879\nExpected: 5.102703761879E8d\nWas: "+DecimalFraction.FromString("510270376.1879").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-693696E-143").ToSingle())
        Assert.fail("decfrac single -693696E-143\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-693696E-143").ToSingle());
      if(-6.93696E-138d!=DecimalFraction.FromString("-693696E-143").ToDouble())
        Assert.fail("decfrac double -693696E-143\nExpected: -6.93696E-138d\nWas: "+DecimalFraction.FromString("-693696E-143").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-91.43E+139").ToSingle())
        Assert.fail("decfrac single -91.43E+139\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-91.43E+139").ToSingle());
      if(-9.143E140d!=DecimalFraction.FromString("-91.43E+139").ToDouble())
        Assert.fail("decfrac double -91.43E+139\nExpected: -9.143E140d\nWas: "+DecimalFraction.FromString("-91.43E+139").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-4103819741762400.45807953367286162E+235").ToSingle())
        Assert.fail("decfrac single -4103819741762400.45807953367286162E+235\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-4103819741762400.45807953367286162E+235").ToSingle());
      if(-4.1038197417624E250d!=DecimalFraction.FromString("-4103819741762400.45807953367286162E+235").ToDouble())
        Assert.fail("decfrac double -4103819741762400.45807953367286162E+235\nExpected: -4.1038197417624E250d\nWas: "+DecimalFraction.FromString("-4103819741762400.45807953367286162E+235").ToDouble());
      if(-1.44700998E11f!=DecimalFraction.FromString("-144701002301.18954542331279957").ToSingle())
        Assert.fail("decfrac single -144701002301.18954542331279957\nExpected: -1.44700998E11f\nWas: "+DecimalFraction.FromString("-144701002301.18954542331279957").ToSingle());
      if(-1.4470100230118954E11d!=DecimalFraction.FromString("-144701002301.18954542331279957").ToDouble())
        Assert.fail("decfrac double -144701002301.18954542331279957\nExpected: -1.4470100230118954E11d\nWas: "+DecimalFraction.FromString("-144701002301.18954542331279957").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("73.01E+211").ToSingle())
        Assert.fail("decfrac single 73.01E+211\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("73.01E+211").ToSingle());
      if(7.301E212d!=DecimalFraction.FromString("73.01E+211").ToDouble())
        Assert.fail("decfrac double 73.01E+211\nExpected: 7.301E212d\nWas: "+DecimalFraction.FromString("73.01E+211").ToDouble());
      if(-4.4030403E9f!=DecimalFraction.FromString("-4403040441").ToSingle())
        Assert.fail("decfrac single -4403040441\nExpected: -4.4030403E9f\nWas: "+DecimalFraction.FromString("-4403040441").ToSingle());
      if(-4.403040441E9d!=DecimalFraction.FromString("-4403040441").ToDouble())
        Assert.fail("decfrac double -4403040441\nExpected: -4.403040441E9d\nWas: "+DecimalFraction.FromString("-4403040441").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-19E+64").ToSingle())
        Assert.fail("decfrac single -19E+64\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-19E+64").ToSingle());
      if(-1.9E65d!=DecimalFraction.FromString("-19E+64").ToDouble())
        Assert.fail("decfrac double -19E+64\nExpected: -1.9E65d\nWas: "+DecimalFraction.FromString("-19E+64").ToDouble());
      if(0.0f!=DecimalFraction.FromString("6454087684516815.5353496080253E-144").ToSingle())
        Assert.fail("decfrac single 6454087684516815.5353496080253E-144\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("6454087684516815.5353496080253E-144").ToSingle());
      if(6.454087684516816E-129d!=DecimalFraction.FromString("6454087684516815.5353496080253E-144").ToDouble())
        Assert.fail("decfrac double 6454087684516815.5353496080253E-144\nExpected: 6.454087684516816E-129d\nWas: "+DecimalFraction.FromString("6454087684516815.5353496080253E-144").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("1051852710343668.522107559786846776E+278").ToSingle())
        Assert.fail("decfrac single 1051852710343668.522107559786846776E+278\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("1051852710343668.522107559786846776E+278").ToSingle());
      if(1.0518527103436685E293d!=DecimalFraction.FromString("1051852710343668.522107559786846776E+278").ToDouble())
        Assert.fail("decfrac double 1051852710343668.522107559786846776E+278\nExpected: 1.0518527103436685E293d\nWas: "+DecimalFraction.FromString("1051852710343668.522107559786846776E+278").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("86077128802.374518623891E+218").ToSingle())
        Assert.fail("decfrac single 86077128802.374518623891E+218\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("86077128802.374518623891E+218").ToSingle());
      if(8.607712880237452E228d!=DecimalFraction.FromString("86077128802.374518623891E+218").ToDouble())
        Assert.fail("decfrac double 86077128802.374518623891E+218\nExpected: 8.607712880237452E228d\nWas: "+DecimalFraction.FromString("86077128802.374518623891E+218").ToDouble());
      if(0.0f!=DecimalFraction.FromString("367820230207102E-199").ToSingle())
        Assert.fail("decfrac single 367820230207102E-199\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("367820230207102E-199").ToSingle());
      if(3.67820230207102E-185d!=DecimalFraction.FromString("367820230207102E-199").ToDouble())
        Assert.fail("decfrac double 367820230207102E-199\nExpected: 3.67820230207102E-185d\nWas: "+DecimalFraction.FromString("367820230207102E-199").ToDouble());
      if(9.105086E-27f!=DecimalFraction.FromString("91050857573912688994E-46").ToSingle())
        Assert.fail("decfrac single 91050857573912688994E-46\nExpected: 9.105086E-27f\nWas: "+DecimalFraction.FromString("91050857573912688994E-46").ToSingle());
      if(9.105085757391269E-27d!=DecimalFraction.FromString("91050857573912688994E-46").ToDouble())
        Assert.fail("decfrac double 91050857573912688994E-46\nExpected: 9.105085757391269E-27d\nWas: "+DecimalFraction.FromString("91050857573912688994E-46").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("73.895899E+102").ToSingle())
        Assert.fail("decfrac single 73.895899E+102\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("73.895899E+102").ToSingle());
      if(7.3895899E103d!=DecimalFraction.FromString("73.895899E+102").ToDouble())
        Assert.fail("decfrac double 73.895899E+102\nExpected: 7.3895899E103d\nWas: "+DecimalFraction.FromString("73.895899E+102").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-796808893178.891470585829021E+330").ToSingle())
        Assert.fail("decfrac single -796808893178.891470585829021E+330\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-796808893178.891470585829021E+330").ToSingle());
      if(Double.NEGATIVE_INFINITY!=DecimalFraction.FromString("-796808893178.891470585829021E+330").ToDouble())
        Assert.fail("decfrac double -796808893178.891470585829021E+330\nExpected: Double.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-796808893178.891470585829021E+330").ToDouble());
      if(0.0f!=DecimalFraction.FromString("275081E-206").ToSingle())
        Assert.fail("decfrac single 275081E-206\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("275081E-206").ToSingle());
      if(2.75081E-201d!=DecimalFraction.FromString("275081E-206").ToDouble())
        Assert.fail("decfrac double 275081E-206\nExpected: 2.75081E-201d\nWas: "+DecimalFraction.FromString("275081E-206").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-4322898910615499.82096E-95").ToSingle())
        Assert.fail("decfrac single -4322898910615499.82096E-95\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-4322898910615499.82096E-95").ToSingle());
      if(-4.3228989106155E-80d!=DecimalFraction.FromString("-4322898910615499.82096E-95").ToDouble())
        Assert.fail("decfrac double -4322898910615499.82096E-95\nExpected: -4.3228989106155E-80d\nWas: "+DecimalFraction.FromString("-4322898910615499.82096E-95").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("143343913109764E+63").ToSingle())
        Assert.fail("decfrac single 143343913109764E+63\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("143343913109764E+63").ToSingle());
      if(1.43343913109764E77d!=DecimalFraction.FromString("143343913109764E+63").ToDouble())
        Assert.fail("decfrac double 143343913109764E+63\nExpected: 1.43343913109764E77d\nWas: "+DecimalFraction.FromString("143343913109764E+63").ToDouble());
      if(-7.9102981E16f!=DecimalFraction.FromString("-79102983237104015").ToSingle())
        Assert.fail("decfrac single -79102983237104015\nExpected: -7.9102981E16f\nWas: "+DecimalFraction.FromString("-79102983237104015").ToSingle());
      if(-7.9102983237104016E16d!=DecimalFraction.FromString("-79102983237104015").ToDouble())
        Assert.fail("decfrac double -79102983237104015\nExpected: -7.9102983237104016E16d\nWas: "+DecimalFraction.FromString("-79102983237104015").ToDouble());
      if(-9.07E-10f!=DecimalFraction.FromString("-907E-12").ToSingle())
        Assert.fail("decfrac single -907E-12\nExpected: -9.07E-10f\nWas: "+DecimalFraction.FromString("-907E-12").ToSingle());
      if(-9.07E-10d!=DecimalFraction.FromString("-907E-12").ToDouble())
        Assert.fail("decfrac double -907E-12\nExpected: -9.07E-10d\nWas: "+DecimalFraction.FromString("-907E-12").ToDouble());
      if(0.0f!=DecimalFraction.FromString("191682103431.217475E-84").ToSingle())
        Assert.fail("decfrac single 191682103431.217475E-84\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("191682103431.217475E-84").ToSingle());
      if(1.9168210343121748E-73d!=DecimalFraction.FromString("191682103431.217475E-84").ToDouble())
        Assert.fail("decfrac double 191682103431.217475E-84\nExpected: 1.9168210343121748E-73d\nWas: "+DecimalFraction.FromString("191682103431.217475E-84").ToDouble());
      if(-5.6E-45f!=DecimalFraction.FromString("-492913.1840948615992120438E-50").ToSingle())
        Assert.fail("decfrac single -492913.1840948615992120438E-50\nExpected: -5.6E-45f\nWas: "+DecimalFraction.FromString("-492913.1840948615992120438E-50").ToSingle());
      if(-4.929131840948616E-45d!=DecimalFraction.FromString("-492913.1840948615992120438E-50").ToDouble())
        Assert.fail("decfrac double -492913.1840948615992120438E-50\nExpected: -4.929131840948616E-45d\nWas: "+DecimalFraction.FromString("-492913.1840948615992120438E-50").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-752873150058767E+272").ToSingle())
        Assert.fail("decfrac single -752873150058767E+272\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-752873150058767E+272").ToSingle());
      if(-7.52873150058767E286d!=DecimalFraction.FromString("-752873150058767E+272").ToDouble())
        Assert.fail("decfrac double -752873150058767E+272\nExpected: -7.52873150058767E286d\nWas: "+DecimalFraction.FromString("-752873150058767E+272").ToDouble());
      if(27.311937f!=DecimalFraction.FromString("27.311937404").ToSingle())
        Assert.fail("decfrac single 27.311937404\nExpected: 27.311937f\nWas: "+DecimalFraction.FromString("27.311937404").ToSingle());
      if(27.311937404d!=DecimalFraction.FromString("27.311937404").ToDouble())
        Assert.fail("decfrac double 27.311937404\nExpected: 27.311937404d\nWas: "+DecimalFraction.FromString("27.311937404").ToDouble());
      if(0.0f!=DecimalFraction.FromString("39147083343918E-143").ToSingle())
        Assert.fail("decfrac single 39147083343918E-143\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("39147083343918E-143").ToSingle());
      if(3.9147083343918E-130d!=DecimalFraction.FromString("39147083343918E-143").ToDouble())
        Assert.fail("decfrac double 39147083343918E-143\nExpected: 3.9147083343918E-130d\nWas: "+DecimalFraction.FromString("39147083343918E-143").ToDouble());
      if(-1.97684019E11f!=DecimalFraction.FromString("-197684018253").ToSingle())
        Assert.fail("decfrac single -197684018253\nExpected: -1.97684019E11f\nWas: "+DecimalFraction.FromString("-197684018253").ToSingle());
      if(-1.97684018253E11d!=DecimalFraction.FromString("-197684018253").ToDouble())
        Assert.fail("decfrac double -197684018253\nExpected: -1.97684018253E11d\nWas: "+DecimalFraction.FromString("-197684018253").ToDouble());
      if(6.400822E14f!=DecimalFraction.FromString("640082188903507").ToSingle())
        Assert.fail("decfrac single 640082188903507\nExpected: 6.400822E14f\nWas: "+DecimalFraction.FromString("640082188903507").ToSingle());
      if(6.40082188903507E14d!=DecimalFraction.FromString("640082188903507").ToDouble())
        Assert.fail("decfrac double 640082188903507\nExpected: 6.40082188903507E14d\nWas: "+DecimalFraction.FromString("640082188903507").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-913144352720144E-312").ToSingle())
        Assert.fail("decfrac single -913144352720144E-312\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-913144352720144E-312").ToSingle());
      if(-9.13144352720144E-298d!=DecimalFraction.FromString("-913144352720144E-312").ToDouble())
        Assert.fail("decfrac double -913144352720144E-312\nExpected: -9.13144352720144E-298d\nWas: "+DecimalFraction.FromString("-913144352720144E-312").ToDouble());
      if(-3.68781005E15f!=DecimalFraction.FromString("-3687809947210631").ToSingle())
        Assert.fail("decfrac single -3687809947210631\nExpected: -3.68781005E15f\nWas: "+DecimalFraction.FromString("-3687809947210631").ToSingle());
      if(-3.687809947210631E15d!=DecimalFraction.FromString("-3687809947210631").ToDouble())
        Assert.fail("decfrac double -3687809947210631\nExpected: -3.687809947210631E15d\nWas: "+DecimalFraction.FromString("-3687809947210631").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("53083788630724917310.06236692262351E+169").ToSingle())
        Assert.fail("decfrac single 53083788630724917310.06236692262351E+169\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("53083788630724917310.06236692262351E+169").ToSingle());
      if(5.3083788630724916E188d!=DecimalFraction.FromString("53083788630724917310.06236692262351E+169").ToDouble())
        Assert.fail("decfrac double 53083788630724917310.06236692262351E+169\nExpected: 5.3083788630724916E188d\nWas: "+DecimalFraction.FromString("53083788630724917310.06236692262351E+169").ToDouble());
      if(-7.0943446E19f!=DecimalFraction.FromString("-70943446332471357958").ToSingle())
        Assert.fail("decfrac single -70943446332471357958\nExpected: -7.0943446E19f\nWas: "+DecimalFraction.FromString("-70943446332471357958").ToSingle());
      if(-7.094344633247136E19d!=DecimalFraction.FromString("-70943446332471357958").ToDouble())
        Assert.fail("decfrac double -70943446332471357958\nExpected: -7.094344633247136E19d\nWas: "+DecimalFraction.FromString("-70943446332471357958").ToDouble());
      if(63367.23f!=DecimalFraction.FromString("63367.23157744207").ToSingle())
        Assert.fail("decfrac single 63367.23157744207\nExpected: 63367.23f\nWas: "+DecimalFraction.FromString("63367.23157744207").ToSingle());
      if(63367.23157744207d!=DecimalFraction.FromString("63367.23157744207").ToDouble())
        Assert.fail("decfrac double 63367.23157744207\nExpected: 63367.23157744207d\nWas: "+DecimalFraction.FromString("63367.23157744207").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("2100535E+120").ToSingle())
        Assert.fail("decfrac single 2100535E+120\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("2100535E+120").ToSingle());
      if(2.100535E126d!=DecimalFraction.FromString("2100535E+120").ToDouble())
        Assert.fail("decfrac double 2100535E+120\nExpected: 2.100535E126d\nWas: "+DecimalFraction.FromString("2100535E+120").ToDouble());
      if(0.0f!=DecimalFraction.FromString("914534543212037911E-174").ToSingle())
        Assert.fail("decfrac single 914534543212037911E-174\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("914534543212037911E-174").ToSingle());
      if(9.14534543212038E-157d!=DecimalFraction.FromString("914534543212037911E-174").ToDouble())
        Assert.fail("decfrac double 914534543212037911E-174\nExpected: 9.14534543212038E-157d\nWas: "+DecimalFraction.FromString("914534543212037911E-174").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-12437185743660570E-180").ToSingle())
        Assert.fail("decfrac single -12437185743660570E-180\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-12437185743660570E-180").ToSingle());
      if(-1.243718574366057E-164d!=DecimalFraction.FromString("-12437185743660570E-180").ToDouble())
        Assert.fail("decfrac double -12437185743660570E-180\nExpected: -1.243718574366057E-164d\nWas: "+DecimalFraction.FromString("-12437185743660570E-180").ToDouble());
      if(-3.3723915E19f!=DecimalFraction.FromString("-33723915695913879E+3").ToSingle())
        Assert.fail("decfrac single -33723915695913879E+3\nExpected: -3.3723915E19f\nWas: "+DecimalFraction.FromString("-33723915695913879E+3").ToSingle());
      if(-3.3723915695913878E19d!=DecimalFraction.FromString("-33723915695913879E+3").ToDouble())
        Assert.fail("decfrac double -33723915695913879E+3\nExpected: -3.3723915695913878E19d\nWas: "+DecimalFraction.FromString("-33723915695913879E+3").ToDouble());
      if(6.3664833E10f!=DecimalFraction.FromString("63664831787").ToSingle())
        Assert.fail("decfrac single 63664831787\nExpected: 6.3664833E10f\nWas: "+DecimalFraction.FromString("63664831787").ToSingle());
      if(6.3664831787E10d!=DecimalFraction.FromString("63664831787").ToDouble())
        Assert.fail("decfrac double 63664831787\nExpected: 6.3664831787E10d\nWas: "+DecimalFraction.FromString("63664831787").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("432187105445201137.3321724908E+97").ToSingle())
        Assert.fail("decfrac single 432187105445201137.3321724908E+97\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("432187105445201137.3321724908E+97").ToSingle());
      if(4.321871054452011E114d!=DecimalFraction.FromString("432187105445201137.3321724908E+97").ToDouble())
        Assert.fail("decfrac double 432187105445201137.3321724908E+97\nExpected: 4.321871054452011E114d\nWas: "+DecimalFraction.FromString("432187105445201137.3321724908E+97").ToDouble());
      if(-5.1953271E13f!=DecimalFraction.FromString("-51953270775979").ToSingle())
        Assert.fail("decfrac single -51953270775979\nExpected: -5.1953271E13f\nWas: "+DecimalFraction.FromString("-51953270775979").ToSingle());
      if(-5.1953270775979E13d!=DecimalFraction.FromString("-51953270775979").ToDouble())
        Assert.fail("decfrac double -51953270775979\nExpected: -5.1953270775979E13d\nWas: "+DecimalFraction.FromString("-51953270775979").ToDouble());
      if(2.14953088E9f!=DecimalFraction.FromString("2149530805").ToSingle())
        Assert.fail("decfrac single 2149530805\nExpected: 2.14953088E9f\nWas: "+DecimalFraction.FromString("2149530805").ToSingle());
      if(2.149530805E9d!=DecimalFraction.FromString("2149530805").ToDouble())
        Assert.fail("decfrac double 2149530805\nExpected: 2.149530805E9d\nWas: "+DecimalFraction.FromString("2149530805").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-4672759140.6362E-223").ToSingle())
        Assert.fail("decfrac single -4672759140.6362E-223\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-4672759140.6362E-223").ToSingle());
      if(-4.6727591406362E-214d!=DecimalFraction.FromString("-4672759140.6362E-223").ToDouble())
        Assert.fail("decfrac double -4672759140.6362E-223\nExpected: -4.6727591406362E-214d\nWas: "+DecimalFraction.FromString("-4672759140.6362E-223").ToDouble());
      if(-9.0f!=DecimalFraction.FromString("-9").ToSingle())
        Assert.fail("decfrac single -9\nExpected: -9.0f\nWas: "+DecimalFraction.FromString("-9").ToSingle());
      if(-9.0d!=DecimalFraction.FromString("-9").ToDouble())
        Assert.fail("decfrac double -9\nExpected: -9.0d\nWas: "+DecimalFraction.FromString("-9").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-1903960322936E+304").ToSingle())
        Assert.fail("decfrac single -1903960322936E+304\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-1903960322936E+304").ToSingle());
      if(Double.NEGATIVE_INFINITY!=DecimalFraction.FromString("-1903960322936E+304").ToDouble())
        Assert.fail("decfrac double -1903960322936E+304\nExpected: Double.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-1903960322936E+304").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("405766405417980707E+316").ToSingle())
        Assert.fail("decfrac single 405766405417980707E+316\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("405766405417980707E+316").ToSingle());
      if(Double.POSITIVE_INFINITY!=DecimalFraction.FromString("405766405417980707E+316").ToDouble())
        Assert.fail("decfrac double 405766405417980707E+316\nExpected: Double.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("405766405417980707E+316").ToDouble());
      if(-166174.94f!=DecimalFraction.FromString("-1661749343992047E-10").ToSingle())
        Assert.fail("decfrac single -1661749343992047E-10\nExpected: -166174.94f\nWas: "+DecimalFraction.FromString("-1661749343992047E-10").ToSingle());
      if(-166174.9343992047d!=DecimalFraction.FromString("-1661749343992047E-10").ToDouble())
        Assert.fail("decfrac double -1661749343992047E-10\nExpected: -166174.9343992047d\nWas: "+DecimalFraction.FromString("-1661749343992047E-10").ToDouble());
      if(5893094.0f!=DecimalFraction.FromString("5893094.099969899224047667").ToSingle())
        Assert.fail("decfrac single 5893094.099969899224047667\nExpected: 5893094.0f\nWas: "+DecimalFraction.FromString("5893094.099969899224047667").ToSingle());
      if(5893094.099969899d!=DecimalFraction.FromString("5893094.099969899224047667").ToDouble())
        Assert.fail("decfrac double 5893094.099969899224047667\nExpected: 5893094.099969899d\nWas: "+DecimalFraction.FromString("5893094.099969899224047667").ToDouble());
      if(-3.4023195E17f!=DecimalFraction.FromString("-340231946762317122").ToSingle())
        Assert.fail("decfrac single -340231946762317122\nExpected: -3.4023195E17f\nWas: "+DecimalFraction.FromString("-340231946762317122").ToSingle());
      if(-3.4023194676231712E17d!=DecimalFraction.FromString("-340231946762317122").ToDouble())
        Assert.fail("decfrac double -340231946762317122\nExpected: -3.4023194676231712E17d\nWas: "+DecimalFraction.FromString("-340231946762317122").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("3.10041643978E+236").ToSingle())
        Assert.fail("decfrac single 3.10041643978E+236\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("3.10041643978E+236").ToSingle());
      if(3.10041643978E236d!=DecimalFraction.FromString("3.10041643978E+236").ToDouble())
        Assert.fail("decfrac double 3.10041643978E+236\nExpected: 3.10041643978E236d\nWas: "+DecimalFraction.FromString("3.10041643978E+236").ToDouble());
      if(1.43429217E13f!=DecimalFraction.FromString("14342921940186").ToSingle())
        Assert.fail("decfrac single 14342921940186\nExpected: 1.43429217E13f\nWas: "+DecimalFraction.FromString("14342921940186").ToSingle());
      if(1.4342921940186E13d!=DecimalFraction.FromString("14342921940186").ToDouble())
        Assert.fail("decfrac double 14342921940186\nExpected: 1.4342921940186E13d\nWas: "+DecimalFraction.FromString("14342921940186").ToDouble());
      if(1.97766234E9f!=DecimalFraction.FromString("1977662368").ToSingle())
        Assert.fail("decfrac single 1977662368\nExpected: 1.97766234E9f\nWas: "+DecimalFraction.FromString("1977662368").ToSingle());
      if(1.977662368E9d!=DecimalFraction.FromString("1977662368").ToDouble())
        Assert.fail("decfrac double 1977662368\nExpected: 1.977662368E9d\nWas: "+DecimalFraction.FromString("1977662368").ToDouble());
      if(0.0f!=DecimalFraction.FromString("891.32009975058011674E-268").ToSingle())
        Assert.fail("decfrac single 891.32009975058011674E-268\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("891.32009975058011674E-268").ToSingle());
      if(8.913200997505801E-266d!=DecimalFraction.FromString("891.32009975058011674E-268").ToDouble())
        Assert.fail("decfrac double 891.32009975058011674E-268\nExpected: 8.913200997505801E-266d\nWas: "+DecimalFraction.FromString("891.32009975058011674E-268").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-895468936291.471679344983419E+316").ToSingle())
        Assert.fail("decfrac single -895468936291.471679344983419E+316\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-895468936291.471679344983419E+316").ToSingle());
      if(Double.NEGATIVE_INFINITY!=DecimalFraction.FromString("-895468936291.471679344983419E+316").ToDouble())
        Assert.fail("decfrac double -895468936291.471679344983419E+316\nExpected: Double.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-895468936291.471679344983419E+316").ToDouble());
      if(0.0f!=DecimalFraction.FromString("61308E-104").ToSingle())
        Assert.fail("decfrac single 61308E-104\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("61308E-104").ToSingle());
      if(6.1308E-100d!=DecimalFraction.FromString("61308E-104").ToDouble())
        Assert.fail("decfrac double 61308E-104\nExpected: 6.1308E-100d\nWas: "+DecimalFraction.FromString("61308E-104").ToDouble());
      if(-5362.791f!=DecimalFraction.FromString("-5362.79122778669072").ToSingle())
        Assert.fail("decfrac single -5362.79122778669072\nExpected: -5362.791f\nWas: "+DecimalFraction.FromString("-5362.79122778669072").ToSingle());
      if(-5362.791227786691d!=DecimalFraction.FromString("-5362.79122778669072").ToDouble())
        Assert.fail("decfrac double -5362.79122778669072\nExpected: -5362.791227786691d\nWas: "+DecimalFraction.FromString("-5362.79122778669072").ToDouble());
      if(0.0f!=DecimalFraction.FromString("861664379590901308.23330613776542261919E-101").ToSingle())
        Assert.fail("decfrac single 861664379590901308.23330613776542261919E-101\nExpected: 0.0f\nWas: "+DecimalFraction.FromString("861664379590901308.23330613776542261919E-101").ToSingle());
      if(8.616643795909013E-84d!=DecimalFraction.FromString("861664379590901308.23330613776542261919E-101").ToDouble())
        Assert.fail("decfrac double 861664379590901308.23330613776542261919E-101\nExpected: 8.616643795909013E-84d\nWas: "+DecimalFraction.FromString("861664379590901308.23330613776542261919E-101").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-1884773180.50192918329237967651E+204").ToSingle())
        Assert.fail("decfrac single -1884773180.50192918329237967651E+204\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-1884773180.50192918329237967651E+204").ToSingle());
      if(-1.884773180501929E213d!=DecimalFraction.FromString("-1884773180.50192918329237967651E+204").ToDouble())
        Assert.fail("decfrac double -1884773180.50192918329237967651E+204\nExpected: -1.884773180501929E213d\nWas: "+DecimalFraction.FromString("-1884773180.50192918329237967651E+204").ToDouble());
      if(1.89187207E13f!=DecimalFraction.FromString("18918720095123.6152").ToSingle())
        Assert.fail("decfrac single 18918720095123.6152\nExpected: 1.89187207E13f\nWas: "+DecimalFraction.FromString("18918720095123.6152").ToSingle());
      if(1.8918720095123613E13d!=DecimalFraction.FromString("18918720095123.6152").ToDouble())
        Assert.fail("decfrac double 18918720095123.6152\nExpected: 1.8918720095123613E13d\nWas: "+DecimalFraction.FromString("18918720095123.6152").ToDouble());
      if(94667.95f!=DecimalFraction.FromString("94667.95264211741602").ToSingle())
        Assert.fail("decfrac single 94667.95264211741602\nExpected: 94667.95f\nWas: "+DecimalFraction.FromString("94667.95264211741602").ToSingle());
      if(94667.95264211742d!=DecimalFraction.FromString("94667.95264211741602").ToDouble())
        Assert.fail("decfrac double 94667.95264211741602\nExpected: 94667.95264211742d\nWas: "+DecimalFraction.FromString("94667.95264211741602").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("1230618521424E+134").ToSingle())
        Assert.fail("decfrac single 1230618521424E+134\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("1230618521424E+134").ToSingle());
      if(1.230618521424E146d!=DecimalFraction.FromString("1230618521424E+134").ToDouble())
        Assert.fail("decfrac double 1230618521424E+134\nExpected: 1.230618521424E146d\nWas: "+DecimalFraction.FromString("1230618521424E+134").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("3022403935588782E+85").ToSingle())
        Assert.fail("decfrac single 3022403935588782E+85\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("3022403935588782E+85").ToSingle());
      if(3.022403935588782E100d!=DecimalFraction.FromString("3022403935588782E+85").ToDouble())
        Assert.fail("decfrac double 3022403935588782E+85\nExpected: 3.022403935588782E100d\nWas: "+DecimalFraction.FromString("3022403935588782E+85").ToDouble());
      if(Float.POSITIVE_INFINITY!=DecimalFraction.FromString("64543E+274").ToSingle())
        Assert.fail("decfrac single 64543E+274\nExpected: Float.POSITIVE_INFINITY\nWas: "+DecimalFraction.FromString("64543E+274").ToSingle());
      if(6.4543E278d!=DecimalFraction.FromString("64543E+274").ToDouble())
        Assert.fail("decfrac double 64543E+274\nExpected: 6.4543E278d\nWas: "+DecimalFraction.FromString("64543E+274").ToDouble());
      if(6.7181355E10f!=DecimalFraction.FromString("67181356837.903551518080873954").ToSingle())
        Assert.fail("decfrac single 67181356837.903551518080873954\nExpected: 6.7181355E10f\nWas: "+DecimalFraction.FromString("67181356837.903551518080873954").ToSingle());
      if(6.718135683790355E10d!=DecimalFraction.FromString("67181356837.903551518080873954").ToDouble())
        Assert.fail("decfrac double 67181356837.903551518080873954\nExpected: 6.718135683790355E10d\nWas: "+DecimalFraction.FromString("67181356837.903551518080873954").ToDouble());
      if(-0.0f!=DecimalFraction.FromString("-4508016E-321").ToSingle())
        Assert.fail("decfrac single -4508016E-321\nExpected: -0.0f\nWas: "+DecimalFraction.FromString("-4508016E-321").ToSingle());
      if(-4.508016E-315d!=DecimalFraction.FromString("-4508016E-321").ToDouble())
        Assert.fail("decfrac double -4508016E-321\nExpected: -4.508016E-315d\nWas: "+DecimalFraction.FromString("-4508016E-321").ToDouble());
      if(Float.NEGATIVE_INFINITY!=DecimalFraction.FromString("-62855032520.512452348497E+39").ToSingle())
        Assert.fail("decfrac single -62855032520.512452348497E+39\nExpected: Float.NEGATIVE_INFINITY\nWas: "+DecimalFraction.FromString("-62855032520.512452348497E+39").ToSingle());
      if(-6.285503252051245E49d!=DecimalFraction.FromString("-62855032520.512452348497E+39").ToDouble())
        Assert.fail("decfrac double -62855032520.512452348497E+39\nExpected: -6.285503252051245E49d\nWas: "+DecimalFraction.FromString("-62855032520.512452348497E+39").ToDouble());
      if(3177.2236f!=DecimalFraction.FromString("3177.2237286").ToSingle())
        Assert.fail("decfrac single 3177.2237286\nExpected: 3177.2236f\nWas: "+DecimalFraction.FromString("3177.2237286").ToSingle());
      if(3177.2237286d!=DecimalFraction.FromString("3177.2237286").ToDouble())
        Assert.fail("decfrac double 3177.2237286\nExpected: 3177.2237286d\nWas: "+DecimalFraction.FromString("3177.2237286").ToDouble());
      if(-7.950583E8f!=DecimalFraction.FromString("-795058316.9186492185346968").ToSingle())
        Assert.fail("decfrac single -795058316.9186492185346968\nExpected: -7.950583E8f\nWas: "+DecimalFraction.FromString("-795058316.9186492185346968").ToSingle());
      if(-7.950583169186492E8d!=DecimalFraction.FromString("-795058316.9186492185346968").ToDouble())
        Assert.fail("decfrac double -795058316.9186492185346968\nExpected: -7.950583169186492E8d\nWas: "+DecimalFraction.FromString("-795058316.9186492185346968").ToDouble());
    }
    
    /**
     * 
     */
@Test
    public void TestDecFracIntegersToSingleDouble() {
      if(-5.7703064E7f!=DecimalFraction.FromString("-57703066").ToSingle())
        Assert.fail("decfrac single -57703066\nExpected: -5.7703064E7f\nWas: "+DecimalFraction.FromString("-57703066").ToSingle());
      if(-5.7703066E7d!=DecimalFraction.FromString("-57703066").ToDouble())
        Assert.fail("decfrac double -57703066\nExpected: -5.7703066E7d\nWas: "+DecimalFraction.FromString("-57703066").ToDouble());
      if(1590432.0f!=DecimalFraction.FromString("1590432").ToSingle())
        Assert.fail("decfrac single 1590432\nExpected: 1590432.0f\nWas: "+DecimalFraction.FromString("1590432").ToSingle());
      if(1590432.0d!=DecimalFraction.FromString("1590432").ToDouble())
        Assert.fail("decfrac double 1590432\nExpected: 1590432.0d\nWas: "+DecimalFraction.FromString("1590432").ToDouble());
      if(9.5464253E9f!=DecimalFraction.FromString("9546425267").ToSingle())
        Assert.fail("decfrac single 9546425267\nExpected: 9.5464253E9f\nWas: "+DecimalFraction.FromString("9546425267").ToSingle());
      if(9.546425267E9d!=DecimalFraction.FromString("9546425267").ToDouble())
        Assert.fail("decfrac double 9546425267\nExpected: 9.546425267E9d\nWas: "+DecimalFraction.FromString("9546425267").ToDouble());
      if(7.3227311E16f!=DecimalFraction.FromString("73227309698439247").ToSingle())
        Assert.fail("decfrac single 73227309698439247\nExpected: 7.3227311E16f\nWas: "+DecimalFraction.FromString("73227309698439247").ToSingle());
      if(7.3227309698439248E16d!=DecimalFraction.FromString("73227309698439247").ToDouble())
        Assert.fail("decfrac double 73227309698439247\nExpected: 7.3227309698439248E16d\nWas: "+DecimalFraction.FromString("73227309698439247").ToDouble());
      if(75114.0f!=DecimalFraction.FromString("75114").ToSingle())
        Assert.fail("decfrac single 75114\nExpected: 75114.0f\nWas: "+DecimalFraction.FromString("75114").ToSingle());
      if(75114.0d!=DecimalFraction.FromString("75114").ToDouble())
        Assert.fail("decfrac double 75114\nExpected: 75114.0d\nWas: "+DecimalFraction.FromString("75114").ToDouble());
      if(64.0f!=DecimalFraction.FromString("64").ToSingle())
        Assert.fail("decfrac single 64\nExpected: 64.0f\nWas: "+DecimalFraction.FromString("64").ToSingle());
      if(64.0d!=DecimalFraction.FromString("64").ToDouble())
        Assert.fail("decfrac double 64\nExpected: 64.0d\nWas: "+DecimalFraction.FromString("64").ToDouble());
      if(8.6352293E15f!=DecimalFraction.FromString("8635229353951207").ToSingle())
        Assert.fail("decfrac single 8635229353951207\nExpected: 8.6352293E15f\nWas: "+DecimalFraction.FromString("8635229353951207").ToSingle());
      if(8.635229353951207E15d!=DecimalFraction.FromString("8635229353951207").ToDouble())
        Assert.fail("decfrac double 8635229353951207\nExpected: 8.635229353951207E15d\nWas: "+DecimalFraction.FromString("8635229353951207").ToDouble());
      if(-8.056573E19f!=DecimalFraction.FromString("-80565729661447979457").ToSingle())
        Assert.fail("decfrac single -80565729661447979457\nExpected: -8.056573E19f\nWas: "+DecimalFraction.FromString("-80565729661447979457").ToSingle());
      if(-8.056572966144798E19d!=DecimalFraction.FromString("-80565729661447979457").ToDouble())
        Assert.fail("decfrac double -80565729661447979457\nExpected: -8.056572966144798E19d\nWas: "+DecimalFraction.FromString("-80565729661447979457").ToDouble());
      if(8.1540558E14f!=DecimalFraction.FromString("815405565228754").ToSingle())
        Assert.fail("decfrac single 815405565228754\nExpected: 8.1540558E14f\nWas: "+DecimalFraction.FromString("815405565228754").ToSingle());
      if(8.15405565228754E14d!=DecimalFraction.FromString("815405565228754").ToDouble())
        Assert.fail("decfrac double 815405565228754\nExpected: 8.15405565228754E14d\nWas: "+DecimalFraction.FromString("815405565228754").ToDouble());
      if(-6.1008438E16f!=DecimalFraction.FromString("-61008438357089231").ToSingle())
        Assert.fail("decfrac single -61008438357089231\nExpected: -6.1008438E16f\nWas: "+DecimalFraction.FromString("-61008438357089231").ToSingle());
      if(-6.1008438357089232E16d!=DecimalFraction.FromString("-61008438357089231").ToDouble())
        Assert.fail("decfrac double -61008438357089231\nExpected: -6.1008438357089232E16d\nWas: "+DecimalFraction.FromString("-61008438357089231").ToDouble());
      if(-46526.0f!=DecimalFraction.FromString("-46526").ToSingle())
        Assert.fail("decfrac single -46526\nExpected: -46526.0f\nWas: "+DecimalFraction.FromString("-46526").ToSingle());
      if(-46526.0d!=DecimalFraction.FromString("-46526").ToDouble())
        Assert.fail("decfrac double -46526\nExpected: -46526.0d\nWas: "+DecimalFraction.FromString("-46526").ToDouble());
      if(5.1199847E18f!=DecimalFraction.FromString("5119984668352258853").ToSingle())
        Assert.fail("decfrac single 5119984668352258853\nExpected: 5.1199847E18f\nWas: "+DecimalFraction.FromString("5119984668352258853").ToSingle());
      if(5.1199846683522591E18d!=DecimalFraction.FromString("5119984668352258853").ToDouble())
        Assert.fail("decfrac double 5119984668352258853\nExpected: 5.1199846683522591E18d\nWas: "+DecimalFraction.FromString("5119984668352258853").ToDouble());
      if(1851.0f!=DecimalFraction.FromString("1851").ToSingle())
        Assert.fail("decfrac single 1851\nExpected: 1851.0f\nWas: "+DecimalFraction.FromString("1851").ToSingle());
      if(1851.0d!=DecimalFraction.FromString("1851").ToDouble())
        Assert.fail("decfrac double 1851\nExpected: 1851.0d\nWas: "+DecimalFraction.FromString("1851").ToDouble());
      if(8.7587332E15f!=DecimalFraction.FromString("8758733009763848").ToSingle())
        Assert.fail("decfrac single 8758733009763848\nExpected: 8.7587332E15f\nWas: "+DecimalFraction.FromString("8758733009763848").ToSingle());
      if(8.758733009763848E15d!=DecimalFraction.FromString("8758733009763848").ToDouble())
        Assert.fail("decfrac double 8758733009763848\nExpected: 8.758733009763848E15d\nWas: "+DecimalFraction.FromString("8758733009763848").ToDouble());
      if(51.0f!=DecimalFraction.FromString("51").ToSingle())
        Assert.fail("decfrac single 51\nExpected: 51.0f\nWas: "+DecimalFraction.FromString("51").ToSingle());
      if(51.0d!=DecimalFraction.FromString("51").ToDouble())
        Assert.fail("decfrac double 51\nExpected: 51.0d\nWas: "+DecimalFraction.FromString("51").ToDouble());
      if(9.4281774E11f!=DecimalFraction.FromString("942817726107").ToSingle())
        Assert.fail("decfrac single 942817726107\nExpected: 9.4281774E11f\nWas: "+DecimalFraction.FromString("942817726107").ToSingle());
      if(9.42817726107E11d!=DecimalFraction.FromString("942817726107").ToDouble())
        Assert.fail("decfrac double 942817726107\nExpected: 9.42817726107E11d\nWas: "+DecimalFraction.FromString("942817726107").ToDouble());
      if(186575.0f!=DecimalFraction.FromString("186575").ToSingle())
        Assert.fail("decfrac single 186575\nExpected: 186575.0f\nWas: "+DecimalFraction.FromString("186575").ToSingle());
      if(186575.0d!=DecimalFraction.FromString("186575").ToDouble())
        Assert.fail("decfrac double 186575\nExpected: 186575.0d\nWas: "+DecimalFraction.FromString("186575").ToDouble());
      if(-3.47313997E9f!=DecimalFraction.FromString("-3473140020").ToSingle())
        Assert.fail("decfrac single -3473140020\nExpected: -3.47313997E9f\nWas: "+DecimalFraction.FromString("-3473140020").ToSingle());
      if(-3.47314002E9d!=DecimalFraction.FromString("-3473140020").ToDouble())
        Assert.fail("decfrac double -3473140020\nExpected: -3.47314002E9d\nWas: "+DecimalFraction.FromString("-3473140020").ToDouble());
      if(2.66134912E8f!=DecimalFraction.FromString("266134912").ToSingle())
        Assert.fail("decfrac single 266134912\nExpected: 2.66134912E8f\nWas: "+DecimalFraction.FromString("266134912").ToSingle());
      if(2.66134912E8d!=DecimalFraction.FromString("266134912").ToDouble())
        Assert.fail("decfrac double 266134912\nExpected: 2.66134912E8d\nWas: "+DecimalFraction.FromString("266134912").ToDouble());
      if(5209.0f!=DecimalFraction.FromString("5209").ToSingle())
        Assert.fail("decfrac single 5209\nExpected: 5209.0f\nWas: "+DecimalFraction.FromString("5209").ToSingle());
      if(5209.0d!=DecimalFraction.FromString("5209").ToDouble())
        Assert.fail("decfrac double 5209\nExpected: 5209.0d\nWas: "+DecimalFraction.FromString("5209").ToDouble());
      if(70489.0f!=DecimalFraction.FromString("70489").ToSingle())
        Assert.fail("decfrac single 70489\nExpected: 70489.0f\nWas: "+DecimalFraction.FromString("70489").ToSingle());
      if(70489.0d!=DecimalFraction.FromString("70489").ToDouble())
        Assert.fail("decfrac double 70489\nExpected: 70489.0d\nWas: "+DecimalFraction.FromString("70489").ToDouble());
      if(-6.1383652E14f!=DecimalFraction.FromString("-613836517344428").ToSingle())
        Assert.fail("decfrac single -613836517344428\nExpected: -6.1383652E14f\nWas: "+DecimalFraction.FromString("-613836517344428").ToSingle());
      if(-6.13836517344428E14d!=DecimalFraction.FromString("-613836517344428").ToDouble())
        Assert.fail("decfrac double -613836517344428\nExpected: -6.13836517344428E14d\nWas: "+DecimalFraction.FromString("-613836517344428").ToDouble());
      if(-3.47896388E16f!=DecimalFraction.FromString("-34789639317051875E0").ToSingle())
        Assert.fail("decfrac single -34789639317051875E0\nExpected: -3.47896388E16f\nWas: "+DecimalFraction.FromString("-34789639317051875E0").ToSingle());
      if(-3.4789639317051876E16d!=DecimalFraction.FromString("-34789639317051875E0").ToDouble())
        Assert.fail("decfrac double -34789639317051875E0\nExpected: -3.4789639317051876E16d\nWas: "+DecimalFraction.FromString("-34789639317051875E0").ToDouble());
      if(-8.4833942E13f!=DecimalFraction.FromString("-84833938642058").ToSingle())
        Assert.fail("decfrac single -84833938642058\nExpected: -8.4833942E13f\nWas: "+DecimalFraction.FromString("-84833938642058").ToSingle());
      if(-8.4833938642058E13d!=DecimalFraction.FromString("-84833938642058").ToDouble())
        Assert.fail("decfrac double -84833938642058\nExpected: -8.4833938642058E13d\nWas: "+DecimalFraction.FromString("-84833938642058").ToDouble());
      if(-359.0f!=DecimalFraction.FromString("-359").ToSingle())
        Assert.fail("decfrac single -359\nExpected: -359.0f\nWas: "+DecimalFraction.FromString("-359").ToSingle());
      if(-359.0d!=DecimalFraction.FromString("-359").ToDouble())
        Assert.fail("decfrac double -359\nExpected: -359.0d\nWas: "+DecimalFraction.FromString("-359").ToDouble());
      if(365981.0f!=DecimalFraction.FromString("365981").ToSingle())
        Assert.fail("decfrac single 365981\nExpected: 365981.0f\nWas: "+DecimalFraction.FromString("365981").ToSingle());
      if(365981.0d!=DecimalFraction.FromString("365981").ToDouble())
        Assert.fail("decfrac double 365981\nExpected: 365981.0d\nWas: "+DecimalFraction.FromString("365981").ToDouble());
      if(9103.0f!=DecimalFraction.FromString("9103").ToSingle())
        Assert.fail("decfrac single 9103\nExpected: 9103.0f\nWas: "+DecimalFraction.FromString("9103").ToSingle());
      if(9103.0d!=DecimalFraction.FromString("9103").ToDouble())
        Assert.fail("decfrac double 9103\nExpected: 9103.0d\nWas: "+DecimalFraction.FromString("9103").ToDouble());
      if(9.822906E11f!=DecimalFraction.FromString("982290625898").ToSingle())
        Assert.fail("decfrac single 982290625898\nExpected: 9.822906E11f\nWas: "+DecimalFraction.FromString("982290625898").ToSingle());
      if(9.82290625898E11d!=DecimalFraction.FromString("982290625898").ToDouble())
        Assert.fail("decfrac double 982290625898\nExpected: 9.82290625898E11d\nWas: "+DecimalFraction.FromString("982290625898").ToDouble());
      if(11.0f!=DecimalFraction.FromString("11").ToSingle())
        Assert.fail("decfrac single 11\nExpected: 11.0f\nWas: "+DecimalFraction.FromString("11").ToSingle());
      if(11.0d!=DecimalFraction.FromString("11").ToDouble())
        Assert.fail("decfrac double 11\nExpected: 11.0d\nWas: "+DecimalFraction.FromString("11").ToDouble());
      if(-2823.0f!=DecimalFraction.FromString("-2823").ToSingle())
        Assert.fail("decfrac single -2823\nExpected: -2823.0f\nWas: "+DecimalFraction.FromString("-2823").ToSingle());
      if(-2823.0d!=DecimalFraction.FromString("-2823").ToDouble())
        Assert.fail("decfrac double -2823\nExpected: -2823.0d\nWas: "+DecimalFraction.FromString("-2823").ToDouble());
      if(1.5945044E10f!=DecimalFraction.FromString("15945044029").ToSingle())
        Assert.fail("decfrac single 15945044029\nExpected: 1.5945044E10f\nWas: "+DecimalFraction.FromString("15945044029").ToSingle());
      if(1.5945044029E10d!=DecimalFraction.FromString("15945044029").ToDouble())
        Assert.fail("decfrac double 15945044029\nExpected: 1.5945044029E10d\nWas: "+DecimalFraction.FromString("15945044029").ToDouble());
      if(-1.69193578E18f!=DecimalFraction.FromString("-1691935711084975329").ToSingle())
        Assert.fail("decfrac single -1691935711084975329\nExpected: -1.69193578E18f\nWas: "+DecimalFraction.FromString("-1691935711084975329").ToSingle());
      if(-1.69193571108497536E18d!=DecimalFraction.FromString("-1691935711084975329").ToDouble())
        Assert.fail("decfrac double -1691935711084975329\nExpected: -1.69193571108497536E18d\nWas: "+DecimalFraction.FromString("-1691935711084975329").ToDouble());
      if(611.0f!=DecimalFraction.FromString("611").ToSingle())
        Assert.fail("decfrac single 611\nExpected: 611.0f\nWas: "+DecimalFraction.FromString("611").ToSingle());
      if(611.0d!=DecimalFraction.FromString("611").ToDouble())
        Assert.fail("decfrac double 611\nExpected: 611.0d\nWas: "+DecimalFraction.FromString("611").ToDouble());
      if(8.1338793E9f!=DecimalFraction.FromString("8133879260").ToSingle())
        Assert.fail("decfrac single 8133879260\nExpected: 8.1338793E9f\nWas: "+DecimalFraction.FromString("8133879260").ToSingle());
      if(8.13387926E9d!=DecimalFraction.FromString("8133879260").ToDouble())
        Assert.fail("decfrac double 8133879260\nExpected: 8.13387926E9d\nWas: "+DecimalFraction.FromString("8133879260").ToDouble());
      if(7.8632614E13f!=DecimalFraction.FromString("78632613962905").ToSingle())
        Assert.fail("decfrac single 78632613962905\nExpected: 7.8632614E13f\nWas: "+DecimalFraction.FromString("78632613962905").ToSingle());
      if(7.8632613962905E13d!=DecimalFraction.FromString("78632613962905").ToDouble())
        Assert.fail("decfrac double 78632613962905\nExpected: 7.8632613962905E13d\nWas: "+DecimalFraction.FromString("78632613962905").ToDouble());
      if(8.686342E19f!=DecimalFraction.FromString("86863421212032782386").ToSingle())
        Assert.fail("decfrac single 86863421212032782386\nExpected: 8.686342E19f\nWas: "+DecimalFraction.FromString("86863421212032782386").ToSingle());
      if(8.686342121203278E19d!=DecimalFraction.FromString("86863421212032782386").ToDouble())
        Assert.fail("decfrac double 86863421212032782386\nExpected: 8.686342121203278E19d\nWas: "+DecimalFraction.FromString("86863421212032782386").ToDouble());
      if(2.46595376E8f!=DecimalFraction.FromString("246595381").ToSingle())
        Assert.fail("decfrac single 246595381\nExpected: 2.46595376E8f\nWas: "+DecimalFraction.FromString("246595381").ToSingle());
      if(2.46595381E8d!=DecimalFraction.FromString("246595381").ToDouble())
        Assert.fail("decfrac double 246595381\nExpected: 2.46595381E8d\nWas: "+DecimalFraction.FromString("246595381").ToDouble());
      if(5.128928E16f!=DecimalFraction.FromString("51289277641921518E0").ToSingle())
        Assert.fail("decfrac single 51289277641921518E0\nExpected: 5.128928E16f\nWas: "+DecimalFraction.FromString("51289277641921518E0").ToSingle());
      if(5.128927764192152E16d!=DecimalFraction.FromString("51289277641921518E0").ToDouble())
        Assert.fail("decfrac double 51289277641921518E0\nExpected: 5.128927764192152E16d\nWas: "+DecimalFraction.FromString("51289277641921518E0").ToDouble());
      if(41105.0f!=DecimalFraction.FromString("41105").ToSingle())
        Assert.fail("decfrac single 41105\nExpected: 41105.0f\nWas: "+DecimalFraction.FromString("41105").ToSingle());
      if(41105.0d!=DecimalFraction.FromString("41105").ToDouble())
        Assert.fail("decfrac double 41105\nExpected: 41105.0d\nWas: "+DecimalFraction.FromString("41105").ToDouble());
      if(4.5854699E16f!=DecimalFraction.FromString("45854697039925162E0").ToSingle())
        Assert.fail("decfrac single 45854697039925162E0\nExpected: 4.5854699E16f\nWas: "+DecimalFraction.FromString("45854697039925162E0").ToSingle());
      if(4.585469703992516E16d!=DecimalFraction.FromString("45854697039925162E0").ToDouble())
        Assert.fail("decfrac double 45854697039925162E0\nExpected: 4.585469703992516E16d\nWas: "+DecimalFraction.FromString("45854697039925162E0").ToDouble());
      if(357.0f!=DecimalFraction.FromString("357").ToSingle())
        Assert.fail("decfrac single 357\nExpected: 357.0f\nWas: "+DecimalFraction.FromString("357").ToSingle());
      if(357.0d!=DecimalFraction.FromString("357").ToDouble())
        Assert.fail("decfrac double 357\nExpected: 357.0d\nWas: "+DecimalFraction.FromString("357").ToDouble());
      if(4055.0f!=DecimalFraction.FromString("4055").ToSingle())
        Assert.fail("decfrac single 4055\nExpected: 4055.0f\nWas: "+DecimalFraction.FromString("4055").ToSingle());
      if(4055.0d!=DecimalFraction.FromString("4055").ToDouble())
        Assert.fail("decfrac double 4055\nExpected: 4055.0d\nWas: "+DecimalFraction.FromString("4055").ToDouble());
      if(-75211.0f!=DecimalFraction.FromString("-75211").ToSingle())
        Assert.fail("decfrac single -75211\nExpected: -75211.0f\nWas: "+DecimalFraction.FromString("-75211").ToSingle());
      if(-75211.0d!=DecimalFraction.FromString("-75211").ToDouble())
        Assert.fail("decfrac double -75211\nExpected: -75211.0d\nWas: "+DecimalFraction.FromString("-75211").ToDouble());
      if(-8.718763E19f!=DecimalFraction.FromString("-87187631416675804676").ToSingle())
        Assert.fail("decfrac single -87187631416675804676\nExpected: -8.718763E19f\nWas: "+DecimalFraction.FromString("-87187631416675804676").ToSingle());
      if(-8.718763141667581E19d!=DecimalFraction.FromString("-87187631416675804676").ToDouble())
        Assert.fail("decfrac double -87187631416675804676\nExpected: -8.718763141667581E19d\nWas: "+DecimalFraction.FromString("-87187631416675804676").ToDouble());
      if(-5.6423271E13f!=DecimalFraction.FromString("-56423269820314").ToSingle())
        Assert.fail("decfrac single -56423269820314\nExpected: -5.6423271E13f\nWas: "+DecimalFraction.FromString("-56423269820314").ToSingle());
      if(-5.6423269820314E13d!=DecimalFraction.FromString("-56423269820314").ToDouble())
        Assert.fail("decfrac double -56423269820314\nExpected: -5.6423269820314E13d\nWas: "+DecimalFraction.FromString("-56423269820314").ToDouble());
      if(-884958.0f!=DecimalFraction.FromString("-884958").ToSingle())
        Assert.fail("decfrac single -884958\nExpected: -884958.0f\nWas: "+DecimalFraction.FromString("-884958").ToSingle());
      if(-884958.0d!=DecimalFraction.FromString("-884958").ToDouble())
        Assert.fail("decfrac double -884958\nExpected: -884958.0d\nWas: "+DecimalFraction.FromString("-884958").ToDouble());
      if(-9.5231607E11f!=DecimalFraction.FromString("-952316071356").ToSingle())
        Assert.fail("decfrac single -952316071356\nExpected: -9.5231607E11f\nWas: "+DecimalFraction.FromString("-952316071356").ToSingle());
      if(-9.52316071356E11d!=DecimalFraction.FromString("-952316071356").ToDouble())
        Assert.fail("decfrac double -952316071356\nExpected: -9.52316071356E11d\nWas: "+DecimalFraction.FromString("-952316071356").ToDouble());
      if(1.07800844E17f!=DecimalFraction.FromString("107800846902684870").ToSingle())
        Assert.fail("decfrac single 107800846902684870\nExpected: 1.07800844E17f\nWas: "+DecimalFraction.FromString("107800846902684870").ToSingle());
      if(1.07800846902684864E17d!=DecimalFraction.FromString("107800846902684870").ToDouble())
        Assert.fail("decfrac double 107800846902684870\nExpected: 1.07800846902684864E17d\nWas: "+DecimalFraction.FromString("107800846902684870").ToDouble());
      if(-8.1588551E18f!=DecimalFraction.FromString("-8158855313340166027").ToSingle())
        Assert.fail("decfrac single -8158855313340166027\nExpected: -8.1588551E18f\nWas: "+DecimalFraction.FromString("-8158855313340166027").ToSingle());
      if(-8.1588553133401661E18d!=DecimalFraction.FromString("-8158855313340166027").ToDouble())
        Assert.fail("decfrac double -8158855313340166027\nExpected: -8.1588553133401661E18d\nWas: "+DecimalFraction.FromString("-8158855313340166027").ToDouble());
      if(1.52743454E18f!=DecimalFraction.FromString("1527434477600178421").ToSingle())
        Assert.fail("decfrac single 1527434477600178421\nExpected: 1.52743454E18f\nWas: "+DecimalFraction.FromString("1527434477600178421").ToSingle());
      if(1.52743447760017843E18d!=DecimalFraction.FromString("1527434477600178421").ToDouble())
        Assert.fail("decfrac double 1527434477600178421\nExpected: 1.52743447760017843E18d\nWas: "+DecimalFraction.FromString("1527434477600178421").ToDouble());
      if(-1.25374015E15f!=DecimalFraction.FromString("-1253740164504924").ToSingle())
        Assert.fail("decfrac single -1253740164504924\nExpected: -1.25374015E15f\nWas: "+DecimalFraction.FromString("-1253740164504924").ToSingle());
      if(-1.253740164504924E15d!=DecimalFraction.FromString("-1253740164504924").ToDouble())
        Assert.fail("decfrac double -1253740164504924\nExpected: -1.253740164504924E15d\nWas: "+DecimalFraction.FromString("-1253740164504924").ToDouble());
      if(9.333153E10f!=DecimalFraction.FromString("93331529453").ToSingle())
        Assert.fail("decfrac single 93331529453\nExpected: 9.333153E10f\nWas: "+DecimalFraction.FromString("93331529453").ToSingle());
      if(9.3331529453E10d!=DecimalFraction.FromString("93331529453").ToDouble())
        Assert.fail("decfrac double 93331529453\nExpected: 9.3331529453E10d\nWas: "+DecimalFraction.FromString("93331529453").ToDouble());
      if(-26195.0f!=DecimalFraction.FromString("-26195").ToSingle())
        Assert.fail("decfrac single -26195\nExpected: -26195.0f\nWas: "+DecimalFraction.FromString("-26195").ToSingle());
      if(-26195.0d!=DecimalFraction.FromString("-26195").ToDouble())
        Assert.fail("decfrac double -26195\nExpected: -26195.0d\nWas: "+DecimalFraction.FromString("-26195").ToDouble());
      if(-369.0f!=DecimalFraction.FromString("-369").ToSingle())
        Assert.fail("decfrac single -369\nExpected: -369.0f\nWas: "+DecimalFraction.FromString("-369").ToSingle());
      if(-369.0d!=DecimalFraction.FromString("-369").ToDouble())
        Assert.fail("decfrac double -369\nExpected: -369.0d\nWas: "+DecimalFraction.FromString("-369").ToDouble());
      if(-831.0f!=DecimalFraction.FromString("-831").ToSingle())
        Assert.fail("decfrac single -831\nExpected: -831.0f\nWas: "+DecimalFraction.FromString("-831").ToSingle());
      if(-831.0d!=DecimalFraction.FromString("-831").ToDouble())
        Assert.fail("decfrac double -831\nExpected: -831.0d\nWas: "+DecimalFraction.FromString("-831").ToDouble());
      if(4.11190218E12f!=DecimalFraction.FromString("4111902130704").ToSingle())
        Assert.fail("decfrac single 4111902130704\nExpected: 4.11190218E12f\nWas: "+DecimalFraction.FromString("4111902130704").ToSingle());
      if(4.111902130704E12d!=DecimalFraction.FromString("4111902130704").ToDouble())
        Assert.fail("decfrac double 4111902130704\nExpected: 4.111902130704E12d\nWas: "+DecimalFraction.FromString("4111902130704").ToDouble());
      if(-7.419975E34f!=DecimalFraction.FromString("-7419975014712636689.1578027201500774E+16").ToSingle())
        Assert.fail("decfrac single -7419975014712636689.1578027201500774E+16\nExpected: -7.419975E34f\nWas: "+DecimalFraction.FromString("-7419975014712636689.1578027201500774E+16").ToSingle());
      if(-7.419975014712636E34d!=DecimalFraction.FromString("-7419975014712636689.1578027201500774E+16").ToDouble())
        Assert.fail("decfrac double -7419975014712636689.1578027201500774E+16\nExpected: -7.419975014712636E34d\nWas: "+DecimalFraction.FromString("-7419975014712636689.1578027201500774E+16").ToDouble());
      if(-1.7915818E7f!=DecimalFraction.FromString("-17915818").ToSingle())
        Assert.fail("decfrac single -17915818\nExpected: -1.7915818E7f\nWas: "+DecimalFraction.FromString("-17915818").ToSingle());
      if(-1.7915818E7d!=DecimalFraction.FromString("-17915818").ToDouble())
        Assert.fail("decfrac double -17915818\nExpected: -1.7915818E7d\nWas: "+DecimalFraction.FromString("-17915818").ToDouble());
      if(-122.0f!=DecimalFraction.FromString("-122").ToSingle())
        Assert.fail("decfrac single -122\nExpected: -122.0f\nWas: "+DecimalFraction.FromString("-122").ToSingle());
      if(-122.0d!=DecimalFraction.FromString("-122").ToDouble())
        Assert.fail("decfrac double -122\nExpected: -122.0d\nWas: "+DecimalFraction.FromString("-122").ToDouble());
      if(-363975.0f!=DecimalFraction.FromString("-363975").ToSingle())
        Assert.fail("decfrac single -363975\nExpected: -363975.0f\nWas: "+DecimalFraction.FromString("-363975").ToSingle());
      if(-363975.0d!=DecimalFraction.FromString("-363975").ToDouble())
        Assert.fail("decfrac double -363975\nExpected: -363975.0d\nWas: "+DecimalFraction.FromString("-363975").ToDouble());
      if(3.22466716E12f!=DecimalFraction.FromString("3224667065103").ToSingle())
        Assert.fail("decfrac single 3224667065103\nExpected: 3.22466716E12f\nWas: "+DecimalFraction.FromString("3224667065103").ToSingle());
      if(3.224667065103E12d!=DecimalFraction.FromString("3224667065103").ToDouble())
        Assert.fail("decfrac double 3224667065103\nExpected: 3.224667065103E12d\nWas: "+DecimalFraction.FromString("3224667065103").ToDouble());
      if(-9.6666224E7f!=DecimalFraction.FromString("-96666228").ToSingle())
        Assert.fail("decfrac single -96666228\nExpected: -9.6666224E7f\nWas: "+DecimalFraction.FromString("-96666228").ToSingle());
      if(-9.6666228E7d!=DecimalFraction.FromString("-96666228").ToDouble())
        Assert.fail("decfrac double -96666228\nExpected: -9.6666228E7d\nWas: "+DecimalFraction.FromString("-96666228").ToDouble());
      if(-6.3737765E19f!=DecimalFraction.FromString("-63737764614634686933").ToSingle())
        Assert.fail("decfrac single -63737764614634686933\nExpected: -6.3737765E19f\nWas: "+DecimalFraction.FromString("-63737764614634686933").ToSingle());
      if(-6.3737764614634684E19d!=DecimalFraction.FromString("-63737764614634686933").ToDouble())
        Assert.fail("decfrac double -63737764614634686933\nExpected: -6.3737764614634684E19d\nWas: "+DecimalFraction.FromString("-63737764614634686933").ToDouble());
      if(-45065.0f!=DecimalFraction.FromString("-45065").ToSingle())
        Assert.fail("decfrac single -45065\nExpected: -45065.0f\nWas: "+DecimalFraction.FromString("-45065").ToSingle());
      if(-45065.0d!=DecimalFraction.FromString("-45065").ToDouble())
        Assert.fail("decfrac double -45065\nExpected: -45065.0d\nWas: "+DecimalFraction.FromString("-45065").ToDouble());
      if(18463.0f!=DecimalFraction.FromString("18463").ToSingle())
        Assert.fail("decfrac single 18463\nExpected: 18463.0f\nWas: "+DecimalFraction.FromString("18463").ToSingle());
      if(18463.0d!=DecimalFraction.FromString("18463").ToDouble())
        Assert.fail("decfrac double 18463\nExpected: 18463.0d\nWas: "+DecimalFraction.FromString("18463").ToDouble());
      if(-5.2669409E15f!=DecimalFraction.FromString("-5266940927335870").ToSingle())
        Assert.fail("decfrac single -5266940927335870\nExpected: -5.2669409E15f\nWas: "+DecimalFraction.FromString("-5266940927335870").ToSingle());
      if(-5.26694092733587E15d!=DecimalFraction.FromString("-5266940927335870").ToDouble())
        Assert.fail("decfrac double -5266940927335870\nExpected: -5.26694092733587E15d\nWas: "+DecimalFraction.FromString("-5266940927335870").ToDouble());
      if(-3.61275925E15f!=DecimalFraction.FromString("-3612759343074710").ToSingle())
        Assert.fail("decfrac single -3612759343074710\nExpected: -3.61275925E15f\nWas: "+DecimalFraction.FromString("-3612759343074710").ToSingle());
      if(-3.61275934307471E15d!=DecimalFraction.FromString("-3612759343074710").ToDouble())
        Assert.fail("decfrac double -3612759343074710\nExpected: -3.61275934307471E15d\nWas: "+DecimalFraction.FromString("-3612759343074710").ToDouble());
      if(-1.49784412E11f!=DecimalFraction.FromString("-149784410976").ToSingle())
        Assert.fail("decfrac single -149784410976\nExpected: -1.49784412E11f\nWas: "+DecimalFraction.FromString("-149784410976").ToSingle());
      if(-1.49784410976E11d!=DecimalFraction.FromString("-149784410976").ToDouble())
        Assert.fail("decfrac double -149784410976\nExpected: -1.49784410976E11d\nWas: "+DecimalFraction.FromString("-149784410976").ToDouble());
      if(-1.01285276E17f!=DecimalFraction.FromString("-101285275020696035").ToSingle())
        Assert.fail("decfrac single -101285275020696035\nExpected: -1.01285276E17f\nWas: "+DecimalFraction.FromString("-101285275020696035").ToSingle());
      if(-1.01285275020696032E17d!=DecimalFraction.FromString("-101285275020696035").ToDouble())
        Assert.fail("decfrac double -101285275020696035\nExpected: -1.01285275020696032E17d\nWas: "+DecimalFraction.FromString("-101285275020696035").ToDouble());
      if(-34.0f!=DecimalFraction.FromString("-34").ToSingle())
        Assert.fail("decfrac single -34\nExpected: -34.0f\nWas: "+DecimalFraction.FromString("-34").ToSingle());
      if(-34.0d!=DecimalFraction.FromString("-34").ToDouble())
        Assert.fail("decfrac double -34\nExpected: -34.0d\nWas: "+DecimalFraction.FromString("-34").ToDouble());
      if(-6.9963739E17f!=DecimalFraction.FromString("-699637360432542026").ToSingle())
        Assert.fail("decfrac single -699637360432542026\nExpected: -6.9963739E17f\nWas: "+DecimalFraction.FromString("-699637360432542026").ToSingle());
      if(-6.9963736043254208E17d!=DecimalFraction.FromString("-699637360432542026").ToDouble())
        Assert.fail("decfrac double -699637360432542026\nExpected: -6.9963736043254208E17d\nWas: "+DecimalFraction.FromString("-699637360432542026").ToDouble());
      if(-8131.0f!=DecimalFraction.FromString("-8131").ToSingle())
        Assert.fail("decfrac single -8131\nExpected: -8131.0f\nWas: "+DecimalFraction.FromString("-8131").ToSingle());
      if(-8131.0d!=DecimalFraction.FromString("-8131").ToDouble())
        Assert.fail("decfrac double -8131\nExpected: -8131.0d\nWas: "+DecimalFraction.FromString("-8131").ToDouble());
      if(6.1692147E8f!=DecimalFraction.FromString("616921472").ToSingle())
        Assert.fail("decfrac single 616921472\nExpected: 6.1692147E8f\nWas: "+DecimalFraction.FromString("616921472").ToSingle());
      if(6.16921472E8d!=DecimalFraction.FromString("616921472").ToDouble())
        Assert.fail("decfrac double 616921472\nExpected: 6.16921472E8d\nWas: "+DecimalFraction.FromString("616921472").ToDouble());
      if(447272.0f!=DecimalFraction.FromString("447272").ToSingle())
        Assert.fail("decfrac single 447272\nExpected: 447272.0f\nWas: "+DecimalFraction.FromString("447272").ToSingle());
      if(447272.0d!=DecimalFraction.FromString("447272").ToDouble())
        Assert.fail("decfrac double 447272\nExpected: 447272.0d\nWas: "+DecimalFraction.FromString("447272").ToDouble());
      if(9.719524E17f!=DecimalFraction.FromString("971952376640924713").ToSingle())
        Assert.fail("decfrac single 971952376640924713\nExpected: 9.719524E17f\nWas: "+DecimalFraction.FromString("971952376640924713").ToSingle());
      if(9.7195237664092467E17d!=DecimalFraction.FromString("971952376640924713").ToDouble())
        Assert.fail("decfrac double 971952376640924713\nExpected: 9.7195237664092467E17d\nWas: "+DecimalFraction.FromString("971952376640924713").ToDouble());
      if(-8622.0f!=DecimalFraction.FromString("-8622").ToSingle())
        Assert.fail("decfrac single -8622\nExpected: -8622.0f\nWas: "+DecimalFraction.FromString("-8622").ToSingle());
      if(-8622.0d!=DecimalFraction.FromString("-8622").ToDouble())
        Assert.fail("decfrac double -8622\nExpected: -8622.0d\nWas: "+DecimalFraction.FromString("-8622").ToDouble());
      if(-9.8425534E13f!=DecimalFraction.FromString("-98425536547570").ToSingle())
        Assert.fail("decfrac single -98425536547570\nExpected: -9.8425534E13f\nWas: "+DecimalFraction.FromString("-98425536547570").ToSingle());
      if(-9.842553654757E13d!=DecimalFraction.FromString("-98425536547570").ToDouble())
        Assert.fail("decfrac double -98425536547570\nExpected: -9.842553654757E13d\nWas: "+DecimalFraction.FromString("-98425536547570").ToDouble());
      if(-1.3578545E14f!=DecimalFraction.FromString("-135785450228746").ToSingle())
        Assert.fail("decfrac single -135785450228746\nExpected: -1.3578545E14f\nWas: "+DecimalFraction.FromString("-135785450228746").ToSingle());
      if(-1.35785450228746E14d!=DecimalFraction.FromString("-135785450228746").ToDouble())
        Assert.fail("decfrac double -135785450228746\nExpected: -1.35785450228746E14d\nWas: "+DecimalFraction.FromString("-135785450228746").ToDouble());
      if(935.0f!=DecimalFraction.FromString("935").ToSingle())
        Assert.fail("decfrac single 935\nExpected: 935.0f\nWas: "+DecimalFraction.FromString("935").ToSingle());
      if(935.0d!=DecimalFraction.FromString("935").ToDouble())
        Assert.fail("decfrac double 935\nExpected: 935.0d\nWas: "+DecimalFraction.FromString("935").ToDouble());
      if(-7890.0f!=DecimalFraction.FromString("-7890E0").ToSingle())
        Assert.fail("decfrac single -7890E0\nExpected: -7890.0f\nWas: "+DecimalFraction.FromString("-7890E0").ToSingle());
      if(-7890.0d!=DecimalFraction.FromString("-7890E0").ToDouble())
        Assert.fail("decfrac double -7890E0\nExpected: -7890.0d\nWas: "+DecimalFraction.FromString("-7890E0").ToDouble());
      if(4.5492643E12f!=DecimalFraction.FromString("45.49264316782E+11").ToSingle())
        Assert.fail("decfrac single 45.49264316782E+11\nExpected: 4.5492643E12f\nWas: "+DecimalFraction.FromString("45.49264316782E+11").ToSingle());
      if(4.549264316782E12d!=DecimalFraction.FromString("45.49264316782E+11").ToDouble())
        Assert.fail("decfrac double 45.49264316782E+11\nExpected: 4.549264316782E12d\nWas: "+DecimalFraction.FromString("45.49264316782E+11").ToDouble());
      if(-7684.0f!=DecimalFraction.FromString("-7684").ToSingle())
        Assert.fail("decfrac single -7684\nExpected: -7684.0f\nWas: "+DecimalFraction.FromString("-7684").ToSingle());
      if(-7684.0d!=DecimalFraction.FromString("-7684").ToDouble())
        Assert.fail("decfrac double -7684\nExpected: -7684.0d\nWas: "+DecimalFraction.FromString("-7684").ToDouble());
      if(734069.0f!=DecimalFraction.FromString("734069").ToSingle())
        Assert.fail("decfrac single 734069\nExpected: 734069.0f\nWas: "+DecimalFraction.FromString("734069").ToSingle());
      if(734069.0d!=DecimalFraction.FromString("734069").ToDouble())
        Assert.fail("decfrac double 734069\nExpected: 734069.0d\nWas: "+DecimalFraction.FromString("734069").ToDouble());
      if(-3.51801573E12f!=DecimalFraction.FromString("-3518015796477").ToSingle())
        Assert.fail("decfrac single -3518015796477\nExpected: -3.51801573E12f\nWas: "+DecimalFraction.FromString("-3518015796477").ToSingle());
      if(-3.518015796477E12d!=DecimalFraction.FromString("-3518015796477").ToDouble())
        Assert.fail("decfrac double -3518015796477\nExpected: -3.518015796477E12d\nWas: "+DecimalFraction.FromString("-3518015796477").ToDouble());
      if(-411720.0f!=DecimalFraction.FromString("-411720").ToSingle())
        Assert.fail("decfrac single -411720\nExpected: -411720.0f\nWas: "+DecimalFraction.FromString("-411720").ToSingle());
      if(-411720.0d!=DecimalFraction.FromString("-411720").ToDouble())
        Assert.fail("decfrac double -411720\nExpected: -411720.0d\nWas: "+DecimalFraction.FromString("-411720").ToDouble());
      if(5.14432512E8f!=DecimalFraction.FromString("514432504").ToSingle())
        Assert.fail("decfrac single 514432504\nExpected: 5.14432512E8f\nWas: "+DecimalFraction.FromString("514432504").ToSingle());
      if(5.14432504E8d!=DecimalFraction.FromString("514432504").ToDouble())
        Assert.fail("decfrac double 514432504\nExpected: 5.14432504E8d\nWas: "+DecimalFraction.FromString("514432504").ToDouble());
      if(3970.0f!=DecimalFraction.FromString("3970").ToSingle())
        Assert.fail("decfrac single 3970\nExpected: 3970.0f\nWas: "+DecimalFraction.FromString("3970").ToSingle());
      if(3970.0d!=DecimalFraction.FromString("3970").ToDouble())
        Assert.fail("decfrac double 3970\nExpected: 3970.0d\nWas: "+DecimalFraction.FromString("3970").ToDouble());
      if(-1.89642527E10f!=DecimalFraction.FromString("-18964252847").ToSingle())
        Assert.fail("decfrac single -18964252847\nExpected: -1.89642527E10f\nWas: "+DecimalFraction.FromString("-18964252847").ToSingle());
      if(-1.8964252847E10d!=DecimalFraction.FromString("-18964252847").ToDouble())
        Assert.fail("decfrac double -18964252847\nExpected: -1.8964252847E10d\nWas: "+DecimalFraction.FromString("-18964252847").ToDouble());
      if(-9.5766118E10f!=DecimalFraction.FromString("-95766116842").ToSingle())
        Assert.fail("decfrac single -95766116842\nExpected: -9.5766118E10f\nWas: "+DecimalFraction.FromString("-95766116842").ToSingle());
      if(-9.5766116842E10d!=DecimalFraction.FromString("-95766116842").ToDouble())
        Assert.fail("decfrac double -95766116842\nExpected: -9.5766116842E10d\nWas: "+DecimalFraction.FromString("-95766116842").ToDouble());
      if(-4.5759559E15f!=DecimalFraction.FromString("-4575956051893063").ToSingle())
        Assert.fail("decfrac single -4575956051893063\nExpected: -4.5759559E15f\nWas: "+DecimalFraction.FromString("-4575956051893063").ToSingle());
      if(-4.575956051893063E15d!=DecimalFraction.FromString("-4575956051893063").ToDouble())
        Assert.fail("decfrac double -4575956051893063\nExpected: -4.575956051893063E15d\nWas: "+DecimalFraction.FromString("-4575956051893063").ToDouble());
      if(5.2050934E9f!=DecimalFraction.FromString("5205093392").ToSingle())
        Assert.fail("decfrac single 5205093392\nExpected: 5.2050934E9f\nWas: "+DecimalFraction.FromString("5205093392").ToSingle());
      if(5.205093392E9d!=DecimalFraction.FromString("5205093392").ToDouble())
        Assert.fail("decfrac double 5205093392\nExpected: 5.205093392E9d\nWas: "+DecimalFraction.FromString("5205093392").ToDouble());
      if(-7.0079627E12f!=DecimalFraction.FromString("-7007962583042").ToSingle())
        Assert.fail("decfrac single -7007962583042\nExpected: -7.0079627E12f\nWas: "+DecimalFraction.FromString("-7007962583042").ToSingle());
      if(-7.007962583042E12d!=DecimalFraction.FromString("-7007962583042").ToDouble())
        Assert.fail("decfrac double -7007962583042\nExpected: -7.007962583042E12d\nWas: "+DecimalFraction.FromString("-7007962583042").ToDouble());
      if(59.0f!=DecimalFraction.FromString("59").ToSingle())
        Assert.fail("decfrac single 59\nExpected: 59.0f\nWas: "+DecimalFraction.FromString("59").ToSingle());
      if(59.0d!=DecimalFraction.FromString("59").ToDouble())
        Assert.fail("decfrac double 59\nExpected: 59.0d\nWas: "+DecimalFraction.FromString("59").ToDouble());
      if(-5.5095849E16f!=DecimalFraction.FromString("-55095850956259910").ToSingle())
        Assert.fail("decfrac single -55095850956259910\nExpected: -5.5095849E16f\nWas: "+DecimalFraction.FromString("-55095850956259910").ToSingle());
      if(-5.5095850956259912E16d!=DecimalFraction.FromString("-55095850956259910").ToDouble())
        Assert.fail("decfrac double -55095850956259910\nExpected: -5.5095850956259912E16d\nWas: "+DecimalFraction.FromString("-55095850956259910").ToDouble());
      if(1.0f!=DecimalFraction.FromString("1").ToSingle())
        Assert.fail("decfrac single 1\nExpected: 1.0f\nWas: "+DecimalFraction.FromString("1").ToSingle());
      if(1.0d!=DecimalFraction.FromString("1").ToDouble())
        Assert.fail("decfrac double 1\nExpected: 1.0d\nWas: "+DecimalFraction.FromString("1").ToDouble());
      if(598.0f!=DecimalFraction.FromString("598").ToSingle())
        Assert.fail("decfrac single 598\nExpected: 598.0f\nWas: "+DecimalFraction.FromString("598").ToSingle());
      if(598.0d!=DecimalFraction.FromString("598").ToDouble())
        Assert.fail("decfrac double 598\nExpected: 598.0d\nWas: "+DecimalFraction.FromString("598").ToDouble());
      if(957.0f!=DecimalFraction.FromString("957").ToSingle())
        Assert.fail("decfrac single 957\nExpected: 957.0f\nWas: "+DecimalFraction.FromString("957").ToSingle());
      if(957.0d!=DecimalFraction.FromString("957").ToDouble())
        Assert.fail("decfrac double 957\nExpected: 957.0d\nWas: "+DecimalFraction.FromString("957").ToDouble());
      if(-1.4772274E7f!=DecimalFraction.FromString("-14772274").ToSingle())
        Assert.fail("decfrac single -14772274\nExpected: -1.4772274E7f\nWas: "+DecimalFraction.FromString("-14772274").ToSingle());
      if(-1.4772274E7d!=DecimalFraction.FromString("-14772274").ToDouble())
        Assert.fail("decfrac double -14772274\nExpected: -1.4772274E7d\nWas: "+DecimalFraction.FromString("-14772274").ToDouble());
      if(-3006.0f!=DecimalFraction.FromString("-3006").ToSingle())
        Assert.fail("decfrac single -3006\nExpected: -3006.0f\nWas: "+DecimalFraction.FromString("-3006").ToSingle());
      if(-3006.0d!=DecimalFraction.FromString("-3006").ToDouble())
        Assert.fail("decfrac double -3006\nExpected: -3006.0d\nWas: "+DecimalFraction.FromString("-3006").ToDouble());
      if(3.07120343E18f!=DecimalFraction.FromString("3071203450148698328").ToSingle())
        Assert.fail("decfrac single 3071203450148698328\nExpected: 3.07120343E18f\nWas: "+DecimalFraction.FromString("3071203450148698328").ToSingle());
      if(3.0712034501486981E18d!=DecimalFraction.FromString("3071203450148698328").ToDouble())
        Assert.fail("decfrac double 3071203450148698328\nExpected: 3.0712034501486981E18d\nWas: "+DecimalFraction.FromString("3071203450148698328").ToDouble());
    }
    
    /**
     * 
     */
@Test
    public void TestDecFracToSingleDouble() {
      if(-4348.0f!=DecimalFraction.FromString("-4348").ToSingle())
        Assert.fail("decfrac single -4348\nExpected: -4348.0f\nWas: "+DecimalFraction.FromString("-4348").ToSingle());
      if(-4348.0d!=DecimalFraction.FromString("-4348").ToDouble())
        Assert.fail("decfrac double -4348\nExpected: -4348.0d\nWas: "+DecimalFraction.FromString("-4348").ToDouble());
      if(-9.85323f!=DecimalFraction.FromString("-9.85323086293411065").ToSingle())
        Assert.fail("decfrac single -9.85323086293411065\nExpected: -9.85323f\nWas: "+DecimalFraction.FromString("-9.85323086293411065").ToSingle());
      if(-9.85323086293411d!=DecimalFraction.FromString("-9.85323086293411065").ToDouble())
        Assert.fail("decfrac double -9.85323086293411065\nExpected: -9.85323086293411d\nWas: "+DecimalFraction.FromString("-9.85323086293411065").ToDouble());
      if(-5.2317E9f!=DecimalFraction.FromString("-5231.7E+6").ToSingle())
        Assert.fail("decfrac single -5231.7E+6\nExpected: -5.2317E9f\nWas: "+DecimalFraction.FromString("-5231.7E+6").ToSingle());
      if(-5.2317E9d!=DecimalFraction.FromString("-5231.7E+6").ToDouble())
        Assert.fail("decfrac double -5231.7E+6\nExpected: -5.2317E9d\nWas: "+DecimalFraction.FromString("-5231.7E+6").ToDouble());
      if(5.7991604E7f!=DecimalFraction.FromString("579916024.449917729730457E-1").ToSingle())
        Assert.fail("decfrac single 579916024.449917729730457E-1\nExpected: 5.7991604E7f\nWas: "+DecimalFraction.FromString("579916024.449917729730457E-1").ToSingle());
      if(5.7991602444991775E7d!=DecimalFraction.FromString("579916024.449917729730457E-1").ToDouble())
        Assert.fail("decfrac double 579916024.449917729730457E-1\nExpected: 5.7991602444991775E7d\nWas: "+DecimalFraction.FromString("579916024.449917729730457E-1").ToDouble());
      if(-515.02563f!=DecimalFraction.FromString("-515025607547098618E-15").ToSingle())
        Assert.fail("decfrac single -515025607547098618E-15\nExpected: -515.02563f\nWas: "+DecimalFraction.FromString("-515025607547098618E-15").ToSingle());
      if(-515.0256075470986d!=DecimalFraction.FromString("-515025607547098618E-15").ToDouble())
        Assert.fail("decfrac double -515025607547098618E-15\nExpected: -515.0256075470986d\nWas: "+DecimalFraction.FromString("-515025607547098618E-15").ToDouble());
      if(-9.3541843E10f!=DecimalFraction.FromString("-93541840706").ToSingle())
        Assert.fail("decfrac single -93541840706\nExpected: -9.3541843E10f\nWas: "+DecimalFraction.FromString("-93541840706").ToSingle());
      if(-9.3541840706E10d!=DecimalFraction.FromString("-93541840706").ToDouble())
        Assert.fail("decfrac double -93541840706\nExpected: -9.3541840706E10d\nWas: "+DecimalFraction.FromString("-93541840706").ToDouble());
      if(3.8568078E23f!=DecimalFraction.FromString("38568076767380659.6E+7").ToSingle())
        Assert.fail("decfrac single 38568076767380659.6E+7\nExpected: 3.8568078E23f\nWas: "+DecimalFraction.FromString("38568076767380659.6E+7").ToSingle());
      if(3.8568076767380657E23d!=DecimalFraction.FromString("38568076767380659.6E+7").ToDouble())
        Assert.fail("decfrac double 38568076767380659.6E+7\nExpected: 3.8568076767380657E23d\nWas: "+DecimalFraction.FromString("38568076767380659.6E+7").ToDouble());
      if(4682.1987f!=DecimalFraction.FromString("468219867826E-8").ToSingle())
        Assert.fail("decfrac single 468219867826E-8\nExpected: 4682.1987f\nWas: "+DecimalFraction.FromString("468219867826E-8").ToSingle());
      if(4682.19867826d!=DecimalFraction.FromString("468219867826E-8").ToDouble())
        Assert.fail("decfrac double 468219867826E-8\nExpected: 4682.19867826d\nWas: "+DecimalFraction.FromString("468219867826E-8").ToDouble());
      if(7.3869363E-4f!=DecimalFraction.FromString("73869365.3859328709200790828E-11").ToSingle())
        Assert.fail("decfrac single 73869365.3859328709200790828E-11\nExpected: 7.3869363E-4f\nWas: "+DecimalFraction.FromString("73869365.3859328709200790828E-11").ToSingle());
      if(7.386936538593287E-4d!=DecimalFraction.FromString("73869365.3859328709200790828E-11").ToDouble())
        Assert.fail("decfrac double 73869365.3859328709200790828E-11\nExpected: 7.386936538593287E-4d\nWas: "+DecimalFraction.FromString("73869365.3859328709200790828E-11").ToDouble());
      if(2.3f!=DecimalFraction.FromString("2.3E0").ToSingle())
        Assert.fail("decfrac single 2.3E0\nExpected: 2.3f\nWas: "+DecimalFraction.FromString("2.3E0").ToSingle());
      if(2.3d!=DecimalFraction.FromString("2.3E0").ToDouble())
        Assert.fail("decfrac double 2.3E0\nExpected: 2.3d\nWas: "+DecimalFraction.FromString("2.3E0").ToDouble());
      if(3.3713182E15f!=DecimalFraction.FromString("3371318258253373.59498533176159560").ToSingle())
        Assert.fail("decfrac single 3371318258253373.59498533176159560\nExpected: 3.3713182E15f\nWas: "+DecimalFraction.FromString("3371318258253373.59498533176159560").ToSingle());
      if(3.3713182582533735E15d!=DecimalFraction.FromString("3371318258253373.59498533176159560").ToDouble())
        Assert.fail("decfrac double 3371318258253373.59498533176159560\nExpected: 3.3713182582533735E15d\nWas: "+DecimalFraction.FromString("3371318258253373.59498533176159560").ToDouble());
      if(0.08044683f!=DecimalFraction.FromString("804468350612974.6118902086132089233E-16").ToSingle())
        Assert.fail("decfrac single 804468350612974.6118902086132089233E-16\nExpected: 0.08044683f\nWas: "+DecimalFraction.FromString("804468350612974.6118902086132089233E-16").ToSingle());
      if(0.08044683506129746d!=DecimalFraction.FromString("804468350612974.6118902086132089233E-16").ToDouble())
        Assert.fail("decfrac double 804468350612974.6118902086132089233E-16\nExpected: 0.08044683506129746d\nWas: "+DecimalFraction.FromString("804468350612974.6118902086132089233E-16").ToDouble());
      if(-7.222071E19f!=DecimalFraction.FromString("-72220708347127407337.28").ToSingle())
        Assert.fail("decfrac single -72220708347127407337.28\nExpected: -7.222071E19f\nWas: "+DecimalFraction.FromString("-72220708347127407337.28").ToSingle());
      if(-7.222070834712741E19d!=DecimalFraction.FromString("-72220708347127407337.28").ToDouble())
        Assert.fail("decfrac double -72220708347127407337.28\nExpected: -7.222070834712741E19d\nWas: "+DecimalFraction.FromString("-72220708347127407337.28").ToDouble());
      if(9715796.0f!=DecimalFraction.FromString("9715796.4299331966870989").ToSingle())
        Assert.fail("decfrac single 9715796.4299331966870989\nExpected: 9715796.0f\nWas: "+DecimalFraction.FromString("9715796.4299331966870989").ToSingle());
      if(9715796.429933196d!=DecimalFraction.FromString("9715796.4299331966870989").ToDouble())
        Assert.fail("decfrac double 9715796.4299331966870989\nExpected: 9715796.429933196d\nWas: "+DecimalFraction.FromString("9715796.4299331966870989").ToDouble());
      if(9.3596612E14f!=DecimalFraction.FromString("93596609961883873.8463754373628236E-2").ToSingle())
        Assert.fail("decfrac single 93596609961883873.8463754373628236E-2\nExpected: 9.3596612E14f\nWas: "+DecimalFraction.FromString("93596609961883873.8463754373628236E-2").ToSingle());
      if(9.359660996188388E14d!=DecimalFraction.FromString("93596609961883873.8463754373628236E-2").ToDouble())
        Assert.fail("decfrac double 93596609961883873.8463754373628236E-2\nExpected: 9.359660996188388E14d\nWas: "+DecimalFraction.FromString("93596609961883873.8463754373628236E-2").ToDouble());
      if(4.82799354E14f!=DecimalFraction.FromString("482799357899450").ToSingle())
        Assert.fail("decfrac single 482799357899450\nExpected: 4.82799354E14f\nWas: "+DecimalFraction.FromString("482799357899450").ToSingle());
      if(4.8279935789945E14d!=DecimalFraction.FromString("482799357899450").ToDouble())
        Assert.fail("decfrac double 482799357899450\nExpected: 4.8279935789945E14d\nWas: "+DecimalFraction.FromString("482799357899450").ToDouble());
      if(3.8193924E25f!=DecimalFraction.FromString("381939236989E+14").ToSingle())
        Assert.fail("decfrac single 381939236989E+14\nExpected: 3.8193924E25f\nWas: "+DecimalFraction.FromString("381939236989E+14").ToSingle());
      if(3.81939236989E25d!=DecimalFraction.FromString("381939236989E+14").ToDouble())
        Assert.fail("decfrac double 381939236989E+14\nExpected: 3.81939236989E25d\nWas: "+DecimalFraction.FromString("381939236989E+14").ToDouble());
      if(-3.1092332E27f!=DecimalFraction.FromString("-3109233371824024E+12").ToSingle())
        Assert.fail("decfrac single -3109233371824024E+12\nExpected: -3.1092332E27f\nWas: "+DecimalFraction.FromString("-3109233371824024E+12").ToSingle());
      if(-3.109233371824024E27d!=DecimalFraction.FromString("-3109233371824024E+12").ToDouble())
        Assert.fail("decfrac double -3109233371824024E+12\nExpected: -3.109233371824024E27d\nWas: "+DecimalFraction.FromString("-3109233371824024E+12").ToDouble());
      if(-0.006658507f!=DecimalFraction.FromString("-66585.07E-7").ToSingle())
        Assert.fail("decfrac single -66585.07E-7\nExpected: -0.006658507f\nWas: "+DecimalFraction.FromString("-66585.07E-7").ToSingle());
      if(-0.006658507d!=DecimalFraction.FromString("-66585.07E-7").ToDouble())
        Assert.fail("decfrac double -66585.07E-7\nExpected: -0.006658507d\nWas: "+DecimalFraction.FromString("-66585.07E-7").ToDouble());
      if(17.276796f!=DecimalFraction.FromString("17.276795549708").ToSingle())
        Assert.fail("decfrac single 17.276795549708\nExpected: 17.276796f\nWas: "+DecimalFraction.FromString("17.276795549708").ToSingle());
      if(17.276795549708d!=DecimalFraction.FromString("17.276795549708").ToDouble())
        Assert.fail("decfrac double 17.276795549708\nExpected: 17.276795549708d\nWas: "+DecimalFraction.FromString("17.276795549708").ToDouble());
      if(-3210939.5f!=DecimalFraction.FromString("-321093943510192.3307E-8").ToSingle())
        Assert.fail("decfrac single -321093943510192.3307E-8\nExpected: -3210939.5f\nWas: "+DecimalFraction.FromString("-321093943510192.3307E-8").ToSingle());
      if(-3210939.4351019235d!=DecimalFraction.FromString("-321093943510192.3307E-8").ToDouble())
        Assert.fail("decfrac double -321093943510192.3307E-8\nExpected: -3210939.4351019235d\nWas: "+DecimalFraction.FromString("-321093943510192.3307E-8").ToDouble());
      if(-976.9676f!=DecimalFraction.FromString("-976.967597776185553735").ToSingle())
        Assert.fail("decfrac single -976.967597776185553735\nExpected: -976.9676f\nWas: "+DecimalFraction.FromString("-976.967597776185553735").ToSingle());
      if(-976.9675977761856d!=DecimalFraction.FromString("-976.967597776185553735").ToDouble())
        Assert.fail("decfrac double -976.967597776185553735\nExpected: -976.9675977761856d\nWas: "+DecimalFraction.FromString("-976.967597776185553735").ToDouble());
      if(-3.49712614E9f!=DecimalFraction.FromString("-3497126138").ToSingle())
        Assert.fail("decfrac single -3497126138\nExpected: -3.49712614E9f\nWas: "+DecimalFraction.FromString("-3497126138").ToSingle());
      if(-3.497126138E9d!=DecimalFraction.FromString("-3497126138").ToDouble())
        Assert.fail("decfrac double -3497126138\nExpected: -3.497126138E9d\nWas: "+DecimalFraction.FromString("-3497126138").ToDouble());
      if(-2.63418028E14f!=DecimalFraction.FromString("-2634180.2455697965376217503E+8").ToSingle())
        Assert.fail("decfrac single -2634180.2455697965376217503E+8\nExpected: -2.63418028E14f\nWas: "+DecimalFraction.FromString("-2634180.2455697965376217503E+8").ToSingle());
      if(-2.6341802455697966E14d!=DecimalFraction.FromString("-2634180.2455697965376217503E+8").ToDouble())
        Assert.fail("decfrac double -2634180.2455697965376217503E+8\nExpected: -2.6341802455697966E14d\nWas: "+DecimalFraction.FromString("-2634180.2455697965376217503E+8").ToDouble());
      if(3.25314253E10f!=DecimalFraction.FromString("32531426161").ToSingle())
        Assert.fail("decfrac single 32531426161\nExpected: 3.25314253E10f\nWas: "+DecimalFraction.FromString("32531426161").ToSingle());
      if(3.2531426161E10d!=DecimalFraction.FromString("32531426161").ToDouble())
        Assert.fail("decfrac double 32531426161\nExpected: 3.2531426161E10d\nWas: "+DecimalFraction.FromString("32531426161").ToDouble());
      if(-83825.7f!=DecimalFraction.FromString("-83825.7").ToSingle())
        Assert.fail("decfrac single -83825.7\nExpected: -83825.7f\nWas: "+DecimalFraction.FromString("-83825.7").ToSingle());
      if(-83825.7d!=DecimalFraction.FromString("-83825.7").ToDouble())
        Assert.fail("decfrac double -83825.7\nExpected: -83825.7d\nWas: "+DecimalFraction.FromString("-83825.7").ToDouble());
      if(9347.0f!=DecimalFraction.FromString("9347").ToSingle())
        Assert.fail("decfrac single 9347\nExpected: 9347.0f\nWas: "+DecimalFraction.FromString("9347").ToSingle());
      if(9347.0d!=DecimalFraction.FromString("9347").ToDouble())
        Assert.fail("decfrac double 9347\nExpected: 9347.0d\nWas: "+DecimalFraction.FromString("9347").ToDouble());
      if(4039.426f!=DecimalFraction.FromString("403942604431E-8").ToSingle())
        Assert.fail("decfrac single 403942604431E-8\nExpected: 4039.426f\nWas: "+DecimalFraction.FromString("403942604431E-8").ToSingle());
      if(4039.42604431d!=DecimalFraction.FromString("403942604431E-8").ToDouble())
        Assert.fail("decfrac double 403942604431E-8\nExpected: 4039.42604431d\nWas: "+DecimalFraction.FromString("403942604431E-8").ToDouble());
      if(9.821772E-8f!=DecimalFraction.FromString("9821771729.481512E-17").ToSingle())
        Assert.fail("decfrac single 9821771729.481512E-17\nExpected: 9.821772E-8f\nWas: "+DecimalFraction.FromString("9821771729.481512E-17").ToSingle());
      if(9.821771729481512E-8d!=DecimalFraction.FromString("9821771729.481512E-17").ToDouble())
        Assert.fail("decfrac double 9821771729.481512E-17\nExpected: 9.821771729481512E-8d\nWas: "+DecimalFraction.FromString("9821771729.481512E-17").ToDouble());
      if(1.47027E24f!=DecimalFraction.FromString("1470270E+18").ToSingle())
        Assert.fail("decfrac single 1470270E+18\nExpected: 1.47027E24f\nWas: "+DecimalFraction.FromString("1470270E+18").ToSingle());
      if(1.47027E24d!=DecimalFraction.FromString("1470270E+18").ToDouble())
        Assert.fail("decfrac double 1470270E+18\nExpected: 1.47027E24d\nWas: "+DecimalFraction.FromString("1470270E+18").ToDouble());
      if(504.07468f!=DecimalFraction.FromString("504.074687047275").ToSingle())
        Assert.fail("decfrac single 504.074687047275\nExpected: 504.07468f\nWas: "+DecimalFraction.FromString("504.074687047275").ToSingle());
      if(504.074687047275d!=DecimalFraction.FromString("504.074687047275").ToDouble())
        Assert.fail("decfrac double 504.074687047275\nExpected: 504.074687047275d\nWas: "+DecimalFraction.FromString("504.074687047275").ToDouble());
      if(8.051101E-11f!=DecimalFraction.FromString("8051.10083245768396604E-14").ToSingle())
        Assert.fail("decfrac single 8051.10083245768396604E-14\nExpected: 8.051101E-11f\nWas: "+DecimalFraction.FromString("8051.10083245768396604E-14").ToSingle());
      if(8.051100832457683E-11d!=DecimalFraction.FromString("8051.10083245768396604E-14").ToDouble())
        Assert.fail("decfrac double 8051.10083245768396604E-14\nExpected: 8.051100832457683E-11d\nWas: "+DecimalFraction.FromString("8051.10083245768396604E-14").ToDouble());
      if(-9789.0f!=DecimalFraction.FromString("-9789").ToSingle())
        Assert.fail("decfrac single -9789\nExpected: -9789.0f\nWas: "+DecimalFraction.FromString("-9789").ToSingle());
      if(-9789.0d!=DecimalFraction.FromString("-9789").ToDouble())
        Assert.fail("decfrac double -9789\nExpected: -9789.0d\nWas: "+DecimalFraction.FromString("-9789").ToDouble());
      if(-2.95046595E10f!=DecimalFraction.FromString("-295046585154199748.8456E-7").ToSingle())
        Assert.fail("decfrac single -295046585154199748.8456E-7\nExpected: -2.95046595E10f\nWas: "+DecimalFraction.FromString("-295046585154199748.8456E-7").ToSingle());
      if(-2.9504658515419975E10d!=DecimalFraction.FromString("-295046585154199748.8456E-7").ToDouble())
        Assert.fail("decfrac double -295046585154199748.8456E-7\nExpected: -2.9504658515419975E10d\nWas: "+DecimalFraction.FromString("-295046585154199748.8456E-7").ToDouble());
      if(5.8642877E23f!=DecimalFraction.FromString("58642877210005207.915393764393974811E+7").ToSingle())
        Assert.fail("decfrac single 58642877210005207.915393764393974811E+7\nExpected: 5.8642877E23f\nWas: "+DecimalFraction.FromString("58642877210005207.915393764393974811E+7").ToSingle());
      if(5.864287721000521E23d!=DecimalFraction.FromString("58642877210005207.915393764393974811E+7").ToDouble())
        Assert.fail("decfrac double 58642877210005207.915393764393974811E+7\nExpected: 5.864287721000521E23d\nWas: "+DecimalFraction.FromString("58642877210005207.915393764393974811E+7").ToDouble());
      if(-5.13554645E11f!=DecimalFraction.FromString("-513554652569").ToSingle())
        Assert.fail("decfrac single -513554652569\nExpected: -5.13554645E11f\nWas: "+DecimalFraction.FromString("-513554652569").ToSingle());
      if(-5.13554652569E11d!=DecimalFraction.FromString("-513554652569").ToDouble())
        Assert.fail("decfrac double -513554652569\nExpected: -5.13554652569E11d\nWas: "+DecimalFraction.FromString("-513554652569").ToDouble());
      if(-1.66059725E10f!=DecimalFraction.FromString("-166059726561900E-4").ToSingle())
        Assert.fail("decfrac single -166059726561900E-4\nExpected: -1.66059725E10f\nWas: "+DecimalFraction.FromString("-166059726561900E-4").ToSingle());
      if(-1.660597265619E10d!=DecimalFraction.FromString("-166059726561900E-4").ToDouble())
        Assert.fail("decfrac double -166059726561900E-4\nExpected: -1.660597265619E10d\nWas: "+DecimalFraction.FromString("-166059726561900E-4").ToDouble());
      if(-3.66681318E9f!=DecimalFraction.FromString("-3666813090").ToSingle())
        Assert.fail("decfrac single -3666813090\nExpected: -3.66681318E9f\nWas: "+DecimalFraction.FromString("-3666813090").ToSingle());
      if(-3.66681309E9d!=DecimalFraction.FromString("-3666813090").ToDouble())
        Assert.fail("decfrac double -3666813090\nExpected: -3.66681309E9d\nWas: "+DecimalFraction.FromString("-3666813090").ToDouble());
      if(-741.0616f!=DecimalFraction.FromString("-741.061579731811").ToSingle())
        Assert.fail("decfrac single -741.061579731811\nExpected: -741.0616f\nWas: "+DecimalFraction.FromString("-741.061579731811").ToSingle());
      if(-741.061579731811d!=DecimalFraction.FromString("-741.061579731811").ToDouble())
        Assert.fail("decfrac double -741.061579731811\nExpected: -741.061579731811d\nWas: "+DecimalFraction.FromString("-741.061579731811").ToDouble());
      if(-2264.0f!=DecimalFraction.FromString("-2264").ToSingle())
        Assert.fail("decfrac single -2264\nExpected: -2264.0f\nWas: "+DecimalFraction.FromString("-2264").ToSingle());
      if(-2264.0d!=DecimalFraction.FromString("-2264").ToDouble())
        Assert.fail("decfrac double -2264\nExpected: -2264.0d\nWas: "+DecimalFraction.FromString("-2264").ToDouble());
      if(9.2388336E10f!=DecimalFraction.FromString("92388332924").ToSingle())
        Assert.fail("decfrac single 92388332924\nExpected: 9.2388336E10f\nWas: "+DecimalFraction.FromString("92388332924").ToSingle());
      if(9.2388332924E10d!=DecimalFraction.FromString("92388332924").ToDouble())
        Assert.fail("decfrac double 92388332924\nExpected: 9.2388332924E10d\nWas: "+DecimalFraction.FromString("92388332924").ToDouble());
      if(4991.7646f!=DecimalFraction.FromString("4991.764823290772791").ToSingle())
        Assert.fail("decfrac single 4991.764823290772791\nExpected: 4991.7646f\nWas: "+DecimalFraction.FromString("4991.764823290772791").ToSingle());
      if(4991.764823290773d!=DecimalFraction.FromString("4991.764823290772791").ToDouble())
        Assert.fail("decfrac double 4991.764823290772791\nExpected: 4991.764823290773d\nWas: "+DecimalFraction.FromString("4991.764823290772791").ToDouble());
      if(-31529.82f!=DecimalFraction.FromString("-3152982E-2").ToSingle())
        Assert.fail("decfrac single -3152982E-2\nExpected: -31529.82f\nWas: "+DecimalFraction.FromString("-3152982E-2").ToSingle());
      if(-31529.82d!=DecimalFraction.FromString("-3152982E-2").ToDouble())
        Assert.fail("decfrac double -3152982E-2\nExpected: -31529.82d\nWas: "+DecimalFraction.FromString("-3152982E-2").ToDouble());
      if(-2.96352045E15f!=DecimalFraction.FromString("-2963520450661169.515038656").ToSingle())
        Assert.fail("decfrac single -2963520450661169.515038656\nExpected: -2.96352045E15f\nWas: "+DecimalFraction.FromString("-2963520450661169.515038656").ToSingle());
      if(-2.9635204506611695E15d!=DecimalFraction.FromString("-2963520450661169.515038656").ToDouble())
        Assert.fail("decfrac double -2963520450661169.515038656\nExpected: -2.9635204506611695E15d\nWas: "+DecimalFraction.FromString("-2963520450661169.515038656").ToDouble());
      if(-9.0629749E13f!=DecimalFraction.FromString("-9062974752750092585.8070204683471E-5").ToSingle())
        Assert.fail("decfrac single -9062974752750092585.8070204683471E-5\nExpected: -9.0629749E13f\nWas: "+DecimalFraction.FromString("-9062974752750092585.8070204683471E-5").ToSingle());
      if(-9.062974752750092E13d!=DecimalFraction.FromString("-9062974752750092585.8070204683471E-5").ToDouble())
        Assert.fail("decfrac double -9062974752750092585.8070204683471E-5\nExpected: -9.062974752750092E13d\nWas: "+DecimalFraction.FromString("-9062974752750092585.8070204683471E-5").ToDouble());
      if(1.32708426E11f!=DecimalFraction.FromString("1327.08423724267788662E+8").ToSingle())
        Assert.fail("decfrac single 1327.08423724267788662E+8\nExpected: 1.32708426E11f\nWas: "+DecimalFraction.FromString("1327.08423724267788662E+8").ToSingle());
      if(1.3270842372426779E11d!=DecimalFraction.FromString("1327.08423724267788662E+8").ToDouble())
        Assert.fail("decfrac double 1327.08423724267788662E+8\nExpected: 1.3270842372426779E11d\nWas: "+DecimalFraction.FromString("1327.08423724267788662E+8").ToDouble());
      if(3.03766274E11f!=DecimalFraction.FromString("3037662626861314743.2222785E-7").ToSingle())
        Assert.fail("decfrac single 3037662626861314743.2222785E-7\nExpected: 3.03766274E11f\nWas: "+DecimalFraction.FromString("3037662626861314743.2222785E-7").ToSingle());
      if(3.037662626861315E11d!=DecimalFraction.FromString("3037662626861314743.2222785E-7").ToDouble())
        Assert.fail("decfrac double 3037662626861314743.2222785E-7\nExpected: 3.037662626861315E11d\nWas: "+DecimalFraction.FromString("3037662626861314743.2222785E-7").ToDouble());
      if(5.3666539E12f!=DecimalFraction.FromString("5366653818787.5E0").ToSingle())
        Assert.fail("decfrac single 5366653818787.5E0\nExpected: 5.3666539E12f\nWas: "+DecimalFraction.FromString("5366653818787.5E0").ToSingle());
      if(5.3666538187875E12d!=DecimalFraction.FromString("5366653818787.5E0").ToDouble())
        Assert.fail("decfrac double 5366653818787.5E0\nExpected: 5.3666538187875E12d\nWas: "+DecimalFraction.FromString("5366653818787.5E0").ToDouble());
      if(-0.09572517f!=DecimalFraction.FromString("-957251.70125291697919424260E-7").ToSingle())
        Assert.fail("decfrac single -957251.70125291697919424260E-7\nExpected: -0.09572517f\nWas: "+DecimalFraction.FromString("-957251.70125291697919424260E-7").ToSingle());
      if(-0.09572517012529169d!=DecimalFraction.FromString("-957251.70125291697919424260E-7").ToDouble())
        Assert.fail("decfrac double -957251.70125291697919424260E-7\nExpected: -0.09572517012529169d\nWas: "+DecimalFraction.FromString("-957251.70125291697919424260E-7").ToDouble());
      if(8.4375632E7f!=DecimalFraction.FromString("8437563497492.8514E-5").ToSingle())
        Assert.fail("decfrac single 8437563497492.8514E-5\nExpected: 8.4375632E7f\nWas: "+DecimalFraction.FromString("8437563497492.8514E-5").ToSingle());
      if(8.437563497492851E7d!=DecimalFraction.FromString("8437563497492.8514E-5").ToDouble())
        Assert.fail("decfrac double 8437563497492.8514E-5\nExpected: 8.437563497492851E7d\nWas: "+DecimalFraction.FromString("8437563497492.8514E-5").ToDouble());
      if(7.7747428E15f!=DecimalFraction.FromString("7774742890322348.749566199224594").ToSingle())
        Assert.fail("decfrac single 7774742890322348.749566199224594\nExpected: 7.7747428E15f\nWas: "+DecimalFraction.FromString("7774742890322348.749566199224594").ToSingle());
      if(7.774742890322349E15d!=DecimalFraction.FromString("7774742890322348.749566199224594").ToDouble())
        Assert.fail("decfrac double 7774742890322348.749566199224594\nExpected: 7.774742890322349E15d\nWas: "+DecimalFraction.FromString("7774742890322348.749566199224594").ToDouble());
      if(-6.3523806E18f!=DecimalFraction.FromString("-6352380631468114E+3").ToSingle())
        Assert.fail("decfrac single -6352380631468114E+3\nExpected: -6.3523806E18f\nWas: "+DecimalFraction.FromString("-6352380631468114E+3").ToSingle());
      if(-6.3523806314681139E18d!=DecimalFraction.FromString("-6352380631468114E+3").ToDouble())
        Assert.fail("decfrac double -6352380631468114E+3\nExpected: -6.3523806314681139E18d\nWas: "+DecimalFraction.FromString("-6352380631468114E+3").ToDouble());
      if(-8.1199685E23f!=DecimalFraction.FromString("-8119968851439E+11").ToSingle())
        Assert.fail("decfrac single -8119968851439E+11\nExpected: -8.1199685E23f\nWas: "+DecimalFraction.FromString("-8119968851439E+11").ToSingle());
      if(-8.119968851439E23d!=DecimalFraction.FromString("-8119968851439E+11").ToDouble())
        Assert.fail("decfrac double -8119968851439E+11\nExpected: -8.119968851439E23d\nWas: "+DecimalFraction.FromString("-8119968851439E+11").ToDouble());
      if(-3.201959E23f!=DecimalFraction.FromString("-3201959209367.08531737604446E+11").ToSingle())
        Assert.fail("decfrac single -3201959209367.08531737604446E+11\nExpected: -3.201959E23f\nWas: "+DecimalFraction.FromString("-3201959209367.08531737604446E+11").ToSingle());
      if(-3.201959209367085E23d!=DecimalFraction.FromString("-3201959209367.08531737604446E+11").ToDouble())
        Assert.fail("decfrac double -3201959209367.08531737604446E+11\nExpected: -3.201959209367085E23d\nWas: "+DecimalFraction.FromString("-3201959209367.08531737604446E+11").ToDouble());
      if(-6.0171188E7f!=DecimalFraction.FromString("-60171187").ToSingle())
        Assert.fail("decfrac single -60171187\nExpected: -6.0171188E7f\nWas: "+DecimalFraction.FromString("-60171187").ToSingle());
      if(-6.0171187E7d!=DecimalFraction.FromString("-60171187").ToDouble())
        Assert.fail("decfrac double -60171187\nExpected: -6.0171187E7d\nWas: "+DecimalFraction.FromString("-60171187").ToDouble());
      if(-6.6884155E-7f!=DecimalFraction.FromString("-66.884154716131E-8").ToSingle())
        Assert.fail("decfrac single -66.884154716131E-8\nExpected: -6.6884155E-7f\nWas: "+DecimalFraction.FromString("-66.884154716131E-8").ToSingle());
      if(-6.6884154716131E-7d!=DecimalFraction.FromString("-66.884154716131E-8").ToDouble())
        Assert.fail("decfrac double -66.884154716131E-8\nExpected: -6.6884154716131E-7d\nWas: "+DecimalFraction.FromString("-66.884154716131E-8").ToDouble());
      if(923595.4f!=DecimalFraction.FromString("923595.376427445").ToSingle())
        Assert.fail("decfrac single 923595.376427445\nExpected: 923595.4f\nWas: "+DecimalFraction.FromString("923595.376427445").ToSingle());
      if(923595.376427445d!=DecimalFraction.FromString("923595.376427445").ToDouble())
        Assert.fail("decfrac double 923595.376427445\nExpected: 923595.376427445d\nWas: "+DecimalFraction.FromString("923595.376427445").ToDouble());
      if(-5.0f!=DecimalFraction.FromString("-5").ToSingle())
        Assert.fail("decfrac single -5\nExpected: -5.0f\nWas: "+DecimalFraction.FromString("-5").ToSingle());
      if(-5.0d!=DecimalFraction.FromString("-5").ToDouble())
        Assert.fail("decfrac double -5\nExpected: -5.0d\nWas: "+DecimalFraction.FromString("-5").ToDouble());
      if(4.7380017E10f!=DecimalFraction.FromString("47380017776.35").ToSingle())
        Assert.fail("decfrac single 47380017776.35\nExpected: 4.7380017E10f\nWas: "+DecimalFraction.FromString("47380017776.35").ToSingle());
      if(4.738001777635E10d!=DecimalFraction.FromString("47380017776.35").ToDouble())
        Assert.fail("decfrac double 47380017776.35\nExpected: 4.738001777635E10d\nWas: "+DecimalFraction.FromString("47380017776.35").ToDouble());
      if(8139584.0f!=DecimalFraction.FromString("8139584.242987").ToSingle())
        Assert.fail("decfrac single 8139584.242987\nExpected: 8139584.0f\nWas: "+DecimalFraction.FromString("8139584.242987").ToSingle());
      if(8139584.242987d!=DecimalFraction.FromString("8139584.242987").ToDouble())
        Assert.fail("decfrac double 8139584.242987\nExpected: 8139584.242987d\nWas: "+DecimalFraction.FromString("8139584.242987").ToDouble());
      if(5.0f!=DecimalFraction.FromString("5").ToSingle())
        Assert.fail("decfrac single 5\nExpected: 5.0f\nWas: "+DecimalFraction.FromString("5").ToSingle());
      if(5.0d!=DecimalFraction.FromString("5").ToDouble())
        Assert.fail("decfrac double 5\nExpected: 5.0d\nWas: "+DecimalFraction.FromString("5").ToDouble());
      if(-3.6578223E27f!=DecimalFraction.FromString("-365782224812843E+13").ToSingle())
        Assert.fail("decfrac single -365782224812843E+13\nExpected: -3.6578223E27f\nWas: "+DecimalFraction.FromString("-365782224812843E+13").ToSingle());
      if(-3.65782224812843E27d!=DecimalFraction.FromString("-365782224812843E+13").ToDouble())
        Assert.fail("decfrac double -365782224812843E+13\nExpected: -3.65782224812843E27d\nWas: "+DecimalFraction.FromString("-365782224812843E+13").ToDouble());
      if(6.263606E23f!=DecimalFraction.FromString("626360584867890223E+6").ToSingle())
        Assert.fail("decfrac single 626360584867890223E+6\nExpected: 6.263606E23f\nWas: "+DecimalFraction.FromString("626360584867890223E+6").ToSingle());
      if(6.263605848678903E23d!=DecimalFraction.FromString("626360584867890223E+6").ToDouble())
        Assert.fail("decfrac double 626360584867890223E+6\nExpected: 6.263605848678903E23d\nWas: "+DecimalFraction.FromString("626360584867890223E+6").ToDouble());
      if(-1.26830412E18f!=DecimalFraction.FromString("-12683040859E+8").ToSingle())
        Assert.fail("decfrac single -12683040859E+8\nExpected: -1.26830412E18f\nWas: "+DecimalFraction.FromString("-12683040859E+8").ToSingle());
      if(-1.2683040859E18d!=DecimalFraction.FromString("-12683040859E+8").ToDouble())
        Assert.fail("decfrac double -12683040859E+8\nExpected: -1.2683040859E18d\nWas: "+DecimalFraction.FromString("-12683040859E+8").ToDouble());
      if(8.9906433E13f!=DecimalFraction.FromString("89906433733052.14691421345561385").ToSingle())
        Assert.fail("decfrac single 89906433733052.14691421345561385\nExpected: 8.9906433E13f\nWas: "+DecimalFraction.FromString("89906433733052.14691421345561385").ToSingle());
      if(8.990643373305214E13d!=DecimalFraction.FromString("89906433733052.14691421345561385").ToDouble())
        Assert.fail("decfrac double 89906433733052.14691421345561385\nExpected: 8.990643373305214E13d\nWas: "+DecimalFraction.FromString("89906433733052.14691421345561385").ToDouble());
      if(82.0f!=DecimalFraction.FromString("82").ToSingle())
        Assert.fail("decfrac single 82\nExpected: 82.0f\nWas: "+DecimalFraction.FromString("82").ToSingle());
      if(82.0d!=DecimalFraction.FromString("82").ToDouble())
        Assert.fail("decfrac double 82\nExpected: 82.0d\nWas: "+DecimalFraction.FromString("82").ToDouble());
      if(9.5523543E16f!=DecimalFraction.FromString("95523541216159667").ToSingle())
        Assert.fail("decfrac single 95523541216159667\nExpected: 9.5523543E16f\nWas: "+DecimalFraction.FromString("95523541216159667").ToSingle());
      if(9.5523541216159664E16d!=DecimalFraction.FromString("95523541216159667").ToDouble())
        Assert.fail("decfrac double 95523541216159667\nExpected: 9.5523541216159664E16d\nWas: "+DecimalFraction.FromString("95523541216159667").ToDouble());
      if(-0.040098447f!=DecimalFraction.FromString("-400984.46498769274346390686E-7").ToSingle())
        Assert.fail("decfrac single -400984.46498769274346390686E-7\nExpected: -0.040098447f\nWas: "+DecimalFraction.FromString("-400984.46498769274346390686E-7").ToSingle());
      if(-0.04009844649876927d!=DecimalFraction.FromString("-400984.46498769274346390686E-7").ToDouble())
        Assert.fail("decfrac double -400984.46498769274346390686E-7\nExpected: -0.04009844649876927d\nWas: "+DecimalFraction.FromString("-400984.46498769274346390686E-7").ToDouble());
      if(9.9082332E14f!=DecimalFraction.FromString("990823307532E+3").ToSingle())
        Assert.fail("decfrac single 990823307532E+3\nExpected: 9.9082332E14f\nWas: "+DecimalFraction.FromString("990823307532E+3").ToSingle());
      if(9.90823307532E14d!=DecimalFraction.FromString("990823307532E+3").ToDouble())
        Assert.fail("decfrac double 990823307532E+3\nExpected: 9.90823307532E14d\nWas: "+DecimalFraction.FromString("990823307532E+3").ToDouble());
      if(-8.969879E8f!=DecimalFraction.FromString("-896987890").ToSingle())
        Assert.fail("decfrac single -896987890\nExpected: -8.969879E8f\nWas: "+DecimalFraction.FromString("-896987890").ToSingle());
      if(-8.9698789E8d!=DecimalFraction.FromString("-896987890").ToDouble())
        Assert.fail("decfrac double -896987890\nExpected: -8.9698789E8d\nWas: "+DecimalFraction.FromString("-896987890").ToDouble());
      if(-5.1842734E9f!=DecimalFraction.FromString("-5184273642.760").ToSingle())
        Assert.fail("decfrac single -5184273642.760\nExpected: -5.1842734E9f\nWas: "+DecimalFraction.FromString("-5184273642.760").ToSingle());
      if(-5.18427364276E9d!=DecimalFraction.FromString("-5184273642.760").ToDouble())
        Assert.fail("decfrac double -5184273642.760\nExpected: -5.18427364276E9d\nWas: "+DecimalFraction.FromString("-5184273642.760").ToDouble());
      if(5.03393772E17f!=DecimalFraction.FromString("503393788336283974").ToSingle())
        Assert.fail("decfrac single 503393788336283974\nExpected: 5.03393772E17f\nWas: "+DecimalFraction.FromString("503393788336283974").ToSingle());
      if(5.0339378833628397E17d!=DecimalFraction.FromString("503393788336283974").ToDouble())
        Assert.fail("decfrac double 503393788336283974\nExpected: 5.0339378833628397E17d\nWas: "+DecimalFraction.FromString("503393788336283974").ToDouble());
      if(-5.50587E15f!=DecimalFraction.FromString("-550587E+10").ToSingle())
        Assert.fail("decfrac single -550587E+10\nExpected: -5.50587E15f\nWas: "+DecimalFraction.FromString("-550587E+10").ToSingle());
      if(-5.50587E15d!=DecimalFraction.FromString("-550587E+10").ToDouble())
        Assert.fail("decfrac double -550587E+10\nExpected: -5.50587E15d\nWas: "+DecimalFraction.FromString("-550587E+10").ToDouble());
      if(-4.0559753E-5f!=DecimalFraction.FromString("-405597523930.814E-16").ToSingle())
        Assert.fail("decfrac single -405597523930.814E-16\nExpected: -4.0559753E-5f\nWas: "+DecimalFraction.FromString("-405597523930.814E-16").ToSingle());
      if(-4.05597523930814E-5d!=DecimalFraction.FromString("-405597523930.814E-16").ToDouble())
        Assert.fail("decfrac double -405597523930.814E-16\nExpected: -4.05597523930814E-5d\nWas: "+DecimalFraction.FromString("-405597523930.814E-16").ToDouble());
      if(-5.326398E9f!=DecimalFraction.FromString("-5326397977").ToSingle())
        Assert.fail("decfrac single -5326397977\nExpected: -5.326398E9f\nWas: "+DecimalFraction.FromString("-5326397977").ToSingle());
      if(-5.326397977E9d!=DecimalFraction.FromString("-5326397977").ToDouble())
        Assert.fail("decfrac double -5326397977\nExpected: -5.326397977E9d\nWas: "+DecimalFraction.FromString("-5326397977").ToDouble());
      if(-9997.447f!=DecimalFraction.FromString("-9997.44701170").ToSingle())
        Assert.fail("decfrac single -9997.44701170\nExpected: -9997.447f\nWas: "+DecimalFraction.FromString("-9997.44701170").ToSingle());
      if(-9997.4470117d!=DecimalFraction.FromString("-9997.44701170").ToDouble())
        Assert.fail("decfrac double -9997.44701170\nExpected: -9997.4470117d\nWas: "+DecimalFraction.FromString("-9997.44701170").ToDouble());
      if(7.3258664E7f!=DecimalFraction.FromString("73258664.23970751611061").ToSingle())
        Assert.fail("decfrac single 73258664.23970751611061\nExpected: 7.3258664E7f\nWas: "+DecimalFraction.FromString("73258664.23970751611061").ToSingle());
      if(7.325866423970751E7d!=DecimalFraction.FromString("73258664.23970751611061").ToDouble())
        Assert.fail("decfrac double 73258664.23970751611061\nExpected: 7.325866423970751E7d\nWas: "+DecimalFraction.FromString("73258664.23970751611061").ToDouble());
      if(-7.9944785E13f!=DecimalFraction.FromString("-79944788804361.667255656660").ToSingle())
        Assert.fail("decfrac single -79944788804361.667255656660\nExpected: -7.9944785E13f\nWas: "+DecimalFraction.FromString("-79944788804361.667255656660").ToSingle());
      if(-7.994478880436167E13d!=DecimalFraction.FromString("-79944788804361.667255656660").ToDouble())
        Assert.fail("decfrac double -79944788804361.667255656660\nExpected: -7.994478880436167E13d\nWas: "+DecimalFraction.FromString("-79944788804361.667255656660").ToDouble());
      if(9.852337E19f!=DecimalFraction.FromString("98523363000987953313E0").ToSingle())
        Assert.fail("decfrac single 98523363000987953313E0\nExpected: 9.852337E19f\nWas: "+DecimalFraction.FromString("98523363000987953313E0").ToSingle());
      if(9.852336300098796E19d!=DecimalFraction.FromString("98523363000987953313E0").ToDouble())
        Assert.fail("decfrac double 98523363000987953313E0\nExpected: 9.852336300098796E19d\nWas: "+DecimalFraction.FromString("98523363000987953313E0").ToDouble());
      if(5.981638E15f!=DecimalFraction.FromString("5981637941716431.55749471240993").ToSingle())
        Assert.fail("decfrac single 5981637941716431.55749471240993\nExpected: 5.981638E15f\nWas: "+DecimalFraction.FromString("5981637941716431.55749471240993").ToSingle());
      if(5.981637941716432E15d!=DecimalFraction.FromString("5981637941716431.55749471240993").ToDouble())
        Assert.fail("decfrac double 5981637941716431.55749471240993\nExpected: 5.981637941716432E15d\nWas: "+DecimalFraction.FromString("5981637941716431.55749471240993").ToDouble());
      if(-1.995E-9f!=DecimalFraction.FromString("-1995E-12").ToSingle())
        Assert.fail("decfrac single -1995E-12\nExpected: -1.995E-9f\nWas: "+DecimalFraction.FromString("-1995E-12").ToSingle());
      if(-1.995E-9d!=DecimalFraction.FromString("-1995E-12").ToDouble())
        Assert.fail("decfrac double -1995E-12\nExpected: -1.995E-9d\nWas: "+DecimalFraction.FromString("-1995E-12").ToDouble());
      if(2.59017677E9f!=DecimalFraction.FromString("2590176810").ToSingle())
        Assert.fail("decfrac single 2590176810\nExpected: 2.59017677E9f\nWas: "+DecimalFraction.FromString("2590176810").ToSingle());
      if(2.59017681E9d!=DecimalFraction.FromString("2590176810").ToDouble())
        Assert.fail("decfrac double 2590176810\nExpected: 2.59017681E9d\nWas: "+DecimalFraction.FromString("2590176810").ToDouble());
      if(2.9604614f!=DecimalFraction.FromString("2.960461297").ToSingle())
        Assert.fail("decfrac single 2.960461297\nExpected: 2.9604614f\nWas: "+DecimalFraction.FromString("2.960461297").ToSingle());
      if(2.960461297d!=DecimalFraction.FromString("2.960461297").ToDouble())
        Assert.fail("decfrac double 2.960461297\nExpected: 2.960461297d\nWas: "+DecimalFraction.FromString("2.960461297").ToDouble());
      if(768802.0f!=DecimalFraction.FromString("768802").ToSingle())
        Assert.fail("decfrac single 768802\nExpected: 768802.0f\nWas: "+DecimalFraction.FromString("768802").ToSingle());
      if(768802.0d!=DecimalFraction.FromString("768802").ToDouble())
        Assert.fail("decfrac double 768802\nExpected: 768802.0d\nWas: "+DecimalFraction.FromString("768802").ToDouble());
      if(145417.38f!=DecimalFraction.FromString("145417.373").ToSingle())
        Assert.fail("decfrac single 145417.373\nExpected: 145417.38f\nWas: "+DecimalFraction.FromString("145417.373").ToSingle());
      if(145417.373d!=DecimalFraction.FromString("145417.373").ToDouble())
        Assert.fail("decfrac double 145417.373\nExpected: 145417.373d\nWas: "+DecimalFraction.FromString("145417.373").ToDouble());
      if(540905.0f!=DecimalFraction.FromString("540905").ToSingle())
        Assert.fail("decfrac single 540905\nExpected: 540905.0f\nWas: "+DecimalFraction.FromString("540905").ToSingle());
      if(540905.0d!=DecimalFraction.FromString("540905").ToDouble())
        Assert.fail("decfrac double 540905\nExpected: 540905.0d\nWas: "+DecimalFraction.FromString("540905").ToDouble());
      if(-6.811958E20f!=DecimalFraction.FromString("-681.1958019894E+18").ToSingle())
        Assert.fail("decfrac single -681.1958019894E+18\nExpected: -6.811958E20f\nWas: "+DecimalFraction.FromString("-681.1958019894E+18").ToSingle());
      if(-6.811958019894E20d!=DecimalFraction.FromString("-681.1958019894E+18").ToDouble())
        Assert.fail("decfrac double -681.1958019894E+18\nExpected: -6.811958019894E20d\nWas: "+DecimalFraction.FromString("-681.1958019894E+18").ToDouble());
      if(54846.0f!=DecimalFraction.FromString("54846.0").ToSingle())
        Assert.fail("decfrac single 54846.0\nExpected: 54846.0f\nWas: "+DecimalFraction.FromString("54846.0").ToSingle());
      if(54846.0d!=DecimalFraction.FromString("54846.0").ToDouble())
        Assert.fail("decfrac double 54846.0\nExpected: 54846.0d\nWas: "+DecimalFraction.FromString("54846.0").ToDouble());
      if(9.7245E9f!=DecimalFraction.FromString("97245E+5").ToSingle())
        Assert.fail("decfrac single 97245E+5\nExpected: 9.7245E9f\nWas: "+DecimalFraction.FromString("97245E+5").ToSingle());
      if(9.7245E9d!=DecimalFraction.FromString("97245E+5").ToDouble())
        Assert.fail("decfrac double 97245E+5\nExpected: 9.7245E9d\nWas: "+DecimalFraction.FromString("97245E+5").ToDouble());
      if(-26.0f!=DecimalFraction.FromString("-26").ToSingle())
        Assert.fail("decfrac single -26\nExpected: -26.0f\nWas: "+DecimalFraction.FromString("-26").ToSingle());
      if(-26.0d!=DecimalFraction.FromString("-26").ToDouble())
        Assert.fail("decfrac double -26\nExpected: -26.0d\nWas: "+DecimalFraction.FromString("-26").ToDouble());
      if(4.15749164E12f!=DecimalFraction.FromString("4157491532482.05").ToSingle())
        Assert.fail("decfrac single 4157491532482.05\nExpected: 4.15749164E12f\nWas: "+DecimalFraction.FromString("4157491532482.05").ToSingle());
      if(4.15749153248205E12d!=DecimalFraction.FromString("4157491532482.05").ToDouble())
        Assert.fail("decfrac double 4157491532482.05\nExpected: 4.15749153248205E12d\nWas: "+DecimalFraction.FromString("4157491532482.05").ToDouble());
      if(4.7747967E15f!=DecimalFraction.FromString("4774796769101.23808389660855287E+3").ToSingle())
        Assert.fail("decfrac single 4774796769101.23808389660855287E+3\nExpected: 4.7747967E15f\nWas: "+DecimalFraction.FromString("4774796769101.23808389660855287E+3").ToSingle());
      if(4.774796769101238E15d!=DecimalFraction.FromString("4774796769101.23808389660855287E+3").ToDouble())
        Assert.fail("decfrac double 4774796769101.23808389660855287E+3\nExpected: 4.774796769101238E15d\nWas: "+DecimalFraction.FromString("4774796769101.23808389660855287E+3").ToDouble());
      if(-9.8263879E14f!=DecimalFraction.FromString("-982638781021905").ToSingle())
        Assert.fail("decfrac single -982638781021905\nExpected: -9.8263879E14f\nWas: "+DecimalFraction.FromString("-982638781021905").ToSingle());
      if(-9.82638781021905E14d!=DecimalFraction.FromString("-982638781021905").ToDouble())
        Assert.fail("decfrac double -982638781021905\nExpected: -9.82638781021905E14d\nWas: "+DecimalFraction.FromString("-982638781021905").ToDouble());
      if(8.8043432E18f!=DecimalFraction.FromString("8804343287262864743").ToSingle())
        Assert.fail("decfrac single 8804343287262864743\nExpected: 8.8043432E18f\nWas: "+DecimalFraction.FromString("8804343287262864743").ToSingle());
      if(8.8043432872628644E18d!=DecimalFraction.FromString("8804343287262864743").ToDouble())
        Assert.fail("decfrac double 8804343287262864743\nExpected: 8.8043432872628644E18d\nWas: "+DecimalFraction.FromString("8804343287262864743").ToDouble());
      if(-6.5138669E13f!=DecimalFraction.FromString("-65138668135711").ToSingle())
        Assert.fail("decfrac single -65138668135711\nExpected: -6.5138669E13f\nWas: "+DecimalFraction.FromString("-65138668135711").ToSingle());
      if(-6.5138668135711E13d!=DecimalFraction.FromString("-65138668135711").ToDouble())
        Assert.fail("decfrac double -65138668135711\nExpected: -6.5138668135711E13d\nWas: "+DecimalFraction.FromString("-65138668135711").ToDouble());
      if(-5.9235733E15f!=DecimalFraction.FromString("-5923573055061163").ToSingle())
        Assert.fail("decfrac single -5923573055061163\nExpected: -5.9235733E15f\nWas: "+DecimalFraction.FromString("-5923573055061163").ToSingle());
      if(-5.923573055061163E15d!=DecimalFraction.FromString("-5923573055061163").ToDouble())
        Assert.fail("decfrac double -5923573055061163\nExpected: -5.923573055061163E15d\nWas: "+DecimalFraction.FromString("-5923573055061163").ToDouble());
      if(-8.6853E-8f!=DecimalFraction.FromString("-8.6853E-8").ToSingle())
        Assert.fail("decfrac single -8.6853E-8\nExpected: -8.6853E-8f\nWas: "+DecimalFraction.FromString("-8.6853E-8").ToSingle());
      if(-8.6853E-8d!=DecimalFraction.FromString("-8.6853E-8").ToDouble())
        Assert.fail("decfrac double -8.6853E-8\nExpected: -8.6853E-8d\nWas: "+DecimalFraction.FromString("-8.6853E-8").ToDouble());
      if(19707.0f!=DecimalFraction.FromString("19707").ToSingle())
        Assert.fail("decfrac single 19707\nExpected: 19707.0f\nWas: "+DecimalFraction.FromString("19707").ToSingle());
      if(19707.0d!=DecimalFraction.FromString("19707").ToDouble())
        Assert.fail("decfrac double 19707\nExpected: 19707.0d\nWas: "+DecimalFraction.FromString("19707").ToDouble());
      if(-8.8478554E14f!=DecimalFraction.FromString("-884785536200446.1859332080").ToSingle())
        Assert.fail("decfrac single -884785536200446.1859332080\nExpected: -8.8478554E14f\nWas: "+DecimalFraction.FromString("-884785536200446.1859332080").ToSingle());
      if(-8.847855362004461E14d!=DecimalFraction.FromString("-884785536200446.1859332080").ToDouble())
        Assert.fail("decfrac double -884785536200446.1859332080\nExpected: -8.847855362004461E14d\nWas: "+DecimalFraction.FromString("-884785536200446.1859332080").ToDouble());
      if(-1.0f!=DecimalFraction.FromString("-1").ToSingle())
        Assert.fail("decfrac single -1\nExpected: -1.0f\nWas: "+DecimalFraction.FromString("-1").ToSingle());
      if(-1.0d!=DecimalFraction.FromString("-1").ToDouble())
        Assert.fail("decfrac double -1\nExpected: -1.0d\nWas: "+DecimalFraction.FromString("-1").ToDouble());
    }
    /**
     * 
     */
@Test
    public void TestDecimalFrac() {
      TestCommon.FromBytesTestAB(
        new byte[]{ (byte)0xc4, (byte)0x82, 0x3, 0x1a, 1, 2, 3, 4 });
    }
    @Test(expected=CBORException.class)
    public void TestDecimalFracExponentMustNotBeBignum() {
      TestCommon.FromBytesTestAB(
        new byte[]{ (byte)0xc4, (byte)0x82, (byte)0xc2, 0x41, 1, 0x1a, 1, 2, 3, 4 });
    }
    /**
     * 
     */
@Test
    public void TestDoubleToOther() {
      CBORObject dbl1 = CBORObject.FromObject((double)Integer.MIN_VALUE);
      CBORObject dbl2 = CBORObject.FromObject((double)Integer.MAX_VALUE);
      try { dbl1.AsInt16(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { dbl1.AsByte(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { dbl1.AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { dbl1.AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { dbl1.AsBigInteger(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { dbl2.AsInt16(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { dbl2.AsByte(); Assert.fail("Should have failed");} catch(ArithmeticException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { dbl2.AsInt32(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { dbl2.AsInt64(); } catch(Exception ex){ Assert.fail(ex.toString()); }
      try { dbl2.AsBigInteger(); } catch(Exception ex){ Assert.fail(ex.toString()); }
    }
    /**
     * 
     */
@Test
    public void TestBigTag() {
      CBORObject.FromObjectAndTag(CBORObject.Null, new BigInteger("18446744073709551615"));
    }
    @Test(expected=CBORException.class)
    public void TestDecimalFracExactlyTwoElements() {
      TestCommon.FromBytesTestAB(
        new byte[]{ (byte)0xc4, (byte)0x82, (byte)0xc2, 0x41, 1 });
    }
    /**
     * 
     */
@Test
    public void TestDecimalFracMantissaMayBeBignum() {
      CBORObject o=TestCommon.FromBytesTestAB(
        new byte[]{ (byte)0xc4, (byte)0x82, 0x3, (byte)0xc2, 0x41, 1 });
      Assert.assertEquals(new DecimalFraction(1,3),o.AsDecimalFraction());
    }
    /**
     * 
     */
@Test
    public void TestShort() {
      for (int i = Short.MIN_VALUE; i <= Short.MAX_VALUE; i++) {
        TestCommon.AssertSer(
          CBORObject.FromObject((short)i),
          String.format(java.util.Locale.US,"%s", i));
      }
    }
    /**
     * 
     */
@Test
    public void TestByteArray() {
      TestCommon.AssertSer(
        CBORObject.FromObject(new byte[]{ 0x20, 0x78 }), "h'2078'");
    }
    
    /**
     * 
     */
@Test
    public void TestBigNumBytes() {
      CBORObject o=null;
      o=TestCommon.FromBytesTestAB(new byte[]{(byte)0xc2,0x41,(byte)0x88});
      Assert.assertEquals(BigInteger.valueOf(0x88L),o.AsBigInteger());
      o=TestCommon.FromBytesTestAB(new byte[]{(byte)0xc2,0x42,(byte)0x88,0x77});
      Assert.assertEquals(BigInteger.valueOf(0x8877L),o.AsBigInteger());
      o=TestCommon.FromBytesTestAB(new byte[]{(byte)0xc2,0x44,(byte)0x88,0x77,0x66,0x55});
      Assert.assertEquals(BigInteger.valueOf(0x88776655L),o.AsBigInteger());
      o=TestCommon.FromBytesTestAB(new byte[]{(byte)0xc2,0x47,(byte)0x88,0x77,0x66,0x55,0x44,0x33,0x22});
      Assert.assertEquals(BigInteger.valueOf(0x88776655443322L),o.AsBigInteger());
    }
    
    /**
     * 
     */
@Test
    public void TestTaggedUntagged() {
      for(int i=200;i<1000;i++){
        CBORObject o,o2;
        o=CBORObject.FromObject(0);
        o2=CBORObject.FromObjectAndTag(o,i);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.FromObjectAndTag(o,i+1);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.FromObject(BigInteger.ONE.shiftLeft(100));
        o2=CBORObject.FromObjectAndTag(o,i);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.FromObjectAndTag(o,i+1);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.FromObject(new byte[]{1,2,3});
        o2=CBORObject.FromObjectAndTag(o,i);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.FromObjectAndTag(o,i+1);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.NewArray();
        o.Add(CBORObject.FromObject(0));
        o.Add(CBORObject.FromObject(1));
        o.Add(CBORObject.FromObject(2));
        o2=CBORObject.FromObjectAndTag(o,i);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.FromObjectAndTag(o,i+1);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.NewMap();
        o.Add("a",CBORObject.FromObject(0));
        o.Add("b",CBORObject.FromObject(1));
        o.Add("c",CBORObject.FromObject(2));
        o2=CBORObject.FromObjectAndTag(o,i);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.FromObjectAndTag(o,i+1);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.FromObject("a");
        o2=CBORObject.FromObjectAndTag(o,i);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.FromObjectAndTag(o,i+1);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.False;
        o2=CBORObject.FromObjectAndTag(o,i);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.FromObjectAndTag(o,i+1);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.True;
        o2=CBORObject.FromObjectAndTag(o,i);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.FromObjectAndTag(o,i+1);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.Null;
        o2=CBORObject.FromObjectAndTag(o,i);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.FromObjectAndTag(o,i+1);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.Undefined;
        o2=CBORObject.FromObjectAndTag(o,i);
        TestCommon.AssertEqualsHashCode(o,o2);
        o=CBORObject.FromObjectAndTag(o,i+1);
        TestCommon.AssertEqualsHashCode(o,o2);
      }
    }
    
    /**
     * 
     */
@Test
    public void TestBigInteger() {
      BigInteger bi = BigInteger.valueOf(3);
      BigInteger negseven = BigInteger.valueOf(-7);
      for (int i = 0; i < 500; i++) {
        TestCommon.AssertSer(
          CBORObject.FromObject(bi),
          String.format(java.util.Locale.US,"%s", bi));
        bi=bi.multiply(negseven);
      }
      BigInteger[] ranges = new BigInteger[]{
        BigInteger.valueOf(Long.MIN_VALUE).subtract(BigInteger.valueOf(512)),
        BigInteger.valueOf(Long.MIN_VALUE).add(BigInteger.valueOf(512)),
        new BigInteger("-18446744073709551616").subtract(BigInteger.valueOf(512)),
        new BigInteger("-18446744073709551616").add(BigInteger.valueOf(512)),
        BigInteger.valueOf(Long.MAX_VALUE).subtract(BigInteger.valueOf(512)),
        BigInteger.valueOf(Long.MAX_VALUE).add(BigInteger.valueOf(512)),
        new BigInteger("18446744073709551615").subtract(BigInteger.valueOf(512)),
        new BigInteger("18446744073709551615").add(BigInteger.valueOf(512)),
      };
      for (int i = 0; i < ranges.length; i += 2) {
        BigInteger bigintTemp = ranges[i];
        while (true) {
          TestCommon.AssertSer(
            CBORObject.FromObject(bigintTemp),
            String.format(java.util.Locale.US,"%s", bigintTemp));
          if (bigintTemp.equals(ranges[i + 1])) break;
          bigintTemp = bigintTemp .add(BigInteger.ONE);
        }
      }
    }
    /**
     * 
     */
@Test
    public void TestLong() {
      long[] ranges = new long[]{
        -65539,65539,
        0xFFFFF000L,0x100000400L,
        Long.MAX_VALUE-1000,Long.MAX_VALUE,
        Long.MIN_VALUE,Long.MIN_VALUE+1000
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
          TestCommon.AssertSer(obj,
                               String.format(java.util.Locale.US,"[%s]", j));
          if (j == ranges[i + 1]) break;
          j++;
        }
      }
    }
    /**
     * 
     */
@Test
    public void TestFloat() {
      TestCommon.AssertSer(CBORObject.FromObject(Float.POSITIVE_INFINITY),
                           "Infinity");
      TestCommon.AssertSer(CBORObject.FromObject(Float.NEGATIVE_INFINITY),
                           "-Infinity");
      TestCommon.AssertSer(CBORObject.FromObject(Float.NaN),
                           "NaN");
      for (int i = -65539; i <= 65539; i++) {
        TestCommon.AssertSer(
          CBORObject.FromObject((float)i),
          String.format(java.util.Locale.US,"%s", i));
      }
    }
    /**
     * 
     */
@Test
    public void TestCodePointCompare() {
      Assert.assertEquals(0, (int)Math.signum(DataUtilities.CodePointCompare("abc", "abc")));
      Assert.assertEquals(0, (int)Math.signum(DataUtilities.CodePointCompare("\ud800\udc00", "\ud800\udc00")));
      Assert.assertEquals(-1, (int)Math.signum(DataUtilities.CodePointCompare("abc", "\ud800\udc00")));
      Assert.assertEquals(-1, (int)Math.signum(DataUtilities.CodePointCompare("\uf000", "\ud800\udc00")));
      Assert.assertEquals(1, (int)Math.signum(DataUtilities.CodePointCompare("\uf000", "\ud800")));
    }
    /**
     * 
     */
@Test
    public void TestSimpleValues() {
      TestCommon.AssertSer(CBORObject.FromObject(true),
                           "true");
      TestCommon.AssertSer(CBORObject.FromObject(false),
                           "false");
      TestCommon.AssertSer(CBORObject.FromObject((Object)null),
                           "null");
    }
    /**
     * 
     */
@Test
    public void TestGetUtf8Length() {
      try { DataUtilities.GetUtf8Length(null, true); } catch(NullPointerException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      try { DataUtilities.GetUtf8Length(null, false); } catch(NullPointerException ex) { } catch (Exception ex) { Assert.fail(ex.toString()); }
      Assert.assertEquals(3, DataUtilities.GetUtf8Length("abc", true));
      Assert.assertEquals(4, DataUtilities.GetUtf8Length("\u0300\u0300", true));
      Assert.assertEquals(6, DataUtilities.GetUtf8Length("\u3000\u3000", true));
      Assert.assertEquals(6, DataUtilities.GetUtf8Length("\ud800\ud800", true));
      Assert.assertEquals(-1, DataUtilities.GetUtf8Length("\ud800\ud800", false));
    }
    /**
     * 
     */
@Test
    public void TestDouble() {
      if(!CBORObject.FromObject(Double.POSITIVE_INFINITY).IsPositiveInfinity())
        Assert.fail("Not positive infinity");
      TestCommon.AssertSer(CBORObject.FromObject(Double.POSITIVE_INFINITY),
                           "Infinity");
      TestCommon.AssertSer(CBORObject.FromObject(Double.NEGATIVE_INFINITY),
                           "-Infinity");
      TestCommon.AssertSer(CBORObject.FromObject(Double.NaN),
                           "NaN");
      CBORObject oldobj = null;
      for (int i = -65539; i <= 65539; i++) {
        CBORObject o = CBORObject.FromObject((double)i);
        TestCommon.AssertSer(o,
                             String.format(java.util.Locale.US,"%s", i));
        if (oldobj != null) {
          CompareTestLess(oldobj,o);
        }
        oldobj = o;
      }
    }
    /**
     * 
     */
@Test
    public void TestTags() {
      BigInteger maxuint = new BigInteger("18446744073709551615");
      BigInteger[] ranges = new BigInteger[]{
        BigInteger.valueOf(6),
        BigInteger.valueOf(65539),
        BigInteger.valueOf(Integer.MAX_VALUE).subtract(BigInteger.valueOf(500)),
        BigInteger.valueOf(Integer.MAX_VALUE).add(BigInteger.valueOf(500)),
        BigInteger.valueOf(Long.MAX_VALUE).subtract(BigInteger.valueOf(500)),
        BigInteger.valueOf(Long.MAX_VALUE).add(BigInteger.valueOf(500)),
        new BigInteger("18446744073709551615").subtract(BigInteger.valueOf(500)),
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
          if (!obj.getInnermostTag().equals(bigintTemp))
            Assert.assertEquals(String.format(java.util.Locale.US,"obj tag doesn't match: %s", obj),bigintTemp,obj.getInnermostTag());
          TestCommon.AssertSer(
            obj,
            String.format(java.util.Locale.US,"%s(0)", bigintTemp));
          if (!(bigintTemp.equals(maxuint))) {
            // Test multiple tags
            CBORObject obj2 = CBORObject.FromObjectAndTag(obj, bigintTemp .add(BigInteger.ONE));
            BigInteger[] bi = obj2.GetTags();
            if (bi.length != 2)
              Assert.assertEquals(String.format(java.util.Locale.US,"Expected 2 tags: %s", obj2),2,bi.length);
            if (!bi[0].equals((BigInteger)bigintTemp .add(BigInteger.ONE)))
              Assert.assertEquals(String.format(java.util.Locale.US,"Outer tag doesn't match: %s", obj2),bigintTemp .add(BigInteger.ONE),bi[0]);
            if (!(bi[1] .equals(bigintTemp)))
              Assert.assertEquals(String.format(java.util.Locale.US,"Inner tag doesn't match: %s", obj2),bigintTemp,bi[1]);
            if (!(obj2.getInnermostTag() .equals(bigintTemp)))
              Assert.assertEquals(String.format(java.util.Locale.US,"Innermost tag doesn't match: %s", obj2),bigintTemp,bi[0]);
            TestCommon.AssertSer(
              obj2,
              String.format(java.util.Locale.US,"%s(%s(0))",
                            bigintTemp .add(BigInteger.ONE), bigintTemp));
          }
          if (bigintTemp.equals(ranges[i + 1])) break;
          bigintTemp = bigintTemp .add(BigInteger.ONE);
        }
      }
    }
  }
