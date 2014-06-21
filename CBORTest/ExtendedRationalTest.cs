using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO;

namespace Test {
  [TestClass]
  public class ExtendedRationalTest {
    [TestMethod]
    public void TestConstructor() {
      // not implemented yet
    }
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
    public void TestCompareToDecimal() {
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
    public void TestDenominator() {
      // not implemented yet
    }
    [TestMethod]
    public void TestDivide() {
      // not implemented yet
    }
    [TestMethod]
    public void TestEquals() {
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
    public void TestFromExtendedDecimal() {
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
    public void TestGetHashCode() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsFinite() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsInfinity() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsNaN() {
      // not implemented yet
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
      Assert.IsTrue(ExtendedRational.NegativeZero.IsZero);
      Assert.IsTrue(ExtendedRational.Zero.IsZero);
      Assert.IsFalse(ExtendedRational.One.IsZero);
      Assert.IsFalse(ExtendedRational.NegativeInfinity.IsZero);
      Assert.IsFalse(ExtendedRational.PositiveInfinity.IsZero);
    }
    [TestMethod]
    public void TestMultiply() {
      // not implemented yet
    }
    [TestMethod]
    public void TestNegate() {
      // not implemented yet
    }
    [TestMethod]
    public void TestNumerator() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRemainder() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSign() {
      Assert.AreEqual(0, ExtendedRational.NegativeZero.Sign);
      Assert.AreEqual(0, ExtendedRational.Zero.Sign);
      Assert.AreEqual(1, ExtendedRational.One.Sign);
      Assert.AreEqual(-1, ExtendedRational.NegativeInfinity.Sign);
      Assert.AreEqual(1, ExtendedRational.PositiveInfinity.Sign);
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
    public void TestToExtendedDecimal() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToExtendedDecimalExactIfPossible() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToExtendedFloat() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToExtendedFloatExactIfPossible() {
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
    public void TestUnsignedNumerator() {
      // not implemented yet
    }
  }
}
