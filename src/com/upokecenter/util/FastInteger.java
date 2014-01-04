package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

//import java.math.*;

    /**
     * A mutable integer class initially backed by a small integer, that
     * only uses a big integer when arithmetic operations would overflow
     * the small integer. <p> This class is ideal for cases where operations
     * should be arbitrary precision, but the need to use a high precision
     * is rare.</p> <p> Many methods in this class return a reference to the
     * same object as used in the call. This allows chaining operations in
     * a single line of code. For example:</p> <code> fastInt.Add(5).Multiply(10);</code>
     */
  final class FastInteger implements Comparable<FastInteger> {

    private static final class MutableNumber {
      int[] data;
      int wordCount;
      public static MutableNumber FromBigInteger(BigInteger bigintVal) {
        MutableNumber mnum=new MutableNumber(0);
        if ( bigintVal.signum() < 0)
          throw new IllegalArgumentException("Only positive integers are supported");
        byte[] bytes=bigintVal.toByteArray(true);
        int len=bytes.length;
        int newWordCount=Math.max(4,(len/4)+1);
        if(newWordCount>mnum.data.length){
          mnum.data=new int[newWordCount];
        }
        mnum.wordCount=newWordCount;
        {
          for(int i=0;i<len;i+=4){
            int x=((int)bytes[i])&0xFF;
            if(i+1<len){
              x|=(((int)bytes[i+1])&0xFF)<<8;
            }
            if(i+2<len){
              x|=(((int)bytes[i+2])&0xFF)<<16;
            }
            if(i+3<len){
              x|=(((int)bytes[i+3])&0xFF)<<24;
            }
            mnum.data[i>>2]=x;
          }
        }
        // Calculate the correct data length
        while(mnum.wordCount!=0 && mnum.data[mnum.wordCount-1]==0)
          mnum.wordCount--;
        return mnum;
      }
      public MutableNumber (int val) {
        if (val < 0)
          throw new IllegalArgumentException("Only positive integers are supported");
        data = new int[4];
        wordCount = (val==0) ? 0 : 1;
        data[0] = ((int)((val) & 0xFFFFFFFFL));
      }

    /**
     *
     * @param val A 32-bit signed integer.
     * @return A MutableNumber object.
     */
      public MutableNumber SetInt(int val) {
        if(val<0)
          throw new IllegalArgumentException("Only positive integers are supported");
        wordCount = (val==0) ? 0 : 1;
        data[0] = ((int)((val) & 0xFFFFFFFFL));
        return this;
      }

    /**
     *
     * @return A BigInteger object.
     */
      public BigInteger ToBigInteger() {
        if(wordCount==1 && (data[0]>>31)==0){
          return BigInteger.valueOf((int)data[0]);
        }
        byte[] bytes = new byte[wordCount * 4 + 1];
        for (int i = 0; i < wordCount; i++) {
          bytes[i * 4 + 0] = (byte)((data[i]) & 0xFF);
          bytes[i * 4 + 1] = (byte)((data[i] >> 8) & 0xFF);
          bytes[i * 4 + 2] = (byte)((data[i] >> 16) & 0xFF);
          bytes[i * 4 + 3] = (byte)((data[i] >> 24) & 0xFF);
        }
        bytes[bytes.length - 1] = (byte)0;
        return BigInteger.fromByteArray((byte[])bytes,true);
      }

      int[] GetLastWordsInternal(int numWords32Bit){
        int[] ret=new int[numWords32Bit];
        System.arraycopy(data,0,ret,0,Math.min(numWords32Bit,this.wordCount));
        return ret;
      }

    /**
     *
     * @return A Boolean object.
     */
      public boolean CanFitInInt32() {
        return wordCount==0 || (wordCount==1 && (data[0]>>31)==0);
      }

    /**
     *
     * @return A 32-bit signed integer.
     */
      public int ToInt32() {
        return wordCount==0 ? 0 : data[0];
      }

    /**
     *
     * @return A MutableNumber object.
     */
      public MutableNumber Copy() {
        MutableNumber mbi=new MutableNumber(0);
        if(this.wordCount>mbi.data.length){
          mbi.data=new int[this.wordCount];
        }
        System.arraycopy(this.data,0,mbi.data,0,this.wordCount);
        mbi.wordCount=this.wordCount;
        return mbi;
      }

    /**
     * Multiplies this instance by the value of a 32-bit signed integer.
     * @param multiplicand A 32-bit signed integer.
     * @return The product of the two objects.
     */
      public MutableNumber Multiply(int multiplicand) {
        if (multiplicand < 0)
          throw new IllegalArgumentException("Only positive multiplicands are supported");
        else if (multiplicand != 0) {
          int carry = 0;
          if(this.wordCount==0){
            if(this.data.length==0)this.data=new int[4];
            this.data[0]=0;
            this.wordCount=1;
          }
          int result0,result1,result2,result3;
          if(multiplicand<65536){
            for (int i = 0; i < this.wordCount; i++) {
              int x0=this.data[i];
              int x1=x0;
              int y0=multiplicand;
              x0&=(65535);
              x1=((x1>>16)&65535);
              int temp = (x0 * y0); // a * c
              result1 = (temp >> 16)&65535;   result0 = temp & 65535;
              result2 = 0;
              temp = (x1 * y0); // b * c
              result2 += (temp >> 16)&65535;  result1 += temp & 65535;
              result2 += (result1 >>16)&65535;  result1 = result1 & 65535;
              result3 = (result2 >>16)&65535;  result2 = result2 & 65535;
              // Add carry
              x0=((int)(result0|(result1<<16)));
              x1=((int)(result2|(result3<<16)));
              int x2=(x0+carry);
              if(((x2>>31)==(x0>>31)) ? ((x2&Integer.MAX_VALUE)<(x0&Integer.MAX_VALUE)) :
                 ((x2>>31)==0)){
                // Carry in addition
                x1=(x1+1);
              }
              data[i]=x2;
              carry=x1;
            }
          } else {
            for (int i = 0; i < this.wordCount; i++) {
              int x0=this.data[i];
              int x1=x0;
              int y0=multiplicand;
              int y1=y0;
              x0&=(65535);
              y0&=(65535);
              x1=((x1>>16)&65535);
              y1=((y1>>16)&65535);
              int temp = (x0 * y0); // a * c
              result1 = (temp >> 16)&65535;   result0 = temp & 65535;
              temp = (x0 * y1); // a * d
              result2 = (temp >> 16)&65535;   result1 += temp & 65535;
              result2 += (result1 >>16)&65535;   result1 = result1 & 65535;
              temp = (x1 * y0); // b * c
              result2 += (temp >> 16)&65535;  result1 += temp & 65535;
              result2 += (result1 >>16)&65535;  result1 = result1 & 65535;
              result3 = (result2 >>16)&65535;  result2 = result2 & 65535;
              temp = (x1 * y1); // b * d
              result3 += (temp >> 16)&65535;   result2 += temp & 65535;
              result3 += (result2 >>16)&65535;   result2 = result2 & 65535;
              // Add carry
              x0=((int)(result0|(result1<<16)));
              x1=((int)(result2|(result3<<16)));
              int x2=(x0+carry);
              if(((x2>>31)==(x0>>31)) ? ((x2&Integer.MAX_VALUE)<(x0&Integer.MAX_VALUE)) :
                 ((x2>>31)==0)){
                // Carry in addition
                x1=(x1+1);
              }
              data[i]=x2;
              carry=x1;
            }
          }
          if (carry != 0) {
            if (wordCount >= data.length) {
              int[] newdata = new int[wordCount + 20];
              System.arraycopy(data, 0, newdata, 0, data.length);
              data = newdata;
            }
            data[wordCount] = carry;
            wordCount++;
          }
          // Calculate the correct data length
          while(this.wordCount!=0 && this.data[this.wordCount-1]==0)
            this.wordCount--;
        } else {
          if(data.length>0)data[0] = 0;
          wordCount = 0;
        }
        return this;
      }

    /**
     *
     */
      public int signum() {
          return (wordCount==0 ? 0 : 1);
        }

    /**
     *
     */
      public boolean isEvenNumber() {
          return (wordCount==0 || (data[0]&1)==0);
        }

    /**
     * Compares a 32-bit signed integer with this instance.
     * @param val A 32-bit signed integer.
     * @return A 32-bit signed integer.
     */
      public int CompareToInt(int val) {
        if(val<0 || wordCount>1)return 1;
        if(wordCount==0){
          // this value is 0
          return (val==0) ? 0 : -1;
        } else if(data[0]==val){
          return 0;
        } else {
          return (((data[0]>>31)==(val>>31)) ? ((data[0]&Integer.MAX_VALUE)<(val&Integer.MAX_VALUE))
                  : ((data[0]>>31)==0)) ? -1 : 1;
        }
      }

    /**
     * Subtracts a 32-bit signed integer from this instance.
     * @param other A 32-bit signed integer.
     * @return The difference of the two objects.
     */
      public MutableNumber SubtractInt(
        int other
       ) {
        if (other < 0)
          throw new IllegalArgumentException("Only positive values are supported");
        else if (other != 0)
        {
          {
            // Ensure a length of at least 1
            if(this.wordCount==0){
              if(this.data.length==0)this.data=new int[4];
              this.data[0]=0;
              this.wordCount=1;
            }
            int borrow;
            int u;
            int a=this.data[0];
            u=(a-other);
            borrow=((((a>>31)==(u>>31)) ?
                     ((a&Integer.MAX_VALUE)<(u&Integer.MAX_VALUE)) :
                     ((a>>31)==0)) || (a==u && other!=0)) ? 1 : 0;
            this.data[0] = (int)(u);
            if(borrow!=0){
              for (int i = 1; i < this.wordCount; i++) {
                u=(this.data[i])-borrow;
                borrow=((((this.data[i]>>31)==(u>>31)) ?
                         ((this.data[i]&Integer.MAX_VALUE)<(u&Integer.MAX_VALUE)) :
                         ((this.data[i]>>31)==0))) ? 1 : 0;
                this.data[i] = (int)(u);
              }
            }
            // Calculate the correct data length
            while(this.wordCount!=0 && this.data[this.wordCount-1]==0)
              this.wordCount--;
          }
        }
        return this;
      }

    /**
     * Subtracts a MutableNumber object from this instance.
     * @param other A MutableNumber object.
     * @return The difference of the two objects.
     */
      public MutableNumber Subtract(
        MutableNumber other
       ) {
        {
          {
            // System.out.println("{0} {1}",this.data.length,other.data.length);
            int neededSize=(this.wordCount>other.wordCount) ? this.wordCount : other.wordCount;
            if(data.length<neededSize){
              int[] newdata = new int[neededSize + 20];
              System.arraycopy(data, 0, newdata, 0, data.length);
              data = newdata;
            }
            neededSize=(this.wordCount<other.wordCount) ? this.wordCount : other.wordCount;
            int u=0;
            int borrow=0;
            for (int i = 0; i < neededSize; i++) {
              int a=this.data[i];
              u=(a-other.data[i])-borrow;
              borrow=((((a>>31)==(u>>31)) ? ((a&Integer.MAX_VALUE)<(u&Integer.MAX_VALUE)) :
                       ((a>>31)==0)) || (a==u && other.data[i]!=0)) ? 1 : 0;
              this.data[i] = (int)(u);
            }
            if(borrow!=0){
              for (int i = neededSize; i < this.wordCount; i++) {
                int a=this.data[i];
                u=(a-other.data[i])-borrow;
                borrow=((((a>>31)==(u>>31)) ? ((a&Integer.MAX_VALUE)<(u&Integer.MAX_VALUE)) :
                         ((a>>31)==0)) || (a==u && other.data[i]!=0)) ? 1 : 0;
                this.data[i] = (int)(u);
              }
            }
            // Calculate the correct data length
            while(this.wordCount!=0 && this.data[this.wordCount-1]==0)
              this.wordCount--;
            return this;
          }
        }
      }

    /**
     * Compares a MutableNumber object with this instance.
     * @param other A MutableNumber object.
     * @return Zero if the values are equal; a negative number if this instance
     * is less, or a positive number if this instance is greater.
     */
      public int compareTo(MutableNumber other) {
        if(this.wordCount!=other.wordCount){
          return (this.wordCount<other.wordCount) ? -1 : 1;
        }
        int N=this.wordCount;
        while ((N--) != 0) {
          int an = this.data[N];
          int bn = other.data[N];
          // Unsigned less-than check
          if (((an>>31)==(bn>>31)) ?
              ((an&Integer.MAX_VALUE)<(bn&Integer.MAX_VALUE)) :
              ((an>>31)==0)){
            return -1;
          } else if (an != bn){
            return 1;
          }
        }
        return 0;
      }

    /**
     *
     * @param augend A 32-bit signed integer.
     */
      public MutableNumber Add(int augend) {
        if (augend < 0)
          throw new IllegalArgumentException("Only positive augends are supported");
        else if (augend != 0)
        {
          int carry = 0;
          // Ensure a length of at least 1
          if(this.wordCount==0){
            if(this.data.length==0)this.data=new int[4];
            this.data[0]=0;
            this.wordCount=1;
          }
          for (int i = 0; i < wordCount; i++) {
            int u;
            int a=this.data[i];
            u=(a+augend)+carry;
            carry=((((u>>31)==(a>>31)) ? ((u&Integer.MAX_VALUE)<(a&Integer.MAX_VALUE)) :
                    ((u>>31)==0)) || (u==a && augend!=0)) ? 1 : 0;
            data[i] = u;
            if (carry == 0) return this;
            augend = 0;
          }
          if (carry != 0) {
            if (wordCount >= data.length) {
              int[] newdata = new int[wordCount + 20];
              System.arraycopy(data, 0, newdata, 0, data.length);
              data = newdata;
            }
            data[wordCount] = carry;
            wordCount++;
          }
        }
        // Calculate the correct data length
        while(this.wordCount!=0 && this.data[this.wordCount-1]==0)
          this.wordCount--;
        return this;
      }
    }

    int smallValue; // if integerMode is 0
    MutableNumber mnum; // if integerMode is 1
    BigInteger largeValue; // if integerMode is 2
    int integerMode = 0;

    private static BigInteger Int32MinValue = BigInteger.valueOf(Integer.MIN_VALUE);
    private static BigInteger Int32MaxValue = BigInteger.valueOf(Integer.MAX_VALUE);
    private static BigInteger NegativeInt32MinValue=(Int32MinValue).negate();

    public FastInteger (int value) {
      smallValue = value;
    }

    public static FastInteger Copy(FastInteger value) {
      FastInteger fi=new FastInteger(value.smallValue);
      fi.integerMode = value.integerMode;
      fi.largeValue = value.largeValue;
      fi.mnum=(value.mnum==null || value.integerMode!=1) ? null : value.mnum.Copy();
      return fi;
    }

    public static FastInteger FromBig(BigInteger bigintVal) {
      int sign = bigintVal.signum();
      if (sign == 0 ||
          (sign < 0 && bigintVal.compareTo(Int32MinValue) >= 0) ||
          (sign > 0 && bigintVal.compareTo(Int32MaxValue) <= 0)) {
        return new FastInteger(bigintVal.intValue());
      } else if(sign>0){
        FastInteger fi=new FastInteger(0);
        fi.integerMode=1;
        fi.mnum=MutableNumber.FromBigInteger(bigintVal);
        return fi;
      } else {
        FastInteger fi=new FastInteger(0);
        fi.integerMode=2;
        fi.largeValue = bigintVal;
        return fi;
      }
    }

    /**
     *
     * @return A 32-bit signed integer.
     */
    public int AsInt32() {
      switch(this.integerMode){
        case 0:
          return smallValue;
        case 1:
          return mnum.ToInt32();
        case 2:
          return largeValue.intValue();
        default:
          throw new IllegalStateException();
      }
    }

    /**
     * Compares a FastInteger object with this instance.
     * @param val A FastInteger object.
     * @return Zero if the values are equal; a negative number if this instance
     * is less, or a positive number if this instance is greater.
     */
    public int compareTo(FastInteger val) {
      switch((this.integerMode<<2)|val.integerMode){
          case ((0<<2)|0):{
            int vsv=val.smallValue;
            return (smallValue == vsv) ? 0 :
              (smallValue < vsv ? -1 : 1);
          }
        case ((0<<2)|1):
          return -(val.mnum.CompareToInt(smallValue));
        case ((0<<2)|2):
          return AsBigInteger().compareTo(val.largeValue);
        case ((1<<2)|0):
          return mnum.CompareToInt(val.smallValue);
        case ((1<<2)|1):
          return mnum.compareTo(val.mnum);
        case ((1<<2)|2):
          return AsBigInteger().compareTo(val.largeValue);
        case ((2<<2)|0):
        case ((2<<2)|1):
        case ((2<<2)|2):
          return largeValue.compareTo(val.AsBigInteger());
        default:
          throw new IllegalStateException();
      }
    }
    /**
     *
     * @return A FastInteger object.
     */
    public FastInteger Abs() {
      return (this.signum() < 0) ? Negate() : this;
    }

    public static BigInteger WordsToBigInteger(int[] words) {
      int wordCount=words.length;
      if(wordCount==1 && (words[0]>>31)==0){
        return BigInteger.valueOf((int)words[0]);
      }
      byte[] bytes = new byte[wordCount * 4 + 1];
      for (int i = 0; i < wordCount; i++) {
        bytes[i * 4 + 0] = (byte)((words[i]) & 0xFF);
        bytes[i * 4 + 1] = (byte)((words[i] >> 8) & 0xFF);
        bytes[i * 4 + 2] = (byte)((words[i] >> 16) & 0xFF);
        bytes[i * 4 + 3] = (byte)((words[i] >> 24) & 0xFF);
      }
      bytes[bytes.length - 1] = (byte)0;
      return BigInteger.fromByteArray((byte[])bytes,true);
    }
    public static int[] GetLastWords(BigInteger bigint, int numWords32Bit) {
      return MutableNumber.FromBigInteger(bigint).GetLastWordsInternal(numWords32Bit);
    }

    /**
     *
     * @param val A 32-bit signed integer.
     * @return A FastInteger object.
     */
    public FastInteger SetInt(int val) {
      smallValue=val;
      integerMode=0;
      return this;
    }

    /**
     *
     * @param digit A 32-bit signed integer.
     * @return A FastInteger object.
     */
    public FastInteger MultiplyByTenAndAdd(int digit) {
      if(digit==0){
        if(integerMode==1){
          mnum.Multiply(10);
          return this;
        } else if(integerMode==0 && smallValue>=214748363){
          integerMode=1;
          mnum=new MutableNumber(smallValue);
          mnum.Multiply(10);
          return this;
        }
      } else if(digit>0){
        if(integerMode==1){
          mnum.Multiply(10).Add(digit);
          return this;
        } else if(integerMode==0 && smallValue>=214748363){
          integerMode=1;
          mnum=new MutableNumber(smallValue);
          mnum.Multiply(10).Add(digit);
          return this;
        }
      }
      return this.Multiply(10).AddInt(digit);
    }

    /**
     *
     * @param divisor A FastInteger object.
     * @return A 32-bit signed integer.
     */
    public int RepeatedSubtract(FastInteger divisor) {
      if(integerMode==1){
        int count=0;
        if(divisor.integerMode==1){
          while(mnum.compareTo(divisor.mnum)>=0){
            mnum.Subtract(divisor.mnum);
            count++;
          }
          return count;
        } else if(divisor.integerMode==0 && divisor.smallValue>=0){
          if(mnum.CanFitInInt32()){
            int small=mnum.ToInt32();
            count=small/divisor.smallValue;
            mnum.SetInt(small%divisor.smallValue);
          } else {
            MutableNumber dmnum=new MutableNumber(divisor.smallValue);
            while(mnum.compareTo(dmnum)>=0){
              mnum.Subtract(dmnum);
              count++;
            }
          }
          return count;
        } else {
          BigInteger bigrem;
          BigInteger bigquo;
{
BigInteger[] divrem=(this.AsBigInteger()).divideAndRemainder(divisor.AsBigInteger());
bigquo=divrem[0];
bigrem=divrem[1]; }
          int smallquo=bigquo.intValue();
          integerMode=2;
          largeValue=bigrem;
          return smallquo;
        }
      } else {
        BigInteger bigrem;
        BigInteger bigquo;
{
BigInteger[] divrem=(this.AsBigInteger()).divideAndRemainder(divisor.AsBigInteger());
bigquo=divrem[0];
bigrem=divrem[1]; }
        int smallquo=bigquo.intValue();
        integerMode=2;
        largeValue=bigrem;
        return smallquo;
      }
    }

    /**
     * Sets this object's value to the current value times another integer.
     * @param val The integer to multiply by.
     * @return This object.
     */
    public FastInteger Multiply(int val) {
      if (val == 0) {
        smallValue = 0;
        integerMode=0;
      } else {
        switch (integerMode) {
          case 0:
            boolean apos = (smallValue > 0L);
            boolean bpos = (val > 0L);
            if (
              (apos && ((!bpos && (Integer.MIN_VALUE / smallValue) > val) ||
                        (bpos && smallValue > (Integer.MAX_VALUE / val)))) ||
              (!apos && ((!bpos && smallValue != 0L &&
                          (Integer.MAX_VALUE / smallValue) > val) ||
                         (bpos && smallValue < (Integer.MIN_VALUE / val))))) {
              // would overflow, convert to large
              if(apos && bpos){
                // if both operands are nonnegative
                // convert to mutable big integer
                integerMode=1;
                mnum=new MutableNumber(smallValue);
                mnum.Multiply(val);
              } else {
                // if either operand is negative
                // convert to big integer
                integerMode=2;
                largeValue = BigInteger.valueOf(smallValue);
                largeValue=largeValue.multiply(BigInteger.valueOf(val));
              }
            } else {
              smallValue *= val;
            }
            break;
          case 1:
            if(val<0){
              integerMode=2;
              largeValue=mnum.ToBigInteger();
              largeValue=largeValue.multiply(BigInteger.valueOf(val));
            } else {
              mnum.Multiply(val);
            }
            break;
          case 2:
            largeValue=largeValue.multiply(BigInteger.valueOf(val));
            break;
          default:
            throw new IllegalStateException();
        }
      }
      return this;
    }

    /**
     * Sets this object's value to 0 minus its current value (reverses its
     * sign).
     * @return This object.
     */
    public FastInteger Negate() {
      switch (integerMode) {
        case 0:
          if (smallValue == Integer.MIN_VALUE) {
            // would overflow, convert to large
            integerMode=1;
            mnum = MutableNumber.FromBigInteger(NegativeInt32MinValue);
          } else {
            smallValue = -smallValue;
          }
          break;
        case 1:
          integerMode=2;
          largeValue=mnum.ToBigInteger();
          largeValue=(largeValue).negate();
          break;
        case 2:
          largeValue=(largeValue).negate();
          break;
        default:
          throw new IllegalStateException();
      }
      return this;
    }

    /**
     * Sets this object's value to the current value minus the given FastInteger
     * value.
     * @param val The subtrahend.
     * @return This object.
     */
    public FastInteger Subtract(FastInteger val) {
      BigInteger valValue;
      switch (integerMode) {
        case 0:
          if(val.integerMode==0){
            int vsv=val.smallValue;
            if ((vsv < 0 && Integer.MAX_VALUE + vsv < smallValue) ||
                (vsv > 0 && Integer.MIN_VALUE + vsv > smallValue)) {
              // would overflow, convert to large
              integerMode=2;
              largeValue = BigInteger.valueOf(smallValue);
              largeValue=largeValue.subtract(BigInteger.valueOf(vsv));
            } else {
              smallValue-=vsv;
            }
          } else {
            integerMode=2;
            largeValue=BigInteger.valueOf(smallValue);
            valValue = val.AsBigInteger();
            largeValue=largeValue.subtract(valValue);
          }
          break;
        case 1:
          if(val.integerMode==1){
            // NOTE: Mutable numbers are
            // currently always zero or positive
            mnum.Subtract(val.mnum);
          } else if(val.integerMode==0 && val.smallValue>=0){
            mnum.SubtractInt(val.smallValue);
          } else {
            integerMode=2;
            largeValue=mnum.ToBigInteger();
            valValue = val.AsBigInteger();
            largeValue=largeValue.subtract(valValue);
          }
          break;
        case 2:
          valValue = val.AsBigInteger();
          largeValue=largeValue.subtract(valValue);
          break;
        default:
          throw new IllegalStateException();
      }
      return this;
    }
    /**
     * Sets this object's value to the current value minus the given integer.
     * @param val The subtrahend.
     * @return This object.
     */
    public FastInteger SubtractInt(int val) {
      if(val==Integer.MIN_VALUE){
        return AddBig(NegativeInt32MinValue);
      } else if(integerMode==0){
        if ((val < 0 && Integer.MAX_VALUE + val < smallValue) ||
            (val > 0 && Integer.MIN_VALUE + val > smallValue)) {
          // would overflow, convert to large
          integerMode=2;
          largeValue = BigInteger.valueOf(smallValue);
          largeValue=largeValue.subtract(BigInteger.valueOf(val));
        } else {
          smallValue-=val;
        }
        return this;
      } else {
        return AddInt(-val);
      }
    }

    /**
     * Sets this object's value to the current value plus the given integer.
     * @param bigintVal The number to add.
     * @return This object.
     */
    public FastInteger AddBig(BigInteger bigintVal) {
      switch (integerMode) {
          case 0:{
            int sign = bigintVal.signum();
            if (sign == 0 ||
                (sign < 0 && bigintVal.compareTo(Int32MinValue) >= 0) ||
                (sign > 0 && bigintVal.compareTo(Int32MaxValue) <= 0)) {
              return AddInt(bigintVal.intValue());
            }
            return Add(FastInteger.FromBig(bigintVal));
          }
        case 1:
          integerMode=2;
          largeValue=mnum.ToBigInteger();
          largeValue=largeValue.add(bigintVal);
          break;
        case 2:
          largeValue=largeValue.add(bigintVal);
          break;
        default:
          throw new IllegalStateException();
      }
      return this;
    }

    /**
     * Sets this object's value to the current value minus the given integer.
     * @param bigintVal The subtrahend.
     * @return This object.
     */
    public FastInteger SubtractBig(BigInteger bigintVal) {
      if (integerMode==2) {
        largeValue=largeValue.subtract(bigintVal);
        return this;
      } else {
        int sign = bigintVal.signum();
        if (sign == 0)return this;
        // Check if this value fits an int, except if
        // it's MinValue
        if(sign < 0 && bigintVal.compareTo(Int32MinValue) > 0){
          return AddInt(-(bigintVal.intValue()));
        }
        if(sign > 0 && bigintVal.compareTo(Int32MaxValue) <= 0){
          return SubtractInt(bigintVal.intValue());
        }
        bigintVal=bigintVal.negate();
        return AddBig(bigintVal);
      }
    }
    /**
     *
     * @param val A FastInteger object.
     * @return A FastInteger object.
     */
    public FastInteger Add(FastInteger val) {
      BigInteger valValue;
      switch (integerMode) {
        case 0:
          if(val.integerMode==0){
            if ((smallValue < 0 && (int)val.smallValue < Integer.MIN_VALUE - smallValue) ||
                (smallValue > 0 && (int)val.smallValue > Integer.MAX_VALUE - smallValue)) {
              // would overflow
              if(val.smallValue>=0){
                integerMode=1;
                mnum=new MutableNumber(this.smallValue);
                mnum.Add(val.smallValue);
              } else {
                integerMode=2;
                largeValue = BigInteger.valueOf(smallValue);
                largeValue=largeValue.add(BigInteger.valueOf(val.smallValue));
              }
            } else {
              smallValue+=val.smallValue;
            }
          } else {
            integerMode=2;
            largeValue=BigInteger.valueOf(smallValue);
            valValue = val.AsBigInteger();
            largeValue=largeValue.add(valValue);
          }
          break;
        case 1:
          if(val.integerMode==0 && val.smallValue>=0){
            mnum.Add(val.smallValue);
          } else {
            integerMode=2;
            largeValue=mnum.ToBigInteger();
            valValue = val.AsBigInteger();
            largeValue=largeValue.add(valValue);
          }
          break;
        case 2:
          valValue = val.AsBigInteger();
          largeValue=largeValue.add(valValue);
          break;
        default:
          throw new IllegalStateException();
      }
      return this;
    }
    /**
     * Sets this object's value to the remainder of the current value divided
     * by the given integer.
     * @param divisor The divisor.
     * @return This object.
     */
    public FastInteger Mod(int divisor) {
      // Mod operator will always result in a
      // number that fits an int for int divisors
      if (divisor != 0) {
        switch (integerMode) {
          case 0:
            smallValue %= divisor;
            break;
          case 1:
            largeValue=mnum.ToBigInteger();
            largeValue = largeValue.remainder(BigInteger.valueOf(divisor));
            smallValue = largeValue.intValue();
            integerMode=0;
            break;
          case 2:
            largeValue = largeValue.remainder(BigInteger.valueOf(divisor));
            smallValue = largeValue.intValue();
            integerMode=0;
            break;
          default:
            throw new IllegalStateException();
        }
      } else {
        throw new ArithmeticException();
      }
      return this;
    }

    /**
     *
     * @return A FastInteger object.
     */
    public FastInteger Increment() {
      if(integerMode==0){
        if(smallValue!=Integer.MAX_VALUE){
          smallValue++;
        } else {
          integerMode=1;
          mnum = MutableNumber.FromBigInteger(NegativeInt32MinValue);
        }
        return this;
      } else {
        return AddInt(1);
      }
    }

    /**
     *
     * @return A FastInteger object.
     */
    public FastInteger Decrement() {
      if(integerMode==0){
        if(smallValue!=Integer.MIN_VALUE){
          smallValue--;
        } else {
          integerMode=1;
          mnum = MutableNumber.FromBigInteger(Int32MinValue);
          mnum.SubtractInt(1);
        }
        return this;
      } else {
        return SubtractInt(1);
      }
    }

    /**
     * Divides this instance by the value of a 32-bit signed integer.
     * @param divisor A 32-bit signed integer.
     * @return The quotient of the two objects.
     */
    public FastInteger Divide(int divisor) {
      if (divisor != 0) {
        switch (integerMode) {
          case 0:
            if (divisor == -1 && smallValue == Integer.MIN_VALUE) {
              // would overflow, convert to large
              integerMode=1;
              mnum = MutableNumber.FromBigInteger(NegativeInt32MinValue);
            } else {
              smallValue /= divisor;
            }
            break;
          case 1:
            integerMode=2;
            largeValue=mnum.ToBigInteger();
            largeValue=largeValue.divide(BigInteger.valueOf(divisor));
            if(largeValue.signum()==0){
              integerMode=0;
              smallValue=0;
            }
            break;
          case 2:
            largeValue=largeValue.divide(BigInteger.valueOf(divisor));
            if(largeValue.signum()==0){
              integerMode=0;
              smallValue=0;
            }
            break;
          default:
            throw new IllegalStateException();
        }
      } else {
        throw new ArithmeticException();
      }
      return this;
    }

    /**
     *
     */
    public boolean isEvenNumber() {
        switch (integerMode) {
          case 0:
            return (smallValue&1)==0;
          case 1:
            return mnum.isEvenNumber();
          case 2:
            return largeValue.testBit(0)==false;
          default:
            throw new IllegalStateException();
        }
      }

    /**
     *
     * @param val A 32-bit signed integer.
     * @return A FastInteger object.
     */
    public FastInteger AddInt(int val) {
      BigInteger valValue;
      switch (integerMode) {
        case 0:
          if ((smallValue < 0 && (int)val < Integer.MIN_VALUE - smallValue) ||
              (smallValue > 0 && (int)val > Integer.MAX_VALUE - smallValue)) {
            // would overflow
            if(val>=0){
              integerMode=1;
              mnum=new MutableNumber(this.smallValue);
              mnum.Add(val);
            } else {
              integerMode=2;
              largeValue = BigInteger.valueOf(smallValue);
              largeValue=largeValue.add(BigInteger.valueOf(val));
            }
          } else {
            smallValue+=val;
          }
          break;
        case 1:
          if(val>=0){
            mnum.Add(val);
          } else {
            integerMode=2;
            largeValue=mnum.ToBigInteger();
            valValue = BigInteger.valueOf(val);
            largeValue=largeValue.add(valValue);
          }
          break;
        case 2:
          valValue = BigInteger.valueOf(val);
          largeValue=largeValue.add(valValue);
          break;
        default:
          throw new IllegalStateException();
      }
      return this;
    }

    /**
     *
     * @return A Boolean object.
     */
    public boolean CanFitInInt32() {
      int sign;
      switch(this.integerMode){
        case 0:
          return true;
        case 1:
          return mnum.CanFitInInt32();
          case 2:{
            sign = largeValue.signum();
            if (sign == 0) return true;
            if (sign < 0) return largeValue.compareTo(Int32MinValue) >= 0;
            return largeValue.compareTo(Int32MaxValue) <= 0;
          }
        default:
          throw new IllegalStateException();
      }
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      switch(this.integerMode){
        case 0:
          return Integer.toString((int)smallValue);
        case 1:
          return mnum.ToBigInteger().toString();
        case 2:
          return largeValue.toString();
        default:
          return "";
      }
    }
    /**
     *
     */
    public int signum() {
        switch(this.integerMode){
          case 0:
            return ((this.smallValue==0) ? 0 : ((this.smallValue<0) ? -1 : 1));
          case 1:
            return mnum.signum();
          case 2:
            return largeValue.signum();
          default:
            return 0;
        }
      }

    /**
     * Compares a 32-bit signed integer with this instance.
     * @param val A 32-bit signed integer.
     * @return Zero if the values are equal; a negative number if this instance
     * is less, or a positive number if this instance is greater.
     */
    public int CompareToInt(int val) {
      switch(this.integerMode){
        case 0:
          return (val == smallValue) ? 0 : (smallValue < val ? -1 : 1);
        case 1:
          return mnum.ToBigInteger().compareTo(BigInteger.valueOf(val));
        case 2:
          return largeValue.compareTo(BigInteger.valueOf(val));
        default:
          return 0;
      }
    }

    /**
     *
     * @param val A 32-bit signed integer.
     * @return A 32-bit signed integer.
     */
    public int MinInt32(int val) {
      return this.CompareToInt(val)<0 ? this.AsInt32() : val;
    }

    /**
     *
     * @return A BigInteger object.
     */
    public BigInteger AsBigInteger() {
      switch(this.integerMode){
        case 0:
          return BigInteger.valueOf(smallValue);
        case 1:
          return mnum.ToBigInteger();
        case 2:
          return largeValue;
        default:
          throw new IllegalStateException();
      }
    }
  }

