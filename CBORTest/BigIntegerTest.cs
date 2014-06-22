using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO;

namespace Test {
  [TestClass]
  public class BigIntegerTest {
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
    public void TestAnd() {
      // not implemented yet
    }
    [TestMethod]
    public void TestBitLength() {
      Assert.AreEqual(31, BigInteger.valueOf(-2147483647L).bitLength());
      Assert.AreEqual(31, BigInteger.valueOf(-2147483648L).bitLength());
      Assert.AreEqual(32, BigInteger.valueOf(-2147483649L).bitLength());
      Assert.AreEqual(32, BigInteger.valueOf(-2147483650L).bitLength());
      Assert.AreEqual(31, BigInteger.valueOf(2147483647L).bitLength());
      Assert.AreEqual(32, BigInteger.valueOf(2147483648L).bitLength());
      Assert.AreEqual(32, BigInteger.valueOf(2147483649L).bitLength());
      Assert.AreEqual(32, BigInteger.valueOf(2147483650L).bitLength());
      Assert.AreEqual(0, BigInteger.valueOf(0).bitLength());
      Assert.AreEqual(1, BigInteger.valueOf(1).bitLength());
      Assert.AreEqual(2, BigInteger.valueOf(2).bitLength());
      Assert.AreEqual(2, BigInteger.valueOf(2).bitLength());
      Assert.AreEqual(31, BigInteger.valueOf(Int32.MaxValue).bitLength());
      Assert.AreEqual(31, BigInteger.valueOf(Int32.MinValue).bitLength());
      Assert.AreEqual(16, BigInteger.valueOf(65535).bitLength());
      Assert.AreEqual(16, BigInteger.valueOf(-65535).bitLength());
      Assert.AreEqual(17, BigInteger.valueOf(65536).bitLength());
      Assert.AreEqual(16, BigInteger.valueOf(-65536).bitLength());
      Assert.AreEqual(65, BigInteger.fromString("19084941898444092059").bitLength());
      Assert.AreEqual(65, BigInteger.fromString("-19084941898444092059").bitLength());
      Assert.AreEqual(0, BigInteger.valueOf(-1).bitLength());
      Assert.AreEqual(1, BigInteger.valueOf(-2).bitLength());
    }
    [TestMethod]
    public void TestCanFitInInt() {
      // not implemented yet
    }
    [TestMethod]
    public void TestCompareTo() {
      // not implemented yet
    }
    [TestMethod]
    public void TestDivide() {
      // not implemented yet
    }
    [TestMethod]
    public void TestDivideAndRemainder() {
      try {
        BigInteger.One.divideAndRemainder(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestDivRem() {
      // not implemented yet
    }
    [TestMethod]
    public void TestEquals() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromByteArray() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromString() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromSubstring() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGcd() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetBits() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetDigitCount() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetHashCode() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetLowestSetBit() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetUnsignedBitLength() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGreatestCommonDivisor() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIntValue() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIntValueChecked() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIntValueUnchecked() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsEven() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsPowerOfTwo() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsZero() {
      // not implemented yet
    }
    [TestMethod]
    public void TestLongValue() {
      // not implemented yet
    }
    [TestMethod]
    public void TestLongValueChecked() {
      // not implemented yet
    }
    [TestMethod]
    public void TestLongValueUnchecked() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMod() {
      try {
        BigInteger.One.mod(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestModPow() {
      // not implemented yet
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
    public void TestNot() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOne() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorAddition() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorDivision() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorExplicit() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorGreaterThan() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorGreaterThanOrEqual() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorImplicit() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorLeftShift() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorLessThan() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorLessThanOrEqual() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorModulus() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorMultiply() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorRightShift() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorSubtraction() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOperatorUnaryNegation() {
      // not implemented yet
    }
    [TestMethod]
    public void TestOr() {
      // not implemented yet
    }
    [TestMethod]
    public void TestPow() {
      // not implemented yet
    }
    [TestMethod]
    public void TestPowBigIntVar() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRemainder() {
      // not implemented yet
    }
    [TestMethod]
    public void TestShiftLeft() {
      BigInteger bigint = BigInteger.One;
      bigint <<= 100;
      Assert.AreEqual(bigint.shiftLeft(12), bigint.shiftRight(-12));
      Assert.AreEqual(bigint.shiftLeft(-12), bigint.shiftRight(12));
    }
    [TestMethod]
    public void TestShiftRight() {
      BigInteger bigint = BigInteger.One;
      bigint <<= 80;
      Assert.AreEqual(bigint.shiftLeft(12), bigint.shiftRight(-12));
      Assert.AreEqual(bigint.shiftLeft(-12), bigint.shiftRight(12));
    }
    [TestMethod]
    public void TestSign() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSqrt() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSqrtWithRemainder() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSubtract() {
      // not implemented yet
    }
    [TestMethod]
    public void TestTestBit() {
      Assert.IsFalse(BigInteger.Zero.testBit(0));
      Assert.IsFalse(BigInteger.Zero.testBit(1));
      Assert.IsTrue(BigInteger.One.testBit(0));
      Assert.IsFalse(BigInteger.One.testBit(1));
      for (int i = 0; i < 32; ++i) {
       Assert.IsTrue(BigInteger.One.negate().testBit(i));
      }
    }
    [TestMethod]
    public void TestToByteArray() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToString() {
      // not implemented yet
    }
    [TestMethod]
    public void TestValueOf() {
      // not implemented yet
    }
    [TestMethod]
    public void TestXor() {
      // not implemented yet
    }
    [TestMethod]
    public void TestZero() {
      // not implemented yet
    }
  }
}
