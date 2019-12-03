using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;
using PeterO.Numbers;

namespace Test {
  [TestFixture]
#pragma warning disable CS0618
  public class CBORObjectTest {
    private static readonly string[] ValueJsonFails = {
      "\"\\uxxxx\"",
      "\"\\ud800\udc00\"",
      "\"\ud800\\udc00\"", "\"\\U0023\"", "\"\\u002x\"", "\"\\u00xx\"",
      "\"\\u0xxx\"", "\"\\u0\"", "\"\\u00\"", "\"\\u000\"", "trbb",
      "trub", "falsb", "nulb", "[true", "[true,", "[true]!",
      "[\"\ud800\\udc00\"]", "[\"\\ud800\udc00\"]",
      "[\"\\udc00\ud800\udc00\"]", "[\"\\ud800\ud800\udc00\"]",
      "[\"\\ud800\"]", "[1,2,", "[1,2,3", "{,\"0\":0,\"1\":1}",
      "{\"0\":0,,\"1\":1}", "{\"0\":0,\"1\":1,}", "[,0,1,2]", "[0,,1,2]",
      "[0,1,,2]", "[0,1,2,]", "[0001]", "{a:true}",
      "{\"a\"://comment\ntrue}", "{\"a\":/*comment*/true}", "{'a':true}",
      "{\"a\":'b'}", "{\"a\t\":true}", "{\"a\r\":true}", "{\"a\n\":true}",
      "['a']", "{\"a\":\"a\t\"}", "[\"a\\'\"]", "[NaN]", "[+Infinity]",
      "[-Infinity]", "[Infinity]", "{\"a\":\"a\r\"}", "{\"a\":\"a\n\"}",
      "[\"a\t\"]", "\"test\"\"", "\"test\"x", "\"test\"\u0300",
      "\"test\"\u0005", "[5]\"", "[5]x", "[5]\u0300", "[5]\u0005",
      "{\"test\":5}\"", "{\"test\":5}x", "{\"test\":5}\u0300",
      "{\"test\":5}\u0005", "true\"", "truex", "true}", "true\u0300",
      "true\u0005", "8024\"", "8024x", "8024}", "8024\u0300",
      "8024\u0005", "{\"test\":5}}", "{\"test\":5}{", "[5]]", "[5][",
      "0000", "0x1", "0xf", "0x20", "0x01",
      "-3x", "-3e89x",
      "0X1", "0Xf", "0X20", "0X01", ".2", ".05", "-.2",
      "-.05", "23.", "23.e0", "23.e1", "0.", "[0000]", "[0x1]",
      "[0xf]", "[0x20]", "[0x01]", "[.2]", "[.05]", "[-.2]", "[-.05]",
      "[23.]", "[23.e0]", "[23.e1]", "[0.]", "\"abc", "\"ab\u0004c\"",
      "\u0004\"abc\"",
      "[1,\u0004" + "2]",
    };

    private static readonly string[] ValueJsonSucceeds = {
      "[0]",
      "[0.1]",
      "[0.1001]",
      "[0.0]",
      "[-3 " + ",-5]",
      "[0.00]", "[0.000]", "[0.01]", "[0.001]", "[0.5]", "[0E5]",
      "[0E+6]", "[\"\ud800\udc00\"]", "[\"\\ud800\\udc00\"]",
      "[\"\\ud800\\udc00\ud800\udc00\"]", "23.0e01", "23.0e00", "[23.0e01]",
      "[23.0e00]", "0", "1", "0.2", "0.05", "-0.2", "-0.05",
    };

    private static readonly JSONOptions ValueNoDuplicateKeys = new
JSONOptions("allowduplicatekeys=false");

    internal static void CheckPropertyNames(
      object ao,
      PODOptions cc,
      string p1,
      string p2,
      string p3) {
      CBORObjectTest.CheckPropertyNames(
        CBORObject.FromObject(ao, cc),
        p1,
        p2,
        p3);
    }

    internal static void CheckArrayPropertyNames(
      CBORObject co,
      int expectedCount,
      string p1,
      string p2,
      string p3) {
      Assert.AreEqual(CBORType.Array, co.Type);
      Assert.AreEqual(expectedCount, co.Count);
      for (var i = 0; i < co.Count; ++i) {
        CBORObjectTest.CheckPropertyNames(co[i], p1, p2, p3);
      }
      CBORTestCommon.AssertRoundTrip(co);
    }

    internal static void CheckPODPropertyNames(
      CBORObject co,
      PODOptions cc,
      string p1,
      string p2,
      string p3) {
      Assert.AreEqual(CBORType.Map, co.Type);
      string keyName = cc.UseCamelCase ? "propValue" : "PropValue";
      if (!co.ContainsKey(keyName)) {
        Assert.Fail("Expected " + keyName + " to exist: " + co.ToString());
      }
      CBORObjectTest.CheckPropertyNames(co[keyName], p1, p2, p3);
    }

    internal static void CheckPODInDictPropertyNames(
      CBORObject co,
      string p1,
      string p2,
      string p3) {
      Assert.AreEqual(CBORType.Map, co.Type);
      if (!co.ContainsKey("PropValue")) {
        Assert.Fail("Expected PropValue to exist: " + co.ToString());
      }
      CBORObjectTest.CheckPropertyNames(co["PropValue"], p1, p2, p3);
    }

    internal static void CheckPropertyNames(
      CBORObject o,
      string p1,
      string p2,
      string p3) {
      Assert.IsFalse(o.ContainsKey("PrivatePropA"));
      Assert.IsFalse(o.ContainsKey("privatePropA"));
      Assert.IsFalse(o.ContainsKey("StaticPropA"));
      Assert.IsFalse(o.ContainsKey("staticPropA"));
      Assert.AreEqual(CBORType.Map, o.Type);
      if (!o.ContainsKey(p1)) {
        Assert.Fail("Expected " + p1 + " to exist: " + o.ToString());
      }
      if (!o.ContainsKey(p2)) {
        Assert.Fail("Expected " + p2 + " to exist: " + o.ToString());
      }
      if (!o.ContainsKey(p3)) {
        Assert.Fail("Expected " + p3 + " to exist: " + o.ToString());
      }
      CBORTestCommon.AssertRoundTrip(o);
    }

    internal static void CheckPropertyNames(object ao) {
      var valueCcTF = new PODOptions(true, false);
      var valueCcFF = new PODOptions(false, false);
      var valueCcFT = new PODOptions(false, true);
      var valueCcTT = new PODOptions(true, true);
      CBORObjectTest.CheckPropertyNames(
        ao,
        valueCcTF,
        "PropA",
        "PropB",
        "IsPropC");
      //--
      CBORObjectTest.CheckPropertyNames(
        ao,
        valueCcFF,
        "PropA",
        "PropB",
        "IsPropC");
      CBORObjectTest.CheckPropertyNames(
        ao,
        valueCcFT,
        "propA",
        "propB",
        "propC");
      CBORObjectTest.CheckPropertyNames(
        ao,
        valueCcTT,
        "propA",
        "propB",
        "propC");
    }

    public static CBORObject GetNumberData() {
      return new AppResources("Resources").GetJSON("numbers");
    }

    public static void TestFailingJSON(string str) {
      TestFailingJSON(str, new JSONOptions("allowduplicatekeys=true"));
    }

