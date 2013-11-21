using NUnit.Framework;
using System.Text;
using System;
using System.IO;
using PeterO;

namespace Test {


  [TestFixture]
  public class BEncodingTest {

    private static CBORObject EncodingFromBytes(byte[] b) {
      try {
        using (var s = new MemoryStream(b)) {
          return BEncoding.Read(s);
        }
      } catch (IOException ex) {
        throw new CBORException("", ex);
      }
    }
    private static byte[] EncodingToBytes(CBORObject b) {
      try {
        using (var s = new MemoryStream()) {
          BEncoding.Write(b, s);
          return s.ToArray();
        }
      } catch (IOException ex) {
        throw new CBORException("", ex);
      }
    }

    public void doTestLong(long value) {
      String b = "i" + value + "e";
      CBORObject beo = EncodingFromBytes(Encoding.UTF8.GetBytes(b));
      Assert.AreEqual(value, beo.AsInt64());
      String newb = Encoding.UTF8.GetString(EncodingToBytes(beo));
      Assert.AreEqual(b, newb);
    }
    public void doTestString(String value) {
      String b = DataUtilities.GetUtf8Length(value, false) + ":" + value;
      CBORObject beo = EncodingFromBytes(Encoding.UTF8.GetBytes(b));
      Assert.AreEqual(value, beo.AsString());
      String newb = Encoding.UTF8.GetString(EncodingToBytes(beo));
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
      Assert.AreEqual("two", beo[1].AsString());
      Assert.AreEqual(3, beo[2].AsInt64());
      Assert.AreEqual("four", beo[3].AsString());
      byte[] b = EncodingToBytes(beo);
      beo = EncodingFromBytes(b);
      Assert.AreEqual(4, beo.Count);
      Assert.AreEqual(1, beo[0].AsInt64());
      Assert.AreEqual("two", beo[1].AsString());
      Assert.AreEqual(3, beo[2].AsInt64());
      Assert.AreEqual("four", beo[3].AsString());
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
      Assert.AreEqual("two", beo["one"].AsString());
      Assert.AreEqual(3, beo["two"].AsInt64());
      Assert.AreEqual("four", beo["three"].AsString());
      byte[] b = EncodingToBytes(beo);
      beo = EncodingFromBytes(b);
      Assert.AreEqual(4, beo.Count);
      Assert.AreEqual(1, beo["zero"].AsInt64());
      Assert.AreEqual("two", beo["one"].AsString());
      Assert.AreEqual(3, beo["two"].AsInt64());
      Assert.AreEqual("four", beo["three"].AsString());
    }

    [Test]
    public void testString() {
      doTestString("");
      doTestString(" ");
      doTestString("test");
      doTestString("testoifdoifdodfioidfifdidfoiidofiosidoiofdsoiiofdsiofdiosiodfiosdoiffiodsiosdfiods");
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