package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */


//import java.math.*;

  final class BitShiftAccumulator implements IShiftAccumulator {
    int bitLeftmost;

    /**
     * Gets whether the last discarded bit was set.
     */
    public int getLastDiscardedDigit() { return bitLeftmost; }
    int bitsAfterLeftmost;

    /**
     * Gets whether any of the discarded bits to the right of the last one was
     * set.
     */
    public int getOlderDiscardedDigits() { return bitsAfterLeftmost; }
    BigInteger shiftedBigInt;
    long knownBitLength;
    
    /**
     * 
     */
    public FastInteger GetDigitLength() {
      if (knownBitLength < 0) {
        knownBitLength = CalcKnownBitLength();
      }
      FastInteger ret;
      if(knownBitLength<=Integer.MAX_VALUE)
        ret=new FastInteger((int)knownBitLength);
      else
        ret=new FastInteger(BigInteger.valueOf(knownBitLength));
      return ret;
    }
    
    /**
     * 
     * @param bits A FastInteger object.
     */
public void ShiftToDigits(FastInteger bits) {
      if(bits.signum()<0)
        throw new IllegalArgumentException("bits is negative");
      if(bits.CanFitInInt32()){
        ShiftToDigits(bits.AsInt32());
      } else {
        knownBitLength=CalcKnownBitLength();
        BigInteger bigintDiff=BigInteger.valueOf(knownBitLength);
        BigInteger bitsBig=bits.AsBigInteger();
        bigintDiff=bigintDiff.subtract(bitsBig);
        if(bigintDiff.signum()>0){
          // current length is greater than the 
          // desired bit length
          ShiftRight(new FastInteger(bigintDiff));
        }
      }
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
    public FastInteger getShiftedIntFast() {
        if (isSmall){
          if(shiftedLong>=Integer.MIN_VALUE && shiftedLong<=Integer.MAX_VALUE){
            return new FastInteger((int)shiftedLong);
          } else {
            return new FastInteger(BigInteger.valueOf(shiftedLong));
          }
        } else {
          return new FastInteger(shiftedBigInt);
        }
      }
    FastInteger discardedBitCount;

    /**
     * 
     */
    public FastInteger getDiscardedDigitCount() { return discardedBitCount; }

    public BitShiftAccumulator(BigInteger bigint,
                               int lastDiscarded,
                               int olderDiscarded
                              ){
 this(bigint);
      bitsAfterLeftmost = (olderDiscarded != 0) ? 1 : 0;
      bitLeftmost = (lastDiscarded != 0) ? 1 : 0;
    }
    public BitShiftAccumulator(BigInteger bigint) {
      if (bigint.signum() < 0)
        throw new IllegalArgumentException("bigint is negative");
      shiftedBigInt = bigint;
      discardedBitCount = new FastInteger();
      knownBitLength = -1;
    }
    public BitShiftAccumulator(long longInt) {
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
      if (fastint.CanFitInInt32()) {
        ShiftRight(fastint.AsInt32());
      } else {
        BigInteger bi = fastint.AsBigInteger();
        while (bi.signum() > 0) {
          int count = 1000000;
          if (bi.compareTo(BigInteger.valueOf(1000000)) < 0) {
            count = bi.intValue();
          }
          ShiftRight(count);
          bi=bi.subtract(BigInteger.valueOf(count));
        }
      }
    }

    private void ShiftRightBig(int bits) {
      if (bits <= 0) return;
      if (shiftedBigInt.signum()==0) {
        discardedBitCount.Add(bits);
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
        knownBitLength = 1;
        return;
      }
      byte[] bytes = shiftedBigInt.toByteArray(true);
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
        byte[] bytes = shiftedBigInt.toByteArray(true);
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
      byte[] bytes = shiftedBigInt.toByteArray(true);
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
        if(bitShift<=Integer.MAX_VALUE)
          discardedBitCount.Add((int)bitShift);
        else
          discardedBitCount.Add(BigInteger.valueOf(bitShift));
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
    public void ShiftRight(int bits) {
      if (isSmall)
        ShiftRightSmall(bits);
      else
        ShiftRightBig(bits);
    }
    private void ShiftRightSmall(int bits) {
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
    public void ShiftToDigits(long bits) {
      if(bits<0)
        throw new IllegalArgumentException("bits is negative");
      if (isSmall)
        ShiftToBitsSmall(bits);
      else
        ShiftToBitsBig(bits);
    }

    private void ShiftToBitsSmall(long bits) {
      int kbl = 64;
      for (int i = 63; i >= 0; i++) {
        if ((shiftedLong & (1L << i)) != 0) {
          break;
        } else {
          kbl--;
        }
      }
      if (kbl == 0) kbl++;
      // Shift by the difference in bit length
      if (kbl > bits) {
        int bitShift = kbl - (int)bits;
        int shift = (int)bitShift;
        knownBitLength = bits;
        if(bitShift<=Integer.MAX_VALUE)
          discardedBitCount.Add((int)bitShift);
        else
          discardedBitCount.Add(BigInteger.valueOf(bitShift));
        bitsAfterLeftmost |= bitLeftmost;
        // Get the bottommost shift minus 1 bits
        bitsAfterLeftmost |= (((shiftedLong << (65 - shift)) != 0) ? 1 : 0);
        // Get the bit just above that bit
        bitLeftmost = (int)((shiftedLong >> (((int)shift) - 1)) & 0x01);
        bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
        shiftedLong >>= shift;
      } else {
        knownBitLength=kbl;
      }
    }
  }
