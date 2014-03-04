package com.upokecenter.test;

import java.io.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.cbor.*;

  public class BEncodingTest {
    private static CBORObject EncodingFromBytes(byte[] b) {
      try {
        java.io.ByteArrayInputStream s=null;
try {
s=new ByteArrayInputStream(b);

          return BEncoding.Read(s);
}
finally {
try { if(s!=null)s.close(); } catch (IOException ex){}
}
      } catch (IOException ex) {
        throw new CBORException("", ex);
      }
    }

    private static byte[] EncodingToBytes(CBORObject b) {
      try {
        java.io.ByteArrayOutputStream s=null;
try {
s=new ByteArrayOutputStream();

          BEncoding.Write(b, s);
          return s.toByteArray();
}
finally {
try { if(s!=null)s.close(); } catch (IOException ex){}
}
      } catch (IOException ex) {
        throw new CBORException("", ex);
      }
    }

    /**
     * Not documented yet.
     * @param value A 64-bit signed integer.
     */
public void doTestLong(long value) {
      String b = "i" + value + "e";
      CBORObject beo = EncodingFromBytes(DataUtilities.GetUtf8Bytes(b,true));
      Assert.assertEquals(value, beo.AsInt64());
      String newb = DataUtilities.GetUtf8String(EncodingToBytes(beo),true);
      Assert.assertEquals(b, newb);
    }

    /**
     * Not documented yet.
     * @param value A string object.
     */
public void doTestString(String value) {
      String b = DataUtilities.GetUtf8Length(value, false) + ":" + value;
      CBORObject beo = EncodingFromBytes(DataUtilities.GetUtf8Bytes(b,true));
      Assert.assertEquals(value, beo.AsString());
      String newb = DataUtilities.GetUtf8String(EncodingToBytes(beo),true);
      Assert.assertEquals(b, newb);
    }

    /**
     * Not documented yet.
     */
@Test
    public void testLong() {
      this.doTestLong(0);
      this.doTestLong(-1);
      this.doTestLong(Integer.MIN_VALUE);
      this.doTestLong(Integer.MAX_VALUE);
      this.doTestLong(Long.MIN_VALUE);
      this.doTestLong(Long.MAX_VALUE);
    }

    /**
     * Not documented yet.
     */
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

    /**
     * Not documented yet.
     */
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

    /**
     * Not documented yet.
     */
@Test
    public void testString() {
      this.doTestString("");
      this.doTestString(" ");
      this.doTestString("test");
      this.doTestString("testoifdoifdodfioidfifdidfoiidofiosidoiofdsoiiofdsiofdiosiodfiosdoiffiodsiosdfiods");
      this.doTestString("te\u007fst");
      this.doTestString("te\u0080st");
      this.doTestString("te\u3000st");
      this.doTestString("te\u07ffst");
      this.doTestString("te\u0800st");
      this.doTestString("te\uffffst");
      this.doTestString("te\ud7ffst");
      this.doTestString("te\ue000st");
      this.doTestString("te\ud800\udc00st");
      this.doTestString("te\udbff\udc00st");
      this.doTestString("te\ud800\udfffst");
      this.doTestString("te\udbff\udfffst");
    }
  }
