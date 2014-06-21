/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO;

namespace Test {
  [TestClass]
  public class BigIntTest
  {
    [TestMethod]
    public void TestBigIntegerFromByteArray() {
      Assert.AreEqual(BigInteger.Zero, BigInteger.fromByteArray(new byte[] { }, false));
    }

    // Test some specific cases
    [TestMethod]
    public void TestSpecificCases() {
      TestCommon.DoTestMultiply("39258416159456516340113264558732499166970244380745050", "39258416159456516340113264558732499166970244380745051", "1541223239349076530208308657654362309553698742116222355477449713742236585667505604058123112521437480247550");
      TestCommon.DoTestMultiply(
        "5786426269322750882632312999752639738983363095641642905722171221986067189342123124290107105663618428969517616421742429671402859775667602123564",
        "331378991485809774307751183645559883724387697397707434271522313077548174328632968616330900320595966360728317363190772921",
        "1917500101435169880779183578665955372346028226046021044867189027856189131730889958057717187493786883422516390996639766012958050987359732634213213442579444095928862861132583117668061032227577386757036981448703231972963300147061503108512300577364845823910107210444");
      TestCommon.DoTestDivide("9999999999999999999999", "281474976710655", "35527136");
    }

    [TestMethod]
    public void TestToString() {
      var r = new FastRandom();
      for (var i = 0; i < 1000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        String s = bigintA.ToString();
        BigInteger big2 = BigInteger.fromString(s);
        Assert.AreEqual(big2.ToString(), s);
      }
    }

    [TestMethod]
    public void TestShiftLeft() {
      var r = new FastRandom();
      for (var i = 0; i < 1000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        BigInteger bigintB = bigintA;
        for (int j = 0; j < 100; ++j) {
          BigInteger ba = bigintA;
          ba <<= j;
          Assert.AreEqual(bigintB, ba);
          int negj = -j;
          ba = bigintA;
          ba >>= negj;
          Assert.AreEqual(bigintB, ba);
          bigintB *= (BigInteger)2;
        }
      }
    }

    [TestMethod]
    public void TestShiftRight() {
      var r = new FastRandom();
      for (var i = 0; i < 1000; ++i) {
        int smallint = r.NextValue(0x7fffffff);
        var bigintA = (BigInteger)smallint;
        string str = bigintA.ToString();
        for (int j = 32; j < 80; ++j) {
          TestCommon.DoTestShiftRight(str, j, "0");
          TestCommon.DoTestShiftRight("-" + str, j, "-1");
        }
      }
      for (var i = 0; i < 1000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        bigintA = BigInteger.Abs(bigintA);
        BigInteger bigintB = bigintA;
        for (int j = 0; j < 100; ++j) {
          BigInteger ba = bigintA;
          ba >>= j;
          Assert.AreEqual(bigintB, ba);
          int negj = -j;
          ba = bigintA;
          ba <<= negj;
          Assert.AreEqual(bigintB, ba);
          bigintB /= (BigInteger)2;
        }
      }
    }

    [TestMethod]
    public void TestDigitCount() {
      var r = new FastRandom();
      for (var i = 0; i < 1000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        String str = BigInteger.Abs(bigintA).ToString();
        Assert.AreEqual(str.Length, bigintA.getDigitCount());
      }
    }

