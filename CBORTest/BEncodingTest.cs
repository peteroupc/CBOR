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
        using (var s = new MemoryStream()) {
          BEncoding.Write(b, s);
          return s.ToArray();
        }
      } catch (IOException ex) {
        throw new CBORException(String.Empty, ex);
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A 64-bit signed integer.</param>
    public void doTestLong(long value) {
      String b = "i" + value + "e";
      CBORObject beo = EncodingFromBytes(Encoding.UTF8.GetBytes(b));
      Assert.AreEqual(value, beo.AsInt64());
      String newb = Encoding.UTF8.GetString(EncodingToBytes(beo));
      Assert.AreEqual(b, newb);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A String object.</param>
    public void doTestString(String value) {
      String b = DataUtilities.GetUtf8Length(value, false) + ":" + value;
      CBORObject beo = EncodingFromBytes(Encoding.UTF8.GetBytes(b));
      Assert.AreEqual(value, beo.AsString());
      String newb = Encoding.UTF8.GetString(EncodingToBytes(beo));
      Assert.AreEqual(b, newb);
    }

    /// <summary>Not documented yet.</summary>
    [Test]
    public void testLong() {
      this.doTestLong(0);
      this.doTestLong(-1);
      this.doTestLong(Int32.MinValue);
      this.doTestLong(Int32.MaxValue);
      this.doTestLong(Int64.MinValue);
      this.doTestLong(Int64.MaxValue);
    }

    /// <summary>Not documented yet.</summary>
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

    /// <summary>Not documented yet.</summary>
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

    /// <summary>Not documented yet.</summary>
    [Test]
    public void testString() {
      this.doTestString(String.Empty);
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
}
