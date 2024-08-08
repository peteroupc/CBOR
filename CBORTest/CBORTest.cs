/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;
using PeterO.Numbers;

namespace Test {
  [TestFixture]
  public class CBORTest {
    public static int ByteArrayCompareLengthFirst(byte[] a, byte[] b) {
      if (a == null) {
        return (b == null) ? 0 : -1;
      }
      if (b == null) {
        return 1;
      }
      if (a.Length != b.Length) {
        return a.Length < b.Length ? -1 : 1;
      }
      for (int i = 0; i < a.Length; ++i) {
        if (a[i] != b[i]) {
          return (a[i] < b[i]) ? -1 : 1;
        }
      }
      return 0;
    }

    [Test]
    public void TestCorrectUtf8Specific() {
      TestJsonUtf8One(new byte[] {
        0xe8,
        0xad,
        0xbd,
        0xf1,
        0x81,
        0x95,
        0xb9,
        0xc3, 0x84, 0xcc, 0xb6, 0xcd,
        0xa3,
      });
      TestJsonUtf8One(new byte[] {
        0xe8,
        0x89,
        0xa0,
        0xf2,
        0x97,
        0x84, 0xbb, 0x3f, 0xd1, 0x83, 0xd1,
        0xb9,
      });
      TestJsonUtf8One(new byte[] {
        0xf3,
        0xbb,
        0x98,
        0x8a,
        0xc3,
        0x9f,
        0xe7,
        0xa5,
        0x96, 0xd9, 0x92, 0xe1, 0xa3,
        0xad,
      });
      TestJsonUtf8One(new byte[] {
        0xf0,
        0xa7,
        0xbf,
        0x84, 0x70, 0x55,
        0xd0, 0x91, 0xe8, 0xbe, 0x9f,
      });
      TestJsonUtf8One(new byte[] {
        0xd9,
        0xae,
        0xe4,
        0xa1,
        0xa0,
        0xf3,
        0x90,
        0x94,
        0x99,
        0xf3,
        0xab,
        0x8a, 0xad, 0xf4, 0x88, 0x9a,
        0x9a,
      });
      TestJsonUtf8One(new byte[] {
        0x3d,
        0xf2,
        0x83,
        0xa9,
        0xbe,
        0xea,
        0xb9,
        0xbd, 0xd7, 0x8b, 0xe7, 0xbc,
        0x83,
      });
      TestJsonUtf8One(new byte[] {
        0xc4,
        0xab, 0xf4, 0x8e, 0x8a, 0x91, 0x61, 0x4d,
        0x3b,
      });
      TestJsonUtf8One(new byte[] {
        0xf1,
        0xae,
        0x86, 0xad, 0x5f, 0xd0, 0xb7, 0x6e, 0xda,
        0x85,
      });
    }

    public static void TestJsonUtf8One(byte[] bytes) {
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      string str = DataUtilities.GetUtf8String(bytes, false);
      var bytes2 = new byte[bytes.Length + 2];
      bytes2[0] = 0x22;
      Array.Copy(bytes, 0, bytes2, 1, bytes.Length);
      bytes2[^1] = 0x22;
      string str2 = CBORObject.FromJSONBytes(bytes2)
        .AsString();
      if (!str.Equals(str2, StringComparison.Ordinal)) {
        Assert.AreEqual(
          str,
          str2,
          TestCommon.ToByteArrayString(bytes));
      }
    }

    [Test]
    public void TestCorrectUtf8() {
      var rg = new RandomGenerator();
      for (int i = 0; i < 500; ++i) {
        TestJsonUtf8One(RandomObjects.RandomUtf8Bytes(rg, true));
      }
    }

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
      var cbor1 = CBORObject.DecodeFromBytes(bytes1);
      var cbor2 = CBORObject.DecodeFromBytes(bytes2);
      var cbor3 = CBORObject.DecodeFromBytes(bytes3);
      var cbor4 = CBORObject.DecodeFromBytes(bytes4);
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
      var cbor1 = CBORObject.DecodeFromBytes(bytes1);
      var cbor2 = CBORObject.DecodeFromBytes(bytes2);
      var cbor3 = CBORObject.DecodeFromBytes(bytes3);
      var cbor4 = CBORObject.DecodeFromBytes(bytes4);
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
      var cbor1 = CBORObject.DecodeFromBytes(bytes1);
      var cbor2 = CBORObject.DecodeFromBytes(bytes2);
      var cbor3 = CBORObject.DecodeFromBytes(bytes3);
      var cbor4 = CBORObject.DecodeFromBytes(bytes4);
      TestCommon.CompareTestLess(cbor1, cbor2);
      TestCommon.CompareTestLess(cbor1, cbor4);
      TestCommon.CompareTestLess(cbor3, cbor2);
      TestCommon.CompareTestLess(cbor3, cbor4);
    }

    [Test]
    public void TestCBORMapAdd() {
      var cbor = CBORObject.NewMap();
      _ = cbor.Add(1, 2);
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
      _ = cbor.Add("hello", 2);
      Assert.IsTrue(cbor.ContainsKey("hello"));

      Assert.IsTrue(cbor.ContainsKey(ToObjectTest.TestToFromObjectRoundTrip(
            "hello")));
      Assert.AreEqual(2, cbor["hello"].AsInt32Value());
      _ = cbor.Set(1, CBORObject.FromInt32(3));
      CBORObject cborone = ToObjectTest.TestToFromObjectRoundTrip(1);
      Assert.IsTrue(cbor.ContainsKey(cborone));
      Assert.AreEqual(3, cbor[cborone].AsInt32Value());
    }

