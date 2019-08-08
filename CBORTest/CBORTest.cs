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
      Assert.IsTrue(cbor.ContainsKey(CBORObject.FromObject(1)));
      Assert.AreEqual((int)2, cbor[CBORObject.FromObject(1)]);
      {
        string stringTemp = cbor.ToJSONString();
        Assert.AreEqual(
          "{\"1\":2}",
          stringTemp);
      }
      cbor.Add("hello", 2);
      Assert.IsTrue(cbor.ContainsKey("hello"));
      Assert.IsTrue(cbor.ContainsKey(CBORObject.FromObject("hello")));
      Assert.AreEqual((int)2, cbor["hello"]);
      cbor.Set(1, 3);
      Assert.IsTrue(cbor.ContainsKey(CBORObject.FromObject(1)));
      Assert.AreEqual((int)3, cbor[CBORObject.FromObject(1)]);
    }

    [Test]
    public void TestArray() {
      CBORObject cbor = CBORObject.FromJSONString("[]");
      cbor.Add(CBORObject.FromObject(3));
      cbor.Add(CBORObject.FromObject(4));
      byte[] bytes = cbor.EncodeToBytes();
      TestCommon.AssertByteArraysEqual(
        new byte[] { (byte)(0x80 | 2), 3, 4 },
        bytes);
      cbor = CBORObject.FromObject(new[] { "a", "b", "c", "d", "e" });
      Assert.AreEqual("[\"a\",\"b\",\"c\",\"d\",\"e\"]", cbor.ToJSONString());
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0x9f, 0, 1, 2, 3, 4, 5,
        6, 7, 0xff,
      });
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
          CBORObject.FromObject(bi),
          bi.ToString());
        Assert.IsTrue(CBORObject.FromObject(bi).IsIntegral);
        CBORTestCommon.AssertRoundTrip(CBORObject.FromObject(bi));
        CBORTestCommon.AssertRoundTrip(CBORObject.FromObject(
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
        EInteger.FromString("18446744073709552127"),
      };
      for (var i = 0; i < ranges.Length; i += 2) {
        EInteger bigintTemp = ranges[i];
        while (true) {
          CBORTestCommon.AssertJSONSer(
            CBORObject.FromObject(bigintTemp),
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
      o = CBORTestCommon.FromBytesTestAB(new byte[] {
        0xc2, 0x44, 0x88, 0x77,
        0x66,
        0x55,
      });
      Assert.AreEqual(
  EInteger.FromRadixString("88776655", 16),
  o.AsEInteger());
      o = CBORTestCommon.FromBytesTestAB(new byte[] {
        0xc2, 0x47, 0x88, 0x77,
        0x66,
        0x55, 0x44, 0x33, 0x22,
      });
      Assert.AreEqual(
  EInteger.FromRadixString("88776655443322", 16),
  o.AsEInteger());
    }

    [Test]
    public void TestByte() {
      for (var i = 0; i <= 255; ++i) {
        CBORTestCommon.AssertJSONSer(
          CBORObject.FromObject((byte)i),
          TestCommon.IntToString(i));
      }
    }

    [Test]
    public void TestByteArray() {
      {
        string stringTemp = CBORObject.FromObject(new byte[] {
          0x20, 0x78,
        }).ToString();
        Assert.AreEqual(
          "h'2078'",
          stringTemp);
}
    }

    [Test]
    public void TestByteStringStream() {
      CBORTestCommon.FromBytesTestAB(
        new byte[] { 0x5f, 0x41, 0x20, 0x41, 0x20, 0xff });
    }
    [Test]
    public void TestByteStringStreamNoIndefiniteWithinDefinite() {
      try {
        CBORTestCommon.FromBytesTestAB(new byte[] {
          0x5f, 0x41, 0x20, 0x5f,
          0x41,
          0x20, 0xff, 0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestByteStringStreamNoTagsBeforeDefinite() {
      try {
        CBORTestCommon.FromBytesTestAB(new byte[] {
          0x5f, 0x41, 0x20, 0xc2,
          0x41,
          0x20, 0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    public static string ObjectMessage(CBORObject obj) {
      if (obj == null) {
        throw new ArgumentNullException(nameof(obj));
      }
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
            if ((bi.GetSignedBitLengthAsEInteger().ToInt32Checked() <= 31) !=
ed.CanFitInInt32()) {
              Assert.Fail(ObjectMessage(ed));
            }
          }
          if ((bi.GetSignedBitLengthAsEInteger().ToInt32Checked() <= 31) !=
            ed.CanTruncatedIntFitInInt32()) {
            Assert.Fail(ObjectMessage(ed));
          }
          if (ed.IsIntegral) {
            if ((bi.GetSignedBitLengthAsEInteger().ToInt32Checked() <= 63) !=
ed.CanFitInInt64()) {
              Assert.Fail(ObjectMessage(ed));
            }
          }
          if ((bi.GetSignedBitLengthAsEInteger().ToInt32Checked() <= 63) !=
            ed.CanTruncatedIntFitInInt64()) {
            Assert.Fail(ObjectMessage(ed));
          }
        }
      }
    }

    [Test]
    public void TestCanFitInSpecificCases() {
      CBORObject cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xfb,
        0x41, (byte)0xe0, (byte)0x85, 0x48, 0x2d, 0x14, 0x47, 0x7a,
      }); // 2217361768.63373
      Assert.AreEqual(
  EInteger.FromString("2217361768"),
  cbor.AsEInteger());

      Assert.IsFalse(
        cbor.AsEInteger().GetSignedBitLengthAsEInteger().ToInt32Checked()
<= 31);
      Assert.IsFalse(cbor.CanTruncatedIntFitInInt32());
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc5, (byte)0x82,
        0x18, 0x2f, 0x32,
      }); // -2674012278751232
      Assert.AreEqual(52,
  cbor.AsEInteger().GetSignedBitLengthAsEInteger().ToInt32Checked());
      Assert.IsTrue(cbor.CanFitInInt64());
      Assert.IsFalse(CBORObject.FromObject(2554895343L).CanFitInSingle());
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc5, (byte)0x82,
        0x10, 0x38, 0x64,
      }); // -6619136
      Assert.AreEqual(EInteger.FromString("-6619136"), cbor.AsEInteger());
      Assert.AreEqual(-6619136, cbor.AsInt32());
      Assert.IsTrue(cbor.CanTruncatedIntFitInInt32());
    }

    [Test]
    public void TestCBOREInteger() {
      CBORObject o = CBORObject.DecodeFromBytes(new byte[] {
        0x3b, (byte)0xce,
        (byte)0xe2, 0x5a, 0x57, (byte)0xd8, 0x21, (byte)0xb9, (byte)0xa7,
      });
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
        CBORObject.FromObject(0).Remove(CBORObject.True);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(String.Empty).Remove(CBORObject.True);
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
        CBORObject.FromObject(String.Empty).AsEFloat();
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
          CBORObject.FromObject(CBORTestCommon.FloatNegInf).ToString();
        Assert.AreEqual(
          "-Infinity",
          stringTemp);
      }
      {
        string stringTemp =
          CBORObject.FromObject(CBORTestCommon.RatPosInf).ToString();
        Assert.AreEqual(
          "Infinity",
          stringTemp);
      }

      CBORTestCommon.AssertRoundTrip(
          CBORObject.FromObject(CBORTestCommon.FloatNegInf));

      CBORTestCommon.AssertRoundTrip(
          CBORObject.FromObject(CBORTestCommon.RatPosInf));
      Assert.IsTrue(CBORObject.FromObject(CBORTestCommon.FloatNegInf)
                    .IsInfinity());
      Assert.IsTrue(CBORObject.FromObject(CBORTestCommon.RatPosInf)
                    .IsInfinity());
      Assert.IsTrue(CBORObject.FromObject(CBORTestCommon.FloatNegInf)
                    .IsNegativeInfinity());
      Assert.IsTrue(CBORObject.FromObject(CBORTestCommon.RatPosInf)
                    .IsPositiveInfinity());
      Assert.IsTrue(CBORObject.PositiveInfinity.IsPositiveInfinity());
      Assert.IsTrue(CBORObject.NegativeInfinity.IsNegativeInfinity());
      Assert.IsTrue(CBORObject.NaN.IsNaN());

      CBORTestCommon.AssertRoundTrip(
          CBORObject.FromObject(CBORTestCommon.DecNegInf));

      CBORTestCommon.AssertRoundTrip(
          CBORObject.FromObject(CBORTestCommon.FloatNegInf));

      CBORTestCommon.AssertRoundTrip(
          CBORObject.FromObject(Double.NegativeInfinity));

      CBORTestCommon.AssertRoundTrip(
          CBORObject.FromObject(Single.NegativeInfinity));

      CBORTestCommon.AssertRoundTrip(
          CBORObject.FromObject(CBORTestCommon.DecPosInf));

      CBORTestCommon.AssertRoundTrip(
          CBORObject.FromObject(CBORTestCommon.FloatPosInf));

      CBORTestCommon.AssertRoundTrip(
          CBORObject.FromObject(Double.PositiveInfinity));

      CBORTestCommon.AssertRoundTrip(
          CBORObject.FromObject(Single.PositiveInfinity));
    }

    [Test]
    public void TestCompareB() {
      {
        string stringTemp = CBORObject.DecodeFromBytes(new byte[] {
          (byte)0xfa, 0x7f, (byte)0x80, 0x00, 0x00,
        }).AsERational().ToString();
        Assert.AreEqual(
          "Infinity",
          stringTemp);
      }
      {
        CBORObject objectTemp = CBORObject.DecodeFromBytes(new byte[] {
          (byte)0xc5, (byte)0x82, 0x38, (byte)0xc7, 0x3b, 0x00, 0x00, 0x08,
          (byte)0xbf, (byte)0xda, (byte)0xaf, 0x73, 0x46,
        });
        CBORObject objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          0x3b, 0x5a, (byte)0x9b, (byte)0x9a, (byte)0x9c, (byte)0xb4, (byte)0x95,
          (byte)0xbf, 0x71,
        });
        AddSubCompare(objectTemp, objectTemp2);
      }
      {
        CBORObject objectTemp = CBORObject.DecodeFromBytes(new byte[] {
          (byte)0xfa, 0x1f, (byte)0x80, (byte)0xdb, (byte)0x9b,
        });
        CBORObject objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          (byte)0xfb, 0x31, (byte)0x90, (byte)0xea, 0x16, (byte)0xbe, (byte)0x80,
          0x0b, 0x37,
        });
        AddSubCompare(objectTemp, objectTemp2);
      }
      CBORObject cbor = CBORObject.FromObjectAndTag(
        Double.NegativeInfinity,
        1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor =

        CBORObject.FromObjectAndTag(
          CBORObject.FromObject(Double.NegativeInfinity),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor =

        CBORObject.FromObjectAndTag(
          CBORObject.FromObject(CBORTestCommon.FloatNegInf),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor =

        CBORObject.FromObjectAndTag(
          CBORObject.FromObject(CBORTestCommon.DecNegInf),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor =

        CBORObject.FromObjectAndTag(
          CBORObject.FromObject(CBORTestCommon.FloatNegInf),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
    }

    [Test]
    public void TestDecFracCompareIntegerVsBigFraction() {
      CBORObject o1 = null;
      CBORObject o2 = null;
      o1 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xfb, (byte)0x8b, 0x44,
        (byte)0xf2, (byte)0xa9, 0x0c, 0x27, 0x42, 0x28,
      });
      o2 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc5, (byte)0x82, 0x38,
        (byte)0xa4, (byte)0xc3, 0x50, 0x02, (byte)0x98, (byte)0xc5, (byte)0xa8,
        0x02, (byte)0xc1, (byte)0xf6, (byte)0xc0, 0x1a, (byte)0xbe, 0x08,
        0x04, (byte)0x86, (byte)0x99, 0x3e, (byte)0xf1,
      });
      AddSubCompare(o1, o2);
    }

    [Test]
    public void TestDecimalFrac() {
      CBORTestCommon.FromBytesTestAB(
        new byte[] { 0xc4, 0x82, 0x3, 0x1a, 1, 2, 3, 4 });
    }
    [Test]
    public void TestDecimalFracExactlyTwoElements() {
      try {
        CBORTestCommon.FromBytesTestAB(new byte[] {
          0xc4, 0x82, 0xc2, 0x41,
          1,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestDecimalFracExponentMustNotBeBignum() {
      try {
        CBORTestCommon.FromBytesTestAB(new byte[] {
          0xc4, 0x82, 0xc2, 0x41, 1,
          0x1a,
          1, 2, 3, 4,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
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
    public void TestMultiply() {
      var r = new RandomGenerator();
      for (var i = 0; i < 3000; ++i) {
        CBORObject o1 = CBORTestCommon.RandomNumber(r);
        CBORObject o2 = CBORTestCommon.RandomNumber(r);
        EDecimal cmpDecFrac = AsED(o1).Multiply(AsED(o2));
        EDecimal cmpCobj = AsED(CBORObject.Multiply(o1, o2));
        if (cmpDecFrac.CompareTo(cmpCobj) != 0) {
          string msg = "o1=" + o1.ToString() + ", o2=" + o2.ToString() +
               ", " + AsED(o1) + ", " + AsED(o2) + ", cmpCobj=" +
cmpCobj.ToString() +
               ", cmpDecFrac=" + cmpDecFrac.ToString();
          TestCommon.CompareTestEqual(cmpDecFrac, cmpCobj, msg);
        }
        CBORTestCommon.AssertRoundTrip(o1);
        CBORTestCommon.AssertRoundTrip(o2);
      }
    }

    [Test]
    public void TestAdd() {
      var r = new RandomGenerator();
      for (var i = 0; i < 3000; ++i) {
        CBORObject o1 = CBORTestCommon.RandomNumber(r);
        CBORObject o2 = CBORTestCommon.RandomNumber(r);
        EDecimal cmpDecFrac = AsED(o1).Add(AsED(o2));
        EDecimal cmpCobj = AsED(CBORObject.Addition(o1, o2));
        if (cmpDecFrac.CompareTo(cmpCobj) != 0) {
          string msg = "o1=" + o1.ToString() + ", o2=" + o2.ToString() +
               ", " + AsED(o1) + ", " + AsED(o2) + ", cmpCobj=" +
cmpCobj.ToString() +
               ", cmpDecFrac=" + cmpDecFrac.ToString();
          TestCommon.CompareTestEqual(cmpDecFrac, cmpCobj, msg);
        }
        CBORTestCommon.AssertRoundTrip(o1);
        CBORTestCommon.AssertRoundTrip(o2);
      }
    }

    [Test]
    public void TestSubtract() {
      var r = new RandomGenerator();
      for (var i = 0; i < 3000; ++i) {
        CBORObject o1 = CBORTestCommon.RandomNumber(r);
        CBORObject o2 = CBORTestCommon.RandomNumber(r);
        EDecimal cmpDecFrac = AsED(o1).Subtract(AsED(o2));
        EDecimal cmpCobj = AsED(CBORObject.Subtract(o1, o2));
        if (cmpDecFrac.CompareTo(cmpCobj) != 0) {
          string msg = "o1=" + o1.ToString() + ", o2=" + o2.ToString() +
               ", " + AsED(o1) + ", " + AsED(o2) + ", cmpCobj=" +
cmpCobj.ToString() +
               ", cmpDecFrac=" + cmpDecFrac.ToString();
          TestCommon.CompareTestEqual(cmpDecFrac, cmpCobj, msg);
        }
        CBORTestCommon.AssertRoundTrip(o1);
        CBORTestCommon.AssertRoundTrip(o2);
      }
    }

    [Test]
    public void TestDivide() {
      var r = new RandomGenerator();
      for (var i = 0; i < 3000; ++i) {
        CBORObject o1 =
          CBORObject.FromObject(RandomObjects.RandomEInteger(r));
        CBORObject o2 = CBORObject.FromObject(
          RandomObjects.RandomEInteger(r));
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
        (!CBORObject.FromObject(Double.PositiveInfinity).IsPositiveInfinity()) {
        Assert.Fail("Not positive infinity");
      }
      AssertSer(
        CBORObject.FromObject(Double.PositiveInfinity),
        "Infinity");
      AssertSer(
        CBORObject.FromObject(Double.NegativeInfinity),
        "-Infinity");
      AssertSer(
        CBORObject.FromObject(Double.NaN),
        "NaN");
      CBORObject oldobj = null;
      for (int i = -65539; i <= 65539; ++i) {
        CBORObject o = CBORObject.FromObject((double)i);
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
      obj = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc4, (byte)0x82,
        0x3a, 0x00, 0x1c, 0x2d, 0x0d, 0x1a, 0x13, 0x6c, (byte)0xa1,
        (byte)0x97,
      });
      CBORTestCommon.AssertRoundTrip(obj);
      obj = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xda, 0x00, 0x14,
        0x57, (byte)0xce, (byte)0xc5, (byte)0x82, 0x1a, 0x46, 0x5a, 0x37,
        (byte)0x87, (byte)0xc3, 0x50, 0x5e, (byte)0xec, (byte)0xfd, 0x73,
        0x50, 0x64, (byte)0xa1, 0x1f, 0x10, (byte)0xc4, (byte)0xff, (byte)0xf2,
        (byte)0xc4, (byte)0xc9, 0x65, 0x12,
      });
      CBORTestCommon.AssertRoundTrip(obj);
      int actual = CBORObject.FromObject(
        EDecimal.FromString("333333e-2"))
        .CompareTo(CBORObject.FromObject(EFloat.Create(
          EInteger.FromString("5234222"),
          EInteger.FromString("-24936668661488"))));
      Assert.AreEqual(1, actual);
    }

    [Test]
    public void TestFloat() {
      AssertSer(
        CBORObject.FromObject(Single.PositiveInfinity),
        "Infinity");
      AssertSer(
        CBORObject.FromObject(Single.NegativeInfinity),
        "-Infinity");
      AssertSer(
        CBORObject.FromObject(Single.NaN),
        "NaN");
      for (int i = -65539; i <= 65539; ++i) {
        CBORTestCommon.AssertJSONSer(
          CBORObject.FromObject((float)i),
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
      Assert.AreEqual(0f, o.AsSingle());
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
        Int64.MaxValue - 1000, Int64.MaxValue, Int64.MinValue,
        Int64.MinValue + 1000,
      };
      for (var i = 0; i < ranges.Length; i += 2) {
        long j = ranges[i];
        while (true) {
          Assert.IsTrue(CBORObject.FromObject(j).IsIntegral);
          Assert.IsTrue(CBORObject.FromObject(j).CanFitInInt64());
          Assert.IsTrue(CBORObject.FromObject(j).CanTruncatedIntFitInInt64());
          CBORTestCommon.AssertJSONSer(
            CBORObject.FromObject(j),
            TestCommon.LongToString(j));
          Assert.AreEqual(
            CBORObject.FromObject(j),
            CBORObject.FromObject(EInteger.FromInt64(j)));
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
        CBORObject.FromObject(2),
        cbor[CBORObject.FromObject("a")]);
      TestCommon.AssertEqualsHashCode(
        CBORObject.FromObject(4),
        cbor[CBORObject.FromObject("b")]);
      Assert.AreEqual(2, cbor[CBORObject.FromObject("a")].AsInt32());
      Assert.AreEqual(4, cbor[CBORObject.FromObject("b")].AsInt32());
      Assert.AreEqual(0, CBORObject.True.Count);
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xbf, 0x61, 0x61, 2,
        0x61, 0x62, 4, 0xff,
      });
      Assert.AreEqual(2, cbor.Count);
      TestCommon.AssertEqualsHashCode(
        CBORObject.FromObject(2),
        cbor[CBORObject.FromObject("a")]);
      TestCommon.AssertEqualsHashCode(
        CBORObject.FromObject(4),
        cbor[CBORObject.FromObject("b")]);
      Assert.AreEqual(2, cbor[CBORObject.FromObject("a")].AsInt32());
      Assert.AreEqual(4, cbor[CBORObject.FromObject("b")].AsInt32());
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
      oo.Add(CBORObject.FromObject(0));
      CBORObject oo2 = CBORObject.NewMap();
      oo2.Add(CBORObject.FromObject(1), CBORObject.FromObject(1368));
      CBORObject oo3 = CBORObject.NewMap();
      oo3.Add(CBORObject.FromObject(2), CBORObject.FromObject(1625));
      CBORObject oo4 = CBORObject.NewMap();
      oo4.Add(oo2, CBORObject.True);
      oo4.Add(oo3, CBORObject.True);
      oo.Add(oo4);
      CBORTestCommon.AssertRoundTrip(oo);
    }

    [Test]
    public void TestAllowEmpty() {
      CBOREncodeOptions options;
      var bytes = new byte[0];
      options = new CBOREncodeOptions(String.Empty);
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      options = new CBOREncodeOptions("allowempty=true");
      Assert.AreEqual(null, CBORObject.DecodeFromBytes(bytes, options));
      using (var ms = new MemoryStream(bytes)) {
        options = new CBOREncodeOptions(String.Empty);
        try {
          CBORObject.Read(ms, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var ms = new MemoryStream(bytes)) {
        options = new CBOREncodeOptions("allowempty=true");
        Assert.AreEqual(null, CBORObject.Read(ms, options));
      }
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
        TestWriteToJSON(obj);
      }
    }

    [Test]
    [Timeout(200000)]
    public void TestRandomNonsense() {
      var rand = new RandomGenerator();
      for (var i = 0; i < 200; ++i) {
        var array = new byte[rand.UniformInt(1000000) + 1];
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
            try {
              CBORObject o = CBORObject.Read(ms);
              try {
                if (o == null) {
                  Assert.Fail("object read is null");
                } else {
                  CBORObject.DecodeFromBytes(o.EncodeToBytes());
                }
              } catch (Exception ex) {
                Assert.Fail(ex.ToString());
                throw new InvalidOperationException(String.Empty, ex);
              }
              String jsonString = String.Empty;
              try {
                if (o.Type == CBORType.Array || o.Type == CBORType.Map) {
                  jsonString = o.ToJSONString();
                  CBORObject.FromJSONString(jsonString);
                  TestWriteToJSON(o);
                }
              } catch (Exception ex) {
                Assert.Fail(jsonString + "\n" + ex);
                throw new InvalidOperationException(String.Empty, ex);
              }
            } catch (CBORException) {
              // NOTE: Expected exception
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
                Assert.Fail(ex.ToString());
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
                Assert.Fail(jsonString + "\n" + ex);
                throw new InvalidOperationException(String.Empty, ex);
              }
            } catch (CBORException ex) {
              // Expected exception
              Console.WriteLine(ex.Message);
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
          CBORObject.FromObject((short)i),
          TestCommon.IntToString(i));
      }
    }

    [Test]
    public void TestSimpleValues() {
      CBORTestCommon.AssertJSONSer(
        CBORObject.FromObject(true),
        "true");
      CBORTestCommon.AssertJSONSer(
        CBORObject.FromObject(false),
        "false");
      CBORTestCommon.AssertJSONSer(
        CBORObject.FromObject((object)null),
        "null");
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
        o = CBORObject.FromObject(0);
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o =
  CBORObject.FromObject(EInteger.FromString(
  "999999999999999999999999999999999"));
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObject(new byte[] { 1, 2, 3 });
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.NewArray();
        o.Add(CBORObject.FromObject(0));
        o.Add(CBORObject.FromObject(1));
        o.Add(CBORObject.FromObject(2));
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.NewMap();
        o.Add("a", CBORObject.FromObject(0));
        o.Add("b", CBORObject.FromObject(1));
        o.Add("c", CBORObject.FromObject(2));
        o2 = CBORObject.FromObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromObject("a");
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
        EInteger.FromString("18446744073709551615"),
      };
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
          if (bigintTemp.GetSignedBitLengthAsEInteger().ToInt32Checked() <=
31) {
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
          AssertSer(
            obj,
            bigintTemp.ToString() + "(0)");
          if (!bigintTemp.Equals(maxuint)) {
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
            String str = bigintNext.ToString() + "(" +
              bigintTemp.ToString() + "(0))";
            AssertSer(
              obj2,
              str);
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
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xd9, 0x01, 0x08, 0x82,
        0xc2, 0x42, 2, 2, 0xc2, 0x42, 2, 2,
      });
      CBORTestCommon.AssertRoundTrip(cbor);
      // Tag 265
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xd9, 0x01, 0x09, 0x82,
        0xc2, 0x42, 2, 2, 0xc2, 0x42, 2, 2,
      });
      CBORTestCommon.AssertRoundTrip(cbor);
    }
    [Test]
    public void TestTagThenBreak() {
      try {
        CBORTestCommon.FromBytesTestAB(new byte[] { 0xd1, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
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
    public void TestTextStringStreamNoIndefiniteWithinDefinite() {
      try {
        CBORTestCommon.FromBytesTestAB(new byte[] {
          0x7f, 0x61, 0x20, 0x7f,
          0x61,
          0x20, 0xff, 0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestTextStringStreamNoTagsBeforeDefinite() {
      try {
        CBORTestCommon.FromBytesTestAB(new byte[] {
          0x7f, 0x61, 0x20, 0xc0,
          0x61,
          0x20, 0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
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
      CBORObject o = CBORObject.FromObject(EDecimal.FromString(r));
      CBORObject o2 = CBORDataUtilities.ParseJSONNumber(r);
      TestCommon.CompareTestEqual(o, o2);
    }

    private static void TestTextStringStreamOne(string longString) {
      CBORObject cbor, cbor2;
      cbor = CBORObject.FromObject(longString);
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

    [Test]
    public void TestIntegerFloatingEquivalence() {
      CBORObject cbor;
      // 0 versus 0.0
      cbor = CBORObject.NewMap();
      cbor.Set((int)0, CBORObject.FromObject("testzero"));
      cbor.Set((double)0.0, CBORObject.FromObject("testpointzero"));
      Assert.AreEqual(2, cbor.Count);
      {
        string stringTemp = cbor[CBORObject.FromObject(0)].AsString();
        Assert.AreEqual(
          "testzero",
          stringTemp);
      }
      {
        string stringTemp = cbor[CBORObject.FromObject((double)0.0)].AsString();
        Assert.AreEqual(
          "testpointzero",
          stringTemp);
      }
      cbor = CBORObject.NewMap();
      cbor.Set((double)0.0, CBORObject.FromObject("testpointzero"));
      cbor.Set((int)0, CBORObject.FromObject("testzero"));
      Assert.AreEqual(2, cbor.Count);
      {
        string stringTemp = cbor[CBORObject.FromObject(0)].AsString();
        Assert.AreEqual(
          "testzero",
          stringTemp);
      }
      {
        string stringTemp = cbor[CBORObject.FromObject((double)0.0)].AsString();
        Assert.AreEqual(
          "testpointzero",
          stringTemp);
      }
      // 3 versus 3.0
      cbor = CBORObject.NewMap();
      cbor.Set((int)3, CBORObject.FromObject("testzero"));
      cbor.Set((double)3.0, CBORObject.FromObject("testpointzero"));
      Assert.AreEqual(2, cbor.Count);
      {
        string stringTemp = cbor[CBORObject.FromObject(3)].AsString();
        Assert.AreEqual(
          "testzero",
          stringTemp);
      }
      {
        string stringTemp = cbor[CBORObject.FromObject((double)3.0)].AsString();
        Assert.AreEqual(
          "testpointzero",
          stringTemp);
      }
      cbor = CBORObject.NewMap();
      cbor.Set((double)3.0, CBORObject.FromObject("testpointzero"));
      cbor.Set((int)3, CBORObject.FromObject("testzero"));
      Assert.AreEqual(2, cbor.Count);
      {
        string stringTemp = cbor[CBORObject.FromObject(3)].AsString();
        Assert.AreEqual(
          "testzero",
          stringTemp);
      }
      {
        string stringTemp = cbor[CBORObject.FromObject((double)3.0)].AsString();
        Assert.AreEqual(
          "testpointzero",
          stringTemp);
      }
    }

    public static void AssertSer(CBORObject o, String s) {
      if (o == null) {
        throw new ArgumentNullException(nameof(o));
      }
      if (s == null) {
        throw new ArgumentNullException(nameof(s));
      }
      if (!s.Equals(o.ToString(), StringComparison.Ordinal)) {
        Assert.AreEqual(s, o.ToString(), "o is not equal to s");
      }
     // Test round-tripping
      CBORObject o2 = CBORTestCommon.FromBytesTestAB(
        o.EncodeToBytes());
      if (!s.Equals(o2.ToString(), StringComparison.Ordinal)) {
        string msg = "o2 is not equal to s:\no = " +
          TestCommon.ToByteArrayString(o.EncodeToBytes()) +
          "\no2 = " + TestCommon.ToByteArrayString(o2.EncodeToBytes()) +
          "\no2string = " + o2.ToString();
        Assert.AreEqual(s, o2.ToString(), msg);
      }
      CBORTestCommon.TestNumber(o);
      TestCommon.AssertEqualsHashCode(o, o2);
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
