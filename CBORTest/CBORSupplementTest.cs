/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;
using PeterO.Numbers;

namespace Test {
  [TestFixture]
  public class CBORSupplementTest {
    [Test]
    public void IncorrectDecimalFrac() {
      byte[] bytes;
      // string instead of array
      bytes = new byte[] { 0xc4, 0x61, 0x41 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // number instead of array
      bytes = new byte[] { 0xc4, 0x00 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc4, 0x81, 0, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc4, 0x82, 0, 0x61, 0x41 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc4, 0x82, 0x61, 0x41, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc4, 0x83, 0, 0, 0 };
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

    [Test]
    public void IncorrectBigFloat() {
      byte[] bytes;
      // string instead of array
      bytes = new byte[] { 0xc5, 0x61, 0x41 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // number instead of array
      bytes = new byte[] { 0xc5, 0x00 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc5, 0x81, 0, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc5, 0x82, 0, 0x61, 0x41 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc5, 0x82, 0x61, 0x41, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc5, 0x83, 0, 0, 0 };
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

    private sealed class FakeConverter : ICBORConverter<Uri> {
      public CBORObject ToCBORObject(Uri obj) {
        throw new InvalidOperationException();
      }
    }

    [Test]
    public void TestCBORObjectArgumentValidation() {
      try {
        ToObjectTest.TestToFromObjectRoundTrip('\udddd');
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(
  CBORObject.Null,
  ToObjectTest.TestToFromObjectRoundTrip((byte[])null));
      Assert.AreEqual(
        CBORObject.Null,
        ToObjectTest.TestToFromObjectRoundTrip((CBORObject[])null));
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
        CBORObject.True.Abs();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.Abs();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().Abs();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().Abs();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsERational();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsERational();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().AsERational();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsERational();
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
        CBORObject.DecodeFromBytes(bytes);
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
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0x9f, 0, 0, 0, 0, 0xff };
      try {
        CBORObject.DecodeFromBytes(bytes);
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
        CBORObject.DecodeFromBytes(bytes);
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
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xbf, 0x61, 0x41, 0, 0x61, 0x42, 0, 0xff };
      try {
        CBORObject.DecodeFromBytes(bytes);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestCyclicRefs() {
      CBORObject cbor = CBORObject.NewArray();
      cbor.Add(CBORObject.NewArray());
      cbor.Add(cbor);
      cbor[0].Add(cbor);
      try {
        using (var memoryStream = new MemoryStream()) {
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
        using (var ms = new MemoryStream()) {
          for (var i = 0; i < 2000; ++i) {
            // Write beginning of indefinite-length array
            ms.WriteByte((byte)0x9f);
          }
          for (var i = 0; i < 2000; ++i) {
            // Write end of indefinite-length array
            ms.WriteByte((byte)0xff);
          }
          // Assert throwing CBOR exception for reaching maximum
          // nesting depth
          try {
            CBORObject.DecodeFromBytes(ms.ToArray());
            Assert.Fail("Should have failed");
          } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        }
        {
          using (var ms = new MemoryStream()) {
          for (var i = 0; i < 495; ++i) {
            // Write beginning of indefinite-length array
            ms.WriteByte((byte)0x9f);
          }
          for (var i = 0; i < 495; ++i) {
            // Write end of indefinite-length array
            ms.WriteByte((byte)0xff);
          }
          // Maximum nesting depth not reached, so shouldn't throw
          try {
            CBORObject.DecodeFromBytes(ms.ToArray());
          } catch (Exception ex) {
            Assert.Fail(ex.ToString());
            throw new InvalidOperationException(String.Empty, ex);
          }
        }
        }
      } catch (Exception ex) {
        throw new InvalidOperationException(ex.Message, ex);
      }
    }

    [Test]
    public void TestCBOREInteger() {
      EInteger bi = EInteger.FromString("9223372036854775808");
      try {
        ToObjectTest.TestToFromObjectRoundTrip(bi).AsInt64();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(bi).AsInt32();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bi = EInteger.FromString("-9223372036854775809");
      try {
        ToObjectTest.TestToFromObjectRoundTrip(bi).AsInt64();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ToObjectTest.TestToFromObjectRoundTrip(bi).AsInt32();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bi = EInteger.FromString("-9223372036854775808");
      try {
        ToObjectTest.TestToFromObjectRoundTrip(bi).AsInt32();
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
      co2 = ToObjectTest.TestToFromObjectRoundTrip(Double.PositiveInfinity);
      TestCommon.CompareTestEqual(co, co2);
      co = CBORObject.NewMap().Add(
        CBORTestCommon.DecPosInf,
        CBORObject.Undefined);
      co2 = CBORObject.NewMap().Add(
        Double.PositiveInfinity,
        CBORObject.Undefined);
      TestCommon.CompareTestEqual(co, co2);
    }

    [Test]
    public void TestSharedRefs() {
      byte[] bytes;
      CBORObject cbor;
      string expected;
bytes = new byte[] { 0x9f, 0xd8, 28, 1, 0xd8, 29, 0, 3, 3, 0xd8, 29, 0, 0xff };
      cbor = CBORObject.DecodeFromBytes(bytes);
      expected = "[1,1,3,3,1]";
      Assert.AreEqual(expected, cbor.ToJSONString());
      bytes = new byte[] { 0x9f, 0xd8, 28, 0x81, 1, 0xd8, 29, 0, 3, 3, 0xd8,
        29, 0, 0xff };
      cbor = CBORObject.DecodeFromBytes(bytes);
      expected = "[[1],[1],3,3,[1]]";
      Assert.AreEqual(expected, cbor.ToJSONString());
      // Checks if both objects are the same reference, not just equal
      Assert.IsTrue(cbor[0] == cbor[1], "cbor[0] not same as cbor[1]");
      Assert.IsTrue(cbor[0] == cbor[4], "cbor[0] not same as cbor[4]");
      bytes = new byte[] { 0xd8, 28, 0x82, 1, 0xd8, 29, 0 };
      cbor = CBORObject.DecodeFromBytes(bytes);
      Assert.AreEqual(2, cbor.Count);
      // Checks if both objects are the same reference, not just equal
      Assert.IsTrue(cbor == cbor[1], "objects not the same");
    }

    [Test]
    public void TestBuiltInTags() {
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x20 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x60 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x80 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0xa0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0xe0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x20 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x60 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x80 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0xa0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0xe0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x20 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x40 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x60 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x80 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
 CBORObject.DecodeFromBytes(new byte[] { 0xd8, 0x1e, 0x9f, 0x01, 0x01, 0xff });
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xd8, 0x1e, 0x9f, 0x01, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xd8, 0x1e, 0x9f, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x9f, 0x00, 0x00, 0xff });
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x9f, 0x00, 0x00, 0xff });
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x9f, 0x00, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x9f, 0x00, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x9f, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x9f, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x81, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0xa0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0xe0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x20 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x40 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x60 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x80 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x81, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0xa0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0xe0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      Assert.AreEqual(
        EInteger.Zero,
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x40 }).AsEInteger());
      Assert.AreEqual(
        EInteger.FromString("-1"),
   CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x41, 0x00 }).AsEInteger());
      Assert.AreEqual(
        EInteger.FromString("-1"),
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x40 }).AsEInteger());
    }

