using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO.Cbor;

namespace Test {
  [TestClass]
  public class CBORDataUtilitiesTest {
    [TestMethod]
    public void TestParseJSONNumber() {
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber(null, false, false));
  Assert.IsNull(
CBORDataUtilities.ParseJSONNumber(
String.Empty,
false,
false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("xyz", false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("true", false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber(".1", false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0..1", false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0xyz", false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0.1xyz", false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0.xyz", false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0.5exyz", false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0.5q+88", false, false));
      Assert.IsNull(CBORDataUtilities.ParseJSONNumber("0.5ee88", false, false));
    Assert.IsNull(
CBORDataUtilities.ParseJSONNumber(
"0.5e+xyz",
false,
false));
  Assert.IsNull(
CBORDataUtilities.ParseJSONNumber(
"0.5e+88xyz",
false,
false));
      CBORObject cbor;
      cbor =
        CBORDataUtilities.ParseJSONNumber(
"1e+99999999999999999999999999",
false,
false);
      Assert.IsTrue(cbor != null);
      Assert.IsFalse(cbor.CanFitInDouble());
      TestCommon.AssertRoundTrip(cbor);
    }
  }
}
