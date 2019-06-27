using System;
using NUnit.Framework;
using PeterO.Cbor;

namespace Test {
  [TestFixture]
  public class CBORExceptionTest {
    [Test]
    public void TestConstructor() {
      Assert.Throws<CBORException>(()=>throw new CBORException("Test exception"));
    }
  }
}
