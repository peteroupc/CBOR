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
      Assert.IsTrue(ExtendedRational.PositiveInfinity.IsInfinity());
      Assert.IsTrue(ExtendedRational.NegativeInfinity.IsInfinity());
      Assert.IsFalse(ExtendedRational.Zero.IsInfinity());
      Assert.IsFalse(ExtendedRational.NaN.IsInfinity());
    }
    [TestMethod]
    public void TestIsNaN() {
      Assert.IsFalse(ExtendedRational.PositiveInfinity.IsNaN());
      Assert.IsFalse(ExtendedRational.NegativeInfinity.IsNaN());
      Assert.IsFalse(ExtendedRational.Zero.IsNaN());
      Assert.IsTrue(ExtendedRational.NaN.IsNaN());
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
      var fr = new FastRandom();
      for (var i = 0; i < 100; ++i) {
        ExtendedRational er;
        ExtendedRational er2;
        er = new ExtendedRational(
          RandomObjects.RandomBigInteger(fr),
          BigInteger.One);
        er2 = new ExtendedRational(
          RandomObjects.RandomBigInteger(fr),
          BigInteger.One);
        if (er2.IsZero || !er2.IsFinite) {
          continue;
        }
        if (er.IsZero || !er.IsFinite) {
          // Code below will divide by "er",
          // so skip if "er" is zero
          continue;
        }
        ExtendedRational ermult = er.Multiply(er2);
        ExtendedRational erdiv = ermult.Divide(er);
        erdiv = ermult.Remainder(er);
        if (!erdiv.IsZero) {
          Assert.Fail(ermult + "; " + er);
        }
        erdiv = ermult.Remainder(er2);
        if (!erdiv.IsZero) {
          Assert.Fail(er + "; " + er2);
        }
      }
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
      // test for correct rounding
      double dbl;
      dbl = ExtendedRational.FromExtendedDecimal(
        ExtendedDecimal.FromString(
       "1.972579273363468721491642554610734805464744567871093749999999999999"))
        .ToDouble();
      Assert.AreEqual(
        "1.9725792733634686104693400920950807631015777587890625",
        ExtendedFloat.FromDouble(dbl).ToPlainString());
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
