/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
using System.Text;
//using System.Numerics;
namespace PeterO {
  internal sealed class BitShiftAccumulator : IShiftAccumulator {
    int bitLeftmost;

    /// <summary> Gets whether the last discarded bit was set. </summary>
    public int LastDiscardedDigit {
      get { return bitLeftmost; }
    }
    int bitsAfterLeftmost;
    private const int SmallBitLength = 32;

    /// <summary> Gets whether any of the discarded bits to the right of the
    /// last one was set. </summary>
    public int OlderDiscardedDigits {
      get { return bitsAfterLeftmost; }
    }

    BigInteger shiftedBigInt;
    FastInteger knownBitLength;

    /// <summary> </summary>
    /// <returns>A FastInteger object.</returns>
    public FastInteger GetDigitLength(){
      if (knownBitLength==null) {
        knownBitLength = CalcKnownBitLength();
      }
      return FastInteger.Copy(knownBitLength);
    }

    /// <summary> </summary>
    /// <param name='bits'>A FastInteger object.</param>
    /// <returns></returns>
    public void ShiftToDigits(FastInteger bits){
      if(bits.Sign<0)
        throw new ArgumentException("bits is negative");
      if(bits.CanFitInInt32()){
        ShiftToDigitsInt(bits.AsInt32());
      } else {
        knownBitLength=CalcKnownBitLength();
        BigInteger bigintDiff=knownBitLength.AsBigInteger();
        BigInteger bitsBig=bits.AsBigInteger();
        bigintDiff-=(BigInteger)bitsBig;
        if(bigintDiff.Sign>0){
          // current length is greater than the
          // desired bit length
          ShiftRight(FastInteger.FromBig(bigintDiff));
        }
      }
    }

    int shiftedSmall;
    bool isSmall;

    /// <summary> </summary>
    public BigInteger ShiftedInt {
      get {
        if (isSmall)
          return (BigInteger)shiftedSmall;
        else
          return shiftedBigInt;
      }
    }
    /// <summary> </summary>
    public FastInteger ShiftedIntFast{
      get {
        if (isSmall){
          return new FastInteger(shiftedSmall);
        } else {
          return FastInteger.FromBig(shiftedBigInt);
        }
      }
    }
    FastInteger discardedBitCount;

    /// <summary> </summary>
    public FastInteger DiscardedDigitCount {
      get { return discardedBitCount; }
    }

    public BitShiftAccumulator(BigInteger bigint,
                               int lastDiscarded,
                               int olderDiscarded
                              ) {
      if (bigint.Sign < 0)
        throw new ArgumentException("bigint is negative");
      shiftedBigInt = bigint;
      discardedBitCount = new FastInteger(0);
      bitsAfterLeftmost = (olderDiscarded != 0) ? 1 : 0;
      bitLeftmost = (lastDiscarded != 0) ? 1 : 0;
    }

    public static BitShiftAccumulator FromInt32(int smallNumber) {
      if (smallNumber < 0)
        throw new ArgumentException("longInt is negative");
      BitShiftAccumulator bsa=new BitShiftAccumulator(BigInteger.Zero,0,0);
      bsa.shiftedSmall = smallNumber;
      bsa.discardedBitCount = new FastInteger(0);
      bsa.isSmall = true;
      return bsa;
    }
    /// <summary> </summary>
    /// <param name='fastint'>A FastInteger object.</param>
    /// <returns></returns>
    public void ShiftRight(FastInteger fastint) {
      if (fastint.Sign <= 0) return;
      if (fastint.CanFitInInt32()) {
        ShiftRightInt(fastint.AsInt32());
      } else {
        BigInteger bi = fastint.AsBigInteger();
        while (bi.Sign > 0) {
          int count = 1000000;
          if (bi.CompareTo((BigInteger)1000000) < 0) {
            count = (int)bi;
          }
          ShiftRightInt(count);
          bi -= (BigInteger)count;
        }
      }
    }

