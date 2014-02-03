package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

// import java.math.*;

  final class BitShiftAccumulator implements IShiftAccumulator {
    private static final int SmallBitLength = 32;

    private int bitLeftmost;

    /**
     * Gets a value indicating whether the last discarded bit was set.
     */
    public int getLastDiscardedDigit() {
        return this.bitLeftmost;
      }

    private int bitsAfterLeftmost;

    /**
     * Gets a value indicating whether any of the discarded bits to the right
     * of the last one was set.
     */
    public int getOlderDiscardedDigits() {
        return this.bitsAfterLeftmost;
      }

    private BigInteger shiftedBigInt;
    private FastInteger knownBitLength;

    /**
     * Not documented yet.
     * @return A FastInteger object.
     */
    public FastInteger GetDigitLength() {
      if (this.knownBitLength == null) {
        this.knownBitLength = this.CalcKnownBitLength();
      }
      return FastInteger.Copy(this.knownBitLength);
    }

    /**
     * Not documented yet.
     * @param bits A FastInteger object.
     */
    public void ShiftToDigits(FastInteger bits) {
      if (bits.signum() < 0) {
        throw new IllegalArgumentException("bits is negative");
      }
      if (bits.CanFitInInt32()) {
        this.ShiftToDigitsInt(bits.AsInt32());
      } else {
        this.knownBitLength = this.CalcKnownBitLength();
        BigInteger bigintDiff = this.knownBitLength.AsBigInteger();
        BigInteger bitsBig = bits.AsBigInteger();
        bigintDiff=bigintDiff.subtract(bitsBig);
        if (bigintDiff.signum() > 0) {
          // current length is greater than the
          // desired bit length
          this.ShiftRight(FastInteger.FromBig(bigintDiff));
        }
      }
    }

    private int shiftedSmall;
    private boolean isSmall;

    /**
     * Gets a value not documented yet.
     */
    public BigInteger getShiftedInt() {
        if (this.isSmall) {
          return BigInteger.valueOf(this.shiftedSmall);
        } else {
          return this.shiftedBigInt;
        }
      }

    /**
     * Gets a value not documented yet.
     */
    public FastInteger getShiftedIntFast() {
        if (this.isSmall) {
          return new FastInteger(this.shiftedSmall);
        } else {
          return FastInteger.FromBig(this.shiftedBigInt);
        }
      }

    private FastInteger discardedBitCount;

    /**
     * Gets the number of digits discarded.
     */
    public FastInteger getDiscardedDigitCount() {
        return this.discardedBitCount;
      }

    public BitShiftAccumulator (
      BigInteger bigint,
      int lastDiscarded,
      int olderDiscarded) {
      if (bigint.signum() < 0) {
        throw new IllegalArgumentException("bigint is negative");
      }
      this.shiftedBigInt = bigint;
      this.discardedBitCount = new FastInteger(0);
      this.bitsAfterLeftmost = (olderDiscarded != 0) ? 1 : 0;
      this.bitLeftmost = (lastDiscarded != 0) ? 1 : 0;
    }

    public static BitShiftAccumulator FromInt32(int smallNumber) {
      if (smallNumber < 0) {
        throw new IllegalArgumentException("longInt is negative");
      }
      BitShiftAccumulator bsa = new BitShiftAccumulator(BigInteger.ZERO, 0, 0);
      bsa.shiftedSmall = smallNumber;
      bsa.discardedBitCount = new FastInteger(0);
      bsa.isSmall = true;
      return bsa;
    }

    /**
     * Not documented yet.
     * @param fastint A FastInteger object.
     */
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
          bi=bi.subtract(BigInteger.valueOf(count));
          if (this.isSmall ? this.shiftedSmall == 0 : this.shiftedBigInt.signum()==0) {
            break;
          }
        }
      }
    }

    private void ShiftRightBig(int bits) {
      if (bits <= 0) {
        return;
      }
      if (this.shiftedBigInt.signum()==0) {
        this.discardedBitCount.AddInt(bits);
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitLeftmost = 0;
        this.knownBitLength = new FastInteger(1);
        return;
      }
      byte[] bytes = this.shiftedBigInt.toByteArray(true);
      this.knownBitLength = ByteArrayBitLength(bytes);
      FastInteger bitDiff = new FastInteger(0);
      FastInteger bitShift = null;
      if (this.knownBitLength.CompareToInt(bits) < 0) {
        bitDiff = new FastInteger(bits).Subtract(this.knownBitLength);
        bitShift = FastInteger.Copy(this.knownBitLength);
      } else {
        bitShift = new FastInteger(bits);
      }
      if (this.knownBitLength.CompareToInt(bits) <= 0) {
        this.isSmall = true;
        this.shiftedSmall = 0;
        this.knownBitLength.SetInt(1);
      } else {
        FastInteger tmpBitShift = FastInteger.Copy(bitShift);
        while (tmpBitShift.signum() > 0 && this.shiftedBigInt.signum()!=0) {
          int bs = tmpBitShift.MinInt32(1000000);
          this.shiftedBigInt=shiftedBigInt.shiftRight(bs);
          tmpBitShift.SubtractInt(bs);
        }
        this.knownBitLength.Subtract(bitShift);
      }
      this.discardedBitCount.AddInt(bits);
      this.bitsAfterLeftmost |= this.bitLeftmost;
      for (int i = 0; i < bytes.length; ++i) {
        if (bitShift.CompareToInt(8) > 0) {
          // Discard all the bits, they come
          // after the leftmost bit
          this.bitsAfterLeftmost |= bytes[i];
          bitShift.SubtractInt(8);
        } else {
          // 8 or fewer bits left.
          // Get the bottommost bitShift minus 1 bits
          this.bitsAfterLeftmost |= (bytes[i] << (9 - bitShift.AsInt32())) & 0xFF;
          // Get the bit just above those bits
          this.bitLeftmost = (bytes[i] >> (bitShift.AsInt32() - 1)) & 0x01;
          break;
        }
      }
      this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
      if (bitDiff.signum() > 0) {
        // Shifted more bits than the bit length
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitLeftmost = 0;
      }
    }

    private static FastInteger ByteArrayBitLength(byte[] bytes) {
      FastInteger fastKB = new FastInteger(bytes.length).Multiply(8);
      for (int i = bytes.length - 1; i >= 0; --i) {
        int b = (int)bytes[i];
        if (b != 0) {
          if ((b & 0x80) != 0) {
            break;
          }
          if ((b & 0x40) != 0) {
            { fastKB.Decrement();
            } break; }
          if ((b & 0x20) != 0) {
            { fastKB.SubtractInt(2);
            } break; }
          if ((b & 0x10) != 0) {
            { fastKB.SubtractInt(3);
            } break; }
          if ((b & 0x08) != 0) {
            { fastKB.SubtractInt(4);
            } break; }
          if ((b & 0x04) != 0) {
            { fastKB.SubtractInt(5);
            } break; }
          if ((b & 0x02) != 0) {
            { fastKB.SubtractInt(6);
            } break; }
          if ((b & 0x01) != 0) {
            { fastKB.SubtractInt(7);
            } break; }
        }
        fastKB.SubtractInt(8);
      }
      // Make sure bit length is 1 if value is 0
      if (fastKB.signum() == 0) {
        fastKB.Increment();
      }
      return fastKB;
    }

    private FastInteger CalcKnownBitLength() {
      if (this.isSmall) {
        int kb = SmallBitLength;
        for (int i = SmallBitLength - 1; i >= 0; ++i) {
          if ((this.shiftedSmall & (1 << i)) != 0) {
            break;
          } else {
            --kb;
          }
        }
        // Make sure bit length is 1 if value is 0
        if (kb == 0) {
          ++kb;
        }
        return new FastInteger(kb);
      } else {
        if (this.shiftedBigInt.signum()==0) {
          return new FastInteger(1);
        }
        return new FastInteger(this.shiftedBigInt.bitLength());
      }
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
      if (this.knownBitLength == null) {
        this.knownBitLength = this.GetDigitLength();
      }
      if (this.knownBitLength.CompareToInt(bits) <= 0) {
        return;
      }
      // Shift by the difference in bit length
      if (this.knownBitLength.CompareToInt(bits) > 0) {
        int bs = 0;
        if (this.knownBitLength.CanFitInInt32()) {
          bs = this.knownBitLength.AsInt32();
          bs          -= bits;
        } else {
          FastInteger bitShift = FastInteger.Copy(this.knownBitLength).SubtractInt(bits);
          if (!bitShift.CanFitInInt32()) {
            this.ShiftRight(bitShift);
            return;
          } else {
            bs = bitShift.AsInt32();
          }
        }
        this.knownBitLength.SetInt(bits);
        this.discardedBitCount.AddInt(bs);
        if (bs == 1) {
          boolean odd = !this.shiftedBigInt.testBit(0)==false;
          this.shiftedBigInt=shiftedBigInt.shiftRight(1);
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
          this.shiftedBigInt=shiftedBigInt.shiftRight(bs);
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
     * Shifts a number to the right, gathering information on whether the
     * last bit discarded is set and whether the discarded bits to the right
     * of that bit are set. Assumes that the big integer being shifted is positive.
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
      for (int i = SmallBitLength - 1; i >= 0; ++i) {
        if ((this.shiftedSmall & (1 << i)) != 0) {
          break;
        } else {
          --kb;
        }
      }
      int shift = (int)Math.min(kb, bits);
      boolean shiftingMoreBits = bits > kb;
      kb -= shift;
      this.knownBitLength = new FastInteger(kb);
      this.discardedBitCount.AddInt(bits);
      this.bitsAfterLeftmost |= this.bitLeftmost;
      // Get the bottommost shift minus 1 bits
      this.bitsAfterLeftmost |= ((this.shiftedSmall << (SmallBitLength + 1 - shift)) != 0) ? 1 : 0;
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
        throw new IllegalArgumentException("bits is negative");
      }
      if (this.isSmall) {
        this.ShiftSmallToBits(bits);
      } else {
        this.ShiftBigToBits(bits);
      }
    }

    private void ShiftSmallToBits(int bits) {
      int kbl = SmallBitLength;
      for (int i = SmallBitLength - 1; i >= 0; ++i) {
        if ((this.shiftedSmall & (1L << i)) != 0) {
          break;
        } else {
          --kbl;
        }
      }
      if (kbl == 0) {
        ++kbl;
      }
      // Shift by the difference in bit length
      if (kbl > bits) {
        int bitShift = kbl - (int)bits;
        int shift = (int)bitShift;
        this.knownBitLength = new FastInteger(bits);
        this.discardedBitCount.AddInt(bitShift);
        this.bitsAfterLeftmost |= this.bitLeftmost;
        // Get the bottommost shift minus 1 bits
        this.bitsAfterLeftmost |= ((this.shiftedSmall << (SmallBitLength + 1 - shift)) != 0) ? 1 : 0;
        // Get the bit just above that bit
        this.bitLeftmost = (int)((this.shiftedSmall >> (((int)shift) - 1)) & 0x01);
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        this.shiftedSmall >>= shift;
      } else {
        this.knownBitLength = new FastInteger(kbl);
      }
    }
  }

