package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.util.*;

import java.io.*;
import java.math.*;


    /**
     * 
     */
 final class CBORObjectPortable
  {
    private static void WritePortable(BigInteger bigint, OutputStream s) throws IOException {
      if((s)==null)throw new NullPointerException("s");
      if((Object)bigint==(Object)null){
        s.write(0xf6);
        return;
      }
      int datatype=0;
      if(bigint.signum()<0){
        datatype=1;
        bigint=bigint.add(BigInteger.ONE);
        bigint=(bigint).negate();
      }
      if(bigint.compareTo(Int64MaxValue)<=0){
        // If the big integer is representable as a long and in
        // major type 0 or 1, write that major type
        // instead of as a bignum
        long ui=bigint.longValue();
        WritePositiveInt64(datatype,ui,s);
      } else {
        java.io.ByteArrayOutputStream ms=null;
try {
ms=new ByteArrayOutputStream();

          long tmp=0;
          byte[] buffer=new byte[10];
          while(bigint.signum()>0){
            // To reduce the number of big integer
            // operations, extract the big int 56 bits at a time
            // (not 64, to avoid negative numbers)
            BigInteger tmpbigint=bigint.and(BigInteger.valueOf(FiftySixBitMask));
            tmp=tmpbigint.longValue();
            bigint=bigint.shiftRight(56);
            boolean isNowZero=(bigint.signum()==0);
            int bufferindex=0;
            for(int i=0;i<7 && (!isNowZero || tmp>0);i++){
              buffer[bufferindex]=(byte)(tmp&0xFF);
              tmp>>=8;
              bufferindex++;
            }
            ms.write(buffer,0,bufferindex);
          }
          byte[] bytes=ms.toByteArray();
          switch(bytes.length){
            case 1: // Fits in 1 byte (won't normally happen though)
              buffer[0]=(byte)((datatype<<5)|24);
              buffer[1]=bytes[0];
              s.write(buffer,0,2);
              break;
            case 2: // Fits in 2 bytes (won't normally happen though)
              buffer[0]=(byte)((datatype<<5)|25);
              buffer[1]=bytes[1];
              buffer[2]=bytes[0];
              s.write(buffer,0,3);
              break;
            case 3:
            case 4:
              buffer[0]=(byte)((datatype<<5)|26);
              buffer[1]=(bytes.length>3) ? bytes[3] : (byte)0;
              buffer[2]=bytes[2];
              buffer[3]=bytes[1];
              buffer[4]=bytes[0];
              s.write(buffer,0,5);
              break;
            case 5:
            case 6:
            case 7:
            case 8:
              buffer[0]=(byte)((datatype<<5)|27);
              buffer[1]=(bytes.length>7) ? bytes[7] : (byte)0;
              buffer[2]=(bytes.length>6) ? bytes[6] : (byte)0;
              buffer[3]=(bytes.length>5) ? bytes[5] : (byte)0;
              buffer[4]=bytes[4];
              buffer[5]=bytes[3];
              buffer[6]=bytes[2];
              buffer[7]=bytes[1];
              buffer[8]=bytes[0];
              s.write(buffer,0,9);
              break;
            default:
              s.write((datatype==0) ?
                          (byte)0xC2 :
                          (byte)0xC3);
              WritePositiveInt(2,bytes.length,s);
              for(int i=bytes.length-1;i>=0;i--){
                s.write(bytes[i]);
              }
              break;
          }
}
finally {
try { if(ms!=null)ms.close(); } catch(IOException ex){}
}
      }
    }
    
    // This is a more "portable" version of ConvertToBigNum,
    // but it's much slower on relatively large BigIntegers.
    private static CBORObject ConvertToBigNumPortable(CBORObject o, boolean negative) {
      if(o.getItemType()!=CBORObjectType_ByteString)
        throw new CBORException("Byte array expected");
      byte[] data=(byte[])o.getThisItem();
      if(data.length<=7){
        long x=0;
        if(data.length>0)x|=(((long)data[0])&0xFF)<<48;
        if(data.length>1)x|=(((long)data[1])&0xFF)<<40;
        if(data.length>2)x|=(((long)data[2])&0xFF)<<32;
        if(data.length>3)x|=(((long)data[3])&0xFF)<<24;
        if(data.length>4)x|=(((long)data[4])&0xFF)<<16;
        if(data.length>5)x|=(((long)data[5])&0xFF)<<8;
        if(data.length>6)x|=(((long)data[6])&0xFF);
        if(negative)x=-x;
        return FromObject(x);
      }
      BigInteger bi=BigInteger.ZERO;
      for(int i=0;i<data.length;i++){
        if(i+7<=data.length){
          long x=(((long)data[i])&0xFF)<<48;
          x|=(((long)data[i+1])&0xFF)<<40;
          x|=(((long)data[i+2])&0xFF)<<32;
          x|=(((long)data[i+3])&0xFF)<<24;
          x|=(((long)data[i+4])&0xFF)<<16;
          x|=(((long)data[i+5])&0xFF)<<8;
          x|=(((long)data[i+6])&0xFF);
          bi=bi.shiftLeft(56);
          bi=bi.or(BigInteger.valueOf(x));
          i+=6;
        } else {
          bi=bi.shiftLeft(8);
          int x=((int)data[i])&0xFF;
          bi=bi.or(BigInteger.valueOf(x));
        }
      }
      if(negative){
        bi=BigInteger.valueOf(-1).subtract(bi); // Convert to a negative
      }
      return RewrapObject(o,FromObject(bi));
    }
    
  }
