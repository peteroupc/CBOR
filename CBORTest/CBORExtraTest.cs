#pragma warning disable SA1118
/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
#if !NET20
using System.Linq;
#endif
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;
using PeterO.Numbers;

namespace Test {
  [TestFixture]
  public class CBORExtraTest {
    private static decimal RandomDecimal(RandomGenerator rand, int exponent) {
      var x = new int[4];
      int r = rand.UniformInt(0x10000);
      r |= ((int)rand.UniformInt(0x10000)) << 16;
      x[0] = r;
      if (rand.UniformInt(2) == 0) {
        r = rand.UniformInt(0x10000);
        r |= ((int)rand.UniformInt(0x10000)) << 16;
        x[1] = r;
        if (rand.UniformInt(2) == 0) {
          r = rand.UniformInt(0x10000);
          r |= ((int)rand.UniformInt(0x10000)) << 16;
          x[2] = r;
        }
      }
      x[3] = exponent << 16;
      if (rand.UniformInt(2) == 0) {
        x[3] |= 1 << 31;
      }
      return new Decimal(x);
    }

    [Test]
    public void TestCBORObjectDecimal() {
      var rand = new RandomGenerator();
      for (var i = 0; i <= 28; ++i) {
       // Try a random decimal with a given
       // exponent
        for (int j = 0; j < 8; ++j) {
          decimal d = RandomDecimal(rand, i);
          CBORObject obj = ToObjectTest.TestToFromObjectRoundTrip(d);
          CBORTestCommon.AssertRoundTrip(obj);
          decimal decimalOther = 0m;
          try {
            decimalOther = obj.AsDecimal();
          } catch (Exception ex) {
            Assert.Fail(ex.ToString() + "\r\n" +
                 CBORTest.ObjectMessage(obj));
            throw new InvalidOperationException(String.Empty, ex);
          }
          Assert.AreEqual(d, decimalOther);
        }
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(EDecimal.NaN).AsDecimal();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
       // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(EDecimal.SignalingNaN).AsDecimal();
Assert.Fail("Should have failed");
      } catch (OverflowException) {
       // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecPosInf)
                  .AsDecimal();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
       // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecNegInf)
                  .AsDecimal();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
       // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(EFloat.NaN).AsDecimal();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
       // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(EFloat.SignalingNaN).AsDecimal();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
       // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatPosInf)
                  .AsDecimal();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
       // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf)
                  .AsDecimal();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
       // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestSByte() {
      for (int i = SByte.MinValue; i <= SByte.MaxValue; ++i) {
        CBORTestCommon.AssertJSONSer(
          ToObjectTest.TestToFromObjectRoundTrip((sbyte)i),
          TestCommon.LongToString(i));
      }
    }

    private static IEnumerable<int> RangeExclusive(int min, int maxExclusive) {
      for (int i = min; i < maxExclusive; ++i) {
        yield return i;
      }
    }

    private enum AByte : byte {
    /// <summary>An arbitrary value.</summary>
      A = 254,

    /// <summary>An arbitrary value.</summary>
      B,
    }

    private enum AInt {
    /// <summary>An arbitrary value.</summary>
      A = 256,

    /// <summary>An arbitrary value.</summary>
      B,
    }

    private enum AULong : ulong {
    /// <summary>An arbitrary value.</summary>
      A = 999999,

    /// <summary>An arbitrary value.</summary>
      B,
    }

    public sealed class CPOD2 {
      public string Aa { get; set; }

      public bool IsAa { get; set; }
    }
    [Test]
    public void TestCPOD2() {
      var m = new CPOD2();
      m.Aa = "Test";
      m.IsAa = false;
      CBORObject cbor = CBORObject.FromObject(m);
     // ambiguous properties
      Assert.IsFalse(cbor.ContainsKey("aa"), cbor.ToString());
      Assert.IsFalse(cbor.ContainsKey("Aa"), cbor.ToString());
    }

    [Test]
    [Timeout(5000)]
    public void TestPODOptions() {
      var ao = new { PropA = 0, PropB = 0, IsPropC = false };
      var valueCcTF = new PODOptions(true, false);
      var valueCcFF = new PODOptions(false, false);
      var valueCcFT = new PODOptions(false, true);
      var valueCcTT = new PODOptions(true, true);
      CBORObjectTest.CheckPropertyNames(ao);
      var arrao = new[] { ao, ao };
      var co = CBORObject.FromObject(arrao, valueCcTF);
      CBORObjectTest.CheckArrayPropertyNames(
  CBORObject.FromObject(arrao, valueCcTF),
  2,
  "PropA",
  "PropB",
  "IsPropC");
      CBORObjectTest.CheckArrayPropertyNames(
  CBORObject.FromObject(arrao, valueCcFF),
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
#if !NET20
      var queryao =
from x in arrao select x;
      co = CBORObject.FromObject(queryao, valueCcTF);
      CBORObjectTest.CheckArrayPropertyNames(
  CBORObject.FromObject(queryao, valueCcTF),
  2,
  "PropA",
  "PropB",
  "IsPropC");
      CBORObjectTest.CheckArrayPropertyNames(
  CBORObject.FromObject(queryao, valueCcFF),
  2,
  "PropA",
  "PropB",
  "IsPropC");
      CBORObjectTest.CheckArrayPropertyNames(
  CBORObject.FromObject(queryao, valueCcFT),
  2,
  "propA",
  "propB",
  "propC");
      CBORObjectTest.CheckArrayPropertyNames(
  CBORObject.FromObject(queryao, valueCcTT),
  2,
  "propA",
  "propB",
  "propC");
#endif
      var ao2 = new {
        PropValue = new { PropA = 0, PropB = 0, IsPropC = false, },
      };
      CBORObjectTest.CheckPODPropertyNames(
  CBORObject.FromObject(ao2, valueCcTF),
  valueCcTF,
  "PropA",
  "PropB",
  "IsPropC");
      CBORObjectTest.CheckPODPropertyNames(
  CBORObject.FromObject(ao2, valueCcFF),
  valueCcFF,
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
      var aodict = new Dictionary<string, object> {
        ["PropValue"] = new { PropA = 0, PropB = 0, IsPropC = false, },
      };
      CBORObjectTest.CheckPODInDictPropertyNames(
  CBORObject.FromObject(aodict, valueCcTF),
  "PropA",
  "PropB",
  "IsPropC");
      CBORObjectTest.CheckPODInDictPropertyNames(
  CBORObject.FromObject(aodict, valueCcFF),
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
    }

    [Test]
    public void TestArbitraryTypes() {
      CBORObject obj = CBORObject.FromObject(new {
        AByte.A,
        B = AInt.A,
        C = AULong.A, });
      if (obj == null) {
        Assert.Fail();
      }
      if (obj["a"] == null) {
        Assert.Fail();
      }
      if (obj["b"] == null) {
        Assert.Fail();
      }
      if (obj["c"] == null) {
        Assert.Fail();
      }
      Assert.AreEqual(254, obj["a"].AsInt32());
      Assert.AreEqual(256, obj["b"].AsInt32());
      Assert.AreEqual(999999, obj["c"].AsInt32());
      obj = CBORObject.FromObject(new { A = "a", B = "b" });
      {
        string stringTemp = obj["a"].AsString();
        Assert.AreEqual(
          "a",
          stringTemp);
      }
      {
        string stringTemp = obj["b"].AsString();
        Assert.AreEqual(
          "b",
          stringTemp);
      }
      CBORTestCommon.AssertRoundTrip(obj);
      obj = CBORObject.FromObject(new { A = "c", B = "b" });
      {
        string stringTemp = obj["a"].AsString();
        Assert.AreEqual(
          "c",
          stringTemp);
      }
      {
        string stringTemp = obj["b"].AsString();
        Assert.AreEqual(
          "b",
          stringTemp);
      }
      CBORTestCommon.AssertRoundTrip(obj);
      obj = CBORObject.FromObject(RangeExclusive(0, 10));
      Assert.AreEqual(10, obj.Count);
      Assert.AreEqual(0, obj[0].AsInt32());
      Assert.AreEqual(1, obj[1].AsInt32());
      obj = CBORObject.FromObject(
       (object)RangeExclusive(0, 10));
      Assert.AreEqual(10, obj.Count);
      Assert.AreEqual(0, obj[0].AsInt32());
      Assert.AreEqual(1, obj[1].AsInt32());
      CBORTestCommon.AssertRoundTrip(obj);
#if !NET20
     // Select all even numbers
      var query =
from i in RangeExclusive(0, 10)
where i % 2 == 0
select i;
      obj = CBORObject.FromObject(query);
      Assert.AreEqual(5, obj.Count);
      Assert.AreEqual(0, obj[0].AsInt32());
      Assert.AreEqual(2, obj[1].AsInt32());
      CBORTestCommon.AssertRoundTrip(obj);
     // Select all even numbers
      var query2 =
from i in RangeExclusive(0, 10)
where i % 2 == 0
select new { A = i, B = i + 1 };
      obj = CBORObject.FromObject(query2);
      Assert.AreEqual(5, obj.Count);
      Assert.AreEqual(0, obj[0]["a"].AsInt32());
      Assert.AreEqual(3, obj[1]["b"].AsInt32());
      CBORTestCommon.AssertRoundTrip(obj);
#endif
    }

    [Test]
    public void TestMultidimArray() {
      int[,] arr = { { 0, 1, 99 }, { 2, 3, 299 } };
      var cbor = CBORObject.FromObject(arr);
      Assert.AreEqual(0, cbor[0][0].AsInt32());
      Assert.AreEqual(1, cbor[0][1].AsInt32());
      Assert.AreEqual(99, cbor[0][2].AsInt32());
      Assert.AreEqual(2, cbor[1][0].AsInt32());
      Assert.AreEqual(3, cbor[1][1].AsInt32());
      Assert.AreEqual(299, cbor[1][2].AsInt32());
      var arr2 = cbor.ToObject(typeof(int[,]));
      Assert.AreEqual(arr, arr2);
      int[,,] arr3 = new int[2, 2, 2];
      arr3[0, 0, 0] = 0;
      arr3[0, 0, 1] = 1;
      arr3[0, 1, 0] = 99;
      arr3[0, 1, 1] = 100;
      arr3[1, 0, 0] = 2;
      arr3[1, 0, 1] = 3;
      arr3[1, 1, 0] = 299;
      arr3[1, 1, 1] = 300;
      cbor = CBORObject.FromObject(arr3);
      Assert.AreEqual(0, cbor[0][0][0].AsInt32());
      Assert.AreEqual(1, cbor[0][0][1].AsInt32());
      Assert.AreEqual(99, cbor[0][1][0].AsInt32());
      Assert.AreEqual(100, cbor[0][1][1].AsInt32());
      Assert.AreEqual(2, cbor[1][0][0].AsInt32());
      Assert.AreEqual(299, cbor[1][1][0].AsInt32());
      var arr4 = cbor.ToObject(typeof(int[,,]));
      Assert.AreEqual(arr3, arr4);
    }

    [Test]
    public void TestFloatCloseToEdge() {
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.147483647E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.147483647E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.147483647E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.147483647E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836470000002E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836470000002E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836470000002E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.1474836470000002E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836469999998E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836469999998E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836469999998E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.1474836469999998E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.147483648E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.147483648E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.147483648E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.147483648E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836480000005E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836480000005E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836480000005E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.1474836480000005E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836479999998E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836479999998E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836479999998E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.1474836479999998E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.147483646E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.147483646E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.147483646E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.147483646E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836460000002E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836460000002E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836460000002E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.1474836460000002E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836459999998E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836459999998E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(2.1474836459999998E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.1474836459999998E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.147483648E9d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.147483648E9d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.147483648E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.147483648E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836479999998E9d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836479999998E9d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836479999998E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836479999998E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836480000005E9d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836480000005E9d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836480000005E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836480000005E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.147483647E9d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.147483647E9d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.147483647E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.147483647E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836469999998E9d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836469999998E9d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836469999998E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836469999998E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836470000002E9d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836470000002E9d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836470000002E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836470000002E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.147483649E9d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.147483649E9d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.147483649E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.147483649E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836489999995E9d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836489999995E9d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836489999995E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836489999995E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836490000005E9d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836490000005E9d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836490000005E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-2.1474836490000005E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(9.223372036854776E18d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(9.223372036854776E18d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(9.223372036854776E18d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(9.223372036854776E18d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(9.223372036854778E18d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(9.223372036854778E18d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(9.223372036854778E18d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(9.223372036854778E18d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(9.2233720368547748E18d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(9.2233720368547748E18d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(9.2233720368547748E18d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(9.2233720368547748E18d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-9.223372036854776E18d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-9.223372036854776E18d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-9.223372036854776E18d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-9.223372036854776E18d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-9.2233720368547748E18d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-9.2233720368547748E18d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-9.2233720368547748E18d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-9.2233720368547748E18d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-9.223372036854778E18d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-9.223372036854778E18d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-9.223372036854778E18d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(-9.223372036854778E18d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.0d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.0d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.0d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.0d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.000000000004d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.000000000004d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.000000000004d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.000000000004d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.999999999996d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.999999999996d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.999999999996d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.999999999996d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.0d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.0d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.0d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.0d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.00000000001d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.00000000001d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.00000000001d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.00000000001d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.999999999996d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.999999999996d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.999999999996d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.999999999996d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.0d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.0d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.0d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.0d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.000000000004d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.000000000004d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.000000000004d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.000000000004d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32765.999999999996d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32765.999999999996d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32765.999999999996d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32765.999999999996d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.0d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.0d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.0d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.0d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.999999999996d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.999999999996d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.999999999996d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.999999999996d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.00000000001d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.00000000001d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.00000000001d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.00000000001d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.0d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.0d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.0d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.0d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32766.999999999996d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32766.999999999996d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32766.999999999996d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32766.999999999996d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.000000000004d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.000000000004d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.000000000004d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.000000000004d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.0d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.0d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.0d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.0d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.99999999999d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.99999999999d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.99999999999d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.99999999999d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.00000000001d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.00000000001d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.00000000001d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.00000000001d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.0d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.0d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.0d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.0d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.9E-324d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.9E-324d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.9E-324d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.9E-324d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-4.9E-324d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-4.9E-324d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-4.9E-324d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-4.9E-324d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0000000000000002d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0000000000000002d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0000000000000002d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0000000000000002d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.9999999999999999d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.9999999999999999d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.9999999999999999d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.9999999999999999d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-0.9999999999999999d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-0.9999999999999999d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-0.9999999999999999d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-0.9999999999999999d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0000000000000002d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0000000000000002d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0000000000000002d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0000000000000002d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.0d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.0d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.0d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.0d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.00000000000003d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.00000000000003d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.00000000000003d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.00000000000003d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.99999999999997d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.99999999999997d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.99999999999997d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.99999999999997d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.0d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.0d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.0d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.0d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.00000000000006d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.00000000000006d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.00000000000006d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.00000000000006d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.99999999999997d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.99999999999997d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.99999999999997d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.99999999999997d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.0d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.0d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.0d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.0d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.00000000000003d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.00000000000003d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.00000000000003d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.00000000000003d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(253.99999999999997d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(253.99999999999997d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(253.99999999999997d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(253.99999999999997d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.14748365E9f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.14748365E9f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.14748365E9f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.14748365E9f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.1474839E9f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.1474839E9f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.1474839E9f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.1474839E9f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.14748352E9f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.14748352E9f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.14748352E9f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(2.14748352E9f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.14748365E9f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.14748365E9f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.14748365E9f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.14748365E9f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.14748352E9f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.14748352E9f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.14748352E9f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.14748352E9f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.1474839E9f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.1474839E9f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.1474839E9f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-2.1474839E9f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(9.223372E18f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(9.223372E18f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(9.223372E18f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(9.223372E18f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(9.223373E18f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(9.223373E18f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(9.223373E18f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(9.223373E18f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(9.2233715E18f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(9.2233715E18f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(9.2233715E18f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(9.2233715E18f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-9.223372E18f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-9.223372E18f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-9.223372E18f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-9.223372E18f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-9.2233715E18f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-9.2233715E18f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-9.2233715E18f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-9.2233715E18f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-9.223373E18f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-9.223373E18f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-9.223373E18f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-9.223373E18f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.0f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.0f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.0f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.0f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.002f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.002f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.002f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.002f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.998f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.998f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.998f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.998f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.0f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.0f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.0f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.0f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.004f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.004f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.004f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32768.004f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.998f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.998f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.998f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32767.998f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.0f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.0f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.0f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.0f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.002f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.002f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.002f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32766.002f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32765.998f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32765.998f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32765.998f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(32765.998f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.0f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.0f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.0f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.0f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.998f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.998f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.998f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.998f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.004f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.004f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.004f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.004f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.0f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.0f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.0f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.0f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32766.998f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32766.998f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32766.998f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32766.998f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.002f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.002f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.002f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32767.002f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.0f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.0f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.0f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.0f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.996f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.996f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.996f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32768.996f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.004f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.004f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.004f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-32769.004f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.0f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.0f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.0f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.0f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.4E-45f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.4E-45f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.4E-45f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.4E-45f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.4E-45f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.4E-45f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.4E-45f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.4E-45f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0000001f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0000001f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0000001f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.0000001f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.99999994f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.99999994f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.99999994f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0.99999994f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-0.99999994f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-0.99999994f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-0.99999994f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-0.99999994f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0000001f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0000001f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0000001f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-1.0000001f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.0f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.0f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.0f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.0f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.00002f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.00002f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.00002f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.00002f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.99998f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.99998f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.99998f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.99998f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.0f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.0f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.0f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.0f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.00003f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.00003f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.00003f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(256.00003f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.99998f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.99998f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.99998f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(255.99998f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.0f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.0f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.0f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.0f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.00002f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.00002f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.00002f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(254.00002f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(253.99998f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(253.99998f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(253.99998f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(253.99998f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.0d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.0d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.0d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.0d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.00000000001d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.00000000001d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.00000000001d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.00000000001d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.99999999999d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.99999999999d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.99999999999d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.99999999999d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.0d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.0d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.0d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.0d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.00000000001d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.00000000001d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.00000000001d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.00000000001d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.99999999999d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.99999999999d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.99999999999d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.99999999999d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.0d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.0d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.0d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.0d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.00000000001d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.00000000001d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.00000000001d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.00000000001d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65533.99999999999d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65533.99999999999d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65533.99999999999d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65533.99999999999d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967295E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967295E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967295E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967295E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(4.2949672950000005E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(4.2949672950000005E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(4.2949672950000005E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.2949672950000005E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(4.2949672949999995E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(4.2949672949999995E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(4.2949672949999995E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.2949672949999995E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967296E9d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967296E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967296E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967296E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967296000001E9d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967296000001E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967296000001E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967296000001E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(4.2949672959999995E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(4.2949672959999995E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(4.2949672959999995E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.2949672959999995E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967294E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967294E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967294E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.294967294E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(4.2949672940000005E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(4.2949672940000005E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(4.2949672940000005E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.2949672940000005E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(4.2949672939999995E9d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(4.2949672939999995E9d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(4.2949672939999995E9d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.2949672939999995E9d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(1.8446744073709552E19d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(1.8446744073709552E19d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(1.8446744073709552E19d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(1.8446744073709552E19d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(1.8446744073709556E19d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(1.8446744073709556E19d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(1.8446744073709556E19d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(1.8446744073709556E19d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(1.844674407370955E19d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(1.844674407370955E19d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
ToObjectTest.TestToFromObjectRoundTrip(1.844674407370955E19d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.844674407370955E19d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.0d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.0d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.0d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.0d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.99999999999999d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.99999999999999d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.99999999999999d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.99999999999999d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.00000000000003d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.00000000000003d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.00000000000003d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.00000000000003d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.0d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.0d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.0d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.0d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-126.99999999999999d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-126.99999999999999d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-126.99999999999999d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-126.99999999999999d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.00000000000001d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.00000000000001d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.00000000000001d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.00000000000001d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.0d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.0d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.0d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.0d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.99999999999997d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.99999999999997d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.99999999999997d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.99999999999997d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.00000000000003d).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.00000000000003d).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.00000000000003d).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.00000000000003d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.0d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.0d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.0d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.0d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.00000000000001d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.00000000000001d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.00000000000001d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.00000000000001d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.99999999999999d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.99999999999999d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.99999999999999d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.99999999999999d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.0d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.0d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.0d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.0d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.00000000000003d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.00000000000003d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.00000000000003d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.00000000000003d).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.99999999999999d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.99999999999999d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.99999999999999d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.99999999999999d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.0d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.0d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.0d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.0d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.00000000000001d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.00000000000001d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.00000000000001d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.00000000000001d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(125.99999999999999d).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(125.99999999999999d).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(125.99999999999999d).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(125.99999999999999d).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.0f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.0f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.0f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.0f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.004f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.004f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.004f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.004f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.996f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.996f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.996f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.996f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.0f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.0f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.0f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.0f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.01f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.01f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.01f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65536.01f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.996f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.996f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.996f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65535.996f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.0f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.0f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.0f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.0f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.004f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.004f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.004f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65534.004f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65533.996f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65533.996f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65533.996f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(65533.996f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.2949673E9f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.2949673E9f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.2949673E9f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.2949673E9f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.2949678E9f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.2949678E9f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.2949678E9f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.2949678E9f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.29496704E9f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.29496704E9f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.29496704E9f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(4.29496704E9f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.8446744E19f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.8446744E19f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.8446744E19f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.8446744E19f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.8446746E19f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.8446746E19f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.8446746E19f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.8446746E19f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.8446743E19f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.8446743E19f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.8446743E19f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(1.8446743E19f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.0f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.0f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.0f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.0f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.99999f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.99999f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.99999f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.99999f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.00002f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.00002f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.00002f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.00002f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.0f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.0f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.0f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.0f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-126.99999f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-126.99999f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-126.99999f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-126.99999f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.00001f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.00001f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.00001f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-127.00001f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.0f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.0f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.0f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.0f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.99998f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.99998f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.99998f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-128.99998f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.00002f).AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.00002f).AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.00002f).AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(-129.00002f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.0f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.0f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.0f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.0f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.00001f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.00001f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.00001f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.00001f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.99999f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.99999f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.99999f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.99999f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.0f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.0f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.0f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.0f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.00002f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.00002f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.00002f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(128.00002f).AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.99999f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.99999f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.99999f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(127.99999f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.0f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.0f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.0f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.0f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.00001f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.00001f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.00001f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(126.00001f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(125.99999f).AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(125.99999f).AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(125.99999f).AsUInt16();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(125.99999f).AsSByte();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
    }

    [Test]
    public void TestULong() {
      ulong[] ranges = {
        0, 65539, 0xfffff000UL, 0x100000400UL,
        0x7ffffffffffff000UL, 0x8000000000000400UL,
        UInt64.MaxValue - 1000, UInt64.MaxValue,
      };
      for (var i = 0; i < ranges.Length; i += 2) {
        ulong j = ranges[i];
        while (true) {
          CBORTestCommon.AssertJSONSer(
            ToObjectTest.TestToFromObjectRoundTrip(j),
            ((PeterO.Numbers.EDecimal)j).ToString());
          if (j == ranges[i + 1]) {
            break;
          }
          ++j;
        }
      }
    }

    private static short Divide32By16(
      int dividendLow,
      short divisor,
      bool returnRemainder) {
      int t;
      var dividendHigh = 0;
      int intDivisor = ((int)divisor) & 0xffff;
      for (var i = 0; i < 32; ++i) {
        t = dividendHigh >> 31;
        dividendHigh <<= 1;
        dividendHigh = unchecked((int)(dividendHigh | ((int)((dividendLow >>
                    31) & 1))));
        dividendLow <<= 1;
        t |= dividendHigh;
       // unsigned greater-than-or-equal check
        if (((t >> 31) != 0) || (t >= intDivisor)) {
          unchecked {
            dividendHigh -= intDivisor;
            ++dividendLow;
          }
        }
      }
      return returnRemainder ? unchecked((short)(((int)dividendHigh) &
                    0xffff)) : unchecked((short)(((int)dividendLow) & 0xffff));
    }

    private static short DivideUnsigned(int x, short y) {
      unchecked {
        int iy = ((int)y) & 0xffff;
        if ((x >> 31) == 0) {
         // x is already nonnegative
          return (short)(((int)x / iy) & 0xffff);
        }
        return Divide32By16(x, y, false);
      }
    }

    [Test]
    public void TestOther() {
      int[,,] arr3 = new int[2, 3, 2];
      CBORObject cbor = CBORObject.FromObject(arr3);
      string stringTemp = cbor.ToJSONString();
      string str145009 = "[[[0,0],[0,0],[0,0]],[[0,0],[0,0],[0,0]]]";
      Assert.AreEqual(
        str145009,
        stringTemp);
      CBORTestCommon.AssertRoundTrip(cbor);
    }

    [Test]
    public void TestDivideUnsigned() {
      var fr = new RandomGenerator();
      unchecked {
        for (var i = 0; i < 1000; ++i) {
          var x = (uint)fr.UniformInt(0x10000);
          x |= ((uint)fr.UniformInt(0x10000)) << 16;
          var y = (ushort)fr.UniformInt(0x10000);
          var dx = (int)x;
          var dy = (short)y;
          if (dy == 0) {
            continue;
          }
          var expected = (short)(x / y);
          short actual = DivideUnsigned(dx, dy);
          if (expected != actual) {
            Assert.AreEqual(expected, actual, "Dividing " + x + " by " + y);
          }
        }
      }
    }

    [Test]
    public void TestUInt() {
      uint[] ranges = {
        0, 65539,
        0x7ffff000U, 0x80000400U, UInt32.MaxValue - 1000, UInt32.MaxValue,
      };
      for (var i = 0; i < ranges.Length; i += 2) {
        uint j = ranges[i];
        while (true) {
          CBORTestCommon.AssertJSONSer(
            ToObjectTest.TestToFromObjectRoundTrip(j),
            TestCommon.LongToString(j));
          if (j == ranges[i + 1]) {
            break;
          }
          ++j;
        }
      }
    }

    [Test]
    public void TestDecimal() {
      CBORTestCommon.AssertJSONSer(
        ToObjectTest.TestToFromObjectRoundTrip(Decimal.MinValue),
        ((EDecimal)Decimal.MinValue).ToString());
      CBORTestCommon.AssertJSONSer(
        ToObjectTest.TestToFromObjectRoundTrip(Decimal.MaxValue),
        ((EDecimal)Decimal.MaxValue).ToString());
      for (int i = -100; i <= 100; ++i) {
        CBORTestCommon.AssertJSONSer(
          ToObjectTest.TestToFromObjectRoundTrip((decimal)i),
          TestCommon.IntToString(i));
        {
CBORObject objectTemp = ToObjectTest.TestToFromObjectRoundTrip((decimal)i +
  0.1m);
string objectTemp2 = ((EDecimal)((decimal)i + 0.1m)).ToString();
CBORTestCommon.AssertJSONSer(objectTemp, objectTemp2);
}
        CBORTestCommon.AssertJSONSer(
          ToObjectTest.TestToFromObjectRoundTrip((decimal)i + 0.1111m),
          ((EDecimal)((decimal)i + 0.1111m)).ToString());
      }
    }

    [Test]
    public void TestUShort() {
      for (int i = UInt16.MinValue; i <= UInt16.MaxValue; ++i) {
        CBORTestCommon.AssertJSONSer(
          ToObjectTest.TestToFromObjectRoundTrip((UInt16)i),
          TestCommon.LongToString(i));
      }
    }

    private struct ExoticStruct {
       public readonly int Pvalue;
       public ExoticStruct(int pv) {
         this.Pvalue = pv;
       }
    }

    [Test]
    public void TestNullable() {
       int? nvalue = 1;
       CBORObject cbor = CBORObject.FromObject(nvalue);
       Assert.AreEqual(CBORObject.FromObject(1), cbor);
       nvalue = null;
       cbor = CBORObject.FromObject(nvalue);
       uint? unvalue = 1u;
       cbor = CBORObject.FromObject(unvalue);
       Assert.AreEqual(CBORObject.FromObject(1), cbor);
       unvalue = null;
       cbor = CBORObject.FromObject(unvalue);
       Assert.AreEqual(CBORObject.Null, cbor);
       Assert.AreEqual(null, CBORObject.Null.ToObject<int?>());
       Assert.AreEqual(1, CBORObject.FromObject(1).ToObject<int?>());
       Assert.AreEqual(null, CBORObject.Null.ToObject<uint?>());
       Assert.AreEqual(1u, CBORObject.FromObject(1).ToObject<uint?>());
       Assert.AreEqual(null, CBORObject.Null.ToObject<double?>());
       if (CBORObject.FromObject(3.5).ToObject<double?>() != 3.5) {
         Assert.Fail();
       }
       ExoticStruct? es = null;
       cbor = CBORObject.FromObject(es);
       Assert.AreEqual(CBORObject.Null, cbor);
    }

    [Test]
    public void TestDoubleToOther() {
      CBORObject dbl1 =
        ToObjectTest.TestToFromObjectRoundTrip((double)Int32.MinValue);
      CBORObject dbl2 =
        ToObjectTest.TestToFromObjectRoundTrip((double)Int32.MaxValue);
      try {
        dbl1.AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        dbl1.AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        dbl1.AsUInt32();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        dbl1.AsUInt64();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        dbl2.AsUInt16();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        dbl2.AsSByte();
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        dbl2.AsUInt32();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
      try {
        dbl2.AsUInt64();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw;
      }
    }
  }
}
