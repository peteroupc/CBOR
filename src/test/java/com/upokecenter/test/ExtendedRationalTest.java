package com.upokecenter.test;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;

  public class ExtendedRationalTest {
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
    public void TestCompareTo() {
      // not implemented yet
    }
    @Test
    public void TestCompareToBinary() {
      // not implemented yet
    }
    @Test
    public void TestCompareToDecimal() {
      // not implemented yet
    }
    @Test
    public void TestCreate() {
      // not implemented yet
    }
    @Test
    public void TestCreateNaN() {
      // not implemented yet
    }
    @Test
    public void TestDenominator() {
      // not implemented yet
    }
    @Test
    public void TestDivide() {
      // not implemented yet
    }
    @Test
    public void TestEquals() {
      // not implemented yet
    }
    @Test
    public void TestFromBigInteger() {
      // not implemented yet
    }
    @Test
    public void TestFromDouble() {
      // not implemented yet
    }
    @Test
    public void TestFromExtendedDecimal() {
      // not implemented yet
    }
    @Test
    public void TestFromExtendedFloat() {
      // not implemented yet
    }
    @Test
    public void TestFromInt32() {
      // not implemented yet
    }
    @Test
    public void TestFromInt64() {
      // not implemented yet
    }
    @Test
    public void TestFromSingle() {
      // not implemented yet
    }
    @Test
    public void TestGetHashCode() {
      // not implemented yet
    }
    @Test
    public void TestIsFinite() {
      // not implemented yet
    }
    @Test
    public void TestIsInfinity() {
      if (!(ExtendedRational.PositiveInfinity.IsInfinity()))Assert.fail();
      if (!(ExtendedRational.NegativeInfinity.IsInfinity()))Assert.fail();
      if (ExtendedRational.Zero.IsInfinity())Assert.fail();
      if (ExtendedRational.NaN.IsInfinity())Assert.fail();
    }
    @Test
    public void TestIsNaN() {
      if (ExtendedRational.PositiveInfinity.IsNaN())Assert.fail();
      if (ExtendedRational.NegativeInfinity.IsNaN())Assert.fail();
      if (ExtendedRational.Zero.IsNaN())Assert.fail();
      if (!(ExtendedRational.NaN.IsNaN()))Assert.fail();
    }
    @Test
    public void TestIsNegative() {
      // not implemented yet
    }
    @Test
    public void TestIsNegativeInfinity() {
      // not implemented yet
    }
    @Test
    public void TestIsPositiveInfinity() {
      // not implemented yet
    }
    @Test
    public void TestIsQuietNaN() {
      // not implemented yet
    }
    @Test
    public void TestIsSignalingNaN() {
      // not implemented yet
    }
    @Test
    public void TestIsZero() {
      if (!(ExtendedRational.NegativeZero.signum() == 0))Assert.fail();
      if (!(ExtendedRational.Zero.signum() == 0))Assert.fail();
      if (ExtendedRational.One.signum() == 0)Assert.fail();
      if (ExtendedRational.NegativeInfinity.signum() == 0)Assert.fail();
      if (ExtendedRational.PositiveInfinity.signum() == 0)Assert.fail();
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
    public void TestNumerator() {
      // not implemented yet
    }
    @Test
    public void TestRemainder() {
      // not implemented yet
    }
    @Test
    public void TestSign() {
      Assert.assertEquals(0, ExtendedRational.NegativeZero.signum());
      Assert.assertEquals(0, ExtendedRational.Zero.signum());
      Assert.assertEquals(1, ExtendedRational.One.signum());
      Assert.assertEquals(-1, ExtendedRational.NegativeInfinity.signum());
      Assert.assertEquals(1, ExtendedRational.PositiveInfinity.signum());
    }
    @Test
    public void TestSubtract() {
      // not implemented yet
    }
    @Test
    public void TestToBigInteger() {
      // not implemented yet
    }
    @Test
    public void TestToBigIntegerExact() {
      // not implemented yet
    }
    @Test
    public void TestToDouble() {
      // not implemented yet
    }
    @Test
    public void TestToExtendedDecimal() {
      // not implemented yet
    }
    @Test
    public void TestToExtendedDecimalExactIfPossible() {
      // not implemented yet
    }
    @Test
    public void TestToExtendedFloat() {
      // not implemented yet
    }
    @Test
    public void TestToExtendedFloatExactIfPossible() {
      // not implemented yet
    }
    @Test
    public void TestToSingle() {
      // not implemented yet
    }
    @Test
    public void TestToString() {
      // not implemented yet
    }
    @Test
    public void TestUnsignedNumerator() {
      // not implemented yet
    }
  }
