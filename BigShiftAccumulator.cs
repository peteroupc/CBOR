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

namespace PeterO
{
  internal sealed class BigShiftAccumulator {
    int bitLeftmost=0;
    
    /// <summary>
    /// Gets whether the last discarded bit was set.
    /// </summary>
    public int BitLeftmost {
      get { return bitLeftmost; }
    }
    int bitsAfterLeftmost=0;
    
    /// <summary>
    /// Gets whether any of the discarded bits
    /// to the right of the last one was set.
    /// </summary>
    public int BitsAfterLeftmost {
      get { return bitsAfterLeftmost; }
    }
    BigInteger shiftedInt;
    
    public BigInteger ShiftedInt {
      get { return shiftedInt; }
    }
    FastInteger discardedBitCount;
    
    public FastInteger DiscardedBitCount {
      get { return discardedBitCount; }
    }
    public BigShiftAccumulator(BigInteger bigint){
      shiftedInt=bigint;
      discardedBitCount=new FastInteger();
    }
    public void ShiftRight(FastInteger fastint){
      if(fastint.Sign<=0)return;
      if(fastint.CanFitInInt64()){
        ShiftRight(fastint.AsInt64());
      } else {
        BigInteger bi=fastint.AsBigInteger();
        while(bi.Sign>0){
          long count=1000000;
          if(bi.CompareTo((BigInteger)1000000)<0){
            count=(long)bi;
          }
          ShiftRight(count);
          bi-=(BigInteger)count;
        }
      }
    }
    private static byte[] ReverseBytes(byte[] bytes) {
      if ((bytes) == null) throw new ArgumentNullException("bytes");
      int half = bytes.Length >> 1;
      int right = bytes.Length - 1;
      for (int i = 0; i < half; i++, right--) {
        byte value = bytes[i];
        bytes[i] = bytes[right];
        bytes[right] = value;
      }
      return bytes;
    }
  
    /// <summary>
    /// Shifts a number to the right,
    /// gathering information
    /// on whether the last bit discarded is set
    /// and whether the discarded bits to the right
    /// of that bit are set.
    /// Assumes that the big integer being shifted
    /// is positive.
    /// </summary>
    public void ShiftRight(long bits){
      if(bits<=0)return;
      if(shiftedInt.IsZero){
        bitsAfterLeftmost|=bitLeftmost;
        bitLeftmost=0;
        return;
      }
      byte[] bytes=shiftedInt.ToByteArray();
      long bitLength=((long)bytes.Length)<<3;
      // Find the last bit set
      for(int i=bytes.Length-1;i>=0;i--){
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
      long bitShift=Math.Min(bitLength,bits);
      if(bits>=bitLength){
        shiftedInt=BigInteger.Zero;
      } else {
        long tmpBitShift=bitShift;
        while(tmpBitShift>0 && !shiftedInt.IsZero){
          int bs=(int)Math.Min(1000000,tmpBitShift);
          shiftedInt>>=bs;
          tmpBitShift-=bs;
        }
      }
      discardedBitCount.Add(bits);
      bitsAfterLeftmost|=bitLeftmost;
      for(int i=0;i<bytes.Length;i++){
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
  
    /// <summary>
    /// Shifts a number until it reaches the
    /// given number of bits, gathering information
    /// on whether the last bit discarded is set
    /// and whether the discarded bits to the right
    /// of that bit are set.
    /// Assumes that the big integer being shifted
    /// is positive.
    /// </summary>
    public void ShiftToBits(long bits){
      byte[] bytes=shiftedInt.ToByteArray();
      long bitLength=((long)bytes.Length)<<3;
      // Find the last bit set
      for(int i=bytes.Length-1;i>=0;i--){
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
        while(tmpBitShift>0 && !shiftedInt.IsZero){
          int bs=(int)Math.Min(1000000,tmpBitShift);
          shiftedInt>>=bs;
          tmpBitShift-=bs;
        }
        bitsAfterLeftmost|=bitLeftmost;
        discardedBitCount.Add(bitShift);
        for(int i=0;i<bytes.Length;i++){
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
}
