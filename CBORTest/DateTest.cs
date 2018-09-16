using System;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;
using PeterO.Numbers;
namespace Test {
  [TestFixture]
  public class DateTest {
    [Test]
    public void TestDate() {
      var cbor = CBORObject.FromObjectAndTag(
        -67596566998,
        1);
    }
  }
}
