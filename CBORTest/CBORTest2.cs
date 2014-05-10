/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.IO;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;

namespace Test {
  [TestFixture]
  public class CBORTest2
  {
    [Test]
    public void TestCyclicRefs() {
      CBORObject cbor = CBORObject.NewArray();
      cbor.Add(CBORObject.NewArray());
      cbor.Add(cbor);
      cbor[0].Add(cbor);
      try {
        cbor.WriteTo(new MemoryStream());
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
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
      bytes = new byte[] { 0x9f, 0xd8, 28, 0x81, 1, 0xd8, 29, 0, 3, 3, 0xd8, 29, 0, 0xff };
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
      FastRandom fr = new FastRandom();
      // System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();
      // System.Diagnostics.Stopwatch sw2 = new System.Diagnostics.Stopwatch();
      for (int i = 0; i < 100; ++i) {
        ExtendedRational er = CBORTest.RandomRational(fr);
        int exp = -100000 + fr.NextValue(200000);
        ExtendedDecimal ed = ExtendedDecimal.Create(
          CBORTest.RandomBigInteger(fr),
          (BigInteger)exp);
        ExtendedRational er2 = ExtendedRational.FromExtendedDecimal(ed);
        // sw1.Start();
        int c2r = er.CompareTo(er2);
        // sw1.Stop();sw2.Start();
        int c2d = er.CompareToDecimal(ed);
        // sw2.Stop();
        Assert.AreEqual(c2r, c2d);
      }
      // Console.WriteLine("CompareTo: " + (sw1.ElapsedMilliseconds/1000.0) + " s");
      // Console.WriteLine("CompareToDecimal: " + (sw2.ElapsedMilliseconds/1000.0) + " s");
    }

    [Test]
    public void TestRationalDivide() {
      FastRandom fr = new FastRandom();
      for (int i = 0; i < 100; ++i) {
        ExtendedRational er = CBORTest.RandomRational(fr);
        ExtendedRational er2 = CBORTest.RandomRational(fr);
        if (er2.IsZero || !er2.IsFinite) {
          continue;
        }
        ExtendedRational ermult = er.Multiply(er2);
        ExtendedRational erdiv = ermult.Divide(er);
        if (erdiv.CompareTo(er2) != 0) {
          Assert.Fail(er + "; " + er2);
        }
        erdiv = ermult.Divide(er2);
        if (erdiv.CompareTo(er) != 0) {
          Assert.Fail(er + "; " + er2);
        }
      }
    }

    [Test]
    public void TestRationalRemainder() {
      FastRandom fr = new FastRandom();
      for (int i = 0; i < 100; ++i) {
        ExtendedRational er;
        ExtendedRational er2;
        er = new ExtendedRational(CBORTest.RandomBigInteger(fr), BigInteger.One);
        er2 = new ExtendedRational(CBORTest.RandomBigInteger(fr), BigInteger.One);
        if (er2.IsZero || !er2.IsFinite) {
          continue;
        }
        ExtendedRational ermult = er.Multiply(er2);
        ExtendedRational erdiv = ermult.Divide(er);
        erdiv = ermult.Remainder(er);
        if (!erdiv.IsZero) {
          Assert.Fail(ermult + "; " + er);
        }
        erdiv = ermult.Remainder(er2);
        if (!erdiv.IsZero) {
          Assert.Fail(er + "; " + er2);
        }
      }
    }

    [Test]
    public void TestRationalCompare() {
      FastRandom fr = new FastRandom();
      for (int i = 0; i < 100; ++i) {
        BigInteger num = CBORTest.RandomBigInteger(fr);
        num = BigInteger.Abs(num);
        ExtendedRational rat = new ExtendedRational(num, BigInteger.One);
        ExtendedRational rat2 = new ExtendedRational(num, (BigInteger)2);
        if (rat2.CompareTo(rat) != -1) {
          Assert.Fail(rat + "; " + rat2);
        }
        if (rat.CompareTo(rat2) != 1) {
          Assert.Fail(rat + "; " + rat2);
        }
      }
      Assert.AreEqual(
        -1,
        new ExtendedRational(BigInteger.One, (BigInteger)2).CompareTo(
          new ExtendedRational((BigInteger)4, BigInteger.One)));
      for (int i = 0; i < 100; ++i) {
        BigInteger num = CBORTest.RandomBigInteger(fr);
        BigInteger den = CBORTest.RandomBigInteger(fr);
        if (den.IsZero) {
          den = BigInteger.One;
        }
        ExtendedRational rat = new ExtendedRational(num, den);
        for (int j = 0; j < 10; ++j) {
          BigInteger num2 = num;
          BigInteger den2 = den;
          BigInteger mult = CBORTest.RandomBigInteger(fr);
          if (mult.IsZero || mult.Equals(BigInteger.One)) {
            mult = (BigInteger)2;
          }
          num2 *= (BigInteger)mult;
          den2 *= (BigInteger)mult;
          ExtendedRational rat2 = new ExtendedRational(num2, den2);
          if (rat.CompareTo(rat2) != 0) {
            Assert.AreEqual(
              0,
              rat.CompareTo(rat2),
              rat + "; " + rat2 + "; " + rat.ToDouble() + "; " + rat2.ToDouble());
          }
        }
      }
    }

    [Test]
    public void TestBuiltInTags() {
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x20 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x60 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x80 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0xa0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0xe0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x20 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x60 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x80 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0xa0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0xe0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x20 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x40 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x60 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x80 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
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
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xd8, 0x1e, 0x9f, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
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
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x9f, 0x00, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x9f, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x9f, 0xff });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x81, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0xa0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0xe0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x20 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x40 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x60 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x80 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0x81, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0xa0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0xe0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      Assert.AreEqual(BigInteger.Zero, CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x40 }).AsBigInteger());
      Assert.AreEqual(BigInteger.Zero - BigInteger.One, CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x41, 0x00 }).AsBigInteger());
      Assert.AreEqual(BigInteger.Zero - BigInteger.One, CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x40 }).AsBigInteger());
    }
    
    [Test]
    public void TestUUID(){
      CBORObject obj=CBORObject.FromObject(Guid.Parse("00112233-4455-6677-8899-AABBCCDDEEFF"));
      Assert.AreEqual(CBORType.ByteString,obj.Type);
      byte[] bytes=obj.GetByteString();
      Assert.AreEqual(16,bytes.Length);
      Assert.AreEqual(0x00,bytes[0]);
      Assert.AreEqual(0x11,bytes[1]);
      Assert.AreEqual(0x22,bytes[2]);
      Assert.AreEqual(0x33,bytes[3]);
      Assert.AreEqual(0x44,bytes[4]);
      Assert.AreEqual(0x55,bytes[5]);
      Assert.AreEqual(0x66,bytes[6]);
      Assert.AreEqual(0x77,bytes[7]);
      Assert.AreEqual((byte)0x88,bytes[8]);
      Assert.AreEqual((byte)0x99,bytes[9]);
      Assert.AreEqual((byte)0xaa,bytes[10]);
      Assert.AreEqual((byte)0xbb,bytes[11]);
      Assert.AreEqual((byte)0xcc,bytes[12]);
      Assert.AreEqual((byte)0xdd,bytes[13]);
      Assert.AreEqual((byte)0xee,bytes[14]);
      Assert.AreEqual((byte)0xff,bytes[15]);
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
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x44, 1, 0, 0, 0 }).AsBigInteger());
      Assert.AreEqual(
        minusone - (BigInteger.One << 32),
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x45, 1, 0, 0, 0, 0 }).AsBigInteger());
      Assert.AreEqual(
        minusone - (BigInteger.One << 40),
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x46, 1, 0, 0, 0, 0, 0 }).AsBigInteger());
      Assert.AreEqual(
        minusone - (BigInteger.One << 48),
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x47, 1, 0, 0, 0, 0, 0, 0 }).AsBigInteger());
      Assert.AreEqual(
        minusone - (BigInteger.One << 56),
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x48, 1, 0, 0, 0, 0, 0, 0, 0 }).AsBigInteger());
      Assert.AreEqual(
        minusone - (BigInteger.One << 64),
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x49, 1, 0, 0, 0, 0, 0, 0, 0, 0 }).AsBigInteger());
      Assert.AreEqual(
        minusone - (BigInteger.One << 72),
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x4a, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 }).AsBigInteger());
    }

    [Test]
    public void TestStringRefs() {
      CBORObject cbor = CBORObject.DecodeFromBytes(
        new byte[] { 0xd9,
        1,
        0,
        0x9f,
        0x64,
        0x61,
        0x62,
        0x63,
        0x64,
        0xd8,
        0x19,
        0x00,
        0xd8,
        0x19,
        0x00,
        0x64,
        0x62,
        0x62,
        0x63,
        0x64,
        0xd8,
        0x19,
        0x01,
        0xd8,
        0x19,
        0x00,
        0xd8,
        0x19,
        0x01,
        0xff });
      string expected = "[\"abcd\",\"abcd\",\"abcd\",\"bbcd\",\"bbcd\",\"abcd\",\"bbcd\"]";
      Assert.AreEqual(expected, cbor.ToJSONString());
      cbor = CBORObject.DecodeFromBytes(
        new byte[] { 0xd9,
        1,
        0,
        0x9f,
        0x64,
        0x61,
        0x62,
        0x63,
        0x64,
        0x62,
        0x61,
        0x61,
        0xd8,
        0x19,
        0x00,
        0xd8,
        0x19,
        0x00,
        0x64,
        0x62,
        0x62,
        0x63,
        0x64,
        0xd8,
        0x19,
        0x01,
        0xd8,
        0x19,
        0x00,
        0xd8,
        0x19,
        0x01,
        0xff });
      expected = "[\"abcd\",\"aa\",\"abcd\",\"abcd\",\"bbcd\",\"bbcd\",\"abcd\",\"bbcd\"]";
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

    [Test]
    public void TestToBigIntegerNonFinite() {
      try {
        ExtendedDecimal.PositiveInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.NegativeInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.NaN.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.SignalingNaN.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.PositiveInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.NegativeInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.NaN.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.SignalingNaN.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
  }
}
