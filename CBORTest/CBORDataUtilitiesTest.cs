using System;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;
using PeterO.Numbers;

namespace Test {
  [TestFixture]
  public class CBORDataUtilitiesTest {
    private static void AssertNegative(CBORObject obj) {
      Assert.IsTrue(obj.AsNumber().IsNegative());
      CBORTestCommon.AssertRoundTrip(obj);
    }

    // testing obsolete method
    #pragma warning disable CS0618
    [Test]
    public void TestPreserveNegativeZero() {
      CBORObject cbor;
      cbor = CBORDataUtilities.ParseJSONNumber("-0", false, false, true);
      {
        string stringTemp = cbor.ToObject(typeof(EDecimal)).ToString();
        Assert.AreEqual(
          "-0",
          stringTemp);
      }
      cbor = CBORDataUtilities.ParseJSONNumber("-0e-1", false, false, true);
      {
        string stringTemp = cbor.ToObject(typeof(EDecimal)).ToString();
        Assert.AreEqual(
          "-0.0",
          stringTemp);
      }
      cbor = CBORDataUtilities.ParseJSONNumber("-0e1", false, false, true);
      {
        string stringTemp = cbor.ToObject(typeof(EDecimal)).ToString();
        Assert.AreEqual(
          "-0E+1",
          stringTemp);
      }
      cbor = CBORDataUtilities.ParseJSONNumber("-0.0e1", false, false, true);
      {
        string stringTemp = cbor.ToObject(typeof(EDecimal)).ToString();
        Assert.AreEqual(
          "-0",
          stringTemp);
      }
      cbor = CBORDataUtilities.ParseJSONNumber("-0.0", false, false, true);
      {
        string stringTemp = cbor.ToObject(typeof(EDecimal)).ToString();
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
        AssertNegative(cbor);
      }
    }

    [Test]
    public void TestParseJSONNumberSubstring() {
      string tstr =

  "-3.00931381333368754713014659613049757554804012787921371662913692598770508705049030832574634419795955864174175076186656951904296875000E-49";
      try {
        CBORDataUtilities.ParseJSONNumber(
          "xyzxyz" + tstr,
          6,
          tstr.Length,
          JSONOptions.Default);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
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
        var jsonDecimal = (EDecimal)CBORDataUtilities
          .ParseJSONNumber(strings[i]).ToObject(typeof(EDecimal));
        Assert.AreEqual(
          strings[i + 1],
          jsonDecimal.ToString());
      }
    }

    private static readonly string[] GoodJsonNumbers = {
      "5.2", "5e+1", "-5.2", "-5e+1",
      "5.2", "5e+01", "-5.2", "-5e+01",
      "5.20000", "5.000e+01", "-5.2000", "-5e+01",
      "5.000e-01", "-5e-01",
      "5.000e01", "-5e01",
    };

    private static readonly string[] BadJsonNumbers = {
      null, "100.", "-100.", "100.e+20", "-100.e+20",
      "100.e20", "+0.1", "0.", "-0.", "+0",
      "=0g.1", "0g.1", "0.e+20", "-0.e20", "-0.e+20",
      "0.e20", String.Empty, "xyz", "Infinity", "-Infinity",
      "true", ".1", ".01", "-.1", "-.01", "-xyz", "-true",
      "0..1", "-0..1", "0xyz", "-0xyz",
      "0.1xyz", "0.xyz", "0.5exyz", "0.5q+88",
      "0.5ee88", "-5e", "5e", "88ee99",
      "-5e-2x", "-5e+2x", "5e-2x", "5e+2x",
      "0.5e+xyz", "0.5e+88xyz",
      "00000", "00.5e+2", "00.5", "00.5e-2", "00.5e-999", "00.5e999",
      "00000", "00.5E+2", "00.5", "00.5E-2", "00.5E-999", "00.5e999",
      "00001", "01.5e+2", "01.5", "01.5e-2", "01.5e-999", "01.5e999",
      "00001", "01.5E+2", "01.5", "01.5E-2", "01.5E-999", "01.5e999",
      "--1", "--0", "--1.5E+2", "--1.5", "--1.5E-2", "--1.5E-999",
      "--1.5E999",
      "-00000", "-00.5e+2", "-00.5", "-00.5e-2", "-00.5e-999", "-00.5e999",
      "-00000", "-00.5E+2", "-00.5", "-00.5E-2", "-00.5E-999", "-00.5E999",
      "-00001", "-01.5e+2", "-01.5", "-01.5e-2", "-01.5e-999", "-01.5e999",
      "-00001", "-01.5E+2", "-01.5", "-01.5E-2", "-01.5E-999", "-01.5E999",
      "0x1", "0xf", "0x20", "0x01", ".2", ".05",
      "-0x1", "-0xf", "-0x20", "-0x01", "-.2", "-.05",
      "23.", "23.e-2", "23.e0", "23.e1", "0.",
    };

    [Test]
    public void TestParseJSONNumberObsolete() {
      foreach (var str in BadJsonNumbers) {
        if (CBORDataUtilities.ParseJSONNumber(str, false, false, true) !=
          null) {
          Assert.Fail(str);
        }
      }
      foreach (var str in GoodJsonNumbers) {
        if (CBORDataUtilities.ParseJSONNumber(str, false, false) == null) {
          Assert.Fail(str);
        }
      }
    }

    [Test]
    public void TestParseJSONNumber() {
      foreach (var str in BadJsonNumbers) {
        if (CBORDataUtilities.ParseJSONNumber(str) != null) {
          Assert.Fail(str);
        }
        if (CBORDataUtilities.ParseJSONNumber(str, JSONOptions.Default) !=
null) {
          Assert.Fail(str);
        }
        if (CBORDataUtilities.ParseJSONNumber(str, false, false, true) !=
          null) {
          Assert.Fail(str);
        }
        if (CBORDataUtilities.ParseJSONNumber(str, false, false, false) !=
          null) {
          Assert.Fail(str);
        }
      }
      CBORObject cbor = CBORDataUtilities.ParseJSONNumber("2e-2147483648");
      CBORTestCommon.AssertJSONSer(cbor, "2E-2147483648");
      foreach (var str in GoodJsonNumbers) {
        if (CBORDataUtilities.ParseJSONNumber(str) == null) {
          Assert.Fail(str);
        }
        if (CBORDataUtilities.ParseJSONNumber(str, false, false, true) ==
          null) {
          Assert.Fail(str);
        }
        if (CBORDataUtilities.ParseJSONNumber(str, false, false, false) ==
          null) {
          Assert.Fail(str);
        }
      }
      TestCommon.CompareTestEqual(
        ToObjectTest.TestToFromObjectRoundTrip(230).AsNumber(),
        CBORDataUtilities.ParseJSONNumber("23.0e01").AsNumber());
      TestCommon.CompareTestEqual(
        ToObjectTest.TestToFromObjectRoundTrip(23).AsNumber(),
        CBORDataUtilities.ParseJSONNumber("23.0e00").AsNumber());
      cbor = CBORDataUtilities.ParseJSONNumber(
          "1e+99999999999999999999999999");
      Assert.IsTrue(cbor != null);
      Assert.IsFalse(cbor.AsNumber().CanFitInDouble());
      CBORTestCommon.AssertRoundTrip(cbor);
    }
  }
}
