package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  final class BitShiftAccumulator implements IShiftAccumulator {

    private static final int SmallBitLength = 32;
    private int bitLeftmost;

    /**
     * Gets a value indicating whether the last discarded bit was set.
     * @return True if the last discarded bit was set; otherwise, false.
     */
    public final int getLastDiscardedDigit() {
        return this.bitLeftmost;
      }

    private int bitsAfterLeftmost;

    /**
     * Gets a value indicating whether any of the discarded bits to the right of
     * the last one was set.
     * @return True if any of the discarded bits to the right of the last one was
     * set; otherwise, false.
     */
    public final int getOlderDiscardedDigits() {
        return this.bitsAfterLeftmost;
      }

    private BigInteger shiftedBigInt;
    private FastInteger knownBitLength;

    public FastInteger GetDigitLength() {
      this.knownBitLength = (this.knownBitLength == null) ? (this.CalcKnownBitLength()) : this.knownBitLength;
      return FastInteger.Copy(this.knownBitLength);
    }

    public void ShiftToDigits(FastInteger bits) {
      if (bits.signum() < 0) {
        throw new IllegalArgumentException("bits's sign (" + bits.signum() +
          ") is less than " + "0");
      }
      if (bits.CanFitInInt32()) {
        this.ShiftToDigitsInt(bits.AsInt32());
      } else {
        this.knownBitLength = this.CalcKnownBitLength();
        BigInteger bigintDiff = this.knownBitLength.AsBigInteger();
        BigInteger bitsBig = bits.AsBigInteger();
        bigintDiff = bigintDiff.subtract(bitsBig);
        if (bigintDiff.signum() > 0) {
          // current length is greater than the
          // desired bit length
          this.ShiftRight(FastInteger.FromBig(bigintDiff));
        }
      }
    }

    private int shiftedSmall;
    private boolean isSmall;

    public final BigInteger getShiftedInt() {
        return this.isSmall ? (BigInteger.valueOf(this.shiftedSmall)) :
        this.shiftedBigInt;
      }

    public final FastInteger getShiftedIntFast() {
        return this.isSmall ? (new FastInteger(this.shiftedSmall)) :
        FastInteger.FromBig(this.shiftedBigInt);
      }

    private FastInteger discardedBitCount;

    /**
     * Gets the number of digits discarded.
     * @return The number of digits discarded.
     */
    public final FastInteger getDiscardedDigitCount() {
        return this.discardedBitCount;
      }

    public BitShiftAccumulator (
      BigInteger bigint,
      int lastDiscarded,
      int olderDiscarded) {
      if (bigint.signum() < 0) {
        throw new IllegalArgumentException("bigint's sign (" + bigint.signum() +
          ") is less than " + "0");
      }
      if (bigint.canFitInInt()) {
        this.isSmall = true;
        this.shiftedSmall = bigint.intValue();
      } else {
        this.shiftedBigInt = bigint;
      }
      this.discardedBitCount = new FastInteger(0);
      this.bitsAfterLeftmost = (olderDiscarded != 0) ? 1 : 0;
      this.bitLeftmost = (lastDiscarded != 0) ? 1 : 0;
    }

    public static BitShiftAccumulator FromInt32(int smallNumber) {
      if (smallNumber < 0) {
        throw new IllegalArgumentException("smallNumber (" + smallNumber +
          ") is less than " + "0");
      }
      BitShiftAccumulator bsa = new BitShiftAccumulator(BigInteger.ZERO, 0, 0);
      bsa.shiftedSmall = smallNumber;
      bsa.discardedBitCount = new FastInteger(0);
      bsa.isSmall = true;
      return bsa;
    }

    public void ShiftRight(FastInteger fastint) {
      if (fastint.signum() <= 0) {
        return;
      }
      if (fastint.CanFitInInt32()) {
        this.ShiftRightInt(fastint.AsInt32());
      } else {
        BigInteger bi = fastint.AsBigInteger();
        while (bi.signum() > 0) {
          int count = 1000000;
          if (bi.compareTo(BigInteger.valueOf(1000000)) < 0) {
            count = bi.intValue();
          }
          this.ShiftRightInt(count);
          bi = bi.subtract(BigInteger.valueOf(count));
          if (this.isSmall ? this.shiftedSmall == 0 :
          this.shiftedBigInt.signum() == 0) {
            break;
          }
        }
      }
    }

    private void ShiftRightBig(int bits) {
      if (bits <= 0) {
        return;
      }
      if (this.shiftedBigInt.signum() == 0) {
        this.discardedBitCount.AddInt(bits);
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitLeftmost = 0;
        this.isSmall = true;
        this.shiftedSmall = 0;
        this.knownBitLength = new FastInteger(1);
        return;
      }
      this.knownBitLength = (this.knownBitLength == null) ? (this.GetDigitLength()) : this.knownBitLength;
      this.discardedBitCount.AddInt(bits);
      int cmp = this.knownBitLength.CompareToInt(bits);
      if (cmp < 0) {
        // too few bits
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitsAfterLeftmost |= this.shiftedBigInt.signum() == 0 ? 0 : 1;
        this.bitLeftmost = 0;
        this.isSmall = true;
        this.shiftedSmall = 0;
        this.knownBitLength = new FastInteger(1);
      } else {
        // enough bits in the current value
        int bs = bits;
        this.knownBitLength.SubtractInt(bits);
        if (bs == 1) {
          boolean odd = !this.shiftedBigInt.testBit(0) == false;
          this.shiftedBigInt = shiftedBigInt.shiftRight(1);
          this.bitsAfterLeftmost |= this.bitLeftmost;
          this.bitLeftmost = odd ? 1 : 0;
        } else {
          this.bitsAfterLeftmost |= this.bitLeftmost;
          int lowestSetBit = this.shiftedBigInt.getLowestSetBit();
          if (lowestSetBit < bs - 1) {
            // One of the discarded bits after
            // the last one is set
            this.bitsAfterLeftmost |= 1;
            this.bitLeftmost = this.shiftedBigInt.testBit(bs - 1) ? 1 : 0;
          } else if (lowestSetBit > bs - 1) {
            // Means all discarded bits are zero
            this.bitLeftmost = 0;
          } else {
            // Only the last discarded bit is set
            this.bitLeftmost = 1;
          }
          this.shiftedBigInt = shiftedBigInt.shiftRight(bs);
        }
        if (this.knownBitLength.CompareToInt(SmallBitLength) < 0) {
          // Shifting to small number of bits,
          // convert to small integer
          this.isSmall = true;
          this.shiftedSmall = this.shiftedBigInt.intValue();
        }
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
      }
    }

    private FastInteger CalcKnownBitLength() {
      if (this.isSmall) {
        int kb = SmallBitLength;
        for (int i = SmallBitLength - 1; i >= 0; --i) {
          if ((this.shiftedSmall & (1 << i)) != 0) {
            break;
          }
          --kb;
        }
        // Make sure bit length is 1 if value is 0
        if (kb == 0) {
          ++kb;
        }
        // System.out.println("{0:X8} kbl=" + (kb));
        return new FastInteger(kb);
      }
      return new FastInteger(this.shiftedBigInt.signum() == 0 ? 1 :
      this.shiftedBigInt.bitLength());
    }

    private void ShiftBigToBits(int bits) {
      // Shifts a number until it reaches the given number of bits,
      // gathering information on whether the last bit discarded is set and
      // whether the discarded bits to the right of that bit are set. Assumes
      // that the big integer being shifted is positive.
      if (this.knownBitLength != null) {
        if (this.knownBitLength.CompareToInt(bits) <= 0) {
          return;
        }
      }
      this.knownBitLength = (this.knownBitLength == null) ? (this.GetDigitLength()) : this.knownBitLength;
      if (this.knownBitLength.CompareToInt(bits) <= 0) {
        return;
      }
      // Shift by the difference in bit length
      if (this.knownBitLength.CompareToInt(bits) > 0) {
        int bs = 0;
        if (this.knownBitLength.CanFitInInt32()) {
          bs = this.knownBitLength.AsInt32();
          bs -= bits;
        } else {
          FastInteger bitShift =
          FastInteger.Copy(this.knownBitLength).SubtractInt(bits);
          if (!bitShift.CanFitInInt32()) {
            this.ShiftRight(bitShift);
            return;
          }
          bs = bitShift.AsInt32();
        }
        this.knownBitLength.SetInt(bits);
        this.discardedBitCount.AddInt(bs);
        if (bs == 1) {
          boolean odd = !this.shiftedBigInt.testBit(0) == false;
          this.shiftedBigInt = shiftedBigInt.shiftRight(1);
          this.bitsAfterLeftmost |= this.bitLeftmost;
          this.bitLeftmost = odd ? 1 : 0;
        } else {
          this.bitsAfterLeftmost |= this.bitLeftmost;
          int lowestSetBit = this.shiftedBigInt.getLowestSetBit();
          if (lowestSetBit < bs - 1) {
            // One of the discarded bits after
            // the last one is set
            this.bitsAfterLeftmost |= 1;
            this.bitLeftmost = this.shiftedBigInt.testBit(bs - 1) ? 1 : 0;
          } else if (lowestSetBit > bs - 1) {
            // Means all discarded bits are zero
            this.bitLeftmost = 0;
          } else {
            // Only the last discarded bit is set
            this.bitLeftmost = 1;
          }
          this.shiftedBigInt = shiftedBigInt.shiftRight(bs);
        }
        if (bits < SmallBitLength) {
          // Shifting to small number of bits,
          // convert to small integer
          this.isSmall = true;
          this.shiftedSmall = this.shiftedBigInt.intValue();
        }
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
      }
    }

    /**
     * Shifts a number to the right, gathering information on whether the last bit
     * discarded is set and whether the discarded bits to the right of that
     * bit are set. Assumes that the big integer being shifted is positive.
     * @param bits A 32-bit signed integer.
     */
    public void ShiftRightInt(int bits) {
      if (this.isSmall) {
        this.ShiftRightSmall(bits);
      } else {
        this.ShiftRightBig(bits);
      }
    }

    private void ShiftRightSmall(int bits) {
      if (bits <= 0) {
        return;
      }
      if (this.shiftedSmall == 0) {
        this.discardedBitCount.AddInt(bits);
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitLeftmost = 0;
        this.knownBitLength = new FastInteger(1);
        return;
      }
      int kb = SmallBitLength;
      for (int i = SmallBitLength - 1; i >= 0; --i) {
        if ((this.shiftedSmall & (1 << i)) != 0) {
          break;
        }
        --kb;
      }
      int shift = (int)Math.min(kb, bits);
      boolean shiftingMoreBits = bits > kb;
      kb -= shift;
      this.knownBitLength = new FastInteger(kb);
      this.discardedBitCount.AddInt(bits);
      this.bitsAfterLeftmost |= this.bitLeftmost;
      // Get the bottommost shift minus 1 bits
      this.bitsAfterLeftmost |= (shift > 1 && (this.shiftedSmall <<
      (SmallBitLength - shift + 1)) != 0) ? 1 : 0;
      // Get the bit just above that bit
      this.bitLeftmost = (int)((this.shiftedSmall >> (shift - 1)) & 0x01);
      this.shiftedSmall >>= shift;
      if (shiftingMoreBits) {
        // Shifted more bits than the bit length
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitLeftmost = 0;
      }
      this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
    }

    /**
     * Shifts a number until it reaches the given number of bits, gathering
     * information on whether the last bit discarded is set and whether the
     * discarded bits to the right of that bit are set. Assumes that the big
     * integer being shifted is positive.
     * @param bits A 32-bit signed integer.
     */
    public void ShiftToDigitsInt(int bits) {
      if (bits < 0) {
        throw new IllegalArgumentException("bits (" + bits + ") is less than " + "0");
      }
      if (this.isSmall) {
        this.ShiftSmallToBits(bits);
      } else {
        this.ShiftBigToBits(bits);
      }
    }

    private void ShiftSmallToBits(int bits) {
      if (this.knownBitLength != null) {
        if (this.knownBitLength.CompareToInt(bits) <= 0) {
          return;
        }
      }
      this.knownBitLength = (this.knownBitLength == null) ? (this.GetDigitLength()) : this.knownBitLength;
      if (this.knownBitLength.CompareToInt(bits) <= 0) {
        return;
      }
      int kbl = this.knownBitLength.AsInt32();
      // Shift by the difference in bit length
      if (kbl > bits) {
        int bitShift = kbl - (int)bits;
        int shift = (int)bitShift;
        this.knownBitLength = new FastInteger(bits);
        this.discardedBitCount.AddInt(bitShift);
        this.bitsAfterLeftmost |= this.bitLeftmost;
        // Get the bottommost shift minus 1 bits
        this.bitsAfterLeftmost |= (shift > 1 && (this.shiftedSmall <<
        (SmallBitLength - shift + 1)) != 0) ? 1 : 0;
        // Get the bit just above that bit
        this.bitLeftmost = (int)((this.shiftedSmall >> (shift - 1)) & 0x01);
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        this.shiftedSmall >>= shift;
      } else {
        this.knownBitLength = new FastInteger(kbl);
      }
    }
  }
