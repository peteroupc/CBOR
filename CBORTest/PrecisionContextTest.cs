using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO;

namespace Test {
  [TestClass]
  public class PrecisionContextTest {
    [TestMethod]
    public void TestConstructor() {
      try {
        Assert.AreEqual(
          null,
          new PrecisionContext(-1, Rounding.HalfEven, 0, 0, false));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(
          null,
          new PrecisionContext(0, Rounding.HalfEven, 0, -1, false));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
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
      PrecisionContext ctx = PrecisionContext.Unlimited;
      Assert.AreEqual(BigInteger.Zero, ctx.EMax);
      ctx = PrecisionContext.Unlimited.WithExponentRange(-5, 5);
      Assert.AreEqual((BigInteger)5, ctx.EMax);
    }
    [TestMethod]
    public void TestEMin() {
      PrecisionContext ctx = PrecisionContext.Unlimited;
      Assert.AreEqual(BigInteger.Zero, ctx.EMin);
      ctx = PrecisionContext.Unlimited.WithExponentRange(-5, 5);
      Assert.AreEqual((BigInteger)(-5), ctx.EMin);
    }
    [TestMethod]
    public void TestExponentWithinRange() {
  Assert.IsTrue(PrecisionContext.Unlimited.ExponentWithinRange(BigInteger.fromString(
"-9999999")));

  Assert.IsTrue(PrecisionContext.Unlimited.ExponentWithinRange(BigInteger.fromString(
"9999999")));
      try {
 PrecisionContext.Unlimited.ExponentWithinRange(null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
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
    public void TestCliDecimal() {
      ExtendedDecimal valueEdTmp;
      valueEdTmp = ExtendedDecimal.FromString(
        "-79228162514264337593543950336" ).RoundToPrecision(PrecisionContext.CliDecimal);
      Assert.AreEqual(
        ExtendedDecimal.NegativeInfinity,
        valueEdTmp);
      valueEdTmp = ExtendedDecimal.FromString(
     "8.782580686213340724E+28").RoundToPrecision(PrecisionContext.CliDecimal);
      Assert.AreEqual(
        ExtendedDecimal.PositiveInfinity,
        valueEdTmp);
      Assert.AreEqual(
        ExtendedDecimal.NegativeInfinity,
        ExtendedDecimal.FromString(
        "-9.3168444507547E+28").RoundToPrecision(PrecisionContext.CliDecimal));
      {
        string stringTemp =

          ExtendedDecimal.FromString(
            "-9344285899206687626894794544.04982268810272216796875" ).RoundToPrecision(PrecisionContext.CliDecimal)
          .ToPlainString();
        Assert.AreEqual(
          "-9344285899206687626894794544",
          stringTemp);
      }
      Assert.AreEqual(
        ExtendedDecimal.PositiveInfinity,
        ExtendedDecimal.FromString(
          "96148154858060747311034406200" ).RoundToPrecision(PrecisionContext.CliDecimal));
      Assert.AreEqual(
        ExtendedDecimal.PositiveInfinity,
        ExtendedDecimal.FromString(
          "90246605365627217170000000000" ).RoundToPrecision(PrecisionContext.CliDecimal));
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
      PrecisionContext ctx;
      ctx = PrecisionContext.ForRounding(Rounding.HalfEven);
      Assert.AreEqual(Rounding.HalfEven, ctx.Rounding);
      ctx = PrecisionContext.ForRounding(Rounding.HalfUp);
      Assert.AreEqual(Rounding.HalfUp, ctx.Rounding);
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
      Assert.IsNotNull(PrecisionContext.Unlimited.ToString());
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
      try {
        PrecisionContext.Unlimited.WithBigPrecision(BigInteger.One.negate());
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
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
        BigInteger bigintBig = BigInteger.One << 64;
        PrecisionContext.Unlimited.WithBigExponentRange(
          bigintBig,
          BigInteger.Zero);
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
      try {
        PrecisionContext.Unlimited.WithPrecision(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      PrecisionContext ctx;
      ctx = PrecisionContext.Unlimited.WithPrecision(6);
      Assert.AreEqual((BigInteger)6, ctx.Precision);
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
      var pc = new PrecisionContext(0, Rounding.HalfUp, 0, 5, true);
      Assert.IsFalse(pc.IsSimplified);
      pc = pc.WithSimplified(true);
      Assert.IsTrue(pc.IsSimplified);
      pc = pc.WithSimplified(false);
      Assert.IsFalse(pc.IsSimplified);
    }
    [TestMethod]
    public void TestWithTraps() {
      // not implemented yet
    }
    [TestMethod]
    public void TestWithUnlimitedExponents() {
      var pc = new PrecisionContext(0, Rounding.HalfUp, 0, 5, true);
      Assert.IsTrue(pc.HasExponentRange);
      pc = pc.WithUnlimitedExponents();
      Assert.IsFalse(pc.HasExponentRange);
    }
  }
}
