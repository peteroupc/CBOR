/*
Written in 2013 by Peter O.

Parts of the code were adapted by Peter O. from
the public-domain code from the library
CryptoPP by Wei Dai.

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

    private static short ShiftWordsLeftByBits(short[] r, int rstart, int n, int shiftBits) {
      #if DEBUG
      if (!(shiftBits < 16)) { throw new ArgumentException("doesn't satisfy shiftBits<16"); }
      #endif

      unchecked {
        short u, carry = 0;
        if (shiftBits != 0) {
          for (int i = 0; i < n; ++i) {
            u = r[rstart + i];
            r[rstart + i] = (short)((int)(u << (int)shiftBits) | (((int)carry) & 0xFFFF));
            carry = (short)((((int)u) & 0xFFFF) >> (int)(16 - shiftBits));
          }
        }
        return carry;
      }
    }

    private static short ShiftWordsRightByBits(short[] r, int rstart, int n, int shiftBits) {
      // DebugAssert.IsTrue(shiftBits<16,"{0} line {1}: shiftBits<16","words.h",67);
      short u, carry = 0;
      unchecked {
        if (shiftBits != 0)
          for (int i = n; i > 0; --i) {
          u = r[rstart + i - 1];
          r[rstart + i - 1] = (short)((((((int)u) & 0xFFFF) >> (int)shiftBits) & 0xFFFF) | (((int)carry) & 0xFFFF));
          carry = (short)((((int)u) & 0xFFFF) << (int)(16 - shiftBits));
        }
        return carry;
      }
    }

    private static short ShiftWordsRightByBitsSignExtend(short[] r, int rstart, int n, int shiftBits) {
      // DebugAssert.IsTrue(shiftBits<16,"{0} line {1}: shiftBits<16","words.h",67);
      unchecked {
        short u, carry = (short)((int)0xFFFF << (int)(16 - shiftBits));
        if (shiftBits != 0)
          for (int i = n; i > 0; --i) {
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
        for (int i = n - 1; i >= shiftWords; --i)
          r[rstart + i] = r[rstart + i - shiftWords];
        Array.Clear((short[])r, rstart, shiftWords);
      }
    }

    private static void ShiftWordsRightByWords(short[] r, int rstart, int n, int shiftWords) {
      shiftWords = Math.Min(shiftWords, n);
      if (shiftWords != 0) {
        for (int i = 0; i + shiftWords < n; ++i)
          r[rstart + i] = r[rstart + i + shiftWords];
        rstart = rstart + n - shiftWords;
        Array.Clear((short[])r, rstart, shiftWords);
      }
    }

    private static void ShiftWordsRightByWordsSignExtend(short[] r, int rstart, int n, int shiftWords) {
      shiftWords = Math.Min(shiftWords, n);
      if (shiftWords != 0) {
        for (int i = 0; i + shiftWords < n; ++i)
          r[rstart + i] = r[rstart + i + shiftWords];
        rstart = rstart + n - shiftWords;
        // Sign extend
        for (int i = 0; i < shiftWords; ++i)
          r[rstart + i] = unchecked((short)0xFFFF);
      }
    }

    private static int Compare(short[] A, int astart, short[] B, int bstart, int N) {
      while (unchecked(N--) != 0) {
        int an = ((int)A[astart + N]) & 0xFFFF;
        int bn = ((int)B[bstart + N]) & 0xFFFF;
        if (an > bn)
          return 1;
        else if (an < bn)
          return -1;
      }
      return 0;
    }

    private static int Increment(short[] A, int Astart, int N, short B) {
      unchecked {
        // DebugAssert.IsTrue(N!=0,"{0} line {1}: N","integer.cpp",63);
        short tmp = A[Astart];
        A[Astart] = (short)(tmp + B);
        if ((((int)A[Astart]) & 0xFFFF) >= (((int)tmp) & 0xFFFF))
          return 0;
        for (int i = 1; i < N; ++i) {
          A[Astart + i]++;
          if (A[Astart + i] != 0)
            return 0;
        }
        return 1;
      }
    }

    private static int Decrement(short[] A, int Astart, int N, short B) {
      // DebugAssert.IsTrue(N!=0,"{0} line {1}: N","integer.cpp",76);
      unchecked {
        short tmp = A[Astart];
        A[Astart] = (short)(tmp - B);
        if ((((int)A[Astart]) & 0xFFFF) <= (((int)tmp) & 0xFFFF))
          return 0;
        for (int i = 1; i < N; ++i) {
          tmp = A[Astart + i];
          A[Astart + i]--;
          if (tmp != 0)
            return 0;
        }
        return 1;
      }
    }

    private static void TwosComplement(short[] A, int Astart, int N) {
      Decrement(A, Astart, N, (short)1);
      for (int i = 0; i < N; ++i)
        A[Astart + i] = unchecked((short)(~A[Astart + i]));
    }

    private static int Add(
      short[] C, int cstart,
      short[] A, int astart,
      short[] B, int bstart, int N) {
      // DebugAssert.IsTrue(N%2 == 0,"{0} line {1}: N%2 == 0","integer.cpp",799);
      unchecked {

        int u;
        u = 0;
        for (int i = 0; i < N; i += 2) {
          u = (((int)A[astart + i]) & 0xFFFF) + (((int)B[bstart + i]) & 0xFFFF) + (short)(u >> 16);
          C[cstart + i] = (short)u;
          u = (((int)A[astart + i + 1]) & 0xFFFF) + (((int)B[bstart + i + 1]) & 0xFFFF) + (short)(u >> 16);
          C[cstart + i + 1] = (short)u;
        }
        return ((int)u >> 16) & 0xFFFF;
      }
    }

    private static int Subtract(
      short[] C, int cstart,
      short[] A, int astart,
      short[] B, int bstart, int N) {
      // DebugAssert.IsTrue(N%2 == 0,"{0} line {1}: N%2 == 0","integer.cpp",799);
      unchecked {
        int u;
        u = 0;
        for (int i = 0; i < N; i += 2) {
          u = (((int)A[astart]) & 0xFFFF) - (((int)B[bstart]) & 0xFFFF) - (int)((u >> 31) & 1);
          C[cstart++] = (short)u;
          astart++;
          bstart++;
          u = (((int)A[astart]) & 0xFFFF) - (((int)B[bstart]) & 0xFFFF) - (int)((u >> 31) & 1);
          C[cstart++] = (short)u;
          astart++;
          bstart++;
        }
        return (int)((u >> 31) & 1);
      }
    }

    private static short LinearMultiply(
short[] productArr, int cstart,
                                        short[] A, int astart, short B, int N) {
      unchecked {
        short carry = 0;
        int Bint = ((int)B) & 0xFFFF;
        for (int i = 0; i < N; ++i) {
          int p;
          p = (((int)A[astart + i]) & 0xFFFF) * Bint;
          p = p + (((int)carry) & 0xFFFF);
          productArr[cstart + i] = (short)p;
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
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart]) & 0xFFFF); R[rstart] = (short)p; e = ((int)p >> 16) & 0xFFFF;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 1]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 4- 3] = c;
        p = (((int)A[astart + 2 - 1]) & 0xFFFF) * (((int)A[astart + 2 - 1]) & 0xFFFF);
        p += e; R[rstart + 4 -  2] = (short)p; R[rstart + 4 -  1] = (short)(p >> 16);
      }
    }

    private static void Baseline_Square4(short[] R, int rstart, short[] A, int astart) {
      unchecked {
        int p; short c; int d; int e;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart]) & 0xFFFF); R[rstart] = (short)p; e = ((int)p >> 16) & 0xFFFF;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 1]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 1] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 2]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 2] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 3] = c;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 4] = c;
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 2 * 4 - 3] = c;
        p = (((int)A[astart + 4 - 1]) & 0xFFFF) * (((int)A[astart + 4 - 1]) & 0xFFFF);
        p += e; R[rstart + 2 * 4 - 2] = (short)p; R[rstart + 2 * 4 - 1] = (short)(p >> 16);
      }
    }

    private static void Baseline_Square8(short[] R, int rstart, short[] A, int astart) {
      unchecked {
        int p; short c; int d; int e;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart]) & 0xFFFF); R[rstart] = (short)p; e = ((int)p >> 16) & 0xFFFF;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 1]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 1] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 2]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 2] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 3] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 4]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 4] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 5] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 6] = c;
        p = (((int)A[astart]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 7] = c;
        p = (((int)A[astart + 1]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 8] = c;
        p = (((int)A[astart + 2]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 9] = c;
        p = (((int)A[astart + 3]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 10] = c;
        p = (((int)A[astart + 4]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 11] = c;
        p = (((int)A[astart + 5]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)A[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 12] = c;
        p = (((int)A[astart + 6]) & 0xFFFF) * (((int)A[astart + 7]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 2 * 8 - 3] = c;
        p = (((int)A[astart + 8 - 1]) & 0xFFFF) * (((int)A[astart + 8 - 1]) & 0xFFFF);
        p += e; R[rstart + 2 * 8 - 2] = (short)p; R[rstart + 2 * 8 - 1] = (short)(p >> 16);
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
        int a0 = ((int)A[astart]) & 0xFFFF;
        int a1 = ((int)A[astart + 1]) & 0xFFFF;
        int b0 = ((int)B[bstart]) & 0xFFFF;
        int b1 = ((int)B[bstart + 1]) & 0xFFFF;
        p = a0 * b0; c = (short)p; d = ((int)p >> 16) & 0xFFFF; R[rstart] = c; c = (short)d; d = (((int)d >> 16) & 0xFFFF);
        p = a0 * b1;
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = a1 * b0;
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 1] = c;
        p = a1 * b1;
        p += d; R[rstart + 1 + 1] = (short)p; R[rstart + 1 + 2] = (short)(p >> 16);
      }
    }

    private static void Baseline_Multiply4(short[] R, int rstart, short[] A, int astart, short[] B, int bstart) {
      int mask = 0xFFFF;
      unchecked {
        int p; short c; int d;
        int a0 = ((int)A[astart]) &  mask;
        int b0 = ((int)B[bstart]) &  mask;
        p = a0 * b0; c = (short)p; d = ((int)p >> 16) & mask; R[rstart] = c; c = (short)d; d = (((int)d >> 16) & mask);
        p = a0 * (((int)B[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 1]) & mask) * b0;
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 1] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = a0 * (((int)B[bstart + 2]) & mask);

        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 1]) & mask) * (((int)B[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 2]) & mask) * b0;
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 2] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = a0 * (((int)B[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 1]) & mask) * (((int)B[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);

        p = (((int)A[astart + 2]) & mask) * (((int)B[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 3]) & mask) * b0;
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 3] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)A[astart + 1]) & mask) * (((int)B[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 2]) & mask) * (((int)B[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 3]) & mask) * (((int)B[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 4] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)A[astart + 2]) & mask) * (((int)B[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 3]) & mask) * (((int)B[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 5] = c;
        p = (((int)A[astart + 3]) & mask) * (((int)B[bstart + 3]) & mask);
        p += d; R[rstart + 5 + 1] = (short)p; R[rstart + 5 + 2] = (short)(p >> 16);
      }
    }

    private static void Baseline_Multiply8(short[] R, int rstart, short[] A, int astart, short[] B, int bstart) {
      int mask = 0xFFFF;
      unchecked {
        int p; short c; int d;
        p = (((int)A[astart]) & mask) * (((int)B[bstart]) & mask); c = (short)p; d = ((int)p >> 16) & mask; R[rstart] = c; c = (short)d; d = (((int)d >> 16) & mask);
        p = (((int)A[astart]) & mask) * (((int)B[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 1]) & mask) * (((int)B[bstart]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 1] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)A[astart]) & mask) * (((int)B[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 1]) & mask) * (((int)B[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 2]) & mask) * (((int)B[bstart]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 2] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)A[astart]) & mask) * (((int)B[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 1]) & mask) * (((int)B[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 2]) & mask) * (((int)B[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 3]) & mask) * (((int)B[bstart]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 3] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)A[astart]) & mask) * (((int)B[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 1]) & mask) * (((int)B[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 2]) & mask) * (((int)B[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 3]) & mask) * (((int)B[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 4]) & mask) * (((int)B[bstart]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 4] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)A[astart]) & mask) * (((int)B[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 1]) & mask) * (((int)B[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 2]) & mask) * (((int)B[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 3]) & mask) * (((int)B[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 4]) & mask) * (((int)B[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 5]) & mask) * (((int)B[bstart]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 5] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)A[astart]) & mask) * (((int)B[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 1]) & mask) * (((int)B[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 2]) & mask) * (((int)B[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 3]) & mask) * (((int)B[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 4]) & mask) * (((int)B[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 5]) & mask) * (((int)B[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 6]) & mask) * (((int)B[bstart]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 6] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)A[astart]) & mask) * (((int)B[bstart + 7]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 1]) & mask) * (((int)B[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 2]) & mask) * (((int)B[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 3]) & mask) * (((int)B[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 4]) & mask) * (((int)B[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 5]) & mask) * (((int)B[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 6]) & mask) * (((int)B[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 7]) & mask) * (((int)B[bstart]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 7] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)A[astart + 1]) & mask) * (((int)B[bstart + 7]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 2]) & mask) * (((int)B[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 3]) & mask) * (((int)B[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 4]) & mask) * (((int)B[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 5]) & mask) * (((int)B[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 6]) & mask) * (((int)B[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 7]) & mask) * (((int)B[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 8] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)A[astart + 2]) & mask) * (((int)B[bstart + 7]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 3]) & mask) * (((int)B[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 4]) & mask) * (((int)B[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 5]) & mask) * (((int)B[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 6]) & mask) * (((int)B[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 7]) & mask) * (((int)B[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 9] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)A[astart + 3]) & mask) * (((int)B[bstart + 7]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 4]) & mask) * (((int)B[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 5]) & mask) * (((int)B[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 6]) & mask) * (((int)B[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 7]) & mask) * (((int)B[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 10] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)A[astart + 4]) & mask) * (((int)B[bstart + 7]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 5]) & mask) * (((int)B[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 6]) & mask) * (((int)B[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 7]) & mask) * (((int)B[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 11] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)A[astart + 5]) & mask) * (((int)B[bstart + 7]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 6]) & mask) * (((int)B[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 7]) & mask) * (((int)B[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 12] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)A[astart + 6]) & mask) * (((int)B[bstart + 7]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)A[astart + 7]) & mask) * (((int)B[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 13] = c;
        p = (((int)A[astart + 7]) & mask) * (((int)B[bstart + 7]) & mask);
        p += d; R[rstart + 13 + 1] = (short)p; R[rstart + 13 + 2] = (short)(p >> 16);
      }
    }

    #endregion
    private const int s_recursionLimit = 8;

    private static void RecursiveMultiply(
      short[] Rarr,  // size 2*N
      int Rstart,
      short[] Tarr,  // size 2*N
      int Tstart,
      short[] Aarr, int Astart,  // size N
      short[] Barr, int Bstart,  // size N
      int N) {
      if (N <= s_recursionLimit) {
        N >>= 2;
        if (N == 0) {
          Baseline_Multiply2(Rarr, Rstart, Aarr, Astart, Barr, Bstart);
        } else if (N == 1) {
          Baseline_Multiply4(Rarr, Rstart, Aarr, Astart, Barr, Bstart);
        } else if (N == 2) {
          Baseline_Multiply8(Rarr, Rstart, Aarr, Astart, Barr, Bstart);
        } else {
          throw new InvalidOperationException();
        }
      } else {
        int N2 = N >> 1;
        int rMediumHigh = Rstart + N;
        int rHigh = rMediumHigh + N2;
        int rMediumLow = Rstart + N2;
        int tsn = Tstart + N;
        int AN = N;
        while (AN != 0 && Aarr[Astart + AN - 1] == 0)AN--;
        int BN = N;
        while (BN != 0 && Barr[Bstart + BN - 1] == 0)BN--;
        int AN2 = 0;
        int BN2 = 0;
        if (AN == 0 || BN == 0) {
          // A or B is empty, so result is 0
          Array.Clear((short[])Rarr, Rstart, N << 1);
          return;
        }
        if (AN <= N2 && BN <= N2) {
          // Console.WriteLine("Can be smaller: {0},{1},{2}",AN,BN,N2);
          Array.Clear((short[])Rarr, Rstart + N, N);
          if (N2 == 8)
            Baseline_Multiply8(Rarr, Rstart, Aarr, Astart, Barr, Bstart);
          else
            RecursiveMultiply(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, Barr, Bstart, N2);
          return;
        }
        AN2 = Compare(Aarr, Astart, Aarr, (int)(Astart + N2), N2) > 0 ? 0 : N2;
        Subtract(Rarr, Rstart, Aarr, (int)(Astart + AN2), Aarr, (int)(Astart + (N2 ^ AN2)), N2);
        BN2 = Compare(Barr, Bstart, Barr, (int)(Bstart + N2), N2) > 0 ? 0 : N2;
        Subtract(Rarr, rMediumLow, Barr, (int)(Bstart + BN2), Barr, (int)(Bstart + (N2 ^ BN2)), N2);
        //---------
        // Medium high result = HighA * HighB
        RecursiveMultiply(Rarr, rMediumHigh, Tarr, tsn, Aarr, (int)(Astart + N2), Barr, (int)(Bstart + N2), N2);
        // Medium high result = Abs(LowA-HighA) * Abs(LowB-HighB)
        RecursiveMultiply(Tarr, Tstart, Tarr, tsn, Rarr, Rstart, Rarr, (int)rMediumLow, N2);
        // Low result = LowA * LowB
        RecursiveMultiply(Rarr, Rstart, Tarr, tsn, Aarr, Astart, Barr, Bstart, N2);
        //
        int c2 = Add(Rarr, rMediumHigh, Rarr, rMediumHigh, Rarr, rMediumLow, N2);
        int c3 = c2;
        c2 += Add(Rarr, rMediumLow, Rarr, rMediumHigh, Rarr, Rstart, N2);
        c3 += Add(Rarr, rMediumHigh, Rarr, rMediumHigh, Rarr, rHigh, N2);
        if (AN2 == BN2)
          c3 -= Subtract(Rarr, rMediumLow, Rarr, rMediumLow, Tarr, Tstart, N);
        else
          c3 += Add(Rarr, rMediumLow, Rarr, rMediumLow, Tarr, Tstart, N);
        c3 += Increment(Rarr, rMediumHigh, N2, (short)c2);
        if (c3 != 0)
          Increment(Rarr, rHigh, N2, (short)c3);
      }
    }

    private static void RecursiveSquare(
short[] Rarr,
                                        int Rstart,
                                        short[] Tarr,
                                        int Tstart, short[] Aarr, int Astart, int N) {
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
          default:
            throw new InvalidOperationException();
        }
      } else {
        int N2 = N >> 1;

        RecursiveSquare(Rarr, Rstart, Tarr, (int)(Tstart + N), Aarr, Astart, N2);
        RecursiveSquare(Rarr, (int)(Rstart + N), Tarr, (int)(Tstart + N), Aarr, (int)(Astart + N2), N2);
        RecursiveMultiply(
Tarr, Tstart, Tarr, (int)(Tstart + N),
                          Aarr, Astart, Aarr, (int)(Astart + N2), N2);

        int carry = Add(Rarr, (int)(Rstart + N2), Rarr, (int)(Rstart + N2), Tarr, Tstart, N);
        carry += Add(Rarr, (int)(Rstart + N2), Rarr, (int)(Rstart + N2), Tarr, Tstart, N);

        Increment(Rarr, (int)(Rstart + N + N2), N2, (short)carry);
      }
    }

    private static void SchoolbookMultiply(
      short[] Rarr, int Rstart,
      short[] Aarr, int Astart, int NA, short[] Barr, int Bstart, int NB) {
      // Method assumes that Rarr was already zeroed
      int cstart;
      if (NA < NB) {
        // A is shorter than B, so put B on top
        for (int i = 0; i < NA; ++i) {
          cstart = Rstart + i;
          unchecked {
            short carry = 0;
            int Bint = ((int)Aarr[Astart + i]) & 0xFFFF;
            for (int j = 0; j < NB; ++j) {
              int p;
              p = (((int)Barr[Bstart + j]) & 0xFFFF) * Bint;
              p = p + (((int)carry) & 0xFFFF);
              if (i != 0)p += ((int)Rarr[cstart + j]) & 0xFFFF;
              Rarr[cstart + j] = (short)p;
              carry = (short)(p >> 16);
            }
            Rarr[cstart + NB] = carry;
          }
        }
      } else {
        // B is shorter than A
        for (int i = 0; i < NB; ++i) {
          cstart = Rstart + i;
          unchecked {
            short carry = 0;
            int Bint = ((int)Barr[Bstart + i]) & 0xFFFF;
            for (int j = 0; j < NA; ++j) {
              int p;
              p = (((int)Aarr[Astart + j]) & 0xFFFF) * Bint;
              p = p + (((int)carry) & 0xFFFF);
              if (i != 0)p += ((int)Rarr[cstart + j]) & 0xFFFF;
              Rarr[cstart + j] = (short)p;
              carry = (short)(p >> 16);
            }
            Rarr[cstart + NA] = carry;
          }
        }
      }
    }

    private static void AsymmetricMultiply(
      short[] Rarr,
      int Rstart,
      short[] Tarr,
      int Tstart, short[] Aarr, int Astart, int NA, short[] Barr, int Bstart, int NB) {
      if (NA == NB) {
        if (Astart == Bstart && Aarr == Barr) {
          RecursiveSquare(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, NA);
        } else if (NA == 2)
          Baseline_Multiply2(Rarr, Rstart, Aarr, Astart, Barr, Bstart);
        else
          RecursiveMultiply(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, Barr, Bstart, NA);

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
            Array.Clear((short[])Rarr, Rstart, NB + 2);
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
        int a0 = ((int)Aarr[Astart]) & 0xFFFF;
        int a1 = ((int)Aarr[Astart + 1]) & 0xFFFF;
        Rarr[Rstart + NB] = (short)0;
        Rarr[Rstart + NB + 1] = (short)0;
        AtomicMultiplyOpt(Rarr, Rstart, a0, a1, Barr, Bstart, 0, NB);
        AtomicMultiplyAddOpt(Rarr, Rstart, a0, a1, Barr, Bstart, 2, NB);
        return;
      } else {
        int i;
        if (((NB / NA) & 1) == 0) {
          RecursiveMultiply(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, Barr, Bstart, NA);
          Array.Copy(Rarr, (int)(Rstart + NA), Tarr, (int)(Tstart + (NA << 1)), (int)NA);
          for (i = NA << 1; i < NB; i += NA << 1)
            RecursiveMultiply(Tarr, (int)(Tstart + NA + i), Tarr, Tstart, Aarr, Astart, Barr, (int)(Bstart + i), NA);
          for (i = NA; i < NB; i += NA << 1)
            RecursiveMultiply(Rarr, (int)(Rstart + i), Tarr, Tstart, Aarr, Astart, Barr, (int)(Bstart + i), NA);
        } else {
          for (i = 0; i < NB; i += NA << 1)
            RecursiveMultiply(Rarr, (int)(Rstart + i), Tarr, Tstart, Aarr, Astart, Barr, (int)(Bstart + i), NA);
          for (i = NA; i < NB; i += NA << 1)
            RecursiveMultiply(Tarr, (int)(Tstart + NA + i), Tarr, Tstart, Aarr, Astart, Barr, (int)(Bstart + i), NA);
        }
        if (Add(Rarr, (int)(Rstart + NA), Rarr, (int)(Rstart + NA), Tarr, (int)(Tstart + (NA << 1)), NB - NA) != 0)
          Increment(Rarr, (int)(Rstart + NB), NA, (short)1);
      }
    }

    private static int MakeUint(short first, short second) {
      return unchecked((int)((((int)first) & 0xFFFF) | ((int)second << 16)));
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
      int i = 16;
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
      int i = 32;
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

    private static short Divide32By16(int dividendLow, short divisorShort, bool returnRemainder) {
      int tmpInt;
      int dividendHigh = 0;
      int intDivisor = ((int)divisorShort) & 0xFFFF;
      for (int i = 0; i < 32; ++i) {
        tmpInt = dividendHigh >> 31;
        dividendHigh <<= 1;
        dividendHigh = unchecked((int)(dividendHigh | ((int)((dividendLow >> 31) & 1))));
        dividendLow <<= 1;
        tmpInt |= dividendHigh;
        // unsigned greater-than-or-equal check
        if (((tmpInt >> 31) != 0) || (tmpInt >= intDivisor)) {
          unchecked {
            dividendHigh -= intDivisor;
            dividendLow += 1;
          }
        }
      }
      return (returnRemainder ?
              unchecked((short)(((int)dividendHigh) & 0xFFFF)) :
              unchecked((short)(((int)dividendLow) & 0xFFFF))
             );
    }

    private static short DivideUnsigned(int x, short y) {
      unchecked {
        int iy = ((int)y) & 0xFFFF;
        if ((x >> 31) == 0) {
          // x is already nonnegative
          return (short)(((int)x / iy) & 0xFFFF);
        } else {
          return Divide32By16(x, y, false);
        }
      }
    }

    private static short RemainderUnsigned(int x, short y) {
      unchecked {
        int iy = ((int)y) & 0xFFFF;
        if ((x >> 31) == 0) {
          // x is already nonnegative
          return (short)(((int)x % iy) & 0xFFFF);
        } else {
          return Divide32By16(x, y, true);
        }
      }
    }

    private static short DivideThreeWordsByTwo(short[] A, int Astart, short B0, short B1) {
      // DebugAssert.IsTrue(A[2] < B1 || (A[2]==B1 && A[1] < B0),"{0} line {1}: A[2] < B1 || (A[2]==B1 && A[1] < B0)","integer.cpp",360);
      short Q;
      unchecked {
        if ((short)(B1 + 1) == 0)
          Q = A[Astart + 2];
        else if (B1 != 0)
          Q = DivideUnsigned(MakeUint(A[Astart + 1], A[Astart + 2]), (short)(((int)B1 + 1) & 0xFFFF));
        else
          Q = DivideUnsigned(MakeUint(A[Astart], A[Astart + 1]), B0);

        int Qint = ((int)Q) & 0xFFFF;
        int B0int = ((int)B0) & 0xFFFF;
        int B1int = ((int)B1) & 0xFFFF;
        int p = B0int * Qint;
        int u = (((int)A[Astart]) & 0xFFFF) - (p & 0xFFFF);
        A[Astart] = GetLowHalf(u);
        u = (((int)A[Astart + 1]) & 0xFFFF) - ((p >> 16) & 0xFFFF) -
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

        }
      }
      return Q;
    }

    private static void AtomicDivide(
short[] Q, int Qstart, short[] A, int Astart,
                                     short B0, short B1, short[] T) {
      if (B0 == 0 && B1 == 0) {
        Q[Qstart] = A[Astart];
        Q[Qstart + 1] = A[Astart + 3];
      } else {
        T[0] = A[Astart];
        T[1] = A[Astart + 1];
        T[2] = A[Astart + 2];
        T[3] = A[Astart + 3];
        short Q1 = DivideThreeWordsByTwo(T, 1, B0, B1);
        short Q0 = DivideThreeWordsByTwo(T, 0, B0, B1);
        Q[Qstart] = Q0;
        Q[Qstart + 1] = Q1;
      }
    }

    private static void AtomicMultiplyOpt(short[] C, int Cstart, int A0, int A1, short[] B, int Bstart, int istart, int iend)
    {
      short s;
      int d;
      int a1MinusA0 = ((int)A1 - A0) & 0xFFFF;
      A1 &= 0xFFFF;
      A0 &= 0xFFFF;
      unchecked {
        if (A1 >= A0) {
          for (int i = istart; i < iend; i += 4) {
            int B0 = ((int)B[Bstart + i]) & 0xFFFF;
            int B1 = ((int)B[Bstart + i + 1]) & 0xFFFF;
            int csi = Cstart + i;
            if (B0 >= B1)
            {
              s = (short)0;
              d = a1MinusA0 * (((int)B0 - B1) & 0xFFFF);
            } else {
              s = (short)a1MinusA0;
              d = (((int)s) & 0xFFFF) * (((int)B0 - B1) & 0xFFFF);
            }
            int A0B0 = A0 * B0;
            C[csi] = (short)(((int)A0B0) & 0xFFFF);
            int a0b0high = (A0B0 >> 16) & 0xFFFF;
            int A1B1 = A1 * B1;
            int tempInt;
            tempInt = a0b0high +
              (((int)A0B0) & 0xFFFF) + (((int)d) & 0xFFFF) + (((int)A1B1) & 0xFFFF);
            C[csi + 1] = (short)(((int)tempInt) & 0xFFFF);

            tempInt = A1B1 + (((int)(tempInt >> 16)) & 0xFFFF) +
              a0b0high + (((int)(d >> 16)) & 0xFFFF) + (((int)(A1B1 >> 16)) & 0xFFFF) -
              (((int)s) & 0xFFFF);

            C[csi + 2] = (short)(((int)tempInt) & 0xFFFF);
            C[csi + 3] = (short)(((int)(tempInt >> 16)) & 0xFFFF);
          }
        } else {
          for (int i = istart; i < iend; i += 4) {
            int B0 = ((int)B[Bstart + i]) & 0xFFFF;
            int B1 = ((int)B[Bstart + i + 1]) & 0xFFFF;
            int csi = Cstart + i;
            if (B0 > B1) {
              s = (short)(((int)B0 - B1) & 0xFFFF);
              d = a1MinusA0 * (((int)s) & 0xFFFF);
            } else {
              s = (short)0;
              d = (((int)A0 - A1) & 0xFFFF) * (((int)B1 - B0) & 0xFFFF);
            }
            int A0B0 = A0 * B0;
            int a0b0high = (A0B0 >> 16) & 0xFFFF;
            C[csi] = (short)(((int)A0B0) & 0xFFFF);

            int A1B1 = A1 * B1;
            int tempInt;
            tempInt = a0b0high +
              (((int)A0B0) & 0xFFFF) + (((int)d) & 0xFFFF) + (((int)A1B1) & 0xFFFF);
            C[csi + 1] = (short)(((int)tempInt) & 0xFFFF);

            tempInt = A1B1 + (((int)(tempInt >> 16)) & 0xFFFF) +
              a0b0high + (((int)(d >> 16)) & 0xFFFF) + (((int)(A1B1 >> 16)) & 0xFFFF) -
              (((int)s) & 0xFFFF);

            C[csi + 2] = (short)(((int)tempInt) & 0xFFFF);
            C[csi + 3] = (short)(((int)(tempInt >> 16)) & 0xFFFF);
          }
        }
      }
    }

    private static void AtomicMultiplyAddOpt(short[] C, int Cstart, int A0, int A1, short[] B, int Bstart, int istart, int iend)
    {
      short s;
      int d;
      int a1MinusA0 = ((int)A1 - A0) & 0xFFFF;
      A1 &= 0xFFFF;
      A0 &= 0xFFFF;
      unchecked {
        if (A1 >= A0) {
          for (int i = istart; i < iend; i += 4) {
            int b0 = ((int)B[Bstart + i]) & 0xFFFF;
            int b1 = ((int)B[Bstart + i + 1]) & 0xFFFF;
            int csi = Cstart + i;
            if (b0 >= b1)
            {
              s = (short)0;
              d = a1MinusA0 * (((int)b0 - b1) & 0xFFFF);
            } else {
              s = (short)a1MinusA0;
              d = (((int)s) & 0xFFFF) * (((int)b0 - b1) & 0xFFFF);
            }
            int A0B0 = A0 * b0;
            int a0b0high = (A0B0 >> 16) & 0xFFFF;
            int tempInt;
            tempInt = A0B0 + (((int)C[csi]) & 0xFFFF);
            C[csi] = (short)(((int)tempInt) & 0xFFFF);

            int A1B1 = A1 * b1;
            int a1b1low = A1B1 & 0xFFFF;
            int a1b1high = ((int)(A1B1 >> 16)) & 0xFFFF;
            tempInt =  (((int)(tempInt >> 16)) & 0xFFFF) + (((int)A0B0) & 0xFFFF) + (((int)d) & 0xFFFF) + a1b1low + (((int)C[csi + 1]) & 0xFFFF);
            C[csi + 1] = (short)(((int)tempInt) & 0xFFFF);

            tempInt =  (((int)(tempInt >> 16)) & 0xFFFF) + a1b1low + a0b0high + (((int)(d >> 16)) & 0xFFFF) +
              a1b1high - (((int)s) & 0xFFFF) + (((int)C[csi + 2]) & 0xFFFF);
            C[csi + 2] = (short)(((int)tempInt) & 0xFFFF);

            tempInt =  (((int)(tempInt >> 16)) & 0xFFFF) + a1b1high + (((int)C[csi + 3]) & 0xFFFF);
            C[csi + 3] = (short)(((int)tempInt) & 0xFFFF);
            if ((tempInt >> 16) != 0) {
              C[csi + 4]++;
              C[csi + 5] += (short)((C[csi + 4] == 0) ? 1 : 0);
            }
          }
        } else {
          for (int i = istart; i < iend; i += 4) {
            int B0 = ((int)B[Bstart + i]) & 0xFFFF;
            int B1 = ((int)B[Bstart + i + 1]) & 0xFFFF;
            int csi = Cstart + i;
            if (B0 > B1) {
              s = (short)(((int)B0 - B1) & 0xFFFF);
              d = a1MinusA0 * (((int)s) & 0xFFFF);
            } else {
              s = (short)0;
              d = (((int)A0 - A1) & 0xFFFF) * (((int)B1 - B0) & 0xFFFF);
            }
            int A0B0 = A0 * B0;
            int a0b0high = (A0B0 >> 16) & 0xFFFF;
            int tempInt;
            tempInt = A0B0 + (((int)C[csi]) & 0xFFFF);
            C[csi] = (short)(((int)tempInt) & 0xFFFF);

            int A1B1 = A1 * B1;
            int a1b1low = A1B1 & 0xFFFF;
            int a1b1high = (A1B1 >> 16) & 0xFFFF;
            tempInt =  (((int)(tempInt >> 16)) & 0xFFFF) + (((int)A0B0) & 0xFFFF) + (((int)d) & 0xFFFF) + a1b1low + (((int)C[csi + 1]) & 0xFFFF);
            C[csi + 1] = (short)(((int)tempInt) & 0xFFFF);

            tempInt =  (((int)(tempInt >> 16)) & 0xFFFF) + a1b1low + a0b0high + (((int)(d >> 16)) & 0xFFFF) +
              a1b1high - (((int)s) & 0xFFFF) + (((int)C[csi + 2]) & 0xFFFF);
            C[csi + 2] = (short)(((int)tempInt) & 0xFFFF);

            tempInt =  (((int)(tempInt >> 16)) & 0xFFFF) +a1b1high + (((int)C[csi + 3]) & 0xFFFF);
            C[csi + 3] = (short)(((int)tempInt) & 0xFFFF);
            if ((tempInt >> 16) != 0) {
              C[csi + 4]++;
              C[csi + 5] += (short)((C[csi + 4] == 0) ? 1 : 0);
            }
          }
        }
      }
    }

    private static void Divide(
      short[] Rarr, int Rstart,  // remainder
      short[] Qarr, int Qstart,  // quotient
      short[] TA, int Tstart,  // scratch space
      short[] Aarr, int Astart, int NAint,  // dividend
      short[] Barr, int Bstart, int NBint  // divisor
     ) {
      // set up temporary work space
      int NA = (int)NAint;
      int NB = (int)NBint;
      #if DEBUG
      if (NAint <= 0) { throw new ArgumentException("NAint" + " not less than " + "0" + " (" + Convert.ToString((int)NAint, System.Globalization.CultureInfo.InvariantCulture) + ")"); }
      if (NBint <= 0) { throw new ArgumentException("NBint" + " not less than " + "0" + " (" + Convert.ToString((int)NBint, System.Globalization.CultureInfo.InvariantCulture) + ")"); }
      if (!(NA % 2 == 0 && NB % 2 == 0)) { throw new ArgumentException("doesn't satisfy NA%2==0 && NB%2==0"); }
      if (!(Barr[Bstart + NB - 1] != 0 ||
            Barr[Bstart + NB - 2] != 0)) { throw new ArgumentException("doesn't satisfy B[NB-1]!=0 || B[NB-2]!=0"); }
      if (!(NB <= NA)) { throw new ArgumentException("doesn't satisfy NB<= NA"); }
      #endif
      short[] TBarr = TA;
      short[] TParr = TA;
      short[] quot = Qarr;
      if (Qarr == null) {
        quot = new short[2];
      }
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
          if (Qarr != null) {
            Qarr[Qstart + NA - NB + 1] = (short)0;
            Qarr[Qstart + NA - NB] = (short)0;
          }
          while (
TA[NA] != 0 || Compare(
TA, (int)(Tstart + NA - NB),
                                        TBarr, TBstart, NB) >= 0) {
            TA[NA] -= (
short)Subtract(
TA, (int)(Tstart + NA - NB),
                                      TA, (int)(Tstart + NA - NB),
                                      TBarr, TBstart, NB);
            if (Qarr != null)
              Qarr[Qstart + NA - NB] += (short)1;
          }
        } else {
          NA += 2;
        }

        short BT0 = (short)(TBarr[TBstart + NB - 2] + (short)1);
        short BT1 = (short)(TBarr[TBstart + NB - 1] + (short)(BT0 == (short)0 ? 1 : 0));

        // start reducing TA mod TB, 2 words at a time
        short[] TAtomic = new short[4];
        for (int i = NA - 2; i >= NB; i -= 2) {
          int qs = (Qarr == null) ? 0 : Qstart + i - NB;
          AtomicDivide(quot, qs, TA, (int)(Tstart + i - 2), BT0, BT1, TAtomic);
          // now correct the underestimated quotient
          int Rstart2 = Tstart + i - NB;
          int N = NB;
          unchecked {
            int Q0 = quot[qs];
            int Q1 = quot[qs + 1];
            if (Q1 == 0) {
              short carry = LinearMultiply(TParr, TPstart, TBarr, TBstart, (short)Q0, N);
              TParr[TPstart + N] = carry;
              TParr[TPstart + N + 1] = 0;
            } else if (N == 2) {
              Baseline_Multiply2(TParr, TPstart, quot, qs, TBarr, TBstart);
            } else {
              TParr[TPstart + N] = (short)0;
              TParr[TPstart + N + 1] = (short)0;
              Q0 &= 0xFFFF;
              Q1 &= 0xFFFF;
              AtomicMultiplyOpt(TParr, TPstart, Q0, Q1, TBarr, TBstart, 0, N);
              AtomicMultiplyAddOpt(TParr, TPstart, Q0, Q1, TBarr, TBstart, 2, N);
            }
            Subtract(TA, Rstart2, TA, Rstart2, TParr, TPstart, N + 2);
            while (TA[Rstart2 + N] != 0 || Compare(TA, Rstart2, TBarr, TBstart, N) >= 0) {
              TA[Rstart2 + N] -= (short)Subtract(TA, Rstart2, TA, Rstart2, TBarr, TBstart, N);
              if (Qarr != null) {
                Qarr[qs]++;
                Qarr[qs + 1] += (short)((Qarr[qs] == 0) ? 1 : 0);
              }
            }
          }

        }
        if (Rarr != null) {  // If the remainder is non-null
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
      else return (int)1 << (int)BitPrecisionInt(n - 1);
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
    public static BigInteger fromByteArray(byte[] bytes, bool littleEndian) {
      BigInteger bigint = new BigInteger();
      bigint.fromByteArrayInternal(bytes, littleEndian);
      return bigint;
    }

    private void fromByteArrayInternal(byte[] bytes, bool littleEndian) {
      if (bytes == null) {
 throw new ArgumentNullException("bytes");
}
      if (bytes.Length == 0) {
        this.reg = new short[] { (short)0, (short)0 };
        this.wordCount = 0;
      } else {
        int len = bytes.Length;
        int wordLength = ((int)len + 1) >> 1;
        wordLength = (wordLength <= 16) ?
          RoundupSizeTable[wordLength] :
          RoundupSize(wordLength);
        this.reg = new short[wordLength];
        int jIndex = littleEndian ? len - 1 : 0;
        bool negative = (bytes[jIndex] & 0x80) != 0;
        this.negative = negative;
        int j = 0;
        if (!negative) {
          for (int i = 0; i < len; i += 2, j++) {
            int index = littleEndian ? i : len - 1 - i;
            int index2 = littleEndian ? i + 1 : len - 2 - i;
            this.reg[j] = (short)(((int)bytes[index]) & 0xFF);
            if (index2 >= 0 && index2 < len) {
              this.reg[j] |= unchecked((short)(((short)bytes[index2]) << 8));
            }
          }
        } else {
          for (int i = 0; i < len; i += 2, j++) {
            int index = littleEndian ? i : len - 1 - i;
            int index2 = littleEndian ? i + 1 : len - 2 - i;
            this.reg[j] = (short)(((int)bytes[index]) & 0xFF);
            if (index2 >= 0 && index2 < len) {
              this.reg[j] |= unchecked((short)(((short)bytes[index2]) << 8));
            } else {
              // sign extend the last byte
              this.reg[j] |= unchecked((short)0xFF00);
            }
          }
          for (; j < this.reg.Length; ++j) {
            this.reg[j] = unchecked((short)0xFFFF);  // sign extend remaining words
          }
          TwosComplement(this.reg, 0, (int)this.reg.Length);
        }
        this.wordCount = this.reg.Length;
        while (this.wordCount != 0 &&
               this.reg[this.wordCount - 1] == 0)
          this.wordCount--;
      }
    }

    private BigInteger Allocate(int length) {
      this.reg = new short[RoundupSize(length)];  // will be initialized to 0
      this.negative = false;
      this.wordCount = 0;
      return this;
    }

    private static short[] GrowForCarry(short[] a, short carry) {
      int oldLength = a.Length;
      short[] ret = CleanGrow(a, RoundupSize(oldLength + 1));
      ret[oldLength] = carry;
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
        this.reg = CleanGrow(this.reg, RoundupSize(BitsToWords(n + 1)));
        this.reg[(n >> 4)] |= (short)((short)1 << (int)(n & 0xf));
        this.wordCount = this.CalcWordCount();
      } else {
        if ((n >> 4) < this.reg.Length)
          this.reg[(n >> 4)] &= unchecked((short)(~((short)1 << (int)(n % 16))));
        this.wordCount = this.CalcWordCount();
      }
    }

    /// <summary> Not documented yet. </summary>
    /// <param name='index'>A 32-bit unsigned integer.</param>
    /// <returns>A Boolean object.</returns>
    public bool testBit(int index) {
      if (index < 0) { throw new ArgumentOutOfRangeException("index"); }
      if (this.Sign < 0) {
        int tcindex = 0;
        int wordpos = index / 16;
        if (wordpos >= this.reg.Length) { return true; }
        while (tcindex < wordpos && this.reg[tcindex] == 0) {
          tcindex++;
        }
        short tc;
        unchecked {
          tc = this.reg[wordpos];
          if (tcindex == wordpos) tc--;
          tc = (short)~tc;
        }
        return (bool)(((tc >> (int)(index & 15)) & 1) != 0);
      } else {
        return this.GetUnsignedBit(index);
      }
    }

    /// <summary> Not documented yet. </summary>
    /// <param name='n'>A 32-bit unsigned integer.</param>
    /// <returns></returns>
    private bool GetUnsignedBit(int n) {
      #if DEBUG
      if (n < 0) { throw new ArgumentException("n" + " not greater or equal to " + "0" + " (" + Convert.ToString((int)n, System.Globalization.CultureInfo.InvariantCulture) + ")"); }
      #endif
      if ((n >> 4) >= this.reg.Length)
        return false;
      else
        return (bool)(((this.reg[(n >> 4)] >> (int)(n & 15)) & 1) != 0);
    }

    private BigInteger InitializeInt(int numberValue) {
      int iut;
      unchecked {
        this.negative = numberValue < 0;
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
          this.wordCount = this.reg[1] != 0 ? 2 : (this.reg[0] == 0 ? 0 : 1);
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
        int byteCount = this.ByteCount();
        int byteArrayLength = byteCount;
        if (this.GetUnsignedBit((byteCount * 8) - 1)) {
          byteArrayLength++;
        }
        byte[] bytes = new byte[byteArrayLength];
        int j = 0;
        for (int i = 0; i < byteCount; i += 2, j++) {
          int index = littleEndian ? i : bytes.Length - 1 - i;
          int index2 = littleEndian ? i + 1 : bytes.Length - 2 - i;
          bytes[index] = (byte)(this.reg[j] & 0xff);
          if (index2 >= 0 && index2 < byteArrayLength) {
            bytes[index2] = (byte)((this.reg[j] >> 8) & 0xff);
          }
        }
        return bytes;
      } else {
        short[] regdata = new short[this.reg.Length];
        Array.Copy(this.reg, regdata, this.reg.Length);
        TwosComplement(regdata, 0, (int)regdata.Length);
        int byteCount = regdata.Length * 2;
        for (int i = regdata.Length - 1; i >= 0; --i) {
          if (regdata[i] == unchecked((short)0xFFFF)) {
            byteCount -= 2;
          } else if ((regdata[i] & 0xFF80) == 0xFF80) {
            // signed first byte, 0xFF second
            byteCount -= 1;
            break;
          } else if ((regdata[i] & 0x8000) == 0x8000) {
            // signed second byte
            break;
          } else {
            // unsigned second byte
            byteCount += 1;
            break;
          }
        }
        if (byteCount == 0) byteCount = 1;
        byte[] bytes = new byte[byteCount];
        bytes[littleEndian ? bytes.Length - 1 : 0] = (byte)0xFF;
        byteCount = Math.Min(byteCount, regdata.Length * 2);
        int j = 0;
        for (int i = 0; i < byteCount; i += 2, j++) {
          int index = littleEndian ? i : bytes.Length - 1 - i;
          int index2 = littleEndian ? i + 1 : bytes.Length - 2 - i;
          bytes[index] = (byte)(regdata[j] & 0xff);
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
      if (numberBits == 0) { return this; }
      if (numberBits < 0) {
        if (numberBits == Int32.MinValue)
          return this.shiftRight(1).shiftRight(Int32.MaxValue);
        return this.shiftRight(-numberBits);
      }
      BigInteger ret = new BigInteger();
      int numWords = (int)this.wordCount;
      int shiftWords = (int)(numberBits >> 4);
      int shiftBits = (int)(numberBits & 15);
      bool neg = numWords > 0 && this.negative;
      if (!neg) {
        ret.negative = false;
        ret.reg = new short[RoundupSize(numWords + BitsToWords((int)numberBits))];
        Array.Copy(this.reg, ret.reg, numWords);
        ShiftWordsLeftByWords(ret.reg, 0, numWords + shiftWords, shiftWords);
        ShiftWordsLeftByBits(ret.reg, (int)shiftWords, numWords + BitsToWords(shiftBits), shiftBits);
        ret.wordCount = ret.CalcWordCount();
      } else {
        ret.negative = true;
        ret.reg = new short[RoundupSize(numWords + BitsToWords((int)numberBits))];
        Array.Copy(this.reg, ret.reg, numWords);
        TwosComplement(ret.reg, 0, (int)ret.reg.Length);
        ShiftWordsLeftByWords(ret.reg, 0, numWords + shiftWords, shiftWords);
        ShiftWordsLeftByBits(ret.reg, (int)shiftWords, numWords + BitsToWords(shiftBits), shiftBits);
        TwosComplement(ret.reg, 0, (int)ret.reg.Length);
        ret.wordCount = ret.CalcWordCount();
      }
      return ret;
    }
    /// <summary> Not documented yet. </summary>
    /// <returns>A BigInteger object.</returns>
    /// <param name='numberBits'>A 32-bit signed integer.</param>
    public BigInteger shiftRight(int numberBits) {
      if (numberBits == 0) { return this; }
      if (numberBits < 0) {
        if (numberBits == Int32.MinValue)
          return this.shiftLeft(1).shiftLeft(Int32.MaxValue);
        return this.shiftLeft(-numberBits);
      }
      BigInteger ret = new BigInteger();
      int numWords = (int)this.wordCount;
      int shiftWords = (int)(numberBits >> 4);
      int shiftBits = (int)(numberBits & 15);
      ret.negative = this.negative;
      ret.reg = new short[RoundupSize(numWords)];
      Array.Copy(this.reg, ret.reg, numWords);
      if (this.Sign < 0) {
        TwosComplement(ret.reg, 0, (int)ret.reg.Length);
        ShiftWordsRightByWordsSignExtend(ret.reg, 0, numWords, shiftWords);
        if (numWords > shiftWords)
          ShiftWordsRightByBitsSignExtend(ret.reg, 0, numWords - shiftWords, shiftBits);
        TwosComplement(ret.reg, 0, (int)ret.reg.Length);
      } else {
        ShiftWordsRightByWords(ret.reg, 0, numWords, shiftWords);
        if (numWords > shiftWords)
          ShiftWordsRightByBits(ret.reg, 0, numWords - shiftWords, shiftBits);
      }
      ret.wordCount = ret.CalcWordCount();
      return ret;
    }

    /// <summary> Not documented yet. </summary>
    /// <returns>A BigInteger object.</returns>
    /// <param name='longerValue'>A 64-bit signed integer.</param>
    public static BigInteger valueOf(long longerValue) {
      if (longerValue == 0) { return BigInteger.Zero; }
      if (longerValue == 1) { return BigInteger.One; }
      BigInteger ret = new BigInteger();
      unchecked {
        ret.negative = longerValue < 0;
        ret.reg = new short[4];
        if (longerValue == Int64.MinValue) {
          ret.reg[0] = 0;
          ret.reg[1] = 0;
          ret.reg[2] = 0;
          ret.reg[3] = (short)0x8000;
          ret.wordCount = 4;
        } else {
          long ut = longerValue;
          if (ut < 0)ut = -ut;
          ret.reg[0] = (short)(ut & 0xFFFF);
          ut >>=  16;
          ret.reg[1] = (short)(ut & 0xFFFF);
          ut >>=  16;
          ret.reg[2] = (short)(ut & 0xFFFF);
          ut >>=  16;
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

    /// <summary> Not documented yet. </summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int intValue() {
      int c = (int)this.wordCount;
      if (c == 0) { return 0; }
      if (c > 2) { throw new OverflowException(); }
      if (c == 2 && (this.reg[1] & 0x8000) != 0) {
        if (((short)(this.reg[1] & (short)0x7FFF) | this.reg[0]) == 0 && this.negative) {
          return Int32.MinValue;
        } else {
          throw new OverflowException();
        }
      } else {
        int ivv = ((int)this.reg[0]) & 0xFFFF;
        if (c > 1) ivv |= (((int)this.reg[1]) & 0xFFFF) << 16;
        if (this.negative) ivv = -ivv;
        return ivv;
      }
    }

    /// <summary> Not documented yet. </summary>
    /// <returns>A Boolean object.</returns>
    public bool canFitInInt() {
      int c = (int)this.wordCount;
      if (c > 2) { return false; }
      if (c == 2 && (this.reg[1] & 0x8000) != 0) {
        return (this.negative && this.reg[1] == unchecked((short)0x8000) &&
                this.reg[0] == 0);
      }
      return true;
    }

    private bool HasSmallValue() {
      int c = (int)this.wordCount;
      if (c > 4) { return false; }
      if (c == 4 && (this.reg[3] & 0x8000) != 0) {
        return (this.negative && this.reg[3] == unchecked((short)0x8000) &&
                this.reg[2] == 0 &&
                this.reg[1] == 0 &&
                this.reg[0] == 0);
      }
      return true;
    }

    /// <summary> Not documented yet. </summary>
    /// <returns>A 64-bit signed integer.</returns>
    public long longValue() {
      int count = this.wordCount;
      if (count == 0) { return (long)0; }
      if (count > 4) { throw new OverflowException(); }
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
        int tmp = ((int)this.reg[0]) & 0xFFFF;
        long vv = (long)tmp;
        if (count > 1) {
          tmp = ((int)this.reg[1]) & 0xFFFF;
          vv |= (long)tmp << 16;
        }
        if (count > 2) {
          tmp = ((int)this.reg[2]) & 0xFFFF;
          vv |= (long)tmp << 32;
        }
        if (count > 3) {
          tmp = ((int)this.reg[3]) & 0xFFFF;
          vv |= (long)tmp << 48;
        }
        if (this.negative) vv = -vv;
        return vv;
      }
    }

    private static BigInteger Power2(int e) {
      BigInteger r = new BigInteger().Allocate(BitsToWords((int)(e + 1)));
      r.SetBitInternal((int)e, true);  // NOTE: Will recalculate word count
      return r;
    }

    /// <summary> Not documented yet. </summary>
    /// <returns>A BigInteger object.</returns>
    /// <param name='power'>A BigInteger object.</param>
    public BigInteger PowBigIntVar(BigInteger power) {
      if (power == null) { throw new ArgumentNullException("power"); }
      int sign = power.Sign;
      if (sign < 0) { throw new ArgumentException("power is negative"); }
      BigInteger thisVar = this;
      if (sign == 0)
        return BigInteger.One;  // however 0 to the power of 0 is undefined
      else if (power.Equals(BigInteger.One))
        return this;
      else if (power.wordCount == 1 && power.reg[0] == 2)
        return thisVar * (BigInteger)thisVar;
      else if (power.wordCount == 1 && power.reg[0] == 3)
        return (thisVar * (BigInteger)thisVar) * (BigInteger)thisVar;
      BigInteger r = BigInteger.One;
      while (!power.IsZero) {
        if (!power.IsEven) {
          r = r * (BigInteger)thisVar;
        }
        power >>= 1;
        if (!power.IsZero) {
          thisVar = thisVar * (BigInteger)thisVar;
        }
      }
      return r;
    }

    /// <summary> Not documented yet. </summary>
    /// <param name='powerSmall'>A 32-bit signed integer.</param>
    /// <returns>A BigInteger object.</returns>
    public BigInteger pow(int powerSmall) {
      if (powerSmall < 0) { throw new ArgumentException("power is negative"); }
      BigInteger thisVar = this;
      if (powerSmall == 0)
        return BigInteger.One;  // however 0 to the power of 0 is undefined
      else if (powerSmall == 1)
        return this;
      else if (powerSmall == 2)
        return thisVar * (BigInteger)thisVar;
      else if (powerSmall == 3)
        return (thisVar * (BigInteger)thisVar) * (BigInteger)thisVar;
      BigInteger r = BigInteger.One;
      while (powerSmall != 0) {
        if ((powerSmall & 1) != 0) {
          r = r * (BigInteger)thisVar;
        }
        powerSmall >>= 1;
        if (powerSmall != 0) {
          thisVar = thisVar * (BigInteger)thisVar;
        }
      }
      return r;
    }

    /// <summary> Not documented yet. </summary>
    /// <returns>A BigInteger object.</returns>
    public BigInteger negate() {
      BigInteger bigintRet = new BigInteger();
      bigintRet.reg = this.reg;  // use the same reference
      bigintRet.wordCount = this.wordCount;
      bigintRet.negative = (this.wordCount != 0) && (!this.negative);
      return bigintRet;
    }
    /// <summary> Not documented yet. </summary>
    /// <returns>A BigInteger object.</returns>
    public BigInteger abs() {
      return (this.wordCount == 0 || !this.negative) ? this : this.negate();
    }

    /// <summary> Not documented yet. </summary>
    /// <returns></returns>
    private int CalcWordCount() {
      return (int)CountWords(this.reg, this.reg.Length);
    }

    /// <summary> Not documented yet. </summary>
    /// <returns></returns>
    private int ByteCount() {
      int wc = this.wordCount;
      if (wc == 0)return 0;
      short s = this.reg[wc - 1];
      wc = (wc - 1) << 1;
      if (s == 0)return wc;
      return ((s >> 8) == 0) ? wc + 1 : wc + 2;
    }

    /// <summary> Finds the minimum number of bits needed to represent this
    /// object's absolute value. </summary>
    /// <returns>The number of bits in this object&apos; s value. Returns
    /// 0 if this object&apos; s value is 0, and returns 1 if the value is negative
    /// 1</returns>
    public int getUnsignedBitLength() {
      int wc = this.wordCount;
      if (wc != 0) {
        int numberValue = ((int)this.reg[wc-1]) & 0xFFFF;
        wc = (wc - 1) << 4;
        if (numberValue == 0)return wc;
        wc += 16;
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

    /// <summary> Not documented yet. </summary>
    /// <param name='reg'>A short[] object.</param>
    /// <param name='wordCount'>A 32-bit signed integer.</param>
    /// <returns>A 32-bit signed integer.</returns>
    private static int getUnsignedBitLengthEx(int numberValue, int wordCount) {
      int wc = wordCount;
      if (wc != 0) {
        wc = (wc - 1) << 4;
        if (numberValue == 0)return wc;
        wc += 16;
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
    /// <returns>The number of bits in this object&apos; s value. Returns
    /// 0 if this object&apos; s value is 0 or negative 1.</returns>
    public int bitLength() {
      int wc = this.wordCount;
      if (wc != 0) {
        int numberValue = ((int)this.reg[wc-1]) & 0xFFFF;
        wc = (wc - 1) << 4;
        if (numberValue == (this.negative ? 1 : 0))return wc;
        wc += 16;
        unchecked {
          if (this.negative) {
            numberValue--;
            numberValue &= 0xFFFF;
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
          return ((numberValue >> 15) == 0) ? wc - 1 : wc;
        }
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
      long value = this.longValue();
      if (value == Int64.MinValue)
        return "-9223372036854775808";
      bool neg = value < 0;
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

    private static int ApproxLogTenOfTwo(int bitlen) {
      int bitlenLow = bitlen & 0xFFFF;
      int bitlenHigh = (bitlen >> 16) & 0xFFFF;
      short resultLow = 0;
      short resultHigh = 0;
      unchecked {
        int p; short c; int d;
        p = bitlenLow * 0x84FB; d = ((int)p >> 16) & 0xFFFF; c = (short)d; d = ((int)d >> 16) & 0xFFFF;
        p = bitlenLow * 0x209A;
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = bitlenHigh * 0x84FB;
        p = p + (((int)c) & 0xFFFF); d = d + (((int)p >> 16) & 0xFFFF); c = (short)d; d = ((int)d >> 16) & 0xFFFF;
        p = bitlenLow * 0x9A;
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = bitlenHigh * 0x209A;
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = ((int)c) & 0xFFFF; c = (short)p; resultLow = c; c = (short)d; d = (((int)d >> 16) & 0xFFFF);
        p = bitlenHigh * 0x9A;
        p = p + (((int)c) & 0xFFFF);
        resultHigh = (short)p;
        int result = ((int)resultLow) & 0xFFFF;
        result |= (((int)resultHigh) & 0xFFFF) << 16;
        return (result & 0x7FFFFFFF) >> 9;
      }
    }

    /// <summary> Finds the number of decimal digits this number has.</summary>
    /// <returns>The number of decimal digits. Returns 1 if this object&apos;
    /// s value is 0.</returns>
    public int getDigitCount() {
      if (this.IsZero)
        return 1;
      if (this.HasSmallValue()) {
        long value = this.longValue();
        if (value == Int64.MinValue)return 19;
        if (value < 0)value = -value;
        if (value >= 1000000000L) {
          if (value >= 1000000000000000000L)return 19;
          if (value >= 100000000000000000L)return 18;
          if (value >= 10000000000000000L)return 17;
          if (value >= 1000000000000000L)return 16;
          if (value >= 100000000000000L)return 15;
          if (value >= 10000000000000L)return 14;
          if (value >= 1000000000000L)return 13;
          if (value >= 100000000000L)return 12;
          if (value >= 10000000000L)return 11;
          if (value >= 1000000000L)return 10;
          return 9;
        } else {
          int v2 = (int)value;
          if (v2 >= 100000000)return 9;
          if (v2 >= 10000000)return 8;
          if (v2 >= 1000000)return 7;
          if (v2 >= 100000)return 6;
          if (v2 >= 10000)return 5;
          if (v2 >= 1000)return 4;
          if (v2 >= 100)return 3;
          if (v2 >= 10)return 2;
          return 1;
        }
      }
      int bitlen = this.getUnsignedBitLength();
      if (bitlen <= 2135) {
        // (x*631305) >> 21 is an approximation
        // to trunc(x*log10(2)) that is correct up
        // to x = 2135; the multiplication would require
        // up to 31 bits in all cases up to 2135
        // (cases up to 64 are already handled above)
        int minDigits = 1 + (((bitlen - 1) * 631305) >> 21);
        int maxDigits = 1 + ((bitlen * 631305) >> 21);
        if (minDigits == maxDigits) {
          // Number of digits is the same for
          // all numbers with this bit length
          return minDigits;
        }
      } else if (bitlen <= 6432162) {
        // Much more accurate approximation
        int minDigits = ApproxLogTenOfTwo(bitlen - 1);
        int maxDigits = ApproxLogTenOfTwo(bitlen);
        if (minDigits == maxDigits) {
          // Number of digits is the same for
          // all numbers with this bit length
          return 1 + minDigits;
        }
      }
      short[] tempReg = null;
      int wordCount = this.wordCount;
      int i = 0;
      while (wordCount != 0) {
        if (wordCount == 1) {
          int rest = ((int)tempReg[0]) & 0xFFFF;
          if (rest >= 10000)i += 5;
          else if (rest >= 1000)i += 4;
          else if (rest >= 100)i += 3;
          else if (rest >= 10)i += 2;
          else i++;
          break;
        } else if (wordCount == 2 && tempReg[1] > 0 && tempReg[1] <= 0x7FFF) {
          int rest = ((int)tempReg[0]) & 0xFFFF;
          rest |= (((int)tempReg[1]) & 0xFFFF) << 16;
          if (rest >= 1000000000)i += 10;
          else if (rest >= 100000000)i += 9;
          else if (rest >= 10000000)i += 8;
          else if (rest >= 1000000)i += 7;
          else if (rest >= 100000)i += 6;
          else if (rest >= 10000)i += 5;
          else if (rest >= 1000)i += 4;
          else if (rest >= 100)i += 3;
          else if (rest >= 10)i += 2;
          else i++;
          break;
        } else {
          int wci = wordCount;
          short remainder = 0;
          int quo, rem;
          bool firstdigit = false;
          short[] dividend = (tempReg == null) ? this.reg : tempReg;
          // Divide by 10000
          while ((wci--) > 0) {
            int curValue = ((int)dividend[wci]) & 0xFFFF;
            int currentDividend = unchecked((int)(curValue |
                                                  ((int)remainder << 16)));
            quo = currentDividend / 10000;
            if (!firstdigit && quo != 0) {
              firstdigit = true;
              // Since we are dividing from left to right, the first
              // nonzero result is the first part of the
              // new quotient
              bitlen = getUnsignedBitLengthEx(quo, wci + 1);
              if (bitlen <= 2135) {
                // (x*631305) >> 21 is an approximation
                // to trunc(x*log10(2)) that is correct up
                // to x = 2135; the multiplication would require
                // up to 31 bits in all cases up to 2135
                // (cases up to 64 are already handled above)
                int minDigits = 1 + (((bitlen - 1) * 631305) >> 21);
                int maxDigits = 1 + ((bitlen * 631305) >> 21);
                if (minDigits == maxDigits) {
                  // Number of digits is the same for
                  // all numbers with this bit length
                  return i + minDigits + 4;
                }
              } else if (bitlen <= 6432162) {
                // Much more accurate approximation
                int minDigits = ApproxLogTenOfTwo(bitlen - 1);
                int maxDigits = ApproxLogTenOfTwo(bitlen);
                if (minDigits == maxDigits) {
                  // Number of digits is the same for
                  // all numbers with this bit length
                  return i + 1 + minDigits + 4;
                }
              }
            }
            if (tempReg == null) {
              if (quo != 0) {
                tempReg = new short[this.wordCount];
                Array.Copy(this.reg, tempReg, tempReg.Length);
                // Use the calculated word count during division;
                // zeros that may have occurred in division
                // are not incorporated in the tempReg
                wordCount = wci + 1;
                tempReg[wci] = unchecked((short)quo);
              }
            } else {
              tempReg[wci] = unchecked((short)quo);
            }
            rem = currentDividend - (10000 * quo);
            remainder = unchecked((short)rem);
          }
          // Recalculate word count
          while (wordCount != 0 && tempReg[wordCount - 1] == 0)
            wordCount--;
          i += 4;
        }
      }
      return i;
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      if (this.IsZero)
        return "0";
      if (this.HasSmallValue()) {
        return this.SmallValueToString();
      }
      short[] tempReg = new short[this.wordCount];
      Array.Copy(this.reg, tempReg, tempReg.Length);
      int wordCount = tempReg.Length;
      while (wordCount != 0 && tempReg[wordCount - 1] == 0)
        wordCount--;
      int i = 0;
      char[] s = new char[(wordCount << 4) + 1];
      while (wordCount != 0) {
        if (wordCount == 1 && tempReg[0] > 0 && tempReg[0] <= 0x7FFF) {
          int rest = tempReg[0];
          while (rest != 0) {
            // accurate approximation to rest/10 up to 43698,
            // and rest can go up to 32767
            int newrest = (rest * 26215) >> 18;
            s[i++] = vec[rest - (newrest * 10)];
            rest = newrest;
          }
          break;
        } else if (wordCount == 2 && tempReg[1] > 0 && tempReg[1] <= 0x7FFF) {
          int rest = ((int)tempReg[0]) & 0xFFFF;
          rest |= (((int)tempReg[1]) & 0xFFFF) << 16;
          while (rest != 0) {
            int newrest = rest / 10;
            s[i++] = vec[rest - (newrest * 10)];
            rest = newrest;
          }
          break;
        } else {
          int wci = wordCount;
          short remainder = 0;
          int quo, rem;
          // Divide by 10000
          while ((wci--) > 0) {
            int currentDividend = unchecked((int)((((int)tempReg[wci]) & 0xFFFF) |
                                                  ((int)remainder << 16)));
            quo = currentDividend / 10000;
            tempReg[wci] = unchecked((short)quo);
            rem = currentDividend - (10000 * quo);
            remainder = unchecked((short)rem);
          }
          int remainderSmall = remainder;
          // Recalculate word count
          while (wordCount != 0 && tempReg[wordCount - 1] == 0)
            wordCount--;
          // accurate approximation to rest/10 up to 16388,
          // and rest can go up to 9999
          int newrest = (remainderSmall * 3277) >> 15;
          s[i++] = vec[(int)(remainderSmall - (newrest * 10))];
          remainderSmall = newrest;
          newrest = (remainderSmall * 3277) >> 15;
          s[i++] = vec[(int)(remainderSmall - (newrest * 10))];
          remainderSmall = newrest;
          newrest = (remainderSmall * 3277) >> 15;
          s[i++] = vec[(int)(remainderSmall - (newrest * 10))];
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

    /// <summary> Not documented yet. </summary>
    /// <param name='str'>A string object.</param>
    /// <returns>A BigInteger object.</returns>
    public static BigInteger fromString(string str) {
      if (str == null)throw new ArgumentNullException("str");
      return fromSubstring(str, 0, str.Length);
    }

    private const int MaxSafeInt = 214748363;

    public static BigInteger fromSubstring(string str, int index, int endIndex) {
      if (str == null)throw new ArgumentNullException("str");
      if (index < 0)throw new ArgumentException("\"str\""+" not greater or equal to "+"0"+" ("+Convert.ToString((long)index,System.Globalization.CultureInfo.InvariantCulture)+")");
      if (index>str.Length)throw new ArgumentException("\"str\""+" not less or equal to "+Convert.ToString((long)str.Length,System.Globalization.CultureInfo.InvariantCulture)+" ("+Convert.ToString((long)index,System.Globalization.CultureInfo.InvariantCulture)+")");
      if (endIndex < 0)throw new ArgumentException("\"index\""+" not greater or equal to "+"0"+" ("+Convert.ToString((long)endIndex,System.Globalization.CultureInfo.InvariantCulture)+")");
      if (endIndex>str.Length)throw new ArgumentException("\"index\""+" not less or equal to "+Convert.ToString((long)str.Length,System.Globalization.CultureInfo.InvariantCulture)+" ("+Convert.ToString((long)endIndex,System.Globalization.CultureInfo.InvariantCulture)+")");
      if (endIndex<index)throw new ArgumentException("\"endIndex\""+" not greater or equal to "+Convert.ToString((long)index,System.Globalization.CultureInfo.InvariantCulture)+" ("+Convert.ToString((long)endIndex,System.Globalization.CultureInfo.InvariantCulture)+")");
      if (index == endIndex) {
 throw new FormatException("No digits");
}
      bool negative = false;
      if (str[0] == '-'){
        index++;
        negative = true;
      }
      BigInteger bigint = new BigInteger().Allocate(4);
      bool haveDigits = false;
      bool haveSmallInt = true;
      int smallInt = 0;
      for (int i = index; i < endIndex; ++i) {
        char c = str[i];
        if (c < '0' || c > '9')throw new FormatException("Illegal character found");
        haveDigits = true;
        int digit = (int)(c - '0');
        if (haveSmallInt && smallInt < MaxSafeInt) {
          smallInt *= 10;
          smallInt += digit;
        } else {
          if (haveSmallInt) {
            bigint.reg[0] = unchecked((short)(smallInt & 0xFFFF));
            bigint.reg[1] = unchecked((short)((smallInt >> 16) & 0xFFFF));
            haveSmallInt = false;
          }
          // Multiply by 10
          short carry = 0;
          int N = bigint.reg.Length;
          for (int j = 0; j < N; ++j) {
            int p;
            unchecked {
              p = (((int)bigint.reg[j]) & 0xFFFF) * 10;
              p = p + (((int)carry) & 0xFFFF);
              bigint.reg[j] = (short)p;
              carry = (short)(p >> 16);
            }
          }
          if (carry != 0)
            bigint.reg = GrowForCarry(bigint.reg, carry);
          // Add the parsed digit
          if (digit != 0) {
            int d = bigint.reg[0] & 0xFFFF;
            if (d <= 65526) {
              bigint.reg[0] = unchecked((short)(d + digit));
            } else if (Increment(bigint.reg, 0, bigint.reg.Length, (short)digit) != 0) {
              bigint.reg = GrowForCarry(bigint.reg, (short)1);
            }
          }
        }
      }
      if (!haveDigits) {
 throw new FormatException("No digits");
}
      if (haveSmallInt) {
        bigint.reg[0] = unchecked((short)(smallInt & 0xFFFF));
        bigint.reg[1] = unchecked((short)((smallInt >> 16) & 0xFFFF));
      }
      bigint.wordCount = bigint.CalcWordCount();
      bigint.negative = bigint.wordCount != 0 && negative;
      return bigint;
    }

    /// <summary> Not documented yet. </summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int getLowestSetBit() {
      int retSetBit = 0;
      for (int i = 0; i < this.wordCount; ++i) {
        short c = this.reg[i];
        if (c == (short)0) {
          retSetBit += 16;
        } else {
          if (((c << 15) & 0xFFFF) != 0)return retSetBit + 0;
          if (((c << 14) & 0xFFFF) != 0)return retSetBit + 1;
          if (((c << 13) & 0xFFFF) != 0)return retSetBit + 2;
          if (((c << 12) & 0xFFFF) != 0)return retSetBit + 3;
          if (((c << 11) & 0xFFFF) != 0)return retSetBit + 4;
          if (((c << 10) & 0xFFFF) != 0)return retSetBit + 5;
          if (((c << 9) & 0xFFFF) != 0)return retSetBit + 6;
          if (((c << 8) & 0xFFFF) != 0)return retSetBit + 7;
          if (((c << 7) & 0xFFFF) != 0)return retSetBit + 8;
          if (((c << 6) & 0xFFFF) != 0)return retSetBit + 9;
          if (((c << 5) & 0xFFFF) != 0)return retSetBit + 10;
          if (((c << 4) & 0xFFFF) != 0)return retSetBit + 11;
          if (((c << 3) & 0xFFFF) != 0)return retSetBit + 12;
          if (((c << 2) & 0xFFFF) != 0)return retSetBit + 13;
          if (((c << 1) & 0xFFFF) != 0)return retSetBit + 14;
          return retSetBit + 15;
        }
      }
      return 0;
    }

    /// <summary>Returns the greatest common divisor of two integers. </summary>
    /// <returns>A BigInteger object.</returns>
    /// <remarks>The greatest common divisor (GCD) is also known as the greatest
    /// common factor (GCF).</remarks>
    /// <param name='bigintSecond'>A BigInteger object.</param>
    public BigInteger gcd(BigInteger bigintSecond) {
      if (bigintSecond == null) { throw new ArgumentNullException("bigintSecond"); }
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
      int expOfTwo = Math.Min(
this.getLowestSetBit(),
                            bigintSecond.getLowestSetBit());
      if (thisValue.wordCount <= 10 && bigintSecond.wordCount <= 10) {

        while (true) {
          BigInteger bigintA = (thisValue - (BigInteger)bigintSecond).abs();
          if (bigintA.IsZero) {
            if (expOfTwo != 0) {
              thisValue <<= expOfTwo;
            }
            return thisValue;
          }
          int setbit = bigintA.getLowestSetBit();
          bigintA >>=  setbit;
          bigintSecond = (thisValue.CompareTo(bigintSecond) < 0) ? thisValue : bigintSecond;
          thisValue = bigintA;
        }
      } else {
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
    }

    /// <summary> Calculates the remainder when a BigInteger raised to a
    /// certain power is divided by another BigInteger. </summary>
    /// <param name='pow'>A BigInteger object.</param>
    /// <param name='mod'>A BigInteger object.</param>
    /// <returns>A BigInteger object.</returns>
    public BigInteger ModPow(BigInteger pow, BigInteger mod) {
      if (pow == null) { throw new ArgumentNullException("pow"); }
      if (pow.Sign < 0) {
 throw new ArgumentException("pow is negative");
}
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

    static void PositiveSubtract(
BigInteger diff,
                                 BigInteger minuend,
                                 BigInteger subtrahend) {
      int aSize = minuend.wordCount;
      aSize += aSize & 1;
      int bSize = subtrahend.wordCount;
      bSize += bSize & 1;
      if (aSize == bSize) {
        if (Compare(minuend.reg, 0, subtrahend.reg, 0, (int)aSize) >= 0) {
          // A is at least as high as B
          Subtract(diff.reg, 0, minuend.reg, 0, subtrahend.reg, 0, (int)aSize);
          diff.negative = false;  // difference will not be negative at this point
        } else {
          // A is less than B
          Subtract(diff.reg, 0, subtrahend.reg, 0, minuend.reg, 0, (int)aSize);
          diff.negative = true;  // difference will be negative
        }
      } else if (aSize > bSize) {
        // A is greater than B
        short borrow = (short)Subtract(diff.reg, 0, minuend.reg, 0, subtrahend.reg, 0, (int)bSize);
        Array.Copy(minuend.reg, bSize, diff.reg, bSize, aSize - bSize);
        borrow = (short)Decrement(diff.reg, bSize, (int)(aSize - bSize), borrow);
        // DebugAssert.IsTrue(borrow==0,"{0} line {1}: !borrow","integer.cpp",3524);
        diff.negative = false;
      } else {
        // A is less than B
        short borrow = (short)Subtract(diff.reg, 0, subtrahend.reg, 0, minuend.reg, 0, (int)aSize);
        Array.Copy(subtrahend.reg, aSize, diff.reg, aSize, bSize - aSize);
        borrow = (short)Decrement(diff.reg, aSize, (int)(bSize - aSize), borrow);
        // DebugAssert.IsTrue(borrow==0,"{0} line {1}: !borrow","integer.cpp",3532);
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
      if (this.wordCount == other.wordCount) {
        if (this.negative != other.negative)return false;
        for (int i = 0; i < this.wordCount; ++i) {
          if (this.reg[i] != other.reg[i])return false;
        }
        return true;
      }
      return false;
    }

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit hash code.</returns>
    public override int GetHashCode() {
      int hashCodeValue = 0;
      unchecked {
        hashCodeValue += 1000000007 * this.Sign.GetHashCode();
        if (this.reg != null) {
          for (int i = 0; i < this.wordCount; ++i) {
            hashCodeValue += 1000000013 * this.reg[i];
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
      if (bigintAugend == null) { throw new ArgumentNullException("bigintAugend"); }
      BigInteger sum;
      if (this.wordCount == 0)
        return bigintAugend;
      if (bigintAugend.wordCount == 0)
        return this;
      if (bigintAugend.wordCount == 1 && this.wordCount == 1) {
        if (this.negative == bigintAugend.negative) {
          int intSum = (((int)this.reg[0]) & 0xFFFF) + (((int)bigintAugend.reg[0]) & 0xFFFF);
          sum = new BigInteger();
          sum.reg = new short[2];
          sum.reg[0] = unchecked((short)intSum);
          sum.reg[1] = unchecked((short)(intSum >> 16));
          sum.wordCount = ((intSum >> 16) == 0) ? 1 : 2;
          sum.negative = this.negative;
          return sum;
        } else {
          int a = ((int)this.reg[0]) & 0xFFFF;
          int b = ((int)bigintAugend.reg[0]) & 0xFFFF;
          if (a == b)return BigInteger.Zero;
          if (a > b) {
            a -= b;
            sum = new BigInteger();
            sum.reg = new short[2];
            sum.reg[0] = unchecked((short)a);
            sum.wordCount = 1;
            sum.negative = this.negative;
            return sum;
          } else {
            b -= a;
            sum = new BigInteger();
            sum.reg = new short[2];
            sum.reg[0] = unchecked((short)b);
            sum.wordCount = 1;
            sum.negative = !this.negative;
            return sum;
          }
        }
      }
      sum = new BigInteger().Allocate((int)Math.Max(this.reg.Length, bigintAugend.reg.Length));
      if ((!this.negative) == (!bigintAugend.negative)) {
        // both nonnegative or both negative
        int carry;
        int addendCount = this.wordCount + (this.wordCount & 1);
        int augendCount = bigintAugend.wordCount + (bigintAugend.wordCount & 1);
        int desiredLength = Math.Max(addendCount, augendCount);
        if (addendCount == augendCount)
          carry = Add(sum.reg, 0, this.reg, 0, bigintAugend.reg, 0, (int)addendCount);
        else if (addendCount > augendCount) {
          // Addend is bigger
          carry = Add(
sum.reg, 0,
                      this.reg, 0,
                      bigintAugend.reg, 0,
                      (int)augendCount);
          Array.Copy(
            this.reg, augendCount,
            sum.reg, augendCount,
            addendCount - augendCount);
          if (carry != 0)
            carry = Increment(
sum.reg, augendCount,
                              (int)(addendCount - augendCount),
                              (short)carry);
        } else {
          // Augend is bigger
          carry = Add(
sum.reg, 0,
                      this.reg, 0,
                      bigintAugend.reg, 0,
                      (int)addendCount);
          Array.Copy(
            bigintAugend.reg, addendCount,
            sum.reg, addendCount,
            augendCount - addendCount);
          if (carry != 0)
            carry = Increment(
sum.reg, addendCount,
                              (int)(augendCount - addendCount),
                              (short)carry);
        }
        bool needShorten = true;
        if (carry != 0) {
          int nextIndex = desiredLength;
          int len = RoundupSize(nextIndex + 1);
          sum.reg = CleanGrow(sum.reg, len);
          sum.reg[nextIndex] = (short)carry;
          needShorten = false;
        }
        sum.negative = false;
        sum.wordCount = sum.CalcWordCount();
        if (needShorten)
          sum.ShortenArray();
        sum.negative = this.negative && !sum.IsZero;
      } else if (this.negative) {
        PositiveSubtract(sum, bigintAugend, this);  // this is negative, b is nonnegative
      } else {
        PositiveSubtract(sum, this, bigintAugend);  // this is nonnegative, b is negative
      }
      return sum;
    }

    /// <summary> Subtracts a BigInteger from this BigInteger. </summary>
    /// <param name='subtrahend'>A BigInteger object.</param>
    /// <returns>The difference of the two objects.</returns>
    public BigInteger subtract(BigInteger subtrahend) {
      if (subtrahend == null) { throw new ArgumentNullException("subtrahend"); }
      if (this.wordCount == 0)
        return subtrahend.negate();
      if (subtrahend.wordCount == 0)
        return this;
      return this.add(subtrahend.negate());
    }

    private void ShortenArray() {
      if (this.reg.Length > 32) {
        int newLength = RoundupSize(this.wordCount);
        if (newLength < this.reg.Length &&
           (this.reg.Length - newLength) >= 16) {
          // Reallocate the array if the rounded length
          // is much smaller than the current length
          short[] newreg = new short[newLength];
          Array.Copy(this.reg, newreg, Math.Min(newLength, this.reg.Length));
          this.reg = newreg;
        }
      }
    }

    /// <summary>Multiplies this instance by the value of a BigInteger object.</summary>
    /// <param name='bigintMult'>A BigInteger object.</param>
    /// <returns>The product of the two objects.</returns>
    public BigInteger multiply(BigInteger bigintMult) {
      if (bigintMult == null)throw new ArgumentNullException("bigintMult");
      if (this.wordCount == 0 || bigintMult.wordCount == 0)
        return BigInteger.Zero;
      if (this.wordCount == 1 && this.reg[0] == 1)
        return this.negative ? bigintMult.negate() : bigintMult;
      if (bigintMult.wordCount == 1 && bigintMult.reg[0] == 1)
        return bigintMult.negative ? this.negate() : this;
      BigInteger product = new BigInteger();
      bool needShorten = true;
      if (this.wordCount == 1) {
        int wc = bigintMult.wordCount;
        int regLength = wc == bigintMult.reg.Length ? RoundupSize(wc + 1) : bigintMult.reg.Length;
        product.reg = new short[regLength];
        product.reg[wc] = LinearMultiply(product.reg, 0, bigintMult.reg, 0, this.reg[0], wc);
        product.negative = false;
        product.wordCount = product.reg.Length;
        needShorten = false;
      } else if (bigintMult.wordCount == 1) {
        int wc = this.wordCount;
        int regLength = wc == this.reg.Length ? RoundupSize(wc + 1) : this.reg.Length;
        product.reg = new short[regLength];
        product.reg[wc] = LinearMultiply(product.reg, 0, this.reg, 0, bigintMult.reg[0], wc);
        product.negative = false;
        product.wordCount = product.reg.Length;
        needShorten = false;
      } else if (this.wordCount <= 10 && bigintMult.wordCount <= 10) {
        int wc = this.wordCount + bigintMult.wordCount;
        wc = (wc <= 16) ? RoundupSizeTable[wc] : RoundupSize(wc);
        product.reg = new short[wc];
        product.negative = false;
        product.wordCount = product.reg.Length;
        SchoolbookMultiply(
product.reg, 0,
                           this.reg, 0, this.wordCount,
                           bigintMult.reg, 0, bigintMult.wordCount);
        needShorten = false;
      } else if (this.Equals(bigintMult)) {
        int aSize = RoundupSize(this.wordCount);
        product.reg = new short[RoundupSize(aSize + aSize)];
        product.wordCount = product.reg.Length;
        product.negative = false;
        short[] workspace = new short[aSize + aSize];
        RecursiveSquare(
product.reg, 0,
                        workspace, 0,
                        this.reg, 0, aSize);
      } else {
        int aSize = this.wordCount;
        int bSize = bigintMult.wordCount;
        aSize = (aSize <= 16) ? RoundupSizeTable[aSize] : RoundupSize(aSize);
        bSize = (bSize <= 16) ? RoundupSizeTable[bSize] : RoundupSize(bSize);
        product.reg = new short[RoundupSize(aSize + bSize)];
        product.negative = false;
        short[] workspace = new short[aSize + bSize];
        product.wordCount = product.reg.Length;
        AsymmetricMultiply(
product.reg, 0,
                           workspace, 0,
                           this.reg, 0, aSize,
                           bigintMult.reg, 0, bSize);
      }
      // Recalculate word count
      while (product.wordCount != 0 && product.reg[product.wordCount - 1] == 0)
        product.wordCount--;
      if (needShorten)
        product.ShortenArray();
      if (this.negative != bigintMult.negative)
        product.NegateInternal();
      return product;
    }

    private static int BitsToWords(int bitCount) {
      return (bitCount + 15) >> 4;
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
      int idivisor = ((int)divisorSmall) & 0xFFFF;
      int quo, rem;
      while ((i--) > 0) {
        int currentDividend = unchecked((int)((((int)dividendReg[i]) & 0xFFFF) |
                                              ((int)remainder << 16)));
        if ((currentDividend >> 31) == 0) {
          quo = currentDividend / idivisor;
          quotientReg[i] = unchecked((short)quo);
          if (i > 0) {
            rem = currentDividend - (idivisor * quo);
            remainder = unchecked((short)rem);
          }
        } else {
          quotientReg[i] = DivideUnsigned(currentDividend, divisorSmall);
          if (i > 0) remainder = RemainderUnsigned(currentDividend, divisorSmall);
        }
      }
    }

    private static short FastDivideAndRemainderEx(
short[] quotientReg,
                                                  short[] dividendReg, int count, short divisorSmall) {
      int i = count;
      short remainder = 0;
      int idivisor = ((int)divisorSmall) & 0xFFFF;
      int quo, rem;
      while ((i--) > 0) {
        int currentDividend = unchecked((int)((((int)dividendReg[i]) & 0xFFFF) |
                                              ((int)remainder << 16)));
        if ((currentDividend >> 31) == 0) {
          quo = currentDividend / idivisor;
          quotientReg[i] = unchecked((short)quo);
          rem = currentDividend - (idivisor * quo);
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
      if (bigintDivisor == null)throw new ArgumentNullException("bigintDivisor");
      int aSize = this.wordCount;
      int bSize = bigintDivisor.wordCount;
      if (bSize == 0) {
 throw new DivideByZeroException();
}
      if (aSize < bSize) {
        // dividend is less than divisor (includes case
        // where dividend is 0)
        return BigInteger.Zero;
      }
      if (aSize <= 2 && bSize <= 2 && this.canFitInInt() && bigintDivisor.canFitInInt()) {
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
        quotient.reg = new short[this.reg.Length];
        quotient.wordCount = this.wordCount;
        quotient.negative = this.negative;
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
      short[] tempbuf = new short[aSize + 3 * (bSize + 2)];
      Divide(
null, 0,
             quotient.reg, 0,
             tempbuf, 0,
             this.reg, 0, aSize,
             bigintDivisor.reg, 0, bSize);
      quotient.wordCount = quotient.CalcWordCount();
      quotient.ShortenArray();
      if ((this.Sign < 0) ^ (bigintDivisor.Sign < 0)) {
        quotient.NegateInternal();
      }
      return quotient;
    }

    /// <summary> Not documented yet. </summary>
    /// <param name='divisor'>A BigInteger object.</param>
    /// <returns>A BigInteger[] object.</returns>
    public BigInteger[] divideAndRemainder(BigInteger divisor) {
      if (divisor == null) { throw new ArgumentNullException("divisor"); }
      BigInteger quotient;
      int aSize = this.wordCount;
      int bSize = divisor.wordCount;
      if (bSize == 0) {
 throw new DivideByZeroException();
}

      if (aSize < bSize) {
        // dividend is less than divisor (includes case
        // where dividend is 0)
        return new BigInteger[] { BigInteger.Zero, this };
      }
      if (bSize == 1) {
        // divisor is small, use a fast path
        quotient = new BigInteger();
        quotient.reg = new short[this.reg.Length];
        quotient.wordCount = this.wordCount;
        quotient.negative = this.negative;
        int smallRemainder = (((int)FastDivideAndRemainderEx(
          quotient.reg, this.reg, aSize, divisor.reg[0])) & 0xFFFF);
        while (quotient.wordCount != 0 &&
               quotient.reg[quotient.wordCount - 1] == 0)
          quotient.wordCount--;
        quotient.ShortenArray();
        if (quotient.wordCount != 0) {
          quotient.negative = this.negative ^ divisor.negative;
        } else {
          quotient = BigInteger.Zero;
        }
        if (this.negative) smallRemainder = -smallRemainder;
        return new BigInteger[] { quotient, new BigInteger().InitializeInt(smallRemainder) };
      }
      if (this.wordCount == 2 && divisor.wordCount == 2 &&
         (this.reg[1] >> 15) != 0 &&
         (divisor.reg[1] >> 15) != 0) {
        int a = ((int)this.reg[0]) & 0xFFFF;
        int b = ((int)divisor.reg[0]) & 0xFFFF;
        unchecked {
          a |= (((int)this.reg[1]) & 0xFFFF) << 16;
          b |= (((int)divisor.reg[1]) & 0xFFFF) << 16;
          int quo = a / b;
          if (this.negative)quo = -quo;
          int rem = a - (b * quo);
          return new BigInteger[] {
            new BigInteger().InitializeInt(quo),
            new BigInteger().InitializeInt(rem)
          };
        }
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
      Divide(
remainder.reg, 0,
             quotient.reg, 0,
             tempbuf, 0,
             this.reg, 0, aSize,
             divisor.reg, 0, bSize);
      remainder.wordCount = remainder.CalcWordCount();
      quotient.wordCount = quotient.CalcWordCount();
      // Console.WriteLine("Divd={0} divs={1} quo={2} rem={3}",this.wordCount,
      //                divisor.wordCount, quotient.wordCount, remainder.wordCount);
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
      if (divisor == null)throw new ArgumentNullException("divisor");
      if (divisor.Sign < 0) {
        throw new ArithmeticException("Divisor is negative");
      }
      BigInteger rem = this.remainder(divisor);
      if (rem.Sign < 0)
        rem = divisor.subtract(rem);
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
      int aSize = this.wordCount;
      int bSize = divisor.wordCount;
      if (bSize == 0) {
 throw new DivideByZeroException();
}
      if (aSize < bSize) {
        // dividend is less than divisor
        return this;
      }
      if (bSize == 1) {
        short shortRemainder = FastRemainder(this.reg, this.wordCount, divisor.reg[0]);
        int smallRemainder = ((int)shortRemainder) & 0xFFFF;
        if (this.negative) smallRemainder = -smallRemainder;
        return new BigInteger().InitializeInt(smallRemainder);
      }
      if (this.PositiveCompare(divisor) < 0) {
        if (divisor.IsZero) { throw new DivideByZeroException(); }
        return this;
      }
      BigInteger remainder = new BigInteger();
      aSize += aSize % 2;
      bSize += bSize % 2;
      remainder.reg = new short[RoundupSize((int)bSize)];
      remainder.negative = false;
      short[] tempbuf = new short[aSize + 3 * (bSize + 2)];
      Divide(
remainder.reg, 0,
             null, 0,
             tempbuf, 0,
             this.reg, 0, aSize,
             divisor.reg, 0, bSize);
      remainder.wordCount = remainder.CalcWordCount();
      remainder.ShortenArray();
      if (this.Sign < 0 && !remainder.IsZero) {
        remainder.NegateInternal();
      }
      return remainder;
    }

    void NegateInternal() {
      if (this.wordCount != 0)
        this.negative = this.Sign > 0;
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
      if (other == null) { return 1; }
      if (this == other)return 0;
      int size = this.wordCount, tSize = other.wordCount;
      int sa = size == 0 ? 0 : (this.negative ? -1 : 1);
      int sb = tSize == 0 ? 0 : (other.negative ? -1 : 1);
      if (sa != sb) { return (sa < sb) ? -1 : 1; }
      if (sa == 0) { return 0; }
      if (size == tSize) {
        if (size == 1 && this.reg[0] == other.reg[0]) {
          return 0;
        } else {
          short[] A = this.reg;
          short[] B = other.reg;
          while (unchecked(size--) != 0) {
            int an = ((int)A[size]) & 0xFFFF;
            int bn = ((int)B[size]) & 0xFFFF;
            if (an > bn)
              return (sa > 0) ? 1 : -1;
            else if (an < bn)
              return (sa > 0) ? -1 : 1;
          }
          return 0;
        }
      } else {
        return ((size > tSize) ^ (sa <= 0)) ? 1 : -1;
      }
    }
    /// <summary> Not documented yet. </summary>
    public int Sign {
      get {
        if (this.wordCount == 0)
          return 0;
        return this.negative ? -1 : 1;
      }
    }

    /// <summary> Not documented yet. </summary>
    public bool IsZero {
      get { return this.wordCount == 0; }
    }

    /// <summary> Finds the square root of this instance's value, rounded
    /// down.</summary>
    /// <returns>The square root of this object&apos; s value. Returns 0
    /// if this value is 0 or less.</returns>
    public BigInteger sqrt() {
      if (this.Sign <= 0)
        return BigInteger.Zero;
      BigInteger bigintX = null;
      BigInteger bigintY = Power2((this.getUnsignedBitLength() + 1) / 2);
      do {
        bigintX = bigintY;
        bigintY = this / (BigInteger)bigintX;
        bigintY += bigintX;
        bigintY >>= 1;
      } while (bigintY.CompareTo(bigintX) < 0);
      return bigintX;
    }

    public BigInteger[] sqrtWithRemainder() {
      if (this.Sign <= 0)
        return new BigInteger[]{ BigInteger.Zero, BigInteger.Zero };
      BigInteger bigintX = null;
      BigInteger bigintY = Power2((this.getUnsignedBitLength() + 1) / 2);
      do {
        bigintX = bigintY;
        bigintY = this / (BigInteger)bigintX;
        bigintY += bigintX;
        bigintY >>= 1;
      } while (bigintY.CompareTo(bigintX) < 0);
      bigintY = bigintX * (BigInteger)bigintX;
      bigintY = this - (BigInteger)bigintY;
      return new BigInteger[]{
        bigintX, bigintY
      };
    }

    /// <summary> Gets whether this value is even. </summary>
    public bool IsEven { get { return !this.GetUnsignedBit(0); } }

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
