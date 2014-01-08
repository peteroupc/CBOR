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
  internal sealed class DigitShiftAccumulator : IShiftAccumulator {
    int bitLeftmost;

    /// <summary> Gets whether the last discarded digit was set. </summary>
    public int LastDiscardedDigit {
      get { return bitLeftmost; }
    }
    int bitsAfterLeftmost;

    /// <summary> Gets whether any of the discarded digits to the right of
    /// the last one was set. </summary>
    public int OlderDiscardedDigits {
      get { return bitsAfterLeftmost; }
    }
    BigInteger shiftedBigInt;
    FastInteger knownBitLength;

    /// <summary> </summary>
    /// <returns>A FastInteger object.</returns>
    public FastInteger GetDigitLength(){
      if (knownBitLength==null) {
        knownBitLength = CalcKnownDigitLength();
      }
      return FastInteger.Copy(knownBitLength);
    }

    int shiftedSmall;
    bool isSmall;

    FastInteger discardedBitCount;

    /// <summary> </summary>
    public FastInteger DiscardedDigitCount {
      get {
        if(discardedBitCount==null)
          discardedBitCount=new FastInteger(0);
        return discardedBitCount;
      }
    }
    private static BigInteger Int32MaxValue = (BigInteger)Int32.MaxValue;
    private static BigInteger Ten = (BigInteger)10;

    /// <summary> </summary>
    public BigInteger ShiftedInt{
      get {
        if(isSmall)
          return (BigInteger)shiftedSmall;
        else
          return shiftedBigInt;
      }
    }

    public DigitShiftAccumulator(BigInteger bigint,
                                 int lastDiscarded,
                                 int olderDiscarded
                                ){
      if(bigint.canFitInInt()){
        shiftedSmall=(int)bigint;
        if(shiftedSmall<0)
          throw new ArgumentException("bigint is negative");
        isSmall=true;
      } else {
        shiftedBigInt = bigint;
        isSmall = false;
      }
      bitsAfterLeftmost = (olderDiscarded != 0) ? 1 : 0;
      bitLeftmost = lastDiscarded;
    }

    private static int FastParseLong(string str, int offset, int length) {
      // Assumes the string is length 9 or less and contains
      // only the digits '0' through '9'
      if((length)>9)throw new ArgumentException(
        "length"+" not less or equal to "+"9"+" ("+
        Convert.ToString((length),System.Globalization.CultureInfo.InvariantCulture)+")");
      int ret = 0;
      for (int i = 0; i < length; i++) {
        int digit = (int)(str[offset + i] - '0');
        ret *= 10;
        ret += digit;
      }
      return ret;
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
    /// <summary> </summary>
    /// <param name='fastint'>A FastInteger object.</param>
    /// <returns></returns>
    public void ShiftRight(FastInteger fastint) {
      if ((fastint) == null) throw new ArgumentNullException("fastint");
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
          if(isSmall ? shiftedSmall==0 : shiftedBigInt.IsZero){
            break;
          }
        }
      }
    }

    private void ShiftRightBig(int digits) {
      if (digits <= 0) return;
      if (shiftedBigInt.IsZero) {
        if(discardedBitCount==null)
          discardedBitCount=new FastInteger(0);
        discardedBitCount.AddInt(digits);
        bitsAfterLeftmost |= bitLeftmost;
        bitLeftmost = 0;
        knownBitLength = new FastInteger(1);
        return;
      }
      //Console.WriteLine("digits={0}",digits);
      if(digits==1){
        BigInteger bigrem;
        BigInteger bigquo=BigInteger.DivRem(shiftedBigInt,(BigInteger)10,
                                            out bigrem);
        bitsAfterLeftmost|=bitLeftmost;
        bitLeftmost=(int)bigrem;
        shiftedBigInt=bigquo;
        if(discardedBitCount==null)
          discardedBitCount=new FastInteger(0);
        discardedBitCount.AddInt(digits);
        if(knownBitLength!=null){
          if(bigquo.IsZero)
            knownBitLength.SetInt(0);
          else
            knownBitLength.Decrement();
        }
        return;
      }
      int startCount=Math.Min(4,digits-1);
      if(startCount>0){
        BigInteger bigrem;
        BigInteger radixPower=DecimalUtility.FindPowerOfTen(startCount);
        BigInteger bigquo=BigInteger.DivRem(shiftedBigInt,radixPower,
                                            out bigrem);
        if(!bigrem.IsZero)
          bitsAfterLeftmost|=1;
        bitsAfterLeftmost|=bitLeftmost;
        shiftedBigInt=bigquo;
        if(discardedBitCount==null)
          discardedBitCount=new FastInteger(0);
        discardedBitCount.AddInt(startCount);
        digits-=startCount;
        if(shiftedBigInt.IsZero){
          // Shifted all the way to 0
          isSmall=true;
          shiftedSmall=0;
          knownBitLength=new FastInteger(1);
          bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
          bitLeftmost=0;
          return;
        }
      }

      String str = shiftedBigInt.ToString();
      // NOTE: Will be 1 if the value is 0
      int digitLength = str.Length;
      int bitDiff = 0;
      if (digits > digitLength) {
        bitDiff = digits - digitLength;
      }
      if(discardedBitCount==null)
        discardedBitCount=new FastInteger(0);
      discardedBitCount.AddInt(digits);
      bitsAfterLeftmost |= bitLeftmost;
      int digitShift = Math.Min(digitLength, digits);
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
          shiftedBigInt = BigInteger.fromSubstring(str, 0, newLength);
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

    /// <summary> Shifts a number until it reaches the given number of digits,
    /// gathering information on whether the last digit discarded is set
    /// and whether the discarded digits to the right of that digit are set.
    /// Assumes that the big integer being shifted is positive. </summary>
    private void ShiftToBitsBig(int digits) {
      if(knownBitLength!=null){
        if(knownBitLength.CompareToInt(digits)<=0){
          return;
        }
      }
      String str;
      knownBitLength=GetDigitLength();
      if(knownBitLength.CompareToInt(digits)<=0){
        return;
      }
      FastInteger digitDiff=FastInteger.Copy(knownBitLength).SubtractInt(digits);
      if(digitDiff.CompareToInt(1)==0){
        BigInteger bigrem;
        BigInteger bigquo=BigInteger.DivRem(shiftedBigInt,Ten,out bigrem);
        bitsAfterLeftmost|=bitLeftmost;
        bitLeftmost=(int)bigrem;
        shiftedBigInt=bigquo;
        if(discardedBitCount==null)
          discardedBitCount=new FastInteger(0);
        discardedBitCount.Add(digitDiff);
        knownBitLength.Subtract(digitDiff);
        bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
        return;
      } else if(digitDiff.CompareToInt(9)<=0){
        BigInteger bigrem;
        int diffInt=digitDiff.AsInt32();
        BigInteger radixPower=DecimalUtility.FindPowerOfTen(diffInt);
        BigInteger bigquo=BigInteger.DivRem(shiftedBigInt,radixPower,
                                            out bigrem);
        int rem=(int)bigrem;
        bitsAfterLeftmost|=bitLeftmost;
        for(int i=0;i<diffInt;i++){
          if(i==diffInt-1){
            bitLeftmost=rem%10;
          } else {
            bitsAfterLeftmost|=(rem%10);
            rem/=10;
          }
        }
        shiftedBigInt=bigquo;
        if(discardedBitCount==null)
          discardedBitCount=new FastInteger(0);
        discardedBitCount.Add(digitDiff);
        knownBitLength.Subtract(digitDiff);
        bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
        return;
      } else if(digitDiff.CompareToInt(Int32.MaxValue)<=0){
        BigInteger bigrem;
        BigInteger radixPower=DecimalUtility.FindPowerOfTen(digitDiff.AsInt32()-1);
        BigInteger bigquo=BigInteger.DivRem(shiftedBigInt,radixPower,
                                            out bigrem);
        bitsAfterLeftmost|=bitLeftmost;
        if(!bigrem.IsZero)
          bitsAfterLeftmost|=1;
        {
          BigInteger bigquo2=BigInteger.DivRem(bigquo,Ten,out bigrem);
          this.bitLeftmost=(int)bigrem;
          shiftedBigInt=bigquo2;
        }
        if(discardedBitCount==null)
          discardedBitCount=new FastInteger(0);
        discardedBitCount.Add(digitDiff);
        knownBitLength.Subtract(digitDiff);
        bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
        return;
      }
      str=shiftedBigInt.ToString();
      // NOTE: Will be 1 if the value is 0
      int digitLength = str.Length;
      knownBitLength = new FastInteger(digitLength);
      // Shift by the difference in digit length
      if (digitLength > digits) {
        int digitShift = digitLength - digits;
        knownBitLength.SubtractInt(digitShift);
        int newLength = (int)(digitLength - digitShift);
        //Console.WriteLine("dlen={0} dshift={1} newlen={2}",digitLength,
        //                digitShift,newLength);
        if(discardedBitCount==null)
          discardedBitCount=new FastInteger(0);
        if(digitShift<=Int32.MaxValue)
          discardedBitCount.AddInt((int)digitShift);
        else
          discardedBitCount.AddBig((BigInteger)digitShift);
        for (int i = str.Length - 1; i >= 0; i--) {
          bitsAfterLeftmost |= bitLeftmost;
          bitLeftmost = (int)(str[i] - '0');
          digitShift--;
          if (digitShift <= 0) {
            break;
          }
        }
        if (newLength <= 9) {
          isSmall = true;
          shiftedSmall = FastParseLong(str, 0, newLength);
        } else {
          shiftedBigInt = BigInteger.fromSubstring(str, 0, newLength);
        }
        bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
      }
    }

    /// <summary> Shifts a number to the right, gathering information on
    /// whether the last digit discarded is set and whether the discarded
    /// digits to the right of that digit are set. Assumes that the big integer
    /// being shifted is positive. </summary>
    /// <returns></returns>
    /// <param name='digits'>A 32-bit signed integer.</param>
    public void ShiftRightInt(int digits) {
      if (isSmall)
        ShiftRightSmall(digits);
      else
        ShiftRightBig(digits);
    }
    private void ShiftRightSmall(int digits) {
      if (digits <= 0) return;
      if (shiftedSmall == 0) {
        if(discardedBitCount==null)
          discardedBitCount=new FastInteger(0);
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
      if(discardedBitCount==null)
        discardedBitCount=new FastInteger(0);
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
          knownBitLength.Decrement();
        }
      }
      bitsAfterLeftmost = (bitsAfterLeftmost != 0) ? 1 : 0;
    }

    /// <summary> </summary>
    /// <param name='bits'>A FastInteger object.</param>
    /// <returns></returns>
    public void ShiftToDigits(FastInteger bits){
      if(bits.CanFitInInt32()){
        int intval=bits.AsInt32();
        if(intval<0)
          throw new ArgumentException("bits is negative");
        ShiftToDigitsInt(intval);
      } else {
        if(bits.Sign<0)
          throw new ArgumentException("bits is negative");
        knownBitLength=CalcKnownDigitLength();
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

    /// <summary> Shifts a number until it reaches the given number of digits,
    /// gathering information on whether the last digit discarded is set
    /// and whether the discarded digits to the right of that digit are set.
    /// Assumes that the big integer being shifted is positive. </summary>
    /// <returns></returns>
    /// <param name='digits'>A 64-bit signed integer.</param>
    public void ShiftToDigitsInt(int digits) {
      if (isSmall)
        ShiftToBitsSmall(digits);
      else
        ShiftToBitsBig(digits);
    }

    private FastInteger CalcKnownDigitLength() {
      if (isSmall) {
        int kb = 0;
        int v2=shiftedSmall;
        if(v2>=1000000000)kb=10;
        else if(v2>=100000000)kb=9;
        else if(v2>=10000000)kb=8;
        else if(v2>=1000000)kb=7;
        else if(v2>=100000)kb=6;
        else if(v2>=10000)kb=5;
        else if(v2>=1000)kb=4;
        else if(v2>=100)kb=3;
        else if(v2>=10)kb=2;
        else kb=1;
        return new FastInteger(kb);
      } else {
        return new FastInteger(shiftedBigInt.getDigitCount());
      }
    }
    private void ShiftToBitsSmall(int digits) {
      int kb=0;
      int v2=shiftedSmall;
      if(v2>=1000000000)kb=10;
      else if(v2>=100000000)kb=9;
      else if(v2>=10000000)kb=8;
      else if(v2>=1000000)kb=7;
      else if(v2>=100000)kb=6;
      else if(v2>=10000)kb=5;
      else if(v2>=1000)kb=4;
      else if(v2>=100)kb=3;
      else if(v2>=10)kb=2;
      else kb=1;
      knownBitLength=new FastInteger(kb);
      if (kb > digits) {
        int digitShift = (int)(kb - digits);
        int newLength = (int)(kb - digitShift);
        knownBitLength = new FastInteger(Math.Max(1, newLength));
        if(discardedBitCount==null)
          discardedBitCount=new FastInteger(digitShift);
        else
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
}
