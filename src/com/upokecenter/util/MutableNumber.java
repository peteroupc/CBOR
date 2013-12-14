package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

//import java.math.*;

  final class MutableNumber {
    int[] data;
    int length;
    private MutableNumber() {
      data = new int[2];
      length = 0;
      data[0] = 0;
    }
    public MutableNumber(BigInteger bigintVal) {
      if ( bigintVal.signum() < 0)
        throw new IllegalArgumentException("Only positive integers are supported");
      byte[] bytes=bigintVal.toByteArray(true);
      int len=bytes.length;
      this.length=Math.max(4,(len/4)+1);
      data=new int[this.length];
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
          data[i>>2]=x;
        }
      }
      // Calculate the correct data length
      while(this.length!=0 && this.data[this.length-1]==0)
        this.length--;
    }
    public MutableNumber(int val) {
      if (val < 0)
        throw new IllegalArgumentException("Only positive integers are supported");
      data = new int[4];
      length = (val==0) ? 0 : 1;
      data[0] = ((int)((val) & 0xFFFFFFFFL));
    }
    
    public static BigInteger WordsToBigInteger(int[] words) {
      return new MutableNumber().SetLastBits(words).ToBigInteger();
    }
    
    /**
     * 
     */
    public BigInteger ToBigInteger() {
      if(length==1 && (data[0]>>31)==0){
        return BigInteger.valueOf((int)data[0]);
      }
      byte[] bytes = new byte[length * 4 + 1];
      for (int i = 0; i < length; i++) {
        bytes[i * 4 + 0] = (byte)((data[i]) & 0xFF);
        bytes[i * 4 + 1] = (byte)((data[i] >> 8) & 0xFF);
        bytes[i * 4 + 2] = (byte)((data[i] >> 16) & 0xFF);
        bytes[i * 4 + 3] = (byte)((data[i] >> 24) & 0xFF);
      }
      bytes[bytes.length - 1] = 0;
      return BigInteger.fromByteArray((byte[])bytes,true);
    }
    
    private int[] GetLastWordsInternal(int numWords32Bit) {
      int[] ret=new int[numWords32Bit];
      System.arraycopy(data,0,ret,0,Math.min(numWords32Bit,this.length));
      return ret;
    }
    
    public static int[] GetLastWords(BigInteger bigint, int numWords32Bit) {
      return new MutableNumber(bigint).GetLastWordsInternal(numWords32Bit);
    }
    
    /**
     * 
     */
public boolean CanFitInInt32() {
      return length==0 || (length==1 && (data[0]>>31)==0);
    }
    
    /**
     * 
     * @param bits A int[] object.
     */
    private MutableNumber SetLastBits(int[] bits) {
      if (this.data.length < bits.length) {
        int[] newdata = new int[bits.length + 20];
        System.arraycopy(data, 0, newdata, 0,
                   Math.min(this.data.length,newdata.length));
        data = newdata;
      }
      System.arraycopy(bits,0,data,0,bits.length);
      this.length=Math.max(bits.length,this.length);
      // Calculate the correct data length
      while(this.length!=0 && this.data[this.length-1]==0)
        this.length--;
      return this;
    }
    
    /**
     * 
     */
    public MutableNumber Copy() {
      MutableNumber mbi=new MutableNumber();
      mbi.data=new int[this.length];
      System.arraycopy(this.data,0,mbi.data,0,this.length);
      mbi.length=this.length;
      return mbi;
    }
    
    /**
     * Multiplies this instance by the value of a Int32 object.
     * @param multiplicand A 32-bit signed integer.
     * @return The product of the two objects.
     */
    public MutableNumber Multiply(int multiplicand) {
      if (multiplicand < 0)
        throw new IllegalArgumentException("Only positive multiplicands are supported");
      else if (multiplicand != 0) {
        int carry = 0;
        if(this.length==0){
          if(this.data.length==0)this.data=new int[4];
          this.length=1;
        }
        for (int i = 0; i < this.length; i++) {
          long subproduct = ((long)data[i]) & 0xFFFFFFFFL;
          subproduct *= multiplicand;
          subproduct += carry;
          carry = ((int)((subproduct >> 32) & 0xFFFFFFFFL));
          data[i] = ((int)((subproduct) & 0xFFFFFFFFL));
        }
        if (carry != 0) {
          if (length >= data.length) {
            int[] newdata = new int[length + 20];
            System.arraycopy(data, 0, newdata, 0, data.length);
            data = newdata;
          }
          data[length] = carry;
          length++;
        }
      } else {
        if(data.length>0)data[0] = 0;
        length = 0;
      }
      return this;
    }
    
    /**
     * 
     */
    public int signum() {
        int ret=0;
        for (int i = 0; i < length && ret==0; i++) {
          ret|=data[i];
        }
        return (ret==0 ? 0 : 1);
      }

    /**
     * 
     */
    public boolean isEvenNumber() {
        return (length==0 || (data[0]&1)==0);
      }
    
    /**
     * Subtracts a MutableBigInteger object from this instance.
     * @param other A MutableBigInteger object.
     * @return The difference of the two objects.
     */
    public MutableNumber Subtract(
      MutableNumber other
     ) {
      {
        // System.out.println("{0} {1}",this.data.length,other.data.length);
        int neededSize=Math.max(this.length,other.length);
        if(data.length<neededSize){
          int[] newdata = new int[neededSize + 20];
          System.arraycopy(data, 0, newdata, 0, data.length);
          data = newdata;
        }
        neededSize=Math.min(this.length,other.length);
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

    /**
     * Compares a MutableBigInteger object with this instance.
     * @param other A MutableBigInteger object.
     * @return Zero if the values are equal; a negative number if this instance
     * is less, or a positive number if this instance is greater.
     */
    public int compareTo(MutableNumber other) {
      if(this.length!=other.length){
        return (this.length<other.length) ? -1 : 1;
      }
      int N=this.length;
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
      else if (augend != 0) {
        int carry = 0;
        // Ensure a length of at least 1
        if(this.length==0){
          if(this.data.length==0)this.data=new int[4];
          this.length=1;
        }
        for (int i = 0; i < length; i++) {
          long subproduct = ((long)data[i]) & 0xFFFFFFFFL;
          subproduct += augend;
          subproduct += carry;
          carry = ((int)((subproduct >> 32) & 0xFFFFFFFFL));
          data[i] = ((int)((subproduct) & 0xFFFFFFFFL));
          if (carry == 0) return this;
          augend = 0;
        }
        if (carry != 0) {
          if (length >= data.length) {
            int[] newdata = new int[length + 20];
            System.arraycopy(data, 0, newdata, 0, data.length);
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
