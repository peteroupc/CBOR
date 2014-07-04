package com.upokecenter.test;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;

  public class PrecisionContextTest {
    @Test
    public void TestConstructor() {
      try {
        Assert.assertEquals(
null,
new PrecisionContext(-1, Rounding.HalfEven, 0, 0, false));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(
null,
new PrecisionContext(0, Rounding.HalfEven, 0, -1, false));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
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
      PrecisionContext ctx = PrecisionContext.Unlimited;
      Assert.assertEquals(BigInteger.ZERO, ctx.getEMax());
      ctx = PrecisionContext.Unlimited.WithExponentRange(-5, 5);
      Assert.assertEquals(BigInteger.valueOf(5), ctx.getEMax());
    }
    @Test
    public void TestEMin() {
      PrecisionContext ctx = PrecisionContext.Unlimited;
      Assert.assertEquals(BigInteger.ZERO, ctx.getEMin());
      ctx = PrecisionContext.Unlimited.WithExponentRange(-5, 5);
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
      PrecisionContext ctx;
      ctx = PrecisionContext.ForRounding(Rounding.HalfEven);
      Assert.assertEquals(Rounding.HalfEven, ctx.getRounding());
      ctx = PrecisionContext.ForRounding(Rounding.HalfUp);
      Assert.assertEquals(Rounding.HalfUp, ctx.getRounding());
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
      if ((PrecisionContext.Unlimited.toString()) == null)Assert.fail();
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
      try {
 PrecisionContext.Unlimited.WithBigPrecision(BigInteger.ONE.negate());
Assert.fail("Should have failed");
} catch (IllegalArgumentException ex) {
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
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
        BigInteger bigintBig = BigInteger.ONE.shiftLeft(64);
   PrecisionContext.Unlimited.WithBigExponentRange(
bigintBig,
BigInteger.ZERO);
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
      try {
 PrecisionContext.Unlimited.WithPrecision(-1);
Assert.fail("Should have failed");
} catch (IllegalArgumentException ex) {
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      PrecisionContext ctx;
      ctx = PrecisionContext.Unlimited.WithPrecision(6);
      Assert.assertEquals(BigInteger.valueOf(6), ctx.getPrecision());
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
      PrecisionContext pc = new PrecisionContext(0, Rounding.HalfUp, 0, 5, true);
      if (pc.isSimplified())Assert.fail();
      pc = pc.WithSimplified(true);
      if (!(pc.isSimplified()))Assert.fail();
      pc = pc.WithSimplified(false);
      if (pc.isSimplified())Assert.fail();
    }
    @Test
    public void TestWithTraps() {
      // not implemented yet
    }
    @Test
    public void TestWithUnlimitedExponents() {
      PrecisionContext pc = new PrecisionContext(0, Rounding.HalfUp, 0, 5, true);
      if (!(pc.getHasExponentRange()))Assert.fail();
      pc = pc.WithUnlimitedExponents();
      if (pc.getHasExponentRange())Assert.fail();
    }
  }
