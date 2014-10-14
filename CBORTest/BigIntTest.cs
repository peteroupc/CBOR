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
    // Test some specific cases
    [TestMethod]
    public void TestSpecificCases() {
  TestCommon.DoTestMultiply(
"39258416159456516340113264558732499166970244380745050",
"39258416159456516340113264558732499166970244380745051",
"1541223239349076530208308657654362309553698742116222355477449713742236585667505604058123112521437480247550" );
      TestCommon.DoTestMultiply(
  "5786426269322750882632312999752639738983363095641642905722171221986067189342123124290107105663618428969517616421742429671402859775667602123564",
  "331378991485809774307751183645559883724387697397707434271522313077548174328632968616330900320595966360728317363190772921",
  "1917500101435169880779183578665955372346028226046021044867189027856189131730889958057717187493786883422516390996639766012958050987359732634213213442579444095928862861132583117668061032227577386757036981448703231972963300147061503108512300577364845823910107210444" );
      TestCommon.DoTestDivide(
"9999999999999999999999",
"281474976710655",
"35527136");
    }

    [TestMethod]
    public void TestToString() {
      var r = new FastRandom();
      for (var i = 0; i < 1000; ++i) {
        BigInteger bigintA = RandomObjects.RandomBigInteger(r);
        String s = bigintA.ToString();
        BigInteger big2 = BigInteger.fromString(s);
        Assert.AreEqual(big2.ToString(), s);
      }
    }

    [TestMethod]
    public void TestMultiplyDivide() {
      var r = new FastRandom();
      for (var i = 0; i < 4000; ++i) {
        BigInteger bigintA = RandomObjects.RandomBigInteger(r);
        BigInteger bigintB = RandomObjects.RandomBigInteger(r);
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
    }

    [TestMethod]
    public void TestPow() {
      var r = new FastRandom();
      for (var i = 0; i < 200; ++i) {
        int power = 1 + r.NextValue(8);
        BigInteger bigintA = RandomObjects.RandomBigInteger(r);
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
        BigInteger bigintA = RandomObjects.RandomBigInteger(r);
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
        BigInteger bigintA = RandomObjects.RandomBigInteger(r);
        BigInteger bigintB = RandomObjects.RandomBigInteger(r);
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
  }
}
