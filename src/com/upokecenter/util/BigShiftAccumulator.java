package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */


import java.math.*;


  final class BigShiftAccumulator {
    int bitLeftmost=0;
    
    /**
     * Gets whether the last discarded bit was set.
     */
    public int getBitLeftmost() { return bitLeftmost; }
    int bitsAfterLeftmost=0;
    
    /**
     * Gets whether any of the discarded bits to the right of the last one was
     * set.
     */
    public int getBitsAfterLeftmost() { return bitsAfterLeftmost; }
    BigInteger shiftedInt;
    
    public BigInteger getShiftedInt() { return shiftedInt; }
    FastInteger discardedBitCount;
    
    public FastInteger getDiscardedBitCount() { return discardedBitCount; }
    public BigShiftAccumulator(BigInteger bigint){
      shiftedInt=bigint;
      discardedBitCount=new FastInteger();
    }
    public void ShiftRight(FastInteger fastint) {
      if(fastint.signum()<=0)return;
      if(fastint.CanFitInInt64()){
        ShiftRight(fastint.AsInt64());
      } else {
        BigInteger bi=fastint.AsBigInteger();
        while(bi.signum()>0){
          long count=1000000;
          if(bi.compareTo(BigInteger.valueOf(1000000))<0){
            count=bi.longValue();
          }
          ShiftRight(count);
          bi=bi.subtract(BigInteger.valueOf(count));
        }
      }
    }
    private static byte[] ReverseBytes(byte[] bytes) {
      if ((bytes) == null) throw new NullPointerException("bytes");
      int half = bytes.length >> 1;
      int right = bytes.length - 1;
      for (int i = 0; i < half; i++, right--) {
        byte value = bytes[i];
        bytes[i] = bytes[right];
        bytes[right] = value;
      }
      return bytes;
    }
  
    /**
     * Shifts a number to the right, gathering information on whether the
     * last bit discarded is set and whether the discarded bits to the right
     * of that bit are set. Assumes that the big integer being shifted is positive.
     */
    public void ShiftRight(long bits) {
      if(bits<=0)return;
      if(shiftedInt.signum()==0){
        bitsAfterLeftmost|=bitLeftmost;
        bitLeftmost=0;
        return;
      }
      byte[] bytes=ReverseBytes(shiftedInt.toByteArray());
      long bitLength=((long)bytes.length)<<3;
      // Find the last bit set
      for(int i=bytes.length-1;i>=0;i--){
        int b=(int)bytes[i];
        if(b!=0){
          if((b&0x80)!=0){ bitLength-=0; break; }
          if((b&0x40)!=0){ bitLength-=1; break; }
          if((b&0x20)!=0){ bitLength-=2; break; }
          if((b&0x10)!=0){ bitLength-=3; break; }
          if((b&0x08)!=0){ bitLength-=4; break; }
          if((b&0x04)!=0){ bitLength-=5; break; }
          if((b&0x02)!=0){ bitLength-=6; break; }
          if((b&0x01)!=0){ bitLength-=7; break; }
        }
        bitLength-=8;
      }
      long bitDiff=0;
      if(bits>bitLength){
        bitDiff=bits-bitLength;
      }
      long bitShift=Math.min(bitLength,bits);
      if(bits>=bitLength){
        shiftedInt=BigInteger.ZERO;
      } else {
        long tmpBitShift=bitShift;
        while(tmpBitShift>0 && shiftedInt.signum()!=0){
          int bs=(int)Math.min(1000000,tmpBitShift);
          shiftedInt=shiftedInt.shiftRight(bs);
          tmpBitShift-=bs;
        }
      }
      discardedBitCount.Add(bits);
      bitsAfterLeftmost|=bitLeftmost;
      for(int i=0;i<bytes.length;i++){
        if(bitShift>8){
          // Discard all the bits, they come
          // after the leftmost bit
          bitsAfterLeftmost|=bytes[i];
          bitShift-=8;
        } else {
          // 8 or fewer bits left.
          // Get the bottommost bitShift minus 1 bits
          bitsAfterLeftmost|=((bytes[i]<<(9-(int)bitShift))&0xFF);
          // Get the bit just above those bits
          bitLeftmost=(bytes[i]>>(((int)bitShift)-1))&0x01;
          break;
        }
      }
      bitsAfterLeftmost=(bitsAfterLeftmost!=0) ? 1 : 0;
      if(bitDiff>0){
        // Shifted more bits than the bit length
        bitsAfterLeftmost|=bitLeftmost;
        bitLeftmost=0;
      }
    }
  
    /**
     * Shifts a number until it reaches the given number of bits, gathering
     * information on whether the last bit discarded is set and whether the
     * discarded bits to the right of that bit are set. Assumes that the big
     * integer being shifted is positive.
     */
    public void ShiftToBits(long bits) {
      byte[] bytes=ReverseBytes(shiftedInt.toByteArray());
      long bitLength=((long)bytes.length)<<3;
      // Find the last bit set
      for(int i=bytes.length-1;i>=0;i--){
        int b=(int)bytes[i];
        if(b!=0){
          if((b&0x80)!=0){ bitLength-=0; break; }
          if((b&0x40)!=0){ bitLength-=1; break; }
          if((b&0x20)!=0){ bitLength-=2; break; }
          if((b&0x10)!=0){ bitLength-=3; break; }
          if((b&0x08)!=0){ bitLength-=4; break; }
          if((b&0x04)!=0){ bitLength-=5; break; }
          if((b&0x02)!=0){ bitLength-=6; break; }
          if((b&0x01)!=0){ bitLength-=7; break; }
        }
        bitLength-=8;
      }
      // Shift by the difference in bit length
      if(bitLength>bits){
        long bitShift=bitLength-bits;
        long tmpBitShift=bitShift;
        while(tmpBitShift>0 && shiftedInt.signum()!=0){
          int bs=(int)Math.min(1000000,tmpBitShift);
          shiftedInt=shiftedInt.shiftRight(bs);
          tmpBitShift-=bs;
        }
        bitsAfterLeftmost|=bitLeftmost;
        discardedBitCount.Add(bitShift);
        for(int i=0;i<bytes.length;i++){
          if(bitShift>8){
            // Discard all the bits, they come
            // after the leftmost bit
            bitsAfterLeftmost|=bytes[i];
            bitShift-=8;
          } else {
            // 8 or fewer bits left.
            // Get the bottommost bitShift minus 1 bits
            bitsAfterLeftmost|=((bytes[i]<<(9-(int)bitShift))&0xFF);
            // Get the bit just above those bits
            bitLeftmost=(bytes[i]>>(((int)bitShift)-1))&0x01;
            break;
          }
        }
        bitsAfterLeftmost=(bitsAfterLeftmost!=0) ? 1 : 0;
      }
    }
  }
