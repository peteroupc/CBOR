using System;
using System.IO;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;

namespace Test {
  [TestFixture]
  public class BEncodingTest {
    private static CBORObject EncodingFromBytes(byte[] b) {
      try {
        using var s = new Test.DelayingStream(b);
        return BEncoding.Read(s);
      } catch (IOException ex) {
        throw new CBORException(String.Empty, ex);
      }
    }

    private static byte[] EncodingToBytes(CBORObject b) {
      try {
        using var ms = new Test.DelayingStream();
        BEncoding.Write(b, ms);
        return ms.ToArray();
      } catch (IOException ex) {
        throw new CBORException(String.Empty, ex);
      }
    }

    public static void DoTestLong(long value) {
      string b = "i" + TestCommon.LongToString(value) + "e";
      CBORObject beo = EncodingFromBytes(DataUtilities.GetUtf8Bytes(b,
            false));
      Assert.AreEqual(value, beo.AsNumber().ToInt64Checked());
      string newb = DataUtilities.GetUtf8String(EncodingToBytes(beo), false);
      Assert.AreEqual(b, newb);
    }

    public static void DoTestString(string value) {
      string b = DataUtilities.GetUtf8Length(value, false) + ":" + value;
      CBORObject beo = EncodingFromBytes(DataUtilities.GetUtf8Bytes(b,
            false));
      Assert.AreEqual(value, beo.AsString());
      string newb = DataUtilities.GetUtf8String(EncodingToBytes(beo), false);
      Assert.AreEqual(b, newb);
    }

    [Test]
    public void TestLong() {
      DoTestLong(0);
      DoTestLong(-1);
      DoTestLong(Int32.MinValue);
      DoTestLong(Int32.MaxValue);
      DoTestLong(Int64.MinValue);
      DoTestLong(Int64.MaxValue);
    }

    [Test]
    public void TestList() {
      var beo = CBORObject.NewArray();
      _ = beo.Add(ToObjectTest.TestToFromObjectRoundTrip(1));
      _ = beo.Add(ToObjectTest.TestToFromObjectRoundTrip("two"));
      _ = beo.Add(ToObjectTest.TestToFromObjectRoundTrip(3));
      _ = beo.Add(ToObjectTest.TestToFromObjectRoundTrip("four"));
      Assert.AreEqual(4, beo.Count);
      Assert.AreEqual(1, beo[0].AsNumber().ToInt64Checked());
      {
        string stringTemp = beo[1].AsString();
        Assert.AreEqual(
          "two",
          stringTemp);
      }
      Assert.AreEqual(3, beo[2].AsNumber().ToInt64Checked());
      {
        string stringTemp = beo[3].AsString();
        Assert.AreEqual(
          "four",
          stringTemp);
      }
      byte[] b = EncodingToBytes(beo);
      beo = EncodingFromBytes(b);
      Assert.AreEqual(4, beo.Count);
      Assert.AreEqual(1, beo[0].AsNumber().ToInt64Checked());
      {
        string stringTemp = beo[1].AsString();
        Assert.AreEqual(
          "two",
          stringTemp);
      }
      Assert.AreEqual(3, beo[2].AsNumber().ToInt64Checked());
      {
        string stringTemp = beo[3].AsString();
        Assert.AreEqual(
          "four",
          stringTemp);
      }
    }

    [Test]
    public void TestDictionary() {
      var beo = CBORObject.NewMap();
      beo["zero"] = ToObjectTest.TestToFromObjectRoundTrip(1);
      beo["one"] = ToObjectTest.TestToFromObjectRoundTrip("two");
      beo["two"] = ToObjectTest.TestToFromObjectRoundTrip(3);
      beo["three"] = ToObjectTest.TestToFromObjectRoundTrip("four");
      Assert.AreEqual(4, beo.Count);
      Assert.AreEqual(1, beo["zero"].AsNumber().ToInt64Checked());
      {
        string stringTemp = beo["one"].AsString();
        Assert.AreEqual(
          "two",
          stringTemp);
      }
      Assert.AreEqual(3, beo["two"].AsNumber().ToInt64Checked());
      {
        string stringTemp = beo["three"].AsString();
        Assert.AreEqual(
          "four",
          stringTemp);
      }
      byte[] b = EncodingToBytes(beo);
      beo = EncodingFromBytes(b);
      Assert.AreEqual(4, beo.Count);
      Assert.AreEqual(1, beo["zero"].AsNumber().ToInt64Checked());
      {
        string stringTemp = beo["one"].AsString();
        Assert.AreEqual(
          "two",
          stringTemp);
      }
      Assert.AreEqual(3, beo["two"].AsNumber().ToInt64Checked());
      {
        string stringTemp = beo["three"].AsString();
        Assert.AreEqual(
          "four",
          stringTemp);
      }
    }

    [Test]
    public void TestString() {
      DoTestString(String.Empty);
      DoTestString(" ");
      DoTestString("test");

      DoTestString(
        TestCommon.Repeat("three", 15));
      DoTestString("te\u007fst");
      DoTestString("te\u0080st");
      DoTestString("te\u3000st");
      DoTestString("te\u07ffst");
      DoTestString("te\u0800st");
      DoTestString("te\uffffst");
      DoTestString("te\ud7ffst");
      DoTestString("te\ue000st");
      DoTestString("te\ud800\udc00st");
      DoTestString("te\udbff\udc00st");
      DoTestString("te\ud800\udfffst");
      DoTestString("te\udbff\udfffst");
    }
  }
}