    [TestMethod]
    public void TestMultiply() {
      var r = new FastRandom();
      for (var i = 0; i < 1000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
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
    }

    [TestMethod]
    public void TestMultiplyDivide() {
      var r = new FastRandom();
      for (var i = 0; i < 4000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        BigInteger bigintB = CBORTest.RandomBigInteger(r);
        // Test that A*B/A = B and A*B/B = A
        BigInteger bigintC = bigintA * (BigInteger)bigintB;
        BigInteger bigintRem;
        BigInteger bigintE;
        BigInteger bigintD;
        if (!bigintB.IsZero) {
          bigintD = BigInteger.DivRem(bigintC, bigintB, out bigintRem);
          if (!bigintD.Equals(bigintA)) {
            Assert.AreEqual(bigintA, bigintD, "TestMultiplyDivide " + bigintA + "; " + bigintB + ";\n" + bigintC);
          }
          if (!bigintRem.IsZero) {
            Assert.AreEqual(BigInteger.Zero, bigintRem, "TestMultiplyDivide " + bigintA + "; " + bigintB);
          }
          bigintE = bigintC / (BigInteger)bigintB;
          if (!bigintD.Equals(bigintE)) {
            // Testing that divideWithRemainder and division method return the same value
            Assert.AreEqual(bigintD, bigintE, "TestMultiplyDivide " + bigintA + "; " + bigintB + ";\n" + bigintC);
          }
          bigintE = bigintC % (BigInteger)bigintB;
          if (!bigintRem.Equals(bigintE)) {
            Assert.AreEqual(bigintRem, bigintE, "TestMultiplyDivide " + bigintA + "; " + bigintB + ";\n" + bigintC);
          }
          if (bigintE.Sign > 0 && !bigintC.mod(bigintB).Equals(bigintE)) {
            Assert.Fail("TestMultiplyDivide " + bigintA + "; " + bigintB + ";\n" + bigintC);
          }
        }
        if (!bigintA.IsZero) {
          bigintD = BigInteger.DivRem(bigintC, bigintA, out bigintRem);
          if (!bigintD.Equals(bigintB)) {
            Assert.AreEqual(bigintB, bigintD, "TestMultiplyDivide " + bigintA + "; " + bigintB);
          }
          if (!bigintRem.IsZero) {
            Assert.AreEqual(BigInteger.Zero, bigintRem, "TestMultiplyDivide " + bigintA + "; " + bigintB);
          }
        }
        if (!bigintB.IsZero) {
          bigintC = BigInteger.DivRem(bigintA, bigintB, out bigintRem);
          bigintD = bigintB * (BigInteger)bigintC;
          bigintD += (BigInteger)bigintRem;
          if (!bigintD.Equals(bigintA)) {
            Assert.AreEqual(bigintA, bigintD, "TestMultiplyDivide " + bigintA + "; " + bigintB);
          }
        }
      }
    }

    [TestMethod]
    public void TestPow() {
      var r = new FastRandom();
      for (var i = 0; i < 200; ++i) {
        int power = 1 + r.NextValue(8);
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        BigInteger bigintB = bigintA;
        for (int j = 1; j < power; ++j) {
          bigintB *= bigintA;
        }
        TestCommon.DoTestPow(bigintA.ToString(), power, bigintB.ToString());
      }
    }

    [TestMethod]
    public void TestSquareRoot() {
      var r = new FastRandom();
      for (var i = 0; i < 10000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
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
          Assert.Fail(srsqr + " not " + bigintA + " or less (TestSqrt, sqrt=" + sr + ")");
        }
        if (sronesqr.CompareTo(bigintA) <= 0) {
          Assert.Fail(srsqr + " not greater than " + bigintA + " (TestSqrt, sqrt=" + sr + ")");
        }
      }
    }

    [TestMethod]
    public void TestSmallIntDivide() {
      int a, b;
      var fr = new FastRandom();
      for (var i = 0; i < 10000; ++i) {
        a = fr.NextValue(0x1000000);
        b = fr.NextValue(0x1000000);
        if (b == 0) {
          continue;
        }
        int c = a / b;
        var bigintA = (BigInteger)a;
        var bigintB = (BigInteger)b;
        BigInteger bigintC = bigintA / (BigInteger)bigintB;
        Assert.AreEqual((int)bigintC, c);
      }
    }

