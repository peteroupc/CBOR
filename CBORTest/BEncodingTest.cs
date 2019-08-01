using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;

namespace Test {
  [TestFixture]
  public class BEncodingTest {
    private static CBORObject EncodingFromBytes(byte[] b) {
      try {
        using (var s = new MemoryStream(b)) {
          return BEncoding.Read(s);
        }
      } catch (IOException ex) {
        throw new CBORException(String.Empty, ex);
      }
    }

    private static byte[] EncodingToBytes(CBORObject b) {
      try {
        using (var ms = new MemoryStream()) {
          BEncoding.Write(b, ms);
          return ms.ToArray();
        }
      } catch (IOException ex) {
        throw new CBORException(String.Empty, ex);
      }
    }

    public static void doTestLong(long value) {
      String b = "i" + TestCommon.LongToString(value) + "e";
      CBORObject beo = EncodingFromBytes(DataUtilities.GetUtf8Bytes(b, false));
      Assert.AreEqual(value, beo.AsInt64());
      String newb = DataUtilities.GetUtf8String(EncodingToBytes(beo), false);
      Assert.AreEqual(b, newb);
    }

    public static void doTestString(String value) {
      String b = DataUtilities.GetUtf8Length(value, false) + ":" + value;
      CBORObject beo = EncodingFromBytes(DataUtilities.GetUtf8Bytes(b, false));
      Assert.AreEqual(value, beo.AsString());
      String newb = DataUtilities.GetUtf8String(EncodingToBytes(beo), false);
      Assert.AreEqual(b, newb);
    }

    [Test]
    public void testLong() {
      doTestLong(0);
      doTestLong(-1);
      doTestLong(Int32.MinValue);
      doTestLong(Int32.MaxValue);
      doTestLong(Int64.MinValue);
      doTestLong(Int64.MaxValue);
    }

    [Test]
    public void testList() {
      CBORObject beo = CBORObject.NewArray();
      beo.Add(CBORObject.FromObject(1));
      beo.Add(CBORObject.FromObject("two"));
      beo.Add(CBORObject.FromObject(3));
      beo.Add(CBORObject.FromObject("four"));
      Assert.AreEqual(4, beo.Count);
      Assert.AreEqual(1, beo[0].AsInt64());
      {
        string stringTemp = beo[1].AsString();
Assert.AreEqual(
  "two",
  stringTemp);
}
      Assert.AreEqual(3, beo[2].AsInt64());
      {
        string stringTemp = beo[3].AsString();
Assert.AreEqual(
  "four",
  stringTemp);
}
      byte[] b = EncodingToBytes(beo);
      beo = EncodingFromBytes(b);
      Assert.AreEqual(4, beo.Count);
      Assert.AreEqual(1, beo[0].AsInt64());
      {
        string stringTemp = beo[1].AsString();
Assert.AreEqual(
  "two",
  stringTemp);
}
      Assert.AreEqual(3, beo[2].AsInt64());
      {
        string stringTemp = beo[3].AsString();
Assert.AreEqual(
  "four",
  stringTemp);
}
    }

    [Test]
    public void testDictionary() {
      CBORObject beo = CBORObject.NewMap();
      beo["zero"] = CBORObject.FromObject(1);
      beo["one"] = CBORObject.FromObject("two");
      beo["two"] = CBORObject.FromObject(3);
      beo["three"] = CBORObject.FromObject("four");
      Assert.AreEqual(4, beo.Count);
      Assert.AreEqual(1, beo["zero"].AsInt64());
      {
        string stringTemp = beo["one"].AsString();
Assert.AreEqual(
  "two",
  stringTemp);
}
      Assert.AreEqual(3, beo["two"].AsInt64());
      {
        string stringTemp = beo["three"].AsString();
Assert.AreEqual(
  "four",
  stringTemp);
}
      byte[] b = EncodingToBytes(beo);
      beo = EncodingFromBytes(b);
      Assert.AreEqual(4, beo.Count);
      Assert.AreEqual(1, beo["zero"].AsInt64());
      {
        string stringTemp = beo["one"].AsString();
Assert.AreEqual(
  "two",
  stringTemp);
}
      Assert.AreEqual(3, beo["two"].AsInt64());
      {
        string stringTemp = beo["three"].AsString();
Assert.AreEqual(
  "four",
  stringTemp);
}
    }

    [Test]
    public void testString() {
      doTestString(String.Empty);
      doTestString(" ");
      doTestString("test");

  doTestString(TestCommon.Repeat("three", 15));
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
}
