/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
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

    /// <summary> Gets whether any of the discarded bits to the right of the
    /// last one was set. </summary>
    public int OlderDiscardedDigits {
      get { return bitsAfterLeftmost; }
    }
    BigInteger shiftedBigInt;
    long knownBitLength;


    /// <summary> Gets the length of the shifted value in bits. </summary>
    public long DigitLength {
      get {
        if (knownBitLength < 0) {
          knownBitLength = CalcKnownBitLength();
        }
        return knownBitLength;
      }
    }
    long shiftedLong;
    bool isSmall;
    
    /// <summary> </summary>
    /// <remarks/>
public bool IsSmall{
    get { return isSmall; }
  }

    /// <summary> </summary>
    /// <remarks/>
    public BigInteger ShiftedInt {
      get {
        if (isSmall)
          return (BigInteger)shiftedLong;
        else
          return shiftedBigInt;
      }
    }
    /// <summary> </summary>
    /// <remarks/>
    public long ShiftedIntSmall {
      get {
        if (isSmall)
          return shiftedLong;
        else
          return (long)shiftedBigInt;
      }
    }
    FastInteger discardedBitCount;

    /// <summary> </summary>
    /// <remarks/>
    public FastInteger DiscardedDigitCount {
      get { return discardedBitCount; }
    }

    public BitShiftAccumulator(BigInteger bigint,
      int lastDiscarded,
      int olderDiscarded
      ) : this(bigint) {
        bitsAfterLeftmost = (olderDiscarded != 0) ? 1 : 0;
        bitLeftmost = (lastDiscarded != 0) ? 1 : 0;
    }
    public BitShiftAccumulator(BigInteger bigint) {
      if (bigint.Sign < 0)
        throw new ArgumentException("bigint is negative");
      shiftedBigInt = bigint;
      discardedBitCount = new FastInteger();
      knownBitLength = -1;
    }
    public BitShiftAccumulator(long longInt) {
      if (longInt < 0)
        throw new ArgumentException("longInt is negative");
      shiftedLong = longInt;
      discardedBitCount = new FastInteger();
      isSmall = true;
      knownBitLength = -1;
    }
    /// <summary> </summary>
    /// <param name='fastint'> A FastInteger object.</param>
    /// <returns></returns>
    /// <remarks/>
    public void ShiftRight(FastInteger fastint) {
      if (fastint.Sign <= 0) return;
      if (fastint.CanFitInInt32()) {
        ShiftRight(fastint.AsInt32());
      } else {
        BigInteger bi = fastint.AsBigInteger();
        while (bi.Sign > 0) {
          int count = 1000000;
          if (bi.CompareTo((BigInteger)1000000) < 0) {
            count = (int)bi;
          }
          ShiftRight(count);
          bi -= (BigInteger)count;
        }
      }
    }

    private void ShiftRightBig(int bits) {
      if (bits <= 0) return;
      if (shiftedBigInt.IsZero) {
        discardedBitCount.Add(bits);
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
        knownBitLength = 1;
        return;
      }
      byte[] bytes = shiftedBigInt.ToByteArray();
      knownBitLength = ((long)bytes.Length) << 3;
      // Find the last bit set
      for (int i = bytes.Length - 1; i >= 0; i--) {
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
      long bitShift = Math.Min(knownBitLength, bits);
      if (bits >= knownBitLength) {
        isSmall = true;
        shiftedLong = 0;
        knownBitLength = 1;
      } else {
        long tmpBitShift = bitShift;
        while (tmpBitShift > 0 && !shiftedBigInt.IsZero) {
          int bs = (int)Math.Min(1000000, tmpBitShift);
          shiftedBigInt >>= bs;
          tmpBitShift -= bs;
        }
        knownBitLength = knownBitLength - bitShift;
      }
      discardedBitCount.Add(bits);
      bitsAfterLeftmost |= bitLeftmost;
      for (int i = 0; i < bytes.Length; i++) {
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
        byte[] bytes = shiftedBigInt.ToByteArray();
        long kb = ((long)bytes.Length) << 3;
        // Find the last bit set
        for (int i = bytes.Length - 1; i >= 0; i--) {
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

    /// <summary> Shifts a number until it reaches the given number of bits,
    /// gathering information on whether the last bit discarded is set and
    /// whether the discarded bits to the right of that bit are set. Assumes
    /// that the big integer being shifted is positive. </summary>
    private void ShiftToBitsBig(long bits) {
      byte[] bytes = shiftedBigInt.ToByteArray();
      knownBitLength = ((long)bytes.Length) << 3;
      // Find the last bit set
      for (int i = bytes.Length - 1; i >= 0; i--) {
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
        while (tmpBitShift > 0 && !shiftedBigInt.IsZero) {
          int bs = (int)Math.Min(1000000, tmpBitShift);
          shiftedBigInt >>= bs;
          tmpBitShift -= bs;
        }
        knownBitLength = bits;
        if (bits <= 63) {
          // Shifting to 63 bits or fewer,
          // convert to small integer
          isSmall = true;
          shiftedLong = (long)shiftedBigInt;
        }
        bitsAfterLeftmost |= bitLeftmost;
        discardedBitCount.Add(bitShift);
        for (int i = 0; i < bytes.Length; i++) {
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


    /// <summary> Shifts a number to the right, gathering information on
    /// whether the last bit discarded is set and whether the discarded bits
    /// to the right of that bit are set. Assumes that the big integer being
    /// shifted is positive. </summary>
    /// <returns></returns>
    /// <param name='bits'> A 64-bit signed integer.</param>
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
      int shift = (int)Math.Min(knownBitLength, bits);
      bool shiftingMoreBits = (bits > knownBitLength);
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

    /// <summary> Shifts a number until it reaches the given number of bits,
    /// gathering information on whether the last bit discarded is set and
    /// whether the discarded bits to the right of that bit are set. Assumes
    /// that the big integer being shifted is positive. </summary>
    /// <returns></returns>
    /// <param name='bits'>A 64-bit signed integer.</param>
    public void ShiftToDigits(long bits) {
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
}