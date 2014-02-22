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
    public void TestRationalCompare() {
      FastRandom fr = new FastRandom();
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
          Assert.AreEqual(0, rat.CompareTo(rat2));
        }
      }
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
