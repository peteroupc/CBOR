using System;
using NUnit.Framework;
using PeterO.Cbor;

namespace Test {
  [TestFixture]
  public class CBORDataUtilitiesTest {
    [Test]
    public void TestParseJSONNumberNegativeZero() {
      {
string stringTemp = CBORDataUtilities.ParseJSONNumber("-0").ToString();
Assert.AreEqual(
"0",
stringTemp);
}
      {
string stringTemp = CBORDataUtilities.ParseJSONNumber("-0E+0").ToString();
Assert.AreEqual(
"0",
stringTemp);
}
      {
string stringTemp = CBORDataUtilities.ParseJSONNumber("-0E-0").ToString();
Assert.AreEqual(
"0",
stringTemp);
}
      {
string stringTemp = CBORDataUtilities.ParseJSONNumber("-0E-1").ToString();
Assert.AreEqual(
"0.0",
stringTemp);
}
      {
string stringTemp = CBORDataUtilities.ParseJSONNumber("-0.00").ToString();
Assert.AreEqual(
"0.00",
stringTemp);
}
      {
string stringTemp = CBORDataUtilities.ParseJSONNumber("-0.00E+0").ToString();
Assert.AreEqual(
"0.00",
stringTemp);
}
      {
string stringTemp = CBORDataUtilities.ParseJSONNumber("-0.00E-0").ToString();
Assert.AreEqual(
"0.00",
stringTemp);
}
      {
string stringTemp = CBORDataUtilities.ParseJSONNumber("-0.00E-1").ToString();
Assert.AreEqual(
"0.000",
stringTemp);
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
      if (
CBORDataUtilities.ParseJSONNumber(
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
    if (
CBORDataUtilities.ParseJSONNumber(
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
      TestCommon.CompareTestEqual(
CBORObject.FromObject(230),
CBORDataUtilities.ParseJSONNumber("23.0e01"));
      TestCommon.CompareTestEqual(
CBORObject.FromObject(23),
CBORDataUtilities.ParseJSONNumber("23.0e00"));
      CBORObject cbor;
      cbor = CBORDataUtilities.ParseJSONNumber(
"1e+99999999999999999999999999",
false,
false);
      Assert.IsTrue(cbor != null);
      Assert.IsFalse(cbor.CanFitInDouble());
      TestCommon.AssertRoundTrip(cbor);
    }
  }
}