    [TestMethod]
    public void TestMiscellaneous() {
      Assert.AreEqual(1, BigInteger.Zero.getDigitCount());
      var minValue = (BigInteger)Int32.MinValue;
      BigInteger minValueTimes2 = minValue + (BigInteger)minValue;
      Assert.AreEqual(Int32.MinValue, (int)minValue);
      try {
        Console.WriteLine((int)minValueTimes2);
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      BigInteger verybig = BigInteger.One << 80;
      try {
        Console.WriteLine((int)verybig);
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Console.WriteLine((long)verybig);
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.PowBigIntVar(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.divideAndRemainder(BigInteger.Zero);
        Assert.Fail("Should have failed");
      } catch (DivideByZeroException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.pow(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        (BigInteger.Zero - BigInteger.One).PowBigIntVar(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
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

    [TestMethod]
    public void TestExceptions() {
      try {
        BigInteger.fromString("xyz");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromString(String.Empty);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        BigInteger.fromSubstring(null, 0, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromString(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        BigInteger.Zero.testBit(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromByteArray(null, false);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        BigInteger.fromSubstring("123", -1, 2);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromSubstring("123", 4, 2);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromSubstring("123", 1, -1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromSubstring("123", 1, 4);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromSubstring("123", 1, 0);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromSubstring("123", 2, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromString("x11");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromString(".");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromString("..");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromString("e200");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        BigInteger.One.mod((BigInteger)(-1));
        Assert.Fail("Should have failed");
      } catch (ArithmeticException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.add(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.subtract(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.multiply(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.divide(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.divide(BigInteger.Zero);
        Assert.Fail("Should have failed");
      } catch (DivideByZeroException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.remainder(BigInteger.Zero);
        Assert.Fail("Should have failed");
      } catch (DivideByZeroException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.mod(BigInteger.Zero);
        Assert.Fail("Should have failed");
      } catch (DivideByZeroException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.remainder(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(BigInteger.One, ((BigInteger)13).mod((BigInteger)4));
      Assert.AreEqual((BigInteger)3, ((BigInteger)(-13)).mod((BigInteger)4));
      try {
        ((BigInteger)13).mod(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ((BigInteger)13).mod((BigInteger)(-4));
        Assert.Fail("Should have failed");
      } catch (ArithmeticException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ((BigInteger)(-13)).mod((BigInteger)(-4));
        Assert.Fail("Should have failed");
      } catch (ArithmeticException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [TestMethod]
    public void TestAddSubtract() {
      var r = new FastRandom();
      for (var i = 0; i < 10000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        BigInteger bigintB = CBORTest.RandomBigInteger(r);
        BigInteger bigintC = bigintA + (BigInteger)bigintB;
        BigInteger bigintD = bigintC - (BigInteger)bigintB;
        if (!bigintD.Equals(bigintA)) {
          Assert.AreEqual(bigintA, bigintD, "TestAddSubtract " + bigintA + "; " + bigintB);
        }
        bigintD = bigintC - (BigInteger)bigintA;
        if (!bigintD.Equals(bigintB)) {
          Assert.AreEqual(bigintB, bigintD, "TestAddSubtract " + bigintA + "; " + bigintB);
        }
        bigintC = bigintA - (BigInteger)bigintB;
        bigintD = bigintC + (BigInteger)bigintB;
        if (!bigintD.Equals(bigintA)) {
          Assert.AreEqual(bigintA, bigintD, "TestAddSubtract " + bigintA + "; " + bigintB);
        }
      }
    }

    [TestMethod]
    public void TestBitLength() {
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

    public static int ModPow(int x, int pow, int mod) {
      if (x < 0) {
        throw new ArgumentException("x (" + Convert.ToString((int)x, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (pow <= 0) {
        throw new ArgumentException("pow (" + Convert.ToString((int)pow, System.Globalization.CultureInfo.InvariantCulture) + ") is not greater than " + "0");
      }
      if (mod <= 0) {
        throw new ArgumentException("mod (" + Convert.ToString((int)mod, System.Globalization.CultureInfo.InvariantCulture) + ") is not greater than " + "0");
      }
      int r = 1;
      int v = x;
      while (pow != 0) {
        if ((pow & 1) != 0) {
          r = (int)(((long)r * (long)v) % mod);
        }
        pow >>= 1;
        if (pow != 0) {
          v = (int)(((long)v * (long)v) % mod);
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
        Assert.AreEqual(bigprime, BigInteger.GreatestCommonDivisor(bigprime, ba));
      }
    }
  }
}
