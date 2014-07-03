using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO;

namespace Test {
  [TestClass]
  public class BigIntegerTest {
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
Assert.AreEqual(65, BigInteger.fromString("19084941898444092059"
).bitLength());
      Assert.AreEqual(65, BigInteger.fromString("-19084941898444092059"
).bitLength());
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
      Assert.AreEqual(BigInteger.Zero, BigInteger.fromByteArray(new byte[] {
        }, false));
    }
    [TestMethod]
    public void TestFromString() {
      try {
        BigInteger.fromString(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestFromSubstring() {
      try {
        BigInteger.fromSubstring(null, 0, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    public static int ModPow(int x, int pow, int intMod) {
      if (x < 0) {
        throw new ArgumentException("x (" + Convert.ToString((int)x,
          System.Globalization.CultureInfo.InvariantCulture) +
          ") is less than " + "0");
      }
      if (pow <= 0) {
        throw new ArgumentException("pow (" + Convert.ToString((int)pow,
          System.Globalization.CultureInfo.InvariantCulture) +
          ") is not greater than " + "0");
      }
      if (intMod <= 0) {
        throw new ArgumentException("mod (" + Convert.ToString((int)intMod,
          System.Globalization.CultureInfo.InvariantCulture) +
          ") is not greater than " + "0");
      }
      int r = 1;
      int v = x;
      while (pow != 0) {
        if ((pow & 1) != 0) {
          r = (int)(((long)r * (long)v) % intMod);
        }
        pow >>= 1;
        if (pow != 0) {
          v = (int)(((long)v * (long)v) % intMod);
        }
      }
      return r;
    }

    public static bool IsPrime(int n) {
      // Use a deterministic Rabin-Miller test
      if (n < 2) {
        return false;
      }
      if (n == 2) {
        return true;
      }
      if (n % 2 == 0) {
        return false;
      }
      int d = n - 1;
      while ((d & 1) == 0) {
        d >>= 1;
      }
      int mp = 0;
      // For all 32-bit integers it's enough
      // to check the strong pseudoprime
      // bases 2, 7, and 61
      if (n > 2) {
        mp = ModPow(2, d, n);
        if (mp != 1 && mp + 1 != n) {
          return false;
        }
      }
      if (n > 7) {
        mp = ModPow(7, d, n);
        if (mp != 1 && mp + 1 != n) {
          return false;
        }
      }
      if (n > 61) {
        mp = ModPow(61, d, n);
        if (mp != 1 && mp + 1 != n) {
          return false;
        }
      }
      return true;
    }

    [TestMethod]
    public void TestGcd() {
      int prime = 0;
      var rand = new FastRandom();
      for (var i = 0; i < 1000; ++i) {
        while (true) {
          prime = rand.NextValue(0x7fffffff);
          prime |= 1;
          if (IsPrime(prime)) {
            break;
          }
        }
        var bigprime = (BigInteger)prime;
        BigInteger ba = CBORTest.RandomBigInteger(rand);
        if (ba.IsZero) {
          continue;
        }
        ba *= (BigInteger)bigprime;
     Assert.AreEqual(bigprime, BigInteger.GreatestCommonDivisor(bigprime,
          ba));
      }
    }

    [TestMethod]
    public void TestGetBits() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetDigitCount() {
      var r = new FastRandom();
      for (var i = 0; i < 1000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        String str = BigInteger.Abs(bigintA).ToString();
        Assert.AreEqual(str.Length, bigintA.getDigitCount());
      }
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
Assert.AreEqual(Int32.MinValue,
        BigInteger.valueOf(Int32.MinValue).intValue());
Assert.AreEqual(Int32.MaxValue,
        BigInteger.valueOf(Int32.MaxValue).intValue());
      try {
        BigInteger.valueOf(Int32.MinValue - 1L).intValue();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.valueOf(Int32.MaxValue + 1L).intValue();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestIntValueChecked() {
      Assert.AreEqual(Int32.MinValue,
        BigInteger.valueOf(Int32.MinValue).intValueChecked());
      Assert.AreEqual(Int32.MaxValue,
        BigInteger.valueOf(Int32.MaxValue).intValueChecked());
      try {
        BigInteger.valueOf(Int32.MinValue - 1L).intValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.valueOf(Int32.MaxValue + 1L).intValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestIntValueUnchecked() {
      Assert.AreEqual(Int32.MinValue,
        BigInteger.valueOf(Int32.MinValue).intValueUnchecked());
      Assert.AreEqual(Int32.MaxValue,
        BigInteger.valueOf(Int32.MaxValue).intValueUnchecked());
      Assert.AreEqual(Int32.MaxValue, BigInteger.valueOf(Int32.MinValue -
        1L).intValueUnchecked());
      Assert.AreEqual(Int32.MinValue, BigInteger.valueOf(Int32.MaxValue +
        1L).intValueUnchecked());
    }
    [TestMethod]
    public void TestIsEven() {
      var r = new FastRandom();
      for (var i = 0; i < 1000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        BigInteger mod = bigintA.remainder(BigInteger.valueOf(2));
        Assert.AreEqual(mod.IsZero, bigintA.IsEven);
      }
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
      Assert.AreEqual(Int64.MinValue,
        BigInteger.valueOf(Int64.MinValue).longValue());
      Assert.AreEqual(Int64.MaxValue,
        BigInteger.valueOf(Int64.MaxValue).longValue());
      try {
        BigInteger.valueOf(Int64.MinValue).subtract(BigInteger.One).longValue();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.valueOf(Int64.MaxValue).add(BigInteger.One).longValue();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(unchecked((long)0xFFFFFFF200000000L),
  BigInteger.valueOf(unchecked((long)0xFFFFFFF200000000L)).longValue());
      Assert.AreEqual(unchecked((long)0xFFFFFFF280000000L),
  BigInteger.valueOf(unchecked((long)0xFFFFFFF280000000L)).longValue());
      Assert.AreEqual(unchecked((long)0xFFFFFFF280000001L),
  BigInteger.valueOf(unchecked((long)0xFFFFFFF280000001L)).longValue());
      Assert.AreEqual(unchecked((long)0xFFFFFFF27FFFFFFFL),
  BigInteger.valueOf(unchecked((long)0xFFFFFFF27FFFFFFFL)).longValue());
      Assert.AreEqual(0x0000000380000001L,
        BigInteger.valueOf(0x0000000380000001L).longValue());
      Assert.AreEqual(0x0000000382222222L,
        BigInteger.valueOf(0x0000000382222222L).longValue());
      Assert.AreEqual(-8L, BigInteger.valueOf(-8L).longValue());
      Assert.AreEqual(-32768L, BigInteger.valueOf(-32768L).longValue());
      Assert.AreEqual(Int32.MinValue,
        BigInteger.valueOf(Int32.MinValue).longValue());
      Assert.AreEqual(Int32.MaxValue,
        BigInteger.valueOf(Int32.MaxValue).longValue());
      Assert.AreEqual(0x80000000L, BigInteger.valueOf(0x80000000L).longValue());
      Assert.AreEqual(0x90000000L, BigInteger.valueOf(0x90000000L).longValue());
    }
    [TestMethod]
    public void TestLongValueChecked() {
      Assert.AreEqual(Int64.MinValue,
        BigInteger.valueOf(Int64.MinValue).longValueChecked());
      Assert.AreEqual(Int64.MaxValue,
        BigInteger.valueOf(Int64.MaxValue).longValueChecked());
      try {
BigInteger.valueOf(Int64.MinValue) .subtract(BigInteger.One)
          .longValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
BigInteger.valueOf(Int64.MaxValue) .add(BigInteger.One) .longValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(unchecked((long)0xFFFFFFF200000000L),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF200000000L))
        .longValueChecked());
      Assert.AreEqual(unchecked((long)0xFFFFFFF280000000L),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF280000000L))
        .longValueChecked());
      Assert.AreEqual(unchecked((long)0xFFFFFFF280000001L),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF280000001L))
        .longValueChecked());
      Assert.AreEqual(unchecked((long)0xFFFFFFF27FFFFFFFL),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF27FFFFFFFL))
        .longValueChecked());
      Assert.AreEqual(0x0000000380000001L,
        BigInteger.valueOf(0x0000000380000001L).longValueChecked());
      Assert.AreEqual(0x0000000382222222L,
        BigInteger.valueOf(0x0000000382222222L).longValueChecked());
      Assert.AreEqual(-8L, BigInteger.valueOf(-8L).longValueChecked());
      Assert.AreEqual(-32768L, BigInteger.valueOf(-32768L).longValueChecked());
      Assert.AreEqual(Int32.MinValue,
        BigInteger.valueOf(Int32.MinValue).longValueChecked());
      Assert.AreEqual(Int32.MaxValue,
        BigInteger.valueOf(Int32.MaxValue).longValueChecked());
      Assert.AreEqual(0x80000000L,
        BigInteger.valueOf(0x80000000L).longValueChecked());
      Assert.AreEqual(0x90000000L,
        BigInteger.valueOf(0x90000000L).longValueChecked());
    }
    [TestMethod]
    public void TestLongValueUnchecked() {
      Assert.AreEqual(Int64.MinValue,
        BigInteger.valueOf(Int64.MinValue).longValueUnchecked());
      Assert.AreEqual(Int64.MaxValue,
        BigInteger.valueOf(Int64.MaxValue).longValueUnchecked());
      Assert.AreEqual(Int64.MaxValue, BigInteger.valueOf(Int64.MinValue)
        .subtract(BigInteger.One) .longValueUnchecked());
      Assert.AreEqual(Int64.MinValue, BigInteger.valueOf(Int64.MaxValue)
        .add(BigInteger.One) .longValueUnchecked());
      Assert.AreEqual(unchecked((long)0xFFFFFFF200000000L),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF200000000L))
        .longValueUnchecked());
      Assert.AreEqual(unchecked((long)0xFFFFFFF280000000L),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF280000000L))
        .longValueUnchecked());
      Assert.AreEqual(unchecked((long)0xFFFFFFF280000001L),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF280000001L))
        .longValueUnchecked());
      Assert.AreEqual(unchecked((long)0xFFFFFFF27FFFFFFFL),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF27FFFFFFFL))
        .longValueUnchecked());
      Assert.AreEqual(0x0000000380000001L,
        BigInteger.valueOf(0x0000000380000001L).longValueUnchecked());
      Assert.AreEqual(0x0000000382222222L,
        BigInteger.valueOf(0x0000000382222222L).longValueUnchecked());
      Assert.AreEqual(-8L, BigInteger.valueOf(-8L).longValueUnchecked());
    Assert.AreEqual(-32768L,
        BigInteger.valueOf(-32768L).longValueUnchecked());
      Assert.AreEqual(Int32.MinValue,
        BigInteger.valueOf(Int32.MinValue).longValueUnchecked());
      Assert.AreEqual(Int32.MaxValue,
        BigInteger.valueOf(Int32.MaxValue).longValueUnchecked());
      Assert.AreEqual(0x80000000L,
        BigInteger.valueOf(0x80000000L).longValueUnchecked());
      Assert.AreEqual(0x90000000L,
        BigInteger.valueOf(0x90000000L).longValueUnchecked());
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
