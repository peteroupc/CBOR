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
import com.upokecenter.cbor.*;

  public class CBORTest2
  {
    @Test
    public void TestRationalCompareDecimal() {
      FastRandom fr = new FastRandom();
      // System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();
      // System.Diagnostics.Stopwatch sw2 = new System.Diagnostics.Stopwatch();
      for (int i = 0; i < 100; ++i) {
        ExtendedRational er = CBORTest.RandomRational(fr);
        int exp = -100000 + fr.NextValue(200000);
        ExtendedDecimal ed = ExtendedDecimal.Create(
          CBORTest.RandomBigInteger(fr),
          BigInteger.valueOf(exp));
        ExtendedRational er2 = ExtendedRational.FromExtendedDecimal(ed);
        // sw1.Start();
        int c2r = er.compareTo(er2);
        // sw1.Stop();sw2.Start();
        int c2d = er.CompareToDecimal(ed);
        // sw2.Stop();
        Assert.assertEquals(c2r, c2d);
      }
      // System.out.println("compareTo: " + (sw1.getElapsedMilliseconds()/1000.0) + " s");
      // System.out.println("CompareToDecimal: " + (sw2.getElapsedMilliseconds()/1000.0) + " s");
    }

    @Test
    public void TestRationalCompare() {
      FastRandom fr = new FastRandom();
      for (int i = 0; i < 100; ++i) {
        BigInteger num = CBORTest.RandomBigInteger(fr);
        num = (num).abs();
        ExtendedRational rat = new ExtendedRational(num, BigInteger.ONE);
        ExtendedRational rat2 = new ExtendedRational(num, BigInteger.valueOf(2));
        if (rat2.compareTo(rat) != -1) {
          Assert.fail(rat + "; " + rat2);
        }
        if (rat.compareTo(rat2) != 1) {
          Assert.fail(rat + "; " + rat2);
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
            Assert.assertEquals(rat + "; " + rat2 + "; " + rat.ToDouble() + "; " + rat2.ToDouble(),0,rat.compareTo(rat2));
          }
        }
      }
    }

    @Test
    public void TestBuiltInTags() {
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc2, 0x00  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc2, 0x20  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc2, 0x60  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc2, (byte)0x80  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc2, (byte)0xA0  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc2, (byte)0xE0  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }

      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, 0x00  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, 0x20  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, 0x60  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, (byte)0x80  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, (byte)0xA0  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, (byte)0xE0  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }

      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc4, 0x00  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc4, 0x20  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc4, 0x40  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc4, 0x60  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc4, (byte)0x80  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc4, (byte)0x81, 0x00  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc4, (byte)0xA0  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc4, (byte)0xE0  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }

      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc5, 0x00  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc5, 0x20  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc5, 0x40  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc5, 0x60  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc5, (byte)0x80  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc5, (byte)0x81, 0x00  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc5, (byte)0xA0  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc5, (byte)0xE0  });
        Assert.fail("Should have failed");
      } catch (CBORException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }

      Assert.assertEquals(BigInteger.ZERO, CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc2, 0x40  }).AsBigInteger());
      Assert.assertEquals(BigInteger.ZERO.subtract(BigInteger.ONE), CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, 0x41, 0x00  }).AsBigInteger());
      Assert.assertEquals(BigInteger.ZERO.subtract(BigInteger.ONE), CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, 0x40  }).AsBigInteger());
    }

    @Test
    public void TestNegativeBigInts() {
      BigInteger minusone = BigInteger.ZERO.subtract(BigInteger.ONE);
      Assert.assertEquals(minusone.subtract(BigInteger.ONE.shiftLeft(8)),
                      CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, 0x42, 1, 0  }).AsBigInteger());
      Assert.assertEquals(minusone.subtract(BigInteger.ONE.shiftLeft(16)),
                      CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, 0x43, 1, 0, 0  }).AsBigInteger());
      Assert.assertEquals(minusone.subtract(BigInteger.ONE.shiftLeft(24)),
                      CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, 0x44, 1, 0, 0, 0  }).AsBigInteger());
      Assert.assertEquals(minusone.subtract(BigInteger.ONE.shiftLeft(32)),
                      CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, 0x45, 1, 0, 0, 0, 0  }).AsBigInteger());
      Assert.assertEquals(minusone.subtract(BigInteger.ONE.shiftLeft(40)),
                      CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, 0x46, 1, 0, 0, 0, 0, 0  }).AsBigInteger());
      Assert.assertEquals(minusone.subtract(BigInteger.ONE.shiftLeft(48)),
                      CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, 0x47, 1, 0, 0, 0, 0, 0, 0  }).AsBigInteger());
      Assert.assertEquals(minusone.subtract(BigInteger.ONE.shiftLeft(56)),
                      CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, 0x48, 1, 0, 0, 0, 0, 0, 0, 0  }).AsBigInteger());
      Assert.assertEquals(minusone.subtract(BigInteger.ONE.shiftLeft(64)),
                      CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, 0x49, 1, 0, 0, 0, 0, 0, 0, 0, 0  }).AsBigInteger());
      Assert.assertEquals(minusone.subtract(BigInteger.ONE.shiftLeft(72)),
                      CBORObject.DecodeFromBytes(new byte[] {  (byte)0xc3, 0x4a, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0  }).AsBigInteger());
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
