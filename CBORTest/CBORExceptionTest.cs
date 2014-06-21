using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO.Cbor;

namespace Test {
  [TestClass]
  public class CBORExceptionTest {
    [TestMethod]
    [ExpectedException(typeof(CBORException))]
    public void TestConstructor() {
      throw new CBORException("Test exception");
    }
  }
}
