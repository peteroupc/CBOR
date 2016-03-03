using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;
using PeterO.Numbers;

namespace Test {
  internal static class CBORTestCommon {
    internal static readonly EDecimal DecPosInf =
  EDecimal.PositiveInfinity;

    internal static readonly EDecimal DecNegInf =
      EDecimal.NegativeInfinity;

    internal static readonly EFloat FloatPosInf =
      EFloat.PositiveInfinity;

    internal static readonly EFloat FloatNegInf =
      EFloat.NegativeInfinity;

    internal static readonly ERational RatPosInf =
      ERational.PositiveInfinity;

    internal static readonly ERational RatNegInf =
      ERational.NegativeInfinity;

    public static CBORObject RandomNumber(FastRandom rand) {
      switch (rand.NextValue(6)) {
        case 0:
return CBORObject.FromObject(
RandomObjects.RandomDouble(
rand,
Int32.MaxValue));
        case 1:
return CBORObject.FromObject(
RandomObjects.RandomSingle(
rand,
Int32.MaxValue));
        case 2:
          return CBORObject.FromObject(RandomObjects.RandomEInteger(rand));
        case 3:
          return CBORObject.FromObject(RandomObjects.RandomEFloat(rand));
        case 4:
       return
  CBORObject.FromObject(RandomObjects.RandomEDecimal(rand));
        case 5:
          return CBORObject.FromObject(RandomObjects.RandomInt64(rand));
        default: throw new ArgumentException();
      }
    }

    public static CBORObject RandomNumberOrRational(FastRandom rand) {
      switch (rand.NextValue(7)) {
        case 0:
return CBORObject.FromObject(
RandomObjects.RandomDouble(
rand,
Int32.MaxValue));
        case 1:
return CBORObject.FromObject(
RandomObjects.RandomSingle(
rand,
Int32.MaxValue));
        case 2:
          return CBORObject.FromObject(RandomObjects.RandomEInteger(rand));
        case 3:
          return CBORObject.FromObject(RandomObjects.RandomEFloat(rand));
        case 4:
       return
  CBORObject.FromObject(RandomObjects.RandomEDecimal(rand));
        case 5:
          return CBORObject.FromObject(RandomObjects.RandomInt64(rand));
        case 6:
          return CBORObject.FromObject(RandomObjects.RandomRational(rand));
        default: throw new ArgumentException();
      }
    }

    public static CBORObject RandomCBORMap(FastRandom rand, int depth) {
      int x = rand.NextValue(100);
      int count = (x < 80) ? 2 : ((x < 93) ? 1 : ((x < 98) ? 0 : 10));
      CBORObject cborRet = CBORObject.NewMap();
      for (var i = 0; i < count; ++i) {
        CBORObject key = RandomCBORObject(rand, depth + 1);
        CBORObject value = RandomCBORObject(rand, depth + 1);
        cborRet[key] = value;
      }
      return cborRet;
    }

    public static CBORObject RandomCBORTaggedObject(
      FastRandom rand,
      int depth) {
      var tag = 0;
      if (rand.NextValue(2) == 0) {
        int[] tagselection = { 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 30, 30,
          30, 0, 1, 25, 26, 27 };
        tag = tagselection[rand.NextValue(tagselection.Length)];
      } else {
        tag = rand.NextValue(0x1000000);
      }
      if (tag == 25) {
        tag = 0;
      }
      if (tag == 30) {
        return CBORObject.FromObject(RandomObjects.RandomByteString(rand));
      }
      for (var i = 0; i < 15; ++i) {
        CBORObject o;
        // Console.WriteLine("tag "+tag+" "+i);
        if (tag == 0 || tag == 1 || tag == 28 || tag == 29) {
          tag = 999;
        }
        if (tag == 2 || tag == 3) {
          o = CBORObject.FromObject(RandomObjects.RandomByteStringShort(rand));
        } else if (tag == 4 || tag == 5) {
          o = CBORObject.NewArray();
          o.Add(CBORObject.FromObject(RandomObjects.RandomSmallIntegral(rand)));
          o.Add(CBORObject.FromObject(RandomObjects.RandomEInteger(rand)));
        } else if (tag == 30) {
          o = CBORObject.NewArray();
          o.Add(CBORObject.FromObject(RandomObjects.RandomSmallIntegral(rand)));
          o.Add(CBORObject.FromObject(RandomObjects.RandomEInteger(rand)));
        } else {
          o = RandomCBORObject(rand, depth + 1);
        }
        try {
          o = CBORObject.FromObjectAndTag(o, tag);
          // Console.WriteLine("done");
          return o;
        } catch (Exception) {
          continue;
        }
      }
      // Console.WriteLine("Failed "+tag);
      return CBORObject.Null;
    }

