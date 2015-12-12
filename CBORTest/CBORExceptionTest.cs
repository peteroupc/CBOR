using System;
using NUnit.Framework;
using PeterO.Cbor;

namespace Test {
  [TestFixture]
  public class CBORExceptionTest {
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestConstructor() {
      throw new CBORException("Test exception");
    }
  }
}
