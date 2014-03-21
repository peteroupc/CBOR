package com.upokecenter.test;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 12/1/2013
 * Time: 11:22 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;

  public class BigIntTests
  {
    // Test some specific cases
    @Test
    public void TestSpecificCases() {
      TestCommon.DoTestMultiply("39258416159456516340113264558732499166970244380745050", "39258416159456516340113264558732499166970244380745051", "1541223239349076530208308657654362309553698742116222355477449713742236585667505604058123112521437480247550");
      TestCommon.DoTestMultiply(
        "5786426269322750882632312999752639738983363095641642905722171221986067189342123124290107105663618428969517616421742429671402859775667602123564",
        "331378991485809774307751183645559883724387697397707434271522313077548174328632968616330900320595966360728317363190772921",
        "1917500101435169880779183578665955372346028226046021044867189027856189131730889958057717187493786883422516390996639766012958050987359732634213213442579444095928862861132583117668061032227577386757036981448703231972963300147061503108512300577364845823910107210444");
      TestCommon.DoTestDivide("9999999999999999999999", "281474976710655", "35527136");
    }

    @Test
    public void TestToString() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 1000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        String s = bigintA.toString();
        BigInteger big2 = BigInteger.fromString(s);
        Assert.assertEquals(big2.toString(), s);
      }
    }

    @Test
    public void TestShiftLeft() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 1000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        BigInteger bigintB = bigintA;
        for (int j = 0; j < 100; ++j) {
          BigInteger ba = bigintA;
          ba=ba.shiftLeft(j);
          Assert.assertEquals(bigintB, ba);
          int negj = -j;
          ba = bigintA;
          ba=ba.shiftRight(negj);
          Assert.assertEquals(bigintB, ba);
          bigintB=bigintB.multiply(BigInteger.valueOf(2));
        }
      }
    }

    @Test
    public void TestShiftRight() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 1000; ++i) {
        int smallint = r.NextValue(0x7FFFFFFF);
        BigInteger bigintA = BigInteger.valueOf(smallint);
        String str = bigintA.toString();
        for (int j = 32; j < 80; ++j) {
          TestCommon.DoTestShiftRight(str, j, "0");
          TestCommon.DoTestShiftRight("-" + str, j, "-1");
        }
      }
      for (int i = 0; i < 1000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        bigintA = (bigintA).abs();
        BigInteger bigintB = bigintA;
        for (int j = 0; j < 100; ++j) {
          BigInteger ba = bigintA;
          ba=ba.shiftRight(j);
          Assert.assertEquals(bigintB, ba);
          int negj = -j;
          ba = bigintA;
          ba=ba.shiftLeft(negj);
          Assert.assertEquals(bigintB, ba);
          bigintB=bigintB.divide(BigInteger.valueOf(2));
        }
      }
    }

    @Test
    public void TestDigitCount() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 1000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        String str = (bigintA).abs().toString();
        Assert.assertEquals(str.length(), bigintA.getDigitCount());
      }
    }

    @Test
    public void TestMultiply() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 1000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        BigInteger bigintB = bigintA .add(BigInteger.ONE);
        BigInteger bigintC = bigintA.multiply(bigintB);
        // Test near-squaring
        if (bigintA.signum()==0 || bigintB.signum()==0) {
          Assert.assertEquals(BigInteger.ZERO, bigintC);
        }
        if (bigintA.equals(BigInteger.ONE)) {
          Assert.assertEquals(bigintB, bigintC);
        }
        if (bigintB.equals(BigInteger.ONE)) {
          Assert.assertEquals(bigintA, bigintC);
        }
        bigintB = bigintA;
        // Test squaring
        bigintC = bigintA.multiply(bigintB);
        if (bigintA.signum()==0 || bigintB.signum()==0) {
          Assert.assertEquals(BigInteger.ZERO, bigintC);
        }
        if (bigintA.equals(BigInteger.ONE)) {
          Assert.assertEquals(bigintB, bigintC);
        }
        if (bigintB.equals(BigInteger.ONE)) {
          Assert.assertEquals(bigintA, bigintC);
        }
      }
    }

    @Test
    public void TestMultiplyDivide() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 4000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        BigInteger bigintB = CBORTest.RandomBigInteger(r);
        // Test that A*B/A = B and A*B/B = A
        BigInteger bigintC = bigintA.multiply(bigintB);
        BigInteger bigintRem;
        BigInteger bigintE;
        BigInteger bigintD;
        if (bigintB.signum()!=0) {
          {
BigInteger[] divrem=(bigintC).divideAndRemainder(bigintB);
bigintD=divrem[0];
bigintRem=divrem[1]; }
          if (!bigintD.equals(bigintA)) {
            Assert.assertEquals("TestMultiplyDivide " + bigintA + "; " + bigintB + ";\n" + bigintC,bigintA,bigintD);
          }
          if (bigintRem.signum()!=0) {
            Assert.assertEquals("TestMultiplyDivide " + bigintA + "; " + bigintB,BigInteger.ZERO,bigintRem);
          }
          bigintE = bigintC.divide(bigintB);
          if (!bigintD.equals(bigintE)) {
            // Testing that divideWithRemainder and division method return the same value
            Assert.assertEquals("TestMultiplyDivide " + bigintA + "; " + bigintB + ";\n" + bigintC,bigintD,bigintE);
          }
          bigintE = bigintC.remainder(bigintB);
          if (!bigintRem.equals(bigintE)) {
            Assert.assertEquals("TestMultiplyDivide " + bigintA + "; " + bigintB + ";\n" + bigintC,bigintRem,bigintE);
          }
          if (bigintE.signum() > 0 && !bigintC.mod(bigintB).equals(bigintE)) {
            Assert.fail("TestMultiplyDivide " + bigintA + "; " + bigintB + ";\n" + bigintC);
          }
        }
        if (bigintA.signum()!=0) {
          {
BigInteger[] divrem=(bigintC).divideAndRemainder(bigintA);
bigintD=divrem[0];
bigintRem=divrem[1]; }
          if (!bigintD.equals(bigintB)) {
            Assert.assertEquals("TestMultiplyDivide " + bigintA + "; " + bigintB,bigintB,bigintD);
          }
          if (bigintRem.signum()!=0) {
            Assert.assertEquals("TestMultiplyDivide " + bigintA + "; " + bigintB,BigInteger.ZERO,bigintRem);
          }
        }
        if (bigintB.signum()!=0) {
          {
BigInteger[] divrem=(bigintA).divideAndRemainder(bigintB);
bigintC=divrem[0];
bigintRem=divrem[1]; }
          bigintD = bigintB.multiply(bigintC);
          bigintD=bigintD.add(bigintRem);
          if (!bigintD.equals(bigintA)) {
            Assert.assertEquals("TestMultiplyDivide " + bigintA + "; " + bigintB,bigintA,bigintD);
          }
        }
      }
    }

    @Test
    public void TestPow() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 200; ++i) {
        int power = 1 + r.NextValue(8);
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        BigInteger bigintB = bigintA;
        for (int j = 1; j < power; ++j) {
          bigintB=bigintB.multiply(bigintA);
        }
        TestCommon.DoTestPow(bigintA.toString(), power, bigintB.toString());
      }
    }

    @Test
    public void TestSquareRoot() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 10000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        if (bigintA.signum() < 0) {
          bigintA=bigintA.negate();
        }
        if (bigintA.signum() == 0) {
          bigintA = BigInteger.ONE;
        }
        BigInteger sr = bigintA.sqrt();
        BigInteger srsqr = sr.multiply(sr);
        sr=sr.add(BigInteger.ONE);
        BigInteger sronesqr = sr.multiply(sr);
        if (srsqr.compareTo(bigintA) > 0) {
          Assert.fail(srsqr + " not " + bigintA + " or less (TestSqrt, sqrt=" + sr + ")");
        }
        if (sronesqr.compareTo(bigintA) <= 0) {
          Assert.fail(srsqr + " not greater than " + bigintA + " (TestSqrt, sqrt=" + sr + ")");
        }
      }
    }

    @Test
    public void TestSmallIntDivide() {
      int a, b;
      FastRandom fr = new FastRandom();
      for (int i = 0; i < 10000; ++i) {
        a = fr.NextValue(0x1000000);
        b = fr.NextValue(0x1000000);
        if (b == 0) {
          continue;
        }
        int c = a / b;
        BigInteger bigintA = BigInteger.valueOf(a);
        BigInteger bigintB = BigInteger.valueOf(b);
        BigInteger bigintC = bigintA.divide(bigintB);
        Assert.assertEquals(bigintC.intValue(), c);
      }
    }

    @Test
    public void TestMiscellaneous() {
     Assert.assertEquals(1, BigInteger.ZERO.getDigitCount());
     BigInteger minValue = BigInteger.valueOf(Integer.MIN_VALUE);
      BigInteger minValueTimes2 = minValue.add(minValue);
      Assert.assertEquals(Integer.MIN_VALUE, minValue.intValue());
      try {
        System.out.println(minValueTimes2.intValue());
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      BigInteger verybig = BigInteger.ONE.shiftLeft(80);
      try {
        System.out.println(verybig.intValue());
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        System.out.println(verybig.longValue());
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.ONE.PowBigIntVar(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.ONE.divideAndRemainder(BigInteger.ZERO);
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.ONE.pow(-1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        (BigInteger.ZERO.subtract(BigInteger.ONE)).PowBigIntVar(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      if(BigInteger.ONE.equals(BigInteger.ZERO))Assert.fail();
      if(verybig.equals(BigInteger.ZERO))Assert.fail();
      if(BigInteger.ONE.equals(BigInteger.ZERO.subtract(BigInteger.ONE)))Assert.fail();
      Assert.assertEquals(1, BigInteger.ONE.compareTo(null));
      BigInteger[] tmpsqrt = BigInteger.ZERO.sqrtWithRemainder();
      Assert.assertEquals(BigInteger.ZERO, tmpsqrt[0]);
    }

    @Test
    public void TestExceptions() {
      try {
        BigInteger.fromString("xyz");
        Assert.fail("Should have failed");
      } catch (NumberFormatException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.fromString("");
        Assert.fail("Should have failed");
      } catch (NumberFormatException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }

      try {
        BigInteger.fromSubstring(null, 0, 1);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.fromString(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }

      try {
        BigInteger.ZERO.testBit(-1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.fromByteArray(null, false);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }

      try {
        BigInteger.fromSubstring("123", -1, 2);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.fromSubstring("123", 4, 2);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.fromSubstring("123", 1, -1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.fromSubstring("123", 1, 4);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.fromSubstring("123", 1, 0);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.fromSubstring("123", 2, 1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.fromString("x11");
        Assert.fail("Should have failed");
      } catch (NumberFormatException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.fromString(".");
        Assert.fail("Should have failed");
      } catch (NumberFormatException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.fromString("..");
        Assert.fail("Should have failed");
      } catch (NumberFormatException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.fromString("e200");
        Assert.fail("Should have failed");
      } catch (NumberFormatException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }

      try {
        BigInteger.ONE.mod(BigInteger.valueOf(-1));
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.ONE.add(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.ONE.subtract(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.ONE.multiply(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.ONE.divide(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.ONE.divide(BigInteger.ZERO);
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.ONE.remainder(BigInteger.ZERO);
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.ONE.mod(BigInteger.ZERO);
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.ONE.remainder(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.ONE.mod(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        BigInteger.ONE.divideAndRemainder(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      Assert.assertEquals(BigInteger.ONE, (BigInteger.valueOf(13)).mod(BigInteger.valueOf(4)));
      Assert.assertEquals(BigInteger.valueOf(3), (BigInteger.valueOf(-13)).mod(BigInteger.valueOf(4)));
      try {
        (BigInteger.valueOf(13)).mod(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        (BigInteger.valueOf(13)).mod(BigInteger.valueOf(-4));
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        (BigInteger.valueOf(-13)).mod(BigInteger.valueOf(-4));
        Assert.fail("Should have failed");
      } catch (ArithmeticException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestAddSubtract() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 10000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        BigInteger bigintB = CBORTest.RandomBigInteger(r);
        BigInteger bigintC = bigintA.add(bigintB);
        BigInteger bigintD = bigintC.subtract(bigintB);
        if (!bigintD.equals(bigintA)) {
          Assert.assertEquals("TestAddSubtract " + bigintA + "; " + bigintB,bigintA,bigintD);
        }
        bigintD = bigintC.subtract(bigintA);
        if (!bigintD.equals(bigintB)) {
          Assert.assertEquals("TestAddSubtract " + bigintA + "; " + bigintB,bigintB,bigintD);
        }
        bigintC = bigintA.subtract(bigintB);
        bigintD = bigintC.add(bigintB);
        if (!bigintD.equals(bigintA)) {
          Assert.assertEquals("TestAddSubtract " + bigintA + "; " + bigintB,bigintA,bigintD);
        }
      }
    }

    @Test
    public void TestBitLength() {
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

    public static int ModPow(int x, int pow, int mod) {
      if (x < 0) {
        throw new IllegalArgumentException("x (" + Long.toString((long)x) + ") is less than " + "0");
      }
      if (pow <= 0) {
        throw new IllegalArgumentException("pow (" + Long.toString((long)pow) + ") is not greater than " + "0");
      }
      if (mod <= 0) {
        throw new IllegalArgumentException("mod (" + Long.toString((long)mod) + ") is not greater than " + "0");
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

    public static boolean IsPrime(int n) {
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

    @Test
    public void TestGcd() {
      int prime = 0;
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 1000; ++i) {
        while (true) {
          prime = rand.NextValue(0x7FFFFFFF);
          prime |= 1;
          if (IsPrime(prime)) {
            break;
          }
        }
        BigInteger bigprime = BigInteger.valueOf(prime);
        BigInteger ba = CBORTest.RandomBigInteger(rand);
        if (ba.signum()==0) {
          continue;
        }
        ba=ba.multiply(bigprime);
        Assert.assertEquals(bigprime, bigprime.gcd(ba));
      }
    }
  }
