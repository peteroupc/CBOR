/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.IO;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;
using PeterO.Numbers;

namespace Test {
  [TestFixture]
  public class CBORTest {
    public static void TestCBORMapAdd() {
      CBORObject cbor = CBORObject.NewMap();
      cbor.Add(1, 2);

  Assert.IsTrue(cbor.ContainsKey(ToObjectTest.TestToFromObjectRoundTrip(1)));
      Assert.AreEqual((int)2, cbor[ToObjectTest.TestToFromObjectRoundTrip(1)]);
      {
        string stringTemp = cbor.ToJSONString();
        Assert.AreEqual(
        "{\"1\":2}",
        stringTemp);
      }
      cbor.Add("hello", 2);
      Assert.IsTrue(cbor.ContainsKey("hello"));

      Assert.IsTrue(cbor.ContainsKey(ToObjectTest.TestToFromObjectRoundTrip(
      "hello")));
      Assert.AreEqual((int)2, cbor["hello"]);
      cbor.Set(1, 3);

  Assert.IsTrue(cbor.ContainsKey(ToObjectTest.TestToFromObjectRoundTrip(1)));
      Assert.AreEqual((int)3, cbor[ToObjectTest.TestToFromObjectRoundTrip(1)]);
    }

    [Test]
    public void TestArray() {
      CBORObject cbor = CBORObject.FromJSONString("[]");
      cbor.Add(ToObjectTest.TestToFromObjectRoundTrip(3));
      cbor.Add(ToObjectTest.TestToFromObjectRoundTrip(4));
      byte[] bytes = cbor.EncodeToBytes();
      TestCommon.AssertByteArraysEqual(
        new byte[] { (byte)(0x80 | 2), 3, 4 },
        bytes);
      cbor = CBORObject.FromObject(new[] { "a", "b", "c",
 "d", "e" });
      Assert.AreEqual("[\"a\",\"b\",\"c\",\"d\",\"e\"]", cbor.ToJSONString());
      string[] strArray = (string[])cbor.ToObject(typeof(string[]));
      cbor = CBORObject.FromObject(strArray);
      Assert.AreEqual("[\"a\",\"b\",\"c\",\"d\",\"e\"]", cbor.ToJSONString());
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0x9f, 0, 1, 2, 3, 4, 5,
                    6, 7, 0xff });
      {
        string stringTemp = cbor.ToJSONString();
        string str1817 = "[0,1,2,3,4,5,6,7]";

        Assert.AreEqual(
  str1817,
  stringTemp);
      }
    }

    [Test]
    public void TestEInteger() {
      var r = new RandomGenerator();
      for (var i = 0; i < 500; ++i) {
        EInteger bi = RandomObjects.RandomEInteger(r);
        CBORTestCommon.AssertJSONSer(
          ToObjectTest.TestToFromObjectRoundTrip(bi),
          bi.ToString());
        Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(bi).IsIntegral);

  CBORTestCommon.AssertRoundTrip(ToObjectTest.TestToFromObjectRoundTrip(bi));
        CBORTestCommon.AssertRoundTrip(ToObjectTest.TestToFromObjectRoundTrip(
          EDecimal.FromString(bi.ToString() + "e1")));
      }
      EInteger[] ranges = {
       EInteger.FromString("-9223372036854776320"),
  EInteger.FromString("-9223372036854775296"),
  EInteger.FromString("-512"),
  EInteger.FromString("512"),
  EInteger.FromString("9223372036854775295"),
  EInteger.FromString("9223372036854776319"),
  EInteger.FromString("18446744073709551103"),
  EInteger.FromString("18446744073709552127")
      };
      for (var i = 0; i < ranges.Length; i += 2) {
        EInteger bigintTemp = ranges[i];
        while (true) {
          CBORTestCommon.AssertJSONSer(
            ToObjectTest.TestToFromObjectRoundTrip(bigintTemp),
            bigintTemp.ToString());
          if (bigintTemp.Equals(ranges[i + 1])) {
            break;
          }
          bigintTemp += EInteger.One;
        }
      }
    }

    [Test]
    public void TestBigNumBytes() {
      CBORObject o = null;
      o = CBORTestCommon.FromBytesTestAB(new byte[] { 0xc2, 0x41, 0x88 });
      Assert.AreEqual(EInteger.FromRadixString("88", 16), o.AsEInteger());
      o = CBORTestCommon.FromBytesTestAB(new byte[] { 0xc2, 0x42, 0x88, 0x77 });
      Assert.AreEqual(EInteger.FromRadixString("8877", 16), o.AsEInteger());
      o = CBORTestCommon.FromBytesTestAB(new byte[] { 0xc2, 0x44, 0x88, 0x77,
        0x66,
        0x55 });
      Assert.AreEqual(
  EInteger.FromRadixString("88776655", 16),
  o.AsEInteger());
      o = CBORTestCommon.FromBytesTestAB(new byte[] { 0xc2, 0x47, 0x88, 0x77,
        0x66,
        0x55, 0x44, 0x33, 0x22 });
      Assert.AreEqual(
  EInteger.FromRadixString("88776655443322", 16),
  o.AsEInteger());
    }

    [Test]
    public void TestByte() {
      for (var i = 0; i <= 255; ++i) {
        CBORTestCommon.AssertJSONSer(
          ToObjectTest.TestToFromObjectRoundTrip((byte)i),
          TestCommon.IntToString(i));
      }
    }

    [Test]
    public void TestByteArray() {
      CBORObject co = ToObjectTest.TestToFromObjectRoundTrip(
              new byte[] { 0x20, 0x78 });
      EInteger[] tags = co.GetAllTags();
      Assert.AreEqual(0, tags.Length);
      byte[] bytes = co.GetByteString();
      Assert.AreEqual(2, bytes.Length);
      Assert.AreEqual(0x20, bytes[0]);
      Assert.AreEqual(0x78, bytes[1]);
    }

    [Test]
    public void TestByteStringStream() {
      CBORTestCommon.FromBytesTestAB(
        new byte[] { 0x5f, 0x41, 0x20, 0x41, 0x20, 0xff });
    }
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestByteStringStreamNoIndefiniteWithinDefinite() {
      CBORTestCommon.FromBytesTestAB(new byte[] { 0x5f, 0x41, 0x20, 0x5f, 0x41,
        0x20, 0xff, 0xff });
    }
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestByteStringStreamNoTagsBeforeDefinite() {
      CBORTestCommon.FromBytesTestAB(new byte[] { 0x5f, 0x41, 0x20, 0xc2, 0x41,
        0x20, 0xff });
    }

    public static string ObjectMessage(CBORObject obj) {
      return new System.Text.StringBuilder()
        .Append("CBORObject.DecodeFromBytes(")
           .Append(TestCommon.ToByteArrayString(obj.EncodeToBytes()))
           .Append("); /").Append("/ ").Append(obj.ToJSONString()).ToString();
    }

    [Test]
    public void TestCanFitIn() {
      var r = new RandomGenerator();
      for (var i = 0; i < 5000; ++i) {
        CBORObject ed = CBORTestCommon.RandomNumber(r);
        EDecimal ed2;

        ed2 = EDecimal.FromDouble(AsED(ed).ToDouble());
        if ((AsED(ed).CompareTo(ed2) == 0) != ed.CanFitInDouble()) {
          Assert.Fail(ObjectMessage(ed));
        }
        ed2 = EDecimal.FromSingle(AsED(ed).ToSingle());
        if ((AsED(ed).CompareTo(ed2) == 0) != ed.CanFitInSingle()) {
          Assert.Fail(ObjectMessage(ed));
        }
        if (!ed.IsInfinity() && !ed.IsNaN()) {
          ed2 = EDecimal.FromEInteger(AsED(ed)
                    .ToEInteger());
          if ((AsED(ed).CompareTo(ed2) == 0) != ed.IsIntegral) {
            Assert.Fail(ObjectMessage(ed));
          }
        }
        if (!ed.IsInfinity() && !ed.IsNaN()) {
          EInteger bi = ed.AsEInteger();
          if (ed.IsIntegral) {
            if ((bi.GetSignedBitLength() <= 31) != ed.CanFitInInt32()) {
              Assert.Fail(ObjectMessage(ed));
            }
          }
          if ((bi.GetSignedBitLength() <= 31) !=
               ed.CanTruncatedIntFitInInt32()) {
            Assert.Fail(ObjectMessage(ed));
          }
          if (ed.IsIntegral) {
            if ((bi.GetSignedBitLength() <= 63) != ed.CanFitInInt64()) {
              Assert.Fail(ObjectMessage(ed));
            }
          }
          if ((bi.GetSignedBitLength() <= 63) !=
               ed.CanTruncatedIntFitInInt64()) {
            Assert.Fail(ObjectMessage(ed));
          }
        }
      }
    }

    [Test]
    public void TestCanFitInSpecificCases() {
      CBORObject cbor = CBORObject.DecodeFromBytes(new byte[] { (byte)0xfb,
        0x41, (byte)0xe0, (byte)0x85, 0x48, 0x2d, 0x14, 0x47, 0x7a });  // 2217361768.63373
      Assert.AreEqual(
  EInteger.FromString("2217361768"),
  cbor.AsEInteger());
      Assert.IsFalse(cbor.AsEInteger().GetSignedBitLength() <= 31);
      Assert.IsFalse(cbor.CanTruncatedIntFitInInt32());
      cbor = CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5, (byte)0x82,
        0x18, 0x2f, 0x32 });  // -2674012278751232
      Assert.AreEqual(52, cbor.AsEInteger().GetSignedBitLength());
      Assert.IsTrue(cbor.CanFitInInt64());
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(2554895343L)
              .CanFitInSingle());
      cbor = CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5, (byte)0x82,
        0x10, 0x38, 0x64 });  // -6619136
      Assert.AreEqual(EInteger.FromString("-6619136"), cbor.AsEInteger());
      Assert.AreEqual(-6619136, cbor.AsInt32());
      Assert.IsTrue(cbor.CanTruncatedIntFitInInt32());
    }

    [Test]
    public void TestCBOREInteger() {
      CBORObject o = CBORObject.DecodeFromBytes(new byte[] { 0x3b, (byte)0xce,
        (byte)0xe2, 0x5a, 0x57, (byte)0xd8, 0x21, (byte)0xb9, (byte)0xa7 });
      Assert.AreEqual(
        EInteger.FromString("-14907577049884506536"),
        o.AsEInteger());
    }

    [Test]
    public void TestCBORExceptions() {
      try {
        CBORObject.NewArray().Remove(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().Remove(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().Add(CBORObject.Null);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().Add(CBORObject.True);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.Remove(CBORObject.True);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(0).Remove(CBORObject.True);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
                  .Remove(CBORObject.True);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().AsEFloat();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsEFloat();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsEFloat();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsEFloat();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.AsEFloat();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(String.Empty).AsEFloat();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestCBORFromArray() {
      CBORObject o = CBORObject.FromObject(new[] { 1, 2, 3 });
      Assert.AreEqual(3, o.Count);
      Assert.AreEqual(1, o[0].AsInt32());
      Assert.AreEqual(2, o[1].AsInt32());
      Assert.AreEqual(3, o[2].AsInt32());
      CBORTestCommon.AssertRoundTrip(o);
    }

    [Test]
    public void TestCBORInfinity() {
      {
        string stringTemp =
ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf)
            .ToString();
        Assert.AreEqual(
        "-Infinity",
        stringTemp);
      }
      {
        string stringTemp =
ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.RatPosInf)
            .ToString();
        Assert.AreEqual(
        "Infinity",
        stringTemp);
      }

  CBORTestCommon.AssertRoundTrip(ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf));

  CBORTestCommon.AssertRoundTrip(ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.RatPosInf));

  Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf)
                    .IsInfinity());

  Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.RatPosInf)
                    .IsInfinity());

  Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf)
                    .IsNegativeInfinity());

  Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.RatPosInf)
                    .IsPositiveInfinity());
      Assert.IsTrue(CBORObject.PositiveInfinity.IsPositiveInfinity());
      Assert.IsTrue(CBORObject.NegativeInfinity.IsNegativeInfinity());
      Assert.IsTrue(CBORObject.NaN.IsNaN());

      CBORTestCommon.AssertRoundTrip(
  ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecNegInf));

      CBORTestCommon.AssertRoundTrip(
  ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf));

      CBORTestCommon.AssertRoundTrip(
  ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity));

      CBORTestCommon.AssertRoundTrip(
  ToObjectTest.TestToFromObjectRoundTrip(Single.NegativeInfinity));

      CBORTestCommon.AssertRoundTrip(
  ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecPosInf));

      CBORTestCommon.AssertRoundTrip(
  ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatPosInf));

  CBORTestCommon.AssertRoundTrip(ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity));

  CBORTestCommon.AssertRoundTrip(ToObjectTest.TestToFromObjectRoundTrip(Single.PositiveInfinity));
    }

    [Test]
    public void TestCompareB() {
      {
  string stringTemp = CBORObject.DecodeFromBytes(new byte[] { (byte)0xfa, 0x7f,
        (byte)0x80, 0x00, 0x00 }).AsERational().ToString();
        Assert.AreEqual(
        "Infinity",
        stringTemp);
      }
      {
    CBORObject objectTemp = CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5,
  (byte)0x82, 0x38, (byte)0xc7, 0x3b, 0x00, 0x00, 0x08, (byte)0xbf,
  (byte)0xda, (byte)0xaf, 0x73, 0x46 });
   CBORObject objectTemp2 = CBORObject.DecodeFromBytes(new byte[] { 0x3b, 0x5a,
  (byte)0x9b, (byte)0x9a, (byte)0x9c, (byte)0xb4, (byte)0x95, (byte)0xbf,
  0x71 });
        AddSubCompare(objectTemp, objectTemp2);
      }
      {
    CBORObject objectTemp = CBORObject.DecodeFromBytes(new byte[] { (byte)0xfa,
  0x1f, (byte)0x80, (byte)0xdb, (byte)0x9b });
   CBORObject objectTemp2 = CBORObject.DecodeFromBytes(new byte[] { (byte)0xfb,
  0x31, (byte)0x90, (byte)0xea, 0x16, (byte)0xbe, (byte)0x80, 0x0b, 0x37 });
        AddSubCompare(objectTemp, objectTemp2);
      }
      CBORObject cbor = CBORObject.FromObjectAndTag(
        Double.NegativeInfinity,
        1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor =

        CBORObject.FromObjectAndTag(
          ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor =

        CBORObject.FromObjectAndTag(
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor =

        CBORObject.FromObjectAndTag(
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecNegInf),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor =

        CBORObject.FromObjectAndTag(
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
    }

    [Test]
    public void TestDecFracCompareIntegerVsBigFraction() {
      CBORObject o1 = null;
      CBORObject o2 = null;
      o1 = CBORObject.DecodeFromBytes(new byte[] { (byte)0xfb, (byte)0x8b, 0x44,
        (byte)0xf2, (byte)0xa9, 0x0c, 0x27, 0x42, 0x28 });
      o2 = CBORObject.DecodeFromBytes(new byte[] { (byte)0xc5, (byte)0x82, 0x38,
        (byte)0xa4, (byte)0xc3, 0x50, 0x02, (byte)0x98, (byte)0xc5, (byte)0xa8,
        0x02, (byte)0xc1, (byte)0xf6, (byte)0xc0, 0x1a, (byte)0xbe, 0x08,
          0x04, (byte)0x86, (byte)0x99, 0x3e, (byte)0xf1 });
      AddSubCompare(o1, o2);
    }

    [Test]
    public void TestDecimalFrac() {
      CBORTestCommon.FromBytesTestAB(
        new byte[] { 0xc4, 0x82, 0x3, 0x1a, 1, 2, 3, 4 });
    }
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestDecimalFracExactlyTwoElements() {
      CBORTestCommon.FromBytesTestAB(new byte[] { 0xc4, 0x82, 0xc2, 0x41, 1 });
    }
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestDecimalFracExponentMustNotBeBignum() {
      CBORTestCommon.FromBytesTestAB(new byte[] { 0xc4, 0x82, 0xc2, 0x41, 1,
        0x1a,
        1, 2, 3, 4 });
    }
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestBigFloatExponentMustNotBeBignum() {
      CBORTestCommon.FromBytesTestAB(new byte[] { 0xc5, 0x82, 0xc2, 0x41, 1,
        0x1a,
        1, 2, 3, 4 });
    }

    [Test]
    public void TestDecimalFracMantissaMayBeBignum() {
      CBORObject o = CBORTestCommon.FromBytesTestAB(
        new byte[] { 0xc4, 0x82, 0x3, 0xc2, 0x41, 1 });
      Assert.AreEqual(
        EDecimal.FromString("1e3"),
        o.AsEDecimal());
    }

    [Test]
    public void TestBigFloatFracMantissaMayBeBignum() {
      CBORObject o = CBORTestCommon.FromBytesTestAB(
        new byte[] { 0xc5, 0x82, 0x3, 0xc2, 0x41, 1 });
      {
        long numberTemp = EFloat.FromString("8").CompareTo(o.AsEFloat());
        Assert.AreEqual(0, numberTemp);
      }
    }

    [Test]
    public void TestDivide() {
      var r = new RandomGenerator();
      for (var i = 0; i < 3000; ++i) {
        CBORObject o1 =
  ToObjectTest.TestToFromObjectRoundTrip(RandomObjects.RandomEInteger(r));
        CBORObject o2 =
    ToObjectTest.TestToFromObjectRoundTrip(RandomObjects.RandomEInteger(r));
        if (o2.IsZero) {
          continue;
        }
        ERational er = ERational.Create(o1.AsEInteger(), o2.AsEInteger());
        {
          ERational objectTemp = er;
          ERational objectTemp2 = CBORObject.Divide(
  o1,
  o2).AsERational();
          TestCommon.CompareTestEqual(objectTemp, objectTemp2);
        }
      }
    }

    [Test]
    public void TestDouble() {
      if
(!ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity)
          .IsPositiveInfinity()) {
        Assert.Fail("Not positive infinity");
      }

  Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity)
      .AsEDecimal().IsPositiveInfinity());

  Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity)
      .AsEDecimal().IsNegativeInfinity());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(Double.NaN)
              .AsEDecimal().IsNaN());
      CBORObject oldobj = null;
      for (int i = -65539; i <= 65539; ++i) {
        CBORObject o = ToObjectTest.TestToFromObjectRoundTrip((double)i);
        Assert.IsTrue(o.CanFitInDouble());
        Assert.IsTrue(o.CanFitInInt32());
        Assert.IsTrue(o.IsIntegral);
        CBORTestCommon.AssertJSONSer(
          o,
          TestCommon.IntToString(i));
        if (oldobj != null) {
          TestCommon.CompareTestLess(oldobj, o);
        }
        oldobj = o;
      }
    }

    [Test]
    public void TestExample() {
      // The following creates a CBOR map and adds
      // several kinds of objects to it
      CBORObject cbor = CBORObject.NewMap().Add("item", "any string")
        .Add("number", 42).Add("map", CBORObject.NewMap().Add("number", 42))
        .Add("array", CBORObject.NewArray().Add(999f).Add("xyz"))
        .Add("bytes", new byte[] { 0, 1, 2 });
      // The following converts the map to CBOR
      cbor.EncodeToBytes();
      // The following converts the map to JSON
      cbor.ToJSONString();
    }

    [Test]
    [Timeout(5000)]
    public void TestExtendedExtremeExponent() {
      // Values with extremely high or extremely low exponents;
      // we just check whether this test method runs reasonably fast
      // for all these test cases
      CBORObject obj;
      obj = CBORObject.DecodeFromBytes(new byte[] { (byte)0xc4, (byte)0x82,
     0x3a, 0x00, 0x1c, 0x2d, 0x0d, 0x1a, 0x13, 0x6c, (byte)0xa1, (byte)0x97 });
      CBORTestCommon.AssertRoundTrip(obj);
      obj = CBORObject.DecodeFromBytes(new byte[] { (byte)0xda, 0x00, 0x14,
        0x57, (byte)0xce, (byte)0xc5, (byte)0x82, 0x1a, 0x46, 0x5a, 0x37,
        (byte)0x87, (byte)0xc3, 0x50, 0x5e, (byte)0xec, (byte)0xfd, 0x73,
          0x50, 0x64, (byte)0xa1, 0x1f, 0x10, (byte)0xc4, (byte)0xff,
          (byte)0xf2, (byte)0xc4, (byte)0xc9, 0x65, 0x12 });
      CBORTestCommon.AssertRoundTrip(obj);
      int actual = ToObjectTest.TestToFromObjectRoundTrip(
        EDecimal.FromString("333333e-2"))
        .CompareTo(ToObjectTest.TestToFromObjectRoundTrip(EFloat.Create(
          EInteger.FromString("5234222"),
          EInteger.FromString("-24936668661488"))));
      Assert.AreEqual(1, actual);
    }

    [Test]
    public void TestFloat() {
  Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(Single.PositiveInfinity)
      .AsEDecimal().IsPositiveInfinity());

  Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(Single.NegativeInfinity)
      .AsEDecimal().IsNegativeInfinity());
      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(Single.NaN)
              .AsEDecimal().IsNaN());
      for (int i = -65539; i <= 65539; ++i) {
        CBORTestCommon.AssertJSONSer(
          ToObjectTest.TestToFromObjectRoundTrip((float)i),
          TestCommon.IntToString(i));
      }
    }
    [Test]
    public void TestHalfPrecision() {
      CBORObject o = CBORObject.DecodeFromBytes(
        new byte[] { 0xf9, 0x7c, 0x00 });
      Assert.AreEqual(Single.PositiveInfinity, o.AsSingle());
      o = CBORObject.DecodeFromBytes(
        new byte[] { 0xf9, 0x00, 0x00 });
      Assert.AreEqual((float)0, o.AsSingle());
      o = CBORObject.DecodeFromBytes(
        new byte[] { 0xf9, 0xfc, 0x00 });
      Assert.AreEqual(Single.NegativeInfinity, o.AsSingle());
      o = CBORObject.DecodeFromBytes(
        new byte[] { 0xf9, 0x7e, 0x00 });
      Assert.IsTrue(Single.IsNaN(o.AsSingle()));
    }

    [Test]
    public void TestJSON() {
      CBORObject o;
      o = CBORObject.FromJSONString("[1,2,null,true,false,\"\"]");
      Assert.AreEqual(6, o.Count);
      Assert.AreEqual(1, o[0].AsInt32());
      Assert.AreEqual(2, o[1].AsInt32());
      Assert.AreEqual(CBORObject.Null, o[2]);
      Assert.AreEqual(CBORObject.True, o[3]);
      Assert.AreEqual(CBORObject.False, o[4]);
      Assert.AreEqual(String.Empty, o[5].AsString());
      o = CBORObject.FromJSONString("[1.5,2.6,3.7,4.0,222.22]");
      double actual = o[0].AsDouble();
      Assert.AreEqual((double)1.5, actual);
      using (var ms2a = new MemoryStream(new byte[] { })) {
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
      using (var ms2b = new MemoryStream(new byte[] { 0x20 })) {
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
      try {
        CBORObject.FromJSONString(String.Empty);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[.1]");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("[-.1]");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromJSONString("\u0020");
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      {
        string stringTemp = CBORObject.FromJSONString("true").ToJSONString();
        Assert.AreEqual(
        "true",
        stringTemp);
      }
      {
        string stringTemp = CBORObject.FromJSONString(" true ").ToJSONString();
        Assert.AreEqual(
        "true",
        stringTemp);
      }
      {
        string stringTemp = CBORObject.FromJSONString("false").ToJSONString();
        Assert.AreEqual(
        "false",
        stringTemp);
      }
      {
        string stringTemp = CBORObject.FromJSONString("null").ToJSONString();
        Assert.AreEqual(
        "null",
        stringTemp);
      }
      {
        string stringTemp = CBORObject.FromJSONString("5").ToJSONString();
        Assert.AreEqual(
        "5",
        stringTemp);
      }
    }

    [Test]
    public void TestJSONEscapedChars() {
      CBORObject o = CBORObject.FromJSONString(
        "[\"\\r\\n\\u0006\\u000E\\u001A\\\\\\\"\"]");
      Assert.AreEqual(1, o.Count);
      {
        string stringTemp = o[0].AsString();
        Assert.AreEqual(
        "\r\n\u0006\u000E\u001A\\\"",
        stringTemp);
      }
      {
        string stringTemp = o.ToJSONString();
        Assert.AreEqual(
        "[\"\\r\\n\\u0006\\u000E\\u001A\\\\\\\"\"]",
        stringTemp);
      }
      CBORTestCommon.AssertRoundTrip(o);
    }

    [Test]
    public void TestLong() {
      long[] ranges = {
        -65539, 65539, 0xfffff000L, 0x100000400L,
        Int64.MaxValue - 1000, Int64.MaxValue, Int64.MinValue, Int64.MinValue +
          1000 };
      for (var i = 0; i < ranges.Length; i += 2) {
        long j = ranges[i];
        while (true) {
          Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(j).IsIntegral);
Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(j).CanFitInInt64());
          Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(j)
                    .CanTruncatedIntFitInInt64());
          CBORTestCommon.AssertJSONSer(
            ToObjectTest.TestToFromObjectRoundTrip(j),
            TestCommon.LongToString(j));
          Assert.AreEqual(
            ToObjectTest.TestToFromObjectRoundTrip(j),
            ToObjectTest.TestToFromObjectRoundTrip(EInteger.FromInt64(j)));
          CBORObject obj = CBORObject.FromJSONString(
            "[" + TestCommon.LongToString(j) + "]");
          CBORTestCommon.AssertJSONSer(
            obj,
            "[" + TestCommon.LongToString(j) + "]");
          if (j == ranges[i + 1]) {
            break;
          }
          ++j;
        }
      }
    }

    [Test]
    public void TestMap() {
      CBORObject cbor = CBORObject.FromJSONString("{\"a\":2,\"b\":4}");
      Assert.AreEqual(2, cbor.Count);
      TestCommon.AssertEqualsHashCode(
        ToObjectTest.TestToFromObjectRoundTrip(2),
        cbor[ToObjectTest.TestToFromObjectRoundTrip("a")]);
      TestCommon.AssertEqualsHashCode(
        ToObjectTest.TestToFromObjectRoundTrip(4),
        cbor[ToObjectTest.TestToFromObjectRoundTrip("b")]);
      {
        long numberTemp = cbor[ToObjectTest.TestToFromObjectRoundTrip(
          "a")].AsInt32();
        Assert.AreEqual(2, numberTemp);
      }
      {
        long numberTemp = cbor[ToObjectTest.TestToFromObjectRoundTrip(
          "b")].AsInt32();
        Assert.AreEqual(4, numberTemp);
      }
      Assert.AreEqual(0, CBORObject.True.Count);
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0xbf, 0x61, 0x61, 2,
                    0x61, 0x62, 4, 0xff });
      Assert.AreEqual(2, cbor.Count);
      TestCommon.AssertEqualsHashCode(
        ToObjectTest.TestToFromObjectRoundTrip(2),
        cbor[ToObjectTest.TestToFromObjectRoundTrip("a")]);
      TestCommon.AssertEqualsHashCode(
        ToObjectTest.TestToFromObjectRoundTrip(4),
        cbor[ToObjectTest.TestToFromObjectRoundTrip("b")]);
      {
        long numberTemp = cbor[ToObjectTest.TestToFromObjectRoundTrip(
          "a")].AsInt32();
        Assert.AreEqual(2, numberTemp);
      }
      {
        long numberTemp = cbor[ToObjectTest.TestToFromObjectRoundTrip(
          "b")].AsInt32();
        Assert.AreEqual(4, numberTemp);
      }
    }

    [Test]
    public void TestMapInMap() {
      CBORObject oo;
      oo = CBORObject.NewArray().Add(CBORObject.NewMap()
                    .Add(
              ERational.Create(EInteger.One, EInteger.FromString("2")),
              3).Add(4, false)).Add(true);
      CBORTestCommon.AssertRoundTrip(oo);
      oo = CBORObject.NewArray();
      oo.Add(ToObjectTest.TestToFromObjectRoundTrip(0));
      CBORObject oo2 = CBORObject.NewMap();
      oo2.Add(
  ToObjectTest.TestToFromObjectRoundTrip(1),
  ToObjectTest.TestToFromObjectRoundTrip(1368));
      CBORObject oo3 = CBORObject.NewMap();
      oo3.Add(
  ToObjectTest.TestToFromObjectRoundTrip(2),
  ToObjectTest.TestToFromObjectRoundTrip(1625));
      CBORObject oo4 = CBORObject.NewMap();
      oo4.Add(oo2, CBORObject.True);
      oo4.Add(oo3, CBORObject.True);
      oo.Add(oo4);
      CBORTestCommon.AssertRoundTrip(oo);
    }

    [Test]
    public void TestParseDecimalStrings() {
      var rand = new RandomGenerator();
      for (var i = 0; i < 3000; ++i) {
        string r = RandomObjects.RandomDecimalString(rand);
        TestDecimalString(r);
      }
    }

    [Test]
    [Timeout(50000)]
    public void TestRandomData() {
      var rand = new RandomGenerator();
      CBORObject obj;
      for (var i = 0; i < 1000; ++i) {
        obj = CBORTestCommon.RandomCBORObject(rand);
        CBORTestCommon.AssertRoundTrip(obj);
string jsonString = String.Empty;
try {
                  jsonString = obj.ToJSONString();
} catch (CBORException ex) {
jsonString = String.Empty;
}
if (jsonString.Length > 0) {
                  CBORObject.FromJSONString(jsonString);
                  TestWriteToJSON(obj);
}
      }
    }

    [Test]
    public void TestSharedRefValidInteger() {
      byte[] bytes;
      // Shared ref is integer
      bytes = new byte[] { 0x82, 0xd8, 0x1c, 0x00, 0xd8, 0x1d, 0x00 };
      try {
 CBORObject.DecodeFromBytes(bytes);
} catch (Exception ex) {
Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      // Shared ref is negative
      bytes = new byte[] { 0x82, 0xd8, 0x1c, 0x00, 0xd8, 0x1d, 0x20 };
      try {
 CBORObject.DecodeFromBytes(bytes);
Assert.Fail("Should have failed");
} catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      // Shared ref is non-integer
      bytes = new byte[] { 0x82, 0xd8, 0x1c, 0x00, 0xd8, 0x1d, 0xc4, 0x82,
        0x27, 0x19, 0xff, 0xff };
      try {
 CBORObject.DecodeFromBytes(bytes);
Assert.Fail("Should have failed");
} catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      // Shared ref is non-number
      bytes = new byte[] { 0x82, 0xd8, 0x1c, 0x00, 0xd8, 0x1d, 0x61, 0x41 };
      try {
 CBORObject.DecodeFromBytes(bytes);
Assert.Fail("Should have failed");
} catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      // Shared ref is out of range
      bytes = new byte[] { 0x82, 0xd8, 0x1c, 0x00, 0xd8, 0x1d, 0x01 };
      try {
 CBORObject.DecodeFromBytes(bytes);
Assert.Fail("Should have failed");
} catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
    }

    private static string ToByteArrayStringFrom(byte[] array, int pos) {
      var newArray = new byte[array.Length - pos];
      Array.Copy(array, pos, newArray, 0, newArray.Length);
      return TestCommon.ToByteArrayString(newArray);
    }

    [Test]
    [Timeout(500000)]
    public void TestRandomNonsense() {
      var rand = new RandomGenerator();
      for (var i = 0; i < 1000; ++i) {
        var array = new byte[rand.UniformInt(1000000) + 1];
        // array = new byte[rand.UniformInt(500) + 1];  // TEMP
        for (int j = 0; j < array.Length; ++j) {
          if (j + 3 <= array.Length) {
            int r = rand.UniformInt(0x1000000);
            array[j] = (byte)(r & 0xff);
            array[j + 1] = (byte)((r >> 8) & 0xff);
            array[j + 2] = (byte)((r >> 16) & 0xff);
            j += 2;
          } else {
            array[j] = (byte)rand.UniformInt(256);
          }
        }
        using (var ms = new MemoryStream(array)) {
          var iobj = 0;
          while (iobj < 25 && ms.Position != ms.Length) {
            ++iobj;
            var objpos = (int)ms.Position;
            try {
              CBORObject o = CBORObject.Read(ms);
              try {
                if (o == null) {
                  Assert.Fail("object read is null");
                } else {
                  CBORObject.DecodeFromBytes(o.EncodeToBytes());
                }
              } catch (Exception ex) {
                string failString = ex.ToString() +
                  (ex.InnerException == null ? String.Empty : "\n" +
                    ex.InnerException.ToString()) +
                  "\n" + ToByteArrayStringFrom(array, objpos);
                Assert.Fail(failString);
                throw new InvalidOperationException(String.Empty, ex);
              }
              String jsonString = String.Empty;
              try {
                if (o.Type == CBORType.Array || o.Type == CBORType.Map) {
try {
                  jsonString = o.ToJSONString();
} catch (CBORException ex) {
jsonString = String.Empty;
}
if (jsonString.Length > 0) {
                  CBORObject.FromJSONString(jsonString);
                  TestWriteToJSON(o);
}
                }
              } catch (Exception ex) {
                string failString = jsonString + "\n" + ex.ToString() +
                  (ex.InnerException == null ? String.Empty : "\n" +
                    ex.InnerException.ToString()) +
                  "\n" + ToByteArrayStringFrom(o.EncodeToBytes(), 0);
                Assert.Fail(failString);
                throw new InvalidOperationException(String.Empty, ex);
              }
            } catch (CBORException) {
              new Object();  // Expected exception
            } catch (Exception ex) {
              // if (!ex.Message.Equals("Not a number type")) {
              string failString = ex.ToString() +
                (ex.InnerException == null ? String.Empty : "\n" +
                  ex.InnerException.ToString()) +
                "\n" + ToByteArrayStringFrom(array, objpos);
              Assert.Fail(failString);
              throw new InvalidOperationException(String.Empty, ex);
              // }
            }
          }
        }
      }
    }

    [Test]
    [Timeout(20000)]
    public void TestRandomSlightlyModified() {
      var rand = new RandomGenerator();
      // Test slightly modified objects
      for (var i = 0; i < 200; ++i) {
        CBORObject originalObject = CBORTestCommon.RandomCBORObject(rand);
        byte[] array = originalObject.EncodeToBytes();
        // Console.WriteLine(originalObject);
        int count2 = rand.UniformInt(10) + 1;
        for (int j = 0; j < count2; ++j) {
          int index = rand.UniformInt(array.Length);
          array[index] = unchecked((byte)rand.UniformInt(256));
        }
        using (var inputStream = new MemoryStream(array)) {
          while (inputStream.Position != inputStream.Length) {
            try {
              CBORObject o = CBORObject.Read(inputStream);
              byte[] encodedBytes = (o == null) ? null : o.EncodeToBytes();
              try {
                CBORObject.DecodeFromBytes(encodedBytes);
              } catch (Exception ex) {
                string failString = ex.ToString() +
              (ex.InnerException == null ? String.Empty : "\n" +
                ex.InnerException.ToString());
                failString += "\n" + TestCommon.ToByteArrayString(array);
                Assert.Fail(failString);
                throw new InvalidOperationException(String.Empty, ex);
              }
              String jsonString = String.Empty;
              try {
                if (o == null) {
                  Assert.Fail("object is null");
                }
                if (o != null && (o.Type == CBORType.Array || o.Type ==
                    CBORType.Map)) {
                  jsonString = o.ToJSONString();
                  // reread JSON string to test validity
                  CBORObject.FromJSONString(jsonString);
                  TestWriteToJSON(o);
                }
              } catch (Exception ex) {
                string failString = jsonString + "\n" + ex +
              (ex.InnerException == null ? String.Empty : "\n" +
                ex.InnerException.ToString());
                failString += "\n" + TestCommon.ToByteArrayString(array);
                Assert.Fail(failString);
                throw new InvalidOperationException(String.Empty, ex);
              }
            } catch (CBORException ex) {
              // Expected exception
              Console.Write(ex.Message.Substring(0, 0));
            } catch (Exception ex) {
              string failString = ex.ToString() +
            (ex.InnerException == null ? String.Empty : "\n" +
              ex.InnerException.ToString());
              failString += "\n" + TestCommon.ToByteArrayString(array);
              Assert.Fail(failString);
              throw new InvalidOperationException(String.Empty, ex);
            }
          }
        }
      }
    }

    [Test]
    public void TestReadWriteInt() {
      var r = new RandomGenerator();
      try {
        for (var i = 0; i < 100000; ++i) {
          int val = unchecked((int)RandomObjects.RandomInt64(r));
          {
            using (var ms = new MemoryStream()) {
              MiniCBOR.WriteInt32(val, ms);
              using (var ms2 = new MemoryStream(ms.ToArray())) {
                Assert.AreEqual(val, MiniCBOR.ReadInt32(ms2));
              }
            }
          }
          {
            using (var ms = new MemoryStream()) {
              CBORObject.Write(val, ms);
              using (var ms2 = new MemoryStream(ms.ToArray())) {
                Assert.AreEqual(val, MiniCBOR.ReadInt32(ms2));
              }
            }
          }
        }
      } catch (IOException ioex) {
        Assert.Fail(ioex.Message);
      }
    }

    [Test]
    public void TestShort() {
      for (int i = Int16.MinValue; i <= Int16.MaxValue; ++i) {
        CBORTestCommon.AssertJSONSer(
          ToObjectTest.TestToFromObjectRoundTrip((short)i),
          TestCommon.IntToString(i));
      }
    }

    [Test]
    public void TestSimpleValues() {
      CBORTestCommon.AssertJSONSer(
        ToObjectTest.TestToFromObjectRoundTrip(true),
        "true");
      CBORTestCommon.AssertJSONSer(
        ToObjectTest.TestToFromObjectRoundTrip(false),
        "false");
      CBORTestCommon.AssertJSONSer(
        ToObjectTest.TestToFromObjectRoundTrip((object)null),
        "null");
    }

    [Test]
    public void TestSubtract() {
      var r = new RandomGenerator();
      for (var i = 0; i < 3000; ++i) {
        CBORObject o1 = CBORTestCommon.RandomNumber(r);
        CBORObject o2 = CBORTestCommon.RandomNumber(r);
        EDecimal cmpDecFrac = AsED(o1).Subtract(AsED(o2));
        EDecimal cmpCobj = AsED(CBORObject.Subtract(o1, o2));
        TestCommon.CompareTestEqual(cmpDecFrac, cmpCobj);
        CBORTestCommon.AssertRoundTrip(o1);
        CBORTestCommon.AssertRoundTrip(o2);
      }
    }

    [Test]
    public void TestTaggedUntagged() {
      for (int i = 200; i < 1000; ++i) {
        if (i == 264 || i == 265 || i + 1 == 264 || i + 1 == 265) {
          // Skip since they're being used as
          // arbitrary-precision numbers
          continue;
        }
        CBORObject o, o2;
        o = ToObjectTest.TestToFromObjectRoundTrip(0);
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o =
  ToObjectTest.TestToFromObjectRoundTrip(EInteger.FromString(
  "999999999999999999999999999999999"));
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = ToObjectTest.TestToFromObjectRoundTrip(new byte[] { 1, 2, 3 });
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.NewArray();
        o.Add(ToObjectTest.TestToFromObjectRoundTrip(0));
        o.Add(ToObjectTest.TestToFromObjectRoundTrip(1));
        o.Add(ToObjectTest.TestToFromObjectRoundTrip(2));
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.NewMap();
        o.Add("a", ToObjectTest.TestToFromObjectRoundTrip(0));
        o.Add("b", ToObjectTest.TestToFromObjectRoundTrip(1));
        o.Add("c", ToObjectTest.TestToFromObjectRoundTrip(2));
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = ToObjectTest.TestToFromObjectRoundTrip("a");
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.False;
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.True;
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.Null;
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.Undefined;
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
      }
    }

    [Test]
    public void TestTags() {
      EInteger maxuint = EInteger.FromString("18446744073709551615");
      EInteger[] ranges = {
        EInteger.FromString("37"),
        EInteger.FromString("65539"),
       EInteger.FromString("2147483147"),
  EInteger.FromString("2147484147"),
  EInteger.FromString("9223372036854775307"),
  EInteger.FromString("9223372036854776307"),
  EInteger.FromString("18446744073709551115"),
  EInteger.FromString("18446744073709551615") };
      Assert.IsFalse(CBORObject.True.IsTagged);
      CBORObject trueObj = CBORObject.True;
      Assert.AreEqual(
        EInteger.FromString("-1"),
        trueObj.MostInnerTag);
      EInteger[] tagstmp = CBORObject.True.GetAllTags();
      Assert.AreEqual(0, tagstmp.Length);
      for (var i = 0; i < ranges.Length; i += 2) {
        EInteger bigintTemp = ranges[i];
        while (true) {
          EInteger ei = bigintTemp;
          EInteger bigintNext = ei.Add(EInteger.One);
          if (bigintTemp.GetSignedBitLength() <= 31) {
            int bc = ei.ToInt32Checked();
            if (bc >= -1 && bc <= 37) {
              bigintTemp = bigintNext;
              continue;
            }
            if (bc == 264 || bc == 265) {
              bigintTemp = bigintNext;
              continue;
            }
          }
          CBORObject obj = CBORObject.FromObjectAndTag(0, bigintTemp);
          Assert.IsTrue(obj.IsTagged, "obj not tagged");
          EInteger[] tags = obj.GetAllTags();
          Assert.AreEqual(1, tags.Length);
          Assert.AreEqual(bigintTemp, tags[0]);
          if (!obj.MostInnerTag.Equals(bigintTemp)) {
            string errmsg = "obj tag doesn't match: " + obj;
            Assert.AreEqual(
              bigintTemp,
              obj.MostInnerTag,
              errmsg);
          }
          tags = obj.GetAllTags();
          Assert.AreEqual(1, tags.Length);
          Assert.AreEqual(bigintTemp, obj.MostOuterTag);
          Assert.AreEqual(bigintTemp, obj.MostInnerTag);
          Assert.AreEqual(0, obj.AsInt32()); if (!bigintTemp.Equals(maxuint)) {
            EInteger bigintNew = bigintNext;
            if (bigintNew.Equals(EInteger.FromString("264")) ||
                bigintNew.Equals(EInteger.FromString("265"))) {
              bigintTemp = bigintNext;
              continue;
            }
            // Test multiple tags
            CBORObject obj2 = CBORObject.FromObjectAndTag(obj, bigintNew);
            EInteger[] bi = obj2.GetAllTags();
            if (bi.Length != 2) {
              {
                string stringTemp = "Expected 2 tags: " + obj2;
                Assert.AreEqual(
                    2,
                    bi.Length,
                    stringTemp);
              }
            }
            bigintNew = bigintNext;
            TestCommon.CompareTestEqualAndConsistent(
  bi[0],
  bigintNew,
  "Outer tag doesn't match");
            TestCommon.CompareTestEqualAndConsistent(
  bi[1],
  bigintTemp,
  "Inner tag doesn't match");
            if (!obj2.MostInnerTag.Equals((object)bigintTemp)) {
              {
                string stringTemp = "Innermost tag doesn't match: " + obj2;
                Assert.AreEqual(
                    bigintTemp,
                    obj2.MostInnerTag,
                    stringTemp);
              }
            }
            EInteger[] tags2 = obj2.GetAllTags();
            Assert.AreEqual(2, tags2.Length);
            Assert.AreEqual(bigintNext, obj2.MostOuterTag);
            Assert.AreEqual(bigintTemp, obj2.MostInnerTag);
            Assert.AreEqual(0, obj2.AsInt32());
          }
          if (bigintTemp.Equals(ranges[i + 1])) {
            break;
          }
          bigintTemp = bigintNext;
        }
      }
    }

    [Test]
    public void TestOverlongSimpleValues() {
      for (var i = 0; i <= 0x1f; ++i) {
        var bytes = new byte[] { (byte)0xf8, (byte)i };
        try {
          CBORObject.DecodeFromBytes(bytes);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    }

    [Test]
    public void TestTags264And265() {
      CBORObject cbor;
      // Tag 264
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0xd9, 0x01, 0x08, 0x82,
        0xc2, 0x42, 2, 2, 0xc2, 0x42, 2, 2 });
      CBORTestCommon.AssertRoundTrip(cbor);
      // Tag 265
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0xd9, 0x01, 0x09, 0x82,
        0xc2, 0x42, 2, 2, 0xc2, 0x42, 2, 2 });
      CBORTestCommon.AssertRoundTrip(cbor);
    }
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestTagThenBreak() {
      CBORTestCommon.FromBytesTestAB(new byte[] { 0xd1, 0xff });
    }

    [Test]
    public void TestTextStringStream() {
      CBORObject cbor = CBORTestCommon.FromBytesTestAB(
        new byte[] { 0x7f, 0x61, 0x2e, 0x61, 0x2e, 0xff });
      {
        string stringTemp = cbor.AsString();
        Assert.AreEqual(
        "..",
        stringTemp);
      }
      TestTextStringStreamOne(TestCommon.Repeat('x', 200000));
      TestTextStringStreamOne(TestCommon.Repeat('\u00e0', 200000));
      TestTextStringStreamOne(TestCommon.Repeat('\u3000', 200000));
      TestTextStringStreamOne(TestCommon.Repeat("\ud800\udc00", 200000));
    }
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestTextStringStreamNoIndefiniteWithinDefinite() {
      CBORTestCommon.FromBytesTestAB(new byte[] { 0x7f, 0x61, 0x20, 0x7f, 0x61,
        0x20, 0xff, 0xff });
    }
    [Test]
    [ExpectedException(typeof(CBORException))]
    public void TestTextStringStreamNoTagsBeforeDefinite() {
      CBORTestCommon.FromBytesTestAB(new byte[] { 0x7f, 0x61, 0x20, 0xc0, 0x61,
        0x20, 0xff });
    }

    private static EDecimal AsED(CBORObject obj) {
      return EDecimal.FromString(
        obj.AsEDecimal().ToString());
    }

    private static void AddSubCompare(CBORObject o1, CBORObject o2) {
      EDecimal cmpDecFrac = AsED(o1).Add(AsED(o2));
      EDecimal cmpCobj = AsED(CBORObject.Addition(o1, o2));
      TestCommon.CompareTestEqual(cmpDecFrac, cmpCobj);
      cmpDecFrac = AsED(o1).Subtract(AsED(o2));
      cmpCobj = AsED(CBORObject.Subtract(o1, o2));
      TestCommon.CompareTestEqual(cmpDecFrac, cmpCobj);
      CBORObjectTest.CompareDecimals(o1, o2);
    }

    private static void TestDecimalString(String r) {
 CBORObject o = ToObjectTest.TestToFromObjectRoundTrip(EDecimal.FromString(r));
      CBORObject o2 = CBORDataUtilities.ParseJSONNumber(r);
      TestCommon.CompareTestEqual(o, o2);
    }

    private static void TestTextStringStreamOne(string longString) {
      CBORObject cbor, cbor2;
      cbor = ToObjectTest.TestToFromObjectRoundTrip(longString);
      cbor2 = CBORTestCommon.FromBytesTestAB(cbor.EncodeToBytes());
      Assert.AreEqual(
        longString,
        CBORObject.DecodeFromBytes(cbor.EncodeToBytes()).AsString());
      {
        object objectTemp = longString;
        object objectTemp2 = CBORObject.DecodeFromBytes(cbor.EncodeToBytes(
                  new CBOREncodeOptions(false, true))).AsString();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      TestCommon.AssertEqualsHashCode(cbor, cbor2);
      Assert.AreEqual(longString, cbor2.AsString());
    }

    private static void TestWriteToJSON(CBORObject obj) {
      CBORObject objA = null;
      string jsonString = String.Empty;
      using (var ms = new MemoryStream()) {
        try {
          obj.WriteJSONTo(ms);
          jsonString = DataUtilities.GetUtf8String(
            ms.ToArray(),
            true);
          objA = CBORObject.FromJSONString(jsonString);
        } catch (CBORException ex) {
          throw new InvalidOperationException(jsonString, ex);
        } catch (IOException ex) {
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      CBORObject objB = CBORObject.FromJSONString(obj.ToJSONString());
      if (!objA.Equals(objB)) {
        Console.WriteLine(objA);
        Console.WriteLine(objB);
        Assert.Fail("WriteJSONTo gives different results from ToJSONString");
      }
    }
  }
}
