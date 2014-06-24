package com.upokecenter.util;
/*
Written in 2013 by Peter O.

Parts of the code were adapted by Peter O. from
the public-domain code from the library
CryptoPP by Wei Dai.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * An arbitrary-precision integer. <p>Instances of this class are
     * immutable, so they are inherently safe for use by multiple threads.
     * Multiple instances of this object with the same value are interchangeable,
     * so they should not be compared using the "==" operator (which only
     * checks if each side of the operator is the same instance).</p>
     */
  public final class BigInteger implements Comparable<BigInteger>
  {
    private static int CountWords(short[] array, int n) {
      while (n != 0 && array[n - 1] == 0) {
        --n;
      }
      return (int)n;
    }

    private static short ShiftWordsLeftByBits(short[] r, int rstart, int n, int shiftBits) {

      {
        short u, carry = 0;
        if (shiftBits != 0) {
          for (int i = 0; i < n; ++i) {
            u = r[rstart + i];
            r[rstart + i] = (short)((int)(u << (int)shiftBits) | (((int)carry) & 0xffff));
            carry = (short)((((int)u) & 0xffff) >> (int)(16 - shiftBits));
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
            r[rstart + i - 1] = (short)((((((int)u) & 0xffff) >> (int)shiftBits) & 0xffff) | (((int)carry) & 0xffff));
            carry = (short)((((int)u) & 0xffff) << (int)(16 - shiftBits));
          }
        }
        return carry;
      }
    }

    private static short ShiftWordsRightByBitsSignExtend(short[] r, int rstart, int n, int shiftBits) {
      // Debugif(!(shiftBits<16))Assert.fail("{0} line {1}: shiftBits<16","words.h",67);
      {
        short u, carry = (short)((int)0xffff << (int)(16 - shiftBits));
        if (shiftBits != 0) {
          for (int i = n; i > 0; --i) {
            u = r[rstart + i - 1];
            r[rstart + i - 1] = (short)(((((int)u) & 0xffff) >> (int)shiftBits) | (((int)carry) & 0xffff));
            carry = (short)((((int)u) & 0xffff) << (int)(16 - shiftBits));
          }
        }
        return carry;
      }
    }

    private static void ShiftWordsLeftByWords(short[] r, int rstart, int n, int shiftWords) {
      shiftWords = Math.min(shiftWords, n);
      if (shiftWords != 0) {
        for (int i = n - 1; i >= shiftWords; --i) {
          r[rstart + i] = r[rstart + i - shiftWords];
        }
        java.util.Arrays.fill(r,rstart,(rstart)+(shiftWords),(short)0);
      }
    }

    private static void ShiftWordsRightByWordsSignExtend(short[] r, int rstart, int n, int shiftWords) {
      shiftWords = Math.min(shiftWords, n);
      if (shiftWords != 0) {
        for (int i = 0; i + shiftWords < n; ++i) {
          r[rstart + i] = r[rstart + i + shiftWords];
        }
        rstart += n - shiftWords;
        // Sign extend
        for (int i = 0; i < shiftWords; ++i) {
          r[rstart + i] = ((short)0xffff);
        }
      }
    }

    private static int Compare(short[] words1, int astart, short[] words2, int bstart, int n) {
      while ((n--) != 0) {
        int an = ((int)words1[astart + n]) & 0xffff;
        int bn = ((int)words2[bstart + n]) & 0xffff;
        if (an > bn) {
          return 1;
        }
        if (an < bn) {
          return -1;
        }
      }
      return 0;
    }

    /*
    private static int CompareUnevenSize(
      short[] words1,
      int astart,
      int acount,
      short[] words2,
      int bstart,
      int bcount) {
      int n = acount;
      if (acount > bcount) {
        while ((acount--) != bcount) {
          if (words1[astart + acount] != 0) {
            return 1;
          }
        }
        n = bcount;
      } else if (bcount > acount) {
        while ((bcount--) != acount) {
          if (words1[astart + acount] != 0) {
            return -1;
          }
        }
        n = acount;
      }
      while ((n--) != 0) {
        int an = ((int)words1[astart + n]) & 0xffff;
        int bn = ((int)words2[bstart + n]) & 0xffff;
        if (an > bn) {
          return 1;
        } else if (an < bn) {
          return -1;
        }
      }
      return 0;
    }
     */

    private static int CompareWithOneBiggerWords1(short[] words1, int astart, short[] words2, int bstart, int words1Count) {
      // NOTE: Assumes that words2's count is 1 less
      if (words1[astart + words1Count - 1] != 0) {
        return 1;
      }
      --words1Count;
      while ((words1Count--) != 0) {
        int an = ((int)words1[astart + words1Count]) & 0xffff;
        int bn = ((int)words2[bstart + words1Count]) & 0xffff;
        if (an > bn) {
          return 1;
        }
        if (an < bn) {
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
        if ((((int)words1[words1Start]) & 0xffff) >= (((int)tmp) & 0xffff)) {
          return 0;
        }
        for (int i = 1; i < n; ++i) {
          ++words1[words1Start + i];
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
        if ((((int)words1[words1Start]) & 0xffff) <= (((int)tmp) & 0xffff)) {
          return 0;
        }
        for (int i = 1; i < n; ++i) {
          tmp = words1[words1Start + i];
          --words1[words1Start + i];
          if (tmp != 0) {
            return 0;
          }
        }
        return 1;
      }
    }

    private static void TwosComplement(short[] words1, int words1Start, int n) {
      Decrement(words1, words1Start, n, (short)1);
      for (int i = 0; i < n; ++i) {
        words1[words1Start + i] = ((short)(~words1[words1Start + i]));
      }
    }

    private static int Add(
      short[] c,
      int cstart,
      short[] words1,
      int astart,
      short[] words2,
      int bstart,
      int n) {
      // Debugif(!(n%2 == 0))Assert.fail("{0} line {1}: n%2 == 0","integer.cpp",799);
      {
        int u;
        u = 0;
        for (int i = 0; i < n; i += 2) {
          u = (((int)words1[astart + i]) & 0xffff) + (((int)words2[bstart + i]) & 0xffff) + (short)(u >> 16);
          c[cstart + i] = (short)u;
          u = (((int)words1[astart + i + 1]) & 0xffff) + (((int)words2[bstart + i + 1]) & 0xffff) + (short)(u >> 16);
          c[cstart + i + 1] = (short)u;
        }
        return ((int)u >> 16) & 0xffff;
      }
    }

    private static int AddOneByOne(
      short[] c,
      int cstart,
      short[] words1,
      int astart,
      short[] words2,
      int bstart,
      int n) {
      // Debugif(!(n%2 == 0))Assert.fail("{0} line {1}: n%2 == 0","integer.cpp",799);
      {
        int u;
        u = 0;
        for (int i = 0; i < n; i += 1) {
          u = (((int)words1[astart + i]) & 0xffff) + (((int)words2[bstart + i]) & 0xffff) + (short)(u >> 16);
          c[cstart + i] = (short)u;
        }
        return ((int)u >> 16) & 0xffff;
      }
    }

    private static int SubtractOneBiggerWords1(
      short[] c,
      int cstart,
      short[] words1,
      int astart,
      short[] words2,
      int bstart,
      int words1Count) {
      // Assumes that words2's count is 1 less
      {
        int u;
        u = 0;
        int cm1 = words1Count - 1;
        for (int i = 0; i < cm1; i += 1) {
          u = (((int)words1[astart]) & 0xffff) - (((int)words2[bstart]) & 0xffff) - (int)((u >> 31) & 1);
          c[cstart++] = (short)u;
          ++astart;
          ++bstart;
        }
        u = (((int)words1[astart]) & 0xffff) - (int)((u >> 31) & 1);
        c[cstart++] = (short)u;
        return (int)((u >> 31) & 1);
      }
    }

    private static int SubtractOneBiggerWords2(
      short[] c,
      int cstart,
      short[] words1,
      int astart,
      short[] words2,
      int bstart,
      int words2Count) {
      // Assumes that words1's count is 1 less
      {
        int u;
        u = 0;
        int cm1 = words2Count - 1;
        for (int i = 0; i < cm1; i += 1) {
          u = (((int)words1[astart]) & 0xffff) - (((int)words2[bstart]) & 0xffff) - (int)((u >> 31) & 1);
          c[cstart++] = (short)u;
          ++astart;
          ++bstart;
        }
        u = 0 - (((int)words2[bstart]) & 0xffff) - (int)((u >> 31) & 1);
        c[cstart++] = (short)u;
        return (int)((u >> 31) & 1);
      }
    }

    private static int AddUnevenSize(
      short[] c,
      int cstart,
      short[] wordsBigger,
      int astart,
      int acount,
      short[] wordsSmaller,
      int bstart,
      int bcount) {

      {
        int u;
        u = 0;
        for (int i = 0; i < bcount; i += 1) {
          u = (((int)wordsBigger[astart + i]) & 0xffff) + (((int)wordsSmaller[bstart + i]) & 0xffff) + (short)(u >> 16);
          c[cstart + i] = (short)u;
        }
        for (int i = bcount; i < acount; i += 1) {
          u = (((int)wordsBigger[astart + i]) & 0xffff) + (short)(u >> 16);
          c[cstart + i] = (short)u;
        }
        return ((int)u >> 16) & 0xffff;
      }
    }

    private static int Subtract(
      short[] c,
      int cstart,
      short[] words1,
      int astart,
      short[] words2,
      int bstart,
      int n) {
      // Debugif(!(n%2 == 0))Assert.fail("{0} line {1}: n%2 == 0","integer.cpp",799);
      {
        int u;
        u = 0;
        for (int i = 0; i < n; i += 2) {
          u = (((int)words1[astart]) & 0xffff) - (((int)words2[bstart]) & 0xffff) - (int)((u >> 31) & 1);
          c[cstart++] = (short)u;
          ++astart;
          ++bstart;
          u = (((int)words1[astart]) & 0xffff) - (((int)words2[bstart]) & 0xffff) - (int)((u >> 31) & 1);
          c[cstart++] = (short)u;
          ++astart;
          ++bstart;
        }
        return (int)((u >> 31) & 1);
      }
    }

    private static int SubtractOneByOne(
      short[] c,
      int cstart,
      short[] words1,
      int astart,
      short[] words2,
      int bstart,
      int n) {
      // Debugif(!(n%2 == 0))Assert.fail("{0} line {1}: n%2 == 0","integer.cpp",799);
      {
        int u;
        u = 0;
        for (int i = 0; i < n; i += 1) {
          u = (((int)words1[astart]) & 0xffff) - (((int)words2[bstart]) & 0xffff) - (int)((u >> 31) & 1);
          c[cstart++] = (short)u;
          ++astart;
          ++bstart;
        }
        return (int)((u >> 31) & 1);
      }
    }

    private static short LinearMultiplyAdd(
      short[] productArr,
      int cstart,
      short[] words1,
      int astart,
      short words2,
      int n) {
      {
        short carry = 0;
        int bint = ((int)words2) & 0xffff;
        for (int i = 0; i < n; ++i) {
          int p;
          p = (((int)words1[astart + i]) & 0xffff) * bint;
          p += ((int)carry) & 0xffff;
          p += ((int)productArr[cstart + i]) & 0xffff;
          productArr[cstart + i] = (short)p;
          carry = (short)(p >> 16);
        }
        return carry;
      }
    }

    private static short LinearMultiply(
      short[] productArr,
      int cstart,
      short[] words1,
      int astart,
      short words2,
      int n) {
      {
        short carry = 0;
        int bint = ((int)words2) & 0xffff;
        for (int i = 0; i < n; ++i) {
          int p;
          p = (((int)words1[astart + i]) & 0xffff) * bint;
          p += ((int)carry) & 0xffff;
          productArr[cstart + i] = (short)p;
          carry = (short)(p >> 16);
        }
        return carry;
      }
    }
    //-----------------------------
    // Baseline Square
    //-----------------------------

    private static void Baseline_Square2(short[] result, int rstart, short[] words1, int astart) {
      {
        int p; short c; int d; int e;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart]) & 0xffff); result[rstart] = (short)p; e = ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 1]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 1] = c;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart + 1]) & 0xffff);
        p += e; result[rstart + 2] = (short)p; result[rstart + 3] = (short)(p >> 16);
      }
    }

    private static void Baseline_Square4(short[] result, int rstart, short[] words1, int astart) {
      {
        int p; short c; int d; int e;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart]) & 0xffff); result[rstart] = (short)p; e = ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 1]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 1] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 2]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart + 1]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 2] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 3]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart + 2]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 3] = c;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart + 3]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart + 2]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 4] = c;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart + 3]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + (2 * 4) - 3] = c;
        p = (((int)words1[astart + 3]) & 0xffff) * (((int)words1[astart + 3]) & 0xffff);
        p += e; result[rstart + 6] = (short)p; result[rstart + 7] = (short)(p >> 16);
      }
    }

    private static void Baseline_Square8(short[] result, int rstart, short[] words1, int astart) {
      {
        int p; short c; int d; int e;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart]) & 0xffff); result[rstart] = (short)p; e = ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 1]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 1] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 2]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart + 1]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 2] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 3]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart + 2]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 3] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 4]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart + 3]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart + 2]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 4] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 5]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart + 4]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart + 3]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 5] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 6]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart + 5]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart + 4]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 3]) & 0xffff) * (((int)words1[astart + 3]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 6] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 7]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart + 6]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart + 5]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 3]) & 0xffff) * (((int)words1[astart + 4]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 7] = c;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart + 7]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart + 6]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 3]) & 0xffff) * (((int)words1[astart + 5]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 4]) & 0xffff) * (((int)words1[astart + 4]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 8] = c;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart + 7]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 3]) & 0xffff) * (((int)words1[astart + 6]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 4]) & 0xffff) * (((int)words1[astart + 5]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 9] = c;
        p = (((int)words1[astart + 3]) & 0xffff) * (((int)words1[astart + 7]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 4]) & 0xffff) * (((int)words1[astart + 6]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 5]) & 0xffff) * (((int)words1[astart + 5]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 10] = c;
        p = (((int)words1[astart + 4]) & 0xffff) * (((int)words1[astart + 7]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 5]) & 0xffff) * (((int)words1[astart + 6]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 11] = c;
        p = (((int)words1[astart + 5]) & 0xffff) * (((int)words1[astart + 7]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 6]) & 0xffff) * (((int)words1[astart + 6]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 12] = c;
        p = (((int)words1[astart + 6]) & 0xffff) * (((int)words1[astart + 7]) & 0xffff); c = (short)p; d = ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) & 0xffff); result[rstart + 13] = c;
        p = (((int)words1[astart + 7]) & 0xffff) * (((int)words1[astart + 7]) & 0xffff);
        p += e; result[rstart + 14] = (short)p; result[rstart + 15] = (short)(p >> 16);
      }
    }

    //---------------------
    // Baseline multiply
    //---------------------

    private static void Baseline_Multiply2(short[] result, int rstart, short[] words1, int astart, short[] words2, int bstart) {
      {
        int p; short c; int d;
        int a0 = ((int)words1[astart]) & 0xffff;
        int a1 = ((int)words1[astart + 1]) & 0xffff;
        int b0 = ((int)words2[bstart]) & 0xffff;
        int b1 = ((int)words2[bstart + 1]) & 0xffff;
        p = a0 * b0; c = (short)p; d = ((int)p >> 16) & 0xffff; result[rstart] = c; c = (short)d; d = ((int)d >> 16) & 0xffff;
        p = a0 * b1;
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = a1 * b0;
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; result[rstart + 1] = c;
        p = a1 * b1;
        p += d; result[rstart + 2] = (short)p; result[rstart + 3] = (short)(p >> 16);
      }
    }

    private static final int ShortMask = 0xffff;

    private static void Baseline_Multiply4(short[] result, int rstart, short[] words1, int astart, short[] words2, int bstart) {
      {
        int p; short c; int d;
        int a0 = ((int)words1[astart]) & ShortMask;
        int b0 = ((int)words2[bstart]) & ShortMask;
        p = a0 * b0; c = (short)p; d = ((int)p >> 16) & ShortMask; result[rstart] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = a0 * (((int)words2[bstart + 1]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 1]) & ShortMask) * b0;
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 1] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = a0 * (((int)words2[bstart + 2]) & ShortMask);

        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 1]) & ShortMask) * (((int)words2[bstart + 1]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 2]) & ShortMask) * b0;
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 2] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = a0 * (((int)words2[bstart + 3]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 1]) & ShortMask) * (((int)words2[bstart + 2]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;

        p = (((int)words1[astart + 2]) & ShortMask) * (((int)words2[bstart + 1]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 3]) & ShortMask) * b0;
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 3] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = (((int)words1[astart + 1]) & ShortMask) * (((int)words2[bstart + 3]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 2]) & ShortMask) * (((int)words2[bstart + 2]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 3]) & ShortMask) * (((int)words2[bstart + 1]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 4] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = (((int)words1[astart + 2]) & ShortMask) * (((int)words2[bstart + 3]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 3]) & ShortMask) * (((int)words2[bstart + 2]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 5] = c;
        p = (((int)words1[astart + 3]) & ShortMask) * (((int)words2[bstart + 3]) & ShortMask);
        p += d; result[rstart + 6] = (short)p; result[rstart + 7] = (short)(p >> 16);
      }
    }

    private static void Baseline_Multiply8(short[] result, int rstart, short[] words1, int astart, short[] words2, int bstart) {
      {
        int p; short c; int d;
        p = (((int)words1[astart]) & ShortMask) * (((int)words2[bstart]) & ShortMask); c = (short)p; d = ((int)p >> 16) & ShortMask; result[rstart] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = (((int)words1[astart]) & ShortMask) * (((int)words2[bstart + 1]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 1]) & ShortMask) * (((int)words2[bstart]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 1] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = (((int)words1[astart]) & ShortMask) * (((int)words2[bstart + 2]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 1]) & ShortMask) * (((int)words2[bstart + 1]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 2]) & ShortMask) * (((int)words2[bstart]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 2] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = (((int)words1[astart]) & ShortMask) * (((int)words2[bstart + 3]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 1]) & ShortMask) * (((int)words2[bstart + 2]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 2]) & ShortMask) * (((int)words2[bstart + 1]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 3]) & ShortMask) * (((int)words2[bstart]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 3] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = (((int)words1[astart]) & ShortMask) * (((int)words2[bstart + 4]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 1]) & ShortMask) * (((int)words2[bstart + 3]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 2]) & ShortMask) * (((int)words2[bstart + 2]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 3]) & ShortMask) * (((int)words2[bstart + 1]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 4]) & ShortMask) * (((int)words2[bstart]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 4] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = (((int)words1[astart]) & ShortMask) * (((int)words2[bstart + 5]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 1]) & ShortMask) * (((int)words2[bstart + 4]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 2]) & ShortMask) * (((int)words2[bstart + 3]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 3]) & ShortMask) * (((int)words2[bstart + 2]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 4]) & ShortMask) * (((int)words2[bstart + 1]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 5]) & ShortMask) * (((int)words2[bstart]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 5] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = (((int)words1[astart]) & ShortMask) * (((int)words2[bstart + 6]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 1]) & ShortMask) * (((int)words2[bstart + 5]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 2]) & ShortMask) * (((int)words2[bstart + 4]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 3]) & ShortMask) * (((int)words2[bstart + 3]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 4]) & ShortMask) * (((int)words2[bstart + 2]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 5]) & ShortMask) * (((int)words2[bstart + 1]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 6]) & ShortMask) * (((int)words2[bstart]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 6] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = (((int)words1[astart]) & ShortMask) * (((int)words2[bstart + 7]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 1]) & ShortMask) * (((int)words2[bstart + 6]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 2]) & ShortMask) * (((int)words2[bstart + 5]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 3]) & ShortMask) * (((int)words2[bstart + 4]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 4]) & ShortMask) * (((int)words2[bstart + 3]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 5]) & ShortMask) * (((int)words2[bstart + 2]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 6]) & ShortMask) * (((int)words2[bstart + 1]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 7]) & ShortMask) * (((int)words2[bstart]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 7] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = (((int)words1[astart + 1]) & ShortMask) * (((int)words2[bstart + 7]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 2]) & ShortMask) * (((int)words2[bstart + 6]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 3]) & ShortMask) * (((int)words2[bstart + 5]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 4]) & ShortMask) * (((int)words2[bstart + 4]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 5]) & ShortMask) * (((int)words2[bstart + 3]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 6]) & ShortMask) * (((int)words2[bstart + 2]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 7]) & ShortMask) * (((int)words2[bstart + 1]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 8] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = (((int)words1[astart + 2]) & ShortMask) * (((int)words2[bstart + 7]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 3]) & ShortMask) * (((int)words2[bstart + 6]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 4]) & ShortMask) * (((int)words2[bstart + 5]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 5]) & ShortMask) * (((int)words2[bstart + 4]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 6]) & ShortMask) * (((int)words2[bstart + 3]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 7]) & ShortMask) * (((int)words2[bstart + 2]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 9] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = (((int)words1[astart + 3]) & ShortMask) * (((int)words2[bstart + 7]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 4]) & ShortMask) * (((int)words2[bstart + 6]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 5]) & ShortMask) * (((int)words2[bstart + 5]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 6]) & ShortMask) * (((int)words2[bstart + 4]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 7]) & ShortMask) * (((int)words2[bstart + 3]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 10] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = (((int)words1[astart + 4]) & ShortMask) * (((int)words2[bstart + 7]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 5]) & ShortMask) * (((int)words2[bstart + 6]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 6]) & ShortMask) * (((int)words2[bstart + 5]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 7]) & ShortMask) * (((int)words2[bstart + 4]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 11] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = (((int)words1[astart + 5]) & ShortMask) * (((int)words2[bstart + 7]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 6]) & ShortMask) * (((int)words2[bstart + 6]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 7]) & ShortMask) * (((int)words2[bstart + 5]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 12] = c; c = (short)d; d = ((int)d >> 16) & ShortMask;
        p = (((int)words1[astart + 6]) & ShortMask) * (((int)words2[bstart + 7]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask;
        p = (((int)words1[astart + 7]) & ShortMask) * (((int)words2[bstart + 6]) & ShortMask);
        p += ((int)c) & ShortMask; c = (short)p;
        d += ((int)p >> 16) & ShortMask; result[rstart + 13] = c;
        p = (((int)words1[astart + 7]) & ShortMask) * (((int)words2[bstart + 7]) & ShortMask);
        p += d; result[rstart + 14] = (short)p; result[rstart + 15] = (short)(p >> 16);
      }
    }

    private static final int RecursionLimit = 10;

    // NOTE: Renamed from RecursiveMultiply to better show that
    // this function only takes operands of the same size, as opposed
    // to AsymmetricMultiply.
    private static void SameSizeMultiply(
      short[] resultArr,  // size 2*count
      int resultStart,
      short[] tempArr,  // size 2*count
      int tempStart,
      short[] words1,
      int words1Start,  // size count
      short[] words2,
      int words2Start,  // size count
      int count) {
      // System.out.println("RecursiveMultiply " + count + " " + count + " [r=" + resultStart + " t=" + tempStart + " a=" + words1Start + " b=" + words2Start + "]");

      if (count <= RecursionLimit) {
        if (count == 2) {
          Baseline_Multiply2(resultArr, resultStart, words1, words1Start, words2, words2Start);
        } else if (count == 4) {
          Baseline_Multiply4(resultArr, resultStart, words1, words1Start, words2, words2Start);
        } else if (count == 8) {
          Baseline_Multiply8(resultArr, resultStart, words1, words1Start, words2, words2Start);
        } else {
          SchoolbookMultiply(resultArr, resultStart, words1, words1Start, count, words2, words2Start, count);
        }
      } else {
        int countA = count;
        while (countA != 0 && words1[words1Start + countA - 1] == 0) {
          --countA;
        }
        int countB = count;
        while (countB != 0 && words2[words2Start + countB - 1] == 0) {
          --countB;
        }
        int offset2For1 = 0;
        int offset2For2 = 0;
        if (countA == 0 || countB == 0) {
          // words1 or words2 is empty, so result is 0
          java.util.Arrays.fill(resultArr,resultStart,(resultStart)+(count << 1),(short)0);
          return;
        }
        // Split words1 and words2 in two parts each
        if ((count & 1) == 0) {
          int count2 = count >> 1;
          if (countA <= count2 && countB <= count2) {
            // System.out.println("Can be smaller: " + AN + "," + BN + "," + (count2));
            java.util.Arrays.fill(resultArr,resultStart + count,(resultStart + count)+(count),(short)0);
            if (count2 == 8) {
              Baseline_Multiply8(resultArr, resultStart, words1, words1Start, words2, words2Start);
            } else {
              SameSizeMultiply(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words2, words2Start, count2);
            }
            return;
          }
          int resultMediumHigh = resultStart + count;
          int resultHigh = resultMediumHigh + count2;
          int resultMediumLow = resultStart + count2;
          int tsn = tempStart + count;
          offset2For1 = Compare(words1, words1Start, words1, words1Start + count2, count2) > 0 ? 0 : count2;
          // Absolute value of low part minus high part of words1
          SubtractOneByOne(resultArr, resultStart, words1, words1Start + offset2For1, words1, (int)(words1Start + (count2 ^ offset2For1)), count2);
          offset2For2 = Compare(words2, words2Start, words2, (int)(words2Start + count2), count2) > 0 ? 0 : count2;
          // Absolute value of low part minus high part of words2
          SubtractOneByOne(resultArr, resultMediumLow, words2, words2Start + offset2For2, words2, (int)(words2Start + (count2 ^ offset2For2)), count2);
          //---------
          // HighA * HighB
          SameSizeMultiply(resultArr, resultMediumHigh, tempArr, tsn, words1, (int)(words1Start + count2), words2, (int)(words2Start + count2), count2);
          // Medium high result = Abs(LowA-HighA) * Abs(LowB-HighB)
          SameSizeMultiply(tempArr, tempStart, tempArr, tsn, resultArr, resultStart, resultArr, (int)resultMediumLow, count2);
          // Low result = LowA * LowB
          SameSizeMultiply(resultArr, resultStart, tempArr, tsn, words1, words1Start, words2, words2Start, count2);
          int c2 = AddOneByOne(resultArr, resultMediumHigh, resultArr, resultMediumHigh, resultArr, resultMediumLow, count2);
          int c3 = c2;
          c2 += AddOneByOne(resultArr, resultMediumLow, resultArr, resultMediumHigh, resultArr, resultStart, count2);
          c3 += AddOneByOne(resultArr, resultMediumHigh, resultArr, resultMediumHigh, resultArr, resultHigh, count2);
          if (offset2For1 == offset2For2) {
            c3 -= SubtractOneByOne(resultArr, resultMediumLow, resultArr, resultMediumLow, tempArr, tempStart, count);
          } else {
            c3 += AddOneByOne(resultArr, resultMediumLow, resultArr, resultMediumLow, tempArr, tempStart, count);
          }
          c3 += Increment(resultArr, resultMediumHigh, count2, (short)c2);
          // DebugWords(resultArr,resultStart,count*2,"p6");
          if (c3 != 0) {
            Increment(resultArr, resultHigh, count2, (short)c3);
          }
          // DebugWords(resultArr,resultStart,count*2,"p7");
        } else {
          // Count is odd, high part will be 1 shorter
          int countHigh = count >> 1;  // Shorter part
          int countLow = count - countHigh;  // Longer part
          offset2For1 = CompareWithOneBiggerWords1(words1, words1Start, words1, words1Start + countLow, countLow) > 0 ? 0 : countLow;
          if (offset2For1 == 0) {
            SubtractOneBiggerWords1(resultArr, resultStart, words1, words1Start, words1, words1Start + countLow, countLow);
          } else {
            SubtractOneBiggerWords2(resultArr, resultStart, words1, words1Start + countLow, words1, words1Start, countLow);
          }
          offset2For2 = CompareWithOneBiggerWords1(words2, words2Start, words2, words2Start + countLow, countLow) > 0 ? 0 : countLow;
          if (offset2For2 == 0) {
            SubtractOneBiggerWords1(tempArr, tempStart, words2, words2Start, words2, words2Start + countLow, countLow);
          } else {
            SubtractOneBiggerWords2(tempArr, tempStart, words2, words2Start + countLow, words2, words2Start, countLow);
          }
          // Abs(LowA-HighA) * Abs(LowB-HighB)
          int shorterOffset = countHigh << 1;
          int longerOffset = countLow << 1;
          SameSizeMultiply(
            tempArr,
            tempStart + shorterOffset,
            resultArr,
            resultStart + shorterOffset,
            resultArr,
            resultStart,
            tempArr,
            tempStart,
            countLow);
          // DebugWords(resultArr, resultStart+shorterOffset,countLow << 1,"w1*w2");
          short resultTmp0 = tempArr[tempStart + shorterOffset];
          short resultTmp1 = tempArr[tempStart + shorterOffset + 1];
          // HighA * HighB
          SameSizeMultiply(
            resultArr,
            resultStart + longerOffset,
            resultArr,
            resultStart,
            words1,
            words1Start + countLow,
            words2,
            words2Start + countLow,
            countHigh);
          // LowA * LowB
          SameSizeMultiply(
            resultArr,
            resultStart,
            tempArr,
            tempStart,
            words1,
            words1Start,
            words2,
            words2Start,
            countLow);
          tempArr[tempStart + shorterOffset] = resultTmp0;
          tempArr[tempStart + shorterOffset + 1] = resultTmp1;
          int countMiddle = countLow << 1;
          // DebugWords(resultArr,resultStart,count*2,"q1");
          int c2 = AddOneByOne(resultArr, resultStart + countMiddle, resultArr, resultStart + countMiddle, resultArr, resultStart + countLow, countLow);
          int c3 = c2;
          // DebugWords(resultArr,resultStart,count*2,"q2");
          c2 += AddOneByOne(resultArr, resultStart + countLow, resultArr, resultStart + countMiddle, resultArr, resultStart, countLow);
          // DebugWords(resultArr,resultStart,count*2,"q3");
          c3 += AddUnevenSize(
            resultArr,
            resultStart + countMiddle,
            resultArr,
            resultStart + countMiddle,
            countLow,
            resultArr,
            resultStart + countMiddle + countLow,
            countLow - 2);
          // DebugWords(resultArr,resultStart,count*2,"q4");
          if (offset2For1 == offset2For2) {
            c3 -= SubtractOneByOne(resultArr, resultStart + countLow, resultArr, resultStart + countLow, tempArr, tempStart + shorterOffset, countLow << 1);
          } else {
            c3 += AddOneByOne(resultArr, resultStart + countLow, resultArr, resultStart + countLow, tempArr, tempStart + shorterOffset, countLow << 1);
          }
          // DebugWords(resultArr,resultStart,count*2,"q5");
          c3 += Increment(resultArr, resultStart + countMiddle, countLow, (short)c2);
          // DebugWords(resultArr,resultStart,count*2,"q6");
          if (c3 != 0) {
            Increment(resultArr, resultStart + countMiddle + countLow, countLow - 2, (short)c3);
          }
          // DebugWords(resultArr,resultStart,count*2,"q7");
        }
      }
    }

    private static void RecursiveSquare(
      short[] resultArr,
      int resultStart,
      short[] tempArr,
      int tempStart,
      short[] words1,
      int words1Start,
      int count) {
      if (count <= RecursionLimit) {
        if (count == 2) {
          Baseline_Square2(resultArr, resultStart, words1, words1Start);
        } else if (count == 4) {
          Baseline_Square4(resultArr, resultStart, words1, words1Start);
        } else if (count == 8) {
          Baseline_Square8(resultArr, resultStart, words1, words1Start);
        } else {
          SchoolbookSquare(resultArr, resultStart, words1, words1Start, count);
        }
      } else if ((count & 1) == 0) {
        int count2 = count >> 1;
        RecursiveSquare(resultArr, resultStart, tempArr, tempStart + count, words1, words1Start, count2);
        RecursiveSquare(resultArr, resultStart + count, tempArr, tempStart + count, words1, words1Start + count2, count2);
        SameSizeMultiply(
          tempArr,
          tempStart,
          tempArr,
          tempStart + count,
          words1,
          words1Start,
          words1,
          words1Start + count2,
          count2);
        int carry = AddOneByOne(resultArr, (int)(resultStart + count2), resultArr, (int)(resultStart + count2), tempArr, tempStart, count);
        carry += AddOneByOne(resultArr, (int)(resultStart + count2), resultArr, (int)(resultStart + count2), tempArr, tempStart, count);
        Increment(resultArr, (int)(resultStart + count + count2), count2, (short)carry);
      } else {
        SameSizeMultiply(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words1, words1Start, count);
      }
    }

    private static void SchoolbookSquare(
      short[] resultArr,
      int resultStart,
      short[] words1,
      int words1Start,
      int words1Count) {
      // Method assumes that resultArr was already zeroed,
      // if resultArr is the same as words1
      int cstart;
      for (int i = 0; i < words1Count; ++i) {
        cstart = resultStart + i;
        {
          short carry = 0;
          int valueBint = ((int)words1[words1Start + i]) & 0xffff;
          for (int j = 0; j < words1Count; ++j) {
            int p;
            p = (((int)words1[words1Start + j]) & 0xffff) * valueBint;
            p += ((int)carry) & 0xffff;
            if (i != 0) {
              p += ((int)resultArr[cstart + j]) & 0xffff;
            }
            resultArr[cstart + j] = (short)p;
            carry = (short)(p >> 16);
          }
          resultArr[cstart + words1Count] = carry;
        }
      }
    }

    private static void SchoolbookMultiply(
      short[] resultArr,
      int resultStart,
      short[] words1,
      int words1Start,
      int words1Count,
      short[] words2,
      int words2Start,
      int words2Count) {
      // Method assumes that resultArr was already zeroed,
      // if resultArr is the same as words1 or words2
      int cstart;
      if (words1Count < words2Count) {
        // words1 is shorter than words2, so put words2 on top
        for (int i = 0; i < words1Count; ++i) {
          cstart = resultStart + i;
          {
            short carry = 0;
            int valueBint = ((int)words1[words1Start + i]) & 0xffff;
            for (int j = 0; j < words2Count; ++j) {
              int p;
              p = (((int)words2[words2Start + j]) & 0xffff) * valueBint;
              p += ((int)carry) & 0xffff;
              if (i != 0) {
                p += ((int)resultArr[cstart + j]) & 0xffff;
              }
              resultArr[cstart + j] = (short)p;
              carry = (short)(p >> 16);
            }
            resultArr[cstart + words2Count] = carry;
          }
        }
      } else {
        // words2 is shorter than words1
        for (int i = 0; i < words2Count; ++i) {
          cstart = resultStart + i;
          {
            short carry = 0;
            int valueBint = ((int)words2[words2Start + i]) & 0xffff;
            for (int j = 0; j < words1Count; ++j) {
              int p;
              p = (((int)words1[words1Start + j]) & 0xffff) * valueBint;
              p += ((int)carry) & 0xffff;
              if (i != 0) {
                p += ((int)resultArr[cstart + j]) & 0xffff;
              }
              resultArr[cstart + j] = (short)p;
              carry = (short)(p >> 16);
            }
            resultArr[cstart + words1Count] = carry;
          }
        }
      }
    }
    /*
    private static void DebugWords(short[] a, int astart, int count, String msg) {
      Console.Write("Words(" + msg + "): ");
      for (int i = 0; i < count; ++i) {
        Console.Write("{0:X4} ", a[astart + i]);
      }
      System.out.println("");
      BigInteger bi = new BigInteger();
      bi.reg = new short[count];
      bi.wordCount = count;
      System.arraycopy(a, astart, bi.reg, 0, count);
      System.out.println("Value(" + msg + "): " + bi);
    }
     */
    private static void ChunkedLinearMultiply(
      short[] productArr,
      int cstart,
      short[] tempArr,
      int tempStart,  // uses bcount*4 space
      short[] words1,
      int astart,
      int acount,  // Equal size or longer
      short[] words2,
      int bstart,
      int bcount) {

      {
        int carryPos = 0;
        // Set carry to zero
        java.util.Arrays.fill(productArr,cstart,(cstart)+(bcount),(short)0);
        for (int i = 0; i < acount; i += bcount) {
          int diff = acount - i;
          if (diff > bcount) {
            SameSizeMultiply(
              tempArr,
              tempStart,  // uses bcount*2 space
              tempArr,
              tempStart + bcount + bcount,  // uses bcount*2 space
              words1,
              astart + i,
              words2,
              bstart,
              bcount);
            // Add carry
            AddUnevenSize(
              tempArr,
              tempStart,
              tempArr,
              tempStart,
              bcount + bcount,
              productArr,
              cstart + carryPos,
              bcount);
            // Copy product and carry
            System.arraycopy(tempArr, tempStart, productArr, cstart + i, bcount + bcount);
            carryPos += bcount;
          } else {
            AsymmetricMultiply(
              tempArr,
              tempStart,  // uses diff + bcount space
              tempArr,
              tempStart + diff + bcount,  // uses diff + bcount space
              words1,
              astart + i,
              diff,
              words2,
              bstart,
              bcount);
            // Add carry
            AddUnevenSize(
              tempArr,
              tempStart,
              tempArr,
              tempStart,
              diff + bcount,
              productArr,
              cstart + carryPos,
              bcount);
            // Copy product without carry
            System.arraycopy(tempArr, tempStart, productArr, cstart + i, diff + bcount);
          }
        }
      }
    }

    // Multiplies two operands of different sizes
    private static void AsymmetricMultiply(
      short[] resultArr,
      int resultStart,  // uses words1Count + words2Count space
      short[] tempArr,
      int tempStart,  // uses words1Count + words2Count space
      short[] words1,
      int words1Start,
      int words1Count,
      short[] words2,
      int words2Start,
      int words2Count) {
      // System.out.println("AsymmetricMultiply " + words1Count + " " + words2Count + " [r=" + resultStart + " t=" + tempStart + " a=" + words1Start + " b=" + words2Start + "]");

      if (words1Count == words2Count) {
        if (words1Start == words2Start && words1 == words2) {
          // Both operands have the same value and the same word count
          RecursiveSquare(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words1Count);
        } else if (words1Count == 2) {
          // Both operands have a word count of 2
          Baseline_Multiply2(resultArr, resultStart, words1, words1Start, words2, words2Start);
        } else {
          // Other cases where both operands have the same word count
          SameSizeMultiply(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words2, words2Start, words1Count);
        }

        return;
      }
      if (words1Count > words2Count) {
        // Ensure that words1 is smaller by swapping if necessary
        short[] tmp1 = words1;
        words1 = words2;
        words2 = tmp1;
        int tmp3 = words1Start;
        words1Start = words2Start;
        words2Start = tmp3;
        int tmp2 = words1Count;
        words1Count = words2Count;
        words2Count = tmp2;
      }

      if (words1Count == 1 || (words1Count == 2 && words1[words1Start + 1] == 0)) {
        switch (words1[words1Start]) {
          case 0:
            // words1 is zero, so result is 0
            java.util.Arrays.fill(resultArr,resultStart,(resultStart)+(words2Count + 2),(short)0);
            return;
          case 1:
            System.arraycopy(words2, words2Start, resultArr, resultStart, (int)words2Count);
            resultArr[resultStart + words2Count] = (short)0;
            resultArr[resultStart + words2Count + 1] = (short)0;
            return;
          default:
            resultArr[resultStart + words2Count] = LinearMultiply(resultArr, resultStart, words2, words2Start, words1[words1Start], words2Count);
            resultArr[resultStart + words2Count + 1] = (short)0;
            return;
        }
      }
      if (words1Count == 2 && (words2Count & 1) == 0) {
        int a0 = ((int)words1[words1Start]) & 0xffff;
        int a1 = ((int)words1[words1Start + 1]) & 0xffff;
        resultArr[resultStart + words2Count] = (short)0;
        resultArr[resultStart + words2Count + 1] = (short)0;
        AtomicMultiplyOpt(resultArr, resultStart, a0, a1, words2, words2Start, 0, words2Count);
        AtomicMultiplyAddOpt(resultArr, resultStart, a0, a1, words2, words2Start, 2, words2Count);
        return;
      }
      if (words1Count <= 10 && words2Count <= 10) {
        SchoolbookMultiply(resultArr, resultStart, words1, words1Start, words1Count, words2, words2Start, words2Count);
      } else {
        int wordsRem = words2Count % words1Count;
        int evenmult = (words2Count / words1Count) & 1;
        int i;
        // System.out.println("counts=" + words1Count + "," + words2Count + " res=" + (resultStart + words1Count) + " temp=" + (tempStart + (words1Count << 1)) + " rem=" + wordsRem + " evenwc=" + evenmult);
        if (wordsRem == 0) {
          // words2Count is divisible by words1count
          if (evenmult == 0) {
            SameSizeMultiply(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words2, words2Start, words1Count);
            System.arraycopy(resultArr, resultStart + words1Count, tempArr, (int)(tempStart + (words1Count << 1)), words1Count);
            for (i = words1Count << 1; i < words2Count; i += words1Count << 1) {
              SameSizeMultiply(tempArr, tempStart + words1Count + i, tempArr, tempStart, words1, words1Start, words2, words2Start + i, words1Count);
            }
            for (i = words1Count; i < words2Count; i += words1Count << 1) {
              SameSizeMultiply(resultArr, resultStart + i, tempArr, tempStart, words1, words1Start, words2, words2Start + i, words1Count);
            }
          } else {
            for (i = 0; i < words2Count; i += words1Count << 1) {
              SameSizeMultiply(resultArr, resultStart + i, tempArr, tempStart, words1, words1Start, words2, words2Start + i, words1Count);
            }
            for (i = words1Count; i < words2Count; i += words1Count << 1) {
              SameSizeMultiply(tempArr, tempStart + words1Count + i, tempArr, tempStart, words1, words1Start, words2, words2Start + i, words1Count);
            }
          }
          if (Add(resultArr, resultStart + words1Count, resultArr, resultStart + words1Count, tempArr, tempStart + (words1Count << 1), words2Count - words1Count) != 0) {
            Increment(resultArr, (int)(resultStart + words2Count), words1Count, (short)1);
          }
        } else if ((words1Count + words2Count) >= (words1Count << 2)) {
          // System.out.println("Chunked Linear Multiply Long");
          ChunkedLinearMultiply(
            resultArr,
            resultStart,
            tempArr,
            tempStart,
            words2,
            words2Start,
            words2Count,
            words1,
            words1Start,
            words1Count);
        } else if (words1Count + 1 == words2Count ||
                   (words1Count + 2 == words2Count && words2[words2Start + words2Count - 1] == 0)) {
          java.util.Arrays.fill(resultArr,resultStart,(resultStart)+(words1Count + words2Count),(short)0);
          // Multiply the low parts of each operand
          SameSizeMultiply(
            resultArr,
            resultStart,
            tempArr,
            tempStart,
            words1,
            words1Start,
            words2,
            words2Start,
            words1Count);
          // Multiply the high parts
          // while adding carry from the high part of the product
          short carry = LinearMultiplyAdd(
            resultArr,
            resultStart + words1Count,
            words1,
            words1Start,
            words2[words2Start + words1Count],
            words1Count);
          resultArr[resultStart + words1Count + words1Count] = carry;
        } else {
          short[] t2 = new short[words1Count << 2];
          // System.out.println("Chunked Linear Multiply Short");
          ChunkedLinearMultiply(
            resultArr,
            resultStart,
            t2,
            0,
            words2,
            words2Start,
            words2Count,
            words1,
            words1Start,
            words1Count);
        }
      }
    }

    private static int MakeUint(short first, short second) {
      return ((int)((((int)first) & 0xffff) | ((int)second << 16)));
    }

    private static short GetLowHalf(int val) {
      return ((short)(val & 0xffff));
    }

    private static short GetHighHalf(int val) {
      return ((short)((val >> 16) & 0xffff));
    }

    private static short GetHighHalfAsBorrow(int val) {
      return ((short)(0 - ((val >> 16) & 0xffff)));
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

    private static short Divide32By16(int dividendLow, short divisorShort, boolean returnRemainder) {
      int tmpInt;
      int dividendHigh = 0;
      int intDivisor = ((int)divisorShort) & 0xffff;
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
            ++dividendLow;
          }
        }
      }
      return returnRemainder ?
        ((short)(((int)dividendHigh) & 0xffff)) :
        ((short)(((int)dividendLow) & 0xffff));
    }

    private static short DivideUnsigned(int x, short y) {
      {
        if ((x >> 31) == 0) {
          // x is already nonnegative
          int iy = ((int)y) & 0xffff;
          return (short)(((int)x / iy) & 0xffff);
        }
        return Divide32By16(x, y, false);
      }
    }

    private static short RemainderUnsigned(int x, short y) {
      {
        int iy = ((int)y) & 0xffff;
        return ((x >> 31) == 0) ? ((short)(((int)x % iy) & 0xffff)) : Divide32By16(x, y, true);
      }
    }

    private static short DivideThreeWordsByTwo(short[] words1, int words1Start, short valueB0, short valueB1) {
      // Debugif(!(words1[2] < valueB1 || (words1[2]==valueB1 && words1[1] < valueB0)))Assert.fail("{0} line {1}: words1[2] < valueB1 || (words1[2]==valueB1 && words1[1] < valueB0)","integer.cpp",360);
      short valueQ;
      {
        valueQ = ((short)(valueB1 + 1) == 0) ? words1[words1Start + 2] : ((valueB1 != 0) ? DivideUnsigned(MakeUint(words1[words1Start + 1], words1[words1Start + 2]), (short)(((int)valueB1 + 1) & 0xffff)) : DivideUnsigned(MakeUint(words1[words1Start], words1[words1Start + 1]), valueB0));

        int valueQint = ((int)valueQ) & 0xffff;
        int valueB0int = ((int)valueB0) & 0xffff;
        int valueB1int = ((int)valueB1) & 0xffff;
        int p = valueB0int * valueQint;
        int u = (((int)words1[words1Start]) & 0xffff) - (p & 0xffff);
        words1[words1Start] = GetLowHalf(u);
        u = (((int)words1[words1Start + 1]) & 0xffff) - ((p >> 16) & 0xffff) -
          (((int)GetHighHalfAsBorrow(u)) & 0xffff) - (valueB1int * valueQint);
        words1[words1Start + 1] = GetLowHalf(u);
        words1[words1Start + 2] += GetHighHalf(u);
        while (words1[words1Start + 2] != 0 ||
               (((int)words1[words1Start + 1]) & 0xffff) > (((int)valueB1) & 0xffff) ||
               (words1[words1Start + 1] == valueB1 && (((int)words1[words1Start]) & 0xffff) >= (((int)valueB0) & 0xffff))) {
          u = (((int)words1[words1Start]) & 0xffff) - valueB0int;
          words1[words1Start] = GetLowHalf(u);
          u = (((int)words1[words1Start + 1]) & 0xffff) - valueB1int - (((int)GetHighHalfAsBorrow(u)) & 0xffff);
          words1[words1Start + 1] = GetLowHalf(u);
          words1[words1Start + 2] += GetHighHalf(u);
          ++valueQ;
        }
      }
      return valueQ;
    }

    private static void DivideFourWordsByTwo(
      short[] quotient,
      int quotientStart,
      short[] words1,
      int words1Start,
      short word2A,
      short word2B,
      short[] temp) {
      if (word2A == 0 && word2B == 0) {
        // if divisor is 0, we assume divisor.compareTo(BigInteger.valueOf(2))==0**32
        quotient[quotientStart] = words1[words1Start + 2];
        quotient[quotientStart + 1] = words1[words1Start + 3];
      } else {
        temp[0] = words1[words1Start];
        temp[1] = words1[words1Start + 1];
        temp[2] = words1[words1Start + 2];
        temp[3] = words1[words1Start + 3];
        short valueQ1 = DivideThreeWordsByTwo(temp, 1, word2A, word2B);
        short valueQ0 = DivideThreeWordsByTwo(temp, 0, word2A, word2B);
        quotient[quotientStart] = valueQ0;
        quotient[quotientStart + 1] = valueQ1;
      }
    }

    private static void AtomicMultiplyOpt(short[] c, int valueCstart, int valueA0, int valueA1, short[] words2, int words2Start, int istart, int iend) {
      short s;
      int d;
      int first1MinusFirst0 = ((int)valueA1 - valueA0) & 0xffff;
      valueA1 &= 0xffff;
      valueA0 &= 0xffff;
      {
        if (valueA1 >= valueA0) {
          for (int i = istart; i < iend; i += 4) {
            int valueB0 = ((int)words2[words2Start + i]) & 0xffff;
            int valueB1 = ((int)words2[words2Start + i + 1]) & 0xffff;
            int csi = valueCstart + i;
            if (valueB0 >= valueB1) {
              s = (short)0;
              d = first1MinusFirst0 * (((int)valueB0 - valueB1) & 0xffff);
            } else {
              s = (short)first1MinusFirst0;
              d = (((int)s) & 0xffff) * (((int)valueB0 - valueB1) & 0xffff);
            }
            int valueA0B0 = valueA0 * valueB0;
            c[csi] = (short)(((int)valueA0B0) & 0xffff);
            int a0b0high = (valueA0B0 >> 16) & 0xffff;
            int valueA1B1 = valueA1 * valueB1;
            int tempInt;
            tempInt = a0b0high +
              (((int)valueA0B0) & 0xffff) + (((int)d) & 0xffff) + (((int)valueA1B1) & 0xffff);
            c[csi + 1] = (short)(((int)tempInt) & 0xffff);

            tempInt = valueA1B1 + (((int)(tempInt >> 16)) & 0xffff) +
              a0b0high + (((int)(d >> 16)) & 0xffff) + (((int)(valueA1B1 >> 16)) & 0xffff) -
              (((int)s) & 0xffff);

            c[csi + 2] = (short)(((int)tempInt) & 0xffff);
            c[csi + 3] = (short)(((int)(tempInt >> 16)) & 0xffff);
          }
        } else {
          for (int i = istart; i < iend; i += 4) {
            int valueB0 = ((int)words2[words2Start + i]) & 0xffff;
            int valueB1 = ((int)words2[words2Start + i + 1]) & 0xffff;
            int csi = valueCstart + i;
            if (valueB0 > valueB1) {
              s = (short)(((int)valueB0 - valueB1) & 0xffff);
              d = first1MinusFirst0 * (((int)s) & 0xffff);
            } else {
              s = (short)0;
              d = (((int)valueA0 - valueA1) & 0xffff) * (((int)valueB1 - valueB0) & 0xffff);
            }
            int valueA0B0 = valueA0 * valueB0;
            int a0b0high = (valueA0B0 >> 16) & 0xffff;
            c[csi] = (short)(((int)valueA0B0) & 0xffff);

            int valueA1B1 = valueA1 * valueB1;
            int tempInt;
            tempInt = a0b0high +
              (((int)valueA0B0) & 0xffff) + (((int)d) & 0xffff) + (((int)valueA1B1) & 0xffff);
            c[csi + 1] = (short)(((int)tempInt) & 0xffff);

            tempInt = valueA1B1 + (((int)(tempInt >> 16)) & 0xffff) +
              a0b0high + (((int)(d >> 16)) & 0xffff) + (((int)(valueA1B1 >> 16)) & 0xffff) -
              (((int)s) & 0xffff);

            c[csi + 2] = (short)(((int)tempInt) & 0xffff);
            c[csi + 3] = (short)(((int)(tempInt >> 16)) & 0xffff);
          }
        }
      }
    }

    private static void AtomicMultiplyAddOpt(short[] c, int valueCstart, int valueA0, int valueA1, short[] words2, int words2Start, int istart, int iend) {
      short s;
      int d;
      int first1MinusFirst0 = ((int)valueA1 - valueA0) & 0xffff;
      valueA1 &= 0xffff;
      valueA0 &= 0xffff;
      {
        if (valueA1 >= valueA0) {
          for (int i = istart; i < iend; i += 4) {
            int b0 = ((int)words2[words2Start + i]) & 0xffff;
            int b1 = ((int)words2[words2Start + i + 1]) & 0xffff;
            int csi = valueCstart + i;
            if (b0 >= b1) {
              s = (short)0;
              d = first1MinusFirst0 * (((int)b0 - b1) & 0xffff);
            } else {
              s = (short)first1MinusFirst0;
              d = (((int)s) & 0xffff) * (((int)b0 - b1) & 0xffff);
            }
            int valueA0B0 = valueA0 * b0;
            int a0b0high = (valueA0B0 >> 16) & 0xffff;
            int tempInt;
            tempInt = valueA0B0 + (((int)c[csi]) & 0xffff);
            c[csi] = (short)(((int)tempInt) & 0xffff);

            int valueA1B1 = valueA1 * b1;
            int a1b1low = valueA1B1 & 0xffff;
            int a1b1high = ((int)(valueA1B1 >> 16)) & 0xffff;
            tempInt = (((int)(tempInt >> 16)) & 0xffff) + (((int)valueA0B0) & 0xffff) + (((int)d) & 0xffff) + a1b1low + (((int)c[csi + 1]) & 0xffff);
            c[csi + 1] = (short)(((int)tempInt) & 0xffff);

            tempInt = (((int)(tempInt >> 16)) & 0xffff) + a1b1low + a0b0high + (((int)(d >> 16)) & 0xffff) +
              a1b1high - (((int)s) & 0xffff) + (((int)c[csi + 2]) & 0xffff);
            c[csi + 2] = (short)(((int)tempInt) & 0xffff);

            tempInt = (((int)(tempInt >> 16)) & 0xffff) + a1b1high + (((int)c[csi + 3]) & 0xffff);
            c[csi + 3] = (short)(((int)tempInt) & 0xffff);
            if ((tempInt >> 16) != 0) {
              ++c[csi + 4];
              c[csi + 5] += (short)((c[csi + 4] == 0) ? 1 : 0);
            }
          }
        } else {
          for (int i = istart; i < iend; i += 4) {
            int valueB0 = ((int)words2[words2Start + i]) & 0xffff;
            int valueB1 = ((int)words2[words2Start + i + 1]) & 0xffff;
            int csi = valueCstart + i;
            if (valueB0 > valueB1) {
              s = (short)(((int)valueB0 - valueB1) & 0xffff);
              d = first1MinusFirst0 * (((int)s) & 0xffff);
            } else {
              s = (short)0;
              d = (((int)valueA0 - valueA1) & 0xffff) * (((int)valueB1 - valueB0) & 0xffff);
            }
            int valueA0B0 = valueA0 * valueB0;
            int a0b0high = (valueA0B0 >> 16) & 0xffff;
            int tempInt;
            tempInt = valueA0B0 + (((int)c[csi]) & 0xffff);
            c[csi] = (short)(((int)tempInt) & 0xffff);

            int valueA1B1 = valueA1 * valueB1;
            int a1b1low = valueA1B1 & 0xffff;
            int a1b1high = (valueA1B1 >> 16) & 0xffff;
            tempInt = (((int)(tempInt >> 16)) & 0xffff) + (((int)valueA0B0) & 0xffff) + (((int)d) & 0xffff) + a1b1low + (((int)c[csi + 1]) & 0xffff);
            c[csi + 1] = (short)(((int)tempInt) & 0xffff);

            tempInt = (((int)(tempInt >> 16)) & 0xffff) + a1b1low + a0b0high + (((int)(d >> 16)) & 0xffff) +
              a1b1high - (((int)s) & 0xffff) + (((int)c[csi + 2]) & 0xffff);
            c[csi + 2] = (short)(((int)tempInt) & 0xffff);

            tempInt = (((int)(tempInt >> 16)) & 0xffff) + a1b1high + (((int)c[csi + 3]) & 0xffff);
            c[csi + 3] = (short)(((int)tempInt) & 0xffff);
            if ((tempInt >> 16) != 0) {
              ++c[csi + 4];
              c[csi + 5] += (short)((c[csi + 4] == 0) ? 1 : 0);
            }
          }
        }
      }
    }

    private static void Divide(
      short[] remainderArr,
      int remainderStart,  // remainder; size: words2Count
      short[] quotientArr,
      int quotientStart,  // quotient
      short[] tempArr,
      int tempStart,  // scratch space
      short[] words1,
      int words1Start,
      int words1Count,  // dividend
      short[] words2,
      int words2Start,
      int words2Count) {
      // set up temporary work space

      if (words2Count == 0) {
        throw new ArithmeticException("division by zero");
      }
      if (words2Count == 1) {
        if (words2[words2Start] == 0) {
          throw new ArithmeticException("division by zero");
        }
        int smallRemainder = ((int)FastDivideAndRemainder(
          quotientArr,
          quotientStart,
          words1,
          words1Start,
          words1Count,
          words2[words2Start])) & 0xffff;
        remainderArr[remainderStart] = (short)smallRemainder;
        return;
      }

      short[] quot = quotientArr;
      if (quotientArr == null) {
        quot = new short[2];
      }
      int valueTBstart = (int)(tempStart + (words1Count + 2));
      int valueTPstart = (int)(tempStart + (words1Count + 2 + words2Count));
      {
        // copy words2 into TB and normalize it so that TB has highest bit set to 1
        int shiftWords = (short)(words2[words2Start + words2Count - 1] == 0 ? 1 : 0);
        tempArr[valueTBstart] = (short)0;
        tempArr[valueTBstart + words2Count - 1] = (short)0;
        System.arraycopy(words2, words2Start, tempArr, (int)(valueTBstart + shiftWords), words2Count - shiftWords);
        short shiftBits = (short)((short)16 - BitPrecision(tempArr[valueTBstart + words2Count - 1]));
        ShiftWordsLeftByBits(
          tempArr,
          valueTBstart,
          words2Count,
          shiftBits);
        // copy words1 into valueTA and normalize it
        tempArr[0] = (short)0;
        tempArr[words1Count] = (short)0;
        tempArr[words1Count + 1] = (short)0;
        System.arraycopy(words1, words1Start, tempArr, (int)(tempStart + shiftWords), words1Count);
        ShiftWordsLeftByBits(
          tempArr,
          tempStart,
          words1Count + 2,
          shiftBits);

        if (tempArr[tempStart + words1Count + 1] == 0 && (((int)tempArr[tempStart + words1Count]) & 0xffff) <= 1) {
          if (quotientArr != null) {
            quotientArr[quotientStart + words1Count - words2Count + 1] = (short)0;
            quotientArr[quotientStart + words1Count - words2Count] = (short)0;
          }
          while (
            tempArr[words1Count] != 0 || Compare(
              tempArr,
              (int)(tempStart + words1Count - words2Count),
              tempArr,
              valueTBstart,
              words2Count) >= 0) {
            tempArr[words1Count] -= (short)Subtract(
              tempArr,
              tempStart + words1Count - words2Count,
              tempArr,
              tempStart + words1Count - words2Count,
              tempArr,
              valueTBstart,
              words2Count);
            if (quotientArr != null) {
              quotientArr[quotientStart + words1Count - words2Count] += (short)1;
            }
          }
        } else {
          words1Count += 2;
        }

        short valueBT0 = (short)(tempArr[valueTBstart + words2Count - 2] + (short)1);
        short valueBT1 = (short)(tempArr[valueTBstart + words2Count - 1] + (short)(valueBT0 == (short)0 ? 1 : 0));

        // start reducing valueTA mod TB, 2 words at a time
        short[] valueTAtomic = new short[4];
        for (int i = words1Count - 2; i >= words2Count; i -= 2) {
          int qs = (quotientArr == null) ? 0 : quotientStart + i - words2Count;
          DivideFourWordsByTwo(quot, qs, tempArr, (int)(tempStart + i - 2), valueBT0, valueBT1, valueTAtomic);
          // now correct the underestimated quotient
          int valueRstart2 = tempStart + i - words2Count;
          int n = words2Count;
          {
            int quotient0 = quot[qs];
            int quotient1 = quot[qs + 1];
            if (quotient1 == 0) {
              short carry = LinearMultiply(tempArr, valueTPstart, tempArr, valueTBstart, (short)quotient0, n);
              tempArr[valueTPstart + n] = carry;
              tempArr[valueTPstart + n + 1] = 0;
            } else if (n == 2) {
              Baseline_Multiply2(tempArr, valueTPstart, quot, qs, tempArr, valueTBstart);
            } else {
              tempArr[valueTPstart + n] = (short)0;
              tempArr[valueTPstart + n + 1] = (short)0;
              quotient0 &= 0xffff;
              quotient1 &= 0xffff;
              AtomicMultiplyOpt(tempArr, valueTPstart, quotient0, quotient1, tempArr, valueTBstart, 0, n);
              AtomicMultiplyAddOpt(tempArr, valueTPstart, quotient0, quotient1, tempArr, valueTBstart, 2, n);
            }
            Subtract(tempArr, valueRstart2, tempArr, valueRstart2, tempArr, valueTPstart, n + 2);
            while (tempArr[valueRstart2 + n] != 0 || Compare(tempArr, valueRstart2, tempArr, valueTBstart, n) >= 0) {
              tempArr[valueRstart2 + n] -= (short)Subtract(tempArr, valueRstart2, tempArr, valueRstart2, tempArr, valueTBstart, n);
              if (quotientArr != null) {
                ++quotientArr[qs];
                quotientArr[qs + 1] += (short)((quotientArr[qs] == 0) ? 1 : 0);
              }
            }
          }
        }
        if (remainderArr != null) {  // If the remainder is non-null
          // copy valueTA into result, and denormalize it
          System.arraycopy(tempArr, (int)(tempStart + shiftWords), remainderArr, remainderStart, words2Count);
          ShiftWordsRightByBits(remainderArr, remainderStart, words2Count, shiftBits);
        }
      }
    }

    private static int RoundupSize(int n) {
      return n + (n & 1);
    }

    private final boolean negative;
    private final int wordCount;
    private final short[] words;

    private BigInteger(int wordCount, short[] reg, boolean negative) {
      this.wordCount = wordCount;
      this.words = reg;
      this.negative = negative;
    }

    /**
     * Initializes a BigInteger object from an array of bytes.
     * @param bytes A byte array. Can be empty, in which case the return value
     * is 0.
     * @param littleEndian A Boolean object.
     * @return A BigInteger object.
     * @throws java.lang.NullPointerException The parameter {@code bytes}
     * is null.
     */
    public static BigInteger fromByteArray(byte[] bytes, boolean littleEndian) {
      if (bytes == null) {
        throw new NullPointerException("bytes");
      }
      if (bytes.length == 0) {
        return BigInteger.ZERO;
      }
      if (bytes == null) {
        throw new NullPointerException("bytes");
      }
      int len = bytes.length;
      int wordLength = ((int)len + 1) >> 1;
      wordLength = RoundupSize(wordLength);
      short[] newreg = new short[wordLength];
      int valueJIndex = littleEndian ? len - 1 : 0;
      boolean numIsNegative = (bytes[valueJIndex] & 0x80) != 0;
      boolean newnegative = numIsNegative;
      int j = 0;
      if (!numIsNegative) {
        for (int i = 0; i < len; i += 2, j++) {
          int index = littleEndian ? i : len - 1 - i;
          int index2 = littleEndian ? i + 1 : len - 2 - i;
          newreg[j] = (short)(((int)bytes[index]) & 0xff);
          if (index2 >= 0 && index2 < len) {
            newreg[j] |= ((short)(((short)bytes[index2]) << 8));
          }
        }
      } else {
        for (int i = 0; i < len; i += 2, j++) {
          int index = littleEndian ? i : len - 1 - i;
          int index2 = littleEndian ? i + 1 : len - 2 - i;
          newreg[j] = (short)(((int)bytes[index]) & 0xff);
          if (index2 >= 0 && index2 < len) {
            newreg[j] |= ((short)(((short)bytes[index2]) << 8));
          } else {
            // sign extend the last byte
            newreg[j] |= ((short)0xff00);
          }
        }
        for (; j < newreg.length; ++j) {
          newreg[j] = ((short)0xffff);  // sign extend remaining words
        }
        TwosComplement(newreg, 0, (int)newreg.length);
      }
      int newwordCount = newreg.length;
      while (newwordCount != 0 &&
             newreg[newwordCount - 1] == 0) {
        --newwordCount;
      }
      return (newwordCount == 0) ? BigInteger.ZERO : (new BigInteger(newwordCount, newreg, newnegative));
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
        System.arraycopy(a, 0, newa, 0, a.length);
        return newa;
      }
      return a;
    }

    /**
     * Returns whether a bit is set in the two's-complement representation
     * of this object's value.
     * @param index Zero based index of the bit to test. 0 means the least significant
     * bit.
     * @return True if the specified bit is set; otherwise, false.
     */
    public boolean testBit(int index) {
      if (index < 0) {
        throw new IllegalArgumentException("index");
      }
      if (this.wordCount == 0) {
        return false;
      }
      if (this.negative) {
        int tcindex = 0;
        int wordpos = index / 16;
        if (wordpos >= this.words.length) {
          return true;
        }
        while (tcindex < wordpos && this.words[tcindex] == 0) {
          ++tcindex;
        }
        short tc;
        {
          tc = this.words[wordpos];
          if (tcindex == wordpos) {
            --tc;
          }
          tc = (short)~tc;
        }
        return (boolean)(((tc >> (int)(index & 15)) & 1) != 0);
      }
      return this.GetUnsignedBit(index);
    }

    private boolean GetUnsignedBit(int n) {

      return ((n >> 4) < this.words.length) && ((boolean)(((this.words[(n >> 4)] >> (int)(n & 15)) & 1) != 0));
    }

    /**
     * Returns a byte array of this object&apos;s value.
     * @param littleEndian A Boolean object.
     * @return A byte array that represents the value of this object.
     */
    public byte[] toByteArray(boolean littleEndian) {
      int sign = this.signum();
      if (sign == 0) {
        return new byte[] { (byte)0  };
      }
      if (sign > 0) {
        int byteCount = this.ByteCount();
        int byteArrayLength = byteCount;
        if (this.GetUnsignedBit((byteCount * 8) - 1)) {
          ++byteArrayLength;
        }
        byte[] bytes = new byte[byteArrayLength];
        int j = 0;
        for (int i = 0; i < byteCount; i += 2, j++) {
          int index = littleEndian ? i : bytes.length - 1 - i;
          int index2 = littleEndian ? i + 1 : bytes.length - 2 - i;
          bytes[index] = (byte)(this.words[j] & 0xff);
          if (index2 >= 0 && index2 < byteArrayLength) {
            bytes[index2] = (byte)((this.words[j] >> 8) & 0xff);
          }
        }
        return bytes;
      } else {
        short[] regdata = new short[this.words.length];
        System.arraycopy(this.words, 0, regdata, 0, this.words.length);
        TwosComplement(regdata, 0, (int)regdata.length);
        int byteCount = regdata.length * 2;
        for (int i = regdata.length - 1; i >= 0; --i) {
          if (regdata[i] == ((short)0xffff)) {
            byteCount -= 2;
          } else if ((regdata[i] & 0xff80) == 0xff80) {
            // signed first byte, 0xff second
            --byteCount;
            break;
          } else if ((regdata[i] & 0x8000) == 0x8000) {
            // signed second byte
            break;
          } else {
            // unsigned second byte
            ++byteCount;
            break;
          }
        }
        if (byteCount == 0) {
          byteCount = 1;
        }
        byte[] bytes = new byte[byteCount];
        bytes[littleEndian ? bytes.length - 1 : 0] = (byte)0xff;
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
      if (numberBits == 0 || this.wordCount == 0) {
        return this;
      }
      if (numberBits < 0) {
        return (numberBits == Integer.MIN_VALUE) ? this.shiftRight(1).shiftRight(Integer.MAX_VALUE) : this.shiftRight(-numberBits);
      }
      int numWords = (int)this.wordCount;
      int shiftWords = (int)(numberBits >> 4);
      int shiftBits = (int)(numberBits & 15);
      if (!this.negative) {
        short[] ret = new short[RoundupSize(numWords + BitsToWords((int)numberBits))];
        System.arraycopy(this.words, 0, ret, shiftWords, numWords);
        ShiftWordsLeftByBits(ret, (int)shiftWords, numWords + BitsToWords(shiftBits), shiftBits);
        return new BigInteger(CountWords(ret, ret.length), ret, false);
      } else {
        short[] ret = new short[RoundupSize(numWords + BitsToWords((int)numberBits))];
        System.arraycopy(this.words, 0, ret, 0, numWords);
        TwosComplement(ret, 0, (int)ret.length);
        ShiftWordsLeftByWords(ret, 0, numWords + shiftWords, shiftWords);
        ShiftWordsLeftByBits(ret, (int)shiftWords, numWords + BitsToWords(shiftBits), shiftBits);
        TwosComplement(ret, 0, (int)ret.length);
        return new BigInteger(CountWords(ret, ret.length), ret, true);
      }
    }

    /**
     * Returns a big integer with the bits shifted to the right.
     * @param numberBits Number of bits to shift right.
     * @return A BigInteger object.
     */
    public BigInteger shiftRight(int numberBits) {
      if (numberBits == 0 || this.wordCount == 0) {
        return this;
      }
      if (numberBits < 0) {
        return (numberBits == Integer.MIN_VALUE) ? this.shiftLeft(1).shiftLeft(Integer.MAX_VALUE) : this.shiftLeft(-numberBits);
      }
      int numWords = (int)this.wordCount;
      int shiftWords = (int)(numberBits >> 4);
      int shiftBits = (int)(numberBits & 15);
      short[] ret;
      int retWordCount;
      if (this.negative) {
        ret = new short[this.words.length];
        System.arraycopy(this.words, 0, ret, 0, numWords);
        TwosComplement(ret, 0, (int)ret.length);
        ShiftWordsRightByWordsSignExtend(ret, 0, numWords, shiftWords);
        if (numWords > shiftWords) {
          ShiftWordsRightByBitsSignExtend(ret, 0, numWords - shiftWords, shiftBits);
        }
        TwosComplement(ret, 0, (int)ret.length);
        retWordCount = ret.length;
      } else {
        if (shiftWords >= numWords) {
          return BigInteger.ZERO;
        }
        ret = new short[this.words.length];
        System.arraycopy(this.words, shiftWords, ret, 0, numWords - shiftWords);
        if (shiftBits != 0) {
          ShiftWordsRightByBits(ret, 0, numWords - shiftWords, shiftBits);
        }
        retWordCount = numWords - shiftWords;
      }
      while (retWordCount != 0 &&
             ret[retWordCount - 1] == 0) {
        --retWordCount;
      }
      if (retWordCount == 0) {
        return BigInteger.ZERO;
      }
      if (shiftWords > 2) {
        ret = ShortenArray(ret, retWordCount);
      }
      return new BigInteger(retWordCount, ret, this.negative);
    }

    /**
     * Converts a 64-bit signed integer to a big integer.
     * @param longerValue A 64-bit signed integer.
     * @return A BigInteger object with the same value as the 64-bit number.
     */
    public static BigInteger valueOf(long longerValue) {
      if (longerValue == 0) {
        return BigInteger.ZERO;
      }
      if (longerValue == 1) {
        return BigInteger.ONE;
      }
      short[] retreg;
      boolean retnegative;
      int retwordcount;
      {
        retnegative = longerValue < 0;
        retreg = new short[4];
        if (longerValue == Long.MIN_VALUE) {
          retreg[0] = 0;
          retreg[1] = 0;
          retreg[2] = 0;
          retreg[3] = (short)0x8000;
          retwordcount = 4;
        } else {
          long ut = longerValue;
          if (ut < 0) {
            ut = -ut;
          }
          retreg[0] = (short)(ut & 0xffff);
          ut >>= 16;
          retreg[1] = (short)(ut & 0xffff);
          ut >>= 16;
          retreg[2] = (short)(ut & 0xffff);
          ut >>= 16;
          retreg[3] = (short)(ut & 0xffff);
          // at this point, the word count can't
          // be 0 (the check for 0 was already done above)
          retwordcount = 4;
          while (retwordcount != 0 &&
                 retreg[retwordcount - 1] == 0) {
            --retwordcount;
          }
        }
      }
      return new BigInteger(retwordcount, retreg, retnegative);
    }

    /**
     * Converts this object's value to a 32-bit signed integer.
     * @return A 32-bit signed integer.
     * @throws ArithmeticException This object's value is too big to fit a
     * 32-bit signed integer.
     */
    public int intValueChecked() {
      int count = this.wordCount;
      if (count == 0) {
        return 0;
      }
      if (count > 2) {
        throw new ArithmeticException();
      }
      if (count == 2 && (this.words[1] & 0x8000) != 0) {
        if (this.negative && this.words[1] == ((short)0x8000) &&
            this.words[0] == 0) {
          return Integer.MIN_VALUE;
        }
        throw new ArithmeticException();
      }
      return this.intValueUnchecked();
    }

    /**
     * Converts this object's value to a 32-bit signed integer. If the value
     * can't fit in a 32-bit integer, returns the lower 32 bits of this object's
     * two's complement representation (in which case the return value
     * might have a different sign than this object's value).
     * @return A 32-bit signed integer.
     */
    public int intValueUnchecked() {
      int c = (int)this.wordCount;
      if (c == 0) {
        return 0;
      }
      int intRetValue = ((int)this.words[0]) & 0xffff;
      if (c > 1) {
        intRetValue |= (((int)this.words[1]) & 0xffff) << 16;
      }
      if (this.negative) {
        intRetValue = (intRetValue - 1);
        intRetValue = (~intRetValue);
      }
      return intRetValue;
    }

    /**
     * Converts this object's value to a 64-bit signed integer.
     * @return A 64-bit signed integer.
     * @throws ArithmeticException This object's value is too big to fit a
     * 64-bit signed integer.
     */
    public long longValueChecked() {
      int count = this.wordCount;
      if (count == 0) {
        return (long)0;
      }
      if (count > 4) {
        throw new ArithmeticException();
      }
      if (count == 4 && (this.words[3] & 0x8000) != 0) {
        if (this.negative && this.words[3] == ((short)0x8000) &&
            this.words[2] == 0 &&
            this.words[1] == 0 &&
            this.words[0] == 0) {
          return Long.MIN_VALUE;
        }
        throw new ArithmeticException();
      }
      return this.longValueUnchecked();
    }

    /**
     * Converts this object's value to a 64-bit signed integer. If the value
     * can't fit in a 64-bit integer, returns the lower 64 bits of this object's
     * two's complement representation (in which case the return value
     * might have a different sign than this object's value).
     * @return A 64-bit signed integer.
     */
    public long longValueUnchecked() {
      int c = (int)this.wordCount;
      if (c == 0) {
        return (long)0;
      }
      long ivv;
      int intRetValue = ((int)this.words[0]) & 0xffff;
      if (c > 1) {
        intRetValue |= (((int)this.words[1]) & 0xffff) << 16;
      }
      if (c > 2) {
        int intRetValue2 = ((int)this.words[2]) & 0xffff;
        if (c > 3) {
          intRetValue2 |= (((int)this.words[3]) & 0xffff) << 16;
        }
        if (this.negative) {
          if (intRetValue == 0) {
            intRetValue = (intRetValue - 1);
            intRetValue2 = (intRetValue2 - 1);
          } else {
            intRetValue = (intRetValue - 1);
          }
          intRetValue = (~intRetValue);
          intRetValue2 = (~intRetValue2);
        }
        ivv = ((long)intRetValue) & 0xFFFFFFFFL;
        ivv |= ((long)intRetValue2) << 32;
        return ivv;
      } else {
        ivv = ((long)intRetValue) & 0xFFFFFFFFL;
        if (this.negative) {
          ivv = -ivv;
        }
        return ivv;
      }
    }

    /**
     * Converts this object's value to a 32-bit signed integer. To make the
     * conversion intention clearer, use the <code>intValueChecked</code>
     * and <code>intValueUnchecked</code> methods instead.
     * @return A 32-bit signed integer.
     * @throws ArithmeticException This object's value is too big to fit a
     * 32-bit signed integer.
     */
    public int intValue() {
      return this.intValueChecked();
    }

    /**
     * Returns whether this object's value can fit in a 32-bit signed integer.
     * @return True if this object's value is MinValue or greater, and MaxValue
     * or less; otherwise, false.
     */
    public boolean canFitInInt() {
      int c = (int)this.wordCount;
      if (c > 2) {
        return false;
      }
      if (c == 2 && (this.words[1] & 0x8000) != 0) {
        return this.negative && this.words[1] == ((short)0x8000) &&
          this.words[0] == 0;
      }
      return true;
    }

    private boolean HasSmallValue() {
      int c = (int)this.wordCount;
      if (c > 4) {
        return false;
      }
      if (c == 4 && (this.words[3] & 0x8000) != 0) {
        return this.negative && this.words[3] == ((short)0x8000) &&
          this.words[2] == 0 &&
          this.words[1] == 0 &&
          this.words[0] == 0;
      }
      return true;
    }

    /**
     * Converts this object's value to a 64-bit signed integer. To make the
     * conversion intention clearer, use the <code>longValueChecked</code>
     * and <code>longValueUnchecked</code> methods instead.
     * @return A 64-bit signed integer.
     * @throws ArithmeticException This object's value is too big to fit a
     * 64-bit signed integer.
     */
    public long longValue() {
      return this.longValueChecked();
    }

    /**
     * Not documented yet.
     * @param power A BigInteger object. (2).
     * @return A BigInteger object.
     * @throws java.lang.NullPointerException The parameter {@code power}
     * is null.
     */
    public BigInteger PowBigIntVar(BigInteger power) {
      if (power == null) {
        throw new NullPointerException("power");
      }
      int sign = power.signum();
      if (sign < 0) {
        throw new IllegalArgumentException("sign (" + Long.toString((long)sign) + ") is less than " + "0");
      }
      BigInteger thisVar = this;
      if (sign == 0) {
        return BigInteger.ONE;
      }
      if (power.equals(BigInteger.ONE)) {
        return this;
      }
      if (power.wordCount == 1 && power.words[0] == 2) {
        return thisVar.multiply(thisVar);
      }
      if (power.wordCount == 1 && power.words[0] == 3) {
        return (thisVar.multiply(thisVar)).multiply(thisVar);
      }
      BigInteger r = BigInteger.ONE;
      while (power.signum()!=0) {
        if (power.testBit(0)) {
          r=r.multiply(thisVar);
        }
        power=power.shiftRight(1);
        if (power.signum()!=0) {
          thisVar=thisVar.multiply(thisVar);
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
        throw new IllegalArgumentException("powerSmall (" + Long.toString((long)powerSmall) + ") is less than " + "0");
      }
      BigInteger thisVar = this;
      if (powerSmall == 0) {
        // however 0 to the power of 0 is undefined
        return BigInteger.ONE;
      }
      if (powerSmall == 1) {
        return this;
      }
      if (powerSmall == 2) {
        return thisVar.multiply(thisVar);
      }
      if (powerSmall == 3) {
        return (thisVar.multiply(thisVar)).multiply(thisVar);
      }
      BigInteger r = BigInteger.ONE;
      while (powerSmall != 0) {
        if ((powerSmall & 1) != 0) {
          r=r.multiply(thisVar);
        }
        powerSmall >>= 1;
        if (powerSmall != 0) {
          thisVar=thisVar.multiply(thisVar);
        }
      }
      return r;
    }

    /**
     * Gets the value of this object with the sign reversed.
     * @return This object's value with the sign reversed.
     */
    public BigInteger negate() {
      return this.wordCount == 0 ? this : new BigInteger(this.wordCount, this.words, !this.negative);
    }

    /**
     * Returns the absolute value of this object's value.
     * @return This object's value with the sign removed.
     */
    public BigInteger abs() {
      return (this.wordCount == 0 || !this.negative) ? this : new BigInteger(this.wordCount, this.words, false);
    }

    private int ByteCount() {
      int wc = this.wordCount;
      if (wc == 0) {
        return 0;
      }
      short s = this.words[wc - 1];
      wc = (wc - 1) << 1;
      return (s == 0) ? wc : (((s >> 8) == 0) ? wc + 1 : wc + 2);
    }

    /**
     * Finds the minimum number of bits needed to represent this object&apos;s
     * absolute value.
     * @return The number of bits in this object's value. Returns 0 if this
     * object's value is 0, and returns 1 if the value is negative 1.
     */
    public int getUnsignedBitLength() {
      int wc = this.wordCount;
      if (wc != 0) {
        int numberValue = ((int)this.words[wc - 1]) & 0xffff;
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
      }
      return 0;
    }

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
      }
      return 0;
    }

    /**
     * Finds the minimum number of bits needed to represent this object&apos;s
     * value, except for its sign. If the value is negative, finds the number
     * of bits in a value equal to this object's absolute value minus 1.
     * @return The number of bits in this object's value. Returns 0 if this
     * object's value is 0 or negative 1.
     */
    public int bitLength() {
      int wc = this.wordCount;
      if (wc != 0) {
        if (this.negative) {
          return this.abs().subtract(BigInteger.ONE).bitLength();
        }
        int numberValue = ((int)this.words[wc - 1]) & 0xffff;
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
          return ((numberValue >> 15) == 0) ? wc - 1 : wc;
        }
      }
      return 0;
    }

    private static final String HexChars = "0123456789ABCDEF";

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
        ++count;
        value = -value;
      }
      while (value != 0) {
        char digit = HexChars.charAt((int)(value % 10));
        chars[count++] = digit;
        value /= 10;
      }
      if (neg) {
        ReverseChars(chars, 1, count - 1);
      } else {
        ReverseChars(chars, 0, count);
      }
      return new String(chars, 0, count);
    }

    private static int ApproxLogTenOfTwo(int bitlen) {
      int bitlenLow = bitlen & 0xffff;
      int bitlenHigh = (bitlen >> 16) & 0xffff;
      short resultLow = 0;
      short resultHigh = 0;
      {
        int p; short c; int d;
        p = bitlenLow * 0x84fb; d = ((int)p >> 16) & 0xffff; c = (short)d; d = ((int)d >> 16) & 0xffff;
        p = bitlenLow * 0x209a;
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = bitlenHigh * 0x84fb;
        p += ((int)c) & 0xffff;
        d += ((int)p >> 16) & 0xffff; c = (short)d; d = ((int)d >> 16) & 0xffff;
        p = bitlenLow * 0x9a;
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = bitlenHigh * 0x209a;
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = ((int)c) & 0xffff; c = (short)p; resultLow = c; c = (short)d; d = ((int)d >> 16) & 0xffff;
        p = bitlenHigh * 0x9a;
        p += ((int)c) & 0xffff;
        resultHigh = (short)p;
        int result = ((int)resultLow) & 0xffff;
        result |= (((int)resultHigh) & 0xffff) << 16;
        return (result & 0x7fffffff) >> 9;
      }
    }

    /**
     * Finds the number of decimal digits this number has.
     * @return The number of decimal digits. Returns 1 if this object' s value
     * is 0.
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
          return (value >= 1000000000000000000L) ? 19 : ((value >= 100000000000000000L) ? 18 : ((value >= 10000000000000000L) ? 17 : ((value >= 1000000000000000L) ? 16 : ((value >= 100000000000000L) ? 15 : ((value >= 10000000000000L) ? 14 : ((value >= 1000000000000L) ? 13 : ((value >= 100000000000L) ? 12 : ((value >= 10000000000L) ? 11 : ((value >= 1000000000L) ? 10 : 9)))))))));
        } else {
          int v2 = (int)value;
          return (v2 >= 100000000) ? 9 : ((v2 >= 10000000) ? 8 : ((v2 >= 1000000) ? 7 : ((v2 >= 100000) ? 6 : ((v2 >= 10000) ? 5 : ((v2 >= 1000) ? 4 : ((v2 >= 100) ? 3 : ((v2 >= 10) ? 2 : 1)))))));
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
      int currentCount = this.wordCount;
      int i = 0;
      while (currentCount != 0) {
        if (currentCount == 1 || (currentCount == 2 && tempReg[1] == 0)) {
          int rest = ((int)tempReg[0]) & 0xffff;
          if (rest >= 10000) {
            i += 5;
          } else if (rest >= 1000) {
            i += 4;
          } else if (rest >= 100) {
            i += 3;
          } else if (rest >= 10) {
            i += 2;
          } else {
            ++i;
          }
          break;
        }
        if (currentCount == 2 && tempReg[1] > 0 && tempReg[1] <= 0x7fff) {
          int rest = ((int)tempReg[0]) & 0xffff;
          rest |= (((int)tempReg[1]) & 0xffff) << 16;
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
            ++i;
          }
          break;
        } else {
          int wci = currentCount;
          short remainderShort = 0;
          int quo, rem;
          boolean firstdigit = false;
          short[] dividend = (tempReg == null) ? (this.words) : tempReg;
          // Divide by 10000
          while ((wci--) > 0) {
            int curValue = ((int)dividend[wci]) & 0xffff;
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
                System.arraycopy(this.words, 0, tempReg, 0, tempReg.length);
                // Use the calculated word count during division;
                // zeros that may have occurred in division
                // are not incorporated in the tempReg
                currentCount = wci + 1;
                tempReg[wci] = ((short)quo);
              }
            } else {
              tempReg[wci] = ((short)quo);
            }
            rem = currentDividend - (10000 * quo);
            remainderShort = ((short)rem);
          }
          // Recalculate word count
          while (currentCount != 0 && tempReg[currentCount - 1] == 0) {
            --currentCount;
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
      System.arraycopy(this.words, 0, tempReg, 0, tempReg.length);
      int numWordCount = tempReg.length;
      while (numWordCount != 0 && tempReg[numWordCount - 1] == 0) {
        --numWordCount;
      }
      int i = 0;
      char[] s = new char[(numWordCount << 4) + 1];
      while (numWordCount != 0) {
        if (numWordCount == 1 && tempReg[0] > 0 && tempReg[0] <= 0x7fff) {
          int rest = tempReg[0];
          while (rest != 0) {
            // accurate approximation to rest/10 up to 43698,
            // and rest can go up to 32767
            int newrest = (rest * 26215) >> 18;
            s[i++] = HexChars.charAt(rest - (newrest * 10));
            rest = newrest;
          }
          break;
        }
        if (numWordCount == 2 && tempReg[1] > 0 && tempReg[1] <= 0x7fff) {
          int rest = ((int)tempReg[0]) & 0xffff;
          rest |= (((int)tempReg[1]) & 0xffff) << 16;
          while (rest != 0) {
            int newrest = rest / 10;
            s[i++] = HexChars.charAt(rest - (newrest * 10));
            rest = newrest;
          }
          break;
        } else {
          int wci = numWordCount;
          short remainderShort = 0;
          int quo, rem;
          // Divide by 10000
          while ((wci--) > 0) {
            int currentDividend = ((int)((((int)tempReg[wci]) & 0xffff) |
                                                  ((int)remainderShort << 16)));
            quo = currentDividend / 10000;
            tempReg[wci] = ((short)quo);
            rem = currentDividend - (10000 * quo);
            remainderShort = ((short)rem);
          }
          int remainderSmall = remainderShort;
          // Recalculate word count
          while (numWordCount != 0 && tempReg[numWordCount - 1] == 0) {
            --numWordCount;
          }
          // accurate approximation to rest/10 up to 16388,
          // and rest can go up to 9999
          int newrest = (remainderSmall * 3277) >> 15;
          s[i++] = HexChars.charAt((int)(remainderSmall - (newrest * 10)));
          remainderSmall = newrest;
          newrest = (remainderSmall * 3277) >> 15;
          s[i++] = HexChars.charAt((int)(remainderSmall - (newrest * 10)));
          remainderSmall = newrest;
          newrest = (remainderSmall * 3277) >> 15;
          s[i++] = HexChars.charAt((int)(remainderSmall - (newrest * 10)));
          remainderSmall = newrest;
          s[i++] = HexChars.charAt(remainderSmall);
        }
      }
      ReverseChars(s, 0, i);
      if (this.negative) {
        StringBuilder sb = new StringBuilder(i + 1);
        sb.append('-');
        sb.append(s,0,(0)+(i));
        return sb.toString();
      }
      return new String(s, 0, i);
    }

    /**
     * Converts a string to an arbitrary-precision integer.
     * @param str A string containing only digits, except that it may start
     * with a minus sign.
     * @return A BigInteger object with the same value as given in the string.
     * @throws java.lang.NullPointerException The parameter {@code str}
     * is null.
     * @throws NumberFormatException The parameter {@code str} is in an invalid
     * format.
     */
    public static BigInteger fromString(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      return fromSubstring(str, 0, str.length());
    }

    private static final int MaxSafeInt = 214748363;

    /**
     * Converts a portion of a string to an arbitrary-precision integer.
     * @param str A string object.
     * @param index The index of the string that starts the string portion.
     * @param endIndex The index of the string that ends the string portion.
     * The length will be index + endIndex - 1.
     * @return A BigInteger object with the same value as given in the string
     * portion.
     * @throws java.lang.NullPointerException The parameter {@code str}
     * is null.
     * @throws java.lang.IllegalArgumentException The parameter {@code index}
     * is less than 0, {@code endIndex} is less than 0, or either is greater
     * than the string's length, or {@code endIndex} is less than {@code
     * index} .
     * @throws NumberFormatException The string portion is empty or in an invalid
     * format.
     */
    public static BigInteger fromSubstring(String str, int index, int endIndex) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index < 0) {
        throw new IllegalArgumentException("index (" + Long.toString((long)index) + ") is less than " + "0");
      }
      if (index > str.length()) {
        throw new IllegalArgumentException("index (" + Long.toString((long)index) + ") is more than " + Long.toString((long)str.length()));
      }
      if (endIndex < 0) {
        throw new IllegalArgumentException("endIndex (" + Long.toString((long)endIndex) + ") is less than " + "0");
      }
      if (endIndex > str.length()) {
        throw new IllegalArgumentException("endIndex (" + Long.toString((long)endIndex) + ") is more than " + Long.toString((long)str.length()));
      }
      if (endIndex < index) {
        throw new IllegalArgumentException("endIndex (" + Long.toString((long)endIndex) + ") is less than " + Long.toString((long)index));
      }
      if (index == endIndex) {
        throw new NumberFormatException("No digits");
      }
      boolean negative = false;
      if (str.charAt(0) == '-') {
        ++index;
        negative = true;
      }
      short[] bigint = new short[4];
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
            bigint[0] = ((short)(smallInt & 0xffff));
            bigint[1] = ((short)((smallInt >> 16) & 0xffff));
            haveSmallInt = false;
          }
          // Multiply by 10
          short carry = 0;
          int n = bigint.length;
          for (int j = 0; j < n; ++j) {
            int p;
            {
              p = (((int)bigint[j]) & 0xffff) * 10;
              p += ((int)carry) & 0xffff;
              bigint[j] = (short)p;
              carry = (short)(p >> 16);
            }
          }
          if (carry != 0) {
            bigint = GrowForCarry(bigint, carry);
          }
          // Add the parsed digit
          if (digit != 0) {
            int d = bigint[0] & 0xffff;
            if (d <= 65526) {
              bigint[0] = ((short)(d + digit));
            } else if (Increment(bigint, 0, bigint.length, (short)digit) != 0) {
              bigint = GrowForCarry(bigint, (short)1);
            }
          }
        }
      }
      if (!haveDigits) {
        throw new NumberFormatException("No digits");
      }
      if (haveSmallInt) {
        bigint[0] = ((short)(smallInt & 0xffff));
        bigint[1] = ((short)((smallInt >> 16) & 0xffff));
      }
      int count = CountWords(bigint, bigint.length);
      return (count == 0) ? BigInteger.ZERO : new BigInteger(count, bigint, negative);
    }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
    public int getLowestSetBit() {
      int retSetBit = 0;
      for (int i = 0; i < this.wordCount; ++i) {
        short c = this.words[i];
        if (c == (short)0) {
          retSetBit += 16;
        } else {
          return (((c << 15) & 0xffff) != 0) ? (retSetBit + 0) : ((((c << 14) & 0xffff) != 0) ? (retSetBit + 1) : ((((c << 13) & 0xffff) != 0) ? (retSetBit + 2) : ((((c << 12) & 0xffff) != 0) ? (retSetBit + 3) : ((((c << 11) & 0xffff) != 0) ? (retSetBit + 4) : ((((c << 10) & 0xffff) != 0) ? (retSetBit + 5) : ((((c << 9) & 0xffff) != 0) ? (retSetBit + 6) : ((((c << 8) & 0xffff) != 0) ? (retSetBit + 7) : ((((c << 7) & 0xffff) != 0) ? (retSetBit + 8) : ((((c << 6) & 0xffff) != 0) ? (retSetBit + 9) : ((((c << 5) & 0xffff) != 0) ? (retSetBit + 10) : ((((c << 4) & 0xffff) != 0) ? (retSetBit + 11) : ((((c << 3) & 0xffff) != 0) ? (retSetBit + 12) : ((((c << 2) & 0xffff) != 0) ? (retSetBit + 13) : ((((c << 1) & 0xffff) != 0) ? (retSetBit + 14) : (retSetBit + 15)))))))))))))));
        }
      }
      return 0;
    }

    /**
     * Returns the greatest common divisor of two integers. The greatest
     * common divisor (GCD) is also known as the greatest common factor (GCF).
     * @param bigintSecond A BigInteger object. (2).
     * @return A BigInteger object.
     * @throws java.lang.NullPointerException The parameter {@code bigintSecond}
     * is null.
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
          thisValue.equals(bigintSecond)) {
        return bigintSecond;
      }
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
          thisValue=thisValue.remainder(bigintSecond);
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
     * @throws java.lang.NullPointerException The parameter {@code pow}
     * is null.
     */
    public BigInteger ModPow(BigInteger pow, BigInteger mod) {
      if (pow == null) {
        throw new NullPointerException("pow");
      }
      if (pow.signum() < 0) {
        throw new IllegalArgumentException("pow (" + pow + ") is less than 0");
      }
      if (mod.signum() <= 0) {
        throw new IllegalArgumentException("mod (" + mod + ") is not greater than 0");
      }
      BigInteger r = BigInteger.ONE;
      BigInteger v = this;
      while (pow.signum()!=0) {
        if (pow.testBit(0)) {
          r = (r.multiply(v)).mod(mod);
        }
        pow=pow.shiftRight(1);
        if (pow.signum()!=0) {
          v = (v.multiply(v)).mod(mod);
        }
      }
      return r;
    }

    /**
     * Determines whether this object and another object are equal.
     * @param obj An arbitrary object.
     * @return True if the objects are equal; otherwise, false.
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
          if (this.words[i] != other.words[i]) {
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
        if (this.words != null) {
          for (int i = 0; i < this.wordCount; ++i) {
            hashCodeValue += 1000000013 * this.words[i];
          }
        }
      }
      return hashCodeValue;
    }

    /**
     * Adds this object and another object.
     * @param bigintAugend A BigInteger object.
     * @return The sum of the two objects.
     * @throws java.lang.NullPointerException The parameter {@code bigintAugend}
     * is null.
     */
    public BigInteger add(BigInteger bigintAugend) {
      if (bigintAugend == null) {
        throw new NullPointerException("bigintAugend");
      }
      if (this.wordCount == 0) {
        return bigintAugend;
      }
      if (bigintAugend.wordCount == 0) {
        return this;
      }
      short[] sumreg;
      if (bigintAugend.wordCount == 1 && this.wordCount == 1) {
        if (this.negative == bigintAugend.negative) {
          int intSum = (((int)this.words[0]) & 0xffff) + (((int)bigintAugend.words[0]) & 0xffff);
          sumreg = new short[2];
          sumreg[0] = ((short)intSum);
          sumreg[1] = ((short)(intSum >> 16));
          return new BigInteger(((intSum >> 16) == 0) ? 1 : 2, sumreg, this.negative);
        } else {
          int a = ((int)this.words[0]) & 0xffff;
          int b = ((int)bigintAugend.words[0]) & 0xffff;
          if (a == b) {
            return BigInteger.ZERO;
          }
          if (a > b) {
            a -= b;
            sumreg = new short[2];
            sumreg[0] = ((short)a);
            return new BigInteger(1, sumreg, this.negative);
          }
          b -= a;
          sumreg = new short[2];
          sumreg[0] = ((short)b);
          return new BigInteger(1, sumreg, !this.negative);
        }
      }
      if ((!this.negative) == (!bigintAugend.negative)) {
        sumreg = new short[(int)Math.max(this.words.length, bigintAugend.words.length)];
        // both nonnegative or both negative
        int carry;
        int addendCount = this.wordCount;
        int augendCount = bigintAugend.wordCount;
        int desiredLength = Math.max(addendCount, augendCount);
        if (addendCount == augendCount) {
          carry = AddOneByOne(sumreg, 0, this.words, 0, bigintAugend.words, 0, (int)addendCount);
        } else if (addendCount > augendCount) {
          // Addend is bigger
          carry = AddOneByOne(
            sumreg,
            0,
            this.words,
            0,
            bigintAugend.words,
            0,
            augendCount);
          System.arraycopy(
            this.words,
            augendCount,
            sumreg,
            augendCount,
            addendCount - augendCount);
          if (carry != 0) {
            carry = Increment(
              sumreg,
              augendCount,
              addendCount - augendCount,
              (short)carry);
          }
        } else {
          // Augend is bigger
          carry = AddOneByOne(
            sumreg,
            0,
            this.words,
            0,
            bigintAugend.words,
            0,
            (int)addendCount);
          System.arraycopy(
            bigintAugend.words,
            addendCount,
            sumreg,
            addendCount,
            augendCount - addendCount);
          if (carry != 0) {
            carry = Increment(
              sumreg,
              addendCount,
              (int)(augendCount - addendCount),
              (short)carry);
          }
        }
        boolean needShorten = true;
        if (carry != 0) {
          int nextIndex = desiredLength;
          int len = RoundupSize(nextIndex + 1);
          sumreg = CleanGrow(sumreg, len);
          sumreg[nextIndex] = (short)carry;
          needShorten = false;
        }
        int sumwordCount = CountWords(sumreg, sumreg.length);
        if (sumwordCount == 0) {
          return BigInteger.ZERO;
        }
        if (needShorten) {
          sumreg = ShortenArray(sumreg, sumwordCount);
        }
        return new BigInteger(sumwordCount, sumreg, this.negative);
      }
      BigInteger minuend = this;
      BigInteger subtrahend = bigintAugend;
      if (this.negative) {
        // this is negative, b is nonnegative
        minuend = bigintAugend;
        subtrahend = this;
      }
      // Do a subtraction
      int words1Size = minuend.wordCount;
      words1Size += words1Size & 1;
      int words2Size = subtrahend.wordCount;
      words2Size += words2Size & 1;
      boolean diffNeg = false;
      short[] diffReg = new short[(int)Math.max(minuend.words.length, subtrahend.words.length)];
      if (words1Size == words2Size) {
        if (Compare(minuend.words, 0, subtrahend.words, 0, (int)words1Size) >= 0) {
          // words1 is at least as high as words2
          Subtract(diffReg, 0, minuend.words, 0, subtrahend.words, 0, (int)words1Size);
        } else {
          // words1 is less than words2
          Subtract(diffReg, 0, subtrahend.words, 0, minuend.words, 0, (int)words1Size);
          diffNeg = true;  // difference will be negative
        }
      } else if (words1Size > words2Size) {
        // words1 is greater than words2
        short borrow = (short)Subtract(diffReg, 0, minuend.words, 0, subtrahend.words, 0, (int)words2Size);
        System.arraycopy(minuend.words, words2Size, diffReg, words2Size, words1Size - words2Size);
        Decrement(diffReg, words2Size, (int)(words1Size - words2Size), borrow);
      } else {
        // words1 is less than words2
        short borrow = (short)Subtract(diffReg, 0, subtrahend.words, 0, minuend.words, 0, (int)words1Size);
        System.arraycopy(subtrahend.words, words1Size, diffReg, words1Size, words2Size - words1Size);
        Decrement(diffReg, words1Size, (int)(words2Size - words1Size), borrow);
        diffNeg = true;
      }
      int count = CountWords(diffReg, diffReg.length);
      if (count == 0) {
        return BigInteger.ZERO;
      }
      diffReg = ShortenArray(diffReg, count);
      return new BigInteger(count, diffReg, diffNeg);
    }

    /**
     * Subtracts a BigInteger from this BigInteger.
     * @param subtrahend A BigInteger object.
     * @return The difference of the two objects.
     * @throws java.lang.NullPointerException The parameter {@code subtrahend}
     * is null.
     */
    public BigInteger subtract(BigInteger subtrahend) {
      if (subtrahend == null) {
        throw new NullPointerException("subtrahend");
      }
      return (this.wordCount == 0) ? subtrahend.negate() : ((subtrahend.wordCount == 0) ? this : this.add(subtrahend.negate()));
    }

    private static short[] ShortenArray(short[] reg, int wordCount) {
      if (reg.length > 32) {
        int newLength = RoundupSize(wordCount);
        if (newLength < reg.length &&
            (reg.length - newLength) >= 16) {
          // Reallocate the array if the rounded length
          // is much smaller than the current length
          short[] newreg = new short[newLength];
          System.arraycopy(reg, 0, newreg, 0, Math.min(newLength, reg.length));
          reg = newreg;
        }
      }
      return reg;
    }

    /**
     * Multiplies this instance by the value of a BigInteger object.
     * @param bigintMult A BigInteger object.
     * @return The product of the two objects.
     * @throws java.lang.NullPointerException The parameter {@code bigintMult}
     * is null.
     */
    public BigInteger multiply(BigInteger bigintMult) {
      if (bigintMult == null) {
        throw new NullPointerException("bigintMult");
      }
      if (this.wordCount == 0 || bigintMult.wordCount == 0) {
        return BigInteger.ZERO;
      }
      if (this.wordCount == 1 && this.words[0] == 1) {
        return this.negative ? bigintMult.negate() : bigintMult;
      }
      if (bigintMult.wordCount == 1 && bigintMult.words[0] == 1) {
        return bigintMult.negative ? this.negate() : this;
      }
      short[] productreg;
      int productwordCount;
      boolean needShorten = true;
      if (this.wordCount == 1) {
        int wc = bigintMult.wordCount;
        int regLength = RoundupSize(wc + 1);
        productreg = new short[regLength];
        productreg[wc] = LinearMultiply(productreg, 0, bigintMult.words, 0, this.words[0], wc);
        productwordCount = productreg.length;
        needShorten = false;
      } else if (bigintMult.wordCount == 1) {
        int wc = this.wordCount;
        int regLength = RoundupSize(wc + 1);
        productreg = new short[regLength];
        productreg[wc] = LinearMultiply(productreg, 0, this.words, 0, bigintMult.words[0], wc);
        productwordCount = productreg.length;
        needShorten = false;
      } else if (this.equals(bigintMult)) {
        int words1Size = RoundupSize(this.wordCount);
        productreg = new short[words1Size + words1Size];
        productwordCount = productreg.length;
        short[] workspace = new short[words1Size + words1Size];
        RecursiveSquare(
          productreg,
          0,
          workspace,
          0,
          this.words,
          0,
          words1Size);
      } else if (this.wordCount <= 10 && bigintMult.wordCount <= 10) {
        int wc = this.wordCount + bigintMult.wordCount;
        wc = RoundupSize(wc);
        productreg = new short[wc];
        productwordCount = productreg.length;
        SchoolbookMultiply(
          productreg,
          0,
          this.words,
          0,
          this.wordCount,
          bigintMult.words,
          0,
          bigintMult.wordCount);
        needShorten = false;
      } else {
        int words1Size = this.wordCount;
        int words2Size = bigintMult.wordCount;
        words1Size = RoundupSize(words1Size);
        words2Size = RoundupSize(words2Size);
        productreg = new short[RoundupSize(words1Size + words2Size)];
        short[] workspace = new short[words1Size + words2Size];
        productwordCount = productreg.length;
        AsymmetricMultiply(
          productreg,
          0,
          workspace,
          0,
          this.words,
          0,
          words1Size,
          bigintMult.words,
          0,
          words2Size);
      }
      // Recalculate word count
      while (productwordCount != 0 && productreg[productwordCount - 1] == 0) {
        --productwordCount;
      }
      if (needShorten) {
        productreg = ShortenArray(productreg, productwordCount);
      }
      return new BigInteger(productwordCount, productreg, this.negative ^ bigintMult.negative);
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
      int idivisor = ((int)divisorSmall) & 0xffff;
      int quo, rem;
      while ((i--) > 0) {
        int currentDividend = ((int)((((int)dividendReg[i]) & 0xffff) |
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

    private static short FastDivideAndRemainder(
      short[] quotientReg,
      int quotientStart,
      short[] dividendReg,
      int dividendStart,
      int count,
      short divisorSmall) {
      int i = count;
      short remainderShort = 0;
      int idivisor = ((int)divisorSmall) & 0xffff;
      int quo, rem;
      while ((i--) > 0) {
        int currentDividend = ((int)((((int)dividendReg[dividendStart + i]) & 0xffff) |
                                              ((int)remainderShort << 16)));
        if ((currentDividend >> 31) == 0) {
          quo = currentDividend / idivisor;
          quotientReg[quotientStart + i] = ((short)quo);
          rem = currentDividend - (idivisor * quo);
          remainderShort = ((short)rem);
        } else {
          quotientReg[quotientStart + i] = DivideUnsigned(currentDividend, divisorSmall);
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
     * @throws java.lang.NullPointerException The parameter {@code bigintDivisor}
     * is null.
     * @throws ArithmeticException Attempted to divide by zero.
     */
    public BigInteger divide(BigInteger bigintDivisor) {
      if (bigintDivisor == null) {
        throw new NullPointerException("bigintDivisor");
      }
      int words1Size = this.wordCount;
      int words2Size = bigintDivisor.wordCount;
      // ---- Special cases
      if (words2Size == 0) {
        throw new ArithmeticException();
      }
      if (words1Size < words2Size) {
        // dividend is less than divisor (includes case
        // where dividend is 0)
        return BigInteger.ZERO;
      }
      if (words1Size <= 2 && words2Size <= 2 && this.canFitInInt() && bigintDivisor.canFitInInt()) {
        int valueASmall = this.intValue();
        int valueBSmall = bigintDivisor.intValue();
        if (valueASmall != Integer.MIN_VALUE || valueBSmall != -1) {
          int result = valueASmall / valueBSmall;
          return BigInteger.valueOf(result);
        }
      }
      short[] quotReg;
      int quotwordCount;
      if (words2Size == 1) {
        // divisor is small, use a fast path
        quotReg = new short[this.words.length];
        quotwordCount = this.wordCount;
        FastDivide(quotReg, this.words, words1Size, bigintDivisor.words[0]);
        while (quotwordCount != 0 &&
               quotReg[quotwordCount - 1] == 0) {
          --quotwordCount;
        }
        return (quotwordCount != 0) ? (new BigInteger(quotwordCount, quotReg, this.negative ^ bigintDivisor.negative)) : BigInteger.ZERO;
      }
      // ---- General case
      words1Size += words1Size & 1;
      words2Size += words2Size & 1;
      quotReg = new short[RoundupSize((int)(words1Size - words2Size + 2))];
      short[] tempbuf = new short[words1Size + (3 * (words2Size + 2))];
      Divide(
        null,
        0,
        quotReg,
        0,
        tempbuf,
        0,
        this.words,
        0,
        words1Size,
        bigintDivisor.words,
        0,
        words2Size);
      quotwordCount = CountWords(quotReg, quotReg.length);
      quotReg = ShortenArray(quotReg, quotwordCount);
      return (quotwordCount != 0) ? (new BigInteger(quotwordCount, quotReg, this.negative ^ bigintDivisor.negative)) : BigInteger.ZERO;
    }

    /**
     * Divides this object by another big integer and returns the quotient
     * and remainder.
     * @param divisor The divisor.
     * @return An array with two big integers: the first is the quotient,
     * and the second is the remainder.
     * @throws java.lang.NullPointerException The parameter divisor is
     * null.
     * @throws ArithmeticException The parameter divisor is 0.
     * @throws ArithmeticException Attempted to divide by zero.
     */
    public BigInteger[] divideAndRemainder(BigInteger divisor) {
      if (divisor == null) {
        throw new NullPointerException("divisor");
      }
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
        short[] quotient = new short[this.words.length];
        int smallRemainder = ((int)FastDivideAndRemainder(
          quotient,
          0,
          this.words,
          0,
          words1Size,
          divisor.words[0])) & 0xffff;
        int count = this.wordCount;
        while (count != 0 &&
               quotient[count - 1] == 0) {
          --count;
        }
        if (count == 0) {
          return new BigInteger[] { BigInteger.ZERO, this };
        }
        quotient = ShortenArray(quotient, count);
        BigInteger bigquo = new BigInteger(count, quotient, this.negative ^ divisor.negative);
        if (this.negative) {
          smallRemainder = -smallRemainder;
        }
        return new BigInteger[] {bigquo, BigInteger.valueOf(smallRemainder) };
      }
      if (this.wordCount == 2 && divisor.wordCount == 2 &&
          (this.words[1] >> 15) != 0 &&
          (divisor.words[1] >> 15) != 0) {
        int a = ((int)this.words[0]) & 0xffff;
        int b = ((int)divisor.words[0]) & 0xffff;
        {
          a |= (((int)this.words[1]) & 0xffff) << 16;
          b |= (((int)divisor.words[1]) & 0xffff) << 16;
          int quo = a / b;
          if (this.negative) {
            quo = -quo;
          }
          int rem = a - (b * quo);
          BigInteger[] quotAndRem = new BigInteger[2];
          quotAndRem[0] = BigInteger.valueOf(quo);
          quotAndRem[1] = BigInteger.valueOf(rem);
          return quotAndRem;
        }
      }
      words1Size += words1Size & 1;
      words2Size += words2Size & 1;
      short[] bigRemainderreg = new short[RoundupSize((int)words2Size)];
      short[] quotientreg = new short[RoundupSize((int)(words1Size - words2Size + 2))];
      short[] tempbuf = new short[words1Size + (3 * (words2Size + 2))];
      Divide(
        bigRemainderreg,
        0,
        quotientreg,
        0,
        tempbuf,
        0,
        this.words,
        0,
        words1Size,
        divisor.words,
        0,
        words2Size);
      int remCount = CountWords(bigRemainderreg, bigRemainderreg.length);
      int quoCount = CountWords(quotientreg, quotientreg.length);
      bigRemainderreg = ShortenArray(bigRemainderreg, remCount);
      quotientreg = ShortenArray(quotientreg, quoCount);
      BigInteger bigrem = (remCount == 0) ? BigInteger.ZERO : new BigInteger(remCount, bigRemainderreg, this.negative);
      BigInteger bigquo2 = (quoCount == 0) ? BigInteger.ZERO : new BigInteger(quoCount, quotientreg, this.negative ^ divisor.negative);
      return new BigInteger[] {bigquo2, bigrem };
    }

    /**
     * Finds the modulus remainder that results when this instance is divided
     * by the value of a BigInteger object. The modulus remainder is the same
     * as the normal remainder if the normal remainder is positive, and equals
     * divisor plus normal remainder if the normal remainder is negative.
     * @param divisor A divisor greater than 0 (the modulus).
     * @return A BigInteger object.
     * @throws ArithmeticException The parameter {@code divisor} is negative.
     * @throws java.lang.NullPointerException The parameter {@code divisor}
     * is null.
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
        rem = divisor.add(rem);
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
     * @throws java.lang.NullPointerException The parameter {@code divisor}
     * is null.
     * @throws ArithmeticException Attempted to divide by zero.
     */
    public BigInteger remainder(BigInteger divisor) {
      if (divisor == null) {
        throw new NullPointerException("divisor");
      }
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
        short shortRemainder = FastRemainder(this.words, this.wordCount, divisor.words[0]);
        int smallRemainder = ((int)shortRemainder) & 0xffff;
        if (this.negative) {
          smallRemainder = -smallRemainder;
        }
        return BigInteger.valueOf(smallRemainder);
      }
      if (this.PositiveCompare(divisor) < 0) {
        return this;
      }
      words1Size += words1Size & 1;
      words2Size += words2Size & 1;
      short[] remainderReg = new short[RoundupSize((int)words2Size)];
      short[] tempbuf = new short[words1Size + (3 * (words2Size + 2))];
      Divide(
        remainderReg,
        0,
        null,
        0,
        tempbuf,
        0,
        this.words,
        0,
        words1Size,
        divisor.words,
        0,
        words2Size);
      int count = CountWords(remainderReg, remainderReg.length);
      if (count == 0) {
        return BigInteger.ZERO;
      }
      remainderReg = ShortenArray(remainderReg, count);
      return new BigInteger(count, remainderReg, this.negative);
    }

    private int PositiveCompare(BigInteger t) {
      int size = this.wordCount, tempSize = t.wordCount;
      return (size == tempSize) ? Compare(this.words, 0, t.words, 0, (int)size) : (size > tempSize ? 1 : -1);
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
        if (size == 1 && this.words[0] == other.words[0]) {
          return 0;
        } else {
          short[] words1 = this.words;
          short[] words2 = other.words;
          while ((size--) != 0) {
            int an = ((int)words1[size]) & 0xffff;
            int bn = ((int)words2[size]) & 0xffff;
            if (an > bn) {
              return (sa > 0) ? 1 : -1;
            }
            if (an < bn) {
              return (sa > 0) ? -1 : 1;
            }
          }
          return 0;
        }
      }
      return ((size > tempSize) ^ (sa <= 0)) ? 1 : -1;
    }

    /**
     * Gets the sign of this object's value.
     * @return 0 if this value is zero; -1 if this value is negative, or 1 if
     * this value is positive.
     */
    public int signum() {
        return (this.wordCount == 0) ? 0 : (this.negative ? -1 : 1);
      }

    /**
     * Gets a value indicating whether this value is 0.
     * @return True if this value is 0; otherwise, false.
     */
    public boolean isZero() {
        return this.wordCount == 0;
      }

    /**
     * Finds the square root of this instance&apos;s value, rounded down.
     * @return The square root of this object's value. Returns 0 if this value
     * is 0 or less.
     */
    public BigInteger sqrt() {
      BigInteger[] srrem = this.sqrtWithRemainder();
      return srrem[0];
    }

    /**
     * Calculates the square root and the remainder.
     * @return An array of two big integers: the first integer is the square
     * root, and the second is the difference between this value and the square
     * of the first integer. Returns two zeros if this value is 0 or less, or
     * one and zero if this value equals 1.
     */
    public BigInteger[] sqrtWithRemainder() {
      if (this.signum() <= 0) {
        return new BigInteger[] { BigInteger.ZERO, BigInteger.ZERO };
      }
      if (this.equals(BigInteger.ONE)) {
        return new BigInteger[] { BigInteger.ONE, BigInteger.ZERO };
      }
      BigInteger bigintX;
      BigInteger bigintY;
      BigInteger thisValue = this;
      int powerBits = (thisValue.getUnsignedBitLength() + 1) / 2;
      if (thisValue.canFitInInt()) {
        int smallValue = thisValue.intValue();
        // No need to check for zero; already done above
        int smallintX = 0;
        int smallintY = 1 << powerBits;
        do {
          smallintX = smallintY;
          smallintY = smallValue / smallintX;
          smallintY += smallintX;
          smallintY >>= 1;
        } while (smallintY < smallintX);
        smallintY = smallintX * smallintX;
        smallintY = smallValue - smallintY;
        return new BigInteger[] { BigInteger.valueOf(smallintX), BigInteger.valueOf(smallintY)
        };
      }
      bigintX = BigInteger.ZERO;
      bigintY = BigInteger.ONE.shiftLeft(powerBits);
      do {
        bigintX = bigintY;
        bigintY = thisValue.divide(bigintX);
        bigintY=bigintY.add(bigintX);
        bigintY=bigintY.shiftRight(1);
      } while (bigintY != null && bigintY.compareTo(bigintX) < 0);
      bigintY = bigintX.multiply(bigintX);
      bigintY = thisValue.subtract(bigintY);
      return new BigInteger[] {bigintX, bigintY
      };
    }

    /**
     * Gets a value indicating whether this value is even.
     * @return True if this value is even; otherwise, false.
     */
    public boolean isEven() {
        return !this.GetUnsignedBit(0);
      }

    /**
     * BigInteger object for the number zero.
     */

    public static final BigInteger ZERO = new BigInteger(0, new short[] { 0, 0 }, false);

    /**
     * BigInteger object for the number one.
     */

    public static final BigInteger ONE = new BigInteger(1, new short[] { 1, 0 }, false);

    /**
     * BigInteger object for the number ten.
     */

    public static final BigInteger TEN = BigInteger.valueOf(10);
  }
