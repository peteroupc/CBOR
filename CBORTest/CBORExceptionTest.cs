using System;
using NUnit.Framework;
using PeterO.Cbor;

namespace Test {
  [TestFixture]
  public class CBORExceptionTest {
    [Test]
    public void TestConstructor() {
     try {
        throw new CBORException("Test exception");
      } catch (CBORException) {
         // expected exception
      }
    }
  }
}