    public static void TestFailingJSON(string str, JSONOptions opt) {
      byte[] bytes = null;
      try {
        bytes = DataUtilities.GetUtf8Bytes(str, false);
      } catch (ArgumentException ex2) {
        Console.WriteLine(ex2.Message);
        // Check only FromJSONString
        try {
          CBORObject.FromJSONString(str, opt);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        return;
      }
      using (var ms = new MemoryStream(bytes)) {
        try {
          CBORObject.ReadJSON(ms, opt);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(str + "\r\n" + ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      try {
        CBORObject.FromJSONString(str, opt);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    public static CBORObject TestSucceedingJSON(string str) {
      return TestSucceedingJSON(str, null);
    }

    public static CBORObject TestSucceedingJSON(
      string str,
      JSONOptions options) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(str, false);
      try {
        using (var ms = new MemoryStream(bytes)) {
          CBORObject obj = options == null ? CBORObject.ReadJSON(ms) :
            CBORObject.ReadJSON(ms, options);
          CBORObject obj2 = options == null ? CBORObject.FromJSONString(str) :
            CBORObject.FromJSONString(str, options);
          TestCommon.CompareTestEqualAndConsistent(
            obj,
            obj2);
          CBORTestCommon.AssertRoundTrip(obj);
          return obj;
        }
      } catch (Exception ex) {
        Assert.Fail(ex.ToString() + "\n" + str);
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    public static string CharString(int cp, bool quoted, char[] charbuf) {
      var index = 0;
      if (quoted) {
        if (charbuf == null) {
          throw new ArgumentNullException(nameof(charbuf));
        }
        charbuf[index++] = (char)0x22;
      }
      if (cp < 0x10000) {
        if (cp >= 0xd800 && cp < 0xe000) {
          return null;
        }
        if (charbuf == null) {
          throw new ArgumentNullException(nameof(charbuf));
        }
        charbuf[index++] = (char)cp;
        if (quoted) {
          charbuf[index++] = (char)0x22;
        }
        return new String(charbuf, 0, index);
      } else {
        cp -= 0x10000;
        if (charbuf == null) {
          throw new ArgumentNullException(nameof(charbuf));
        }
        charbuf[index++] = (char)((cp >> 10) + 0xd800);
        charbuf[index++] = (char)((cp & 0x3ff) | 0xdc00);
        if (quoted) {
          charbuf[index++] = (char)0x22;
        }
        return new String(charbuf, 0, index);
      }
    }
    [Test]
    public void TestAdd() {
      CBORObject cbor = CBORObject.NewMap();
      CBORObject cborNull = CBORObject.Null;
      cbor.Add(null, true);
      Assert.AreEqual(CBORObject.True, cbor[cborNull]);
      cbor.Add("key", null);
      Assert.AreEqual(CBORObject.Null, cbor["key"]);
    }
    [Test]
    public void TestAddConverter() {
      // not implemented yet
    }

    private static EDecimal AsED(CBORObject obj) {
      return (EDecimal)obj.ToObject(typeof(EDecimal));
    }
    [Test]
    public void TestAsNumberAdd() {
      var r = new RandomGenerator();
      for (var i = 0; i < 1000; ++i) {
        CBORObject o1 = CBORTestCommon.RandomNumber(r);
        CBORObject o2 = CBORTestCommon.RandomNumber(r);
        EDecimal cmpDecFrac = AsED(o1).Add(AsED(o2));
        EDecimal cmpCobj = o1.AsNumber().Add(o2.AsNumber()).ToEDecimal();
        TestCommon.CompareTestEqual(cmpDecFrac, cmpCobj);
        CBORTestCommon.AssertRoundTrip(o1);
        CBORTestCommon.AssertRoundTrip(o2);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2).AsNumber().Add(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestAsBoolean() {
      Assert.IsTrue(CBORObject.True.AsBoolean());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(0).AsBoolean());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .AsBoolean());
      Assert.IsFalse(CBORObject.False.AsBoolean());
      Assert.IsFalse(CBORObject.Null.AsBoolean());
      Assert.IsFalse(CBORObject.Undefined.AsBoolean());
      Assert.IsTrue(CBORObject.NewArray().AsBoolean());
      Assert.IsTrue(CBORObject.NewMap().AsBoolean());
    }
    [Test]
    public void TestAsByte() {
      try {
        CBORObject.NewArray().AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty).AsByte();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              numberinfo["number"].AsString()));
        if (numberinfo["byte"].AsBoolean()) {
          Assert.AreEqual(
            TestCommon.StringToInt(numberinfo["integer"].AsString()),
            ((int)cbornumber.AsByte()) & 0xff);
        } else {
          try {
            cbornumber.AsByte();
            Assert.Fail("Should have failed " + cbornumber);
          } catch (OverflowException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + cbornumber);
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      }
      for (var i = 0; i < 255; ++i) {
        Assert.AreEqual(
          (byte)i,
          ToObjectTest.TestToFromObjectRoundTrip(i).AsByte());
      }
      for (int i = -200; i < 0; ++i) {
        try {
          ToObjectTest.TestToFromObjectRoundTrip(i).AsByte();
        } catch (OverflowException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      for (int i = 256; i < 512; ++i) {
        try {
          ToObjectTest.TestToFromObjectRoundTrip(i).AsByte();
        } catch (OverflowException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    }

    [Test]
    public void TestAsDouble() {
      try {
        CBORObject.NewArray().AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty).AsDouble();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              numberinfo["number"].AsString()));
        {
          var dtemp = (double)EDecimal.FromString(
  numberinfo["number"].AsString()).ToDouble();
          double dtemp2 = cbornumber.AsDouble();
          AreEqualExact(dtemp, dtemp2);
        }
      }
    }

    [Test]
    public void TestAsInt16() {
      try {
        CBORObject.NewArray().AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty).AsInt16();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(
            EDecimal.FromString(numberinfo["number"].AsString()));
        if (numberinfo["int16"].AsBoolean()) {
          Assert.AreEqual(
            TestCommon.StringToInt(numberinfo["integer"].AsString()),
            cbornumber.AsInt16());
        } else {
          try {
            cbornumber.AsInt16();
            Assert.Fail("Should have failed " + cbornumber);
          } catch (OverflowException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + cbornumber);
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      }
    }

    [Test]
    public void TestAsInt32() {
      try {
        CBORObject.NewArray().AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty).AsInt32();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        EDecimal edec =
          EDecimal.FromString(numberinfo["number"].AsString());
        CBORObject cbornumber = ToObjectTest.TestToFromObjectRoundTrip(edec);
        bool isdouble = numberinfo["double"].AsBoolean();
        CBORObject cbornumberdouble =
          ToObjectTest.TestToFromObjectRoundTrip(edec.ToDouble());
        bool issingle = numberinfo["single"].AsBoolean();
        CBORObject cbornumbersingle =
          ToObjectTest.TestToFromObjectRoundTrip(edec.ToSingle());
        if (numberinfo["int32"].AsBoolean()) {
          Assert.AreEqual(
            TestCommon.StringToInt(numberinfo["integer"].AsString()),
            cbornumber.AsInt32());
          if (isdouble) {
            Assert.AreEqual(
              TestCommon.StringToInt(numberinfo["integer"].AsString()),
              cbornumberdouble.AsInt32());
          }
          if (issingle) {
            Assert.AreEqual(
              TestCommon.StringToInt(numberinfo["integer"].AsString()),
              cbornumbersingle.AsInt32());
          }
        } else {
          try {
            cbornumber.AsInt32();
            Assert.Fail("Should have failed " + cbornumber);
          } catch (OverflowException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + cbornumber);
            throw new InvalidOperationException(String.Empty, ex);
          }
          if (isdouble) {
            try {
              cbornumberdouble.AsInt32();
              Assert.Fail("Should have failed");
            } catch (OverflowException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
          }
          if (issingle) {
            try {
              cbornumbersingle.AsInt32();
              Assert.Fail("Should have failed");
            } catch (OverflowException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
          }
        }
      }
    }
    [Test]
    public void TestAsInt64() {
      try {
        CBORObject.NewArray().AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty).AsInt64();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        EDecimal edec =
          EDecimal.FromString(numberinfo["number"].AsString());
        CBORObject cbornumber = ToObjectTest.TestToFromObjectRoundTrip(edec);
        bool isdouble = numberinfo["double"].AsBoolean();
        CBORObject cbornumberdouble =
          ToObjectTest.TestToFromObjectRoundTrip(edec.ToDouble());
        bool issingle = numberinfo["single"].AsBoolean();
        CBORObject cbornumbersingle =
          ToObjectTest.TestToFromObjectRoundTrip(edec.ToSingle());
        if (numberinfo["int64"].AsBoolean()) {
          Assert.AreEqual(
            TestCommon.StringToLong(numberinfo["integer"].AsString()),
            cbornumber.AsInt64());
          if (isdouble) {
            Assert.AreEqual(
              TestCommon.StringToLong(numberinfo["integer"].AsString()),
              cbornumberdouble.AsInt64());
          }
          if (issingle) {
            Assert.AreEqual(
              TestCommon.StringToLong(numberinfo["integer"].AsString()),
              cbornumbersingle.AsInt64());
          }
        } else {
          try {
            cbornumber.AsInt64();
            Assert.Fail("Should have failed " + cbornumber);
          } catch (OverflowException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + cbornumber);
            throw new InvalidOperationException(String.Empty, ex);
          }
          if (isdouble) {
            try {
              cbornumberdouble.AsInt64();
              Assert.Fail("Should have failed");
            } catch (OverflowException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
          }
          if (issingle) {
            try {
              cbornumbersingle.AsInt64();
              Assert.Fail("Should have failed");
            } catch (OverflowException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
          }
        }
      }
    }
    [Test]
    public void TestAsSByte() {
      // not implemented yet
    }
    [Test]
    public void TestAsSingle() {
      try {
        CBORObject.NewArray().AsSingle();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsSingle();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsSingle();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsSingle();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsSingle();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty).AsSingle();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              numberinfo["number"].AsString()));
        {
          var ftemp = (float)EDecimal.FromString(
  numberinfo["number"].AsString()).ToSingle();
          float ftemp2 = cbornumber.AsSingle();
          AreEqualExact(ftemp, ftemp2);
        }
      }
    }
    [Test]
    public void TestAsString() {
      {
        string stringTemp = ToObjectTest.TestToFromObjectRoundTrip("test")
          .AsString();
        Assert.AreEqual(
          "test",
          stringTemp);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(CBORObject.Null).AsString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(true).AsString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(false).AsString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(5).AsString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().AsString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestAsUInt16() {
      // not implemented yet
    }
    [Test]
    public void TestAsUInt32() {
      // not implemented yet
    }
    [Test]
    public void TestAsUInt64() {
      // not implemented yet
    }
    [Test]
    public void TestCanFitInDouble() {
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(
          0).CanFitInDouble());
      Assert.IsFalse(CBORObject.True.CanFitInDouble());
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .CanFitInDouble());
      Assert.IsFalse(CBORObject.NewArray().CanFitInDouble());
      Assert.IsFalse(CBORObject.NewMap().CanFitInDouble());
      Assert.IsFalse(CBORObject.False.CanFitInDouble());
      Assert.IsFalse(CBORObject.Null.CanFitInDouble());
      Assert.IsFalse(CBORObject.Undefined.CanFitInDouble());
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              numberinfo["number"].AsString()));
        if (numberinfo["double"].AsBoolean()) {
          Assert.IsTrue(cbornumber.CanFitInDouble());
        } else {
          Assert.IsFalse(cbornumber.CanFitInDouble());
        }
      }
      var rand = new RandomGenerator();
      for (var i = 0; i < 2047; ++i) {
        // Try a random double with a given
        // exponent
        object o = RandomObjects.RandomDouble(rand, i);
        CBORObject cbornumber = ToObjectTest.TestToFromObjectRoundTrip(o);
        Assert.IsTrue(cbornumber.CanFitInDouble());
      }
    }
    [Test]
    public void TestCanFitInInt32() {
      Assert.IsTrue(CInt32(ToObjectTest.TestToFromObjectRoundTrip(0)));
      Assert.IsFalse(CInt32(CBORObject.True));
      Assert.IsFalse(CInt32(ToObjectTest.TestToFromObjectRoundTrip(
            String.Empty)));
      Assert.IsFalse(CInt32(CBORObject.NewArray()));
      Assert.IsFalse(CInt32(CBORObject.NewMap()));
      Assert.IsFalse(CInt32(CBORObject.False));
      Assert.IsFalse(CInt32(CBORObject.Null));
      Assert.IsFalse(CInt32(CBORObject.Undefined));
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              numberinfo["number"].AsString()));
        if (numberinfo["int32"].AsBoolean() &&
          numberinfo["isintegral"].AsBoolean()) {
          Assert.IsTrue(CInt32(cbornumber));

          Assert.IsTrue(
            CInt32(ToObjectTest.TestToFromObjectRoundTrip(
                cbornumber.AsInt32())));
        } else {
          Assert.IsFalse(CInt32(cbornumber));
        }
      }
    }
    private static bool CInt64(CBORObject cbor) {
      return cbor.IsNumber && cbor.AsNumber().CanFitInInt64();
    }

    private static bool CInt32(CBORObject cbor) {
      return cbor.IsNumber && cbor.AsNumber().CanFitInInt32();
    }
    [Test]
    public void TestCanFitInInt64() {
      Assert.IsTrue(CInt64(ToObjectTest.TestToFromObjectRoundTrip(0)));
      Assert.IsFalse(CInt64(CBORObject.True));
      Assert.IsFalse(CInt64(ToObjectTest.TestToFromObjectRoundTrip(
            String.Empty)));
      Assert.IsFalse(CInt64(CBORObject.NewArray()));
      Assert.IsFalse(CInt64(CBORObject.NewMap()));
      Assert.IsFalse(CInt64(CBORObject.False));
      Assert.IsFalse(CInt64(CBORObject.Null));
      Assert.IsFalse(CInt64(CBORObject.Undefined));

      EInteger ei;
      ei = EInteger.FromString("9223372036854775807");
      Assert.IsTrue(CInt64(CBORObject.FromObject(ei)), ei.ToString());
      ei = EInteger.FromString("9223372036854775808");
      Assert.IsFalse(CInt64(CBORObject.FromObject(ei)), ei.ToString());
      ei = EInteger.FromString("-9223372036854775807");
      Assert.IsTrue(CInt64(CBORObject.FromObject(ei)), ei.ToString());
      ei = EInteger.FromString("-9223372036854775808");
      Assert.IsTrue(CInt64(CBORObject.FromObject(ei)), ei.ToString());
      ei = EInteger.FromString("-9223372036854775809");
      Assert.IsFalse(CInt64(CBORObject.FromObject(ei)), ei.ToString());
      ei = EInteger.FromString("-9223373136366403584");
      Assert.IsFalse(CInt64(CBORObject.FromObject(ei)), ei.ToString());
      ei = EInteger.FromString("9223373136366403584");
      Assert.IsFalse(CInt64(CBORObject.FromObject(ei)), ei.ToString());
      var strings = new string[] {
        "8000FFFFFFFF0000",
        "8000AAAAAAAA0000",
        "8000800080000000",
        "8000000100010000",
        "8000FFFF00000000",
        "80000000FFFF0000",
        "8000800000000000",
        "8000000080000000",
        "8000AAAA00000000",
        "80000000AAAA0000",
        "8000000100000000",
        "8000000000010000",
      };
      foreach (var str in strings) {
        ei = EInteger.FromRadixString(str, 16);
        Assert.IsFalse(CInt64(CBORObject.FromObject(ei)));
        ei = ei.Negate();
        Assert.IsFalse(CInt64(CBORObject.FromObject(ei)));
      }

      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              numberinfo["number"].AsString()));
        if (numberinfo["int64"].AsBoolean() &&
          numberinfo["isintegral"].AsBoolean()) {
          Assert.IsTrue(CInt64(cbornumber));

          Assert.IsTrue(
            CInt64(ToObjectTest.TestToFromObjectRoundTrip(
                cbornumber.AsInt64())));
        } else {
          Assert.IsFalse(CInt64(cbornumber));
        }
      }
    }
    [Test]
    public void TestCanFitInSingle() {
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(
          0).CanFitInSingle());
      Assert.IsFalse(CBORObject.True.CanFitInSingle());
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .CanFitInSingle());
      Assert.IsFalse(CBORObject.NewArray().CanFitInSingle());
      Assert.IsFalse(CBORObject.NewMap().CanFitInSingle());
      Assert.IsFalse(CBORObject.False.CanFitInSingle());
      Assert.IsFalse(CBORObject.Null.CanFitInSingle());
      Assert.IsFalse(CBORObject.Undefined.CanFitInSingle());
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              numberinfo["number"].AsString()));
        if (numberinfo["single"].AsBoolean()) {
          Assert.IsTrue(cbornumber.CanFitInSingle());
        } else {
          Assert.IsFalse(cbornumber.CanFitInSingle());
        }
      }

      var rand = new RandomGenerator();
      for (var i = 0; i < 255; ++i) {
        // Try a random float with a given
        // exponent
        Assert.IsTrue(
          ToObjectTest.TestToFromObjectRoundTrip(
            RandomObjects.RandomSingle(
              rand,
              i)).CanFitInSingle());
      }
    }
    [Test]
    public void TestCanTruncatedIntFitInInt32() {
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            11)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            12)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            13)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            14)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            15)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            16)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            17)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            18)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            19)).CanTruncatedIntFitInInt32());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(0)
        .CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.True.CanTruncatedIntFitInInt32());
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.NewArray().CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.NewMap().CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.False.CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.Null.CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.Undefined.CanTruncatedIntFitInInt32());
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(
            EDecimal.FromString(numberinfo["number"].AsString()));
        if (numberinfo["int32"].AsBoolean()) {
          Assert.IsTrue(cbornumber.CanTruncatedIntFitInInt32());
        } else {
          Assert.IsFalse(cbornumber.CanTruncatedIntFitInInt32());
        }
      }

      Assert.IsFalse(CBORObject.True.CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.False.CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.NewArray().CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.NewMap().CanTruncatedIntFitInInt32());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(0)
        .CanTruncatedIntFitInInt32());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(2.5)
        .CanTruncatedIntFitInInt32());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(Int32.MinValue)
        .CanTruncatedIntFitInInt32());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(Int32.MaxValue)
        .CanTruncatedIntFitInInt32());
      var negint32 = new object[] {
        Double.PositiveInfinity,
        Double.NegativeInfinity,
        Double.NaN,
        CBORTestCommon.DecPosInf,
        CBORTestCommon.DecNegInf,
        EDecimal.NaN,
      };
      foreach (var obj in negint32) {
        bool bval = ToObjectTest.TestToFromObjectRoundTrip(obj)
          .CanTruncatedIntFitInInt32();
        Assert.IsFalse(bval, obj.ToString());
      }
    }

    [Test]
    public void TestCanTruncatedIntFitInInt64() {
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            11)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            12)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            13)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            14)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            15)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            16)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            17)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            18)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
            -2,
            19)).CanTruncatedIntFitInInt64());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(0)
        .CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.True.CanTruncatedIntFitInInt64());
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.NewArray().CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.NewMap().CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.False.CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.Null.CanTruncatedIntFitInInt64());
      Assert.IsFalse(CBORObject.Undefined.CanTruncatedIntFitInInt64());

      EInteger ei;
      ei = EInteger.FromString("9223372036854775807");
      {
        bool btemp = CBORObject.FromObject(ei)
          .CanTruncatedIntFitInInt64();
        Assert.IsTrue(btemp, ei.ToString());
      }
      ei = EInteger.FromString("9223372036854775808");
      Assert.IsFalse(CBORObject.FromObject(ei).CanTruncatedIntFitInInt64(),
        ei.ToString());
      ei = EInteger.FromString("-9223372036854775807");
      Assert.IsTrue(CBORObject.FromObject(ei).CanTruncatedIntFitInInt64(),
        ei.ToString());
      ei = EInteger.FromString("-9223372036854775808");
      Assert.IsTrue(CBORObject.FromObject(ei).CanTruncatedIntFitInInt64(),
        ei.ToString());
      ei = EInteger.FromString("-9223372036854775809");
      Assert.IsFalse(CBORObject.FromObject(ei).CanTruncatedIntFitInInt64(),
        ei.ToString());
      ei = EInteger.FromString("-9223373136366403584");
      Assert.IsFalse(CBORObject.FromObject(ei).CanTruncatedIntFitInInt64(),
        ei.ToString());
      ei = EInteger.FromString("9223373136366403584");
      Assert.IsFalse(CBORObject.FromObject(ei).CanTruncatedIntFitInInt64(),
        ei.ToString());
      var strings = new string[] {
        "8000FFFFFFFF0000",
        "8000AAAAAAAA0000",
        "8000800080000000",
        "8000000100010000",
        "8000FFFF00000000",
        "80000000FFFF0000",
        "8000800000000000",
        "8000000080000000",
        "8000AAAA00000000",
        "80000000AAAA0000",
        "8000000100000000",
        "8000000000010000",
      };
      foreach (var str in strings) {
        ei = EInteger.FromRadixString(str, 16);
        Assert.IsFalse(CBORObject.FromObject(ei).CanTruncatedIntFitInInt64());
        ei = ei.Negate();
        Assert.IsFalse(CBORObject.FromObject(ei).CanTruncatedIntFitInInt64());
      }

      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        string numberString = numberinfo["number"].AsString();
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              numberString));
        if (numberinfo["int64"].AsBoolean()) {
          Assert.IsTrue(
            cbornumber.CanTruncatedIntFitInInt64(),
            numberString);
        } else {
          Assert.IsFalse(
            cbornumber.CanTruncatedIntFitInInt64(),
            numberString);
        }
      }
    }
    [Test]
    [Timeout(1000)]
    public void TestSlowCompareTo2() {
      CBORObject cbor1 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc5,
        (byte)0x82, 0x3b, 0x00, 0x00, 0x00, (byte)0xd3, (byte)0xe1, 0x26,
        (byte)0xf9, 0x3b, (byte)0xc2, 0x4c, 0x01, 0x01, 0x01, 0x00, 0x00, 0x01,
        0x01, 0x00, 0x00, 0x01, 0x00, 0x00,
      });
      CBORObject cbor2 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc4,
        (byte)0x82, 0x3b, 0x00, 0x00, 0x00, 0x56, (byte)0xe9, 0x21, (byte)0xda,
        (byte)0xe9, (byte)0xc2, 0x58, 0x2a, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x01, 0x00, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00,
        0x01, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x01,
        0x01, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x01, 0x01, 0x00, 0x01,
        0x00,
      });
      Console.WriteLine(cbor1);
      Console.WriteLine(cbor2);
      TestCommon.CompareTestGreater(cbor1.AsNumber(), cbor2.AsNumber());
    }

    [Test]
    [Timeout(1000)]
    public void TestSlowCompareTo6() {
      CBORObject cbor1 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc5,
        (byte)0x82, 0x1b, 0x00, 0x00, 0x00, 0x7a, 0x50, (byte)0xe0, 0x1f,
        (byte)0xc6, (byte)0xc2, 0x4c, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x01,
        0x00, 0x00, 0x00, 0x01, 0x01,
      });
      CBORObject cbor2 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc4,
        (byte)0x82, 0x19, 0x01, 0x60, (byte)0xc2, 0x58, (byte)0x87, (byte)0xbb,
        (byte)0xf8, 0x74, (byte)0xbe, (byte)0xcc, 0x46, 0x6b, 0x02, 0x3c,
        (byte)0x84, (byte)0xe9, (byte)0xd1, (byte)0xe9, 0x3c, (byte)0xd3,
        (byte)0xd5, 0x20, (byte)0xc1, 0x7e, 0x17, 0x09, 0x0f, (byte)0xdd, 0x73,
        0x5d, (byte)0xe4, 0x51, (byte)0xd6, 0x10, 0x52, 0x2e, 0x6c, 0x77,
        (byte)0x9f, 0x5e, 0x4f, 0x58, 0x72, 0x38, 0x43, (byte)0xb0, 0x28, 0x5a,
        0x6c, (byte)0xe5, (byte)0xd2, 0x36, (byte)0x9e, 0x69, 0x50, (byte)0xf9,
        0x62, 0x7f, (byte)0xcb, (byte)0xf5, 0x12, (byte)0x8c, 0x37, 0x2d,
        (byte)0x8e, 0x4f, (byte)0x83, 0x5c, (byte)0xd6, 0x6d, 0x5e, (byte)0xf0,
        0x65, 0x12, 0x4a, 0x0a, (byte)0x81, (byte)0x89, (byte)0xed, 0x20, 0x50,
        (byte)0xca, 0x0e, (byte)0x81, (byte)0xbc, (byte)0x9e, (byte)0x83, 0x66,
        (byte)0xb1, (byte)0xcd, 0x23, (byte)0xee, 0x24, 0x2e, (byte)0xec, 0x77,
        0x13, (byte)0x89, (byte)0xbd, (byte)0xfb, 0x47, (byte)0xd1, 0x02, 0x1c,
        0x4e, (byte)0xf5, 0x30, 0x59, 0x75, (byte)0xce, (byte)0xa8, (byte)0xaf,
        0x23, 0x51, 0x7e, 0x26, (byte)0xaa, (byte)0xed, (byte)0xe9, 0x34, 0x02,
        0x31, 0x70, (byte)0xe3, 0x3f, 0x71, (byte)0x9a, (byte)0x9a, (byte)0xe9,
        (byte)0xf3, 0x6d, (byte)0xd7, 0x28, 0x18, (byte)0xa2, (byte)0xb5,
        (byte)0x8b, (byte)0xca, 0x11, (byte)0x99,
      });
      Console.WriteLine(cbor1);
      Console.WriteLine(cbor2);
      TestCommon.CompareTestReciprocal(cbor1.AsNumber(), cbor2.AsNumber());
    }
    [Test]
    [Timeout(1000)]
    public void TestSlowCompareTo5() {
      CBORObject cbor1 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc5,
        (byte)0x82, 0x1b, 0x00, 0x00, 0x10, 0x57, (byte)0xa5, (byte)0x96,
        (byte)0xbe, 0x7b, (byte)0xc2, 0x53, 0x01, 0x01, 0x00, 0x00, 0x00, 0x01,
        0x01, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01, 0x01,
        0x00,
      });
      CBORObject cbor2 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc4,
        (byte)0x82, 0x19, 0x01, (byte)0x84, (byte)0xc2, 0x53, 0x20, 0x44, 0x52,
        0x64, (byte)0x9d, (byte)0xea, (byte)0xe8, 0x57, 0x13, (byte)0xa3, 0x7c,
        (byte)0xeb, 0x5e, 0x0e, 0x54, (byte)0xc8, (byte)0xf0, (byte)0xb2,
        0x58,
      });
      Console.WriteLine(cbor1);
      Console.WriteLine(cbor2);
      TestCommon.CompareTestReciprocal(cbor1.AsNumber(), cbor2.AsNumber());
    }

    [Test]
    [Timeout(1000)]
    public void TestSlowCompareTo() {
      CBORObject cbor1 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc5,
        (byte)0x82, 0x3b, 0x00, 0x00, 0x00, 0x15, (byte)0xfc, (byte)0xa0,
        (byte)0xd9, (byte)0xf9, (byte)0xc3, 0x58, 0x36, 0x02, (byte)0x83, 0x3b,
        0x3c, (byte)0x99, (byte)0xdb, (byte)0xe4, (byte)0xfc, 0x2a, 0x69, 0x69,
        (byte)0xe7, 0x63, (byte)0xb7, 0x5d, 0x48, (byte)0xcf, 0x51, 0x33,
        (byte)0xd7, (byte)0xc3, 0x59, 0x4d, 0x63, 0x3c, (byte)0xbb, (byte)0x9d,
        0x43, 0x2d, (byte)0xd1, 0x51, 0x39, 0x1f, 0x03, 0x22, 0x5c, 0x13,
        (byte)0xed, 0x02, (byte)0xca, (byte)0xda, 0x09, 0x22, 0x07, (byte)0x9f,
        0x34, (byte)0x84, (byte)0xb4, 0x22, (byte)0xa8, 0x26, (byte)0x9f, 0x35,
        (byte)0x8d,
      });
      CBORObject cbor2 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc4,
        (byte)0x82, 0x24, 0x26,
      });
      Console.WriteLine(cbor1);
      Console.WriteLine(cbor2);
      TestCommon.CompareTestGreater(cbor1.AsNumber(), cbor2.AsNumber());
    }

    [Test]
    [Timeout(1000)]
    public void TestSlowCompareTo3() {
      CBORObject cbor1 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc5,
        (byte)0x82, 0x3b, 0x04, 0x55, 0x0a, 0x12, (byte)0x94, (byte)0xf8, 0x1f,
        (byte)0x9b, (byte)0xc2, 0x58, 0x1f, 0x01, 0x00, 0x00, 0x01, 0x01, 0x00,
        0x00, 0x01, 0x00, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x01,
        0x01, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x01,
        0x00,
      });
      CBORObject cbor2 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc4,
        (byte)0x82, 0x39, 0x02, 0x03, (byte)0xc2, 0x58, 0x2d, 0x01, 0x00, 0x00,
        0x00, 0x00, 0x01, 0x01, 0x00, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01,
        0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
        0x00, 0x01, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x01, 0x01, 0x00,
        0x01, 0x01, 0x00, 0x01, 0x00, 0x01,
      });
      Console.WriteLine(cbor1);
      Console.WriteLine(cbor2);
      TestCommon.CompareTestLess(cbor1.AsNumber(), cbor2.AsNumber());
    }

    [Test]
    [Timeout(1000)]
    public void TestSlowCompareTo4() {
      CBORObject cbor1 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc4,
        (byte)0x82, 0x2f, 0x3b, 0x00, 0x1e, (byte)0xdc, 0x5d, 0x51, 0x5d, 0x26,
        (byte)0xb7,
      });
      CBORObject cbor2 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc5,
        (byte)0x82, 0x3b, 0x00, 0x18, 0x72, 0x44, 0x49, (byte)0xd0, 0x0c,
        (byte)0xb6, (byte)0xc3, 0x58, (byte)0x88, 0x0a, (byte)0xd0, 0x12,
        (byte)0x93, (byte)0xcb, 0x0a, 0x30, 0x2d, 0x11, 0x36, 0x59, 0x5a,
        (byte)0xfe, (byte)0x81, 0x79, (byte)0x80, (byte)0x86, (byte)0xb8, 0x2f,
        0x26, 0x4b, (byte)0xf4, 0x70, (byte)0xb4, 0x37, 0x3b, 0x7a, 0x1d,
        (byte)0x89, 0x4b, (byte)0xd4, 0x75, 0x07, (byte)0xad, 0x0c, (byte)0x90,
        0x6b, 0x1f, 0x53, (byte)0xf7, (byte)0xc3, (byte)0xde, 0x61, (byte)0xf2,
        0x62, 0x78, (byte)0x8a, 0x29, 0x31, 0x44, (byte)0xdd, 0x20, (byte)0xa4,
        0x79, 0x76, 0x59, (byte)0xb7, (byte)0xf7, 0x7c, 0x37, (byte)0xb8, 0x47,
        (byte)0xcf, (byte)0x96, (byte)0xf8, (byte)0x85, (byte)0xae, (byte)0xee,
        (byte)0xb4, 0x06, 0x13, (byte)0xef, (byte)0xd1, (byte)0xe6, 0x36,
        (byte)0xa5, (byte)0xfe, (byte)0xec, (byte)0x8f, (byte)0x8e, 0x00,
        (byte)0xaa, (byte)0xc2, (byte)0xd4, 0x77, (byte)0xcf, (byte)0xea,
        (byte)0xff, 0x4d, 0x12, 0x0b, (byte)0xf5, 0x08, (byte)0xc4, 0x0f, 0x08,
        (byte)0xa7, 0x07, (byte)0xb6, 0x45, 0x47, (byte)0x89, (byte)0xba, 0x5a,
        (byte)0xde, 0x6c, 0x69, 0x6a, 0x49, (byte)0xba, (byte)0xb2, (byte)0xd9,
        0x0f, (byte)0x9c, (byte)0xa4, (byte)0xec, 0x48, (byte)0xd2, 0x71, 0x50,
        (byte)0xde, (byte)0x96, (byte)0x99, (byte)0x9e, (byte)0x89, 0x33,
        (byte)0x8f, 0x6f, (byte)0xa8, 0x30, (byte)0xa1, 0x0a, 0x0f, (byte)0xab,
        (byte)0xfe, (byte)0xbe,
      });
      Console.WriteLine(cbor1);
      Console.WriteLine(cbor2);
      TestCommon.CompareTestLess(cbor1.AsNumber(), cbor2.AsNumber());
    }
    private static string TrimStr(string str, int len) {
      return str.Substring(0, Math.Min(len, str.Length));
    }
    [Test]
    public void CompareLongDouble() {
      CBORObject cbor1 = CBORObject.FromObject(3.5E-15);
      CBORObject cbor2 = CBORObject.FromObject(281479271677953L);
      TestCommon.CompareTestLess(cbor1.AsDouble(), cbor2.AsDouble());
    }

    [Test]
    [Timeout(300000)]
    public void TestCompareTo() {
      var r = new RandomGenerator();
      const int CompareCount = 3000;
      var list = new List<CBORObject>();
      for (var i = 0; i < CompareCount; ++i) {
        CBORObject o1 = CBORTestCommon.RandomCBORObject(r);
        CBORObject o2 = CBORTestCommon.RandomCBORObject(r);
        CBORObject o3 = CBORTestCommon.RandomCBORObject(r);
        TestCommon.CompareTestRelations(o1, o2, o3);
      }
      Console.WriteLine("Check compare");
      for (var i = 0; i < list.Count; ++i) {
        int j;
        j = i + 1;
        for (; j < list.Count; ++j) {
          CBORObject o1 = list[i];
          CBORObject o2 = list[j];
          TestCommon.CompareTestReciprocal(o1, o2);
        }
      }
      Console.WriteLine("Sorting");
      list.Sort();
      Console.WriteLine("Check compare 2");
      for (var i = 0; i < list.Count - 1; ++i) {
        CBORObject o1 = list[i];
        CBORObject o2 = list[i + 1];
        TestCommon.CompareTestLessEqual(o1, o2);
      }
      for (var i = 0; i < 5000; ++i) {
        CBORObject o1 = CBORTestCommon.RandomNumber(r);
        CBORObject o2 = CBORTestCommon.RandomNumber(r);
        CompareDecimals(o1, o2);
      }
      TestCommon.CompareTestEqual(
        ToObjectTest.TestToFromObjectRoundTrip(0.1),
        ToObjectTest.TestToFromObjectRoundTrip(0.1));
      TestCommon.CompareTestEqual(
        ToObjectTest.TestToFromObjectRoundTrip(0.1f),
        ToObjectTest.TestToFromObjectRoundTrip(0.1f));
      for (var i = 0; i < 50; ++i) {
        CBORObject o1 =
          ToObjectTest.TestToFromObjectRoundTrip(Single.NegativeInfinity);
        CBORObject o2 = CBORTestCommon.RandomNumberOrRational(r);
        if (o2.AsNumber().IsInfinity() || o2.AsNumber().IsNaN()) {
          continue;
        }
        TestCommon.CompareTestLess(o1.AsNumber(), o2.AsNumber());
        o1 = ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity);
        TestCommon.CompareTestLess(o1.AsNumber(), o2.AsNumber());
        o1 = ToObjectTest.TestToFromObjectRoundTrip(Single.PositiveInfinity);
        TestCommon.CompareTestGreater(o1.AsNumber(), o2.AsNumber());
        o1 = ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity);
        TestCommon.CompareTestGreater(o1.AsNumber(), o2.AsNumber());
      }
      byte[] bytes1 = { 0, 1 };
      byte[] bytes2 = { 0, 2 };
      byte[] bytes3 = { 1, 1 };
      byte[] bytes4 = { 1, 2 };
      byte[] bytes5 = { 0, 2, 0 };
      byte[] bytes6 = { 1, 1, 4 };
      byte[] bytes7 = { 1, 2, 6 };
      CBORObject[] sortedObjects = {
        ToObjectTest.TestToFromObjectRoundTrip(bytes1),
        ToObjectTest.TestToFromObjectRoundTrip(bytes2),
        ToObjectTest.TestToFromObjectRoundTrip(bytes3),
        ToObjectTest.TestToFromObjectRoundTrip(bytes4),
        ToObjectTest.TestToFromObjectRoundTrip(bytes5),
        ToObjectTest.TestToFromObjectRoundTrip(bytes6),
        ToObjectTest.TestToFromObjectRoundTrip(bytes7),
        ToObjectTest.TestToFromObjectRoundTrip("aa"),
        ToObjectTest.TestToFromObjectRoundTrip("ab"),
        ToObjectTest.TestToFromObjectRoundTrip("ba"),
        ToObjectTest.TestToFromObjectRoundTrip("abc"),
        ToObjectTest.TestToFromObjectRoundTrip(CBORObject.NewArray()),
        ToObjectTest.TestToFromObjectRoundTrip(CBORObject.NewMap()),
        CBORObject.FromSimpleValue(0),
        CBORObject.FromSimpleValue(1),
        CBORObject.FromSimpleValue(19), CBORObject.FromSimpleValue(32),
        CBORObject.FromSimpleValue(255),
        ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity),
      };
      for (var i = 0; i < sortedObjects.Length; ++i) {
        for (int j = i; j < sortedObjects.Length; ++j) {
          if (i == j) {
            TestCommon.CompareTestEqual(sortedObjects[i], sortedObjects[j]);
          } else {
            TestCommon.CompareTestLess(sortedObjects[i], sortedObjects[j]);
          }
        }
        Assert.AreEqual(1, sortedObjects[i].CompareTo(null));
      }
      CBORNumber sp =
        CBORObject.FromObject(Single.PositiveInfinity).AsNumber();
      CBORNumber sn = CBORObject.FromObject(
          Single.NegativeInfinity).AsNumber();
      CBORNumber snan = CBORObject.FromObject(Single.NaN).AsNumber();
      CBORNumber dp = CBORObject.FromObject(
          Double.PositiveInfinity).AsNumber();
      CBORNumber dn = CBORObject.FromObject(
          Double.NegativeInfinity).AsNumber();
      CBORNumber dnan = CBORObject.FromObject(Double.NaN).AsNumber();
      TestCommon.CompareTestEqual(sp, sp);
      TestCommon.CompareTestEqual(sp, dp);
      TestCommon.CompareTestEqual(dp, dp);
      TestCommon.CompareTestEqual(sn, sn);
      TestCommon.CompareTestEqual(sn, dn);
      TestCommon.CompareTestEqual(dn, dn);
      TestCommon.CompareTestEqual(snan, snan);
      TestCommon.CompareTestEqual(snan, dnan);
      TestCommon.CompareTestEqual(dnan, dnan);
      TestCommon.CompareTestLess(sn, sp);
      TestCommon.CompareTestLess(sn, dp);
      TestCommon.CompareTestLess(sn, snan);
      TestCommon.CompareTestLess(sn, dnan);
      TestCommon.CompareTestLess(sp, snan);
      TestCommon.CompareTestLess(sp, dnan);
      TestCommon.CompareTestLess(dn, dp);
      TestCommon.CompareTestLess(dp, dnan);
      Assert.AreEqual(1, CBORObject.True.CompareTo(null));
      Assert.AreEqual(1, CBORObject.False.CompareTo(null));
      Assert.AreEqual(1, CBORObject.Null.CompareTo(null));
      Assert.AreEqual(1, CBORObject.NewArray().CompareTo(null));
      Assert.AreEqual(1, CBORObject.NewMap().CompareTo(null));
      {
        long numberTemp =
          ToObjectTest.TestToFromObjectRoundTrip(100).CompareTo(null);
        Assert.AreEqual(1, numberTemp);
      }
      {
        long numberTemp =
          ToObjectTest.TestToFromObjectRoundTrip(Double.NaN).CompareTo(null);
        Assert.AreEqual(1, numberTemp);
      }
      TestCommon.CompareTestLess(
        ToObjectTest.TestToFromObjectRoundTrip(0).AsNumber(),
        ToObjectTest.TestToFromObjectRoundTrip(1).AsNumber());
      TestCommon.CompareTestLess(
        ToObjectTest.TestToFromObjectRoundTrip(0.0f).AsNumber(),
        ToObjectTest.TestToFromObjectRoundTrip(1.0f).AsNumber());
      TestCommon.CompareTestLess(
        ToObjectTest.TestToFromObjectRoundTrip(0.0).AsNumber(),
        ToObjectTest.TestToFromObjectRoundTrip(1.0).AsNumber());
      TestCommon.CompareTestEqual(
        CBORObject.FromObject(10).AsNumber(),
        CBORObject.FromObject(ERational.Create(10, 1)).AsNumber());
    }
    [Test]
    public void TestContainsKey() {
      // not implemented yet
    }
    [Test]
    public void TestCount() {
      Assert.AreEqual(0, CBORObject.True.Count);
      Assert.AreEqual(0, CBORObject.False.Count);
      Assert.AreEqual(0, CBORObject.NewArray().Count);
      Assert.AreEqual(0, CBORObject.NewMap().Count);
    }

    [Test]
    public void TestDecodeFromBytes() {
      try {
        CBORObject.DecodeFromBytes(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0 }, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0x1c });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0x1e });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xfe });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestDecodeFromBytesNoDuplicateKeys() {
      byte[] bytes;
      bytes = new byte[] { 0xa2, 0x01, 0x00, 0x02, 0x03 };
      try {
        CBORObject.DecodeFromBytes(bytes, new
CBOREncodeOptions("allowduplicatekeys=0"));
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xa2, 0x01, 0x00, 0x01, 0x03 };
      try {
        CBORObject.DecodeFromBytes(bytes, new
CBOREncodeOptions("allowduplicatekeys=0"));
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xa2, 0x01, 0x00, 0x01, 0x03 };
      try {
        CBORObject.DecodeFromBytes(bytes, new
CBOREncodeOptions("allowduplicatekeys=1;useindefencoding=1"));
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xa2, 0x60, 0x00, 0x60, 0x03 };
      try {
        CBORObject.DecodeFromBytes(bytes, new
CBOREncodeOptions("allowduplicatekeys=0"));
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] {
        0xa3, 0x60, 0x00, 0x62, 0x41, 0x41, 0x00, 0x60,
        0x03,
      };
      try {
        CBORObject.DecodeFromBytes(bytes, new
CBOREncodeOptions("allowduplicatekeys=0"));
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xa2, 0x61, 0x41, 0x00, 0x61, 0x41, 0x03 };
      try {
        CBORObject.DecodeFromBytes(bytes, new
CBOREncodeOptions("allowduplicatekeys=0"));
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestDecodeSequenceFromBytes() {
      CBORObject[] objs;
      byte[] bytes;
      bytes = new byte[] { 0 };
      objs = CBORObject.DecodeSequenceFromBytes(bytes);
      Assert.AreEqual(1, objs.Length);
      Assert.AreEqual(CBORObject.FromObject(0), objs[0]);
      bytes = new byte[] { 0, 1, 2 };
      objs = CBORObject.DecodeSequenceFromBytes(bytes);
      Assert.AreEqual(3, objs.Length);
      Assert.AreEqual(CBORObject.FromObject(0), objs[0]);
      Assert.AreEqual(CBORObject.FromObject(1), objs[1]);
      Assert.AreEqual(CBORObject.FromObject(2), objs[2]);
      bytes = new byte[] { 0, 1, 0x61 };
      try {
        CBORObject.DecodeSequenceFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0x61 };
      try {
        CBORObject.DecodeSequenceFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0, 1, 0x61, 0x41 };
      objs = CBORObject.DecodeSequenceFromBytes(bytes);
      Assert.AreEqual(3, objs.Length);
      Assert.AreEqual(CBORObject.FromObject(0), objs[0]);
      Assert.AreEqual(CBORObject.FromObject(1), objs[1]);
      Assert.AreEqual(CBORObject.FromObject("A"), objs[2]);
      bytes = new byte[] { };
      objs = CBORObject.DecodeSequenceFromBytes(bytes);
      Assert.AreEqual(0, objs.Length);
      try {
        CBORObject.DecodeSequenceFromBytes(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeSequenceFromBytes(bytes, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestReadSequence() {
      CBORObject[] objs;
      byte[] bytes;
      bytes = new byte[] { 0 };
      using (var ms = new MemoryStream(bytes)) {
        objs = null;
        try {
          objs = CBORObject.ReadSequence(ms);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      Assert.AreEqual(1, objs.Length);
      Assert.AreEqual(CBORObject.FromObject(0), objs[0]);
      bytes = new byte[] { 0, 1, 2 };
      using (var ms = new MemoryStream(bytes)) {
        objs = null;
        try {
          objs = CBORObject.ReadSequence(ms);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      Assert.AreEqual(3, objs.Length);
      Assert.AreEqual(CBORObject.FromObject(0), objs[0]);
      Assert.AreEqual(CBORObject.FromObject(1), objs[1]);
      Assert.AreEqual(CBORObject.FromObject(2), objs[2]);
      bytes = new byte[] { 0, 1, 0x61 };
      using (var ms = new MemoryStream(bytes)) {
        try {
          CBORObject.ReadSequence(ms);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      bytes = new byte[] { 0x61 };
      using (var ms = new MemoryStream(bytes)) {
        try {
          CBORObject.ReadSequence(ms);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      bytes = new byte[] { 0, 1, 0x61, 0x41 };
      using (var ms = new MemoryStream(bytes)) {
        objs = null;
        try {
          objs = CBORObject.ReadSequence(ms);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      Assert.AreEqual(3, objs.Length);
      Assert.AreEqual(CBORObject.FromObject(0), objs[0]);
      Assert.AreEqual(CBORObject.FromObject(1), objs[1]);
      Assert.AreEqual(CBORObject.FromObject("A"), objs[2]);
      bytes = new byte[] { };
      using (var ms = new MemoryStream(bytes)) {
        objs = null;
        try {
          objs = CBORObject.ReadSequence(ms);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      Assert.AreEqual(0, objs.Length);
      try {
        CBORObject.ReadSequence(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      using (var ms = new MemoryStream(bytes)) {
        try {
          CBORObject.ReadSequence(ms, null);
          Assert.Fail("Should have failed");
        } catch (ArgumentNullException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    }

    [Test]
    public void TestEncodeToBytes() {
      // Test minimum data length
      int[] ranges = {
        -24, 23, 1,
        -256, -25, 2,
        24, 255, 2,
        256, 266, 3,
        -266, -257, 3,
        65525, 65535, 3,
        -65536, -65525, 3,
        65536, 65546, 5,
        -65547, -65537, 5,
      };
      string[] bigRanges = {
        "4294967285", "4294967295",
        "4294967296", "4294967306",
        "18446744073709551604", "18446744073709551615",
        "-4294967296", "-4294967286",
        "-4294967306", "-4294967297",
        "-18446744073709551616", "-18446744073709551604",
      };
      int[] bigSizes = { 5, 9, 9, 5, 9, 9 };
      for (int i = 0; i < ranges.Length; i += 3) {
        for (int j = ranges[i]; j <= ranges[i + 1]; ++j) {
          CBORObject bcbor = ToObjectTest.TestToFromObjectRoundTrip(j);
          byte[] bytes = CBORTestCommon.CheckEncodeToBytes(bcbor);
          if (bytes.Length != ranges[i + 2]) {
            string i2s = TestCommon.IntToString(j);
            Assert.AreEqual(
              ranges[i + 2],
              bytes.Length,
              i2s);
          }
          bytes =
ToObjectTest.TestToFromObjectRoundTrip(j).EncodeToBytes(new
CBOREncodeOptions(false, false, true));
          if (bytes.Length != ranges[i + 2]) {
            string i2s = TestCommon.IntToString(j);
            Assert.AreEqual(
              ranges[i + 2],
              bytes.Length,
              i2s);
          }
        }
      }
      string veryLongString = TestCommon.Repeat("x", 10000);
      byte[] stringBytes =
        ToObjectTest.TestToFromObjectRoundTrip(veryLongString)
        .EncodeToBytes(new CBOREncodeOptions(false, false, true));
      Assert.AreEqual(10003, stringBytes.Length);
      stringBytes = ToObjectTest.TestToFromObjectRoundTrip(veryLongString)
        .EncodeToBytes(new CBOREncodeOptions(false, true));
      Assert.AreEqual(10003, stringBytes.Length);
      for (int i = 0; i < bigRanges.Length; i += 2) {
        EInteger bj = EInteger.FromString(bigRanges[i]);
        EInteger valueBjEnd = EInteger.FromString(bigRanges[i + 1]);
        while (bj < valueBjEnd) {
          CBORObject cbor = ToObjectTest.TestToFromObjectRoundTrip(bj);
          byte[] bytes = CBORTestCommon.CheckEncodeToBytes(cbor);
          if (bytes.Length != bigSizes[i / 2]) {
            Assert.Fail(bj.ToString() + "\n" +
              TestCommon.ToByteArrayString(bytes));
          }
          bytes = ToObjectTest.TestToFromObjectRoundTrip(bj)
            .EncodeToBytes(new CBOREncodeOptions(false, false, true));
          if (bytes.Length != bigSizes[i / 2]) {
            Assert.Fail(bj.ToString() + "\n" +
              TestCommon.ToByteArrayString(bytes));
          }
          bj += EInteger.One;
        }
      }
      try {
        CBORObject.True.EncodeToBytes(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestItem() {
      CBORObject cbor;
      CBORObject dummy = CBORObject.True;
      cbor = CBORObject.NewArray().Add(1).Add(2);
      Assert.AreEqual(1, cbor[0].AsInt32());
      Assert.AreEqual(2, cbor[1].AsInt32());
      Assert.AreEqual(1, cbor[CBORObject.FromObject(0)].AsInt32());
      Assert.AreEqual(2, cbor[CBORObject.FromObject(1)].AsInt32());
      try {
        dummy = cbor[-1];
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        dummy = cbor[2];
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        dummy = cbor[CBORObject.FromObject(-1)];
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        dummy = cbor[CBORObject.FromObject(2)];
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor[0] = CBORObject.FromObject(3);
      cbor[1] = CBORObject.FromObject(4);
      Assert.AreEqual(3, cbor[0].AsInt32());
      Assert.AreEqual(4, cbor[1].AsInt32());
      Assert.AreEqual(3, cbor[CBORObject.FromObject(0)].AsInt32());
      Assert.AreEqual(4, cbor[CBORObject.FromObject(1)].AsInt32());
      try {
        cbor[-1] = dummy;
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor[2] = dummy;
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor[CBORObject.FromObject(-1)] = dummy;
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor[CBORObject.FromObject(2)] = dummy;
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      var bytes = new byte[] { 1, 2, 3, 4 };
      var othercbor = new CBORObject[] {
        CBORObject.FromObject(9), CBORObject.True,
        CBORObject.FromObject(bytes),
        CBORObject.False, CBORObject.Null, CBORObject.FromObject("test"),
        CBORObject.FromObject(99999), CBORObject.FromObject(-1),
      };
      foreach (CBORObject c2 in othercbor) {
        try {
          dummy = c2[0];
          Assert.Fail("Should have failed");
        } catch (InvalidOperationException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        try {
          dummy = c2[CBORObject.FromObject(0)];
          Assert.Fail("Should have failed");
        } catch (InvalidOperationException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      cbor = CBORObject.NewMap().Add(0, 1).Add(-1, 2);
      Assert.AreEqual(1, cbor[0].AsInt32());
      Assert.AreEqual(2, cbor[-1].AsInt32());
      Assert.AreEqual(1, cbor[CBORObject.FromObject(0)].AsInt32());
      Assert.AreEqual(2, cbor[CBORObject.FromObject(-1)].AsInt32());
      if (cbor[-2] != null) {
        Assert.Fail();
      }
      if (cbor[2] != null) {
        Assert.Fail();
      }
      if (cbor["test"] != null) {
        Assert.Fail();
      }
      if (cbor[CBORObject.FromObject(-2)] != null) {
        Assert.Fail();
      }
      if (cbor[CBORObject.FromObject(2)] != null) {
        Assert.Fail();
      }
      if (cbor[CBORObject.FromObject("test")] != null) {
        Assert.Fail();
      }
      cbor[0] = CBORObject.FromObject(3);
      cbor[-1] = CBORObject.FromObject(4);
      Assert.AreEqual(3, cbor[0].AsInt32());
      Assert.AreEqual(4, cbor[-1].AsInt32());
      Assert.AreEqual(3, cbor[CBORObject.FromObject(0)].AsInt32());
      Assert.AreEqual(4, cbor[CBORObject.FromObject(-1)].AsInt32());
      try {
        cbor[-2] = dummy;
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(dummy, cbor[-2]);
      try {
        cbor[2] = dummy;
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor[CBORObject.FromObject(-2)] = dummy;
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor[CBORObject.FromObject(2)] = dummy;
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(dummy, cbor[2]);
      try {
        cbor[CBORObject.FromObject(-5)] = dummy;
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(dummy, cbor[-5]);
      try {
        cbor[CBORObject.FromObject(5)] = dummy;
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(dummy, cbor[-5]);
    }

    [Test]
    public void TestEquals() {
      var cborbytes = new byte[] {
        (byte)0xd8, 0x1e, (byte)0x82, 0x00, 0x19,
        0x0f, 0x50,
      };
      CBORObject cbor = CBORObject.DecodeFromBytes(cborbytes);
      CBORObject cbor2 = CBORObject.DecodeFromBytes(cborbytes);
      TestCommon.CompareTestEqualAndConsistent(cbor, cbor2);
      ERational erat = ERational.Create(0, 3920);
      cbor2 = ToObjectTest.TestToFromObjectRoundTrip(erat);
      TestCommon.CompareTestEqualAndConsistent(cbor, cbor2);
      cbor2 = ToObjectTest.TestToFromObjectRoundTrip(cbor2);
      TestCommon.CompareTestEqualAndConsistent(cbor, cbor2);
      TestWriteObj(erat, erat);
      erat = ERational.Create(
          EInteger.Zero,
          EInteger.FromString("84170882933504200501581262010093"));
      cbor = ToObjectTest.TestToFromObjectRoundTrip(erat);
      ERational erat2 = ERational.Create(
          EInteger.Zero,
          EInteger.FromString("84170882933504200501581262010093"));
      cbor2 = ToObjectTest.TestToFromObjectRoundTrip(erat2);
      TestCommon.CompareTestEqualAndConsistent(cbor, cbor2);
      cbor2 = ToObjectTest.TestToFromObjectRoundTrip(cbor2);
      TestCommon.CompareTestEqualAndConsistent(cbor, cbor2);
      TestWriteObj(cbor, cbor2);
      TestWriteObj(erat, erat2);
    }

    private static void CompareTestNumber(CBORObject o1, CBORObject o2) {
      TestCommon.CompareTestEqual(o1.AsNumber(), o2.AsNumber());
    }

    [Test]
    public void TestEquivalentNegativeInfinity() {
      CompareTestNumber(
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecNegInf),
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf));
      {
        CBORObject objectTemp =
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecNegInf);
        CBORObject objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf);
        CompareTestNumber(objectTemp, objectTemp2);
      }
      {
        CBORObject objectTemp =
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecNegInf);
        CBORObject objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity);
        CompareTestNumber(objectTemp, objectTemp2);
      }
      {
        CBORObject objectTemp =
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf);
        CBORObject objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity);
        CompareTestNumber(objectTemp, objectTemp2);
      }
      {
        CBORObject objectTemp =
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf);
        CBORObject objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity);
        CompareTestNumber(objectTemp, objectTemp2);
      }
      {
        CBORObject objectTemp =
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf);
        CBORObject objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf);
        CompareTestNumber(objectTemp, objectTemp2);
      }
    }

    [Test]
    public void TestEquivalentPositiveInfinity() {
      CompareTestNumber(
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecPosInf),
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatPosInf));
      {
        CBORObject objectTemp =
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecPosInf);
        CBORObject objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.RatPosInf);
        CompareTestNumber(objectTemp, objectTemp2);
      }
      {
        CBORObject objectTemp =
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecPosInf);
        CBORObject objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity);
        CompareTestNumber(objectTemp, objectTemp2);
      }
      {
        CBORObject objectTemp =
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatPosInf);
        CBORObject objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity);
        CompareTestNumber(objectTemp, objectTemp2);
      }
      {
        CBORObject objectTemp =
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.RatPosInf);
        CBORObject objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity);
        CompareTestNumber(objectTemp, objectTemp2);
      }
      {
        CBORObject objectTemp =
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatPosInf);
        CBORObject objectTemp2 =
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.RatPosInf);
        CompareTestNumber(objectTemp, objectTemp2);
      }
    }

    [Test]
    public void TestFalse() {
      CBORTestCommon.AssertJSONSer(CBORObject.False, "false");
      Assert.AreEqual(
        CBORObject.False,
        ToObjectTest.TestToFromObjectRoundTrip(false));
    }

    [Test]
    public void TestFromJSONString() {
      var charbuf = new char[4];
      CBORObject cbor;
      // Test single-character strings
      for (var i = 0; i < 0x110000; ++i) {
        if (i >= 0xd800 && i < 0xe000) {
          continue;
        }
        string str = CharString(i, true, charbuf);
        if (i < 0x20 || i == 0x22 || i == 0x5c) {
          TestFailingJSON(str);
        } else {
          cbor = TestSucceedingJSON(str);
          string exp = CharString(i, false, charbuf);
          if (!exp.Equals(cbor.AsString(), StringComparison.Ordinal)) {
            Assert.AreEqual(exp, cbor.AsString());
          }
        }
      }
      foreach (string str in ValueJsonFails) {
        TestFailingJSON(str);
      }
      foreach (string str in ValueJsonSucceeds) {
        TestSucceedingJSON(str);
      }
      try {
        CBORObject.FromJSONString("\ufeff\u0020 {}");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[]", (CBOREncodeOptions)null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[]", (JSONOptions)null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      TestFailingJSON("{\"a\":1,\"a\":2}", ValueNoDuplicateKeys);
      string aba = "{\"a\":1,\"b\":3,\"a\":2}";
      TestFailingJSON(aba, ValueNoDuplicateKeys);
      cbor = TestSucceedingJSON(aba, new JSONOptions("allowduplicatekeys=1"));
      Assert.AreEqual(ToObjectTest.TestToFromObjectRoundTrip(2), cbor["a"]);
      aba = "{\"a\":1,\"a\":4}";
      cbor = TestSucceedingJSON(aba, new JSONOptions("allowduplicatekeys=1"));
      Assert.AreEqual(ToObjectTest.TestToFromObjectRoundTrip(4), cbor["a"]);
      cbor = TestSucceedingJSON("\"\\t\"");
      {
        string stringTemp = cbor.AsString();
        Assert.AreEqual(
          "\t",
          stringTemp);
      }
      Assert.AreEqual(CBORObject.True, TestSucceedingJSON("true"));
      Assert.AreEqual(CBORObject.False, TestSucceedingJSON("false"));
      Assert.AreEqual(CBORObject.Null, TestSucceedingJSON("null"));
      Assert.AreEqual(5, TestSucceedingJSON(" 5 ").AsInt32());
      {
        string stringTemp = TestSucceedingJSON("\"\\/\\b\"").AsString();
        Assert.AreEqual(
          "/\b",
          stringTemp);
      }
      {
        string stringTemp = TestSucceedingJSON("\"\\/\\f\"").AsString();
        Assert.AreEqual(
          "/\f",
          stringTemp);
      }
      string jsonTemp = TestCommon.Repeat(
          "[",
          2000) + TestCommon.Repeat(
          "]",
          2000);
      TestFailingJSON(jsonTemp);
    }

    [Test]
    public void TestTagArray() {
      CBORObject obj = CBORObject.FromObjectAndTag("test", 999);
      EInteger[] etags = obj.GetAllTags();
      Assert.AreEqual(1, etags.Length);
      Assert.AreEqual(999, etags[0].ToInt32Checked());
      obj = ToObjectTest.TestToFromObjectRoundTrip("test");
      etags = obj.GetAllTags();
      Assert.AreEqual(0, etags.Length);
    }
    [Test]
    public void TestEI() {
      CBORObject cbor =
        ToObjectTest.TestToFromObjectRoundTrip(EInteger.FromString("100"));
      Assert.IsTrue(cbor.IsNumber);
      {
        string stringTemp = cbor.ToJSONString();
        Assert.AreEqual(
          "100",
          stringTemp);
      }
      cbor = ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
            "200"));
      Assert.IsTrue(cbor.IsNumber);
      {
        string stringTemp = cbor.ToJSONString();
        Assert.AreEqual(
          "200",
          stringTemp);
      }
      cbor = ToObjectTest.TestToFromObjectRoundTrip(EFloat.FromString("300"));
      Assert.IsTrue(cbor.IsNumber);
      {
        string stringTemp = cbor.ToJSONString();
        Assert.AreEqual(
          "300",
          stringTemp);
      }
      cbor = ToObjectTest.TestToFromObjectRoundTrip(ERational.Create(1, 2));
      Assert.IsTrue(cbor.IsNumber);
    }
    [Test]
    public void TestFromObject() {
      var cborarray = new CBORObject[2];
      cborarray[0] = CBORObject.False;
      cborarray[1] = CBORObject.True;
      CBORObject cbor = CBORObject.FromObject(cborarray);
      Assert.AreEqual(2, cbor.Count);
      Assert.AreEqual(CBORObject.False, cbor[0]);
      Assert.AreEqual(CBORObject.True, cbor[1]);
      CBORTestCommon.AssertRoundTrip(cbor);
      Assert.AreEqual(
        CBORObject.Null,
        CBORObject.FromObject((int[])null));
      long[] longarray = { 2, 3 };
      cbor = CBORObject.FromObject(longarray);
      Assert.AreEqual(2, cbor.Count);
      Assert.IsTrue(CBORObject.FromObject(2).CompareTo(cbor[0])
        == 0);
      Assert.IsTrue(CBORObject.FromObject(3).CompareTo(cbor[1])
        == 0);
      CBORTestCommon.AssertRoundTrip(cbor);
      Assert.AreEqual(
        CBORObject.Null,
        CBORObject.FromObject((ERational)null));
      Assert.AreEqual(
        CBORObject.Null,
        CBORObject.FromObject((EDecimal)null));
      try {
        CBORObject.FromObject(ERational.Create(10, 2));
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        CBORObject.FromObject(CBORObject.FromObject(Double.NaN)
          .Sign);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.True;
      try {
        CBORObject.FromObject(cbor[0]);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor[0] = CBORObject.False;
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor = CBORObject.False;
        CBORObject.FromObject(cbor.Keys);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip('\udddd');
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(CBORObject.NewArray().Keys);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(CBORObject.NewArray().Sign);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(CBORObject.NewMap().Sign);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    private void CheckKeyValue(CBORObject o, string key, object value) {
      if (!o.ContainsKey(key)) {
        Assert.Fail("Expected " + key + " to exist: " + o.ToString());
      }
      TestCommon.AssertEqualsHashCode(o[key], value);
    }

    public enum EnumClass {
      /// <summary>Internal API.</summary>
      Value1,

      /// <summary>Internal API.</summary>
      Value2,

      /// <summary>Internal API.</summary>
      Value3,
    }

    [Test]
    public void TestFromObject_Enum() {
      CBORObject cbor;
      cbor = ToObjectTest.TestToFromObjectRoundTrip(EnumClass.Value1);
      Assert.AreEqual(0, cbor.AsInt32());
      cbor = ToObjectTest.TestToFromObjectRoundTrip(EnumClass.Value2);
      Assert.AreEqual(1, cbor.AsInt32());
      cbor = ToObjectTest.TestToFromObjectRoundTrip(EnumClass.Value3);
      Assert.AreEqual(2, cbor.AsInt32());
    }

    [Test]
    public void TestToObject_Enum() {
      CBORObject cbor;
      EnumClass ec;
      cbor = CBORObject.FromObject("Value1");
      ec = (EnumClass)cbor.ToObject(typeof(EnumClass));
      Assert.AreEqual(EnumClass.Value1, ec);
      cbor = CBORObject.FromObject("Value2");
      ec = (EnumClass)cbor.ToObject(typeof(EnumClass));
      Assert.AreEqual(EnumClass.Value2, ec);
      cbor = CBORObject.FromObject("Value3");
      ec = (EnumClass)cbor.ToObject(typeof(EnumClass));
      Assert.AreEqual(EnumClass.Value3, ec);
      cbor = CBORObject.FromObject("ValueXYZ");
      try {
        cbor.ToObject(typeof(EnumClass));
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.FromObject(true);
      try {
        cbor.ToObject(typeof(EnumClass));
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestToObject_UnknownEnum() {
      CBORObject cbor;
      cbor = CBORObject.FromObject(999);
      try {
        cbor.ToObject(typeof(EnumClass));
        Assert.Fail("Should have failed -- " +
          cbor.ToObject(typeof(EnumClass)));
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    private sealed class TestConverter : ICBORToFromConverter<string> {
      public CBORObject ToCBORObject(string strValue) {
        return CBORObject.FromObject(
            DataUtilities.ToLowerCaseAscii(strValue));
      }

      public string FromCBORObject(CBORObject cbor) {
        if (cbor == null) {
          throw new ArgumentNullException(nameof(cbor));
        }
        if (cbor.Type == CBORType.TextString) {
          return DataUtilities.ToLowerCaseAscii(cbor.AsString());
        }
        throw new CBORException();
      }
    }

    [Test]
    public void TestFromObject_TypeMapper() {
      var mapper = new CBORTypeMapper()
      .AddConverter(typeof(string), new TestConverter());
      CBORObject cbor = CBORObject.FromObject("UPPER", mapper);
      Assert.AreEqual(CBORType.TextString, cbor.Type);
      {
        string stringTemp = cbor.AsString();
        Assert.AreEqual(
          "upper",
          stringTemp);
      }
      cbor = CBORObject.FromObject("LoWeR", mapper);
      Assert.AreEqual(CBORType.TextString, cbor.Type);
      {
        string stringTemp = cbor.AsString();
        Assert.AreEqual(
          "lower",
          stringTemp);
      }
    }

    [Test]
    public void TestFromObject_Dictionary() {
      IDictionary<string, string> dict = new Dictionary<string, string>();
      dict["TestKey"] = "TestValue";
      dict["TestKey2"] = "TestValue2";
      CBORObject c = CBORObject.FromObject(dict);
      this.CheckKeyValue(c, "TestKey", "TestValue");
      this.CheckKeyValue(c, "TestKey2", "TestValue2");
      dict = (IDictionary<string, string>)c.ToObject(
          typeof(IDictionary<string, string>));
      Assert.AreEqual(2, dict.Keys.Count);
      Assert.IsTrue(dict.ContainsKey("TestKey"));
      Assert.IsTrue(dict.ContainsKey("TestKey2"));
      Assert.AreEqual("TestValue", dict["TestKey"]);
      Assert.AreEqual("TestValue2", dict["TestKey2"]);
    }

#pragma warning disable CA1034
    // nesting a public type is needed
    // here for testing purposes
    public sealed class NestedPODClass {
      public NestedPODClass() {
        this.PropValue = new PODClass();
      }

      public PODClass PropValue {
        get;
        private set;
      }
    }
#pragma warning restore CA1034

    [Test]
    public void TestBase64Extras() {
      // Base64 tests
      CBORObject o;
      o = CBORObject.FromObjectAndTag(
          new byte[] { 0x9a, 0xd6, 0xf0, 0xe8 },
          23);
      {
        string stringTemp = o.ToJSONString();
        Assert.AreEqual(
          "\"9AD6F0E8\"",
          stringTemp);
      }
      o = ToObjectTest.TestToFromObjectRoundTrip(new byte[] {
        0x9a, 0xd6,
        0xff, 0xe8,
      });
      // Encode with Base64URL by default
      {
        string stringTemp = o.ToJSONString();
        Assert.AreEqual(
          "\"mtb_6A\"",
          stringTemp);
      }
      o = CBORObject.FromObjectAndTag(
          new byte[] { 0x9a, 0xd6, 0xff, 0xe8 },
          22);
      // Encode with Base64
      {
        string stringTemp = o.ToJSONString();
        Assert.AreEqual(
          "\"mtb/6A==\"",
          stringTemp);
      }
      var options = new JSONOptions("base64padding=1");
      o = ToObjectTest.TestToFromObjectRoundTrip(new byte[] {
        0x9a, 0xd6,
        0xff, 0xe8,
      });
      // Encode with Base64URL by default
      {
        string stringTemp = o.ToJSONString(options);
        Assert.AreEqual(
          "\"mtb_6A\"",
          stringTemp);
      }
      o = CBORObject.FromObjectAndTag(
          new byte[] { 0x9a, 0xd6, 0xff, 0xe8 },
          22);
      // Encode with Base64
      {
        string stringTemp = o.ToJSONString(options);
        Assert.AreEqual(
          "\"mtb/6A==\"",
          stringTemp);
      }
    }

    [Test]
    public void TestFromObject_PODOptions() {
      var ao = new PODClass();
      var valueCcTF = new PODOptions(true, false);
      var valueCcFF = new PODOptions(false, false);
      var valueCcFT = new PODOptions(false, true);
      var valueCcTT = new PODOptions(true, true);
      CBORObject co;
      CBORObjectTest.CheckPropertyNames(ao);
      var arrao = new PODClass[] { ao, ao };
      co = CBORObject.FromObject(arrao, valueCcTF);
      CBORObjectTest.CheckArrayPropertyNames(
        CBORObject.FromObject(arrao, valueCcTF),
        2,
        "PropA",
        "PropB",
        "IsPropC");
      CBORObjectTest.CheckArrayPropertyNames(
        CBORObject.FromObject(arrao, valueCcFT),
        2,
        "propA",
        "propB",
        "propC");
      CBORObjectTest.CheckArrayPropertyNames(
        CBORObject.FromObject(arrao, valueCcTT),
        2,
        "propA",
        "propB",
        "propC");
      var ao2 = new NestedPODClass();
      CBORObjectTest.CheckPODPropertyNames(
        CBORObject.FromObject(ao2, valueCcTF),
        valueCcTF,
        "PropA",
        "PropB",
        "IsPropC");
      CBORObjectTest.CheckPODPropertyNames(
        CBORObject.FromObject(ao2, valueCcFT),
        valueCcFT,
        "propA",
        "propB",
        "propC");
      CBORObjectTest.CheckPODPropertyNames(
        CBORObject.FromObject(ao2, valueCcTT),
        valueCcTT,
        "propA",
        "propB",
        "propC");
      var aodict = new Dictionary<string, object>();
      aodict["PropValue"] = new PODClass();

      CBORObjectTest.CheckPODInDictPropertyNames(
        CBORObject.FromObject(aodict, valueCcTF),
        "PropA",
        "PropB",
        "IsPropC");
      CBORObjectTest.CheckPODInDictPropertyNames(
        CBORObject.FromObject(aodict, valueCcFT),
        "propA",
        "propB",
        "propC");
      CBORObjectTest.CheckPODInDictPropertyNames(
        CBORObject.FromObject(aodict, valueCcTT),
        "propA",
        "propB",
        "propC");
      CBORObjectTest.CheckArrayPropertyNames(
        CBORObject.FromObject(arrao, valueCcFF),
        2,
        "PropA",
        "PropB",
        "IsPropC");
      CBORObjectTest.CheckPODPropertyNames(
        CBORObject.FromObject(ao2, valueCcFF),
        valueCcFF,
        "PropA",
        "PropB",
        "IsPropC");
      CBORObjectTest.CheckPODInDictPropertyNames(
        CBORObject.FromObject(aodict, valueCcFF),
        "PropA",
        "PropB",
        "IsPropC");
    }

    [Test]
    public void TestFromObjectAndTag() {
      EInteger bigvalue = EInteger.FromString("99999999999999999999999999999");
      try {
        CBORObject.FromObjectAndTag(2, bigvalue);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObjectAndTag(2, -1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObjectAndTag(CBORObject.Null, -1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObjectAndTag(CBORObject.Null, 999999);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      EInteger eintNull = null;
      try {
        CBORObject.FromObjectAndTag(2, eintNull);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObjectAndTag(2, EInteger.FromString("-1"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestFromSimpleValue() {
      try {
        CBORObject.FromSimpleValue(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromSimpleValue(256);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      for (int i = 0; i < 256; ++i) {
        if (i >= 24 && i < 32) {
          try {
            CBORObject.FromSimpleValue(i);
            Assert.Fail("Should have failed");
          } catch (ArgumentException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        } else {
          CBORObject cbor = CBORObject.FromSimpleValue(i);
          Assert.AreEqual(i, cbor.SimpleValue);
        }
      }
    }
    [Test]
    public void TestGetByteString() {
      try {
        CBORObject.True.GetByteString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0).GetByteString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip("test").GetByteString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.GetByteString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().GetByteString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().GetByteString();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestGetHashCode() {
      // not implemented yet
    }
    [Test]
    public void TestGetTags() {
      // not implemented yet
    }
    [Test]
    public void TestHasTag() {
      try {
        CBORObject.True.HasTag(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        const EInteger ValueBigintNull = null;
        CBORObject.True.HasTag(ValueBigintNull);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.HasTag(EInteger.FromString("-1"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.IsFalse(CBORObject.True.HasTag(0));
      Assert.IsFalse(CBORObject.True.HasTag(EInteger.Zero));
    }
    [Test]
    public void TestMostInnerTag() {
      // not implemented yet
    }
    [Test]
    public void TestInsert() {
      // not implemented yet
    }
    [Test]
    public void TestIsFalse() {
      // not implemented yet
    }
    [Test]
    public void TestIsFinite() {
      CBORObject cbor;
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(0).IsFinite);
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .IsFinite);
      Assert.IsFalse(CBORObject.NewArray().IsFinite);
      Assert.IsFalse(CBORObject.NewMap().IsFinite);
      cbor = CBORObject.True;
      Assert.IsFalse(cbor.IsFinite);
      cbor = CBORObject.False;
      Assert.IsFalse(cbor.IsFinite);
      cbor = CBORObject.Null;
      Assert.IsFalse(cbor.IsFinite);
      cbor = CBORObject.Undefined;
      Assert.IsFalse(cbor.IsFinite);
      Assert.IsFalse(CBORObject.NewMap().IsFinite);
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(0).IsFinite);
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(2.5).IsFinite);
      Assert.IsFalse(
        ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity)
        .IsFinite);

      Assert.IsFalse(
        ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity)
        .IsFinite);
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(Double.NaN)
        .IsFinite);
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(
          CBORTestCommon.DecPosInf).IsFinite);
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(
          CBORTestCommon.DecNegInf).IsFinite);
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(EDecimal.NaN)
        .IsFinite);
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        EDecimal ed = EDecimal.FromString(numberinfo["number"].AsString());
        CBORObject cbornumber = ToObjectTest.TestToFromObjectRoundTrip(ed);
        if (numberinfo["isintegral"].AsBoolean()) {
          Assert.IsTrue(cbornumber.IsFinite, numberinfo["number"].AsString());
        }
        // NOTE: A nonintegral number is not necessarily non-finite
      }
    }
    [Test]
    public void TestIsInfinity() {
      Assert.IsTrue(CBORObject.PositiveInfinity.AsNumber().IsInfinity());
      Assert.IsTrue(CBORObject.NegativeInfinity.AsNumber().IsInfinity());
      Assert.IsTrue(CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xfa, 0x7f,
        (byte)0x80, 0x00, 0x00,
      }).AsNumber().IsInfinity());
    }

    [Test]
    public void TestIsIntegral() {
      CBORObject cbor;
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(0).IsIntegral);
      cbor = ToObjectTest.TestToFromObjectRoundTrip(String.Empty);
      Assert.IsFalse(cbor.IsIntegral);
      Assert.IsFalse(CBORObject.NewArray().IsIntegral);
      Assert.IsFalse(CBORObject.NewMap().IsIntegral);
      cbor = ToObjectTest.TestToFromObjectRoundTrip(
          EInteger.FromRadixString(
            "8000000000000000",
            16));
      Assert.IsTrue(cbor.IsIntegral);
      cbor = ToObjectTest.TestToFromObjectRoundTrip(
          EInteger.FromRadixString(
            "80000000000000000000",
            16));
      Assert.IsTrue(cbor.IsIntegral);
      cbor = ToObjectTest.TestToFromObjectRoundTrip(
          EInteger.FromRadixString(
            "8000000000000000000000000",
            16));
      Assert.IsTrue(cbor.IsIntegral);
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(
          EDecimal.FromString("4444e+800")).IsIntegral);

      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(
          EDecimal.FromString("4444e-800")).IsIntegral);
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(2.5).IsIntegral);
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(
          999.99).IsIntegral);
      cbor = ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity);

      Assert.IsFalse(cbor.IsIntegral);

      cbor = ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity);

      Assert.IsFalse(cbor.IsIntegral);
      cbor = ToObjectTest.TestToFromObjectRoundTrip(Double.NaN);

      Assert.IsFalse(cbor.IsIntegral);
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(
          CBORTestCommon.DecPosInf).IsIntegral);
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(
          CBORTestCommon.DecNegInf).IsIntegral);
      cbor = ToObjectTest.TestToFromObjectRoundTrip(EDecimal.NaN);

      Assert.IsFalse(cbor.IsIntegral);
      cbor = CBORObject.True;
      Assert.IsFalse(cbor.IsIntegral);
      cbor = CBORObject.False;
      Assert.IsFalse(cbor.IsIntegral);
      cbor = CBORObject.Null;
      Assert.IsFalse(cbor.IsIntegral);
      cbor = CBORObject.Undefined;
      Assert.IsFalse(cbor.IsIntegral);
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              numberinfo["number"].AsString()));
        if (numberinfo["isintegral"].AsBoolean()) {
          Assert.IsTrue(cbornumber.IsIntegral);
          Assert.IsFalse(cbornumber.AsNumber().IsPositiveInfinity());
          Assert.IsFalse(cbornumber.AsNumber().IsNegativeInfinity());
          Assert.IsFalse(cbornumber.AsNumber().IsNaN());
          Assert.IsFalse(cbornumber.IsNull);
        } else {
          Assert.IsFalse(cbornumber.IsIntegral);
        }
      }
    }
    [Test]
    public void TestAsNumber() {
      try {
        CBORObject.True.AsNumber();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty).AsNumber();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().AsNumber();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsNumber();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsNumber();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Null.AsNumber();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsNumber();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestAsNumberIsNegativeInfinity() {
      Assert.IsFalse(CBORObject.FromObject(
  0).AsNumber().IsNegativeInfinity());

      Assert.IsFalse(
        CBORObject.PositiveInfinity.AsNumber().IsNegativeInfinity());

      Assert.IsTrue(
        CBORObject.NegativeInfinity.AsNumber().IsNegativeInfinity());
      Assert.IsFalse(CBORObject.NaN.AsNumber().IsNegativeInfinity());
    }
    [Test]
    public void TestIsNull() {
      Assert.IsFalse(CBORObject.True.IsNull);
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .IsNull);
      Assert.IsFalse(CBORObject.NewArray().IsNull);
      Assert.IsFalse(CBORObject.NewMap().IsNull);
      Assert.IsFalse(CBORObject.False.IsNull);
      Assert.IsTrue(CBORObject.Null.IsNull);
      Assert.IsFalse(CBORObject.Undefined.IsNull);
      Assert.IsFalse(CBORObject.PositiveInfinity.IsNull);
      Assert.IsFalse(CBORObject.NegativeInfinity.IsNull);
      Assert.IsFalse(CBORObject.NaN.IsNull);
    }
    [Test]
    public void TestAsNumberIsPositiveInfinity() {
      Assert.IsFalse(CBORObject.FromObject(
  0).AsNumber().IsPositiveInfinity());

      Assert.IsTrue(
        CBORObject.PositiveInfinity.AsNumber().IsPositiveInfinity());

      Assert.IsFalse(
        CBORObject.NegativeInfinity.AsNumber().IsPositiveInfinity());
      Assert.IsFalse(CBORObject.NaN.AsNumber().IsPositiveInfinity());
    }
    [Test]
    public void TestIsTagged() {
      // not implemented yet
    }
    [Test]
    public void TestIsTrue() {
      // not implemented yet
    }
    [Test]
    public void TestIsUndefined() {
      Assert.IsFalse(CBORObject.True.IsUndefined);
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .IsUndefined);
      Assert.IsFalse(CBORObject.NewArray().IsUndefined);
      Assert.IsFalse(CBORObject.NewMap().IsUndefined);
      Assert.IsFalse(CBORObject.False.IsUndefined);
      Assert.IsFalse(CBORObject.Null.IsUndefined);
      Assert.IsTrue(CBORObject.Undefined.IsUndefined);
      Assert.IsFalse(CBORObject.PositiveInfinity.IsUndefined);
      Assert.IsFalse(CBORObject.NegativeInfinity.IsUndefined);
      Assert.IsFalse(CBORObject.NaN.IsUndefined);
    }
    [Test]
    public void TestIsZero() {
      // not implemented yet
    }
    [Test]
    public void TestItem2() {
      CBORObject cbor = CBORObject.True;
      try {
        CBORObject cbor2 = cbor[0];
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.False;
      try {
        CBORObject cbor2 = cbor[0];
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = ToObjectTest.TestToFromObjectRoundTrip(0);
      try {
        CBORObject cbor2 = cbor[0];
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = ToObjectTest.TestToFromObjectRoundTrip(2);
      try {
        CBORObject cbor2 = cbor[0];
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewArray();
      try {
        CBORObject cbor2 = cbor[0];
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestGetOrDefault() {
      CBORObject cbor = CBORObject.NewArray().Add(2).Add(3).Add(7);
      {
        object objectTemp = CBORObject.Null;
        object objectTemp2 = cbor.GetOrDefault(-1,
            CBORObject.Null);
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = CBORObject.FromObject(2);
        object objectTemp2 = cbor.GetOrDefault(
            0,
            CBORObject.Null);
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      Assert.AreEqual(
        CBORObject.FromObject(2),
        cbor.GetOrDefault(CBORObject.FromObject(0), CBORObject.Null));
      {
        object objectTemp = CBORObject.FromObject(3);
        object objectTemp2 = cbor.GetOrDefault(
            1,
            CBORObject.Null);
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = CBORObject.FromObject(7);
        object objectTemp2 = cbor.GetOrDefault(
            2,
            CBORObject.Null);
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      Assert.AreEqual(CBORObject.Null, cbor.GetOrDefault(3, CBORObject.Null));
      {
        object objectTemp = CBORObject.Null;
        object objectTemp2 = cbor.GetOrDefault("key",
            CBORObject.Null);
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      cbor = CBORObject.NewMap().Add(1, 2).Add("key", "value");
      {
        object objectTemp = CBORObject.Null;
        object objectTemp2 = cbor.GetOrDefault(-1,
            CBORObject.Null);
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      Assert.AreEqual(CBORObject.Null, cbor.GetOrDefault(0, CBORObject.Null));
      {
        object objectTemp = CBORObject.FromObject(2);
        object objectTemp2 = cbor.GetOrDefault(
            1,
            CBORObject.Null);
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      Assert.AreEqual(CBORObject.Null, cbor.GetOrDefault(2, CBORObject.Null));
      Assert.AreEqual(CBORObject.Null, cbor.GetOrDefault(3, CBORObject.Null));
      {
        object objectTemp = CBORObject.FromObject("value");
        object objectTemp2 = cbor.GetOrDefault(
            "key",
            CBORObject.Null);
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      Assert.AreEqual(
        CBORObject.FromObject("value"),
        cbor.GetOrDefault(CBORObject.FromObject("key"), CBORObject.Null));
      {
        object objectTemp = CBORObject.Null;
        object objectTemp2 = cbor.GetOrDefault("key2",
            CBORObject.Null);
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      cbor = CBORObject.False;
      {
        object objectTemp = CBORObject.Null;
        object objectTemp2 = cbor.GetOrDefault(-1,
            CBORObject.Null);
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      Assert.AreEqual(CBORObject.Null, cbor.GetOrDefault(0, CBORObject.Null));
      {
        object objectTemp = CBORObject.Null;
        object objectTemp2 = cbor.GetOrDefault("key",
            CBORObject.Null);
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = CBORObject.Null;
        object objectTemp2 = cbor.GetOrDefault("key2",
            CBORObject.Null);
        Assert.AreEqual(objectTemp, objectTemp2);
      }
    }

    private void Sink(object obj) {
      Console.WriteLine("Sink for " + obj);
      Assert.Fail();
    }

    [Test]
    public void TestKeys() {
      CBORObject co;
      try {
        co = CBORObject.True;
        this.Sink(co.Keys);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        this.Sink(ToObjectTest.TestToFromObjectRoundTrip(0).Keys);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        this.Sink(ToObjectTest.TestToFromObjectRoundTrip("string").Keys);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        this.Sink(CBORObject.NewArray().Keys);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        this.Sink(ToObjectTest.TestToFromObjectRoundTrip(
            new byte[] { 0 }).Keys);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      if (CBORObject.NewMap().Keys == null) {
        Assert.Fail();
      }
    }
    [Test]
    public void TestAsNumberMultiply() {
      var r = new RandomGenerator();
      for (var i = 0; i < 3000; ++i) {
        CBORObject o1 = CBORTestCommon.RandomNumber(r);
        CBORObject o2 = CBORTestCommon.RandomNumber(r);
        EDecimal cmpDecFrac = AsED(o1).Multiply(AsED(o2));
        EDecimal cmpCobj = o1.AsNumber().Multiply(o2.AsNumber()).ToEDecimal();
        if (!cmpDecFrac.Equals(cmpCobj)) {
          TestCommon.CompareTestEqual(
            cmpDecFrac,
            cmpCobj,
            o1.ToString() + "\n" + o2.ToString());
        }
      }
    }
    [Test]
    public void TestAsNumberNegate() {
      TestCommon.CompareTestEqual(
        ToObjectTest.TestToFromObjectRoundTrip(2).AsNumber(),
        ToObjectTest.TestToFromObjectRoundTrip(-2).AsNumber().Negate());
      TestCommon.CompareTestEqual(
        ToObjectTest.TestToFromObjectRoundTrip(-2).AsNumber(),
        ToObjectTest.TestToFromObjectRoundTrip(2).AsNumber().Negate());
    }

    [Test]
    public void TestNegativeTenDigitLong() {
      CBORObject obj = CBORObject.FromJSONString("-1000000000");
      {
        string stringTemp = obj.ToJSONString();
        Assert.AreEqual(
          "-1000000000",
          stringTemp);
      }
      {
        string stringTemp = obj.ToString();
        Assert.AreEqual(
          "-1000000000",
          stringTemp);
      }
    }

    [Test]
    public void TestNegativeZero() {
      CBORObject negzero = ToObjectTest.TestToFromObjectRoundTrip(
          EDecimal.FromString("-0"));
      CBORTestCommon.AssertRoundTrip(negzero);
    }
    [Test]
    public void TestNewArray() {
      // not implemented yet
    }
    [Test]
    public void TestNewMap() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorAddition() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorDivision() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorModulus() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorMultiply() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorSubtraction() {
      // not implemented yet
    }
    [Test]
    public void TestMostOuterTag() {
      CBORObject cbor = CBORObject.FromObjectAndTag(CBORObject.True, 999);
      cbor = CBORObject.FromObjectAndTag(CBORObject.True, 1000);
      Assert.AreEqual(EInteger.FromString("1000"), cbor.MostOuterTag);
      cbor = CBORObject.True;
      Assert.AreEqual(EInteger.FromString("-1"), cbor.MostOuterTag);
    }
    [Test]
    public void TestRead() {
      try {
        CBORObject.Read(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        using (var ms2 = new MemoryStream(new byte[] { 0 })) {
          try {
            CBORObject.Read(ms2, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestReadJSON() {
      try {
        using (var ms2 = new MemoryStream(new byte[] { 0x30 })) {
          try {
            CBORObject.ReadJSON(ms2, (CBOREncodeOptions)null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            CBORObject.ReadJSON(ms2, (JSONOptions)null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var ms = new MemoryStream(new byte[] {
          0xef, 0xbb, 0xbf,
          0x7b, 0x7d,
        })) {
          try {
            CBORObject.ReadJSON(ms);
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        // whitespace followed by BOM
        using (var ms2 = new MemoryStream(new byte[] {
          0x20, 0xef, 0xbb,
          0xbf, 0x7b, 0x7d,
        })) {
          try {
            CBORObject.ReadJSON(ms2);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var ms2a = new MemoryStream(new byte[] { 0x7b, 0x05, 0x7d })) {
          try {
            CBORObject.ReadJSON(ms2a);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var ms2b = new MemoryStream(new byte[] { 0x05, 0x7b, 0x7d })) {
          try {
            CBORObject.ReadJSON(ms2b);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        // two BOMs
        using (var ms3 = new MemoryStream(new byte[] {
          0xef, 0xbb, 0xbf,
          0xef, 0xbb, 0xbf, 0x7b, 0x7d,
        })) {
          try {
            CBORObject.ReadJSON(ms3);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0, 0, 0xfe, 0xff, 0,
          0,
          0,
          0x74, 0, 0, 0, 0x72, 0, 0, 0, 0x75, 0, 0, 0,
          0x65,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new MemoryStream(new byte[] {
          0, 0, 0, 0x74, 0, 0,
          0, 0x72, 0,
          0, 0, 0x75, 0, 0, 0, 0x65,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xff, 0xfe, 0, 0,
          0x74, 0, 0, 0,
          0x72, 0, 0, 0, 0x75, 0, 0, 0, 0x65, 0, 0, 0,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new MemoryStream(new byte[] {
          0x74, 0, 0, 0, 0x72,
          0,
          0,
          0,
          0x75, 0, 0, 0, 0x65, 0, 0, 0,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xfe, 0xff, 0, 0x74,
          0, 0x72, 0,
          0x75, 0, 0x65,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new MemoryStream(new byte[] {
          0, 0x74, 0, 0x72, 0,
          0x75, 0, 0x65,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xff, 0xfe, 0x74, 0,
          0x72,
          0,
          0x75,
          0, 0x65, 0,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new MemoryStream(new byte[] {
          0x74, 0, 0x72, 0,
          0x75, 0, 0x65, 0,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xef, 0xbb, 0xbf,
          0x74, 0x72, 0x75, 0x65,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new MemoryStream(new byte[] {
          0x74, 0x72, 0x75,
          0x65,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new MemoryStream(new byte[] {
          0, 0, 0xfe, 0xff, 0,
          0, 0, 0x22,
          0, 1, 0, 0, 0, 0, 0, 0x22,
        })) {
          {
            string stringTemp = CBORObject.ReadJSON(msjson).AsString();
            Assert.AreEqual(
              "\ud800\udc00",
              stringTemp);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0, 0, 0, 0x22, 0, 1,
          0, 0, 0, 0,
          0, 0x22,
        })) {
          {
            string stringTemp = CBORObject.ReadJSON(msjson).AsString();
            Assert.AreEqual(
              "\ud800\udc00",
              stringTemp);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xff, 0xfe, 0, 0,
          0x22, 0, 0, 0,
          0, 0, 1, 0, 0x22, 0, 0, 0,
        })) {
          {
            string stringTemp = CBORObject.ReadJSON(msjson).AsString();
            Assert.AreEqual(
              "\ud800\udc00",
              stringTemp);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0x22, 0, 0, 0, 0, 0,
          1, 0, 0x22,
          0,
          0, 0,
        })) {
          {
            string stringTemp = CBORObject.ReadJSON(msjson).AsString();
            Assert.AreEqual(
              "\ud800\udc00",
              stringTemp);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xfe, 0xff, 0,
          0x22, 0xd8,
          0,
          0xdc, 0, 0, 0x22,
        })) {
          {
            string stringTemp = CBORObject.ReadJSON(msjson).AsString();
            Assert.AreEqual(
              "\ud800\udc00",
              stringTemp);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0, 0x22, 0xd8, 0,
          0xdc, 0, 0, 0x22,
        })) {
          {
            string stringTemp = CBORObject.ReadJSON(msjson).AsString();
            Assert.AreEqual(
              "\ud800\udc00",
              stringTemp);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xff, 0xfe, 0x22, 0,
          0, 0xd8, 0,
          0xdc, 0x22, 0,
        })) {
          {
            string stringTemp = CBORObject.ReadJSON(msjson).AsString();
            Assert.AreEqual(
              "\ud800\udc00",
              stringTemp);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0x22, 0, 0, 0xd8, 0,
          0xdc, 0x22, 0,
        })) {
          {
            string stringTemp = CBORObject.ReadJSON(msjson).AsString();
            Assert.AreEqual(
              "\ud800\udc00",
              stringTemp);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0, 0, 0xfe, 0xff, 0,
          0, 0, 0x22,
          0, 0, 0xd8, 0, 0, 0, 0, 0x22,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0, 0, 0, 0x22, 0, 0,
          0xd8, 0, 0,
          0,
          0, 0x22,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xff, 0xfe, 0, 0,
          0x22, 0, 0, 0,
          0, 0xd8, 0, 0, 0x22, 0, 0, 0,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0x22, 0, 0, 0, 0,
          0xd8,
          0,
          0,
          0x22, 0, 0, 0,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xfe, 0xff, 0, 0x22,
          0, 0xdc, 0,
          0xdc, 0, 0, 0x22,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0, 0x22, 0, 0xdc, 0,
          0xdc, 0, 0,
          0x22,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xff, 0xfe, 0x22, 0,
          0, 0xdc, 0,
          0xdc, 0x22, 0,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0x22, 0, 0, 0xdc, 0,
          0xdc, 0x22, 0,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] { 0xfc })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] { 0, 0 })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        // Illegal UTF-16
        using (var msjson = new MemoryStream(new byte[] {
          0xfe, 0xff, 0x20,
          0x20, 0x20,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xff, 0xfe, 0x20,
          0x20, 0x20,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xfe, 0xff, 0xd8,
          0x00,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xfe, 0xff, 0xdc,
          0x00,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xfe, 0xff, 0xd8,
          0x00, 0x20, 0x00,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xfe, 0xff, 0xdc,
          0x00, 0x20, 0x00,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xfe, 0xff, 0xd8,
          0x00, 0xd8, 0x00,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xfe, 0xff, 0xdc,
          0x00, 0xd8, 0x00,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xfe, 0xff, 0xdc,
          0x00, 0xd8, 0x00, 0xdc, 0x00,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xfe, 0xff, 0xdc,
          0x00, 0xdc, 0x00,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }

        using (var msjson = new MemoryStream(new byte[] {
          0xff, 0xfe, 0x00,
          0xd8,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xff, 0xfe, 0x00,
          0xdc,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xff, 0xfe, 0x00,
          0xd8, 0x00, 0x20,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xff, 0xfe, 0x00,
          0xdc, 0x00, 0x20,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xff, 0xfe, 0x00,
          0xd8, 0x00, 0xd8,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xff, 0xfe, 0x00,
          0xdc, 0x00, 0xd8,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xff, 0xfe, 0x00,
          0xdc, 0x00, 0xd8, 0x00, 0xdc,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        using (var msjson = new MemoryStream(new byte[] {
          0xff, 0xfe, 0x00,
          0xdc, 0x00, 0xdc,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }

        // Illegal UTF-32
        using (var msjson = new MemoryStream(new byte[] {
          0, 0, 0, 0x20, 0,
        })) {
          try {
            CBORObject.ReadJSON(msjson);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        var msbytes = new byte[] { 0, 0, 0, 0x20, 0, 0, };
        ReadJsonFail(msbytes);
        msbytes = new byte[] { 0, 0, 0, 0x20, 0, 0, 0 };
        ReadJsonFail(msbytes);
        msbytes = new byte[] {
          0, 0, 0, 0x20, 0, 0,
          0xd8, 0,
        };
        ReadJsonFail(msbytes);
        msbytes = new byte[] {
          0, 0, 0, 0x20, 0, 0,
          0xdc, 0,
        };
        ReadJsonFail(msbytes);
        msbytes = new byte[] {
          0, 0, 0, 0x20, 0,
          0x11, 0x00, 0,
        };
        ReadJsonFail(msbytes);
        msbytes = new byte[] {
          0, 0, 0, 0x20, 0,
          0xff, 0x00, 0,
        };
        ReadJsonFail(msbytes);
        msbytes = new byte[] {
          0, 0, 0, 0x20, 0x1,
          0, 0x00, 0,
        };
        ReadJsonFail(msbytes);
        msbytes = new byte[] { 0, 0, 0xfe, 0xff, 0, };
        ReadJsonFail(msbytes);
        msbytes = new byte[] { 0, 0, 0xfe, 0xff, 0, 0, };
        ReadJsonFail(msbytes);
        msbytes = new byte[] {
          0, 0, 0xfe, 0xff, 0,
          0, 0,
        };
        ReadJsonFail(msbytes);
        msbytes = new byte[] {
          0, 0, 0xfe, 0xff, 0,
          0, 0xd8, 0,
        };
        ReadJsonFail(msbytes);
        msbytes = new byte[] {
          0, 0, 0xfe, 0xff, 0,
          0, 0xdc, 0,
        };
        ReadJsonFail(msbytes);
        msbytes = new byte[] {
          0, 0, 0xfe, 0xff, 0,
          0x11, 0x00, 0,
        };
        ReadJsonFail(msbytes);
        msbytes = new byte[] {
          0, 0, 0xfe, 0xff, 0,
          0xff, 0x00, 0,
        };
        ReadJsonFail(msbytes);
        msbytes = new byte[] { 0, 0, 0xfe, 0xff, 0x1, 0, 0x00, 0, };
        ReadJsonFail(msbytes);
      } catch (IOException ex) {
        Assert.Fail(ex.Message);
      }
    }
    private static void ReadJsonFail(byte[] msbytes) {
      using (var msjson = new MemoryStream(msbytes)) {
        try {
          CBORObject.ReadJSON(msjson);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    }

    // TODO: In next major version, consider using CBORException
    // for circular refs in EncodeToBytes
    [Test]
    public void TestEncodeToBytesCircularRefs() {
      CBORObject cbor = CBORObject.NewArray().Add(1).Add(2);
      cbor.Add(cbor);
      try {
        cbor.EncodeToBytes();
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add(1, 2).Add(3, 4);
      cbor.Add(cbor, "test");
      try {
        cbor.EncodeToBytes();
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add(1, 2).Add(3, 4);
      cbor.Add("test", cbor);
      try {
        cbor.EncodeToBytes();
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewArray().Add(1).Add(2);
      cbor.Add(CBORObject.NewArray().Add(cbor));
      try {
        cbor.EncodeToBytes();
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add(1, 2).Add(3, 4);
      cbor.Add(CBORObject.NewArray().Add(cbor), "test");
      try {
        cbor.EncodeToBytes();
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add(1, 2).Add(3, 4);
      cbor.Add("test", CBORObject.NewArray().Add(cbor));
      try {
        cbor.EncodeToBytes();
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestCalcEncodedSizeCircularRefs() {
      CBORObject cbor = CBORObject.NewArray().Add(1).Add(2);
      cbor.Add(cbor);
      try {
        cbor.CalcEncodedSize();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add(1, 2).Add(3, 4);
      cbor.Add(cbor, "test");
      try {
        cbor.CalcEncodedSize();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add(1, 2).Add(3, 4);
      cbor.Add("test", cbor);
      try {
        cbor.CalcEncodedSize();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewArray().Add(1).Add(2);
      cbor.Add(CBORObject.NewArray().Add(cbor));
      try {
        cbor.CalcEncodedSize();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add(1, 2).Add(3, 4);
      cbor.Add(CBORObject.NewArray().Add(cbor), "test");
      try {
        cbor.CalcEncodedSize();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add(1, 2).Add(3, 4);
      cbor.Add("test", CBORObject.NewArray().Add(cbor));
      try {
        cbor.CalcEncodedSize();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestClear() {
      CBORObject cbor;
      cbor = CBORObject.NewArray().Add("a").Add("b").Add("c");
      Assert.AreEqual(3, cbor.Count);
      cbor.Clear();
      Assert.AreEqual(0, cbor.Count);
      cbor = CBORObject.NewMap()
        .Add("a", 0).Add("b", 1).Add("c", 2);
      Assert.AreEqual(3, cbor.Count);
      cbor.Clear();
      Assert.AreEqual(0, cbor.Count);
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1).Clear();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.Clear();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Null.Clear();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestRemove() {
      CBORObject cbor;
      cbor = CBORObject.NewArray().Add("a").Add("b").Add("c");
      Assert.AreEqual(3, cbor.Count);
      Assert.IsTrue(cbor.Remove(ToObjectTest.TestToFromObjectRoundTrip(
            "b")));
      Assert.IsFalse(cbor.Remove(ToObjectTest.TestToFromObjectRoundTrip(
            "x")));
      try {
        cbor.Remove((CBORObject)null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(2, cbor.Count);
      Assert.AreEqual(ToObjectTest.TestToFromObjectRoundTrip("a"), cbor[0]);
      Assert.AreEqual(ToObjectTest.TestToFromObjectRoundTrip("c"), cbor[1]);
      cbor = CBORObject.NewArray().Add("a").Add("b").Add("c");
      Assert.AreEqual(3, cbor.Count);

      Assert.IsTrue(cbor.Remove("b"));
      Assert.IsFalse(cbor.Remove("x"));
      Assert.AreEqual(2, cbor.Count);
      Assert.AreEqual(ToObjectTest.TestToFromObjectRoundTrip("a"), cbor[0]);
      Assert.AreEqual(ToObjectTest.TestToFromObjectRoundTrip("c"), cbor[1]);
      cbor = CBORObject.NewMap().Add("a", 0).Add("b", 1).Add("c", 2);
      Assert.AreEqual(3, cbor.Count);

      Assert.IsTrue(cbor.Remove(ToObjectTest.TestToFromObjectRoundTrip(
            "b")));
      Assert.IsFalse(cbor.Remove(ToObjectTest.TestToFromObjectRoundTrip(
            "x")));
      try {
        cbor.Remove((CBORObject)null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(2, cbor.Count);
      Assert.IsTrue(cbor.ContainsKey("a"));
      Assert.IsTrue(cbor.ContainsKey("c"));
      cbor = CBORObject.NewMap().Add("a", 0).Add("b", 1).Add("c", 2);
      Assert.AreEqual(3, cbor.Count);

      Assert.IsTrue(cbor.Remove("b"));
      Assert.IsFalse(cbor.Remove("x"));
      Assert.AreEqual(2, cbor.Count);
      Assert.IsTrue(cbor.ContainsKey("a"));
      Assert.IsTrue(cbor.ContainsKey("c"));
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1).Remove("x");
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.Remove("x");
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Null.Remove("x");
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1)
        .Remove(ToObjectTest.TestToFromObjectRoundTrip("b"));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.Remove(ToObjectTest.TestToFromObjectRoundTrip("b"));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Null.Remove(ToObjectTest.TestToFromObjectRoundTrip("b"));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestRemoveAt() {
      CBORObject cbor;
      cbor = CBORObject.NewArray().Add("a").Add("b").Add("c");
      Assert.IsTrue(cbor.RemoveAt(1));
      Assert.IsFalse(cbor.RemoveAt(2));
      Assert.IsFalse(cbor.RemoveAt(-1));
      Assert.AreEqual(2, cbor.Count);
      Assert.AreEqual(ToObjectTest.TestToFromObjectRoundTrip("a"), cbor[0]);
      Assert.AreEqual(ToObjectTest.TestToFromObjectRoundTrip("c"), cbor[1]);
      try {
        CBORObject.NewMap().RemoveAt(0);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1).RemoveAt(0);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.RemoveAt(0);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Null.RemoveAt(0);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestSet() {
      CBORObject cbor = CBORObject.NewMap().Add("x", 0).Add("y", 1);
      Assert.AreEqual(0, cbor["x"].AsInt32());
      Assert.AreEqual(1, cbor["y"].AsInt32());
      cbor.Set("x", 5).Set("z", 6);
      Assert.AreEqual(5, cbor["x"].AsInt32());
      Assert.AreEqual(6, cbor["z"].AsInt32());
      cbor = CBORObject.NewArray().Add(1).Add(2).Add(3).Add(4);
      Assert.AreEqual(1, cbor[0].AsInt32());
      Assert.AreEqual(2, cbor[1].AsInt32());
      Assert.AreEqual(3, cbor[2].AsInt32());
      try {
        cbor.Set(-1, 0);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.Set(4, 0);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.Set(999, 0);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject cbor2 = CBORObject.True;
      try {
        cbor2.Set(0, 0);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor.Set(0, 99);
      Assert.AreEqual(99, cbor[0].AsInt32());
      cbor.Set(3, 199);
      Assert.AreEqual(199, cbor[3].AsInt32());
    }
    [Test]
    public void TestSign() {
      try {
        int sign = CBORObject.True.Sign;
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        int sign = CBORObject.False.Sign;
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        int sign = CBORObject.NewArray().Sign;
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        int sign = CBORObject.NewMap().Sign;
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              numberinfo["number"].AsString()));
        if (cbornumber.AsNumber().IsNaN()) {
          try {
            Assert.Fail(String.Empty + cbornumber.Sign);
            Assert.Fail("Should have failed");
          } catch (InvalidOperationException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        } else if (numberinfo["number"].AsString().IndexOf('-') == 0) {
          Assert.AreEqual(-1, cbornumber.Sign);
        } else if (numberinfo["number"].AsString().Equals("0",
            StringComparison.Ordinal)) {
          Assert.AreEqual(0, cbornumber.Sign);
        } else {
          Assert.AreEqual(1, cbornumber.Sign);
        }
      }
    }
    [Test]
    public void TestAsNumberSubtract() {
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2).AsNumber().Subtract(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestToJSONString() {
      {
        string stringTemp = ToObjectTest.TestToFromObjectRoundTrip(
            "\u2027\u2028\u2029\u202a\u0008\u000c").ToJSONString();
        Assert.AreEqual(
          "\"\u2027\\u2028\\u2029\u202a\\b\\f\"",
          stringTemp);
      }
      {
        string stringTemp = ToObjectTest.TestToFromObjectRoundTrip(
            "\u0085\ufeff\ufffe\uffff").ToJSONString();
        Assert.AreEqual(
          "\"\\u0085\\uFEFF\\uFFFE\\uFFFF\"",
          stringTemp);
      }
      {
        string stringTemp = CBORObject.True.ToJSONString();
        Assert.AreEqual(
          "true",
          stringTemp);
      }
      {
        string stringTemp = CBORObject.False.ToJSONString();
        Assert.AreEqual(
          "false",
          stringTemp);
      }
      {
        string stringTemp = CBORObject.Null.ToJSONString();
        Assert.AreEqual(
          "null",
          stringTemp);
      }
      {
        string stringTemp =
          ToObjectTest.TestToFromObjectRoundTrip(Single.PositiveInfinity)
          .ToJSONString();
        Assert.AreEqual(
          "null",
          stringTemp);
      }
      {
        string stringTemp =
          ToObjectTest.TestToFromObjectRoundTrip(Single.NegativeInfinity)
          .ToJSONString();
        Assert.AreEqual(
          "null",
          stringTemp);
      }
      {
        string stringTemp =
          ToObjectTest.TestToFromObjectRoundTrip(Single.NaN).ToJSONString();
        Assert.AreEqual(
          "null",
          stringTemp);
      }
      {
        string stringTemp =
          ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity)
          .ToJSONString();
        Assert.AreEqual(
          "null",
          stringTemp);
      }
      {
        string stringTemp =
          ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity)
          .ToJSONString();
        Assert.AreEqual(
          "null",
          stringTemp);
      }
      {
        string stringTemp =
          ToObjectTest.TestToFromObjectRoundTrip(Double.NaN).ToJSONString();
        Assert.AreEqual(
          "null",
          stringTemp);
      }

      CBORObject cbor = CBORObject.NewArray();
      var b64bytes = new byte[] {
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xff, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xff, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
        0x01, 0xfe, 0xdd, 0xfd, 0xdc,
      };
      cbor.Add(b64bytes);
      TestSucceedingJSON(cbor.ToJSONString());
      cbor = CBORObject.NewMap();
      cbor.Add("key", "\ud800\udc00");
      try {
        cbor.ToJSONString();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap();
      try {
        cbor.Add("key", "\ud800");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap();
      try {
        cbor.Add("key", "\udc00");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap();
      try {
        cbor.Add("key", "\ud800\udc00\ud800");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap();
      try {
        cbor.Add("key", "\udc00\udc00\ud800");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap();
      cbor.Add("\ud800\udc00", "value");
      try {
        cbor.ToJSONString();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap();
      try {
        cbor.Add("\ud800", "value");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap();
      try {
        cbor.Add("\udc00", "value");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap();
      try {
        cbor.Add("\ud800\udc00\ud800", "value");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap();
      try {
        cbor.Add("\udc00\udc00\ud800", "value");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewArray();
      cbor.Add("\ud800\udc00");
      try {
        cbor.ToJSONString();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewArray();
      try {
        cbor.Add("\ud800");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewArray();
      try {
        cbor.Add("\udc00");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewArray();
      try {
        cbor.Add("\ud800\udc00\ud800");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewArray();
      try {
        cbor.Add("\udc00\udc00\ud800");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestToJSONString_DuplicateKeys() {
      CBORObject cbor;
      cbor = CBORObject.NewMap().Add("true", 1).Add(true, 1);
      try {
        cbor.ToJSONString();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add("true", 1).Add(false, 1);
      try {
        cbor.ToJSONString();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add("9999-01-01T00:00:00Z", 1)
        .Add(CBORObject.FromObjectAndTag("9999-01-01T00:00:00Z", 0), 1);
      try {
        cbor.ToJSONString();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add("34", 1).Add(34, 1);
      try {
        cbor.ToJSONString();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add("-34", 1).Add(-34, 1);
      try {
        cbor.ToJSONString();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add("-34", 1).Add(-35, 1);
      try {
        cbor.ToJSONString();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestToJSONString_ByteArray_Padding() {
      CBORObject o;
      var options = new JSONOptions(String.Empty);
      o = CBORObject.FromObjectAndTag(
          new byte[] { 0x9a, 0xd6, 0xf0, 0xe8 }, 22);
      {
        string stringTemp = o.ToJSONString(options);
        Assert.AreEqual(
          "\"mtbw6A==\"",
          stringTemp);
      }
      // untagged, so base64url without padding
      o = ToObjectTest.TestToFromObjectRoundTrip(new byte[] {
        0x9a, 0xd6,
        0xf0, 0xe8,
      });
      {
        string stringTemp = o.ToJSONString(options);
        Assert.AreEqual(
          "\"mtbw6A\"",
          stringTemp);
      }
      // tagged 23, so base16
      o = CBORObject.FromObjectAndTag(
          new byte[] { 0x9a, 0xd6, 0xf0, 0xe8 },
          23);
      {
        string stringTemp = o.ToJSONString(options);
        Assert.AreEqual(
          "\"9AD6F0E8\"",
          stringTemp);
      }
      o = ToObjectTest.TestToFromObjectRoundTrip(new byte[] {
        0x9a, 0xd6,
        0xff, 0xe8,
      });
    }

    [Test]
    public void TestToString() {
      {
        string stringTemp = CBORObject.Undefined.ToString();
        Assert.AreEqual(
          "undefined",
          stringTemp);
      }
      CBORObject cbor = CBORObject.True;
      string cborString;
      cborString = cbor.ToString();
      if (cborString == null) {
        Assert.Fail();
      }
      TestCommon.AssertNotEqual("21", cborString);
      TestCommon.AssertNotEqual("simple(21)", cborString);
      cbor = CBORObject.False;
      cborString = cbor.ToString();
      if (cborString == null) {
        Assert.Fail();
      }
      TestCommon.AssertNotEqual("20", cborString);
      TestCommon.AssertNotEqual("simple(20)", cborString);
      cbor = CBORObject.Null;
      cborString = cbor.ToString();
      if (cborString == null) {
        Assert.Fail();
      }
      TestCommon.AssertNotEqual("22", cborString);
      TestCommon.AssertNotEqual("simple(22)", cborString);
      cbor = CBORObject.Undefined;
      cborString = cbor.ToString();
      if (cborString == null) {
        Assert.Fail();
      }
      TestCommon.AssertNotEqual("23", cborString);
      TestCommon.AssertNotEqual("simple(23)", cborString);
      {
        string stringTemp = CBORObject.FromSimpleValue(50).ToString();
        Assert.AreEqual(
          "simple(50)",
          stringTemp);
      }
    }

    [Test]
    public void TestSimpleValuesNotIntegers() {
      CBORObject cbor = CBORObject.True;
      TestCommon.AssertNotEqual(CBORObject.FromObject(21), cbor);
      cbor = CBORObject.False;
      TestCommon.AssertNotEqual(CBORObject.FromObject(20), cbor);
      cbor = CBORObject.Null;
      TestCommon.AssertNotEqual(CBORObject.FromObject(22), cbor);
      cbor = CBORObject.Undefined;
      TestCommon.AssertNotEqual(CBORObject.FromObject(23), cbor);
    }

    [Test]
    public void TestTrue() {
      CBORTestCommon.AssertJSONSer(CBORObject.True, "true");
      Assert.AreEqual(
        CBORObject.True,
        ToObjectTest.TestToFromObjectRoundTrip(true));
    }

    [Test]
    public void TestCalcEncodedBytesSpecific() {
      CBORObject cbor;

      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xda, 0x00, 0x1d,
        (byte)0xdb, 0x03, (byte)0xd9, 0x01, 0x0d, (byte)0x83, 0x00, 0x00,
        0x03,
      });
      CBORTestCommon.CheckEncodeToBytes(cbor);

      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xda, 0x00, 0x14,
        0x57,
        (byte)0xce,
        (byte)0xc5,
        (byte)0x82, 0x1a, 0x46, 0x5a, 0x37,
        (byte)0x87,
        (byte)0xc3, 0x50, 0x5e,
        (byte)0xec,
        (byte)0xfd, 0x73, 0x50, 0x64,
        (byte)0xa1, 0x1f, 0x10,
        (byte)0xc4, (byte)0xff, (byte)0xf2, (byte)0xc4, (byte)0xc9, 0x65,
        0x12,
      });
      CBORTestCommon.CheckEncodeToBytes(cbor);

      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xfa, 0x56, 0x00,
        0x69, 0x2a,
      });
      CBORTestCommon.CheckEncodeToBytes(cbor);
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xf9, (byte)0xfc,
        0x00,
      });
      CBORTestCommon.CheckEncodeToBytes(cbor);
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xa2,
        (byte)0x82,
        (byte)0xf6,
        (byte)0x82,
        (byte)0xfb, 0x3c,
        (byte)0xf0, 0x03, 0x42,
        (byte)0xcb, 0x54, 0x6c,
        (byte)0x85,
        (byte)0x82,
        (byte)0xc5,
        (byte)0x82, 0x18,
        (byte)0xba, 0x0a,
        (byte)0xfa,
        (byte)0x84,
        (byte)0xa0, 0x57,
        (byte)0x97, 0x42, 0x00, 0x01, 0x65, 0x62, 0x7d, 0x45, 0x20, 0x6c, 0x41,
        0x00,
      });
      CBORTestCommon.CheckEncodeToBytes(cbor);
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0x82,
        (byte)0xfa,
        (byte)0xe0,
        (byte)0xa0,
        (byte)0x9d,
        (byte)0xba,
        (byte)0x82,
        (byte)0x82,
        (byte)0xf7, (byte)0xa2, (byte)0xa0, (byte)0xf7, 0x60, 0x41, 0x00,
        (byte)0xf4,
      });
      CBORTestCommon.CheckEncodeToBytes(cbor);
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xfa, (byte)0xc7,
        (byte)0x80, 0x01, (byte)0x80,
      });
      CBORTestCommon.CheckEncodeToBytes(cbor);
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xa5, 0x64, 0x69,
        0x74, 0x65, 0x6d, 0x6a, 0x61, 0x6e, 0x79, 0x20, 0x73, 0x74, 0x72, 0x69,
        0x6e, 0x67, 0x66, 0x6e, 0x75, 0x6d, 0x62, 0x65, 0x72, 0x18, 0x2a, 0x63,
        0x6d, 0x61, 0x70,
        (byte)0xa1, 0x66, 0x6e, 0x75, 0x6d, 0x62, 0x65, 0x72, 0x18, 0x2a, 0x65,
        0x61, 0x72, 0x72, 0x61, 0x79,
        (byte)0x82,
        (byte)0xf9, 0x63,
        (byte)0xce, 0x63, 0x78, 0x79, 0x7a, 0x65, 0x62, 0x79, 0x74, 0x65, 0x73,
        0x43, 0x00, 0x01, 0x02,
      });
      CBORTestCommon.CheckEncodeToBytes(cbor);
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xda, 0x00, 0x14,
        0x57,
        (byte)0xce,
        (byte)0xc5,
        (byte)0x82, 0x1a, 0x46, 0x5a, 0x37,
        (byte)0x87,
        (byte)0xc3, 0x50, 0x5e,
        (byte)0xec,
        (byte)0xfd, 0x73, 0x50, 0x64,
        (byte)0xa1, 0x1f, 0x10,
        (byte)0xc4, (byte)0xff, (byte)0xf2, (byte)0xc4, (byte)0xc9, 0x65,
        0x12,
      });
      CBORTestCommon.CheckEncodeToBytes(cbor);
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xfa, (byte)0xc7,
        (byte)0x80, 0x01, (byte)0x80,
      });
      CBORTestCommon.CheckEncodeToBytes(cbor);
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0x82,
        (byte)0xda, 0x00, 0x0a,
        (byte)0xe8,
        (byte)0xb6,
        (byte)0xfb, 0x43,
        (byte)0xc0, 0x00, 0x00,
        (byte)0xd5, 0x42, 0x7f,
        (byte)0xdc, (byte)0xfa, 0x71, (byte)0x80, (byte)0xd7, (byte)0xc8,
      });
      CBORTestCommon.CheckEncodeToBytes(cbor);
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xfa, 0x29, 0x0a,
        0x4c, (byte)0x9e,
      });
      CBORTestCommon.CheckEncodeToBytes(cbor);
    }

    [Test]
    public void TestType() {
      CBORObject cbor = CBORObject.True;
      Assert.AreEqual(
        CBORType.Boolean,
        cbor.Type);
      // Simple value for true
      cbor = CBORObject.FromSimpleValue(21);
      Assert.AreEqual(
        CBORType.Boolean,
        cbor.Type);
      cbor = CBORObject.FromObjectAndTag(CBORObject.True, 999);
      Assert.AreEqual(
        CBORType.Boolean,
        cbor.Type);
      cbor = CBORObject.False;
      Assert.AreEqual(
        CBORType.Boolean,
        cbor.Type);
      cbor = CBORObject.Null;
      Assert.AreEqual(
        CBORType.SimpleValue,
        cbor.Type);
      cbor = CBORObject.Undefined;
      Assert.AreEqual(
        CBORType.SimpleValue,
        cbor.Type);
      cbor = CBORObject.FromSimpleValue(99);
      Assert.AreEqual(
        CBORType.SimpleValue,
        cbor.Type);
    }
    [Test]
    public void TestUntag() {
      CBORObject o = CBORObject.FromObjectAndTag("test", 999);
      Assert.AreEqual(EInteger.FromString("999"), o.MostInnerTag);
      o = o.Untag();
      Assert.AreEqual(EInteger.FromString("-1"), o.MostInnerTag);
    }
    [Test]
    public void TestUntagOne() {
      // not implemented yet
    }
    [Test]
    public void TestValues() {
      // not implemented yet
    }

    [Test]
    public void TestWrite() {
      for (var i = 0; i < 2000; ++i) {
        this.TestWrite2();
      }
      for (var i = 0; i < 40; ++i) {
        TestWrite3();
      }
    }

    [Test]
    public void TestWriteExtra() {
      try {
        string str = null;
        {
          CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(str);
          CBORObject cborTemp2 =
            ToObjectTest.TestToFromObjectRoundTrip((object)str);
          TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
          try {
            CBORObject.Write(str, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new MemoryStream()) {
            CBORObject.Write(str, ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            AssertReadThree(
              ms.ToArray(),
              ToObjectTest.TestToFromObjectRoundTrip((object)null));
          }
          TestWriteObj((object)str, null);
        }

        {
          CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(
              "test");
          CBORObject cborTemp2 =
            ToObjectTest.TestToFromObjectRoundTrip((object)"test");
          TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
          try {
            CBORObject.Write("test", null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new MemoryStream()) {
            CBORObject.Write("test", ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            {
              CBORObject objectTemp2 = ToObjectTest.TestToFromObjectRoundTrip(
                  "test");
              AssertReadThree(ms.ToArray(), objectTemp2);
            }
          }
          TestWriteObj((object)"test", "test");
        }

        str = TestCommon.Repeat("test", 4000);
        {
          CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(str);
          CBORObject cborTemp2 =
            ToObjectTest.TestToFromObjectRoundTrip((object)str);
          TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
          try {
            CBORObject.Write(str, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new MemoryStream()) {
            CBORObject.Write(str, ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            AssertReadThree(
              ms.ToArray(),
              ToObjectTest.TestToFromObjectRoundTrip(str));
          }
          TestWriteObj((object)str, str);
        }

        long[] values = {
          0, 1, 23, 24, -1, -23, -24, -25,
          0x7f, -128, 255, 256, 0x7fff, -32768, 0x7fff,
          -32768, -65536, -32769, -65537,
          0x7fffff, 0x7fff7f, 0x7fff7fff, 0x7fff7fff7fL, 0x7fff7fff7fffL,
          0x7fff7fff7fff7fL, 0x7fff7fff7fff7fffL,
          Int64.MaxValue, Int64.MinValue, Int32.MinValue,
          Int32.MaxValue,
        };
        for (var i = 0; i < values.Length; ++i) {
          {
            CBORObject cborTemp1 =
              ToObjectTest.TestToFromObjectRoundTrip(values[i]);
            CBORObject cborTemp2 =
              ToObjectTest.TestToFromObjectRoundTrip((object)values[i]);
            TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
            try {
              CBORObject.Write(values[i], null);
              Assert.Fail("Should have failed");
            } catch (ArgumentNullException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            AssertWriteThrow(cborTemp1);
            using (var ms = new MemoryStream()) {
              CBORObject.Write(values[i], ms);
              CBORObject.Write(cborTemp1, ms);
              cborTemp1.WriteTo(ms);
              AssertReadThree(
                ms.ToArray(),
                ToObjectTest.TestToFromObjectRoundTrip(values[i]));
            }
            TestWriteObj((object)values[i], values[i]);
          }

          EInteger bigintVal = EInteger.FromInt64(values[i]);
          {
            CBORObject cborTemp1 =
              ToObjectTest.TestToFromObjectRoundTrip(bigintVal);
            CBORObject cborTemp2 =
              ToObjectTest.TestToFromObjectRoundTrip((object)bigintVal);
            TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
            try {
              CBORObject.Write(bigintVal, null);
              Assert.Fail("Should have failed");
            } catch (ArgumentNullException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            AssertWriteThrow(cborTemp1);
            using (var ms = new MemoryStream()) {
              CBORObject.Write(bigintVal, ms);
              CBORObject.Write(cborTemp1, ms);
              cborTemp1.WriteTo(ms);
              AssertReadThree(
                ms.ToArray(),
                ToObjectTest.TestToFromObjectRoundTrip(bigintVal));
            }
            TestWriteObj((object)bigintVal, bigintVal);
          }

          if (values[i] >= (long)Int32.MinValue && values[i] <=
            (long)Int32.MaxValue) {
            var intval = (int)values[i];
            {
              CBORObject cborTemp1 =
                ToObjectTest.TestToFromObjectRoundTrip(intval);
              CBORObject cborTemp2 =
                ToObjectTest.TestToFromObjectRoundTrip((object)intval);
              TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
              try {
                CBORObject.Write(intval, null);
                Assert.Fail("Should have failed");
              } catch (ArgumentNullException) {
                // NOTE: Intentionally empty
              } catch (Exception ex) {
                Assert.Fail(ex.ToString());
                throw new InvalidOperationException(String.Empty, ex);
              }
              AssertWriteThrow(cborTemp1);
              using (var ms = new MemoryStream()) {
                CBORObject.Write(intval, ms);
                CBORObject.Write(cborTemp1, ms);
                cborTemp1.WriteTo(ms);
                AssertReadThree(
                  ms.ToArray(),
                  ToObjectTest.TestToFromObjectRoundTrip(intval));
              }
              TestWriteObj((object)intval, intval);
            }
          }
          if (values[i] >= -32768L && values[i] <= 32767) {
            var shortval = (short)values[i];
            CBORObject cborTemp1 = ToObjectTest
              .TestToFromObjectRoundTrip(shortval);
            CBORObject cborTemp2 =
              ToObjectTest.TestToFromObjectRoundTrip((object)shortval);
            TestCommon.CompareTestEqualAndConsistent(
              cborTemp1,
              cborTemp2);
            try {
              CBORObject.Write(shortval, null);
              Assert.Fail("Should have failed");
            } catch (ArgumentNullException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            AssertWriteThrow(cborTemp1);
            using (var ms = new MemoryStream()) {
              CBORObject.Write(shortval, ms);
              CBORObject.Write(cborTemp1, ms);
              cborTemp1.WriteTo(ms);
              AssertReadThree(
                ms.ToArray(),
                ToObjectTest.TestToFromObjectRoundTrip(shortval));
            }
            TestWriteObj((object)shortval, shortval);
          }
          if (values[i] >= 0L && values[i] <= 255) {
            var byteval = (byte)values[i];
            {
              CBORObject cborTemp1 =
                ToObjectTest.TestToFromObjectRoundTrip(byteval);
              CBORObject cborTemp2 =
                ToObjectTest.TestToFromObjectRoundTrip((object)byteval);
              TestCommon.CompareTestEqualAndConsistent(cborTemp1,
                cborTemp2);
              try {
                CBORObject.Write(byteval, null);
                Assert.Fail("Should have failed");
              } catch (ArgumentNullException) {
                // NOTE: Intentionally empty
              } catch (Exception ex) {
                Assert.Fail(ex.ToString());
                throw new InvalidOperationException(String.Empty, ex);
              }
              AssertWriteThrow(cborTemp1);
              using (var ms = new MemoryStream()) {
                CBORObject.Write(byteval, ms);
                CBORObject.Write(cborTemp1, ms);
                cborTemp1.WriteTo(ms);
                AssertReadThree(
                  ms.ToArray(),
                  ToObjectTest.TestToFromObjectRoundTrip(byteval));
              }
              TestWriteObj((object)byteval, byteval);
            }
          }
        }
        {
          CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(0.0f);
          CBORObject cborTemp2 =
            ToObjectTest.TestToFromObjectRoundTrip((object)0.0f);
          TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
          try {
            CBORObject.Write(0.0f, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new MemoryStream()) {
            CBORObject.Write(0.0f, ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            AssertReadThree(
              ms.ToArray(),
              ToObjectTest.TestToFromObjectRoundTrip(0.0f));
          }
          TestWriteObj((object)0.0f, 0.0f);
        }

        {
          CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(2.6);
          CBORObject cborTemp2 =
            ToObjectTest.TestToFromObjectRoundTrip((object)2.6);
          TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
          try {
            CBORObject.Write(2.6, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new MemoryStream()) {
            CBORObject.Write(2.6, ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            AssertReadThree(
              ms.ToArray(),
              ToObjectTest.TestToFromObjectRoundTrip(2.6));
          }
          TestWriteObj((object)2.6, 2.6);
        }

        CBORObject cbor = null;
        {
          CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(cbor);
          CBORObject cborTemp2 =
            ToObjectTest.TestToFromObjectRoundTrip((object)cbor);
          TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
          try {
            CBORObject.Write(cbor, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new MemoryStream()) {
            CBORObject.Write(cbor, ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            AssertReadThree(
              ms.ToArray(),
              ToObjectTest.TestToFromObjectRoundTrip((object)null));
          }
          TestWriteObj((object)cbor, null);
        }

        object aobj = null;
        {
          CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(aobj);
          CBORObject cborTemp2 =
            ToObjectTest.TestToFromObjectRoundTrip((object)aobj);
          TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
          try {
            CBORObject.Write(aobj, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new MemoryStream()) {
            CBORObject.Write(aobj, ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            AssertReadThree(
              ms.ToArray(),
              ToObjectTest.TestToFromObjectRoundTrip((object)null));
          }
          TestWriteObj((object)aobj, null);
        }
      } catch (IOException ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(ex.ToString(), ex);
      }
    }

    public static void TestWrite3() {
      EFloat ef = null;
      EDecimal ed = null;
      var fr = new RandomGenerator();
      try {
        for (var i = 0; i < 256; ++i) {
          var b = (byte)(i & 0xff);
          using (var ms = new MemoryStream()) {
            CBORObject.Write((byte)b, ms);
            CBORObject cobj = CBORObject.DecodeFromBytes(ms.ToArray());
            Assert.AreEqual(i, cobj.AsInt32());
          }
        }

        for (var i = 0; i < 50; ++i) {
          ef = RandomObjects.RandomEFloat(fr);
          if (!ef.IsNaN()) {
            CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(ef);
            CBORObject cborTemp2 =
              ToObjectTest.TestToFromObjectRoundTrip((object)ef);
            TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
            try {
              CBORObject.Write(ef, null);
              Assert.Fail("Should have failed");
            } catch (ArgumentNullException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            AssertWriteThrow(cborTemp1);
            using (var ms = new MemoryStream()) {
              CBORObject.Write(ef, ms);
              CBORObject.Write(cborTemp1, ms);
              cborTemp1.WriteTo(ms);
              AssertReadThree(
                ms.ToArray(),
                ToObjectTest.TestToFromObjectRoundTrip(ef));
            }
            TestWriteObj((object)ef, ef);
          }

          ef = EFloat.Create(
              RandomObjects.RandomEInteger(fr),
              RandomObjects.RandomEInteger(fr));
          {
            CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(ef);
            CBORObject cborTemp2 =
              ToObjectTest.TestToFromObjectRoundTrip((object)ef);
            TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
            try {
              CBORObject.Write(ef, null);
              Assert.Fail("Should have failed");
            } catch (ArgumentNullException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            AssertWriteThrow(cborTemp1);
            using (var ms = new MemoryStream()) {
              CBORObject.Write(ef, ms);
              CBORObject.Write(cborTemp1, ms);
              cborTemp1.WriteTo(ms);
              if (cborTemp1.IsNegative && cborTemp1.IsZero) {
                AssertReadThree(ms.ToArray());
              } else {
                AssertReadThree(
                  ms.ToArray(),
                  ToObjectTest.TestToFromObjectRoundTrip(ef));
              }
            }
            TestWriteObj((object)ef, ef);
          }
        }
        for (var i = 0; i < 50; ++i) {
          ed = RandomObjects.RandomEDecimal(fr);
          if (!ed.IsNaN()) {
            CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(ed);
            CBORObject cborTemp2 =
              ToObjectTest.TestToFromObjectRoundTrip((object)ed);
            TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
            try {
              CBORObject.Write(ed, null);
              Assert.Fail("Should have failed");
            } catch (ArgumentNullException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            AssertWriteThrow(cborTemp1);
            using (var ms = new MemoryStream()) {
              CBORObject.Write(ed, ms);
              CBORObject.Write(cborTemp1, ms);
              cborTemp1.WriteTo(ms);
              if (cborTemp1.IsNegative && cborTemp1.IsZero) {
                AssertReadThree(ms.ToArray());
              } else {
                AssertReadThree(
                  ms.ToArray(),
                  ToObjectTest.TestToFromObjectRoundTrip(ed));
              }
            }
            if (!(cborTemp1.IsNegative && cborTemp1.IsZero)) {
              TestWriteObj((object)ed, ed);
            }
          }

          ed = EDecimal.Create(
              RandomObjects.RandomEInteger(fr),
              RandomObjects.RandomEInteger(fr));
          {
            CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(ed);
            CBORObject cborTemp2 =
              ToObjectTest.TestToFromObjectRoundTrip((object)ed);
            TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
            try {
              CBORObject.Write(ed, null);
              Assert.Fail("Should have failed");
            } catch (ArgumentNullException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            AssertWriteThrow(cborTemp1);
            using (var ms = new MemoryStream()) {
              CBORObject.Write(ed, ms);
              CBORObject.Write(cborTemp1, ms);
              cborTemp1.WriteTo(ms);
              AssertReadThree(
                ms.ToArray(),
                ToObjectTest.TestToFromObjectRoundTrip(ed));
            }
            TestWriteObj((object)ed, ed);
          }
        }
      } catch (IOException ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(ex.ToString(), ex);
      }
    }

    [Test]
    public void TestWrite2() {
      try {
        var fr = new RandomGenerator();

        EFloat ef = null;
        {
          CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(ef);
          CBORObject cborTemp2 =
            ToObjectTest.TestToFromObjectRoundTrip((object)ef);
          TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
          try {
            CBORObject.Write(ef, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new MemoryStream()) {
            CBORObject.Write(ef, ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            AssertReadThree(
              ms.ToArray(),
              ToObjectTest.TestToFromObjectRoundTrip((object)null));
          }
          TestWriteObj((object)ef, null);
        }

        ef = EFloat.FromString("20");
        {
          CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(ef);
          CBORObject cborTemp2 =
            ToObjectTest.TestToFromObjectRoundTrip((object)ef);
          TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
          try {
            CBORObject.Write(ef, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new MemoryStream()) {
            CBORObject.Write(ef, ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            AssertReadThree(
              ms.ToArray(),
              ToObjectTest.TestToFromObjectRoundTrip(ef));
          }
          TestWriteObj((object)ef, ef);
        }

        ERational er = null;
        {
          CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(er);
          CBORObject cborTemp2 =
            ToObjectTest.TestToFromObjectRoundTrip((object)er);
          TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
          try {
            CBORObject.Write(er, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new MemoryStream()) {
            CBORObject.Write(er, ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            AssertReadThree(
              ms.ToArray(),
              ToObjectTest.TestToFromObjectRoundTrip((object)null));
          }
          TestWriteObj((object)er, null);
        }
        do {
          er = RandomObjects.RandomERational(fr);
        } while (er.IsNegative && er.IsZero);
        {
          CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(er);
          CBORObject cborTemp2 =
            ToObjectTest.TestToFromObjectRoundTrip((object)er);
          TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
          try {
            CBORObject.Write(er, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new MemoryStream()) {
            CBORObject.Write(er, ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            if (cborTemp1.IsNegative && cborTemp1.IsZero) {
              AssertReadThree(ms.ToArray());
            } else {
              AssertReadThree(
                ms.ToArray(),
                ToObjectTest.TestToFromObjectRoundTrip(er));
            }
          }
          TestWriteObj((object)er, er);
        }

        EDecimal ed = null;
        {
          CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(ed);
          CBORObject cborTemp2 =
            ToObjectTest.TestToFromObjectRoundTrip((object)ed);
          TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
          try {
            CBORObject.Write(ed, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new MemoryStream()) {
            CBORObject.Write(ed, ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            AssertReadThree(
              ms.ToArray(),
              ToObjectTest.TestToFromObjectRoundTrip((object)null));
          }
          TestWriteObj((object)ed, null);
        }

        EInteger bigint = null;
        {
          CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(
              bigint);
          CBORObject cborTemp2 =
            ToObjectTest.TestToFromObjectRoundTrip((object)bigint);
          TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
          try {
            CBORObject.Write(bigint, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new MemoryStream()) {
            CBORObject.Write(bigint, ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            AssertReadThree(
              ms.ToArray(),
              ToObjectTest.TestToFromObjectRoundTrip((object)null));
          }
          TestWriteObj((object)bigint, null);
        }
      } catch (IOException ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(ex.ToString(), ex);
      }
    }
    [Test]
    public void TestWriteJSON() {
      // not implemented yet
      try {
        using (var ms = new MemoryStream()) {
          CBORObject.WriteJSON(CBORObject.True, ms);
          byte[] bytes = ms.ToArray();
          string str = DataUtilities.GetUtf8String(bytes, false);
          Assert.AreEqual("true", str);
        }
        using (var ms = new MemoryStream()) {
          CBORObject.True.WriteJSONTo(ms);
          byte[] bytes = ms.ToArray();
          string str = DataUtilities.GetUtf8String(bytes, false);
          Assert.AreEqual("true", str);
        }
        using (var ms = new MemoryStream()) {
          CBORObject.WriteJSON(CBORObject.False, ms);
          byte[] bytes = ms.ToArray();
          string str = DataUtilities.GetUtf8String(bytes, false);
          Assert.AreEqual("false", str);
        }
        using (var ms = new MemoryStream()) {
          CBORObject.False.WriteJSONTo(ms);
          byte[] bytes = ms.ToArray();
          string str = DataUtilities.GetUtf8String(bytes, false);
          Assert.AreEqual("false", str);
        }
      } catch (IOException ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(ex.ToString(), ex);
      }
    }
    [Test]
    public void TestWriteJSONTo() {
      // not implemented yet
    }
    [Test]
    public void TestWriteTo() {
      // not implemented yet
    }

    [Test]
    public void TestZero() {
      {
        string stringTemp = CBORObject.Zero.ToString();
        Assert.AreEqual(
          "0",
          stringTemp);
      }
      Assert.AreEqual(
        ToObjectTest.TestToFromObjectRoundTrip(0),
        CBORObject.Zero);
    }

    internal static void CompareDecimals(CBORObject o1, CBORObject o2) {
      int cmpDecFrac = TestCommon.CompareTestReciprocal(
          AsED(o1),
          AsED(o2));
      int cmpCobj = TestCommon.CompareTestReciprocal(o1.AsNumber(),
          o2.AsNumber());
      if (cmpDecFrac != cmpCobj) {
        Assert.Fail(TestCommon.ObjectMessages(
            o1,
            o2,
            "Compare: Results\u0020don't match"));
      }
      CBORTestCommon.AssertRoundTrip(o1);
      CBORTestCommon.AssertRoundTrip(o2);
    }

    internal static void AreEqualExact(double a, double b) {
      if (Double.IsNaN(a)) {
        Assert.IsTrue(Double.IsNaN(b));
      } else if (a != b) {
        Assert.Fail("expected " + a + ", got " + b);
      }
    }

    internal static void AreEqualExact(float a, float b) {
      if (Single.IsNaN(a)) {
        Assert.IsTrue(Single.IsNaN(b));
      } else if (a != b) {
        Assert.Fail("expected " + a + ", got " + b);
      }
    }

    private static void AssertReadThree(byte[] bytes) {
      try {
        using (var ms = new MemoryStream(bytes)) {
          CBORObject cbor1, cbor2, cbor3;
          cbor1 = CBORObject.Read(ms);
          cbor2 = CBORObject.Read(ms);
          cbor3 = CBORObject.Read(ms);
          TestCommon.CompareTestRelations(cbor1, cbor2, cbor3);
          TestCommon.CompareTestEqualAndConsistent(cbor1, cbor2);
          TestCommon.CompareTestEqualAndConsistent(cbor2, cbor3);
          TestCommon.CompareTestEqualAndConsistent(cbor3, cbor1);
        }
      } catch (Exception ex) {
        Assert.Fail(ex.ToString() + "\r\n" +
          TestCommon.ToByteArrayString(bytes));
        throw new InvalidOperationException(ex.ToString(), ex);
      }
    }

    private static void AssertReadThree(byte[] bytes, CBORObject cbor) {
      try {
        using (var ms = new MemoryStream(bytes)) {
          CBORObject cbor1, cbor2, cbor3;
          cbor1 = CBORObject.Read(ms);
          cbor2 = CBORObject.Read(ms);
          cbor3 = CBORObject.Read(ms);
          TestCommon.CompareTestEqualAndConsistent(cbor1, cbor);
          TestCommon.CompareTestRelations(cbor1, cbor2, cbor3);
          TestCommon.CompareTestEqualAndConsistent(cbor1, cbor2);
          TestCommon.CompareTestEqualAndConsistent(cbor2, cbor3);
          TestCommon.CompareTestEqualAndConsistent(cbor3, cbor1);
        }
      } catch (Exception ex) {
        Assert.Fail(ex.ToString() + "\r\n" +
          TestCommon.ToByteArrayString(bytes) + "\r\n" +
          "cbor = " + cbor.ToString() + "\r\n");
        throw new InvalidOperationException(ex.ToString(), ex);
      }
    }

    private static void AssertWriteThrow(CBORObject cbor) {
      try {
        cbor.WriteTo(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Write(cbor, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestWriteBigExponentNumber() {
      var exponents = new string[] {
        "15368525994429920286",
        "18446744073709551615",
        "-18446744073709551616",
        "18446744073709551616",
        "-18446744073709551617",
        "18446744073709551615",
        "-18446744073709551614",
      };
      foreach (string strexp in exponents) {
        EInteger bigexp = EInteger.FromString(strexp);
        EDecimal ed = EDecimal.Create(EInteger.FromInt32(99), bigexp);
        TestWriteObj(ed);
        EFloat ef = EFloat.Create(EInteger.FromInt32(99), bigexp);
        TestWriteObj(ef);
        bigexp = bigexp.Negate();
        ed = EDecimal.Create(EInteger.FromInt32(99), bigexp);
        TestWriteObj(ed);
        ef = EFloat.Create(EInteger.FromInt32(99), bigexp);
        TestWriteObj(ef);
      }
    }

    private static void TestWriteObj(object obj) {
      try {
        {
          CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(obj);
          try {
            CBORObject.Write(obj, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new MemoryStream()) {
            CBORObject.Write(obj, ms);
            CBORObject.Write(ToObjectTest.TestToFromObjectRoundTrip(obj), ms);
            ToObjectTest.TestToFromObjectRoundTrip(obj).WriteTo(ms);
            AssertReadThree(ms.ToArray());
          }
        }
      } catch (IOException ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(ex.ToString(), ex);
      }
    }

    [Test]
    public void TestEMap() {
      CBORObject cbor = CBORObject.NewMap()
        .Add("name", "Example");
      byte[] bytes = CBORTestCommon.CheckEncodeToBytes(cbor);
    }

    private static void TestWriteObj(object obj, object objTest) {
      try {
        {
          CBORObject cborTemp1 = ToObjectTest.TestToFromObjectRoundTrip(obj);
          try {
            CBORObject.Write(obj, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new MemoryStream()) {
            CBORObject.Write(obj, ms);
            CBORObject.Write(ToObjectTest.TestToFromObjectRoundTrip(obj), ms);
            ToObjectTest.TestToFromObjectRoundTrip(obj).WriteTo(ms);
            AssertReadThree(
              ms.ToArray(),
              ToObjectTest.TestToFromObjectRoundTrip(objTest));
          }
        }
      } catch (IOException ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(ex.ToString(), ex);
      }
    }

    [Test]
    public void TestWriteValue() {
      try {
        try {
          CBORObject.WriteValue(null, 0, 0);
          Assert.Fail("Should have failed");
        } catch (ArgumentNullException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        try {
          CBORObject.WriteValue(null, 1, 0);
          Assert.Fail("Should have failed");
        } catch (ArgumentNullException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        try {
          CBORObject.WriteValue(null, 2, 0);
          Assert.Fail("Should have failed");
        } catch (ArgumentNullException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        try {
          CBORObject.WriteValue(null, 3, 0);
          Assert.Fail("Should have failed");
        } catch (ArgumentNullException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        try {
          CBORObject.WriteValue(null, 4, 0);
          Assert.Fail("Should have failed");
        } catch (ArgumentNullException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        using (var ms = new MemoryStream()) {
          try {
            CBORObject.WriteValue(ms, -1, 0);
            Assert.Fail("Should have failed");
          } catch (ArgumentException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            CBORObject.WriteValue(ms, 8, 0);
            Assert.Fail("Should have failed");
          } catch (ArgumentException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            CBORObject.WriteValue(ms, 7, 256);
            Assert.Fail("Should have failed");
          } catch (ArgumentException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            CBORObject.WriteValue(ms, 7, Int32.MaxValue);
            Assert.Fail("Should have failed");
          } catch (ArgumentException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            CBORObject.WriteValue(ms, 7, Int64.MaxValue);
            Assert.Fail("Should have failed");
          } catch (ArgumentException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          for (var i = 0; i <= 7; ++i) {
            try {
              CBORObject.WriteValue(ms, i, -1);
              Assert.Fail("Should have failed");
            } catch (ArgumentException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              CBORObject.WriteValue(ms, i, Int32.MinValue);
              Assert.Fail("Should have failed");
            } catch (ArgumentException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              CBORObject.WriteValue(ms, i, -1L);
              Assert.Fail("Should have failed");
            } catch (ArgumentException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              CBORObject.WriteValue(ms, i, Int64.MinValue);
              Assert.Fail("Should have failed");
            } catch (ArgumentException) {
              // NOTE: Intentionally empty
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
          }
          for (var i = 0; i <= 6; ++i) {
            try {
              CBORObject.WriteValue(ms, i, Int32.MaxValue);
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
            try {
              CBORObject.WriteValue(ms, i, Int64.MaxValue);
            } catch (Exception ex) {
              Assert.Fail(ex.ToString());
              throw new InvalidOperationException(String.Empty, ex);
            }
          }
          // Test minimum data length
          int[] ranges = {
            0, 23, 1,
            24, 255, 2,
            256, 266, 3,
            65525, 65535, 3,
            65536, 65546, 5,
          };
          string[] bigRanges = {
            "4294967285", "4294967295",
            "4294967296", "4294967306",
            "18446744073709551604", "18446744073709551615",
          };
          int[] bigSizes = { 5, 9, 9, 5, 9, 9 };
          for (int i = 0; i < ranges.Length; i += 3) {
            for (int j = ranges[i]; j <= ranges[i + 1]; ++j) {
              for (var k = 0; k <= 6; ++k) {
                int count;
                count = CBORObject.WriteValue(ms, k, j);
                Assert.AreEqual(ranges[i + 2], count);
                count = CBORObject.WriteValue(ms, k, (long)j);
                Assert.AreEqual(ranges[i + 2], count);
                count = CBORObject.WriteValue(ms, k, EInteger.FromInt32(j));
                Assert.AreEqual(ranges[i + 2], count);
              }
            }
          }
          for (int i = 0; i < bigRanges.Length; i += 2) {
            EInteger bj = EInteger.FromString(bigRanges[i]);
            EInteger valueBjEnd = EInteger.FromString(bigRanges[i + 1]);
            while (bj < valueBjEnd) {
              for (var k = 0; k <= 6; ++k) {
                int count;
                count = CBORObject.WriteValue(ms, k, bj);
                Assert.AreEqual(bigSizes[i / 2], count);
              }
              bj += EInteger.One;
            }
          }
        }
      } catch (IOException ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(ex.ToString(), ex);
      }
    }

    [Test]
    public void TestWriteFloatingPointValue() {
      var r = new RandomGenerator();
      var bytes = new byte[] { 0, 0, 0 };
      try {
        for (var i = 0; i < 0x10000; ++i) {
          bytes[0] = (byte)0xf9;
          bytes[1] = (byte)((i >> 8) & 0xff);
          bytes[2] = (byte)(i & 0xff);
          CBORObject cbor = CBORObject.DecodeFromBytes(bytes);
          if (!cbor.AsNumber().IsNaN()) {
            using (var ms = new MemoryStream()) {
              CBORObject.WriteFloatingPointValue(
                ms,
                cbor.AsDouble(),
                2);
              TestCommon.AssertByteArraysEqual(bytes, ms.ToArray());
            }
            using (var ms = new MemoryStream()) {
              CBORObject.WriteFloatingPointValue(
                ms,
                cbor.AsSingle(),
                2);
              TestCommon.AssertByteArraysEqual(bytes, ms.ToArray());
            }
          }
        }
        // 32-bit values
        bytes = new byte[5];
        for (var i = 0; i < 100000; ++i) {
          bytes[0] = (byte)0xfa;
          for (var j = 1; j <= 4; ++j) {
            bytes[j] = (byte)r.UniformInt(256);
          }

          CBORObject cbor = CBORObject.DecodeFromBytes(bytes);
          if (!cbor.AsNumber().IsNaN()) {
            using (var ms = new MemoryStream()) {
              CBORObject.WriteFloatingPointValue(
                ms,
                cbor.AsDouble(),
                4);
              TestCommon.AssertByteArraysEqual(bytes, ms.ToArray());
            }
            using (var ms = new MemoryStream()) {
              CBORObject.WriteFloatingPointValue(
                ms,
                cbor.AsSingle(),
                4);
              TestCommon.AssertByteArraysEqual(bytes, ms.ToArray());
            }
          }
        }
        // 64-bit values
        bytes = new byte[9];
        for (var i = 0; i < 100000; ++i) {
          bytes[0] = (byte)0xfb;
          for (var j = 1; j <= 8; ++j) {
            bytes[j] = (byte)r.UniformInt(256);
          }
          CBORObject cbor = CBORObject.DecodeFromBytes(bytes);
          if (!cbor.AsNumber().IsNaN()) {
            using (var ms = new MemoryStream()) {
              CBORObject.WriteFloatingPointValue(
                ms,
                cbor.AsDouble(),
                8);
              TestCommon.AssertByteArraysEqual(bytes, ms.ToArray());
            }
            CBORObject c2 = null;
            byte[] c2bytes = null;
            using (var ms = new MemoryStream()) {
              CBORObject.WriteFloatingPointValue(
                ms,
                cbor.AsSingle(),
                8);
              c2bytes = ms.ToArray();
              c2 = CBORObject.DecodeFromBytes(
                  c2bytes);
            }
            using (var ms = new MemoryStream()) {
              CBORObject.WriteFloatingPointValue(
                ms,
                c2.AsSingle(),
                8);
              TestCommon.AssertByteArraysEqual(c2bytes, ms.ToArray());
            }
            if (i == 0) {
              using (var ms = new MemoryStream()) {
                try {
                  CBORObject.WriteFloatingPointValue(ms, cbor.AsSingle(), 5);
                  Assert.Fail("Should have failed");
                } catch (ArgumentException) {
                  // NOTE: Intentionally empty
                } catch (Exception ex) {
                  Assert.Fail(ex.ToString());
                  throw new InvalidOperationException(String.Empty, ex);
                }
                try {
                  CBORObject.WriteFloatingPointValue(null, cbor.AsSingle(), 4);
                  Assert.Fail("Should have failed");
                } catch (ArgumentNullException) {
                  // NOTE: Intentionally empty
                } catch (Exception ex) {
                  Assert.Fail(ex.ToString());
                  throw new InvalidOperationException(String.Empty, ex);
                }
              }
            }
          }
        }
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    private static string DateTimeToString(
      int year,
      int month,
      int day,
      int hour,
      int minute,
      int second,
      int millisecond) {
      var charbuf = new char[millisecond > 0 ? 24 : 20];
      charbuf[0] = (char)('0' + ((year / 1000) % 10));
      charbuf[1] = (char)('0' + ((year / 100) % 10));
      charbuf[2] = (char)('0' + ((year / 10) % 10));
      charbuf[3] = (char)('0' + (year % 10));
      charbuf[4] = '-';
      charbuf[5] = (char)('0' + ((month / 10) % 10));
      charbuf[6] = (char)('0' + (month % 10));
      charbuf[7] = '-';
      charbuf[8] = (char)('0' + ((day / 10) % 10));
      charbuf[9] = (char)('0' + (day % 10));
      charbuf[10] = 'T';
      charbuf[11] = (char)('0' + ((hour / 10) % 10));
      charbuf[12] = (char)('0' + (hour % 10));
      charbuf[13] = ':';
      charbuf[14] = (char)('0' + ((minute / 10) % 10));
      charbuf[15] = (char)('0' + (minute % 10));
      charbuf[16] = ':';
      charbuf[17] = (char)('0' + ((second / 10) % 10));
      charbuf[18] = (char)('0' + (second % 10));
      if (millisecond > 0) {
        charbuf[19] = '.';
        charbuf[20] = (char)('0' + ((millisecond / 100) % 10));
        charbuf[21] = (char)('0' + ((millisecond / 10) % 10));
        charbuf[22] = (char)('0' + (millisecond % 10));
        charbuf[23] = 'Z';
      } else {
        charbuf[19] = 'Z';
      }
      return new String(charbuf);
    }

    [Test]
    public void TestDateTime() {
      var dateList = new List<string>();
      dateList.Add("0783-08-19T03:10:29.406Z");
      dateList.Add("1954-03-07T16:20:38.256Z");
      var rng = new RandomGenerator();
      for (var i = 0; i < 2000; ++i) {
        string dtstr = DateTimeToString(
            rng.UniformInt(9999) + 1,
            rng.UniformInt(12) + 1,
            rng.UniformInt(28) + 1,
            rng.UniformInt(24),
            rng.UniformInt(60),
            rng.UniformInt(60),
            rng.UniformInt(1000));
        dateList.Add(dtstr);
      }
      foreach (string dtstr in dateList) {
        CBORObject cbor = CBORObject.FromObjectAndTag(dtstr, 0);
        var dt = (DateTime)cbor.ToObject(typeof(DateTime));
        ToObjectTest.TestToFromObjectRoundTrip(dt);
      }
    }

    private static CBORObject FromJSON(string json, JSONOptions jsonop) {
      // var sw = new System.Diagnostics.Stopwatch();
      // sw.Start();
      CBORObject cbor = CBORObject.FromJSONString(json, jsonop);
      // sw.Stop();
      // Console.WriteLine(String.Empty + sw.ElapsedMilliseconds + " ms");
      return cbor;
    }

    private static CBORObject FromJSON(string json, string numconv) {
      return FromJSON(json, new JSONOptions("numberconversion=" + numconv));
    }

    private static void AssertJSONDouble(
      string json,
      string numconv,
      double dbl) {
      CBORObject cbor = FromJSON(json, numconv);
      Assert.AreEqual(CBORType.FloatingPoint, cbor.Type);
      Assert.AreEqual(dbl, cbor.AsDoubleValue());
    }

    private static void AssertJSONInteger(
      string json,
      string numconv,
      int intval) {
      CBORObject cbor = FromJSON(json, numconv);
      Assert.AreEqual(CBORType.Integer, cbor.Type);
      Assert.AreEqual(intval, cbor.AsInt32Value());
    }

    [Test]
    [Timeout(5000)]
    public void TestFromJsonStringFastCases() {
      AssertJSONDouble(
        "0e-" + TestCommon.Repeat("0", 1000000),
        "double",
        0.0);
      AssertJSONDouble(
        "0." + TestCommon.Repeat("0", 1000000),
        "double",
        0.0);
      AssertJSONDouble(
        "0." + TestCommon.Repeat("0", 1000000) + "e-9999999999999",
        "double",
        0.0);
      AssertJSONDouble(
        TestCommon.Repeat("3", 1000000) + "e-9999999999999",
        "double",
        0.0);
      AssertJSONDouble(
        TestCommon.Repeat("3", 1000000) + "e-9999999999999",
        "intorfloat",
        0.0);
      AssertJSONInteger(
        TestCommon.Repeat("3", 1000000) + "e-9999999999999",
        "intorfloatfromdouble",
        0);
      AssertJSONDouble(
        "0." + TestCommon.Repeat("0", 1000000) + "e-99999999",
        "double",
        0.0);
      AssertJSONDouble(
        TestCommon.Repeat("3", 1000000) + "e-99999999",
        "double",
        0.0);
      AssertJSONDouble(
        TestCommon.Repeat("3", 1000000) + "e-99999999",
        "intorfloat",
        0.0);
      AssertJSONInteger(
        TestCommon.Repeat("3", 1000000) + "e-99999999",
        "intorfloatfromdouble",
        0);
      AssertJSONInteger(
        "0e-" + TestCommon.Repeat("0", 1000000),
        "intorfloat",
        0);
      AssertJSONInteger(
        "0e-" + TestCommon.Repeat("0", 1000000),
        "intorfloatfromdouble",
        0);
      AssertJSONInteger(
        "-0e-" + TestCommon.Repeat("0", 1000000),
        "intorfloat",
        0);
      AssertJSONInteger(
        "-0e-" + TestCommon.Repeat("0", 1000000),
        "intorfloatfromdouble",
        0);
      AssertJSONInteger(
        "0." + TestCommon.Repeat("0", 1000000),
        "intorfloat",
        0);
      AssertJSONInteger(
        "0." + TestCommon.Repeat("0", 1000000),
        "intorfloatfromdouble",
        0);
      AssertJSONInteger(
        "-0." + TestCommon.Repeat("0", 1000000),
        "intorfloat",
        0);
      AssertJSONInteger(
        "-0." + TestCommon.Repeat("0", 1000000),
        "intorfloatfromdouble",
        0);
    }

    [Test]
    [Timeout(10000)]
    public void TestFromJsonStringLongKindFull() {
      var jsonop = new JSONOptions("numberconversion=full");
      string json = TestCommon.Repeat("7", 100000);
      CBORObject cbor = FromJSON(json, jsonop);
      Assert.IsTrue(cbor.IsTagged);
    }

    [Test]
    [Timeout(10000)]
    public void TestFromJsonStringLongKindFull2() {
      var jsonop = new JSONOptions("numberconversion=full");
      string json = TestCommon.Repeat("7", 100000) + ".0";
      CBORObject cbor = FromJSON(json, jsonop);
      Assert.IsTrue(cbor.IsTagged);
    }

    [Test]
    [Timeout(2000)]
    public void TestFromJsonStringLongKindFullBad() {
      Console.WriteLine("FullBad 1");
      var jsonop = new JSONOptions("numberconversion=full");
      string[] badjson = {
        TestCommon.Repeat("7", 1000000) + "x",
        "7x" + TestCommon.Repeat("7", 1000000),
        TestCommon.Repeat("7", 1000000) + "e0x",
        "-" + TestCommon.Repeat("7", 1000000) + "x",
        "-7x" + TestCommon.Repeat("7", 1000000),
        "-" + TestCommon.Repeat("7", 1000000) + "e0x",
      };
      foreach (string str in badjson) {
      try {
        FromJSON(str, jsonop);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      }
      Console.WriteLine("FullBad 2");
      string json = TestCommon.Repeat("0", 1000000);
      try {
        FromJSON(json, jsonop);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    [Timeout(2000)]
    public void TestFromJsonStringLongKindsBad() {
      JSONOptions jsonop;
      string json = TestCommon.Repeat("7", 1000000) + "x";
      jsonop = new JSONOptions("numberconversion=double");
      try {
        FromJSON(json, jsonop);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      jsonop = new JSONOptions("numberconversion=intorfloatfromdouble");
      try {
        FromJSON(json, jsonop);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      jsonop = new JSONOptions("numberconversion=intorfloat");
      try {
        FromJSON(json, jsonop);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      json = TestCommon.Repeat("0", 1000000);
      jsonop = new JSONOptions("numberconversion=double");
      try {
        FromJSON(json, jsonop);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      jsonop = new JSONOptions("numberconversion=intorfloatfromdouble");
      try {
        FromJSON(json, jsonop);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      jsonop = new JSONOptions("numberconversion=intorfloat");
      try {
        FromJSON(json, jsonop);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    [Timeout(2000)]
    public void TestFromJsonStringLongKindIntOrFloatFromDouble() {
      var jsonop = new JSONOptions("numberconversion=intorfloatfromdouble");
      string json = TestCommon.Repeat("7", 1000000);
      CBORObject cbor = FromJSON(json, jsonop);
      Assert.AreEqual(CBORType.FloatingPoint, cbor.Type);
      Assert.AreEqual(Double.PositiveInfinity, cbor.AsDoubleValue());
      json = TestCommon.Repeat("7", 1000000) + "e+0";
      cbor = FromJSON(json, jsonop);
      Assert.AreEqual(CBORType.FloatingPoint, cbor.Type);
      Assert.AreEqual(Double.PositiveInfinity, cbor.AsDoubleValue());
      json = TestCommon.Repeat("7", 1000000) + "e0";
      cbor = FromJSON(json, jsonop);
      Assert.AreEqual(CBORType.FloatingPoint, cbor.Type);
      Assert.AreEqual(Double.PositiveInfinity, cbor.AsDoubleValue());
    }

    [Test]
    [Timeout(2000)]
    public void TestFromJsonStringLongKindIntOrFloat() {
      var jsonop = new JSONOptions("numberconversion=intorfloat");
      string json = TestCommon.Repeat("7", 1000000);
      CBORObject cbor = FromJSON(json, jsonop);
      Assert.AreEqual(CBORType.FloatingPoint, cbor.Type);
      Assert.AreEqual(Double.PositiveInfinity, cbor.AsDoubleValue());
    }

    [Test]
    [Timeout(2000)]
    public void TestFromJsonStringLongKindIntOrFloat2() {
      var jsonop = new JSONOptions("numberconversion=intorfloat");
      string json = "-" + TestCommon.Repeat("7", 1000000);
      CBORObject cbor = FromJSON(json, jsonop);
      Assert.AreEqual(CBORType.FloatingPoint, cbor.Type);
      Assert.AreEqual(Double.NegativeInfinity, cbor.AsDoubleValue());
    }

    [Test]
    public void TestToObject_TypeMapper() {
      var mapper = new CBORTypeMapper()
      .AddConverter(typeof(string), new TestConverter());
      CBORObject cbor = CBORObject.FromObject("UpPeR");
      {
        var stringTemp = (string)cbor.ToObject(typeof(string), mapper);
        Assert.AreEqual(
          "upper",
          stringTemp);
      }
      cbor = CBORObject.FromObject("TRUE");
      {
        var stringTemp = (string)cbor.ToObject(typeof(string), mapper);
        Assert.AreEqual(
          "true",
          stringTemp);
      }
      cbor = CBORObject.FromObject("false");
      {
        var stringTemp = (string)cbor.ToObject(typeof(string), mapper);
        Assert.AreEqual(
          "false",
          stringTemp);
      }
      cbor = CBORObject.FromObject("FALSE");
      {
        var stringTemp = (string)cbor.ToObject(typeof(string), mapper);
        Assert.AreEqual(
          "false",
          stringTemp);
      }
    }
  }
}