    private void ShiftRightBig(int bits) {
      if (bits <= 0) return;
      if (shiftedBigInt.IsZero) {
        discardedBitCount.AddInt(bits);
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
        knownBitLength=new FastInteger(1);
        return;
      }
      byte[] bytes = shiftedBigInt.ToByteArray();
      knownBitLength = ByteArrayBitLength(bytes);
      FastInteger bitDiff = new FastInteger(0);
      FastInteger bitShift=null;
      if (knownBitLength.CompareToInt(bits)<0) {
        bitDiff = new FastInteger(bits).Subtract(knownBitLength);
        bitShift=FastInteger.Copy(knownBitLength);
      } else {
        bitShift=new FastInteger(bits);
      }
      if (knownBitLength.CompareToInt(bits)<=0) {
        isSmall = true;
        shiftedSmall = 0;
        knownBitLength.Multiply(0).AddInt(1);
      } else {
        FastInteger tmpBitShift = FastInteger.Copy(bitShift);
        while (tmpBitShift.Sign > 0 && !shiftedBigInt.IsZero) {
          int bs = tmpBitShift.MinInt32(1000000);
          shiftedBigInt >>= bs;
          tmpBitShift.SubtractInt(bs);
        }
        knownBitLength.Subtract(bitShift);
      }
      discardedBitCount.AddInt(bits);
      bitsAfterLeftmost |= bitLeftmost;
      for (int i = 0; i < bytes.Length; i++) {
        if (bitShift.CompareToInt(8)>0) {
          // Discard all the bits, they come
          // after the leftmost bit
          bitsAfterLeftmost |= bytes[i];
          bitShift.SubtractInt(8);
        } else {
          // 8 or fewer bits left.
          // Get the bottommost bitShift minus 1 bits
          bitsAfterLeftmost |= ((bytes[i] << (9 - bitShift.AsInt32())) & 0xFF);
          // Get the bit just above those bits
          bitLeftmost = (bytes[i] >> ((bitShift.AsInt32()) - 1)) & 0x01;
          break;
        }
      }
      bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
      if (bitDiff.Sign > 0) {
        // Shifted more bits than the bit length
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
      }
    }

    private static FastInteger ByteArrayBitLength(byte[] bytes){
      FastInteger fastKB = new FastInteger(bytes.Length).Multiply(8);
      for (int i = bytes.Length - 1; i >= 0; i--) {
        int b = (int)bytes[i];
        if (b != 0) {
          if ((b & 0x80) != 0) { break; }
          if ((b & 0x40) != 0) { fastKB.SubtractInt(1); break; }
          if ((b & 0x20) != 0) { fastKB.SubtractInt(2); break; }
          if ((b & 0x10) != 0) { fastKB.SubtractInt(3); break; }
          if ((b & 0x08) != 0) { fastKB.SubtractInt(4); break; }
          if ((b & 0x04) != 0) { fastKB.SubtractInt(5); break; }
          if ((b & 0x02) != 0) { fastKB.SubtractInt(6); break; }
          if ((b & 0x01) != 0) { fastKB.SubtractInt(7); break; }
        }
        fastKB.SubtractInt(8);
      }
      // Make sure bit length is 1 if value is 0
      if (fastKB.Sign == 0) fastKB.AddInt(1);
      return fastKB;
    }

    private FastInteger CalcKnownBitLength() {
      if (isSmall) {
        int kb = SmallBitLength;
        for (int i = SmallBitLength-1; i >= 0; i++) {
          if ((shiftedSmall & (1 << i)) != 0) {
            break;
          } else {
            kb--;
          }
        }
        // Make sure bit length is 1 if value is 0
        if (kb == 0) kb++;
        return new FastInteger(kb);
      } else {
        byte[] bytes = shiftedBigInt.ToByteArray();
        // Find the last bit set
        return ByteArrayBitLength(bytes);
      }
    }

