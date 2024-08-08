/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.IO;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;
using PeterO.Numbers;

namespace Test {
  [TestFixture]
  public partial class CBORSupplementTest {
    [Test]
    public void IncorrectDecimalFrac() {
      byte[] bytes;
      CBORObject cbor;
      // string instead of array
      bytes = new byte[] { 0xc4, 0x61, 0x41 };
      cbor = CBORObject.DecodeFromBytes(bytes);
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      try {
        Console.WriteLine(String.Empty + cbor.ToObject(typeof(EDecimal)));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // number instead of array
      bytes = new byte[] { 0xc4, 0x00 };
      cbor = CBORObject.DecodeFromBytes(bytes);
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      try {
        Console.WriteLine(String.Empty + cbor.ToObject(typeof(EDecimal)));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc4, 0x81, 0, };
      cbor = CBORObject.DecodeFromBytes(bytes);
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      try {
        Console.WriteLine(String.Empty + cbor.ToObject(typeof(EDecimal)));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc4, 0x82, 0, 0x61, 0x41 };
      cbor = CBORObject.DecodeFromBytes(bytes);
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      try {
        Console.WriteLine(String.Empty + cbor.ToObject(typeof(EDecimal)));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc4, 0x82, 0x61, 0x41, 0 };
      cbor = CBORObject.DecodeFromBytes(bytes);
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      try {
        Console.WriteLine(String.Empty + cbor.ToObject(typeof(EDecimal)));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc4, 0x83, 0, 0, 0 };
      cbor = CBORObject.DecodeFromBytes(bytes);
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      try {
        Console.WriteLine(String.Empty + cbor.ToObject(typeof(EDecimal)));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void IncorrectBigFloat() {
      byte[] bytes;
      CBORObject cbor;
      // string instead of array
      bytes = new byte[] { 0xc5, 0x61, 0x41 };
      cbor = CBORObject.DecodeFromBytes(bytes);
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      try {
        Console.WriteLine(String.Empty + cbor.ToObject(typeof(EFloat)));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // number instead of array
      bytes = new byte[] { 0xc5, 0x00 };
      cbor = CBORObject.DecodeFromBytes(bytes);
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      try {
        Console.WriteLine(String.Empty + cbor.ToObject(typeof(EFloat)));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc5, 0x81, 0, };
      cbor = CBORObject.DecodeFromBytes(bytes);
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      try {
        Console.WriteLine(String.Empty + cbor.ToObject(typeof(EFloat)));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc5, 0x82, 0, 0x61, 0x41 };
      cbor = CBORObject.DecodeFromBytes(bytes);
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      try {
        Console.WriteLine(String.Empty + cbor.ToObject(typeof(EFloat)));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc5, 0x82, 0x61, 0x41, 0 };
      cbor = CBORObject.DecodeFromBytes(bytes);
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      try {
        Console.WriteLine(String.Empty + cbor.ToObject(typeof(EFloat)));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc5, 0x83, 0, 0, 0 };
      cbor = CBORObject.DecodeFromBytes(bytes);
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
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
    public void TestCBORObjectArgumentValidation() {
      Assert.AreEqual(
        CBORObject.Null,
        ToObjectTest.TestToFromObjectRoundTrip(null));
      Assert.AreEqual(
        CBORObject.Null,
        ToObjectTest.TestToFromObjectRoundTrip(null));
      Assert.AreEqual(
        CBORObject.True,
        ToObjectTest.TestToFromObjectRoundTrip(true));
      Assert.AreEqual(
        CBORObject.False,
        ToObjectTest.TestToFromObjectRoundTrip(false));
      Assert.AreEqual(
        ToObjectTest.TestToFromObjectRoundTrip(8),
        ToObjectTest.TestToFromObjectRoundTrip((byte)8));

      try {
        _ = CBORObject.True.ToObject(typeof(ERational));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.False.ToObject(typeof(ERational));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.NewArray().ToObject(typeof(ERational));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = CBORObject.NewMap().ToObject(typeof(ERational));
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestIncompleteCBORString() {
      byte[] bytes = { 0x65, 0x41, 0x41, 0x41, 0x41 };
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

    [Test]
    public void TestIncompleteIndefLengthArray() {
      byte[] bytes;
      bytes = new byte[] { 0x9f, 0, 0, 0, 0, 0 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0x9f, 0, 0, 0, 0, 0xff };
      try {
        _ = CBORObject.DecodeFromBytes(bytes);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestIncompleteIndefLengthMap() {
      // Premature end after value
      byte[] bytes = { 0xbf, 0x61, 0x41, 0, 0x61, 0x42, 0 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Premature end after key
      bytes = new byte[] { 0xbf, 0x61, 0x41, 0, 0x61, 0x42 };
      try {
        _ = CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xbf, 0x61, 0x41, 0, 0x61, 0x42, 0, 0xff };
      try {
        _ = CBORObject.DecodeFromBytes(bytes);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestCyclicRefs() {
      var cbor = CBORObject.NewArray();
      _ = cbor.Add(CBORObject.NewArray());
      _ = cbor.Add(cbor);
      _ = cbor[0].Add(cbor);
      try {
        using (var memoryStream = new Test.DelayingStream()) {
          cbor.WriteTo(memoryStream);
        }
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestNestingDepth() {
      try {
        {
          using var ms = new Test.DelayingStream();
          for (int i = 0; i < 2000; ++i) {
            // Write beginning of indefinite-length array
            ms.WriteByte(0x9f);
          }
          for (int i = 0; i < 2000; ++i) {
            // Write end of indefinite-length array
            ms.WriteByte(0xff);
          }
          // Assert throwing CBOR exception for reaching maximum
          // nesting depth
          try {
            _ = CBORObject.DecodeFromBytes(ms.ToArray());
            Assert.Fail("Should have failed");
          } catch (CBORException) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        {
          using var ms = new Test.DelayingStream();
          for (int i = 0; i < 495; ++i) {
            // Write beginning of indefinite-length array
            ms.WriteByte(0x9f);
          }
          for (int i = 0; i < 495; ++i) {
            // Write end of indefinite-length array
            ms.WriteByte(0xff);
          }
          // Maximum nesting depth not reached, so shouldn't throw
          try {
            CBORObject.DecodeFromBytes(ms.ToArray());
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
      } catch (Exception ex) {
        throw new InvalidOperationException(ex.Message, ex);
      }
    }

    [Test]
    public void TestCBOREInteger() {
      var bi = EInteger.FromString("9223372036854775808");
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(bi).ToObject(typeof(long));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(bi).ToObject(typeof(int));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bi = EInteger.FromString("-9223372036854775809");
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(bi).ToObject(typeof(long));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(bi).ToObject(typeof(int));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bi = EInteger.FromString("-9223372036854775808");
      try {
        _ = ToObjectTest.TestToFromObjectRoundTrip(bi).ToObject(typeof(int));
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestEquivalentInfinities() {
      CBORObject co, co2;
      co = ToObjectTest.TestToFromObjectRoundTrip(CBORTestCommon.DecPosInf);
      co2 = ToObjectTest.TestToFromObjectRoundTrip(double.PositiveInfinity);
      {
        bool boolTemp = co.AsNumber().IsNegative() &&
          co.AsNumber().IsInfinity();
        bool boolTemp2 = co2.AsNumber().IsNegative() &&
          co2.AsNumber().IsInfinity();
        Assert.AreEqual(boolTemp, boolTemp2);
      }
    }

    [Test]
    public void TestSharedRefs() {
      var encodeOptions = new CBOREncodeOptions("resolvereferences=true");
      byte[] bytes;
      CBORObject cbor;
      string expected;
      bytes = new byte[] {
        0x9f, 0xd8, 28, 1, 0xd8, 29, 0, 3, 3, 0xd8, 29,
        0, 0xff,
      };
      cbor = CBORObject.DecodeFromBytes(bytes, encodeOptions);
      expected = "[1,1,3,3,1]";
      Assert.AreEqual(expected, cbor.ToJSONString());
      bytes = new byte[] {
        0x9f, 0xd8, 28, 0x81, 1, 0xd8, 29, 0, 3, 3, 0xd8,
        29, 0, 0xff,
      };
      cbor = CBORObject.DecodeFromBytes(bytes, encodeOptions);
      expected = "[[1],[1],3,3,[1]]";
      Assert.AreEqual(expected, cbor.ToJSONString());
      // Checks if both objects are the same reference, not just equal
      Assert.IsTrue(cbor[0] == cbor[1], "cbor[0] not same as cbor[1]");
      Assert.IsTrue(cbor[0] == cbor[4], "cbor[0] not same as cbor[4]");
      bytes = new byte[] { 0xd8, 28, 0x82, 1, 0xd8, 29, 0 };
      cbor = CBORObject.DecodeFromBytes(bytes, encodeOptions);
      Assert.AreEqual(2, cbor.Count);
      // Checks if both objects are the same reference, not just equal
      Assert.IsTrue(cbor == cbor[1], "objects not the same");
    }

    [Test]
    public void TestBuiltInTags() {
      // As of 4.0, nearly all tags are no longer converted to native objects; thus,
      // DecodeFromBytes no longer fails when such tags are encountered but
      // have the wrong format (though it can fail for other reasons).
      // Tag 2, bignums
      byte[] bytes;
      var secondbytes = new byte[] { 0, 0x20, 0x60, 0x80, 0xa0, 0xe0 };
      var firstbytes = new byte[] { 0xc2, 0xc3, 0xc4, 0xc5 };
      CBORObject cbor;
      foreach (byte firstbyte in firstbytes) {
        foreach (byte secondbyte in secondbytes) {
          bytes = new byte[] { firstbyte, secondbyte };
          cbor = CBORObject.DecodeFromBytes(bytes);
          Assert.IsFalse(cbor.IsNumber, cbor.ToString());
        }
      }
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xd8, 0x1e, 0x9f, 0x01,
        0x01, 0xff,
      });
      Assert.IsTrue(cbor.IsNumber, cbor.ToString());

      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xd8, 0x1e, 0x9f, 0x01,
        0x01, 0x01, 0xff,
      });
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xd8, 0x1e, 0x9f, 0x01,
        0xff,
      });
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());

      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xd9, 0x01, 0x0e, 0x9f,
        0x01,
        0x01, 0xff,
      });
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());

      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xd9, 0x01, 0x0e, 0x9f,
        0x01,
        0x01, 0x01, 0xff,
      });
      Assert.IsTrue(cbor.IsNumber, cbor.ToString());
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xd9, 0x01, 0x0e, 0x9f,
        0x01,
        0xff,
      });
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());

      cbor = CBORObject.DecodeFromBytes(
          new byte[] { 0xd8, 0x1e, 0x9f, 0xff, });
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());

      cbor = CBORObject.DecodeFromBytes(
          new byte[] { 0xd9, 0x01, 0x0e, 0x9f, 0xff, });
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xc4, 0x9f, 0x00, 0x00,
        0xff,
      });
      Assert.IsTrue(cbor.IsNumber, cbor.ToString());
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xc5, 0x9f, 0x00, 0x00,
        0xff,
      });
      Assert.IsTrue(cbor.IsNumber, cbor.ToString());
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xc4, 0x9f, 0x00, 0x00,
        0x00,
        0xff,
      });
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xc5, 0x9f, 0x00,
        0x00, 0x00,
        0xff,
      });
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xc4, 0x9f, 0x00,
        0xff,
      });
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xc5, 0x9f, 0x00,
        0xff,
      });
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x9f, 0xff, });
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x9f, 0xff, });
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x81, 0x00, });
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x81, 0x00, });
      Assert.IsFalse(cbor.IsNumber, cbor.ToString());
      {
        object objectTemp = EInteger.Zero;
        object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          0xc2,
          0x40,
        }).ToObject(typeof(EInteger));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = EInteger.FromString("-1");
        object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          0xc3,
          0x41, 0x00,
        }).ToObject(typeof(EInteger));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = EInteger.FromString("-1");
        object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          0xc3,
          0x40,
        }).ToObject(typeof(EInteger));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
    }

    [Test]
    public void TestUUID() {
      CBORObject obj =
        ToObjectTest.TestToFromObjectRoundTrip(new Guid(
            "00112233-4455-6677-8899-AABBCCDDEEFF"));
      Assert.AreEqual(CBORType.ByteString, obj.Type);
      Assert.AreEqual(EInteger.FromString("37"), obj.MostInnerTag);
      byte[] bytes = obj.GetByteString();
      Assert.AreEqual(16, bytes.Length);
      Assert.AreEqual(0x00, bytes[0]);
      Assert.AreEqual(0x11, bytes[1]);
      Assert.AreEqual(0x22, bytes[2]);
      Assert.AreEqual(0x33, bytes[3]);
      Assert.AreEqual(0x44, bytes[4]);
      Assert.AreEqual(0x55, bytes[5]);
      Assert.AreEqual(0x66, bytes[6]);
      Assert.AreEqual(0x77, bytes[7]);
      Assert.AreEqual((byte)0x88, bytes[8]);
      Assert.AreEqual((byte)0x99, bytes[9]);
      Assert.AreEqual((byte)0xaa, bytes[10]);
      Assert.AreEqual((byte)0xbb, bytes[11]);
      Assert.AreEqual((byte)0xcc, bytes[12]);
      Assert.AreEqual((byte)0xdd, bytes[13]);
      Assert.AreEqual((byte)0xee, bytes[14]);
      Assert.AreEqual((byte)0xff, bytes[15]);
      bytes = new byte[] {
        0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,
        0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff,
      };
      obj = CBORObject.FromCBORObjectAndTag(
          CBORObject.FromByteArray(bytes),
          37);
      {
        string stringTemp =
DataUtilities.ToLowerCaseAscii(obj.AsGuid().ToString());
        Assert.AreEqual(
          "00112233-4455-6677-8899-aabbccddeeff",
          stringTemp);
}
    }

    // [Test]
    public static void TestMiniCBOR() {
      byte[] bytes;
      bytes = new byte[] { 0x19, 2 };
      try {
        using (var memoryStream = new Test.DelayingStream(bytes)) {
          _ = MiniCBOR.ReadInt32(memoryStream);
        }
        Assert.Fail("Should have failed");
      } catch (IOException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0x1a, 2 };
      try {
        using (var memoryStream = new Test.DelayingStream(bytes)) {
          _ = MiniCBOR.ReadInt32(memoryStream);
        }
        Assert.Fail("Should have failed");
      } catch (IOException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0x1b, 2 };
      try {
        using (var ms = new Test.DelayingStream(bytes)) {
          _ = MiniCBOR.ReadInt32(ms);
        }
        Assert.Fail("Should have failed");
      } catch (IOException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0x1b, 2, 2, 2, 2, 2, 2, 2, 2 };
      try {
        using (var memoryStream = new Test.DelayingStream(bytes)) {
          _ = MiniCBOR.ReadInt32(memoryStream);
        }
        Assert.Fail("Should have failed");
      } catch (IOException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0x1c, 2 };
      try {
        using (var memoryStream = new Test.DelayingStream(bytes)) {
          _ = MiniCBOR.ReadInt32(memoryStream);
        }
        Assert.Fail("Should have failed");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        bytes = new byte[] { 0 };
        using (var ms = new Test.DelayingStream(bytes)) {
          Assert.AreEqual(0, MiniCBOR.ReadInt32(ms));
        }
        bytes = new byte[] { 0x17 };
        using (var ms2 = new Test.DelayingStream(bytes)) {
          Assert.AreEqual(0x17, MiniCBOR.ReadInt32(ms2));
        }
        bytes = new byte[] { 0x18, 2 };
        using (var ms3 = new Test.DelayingStream(bytes)) {
          Assert.AreEqual(2, MiniCBOR.ReadInt32(ms3));
        }
        bytes = new byte[] { 0x19, 0, 2 };
        using (var ms4 = new Test.DelayingStream(bytes)) {
          Assert.AreEqual(2, MiniCBOR.ReadInt32(ms4));
        }
        bytes = new byte[] { 0x27 };
        using (var ms5 = new Test.DelayingStream(bytes)) {
          Assert.AreEqual(-1 - 7, MiniCBOR.ReadInt32(ms5));
        }
        bytes = new byte[] { 0x37 };
        using (var ms6 = new Test.DelayingStream(bytes)) {
          Assert.AreEqual(-1 - 0x17, MiniCBOR.ReadInt32(ms6));
        }
      } catch (IOException ioex) {
        Assert.Fail(ioex.Message);
      }
    }

    [Test]
    public void TestNegativeBigInts() {
      {
        object objectTemp = EInteger.FromString("-257");
        object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          0xc3,
          0x42, 1,
          0,
        }).ToObject(typeof(EInteger));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = EInteger.FromString("-65537");
        object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          0xc3,
          0x43, 1,
          0, 0,
        }).ToObject(typeof(EInteger));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = EInteger.FromString("-16777217");
        object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          0xc3, 0x44,
          1,
          0, 0, 0,
        }).ToObject(typeof(EInteger));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = EInteger.FromString("-4294967297");
        object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          0xc3, 0x45,
          1,
          0, 0, 0, 0,
        }).ToObject(typeof(EInteger));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = EInteger.FromString("-1099511627777");
        object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          0xc3, 0x46,
          1,
          0, 0, 0, 0, 0,
        }).ToObject(typeof(EInteger));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = EInteger.FromString("-281474976710657");
        object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          0xc3, 0x47,
          1,
          0, 0, 0, 0,
          0, 0,
        }).ToObject(typeof(EInteger));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = EInteger.FromString("-72057594037927937");
        object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          0xc3, 0x48,
          1,
          0, 0, 0, 0,
          0, 0, 0,
        }).ToObject(typeof(EInteger));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = EInteger.FromString("-18446744073709551617");
        object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          0xc3, 0x49,
          1,
          0, 0, 0, 0, 0, 0, 0, 0,
        }).ToObject(typeof(EInteger));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = EInteger.FromString("-4722366482869645213697");
        object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] {
          0xc3, 0x4a,
          1,
          0, 0, 0, 0, 0, 0, 0, 0, 0,
        }).ToObject(typeof(EInteger));
        Assert.AreEqual(objectTemp, objectTemp2);
      }
    }

    [Test]
    public void TestStringRefs() {
      var encodeOptions = new CBOREncodeOptions("resolvereferences=true");
      var cbor = CBORObject.DecodeFromBytes(
      new byte[] {
        0xd9, 1, 0, 0x9f, 0x64, 0x61, 0x62, 0x63, 0x64, 0xd8,
        0x19, 0x00, 0xd8, 0x19, 0x00, 0x64, 0x62, 0x62, 0x63, 0x64, 0xd8, 0x19,
        0x01, 0xd8, 0x19, 0x00, 0xd8, 0x19, 0x01, 0xff,
      },
      encodeOptions);
      string expected =
        "[\"abcd\",\"abcd\",\"abcd\",\"bbcd\",\"bbcd\",\"abcd\",\"bbcd\"]";
      Assert.AreEqual(expected, cbor.ToJSONString());
      cbor = CBORObject.DecodeFromBytes(new byte[] {
        0xd9,
        1, 0, 0x9f, 0x64, 0x61, 0x62, 0x63, 0x64, 0x62, 0x61,
        0x61, 0xd8, 0x19, 0x00, 0xd8, 0x19, 0x00, 0x64, 0x62,
        0x62, 0x63, 0x64, 0xd8, 0x19, 0x01, 0xd8, 0x19, 0x00,
        0xd8, 0x19, 0x01, 0xff,
      },
      encodeOptions);
      expected =
  "[\"abcd\",\"aa\",\"abcd\",\"abcd\",\"bbcd\",\"bbcd\",\"abcd\",\"bbcd\"]";
      Assert.AreEqual(expected, cbor.ToJSONString());
    }

    [Test]
    public void TestPodCompareTo() {
      var cpod = new CPOD3();
      CBORObject cbor, cbor2;
      cpod.Aa = "Gg";
      cpod.Bb = "Jj";
      cpod.Cc = "Hh";
      cbor = CBORObject.FromObject(cpod);
      cbor2 = CBORObject.NewMap().Add("aa", "Gg").Add("bb", "Jj").Add("cc",
  "Hh");
      TestCommon.CompareTestEqual(cbor, cbor2);
      cbor2 = CBORObject.FromInt32(100);
      TestCommon.CompareTestGreater(cbor, cbor2);
      cbor2 = CBORObject.FromSimpleValue(10);
      TestCommon.CompareTestLess(cbor, cbor2);
    }

    [Test]
    public void TestCPOD() {
      var m = new CPOD
      {
        Aa = "Test",
      };
      var cbor = CBORObject.FromObject(m);
      Assert.IsFalse(cbor.ContainsKey("bb"), cbor.ToString());
      Assert.AreEqual("Test", cbor["aa"].AsString(), cbor.ToString());
    }
  }
}
