package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.math.*;


  final class SmallShiftAccumulator {
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
    long shiftedLong;
    
    public long getShiftedInt() { return shiftedLong; }
    FastInteger discardedBitCount;
    
    public FastInteger getDiscardedBitCount() { return discardedBitCount; }
    public SmallShiftAccumulator(long longInt){
      if(longInt<0)
        throw new IllegalArgumentException("longInt is negative");
      shiftedLong=longInt;
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
  
    /**
     * Shifts a number to the right, gathering information on whether the
     * last bit discarded is set and whether the discarded bits to the right
     * of that bit are set. Assumes that the big integer being shifted is positive.
     */
    public void ShiftRight(long bits) {
      if(bits<=0)return;
      if(shiftedLong==0){
        bitsAfterLeftmost|=bitLeftmost;
        bitLeftmost=0;
        return;
      }
      long bitLength=64;
      for(int i=63;i>=0;i++){
        if((shiftedLong&(1L<<i))!=0){
          break;
        } else {
          bitLength--;
        }
      }
      int shift=(int)Math.min(bitLength,bits);
      discardedBitCount.Add(bits);
      bitsAfterLeftmost|=bitLeftmost;
      // Get the bottommost shift minus 1 bits
      bitsAfterLeftmost|=(((shiftedLong<<(65-shift))!=0) ? 1 : 0);
      // Get the bit just above that bit
      bitLeftmost=(int)((shiftedLong>>(((int)shift)-1))&0x01);
      shiftedLong>>=shift;
      if(bits>bitLength){
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
      long bitLength=64;
      for(int i=63;i>=0;i++){
        if((shiftedLong&(1L<<i))!=0){
          break;
        } else {
          bitLength--;
        }
      }
      // Shift by the difference in bit length
      if(bitLength>bits){
        long bitShift=bitLength-bits;
        int shift=(int)bitShift;
        discardedBitCount.Add(bitShift);
        bitsAfterLeftmost|=bitLeftmost;
        // Get the bottommost shift minus 1 bits
        bitsAfterLeftmost|=(((shiftedLong<<(65-shift))!=0) ? 1 : 0);
        // Get the bit just above that bit
        bitLeftmost=(int)((shiftedLong>>(((int)shift)-1))&0x01);
        shiftedLong>>=shift;
      }
    }
  }
