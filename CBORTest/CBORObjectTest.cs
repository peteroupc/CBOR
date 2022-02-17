using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
      "trub", "falsb", "nulb", "[true", "[true,", "[true]!", "tr\u0020",
      "tr", "fa", "nu", "True", "False", "Null", "TRUE", "FALSE", "NULL",
      "truE", "falsE", "nulL", "tRUE", "fALSE", "nULL", "tRuE", "fAlSe", "nUlL",
      "[tr]", "[fa]",
      "[nu]", "[True]", "[False]", "[Null]", "[TRUE]", "[FALSE]", "[NULL]",
      "[truE]", "[falsE]",
      "[nulL]", "[tRUE]", "[fALSE]", "[nULL]", "[tRuE]", "[fAlSe]", "[nUlL]",
      "fa ", "nu ", "fa lse", "nu ll", "tr ue",
      "[\"\ud800\\udc00\"]", "[\"\\ud800\udc00\"]",
      "[\"\\udc00\ud800\udc00\"]", "[\"\\ud800\ud800\udc00\"]",
      "[\"\\ud800\"]", "[1,2,", "[1,2,3", "{,\"0\":0,\"1\":1}",
      "{\"0\"::0}", "{\"0\":0,,\"1\":1}",
      "{\"0\":0,\"1\":1,}", "[,0,1,2]", "[0,,1,2]", "[0:1]", "[0:1:2]",
      "[0,1,,2]", "[0,1,2,]", "[0001]", "{a:true}",
      "{\"a\":#comment\ntrue}",
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
      "00", "000", "001", "0001", "00.0", "001.0", "0001.0", "01E-4", "01.1E-4",
      "01E4", "01.1E4", "01e-4", "01.1e-4",
      "01e4", "01.1e4",
      "+0", "+1", "+0.0", "+1e4", "+1e-4", "+1.0", "+1.0e4",
      "+1.0e+4", "+1.0e-4",
      "0000", "0x1", "0xf", "0x20", "0x01",
      "-3x", "-3e89x", "\u0005true", "x\\u0005z",
      "0,2", "0,05", "-0,2", "-0,05", "\u007F0.0", "\u00010.0", "0.0\u007F",
      "0.0\u0001", "-1.D\r\n", "-1.D\u0020", "-1.5L", "-0.0L", "0L", "1L",
      "1.5L",
      "0.0L",
      "0X1", "0Xf", "0X20", "0X01", ".2", ".05", "-.2",
      "-.05", "23.", "23.e0", "23.e1", "0.", "-0.", "[0000]", "[0x1]",
      "[0xf]", "[0x20]", "[0x01]", "[.2]", "[.05]", "[-.2]", "[-.05]",
      "[23.]", "[23.e0]", "[23.e1]", "[0.]", "\"abc", "\"ab\u0004c\"",
      "\u0004\"abc\"",
      "{\"x\":true \"y\":true}",
      "{\"x\":true\n\"y\":true}",
      "0,1,2,3", "\"x\",true",
      "\"x\",true",
      "\"x\":true",
      "\"x\":true,\"y\":true",
      "\"x\":true\n\"y\":true",
      "\"x\":true \"y\":true",
      "{\"x\":true,\"y\"}",
      "{\"x\",\"y\":true}",
      "{\"x\":true, \"y\"}",
      "{\"x\", \"y\":true}",
      "{[\"x\"]:true}",
      "{null:true}", "{true:true}", "{false:true}",
      "{[0]:true}", "{1:true}", "{{\"a\":true}:true}",
      "[1,\u0004" + "2]",
    };

    private static readonly string[] ValueJsonSucceeds = {
      "[0]",
      "[0.1]",
      "[0.1001]",
      "[0.0]", "true\n\r\t\u0020",
      "[-3 " + ",-5]", "\n\r\t\u0020true", "\"x\\u0005z\"",
      "[0.00]", "[0.000]", "[0.01]", "[0.001]", "[0.5]", "[0E5]", "[0e5]",
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
          Assert.Fail("Should have failed: str = " + str);
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        return;
      }
      using (var ms = new Test.DelayingStream(bytes)) {
        try {
          CBORObject.ReadJSON(ms, opt);
          Assert.Fail("Should have failed: str = " + str);
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(str + "\r\n" + ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      try {
        CBORObject.FromJSONString(str, opt);
        Assert.Fail("Should have failed: str = " + str);
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
        using (var ms = new Test.DelayingStream(bytes)) {
          CBORObject obj = options == null ? CBORObject.ReadJSON(ms) :
            CBORObject.ReadJSON(ms, options);
          CBORObject obj2 = options == null ? CBORObject.FromJSONString(str) :
            CBORObject.FromJSONString(str, options);
          if (!obj.Equals(obj2)) {
            TestCommon.CompareTestEqualAndConsistent(
              obj,
              obj2);
          }
          if (str == null) {
            throw new ArgumentNullException(nameof(str));
          }
          CBORObject obj3 = options == null ? CBORObject.FromJSONString(
              str,
              0,
              str.Length) :
            CBORObject.FromJSONString(str, 0, str.Length, options);
          if (!obj.Equals(obj3)) {
            Assert.AreEqual(obj, obj3);
          }
          obj3 = options == null ? CBORObject.FromJSONString(
              "xyzxyz" + str,
              6,
              str.Length) :
            CBORObject.FromJSONString("xyzxyz" + str, 6, str.Length, options);
          if (!obj.Equals(obj3)) {
            Assert.AreEqual(obj, obj3);
          }
          obj3 = options == null ? CBORObject.FromJSONString(
              "xyzxyz" + str + "xyzxyz",
              6,
              str.Length) : CBORObject.FromJSONString(
              "xyzxyz" + str + "xyzxyz",
              6,
              str.Length,
              options);
          if (!obj.Equals(obj3)) {
            Assert.AreEqual(obj, obj3);
          }
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
    [Timeout(5000)]
    public void TestAsNumberAdd() {
      var r = new RandomGenerator();
      for (var i = 0; i < 1000; ++i) {
        // NOTE: Avoid generating high-exponent numbers for this test
        CBORObject o1 = CBORTestCommon.RandomNumber(r, true);
        CBORObject o2 = CBORTestCommon.RandomNumber(r, true);
        EDecimal cmpCobj = null;
        try {
          cmpCobj = o1.AsNumber().Add(o2.AsNumber()).ToEDecimal();
        } catch (OutOfMemoryException) {
          continue;
        }
        EDecimal cmpDecFrac = AsED(o1).Add(AsED(o2));
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
    }
    [Test]
    public void TestCanFitInDoubleA() {
      Assert.IsFalse(CBORObject.True.CanFitInDouble());
    }
    [Test]
    public void TestCanFitInDoubleB() {
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .CanFitInDouble());
    }
    [Test]
    public void TestCanFitInDoubleC() {
      Assert.IsFalse(CBORObject.NewArray().CanFitInDouble());
      Assert.IsFalse(CBORObject.NewMap().CanFitInDouble());
    }
    [Test]
    public void TestCanFitInDoubleD() {
      Assert.IsFalse(CBORObject.False.CanFitInDouble());
      Assert.IsFalse(CBORObject.Null.CanFitInDouble());
    }
    [Test]
    public void TestCanFitInDoubleE() {
      Assert.IsFalse(CBORObject.Undefined.CanFitInDouble());
    }
    [Test]
    public void TestCanFitInDoubleF() {
      CBORObject numbers = GetNumberData();
      for (int i = 0; i < numbers.Count; ++i) {
        CBORObject numberinfo = numbers[i];
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              numberinfo["number"].AsString()));
        if (cbornumber == null) {
          Assert.Fail();
        }
        if (numberinfo["double"].AsBoolean()) {
          if (!cbornumber.CanFitInDouble()) {
            Assert.Fail(cbornumber.ToString());
          }
        } else {
          if (cbornumber.CanFitInDouble()) {
            Assert.Fail(cbornumber.ToString());
          }
        }
      }
      var rand = new RandomGenerator();
      for (var i = 0; i < 2047; ++i) {
        // Try a random double with a given
        // exponent
        object o = RandomObjects.RandomDouble(rand, i);
        CBORObject cbornumber = ToObjectTest.TestToFromObjectRoundTrip(o);
        if (cbornumber == null) {
          Assert.Fail();
        }
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
        if (numberinfo["number"] == null) {
          Assert.Fail();
        }
        if (numberinfo["int32"] == null) {
          Assert.Fail();
        }
        if (numberinfo["isintegral"] == null) {
          Assert.Fail();
        }
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              numberinfo["number"].AsString()));
        if (cbornumber == null) {
          Assert.Fail();
        }
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
      return cbor != null && cbor.IsNumber && cbor.AsNumber().CanFitInInt64();
    }

    private static bool CInt32(CBORObject cbor) {
      return cbor != null && cbor.IsNumber && cbor.AsNumber().CanFitInInt32();
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
        if (numberinfo["number"] == null) {
          Assert.Fail();
        }
        if (numberinfo["int64"] == null) {
          Assert.Fail();
        }
        if (numberinfo["isintegral"] == null) {
          Assert.Fail();
        }
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
        if (numberinfo["number"] == null) {
          Assert.Fail();
        }
        if (numberinfo["single"] == null) {
          Assert.Fail();
        }
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
        if (numberinfo["number"] == null) {
          Assert.Fail();
        }
        if (numberinfo["int32"] == null) {
          Assert.Fail();
        }
        string numberString = numberinfo["number"].AsString();
        CBORObject cbornumber = ToObjectTest.TestToFromObjectRoundTrip(
            EDecimal.FromString(numberString));
        if (numberinfo["int32"].AsBoolean()) {
          Assert.IsTrue(cbornumber.CanTruncatedIntFitInInt32(), numberString);
        } else {
          Assert.IsFalse(cbornumber.CanTruncatedIntFitInInt32(), numberString);
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
      TestCommon.CompareTestLess(dn, dnan);
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
        string opts = "allowduplicatekeys=1;useindeflengthstrings=1";
        CBORObject.DecodeFromBytes(bytes,
          new CBOREncodeOptions(opts));
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
      using (var ms = new Test.DelayingStream(bytes)) {
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
      using (var ms = new Test.DelayingStream(bytes)) {
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
      using (var ms = new Test.DelayingStream(bytes)) {
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
      using (var ms = new Test.DelayingStream(bytes)) {
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
      using (var ms = new Test.DelayingStream(bytes)) {
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
      using (var ms = new Test.DelayingStream(bytes)) {
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
      using (var ms = new Test.DelayingStream(bytes)) {
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
    public void TestEncodeFloat64() {
      try {
        var rg = new RandomGenerator();
        var options = new CBOREncodeOptions("float64=true");
        for (var i = 0; i < 10000; ++i) {
          double dbl = 0.0;
          dbl = (i == 0) ? Double.PositiveInfinity : ((i == 1) ?
              Double.NegativeInfinity : RandomObjects.RandomDouble(rg));
          CBORObject cbor = CBORObject.FromObject(dbl);
          byte[] bytes = cbor.EncodeToBytes(options);
          Assert.AreEqual(9, bytes.Length);
          TestCommon.AssertEqualsHashCode(
            cbor,
            CBORObject.DecodeFromBytes(bytes));
          using (var ms = new Test.DelayingStream()) {
            cbor.WriteTo(ms, options);
            bytes = ms.ToArray();
            Assert.AreEqual(9, bytes.Length);
          }
          using (var ms = new Test.DelayingStream()) {
            CBORObject.Write(dbl, ms, options);
            bytes = ms.ToArray();
            Assert.AreEqual(9, bytes.Length);
          }
          using (var ms = new Test.DelayingStream()) {
            CBORObject.Write(cbor, ms, options);
            bytes = ms.ToArray();
            Assert.AreEqual(9, bytes.Length);
          }
          CBORObject cbor2 = CBORObject.NewArray().Add(cbor);
          bytes = cbor2.EncodeToBytes(options);
          TestCommon.AssertEqualsHashCode(
            cbor2,
            CBORObject.DecodeFromBytes(bytes));
          Assert.AreEqual(10, bytes.Length);
          using (var ms = new Test.DelayingStream()) {
            cbor2.WriteTo(ms, options);
            bytes = ms.ToArray();
            Assert.AreEqual(10, bytes.Length);
          }
          using (var ms = new Test.DelayingStream()) {
            CBORObject.Write(cbor2, ms, options);
            bytes = ms.ToArray();
            Assert.AreEqual(10, bytes.Length);
          }
          cbor2 = cbor.WithTag(1);
          bytes = cbor2.EncodeToBytes(options);
          Assert.AreEqual(10, bytes.Length);
          TestCommon.AssertEqualsHashCode(
            cbor2,
            CBORObject.DecodeFromBytes(bytes));
          using (var ms = new Test.DelayingStream()) {
            cbor2.WriteTo(ms, options);
            bytes = ms.ToArray();
            Assert.AreEqual(10, bytes.Length);
          }
          using (var ms = new Test.DelayingStream()) {
            CBORObject.Write(cbor2, ms, options);
            bytes = ms.ToArray();
            Assert.AreEqual(10, bytes.Length);
          }
        }
      } catch (IOException ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    public static readonly int[] EtbRanges = {
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

    [Test]
    public void TestEncodeToBytes() {
      // Test minimum data length
      int[] ranges = EtbRanges;
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
    [Timeout(100000)]
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
      aba = "{\"a\" :1}";
      cbor = TestSucceedingJSON(aba);
      Assert.AreEqual(ToObjectTest.TestToFromObjectRoundTrip(1), cbor["a"]);
      aba = "{\"a\" : 1}";
      cbor = TestSucceedingJSON(aba);
      Assert.AreEqual(ToObjectTest.TestToFromObjectRoundTrip(1), cbor["a"]);
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

    private static void CheckKeyValue(CBORObject o, string key, object value) {
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
      CheckKeyValue(c, "TestKey", "TestValue");
      CheckKeyValue(c, "TestKey2", "TestValue2");
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
    public void TestWithTag() {
      EInteger bigvalue = EInteger.FromString("99999999999999999999999999999");
      try {
        CBORObject.FromObject(2).WithTag(bigvalue);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2).WithTag(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(CBORObject.Null).WithTag(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(CBORObject.Null).WithTag(999999);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      EInteger eintNull = null;
      try {
        CBORObject.FromObject(2).WithTag(eintNull);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(2).WithTag(EInteger.FromString("-1"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
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

    private static void Sink(object obj) {
      Console.WriteLine("Sink for " + obj);
      Assert.Fail();
    }

    [Test]
    public void TestKeys() {
      CBORObject co;
      try {
        co = CBORObject.True;
        Sink(co.Keys);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Sink(ToObjectTest.TestToFromObjectRoundTrip(0).Keys);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Sink(ToObjectTest.TestToFromObjectRoundTrip("string").Keys);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Sink(CBORObject.NewArray().Keys);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Sink(ToObjectTest.TestToFromObjectRoundTrip(
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
    [Timeout(200000)]
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
        using (var ms2 = new Test.DelayingStream(new byte[] { 0 })) {
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

    public static void ExpectJsonSequenceError(byte[] bytes) {
      using (var ms = new Test.DelayingStream(bytes)) {
        try {
          CBORObject.ReadJSONSequence(ms);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    }

    public static void ExpectJsonSequenceZero(byte[] bytes) {
      try {
        using (var ms = new Test.DelayingStream(bytes)) {
          string ss = TestCommon.ToByteArrayString(bytes);
          CBORObject[] array = CBORObject.ReadJSONSequence(ms);
          Assert.AreEqual(0, array.Length, ss);
        }
      } catch (IOException ioe) {
        throw new InvalidOperationException(ioe.Message, ioe);
      }
    }

    public static void ExpectJsonSequenceOne(byte[] bytes, CBORObject o1) {
      try {
        using (var ms = new Test.DelayingStream(bytes)) {
          string ss = TestCommon.ToByteArrayString(bytes);
          CBORObject[] array = CBORObject.ReadJSONSequence(ms);
          Assert.AreEqual(1, array.Length, ss);
          Assert.AreEqual(o1, array[0], ss);
        }
      } catch (IOException ioe) {
        throw new InvalidOperationException(ioe.Message, ioe);
      }
    }

    public static void ExpectJsonSequenceTwo(
      byte[] bytes,
      CBORObject o1,
      CBORObject o2) {
      try {
        using (var ms = new Test.DelayingStream(bytes)) {
          string ss = TestCommon.ToByteArrayString(bytes);
          CBORObject[] array = CBORObject.ReadJSONSequence(ms);
          Assert.AreEqual(2, array.Length, ss);
          Assert.AreEqual(o1, array[0], ss);
          Assert.AreEqual(o2, array[1], ss);
        }
      } catch (IOException ioe) {
        throw new InvalidOperationException(ioe.Message, ioe);
      }
    }

    [Test]
    public void TestJsonSequence() {
      byte[] bytes;
      bytes = new byte[] { };
      ExpectJsonSequenceZero(bytes);
      bytes = new byte[] { 0x1e, 0x22, 0x41, 0x22, 0x0a };
      ExpectJsonSequenceOne(bytes, CBORObject.FromObject("A"));
      bytes = new byte[] { 0x1e, 0x22, 0x41, 0x22, 0x20 };
      ExpectJsonSequenceOne(bytes, CBORObject.FromObject("A"));
      bytes = new byte[] { 0x1e, 0x22, 0x41, 0x22, 0x09 };
      ExpectJsonSequenceOne(bytes, CBORObject.FromObject("A"));
      bytes = new byte[] { 0x1e, 0x22, 0x41, 0x22, 0x0d };
      ExpectJsonSequenceOne(bytes, CBORObject.FromObject("A"));
      bytes = new byte[] {
        0x1e, (byte)0x66, (byte)0x61, (byte)0x6c, (byte)0x73,
        (byte)0x65, 0x0a,
      };
      ExpectJsonSequenceOne(bytes, CBORObject.False);
      bytes = new byte[] {
        0x1e, (byte)0x66, (byte)0x61, (byte)0x6c, (byte)0x73,
        (byte)0x65,
      };
      ExpectJsonSequenceOne(bytes, null);
      bytes = new byte[] {
        0x1e, (byte)0x66, (byte)0x61, (byte)0x6c, (byte)0x73,
        (byte)0x65, (byte)0x74, (byte)0x72, (byte)0x75, (byte)0x65, 0x0a,
      };
      ExpectJsonSequenceOne(bytes, null);
      bytes = new byte[] {
        0x1e, (byte)0x74, (byte)0x72, (byte)0x75, (byte)0x65,
        0x0a,
      };
      ExpectJsonSequenceOne(bytes, CBORObject.True);
      bytes = new byte[] {
        0x1e, (byte)0x74, (byte)0x72, (byte)0x75,
        (byte)0x65,
      };
      ExpectJsonSequenceOne(bytes, null);
      bytes = new byte[] {
        0x1e, (byte)0x6e, (byte)0x75, (byte)0x6c, (byte)0x6c,
        0x0a,
      };
      ExpectJsonSequenceOne(bytes, CBORObject.Null);
      bytes = new byte[] {
        0x1e, (byte)0x6e, (byte)0x75, (byte)0x6c,
        (byte)0x6c,
      };
      ExpectJsonSequenceOne(bytes, null);
      bytes = new byte[] { 0x1e, 0x22, 0x41, 0x22, 0x1e, (byte)'[', (byte)']' };
      ExpectJsonSequenceTwo(
        bytes,
        CBORObject.FromObject("A"),
        CBORObject.NewArray());
      bytes = new byte[] {
        0x1e, 0x22, 0x41, 0x22, 0x0a, 0x1e, (byte)'[',
        (byte)']',
      };
      ExpectJsonSequenceTwo(
        bytes,
        CBORObject.FromObject("A"),
        CBORObject.NewArray());
      bytes = new byte[] {
        0x1e, 0x22, 0x41, 0x22, 0x41, 0x1e, (byte)'[',
        (byte)']',
      };
      ExpectJsonSequenceTwo(bytes, null, CBORObject.NewArray());
      bytes = new byte[] { 0x1e, 0x1e, 0x22, 0x41, 0x22, 0x0a };
      ExpectJsonSequenceOne(bytes, CBORObject.FromObject("A"));
      bytes = new byte[] { 0x1e, 0x1e, 0x30, 0x0a };
      ExpectJsonSequenceOne(bytes, CBORObject.FromObject(0));
      bytes = new byte[] { 0x1e, 0x1e, 0xef, 0xbb, 0xbf, 0x30, 0x0a };
      ExpectJsonSequenceOne(bytes, null);
      bytes = new byte[] {
        0x1e, 0x1e, 0xef, 0xbb, 0xbf, 0x30, 0x0a, 0x1e, 0x30,
        0x0a,
      };
      ExpectJsonSequenceTwo(bytes, null, CBORObject.FromObject(0));
      bytes = new byte[] { 0x22, 0x41, 0x22, 0x0a };
      ExpectJsonSequenceError(bytes);
      bytes = new byte[] { 0xef, 0xbb, 0xbf, 0x1e, 0x30, 0x0a };
      ExpectJsonSequenceError(bytes);
      bytes = new byte[] { 0xfe, 0xff, 0x00, 0x1e, 0, 0x30, 0, 0x0a };
      ExpectJsonSequenceError(bytes);
      bytes = new byte[] { 0xff, 0xfe, 0x1e, 0, 0x30, 0, 0x0a, 0 };
      ExpectJsonSequenceError(bytes);
      bytes = new byte[] { 0x1e, 0x22, 0x41, 0x22, 0x0a, 0x31, 0x31 };
      ExpectJsonSequenceOne(bytes, null);
      bytes = new byte[] { 0x1e, 0x22, 0x41, 0x22, 0x0a, 0x1e };
      ExpectJsonSequenceTwo(bytes, CBORObject.FromObject("A"), null);
      bytes = new byte[] { 0x1e, 0x22, 0x41, 0x22, 0x0a, 0x31, 0x31, 0x1e };
      ExpectJsonSequenceTwo(bytes, null, null);
      bytes = new byte[] { 0x1e };
      ExpectJsonSequenceOne(bytes, null);
      bytes = new byte[] { 0x1e, 0x1e };
      ExpectJsonSequenceOne(bytes, null);
    }

    [Test]
    public void TestNonUtf8FromJSONBytes() {
      byte[] bytes;
      CBORObject cbor;
      bytes = new byte[] { 0x31, 0, 0x31, 0 };
      cbor = CBORObject.FromJSONBytes(bytes);
      Assert.AreEqual(CBORObject.FromObject(11), cbor);
      bytes = new byte[] { 0x31, 0, 0, 0, 0x31, 0, 0, 0 };
      cbor = CBORObject.FromJSONBytes(bytes);
      Assert.AreEqual(CBORObject.FromObject(11), cbor);
    }

    [Test]
    public void TestReadJSON() {
      try {
        using (var ms2 = new Test.DelayingStream(new byte[] { 0x30 })) {
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
        using (var ms = new Test.DelayingStream(new byte[] {
          0xef, 0xbb,
          0xbf, 0x7b, 0x7d,
        })) {
          try {
            CBORObject.ReadJSON(ms);
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        // whitespace followed by BOM
        using (var ms2 = new Test.DelayingStream(new byte[] {
          0x20, 0xef,
          0xbb, 0xbf, 0x7b, 0x7d,
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
        using (var ms2a = new Test.DelayingStream(new byte[] {
          0x7b, 0x05,
          0x7d,
        })) {
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
        using (var ms2b = new Test.DelayingStream(new byte[] {
          0x05, 0x7b,
          0x7d,
        })) {
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
        using (var ms3 = new Test.DelayingStream(new byte[] {
          0xef, 0xbb,
          0xbf, 0xef, 0xbb, 0xbf, 0x7b, 0x7d,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0, 0, 0xfe,
          0xff, 0,
          0,
          0,
          0x74, 0, 0, 0, 0x72, 0, 0, 0, 0x75, 0, 0, 0,
          0x65,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new Test.DelayingStream(new byte[] {
          0, 0, 0,
          0x74, 0, 0,
          0, 0x72, 0,
          0, 0, 0x75, 0, 0, 0, 0x65,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xff, 0xfe,
          0, 0,
          0x74, 0, 0, 0,
          0x72, 0, 0, 0, 0x75, 0, 0, 0, 0x65, 0, 0, 0,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new Test.DelayingStream(new byte[] {
          0x74, 0, 0,
          0, 0x72,
          0,
          0,
          0,
          0x75, 0, 0, 0, 0x65, 0, 0, 0,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xfe, 0xff,
          0, 0x74,
          0, 0x72, 0,
          0x75, 0, 0x65,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new Test.DelayingStream(new byte[] {
          0, 0x74, 0,
          0x72, 0,
          0x75, 0, 0x65,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xff, 0xfe,
          0x74, 0,
          0x72,
          0,
          0x75,
          0, 0x65, 0,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new Test.DelayingStream(new byte[] {
          0x74, 0,
          0x72, 0,
          0x75, 0, 0x65, 0,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xef, 0xbb,
          0xbf, 0x74, 0x72, 0x75, 0x65,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new Test.DelayingStream(new byte[] {
          0x74, 0x72,
          0x75, 0x65,
        })) {
          Assert.AreEqual(CBORObject.True, CBORObject.ReadJSON(msjson));
        }
        using (var msjson = new Test.DelayingStream(new byte[] {
          0, 0, 0xfe,
          0xff, 0,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0, 0, 0,
          0x22, 0, 1,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xff, 0xfe,
          0, 0,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0x22, 0, 0,
          0, 0, 0,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0, 0x22,
          0xd8, 0,
          0xdc, 0, 0, 0x22,
        })) {
          {
            string stringTemp = CBORObject.ReadJSON(msjson).AsString();
            Assert.AreEqual(
              "\ud800\udc00",
              stringTemp);
          }
        }
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xff, 0xfe,
          0x22, 0,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0x22, 0, 0,
          0xd8, 0,
          0xdc, 0x22, 0,
        })) {
          {
            string stringTemp = CBORObject.ReadJSON(msjson).AsString();
            Assert.AreEqual(
              "\ud800\udc00",
              stringTemp);
          }
        }
        using (var msjson = new Test.DelayingStream(new byte[] {
          0, 0, 0xfe,
          0xff, 0,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0, 0, 0,
          0x22, 0, 0,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xff, 0xfe,
          0, 0,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0x22, 0, 0,
          0, 0,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xfe, 0xff,
          0, 0x22,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0, 0x22, 0,
          0xdc, 0,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xff, 0xfe,
          0x22, 0,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0x22, 0, 0,
          0xdc, 0,
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
        using (var msjson = new Test.DelayingStream(new byte[] { 0xfc })) {
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
        using (var msjson = new Test.DelayingStream(new byte[] { 0, 0 })) {
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xfe, 0xff,
          0x20, 0x20, 0x20,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xff, 0xfe,
          0x20, 0x20, 0x20,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xfe, 0xff,
          0xd8, 0x00,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xfe, 0xff,
          0xdc, 0x00,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xfe, 0xff,
          0xd8, 0x00, 0x20, 0x00,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xfe, 0xff,
          0xdc, 0x00, 0x20, 0x00,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xfe, 0xff,
          0xd8, 0x00, 0xd8, 0x00,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xfe, 0xff,
          0xdc, 0x00, 0xd8, 0x00,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xfe, 0xff,
          0xdc, 0x00, 0xd8, 0x00, 0xdc, 0x00,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xfe, 0xff,
          0xdc, 0x00, 0xdc, 0x00,
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

        using (var msjson = new Test.DelayingStream(new byte[] {
          0xff, 0xfe,
          0x00, 0xd8,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xff, 0xfe,
          0x00, 0xdc,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xff, 0xfe,
          0x00, 0xd8, 0x00, 0x20,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xff, 0xfe,
          0x00, 0xdc, 0x00, 0x20,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xff, 0xfe,
          0x00, 0xd8, 0x00, 0xd8,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xff, 0xfe,
          0x00, 0xdc, 0x00, 0xd8,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xff, 0xfe,
          0x00, 0xdc, 0x00, 0xd8, 0x00, 0xdc,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0xff, 0xfe,
          0x00, 0xdc, 0x00, 0xdc,
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
        using (var msjson = new Test.DelayingStream(new byte[] {
          0, 0, 0,
          0x20, 0,
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
      using (var msjson = new Test.DelayingStream(msbytes)) {
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
    public void TestCalcEncodedSizeCircularRefs1() {
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
    }
    [Test]
    public void TestCalcEncodedSizeCircularRefs4() {
      CBORObject cbor = CBORObject.NewArray().Add(1).Add(2);
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
    }
    [Test]
    public void TestCalcEncodedSizeCircularRefs2() {
      CBORObject cbor = CBORObject.NewMap().Add(1, 2).Add(3, 4);
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
    }
    [Test]
    public void TestCalcEncodedSizeCircularRefs3() {
      CBORObject cbor = CBORObject.NewMap().Add(1, 2).Add(3, 4);
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
    }
    [Test]
    public void TestCalcEncodedSizeCircularRefs5() {
      CBORObject cbor = CBORObject.NewMap().Add(1, 2).Add(3, 4);
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
    }
    [Test]
    public void TestCalcEncodedSizeCircularRefs6() {
      CBORObject cbor = CBORObject.NewMap().Add(1, 2).Add(3, 4);
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
    public void TestCalcEncodedSizeCircularRefs3a() {
      CBORObject cbor = CBORObject.NewOrderedMap().Add(1, 2).Add(3, 4);
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
    }
    [Test]
    public void TestCalcEncodedSizeCircularRefs3b() {
      CBORObject cbor;
      cbor = CBORObject.NewOrderedMap().Add(1, 2).Add(3, 4);
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
    }

    [Test]
    public void TestCalcEncodedSizeCircularRefs3ba() {
      CBORObject cbor;

      cbor = CBORObject.NewOrderedMap().Add("abc", 2).Add("def", 4);
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
    }

    [Test]
    public void TestCalcEncodedSizeCircularRefs3bb() {
      CBORObject cbor;

      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
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
    }

    [Test]
    public void TestCalcEncodedSizeCircularRefs3bc() {
      CBORObject cbor;
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      cbor.Add(CBORObject.NewOrderedMap().Add("jkl", cbor), "test");
      try {
        cbor.CalcEncodedSize();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      cbor.Add("test", CBORObject.NewOrderedMap().Add("jkl", cbor));
      try {
        cbor.CalcEncodedSize();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      cbor.Add(CBORObject.NewOrderedMap().Add(cbor, "jkl"), "test");
      try {
        cbor.CalcEncodedSize();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      cbor.Add("test", CBORObject.NewOrderedMap().Add(cbor, "jkl"));
      try {
        cbor.CalcEncodedSize();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      {
        object objectTemp = CBORObject.NewOrderedMap().Add(cbor,
            "jkl").Add("mno",
            1);
        object objectTemp2 = "test";
        cbor.Add(objectTemp, objectTemp2);
      }
      try {
        cbor.CalcEncodedSize();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      {
        object objectTemp = "test";
        object objectTemp2 = CBORObject.NewOrderedMap().Add(cbor,
            "jkl").Add("mno", 1);
        cbor.Add(objectTemp, objectTemp2);
      }
      try {
        cbor.CalcEncodedSize();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      {
        object objectTemp = "test";
        object objectTemp2 = CBORObject.NewOrderedMap().Add("mno",
            1).Add(cbor, "jkl");
        cbor.Add(objectTemp, objectTemp2);
      }
      try {
        cbor.CalcEncodedSize();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      {
        object objectTemp = CBORObject.NewOrderedMap().Add("mno", 1).Add(cbor,
            "jkl");
        object objectTemp2 = "test";
        cbor.Add(objectTemp, objectTemp2);
      }
      try {
        cbor.CalcEncodedSize();
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // No circular refs
      cbor = CBORObject.NewOrderedMap().Add(1, 2).Add(3, 4);
      cbor.Add("test", CBORObject.NewOrderedMap());
      Assert.IsTrue(cbor.CalcEncodedSize() > 2);
      cbor = CBORObject.NewOrderedMap().Add("abc", 2).Add("def", 4);
      cbor.Add("test", CBORObject.NewOrderedMap());
      Assert.IsTrue(cbor.CalcEncodedSize() > 2);
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      cbor.Add("test", CBORObject.NewOrderedMap());
      Assert.IsTrue(cbor.CalcEncodedSize() > 2);
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      {
        object objectTemp = CBORObject.NewOrderedMap().Add("jkl",
            CBORObject.NewOrderedMap());
        object objectTemp2 = "test";
        cbor.Add(objectTemp, objectTemp2);
      }
      Assert.IsTrue(cbor.CalcEncodedSize() > 2);
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      {
        object objectTemp = "test";
        object objectTemp2 = CBORObject.NewOrderedMap().Add("jkl",
            CBORObject.NewOrderedMap());
        cbor.Add(objectTemp, objectTemp2);
      }
      Assert.IsTrue(cbor.CalcEncodedSize() > 2);
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      {
        object objectTemp =
          CBORObject.NewOrderedMap().Add(CBORObject.NewOrderedMap(),
            "jkl");
        object objectTemp2 = "test";
        cbor.Add(objectTemp, objectTemp2);
      }
      Assert.IsTrue(cbor.CalcEncodedSize() > 2);
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      {
        object objectTemp = "test";
        object objectTemp2 = CBORObject.NewOrderedMap()
          .Add(CBORObject.NewOrderedMap(),
            "jkl");
        cbor.Add(objectTemp, objectTemp2);
      }
      Assert.IsTrue(cbor.CalcEncodedSize() > 2);
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      {
        CBORObject.NewOrderedMap().Add(CBORObject.NewOrderedMap(),
          "jkl").Add("mno",
            1);
        object objectTemp2 = "test";
        cbor.Add("test", objectTemp2);
      }
      Assert.IsTrue(cbor.CalcEncodedSize() > 2);
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      {
        object objectTemp2 =
          CBORObject.NewOrderedMap().Add(CBORObject.NewOrderedMap(),
            "jkl").Add("mno",
            1);
        cbor.Add("test", objectTemp2);
      }
      Assert.IsTrue(cbor.CalcEncodedSize() > 2);
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      {
        object objectTemp2 =
          CBORObject.NewOrderedMap().Add("mno", 1).Add(
            CBORObject.NewOrderedMap(),
            "jkl");
        cbor.Add("test", objectTemp2);
      }
      Assert.IsTrue(cbor.CalcEncodedSize() > 2);
      cbor = CBORObject.NewOrderedMap().Add("ghi", 2).Add("abc", 4);
      {
        object objectTemp =
          CBORObject.NewOrderedMap().Add("mno", 1).Add(
            CBORObject.NewOrderedMap(),
            "jkl");
        cbor.Add(objectTemp, "test");
      }
      Assert.IsTrue(cbor.CalcEncodedSize() > 2);
    }

    [Test]
    public void TestCalcEncodedSizeCircularRefs2a() {
      CBORObject cbor = CBORObject.NewOrderedMap().Add(1, 2).Add(3, 4);
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
    }

    [Test]
    public void TestCalcEncodedSizeCircularRefs5a() {
      CBORObject cbor = CBORObject.NewOrderedMap().Add(1, 2).Add(3, 4);
      CBORObject cbor2 = CBORObject.NewArray().Add(cbor);
      cbor.Add(cbor2, "test");
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
    public void TestCalcEncodedSizeCircularRefs6a() {
      CBORObject cbor = CBORObject.NewOrderedMap().Add(1, 2).Add(3, 4);
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
        string numberString = numberinfo["number"].AsString();
        CBORObject cbornumber =
          ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(
              numberString));
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
        } else if (numberString.Length > 0 && numberString[0] == '-') {
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
    [Timeout(1000)]
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
    public void TestCompareToUnicodeString() {
      CBORObject cbora;
      CBORObject cborb;
      cbora = CBORObject.FromObject("aa\ud200\ue000");
      cborb = CBORObject.FromObject("aa\ud200\ue001");
      TestCommon.CompareTestLess(cbora, cborb);
      cbora = CBORObject.FromObject("aa\ud200\ue000");
      cborb = CBORObject.FromObject("aa\ud201\ue000");
      TestCommon.CompareTestLess(cbora, cborb);
      cbora = CBORObject.FromObject("aa\ud800\udc00\ue000");
      cborb = CBORObject.FromObject("aa\ue001\ue000");
      TestCommon.CompareTestGreater(cbora, cborb);
      cbora = CBORObject.FromObject("aa\ud800\udc00\ue000");
      cborb = CBORObject.FromObject("aa\ud800\udc01\ue000");
      TestCommon.CompareTestLess(cbora, cborb);
      cbora = CBORObject.FromObject("aa\ud800\udc00\ue000");
      cborb = CBORObject.FromObject("aa\ud801\udc00\ue000");
      TestCommon.CompareTestLess(cbora, cborb);
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
    public void TestToFloatingPointBits() {
      try {
        CBORObject.FromFloatingPointBits(0, 0);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromFloatingPointBits(0, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromFloatingPointBits(0, 3);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromFloatingPointBits(0, 5);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromFloatingPointBits(0, 6);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromFloatingPointBits(0, 7);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromFloatingPointBits(0, 9);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestToFloatingPointBitsSingle() {
      // Regression test
      CBORObject o;
      o = CBORObject.FromFloatingPointBits(2140148306L, 4);
      Assert.IsTrue(Double.IsNaN(o.AsDoubleValue()));
      o = CBORObject.FromFloatingPointBits(1651724151L, 4);
      Assert.IsTrue(o.AsDoubleValue() == 1.1220712138406615E21);
      o = CBORObject.FromFloatingPointBits(-1566356128L, 4);
      Assert.IsTrue(o.AsDoubleValue() == -4.426316249665156E-18);
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

    public static void TestWriteExtraOne(long longValue) {
      try {
        {
          CBORObject cborTemp1 =
            ToObjectTest.TestToFromObjectRoundTrip(longValue);
          CBORObject cborTemp2 =
            ToObjectTest.TestToFromObjectRoundTrip((object)longValue);
          TestCommon.CompareTestEqualAndConsistent(cborTemp1, cborTemp2);
          try {
            CBORObject.Write(longValue, null);
            Assert.Fail("Should have failed");
          } catch (ArgumentNullException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          AssertWriteThrow(cborTemp1);
          using (var ms = new Test.DelayingStream()) {
            CBORObject.Write(longValue, ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            AssertReadThree(
              ms.ToArray(),
              ToObjectTest.TestToFromObjectRoundTrip(longValue));
          }
          TestWriteObj((object)longValue, longValue);
        }

        EInteger bigintVal = EInteger.FromInt64(longValue);
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
          using (var ms = new Test.DelayingStream()) {
            CBORObject.Write(bigintVal, ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            AssertReadThree(
              ms.ToArray(),
              ToObjectTest.TestToFromObjectRoundTrip(bigintVal));
          }
          TestWriteObj((object)bigintVal, bigintVal);
        }

        if (longValue >= (long)Int32.MinValue && longValue <=
          (long)Int32.MaxValue) {
          var intval = (int)longValue;
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
            using (var ms = new Test.DelayingStream()) {
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
        if (longValue >= -32768L && longValue <= 32767) {
          var shortval = (short)longValue;
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
          using (var ms = new Test.DelayingStream()) {
            CBORObject.Write(shortval, ms);
            CBORObject.Write(cborTemp1, ms);
            cborTemp1.WriteTo(ms);
            AssertReadThree(
              ms.ToArray(),
              ToObjectTest.TestToFromObjectRoundTrip(shortval));
          }
          TestWriteObj((object)shortval, shortval);
        }
        if (longValue >= 0L && longValue <= 255) {
          var byteval = (byte)longValue;
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
            using (var ms = new Test.DelayingStream()) {
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
      } catch (IOException ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(ex.ToString(), ex);
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
          using (var ms = new Test.DelayingStream()) {
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
          using (var ms = new Test.DelayingStream()) {
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
          using (var ms = new Test.DelayingStream()) {
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
          TestWriteExtraOne(values[i]);
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
          using (var ms = new Test.DelayingStream()) {
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
          using (var ms = new Test.DelayingStream()) {
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
          using (var ms = new Test.DelayingStream()) {
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
          using (var ms = new Test.DelayingStream()) {
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
          using (var ms = new Test.DelayingStream()) {
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
            using (var ms = new Test.DelayingStream()) {
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
            using (var ms = new Test.DelayingStream()) {
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
            using (var ms = new Test.DelayingStream()) {
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
            using (var ms = new Test.DelayingStream()) {
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
          using (var ms = new Test.DelayingStream()) {
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
          using (var ms = new Test.DelayingStream()) {
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
          using (var ms = new Test.DelayingStream()) {
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
          using (var ms = new Test.DelayingStream()) {
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
          using (var ms = new Test.DelayingStream()) {
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
          using (var ms = new Test.DelayingStream()) {
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
        using (var ms = new Test.DelayingStream()) {
          CBORObject.WriteJSON(CBORObject.True, ms);
          byte[] bytes = ms.ToArray();
          string str = DataUtilities.GetUtf8String(bytes, false);
          Assert.AreEqual("true", str);
        }
        using (var ms = new Test.DelayingStream()) {
          CBORObject.True.WriteJSONTo(ms);
          byte[] bytes = ms.ToArray();
          string str = DataUtilities.GetUtf8String(bytes, false);
          Assert.AreEqual("true", str);
        }
        using (var ms = new Test.DelayingStream()) {
          CBORObject.WriteJSON(CBORObject.False, ms);
          byte[] bytes = ms.ToArray();
          string str = DataUtilities.GetUtf8String(bytes, false);
          Assert.AreEqual("false", str);
        }
        using (var ms = new Test.DelayingStream()) {
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

    private static string Chop(string str) {
      return (str.Length < 100) ? str : (str.Substring(0, 100) + "...");
    }

    private static void AssertReadThree(byte[] bytes) {
      try {
        using (var ms = new Test.DelayingStream(bytes)) {
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
        using (var ms = new Test.DelayingStream(bytes)) {
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
          Chop(TestCommon.ToByteArrayString(bytes)) + "\r\n" +
          "cbor = " + Chop(cbor.ToString()) + "\r\n");
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
          using (var ms = new Test.DelayingStream()) {
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
          using (var ms = new Test.DelayingStream()) {
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
        using (var ms = new Test.DelayingStream()) {
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
    public void TestKeepKeyOrder() {
      byte[] bytes;
      byte[] bytes2;
      CBORObject cbor;
      var list = new List<CBORObject>();
      var options = new CBOREncodeOptions("keepkeyorder=true");
      Assert.IsTrue(options.KeepKeyOrder);
      bytes = new byte[] { (byte)0xa3, 0x01, 0, 0x02, 0, 0x03, 0 };
      cbor = CBORObject.DecodeFromBytes(bytes, options);
      foreach (CBORObject key in cbor.Keys) {
        list.Add(key);
      }
      Assert.AreEqual(CBORObject.FromObject(1), list[0]);
      Assert.AreEqual(CBORObject.FromObject(2), list[1]);
      Assert.AreEqual(CBORObject.FromObject(3), list[2]);
      bytes2 = cbor.EncodeToBytes();
      TestCommon.AssertByteArraysEqual(bytes, bytes2);
      list = new List<CBORObject>();
      bytes = new byte[] { (byte)0xbf, 0x01, 0, 0x02, 0, 0x03, 0, 0xff };
      cbor = CBORObject.DecodeFromBytes(bytes, options);
      foreach (CBORObject key in cbor.Keys) {
        list.Add(key);
      }
      Assert.AreEqual(CBORObject.FromObject(1), list[0]);
      Assert.AreEqual(CBORObject.FromObject(2), list[1]);
      Assert.AreEqual(CBORObject.FromObject(3), list[2]);
      bytes = new byte[] { (byte)0xa3, 0x01, 0, 0x02, 0, 0x03, 0 };
      bytes2 = cbor.EncodeToBytes();
      TestCommon.AssertByteArraysEqual(bytes, bytes2);
      list = new List<CBORObject>();
      bytes = new byte[] { (byte)0xa3, 0x03, 0, 0x02, 0, 0x01, 0 };
      cbor = CBORObject.DecodeFromBytes(bytes, options);
      foreach (CBORObject key in cbor.Keys) {
        list.Add(key);
      }
      Assert.AreEqual(CBORObject.FromObject(3), list[0]);
      Assert.AreEqual(CBORObject.FromObject(2), list[1]);
      Assert.AreEqual(CBORObject.FromObject(1), list[2]);
      bytes2 = cbor.EncodeToBytes();
      TestCommon.AssertByteArraysEqual(bytes, bytes2);
      list = new List<CBORObject>();
      bytes = new byte[] { (byte)0xbf, 0x03, 0, 0x02, 0, 0x01, 0, 0xff };
      cbor = CBORObject.DecodeFromBytes(bytes, options);
      foreach (CBORObject key in cbor.Keys) {
        list.Add(key);
      }
      Assert.AreEqual(CBORObject.FromObject(3), list[0]);
      Assert.AreEqual(CBORObject.FromObject(2), list[1]);
      Assert.AreEqual(CBORObject.FromObject(1), list[2]);
      bytes = new byte[] { (byte)0xa3, 0x03, 0, 0x02, 0, 0x01, 0 };
      bytes2 = cbor.EncodeToBytes();
      TestCommon.AssertByteArraysEqual(bytes, bytes2);

      // JSON
      var joptions = new JSONOptions("keepkeyorder=true");
      Assert.IsTrue(joptions.KeepKeyOrder);
      string jsonstring;
      jsonstring = "{\"1\":0,\"2\":0,\"3\":0}";
      cbor = CBORObject.FromJSONString(jsonstring, joptions);
      list = new List<CBORObject>();
      foreach (CBORObject key in cbor.Keys) {
        list.Add(key);
      }
      Assert.AreEqual(CBORObject.FromObject("1"), list[0]);
      Assert.AreEqual(CBORObject.FromObject("2"), list[1]);
      Assert.AreEqual(CBORObject.FromObject("3"), list[2]);

      jsonstring = "{\"3\":0,\"2\":0,\"1\":0}";
      cbor = CBORObject.FromJSONString(jsonstring, joptions);
      list = new List<CBORObject>();
      foreach (CBORObject key in cbor.Keys) {
        list.Add(key);
      }
      Assert.AreEqual(CBORObject.FromObject("3"), list[0]);
      Assert.AreEqual(CBORObject.FromObject("2"), list[1]);
      Assert.AreEqual(CBORObject.FromObject("1"), list[2]);

      jsonstring = "{\"3\":0,\"2\":0,\"1\":0}";
      bytes = DataUtilities.GetUtf8Bytes(jsonstring, false);
      cbor = CBORObject.FromJSONBytes(bytes, joptions);
      list = new List<CBORObject>();
      foreach (CBORObject key in cbor.Keys) {
        list.Add(key);
      }
      Assert.AreEqual(CBORObject.FromObject("3"), list[0]);
      Assert.AreEqual(CBORObject.FromObject("2"), list[1]);
      Assert.AreEqual(CBORObject.FromObject("1"), list[2]);
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
            using (var ms = new Test.DelayingStream()) {
              CBORObject.WriteFloatingPointValue(
                ms,
                cbor.AsDouble(),
                2);
              TestCommon.AssertByteArraysEqual(bytes, ms.ToArray());
            }
            using (var ms = new Test.DelayingStream()) {
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
            using (var ms = new Test.DelayingStream()) {
              CBORObject.WriteFloatingPointValue(
                ms,
                cbor.AsDouble(),
                4);
              TestCommon.AssertByteArraysEqual(bytes, ms.ToArray());
            }
            using (var ms = new Test.DelayingStream()) {
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
            using (var ms = new Test.DelayingStream()) {
              CBORObject.WriteFloatingPointValue(
                ms,
                cbor.AsDouble(),
                8);
              TestCommon.AssertByteArraysEqual(bytes, ms.ToArray());
            }
            CBORObject c2 = null;
            byte[] c2bytes = null;
            using (var ms = new Test.DelayingStream()) {
              CBORObject.WriteFloatingPointValue(
                ms,
                cbor.AsSingle(),
                8);
              c2bytes = ms.ToArray();
              c2 = CBORObject.DecodeFromBytes(
                  c2bytes);
            }
            using (var ms = new Test.DelayingStream()) {
              CBORObject.WriteFloatingPointValue(
                ms,
                c2.AsSingle(),
                8);
              TestCommon.AssertByteArraysEqual(c2bytes, ms.ToArray());
            }
            if (i == 0) {
              using (var ms = new Test.DelayingStream()) {
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

    private static void TestDateTimeStringNumberOne(string str, long num) {
      CBORObject dtstring = CBORObject.FromObject(str).WithTag(0);
      CBORObject dtnum = CBORObject.FromObject(num).WithTag(1);
      TestDateTimeStringNumberOne(dtstring, dtnum);
    }
    private static void TestDateTimeStringNumberOne(string str, double num) {
      CBORObject dtstring = CBORObject.FromObject(str).WithTag(0);
      CBORObject dtnum = CBORObject.FromObject(num).WithTag(1);
      TestDateTimeStringNumberOne(dtstring, dtnum);
    }
    private static void TestDateTimeStringNumberOne(CBORObject dtstring,
      CBORObject dtnum) {
      CBORDateConverter convNumber = CBORDateConverter.TaggedNumber;
      CBORDateConverter convString = CBORDateConverter.TaggedString;
      CBORObject cbor;
      var eiYear = new EInteger[1];
      var lesserFields = new int[7];
      string strnum = dtstring + ", " + dtnum;
      cbor = convNumber.ToCBORObject(convNumber.FromCBORObject(dtstring));
      Assert.AreEqual(dtnum, cbor, strnum);
      if (!convNumber.TryGetDateTimeFields(dtstring, eiYear, lesserFields)) {
        Assert.Fail(strnum);
      }
      cbor = convNumber.DateTimeFieldsToCBORObject(eiYear[0], lesserFields);
      Assert.AreEqual(dtnum, cbor, strnum);
      cbor = convString.DateTimeFieldsToCBORObject(eiYear[0], lesserFields);
      Assert.AreEqual(dtstring, cbor, strnum);
      cbor = convString.ToCBORObject(convString.FromCBORObject(dtnum));
      Assert.AreEqual(dtstring, cbor, strnum);
      if (!convString.TryGetDateTimeFields(dtnum, eiYear, lesserFields)) {
        Assert.Fail(strnum);
      }
      cbor = convNumber.DateTimeFieldsToCBORObject(eiYear[0], lesserFields);
      Assert.AreEqual(dtnum, cbor, strnum);
      cbor = convString.DateTimeFieldsToCBORObject(eiYear[0], lesserFields);
      Assert.AreEqual(dtstring, cbor, strnum);
    }

    [Test]
    public void TestDateTimeStringNumber() {
      TestDateTimeStringNumberOne("1970-01-01T00:00:00.25Z", 0.25);
      TestDateTimeStringNumberOne("1970-01-01T00:00:00.75Z", 0.75);
      TestDateTimeStringNumberOne("1969-12-31T23:59:59.75Z", -0.25);
      TestDateTimeStringNumberOne("1969-12-31T23:59:59.25Z", -0.75);
      TestDateTimeStringNumberOne("1970-01-03T00:00:00Z", 172800);
      TestDateTimeStringNumberOne("1970-01-03T00:00:00Z", 172800);
      TestDateTimeStringNumberOne("1970-01-03T00:00:00Z", 172800);
      TestDateTimeStringNumberOne("2001-01-03T00:00:00Z", 978480000);
      TestDateTimeStringNumberOne("2001-01-03T00:00:00.25Z", 978480000.25);
      TestDateTimeStringNumberOne("1960-01-03T00:00:00Z", -315446400);
      TestDateTimeStringNumberOne("1400-01-03T00:00:00Z", -17987270400L);
      TestDateTimeStringNumberOne("2100-01-03T00:00:00Z", 4102617600L);
      TestDateTimeStringNumberOne("1970-01-03T00:00:01Z", 172801);
      TestDateTimeStringNumberOne("2001-01-03T00:00:01Z", 978480001);
      TestDateTimeStringNumberOne("1960-01-03T00:00:01Z", -315446399);
      TestDateTimeStringNumberOne("1960-01-03T00:00:00.25Z", -315446399.75);
      TestDateTimeStringNumberOne("1960-01-03T00:00:00.75Z", -315446399.25);
      TestDateTimeStringNumberOne("1400-01-03T00:00:01Z", -17987270399L);
      TestDateTimeStringNumberOne("2100-01-03T00:00:01Z", 4102617601L);
    }

    public static void TestApplyJSONPatchOpAdd(
      CBORObject expected,
      CBORObject src,
      string path,
      object obj) {
      CBORObject patch = CBORObject.NewMap().Add("op", "add")
        .Add("path", path).Add("value", CBORObject.FromObject(obj));
      patch = CBORObject.NewArray().Add(patch);
      TestApplyJSONPatchOp(expected, src, patch);
    }

    public void TestApplyJSONPatchOpReplace(
      CBORObject expected,
      CBORObject src,
      string path,
      object obj) {
      CBORObject patch = CBORObject.NewMap().Add("op", "replace")
        .Add("path", path).Add("value", CBORObject.FromObject(obj));
      patch = CBORObject.NewArray().Add(patch);
      TestApplyJSONPatchOp(expected, src, patch);
    }

    public static void TestApplyJSONPatchOpRemove(
      CBORObject expected,
      CBORObject src,
      string path) {
      CBORObject patch = CBORObject.NewMap().Add("op", "remove")
        .Add("path", path);
      patch = CBORObject.NewArray().Add(patch);
      TestApplyJSONPatchOp(expected, src, patch);
    }

    public static void TestApplyJSONPatchOp(
      CBORObject expected,
      CBORObject src,
      CBORObject patch) {
      CBORObject actual = CBORObject.DecodeFromBytes(src.EncodeToBytes());
      if (expected == null) {
        try {
          actual.ApplyJSONPatch(patch);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          throw new InvalidOperationException(ex.ToString() + "\n" + patch);
        }
      } else {
        try {
          byte[] oldactualbytes = actual.EncodeToBytes();
          CBORObject oldactual = actual;
          actual = actual.ApplyJSONPatch(patch);
          byte[] newactualbytes = oldactual.EncodeToBytes();
          // Check whether the patch didn't change the existing object
          TestCommon.AssertByteArraysEqual(oldactualbytes, newactualbytes);
        } catch (Exception ex) {
          throw new InvalidOperationException(ex.ToString() + "\n" + patch);
        }
        Assert.AreEqual(expected, actual);
      }
    }

    public static void TestApplyJSONPatchJSONTestsCore(string patchTests) {
      CBORObject tests = CBORObject.FromJSONString(patchTests,
          new JSONOptions("allowduplicatekeys=1"));
      foreach (CBORObject testcbor in tests.Values) {
        if (testcbor.GetOrDefault("disabled", CBORObject.False).AsBoolean()) {
          continue;
        }
        string err = testcbor.GetOrDefault("error",
            CBORObject.FromObject(String.Empty)).AsString();
        string comment = testcbor.GetOrDefault("comment",
            CBORObject.FromObject(String.Empty)).AsString();
        try {
          if (testcbor.ContainsKey("error")) {
            TestApplyJSONPatchOp(null, testcbor["doc"], testcbor["patch"]);
          } else {
            TestApplyJSONPatchOp(
              testcbor["expected"],
              testcbor["doc"],
              testcbor["patch"]);
          }
        } catch (Exception ex) {
          string exmsg = ex.GetType() + "\n" + comment + "\n" + err;
          throw new InvalidOperationException(exmsg, ex);
        }
      }
    }

    [Test]
    public void TestApplyJSONPatchTest() {
      CBORObject patch;
      patch = CBORObject.NewMap().Add("op", "test")
        .Add("path", String.Empty).Add("value",
          CBORObject.NewArray().Add(1).Add(2));
      patch = CBORObject.NewArray().Add(patch);
      CBORObject exp;
      exp = CBORObject.NewArray().Add(1).Add(2);
      TestApplyJSONPatchOp(exp, exp, patch);
      patch = CBORObject.NewMap().Add("op", "test")
        .Add("path", String.Empty).Add("value",
          CBORObject.NewArray().Add(1).Add(3));
      patch = CBORObject.NewArray().Add(patch);
      TestApplyJSONPatchOp(null, exp, patch);
      patch = CBORObject.NewMap().Add("op", "test")
        .Add("path", String.Empty).Add("value",
          CBORObject.NewArray().Add(2).Add(2));
      patch = CBORObject.NewArray().Add(patch);
      TestApplyJSONPatchOp(null, exp, patch);
      patch = CBORObject.NewMap().Add("op", "test")
        .Add("path", String.Empty).Add("value", CBORObject.NewMap().Add(2,
            2));
      patch = CBORObject.NewArray().Add(patch);
      TestApplyJSONPatchOp(null, exp, patch);
      patch = CBORObject.NewMap().Add("op", "test")
        .Add("path", String.Empty).Add("value", CBORObject.True);
      patch = CBORObject.NewArray().Add(patch);
      TestApplyJSONPatchOp(null, exp, patch);
      patch = CBORObject.NewMap().Add("op", "test")
        .Add("path", String.Empty).Add("value", CBORObject.Null);
      patch = CBORObject.NewArray().Add(patch);
      TestApplyJSONPatchOp(null, exp, patch);
    }

    [Test]
    public void TestApplyJSONPatch() {
      // TODO: Finish tests for ApplyJSONPatch
      CBORObject patch;
      patch = CBORObject.NewArray().Add(
          CBORObject.NewMap().Add("path", "/0"));
      TestApplyJSONPatchOp(
        null,
        CBORObject.NewArray().Add(1),
        patch);

      patch = CBORObject.NewArray().Add(
          CBORObject.NewMap().Add("op", "ADD").Add("path", "/0").Add("value",
            3));
      TestApplyJSONPatchOp(
        null,
        CBORObject.NewArray().Add(1),
        patch);

      patch = CBORObject.NewArray().Add(
          CBORObject.NewMap().Add("op", "RePlAcE").Add("path",
            "/0").Add("value", 3));
      TestApplyJSONPatchOp(
        null,
        CBORObject.NewArray().Add(1),
        patch);

      patch = CBORObject.NewArray().Add(
          CBORObject.NewMap().Add("op", "unknown").Add("path", "/0"));
      TestApplyJSONPatchOp(
        null,
        CBORObject.NewArray().Add(1),
        patch);
      patch = CBORObject.NewArray().Add(
          CBORObject.NewMap().Add("op", "add").Add("path", String.Empty)
          .Add("value", CBORObject.True));
      TestApplyJSONPatchOp(
        CBORObject.True,
        CBORObject.NewArray().Add(1),
        patch);
      patch = CBORObject.NewArray().Add(
          CBORObject.NewMap().Add("op", "add").Add("path", String.Empty)
          .Add("value", CBORObject.NewMap()));
      TestApplyJSONPatchOp(
        CBORObject.NewMap(),
        CBORObject.NewArray().Add(1),
        patch);

      patch = CBORObject.NewArray().Add(
          CBORObject.NewMap().Add("op", "add").Add("path", "/0"));
      TestApplyJSONPatchOp(
        null,
        CBORObject.NewArray().Add(1),
        patch);
      patch = CBORObject.NewArray().Add(
          CBORObject.NewMap().Add("op", "add").Add("path", null).Add("value",
            2));
      TestApplyJSONPatchOp(
        null,
        CBORObject.NewArray().Add(1),
        patch);
      patch = CBORObject.NewArray().Add(
          CBORObject.NewMap().Add("op", "add").Add("value", 2));
      TestApplyJSONPatchOp(
        null,
        CBORObject.NewArray().Add(1),
        patch);
      patch = CBORObject.NewArray().Add(
          CBORObject.NewMap().Add("op", "remove"));
      TestApplyJSONPatchOp(
        null,
        CBORObject.NewArray().Add(1),
        patch);
      patch = CBORObject.NewArray().Add(
          CBORObject.NewMap().Add("op", "remove").Add("value", 2));
      TestApplyJSONPatchOp(
        null,
        CBORObject.NewArray().Add(1),
        patch);

      TestApplyJSONPatchOp(
        null,
        CBORObject.NewArray().Add(1),
        patch);
      patch = CBORObject.NewArray().Add(
          CBORObject.NewMap().Add("op", "replace"));
      TestApplyJSONPatchOp(
        null,
        CBORObject.NewArray().Add(1),
        patch);
      patch = CBORObject.NewArray().Add(
          CBORObject.NewMap().Add("op", "replace").Add("valuuuuu", 2));
      TestApplyJSONPatchOp(
        null,
        CBORObject.NewArray().Add(1),
        patch);
      patch = CBORObject.NewArray().Add(
          CBORObject.NewMap().Add("op", "replace").Add("path", "/0"));
      TestApplyJSONPatchOp(
        null,
        CBORObject.NewArray().Add(1),
        patch);

      this.TestApplyJSONPatchOpReplace(
        CBORObject.NewArray().Add(1).Add(3),
        CBORObject.NewArray().Add(1).Add(2),
        "/1",
        3);
      this.TestApplyJSONPatchOpReplace(
        CBORObject.NewArray().Add(3).Add(2),
        CBORObject.NewArray().Add(1).Add(2),
        "/0",
        3);
      this.TestApplyJSONPatchOpReplace(
        null,
        CBORObject.NewArray().Add(1).Add(2),
        "/00",
        3);
      this.TestApplyJSONPatchOpReplace(
        null,
        CBORObject.NewArray().Add(1).Add(2),
        "/00000",
        3);
      this.TestApplyJSONPatchOpReplace(
        CBORObject.NewMap().Add("f1", "f2").Add("f3", 3),
        CBORObject.NewMap().Add("f1", "f2").Add("f3", "f4"),
        "/f3",
        3);
      this.TestApplyJSONPatchOpReplace(
        CBORObject.NewMap().Add("f1", 3).Add("f3", "f4"),
        CBORObject.NewMap().Add("f1", "f2").Add("f3", "f4"),
        "/f1",
        3);
      this.TestApplyJSONPatchOpReplace(
        null,
        CBORObject.NewMap().Add("f1", "f2").Add("f3", "f4"),
        "/foo",
        3);
      this.TestApplyJSONPatchOpReplace(
        null,
        CBORObject.NewMap().Add("f1", "f2").Add("f3", "f4"),
        "/f1/xyz",
        3);
      this.TestApplyJSONPatchOpReplace(
        null,
        CBORObject.NewMap().Add("f1", "f2").Add("f3", "f4"),
        "/f1/",
        3);
      this.TestApplyJSONPatchOpReplace(
        null,
        CBORObject.NewMap().Add("f1", "f2").Add("f3", "f4"),
        "/0",
        3);
      this.TestApplyJSONPatchOpReplace(
        null,
        CBORObject.NewMap().Add("f1", "f2").Add("f3", "f4"),
        "/-",
        3);
      this.TestApplyJSONPatchOpReplace(
        null,
        CBORObject.NewArray().Add(1).Add(2),
        "/-",
        3);
      this.TestApplyJSONPatchOpReplace(
        null,
        CBORObject.NewArray().Add(1).Add(2),
        "/foo",
        3);
      TestApplyJSONPatchOpRemove(
        CBORObject.NewArray().Add(1),
        CBORObject.NewArray().Add(1).Add(2),
        "/1");
      TestApplyJSONPatchOpRemove(
        null,
        CBORObject.NewArray().Add(1).Add(2),
        "/01");
      TestApplyJSONPatchOpRemove(
        CBORObject.NewArray().Add(2),
        CBORObject.NewArray().Add(1).Add(2),
        "/0");
      TestApplyJSONPatchOpRemove(
        CBORObject.NewMap().Add("f1", "f2"),
        CBORObject.NewMap().Add("f1", "f2").Add("f3", "f4"),
        "/f3");
      TestApplyJSONPatchOpRemove(
        CBORObject.NewMap().Add("f3", "f4"),
        CBORObject.NewMap().Add("f1", "f2").Add("f3", "f4"),
        "/f1");
      TestApplyJSONPatchOpRemove(
        null,
        CBORObject.NewMap().Add("f1", "f2").Add("f3", "f4"),
        "/foo");
      TestApplyJSONPatchOpRemove(
        null,
        CBORObject.NewMap().Add("f1", "f2").Add("f3", "f4"),
        "/f1/xyz");
      TestApplyJSONPatchOpRemove(
        null,
        CBORObject.NewMap().Add("f1", "f2").Add("f3", "f4"),
        "/f1/");
      TestApplyJSONPatchOpRemove(
        null,
        CBORObject.NewMap().Add("f1", "f2").Add("f3", "f4"),
        "/0");
      TestApplyJSONPatchOpRemove(
        null,
        CBORObject.NewMap().Add("f1", "f2").Add("f3", "f4"),
        "/-");
      TestApplyJSONPatchOpRemove(
        null,
        CBORObject.NewArray().Add(1).Add(2),
        "/-");
      TestApplyJSONPatchOpRemove(
        null,
        CBORObject.NewArray().Add(1).Add(2),
        "/foo");
      TestApplyJSONPatchOpAdd(
        CBORObject.NewArray().Add(1),
        CBORObject.NewArray(),
        "/-",
        1);
      TestApplyJSONPatchOpAdd(
        CBORObject.NewArray().Add(1),
        CBORObject.NewArray(),
        "/0",
        1);
      TestApplyJSONPatchOpAdd(
        null,
        CBORObject.NewArray(),
        "/1",
        1);
      TestApplyJSONPatchOpAdd(
        CBORObject.NewArray().Add(1).Add(2),
        CBORObject.NewArray().Add(1),
        "/-",
        2);
      TestApplyJSONPatchOpAdd(
        CBORObject.NewArray().Add(0).Add(1).Add(2),
        CBORObject.NewArray().Add(1).Add(2),
        "/0",
        0);
      TestApplyJSONPatchOpAdd(
        CBORObject.NewArray().Add(1).Add(0).Add(2),
        CBORObject.NewArray().Add(1).Add(2),
        "/1",
        0);
      TestApplyJSONPatchOpAdd(
        CBORObject.NewArray().Add(1).Add(2).Add(0),
        CBORObject.NewArray().Add(1).Add(2),
        "/2",
        0);
      TestApplyJSONPatchOpAdd(
        null,
        CBORObject.NewArray().Add(1).Add(2),
        "/3",
        0);
      TestApplyJSONPatchOpAdd(
        null,
        CBORObject.NewArray().Add(1).Add(2),
        "/foo",
        0);
      TestApplyJSONPatchOpAdd(
        CBORObject.NewMap().Add("foo", "bar"),
        CBORObject.NewMap(),
        "/foo",
        "bar");
      TestApplyJSONPatchOpAdd(
        CBORObject.NewMap().Add("foo", "baz"),
        CBORObject.NewMap().Add("foo", "bar"),
        "/foo",
        "baz");
    }

    [Test]
    public void TestAtJSONPointer() {
      CBORObject cbor;
      cbor = CBORObject.FromObject("xyz");
      Assert.AreEqual(cbor, cbor.AtJSONPointer(String.Empty));
      try {
        cbor.AtJSONPointer(null);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.AtJSONPointer("/");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.AtJSONPointer("/foo");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.FromObject(0);
      Assert.AreEqual(cbor, cbor.AtJSONPointer(String.Empty));
      try {
        cbor.AtJSONPointer(null);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.AtJSONPointer("/");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.AtJSONPointer("/foo");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.FromObject(0.5);
      Assert.AreEqual(cbor, cbor.AtJSONPointer(String.Empty));
      try {
        cbor.AtJSONPointer(null);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.AtJSONPointer("/");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.AtJSONPointer("/foo");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap();
      Assert.AreEqual(cbor, cbor.AtJSONPointer(String.Empty));
      try {
        cbor.AtJSONPointer(null);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.AtJSONPointer("/");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.AtJSONPointer("/foo");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewArray();
      Assert.AreEqual(cbor, cbor.AtJSONPointer(String.Empty));
      try {
        cbor.AtJSONPointer(null);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.AtJSONPointer("/");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.AtJSONPointer("/foo");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor.Add(3);
      Assert.AreEqual(cbor[0], cbor.AtJSONPointer("/0"));
      try {
        cbor.AtJSONPointer("/1");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.AtJSONPointer("/-");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add("foo", 0);
      Assert.AreEqual(cbor, cbor.AtJSONPointer(String.Empty));
      try {
        cbor.AtJSONPointer(null);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.AtJSONPointer("/");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(cbor["foo"], cbor.AtJSONPointer("/foo"));
      try {
        cbor.AtJSONPointer("/bar");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add("f~o", 0);
      Assert.AreEqual(cbor["f~o"], cbor.AtJSONPointer("/f~0o"));
      cbor = CBORObject.NewMap().Add("f~0o", 0);
      Assert.AreEqual(cbor["f~0o"], cbor.AtJSONPointer("/f~00o"));
      cbor = CBORObject.NewMap().Add("f~1o", 0);
      Assert.AreEqual(cbor["f~1o"], cbor.AtJSONPointer("/f~01o"));
      cbor = CBORObject.NewMap().Add("f/o", 0);
      Assert.AreEqual(cbor["f/o"], cbor.AtJSONPointer("/f~1o"));
      cbor = CBORObject.NewMap().Add("foo", CBORObject.NewMap().Add("bar",
            345));

      Assert.AreEqual(
        CBORObject.FromObject(345),
        cbor.AtJSONPointer("/foo/bar"));
      cbor = CBORObject.NewMap().Add("foo", CBORObject.NewArray().Add(678));
      Assert.AreEqual(CBORObject.FromObject(678), cbor.AtJSONPointer("/foo/0"));
      try {
        cbor.AtJSONPointer("/foo/1");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.AtJSONPointer("/foo/-");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        cbor.AtJSONPointer("/foo/-1");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      cbor = CBORObject.NewMap().Add("-", 0);
      Assert.AreEqual(cbor["-"], cbor.AtJSONPointer("/-"));
      cbor = CBORObject.NewMap().Add(String.Empty, 0);
      Assert.AreEqual(cbor[String.Empty], cbor.AtJSONPointer("/"));
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

    public static void TestDateTimeTag1One(string str, long timeValue) {
      TestDateTimeTag1One(str, EInteger.FromInt64(timeValue));
    }

    public static void TestDateTimeTag1One(string str, EInteger ei) {
      CBORObject cbornum;
      cbornum = CBORObject.FromObjectAndTag(str, 0);
      var dtx = (DateTime)cbornum.ToObject(typeof(DateTime));
      ToObjectTest.TestToFromObjectRoundTrip(dtx);
      cbornum = CBORObject.FromObjectAndTag(ei, 1);
      var dtx2 = (DateTime)cbornum.ToObject(typeof(DateTime));
      ToObjectTest.TestToFromObjectRoundTrip(dtx2);
      TestCommon.AssertEqualsHashCode(dtx, dtx2);
      if (ei == null) {
        throw new ArgumentNullException(nameof(ei));
      }
      if (ei.CanFitInInt64()) {
        cbornum = CBORObject.FromObjectAndTag(ei.ToInt64Checked(), 1);
        dtx2 = (DateTime)cbornum.ToObject(typeof(DateTime));
        TestCommon.AssertEqualsHashCode(dtx, dtx2);
        ToObjectTest.TestToFromObjectRoundTrip(dtx2);
      }
      EFloat ef1 = EFloat.FromEInteger(ei).Plus(EContext.Binary64);
      EFloat ef2 = EFloat.FromEInteger(ei);
      if (ef1.CompareTo(ef2) == 0) {
        cbornum = CBORObject.FromObjectAndTag(ef1, 1);
        dtx2 = (DateTime)cbornum.ToObject(typeof(DateTime));
        TestCommon.AssertEqualsHashCode(dtx, dtx2);
        ToObjectTest.TestToFromObjectRoundTrip(dtx2);
        cbornum = CBORObject.FromObjectAndTag(ef1.ToDouble(), 1);
        dtx2 = (DateTime)cbornum.ToObject(typeof(DateTime));
        TestCommon.AssertEqualsHashCode(dtx, dtx2);
        ToObjectTest.TestToFromObjectRoundTrip(dtx2);
      }
    }

    public static void TestDateTimeTag1One(string str, double dbl) {
      CBORObject cbornum;
      cbornum = CBORObject.FromObjectAndTag(str, 0);
      var dtx = (DateTime)cbornum.ToObject(typeof(DateTime));
      ToObjectTest.TestToFromObjectRoundTrip(dtx);
      cbornum = CBORObject.FromObjectAndTag(dbl, 1);
      var dtx2 = (DateTime)cbornum.ToObject(typeof(DateTime));
      ToObjectTest.TestToFromObjectRoundTrip(dtx2);
      TestCommon.AssertEqualsHashCode(dtx, dtx2);
    }

    [Test]
    [Timeout(10000)]
    public void TestDateTimeTag1Specific1() {
      // Test speed
      EInteger ei = EInteger.FromString("-14261178672295354872");
      CBORObject cbornum = CBORObject.FromObjectAndTag(ei, 1);
      try {
        var dtx = (DateTime)cbornum.ToObject(typeof(DateTime));
        ToObjectTest.TestToFromObjectRoundTrip(dtx);
      } catch (CBORException) {
        Console.WriteLine("Not supported: " + ei);
      }
    }

    [Test]
    public void TestDateTimeSpecific2() {
      TestDateTimeTag1One("1758-09-28T23:25:24Z", -6666626076L);
      TestDateTimeTag1One("1758-09-28T23:25:24.000Z", -6666626076L);
      TestDateTimeTag1One("1758-09-28T23:25:24.500Z", -6666626075.5);
      TestDateTimeTag1One("2325-11-08T01:47:40Z", 11229587260L);
      TestDateTimeTag1One("2325-11-08T01:47:40.000Z", 11229587260L);
      TestDateTimeTag1One("2325-11-08T01:47:40.500Z", 11229587260.5);
      TestDateTimeTag1One("1787-03-04T10:21:24Z", -5769495516L);
      TestDateTimeTag1One("1787-03-04T10:21:24.000Z", -5769495516L);
      TestDateTimeTag1One("1787-03-04T10:21:24.500Z", -5769495515.5);
      TestDateTimeTag1One("1828-11-17T11:59:01Z", -4453358459L);
      TestDateTimeTag1One("1828-11-17T11:59:01.000Z", -4453358459L);
      TestDateTimeTag1One("1828-11-17T11:59:01.500Z", -4453358458.5);
      TestDateTimeTag1One("2379-01-22T01:20:02Z", 12908596802L);
      TestDateTimeTag1One("2379-01-22T01:20:02.000Z", 12908596802L);
      TestDateTimeTag1One("1699-05-31T22:37:24Z", -8538830556L);
      TestDateTimeTag1One("1699-05-31T22:37:24.000Z", -8538830556L);
      TestDateTimeTag1One("1699-05-31T22:37:24.500Z", -8538830555.5);
      TestDateTimeTag1One("2248-02-13T03:16:17Z", 8776523777L);
      TestDateTimeTag1One("2248-02-13T03:16:17.000Z", 8776523777L);
      TestDateTimeTag1One("2248-02-13T03:16:17.500Z", 8776523777.5);
      TestDateTimeTag1One("2136-04-15T16:45:29Z", 5247564329L);
      TestDateTimeTag1One("2136-04-15T16:45:29.000Z", 5247564329L);
      TestDateTimeTag1One("1889-09-05T00:23:45Z", -2534715375L);
      TestDateTimeTag1One("1889-09-05T00:23:45.000Z", -2534715375L);
      TestDateTimeTag1One("1889-09-05T00:23:45.500Z", -2534715374.5);
      TestDateTimeTag1One("2095-08-13T20:04:08Z", 3964104248L);
      TestDateTimeTag1One("2095-08-13T20:04:08.000Z", 3964104248L);
      TestDateTimeTag1One("2095-08-13T20:04:08.500Z", 3964104248.5);
      TestDateTimeTag1One("2475-03-27T17:41:48Z", 15943714908L);
      TestDateTimeTag1One("2475-03-27T17:41:48.000Z", 15943714908L);
      TestDateTimeTag1One("2475-03-27T17:41:48.500Z", 15943714908.5);
      TestDateTimeTag1One("1525-11-18T07:47:54Z", -14015088726L);
      TestDateTimeTag1One("1525-11-18T07:47:54.000Z", -14015088726L);
      TestDateTimeTag1One("2353-01-12T09:36:32Z", 12087308192L);
      TestDateTimeTag1One("2353-01-12T09:36:32.000Z", 12087308192L);
      TestDateTimeTag1One("2353-01-12T09:36:32.500Z", 12087308192.5);
      TestDateTimeTag1One("2218-11-29T08:23:31Z", 7854827011L);
      TestDateTimeTag1One("2218-11-29T08:23:31.000Z", 7854827011L);
      TestDateTimeTag1One("2377-08-21T09:44:12Z", 12863785452L);
      TestDateTimeTag1One("2377-08-21T09:44:12.000Z", 12863785452L);
      TestDateTimeTag1One("2377-08-21T09:44:12.500Z", 12863785452.5);
      TestDateTimeTag1One("1530-09-02T02:13:52Z", -13863995168L);
      TestDateTimeTag1One("1530-09-02T02:13:52.000Z", -13863995168L);
      TestDateTimeTag1One("1530-09-02T02:13:52.500Z", -13863995167.5);
      TestDateTimeTag1One("2319-03-11T18:18:48Z", 11019349128L);
      TestDateTimeTag1One("2319-03-11T18:18:48.000Z", 11019349128L);
      TestDateTimeTag1One("2319-03-11T18:18:48.500Z", 11019349128.5);
      TestDateTimeTag1One("1602-12-05T09:36:58Z", -11583699782L);
      TestDateTimeTag1One("1602-12-05T09:36:58.000Z", -11583699782L);
      TestDateTimeTag1One("1874-01-25T21:14:10Z", -3027293150L);
      TestDateTimeTag1One("1874-01-25T21:14:10.000Z", -3027293150L);
      TestDateTimeTag1One("1874-01-25T21:14:10.500Z", -3027293149.5);
      TestDateTimeTag1One("1996-02-26T04:09:49Z", 825307789L);
      TestDateTimeTag1One("1996-02-26T04:09:49.000Z", 825307789L);
      TestDateTimeTag1One("1996-02-26T04:09:49.500Z", 825307789.5);
      TestDateTimeTag1One("2113-11-27T22:16:09Z", 4541264169L);
      TestDateTimeTag1One("2113-11-27T22:16:09.000Z", 4541264169L);
      TestDateTimeTag1One("2113-11-27T22:16:09.500Z", 4541264169.5);
      TestDateTimeTag1One("1612-01-07T16:25:51Z", -11296827249L);
      TestDateTimeTag1One("1612-01-07T16:25:51.000Z", -11296827249L);
      TestDateTimeTag1One("1612-01-07T16:25:51.500Z", -11296827248.5);
      TestDateTimeTag1One("2077-12-08T22:15:00Z", 3406227300L);
      TestDateTimeTag1One("2077-12-08T22:15:00.000Z", 3406227300L);
      TestDateTimeTag1One("2077-12-08T22:15:00.500Z", 3406227300.5);
      TestDateTimeTag1One("1820-07-06T12:06:08Z", -4717396432L);
      TestDateTimeTag1One("1820-07-06T12:06:08.000Z", -4717396432L);
      TestDateTimeTag1One("1820-07-06T12:06:08.500Z", -4717396431.5);
      TestDateTimeTag1One("1724-01-17T16:42:20Z", -7761597460L);
      TestDateTimeTag1One("1724-01-17T16:42:20.000Z", -7761597460L);
      TestDateTimeTag1One("1724-01-17T16:42:20.500Z", -7761597459.5);
      TestDateTimeTag1One("2316-03-11T00:46:54Z", 10924678014L);
      TestDateTimeTag1One("2316-03-11T00:46:54.000Z", 10924678014L);
      TestDateTimeTag1One("2495-07-18T22:11:29Z", 16584646289L);
      TestDateTimeTag1One("2495-07-18T22:11:29.000Z", 16584646289L);
      TestDateTimeTag1One("2495-07-18T22:11:29.500Z", 16584646289.5);
      TestDateTimeTag1One("1874-04-25T08:52:46Z", -3019561634L);
      TestDateTimeTag1One("1874-04-25T08:52:46.000Z", -3019561634L);
      TestDateTimeTag1One("1874-04-25T08:52:46.500Z", -3019561633.5);
      TestDateTimeTag1One("2226-05-18T19:38:50Z", 8090480330L);
      TestDateTimeTag1One("2226-05-18T19:38:50.000Z", 8090480330L);
      TestDateTimeTag1One("2226-05-18T19:38:50.500Z", 8090480330.5);
      TestDateTimeTag1One("2108-06-26T09:01:48Z", 4370144508L);
      TestDateTimeTag1One("2108-06-26T09:01:48.000Z", 4370144508L);
      TestDateTimeTag1One("2108-06-26T09:01:48.500Z", 4370144508.5);
      TestDateTimeTag1One("1955-10-03T06:06:55Z", -449603585L);
      TestDateTimeTag1One("1955-10-03T06:06:55.000Z", -449603585L);
      TestDateTimeTag1One("1955-10-03T06:06:55.500Z", -449603584.5);
      TestDateTimeTag1One("1906-03-26T17:32:58Z", -2012365622L);
      TestDateTimeTag1One("1906-03-26T17:32:58.000Z", -2012365622L);
      TestDateTimeTag1One("1906-03-26T17:32:58.500Z", -2012365621.5);
      TestDateTimeTag1One("1592-03-10T03:46:03Z", -11922581637L);
      TestDateTimeTag1One("1592-03-10T03:46:03.000Z", -11922581637L);
      TestDateTimeTag1One("1592-03-10T03:46:03.500Z", -11922581636.5);
      TestDateTimeTag1One("2433-12-19T01:24:19Z", 14641349059L);
      TestDateTimeTag1One("2433-12-19T01:24:19.000Z", 14641349059L);
      TestDateTimeTag1One("2433-12-19T01:24:19.500Z", 14641349059.5);
      TestDateTimeTag1One("1802-02-07T09:43:23Z", -5298358597L);
      TestDateTimeTag1One("1802-02-07T09:43:23.000Z", -5298358597L);
      TestDateTimeTag1One("2318-04-11T20:11:23Z", 10990498283L);
      TestDateTimeTag1One("2318-04-11T20:11:23.000Z", 10990498283L);
      TestDateTimeTag1One("2318-04-11T20:11:23.500Z", 10990498283.5);
      TestDateTimeTag1One("2083-01-06T11:06:22Z", 3566459182L);
      TestDateTimeTag1One("2083-01-06T11:06:22.000Z", 3566459182L);
      TestDateTimeTag1One("2083-01-06T11:06:22.500Z", 3566459182.5);
      TestDateTimeTag1One("1561-08-16T19:31:48Z", -12887094492L);
      TestDateTimeTag1One("1561-08-16T19:31:48.000Z", -12887094492L);
      TestDateTimeTag1One("1561-08-16T19:31:48.500Z", -12887094491.5);
      TestDateTimeTag1One("2475-11-05T21:20:03Z", 15962995203L);
      TestDateTimeTag1One("2475-11-05T21:20:03.000Z", 15962995203L);
      TestDateTimeTag1One("2475-11-05T21:20:03.500Z", 15962995203.5);
      TestDateTimeTag1One("2209-05-13T09:31:56Z", 7553554316L);
      TestDateTimeTag1One("2209-05-13T09:31:56.000Z", 7553554316L);
      TestDateTimeTag1One("2209-05-13T09:31:56.500Z", 7553554316.5);
      TestDateTimeTag1One("1943-06-25T19:09:49Z", -836887811L);
      TestDateTimeTag1One("1943-06-25T19:09:49.000Z", -836887811L);
      TestDateTimeTag1One("1943-06-25T19:09:49.500Z", -836887810.5);
      TestDateTimeTag1One("1751-09-18T07:31:00Z", -6888472140L);
      TestDateTimeTag1One("1751-09-18T07:31:00.000Z", -6888472140L);
      TestDateTimeTag1One("1751-09-18T07:31:00.500Z", -6888472139.5);
      TestDateTimeTag1One("1538-05-07T23:40:25Z", -13621652375L);
      TestDateTimeTag1One("1538-05-07T23:40:25.000Z", -13621652375L);
      TestDateTimeTag1One("1538-05-07T23:40:25.500Z", -13621652374.5);
      TestDateTimeTag1One("1628-02-10T00:07:33Z", -10789026747L);
      TestDateTimeTag1One("1628-02-10T00:07:33.000Z", -10789026747L);
      TestDateTimeTag1One("1628-02-10T00:07:33.500Z", -10789026746.5);
      TestDateTimeTag1One("1584-08-23T09:30:49Z", -12160679351L);
      TestDateTimeTag1One("1584-08-23T09:30:49.000Z", -12160679351L);
      TestDateTimeTag1One("1584-08-23T09:30:49.500Z", -12160679350.5);
      TestDateTimeTag1One("2230-08-28T23:13:43Z", 8225536423L);
      TestDateTimeTag1One("2230-08-28T23:13:43.000Z", 8225536423L);
      TestDateTimeTag1One("1846-02-19T20:02:33Z", -3908750247L);
      TestDateTimeTag1One("1846-02-19T20:02:33.000Z", -3908750247L);
      TestDateTimeTag1One("1846-02-19T20:02:33.500Z", -3908750246.5);
      TestDateTimeTag1One("2114-07-28T00:06:13Z", 4562179573L);
      TestDateTimeTag1One("2114-07-28T00:06:13.000Z", 4562179573L);
      TestDateTimeTag1One("2114-07-28T00:06:13.500Z", 4562179573.5);
      TestDateTimeTag1One("1855-04-03T15:29:33Z", -3621054627L);
      TestDateTimeTag1One("1855-04-03T15:29:33.000Z", -3621054627L);
      TestDateTimeTag1One("1855-04-03T15:29:33.500Z", -3621054626.5);
      TestDateTimeTag1One("1532-02-04T13:08:22Z", -13819027898L);
      TestDateTimeTag1One("1532-02-04T13:08:22.000Z", -13819027898L);
      TestDateTimeTag1One("2285-12-28T16:35:29Z", 9971742929L);
      TestDateTimeTag1One("2285-12-28T16:35:29.000Z", 9971742929L);
      TestDateTimeTag1One("2285-12-28T16:35:29.500Z", 9971742929.5);
      TestDateTimeTag1One("1784-08-08T15:25:01Z", -5850520499L);
      TestDateTimeTag1One("1784-08-08T15:25:01.000Z", -5850520499L);
      TestDateTimeTag1One("2190-06-25T10:55:10Z", 6957744910L);
      TestDateTimeTag1One("2190-06-25T10:55:10.000Z", 6957744910L);
      TestDateTimeTag1One("2190-06-25T10:55:10.500Z", 6957744910.5);
      TestDateTimeTag1One("2263-10-08T20:28:28Z", 9270448108L);
      TestDateTimeTag1One("2263-10-08T20:28:28.000Z", 9270448108L);
      TestDateTimeTag1One("2263-10-08T20:28:28.500Z", 9270448108.5);
      TestDateTimeTag1One("2036-05-12T10:02:45Z", 2094199365L);
      TestDateTimeTag1One("2036-05-12T10:02:45.000Z", 2094199365L);
      TestDateTimeTag1One("2036-05-12T10:02:45.500Z", 2094199365.5);
      TestDateTimeTag1One("2166-09-08T09:25:14Z", 6206837114L);
      TestDateTimeTag1One("2166-09-08T09:25:14.000Z", 6206837114L);
      TestDateTimeTag1One("2166-09-08T09:25:14.500Z", 6206837114.5);
      TestDateTimeTag1One("1698-12-30T18:31:11Z", -8551978129L);
      TestDateTimeTag1One("1698-12-30T18:31:11.000Z", -8551978129L);
      TestDateTimeTag1One("1780-10-16T15:02:56Z", -5970790624L);
      TestDateTimeTag1One("1780-10-16T15:02:56.000Z", -5970790624L);
      TestDateTimeTag1One("1780-10-16T15:02:56.500Z", -5970790623.5);
      TestDateTimeTag1One("1710-10-12T20:07:58Z", -8180193122L);
      TestDateTimeTag1One("1710-10-12T20:07:58.000Z", -8180193122L);
      TestDateTimeTag1One("1710-10-12T20:07:58.500Z", -8180193121.5);
      TestDateTimeTag1One("2034-09-28T04:45:04Z", 2043031504L);
      TestDateTimeTag1One("2034-09-28T04:45:04.000Z", 2043031504L);
      TestDateTimeTag1One("2034-09-28T04:45:04.500Z", 2043031504.5);
      TestDateTimeTag1One("1801-12-10T15:45:47Z", -5303434453L);
      TestDateTimeTag1One("1801-12-10T15:45:47.000Z", -5303434453L);
      TestDateTimeTag1One("1537-08-24T13:13:09Z", -13643808411L);
      TestDateTimeTag1One("1537-08-24T13:13:09.000Z", -13643808411L);
      TestDateTimeTag1One("1537-08-24T13:13:09.500Z", -13643808410.5);
      TestDateTimeTag1One("2249-09-24T21:07:14Z", 8827477634L);
      TestDateTimeTag1One("2249-09-24T21:07:14.000Z", 8827477634L);
      TestDateTimeTag1One("2249-09-24T21:07:14.500Z", 8827477634.5);
      TestDateTimeTag1One("2137-11-27T05:22:38Z", 5298585758L);
      TestDateTimeTag1One("2137-11-27T05:22:38.000Z", 5298585758L);
      TestDateTimeTag1One("2137-11-27T05:22:38.500Z", 5298585758.5);
      TestDateTimeTag1One("2123-07-31T13:09:34Z", 4846482574L);
      TestDateTimeTag1One("2123-07-31T13:09:34.000Z", 4846482574L);
      TestDateTimeTag1One("2123-07-31T13:09:34.500Z", 4846482574.5);
      TestDateTimeTag1One("2242-01-31T12:14:20Z", 8586130460L);
      TestDateTimeTag1One("2242-01-31T12:14:20.000Z", 8586130460L);
      TestDateTimeTag1One("2242-01-31T12:14:20.500Z", 8586130460.5);
      TestDateTimeTag1One("2232-11-04T21:12:33Z", 8294562753L);
      TestDateTimeTag1One("2232-11-04T21:12:33.000Z", 8294562753L);
      TestDateTimeTag1One("1590-12-06T04:30:48Z", -11962322952L);
      TestDateTimeTag1One("1590-12-06T04:30:48.000Z", -11962322952L);
      TestDateTimeTag1One("1590-12-06T04:30:48.500Z", -11962322951.5);
      TestDateTimeTag1One("1910-05-16T17:54:04Z", -1881727556L);
      TestDateTimeTag1One("1910-05-16T17:54:04.000Z", -1881727556L);
      TestDateTimeTag1One("1910-05-16T17:54:04.500Z", -1881727555.5);
      TestDateTimeTag1One("2482-06-15T23:28:00Z", 16171572480L);
      TestDateTimeTag1One("2482-06-15T23:28:00.000Z", 16171572480L);
      TestDateTimeTag1One("2482-06-15T23:28:00.500Z", 16171572480.5);
      TestDateTimeTag1One("1808-01-17T13:11:23Z", -5110858117L);
      TestDateTimeTag1One("1808-01-17T13:11:23.000Z", -5110858117L);
      TestDateTimeTag1One("1872-05-04T12:15:05Z", -3081843895L);
      TestDateTimeTag1One("1872-05-04T12:15:05.000Z", -3081843895L);
      TestDateTimeTag1One("1872-05-04T12:15:05.500Z", -3081843894.5);
      TestDateTimeTag1One("1719-05-18T16:44:33Z", -7908909327L);
      TestDateTimeTag1One("1719-05-18T16:44:33.000Z", -7908909327L);
      TestDateTimeTag1One("2137-05-26T02:17:32Z", 5282590652L);
      TestDateTimeTag1One("2137-05-26T02:17:32.000Z", 5282590652L);
      TestDateTimeTag1One("2137-05-26T02:17:32.500Z", 5282590652.5);
      TestDateTimeTag1One("1714-06-15T13:41:14Z", -8064267526L);
      TestDateTimeTag1One("1714-06-15T13:41:14.000Z", -8064267526L);
      TestDateTimeTag1One("1714-06-15T13:41:14.500Z", -8064267525.5);
      TestDateTimeTag1One("1878-12-03T20:14:03Z", -2874109557L);
      TestDateTimeTag1One("1878-12-03T20:14:03.000Z", -2874109557L);
      TestDateTimeTag1One("1878-12-03T20:14:03.500Z", -2874109556.5);
      TestDateTimeTag1One("2190-11-26T23:45:55Z", 6971096755L);
      TestDateTimeTag1One("2190-11-26T23:45:55.000Z", 6971096755L);
      TestDateTimeTag1One("2020-01-22T15:58:52Z", 1579708732L);
      TestDateTimeTag1One("2020-01-22T15:58:52.000Z", 1579708732L);
      TestDateTimeTag1One("2020-01-22T15:58:52.500Z", 1579708732.5);
      TestDateTimeTag1One("2245-10-06T15:40:51Z", 8702264451L);
      TestDateTimeTag1One("2245-10-06T15:40:51.000Z", 8702264451L);
      TestDateTimeTag1One("2245-10-06T15:40:51.500Z", 8702264451.5);
      TestDateTimeTag1One("1647-08-10T21:26:16Z", -10173695624L);
      TestDateTimeTag1One("1647-08-10T21:26:16.000Z", -10173695624L);
      TestDateTimeTag1One("1647-08-10T21:26:16.500Z", -10173695623.5);
      TestDateTimeTag1One("1628-11-10T01:03:36Z", -10765349784L);
      TestDateTimeTag1One("1628-11-10T01:03:36.000Z", -10765349784L);
      TestDateTimeTag1One("1628-11-10T01:03:36.500Z", -10765349783.5);
      TestDateTimeTag1One("2359-11-30T16:24:04Z", 12304455844L);
      TestDateTimeTag1One("2359-11-30T16:24:04.000Z", 12304455844L);
      TestDateTimeTag1One("2359-11-30T16:24:04.500Z", 12304455844.5);
      TestDateTimeTag1One("1833-10-12T18:44:22Z", -4298678138L);
      TestDateTimeTag1One("1833-10-12T18:44:22.000Z", -4298678138L);
      TestDateTimeTag1One("1833-10-12T18:44:22.500Z", -4298678137.5);
      TestDateTimeTag1One("1550-07-27T20:11:15Z", -13235975325L);
      TestDateTimeTag1One("1550-07-27T20:11:15.000Z", -13235975325L);
      TestDateTimeTag1One("1550-07-27T20:11:15.500Z", -13235975324.5);
      TestDateTimeTag1One("2376-11-23T23:17:49Z", 12840419869L);
      TestDateTimeTag1One("2376-11-23T23:17:49.000Z", 12840419869L);
      TestDateTimeTag1One("2376-11-23T23:17:49.500Z", 12840419869.5);
      TestDateTimeTag1One("2291-11-16T10:53:45Z", 10157396025L);
      TestDateTimeTag1One("2291-11-16T10:53:45.000Z", 10157396025L);
      TestDateTimeTag1One("2291-11-16T10:53:45.500Z", 10157396025.5);
      TestDateTimeTag1One("2349-11-15T11:45:50Z", 11987610350L);
      TestDateTimeTag1One("2349-11-15T11:45:50.000Z", 11987610350L);
      TestDateTimeTag1One("2059-05-22T21:03:13Z", 2820862993L);
      TestDateTimeTag1One("2059-05-22T21:03:13.000Z", 2820862993L);
      TestDateTimeTag1One("2059-05-22T21:03:13.500Z", 2820862993.5);
      TestDateTimeTag1One("1601-04-03T01:34:37Z", -11636519123L);
      TestDateTimeTag1One("1601-04-03T01:34:37.000Z", -11636519123L);
      TestDateTimeTag1One("1601-04-03T01:34:37.500Z", -11636519122.5);
      TestDateTimeTag1One("1853-11-01T19:05:56Z", -3665796844L);
      TestDateTimeTag1One("1853-11-01T19:05:56.000Z", -3665796844L);
      TestDateTimeTag1One("1853-11-01T19:05:56.500Z", -3665796843.5);
      TestDateTimeTag1One("2465-03-10T00:10:34Z", 15626650234L);
      TestDateTimeTag1One("2465-03-10T00:10:34.000Z", 15626650234L);
      TestDateTimeTag1One("1961-06-28T14:59:41Z", -268563619L);
      TestDateTimeTag1One("1961-06-28T14:59:41.000Z", -268563619L);
      TestDateTimeTag1One("1961-06-28T14:59:41.500Z", -268563618.5);
      TestDateTimeTag1One("2078-02-03T01:57:23Z", 3411079043L);
      TestDateTimeTag1One("2078-02-03T01:57:23.000Z", 3411079043L);
      TestDateTimeTag1One("2078-02-03T01:57:23.500Z", 3411079043.5);
      TestDateTimeTag1One("2325-11-05T11:53:57Z", 11229364437L);
      TestDateTimeTag1One("2325-11-05T11:53:57.000Z", 11229364437L);
      TestDateTimeTag1One("2325-11-05T11:53:57.500Z", 11229364437.5);
      TestDateTimeTag1One("2189-02-10T04:55:14Z", 6914523314L);
      TestDateTimeTag1One("2189-02-10T04:55:14.000Z", 6914523314L);
      TestDateTimeTag1One("2189-02-10T04:55:14.500Z", 6914523314.5);
      TestDateTimeTag1One("2416-04-20T21:48:33Z", 14083969713L);
      TestDateTimeTag1One("2416-04-20T21:48:33.000Z", 14083969713L);
      TestDateTimeTag1One("2416-04-20T21:48:33.500Z", 14083969713.5);
      TestDateTimeTag1One("2009-06-24T20:06:34Z", 1245873994L);
      TestDateTimeTag1One("2009-06-24T20:06:34.000Z", 1245873994L);
      TestDateTimeTag1One("2009-06-24T20:06:34.500Z", 1245873994.5);
      TestDateTimeTag1One("2488-05-20T22:56:10Z", 16358712970L);
      TestDateTimeTag1One("2488-05-20T22:56:10.000Z", 16358712970L);
      TestDateTimeTag1One("1519-07-05T21:55:20Z", -14216177080L);
      TestDateTimeTag1One("1519-07-05T21:55:20.000Z", -14216177080L);
      TestDateTimeTag1One("1519-07-05T21:55:20.500Z", -14216177079.5);
      TestDateTimeTag1One("2349-05-25T11:44:14Z", 11972576654L);
      TestDateTimeTag1One("2349-05-25T11:44:14.000Z", 11972576654L);
    }

    [Test]
    [Timeout(100000)]
    public void TestDateTimeTag1() {
      CBORObject cbornum;
      var rg = new RandomGenerator();
      DateTime dt, dt2;
      for (var i = 0; i < 1000; ++i) {
        EInteger ei = CBORTestCommon.RandomEIntegerMajorType0Or1(rg);
        cbornum = CBORObject.FromObjectAndTag(ei, 1);
        try {
          var dtx = (DateTime)cbornum.ToObject(typeof(DateTime));
          ToObjectTest.TestToFromObjectRoundTrip(dtx);
        } catch (CBORException) {
          // Console.WriteLine("Not supported: "+ei);
        }
      }
      for (var i = 0; i < 1000; ++i) {
        double dbl = RandomObjects.RandomFiniteDouble(rg);
        cbornum = CBORObject.FromObjectAndTag(dbl, 1);
        try {
          var dtx = (DateTime)cbornum.ToObject(typeof(DateTime));
          ToObjectTest.TestToFromObjectRoundTrip(dtx);
        } catch (CBORException) {
          // Console.WriteLine("Not supported: "+dbl);
        }
      }
      string dateStr = "1970-01-01T00:00:00.000Z";
      CBORObject cbor = CBORObject.FromObjectAndTag(dateStr, 0);
      dt = (DateTime)cbor.ToObject(typeof(DateTime));
      CBORObject cbor2 = CBORObject.FromObjectAndTag(0, 1);
      dt2 = (DateTime)cbor.ToObject(typeof(DateTime));
      Assert.AreEqual(dt2, dt);
    }

    private static string RandomQueryStringLike(IRandomGenExtended irg) {
      var sb = new StringBuilder();
      while (true) {
        int x = irg.GetInt32(100);
        if (x == 0) {
          break;
        } else if (x < 10) {
          sb.Append('&');
        } else if (x < 20) {
          sb.Append('=');
        } else if (x < 25) {
          string hex = "0123456789ABCDEF";
          sb.Append('%');
          sb.Append(hex[irg.GetInt32(hex.Length)]);
          sb.Append(hex[irg.GetInt32(hex.Length)]);
        } else if (x < 30) {
          string hex = "0123456789abcdef";
          sb.Append('%');
          sb.Append(hex[irg.GetInt32(hex.Length)]);
          sb.Append(hex[irg.GetInt32(hex.Length)]);
        } else if (x < 95) {
          sb.Append((char)(irg.GetInt32(0x5e) + 0x21));
        } else {
          sb.Append((char)irg.GetInt32(0x80));
        }
      }
      return sb.ToString();
    }

    [Test]
    public void TestQueryStrings() {
      // TODO: Add utility to create query strings
      String test = "a=b&c=d&e=f&g\u005b0]=h&g\u005b1]=j&g\u005b2]\u005b";
      test += "a]=k&g\u005b2]\u005bb]=m";
      CBORObject cbor =
        CBORObject.FromObject(QueryStringHelper.QueryStringToDict(test));
      Console.WriteLine(cbor.ToJSONString());
      cbor = CBORObject.FromObject(QueryStringHelper.QueryStringToCBOR(test));
      Console.WriteLine(cbor.ToJSONString());
      var rg = new RandomGenerator();
      for (var i = 0; i < 100000; ++i) {
        string str = RandomQueryStringLike(rg);
        try {
          cbor = QueryStringHelper.QueryStringToCBOR(str);
          // Console.WriteLine("succ: " + str);
          // Console.WriteLine(cbor.ToJSONString());
        } catch (InvalidOperationException) {
          // Console.WriteLine("throws: "+str);
        }
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

    public static void AssertJSONDouble(
      string json,
      string numconv,
      double dbl) {
      var opt = new JSONOptions("numberconversion=" + numconv);
      CBORObject[] cbors = {
        FromJSON(json, numconv),
        CBORDataUtilities.ParseJSONNumber(json, opt),
      };
      foreach (CBORObject cbor in cbors) {
        if (cbor.Type != CBORType.FloatingPoint) {
          Assert.AreEqual(
            CBORType.FloatingPoint,
            cbor.Type,
            json + " " + numconv + " " + dbl);
        }
        double cbordbl = cbor.AsDoubleValue();
        if (dbl != cbordbl) {
          Assert.Fail("dbl = " + dbl + ", cbordbl = " + cbordbl + ", " +
            json + " " + numconv + " " + dbl);
        }
      }
    }

    public static void AssertJSONInteger(
      string json,
      string numconv,
      long longval) {
      var opt = new JSONOptions("numberconversion=" + numconv);
      CBORObject[] cbors = {
        FromJSON(json, numconv),
        CBORDataUtilities.ParseJSONNumber(json, opt),
      };
      foreach (CBORObject cbor in cbors) {
        if (cbor.Type != CBORType.Integer) {
          string msg = json + " " + numconv + " " + longval;
          msg = msg.Substring(0, Math.Min(100, msg.Length));
          if (msg.Length > 100) {
            msg += "...";
          }
          Assert.AreEqual(CBORType.Integer, cbor.Type, msg);
        }
        Assert.AreEqual(longval, cbor.AsInt64Value());
      }
    }

    public static void AssertJSONInteger(
      string json,
      string numconv,
      int intval) {
      var opt = new JSONOptions("numberconversion=" + numconv);
      CBORObject[] cbors = {
        FromJSON(json, numconv),
        CBORDataUtilities.ParseJSONNumber(json, opt),
      };
      foreach (CBORObject cbor in cbors) {
        if (cbor.Type != CBORType.Integer) {
          string msg = json + " " + numconv + " " + intval;
          msg = msg.Substring(0, Math.Min(100, msg.Length));
          if (msg.Length > 100) {
            msg += "...";
          }
          Assert.AreEqual(CBORType.Integer, cbor.Type, msg);
        }
        Assert.AreEqual(intval, cbor.AsInt32Value());
      }
    }

    [Test]
    [Timeout(10000)]
    public void TestFromJsonStringLongSpecific1() {
      JSONOptions jsonop = JSONOptions.Default;
      string json = "{\"x\":-9.2574033594381E-7962\u002c\"1\":" +
        "-2.8131427974929237E+240}";
      try {
        FromJSON(json, jsonop);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    private static readonly JSONOptions JSONOptionsDouble = new JSONOptions(
      "numberconversion=double");
    private static readonly JSONOptions JSONOptionsFull = new JSONOptions(
      "numberconversion=full");

    public static void TestParseNumberFxxLine(string line) {
      // Parse test case format used in:
      // https://github.com/nigeltao/parse-number-fxx-test-data
      string f16 = line.Substring(0, 4);
      if (line[4] != ' ') {
        Assert.Fail(line);
      }
      string f32 = line.Substring(4 + 1, 8);
      if (line[4 + 9] != ' ') {
        Assert.Fail(line);
      }
      string f64 = line.Substring(4 + 1 + 8 + 1, 16);
      if (line[4 + 26] != ' ') {
        Assert.Fail(line);
      }
      string str = line.Substring(4 + 1 + 8 + 1 + 16 + 1);
      short sf16 = EInteger.FromRadixString(f16, 16).ToInt16Unchecked();
      int sf32 = EInteger.FromRadixString(f32, 16).ToInt32Unchecked();
      long sf64 = EInteger.FromRadixString(f64, 16).ToInt64Unchecked();
      TestParseNumberFxx(str, sf16, sf32, sf64, line);
    }

    public static void TestParseNumberFxx(
      string str,
      short f16,
      int f32,
      long f64,
      string line) {
      if (str[0] == '.' || str[str.Length - 1] == '.' ||
        str.Contains(".e") || str.Contains(".E")) {
        // Not a valid JSON number, so skip
        // Console.WriteLine(str);
        return;
      }
      if (CBORObject.FromObject(f16) == null) {
        Assert.Fail();
      }
      CBORObject cbor = CBORDataUtilities.ParseJSONNumber(str,
          JSONOptionsDouble);
      if (cbor == null) {
        Console.WriteLine(str);
        return;
      }
      Assert.AreEqual(f64, cbor.AsDoubleBits(), line);
      cbor = CBORObject.FromJSONString(str, JSONOptionsDouble);
      Assert.AreEqual(f64, cbor.AsDoubleBits(), line);
      cbor = CBORObject.FromJSONBytes(
          DataUtilities.GetUtf8Bytes(str, false),
          JSONOptionsDouble);
      Assert.AreEqual(f64, cbor.AsDoubleBits(), line);
      float sing = CBORObject.FromFloatingPointBits(f32, 4).AsSingle();
      cbor = CBORDataUtilities.ParseJSONNumber(str, JSONOptionsFull);
      if (cbor == null) {
        Assert.Fail();
      }
      Assert.AreEqual(sing, cbor.AsSingle(), line);
      cbor = CBORObject.FromJSONString(str, JSONOptionsFull);
      Assert.AreEqual(sing, cbor.AsSingle(), line);
      cbor = CBORObject.FromJSONBytes(
          DataUtilities.GetUtf8Bytes(str, false),
          JSONOptionsFull);
      Assert.AreEqual(sing, cbor.AsSingle(), line);
      // TODO: Test f16
    }

    [Test]
    public void TestCloseToPowerOfTwo() {
      for (var i = 31; i < 129; ++i) {
        EInteger ei = EInteger.FromInt32(1).ShiftLeft(i);
        {
          AssertJSONDouble(
            ei.ToString(),
            "double",
            EFloat.FromEInteger(ei).ToDouble());
          AssertJSONDouble(
            ei.Add(1).ToString(),
            "double",
            EFloat.FromEInteger(ei.Add(1)).ToDouble());
          AssertJSONDouble(
            ei.Subtract(2).ToString(),
            "double",
            EFloat.FromEInteger(ei.Subtract(2)).ToDouble());
          AssertJSONDouble(
            ei.Add(2).ToString(),
            "double",
            EFloat.FromEInteger(ei.Add(2)).ToDouble());
          AssertJSONDouble(
            ei.Subtract(2).ToString(),
            "double",
            EFloat.FromEInteger(ei.Subtract(2)).ToDouble());
        }
      }
    }

    [Test]
    public void TestFromJsonStringFastCases() {
      var op = new JSONOptions("numberconversion=double");
      Assert.AreEqual(
        JSONOptions.ConversionMode.Double,
        op.NumberConversion);
      op = new JSONOptions("numberconversion=intorfloat");
      Assert.AreEqual(
        JSONOptions.ConversionMode.IntOrFloat,
        op.NumberConversion);
      string manyzeros = TestCommon.Repeat("0", 1000000);
      string manythrees = TestCommon.Repeat("3", 1000000);
      AssertJSONDouble(
        "0e-" + manyzeros,
        "double",
        0.0);

      AssertJSONDouble(
        "0." + manyzeros,
        "double",
        0.0);

      AssertJSONDouble(
        "0." + manyzeros + "e-9999999999999",
        "double",
        0.0);

      AssertJSONDouble(
        manythrees + "e-9999999999999",
        "double",
        0.0);

      AssertJSONDouble(
        manythrees + "e-9999999999999",
        "intorfloat",
        0.0);

      AssertJSONInteger(
        manythrees + "e-9999999999999",
        "intorfloatfromdouble",
        0);

      AssertJSONDouble(
        "0." + manyzeros + "e-99999999",
        "double",
        0.0);

      AssertJSONDouble(
        manythrees + "e-99999999",
        "double",
        0.0);

      AssertJSONDouble(
        manythrees + "e-99999999",
        "intorfloat",
        0.0);
      AssertJSONInteger(
        manythrees + "e-99999999",
        "intorfloatfromdouble",
        0);
      AssertJSONInteger(
        "0e-" + manyzeros,
        "intorfloat",
        0);
      AssertJSONInteger(
        "0e-" + manyzeros,
        "intorfloatfromdouble",
        0);
      AssertJSONInteger(
        "-0e-" + manyzeros,
        "intorfloat",
        0);
      AssertJSONInteger(
        "-0e-" + manyzeros,
        "intorfloatfromdouble",
        0);
      AssertJSONInteger(
        "0." + manyzeros,
        "intorfloat",
        0);
      AssertJSONInteger(
        "0." + manyzeros,
        "intorfloatfromdouble",
        0);
      AssertJSONInteger(
        "-0." + manyzeros,
        "intorfloat",
        0);
      AssertJSONInteger(
        "-0." + manyzeros,
        "intorfloatfromdouble",
        0);
    }

    [Test]
    public void TestFromJsonStringFiniteDoubleSpec() {
      var rg = new RandomGenerator();
      for (var i = 0; i < 10000; ++i) {
        double dbl = RandomObjects.RandomFiniteDouble(rg);
        EFloat efd = EFloat.FromDouble(dbl);
        AssertJSONDouble(
          efd.ToShortestString(EContext.Binary64),
          "double",
          dbl);
        AssertJSONDouble(
          efd.ToString(),
          "double",
          dbl);
      }
    }

    [Test]
    public void TestEDecimalEFloatWithHighExponent() {
      string decstr = "0E100441809235791722330759976";
      Assert.AreEqual(0L, EDecimal.FromString(decstr).ToDoubleBits());
      Assert.AreEqual(0L, EFloat.FromString(decstr).ToDoubleBits());
      {
        object objectTemp = 0L;
        object objectTemp2 = EDecimal.FromString(decstr,
            EContext.Decimal32).ToDoubleBits();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = 0L;
        object objectTemp2 = EFloat.FromString(decstr,
            EContext.Binary64).ToDoubleBits();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      decstr = "0E-100441809235791722330759976";
      Assert.AreEqual(0L, EDecimal.FromString(decstr).ToDoubleBits());
      Assert.AreEqual(0L, EFloat.FromString(decstr).ToDoubleBits());
      {
        object objectTemp = 0L;
        object objectTemp2 = EDecimal.FromString(decstr,
            EContext.Decimal32).ToDoubleBits();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = 0L;
        object objectTemp2 = EFloat.FromString(decstr,
            EContext.Binary64).ToDoubleBits();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      decstr = "-0E100441809235791722330759976";
      long negzero = 1L << 63;
      Assert.AreEqual(negzero, EDecimal.FromString(decstr).ToDoubleBits());
      Assert.AreEqual(negzero, EFloat.FromString(decstr).ToDoubleBits());
      {
        object objectTemp = negzero;
        object objectTemp2 = EDecimal.FromString(decstr,
            EContext.Decimal32).ToDoubleBits();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = negzero;
        object objectTemp2 = EFloat.FromString(decstr,
            EContext.Binary64).ToDoubleBits();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      decstr = "-0E-100441809235791722330759976";
      Assert.AreEqual(negzero, EDecimal.FromString(decstr).ToDoubleBits());
      Assert.AreEqual(negzero, EFloat.FromString(decstr).ToDoubleBits());
      {
        object objectTemp = negzero;
        object objectTemp2 = EDecimal.FromString(decstr,
            EContext.Decimal32).ToDoubleBits();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = negzero;
        object objectTemp2 = EFloat.FromString(decstr,
            EContext.Binary64).ToDoubleBits();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
    }

    [Test]
    public void TestFromJsonStringZeroWithHighExponent() {
      string decstr = "0E100441809235791722330759976";
      EDecimal ed = EDecimal.FromString(decstr);
      double dbl = ed.ToDouble();
      Assert.AreEqual(0.0d, dbl);
      AssertJSONDouble(decstr, "double", dbl);
      AssertJSONInteger(decstr, "intorfloat", 0);
      decstr = "0E1321909565013040040586";
      ed = EDecimal.FromString(decstr);
      dbl = ed.ToDouble();
      Assert.AreEqual(0.0d, dbl);
      AssertJSONDouble(decstr, "double", dbl);
      AssertJSONInteger(decstr, "intorfloat", 0);
      double dblnegzero = EFloat.FromString("-0").ToDouble();
      AssertJSONDouble("0E-1321909565013040040586", "double", 0.0);
      AssertJSONInteger("0E-1321909565013040040586", "intorfloat", 0);
      AssertJSONDouble("-0E1321909565013040040586", "double", dblnegzero);
      AssertJSONInteger("-0E1321909565013040040586", "intorfloat", 0);
      AssertJSONDouble("-0E-1321909565013040040586", "double", dblnegzero);
      AssertJSONInteger("-0E-1321909565013040040586", "intorfloat", 0);

      AssertJSONDouble("0E-100441809235791722330759976", "double", 0.0);
      AssertJSONInteger("0E-100441809235791722330759976", "intorfloat", 0);
      AssertJSONDouble("-0E100441809235791722330759976", "double", dblnegzero);
      AssertJSONInteger("-0E100441809235791722330759976", "intorfloat", 0);
      AssertJSONDouble("-0E-100441809235791722330759976", "double", dblnegzero);
      AssertJSONInteger("-0E-100441809235791722330759976", "intorfloat", 0);
    }

    [Test]
    public void TestFromJsonStringEDecimalSpec() {
      var rg = new RandomGenerator();
      for (var i = 0; i < 2000; ++i) {
        var decstring = new string[1];
        EDecimal ed = RandomObjects.RandomEDecimal(rg, decstring);
        if (decstring[0] == null) {
          Assert.Fail();
        }
        double dbl = ed.ToDouble();
        if (Double.IsPositiveInfinity(dbl) ||
                 Double.IsNegativeInfinity(dbl) ||
                 Double.IsNaN(dbl)) {
          continue;
        }
        AssertJSONDouble(
          decstring[0],
          "double",
          dbl);
      }
    }

    [Test]
    public void TestFromJsonCTLInString() {
      for (var i = 0; i <= 0x20; ++i) {
        byte[] bytes = { 0x22, (byte)i, 0x22 };
        char[] chars = { (char)0x22, (char)i, (char)0x22 };
        string str = new String(chars, 0, chars.Length);
        if (i == 0x20) {
          try {
            CBORObject.FromJSONString(str);
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            CBORObject.FromJSONBytes(bytes);
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        } else {
          try {
            CBORObject.FromJSONString(str);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
          try {
            CBORObject.FromJSONBytes(bytes);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      }
    }

    // [Test]
    public void TestFromJsonLeadingTrailingCTLBytes() {
      // TODO: Reenable eventually, once UTF-8 only support
      // for CBORObject.FromJSONBytes is implemented
      for (var i = 0; i <= 0x20; ++i) {
        // Leading CTL
        byte[] bytes = { (byte)i, 0x31 };
        if (i == 0x09 || i == 0x0d || i == 0x0a || i == 0x20) {
          try {
            CBORObject.FromJSONBytes(bytes);
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + "bytes " + i);
            throw new InvalidOperationException(String.Empty, ex);
          }
        } else {
          try {
            CBORObject.FromJSONBytes(bytes);
            Assert.Fail("Should have failed bytes " + i);
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        // Trailing CTL
        bytes = new byte[] { 0x31, (byte)i };
        if (i == 0x09 || i == 0x0d || i == 0x0a || i == 0x20) {
          try {
            CBORObject.FromJSONBytes(bytes);
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + "bytes " + i);
            throw new InvalidOperationException(String.Empty, ex);
          }
        } else {
          try {
            CBORObject.FromJSONBytes(bytes);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + "bytes " + i);
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      }
    }

    [Test]
    public void TestFromJsonLeadingTrailingCTL() {
      for (var i = 0; i <= 0x20; ++i) {
        // Leading CTL
        char[] chars = { (char)i, (char)0x31 };
        string str = new String(chars, 0, chars.Length);
        if (i == 0x09 || i == 0x0d || i == 0x0a || i == 0x20) {
          try {
            CBORObject.FromJSONString(str);
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + "string " + i);
            throw new InvalidOperationException(String.Empty, ex);
          }
        } else {
          try {
            CBORObject.FromJSONString(str);
            Assert.Fail("Should have failed string " + i);
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        // Trailing CTL
        chars = new char[] { (char)0x31, (char)i };
        str = new String(chars, 0, chars.Length);
        if (i == 0x09 || i == 0x0d || i == 0x0a || i == 0x20) {
          try {
            CBORObject.FromJSONString(str);
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + "string " + i);
            throw new InvalidOperationException(String.Empty, ex);
          }
        } else {
          try {
            CBORObject.FromJSONString(str);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + "string " + i);
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      }
    }

    [Test]
    public void TestFromJsonStringSmallDoubleSpec() {
      var rg = new RandomGenerator();
      for (var i = 0; i < 10000; ++i) {
        int rv = rg.GetInt32(Int32.MaxValue) * ((rg.GetInt32(2) * 2) - 1);
        string rvstring = TestCommon.IntToString(rv);
        AssertJSONDouble(
          rvstring,
          "double",
          (double)rv);
        AssertJSONInteger(
          rvstring,
          "intorfloat",
          rv);
      }
      AssertJSONDouble("511", "double", 511);
      AssertJSONDouble("-511", "double", -511);
      AssertJSONDouble(
        TestCommon.IntToString(Int32.MaxValue),
        "double",
        (double)Int32.MaxValue);
      AssertJSONDouble(
        TestCommon.IntToString(Int32.MaxValue),
        "double",
        (double)Int32.MaxValue);
      AssertJSONDouble(
        TestCommon.IntToString(Int32.MinValue),
        "double",
        (double)Int32.MinValue);
    }

    [Test]
    [Timeout(10000)]
    public void TestFromJsonStringSmallDouble() {
      CBORObject cbor;
      AssertJSONDouble("0", "double", 0.0);
      cbor = FromJSON("[0, 1, 2, 3]", "double");
      Assert.AreEqual(4, cbor.Count);
      Assert.AreEqual((double)0.0, cbor[0].AsDouble());
      Assert.AreEqual((double)1.0, cbor[1].AsDouble());
      Assert.AreEqual((double)2.0, cbor[2].AsDouble());
      Assert.AreEqual((double)3.0, cbor[3].AsDouble());
      cbor = FromJSON("[0]", "double");
      Assert.AreEqual(1, cbor.Count);
      Assert.AreEqual((double)0.0, cbor[0].AsDouble());
      cbor = FromJSON("[-0]", "double");
      Assert.AreEqual(1, cbor.Count);
      cbor = FromJSON("[1]", "double");
      Assert.AreEqual(1, cbor.Count);
      Assert.AreEqual((double)1.0, cbor[0].AsDouble());
      cbor = FromJSON("[-1]", "double");
      Assert.AreEqual(1, cbor.Count);
      Assert.AreEqual((double)-1.0, cbor[0].AsDouble());
      cbor = FromJSON("[-1022,-1023,-1024,-1025,1022,1023,1024,1025]",
          "double");
      Assert.AreEqual(8, cbor.Count);
      Assert.AreEqual((double)-1022.0, cbor[0].AsDouble());
      Assert.AreEqual((double)-1023.0, cbor[1].AsDouble());
      Assert.AreEqual((double)-1024.0, cbor[2].AsDouble());
      Assert.AreEqual((double)-1025.0, cbor[3].AsDouble());
      Assert.AreEqual((double)1022.0, cbor[4].AsDouble());
      Assert.AreEqual((double)1023.0, cbor[5].AsDouble());
      Assert.AreEqual((double)1024.0, cbor[6].AsDouble());
      Assert.AreEqual((double)1025.0, cbor[7].AsDouble());
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
    [Timeout(10000)]
    public void TestFromJsonStringLongKindFullBad() {
      Console.WriteLine("FullBad 1");
      var jsonop = new JSONOptions("numberconversion=full");
      string manysevens = TestCommon.Repeat("7", 1000000);
      string[] badjson = {
        manysevens + "x",
        "7x" + manysevens,
        manysevens + "e0x",
        "-" + manysevens + "x",
        "-7x" + manysevens,
        "-" + manysevens + "e0x",
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
    [Timeout(10000)]
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
    [Timeout(10000)]
    public void TestFromJsonStringLongKindIntOrFloatFromDouble() {
      var jsonop = new JSONOptions("numberconversion=intorfloatfromdouble");
      string manysevens = TestCommon.Repeat("7", 1000000);
      string json = manysevens;
      CBORObject cbor = FromJSON(json, jsonop);
      Assert.AreEqual(CBORType.FloatingPoint, cbor.Type);
      Assert.IsTrue(cbor.AsDoubleValue() == Double.PositiveInfinity);
      json = manysevens + "e+0";
      cbor = FromJSON(json, jsonop);
      Assert.AreEqual(CBORType.FloatingPoint, cbor.Type);
      Assert.IsTrue(cbor.AsDoubleValue() == Double.PositiveInfinity);
      json = manysevens + "e0";
      cbor = FromJSON(json, jsonop);
      Assert.AreEqual(CBORType.FloatingPoint, cbor.Type);
      Assert.IsTrue(cbor.AsDoubleValue() == Double.PositiveInfinity);
    }

    [Test]
    [Timeout(10000)]
    public void TestFromJsonStringLongKindIntOrFloat() {
      var jsonop = new JSONOptions("numberconversion=intorfloat");
      string json = TestCommon.Repeat("7", 1000000);
      CBORObject cbor = FromJSON(json, jsonop);
      Assert.AreEqual(CBORType.FloatingPoint, cbor.Type);
      Assert.IsTrue(cbor.AsDoubleValue() == Double.PositiveInfinity);
    }

    [Test]
    [Timeout(10000)]
    public void TestFromJsonStringLongKindIntOrFloat2() {
      var jsonop = new JSONOptions("numberconversion=intorfloat");
      string json = "-" + TestCommon.Repeat("7", 1000000);
      CBORObject cbor = FromJSON(json, jsonop);
      Assert.AreEqual(CBORType.FloatingPoint, cbor.Type);
      Assert.IsTrue(cbor.AsDoubleValue() == Double.NegativeInfinity);
    }

[Test]
public void TestRoundTripRegressions() {
{
var options = new CBOREncodeOptions("allowduplicatekeys=1;keepkeyorder=1");
var bytes = new byte[] {
  (byte)0xba, 0x00, 0x00, 0x00, 0x03,
  (byte)0xf9,
  (byte)0x83, 0x1d,
  (byte)0xda,
  (byte)0xb6,
  (byte)0xda, 0x50, 0x56, 0x1a, 0x50,
  (byte)0xe3, 0x2c, 0x7a, 0x16,
  (byte)0xfa, 0x50, 0x32, 0x73, 0x07,
  (byte)0xfa, (byte)0xb9, 0x2d, 0x73, (byte)0xce, 0x38, (byte)0xd0,
};
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(bytes, options));
}
{
var options = new CBOREncodeOptions("allowduplicatekeys=1;keepkeyorder=1");
var bytes = new byte[] {
  (byte)0xbf,
  (byte)0x9f,
  (byte)0xbf, 0x39, 0x20,
  (byte)0x8f, 0x4a, 0x1f, 0x46, 0x26, 0x0b, 0x3e, 0x72, 0x2c, 0x7f, 0x11,
  0x2e, 0x39,
  (byte)0x9d,
  (byte)0xba, 0x1a, 0x11,
  (byte)0x8d,
  (byte)0xc0,
  (byte)0xb4, 0x38,
  (byte)0xb6,
  (byte)0x9b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
  (byte)0xd8, 0x3b,
  (byte)0x99, 0x00, 0x02, 0x3b, 0x05,
  (byte)0xbb,
  (byte)0xea,
  (byte)0x8e, 0x4b,
  (byte)0xd3, 0x5e, 0x22,
  (byte)0x9f, 0x59, 0x00, 0x00,
  (byte)0xbb, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x41, 0x20,
  (byte)0xbf, 0x1a, 0x00, 0x00, 0x00, 0x61,
  (byte)0xb9, 0x00, 0x01, 0x1a, 0x00, 0x00, 0x00, 0x0e,
  (byte)0xba, 0x00, 0x00, 0x00, 0x00,
  (byte)0xff,
  (byte)0xff,
  (byte)0xff,
  (byte)0xd8, 0x22,
  (byte)0xf8,
  (byte)0x93,
  (byte)0xd9,
  (byte)0xaf, 0x33, 0x19,
  (byte)0xf0,
  (byte)0xf0,
  (byte)0xf9,
  (byte)0x85,
  (byte)0x93,
  (byte)0x99, 0x00, 0x01, 0x3a,
  (byte)0xb5,
  (byte)0xfb, 0x4d, 0x43,
  (byte)0x98, 0x00,
  (byte)0xff, (byte)0xfa, (byte)0xb0, (byte)0xb4, (byte)0xdc, 0x6d,
  (byte)0xff,
};
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(bytes, options));
}
{
var options = new CBOREncodeOptions("allowduplicatekeys=1;keepkeyorder=1");
var bytes = new byte[] {
  (byte)0xdb, 0x0d,
  (byte)0xcb, 0x5d, 0x78,
  (byte)0x92,
  (byte)0xc2,
  (byte)0xc7, 0x2b,
  (byte)0xb9, 0x00, 0x02, 0x39,
  (byte)0xee,
  (byte)0xa0, (byte)0xa0, 0x1a, 0x0e, (byte)0xd9, (byte)0xec, (byte)0xca,
  (byte)0xf2,
};
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(bytes, options));
}
{
var options = new CBOREncodeOptions("allowduplicatekeys=1;keepkeyorder=1");
var bytes = new byte[] {
  (byte)0xbf,
  (byte)0xfb,
  (byte)0xb1, 0x21,
  (byte)0x93,
  (byte)0x8c,
  (byte)0xc6,
  (byte)0xf3,
  (byte)0xcf,
  (byte)0xb7, (byte)0xf8, 0x76, 0x18, (byte)0xda, 0x39, 0x60, (byte)0xf4,
  (byte)0xff,
};
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(bytes, options));
}
{
var options = new CBOREncodeOptions("allowduplicatekeys=1;keepkeyorder=1");
var bytes = new byte[] {
  (byte)0xbb, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
  0x00, 0x02, (byte)0xf0, 0x0d, 0x2a, 0x21,
};
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(bytes, options));
}
{
var options = new CBOREncodeOptions("allowduplicatekeys=1;keepkeyorder=1");
var bytes = new byte[] {
  (byte)0xba, 0x00, 0x00, 0x00, 0x02,
  (byte)0xf9, 0x48, 0x37,
  (byte)0xda,
  (byte)0xb5, 0x72,
  (byte)0xcf,
  (byte)0xf8, 0x31, 0x3b, 0x06, 0x78,
  (byte)0xdb, 0x44, 0x7d, (byte)0xba, (byte)0xbd, 0x7d, 0x39, (byte)0x98,
  (byte)0xb9,
};
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(bytes, options));
}
{
var options = new CBOREncodeOptions("allowduplicatekeys=1");
var bytes = new byte[] {
  (byte)0xbf, 0x0d,
  (byte)0xdb, 0x7f, 0x53,
  (byte)0xd5, 0x1e,
  (byte)0xab, 0x1f,
  (byte)0xb2,
  (byte)0xc2,
  (byte)0xb8, 0x02, 0x7f, 0x7a, 0x00, 0x00, 0x00, 0x09,
  (byte)0xf0,
  (byte)0xb8,
  (byte)0xbf,
  (byte)0xbf, 0x00,
  (byte)0xf0,
  (byte)0x9d,
  (byte)0x84,
  (byte)0xa1,
  (byte)0xff, 0x1a, 0x00, 0x46, 0x31,
  (byte)0xdf, 0x7f, 0x69, 0x05, 0x47, 0x76, 0x4f, 0x01,
  (byte)0xf4,
  (byte)0x80,
  (byte)0x80,
  (byte)0x80, (byte)0xff, 0x3a, 0x0a, (byte)0xaa, (byte)0xf2, 0x00,
  (byte)0xff,
};
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(bytes, options));
}
{
var options = new CBOREncodeOptions("allowduplicatekeys=1");
var bytes = new byte[] {
  (byte)0xbf, 0x0d,
  (byte)0xdb, 0x7f, 0x53,
  (byte)0xd5, 0x1e,
  (byte)0xab, 0x1f, 0x23,
  (byte)0xc2,
  (byte)0xb8, 0x02, 0x7f, 0x69,
  (byte)0xc2,
  (byte)0xa8, 0x7f, 0x39, 0x7f,
  (byte)0xe4,
  (byte)0xa1,
  (byte)0xae, 0x1c,
  (byte)0xff, 0x17, 0x7f, 0x69, 0x05, 0x47, 0x76, 0x4f, 0x01,
  (byte)0xec,
  (byte)0x90,
  (byte)0xb2, 0x0a, (byte)0xff, (byte)0xfa, 0x12, 0x49, 0x20, 0x61,
  (byte)0xff,
};
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(bytes, options));
}
{
var options = new CBOREncodeOptions("allowduplicatekeys=1");
var bytes = new byte[] {
  (byte)0x9b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
  0x00, 0x01,
  (byte)0xd8, 0x52,
  (byte)0xbf, 0x0c,
  (byte)0xf9, 0x68, 0x67, 0x3b,
  (byte)0xdb,
  (byte)0x85, 0x5b, 0x59,
  (byte)0xfd, 0x03, 0x6c,
  (byte)0x80,
  (byte)0xf8,
  (byte)0xc4, 0x7f, 0x67, 0x73, 0x2b, 0x51, 0x31, 0x5d, 0x26, 0x67,
  (byte)0xff, 0x5f, 0x5b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08,
  (byte)0xc7,
  (byte)0xb9, 0x6b,
  (byte)0xb0,
  (byte)0xb6,
  (byte)0xbe, 0x6d,
  (byte)0x9e, 0x41, 0x34, 0x5a, 0x00, 0x00, 0x00, 0x02,
  (byte)0xc4, 0x4a,
  (byte)0xff, 0x67,
  (byte)0xe1,
  (byte)0x99,
  (byte)0x92,
  (byte)0xf0,
  (byte)0xb5, (byte)0xa4, (byte)0xa2, 0x3a, 0x77, 0x11, 0x4c, 0x6f,
  (byte)0xff,
};
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(bytes, options));
}
{
var options = new CBOREncodeOptions("allowduplicatekeys=1;keepkeyorder=1");
var bytes = new byte[] {
  (byte)0x9b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
  0x00, 0x01,
  (byte)0x9f,
  (byte)0xf9, 0x03,
  (byte)0xf1, 0x3b, 0x1a, 0x6f,
  (byte)0xc2, 0x1b,
  (byte)0xce, 0x23,
  (byte)0xcb, 0x2e,
  (byte)0xbf,
  (byte)0xf8, 0x25,
  (byte)0xfb, 0x01, 0x54, 0x4a, 0x78, 0x13,
  (byte)0xff, 0x12,
  (byte)0x91,
  (byte)0xff,
  (byte)0xbf, 0x78, 0x04, 0x7a, 0x43, 0x30, 0x04, 0x41, 0x55, 0x7f, 0x7a,
  0x00, 0x00, 0x00, 0x03,
  (byte)0xda,
  (byte)0xb3, 0x64, 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x58,
  (byte)0xff, 0x39, (byte)0xa2, 0x48, (byte)0xff, (byte)0xff,
};
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(bytes, options));
}
{
var options = new CBOREncodeOptions("allowduplicatekeys=1");
var bytes = new byte[] {
  (byte)0x81,
  (byte)0xda,
  (byte)0x8a, 0x18, 0x00, 0x00,
  (byte)0xda,
  (byte)0xd5,
  (byte)0xf5,
  (byte)0x96, 0x10,
  (byte)0x9b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
  (byte)0xbf, 0x6f, 0x22, 0x65, 0x65,
  (byte)0xf1,
  (byte)0x86,
  (byte)0x9d,
  (byte)0xad, 0x22, 0x42,
  (byte)0xc5,
  (byte)0xb1, 0x62, 0x58, 0x01, 0x5e,
  (byte)0xda, 0x47, 0x47,
  (byte)0x87,
  (byte)0x94,
  (byte)0xed, 0x7f, 0x6c,
  (byte)0xf0,
  (byte)0x9c,
  (byte)0xbc,
  (byte)0x96, 0x2f, 0x47, 0x7c, 0x00, 0x50, 0x67, 0x67, 0x10, 0x78, 0x03,
  (byte)0xc3,
  (byte)0x90, 0x17,
  (byte)0xff, 0x19,
  (byte)0xd2,
  (byte)0xe7,
  (byte)0x99, 0x00, 0x01, 0x1b, 0x2a, 0x6e, 0x6f, 0x67, 0x4b, 0x18, 0x60,
  0x51, 0x1b, 0x46,
  (byte)0x9f, (byte)0xd3, (byte)0xb7, (byte)0xf4, 0x74, (byte)0xad, 0x6c,
  (byte)0xff,
};
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(bytes, options));
}
{
var options = new CBOREncodeOptions("allowduplicatekeys=1;keepkeyorder=1");
var bytes = new byte[] {
  (byte)0xda,
  (byte)0xcf,
  (byte)0xf0,
  (byte)0xbe, 0x18,
  (byte)0x99, 0x00, 0x01,
  (byte)0xb9, 0x00, 0x01,
  (byte)0xbf, 0x7f, 0x61, 0x5d, 0x7a, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00,
  0x20, 0x5a, 0x66, 0x1c, 0x7a, 0x00, 0x00, 0x00, 0x02, 0x7d, 0x7f, 0x60,
  0x78, 0x01, 0x43,
  (byte)0xff, 0x1a,
  (byte)0xca, 0x5c,
  (byte)0x83, 0x47, 0x7f, 0x79, 0x00, 0x0a,
  (byte)0xcc,
  (byte)0x88, 0x00, 0x73, 0x5f, 0x00, 0x26, 0x08, 0x72, 0x60,
  (byte)0xff, 0x00,
  (byte)0xff, 0x1b,
  (byte)0xbb, 0x19, (byte)0xbf, (byte)0x9f, 0x55, (byte)0xee, 0x56, 0x09,
};
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(bytes, options));
}
}
[Test]
public void TestMapCompareRegressions() {
  CBORObject m1, m2;
  m1 = CBORObject.NewMap().Add(3, 4).Add(1, 2);
  m2 = CBORObject.NewOrderedMap().Add(3, 4).Add(1, 2);
  Assert.AreEqual(0, m1.CompareTo(m2));
  TestCommon.CompareTestEqualAndConsistent(m1, m2);
  m1 = CBORObject.NewMap().Add(3, 2).Add(1, 2);
  m2 = CBORObject.NewOrderedMap().Add(3, 4).Add(1, 2);
  TestCommon.CompareTestLess(m1, m2);
  m1 = CBORObject.NewMap().Add(3, 7).Add(1, 2);
  m2 = CBORObject.NewOrderedMap().Add(3, 4).Add(1, 2);
  TestCommon.CompareTestGreater(m1, m2);
  m1 = CBORObject.NewMap().Add(3, 4).Add(1, 0);
  m2 = CBORObject.NewOrderedMap().Add(3, 4).Add(1, 2);
  TestCommon.CompareTestLess(m1, m2);
  m1 = CBORObject.NewMap().Add(3, 4).Add(1, 7);
  m2 = CBORObject.NewOrderedMap().Add(3, 4).Add(1, 2);
  TestCommon.CompareTestGreater(m1, m2);
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

[Test]
public void TestRegressionFour() {
   CBORObject o1 = CBORObject.FromObject(new byte[] { (byte)5, (byte)2 });
   CBORObject o2 = CBORObject.FromObject(new byte[] { (byte)0x85, (byte)2 });
   TestCommon.CompareTestLess(o1, o2);
}

[Test]
public void TestRegressionOne() {
{
CBOREncodeOptions options = new CBOREncodeOptions();
byte[] bytes = new byte[] {
  (byte)0xbf, 0x0d,
  (byte)0xdb, 0x7f, 0x53,
  (byte)0xd5, 0x1e,
  (byte)0xab, 0x1f,
  (byte)0xb2,
  (byte)0xc2,
  (byte)0xb8, 0x02, 0x7f, 0x7a, 0x00, 0x00, 0x00, 0x09,
  (byte)0xf0,
  (byte)0xb8,
  (byte)0xbf,
  (byte)0xbf, 0x00,
  (byte)0xf0,
  (byte)0x9d,
  (byte)0x84,
  (byte)0xa1,
  (byte)0xff, 0x1a, 0x00, 0x46, 0x31,
  (byte)0xdf, 0x7f, 0x69, 0x05, 0x47, 0x76, 0x4f, 0x01,
  (byte)0xf4,
  (byte)0x80,
  (byte)0x80,
  (byte)0x80, (byte)0xff, 0x3a, 0x0a, (byte)0xaa, (byte)0xf2, 0x00,
  (byte)0xff,
};
byte[] encodedBytes = new byte[] {
  (byte)0xa1, 0x0d,
  (byte)0xdb, 0x7f, 0x53,
  (byte)0xd5, 0x1e,
  (byte)0xab, 0x1f,
  (byte)0xb2,
  (byte)0xc2,
  (byte)0xa2, 0x69, 0x05, 0x47, 0x76, 0x4f, 0x01,
  (byte)0xf4,
  (byte)0x80,
  (byte)0x80,
  (byte)0x80, 0x3a, 0x0a,
  (byte)0xaa,
  (byte)0xf2, 0x00, 0x69,
  (byte)0xf0,
  (byte)0xb8,
  (byte)0xbf,
  (byte)0xbf, 0x00,
  (byte)0xf0, (byte)0x9d, (byte)0x84, (byte)0xa1, 0x1a, 0x00, 0x46, 0x31,
  (byte)0xdf,
};
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(bytes));
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(encodedBytes));
}
}

[Test]
public void TestRegressionTwo() {
{
CBOREncodeOptions options = new CBOREncodeOptions();
byte[] bytes = new byte[] {
  (byte)0xb8, 0x02, 0x7f, 0x7a, 0x00, 0x00, 0x00,
  0x09,
  (byte)0xf0,
  (byte)0xb8,
  (byte)0xbf,
  (byte)0xbf, 0x00,
  (byte)0xf0,
  (byte)0x9d,
  (byte)0x84,
  (byte)0xa1,
  (byte)0xff, 0x1a, 0x00, 0x46, 0x31,
  (byte)0xdf, 0x7f, 0x69, 0x05, 0x47, 0x76, 0x4f, 0x01,
  (byte)0xf4,
  (byte)0x80,
  (byte)0x80,
  (byte)0x80, (byte)0xff, 0x3a, 0x0a, (byte)0xaa, (byte)0xf2, 0x00,
};
byte[] encodedBytes = new byte[] {
  (byte)0xa2, 0x69, 0x05, 0x47, 0x76, 0x4f,
  0x01,
  (byte)0xf4,
  (byte)0x80,
  (byte)0x80,
  (byte)0x80, 0x3a, 0x0a,
  (byte)0xaa,
  (byte)0xf2, 0x00, 0x69,
  (byte)0xf0,
  (byte)0xb8,
  (byte)0xbf,
  (byte)0xbf, 0x00,
  (byte)0xf0, (byte)0x9d, (byte)0x84, (byte)0xa1, 0x1a, 0x00, 0x46, 0x31,
  (byte)0xdf,
};
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(bytes));
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(encodedBytes));
}
}

[Test]
public void TestRegressionThree() {
{
CBOREncodeOptions options = new CBOREncodeOptions();
byte[] bytes = new byte[] {
  (byte)0xb8, 0x02, 0x7f, 0x7a, 0x00, 0x00, 0x00,
  0x09,
  (byte)0xf0,
  (byte)0xb8,
  (byte)0xbf,
  (byte)0xbf, 0x00,
  (byte)0xf0,
  (byte)0x9d,
  (byte)0x84,
  (byte)0xa1,
  (byte)0xff, 0, 0x7f, 0x69, 0x05, 0x47, 0x76, 0x4f, 0x01,
  (byte)0xf4,
  (byte)0x80,
  (byte)0x80,
  (byte)0x80, (byte)0xff, 0,
};
byte[] encodedBytes = new byte[] {
  (byte)0xa2, 0x69, 0x05, 0x47, 0x76, 0x4f,
  0x01,
  (byte)0xf4,
  (byte)0x80,
  (byte)0x80,
  (byte)0x80, 0, 0x69,
  (byte)0xf0,
  (byte)0xb8,
  (byte)0xbf, (byte)0xbf, 0x00, (byte)0xf0, (byte)0x9d, (byte)0x84,
  (byte)0xa1, 0,
};
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(bytes));
CBORTestCommon.AssertRoundTrip(CBORObject.DecodeFromBytes(encodedBytes));
}
}

[Test]
public void TestStringCompareBug() {
  CBORObject a, b, c, d;
  var bytes = new byte[] {
    0x69, 0x05, 0x47, 0x76, 0x4f, 0x01,
    (byte)0xf4,
    (byte)0x80,
    (byte)0x80,
    (byte)0x80,
  };
  a = CBORObject.DecodeFromBytes(bytes);
  c = a;
  bytes = new byte[] {
    0x7f, 0x69, 0x05, 0x47, 0x76, 0x4f, 0x01,
    (byte)0xf4,
    (byte)0x80,
    (byte)0x80,
    (byte)0x80, (byte)0xff,
  };
  b = CBORObject.DecodeFromBytes(bytes);
  d = b;
  TestCommon.CompareTestEqual(a, b);
  bytes = new byte[] {
    0x7f, 0x7a, 0x00, 0x00, 0x00, 0x09,
    (byte)0xf0,
    (byte)0xb8,
    (byte)0xbf,
    (byte)0xbf, 0x00,
    (byte)0xf0,
    (byte)0x9d,
    (byte)0x84,
    (byte)0xa1,
    (byte)0xff,
  };
  a = CBORObject.DecodeFromBytes(bytes);
  bytes = new byte[] {
    0x7f, 0x69,
    (byte)0xf0,
    (byte)0xb8,
    (byte)0xbf,
    (byte)0xbf, 0x00,
    (byte)0xf0,
    (byte)0x9d,
    (byte)0x84,
    (byte)0xa1,
    (byte)0xff,
  };
  b = CBORObject.DecodeFromBytes(bytes);
  TestCommon.CompareTestEqual(a, b);
  TestCommon.CompareTestLess(c, a);
  TestCommon.CompareTestLess(c, b);
  TestCommon.CompareTestLess(d, a);
  TestCommon.CompareTestLess(d, b);
  CBORObject o1 = CBORObject.NewMap();
  o1.Add(b, CBORObject.FromObject(0));
  o1.Add(c, CBORObject.FromObject(0));
  CBORObject o2 = CBORObject.NewMap();
  o2.Add(c, CBORObject.FromObject(0));
  o2.Add(b, CBORObject.FromObject(0));
  TestCommon.CompareTestEqual(a, b);
}

[Test]
[Timeout(2000)]
public void TestSlowDecode() {
byte[] bytes = new byte[] {
  (byte)0xa4,
  (byte)0xa3,
  (byte)0xa4,
  (byte)0xa3,
  (byte)0xa3,
  (byte)0xf0, 0x02,
  (byte)0xf1,
  (byte)0xf5,
  (byte)0xf2, 0x02,
  (byte)0xf7,
  (byte)0xa0, 0x02,
  (byte)0xa3,
  (byte)0xf0, 0x01,
  (byte)0xf1, 0x01,
  (byte)0xf2,
  (byte)0xf7,
  (byte)0xf7,
  (byte)0xf5,
  (byte)0xa0, 0x01,
  (byte)0xa3,
  (byte)0xa3, 0x00,
  (byte)0xf5, 0x01,
  (byte)0xf4, 0x02, 0x02, 0x02,
  (byte)0xa0, 0x01,
  (byte)0xa3, 0x00,
  (byte)0xa0, 0x01,
  (byte)0xa0, 0x02, 0x02,
  (byte)0xf6, 0x02, 0x40,
  (byte)0xf5, 0x02,
  (byte)0xa4,
  (byte)0xa3,
  (byte)0xf0, 0x02,
  (byte)0xf1,
  (byte)0xf7,
  (byte)0xf2,
  (byte)0xf6, 0x00,
  (byte)0xa3,
  (byte)0xa4,
  (byte)0xa3,
  (byte)0xf1,
  (byte)0xf4,
  (byte)0xf2, 0x02,
  (byte)0xf3, 0x01, 0x02,
  (byte)0xa0,
  (byte)0xa0,
  (byte)0xa3,
  (byte)0xf0, 0x00,
  (byte)0xf1,
  (byte)0xf7,
  (byte)0xf2,
  (byte)0xf5, 0x00, 0x40,
  (byte)0xf4,
  (byte)0xf4,
  (byte)0xa4, 0x00,
  (byte)0xf6, 0x01,
  (byte)0xf4, 0x02, 0x00, 0x40, 0x01,
  (byte)0xf5,
  (byte)0xa4,
  (byte)0xf0,
  (byte)0xa0,
  (byte)0xf1,
  (byte)0xa0,
  (byte)0xf2,
  (byte)0xf4, 0x40, 0x00, 0x02, 0x01,
  (byte)0xa3,
  (byte)0xa3,
  (byte)0xf1,
  (byte)0xf6,
  (byte)0xf2,
  (byte)0xf6,
  (byte)0xf3,
  (byte)0xf4,
  (byte)0xa0,
  (byte)0xa3, 0x00, 0x00, 0x01,
  (byte)0xf4, 0x02,
  (byte)0xf7,
  (byte)0xf6,
  (byte)0xa3,
  (byte)0xa4,
  (byte)0xf1,
  (byte)0xf5,
  (byte)0xf2,
  (byte)0xf6,
  (byte)0xf3, 0x01, 0x40,
  (byte)0xa0, 0x00,
  (byte)0xa4, 0x00, 0x02, 0x01,
  (byte)0xa0, 0x02, 0x01, 0x40, 0x00,
  (byte)0xf4,
  (byte)0xa4,
  (byte)0xa4,
  (byte)0xa3,
  (byte)0xf0,
  (byte)0xf5,
  (byte)0xf1,
  (byte)0xf7,
  (byte)0xf2,
  (byte)0xf7,
  (byte)0xf4,
  (byte)0xa3, 0x00,
  (byte)0xf6, 0x01, 0x02, 0x02,
  (byte)0xf5,
  (byte)0xf5,
  (byte)0xa3,
  (byte)0xa3,
  (byte)0xa3,
  (byte)0xf0,
  (byte)0xf4,
  (byte)0xf1,
  (byte)0xf5,
  (byte)0xf2,
  (byte)0xf6, 0x01,
  (byte)0xa3,
  (byte)0xa4,
  (byte)0xa4,
  (byte)0xa3,
  (byte)0xa4,
  (byte)0xa3,
  (byte)0xa3,
  (byte)0xf0,
  (byte)0xf4,
  (byte)0xf1,
  (byte)0xf6,
  (byte)0xf2, 0x02, 0x01,
  (byte)0xa0,
  (byte)0xf6,
  (byte)0xa3,
  (byte)0xf1,
  (byte)0xf7,
  (byte)0xf2, 0x00,
  (byte)0xf3, 0x01, 0x01,
  (byte)0xf4,
  (byte)0xa3,
  (byte)0xf1, 0x01,
  (byte)0xf2, 0x00,
  (byte)0xf3,
  (byte)0xf6,
  (byte)0xf5,
  (byte)0xa3,
  (byte)0xf1,
  (byte)0xf5,
  (byte)0xf2,
  (byte)0xa0,
  (byte)0xf3,
  (byte)0xf6,
  (byte)0xf4, 0x40, 0x02, 0x02,
  (byte)0xa4,
  (byte)0xa3,
  (byte)0xa4,
  (byte)0xf1,
  (byte)0xf7,
  (byte)0xf2,
  (byte)0xf4,
  (byte)0xf3,
  (byte)0xf7, 0x40, 0x01, 0x00,
  (byte)0xa4,
  (byte)0xf0,
  (byte)0xf6,
  (byte)0xf1,
  (byte)0xf5,
  (byte)0xf2, 0x00, 0x40,
  (byte)0xf5,
  (byte)0xa0,
  (byte)0xa4,
  (byte)0xa3, 0x00, 0x00, 0x01,
  (byte)0xf5, 0x02,
  (byte)0xf6, 0x00,
  (byte)0xa0, 0x00,
  (byte)0xa3, 0x00, 0x02, 0x01, 0x02, 0x02, 0x00, 0x02, 0x40, 0x02, 0x00,
  (byte)0xf6,
  (byte)0xa3, 0x00,
  (byte)0xf7, 0x01, 0x00, 0x02, 0x01, 0x02,
  (byte)0xa3,
  (byte)0xa4,
  (byte)0xa4, 0x00,
  (byte)0xf4, 0x01,
  (byte)0xf5, 0x02, 0x00, 0x40,
  (byte)0xf7, 0x00,
  (byte)0xa4,
  (byte)0xf1,
  (byte)0xf6,
  (byte)0xf2,
  (byte)0xf4,
  (byte)0xf3,
  (byte)0xf5, 0x40,
  (byte)0xa0,
  (byte)0xf7,
  (byte)0xa4, 0x00, 0x00, 0x01,
  (byte)0xa0, 0x02,
  (byte)0xf5, 0x40, 0x00, 0x02, 0x40, 0x01,
  (byte)0xf7,
  (byte)0xa4,
  (byte)0xa3,
  (byte)0xa3, 0x00,
  (byte)0xf6, 0x01,
  (byte)0xf4, 0x02,
  (byte)0xf4, 0x02,
  (byte)0xa3,
  (byte)0xf0,
  (byte)0xf6,
  (byte)0xf1,
  (byte)0xf4,
  (byte)0xf2, 0x01,
  (byte)0xf4,
  (byte)0xa3,
  (byte)0xa4,
  (byte)0xa3,
  (byte)0xf1,
  (byte)0xf4,
  (byte)0xf2,
  (byte)0xa0,
  (byte)0xf3, 0x01,
  (byte)0xf6,
  (byte)0xa0,
  (byte)0xf6,
  (byte)0xa3,
  (byte)0xf0,
  (byte)0xf5,
  (byte)0xf1, 0x00,
  (byte)0xf2,
  (byte)0xf5, 0x02, 0x40,
  (byte)0xf6,
  (byte)0xf6,
  (byte)0xa4,
  (byte)0xf0, 0x01,
  (byte)0xf1,
  (byte)0xf4,
  (byte)0xf2,
  (byte)0xf6, 0x40,
  (byte)0xf4,
  (byte)0xa0,
  (byte)0xa4,
  (byte)0xa3,
  (byte)0xa4, 0x00,
  (byte)0xa0, 0x01, 0x00, 0x02,
  (byte)0xf6, 0x40,
  (byte)0xf4, 0x00,
  (byte)0xa4,
  (byte)0xa4,
  (byte)0xf0,
  (byte)0xf4,
  (byte)0xf1,
  (byte)0xf5,
  (byte)0xf2, 0x00, 0x40,
  (byte)0xa0,
  (byte)0xf6,
  (byte)0xa4, 0x00, 0x00, 0x01, 0x02, 0x02, 0x02, 0x40,
  (byte)0xf6,
  (byte)0xa0,
  (byte)0xa4,
  (byte)0xf1,
  (byte)0xf5,
  (byte)0xf2,
  (byte)0xf4,
  (byte)0xf3,
  (byte)0xf4, 0x40,
  (byte)0xf5,
  (byte)0xa0, 0x40,
  (byte)0xf7, 0x02,
  (byte)0xa4,
  (byte)0xa4, 0x00, 0x02, 0x01, 0x02, 0x02, 0x01, 0x40,
  (byte)0xf5,
  (byte)0xf7,
  (byte)0xa4,
  (byte)0xf1,
  (byte)0xa0,
  (byte)0xf2,
  (byte)0xf6,
  (byte)0xf3,
  (byte)0xf4, 0x40,
  (byte)0xf6,
  (byte)0xf4,
  (byte)0xa4,
  (byte)0xf0, 0x00,
  (byte)0xf1,
  (byte)0xf7,
  (byte)0xf2, 0x02, 0x40,
  (byte)0xf4,
  (byte)0xf4, 0x40, 0x02,
  (byte)0xf4,
  (byte)0xf6,
  (byte)0xa3,
  (byte)0xf1,
  (byte)0xf5,
  (byte)0xf2,
  (byte)0xf6,
  (byte)0xf3, 0x00,
  (byte)0xf7,
  (byte)0xa3,
  (byte)0xf0,
  (byte)0xf7,
  (byte)0xf1,
  (byte)0xf5,
  (byte)0xf2, 0x00, 0x01, 0x40, 0x01, 0x01, 0x00,
  (byte)0xf7,
  (byte)0xa0, 0x00,
  (byte)0xa3,
  (byte)0xa4,
  (byte)0xf1, 0x02,
  (byte)0xf2,
  (byte)0xf4,
  (byte)0xf3,
  (byte)0xf7, 0x40,
  (byte)0xf6,
  (byte)0xf7,
  (byte)0xa4,
  (byte)0xf1,
  (byte)0xf6,
  (byte)0xf2,
  (byte)0xf4,
  (byte)0xf3,
  (byte)0xf4, 0x40,
  (byte)0xf6,
  (byte)0xf6,
  (byte)0xa4,
  (byte)0xa3,
  (byte)0xf1,
  (byte)0xf6,
  (byte)0xf2, 0x00,
  (byte)0xf3,
  (byte)0xf4,
  (byte)0xf5,
  (byte)0xa0,
  (byte)0xf6,
  (byte)0xa3, 0x00, 0x01, 0x01,
  (byte)0xa0, 0x02, 0x01,
  (byte)0xa0, 0x40, 0x02, 0x00,
  (byte)0xf5, 0x40,
  (byte)0xf5,
  (byte)0xa0,
  (byte)0xa4,
  (byte)0xa3, 0x00,
  (byte)0xf5, 0x01, 0x02, 0x02, 0x00,
  (byte)0xf6,
  (byte)0xa0,
  (byte)0xf6,
  (byte)0xa3,
  (byte)0xf1,
  (byte)0xf6,
  (byte)0xf2,
  (byte)0xf5,
  (byte)0xf3,
  (byte)0xf5, 0x00, 0x40,
  (byte)0xf5,
  (byte)0xf6, 0x02, 0x40, 0x01, 0x01,
  (byte)0xa4,
  (byte)0xf1,
  (byte)0xa0,
  (byte)0xf2, 0x01,
  (byte)0xf3,
  (byte)0xf5, 0x40, 0x00, 0x02,
  (byte)0xa0,
  (byte)0xa0, 0x00,
  (byte)0xa3,
  (byte)0xf0,
  (byte)0xf4,
  (byte)0xf1,
  (byte)0xa0,
  (byte)0xf2, 0x00, 0x01, 0x40, 0x02, 0x00,
  (byte)0xa4,
  (byte)0xf0, 0x01,
  (byte)0xf1, 0x02,
  (byte)0xf2,
  (byte)0xa0, 0x40,
  (byte)0xf5, 0x02,
  (byte)0xa4,
  (byte)0xa3,
  (byte)0xf0, 0x00,
  (byte)0xf1, 0x01,
  (byte)0xf2,
  (byte)0xf5,
  (byte)0xf7,
  (byte)0xa3,
  (byte)0xa3,
  (byte)0xf1, 0x01,
  (byte)0xf2, 0x01,
  (byte)0xf3, 0x01,
  (byte)0xa0,
  (byte)0xa0, 0x01,
  (byte)0xa3,
  (byte)0xf1,
  (byte)0xf4,
  (byte)0xf2, 0x00,
  (byte)0xf3,
  (byte)0xf4,
  (byte)0xf6, 0x02,
  (byte)0xa3,
  (byte)0xf1,
  (byte)0xf4,
  (byte)0xf2, 0x01,
  (byte)0xf3,
  (byte)0xf4, 0x02, 0x40, 0x00,
  (byte)0xa0, 0x40,
  (byte)0xa0,
  (byte)0xf5,
  (byte)0xa4,
  (byte)0xf0,
  (byte)0xf7,
  (byte)0xf1,
  (byte)0xf4,
  (byte)0xf2,
  (byte)0xa0, 0x40,
  (byte)0xa0, 0x00,
  (byte)0xa4, 0x00, 0x00, 0x01,
  (byte)0xa0, 0x02,
  (byte)0xf5, 0x40,
  (byte)0xf7,
  (byte)0xf7, 0x00,
  (byte)0xa3, 0x00, 0x01, 0x01, 0x01, 0x02,
  (byte)0xf4,
  (byte)0xf7,
  (byte)0xf7,
  (byte)0xa0,
  (byte)0xa0,
  (byte)0xa3,
  (byte)0xf0,
  (byte)0xf6,
  (byte)0xf1, 0x01,
  (byte)0xf2, 0x01, 0x00,
  (byte)0xf6, 0x40,
  (byte)0xf6,
  (byte)0xf6,
  (byte)0xa4,
  (byte)0xf1,
  (byte)0xf7,
  (byte)0xf2, 0x01,
  (byte)0xf3, 0x01, 0x40,
  (byte)0xf7,
  (byte)0xf5,
  (byte)0xa4,
  (byte)0xf1,
  (byte)0xf6,
  (byte)0xf2, 0x00,
  (byte)0xf3,
  (byte)0xa0, 0x40, 0x01, 0x02, 0x40, 0x01,
  (byte)0xa0,
  (byte)0xa0, 0x02, 0x40, 0x01, 0x02,
  (byte)0xa4,
  (byte)0xf0,
  (byte)0xf4,
  (byte)0xf1,
  (byte)0xf5,
  (byte)0xf2,
  (byte)0xf5, 0x40,
  (byte)0xf7,
  (byte)0xa0,
  (byte)0xf5,
  (byte)0xa0,
  (byte)0xf5,
  (byte)0xa3, 0x00,
  (byte)0xf6, 0x01, (byte)0xf6, 0x02, (byte)0xa0, (byte)0xf4, 0x40,
  (byte)0xf7,
};
var options = new CBOREncodeOptions("allowduplicatekeys=1");
for (var i = 0; i < 10; ++i) {
  CBORObject.DecodeFromBytes(bytes, options);
}
}
  }
}
