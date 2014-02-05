/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
using System.Text;
// using System.Numerics;
namespace PeterO {
  internal sealed class BitShiftAccumulator : IShiftAccumulator {
    private const int SmallBitLength = 32;

    private int bitLeftmost;

    /// <summary>Gets a value indicating whether the last discarded bit
    /// was set.</summary>
    /// <value>Whether the last discarded bit was set.</value>
    public int LastDiscardedDigit {
      get {
        return this.bitLeftmost;
      }
    }

    private int bitsAfterLeftmost;

    /// <summary>Gets a value indicating whether any of the discarded bits
    /// to the right of the last one was set.</summary>
    /// <value>Whether any of the discarded bits to the right of the last one
    /// was set.</value>
    public int OlderDiscardedDigits {
      get {
        return this.bitsAfterLeftmost;
      }
    }

    private BigInteger shiftedBigInt;
    private FastInteger knownBitLength;

    /// <summary>Not documented yet.</summary>
    /// <returns>A FastInteger object.</returns>
    public FastInteger GetDigitLength() {
      if (this.knownBitLength == null) {
        this.knownBitLength = this.CalcKnownBitLength();
      }
      return FastInteger.Copy(this.knownBitLength);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bits'>A FastInteger object.</param>
    public void ShiftToDigits(FastInteger bits) {
      if (bits.Sign < 0) {
        throw new ArgumentException("bits is negative");
      }
      if (bits.CanFitInInt32()) {
        this.ShiftToDigitsInt(bits.AsInt32());
      } else {
        this.knownBitLength = this.CalcKnownBitLength();
        BigInteger bigintDiff = this.knownBitLength.AsBigInteger();
        BigInteger bitsBig = bits.AsBigInteger();
        bigintDiff -= (BigInteger)bitsBig;
        if (bigintDiff.Sign > 0) {
          // current length is greater than the
          // desired bit length
          this.ShiftRight(FastInteger.FromBig(bigintDiff));
        }
      }
    }

    private int shiftedSmall;
    private bool isSmall;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public BigInteger ShiftedInt {
      get {
        if (this.isSmall) {
          return (BigInteger)this.shiftedSmall;
        } else {
          return this.shiftedBigInt;
        }
      }
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public FastInteger ShiftedIntFast {
      get {
        if (this.isSmall) {
          return new FastInteger(this.shiftedSmall);
        } else {
          return FastInteger.FromBig(this.shiftedBigInt);
        }
      }
    }

    private FastInteger discardedBitCount;

    /// <summary>Gets the number of digits discarded.</summary>
    /// <value>The number of digits discarded.</value>
    public FastInteger DiscardedDigitCount {
      get {
        return this.discardedBitCount;
      }
    }

    public BitShiftAccumulator(
      BigInteger bigint,
      int lastDiscarded,
      int olderDiscarded) {
      if (bigint.Sign < 0) {
        throw new ArgumentException("bigint is negative");
      }
      if (bigint.canFitInInt()) {
        this.isSmall = true;
        this.shiftedSmall = (int)bigint;
      } else {
        this.shiftedBigInt = bigint;
      }
      this.discardedBitCount = new FastInteger(0);
      this.bitsAfterLeftmost = (olderDiscarded != 0) ? 1 : 0;
      this.bitLeftmost = (lastDiscarded != 0) ? 1 : 0;
    }

    public static BitShiftAccumulator FromInt32(int smallNumber) {
      if (smallNumber < 0) {
        throw new ArgumentException("longInt is negative");
      }
      BitShiftAccumulator bsa = new BitShiftAccumulator(BigInteger.Zero, 0, 0);
      bsa.shiftedSmall = smallNumber;
      bsa.discardedBitCount = new FastInteger(0);
      bsa.isSmall = true;
      return bsa;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='fastint'>A FastInteger object.</param>
    public void ShiftRight(FastInteger fastint) {
      if (fastint.Sign <= 0) {
        return;
      }
      if (fastint.CanFitInInt32()) {
        this.ShiftRightInt(fastint.AsInt32());
      } else {
        BigInteger bi = fastint.AsBigInteger();
        while (bi.Sign > 0) {
          int count = 1000000;
          if (bi.CompareTo((BigInteger)1000000) < 0) {
            count = (int)bi;
          }
          this.ShiftRightInt(count);
          bi -= (BigInteger)count;
          if (this.isSmall ? this.shiftedSmall == 0 : this.shiftedBigInt.IsZero) {
            break;
          }
        }
      }
    }

    private void ShiftRightBig(int bits) {
      if (bits <= 0) {
        return;
      }
      if (this.shiftedBigInt.IsZero) {
        this.discardedBitCount.AddInt(bits);
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitLeftmost = 0;
        this.knownBitLength = new FastInteger(1);
        return;
      }
      byte[] bytes = this.shiftedBigInt.ToByteArray();
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
        while (tmpBitShift.Sign > 0 && !this.shiftedBigInt.IsZero) {
          int bs = tmpBitShift.MinInt32(1000000);
          this.shiftedBigInt >>= bs;
          tmpBitShift.SubtractInt(bs);
        }
        this.knownBitLength.Subtract(bitShift);
      }
      this.discardedBitCount.AddInt(bits);
      this.bitsAfterLeftmost |= this.bitLeftmost;
      for (int i = 0; i < bytes.Length; ++i) {
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
      if (bitDiff.Sign > 0) {
        // Shifted more bits than the bit length
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitLeftmost = 0;
      }
    }

    private static FastInteger ByteArrayBitLength(byte[] bytes) {
      FastInteger fastKB = new FastInteger(bytes.Length).Multiply(8);
      for (int i = bytes.Length - 1; i >= 0; --i) {
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
      if (fastKB.Sign == 0) {
        fastKB.Increment();
      }
      return fastKB;
    }

    private FastInteger CalcKnownBitLength() {
      if (this.isSmall) {
        int kb = SmallBitLength;
        for (int i = SmallBitLength - 1; i >= 0; --i) {
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
        // Console.WriteLine("{0:X8} kbl=" + (kb));
        return new FastInteger(kb);
      } else {
        if (this.shiftedBigInt.IsZero) {
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
          bs -= bits;
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
          bool odd = !this.shiftedBigInt.IsEven;
          this.shiftedBigInt >>= 1;
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
          this.shiftedBigInt >>= bs;
        }
        if (bits < SmallBitLength) {
          // Shifting to small number of bits,
          // convert to small integer
          this.isSmall = true;
          this.shiftedSmall = (int)this.shiftedBigInt;
        }
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
      }
    }

    /// <summary>Shifts a number to the right, gathering information on
    /// whether the last bit discarded is set and whether the discarded bits
    /// to the right of that bit are set. Assumes that the big integer being
    /// shifted is positive.</summary>
    /// <param name='bits'>A 32-bit signed integer.</param>
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
        } else {
          --kb;
        }
      }
      int shift = (int)Math.Min(kb, bits);
      bool shiftingMoreBits = bits > kb;
      kb -= shift;
      this.knownBitLength = new FastInteger(kb);
      this.discardedBitCount.AddInt(bits);
      this.bitsAfterLeftmost |= this.bitLeftmost;
      // Get the bottommost shift minus 1 bits
      this.bitsAfterLeftmost |= (shift > 1 && (this.shiftedSmall << (SmallBitLength - shift + 1)) != 0) ? 1 : 0;
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

    /// <summary>Shifts a number until it reaches the given number of bits,
    /// gathering information on whether the last bit discarded is set and
    /// whether the discarded bits to the right of that bit are set. Assumes
    /// that the big integer being shifted is positive.</summary>
    /// <param name='bits'>A 32-bit signed integer.</param>
    public void ShiftToDigitsInt(int bits) {
      if (bits < 0) {
        throw new ArgumentException("bits is negative");
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
      if (this.knownBitLength == null) {
        this.knownBitLength = this.GetDigitLength();
      }
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
        this.bitsAfterLeftmost |= (shift > 1 && (this.shiftedSmall << (SmallBitLength - shift + 1)) != 0) ? 1 : 0;
        // Get the bit just above that bit
        this.bitLeftmost = (int)((this.shiftedSmall >> (shift - 1)) & 0x01);
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        this.shiftedSmall >>= shift;
      } else {
        this.knownBitLength = new FastInteger(kbl);
      }
    }
  }
}
