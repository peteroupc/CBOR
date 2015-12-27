using System;
using NUnit.Framework;
using PeterO;

namespace Test {
[TestFixture]
  public class ExtendedFloatTest {
    [Test]
    public void TestMovePointRight() {
      ExtendedFloat ef;
      ExtendedFloat ef2;
      ef = ExtendedFloat.FromInt32(0x100).MovePointRight(4);
      ef2 = ExtendedFloat.FromInt32(0x1000);
      Assert.AreEqual(0, ef.CompareTo(ef2));
    }
    [Test]
    public void TestMovePointLeft() {
      ExtendedFloat ef;
      ExtendedFloat ef2;
      ef = ExtendedFloat.FromInt32(0x150).MovePointLeft(4);
      ef2 = ExtendedFloat.FromInt32(0x15);
      Assert.AreEqual(0, ef.CompareTo(ef2));
    }

    [Test]
    public void TestFloatDecimalRoundTrip() {
      var r = new FastRandom();
      for (var i = 0; i < 5000; ++i) {
        ExtendedFloat ef = RandomObjects.RandomExtendedFloat(r);
        ExtendedDecimal ed = ef.ToExtendedDecimal();
        ExtendedFloat ef2 = ed.ToExtendedFloat();
        // Tests that values converted from float to decimal and
        // back have the same numerical value
        TestCommon.CompareTestEqual(ef, ef2);
      }
    }

    public static ExtendedFloat FromBinary(string str) {
      var smallExponent = 0;
      var index = 0;
      BigInteger ret = BigInteger.Zero;
      while (index < str.Length) {
        if (str[index] == '0') {
          ++index;
        } else {
          break;
        }
      }
      while (index < str.Length) {
        if (str[index] == '.') {
          ++index;
          break;
        }
        if (str[index] == '1') {
          ++index;
          if (ret.IsZero) {
            ret = BigInteger.One;
          } else {
            ret <<= 1;
            ret += BigInteger.One;
          }
        } else if (str[index] == '0') {
          ++index;
          ret <<= 1;
          continue;
        } else {
          break;
        }
      }
      while (index < str.Length) {
        if (str[index] == '1') {
          ++index;
          --smallExponent;
          if (ret.IsZero) {
            ret = BigInteger.One;
          } else {
            ret <<= 1;
            ret += BigInteger.One;
          }
        } else if (str[index] == '0') {
          ++index;
          --smallExponent;
          ret <<= 1;
          continue;
        } else {
          break;
        }
      }
      return ExtendedFloat.Create(ret, (BigInteger)smallExponent);
    }

