/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
//using System.Numerics;
namespace PeterO {
  internal sealed class MutableNumber {
    int[] data;
    int length;
    private MutableNumber() {
      data = new int[2];
      length = 0;
      data[0] = 0;
    }
    public MutableNumber(BigInteger bigintVal) {
      if ( bigintVal.Sign < 0)
        throw new ArgumentException("Only positive integers are supported");
      byte[] bytes=bigintVal.ToByteArray();
      int len=bytes.Length;
      this.length=Math.Max(4,(len/4)+1);
      data=new int[this.length];
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
          data[i>>2]=x;
        }
      }
      // Calculate the correct data length
      while(this.length!=0 && this.data[this.length-1]==0)
        this.length--;
    }
    public MutableNumber(int val) {
      if (val < 0)
        throw new ArgumentException("Only positive integers are supported");
      data = new int[4];
      length = (val==0) ? 0 : 1;
      data[0] = unchecked((int)((val) & 0xFFFFFFFFL));
    }
    
    public static BigInteger WordsToBigInteger(int[] words){
      return new MutableNumber().SetLastBits(words).ToBigInteger();
    }
    
    /// <summary> </summary>
    /// <returns></returns>
    public BigInteger ToBigInteger() {
      if(length==1 && (data[0]>>31)==0){
        return (BigInteger)((int)data[0]);
      }
      byte[] bytes = new byte[length * 4 + 1];
      for (int i = 0; i < length; i++) {
        bytes[i * 4 + 0] = (byte)((data[i]) & 0xFF);
        bytes[i * 4 + 1] = (byte)((data[i] >> 8) & 0xFF);
        bytes[i * 4 + 2] = (byte)((data[i] >> 16) & 0xFF);
        bytes[i * 4 + 3] = (byte)((data[i] >> 24) & 0xFF);
      }
      bytes[bytes.Length - 1] = 0;
      return new BigInteger((byte[])bytes);
    }
    
    private int[] GetLastWordsInternal(int numWords32Bit){
      int[] ret=new int[numWords32Bit];
      Array.Copy(data,ret,Math.Min(numWords32Bit,this.length));
      return ret;
    }
    
    public static int[] GetLastWords(BigInteger bigint, int numWords32Bit){
      return new MutableNumber(bigint).GetLastWordsInternal(numWords32Bit);
    }
    
    /// <summary> </summary>
    /// <returns></returns>
public bool CanFitInInt32(){
      return length==0 || (length==1 && (data[0]>>31)==0);
    }
    
    /// <summary> </summary>
    /// <param name='bits'>A int[] object.</param>
    /// <returns/>
    private MutableNumber SetLastBits(int[] bits){
      if (this.data.Length < bits.Length) {
        int[] newdata = new int[bits.Length + 20];
        Array.Copy(data, 0, newdata, 0,
                   Math.Min(this.data.Length,newdata.Length));
        data = newdata;
      }
      Array.Copy(bits,data,bits.Length);
      this.length=Math.Max(bits.Length,this.length);
      // Calculate the correct data length
      while(this.length!=0 && this.data[this.length-1]==0)
        this.length--;
      return this;
    }
    
    /// <summary> </summary>
    /// <returns></returns>
    public MutableNumber Copy(){
      MutableNumber mbi=new MutableNumber();
      mbi.data=new int[this.length];
      Array.Copy(this.data,mbi.data,this.length);
      mbi.length=this.length;
      return mbi;
    }
    
    /// <summary> Multiplies this instance by the value of a Int32 object.</summary>
    /// <param name='multiplicand'> A 32-bit signed integer.</param>
    /// <returns> The product of the two objects.</returns>
    public MutableNumber Multiply(int multiplicand) {
      if (multiplicand < 0)
        throw new ArgumentException("Only positive multiplicands are supported");
      else if (multiplicand != 0) {
        int carry = 0;
        if(this.length==0){
          if(this.data.Length==0)this.data=new int[4];
          this.length=1;
        }
        for (int i = 0; i < this.length; i++) {
          long subproduct = ((long)data[i]) & 0xFFFFFFFFL;
          subproduct *= multiplicand;
          subproduct += carry;
          carry = unchecked((int)((subproduct >> 32) & 0xFFFFFFFFL));
          data[i] = unchecked((int)((subproduct) & 0xFFFFFFFFL));
        }
        if (carry != 0) {
          if (length >= data.Length) {
            int[] newdata = new int[length + 20];
            Array.Copy(data, 0, newdata, 0, data.Length);
            data = newdata;
          }
          data[length] = carry;
          length++;
        }
      } else {
        if(data.Length>0)data[0] = 0;
        length = 0;
      }
      return this;
    }
    
    /// <summary> </summary>
    public int Sign{
      get {
        int ret=0;
        for (int i = 0; i < length && ret==0; i++) {
          ret|=data[i];
        }
        return (ret==0 ? 0 : 1);
      }
    }

    /// <summary> </summary>
    public bool IsEvenNumber{
      get {
        return (length==0 || (data[0]&1)==0);
      }
    }
    
    /// <summary>Subtracts a MutableBigInteger object from this instance.</summary>
    /// <param name='other'>A MutableBigInteger object.</param>
    /// <returns>The difference of the two objects.</returns>
    public MutableNumber Subtract(
      MutableNumber other
     ) {
      unchecked {
        // Console.WriteLine("{0} {1}",this.data.Length,other.data.Length);
        int neededSize=Math.Max(this.length,other.length);
        if(data.Length<neededSize){
          int[] newdata = new int[neededSize + 20];
          Array.Copy(data, 0, newdata, 0, data.Length);
          data = newdata;
        }
        neededSize=Math.Min(this.length,other.length);
        long u;
        u = 0;
        for (int i = 0; i < neededSize; i ++) {
          u = (((long)this.data[i]) & 0xFFFFFFFFL) -
            (((long)other.data[i]) & 0xFFFFFFFFL) - (long)((u >> 63) & 1);
          this.data[i] = (int)(u);
        }
        if(((u >> 63) & 1)!=0){
          for (int i = neededSize; i < this.length; i++) {
            u = (((long)this.data[i]) & 0xFFFFFFFFL) - (long)((u >> 63) & 1);
            this.data[i] = (int)(u);
          }
        }
        // Calculate the correct data length
        while(this.length!=0 && this.data[this.length-1]==0)
          this.length--;
        return this;
      }
    }

    /// <summary>Compares a MutableBigInteger object with this instance.</summary>
    /// <param name='other'>A MutableBigInteger object.</param>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    public int CompareTo(MutableNumber other){
      if(this.length!=other.length){
        return (this.length<other.length) ? -1 : 1;
      }
      int N=this.length;
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
      else if (augend != 0) {
        int carry = 0;
        // Ensure a length of at least 1
        if(this.length==0){
          if(this.data.Length==0)this.data=new int[4];
          this.length=1;
        }
        for (int i = 0; i < length; i++) {
          long subproduct = ((long)data[i]) & 0xFFFFFFFFL;
          subproduct += augend;
          subproduct += carry;
          carry = unchecked((int)((subproduct >> 32) & 0xFFFFFFFFL));
          data[i] = unchecked((int)((subproduct) & 0xFFFFFFFFL));
          if (carry == 0) return this;
          augend = 0;
        }
        if (carry != 0) {
          if (length >= data.Length) {
            int[] newdata = new int[length + 20];
            Array.Copy(data, 0, newdata, 0, data.Length);
            data = newdata;
          }
          data[length] = carry;
          length++;
        }
      }
      // Calculate the correct data length
      while(this.length!=0 && this.data[this.length-1]==0)
        this.length--;
      return this;
    }
  }
}