    [Test]
    public void TestUUID() {
      CBORObject obj =
        ToObjectTest.TestToFromObjectRoundTrip(Guid.Parse(
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
    }

    // [Test]
    public static void TestMiniCBOR() {
      byte[] bytes;
      bytes = new byte[] { 0x19, 2 };
      try {
        using (var memoryStream = new MemoryStream(bytes)) {
          MiniCBOR.ReadInt32(memoryStream);
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
        using (var memoryStream = new MemoryStream(bytes)) {
          MiniCBOR.ReadInt32(memoryStream);
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
        using (var ms = new MemoryStream(bytes)) {
          MiniCBOR.ReadInt32(ms);
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
        using (var memoryStream = new MemoryStream(bytes)) {
          MiniCBOR.ReadInt32(memoryStream);
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
        using (var memoryStream = new MemoryStream(bytes)) {
          MiniCBOR.ReadInt32(memoryStream);
        }
        Assert.Fail("Should have failed");
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        bytes = new byte[] { 0 };
        using (var ms = new MemoryStream(bytes)) {
          Assert.AreEqual(0, MiniCBOR.ReadInt32(ms));
        }
        bytes = new byte[] { 0x17 };
        using (var ms2 = new MemoryStream(bytes)) {
          Assert.AreEqual(0x17, MiniCBOR.ReadInt32(ms2));
        }
        bytes = new byte[] { 0x18, 2 };
        using (var ms3 = new MemoryStream(bytes)) {
          Assert.AreEqual(2, MiniCBOR.ReadInt32(ms3));
        }
        bytes = new byte[] { 0x19, 0, 2 };
        using (var ms4 = new MemoryStream(bytes)) {
          Assert.AreEqual(2, MiniCBOR.ReadInt32(ms4));
        }
        bytes = new byte[] { 0x27 };
        using (var ms5 = new MemoryStream(bytes)) {
          Assert.AreEqual(-1 - 7, MiniCBOR.ReadInt32(ms5));
        }
        bytes = new byte[] { 0x37 };
        using (var ms6 = new MemoryStream(bytes)) {
          Assert.AreEqual(-1 - 0x17, MiniCBOR.ReadInt32(ms6));
        }
      } catch (IOException ioex) {
        Assert.Fail(ioex.Message);
      }
    }

    [Test]
    public void TestNegativeBigInts() {
      Assert.AreEqual(
  EInteger.FromString("-257"),
   CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x42, 1, 0 }).AsEInteger());
      Assert.AreEqual(
  EInteger.FromString("-65537"),
  CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x43, 1, 0, 0 }).AsEInteger());
      {
object objectTemp = EInteger.FromString("-16777217");
object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x44, 1,
  0, 0, 0 }).AsEInteger();
Assert.AreEqual(objectTemp, objectTemp2);
}
      {
object objectTemp = EInteger.FromString("-4294967297");
object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x45, 1,
  0, 0, 0, 0 }).AsEInteger();
Assert.AreEqual(objectTemp, objectTemp2);
}
      {
object objectTemp = EInteger.FromString("-1099511627777");
object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x46, 1,
  0, 0, 0, 0, 0 }).AsEInteger();
