/*
Written in 2013 by Peter O.

Parts of the code were adapted by Peter O. from
the public-domain library CryptoPP by Wei Dai.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
namespace PeterO {
  /// <summary> An arbitrary-precision integer. </summary>
  public sealed partial class BigInteger : IComparable<BigInteger>, IEquatable<BigInteger>
  {

    private static int CountWords(short[] X, int N) {
      while (N != 0 && X[N - 1] == 0)
        N--;
      return (int)N;
    }

    private static void SetWords(short[] r, int rstart, short a, int n) {
      for (int i = 0; i < n; i++)
        r[rstart + i] = a;
    }

    private static short ShiftWordsLeftByBits(short[] r, int rstart, int n, int shiftBits) {
      #if DEBUG
      if (!(shiftBits < 16)) throw new ArgumentException("doesn't satisfy shiftBits<16");
      #endif

      unchecked {
        short u, carry = 0;
        if (shiftBits != 0) {
          for (int i = 0; i < n; i++) {
            u = r[rstart + i];
            r[rstart + i] = (short)((int)(u << (int)shiftBits) | (((int)carry) & 0xFFFF));
            carry = (short)((((int)u) & 0xFFFF) >> (int)(16 - shiftBits));
          }
        }
        return carry;
      }
    }

    private static short ShiftWordsRightByBits(short[] r, int rstart, int n, int shiftBits) {
      //DebugAssert.IsTrue(shiftBits<16,"{0} line {1}: shiftBits<16","words.h",67);
      short u, carry = 0;
      unchecked {
        if (shiftBits != 0)
          for (int i = n; i > 0; i--) {
          u = r[rstart + i - 1];
          r[rstart + i - 1] = (short)((((((int)u) & 0xFFFF) >> (int)shiftBits) & 0xFFFF) | (((int)carry) & 0xFFFF));
          carry = (short)((((int)u) & 0xFFFF) << (int)(16 - shiftBits));
        }
        return carry;
      }
    }

    private static short ShiftWordsRightByBitsSignExtend(short[] r, int rstart, int n, int shiftBits) {
      //DebugAssert.IsTrue(shiftBits<16,"{0} line {1}: shiftBits<16","words.h",67);
      unchecked {
        short u, carry = (short)((int)0xFFFF << (int)(16 - shiftBits));
        if (shiftBits != 0)
          for (int i = n; i > 0; i--) {
          u = r[rstart + i - 1];
          r[rstart + i - 1] = (short)(((((int)u) & 0xFFFF) >> (int)shiftBits) | (((int)carry) & 0xFFFF));
          carry = (short)((((int)u) & 0xFFFF) << (int)(16 - shiftBits));
        }
        return carry;
      }
    }

    private static void ShiftWordsLeftByWords(short[] r, int rstart, int n, int shiftWords) {
      shiftWords = Math.Min(shiftWords, n);
      if (shiftWords != 0) {
        for (int i = n - 1; i >= shiftWords; i--)
          r[rstart + i] = r[rstart + i - shiftWords];
        SetWords(r, rstart, (short)0, shiftWords);
      }
    }

    private static void ShiftWordsRightByWords(short[] r, int rstart, int n, int shiftWords) {
      shiftWords = Math.Min(shiftWords, n);
      if (shiftWords != 0) {
        for (int i = 0; i + shiftWords < n; i++)
          r[rstart + i] = r[rstart + i + shiftWords];
        SetWords(r, (int)(rstart + n - shiftWords), (short)0, shiftWords);
      }
    }

    private static void ShiftWordsRightByWordsSignExtend(short[] r, int rstart, int n, int shiftWords) {
      shiftWords = Math.Min(shiftWords, n);
      if (shiftWords != 0) {
        for (int i = 0; i + shiftWords < n; i++)
          r[rstart + i] = r[rstart + i + shiftWords];
        SetWords(r, (int)(rstart + n - shiftWords), unchecked((short)0xFFFF), shiftWords);
      }
    }

    private static int Compare(short[] A, int astart, short[] B, int bstart, int N) {
      while (unchecked(N--) != 0) {
        int an = (((int)A[astart + N]) & 0xFFFF);
        int bn = (((int)B[bstart + N]) & 0xFFFF);
        if (an > bn)
          return 1;
        else if (an < bn)
          return -1;
      }
      return 0;
    }
    private static int Increment(short[] A, int Astart, int N, short B) {
      unchecked {
        //DebugAssert.IsTrue(N!=0,"{0} line {1}: N","integer.cpp",63);
        short tmp = A[Astart];
        A[Astart] = (short)(tmp + B);
        if ((((int)A[Astart]) & 0xFFFF) >= (((int)tmp) & 0xFFFF))
          return 0;
        for (int i = 1; i < N; i++){
          A[Astart+i]++;
          if (A[Astart + i] != 0)
            return 0;
        }
        return 1;
      }
    }
    private static int Decrement(short[] A, int Astart, int N, short B) {
      //DebugAssert.IsTrue(N!=0,"{0} line {1}: N","integer.cpp",76);
      unchecked {
        short tmp = A[Astart];
        A[Astart] = (short)(tmp - B);
        if ((((int)A[Astart]) & 0xFFFF) <= (((int)tmp) & 0xFFFF))
          return 0;
        for (int i = 1; i < N; i++){
          tmp=A[Astart+i];
          A[Astart+i]--;
          if (tmp != 0)
            return 0;
        }
        return 1;
      }
    }

    private static void TwosComplement(short[] A, int Astart, int N) {
      Decrement(A, Astart, N, (short)1);
      for (int i = 0; i < N; i++)
        A[Astart + i] = unchecked((short)(~A[Astart + i]));
    }

    private static int Add(
      short[] C, int cstart,
      short[] A, int astart,
      short[] B, int bstart, int N) {
      //DebugAssert.IsTrue(N%2 == 0,"{0} line {1}: N%2 == 0","integer.cpp",799);
      unchecked {

        int u;
        u = 0;
        for (int i = 0; i < N; i += 2) {
          u = (((int)A[astart + i]) & 0xFFFF) + (((int)B[bstart + i]) & 0xFFFF) + (short)(u >> 16);
          C[cstart + i] = (short)(u);
          u = (((int)A[astart + i + 1]) & 0xFFFF) + (((int)B[bstart + i + 1]) & 0xFFFF) + (short)(u >> 16);
          C[cstart + i + 1] = (short)(u);
        }
        return (((int)u >> 16) & 0xFFFF);
      }
    }

    private static int Subtract(
      short[] C, int cstart,
      short[] A, int astart,
      short[] B, int bstart, int N) {
      //DebugAssert.IsTrue(N%2 == 0,"{0} line {1}: N%2 == 0","integer.cpp",799);
      unchecked {

        int u;
        u = 0;
        for (int i = 0; i < N; i += 2) {
          u = (((int)A[astart + i]) & 0xFFFF) - (((int)B[bstart + i]) & 0xFFFF) - (int)((u >> 31) & 1);
          C[cstart + i] = (short)(u);
          u = (((int)A[astart + i + 1]) & 0xFFFF) - (((int)B[bstart + i + 1]) & 0xFFFF) - (int)((u >> 31) & 1);
          C[cstart + i + 1] = (short)(u);
        }
        return (int)((u >> 31) & 1);
      }
    }

    private static short LinearMultiply(short[] productArr, int cstart,
                                        short[] A, int astart, short B, int N) {
      unchecked {
        short carry = 0;
        int Bint = (((int)B) & 0xFFFF);
        for (int i = 0; i < N; i++) {
          int p;
          p = (((int)A[astart + i]) & 0xFFFF) * Bint;
          p = p + (((int)carry) & 0xFFFF);
          productArr[cstart + i] = (short)(p);
          carry = (short)(p >> 16);
        }
        return carry;
      }
    }
    //-----------------------------
    //  Baseline Square
    //-----------------------------

    #region Baseline Square

    private static void Baseline_Square2(short[] R, int rstart, short[] A, int astart) {
      unchecked {
        int p; short c; int d; int e;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart]) & 0xFFFF); R[rstart] = (short)(p); e = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 1]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 2 * 2 - 3] = c;
        p = (((int)A[astart + 2 - 1]) & 0xFFFF) * (((int)A[astart + 2 - 1]) & 0xFFFF);
        p += e; R[rstart + 2 * 2 - 2] = (short)(p); R[rstart + 2 * 2 - 1] = (short)(p >> 16);
      }
    }

    private static void Baseline_Square4(short[] R, int rstart, short[] A, int astart) {
      unchecked {
        int p; short c; int d; int e;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart]) & 0xFFFF); R[rstart] = (short)(p); e = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 1]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 1] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 2]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 2] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 3] = c;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 4] = c;
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 2 * 4 - 3] = c;
        p = (((int)A[astart + 4 - 1]) & 0xFFFF) * (((int)A[astart + 4 - 1]) & 0xFFFF);
        p += e; R[rstart + 2 * 4 - 2] = (short)(p); R[rstart + 2 * 4 - 1] = (short)(p >> 16);
      }
    }

    private static void Baseline_Square8(short[] R, int rstart, short[] A, int astart) {
      unchecked {
        int p; short c; int d; int e;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart]) & 0xFFFF); R[rstart] = (short)(p); e = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 1]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 1] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 2]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 2] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 3] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 4]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 4] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 5] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 6] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 7] = c;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 8] = c;
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 9] = c;
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 10] = c;
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 11] = c;
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 12] = c;
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 2 * 8 - 3] = c;
        p = (((int)A[astart + 8 - 1]) & 0xFFFF) * (((int)A[astart + 8 - 1]) & 0xFFFF);
        p += e; R[rstart + 2 * 8 - 2] = (short)(p); R[rstart + 2 * 8 - 1] = (short)(p >> 16);
      }
    }
    private static void Baseline_Square16(short[] R, int rstart, short[] A, int astart) {
      unchecked {
        int p; short c; int d; int e;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart]) & 0xFFFF); R[rstart] = (short)(p); e = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 1]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 1] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 2]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 2] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 3] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 4]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 4] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 5] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 6] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 7] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 8]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 8] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 9]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 9] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 10]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 10] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 11]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 11] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 12]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 12] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 13]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 13] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 14]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)A[astart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 14] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 15]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)A[astart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)A[astart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 15] = c;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 15]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)A[astart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)A[astart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)A[astart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 16] = c;
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 15]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)A[astart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)A[astart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)A[astart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 17] = c;
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 15]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)A[astart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)A[astart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)A[astart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)A[astart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 18] = c;
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 15]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)A[astart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)A[astart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)A[astart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)A[astart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 19] = c;
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 15]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)A[astart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)A[astart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)A[astart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)A[astart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)A[astart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 20] = c;
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)A[astart + 15]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)A[astart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)A[astart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)A[astart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)A[astart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 21] = c;
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)A[astart + 15]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)A[astart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)A[astart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)A[astart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)A[astart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 22] = c;
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)A[astart + 15]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)A[astart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)A[astart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)A[astart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 23] = c;
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)A[astart + 15]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)A[astart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)A[astart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)A[astart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 24] = c;
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)A[astart + 15]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)A[astart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)A[astart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 25] = c;
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)A[astart + 15]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)A[astart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)A[astart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 26] = c;
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)A[astart + 15]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)A[astart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 27] = c;
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)A[astart + 15]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)A[astart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 28] = c;
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)A[astart + 15]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)(e); e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 2 * 16 - 3] = c;
        p = (((int)A[astart + 16 - 1]) & 0xFFFF) * (((int)A[astart + 16 - 1]) & 0xFFFF);
        p += e; R[rstart + 2 * 16 - 2] = (short)(p); R[rstart + 2 * 16 - 1] = (short)(p >> 16);
      }
    }
    #endregion
    //---------------------
    //  Baseline multiply
    //---------------------
    #region Baseline Multiply

    private static void Baseline_Multiply2(short[] R, int rstart, short[] A, int astart, short[] B, int bstart) {
      unchecked {
        int p; short c; int d;
        int a0=(((int)A[astart]) & 0xFFFF);
        int a1=(((int)A[astart+1]) & 0xFFFF);
        int b0=(((int)B[bstart]) & 0xFFFF);
        int b1=(((int)B[bstart+1]) & 0xFFFF);
        p = a0 * b0; c = (short)(p); d = (((int)p >> 16) & 0xFFFF); R[rstart] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = a0 * b1;
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = a1 * b0;
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 1] = c;
        p = a1 * b1;
        p += d; R[rstart + 1 + 1] = (short)(p); R[rstart + 1 + 2] = (short)(p >> 16);
      }
    }

    private static void Baseline_Multiply4(short[] R, int rstart, short[] A, int astart, short[] B, int bstart) {
      unchecked {
        int p; short c; int d;
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); R[rstart] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 1] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 2] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 3] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 4] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 5] = c;
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p += d; R[rstart + 5 + 1] = (short)(p); R[rstart + 5 + 2] = (short)(p >> 16);
      }
    }

    private static void Baseline_Multiply8(short[] R, int rstart, short[] A, int astart, short[] B, int bstart) {
      unchecked {
        int p; short c; int d;
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); R[rstart] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 1] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 2] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 3] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 4] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 5] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 6] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 7] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 8] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 9] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 10] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 11] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 12] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 13] = c;
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p += d; R[rstart + 13 + 1] = (short)(p); R[rstart + 13 + 2] = (short)(p >> 16);
      }
    }
    private static void Baseline_Multiply16(short[] R, int rstart, short[] A, int astart, short[] B, int bstart) {
      unchecked {
        int p; short c; int d;
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF); c = (short)(p); d = (((int)p >> 16) & 0xFFFF); R[rstart] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 1] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 2] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 3] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 4] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 5] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 6] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 7] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 8] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 9] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 10] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 11] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 12] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 13] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 14] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 15] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 16] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 17] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 18] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 19] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 20] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 21] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 7]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart + 7]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 22] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 8]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart + 8]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 23] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 9]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart + 9]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 24] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 10]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart + 10]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 25] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 11]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart + 11]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 26] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 12]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart + 12]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 27] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 13]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart + 13]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 28] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = (((int)A[astart + 14]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart + 14]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 29] = c;
        p = (((int)A[astart + 15]) & 0xFFFF) * (((int)B[bstart + 15]) & 0xFFFF);
        p += d; R[rstart + 30] = (short)(p); R[rstart + 31] = (short)(p >> 16);
      }
    }
    #endregion
    private const int s_recursionLimit = 16;

    private static void RecursiveMultiply(short[] Rarr,
                                          int Rstart,
                                          short[] Tarr,
                                          int Tstart, short[] Aarr, int Astart,
                                          short[] Barr, int Bstart, int N) {
      //DebugAssert.IsTrue(N>=2 && N%2==0,"{0} line {1}: N>=2 && N%2==0","integer.cpp",2066);
      if (N <= s_recursionLimit) {
        N >>= 2;
        switch (N) {
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
            throw new InvalidOperationException();
        }
      } else {
        int N2 = N / 2;
        int AN2 = Compare(Aarr, Astart, Aarr, (int)(Astart + N2), N2) > 0 ? 0 : N2;
        Subtract(Rarr, Rstart, Aarr, (int)(Astart + AN2), Aarr, (int)(Astart + (N2 ^ AN2)), N2);
        int BN2 = Compare(Barr, Bstart, Barr, (int)(Bstart + N2), N2) > 0 ? 0 : N2;
        Subtract(Rarr, ((int)(Rstart + N2)), Barr, (int)(Bstart + BN2), Barr, (int)(Bstart + (N2 ^ BN2)), N2);
        RecursiveMultiply(Rarr, (int)(Rstart + N), Tarr, (int)(Tstart + N), Aarr, (int)(Astart + N2), Barr, (int)(Bstart + N2), N2);
        RecursiveMultiply(Tarr, Tstart, Tarr, (int)(Tstart + N), Rarr, Rstart, Rarr, (int)(Rstart + N2), N2);
        RecursiveMultiply(Rarr, Rstart, Tarr, (int)(Tstart + N), Aarr, Astart, Barr, Bstart, N2);
        int c2 = Add(Rarr, (int)(Rstart + N), Rarr, (int)(Rstart + N), Rarr, ((int)(Rstart + N2)), N2);
        int c3 = c2;
        c2 += Add(Rarr, ((int)(Rstart + N2)), Rarr, (int)(Rstart + N), Rarr, (Rstart), N2);
        c3 += Add(Rarr, (int)(Rstart + N), Rarr, (int)(Rstart + N), Rarr, (int)(Rstart + N + N2), N2);
        if (AN2 == BN2)
          c3 -= Subtract(Rarr, ((int)(Rstart + N2)), Rarr, ((int)(Rstart + N2)), Tarr, Tstart, N);
        else
          c3 += Add(Rarr, ((int)(Rstart + N2)), Rarr, ((int)(Rstart + N2)), Tarr, Tstart, N);

        c3 += Increment(Rarr, (int)(Rstart + N), N2, (short)c2);
        Increment(Rarr, (int)(Rstart + N + N2), N2, (short)c3);
      }
    }

    private static void RecursiveSquare(short[] Rarr,
                                        int Rstart,
                                        short[] Tarr,
                                        int Tstart, short[] Aarr, int Astart, int N) {
      //DebugAssert.IsTrue(N!=0 && N%2==0,"{0} line {1}: N && N%2==0","integer.cpp",2108);

      if (N <= s_recursionLimit) {
        N >>= 2;
        switch (N) {
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
            throw new InvalidOperationException();
        }
      } else {
        int N2 = N / 2;

        RecursiveSquare(Rarr, Rstart, Tarr, (int)(Tstart + N), Aarr, Astart, N2);
        RecursiveSquare(Rarr, (int)(Rstart + N), Tarr, (int)(Tstart + N), Aarr, (int)(Astart + N2), N2);
        RecursiveMultiply(Tarr, Tstart, Tarr, (int)(Tstart + N),
                          Aarr, Astart, Aarr, (int)(Astart + N2), N2);

        int carry = Add(Rarr, (int)(Rstart + N2), Rarr, (int)(Rstart + N2), Tarr, Tstart, N);
        carry += Add(Rarr, (int)(Rstart + N2), Rarr, (int)(Rstart + N2), Tarr, Tstart, N);

        Increment(Rarr, (int)(Rstart + N + N2), N2, (short)carry);
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

    private static void Baseline_Multiply2Opt(short[] R, int rstart, int a0, int a1, short[] B, int bstart) {
      unchecked {
        int p; short c; int d;
        int b0=(((int)B[bstart]) & 0xFFFF);
        int b1=(((int)B[bstart+1]) & 0xFFFF);
        p = a0 * b0; c = (short)(p); d = (((int)p >> 16) & 0xFFFF); R[rstart] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
        p = a0 * b1;
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
        p = a1 * b0;
        p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 1] = c;
        p = a1 * b1;
        p += d; R[rstart + 2] = (short)(p); R[rstart + 3] = (short)(p >> 16);
      }
    }

    private static void AsymmetricMultiply(
      short[] Rarr,
      int Rstart,
      short[] Tarr,
      int Tstart, short[] Aarr, int Astart, int NA, short[] Barr, int Bstart, int NB) {
      if (NA == NB) {
        if (Astart == Bstart && Aarr == Barr){
          Square(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, NA);
        } else if (NA == 2)
          Baseline_Multiply2(Rarr, Rstart, Aarr, Astart, Barr, Bstart);
        else
          Multiply(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, Barr, Bstart, NA);

        return;
      }

      if (NA > NB) {
        short[] tmp1 = Aarr; Aarr = Barr; Barr = tmp1;
        int tmp3 = Astart; Astart = Bstart; Bstart = tmp3;
        int tmp2 = NA; NA = NB; NB = tmp2;
      }

      if (NA == 2 && Aarr[Astart + 1] == 0) {
        switch (Aarr[Astart]) {
          case 0:
            SetWords(Rarr, Rstart, (short)0, NB + 2);
            return;
          case 1:
            Array.Copy(Barr, Bstart, Rarr, Rstart, (int)NB);
            Rarr[Rstart + NB] = (short)0;
            Rarr[Rstart + NB + 1] = (short)0;
            return;
          default:
            Rarr[Rstart + NB] = LinearMultiply(Rarr, Rstart, Barr, Bstart, Aarr[Astart], NB);
            Rarr[Rstart + NB + 1] = (short)0;
            return;
        }
      } else if (NA == 2) {
        int a0=(((int)Aarr[Astart]) & 0xFFFF);
        int a1=(((int)Aarr[Astart+1]) & 0xFFFF);
        if (((NB>>1)&1) == 0) {
          Baseline_Multiply2Opt(Rarr, Rstart, a0,a1, Barr, Bstart);
          Array.Copy(Rarr, (int)(Rstart + 2), Tarr, (int)(Tstart + 4), 2);
          Baseline_Multiply2Opt2(Tarr,Tstart+2,a0,a1,Barr,Bstart,4,NB);
          Baseline_Multiply2Opt2(Rarr,Rstart,a0,a1,Barr,Bstart,2,NB);
        } else {
          Baseline_Multiply2Opt2(Rarr,Rstart,a0,a1,Barr,Bstart,0,NB);
          Baseline_Multiply2Opt2(Tarr,Tstart+2,a0,a1,Barr,Bstart,2,NB);
        }
      } else {

        int i;
        if ((NB / NA) % 2 == 0) {
          Multiply(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, Barr, Bstart, NA);
          Array.Copy(Rarr, (int)(Rstart + NA), Tarr, (int)(Tstart + 2 * NA), (int)NA);
          for (i = 2 * NA; i < NB; i += 2 * NA)
            Multiply(Tarr, (int)(Tstart + NA + i), Tarr, Tstart, Aarr, Astart, Barr, (int)(Bstart + i), NA);
          for (i = NA; i < NB; i += 2 * NA)
            Multiply(Rarr, (int)(Rstart + i), Tarr, Tstart, Aarr, Astart, Barr, (int)(Bstart + i), NA);
        } else {
          for (i = 0; i < NB; i += 2 * NA)
            Multiply(Rarr, (int)(Rstart + i), Tarr, Tstart, Aarr, Astart, Barr, (int)(Bstart + i), NA);
          for (i = NA; i < NB; i += 2 * NA)
            Multiply(Tarr, (int)(Tstart + NA + i), Tarr, Tstart, Aarr, Astart, Barr, (int)(Bstart + i), NA);
        }
      }
      if (Add(Rarr, (int)(Rstart + NA), Rarr, (int)(Rstart + NA), Tarr, (int)(Tstart + 2 * NA), NB - NA) != 0)
        Increment(Rarr, (int)(Rstart + NB), NA, (short)1);
    }

    private static int MakeUint(short first, short second) {
      return unchecked((int)((((int)first) & 0xFFFF) | ((int)(second) << 16)));
    }

    private static short GetLowHalf(int val) {
      return unchecked((short)(val & 0xFFFF));
    }

    private static short GetHighHalf(int val) {
      return unchecked((short)((val >> 16) & 0xFFFF));
    }

    private static short GetHighHalfAsBorrow(int val) {
      return unchecked((short)(0 - ((val >> 16) & 0xFFFF)));
    }

    private static int BitPrecision(short numberValue) {
      if (numberValue == 0)
        return 0;
      int i=16;
      unchecked {
        if ((numberValue >> 8) == 0) {
          numberValue <<= 8;
          i -= 8;
        }

        if ((numberValue >> 12) == 0) {
          numberValue <<= 4;
          i -= 4;
        }

        if ((numberValue >> 14) == 0) {
          numberValue <<= 2;
          i -= 2;
        }

        if ((numberValue >> 15) == 0)
          --i;
      }
      return i;
    }

    private static int BitPrecisionInt(int numberValue) {
      if (numberValue == 0)
        return 0;
      int i=32;
      unchecked {
        if ((numberValue >> 16) == 0) {
          numberValue <<= 16;
          i -= 16;
        }

        if ((numberValue >> 24) == 0) {
          numberValue <<= 8;
          i -= 8;
        }

        if ((numberValue >> 28) == 0) {
          numberValue <<= 4;
          i -= 4;
        }

        if ((numberValue >> 30) == 0) {
          numberValue <<= 2;
          i -= 2;
        }

        if ((numberValue >> 31) == 0)
          --i;
      }
      return i;
    }

    private static short Divide32By16(int dividendLow, short divisorShort, bool returnRemainder){
      int tmpInt;
      int dividendHigh=0;
      int intDivisor=(((int)divisorShort)&0xFFFF);
      for(int i=0;i<32;i++){
        tmpInt=dividendHigh>>31;
        dividendHigh<<=1;
        dividendHigh=unchecked((int)(dividendHigh|((int)((dividendLow>>31)&1))));
        dividendLow<<=1;
        tmpInt|=dividendHigh;
        // unsigned greater-than-or-equal check
        if(((tmpInt>>31)!=0) || (tmpInt>=intDivisor)){
          unchecked {
            dividendHigh-=intDivisor;
            dividendLow+=1;
          }
        }
      }
      return (returnRemainder ?
              unchecked((short)(((int)dividendHigh)&0xFFFF)) :
              unchecked((short)(((int)dividendLow)&0xFFFF))
             );
    }

    private static short DivideUnsigned(int x, short y) {
      unchecked {
        int iy = (((int)y) & 0xFFFF);
        if ((x >> 31) == 0) {
          // x is already nonnegative
          return (short)(((int)x / iy) & 0xFFFF);
        } else {
          return Divide32By16(x,y,false);
        }
      }
    }
    private static short RemainderUnsigned(int x, short y) {
      unchecked {
        int iy = (((int)y) & 0xFFFF);
        if ((x >> 31) == 0) {
          // x is already nonnegative
          return (short)(((int)x % iy) & 0xFFFF);
        } else {
          return Divide32By16(x,y,true);
        }
      }
    }

    private static short DivideThreeWordsByTwo(short[] A, int Astart, short B0, short B1) {
      //DebugAssert.IsTrue(A[2] < B1 || (A[2]==B1 && A[1] < B0),"{0} line {1}: A[2] < B1 || (A[2]==B1 && A[1] < B0)","integer.cpp",360);
      short Q;
      unchecked {
        if ((short)(B1 + 1) == 0)
          Q = A[Astart + 2];
        else if ((((int)B1) & 0xFFFF) > 0)
          Q = DivideUnsigned(MakeUint(A[Astart + 1], A[Astart + 2]), (short)(((int)B1 + 1) & 0xFFFF));
        else
          Q = DivideUnsigned(MakeUint(A[Astart], A[Astart + 1]), B0);

        int Qint = (((int)Q) & 0xFFFF);
        int B0int = (((int)B0) & 0xFFFF);
        int B1int = (((int)B1) & 0xFFFF);
        int p = B0int * Qint;
        int u = (((int)A[Astart]) & 0xFFFF) - (((int)GetLowHalf(p)) & 0xFFFF);
        A[Astart] = GetLowHalf(u);
        u = (((int)A[Astart + 1]) & 0xFFFF) - (((int)GetHighHalf(p)) & 0xFFFF) -
          (((int)GetHighHalfAsBorrow(u)) & 0xFFFF) - (B1int * Qint);
        A[Astart + 1] = GetLowHalf(u);
        A[Astart + 2] += GetHighHalf(u);
        while (A[Astart + 2] != 0 ||
               (((int)A[Astart + 1]) & 0xFFFF) > (((int)B1) & 0xFFFF) ||
               (A[Astart + 1] == B1 && (((int)A[Astart]) & 0xFFFF) >= (((int)B0) & 0xFFFF))) {
          u = (((int)A[Astart]) & 0xFFFF) - B0int;
          A[Astart] = GetLowHalf(u);
          u = (((int)A[Astart + 1]) & 0xFFFF) - B1int - (((int)GetHighHalfAsBorrow(u)) & 0xFFFF);
          A[Astart + 1] = GetLowHalf(u);
          A[Astart + 2] += GetHighHalf(u);
          Q++;
          #if DEBUG
          if (!(Q != 0)) throw new ArgumentException("doesn't satisfy Q!=0");
          #endif

        }
      }
      return Q;
    }

    private static int DivideFourWordsByTwo(short[] T, int Al, int Ah, int B) {
      if (B == 0)
        return MakeUint(GetLowHalf(Al), GetHighHalf(Ah));
      else {
        short[] Q = new short[2];
        T[0] = GetLowHalf(Al);
        T[1] = GetHighHalf(Al);
        T[2] = GetLowHalf(Ah);
        T[3] = GetHighHalf(Ah);
        Q[1] = DivideThreeWordsByTwo(T, 1, GetLowHalf(B), GetHighHalf(B));
        Q[0] = DivideThreeWordsByTwo(T, 0, GetLowHalf(B), GetHighHalf(B));
        return MakeUint(Q[0], Q[1]);
      }
    }

    private static void AtomicDivide(short[] Q, int Qstart, short[] A, int Astart, short[] B, int Bstart) {
      short[] T = new short[4];
      int q = DivideFourWordsByTwo(T, MakeUint(A[Astart], A[Astart + 1]),
                                   MakeUint(A[Astart + 2], A[Astart + 3]),
                                   MakeUint(B[Bstart], B[Bstart + 1]));
      Q[Qstart] = GetLowHalf(q);
      Q[Qstart + 1] = GetHighHalf(q);
    }

    private static void Baseline_Multiply2Opt2(short[] R, int rstart, int a0, int a1, short[] B, int bstart, int istart, int iend) {
      unchecked {
        int p; short c; int d;
        for(int i=istart;i<iend;i+=4){
          int b0=(((int)B[bstart+i]) & 0xFFFF);
          int b1=(((int)B[bstart+i+1]) & 0xFFFF);
          p = a0 * b0; c = (short)(p); d = (((int)p >> 16) & 0xFFFF); R[rstart+i] = c; c = (short)(d); d = (((int)d >> 16) & 0xFFFF);
          p = a0 * b1;
          p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF);
          p = a1 * b0;
          p = p + (((int)c) & 0xFFFF); c = (short)(p); d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 1+i] = c;
          p = a1 * b1;
          p += d; R[rstart + 2+i] = (short)(p); R[rstart + 3+i] = (short)(p >> 16);
        }
      }
    }

    // for use by Divide(), corrects the underestimated quotient {Q1,Q0}
    private static void CorrectQuotientEstimate(
      short[] Rarr,
      int Rstart,
      short[] Tarr, int Tstart,
      short[] Qarr, int Qstart,
      short[] Barr, int Bstart, int N) {
      #if DEBUG
      if (!(N != 0 && N % 2 == 0)) throw new ArgumentException("doesn't satisfy N!=0 && N%2==0");
      #endif
      unchecked {
        if (N == 2)
          Baseline_Multiply2(Tarr, Tstart, Qarr, Qstart, Barr, Bstart);
        else
          AsymmetricMultiply(Tarr, Tstart, Tarr, (int)(Tstart + (N + 2)), Qarr, Qstart, 2, Barr, Bstart, N);
        Subtract(Rarr, Rstart, Rarr, Rstart, Tarr, Tstart, N + 2);
        while (Rarr[Rstart + N] != 0 || Compare(Rarr, Rstart, Barr, Bstart, N) >= 0) {
          Rarr[Rstart + N] -= (short)Subtract(Rarr, Rstart, Rarr, Rstart, Barr, Bstart, N);
          Qarr[Qstart]++;
          Qarr[Qstart + 1] += (short)((Qarr[Qstart] == 0) ? 1 : 0);
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
      int NA = (int)NAint;
      int NB = (int)NBint;
      #if DEBUG
      if ((NAint) <= 0) throw new ArgumentException("NAint" + " not less than " + "0" + " (" + Convert.ToString((int)(NAint), System.Globalization.CultureInfo.InvariantCulture) + ")");
      if ((NBint) <= 0) throw new ArgumentException("NBint" + " not less than " + "0" + " (" + Convert.ToString((int)(NBint), System.Globalization.CultureInfo.InvariantCulture) + ")");
      if (!(NA % 2 == 0 && NB % 2 == 0)) throw new ArgumentException("doesn't satisfy NA%2==0 && NB%2==0");
      if (!(Barr[Bstart + NB - 1] != 0 ||
            Barr[Bstart + NB - 2] != 0)) throw new ArgumentException("doesn't satisfy B[NB-1]!=0 || B[NB-2]!=0");
      if (!(NB <= NA)) throw new ArgumentException("doesn't satisfy NB<=NA");
      #endif
      short[] TBarr = TA;
      short[] TParr = TA;
      int TBstart = (int)(Tstart + (NA + 2));
      int TPstart = (int)(Tstart + (NA + 2 + NB));
      unchecked {
        // copy B into TB and normalize it so that TB has highest bit set to 1
        int shiftWords = (short)(Barr[Bstart + NB - 1] == 0 ? 1 : 0);
        TBarr[TBstart] = (short)0;
        TBarr[TBstart + NB - 1] = (short)0;
        Array.Copy(Barr, Bstart, TBarr, (int)(TBstart + shiftWords), NB - shiftWords);
        short shiftBits = (short)((short)16 - BitPrecision(TBarr[TBstart + NB - 1]));
        ShiftWordsLeftByBits(
          TBarr, TBstart,
          NB, shiftBits);
        // copy A into TA and normalize it
        TA[0] = (short)0;
        TA[NA] = (short)0;
        TA[NA + 1] = (short)0;
        Array.Copy(Aarr, Astart, TA, (int)(Tstart + shiftWords), NAint);
        ShiftWordsLeftByBits(
          TA, Tstart, NA + 2, shiftBits);

        if (TA[Tstart + NA + 1] == 0 && (((int)TA[Tstart + NA]) & 0xFFFF) <= 1) {
          Qarr[Qstart + NA - NB + 1] = (short)0;
          Qarr[Qstart + NA - NB] = (short)0;
          while (TA[NA] != 0 || Compare(TA, (int)(Tstart + NA - NB),
                                        TBarr, TBstart, NB) >= 0) {
            TA[NA] -= (short)Subtract(TA, (int)(Tstart + NA - NB),
                                      TA, (int)(Tstart + NA - NB),
                                      TBarr, TBstart, NB);
            Qarr[Qstart + NA - NB]+=(short)1;
          }
        } else {
          NA += 2;
        }

        short[] BT = new short[2];
        BT[0] = (short)(TBarr[TBstart + NB - 2] + (short)1);
        BT[1] = (short)(TBarr[TBstart + NB - 1] + (short)(BT[0] == 0 ? 1 : 0));

        // start reducing TA mod TB, 2 words at a time
        for (int i = NA - 2; i >= NB; i -= 2) {
          AtomicDivide(Qarr, (int)(Qstart + i - NB), TA, (int)(Tstart + i - 2), BT, 0);
          CorrectQuotientEstimate(TA, (int)(Tstart + i - NB),
                                  TParr, TPstart, Qarr, (int)(Qstart + (i - NB)), TBarr, TBstart, NB);
        }
        if (Rarr != null) { // If the remainder is non-null
          // copy TA into R, and denormalize it
          Array.Copy(TA, (int)(Tstart + shiftWords), Rarr, Rstart, NB);
          ShiftWordsRightByBits(Rarr, Rstart, NB, shiftBits);
        }
      }
    }

    private static int[] RoundupSizeTable = new int[] {
      2, 2, 2, 4, 4, 8, 8, 8, 8,
      16, 16, 16, 16, 16, 16, 16, 16
    };

    private static int RoundupSize(int n) {
      if (n <= 16)
        return RoundupSizeTable[n];
      else if (n <= 32)
        return 32;
      else if (n <= 64)
        return 64;
      else return (int)(1) << (int)BitPrecisionInt(n - 1);
    }
    bool negative;
    int wordCount = -1;
    short[] reg;
    /// <summary> Initializes a BigInteger object set to zero. </summary>
    private BigInteger() {}
    /// <summary> Initializes a BigInteger object from an array of bytes.
    /// </summary>
    /// <param name='bytes'>A byte[] object.</param>
    /// <returns>A BigInteger object.</returns>
    /// <param name='littleEndian'>A Boolean object.</param>
    public static BigInteger fromByteArray(byte[] bytes, bool littleEndian){
      BigInteger bigint=new BigInteger();
      bigint.fromByteArrayInternal(bytes, littleEndian);
      return bigint;
    }
    private void fromByteArrayInternal(byte[] bytes, bool littleEndian) {
      if (bytes == null)
        throw new ArgumentNullException("bytes");
      if (bytes.Length == 0) {
        this.reg = new short[] { (short)0, (short)0 };
        this.wordCount = 0;
      } else {
        int len=bytes.Length;
        int wordLength=((int)len + 1) / 2;
        wordLength=(wordLength<=16) ?
          RoundupSizeTable[wordLength] :
          RoundupSize(wordLength);
        this.reg = new short[wordLength];
        int jIndex = (littleEndian) ? len - 1 : 0;
        bool negative = ((bytes[jIndex]) & 0x80) != 0;
        this.negative=negative;
        int j = 0;
        if(!negative){
          for (int i = 0; i < len; i += 2, j++) {
            int index = (littleEndian) ? i : len - 1 - i;
            int index2 = (littleEndian) ? i + 1 : len - 2 - i;
            this.reg[j] = (short)(((int)bytes[index]) & 0xFF);
            if (index2 >= 0 && index2 < len) {
              this.reg[j] |= unchecked((short)(((short)bytes[index2]) << 8));
            }
          }
        } else {
          for (int i = 0; i < len; i += 2, j++) {
            int index = (littleEndian) ? i : len - 1 - i;
            int index2 = (littleEndian) ? i + 1 : len - 2 - i;
            this.reg[j] = (short)(((int)bytes[index]) & 0xFF);
            if (index2 >= 0 && index2 < len) {
              this.reg[j] |= unchecked((short)(((short)bytes[index2]) << 8));
            } else {
              // sign extend the last byte
              this.reg[j] |= unchecked((short)0xFF00);
            }
          }
          for (; j < reg.Length; j++) {
            this.reg[j] = unchecked((short)0xFFFF); // sign extend remaining words
          }
          TwosComplement(this.reg, 0, (int)this.reg.Length);
        }
        this.wordCount=this.reg.Length;
        while (this.wordCount != 0 &&
               this.reg[this.wordCount - 1] == 0)
          this.wordCount--;
      }
    }

    private BigInteger Allocate(int length) {
      this.reg = new short[RoundupSize(length)]; // will be initialized to 0
      this.negative = false;
      this.wordCount = 0;
      return this;
    }

    private static short[] GrowForCarry(short[] a, short carry){
      int oldLength=a.Length;
      short[] ret=CleanGrow(a,RoundupSize(oldLength+1));
      ret[oldLength]=carry;
      return ret;
    }

    private static short[] CleanGrow(short[] a, int size) {
      if (size > a.Length) {
        short[] newa = new short[size];
        Array.Copy(a, newa, a.Length);
        return newa;
      }
      return a;
    }

    private void SetBitInternal(int n, bool value) {
      if (value) {
        this.reg = CleanGrow(reg, RoundupSize(BitsToWords(n + 1)));
        this.reg[n / 16] |= (short)((short)(1) << (int)(n & 0xf));
        this.wordCount = CalcWordCount();
      } else {
        if (n / 16 < reg.Length)
          reg[n / 16] &= unchecked((short)(~((short)(1) << (int)(n % 16))));
        this.wordCount = CalcWordCount();
      }
    }

    /// <summary> </summary>
    /// <param name='index'>A 32-bit unsigned integer.</param>
    /// <returns>A Boolean object.</returns>
    public bool testBit(int index) {
      if (index < 0) throw new ArgumentOutOfRangeException("index");
      if (this.Sign < 0) {
        int tcindex = 0;
        int wordpos = index / 16;
        if (wordpos >= reg.Length) return true;
        while (tcindex < wordpos && reg[tcindex] == 0) {
          tcindex++;
        }
        short tc;
        unchecked {
          tc = reg[wordpos];
          if (tcindex == wordpos) tc--;
          tc = (short)~tc;
        }
        return (bool)(((tc >> (int)(index & 15)) & 1) != 0);
      } else {
        return this.GetUnsignedBit(index);
      }
    }

    /// <summary> </summary>
    /// <param name='n'>A 32-bit unsigned integer.</param>
    /// <returns></returns>
    private bool GetUnsignedBit(int n) {
      #if DEBUG
      if ((n) < 0) throw new ArgumentException("n" + " not greater or equal to " + "0" + " (" + Convert.ToString((int)(n), System.Globalization.CultureInfo.InvariantCulture) + ")");
      #endif
      if (n / 16 >= reg.Length)
        return false;
      else
        return (bool)(((reg[n / 16] >> (int)(n & 15)) & 1) != 0);
    }

    private BigInteger InitializeInt(int numberValue) {
      int iut;
      unchecked {
        this.negative = (numberValue < 0);
        if (numberValue == Int32.MinValue) {
          this.reg = new short[2];
          this.reg[0] = 0;
          this.reg[1] = (short)0x8000;
          this.wordCount = 2;
        } else {
          iut = unchecked((numberValue < 0) ? (int)-numberValue : (int)numberValue);
          this.reg = new short[2];
          this.reg[0] = (short)iut;
          this.reg[1] = (short)(iut >> 16);
          this.wordCount = (this.reg[1] != 0 ? 2 : (this.reg[0] == 0 ? 0 : 1));
        }
      }
      return this;
    }
    /// <summary>Returns a byte array of this object's value. </summary>
    /// <returns>A byte array that represents the value of this object.</returns>
    /// <param name='littleEndian'>A Boolean object.</param>
    public byte[] toByteArray(bool littleEndian) {
      int sign = this.Sign;
      if (sign == 0) {
        return new byte[] { (byte)0 };
      } else if (sign > 0) {
        int byteCount = ByteCount();
        int byteArrayLength = byteCount;
        if (GetUnsignedBit((byteCount*8) - 1)) {
          byteArrayLength++;
        }
        byte[] bytes = new byte[byteArrayLength];
        int j = 0;
        for (int i = 0; i < byteCount; i += 2, j++) {
          int index = (littleEndian) ? i : bytes.Length - 1 - i;
          int index2 = (littleEndian) ? i + 1 : bytes.Length - 2 - i;
          bytes[index] = (byte)((reg[j]) & 0xff);
          if (index2 >= 0 && index2 < byteArrayLength) {
            bytes[index2] = (byte)((reg[j] >> 8) & 0xff);
          }
        }
        return bytes;
      } else {
        short[] regdata = new short[reg.Length];
        Array.Copy(reg, regdata, reg.Length);
        TwosComplement(regdata, 0, (int)regdata.Length);
        int byteCount = regdata.Length * 2;
        for (int i = regdata.Length - 1; i >= 0; i--) {
          if (regdata[i] == unchecked((short)0xFFFF)) {
            byteCount -= 2;
          } else if ((regdata[i] & 0xFF80) == 0xFF80) {
            //signed first byte, 0xFF second
            byteCount -= 1;
            break;
          } else if ((regdata[i] & 0x8000) == 0x8000) {
            //signed second byte
            break;
          } else {
            //unsigned second byte
            byteCount += 1;
            break;
          }
        }
        if (byteCount == 0) byteCount = 1;
        byte[] bytes = new byte[byteCount];
        bytes[(littleEndian) ? bytes.Length - 1 : 0] = (byte)0xFF;
        byteCount = Math.Min(byteCount, regdata.Length * 2);
        int j = 0;
        for (int i = 0; i < byteCount; i += 2, j++) {
          int index = (littleEndian) ? i : bytes.Length - 1 - i;
          int index2 = (littleEndian) ? i + 1 : bytes.Length - 2 - i;
          bytes[index] = (byte)((regdata[j]) & 0xff);
          if (index2 >= 0 && index2 < byteCount) {
            bytes[index2] = (byte)((regdata[j] >> 8) & 0xff);
          }
        }
        return bytes;
      }
    }

    /// <summary> Shifts this object's value by a number of bits. A value of
    /// 1 doubles this value, a value of 2 multiplies it by 4, a value of 3 by 8,
    /// a value of 4 by 16, and so on.</summary>
    /// <param name='numberBits'>The number of bits to shift. Can be negative,
    /// in which case this is the same as shiftRight with the absolute value
    /// of numberBits.</param>
    /// <returns>A BigInteger object.</returns>
    public BigInteger shiftLeft(int numberBits) {
      if (numberBits == 0) return this;
      if (numberBits < 0){
        if(numberBits==Int32.MinValue)
          return this.shiftRight(1).shiftRight(Int32.MaxValue);
        return this.shiftRight(-numberBits);
      }
      BigInteger ret = new BigInteger();
      int numWords = (int)(this.wordCount);
      int shiftWords = (int)(numberBits >> 4);
      int shiftBits = (int)(numberBits & 15);
      bool neg=numWords>0 && this.negative;
      if(!neg){
        ret.negative=false;
        ret.reg = new short[RoundupSize(numWords + BitsToWords((int)numberBits))];
        Array.Copy(this.reg,ret.reg,numWords);
        ShiftWordsLeftByWords(ret.reg, 0, numWords + shiftWords, shiftWords);
        ShiftWordsLeftByBits(ret.reg, (int)shiftWords, numWords + BitsToWords(shiftBits), shiftBits);
        ret.wordCount = ret.CalcWordCount();
      } else {
        ret.negative=true;
        ret.reg = new short[RoundupSize(numWords + BitsToWords((int)numberBits))];
        Array.Copy(this.reg,ret.reg,numWords);
        TwosComplement(ret.reg, 0, (int)(ret.reg.Length));
        ShiftWordsLeftByWords(ret.reg, 0, numWords + shiftWords, shiftWords);
        ShiftWordsLeftByBits(ret.reg, (int)shiftWords, numWords + BitsToWords(shiftBits), shiftBits);
        TwosComplement(ret.reg, 0, (int)(ret.reg.Length));
        ret.wordCount = ret.CalcWordCount();
      }
      return ret;
    }
    /// <summary> </summary>
    /// <returns>A BigInteger object.</returns>
    /// <param name='numberBits'>A 32-bit signed integer.</param>
    public BigInteger shiftRight(int numberBits) {
      if (numberBits == 0) return this;
      if (numberBits < 0){
        if(numberBits==Int32.MinValue)
          return this.shiftLeft(1).shiftLeft(Int32.MaxValue);
        return this.shiftLeft(-numberBits);
      }
      BigInteger ret = new BigInteger();
      int numWords = (int)(this.wordCount);
      int shiftWords = (int)(numberBits >> 4);
      int shiftBits = (int)(numberBits & 15);
      ret.negative=this.negative;
      ret.reg = new short[RoundupSize(numWords)];
      Array.Copy(this.reg,ret.reg,numWords);
      if (this.Sign < 0) {
        TwosComplement(ret.reg, 0, (int)(ret.reg.Length));
        ShiftWordsRightByWordsSignExtend(ret.reg, 0, numWords, shiftWords);
        if (numWords > shiftWords)
          ShiftWordsRightByBitsSignExtend(ret.reg, 0, numWords - shiftWords, shiftBits);
        TwosComplement(ret.reg, 0, (int)(ret.reg.Length));
      } else {
        ShiftWordsRightByWords(ret.reg, 0, numWords, shiftWords);
        if (numWords > shiftWords)
          ShiftWordsRightByBits(ret.reg, 0, numWords - shiftWords, shiftBits);
      }
      ret.wordCount = ret.CalcWordCount();
      return ret;
    }

    /// <summary> </summary>
    /// <returns>A BigInteger object.</returns>
    /// <param name='longerValue'>A 64-bit signed integer.</param>
    public static BigInteger valueOf(long longerValue) {
      if (longerValue == 0) return BigInteger.Zero;
      if (longerValue == 1) return BigInteger.One;
      BigInteger ret = new BigInteger();
      unchecked {
        ret.negative = (longerValue < 0);
        ret.reg = new short[4];
        if (longerValue == Int64.MinValue) {
          ret.reg[0] = 0;
          ret.reg[1] = 0;
          ret.reg[2] = 0;
          ret.reg[3] = (short)0x8000;
          ret.wordCount = 4;
        } else {
          long ut = longerValue;
          if(ut<0)ut=-ut;
          ret.reg[0] = (short)(ut & 0xFFFF);
          ut>>=16;
          ret.reg[1] = (short)(ut & 0xFFFF);
          ut>>=16;
          ret.reg[2] = (short)(ut & 0xFFFF);
          ut>>=16;
          ret.reg[3] = (short)(ut & 0xFFFF);
          // at this point, the word count can't
          // be 0 (the check for 0 was already done above)
          ret.wordCount = 4;
          while (ret.wordCount != 0 &&
                 ret.reg[ret.wordCount - 1] == 0)
            ret.wordCount--;
        }
      }
      return ret;
    }

    /// <summary> </summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int intValue() {
      int c = (int)this.wordCount;
      if (c == 0) return 0;
      if (c > 2) throw new OverflowException();
      if (c == 2 && (this.reg[1] & 0x8000) != 0) {
        if (((short)(this.reg[1] & (short)0x7FFF) | this.reg[0]) == 0 && this.negative) {
          return Int32.MinValue;
        } else {
          throw new OverflowException();
        }
      } else {
        int ivv = (((int)this.reg[0]) & 0xFFFF);
        if (c > 1) ivv |= (((int)this.reg[1]) & 0xFFFF) << 16;
        if (this.negative) ivv = -ivv;
        return ivv;
      }
    }

    private bool HasTinyValue() {
      int c = (int)this.wordCount;
      if (c > 2) return false;
      if (c == 2 && (this.reg[1] & 0x8000) != 0) {
        return (this.negative && this.reg[1] == unchecked((short)0x8000) &&
                this.reg[0] == 0);
      }
      return true;
    }

    private bool HasSmallValue() {
      int c = (int)this.wordCount;
      if (c > 4) return false;
      if (c == 4 && (this.reg[3] & 0x8000) != 0) {
        return (this.negative && this.reg[3] == unchecked((short)0x8000) &&
                this.reg[2] == 0 &&
                this.reg[1] == 0 &&
                this.reg[0] == 0);
      }
      return true;
    }

    /// <summary> </summary>
    /// <returns>A 64-bit signed integer.</returns>
    public long longValue() {
      int count = this.wordCount;
      if (count == 0) return (long)0;
      if (count > 4) throw new OverflowException();
      if (count == 4 && (this.reg[3] & 0x8000) != 0) {
        if (this.negative && this.reg[3] == unchecked((short)0x8000) &&
            this.reg[2] == 0 &&
            this.reg[1] == 0 &&
            this.reg[0] == 0) {
          return Int64.MinValue;
        } else {
          throw new OverflowException();
        }
      } else {
        int tmp=((int)this.reg[0]) & 0xFFFF;
        long vv = (long)tmp;
        if (count > 1){
          tmp=((int)this.reg[1]) & 0xFFFF;
          vv |= (long)(tmp) << 16;
        }
        if (count > 2){
          tmp=((int)this.reg[2]) & 0xFFFF;
          vv |= (long)(tmp) << 32;
        }
        if (count > 3){
          tmp=((int)this.reg[3]) & 0xFFFF;
          vv |= (long)(tmp) << 48;
        }
        if (this.negative) vv = -vv;
        return vv;
      }
    }

    private static BigInteger Power2(int e) {
      BigInteger r = new BigInteger().Allocate(BitsToWords((int)(e + 1)));
      r.SetBitInternal((int)e, true); // NOTE: Will recalculate word count
      return r;
    }

    /// <summary> </summary>
    /// <returns>A BigInteger object.</returns>
    /// <param name='power'>A BigInteger object.</param>
    public BigInteger PowBigIntVar(BigInteger power) {
      if ((power) == null) throw new ArgumentNullException("power");
      int sign=power.Sign;
      if (sign < 0) throw new ArgumentException("power is negative");
      BigInteger thisVar = this;
      if (sign==0)
        return BigInteger.One; // however 0 to the power of 0 is undefined
      else if (power.Equals(BigInteger.One))
        return this;
      else if (power.wordCount==1 && power.reg[0]==2)
        return thisVar * (BigInteger)thisVar;
      else if (power.wordCount==1 && power.reg[0]==3)
        return (thisVar * (BigInteger)thisVar) * (BigInteger)thisVar;
      BigInteger r = BigInteger.One;
      while (!power.IsZero) {
        if (!power.IsEven) {
          r = (r * (BigInteger)thisVar);
        }
        power >>= 1;
        if (!power.IsZero) {
          thisVar = (thisVar * (BigInteger)thisVar);
        }
      }
      return r;
    }

    /// <summary> </summary>
    /// <param name='powerSmall'>A 32-bit signed integer.</param>
    /// <returns>A BigInteger object.</returns>
    public BigInteger pow(int powerSmall) {
      if (powerSmall < 0) throw new ArgumentException("power is negative");
      BigInteger thisVar = this;
      if (powerSmall == 0)
        return BigInteger.One; // however 0 to the power of 0 is undefined
      else if (powerSmall == 1)
        return this;
      else if (powerSmall == 2)
        return thisVar * (BigInteger)thisVar;
      else if (powerSmall == 3)
        return (thisVar * (BigInteger)thisVar) * (BigInteger)thisVar;
      BigInteger r = BigInteger.One;
      while (powerSmall != 0) {
        if ((powerSmall & 1) != 0) {
          r = (r * (BigInteger)thisVar);
        }
        powerSmall >>= 1;
        if (powerSmall != 0) {
          thisVar = (thisVar * (BigInteger)thisVar);
        }
      }
      return r;
    }

    /// <summary> </summary>
    /// <returns>A BigInteger object.</returns>
    public BigInteger negate() {
      BigInteger bigintRet = new BigInteger();
      bigintRet.reg = this.reg; // use the same reference
      bigintRet.wordCount = this.wordCount;
      bigintRet.negative = (this.wordCount!=0) && (!this.negative);
      return bigintRet;
    }
    /// <summary> </summary>
    /// <returns>A BigInteger object.</returns>
    public BigInteger abs() {
      return this.Sign >= 0 ? this : this.negate();
    }

    /// <summary> </summary>
    /// <returns></returns>
    private int CalcWordCount() {
      return (int)CountWords(reg, reg.Length);
    }

    /// <summary> </summary>
    /// <returns></returns>
    private int ByteCount() {
      int wc = this.wordCount;
      if(wc==0)return 0;
      short s=reg[wc-1];
      wc=(wc-1)<<1;
      if(s==0)return wc;
      return ((s>>8)==0) ? wc+1 : wc+2;
    }

    /// <summary> Finds the minimum number of bits needed to represent this
    /// object's absolute value. </summary>
    /// <returns>The number of bits in this object&apos;s value. Returns
    /// 0 if this object&apos;s value is 0, and returns 1 if the value is negative
    /// 1</returns>
    public int getUnsignedBitLength(){
      int wc = this.wordCount;
      if (wc!=0){
        int numberValue=(((int)(this.reg[wc-1]))&0xFFFF);
        wc=(wc-1)<<4;
        if (numberValue == 0)return wc;
        wc+=16;
        unchecked {
          if ((numberValue >> 8) == 0) {
            numberValue <<= 8;
            wc -= 8;
          }
          if ((numberValue >> 12) == 0) {
            numberValue <<= 4;
            wc -= 4;
          }
          if ((numberValue >> 14) == 0) {
            numberValue <<= 2;
            wc -= 2;
          }
          if ((numberValue >> 15) == 0)
            --wc;
        }
        return wc;
      } else {
        return 0;
      }
    }

    /// <summary> </summary>
    /// <param name='reg'>A short[] object.</param>
    /// <param name='wordCount'>A 32-bit signed integer.</param>
    /// <returns>A 32-bit signed integer.</returns>
    private int getUnsignedBitLengthEx(short[] reg, int wordCount){
      int wc = wordCount;
      if (wc!=0){
        int numberValue=(((int)(reg[wc-1]))&0xFFFF);
        wc=(wc-1)<<4;
        if (numberValue == 0)return wc;
        wc+=16;
        unchecked {
          if ((numberValue >> 8) == 0) {
            numberValue <<= 8;
            wc -= 8;
          }
          if ((numberValue >> 12) == 0) {
            numberValue <<= 4;
            wc -= 4;
          }
          if ((numberValue >> 14) == 0) {
            numberValue <<= 2;
            wc -= 2;
          }
          if ((numberValue >> 15) == 0)
            --wc;
        }
        return wc;
      } else {
        return 0;
      }
    }

    /// <summary>Finds the minimum number of bits needed to represent this
    /// object's value, except for its sign. If the value is negative, finds
    /// the number of bits in (its absolute value minus 1).</summary>
    /// <returns>The number of bits in this object&apos;s value. Returns
    /// 0 if this object&apos;s value is 0 or negative 1.</returns>
    public int bitLength() {
      int wc = this.wordCount;
      if (wc!=0){
        int numberValue=(((int)(reg[wc-1]))&0xFFFF);
        wc=(wc-1)<<4;
        if (numberValue == (this.negative ? 1 : 0))return wc;
        wc+=16;
        unchecked {
          if(this.negative){
            numberValue--;
            numberValue&=0xFFFF;
          }
          if ((numberValue >> 8) == 0) {
            numberValue <<= 8;
            wc -= 8;
          }
          if ((numberValue >> 12) == 0) {
            numberValue <<= 4;
            wc -= 4;
          }
          if ((numberValue >> 14) == 0) {
            numberValue <<= 2;
            wc -= 2;
          }
          if ((numberValue >> 15) == 0)
            --wc;
        }
        return wc;
      } else {
        return 0;
      }
    }

    private const string vec = "0123456789ABCDEF";

    private static void ReverseChars(char[] chars, int offset, int length) {
      int half = length >> 1;
      int right = offset + length - 1;
      for (int i = 0; i < half; i++, right--) {
        char value = chars[offset + i];
        chars[offset + i] = chars[right];
        chars[right] = value;
      }
    }
    private string SmallValueToString() {
      long value = longValue();
      if (value == Int64.MinValue)
        return "-9223372036854775808";
      bool neg = (value < 0);
      char[] chars = new char[24];
      int count = 0;
      if (neg) {
        chars[0] = '-';
        count++;
        value = -value;
      }
      while (value != 0) {
        char digit = vec[(int)(value % 10)];
        chars[count++] = digit;
        value = value / 10;
      }
      if (neg)
        ReverseChars(chars, 1, count - 1);
      else
        ReverseChars(chars, 0, count);
      return new String(chars, 0, count);
    }

    /// <summary> Finds the number of decimal digits this number has.</summary>
    /// <returns>The number of decimal digits. Returns 1 if this object&apos;s
    /// value is 0.</returns>
    public int getDigitCount() {
      if (this.IsZero)
        return 1;
      if (HasSmallValue()) {
        long value = longValue();
        if(value==Int64.MinValue)return 19;
        if(value<0)value=-value;
        if(value>=1000000000L){
          if(value>=1000000000000000000L)return 19;
          if(value>=100000000000000000L)return 18;
          if(value>=10000000000000000L)return 17;
          if(value>=1000000000000000L)return 16;
          if(value>=100000000000000L)return 15;
          if(value>=10000000000000L)return 14;
          if(value>=1000000000000L)return 13;
          if(value>=100000000000L)return 12;
          if(value>=10000000000L)return 11;
          if(value>=1000000000L)return 10;
          return 9;
        } else {
          int v2=(int)value;
          if(v2>=100000000)return 9;
          if(v2>=10000000)return 8;
          if(v2>=1000000)return 7;
          if(v2>=100000)return 6;
          if(v2>=10000)return 5;
          if(v2>=1000)return 4;
          if(v2>=100)return 3;
          if(v2>=10)return 2;
          return 1;
        }
      }
      int bitlen=getUnsignedBitLength();
      if(bitlen<=2135){
        // (x*631305)>>21 is an approximation
        // to trunc(x*log10(2)) that is correct up
        // to x=2135; the multiplication would require
        // up to 31 bits in all cases up to 2135
        // (cases up to 64 are already handled above)
        int minDigits=1+(((bitlen-1)*631305)>>21);
        int maxDigits=1+(((bitlen)*631305)>>21);
        if(minDigits==maxDigits){
          // Number of digits is the same for
          // all numbers with this bit length
          return minDigits;
        }
      } else if(bitlen<=6432162){
        // Much more accurate approximation
        BigInteger biMult=(BigInteger)661971961083L;
        BigInteger minDigits=(BigInteger)(bitlen-1);
        minDigits*=(BigInteger)biMult;
        minDigits>>=41;
        BigInteger maxDigits=(BigInteger)(bitlen);
        maxDigits*=(BigInteger)biMult;
        maxDigits>>=41;
        if(minDigits.Equals(maxDigits)){
          // Number of digits is the same for
          // all numbers with this bit length
          return 1+(int)minDigits;
        }
      }
      short[] tempReg = new short[this.wordCount];
      Array.Copy(this.reg, tempReg, tempReg.Length);
      int wordCount = tempReg.Length;
      while (wordCount != 0 && tempReg[wordCount - 1] == 0)
        wordCount--;
      int i = 0;
      while (wordCount != 0) {
        if (wordCount == 1) {
          int rest = (((int)tempReg[0])&0xFFFF);
          if(rest>=10000)i+=5;
          else if(rest>=1000)i+=4;
          else if(rest>=100)i+=3;
          else if(rest>=10)i+=2;
          else i++;
          break;
        } else if (wordCount == 2 && tempReg[1] > 0 && tempReg[1] <=0x7FFF) {
          int rest = (((int)tempReg[0])&0xFFFF);
          rest|=((((int)tempReg[1])&0xFFFF)<<16);
          if(rest>=1000000000)i+=10;
          else if(rest>=100000000)i+=9;
          else if(rest>=10000000)i+=8;
          else if(rest>=1000000)i+=7;
          else if(rest>=100000)i+=6;
          else if(rest>=10000)i+=5;
          else if(rest>=1000)i+=4;
          else if(rest>=100)i+=3;
          else if(rest>=10)i+=2;
          else i++;
          break;
        } else {
          FastDivideAndRemainder(tempReg, wordCount, (short)10000);
          // Recalculate word count
          while (wordCount != 0 && tempReg[wordCount - 1] == 0)
            wordCount--;
          i+=4;
          bitlen=getUnsignedBitLengthEx(tempReg,wordCount);
          if(bitlen<=2135){
            // (x*631305)>>21 is an approximation
            // to trunc(x*log10(2)) that is correct up
            // to x=2135; the multiplication would require
            // up to 31 bits in all cases up to 2135
            // (cases up to 64 are already handled above)
            int minDigits=1+(((bitlen-1)*631305)>>21);
            int maxDigits=1+(((bitlen)*631305)>>21);
            if(minDigits==maxDigits){
              // Number of digits is the same for
              // all numbers with this bit length
              return i+minDigits;
            }
          } else if(bitlen<=6432162){
            // Much more accurate approximation
            BigInteger biMult=(BigInteger)661971961083L;
            BigInteger minDigits=(BigInteger)(bitlen-1);
            minDigits*=(BigInteger)biMult;
            minDigits>>=41;
            BigInteger maxDigits=(BigInteger)(bitlen);
            maxDigits*=(BigInteger)biMult;
            maxDigits>>=41;
            if(minDigits.Equals(maxDigits)){
              // Number of digits is the same for
              // all numbers with this bit length
              return i+1+(int)minDigits;
            }
          }
        }
      }
      return i;
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      if (this.IsZero)
        return "0";
      if (HasSmallValue()) {
        return SmallValueToString();
      }
      short[] tempReg = new short[this.wordCount];
      Array.Copy(this.reg, tempReg, tempReg.Length);
      int wordCount = tempReg.Length;
      while (wordCount != 0 && tempReg[wordCount - 1] == 0)
        wordCount--;
      int i = 0;
      char[] s = new char[(wordCount<<4)+1];
      while (wordCount != 0) {
        if (wordCount == 1 && tempReg[0] > 0 && tempReg[0] <=0x7FFF) {
          int rest = tempReg[0];
          while (rest != 0) {
            // accurate approximation to rest/10 up to 43698,
            // and rest can go up to 32767
            int newrest=(rest*26215)>>18;
            s[i++] = vec[rest-(newrest*10)];
            rest = newrest;
          }
          break;
        } else if (wordCount == 2 && tempReg[1] > 0 && tempReg[1] <=0x7FFF) {
          int rest = (((int)tempReg[0])&0xFFFF);
          rest|=(((int)tempReg[1])&0xFFFF)<<16;
          while (rest != 0) {
            int newrest=rest/10;
            s[i++] = vec[rest-(newrest*10)];
            rest = newrest;
          }
          break;
        } else {
          int wci = wordCount;
          short remainder = 0;
          int quo,rem;
          // Divide by 10000
          while ((wci--) > 0) {
            int currentDividend = unchecked((int)((((int)tempReg[wci]) & 0xFFFF) |
                                                  ((int)(remainder) << 16)));
            quo=currentDividend/10000;
            tempReg[wci] = unchecked((short)quo);
            rem=currentDividend-(10000*quo);
            remainder = unchecked((short)rem);
          }
          int remainderSmall=remainder;
          // Recalculate word count
          while (wordCount != 0 && tempReg[wordCount - 1] == 0)
            wordCount--;
          // accurate approximation to rest/10 up to 16388,
          // and rest can go up to 9999
          int newrest=(remainderSmall*3277)>>15;
          s[i++] = vec[(int)(remainderSmall-(newrest*10))];
          remainderSmall = newrest;
          newrest=(remainderSmall*3277)>>15;
          s[i++] = vec[(int)(remainderSmall-(newrest*10))];
          remainderSmall = newrest;
          newrest=(remainderSmall*3277)>>15;
          s[i++] = vec[(int)(remainderSmall-(newrest*10))];
          remainderSmall = newrest;
          s[i++] = vec[remainderSmall];
        }
      }
      ReverseChars(s, 0, i);
      if (this.negative) {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(i + 1);
        sb.Append('-');
        sb.Append(s, 0, i);
        return sb.ToString();
      } else {
        return new String(s, 0, i);
      }

    }

    /// <summary> </summary>
    /// <param name='str'>A string object.</param>
    /// <returns>A BigInteger object.</returns>
    public static BigInteger fromString(string str){
      if((str)==null)throw new ArgumentNullException("str");
      return fromSubstring(str,0,str.Length);
    }
    
    private const int MaxSafeInt = 214748363;


    public static BigInteger fromSubstring(string str, int index, int endIndex){
      if((str)==null)throw new ArgumentNullException("str");
      if((index)<0)throw new ArgumentException("\"str\""+" not greater or equal to "+"0"+" ("+Convert.ToString((long)(index),System.Globalization.CultureInfo.InvariantCulture)+")");
      if((index)>str.Length)throw new ArgumentException("\"str\""+" not less or equal to "+Convert.ToString((long)(str.Length),System.Globalization.CultureInfo.InvariantCulture)+" ("+Convert.ToString((long)(index),System.Globalization.CultureInfo.InvariantCulture)+")");
      if((endIndex)<0)throw new ArgumentException("\"index\""+" not greater or equal to "+"0"+" ("+Convert.ToString((long)(endIndex),System.Globalization.CultureInfo.InvariantCulture)+")");
      if((endIndex)>str.Length)throw new ArgumentException("\"index\""+" not less or equal to "+Convert.ToString((long)(str.Length),System.Globalization.CultureInfo.InvariantCulture)+" ("+Convert.ToString((long)(endIndex),System.Globalization.CultureInfo.InvariantCulture)+")");
      if((endIndex)<index)throw new ArgumentException("\"endIndex\""+" not greater or equal to "+Convert.ToString((long)(index),System.Globalization.CultureInfo.InvariantCulture)+" ("+Convert.ToString((long)(endIndex),System.Globalization.CultureInfo.InvariantCulture)+")");
      if(index==endIndex)
        throw new FormatException("No digits");
      bool negative=false;
      if(str[0]=='-'){
        index++;
        negative=true;
      }
      BigInteger bigint=new BigInteger().Allocate(4);
      bool haveDigits=false;
      bool haveSmallInt=true;
      int smallInt=0;
      for (int i = index; i < endIndex; i++) {
        char c=str[i];
        if(c<'0' || c>'9')throw new FormatException("Illegal character found");
        haveDigits=true;
        int digit = (int)(c - '0');
        if(haveSmallInt && smallInt<MaxSafeInt){
          smallInt*=10;
          smallInt+=digit;
        } else {
          if(haveSmallInt){
            bigint.reg[0]=unchecked((short)((smallInt)&0xFFFF));
            bigint.reg[1]=unchecked((short)((smallInt>>16)&0xFFFF));
            haveSmallInt=false;
          }
          // Multiply by 10
          short carry=LinearMultiply(bigint.reg,0,bigint.reg,0,(short)10,bigint.reg.Length);
          if(carry!=0)
            bigint.reg=GrowForCarry(bigint.reg,carry);
          // Add the parsed digit
          if(digit!=0 && Increment(bigint.reg,0,bigint.reg.Length,(short)digit)!=0)
            bigint.reg=GrowForCarry(bigint.reg,(short)1);
        }
      }
      if(!haveDigits)
        throw new FormatException("No digits");
      if(haveSmallInt){
        bigint.reg[0]=unchecked((short)((smallInt)&0xFFFF));
        bigint.reg[1]=unchecked((short)((smallInt>>16)&0xFFFF));
      }
      bigint.wordCount=bigint.CalcWordCount();
      bigint.negative=(bigint.wordCount!=0 && negative);
      return bigint;
    }

    /// <summary>Returns the greatest common divisor of two integers. </summary>
    /// <returns>A BigInteger object.</returns>
    /// <remarks>The greatest common divisor (GCD) is also known as the greatest
    /// common factor (GCF).</remarks>
    /// <param name='bigintSecond'>A BigInteger object.</param>
    public BigInteger gcd(BigInteger bigintSecond) {
      if ((bigintSecond) == null) throw new ArgumentNullException("bigintSecond");
      if (this.IsZero)
        return BigInteger.Abs(bigintSecond);
      if (bigintSecond.IsZero)
        return BigInteger.Abs(this);
      BigInteger thisValue = this.abs();
      bigintSecond = bigintSecond.abs();
      if (bigintSecond.Equals(BigInteger.One) ||
          thisValue.Equals(bigintSecond))
        return bigintSecond;
      if (thisValue.Equals(BigInteger.One))
        return thisValue;
      BigInteger temp;
      while (!thisValue.IsZero) {
        if (thisValue.CompareTo(bigintSecond) < 0) {
          temp = thisValue;
          thisValue = bigintSecond;
          bigintSecond = temp;
        }
        thisValue = thisValue % (BigInteger)bigintSecond;
      }
      return bigintSecond;
    }

    /// <summary> Calculates the remainder when a BigInteger raised to a
    /// certain power is divided by another BigInteger. </summary>
    /// <param name='pow'>A BigInteger object.</param>
    /// <param name='mod'>A BigInteger object.</param>
    /// <returns>A BigInteger object.</returns>
    public BigInteger ModPow(BigInteger pow, BigInteger mod) {
      if ((pow) == null) throw new ArgumentNullException("pow");
      if (pow.Sign < 0)
        throw new ArgumentException("pow is negative");
      BigInteger r = BigInteger.One;
      BigInteger v = this;
      while (!pow.IsZero) {
        if (!pow.IsEven) {
          r = (r * (BigInteger)v) % (BigInteger)mod;
        }
        pow >>= 1;
        if (!pow.IsZero) {
          v = (v * (BigInteger)v) % (BigInteger)mod;
        }
      }
      return r;
    }

    private static void PositiveAdd(BigInteger sum,
                                    BigInteger bigintAddend,
                                    BigInteger bigintAugend) {
      int carry;
      int addendCount=bigintAddend.wordCount+(bigintAddend.wordCount&1);
      int augendCount=bigintAugend.wordCount+(bigintAugend.wordCount&1);
      int desiredLength=Math.Max(addendCount,augendCount);
      if (addendCount == augendCount)
        carry = Add(sum.reg, 0, bigintAddend.reg, 0, bigintAugend.reg, 0, (int)(addendCount));
      else if (addendCount > augendCount) {
        // Addend is bigger
        carry = Add(sum.reg, 0,
                    bigintAddend.reg, 0,
                    bigintAugend.reg, 0,
                    (int)(augendCount));
        Array.Copy(
          bigintAddend.reg, augendCount,
          sum.reg, augendCount,
          addendCount - augendCount);
        carry = Increment(sum.reg, augendCount,
                          (int)(addendCount - augendCount),
                          (short)carry);
      } else {
        // Augend is bigger
        carry = Add(sum.reg, 0,
                    bigintAddend.reg, 0,
                    bigintAugend.reg, 0,
                    (int)(addendCount));
        Array.Copy(
          bigintAugend.reg, addendCount,
          sum.reg, addendCount,
          augendCount - addendCount);
        carry = Increment(sum.reg, addendCount,
                          (int)(augendCount - addendCount),
                          (short)carry);
      }
      if (carry != 0) {
        int nextIndex=desiredLength;
        int len = RoundupSize(nextIndex + 1);
        sum.reg = CleanGrow(sum.reg, len);
        sum.reg[nextIndex] = (short)carry;
      }
      sum.negative = false;
      sum.wordCount = sum.CalcWordCount();
      sum.ShortenArray();
    }

    static void PositiveSubtract(BigInteger diff,
                                 BigInteger minuend,
                                 BigInteger subtrahend) {
      int aSize = minuend.wordCount;
      aSize += aSize % 2;
      int bSize = subtrahend.wordCount;
      bSize += bSize % 2;

      if (aSize == bSize) {
        if (Compare(minuend.reg, 0, subtrahend.reg, 0, (int)aSize) >= 0) {
          // A is at least as high as B
          Subtract(diff.reg, 0, minuend.reg, 0, subtrahend.reg, 0, (int)aSize);
          diff.negative = false; // difference will not be negative at this point
        } else {
          // A is less than B
          Subtract(diff.reg, 0, subtrahend.reg, 0, minuend.reg, 0, (int)aSize);
          diff.negative = true; // difference will be negative
        }
      } else if (aSize > bSize) {
        // A is greater than B
        short borrow = (short)Subtract(diff.reg, 0, minuend.reg, 0, subtrahend.reg, 0, (int)bSize);
        Array.Copy(minuend.reg, bSize, diff.reg, bSize, aSize - bSize);
        borrow = (short)Decrement(diff.reg, bSize, (int)(aSize - bSize), borrow);
        //DebugAssert.IsTrue(borrow==0,"{0} line {1}: !borrow","integer.cpp",3524);
        diff.negative = false;
      } else {
        // A is less than B
        short borrow = (short)Subtract(diff.reg, 0, subtrahend.reg, 0, minuend.reg, 0, (int)aSize);
        Array.Copy(subtrahend.reg, aSize, diff.reg, aSize, bSize - aSize);
        borrow = (short)Decrement(diff.reg, aSize, (int)(bSize - aSize), borrow);
        //DebugAssert.IsTrue(borrow==0,"{0} line {1}: !borrow","integer.cpp",3532);
        diff.negative = true;
      }
      diff.wordCount = diff.CalcWordCount();
      diff.ShortenArray();
      if (diff.wordCount == 0) diff.negative = false;
    }

    #region Equals and GetHashCode implementation
    /// <inheritdoc/><summary>Determines whether this object and another
    /// object are equal.</summary>
    /// <returns>True if the objects are equal; false otherwise.</returns>
    /// <param name='obj'>An arbitrary object.</param>
    public override bool Equals(object obj) {
      BigInteger other = obj as BigInteger;
      if (other == null)
        return false;
      return other.CompareTo(this) == 0;
    }

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit hash code.</returns>
    public override int GetHashCode() {
      int hashCodeValue = 0;
      unchecked {
        hashCodeValue += 1000000007 * this.Sign.GetHashCode();
        if (reg != null) {
          for (int i = 0; i < wordCount; i++) {
            hashCodeValue += 1000000013 * reg[i];
          }
        }
      }
      return hashCodeValue;
    }
    #endregion

    /// <summary> Adds this object and another object.</summary>
    /// <returns>The sum of the two objects.</returns>
    /// <param name='bigintAugend'>A BigInteger object.</param>
    public BigInteger add(BigInteger bigintAugend) {
      if ((bigintAugend) == null) throw new ArgumentNullException("bigintAugend");
      BigInteger sum;
      if(this.wordCount==0)
        return bigintAugend;
      if(bigintAugend.wordCount==0)
        return this;
      if(bigintAugend.wordCount==1 && this.wordCount==1){
        if(this.negative==bigintAugend.negative){
          int intSum=(((int)this.reg[0])&0xFFFF)+(((int)(bigintAugend.reg[0]))&0xFFFF);
          sum=new BigInteger();
          sum.reg=new short[2];
          sum.reg[0]=unchecked((short)intSum);
          sum.reg[1]=unchecked((short)(intSum>>16));
          sum.wordCount=((intSum>>16)==0) ? 1 : 2;
          sum.negative=this.negative;
          return sum;
        } else {
          int a=(((int)this.reg[0])&0xFFFF);
          int b=(((int)(bigintAugend.reg[0]))&0xFFFF);
          if(a==b)return BigInteger.Zero;
          if(a>b){
            a-=b;
            sum=new BigInteger();
            sum.reg=new short[2];
            sum.reg[0]=unchecked((short)a);
            sum.wordCount=1;
            sum.negative=this.negative;
            return sum;
          } else {
            b-=a;
            sum=new BigInteger();
            sum.reg=new short[2];
            sum.reg[0]=unchecked((short)b);
            sum.wordCount=1;
            sum.negative=!this.negative;
            return sum;
          }
        }
      }
      sum = new BigInteger().Allocate((int)Math.Max(reg.Length, bigintAugend.reg.Length));
      if (this.Sign >= 0) {
        if (bigintAugend.Sign >= 0)
          PositiveAdd(sum, this, bigintAugend); // both nonnegative
        else
          PositiveSubtract(sum, this, bigintAugend); // this is nonnegative, b is negative
      } else {
        if (bigintAugend.Sign >= 0) {
          PositiveSubtract(sum, bigintAugend, this); // this is negative, b is nonnegative
        } else {
          PositiveAdd(sum, this, bigintAugend); // both are negative
          sum.negative = !sum.IsZero;
        }
      }
      return sum;
    }

    /// <summary> Subtracts a BigInteger from this BigInteger. </summary>
    /// <param name='subtrahend'>A BigInteger object.</param>
    /// <returns>The difference of the two objects.</returns>
    public BigInteger subtract(BigInteger subtrahend) {
      if ((subtrahend) == null) throw new ArgumentNullException("subtrahend");
      BigInteger diff = new BigInteger().Allocate((int)Math.Max(reg.Length, subtrahend.reg.Length));
      if (this.Sign >= 0) {
        if (subtrahend.Sign >= 0)
          PositiveSubtract(diff, this, subtrahend);
        else
          PositiveAdd(diff, this, subtrahend);
      } else {
        if (subtrahend.Sign >= 0) {
          PositiveAdd(diff, this, subtrahend);
          diff.negative = !diff.IsZero;
        } else {
          PositiveSubtract(diff, subtrahend, this);
        }
      }
      return diff;
    }

    private void ShortenArray(){
      if(this.reg.Length>32){
        int newLength=RoundupSize(this.wordCount);
        if(newLength<this.reg.Length &&
           (this.reg.Length-newLength)>=16){
          // Reallocate the array if the rounded length
          // is much smaller than the current length
          short[] newreg=new short[newLength];
          Array.Copy(this.reg,newreg,Math.Min(newLength,this.reg.Length));
          this.reg=newreg;
        }
      }
    }

    private static void PositiveMultiply(BigInteger product, BigInteger bigintA, BigInteger bigintB) {
      if(bigintA.wordCount==1){
        int wc=bigintB.wordCount;
        product.reg = new short[RoundupSize(wc+1)];
        product.reg[wc]=LinearMultiply(product.reg,0,bigintB.reg,0,bigintA.reg[0],wc);
        product.negative = false;
        product.wordCount = product.CalcWordCount();
        return;
      } else if(bigintB.wordCount==1){
        int wc=bigintA.wordCount;
        product.reg = new short[RoundupSize(wc+1)];
        product.reg[wc]=LinearMultiply(product.reg,0,bigintA.reg,0,bigintB.reg[0],wc);
        product.negative = false;
        product.wordCount = product.CalcWordCount();
        return;
      } else if (bigintA.Equals(bigintB)) {
        int aSize = RoundupSize(bigintA.wordCount);
        product.reg = new short[RoundupSize(aSize + aSize)];
        product.negative = false;
        short[] workspace = new short[aSize + aSize];
        Square(product.reg, 0,
               workspace, 0,
               bigintA.reg, 0, aSize);
      } else {
        int aSize = RoundupSize(bigintA.wordCount);
        int bSize = RoundupSize(bigintB.wordCount);
        product.reg = new short[RoundupSize(aSize + bSize)];
        product.negative = false;
        short[] workspace = new short[aSize + bSize];
        AsymmetricMultiply(product.reg, 0,
                           workspace, 0,
                           bigintA.reg, 0, aSize,
                           bigintB.reg, 0, bSize);
      }
      product.wordCount = product.CalcWordCount();
      product.ShortenArray();
    }

    /// <summary>Multiplies this instance by the value of a BigInteger object.</summary>
    /// <param name='bigintMult'>A BigInteger object.</param>
    /// <returns>The product of the two objects.</returns>
    public BigInteger multiply(BigInteger bigintMult) {
      if((bigintMult)==null)throw new ArgumentNullException("bigintMult");
      BigInteger product = new BigInteger();
      if(this.wordCount==0 || bigintMult.wordCount==0)
        return BigInteger.Zero;
      if(this.wordCount==1 && this.reg[0]==1)
        return this.negative ? bigintMult.negate() : bigintMult;
      if(bigintMult.wordCount==1 && bigintMult.reg[0]==1)
        return bigintMult.negative ? this.negate() : this;
      PositiveMultiply(product, this, bigintMult);
      if ((this.Sign >= 0) != (bigintMult.Sign >= 0))
        product.NegateInternal();
      return product;
    }

    private static int OperandLength(short[] a) {
      for (int i = a.Length - 1; i >= 0; i--) {
        if (a[i] != 0) return i + 1;
      }
      return 0;
    }

    private static void DivideWithRemainderAnyLength(
      short[] a,
      short[] b,
      short[] quotResult,
      short[] modResult
     ) {
      int lengthA = OperandLength(a);
      int lengthB = OperandLength(b);
      if (lengthB == 0) // check for zero on B first
        throw new DivideByZeroException("The divisor is zero.");
      if (lengthA == 0) { // 0 divided by X equals 0
        if (modResult != null)
          Array.Clear((short[])modResult, 0, modResult.Length);
        if (quotResult != null) // Set array to 0
          Array.Clear((short[])quotResult, 0, quotResult.Length);
        return;
      }
      if (lengthA < lengthB) {
        // If lengthA is less than lengthB, then
        // A is less than B, so set quotient to 0
        // and remainder to A
        if (modResult != null) {
          short[] tmpa = new short[a.Length];
          Array.Copy(a, tmpa, a.Length);
          Array.Clear((short[])modResult, 0, modResult.Length);
          Array.Copy(tmpa, modResult, Math.Min(tmpa.Length, modResult.Length));
        }
        if (quotResult != null) { // Set quotient to 0
          Array.Clear((short[])quotResult, 0, quotResult.Length);
        }
        return;
      }
      if (lengthA == 1 && lengthB == 1) {
        int a0 = ((int)a[0]) & 0xFFFF;
        int b0 = ((int)b[0]) & 0xFFFF;
        short result = unchecked((short)(a0 / b0));
        short mod = (modResult != null) ? unchecked((short)(a0 % b0)) : (short)0;
        if (quotResult != null) {
          Array.Clear((short[])quotResult, 0, quotResult.Length);
          quotResult[0] = result;
        }
        if (modResult != null) {
          Array.Clear((short[])modResult, 0, modResult.Length);
          modResult[0] = mod;
        }
        return;
      }
      lengthA += lengthA % 2;
      if (lengthA > a.Length) throw new InvalidOperationException("no room");
      lengthB += lengthB % 2;
      if (lengthB > b.Length) throw new InvalidOperationException("no room");
      short[] tempbuf = new short[lengthA + 3 * (lengthB + 2)];
      Divide(modResult, 0,
             quotResult, 0,
             tempbuf, 0,
             a, 0, lengthA,
             b, 0, lengthB);
      // Clear the area beyond the quotient in
      // the quotient array, in case the dividend
      // and the quotient are the same array
      if (quotResult != null) {
        int quotEnd = lengthA - lengthB + 2;
        Array.Clear((short[])quotResult, quotEnd, quotResult.Length - quotEnd);
      }
    }

    private static int BitsToWords(int bitCount) {
      return ((bitCount + 16 - 1) / (16));
    }

    private static short FastRemainder(short[] dividendReg, int count, short divisorSmall) {
      int i = count;
      short remainder = 0;
      while ((i--) > 0) {
        remainder = RemainderUnsigned(
          MakeUint(dividendReg[i], remainder), divisorSmall);
      }
      return remainder;
    }
    private static void FastDivide(short[] quotientReg, short[] dividendReg, int count, short divisorSmall) {
      int i = count;
      short remainder = 0;
      int idivisor = (((int)divisorSmall) & 0xFFFF);
      int quo,rem;
      while ((i--) > 0) {
        int currentDividend = unchecked((int)((((int)dividendReg[i]) & 0xFFFF) |
                                              ((int)(remainder) << 16)));
        if ((currentDividend >> 31) == 0) {
          quo=currentDividend / idivisor;
          quotientReg[i] = unchecked((short)quo);
          if(i>0){
            rem=currentDividend-(idivisor*quo);
            remainder = unchecked((short)rem);
          }
        } else {
          quotientReg[i] = DivideUnsigned(currentDividend, divisorSmall);
          if (i > 0) remainder = RemainderUnsigned(currentDividend, divisorSmall);
        }
      }
    }

    private static short FastDivideAndRemainderEx(short[] quotientReg,
                                                  short[] dividendReg, int count, short divisorSmall) {
      int i = count;
      short remainder = 0;
      int idivisor = (((int)divisorSmall) & 0xFFFF);
      int quo,rem;
      while ((i--) > 0) {
        int currentDividend = unchecked((int)((((int)dividendReg[i]) & 0xFFFF) |
                                              ((int)(remainder) << 16)));
        if ((currentDividend >> 31) == 0) {
          quo=currentDividend / idivisor;
          quotientReg[i] = unchecked((short)quo);
          rem=currentDividend-(idivisor*quo);
          remainder = unchecked((short)rem);
        } else {
          quotientReg[i] = DivideUnsigned(currentDividend, divisorSmall);
          remainder = RemainderUnsigned(currentDividend, divisorSmall);
        }
      }
      return remainder;
    }
    private static short FastDivideAndRemainder(short[] quotientReg, int count, short divisorSmall) {
      int i = count;
      short remainder = 0;
      int quo,rem;
      int idivisor = (((int)divisorSmall) & 0xFFFF);
      while ((i--) > 0) {
        int currentDividend = unchecked((int)((((int)quotientReg[i]) & 0xFFFF) |
                                              ((int)(remainder) << 16)));
        if ((currentDividend >> 31) == 0) {
          quo=currentDividend / idivisor;
          quotientReg[i] = unchecked((short)quo);
          rem=currentDividend-(idivisor*quo);
          remainder = unchecked((short)rem);
        } else {
          quotientReg[i] = DivideUnsigned(currentDividend, divisorSmall);
          remainder = RemainderUnsigned(currentDividend, divisorSmall);
        }
      }
      return remainder;
    }

    /// <summary>Divides this instance by the value of a BigInteger object.
    /// The result is rounded down (the fractional part is discarded). Except
    /// if the result is 0, it will be negative if this object is positive and
    /// the other is negative, or vice versa, and will be positive if both are
    /// positive or both are negative.</summary>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='DivideByZeroException'>The divisor is zero.</exception>
    /// <param name='bigintDivisor'>A BigInteger object.</param>
    public BigInteger divide(BigInteger bigintDivisor) {
      if((bigintDivisor)==null)throw new ArgumentNullException("bigintDivisor");
      int aSize = this.wordCount;
      int bSize = bigintDivisor.wordCount;
      if (bSize == 0)
        throw new DivideByZeroException();
      if (aSize < bSize || aSize==0) {
        // dividend is less than divisor, or dividend is 0
        return BigInteger.Zero;
      }
      if (aSize <= 2 && bSize <= 2 && this.HasTinyValue() && bigintDivisor.HasTinyValue()) {
        int aSmall = this.intValue();
        int  bSmall = bigintDivisor.intValue();
        if (aSmall != Int32.MinValue || bSmall != -1) {
          int result = aSmall / bSmall;
          return new BigInteger().InitializeInt(result);
        }
      }
      BigInteger quotient;
      if (bSize == 1) {
        // divisor is small, use a fast path
        quotient = new BigInteger();
        quotient.reg=new short[this.reg.Length];
        quotient.wordCount=this.wordCount;
        quotient.negative=this.negative;
        FastDivide(quotient.reg, this.reg, aSize, bigintDivisor.reg[0]);
        while (quotient.wordCount != 0 &&
               quotient.reg[quotient.wordCount - 1] == 0)
          quotient.wordCount--;
        if (quotient.wordCount != 0) {
          quotient.negative = this.negative ^ bigintDivisor.negative;
          return quotient;
        } else {
          return BigInteger.Zero;
        }
      }
      quotient = new BigInteger();
      aSize += aSize % 2;
      bSize += bSize % 2;
      quotient.reg = new short[RoundupSize((int)(aSize - bSize + 2))];
      quotient.negative = false;
      DivideWithRemainderAnyLength(this.reg, bigintDivisor.reg,
                                   quotient.reg, null);
      quotient.wordCount = quotient.CalcWordCount();
      quotient.ShortenArray();
      if ((this.Sign < 0) ^ (bigintDivisor.Sign < 0)) {
        quotient.NegateInternal();
      }
      return quotient;
    }

    /// <summary> </summary>
    /// <param name='divisor'>A BigInteger object.</param>
    /// <returns>A BigInteger[] object.</returns>
    public BigInteger[] divideAndRemainder(BigInteger divisor) {
      if ((divisor) == null) throw new ArgumentNullException("divisor");
      BigInteger quotient;
      int aSize = this.wordCount;
      int bSize = divisor.wordCount;
      if (bSize == 0)
        throw new DivideByZeroException();

      if (aSize < bSize) {
        // dividend is less than divisor
        return new BigInteger[] { BigInteger.Zero, this };
      }
      if (bSize == 1) {
        // divisor is small, use a fast path
        quotient = new BigInteger();
        quotient.reg=new short[this.reg.Length];
        quotient.wordCount=this.wordCount;
        quotient.negative=this.negative;
        int smallRemainder = (((int)FastDivideAndRemainderEx(
          quotient.reg, this.reg, aSize, divisor.reg[0])) & 0xFFFF);
        while (quotient.wordCount != 0 &&
               quotient.reg[quotient.wordCount - 1] == 0)
          quotient.wordCount--;
        quotient.ShortenArray();
        if (quotient.wordCount != 0) {
          quotient.negative = this.negative ^ divisor.negative;
        } else {
          quotient=BigInteger.Zero;
        }
        if (this.negative) smallRemainder = -smallRemainder;
        return new BigInteger[] { quotient, new BigInteger().InitializeInt(smallRemainder) };
      }
      BigInteger remainder = new BigInteger();
      quotient = new BigInteger();
      aSize += aSize & 1;
      bSize += bSize & 1;
      remainder.reg = new short[RoundupSize((int)bSize)];
      remainder.negative = false;
      quotient.reg = new short[RoundupSize((int)(aSize - bSize + 2))];
      quotient.negative = false;
      short[] tempbuf = new short[aSize + 3 * (bSize + 2)];
      Divide(remainder.reg, 0,
             quotient.reg, 0,
             tempbuf, 0,
             this.reg, 0, aSize,
             divisor.reg, 0, bSize);
      remainder.wordCount = remainder.CalcWordCount();
      quotient.wordCount = quotient.CalcWordCount();
      remainder.ShortenArray();
      quotient.ShortenArray();
      if (this.Sign < 0) {
        quotient.NegateInternal();
        if (!remainder.IsZero) {
          remainder.NegateInternal();
        }
      }
      if (divisor.Sign < 0)
        quotient.NegateInternal();
      return new BigInteger[] { quotient, remainder };
    }

    /// <summary> Finds the modulus remainder that results when this instance
    /// is divided by the value of a BigInteger object. The modulus remainder
    /// is the same as the normal remainder if the normal remainder is positive,
    /// and equals divisor minus normal remainder if the normal remainder
    /// is negative. </summary>
    /// <param name='divisor'>A divisor greater than 0.</param>
    /// <returns>A BigInteger object.</returns>
    public BigInteger mod(BigInteger divisor) {
      if((divisor)==null)throw new ArgumentNullException("divisor");
      if(divisor.Sign<0){
        throw new ArithmeticException("Divisor is negative");
      }
      BigInteger rem=this.remainder(divisor);
      if(rem.Sign<0)
        rem=divisor.subtract(rem);
      return rem;
    }

    /// <summary>Finds the remainder that results when this instance is
    /// divided by the value of a BigInteger object. The remainder is the value
    /// that remains when the absolute value of this object is divided by the
    /// absolute value of the other object; the remainder has the same sign
    /// (positive or negative) as this object.</summary>
    /// <param name='divisor'>A BigInteger object.</param>
    /// <returns>The remainder of the two objects.</returns>
    public BigInteger remainder(BigInteger divisor) {
      if (this.PositiveCompare(divisor) < 0) {
        if (divisor.IsZero) throw new DivideByZeroException();
        return this;
      }
      BigInteger remainder = new BigInteger();
      int aSize = this.wordCount;
      int bSize = divisor.wordCount;
      if (bSize == 0)
        throw new DivideByZeroException();
      if (aSize < bSize) {
        // dividend is less than divisor
        return this;
      }
      if (bSize == 1) {
        short shortRemainder = FastRemainder(this.reg, this.wordCount, divisor.reg[0]);
        int smallRemainder = (((int)shortRemainder) & 0xFFFF);
        if (this.Sign < 0) smallRemainder = -smallRemainder;
        return new BigInteger().InitializeInt(smallRemainder);
      }
      aSize += aSize % 2;
      bSize += bSize % 2;
      remainder.reg = new short[RoundupSize((int)bSize)];
      remainder.negative = false;
      short[] quotientReg = new short[RoundupSize((int)(aSize - bSize + 2))];
      DivideWithRemainderAnyLength(this.reg, divisor.reg, quotientReg, remainder.reg);
      remainder.wordCount = remainder.CalcWordCount();
      remainder.ShortenArray();
      if (this.Sign < 0 && !remainder.IsZero) {
        remainder.NegateInternal();
      }
      return remainder;
    }

    void NegateInternal() {
      if (this.wordCount != 0)
        this.negative = (this.Sign > 0);
    }

    int PositiveCompare(BigInteger t) {
      int size = this.wordCount, tSize = t.wordCount;
      if (size == tSize)
        return Compare(this.reg, 0, t.reg, 0, (int)size);
      else
        return size > tSize ? 1 : -1;
    }

    /// <summary>Compares a BigInteger object with this instance.</summary>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    /// <param name='other'>A BigInteger object.</param>
    public int CompareTo(BigInteger other) {
      if (other == null) return 1;
      if(this==other)return 0;
      int size = this.wordCount, tSize = other.wordCount;
      int sa = (size == 0 ? 0 : (this.negative ? -1 : 1));
      int sb = (tSize == 0 ? 0 : (other.negative ? -1 : 1));
      if (sa != sb) return (sa < sb) ? -1 : 1;
      if (sa == 0) return 0;
      int cmp = 0;
      if (size == tSize){
        if(size==1 && this.reg[0]==other.reg[0]){
          return 0;
        } else {
          cmp = Compare(this.reg, 0, other.reg, 0, (int)size);
        }
      } else {
        cmp = size > tSize ? 1 : -1;
      }
      return (sa > 0) ? cmp : -cmp;
    }
    /// <summary> </summary>
    public int Sign {
      get {
        if (this.wordCount == 0)
          return 0;
        return (this.negative) ? -1 : 1;
      }
    }

    /// <summary> </summary>
    public bool IsZero {
      get { return (this.wordCount == 0); }
    }

    /// <summary> Finds the square root of this instance's value.</summary>
    /// <returns>The square root of this object&apos;s value. Returns 0
    /// if this value is 0 or less.</returns>
    public BigInteger sqrt() {
      if (this.Sign <= 0)
        return BigInteger.Zero;
      BigInteger bigintX = null;
      BigInteger bigintY = Power2((getUnsignedBitLength() + 1) / 2);
      do {
        bigintX = bigintY;
        bigintY = this / (BigInteger)bigintX;
        bigintY += bigintX;
        bigintY >>= 1;
      } while (bigintY.CompareTo(bigintX) < 0);
      return bigintX;
    }
    /// <summary> Gets whether this value is even. </summary>
    public bool IsEven { get { return !GetUnsignedBit(0); } }

    /// <summary> BigInteger object for the number zero.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security","CA2104",
      Justification="BigInteger is immutable")]
    #endif
    public static readonly BigInteger ZERO = new BigInteger().InitializeInt(0);
    /// <summary> BigInteger object for the number one. </summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security","CA2104",
      Justification="BigInteger is immutable")]
    #endif
    public static readonly BigInteger ONE = new BigInteger().InitializeInt(1);
    /// <summary> BigInteger object for the number ten. </summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security","CA2104",
      Justification="BigInteger is immutable")]
    #endif
    public static readonly BigInteger TEN = new BigInteger().InitializeInt(10);
  }
}
