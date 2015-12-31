/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Numbers {
  internal sealed class DigitShiftAccumulator : IShiftAccumulator {
    private int bitLeftmost;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.DigitShiftAccumulator.LastDiscardedDigit"]/*'/>
    public int LastDiscardedDigit {
      get {
        return this.bitLeftmost;
      }
    }

    private int bitsAfterLeftmost;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.DigitShiftAccumulator.OlderDiscardedDigits"]/*'/>
    public int OlderDiscardedDigits {
      get {
        return this.bitsAfterLeftmost;
      }
    }

    private EInteger shiftedBigInt;
    private FastInteger knownBitLength;

    public FastInteger GetDigitLength() {
      this.knownBitLength = this.knownBitLength ?? this.CalcKnownDigitLength();
      return FastInteger.Copy(this.knownBitLength);
    }

    private int shiftedSmall;
    private bool isSmall;
    private FastInteger discardedBitCount;

    public FastInteger DiscardedDigitCount {
      get {
        this.discardedBitCount = this.discardedBitCount ?? (new FastInteger(0));
        return this.discardedBitCount;
      }
    }

    private static readonly EInteger ValueTen = (EInteger)10;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.DigitShiftAccumulator.ShiftedInt"]/*'/>
    public EInteger ShiftedInt {
      get {
        return this.isSmall ? ((EInteger)this.shiftedSmall) :
        this.shiftedBigInt;
      }
    }

    public DigitShiftAccumulator(
EInteger bigint,
int lastDiscarded,
int olderDiscarded) {
      if (bigint.canFitInInt()) {
        this.shiftedSmall = (int)bigint;
        if (this.shiftedSmall < 0) {
          throw new ArgumentException("shiftedSmall (" + this.shiftedSmall +
            ") is less than 0");
        }
        this.isSmall = true;
      } else {
        this.shiftedBigInt = bigint;
        this.isSmall = false;
      }
      this.bitsAfterLeftmost = (olderDiscarded != 0) ? 1 : 0;
      this.bitLeftmost = lastDiscarded;
    }

    private static int FastParseLong(string str, int offset, int length) {
      // Assumes the string is length 9 or less and contains
      // only the digits '0' through '9'
      if (length > 9) {
   throw new ArgumentException("length (" + length + ") is more than " +
          "9 ");
      }
      var ret = 0;
      for (var i = 0; i < length; ++i) {
        var digit = (int)(str[offset + i] - '0');
        ret *= 10;
        ret += digit;
      }
      return ret;
    }

    public FastInteger ShiftedIntFast {
      get {
        return this.isSmall ? (new FastInteger(this.shiftedSmall)) :
        FastInteger.FromBig(this.shiftedBigInt);
      }
    }

    public void ShiftRight(FastInteger fastint) {
      if (fastint == null) {
        throw new ArgumentNullException("fastint");
      }
      if (fastint.Sign <= 0) {
        return;
      }
      if (fastint.CanFitInInt32()) {
        this.ShiftRightInt(fastint.AsInt32());
      } else {
        EInteger bi = fastint.AsBigInteger();
        while (bi.Sign > 0) {
          var count = 1000000;
          if (bi.CompareTo((EInteger)1000000) < 0) {
            count = (int)bi;
          }
          this.ShiftRightInt(count);
          bi -= (EInteger)count;
          if (this.isSmall ? this.shiftedSmall == 0 :
          this.shiftedBigInt.IsZero) {
            break;
          }
        }
      }
    }

    private void ShiftRightBig(int digits) {
      if (digits <= 0) {
        return;
      }
      if (this.shiftedBigInt.IsZero) {
        this.discardedBitCount = this.discardedBitCount ?? (new FastInteger(0));
        this.discardedBitCount.AddInt(digits);
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitLeftmost = 0;
        this.knownBitLength = new FastInteger(1);
        return;
      }
      // Console.WriteLine("digits=" + digits);
      if (digits == 1) {
        EInteger bigrem;
        EInteger bigquo;
{
EInteger[] divrem = this.shiftedBigInt.DivRem((EInteger)10);
bigquo = divrem[0];
bigrem = divrem[1]; }
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitLeftmost = (int)bigrem;
        this.shiftedBigInt = bigquo;
        this.discardedBitCount = this.discardedBitCount ?? (new FastInteger(0));
        this.discardedBitCount.AddInt(digits);
        if (this.knownBitLength != null) {
          if (bigquo.IsZero) {
            this.knownBitLength.SetInt(0);
          } else {
            this.knownBitLength.Decrement();
          }
        }
        return;
      }
      int startCount = Math.Min(4, digits - 1);
      if (startCount > 0) {
        EInteger bigrem;
        EInteger radixPower = DecimalUtility.FindPowerOfTen(startCount);
        EInteger bigquo;
{
EInteger[] divrem = this.shiftedBigInt.DivRem(radixPower);
bigquo = divrem[0];
bigrem = divrem[1]; }
        if (!bigrem.IsZero) {
          this.bitsAfterLeftmost |= 1;
        }
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.shiftedBigInt = bigquo;
        this.discardedBitCount = this.discardedBitCount ?? (new FastInteger(0));
        this.discardedBitCount.AddInt(startCount);
        digits -= startCount;
        if (this.shiftedBigInt.IsZero) {
          // Shifted all the way to 0
          this.isSmall = true;
          this.shiftedSmall = 0;
          this.knownBitLength = new FastInteger(1);
          this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
          this.bitLeftmost = 0;
          return;
        }
      }
      if (digits == 1) {
        EInteger bigrem;
        EInteger bigquo;
{
EInteger[] divrem = this.shiftedBigInt.DivRem(ValueTen);
bigquo = divrem[0];
bigrem = divrem[1]; }
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitLeftmost = (int)bigrem;
        this.shiftedBigInt = bigquo;
        this.discardedBitCount = this.discardedBitCount ?? (new FastInteger(0));
        this.discardedBitCount.Increment();
        this.knownBitLength = (this.knownBitLength != null) ?
        this.knownBitLength.Decrement() : this.GetDigitLength();
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        return;
      }
      this.knownBitLength = this.knownBitLength ?? this.GetDigitLength();
      if (new FastInteger(digits).Decrement().CompareTo(this.knownBitLength)
      >= 0) {
        // Shifting more bits than available
        this.bitsAfterLeftmost |= this.shiftedBigInt.IsZero ? 0 : 1;
        this.isSmall = true;
        this.shiftedSmall = 0;
        this.knownBitLength = new FastInteger(1);
        this.discardedBitCount = this.discardedBitCount ?? (new FastInteger(0));
        this.discardedBitCount.AddInt(digits);
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitLeftmost = 0;
        return;
      }
      if (this.shiftedBigInt.canFitInInt()) {
        this.isSmall = true;
        this.shiftedSmall = (int)this.shiftedBigInt;
        this.ShiftRightSmall(digits);
        return;
      }
      string str = this.shiftedBigInt.ToString();
      // NOTE: Will be 1 if the value is 0
      int digitLength = str.Length;
      var bitDiff = 0;
      if (digits > digitLength) {
        bitDiff = digits - digitLength;
      }
      this.discardedBitCount = this.discardedBitCount ?? (new FastInteger(0));
      this.discardedBitCount.AddInt(digits);
      this.bitsAfterLeftmost |= this.bitLeftmost;
      int digitShift = Math.Min(digitLength, digits);
      if (digits >= digitLength) {
        this.isSmall = true;
        this.shiftedSmall = 0;
        this.knownBitLength = new FastInteger(1);
      } else {
        var newLength = (int)(digitLength - digitShift);
        this.knownBitLength = new FastInteger(newLength);
        if (newLength <= 9) {
          // Fits in a small number
          this.isSmall = true;
          this.shiftedSmall = FastParseLong(str, 0, newLength);
        } else {
          this.shiftedBigInt = EInteger.FromSubstring(str, 0, newLength);
        }
      }
      for (int i = str.Length - 1; i >= 0; --i) {
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitLeftmost = (int)(str[i] - '0');
        --digitShift;
        if (digitShift <= 0) {
          break;
        }
      }
      this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
      if (bitDiff > 0) {
        // Shifted more digits than the digit length
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitLeftmost = 0;
      }
    }

    private void ShiftToBitsBig(int digits) {
      // Shifts a number until it reaches the given number of digits,
      // gathering information on whether the last digit discarded is set
      // and whether the discarded digits to the right of that digit are set.
      // Assumes that the big integer being shifted is positive.
      if (this.knownBitLength != null) {
        if (this.knownBitLength.CompareToInt(digits) <= 0) {
          return;
        }
      }
      string str;
      this.knownBitLength = this.knownBitLength ?? this.GetDigitLength();
      if (this.knownBitLength.CompareToInt(digits) <= 0) {
        return;
      }
      FastInteger digitDiff =
      FastInteger.Copy(this.knownBitLength).SubtractInt(digits);
      if (digitDiff.CompareToInt(1) == 0) {
        EInteger bigrem;
        EInteger bigquo;
{
EInteger[] divrem = this.shiftedBigInt.DivRem(ValueTen);
bigquo = divrem[0];
bigrem = divrem[1]; }
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitLeftmost = (int)bigrem;
        this.shiftedBigInt = bigquo;
        this.discardedBitCount = this.discardedBitCount ?? (new FastInteger(0));
        this.discardedBitCount.Add(digitDiff);
        this.knownBitLength.Subtract(digitDiff);
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        return;
      }
      if (digitDiff.CompareToInt(9) <= 0) {
        EInteger bigrem;
        int diffInt = digitDiff.AsInt32();
        EInteger radixPower = DecimalUtility.FindPowerOfTen(diffInt);
        EInteger bigquo;
{
EInteger[] divrem = this.shiftedBigInt.DivRem(radixPower);
bigquo = divrem[0];
bigrem = divrem[1]; }
        var rem = (int)bigrem;
        this.bitsAfterLeftmost |= this.bitLeftmost;
        for (var i = 0; i < diffInt; ++i) {
          if (i == diffInt - 1) {
            this.bitLeftmost = rem % 10;
          } else {
            this.bitsAfterLeftmost |= rem % 10;
            rem /= 10;
          }
        }
        this.shiftedBigInt = bigquo;
        this.discardedBitCount = this.discardedBitCount ?? (new FastInteger(0));
        this.discardedBitCount.Add(digitDiff);
        this.knownBitLength.Subtract(digitDiff);
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        return;
      }
      if (digitDiff.CompareToInt(Int32.MaxValue) <= 0) {
        EInteger bigrem;
        EInteger radixPower =
        DecimalUtility.FindPowerOfTen(digitDiff.AsInt32() - 1);
        EInteger bigquo;
{
EInteger[] divrem = this.shiftedBigInt.DivRem(radixPower);
bigquo = divrem[0];
bigrem = divrem[1]; }
        this.bitsAfterLeftmost |= this.bitLeftmost;
        if (!bigrem.IsZero) {
          this.bitsAfterLeftmost |= 1;
        }
        {
          EInteger bigquo2;
{
EInteger[] divrem = bigquo.DivRem(ValueTen);
bigquo2 = divrem[0];
bigrem = divrem[1]; }
          this.bitLeftmost = (int)bigrem;
          this.shiftedBigInt = bigquo2;
        }
        this.discardedBitCount = this.discardedBitCount ?? (new FastInteger(0));
        this.discardedBitCount.Add(digitDiff);
        this.knownBitLength.Subtract(digitDiff);
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        return;
      }
      str = this.shiftedBigInt.ToString();
      // NOTE: Will be 1 if the value is 0
      int digitLength = str.Length;
      this.knownBitLength = new FastInteger(digitLength);
      // Shift by the difference in digit length
      if (digitLength > digits) {
        int digitShift = digitLength - digits;
        this.knownBitLength.SubtractInt(digitShift);
        var newLength = (int)(digitLength - digitShift);
        // Console.WriteLine("dlen= " + digitLength + " dshift=" +
        // digitShift + " newlen= " + newLength);
        this.discardedBitCount = this.discardedBitCount ?? (new FastInteger(0));
        if (digitShift <= Int32.MaxValue) {
          this.discardedBitCount.AddInt((int)digitShift);
        } else {
          this.discardedBitCount.AddBig((EInteger)digitShift);
        }
        for (int i = str.Length - 1; i >= 0; --i) {
          this.bitsAfterLeftmost |= this.bitLeftmost;
          this.bitLeftmost = (int)(str[i] - '0');
          --digitShift;
          if (digitShift <= 0) {
            break;
          }
        }
        if (newLength <= 9) {
          this.isSmall = true;
          this.shiftedSmall = FastParseLong(str, 0, newLength);
        } else {
          this.shiftedBigInt = EInteger.FromSubstring(str, 0, newLength);
        }
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
      }
    }

    public void ShiftRightInt(int digits) {
      // <summary>Shifts a number to the right, gathering information on
      // whether the last digit discarded is set and whether the discarded
      // digits to the right of that digit are set. Assumes that the big
      // integer being shifted is positive.</summary>
      if (this.isSmall) {
        this.ShiftRightSmall(digits);
      } else {
        this.ShiftRightBig(digits);
      }
    }

    private void ShiftRightSmall(int digits) {
      if (digits <= 0) {
        return;
      }
      if (this.shiftedSmall == 0) {
        this.discardedBitCount = this.discardedBitCount ?? (new FastInteger(0));
        this.discardedBitCount.AddInt(digits);
        this.bitsAfterLeftmost |= this.bitLeftmost;
        this.bitLeftmost = 0;
        this.knownBitLength = new FastInteger(1);
        return;
      }
      var kb = 0;
      int tmp = this.shiftedSmall;
      while (tmp > 0) {
        ++kb;
        tmp /= 10;
      }
      // Make sure digit length is 1 if value is 0
      if (kb == 0) {
        ++kb;
      }
      this.knownBitLength = new FastInteger(kb);
      this.discardedBitCount = this.discardedBitCount ?? (new FastInteger(0));
      this.discardedBitCount.AddInt(digits);
      while (digits > 0) {
        if (this.shiftedSmall == 0) {
          this.bitsAfterLeftmost |= this.bitLeftmost;
          this.bitLeftmost = 0;
          this.knownBitLength = new FastInteger(0);
          break;
        } else {
          var digit = (int)(this.shiftedSmall % 10);
          this.bitsAfterLeftmost |= this.bitLeftmost;
          this.bitLeftmost = digit;
          --digits;
          this.shiftedSmall /= 10;
          this.knownBitLength.Decrement();
        }
      }
      this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
    }

    public void ShiftToDigits(FastInteger bits) {
      if (bits.CanFitInInt32()) {
        int intval = bits.AsInt32();
        if (intval < 0) {
   throw new ArgumentException("intval (" + intval + ") is less than " +
            "0");
        }
        this.ShiftToDigitsInt(intval);
      } else {
        if (bits.Sign < 0) {
          throw new ArgumentException("bits's sign (" + bits.Sign +
            ") is less than 0");
        }
        this.knownBitLength = this.CalcKnownDigitLength();
        EInteger bigintDiff = this.knownBitLength.AsBigInteger();
        EInteger bitsBig = bits.AsBigInteger();
        bigintDiff -= (EInteger)bitsBig;
        if (bigintDiff.Sign > 0) {
          // current length is greater than the
          // desired bit length
          this.ShiftRight(FastInteger.FromBig(bigintDiff));
        }
      }
    }

    public void ShiftToDigitsInt(int digits) {
      // <summary>Shifts a number until it reaches the given number of
      // digits, gathering information on whether the last digit discarded
      // is set and whether the discarded digits to the right of that digit
      // are set. Assumes that the big integer being shifted is
      // positive.</summary>
      if (this.isSmall) {
        this.ShiftToBitsSmall(digits);
      } else {
        this.ShiftToBitsBig(digits);
      }
    }

    private FastInteger CalcKnownDigitLength() {
      if (this.isSmall) {
        var kb = 0;
        int v2 = this.shiftedSmall;
        kb = (v2 >= 1000000000) ? 10 : ((v2 >= 100000000) ? 9 : ((v2 >=
        10000000) ? 8 : ((v2 >= 1000000) ? 7 : ((v2 >= 100000) ? 6 : ((v2 >=
        10000) ? 5 : ((v2 >= 1000) ? 4 : ((v2 >= 100) ? 3 : ((v2 >= 10) ? 2 :
        1))))))));
        return new FastInteger(kb);
      }
      return new FastInteger(this.shiftedBigInt.getDigitCount());
    }

    private void ShiftToBitsSmall(int digits) {
      var kb = 0;
      int v2 = this.shiftedSmall;
      kb = (v2 >= 1000000000) ? 10 : ((v2 >= 100000000) ? 9 : ((v2 >=
      10000000) ? 8 : ((v2 >= 1000000) ? 7 : ((v2 >= 100000) ? 6 : ((v2 >=
      10000) ? 5 : ((v2 >= 1000) ? 4 : ((v2 >= 100) ? 3 : ((v2 >= 10) ? 2 :
      1))))))));
      this.knownBitLength = new FastInteger(kb);
      if (kb > digits) {
        var digitShift = (int)(kb - digits);
        var newLength = (int)(kb - digitShift);
        this.knownBitLength = new FastInteger(Math.Max(1, newLength));
        this.discardedBitCount = this.discardedBitCount != null ?
          this.discardedBitCount.AddInt(digitShift) :
          (new FastInteger(digitShift));
        for (var i = 0; i < digitShift; ++i) {
          var digit = (int)(this.shiftedSmall % 10);
          this.shiftedSmall /= 10;
          this.bitsAfterLeftmost |= this.bitLeftmost;
          this.bitLeftmost = digit;
        }
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
      }
    }
  }
}