Assert.AreEqual(objectTemp, objectTemp2);
}
      {
object objectTemp = EInteger.FromString("-281474976710657");
    object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x47,
          1,
  0, 0, 0, 0,
                    0, 0 }).AsEInteger();
Assert.AreEqual(objectTemp, objectTemp2);
}
      {
object objectTemp = EInteger.FromString("-72057594037927937");
    object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x48,
          1,
  0, 0, 0, 0,
                    0, 0, 0 }).AsEInteger();
Assert.AreEqual(objectTemp, objectTemp2);
}
      {
object objectTemp = EInteger.FromString("-18446744073709551617");
object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x49, 1,
  0, 0, 0, 0, 0, 0, 0, 0 }).AsEInteger();
Assert.AreEqual(objectTemp, objectTemp2);
}
      {
object objectTemp = EInteger.FromString("-4722366482869645213697");
object objectTemp2 = CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x4a, 1,
  0, 0, 0, 0, 0, 0, 0, 0, 0 }).AsEInteger();
Assert.AreEqual(objectTemp, objectTemp2);
}
    }

    [Test]
    public void TestStringRefs() {
      CBORObject cbor = CBORObject.DecodeFromBytes(
        new byte[] { 0xd9, 1, 0, 0x9f, 0x64, 0x61, 0x62, 0x63, 0x64, 0xd8,
          0x19, 0x00, 0xd8, 0x19, 0x00, 0x64, 0x62, 0x62, 0x63, 0x64, 0xd8,
          0x19, 0x01, 0xd8, 0x19, 0x00, 0xd8, 0x19, 0x01, 0xff });
      string expected =
        "[\"abcd\",\"abcd\",\"abcd\",\"bbcd\",\"bbcd\",\"abcd\",\"bbcd\"]";
      Assert.AreEqual(expected, cbor.ToJSONString());
      cbor = CBORObject.DecodeFromBytes(new byte[] { 0xd9,
                    1, 0, 0x9f, 0x64, 0x61, 0x62, 0x63, 0x64, 0x62, 0x61,
                      0x61, 0xd8, 0x19, 0x00, 0xd8, 0x19, 0x00, 0x64, 0x62,
                      0x62, 0x63, 0x64, 0xd8, 0x19, 0x01, 0xd8, 0x19, 0x00,
                      0xd8, 0x19, 0x01, 0xff });
      expected =
     "[\"abcd\",\"aa\",\"abcd\",\"abcd\",\"bbcd\",\"bbcd\",\"abcd\",\"bbcd\"]";
      Assert.AreEqual(expected, cbor.ToJSONString());
    }

    public sealed class CPOD {
      public string Aa { get; set; }

      private string Bb { get; set; }
    }
    [Test]
    public void TestCPOD() {
      var m = new CPOD();
      m.Aa = "Test";
      CBORObject cbor = CBORObject.FromObject(m);
      Assert.IsFalse(cbor.ContainsKey("bb"), cbor.ToString());
      Assert.AreEqual("Test", cbor["aa"].AsString(), cbor.ToString());
    }
  }
}
