package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */


import java.math.*;

  final class DigitShiftAccumulator implements IShiftAccumulator {
    int bitLeftmost = 0;

    /**
     * Gets whether the last discarded digit was set.
     */
    public int getLastDiscardedDigit() { return bitLeftmost; }
    int bitsAfterLeftmost = 0;

    /**
     * Gets whether any of the discarded digits to the right of the last one
     * was set.
     */
    public int getOlderDiscardedDigits() { return bitsAfterLeftmost; }
    BigInteger shiftedBigInt;
    long knownBitLength;

    /**
     * Gets the length of the shifted value in digits.
     */
    public long getDigitLength() {
        if (knownBitLength < 0) {
          knownBitLength = CalcKnownBitLength();
        }
        return knownBitLength;
      }
    long shiftedLong;
    boolean isSmall;

    /**
     * 
     */
    public BigInteger getShiftedInt() {
        if (isSmall)
          return BigInteger.valueOf(shiftedLong);
        else
          return shiftedBigInt;
      }
    /**
     * 
     */
    public long getShiftedIntSmall() {
        if (isSmall)
          return shiftedLong;
        else
          return shiftedBigInt.longValue();
      }
    FastInteger discardedBitCount;

    /**
     * 
     */
    public FastInteger getDiscardedDigitCount() { return discardedBitCount; }
    private static BigInteger Int64MaxValue = BigInteger.valueOf(Long.MAX_VALUE);

    public DigitShiftAccumulator(BigInteger bigint,
      int lastDiscarded,
      int olderDiscarded
      ){
 this(bigint);
        bitsAfterLeftmost = (olderDiscarded != 0) ? 1 : 0;
        bitLeftmost = lastDiscarded;
    }

    public DigitShiftAccumulator(BigInteger bigint) {
      if (bigint.signum() < 0)
        throw new IllegalArgumentException("bigint is negative");
      discardedBitCount = new FastInteger();
      if (bigint.compareTo(Int64MaxValue) <= 0) {
        shiftedLong = bigint.longValue();
        isSmall = true;
        knownBitLength = -1;
      } else {
        shiftedBigInt = bigint;
        isSmall = false;
        knownBitLength = -1;
      }
    }
    
public DigitShiftAccumulator(long longInt) {
      if (longInt < 0)
        throw new IllegalArgumentException("longInt is negative");
      shiftedLong = longInt;
      discardedBitCount = new FastInteger();
      isSmall = true;
      knownBitLength = -1;
    }
    private static BigInteger FastParseBigInt(String str, int offset, int length) {
      MutableBigInteger mbi = new MutableBigInteger();
      for (int i = 0; i < length; i++) {
        int digit = (int)(str.charAt(offset + i) - '0');
        mbi.Multiply(10).Add(digit);
      }
      return mbi.ToBigInteger();
    }

    private static long FastParseLong(String str, int offset, int length) {
      // Assumes the String is length 18 or less and contains
      // only the digits '0' through '9'
      if (length > 18)
        throw new IllegalArgumentException();
      long ret = 0;
      for (int i = 0; i < length; i++) {
        int digit = (int)(str.charAt(offset + i) - '0');
        ret *= 10;
        ret += digit;
      }
      return ret;
    }
    /**
     * 
     * @param fastint A FastInteger object.
     */
    public void ShiftRight(FastInteger fastint) {
      if ((fastint) == null) throw new NullPointerException("fastint");
      if (fastint.signum() <= 0) return;
      if (fastint.CanFitInInt64()) {
        ShiftRight(fastint.AsInt64());
      } else {
        BigInteger bi = fastint.AsBigInteger();
        while (bi.signum() > 0) {
          long count = 1000000;
          if (bi.compareTo(BigInteger.valueOf(1000000)) < 0) {
            count = bi.longValue();
          }
          ShiftRight(count);
          bi=bi.subtract(BigInteger.valueOf(count));
        }
      }
    }

    private void ShiftRightBig(long digits) {
      if (digits <= 0) return;
      if (shiftedBigInt.signum()==0) {
        discardedBitCount.Add(digits);
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
        knownBitLength = 1;
        return;
      }
      String str = shiftedBigInt.toString();
      // NOTE: Will be 1 if the value is 0
      long digitLength = str.length();
      long bitDiff = 0;
      if (digits > digitLength) {
        bitDiff = digits - digitLength;
      }
      discardedBitCount.Add(digits);
      bitsAfterLeftmost |= bitLeftmost;
      long digitShift = Math.min(digitLength, digits);
      if (digits >= digitLength) {
        isSmall = true;
        shiftedLong = 0;
        knownBitLength = 1;
      } else {
        int newLength = (int)(digitLength - digitShift);
        knownBitLength = digitLength - digitShift;
        if (newLength <= 18) {
          // Fits in a long
          isSmall = true;
          shiftedLong = FastParseLong(str, 0, newLength);
        } else {
          shiftedBigInt = FastParseBigInt(str, 0, newLength);
        }
      }
      for (int i = str.length() - 1; i >= 0; i--) {
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = (int)(str.charAt(i) - '0');
        digitShift--;
        if (digitShift <= 0) {
          break;
        }
      }
      bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
      if (bitDiff > 0) {
        // Shifted more digits than the digit length
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
      }
    }
    private static BigInteger[] BigIntPowersOfTen = new BigInteger[]{
      BigInteger.ONE, BigInteger.TEN, BigInteger.valueOf(100), BigInteger.valueOf(1000), BigInteger.valueOf(10000), BigInteger.valueOf(100000), BigInteger.valueOf(1000000), BigInteger.valueOf(10000000), BigInteger.valueOf(100000000), BigInteger.valueOf(1000000000),
      BigInteger.valueOf(10000000000L), BigInteger.valueOf(100000000000L), BigInteger.valueOf(1000000000000L), BigInteger.valueOf(10000000000000L),
      BigInteger.valueOf(100000000000000L), BigInteger.valueOf(1000000000000000L), BigInteger.valueOf(10000000000000000L),
      BigInteger.valueOf(100000000000000000L), BigInteger.valueOf(1000000000000000000L)
    };

    /**
     * Shifts a number until it reaches the given number of digits, gathering
     * information on whether the last digit discarded is set and whether
     * the discarded digits to the right of that digit are set. Assumes that
     * the big integer being shifted is positive.
     */
    private void ShiftToBitsBig(long digits) {
      String str = shiftedBigInt.toString();
      // NOTE: Will be 1 if the value is 0
      long digitLength = str.length();
      knownBitLength = digitLength;
      // Shift by the difference in digit length
      if (digitLength > digits) {
        long digitShift = digitLength - digits;
        long bitShiftCount = digitShift;
        int newLength = (int)(digitLength - digitShift);
        discardedBitCount.Add(digitShift);
        for (int i = str.length() - 1; i >= 0; i--) {
          bitsAfterLeftmost |= bitLeftmost;
          bitLeftmost = (int)(str.charAt(i) - '0');
          digitShift--;
          if (digitShift <= 0) {
            break;
          }
        }
        knownBitLength = digits;
        if (newLength <= 18) {
          // Fits in a long
          isSmall = true;
          shiftedLong = FastParseLong(str, 0, newLength);
        } else if (bitShiftCount <= 18) {
          BigInteger bigpow = BigIntPowersOfTen[(int)bitShiftCount];
          shiftedBigInt=shiftedBigInt.divide(bigpow);
        } else {
          shiftedBigInt = FastParseBigInt(str, 0, newLength);
        }
        bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
      }
    }


    /**
     * Shifts a number to the right, gathering information on whether the
     * last digit discarded is set and whether the discarded digits to the
     * right of that digit are set. Assumes that the big integer being shifted
     * is positive.
     * @param digits A 64-bit signed integer.
     */
    public void ShiftRight(long digits) {
      if (isSmall)
        ShiftRightSmall(digits);
      else
        ShiftRightBig(digits);
    }
    private void ShiftRightSmall(long digits) {
      if (digits <= 0) return;
      if (shiftedLong == 0) {
        discardedBitCount.Add(digits);
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
        knownBitLength = 1;
        return;
      }
      knownBitLength = 0;
      long tmp = shiftedLong;
      while (tmp > 0) {
        knownBitLength++;
        tmp /= 10;
      }
      // Make sure digit length is 1 if value is 0
      if (knownBitLength == 0) knownBitLength++;
      discardedBitCount.Add(digits);
      while (digits > 0) {
        if (shiftedLong == 0) {
          bitsAfterLeftmost |= bitLeftmost;
          bitLeftmost = 0;
          knownBitLength = 1;
          break;
        } else {
          int digit = (int)(shiftedLong % 10);
          bitsAfterLeftmost |= bitLeftmost;
          bitLeftmost = digit;
          digits--;
          shiftedLong /= 10;
          knownBitLength--;
        }
      }
      bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
    }

    /**
     * Shifts a number until it reaches the given number of digits, gathering
     * information on whether the last digit discarded is set and whether
     * the discarded digits to the right of that digit are set. Assumes that
     * the big integer being shifted is positive.
     * @param digits A 64-bit signed integer.
     */
    public void ShiftToDigits(long digits) {
      if (isSmall)
        ShiftToBitsSmall(digits);
      else
        ShiftToBitsBig(digits);
    }
    private long CalcKnownBitLength() {
      if (isSmall) {
        int kb = 0;
        long tmp = shiftedLong;
        while (tmp > 0) {
          kb++;
          tmp /= 10;
        }
        return kb == 0 ? 1 : kb;
      } else {
        String str = shiftedBigInt.toString();
        return str.length();
      }
    }
    private void ShiftToBitsSmall(long digits) {
      knownBitLength = 0;
      long tmp = shiftedLong;
      while (tmp > 0) {
        knownBitLength++;
        tmp /= 10;
      }
      // Make sure digit length is 1 if value is 0
      if (knownBitLength == 0) knownBitLength++;
      if (knownBitLength > digits) {
        int digitShift = (int)(knownBitLength - digits);
        int newLength = (int)(knownBitLength - digitShift);
        knownBitLength = Math.max(1, newLength);
        discardedBitCount.Add(digitShift);
        for (int i = 0; i < digitShift; i++) {
          int digit = (int)(shiftedLong % 10);
          shiftedLong /= 10;
          bitsAfterLeftmost |= bitLeftmost;
          bitLeftmost = digit;
        }
        bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
      }
    }
  }
