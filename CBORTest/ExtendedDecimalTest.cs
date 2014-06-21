using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO;

namespace Test {
  [TestClass]
  public class ExtendedDecimalTest {
    [TestMethod]
    public void TestAbs() {
      // not implemented yet
    }
    [TestMethod]
    public void TestAdd() {
      // not implemented yet
    }
    [TestMethod]
    public void TestCompareTo() {
      // not implemented yet
    }
    [TestMethod]
    public void TestCompareToBinary() {
      // not implemented yet
    }
    [TestMethod]
    public void TestCompareToSignal() {
      // not implemented yet
    }
    [TestMethod]
    public void TestCompareToWithContext() {
      // not implemented yet
    }
    [TestMethod]
    public void TestCreate() {
      // not implemented yet
    }
    [TestMethod]
    public void TestCreateNaN() {
      // not implemented yet
    }
    [TestMethod]
    public void TestDivide() {
      // not implemented yet
    }
    [TestMethod]
    public void TestDivideToExponent() {
      // not implemented yet
    }
    [TestMethod]
    public void TestDivideToIntegerNaturalScale() {
      // not implemented yet
    }
    [TestMethod]
    public void TestDivideToIntegerZeroScale() {
      // not implemented yet
    }
    [TestMethod]
    public void TestDivideToSameExponent() {
      // not implemented yet
    }
    [TestMethod]
    public void TestEquals() {
      // not implemented yet
    }
    [TestMethod]
    public void TestExp() {
      // not implemented yet
    }
    [TestMethod]
    public void TestExponent() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromBigInteger() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromDouble() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromExtendedFloat() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromInt32() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromInt64() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromSingle() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromString() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetHashCode() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsFinite() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsInfinity() {
      Assert.IsTrue(ExtendedDecimal.PositiveInfinity.IsInfinity());
      Assert.IsTrue(ExtendedDecimal.NegativeInfinity.IsInfinity());
      Assert.IsFalse(ExtendedDecimal.Zero.IsInfinity());
      Assert.IsFalse(ExtendedDecimal.NaN.IsInfinity());
    }
    [TestMethod]
    public void TestIsNaN() {
      Assert.IsFalse(ExtendedDecimal.PositiveInfinity.IsNaN());
      Assert.IsFalse(ExtendedDecimal.NegativeInfinity.IsNaN());
      Assert.IsFalse(ExtendedDecimal.Zero.IsNaN());
      Assert.IsTrue(ExtendedDecimal.NaN.IsNaN());
    }
    [TestMethod]
    public void TestIsNegative() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsNegativeInfinity() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsPositiveInfinity() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsQuietNaN() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsSignalingNaN() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsZero() {
      // not implemented yet
    }
    [TestMethod]
    public void TestLog() {
      // not implemented yet
    }
    [TestMethod]
    public void TestLog10() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMantissa() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMax() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMaxMagnitude() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMin() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMinMagnitude() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMultiply() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMultiplyAndAdd() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMultiplyAndSubtract() {
      // not implemented yet
    }
    [TestMethod]
    public void TestNegate() {
      // not implemented yet
    }
    [TestMethod]
    public void TestNextMinus() {
      // not implemented yet
    }
    [TestMethod]
    public void TestNextPlus() {
      // not implemented yet
    }
    [TestMethod]
    public void TestNextToward() {
      // not implemented yet
    }
    [TestMethod]
    public void TestPI() {
      // not implemented yet
    }
    [TestMethod]
    public void TestPlus() {
      // not implemented yet
    }
    [TestMethod]
    public void TestPow() {
      // not implemented yet
    }
    [TestMethod]
    public void TestQuantize() {
      // not implemented yet
    }
    [TestMethod]
    public void TestReduce() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRemainder() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRemainderNaturalScale() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRemainderNear() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRoundToBinaryPrecision() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRoundToExponent() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRoundToExponentExact() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRoundToIntegralExact() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRoundToIntegralNoRoundedFlag() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRoundToPrecision() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSign() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSquareRoot() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSubtract() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToBigInteger() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToBigIntegerExact() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToDouble() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToEngineeringString() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToExtendedFloat() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToPlainString() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToSingle() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToString() {
      // not implemented yet
    }
    [TestMethod]
    public void TestUnsignedMantissa() {
      // not implemented yet
    }
  }
}
