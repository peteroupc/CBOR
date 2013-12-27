package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

//import java.math.*;

  final class DigitShiftAccumulator implements IShiftAccumulator {
    int bitLeftmost;

    /**
     * Gets whether the last discarded digit was set.
     */
    public int getLastDiscardedDigit() { return bitLeftmost; }
    int bitsAfterLeftmost;

    /**
     * Gets whether any of the discarded digits to the right of the last one
     * was set.
     */
    public int getOlderDiscardedDigits() { return bitsAfterLeftmost; }
    BigInteger shiftedBigInt;
    FastInteger knownBitLength;

    /**
     *
     * @return A FastInteger object.
     */
    public FastInteger GetDigitLength() {
      if (knownBitLength==null) {
        knownBitLength = CalcKnownBitLength();
      }
      return FastInteger.Copy(knownBitLength);
    }

    int shiftedSmall;
    boolean isSmall;

    /**
     *
     */
    public BigInteger getShiftedInt() {
        if (isSmall)
          return BigInteger.valueOf(shiftedSmall);
        else
          return shiftedBigInt;
      }
    FastInteger discardedBitCount;

    /**
     *
     */
    public FastInteger getDiscardedDigitCount() { return discardedBitCount; }
    private static BigInteger Int32MaxValue = BigInteger.valueOf(Integer.MAX_VALUE);

    public DigitShiftAccumulator (BigInteger bigint,
                                 int lastDiscarded,
                                 int olderDiscarded
                                ) {
      if (bigint.signum() < 0)
        throw new IllegalArgumentException("bigint is negative");
      discardedBitCount = new FastInteger(0);
      if (bigint.compareTo(Int32MaxValue) <= 0) {
        shiftedSmall = bigint.intValue();
        isSmall = true;
      } else {
        shiftedBigInt = bigint;
        isSmall = false;
      }
      bitsAfterLeftmost = (olderDiscarded != 0) ? 1 : 0;
      bitLeftmost = lastDiscarded;
    }

    private static BigInteger FastParseBigInt(String str, int offset, int length) {
      // Assumes the String contains
      // only the digits '0' through '9'
      int smallint=0;
      int mlength=Math.min(9,length);
      for (int i = 0; i < mlength; i++) {
        int digit = (int)(str.charAt(offset + i) - '0');
        smallint*=10;
        smallint+=digit;
      }
      if(mlength==length){
        return BigInteger.valueOf(smallint);
      } else {
        FastInteger mbi = new FastInteger(smallint);
        for (int i = 9; i < length;) {
          mlength=Math.min(9,length-i);
          int multer=1;
          int adder=0;
          for(int j=i;j<i+mlength;j++){
            int digit = (int)(str.charAt(offset + j) - '0');
            multer*=10;
            adder*=10;
            adder+=digit;
          }
          mbi.Multiply(multer).AddInt(adder);
          i+=mlength;
        }
        return mbi.AsBigInteger();
      }
    }

    private static int FastParseLong(String str, int offset, int length) {
      // Assumes the String is length 9 or less and contains
      // only the digits '0' through '9'
      if((length)>9)throw new IllegalArgumentException(
        "length"+" not less or equal to "+"9"+" ("+(length)+")");
      int ret = 0;
      for (int i = 0; i < length; i++) {
        int digit = (int)(str.charAt(offset + i) - '0');
        ret *= 10;
        ret += digit;
      }
      return ret;
    }

    /**
     *
     */
    public FastInteger getShiftedIntFast() {
        if (isSmall){
          return new FastInteger(shiftedSmall);
        } else {
          return FastInteger.FromBig(shiftedBigInt);
        }
      }
    /**
     *
     * @param fastint A FastInteger object.
     */
    public void ShiftRight(FastInteger fastint) {
      if ((fastint) == null) throw new NullPointerException("fastint");
      if (fastint.signum() <= 0) return;
      if (fastint.CanFitInInt32()) {
        ShiftRightInt(fastint.AsInt32());
      } else {
        BigInteger bi = fastint.AsBigInteger();
        while (bi.signum() > 0) {
          int count = 1000000;
          if (bi.compareTo(BigInteger.valueOf(1000000)) < 0) {
            count = bi.intValue();
          }
          ShiftRightInt(count);
          bi=bi.subtract(BigInteger.valueOf(count));
        }
      }
    }

    private void ShiftRightBig(int digits) {
      if (digits <= 0) return;
      if (shiftedBigInt.signum()==0) {
        discardedBitCount.AddInt(digits);
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
        knownBitLength = new FastInteger(1);
        return;
      }
      String str = shiftedBigInt.toString();
      // NOTE: Will be 1 if the value is 0
      int digitLength = str.length();
      int bitDiff = 0;
      if (digits > digitLength) {
        bitDiff = digits - digitLength;
      }
      discardedBitCount.AddInt(digits);
      bitsAfterLeftmost |= bitLeftmost;
      int digitShift = Math.min(digitLength, digits);
      if (digits >= digitLength) {
        isSmall = true;
        shiftedSmall = 0;
        knownBitLength = new FastInteger(1);
      } else {
        int newLength = (int)(digitLength - digitShift);
        knownBitLength = new FastInteger(newLength);
        if (newLength <= 9) {
          // Fits in a small number
          isSmall = true;
          shiftedSmall = FastParseLong(str, 0, newLength);
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

    /**
     * Shifts a number until it reaches the given number of digits, gathering
     * information on whether the last digit discarded is set and whether
     * the discarded digits to the right of that digit are set. Assumes that
     * the big integer being shifted is positive.
     */
    private void ShiftToBitsBig(int digits) {
      String str;
      str=shiftedBigInt.toString();
      // NOTE: Will be 1 if the value is 0
      int digitLength = str.length();
      knownBitLength = new FastInteger(digitLength);
      // Shift by the difference in digit length
      if (digitLength > digits) {
        int digitShift = digitLength - digits;
        knownBitLength.SubtractInt(digitShift);
        //System.out.println("dlen={0} dshift={1}",digitLength,digitShift);
        int newLength = (int)(digitLength - digitShift);
        if(digitShift<=Integer.MAX_VALUE)
          discardedBitCount.AddInt((int)digitShift);
        else
          discardedBitCount.AddBig(BigInteger.valueOf(digitShift));
        for (int i = str.length() - 1; i >= 0; i--) {
          bitsAfterLeftmost |= bitLeftmost;
          bitLeftmost = (int)(str.charAt(i) - '0');
          digitShift--;
          if (digitShift <= 0) {
            break;
          }
        }
        if (newLength <= 9) {
          isSmall = true;
          shiftedSmall = FastParseLong(str, 0, newLength);
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
     * @param digits A 32-bit signed integer.
     */
    public void ShiftRightInt(int digits) {
      if (isSmall)
        ShiftRightSmall(digits);
      else
        ShiftRightBig(digits);
    }
    private void ShiftRightSmall(int digits) {
      if (digits <= 0) return;
      if (shiftedSmall == 0) {
        discardedBitCount.AddInt(digits);
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
        knownBitLength = new FastInteger(1);
        return;
      }

      int kb = 0;
      int tmp = shiftedSmall;
      while (tmp > 0) {
        kb++;
        tmp /= 10;
      }
      // Make sure digit length is 1 if value is 0
      if (kb == 0) kb++;
      knownBitLength=new FastInteger(kb);
      discardedBitCount.AddInt(digits);
      while (digits > 0) {
        if (shiftedSmall == 0) {
          bitsAfterLeftmost |= bitLeftmost;
          bitLeftmost = 0;
          knownBitLength = new FastInteger(0);
          break;
        } else {
          int digit = (int)(shiftedSmall % 10);
          bitsAfterLeftmost |= bitLeftmost;
          bitLeftmost = digit;
          digits--;
          shiftedSmall /= 10;
          knownBitLength.SubtractInt(1);
        }
      }
      bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
    }

    /**
     *
     * @param bits A FastInteger object.
     */
    public void ShiftToDigits(FastInteger bits) {
      if(bits.signum()<0)
        throw new IllegalArgumentException("bits is negative");
      if(bits.CanFitInInt32()){
        ShiftToDigitsInt(bits.AsInt32());
      } else {
        knownBitLength=CalcKnownBitLength();
        BigInteger bigintDiff=knownBitLength.AsBigInteger();
        BigInteger bitsBig=bits.AsBigInteger();
        bigintDiff=bigintDiff.subtract(bitsBig);
        if(bigintDiff.signum()>0){
          // current length is greater than the
          // desired bit length
          ShiftRight(FastInteger.FromBig(bigintDiff));
        }
      }
    }

    /**
     * Shifts a number until it reaches the given number of digits, gathering
     * information on whether the last digit discarded is set and whether
     * the discarded digits to the right of that digit are set. Assumes that
     * the big integer being shifted is positive.
     * @param digits A 64-bit signed integer.
     */
    public void ShiftToDigitsInt(int digits) {
      if (isSmall)
        ShiftToBitsSmall(digits);
      else
        ShiftToBitsBig(digits);
    }
    private FastInteger CalcKnownBitLength() {
      if (isSmall) {
        int kb = 0;
        int tmp = shiftedSmall;
        while (tmp > 0) {
          kb++;
          tmp /= 10;
        }
        kb=(kb == 0 ? 1 : kb);
        return new FastInteger(kb);
      } else {
        String str = shiftedBigInt.toString();
        return new FastInteger(str.length());
      }
    }
    private void ShiftToBitsSmall(int digits) {
      int kb=0;
      int tmp = shiftedSmall;
      while (tmp > 0) {
        kb++;
        tmp /= 10;
      }
      // Make sure digit length is 1 if value is 0
      if (kb == 0) kb++;
      knownBitLength=new FastInteger(kb);
      if (kb > digits) {
        int digitShift = (int)(kb - digits);
        int newLength = (int)(kb - digitShift);
        knownBitLength = new FastInteger(Math.max(1, newLength));
        discardedBitCount.AddInt(digitShift);
        for (int i = 0; i < digitShift; i++) {
          int digit = (int)(shiftedSmall % 10);
          shiftedSmall /= 10;
          bitsAfterLeftmost |= bitLeftmost;
          bitLeftmost = digit;
        }
        bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
      }
    }
  }

