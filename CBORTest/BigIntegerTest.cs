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
      Assert.AreEqual(
        65,
        BigInteger.fromString("19084941898444092059").bitLength());
      Assert.AreEqual(
        65,
        BigInteger.fromString("-19084941898444092059").bitLength());
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
      Assert.AreEqual(
        BigInteger.Zero,
        BigInteger.fromByteArray(new byte[] { }, false));
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
        throw new ArgumentException(
          "x (" + x + ") is less than " + "0");
      }
      if (pow <= 0) {
        throw new ArgumentException(
          "pow (" + pow + ") is not greater than " + "0");
      }
      if (intMod <= 0) {
        throw new ArgumentException(
          "mod (" + intMod + ") is not greater than " + "0");
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
      if (n < 2) {
        return false;
      }
      if (n == 2) {
        return true;
      }
      if (n % 2 == 0) {
        return false;
      }
      if(n<=23){
        return (n == 3 || n == 5 || n == 7 || n == 11 ||
                n == 13 || n == 17 || n == 19 || n == 23);
      }
      // Use a deterministic Rabin-Miller test
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
    
    private static void TestGcdPair(BigInteger biga, BigInteger bigb, BigInteger biggcd){
      BigInteger ba=BigInteger.GreatestCommonDivisor(biga,bigb);
      BigInteger bb=BigInteger.GreatestCommonDivisor(bigb,biga);
      Assert.AreEqual(ba,biggcd);
      Assert.AreEqual(bb,biggcd);
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
        BigInteger ba = RandomObjects.RandomBigInteger(rand);
        if (ba.IsZero) {
          continue;
        }
        ba *= (BigInteger)bigprime;
        Assert.AreEqual(
          bigprime,
          BigInteger.GreatestCommonDivisor(bigprime, ba));
      }
      TestGcdPair((BigInteger)(-1867), (BigInteger)(-4456), (BigInteger)(1));
      TestGcdPair((BigInteger)(4604), (BigInteger)(-4516), (BigInteger)(4));
      TestGcdPair((BigInteger)(-1756), (BigInteger)(4525), (BigInteger)(1));
      TestGcdPair((BigInteger)(1568), (BigInteger)(-4955), (BigInteger)(1));
      TestGcdPair((BigInteger)(2519), (BigInteger)(2845), (BigInteger)(1));
      TestGcdPair((BigInteger)(-1470), (BigInteger)(132), (BigInteger)(6));
      TestGcdPair((BigInteger)(-2982), (BigInteger)(2573), (BigInteger)(1));
      TestGcdPair((BigInteger)(-244), (BigInteger)(-3929), (BigInteger)(1));
      TestGcdPair((BigInteger)(-3794), (BigInteger)(-2325), (BigInteger)(1));
      TestGcdPair((BigInteger)(-2667), (BigInteger)(2123), (BigInteger)(1));
      TestGcdPair((BigInteger)(-3712), (BigInteger)(-1850), (BigInteger)(2));
      TestGcdPair((BigInteger)(2329), (BigInteger)(3874), (BigInteger)(1));
      TestGcdPair((BigInteger)(1384), (BigInteger)(-4278), (BigInteger)(2));
      TestGcdPair((BigInteger)(213), (BigInteger)(-1217), (BigInteger)(1));
      TestGcdPair((BigInteger)(1163), (BigInteger)(2819), (BigInteger)(1));
      TestGcdPair((BigInteger)(1921), (BigInteger)(-579), (BigInteger)(1));
      TestGcdPair((BigInteger)(3886), (BigInteger)(-13), (BigInteger)(1));
      TestGcdPair((BigInteger)(-3270), (BigInteger)(-3760), (BigInteger)(10));
      TestGcdPair((BigInteger)(-3528), (BigInteger)(1822), (BigInteger)(2));
      TestGcdPair((BigInteger)(1547), (BigInteger)(-333), (BigInteger)(1));
      TestGcdPair((BigInteger)(2402), (BigInteger)(2850), (BigInteger)(2));
      TestGcdPair((BigInteger)(4519), (BigInteger)(1296), (BigInteger)(1));
      TestGcdPair((BigInteger)(1821), (BigInteger)(2949), (BigInteger)(3));
      TestGcdPair((BigInteger)(-2634), (BigInteger)(-4353), (BigInteger)(3));
      TestGcdPair((BigInteger)(-1728), (BigInteger)(199), (BigInteger)(1));
      TestGcdPair((BigInteger)(-4646), (BigInteger)(-1418), (BigInteger)(2));
      TestGcdPair((BigInteger)(-35), (BigInteger)(-3578), (BigInteger)(1));
      TestGcdPair((BigInteger)(-2244), (BigInteger)(-3250), (BigInteger)(2));
      TestGcdPair((BigInteger)(-3329), (BigInteger)(1039), (BigInteger)(1));
      TestGcdPair((BigInteger)(-3064), (BigInteger)(-4730), (BigInteger)(2));
      TestGcdPair((BigInteger)(-1214), (BigInteger)(4130), (BigInteger)(2));
      TestGcdPair((BigInteger)(-3038), (BigInteger)(-3184), (BigInteger)(2));
      TestGcdPair((BigInteger)(-209), (BigInteger)(-1617), (BigInteger)(11));
      TestGcdPair((BigInteger)(-1101), (BigInteger)(-2886), (BigInteger)(3));
      TestGcdPair((BigInteger)(-3021), (BigInteger)(-4499), (BigInteger)(1));
      TestGcdPair((BigInteger)(3108), (BigInteger)(1815), (BigInteger)(3));
      TestGcdPair((BigInteger)(1195), (BigInteger)(4618), (BigInteger)(1));
      TestGcdPair((BigInteger)(-3643), (BigInteger)(2156), (BigInteger)(1));
      TestGcdPair((BigInteger)(-2067), (BigInteger)(-3780), (BigInteger)(3));
      TestGcdPair((BigInteger)(4251), (BigInteger)(1607), (BigInteger)(1));
      TestGcdPair((BigInteger)(438), (BigInteger)(741), (BigInteger)(3));
      TestGcdPair((BigInteger)(-3692), (BigInteger)(-2135), (BigInteger)(1));
      TestGcdPair((BigInteger)(-1076), (BigInteger)(2149), (BigInteger)(1));
      TestGcdPair((BigInteger)(-3224), (BigInteger)(-1532), (BigInteger)(4));
      TestGcdPair((BigInteger)(-3713), (BigInteger)(1721), (BigInteger)(1));
      TestGcdPair((BigInteger)(3038), (BigInteger)(-2657), (BigInteger)(1));
      TestGcdPair((BigInteger)(4977), (BigInteger)(-110), (BigInteger)(1));
      TestGcdPair((BigInteger)(-3305), (BigInteger)(-922), (BigInteger)(1));
      TestGcdPair((BigInteger)(1902), (BigInteger)(2481), (BigInteger)(3));
      TestGcdPair((BigInteger)(-4804), (BigInteger)(-1378), (BigInteger)(2));
      TestGcdPair((BigInteger)(-1446), (BigInteger)(-4226), (BigInteger)(2));
      TestGcdPair((BigInteger)(-1409), (BigInteger)(3303), (BigInteger)(1));
      TestGcdPair((BigInteger)(-1626), (BigInteger)(-3193), (BigInteger)(1));
      TestGcdPair((BigInteger)(912), (BigInteger)(-421), (BigInteger)(1));
      TestGcdPair((BigInteger)(751), (BigInteger)(-1755), (BigInteger)(1));
      TestGcdPair((BigInteger)(3135), (BigInteger)(-3581), (BigInteger)(1));
      TestGcdPair((BigInteger)(-4941), (BigInteger)(-2885), (BigInteger)(1));
      TestGcdPair((BigInteger)(4744), (BigInteger)(3240), (BigInteger)(8));
      TestGcdPair((BigInteger)(3488), (BigInteger)(4792), (BigInteger)(8));
      TestGcdPair((BigInteger)(3632), (BigInteger)(3670), (BigInteger)(2));
      TestGcdPair((BigInteger)(-4821), (BigInteger)(-1749), (BigInteger)(3));
      TestGcdPair((BigInteger)(4666), (BigInteger)(2013), (BigInteger)(1));
      TestGcdPair((BigInteger)(810), (BigInteger)(-3466), (BigInteger)(2));
      TestGcdPair((BigInteger)(2199), (BigInteger)(161), (BigInteger)(1));
      TestGcdPair((BigInteger)(-1137), (BigInteger)(-1620), (BigInteger)(3));
      TestGcdPair((BigInteger)(-472), (BigInteger)(66), (BigInteger)(2));
      TestGcdPair((BigInteger)(3825), (BigInteger)(2804), (BigInteger)(1));
      TestGcdPair((BigInteger)(-2895), (BigInteger)(1942), (BigInteger)(1));
      TestGcdPair((BigInteger)(1576), (BigInteger)(-4209), (BigInteger)(1));
      TestGcdPair((BigInteger)(-277), (BigInteger)(-4415), (BigInteger)(1));
      for (var i = 0; i < 1000; ++i) {
        prime = rand.NextValue(0x7fffffff);
        if(rand.NextValue(2)==0){
          prime=-prime;
        }
        int b=rand.NextValue(0x7fffffff);
        if(rand.NextValue(2)==0){
          b=-b;
        }
        var biga=(BigInteger)prime;
        var bigb=(BigInteger)b;
        BigInteger ba=BigInteger.GreatestCommonDivisor(biga,bigb);
        BigInteger bb=BigInteger.GreatestCommonDivisor(bigb,biga);
        Assert.AreEqual(ba,bb);
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
        BigInteger bigintA = RandomObjects.RandomBigInteger(r);
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
      Assert.AreEqual(
        Int32.MinValue,
        BigInteger.valueOf(Int32.MinValue).intValue());
      Assert.AreEqual(
        Int32.MaxValue,
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
      Assert.AreEqual(
        Int32.MinValue,
        BigInteger.valueOf(Int32.MinValue).intValueChecked());
      Assert.AreEqual(
        Int32.MaxValue,
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
      Assert.AreEqual(
        Int32.MinValue,
        BigInteger.valueOf(Int32.MinValue).intValueUnchecked());
      Assert.AreEqual(
        Int32.MaxValue,
        BigInteger.valueOf(Int32.MaxValue).intValueUnchecked());
      Assert.AreEqual(
        Int32.MaxValue,
        BigInteger.valueOf(Int32.MinValue - 1L).intValueUnchecked());
      Assert.AreEqual(
        Int32.MinValue,
        BigInteger.valueOf(Int32.MaxValue + 1L).intValueUnchecked());
    }
    [TestMethod]
    public void TestIsEven() {
      var r = new FastRandom();
      for (var i = 0; i < 1000; ++i) {
        BigInteger bigintA = RandomObjects.RandomBigInteger(r);
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
      Assert.AreEqual(
        Int64.MinValue,
        BigInteger.valueOf(Int64.MinValue).longValue());
      Assert.AreEqual(
        Int64.MaxValue,
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
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF200000000L),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF200000000L)).longValue());
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF280000000L),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF280000000L)).longValue());
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF280000001L),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF280000001L)).longValue());
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF27FFFFFFFL),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF27FFFFFFFL)).longValue());
      Assert.AreEqual(
        0x0000000380000001L,
        BigInteger.valueOf(0x0000000380000001L).longValue());
      Assert.AreEqual(
        0x0000000382222222L,
        BigInteger.valueOf(0x0000000382222222L).longValue());
      Assert.AreEqual(-8L, BigInteger.valueOf(-8L).longValue());
      Assert.AreEqual(-32768L, BigInteger.valueOf(-32768L).longValue());
      Assert.AreEqual(
        Int32.MinValue,
        BigInteger.valueOf(Int32.MinValue).longValue());
      Assert.AreEqual(
        Int32.MaxValue,
        BigInteger.valueOf(Int32.MaxValue).longValue());
      Assert.AreEqual(0x80000000L, BigInteger.valueOf(0x80000000L).longValue());
      Assert.AreEqual(0x90000000L, BigInteger.valueOf(0x90000000L).longValue());
    }
    [TestMethod]
    public void TestLongValueChecked() {
      Assert.AreEqual(
        Int64.MinValue,
        BigInteger.valueOf(Int64.MinValue).longValueChecked());
      Assert.AreEqual(
        Int64.MaxValue,
        BigInteger.valueOf(Int64.MaxValue).longValueChecked());
      try {
        BigInteger.valueOf(Int64.MinValue).subtract(BigInteger.One)
          .longValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.valueOf(Int64.MaxValue).add(BigInteger.One).longValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF200000000L),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF200000000L))
        .longValueChecked());
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF280000000L),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF280000000L))
        .longValueChecked());
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF280000001L),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF280000001L))
        .longValueChecked());
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF27FFFFFFFL),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF27FFFFFFFL))
        .longValueChecked());
      Assert.AreEqual(
        0x0000000380000001L,
        BigInteger.valueOf(0x0000000380000001L).longValueChecked());
      Assert.AreEqual(
        0x0000000382222222L,
        BigInteger.valueOf(0x0000000382222222L).longValueChecked());
      Assert.AreEqual(-8L, BigInteger.valueOf(-8L).longValueChecked());
      Assert.AreEqual(-32768L, BigInteger.valueOf(-32768L).longValueChecked());
      Assert.AreEqual(
        Int32.MinValue,
        BigInteger.valueOf(Int32.MinValue).longValueChecked());
      Assert.AreEqual(
        Int32.MaxValue,
        BigInteger.valueOf(Int32.MaxValue).longValueChecked());
      Assert.AreEqual(
        0x80000000L,
        BigInteger.valueOf(0x80000000L).longValueChecked());
      Assert.AreEqual(
        0x90000000L,
        BigInteger.valueOf(0x90000000L).longValueChecked());
    }
    [TestMethod]
    public void TestLongValueUnchecked() {
      Assert.AreEqual(
        Int64.MinValue,
        BigInteger.valueOf(Int64.MinValue).longValueUnchecked());
      Assert.AreEqual(
        Int64.MaxValue,
        BigInteger.valueOf(Int64.MaxValue).longValueUnchecked());
      Assert.AreEqual(
        Int64.MaxValue,
        BigInteger.valueOf(Int64.MinValue)
        .subtract(BigInteger.One).longValueUnchecked());
      Assert.AreEqual(
        Int64.MinValue,
        BigInteger.valueOf(Int64.MaxValue).add(BigInteger.One)
        .longValueUnchecked());
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF200000000L),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF200000000L))
        .longValueUnchecked());
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF280000000L),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF280000000L))
        .longValueUnchecked());
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF280000001L),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF280000001L))
        .longValueUnchecked());
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF27FFFFFFFL),
        BigInteger.valueOf(unchecked((long)0xFFFFFFF27FFFFFFFL))
        .longValueUnchecked());
      Assert.AreEqual(
        0x0000000380000001L,
        BigInteger.valueOf(0x0000000380000001L).longValueUnchecked());
      Assert.AreEqual(
        0x0000000382222222L,
        BigInteger.valueOf(0x0000000382222222L).longValueUnchecked());
      Assert.AreEqual(-8L, BigInteger.valueOf(-8L).longValueUnchecked());
      Assert.AreEqual(
        -32768L,
        BigInteger.valueOf(-32768L).longValueUnchecked());
      Assert.AreEqual(
        Int32.MinValue,
        BigInteger.valueOf(Int32.MinValue).longValueUnchecked());
      Assert.AreEqual(
        Int32.MaxValue,
        BigInteger.valueOf(Int32.MaxValue).longValueUnchecked());
      Assert.AreEqual(
        0x80000000L,
        BigInteger.valueOf(0x80000000L).longValueUnchecked());
      Assert.AreEqual(
        0x90000000L,
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
