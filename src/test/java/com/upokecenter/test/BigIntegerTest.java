package com.upokecenter.test;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;

  public class BigIntegerTest {
    @Test
    public void TestConstructor() {
      // not implemented yet
    }
    @Test
    public void TestAbs() {
      // not implemented yet
    }
    @Test
    public void TestAdd() {
      // not implemented yet
    }
    @Test
    public void TestAnd() {
      // not implemented yet
    }
    @Test
    public void TestBitLength() {
      Assert.assertEquals(31, BigInteger.valueOf(-2147483647L).bitLength());
      Assert.assertEquals(31, BigInteger.valueOf(-2147483648L).bitLength());
      Assert.assertEquals(32, BigInteger.valueOf(-2147483649L).bitLength());
      Assert.assertEquals(32, BigInteger.valueOf(-2147483650L).bitLength());
      Assert.assertEquals(31, BigInteger.valueOf(2147483647L).bitLength());
      Assert.assertEquals(32, BigInteger.valueOf(2147483648L).bitLength());
      Assert.assertEquals(32, BigInteger.valueOf(2147483649L).bitLength());
      Assert.assertEquals(32, BigInteger.valueOf(2147483650L).bitLength());
      Assert.assertEquals(0, BigInteger.valueOf(0).bitLength());
      Assert.assertEquals(1, BigInteger.valueOf(1).bitLength());
      Assert.assertEquals(2, BigInteger.valueOf(2).bitLength());
      Assert.assertEquals(2, BigInteger.valueOf(2).bitLength());
      Assert.assertEquals(31, BigInteger.valueOf(Integer.MAX_VALUE).bitLength());
      Assert.assertEquals(31, BigInteger.valueOf(Integer.MIN_VALUE).bitLength());
      Assert.assertEquals(16, BigInteger.valueOf(65535).bitLength());
      Assert.assertEquals(16, BigInteger.valueOf(-65535).bitLength());
      Assert.assertEquals(17, BigInteger.valueOf(65536).bitLength());
      Assert.assertEquals(16, BigInteger.valueOf(-65536).bitLength());
      Assert.assertEquals(65, BigInteger.fromString("19084941898444092059").bitLength());
      Assert.assertEquals(65, BigInteger.fromString("-19084941898444092059").bitLength());
      Assert.assertEquals(0, BigInteger.valueOf(-1).bitLength());
      Assert.assertEquals(1, BigInteger.valueOf(-2).bitLength());
    }
    @Test
    public void TestCanFitInInt() {
      // not implemented yet
    }
    @Test
    public void TestCompareTo() {
      // not implemented yet
    }
    @Test
    public void TestDivide() {
      // not implemented yet
    }
    @Test
    public void TestDivideAndRemainder() {
      try {
        BigInteger.ONE.divideAndRemainder(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestDivRem() {
      // not implemented yet
    }
    @Test
    public void TestEquals() {
      // not implemented yet
    }
    @Test
    public void TestFromByteArray() {
      // not implemented yet
    }
    @Test
    public void TestFromString() {
      // not implemented yet
    }
    @Test
    public void TestFromSubstring() {
      // not implemented yet
    }
    @Test
    public void TestGcd() {
      // not implemented yet
    }
    @Test
    public void TestGetBits() {
      // not implemented yet
    }
    @Test
    public void TestGetDigitCount() {
      // not implemented yet
    }
    @Test
    public void TestGetHashCode() {
      // not implemented yet
    }
    @Test
    public void TestGetLowestSetBit() {
      // not implemented yet
    }
    @Test
    public void TestGetUnsignedBitLength() {
      // not implemented yet
    }
    @Test
    public void TestGreatestCommonDivisor() {
      // not implemented yet
    }
    @Test
    public void TestIntValue() {
      // not implemented yet
    }
    @Test
    public void TestIntValueChecked() {
      // not implemented yet
    }
    @Test
    public void TestIntValueUnchecked() {
      // not implemented yet
    }
    @Test
    public void TestIsEven() {
      // not implemented yet
    }
    @Test
    public void TestIsPowerOfTwo() {
      // not implemented yet
    }
    @Test
    public void TestIsZero() {
      // not implemented yet
    }
    @Test
    public void TestLongValue() {
      // not implemented yet
    }
    @Test
    public void TestLongValueChecked() {
      // not implemented yet
    }
    @Test
    public void TestLongValueUnchecked() {
      // not implemented yet
    }
    @Test
    public void TestMod() {
      try {
        BigInteger.ONE.mod(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestModPow() {
      // not implemented yet
    }
    @Test
    public void TestMultiply() {
      // not implemented yet
    }
    @Test
    public void TestNegate() {
      // not implemented yet
    }
    @Test
    public void TestNot() {
      // not implemented yet
    }
    @Test
    public void TestOne() {
      // not implemented yet
    }
    @Test
    public void TestOperatorAddition() {
      // not implemented yet
    }
    @Test
    public void TestOperatorDivision() {
      // not implemented yet
    }
    @Test
    public void TestOperatorExplicit() {
      // not implemented yet
    }
    @Test
    public void TestOperatorGreaterThan() {
      // not implemented yet
    }
    @Test
    public void TestOperatorGreaterThanOrEqual() {
      // not implemented yet
    }
    @Test
    public void TestOperatorImplicit() {
      // not implemented yet
    }
    @Test
    public void TestOperatorLeftShift() {
      // not implemented yet
    }
    @Test
    public void TestOperatorLessThan() {
      // not implemented yet
    }
    @Test
    public void TestOperatorLessThanOrEqual() {
      // not implemented yet
    }
    @Test
    public void TestOperatorModulus() {
      // not implemented yet
    }
    @Test
    public void TestOperatorMultiply() {
      // not implemented yet
    }
    @Test
    public void TestOperatorRightShift() {
      // not implemented yet
    }
    @Test
    public void TestOperatorSubtraction() {
      // not implemented yet
    }
    @Test
    public void TestOperatorUnaryNegation() {
      // not implemented yet
    }
    @Test
    public void TestOr() {
      // not implemented yet
    }
    @Test
    public void TestPow() {
      // not implemented yet
    }
    @Test
    public void TestPowBigIntVar() {
      // not implemented yet
    }
    @Test
    public void TestRemainder() {
      // not implemented yet
    }
    @Test
    public void TestShiftLeft() {
      BigInteger bigint = BigInteger.ONE;
      bigint=bigint.shiftLeft(100);
      Assert.assertEquals(bigint.shiftLeft(12), bigint.shiftRight(-12));
      Assert.assertEquals(bigint.shiftLeft(-12), bigint.shiftRight(12));
    }
    @Test
    public void TestShiftRight() {
      BigInteger bigint = BigInteger.ONE;
      bigint=bigint.shiftLeft(80);
      Assert.assertEquals(bigint.shiftLeft(12), bigint.shiftRight(-12));
      Assert.assertEquals(bigint.shiftLeft(-12), bigint.shiftRight(12));
    }
    @Test
    public void TestSign() {
      // not implemented yet
    }
    @Test
    public void TestSqrt() {
      // not implemented yet
    }
    @Test
    public void TestSqrtWithRemainder() {
      // not implemented yet
    }
    @Test
    public void TestSubtract() {
      // not implemented yet
    }
    @Test
    public void TestTestBit() {
      if(BigInteger.ZERO.testBit(0))Assert.fail();
      if(BigInteger.ZERO.testBit(1))Assert.fail();
      if(!(BigInteger.ONE.testBit(0)))Assert.fail();
      if(BigInteger.ONE.testBit(1))Assert.fail();
      for (int i = 0; i < 32; ++i) {
       if(!(BigInteger.ONE.negate().testBit(i)))Assert.fail();
      }
    }
    @Test
    public void TestToByteArray() {
      // not implemented yet
    }
    @Test
    public void TestToString() {
      // not implemented yet
    }
    @Test
    public void TestValueOf() {
      // not implemented yet
    }
    @Test
    public void TestXor() {
      // not implemented yet
    }
    @Test
    public void TestZero() {
      // not implemented yet
    }
  }