    public static CBORObject RandomCBORArray(FastRandom rand, int depth) {
      int x = rand.NextValue(100);
      int count = (x < 80) ? 2 : ((x < 93) ? 1 : ((x < 98) ? 0 : 10));
      CBORObject cborRet = CBORObject.NewArray();
      for (var i = 0; i < count; ++i) {
        cborRet.Add(RandomCBORObject(rand, depth + 1));
      }
      return cborRet;
    }

    public static CBORObject RandomCBORObject(FastRandom rand) {
      return RandomCBORObject(rand, 0);
    }

    public static CBORObject RandomCBORObject(FastRandom rand, int depth) {
      int nextval = rand.NextValue(11);
      switch (nextval) {
        case 0:
        case 1:
        case 2:
        case 3:
          return RandomNumberOrRational(rand);
        case 4:
          return rand.NextValue(2) == 0 ? CBORObject.True : CBORObject.False;
        case 5:
          return rand.NextValue(2) == 0 ? CBORObject.Null :
            CBORObject.Undefined;
        case 6:
          return CBORObject.FromObject(RandomObjects.RandomTextString(rand));
        case 7:
          return CBORObject.FromObject(RandomObjects.RandomByteString(rand));
        case 8:
          return RandomCBORArray(rand, depth);
        case 9:
          return RandomCBORMap(rand, depth);
        case 10:
          return RandomCBORTaggedObject(rand, depth);
        default: return RandomNumber(rand);
      }
    }

    public static void TestNumber(CBORObject o) {
      if (o.Type != CBORType.Number) {
        return;
      }
      if (o.IsPositiveInfinity() || o.IsNegativeInfinity() ||
          o.IsNaN()) {
        try {
          o.AsByte();
          Assert.Fail("Should have failed");
        } catch (OverflowException) {
          new Object();
        } catch (Exception ex) {
          Assert.Fail("Object: " + o + ", " + ex); throw new
            InvalidOperationException(String.Empty, ex);
        }
        try {
          o.AsInt16();
          Assert.Fail("Should have failed");
        } catch (OverflowException) {
          new Object();
        } catch (Exception ex) {
          Assert.Fail("Object: " + o + ", " + ex); throw new
            InvalidOperationException(String.Empty, ex);
        }
        try {
          o.AsInt32();
          Assert.Fail("Should have failed");
        } catch (OverflowException) {
          new Object();
        } catch (Exception ex) {
          Assert.Fail("Object: " + o + ", " + ex); throw new
            InvalidOperationException(String.Empty, ex);
        }
        try {
          o.AsInt64();
          Assert.Fail("Should have failed");
        } catch (OverflowException) {
          new Object();
        } catch (Exception ex) {
          Assert.Fail("Object: " + o + ", " + ex); throw new
            InvalidOperationException(String.Empty, ex);
        }
        try {
          o.AsSingle();
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        try {
          o.AsDouble();
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        try {
          o.AsEInteger();
          Assert.Fail("Should have failed");
        } catch (OverflowException) {
          new Object();
        } catch (Exception ex) {
          Assert.Fail("Object: " + o + ", " + ex); throw new
            InvalidOperationException(String.Empty, ex);
        }
        return;
      }
      try {
        o.AsSingle();
      } catch (Exception ex) {
        Assert.Fail("Object: " + o + ", " + ex); throw new
          InvalidOperationException(String.Empty, ex);
      }
      try {
        o.AsDouble();
      } catch (Exception ex) {
        Assert.Fail("Object: " + o + ", " + ex); throw new
          InvalidOperationException(String.Empty, ex);
      }
    }

    public static void AssertRoundTrip(CBORObject o) {
      CBORObject o2 = FromBytesTestAB(o.EncodeToBytes());
      TestCommon.CompareTestEqual(o, o2);
      TestNumber(o);
      TestCommon.AssertEqualsHashCode(o, o2);
    }

    public static void AssertSer(CBORObject o, String s) {
      if (!s.Equals(o.ToString())) {
        Assert.AreEqual(s, o.ToString(), "o is not equal to s");
      }
      // Test round-tripping
      CBORObject o2 = FromBytesTestAB(o.EncodeToBytes());
      if (!s.Equals(o2.ToString())) {
        Assert.AreEqual(s, o2.ToString(), "o2 is not equal to s");
      }
      TestNumber(o);
      TestCommon.AssertEqualsHashCode(o, o2);
    }

    // Tests the equivalence of the FromBytes and Read methods.
    public static CBORObject FromBytesTestAB(byte[] b) {
      CBORObject oa = FromBytesA(b);
      CBORObject ob = FromBytesB(b);
      if (!oa.Equals(ob)) {
        Assert.AreEqual(oa, ob);
      }
      return oa;
    }

    private static CBORObject FromBytesA(byte[] b) {
      return CBORObject.DecodeFromBytes(b);
    }

    private static CBORObject FromBytesB(byte[] b) {
      using (var ms = new System.IO.MemoryStream(b)) {
        CBORObject o = CBORObject.Read(ms);
        if (ms.Position != ms.Length) {
          throw new CBORException("not at EOF");
        }
        return o;
      }
    }
  }
}
