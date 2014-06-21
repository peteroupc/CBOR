using System;
using PeterO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test {
  [TestClass]
  public class PrecisionContextTest {
    [TestMethod]
    public void TestConstructor() {
      // not implemented yet
    }
    [TestMethod]
    public void TestAdjustExponent() {
      // not implemented yet
    }
    [TestMethod]
    public void TestClampNormalExponents() {
      // not implemented yet
    }
    [TestMethod]
    public void TestCopy() {
      // not implemented yet
    }
    [TestMethod]
    public void TestEMax() {
      Assert.AreEqual(BigInteger.Zero, PrecisionContext.Unlimited.EMax);
      PrecisionContext ctx = PrecisionContext.Unlimited.WithExponentRange(-5, 5);
      Assert.AreEqual((BigInteger)5, ctx.EMax);
    }
    [TestMethod]
    public void TestEMin() {
      Assert.AreEqual(BigInteger.Zero, PrecisionContext.Unlimited.EMin);
      PrecisionContext ctx = PrecisionContext.Unlimited.WithExponentRange(-5, 5);
      Assert.AreEqual((BigInteger)(-5), ctx.EMin);
    }
    [TestMethod]
    public void TestExponentWithinRange() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFlags() {
      PrecisionContext ctx = PrecisionContext.Unlimited;
      try {
        ctx.Flags = 5;
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      ctx = ctx.WithBlankFlags();
      try {
        ctx.Flags = 5;
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      ctx = ctx.WithNoFlags();
      try {
        ctx.Flags = 5;
        Assert.Fail("Should have failed");
      } catch (InvalidOperationException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestForPrecision() {
      // not implemented yet
    }
    [TestMethod]
    public void TestForPrecisionAndRounding() {
      // not implemented yet
    }
    [TestMethod]
    public void TestForRounding() {
      // not implemented yet
    }
    [TestMethod]
    public void TestHasExponentRange() {
      // not implemented yet
    }
    [TestMethod]
    public void TestHasFlags() {
      // not implemented yet
    }
    [TestMethod]
    public void TestHasMaxPrecision() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsPrecisionInBits() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsSimplified() {
      // not implemented yet
    }
    [TestMethod]
    public void TestPrecision() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRounding() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToString() {
      Assert.IsTrue(PrecisionContext.Unlimited.ToString().Contains("PrecisionContext"));
    }
    [TestMethod]
    public void TestTraps() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWithAdjustExponent() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWithBigExponentRange() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWithBigPrecision() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWithBlankFlags() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWithExponentClamp() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWithExponentRange() {
      try {
        PrecisionContext.Unlimited.WithExponentRange(1, 0);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        PrecisionContext.Unlimited.WithBigExponentRange(null, BigInteger.Zero);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        PrecisionContext.Unlimited.WithBigExponentRange(BigInteger.Zero, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        PrecisionContext.Unlimited.WithExponentRange((int)BigInteger.One, (int)BigInteger.Zero);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestWithNoFlags() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWithPrecision() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWithPrecisionInBits() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWithRounding() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWithSimplified() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWithTraps() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWithUnlimitedExponents() {
      // not implemented yet
    }
  }
}
