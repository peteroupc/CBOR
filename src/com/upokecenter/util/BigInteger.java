package com.upokecenter.util;
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

    /**
     * An arbitrary-precision integer.
     */
  public final class BigInteger implements Comparable<BigInteger>
  {
    private static int CountWords(short[] X, int n) {
      while (n != 0 && X[n - 1] == 0) {
        n--;
      }
      return (int)n;
    }

    private static short ShiftWordsLeftByBits(short[] r, int rstart, int n, int shiftBits) {

      {
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
      // Debugif(!(shiftBits<16))Assert.fail("{0} line {1}: shiftBits<16","words.h",67);
      short u, carry = 0;
      {
        if (shiftBits != 0) {
          for (int i = n; i > 0; --i) {
            u = r[rstart + i - 1];
            r[rstart + i - 1] = (short)((((((int)u) & 0xFFFF) >> (int)shiftBits) & 0xFFFF) | (((int)carry) & 0xFFFF));
            carry = (short)((((int)u) & 0xFFFF) << (int)(16 - shiftBits));
          }
        }
        return carry;
      }
    }

    private static short ShiftWordsRightByBitsSignExtend(short[] r, int rstart, int n, int shiftBits) {
      // Debugif(!(shiftBits<16))Assert.fail("{0} line {1}: shiftBits<16","words.h",67);
      {
        short u, carry = (short)((int)0xFFFF << (int)(16 - shiftBits));
        if (shiftBits != 0) {
          for (int i = n; i > 0; --i) {
            u = r[rstart + i - 1];
            r[rstart + i - 1] = (short)(((((int)u) & 0xFFFF) >> (int)shiftBits) | (((int)carry) & 0xFFFF));
            carry = (short)((((int)u) & 0xFFFF) << (int)(16 - shiftBits));
          }
        }
        return carry;
      }
    }

    private static void ShiftWordsLeftByWords(short[] r, int rstart, int n, int shiftWords) {
      shiftWords = Math.min(shiftWords, n);
      if (shiftWords != 0) {
        for (int i = n - 1; i >= shiftWords; --i)
          r[rstart + i] = r[rstart + i - shiftWords];
        java.util.Arrays.fill(r,rstart,(rstart)+(shiftWords),(short)0);
      }
    }

    private static void ShiftWordsRightByWords(short[] r, int rstart, int n, int shiftWords) {
      shiftWords = Math.min(shiftWords, n);
      if (shiftWords != 0) {
        for (int i = 0; i + shiftWords < n; ++i)
          r[rstart + i] = r[rstart + i + shiftWords];
        rstart = rstart + n - shiftWords;
        java.util.Arrays.fill(r,rstart,(rstart)+(shiftWords),(short)0);
      }
    }

    private static void ShiftWordsRightByWordsSignExtend(short[] r, int rstart, int n, int shiftWords) {
      shiftWords = Math.min(shiftWords, n);
      if (shiftWords != 0) {
        for (int i = 0; i + shiftWords < n; ++i)
          r[rstart + i] = r[rstart + i + shiftWords];
        rstart = rstart + n - shiftWords;
        // Sign extend
        for (int i = 0; i < shiftWords; ++i)
          r[rstart + i] = ((short)0xFFFF);
      }
    }

    private static int Compare(short[] words1, int astart, short[] words2, int bstart, int n) {
      while ((n--) != 0) {
        int an = ((int)words1[astart + n]) & 0xFFFF;
        int bn = ((int)words2[bstart + n]) & 0xFFFF;
        if (an > bn) {
          return 1;
        } else if (an < bn) {
          return -1;
        }
      }
      return 0;
    }

    private static int Increment(short[] words1, int words1Start, int n, short words2) {
      {
        // Debugif(!(n!=0))Assert.fail("{0} line {1}: n","integer.cpp",63);
        short tmp = words1[words1Start];
        words1[words1Start] = (short)(tmp + words2);
        if ((((int)words1[words1Start]) & 0xFFFF) >= (((int)tmp) & 0xFFFF))
          return 0;
        for (int i = 1; i < n; ++i) {
          words1[words1Start + i]++;
          if (words1[words1Start + i] != 0) {
            return 0;
          }
        }
        return 1;
      }
    }

    private static int Decrement(short[] words1, int words1Start, int n, short words2) {
      // Debugif(!(n!=0))Assert.fail("{0} line {1}: n","integer.cpp",76);
      {
        short tmp = words1[words1Start];
        words1[words1Start] = (short)(tmp - words2);
        if ((((int)words1[words1Start]) & 0xFFFF) <= (((int)tmp) & 0xFFFF)) {
          return 0;
        }
        for (int i = 1; i < n; ++i) {
          tmp = words1[words1Start + i];
          words1[words1Start + i]--;
          if (tmp != 0) {
            return 0;
          }
        }
        return 1;
      }
    }

    private static void TwosComplement(short[] words1, int words1Start, int n) {
      Decrement(words1, words1Start, n, (short)1);
      for (int i = 0; i < n; ++i)
        words1[words1Start + i] = ((short)(~words1[words1Start + i]));
    }

    private static int Add(
      short[] C, int cstart,
      short[] words1, int astart,
      short[] words2, int bstart, int n) {
      // Debugif(!(n%2 == 0))Assert.fail("{0} line {1}: n%2 == 0","integer.cpp",799);
      {
        int u;
        u = 0;
        for (int i = 0; i < n; i += 2) {
          u = (((int)words1[astart + i]) & 0xFFFF) + (((int)words2[bstart + i]) & 0xFFFF) + (short)(u >> 16);
          C[cstart + i] = (short)u;
          u = (((int)words1[astart + i + 1]) & 0xFFFF) + (((int)words2[bstart + i + 1]) & 0xFFFF) + (short)(u >> 16);
          C[cstart + i + 1] = (short)u;
        }
        return ((int)u >> 16) & 0xFFFF;
      }
    }

    private static int Subtract(
      short[] C, int cstart,
      short[] words1, int astart,
      short[] words2, int bstart, int n) {
      // Debugif(!(n%2 == 0))Assert.fail("{0} line {1}: n%2 == 0","integer.cpp",799);
      {
        int u;
        u = 0;
        for (int i = 0; i < n; i += 2) {
          u = (((int)words1[astart]) & 0xFFFF) - (((int)words2[bstart]) & 0xFFFF) - (int)((u >> 31) & 1);
          C[cstart++] = (short)u;
          astart++;
          bstart++;
          u = (((int)words1[astart]) & 0xFFFF) - (((int)words2[bstart]) & 0xFFFF) - (int)((u >> 31) & 1);
          C[cstart++] = (short)u;
          astart++;
          bstart++;
        }
        return (int)((u >> 31) & 1);
      }
    }

    private static short LinearMultiply(
      short[] productArr, int cstart,
      short[] words1, int astart, short words2, int n) {
      {
        short carry = 0;
        int Bint = ((int)words2) & 0xFFFF;
        for (int i = 0; i < n; ++i) {
          int p;
          p = (((int)words1[astart + i]) & 0xFFFF) * Bint;
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

    private static void Baseline_Square2(short[] R, int rstart, short[] words1, int astart) {
      {
        int p; short c; int d; int e;
        p = (((int)words1[astart]) & 0xFFFF) * (((int)words1[astart]) & 0xFFFF); R[rstart] = (short)p; e = ((int)p >> 16) & 0xFFFF;
        p = (((int)words1[astart]) & 0xFFFF) * (((int)words1[astart + 1]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 4 -  3] = c;
        p = (((int)words1[astart + 2 - 1]) & 0xFFFF) * (((int)words1[astart + 2 - 1]) & 0xFFFF);
        p += e; R[rstart + 4 -  2] = (short)p; R[rstart + 4 -  1] = (short)(p >> 16);
      }
    }

    private static void Baseline_Square4(short[] R, int rstart, short[] words1, int astart) {
      {
        int p; short c; int d; int e;
        p = (((int)words1[astart]) & 0xFFFF) * (((int)words1[astart]) & 0xFFFF); R[rstart] = (short)p; e = ((int)p >> 16) & 0xFFFF;
        p = (((int)words1[astart]) & 0xFFFF) * (((int)words1[astart + 1]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 1] = c;
        p = (((int)words1[astart]) & 0xFFFF) * (((int)words1[astart + 2]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 1]) & 0xFFFF) * (((int)words1[astart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 2] = c;
        p = (((int)words1[astart]) & 0xFFFF) * (((int)words1[astart + 3]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)words1[astart + 1]) & 0xFFFF) * (((int)words1[astart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 3] = c;
        p = (((int)words1[astart + 1]) & 0xFFFF) * (((int)words1[astart + 3]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 2]) & 0xFFFF) * (((int)words1[astart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 4] = c;
        p = (((int)words1[astart + 2]) & 0xFFFF) * (((int)words1[astart + 3]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 2 * 4 - 3] = c;
        p = (((int)words1[astart + 4 - 1]) & 0xFFFF) * (((int)words1[astart + 4 - 1]) & 0xFFFF);
        p += e; R[rstart + 2 * 4 - 2] = (short)p; R[rstart + 2 * 4 - 1] = (short)(p >> 16);
      }
    }

    private static void Baseline_Square8(short[] R, int rstart, short[] words1, int astart) {
      {
        int p; short c; int d; int e;
        p = (((int)words1[astart]) & 0xFFFF) * (((int)words1[astart]) & 0xFFFF); R[rstart] = (short)p; e = ((int)p >> 16) & 0xFFFF;
        p = (((int)words1[astart]) & 0xFFFF) * (((int)words1[astart + 1]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 1] = c;
        p = (((int)words1[astart]) & 0xFFFF) * (((int)words1[astart + 2]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 1]) & 0xFFFF) * (((int)words1[astart + 1]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 2] = c;
        p = (((int)words1[astart]) & 0xFFFF) * (((int)words1[astart + 3]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)words1[astart + 1]) & 0xFFFF) * (((int)words1[astart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 3] = c;
        p = (((int)words1[astart]) & 0xFFFF) * (((int)words1[astart + 4]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)words1[astart + 1]) & 0xFFFF) * (((int)words1[astart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 2]) & 0xFFFF) * (((int)words1[astart + 2]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 4] = c;
        p = (((int)words1[astart]) & 0xFFFF) * (((int)words1[astart + 5]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)words1[astart + 1]) & 0xFFFF) * (((int)words1[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)words1[astart + 2]) & 0xFFFF) * (((int)words1[astart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 5] = c;
        p = (((int)words1[astart]) & 0xFFFF) * (((int)words1[astart + 6]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)words1[astart + 1]) & 0xFFFF) * (((int)words1[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)words1[astart + 2]) & 0xFFFF) * (((int)words1[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 3]) & 0xFFFF) * (((int)words1[astart + 3]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 6] = c;
        p = (((int)words1[astart]) & 0xFFFF) * (((int)words1[astart + 7]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)words1[astart + 1]) & 0xFFFF) * (((int)words1[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)words1[astart + 2]) & 0xFFFF) * (((int)words1[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)words1[astart + 3]) & 0xFFFF) * (((int)words1[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 7] = c;
        p = (((int)words1[astart + 1]) & 0xFFFF) * (((int)words1[astart + 7]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)words1[astart + 2]) & 0xFFFF) * (((int)words1[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)words1[astart + 3]) & 0xFFFF) * (((int)words1[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 4]) & 0xFFFF) * (((int)words1[astart + 4]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 8] = c;
        p = (((int)words1[astart + 2]) & 0xFFFF) * (((int)words1[astart + 7]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)words1[astart + 3]) & 0xFFFF) * (((int)words1[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = (((int)words1[astart + 4]) & 0xFFFF) * (((int)words1[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 9] = c;
        p = (((int)words1[astart + 3]) & 0xFFFF) * (((int)words1[astart + 7]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)words1[astart + 4]) & 0xFFFF) * (((int)words1[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 5]) & 0xFFFF) * (((int)words1[astart + 5]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 10] = c;
        p = (((int)words1[astart + 4]) & 0xFFFF) * (((int)words1[astart + 7]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF;
        p = (((int)words1[astart + 5]) & 0xFFFF) * (((int)words1[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 11] = c;
        p = (((int)words1[astart + 5]) & 0xFFFF) * (((int)words1[astart + 7]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 6]) & 0xFFFF) * (((int)words1[astart + 6]) & 0xFFFF);
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 12] = c;
        p = (((int)words1[astart + 6]) & 0xFFFF) * (((int)words1[astart + 7]) & 0xFFFF); c = (short)p; d = ((int)p >> 16) & 0xFFFF; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1; e = e + (((int)c) & 0xFFFF); c = (short)e; e = d + (((int)e >> 16) & 0xFFFF); R[rstart + 2 * 8 - 3] = c;
        p = (((int)words1[astart + 8 - 1]) & 0xFFFF) * (((int)words1[astart + 8 - 1]) & 0xFFFF);
        p += e; R[rstart + 2 * 8 - 2] = (short)p; R[rstart + 2 * 8 - 1] = (short)(p >> 16);
      }
    }

    //---------------------
    //  Baseline multiply
    //---------------------

    private static void Baseline_Multiply2(short[] R, int rstart, short[] words1, int astart, short[] words2, int bstart) {
      {
        int p; short c; int d;
        int a0 = ((int)words1[astart]) & 0xFFFF;
        int a1 = ((int)words1[astart + 1]) & 0xFFFF;
        int b0 = ((int)words2[bstart]) & 0xFFFF;
        int b1 = ((int)words2[bstart + 1]) & 0xFFFF;
        p = a0 * b0; c = (short)p; d = ((int)p >> 16) & 0xFFFF; R[rstart] = c; c = (short)d; d = ((int)d >> 16) & 0xFFFF;
        p = a0 * b1;
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF);
        p = a1 * b0;
        p = p + (((int)c) & 0xFFFF); c = (short)p; d = d + (((int)p >> 16) & 0xFFFF); R[rstart + 1] = c;
        p = a1 * b1;
        p += d; R[rstart + 1 + 1] = (short)p; R[rstart + 1 + 2] = (short)(p >> 16);
      }
    }

    private static void Baseline_Multiply4(short[] R, int rstart, short[] words1, int astart, short[] words2, int bstart) {
      int mask = 0xFFFF;
      {
        int p; short c; int d;
        int a0 = ((int)words1[astart]) &  mask;
        int b0 = ((int)words2[bstart]) &  mask;
        p = a0 * b0; c = (short)p; d = ((int)p >> 16) & mask; R[rstart] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = a0 * (((int)words2[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 1]) & mask) * b0;
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 1] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = a0 * (((int)words2[bstart + 2]) & mask);

        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 1]) & mask) * (((int)words2[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 2]) & mask) * b0;
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 2] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = a0 * (((int)words2[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 1]) & mask) * (((int)words2[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);

        p = (((int)words1[astart + 2]) & mask) * (((int)words2[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 3]) & mask) * b0;
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 3] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)words1[astart + 1]) & mask) * (((int)words2[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 2]) & mask) * (((int)words2[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 3]) & mask) * (((int)words2[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 4] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)words1[astart + 2]) & mask) * (((int)words2[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 3]) & mask) * (((int)words2[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 5] = c;
        p = (((int)words1[astart + 3]) & mask) * (((int)words2[bstart + 3]) & mask);
        p += d; R[rstart + 5 + 1] = (short)p; R[rstart + 5 + 2] = (short)(p >> 16);
      }
    }

    private static void Baseline_Multiply8(short[] R, int rstart, short[] words1, int astart, short[] words2, int bstart) {
      int mask = 0xFFFF;
      {
        int p; short c; int d;
        p = (((int)words1[astart]) & mask) * (((int)words2[bstart]) & mask); c = (short)p; d = ((int)p >> 16) & mask; R[rstart] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)words1[astart]) & mask) * (((int)words2[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 1]) & mask) * (((int)words2[bstart]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 1] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)words1[astart]) & mask) * (((int)words2[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 1]) & mask) * (((int)words2[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 2]) & mask) * (((int)words2[bstart]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 2] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)words1[astart]) & mask) * (((int)words2[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 1]) & mask) * (((int)words2[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 2]) & mask) * (((int)words2[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 3]) & mask) * (((int)words2[bstart]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 3] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)words1[astart]) & mask) * (((int)words2[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 1]) & mask) * (((int)words2[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 2]) & mask) * (((int)words2[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 3]) & mask) * (((int)words2[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 4]) & mask) * (((int)words2[bstart]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 4] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)words1[astart]) & mask) * (((int)words2[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 1]) & mask) * (((int)words2[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 2]) & mask) * (((int)words2[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 3]) & mask) * (((int)words2[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 4]) & mask) * (((int)words2[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 5]) & mask) * (((int)words2[bstart]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 5] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)words1[astart]) & mask) * (((int)words2[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 1]) & mask) * (((int)words2[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 2]) & mask) * (((int)words2[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 3]) & mask) * (((int)words2[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 4]) & mask) * (((int)words2[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 5]) & mask) * (((int)words2[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 6]) & mask) * (((int)words2[bstart]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 6] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)words1[astart]) & mask) * (((int)words2[bstart + 7]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 1]) & mask) * (((int)words2[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 2]) & mask) * (((int)words2[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 3]) & mask) * (((int)words2[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 4]) & mask) * (((int)words2[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 5]) & mask) * (((int)words2[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 6]) & mask) * (((int)words2[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 7]) & mask) * (((int)words2[bstart]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 7] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)words1[astart + 1]) & mask) * (((int)words2[bstart + 7]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 2]) & mask) * (((int)words2[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 3]) & mask) * (((int)words2[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 4]) & mask) * (((int)words2[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 5]) & mask) * (((int)words2[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 6]) & mask) * (((int)words2[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 7]) & mask) * (((int)words2[bstart + 1]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 8] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)words1[astart + 2]) & mask) * (((int)words2[bstart + 7]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 3]) & mask) * (((int)words2[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 4]) & mask) * (((int)words2[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 5]) & mask) * (((int)words2[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 6]) & mask) * (((int)words2[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 7]) & mask) * (((int)words2[bstart + 2]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 9] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)words1[astart + 3]) & mask) * (((int)words2[bstart + 7]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 4]) & mask) * (((int)words2[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 5]) & mask) * (((int)words2[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 6]) & mask) * (((int)words2[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 7]) & mask) * (((int)words2[bstart + 3]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 10] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)words1[astart + 4]) & mask) * (((int)words2[bstart + 7]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 5]) & mask) * (((int)words2[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 6]) & mask) * (((int)words2[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 7]) & mask) * (((int)words2[bstart + 4]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 11] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)words1[astart + 5]) & mask) * (((int)words2[bstart + 7]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 6]) & mask) * (((int)words2[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 7]) & mask) * (((int)words2[bstart + 5]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 12] = c; c = (short)d; d = ((int)d >> 16) & mask;
        p = (((int)words1[astart + 6]) & mask) * (((int)words2[bstart + 7]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask);
        p = (((int)words1[astart + 7]) & mask) * (((int)words2[bstart + 6]) & mask);
        p = p + (((int)c) & mask); c = (short)p; d = d + (((int)p >> 16) & mask); R[rstart + 13] = c;
        p = (((int)words1[astart + 7]) & mask) * (((int)words2[bstart + 7]) & mask);
        p += d; R[rstart + 13 + 1] = (short)p; R[rstart + 13 + 2] = (short)(p >> 16);
      }
    }

    private static final int RecursionLimit = 8;

    private static void RecursiveMultiply(
      short[] resultArr,  // size 2*n
      int resultStart,
      short[] tempArr,  // size 2*n
      int tempStart,
      short[] words1, int words1Start,  // size n
      short[] words2, int words2Start,  // size n
      int count) {
      if (count <= RecursionLimit) {
        count >>= 2;
        if (count == 0) {
          Baseline_Multiply2(resultArr, resultStart, words1, words1Start, words2, words2Start);
        } else if (count == 1) {
          Baseline_Multiply4(resultArr, resultStart, words1, words1Start, words2, words2Start);
        } else if (count == 2) {
          Baseline_Multiply8(resultArr, resultStart, words1, words1Start, words2, words2Start);
        } else {
          throw new IllegalStateException();
        }
      } else {
        int count2 = count >> 1;
        int rMediumHigh = resultStart + count;
        int rHigh = rMediumHigh + count2;
        int rMediumLow = resultStart + count2;
        int tsn = tempStart + count;
        int countA = count;
        while (countA != 0 && words1[words1Start + countA - 1] == 0) {
          countA--;
        }
        int countB = count;
        while (countB != 0 && words2[words2Start + countB - 1] == 0) {
          countB--;
        }
        int count2For1 = 0;
        int count2For2 = 0;
        if (countA == 0 || countB == 0) {
          // words1 or words2 is empty, so result is 0
          java.util.Arrays.fill(resultArr,resultStart,(resultStart)+(count << 1),(short)0);
          return;
        }
        if (countA <= count2 && countB <= count2) {
          // System.out.println("Can be smaller: {0},{1},{2}",AN,BN,count2);
          java.util.Arrays.fill(resultArr,resultStart + count,(resultStart + count)+(count),(short)0);
          if (count2 == 8) {
            Baseline_Multiply8(resultArr, resultStart, words1, words1Start, words2, words2Start);
          } else {
            RecursiveMultiply(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words2, words2Start, count2);
          }
          return;
        }
        count2For1 = Compare(words1, words1Start, words1, (int)(words1Start + count2), count2) > 0 ? 0 : count2;
        Subtract(resultArr, resultStart, words1, (int)(words1Start + count2For1), words1, (int)(words1Start + (count2 ^ count2For1)), count2);
        count2For2 = Compare(words2, words2Start, words2, (int)(words2Start + count2), count2) > 0 ? 0 : count2;
        Subtract(resultArr, rMediumLow, words2, (int)(words2Start + count2For2), words2, (int)(words2Start + (count2 ^ count2For2)), count2);
        //---------
        // Medium high result = HighA * HighB
        RecursiveMultiply(resultArr, rMediumHigh, tempArr, tsn, words1, (int)(words1Start + count2), words2, (int)(words2Start + count2), count2);
        // Medium high result = Abs(LowA-HighA) * Abs(LowB-HighB)
        RecursiveMultiply(tempArr, tempStart, tempArr, tsn, resultArr, resultStart, resultArr, (int)rMediumLow, count2);
        // Low result = LowA * LowB
        RecursiveMultiply(resultArr, resultStart, tempArr, tsn, words1, words1Start, words2, words2Start, count2);
        //
        int c2 = Add(resultArr, rMediumHigh, resultArr, rMediumHigh, resultArr, rMediumLow, count2);
        int c3 = c2;
        c2 += Add(resultArr, rMediumLow, resultArr, rMediumHigh, resultArr, resultStart, count2);
        c3 += Add(resultArr, rMediumHigh, resultArr, rMediumHigh, resultArr, rHigh, count2);
        if (count2For1 == count2For2) {
          c3 -= Subtract(resultArr, rMediumLow, resultArr, rMediumLow, tempArr, tempStart, count);
        } else {
          c3 += Add(resultArr, rMediumLow, resultArr, rMediumLow, tempArr, tempStart, count);
        }
        c3 += Increment(resultArr, rMediumHigh, count2, (short)c2);
        if (c3 != 0) {
          Increment(resultArr, rHigh, count2, (short)c3);
        }
      }
    }

    private static void RecursiveSquare(
      short[] resultArr,
      int resultStart,
      short[] tempArr,
      int tempStart, short[] words1, int words1Start, int n) {
      if (n <= RecursionLimit) {
        n >>= 2;
        switch (n) {
          case 0:
            Baseline_Square2(resultArr, resultStart, words1, words1Start);
            break;
          case 1:
            Baseline_Square4(resultArr, resultStart, words1, words1Start);
            break;
          case 2:
            Baseline_Square8(resultArr, resultStart, words1, words1Start);
            break;
          default:
            throw new IllegalStateException();
        }
      } else {
        int count2 = n >> 1;

        RecursiveSquare(resultArr, resultStart, tempArr, (int)(tempStart + n), words1, words1Start, count2);
        RecursiveSquare(resultArr, (int)(resultStart + n), tempArr, (int)(tempStart + n), words1, (int)(words1Start + count2), count2);
        RecursiveMultiply(
          tempArr, tempStart, tempArr, (int)(tempStart + n),
          words1, words1Start, words1, (int)(words1Start + count2), count2);

        int carry = Add(resultArr, (int)(resultStart + count2), resultArr, (int)(resultStart + count2), tempArr, tempStart, n);
        carry += Add(resultArr, (int)(resultStart + count2), resultArr, (int)(resultStart + count2), tempArr, tempStart, n);

        Increment(resultArr, (int)(resultStart + n + count2), count2, (short)carry);
      }
    }

    private static void SchoolbookMultiply(
      short[] resultArr, int resultStart,
      short[] words1, int words1Start, int NA, short[] words2, int words2Start, int NB) {
      // Method assumes that resultArr was already zeroed
      int cstart;
      if (NA < NB) {
        // words1 is shorter than words2, so put words2 on top
        for (int i = 0; i < NA; ++i) {
          cstart = resultStart + i;
          {
            short carry = 0;
            int Bint = ((int)words1[words1Start + i]) & 0xFFFF;
            for (int j = 0; j < NB; ++j) {
              int p;
              p = (((int)words2[words2Start + j]) & 0xFFFF) * Bint;
              p = p + (((int)carry) & 0xFFFF);
              if (i != 0) {
                p += ((int)resultArr[cstart + j]) & 0xFFFF;
              }
              resultArr[cstart + j] = (short)p;
              carry = (short)(p >> 16);
            }
            resultArr[cstart + NB] = carry;
          }
        }
      } else {
        // words2 is shorter than words1
        for (int i = 0; i < NB; ++i) {
          cstart = resultStart + i;
          {
            short carry = 0;
            int Bint = ((int)words2[words2Start + i]) & 0xFFFF;
            for (int j = 0; j < NA; ++j) {
              int p;
              p = (((int)words1[words1Start + j]) & 0xFFFF) * Bint;
              p = p + (((int)carry) & 0xFFFF);
              if (i != 0) {
                p += ((int)resultArr[cstart + j]) & 0xFFFF;
              }
              resultArr[cstart + j] = (short)p;
              carry = (short)(p >> 16);
            }
            resultArr[cstart + NA] = carry;
          }
        }
      }
    }

    private static void AsymmetricMultiply(
      short[] resultArr,
      int resultStart,
      short[] tempArr,
      int tempStart, short[] words1, int words1Start, int NA, short[] words2, int words2Start, int NB) {
      if (NA == NB) {
        if (words1Start == words2Start && words1 == words2) {
          RecursiveSquare(resultArr, resultStart, tempArr, tempStart, words1, words1Start, NA);
        } else if (NA == 2) {
          Baseline_Multiply2(resultArr, resultStart, words1, words1Start, words2, words2Start);
        } else {
          RecursiveMultiply(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words2, words2Start, NA);
        }

        return;
      }

      if (NA > NB) {
        short[] tmp1 = words1; words1 = words2; words2 = tmp1;
        int tmp3 = words1Start; words1Start = words2Start; words2Start = tmp3;
        int tmp2 = NA; NA = NB; NB = tmp2;
      }

      if (NA == 2 && words1[words1Start + 1] == 0) {
        switch (words1[words1Start]) {
          case 0:
            java.util.Arrays.fill(resultArr,resultStart,(resultStart)+(NB + 2),(short)0);
            return;
          case 1:
            System.arraycopy(words2, words2Start, resultArr, resultStart, (int)NB);
            resultArr[resultStart + NB] = (short)0;
            resultArr[resultStart + NB + 1] = (short)0;
            return;
          default:
            resultArr[resultStart + NB] = LinearMultiply(resultArr, resultStart, words2, words2Start, words1[words1Start], NB);
            resultArr[resultStart + NB + 1] = (short)0;
            return;
        }
      } else if (NA == 2) {
        int a0 = ((int)words1[words1Start]) & 0xFFFF;
        int a1 = ((int)words1[words1Start + 1]) & 0xFFFF;
        resultArr[resultStart + NB] = (short)0;
        resultArr[resultStart + NB + 1] = (short)0;
        AtomicMultiplyOpt(resultArr, resultStart, a0, a1, words2, words2Start, 0, NB);
        AtomicMultiplyAddOpt(resultArr, resultStart, a0, a1, words2, words2Start, 2, NB);
        return;
      } else {
        int i;
        if (((NB / NA) & 1) == 0) {
          RecursiveMultiply(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words2, words2Start, NA);
          System.arraycopy(resultArr, (int)(resultStart + NA), tempArr, (int)(tempStart + (NA << 1)), (int)NA);
          for (i = NA << 1; i < NB; i += NA << 1)
            RecursiveMultiply(tempArr, (int)(tempStart + NA + i), tempArr, tempStart, words1, words1Start, words2, (int)(words2Start + i), NA);
          for (i = NA; i < NB; i += NA << 1)
            RecursiveMultiply(resultArr, (int)(resultStart + i), tempArr, tempStart, words1, words1Start, words2, (int)(words2Start + i), NA);
        } else {
          for (i = 0; i < NB; i += NA << 1)
            RecursiveMultiply(resultArr, (int)(resultStart + i), tempArr, tempStart, words1, words1Start, words2, (int)(words2Start + i), NA);
          for (i = NA; i < NB; i += NA << 1)
            RecursiveMultiply(tempArr, (int)(tempStart + NA + i), tempArr, tempStart, words1, words1Start, words2, (int)(words2Start + i), NA);
        }
        if (Add(resultArr, (int)(resultStart + NA), resultArr, (int)(resultStart + NA), tempArr, (int)(tempStart + (NA << 1)), NB - NA) != 0)
          Increment(resultArr, (int)(resultStart + NB), NA, (short)1);
      }
    }

    private static int MakeUint(short first, short second) {
      return ((int)((((int)first) & 0xFFFF) | ((int)second << 16)));
    }

    private static short GetLowHalf(int val) {
      return ((short)(val & 0xFFFF));
    }

    private static short GetHighHalf(int val) {
      return ((short)((val >> 16) & 0xFFFF));
    }

    private static short GetHighHalfAsBorrow(int val) {
      return ((short)(0 - ((val >> 16) & 0xFFFF)));
    }

    private static int BitPrecision(short numberValue) {
      if (numberValue == 0) {
        return 0;
      }
      int i = 16;
      {
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

        if ((numberValue >> 15) == 0) {
          --i;
        }
      }
      return i;
    }

    private static int BitPrecisionInt(int numberValue) {
      if (numberValue == 0) {
        return 0;
      }
      int i = 32;
      {
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

        if ((numberValue >> 31) == 0) {
          --i;
        }
      }
      return i;
    }

    private static short Divide32By16(int dividendLow, short divisorShort, boolean returnRemainder) {
      int tmpInt;
      int dividendHigh = 0;
      int intDivisor = ((int)divisorShort) & 0xFFFF;
      for (int i = 0; i < 32; ++i) {
        tmpInt = dividendHigh >> 31;
        dividendHigh <<= 1;
        dividendHigh = ((int)(dividendHigh | ((int)((dividendLow >> 31) & 1))));
        dividendLow <<= 1;
        tmpInt |= dividendHigh;
        // unsigned greater-than-or-equal check
        if (((tmpInt >> 31) != 0) || (tmpInt >= intDivisor)) {
          {
            dividendHigh -= intDivisor;
            dividendLow += 1;
          }
        }
      }
      return (returnRemainder ?
              ((short)(((int)dividendHigh) & 0xFFFF)) :
              ((short)(((int)dividendLow) & 0xFFFF))
             );
    }

    private static short DivideUnsigned(int x, short y) {
      {
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
      {
        int iy = ((int)y) & 0xFFFF;
        if ((x >> 31) == 0) {
          // x is already nonnegative
          return (short)(((int)x % iy) & 0xFFFF);
        } else {
          return Divide32By16(x, y, true);
        }
      }
    }

    private static short DivideThreeWordsByTwo(short[] words1, int words1Start, short B0, short B1) {
      // Debugif(!(words1[2] < B1 || (words1[2]==B1 && words1[1] < B0)))Assert.fail("{0} line {1}: words1[2] < B1 || (words1[2]==B1 && words1[1] < B0)","integer.cpp",360);
      short Q;
      {
        if ((short)(B1 + 1) == 0)
          Q = words1[words1Start + 2];
        else if (B1 != 0) {
          Q = DivideUnsigned(MakeUint(words1[words1Start + 1], words1[words1Start + 2]), (short)(((int)B1 + 1) & 0xFFFF));
        } else {
          Q = DivideUnsigned(MakeUint(words1[words1Start], words1[words1Start + 1]), B0);
        }

        int Qint = ((int)Q) & 0xFFFF;
        int B0int = ((int)B0) & 0xFFFF;
        int B1int = ((int)B1) & 0xFFFF;
        int p = B0int * Qint;
        int u = (((int)words1[words1Start]) & 0xFFFF) - (p & 0xFFFF);
        words1[words1Start] = GetLowHalf(u);
        u = (((int)words1[words1Start + 1]) & 0xFFFF) - ((p >> 16) & 0xFFFF) -
          (((int)GetHighHalfAsBorrow(u)) & 0xFFFF) - (B1int * Qint);
        words1[words1Start + 1] = GetLowHalf(u);
        words1[words1Start + 2] += GetHighHalf(u);
        while (words1[words1Start + 2] != 0 ||
               (((int)words1[words1Start + 1]) & 0xFFFF) > (((int)B1) & 0xFFFF) ||
               (words1[words1Start + 1] == B1 && (((int)words1[words1Start]) & 0xFFFF) >= (((int)B0) & 0xFFFF))) {
          u = (((int)words1[words1Start]) & 0xFFFF) - B0int;
          words1[words1Start] = GetLowHalf(u);
          u = (((int)words1[words1Start + 1]) & 0xFFFF) - B1int - (((int)GetHighHalfAsBorrow(u)) & 0xFFFF);
          words1[words1Start + 1] = GetLowHalf(u);
          words1[words1Start + 2] += GetHighHalf(u);
          Q++;
        }
      }
      return Q;
    }

    private static void AtomicDivide(
      short[] quotient, int quotientStart, short[] words1, int words1Start,
      short word2A, short word2B, short[] temp) {
      if (word2A == 0 && word2B == 0) {
        quotient[quotientStart] = words1[words1Start];
        quotient[quotientStart + 1] = words1[words1Start + 3];
      } else {
        temp[0] = words1[words1Start];
        temp[1] = words1[words1Start + 1];
        temp[2] = words1[words1Start + 2];
        temp[3] = words1[words1Start + 3];
        short Q1 = DivideThreeWordsByTwo(temp, 1, word2A, word2B);
        short Q0 = DivideThreeWordsByTwo(temp, 0, word2A, word2B);
        quotient[quotientStart] = Q0;
        quotient[quotientStart + 1] = Q1;
      }
    }

    private static void AtomicMultiplyOpt(short[] C, int Cstart, int A0, int A1, short[] words2, int words2Start, int istart, int iend) {
      short s;
      int d;
      int first1MinusFirst0 = ((int)A1 - A0) & 0xFFFF;
      A1 &= 0xFFFF;
      A0 &= 0xFFFF;
      {
        if (A1 >= A0) {
          for (int i = istart; i < iend; i += 4) {
            int B0 = ((int)words2[words2Start + i]) & 0xFFFF;
            int B1 = ((int)words2[words2Start + i + 1]) & 0xFFFF;
            int csi = Cstart + i;
            if (B0 >= B1)
            {
              s = (short)0;
              d = first1MinusFirst0 * (((int)B0 - B1) & 0xFFFF);
            } else {
              s = (short)first1MinusFirst0;
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
            int B0 = ((int)words2[words2Start + i]) & 0xFFFF;
            int B1 = ((int)words2[words2Start + i + 1]) & 0xFFFF;
            int csi = Cstart + i;
            if (B0 > B1) {
              s = (short)(((int)B0 - B1) & 0xFFFF);
              d = first1MinusFirst0 * (((int)s) & 0xFFFF);
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

    private static void AtomicMultiplyAddOpt(short[] C, int Cstart, int A0, int A1, short[] words2, int words2Start, int istart, int iend) {
      short s;
      int d;
      int first1MinusFirst0 = ((int)A1 - A0) & 0xFFFF;
      A1 &= 0xFFFF;
      A0 &= 0xFFFF;
      {
        if (A1 >= A0) {
          for (int i = istart; i < iend; i += 4) {
            int b0 = ((int)words2[words2Start + i]) & 0xFFFF;
            int b1 = ((int)words2[words2Start + i + 1]) & 0xFFFF;
            int csi = Cstart + i;
            if (b0 >= b1)
            {
              s = (short)0;
              d = first1MinusFirst0 * (((int)b0 - b1) & 0xFFFF);
            } else {
              s = (short)first1MinusFirst0;
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
            int B0 = ((int)words2[words2Start + i]) & 0xFFFF;
            int B1 = ((int)words2[words2Start + i + 1]) & 0xFFFF;
            int csi = Cstart + i;
            if (B0 > B1) {
              s = (short)(((int)B0 - B1) & 0xFFFF);
              d = first1MinusFirst0 * (((int)s) & 0xFFFF);
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

            tempInt =  (((int)(tempInt >> 16)) & 0xFFFF) + a1b1high + (((int)C[csi + 3]) & 0xFFFF);
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
      short[] resultArr, int resultStart,  // remainder
      short[] Qarr, int Qstart,  // quotient
      short[] TA, int tempStart,  // scratch space
      short[] words1, int words1Start, int NAint,  // dividend
      short[] words2, int words2Start, int NBint  // divisor
     ) {
      // set up temporary work space
      int NA = (int)NAint;
      int NB = (int)NBint;

      short[] TBarr = TA;
      short[] TParr = TA;
      short[] quot = Qarr;
      if (Qarr == null) {
        quot = new short[2];
      }
      int TBstart = (int)(tempStart + (NA + 2));
      int TPstart = (int)(tempStart + (NA + 2 + NB));
      {
        // copy words2 into TB and normalize it so that TB has highest bit set to 1
        int shiftWords = (short)(words2[words2Start + NB - 1] == 0 ? 1 : 0);
        TBarr[TBstart] = (short)0;
        TBarr[TBstart + NB - 1] = (short)0;
        System.arraycopy(words2, words2Start, TBarr, (int)(TBstart + shiftWords), NB - shiftWords);
        short shiftBits = (short)((short)16 - BitPrecision(TBarr[TBstart + NB - 1]));
        ShiftWordsLeftByBits(
          TBarr,
          TBstart,
          NB,
          shiftBits);
        // copy words1 into TA and normalize it
        TA[0] = (short)0;
        TA[NA] = (short)0;
        TA[NA + 1] = (short)0;
        System.arraycopy(words1, words1Start, TA, (int)(tempStart + shiftWords), NAint);
        ShiftWordsLeftByBits(
          TA,
          tempStart,
          NA + 2,
          shiftBits);

        if (TA[tempStart + NA + 1] == 0 && (((int)TA[tempStart + NA]) & 0xFFFF) <= 1) {
          if (Qarr != null) {
            Qarr[Qstart + NA - NB + 1] = (short)0;
            Qarr[Qstart + NA - NB] = (short)0;
          }
          while (
            TA[NA] != 0 || Compare(
              TA, (int)(tempStart + NA - NB),
              TBarr, TBstart, NB) >= 0) {
            TA[NA] -= (
              short)Subtract(
              TA, (int)(tempStart + NA - NB),
              TA, (int)(tempStart + NA - NB),
              TBarr, TBstart, NB);
            if (Qarr != null) {
              Qarr[Qstart + NA - NB] += (short)1;
            }
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
          AtomicDivide(quot, qs, TA, (int)(tempStart + i - 2), BT0, BT1, TAtomic);
          // now correct the underestimated quotient
          int Rstart2 = tempStart + i - NB;
          int n = NB;
          {
            int Q0 = quot[qs];
            int Q1 = quot[qs + 1];
            if (Q1 == 0) {
              short carry = LinearMultiply(TParr, TPstart, TBarr, TBstart, (short)Q0, n);
              TParr[TPstart + n] = carry;
              TParr[TPstart + n + 1] = 0;
            } else if (n == 2) {
              Baseline_Multiply2(TParr, TPstart, quot, qs, TBarr, TBstart);
            } else {
              TParr[TPstart + n] = (short)0;
              TParr[TPstart + n + 1] = (short)0;
              Q0 &= 0xFFFF;
              Q1 &= 0xFFFF;
              AtomicMultiplyOpt(TParr, TPstart, Q0, Q1, TBarr, TBstart, 0, n);
              AtomicMultiplyAddOpt(TParr, TPstart, Q0, Q1, TBarr, TBstart, 2, n);
            }
            Subtract(TA, Rstart2, TA, Rstart2, TParr, TPstart, n + 2);
            while (TA[Rstart2 + n] != 0 || Compare(TA, Rstart2, TBarr, TBstart, n) >= 0) {
              TA[Rstart2 + n] -= (short)Subtract(TA, Rstart2, TA, Rstart2, TBarr, TBstart, n);
              if (Qarr != null) {
                Qarr[qs]++;
                Qarr[qs + 1] += (short)((Qarr[qs] == 0) ? 1 : 0);
              }
            }
          }
        }
        if (resultArr != null) {  // If the remainder is non-null
          // copy TA into R, and denormalize it
          System.arraycopy(TA, (int)(tempStart + shiftWords), resultArr, resultStart, NB);
          ShiftWordsRightByBits(resultArr, resultStart, NB, shiftBits);
        }
      }
    }

    private static int[] RoundupSizeTable = new int[] {
      2, 2, 2, 4, 4, 8, 8, 8, 8,
      16, 16, 16, 16, 16, 16, 16, 16
    };

    private static int RoundupSize(int n) {
      if (n <= 16) {
        return RoundupSizeTable[n];
      } else if (n <= 32) {
        return 32;
      } else if (n <= 64) {
        return 64;
      } else {
        return (int)1 << (int)BitPrecisionInt(n - 1);
      }
    }

    private boolean negative;
    private int wordCount = -1;
    private short[] reg;
    /**
     * Initializes a BigInteger object set to zero.
     */
    private BigInteger() {}

    /**
     * Initializes a BigInteger object from an array of bytes.
     * @param bytes A byte[] object.
     * @param littleEndian A Boolean object.
     * @return A BigInteger object.
     */
    public static BigInteger fromByteArray(byte[] bytes, boolean littleEndian) {
      BigInteger bigint = new BigInteger();
      bigint.fromByteArrayInternal(bytes, littleEndian);
      return bigint;
    }

    private void fromByteArrayInternal(byte[] bytes, boolean littleEndian) {
      if (bytes == null) {
        throw new NullPointerException("bytes");
      }
      if (bytes.length == 0) {
        this.reg = new short[] { (short)0, (short)0 };
        this.wordCount = 0;
      } else {
        int len = bytes.length;
        int wordLength = ((int)len + 1) >> 1;
        wordLength = (wordLength <= 16) ?
          RoundupSizeTable[wordLength] :
          RoundupSize(wordLength);
        this.reg = new short[wordLength];
        int jIndex = littleEndian ? len - 1 : 0;
        boolean negative = (bytes[jIndex] & 0x80) != 0;
        this.negative = negative;
        int j = 0;
        if (!negative) {
          for (int i = 0; i < len; i += 2, j++) {
            int index = littleEndian ? i : len - 1 - i;
            int index2 = littleEndian ? i + 1 : len - 2 - i;
            this.reg[j] = (short)(((int)bytes[index]) & 0xFF);
            if (index2 >= 0 && index2 < len) {
              this.reg[j] |= ((short)(((short)bytes[index2]) << 8));
            }
          }
        } else {
          for (int i = 0; i < len; i += 2, j++) {
            int index = littleEndian ? i : len - 1 - i;
            int index2 = littleEndian ? i + 1 : len - 2 - i;
            this.reg[j] = (short)(((int)bytes[index]) & 0xFF);
            if (index2 >= 0 && index2 < len) {
              this.reg[j] |= ((short)(((short)bytes[index2]) << 8));
            } else {
              // sign extend the last byte
              this.reg[j] |= ((short)0xFF00);
            }
          }
          for (; j < this.reg.length; ++j) {
            this.reg[j] = ((short)0xFFFF);  // sign extend remaining words
          }
          TwosComplement(this.reg, 0, (int)this.reg.length);
        }
        this.wordCount = this.reg.length;
        while (this.wordCount != 0 &&
               this.reg[this.wordCount - 1] == 0) {
          this.wordCount--;
        }
      }
    }

    private BigInteger Allocate(int length) {
      this.reg = new short[RoundupSize(length)];  // will be initialized to 0
      this.negative = false;
      this.wordCount = 0;
      return this;
    }

    private static short[] GrowForCarry(short[] a, short carry) {
      int oldLength = a.length;
      short[] ret = CleanGrow(a, RoundupSize(oldLength + 1));
      ret[oldLength] = carry;
      return ret;
    }

    private static short[] CleanGrow(short[] a, int size) {
      if (size > a.length) {
        short[] newa = new short[size];
        System.arraycopy(a,0,newa,0,a.length);
        return newa;
      }
      return a;
    }

    private void SetBitInternal(int n, boolean value) {
      if (value) {
        this.reg = CleanGrow(this.reg, RoundupSize(BitsToWords(n + 1)));
        this.reg[(n >> 4)] |= (short)((short)1 << (int)(n & 0xf));
        this.wordCount = this.CalcWordCount();
      } else {
        if ((n >> 4) < this.reg.length) {
          this.reg[(n >> 4)] &= ((short)(~((short)1 << (int)(n % 16))));
        }
        this.wordCount = this.CalcWordCount();
      }
    }

    /**
     * Not documented yet.
     * @param index A 32-bit unsigned integer.
     * @return A Boolean object.
     */
    public boolean testBit(int index) {
      if (index < 0) {
        throw new IllegalArgumentException("index");
      }
      if (this.signum() < 0) {
        int tcindex = 0;
        int wordpos = index / 16;
        if (wordpos >= this.reg.length) {
          return true;
        }
        while (tcindex < wordpos && this.reg[tcindex] == 0) {
          tcindex++;
        }
        short tc;
        {
          tc = this.reg[wordpos];
          if (tcindex == wordpos) {
            tc--;
          }
          tc = (short)~tc;
        }
        return (boolean)(((tc >> (int)(index & 15)) & 1) != 0);
      } else {
        return this.GetUnsignedBit(index);
      }
    }

    /**
     * Not documented yet.
     * @param n A 32-bit unsigned integer.
     */
    private boolean GetUnsignedBit(int n) {

      if ((n >> 4) >= this.reg.length) {
        return false;
      } else {
        return (boolean)(((this.reg[(n >> 4)] >> (int)(n & 15)) & 1) != 0);
      }
    }

    private BigInteger InitializeInt(int numberValue) {
      int iut;
      {
        this.negative = numberValue < 0;
        if (numberValue == Integer.MIN_VALUE) {
          this.reg = new short[2];
          this.reg[0] = 0;
          this.reg[1] = (short)0x8000;
          this.wordCount = 2;
        } else {
          iut = ((numberValue < 0) ? (int)-numberValue : (int)numberValue);
          this.reg = new short[2];
          this.reg[0] = (short)iut;
          this.reg[1] = (short)(iut >> 16);
          this.wordCount = this.reg[1] != 0 ? 2 : (this.reg[0] == 0 ? 0 : 1);
        }
      }
      return this;
    }

    /**
     * Returns a byte array of this object&apos;s value.
     * @param littleEndian A Boolean object.
     * @return A byte array that represents the value of this object.
     */
    public byte[] toByteArray(boolean littleEndian) {
      int sign = this.signum();
      if (sign == 0) {
        return new byte[]{ (byte)0 };
      } else if (sign > 0) {
        int byteCount = this.ByteCount();
        int byteArrayLength = byteCount;
        if (this.GetUnsignedBit((byteCount * 8) - 1)) {
          byteArrayLength++;
        }
        byte[] bytes = new byte[byteArrayLength];
        int j = 0;
        for (int i = 0; i < byteCount; i += 2, j++) {
          int index = littleEndian ? i : bytes.length - 1 - i;
          int index2 = littleEndian ? i + 1 : bytes.length - 2 - i;
          bytes[index] = (byte)(this.reg[j] & 0xff);
          if (index2 >= 0 && index2 < byteArrayLength) {
            bytes[index2] = (byte)((this.reg[j] >> 8) & 0xff);
          }
        }
        return bytes;
      } else {
        short[] regdata = new short[this.reg.length];
        System.arraycopy(this.reg,0,regdata,0,this.reg.length);
        TwosComplement(regdata, 0, (int)regdata.length);
        int byteCount = regdata.length * 2;
        for (int i = regdata.length - 1; i >= 0; --i) {
          if (regdata[i] == ((short)0xFFFF)) {
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
        if (byteCount == 0) {
          byteCount = 1;
        }
        byte[] bytes = new byte[byteCount];
        bytes[littleEndian ? bytes.length - 1 : 0] = (byte)0xFF;
        byteCount = Math.min(byteCount, regdata.length * 2);
        int j = 0;
        for (int i = 0; i < byteCount; i += 2, j++) {
          int index = littleEndian ? i : bytes.length - 1 - i;
          int index2 = littleEndian ? i + 1 : bytes.length - 2 - i;
          bytes[index] = (byte)(regdata[j] & 0xff);
          if (index2 >= 0 && index2 < byteCount) {
            bytes[index2] = (byte)((regdata[j] >> 8) & 0xff);
          }
        }
        return bytes;
      }
    }

    /**
     * Shifts this object&apos;s value by a number of bits. A value of 1 doubles
     * this value, a value of 2 multiplies it by 4, a value of 3 by 8, a value of
     * 4 by 16, and so on.
     * @param numberBits The number of bits to shift. Can be negative, in
     * which case this is the same as shiftRight with the absolute value of
     * numberBits.
     * @return A BigInteger object.
     */
    public BigInteger shiftLeft(int numberBits) {
      if (numberBits == 0) {
        return this;
      }
      if (numberBits < 0) {
        if (numberBits == Integer.MIN_VALUE) {
          return this.shiftRight(1).shiftRight(Integer.MAX_VALUE);
        }
        return this.shiftRight(-numberBits);
      }
      BigInteger ret = new BigInteger();
      int numWords = (int)this.wordCount;
      int shiftWords = (int)(numberBits >> 4);
      int shiftBits = (int)(numberBits & 15);
      boolean neg = numWords > 0 && this.negative;
      if (!neg) {
        ret.negative = false;
        ret.reg = new short[RoundupSize(numWords + BitsToWords((int)numberBits))];
        System.arraycopy(this.reg,0,ret.reg,0,numWords);
        ShiftWordsLeftByWords(ret.reg, 0, numWords + shiftWords, shiftWords);
        ShiftWordsLeftByBits(ret.reg, (int)shiftWords, numWords + BitsToWords(shiftBits), shiftBits);
        ret.wordCount = ret.CalcWordCount();
      } else {
        ret.negative = true;
        ret.reg = new short[RoundupSize(numWords + BitsToWords((int)numberBits))];
        System.arraycopy(this.reg,0,ret.reg,0,numWords);
        TwosComplement(ret.reg, 0, (int)ret.reg.length);
        ShiftWordsLeftByWords(ret.reg, 0, numWords + shiftWords, shiftWords);
        ShiftWordsLeftByBits(ret.reg, (int)shiftWords, numWords + BitsToWords(shiftBits), shiftBits);
        TwosComplement(ret.reg, 0, (int)ret.reg.length);
        ret.wordCount = ret.CalcWordCount();
      }
      return ret;
    }

    /**
     * Not documented yet.
     * @param numberBits A 32-bit signed integer.
     * @return A BigInteger object.
     */
    public BigInteger shiftRight(int numberBits) {
      if (numberBits == 0) {
        return this;
      }
      if (numberBits < 0) {
        if (numberBits == Integer.MIN_VALUE) {
          return this.shiftLeft(1).shiftLeft(Integer.MAX_VALUE);
        }
        return this.shiftLeft(-numberBits);
      }
      BigInteger ret = new BigInteger();
      int numWords = (int)this.wordCount;
      int shiftWords = (int)(numberBits >> 4);
      int shiftBits = (int)(numberBits & 15);
      ret.negative = this.negative;
      ret.reg = new short[RoundupSize(numWords)];
      System.arraycopy(this.reg,0,ret.reg,0,numWords);
      if (this.signum() < 0) {
        TwosComplement(ret.reg, 0, (int)ret.reg.length);
        ShiftWordsRightByWordsSignExtend(ret.reg, 0, numWords, shiftWords);
        if (numWords > shiftWords) {
          ShiftWordsRightByBitsSignExtend(ret.reg, 0, numWords - shiftWords, shiftBits);
        }
        TwosComplement(ret.reg, 0, (int)ret.reg.length);
      } else {
        ShiftWordsRightByWords(ret.reg, 0, numWords, shiftWords);
        if (numWords > shiftWords) {
          ShiftWordsRightByBits(ret.reg, 0, numWords - shiftWords, shiftBits);
        }
      }
      ret.wordCount = ret.CalcWordCount();
      return ret;
    }

    /**
     * Not documented yet.
     * @param longerValue A 64-bit signed integer.
     * @return A BigInteger object.
     */
    public static BigInteger valueOf(long longerValue) {
      if (longerValue == 0) {
        return BigInteger.ZERO;
      }
      if (longerValue == 1) {
        return BigInteger.ONE;
      }
      BigInteger ret = new BigInteger();
      {
        ret.negative = longerValue < 0;
        ret.reg = new short[4];
        if (longerValue == Long.MIN_VALUE) {
          ret.reg[0] = 0;
          ret.reg[1] = 0;
          ret.reg[2] = 0;
          ret.reg[3] = (short)0x8000;
          ret.wordCount = 4;
        } else {
          long ut = longerValue;
          if (ut < 0) {
            ut = -ut;
          }
          ret.reg[0] = (short)(ut & 0xFFFF);
          ut >>= 16;
          ret.reg[1] = (short)(ut & 0xFFFF);
          ut >>= 16;
          ret.reg[2] = (short)(ut & 0xFFFF);
          ut >>= 16;
          ret.reg[3] = (short)(ut & 0xFFFF);
          // at this point, the word count can't
          // be 0 (the check for 0 was already done above)
          ret.wordCount = 4;
          while (ret.wordCount != 0 &&
                 ret.reg[ret.wordCount - 1] == 0) {
            ret.wordCount--;
          }
        }
      }
      return ret;
    }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
    public int intValue() {
      int c = (int)this.wordCount;
      if (c == 0) {
        return 0;
      }
      if (c > 2) {
        throw new ArithmeticException();
      }
      if (c == 2 && (this.reg[1] & 0x8000) != 0) {
        if (((short)(this.reg[1] & (short)0x7FFF) | this.reg[0]) == 0 && this.negative) {
          return Integer.MIN_VALUE;
        } else {
          throw new ArithmeticException();
        }
      } else {
        int ivv = ((int)this.reg[0]) & 0xFFFF;
        if (c > 1) {
          ivv |= (((int)this.reg[1]) & 0xFFFF) << 16;
        }
        if (this.negative) {
          ivv = -ivv;
        }
        return ivv;
      }
    }

    /**
     * Not documented yet.
     * @return A Boolean object.
     */
    public boolean canFitInInt() {
      int c = (int)this.wordCount;
      if (c > 2) {
        return false;
      }
      if (c == 2 && (this.reg[1] & 0x8000) != 0) {
        return (this.negative && this.reg[1] == ((short)0x8000) &&
                this.reg[0] == 0);
      }
      return true;
    }

    private boolean HasSmallValue() {
      int c = (int)this.wordCount;
      if (c > 4) {
        return false;
      }
      if (c == 4 && (this.reg[3] & 0x8000) != 0) {
        return (this.negative && this.reg[3] == ((short)0x8000) &&
                this.reg[2] == 0 &&
                this.reg[1] == 0 &&
                this.reg[0] == 0);
      }
      return true;
    }

    /**
     * Not documented yet.
     * @return A 64-bit signed integer.
     */
    public long longValue() {
      int count = this.wordCount;
      if (count == 0) {
        return (long)0;
      }
      if (count > 4) {
        throw new ArithmeticException();
      }
      if (count == 4 && (this.reg[3] & 0x8000) != 0) {
        if (this.negative && this.reg[3] == ((short)0x8000) &&
            this.reg[2] == 0 &&
            this.reg[1] == 0 &&
            this.reg[0] == 0) {
          return Long.MIN_VALUE;
        } else {
          throw new ArithmeticException();
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
        if (this.negative) {
          vv = -vv;
        }
        return vv;
      }
    }

    private static BigInteger Power2(int e) {
      BigInteger r = new BigInteger().Allocate(BitsToWords((int)(e + 1)));
      r.SetBitInternal((int)e, true);  // NOTE: Will recalculate word count
      return r;
    }

    /**
     * Not documented yet.
     * @param power A BigInteger object. (2).
     * @return A BigInteger object.
     */
    public BigInteger PowBigIntVar(BigInteger power) {
      if (power == null) {
        throw new NullPointerException("power");
      }
      int sign = power.signum();
      if (sign < 0) {
        throw new IllegalArgumentException("power is negative");
      }
      BigInteger thisVar = this;
      if (sign == 0) {
        return BigInteger.ONE;
      }  // however 0 to the power of 0 is undefined
      else if (power.equals(BigInteger.ONE)) {
        return this;
      } else if (power.wordCount == 1 && power.reg[0] == 2) {
        return thisVar.multiply(thisVar);
      } else if (power.wordCount == 1 && power.reg[0] == 3) {
        return (thisVar.multiply(thisVar)).multiply(thisVar);
      }
      BigInteger r = BigInteger.ONE;
      while (power.signum()!=0) {
        if (power.testBit(0)) {
          r = r.multiply(thisVar);
        }
        power=power.shiftRight(1);
        if (power.signum()!=0) {
          thisVar = thisVar.multiply(thisVar);
        }
      }
      return r;
    }

    /**
     * Not documented yet.
     * @param powerSmall A 32-bit signed integer.
     * @return A BigInteger object.
     */
    public BigInteger pow(int powerSmall) {
      if (powerSmall < 0) {
        throw new IllegalArgumentException("power is negative");
      }
      BigInteger thisVar = this;
      if (powerSmall == 0) {
        return BigInteger.ONE;
      }  // however 0 to the power of 0 is undefined
      else if (powerSmall == 1) {
        return this;
      } else if (powerSmall == 2) {
        return thisVar.multiply(thisVar);
      } else if (powerSmall == 3) {
        return (thisVar.multiply(thisVar)).multiply(thisVar);
      }
      BigInteger r = BigInteger.ONE;
      while (powerSmall != 0) {
        if ((powerSmall & 1) != 0) {
          r = r.multiply(thisVar);
        }
        powerSmall >>= 1;
        if (powerSmall != 0) {
          thisVar = thisVar.multiply(thisVar);
        }
      }
      return r;
    }

    /**
     * Not documented yet.
     * @return A BigInteger object.
     */
    public BigInteger negate() {
      BigInteger bigintRet = new BigInteger();
      bigintRet.reg = this.reg;  // use the same reference
      bigintRet.wordCount = this.wordCount;
      bigintRet.negative = (this.wordCount != 0) && (!this.negative);
      return bigintRet;
    }

    /**
     * Not documented yet.
     * @return A BigInteger object.
     */
    public BigInteger abs() {
      return (this.wordCount == 0 || !this.negative) ? this : this.negate();
    }

    /**
     * Not documented yet.
     */
    private int CalcWordCount() {
      return (int)CountWords(this.reg, this.reg.length);
    }

    /**
     * Not documented yet.
     */
    private int ByteCount() {
      int wc = this.wordCount;
      if (wc == 0) {
        return 0;
      }
      short s = this.reg[wc - 1];
      wc = (wc - 1) << 1;
      if (s == 0) {
        return wc;
      }
      return ((s >> 8) == 0) ? wc + 1 : wc + 2;
    }

    /**
     * Finds the minimum number of bits needed to represent this object&apos;s
     * absolute value.
     * @return The number of bits in this object&apos;s value. Returns 0
     * if this object&apos;s value is 0, and returns 1 if the value is negative
     * 1.
     */
    public int getUnsignedBitLength() {
      int wc = this.wordCount;
      if (wc != 0) {
        int numberValue = ((int)this.reg[wc - 1]) & 0xFFFF;
        wc = (wc - 1) << 4;
        if (numberValue == 0) {
          return wc;
        }
        wc += 16;
        {
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
          if ((numberValue >> 15) == 0) {
            --wc;
          }
        }
        return wc;
      } else {
        return 0;
      }
    }

    /**
     * Not documented yet.
     * @param reg A short[] object.
     * @param wordCount A 32-bit signed integer.
     * @return A 32-bit signed integer.
     */
    private static int getUnsignedBitLengthEx(int numberValue, int wordCount) {
      int wc = wordCount;
      if (wc != 0) {
        wc = (wc - 1) << 4;
        if (numberValue == 0) {
          return wc;
        }
        wc += 16;
        {
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
          if ((numberValue >> 15) == 0) {
            --wc;
          }
        }
        return wc;
      } else {
        return 0;
      }
    }

    /**
     * Finds the minimum number of bits needed to represent this object&apos;s
     * value, except for its sign. If the value is negative, finds the number
     * of bits in (its absolute value minus 1).
     * @return The number of bits in this object&apos;s value. Returns 0
     * if this object&apos;s value is 0 or negative 1.
     */
    public int bitLength() {
      int wc = this.wordCount;
      if (wc != 0) {
        int numberValue = ((int)this.reg[wc - 1]) & 0xFFFF;
        wc = (wc - 1) << 4;
        if (numberValue == (this.negative ? 1 : 0)) {
          return wc;
        }
        wc += 16;
        {
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

    private static final String vec = "0123456789ABCDEF";

    private static void ReverseChars(char[] chars, int offset, int length) {
      int half = length >> 1;
      int right = offset + length - 1;
      for (int i = 0; i < half; i++, right--) {
        char value = chars[offset + i];
        chars[offset + i] = chars[right];
        chars[right] = value;
      }
    }

    private String SmallValueToString() {
      long value = this.longValue();
      if (value == Long.MIN_VALUE) {
        return "-9223372036854775808";
      }
      boolean neg = value < 0;
      char[] chars = new char[24];
      int count = 0;
      if (neg) {
        chars[0] = '-';
        count++;
        value = -value;
      }
      while (value != 0) {
        char digit = vec.charAt((int)(value % 10));
        chars[count++] = digit;
        value = value / 10;
      }
      if (neg) {
        ReverseChars(chars, 1, count - 1);
      } else {
        ReverseChars(chars, 0, count);
      }
      return new String(chars, 0, count);
    }

    private static int ApproxLogTenOfTwo(int bitlen) {
      int bitlenLow = bitlen & 0xFFFF;
      int bitlenHigh = (bitlen >> 16) & 0xFFFF;
      short resultLow = 0;
      short resultHigh = 0;
      {
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
        p = ((int)c) & 0xFFFF; c = (short)p; resultLow = c; c = (short)d; d = ((int)d >> 16) & 0xFFFF;
        p = bitlenHigh * 0x9A;
        p = p + (((int)c) & 0xFFFF);
        resultHigh = (short)p;
        int result = ((int)resultLow) & 0xFFFF;
        result |= (((int)resultHigh) & 0xFFFF) << 16;
        return (result & 0x7FFFFFFF) >> 9;
      }
    }

    /**
     * Finds the number of decimal digits this number has.
     * @return The number of decimal digits. Returns 1 if this object&apos;
     * s value is 0.
     */
    public int getDigitCount() {
      if (this.signum()==0) {
        return 1;
      }
      if (this.HasSmallValue()) {
        long value = this.longValue();
        if (value == Long.MIN_VALUE) {
          return 19;
        }
        if (value < 0) {
          value = -value;
        }
        if (value >= 1000000000L) {
          if (value >= 1000000000000000000L) {
            return 19;
          }
          if (value >= 100000000000000000L) {
            return 18;
          }
          if (value >= 10000000000000000L) {
            return 17;
          }
          if (value >= 1000000000000000L) {
            return 16;
          }
          if (value >= 100000000000000L) {
            return 15;
          }
          if (value >= 10000000000000L) {
            return 14;
          }
          if (value >= 1000000000000L) {
            return 13;
          }
          if (value >= 100000000000L) {
            return 12;
          }
          if (value >= 10000000000L) {
            return 11;
          }
          if (value >= 1000000000L) {
            return 10;
          }
          return 9;
        } else {
          int v2 = (int)value;
          if (v2 >= 100000000) {
            return 9;
          }
          if (v2 >= 10000000) {
            return 8;
          }
          if (v2 >= 1000000) {
            return 7;
          }
          if (v2 >= 100000) {
            return 6;
          }
          if (v2 >= 10000) {
            return 5;
          }
          if (v2 >= 1000) {
            return 4;
          }
          if (v2 >= 100) {
            return 3;
          }
          if (v2 >= 10) {
            return 2;
          }
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
          if (rest >= 10000) {
            i += 5;
          } else if (rest >= 1000) {
            i += 4;
          } else if (rest >= 100) {
            i += 3;
          } else if (rest >= 10) {
            i += 2;
          } else {
            i++;
          }
          break;
        } else if (wordCount == 2 && tempReg[1] > 0 && tempReg[1] <= 0x7FFF) {
          int rest = ((int)tempReg[0]) & 0xFFFF;
          rest |= (((int)tempReg[1]) & 0xFFFF) << 16;
          if (rest >= 1000000000) {
            i += 10;
          } else if (rest >= 100000000) {
            i += 9;
          } else if (rest >= 10000000) {
            i += 8;
          } else if (rest >= 1000000) {
            i += 7;
          } else if (rest >= 100000) {
            i += 6;
          } else if (rest >= 10000) {
            i += 5;
          } else if (rest >= 1000) {
            i += 4;
          } else if (rest >= 100) {
            i += 3;
          } else if (rest >= 10) {
            i += 2;
          } else {
            i++;
          }
          break;
        } else {
          int wci = wordCount;
          short remainderShort = 0;
          int quo, rem;
          boolean firstdigit = false;
          short[] dividend = (tempReg == null) ? this.reg : tempReg;
          // Divide by 10000
          while ((wci--) > 0) {
            int curValue = ((int)dividend[wci]) & 0xFFFF;
            int currentDividend = ((int)(curValue |
                                                  ((int)remainderShort << 16)));
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
                System.arraycopy(this.reg,0,tempReg,0,tempReg.length);
                // Use the calculated word count during division;
                // zeros that may have occurred in division
                // are not incorporated in the tempReg
                wordCount = wci + 1;
                tempReg[wci] = ((short)quo);
              }
            } else {
              tempReg[wci] = ((short)quo);
            }
            rem = currentDividend - (10000 * quo);
            remainderShort = ((short)rem);
          }
          // Recalculate word count
          while (wordCount != 0 && tempReg[wordCount - 1] == 0) {
            wordCount--;
          }
          i += 4;
        }
      }
      return i;
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      if (this.signum()==0) {
        return "0";
      }
      if (this.HasSmallValue()) {
        return this.SmallValueToString();
      }
      short[] tempReg = new short[this.wordCount];
      System.arraycopy(this.reg,0,tempReg,0,tempReg.length);
      int wordCount = tempReg.length;
      while (wordCount != 0 && tempReg[wordCount - 1] == 0) {
        wordCount--;
      }
      int i = 0;
      char[] s = new char[(wordCount << 4) + 1];
      while (wordCount != 0) {
        if (wordCount == 1 && tempReg[0] > 0 && tempReg[0] <= 0x7FFF) {
          int rest = tempReg[0];
          while (rest != 0) {
            // accurate approximation to rest/10 up to 43698,
            // and rest can go up to 32767
            int newrest = (rest * 26215) >> 18;
            s[i++] = vec.charAt(rest - (newrest * 10));
            rest = newrest;
          }
          break;
        } else if (wordCount == 2 && tempReg[1] > 0 && tempReg[1] <= 0x7FFF) {
          int rest = ((int)tempReg[0]) & 0xFFFF;
          rest |= (((int)tempReg[1]) & 0xFFFF) << 16;
          while (rest != 0) {
            int newrest = rest / 10;
            s[i++] = vec.charAt(rest - (newrest * 10));
            rest = newrest;
          }
          break;
        } else {
          int wci = wordCount;
          short remainderShort = 0;
          int quo, rem;
          // Divide by 10000
          while ((wci--) > 0) {
            int currentDividend = ((int)((((int)tempReg[wci]) & 0xFFFF) |
                                                  ((int)remainderShort << 16)));
            quo = currentDividend / 10000;
            tempReg[wci] = ((short)quo);
            rem = currentDividend - (10000 * quo);
            remainderShort = ((short)rem);
          }
          int remainderSmall = remainderShort;
          // Recalculate word count
          while (wordCount != 0 && tempReg[wordCount - 1] == 0) {
            wordCount--;
          }
          // accurate approximation to rest/10 up to 16388,
          // and rest can go up to 9999
          int newrest = (remainderSmall * 3277) >> 15;
          s[i++] = vec.charAt((int)(remainderSmall - (newrest * 10)));
          remainderSmall = newrest;
          newrest = (remainderSmall * 3277) >> 15;
          s[i++] = vec.charAt((int)(remainderSmall - (newrest * 10)));
          remainderSmall = newrest;
          newrest = (remainderSmall * 3277) >> 15;
          s[i++] = vec.charAt((int)(remainderSmall - (newrest * 10)));
          remainderSmall = newrest;
          s[i++] = vec.charAt(remainderSmall);
        }
      }
      ReverseChars(s, 0, i);
      if (this.negative) {
        StringBuilder sb = new StringBuilder(i + 1);
        sb.append('-');
        sb.append(s,0,(0)+(i));
        return sb.toString();
      } else {
        return new String(s, 0, i);
      }
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return A BigInteger object.
     */
    public static BigInteger fromString(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      return fromSubstring(str, 0, str.length());
    }

    private static final int MaxSafeInt = 214748363;

    public static BigInteger fromSubstring(String str, int index, int endIndex) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index < 0) {
        throw new IllegalArgumentException("\"str\"" + " not greater or equal to " + "0" + " (" + Long.toString((long)index) + ")");
      }
      if (index > str.length()) {
        throw new IllegalArgumentException("\"str\"" + " not less or equal to " + Long.toString((long)str.length()) + " (" + Long.toString((long)index) + ")");
      }
      if (endIndex < 0) {
        throw new IllegalArgumentException("\"index\"" + " not greater or equal to " + "0" + " (" + Long.toString((long)endIndex) + ")");
      }
      if (endIndex > str.length()) {
        throw new IllegalArgumentException("\"index\"" + " not less or equal to " + Long.toString((long)str.length()) + " (" + Long.toString((long)endIndex) + ")");
      }
      if (endIndex < index) {
        throw new IllegalArgumentException("\"endIndex\"" + " not greater or equal to " + Long.toString((long)index) + " (" + Long.toString((long)endIndex) + ")");
      }
      if (index == endIndex) {
        throw new NumberFormatException("No digits");
      }
      boolean negative = false;
      if (str.charAt(0) == '-') {
        index++;
        negative = true;
      }
      BigInteger bigint = new BigInteger().Allocate(4);
      boolean haveDigits = false;
      boolean haveSmallInt = true;
      int smallInt = 0;
      for (int i = index; i < endIndex; ++i) {
        char c = str.charAt(i);
        if (c < '0' || c > '9') {
          throw new NumberFormatException("Illegal character found");
        }
        haveDigits = true;
        int digit = (int)(c - '0');
        if (haveSmallInt && smallInt < MaxSafeInt) {
          smallInt *= 10;
          smallInt += digit;
        } else {
          if (haveSmallInt) {
            bigint.reg[0] = ((short)(smallInt & 0xFFFF));
            bigint.reg[1] = ((short)((smallInt >> 16) & 0xFFFF));
            haveSmallInt = false;
          }
          // Multiply by 10
          short carry = 0;
          int n = bigint.reg.length;
          for (int j = 0; j < n; ++j) {
            int p;
            {
              p = (((int)bigint.reg[j]) & 0xFFFF) * 10;
              p = p + (((int)carry) & 0xFFFF);
              bigint.reg[j] = (short)p;
              carry = (short)(p >> 16);
            }
          }
          if (carry != 0) {
            bigint.reg = GrowForCarry(bigint.reg, carry);
          }
          // Add the parsed digit
          if (digit != 0) {
            int d = bigint.reg[0] & 0xFFFF;
            if (d <= 65526) {
              bigint.reg[0] = ((short)(d + digit));
            } else if (Increment(bigint.reg, 0, bigint.reg.length, (short)digit) != 0) {
              bigint.reg = GrowForCarry(bigint.reg, (short)1);
            }
          }
        }
      }
      if (!haveDigits) {
        throw new NumberFormatException("No digits");
      }
      if (haveSmallInt) {
        bigint.reg[0] = ((short)(smallInt & 0xFFFF));
        bigint.reg[1] = ((short)((smallInt >> 16) & 0xFFFF));
      }
      bigint.wordCount = bigint.CalcWordCount();
      bigint.negative = bigint.wordCount != 0 && negative;
      return bigint;
    }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
    public int getLowestSetBit() {
      int retSetBit = 0;
      for (int i = 0; i < this.wordCount; ++i) {
        short c = this.reg[i];
        if (c == (short)0) {
          retSetBit += 16;
        } else {
          if (((c << 15) & 0xFFFF) != 0) {
            return retSetBit + 0;
          }
          if (((c << 14) & 0xFFFF) != 0) {
            return retSetBit + 1;
          }
          if (((c << 13) & 0xFFFF) != 0) {
            return retSetBit + 2;
          }
          if (((c << 12) & 0xFFFF) != 0) {
            return retSetBit + 3;
          }
          if (((c << 11) & 0xFFFF) != 0) {
            return retSetBit + 4;
          }
          if (((c << 10) & 0xFFFF) != 0) {
            return retSetBit + 5;
          }
          if (((c << 9) & 0xFFFF) != 0) {
            return retSetBit + 6;
          }
          if (((c << 8) & 0xFFFF) != 0) {
            return retSetBit + 7;
          }
          if (((c << 7) & 0xFFFF) != 0) {
            return retSetBit + 8;
          }
          if (((c << 6) & 0xFFFF) != 0) {
            return retSetBit + 9;
          }
          if (((c << 5) & 0xFFFF) != 0) {
            return retSetBit + 10;
          }
          if (((c << 4) & 0xFFFF) != 0) {
            return retSetBit + 11;
          }
          if (((c << 3) & 0xFFFF) != 0) {
            return retSetBit + 12;
          }
          if (((c << 2) & 0xFFFF) != 0) {
            return retSetBit + 13;
          }
          if (((c << 1) & 0xFFFF) != 0) {
            return retSetBit + 14;
          }
          return retSetBit + 15;
        }
      }
      return 0;
    }

    /**
     * Returns the greatest common divisor of two integers.
     * @param bigintSecond A BigInteger object. (2).
     * @return A BigInteger object.
     */
    public BigInteger gcd(BigInteger bigintSecond) {
      if (bigintSecond == null) {
        throw new NullPointerException("bigintSecond");
      }
      if (this.signum()==0) {
        return (bigintSecond).abs();
      }
      if (bigintSecond.signum()==0) {
        return (this).abs();
      }
      BigInteger thisValue = this.abs();
      bigintSecond = bigintSecond.abs();
      if (bigintSecond.equals(BigInteger.ONE) ||
          thisValue.equals(bigintSecond))
        return bigintSecond;
      if (thisValue.equals(BigInteger.ONE)) {
        return thisValue;
      }
      int expOfTwo = Math.min(
        this.getLowestSetBit(),
        bigintSecond.getLowestSetBit());
      if (thisValue.wordCount <= 10 && bigintSecond.wordCount <= 10) {
        while (true) {
          BigInteger bigintA = (thisValue.subtract(bigintSecond)).abs();
          if (bigintA.signum()==0) {
            if (expOfTwo != 0) {
              thisValue=thisValue.shiftLeft(expOfTwo);
            }
            return thisValue;
          }
          int setbit = bigintA.getLowestSetBit();
          bigintA=bigintA.shiftRight(setbit);
          bigintSecond = (thisValue.compareTo(bigintSecond) < 0) ? thisValue : bigintSecond;
          thisValue = bigintA;
        }
      } else {
        BigInteger temp;
        while (thisValue.signum()!=0) {
          if (thisValue.compareTo(bigintSecond) < 0) {
            temp = thisValue;
            thisValue = bigintSecond;
            bigintSecond = temp;
          }
          thisValue = thisValue.remainder(bigintSecond);
        }
        return bigintSecond;
      }
    }

    /**
     * Calculates the remainder when a BigInteger raised to a certain power
     * is divided by another BigInteger.
     * @param pow A BigInteger object. (2).
     * @param mod A BigInteger object. (3).
     * @return A BigInteger object.
     */
    public BigInteger ModPow(BigInteger pow, BigInteger mod) {
      if (pow == null) {
        throw new NullPointerException("pow");
      }
      if (pow.signum() < 0) {
        throw new IllegalArgumentException("pow is negative");
      }
      BigInteger r = BigInteger.ONE;
      BigInteger v = this;
      while (pow.signum()!=0) {
        if (pow.testBit(0)) {
          r = (r.multiply(v)).remainder(mod);
        }
        pow=pow.shiftRight(1);
        if (pow.signum()!=0) {
          v = (v.multiply(v)).remainder(mod);
        }
      }
      return r;
    }

    static void PositiveSubtract(
      BigInteger diff,
      BigInteger minuend,
      BigInteger subtrahend) {
      int words1Size = minuend.wordCount;
      words1Size += words1Size & 1;
      int words2Size = subtrahend.wordCount;
      words2Size += words2Size & 1;
      if (words1Size == words2Size) {
        if (Compare(minuend.reg, 0, subtrahend.reg, 0, (int)words1Size) >= 0) {
          // words1 is at least as high as words2
          Subtract(diff.reg, 0, minuend.reg, 0, subtrahend.reg, 0, (int)words1Size);
          diff.negative = false;  // difference will not be negative at this point
        } else {
          // words1 is less than words2
          Subtract(diff.reg, 0, subtrahend.reg, 0, minuend.reg, 0, (int)words1Size);
          diff.negative = true;  // difference will be negative
        }
      } else if (words1Size > words2Size) {
        // words1 is greater than words2
        short borrow = (short)Subtract(diff.reg, 0, minuend.reg, 0, subtrahend.reg, 0, (int)words2Size);
        System.arraycopy(minuend.reg, words2Size, diff.reg, words2Size, words1Size - words2Size);
        borrow = (short)Decrement(diff.reg, words2Size, (int)(words1Size - words2Size), borrow);
        // Debugif(!(borrow==0))Assert.fail("{0} line {1}: !borrow","integer.cpp",3524);
        diff.negative = false;
      } else {
        // words1 is less than words2
        short borrow = (short)Subtract(diff.reg, 0, subtrahend.reg, 0, minuend.reg, 0, (int)words1Size);
        System.arraycopy(subtrahend.reg, words1Size, diff.reg, words1Size, words2Size - words1Size);
        borrow = (short)Decrement(diff.reg, words1Size, (int)(words2Size - words1Size), borrow);
        // Debugif(!(borrow==0))Assert.fail("{0} line {1}: !borrow","integer.cpp",3532);
        diff.negative = true;
      }
      diff.wordCount = diff.CalcWordCount();
      diff.ShortenArray();
      if (diff.wordCount == 0) {
        diff.negative = false;
      }
    }

    /**
     * Determines whether this object and another object are equal.
     * @param obj An arbitrary object.
     * @return True if the objects are equal; false otherwise.
     */
    @Override public boolean equals(Object obj) {
      BigInteger other = ((obj instanceof BigInteger) ? (BigInteger)obj : null);
      if (other == null) {
        return false;
      }
      if (this.wordCount == other.wordCount) {
        if (this.negative != other.negative) {
          return false;
        }
        for (int i = 0; i < this.wordCount; ++i) {
          if (this.reg[i] != other.reg[i]) {
            return false;
          }
        }
        return true;
      }
      return false;
    }

    /**
     * Returns the hash code for this instance.
     * @return A 32-bit hash code.
     */
    @Override public int hashCode() {
      int hashCodeValue = 0;
      {
        hashCodeValue += 1000000007 * this.signum();
        if (this.reg != null) {
          for (int i = 0; i < this.wordCount; ++i) {
            hashCodeValue += 1000000013 * this.reg[i];
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
      if (bigintAugend == null) {
        throw new NullPointerException("bigintAugend");
      }
      BigInteger sum;
      if (this.wordCount == 0) {
        return bigintAugend;
      }
      if (bigintAugend.wordCount == 0) {
        return this;
      }
      if (bigintAugend.wordCount == 1 && this.wordCount == 1) {
        if (this.negative == bigintAugend.negative) {
          int intSum = (((int)this.reg[0]) & 0xFFFF) + (((int)bigintAugend.reg[0]) & 0xFFFF);
          sum = new BigInteger();
          sum.reg = new short[2];
          sum.reg[0] = ((short)intSum);
          sum.reg[1] = ((short)(intSum >> 16));
          sum.wordCount = ((intSum >> 16) == 0) ? 1 : 2;
          sum.negative = this.negative;
          return sum;
        } else {
          int a = ((int)this.reg[0]) & 0xFFFF;
          int b = ((int)bigintAugend.reg[0]) & 0xFFFF;
          if (a == b) {
            return BigInteger.ZERO;
          }
          if (a > b) {
            a -= b;
            sum = new BigInteger();
            sum.reg = new short[2];
            sum.reg[0] = ((short)a);
            sum.wordCount = 1;
            sum.negative = this.negative;
            return sum;
          } else {
            b -= a;
            sum = new BigInteger();
            sum.reg = new short[2];
            sum.reg[0] = ((short)b);
            sum.wordCount = 1;
            sum.negative = !this.negative;
            return sum;
          }
        }
      }
      sum = new BigInteger().Allocate((int)Math.max(this.reg.length, bigintAugend.reg.length));
      if ((!this.negative) == (!bigintAugend.negative)) {
        // both nonnegative or both negative
        int carry;
        int addendCount = this.wordCount + (this.wordCount & 1);
        int augendCount = bigintAugend.wordCount + (bigintAugend.wordCount & 1);
        int desiredLength = Math.max(addendCount, augendCount);
        if (addendCount == augendCount) {
          carry = Add(sum.reg, 0, this.reg, 0, bigintAugend.reg, 0, (int)addendCount);
        } else if (addendCount > augendCount) {
          // Addend is bigger
          carry = Add(
            sum.reg, 0,
            this.reg, 0,
            bigintAugend.reg, 0,
            (int)augendCount);
          System.arraycopy(
            this.reg,
            augendCount,
            sum.reg,
            augendCount,
            addendCount - augendCount);
          if (carry != 0)
            carry = Increment(
              sum.reg,
              augendCount,
              (int)(addendCount - augendCount),
              (short)carry);
        } else {
          // Augend is bigger
          carry = Add(
            sum.reg,
            0,
            this.reg,
            0,
            bigintAugend.reg,
            0,
            (int)addendCount);
          System.arraycopy(
            bigintAugend.reg,
            addendCount,
            sum.reg,
            addendCount,
            augendCount - addendCount);
          if (carry != 0)
            carry = Increment(
              sum.reg,
              addendCount,
              (int)(augendCount - addendCount),
              (short)carry);
        }
        boolean needShorten = true;
        if (carry != 0) {
          int nextIndex = desiredLength;
          int len = RoundupSize(nextIndex + 1);
          sum.reg = CleanGrow(sum.reg, len);
          sum.reg[nextIndex] = (short)carry;
          needShorten = false;
        }
        sum.negative = false;
        sum.wordCount = sum.CalcWordCount();
        if (needShorten) {
          sum.ShortenArray();
        }
        sum.negative = this.negative && sum.signum()!=0;
      } else if (this.negative) {
        PositiveSubtract(sum, bigintAugend, this);  // this is negative, b is nonnegative
      } else {
        PositiveSubtract(sum, this, bigintAugend);  // this is nonnegative, b is negative
      }
      return sum;
    }

    /**
     * Subtracts a BigInteger from this BigInteger.
     * @param subtrahend A BigInteger object.
     * @return The difference of the two objects.
     */
    public BigInteger subtract(BigInteger subtrahend) {
      if (subtrahend == null) {
        throw new NullPointerException("subtrahend");
      }
      if (this.wordCount == 0) {
        return subtrahend.negate();
      }
      if (subtrahend.wordCount == 0) {
        return this;
      }
      return this.add(subtrahend.negate());
    }

    private void ShortenArray() {
      if (this.reg.length > 32) {
        int newLength = RoundupSize(this.wordCount);
        if (newLength < this.reg.length &&
            (this.reg.length - newLength) >= 16) {
          // Reallocate the array if the rounded length
          // is much smaller than the current length
          short[] newreg = new short[newLength];
          System.arraycopy(this.reg,0,newreg,0,Math.min(newLength, this.reg.length));
          this.reg = newreg;
        }
      }
    }

    /**
     * Multiplies this instance by the value of a BigInteger object.
     * @param bigintMult A BigInteger object.
     * @return The product of the two objects.
     */
    public BigInteger multiply(BigInteger bigintMult) {
      if (bigintMult == null) {
        throw new NullPointerException("bigintMult");
      }
      if (this.wordCount == 0 || bigintMult.wordCount == 0) {
        return BigInteger.ZERO;
      }
      if (this.wordCount == 1 && this.reg[0] == 1) {
        return this.negative ? bigintMult.negate() : bigintMult;
      }
      if (bigintMult.wordCount == 1 && bigintMult.reg[0] == 1) {
        return bigintMult.negative ? this.negate() : this;
      }
      BigInteger product = new BigInteger();
      boolean needShorten = true;
      if (this.wordCount == 1) {
        int wc = bigintMult.wordCount;
        int regLength = wc == bigintMult.reg.length ? RoundupSize(wc + 1) : bigintMult.reg.length;
        product.reg = new short[regLength];
        product.reg[wc] = LinearMultiply(product.reg, 0, bigintMult.reg, 0, this.reg[0], wc);
        product.negative = false;
        product.wordCount = product.reg.length;
        needShorten = false;
      } else if (bigintMult.wordCount == 1) {
        int wc = this.wordCount;
        int regLength = wc == this.reg.length ? RoundupSize(wc + 1) : this.reg.length;
        product.reg = new short[regLength];
        product.reg[wc] = LinearMultiply(product.reg, 0, this.reg, 0, bigintMult.reg[0], wc);
        product.negative = false;
        product.wordCount = product.reg.length;
        needShorten = false;
      } else if (this.wordCount <= 10 && bigintMult.wordCount <= 10) {
        int wc = this.wordCount + bigintMult.wordCount;
        wc = (wc <= 16) ? RoundupSizeTable[wc] : RoundupSize(wc);
        product.reg = new short[wc];
        product.negative = false;
        product.wordCount = product.reg.length;
        SchoolbookMultiply(
          product.reg, 0,
          this.reg, 0, this.wordCount,
          bigintMult.reg, 0, bigintMult.wordCount);
        needShorten = false;
      } else if (this.equals(bigintMult)) {
        int words1Size = RoundupSize(this.wordCount);
        product.reg = new short[RoundupSize(words1Size + words1Size)];
        product.wordCount = product.reg.length;
        product.negative = false;
        short[] workspace = new short[words1Size + words1Size];
        RecursiveSquare(
          product.reg, 0,
          workspace, 0,
          this.reg, 0, words1Size);
      } else {
        int words1Size = this.wordCount;
        int words2Size = bigintMult.wordCount;
        words1Size = (words1Size <= 16) ? RoundupSizeTable[words1Size] : RoundupSize(words1Size);
        words2Size = (words2Size <= 16) ? RoundupSizeTable[words2Size] : RoundupSize(words2Size);
        product.reg = new short[RoundupSize(words1Size + words2Size)];
        product.negative = false;
        short[] workspace = new short[words1Size + words2Size];
        product.wordCount = product.reg.length;
        AsymmetricMultiply(
          product.reg, 0,
          workspace, 0,
          this.reg, 0, words1Size,
          bigintMult.reg, 0, words2Size);
      }
      // Recalculate word count
      while (product.wordCount != 0 && product.reg[product.wordCount - 1] == 0) {
        product.wordCount--;
      }
      if (needShorten) {
        product.ShortenArray();
      }
      if (this.negative != bigintMult.negative) {
        product.NegateInternal();
      }
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
          MakeUint(dividendReg[i], remainder),
          divisorSmall);
      }
      return remainder;
    }

    private static void FastDivide(short[] quotientReg, short[] dividendReg, int count, short divisorSmall) {
      int i = count;
      short remainderShort = 0;
      int idivisor = ((int)divisorSmall) & 0xFFFF;
      int quo, rem;
      while ((i--) > 0) {
        int currentDividend = ((int)((((int)dividendReg[i]) & 0xFFFF) |
                                              ((int)remainderShort << 16)));
        if ((currentDividend >> 31) == 0) {
          quo = currentDividend / idivisor;
          quotientReg[i] = ((short)quo);
          if (i > 0) {
            rem = currentDividend - (idivisor * quo);
            remainderShort = ((short)rem);
          }
        } else {
          quotientReg[i] = DivideUnsigned(currentDividend, divisorSmall);
          if (i > 0) {
            remainderShort = RemainderUnsigned(currentDividend, divisorSmall);
          }
        }
      }
    }

    private static short FastDivideAndRemainderEx(
      short[] quotientReg,
      short[] dividendReg,
      int count,
      short divisorSmall) {
      int i = count;
      short remainderShort = 0;
      int idivisor = ((int)divisorSmall) & 0xFFFF;
      int quo, rem;
      while ((i--) > 0) {
        int currentDividend = ((int)((((int)dividendReg[i]) & 0xFFFF) |
                                              ((int)remainderShort << 16)));
        if ((currentDividend >> 31) == 0) {
          quo = currentDividend / idivisor;
          quotientReg[i] = ((short)quo);
          rem = currentDividend - (idivisor * quo);
          remainderShort = ((short)rem);
        } else {
          quotientReg[i] = DivideUnsigned(currentDividend, divisorSmall);
          remainderShort = RemainderUnsigned(currentDividend, divisorSmall);
        }
      }
      return remainderShort;
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
      if (bigintDivisor == null) {
        throw new NullPointerException("bigintDivisor");
      }
      int words1Size = this.wordCount;
      int words2Size = bigintDivisor.wordCount;
      if (words2Size == 0) {
        throw new ArithmeticException();
      }
      if (words1Size < words2Size) {
        // dividend is less than divisor (includes case
        // where dividend is 0)
        return BigInteger.ZERO;
      }
      if (words1Size <= 2 && words2Size <= 2 && this.canFitInInt() && bigintDivisor.canFitInInt()) {
        int aSmall = this.intValue();
        int bSmall = bigintDivisor.intValue();
        if (aSmall != Integer.MIN_VALUE || bSmall != -1) {
          int result = aSmall / bSmall;
          return new BigInteger().InitializeInt(result);
        }
      }
      BigInteger quotient;
      if (words2Size == 1) {
        // divisor is small, use a fast path
        quotient = new BigInteger();
        quotient.reg = new short[this.reg.length];
        quotient.wordCount = this.wordCount;
        quotient.negative = this.negative;
        FastDivide(quotient.reg, this.reg, words1Size, bigintDivisor.reg[0]);
        while (quotient.wordCount != 0 &&
               quotient.reg[quotient.wordCount - 1] == 0) {
          quotient.wordCount--;
        }
        if (quotient.wordCount != 0) {
          quotient.negative = this.negative ^ bigintDivisor.negative;
          return quotient;
        } else {
          return BigInteger.ZERO;
        }
      }
      quotient = new BigInteger();
      words1Size += words1Size % 2;
      words2Size += words2Size % 2;
      quotient.reg = new short[RoundupSize((int)(words1Size - words2Size + 2))];
      quotient.negative = false;
      short[] tempbuf = new short[words1Size + 3 * (words2Size + 2)];
      Divide(
        null, 0,
        quotient.reg, 0,
        tempbuf, 0,
        this.reg, 0, words1Size,
        bigintDivisor.reg, 0, words2Size);
      quotient.wordCount = quotient.CalcWordCount();
      quotient.ShortenArray();
      if ((this.signum() < 0) ^ (bigintDivisor.signum() < 0)) {
        quotient.NegateInternal();
      }
      return quotient;
    }

    /**
     * Not documented yet.
     * @param divisor A BigInteger object.
     * @return A BigInteger[] object.
     */
    public BigInteger[] divideAndRemainder(BigInteger divisor) {
      if (divisor == null) {
        throw new NullPointerException("divisor");
      }
      BigInteger quotient;
      int words1Size = this.wordCount;
      int words2Size = divisor.wordCount;
      if (words2Size == 0) {
        throw new ArithmeticException();
      }

      if (words1Size < words2Size) {
        // dividend is less than divisor (includes case
        // where dividend is 0)
        return new BigInteger[] { BigInteger.ZERO, this };
      }
      if (words2Size == 1) {
        // divisor is small, use a fast path
        quotient = new BigInteger();
        quotient.reg = new short[this.reg.length];
        quotient.wordCount = this.wordCount;
        quotient.negative = this.negative;
        int smallRemainder = (((int)FastDivideAndRemainderEx(
          quotient.reg,
          this.reg,
          words1Size,
          divisor.reg[0])) & 0xFFFF);
        while (quotient.wordCount != 0 &&
               quotient.reg[quotient.wordCount - 1] == 0) {
          quotient.wordCount--;
        }
        quotient.ShortenArray();
        if (quotient.wordCount != 0) {
          quotient.negative = this.negative ^ divisor.negative;
        } else {
          quotient = BigInteger.ZERO;
        }
        if (this.negative) {
          smallRemainder = -smallRemainder;
        }
        return new BigInteger[] { quotient, new BigInteger().InitializeInt(smallRemainder) };
      }
      if (this.wordCount == 2 && divisor.wordCount == 2 &&
          (this.reg[1] >> 15) != 0 &&
          (divisor.reg[1] >> 15) != 0) {
        int a = ((int)this.reg[0]) & 0xFFFF;
        int b = ((int)divisor.reg[0]) & 0xFFFF;
        {
          a |= (((int)this.reg[1]) & 0xFFFF) << 16;
          b |= (((int)divisor.reg[1]) & 0xFFFF) << 16;
          int quo = a / b;
          if (this.negative) {
            quo = -quo;
          }
          int rem = a - (b * quo);
          return new BigInteger[] {
            new BigInteger().InitializeInt(quo),
            new BigInteger().InitializeInt(rem)
          };
        }
      }
      BigInteger remainder = new BigInteger();
      quotient = new BigInteger();
      words1Size += words1Size & 1;
      words2Size += words2Size & 1;
      remainder.reg = new short[RoundupSize((int)words2Size)];
      remainder.negative = false;
      quotient.reg = new short[RoundupSize((int)(words1Size - words2Size + 2))];
      quotient.negative = false;
      short[] tempbuf = new short[words1Size + 3 * (words2Size + 2)];
      Divide(
        remainder.reg, 0,
        quotient.reg, 0,
        tempbuf, 0,
        this.reg, 0, words1Size,
        divisor.reg, 0, words2Size);
      remainder.wordCount = remainder.CalcWordCount();
      quotient.wordCount = quotient.CalcWordCount();
      // System.out.println("Divd={0} divs={1} quo={2} rem={3}",this.wordCount,
      //                divisor.wordCount, quotient.wordCount, remainder.wordCount);
      remainder.ShortenArray();
      quotient.ShortenArray();
      if (this.signum() < 0) {
        quotient.NegateInternal();
        if (remainder.signum()!=0) {
          remainder.NegateInternal();
        }
      }
      if (divisor.signum() < 0) {
        quotient.NegateInternal();
      }
      return new BigInteger[] { quotient, remainder };
    }

    /**
     * Finds the modulus remainder that results when this instance is divided
     * by the value of a BigInteger object. The modulus remainder is the same
     * as the normal remainder if the normal remainder is positive, and equals
     * divisor minus normal remainder if the normal remainder is negative.
     * @param divisor A divisor greater than 0.
     * @return A BigInteger object.
     */
    public BigInteger mod(BigInteger divisor) {
      if (divisor == null) {
        throw new NullPointerException("divisor");
      }
      if (divisor.signum() < 0) {
        throw new ArithmeticException("Divisor is negative");
      }
      BigInteger rem = this.remainder(divisor);
      if (rem.signum() < 0) {
        rem = divisor.subtract(rem);
      }
      return rem;
    }

    /**
     * Finds the remainder that results when this instance is divided by
     * the value of a BigInteger object. The remainder is the value that remains
     * when the absolute value of this object is divided by the absolute value
     * of the other object; the remainder has the same sign (positive or negative)
     * as this object.
     * @param divisor A BigInteger object.
     * @return The remainder of the two objects.
     */
    public BigInteger remainder(BigInteger divisor) {
      int words1Size = this.wordCount;
      int words2Size = divisor.wordCount;
      if (words2Size == 0) {
        throw new ArithmeticException();
      }
      if (words1Size < words2Size) {
        // dividend is less than divisor
        return this;
      }
      if (words2Size == 1) {
        short shortRemainder = FastRemainder(this.reg, this.wordCount, divisor.reg[0]);
        int smallRemainder = ((int)shortRemainder) & 0xFFFF;
        if (this.negative) {
          smallRemainder = -smallRemainder;
        }
        return new BigInteger().InitializeInt(smallRemainder);
      }
      if (this.PositiveCompare(divisor) < 0) {
        if (divisor.signum()==0) {
          throw new ArithmeticException();
        }
        return this;
      }
      BigInteger remainder = new BigInteger();
      words1Size += words1Size % 2;
      words2Size += words2Size % 2;
      remainder.reg = new short[RoundupSize((int)words2Size)];
      remainder.negative = false;
      short[] tempbuf = new short[words1Size + 3 * (words2Size + 2)];
      Divide(
        remainder.reg, 0,
        null, 0,
        tempbuf, 0,
        this.reg, 0, words1Size,
        divisor.reg, 0, words2Size);
      remainder.wordCount = remainder.CalcWordCount();
      remainder.ShortenArray();
      if (this.signum() < 0 && remainder.signum()!=0) {
        remainder.NegateInternal();
      }
      return remainder;
    }

    void NegateInternal() {
      if (this.wordCount != 0) {
        this.negative = this.signum() > 0;
      }
    }

    int PositiveCompare(BigInteger t) {
      int size = this.wordCount, tempSize = t.wordCount;
      if (size == tempSize) {
        return Compare(this.reg, 0, t.reg, 0, (int)size);
      } else {
        return size > tempSize ? 1 : -1;
      }
    }

    /**
     * Compares a BigInteger object with this instance.
     * @param other A BigInteger object.
     * @return Zero if the values are equal; a negative number if this instance
     * is less, or a positive number if this instance is greater.
     */
    public int compareTo(BigInteger other) {
      if (other == null) {
        return 1;
      }
      if (this == other) {
        return 0;
      }
      int size = this.wordCount, tempSize = other.wordCount;
      int sa = size == 0 ? 0 : (this.negative ? -1 : 1);
      int sb = tempSize == 0 ? 0 : (other.negative ? -1 : 1);
      if (sa != sb) {
        return (sa < sb) ? -1 : 1;
      }
      if (sa == 0) {
        return 0;
      }
      if (size == tempSize) {
        if (size == 1 && this.reg[0] == other.reg[0]) {
          return 0;
        } else {
          short[] words1 = this.reg;
          short[] words2 = other.reg;
          while ((size--) != 0) {
            int an = ((int)words1[size]) & 0xFFFF;
            int bn = ((int)words2[size]) & 0xFFFF;
            if (an > bn) {
              return (sa > 0) ? 1 : -1;
            } else if (an < bn) {
              return (sa > 0) ? -1 : 1;
            }
          }
          return 0;
        }
      } else {
        return ((size > tempSize) ^ (sa <= 0)) ? 1 : -1;
      }
    }

    /**
     * Not documented yet.
     */
    public int signum() {
        if (this.wordCount == 0) {
          return 0;
        }
        return this.negative ? -1 : 1;
      }

    /**
     * Not documented yet.
     */
    public boolean isZero() {
        return this.wordCount == 0;
      }

    /**
     * Finds the square root of this instance&apos;s value, rounded down.
     * @return The square root of this object&apos;s value. Returns 0 if
     * this value is 0 or less.
     */
    public BigInteger sqrt() {
      if (this.signum() <= 0) {
        return BigInteger.ZERO;
      }
      BigInteger bigintX = null;
      BigInteger bigintY = Power2((this.getUnsignedBitLength() + 1) / 2);
      do {
        bigintX = bigintY;
        bigintY = this.divide(bigintX);
        bigintY=bigintY.add(bigintX);
        bigintY=bigintY.shiftRight(1);
      } while (bigintY.compareTo(bigintX) < 0);
      return bigintX;
    }

    public BigInteger[] sqrtWithRemainder() {
      if (this.signum() <= 0) {
        return new BigInteger[]{ BigInteger.ZERO, BigInteger.ZERO };
      }
      BigInteger bigintX = null;
      BigInteger bigintY = Power2((this.getUnsignedBitLength() + 1) / 2);
      do {
        bigintX = bigintY;
        bigintY = this.divide(bigintX);
        bigintY=bigintY.add(bigintX);
        bigintY=bigintY.shiftRight(1);
      } while (bigintY.compareTo(bigintX) < 0);
      bigintY = bigintX.multiply(bigintX);
      bigintY = this.subtract(bigintY);
      return new BigInteger[]{
        bigintX, bigintY
      };
    }

    /**
     * Gets a value indicating whether this value is even.
     */
    public boolean isEven() {
        return !this.GetUnsignedBit(0);
      }

    /**
     * BigInteger object for the number zero.
     */

    public static final BigInteger ZERO = new BigInteger().InitializeInt(0);
    /**
     * BigInteger object for the number one.
     */

    public static final BigInteger ONE = new BigInteger().InitializeInt(1);
    /**
     * BigInteger object for the number ten.
     */

    public static final BigInteger TEN = new BigInteger().InitializeInt(10);
  }

