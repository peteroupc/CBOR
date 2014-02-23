package com.upokecenter.test;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/21/2014
 * Time: 12:43 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;

  public class CBORTest2
  {
    @Test
    public void TestRationalCompare() {
      FastRandom fr = new FastRandom();
      for (int i = 0; i < 100; ++i) {
        BigInteger num = CBORTest.RandomBigInteger(fr);
        num = (num).abs();
        ExtendedRational rat = new ExtendedRational(num, BigInteger.ONE);
        ExtendedRational rat2 = new ExtendedRational(num, BigInteger.valueOf(2));
        if (rat2.compareTo(rat) !=-1) {
 Assert.assertEquals(-1, rat2.compareTo(rat), rat + ", " + rat2);
}
        if (rat.compareTo(rat2) != 1) {
 Assert.assertEquals(1, rat.compareTo(rat2), rat + ", " + rat2);
}
      }
      Assert.assertEquals(
-1,
                      new ExtendedRational(BigInteger.ONE, BigInteger.valueOf(2)).compareTo(
                        new ExtendedRational(BigInteger.valueOf(4), BigInteger.ONE)));
      for (int i = 0; i < 100; ++i) {
        BigInteger num = CBORTest.RandomBigInteger(fr);
        BigInteger den = CBORTest.RandomBigInteger(fr);
        if (den.signum()==0) {
          den = BigInteger.ONE;
        }
        ExtendedRational rat = new ExtendedRational(num, den);
        for (int j = 0; j < 10; ++j) {
          BigInteger num2 = num;
          BigInteger den2 = den;
          BigInteger mult = CBORTest.RandomBigInteger(fr);
          if (mult.signum()==0 || mult.equals(BigInteger.ONE)) {
            mult = BigInteger.valueOf(2);
          }
          num2=num2.multiply(mult);
          den2=den2.multiply(mult);
          ExtendedRational rat2 = new ExtendedRational(num2, den2);
          if (rat.compareTo(rat2) != 0) {
            Assert.assertEquals(
0, rat.compareTo(rat2), rat + ", " + rat2 + ", " +
                            rat.ToDouble() + ", " + rat2.ToDouble());
          }
        }
      }
    }

    @Test
    public void TestExtendedNaNZero() {
      if(ExtendedDecimal.NaN.signum()==0)Assert.fail();
      if(ExtendedDecimal.SignalingNaN.signum()==0)Assert.fail();
      if(ExtendedFloat.NaN.signum()==0)Assert.fail();
      if(ExtendedFloat.SignalingNaN.signum()==0)Assert.fail();
      if(ExtendedRational.NaN.signum()==0)Assert.fail();
      if(ExtendedRational.SignalingNaN.signum()==0)Assert.fail();
    }

    @Test
    public void TestToBigIntegerNonFinite() {
      try {
        ExtendedDecimal.PositiveInfinity.ToBigInteger();
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        ExtendedDecimal.NegativeInfinity.ToBigInteger();
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        ExtendedDecimal.NaN.ToBigInteger();
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        ExtendedDecimal.SignalingNaN.ToBigInteger();
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        ExtendedFloat.PositiveInfinity.ToBigInteger();
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        ExtendedFloat.NegativeInfinity.ToBigInteger();
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        ExtendedFloat.NaN.ToBigInteger();
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        ExtendedFloat.SignalingNaN.ToBigInteger();
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
  }
