using System;
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

    private static EFloat RandomEFloatLowExponent(IRandomGenExtended rand) {
      while (true) {
        EFloat ef = RandomObjects.RandomEFloat(rand);
        if (
          ef.Exponent.CompareTo(-20000) >= 0 &&
ef.Exponent.CompareTo(20000) <= 0) {
          return ef;
        }
      }
    }

    private static EDecimal RandomEDecimalLowExponent(IRandomGenExtended rand) {
      while (true) {
        EDecimal ef = RandomObjects.RandomEDecimal(rand);
        if (
          ef.Exponent.CompareTo(-20000) >= 0 &&
ef.Exponent.CompareTo(20000) <= 0) {
          return ef;
        }
      }
    }

    public static CBORObject RandomNumber(IRandomGenExtended rand) {
      return RandomNumber(rand, false);
    }

    public static CBORObject RandomNumber(IRandomGenExtended rand, bool
lowExponent) {
      object o;
      switch (rand.GetInt32(6)) {
        case 0:
          o = RandomObjects.RandomDouble(
            rand,
            Int32.MaxValue);
          return CBORObject.FromObject(o);
        case 1:
          o = RandomObjects.RandomSingle(
            rand,
            Int32.MaxValue);
          return CBORObject.FromObject(o);
        case 2:
          return CBORObject.FromEInteger(
              RandomObjects.RandomEInteger(rand));
        case 3:
          o = lowExponent ? RandomEFloatLowExponent(rand) :
               RandomObjects.RandomEFloat(rand);
          return CBORObject.FromObject(o);
        case 4:
          o = lowExponent ? RandomEDecimalLowExponent(rand) :
               RandomObjects.RandomEDecimal(rand);
          return CBORObject.FromObject(o);
        case 5:
          o = RandomObjects.RandomInt64(rand);
          return CBORObject.FromObject(o);
        default: throw new InvalidOperationException();
      }
    }

    public static CBORObject RandomNumberOrRational(IRandomGenExtended rand) {
      object o;
      switch (rand.GetInt32(7)) {
        case 0:
          o = RandomObjects.RandomDouble(
            rand,
            Int32.MaxValue);
          return CBORObject.FromObject(o);
        case 1:
          o = RandomObjects.RandomSingle(
            rand,
            Int32.MaxValue);
          return CBORObject.FromObject(o);
        case 2:
          return CBORObject.FromEInteger(
              RandomObjects.RandomEInteger(rand));
        case 3:
          return CBORObject.FromEFloat(
              RandomObjects.RandomEFloat(rand));
        case 4:
          o = RandomObjects.RandomEDecimal(rand);
          return CBORObject.FromObject(o);
        case 5:
          o = RandomObjects.RandomInt64(rand);
          return CBORObject.FromObject(o);
        case 6:
          o = RandomObjects.RandomERational(rand);
          return CBORObject.FromObject(o);
        default: throw new InvalidOperationException();
      }
    }

    public static CBORObject RandomCBORMap(IRandomGenExtended rand, int depth) {
      int x = rand.GetInt32(100);
      int count = (x < 80) ? 2 : ((x < 93) ? 1 : ((x < 98) ? 0 : 10));
      CBORObject cborRet = rand.GetInt32(100) < 30 ?
         CBORObject.NewOrderedMap() : CBORObject.NewMap();
      for (int i = 0; i < count; ++i) {
        CBORObject key = RandomCBORObject(rand, depth + 1);
        CBORObject value = RandomCBORObject(rand, depth + 1);
        cborRet[key] = value;
      }
      return cborRet;
    }

    public static EInteger RandomEIntegerMajorType0(IRandomGenExtended rand) {
      int v = rand.GetInt32(0x10000);
      var ei = EInteger.FromInt32(v);
      ei = ei.ShiftLeft(16).Add(rand.GetInt32(0x10000));
      ei = ei.ShiftLeft(16).Add(rand.GetInt32(0x10000));
      ei = ei.ShiftLeft(16).Add(rand.GetInt32(0x10000));
      return ei;
    }

    public static EInteger RandomEIntegerMajorType0Or1(IRandomGenExtended
rand) {
      int v = rand.GetInt32(0x10000);
      var ei = EInteger.FromInt32(v);
      ei = ei.ShiftLeft(16).Add(rand.GetInt32(0x10000));
      ei = ei.ShiftLeft(16).Add(rand.GetInt32(0x10000));
      ei = ei.ShiftLeft(16).Add(rand.GetInt32(0x10000));
      if (rand.GetInt32(2) == 0) {
        ei = ei.Add(1).Negate();
      }
      return ei;
    }

    public static CBORObject RandomCBORTaggedObject(
      IRandomGenExtended rand,
      int depth) {
      int tag;
      if (rand.GetInt32(2) == 0) {
        int[] tagselection = {
          2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 30, 30,
          30, 0, 1, 25, 26, 27,
        };
        tag = tagselection[rand.GetInt32(tagselection.Length)];
      } else {
        return rand.GetInt32(100) < 90 ?
          CBORObject.FromCBORObjectAndTag(
            RandomCBORObject(rand, depth + 1),
            rand.GetInt32(0x100000)) : CBORObject.FromCBORObjectAndTag(
            RandomCBORObject(rand, depth + 1),
            RandomEIntegerMajorType0(rand));
      }
      if (tag == 25) {
        tag = 0;
      }
      if (tag == 30) {
        object o = RandomObjects.RandomByteString(rand);
        return CBORObject.FromObject(o);
      }
      {
        CBORObject cbor;
        // Console.WriteLine("tag "+tag+" "+i);
        if (tag is 0 or 1 or 28 or 29) {
          tag = 999;
        }
        if (tag is 2 or 3) {
          object o = RandomObjects.RandomByteStringShort(rand);
          cbor = CBORObject.FromObject(o);
        } else if (tag is 4 or 5) {
          cbor = CBORObject.NewArray();
          object o = RandomObjects.RandomSmallIntegral(rand);
          _ = cbor.Add(o);
          o = RandomObjects.RandomEInteger(rand);
          _ = cbor.Add(o);
        } else if (tag == 30) {
          cbor = CBORObject.NewArray();
          object o = RandomObjects.RandomSmallIntegral(rand);
          _ = cbor.Add(o);
          o = RandomObjects.RandomEInteger(rand);
          _ = cbor.Add(o);
        } else {
          cbor = RandomCBORObject(rand, depth + 1);
        }
        return CBORObject.FromCBORObjectAndTag(cbor, tag);
      }
    }

    public static CBORObject RandomCBORArray(IRandomGenExtended rand, int
depth) {
      int x = rand.GetInt32(100);
      int count = (x < 80) ? 2 : ((x < 93) ? 1 : ((x < 98) ? 0 : 10));
      var cborRet = CBORObject.NewArray();
      for (int i = 0; i < count; ++i) {
        _ = cborRet.Add(RandomCBORObject(rand, depth + 1));
      }
      return cborRet;
    }

    public static CBORObject RandomCBORObject(IRandomGenExtended rand) {
      return RandomCBORObject(rand, 0);
    }

    public static CBORObject RandomCBORObject(IRandomGenExtended rand, int
      depth) {
      int nextval = rand.GetInt32(11);
      return nextval switch {
        0 or 1 or 2 or 3 => RandomNumberOrRational(rand),
        4 => rand.GetInt32(2) == 0 ? CBORObject.True : CBORObject.False,
        5 => rand.GetInt32(2) == 0 ? CBORObject.Null :
                    CBORObject.Undefined,
        6 => CBORObject.FromString(
                      RandomObjects.RandomTextString(rand)),
        7 => CBORObject.FromByteArray(
                      RandomObjects.RandomByteString(rand)),
        8 => RandomCBORArray(rand, depth),
        9 => RandomCBORMap(rand, depth),
        10 => RandomCBORTaggedObject(rand, depth),
        _ => RandomNumber(rand),
      };
    }

    public static byte[] CheckEncodeToBytes(CBORObject o) {
      byte[] bytes = o.EncodeToBytes();
      if (bytes.Length != o.CalcEncodedSize()) {
        string msg = "encoded size doesn't match:\no = " +
          TestCommon.ToByteArrayString(bytes) + "\nostring = " + o.ToString();
        Assert.AreEqual(bytes.Length, o.CalcEncodedSize(), msg);
      }
      return bytes;
    }

    public static void AssertRoundTrip(CBORObject o) {
      CBORObject o2 = FromBytesTestAB(CheckEncodeToBytes(o));
      TestCommon.CompareTestEqual(o, o2);
      TestCommon.AssertEqualsHashCode(o, o2);
    }

    public static void AssertJSONSer(CBORObject o, string s) {
      if (!s.Equals(o.ToJSONString(), StringComparison.Ordinal)) {
        Assert.AreEqual(s, o.ToJSONString(), "o is not equal to s");
      }
      byte[] bytes = CheckEncodeToBytes(o);
      // Test round-tripping
      CBORObject o2 = FromBytesTestAB(bytes);
      if (!s.Equals(o2.ToJSONString(), StringComparison.Ordinal)) {
        string msg = "o2 is not equal to s:\no = " +
          TestCommon.ToByteArrayString(bytes) +
          "\no2 = " + TestCommon.ToByteArrayString(o2.EncodeToBytes()) +
          "\no2string = " + o2.ToString();
        Assert.AreEqual(s, o2.ToJSONString(), msg);
      }
      TestCommon.AssertEqualsHashCode(o, o2);
    }

    // Tests the equivalence of the DecodeFromBytes and Read methods.
    public static CBORObject FromBytesTestAB(byte[] b, CBOREncodeOptions
options) {
      CBORObject oa = FromBytesA(b, options);
      CBORObject ob = FromBytesB(b, options);
      if (!oa.Equals(ob)) {
        Assert.AreEqual(oa, ob);
      }
      return oa;
    }

    private static CBORObject FromBytesA(byte[] b, CBOREncodeOptions options) {
      return CBORObject.DecodeFromBytes(b, options);
    }

    private static CBORObject FromBytesB(byte[] b, CBOREncodeOptions options) {
      using var ms = new Test.DelayingStream(b);
      var o = CBORObject.Read(ms, options);
      return ms.Position != ms.Length ? throw new CBORException("not at" +
"\u0020EOF") : o;
    }

    // Tests the equivalence of the DecodeFromBytes and Read methods.
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
      using var ms = new Test.DelayingStream(b);
      var o = CBORObject.Read(ms);
      return ms.Position != ms.Length ? throw new CBORException("not at" +
"\u0020EOF") : o;
    }
  }
}
