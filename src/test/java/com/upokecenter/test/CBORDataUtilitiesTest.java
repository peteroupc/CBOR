package com.upokecenter.test;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.cbor.*;

  public class CBORDataUtilitiesTest {
    @Test
    public void TestParseJSONNumber() {
      if ((CBORDataUtilities.ParseJSONNumber("100.", false, false))!=null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber("-100.", false, false))!=null)Assert.fail();
    if ((
CBORDataUtilities.ParseJSONNumber(
"100.e+20",
false,
false)) != null)Assert.fail();
    if ((
CBORDataUtilities.ParseJSONNumber(
"-100.e20",
false,
false)) != null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber("100.e20", false, false))!=null)Assert.fail();

      if ((CBORDataUtilities.ParseJSONNumber("+0.1", false, false))!=null)Assert.fail();

      if ((CBORDataUtilities.ParseJSONNumber("0.", false, false))!=null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber("-0.", false, false))!=null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber("0.e+20", false, false))!=null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber("-0.e20", false, false))!=null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber("0.e20", false, false))!=null)Assert.fail();

      if ((CBORDataUtilities.ParseJSONNumber(null, false, false)) != null)Assert.fail();
  if ((
CBORDataUtilities.ParseJSONNumber(
"",
false,
false)) != null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber("xyz", false, false))!=null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber("true", false, false))!=null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber(".1", false, false))!=null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber("0..1", false, false))!=null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber("0xyz", false, false))!=null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber("0.1xyz", false, false))!=null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber("0.xyz", false, false))!=null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber("0.5exyz", false, false))!=null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber("0.5q+88", false, false))!=null)Assert.fail();
      if ((CBORDataUtilities.ParseJSONNumber("0.5ee88", false, false))!=null)Assert.fail();
    if ((
CBORDataUtilities.ParseJSONNumber(
"0.5e+xyz",
false,
false)) != null)Assert.fail();
  if ((
CBORDataUtilities.ParseJSONNumber(
"0.5e+88xyz",
false,
false)) != null)Assert.fail();
      CBORObject cbor;
      cbor =
        CBORDataUtilities.ParseJSONNumber(
"1e+99999999999999999999999999",
false,
false);
      if (!(cbor != null))Assert.fail();
      if (cbor.CanFitInDouble())Assert.fail();
      TestCommon.AssertRoundTrip(cbor);
    }
  }
