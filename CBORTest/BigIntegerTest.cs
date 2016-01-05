using System;
using System.Text;
using NUnit.Framework;
using PeterO;

namespace Test {
  [TestFixture]
  public class BigIntegerTest {
    internal static BigInteger BigValueOf(long value) {
      return BigInteger.valueOf(value);
    }

    internal static BigInteger BigFromString(string str) {
      return BigInteger.fromString(str);
    }

    internal static BigInteger BigFromBytes(byte[] bytes) {
      return BigInteger.fromBytes(bytes, true);
    }

    public static BigInteger RandomBigInteger(FastRandom r) {
      int selection = r.NextValue(100);
      int count = r.NextValue(60) + 1;
      if (selection < 40) {
        count = r.NextValue(7) + 1;
      }
      if (selection < 50) {
        count = r.NextValue(15) + 1;
      }
      var bytes = new byte[count];
        for (var i = 0; i < count; ++i) {
          bytes[i] = (byte)((int)r.NextValue(256));
        }
      return BigFromBytes(bytes);
    }

    public static void DoTestDivide(
string dividend,
string divisor,
string result) {
      BigInteger bigintA = BigFromString(dividend);
      BigInteger bigintB = BigFromString(divisor);
      BigInteger bigintTemp;
      if (bigintB.IsZero) {
        try {
          bigintTemp = bigintA / bigintB;
          Assert.Fail("Expected divide by 0 error");
        } catch (ArithmeticException ex) {
          Console.WriteLine(ex.Message);
        }
        try {
          bigintA.divideAndRemainder(bigintB);
          Assert.Fail("Should have failed");
        } catch (ArithmeticException) {
          Console.Write(String.Empty);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      } else {
        AssertBigIntegersEqual(result, bigintA / bigintB);
        AssertBigIntegersEqual(result, bigintA.divideAndRemainder(bigintB)[0]);
      }
    }

    public static void DoTestMultiply(string m1, string m2, string result) {
      BigInteger bigintA = BigFromString(m1);
      BigInteger bigintB = BigFromString(m2);
      bigintA *= bigintB;
      AssertBigIntegersEqual(result, bigintA);
    }

    public static void DoTestPow(string m1, int m2, string result) {
      BigInteger bigintA = BigFromString(m1);
      AssertBigIntegersEqual(result, bigintA.pow(m2));
// #if UNUSED
      AssertBigIntegersEqual(result, bigintA.PowBigIntVar((BigInteger)m2));
////#endif
    }

    public static void AssertBigIntegersEqual(string a, BigInteger b) {
      Assert.AreEqual(a, b.ToString());
      BigInteger a2 = BigFromString(a);
      TestCommon.CompareTestEqualAndConsistent(a2, b);
      TestCommon.AssertEqualsHashCode(a2, b);
    }

    public static void DoTestDivideAndRemainder(
string dividend,
string divisor,
string result,
string rem) {
      BigInteger bigintA = BigFromString(dividend);
      BigInteger bigintB = BigFromString(divisor);
      BigInteger rembi;
      if (bigintB.IsZero) {
        try {
          BigInteger quo = BigInteger.DivRem(bigintA, bigintB, out rembi);
          if (((object)quo) == null) {
            Assert.Fail();
          }
          Assert.Fail("Expected divide by 0 error");
        } catch (ArithmeticException ex) {
          Console.WriteLine(ex.Message);
        }
      } else {
        BigInteger quo = BigInteger.DivRem(bigintA, bigintB, out rembi);
        AssertBigIntegersEqual(result, quo);
        AssertBigIntegersEqual(rem, rembi);
      }
    }

    private void TestMultiplyDivideOne(BigInteger bigintA, BigInteger bigintB) {
        // Test that A*B/A = B and A*B/B = A
        BigInteger bigintC = bigintA * (BigInteger)bigintB;
        BigInteger bigintRem;
        BigInteger bigintE;
        BigInteger bigintD;
        if (!bigintB.IsZero) {
          bigintD = BigInteger.DivRem(bigintC, bigintB, out bigintRem);
          if (!bigintD.Equals(bigintA)) {
            Assert.AreEqual(
bigintA,
bigintD,
"TestMultiplyDivide " + bigintA + "; " + bigintB + ";\n" + bigintC);
          }
          if (!bigintRem.IsZero) {
            Assert.AreEqual(
BigInteger.Zero,
bigintRem,
"TestMultiplyDivide " + bigintA + "; " + bigintB);
          }
          bigintE = bigintC / (BigInteger)bigintB;
          if (!bigintD.Equals(bigintE)) {
            // Testing that divideWithRemainder and division method return
            // the same value
            Assert.AreEqual(
bigintD,
bigintE,
"TestMultiplyDivide " + bigintA + "; " + bigintB + ";\n" + bigintC);
          }
          bigintE = bigintC % (BigInteger)bigintB;
          if (!bigintRem.Equals(bigintE)) {
            Assert.AreEqual(
bigintRem,
bigintE,
"TestMultiplyDivide " + bigintA + "; " + bigintB + ";\n" + bigintC);
          }
          if (bigintE.Sign > 0 && !bigintC.mod(bigintB).Equals(bigintE)) {
            Assert.Fail("TestMultiplyDivide " + bigintA + "; " + bigintB +
              ";\n" + bigintC);
          }
        }
        if (!bigintA.IsZero) {
          bigintD = BigInteger.DivRem(bigintC, bigintA, out bigintRem);
          if (!bigintD.Equals(bigintB)) {
            Assert.AreEqual(
bigintB,
bigintD,
"TestMultiplyDivide " + bigintA + "; " + bigintB);
          }
          if (!bigintRem.IsZero) {
            Assert.AreEqual(
BigInteger.Zero,
bigintRem,
"TestMultiplyDivide " + bigintA + "; " + bigintB);
          }
        }
        if (!bigintB.IsZero) {
          bigintC = BigInteger.DivRem(bigintA, bigintB, out bigintRem);
          bigintD = bigintB * (BigInteger)bigintC;
          bigintD += (BigInteger)bigintRem;
          if (!bigintD.Equals(bigintA)) {
            Assert.AreEqual(
bigintA,
bigintD,
"TestMultiplyDivide " + bigintA + "; " + bigintB);
          }
        }
    }

    [Test]
    public void TestMultiplyDivide() {
      var r = new FastRandom();
      for (var i = 0; i < 10000; ++i) {
        BigInteger bigintA = RandomBigInteger(r);
        BigInteger bigintB = RandomBigInteger(r);
        this.TestMultiplyDivideOne(bigintA, bigintB);
        this.TestMultiplyDivideOne(bigintB, bigintA);
      }
    }

    [Test]
    public void TestAddSubtract() {
      var r = new FastRandom();
      for (var i = 0; i < 10000; ++i) {
        BigInteger bigintA = RandomBigInteger(r);
        BigInteger bigintB = RandomBigInteger(r);
        BigInteger bigintC = bigintA + (BigInteger)bigintB;
        BigInteger bigintD = bigintC - (BigInteger)bigintB;
        if (!bigintD.Equals(bigintA)) {
          Assert.AreEqual(
bigintA,
bigintD,
"TestAddSubtract " + bigintA + "; " + bigintB);
        }
        bigintD = bigintC - (BigInteger)bigintA;
        if (!bigintD.Equals(bigintB)) {
          Assert.AreEqual(
bigintB,
bigintD,
"TestAddSubtract " + bigintA + "; " + bigintB);
        }
        bigintC = bigintA - (BigInteger)bigintB;
        bigintD = bigintC + (BigInteger)bigintB;
        if (!bigintD.Equals(bigintA)) {
          Assert.AreEqual(
bigintA,
bigintD,
"TestAddSubtract " + bigintA + "; " + bigintB);
        }
      }
    }

    public static void DoTestRemainder(
string dividend,
string divisor,
string result) {
      BigInteger bigintA = BigFromString(dividend);
      BigInteger bigintB = BigFromString(divisor);
      if (bigintB.IsZero) {
        try {
          bigintA.remainder(bigintB); Assert.Fail("Expected divide by 0 error");
        } catch (ArithmeticException ex) {
          Console.WriteLine(ex.Message);
        }
        try {
          bigintA.divideAndRemainder(bigintB);
          Assert.Fail("Should have failed");
        } catch (ArithmeticException) {
          Console.Write(String.Empty);
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      } else {
        AssertBigIntegersEqual(result, bigintA.remainder(bigintB));
        AssertBigIntegersEqual(result, bigintA.divideAndRemainder(bigintB)[1]);
      }
    }

    public static void AssertAdd(BigInteger bi, BigInteger bi2, string s) {
      BigIntegerTest.AssertBigIntegersEqual(s, bi + (BigInteger)bi2);
      BigIntegerTest.AssertBigIntegersEqual(s, bi2 + (BigInteger)bi);
      BigInteger negbi = BigInteger.Zero - (BigInteger)bi;
      BigInteger negbi2 = BigInteger.Zero - (BigInteger)bi2;
      BigIntegerTest.AssertBigIntegersEqual(s, bi - (BigInteger)negbi2);
      BigIntegerTest.AssertBigIntegersEqual(s, bi2 - (BigInteger)negbi);
    }

    [Test]
    public void TestAdd() {
      var posSmall = (BigInteger)5;
      BigInteger negSmall = -(BigInteger)5;
      var posLarge = (BigInteger)5555555;
      BigInteger negLarge = -(BigInteger)5555555;
      AssertAdd(posSmall, posSmall, "10");
      AssertAdd(posSmall, negSmall, "0");
      AssertAdd(posSmall, posLarge, "5555560");
      AssertAdd(posSmall, negLarge, "-5555550");
      AssertAdd(negSmall, negSmall, "-10");
      AssertAdd(negSmall, posLarge, "5555550");
      AssertAdd(negSmall, negLarge, "-5555560");
      AssertAdd(posLarge, posLarge, "11111110");
      AssertAdd(posLarge, negLarge, "0");
      AssertAdd(negLarge, negLarge, "-11111110");
    }

    [Test]
    public void TestCanFitInInt() {
      // not implemented yet
    }
    [Test]
    public void TestCompareTo() {
      var r = new FastRandom();
      for (var i = 0; i < 500; ++i) {
        BigInteger bigintA = RandomBigInteger(r);
        BigInteger bigintB = RandomBigInteger(r);
        BigInteger bigintC = RandomBigInteger(r);
        TestCommon.CompareTestRelations(bigintA, bigintB, bigintC);
        TestCommon.CompareTestConsistency(bigintA, bigintB, bigintC);
      }
    }
    [Test]
    public void TestDivide() {
      int intA, intB;
      var fr = new FastRandom();
      for (var i = 0; i < 10000; ++i) {
        intA = fr.NextValue(0x1000000);
        intB = fr.NextValue(0x1000000);
        if (intB == 0) {
          continue;
        }
        int c = intA / intB;
        var bigintA = (BigInteger)intA;
        var bigintB = (BigInteger)intB;
        BigInteger bigintC = bigintA / (BigInteger)bigintB;
        Assert.AreEqual((int)bigintC, c);
      }
      DoTestDivide("2472320648", "2831812081", "0");
      DoTestDivide("-2472320648", "2831812081", "0");
      DoTestDivide(
    "9999999999999999999999",
    "281474976710655",
    "35527136");
    }
    [Test]
    public void TestDivRem() {
      // not implemented yet
    }
    [Test]
    public void TestEquals() {
      Assert.IsFalse(BigInteger.One.Equals(null));
      Assert.IsFalse(BigInteger.Zero.Equals(null));
      Assert.IsFalse(BigInteger.One.Equals(BigInteger.Zero));
      Assert.IsFalse(BigInteger.Zero.Equals(BigInteger.One));
      var r = new FastRandom();
      for (var i = 0; i < 500; ++i) {
        BigInteger bigintA = RandomObjects.RandomBigInteger(r);
        BigInteger bigintB = RandomObjects.RandomBigInteger(r);
        TestCommon.AssertEqualsHashCode(bigintA, bigintB);
      }
    }

    public static int ModPow(int x, int pow, int intMod) {
      if (x < 0) {
        throw new ArgumentException(
          "x (" + x + ") is less than 0");
      }
      if (pow <= 0) {
        throw new ArgumentException(
          "pow (" + pow + ") is not greater than 0");
      }
      if (intMod <= 0) {
        throw new ArgumentException(
          "mod (" + intMod + ") is not greater than 0");
      }
      var r = 1;
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
      if (n <= 23) {
        return n == 3 || n == 5 || n == 7 || n == 11 ||
          n == 13 || n == 17 || n == 19 || n == 23;
      }
      // Use a deterministic Rabin-Miller test
      int d = n - 1;
      var shift = 0;
      while ((d & 1) == 0) {
        d >>= 1;
        ++shift;
      }
      int mp = 0, mp2 = 0;
      var found = false;
      // For all 32-bit integers it's enough
      // to check the strong pseudoprime
      // bases 2, 7, and 61
      if (n > 2) {
        mp = ModPow(2, d, n);
        if (mp != 1 && mp + 1 != n) {
          found = false;
          for (var i = 1; i < shift; ++i) {
            mp2 = ModPow(2, d << i, n);
            if (mp2 + 1 == n) {
              found = true;
              break;
            }
          }
          if (found) {
            return false;
          }
        }
      }
      if (n > 7) {
        mp = ModPow(7, d, n);
        if (mp != 1 && mp + 1 != n) {
          found = false;
          for (var i = 1; i < shift; ++i) {
            mp2 = ModPow(7, d << i, n);
            if (mp2 + 1 == n) {
              found = true;
              break;
            }
          }
          if (found) {
            return false;
          }
        }
      }
      if (n > 61) {
        mp = ModPow(61, d, n);
        if (mp != 1 && mp + 1 != n) {
          found = false;
          for (var i = 1; i < shift; ++i) {
            mp2 = ModPow(61, d << i, n);
            if (mp2 + 1 == n) {
              found = true;
              break;
            }
          }
          if (found) {
            return false;
          }
        }
      }
      return true;
    }

    private static void TestGcdPair(
      BigInteger biga,
      BigInteger bigb,
      BigInteger biggcd) {
      BigInteger ba = BigInteger.GreatestCommonDivisor(biga, bigb);
      BigInteger bb = BigInteger.GreatestCommonDivisor(bigb, biga);
      Assert.AreEqual(ba, biggcd);
      Assert.AreEqual(bb, biggcd);
    }

    [Test]
    public void TestGcd() {
      try {
 BigInteger.Zero.gcd(null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      {
string stringTemp = BigInteger.Zero.gcd(BigFromString(
"244")).ToString();
Assert.AreEqual(
"244",
stringTemp);
}
      {
string stringTemp = BigInteger.Zero.gcd(BigFromString(
"-244")).ToString();
Assert.AreEqual(
"244",
stringTemp);
}
      {
string stringTemp = BigFromString(
"244").gcd(BigInteger.Zero).ToString();
Assert.AreEqual(
"244",
stringTemp);
}
      {
string stringTemp = BigFromString(
"-244").gcd(BigInteger.Zero).ToString();
Assert.AreEqual(
"244",
stringTemp);
}
      {
string stringTemp = BigInteger.One.gcd(BigFromString("244")).ToString();
Assert.AreEqual(
"1",
stringTemp);
}
      {
string stringTemp = BigInteger.One.gcd(BigFromString(
"-244")).ToString();
Assert.AreEqual(
"1",
stringTemp);
}
      {
string stringTemp = BigFromString("244").gcd(BigInteger.One).ToString();
Assert.AreEqual(
"1",
stringTemp);
}
      {
string stringTemp = BigFromString(
"-244").gcd(BigInteger.One).ToString();
Assert.AreEqual(
"1",
stringTemp);
}
      {
string stringTemp = BigFromString("15").gcd(BigFromString(
"15")).ToString();
Assert.AreEqual(
"15",
stringTemp);
}
      {
string stringTemp = BigFromString("-15").gcd(
        BigFromString("15")).ToString();
Assert.AreEqual(
"15",
stringTemp);
}
      {
string stringTemp = BigFromString("15").gcd(
        BigFromString("-15")).ToString();
Assert.AreEqual(
"15",
stringTemp);
}
      {
string stringTemp = BigFromString(
"-15").gcd(BigFromString("-15")).ToString();
Assert.AreEqual(
"15",
stringTemp);
}
      var prime = 0;
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
        BigInteger ba = RandomBigInteger(rand);
        if (ba.IsZero) {
          continue;
        }
        ba *= (BigInteger)bigprime;
        Assert.AreEqual(
          bigprime,
          BigInteger.GreatestCommonDivisor(bigprime, ba));
      }
      TestGcdPair((BigInteger)(-1867), (BigInteger)(-4456), BigInteger.One);
      TestGcdPair((BigInteger)4604, (BigInteger)(-4516), (BigInteger)4);
      TestGcdPair((BigInteger)(-1756), (BigInteger)4525, BigInteger.One);
      TestGcdPair((BigInteger)1568, (BigInteger)(-4955), BigInteger.One);
      TestGcdPair((BigInteger)2519, (BigInteger)2845, BigInteger.One);
      TestGcdPair((BigInteger)(-1470), (BigInteger)132, (BigInteger)6);
      TestGcdPair((BigInteger)(-2982), (BigInteger)2573, BigInteger.One);
      TestGcdPair((BigInteger)(-244), (BigInteger)(-3929), BigInteger.One);
      TestGcdPair((BigInteger)(-3794), (BigInteger)(-2325), BigInteger.One);
      TestGcdPair((BigInteger)(-2667), (BigInteger)2123, BigInteger.One);
      TestGcdPair((BigInteger)(-3712), (BigInteger)(-1850), (BigInteger)2);
      TestGcdPair((BigInteger)2329, (BigInteger)3874, BigInteger.One);
      TestGcdPair((BigInteger)1384, (BigInteger)(-4278), (BigInteger)2);
      TestGcdPair((BigInteger)213, (BigInteger)(-1217), BigInteger.One);
      TestGcdPair((BigInteger)1163, (BigInteger)2819, BigInteger.One);
      TestGcdPair((BigInteger)1921, (BigInteger)(-579), BigInteger.One);
      TestGcdPair((BigInteger)3886, (BigInteger)(-13), BigInteger.One);
      TestGcdPair((BigInteger)(-3270), (BigInteger)(-3760), (BigInteger)10);
      TestGcdPair((BigInteger)(-3528), (BigInteger)1822, (BigInteger)2);
      TestGcdPair((BigInteger)1547, (BigInteger)(-333), BigInteger.One);
      TestGcdPair((BigInteger)2402, (BigInteger)2850, (BigInteger)2);
      TestGcdPair((BigInteger)4519, (BigInteger)1296, BigInteger.One);
      TestGcdPair((BigInteger)1821, (BigInteger)2949, (BigInteger)3);
      TestGcdPair((BigInteger)(-2634), (BigInteger)(-4353), (BigInteger)3);
      TestGcdPair((BigInteger)(-1728), (BigInteger)199, BigInteger.One);
      TestGcdPair((BigInteger)(-4646), (BigInteger)(-1418), (BigInteger)2);
      TestGcdPair((BigInteger)(-35), (BigInteger)(-3578), BigInteger.One);
      TestGcdPair((BigInteger)(-2244), (BigInteger)(-3250), (BigInteger)2);
      TestGcdPair((BigInteger)(-3329), (BigInteger)1039, BigInteger.One);
      TestGcdPair((BigInteger)(-3064), (BigInteger)(-4730), (BigInteger)2);
      TestGcdPair((BigInteger)(-1214), (BigInteger)4130, (BigInteger)2);
      TestGcdPair((BigInteger)(-3038), (BigInteger)(-3184), (BigInteger)2);
      TestGcdPair((BigInteger)(-209), (BigInteger)(-1617), (BigInteger)11);
      TestGcdPair((BigInteger)(-1101), (BigInteger)(-2886), (BigInteger)3);
      TestGcdPair((BigInteger)(-3021), (BigInteger)(-4499), BigInteger.One);
      TestGcdPair((BigInteger)3108, (BigInteger)1815, (BigInteger)3);
      TestGcdPair((BigInteger)1195, (BigInteger)4618, BigInteger.One);
      TestGcdPair((BigInteger)(-3643), (BigInteger)2156, BigInteger.One);
      TestGcdPair((BigInteger)(-2067), (BigInteger)(-3780), (BigInteger)3);
      TestGcdPair((BigInteger)4251, (BigInteger)1607, BigInteger.One);
      TestGcdPair((BigInteger)438, (BigInteger)741, (BigInteger)3);
      TestGcdPair((BigInteger)(-3692), (BigInteger)(-2135), BigInteger.One);
      TestGcdPair((BigInteger)(-1076), (BigInteger)2149, BigInteger.One);
      TestGcdPair((BigInteger)(-3224), (BigInteger)(-1532), (BigInteger)4);
      TestGcdPair((BigInteger)(-3713), (BigInteger)1721, BigInteger.One);
      TestGcdPair((BigInteger)3038, (BigInteger)(-2657), BigInteger.One);
      TestGcdPair((BigInteger)4977, (BigInteger)(-110), BigInteger.One);
      TestGcdPair((BigInteger)(-3305), (BigInteger)(-922), BigInteger.One);
      TestGcdPair((BigInteger)1902, (BigInteger)2481, (BigInteger)3);
      TestGcdPair((BigInteger)(-4804), (BigInteger)(-1378), (BigInteger)2);
      TestGcdPair((BigInteger)(-1446), (BigInteger)(-4226), (BigInteger)2);
      TestGcdPair((BigInteger)(-1409), (BigInteger)3303, BigInteger.One);
      TestGcdPair((BigInteger)(-1626), (BigInteger)(-3193), BigInteger.One);
      TestGcdPair((BigInteger)912, (BigInteger)(-421), BigInteger.One);
      TestGcdPair((BigInteger)751, (BigInteger)(-1755), BigInteger.One);
      TestGcdPair((BigInteger)3135, (BigInteger)(-3581), BigInteger.One);
      TestGcdPair((BigInteger)(-4941), (BigInteger)(-2885), BigInteger.One);
      TestGcdPair((BigInteger)4744, (BigInteger)3240, (BigInteger)8);
      TestGcdPair((BigInteger)3488, (BigInteger)4792, (BigInteger)8);
      TestGcdPair((BigInteger)3632, (BigInteger)3670, (BigInteger)2);
      TestGcdPair((BigInteger)(-4821), (BigInteger)(-1749), (BigInteger)3);
      TestGcdPair((BigInteger)4666, (BigInteger)2013, BigInteger.One);
      TestGcdPair((BigInteger)810, (BigInteger)(-3466), (BigInteger)2);
      TestGcdPair((BigInteger)2199, (BigInteger)161, BigInteger.One);
      TestGcdPair((BigInteger)(-1137), (BigInteger)(-1620), (BigInteger)3);
      TestGcdPair((BigInteger)(-472), (BigInteger)66, (BigInteger)2);
      TestGcdPair((BigInteger)3825, (BigInteger)2804, BigInteger.One);
      TestGcdPair((BigInteger)(-2895), (BigInteger)1942, BigInteger.One);
      TestGcdPair((BigInteger)1576, (BigInteger)(-4209), BigInteger.One);
      TestGcdPair((BigInteger)(-277), (BigInteger)(-4415), BigInteger.One);
      for (var i = 0; i < 1000; ++i) {
        prime = rand.NextValue(0x7fffffff);
        if (rand.NextValue(2) == 0) {
          prime = -prime;
        }
        int intB = rand.NextValue(0x7fffffff);
        if (rand.NextValue(2) == 0) {
          intB = -intB;
        }
        var biga = (BigInteger)prime;
        var bigb = (BigInteger)intB;
        BigInteger ba = BigInteger.GreatestCommonDivisor(biga, bigb);
        BigInteger bb = BigInteger.GreatestCommonDivisor(bigb, biga);
        Assert.AreEqual(ba, bb);
      }
    }

    [Test]
    public void TestGetBits() {
      // not implemented yet
    }
    [Test]
    public void TestGetLowBit() {
      // not implemented yet
    }
    [Test]
    public void TestGetUnsignedBitLength() {
      // not implemented yet
    }

    [Test]
    public void TestIntValueChecked() {
      Assert.AreEqual(
        Int32.MinValue,
        BigValueOf(Int32.MinValue).intValueChecked());
      Assert.AreEqual(
        Int32.MaxValue,
        BigValueOf(Int32.MaxValue).intValueChecked());
      try {
        BigValueOf(Int32.MinValue - 1L).intValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigValueOf(Int32.MaxValue + 1L).intValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigFromString("999999999999999999999999999999999").intValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(
        Int32.MinValue,
        BigValueOf(Int32.MinValue).intValueChecked());
      Assert.AreEqual(
        Int32.MaxValue,
        BigValueOf(Int32.MaxValue).intValueChecked());
      try {
        BigValueOf(Int32.MinValue - 1L).intValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigValueOf(Int32.MaxValue + 1L).intValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestIsEven() {
      var r = new FastRandom();
      for (var i = 0; i < 1000; ++i) {
        BigInteger bigintA = RandomBigInteger(r);
        BigInteger mod = bigintA.remainder(BigValueOf(2));
        Assert.AreEqual(mod.IsZero, bigintA.IsEven);
      }
    }

    [Test]
    public void TestLongValueChecked() {
      Assert.AreEqual(
        Int64.MinValue,
        BigValueOf(Int64.MinValue).longValueChecked());
      Assert.AreEqual(
        Int64.MaxValue,
        BigValueOf(Int64.MaxValue).longValueChecked());
      try {
        BigInteger bigintTemp = BigValueOf(Int64.MinValue);
        bigintTemp -= BigInteger.One;
        bigintTemp.longValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger bigintTemp = BigValueOf(Int64.MaxValue);
        bigintTemp += BigInteger.One;
        bigintTemp.longValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF200000000L),
  BigValueOf(unchecked((long)0xFFFFFFF200000000L)).longValueChecked());
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF280000000L),
  BigValueOf(unchecked((long)0xFFFFFFF280000000L)).longValueChecked());
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF280000001L),
  BigValueOf(unchecked((long)0xFFFFFFF280000001L)).longValueChecked());
      Assert.AreEqual(
        unchecked((long)0xFFFFFFF27FFFFFFFL),
  BigValueOf(unchecked((long)0xFFFFFFF27FFFFFFFL)).longValueChecked());
      Assert.AreEqual(
        0x0000000380000001L,
        BigValueOf(0x0000000380000001L).longValueChecked());
      Assert.AreEqual(
        0x0000000382222222L,
        BigValueOf(0x0000000382222222L).longValueChecked());
      Assert.AreEqual(-8L, BigValueOf(-8L).longValueChecked());
      Assert.AreEqual(-32768L, BigValueOf(-32768L).longValueChecked());
      Assert.AreEqual(
        Int32.MinValue,
        BigValueOf(Int32.MinValue).longValueChecked());
      Assert.AreEqual(
        Int32.MaxValue,
        BigValueOf(Int32.MaxValue).longValueChecked());
      Assert.AreEqual(
        0x80000000L,
        BigValueOf(0x80000000L).longValueChecked());
      Assert.AreEqual(
        0x90000000L,
        BigValueOf(0x90000000L).longValueChecked());
      try {
        BigFromString("999999999999999999999999999999999").longValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(
        Int64.MinValue,
        BigValueOf(Int64.MinValue).longValueChecked());
      Assert.AreEqual(
        Int64.MaxValue,
        BigValueOf(Int64.MaxValue).longValueChecked());
      try {
        BigInteger bigintTemp = BigValueOf(Int64.MinValue);
        bigintTemp -= BigInteger.One;
        bigintTemp.longValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger bigintTemp = BigValueOf(Int64.MaxValue);
        bigintTemp += BigInteger.One;
        bigintTemp.longValueChecked();
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      long longV = unchecked((long)0xFFFFFFF200000000L);
      Assert.AreEqual(
longV,
BigValueOf(longV).longValueChecked());
      longV = unchecked((long)0xFFFFFFF280000000L);
      Assert.AreEqual(
longV,
BigValueOf(longV).longValueChecked());
      longV = unchecked((long)0xFFFFFFF280000001L);
      Assert.AreEqual(
longV,
BigValueOf(longV).longValueChecked());
      longV = unchecked((long)0xFFFFFFF27FFFFFFFL);
      Assert.AreEqual(
longV,
BigValueOf(longV).longValueChecked());
      Assert.AreEqual(
        0x0000000380000001L,
        BigValueOf(0x0000000380000001L).longValueChecked());
      Assert.AreEqual(
        0x0000000382222222L,
        BigValueOf(0x0000000382222222L).longValueChecked());
      Assert.AreEqual(-8L, BigValueOf(-8L).longValueChecked());
      Assert.AreEqual(-32768L, BigValueOf(-32768L).longValueChecked());
      Assert.AreEqual(
        Int32.MinValue,
        BigValueOf(Int32.MinValue).longValueChecked());
      Assert.AreEqual(
        Int32.MaxValue,
        BigValueOf(Int32.MaxValue).longValueChecked());
      Assert.AreEqual(
        0x80000000L,
        BigValueOf(0x80000000L).longValueChecked());
      Assert.AreEqual(
        0x90000000L,
        BigValueOf(0x90000000L).longValueChecked());
    }

    [Test]
    public void TestMod() {
      try {
        BigInteger.One.mod(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ((BigInteger)13).mod(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ((BigInteger)13).mod((BigInteger)(-4));
        Assert.Fail("Should have failed");
      } catch (ArithmeticException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ((BigInteger)(-13)).mod((BigInteger)(-4));
        Assert.Fail("Should have failed");
      } catch (ArithmeticException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestIntValueUnchecked() {
      Assert.AreEqual(0L, BigInteger.Zero.intValueUnchecked());
      Assert.AreEqual(
        Int32.MinValue,
        BigValueOf(Int32.MinValue).intValueUnchecked());
      Assert.AreEqual(
        Int32.MaxValue,
        BigValueOf(Int32.MaxValue).intValueUnchecked());
      Assert.AreEqual(
        Int32.MaxValue,
        BigValueOf(Int32.MinValue - 1L).intValueUnchecked());
      Assert.AreEqual(
        Int32.MinValue,
        BigValueOf(Int32.MaxValue + 1L).intValueUnchecked());
    }
    [Test]
    public void TestLongValueUnchecked() {
      Assert.AreEqual(0L, BigInteger.Zero.longValueUnchecked());
      Assert.AreEqual(
        Int64.MinValue,
        BigValueOf(Int64.MinValue).longValueUnchecked());
      Assert.AreEqual(
        Int64.MaxValue,
        BigValueOf(Int64.MaxValue).longValueUnchecked());
      {
object objectTemp = Int64.MaxValue;
object objectTemp2 = BigValueOf(Int64.MinValue)
        .subtract(BigInteger.One).longValueUnchecked();
Assert.AreEqual(objectTemp, objectTemp2);
}
      Assert.AreEqual(
        Int64.MinValue,
        BigValueOf(Int64.MaxValue).add(BigInteger.One).longValueUnchecked());
      long aa = unchecked((long)0xFFFFFFF200000000L);
      Assert.AreEqual(
              aa,
              BigValueOf(aa).longValueUnchecked());
      aa = unchecked((long)0xFFFFFFF280000000L);
      Assert.AreEqual(
              aa,
              BigValueOf(aa).longValueUnchecked());
      aa = unchecked((long)0xFFFFFFF200000001L);
      Assert.AreEqual(
              aa,
              BigValueOf(aa).longValueUnchecked());
      aa = unchecked((long)0xFFFFFFF27FFFFFFFL);
      Assert.AreEqual(
              aa,
              BigValueOf(aa).longValueUnchecked());
      Assert.AreEqual(
        0x0000000380000001L,
        BigValueOf(0x0000000380000001L).longValueUnchecked());
      Assert.AreEqual(
        0x0000000382222222L,
        BigValueOf(0x0000000382222222L).longValueUnchecked());
      Assert.AreEqual(-8L, BigValueOf(-8L).longValueUnchecked());
      Assert.AreEqual(
        -32768L,
        BigValueOf(-32768L).longValueUnchecked());
      Assert.AreEqual(
        Int32.MinValue,
        BigValueOf(Int32.MinValue).longValueUnchecked());
      Assert.AreEqual(
        Int32.MaxValue,
        BigValueOf(Int32.MaxValue).longValueUnchecked());
      Assert.AreEqual(
        0x80000000L,
        BigValueOf(0x80000000L).longValueUnchecked());
      Assert.AreEqual(
        0x90000000L,
        BigValueOf(0x90000000L).longValueUnchecked());
    }
    [Test]
    public void TestIsPowerOfTwo() {
      // not implemented yet
    }
    [Test]
    public void TestIsZero() {
      // not implemented yet
    }

////#if true

    [Test]
    public void TestDivideAndRemainder() {
      try {
        BigInteger.One.divideAndRemainder(BigInteger.Zero);
        Assert.Fail("Should have failed");
      } catch (DivideByZeroException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.divideAndRemainder(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestFromBytes() {
      Assert.AreEqual(
        BigInteger.Zero, BigInteger.fromBytes(new byte[] { }, false));

      try {
        BigInteger.fromBytes(null, false);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestBitLength() {
      Assert.AreEqual(31, BigValueOf(-2147483647L).bitLength());
      Assert.AreEqual(31, BigValueOf(-2147483648L).bitLength());
      Assert.AreEqual(32, BigValueOf(-2147483649L).bitLength());
      Assert.AreEqual(32, BigValueOf(-2147483650L).bitLength());
      Assert.AreEqual(31, BigValueOf(2147483647L).bitLength());
      Assert.AreEqual(32, BigValueOf(2147483648L).bitLength());
      Assert.AreEqual(32, BigValueOf(2147483649L).bitLength());
      Assert.AreEqual(32, BigValueOf(2147483650L).bitLength());
      Assert.AreEqual(0, BigValueOf(0).bitLength());
      Assert.AreEqual(1, BigValueOf(1).bitLength());
      Assert.AreEqual(2, BigValueOf(2).bitLength());
      Assert.AreEqual(2, BigValueOf(2).bitLength());
      Assert.AreEqual(31, BigValueOf(Int32.MaxValue).bitLength());
      Assert.AreEqual(31, BigValueOf(Int32.MinValue).bitLength());
      Assert.AreEqual(16, BigValueOf(65535).bitLength());
      Assert.AreEqual(16, BigValueOf(-65535).bitLength());
      Assert.AreEqual(17, BigValueOf(65536).bitLength());
      Assert.AreEqual(16, BigValueOf(-65536).bitLength());
      Assert.AreEqual(
        65,
        BigFromString("19084941898444092059").bitLength());
      Assert.AreEqual(
        65,
        BigFromString("-19084941898444092059").bitLength());
      Assert.AreEqual(0, BigValueOf(-1).bitLength());
      Assert.AreEqual(1, BigValueOf(-2).bitLength());
    }
    [Test]
    public void TestFromString() {
      try {
        BigFromString("xyz");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigFromString(String.Empty);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigFromString(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestFromSubstring() {
      try {
        BigInteger.fromSubstring(null, 0, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        BigInteger.fromSubstring("123", -1, 2);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromSubstring("123", 4, 2);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromSubstring("123", 1, -1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromSubstring("123", 1, 4);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromSubstring("123", 1, 0);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromSubstring("123", 2, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      {
 string stringTemp = BigInteger.fromSubstring(
   "0123456789",
   9,
   10).ToString();
        Assert.AreEqual(
          "9",
          stringTemp);
      }
      {
 string stringTemp = BigInteger.fromSubstring(
   "0123456789",
   8,
   10).ToString();
        Assert.AreEqual(
          "89",
          stringTemp);
      }
      {
 string stringTemp = BigInteger.fromSubstring(
   "0123456789",
   7,
   10).ToString();
        Assert.AreEqual(
          "789",
          stringTemp);
      }
      {
 string stringTemp = BigInteger.fromSubstring(
   "0123456789",
   6,
   10).ToString();
        Assert.AreEqual(
          "6789",
          stringTemp);
      }
    }
    [Test]
    public void TestFromRadixString() {
      try {
        BigInteger.fromRadixString(null, 10);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixString("0", 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixString("0", 0);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixString("0", -37);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixString("0", Int32.MinValue);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixString("0", Int32.MaxValue);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      var fr = new FastRandom();
      for (int i = 2; i <= 36; ++i) {
        for (int j = 0; j < 100; ++j) {
          StringAndBigInt sabi = StringAndBigInt.Generate(fr, i);
          Assert.AreEqual(
            sabi.BigIntValue,
            BigInteger.fromRadixString(sabi.StringValue, i));
        }
      }
    }
    [Test]
    public void TestFromRadixSubstring() {
      try {
        BigInteger.fromRadixSubstring(null, 10, 0, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("0", 1, 0, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("0", 0, 0, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("0", -37, 0, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("0", Int32.MinValue, 0, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("0", Int32.MaxValue, 0, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("123", 10, -1, 2);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("123", 10, 4, 5);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("123", 10, 0, -8);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("123", 10, 0, 6);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("123", 10, 2, 0);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("123", 10, 0, 0);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("123", 10, 1, 1);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("-", 10, 0, 1);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("g", 16, 0, 1);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("0123gggg", 16, 0, 8);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("0123gggg", 10, 0, 8);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromRadixSubstring("0123aaaa", 10, 0, 8);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      var fr = new FastRandom();
      for (int i = 2; i <= 36; ++i) {
        var padding = new StringBuilder();
        for (int j = 0; j < 100; ++j) {
          StringAndBigInt sabi = StringAndBigInt.Generate(fr, i);
          padding.Append('!');
          string sabiString = sabi.StringValue;
          BigInteger actualBigInt = BigInteger.fromRadixSubstring(
            padding + sabiString,
            i,
            j + 1,
            j + 1 + sabiString.Length);
          Assert.AreEqual(
            sabi.BigIntValue,
            actualBigInt);
        }
      }
    }
    [Test]
    public void TestGetDigitCount() {
      var r = new FastRandom();
      for (var i = 0; i < 1000; ++i) {
        BigInteger bigintA = RandomBigInteger(r);
        String str = BigInteger.Abs(bigintA).ToString();
        Assert.AreEqual(str.Length, bigintA.getDigitCount());
      }
    }
    [Test]
    public void TestBigIntegerModPow() {
      try {
 BigInteger.One.ModPow(null, null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 BigInteger.One.ModPow(null, BigInteger.Zero);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 BigInteger.One.ModPow(BigInteger.Zero, null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 BigInteger.One.ModPow(BigFromString("-1"), BigFromString("1"));
Assert.Fail("Should have failed");
} catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 BigInteger.One.ModPow(BigFromString("0"), BigFromString("0"));
Assert.Fail("Should have failed");
} catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 BigInteger.One.ModPow(BigFromString("0"), BigFromString("-1"));
Assert.Fail("Should have failed");
} catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 BigInteger.One.ModPow(BigFromString("1"), BigFromString("0"));
Assert.Fail("Should have failed");
} catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 BigInteger.One.ModPow(BigFromString("1"), BigFromString("-1"));
Assert.Fail("Should have failed");
} catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
    }

    [Test]
    public void TestSqrt() {
      var r = new FastRandom();
      for (var i = 0; i < 10000; ++i) {
        BigInteger bigintA = RandomBigInteger(r);
        if (bigintA.Sign < 0) {
          bigintA = -bigintA;
        }
        if (bigintA.Sign == 0) {
          bigintA = BigInteger.One;
        }
        BigInteger sr = bigintA.sqrt();
        BigInteger srsqr = sr * (BigInteger)sr;
        sr += BigInteger.One;
        BigInteger sronesqr = sr * (BigInteger)sr;
        if (srsqr.CompareTo(bigintA) > 0) {
          Assert.Fail(srsqr + " not " + bigintA +
            " or less (TestSqrt, sqrt=" + sr + ")");
        }
        if (sronesqr.CompareTo(bigintA) <= 0) {
          Assert.Fail(srsqr + " not greater than " + bigintA +
            " (TestSqrt, sqrt=" + sr + ")");
        }
      }
    }
    [Test]
    public void TestTestBit() {
      Assert.IsFalse(BigInteger.Zero.testBit(0));
      Assert.IsFalse(BigInteger.Zero.testBit(1));
      Assert.IsTrue(BigInteger.One.testBit(0));
      Assert.IsFalse(BigInteger.One.testBit(1));
      for (int i = 0; i < 32; ++i) {
        Assert.IsTrue(BigValueOf(-1).testBit(i));
      }
    }

    [Test]
    public void TestToRadixString() {
      var fr = new FastRandom();
      try {
        BigInteger.One.toRadixString(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.toRadixString(0);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.toRadixString(1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.toRadixString(37);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.toRadixString(Int32.MinValue);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.toRadixString(Int32.MaxValue);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      for (int i = 2; i <= 36; ++i) {
        for (int j = 0; j < 100; ++j) {
          StringAndBigInt sabi = StringAndBigInt.Generate(fr, i);
          // Upper case result expected
          string expected = ToUpperCaseAscii(sabi.StringValue);
          var k = 0;
          // Expects result with no unnecessary leading zeros
          bool negative = sabi.BigIntValue.Sign < 0;
          if (expected[0] == '-') {
            ++k;
          }
          while (k < expected.Length - 1) {
            if (expected[k] == '0') {
              ++k;
            } else {
              break;
            }
          }
          expected = expected.Substring(k);
          if (negative) {
            expected = "-" + expected;
          }
          Assert.AreEqual(
            expected,
            sabi.BigIntValue.toRadixString(i));
        }
      }
      var r = new FastRandom();
      for (var radix = 2; radix < 36; ++radix) {
        for (var i = 0; i < 80; ++i) {
          BigInteger bigintA = RandomBigInteger(r);
          String s = bigintA.toRadixString(radix);
          BigInteger big2 = BigInteger.fromRadixString(s, radix);
          Assert.AreEqual(big2.toRadixString(radix), s);
        }
      }
    }

////#endif
    [Test]
    public void TestNegate() {
      // not implemented yet
    }
    [Test]
    public void TestNot() {
      // not implemented yet
    }
    [Test]
    public void TestOne() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorAddition() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorDivision() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorExplicit() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorGreaterThan() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorGreaterThanOrEqual() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorImplicit() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorLeftShift() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorLessThan() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorLessThanOrEqual() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorModulus() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorMultiply() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorRightShift() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorSubtraction() {
      // not implemented yet
    }
    [Test]
    public void TestOperatorUnaryNegation() {
      // not implemented yet
    }
    [Test]
    public void TestOr() {
      // not implemented yet
    }

    [Test]
    public void TestPow() {
      var r = new FastRandom();
      for (var i = 0; i < 200; ++i) {
        int power = 1 + r.NextValue(8);
        BigInteger bigintA = RandomBigInteger(r);
        BigInteger bigintB = bigintA;
        for (int j = 1; j < power; ++j) {
          bigintB *= bigintA;
        }
        DoTestPow(bigintA.ToString(), power, bigintB.ToString());
      }
    }
    [Test]
    public void TestMultiply() {
      try {
        BigInteger.One.multiply(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      var r = new FastRandom();
      for (var i = 0; i < 10000; ++i) {
        BigInteger bigintA = RandomBigInteger(r);
        BigInteger bigintB = bigintA + BigInteger.One;
        BigInteger bigintC = bigintA * (BigInteger)bigintB;
        // Test near-squaring
        if (bigintA.IsZero || bigintB.IsZero) {
          Assert.AreEqual(BigInteger.Zero, bigintC);
        }
        if (bigintA.Equals(BigInteger.One)) {
          Assert.AreEqual(bigintB, bigintC);
        }
        if (bigintB.Equals(BigInteger.One)) {
          Assert.AreEqual(bigintA, bigintC);
        }
        bigintB = bigintA;
        // Test squaring
        bigintC = bigintA * (BigInteger)bigintB;
        if (bigintA.IsZero || bigintB.IsZero) {
          Assert.AreEqual(BigInteger.Zero, bigintC);
        }
        if (bigintA.Equals(BigInteger.One)) {
          Assert.AreEqual(bigintB, bigintC);
        }
        if (bigintB.Equals(BigInteger.One)) {
          Assert.AreEqual(bigintA, bigintC);
        }
      }
      DoTestMultiply(
"39258416159456516340113264558732499166970244380745050",
"39258416159456516340113264558732499166970244380745051",
"1541223239349076530208308657654362309553698742116222355477449713742236585667505604058123112521437480247550");
      DoTestMultiply(
  "5786426269322750882632312999752639738983363095641642905722171221986067189342123124290107105663618428969517616421742429671402859775667602123564",
  "331378991485809774307751183645559883724387697397707434271522313077548174328632968616330900320595966360728317363190772921",
  "1917500101435169880779183578665955372346028226046021044867189027856189131730889958057717187493786883422516390996639766012958050987359732634213213442579444095928862861132583117668061032227577386757036981448703231972963300147061503108512300577364845823910107210444");
    }
    [Test]
    public void TestPowBigIntVar() {
      // not implemented yet
    }
    [Test]
    public void TestRemainder() {
      DoTestRemainder("2472320648", "2831812081", "2472320648");
      DoTestRemainder("-2472320648", "2831812081", "-2472320648");
    }

    public static void DoTestShiftLeft(string m1, int m2, string result) {
      BigInteger bigintA = BigFromString(m1);
      AssertBigIntegersEqual(result, bigintA << m2);
      m2 = -m2;
      AssertBigIntegersEqual(result, bigintA >> m2);
    }

    public static void DoTestShiftRight(string m1, int m2, string result) {
      BigInteger bigintA = BigFromString(m1);
      AssertBigIntegersEqual(result, bigintA >> m2);
      m2 = -m2;
      AssertBigIntegersEqual(result, bigintA << m2);
    }

    public static void DoTestShiftRight2(string m1, int m2, BigInteger result) {
      BigInteger bigintA = BigFromString(m1);
      TestCommon.CompareTestEqualAndConsistent(result, bigintA >> m2);
      m2 = -m2;
      TestCommon.CompareTestEqualAndConsistent(result, bigintA << m2);
    }
    [Test]
    public void TestShiftLeft() {
      BigInteger bigint = BigInteger.One;
      bigint <<= 100;
      TestCommon.CompareTestEqualAndConsistent(bigint << 12, bigint >> -12);
      TestCommon.CompareTestEqualAndConsistent(bigint << -12, bigint >> 12);
      var r = new FastRandom();
      for (var i = 0; i < 1000; ++i) {
        BigInteger bigintA = RandomBigInteger(r);
        BigInteger bigintB = bigintA;
        for (int j = 0; j < 100; ++j) {
          BigInteger ba = bigintA;
          ba <<= j;
          TestCommon.CompareTestEqualAndConsistent(bigintB, ba);
          int negj = -j;
          ba = bigintA;
          ba >>= negj;
          TestCommon.CompareTestEqualAndConsistent(bigintB, ba);
          bigintB *= (BigInteger)2;
        }
      }
    }
    [Test]
    public void TestShiftRight() {
      BigInteger bigint = BigInteger.One;
      bigint <<= 80;
      TestCommon.CompareTestEqualAndConsistent(bigint << 12, bigint >> -12);
      TestCommon.CompareTestEqualAndConsistent(bigint << -12, bigint >> 12);
      var r = new FastRandom();
      BigInteger minusone = BigInteger.Zero;
      minusone -= BigInteger.One;
      for (var i = 0; i < 1000; ++i) {
        int smallint = r.NextValue(0x7fffffff);
        var bigintA = (BigInteger)smallint;
        string str = bigintA.ToString();
        for (int j = 32; j < 80; ++j) {
          DoTestShiftRight2(str, j, BigInteger.Zero);
          DoTestShiftRight2("-" + str, j, minusone);
        }
      }
      for (var i = 0; i < 1000; ++i) {
        BigInteger bigintA = RandomBigInteger(r);
        bigintA = BigInteger.Abs(bigintA);
        BigInteger bigintB = bigintA;
        for (int j = 0; j < 100; ++j) {
          BigInteger ba = bigintA;
          ba >>= j;
          TestCommon.CompareTestEqualAndConsistent(bigintB, ba);
          int negj = -j;
          ba = bigintA;
          ba <<= negj;
          TestCommon.CompareTestEqualAndConsistent(bigintB, ba);
          bigintB /= (BigInteger)2;
        }
      }
    }
    [Test]
    public void TestSign() {
      // not implemented yet
    }
    [Test]
    public void TestSqrtWithRemainder() {
      // not implemented yet
    }
    [Test]
    public void TestSubtract() {
      // not implemented yet
    }
    [Test]
    public void TestToByteArray() {
      // not implemented yet
    }

    private static string ToUpperCaseAscii(string str) {
      if (str == null) {
        return null;
      }
      int len = str.Length;
      var c = (char)0;
      var hasLowerCase = false;
      for (var i = 0; i < len; ++i) {
        c = str[i];
        if (c >= 'a' && c <= 'z') {
          hasLowerCase = true;
          break;
        }
      }
      if (!hasLowerCase) {
        return str;
      }
      var builder = new StringBuilder();
      for (var i = 0; i < len; ++i) {
        c = str[i];
        if (c >= 'a' && c <= 'z') {
          builder.Append((char)(c - 0x20));
        } else {
          builder.Append(c);
        }
      }
      return builder.ToString();
    }

    [Test]
    public void TestToString() {
      var bi = (BigInteger)3;
      AssertBigIntegersEqual("3", bi);
      var negseven = (BigInteger)(-7);
      AssertBigIntegersEqual("-7", negseven);
      var other = (BigInteger)(-898989);
      AssertBigIntegersEqual("-898989", other);
      other = (BigInteger)898989;
      AssertBigIntegersEqual("898989", other);
      var r = new FastRandom();
      for (var i = 0; i < 1000; ++i) {
        BigInteger bigintA = RandomBigInteger(r);
        String s = bigintA.ToString();
        BigInteger big2 = BigFromString(s);
        Assert.AreEqual(big2.ToString(), s);
      }
    }
    [Test]
    public void TestValueOf() {
      // not implemented yet
    }
    [Test]
    public void TestXor() {
      // not implemented yet
    }
    [Test]
    public void TestZero() {
      // not implemented yet
      {
        string stringTemp = BigInteger.Zero.ToString();
        Assert.AreEqual(
          "0",
          stringTemp);
      }
    }

////#if true
    [Test]
    public void TestMiscellaneous() {
      Assert.AreEqual(1, BigInteger.Zero.getDigitCount());
      var minValue = (BigInteger)Int32.MinValue;
      BigInteger minValueTimes2 = minValue + (BigInteger)minValue;
      Assert.AreEqual(Int32.MinValue, (int)minValue);
      try {
        Console.WriteLine((int)minValueTimes2);
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      BigInteger verybig = BigInteger.One << 80;
      try {
        Console.WriteLine((int)verybig);
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Console.WriteLine((long)verybig);
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.PowBigIntVar(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.pow(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        (BigInteger.Zero - BigInteger.One).PowBigIntVar(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.IsFalse(BigInteger.One.Equals(BigInteger.Zero));
      Assert.IsFalse(verybig.Equals(BigInteger.Zero));
      Assert.IsFalse(BigInteger.One.Equals(BigInteger.Zero - BigInteger.One));
      Assert.AreEqual(1, BigInteger.One.CompareTo(null));
      BigInteger[] tmpsqrt = BigInteger.Zero.sqrtWithRemainder();
      Assert.AreEqual(BigInteger.Zero, tmpsqrt[0]);
    }

    [Test]
    public void TestExceptions() {
      try {
        BigFromString(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        BigInteger.Zero.testBit(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigFromString("x11");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigFromString(".");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigFromString("..");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigFromString("e200");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        BigInteger.One.mod((BigInteger)(-1));
        Assert.Fail("Should have failed");
      } catch (ArithmeticException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.add(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.subtract(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.divide(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.divide(BigInteger.Zero);
        Assert.Fail("Should have failed");
      } catch (DivideByZeroException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.remainder(BigInteger.Zero);
        Assert.Fail("Should have failed");
      } catch (DivideByZeroException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.mod(BigInteger.Zero);
        Assert.Fail("Should have failed");
      } catch (DivideByZeroException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.remainder(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(BigInteger.One, ((BigInteger)13).mod((BigInteger)4));
      Assert.AreEqual((BigInteger)3, ((BigInteger)(-13)).mod((BigInteger)4));
    }

////#endif
  }
}
