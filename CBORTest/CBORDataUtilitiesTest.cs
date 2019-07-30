using System;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;
using PeterO.Numbers;

namespace Test {
  [TestFixture]
  public class CBORDataUtilitiesTest {
    private void AssertNegative(CBORObject obj) {
      Assert.IsTrue(obj.IsNegative);
      CBORTestCommon.AssertRoundTrip(obj);
    }
    [Test]
    public void TestPreserveNegativeZero() {
      CBORObject cbor;
      cbor = CBORDataUtilities.ParseJSONNumber("-0", false, false, true);
      {
        string stringTemp = cbor.AsEDecimal().ToString();
        Assert.AreEqual(
          "-0",
          stringTemp);
      }
      cbor = CBORDataUtilities.ParseJSONNumber("-0e-1", false, false, true);
      {
        string stringTemp = cbor.AsEDecimal().ToString();
        Assert.AreEqual(
          "-0.0",
          stringTemp);
      }
      cbor = CBORDataUtilities.ParseJSONNumber("-0e1", false, false, true);
      {
        string stringTemp = cbor.AsEDecimal().ToString();
        Assert.AreEqual(
          "-0E+1",
          stringTemp);
      }
      cbor = CBORDataUtilities.ParseJSONNumber("-0.0e1", false, false, true);
      {
        string stringTemp = cbor.AsEDecimal().ToString();
        Assert.AreEqual(
          "-0",
          stringTemp);
      }
      cbor = CBORDataUtilities.ParseJSONNumber("-0.0", false, false, true);
      {
        string stringTemp = cbor.AsEDecimal().ToString();
        Assert.AreEqual(
          "-0.0",
          stringTemp);
      }
      var assertNegatives = new string[] {
        "-0",
        "-0.0",
        "-0.0000",
        "-0e0",
        "-0e+1",
        "-0e-1",
        "-0e+999999999999",
        "-0e-999999999999",
        "-0.0e0",
        "-0.0e+1",
        "-0.0e-1",
        "-0.0e+999999999999",
        "-0.0e-999999999999",
        "-0.000e0",
        "-0.000e+0",
        "-0.000e-0",
        "-0.000e1",
        "-0.000e+1",
        "-0.000e-1",
        "-0.000e+999999999999",
        "-0.000e-999999999999",
      };
      foreach (var str in assertNegatives) {
        cbor = CBORDataUtilities.ParseJSONNumber(str, false, false, true);
        this.AssertNegative(cbor);
      }
    }
    [Test]
    public void TestParseJSONNumberNegativeZero() {
      var strings = new string[] {
        "-0", "0", "-0E+0", "0", "-0E-0", "0", "-0E-1", "0.0",
        "-0.00", "0.00", "-0.00E+0", "0.00", "-0.00E-0", "0.00",
        "-0.00E-1", "0.000",
      };
      for (var i = 0; i < strings.Length; i += 2) {
        EDecimal jsonDecimal = CBORDataUtilities
                  .ParseJSONNumber(strings[i]).AsEDecimal();
        Assert.AreEqual(
          strings[i + 1],
          jsonDecimal.ToString());
      }
    }

    [Test]
    public void TestParseJSONNumber() {
      if (CBORDataUtilities.ParseJSONNumber("100.", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("-100.", false, false) != null) {
        Assert.Fail();
      }
      if (
       CBORDataUtilities.ParseJSONNumber(
         "100.e+20",
         false,
         false) != null) {
        Assert.Fail();
      }
      if (
       CBORDataUtilities.ParseJSONNumber(
         "-100.e20",
         false,
         false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("100.e20", false, false) != null) {
        Assert.Fail();
      }

      if (CBORDataUtilities.ParseJSONNumber("+0.1", false, false) != null) {
        Assert.Fail();
      }

      if (CBORDataUtilities.ParseJSONNumber("0.", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("-0.", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0g.1", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0.e+20", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("-0.e20", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0.e20", false, false) != null) {
        Assert.Fail();
      }

      if (CBORDataUtilities.ParseJSONNumber(null, false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber(
        String.Empty,
        false,
        false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("xyz", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("true", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber(".1", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0..1", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0xyz", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0.1xyz", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0.xyz", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0.5exyz", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0.5q+88", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0.5ee88", false, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("-5e") != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("-5e-2x") != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("-5e+2x") != null) {
        Assert.Fail();
      }
      CBORObject cbor = CBORDataUtilities.ParseJSONNumber("2e-2147483648");
      CBORTestCommon.AssertJSONSer(cbor, "2E-2147483648");
      if (CBORDataUtilities.ParseJSONNumber(
        "0.5e+xyz",
        false,
        false) != null) {
        Assert.Fail();
      }
      if (
       CBORDataUtilities.ParseJSONNumber(
         "0.5e+88xyz",
         false,
         false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0000") != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0x1") != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0xf") != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0x20") != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0x01") != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber(".2") != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber(".05") != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("-.2") != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("-.05") != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("23.") != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("23.e0") != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("23.e1") != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("0.") != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("5.2", true, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("5e+1", true, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("-5.2", true, false) != null) {
        Assert.Fail();
      }
      if (CBORDataUtilities.ParseJSONNumber("-5e+1", true, false) != null) {
        Assert.Fail();
      }

      TestCommon.CompareTestEqual(
  ToObjectTest.TestToFromObjectRoundTrip(230).AsNumber(),
  CBORDataUtilities.ParseJSONNumber("23.0e01").AsNumber());
      TestCommon.CompareTestEqual(
  ToObjectTest.TestToFromObjectRoundTrip(23).AsNumber(),
  CBORDataUtilities.ParseJSONNumber("23.0e00").AsNumber());
      cbor = CBORDataUtilities.ParseJSONNumber(
        "1e+99999999999999999999999999",
        false,
        false);
      Assert.IsTrue(cbor != null);
      Assert.IsFalse(cbor.CanFitInDouble());
      CBORTestCommon.AssertRoundTrip(cbor);
    }
  }
}
