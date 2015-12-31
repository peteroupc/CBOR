using System;
using NUnit.Framework;
using PeterO;

namespace Test {
  [TestFixture]
  public class ExtendedRationalTest {
    [Test]
    public void TestConstructor() {
      // not implemented yet
    }
    [Test]
    public void TestAbs() {
      // not implemented yet
    }
    [Test]
    public void TestAdd() {
      // not implemented yet
    }
    [Test]
    public void TestCompareTo() {
      var r = new FastRandom();
      for (var i = 0; i < 500; ++i) {
        ExtendedRational bigintA = RandomObjects.RandomRational(r);
        ExtendedRational bigintB = RandomObjects.RandomRational(r);
        ExtendedRational bigintC = RandomObjects.RandomRational(r);
        TestCommon.CompareTestRelations(bigintA, bigintB, bigintC);
      }
      TestCommon.CompareTestLess(ExtendedRational.Zero, ExtendedRational.NaN);
      for (var i = 0; i < 100; ++i) {
        BigInteger num = RandomObjects.RandomBigInteger(r);
        if (num.IsZero) {
          // Skip if number is 0; 0/1 and 0/2 are
          // equal in that case
          continue;
        }
        num = BigInteger.Abs(num);
        var rat = new ExtendedRational(num, BigInteger.One);
        var rat2 = new ExtendedRational(num, (BigInteger)2);
        TestCommon.CompareTestLess(rat2, rat);
        TestCommon.CompareTestGreater(rat, rat2);
      }
      TestCommon.CompareTestLess(
        new ExtendedRational(BigInteger.One, (BigInteger)2),
        new ExtendedRational((BigInteger)4, BigInteger.One));
      for (var i = 0; i < 100; ++i) {
        BigInteger num = RandomObjects.RandomBigInteger(r);
        BigInteger den = RandomObjects.RandomBigInteger(r);
        if (den.IsZero) {
          den = BigInteger.One;
        }
        var rat = new ExtendedRational(num, den);
        for (int j = 0; j < 10; ++j) {
          BigInteger num2 = num;
          BigInteger den2 = den;
          BigInteger mult = RandomObjects.RandomBigInteger(r);
          if (mult.IsZero || mult.Equals(BigInteger.One)) {
            mult = (BigInteger)2;
          }
          num2 *= (BigInteger)mult;
          den2 *= (BigInteger)mult;
          var rat2 = new ExtendedRational(num2, den2);
          TestCommon.CompareTestEqual(rat, rat2);
        }
      }
    }
    [Test]
    public void TestCompareToBinary() {
      // not implemented yet
    }
    [Test]
    public void TestCompareToDecimal() {
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
    public void TestDenominator() {
      // not implemented yet
    }
    [Test]
    public void TestDivide() {
      var fr = new FastRandom();
      for (var i = 0; i < 500; ++i) {
        ExtendedRational er = RandomObjects.RandomRational(fr);
        ExtendedRational er2 = RandomObjects.RandomRational(fr);
        if (er2.IsZero || !er2.IsFinite) {
          continue;
        }
        if (er.IsZero || !er.IsFinite) {
          continue;
        }
        ExtendedRational ermult = er.Multiply(er2);
        ExtendedRational erdiv = ermult.Divide(er);
        TestCommon.CompareTestEqual(erdiv, er2);
        erdiv = ermult.Divide(er2);
        TestCommon.CompareTestEqual(erdiv, er);
      }
    }
    [Test]
    public void TestEquals() {
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
    public void TestFromExtendedDecimal() {
      // not implemented yet
    }
    [Test]
    public void TestFromExtendedFloat() {
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
    public void TestGetHashCode() {
      // not implemented yet
    }
    [Test]
    public void TestIsFinite() {
      Assert.IsFalse(ExtendedRational.PositiveInfinity.IsFinite);
      Assert.IsFalse(ExtendedRational.NegativeInfinity.IsFinite);
      Assert.IsTrue(ExtendedRational.Zero.IsFinite);
      Assert.IsFalse(ExtendedRational.NaN.IsFinite);
    }
    [Test]
    public void TestIsInfinity() {
      Assert.IsTrue(ExtendedRational.PositiveInfinity.IsInfinity());
      Assert.IsTrue(ExtendedRational.NegativeInfinity.IsInfinity());
      Assert.IsFalse(ExtendedRational.Zero.IsInfinity());
      Assert.IsFalse(ExtendedRational.NaN.IsInfinity());
    }
    [Test]
    public void TestIsNaN() {
      Assert.IsFalse(ExtendedRational.PositiveInfinity.IsNaN());
      Assert.IsFalse(ExtendedRational.NegativeInfinity.IsNaN());
      Assert.IsFalse(ExtendedRational.Zero.IsNaN());
      Assert.IsFalse(ExtendedRational.One.IsNaN());
      Assert.IsTrue(ExtendedRational.NaN.IsNaN());
    }
    [Test]
    public void TestIsNegative() {
      // not implemented yet
    }
    [Test]
    public void TestIsNegativeInfinity() {
      Assert.IsFalse(ExtendedRational.PositiveInfinity.IsNegativeInfinity());
      Assert.IsTrue(ExtendedRational.NegativeInfinity.IsNegativeInfinity());
      Assert.IsFalse(ExtendedRational.Zero.IsNegativeInfinity());
      Assert.IsFalse(ExtendedRational.One.IsNegativeInfinity());
      Assert.IsFalse(ExtendedRational.NaN.IsNegativeInfinity());
    }
    [Test]
    public void TestIsPositiveInfinity() {
      Assert.IsTrue(ExtendedRational.PositiveInfinity.IsPositiveInfinity());
      Assert.IsFalse(ExtendedRational.NegativeInfinity.IsPositiveInfinity());
      Assert.IsFalse(ExtendedRational.Zero.IsPositiveInfinity());
      Assert.IsFalse(ExtendedRational.One.IsPositiveInfinity());
      Assert.IsFalse(ExtendedRational.NaN.IsPositiveInfinity());
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
      Assert.IsTrue(ExtendedRational.NegativeZero.IsZero);
      Assert.IsTrue(ExtendedRational.Zero.IsZero);
      Assert.IsFalse(ExtendedRational.One.IsZero);
      Assert.IsFalse(ExtendedRational.NegativeInfinity.IsZero);
      Assert.IsFalse(ExtendedRational.PositiveInfinity.IsZero);
    }
    [Test]
    public void TestMultiply() {
      // not implemented yet
    }
    [Test]
    public void TestNegate() {
      // not implemented yet
    }
    [Test]
    public void TestNumerator() {
      // not implemented yet
    }
    [Test]
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
    [Test]
    public void TestSign() {
      Assert.AreEqual(0, ExtendedRational.NegativeZero.Sign);
      Assert.AreEqual(0, ExtendedRational.Zero.Sign);
      Assert.AreEqual(1, ExtendedRational.One.Sign);
      Assert.AreEqual(-1, ExtendedRational.NegativeInfinity.Sign);
      Assert.AreEqual(1, ExtendedRational.PositiveInfinity.Sign);
    }
    [Test]
    public void TestSubtract() {
      // not implemented yet
    }
    [Test]
    public void TestToBigInteger() {
      // not implemented yet
    }
    [Test]
    public void TestToBigIntegerExact() {
      try {
        ExtendedRational.PositiveInfinity.ToBigIntegerExact();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedRational.NegativeInfinity.ToBigIntegerExact();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedRational.NaN.ToBigIntegerExact();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedRational.SignalingNaN.ToBigIntegerExact();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestToDouble() {
      // test for correct rounding
      double dbl;
      dbl = ExtendedRational.FromExtendedDecimal(
        ExtendedDecimal.FromString(
       "1.972579273363468721491642554610734805464744567871093749999999999999"))
        .ToDouble();
      {
string stringTemp = ExtendedFloat.FromDouble(dbl).ToPlainString();
Assert.AreEqual(
"1.9725792733634686104693400920950807631015777587890625",
stringTemp);
}
    }
    [Test]
    public void TestToExtendedDecimal() {
      // not implemented yet
    }
    [Test]
    public void TestToExtendedDecimalExactIfPossible() {
      // not implemented yet
    }
    [Test]
    public void TestToExtendedFloat() {
      // not implemented yet
    }
    [Test]
    public void TestToExtendedFloatExactIfPossible() {
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
    public void TestUnsignedNumerator() {
      // not implemented yet
    }
  }
}
