package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */


import java.math.*;

  final class ShiftAccumulator {
    int bitLeftmost = 0;

    /**
     * Gets whether the last discarded bit was set.
     */
    public int getBitLeftmost() { return bitLeftmost; }
    int bitsAfterLeftmost = 0;

    /**
     * Gets whether any of the discarded bits to the right of the last one was
     * set.
     */
    public int getBitsAfterLeftmost() { return bitsAfterLeftmost; }
    BigInteger shiftedBigInt;
    long knownBitLength;


    /**
     * Gets the length of the shifted value in bits.
     */
    public long getKnownBitLength() {
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
    public FastInteger getDiscardedBitCount() { return discardedBitCount; }
    public ShiftAccumulator(BigInteger bigint) {
      if (bigint.signum() < 0)
        throw new IllegalArgumentException("bigint is negative");
      shiftedBigInt = bigint;
      discardedBitCount = new FastInteger();
      isSmall = false;
      knownBitLength = -1;
    }
    public ShiftAccumulator(long longInt) {
      if (longInt < 0)
        throw new IllegalArgumentException("longInt is negative");
      shiftedLong = longInt;
      discardedBitCount = new FastInteger();
      isSmall = true;
      knownBitLength = -1;
    }
    /**
     * 
     * @param fastint A FastInteger object.
     */
    public void ShiftRight(FastInteger fastint) {
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
    private static byte[] ReverseBytes(byte[] bytes) {
      if ((bytes) == null) throw new NullPointerException("bytes");
      int half = bytes.length >> 1;
      int right = bytes.length - 1;
      for (int i = 0; i < half; i++, right--) {
        byte value = bytes[i];
        bytes[i] = bytes[right];
        bytes[right] = value;
      }
      return bytes;
    }

    private void ShiftRightBig(long bits) {
      if (bits <= 0) return;
      if (shiftedBigInt.signum()==0) {
        discardedBitCount.Add(bits);
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
        knownBitLength = 1;
        return;
      }
      byte[] bytes = ReverseBytes(shiftedBigInt.toByteArray());
      knownBitLength = ((long)bytes.length) << 3;
      // Find the last bit set
      for (int i = bytes.length - 1; i >= 0; i--) {
        int b = (int)bytes[i];
        if (b != 0) {
          if ((b & 0x80) != 0) { knownBitLength -= 0; break; }
          if ((b & 0x40) != 0) { knownBitLength -= 1; break; }
          if ((b & 0x20) != 0) { knownBitLength -= 2; break; }
          if ((b & 0x10) != 0) { knownBitLength -= 3; break; }
          if ((b & 0x08) != 0) { knownBitLength -= 4; break; }
          if ((b & 0x04) != 0) { knownBitLength -= 5; break; }
          if ((b & 0x02) != 0) { knownBitLength -= 6; break; }
          if ((b & 0x01) != 0) { knownBitLength -= 7; break; }
        }
        knownBitLength -= 8;
      }
      long bitDiff = 0;
      if (bits > knownBitLength) {
        bitDiff = bits - knownBitLength;
      }
      long bitShift = Math.min(knownBitLength, bits);
      if (bits >= knownBitLength) {
        isSmall = true;
        shiftedLong = 0;
        knownBitLength = 1;
      } else {
        long tmpBitShift = bitShift;
        while (tmpBitShift > 0 && shiftedBigInt.signum()!=0) {
          int bs = (int)Math.min(1000000, tmpBitShift);
          shiftedBigInt=shiftedBigInt.shiftRight(bs);
          tmpBitShift -= bs;
        }
        knownBitLength = knownBitLength - bitShift;
      }
      discardedBitCount.Add(bits);
      bitsAfterLeftmost |= bitLeftmost;
      for (int i = 0; i < bytes.length; i++) {
        if (bitShift > 8) {
          // Discard all the bits, they come
          // after the leftmost bit
          bitsAfterLeftmost |= bytes[i];
          bitShift -= 8;
        } else {
          // 8 or fewer bits left.
          // Get the bottommost bitShift minus 1 bits
          bitsAfterLeftmost |= ((bytes[i] << (9 - (int)bitShift)) & 0xFF);
          // Get the bit just above those bits
          bitLeftmost = (bytes[i] >> (((int)bitShift) - 1)) & 0x01;
          break;
        }
      }
      bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
      if (bitDiff > 0) {
        // Shifted more bits than the bit length
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
      }
    }

    private long CalcKnownBitLength() {
      if (isSmall) {
        int kb = 64;
        for (int i = 63; i >= 0; i++) {
          if ((shiftedLong & (1L << i)) != 0) {
            break;
          } else {
            kb--;
          }
        }
        // Make sure bit length is 1 if value is 0
        if (kb == 0) knownBitLength++;
        return kb;
      } else {
        byte[] bytes = ReverseBytes(shiftedBigInt.toByteArray());
        long kb = ((long)bytes.length) << 3;
        // Find the last bit set
        for (int i = bytes.length - 1; i >= 0; i--) {
          int b = (int)bytes[i];
          if (b != 0) {
            if ((b & 0x80) != 0) { kb -= 0; break; }
            if ((b & 0x40) != 0) { kb -= 1; break; }
            if ((b & 0x20) != 0) { kb -= 2; break; }
            if ((b & 0x10) != 0) { kb -= 3; break; }
            if ((b & 0x08) != 0) { kb -= 4; break; }
            if ((b & 0x04) != 0) { kb -= 5; break; }
            if ((b & 0x02) != 0) { kb -= 6; break; }
            if ((b & 0x01) != 0) { kb -= 7; break; }
          }
          kb -= 8;
        }
        // Make sure bit length is 1 if value is 0
        if (kb == 0) knownBitLength++;
        return kb;
      }
    }

    /**
     * Shifts a number until it reaches the given number of bits, gathering
     * information on whether the last bit discarded is set and whether the
     * discarded bits to the right of that bit are set. Assumes that the big
     * integer being shifted is positive.
     */
    private void ShiftToBitsBig(long bits) {
      byte[] bytes = ReverseBytes(shiftedBigInt.toByteArray());
      knownBitLength = ((long)bytes.length) << 3;
      // Find the last bit set
      for (int i = bytes.length - 1; i >= 0; i--) {
        int b = (int)bytes[i];
        if (b != 0) {
          if ((b & 0x80) != 0) { knownBitLength -= 0; break; }
          if ((b & 0x40) != 0) { knownBitLength -= 1; break; }
          if ((b & 0x20) != 0) { knownBitLength -= 2; break; }
          if ((b & 0x10) != 0) { knownBitLength -= 3; break; }
          if ((b & 0x08) != 0) { knownBitLength -= 4; break; }
          if ((b & 0x04) != 0) { knownBitLength -= 5; break; }
          if ((b & 0x02) != 0) { knownBitLength -= 6; break; }
          if ((b & 0x01) != 0) { knownBitLength -= 7; break; }
        }
        knownBitLength -= 8;
      }
      // Make sure bit length is 1 if value is 0
      if (knownBitLength == 0) knownBitLength++;
      // Shift by the difference in bit length
      if (knownBitLength > bits) {
        long bitShift = knownBitLength - bits;
        long tmpBitShift = bitShift;
        while (tmpBitShift > 0 && shiftedBigInt.signum()!=0) {
          int bs = (int)Math.min(1000000, tmpBitShift);
          shiftedBigInt=shiftedBigInt.shiftRight(bs);
          tmpBitShift -= bs;
        }
        knownBitLength = bits;
        if (bits <= 63) {
          // Shifting to 63 bits or fewer,
          // convert to small integer
          isSmall = true;
          shiftedLong = shiftedBigInt.longValue();
        }
        bitsAfterLeftmost |= bitLeftmost;
        discardedBitCount.Add(bitShift);
        for (int i = 0; i < bytes.length; i++) {
          if (bitShift > 8) {
            // Discard all the bits, they come
            // after the leftmost bit
            bitsAfterLeftmost |= bytes[i];
            bitShift -= 8;
          } else {
            // 8 or fewer bits left.
            // Get the bottommost bitShift minus 1 bits
            bitsAfterLeftmost |= ((bytes[i] << (9 - (int)bitShift)) & 0xFF);
            // Get the bit just above those bits
            bitLeftmost = (bytes[i] >> (((int)bitShift) - 1)) & 0x01;
            break;
          }
        }
        bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
      }
    }


    /**
     * Shifts a number to the right, gathering information on whether the
     * last bit discarded is set and whether the discarded bits to the right
     * of that bit are set. Assumes that the big integer being shifted is positive.
     * @param bits A 64-bit signed integer.
     */
    public void ShiftRight(long bits) {
      if (isSmall)
        ShiftRightSmall(bits);
      else
        ShiftRightBig(bits);
    }
    private void ShiftRightSmall(long bits) {
      if (bits <= 0) return;
      if (shiftedLong == 0) {
        discardedBitCount.Add(bits);
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
        knownBitLength = 1;
        return;
      }
      knownBitLength = 64;
      for (int i = 63; i >= 0; i++) {
        if ((shiftedLong & (1L << i)) != 0) {
          break;
        } else {
          knownBitLength--;
        }
      }
      int shift = (int)Math.min(knownBitLength, bits);
      boolean shiftingMoreBits = (bits > knownBitLength);
      knownBitLength = knownBitLength - shift;
      discardedBitCount.Add(bits);
      bitsAfterLeftmost |= bitLeftmost;
      // Get the bottommost shift minus 1 bits
      bitsAfterLeftmost |= (((shiftedLong << (65 - shift)) != 0) ? 1 : 0);
      // Get the bit just above that bit
      bitLeftmost = (int)((shiftedLong >> (((int)shift) - 1)) & 0x01);
      shiftedLong >>= shift;
      if (shiftingMoreBits) {
        // Shifted more bits than the bit length
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
      }
      bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
    }

    /**
     * Shifts a number until it reaches the given number of bits, gathering
     * information on whether the last bit discarded is set and whether the
     * discarded bits to the right of that bit are set. Assumes that the big
     * integer being shifted is positive.
     * @param bits A 64-bit signed integer.
     */
    public void ShiftToBits(long bits) {
      if (isSmall)
        ShiftToBitsSmall(bits);
      else
        ShiftToBitsBig(bits);
    }

    private void ShiftToBitsSmall(long bits) {
      knownBitLength = 64;
      for (int i = 63; i >= 0; i++) {
        if ((shiftedLong & (1L << i)) != 0) {
          break;
        } else {
          knownBitLength--;
        }
      }
      if (knownBitLength == 0) knownBitLength++;
      // Shift by the difference in bit length
      if (knownBitLength > bits) {
        long bitShift = knownBitLength - bits;
        int shift = (int)bitShift;
        knownBitLength = bits;
        discardedBitCount.Add(bitShift);
        bitsAfterLeftmost |= bitLeftmost;
        // Get the bottommost shift minus 1 bits
        bitsAfterLeftmost |= (((shiftedLong << (65 - shift)) != 0) ? 1 : 0);
        // Get the bit just above that bit
        bitLeftmost = (int)((shiftedLong >> (((int)shift) - 1)) & 0x01);
        bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
        shiftedLong >>= shift;
      }
    }
  }
