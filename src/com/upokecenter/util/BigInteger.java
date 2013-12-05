package com.upokecenter.util;
/*
Written in 2013 by Peter O.

Parts of the code were adapted by Peter O. from
the public-domain library CryptoPP by Wei Dai.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */


    /**
     * An arbitrary-precision integer.
     */
  public final class BigInteger implements Comparable<BigInteger>
  {

    private static boolean LittleEndianSerialize=false;

    private static int CountWords(short[] X, int N) {
      while (N!=0 && X[N-1]==0)
        N--;
      return (int)N;
    }
    

    private static void SetWords(short[] r, int rstart, short a, int n) {
      for (int i=0; i<n; i++)
        r[rstart+i] = a;
    }


    private static short ShiftWordsLeftByBits(short[] r, int rstart, int n, int shiftBits) {
      

      {
        short u, carry=0;
        if (shiftBits!=0){
          for (int i=0; i<n; i++)
          {
            u = r[rstart+i];
            r[rstart+i] = (short)((int)(u << (int)shiftBits) | (((int)carry)&0xFFFF));
            carry = (short)((((int)u)&0xFFFF) >> (int)(16-shiftBits));
          }
        }
        return carry;
      }
    }

    private static short ShiftWordsRightByBits(short[] r, int rstart, int n, int shiftBits) {
      //Debugif(!(shiftBits<16))Assert.fail("{0} line {1}: shiftBits<16","words.h",67);
      short u, carry=0;
      {
        if (shiftBits!=0)
          for (int i=n; i>0; i--)
        {
          u = r[rstart+i-1];
          r[rstart+i-1] = (short)((((((int)u)&0xFFFF) >> (int)shiftBits)&0xFFFF) | (((int)carry)&0xFFFF));
          carry = (short)((((int)u)&0xFFFF) << (int)(16-shiftBits));
        }
        return carry;
      }
    }

    private static short ShiftWordsRightByBitsSignExtend(short[] r, int rstart, int n, int shiftBits) {
      //Debugif(!(shiftBits<16))Assert.fail("{0} line {1}: shiftBits<16","words.h",67);
      {
        short u, carry=(short)((int)0xFFFF << (int)(16-shiftBits));
        if (shiftBits!=0)
          for (int i=n; i>0; i--)
        {
          u = r[rstart+i-1];
          r[rstart+i-1] = (short)(((((int)u)&0xFFFF) >> (int)shiftBits) | (((int)carry)&0xFFFF));
          carry = (short)((((int)u)&0xFFFF) << (int)(16-shiftBits));
        }
        return carry;
      }
    }



    private static void ShiftWordsLeftByWords(short[] r, int rstart, int n, int shiftWords) {
      shiftWords = Math.min(shiftWords, n);
      if (shiftWords!=0)
      {
        for (int i=n-1; i>=shiftWords; i--)
          r[rstart+i] = r[rstart+i-shiftWords];
        SetWords(r,rstart, (short)0, shiftWords);
      }
    }

    private static void ShiftWordsRightByWords(short[] r, int rstart, int n, int shiftWords) {
      shiftWords = Math.min(shiftWords, n);
      if (shiftWords!=0)
      {
        for (int i=0; i+shiftWords<n; i++)
          r[rstart+i] = r[rstart+i+shiftWords];
        SetWords(r,(int)(rstart+n-shiftWords), (short)0, shiftWords);
      }
    }
    
    private static void ShiftWordsRightByWordsSignExtend(short[] r, int rstart, int n, int shiftWords) {
      shiftWords = Math.min(shiftWords, n);
      if (shiftWords!=0)
      {
        for (int i=0; i+shiftWords<n; i++)
          r[rstart+i] = r[rstart+i+shiftWords];
        SetWords(r,(int)(rstart+n-shiftWords), ((short)0xFFFF), shiftWords);
      }
    }
    
    private static int Compare( short[] A, int astart, short[] B, int bstart, int N) {
      while ((N--)!=0){
        int an=(((int)A[astart+N])&0xFFFF);
        int bn=(((int)B[bstart+N])&0xFFFF);
        if (an>bn)
          return 1;
        else if (an<bn)
          return -1;
      }
      return 0;
    }
    private static int Increment(short[] A, int Astart, int N, short B) {
      {
        //Debugif(!(N!=0))Assert.fail("{0} line {1}: N","integer.cpp",63);
        short tmp = A[Astart];
        A[Astart] = (short)(tmp+B);
        if ((((int)A[Astart])&0xFFFF) >= (((int)tmp)&0xFFFF))
          return 0;
        for (int i=1; i<N; i++)
          if (++A[Astart+i]!=0)
            return 0;
        return 1;
      }
    }
    private static int Decrement(short[] A, int Astart, int N, short B) {
      //Debugif(!(N!=0))Assert.fail("{0} line {1}: N","integer.cpp",76);
      {
        short tmp = A[Astart];
        A[Astart] = (short)(tmp-B);
        if ((((int)A[Astart])&0xFFFF) <= (((int)tmp)&0xFFFF))
          return 0;
        for (int i=1; i<N; i++)
          if (A[Astart+i]--!=0)
            return 0;
        return 1;
      }
    }


    private static void TwosComplement(short[] A, int Astart, int N) {
      Decrement(A, Astart, N, (short)1);
      for (int i=0; i<N; i++)
        A[Astart+i] = ((short)(~A[Astart+i]));
    }

    private static int Decrement(short[] A, int N) {
      return Decrement(A,N,(short)1);
    }
    private static int Decrement(short[] A, int N, short B) {
      //Debugif(!(N!=0))Assert.fail("{0} line {1}: N","integer.cpp",76);
      {
        short tmp = A[0];
        A[0] = (short)(tmp-B);
        if ((((int)A[0])&0xFFFF) <= (((int)tmp)&0xFFFF))
          return 0;
        for (int i=1; i<N; i++)
          if (A[i]--!=0)
            return 0;
        return 1;
      }
    }
    private static void TwosComplement(short[] A, int N) {
      Decrement(A, N);
      for (int i=0; i<N; i++)
        A[i] = ((short)(~A[i]));
    }
    
    private static int Add(
      short[] C, int cstart,
      short[] A, int astart,
      short[] B, int bstart, int N) {
      //Debugif(!(N%2 == 0))Assert.fail("{0} line {1}: N%2 == 0","integer.cpp",799);
      {

        int u;
        u = 0;
        for (int i=0; i<N; i+=2)
        {
          u = (((int)A[astart+i])&0xFFFF) + (((int)B[bstart+i])&0xFFFF) + (short)(u>>16);
          C[cstart+i] = (short)(u);
          u = (((int)A[astart+i+1])&0xFFFF) + (((int)B[bstart+i+1])&0xFFFF) + (short)(u>>16);
          C[cstart+i+1] = (short)(u);
        }
        return (((int)u>>16)&0xFFFF);
      }
    }

    private static int Subtract(
      short[] C, int cstart,
      short[] A, int astart,
      short[] B, int bstart, int N) {
      //Debugif(!(N%2 == 0))Assert.fail("{0} line {1}: N%2 == 0","integer.cpp",799);
      {

        int u;
        u = 0;
        for (int i=0; i<N; i+=2)
        {
          u = (((int)A[astart+i])&0xFFFF) - (((int)B[bstart+i])&0xFFFF) - (int)((u>>31)&1);
          C[cstart+i] = (short)(u);
          u = (((int)A[astart+i+1])&0xFFFF) - (((int)B[bstart+i+1])&0xFFFF) - (int)((u>>31)&1);
          C[cstart+i+1] = (short)(u);
        }
        return (int)((u>>31)&1);
      }
    }

    
    private static short LinearMultiply(short[] C, int cstart,
                                        short[] A, int astart, short B, int N) {
      {
        short carry=0;
        int Bint=(((int)B)&0xFFFF);
        for(int i=0; i<N; i++)
        {
          int p;
          p = (((int)A[astart+i])&0xFFFF)*Bint;
          p = p + (((int)carry)&0xFFFF);
          C[cstart+i] = (short)(p);
          carry = (short)(p>>16);
        }
        return carry;
      }
    }
    //-----------------------------
    //  Baseline Square
    //-----------------------------

    

    private static void Baseline_Square2(short[] R, int rstart, short[] A, int astart) {
      {
        int p; short c; int d; int e;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart])&0xFFFF); R[rstart] = (short)(p); e = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+1])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+2*2 -3] = c;
        p = (((int)A[astart+2 -1])&0xFFFF)*(((int)A[astart+2 -1])&0xFFFF);
        p += e; R[rstart+2*2 -2] = (short)(p); R[rstart+2*2 -1] = (short)(p>>16);
      }
    }

    private static void Baseline_Square4(short[] R, int rstart, short[] A, int astart) {
      {
        int p; short c; int d; int e;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart])&0xFFFF); R[rstart] = (short)(p); e = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+1])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+1] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+2])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+2] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+3])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+3] = c;
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+3])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+4] = c;
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+3])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+2*4 -3] = c;
        p = (((int)A[astart+4 -1])&0xFFFF)*(((int)A[astart+4 -1])&0xFFFF);
        p += e; R[rstart+2*4 -2] = (short)(p); R[rstart+2*4 -1] = (short)(p>>16);
      }
    }

    private static void Baseline_Square8(short[] R, int rstart, short[] A, int astart) {
      {
        int p; short c; int d; int e;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart])&0xFFFF); R[rstart] = (short)(p); e = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+1])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+1] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+2])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+2] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+3])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+3] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+4])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+4] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+5])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+5] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+6])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+6] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+7])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+7] = c;
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+7])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+8] = c;
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+7])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+9] = c;
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+7])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+5])&0xFFFF)*(((int)A[astart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+10] = c;
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+7])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)A[astart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+11] = c;
        p = (((int)A[astart+5])&0xFFFF)*(((int)A[astart+7])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+6])&0xFFFF)*(((int)A[astart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+12] = c;
        p = (((int)A[astart+6])&0xFFFF)*(((int)A[astart+7])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+2*8 -3] = c;
        p = (((int)A[astart+8 -1])&0xFFFF)*(((int)A[astart+8 -1])&0xFFFF);
        p += e; R[rstart+2*8 -2] = (short)(p); R[rstart+2*8 -1] = (short)(p>>16);
      }
    }
    private static void Baseline_Square16(short[] R, int rstart, short[] A, int astart) {
      {
        int p; short c; int d; int e;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart])&0xFFFF); R[rstart] = (short)(p); e = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+1])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+1] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+2])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+2] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+3])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+3] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+4])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+4] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+5])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+5] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+6])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+6] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+7])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+7] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+8])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+8] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+9])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+9] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+10])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+5])&0xFFFF)*(((int)A[astart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+10] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+11])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)A[astart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+11] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+12])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)A[astart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+6])&0xFFFF)*(((int)A[astart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+12] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+13])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)A[astart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)A[astart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+13] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+14])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)A[astart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)A[astart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+7])&0xFFFF)*(((int)A[astart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+14] = c;
        p = (((int)A[astart])&0xFFFF)*(((int)A[astart+15])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)A[astart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)A[astart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)A[astart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+15] = c;
        p = (((int)A[astart+1])&0xFFFF)*(((int)A[astart+15])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)A[astart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)A[astart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)A[astart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+8])&0xFFFF)*(((int)A[astart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+16] = c;
        p = (((int)A[astart+2])&0xFFFF)*(((int)A[astart+15])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)A[astart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)A[astart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)A[astart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)A[astart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+17] = c;
        p = (((int)A[astart+3])&0xFFFF)*(((int)A[astart+15])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)A[astart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)A[astart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)A[astart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)A[astart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+9])&0xFFFF)*(((int)A[astart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+18] = c;
        p = (((int)A[astart+4])&0xFFFF)*(((int)A[astart+15])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)A[astart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)A[astart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)A[astart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)A[astart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)A[astart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+19] = c;
        p = (((int)A[astart+5])&0xFFFF)*(((int)A[astart+15])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)A[astart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)A[astart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)A[astart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)A[astart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+10])&0xFFFF)*(((int)A[astart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+20] = c;
        p = (((int)A[astart+6])&0xFFFF)*(((int)A[astart+15])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)A[astart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)A[astart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)A[astart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)A[astart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+21] = c;
        p = (((int)A[astart+7])&0xFFFF)*(((int)A[astart+15])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)A[astart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)A[astart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)A[astart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+11])&0xFFFF)*(((int)A[astart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+22] = c;
        p = (((int)A[astart+8])&0xFFFF)*(((int)A[astart+15])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)A[astart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)A[astart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)A[astart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+23] = c;
        p = (((int)A[astart+9])&0xFFFF)*(((int)A[astart+15])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)A[astart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)A[astart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+12])&0xFFFF)*(((int)A[astart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+24] = c;
        p = (((int)A[astart+10])&0xFFFF)*(((int)A[astart+15])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)A[astart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)A[astart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+25] = c;
        p = (((int)A[astart+11])&0xFFFF)*(((int)A[astart+15])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)A[astart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+13])&0xFFFF)*(((int)A[astart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+26] = c;
        p = (((int)A[astart+12])&0xFFFF)*(((int)A[astart+15])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)A[astart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+27] = c;
        p = (((int)A[astart+13])&0xFFFF)*(((int)A[astart+15])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1;
        p = (((int)A[astart+14])&0xFFFF)*(((int)A[astart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+28] = c;
        p = (((int)A[astart+14])&0xFFFF)*(((int)A[astart+15])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); d = (int)((d<<1) + (((int)c>>15)&1)); c<<=1; e = e + (((int)c)&0xFFFF); c = (short)(e); e = d + (((int)e>>16)&0xFFFF); R[rstart+2*16 -3] = c;
        p = (((int)A[astart+16 -1])&0xFFFF)*(((int)A[astart+16 -1])&0xFFFF);
        p += e; R[rstart+2*16 -2] = (short)(p); R[rstart+2*16 -1] = (short)(p>>16);
      }
    }
    
    //---------------------
    //  Baseline multiply
    //---------------------
    

    private static void Baseline_Multiply2(short[] R, int rstart, short[] A, int astart, short[] B, int bstart) {
      {
        int p; short c; int d;
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); R[rstart] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+1] = c;
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p += d; R[rstart+1 +1] = (short)(p); R[rstart+1 +2] = (short)(p>>16);
      }
    }

    private static void Baseline_Multiply4(short[] R, int rstart, short[] A, int astart, short[] B, int bstart) {
      {
        int p; short c; int d;
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); R[rstart] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+1] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+2] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+3] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+4] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+5] = c;
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p += d; R[rstart+5 +1] = (short)(p); R[rstart+5 +2] = (short)(p>>16);
      }
    }

    private static void Baseline_Multiply8(short[] R, int rstart, short[] A, int astart, short[] B, int bstart) {
      {
        int p; short c; int d;
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); R[rstart] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+1] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+2] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+3] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+4] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+5] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+6] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+7] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+8] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+9] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+10] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+11] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+12] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+13] = c;
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p += d; R[rstart+13 +1] = (short)(p); R[rstart+13 +2] = (short)(p>>16);
      }
    }
    private static void Baseline_Multiply16(short[] R, int rstart, short[] A, int astart, short[] B, int bstart) {
      {
        int p; short c; int d;
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart])&0xFFFF); c = (short)(p); d = (((int)p>>16)&0xFFFF); R[rstart] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+1] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+2] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+3] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+4] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+5] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+6] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+7] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+8] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+9] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+10] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+11] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+12] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+13] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+14] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+15] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+1])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart+1])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+16] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+2])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart+2])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+17] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+3])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart+3])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+18] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+4])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart+4])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+19] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+5])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart+5])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+20] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+6])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart+6])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+21] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+7])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart+7])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+22] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+8])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart+8])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+23] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+9])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart+9])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+24] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+10])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart+10])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+25] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+11])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart+11])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+26] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+12])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart+12])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+27] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+13])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart+13])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+28] = c; c = (short)(d); d = (((int)d>>16)&0xFFFF);
        p = (((int)A[astart+14])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF);
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart+14])&0xFFFF);
        p = p + (((int)c)&0xFFFF); c = (short)(p); d = d + (((int)p>>16)&0xFFFF); R[rstart+29] = c;
        p = (((int)A[astart+15])&0xFFFF)*(((int)B[bstart+15])&0xFFFF);
        p += d; R[rstart+30] = (short)(p); R[rstart+31] = (short)(p>>16);
      }
    }
    
    private static final int s_recursionLimit=16;
    
    private static void RecursiveMultiply(short[] Rarr,
                                          int Rstart,
                                          short[] Tarr,
                                          int Tstart, short[] Aarr, int Astart,
                                          short[] Barr, int Bstart, int N) {
      //Debugif(!(N>=2 && N%2==0))Assert.fail("{0} line {1}: N>=2 && N%2==0","integer.cpp",2066);
      if (N <= s_recursionLimit){
        N>>=2;
        switch(N){
          case 0:
            Baseline_Multiply2(Rarr, Rstart, Aarr, Astart, Barr, Bstart);
            break;
          case 1:
            Baseline_Multiply4(Rarr, Rstart, Aarr, Astart, Barr, Bstart);
            break;
          case 2:
            Baseline_Multiply8(Rarr, Rstart, Aarr, Astart, Barr, Bstart);
            break;
          case 4:
            Baseline_Multiply16(Rarr, Rstart, Aarr, Astart, Barr, Bstart);
            break;
          default:
            throw new IllegalStateException();
        }
      }
      else
      {
        int N2 = N/2;
        int AN2 = Compare(Aarr, Astart, Aarr, (int)(Astart+N2), N2) > 0 ? 0 : N2;
        Subtract(Rarr, Rstart, Aarr, (int)(Astart+AN2), Aarr, (int)(Astart+(N2 ^ AN2)), N2);
        int BN2 = Compare(Barr, Bstart, Barr, (int)(Bstart+N2), N2) > 0 ? 0 : N2;
        Subtract(Rarr, ((int)(Rstart+N2)), Barr, (int)(Bstart + BN2), Barr, (int)(Bstart + (N2 ^ BN2)), N2);
        RecursiveMultiply(Rarr, (int)(Rstart+N), Tarr, (int)(Tstart+N), Aarr, (int)(Astart+N2), Barr, (int)(Bstart+N2), N2);
        RecursiveMultiply(Tarr, Tstart, Tarr, (int)(Tstart+N), Rarr, Rstart, Rarr, (int)(Rstart+N2), N2);
        RecursiveMultiply(Rarr, Rstart, Tarr, (int)(Tstart+N), Aarr, Astart, Barr, Bstart,N2);
        int c2 = Add(Rarr, (int)(Rstart+N), Rarr, (int)(Rstart+N), Rarr, ((int)(Rstart+N2)), N2);
        int c3 = c2;
        c2 += Add(Rarr, ((int)(Rstart+N2)), Rarr, (int)(Rstart+N), Rarr, (Rstart), N2);
        c3 += Add(Rarr, (int)(Rstart+N), Rarr, (int)(Rstart+N), Rarr, (int)(Rstart+N+N2), N2);
        if (AN2 == BN2)
          c3 -= Subtract(Rarr, ((int)(Rstart+N2)), Rarr, ((int)(Rstart+N2)), Tarr, Tstart, N);
        else
          c3 += Add(Rarr, ((int)(Rstart+N2)), Rarr, ((int)(Rstart+N2)), Tarr, Tstart, N);

        c3 += Increment(Rarr, (int)(Rstart+N), N2, (short)c2);
        Increment(Rarr, (int)(Rstart+N+N2), N2, (short)c3);
      }
    }

    private static void RecursiveSquare(short[] Rarr,
                                        int Rstart,
                                        short[] Tarr,
                                        int Tstart, short[] Aarr, int Astart, int N) {
      //Debugif(!(N!=0 && N%2==0))Assert.fail("{0} line {1}: N && N%2==0","integer.cpp",2108);

      if (N <= s_recursionLimit){
        N>>=2;
        switch(N){
          case 0:
            Baseline_Square2(Rarr, Rstart, Aarr, Astart);
            break;
          case 1:
            Baseline_Square4(Rarr, Rstart, Aarr, Astart);
            break;
          case 2:
            Baseline_Square8(Rarr, Rstart, Aarr, Astart);
            break;
          case 4:
            Baseline_Square16(Rarr, Rstart, Aarr, Astart);
            break;
          default:
            throw new IllegalStateException();
        }
      }
      else
      {
        int N2 = N/2;

        RecursiveSquare(Rarr, Rstart,  Tarr, (int)(Tstart+N), Aarr,Astart, N2);
        RecursiveSquare(Rarr, (int)(Rstart+N),  Tarr, (int)(Tstart+N), Aarr, (int)(Astart+N2), N2);
        RecursiveMultiply(Tarr, Tstart, Tarr, (int)(Tstart+N),
                          Aarr, Astart, Aarr, (int)(Astart+N2), N2);
        
        int carry = Add(Rarr, (int)(Rstart+N2), Rarr, (int)(Rstart+N2), Tarr, Tstart, N);
        carry += Add(Rarr, (int)(Rstart+N2), Rarr, (int)(Rstart+N2), Tarr, Tstart, N);
        
        Increment(Rarr, (int)(Rstart+N+N2), N2, (short)carry);
      }
    }
    
    private static void Multiply(short[] Rarr,
                                 int Rstart,
                                 short[] Tarr,
                                 int Tstart, short[] Aarr, int Astart,
                                 short[] Barr, int Bstart, int N) {
      RecursiveMultiply(Rarr, Rstart, Tarr, Tstart, Aarr, Astart,
                        Barr, Bstart, N);
    }

    private static void Square(short[] Rarr,
                               int Rstart,
                               short[] Tarr,
                               int Tstart, short[] Aarr, int Astart, int N) {
      RecursiveSquare(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, N);
    }
    
    private static void AsymmetricMultiply(
      short[] Rarr,
      int Rstart,
      short[] Tarr,
      int Tstart, short[] Aarr, int Astart, int NA, short[] Barr, int Bstart, int NB) {
      if (NA == NB)
      {
        if (Astart==Bstart && Aarr == Barr)
          Square(Rarr,Rstart, Tarr,Tstart, Aarr,Astart, NA);
        else if(NA==2)
          Baseline_Multiply2(Rarr,Rstart, Aarr,Astart, Barr,Bstart);
        else
          Multiply(Rarr,Rstart, Tarr,Tstart, Aarr,Astart, Barr,Bstart, NA);

        return;
      }

      if (NA > NB)
      {
        short[] tmp1=Aarr;Aarr=Barr;Barr=tmp1;
        int tmp3=Astart;Astart=Bstart;Bstart=tmp3;
        int tmp2=NA;NA=NB;NB=tmp2;
      }

      if (NA==2 && Aarr[Astart+1]==0)
      {
        switch (Aarr[Astart])
        {
          case 0:
            SetWords(Rarr,Rstart, (short)0, NB+2);
            return;
          case 1:
            System.arraycopy(Barr,Bstart,Rarr,Rstart,(int)NB);
            Rarr[Rstart+NB] = Rarr[Rstart+NB+1] = 0;
            return;
          default:
            Rarr[Rstart+NB] = LinearMultiply(Rarr, Rstart, Barr, Bstart, Aarr[Astart], NB);
            Rarr[Rstart+NB+1] = 0;
            return;
        }
      }
      
      if(NA==2){
        int i;
        if ((NB/2)%2 == 0)
        {
          Baseline_Multiply2(Rarr,Rstart, Aarr,Astart, Barr,Bstart);
          System.arraycopy(Rarr, (int)(Rstart+2),Tarr, (int)(Tstart+2*2), (int)2);
          for (i=2*2; i<NB; i+=2*2)
            Baseline_Multiply2(Tarr,(int)(Tstart+2+i), Aarr, Astart, Barr, (int)(Bstart+i));
          for (i=2; i<NB; i+=2*2)
            Baseline_Multiply2(Rarr, (int)(Rstart+i), Aarr,Astart, Barr, (int)(Bstart+i));
        }
        else
        {
          for (i=0; i<NB; i+=2*2)
            Baseline_Multiply2(Rarr, (int)(Rstart+i), Aarr, Astart, Barr, (int)(Bstart+i));
          for (i=2; i<NB; i+=2*2)
            Baseline_Multiply2(Tarr,(int)(Tstart+2+i), Aarr, Astart, Barr, (int)(Bstart+i));
        }
      } else {

        int i;
        if ((NB/NA)%2 == 0)
        {
          Multiply(Rarr,Rstart, Tarr,Tstart, Aarr,Astart, Barr,Bstart, NA);
          System.arraycopy(Rarr, (int)(Rstart+NA),Tarr, (int)(Tstart+2*NA), (int)NA);
          for (i=2*NA; i<NB; i+=2*NA)
            Multiply(Tarr,(int)(Tstart+NA+i), Tarr,Tstart, Aarr, Astart, Barr, (int)(Bstart+i), NA);
          for (i=NA; i<NB; i+=2*NA)
            Multiply(Rarr, (int)(Rstart+i), Tarr,Tstart, Aarr,Astart, Barr, (int)(Bstart+i), NA);
        }
        else
        {
          for (i=0; i<NB; i+=2*NA)
            Multiply(Rarr, (int)(Rstart+i), Tarr,Tstart, Aarr, Astart, Barr, (int)(Bstart+i), NA);
          for (i=NA; i<NB; i+=2*NA)
            Multiply(Tarr,(int)(Tstart+NA+i), Tarr,Tstart, Aarr, Astart, Barr, (int)(Bstart+i), NA);
        }
      }

      if (Add(Rarr, (int)(Rstart+NA), Rarr, (int)(Rstart+NA), Tarr, (int)(Tstart+2*NA), NB-NA)!=0)
        Increment(Rarr, (int)(Rstart+NB), NA, (short)1);
    }

    private static int MakeUint(short first, short second) {
      return ((int)((((int)first)&0xFFFF)|((int)(second)<<16)));
    }
    
    private static short GetLowHalf(int val) {
      return ((short)(val&0xFFFF));
    }

    private static short GetHighHalf(int val) {
      return ((short)((val>>16)&0xFFFF));
    }

    private static short GetHighHalfAsBorrow(int val) {
      return ((short)(0-((val>>16)&0xFFFF)));
    }
    
    private static int BitPrecision(short numberValue) {
      if (numberValue==0)
        return 0;
      int ivalue=(((int)numberValue)&0xFFFF);
      int l=0, h=16;
      while (h-l > 1)
      {
        int middle = (l+h)/2;
        if ((ivalue >> (int)middle)!=0)
          l = middle;
        else
          h = middle;
      }

      return h;
    }

    private static int BitPrecisionInt(int numberValue) {
      if (numberValue==0)
        return 0;
      int l=0, h=32;
      while (h-l > 1)
      {
        int middle = (l+h)/2;
        if ((numberValue >> (int)middle)!=0)
          l = middle;
        else
          h = middle;
      }

      return h;
    }

    private static short DivideUnsigned(int x, short y) {
      {
        int iy=(((int)y)&0xFFFF);
        if((x>>31)==0){
          // x is already nonnegative
          return (short)(((int)x/iy)&0xFFFF);
        } else {
          // convert to long to prevent dividing
          // a negative number
          long lx=((long)x)&0xFFFFFFFFL;
          lx=(lx/iy)&0xFFFFL;
          return (short)(((int)lx)&0xFFFF);
        }
      }
    }
    private static short RemainderUnsigned(int x, short y) {
      {
        int iy=(((int)y)&0xFFFF);
        if((x>>31)==0){
          // x is already nonnegative
          return (short)(((int)x%iy)&0xFFFF);
        } else {
          // convert to long to prevent dividing
          // a negative number
          long lx=((long)x)&0xFFFFFFFFL;
          lx=(lx%iy)&0xFFFFL;
          return (short)(((int)lx)&0xFFFF);
        }
      }
    }
    
    private static short DivideThreeWordsByTwo(short[] A, int Astart, short B0, short B1) {
      //Debugif(!(A[2] < B1 || (A[2]==B1 && A[1] < B0)))Assert.fail("{0} line {1}: A[2] < B1 || (A[2]==B1 && A[1] < B0)","integer.cpp",360);
      short Q;
      {
        if ((short)(B1+1) == 0)
          Q = A[Astart+2];
        else if ((((int)B1)&0xFFFF) > 0)
          Q = DivideUnsigned(MakeUint(A[Astart+1], A[Astart+2]),(short)(((int)B1+1)&0xFFFF));
        else
          Q = DivideUnsigned(MakeUint(A[Astart], A[Astart+1]),B0);

        int Qint=(((int)Q)&0xFFFF);
        int B0int=(((int)B0)&0xFFFF);
        int B1int=(((int)B1)&0xFFFF);
        int p = B0int*Qint;
        int u = (((int)A[Astart])&0xFFFF) - (((int)GetLowHalf(p))&0xFFFF);
        A[Astart] = GetLowHalf(u);
        u = (((int)A[Astart+1])&0xFFFF) - (((int)GetHighHalf(p))&0xFFFF) -
          (((int)GetHighHalfAsBorrow(u))&0xFFFF) - (B1int*Qint);
        A[Astart+1] = GetLowHalf(u);
        A[Astart+2] += GetHighHalf(u);


        while (A[Astart+2]!=0 ||
               (((int)A[Astart+1])&0xFFFF) > (((int)B1)&0xFFFF) ||
               (A[Astart+1]==B1 && (((int)A[Astart])&0xFFFF)>=(((int)B0)&0xFFFF)))
        {
          u = (((int)A[Astart])&0xFFFF) - B0int;
          A[Astart] = GetLowHalf(u);
          u = (((int)A[Astart+1])&0xFFFF) - B1int - (((int)GetHighHalfAsBorrow(u))&0xFFFF);
          A[Astart+1] = GetLowHalf(u);
          A[Astart+2] += GetHighHalf(u);
          Q++;
          

        }
      }
      return Q;
    }

    private static int DivideFourWordsByTwo(short[] T, int Al, int Ah, int B) {
      if (B==0)
        return MakeUint(GetLowHalf(Al), GetHighHalf(Ah));
      else
      {
        short[] Q=new short[2];
        T[0] = GetLowHalf(Al);
        T[1] = GetHighHalf(Al);
        T[2] = GetLowHalf(Ah);
        T[3] = GetHighHalf(Ah);
        Q[1] = DivideThreeWordsByTwo(T,1, GetLowHalf(B), GetHighHalf(B));
        Q[0] = DivideThreeWordsByTwo(T,0, GetLowHalf(B), GetHighHalf(B));
        return MakeUint(Q[0], Q[1]);
      }
    }
    
    private static void AtomicDivide(short[] Q, int Qstart, short[] A, int Astart, short[] B, int Bstart) {
      short[] T=new short[4];
      int q = DivideFourWordsByTwo(T, MakeUint(A[Astart], A[Astart+1]),
                                   MakeUint(A[Astart+2], A[Astart+3]),
                                   MakeUint(B[Bstart], B[Bstart+1]));
      Q[Qstart] = GetLowHalf(q);
      Q[Qstart+1] = GetHighHalf(q);
    }
    
    

    // for use by Divide(), corrects the underestimated quotient {Q1,Q0}
    private static void CorrectQuotientEstimate(
      short[] Rarr,
      int Rstart,
      short[] Tarr, int Tstart,
      short[] Qarr, int Qstart,
      short[] Barr, int Bstart, int N) {
      

      {
        if(N==2)
          Baseline_Multiply2(Tarr, Tstart, Qarr,Qstart, Barr,Bstart);
        else
          AsymmetricMultiply(Tarr, Tstart, Tarr, (int)(Tstart+(N+2)), Qarr,Qstart, 2, Barr,Bstart, N);
        Subtract(Rarr,Rstart,Rarr,Rstart, Tarr,Tstart, N+2);

        while (Rarr[Rstart+N]!=0 || Compare(Rarr,Rstart, Barr,Bstart, N) >= 0)
        {
          Rarr[Rstart+N] -= (short)Subtract(Rarr,Rstart, Rarr,Rstart, Barr,Bstart, N);
          Qarr[Qstart]++;
          Qarr[Qstart+1] += (short)((Qarr[Qstart]==0) ? 1 : 0);
        }
      }
    }
    
    private static void Divide(
      short[] Rarr, int Rstart, // remainder
      short[] Qarr, int Qstart, // quotient
      short[] TA, int Tstart, // scratch space
      short[] Aarr, int Astart, int NAint, // dividend
      short[] Barr, int Bstart, int NBint  // divisor
     ) {
      // set up temporary work space
      int NA=(int)NAint;
      int NB=(int)NBint;
      
      short[] TBarr=TA;
      short[] TParr=TA;
      int TBstart=(int)(Tstart+(NA+2));
      int TPstart=(int)(Tstart+(NA+2+NB));
      {
        // copy B into TB and normalize it so that TB has highest bit set to 1
        int shiftWords = (short)(Barr[Bstart+NB-1]==0 ? 1 : 0);
        TBarr[TBstart] = TBarr[TBstart+NB-1] = 0;
        System.arraycopy(Barr,Bstart,TBarr,(int)(TBstart+shiftWords),NB-shiftWords);
        short shiftBits = (short)((short)16 - BitPrecision(TBarr[TBstart+NB-1]));
        ShiftWordsLeftByBits(
          TBarr,TBstart,
          NB, shiftBits);
        // copy A into TA and normalize it
        TA[0] = TA[NA] = TA[NA+1] = 0;
        System.arraycopy(Aarr,Astart,TA,(int)(Tstart+shiftWords),NAint);
        ShiftWordsLeftByBits(
          TA, Tstart, NA+2, shiftBits);

        if (TA[Tstart+NA+1]==0 && (((int)TA[Tstart+NA])&0xFFFF) <= 1)
        {
          Qarr[Qstart+NA-NB+1] = Qarr[Qstart+NA-NB] = 0;
          while (TA[NA]!=0 || Compare(TA,(int)(Tstart+NA-NB),
                                      TBarr,TBstart, NB) >= 0)
          {
            TA[NA] -= (short)Subtract(TA,(int)(Tstart+NA-NB),
                                      TA,(int)(Tstart+NA-NB),
                                      TBarr,TBstart, NB);
            Qarr[Qstart+NA-NB]++;
          }
        }
        else
        {
          NA+=2;
        }

        short[] BT=new short[2];
        BT[0] = (short)(TBarr[TBstart+NB-2] + (short)1);
        BT[1] = (short)(TBarr[TBstart+NB-1] + (short)(BT[0]==0 ? 1 : 0));

        // start reducing TA mod TB, 2 words at a time
        for (int i=NA-2; i>=NB; i-=2)
        {
          //"T",TA);
          AtomicDivide(Qarr,(int)(Qstart+i-NB), TA, (int)(Tstart+i-2), BT,0);
          //"Ta",TA);
          CorrectQuotientEstimate(TA, (int)(Tstart+i-NB),
                                  TParr,TPstart, Qarr,(int)(Qstart+(i-NB)), TBarr,TBstart, NB);
        }
        if(Rarr!=null){ // If the remainder is non-null
          // copy TA into R, and denormalize it
          System.arraycopy(TA,(int)(Tstart+shiftWords),Rarr,Rstart,NB);
          ShiftWordsRightByBits(Rarr,Rstart, NB, shiftBits);
        }
      }
    }

    private static int[] RoundupSizeTable=new int[] {2, 2, 2, 4, 4, 8, 8, 8, 8};

    private static int RoundupSize(int n) {
      if (n<=8)
        return RoundupSizeTable[n];
      else if (n<=16)
        return 16;
      else if (n<=32)
        return 32;
      else if (n<=64)
        return 64;
      else return (int)(1) << (int)BitPrecisionInt(n-1);
    }
    
    /**
     * BigInteger object for the number zero.
     */
    public static final BigInteger ZERO=new BigInteger(0);
    /**
     * BigInteger object for the number one.
     */
    public static final BigInteger ONE=new BigInteger(1);
    /**
     * BigInteger object for the number ten.
     */
    public static final BigInteger TEN=new BigInteger(10);
    
    boolean negative;
    int wordCount=-1;
    short[] reg;
    /**
     * Initializes a BigInteger object set to zero.
     */
    private BigInteger(){
      this.reg=new short[]{0,0};
      this.wordCount=0;
    }
    /**
     * Initializes a BigInteger object from an array of bytes.
     * @param bytes A byte[] object.
     */
    public BigInteger(byte[] bytes){
      if(bytes==null)
        throw new NullPointerException("bytes");
      if(bytes.length==0){
        this.reg=new short[]{0,0};
        this.wordCount=0;
      } else {
        this.reg=new short[RoundupSize(((int)bytes.length+1)/2)];
        int jIndex=(LittleEndianSerialize) ? bytes.length-1 : 0;
        boolean negative=((bytes[jIndex])&0x80)!=0;
        int j=0;
        for(int i=0;i<bytes.length;i+=2,j++){
          int index=(LittleEndianSerialize) ? i : bytes.length-1-i;
          int index2=(LittleEndianSerialize) ? i+1 : bytes.length-2-i;
          this.reg[j]=(short)(((int)bytes[index])&0xFF);
          if(index2>=0 && index2<bytes.length){
            this.reg[j]|=((short)(((short)bytes[index2])<<8));
          } else if(negative){
            // sign extend the last byte
            this.reg[j]|=((short)0xFF00);
          }
        }
        this.negative=negative;
        if(negative){
          for(;j<reg.length;j++){
            this.reg[j]=((short)0xFFFF); // sign extend remaining words
          }
          TwosComplement(this.reg,0,(int)this.reg.length);
        }
        this.wordCount=CalcWordCount();
      }
    }
    private BigInteger Allocate(int length) {
      this.reg=new short[RoundupSize(length)];
      SetWords(this.reg,1,(short)0,(int)(this.reg.length-1));
      // IsZero relies on the current state of reg
      this.negative=false;
      this.wordCount=0;
      return this;
    }
    
    private BigInteger(BigInteger t){
      this.reg=new short[RoundupSize((int)(t.wordCount))];
      System.arraycopy(t.reg,0,reg,0,Math.min(reg.length,t.reg.length));
      // IsZero relies on the current state of reg
      this.negative=(t.signum()<0);
      this.wordCount=CalcWordCount();
    }
    
    private static short[] CleanGrow(short[] a, int size) {
      if(size>a.length){
        short[] newa=new short[size];
        System.arraycopy(a,0,newa,0,a.length);
        return newa;
      }
      return a;
    }

    private void SetBitInternal(int n, boolean value) {
      if (value)
      {
        this.reg=CleanGrow(reg,RoundupSize(BitsToWords(n+1)));
        this.reg[n/16] |= (short)((short)(1) << (int)(n&0xf));
        this.wordCount=CalcWordCount();
      }
      else
      {
        if (n/16 < reg.length)
          reg[n/16] &= ((short)(~((short)(1) << (int)(n%16))));
        this.wordCount=CalcWordCount();
      }
    }
    
    /**
     * 
     * @param index A 32-bit unsigned integer.
     */
    public boolean testBit(int index) {
      if(index<0)throw new IllegalArgumentException("index");
      if(this.signum()<0){
        int tcindex=0;
        int wordpos=index/16;
        if(wordpos>=reg.length)return true;
        while(tcindex<wordpos && reg[tcindex]==0){
          tcindex++;
        }
        short tc;
        {
          tc=reg[wordpos];
          if(tcindex==wordpos)tc--;
          tc=(short)~tc;
        }
        return (boolean)(((tc >> (int)(index & 15)) & 1)!=0);
      } else {
        return this.GetUnsignedBit(index);
      }
    }
    
    /**
     * 
     * @param n A 32-bit unsigned integer.
     */
    private boolean GetUnsignedBit(int n) {
      if((n)<0)throw new IllegalArgumentException("n"+" not greater or equal to "+"0"+" ("+Long.toString((long)(long)(n))+")");
      if (n/16 >= reg.length)
        return false;
      else
        return (boolean)(((reg[n/16] >> (int)(n & 15)) & 1)!=0);
    }
    
    private BigInteger(int numberValue){
      int ut;
      {
        negative=(numberValue<0);
        if(numberValue==Integer.MIN_VALUE){
          reg=new short[2];
          reg[0]=0;
          reg[1]=(short)0x8000;
          wordCount=2;
        } else {
          ut=((numberValue<0) ? (int)-numberValue : (int)numberValue);
          reg=new short[2];
          reg[0]=(short)ut;
          reg[1]=(short)(ut>>16);
          wordCount=(reg[1]!=0 ? 2 : (reg[0]==0 ? 0 : 1));
        }
      }
    }
    /**
     * Returns a byte array of this object's value.
     * @return A byte array that represents the value of this object.
     */
    public byte[] toByteArray() {
      int sign=this.signum();
      if(sign==0){
        return new byte[]{0};
      } else if(sign>0){
        int byteCount=ByteCount();
        int bc=BitLength();
        int byteArrayLength=byteCount;
        if((bc&7)==0 && GetUnsignedBit(bc-1)){
          byteArrayLength++;
        }
        byte[] bytes=new byte[byteArrayLength];
        int j=0;
        for(int i=0;i<byteCount;i+=2,j++){
          int index=(LittleEndianSerialize) ? i : bytes.length-1-i;
          int index2=(LittleEndianSerialize) ? i+1 : bytes.length-2-i;
          bytes[index]=(byte)((reg[j])&0xff);
          if(index2>=0 && index2<byteArrayLength){
            bytes[index2]=(byte)((reg[j]>>8)&0xff);
          }
        }
        return bytes;
      } else {
        short[] regdata=new short[reg.length];
        System.arraycopy(reg,0,regdata,0,reg.length);
        TwosComplement(regdata,0,(int)regdata.length);
        int byteCount=regdata.length*2;
        for(int i=regdata.length-1;i>=0;i--){
          if(regdata[i]==((short)0xFFFF)){
            byteCount-=2;
          } else if((regdata[i]&0xFF80)==0xFF80){
            //signed first byte, 0xFF second
            byteCount-=1;
            break;
          } else if((regdata[i]&0x8000)==0x8000){
            //signed second byte
            break;
          } else {
            //unsigned second byte
            byteCount+=1;
            break;
          }
        }
        if(byteCount==0)byteCount=1;
        byte[] bytes=new byte[byteCount];
        bytes[(LittleEndianSerialize) ? bytes.length-1 : 0]=(byte)0xFF;
        byteCount=Math.min(byteCount,regdata.length*2);
        int j=0;
        for(int i=0;i<byteCount;i+=2,j++){
          int index=(LittleEndianSerialize) ? i : bytes.length-1-i;
          int index2=(LittleEndianSerialize) ? i+1 : bytes.length-2-i;
          bytes[index]=(byte)((regdata[j])&0xff);
          if(index2>=0 && index2<byteCount){
            bytes[index2]=(byte)((regdata[j]>>8)&0xff);
          }
        }
        return bytes;
      }
    }

    /**
     * 
     * @param n A 32-bit signed integer.
     */
    public BigInteger shiftLeft(int n) {
      if((n)<=Integer.MIN_VALUE)throw new IllegalArgumentException("n"+" not less than "+Long.toString((long)(long)(Integer.MIN_VALUE))+" ("+Long.toString((long)(long)(n))+")");
      if(n==0)return this;
      if(n<0)return this.shiftRight(-n);
      BigInteger ret=new BigInteger(this);
      int numWords = (int)(ret.wordCount);
      int shiftWords = (int)(n >> 4);
      int shiftBits = (int)(n & 15);
      int sign=ret.signum();
      ret.reg=CleanGrow(ret.reg,RoundupSize(numWords+BitsToWords((int)n)));
      if(sign<0){ TwosComplement(ret.reg,(int)(ret.reg.length)); }
      ShiftWordsLeftByWords(ret.reg,0, numWords + shiftWords, shiftWords);
      ShiftWordsLeftByBits(ret.reg,(int)shiftWords, numWords+BitsToWords(shiftBits), shiftBits);
      if(sign<0){ TwosComplement(ret.reg,(int)(ret.reg.length)); }
      ret.wordCount=ret.CalcWordCount();
      return ret;
    }
    /**
     * 
     * @param n A 32-bit signed integer.
     */
    public BigInteger shiftRight(int n) {
      if((n)<=Integer.MIN_VALUE)throw new IllegalArgumentException("n"+" not less than "+Long.toString((long)(long)(Integer.MIN_VALUE))+" ("+Long.toString((long)(long)(n))+")");
      if(n==0)return this;
      if(n<0)return this.shiftLeft(-n);
      BigInteger ret=new BigInteger(this);
      int numWords = (int)(ret.wordCount);
      int shiftWords = (int)(n >> 4);
      int shiftBits = (int)(n & 15);
      if(ret.signum()<0){
        TwosComplement(ret.reg,(int)(ret.reg.length));
        ShiftWordsRightByWordsSignExtend(ret.reg,0, numWords, shiftWords);
        if (numWords > shiftWords)
          ShiftWordsRightByBitsSignExtend(ret.reg,0, numWords-shiftWords, shiftBits);
        TwosComplement(ret.reg,(int)(ret.reg.length));
      } else {
        ShiftWordsRightByWords(ret.reg,0, numWords, shiftWords);
        if (numWords > shiftWords)
          ShiftWordsRightByBits(ret.reg,0, numWords-shiftWords, shiftBits);
      }
      ret.wordCount=ret.CalcWordCount();
      return ret;
    }
    
    /**
     * 
     * @param numberValue A 64-bit signed integer.
     */
    public static BigInteger valueOf(long numberValue) {
      if(numberValue==0)return BigInteger.ZERO;
      if(numberValue==1)return BigInteger.ONE;
      BigInteger ret=new BigInteger();
      {
        ret.negative=(numberValue<0);
        ret.reg=new short[4];
        if(numberValue==Long.MIN_VALUE){
          ret.reg[0]=0;
          ret.reg[1]=0;
          ret.reg[2]=0;
          ret.reg[3]=(short)0x8000;
          ret.wordCount=4;
        } else {
          long ut=((numberValue<0) ? -numberValue : numberValue);
          ret.reg[0]=(short)(ut&0xFFFF);
          ret.reg[1]=(short)((ut>>16)&0xFFFF);
          ret.reg[2]=(short)((ut>>32)&0xFFFF);
          ret.reg[3]=(short)((ut>>48)&0xFFFF);
          // at this point, the word count can't
          // be 0 (the check for 0 was already done above)
          ret.wordCount=4;
          if((ut>>48)==0)ret.wordCount=3;
          if((ut>>32)==0)ret.wordCount=2;
          if((ut>>16)==0)ret.wordCount=1;
        }
      }
      return ret;
    }
    
    /**
     * 
     */
    public int intValue() {
      int c=(int)this.wordCount;
      if(c==0)return 0;
      if(c>2)throw new ArithmeticException();
      if(c==2 && (this.reg[1]&0x8000)!=0){
        if(((short)(this.reg[1]&(short)0x7FFF)|this.reg[0])==0 && this.negative){
          return Integer.MIN_VALUE;
        } else {
          throw new ArithmeticException();
        }
      } else {
        int vv=(((int)this.reg[0])&0xFFFF);
        if(c>1)vv |= (((int)this.reg[1])&0xFFFF)<<16;
        if(this.negative)vv=-vv;
        return vv;
      }
    }
    
    private boolean HasSmallValue() {
      int c=(int)this.wordCount;
      if(c>4)return false;
      if(c==4 && (this.reg[3]&0x8000)!=0){
        return (this.negative && this.reg[3]==((short)0x8000) &&
                this.reg[2]==0 &&
                this.reg[1]==0 &&
                this.reg[0]==0);
      }
      return true;
    }
    
    /**
     * 
     */
    public long longValue() {
      int count=(int)this.wordCount;
      if(count==0)return 0;
      if(count>4)throw new ArithmeticException();
      if(count==4 && (this.reg[3]&0x8000)!=0){
        if(this.negative && this.reg[3]==((short)0x8000) &&
           this.reg[2]==0 &&
           this.reg[1]==0 &&
           this.reg[0]==0){
          return Long.MIN_VALUE;
        } else {
          throw new ArithmeticException();
        }
      } else {
        long vv=(long)(((int)this.reg[0])&0xFFFF);
        if(count>1)vv |= (long)(((int)this.reg[1])&0xFFFF)<<16;
        if(count>2)vv |= (long)(((int)this.reg[2])&0xFFFF)<<32;
        if(count>3)vv |= (long)(((int)this.reg[3])&0xFFFF)<<48;
        if(this.negative)vv=-vv;
        return vv;
      }
    }
    
    private static BigInteger Power2(int e) {
      BigInteger r=new BigInteger().Allocate(BitsToWords((int)(e+1)));
      r.SetBitInternal((int)e,true); // NOTE: Will recalculate word count
      return r;
    }
    
    /**
     * 
     * @param power A BigInteger object.
     */
    public BigInteger pow(BigInteger power) {
      if((power)==null)throw new NullPointerException("power");
      if(power.signum()<0)throw new IllegalArgumentException("power is negative");
      BigInteger thisValue=this;
      if(power.equals(BigInteger.ZERO))
        return BigInteger.ONE; // however 0 to the power of 0 is undefined
      else if(power.equals(BigInteger.ONE))
        return this;
      else if(power.equals(BigInteger.valueOf(2)))
        return thisValue.multiply(thisValue);
      else if(power.equals(BigInteger.valueOf(3)))
        return (thisValue.multiply(thisValue)).multiply(thisValue);
      BigInteger r=BigInteger.ONE;
      while(power.signum()!=0){
        if(power.testBit(0)){
          r=(r.multiply(thisValue));
        }
        power=power.shiftRight(1);
        if(power.signum()!=0){
          thisValue=(thisValue.multiply(thisValue));
        }
      }
      return r;
    }

    /**
     * 
     * @param powerSmall A 32-bit signed integer.
     */
    public BigInteger pow(int powerSmall) {
      if(powerSmall<0)throw new IllegalArgumentException("power is negative");
      BigInteger thisValue=this;
      if(powerSmall==0)
        return BigInteger.ONE; // however 0 to the power of 0 is undefined
      else if(powerSmall==1)
        return this;
      else if(powerSmall==2)
        return thisValue.multiply(thisValue);
      else if(powerSmall==3)
        return (thisValue.multiply(thisValue)).multiply(thisValue);
      BigInteger r=BigInteger.ONE;
      while(powerSmall!=0){
        if((powerSmall&1)!=0){
          r=(r.multiply(thisValue));
        }
        powerSmall>>=1;
        if(powerSmall!=0){
          thisValue=(thisValue.multiply(thisValue));
        }
      }
      return r;
    }
    
    /**
     * 
     */
    public BigInteger negate() {
      BigInteger bigintRet=new BigInteger();
      bigintRet.reg=this.reg; // use the same reference;
      bigintRet.wordCount=this.wordCount;
      bigintRet.negative=(this.signum()>0);
      return bigintRet;
    }
    /**
     * 
     */
    public BigInteger abs() {
      return this.signum()>=0 ? this : this.negate();
    }
    static int BytePrecision(short numberValue)
    {
      if (numberValue==0)
        return 0;

      int l=0, h=8*2;

      while (h-l > 8)
      {
        int t = (l+h)/2;
        if ((numberValue >> t)!=0)
          l = t;
        else
          h = t;
      }

      return (int)(h/8);
    }
    
    /**
     * 
     */
    private int CalcWordCount() {
      return (int)CountWords(reg,reg.length);
    }

    /**
     * 
     */
    private int ByteCount() {
      int wc=this.wordCount;
      if(wc>0)
        return (int)((wc-1)*2+BytePrecision(reg[wc-1]));
      else
        return 0;
    }

    /**
     * 
     */
    private int BitLength() {
      int wc=this.wordCount;
      if(wc>0)
        return (int)((wc-1)*16+BitPrecision(reg[wc-1]));
      else
        return 0;
    }

    private static final String vec="0123456789ABCDEF";

    private static void ReverseChars(char[] chars, int offset, int length) {
      int half = length >> 1;
      int right = offset+length-1;
      for (int i = 0; i < half; i++, right--) {
        char value = chars[offset+i];
        chars[offset+i] = chars[right];
        chars[right] = value;
      }
    }
    private String SmallValueToString() {
      long value=longValue();
      if(value==Long.MIN_VALUE)
        return "-9223372036854775808";
      boolean neg=(value<0);
      char[] chars=new char[24];
      int count=0;
      if(neg){
        chars[0]='-';
        count++;
        value=-value;
      }
      while(value!=0){
        char digit=vec.charAt((int)(value%10));
        chars[count++]=digit;
        value/=10;
      }
      if(neg)
        ReverseChars(chars,1,count-1);
      else
        ReverseChars(chars,0,count);
      return new String(chars,0,count);
    }
    
    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      if (this.signum()==0)
        return "0";
      if(HasSmallValue()){
        return SmallValueToString();
      }
      short[] tempReg=new short[this.wordCount];
      System.arraycopy(this.reg,0,tempReg,0,tempReg.length);
      int wordCount=tempReg.length;
      while (wordCount!=0 && tempReg[wordCount-1]==0)
        wordCount--;
      int i=0;
      char[] s=new char[(this.BitLength() / 3 + 1)];
      while (wordCount!=0)
      {
        if(wordCount==1 && tempReg[0]>0 && tempReg[0]<10000){
          int rest=tempReg[0];
          while(rest!=0){
            s[i++]=vec.charAt(rest%10);
            rest/=10;
          }
          break;
        } else {
          int remainderSmall=FastDivideAndRemainder(tempReg,wordCount,(short)10000);
          // Recalculate word count
          while (wordCount!=0 && tempReg[wordCount-1]==0)
            wordCount--;
          for(int j=0;j<4;j++){
            s[i++]=vec.charAt((int)(remainderSmall%10));
            remainderSmall/=10;
          }
        }
      }
      ReverseChars(s,0,i);
      if (this.signum()<0){
        StringBuilder sb=new StringBuilder(i+1);
        sb.append('-');
        sb.append(s,0,(0)+(i));
        return sb.toString();
      } else {
        return new String(s,0,i);
      }

    }
    
    /**
     * Returns the greatest common divisor of two integers.
     * @param bigintSecond A BigInteger object.
     */
    public BigInteger gcd(BigInteger bigintSecond) {
      if((bigintSecond)==null)throw new NullPointerException("bigintSecond");
      if(this.signum()==0)
        return (bigintSecond).abs();
      if(bigintSecond.signum()==0)
        return (this).abs();
      BigInteger thisValue=this.abs();
      bigintSecond=bigintSecond.abs();
      if(bigintSecond.equals(BigInteger.ONE) ||
         thisValue.equals(bigintSecond))
        return bigintSecond;
      if(thisValue.equals(BigInteger.ONE))
        return thisValue;
      BigInteger temp;
      while(thisValue.signum()!=0){
        if (thisValue.compareTo(bigintSecond)<0)
        {
          temp = thisValue;
          thisValue = bigintSecond;
          bigintSecond = temp;
        }
        thisValue=thisValue.remainder(bigintSecond);
      }
      return bigintSecond;
    }
    
    /**
     * Calculates the remainder when a BigInteger raised to a certain power
     * is divided by another BigInteger.
     * @param bigintValue A BigInteger object.
     * @param pow A BigInteger object.
     * @param mod A BigInteger object.
     * @return (bigintValue^pow).remainder(mod)
     */
    public static BigInteger ModPow(BigInteger bigintValue, BigInteger pow, BigInteger mod) {
      if((bigintValue)==null)throw new NullPointerException("value");
      return bigintValue.ModPow(pow,mod);
    }
    /**
     * Calculates the remainder when a BigInteger raised to a certain power
     * is divided by another BigInteger.
     * @param pow A BigInteger object.
     * @param mod A BigInteger object.
     */
    public BigInteger ModPow(BigInteger pow, BigInteger mod) {
      if((pow)==null)throw new NullPointerException("pow");
      if(pow.signum()<0)
        throw new IllegalArgumentException("pow is negative");
      BigInteger r=BigInteger.ONE;
      BigInteger v=this;
      while(pow.signum()!=0){
        if(pow.testBit(0)){
          r=(r.multiply(v)).remainder(mod);
        }
        pow=pow.shiftRight(1);
        if(pow.signum()!=0){
          v=(v.multiply(v)).remainder(mod);
        }
      }
      return r;
    }
    
    
    private static void PositiveAdd(BigInteger sum,
                                    BigInteger bigintAddend,
                                    BigInteger bigintAugend) {
      int carry;
      if (bigintAddend.reg.length == bigintAugend.reg.length)
        carry = Add(sum.reg,0, bigintAddend.reg,0, bigintAugend.reg,0, (int)(bigintAddend.reg.length));
      else if (bigintAddend.reg.length > bigintAugend.reg.length)
      {
        carry = Add(sum.reg,0, bigintAddend.reg,0, bigintAugend.reg,0, (int)(bigintAugend.reg.length));
        System.arraycopy(
          bigintAddend.reg,bigintAugend.reg.length,
          sum.reg,bigintAugend.reg.length,
          bigintAddend.reg.length-bigintAugend.reg.length);
        carry = Increment(sum.reg, bigintAugend.reg.length,
                          (int)(bigintAddend.reg.length-bigintAugend.reg.length), (short)carry);
      }
      else
      {
        carry = Add(sum.reg,0, bigintAddend.reg,0, bigintAugend.reg,0, (int)(bigintAddend.reg.length));
        System.arraycopy(
          bigintAugend.reg,bigintAddend.reg.length,
          sum.reg,bigintAddend.reg.length,
          bigintAugend.reg.length-bigintAddend.reg.length);
        carry = Increment(sum.reg, bigintAddend.reg.length,
                          (int)(bigintAugend.reg.length-bigintAddend.reg.length),
                          (short)carry);
      }
      if (carry!=0)
      {
        int len=RoundupSize((sum.reg.length/2)+1);
        sum.reg=CleanGrow(sum.reg,len);
        sum.reg[sum.reg.length/2] = 1;
      }
      sum.negative=false;
      sum.wordCount=sum.CalcWordCount();
      if(sum.reg.length-sum.wordCount>10){
        // Shorten the array if there are too many zeros
        // at the end
        int newLength=RoundupSize(sum.wordCount);
        if(newLength<sum.reg.length){
          short[] newreg=new short[newLength];
          System.arraycopy(sum.reg,0,newreg,0,sum.wordCount);
          sum.reg=newreg;
        }
      }
    }
    
    static void PositiveSubtract(BigInteger diff,
                                 BigInteger minuend,
                                 BigInteger subtrahend)
    {
      int aSize = minuend.wordCount;
      aSize += aSize%2;
      int bSize = subtrahend.wordCount;
      bSize += bSize%2;

      if (aSize == bSize)
      {
        if (Compare(minuend.reg, 0, subtrahend.reg, 0, (int)aSize) >= 0)
        {
          // A is at least as high as B
          Subtract(diff.reg,0,minuend.reg,0,subtrahend.reg,0, (int)aSize);
          diff.negative=false; // difference will not be negative at this point
        }
        else
        {
          // A is less than B
          Subtract(diff.reg,0,subtrahend.reg,0,minuend.reg,0, (int)aSize);
          diff.negative=true; // difference will be negative
        }
      }
      else if (aSize > bSize)
      {
        // A is greater than B
        short borrow = (short)Subtract(diff.reg,0,minuend.reg,0,subtrahend.reg,0, (int)bSize);
        System.arraycopy(minuend.reg,bSize,diff.reg,bSize,aSize-bSize);
        borrow = (short)Decrement(diff.reg, bSize, (int)(aSize-bSize), borrow);
        //Debugif(!(borrow==0))Assert.fail("{0} line {1}: !borrow","integer.cpp",3524);
        diff.negative=false;
      }
      else
      {
        // A is less than B
        short borrow = (short)Subtract(diff.reg,0,subtrahend.reg,0,minuend.reg,0, (int)aSize);
        System.arraycopy(subtrahend.reg,aSize,diff.reg,aSize,bSize-aSize);
        borrow = (short)Decrement(diff.reg, aSize, (int)(bSize-aSize), borrow);
        //Debugif(!(borrow==0))Assert.fail("{0} line {1}: !borrow","integer.cpp",3532);
        diff.negative=true;
      }
      diff.wordCount=diff.CalcWordCount();
      if(diff.wordCount==0)diff.negative=false;
    }

    
    /**
     * Determines whether this object and another object are equal.
     * @param obj An arbitrary object.
     * @return True if the objects are equal; false otherwise.
     */
    @Override public boolean equals(Object obj) {
      BigInteger other = ((obj instanceof BigInteger) ? (BigInteger)obj : null);
      if (other == null)
        return false;
      return other.compareTo(this)==0;
    }
    
    /**
     * Returns the hash code for this instance.
     * @return A 32-bit hash code.
     */
    @Override public int hashCode() {
      int hashCodeValue = 0;
      {
        hashCodeValue += 1000000007 * this.signum();
        if (reg != null){
          for(int i=0;i<wordCount;i++){
            hashCodeValue+=1000000013*reg[i];
          }
        }
      }
      return hashCodeValue;
    }
    


    /**
     * Adds this object and another object.
     * @param bigintAugend A BigInteger object.
     * @return The sum of the two objects.
     */
    public BigInteger add(BigInteger bigintAugend) {
      if((bigintAugend)==null)throw new NullPointerException("bigintAugend");
      BigInteger sum=new BigInteger().Allocate((int)Math.max(reg.length, bigintAugend.reg.length));
      if(this.signum()>=0)
      {
        if (bigintAugend.signum()>=0)
          PositiveAdd(sum, this, bigintAugend); // both nonnegative
        else
          PositiveSubtract(sum, this, bigintAugend); // this is nonnegative, b is negative
      }
      else
      {
        if (bigintAugend.signum()>=0){
          PositiveSubtract(sum, bigintAugend, this); // this is negative, b is nonnegative
        }
        else
        {
          PositiveAdd(sum, this, bigintAugend); // both are negative
          sum.negative=sum.signum()!=0;
        }
      }
      return sum;
    }
    
    /**
     * Subtracts a BigInteger from this BigInteger.
     * @param subtrahend A BigInteger object.
     * @return The difference of the two objects.
     */
    public BigInteger subtract(BigInteger subtrahend) {
      if((subtrahend)==null)throw new NullPointerException("subtrahend");
      BigInteger diff=new BigInteger().Allocate((int)Math.max(reg.length, subtrahend.reg.length));
      if(this.signum()>=0)
      {
        if (subtrahend.signum()>=0)
          PositiveSubtract(diff, this, subtrahend);
        else
          PositiveAdd(diff, this, subtrahend);
      }
      else
      {
        if (subtrahend.signum()>=0)
        {
          PositiveAdd(diff, this, subtrahend);
          diff.negative=diff.signum()!=0;
        }
        else {
          PositiveSubtract(diff, subtrahend, this);
        }
      }
      return diff;
    }
    
    static void PositiveMultiply(BigInteger product, BigInteger bigintA, BigInteger bigintB)
    {
      int aSize = RoundupSize((int)(bigintA.wordCount));
      int bSize = RoundupSize((int)(bigintB.wordCount));

      product.reg=new short[RoundupSize(aSize+bSize)];
      product.negative=false;

      short[] workspace=new short[aSize + bSize];
      if(bigintA.equals(bigintB)){
        AsymmetricMultiply(product.reg,0,
                           workspace,0,
                           bigintA.reg,0, aSize,
                           bigintA.reg,0, aSize);
      } else {
        AsymmetricMultiply(product.reg,0,
                           workspace,0,
                           bigintA.reg,0, aSize,
                           bigintB.reg,0, bSize);
      }
      product.wordCount=product.CalcWordCount();
    }

    static void Multiply(BigInteger product, BigInteger mul1, BigInteger mul2)
    {
      PositiveMultiply(product, mul1, mul2);

      if ((mul1.signum()>=0) != (mul2.signum()>=0))
        product.NegateInternal();
    }

    /**
     * Multiplies this instance by the value of a BigInteger object.
     * @param bigintMult A BigInteger object.
     * @return The product of the two objects.
     */
    public BigInteger multiply(BigInteger bigintMult) {
      BigInteger product=new BigInteger();
      Multiply(product, this, bigintMult);
      return product;
    }

    private static int OperandLength(short[] a) {
      for(int i=a.length-1;i>=0;i--){
        if(a[i]!=0)return i+1;
      }
      return 0;
    }
    
    private static void DivideWithRemainderAnyLength(
      short[] a,
      short[] b,
      short[] quotResult,
      short[] modResult
     ) {
      int lengthA=OperandLength(a);
      int lengthB=OperandLength(b);
      if(lengthB==0) // check for zero on B first
        throw new ArithmeticException("The divisor is zero.");
      if(lengthA==0){ // 0 divided by X equals 0
        if(modResult!=null)
          java.util.Arrays.fill(modResult,0,(0)+(modResult.length),(short)0);
        if(quotResult!=null) // Set array to 0
          java.util.Arrays.fill(quotResult,0,(0)+(quotResult.length),(short)0);
        return;
      }
      if(lengthA<lengthB){
        // If lengthA is less than lengthB, then
        // A is less than B, so set quotient to 0
        // and modulus to A
        if(modResult!=null){
          short[] tmpa=new short[a.length];
          System.arraycopy(a,0,tmpa,0,a.length);
          java.util.Arrays.fill(modResult,0,(0)+(modResult.length),(short)0);
          System.arraycopy(tmpa,0,modResult,0,Math.min(tmpa.length,modResult.length));
        }
        if(quotResult!=null){ // Set quotient to 0
          java.util.Arrays.fill(quotResult,0,(0)+(quotResult.length),(short)0);
        }
        return;
      }
      if(lengthA==1 && lengthB==1){
        int a0=((int)a[0])&0xFFFF;
        int b0=((int)b[0])&0xFFFF;
        short result=((short)(a0/b0));
        short mod=(modResult!=null) ? ((short)(a0%b0)) : (short)0;
        if(quotResult!=null){
          java.util.Arrays.fill(quotResult,0,(0)+(quotResult.length),(short)0);
          quotResult[0]=result;
        }
        if(modResult!=null){
          java.util.Arrays.fill(modResult,0,(0)+(modResult.length),(short)0);
          modResult[0]=mod;
        }
        return;
      }
      lengthA+=lengthA%2;
      if(lengthA>a.length)throw new IllegalStateException("no room");
      lengthB+=lengthB%2;
      if(lengthB>b.length)throw new IllegalStateException("no room");
      short[] tempbuf=new short[lengthA+3*(lengthB+2)];
      Divide(modResult,0,
             quotResult,0,
             tempbuf,0,
             a,0,lengthA,
             b,0,lengthB);
      // Clear the area beyond the quotient in
      // the quotient array, in case the dividend
      // and the quotient are the same array
      if(quotResult!=null){
        int quotEnd=lengthA-lengthB+2;
        java.util.Arrays.fill(quotResult,quotEnd,(quotEnd)+(quotResult.length-quotEnd),(short)0);
      }
    }
    
    private static int BitsToWords(int bitCount) {
      return ((bitCount+16-1)/(16));
    }
    
    private static short FastRemainder(short[] dividendReg, int count, short divisorSmall) {
      int i=count;
      short remainder=0;
      while((i--)>0){
        remainder=RemainderUnsigned(
          MakeUint(dividendReg[i],remainder),divisorSmall);
      }
      return remainder;
    }
    
    private static void FastDivide(short[] quotientReg, int count, short divisorSmall) {
      int i=count;
      short remainder=0;
      int idivisor=(((int)divisorSmall)&0xFFFF);
      while ((i--)>0)
      {
        int currentDividend=MakeUint(quotientReg[i], remainder);
        if((currentDividend>>31)==0){
          quotientReg[i] = ((short)(currentDividend/idivisor));
          if(i>0)remainder = ((short)(currentDividend%idivisor));
        } else {
          quotientReg[i] = DivideUnsigned(currentDividend,divisorSmall);
          if(i>0)remainder = RemainderUnsigned(currentDividend,divisorSmall);
        }
      }
    }
    private static short FastDivideAndRemainder(short[] quotientReg, int count, short divisorSmall) {
      int i=count;
      short remainder=0;
      int idivisor=(((int)divisorSmall)&0xFFFF);
      while ((i--)>0)
      {
        int currentDividend=MakeUint(quotientReg[i], remainder);
        if((currentDividend>>31)==0){
          quotientReg[i] = ((short)(currentDividend/idivisor));
          remainder = ((short)(currentDividend%idivisor));
        } else {
          quotientReg[i] = DivideUnsigned(currentDividend,divisorSmall);
          remainder = RemainderUnsigned(currentDividend,divisorSmall);
        }
      }
      return remainder;
    }
    
    /**
     * Divides this instance by the value of a BigInteger object. The result
     * is rounded down (the fractional part is discarded). Except if the
     * result is 0, it will be negative if this object is positive and the other
     * is negative, or vice versa, and will be positive if both are positive
     * or both are negative.
     * @param bigintDivisor A BigInteger object.
     * @return The quotient of the two objects.
     * @throws ArithmeticException The divisor is zero.
     */
    public BigInteger divide(BigInteger bigintDivisor) {
      int aSize = this.wordCount;
      int bSize = bigintDivisor.wordCount;
      if (bSize==0)
        throw new ArithmeticException();
      if (aSize<bSize)
      {
        // dividend is less than divisor
        return BigInteger.ZERO;
      }
      if(aSize<=4 && bSize<=4 && this.HasSmallValue() && bigintDivisor.HasSmallValue()){
        long aSmall=this.longValue();
        long bSmall=bigintDivisor.longValue();
        if(aSmall!=Long.MIN_VALUE || bSmall!=-1){
          long result=aSmall/bSmall;
          return BigInteger.valueOf(result);
        }
      }
      BigInteger quotient;
      if(bSize==1){
        // divisor is small, use a fast path
        quotient=new BigInteger(this);
        FastDivide(quotient.reg,aSize,bigintDivisor.reg[0]);
        quotient.wordCount=quotient.CalcWordCount();
        if(quotient.wordCount!=0){
          quotient.negative=(this.signum()<0)^(bigintDivisor.signum()<0);
        } else {
          quotient.negative=false;
        }
        return quotient;
      }
      quotient=new BigInteger();
      aSize += aSize%2;
      bSize += bSize%2;
      quotient.reg=new short[RoundupSize((int)(aSize-bSize+2))];
      quotient.negative=false;
      DivideWithRemainderAnyLength(this.reg,bigintDivisor.reg,
                                   quotient.reg,null);
      quotient.wordCount=quotient.CalcWordCount();
      if ((this.signum()<0)^(bigintDivisor.signum()<0))
      {
        quotient.NegateInternal();
      }
      return quotient;
    }

    /**
     * 
     * @param divisor A BigInteger object.
     */
    public BigInteger[] divideAndRemainder(BigInteger divisor) {
      if((divisor)==null)throw new NullPointerException("dividend");
      BigInteger quotient;
      int aSize = this.wordCount;
      int bSize = divisor.wordCount;
      if (bSize==0)
        throw new ArithmeticException();

      if (aSize<bSize)
      {
        // dividend is less than divisor
        return new BigInteger[]{BigInteger.ZERO,this};
      }
      if(bSize==1){
        // divisor is small, use a fast path
        quotient=new BigInteger(this);
        int smallRemainder=(((int)FastDivideAndRemainder(
          quotient.reg,aSize,divisor.reg[0]))&0xFFFF);
        quotient.wordCount=quotient.CalcWordCount();
        if(quotient.wordCount!=0){
          quotient.negative=(this.signum()<0)^(divisor.signum()<0);
        } else {
          quotient.negative=false;
        }
        if(this.signum()<0)smallRemainder=-smallRemainder;
        return new BigInteger[]{quotient,BigInteger.valueOf(smallRemainder)};
      }
      BigInteger remainder=new BigInteger();
      quotient=new BigInteger();
      aSize += aSize%2;
      bSize += bSize%2;
      remainder.reg=new short[RoundupSize((int)bSize)];
      remainder.negative=false;
      quotient.reg=new short[RoundupSize((int)(aSize-bSize+2))];
      quotient.negative=false;
      DivideWithRemainderAnyLength(this.reg,divisor.reg,quotient.reg,remainder.reg);
      remainder.wordCount=remainder.CalcWordCount();
      quotient.wordCount=quotient.CalcWordCount();
      if (this.signum()<0)
      {
        quotient.NegateInternal();
        if (remainder.signum()!=0)
        {
          remainder.NegateInternal();
        }
      }
      if (divisor.signum()<0)
        quotient.NegateInternal();
      return new BigInteger[]{quotient,remainder};
    }
    /**
     * Finds the remainder that results when this instance is divided by
     * the value of a BigInteger object.
     * @param divisor A BigInteger object.
     * @return The remainder of the two objects.
     */
    public BigInteger remainder(BigInteger divisor) {
      if(this.PositiveCompare(divisor)<0){
        if(divisor.signum()==0)throw new ArithmeticException();
        return this;
      }
      BigInteger remainder=new BigInteger();
      int aSize = this.wordCount;
      int bSize = divisor.wordCount;
      if (bSize==0)
        throw new ArithmeticException();
      if (aSize<bSize)
      {
        // dividend is less than divisor
        return this;
      }
      if(bSize==1){
        short shortRemainder=FastRemainder(this.reg,this.wordCount,divisor.reg[0]);
        int smallRemainder=(((int)shortRemainder)&0xFFFF);
        if(this.signum()<0)smallRemainder=-smallRemainder;
        return BigInteger.valueOf(smallRemainder);
      }
      aSize += aSize%2;
      bSize += bSize%2;
      remainder.reg=new short[RoundupSize((int)bSize)];
      remainder.negative=false;
      short[] quotientReg=new short[RoundupSize((int)(aSize-bSize+2))];
      DivideWithRemainderAnyLength(this.reg,divisor.reg,quotientReg,remainder.reg);
      remainder.wordCount=remainder.CalcWordCount();
      if (this.signum()<0 && remainder.signum()!=0){
        remainder.NegateInternal();
      }
      return remainder;
    }


    void NegateInternal()
    {
      if(this.wordCount!=0)
        this.negative=(this.signum()>0);
    }

    int PositiveCompare(BigInteger t)
    {
      int size = this.wordCount, tSize = t.wordCount;
      if (size == tSize)
        return Compare(this.reg, 0, t.reg, 0, (int)size);
      else
        return size > tSize ? 1 : -1;
    }
    
    /**
     * Compares a BigInteger object with this instance.
     * @param other A BigInteger object.
     * @return Zero if the values are equal; a negative number is this instance
     * is less, or a positive number if this instance is greater.
     */
    public int compareTo(BigInteger other) {
      if(other==null)return 1;
      int size = this.wordCount, tSize = other.wordCount;
      int sa=(size==0 ? 0 : (this.negative ? -1 : 1));
      int sb=(tSize==0 ? 0 : (other.negative ? -1 : 1));
      if(sa!=sb)return (sa<sb) ? -1 : 1;
      if(sa==0)return 0;
      int cmp=0;
      if (size == tSize)
        cmp=Compare(this.reg, 0, other.reg, 0, (int)size);
      else
        cmp=size > tSize ? 1 : -1;
      return (sa>0) ? cmp : -cmp;
    }
    /**
     * 
     */
    public int signum() {
        if(this.wordCount==0)
          return 0;
        return (this.negative) ? -1 : 1;
      }
    
    /**
     * 
     */
    public boolean isZero() { return (this.wordCount==0); }
    
    /**
     * 
     * @param bi A BigInteger object.
     */
    public BigInteger Sqrt(BigInteger bi) {
      if (this.signum()<0)
        return BigInteger.ZERO;
      BigInteger bigintX=null;
      BigInteger bigintY = Power2((BitLength()+1)/2);
      do
      {
        bigintX = bigintY;
        bigintY = bi.divide(bigintX);
        bigintY=bigintY.add(bigintX);
        bigintY=bigintY.shiftRight(1);
      } while (bigintY.compareTo(bigintX)<0);
      return bigintX;
    }
    /**
     * Gets whether this value is even.
     */
    public boolean isEven() { return !GetUnsignedBit(0); }
    
    /**
     * 
     * @param other A BigInteger object.
     */
    public boolean equals(BigInteger other) {
      if(other==null)return false;
      return this.compareTo(other)==0;
    }
  }
