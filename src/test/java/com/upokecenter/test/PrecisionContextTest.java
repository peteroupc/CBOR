package com.upokecenter.test;

import com.upokecenter.util.*;
import org.junit.Assert;
import org.junit.Test;

  public class PrecisionContextTest {
    @Test
    public void TestConstructor() {
      // not implemented yet
    }
    @Test
    public void TestAdjustExponent() {
      // not implemented yet
    }
    @Test
    public void TestClampNormalExponents() {
      // not implemented yet
    }
    @Test
    public void TestCopy() {
      // not implemented yet
    }
    @Test
    public void TestEMax() {
      Assert.assertEquals(BigInteger.ZERO, PrecisionContext.Unlimited.EMax);
      PrecisionContext ctx = PrecisionContext.Unlimited.WithExponentRange(-5, 5);
      Assert.assertEquals(BigInteger.valueOf(5), ctx.getEMax());
    }
    @Test
    public void TestEMin() {
      Assert.assertEquals(BigInteger.ZERO, PrecisionContext.Unlimited.EMin);
      PrecisionContext ctx = PrecisionContext.Unlimited.WithExponentRange(-5, 5);
      Assert.assertEquals(BigInteger.valueOf(-5), ctx.getEMin());
    }
    @Test
    public void TestExponentWithinRange() {
      // not implemented yet
    }
    @Test
    public void TestFlags() {
      PrecisionContext ctx = PrecisionContext.Unlimited;
      try {
        ctx.setFlags(5);
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      ctx = ctx.WithBlankFlags();
      try {
        ctx.setFlags(5);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      ctx = ctx.WithNoFlags();
      try {
        ctx.setFlags(5);
        Assert.fail("Should have failed");
      } catch (IllegalStateException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestForPrecision() {
      // not implemented yet
    }
    @Test
    public void TestForPrecisionAndRounding() {
      // not implemented yet
    }
    @Test
    public void TestForRounding() {
      // not implemented yet
    }
    @Test
    public void TestHasExponentRange() {
      // not implemented yet
    }
    @Test
    public void TestHasFlags() {
      // not implemented yet
    }
    @Test
    public void TestHasMaxPrecision() {
      // not implemented yet
    }
    @Test
    public void TestIsPrecisionInBits() {
      // not implemented yet
    }
    @Test
    public void TestIsSimplified() {
      // not implemented yet
    }
    @Test
    public void TestPrecision() {
      // not implemented yet
    }
    @Test
    public void TestRounding() {
      // not implemented yet
    }
    @Test
    public void TestToString() {
      if(!(PrecisionContext.Unlimited.toString().Contains("PrecisionContext")))Assert.fail();
    }
    @Test
    public void TestTraps() {
      // not implemented yet
    }
    @Test
    public void TestWithAdjustExponent() {
      // not implemented yet
    }
    @Test
    public void TestWithBigExponentRange() {
      // not implemented yet
    }
    @Test
    public void TestWithBigPrecision() {
      // not implemented yet
    }
    @Test
    public void TestWithBlankFlags() {
      // not implemented yet
    }
    @Test
    public void TestWithExponentClamp() {
      // not implemented yet
    }
    @Test
    public void TestWithExponentRange() {
      try {
        PrecisionContext.Unlimited.WithExponentRange(1, 0);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        PrecisionContext.Unlimited.WithBigExponentRange(null, BigInteger.ZERO);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        PrecisionContext.Unlimited.WithBigExponentRange(BigInteger.ZERO, null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        PrecisionContext.Unlimited.WithExponentRange((int)BigInteger.ONE, (int)BigInteger.ZERO);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestWithNoFlags() {
      // not implemented yet
    }
    @Test
    public void TestWithPrecision() {
      // not implemented yet
    }
    @Test
    public void TestWithPrecisionInBits() {
      // not implemented yet
    }
    @Test
    public void TestWithRounding() {
      // not implemented yet
    }
    @Test
    public void TestWithSimplified() {
      // not implemented yet
    }
    @Test
    public void TestWithTraps() {
      // not implemented yet
    }
    @Test
    public void TestWithUnlimitedExponents() {
      // not implemented yet
    }
  }