    /// <summary> Shifts a number until it reaches the given number of bits,
    /// gathering information on whether the last bit discarded is set and
    /// whether the discarded bits to the right of that bit are set. Assumes
    /// that the big integer being shifted is positive. </summary>
    private void ShiftBigToBits(int bits) {
      byte[] bytes = shiftedBigInt.ToByteArray();
      knownBitLength = ByteArrayBitLength(bytes);
      // Shift by the difference in bit length
      if (knownBitLength.CompareToInt(bits) > 0) {
        FastInteger bitShift = FastInteger.Copy(knownBitLength).SubtractInt(bits);
        FastInteger tmpBitShift = FastInteger.Copy(bitShift);
        while (tmpBitShift.Sign > 0 && !shiftedBigInt.IsZero) {
          int bs = tmpBitShift.MinInt32(1000000);
          shiftedBigInt >>= bs;
          tmpBitShift.SubtractInt(bs);
        }
        knownBitLength.Multiply(0).AddInt(bits);
        if (bits < SmallBitLength) {
          // Shifting to small number of bits,
          // convert to small integer
          isSmall = true;
          shiftedSmall = (int)shiftedBigInt;
        }
        bitsAfterLeftmost |= bitLeftmost;
        discardedBitCount.Add(bitShift);
        for (int i = 0; i < bytes.Length; i++) {
          if (bitShift.CompareToInt(8)>0) {
            // Discard all the bits, they come
            // after the leftmost bit
            bitsAfterLeftmost |= bytes[i];
            bitShift.SubtractInt(8);
          } else {
            // 8 or fewer bits left.
            // Get the bottommost bitShift minus 1 bits
            bitsAfterLeftmost |= ((bytes[i] << (9 - bitShift.AsInt32())) & 0xFF);
            // Get the bit just above those bits
            bitLeftmost = (bytes[i] >> ((bitShift.AsInt32()) - 1)) & 0x01;
            break;
          }
        }
        bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
      }
    }

    /// <summary> Shifts a number to the right, gathering information on
    /// whether the last bit discarded is set and whether the discarded bits
    /// to the right of that bit are set. Assumes that the big integer being
    /// shifted is positive. </summary>
    /// <returns></returns>
    /// <param name='bits'>A 32-bit signed integer.</param>
    public void ShiftRightInt(int bits) {
      if (isSmall)
        ShiftRightSmall(bits);
      else
        ShiftRightBig(bits);
    }
    private void ShiftRightSmall(int bits) {
      if (bits <= 0) return;
      if (shiftedSmall == 0) {
        discardedBitCount.AddInt(bits);
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
        knownBitLength = new FastInteger(1);
        return;
      }
      int kb = SmallBitLength;
      for (int i = SmallBitLength-1; i >= 0; i++) {
        if ((shiftedSmall & (1 << i)) != 0) {
          break;
        } else {
          kb--;
        }
      }
      int shift = (int)Math.Min(kb, bits);
      bool shiftingMoreBits = (bits > kb);
      kb = kb - shift;
      knownBitLength=new FastInteger(kb);
      discardedBitCount.AddInt(bits);
      bitsAfterLeftmost |= bitLeftmost;
      // Get the bottommost shift minus 1 bits
      bitsAfterLeftmost |= (((shiftedSmall << (SmallBitLength + 1 - shift)) != 0) ? 1 : 0);
      // Get the bit just above that bit
      bitLeftmost = (int)((shiftedSmall >> ((shift) - 1)) & 0x01);
      shiftedSmall >>= shift;
      if (shiftingMoreBits) {
        // Shifted more bits than the bit length
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
      }
      bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
    }

    /// <summary> Shifts a number until it reaches the given number of bits,
    /// gathering information on whether the last bit discarded is set and
    /// whether the discarded bits to the right of that bit are set. Assumes
    /// that the big integer being shifted is positive. </summary>
    /// <returns></returns>
    /// <param name='bits'>A 32-bit signed integer.</param>
    public void ShiftToDigitsInt(int bits) {
      if(bits<0)
        throw new ArgumentException("bits is negative");
      if (isSmall)
        ShiftSmallToBits(bits);
      else
        ShiftBigToBits(bits);
    }

    private void ShiftSmallToBits(int bits) {
      int kbl = SmallBitLength;
      for (int i = SmallBitLength - 1; i >= 0; i++) {
        if ((shiftedSmall & (1L << i)) != 0) {
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
        knownBitLength = new FastInteger(bits);
        discardedBitCount.AddInt(bitShift);
        bitsAfterLeftmost |= bitLeftmost;
        // Get the bottommost shift minus 1 bits
        bitsAfterLeftmost |= (((shiftedSmall << (SmallBitLength + 1 - shift)) != 0) ? 1 : 0);
        // Get the bit just above that bit
        bitLeftmost = (int)((shiftedSmall >> (((int)shift) - 1)) & 0x01);
        bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
        shiftedSmall >>= shift;
      } else {
        knownBitLength=new FastInteger(kbl);
      }
    }
  }
}
