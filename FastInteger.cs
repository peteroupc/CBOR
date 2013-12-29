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
    /// <summary> A mutable integer class initially backed by a small integer,
    /// that only uses a big integer when arithmetic operations would overflow
    /// the small integer. <para> This class is ideal for cases where operations
    /// should be arbitrary precision, but the need to use a high precision
    /// is rare.</para>
    /// <para> Many methods in this class return a reference to the same object
    /// as used in the call. This allows chaining operations in a single line
    /// of code. For example:</para>
    /// <code> fastInt.Add(5).Multiply(10);</code>
    /// </summary>
  sealed class FastInteger : IComparable<FastInteger> {

    private sealed class MutableNumber {
      int[] data;
      int wordCount;
      public static MutableNumber FromBigInteger(BigInteger bigintVal) {
        MutableNumber mnum=new MutableNumber(0);
        if ( bigintVal.Sign < 0)
          throw new ArgumentException("Only positive integers are supported");
        byte[] bytes=bigintVal.ToByteArray();
        int len=bytes.Length;
        int newWordCount=Math.Max(4,(len/4)+1);
        if(newWordCount>mnum.data.Length){
          mnum.data=new int[newWordCount];
        }
        mnum.wordCount=newWordCount;
        unchecked {
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
      public MutableNumber(int val) {
        if (val < 0)
          throw new ArgumentException("Only positive integers are supported");
        data = new int[4];
        wordCount = (val==0) ? 0 : 1;
        data[0] = unchecked((int)((val) & 0xFFFFFFFFL));
      }

    /// <summary> </summary>
    /// <param name='val'>A 32-bit signed integer.</param>
    /// <returns>A MutableNumber object.</returns>
public MutableNumber SetInt(int val){
        if(val<0)
          throw new ArgumentException("Only positive integers are supported");
        wordCount = (val==0) ? 0 : 1;
        data[0] = unchecked((int)((val) & 0xFFFFFFFFL));
        return this;
      }

    /// <summary> </summary>
    /// <returns>A BigInteger object.</returns>
      public BigInteger ToBigInteger() {
        if(wordCount==1 && (data[0]>>31)==0){
          return (BigInteger)((int)data[0]);
        }
        byte[] bytes = new byte[wordCount * 4 + 1];
        for (int i = 0; i < wordCount; i++) {
          bytes[i * 4 + 0] = (byte)((data[i]) & 0xFF);
          bytes[i * 4 + 1] = (byte)((data[i] >> 8) & 0xFF);
          bytes[i * 4 + 2] = (byte)((data[i] >> 16) & 0xFF);
          bytes[i * 4 + 3] = (byte)((data[i] >> 24) & 0xFF);
        }
        bytes[bytes.Length - 1] = (byte)0;
        return new BigInteger((byte[])bytes);
      }

      internal int[] GetLastWordsInternal(int numWords32Bit){
        int[] ret=new int[numWords32Bit];
        Array.Copy(data,ret,Math.Min(numWords32Bit,this.wordCount));
        return ret;
      }

    /// <summary> </summary>
    /// <returns>A Boolean object.</returns>
      public bool CanFitInInt32(){
        return wordCount==0 || (wordCount==1 && (data[0]>>31)==0);
      }

    /// <summary> </summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int ToInt32(){
        return wordCount==0 ? 0 : data[0];
      }

    /// <summary> </summary>
    /// <returns>A MutableNumber object.</returns>
      public MutableNumber Copy(){
        MutableNumber mbi=new MutableNumber(0);
        if(this.wordCount>mbi.data.Length){
          mbi.data=new int[this.wordCount];
        }
        Array.Copy(this.data,mbi.data,this.wordCount);
        mbi.wordCount=this.wordCount;
        return mbi;
      }

    /// <summary> Multiplies this instance by the value of a 32-bit signed
    /// integer.</summary>
    /// <param name='multiplicand'>A 32-bit signed integer.</param>
    /// <returns>The product of the two objects.</returns>
      public MutableNumber Multiply(int multiplicand) {
        if (multiplicand < 0)
          throw new ArgumentException("Only positive multiplicands are supported");
        else if (multiplicand != 0) {
          int carry = 0;
          if(this.wordCount==0){
            if(this.data.Length==0)this.data=new int[4];
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
              int temp = unchecked(x0 * y0); // a * c
              result1 = (temp >> 16)&65535;   result0 = temp & 65535;
              result2 = 0;
              temp = unchecked(x1 * y0); // b * c
              result2 += (temp >> 16)&65535;  result1 += temp & 65535;
              result2 += (result1 >>16)&65535;  result1 = result1 & 65535;
              result3 = (result2 >>16)&65535;  result2 = result2 & 65535;
              // Add carry
              x0=unchecked((int)(result0|(result1<<16)));
              x1=unchecked((int)(result2|(result3<<16)));
              int x2=unchecked(x0+carry);
              if(((x2>>31)==(x0>>31)) ? ((x2&Int32.MaxValue)<(x0&Int32.MaxValue)) :
                 ((x2>>31)==0)){
                // Carry in addition
                x1=unchecked(x1+1);
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
              int temp = unchecked(x0 * y0); // a * c
              result1 = (temp >> 16)&65535;   result0 = temp & 65535;
              temp = unchecked(x0 * y1); // a * d
              result2 = (temp >> 16)&65535;   result1 += temp & 65535;
              result2 += (result1 >>16)&65535;   result1 = result1 & 65535;
              temp = unchecked(x1 * y0); // b * c
              result2 += (temp >> 16)&65535;  result1 += temp & 65535;
              result2 += (result1 >>16)&65535;  result1 = result1 & 65535;
              result3 = (result2 >>16)&65535;  result2 = result2 & 65535;
              temp = unchecked(x1 * y1); // b * d
              result3 += (temp >> 16)&65535;   result2 += temp & 65535;
              result3 += (result2 >>16)&65535;   result2 = result2 & 65535;
              // Add carry
              x0=unchecked((int)(result0|(result1<<16)));
              x1=unchecked((int)(result2|(result3<<16)));
              int x2=unchecked(x0+carry);
              if(((x2>>31)==(x0>>31)) ? ((x2&Int32.MaxValue)<(x0&Int32.MaxValue)) :
                 ((x2>>31)==0)){
                // Carry in addition
                x1=unchecked(x1+1);
              }
              data[i]=x2;
              carry=x1;
            }
          }
          if (carry != 0) {
            if (wordCount >= data.Length) {
              int[] newdata = new int[wordCount + 20];
              Array.Copy(data, 0, newdata, 0, data.Length);
              data = newdata;
            }
            data[wordCount] = carry;
            wordCount++;
          }
          // Calculate the correct data length
          while(this.wordCount!=0 && this.data[this.wordCount-1]==0)
            this.wordCount--;
        } else {
          if(data.Length>0)data[0] = 0;
          wordCount = 0;
        }
        return this;
      }

    /// <summary> </summary>
      public int Sign{
        get {
          return (wordCount==0 ? 0 : 1);
        }
      }

    /// <summary> </summary>
      public bool IsEvenNumber{
        get {
          return (wordCount==0 || (data[0]&1)==0);
        }
      }

    /// <summary>Compares a 32-bit signed integer with this instance.</summary>
    /// <param name='val'>A 32-bit signed integer.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public int CompareToInt(int val){
        if(val<0 || wordCount>1)return 1;
        if(wordCount==0){
          // this value is 0
          return (val==0) ? 0 : -1;
        } else if(data[0]==val){
          return 0;
        } else {
          return (((data[0]>>31)==(val>>31)) ? ((data[0]&Int32.MaxValue)<(val&Int32.MaxValue))
                  : ((data[0]>>31)==0)) ? -1 : 1;
        }
      }

    /// <summary>Subtracts a 32-bit signed integer from this instance.</summary>
    /// <param name='other'>A 32-bit signed integer.</param>
    /// <returns>The difference of the two objects.</returns>
      public MutableNumber SubtractInt(
        int other
      ) {
        if (other < 0)
          throw new ArgumentException("Only positive values are supported");
        else if (other != 0)
        {
          unchecked {
            // Ensure a length of at least 1
            if(this.wordCount==0){
              if(this.data.Length==0)this.data=new int[4];
              this.data[0]=0;
              this.wordCount=1;
            }
            int borrow;
            int u;
            int a=this.data[0];
            u=(a-other);
            borrow=((((a>>31)==(u>>31)) ?
                     ((a&Int32.MaxValue)<(u&Int32.MaxValue)) :
                     ((a>>31)==0)) || (a==u && other!=0)) ? 1 : 0;
            this.data[0] = (int)(u);
            if(borrow!=0){
              for (int i = 1; i < this.wordCount; i++) {
                u=(this.data[i])-borrow;
                borrow=((((this.data[i]>>31)==(u>>31)) ?
                         ((this.data[i]&Int32.MaxValue)<(u&Int32.MaxValue)) :
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

    /// <summary>Subtracts a MutableNumber object from this instance.</summary>
    /// <param name='other'>A MutableNumber object.</param>
    /// <returns>The difference of the two objects.</returns>
      public MutableNumber Subtract(
        MutableNumber other
      ) {
        unchecked {
          {
            // Console.WriteLine("{0} {1}",this.data.Length,other.data.Length);
            int neededSize=(this.wordCount>other.wordCount) ? this.wordCount : other.wordCount;
            if(data.Length<neededSize){
              int[] newdata = new int[neededSize + 20];
              Array.Copy(data, 0, newdata, 0, data.Length);
              data = newdata;
            }
            neededSize=(this.wordCount<other.wordCount) ? this.wordCount : other.wordCount;
            int u=0;
            int borrow=0;
            for (int i = 0; i < neededSize; i++) {
              int a=this.data[i];
              u=(a-other.data[i])-borrow;
              borrow=((((a>>31)==(u>>31)) ? ((a&Int32.MaxValue)<(u&Int32.MaxValue)) :
                       ((a>>31)==0)) || (a==u && other.data[i]!=0)) ? 1 : 0;
              this.data[i] = (int)(u);
            }
            if(borrow!=0){
              for (int i = neededSize; i < this.wordCount; i++) {
                int a=this.data[i];
                u=(a-other.data[i])-borrow;
                borrow=((((a>>31)==(u>>31)) ? ((a&Int32.MaxValue)<(u&Int32.MaxValue)) :
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

    /// <summary>Compares a MutableNumber object with this instance.</summary>
    /// <param name='other'>A MutableNumber object.</param>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
      public int CompareTo(MutableNumber other){
        if(this.wordCount!=other.wordCount){
          return (this.wordCount<other.wordCount) ? -1 : 1;
        }
        int N=this.wordCount;
        while (unchecked(N--) != 0) {
          int an = this.data[N];
          int bn = other.data[N];
          // Unsigned less-than check
          if (((an>>31)==(bn>>31)) ?
              ((an&Int32.MaxValue)<(bn&Int32.MaxValue)) :
              ((an>>31)==0)){
            return -1;
          } else if (an != bn){
            return 1;
          }
        }
        return 0;
      }

    /// <summary> </summary>
    /// <param name='augend'> A 32-bit signed integer.</param>
    /// <returns></returns>
      public MutableNumber Add(int augend) {
        if (augend < 0)
          throw new ArgumentException("Only positive augends are supported");
        else if (augend != 0)
        {
          int carry = 0;
          // Ensure a length of at least 1
          if(this.wordCount==0){
            if(this.data.Length==0)this.data=new int[4];
            this.data[0]=0;
            this.wordCount=1;
          }
          for (int i = 0; i < wordCount; i++) {
            int u;
            int a=this.data[i];
            u=(a+augend)+carry;
            carry=((((u>>31)==(a>>31)) ? ((u&Int32.MaxValue)<(a&Int32.MaxValue)) :
                    ((u>>31)==0)) || (u==a && augend!=0)) ? 1 : 0;
            data[i] = u;
            if (carry == 0) return this;
            augend = 0;
          }
          if (carry != 0) {
            if (wordCount >= data.Length) {
              int[] newdata = new int[wordCount + 20];
              Array.Copy(data, 0, newdata, 0, data.Length);
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

    private static BigInteger Int32MinValue = (BigInteger)Int32.MinValue;
    private static BigInteger Int32MaxValue = (BigInteger)Int32.MaxValue;
    private static BigInteger NegativeInt32MinValue = -(BigInteger)Int32MinValue;

    public FastInteger(int value) {
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
      int sign = bigintVal.Sign;
      if (sign == 0 ||
          (sign < 0 && bigintVal.CompareTo(Int32MinValue) >= 0) ||
          (sign > 0 && bigintVal.CompareTo(Int32MaxValue) <= 0)) {
        return new FastInteger((int)bigintVal);
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

    /// <summary> </summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int AsInt32() {
      switch(this.integerMode){
        case 0:
          return smallValue;
        case 1:
          return mnum.ToInt32();
        case 2:
          return (int)largeValue;
        default:
          throw new InvalidOperationException();
      }
    }

    /// <summary>Compares a FastInteger object with this instance.</summary>
    /// <param name='val'>A FastInteger object.</param>
    /// <returns>Zero if the values are equal; a negative number is this instance
    /// is less, or a positive number if this instance is greater.</returns>
    public int CompareTo(FastInteger val) {
      switch((this.integerMode<<2)|val.integerMode){
          case ((0<<2)|0):{
            int vsv=val.smallValue;
            return (smallValue == vsv) ? 0 :
              (smallValue < vsv ? -1 : 1);
          }
        case ((0<<2)|1):
          return -(val.mnum.CompareToInt(smallValue));
        case ((0<<2)|2):
          return AsBigInteger().CompareTo(val.largeValue);
        case ((1<<2)|0):
          return mnum.CompareToInt(val.smallValue);
        case ((1<<2)|1):
          return mnum.CompareTo(val.mnum);
        case ((1<<2)|2):
          return AsBigInteger().CompareTo(val.largeValue);
        case ((2<<2)|0):
        case ((2<<2)|1):
        case ((2<<2)|2):
          return largeValue.CompareTo(val.AsBigInteger());
        default:
          throw new InvalidOperationException();
      }
    }
    /// <summary> </summary>
    /// <returns>A FastInteger object.</returns>
    public FastInteger Abs() {
      return (this.Sign < 0) ? Negate() : this;
    }

    public static BigInteger WordsToBigInteger(int[] words){
      int wordCount=words.Length;
      if(wordCount==1 && (words[0]>>31)==0){
        return (BigInteger)((int)words[0]);
      }
      byte[] bytes = new byte[wordCount * 4 + 1];
      for (int i = 0; i < wordCount; i++) {
        bytes[i * 4 + 0] = (byte)((words[i]) & 0xFF);
        bytes[i * 4 + 1] = (byte)((words[i] >> 8) & 0xFF);
        bytes[i * 4 + 2] = (byte)((words[i] >> 16) & 0xFF);
        bytes[i * 4 + 3] = (byte)((words[i] >> 24) & 0xFF);
      }
      bytes[bytes.Length - 1] = (byte)0;
      return new BigInteger((byte[])bytes);
    }
    public static int[] GetLastWords(BigInteger bigint, int numWords32Bit){
      return MutableNumber.FromBigInteger(bigint).GetLastWordsInternal(numWords32Bit);
    }

    /// <summary> </summary>
    /// <param name='val'>A 32-bit signed integer.</param>
    /// <returns>A FastInteger object.</returns>
public FastInteger SetInt(int val){
      smallValue=val;
      integerMode=0;
      return this;
    }

    /// <summary> </summary>
    /// <param name='digit'>A 32-bit signed integer.</param>
    /// <returns>A FastInteger object.</returns>
public FastInteger MultiplyByTenAndAdd(int digit){
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

    /// <summary> </summary>
    /// <param name='divisor'>A FastInteger object.</param>
    /// <returns>A 32-bit signed integer.</returns>
public int RepeatedSubtract(FastInteger divisor){
      if(integerMode==1){
        int count=0;
        if(divisor.integerMode==1){
          while(mnum.CompareTo(divisor.mnum)>=0){
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
            while(mnum.CompareTo(dmnum)>=0){
              mnum.Subtract(dmnum);
              count++;
            }
          }
          return count;
        } else {
          BigInteger bigrem;
          BigInteger bigquo=BigInteger.DivRem(this.AsBigInteger(),divisor.AsBigInteger(),out bigrem);
          int smallquo=(int)bigquo;
          integerMode=2;
          largeValue=bigrem;
          return smallquo;
        }
      } else {
        BigInteger bigrem;
        BigInteger bigquo=BigInteger.DivRem(this.AsBigInteger(),divisor.AsBigInteger(),out bigrem);
        int smallquo=(int)bigquo;
        integerMode=2;
        largeValue=bigrem;
        return smallquo;
      }
    }

    /// <summary> Sets this object's value to the current value times another
    /// integer. </summary>
    /// <param name='val'>The integer to multiply by.</param>
    /// <returns>This object.</returns>
    public FastInteger Multiply(int val) {
      if (val == 0) {
        smallValue = 0;
        integerMode=0;
      } else {
        switch (integerMode) {
          case 0:
            bool apos = (smallValue > 0L);
            bool bpos = (val > 0L);
            if (
              (apos && ((!bpos && (Int32.MinValue / smallValue) > val) ||
                        (bpos && smallValue > (Int32.MaxValue / val)))) ||
              (!apos && ((!bpos && smallValue != 0L &&
                          (Int32.MaxValue / smallValue) > val) ||
                         (bpos && smallValue < (Int32.MinValue / val))))) {
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
                largeValue = (BigInteger)smallValue;
                largeValue *= (BigInteger)val;
              }
            } else {
              smallValue *= val;
            }
            break;
          case 1:
            if(val<0){
              integerMode=2;
              largeValue=mnum.ToBigInteger();
              largeValue*=(BigInteger)val;
            } else {
              mnum.Multiply(val);
            }
            break;
          case 2:
            largeValue*=(BigInteger)val;
            break;
          default:
            throw new InvalidOperationException();
        }
      }
      return this;
    }

    /// <summary> Sets this object's value to 0 minus its current value (reverses
    /// its sign). </summary>
    /// <returns>This object.</returns>
    public FastInteger Negate() {
      switch (integerMode) {
        case 0:
          if (smallValue == Int32.MinValue) {
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
          largeValue=-(BigInteger)largeValue;
          break;
        case 2:
          largeValue=-(BigInteger)largeValue;
          break;
        default:
          throw new InvalidOperationException();
      }
      return this;
    }

    /// <summary> Sets this object's value to the current value minus the
    /// given FastInteger value. </summary>
    /// <param name='val'>The subtrahend.</param>
    /// <returns>This object.</returns>
    public FastInteger Subtract(FastInteger val) {
      BigInteger valValue;
      switch (integerMode) {
        case 0:
          if(val.integerMode==0){
            int vsv=val.smallValue;
            if ((vsv < 0 && Int32.MaxValue + vsv < smallValue) ||
                (vsv > 0 && Int32.MinValue + vsv > smallValue)) {
              // would overflow, convert to large
              integerMode=2;
              largeValue = (BigInteger)smallValue;
              largeValue -= (BigInteger)vsv;
            } else {
              smallValue-=vsv;
            }
          } else {
            integerMode=2;
            largeValue=(BigInteger)smallValue;
            valValue = val.AsBigInteger();
            largeValue -= (BigInteger)valValue;
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
            largeValue -= (BigInteger)valValue;
          }
          break;
        case 2:
          valValue = val.AsBigInteger();
          largeValue -= (BigInteger)valValue;
          break;
        default:
          throw new InvalidOperationException();
      }
      return this;
    }
    /// <summary> Sets this object's value to the current value minus the
    /// given integer. </summary>
    /// <param name='val'>The subtrahend.</param>
    /// <returns>This object.</returns>
    public FastInteger SubtractInt(int val) {
      if(val==Int32.MinValue){
        return AddBig(NegativeInt32MinValue);
      } else {
        return AddInt(-val);
      }
    }

    /// <summary> Sets this object's value to the current value plus the given
    /// integer. </summary>
    /// <param name='bigintVal'>The number to add.</param>
    /// <returns>This object.</returns>
    public FastInteger AddBig(BigInteger bigintVal) {
      switch (integerMode) {
          case 0:{
            int sign = bigintVal.Sign;
            if (sign == 0 ||
                (sign < 0 && bigintVal.CompareTo(Int32MinValue) >= 0) ||
                (sign > 0 && bigintVal.CompareTo(Int32MaxValue) <= 0)) {
              return AddInt((int)bigintVal);
            }
            return Add(FastInteger.FromBig(bigintVal));
          }
        case 1:
          integerMode=2;
          largeValue=mnum.ToBigInteger();
          largeValue += bigintVal;
          break;
        case 2:
          largeValue += bigintVal;
          break;
        default:
          throw new InvalidOperationException();
      }
      return this;
    }

    /// <summary> Sets this object's value to the current value minus the
    /// given integer. </summary>
    /// <param name='bigintVal'>The subtrahend.</param>
    /// <returns>This object.</returns>
    public FastInteger SubtractBig(BigInteger bigintVal) {
      if (integerMode==2) {
        largeValue -= (BigInteger)bigintVal;
        return this;
      } else {
        int sign = bigintVal.Sign;
        if (sign == 0)return this;
        // Check if this value fits an int, except if
        // it's MinValue
        if(sign < 0 && bigintVal.CompareTo(Int32MinValue) > 0){
          return AddInt(-((int)bigintVal));
        }
        if(sign > 0 && bigintVal.CompareTo(Int32MaxValue) <= 0){
          return SubtractInt((int)bigintVal);
        }
        bigintVal=-bigintVal;
        return AddBig(bigintVal);
      }
    }
    /// <summary> </summary>
    /// <param name='val'>A FastInteger object.</param>
    /// <returns>A FastInteger object.</returns>
    public FastInteger Add(FastInteger val) {
      BigInteger valValue;
      switch (integerMode) {
        case 0:
          if(val.integerMode==0){
            if ((smallValue < 0 && (int)val.smallValue < Int32.MinValue - smallValue) ||
                (smallValue > 0 && (int)val.smallValue > Int32.MaxValue - smallValue)) {
              // would overflow
              if(val.smallValue>=0){
                integerMode=1;
                mnum=new MutableNumber(this.smallValue);
                mnum.Add(val.smallValue);
              } else {
                integerMode=2;
                largeValue = (BigInteger)smallValue;
                largeValue += (BigInteger)val.smallValue;
              }
            } else {
              smallValue+=val.smallValue;
            }
          } else {
            integerMode=2;
            largeValue=(BigInteger)smallValue;
            valValue = val.AsBigInteger();
            largeValue += (BigInteger)valValue;
          }
          break;
        case 1:
          if(val.integerMode==0 && val.smallValue>=0){
            mnum.Add(val.smallValue);
          } else {
            integerMode=2;
            largeValue=mnum.ToBigInteger();
            valValue = val.AsBigInteger();
            largeValue += (BigInteger)valValue;
          }
          break;
        case 2:
          valValue = val.AsBigInteger();
          largeValue += (BigInteger)valValue;
          break;
        default:
          throw new InvalidOperationException();
      }
      return this;
    }
    /// <summary> Sets this object's value to the remainder of the current
    /// value divided by the given integer. </summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>This object.</returns>
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
            largeValue = largeValue % (BigInteger)divisor;
            smallValue = (int)largeValue;
            integerMode=0;
            break;
          case 2:
            largeValue = largeValue % (BigInteger)divisor;
            smallValue = (int)largeValue;
            integerMode=0;
            break;
          default:
            throw new InvalidOperationException();
        }
      } else {
        throw new DivideByZeroException();
      }
      return this;
    }

    /// <summary> </summary>
    /// <returns>A FastInteger object.</returns>
public FastInteger Increment(){
      if(integerMode==0){
        if(smallValue!=Int32.MaxValue){
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

    /// <summary> Divides this instance by the value of a 32-bit signed integer.</summary>
    /// <param name='divisor'>A 32-bit signed integer.</param>
    /// <returns>The quotient of the two objects.</returns>
    public FastInteger Divide(int divisor) {
      if (divisor != 0) {
        switch (integerMode) {
          case 0:
            if (divisor == -1 && smallValue == Int32.MinValue) {
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
            largeValue/=(BigInteger)divisor;
            if(largeValue.IsZero){
              integerMode=0;
              smallValue=0;
            }
            break;
          case 2:
            largeValue/=(BigInteger)divisor;
            if(largeValue.IsZero){
              integerMode=0;
              smallValue=0;
            }
            break;
          default:
            throw new InvalidOperationException();
        }
      } else {
        throw new DivideByZeroException();
      }
      return this;
    }

    /// <summary> </summary>
    public bool IsEvenNumber{
      get {
        switch (integerMode) {
          case 0:
            return (smallValue&1)==0;
          case 1:
            return mnum.IsEvenNumber;
          case 2:
            return largeValue.IsEven;
          default:
            throw new InvalidOperationException();
        }
      }
    }

    /// <summary> </summary>
    /// <param name='val'>A 32-bit signed integer.</param>
    /// <returns>A FastInteger object.</returns>
    public FastInteger AddInt(int val) {
      BigInteger valValue;
      switch (integerMode) {
        case 0:
          if ((smallValue < 0 && (int)val < Int32.MinValue - smallValue) ||
              (smallValue > 0 && (int)val > Int32.MaxValue - smallValue)) {
            // would overflow
            if(val>=0){
              integerMode=1;
              mnum=new MutableNumber(this.smallValue);
              mnum.Add(val);
            } else {
              integerMode=2;
              largeValue = (BigInteger)smallValue;
              largeValue += (BigInteger)val;
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
            valValue = (BigInteger)val;
            largeValue += (BigInteger)valValue;
          }
          break;
        case 2:
          valValue = (BigInteger)val;
          largeValue += (BigInteger)valValue;
          break;
        default:
          throw new InvalidOperationException();
      }
      return this;
    }

    /// <summary> </summary>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInInt32() {
      int sign;
      switch(this.integerMode){
        case 0:
          return true;
        case 1:
          return mnum.CanFitInInt32();
          case 2:{
            sign = largeValue.Sign;
            if (sign == 0) return true;
            if (sign < 0) return largeValue.CompareTo(Int32MinValue) >= 0;
            return largeValue.CompareTo(Int32MaxValue) <= 0;
          }
        default:
          throw new InvalidOperationException();
      }
    }

    /// <summary> Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      switch(this.integerMode){
        case 0:
          return Convert.ToString((int)smallValue, System.Globalization.CultureInfo.InvariantCulture);
        case 1:
          return mnum.ToBigInteger().ToString();
        case 2:
          return largeValue.ToString();
        default:
          return "";
      }
    }
    /// <summary> </summary>
    public int Sign {
      get {
        switch(this.integerMode){
          case 0:
            return Math.Sign(this.smallValue);
          case 1:
            return mnum.Sign;
          case 2:
            return largeValue.Sign;
          default:
            return 0;
        }
      }
    }

    /// <summary>Compares a 32-bit signed integer with this instance.</summary>
    /// <param name='val'>A 32-bit signed integer.</param>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    public int CompareToInt(int val) {
      switch(this.integerMode){
        case 0:
          return (val == smallValue) ? 0 : (smallValue < val ? -1 : 1);
        case 1:
          return mnum.ToBigInteger().CompareTo((BigInteger)val);
        case 2:
          return largeValue.CompareTo((BigInteger)val);
        default:
          return 0;
      }
    }

    /// <summary> </summary>
    /// <param name='val'>A 32-bit signed integer.</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int MinInt32(int val) {
      return this.CompareToInt(val)<0 ? this.AsInt32() : val;
    }

    /// <summary> </summary>
    /// <returns>A BigInteger object.</returns>
    public BigInteger AsBigInteger() {
      switch(this.integerMode){
        case 0:
          return (BigInteger)smallValue;
        case 1:
          return mnum.ToBigInteger();
        case 2:
          return largeValue;
        default:
          throw new InvalidOperationException();
      }
    }
  }
}
