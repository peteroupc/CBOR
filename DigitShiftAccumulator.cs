/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;
using System.Numerics;
namespace PeterO {
  internal sealed class DigitShiftAccumulator : IShiftAccumulator {
    int bitLeftmost = 0;

    /// <summary> Gets whether the last discarded digit was set. </summary>
    public int LastDiscardedDigit {
      get { return bitLeftmost; }
    }
    int bitsAfterLeftmost = 0;

    /// <summary> Gets whether any of the discarded digits to the right of
    /// the last one was set. </summary>
    public int OlderDiscardedDigits {
      get { return bitsAfterLeftmost; }
    }
    BigInteger shiftedBigInt;
    long knownBitLength;

    /// <summary> Gets the length of the shifted value in digits. </summary>
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
    private static BigInteger Int64MaxValue = (BigInteger)Int64.MaxValue;

    public DigitShiftAccumulator(BigInteger bigint,
      int lastDiscarded,
      int olderDiscarded
      ) : this(bigint) {
        bitsAfterLeftmost = (olderDiscarded != 0) ? 1 : 0;
        bitLeftmost = lastDiscarded;
    }

    public DigitShiftAccumulator(BigInteger bigint) {
      if (bigint.Sign < 0)
        throw new ArgumentException("bigint is negative");
      discardedBitCount = new FastInteger();
      if (bigint.CompareTo(Int64MaxValue) <= 0) {
        shiftedLong = (long)bigint;
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
        throw new ArgumentException("longInt is negative");
      shiftedLong = longInt;
      discardedBitCount = new FastInteger();
      isSmall = true;
      knownBitLength = -1;
    }
    private static BigInteger FastParseBigInt(string str, int offset, int length) {
      MutableBigInteger mbi = new MutableBigInteger();
      for (int i = 0; i < length; i++) {
        int digit = (int)(str[offset + i] - '0');
        mbi.Multiply(10).Add(digit);
      }
      return mbi.ToBigInteger();
    }

    private static long FastParseLong(string str, int offset, int length) {
      // Assumes the string is length 18 or less and contains
      // only the digits '0' through '9'
      if (length > 18)
        throw new ArgumentException();
      long ret = 0;
      for (int i = 0; i < length; i++) {
        int digit = (int)(str[offset + i] - '0');
        ret *= 10;
        ret += digit;
      }
      return ret;
    }
    /// <summary> </summary>
    /// <param name='fastint'> A FastInteger object.</param>
    /// <returns></returns>
    /// <remarks/>
    public void ShiftRight(FastInteger fastint) {
      if ((fastint) == null) throw new ArgumentNullException("fastint");
      if (fastint.Sign <= 0) return;
      if (fastint.CanFitInInt64()) {
        ShiftRight(fastint.AsInt64());
      } else {
        BigInteger bi = fastint.AsBigInteger();
        while (bi.Sign > 0) {
          long count = 1000000;
          if (bi.CompareTo((BigInteger)1000000) < 0) {
            count = (long)bi;
          }
          ShiftRight(count);
          bi -= (BigInteger)count;
        }
      }
    }

    private void ShiftRightBig(long digits) {
      if (digits <= 0) return;
      if (shiftedBigInt.IsZero) {
        discardedBitCount.Add(digits);
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
        knownBitLength = 1;
        return;
      }
      String str = shiftedBigInt.ToString(
        System.Globalization.CultureInfo.InvariantCulture);
      // NOTE: Will be 1 if the value is 0
      long digitLength = str.Length;
      long bitDiff = 0;
      if (digits > digitLength) {
        bitDiff = digits - digitLength;
      }
      discardedBitCount.Add(digits);
      bitsAfterLeftmost |= bitLeftmost;
      long digitShift = Math.Min(digitLength, digits);
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
      for (int i = str.Length - 1; i >= 0; i--) {
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = (int)(str[i] - '0');
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
      (BigInteger)1, (BigInteger)10, (BigInteger)100, (BigInteger)1000, (BigInteger)10000, (BigInteger)100000, (BigInteger)1000000, (BigInteger)10000000, (BigInteger)100000000, (BigInteger)1000000000,
      (BigInteger)10000000000L, (BigInteger)100000000000L, (BigInteger)1000000000000L, (BigInteger)10000000000000L,
      (BigInteger)100000000000000L, (BigInteger)1000000000000000L, (BigInteger)10000000000000000L,
      (BigInteger)100000000000000000L, (BigInteger)1000000000000000000L
    };

    /// <summary> Shifts a number until it reaches the given number of digits,
    /// gathering information on whether the last digit discarded is set
    /// and whether the discarded digits to the right of that digit are set.
    /// Assumes that the big integer being shifted is positive. </summary>
    private void ShiftToBitsBig(long digits) {
      String str = shiftedBigInt.ToString(
        System.Globalization.CultureInfo.InvariantCulture);
      // NOTE: Will be 1 if the value is 0
      long digitLength = str.Length;
      knownBitLength = digitLength;
      // Shift by the difference in digit length
      if (digitLength > digits) {
        long digitShift = digitLength - digits;
        long bitShiftCount = digitShift;
        int newLength = (int)(digitLength - digitShift);
        discardedBitCount.Add(digitShift);
        for (int i = str.Length - 1; i >= 0; i--) {
          bitsAfterLeftmost |= bitLeftmost;
          bitLeftmost = (int)(str[i] - '0');
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
          shiftedBigInt /= (BigInteger)bigpow;
        } else {
          shiftedBigInt = FastParseBigInt(str, 0, newLength);
        }
        bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
      }
    }


    /// <summary> Shifts a number to the right, gathering information on
    /// whether the last digit discarded is set and whether the discarded
    /// digits to the right of that digit are set. Assumes that the big integer
    /// being shifted is positive. </summary>
    /// <returns></returns>
    /// <param name='digits'> A 64-bit signed integer.</param>
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

    /// <summary> Shifts a number until it reaches the given number of digits,
    /// gathering information on whether the last digit discarded is set
    /// and whether the discarded digits to the right of that digit are set.
    /// Assumes that the big integer being shifted is positive. </summary>
    /// <returns></returns>
    /// <param name='digits'> A 64-bit signed integer.</param>
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
        String str = shiftedBigInt.ToString(
          System.Globalization.CultureInfo.InvariantCulture);
        return str.Length;
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
        knownBitLength = Math.Max(1, newLength);
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
}