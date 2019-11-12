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
  [Test]
    public void TestLexOrderSpecific1() {
      var bytes1 = new byte[] {
        129, 165, 27, 0, 0, 65, 2, 0, 0, 144, 172, 71,
        125, 0, 14, 204, 3, 19, 214, 67, 93, 67, 70, 101, 123, 121, 96, 44,
        68, 69,
        158, 1, 193, 250, 21, 59, 122, 166, 24, 16, 141, 232, 48, 145, 97,
        72, 58,
        134, 85, 244, 83, 100, 92, 115, 76, 82, 99, 80, 122, 94,
      };
      var bytes2 = new byte[] {
        129, 165, 27, 0, 0, 127, 163, 0, 0, 137, 100,
        69, 167, 15, 101, 37, 18, 69, 230, 236, 57, 241, 146, 101, 120, 80,
        66, 115,
        64, 98, 91, 105, 100, 102, 102, 78, 106, 101, 82, 117, 82, 46, 80, 69,
        150,
        80, 162, 211, 214, 105, 122, 59, 65, 32, 80, 70, 47, 90, 113, 66, 187,
        69,
      };
      var bytes3 = new byte[] {
        129, 165, 67, 93, 67, 70, 101, 123, 121, 96,
        44, 68, 100, 92, 115, 76, 82, 99, 80, 122, 94, 27, 0, 0, 65, 2, 0,
        0, 144,
        172, 71, 125, 0, 14, 204, 3, 19, 214, 97, 72, 58, 134, 85, 244, 83,
        69, 158,
        1, 193, 250, 21, 59, 122, 166, 24, 16, 141, 232, 48, 145,
      };
      var bytes4 = new byte[] {
        129, 165, 27, 0, 0, 127, 163, 0, 0, 137, 100,
        69, 167, 15, 101, 37, 18, 98, 91, 105, 100, 102, 102, 78, 106, 69,
        230, 236,
        57, 241, 146, 101, 120, 80, 66, 115, 64, 105, 122, 59, 65, 32, 80,
        70, 47,
        90, 113, 66, 187, 69, 101, 82, 117, 82, 46, 80, 69, 150, 80, 162, 211,
        214,
      };
      CBORObject cbor1 = CBORObject.DecodeFromBytes(bytes1);
      CBORObject cbor2 = CBORObject.DecodeFromBytes(bytes2);
      CBORObject cbor3 = CBORObject.DecodeFromBytes(bytes3);
      CBORObject cbor4 = CBORObject.DecodeFromBytes(bytes4);
      TestCommon.CompareTestLess(cbor1, cbor2);
      TestCommon.CompareTestLess(cbor1, cbor4);
      TestCommon.CompareTestLess(cbor3, cbor2);
      TestCommon.CompareTestLess(cbor3, cbor4);
    }

    [Test]
    public void TestLexOrderSpecific2() {
      var bytes1 = new byte[] {
        129, 165, 59, 72, 110, 0, 0, 122, 250, 251,
        131, 71, 22, 187, 235, 209, 143, 30, 146, 69, 36, 230, 134, 20, 97,
        100, 78,
        112, 92, 101, 70, 54, 136, 203, 227, 188, 120, 64, 72, 58, 42, 171,
        177, 73,
        245, 198, 139, 99, 36, 116, 76, 101, 99, 109, 60, 113, 107, 70, 219,
        37, 80,
        108, 40, 133,
      };
      var bytes2 = new byte[] {
        129, 165, 67, 62, 217, 7, 69, 113, 188, 156,
        26, 34, 69, 32, 101, 130, 188, 201, 27, 122, 228, 0, 0, 0, 0, 186,
        9, 69,
        70, 71, 152, 50, 17, 67, 231, 129, 240, 100, 79, 116, 84, 81, 69, 188,
        114,
        227, 101, 209, 244, 103, 91, 37, 62, 59, 78, 124, 95,
      };
      var bytes3 = new byte[] {
        129, 165, 72, 58, 42, 171, 177, 73, 245, 198,
        139, 99, 36, 116, 76, 69, 36, 230, 134, 20, 97, 100, 78, 112, 92,
        101, 70,
        54, 136, 203, 227, 188, 120, 64, 59, 72, 110, 0, 0, 122, 250, 251,
        131, 71,
        22, 187, 235, 209, 143, 30, 146, 101, 99, 109, 60, 113, 107, 70,
        219, 37,
        80, 108, 40, 133,
      };
      var bytes4 = new byte[] {
        129, 165, 69, 70, 71, 152, 50, 17, 67, 231,
        129, 240, 100, 79, 116, 84, 81, 69, 188, 114, 227, 101, 209, 67, 62,
        217, 7,
        69, 113, 188, 156, 26, 34, 244, 103, 91, 37, 62, 59, 78, 124, 95,
        69, 32,
        101, 130, 188, 201, 27, 122, 228, 0, 0, 0, 0, 186, 9,
      };
      CBORObject cbor1 = CBORObject.DecodeFromBytes(bytes1);
      CBORObject cbor2 = CBORObject.DecodeFromBytes(bytes2);
      CBORObject cbor3 = CBORObject.DecodeFromBytes(bytes3);
      CBORObject cbor4 = CBORObject.DecodeFromBytes(bytes4);
      TestCommon.CompareTestLess(cbor1, cbor2);
      TestCommon.CompareTestLess(cbor1, cbor4);
      TestCommon.CompareTestLess(cbor3, cbor2);
      TestCommon.CompareTestLess(cbor3, cbor4);
    }

    [Test]
    public void TestLexOrderSpecific3() {
      var bytes1 = new byte[] {
        129, 165, 67, 62, 217, 7, 69, 113, 188, 156,
        26, 34, 69, 32, 101, 130, 188, 201, 27, 122, 228, 0, 0, 0, 0, 186,
        9, 69,
        70, 71, 152, 50, 17, 67, 231, 129, 240, 100, 79, 116, 84, 81, 69, 188,
        114,
        227, 101, 209, 244, 103, 91, 37, 62, 59, 78, 124, 95,
      };
      var bytes2 = new byte[] {
        129, 165, 67, 64, 196, 213, 217, 43, 37, 27,
        37, 184, 58, 144, 176, 207, 252, 194, 68, 43, 68, 5, 219, 27, 0, 0, 126,
        173, 36, 137, 166, 19, 69, 27, 99, 166, 37, 216, 101, 87, 91, 80,
        79, 100,
        69, 217, 77, 189, 138, 22, 101, 40, 93, 54, 59, 73, 97, 60, 99, 69,
        35, 66,
      };
      var bytes3 = new byte[] {
        129, 165, 69, 70, 71, 152, 50, 17, 67, 231,
        129, 240, 100, 79, 116, 84, 81, 69, 188, 114, 227, 101, 209, 67, 62,
        217, 7,
        69, 113, 188, 156, 26, 34, 244, 103, 91, 37, 62, 59, 78, 124, 95,
        69, 32,
        101, 130, 188, 201, 27, 122, 228, 0, 0, 0, 0, 186, 9,
      };
      var bytes4 = new byte[] {
        129, 165, 67, 64, 196, 213, 217, 43, 37, 27,
        37, 184, 58, 144, 176, 207, 252, 194, 69, 27, 99, 166, 37, 216, 101,
        87, 91,
        80, 79, 100, 97, 60, 99, 69, 35, 66, 69, 217, 77, 189, 138, 22, 101,
        40, 93,
        54, 59, 73, 68, 43, 68, 5, 219, 27, 0, 0, 126, 173, 36, 137, 166, 19,
      };
      CBORObject cbor1 = CBORObject.DecodeFromBytes(bytes1);
      CBORObject cbor2 = CBORObject.DecodeFromBytes(bytes2);
      CBORObject cbor3 = CBORObject.DecodeFromBytes(bytes3);
      CBORObject cbor4 = CBORObject.DecodeFromBytes(bytes4);
      TestCommon.CompareTestLess(cbor1, cbor2);
      TestCommon.CompareTestLess(cbor1, cbor4);
      TestCommon.CompareTestLess(cbor3, cbor2);
      TestCommon.CompareTestLess(cbor3, cbor4);
    }

    [Test]
    public void TestCBORMapAdd() {
      CBORObject cbor = CBORObject.NewMap();
      cbor.Add(1, 2);
      Assert.IsTrue(cbor.ContainsKey(
          ToObjectTest.TestToFromObjectRoundTrip(1)));
      {
        int varintTemp2 = cbor[
          ToObjectTest.TestToFromObjectRoundTrip(1)]
          .AsInt32();
        Assert.AreEqual(2, varintTemp2);
      }
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
      Assert.AreEqual((int)2, cbor["hello"].AsInt32Value());
      cbor.Set(1, 3);
      CBORObject cborone = ToObjectTest.TestToFromObjectRoundTrip(1);
      Assert.IsTrue(cbor.ContainsKey(cborone));
      Assert.AreEqual((int)3, cbor[cborone].AsInt32Value());
    }

    [Test]
    public void TestArray() {
      CBORObject cbor = CBORObject.FromJSONString("[]");
      cbor.Add(ToObjectTest.TestToFromObjectRoundTrip(3));
      cbor.Add(ToObjectTest.TestToFromObjectRoundTrip(4));
      byte[] bytes = CBORTestCommon.CheckEncodeToBytes(cbor);
      TestCommon.AssertByteArraysEqual (
        new byte[] { (byte)((byte)0x80 | 2), 3, 4 },
        bytes);
      cbor = CBORObject.FromObject(new[] {
        "a", "b", "c",
        "d", "e",
      });
      Assert.AreEqual("[\"a\",\"b\",\"c\",\"d\",\"e\"]", cbor.ToJSONString());
      string[] strArray = (string[])cbor.ToObject(typeof(string[]));
      cbor = CBORObject.FromObject(strArray);
      Assert.AreEqual("[\"a\",\"b\",\"c\",\"d\",\"e\"]", cbor.ToJSONString());
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0x9f, 0, 1, 2, 3,
        4, 5,
        6, 7, (byte)0xff,
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
        CBORTestCommon.AssertJSONSer (
          ToObjectTest.TestToFromObjectRoundTrip(bi),
          bi.ToString());

        Assert.IsTrue(
          ToObjectTest.TestToFromObjectRoundTrip(
          bi).AsNumber().IsInteger());

        CBORTestCommon.AssertRoundTrip (
          ToObjectTest.TestToFromObjectRoundTrip(bi));
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
        EInteger.FromString("18446744073709552127"),
      };
      for (var i = 0; i < ranges.Length; i += 2) {
        EInteger bigintTemp = ranges[i];
        while (true) {
          CBORTestCommon.AssertJSONSer (
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
      o = CBORTestCommon.FromBytesTestAB(new byte[] {
        (byte)0xc2, 0x41,
        (byte)0x88,
      });
      Assert.AreEqual(EInteger.FromRadixString("88", 16),
        o.ToObject(typeof(EInteger)));
      o = CBORTestCommon.FromBytesTestAB(new byte[] {
        (byte)0xc2, 0x42,
        (byte)0x88,
        0x77,
      });
      Assert.AreEqual(EInteger.FromRadixString("8877", 16),
        o.ToObject(typeof(EInteger)));
      o = CBORTestCommon.FromBytesTestAB(new byte[] {
        (byte)0xc2, 0x44,
        (byte)0x88, 0x77,
        0x66,
        0x55,
      });
      Assert.AreEqual (
        EInteger.FromRadixString("88776655", 16),
        o.ToObject(typeof(EInteger)));
      o = CBORTestCommon.FromBytesTestAB(new byte[] {
        (byte)0xc2, 0x47,
        (byte)0x88, 0x77,
        0x66,
        0x55, 0x44, 0x33, 0x22,
      });
      Assert.AreEqual (
        EInteger.FromRadixString("88776655443322", 16),
        o.ToObject(typeof(EInteger)));
    }

    [Test]
    public void TestByte() {
      for (var i = 0; i <= 255; ++i) {
        CBORTestCommon.AssertJSONSer (
          ToObjectTest.TestToFromObjectRoundTrip((byte)i),
          TestCommon.IntToString(i));
      }
    }

    [Test]
    public void TestByteArray() {
      CBORObject co = ToObjectTest.TestToFromObjectRoundTrip (
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
      CBORTestCommon.FromBytesTestAB (
        new byte[] { 0x5f, 0x41, 0x20, 0x41, 0x20, (byte)0xff });
    }

    [Test]
    public void TestEmptyIndefiniteLength() {
      CBORObject cbor;
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0x5f, (byte)0xff });
      Assert.AreEqual(0, cbor.GetByteString().Length);
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0x7f, (byte)0xff });
      string str = cbor.AsString();
      Assert.AreEqual(0, str.Length);
      cbor = CBORObject.DecodeFromBytes(new byte[] { (byte)0x9f, (byte)0xff });
      Assert.AreEqual(CBORType.Array, cbor.Type);
      Assert.AreEqual(0, cbor.Count);
      cbor = CBORObject.DecodeFromBytes(new byte[] { (byte)0xbf, (byte)0xff });
      Assert.AreEqual(CBORType.Map, cbor.Type);
      Assert.AreEqual(0, cbor.Count);
    }
    [Test]
    public void TestByteStringStreamNoIndefiniteWithinDefinite() {
      try {
        CBORTestCommon.FromBytesTestAB(new byte[] {
          0x5f, 0x41, 0x20, 0x5f,
          0x41, 0x20, (byte)0xff, (byte)0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {
          0x5f, 0x5f, 0x42, 0x20,
          0x20, (byte)0xff, (byte)0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {
          0x5f, 0x42, 0x20, 0x20,
          0x5f, 0x42, 0x20, 0x20, (byte)0xff, (byte)0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {
          0x5f, 0x7f, 0x62, 0x20,
          0x20, (byte)0xff, (byte)0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {
          0x5f, 0x5f, 0x41, 0x20,
          (byte)0xff, 0x41, 0x20, (byte)0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {
          0x7f, 0x7f, 0x62, 0x20,
          0x20, (byte)0xff, (byte)0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {
          0x7f, 0x62, 0x20, 0x20,
          0x7f, 0x62, 0x20, 0x20, (byte)0xff, (byte)0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {
          0x7f, 0x5f, 0x42, 0x20,
          0x20, (byte)0xff, (byte)0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {
          0x7f, 0x7f, 0x61, 0x20,
          (byte)0xff, 0x61, 0x20, (byte)0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0x5f, 0x00, (byte)0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0x7f, 0x00, (byte)0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0x5f, 0x20, (byte)0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0x7f, 0x20, (byte)0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xbf, 0x00, (byte)0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { (byte)0xbf, 0x20, (byte)0xff });
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
          0x5f, 0x41, 0x20,
          (byte)0xc2, 0x41, 0x20, (byte)0xff,
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
        if (!ed.AsNumber().IsInfinity() && !ed.AsNumber().IsNaN()) {
          ed2 = EDecimal.FromEInteger(AsED(ed).ToEInteger());
          if ((AsED(ed).CompareTo(ed2) == 0) != ed.AsNumber().IsInteger()) {
            Assert.Fail(ObjectMessage(ed));
          }
        }
        if (!ed.AsNumber().IsInfinity() && !ed.AsNumber().IsNaN()) {
          EInteger bi = AsEI(ed);
          if (ed.AsNumber().IsInteger()) {
            if ((bi.GetSignedBitLengthAsEInteger().ToInt32Checked() <= 31) !=
              ed.AsNumber().CanFitInInt32()) {
              Assert.Fail(ObjectMessage(ed));
            }
          }
          if ((bi.GetSignedBitLengthAsEInteger().ToInt32Checked() <= 31) !=
            ed.CanTruncatedIntFitInInt32()) {
            Assert.Fail(ObjectMessage(ed));
          }
          if (ed.AsNumber().IsInteger()) {
            if ((bi.GetSignedBitLengthAsEInteger().ToInt32Checked() <= 63) !=
              ed.AsNumber().CanFitInInt64()) {
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
      Assert.AreEqual (
        EInteger.FromString("2217361768"),
        cbor.ToObject(typeof(EInteger)));

      Assert.IsFalse (
        AsEI(cbor).GetSignedBitLengthAsEInteger().ToInt32Checked()
        <= 31);
      Assert.IsFalse(cbor.CanTruncatedIntFitInInt32());
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc5, (byte)0x82,
        0x18, 0x2f, 0x32,
      }); // -2674012278751232
      {
        int intTemp = AsEI(cbor)
        .GetSignedBitLengthAsEInteger().ToInt32Checked();
        Assert.AreEqual(52, intTemp);
      }
      Assert.IsTrue(cbor.AsNumber().CanFitInInt64());
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(2554895343L)
        .CanFitInSingle());
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc5, (byte)0x82,
        0x10, 0x38, 0x64,
      }); // -6619136
      Assert.AreEqual(EInteger.FromString("-6619136"),
        cbor.ToObject(typeof(EInteger)));
      Assert.AreEqual(-6619136, cbor.AsInt32());
      Assert.IsTrue(cbor.CanTruncatedIntFitInInt32());
    }

    [Test]
    public void TestCBOREInteger() {
      CBORObject o = CBORObject.DecodeFromBytes(new byte[] {
        0x3b, (byte)0xce,
        (byte)0xe2, 0x5a, 0x57, (byte)0xd8, 0x21, (byte)0xb9, (byte)0xa7,
      });
      Assert.AreEqual (
        EInteger.FromString("-14907577049884506536"),
        o.ToObject(typeof(EInteger)));
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
        CBORObject.NewArray().ToObject(typeof(EFloat));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().ToObject(typeof(EFloat));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.ToObject(typeof(EFloat));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.ToObject(typeof(EFloat));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.Undefined.ToObject(typeof(EFloat));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip (
          String.Empty).ToObject(typeof(EFloat));
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
    public void TestCBORInfinityRoundTrip() {
      CBORTestCommon.AssertRoundTrip (
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf));

      CBORTestCommon.AssertRoundTrip (
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.RatPosInf));

      bool bval = ToObjectTest.TestToFromObjectRoundTrip (
          CBORTestCommon.FloatNegInf).AsNumber().IsInfinity();
      Assert.IsTrue(bval);

      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(
          CBORTestCommon.RatPosInf).AsNumber().IsInfinity());

      Assert.IsTrue (
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf)
        .AsNumber().IsNegativeInfinity());

      Assert.IsTrue (
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.RatPosInf)
        .AsNumber().IsPositiveInfinity());
      Assert.IsTrue (
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.RatPosInf)
        .AsNumber().IsInfinity());

      Assert.IsTrue(
        CBORObject.PositiveInfinity.AsNumber().IsPositiveInfinity());

      Assert.IsTrue(
        CBORObject.NegativeInfinity.AsNumber().IsNegativeInfinity());
      Assert.IsTrue(CBORObject.NaN.AsNumber().IsNaN());

      CBORTestCommon.AssertRoundTrip (
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecNegInf));

      CBORTestCommon.AssertRoundTrip (
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf));

      CBORTestCommon.AssertRoundTrip (
        ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity));

      CBORTestCommon.AssertRoundTrip (
        ToObjectTest.TestToFromObjectRoundTrip(Single.NegativeInfinity));

      CBORTestCommon.AssertRoundTrip (
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecPosInf));

      CBORTestCommon.AssertRoundTrip (
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatPosInf));

      CBORTestCommon.AssertRoundTrip (
        ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity));

      CBORTestCommon.AssertRoundTrip (
        ToObjectTest.TestToFromObjectRoundTrip(Single.PositiveInfinity));
    }

    [Test]
    public void TestCompareB() {
      {
        string stringTemp = CBORObject.DecodeFromBytes(new byte[] {
          (byte)0xfa, 0x7f, (byte)0x80, 0x00, 0x00,
        }).ToObject(typeof(ERational)).ToString();
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

        CBORObject.FromObjectAndTag (
          ToObjectTest.TestToFromObjectRoundTrip(Double.NegativeInfinity),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor =

        CBORObject.FromObjectAndTag (
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor =

        CBORObject.FromObjectAndTag (
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecNegInf),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor =

        CBORObject.FromObjectAndTag (
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
    }

    [Test]
    public void TestDecFracCompareIntegerVsBigFraction() {
      CBORObject o1 = null;
      CBORObject o2 = null;
      o1 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xfb, (byte)0x8b,
        0x44,
        (byte)0xf2, (byte)0xa9, 0x0c, 0x27, 0x42, 0x28,
      });
      o2 = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc5, (byte)0x82,
        0x38,
        (byte)0xa4, (byte)0xc3, 0x50, 0x02, (byte)0x98, (byte)0xc5, (byte)0xa8,
        0x02, (byte)0xc1, (byte)0xf6, (byte)0xc0, 0x1a, (byte)0xbe, 0x08,
        0x04, (byte)0x86, (byte)0x99, 0x3e, (byte)0xf1,
      });
      AddSubCompare(o1, o2);
    }

    [Test]
    public void TestDecimalFrac() {
      CBORObject obj = CBORTestCommon.FromBytesTestAB (
          new byte[] { (byte)0xc4, (byte)0x82, 0x3, 0x1a, 1, 2, 3, 4 });
      try {
        Console.WriteLine(obj.ToObject(typeof(EDecimal)));
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestDecimalFracExactlyTwoElements() {
      CBORObject obj = CBORTestCommon.FromBytesTestAB(new byte[] {
        (byte)0xc4, (byte)0x81,
        (byte)0xc2, 0x41,
        1,
      });
      try {
        Console.WriteLine(obj.ToObject(typeof(EDecimal)));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestDecimalFracExponentMustNotBeBignum() {
      CBORObject obj = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc4,
        (byte)0x82,
        (byte)0xc2, 0x41, 1,
        0x1a,
        1, 2, 3, 4,
      });
      try {
        Console.WriteLine(obj.ToObject(typeof(EDecimal)));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestBigFloatExponentMustNotBeBignum() {
      CBORObject cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xc5,
        (byte)0x82,
        (byte)0xc2, 0x41, 1,
        0x1a,
        1, 2, 3, 4,
      });
      try {
        Console.WriteLine(cbor.ToObject(typeof(EFloat)));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestDecimalFracMantissaMayBeBignum() {
      CBORObject o = CBORTestCommon.FromBytesTestAB (
          new byte[] { (byte)0xc4, (byte)0x82, 0x3, (byte)0xc2, 0x41, 1 });
      Assert.AreEqual (
        EDecimal.FromString("1e3"),
        o.ToObject(typeof(EDecimal)));
    }

    [Test]
    public void TestBigFloatFracMantissaMayBeBignum() {
      CBORObject o = CBORTestCommon.FromBytesTestAB (
          new byte[] { (byte)0xc5, (byte)0x82, 0x3, (byte)0xc2, 0x41, 1 });
      {
        long numberTemp = EFloat.FromString("8").CompareTo(
  (EFloat)o.ToObject(typeof(EFloat)));

        Assert.AreEqual(0, numberTemp);
      }
    }

    [Test]
    public void TestDivide() {
      var r = new RandomGenerator();
      for (var i = 0; i < 3000; ++i) {
        CBORObject o1 =
          ToObjectTest.TestToFromObjectRoundTrip(
  RandomObjects.RandomEInteger(r));

        CBORObject o2 = ToObjectTest.TestToFromObjectRoundTrip(
  RandomObjects.RandomEInteger(r));

        if (o2.IsZero) {
          continue;
        }
        ERational er = ERational.Create(AsEI(o1), AsEI(o2));
        {
          ERational objectTemp = er;
          ERational objectTemp2;
          objectTemp2 = (ERational)CBORObject.Divide(
            o1,
            o2).ToObject(typeof(ERational));
          TestCommon.CompareTestEqual(objectTemp, objectTemp2);
        }
      }
    }

    [Test]
    public void TestCBORCompareTo() {
      int cmp = CBORObject.FromObject(0).CompareTo(null);
      if (cmp <= 0) {
        Assert.Fail();
      }
      cmp = CBORObject.FromObject(0).AsNumber().CompareTo(null);
      if (cmp <= 0) {
        Assert.Fail();
      }
    }

    [Test]
    public void TestDouble() {
      if (!ToObjectTest.TestToFromObjectRoundTrip (
          Double.PositiveInfinity).AsNumber().IsPositiveInfinity()) {
        Assert.Fail("Not positive infinity");
      }

      Assert.IsTrue (
        (
          (EDecimal)ToObjectTest.TestToFromObjectRoundTrip (
            Double.PositiveInfinity)
          .ToObject(typeof(EDecimal))).IsPositiveInfinity());

      Assert.IsTrue (
        (
          (EDecimal)ToObjectTest.TestToFromObjectRoundTrip (
            Double.NegativeInfinity)
          .ToObject(typeof(EDecimal))).IsNegativeInfinity());
      Assert.IsTrue (
  ((EDecimal)ToObjectTest.TestToFromObjectRoundTrip(Double.NaN)

          .ToObject(typeof(EDecimal))).IsNaN());
      for (int i = -65539; i <= 65539; ++i) {
        CBORObject o = ToObjectTest.TestToFromObjectRoundTrip((double)i);
        Assert.IsTrue(o.CanFitInDouble());
        Assert.IsTrue(o.AsNumber().CanFitInInt32());
        Assert.IsTrue(o.AsNumber().IsInteger());
        CBORTestCommon.AssertJSONSer (
          o,
          TestCommon.IntToString(i));
      }
    }
    [Test]
    public void TestDoubleCompare() {
      CBORObject oldobj = null;
      for (int i = -65539; i <= 65539; ++i) {
        CBORObject o = ToObjectTest.TestToFromObjectRoundTrip((double)i);
        if (oldobj != null) {
          TestCommon.CompareTestLess(oldobj.AsNumber(), o.AsNumber());
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
      CBORTestCommon.CheckEncodeToBytes(cbor);
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
        0x50, 0x64, (byte)0xa1, 0x1f, 0x10, (byte)0xc4, (byte)0xff,
        (byte)0xf2, (byte)0xc4, (byte)0xc9, 0x65, 0x12,
      });
      CBORTestCommon.AssertRoundTrip(obj);
    }

    [Test]
    [Timeout(5000)]
    public void TestExtendedExtremeExponentCompare() {
      CBORObject cbor1 = ToObjectTest.TestToFromObjectRoundTrip (
          EDecimal.FromString("333333e-2"));
      CBORObject cbor2 = ToObjectTest.TestToFromObjectRoundTrip (
          EFloat.Create (
            EInteger.FromString("5234222"),
            EInteger.FromString("-24936668661488")));
      TestCommon.CompareTestGreater(cbor1.AsNumber(), cbor2.AsNumber());
    }

    [Test]
    public void TestFloat() {
      Assert.IsTrue (
        (
          (EDecimal)ToObjectTest.TestToFromObjectRoundTrip (
            Single.PositiveInfinity)
          .ToObject(typeof(EDecimal))).IsPositiveInfinity());
      Assert.IsTrue (
        (
          (EDecimal)ToObjectTest.TestToFromObjectRoundTrip (
            Single.NegativeInfinity)
          .ToObject(typeof(EDecimal))).IsNegativeInfinity());
      Assert.IsTrue (
  ((EDecimal)ToObjectTest.TestToFromObjectRoundTrip(Single.NaN)

          .ToObject(typeof(EDecimal))).IsNaN());
      for (int i = -65539; i <= 65539; ++i) {
        CBORObject o = ToObjectTest.TestToFromObjectRoundTrip((float)i);
        // Console.Write("jsonser i=" + (// i) + " o=" + (o.ToString()) + " json=" +
        // (o.ToJSONString()) + " type=" + (o.Type));
        CBORTestCommon.AssertJSONSer (
          o,
          TestCommon.IntToString(i));
      }
    }
    [Test]
    public void TestHalfPrecision() {
      CBORObject o = CBORObject.DecodeFromBytes (
          new byte[] { (byte)0xf9, 0x7c, 0x00 });
      if (o.AsSingle() != Single.PositiveInfinity) {
        Assert.Fail();
      }
      o = CBORObject.DecodeFromBytes (
          new byte[] { (byte)0xf9, 0x00, 0x00 });
      if (o.AsSingle() != 0f) {
        Assert.Fail();
      }
      o = CBORObject.DecodeFromBytes (
          new byte[] { (byte)0xf9, (byte)0xfc, 0x00 });
      if (o.AsSingle() != Single.NegativeInfinity) {
        Assert.Fail();
      }
      o = CBORObject.DecodeFromBytes (
          new byte[] { (byte)0xf9, 0x7e, 0x00 });
      Assert.IsTrue(Single.IsNaN(o.AsSingle()));
    }

    [Test]
    public void TestTag268() {
      CBORObject cbor;
      CBORObject cbortag;
      for (var tag = 268; tag <= 269; ++tag) {
        cbor = CBORObject.NewArray().Add(-3).Add(99999).Add(0);
        cbortag = CBORObject.FromObjectAndTag(cbor, tag);
        Assert.IsFalse(cbortag.AsNumber().IsNegative());
        cbor = CBORObject.NewArray().Add(-3).Add(99999);
        cbortag = CBORObject.FromObjectAndTag(cbor, tag);
        try {
          Console.WriteLine(cbortag.ToObject(typeof(EDecimal)));
          Assert.Fail("Should have failed " + cbortag.ToString());
        } catch (InvalidOperationException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        cbor = CBORObject.NewArray().Add(-3).Add(99999).Add(1);
        cbortag = CBORObject.FromObjectAndTag(cbor, tag);
        Assert.IsTrue(cbortag.AsNumber().IsNegative());
        cbor = CBORObject.NewArray().Add(-3).Add(99999).Add(-1);
        cbortag = CBORObject.FromObjectAndTag(cbor, tag);
        try {
          Console.WriteLine(cbortag.ToObject(typeof(EDecimal)));
          Assert.Fail("Should have failed " + cbortag.ToString());
        } catch (InvalidOperationException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        cbor = CBORObject.NewArray().Add(-3).Add(99999).Add(2);
        cbortag = CBORObject.FromObjectAndTag(cbor, tag);
        try {
          Console.WriteLine(cbortag.ToObject(typeof(EDecimal)));
          Assert.Fail("Should have failed " + cbortag.ToString());
        } catch (InvalidOperationException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        cbor = CBORObject.NewArray().Add(0).Add(0).Add(2);
        cbortag = CBORObject.FromObjectAndTag(cbor, tag);
        Assert.IsFalse(cbortag.AsNumber().IsNegative());
        cbor = CBORObject.NewArray().Add(0).Add(0).Add(3);
        cbortag = CBORObject.FromObjectAndTag(cbor, tag);
        Assert.IsTrue(cbortag.AsNumber().IsNegative());
        cbor = CBORObject.NewArray().Add(-3).Add(99999).Add(8);
        cbortag = CBORObject.FromObjectAndTag(cbor, tag);
        try {
          Console.WriteLine(cbortag.ToObject(typeof(EDecimal)));
          Assert.Fail("Should have failed " + cbortag.ToString());
        } catch (InvalidOperationException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
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
      CBORObject o = CBORObject.FromJSONString (
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
        Int64.MaxValue - 1000,
        Int64.MaxValue,
        Int64.MinValue,
        Int64.MinValue + 1000,
      };
      for (var i = 0; i < ranges.Length; i += 2) {
        long j = ranges[i];
        while (true) {
          CBORNumber cn = ToObjectTest.TestToFromObjectRoundTrip(j).AsNumber();
          Assert.IsTrue(cn.IsInteger());
          Assert.IsTrue(cn.CanFitInInt64());
          Assert.IsTrue(cn.CanTruncatedIntFitInInt64());
          CBORTestCommon.AssertJSONSer (
            ToObjectTest.TestToFromObjectRoundTrip(j),
            TestCommon.LongToString(j));
          Assert.AreEqual (
            ToObjectTest.TestToFromObjectRoundTrip(j),
            ToObjectTest.TestToFromObjectRoundTrip(EInteger.FromInt64(j)));
          CBORObject obj = CBORObject.FromJSONString (
              "[" + TestCommon.LongToString(j) + "]");
          CBORTestCommon.AssertJSONSer (
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
      TestCommon.AssertEqualsHashCode (
        ToObjectTest.TestToFromObjectRoundTrip(2),
        cbor[ToObjectTest.TestToFromObjectRoundTrip("a")]);
      TestCommon.AssertEqualsHashCode (
        ToObjectTest.TestToFromObjectRoundTrip(4),
        cbor[ToObjectTest.TestToFromObjectRoundTrip("b")]);
      {
        long numberTemp = cbor[ToObjectTest.TestToFromObjectRoundTrip (
              "a")].AsInt32();
        Assert.AreEqual(2, numberTemp);
      }
      {
        long numberTemp = cbor[ToObjectTest.TestToFromObjectRoundTrip (
              "b")].AsInt32();
        Assert.AreEqual(4, numberTemp);
      }
      Assert.AreEqual(0, CBORObject.True.Count);
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xbf, 0x61, 0x61, 2,
        0x61, 0x62, 4, (byte)0xff,
      });
      Assert.AreEqual(2, cbor.Count);
      TestCommon.AssertEqualsHashCode (
        ToObjectTest.TestToFromObjectRoundTrip(2),
        cbor[ToObjectTest.TestToFromObjectRoundTrip("a")]);
      TestCommon.AssertEqualsHashCode (
        ToObjectTest.TestToFromObjectRoundTrip(4),
        cbor[ToObjectTest.TestToFromObjectRoundTrip("b")]);
      {
        long numberTemp = cbor[ToObjectTest.TestToFromObjectRoundTrip (
              "a")].AsInt32();
        Assert.AreEqual(2, numberTemp);
      }
      {
        long numberTemp = cbor[ToObjectTest.TestToFromObjectRoundTrip (
              "b")].AsInt32();
        Assert.AreEqual(4, numberTemp);
      }
    }

    [Test]
    public void TestMapInMap() {
      CBORObject oo;
      oo = CBORObject.NewArray().Add(CBORObject.NewMap()
          .Add (
            ERational.Create(EInteger.One, EInteger.FromString("2")),
            3).Add(4, false)).Add(true);
      CBORTestCommon.AssertRoundTrip(oo);
      oo = CBORObject.NewArray();
      oo.Add(ToObjectTest.TestToFromObjectRoundTrip(0));
      CBORObject oo2 = CBORObject.NewMap();
      oo2.Add (
        ToObjectTest.TestToFromObjectRoundTrip(1),
        ToObjectTest.TestToFromObjectRoundTrip(1368));
      CBORObject oo3 = CBORObject.NewMap();
      oo3.Add (
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
        CBORObject o = ToObjectTest.TestToFromObjectRoundTrip (
            EDecimal.FromString(r));
        CBORObject o2 = CBORDataUtilities.ParseJSONNumber(r);
        TestCommon.CompareTestEqual(o.AsNumber(), o2.AsNumber());
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
        } catch (CBORException) {
          jsonString = String.Empty;
        }
        if (jsonString.Length > 0) {
          CBORObject.FromJSONString(jsonString);
          TestWriteToJSON(obj);
        }
      }
    }
    public static CBORObject ReferenceTestObject() {
      return ReferenceTestObject(50);
    }
    public static CBORObject ReferenceTestObject(int nests) {
      CBORObject root = CBORObject.NewArray();
      CBORObject arr = CBORObject.NewArray().Add("xxx").Add("yyy");
      arr.Add("zzz")
      .Add("wwww").Add("iiiiiii").Add("aaa").Add("bbb").Add("ccc");
      arr = CBORObject.FromObjectAndTag(arr, 28);
      root.Add(arr);
      CBORObject refobj;
      for (var i = 0; i <= nests; ++i) {
        refobj = CBORObject.FromObjectAndTag(i, 29);
        arr = CBORObject.FromObject(new CBORObject[] {
          refobj, refobj, refobj, refobj, refobj, refobj, refobj, refobj,
          refobj,
        });
        arr = CBORObject.FromObjectAndTag(arr, 28);
        root.Add(arr);
      }
      return root;
    }

    [Test]
    [Timeout(5000)]
    public void TestCtap2CanonicalReferenceTest() {
       for (var i = 4; i <= 60; ++i) {
           // has high recursive reference depths, higher than
           // Ctap2Canonical supports, which is 4
           TestCtap2CanonicalReferenceTestOne(ReferenceTestObject(i));
       }
    }
    public static void TestCtap2CanonicalReferenceTestOne(CBORObject root) {
      if (root == null) {
        throw new ArgumentNullException(nameof(root));
      }
      byte[] bytes = root.EncodeToBytes();
      // NOTE: root has a nesting depth of more than four, so
      // encoding it should fail with Ctap2Canonical
      CBORObject origroot = root;
      var encodeOptions = new CBOREncodeOptions("resolvereferences=true");
      root = CBORObject.DecodeFromBytes(bytes, encodeOptions);
      encodeOptions = new CBOREncodeOptions("ctap2canonical=true");
      if (root == null) {
        Assert.Fail();
      }
      try {
        using (var lms = new MemoryStream()) {
          root.WriteTo(lms, encodeOptions);
          Assert.Fail("Should have failed");
        }
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    [Timeout(50000)]
    public void TestNoRecursiveExpansion() {
       for (var i = 5; i <= 60; ++i) {
           // has high recursive reference depths
// var sw = new System.Diagnostics.Stopwatch();sw.Start();
// Console.WriteLine("depth = "+i);
           TestNoRecursiveExpansionOne(ReferenceTestObject(i));
// Console.WriteLine("elapsed=" + sw.ElapsedMilliseconds + " ms");
       }
    }

    [Test]
    [Timeout(50000)]
    public void TestNoRecursiveExpansionJSON() {
       for (var i = 5; i <= 60; ++i) {
           // has high recursive reference depths
// var sw = new System.Diagnostics.Stopwatch();sw.Start();
// Console.WriteLine("depth = "+i);
           TestNoRecursiveExpansionJSONOne(ReferenceTestObject(i));
// Console.WriteLine("elapsed=" + sw.ElapsedMilliseconds + " ms");
       }
    }

    public static void TestNoRecursiveExpansionOne(CBORObject root) {
      if (root == null) {
        throw new ArgumentNullException(nameof(root));
      }
      CBORObject origroot = root;
      byte[] bytes = CBORTestCommon.CheckEncodeToBytes(root);
      var encodeOptions = new CBOREncodeOptions("resolvereferences=false");
      root = CBORObject.DecodeFromBytes(bytes, encodeOptions);
      encodeOptions = new CBOREncodeOptions("resolvereferences=true");
      root = CBORObject.DecodeFromBytes(bytes, encodeOptions);
      if (root == null) {
        Assert.Fail();
      }
      // Test a mitigation for wild recursive-reference expansions
      encodeOptions = new CBOREncodeOptions("resolvereferences=true");
      root = CBORObject.DecodeFromBytes(bytes, encodeOptions);
      if (root == null) {
        Assert.Fail();
      }
      try {
        using (var lms = new LimitedMemoryStream(100000)) {
          root.WriteTo(lms);
          Assert.Fail("Should have failed");
        }
      } catch (NotSupportedException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        using (var lms = new LimitedMemoryStream(100000)) {
          origroot.WriteTo(lms);
        }
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    public static void TestNoRecursiveExpansionJSONOne(CBORObject root) {
      if (root == null) {
        throw new ArgumentNullException(nameof(root));
      }
      CBORObject origroot = root;
      byte[] bytes = CBORTestCommon.CheckEncodeToBytes(root);
      var encodeOptions = new CBOREncodeOptions("resolvereferences=false");
      root = CBORObject.DecodeFromBytes(bytes, encodeOptions);
      encodeOptions = new CBOREncodeOptions("resolvereferences=true");
      root = CBORObject.DecodeFromBytes(bytes, encodeOptions);
      if (root == null) {
        Assert.Fail();
      }
      // Test a mitigation for wild recursive-reference expansions
      encodeOptions = new CBOREncodeOptions("resolvereferences=true");
      root = CBORObject.DecodeFromBytes(bytes, encodeOptions);
      if (root == null) {
        Assert.Fail();
      }
      try {
        using (var lms = new LimitedMemoryStream(100000)) {
          root.WriteJSONTo(lms);
          Assert.Fail("Should have failed");
        }
      } catch (NotSupportedException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        using (var lms = new LimitedMemoryStream(100000)) {
          origroot.WriteJSONTo(lms);
        }
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestSharedRefValidInteger() {
      byte[] bytes;
      var encodeOptions = new CBOREncodeOptions("resolvereferences=true");
      // Shared ref is integer
      bytes = new byte[] {
        (byte)0x82, (byte)0xd8, 0x1c, 0x00, (byte)0xd8,
        0x1d, 0x00,
      };
      try {
        CBORObject.DecodeFromBytes(bytes, encodeOptions);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Shared ref is negative
      bytes = new byte[] {
        (byte)0x82, (byte)0xd8, 0x1c, 0x00, (byte)0xd8,
        0x1d, 0x20,
      };
      try {
        CBORObject.DecodeFromBytes(bytes, encodeOptions);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Shared ref is non-integer
      bytes = new byte[] {
        (byte)0x82, (byte)0xd8, 0x1c, 0x00, (byte)0xd8,
        0x1d, (byte)0xc4, (byte)0x82,
        0x27, 0x19, (byte)0xff, (byte)0xff,
      };
      try {
        CBORObject.DecodeFromBytes(bytes, encodeOptions);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Shared ref is non-number
      bytes = new byte[] {
        (byte)0x82, (byte)0xd8, 0x1c, 0x00, (byte)0xd8,
        0x1d, 0x61, 0x41,
      };
      try {
        CBORObject.DecodeFromBytes(bytes, encodeOptions);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Shared ref is out of range
      bytes = new byte[] {
        (byte)0x82, (byte)0xd8, 0x1c, 0x00, (byte)0xd8,
        0x1d, 0x01,
      };
      try {
        CBORObject.DecodeFromBytes(bytes, encodeOptions);
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
        for (int j = 0; j < array.Length; ++j) {
          if (j + 3 <= array.Length) {
            int r = rand.UniformInt(0x1000000);
            array[j] = (byte)(r & (byte)0xff);
            array[j + 1] = (byte)((r >> 8) & (byte)0xff);
            array[j + 2] = (byte)((r >> 16) & (byte)0xff);
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
                try {
                  jsonString = o.ToJSONString();
                } catch (CBORException) {
                  jsonString = String.Empty;
                }
                if (jsonString.Length > 0) {
                  CBORObject.FromJSONString(jsonString);
                  TestWriteToJSON(o);
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
              // Expected exception
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

    private static void TestRandomSlightlyModifiedOne(byte[] array,
      RandomGenerator rand) {
      if (array.Length > 50000) {
        Console.WriteLine(String.Empty + array.Length);
      }
      int count2 = rand.UniformInt(10) + 1;
      for (int j = 0; j < count2; ++j) {
        int index = rand.UniformInt(array.Length);
        array[index] = unchecked((byte)rand.UniformInt(256));
      }
      using (var inputStream = new MemoryStream(array)) {
        while (inputStream.Position != inputStream.Length) {
          try {
            CBORObject o;
            o = CBORObject.Read(inputStream);
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
              if (o != null) {
                try {
                  jsonString = o.ToJSONString();
                } catch (CBORException ex) {
                  Console.WriteLine(ex.Message);
                  jsonString = String.Empty;
                }
                if (jsonString.Length > 0) {
                  CBORObject.FromJSONString(jsonString);
                  TestWriteToJSON(o);
                }
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
          } catch (InvalidOperationException ex) {
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

    [Test]
    public void TestRandomSlightlyModified() {
      var rand = new RandomGenerator();
      // Test slightly modified objects
      for (var i = 0; i < 2000; ++i) {
        CBORObject originalObject = CBORTestCommon.RandomCBORObject(rand);
        byte[] array = originalObject.EncodeToBytes();
        TestRandomSlightlyModifiedOne(array, rand);
      }
    }

    private static void TestReadWriteIntOne(int val) {
      try {
          {
            using (var ms = new MemoryStream()) {
              MiniCBOR.WriteInt32(val, ms);
              byte[] msarray = ms.ToArray();
              using (var ms2 = new MemoryStream(msarray)) {
                Assert.AreEqual(val, MiniCBOR.ReadInt32(ms2),
                   TestCommon.ToByteArrayString(msarray));
              }
            }
          }
          {
            using (var ms = new MemoryStream()) {
              CBORObject.Write(val, ms);
              byte[] msarray = ms.ToArray();
              using (var ms2 = new MemoryStream(msarray)) {
                Assert.AreEqual(val, MiniCBOR.ReadInt32(ms2),
                   TestCommon.ToByteArrayString(msarray));
              }
            }
          }
      } catch (IOException ioex) {
        Assert.Fail(ioex.Message);
      }
    }

    [Test]
    public void TestReadWriteInt() {
      var r = new RandomGenerator();
      for (var i = -70000; i < 70000; ++i) {
        TestReadWriteIntOne(i);
      }
      for (var i = 0; i < 100000; ++i) {
          int val = unchecked((int)RandomObjects.RandomInt64(r));
          TestReadWriteIntOne(val);
        }
    }

    [Test]
    public void TestShort() {
      for (int i = Int16.MinValue; i <= Int16.MaxValue; ++i) {
        CBORTestCommon.AssertJSONSer (
          ToObjectTest.TestToFromObjectRoundTrip((short)i),
          TestCommon.IntToString(i));
      }
    }

    [Test]
    public void TestSimpleValues() {
      CBORTestCommon.AssertJSONSer (
        ToObjectTest.TestToFromObjectRoundTrip(true),
        "true");
      CBORTestCommon.AssertJSONSer (
        ToObjectTest.TestToFromObjectRoundTrip(false),
        "false");
      CBORTestCommon.AssertJSONSer (
        ToObjectTest.TestToFromObjectRoundTrip((object)null),
        "null");
    }

    [Test]
    public void TestCtap2NestingLevel() {
      CBORObject o;
      var ctap = new CBOREncodeOptions("ctap2canonical=true");
      // 1 nesting level
      o = CBORObject.FromJSONString("[]");
      if (o.EncodeToBytes(ctap) == null) {
        Assert.Fail();
      }
      // 1 nesting level
      o = CBORObject.FromJSONString("[0]");
      if (o.EncodeToBytes(ctap) == null) {
        Assert.Fail();
      }
      // 3 nesting levels
      o = CBORObject.FromJSONString("[[[]]]");
      if (o.EncodeToBytes(ctap) == null) {
        Assert.Fail();
      }
      // 4 nesting levels
      o = CBORObject.FromJSONString("[[[[]]]]");
      if (o.EncodeToBytes(ctap) == null) {
        Assert.Fail();
      }
      // 5 nesting levels
      o = CBORObject.FromJSONString("[[[[[]]]]]");
      try {
        o.EncodeToBytes(ctap);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // 4 nesting levels
      o = CBORObject.FromJSONString("[[[[0]]]]");
      if (o.EncodeToBytes(ctap) == null) {
        Assert.Fail();
      }
      // 1 nesting level
      o = CBORObject.FromJSONString("[]");
      if (o.EncodeToBytes(ctap) == null) {
        Assert.Fail();
      }
      // 3 nesting levels
      o = CBORObject.FromJSONString("[[[]]]");
      if (o.EncodeToBytes(ctap) == null) {
        Assert.Fail();
      }
      // 4 nesting levels
      o = CBORObject.FromJSONString("[[{\"x\": []}]]");
      if (o.EncodeToBytes(ctap) == null) {
        Assert.Fail();
      }
      // 5 nesting levels
      o = CBORObject.FromJSONString("[[[{\"x\": []}]]]");
      try {
        o.EncodeToBytes(ctap);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // 4 nesting levels
      o = CBORObject.FromJSONString("[[[{\"x\": 0}]]]");
      if (o.EncodeToBytes(ctap) == null) {
        Assert.Fail();
      }
      // 5 nesting levels
      o = CBORObject.FromJSONString("[[[[{\"x\": 0}]]]]");
      try {
        o.EncodeToBytes(ctap);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
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
            cmpCobj.ToString() + ", cmpDecFrac=" + cmpDecFrac.ToString();
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
        EDecimal cmpCobj = o1.AsNumber().Add(o2.AsNumber()).AsEDecimal();
        if (cmpDecFrac.CompareTo(cmpCobj) != 0) {
          string msg = "o1=" + o1.ToString() + ", o2=" + o2.ToString() +
            ", " + AsED(o1) + ", " + AsED(o2) + ", cmpCobj=" +
            cmpCobj.ToString() + ", cmpDecFrac=" + cmpDecFrac.ToString();
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
        EDecimal cmpCobj = o1.AsNumber().Subtract(o2.AsNumber()).AsEDecimal();
        if (cmpDecFrac.CompareTo(cmpCobj) != 0) {
          string msg = "o1=" + o1.ToString() + ", o2=" + o2.ToString() +
            ", " + AsED(o1) + ", " + AsED(o2) + ", cmpCobj=" +
            cmpCobj.ToString() + ", cmpDecFrac=" + cmpDecFrac.ToString();
          TestCommon.CompareTestEqual(cmpDecFrac, cmpCobj, msg);
        }
        CBORTestCommon.AssertRoundTrip(o1);
        CBORTestCommon.AssertRoundTrip(o2);
      }
    }

    [Test]
    [Timeout(10000)]
    public void TestTaggedUntagged() {
      for (int i = 200; i < 1000; ++i) {
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

    private static void AssertEquals(int exp, int act) {
      // Much less overhead than Assert alone if the
      // two arguments are equal
      if (exp != act) {
        Assert.AreEqual(exp, act);
      }
    }
    private static void AssertEquals(object oexp, object oact) {
      // Much less overhead than Assert alone if the
      // two arguments are equal
      if (oexp == null ? oact != null : !oexp.Equals(oact)) {
        Assert.AreEqual(oexp, oact);
      }
    }
    private static void AssertEquals(object oexp, object oact, string str) {
      // Much less overhead than Assert alone if the
      // two arguments are equal
      if (oexp == null ? oact != null : !oexp.Equals(oact)) {
        Assert.AreEqual(oexp, oact, str);
      }
    }

    [Test]
    [Timeout(15000)]
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
      AssertEquals (
        EInteger.FromString("-1"),
        trueObj.MostInnerTag);
      EInteger[] tagstmp = CBORObject.True.GetAllTags();
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
            if (bc >= 264 && bc <= 270) {
              bigintTemp = bigintNext;
              continue;
            }
          }
          CBORObject obj = CBORObject.FromObjectAndTag(0, bigintTemp);
          if (!obj.IsTagged) {
            Assert.Fail("obj not tagged");
          }
          EInteger[] tags = obj.GetAllTags();
          AssertEquals(1, tags.Length);
          AssertEquals(bigintTemp, tags[0]);
          if (!obj.MostInnerTag.Equals(bigintTemp)) {
            string errmsg = "obj tag doesn't match: " + obj;
            AssertEquals(
              bigintTemp,
              obj.MostInnerTag,
              errmsg);
          }
          tags = obj.GetAllTags();
          AssertEquals(1, tags.Length);
          AssertEquals(bigintTemp, obj.MostOuterTag);
          AssertEquals(bigintTemp, obj.MostInnerTag);
          AssertEquals(0, obj.AsInt32Value());
          if (!bigintTemp.Equals(maxuint)) {
            EInteger bigintNew = bigintNext;
            // Test multiple tags
            CBORObject obj2 = CBORObject.FromObjectAndTag(obj, bigintNew);
            EInteger[] bi = obj2.GetAllTags();
            if (bi.Length != 2) {
              {
                string stringTemp = "Expected 2 tags: " + obj2;
                AssertEquals(
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
                AssertEquals(
                  bigintTemp,
                  obj2.MostInnerTag,
                  stringTemp);
              }
            }
            EInteger[] tags2 = obj2.GetAllTags();
            AssertEquals(2, tags2.Length);
            AssertEquals(bigintNext, obj2.MostOuterTag);
            AssertEquals(bigintTemp, obj2.MostInnerTag);
            AssertEquals(0, obj2.AsInt32Value());
          }
          if (bigintTemp.CompareTo(ranges[i + 1]) >= 0) {
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
    public void TestDecodeCtap2Canonical() {
      // Tests that the code rejects noncanonical data
      var options = new CBOREncodeOptions("ctap2canonical=1");
      Assert.IsTrue(options.Ctap2Canonical);
      byte[] bytes;
      for (var i = 0; i < 2; ++i) {
        int eb = i == 0 ? 0 : 0x20;
        bytes = new byte[] { (byte)eb };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x17 + eb) };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x18 + eb), 0 };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x19 + eb), 0, 0 };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x1a + eb), 0, 0, 0, 0 };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x1b + eb), 0, 0, 0, 0, 0, 0, 0, 0 };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x18 + eb), 0x17 };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x18 + eb), 0x18 };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x19 + eb), 0, (byte)0xff };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x19 + eb), 1, 0 };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x1a + eb), 0, 0, (byte)0xff, (byte)0xff };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x1a + eb), 0, 1, 0, 0 };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] {
          (byte)(0x1b + eb), 0, 0, 0, 0, (byte)0xff,
          (byte)0xff, (byte)0xff, (byte)0xff,
        };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x1b + eb), 0, 0, 0, 1, 0, 0, 0, 0 };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      for (var i = 2; i <= 5; ++i) {
        int eb = 0x20 * i;
        bytes = new byte[] { (byte)eb };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x18 + eb), 0 };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x19 + eb), 0, 0 };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x1a + eb), 0, 0, 0, 0 };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x1b + eb), 0, 0, 0, 0, 0, 0, 0, 0 };
        try {
          CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      bytes = new byte[] { (byte)0xc0, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { (byte)0xd7, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { (byte)0xd8, (byte)0xff, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { (byte)0xd9, (byte)0xff, (byte)0xff, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] {
        (byte)0xda, (byte)0xff, (byte)0xff, (byte)0xff,
        (byte)0xff, 0,
      };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] {
        (byte)0xdb, (byte)0xff, (byte)0xff, (byte)0xff,
        (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff,
        (byte)0xff, 0,
      };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Nesting depth
      bytes = new byte[] { (byte)0x81, (byte)0x81, (byte)0x81, (byte)0x80 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { (byte)0x81, (byte)0x81, (byte)0x81, (byte)0x81, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] {
        (byte)0x81, (byte)0x81, (byte)0x81, (byte)0xa1,
        0, 0,
      };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] {
        (byte)0x81, (byte)0x81, (byte)0x81, (byte)0x81,
        (byte)0x80,
      };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] {
        (byte)0x81, (byte)0x81, (byte)0x81, (byte)0xa1,
        0, 0,
      };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] {
        (byte)0x81, (byte)0x81, (byte)0x81, (byte)0x81,
        (byte)0xa0,
      };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Floating Point Numbers
      bytes = new byte[] { (byte)0xf9, 8, 8 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { (byte)0xfa, 8, 8, 8, 8 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { (byte)0xfb, 8, 8, 8, 8, 8, 8, 8, 8 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Map Key Ordering
      bytes = new byte[] { (byte)0xa2, 0, 0, 1, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { (byte)0xa2, 1, 0, 0, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { (byte)0xa2, 0, 0, 0x20, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { (byte)0xa2, 0x20, 0, 0, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { (byte)0xa2, 0, 0, 0x38, (byte)0xff, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { (byte)0xa2, 0x38, (byte)0xff, 0, 0, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { (byte)0xa2, 0x41, (byte)0xff, 0, 0x42, 0, 0, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { (byte)0xa2, 0x42, 0, 0, 0, 0x41, (byte)0xff, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { (byte)0xa2, 0x61, 0x7f, 0, 0x62, 0, 0, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { (byte)0xa2, 0x62, 0, 0, 0, 0x61, 0x7f, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestTags264And265() {
      CBORObject cbor;
      // Tag 264
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xd9, 0x01, 0x08,
        (byte)0x82,
        (byte)0xc2, 0x42, 2, 2, (byte)0xc2, 0x42, 2, 2,
      });
      CBORTestCommon.AssertRoundTrip(cbor);
      // Tag 265
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        (byte)0xd9, 0x01, 0x09,
        (byte)0x82,
        (byte)0xc2, 0x42, 2, 2, (byte)0xc2, 0x42, 2, 2,
      });
      CBORTestCommon.AssertRoundTrip(cbor);
    }
    [Test]
    public void TestTagThenBreak() {
      try {
        CBORTestCommon.FromBytesTestAB(new byte[] { (byte)0xd1, (byte)0xff });
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
      CBORObject cbor = CBORTestCommon.FromBytesTestAB (
          new byte[] { 0x7f, 0x61, 0x2e, 0x61, 0x2e, (byte)0xff });
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
          0x61, 0x20, (byte)0xff, (byte)0xff,
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
        string stringTemp = cbor[CBORObject.FromObject(
            (double)0.0)].AsString();
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
        string stringTemp = cbor[CBORObject.FromObject(
            (double)0.0)].AsString();
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
        string stringTemp = cbor[CBORObject.FromObject(
            (double)3.0)].AsString();
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
        string stringTemp = cbor[CBORObject.FromObject(
            (double)3.0)].AsString();
        Assert.AreEqual(
          "testpointzero",
          stringTemp);
      }
    }

    [Test]
    public void TestRoundTripESignalingNaN() {
      ToObjectTest.TestToFromObjectRoundTrip(EDecimal.SignalingNaN);
      ToObjectTest.TestToFromObjectRoundTrip(ERational.SignalingNaN);
      ToObjectTest.TestToFromObjectRoundTrip(EFloat.SignalingNaN);
    }

    [Test]
    public void TestBigNumberThresholds() {
      EInteger maxCborInteger = EInteger.FromString("18446744073709551615");
      EInteger maxInt64 = EInteger.FromString("9223372036854775807");
      EInteger minCborInteger = EInteger.FromString("-18446744073709551616");
      EInteger minInt64 = EInteger.FromString("-9223372036854775808");
      EInteger pastMaxCborInteger = EInteger.FromString(
        "18446744073709551616");
      EInteger pastMaxInt64 = EInteger.FromString("9223372036854775808");
      EInteger pastMinCborInteger =
        EInteger.FromString("-18446744073709551617");
      EInteger pastMinInt64 = EInteger.FromString("-9223372036854775809");
      var eints = new EInteger[] {
        maxCborInteger, maxInt64, minCborInteger,
        minInt64, pastMaxCborInteger, pastMaxInt64, pastMinCborInteger,
        pastMinInt64,
      };
      var isPastCbor = new bool[] {
        false, false, false, false, true, false, true,
        false,
      };
      var isPastInt64 = new bool[] {
        false, false, false, false, true, true, true,
        true,
      };
      for (var i = 0; i < eints.Length; ++i) {
        CBORObject cbor;
        bool isNegative = eints[i].Sign < 0;
        cbor = CBORObject.FromObject(eints[i]);
        Assert.IsTrue(cbor.IsNumber, cbor.ToString());
        if (isPastCbor[i]) {
          if (isNegative) {
            Assert.IsTrue(cbor.HasOneTag(3));
          } else {
            Assert.IsTrue(cbor.HasOneTag(2));
          }
        } else {
          Assert.AreEqual(CBORType.Integer, cbor.Type);
          Assert.AreEqual(0, cbor.TagCount);
        }
        EFloat ef = EFloat.Create(EInteger.One, eints[i]);
        cbor = CBORObject.FromObject(ef);
        Assert.IsTrue(cbor.IsNumber, cbor.ToString());
        if (isPastCbor[i]) {
          Assert.IsTrue(cbor.HasOneTag(265));
          if (isNegative) {
            Assert.IsTrue(cbor[0].HasOneTag(3));
          } else {
            Assert.IsTrue(cbor[0].HasOneTag(2));
          }
        } else {
          Assert.IsTrue(cbor.HasOneTag(5));
          Assert.AreEqual(CBORType.Integer, cbor[0].Type);
          Assert.AreEqual(0, cbor[0].TagCount);
        }
        try {
          using (var ms = new MemoryStream()) {
            CBORObject.Write(ef, ms);
            cbor = CBORObject.DecodeFromBytes(ms.ToArray());
            Assert.IsTrue(cbor.IsNumber, cbor.ToString());
            if (isPastCbor[i]) {
              Assert.IsTrue(cbor.HasOneTag(265));
              if (isNegative) {
                Assert.IsTrue(cbor[0].HasOneTag(3));
              } else {
                Assert.IsTrue(cbor[0].HasOneTag(2));
              }
            } else {
              Assert.IsTrue(cbor.HasOneTag(5));
              Assert.AreEqual(CBORType.Integer, cbor[0].Type);
              Assert.AreEqual(0, cbor[0].TagCount);
            }
          }
          EDecimal ed = EDecimal.Create(EInteger.One, eints[i]);
          cbor = CBORObject.FromObject(ed);
          Assert.IsTrue(cbor.IsNumber, cbor.ToString());
          if (isPastCbor[i]) {
            Assert.IsTrue(cbor.HasOneTag(264));
            if (isNegative) {
              Assert.IsTrue(cbor[0].HasOneTag(3));
            } else {
              Assert.IsTrue(cbor[0].HasOneTag(2));
            }
          } else {
            Assert.IsTrue(cbor.HasOneTag(4));
            Assert.AreEqual(CBORType.Integer, cbor[0].Type);
            Assert.AreEqual(0, cbor[0].TagCount);
          }
          using (var ms2 = new MemoryStream()) {
            CBORObject.Write(ed, ms2);
            cbor = CBORObject.DecodeFromBytes(ms2.ToArray());
            Assert.IsTrue(cbor.IsNumber, cbor.ToString());
            if (isPastCbor[i]) {
              Assert.IsTrue(cbor.HasOneTag(264));
              if (isNegative) {
                Assert.IsTrue(cbor[0].HasOneTag(3));
              } else {
                Assert.IsTrue(cbor[0].HasOneTag(2));
              }
            } else {
              Assert.IsTrue(cbor.HasOneTag(4));
              Assert.AreEqual(CBORType.Integer, cbor[0].Type);
              Assert.AreEqual(0, cbor[0].TagCount);
            }
          }
        } catch (IOException ioe) {
          throw new InvalidOperationException(ioe.Message, ioe);
        }
      }
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
    public void TestCtap2CanonicalDecodeEncodeSpecific1() {
      var bytes = new byte[] {
        (byte)0xa2, (byte)0x82, (byte)0xf6,
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
      };
      CBORObject cbor = CBORObject.DecodeFromBytes(bytes);
      var options = new CBOREncodeOptions("ctap2canonical=true");
      Console.WriteLine(cbor);
      try {
        cbor.EncodeToBytes(options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(CBORTestCommon.CheckEncodeToBytes(cbor),
  options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestCtap2CanonicalDecodeEncodeSpecific2() {
      var bytes = new byte[] {
        (byte)0x82,
        (byte)0x82,
        (byte)0xf5,
        (byte)0x82,
        (byte)0x81,
        (byte)0xd8, 0x1e, (byte)0x82, 0x29, 0x01, (byte)0x80, 0x43, 0x01, 0x01,
        0x00,
      };
      CBORObject cbor = CBORObject.DecodeFromBytes(bytes);
      var options = new CBOREncodeOptions("ctap2canonical=true");
      Console.WriteLine(cbor);
      try {
        cbor.EncodeToBytes(options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(CBORTestCommon.CheckEncodeToBytes(cbor),
  options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestCtap2CanonicalDecodeEncodeSpecific3() {
      var bytes = new byte[] {
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
      };
      CBORObject cbor = CBORObject.DecodeFromBytes(bytes);
      var options = new CBOREncodeOptions("ctap2canonical=true");
      Console.WriteLine(cbor);
      try {
        cbor.EncodeToBytes(options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(CBORTestCommon.CheckEncodeToBytes(cbor),
  options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestCtap2CanonicalDecodeEncodeSpecific4() {
      var bytes = new byte[] {
        (byte)0x81,
        (byte)0x82,
        (byte)0xda, 0x00, 0x0d, 0x77, 0x09,
        (byte)0xf4, (byte)0x82, (byte)0x82, (byte)0xf4, (byte)0xa0,
        (byte)0xf6,
      };
      CBORObject cbor = CBORObject.DecodeFromBytes(bytes);
      var options = new CBOREncodeOptions("ctap2canonical=true");
      Console.WriteLine(cbor);
      try {
        cbor.EncodeToBytes(options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(CBORTestCommon.CheckEncodeToBytes(cbor),
  options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestCtap2CanonicalDecodeEncodeSpecific5() {
      var bytes = new byte[] {
        (byte)0xa2,
        (byte)0xda, 0x00, 0x03, 0x69,
        (byte)0x95, (byte)0xf6, (byte)0xf7, (byte)0xf6, (byte)0xf4,
      };
      CBORObject cbor = CBORObject.DecodeFromBytes(bytes);
      var options = new CBOREncodeOptions("ctap2canonical=true");
      Console.WriteLine(cbor);
      try {
        cbor.EncodeToBytes(options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(CBORTestCommon.CheckEncodeToBytes(cbor),
  options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestCtap2CanonicalDecodeEncode() {
      var r = new RandomGenerator();

      var options = new CBOREncodeOptions("ctap2canonical=true");
      for (var i = 0; i < 3000; ++i) {
        CBORObject cbor = CBORTestCommon.RandomCBORObject(r);
        byte[] e2bytes = CBORTestCommon.CheckEncodeToBytes(cbor);
        byte[] bytes = e2bytes;
        cbor = CBORObject.DecodeFromBytes(bytes);
        CBORObject cbor2 = null;
        try {
          bytes = cbor.EncodeToBytes(options);
          try {
            cbor2 = CBORObject.DecodeFromBytes(bytes, options);
          } catch (Exception ex2) {
            Assert.Fail(ex2.ToString());
            throw new InvalidOperationException(String.Empty, ex2);
          }
          byte[] bytes2 = cbor2.EncodeToBytes(options);
          TestCommon.AssertByteArraysEqual(bytes, bytes2);
        } catch (CBORException ex4) {
          // Canonical encoding failed, so DecodeFromBytes must fail
          bytes = CBORTestCommon.CheckEncodeToBytes(cbor);
          try {
            CBORObject.DecodeFromBytes(bytes, options);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex3) {
            Assert.Fail(ex3.ToString() + "\n" + ex4.ToString());
            throw new InvalidOperationException(String.Empty, ex3);
          }
        }
      }
    }

    [Test]
    public void TestTextStringStreamNoTagsBeforeDefinite() {
      try {
        CBORTestCommon.FromBytesTestAB(new byte[] {
          0x7f, 0x61, 0x20,
          (byte)0xc0, 0x61, 0x20, (byte)0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    private static EInteger AsEI(CBORObject obj) {
      object o = obj.ToObject(typeof(EInteger));
      return (EInteger)o;
    }

    private static EDecimal AsED(CBORObject obj) {
      return EDecimal.FromString (
          obj.ToObject(typeof(EDecimal)).ToString());
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

    private static void TestTextStringStreamOne(string longString) {
      CBORObject cbor, cbor2;
      cbor = ToObjectTest.TestToFromObjectRoundTrip(longString);
      cbor2 =
CBORTestCommon.FromBytesTestAB(CBORTestCommon.CheckEncodeToBytes(cbor));
      {
        object objectTemp = longString;
        object objectTemp2 =
CBORObject.DecodeFromBytes(
  CBORTestCommon.CheckEncodeToBytes(cbor)).AsString();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
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
          jsonString = DataUtilities.GetUtf8String (
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
