/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.IO;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;

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
Console.Write(String.Empty);
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
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc4, 0x81, 0, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc4, 0x82, 0, 0x61, 0x41 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc4, 0x82, 0x61, 0x41, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc4, 0x83, 0, 0, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
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
Console.Write(String.Empty);
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
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc5, 0x81, 0, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc5, 0x82, 0, 0x61, 0x41 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc5, 0x82, 0x61, 0x41, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bytes = new byte[] { 0xc5, 0x83, 0, 0, 0 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
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
        CBORObject.FromObject('\udddd');
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(CBORObject.Null, CBORObject.FromObject((byte[])null));
      Assert.AreEqual(
        CBORObject.Null,
        CBORObject.FromObject((CBORObject[])null));
      Assert.AreEqual(CBORObject.True, CBORObject.FromObject(true));
      Assert.AreEqual(CBORObject.False, CBORObject.FromObject(false));
      Assert.AreEqual(CBORObject.FromObject(8), CBORObject.FromObject((byte)8));
      try {
        CBORObject.AddConverter(null, new FakeConverter());
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.AddConverter(typeof(String), new FakeConverter());
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.AddTagHandler(null, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.Abs();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.Abs();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().Abs();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().Abs();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.True.AsExtendedRational();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.False.AsExtendedRational();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewArray().AsExtendedRational();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.NewMap().AsExtendedRational();
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestCBORObjectCanTruncatedIntFitInInt32() {
      Assert.IsFalse(CBORObject.True.CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.False.CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.NewArray().CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.NewMap().CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(0).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(2.5).CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(Int32.MinValue)
                    .CanTruncatedIntFitInInt32());
      Assert.IsTrue(CBORObject.FromObject(Int32.MaxValue)
                    .CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.FromObject(Double.PositiveInfinity)
                    .CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.FromObject(Double.NegativeInfinity)
                    .CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.FromObject(Double.NaN)
                    .CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.FromObject(ExtendedDecimal.PositiveInfinity)
                    .CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.FromObject(ExtendedDecimal.NegativeInfinity)
                    .CanTruncatedIntFitInInt32());
      Assert.IsFalse(CBORObject.FromObject(ExtendedDecimal.NaN)
                    .CanTruncatedIntFitInInt32());
    }

    [Test]
    public void TestIncompleteCBORString() {
      byte[] bytes = { 0x65, 0x41, 0x41, 0x41, 0x41 };
      try {
        CBORObject.DecodeFromBytes(bytes);
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
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
Console.Write(String.Empty);
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
Console.Write(String.Empty);
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
Console.Write(String.Empty);
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
    public void TestExtendedDecimalArgValidation() {
      try {
        ExtendedDecimal.FromString(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(ExtendedDecimal.Zero, ExtendedDecimal.FromString("0"));
      Assert.AreEqual(
        ExtendedDecimal.Zero,
        ExtendedDecimal.FromString("0", null));
      try {
        ExtendedDecimal.FromString(null, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString(String.Empty);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString(null, 0, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString("x", -1, 1);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString("x", 2, 1);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString("x", 0, -1);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString("x", 0, 2);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString("x", 1, 1);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString(null, 0, 1, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString("x", -1, 1, null);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString("x", 2, 1, null);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString("x", 0, -1, null);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString("x", 0, 2, null);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString("x", 1, 1, null);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        ExtendedFloat.Create(null, BigInteger.One);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.Create(null, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.Create(BigInteger.One, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(ExtendedFloat.Zero, ExtendedFloat.FromString("0"));
      Assert.AreEqual(ExtendedFloat.Zero, ExtendedFloat.FromString("0", null));
      try {
        ExtendedFloat.FromString(null, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString(String.Empty);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString(null, 0, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString("x", -1, 1);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString("x", 2, 1);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString("x", 0, -1);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString("x", 0, 2);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString("x", 1, 1);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString(null, 0, 1, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString("x", -1, 1, null);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString("x", 2, 1, null);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString("x", 0, -1, null);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString("x", 0, 2, null);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString("x", 1, 1, null);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString(
          "Infinity",
          PrecisionContext.Unlimited.WithSimplified(true));
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString(
          "-Infinity",
          PrecisionContext.Unlimited.WithSimplified(true));
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString(
          "NaN",
          PrecisionContext.Unlimited.WithSimplified(true));
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString(
          "sNaN",
          PrecisionContext.Unlimited.WithSimplified(true));
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString(
          "Infinity",
          PrecisionContext.Unlimited.WithSimplified(true));
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString(
          "-Infinity",
          PrecisionContext.Unlimited.WithSimplified(true));
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString(
          "NaN",
          PrecisionContext.Unlimited.WithSimplified(true));
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString(
          "sNaN",
          PrecisionContext.Unlimited.WithSimplified(true));
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        ExtendedDecimal.FromString("0..1");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString("0.1x+222");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.FromString("0.1g-222");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString("0..1");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString("0.1x+222");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.FromString("0.1g-222");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestExtendedToInteger() {
      ExtendedDecimal dec = ExtendedDecimal.Create(999, -1);
      ExtendedFloat flo = ExtendedFloat.Create(999, -1);
      ExtendedRational rat = ExtendedRational.Create(8, 5);
      try {
        dec.ToBigIntegerExact();
        Assert.Fail("Should have failed");
      } catch (ArithmeticException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        flo.ToBigIntegerExact();
        Assert.Fail("Should have failed");
      } catch (ArithmeticException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        rat.ToBigIntegerExact();
        Assert.Fail("Should have failed");
      } catch (ArithmeticException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        dec.ToBigInteger();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        flo.ToBigInteger();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        rat.ToBigInteger();
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
Console.Write(String.Empty);
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
Console.Write(String.Empty);
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
    public void TestCBORBigInteger() {
      var bi = (BigInteger)Int64.MaxValue;
      bi += BigInteger.One;
      try {
        CBORObject.FromObject(bi).AsInt64();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(bi).AsInt32();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bi = (BigInteger)Int64.MinValue;
      bi -= BigInteger.One;
      try {
        CBORObject.FromObject(bi).AsInt64();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.FromObject(bi).AsInt32();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      bi = (BigInteger)Int64.MinValue;
      try {
        CBORObject.FromObject(bi).AsInt32();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestEquivalentInfinities() {
      CBORObject co, co2;
      co = CBORObject.FromObject(ExtendedDecimal.PositiveInfinity);
      co2 = CBORObject.FromObject(Double.PositiveInfinity);
      TestCommon.CompareTestEqual(co, co2);
      co = CBORObject.NewMap().Add(
        ExtendedDecimal.PositiveInfinity,
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
    public void TestRationalCompareDecimal() {
      var fr = new FastRandom();
       for (var i = 0; i < 100; ++i) {
        ExtendedRational er = RandomObjects.RandomRational(fr);
        int exp = -100000 + fr.NextValue(200000);
        ExtendedDecimal ed = ExtendedDecimal.Create(
          RandomObjects.RandomBigInteger(fr),
          (BigInteger)exp);
        ExtendedRational er2 = ExtendedRational.FromExtendedDecimal(ed);
        // sw1.Start();
        int c2r = er.CompareTo(er2);
        // sw1.Stop();sw2.Start();
        int c2d = er.CompareToDecimal(ed);
        // sw2.Stop();
        Assert.AreEqual(c2r, c2d);
      }
    }

    [Test]
    public void TestBuiltInTags() {
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x20 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x60 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x80 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0xa0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0xe0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x20 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x60 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x80 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0xa0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0xe0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x20 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x40 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x60 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x80 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
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
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xd8, 0x1e, 0x9f, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
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
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x9f, 0x00, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x9f, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x9f, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x81, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0xa0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0xe0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x20 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x40 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x60 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x80 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x81, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0xa0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0xe0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      Assert.AreEqual(
        BigInteger.Zero,
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x40 }).AsBigInteger());
      Assert.AreEqual(
        BigInteger.Zero - BigInteger.One,
   CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x41, 0x00 }).AsBigInteger());
      Assert.AreEqual(
        BigInteger.Zero - BigInteger.One,
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x40 }).AsBigInteger());
    }

    [Test]
    public void TestUUID() {
      CBORObject obj =
        CBORObject.FromObject(Guid.Parse(
          "00112233-4455-6677-8899-AABBCCDDEEFF"));
      Assert.AreEqual(CBORType.ByteString, obj.Type);
      Assert.AreEqual((BigInteger)37, obj.InnermostTag);
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
Console.Write(String.Empty);
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
Console.Write(String.Empty);
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
Console.Write(String.Empty);
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
Console.Write(String.Empty);
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
      BigInteger minusone = BigInteger.Zero - BigInteger.One;
      Assert.AreEqual(
        minusone - (BigInteger.One << 8),
   CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x42, 1, 0 }).AsBigInteger());
      Assert.AreEqual(
        minusone - (BigInteger.One << 16),
CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x43, 1, 0, 0 }).AsBigInteger());
      Assert.AreEqual(
        minusone - (BigInteger.One << 24),
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x44, 1, 0, 0, 0
          }).AsBigInteger());
      Assert.AreEqual(
        minusone - (BigInteger.One << 32),
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x45, 1, 0, 0, 0, 0
          }).AsBigInteger());
      Assert.AreEqual(
        minusone - (BigInteger.One << 40),
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x46, 1, 0, 0, 0, 0, 0
          }).AsBigInteger());
      Assert.AreEqual(
        minusone - (BigInteger.One << 48),
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x47, 1, 0, 0, 0, 0,
                    0, 0 }).AsBigInteger());
      Assert.AreEqual(
        minusone - (BigInteger.One << 56),
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x48, 1, 0, 0, 0, 0,
                    0, 0, 0 }).AsBigInteger());
      Assert.AreEqual(
        minusone - (BigInteger.One << 64),
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x49, 1, 0, 0, 0, 0,
                    0, 0, 0, 0 }).AsBigInteger());
      Assert.AreEqual(
        minusone - (BigInteger.One << 72),
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x4a, 1, 0, 0, 0, 0,
                    0, 0, 0, 0, 0 }).AsBigInteger());
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
      "[\"abcd\",\"aa\",\"abcd\",\"abcd\",\"bbcd\",\"bbcd\",\"abcd\",\"bbcd\"]"
;
      Assert.AreEqual(expected, cbor.ToJSONString());
    }

    [Test]
    public void TestExtendedNaNZero() {
      Assert.IsFalse(ExtendedDecimal.NaN.IsZero);
      Assert.IsFalse(ExtendedDecimal.SignalingNaN.IsZero);
      Assert.IsFalse(ExtendedFloat.NaN.IsZero);
      Assert.IsFalse(ExtendedFloat.SignalingNaN.IsZero);
      Assert.IsFalse(ExtendedRational.NaN.IsZero);
      Assert.IsFalse(ExtendedRational.SignalingNaN.IsZero);
    }
  }
}
