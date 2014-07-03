package com.upokecenter.test;

import java.io.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.cbor.*;

  public class BEncodingTest {
    private static CBORObject EncodingFromBytes(byte[] b) {
      try {
        java.io.ByteArrayInputStream s = null;
try {
s = new java.io.ByteArrayInputStream(b);

          return BEncoding.Read(s);
}
finally {
try { if (s != null)s.close(); } catch (java.io.IOException ex) {}
}
      } catch (IOException ex) {
        throw new CBORException("", ex);
      }
    }

    private static byte[] EncodingToBytes(CBORObject b) {
      try {
        java.io.ByteArrayOutputStream s = null;
try {
s = new java.io.ByteArrayOutputStream();

          BEncoding.Write(b, s);
          return s.toByteArray();
}
finally {
try { if (s != null)s.close(); } catch (java.io.IOException ex) {}
}
      } catch (IOException ex) {
        throw new CBORException("", ex);
      }
    }

    public static void doTestLong(long value) {
      String b = "i" + value + "e";
      CBORObject beo = EncodingFromBytes(com.upokecenter.util.DataUtilities.GetUtf8Bytes(b, true));
      Assert.assertEquals(value, beo.AsInt64());
      String newb = com.upokecenter.util.DataUtilities.GetUtf8String(EncodingToBytes(beo), true);
      Assert.assertEquals(b, newb);
    }

    public static void doTestString(String value) {
      String b = DataUtilities.GetUtf8Length(value, false) + ":" + value;
      CBORObject beo = EncodingFromBytes(com.upokecenter.util.DataUtilities.GetUtf8Bytes(b, true));
      Assert.assertEquals(value, beo.AsString());
      String newb = com.upokecenter.util.DataUtilities.GetUtf8String(EncodingToBytes(beo), true);
      Assert.assertEquals(b, newb);
    }

    @Test
    public void testLong() {
      doTestLong(0);
      doTestLong(-1);
      doTestLong(Integer.MIN_VALUE);
      doTestLong(Integer.MAX_VALUE);
      doTestLong(Long.MIN_VALUE);
      doTestLong(Long.MAX_VALUE);
    }

    @Test
    public void testList() {
      CBORObject beo = CBORObject.NewArray();
      beo.Add(CBORObject.FromObject(1));
      beo.Add(CBORObject.FromObject("two"));
      beo.Add(CBORObject.FromObject(3));
      beo.Add(CBORObject.FromObject("four"));
      Assert.assertEquals(4, beo.size());
      Assert.assertEquals(1, beo.get(0).AsInt64());
      Assert.assertEquals("two", beo.get(1).AsString());
      Assert.assertEquals(3, beo.get(2).AsInt64());
      Assert.assertEquals("four", beo.get(3).AsString());
      byte[] b = EncodingToBytes(beo);
      beo = EncodingFromBytes(b);
      Assert.assertEquals(4, beo.size());
      Assert.assertEquals(1, beo.get(0).AsInt64());
      Assert.assertEquals("two", beo.get(1).AsString());
      Assert.assertEquals(3, beo.get(2).AsInt64());
      Assert.assertEquals("four", beo.get(3).AsString());
    }

    @Test
    public void testDictionary() {
      CBORObject beo = CBORObject.NewMap();
      beo.set("zero",CBORObject.FromObject(1));
      beo.set("one",CBORObject.FromObject("two"));
      beo.set("two",CBORObject.FromObject(3));
      beo.set("three",CBORObject.FromObject("four"));
      Assert.assertEquals(4, beo.size());
      Assert.assertEquals(1, beo.get("zero").AsInt64());
      Assert.assertEquals("two", beo.get("one").AsString());
      Assert.assertEquals(3, beo.get("two").AsInt64());
      Assert.assertEquals("four", beo.get("three").AsString());
      byte[] b = EncodingToBytes(beo);
      beo = EncodingFromBytes(b);
      Assert.assertEquals(4, beo.size());
      Assert.assertEquals(1, beo.get("zero").AsInt64());
      Assert.assertEquals("two", beo.get("one").AsString());
      Assert.assertEquals(3, beo.get("two").AsInt64());
      Assert.assertEquals("four", beo.get("three").AsString());
    }

    @Test
    public void testString() {
      doTestString("");
      doTestString(" ");
      doTestString("test");

  doTestString("testoifdoifdodfioidfifdidfoiidofiosidoiofdsoiiofdsiofdiosiodfiosdoiffiodsiosdfiods"
);
      doTestString("te\u007fst");
      doTestString("te\u0080st");
      doTestString("te\u3000st");
      doTestString("te\u07ffst");
      doTestString("te\u0800st");
      doTestString("te\uffffst");
      doTestString("te\ud7ffst");
      doTestString("te\ue000st");
      doTestString("te\ud800\udc00st");
      doTestString("te\udbff\udc00st");
      doTestString("te\ud800\udfffst");
      doTestString("te\udbff\udfffst");
    }
  }