    [Test]
    public void TestAbs() {
      // not implemented yet
    }
    [Test]
    public void TestAdd() {
      try {
        ExtendedFloat.Zero.Add(null, PrecisionContext.Unlimited);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestCompareTo() {
      var r = new FastRandom();
      for (var i = 0; i < 500; ++i) {
        ExtendedFloat bigintA = RandomObjects.RandomExtendedFloat(r);
        ExtendedFloat bigintB = RandomObjects.RandomExtendedFloat(r);
        ExtendedFloat bigintC = RandomObjects.RandomExtendedFloat(r);
        TestCommon.CompareTestRelations(bigintA, bigintB, bigintC);
      }
      TestCommon.CompareTestLess(ExtendedFloat.Zero, ExtendedFloat.NaN);
      ExtendedDecimal a = ExtendedDecimal.FromString(
        "7.00468923842476447758037175245551511770928808756622205663208" + "4784688080253355047487262563521426272927783429622650146484375");
      ExtendedDecimal b = ExtendedDecimal.FromString("5");
      TestCommon.CompareTestLess(b, a);
    }
    [Test]
    public void TestCompareToSignal() {
      // not implemented yet
    }
    [Test]
    public void TestCompareToWithContext() {
      // not implemented yet
    }
    [Test]
    public void TestCreate() {
      // not implemented yet
    }
    [Test]
    public void TestCreateNaN() {
      // not implemented yet
    }
    [Test]
    public void TestDivide() {
      try {
 ExtendedDecimal.FromString("1").Divide(ExtendedDecimal.FromString("3"), null);
} catch (Exception ex) {
Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
    }
    [Test]
    public void TestDivideToExponent() {
      // not implemented yet
    }
    [Test]
    public void TestDivideToIntegerNaturalScale() {
      // not implemented yet
    }
    [Test]
    public void TestDivideToIntegerZeroScale() {
      // not implemented yet
    }
    [Test]
    public void TestDivideToSameExponent() {
      // not implemented yet
    }
    [Test]
    public void TestEquals() {
      var r = new FastRandom();
      for (var i = 0; i < 500; ++i) {
        ExtendedFloat bigintA = RandomObjects.RandomExtendedFloat(r);
        ExtendedFloat bigintB = RandomObjects.RandomExtendedFloat(r);
        TestCommon.AssertEqualsHashCode(bigintA, bigintB);
      }
    }
    [Test]
    public void TestEqualsInternal() {
      // not implemented yet
    }
    [Test]
    public void TestExp() {
      // not implemented yet
    }
    [Test]
    public void TestExponent() {
      // not implemented yet
    }
    [Test]
    public void TestFromBigInteger() {
      // not implemented yet
    }
    [Test]
    public void TestFromDouble() {
      // not implemented yet
    }
    [Test]
    public void TestFromInt32() {
      // not implemented yet
    }
    [Test]
    public void TestFromInt64() {
      // not implemented yet
    }
    [Test]
    public void TestFromSingle() {
      // not implemented yet
    }
    [Test]
    public void TestFromString() {
      try {
 ExtendedFloat.FromString("2", 0, 1, null);
} catch (Exception ex) {
Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
        ExtendedFloat.FromString(String.Empty);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestIsFinite() {
      // not implemented yet
    }

    [Test]
    public void TestIsInfinity() {
      Assert.IsTrue(ExtendedFloat.PositiveInfinity.IsInfinity());
      Assert.IsTrue(ExtendedFloat.NegativeInfinity.IsInfinity());
      Assert.IsFalse(ExtendedFloat.Zero.IsInfinity());
      Assert.IsFalse(ExtendedFloat.NaN.IsInfinity());
    }
    [Test]
    public void TestIsNaN() {
      Assert.IsFalse(ExtendedFloat.PositiveInfinity.IsNaN());
      Assert.IsFalse(ExtendedFloat.NegativeInfinity.IsNaN());
      Assert.IsFalse(ExtendedFloat.Zero.IsNaN());
      Assert.IsTrue(ExtendedFloat.NaN.IsNaN());
    }
    [Test]
    public void TestIsNegative() {
      // not implemented yet
    }
    [Test]
    public void TestIsNegativeInfinity() {
      // not implemented yet
    }
    [Test]
    public void TestIsPositiveInfinity() {
      // not implemented yet
    }
    [Test]
    public void TestIsQuietNaN() {
      // not implemented yet
    }
    [Test]
    public void TestIsSignalingNaN() {
      // not implemented yet
    }
    [Test]
    public void TestIsZero() {
      // not implemented yet
    }
    [Test]
    public void TestLog() {
      Assert.IsTrue(ExtendedFloat.One.Log(null).IsNaN());
      Assert.IsTrue(ExtendedFloat.One.Log(PrecisionContext.Unlimited).IsNaN());
    }
    [Test]
    public void TestLog10() {
      Assert.IsTrue(ExtendedFloat.One.Log10(null).IsNaN());
      Assert.IsTrue(ExtendedFloat.One.Log10(PrecisionContext.Unlimited)
              .IsNaN());
    }
    [Test]
    public void TestMantissa() {
      // not implemented yet
    }
    [Test]
    public void TestMax() {
      try {
        ExtendedFloat.Max(null, ExtendedFloat.One);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.Max(ExtendedFloat.One, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      var r = new FastRandom();
      for (var i = 0; i < 500; ++i) {
        ExtendedFloat bigintA = RandomObjects.RandomExtendedFloat(r);
        ExtendedFloat bigintB = RandomObjects.RandomExtendedFloat(r);
        if (!bigintA.IsFinite || !bigintB.IsFinite) {
          continue;
        }
        int cmp = TestCommon.CompareTestReciprocal(bigintA, bigintB);
        if (cmp < 0) {
     TestCommon.CompareTestEqual(bigintB, ExtendedFloat.Max(bigintA,
            bigintB));
        } else if (cmp > 0) {
     TestCommon.CompareTestEqual(bigintA, ExtendedFloat.Max(bigintA,
            bigintB));
        } else {
     TestCommon.CompareTestEqual(bigintA, ExtendedFloat.Max(bigintA,
            bigintB));
     TestCommon.CompareTestEqual(bigintB, ExtendedFloat.Max(bigintA,
            bigintB));
        }
      }
    }
    [Test]
    public void TestMaxMagnitude() {
      try {
        ExtendedFloat.MaxMagnitude(null, ExtendedFloat.One);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.MaxMagnitude(ExtendedFloat.One, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestMin() {
      try {
        ExtendedFloat.Min(null, ExtendedFloat.One);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.Min(ExtendedFloat.One, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      var r = new FastRandom();
      for (var i = 0; i < 500; ++i) {
        ExtendedFloat bigintA = RandomObjects.RandomExtendedFloat(r);
        ExtendedFloat bigintB = RandomObjects.RandomExtendedFloat(r);
        if (!bigintA.IsFinite || !bigintB.IsFinite) {
          continue;
        }
        int cmp = TestCommon.CompareTestReciprocal(bigintA, bigintB);
        if (cmp < 0) {
     TestCommon.CompareTestEqual(bigintA, ExtendedFloat.Min(bigintA,
            bigintB));
        } else if (cmp > 0) {
     TestCommon.CompareTestEqual(bigintB, ExtendedFloat.Min(bigintA,
            bigintB));
        } else {
     TestCommon.CompareTestEqual(bigintA, ExtendedFloat.Min(bigintA,
            bigintB));
     TestCommon.CompareTestEqual(bigintB, ExtendedFloat.Min(bigintA,
            bigintB));
        }
      }
    }
    [Test]
    public void TestMinMagnitude() {
      try {
        ExtendedFloat.MinMagnitude(null, ExtendedFloat.One);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.MinMagnitude(ExtendedFloat.One, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestMultiply() {
      // not implemented yet
    }
    [Test]
    public void TestMultiplyAndAdd() {
      // not implemented yet
    }
    [Test]
    public void TestMultiplyAndSubtract() {
      // not implemented yet
    }
    [Test]
    public void TestNegate() {
      // not implemented yet
    }
    [Test]
    public void TestNextMinus() {
      // not implemented yet
    }
    [Test]
    public void TestNextPlus() {
      // not implemented yet
    }
    [Test]
    public void TestNextToward() {
      // not implemented yet
    }
    [Test]
    public void TestPI() {
      // not implemented yet
    }
    [Test]
    public void TestPlus() {
      // not implemented yet
    }
    [Test]
    public void TestPow() {
      // not implemented yet
    }
    [Test]
    public void TestQuantize() {
      // not implemented yet
    }
    [Test]
    public void TestReduce() {
      // not implemented yet
    }
    [Test]
    public void TestRemainder() {
      // not implemented yet
    }
    [Test]
    public void TestRemainderNaturalScale() {
      // not implemented yet
    }
    [Test]
    public void TestRemainderNear() {
      // not implemented yet
    }
    [Test]
    public void TestRoundToBinaryPrecision() {
      // not implemented yet
    }
    [Test]
    public void TestRoundToExponent() {
      // not implemented yet
    }
    [Test]
    public void TestRoundToExponentExact() {
      // not implemented yet
    }
    [Test]
    public void TestRoundToIntegralExact() {
      // not implemented yet
    }
    [Test]
    public void TestRoundToIntegralNoRoundedFlag() {
      // not implemented yet
    }
    [Test]
    public void TestRoundToPrecision() {
      // not implemented yet
    }
    [Test]
    public void TestSign() {
      // not implemented yet
    }
    [Test]
    public void TestSquareRoot() {
      // not implemented yet
    }
    [Test]
    public void TestSubtract() {
      try {
        ExtendedFloat.Zero.Subtract(null, PrecisionContext.Unlimited);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestToBigInteger() {
      try {
        ExtendedFloat.PositiveInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.NegativeInfinity.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.NaN.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedFloat.SignalingNaN.ToBigInteger();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestToBigIntegerExact() {
      // not implemented yet
    }
    [Test]
    public void TestToDouble() {
      // not implemented yet
    }
    [Test]
    public void TestToEngineeringString() {
      // not implemented yet
    }
    [Test]
    public void TestToExtendedDecimal() {
      // not implemented yet
    }
    [Test]
    public void TestToPlainString() {
      // not implemented yet
    }
    [Test]
    public void TestToSingle() {
      // not implemented yet
    }
    [Test]
    public void TestToString() {
      // not implemented yet
    }
    [Test]
    public void TestUnsignedMantissa() {
      // not implemented yet
    }
  }
}
