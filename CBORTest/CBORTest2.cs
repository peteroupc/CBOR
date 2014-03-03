/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/21/2014
 * Time: 12:43 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;
using PeterO;
using PeterO.Cbor;

namespace Test
{
  [TestFixture]
  public class CBORTest2
  {
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
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0xA0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0xE0 });
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
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0xA0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0xE0 });
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
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0x81, 0x00 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0xA0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc4, 0xE0 });
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
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0xA0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] { 0xc5, 0xE0 });
        Assert.Fail("Should have failed");
      } catch (CBORException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      Assert.AreEqual(BigInteger.Zero, CBORObject.DecodeFromBytes(new byte[] { 0xc2, 0x40 }).AsBigInteger());
      Assert.AreEqual(BigInteger.Zero-BigInteger.One, CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x41, 0x00 }).AsBigInteger());
      Assert.AreEqual(BigInteger.Zero-BigInteger.One, CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x40 }).AsBigInteger());
    }

    [Test]
    public void TestNegativeBigInts() {
      BigInteger minusone = BigInteger.Zero-BigInteger.One;
      Assert.AreEqual(minusone-(BigInteger.One << 8),
                      CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x42, 1, 0 }).AsBigInteger());
      Assert.AreEqual(minusone-(BigInteger.One << 16),
                      CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x43, 1, 0, 0 }).AsBigInteger());
      Assert.AreEqual(minusone-(BigInteger.One << 24),
                      CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x44, 1, 0, 0, 0 }).AsBigInteger());
      Assert.AreEqual(minusone-(BigInteger.One << 32),
                      CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x45, 1, 0, 0, 0, 0 }).AsBigInteger());
      Assert.AreEqual(minusone-(BigInteger.One << 40),
                      CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x46, 1, 0, 0, 0, 0, 0 }).AsBigInteger());
      Assert.AreEqual(minusone-(BigInteger.One << 48),
                      CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x47, 1, 0, 0, 0, 0, 0, 0 }).AsBigInteger());
      Assert.AreEqual(minusone-(BigInteger.One << 56),
                      CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x48, 1, 0, 0, 0, 0, 0, 0, 0 }).AsBigInteger());
      Assert.AreEqual(minusone-(BigInteger.One << 64),
                      CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x49, 1, 0, 0, 0, 0, 0, 0, 0, 0 }).AsBigInteger());
      Assert.AreEqual(minusone-(BigInteger.One << 72),
                      CBORObject.DecodeFromBytes(new byte[] { 0xc3, 0x4a, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 }).AsBigInteger());
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
