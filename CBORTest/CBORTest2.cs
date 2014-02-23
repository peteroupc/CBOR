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
        int exp = fr.NextValue(200000) -100000;
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
        if (rat2.CompareTo(rat) !=-1) {
          Assert.AreEqual(-1, rat2.CompareTo(rat), rat + ", " + rat2);
        }
        if (rat.CompareTo(rat2) != 1) {
          Assert.AreEqual(1, rat.CompareTo(rat2), rat + ", " + rat2);
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

              0, rat.CompareTo(
 rat2), rat + ", " + rat2 + ", " +
              rat.ToDouble() + ", " + rat2.ToDouble());
          }
        }
      }
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