    [Test]
    public void TestArray() {
      var cbor = CBORObject.FromJSONString("[]");
      _ = cbor.Add(ToObjectTest.TestToFromObjectRoundTrip(3));
      _ = cbor.Add(ToObjectTest.TestToFromObjectRoundTrip(4));
      byte[] bytes = CBORTestCommon.CheckEncodeToBytes(cbor);
      TestCommon.AssertByteArraysEqual(
        new byte[] { 0x80 | 2, 3, 4 },
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
        0x9f, 0, 1, 2, 3,
        4, 5,
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
      for (int i = 0; i < 500; ++i) {
        EInteger bi = RandomObjects.RandomEInteger(r);
        CBORTestCommon.AssertJSONSer(
          ToObjectTest.TestToFromObjectRoundTrip(bi),
          bi.ToString());

        Assert.IsTrue(
          ToObjectTest.TestToFromObjectRoundTrip(
            bi).AsNumber().IsInteger());

        CBORTestCommon.AssertRoundTrip(
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
      for (int i = 0; i < ranges.Length; i += 2) {
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
      CBORObject o = CBORTestCommon.FromBytesTestAB(new byte[] {
        0xc2, 0x41,
        0x88,
      });
      Assert.AreEqual(
        EInteger.FromRadixString("88", 16),
        o.ToObject(typeof(EInteger)));
      o = CBORTestCommon.FromBytesTestAB(new byte[] {
        0xc2, 0x42,
        0x88,
        0x77,
      });
      Assert.AreEqual(
        EInteger.FromRadixString("8877", 16),
        o.ToObject(typeof(EInteger)));
      o = CBORTestCommon.FromBytesTestAB(new byte[] {
        0xc2, 0x44,
        0x88, 0x77,
        0x66,
        0x55,
      });
      Assert.AreEqual(
        EInteger.FromRadixString("88776655", 16),
        o.ToObject(typeof(EInteger)));
      o = CBORTestCommon.FromBytesTestAB(new byte[] {
        0xc2, 0x47,
        0x88, 0x77,
        0x66,
        0x55, 0x44, 0x33, 0x22,
      });
      Assert.AreEqual(
        EInteger.FromRadixString("88776655443322", 16),
        o.ToObject(typeof(EInteger)));
    }

    [Test]
    public void TestByte() {
      for (int i = 0; i <= 255; ++i) {
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
      _ = CBORTestCommon.FromBytesTestAB(
        new byte[] { 0x5f, 0x41, 0x20, 0x41, 0x20, 0xff });
    }

    [Test]
    public void TestWriteToJSONSpecific() {
      var bytes = new byte[] {
        0x6a, 0x25, 0x7f, 0x41, 0x58, 0x11, 0x54,
        0xc3, 0x94, 0x19, 0x49,
      };
      TestWriteToJSON(CBORObject.DecodeFromBytes(bytes));
      bytes = new byte[] {
        0xfb, 0x61, 0x90, 0x00, 0x00, 0x7c,
        0x01, 0x5a, 0x0a,
      };
      TestWriteToJSON(CBORObject.DecodeFromBytes(bytes));
      bytes = new byte[] {
        0xfb, 0x36, 0x90, 0x01, 0x00, 0x3f,
        0xd9, 0x2b, 0xdb,
      };
      TestWriteToJSON(CBORObject.DecodeFromBytes(bytes));
    }

    [Test]
    public void TestEmptyIndefiniteLength() {
      CBORObject cbor;
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0x5f, 0xff });
      Assert.AreEqual(0, cbor.GetByteString().Length);
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0x7f, 0xff });
      string str = cbor.AsString();
      Assert.AreEqual(0, str.Length);
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0x9f, 0xff });
      Assert.AreEqual(CBORType.Array, cbor.Type);
      Assert.AreEqual(0, cbor.Count);
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0xbf, 0xff });
      Assert.AreEqual(CBORType.Map, cbor.Type);
      Assert.AreEqual(0, cbor.Count);
    }

    [Test]
    public void TestByteStringStreamNoIndefiniteWithinDefinite() {
      try {
        _ = CBORTestCommon.FromBytesTestAB(new byte[] {
          0x5f, 0x41, 0x20,
          0x5f, 0x41, 0x20, 0xff, 0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(new byte[] {
          0x5f, 0x5f, 0x42, 0x20,
          0x20, 0xff, 0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(new byte[] {
          0x5f, 0x42, 0x20, 0x20,
          0x5f, 0x42, 0x20, 0x20, 0xff, 0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(new byte[] {
          0x5f, 0x7f, 0x62, 0x20,
          0x20, 0xff, 0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(new byte[] {
          0x5f, 0x5f, 0x41, 0x20,
          0xff, 0x41, 0x20, 0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(new byte[] {
          0x7f, 0x7f, 0x62, 0x20,
          0x20, 0xff, 0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(new byte[] {
          0x7f, 0x62, 0x20, 0x20,
          0x7f, 0x62, 0x20, 0x20, 0xff, 0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(new byte[] {
          0x7f, 0x5f, 0x42, 0x20,
          0x20, 0xff, 0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(new byte[] {
          0x7f, 0x7f, 0x61, 0x20,
          0xff, 0x61, 0x20, 0xff,
        });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(new byte[] { 0x5f, 0x00, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(new byte[] { 0x7f, 0x00, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(new byte[] { 0x5f, 0x20, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(new byte[] { 0x7f, 0x20, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(new byte[] { 0xbf, 0x00, 0xff, });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(new byte[] { 0xbf, 0x20, 0xff, });
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
        _ = CBORTestCommon.FromBytesTestAB(new byte[] {
          0x5f, 0x41, 0x20,
          0xc2, 0x41, 0x20, 0xff,
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
      return obj == null ? throw new ArgumentNullException(nameof(obj)) :
        new System.Text.StringBuilder().Append("CBORObject.DecodeFromBytes(")
        .Append(TestCommon.ToByteArrayString(obj.EncodeToBytes()))
        .Append("); /").Append("/ ").Append(obj.ToJSONString()).ToString();
    }

    public static void TestCanFitInOne(CBORObject ed) {
      EDecimal ed2;
      if (ed == null) {
        throw new ArgumentNullException(nameof(ed));
      }
      CBORNumber edNumber = ed.AsNumber();
      EDecimal edNumberED = AsED(ed);
      ed2 = EDecimal.FromDouble(edNumberED.ToDouble());
      if (edNumberED.CompareTo(ed2) == 0 != edNumber.CanFitInDouble()) {
        Assert.Fail(ObjectMessage(ed) + "\n// CanFitInDouble");
      }
      ed2 = EDecimal.FromSingle(AsED(ed).ToSingle());
      if (edNumberED.CompareTo(ed2) == 0 != edNumber.CanFitInSingle()) {
        Assert.Fail(ObjectMessage(ed) + "\n// CanFitInSingle");
      }
      if (!edNumber.IsInfinity() && !edNumber.IsNaN()) {
        if (edNumberED.IsInteger() != edNumber.IsInteger()) {
          Assert.Fail(ObjectMessage(ed) + "\n// IsInteger");
        }
      }
      if (!edNumber.IsInfinity() && !edNumber.IsNaN()) {
        EDecimal edec = edNumberED;
        EInteger bi;
        try {
          bi = edec.ToSizedEInteger(128);
        } catch (OverflowException) {
          bi = null;
        }
        if (edNumber.IsInteger()) {
          if ((bi != null && bi.GetSignedBitLengthAsInt64() <= 31) !=
            edNumber.CanFitInInt32()) {
            Assert.Fail(ObjectMessage(ed) + "\n// Int32");
          }
        }
        if ((bi != null && bi.GetSignedBitLengthAsInt64() <= 31) !=
          edNumber.CanTruncatedIntFitInInt32()) {
          Assert.Fail(ObjectMessage(ed) + "\n// TruncInt32");
        }
        if (edNumber.IsInteger()) {
          if ((bi != null && bi.GetSignedBitLengthAsInt64() <= 63) !=
            edNumber.CanFitInInt64()) {
            Assert.Fail(ObjectMessage(ed) + "\n// Int64");
          }
        }
        if ((bi != null && bi.GetSignedBitLengthAsInt64() <= 63) !=
          edNumber.CanTruncatedIntFitInInt64()) {
          Assert.Fail(ObjectMessage(ed) + "\n// TruncInt64");
        }
      }
    }

    [Test]
    [Timeout(10000)]
    public void TestCanFitIn() {
      var r = new RandomGenerator();
      for (int i = 0; i < 5000; ++i) {
        CBORObject ed = CBORTestCommon.RandomNumber(r);
        TestCanFitInOne(ed);
      }
      var cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xfb,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
      });
      TestCanFitInOne(cbor);
    }

    [Test]
    public void TestCanFitInSpecificCases() {
      var cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xfb,
        0x41, 0xe0, 0x85, 0x48, 0x2d, 0x14, 0x47, 0x7a,
      }); // 2217361768.63373
      Assert.AreEqual(
        EInteger.FromString("2217361768"),
        cbor.ToObject(typeof(EInteger)));

      Assert.IsFalse(
        AsEI(cbor).GetSignedBitLengthAsInt64()
        <= 31);
      Assert.IsFalse(cbor.AsNumber().CanTruncatedIntFitInInt32());
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xc5, 0x82,
        0x18, 0x2f, 0x32,
      }); // -2674012278751232
      Assert.AreEqual(
        52L,
        AsEI(cbor).GetSignedBitLengthAsInt64());
      Assert.IsTrue(cbor.AsNumber().CanFitInInt64());
      Assert.IsFalse(ToObjectTest.TestToFromObjectRoundTrip(2554895343L)
        .AsNumber().CanFitInSingle());
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xc5, 0x82,
        0x10, 0x38, 0x64,
      }); // -6619136
      Assert.AreEqual(EInteger.FromString("-6619136"),
        cbor.ToObject(typeof(EInteger)));
      Assert.AreEqual(-6619136, cbor.AsInt32());
      Assert.IsTrue(cbor.AsNumber().CanTruncatedIntFitInInt32());
    }

    [Test]
    public void TestCBOREInteger() {
      var o = CBORObject.DecodeFromBytes(new byte[] {
        0x3b, 0xce,
        0xe2, 0x5a, 0x57, 0xd8, 0x21, 0xb9, 0xa7,
      });
      Assert.AreEqual(
        EInteger.FromString("-14907577049884506536"),
        o.ToObject(typeof(EInteger)));
    }

    [Test]
    public void TestCBORExceptions() {
      try {
        _ = CBORObject.NewArray().Remove(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.NewMap().Remove(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.NewArray().Add(CBORObject.Null);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.NewMap().Add(CBORObject.True);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.True.Remove(CBORObject.True);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(0).Remove(CBORObject.True);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(String.Empty)
        .Remove(CBORObject.True);
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.NewArray().ToObject(typeof(EFloat));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.NewMap().ToObject(typeof(EFloat));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.True.ToObject(typeof(EFloat));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.False.ToObject(typeof(EFloat));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.Undefined.ToObject(typeof(EFloat));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(
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
      var o = CBORObject.FromObject(new[] { 1, 2, 3 });
      Assert.AreEqual(3, o.Count);
      Assert.AreEqual(1, o[0].AsInt32());
      Assert.AreEqual(2, o[1].AsInt32());
      Assert.AreEqual(3, o[2].AsInt32());
      CBORTestCommon.AssertRoundTrip(o);
    }

    [Test]
    public void TestCBORInfinityRoundTrip() {
      CBORTestCommon.AssertRoundTrip(
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf));

      CBORTestCommon.AssertRoundTrip(
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.RatPosInf));

      bool bval = ToObjectTest.TestToFromObjectRoundTrip(
          CBORTestCommon.FloatNegInf).AsNumber().IsInfinity();
      Assert.IsTrue(bval);

      Assert.IsTrue(ToObjectTest.TestToFromObjectRoundTrip(
          CBORTestCommon.RatPosInf).AsNumber().IsInfinity());

      Assert.IsTrue(
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf)
        .AsNumber().IsNegativeInfinity());

      Assert.IsTrue(
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.RatPosInf)
        .AsNumber().IsPositiveInfinity());
      Assert.IsTrue(
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.RatPosInf)
        .AsNumber().IsInfinity());

      Assert.IsTrue(
        CBORObject.PositiveInfinity.AsNumber().IsPositiveInfinity());

      Assert.IsTrue(
        CBORObject.NegativeInfinity.AsNumber().IsNegativeInfinity());
      Assert.IsTrue(CBORObject.NaN.AsNumber().IsNaN());

      CBORTestCommon.AssertRoundTrip(
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecNegInf));

      CBORTestCommon.AssertRoundTrip(
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf));

      CBORTestCommon.AssertRoundTrip(
        ToObjectTest.TestToFromObjectRoundTrip(double.NegativeInfinity));

      CBORTestCommon.AssertRoundTrip(
        ToObjectTest.TestToFromObjectRoundTrip(float.NegativeInfinity));

      CBORTestCommon.AssertRoundTrip(
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecPosInf));

      CBORTestCommon.AssertRoundTrip(
        ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatPosInf));

      CBORTestCommon.AssertRoundTrip(
        ToObjectTest.TestToFromObjectRoundTrip(double.PositiveInfinity));

      CBORTestCommon.AssertRoundTrip(
        ToObjectTest.TestToFromObjectRoundTrip(float.PositiveInfinity));
    }

    [Test]
    public void TestEquivJSONSpecificA() {
      _ = TestEquivJSONOne(new byte[] {
        0x2d, 0x37, 0x30, 0x31, 0x39, 0x34,
        0x38, 0x33, 0x35, 0x39, 0x31, 0x33, 0x37, 0x34, 0x38, 0x45, 0x30,
      });
    }

    public static bool TestEquivJSONOne(byte[] bytes) {
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      if (!(bytes.Length > 0)) {
        return false;
      }
      var cbo = CBORObject.FromJSONBytes(bytes);
      Assert.IsTrue(cbo != null);
      var cbo2 = CBORObject.FromJSONString(cbo.ToJSONString());
      Assert.IsTrue(cbo2 != null);
      if (!cbo.Equals(cbo2)) {
        Console.Write("jsonstring");
        Console.Write(TestCommon.ToByteArrayString(bytes));
        Console.Write(DataUtilities.GetUtf8String(bytes, true));
        Console.Write("old " + TestCommon.ToByteArrayString(cbo.ToJSONBytes()));
        Console.Write(cbo.ToJSONString());
        Console.Write("new " +
          TestCommon.ToByteArrayString(cbo2.ToJSONBytes()));
        Console.Write(cbo2.ToJSONString());
        Assert.AreEqual(cbo, cbo2);
      }
      cbo2 = CBORObject.FromJSONBytes(cbo.ToJSONBytes());
      Assert.IsTrue(cbo2 != null);
      if (!cbo.Equals(cbo2)) {
        Console.Write("jsonbytes");
        Console.Write(TestCommon.ToByteArrayString(bytes));
        Console.Write(DataUtilities.GetUtf8String(bytes, true));
        Console.Write("old " + TestCommon.ToByteArrayString(cbo.ToJSONBytes()));
        Console.Write(cbo.ToJSONString());
        Console.Write("new " +
          TestCommon.ToByteArrayString(cbo2.ToJSONBytes()));
        Console.Write(cbo2.ToJSONString());
        Assert.AreEqual(cbo, cbo2);
      }
      return true;
    }

    public static bool TestEquivJSONNumberOne(byte[] bytes) {
      // Assume the JSON begins and ends with a digit
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      if (!(bytes.Length > 0)) {
        return false;
      }
      if (bytes[0] is not ((>= 0x30 and <= 0x39) or (byte)'-')) {
        return false;
      }
      if (bytes[^1] is not (>= 0x30 and <= 0x39)) {
        return false;
      }
      CBORObject cbor, cbor2, cbored, cbor3;
      var jsoptions = new JSONOptions("numberconversion=full");
      string str = DataUtilities.GetUtf8String(bytes, true);
      var ed = EDecimal.FromString(str);
      // Test consistency between JSON conversion methods
      cbor = CBORObject.FromJSONBytes(bytes, jsoptions);
      cbor2 = CBORDataUtilities.ParseJSONNumber(str, jsoptions);
      cbor3 = CBORObject.FromJSONString(str, jsoptions);
      cbored = (ed.Exponent.CompareTo(0) == 0 && !(ed.IsNegative && ed.Sign
            == 0)) ?
        CBORObject.FromEInteger(ed.Mantissa) : CBORObject.FromEDecimal(ed);
      Assert.AreEqual(cbor, cbor2, "[" + str + "] cbor2");
      Assert.AreEqual(cbor, cbor3, "[" + str + "] cbor3");
      Assert.AreEqual(cbor, cbored, "[" + str + "] cbored");
      return true;
    }

    public static bool TestEquivJSONNumberDecimal128One(byte[] bytes) {
      // Assume the JSON begins and ends with a digit
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      if (!(bytes.Length > 0)) {
        return false;
      }
      if (bytes[0] is not ((>= 0x30 and <= 0x39) or (byte)'-')) {
        return false;
      }
      if (bytes[^1] is not (>= 0x30 and <= 0x39)) {
        return false;
      }
      CBORObject cbor, cbor2, cbored, cbor3;
      var jsoptions = new JSONOptions("numberconversion=decimal128");
      string str = DataUtilities.GetUtf8String(bytes, true);
      // Test consistency between JSON conversion methods
      var ed = EDecimal.FromString(str, EContext.Decimal128);
      cbor = CBORObject.FromJSONBytes(bytes, jsoptions);
      cbor2 = CBORDataUtilities.ParseJSONNumber(str, jsoptions);
      cbor3 = CBORObject.FromJSONString(str, jsoptions);
      cbored = CBORObject.FromEDecimal(ed);
      Assert.AreEqual(cbor, cbor2, "[" + str + "] cbor2");
      Assert.AreEqual(cbor, cbor3, "[" + str + "] cbor3");
      Assert.AreEqual(cbor, cbored, "[" + str + "] cbored");
      return true;
    }

    public static void TestCompareToOne(byte[] bytes) {
      var cbor = CBORObject.DecodeFromBytes(bytes, new
          CBOREncodeOptions("allowduplicatekeys=1"));
      byte[] bytes2 = cbor.EncodeToBytes();
      var cbor2 = CBORObject.DecodeFromBytes(bytes2);
      if (!cbor.Equals(cbor2)) {
        string sbytes = TestCommon.ToByteArrayString(bytes) +
          "\ncbor=" + cbor +
          "\ncborbytes=" + TestCommon.ToByteArrayString(bytes2) +
          "\ncbor2=" + cbor2 +
          "\ncborbytes2=" + TestCommon.ToByteArrayString(cbor2.EncodeToBytes());
        Assert.AreEqual(cbor, cbor2, sbytes);
      } else {
        Assert.AreEqual(cbor, cbor2);
      }
      if (cbor.CompareTo(cbor2) != 0) {
        string sbytes = TestCommon.ToByteArrayString(bytes) +
          "\ncbor=" + cbor +
          "\ncborbytes=" + TestCommon.ToByteArrayString(bytes2) +
          "\ncbor2=" + cbor2 +
          "\ncborbytes2=" + TestCommon.ToByteArrayString(cbor2.EncodeToBytes());
        Assert.AreEqual(0, cbor.CompareTo(cbor2), sbytes);
      } else {
        Assert.AreEqual(0, cbor.CompareTo(cbor2));
      }
    }

    [Test]
    public void TestCompareToSpecificA() {
      var bytes = new byte[] { 0xfa, 0xb3, 0x00, 0x00, 0x00 };
      TestCompareToOne(bytes);
    }

    [Test]
    public void TestCompareToSpecificE() {
      var bytes = new byte[] {
        0xbf,
        0xf9,
        0xce,
        0xdc,
        0x99, 0x00, 0x01,
        0xf8,
        0xa0, 0x61, 0x37, 0x12, 0x7f, 0x78, 0x0d, 0x1c, 0x78, 0x4a,
        0x48, 0x3e,
        0xe1,
        0xa5,
        0xb2,
        0xf4,
        0x82,
        0x8f,
        0x8a, 0x32, 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04,
        0x2d, 0x57, 0x55, 0x08, 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x02, 0x41, 0x28,
        0xff, 0xe3, 0xff,
      };
      TestCompareToOne(bytes);
    }

    [Test]
    public void TestCompareToSpecificC() {
      var bytes = new byte[] {
        0xb9, 0x00, 0x02,
        0xfa,
        0x93,
        0x96,
        0xf3,
        0xcb, 0x1b,
        0xe7, 0x65, 0x72,
        0x83,
        0xa0, 0x39,
        0xa0,
        0xfe, 0x7f, 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
        0x2e, 0x7a, 0x00, 0x00, 0x00, 0x03, 0x1e, 0x33, 0x52, 0x60, 0x7a, 0x00,
        0x00, 0x00, 0x03, 0x62, 0x1e, 0x23,
        0xff, 0x18, 0x89,
      };
      TestCompareToOne(bytes);
    }

    [Test]
    public void TestCompareToSpecificD() {
      var bytes = new byte[] {
        0xbf, 0x00, 0x00,
        0xe0, 0x00, 0x7f, 0x78, 0x10, 0x64, 0x6b, 0x05, 0x77, 0x38, 0x3c,
        0x51, 0x66, 0x7c, 0x02, 0x31, 0x51, 0x56, 0x33, 0x56, 0x6a, 0x7b, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x70, 0x16, 0x20, 0x2f, 0x29,
        0x1a, 0x1f, 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x78,
        0x01, 0x5c,
        0xff, 0xfa, 0xa1, 0xeb, 0xc3, 0x1d,
        0xff,
      };
      TestCompareToOne(bytes);
    }

    [Test]
    public void TestCompareToSpecificB() {
      var bytes = new byte[] {
        0xa4,
        0xe3,
        0xf8, 0x70,
        0xdb, 0x02, 0x2d, 0x0d, 0x30, 0x39, 0x14,
        0xf5,
        0x8c, 0x39, 0x56, 0x1c, 0x3a,
        0x92, 0x27, 0x00, 0x04, 0x39, 0x1e, 0x05,
        0xf9, 0x73,
        0xac, 0x7f, 0x78, 0x05, 0x2d,
        0xe5,
        0xad,
        0xb8, 0x0b, 0x63, 0x27, 0x50, 0x7e, 0x78, 0x02, 0x04, 0x56,
        0xff, 0x1b,
        0x9d, 0x8c, 0x66, 0xaf, 0x18, 0x1d, 0x01,
        0x8e,
      };
      TestCompareToOne(bytes);
    }

    [Test]
    public void TestCompareToSpecific() {
      byte[] bytes;
      bytes = new byte[] {
        0xa2,
        0xf8,
        0xf7, 0x19,
        0xde,
        0x91, 0x7f, 0x79, 0x00, 0x11, 0x7b, 0x1b, 0x29, 0x59, 0x57, 0x6a,
        0x70, 0x68,
        0xe3,
        0x98,
        0xba, 0x6a, 0x49, 0x50, 0x54, 0x0b, 0x21, 0x62, 0x32, 0x17, 0x7b,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x67, 0x43, 0x37, 0x42,
        0x5f, 0x22, 0x7c, 0x0e, 0x68, 0x13, 0x74, 0x43, 0x1e, 0x4c, 0x5b, 0x2b,
        0x6c, 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7a, 0x00,
        0x00, 0x00, 0x00, 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x78, 0x01, 0x38, 0x78, 0x00, 0x78, 0x00, 0x7b, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x01, 0x39,
        0xff, 0x9f, 0xff,
      };
      TestCompareToOne(bytes);
    }

    [Test]
    public void TestCompareB1() {
      byte[] bytes;
      CBORObject o;
      bytes = new byte[] {
        0xbb, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x02,
        0xf8, 0x2d, 0x11, 0x7f, 0x79, 0x00, 0x2e, 0x7c, 0x2c, 0x18, 0x40,
        0x3e,
        0xc7,
        0xa9, 0x0c, 0x57, 0x50, 0x63, 0x30, 0x0f, 0x07, 0x76, 0x14, 0x31,
        0x52, 0x5c, 0x0a, 0x43, 0x4a, 0x6f, 0x08, 0x11, 0x25, 0x0b, 0x1a, 0x10,
        0x74,
        0xf1,
        0x84,
        0xbd,
        0x93, 0x4f, 0x74, 0x23, 0x5b, 0x7c, 0x5c, 0x76, 0x70, 0x0a,
        0xde,
        0xa3, 0x5e, 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0b,
        0x76,
        0xf0,
        0xad,
        0xbf,
        0xba, 0x14, 0x45, 0x0d, 0x2e, 0x6e, 0x62, 0x62, 0x10, 0x63,
        0xff, 0x35,
      };
      o = CBORObject.DecodeFromBytes(bytes, new
          CBOREncodeOptions("allowduplicatekeys=1"));
      CBORTestCommon.AssertRoundTrip(o);
      bytes = new byte[] {
        0xd9, 0x0e, 0x02,
        0xbf, 0x7f, 0x78, 0x07, 0x12, 0x45, 0x2f, 0x48,
        0xc8,
        0xb7, 0x5a, 0x79, 0x00, 0x01, 0x5e, 0x7b, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x01, 0x72, 0x78, 0x00, 0x7a, 0x00, 0x00, 0x00, 0x01,
        0x49, 0x61, 0x6d, 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
        0x13,
        0xff,
        0xed,
        0xfb,
        0x82, 0x18,
        0xc9, 0x6c, 0x3b, 0xc0, 0x53, 0x1f, 0xeb,
        0xff,
      };
      o = CBORObject.DecodeFromBytes(bytes, new
          CBOREncodeOptions("allowduplicatekeys=1"));
      CBORTestCommon.AssertRoundTrip(o);
      bytes = new byte[] {
        0xbf,
        0xfa,
        0xc5, 0x7f, 0x16,
        0xe2,
        0xf9, 0x05, 0x2d, 0x7f, 0x79, 0x00, 0x02, 0x4f, 0x0a, 0x67, 0x1a,
        0x17, 0x17, 0x1d, 0x0a, 0x74, 0x0a, 0x79, 0x00, 0x0e, 0x48, 0x23, 0x4e,
        0x32, 0x53, 0x74, 0x78,
        0xf0,
        0xa9,
        0x8b,
        0xb9, 0x03, 0x68, 0x3b, 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x02, 0x67, 0x0e, 0x7a, 0x00, 0x00, 0x00, 0x02, 0x74, 0x37, 0x79,
        0x00, 0x09, 0x6f, 0x11, 0x60, 0x3c, 0x24, 0x13, 0x16, 0x25, 0x35, 0x78,
        0x01, 0x6a,
        0xff, 0xf9, 0xc0, 0x69, 0x19, 0x0b, 0x8a, 0x05,
        0xff,
      };
      o = CBORObject.DecodeFromBytes(bytes, new
          CBOREncodeOptions("allowduplicatekeys=1"));
      CBORTestCommon.AssertRoundTrip(o);
    }

    [Test]
    public void TestCompareB() {
      {
        string stringTemp = CBORObject.DecodeFromBytes(new byte[] {
          0xfa,
          0x7f, 0x80, 0x00, 0x00,
        }).ToObject(typeof(ERational)).ToString();
        Assert.AreEqual(
          "Infinity",
          stringTemp);
      }
      {
        var objectTemp = CBORObject.DecodeFromBytes(new byte[] {
          0xc5, 0x82,
          0x38, 0xc7, 0x3b, 0x00, 0x00, 0x08, 0xbf, 0xda, 0xaf, 0x73, 0x46,
        });
        var objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          0x3b,
          0x5a, 0x9b, 0x9a, 0x9c, 0xb4, 0x95, 0xbf, 0x71,
        });
        AddSubCompare(objectTemp, objectTemp2);
      }
      {
        var objectTemp = CBORObject.DecodeFromBytes(new byte[] {
          0xfa, 0x1f,
          0x80, 0xdb, 0x9b,
        });
        var objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          0xfb,
          0x31, 0x90, 0xea, 0x16, 0xbe, 0x80, 0x0b, 0x37,
        });
        AddSubCompare(objectTemp, objectTemp2);
      }
      var cbor = CBORObject.FromCBORObjectAndTag(
          CBORObject.FromDouble(double.NegativeInfinity),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor = CBORObject.FromCBORObjectAndTag(
          ToObjectTest.TestToFromObjectRoundTrip(double.NegativeInfinity),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor = CBORObject.FromCBORObjectAndTag(
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor = CBORObject.FromCBORObjectAndTag(
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecNegInf),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
      cbor = CBORObject.FromCBORObjectAndTag(
          ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.FloatNegInf),
          1956611);
      CBORTestCommon.AssertRoundTrip(cbor);
    }

    [Test]
    public void TestEquivJSON() {
      var jsonBytes = new byte[] {
        0x22, 0x48, 0x54, 0x30, 0x43, 0x5c, 0x75,
        0x64, 0x61, 0x62, 0x43, 0x5c, 0x75, 0x64, 0x64, 0x32, 0x39, 0x48,
        0xdc,
        0x9a, 0x4e,
        0xc2, 0xa3, 0x49, 0x4d, 0x43, 0x40, 0x25, 0x31, 0x3b,
        0x22,
      };
      _ = TestEquivJSONOne(jsonBytes);
      jsonBytes = new byte[] {
        0x22, 0x35, 0x54, 0x30, 0x4d, 0x2d, 0x2b, 0x5c,
        0x75, 0x64, 0x38, 0x36, 0x38, 0x5c, 0x75, 0x44, 0x63, 0x46, 0x32, 0x4f,
        0x34, 0x4e, 0x34,
        0xe0, 0xa3, 0xac, 0x2b, 0x31, 0x23, 0x22,
      };
      _ = TestEquivJSONOne(jsonBytes);
    }

    [Test]
    public void TestDecFracCompareIntegerVsBigFraction() {
      var o1 = CBORObject.DecodeFromBytes(new byte[] {
        0xfb, 0x8b,
        0x44,
        0xf2, 0xa9, 0x0c, 0x27, 0x42, 0x28,
      });
      var o2 = CBORObject.DecodeFromBytes(new byte[] {
        0xc5, 0x82,
        0x38,
        0xa4, 0xc3, 0x50, 0x02, 0x98, 0xc5, 0xa8,
        0x02, 0xc1, 0xf6, 0xc0, 0x1a, 0xbe, 0x08,
        0x04, 0x86, 0x99, 0x3e, 0xf1,
      });
      AddSubCompare(o1, o2);
    }

    [Test]
    public void TestDecimalFrac() {
      CBORObject obj = CBORTestCommon.FromBytesTestAB(
          new byte[] { 0xc4, 0x82, 0x3, 0x1a, 1, 2, 3, 4 });
      try {
        Console.WriteLine(String.Empty + obj.ToObject(typeof(EDecimal)));
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestDecimalFracExactlyTwoElements() {
      CBORObject obj = CBORTestCommon.FromBytesTestAB(new byte[] {
        0xc4, 0x81,
        0xc2, 0x41,
        1,
      });
      try {
        Console.WriteLine(String.Empty + obj.ToObject(typeof(EDecimal)));
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
      var obj = CBORObject.DecodeFromBytes(new byte[] {
        0xc4,
        0x82,
        0xc2, 0x41, 1,
        0x1a,
        1, 2, 3, 4,
      });
      try {
        Console.WriteLine(String.Empty + obj.ToObject(typeof(EDecimal)));
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
      var cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xc5,
        0x82,
        0xc2, 0x41, 1,
        0x1a,
        1, 2, 3, 4,
      });
      try {
        Console.WriteLine(String.Empty + cbor.ToObject(typeof(EFloat)));
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
      CBORObject o = CBORTestCommon.FromBytesTestAB(
          new byte[] { 0xc4, 0x82, 0x3, 0xc2, 0x41, 1 });
      Assert.AreEqual(
        EDecimal.FromString("1e3"),
        o.ToObject(typeof(EDecimal)));
    }

    [Test]
    public void TestBigFloatFracMantissaMayBeBignum() {
      CBORObject o = CBORTestCommon.FromBytesTestAB(
          new byte[] { 0xc5, 0x82, 0x3, 0xc2, 0x41, 1 });
      {
        long numberTemp = EFloat.FromString("8").CompareTo(
            (EFloat)o.ToObject(typeof(EFloat)));

        Assert.AreEqual(0, numberTemp);
      }
    }

    [Test]
    public void TestDivide() {
      var r = new RandomGenerator();
      for (int i = 0; i < 3000; ++i) {
        CBORObject o1 = ToObjectTest.TestToFromObjectRoundTrip(
            RandomObjects.RandomEInteger(r));

        CBORObject o2 = ToObjectTest.TestToFromObjectRoundTrip(
            RandomObjects.RandomEInteger(r));

        if (o2.AsNumber().IsZero()) {
          continue;
        }
        var er = ERational.Create(AsEI(o1), AsEI(o2));
        {
          ERational objectTemp = er;
          ERational objectTemp2;
          CBORNumber cn = o1.AsNumber()
            .Divide(o2.AsNumber());
          objectTemp2 = cn.ToERational();
          TestCommon.CompareTestEqual(objectTemp, objectTemp2);
        }
      }
    }

    [Test]
    public void TestCBORCompareTo() {
      int cmp = CBORObject.FromInt32(0).CompareTo(null);
      if (cmp <= 0) {
        Assert.Fail();
      }
      cmp = CBORObject.FromInt32(0).AsNumber().CompareTo(null);
      if (cmp <= 0) {
        Assert.Fail();
      }
    }

    [Test]
    public void TestDouble() {
      if (!ToObjectTest.TestToFromObjectRoundTrip(
          double.PositiveInfinity).AsNumber().IsPositiveInfinity()) {
        Assert.Fail("Not positive infinity");
      }

      Assert.IsTrue(
        (
          (EDecimal)ToObjectTest.TestToFromObjectRoundTrip(
            double.PositiveInfinity)
          .ToObject(typeof(EDecimal))).IsPositiveInfinity());

      Assert.IsTrue(
        (
          (EDecimal)ToObjectTest.TestToFromObjectRoundTrip(
            double.NegativeInfinity)
          .ToObject(typeof(EDecimal))).IsNegativeInfinity());
      Assert.IsTrue(
        ((EDecimal)ToObjectTest.TestToFromObjectRoundTrip(double.NaN)

          .ToObject(typeof(EDecimal))).IsNaN());
      for (int i = -65539; i <= 65539; ++i) {
        CBORObject o = ToObjectTest.TestToFromObjectRoundTrip((double)i);
        Assert.IsTrue(o.AsNumber().CanFitInDouble());
        Assert.IsTrue(o.AsNumber().CanFitInInt32());
        Assert.IsTrue(o.AsNumber().IsInteger());
        CBORTestCommon.AssertJSONSer(
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
      _ = CBORTestCommon.CheckEncodeToBytes(cbor);
      // The following converts the map to JSON
      _ = cbor.ToJSONString();
    }

    [Test]
    [Timeout(5000)]
    public void TestExtendedExtremeExponent() {
      // Values with extremely high or extremely low exponents;
      // we just check whether this test method runs reasonably fast
      // for all these test cases
      CBORObject obj;
      obj = CBORObject.DecodeFromBytes(new byte[] {
        0xc4, 0x82,
        0x3a, 0x00, 0x1c, 0x2d, 0x0d, 0x1a, 0x13, 0x6c, 0xa1,
        0x97,
      });
      CBORTestCommon.AssertRoundTrip(obj);
      obj = CBORObject.DecodeFromBytes(new byte[] {
        0xda, 0x00, 0x14,
        0x57, 0xce, 0xc5, 0x82, 0x1a, 0x46, 0x5a, 0x37,
        0x87, 0xc3, 0x50, 0x5e, 0xec, 0xfd, 0x73,
        0x50, 0x64, 0xa1, 0x1f, 0x10, 0xc4, 0xff,
        0xf2, 0xc4, 0xc9, 0x65, 0x12,
      });
      CBORTestCommon.AssertRoundTrip(obj);
    }

    [Test]
    [Timeout(5000)]
    public void TestExtendedExtremeExponentCompare() {
      CBORObject cbor1 = ToObjectTest.TestToFromObjectRoundTrip(
          EDecimal.FromString("333333e-2"));
      CBORObject cbor2 = ToObjectTest.TestToFromObjectRoundTrip(
          EFloat.Create(
            EInteger.FromString("5234222"),
            EInteger.FromString("-24936668661488")));
      TestCommon.CompareTestGreater(cbor1.AsNumber(), cbor2.AsNumber());
    }

    [Test]
    public void TestFloat() {
      Assert.IsTrue(
        (
          (EDecimal)ToObjectTest.TestToFromObjectRoundTrip(
            float.PositiveInfinity)
          .ToObject(typeof(EDecimal))).IsPositiveInfinity());
      Assert.IsTrue(
        (
          (EDecimal)ToObjectTest.TestToFromObjectRoundTrip(
            float.NegativeInfinity)
          .ToObject(typeof(EDecimal))).IsNegativeInfinity());
      Assert.IsTrue(
        ((EDecimal)ToObjectTest.TestToFromObjectRoundTrip(float.NaN)

          .ToObject(typeof(EDecimal))).IsNaN());
      for (int i = -65539; i <= 65539; ++i) {
        CBORObject o = ToObjectTest.TestToFromObjectRoundTrip((float)i);
        // Console.Write("jsonser i=" + (// i) + " o=" + (o.ToString()) + " json=" +
        // (o.ToJSONString()) + " type=" + (o.Type));
        CBORTestCommon.AssertJSONSer(
          o,
          TestCommon.IntToString(i));
      }
    }

    [Test]
    public void TestHalfPrecision() {
      var o = CBORObject.DecodeFromBytes(
          new byte[] { 0xf9, 0x7c, 0x00 });
      if (o.AsSingle() != float.PositiveInfinity) {
        Assert.Fail();
      }
      o = CBORObject.DecodeFromBytes(
          new byte[] { 0xf9, 0x00, 0x00 });
      if (o.AsSingle() != 0f) {
        Assert.Fail();
      }
      o = CBORObject.DecodeFromBytes(
          new byte[] { 0xf9, 0xfc, 0x00 });
      if (o.AsSingle() != float.NegativeInfinity) {
        Assert.Fail();
      }
      o = CBORObject.DecodeFromBytes(
          new byte[] { 0xf9, 0x7e, 0x00 });
      Assert.IsTrue(float.IsNaN(o.AsSingle()));
    }

    [Test]
    public void TestTag268() {
      CBORObject cbor;
      CBORObject cbortag;
      for (int tag = 268; tag <= 269; ++tag) {
        cbor = CBORObject.NewArray().Add(-3).Add(99999).Add(0);
        cbortag = CBORObject.FromCBORObjectAndTag(cbor, tag);
        Assert.IsFalse(cbortag.AsNumber().IsNegative());
        cbor = CBORObject.NewArray().Add(-3).Add(99999);
        cbortag = CBORObject.FromCBORObjectAndTag(cbor, tag);
        try {
          Console.WriteLine(String.Empty + cbortag.ToObject(typeof(EDecimal)));
          Assert.Fail("Should have failed " + cbortag.ToString());
        } catch (InvalidOperationException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        cbor = CBORObject.NewArray().Add(-3).Add(99999).Add(1);
        cbortag = CBORObject.FromCBORObjectAndTag(cbor, tag);
        Assert.IsTrue(cbortag.AsNumber().IsNegative());
        cbor = CBORObject.NewArray().Add(-3).Add(99999).Add(-1);
        cbortag = CBORObject.FromCBORObjectAndTag(cbor, tag);
        try {
          Console.WriteLine(String.Empty + cbortag.ToObject(typeof(EDecimal)));
          Assert.Fail("Should have failed " + cbortag.ToString());
        } catch (InvalidOperationException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        cbor = CBORObject.NewArray().Add(-3).Add(99999).Add(2);
        cbortag = CBORObject.FromCBORObjectAndTag(cbor, tag);
        try {
          Console.WriteLine(String.Empty + cbortag.ToObject(typeof(EDecimal)));
          Assert.Fail("Should have failed " + cbortag.ToString());
        } catch (InvalidOperationException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        cbor = CBORObject.NewArray().Add(0).Add(0).Add(2);
        cbortag = CBORObject.FromCBORObjectAndTag(cbor, tag);
        Assert.IsFalse(cbortag.AsNumber().IsNegative());
        cbor = CBORObject.NewArray().Add(0).Add(0).Add(3);
        cbortag = CBORObject.FromCBORObjectAndTag(cbor, tag);
        Assert.IsTrue(cbortag.AsNumber().IsNegative());
        cbor = CBORObject.NewArray().Add(-3).Add(99999).Add(8);
        cbortag = CBORObject.FromCBORObjectAndTag(cbor, tag);
        try {
          Console.WriteLine(String.Empty + cbortag.ToObject(typeof(EDecimal)));
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
    public void TestRoundTripNaN() {
      long doublennan = unchecked((long)0xfff8000000000000L);
      long doublepnan = unchecked(0x7ff8000000000000L);
      int singlennan = unchecked((int)0xffc00000);
      int singlepnan = unchecked(0x7fc00000);
      var halfnnan = 0xfe00;
      var halfpnan = 0x7e00;
      {
        object objectTemp = doublennan;
        object objectTemp2 = CBORObject.FromFloatingPointBits(doublennan,
          8).AsDoubleBits();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = doublepnan;
        object objectTemp2 = CBORObject.FromFloatingPointBits(doublepnan,
          8).AsDoubleBits();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = doublennan;
        object objectTemp2 = CBORObject.FromFloatingPointBits(singlennan,
          4).AsDoubleBits();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = doublepnan;
        object objectTemp2 = CBORObject.FromFloatingPointBits(singlepnan,
          4).AsDoubleBits();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = doublennan;
        object objectTemp2 = CBORObject.FromFloatingPointBits(halfnnan,
          2).AsDoubleBits();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = doublepnan;
        object objectTemp2 = CBORObject.FromFloatingPointBits(halfpnan,
          2).AsDoubleBits();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
    }

    [Test]
    public void TestPlist() {
      CBORObject o;
      o = CBORObject.FromJSONString("[1,2,null,true,false,\"\"]");
      _ = o.Add(new byte[] { 32, 33, 44, 55 });
      _ = o.Add(CBORObject.FromCBORObjectAndTag(CBORObject.FromInt32(9999), 1));
      Console.WriteLine(o.ToJSONString());
      Console.WriteLine(CBORPlistWriter.ToPlistString(o));
    }

    private static void AreEqualDouble(double a, double b) {
      if (a != b) {
        Assert.Fail(a + ", " + b);
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
      AreEqualDouble(1.5, actual);
      using (var ms2a = new Test.DelayingStream(new byte[] { })) {
        try {
          _ = CBORObject.ReadJSON(ms2a);
          Assert.Fail("Should have failed A");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var ms2b = new Test.DelayingStream(new byte[] { 0x20 })) {
        try {
          _ = CBORObject.ReadJSON(ms2b);
          Assert.Fail("Should have failed B");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      try {
        _ = CBORObject.FromJSONString(String.Empty);
        Assert.Fail("Should have failed C");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.FromJSONString("[.1]");
        Assert.Fail("Should have failed D");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.FromJSONString("[-.1]");
        Assert.Fail("Should have failed E");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.FromJSONString("\u0020");
        Assert.Fail("Should have failed F");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      {
        string stringTemp = CBORObject.FromJSONString(" true ").ToJSONString();
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
      var o = CBORObject.FromJSONString(
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
    [Timeout(100000)]
    public void TestLong() {
      long[] ranges = {
        0, 65539, 0xfffff000L, 0x100000400L,
        Int64.MaxValue - 1000,
        Int64.MaxValue,
        Int64.MinValue,
        Int64.MinValue + 1000,
      };
      ranges[0] = -65539;
      var jso = new JSONOptions("numberconversion=full");
      for (int i = 0; i < ranges.Length; i += 2) {
        long j = ranges[i];
        while (true) {
          CBORNumber cn = ToObjectTest.TestToFromObjectRoundTrip(j).AsNumber();
          Assert.IsTrue(cn.IsInteger());
          Assert.IsTrue(cn.CanFitInInt64());
          Assert.IsTrue(cn.CanTruncatedIntFitInInt64());
          string l2s = TestCommon.LongToString(j);
          Assert.AreEqual(
            ToObjectTest.TestToFromObjectRoundTrip(j),
            ToObjectTest.TestToFromObjectRoundTrip(EInteger.FromInt64(j)));
          var obj = CBORObject.FromJSONString(
            "[" + l2s + "]",
            jso);
          CBORTestCommon.AssertJSONSer(
            ToObjectTest.TestToFromObjectRoundTrip(j),
            l2s);
          CBORTestCommon.AssertJSONSer(
              obj,
              "[" + l2s + "]");
          if (j == ranges[i + 1]) {
            break;
          }
          ++j;
        }
      }
    }

    [Test]
    public void TestMap() {
      var cbor = CBORObject.FromJSONString("{\"a\":2,\"b\":4}");
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
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xbf, 0x61, 0x61, 2,
        0x61, 0x62, 4, 0xff,
      });
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
      _ = oo.Add(ToObjectTest.TestToFromObjectRoundTrip(0));
      var oo2 = CBORObject.NewMap();
      _ = oo2.Add(
        ToObjectTest.TestToFromObjectRoundTrip(1),
        ToObjectTest.TestToFromObjectRoundTrip(1368));
      var oo3 = CBORObject.NewMap();
      _ = oo3.Add(
        ToObjectTest.TestToFromObjectRoundTrip(2),
        ToObjectTest.TestToFromObjectRoundTrip(1625));
      var oo4 = CBORObject.NewMap();
      _ = oo4.Add(oo2, CBORObject.True);
      _ = oo4.Add(oo3, CBORObject.True);
      _ = oo.Add(oo4);
      CBORTestCommon.AssertRoundTrip(oo);
    }

    private static readonly JSONOptions FullJsonOptions = new
("numberconversion=full;preservenegativezero=false");

    public static void TestParseDecimalStringsOne(string r) {
      CBORObject o = ToObjectTest.TestToFromObjectRoundTrip(
          EDecimal.FromString(r));
      CBORObject o2 = CBORDataUtilities.ParseJSONNumber(r, FullJsonOptions);
      TestCommon.CompareTestEqual(o.AsNumber(), o2.AsNumber());
    }

    [Test]
    public void TestJSONWithComments() {
      IDictionary<string, string> dict;
      string str = "[\n {\n # Bm\n\"a\":1,\n\"b\":2\n},{\n #" +
        "\u0020Sm\n\"a\":3,\n\"b\":4\n}\n]";
      CBORObject obj = JSONWithComments.FromJSONString(str);
      Console.WriteLine(obj);
      str = "[\n {\n # B\n # Dm\n\"a\":1,\n\"b\":2\n},{\n #" +
        "\u0020Sm\n\"a\":3,\n\"b\":4\n}\n]";
      obj = JSONWithComments.FromJSONString(str);
      Console.WriteLine(obj);
      str = "[\n {\n # B A C\n # Dm\n\"a\":1,\n\"b\":2\n},{\n #" +
        "\u0020Sm\n\"a\":3,\n\"b\":4\n}\n]";
      obj = JSONWithComments.FromJSONString(str);
      Console.WriteLine(obj);
      str = "[\n {\n # B\t \tA C\n # Dm\n\"a\":1,\n\"b\":2\n},{\n #" +
        "\u0020Sm\n\"a\":3,\n\"b\":4\n}\n]";
      obj = JSONWithComments.FromJSONString(str);
      Console.WriteLine(obj);
      dict = new Dictionary<string, string>();
      str = "{\"f\":[\n {\n # B\t \tA C\n # Dm\n\"a\":1,\n\"b\":2\n},{\n #" +
        "\u0020Sm\n\"a\":3,\n\"b\":4\n}\n]}";
      obj = JSONWithComments.FromJSONString(str);
      Console.WriteLine(obj);
      obj = JSONWithComments.FromJSONStringWithPointers(str, dict);
      foreach (string key in dict.Keys) {
        Console.WriteLine(key);
        Console.WriteLine(dict[key]);
        Console.WriteLine(obj.AtJSONPointer(dict[key]));
      }
      Console.WriteLine(obj);
    }

    [Test]
    public void TestParseDecimalStrings() {
      var rand = new RandomGenerator();
      for (int i = 0; i < 3000; ++i) {
        TestParseDecimalStringsOne(RandomObjects.RandomDecimalString(rand));
      }
    }

    [Test]
    [Timeout(200000)]
    public void TestRandomData() {
      var rand = new RandomGenerator();
      CBORObject obj;
      for (int i = 0; i < 1000; ++i) {
        obj = CBORTestCommon.RandomCBORObject(rand);
        CBORTestCommon.AssertRoundTrip(obj);
        string jsonString;
        try {
          jsonString = obj.ToJSONString();
        } catch (InvalidOperationException ex) {
          Console.WriteLine(TestCommon.ToByteArrayString(obj.EncodeToBytes()));
          throw new InvalidOperationException(ex.Message, ex);
        } catch (CBORException) {
          jsonString = String.Empty;
        }
        if (jsonString.Length > 0) {
          _ = CBORObject.FromJSONString(jsonString);
          TestWriteToJSON(obj);
        }
      }
    }

    public static CBORObject ReferenceTestObject() {
      return ReferenceTestObject(50);
    }

    public static CBORObject ReferenceTestObject(int nests) {
      var root = CBORObject.NewArray();
      CBORObject arr = CBORObject.NewArray().Add("xxx").Add("yyy");
      _ = arr.Add("zzz")
      .Add("wwww").Add("iiiiiii").Add("aaa").Add("bbb").Add("ccc");
      arr = CBORObject.FromCBORObjectAndTag(arr, 28);
      _ = root.Add(arr);
      CBORObject refobj;
      for (int i = 0; i <= nests; ++i) {
        refobj = CBORObject.FromCBORObjectAndTag(CBORObject.FromInt32(i), 29);
        arr = CBORObject.FromCBORArray(new CBORObject[] {
          refobj, refobj, refobj, refobj, refobj, refobj, refobj, refobj,
          refobj,
        });
        arr = CBORObject.FromCBORObjectAndTag(arr, 28);
        _ = root.Add(arr);
      }
      return root;
    }

    [Test]
    [Timeout(5000)]
    public void TestCtap2CanonicalReferenceTest() {
      for (int i = 4; i <= 60; ++i) {
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
      var encodeOptions = new CBOREncodeOptions("resolvereferences=true");
      root = CBORObject.DecodeFromBytes(bytes, encodeOptions);
      encodeOptions = new CBOREncodeOptions("ctap2canonical=true");
      if (root == null) {
        Assert.Fail();
      }
      try {
        using var lms = new Test.DelayingStream();
        root.WriteTo(lms, encodeOptions);
        Assert.Fail("Should have failed");
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
      for (int i = 5; i <= 60; ++i) {
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
      for (int i = 5; i <= 60; ++i) {
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
      _ = CBORObject.DecodeFromBytes(bytes, encodeOptions);
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
        using var lms = new LimitedMemoryStream(100000);
        root.WriteTo(lms);
        Assert.Fail("Should have failed");
      } catch (NotSupportedException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        using var lms = new LimitedMemoryStream(100000);
        origroot.WriteTo(lms);
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
      _ = CBORObject.DecodeFromBytes(bytes, encodeOptions);
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
        using var lms = new LimitedMemoryStream(100000);
        root.WriteJSONTo(lms);
        Assert.Fail("Should have failed");
      } catch (NotSupportedException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        using var lms = new LimitedMemoryStream(100000);
        origroot.WriteJSONTo(lms);
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
        0x82, 0xd8, 0x1c, 0x00, 0xd8,
        0x1d, 0x00,
      };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, encodeOptions);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Shared ref is negative
      bytes = new byte[] {
        0x82, 0xd8, 0x1c, 0x00, 0xd8,
        0x1d, 0x20,
      };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, encodeOptions);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Shared ref is non-integer
      bytes = new byte[] {
        0x82, 0xd8, 0x1c, 0x00, 0xd8,
        0x1d, 0xc4, 0x82,
        0x27, 0x19, 0xff, 0xff,
      };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, encodeOptions);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Shared ref is non-number
      bytes = new byte[] {
        0x82, 0xd8, 0x1c, 0x00, 0xd8,
        0x1d, 0x61, 0x41,
      };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, encodeOptions);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Shared ref is out of range
      bytes = new byte[] {
        0x82, 0xd8, 0x1c, 0x00, 0xd8,
        0x1d, 0x01,
      };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, encodeOptions);
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
      for (int i = 0; i < 1000; ++i) {
        var array = new byte[rand.UniformInt(100000) + 1];
        _ = rand.GetBytes(array, 0, array.Length);
        TestRandomOne(array);
      }
    }

    public static byte[] SlightlyModify(byte[] array,
      IRandomGenExtended rand) {
      if (array == null) {
        throw new ArgumentNullException(nameof(array));
      }
      if (array.Length > 50000) {
        Console.WriteLine(String.Empty + array.Length);
      }
      if (rand == null) {
        throw new ArgumentNullException(nameof(rand));
      }
      int count2 = rand.GetInt32(10) + 1;
      for (int j = 0; j < count2; ++j) {
        int index = rand.GetInt32(array.Length);
        array[index] = unchecked((byte)rand.GetInt32(256));
      }
      return array;
    }

    public static void TestRandomOne(byte[] array) {
      using var inputStream = new Test.DelayingStream(array);
      while (inputStream.Position != inputStream.Length) {
        long oldPos = 0L;
        try {
          CBORObject o;
          oldPos = inputStream.Position;
          o = CBORObject.Read(inputStream);
          long cborlen = inputStream.Position - oldPos;
          // if (cborlen > 3000) {
          // Console.WriteLine("pos=" + inputStream.Position + " of " +
          // inputStream.Length + ", cborlen=" + cborlen);
          // }
          byte[] encodedBytes = o?.EncodeToBytes();
          try {
            _ = CBORObject.DecodeFromBytes(encodedBytes);
          } catch (Exception ex) {
            throw new InvalidOperationException(ex.Message, ex);
          }
          string jsonString = String.Empty;
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
                _ = CBORObject.FromJSONString(jsonString);
                TestWriteToJSON(o);
              }
            }
          } catch (Exception ex) {
            throw new InvalidOperationException(ex.Message, ex);
          }
        } catch (CBORException ex) {
          // Expected exception
          string exmessage = ex.Message;
          Console.Write(exmessage[..0]);
        } catch (InvalidOperationException ex) {
          string failString = ex.ToString() +
            (ex.InnerException == null ? String.Empty : "\n" +
              ex.InnerException.ToString());
          failString += "\nlength: " + array.Length + " bytes";
          failString += "\nstart pos: " + oldPos + ", truelen=" +
            (inputStream.Position - oldPos);
          failString += "\n" + TestCommon.ToByteArrayString(array);
          int endPos = Math.Min(2000, failString.Length);
          failString = failString[..endPos];
          throw new InvalidOperationException(failString, ex);
        }
      }
    }

    [Test]
    public void TestRandomSlightlyModified() {
      var rand = new RandomGenerator();
      // Test slightly modified objects
      for (int i = 0; i < 2000; ++i) {
        CBORObject originalObject = CBORTestCommon.RandomCBORObject(rand);
        byte[] array = originalObject.EncodeToBytes();
        // Console.WriteLine("i=" + i + " obj=" + array.Length);
        TestRandomOne(SlightlyModify(array, rand));
      }
    }

    private static void TestReadWriteIntOne(int val) {
      try {
        {
          using var ms = new Test.DelayingStream();
          MiniCBOR.WriteInt32(val, ms);
          byte[] msarray = ms.ToArray();
          using var ms2 = new Test.DelayingStream(msarray);
          Assert.AreEqual(
            val,
            MiniCBOR.ReadInt32(ms2),
            TestCommon.ToByteArrayString(msarray));
        }
        {
          using var ms = new Test.DelayingStream();
          CBORObject.Write(val, ms);
          byte[] msarray = ms.ToArray();
          using var ms2 = new Test.DelayingStream(msarray);
          Assert.AreEqual(
            val,
            MiniCBOR.ReadInt32(ms2),
            TestCommon.ToByteArrayString(msarray));
        }
      } catch (IOException ioex) {
        Assert.Fail(ioex.Message + " val=" + val);
      }
    }

    public static EInteger UnsignedLongToEInteger(long v) {
      return v >= 0 ? EInteger.FromInt64(v) :
EInteger.FromInt32(1).ShiftLeft(64).Add(v);
    }

    public static void TestUnsignedLongOne(long v, string expectedStr) {
      EInteger ei = UnsignedLongToEInteger(v);

      Assert.AreEqual(
        expectedStr,
        DataUtilities.ToLowerCaseAscii(ei.ToRadixString(16)));
      var cbor = CBORObject.FromEInteger(ei);
      Assert.IsTrue(cbor.AsNumber().Sign >= 0);
      TestCommon.AssertEqualsHashCode(
        ei,
        cbor.ToObject(typeof(EInteger)));
    }

    [Test]
    public void TestUnsignedLong() {
      TestUnsignedLongOne(0x0L, "0");
      TestUnsignedLongOne(0xFL, "f");
      TestUnsignedLongOne(0xFFFFFFFFL, "ffffffff");
      TestUnsignedLongOne(-1, "ffffffffffffffff");
      TestUnsignedLongOne(-3, "fffffffffffffffd");
      TestUnsignedLongOne(Int64.MaxValue, "7fffffffffffffff");
      TestUnsignedLongOne(Int64.MaxValue - 1, "7ffffffffffffffe");
      TestUnsignedLongOne(Int64.MinValue, "8000000000000000");
      TestUnsignedLongOne(Int64.MinValue + 1, "8000000000000001");
    }

    [Test]
    public void TestReadWriteInt() {
      var r = new RandomGenerator();
      for (int i = -70000; i < 70000; ++i) {
        TestReadWriteIntOne(i);
      }
      for (int i = 0; i < 100000; ++i) {
        int val = unchecked((int)RandomObjects.RandomInt64(r));
        TestReadWriteIntOne(val);
      }
    }

    [Test]
    public void TestShort() {
      for (int i = short.MinValue; i <= short.MaxValue; ++i) {
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
        ToObjectTest.TestToFromObjectRoundTrip(null),
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
        _ = o.EncodeToBytes(ctap);
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
        _ = o.EncodeToBytes(ctap);
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
        _ = o.EncodeToBytes(ctap);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    private static string Chop(string str) {
      return (str.Length < 100) ? str : (str[..100] + "...");
    }

    private static void VerifyEqual(
      CBORNumber expected,
      CBORNumber actual,
      CBORObject o1,
      CBORObject o2) {
      if (expected.CompareTo(actual) != 0) {
        string msg = "o1=" + Chop(o1.ToString()) + ", o2=" +
          Chop(o2.ToString());
        TestCommon.CompareTestEqual(expected, actual, msg);
      }
    }

    [Test]
    public void TestRational1() {
      var eb1 = new byte[] {
        0xd8, 0x1e,
        0x82,
        0xc2, 0x58, 0x22, 0x24,
        0xba, 0x21,
        0xf3,
        0xa9,
        0xfb, 0x1c,
        0xde,
        0xc5, 0x49,
        0xd2, 0x2c,
        0x94, 0x27,
        0xb1, 0x0d, 0x36,
        0xea, 0x1c,
        0xcb, 0x5d,
        0xe9, 0x13,
        0xef,
        0xf2, 0x2c,
        0xbf,
        0xc8,
        0xad, 0x42,
        0x8a,
        0xae, 0x65,
        0x85,
        0xc2, 0x58, 0x19, 0x74,
        0xf5, 0x20, 0x74, 0x43,
        0xd4,
        0xdf,
        0x93, 0x12,
        0xc3,
        0x89,
        0xdd, 0x53, 0x62,
        0xdf, 0x5c, 0x66, 0x2f, 0x4d,
        0xbd, 0x7e, 0x57, 0xdd, 0x91, 0x6c,
      };
      Console.WriteLine(String.Empty +
        CBORObject.DecodeFromBytes(eb1).ToObject(typeof(ERational)));
      TestRandomOne(eb1);
    }

    [Test]
    public void TestRational2() {
      var eb1 = new byte[] {
        0xd8, 0x1e,
        0x82,
        0xc2, 0x58, 0x18, 0x2d,
        0x8e, 0x6b, 0x70, 0x4e,
        0xf2,
        0xc9, 0x15,
        0xe3, 0x34, 0x5f, 0x7c,
        0xbb, 0x07, 0x22,
        0xd3, 0x40, 0x37, 0x52,
        0xbd, 0x75, 0x3a, 0x4b,
        0xe0,
        0xc2, 0x51, 0x28, 0x42,
        0x81,
        0x93, 0x22, 0x6e,
        0x94, 0x4d,
        0xff, 0xdb, 0x45, 0x97, 0x0c, 0x56, 0x04, 0xe3,
        0x21,
      };
      Console.WriteLine(String.Empty +
        CBORObject.DecodeFromBytes(eb1).ToObject(typeof(ERational)));
      TestRandomOne(eb1);
    }

    [Test]
    public void TestRational3() {
      var eb1 = new byte[] {
        0xd8, 0x1e,
        0x82, 0x1b, 0x00, 0x00, 0x26,
        0xbd, 0x75, 0x51,
        0x9a, 0x7b,
        0xc2, 0x57, 0x0c,
        0xb4, 0x04,
        0xe3, 0x21,
        0xf0,
        0xb6, 0x2d,
        0xd3, 0x6b,
        0xd8, 0x4e,
        0xf2,
        0xc9, 0x15, 0xe3, 0x34, 0xa2, 0x16, 0x07, 0x07, 0x0d,
        0xd3,
      };
      Console.WriteLine(String.Empty +
        CBORObject.DecodeFromBytes(eb1).ToObject(typeof(ERational)));
      TestRandomOne(eb1);
    }

    [Test]
    [Timeout(60000)]
    public void TestAsNumberAddSubtractSpecific() {
      var eb1 = new byte[] {
        0xd9, 0x01, 0x08,
        0x82,
        0xc3, 0x57, 0x0f,
        0xf2,
        0xa2,
        0x97, 0x0b,
        0xee,
        0xa8,
        0x9c,
        0xa1, 0x3f, 0x7b, 0x22, 0x5f,
        0x82, 0x4f,
        0xfa, 0x3d,
        0xaa,
        0xfc, 0x27, 0x64,
        0xf0, 0x2f,
        0xc2, 0x58, 0x19, 0x16, 0x01,
        0xe6, 0x6a, 0x7f,
        0xe4,
        0x90,
        0x9e, 0x28, 0x33, 0x1d,
        0x87,
        0xcd, 0x1e, 0x37,
        0xdb, 0x5d,
        0xd1, 0xc2, 0xc9, 0x40, 0xa6, 0x1b,
        0xb5, 0x87,
      };
      var eb2 = new byte[] {
        0xc5,
        0x82, 0x18,
        0xbe,
        0xc2, 0x58, 0x26, 0x06, 0x5d, 0x42,
        0xc3,
        0x88,
        0xbe,
        0x86,
        0xbe, 0x15,
        0x9f,
        0x99,
        0x81,
        0x96,
        0xa6,
        0xac, 0x4b, 0x37,
        0xb4, 0x43,
        0xf8, 0x17, 0x6d, 0x7e, 0x10, 0x38,
        0xda, 0x65,
        0x90,
        0xa9,
        0x80, 0xef, 0xa3, 0x65, 0xca, 0x7d, 0x4f,
        0xa8, 0x27,
      };
      var cbor1 = CBORObject.DecodeFromBytes(eb1);
      var cbor2 = CBORObject.DecodeFromBytes(eb2);
      EDecimal ed1 = AsED(cbor1);
      EDecimal ed2 = AsED(cbor2);
      // Should return NaN due to memory issues
      ed1 = ed1.Add(ed2);
      Assert.IsTrue(ed1.IsNaN());
    }

    [Test]
    [Timeout(60000)]
    public void TestAsNumberAddSubtractSpecific2() {
      var eb1 = new byte[] {
        0xc4,
        0x82, 0x1b, 0x00, 0x00, 0x00, 0x6e, 0x1c, 0x51, 0x6c, 0x6e,
        0xc3, 0x4f, 0x7c, 0x0f, 0x6e, 0x1d,
        0x89, 0x26,
        0x8d, 0x57, 0xec, 0x00, 0x54, 0xb9, 0x51,
        0xae, 0x43,
      };
      var eb2 = new byte[] { 0xfa, 0x75, 0x00, 0x57, 0xbe };
      var cbor1 = CBORObject.DecodeFromBytes(eb1);
      var cbor2 = CBORObject.DecodeFromBytes(eb2);
      EDecimal ed1 = AsED(cbor1);
      EDecimal ed2 = AsED(cbor2);
      // Should return NaN due to memory issues
      ed1 = ed1.Add(ed2);
      Assert.IsTrue(ed1.IsNaN());
    }

    [Test]
    [Timeout(100000)]
    public void TestAsNumberAddSubtract() {
      var r = new RandomGenerator();
      for (int i = 0; i < 3000; ++i) {
        // NOTE: Avoid generating high-exponent numbers for this test
        CBORObject o1 = CBORTestCommon.RandomNumber(r, true);
        CBORObject o2 = CBORTestCommon.RandomNumber(r, true);
        _ = o1.EncodeToBytes();
        _ = o2.EncodeToBytes();
        CBORTestCommon.AssertRoundTrip(o1);
        CBORTestCommon.AssertRoundTrip(o2);
        CBORNumber on1 = o1.AsNumber();
        CBORNumber on2 = o2.AsNumber();
        // Console.WriteLine(i+"");
        // Console.WriteLine(i+" "+Chop(o1.ToString()));
        // Console.WriteLine(i+" "+Chop(o2.ToString()));
        // Console.WriteLine(i+" "+TestCommon.ToByteArrayString(eb1));
        // Console.WriteLine(i+" "+TestCommon.ToByteArrayString(eb2));
        CBORNumber onSum;
        try {
          onSum = on1.Add(on2);
        } catch (OutOfMemoryException) {
          continue;
        }
        if (!onSum.IsFinite()) {
          // Console.WriteLine("on1=" + o1);
          // Console.WriteLine("on2=" + o2);
          continue;
        }
        if (!onSum.IsFinite()) {
          Assert.Fail(o1.ToString());
        }
        CBORNumber on2a = onSum.Subtract(on1);
        if (!on2a.IsFinite()) {
          Assert.Fail(o1.ToString());
        }
        VerifyEqual(on2a, on2, o1, o2);
        CBORNumber on1a = onSum.Subtract(on2);
        if (!on1a.IsFinite()) {
          Assert.Fail(o1.ToString());
        }
        VerifyEqual(on1a, on1, o1, o2);
      }
    }

    public static bool TestAsNumberMultiplyDivideOne(
      CBORObject o1,
      CBORObject o2) {
      if (o1 == null) {
        throw new ArgumentNullException(nameof(o1));
      }
      if (o2 == null) {
        throw new ArgumentNullException(nameof(o2));
      }
      if (!o1.IsNumber || !o2.IsNumber) {
        return false;
      }
      byte[] eb1 = o1.EncodeToBytes();
      byte[] eb2 = o2.EncodeToBytes();
      CBORTestCommon.AssertRoundTrip(o1);
      CBORTestCommon.AssertRoundTrip(o2);
      CBORNumber on1 = o1.AsNumber();
      CBORNumber on2 = o2.AsNumber();
      CBORNumber onSum;
      try {
        onSum = on1.Multiply(on2);
      } catch (OutOfMemoryException) {
        return false;
      }
      if (!onSum.IsFinite()) {
        // Console.WriteLine("on1=" + o1);
        // Console.WriteLine("on2=" + o2);
        return false;
      }
      // Console.WriteLine(i+"");
      // Console.WriteLine(i+" "+Chop(o1.ToString()));
      // Console.WriteLine(i+" "+Chop(o2.ToString()));
      // Console.WriteLine(i + " " + Chop(onSum.ToString()));
      if (!onSum.IsFinite()) {
        Assert.Fail("onSum is not finite\n" +
          "o1=" + TestCommon.ToByteArrayString(eb1) + "\n" +
          "o2=" + TestCommon.ToByteArrayString(eb2) + "\n");
      }
      CBORNumber on2a = onSum.Divide(on1);
      // NOTE: Ignore if divisor is zero
      if (!on1.IsZero() && !on2a.IsFinite()) {
        Assert.Fail("on2a is not finite\n" +
          "o1=" + TestCommon.ToByteArrayString(eb1) + "\n" +
          "o2=" + TestCommon.ToByteArrayString(eb2) + "\n");
      }
      if (!on1.IsZero() && !on2.IsZero()) {
        VerifyEqual(on2a, on2, o1, o2);
      }
      CBORNumber on1a = onSum.Divide(on2);
      // NOTE: Ignore if divisor is zero
      if (!on2.IsZero() && !on1a.IsFinite()) {
        Assert.Fail("on1a is not finite\n" +
          "o1=" + on1 + "\n" + "o2=" + on2 + "\n" +
          "{\nbyte[] by" +
          "tes1 = " + TestCommon.ToByteArrayString(eb1) + ";\n" +
          "byte[] by" + "tes2 =" + TestCommon.ToByteArrayString(eb2) + ";\n" +
          "TestAsNumberMultiplyDivideOne(\nCBORObject.D" +
          "ecodeFromBytes(bytes1),\n" +
          "CBORObject.DecodeFromBytes(bytes2));\n}\n");
      }
      if (!on1.IsZero() && !on2.IsZero()) {
        VerifyEqual(on1a, on1, o1, o2);
      }
      return true;
    }

    [Test]
    [Timeout(100000)]
    public void TestAsNumberMultiplyDivide() {
      var bo1 = new byte[] {
        0x1b, 0x75, 0xdd, 0xb0,
        0xcc, 0x50, 0x9b, 0xd0, 0x2b,
      };
      var bo2 = new byte[] { 0xc5, 0x82, 0x23, 0x00 };
      var cbor1 = CBORObject.DecodeFromBytes(bo1);
      var cbor2 = CBORObject.DecodeFromBytes(bo2);
      _ = TestAsNumberMultiplyDivideOne(cbor1, cbor2);
      var r = new RandomGenerator();
      for (int i = 0; i < 3000; ++i) {
        CBORObject o1 = CBORTestCommon.RandomNumber(r);
        CBORObject o2 = CBORTestCommon.RandomNumber(r);
        _ = TestAsNumberMultiplyDivideOne(o1, o2);
      }
    }

    [Test]
    public void TestOrderedMap() {
      CBORObject cbor;
      IList<CBORObject> list;
      cbor = CBORObject.NewOrderedMap().Add("a", 1).Add("b", 2).Add("c", 3);
      list = new List<CBORObject>();
      foreach (CBORObject obj in cbor.Keys) {
        list.Add(obj);
      }
      Assert.AreEqual(3, list.Count);
      TestCommon.AssertEqualsHashCode(CBORObject.FromString("a"), list[0]);
      TestCommon.AssertEqualsHashCode(CBORObject.FromString("b"), list[1]);
      TestCommon.AssertEqualsHashCode(CBORObject.FromString("c"), list[2]);
      cbor = CBORObject.NewOrderedMap().Add("c", 1).Add("a", 2).Add("vv", 3);
      list = new List<CBORObject>();
      foreach (CBORObject obj in cbor.Keys) {
        list.Add(obj);
      }
      Assert.AreEqual(3, list.Count);
      TestCommon.AssertEqualsHashCode(CBORObject.FromString("c"), list[0]);
      TestCommon.AssertEqualsHashCode(CBORObject.FromString("a"), list[1]);
      TestCommon.AssertEqualsHashCode(CBORObject.FromString("vv"), list[2]);
      list = new List<CBORObject>();
      foreach (CBORObject obj in cbor.Values) {
        list.Add(obj);
      }
      Assert.AreEqual(3, list.Count);
      TestCommon.AssertEqualsHashCode(CBORObject.FromInt32(1), list[0]);
      TestCommon.AssertEqualsHashCode(CBORObject.FromInt32(2), list[1]);
      TestCommon.AssertEqualsHashCode(CBORObject.FromInt32(3), list[2]);
    }

    [Test]
    [Timeout(10000)]
    public void TestTaggedUntagged() {
      for (int i = 200; i < 1000; ++i) {
        CBORObject o, o2;
        o = ToObjectTest.TestToFromObjectRoundTrip(0);
        o2 = CBORObject.FromCBORObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromCBORObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o =
          ToObjectTest.TestToFromObjectRoundTrip(EInteger.FromString(
              "999999999999999999999999999999999"));
        o2 = CBORObject.FromCBORObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromCBORObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = ToObjectTest.TestToFromObjectRoundTrip(new byte[] { 1, 2, 3 });
        o2 = CBORObject.FromCBORObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromCBORObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.NewArray();
        _ = o.Add(ToObjectTest.TestToFromObjectRoundTrip(0));
        _ = o.Add(ToObjectTest.TestToFromObjectRoundTrip(1));
        _ = o.Add(ToObjectTest.TestToFromObjectRoundTrip(2));
        o2 = CBORObject.FromCBORObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromCBORObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.NewMap();
        _ = o.Add("a", ToObjectTest.TestToFromObjectRoundTrip(0));
        _ = o.Add("b", ToObjectTest.TestToFromObjectRoundTrip(1));
        _ = o.Add("c", ToObjectTest.TestToFromObjectRoundTrip(2));
        o2 = CBORObject.FromCBORObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromCBORObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = ToObjectTest.TestToFromObjectRoundTrip("a");
        o2 = CBORObject.FromCBORObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromCBORObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.False;
        o2 = CBORObject.FromCBORObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromCBORObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.True;
        o2 = CBORObject.FromCBORObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromCBORObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.Null;
        o2 = CBORObject.FromCBORObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromCBORObjectAndTag(o, i + 1);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.Undefined;
        o2 = CBORObject.FromCBORObjectAndTag(o, i);
        TestCommon.AssertEqualsHashCode(o, o2);
        o = CBORObject.FromCBORObjectAndTag(o, i + 1);
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
      var maxuint = EInteger.FromString("18446744073709551615");
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
      AssertEquals(
        EInteger.FromString("-1"),
        trueObj.MostInnerTag);
      _ = CBORObject.True.GetAllTags();
      for (int i = 0; i < ranges.Length; i += 2) {
        EInteger bigintTemp = ranges[i];
        while (true) {
          EInteger ei = bigintTemp;
          EInteger bigintNext = ei.Add(EInteger.One);
          if (bigintTemp.GetSignedBitLengthAsInt64() <=
            31) {
            int bc = ei.ToInt32Checked();
            if (bc is >= -1 and <= 37) {
              bigintTemp = bigintNext;
              continue;
            }
            if (bc is >= 264 and <= 270) {
              bigintTemp = bigintNext;
              continue;
            }
          }
          var obj = CBORObject.FromCBORObjectAndTag(CBORObject.FromInt32(0),
  bigintTemp);
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
            var obj2 = CBORObject.FromCBORObjectAndTag(obj, bigintNew);
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
      for (int i = 0; i <= 0x1f; ++i) {
        var bytes = new byte[] { 0xf8, (byte)i };
        try {
          _ = CBORObject.DecodeFromBytes(bytes);
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
      for (int i = 0; i < 2; ++i) {
        int eb = i == 0 ? 0 : 0x20;
        bytes = new byte[] { (byte)eb };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x17 + eb) };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x18 + eb), 0 };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x19 + eb), 0, 0 };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x1a + eb), 0, 0, 0, 0 };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x1b + eb), 0, 0, 0, 0, 0, 0, 0, 0 };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x18 + eb), 0x17 };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x18 + eb), 0x18 };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x19 + eb), 0, 0xff };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x19 + eb), 1, 0 };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x1a + eb), 0, 0, 0xff, 0xff };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x1a + eb), 0, 1, 0, 0 };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] {
          (byte)(0x1b + eb), 0, 0, 0, 0, 0xff, 0xff,
          0xff, 0xff,
        };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x1b + eb), 0, 0, 0, 1, 0, 0, 0, 0 };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      for (int i = 2; i <= 5; ++i) {
        int eb = 0x20 * i;
        bytes = new byte[] { (byte)eb };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x18 + eb), 0 };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x19 + eb), 0, 0 };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x1a + eb), 0, 0, 0, 0 };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
        bytes = new byte[] { (byte)(0x1b + eb), 0, 0, 0, 0, 0, 0, 0, 0 };
        try {
          _ = CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      bytes = new byte[] { 0xc0, 0 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xd7, 0 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xd8, 0xff, 0 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xd9, 0xff, 0xff, 0 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] {
        0xda, 0xff, 0xff, 0xff,
        0xff, 0,
      };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] {
        0xdb, 0xff, 0xff, 0xff,
        0xff, 0xff, 0xff, 0xff,
        0xff, 0,
      };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Nesting depth
      bytes = new byte[] { 0x81, 0x81, 0x81, 0x80 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0x81, 0x81, 0x81, 0x81, 0 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] {
        0x81, 0x81, 0x81, 0xa1,
        0, 0,
      };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] {
        0x81, 0x81, 0x81, 0x81,
        0x80,
      };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] {
        0x81, 0x81, 0x81, 0xa1,
        0, 0,
      };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] {
        0x81, 0x81, 0x81, 0x81,
        0xa0,
      };
      TestFailingDecode(bytes, options);
      // Floating Point Numbers
      bytes = new byte[] { 0xf9, 8, 8 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xfa, 8, 8, 8, 8 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xfb, 8, 8, 8, 8, 8, 8, 8, 8 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Map Key Ordering
      bytes = new byte[] { 0xa2, 0, 0, 1, 0 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xa2, 1, 0, 0, 0 };
      TestFailingDecode(bytes, options);
      bytes = new byte[] { 0xa2, 0, 0, 0x20, 0 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xa2, 0x20, 0, 0, 0 };
      TestFailingDecode(bytes, options);
      bytes = new byte[] { 0xa2, 0, 0, 0x38, 0xff, 0 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xa2, 0x38, 0xff, 0, 0, 0 };
      TestFailingDecode(bytes, options);
      bytes = new byte[] { 0xa2, 0x41, 0xff, 0, 0x42, 0, 0, 0 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xa2, 0x42, 0, 0, 0, 0x41, 0xff, 0 };
      TestFailingDecode(bytes, options);
      bytes = new byte[] { 0xa2, 0x61, 0x7f, 0, 0x62, 0, 0, 0 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xa2, 0x62, 0, 0, 0, 0x61, 0x7f, 0 };
      TestFailingDecode(bytes, options);
    }

    [Test]
    public void TestIndefLengthMore() {
      byte[] bytes;
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0x41, 0x31, 0xff };
      TestSucceedingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0x00, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0x18, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0x20, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0x38, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0x60, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0x61, 0x31, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0x80, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0x81, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0xa0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0xa1, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0xc0, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0xd8, 0xff, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0xe0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0xf8, 0xff, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x30, 0xf9, 0xff, 0xff, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] {
        0x5f, 0x41, 0x30, 0xfa, 0xff, 0xff, 0xff, 0xff,
        0xff,
      };
      TestFailingDecode(bytes);
      bytes = new byte[] {
        0x5f, 0x41, 0x30, 0xfb, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0xff, 0xff, 0xff,
      };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0x61, 0x31, 0xff };
      TestSucceedingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0x00, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0x18, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0x20, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0x38, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0x40, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0x41, 0x31, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0x80, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0x81, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0xa0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0xa1, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0xc0, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0xd8, 0xff, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0xe0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0xf8, 0xff, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x30, 0xf9, 0xff, 0xff, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] {
        0x7f, 0x61, 0x30, 0xfa, 0xff, 0xff, 0xff, 0xff,
        0xff,
      };
      TestFailingDecode(bytes);
      bytes = new byte[] {
        0x7f, 0x61, 0x30, 0xfb, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0xff, 0xff, 0xff,
      };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x41, 0x31, 0xff };
      TestSucceedingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x00, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x18, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x20, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x38, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x60, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x61, 0x31, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x80, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0x81, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0xa0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0xa1, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0xc0, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0xd8, 0xff, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0xe0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0xf8, 0xff, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0xf9, 0xff, 0xff, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x5f, 0xfa, 0xff, 0xff, 0xff, 0xff, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] {
        0x5f, 0xfb, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0xff,
      };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0x31, 0xff };
      TestSucceedingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x00, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x18, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x20, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x38, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x40, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x41, 0x31, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x80, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x81, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0xa0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0xa1, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0xc0, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0xd8, 0xff, 0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0xe0, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0xf8, 0xff, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0xf9, 0xff, 0xff, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0xfa, 0xff, 0xff, 0xff, 0xff, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] {
        0x7f, 0xfb, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0xff,
      };
      TestFailingDecode(bytes);
      // Indefinite-length string with one Unicode code point
      bytes = new byte[] { 0x7f, 0x62, 0xc2, 0x80, 0xff };
      TestSucceedingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x63, 0xe2, 0x80, 0x80, 0xff };
      TestSucceedingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x64, 0xf2, 0x80, 0x80, 0x80, 0xff };
      TestSucceedingDecode(bytes);
      // Disallow splitting code points in indefinite-length
      // text strings
      bytes = new byte[] { 0x7f, 0x61, 0xc2, 0x61, 0x80, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0xe2, 0x62, 0x80, 0x80, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x62, 0xe2, 0x80, 0x61, 0x80, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0xf2, 0x63, 0x80, 0x80, 0x80, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x62, 0xf2, 0x80, 0x62, 0x80, 0x80, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x63, 0xf2, 0x80, 0x80, 0x61, 0x80, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0xc2, 0x61, 0x20, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x62, 0xe2, 0x80, 0x61, 0x20, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x63, 0xf2, 0x80, 0x80, 0x61, 0x20, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x61, 0xc2, 0x62, 0x80, 0x20, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] { 0x7f, 0x62, 0xe2, 0x80, 0x62, 0x80, 0x20, 0xff };
      TestFailingDecode(bytes);
      bytes = new byte[] {
        0x7f, 0x63, 0xf2, 0x80, 0x80, 0x62, 0x80, 0x20,
        0xff,
      };
      TestFailingDecode(bytes);
    }

    private static void TestSucceedingDecode(byte[] bytes) {
      try {
        _ = CBORTestCommon.FromBytesTestAB(bytes);
      } catch (Exception ex) {
        Assert.Fail(TestCommon.ToByteArrayString(bytes) + "\n" +
          ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    private static void TestFailingDecode(byte[] bytes) {
      try {
        _ = CBORTestCommon.FromBytesTestAB(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(TestCommon.ToByteArrayString(bytes) + "\n" +
          ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(bytes);
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(TestCommon.ToByteArrayString(bytes) + "\n" +
          ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    private static void TestFailingDecode(byte[] bytes, CBOREncodeOptions
      options) {
      try {
        _ = CBORTestCommon.FromBytesTestAB(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(TestCommon.ToByteArrayString(bytes) + "\n" +
          ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    private static readonly int[][] ValueBadLesserFields = {
      new int[] { 0, 1, 0, 0, 0, 0, 0 },
      new int[] { -1, 1, 0, 0, 0, 0, 0 },
      new int[] { 1, 32, 0, 0, 0, 0, 0 },
      new int[] { 2, 30, 0, 0, 0, 0, 0 },
      new int[] { 3, 32, 0, 0, 0, 0, 0 },
      new int[] { 4, 31, 0, 0, 0, 0, 0 },
      new int[] { 5, 32, 0, 0, 0, 0, 0 },
      new int[] { 6, 31, 0, 0, 0, 0, 0 },
      new int[] { 7, 32, 0, 0, 0, 0, 0 },
      new int[] { 8, 32, 0, 0, 0, 0, 0 },
      new int[] { 9, 31, 0, 0, 0, 0, 0 },
      new int[] { 10, 32, 0, 0, 0, 0, 0 },
      new int[] { 11, 31, 0, 0, 0, 0, 0 },
      new int[] { 12, 32, 0, 0, 0, 0, 0 },
      new int[] { 13, 1, 0, 0, 0, 0, 0 },
      new int[] { Int32.MinValue, 1, 0, 0, 0, 0, 0 },
      new int[] { Int32.MaxValue, 1, 0, 0, 0, 0, 0 },
      new int[] { 1, 0, 0, 0, 0, 0, 0 },
      new int[] { 1, -1, 0, 0, 0, 0, 0 },
      new int[] { 1, Int32.MinValue, 0, 0, 0, 0, 0 },
      new int[] { 1, 32, 0, 0, 0, 0, 0 },
      new int[] { 1, Int32.MaxValue, 0, 0, 0, 0, 0 },
      new int[] { 1, 1, -1, 0, 0, 0, 0 },
      new int[] { 1, 1, Int32.MinValue, 0, 0, 0, 0 },
      new int[] { 1, 1, 24, 0, 0, 0, 0 },
      new int[] { 1, 1, 59, 0, 0, 0, 0 },
      new int[] { 1, 1, 60, 0, 0, 0, 0 },
      new int[] { 1, 1, Int32.MaxValue, 0, 0, 0, 0 },
      new int[] { 1, 1, 0, -1, 0, 0, 0 },
      new int[] { 1, 1, 0, Int32.MinValue, 0, 0, 0 },
      new int[] { 1, 1, 0, 60, 0, 0, 0 },
      new int[] { 1, 1, 0, Int32.MaxValue, 0, 0, 0 },
      new int[] { 1, 1, 0, 0, -1, 0, 0 },
      new int[] { 1, 1, 0, 0, Int32.MinValue, 0, 0 },
      new int[] { 1, 1, 0, 0, 60, 0, 0 },
      new int[] { 1, 1, 0, 0, Int32.MaxValue, 0, 0 },
      new int[] { 1, 1, 0, 0, 0, -1, 0 },
      new int[] { 1, 1, 0, 0, 0, Int32.MinValue, 0 },
      new int[] { 1, 1, 0, 0, 0, 1000 * 1000 * 1000, 0 },
      new int[] { 1, 1, 0, 0, 0, Int32.MaxValue, 0 },
      new int[] { 1, 1, 0, 0, 0, 0, -1440 },
      new int[] { 1, 1, 0, 0, 0, 0, Int32.MinValue },
      new int[] { 1, 1, 0, 0, 0, 0, 1440 },
      new int[] { 1, 1, 0, 0, 0, 0, Int32.MaxValue },
    };

    private static void TestBadDateFieldsOne(CBORDateConverter conv) {
      var eint = EInteger.FromInt32(2000);
      int[] lesserFields;
      for (int i = 0; i < ValueBadLesserFields.Length; ++i) {
        lesserFields = ValueBadLesserFields[i];
        Assert.AreEqual(7, lesserFields.Length, String.Empty + i);
        if (lesserFields[3] == 0 && lesserFields[4] == 0 &&
          lesserFields[5] == 0 && lesserFields[6] == 0 && lesserFields[2] ==
0) {
          try {
            _ = conv.DateTimeFieldsToCBORObject(
              2000,
              lesserFields[0],
              lesserFields[1]);
            Assert.Fail(
              "Should have failed: " + lesserFields[0] + " " + lesserFields[1]);
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        if (lesserFields[5] == 0 && lesserFields[6] == 0) {
          try {
            _ = conv.DateTimeFieldsToCBORObject(
              2000,
              lesserFields[0],
              lesserFields[1],
              lesserFields[2],
              lesserFields[3],
              lesserFields[4]);
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        try {
          _ = conv.DateTimeFieldsToCBORObject(eint, lesserFields);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      lesserFields = null;
      try {
        _ = conv.DateTimeFieldsToCBORObject(eint, lesserFields);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // TODO: Make into CBORException in next major version
      lesserFields = new int[] { 1 };
      try {
        _ = conv.DateTimeFieldsToCBORObject(eint, lesserFields);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      lesserFields = new int[] { 1, 1 };
      try {
        _ = conv.DateTimeFieldsToCBORObject(eint, lesserFields);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      lesserFields = new int[] { 1, 1, 0 };
      try {
        _ = conv.DateTimeFieldsToCBORObject(eint, lesserFields);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      lesserFields = new int[] { 1, 1, 0, 0 };
      try {
        _ = conv.DateTimeFieldsToCBORObject(eint, lesserFields);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      lesserFields = new int[] { 1, 1, 0, 0, 0 };
      try {
        _ = conv.DateTimeFieldsToCBORObject(eint, lesserFields);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      lesserFields = new int[] { 1, 1, 0, 0, 0, 0 };
      try {
        _ = conv.DateTimeFieldsToCBORObject(eint, lesserFields);
        Assert.Fail("Should have failed: 6");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = conv.DateTimeFieldsToCBORObject(2100, 2, 29);
        Assert.Fail("Should have failed: 2100/2/29");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = conv.DateTimeFieldsToCBORObject(2001, 2, 29);
        Assert.Fail("Should have failed: 2001/2/29");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = conv.DateTimeFieldsToCBORObject(2007, 2, 29);
        Assert.Fail("Should have failed: 2007/2/29");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = conv.DateTimeFieldsToCBORObject(2000, 2, 28);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = conv.DateTimeFieldsToCBORObject(2100, 2, 28);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = conv.DateTimeFieldsToCBORObject(2001, 2, 28);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = conv.DateTimeFieldsToCBORObject(2007, 2, 28);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = conv.DateTimeFieldsToCBORObject(2004, 2, 29);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = conv.DateTimeFieldsToCBORObject(2008, 2, 29);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = conv.DateTimeFieldsToCBORObject(2000, 2, 29);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestBadDateFields() {
      TestBadDateFieldsOne(CBORDateConverter.TaggedNumber);
      TestBadDateFieldsOne(CBORDateConverter.UntaggedNumber);
      TestBadDateFieldsOne(CBORDateConverter.TaggedString);
    }

    [Test]
    public void TestTags264And265() {
      CBORObject cbor;
      // Tag 264
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xd9, 0x01, 0x08,
        0x82,
        0xc2, 0x42, 2, 2, 0xc2, 0x42, 2, 2,
      });
      CBORTestCommon.AssertRoundTrip(cbor);
      // Tag 265
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xd9, 0x01, 0x09,
        0x82,
        0xc2, 0x42, 2, 2, 0xc2, 0x42, 2, 2,
      });
      CBORTestCommon.AssertRoundTrip(cbor);
    }

    [Test]
    public void TestTagThenBreak() {
      TestFailingDecode(new byte[] { 0xd1, 0xff });
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
      _ = TestTextStringStreamOne(TestCommon.Repeat('x', 200000));
      _ = TestTextStringStreamOne(TestCommon.Repeat('\u00e0', 200000));
      _ = TestTextStringStreamOne(TestCommon.Repeat('\u3000', 200000));
      _ = TestTextStringStreamOne(TestCommon.Repeat("\ud800\udc00", 200000));
      _ = TestTextStringStreamOne(
        "A" + TestCommon.Repeat('\u00e0', 200000));
      _ = TestTextStringStreamOne(
        "A" + TestCommon.Repeat('\u3000', 200000));
      _ = TestTextStringStreamOne(
        "AA" + TestCommon.Repeat('\u3000', 200000));
      _ = TestTextStringStreamOne(
        "A" + TestCommon.Repeat("\ud800\udc00", 200000));
      _ = TestTextStringStreamOne(
        "AA" + TestCommon.Repeat("\ud800\udc00", 200000));
      _ = TestTextStringStreamOne(
        "AAA" + TestCommon.Repeat("\ud800\udc00", 200000));
    }

    [Test]
    public void TestTextStringStreamNoIndefiniteWithinDefinite() {
      TestFailingDecode(new byte[] {
        0x7f, 0x61, 0x20, 0x7f, 0x61, 0x20,
        0xff, 0xff,
      });
    }

    [Test]
    public void TestIntegerFloatingEquivalence() {
      CBORObject cbor;
      // 0 versus 0.0
      cbor = CBORObject.NewMap();
      _ = cbor.Set(0, CBORObject.FromString("testzero"));
      _ = cbor.Set(CBORObject.FromDouble(0.0),
  CBORObject.FromString("testpointzero"));
      Assert.AreEqual(2, cbor.Count);
      {
        string stringTemp = cbor[CBORObject.FromInt32(0)].AsString();
        Assert.AreEqual(
          "testzero",
          stringTemp);
      }
      {
        string stringTemp = cbor[CBORObject.FromDouble(
              (double)0.0)].AsString();
        Assert.AreEqual(
          "testpointzero",
          stringTemp);
      }
      cbor = CBORObject.NewMap();
      _ = cbor.Set(CBORObject.FromDouble(0.0),
  CBORObject.FromString("testpointzero"));
      _ = cbor.Set(0, CBORObject.FromString("testzero"));
      Assert.AreEqual(2, cbor.Count);
      {
        string stringTemp = cbor[CBORObject.FromInt32(0)].AsString();
        Assert.AreEqual(
          "testzero",
          stringTemp);
      }
      {
        string stringTemp = cbor[CBORObject.FromDouble(
              (double)0.0)].AsString();
        Assert.AreEqual(
          "testpointzero",
          stringTemp);
      }
      // 3 versus 3.0
      cbor = CBORObject.NewMap();
      _ = cbor.Set(3, CBORObject.FromString("testzero"));
      _ = cbor.Set(CBORObject.FromDouble(3.0),
  CBORObject.FromString("testpointzero"));
      Assert.AreEqual(2, cbor.Count);
      {
        string stringTemp = cbor[CBORObject.FromInt32(3)].AsString();
        Assert.AreEqual(
          "testzero",
          stringTemp);
      }
      {
        string stringTemp = cbor[CBORObject.FromDouble(
              (double)3.0)].AsString();
        Assert.AreEqual(
          "testpointzero",
          stringTemp);
      }
      cbor = CBORObject.NewMap();
      _ = cbor.Set(CBORObject.FromDouble(3.0),
  CBORObject.FromString("testpointzero"));
      _ = cbor.Set(3, CBORObject.FromString("testzero"));
      Assert.AreEqual(2, cbor.Count);
      {
        string stringTemp = cbor[CBORObject.FromInt32(3)].AsString();
        Assert.AreEqual(
          "testzero",
          stringTemp);
      }
      {
        string stringTemp = cbor[CBORObject.FromDouble(
              (double)3.0)].AsString();
        Assert.AreEqual(
          "testpointzero",
          stringTemp);
      }
    }

    [Test]
    public void TestRoundTripESignalingNaN() {
      _ = ToObjectTest.TestToFromObjectRoundTrip(EDecimal.SignalingNaN);
      _ = ToObjectTest.TestToFromObjectRoundTrip(ERational.SignalingNaN);
      _ = ToObjectTest.TestToFromObjectRoundTrip(EFloat.SignalingNaN);
    }

    [Test]
    public void TestBigNumberThresholds() {
      var maxCborInteger = EInteger.FromString("18446744073709551615");
      var maxInt64 = EInteger.FromString("9223372036854775807");
      var minCborInteger = EInteger.FromString("-18446744073709551616");
      var minInt64 = EInteger.FromString("-9223372036854775808");
      var pastMaxCborInteger = EInteger.FromString(
          "18446744073709551616");
      var pastMaxInt64 = EInteger.FromString("9223372036854775808");
      var pastMinCborInteger = EInteger.FromString("-18446744073709551617");
      var pastMinInt64 = EInteger.FromString("-9223372036854775809");
      var eints = new EInteger[] {
        maxCborInteger, maxInt64, minCborInteger,
        minInt64, pastMaxCborInteger, pastMaxInt64, pastMinCborInteger,
        pastMinInt64,
      };
      var isPastCbor = new bool[] {
        false, false, false, false, true, false, true,
        false,
      };
      for (int i = 0; i < eints.Length; ++i) {
        CBORObject cbor;
        bool isNegative = eints[i].Sign < 0;
        cbor = CBORObject.FromEInteger(eints[i]);
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
        var ef = EFloat.Create(EInteger.One, eints[i]);
        cbor = CBORObject.FromEFloat(ef);
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
          using (var ms = new Test.DelayingStream()) {
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
          var ed = EDecimal.Create(EInteger.One, eints[i]);
          cbor = CBORObject.FromEDecimal(ed);
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
          using (var ms2 = new Test.DelayingStream()) {
            CBORObject.Write(ed, ms2);
            cbor = CBORObject.DecodeFromBytes(ms2.ToArray());
          }
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
        } catch (IOException ioe) {
          throw new InvalidOperationException(ioe.Message, ioe);
        }
      }
    }

    [Test]
    public void TestRationalJSONSpecificA() {
      var er =

  ERational.FromString("1088692579850251977918382727683876451288883451475551838663907953515213777772897669/734154292316019508508581520803142368704146796235662433292652");
      _ = CBORObject.FromERational(er).ToJSONString();
    }
    [Test]
    public void TestRationalJSONSpecificB() {
      var er2 =

  ERational.FromString("1117037884940373468269515037592447741921166676191625235424/13699696515096285881634845839085271311137");
      _ = CBORObject.FromERational(er2).ToJSONString();
    }
    [Test]
    public void TestRationalJSONSpecificC() {
      var er2 =

  ERational.FromString("42595158956667/1216724793801972483341765319799605241541780250657492435");
      _ = CBORObject.FromERational(er2).ToJSONString();
    }

    [Test]
    public void TestAllowEmpty() {
      CBOREncodeOptions options;
      var bytes = new byte[0];
      options = new CBOREncodeOptions(String.Empty);
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      options = new CBOREncodeOptions("allowempty=true");
      Assert.AreEqual(null, CBORObject.DecodeFromBytes(bytes, options));
      using (var ms = new Test.DelayingStream(bytes)) {
        options = new CBOREncodeOptions(String.Empty);
        try {
          _ = CBORObject.Read(ms, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
      using (var ms = new Test.DelayingStream(bytes)) {
        options = new CBOREncodeOptions("allowempty=true");
        Assert.AreEqual(null, CBORObject.Read(ms, options));
      }
    }

    [Test]
    public void TestCtap2CanonicalDecodeEncodeSpecific1() {
      var bytes = new byte[] {
        0xa2, 0x82, 0xf6,
        0x82,
        0xfb, 0x3c,
        0xf0, 0x03, 0x42,
        0xcb, 0x54, 0x6c,
        0x85,
        0x82,
        0xc5,
        0x82, 0x18,
        0xba, 0x0a,
        0xfa,
        0x84,
        0xa0, 0x57,
        0x97, 0x42, 0x00, 0x01, 0x65, 0x62, 0x7d, 0x45, 0x20, 0x6c, 0x41,
        0x00,
      };
      var cbor = CBORObject.DecodeFromBytes(bytes);
      var options = new CBOREncodeOptions("ctap2canonical=true");
      Console.WriteLine(String.Empty + cbor);
      try {
        _ = cbor.EncodeToBytes(options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(CBORTestCommon.CheckEncodeToBytes(cbor),
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
        0x82,
        0x82,
        0xf5,
        0x82,
        0x81,
        0xd8, 0x1e, 0x82, 0x29, 0x01, 0x80, 0x43, 0x01, 0x01,
        0x00,
      };
      var cbor = CBORObject.DecodeFromBytes(bytes);
      var options = new CBOREncodeOptions("ctap2canonical=true");
      Console.WriteLine(String.Empty + cbor);
      try {
        _ = cbor.EncodeToBytes(options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(CBORTestCommon.CheckEncodeToBytes(cbor),
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
        0x82,
        0xfa,
        0xe0,
        0xa0,
        0x9d,
        0xba,
        0x82,
        0x82,
        0xf7, 0xa2, 0xa0, 0xf7, 0x60, 0x41, 0x00,
        0xf4,
      };
      var cbor = CBORObject.DecodeFromBytes(bytes);
      var options = new CBOREncodeOptions("ctap2canonical=true");
      Console.WriteLine(String.Empty + cbor);
      try {
        _ = cbor.EncodeToBytes(options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(CBORTestCommon.CheckEncodeToBytes(cbor),
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
        0x81,
        0x82,
        0xda, 0x00, 0x0d, 0x77, 0x09,
        0xf4, 0x82, 0x82, 0xf4, 0xa0,
        0xf6,
      };
      var cbor = CBORObject.DecodeFromBytes(bytes);
      var options = new CBOREncodeOptions("ctap2canonical=true");
      Console.WriteLine(String.Empty + cbor);
      try {
        _ = cbor.EncodeToBytes(options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(CBORTestCommon.CheckEncodeToBytes(cbor),
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
        0xa2,
        0xda, 0x00, 0x03, 0x69,
        0x95, 0xf6, 0xf7, 0xf6, 0xf4,
      };
      var cbor = CBORObject.DecodeFromBytes(bytes);
      var options = new CBOREncodeOptions("ctap2canonical=true");
      Console.WriteLine(String.Empty + cbor);
      try {
        _ = cbor.EncodeToBytes(options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(bytes, options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.DecodeFromBytes(CBORTestCommon.CheckEncodeToBytes(cbor),
          options);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    public static void TestCtap2CanonicalDecodeEncodeOne(
      CBORObject cbor) {
      var options = new CBOREncodeOptions("ctap2canonical=true");
      if (cbor == null) {
        throw new ArgumentNullException(nameof(cbor));
      }
      byte[] e2bytes = CBORTestCommon.CheckEncodeToBytes(cbor);
      byte[] bytes = e2bytes;
      cbor = CBORObject.DecodeFromBytes(bytes);
      try {
        bytes = cbor.EncodeToBytes(options);
        CBORObject cbor2;
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
          _ = CBORObject.DecodeFromBytes(bytes, options);
          Assert.Fail("Should have failed");
        } catch (CBORException) {
          // NOTE: Intentionally empty
        } catch (Exception ex3) {
          Assert.Fail(ex3.ToString() + "\n" + ex4.ToString());
          throw new InvalidOperationException(String.Empty, ex3);
        }
      }
    }

    [Test]
    public void TestCtap2CanonicalDecodeEncode() {
      var r = new RandomGenerator();
      for (int i = 0; i < 3000; ++i) {
        TestCtap2CanonicalDecodeEncodeOne(
          CBORTestCommon.RandomCBORObject(r));
      }
    }

    [Test]
    public void TestTextStringStreamNoTagsBeforeDefinite() {
      try {
        _ = CBORTestCommon.FromBytesTestAB(new byte[] {
          0x7f, 0x61, 0x20,
          0xc0, 0x61, 0x20, 0xff,
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
      object o = obj.ToObject(typeof(EDecimal));
      return (EDecimal)o;
    }

    private static ERational AsER(CBORObject obj) {
      object o = obj.ToObject(typeof(ERational));
      return (ERational)o;
    }

    private static void AddSubCompare(CBORObject o1, CBORObject o2) {
      EDecimal cmpDecFrac, cmpCobj;
      cmpDecFrac = AsED(o1).Add(AsED(o2));
      cmpCobj = o1.AsNumber().Add(o2.AsNumber()).ToEDecimal();
      TestCommon.CompareTestEqual(cmpDecFrac, cmpCobj);
      cmpDecFrac = AsED(o1).Subtract(AsED(o2));
      cmpCobj = o1.AsNumber().Subtract(o2.AsNumber()).ToEDecimal();
      TestCommon.CompareTestEqual(cmpDecFrac, cmpCobj);
      CBORObjectTest.CompareDecimals(o1, o2);
    }

    [Test]
    public void TestRationalJsonString() {
      string s1 =

  "2314185985457202732189984229086860275536452482912712559300364012538811890519021609896348772904852567130731662638662357113651250315642348662481229868556065813139982071069333964882192144997551182445870403177326619887472161149361459394237531679153467064950578633985038857850930553390675215926785522674620921221013857844957579079905210161700278381169854796455676266121858525817919848944985101521416062436650605384179954486013171983603514573732843973878942460661051122207994787725632035785836247773451399551083190779512400561839577794870702499681043124072992405732619348558204728800270899359780143357389476977840367320292768181094717788094551212489822736249585469244387735363318078783976724668392554429679443922755068135350076319909649622682466354980725423633530350364989945871920048447230307815643527525431336201627641891131614532527580497256382071436840494627668584005384127077683035880018530366999707415485257695504047147523521952194384880231172509079788316925500613704258819197092976088140216280520582313645747413451716685429138670645309423396623806701594839731451445336814620082926910150739091172178600865482539725012429775997863264496120844788653020449046903816363344201802799558922359223708825558520103859838244276323990910167216851809090120320961066908102124848129364767874532700083684330840078660557364044159387179646160035386030868471110043830522222249658101959143096323641704675830142899751696476007503506009598273729872080504917363964684006707667515610753851782851579370526135223570019729110932882718719";
      string s2 =

  "6662791484278690594826817847881545965329948329731867968121995135273120814985447625408875010164308165523077008393040907448927095816668472183767306507621988644226927007049807896601977790621449471807224544610921018712323247068196141241260970690722422573836727986751170029846748991630865560108915742912790575418880931905841405752318207096850143527159053198029648842245667818442862752212319584591326350903220882410151458427571245209321776934621224736963318933098990162346637307854541301688032696173626360523085187457965260167140087021479260407414362314681927575639118779628079152745063483804212029391314516551540082552323766393935679162832149309343521979435765872081112730566874916857979923774605127048865566043423311513224206112727810624953812129189407444425723013814542858953773303224750083748214186967592731457750110532337407558719554095585903998079748001889804344632924251379769721367766565683489037136792018541299840911134792202457550460405605363852082703644386814261111315827747899661812006358141505684436007974039689212221755906535319187254965909243842599581550882694985174561192357511545227109515529785121078195397742875082523296406527673130136841581998940369597346610553537051630040762759128694436878055285011408511186930096142698312900789328008870013582608819840691525856150433351282368061590406881127142805435230013505013582096402814554965693562771980924387951907732638686068565579913844909487962223859043024131114445573057517284388114134555750443506173757889119715387627461644374462498045130424821914143893279013612002227413094709860042079542320696728791055885208451681839380238306841352325674806804434188273228678316889664118537421135644047836961335665043472528998461372064871916691003281042407296035913958087310042321020211485879442799018303005446353339317990963540";

      var er = ERational.Create(
          EInteger.FromString(s1),
          EInteger.FromString(s2));
      var cbor = CBORObject.FromERational(er);
      _ = cbor.ToJSONString();
    }

    public static bool CheckUtf16(string str) {
      if (str == null) {
        return false;
      }
      for (int i = 0; i < str.Length; ++i) {
        char c = str[i];
        if ((c & 0xfc00) == 0xd800 && i + 1 < str.Length &&
          (str[i] & 0xfc00) == 0xdc00) {
          ++i;
        } else if ((c & 0xf800) == 0xd800) {
          return false;
        }
      }
      return true;
    }

    [Test]
    public void TestWriteBasic() {
      var jsonop1 = new JSONOptions("writebasic=true");
      string json = CBORObject.FromString("\uD800\uDC00").ToJSONString(jsonop1);
      Assert.AreEqual("\"\\uD800\\uDC00\"", json);
      json = CBORObject.FromString("\u0800\u0C00").ToJSONString(jsonop1);
      Assert.AreEqual("\"\\u0800\\u0C00\"", json);
      json = CBORObject.FromString("\u0085\uFFFF").ToJSONString(jsonop1);
      Assert.AreEqual("\"\\u0085\\uFFFF\"", json);
      var rg = new RandomGenerator();
      for (int i = 0; i < 1000; ++i) {
        string rts = RandomObjects.RandomTextString(rg);
        var cbor = CBORObject.FromString(rts);
        json = cbor.ToJSONString(jsonop1);
        // Check that the JSON contains only ASCII code points
        for (int j = 0; j < json.Length; ++j) {
          char c = json[j];
          if (c is (< (char)0x20 and not (char)0x09 and not (char)0x0a and
not (char)0x0d) or >= (char)0x7f) {
            Assert.Fail(rts);
          }
        }
        // Round-trip check
        Assert.AreEqual(cbor, CBORObject.FromJSONString(json));
      }
    }

    [Test]
    public void TestJSONOptions() {
      var jsonop1 = new JSONOptions("numberconversion=intorfloat");
      {
        object objectTemp = jsonop1.ToString();
        object objectTemp2 = new JSONOptions(jsonop1.ToString()).ToString();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      var jsonop2 = new JSONOptions("numberconversion=decimal128");
      {
        object objectTemp = jsonop2.ToString();
        object objectTemp2 = new JSONOptions(jsonop2.ToString()).ToString();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      var jsonop3 = new JSONOptions("numberconversion=intorfloatfromdouble");
      {
        object objectTemp = jsonop3.ToString();
        object objectTemp2 = new JSONOptions(jsonop3.ToString()).ToString();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      var jsonop4 = new JSONOptions("numberconversion=double");
      {
        object objectTemp = jsonop4.ToString();
        object objectTemp2 = new JSONOptions(jsonop4.ToString()).ToString();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
    }

    [Test]
    public void TestPODOptions() {
      PODOptions podop = PODOptions.Default;
      {
        object objectTemp = podop.ToString();
        object objectTemp2 = new PODOptions(podop.ToString()).ToString();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
    }

    [Test]
    public void TestCBOREncodeOptions() {
      CBOREncodeOptions encodeop = CBOREncodeOptions.Default;
      {
        object objectTemp = encodeop.ToString();
        object objectTemp2 = new
        CBOREncodeOptions(encodeop.ToString()).ToString();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
    }

    [Test]
    public void TestRandomJSON() {
      var jsongen = new JSONGenerator();
      var rg = new RandomGenerator();
      var jsonop1 = new JSONOptions("numberconversion=intorfloat");
      var jsonop2 = new JSONOptions("numberconversion=decimal128");
      var jsonop3 = new JSONOptions("numberconversion=intorfloatfromdouble");
      var jsonop4 = new JSONOptions("numberconversion=double");
      for (int i = 0; i < 200; ++i) {
        byte[] jsonbytes = jsongen.Generate(rg);
        // Console.WriteLine(String.Empty + i + " len=" + jsonbytes.Length);
        JSONOptions currop = null;
        try {
          currop = jsonop1;
          _ = CBORObject.FromJSONBytes(jsonbytes, jsonop1);
          currop = jsonop2;
          _ = CBORObject.FromJSONBytes(jsonbytes, jsonop2);
          currop = jsonop3;
          _ = CBORObject.FromJSONBytes(jsonbytes, jsonop3);
          currop = jsonop4;
          _ = CBORObject.FromJSONBytes(jsonbytes, jsonop4);
        } catch (CBORException ex) {
          string msg = ex.Message + "\n" +
            DataUtilities.GetUtf8String(jsonbytes, true) + "\n" + currop;
          throw new InvalidOperationException(msg, ex);
        }
      }
    }

    public static bool TestTextStringStreamOne(string longString) {
      if (!CheckUtf16(longString)) {
        return false;
      }
      CBORObject cbor, cbor2;
      cbor = ToObjectTest.TestToFromObjectRoundTrip(longString);
      cbor2 = CBORTestCommon.FromBytesTestAB(
          CBORTestCommon.CheckEncodeToBytes(
            cbor));
      {
        object objectTemp = longString;
        object objectTemp2 = CBORObject.DecodeFromBytes(
            CBORTestCommon.CheckEncodeToBytes(cbor)).AsString();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        string cc = "useindeflengthstrings";
        cc += "=";
        cc += "false,allowduplicatekeys";
        cc += "=";
        cc += "true";
        string strTemp2 = CBORObject.DecodeFromBytes(cbor.EncodeToBytes(
              new CBOREncodeOptions(cc))).AsString();
        Assert.AreEqual(longString, strTemp2);
      }
      {
        string cc = "useindeflengthstrings";
        cc += "=";
        cc += "true,allowduplicatekeys";
        cc += "=";
        cc += "true";
        string strTemp2 = CBORObject.DecodeFromBytes(cbor.EncodeToBytes(
              new CBOREncodeOptions(cc))).AsString();
        Assert.AreEqual(longString, strTemp2);
      }
      TestCommon.AssertEqualsHashCode(cbor, cbor2);
      Assert.AreEqual(longString, cbor2.AsString());
      return true;
    }

    public static void TestWriteToJSON(CBORObject obj) {
      CBORObject objA = null;
      string jsonString = String.Empty;
      using (var ms = new Test.DelayingStream()) {
        try {
          if (obj == null) {
            throw new ArgumentNullException(nameof(obj));
          }
          obj.WriteJSONTo(ms);
          jsonString = DataUtilities.GetUtf8String(
              ms.ToArray(),
              false);
          objA = CBORObject.FromJSONString(jsonString);
        } catch (CBORException ex) {
          throw new InvalidOperationException(
            jsonString + "\n" + ex.ToString(),
            ex);
        } catch (IOException ex) {
          throw new InvalidOperationException(
            "IOException\n" + ex.ToString(),
            ex);
        }
      }
      if (obj == null) {
        throw new ArgumentNullException(nameof(obj));
      }
      var objB = CBORObject.FromJSONString(obj.ToJSONString());
      if (!objA.Equals(objB)) {
        Assert.Fail("WriteJSONTo gives different results from " +
          "ToJSONString\nobj=" +
          TestCommon.ToByteArrayString(obj.EncodeToBytes()) +
          "\nobjA=" + TestCommon.ToByteArrayString(objA.EncodeToBytes()) +
          "\nobjB=" + TestCommon.ToByteArrayString(objB.EncodeToBytes()) +
          "\nobj=" + obj.ToString() + "\nobjA=" + objA.ToString() +
          "\nobjB=" + objB.ToString() + "\njsonstring=" + jsonString +
          "\ntojsonstring=" + obj.ToJSONString());
      }
    }
  }
}
