/*
Written in 2013 by Peter O.

Parts of the code were adapted by Peter O. from
the public-domain code from the library
CryptoPP by Wei Dai.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO {
    /// <summary>An arbitrary-precision integer.
    /// <para>Instances of this class are immutable, so they are inherently
    /// safe for use by multiple threads. Multiple instances of this object
    /// with the same value are interchangeable, so they should not be
    /// compared using the "==" operator (which only checks if each side of
    /// the operator is the same instance).</para></summary>
  public sealed partial class BigInteger : IComparable<BigInteger>,
  IEquatable<BigInteger> {
    private static int CountWords(short[] array, int n) {
      while (n != 0 && array[n - 1] == 0) {
        --n;
      }
      return (int)n;
    }

    private static short ShiftWordsLeftByBits(
      short[] r,
      int rstart,
      int n,
      int shiftBits) {
      #if DEBUG
      if (shiftBits >= 16) {
        throw new ArgumentException("doesn't satisfy shiftBits<16");
      }
      #endif

      unchecked {
        short u, carry = 0;
        if (shiftBits != 0) {
          for (var i = 0; i < n; ++i) {
            u = r[rstart + i];
            r[rstart + i] = (short)((int)(u << (int)shiftBits) | (((int)carry) &
                    0xffff));
            carry = (short)((((int)u) & 0xffff) >> (int)(16 - shiftBits));
          }
        }
        return carry;
      }
    }

    private static short ShiftWordsRightByBits(
      short[] r,
      int rstart,
      int n,
      int shiftBits) {
      short u, carry = 0;
      unchecked {
        if (shiftBits != 0) {
          for (int i = n; i > 0; --i) {
            u = r[rstart + i - 1];
            r[rstart + i - 1] = (short)((((((int)u) & 0xffff) >>
                    (int)shiftBits) & 0xffff) | (((int)carry) &
                    0xffff));
            carry = (short)((((int)u) & 0xffff) << (int)(16 - shiftBits));
          }
        }
        return carry;
      }
    }

    private static short ShiftWordsRightByBitsSignExtend(
      short[] r,
      int rstart,
      int n,
      int shiftBits) {
      unchecked {
        short u, carry = (short)((int)0xffff << (int)(16 - shiftBits));
        if (shiftBits != 0) {
          for (int i = n; i > 0; --i) {
            u = r[rstart + i - 1];
            r[rstart + i - 1] = (short)(((((int)u) & 0xffff) >>
                    (int)shiftBits) | (((int)carry) & 0xffff));
            carry = (short)((((int)u) & 0xffff) << (int)(16 - shiftBits));
          }
        }
        return carry;
      }
    }

    private static void ShiftWordsLeftByWords(
      short[] r,
      int rstart,
      int n,
      int shiftWords) {
      shiftWords = Math.Min(shiftWords, n);
      if (shiftWords != 0) {
        for (int i = n - 1; i >= shiftWords; --i) {
          r[rstart + i] = r[rstart + i - shiftWords];
        }
        Array.Clear((short[])r, rstart, shiftWords);
      }
    }

    private static void ShiftWordsRightByWordsSignExtend(
      short[] r,
      int rstart,
      int n,
      int shiftWords) {
      shiftWords = Math.Min(shiftWords, n);
      if (shiftWords != 0) {
        for (var i = 0; i + shiftWords < n; ++i) {
          r[rstart + i] = r[rstart + i + shiftWords];
        }
        rstart += n - shiftWords;
        // Sign extend
        for (var i = 0; i < shiftWords; ++i) {
          r[rstart + i] = unchecked((short)0xffff);
        }
      }
    }

    private static int Compare(
      short[] words1,
      int astart,
      short[] words2,
      int bstart,
      int n) {
      while (unchecked(n--) != 0) {
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
    private static int CompareUnevenSize(short[] words1,
      int astart, int acount, short[] words2, int bstart,
      int bcount) {
      int n = acount;
      if (acount > bcount) {
        while (unchecked(acount--) != bcount) {
          if (words1[astart + acount] != 0) {
            return 1;
          }
        }
        n = bcount;
      } else if (bcount > acount) {
        while (unchecked(bcount--) != acount) {
          if (words1[astart + acount] != 0) {
            return -1;
          }
        }
        n = acount;
      }
      while (unchecked(n--) != 0) {
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

    private static int CompareWithOneBiggerWords1(
      short[] words1,
      int astart,
      short[] words2,
      int bstart,
      int words1Count) {
      // NOTE: Assumes that words2's count is 1 less
      if (words1[astart + words1Count - 1] != 0) {
        return 1;
      }
      int w1c = words1Count;
      --w1c;
      while (unchecked(w1c--) != 0) {
        int an = ((int)words1[astart + w1c]) & 0xffff;
        int bn = ((int)words2[bstart + w1c]) & 0xffff;
        if (an > bn) {
          return 1;
        }
        if (an < bn) {
          return -1;
        }
      }
      return 0;
    }

    private static int Increment(
      short[] words1,
      int words1Start,
      int n,
      short words2) {
      unchecked {
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

    private static int Decrement(
      short[] words1,
      int words1Start,
      int n,
      short words2) {
      unchecked {
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
      for (var i = 0; i < n; ++i) {
        words1[words1Start + i] = unchecked((short)(~words1[words1Start + i]));
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
      unchecked {
        int u;
        u = 0;
        for (var i = 0; i < n; i += 2) {
          u = (((int)words1[astart + i]) & 0xffff) + (((int)words2[bstart +
                    i]) & 0xffff) + (short)(u >> 16);
          c[cstart + i] = (short)u;
          u = (((int)words1[astart + i + 1]) & 0xffff) +
            (((int)words2[bstart + i + 1]) & 0xffff) + (short)(u >> 16);
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
      unchecked {
        int u;
        u = 0;
        for (var i = 0; i < n; i += 1) {
          u = (((int)words1[astart + i]) & 0xffff) + (((int)words2[bstart +
                    i]) & 0xffff) + (short)(u >> 16);
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
      unchecked {
        int u;
        u = 0;
        int cm1 = words1Count - 1;
        for (var i = 0; i < cm1; i += 1) {
          u = (((int)words1[astart]) & 0xffff) - (((int)words2[bstart]) &
                    0xffff) - (int)((u >> 31) & 1);
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
      unchecked {
        int u;
        u = 0;
        int cm1 = words2Count - 1;
        for (var i = 0; i < cm1; i += 1) {
          u = (((int)words1[astart]) & 0xffff) - (((int)words2[bstart]) &
                    0xffff) - (int)((u >> 31) & 1);
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
      #if DEBUG
      if (acount < bcount) {
        throw new ArgumentException("acount (" + acount + ") is less than " +
                    bcount);
      }
      #endif
      unchecked {
        int u;
        u = 0;
        for (var i = 0; i < bcount; i += 1) {
          u = (((int)wordsBigger[astart + i]) & 0xffff) +
            (((int)wordsSmaller[bstart + i]) & 0xffff) + (short)(u >> 16);
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
      unchecked {
        int u;
        u = 0;
        for (var i = 0; i < n; i += 2) {
          u = (((int)words1[astart]) & 0xffff) - (((int)words2[bstart]) &
                    0xffff) - (int)((u >> 31) & 1);
          c[cstart++] = (short)u;
          ++astart;
          ++bstart;
          u = (((int)words1[astart]) & 0xffff) - (((int)words2[bstart]) &
                    0xffff) - (int)((u >> 31) & 1);
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
      unchecked {
        int u;
        u = 0;
        for (var i = 0; i < n; i += 1) {
          u = (((int)words1[astart]) & 0xffff) - (((int)words2[bstart]) &
                    0xffff) - (int)((u >> 31) & 1);
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
      unchecked {
        short carry = 0;
        int bint = ((int)words2) & 0xffff;
        for (var i = 0; i < n; ++i) {
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
      unchecked {
        short carry = 0;
        int bint = ((int)words2) & 0xffff;
        for (var i = 0; i < n; ++i) {
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
    #region Baseline Square

    private static void BaselineSquare2(
      short[] result,
      int rstart,
      short[] words1,
      int astart) {
      unchecked {
        int p; short c; int d; int e;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart]) &
                    0xffff); result[rstart] = (short)p; e = ((int)p >>
                    16) & 0xffff;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 1]) &
                    0xffff); c = (short)p; d = ((int)p >> 16) &
          0xffff;
        d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<=
          1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 1] = c;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart +
                    1]) & 0xffff);
        p += e; result[rstart + 2] = (short)p; result[rstart + 3] = (short)(p >>
                    16);
      }
    }

    private static void BaselineSquare4(
      short[] result,
      int rstart,
      short[] words1,
      int astart) {
      unchecked {
        int p; short c; int d; int e;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart]) &
                    0xffff); result[rstart] = (short)p; e = ((int)p >>
                    16) & 0xffff;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 1]) &
                    0xffff); c = (short)p; d = ((int)p >> 16) &
          0xffff;
        d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<=
          1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 1] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 2]) &
                    0xffff); c = (short)p; d = ((int)p >> 16) &
          0xffff;
        d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<=
          1;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart +
                    1]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 2] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 3]) &
                    0xffff); c = (short)p; d = ((int)p >> 16) &
          0xffff;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart +
                    2]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) &
                    1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 3] = c;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart +
                    3]) & 0xffff); c = (short)p; d = ((int)p >>
                    16) & 0xffff;
        d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart +
                    2]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 4] = c;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart +
                    3]) & 0xffff); c = (short)p; d = ((int)p >>
                    16) & 0xffff;
        d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + (2 * 4) - 3] = c;
        p = (((int)words1[astart + 3]) & 0xffff) * (((int)words1[astart +
                    3]) & 0xffff);
        p += e; result[rstart + 6] = (short)p; result[rstart + 7] = (short)(p >>
                    16);
      }
    }

    private static void BaselineSquare8(
      short[] result,
      int rstart,
      short[] words1,
      int astart) {
      unchecked {
        int p; short c; int d; int e;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart]) &
                    0xffff); result[rstart] = (short)p; e = ((int)p >>
                    16) & 0xffff;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 1]) &
                    0xffff); c = (short)p; d = ((int)p >> 16) &
          0xffff;
        d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<=
          1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 1] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 2]) &
                    0xffff); c = (short)p; d = ((int)p >> 16) &
          0xffff;
        d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<=
          1;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart +
                    1]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 2] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 3]) &
                    0xffff); c = (short)p; d = ((int)p >> 16) &
          0xffff;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart +
                    2]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) &
                    1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 3] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 4]) &
                    0xffff); c = (short)p; d = ((int)p >> 16) &
          0xffff;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart +
                    3]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) &
                    1)); c <<= 1;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart +
                    2]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 4] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 5]) &
                    0xffff); c = (short)p; d = ((int)p >> 16) &
          0xffff;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart +
                    4]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart +
                    3]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) &
                    1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 5] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 6]) &
                    0xffff); c = (short)p; d = ((int)p >> 16) &
          0xffff;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart +
                    5]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart +
                    4]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) &
                    1)); c <<= 1;
        p = (((int)words1[astart + 3]) & 0xffff) * (((int)words1[astart +
                    3]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 6] = c;
        p = (((int)words1[astart]) & 0xffff) * (((int)words1[astart + 7]) &
                    0xffff); c = (short)p; d = ((int)p >> 16) &
          0xffff;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart +
                    6]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart +
                    5]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 3]) & 0xffff) * (((int)words1[astart +
                    4]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) &
                    1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 7] = c;
        p = (((int)words1[astart + 1]) & 0xffff) * (((int)words1[astart +
                    7]) & 0xffff); c = (short)p; d = ((int)p >>
                    16) & 0xffff;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart +
                    6]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 3]) & 0xffff) * (((int)words1[astart +
                    5]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) &
                    1)); c <<= 1;
        p = (((int)words1[astart + 4]) & 0xffff) * (((int)words1[astart +
                    4]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 8] = c;
        p = (((int)words1[astart + 2]) & 0xffff) * (((int)words1[astart +
                    7]) & 0xffff); c = (short)p; d = ((int)p >>
                    16) & 0xffff;
        p = (((int)words1[astart + 3]) & 0xffff) * (((int)words1[astart +
                    6]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = (((int)words1[astart + 4]) & 0xffff) * (((int)words1[astart +
                    5]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) &
                    1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 9] = c;
        p = (((int)words1[astart + 3]) & 0xffff) * (((int)words1[astart +
                    7]) & 0xffff); c = (short)p; d = ((int)p >>
                    16) & 0xffff;
        p = (((int)words1[astart + 4]) & 0xffff) * (((int)words1[astart +
                    6]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) &
                    1)); c <<= 1;
        p = (((int)words1[astart + 5]) & 0xffff) * (((int)words1[astart +
                    5]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 10] = c;
        p = (((int)words1[astart + 4]) & 0xffff) * (((int)words1[astart +
                    7]) & 0xffff); c = (short)p; d = ((int)p >>
                    16) & 0xffff;
        p = (((int)words1[astart + 5]) & 0xffff) * (((int)words1[astart +
                    6]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; d = (int)((d << 1) + (((int)c >> 15) &
                    1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 11] = c;
        p = (((int)words1[astart + 5]) & 0xffff) * (((int)words1[astart +
                    7]) & 0xffff); c = (short)p; d = ((int)p >>
                    16) & 0xffff;
        d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        p = (((int)words1[astart + 6]) & 0xffff) * (((int)words1[astart +
                    6]) & 0xffff);
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 12] = c;
        p = (((int)words1[astart + 6]) & 0xffff) * (((int)words1[astart +
                    7]) & 0xffff); c = (short)p; d = ((int)p >>
                    16) & 0xffff;
        d = (int)((d << 1) + (((int)c >> 15) & 1)); c <<= 1;
        e += ((int)c) & 0xffff; c = (short)e; e = d + (((int)e >> 16) &
                    0xffff); result[rstart + 13] = c;
        p = (((int)words1[astart + 7]) & 0xffff) * (((int)words1[astart +
                    7]) & 0xffff);
        p += e; result[rstart + 14] = (short)p; result[rstart + 15] =
          (short)(p >> 16);
      }
    }
    #endregion
    //---------------------
    // Baseline multiply
    //---------------------
    #region Baseline Multiply

    private static void BaselineMultiply2(
      short[] result,
      int rstart,
      short[] words1,
      int astart,
      short[] words2,
      int bstart) {
      unchecked {
        int p; short c; int d;
        int a0 = ((int)words1[astart]) & 0xffff;
        int a1 = ((int)words1[astart + 1]) & 0xffff;
        int b0 = ((int)words2[bstart]) & 0xffff;
        int b1 = ((int)words2[bstart + 1]) & 0xffff;
        p = a0 * b0; c = (short)p; d = ((int)p >> 16) & 0xffff;
        result[rstart] = c; c = (short)d; d = ((int)d >> 16) & 0xffff;
        p = a0 * b1;
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff;
        p = a1 * b0;
        p += ((int)c) & 0xffff; c = (short)p;
        d += ((int)p >> 16) & 0xffff; result[rstart + 1] = c;
        p = a1 * b1;
        p += d; result[rstart + 2] = (short)p; result[rstart + 3] = (short)(p >>
                    16);
      }
    }

    private const int ShortMask = 0xffff;

    private static void BaselineMultiply4(
      short[] result,
      int rstart,
      short[] words1,
      int astart,
      short[] words2,
      int bstart) {
      unchecked {
        const int SMask = ShortMask;
        int p; short c; int d;
        int a0 = ((int)words1[astart]) & SMask;
        int b0 = ((int)words2[bstart]) & SMask;
        p = a0 * b0; c = (short)p; d = ((int)p >> 16) & SMask;
        result[rstart] = c; c = (short)d; d = ((int)d >> 16) & SMask;
        p = a0 * (((int)words2[bstart + 1]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 1]) & SMask) * b0;
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 1] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = a0 * (((int)words2[bstart + 2]) & SMask);

        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 1]) & SMask) * (((int)words2[bstart +
                    1]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 2]) & SMask) * b0;
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 2] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = a0 * (((int)words2[bstart + 3]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 1]) & SMask) * (((int)words2[bstart +
                    2]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;

        p = (((int)words1[astart + 2]) & SMask) * (((int)words2[bstart +
                    1]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 3]) & SMask) * b0;
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 3] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = (((int)words1[astart + 1]) & SMask) * (((int)words2[bstart +
                    3]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 2]) & SMask) * (((int)words2[bstart +
                    2]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 3]) & SMask) * (((int)words2[bstart +
                    1]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 4] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = (((int)words1[astart + 2]) & SMask) * (((int)words2[bstart +
                    3]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 3]) & SMask) * (((int)words2[bstart +
                    2]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 5] = c;
        p = (((int)words1[astart + 3]) & SMask) * (((int)words2[bstart +
                    3]) & SMask);
        p += d; result[rstart + 6] = (short)p; result[rstart + 7] = (short)(p >>
                    16);
      }
    }

    private static void BaselineMultiply8(
      short[] result,
      int rstart,
      short[] words1,
      int astart,
      short[] words2,
      int bstart) {
      unchecked {
        int p; short c; int d;
        const int SMask = ShortMask;
        p = (((int)words1[astart]) & SMask) * (((int)words2[bstart]) &
                    SMask); c = (short)p; d = ((int)p >> 16) &
          SMask;
        result[rstart] = c; c = (short)d; d = ((int)d >> 16) & SMask;
        p = (((int)words1[astart]) & SMask) * (((int)words2[bstart + 1]) &
                    SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 1]) & SMask) * (((int)words2[bstart]) &
                    SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 1] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = (((int)words1[astart]) & SMask) * (((int)words2[bstart + 2]) &
                    SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 1]) & SMask) * (((int)words2[bstart +
                    1]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 2]) & SMask) * (((int)words2[bstart]) &
                    SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 2] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = (((int)words1[astart]) & SMask) * (((int)words2[bstart + 3]) &
                    SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 1]) & SMask) * (((int)words2[bstart +
                    2]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 2]) & SMask) * (((int)words2[bstart +
                    1]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 3]) & SMask) * (((int)words2[bstart]) &
                    SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 3] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = (((int)words1[astart]) & SMask) * (((int)words2[bstart + 4]) &
                    SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 1]) & SMask) * (((int)words2[bstart +
                    3]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 2]) & SMask) * (((int)words2[bstart +
                    2]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 3]) & SMask) * (((int)words2[bstart +
                    1]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 4]) & SMask) * (((int)words2[bstart]) &
                    SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 4] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = (((int)words1[astart]) & SMask) * (((int)words2[bstart + 5]) &
                    SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 1]) & SMask) * (((int)words2[bstart +
                    4]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 2]) & SMask) * (((int)words2[bstart +
                    3]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 3]) & SMask) * (((int)words2[bstart +
                    2]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 4]) & SMask) * (((int)words2[bstart +
                    1]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 5]) & SMask) * (((int)words2[bstart]) &
                    SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 5] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = (((int)words1[astart]) & SMask) * (((int)words2[bstart + 6]) &
                    SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 1]) & SMask) * (((int)words2[bstart +
                    5]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 2]) & SMask) * (((int)words2[bstart +
                    4]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 3]) & SMask) * (((int)words2[bstart +
                    3]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 4]) & SMask) * (((int)words2[bstart +
                    2]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 5]) & SMask) * (((int)words2[bstart +
                    1]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 6]) & SMask) * (((int)words2[bstart]) &
                    SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 6] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = (((int)words1[astart]) & SMask) * (((int)words2[bstart + 7]) &
                    SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 1]) & SMask) * (((int)words2[bstart +
                    6]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 2]) & SMask) * (((int)words2[bstart +
                    5]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 3]) & SMask) * (((int)words2[bstart +
                    4]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 4]) & SMask) * (((int)words2[bstart +
                    3]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 5]) & SMask) * (((int)words2[bstart +
                    2]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 6]) & SMask) * (((int)words2[bstart +
                    1]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 7]) & SMask) * (((int)words2[bstart]) &
                    SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 7] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = (((int)words1[astart + 1]) & SMask) * (((int)words2[bstart +
                    7]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 2]) & SMask) * (((int)words2[bstart +
                    6]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 3]) & SMask) * (((int)words2[bstart +
                    5]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 4]) & SMask) * (((int)words2[bstart +
                    4]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 5]) & SMask) * (((int)words2[bstart +
                    3]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 6]) & SMask) * (((int)words2[bstart +
                    2]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 7]) & SMask) * (((int)words2[bstart +
                    1]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 8] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = (((int)words1[astart + 2]) & SMask) * (((int)words2[bstart +
                    7]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 3]) & SMask) * (((int)words2[bstart +
                    6]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 4]) & SMask) * (((int)words2[bstart +
                    5]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 5]) & SMask) * (((int)words2[bstart +
                    4]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 6]) & SMask) * (((int)words2[bstart +
                    3]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 7]) & SMask) * (((int)words2[bstart +
                    2]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 9] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = (((int)words1[astart + 3]) & SMask) * (((int)words2[bstart +
                    7]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 4]) & SMask) * (((int)words2[bstart +
                    6]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 5]) & SMask) * (((int)words2[bstart +
                    5]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 6]) & SMask) * (((int)words2[bstart +
                    4]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 7]) & SMask) * (((int)words2[bstart +
                    3]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 10] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = (((int)words1[astart + 4]) & SMask) * (((int)words2[bstart +
                    7]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 5]) & SMask) * (((int)words2[bstart +
                    6]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 6]) & SMask) * (((int)words2[bstart +
                    5]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 7]) & SMask) * (((int)words2[bstart +
                    4]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 11] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = (((int)words1[astart + 5]) & SMask) * (((int)words2[bstart +
                    7]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 6]) & SMask) * (((int)words2[bstart +
                    6]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 7]) & SMask) * (((int)words2[bstart +
                    5]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 12] = c; c =
          (short)d; d = ((int)d >> 16) & SMask;
        p = (((int)words1[astart + 6]) & SMask) * (((int)words2[bstart +
                    7]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask;
        p = (((int)words1[astart + 7]) & SMask) * (((int)words2[bstart +
                    6]) & SMask);
        p += ((int)c) & SMask; c = (short)p;
        d += ((int)p >> 16) & SMask; result[rstart + 13] = c;
        p = (((int)words1[astart + 7]) & SMask) * (((int)words2[bstart +
                    7]) & SMask);
        p += d; result[rstart + 14] = (short)p; result[rstart + 15] =
          (short)(p >> 16);
      }
    }

    #endregion
    private const int RecursionLimit = 10;

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
      // Console.WriteLine("RecursiveMultiply " + count + " " + count +
      // " [r=" + resultStart + " t=" + tempStart + " a=" + words1Start +
      // " b=" + words2Start + "]");
      #if DEBUG
      if (resultArr == null) {
        throw new ArgumentNullException("resultArr");
      }

      if (resultStart < 0) {
        throw new ArgumentException("resultStart (" + resultStart +
                    ") is less than 0");
      }

      if (resultStart > resultArr.Length) {
        throw new ArgumentException("resultStart (" + resultStart +
                    ") is more than " + resultArr.Length);
      }

      if (count + count < 0) {
        throw new ArgumentException("count plus count (" + (count + count) +
                    ") is less than 0");
      }
      if (count + count > resultArr.Length) {
        throw new ArgumentException("count plus count (" + (count + count) +
                    ") is more than " + resultArr.Length);
      }

      if (resultArr.Length - resultStart < count + count) {
        throw new ArgumentException("resultArr.Length minus resultStart (" +
                    (resultArr.Length - resultStart) +
                    ") is less than " + (count + count));
      }

      if (tempArr == null) {
        throw new ArgumentNullException("tempArr");
      }

      if (tempStart < 0) {
        throw new ArgumentException("tempStart (" + tempStart +
                    ") is less than 0");
      }

      if (tempStart > tempArr.Length) {
        throw new ArgumentException("tempStart (" + tempStart +
                    ") is more than " + tempArr.Length);
      }

      if (count + count < 0) {
        throw new ArgumentException("count plus count (" + (count + count) +
                    ") is less than 0");
      }

      if (count + count > tempArr.Length) {
        throw new ArgumentException("count plus count (" + (count + count) +
                    ") is more than " + tempArr.Length);
      }

      if (tempArr.Length - tempStart < count + count) {
        throw new ArgumentException("tempArr.Length minus tempStart (" +
                    (tempArr.Length - tempStart) +
                    ") is less than " + (count + count));
      }

      if (words1 == null) {
        throw new ArgumentNullException("words1");
      }

      if (words1Start < 0) {
        throw new ArgumentException("words1Start (" + words1Start +
                    ") is less than 0");
      }

      if (words1Start > words1.Length) {
        throw new ArgumentException("words1Start (" + words1Start +
                    ") is more than " + words1.Length);
      }

      if (count < 0) {
        throw new ArgumentException("count (" + count + ") is less than " +
                    "0");
      }

      if (count > words1.Length) {
        throw new ArgumentException("count (" + count + ") is more than " +
                    words1.Length);
      }

      if (words1.Length - words1Start < count) {
        throw new ArgumentException("words1.Length minus words1Start (" +
                    (words1.Length - words1Start) + ") is less than " +
                    count);
      }

      if (words2 == null) {
        throw new ArgumentNullException("words2");
      }

      if (words2Start < 0) {
        throw new ArgumentException("words2Start (" + words2Start +
                    ") is less than 0");
      }

      if (words2Start > words2.Length) {
        throw new ArgumentException("words2Start (" + words2Start +
                    ") is more than " + words2.Length);
      }

      if (count < 0) {
        throw new ArgumentException("count (" + count + ") is less than " +
                    "0");
      }

      if (count > words2.Length) {
        throw new ArgumentException("count (" + count + ") is more than " +
                    words2.Length);
      }

      if (words2.Length - words2Start < count) {
        throw new ArgumentException("words2.Length minus words2Start (" +
                    (words2.Length - words2Start) + ") is less than " +
                    count);
      }
      #endif

      if (count <= RecursionLimit) {
        switch (count) {
          case 2:
            BaselineMultiply2(
resultArr,
resultStart,
words1,
words1Start,
words2,
words2Start);
            break;
          case 4:
            BaselineMultiply4(
  resultArr,
  resultStart,
  words1,
  words1Start,
  words2,
  words2Start);
            break;
          case 8:
            BaselineMultiply8(
  resultArr,
  resultStart,
  words1,
  words1Start,
  words2,
  words2Start);
            break;
          default: SchoolbookMultiply(
resultArr,
resultStart,
words1,
words1Start,
count,
words2,
words2Start,
count);
            break;
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
        var offset2For1 = 0;
        var offset2For2 = 0;
        if (countA == 0 || countB == 0) {
          // words1 or words2 is empty, so result is 0
          Array.Clear((short[])resultArr, resultStart, count << 1);
          return;
        }
        // Split words1 and words2 in two parts each
        if ((count & 1) == 0) {
          int count2 = count >> 1;
          if (countA <= count2 && countB <= count2) {
            // Console.WriteLine("Can be smaller: " + AN + "," + BN + "," +
            // (count2));
            Array.Clear((short[])resultArr, resultStart + count, count);
            if (count2 == 8) {
              BaselineMultiply8(
                resultArr,
                resultStart,
                words1,
                words1Start,
                words2,
                words2Start);
            } else {
              SameSizeMultiply(
                resultArr,
                resultStart,
                tempArr,
                tempStart,
                words1,
                words1Start,
                words2,
                words2Start,
                count2);
            }
            return;
          }
          int resultMediumHigh = resultStart + count;
          int resultHigh = resultMediumHigh + count2;
          int resultMediumLow = resultStart + count2;
          int tsn = tempStart + count;
          offset2For1 = Compare(
            words1,
            words1Start,
            words1,
            words1Start + count2,
            count2) > 0 ? 0 : count2;
          // Absolute value of low part minus high part of words1
          var tmpvar = (int)(words1Start + (count2 ^
                    offset2For1));
          SubtractOneByOne(
            resultArr,
            resultStart,
            words1,
            words1Start + offset2For1,
            words1,
            tmpvar,
            count2);
          offset2For2 = Compare(
            words2,
            words2Start,
            words2,
            words2Start + count2,
            count2) > 0 ? 0 : count2;
          // Absolute value of low part minus high part of words2
          int tmp = words2Start + (count2 ^ offset2For2);
          SubtractOneByOne(
            resultArr,
            resultMediumLow,
            words2,
            words2Start + offset2For2,
            words2,
            tmp,
            count2);
          //---------
          // HighA * HighB
          SameSizeMultiply(
            resultArr,
            resultMediumHigh,
            tempArr,
            tsn,
            words1,
            words1Start + count2,
            words2,
            words2Start + count2,
            count2);
          // Medium high result = Abs(LowA-HighA) * Abs(LowB-HighB)
          SameSizeMultiply(
            tempArr,
            tempStart,
            tempArr,
            tsn,
            resultArr,
            resultStart,
            resultArr,
            resultMediumLow,
            count2);
          // Low result = LowA * LowB
          SameSizeMultiply(
            resultArr,
            resultStart,
            tempArr,
            tsn,
            words1,
            words1Start,
            words2,
            words2Start,
            count2);
          int c2 = AddOneByOne(
            resultArr,
            resultMediumHigh,
            resultArr,
            resultMediumHigh,
            resultArr,
            resultMediumLow,
            count2);
          int c3 = c2;
          c2 += AddOneByOne(
            resultArr,
            resultMediumLow,
            resultArr,
            resultMediumHigh,
            resultArr,
            resultStart,
            count2);
          c3 += AddOneByOne(
            resultArr,
            resultMediumHigh,
            resultArr,
            resultMediumHigh,
            resultArr,
            resultHigh,
            count2);
          if (offset2For1 == offset2For2) {
            c3 -= SubtractOneByOne(
              resultArr,
              resultMediumLow,
              resultArr,
              resultMediumLow,
              tempArr,
              tempStart,
              count);
          } else {
            c3 += AddOneByOne(
              resultArr,
              resultMediumLow,
              resultArr,
              resultMediumLow,
              tempArr,
              tempStart,
              count);
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
          offset2For1 = CompareWithOneBiggerWords1(
            words1,
            words1Start,
            words1,
            words1Start + countLow,
            countLow) > 0 ? 0 : countLow;
          if (offset2For1 == 0) {
            SubtractOneBiggerWords1(
              resultArr,
              resultStart,
              words1,
              words1Start,
              words1,
              words1Start + countLow,
              countLow);
          } else {
            SubtractOneBiggerWords2(
              resultArr,
              resultStart,
              words1,
              words1Start + countLow,
              words1,
              words1Start,
              countLow);
          }
          offset2For2 = CompareWithOneBiggerWords1(
            words2,
            words2Start,
            words2,
            words2Start + countLow,
            countLow) > 0 ? 0 : countLow;
          if (offset2For2 == 0) {
            SubtractOneBiggerWords1(
              tempArr,
              tempStart,
              words2,
              words2Start,
              words2,
              words2Start + countLow,
              countLow);
          } else {
            SubtractOneBiggerWords2(
              tempArr,
              tempStart,
              words2,
              words2Start + countLow,
              words2,
              words2Start,
              countLow);
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
          // DebugWords(resultArr, resultStart + shorterOffset, countLow <<
          // 1,"w1*w2");
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
          int c2 = AddOneByOne(
            resultArr,
            resultStart + countMiddle,
            resultArr,
            resultStart + countMiddle,
            resultArr,
            resultStart + countLow,
            countLow);
          int c3 = c2;
          // DebugWords(resultArr,resultStart,count*2,"q2");
          c2 += AddOneByOne(
            resultArr,
            resultStart + countLow,
            resultArr,
            resultStart + countMiddle,
            resultArr,
            resultStart,
            countLow);
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
            c3 -= SubtractOneByOne(
              resultArr,
              resultStart + countLow,
              resultArr,
              resultStart + countLow,
              tempArr,
              tempStart + shorterOffset,
              countLow << 1);
          } else {
            c3 += AddOneByOne(
              resultArr,
              resultStart + countLow,
              resultArr,
              resultStart + countLow,
              tempArr,
              tempStart + shorterOffset,
              countLow << 1);
          }
          // DebugWords(resultArr,resultStart,count*2,"q5");
          c3 += Increment(
            resultArr,
            resultStart + countMiddle,
            countLow,
            (short)c2);
          // DebugWords(resultArr,resultStart,count*2,"q6");
          if (c3 != 0) {
            Increment(
              resultArr,
              resultStart + countMiddle + countLow,
              countLow - 2,
              (short)c3);
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
        switch (count) {
          case 2:
            BaselineSquare2(resultArr, resultStart, words1, words1Start);
            break;
          case 4:
            BaselineSquare4(resultArr, resultStart, words1, words1Start);
            break;
          case 8:
            BaselineSquare8(resultArr, resultStart, words1, words1Start);
            break;
          default:
          SchoolbookSquare(resultArr, resultStart, words1, words1Start,
              count);
            break;
        }
      } else if ((count & 1) == 0) {
        int count2 = count >> 1;
        RecursiveSquare(
          resultArr,
          resultStart,
          tempArr,
          tempStart + count,
          words1,
          words1Start,
          count2);
        RecursiveSquare(
          resultArr,
          resultStart + count,
          tempArr,
          tempStart + count,
          words1,
          words1Start + count2,
          count2);
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
        int carry = AddOneByOne(
          resultArr,
          resultStart + count2,
          resultArr,
          resultStart + count2,
          tempArr,
          tempStart,
          count);
        carry += AddOneByOne(
          resultArr,
          resultStart + count2,
          resultArr,
          resultStart + count2,
          tempArr,
          tempStart,
          count);
        Increment(
          resultArr,
          (int)(resultStart + count + count2),
          count2,
          (short)carry);
      } else {
        SameSizeMultiply(
          resultArr,
          resultStart,
          tempArr,
          tempStart,
          words1,
          words1Start,
          words1,
          words1Start,
          count);
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
      for (var i = 0; i < words1Count; ++i) {
        cstart = resultStart + i;
        unchecked {
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
        for (var i = 0; i < words1Count; ++i) {
          cstart = resultStart + i;
          unchecked {
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
        for (var i = 0; i < words2Count; ++i) {
          cstart = resultStart + i;
          unchecked {
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
      #if DEBUG
      if (acount < bcount) {
        throw new ArgumentException("acount (" + acount + ") is less than " +
                    bcount);
      }

      if (productArr == null) {
        throw new ArgumentNullException("productArr");
      }

      if (cstart < 0) {
        throw new ArgumentException("cstart (" + cstart + ") is less than " +
                    "0");
      }

      if (cstart > productArr.Length) {
        throw new ArgumentException("cstart (" + cstart + ") is more than " +
                    productArr.Length);
      }

      if (acount + bcount < 0) {
        throw new ArgumentException("acount plus bcount (" + (acount + bcount) +
                    ") is less than 0");
      }

      if (acount + bcount > productArr.Length) {
        throw new ArgumentException("acount plus bcount (" + (acount + bcount) +
                    ") is more than " + productArr.Length);
      }

      if (productArr.Length - cstart < acount + bcount) {
        throw new ArgumentException("productArr.Length minus cstart (" +
                    (productArr.Length - cstart) +
                    ") is less than " + (acount + bcount));
      }

      if (tempArr == null) {
        throw new ArgumentNullException("tempArr");
      }

      if (tempStart < 0) {
        throw new ArgumentException("tempStart (" + tempStart +
                    ") is less than 0");
      }

      if (tempStart > tempArr.Length) {
        throw new ArgumentException("tempStart (" + tempStart +
                    ") is more than " + tempArr.Length);
      }

      if ((bcount * 4) < 0) {
        throw new ArgumentException("bcount * 4 less than 0 (" + (bcount * 4) +
                    ")");
      }

      if ((bcount * 4) > tempArr.Length) {
        throw new ArgumentException("bcount * 4 more than " + tempArr.Length +
                    " (" + (bcount * 4) + ")");
      }

      if (tempArr.Length - tempStart < bcount * 4) {
        throw new ArgumentException("tempArr.Length minus tempStart (" +
                    (tempArr.Length - tempStart) +
                    ") is less than " + (bcount * 4));
      }

      if (words1 == null) {
        throw new ArgumentNullException("words1");
      }

      if (astart < 0) {
        throw new ArgumentException("astart (" + astart + ") is less than " +
                    "0");
      }

      if (astart > words1.Length) {
        throw new ArgumentException("astart (" + astart + ") is more than " +
                    words1.Length);
      }

      if (acount < 0) {
        throw new ArgumentException("acount (" + acount + ") is less than " +
                    "0");
      }

      if (acount > words1.Length) {
        throw new ArgumentException("acount (" + acount + ") is more than " +
                    words1.Length);
      }

      if (words1.Length - astart < acount) {
        throw new ArgumentException("words1.Length minus astart (" +
                    (words1.Length - astart) + ") is less than " +
                    acount);
      }

      if (words2 == null) {
        throw new ArgumentNullException("words2");
      }

      if (bstart < 0) {
        throw new ArgumentException("bstart (" + bstart + ") is less than " +
                    "0");
      }

      if (bstart > words2.Length) {
        throw new ArgumentException("bstart (" + bstart + ") is more than " +
                    words2.Length);
      }

      if (bcount < 0) {
        throw new ArgumentException("bcount (" + bcount + ") is less than " +
                    "0");
      }

      if (bcount > words2.Length) {
        throw new ArgumentException("bcount (" + bcount + ") is more than " +
                    words2.Length);
      }

      if (words2.Length - bstart < bcount) {
        throw new ArgumentException("words2.Length minus bstart (" +
                    (words2.Length - bstart) + ") is less than " +
                    bcount);
      }
      #endif

      unchecked {
        var carryPos = 0;
        // Set carry to zero
        Array.Clear((short[])productArr, cstart, bcount);
        for (var i = 0; i < acount; i += bcount) {
          int diff = acount - i;
          if (diff > bcount) {
            SameSizeMultiply(
              tempArr,
              tempStart,
              tempArr,
              tempStart + bcount + bcount,
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
            Array.Copy(
              tempArr,
              tempStart,
              productArr,
              cstart + i,
              bcount + bcount);
            carryPos += bcount;
          } else {
            AsymmetricMultiply(
              tempArr,
              tempStart,  // uses diff + bcount space
              tempArr,
              tempStart + diff + bcount,  // uses diff + bcount
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
            Array.Copy(
              tempArr,
              tempStart,
              productArr,
              cstart + i,
              diff + bcount);
          }
        }
      }
    }

    // Multiplies two operands of different sizes
    private static void AsymmetricMultiply(
      short[] resultArr,
      int resultStart,  // uses words1Count + words2Count
      short[] tempArr,
      int tempStart,  // uses words1Count + words2Count
      short[] words1,
      int words1Start,
      int words1Count,
      short[] words2,
      int words2Start,
      int words2Count) {
      // Console.WriteLine("AsymmetricMultiply " + words1Count + " " +
      // words2Count + " [r=" + resultStart + " t=" + tempStart + " a=" +
      // words1Start + " b=" + words2Start + "]");
      #if DEBUG
      if (resultArr == null) {
        throw new ArgumentNullException("resultArr");
      }

      if (resultStart < 0) {
        throw new ArgumentException("resultStart (" + resultStart +
                    ") is less than 0");
      }

      if (resultStart > resultArr.Length) {
        throw new ArgumentException("resultStart (" + resultStart +
                    ") is more than " + resultArr.Length);
      }

      if (words1Count + words2Count < 0) {
        throw new ArgumentException("words1Count plus words2Count (" +
                    (words1Count + words2Count) + ") is less than " +
                    "0");
      }

      if (words1Count + words2Count > resultArr.Length) {
        throw new ArgumentException("words1Count plus words2Count (" +
                    (words1Count + words2Count) +
                    ") is more than " + resultArr.Length);
      }

      if (resultArr.Length - resultStart < words1Count + words2Count) {
        throw new ArgumentException("resultArr.Length minus resultStart (" +
                    (resultArr.Length - resultStart) +
                    ") is less than " + (words1Count +
                    words2Count));
      }

      if (tempArr == null) {
        throw new ArgumentNullException("tempArr");
      }

      if (tempStart < 0) {
        throw new ArgumentException("tempStart (" + tempStart +
                    ") is less than 0");
      }

      if (tempStart > tempArr.Length) {
        throw new ArgumentException("tempStart (" + tempStart +
                    ") is more than " + tempArr.Length);
      }

      if (words1Count + words2Count < 0) {
        throw new ArgumentException("words1Count plus words2Count (" +
                    (words1Count + words2Count) + ") is less than " +
                    "0");
      }

      if (words1Count + words2Count > tempArr.Length) {
        throw new ArgumentException("words1Count plus words2Count (" +
                    (words1Count + words2Count) +
                    ") is more than " + tempArr.Length);
      }

      if (tempArr.Length - tempStart < words1Count + words2Count) {
        throw new ArgumentException("tempArr.Length minus tempStart (" +
                    (tempArr.Length - tempStart) +
                    ") is less than " + (words1Count +
                    words2Count));
      }

      if (words1 == null) {
        throw new ArgumentNullException("words1");
      }

      if (words1Start < 0) {
        throw new ArgumentException("words1Start (" + words1Start +
                    ") is less than 0");
      }

      if (words1Start > words1.Length) {
        throw new ArgumentException("words1Start (" + words1Start +
                    ") is more than " + words1.Length);
      }

      if (words1Count < 0) {
        throw new ArgumentException("words1Count (" + words1Count +
                    ") is less than 0");
      }

      if (words1Count > words1.Length) {
        throw new ArgumentException("words1Count (" + words1Count +
                    ") is more than " + words1.Length);
      }

      if (words1.Length - words1Start < words1Count) {
        throw new ArgumentException("words1.Length minus words1Start (" +
                    (words1.Length - words1Start) +
                    ") is less than " + words1Count);
      }

      if (words2 == null) {
        throw new ArgumentNullException("words2");
      }

      if (words2Start < 0) {
        throw new ArgumentException("words2Start (" + words2Start +
                    ") is less than 0");
      }

      if (words2Start > words2.Length) {
        throw new ArgumentException("words2Start (" + words2Start +
                    ") is more than " + words2.Length);
      }

      if (words2Count < 0) {
        throw new ArgumentException("words2Count (" + words2Count +
                    ") is less than 0");
      }

      if (words2Count > words2.Length) {
        throw new ArgumentException("words2Count (" + words2Count +
                    ") is more than " + words2.Length);
      }

      if (words2.Length - words2Start < words2Count) {
        throw new ArgumentException("words2.Length minus words2Start (" +
                    (words2.Length - words2Start) +
                    ") is less than " + words2Count);
      }
      #endif

      if (words1Count == words2Count) {
        if (words1Start == words2Start && words1 == words2) {
          // Both operands have the same value and the same word count
          RecursiveSquare(
            resultArr,
            resultStart,
            tempArr,
            tempStart,
            words1,
            words1Start,
            words1Count);
        } else if (words1Count == 2) {
          // Both operands have a word count of 2
          BaselineMultiply2(
            resultArr,
            resultStart,
            words1,
            words1Start,
            words2,
            words2Start);
        } else {
          // Other cases where both operands have the same word count
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

      if (words1Count == 1 || (words1Count == 2 && words1[words1Start + 1] ==
                    0)) {
        switch (words1[words1Start]) {
          case 0:
            // words1 is zero, so result is 0
            Array.Clear((short[])resultArr, resultStart, words2Count + 2);
            return;
          case 1:
            Array.Copy(
              words2,
              words2Start,
              resultArr,
              resultStart,
              (int)words2Count);
            resultArr[resultStart + words2Count] = (short)0;
            resultArr[resultStart + words2Count + 1] = (short)0;
            return;
          default:
            resultArr[resultStart + words2Count] = LinearMultiply(
              resultArr,
              resultStart,
              words2,
              words2Start,
              words1[words1Start],
              words2Count);
            resultArr[resultStart + words2Count + 1] = (short)0;
            return;
        }
      }
      if (words1Count == 2 && (words2Count & 1) == 0) {
        int a0 = ((int)words1[words1Start]) & 0xffff;
        int a1 = ((int)words1[words1Start + 1]) & 0xffff;
        resultArr[resultStart + words2Count] = (short)0;
        resultArr[resultStart + words2Count + 1] = (short)0;
        AtomicMultiplyOpt(
          resultArr,
          resultStart,
          a0,
          a1,
          words2,
          words2Start,
          0,
          words2Count);
        AtomicMultiplyAddOpt(
          resultArr,
          resultStart,
          a0,
          a1,
          words2,
          words2Start,
          2,
          words2Count);
        return;
      }
      if (words1Count <= 10 && words2Count <= 10) {
        SchoolbookMultiply(
          resultArr,
          resultStart,
          words1,
          words1Start,
          words1Count,
          words2,
          words2Start,
          words2Count);
      } else {
        int wordsRem = words2Count % words1Count;
        int evenmult = (words2Count / words1Count) & 1;
        int i;
        // Console.WriteLine("counts=" + words1Count + "," + words2Count +
        // " res=" + (resultStart + words1Count) + " temp=" + (tempStart +
        // (words1Count << 1)) + " rem=" + wordsRem + " evenwc=" + evenmult);
        if (wordsRem == 0) {
          // words2Count is divisible by words1count
          if (evenmult == 0) {
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
            Array.Copy(
              resultArr,
              resultStart + words1Count,
              tempArr,
              (int)(tempStart + (words1Count << 1)),
              words1Count);
            for (i = words1Count << 1; i < words2Count; i += words1Count << 1) {
              SameSizeMultiply(
                tempArr,
                tempStart + words1Count + i,
                tempArr,
                tempStart,
                words1,
                words1Start,
                words2,
                words2Start + i,
                words1Count);
            }
            for (i = words1Count; i < words2Count; i += words1Count << 1) {
              SameSizeMultiply(
                resultArr,
                resultStart + i,
                tempArr,
                tempStart,
                words1,
                words1Start,
                words2,
                words2Start + i,
                words1Count);
            }
          } else {
            for (i = 0; i < words2Count; i += words1Count << 1) {
              SameSizeMultiply(
                resultArr,
                resultStart + i,
                tempArr,
                tempStart,
                words1,
                words1Start,
                words2,
                words2Start + i,
                words1Count);
            }
            for (i = words1Count; i < words2Count; i += words1Count << 1) {
              SameSizeMultiply(
                tempArr,
                tempStart + words1Count + i,
                tempArr,
                tempStart,
                words1,
                words1Start,
                words2,
                words2Start + i,
                words1Count);
            }
          }
          if (
            Add(
              resultArr,
              resultStart + words1Count,
              resultArr,
              resultStart + words1Count,
              tempArr,
              tempStart + (words1Count << 1),
              words2Count - words1Count) != 0) {
            Increment(
              resultArr,
              (int)(resultStart + words2Count),
              words1Count,
              (short)1);
          }
        } else if ((words1Count + words2Count) >= (words1Count << 2)) {
          // Console.WriteLine("Chunked Linear Multiply Long");
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
                   (words1Count + 2 == words2Count && words2[words2Start +
                    words2Count - 1] == 0)) {
          Array.Clear(
            (short[])resultArr,
            resultStart,
            words1Count + words2Count);
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
          var t2 = new short[words1Count << 2];
          // Console.WriteLine("Chunked Linear Multiply Short");
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
      return unchecked((int)((((int)first) & 0xffff) | ((int)second << 16)));
    }

    private static short GetLowHalf(int val) {
      return unchecked((short)(val & 0xffff));
    }

    private static short GetHighHalf(int val) {
      return unchecked((short)((val >> 16) & 0xffff));
    }

    private static short GetHighHalfAsBorrow(int val) {
      return unchecked((short)(0 - ((val >> 16) & 0xffff)));
    }

    private static int BitPrecision(short numberValue) {
      if (numberValue == 0) {
        return 0;
      }
      var i = 16;
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

        if ((numberValue >> 15) == 0) {
          --i;
        }
      }
      return i;
    }

    private static short Divide32By16(
      int dividendLow,
      short divisorShort,
      bool returnRemainder) {
      int tmpInt;
      var dividendHigh = 0;
      int intDivisor = ((int)divisorShort) & 0xffff;
      for (var i = 0; i < 32; ++i) {
        tmpInt = dividendHigh >> 31;
        dividendHigh <<= 1;
        dividendHigh = unchecked((int)(dividendHigh | ((int)((dividendLow >>
                    31) & 1))));
        dividendLow <<= 1;
        tmpInt |= dividendHigh;
        // unsigned greater-than-or-equal check
        if (((tmpInt >> 31) != 0) || (tmpInt >= intDivisor)) {
          unchecked {
            dividendHigh -= intDivisor;
            ++dividendLow;
          }
        }
      }
      return returnRemainder ? unchecked((short)(((int)dividendHigh) &
                    0xffff)) : unchecked((short)(((int)dividendLow) &
                    0xffff));
    }

    private static short DivideUnsigned(int x, short y) {
      unchecked {
        if ((x >> 31) == 0) {
          // x is already nonnegative
          int iy = ((int)y) & 0xffff;
          return (short)(((int)x / iy) & 0xffff);
        }
        return Divide32By16(x, y, false);
      }
    }

    private static short RemainderUnsigned(int x, short y) {
      unchecked {
        int iy = ((int)y) & 0xffff;
        return ((x >> 31) == 0) ? ((short)(((int)x % iy) & 0xffff)) :
          Divide32By16(x, y, true);
      }
    }

    private static short DivideThreeWordsByTwo(
      short[] words1,
      int words1Start,
      short valueB0,
      short valueB1) {
      short valueQ;
      unchecked {
        valueQ = ((short)(valueB1 + 1) == 0) ? words1[words1Start + 2] :
          ((valueB1 != 0) ? DivideUnsigned(
            MakeUint(
              words1[words1Start + 1],
              words1[words1Start + 2]),
            (short)(((int)valueB1 + 1) & 0xffff)) : DivideUnsigned(
             MakeUint(words1[words1Start], words1[words1Start + 1]),
             valueB0));

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
               (((int)words1[words1Start + 1]) & 0xffff) > (((int)valueB1) &
                    0xffff) || (words1[words1Start + 1] == valueB1 &&
                    (((int)words1[words1Start]) & 0xffff) >=
                    (((int)valueB0) & 0xffff))) {
          u = (((int)words1[words1Start]) & 0xffff) - valueB0int;
          words1[words1Start] = GetLowHalf(u);
          u = (((int)words1[words1Start + 1]) & 0xffff) - valueB1int -
            (((int)GetHighHalfAsBorrow(u)) & 0xffff);
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
        // if divisor is 0, we assume divisor == 2**32
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

    private static void AtomicMultiplyOpt(
      short[] c,
      int valueCstart,
      int valueA0,
      int valueA1,
      short[] words2,
      int words2Start,
      int istart,
      int iend) {
      short s;
      int d;
      int first1MinusFirst0 = ((int)valueA1 - valueA0) & 0xffff;
      valueA1 &= 0xffff;
      valueA0 &= 0xffff;
      unchecked {
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
            tempInt = a0b0high + (((int)valueA0B0) & 0xffff) + (((int)d) &
                    0xffff) + (((int)valueA1B1) & 0xffff);
            c[csi + 1] = (short)(((int)tempInt) & 0xffff);

            tempInt = valueA1B1 + (((int)(tempInt >> 16)) & 0xffff) +
              a0b0high + (((int)(d >> 16)) & 0xffff) + (((int)(valueA1B1 >>
                    16)) & 0xffff) - (((int)s) & 0xffff);

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
              d = (((int)valueA0 - valueA1) & 0xffff) * (((int)valueB1 -
                    valueB0) & 0xffff);
            }
            int valueA0B0 = valueA0 * valueB0;
            int a0b0high = (valueA0B0 >> 16) & 0xffff;
            c[csi] = (short)(((int)valueA0B0) & 0xffff);

            int valueA1B1 = valueA1 * valueB1;
            int tempInt;
            tempInt = a0b0high + (((int)valueA0B0) & 0xffff) + (((int)d) &
                    0xffff) + (((int)valueA1B1) & 0xffff);
            c[csi + 1] = (short)(((int)tempInt) & 0xffff);

            tempInt = valueA1B1 + (((int)(tempInt >> 16)) & 0xffff) +
              a0b0high + (((int)(d >> 16)) & 0xffff) + (((int)(valueA1B1 >>
                    16)) & 0xffff) - (((int)s) & 0xffff);

            c[csi + 2] = (short)(((int)tempInt) & 0xffff);
            c[csi + 3] = (short)(((int)(tempInt >> 16)) & 0xffff);
          }
        }
      }
    }

    private static void AtomicMultiplyAddOpt(
      short[] c,
      int valueCstart,
      int valueA0,
      int valueA1,
      short[] words2,
      int words2Start,
      int istart,
      int iend) {
      short s;
      int d;
      int first1MinusFirst0 = ((int)valueA1 - valueA0) & 0xffff;
      valueA1 &= 0xffff;
      valueA0 &= 0xffff;
      unchecked {
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
            tempInt = (((int)(tempInt >> 16)) & 0xffff) + (((int)valueA0B0) &
                    0xffff) + (((int)d) & 0xffff) + a1b1low +
              (((int)c[csi + 1]) & 0xffff);
            c[csi + 1] = (short)(((int)tempInt) & 0xffff);

            tempInt = (((int)(tempInt >> 16)) & 0xffff) + a1b1low + a0b0high +
              (((int)(d >> 16)) & 0xffff) +
              a1b1high - (((int)s) & 0xffff) + (((int)c[csi + 2]) & 0xffff);
            c[csi + 2] = (short)(((int)tempInt) & 0xffff);

            tempInt = (((int)(tempInt >> 16)) & 0xffff) + a1b1high +
              (((int)c[csi + 3]) & 0xffff);
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
              d = (((int)valueA0 - valueA1) & 0xffff) * (((int)valueB1 -
                    valueB0) & 0xffff);
            }
            int valueA0B0 = valueA0 * valueB0;
            int a0b0high = (valueA0B0 >> 16) & 0xffff;
            int tempInt;
            tempInt = valueA0B0 + (((int)c[csi]) & 0xffff);
            c[csi] = (short)(((int)tempInt) & 0xffff);

            int valueA1B1 = valueA1 * valueB1;
            int a1b1low = valueA1B1 & 0xffff;
            int a1b1high = (valueA1B1 >> 16) & 0xffff;
            tempInt = (((int)(tempInt >> 16)) & 0xffff) + (((int)valueA0B0) &
                    0xffff) + (((int)d) & 0xffff) + a1b1low +
              (((int)c[csi + 1]) & 0xffff);
            c[csi + 1] = (short)(((int)tempInt) & 0xffff);

            tempInt = (((int)(tempInt >> 16)) & 0xffff) + a1b1low + a0b0high +
              (((int)(d >> 16)) & 0xffff) +
              a1b1high - (((int)s) & 0xffff) + (((int)c[csi + 2]) & 0xffff);
            c[csi + 2] = (short)(((int)tempInt) & 0xffff);

            tempInt = (((int)(tempInt >> 16)) & 0xffff) + a1b1high +
              (((int)c[csi + 3]) & 0xffff);
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
      int remainderStart,  // remainder
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
      #if DEBUG
      if (words1Count <= 0) {
        throw new ArgumentException("words1Count (" + words1Count +
                    ") is not greater than 0");
      }
      if (words2Count <= 0) {
        throw new ArgumentException("words2Count (" + words2Count +
                    ") is not greater than 0");
      }
      #endif
      if (words2Count == 0) {
        throw new DivideByZeroException("division by zero");
      }
      if (words2Count == 1) {
        if (words2[words2Start] == 0) {
          throw new DivideByZeroException("division by zero");
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
      #if DEBUG
      if (!(words1Count % 2 == 0 && words2Count % 2 == 0)) {
        throw new
          ArgumentException("doesn't satisfy valueNA%2==0 && valueNB%2==0");
      }
      if (!(words2[words2Start + words2Count - 1] != 0 ||
            words2[words2Start + words2Count - 2] != 0)) {
        throw new ArgumentException(
          "doesn't satisfy words2[valueNB-1]!=0 || words2[valueNB-2]!=0");
      }
      if (words2Count > words1Count) {
        throw new ArgumentException("doesn't satisfy valueNB<= valueNA");
      }
      #endif
      short[] quot = quotientArr;
      if (quotientArr == null) {
        quot = new short[2];
      }
      var valueTBstart = (int)(tempStart + (words1Count + 2));
      var valueTPstart = (int)(tempStart + (words1Count + 2 + words2Count));
      unchecked {
        // copy words2 into TB and normalize it so that TB has highest bit
        // set to 1
        int shiftWords = (short)(words2[words2Start + words2Count - 1] == 0 ?
                    1 : 0);
        tempArr[valueTBstart] = (short)0;
        tempArr[valueTBstart + words2Count - 1] = (short)0;
        Array.Copy(
          words2,
          words2Start,
          tempArr,
          (int)(valueTBstart + shiftWords),
          words2Count - shiftWords);
        var shiftBits = (short)((short)16 - BitPrecision(tempArr[valueTBstart +
                    words2Count - 1]));
        ShiftWordsLeftByBits(
          tempArr,
          valueTBstart,
          words2Count,
          shiftBits);
        // copy words1 into valueTA and normalize it
        tempArr[0] = (short)0;
        tempArr[words1Count] = (short)0;
        tempArr[words1Count + 1] = (short)0;
        Array.Copy(
          words1,
          words1Start,
          tempArr,
          (int)(tempStart + shiftWords),
          words1Count);
        ShiftWordsLeftByBits(
          tempArr,
          tempStart,
          words1Count + 2,
          shiftBits);

        if (tempArr[tempStart + words1Count + 1] == 0 &&
            (((int)tempArr[tempStart + words1Count]) & 0xffff) <= 1) {
          if (quotientArr != null) {
            quotientArr[quotientStart + words1Count - words2Count + 1] =
              (short)0;
            quotientArr[quotientStart + words1Count - words2Count] = (short)0;
          }
          while (
            tempArr[words1Count] != 0 || Compare(
              tempArr,
              (int)(tempStart + words1Count - words2Count),
              tempArr,
              valueTBstart,
              words2Count) >= 0) {
            tempArr[words1Count] -= (
              short)Subtract(
              tempArr,
              tempStart + words1Count - words2Count,
              tempArr,
              tempStart + words1Count - words2Count,
              tempArr,
              valueTBstart,
              words2Count);
            if (quotientArr != null) {
              quotientArr[quotientStart + words1Count - words2Count] +=
                (short)1;
            }
          }
        } else {
          words1Count += 2;
        }

        var valueBT0 = (short)(tempArr[valueTBstart + words2Count - 2] +
                    (short)1);
        var valueBT1 = (short)(tempArr[valueTBstart + words2Count - 1] +
                    (short)(valueBT0 == (short)0 ? 1 : 0));

        // start reducing valueTA mod TB, 2 words at a time
        var valueTAtomic = new short[4];
        for (int i = words1Count - 2; i >= words2Count; i -= 2) {
          int qs = (quotientArr == null) ? 0 : quotientStart + i - words2Count;
          DivideFourWordsByTwo(
            quot,
            qs,
            tempArr,
            tempStart + i - 2,
            valueBT0,
            valueBT1,
            valueTAtomic);
          // now correct the underestimated quotient
          int valueRstart2 = tempStart + i - words2Count;
          int n = words2Count;
          unchecked {
            int quotient0 = quot[qs];
            int quotient1 = quot[qs + 1];
            if (quotient1 == 0) {
              short carry = LinearMultiply(
                tempArr,
                valueTPstart,
                tempArr,
                valueTBstart,
                (short)quotient0,
                n);
              tempArr[valueTPstart + n] = carry;
              tempArr[valueTPstart + n + 1] = 0;
            } else if (n == 2) {
              BaselineMultiply2(
                tempArr,
                valueTPstart,
                quot,
                qs,
                tempArr,
                valueTBstart);
            } else {
              tempArr[valueTPstart + n] = (short)0;
              tempArr[valueTPstart + n + 1] = (short)0;
              quotient0 &= 0xffff;
              quotient1 &= 0xffff;
              AtomicMultiplyOpt(
                tempArr,
                valueTPstart,
                quotient0,
                quotient1,
                tempArr,
                valueTBstart,
                0,
                n);
              AtomicMultiplyAddOpt(
                tempArr,
                valueTPstart,
                quotient0,
                quotient1,
                tempArr,
                valueTBstart,
                2,
                n);
            }
            Subtract(
              tempArr,
              valueRstart2,
              tempArr,
              valueRstart2,
              tempArr,
              valueTPstart,
              n + 2);
            while (tempArr[valueRstart2 + n] != 0 || Compare(
              tempArr,
              valueRstart2,
              tempArr,
              valueTBstart,
              n) >= 0) {
              tempArr[valueRstart2 + n] -= (
                short)Subtract(
                tempArr,
                valueRstart2,
                tempArr,
                valueRstart2,
                tempArr,
                valueTBstart,
                n);
              if (quotientArr != null) {
                ++quotientArr[qs];
                quotientArr[qs + 1] += (short)((quotientArr[qs] == 0) ? 1 : 0);
              }
            }
          }
        }
        if (remainderArr != null) {  // If the remainder is non-null
          // copy valueTA into result, and denormalize it
          Array.Copy(
            tempArr,
            (int)(tempStart + shiftWords),
            remainderArr,
            remainderStart,
            words2Count);
          ShiftWordsRightByBits(
            remainderArr,
            remainderStart,
            words2Count,
            shiftBits);
        }
      }
    }

    private static int RoundupSize(int n) {
      return n + (n & 1);
    }

    private readonly bool negative;
    private readonly int wordCount;
    private readonly short[] words;

    private BigInteger(int wordCount, short[] reg, bool negative) {
      this.wordCount = wordCount;
      this.words = reg;
      this.negative = negative;
    }

    /// <summary>Initializes a BigInteger object from an array of
    /// bytes.</summary>
    /// <param name='bytes'>A byte array.</param>
    /// <param name='littleEndian'>A Boolean object.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    [Obsolete("Renamed to 'fromBytes'.")]
    public static BigInteger fromByteArray(byte[] bytes, bool littleEndian) {
      return fromBytes(bytes, littleEndian);
    }

    /// <summary>Initializes a BigInteger object from an array of
    /// bytes.</summary>
    /// <param name='bytes'>A byte array.</param>
    /// <param name='littleEndian'>A Boolean object.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    public static BigInteger fromBytes(byte[] bytes, bool littleEndian) {
      if (bytes == null) {
        throw new ArgumentNullException("bytes");
      }
      if (bytes.Length == 0) {
        return BigInteger.Zero;
      }
      int len = bytes.Length;
      int wordLength = ((int)len + 1) >> 1;
      wordLength = RoundupSize(wordLength);
      var newreg = new short[wordLength];
      int valueJIndex = littleEndian ? len - 1 : 0;
      bool numIsNegative = (bytes[valueJIndex] & 0x80) != 0;
      bool newnegative = numIsNegative;
      var j = 0;
      if (!numIsNegative) {
        for (var i = 0; i < len; i += 2, j++) {
          int index = littleEndian ? i : len - 1 - i;
          int index2 = littleEndian ? i + 1 : len - 2 - i;
          newreg[j] = (short)(((int)bytes[index]) & 0xff);
          if (index2 >= 0 && index2 < len) {
            newreg[j] |= unchecked((short)(((short)bytes[index2]) << 8));
          }
        }
      } else {
        for (var i = 0; i < len; i += 2, j++) {
          int index = littleEndian ? i : len - 1 - i;
          int index2 = littleEndian ? i + 1 : len - 2 - i;
          newreg[j] = (short)(((int)bytes[index]) & 0xff);
          if (index2 >= 0 && index2 < len) {
            newreg[j] |= unchecked((short)(((short)bytes[index2]) << 8));
          } else {
            // sign extend the last byte
            newreg[j] |= unchecked((short)0xff00);
          }
        }
        for (; j < newreg.Length; ++j) {
          newreg[j] = unchecked((short)0xffff);  // sign extend remaining words
        }
        TwosComplement(newreg, 0, (int)newreg.Length);
      }
      int newwordCount = newreg.Length;
      while (newwordCount != 0 && newreg[newwordCount - 1] == 0) {
        --newwordCount;
      }
      return (newwordCount == 0) ? BigInteger.Zero : (new
                    BigInteger(
                    newwordCount,
                    newreg,
                    newnegative));
    }

    private static short[] GrowForCarry(short[] a, short carry) {
      int oldLength = a.Length;
      short[] ret = CleanGrow(a, RoundupSize(oldLength + 1));
      ret[oldLength] = carry;
      return ret;
    }

    private static short[] CleanGrow(short[] a, int size) {
      if (size > a.Length) {
        var newa = new short[size];
        Array.Copy(a, newa, a.Length);
        return newa;
      }
      return a;
    }

    /// <summary>Returns whether a bit is set in the two's-complement
    /// representation of this object's value.</summary>
    /// <param name='index'>Zero based index of the bit to test. 0 means
    /// the least significant bit.</param>
    /// <returns>True if a bit is set in the two's-complement
    /// representation of this object's value; otherwise, false.</returns>
    public bool testBit(int index) {
      if (index < 0) {
        throw new ArgumentOutOfRangeException("index");
      }
      if (this.wordCount == 0) {
        return false;
      }
      if (this.negative) {
        var tcindex = 0;
        int wordpos = index / 16;
        if (wordpos >= this.words.Length) {
          return true;
        }
        while (tcindex < wordpos && this.words[tcindex] == 0) {
          ++tcindex;
        }
        short tc;
        unchecked {
          tc = this.words[wordpos];
          if (tcindex == wordpos) {
            --tc;
          }
          tc = (short)~tc;
        }
        return (bool)(((tc >> (int)(index & 15)) & 1) != 0);
      }
      return this.GetUnsignedBit(index);
    }

    private bool GetUnsignedBit(int n) {
      #if DEBUG
      if (n < 0) {
        throw new ArgumentException("n (" + n + ") is less than 0");
      }
      #endif
      return ((n >> 4) < this.words.Length) && ((bool)(((this.words[(n >>
                    4)] >> (int)(n & 15)) & 1) != 0));
    }

    /// <summary>Returns a byte array of this object&#x27;s
    /// value.</summary>
    /// <param name='littleEndian'>A Boolean object.</param>
    /// <returns>A byte array.</returns>
    [Obsolete("Renamed to 'toBytes'.")]
    public byte[] toByteArray(bool littleEndian) {
      return this.toBytes(littleEndian);
    }

    /// <summary>Returns a byte array of this object&#x27;s value. The byte
    /// array will take the form of the number's two' s-complement
    /// representation, using the fewest bytes necessary to represent its
    /// value unambiguously. If this value is negative, the bits that
    /// appear "before" the most significant bit of the number will be all
    /// ones.</summary>
    /// <param name='littleEndian'>If true, the least significant bits will
    /// appear first.</param>
    /// <returns>A byte array. If this value is 0, returns a byte array
    /// with the single element 0.</returns>
    public byte[] toBytes(bool littleEndian) {
      int sign = this.Sign;
      if (sign == 0) {
        return new[] { (byte)0 };
      }
      if (sign > 0) {
        int byteCount = this.ByteCount();
        int byteArrayLength = byteCount;
        if (this.GetUnsignedBit((byteCount * 8) - 1)) {
          ++byteArrayLength;
        }
        var bytes = new byte[byteArrayLength];
        var j = 0;
        for (var i = 0; i < byteCount; i += 2, j++) {
          int index = littleEndian ? i : bytes.Length - 1 - i;
          int index2 = littleEndian ? i + 1 : bytes.Length - 2 - i;
          bytes[index] = (byte)(this.words[j] & 0xff);
          if (index2 >= 0 && index2 < byteArrayLength) {
            bytes[index2] = (byte)((this.words[j] >> 8) & 0xff);
          }
        }
        return bytes;
      } else {
        var regdata = new short[this.words.Length];
        Array.Copy(this.words, regdata, this.words.Length);
        TwosComplement(regdata, 0, (int)regdata.Length);
        int byteCount = regdata.Length * 2;
        for (int i = regdata.Length - 1; i >= 0; --i) {
          if (regdata[i] == unchecked((short)0xffff)) {
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
        var bytes = new byte[byteCount];
        bytes[littleEndian ? bytes.Length - 1 : 0] = (byte)0xff;
        byteCount = Math.Min(byteCount, regdata.Length * 2);
        var j = 0;
        for (var i = 0; i < byteCount; i += 2, j++) {
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

    /// <summary>Shifts this object&#x27;s value by a number of bits. A
    /// value of 1 doubles this value, a value of 2 multiplies it by 4, a
    /// value of 3 by 8, a value of 4 by 16, and so on.</summary>
    /// <param name='numberBits'>The number of bits to shift. Can be
    /// negative, in which case this is the same as shiftRight with the
    /// absolute value of numberBits.</param>
    /// <returns>A BigInteger object.</returns>
    public BigInteger shiftLeft(int numberBits) {
      if (numberBits == 0 || this.wordCount == 0) {
        return this;
      }
      if (numberBits < 0) {
        return (numberBits == Int32.MinValue) ?
          this.shiftRight(1).shiftRight(Int32.MaxValue) :
          this.shiftRight(-numberBits);
      }
      var numWords = (int)this.wordCount;
      var shiftWords = (int)(numberBits >> 4);
      var shiftBits = (int)(numberBits & 15);
      if (!this.negative) {
        var ret = new short[RoundupSize(numWords +
                    BitsToWords((int)numberBits))];
        Array.Copy(this.words, 0, ret, shiftWords, numWords);
        ShiftWordsLeftByBits(
          ret,
          (int)shiftWords,
          numWords + BitsToWords(shiftBits),
          shiftBits);
        return new BigInteger(CountWords(ret, ret.Length), ret, false);
      } else {
        var ret = new short[RoundupSize(numWords +
                    BitsToWords((int)numberBits))];
        Array.Copy(this.words, ret, numWords);
        TwosComplement(ret, 0, (int)ret.Length);
        ShiftWordsLeftByWords(ret, 0, numWords + shiftWords, shiftWords);
        ShiftWordsLeftByBits(
          ret,
          (int)shiftWords,
          numWords + BitsToWords(shiftBits),
          shiftBits);
        TwosComplement(ret, 0, (int)ret.Length);
        return new BigInteger(CountWords(ret, ret.Length), ret, true);
      }
    }

    /// <summary>Returns a big integer with the bits shifted to the
    /// right.</summary>
    /// <param name='numberBits'>Number of bits to shift right.</param>
    /// <returns>A BigInteger object.</returns>
    public BigInteger shiftRight(int numberBits) {
      if (numberBits == 0 || this.wordCount == 0) {
        return this;
      }
      if (numberBits < 0) {
        return (numberBits == Int32.MinValue) ?
          this.shiftLeft(1).shiftLeft(Int32.MaxValue) :
          this.shiftLeft(-numberBits);
      }
      var numWords = (int)this.wordCount;
      var shiftWords = (int)(numberBits >> 4);
      var shiftBits = (int)(numberBits & 15);
      short[] ret;
      int retWordCount;
      if (this.negative) {
        ret = new short[this.words.Length];
        Array.Copy(this.words, ret, numWords);
        TwosComplement(ret, 0, (int)ret.Length);
        ShiftWordsRightByWordsSignExtend(ret, 0, numWords, shiftWords);
        if (numWords > shiftWords) {
          ShiftWordsRightByBitsSignExtend(
            ret,
            0,
            numWords - shiftWords,
            shiftBits);
        }
        TwosComplement(ret, 0, (int)ret.Length);
        retWordCount = ret.Length;
      } else {
        if (shiftWords >= numWords) {
          return BigInteger.Zero;
        }
        ret = new short[this.words.Length];
        Array.Copy(this.words, shiftWords, ret, 0, numWords - shiftWords);
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
        return BigInteger.Zero;
      }
      if (shiftWords > 2) {
        ret = ShortenArray(ret, retWordCount);
      }
      return new BigInteger(retWordCount, ret, this.negative);
    }

    /// <summary>Converts a 64-bit signed integer to a big
    /// integer.</summary>
    /// <param name='longerValue'>A 64-bit signed integer.</param>
    /// <returns>A BigInteger object with the same value as the 64-bit
    /// number.</returns>
    public static BigInteger valueOf(long longerValue) {
      if (longerValue == 0) {
        return BigInteger.Zero;
      }
      if (longerValue == 1) {
        return BigInteger.One;
      }
      short[] retreg;
      bool retnegative;
      int retwordcount;
      unchecked {
        retnegative = longerValue < 0;
        retreg = new short[4];
        if (longerValue == Int64.MinValue) {
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

    /// <summary>Converts this object's value to a 32-bit signed
    /// integer.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    /// <exception cref='OverflowException'>This object's value is too big
    /// to fit a 32-bit signed integer.</exception>
    public int intValueChecked() {
      int count = this.wordCount;
      if (count == 0) {
        return 0;
      }
      if (count > 2) {
        throw new OverflowException();
      }
      if (count == 2 && (this.words[1] & 0x8000) != 0) {
        if (this.negative && this.words[1] == unchecked((short)0x8000) &&
            this.words[0] == 0) {
          return Int32.MinValue;
        }
        throw new OverflowException();
      }
      return this.intValueUnchecked();
    }

    /// <summary>Converts this object's value to a 32-bit signed integer.
    /// If the value can't fit in a 32-bit integer, returns the lower 32
    /// bits of this object's two's complement representation (in which
    /// case the return value might have a different sign than this
    /// object's value).</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int intValueUnchecked() {
      var c = (int)this.wordCount;
      if (c == 0) {
        return 0;
      }
      int intRetValue = ((int)this.words[0]) & 0xffff;
      if (c > 1) {
        intRetValue |= (((int)this.words[1]) & 0xffff) << 16;
      }
      if (this.negative) {
        intRetValue = unchecked(intRetValue - 1);
        intRetValue = unchecked(~intRetValue);
      }
      return intRetValue;
    }

    /// <summary>Converts this object's value to a 64-bit signed
    /// integer.</summary>
    /// <returns>A 64-bit signed integer.</returns>
    /// <exception cref='OverflowException'>This object's value is too big
    /// to fit a 64-bit signed integer.</exception>
    public long longValueChecked() {
      int count = this.wordCount;
      if (count == 0) {
        return (long)0;
      }
      if (count > 4) {
        throw new OverflowException();
      }
      if (count == 4 && (this.words[3] & 0x8000) != 0) {
        if (this.negative && this.words[3] == unchecked((short)0x8000) &&
            this.words[2] == 0 && this.words[1] == 0 &&
            this.words[0] == 0) {
          return Int64.MinValue;
        }
        throw new OverflowException();
      }
      return this.longValueUnchecked();
    }

    /// <summary>Converts this object's value to a 64-bit signed integer.
    /// If the value can't fit in a 64-bit integer, returns the lower 64
    /// bits of this object's two's complement representation (in which
    /// case the return value might have a different sign than this
    /// object's value).</summary>
    /// <returns>A 64-bit signed integer.</returns>
    public long longValueUnchecked() {
      var c = (int)this.wordCount;
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
            intRetValue = unchecked(intRetValue - 1);
            intRetValue2 = unchecked(intRetValue2 - 1);
          } else {
            intRetValue = unchecked(intRetValue - 1);
          }
          intRetValue = unchecked(~intRetValue);
          intRetValue2 = unchecked(~intRetValue2);
        }
        ivv = ((long)intRetValue) & 0xFFFFFFFFL;
        ivv |= ((long)intRetValue2) << 32;
        return ivv;
      }
      ivv = ((long)intRetValue) & 0xFFFFFFFFL;
      if (this.negative) {
        ivv = -ivv;
      }
      return ivv;
    }

    /// <summary>Converts this object's value to a 32-bit signed
    /// integer.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    /// <exception cref='OverflowException'>This object's value is too big
    /// to fit a 32-bit signed integer.</exception>
  [Obsolete(
  "To make the conversion intention clearer use the 'intValueChecked' and 'intValueUnchecked' methods instead. Replace 'intValue' with 'intValueChecked' in your code." )]
    public int intValue() {
      return this.intValueChecked();
    }

    /// <summary>Returns whether this object's value can fit in a 32-bit
    /// signed integer.</summary>
    /// <returns>True if this object's value is MinValue or greater, and
    /// MaxValue or less; otherwise, false.</returns>
    public bool canFitInInt() {
      var c = (int)this.wordCount;
      if (c > 2) {
        return false;
      }
      if (c == 2 && (this.words[1] & 0x8000) != 0) {
        return this.negative && this.words[1] == unchecked((short)0x8000) &&
          this.words[0] == 0;
      }
      return true;
    }

    private bool HasSmallValue() {
      var c = (int)this.wordCount;
      if (c > 4) {
        return false;
      }
      if (c == 4 && (this.words[3] & 0x8000) != 0) {
        return this.negative && this.words[3] == unchecked((short)0x8000) &&
          this.words[2] == 0 && this.words[1] == 0 &&
          this.words[0] == 0;
      }
      return true;
    }

    /// <summary>Converts this object's value to a 64-bit signed
    /// integer.</summary>
    /// <returns>A 64-bit signed integer.</returns>
    /// <exception cref='OverflowException'>This object's value is too big
    /// to fit a 64-bit signed integer.</exception>
  [Obsolete(
  "To make the conversion intention clearer use the 'longValueChecked' and 'longValueUnchecked' methods instead. Replace 'longValue' with 'longValueChecked' in your code." )]
    public long longValue() {
      return this.longValueChecked();
    }

    /// <summary>Raises a big integer to a power, which is given as another
    /// big integer.</summary>
    /// <param name='power'>The exponent to raise to.</param>
    /// <returns>The result. Returns 1 if <paramref name='power'/> is
    /// 0.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='power'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='power'/> is less than 0.</exception>
  public BigInteger PowBigIntVar(BigInteger power) {
      if (power == null) {
        throw new ArgumentNullException("power");
      }
      int sign = power.Sign;
      if (sign < 0) {
        throw new ArgumentException(
          "sign (" + sign + ") is less than 0");
      }
      BigInteger thisVar = this;
      if (sign == 0) {
        return BigInteger.One;
      }
      if (power.Equals(BigInteger.One)) {
        return this;
      }
      if (power.wordCount == 1 && power.words[0] == 2) {
        return thisVar * (BigInteger)thisVar;
      }
      if (power.wordCount == 1 && power.words[0] == 3) {
        return (thisVar * (BigInteger)thisVar) * (BigInteger)thisVar;
      }
      BigInteger r = BigInteger.One;
      while (!power.IsZero) {
        if (!power.IsEven) {
          r *= (BigInteger)thisVar;
        }
        power >>= 1;
        if (!power.IsZero) {
          thisVar *= (BigInteger)thisVar;
        }
      }
      return r;
    }

    /// <summary>Raises a big integer to a power.</summary>
    /// <param name='powerSmall'>The exponent to raise to.</param>
    /// <returns>The result. Returns 1 if <paramref name='powerSmall'/> is
    /// 0.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='powerSmall'/> is less than 0.</exception>
    public BigInteger pow(int powerSmall) {
      if (powerSmall < 0) {
        throw new ArgumentException("powerSmall (" + powerSmall +
                    ") is less than 0");
      }
      BigInteger thisVar = this;
      if (powerSmall == 0) {
        // however 0 to the power of 0 is undefined
        return BigInteger.One;
      }
      if (powerSmall == 1) {
        return this;
      }
      if (powerSmall == 2) {
        return thisVar * (BigInteger)thisVar;
      }
      if (powerSmall == 3) {
        return (thisVar * (BigInteger)thisVar) * (BigInteger)thisVar;
      }
      BigInteger r = BigInteger.One;
      while (powerSmall != 0) {
        if ((powerSmall & 1) != 0) {
          r *= (BigInteger)thisVar;
        }
        powerSmall >>= 1;
        if (powerSmall != 0) {
          thisVar *= (BigInteger)thisVar;
        }
      }
      return r;
    }

    /// <summary>Gets the value of this object with the sign
    /// reversed.</summary>
    /// <returns>This object's value with the sign reversed.</returns>
    public BigInteger negate() {
      return this.wordCount == 0 ? this : new BigInteger(
        this.wordCount,
        this.words,
        !this.negative);
    }

    /// <summary>Returns the absolute value of this object's
    /// value.</summary>
    /// <returns>This object's value with the sign removed.</returns>
    public BigInteger abs() {
      return (this.wordCount == 0 || !this.negative) ? this : new
        BigInteger(this.wordCount, this.words, false);
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

    /// <summary>Finds the minimum number of bits needed to represent this
    /// object&#x27;s absolute value.</summary>
    /// <returns>The number of bits in this object's value. Returns 0 if
    /// this object's value is 0, and returns 1 if the value is negative
    /// 1.</returns>
    public int getUnsignedBitLength() {
      int wc = this.wordCount;
      if (wc != 0) {
        int numberValue = ((int)this.words[wc - 1]) & 0xffff;
        wc = (wc - 1) << 4;
        if (numberValue == 0) {
          return wc;
        }
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
          if ((numberValue >> 15) == 0) {
            --wc;
          }
        }
        return wc;
      }
      return 0;
    }

    /// <summary>Finds the minimum number of bits needed to represent this
    /// object&#x27;s value, except for its sign. If the value is negative,
    /// finds the number of bits in a value equal to this object's absolute
    /// value minus 1.</summary>
    /// <returns>The number of bits in this object's value. Returns 0 if
    /// this object's value is 0 or negative 1.</returns>
    public int bitLength() {
      int wc = this.wordCount;
      if (wc != 0) {
        if (this.negative) {
          return this.abs().subtract(BigInteger.One).bitLength();
        }
        int numberValue = ((int)this.words[wc - 1]) & 0xffff;
        wc = (wc - 1) << 4;
        if (numberValue == 0) {
          return wc;
        }
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
          return ((numberValue >> 15) == 0) ? wc - 1 : wc;
        }
      }
      return 0;
    }

    private static void ReverseChars(char[] chars, int offset, int length) {
      int half = length >> 1;
      int right = offset + length - 1;
      for (var i = 0; i < half; i++, right--) {
        char value = chars[offset + i];
        chars[offset + i] = chars[right];
        chars[right] = value;
      }
    }

    private string SmallValueToString() {
      long value = this.longValueChecked();
      if (value == Int64.MinValue) {
        return "-9223372036854775808";
      }
      bool neg = value < 0;
      var chars = new char[24];
      var count = 0;
      if (neg) {
        chars[0] = '-';
        ++count;
        value = -value;
      }
      while (value != 0) {
        char digit = Digits[(int)(value % 10)];
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
      unchecked {
        int p; short c; int d;
        p = bitlenLow * 0x84fb; d = ((int)p >> 16) & 0xffff; c = (short)d; d
          = ((int)d >> 16) & 0xffff;
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
        p = ((int)c) & 0xffff; c = (short)p; resultLow = c; c = (short)d; d
          = ((int)d >> 16) & 0xffff;
        p = bitlenHigh * 0x9a;
        p += ((int)c) & 0xffff;
        resultHigh = (short)p;
        int result = ((int)resultLow) & 0xffff;
        result |= (((int)resultHigh) & 0xffff) << 16;
        return (result & 0x7fffffff) >> 9;
      }
    }

    /// <summary>Finds the number of decimal digits this number
    /// has.</summary>
    /// <returns>The number of decimal digits. Returns 1 if this object' s
    /// value is 0.</returns>
    public int getDigitCount() {
      if (this.IsZero) {
        return 1;
      }
      if (this.HasSmallValue()) {
        long value = this.longValueChecked();
        if (value == Int64.MinValue) {
          return 19;
        }
        if (value < 0) {
          value = -value;
        }
        if (value >= 1000000000L) {
          return (value >= 1000000000000000000L) ? 19 : ((value >=
                   100000000000000000L) ? 18 : ((value >= 10000000000000000L) ?
                    17 : ((value >= 1000000000000000L) ? 16 :
                    ((value >= 100000000000000L) ? 15 : ((value
                    >= 10000000000000L) ?
                    14 : ((value >= 1000000000000L) ? 13 : ((value
                    >= 100000000000L) ? 12 : ((value >= 10000000000L) ?
                    11 : ((value >= 1000000000L) ? 10 : 9)))))))));
        } else {
          var v2 = (int)value;
          return (v2 >= 100000000) ? 9 : ((v2 >= 10000000) ? 8 : ((v2 >=
                    1000000) ? 7 : ((v2 >= 100000) ? 6 : ((v2
                    >= 10000) ? 5 : ((v2 >= 1000) ? 4 : ((v2 >= 100) ?
                    3 : ((v2 >= 10) ? 2 : 1)))))));
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
      var i = 0;
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
          var firstdigit = false;
          short[] dividend = tempReg ?? this.words;
          // Divide by 10000
          while ((wci--) > 0) {
            int curValue = ((int)dividend[wci]) & 0xffff;
            int currentDividend = unchecked((int)(curValue |
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
                Array.Copy(this.words, tempReg, tempReg.Length);
                // Use the calculated word count during division;
                // zeros that may have occurred in division
                // are not incorporated in the tempReg
                currentCount = wci + 1;
                tempReg[wci] = unchecked((short)quo);
              }
            } else {
              tempReg[wci] = unchecked((short)quo);
            }
            rem = currentDividend - (10000 * quo);
            remainderShort = unchecked((short)rem);
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

    private const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>Generates a string representing the value of this object,
    /// in the given radix.</summary>
    /// <param name='radix'>A radix from 2 through 36. For example, to
    /// generate a hexadecimal string, specify 16. To generate a decimal
    /// string, specify 10.</param>
    /// <returns>A string object.</returns>
    /// <exception cref='ArgumentException'>The parameter "index" is less
    /// than 0, "endIndex" is less than 0, or either is greater than the
    /// string's length, or "endIndex" is less than "index" ; or radix is
    /// less than 2 or greater than 36.</exception>
    public string toRadixString(int radix) {
      if (radix < 2) {
        throw new ArgumentException("radix (" + radix +
                    ") is less than 2");
      }
      if (radix > 36) {
        throw new ArgumentException("radix (" + radix +
                    ") is more than 36");
      }
      if (this.wordCount == 0) {
        return "0";
      }
      if (radix == 10) {
        // Decimal
        if (this.HasSmallValue()) {
          return this.SmallValueToString();
        }
        var tempReg = new short[this.wordCount];
        Array.Copy(this.words, tempReg, tempReg.Length);
        int numWordCount = tempReg.Length;
        while (numWordCount != 0 && tempReg[numWordCount - 1] == 0) {
          --numWordCount;
        }
        var i = 0;
        var s = new char[(numWordCount << 4) + 1];
        while (numWordCount != 0) {
          if (numWordCount == 1 && tempReg[0] > 0 && tempReg[0] <= 0x7fff) {
            int rest = tempReg[0];
            while (rest != 0) {
              // accurate approximation to rest/10 up to 43698,
              // and rest can go up to 32767
              int newrest = (rest * 26215) >> 18;
              s[i++] = Digits[rest - (newrest * 10)];
              rest = newrest;
            }
            break;
          }
          if (numWordCount == 2 && tempReg[1] > 0 && tempReg[1] <= 0x7fff) {
            int rest = ((int)tempReg[0]) & 0xffff;
            rest |= (((int)tempReg[1]) & 0xffff) << 16;
            while (rest != 0) {
              int newrest = rest / 10;
              s[i++] = Digits[rest - (newrest * 10)];
              rest = newrest;
            }
            break;
          } else {
            int wci = numWordCount;
            short remainderShort = 0;
            int quo, rem;
            // Divide by 10000
            while ((wci--) > 0) {
              int currentDividend = unchecked((int)((((int)tempReg[wci]) &
                    0xffff) | ((int)remainderShort << 16)));
              quo = currentDividend / 10000;
              tempReg[wci] = unchecked((short)quo);
              rem = currentDividend - (10000 * quo);
              remainderShort = unchecked((short)rem);
            }
            int remainderSmall = remainderShort;
            // Recalculate word count
            while (numWordCount != 0 && tempReg[numWordCount - 1] == 0) {
              --numWordCount;
            }
            // accurate approximation to rest/10 up to 16388,
            // and rest can go up to 9999
            int newrest = (remainderSmall * 3277) >> 15;
            s[i++] = Digits[(int)(remainderSmall - (newrest * 10))];
            remainderSmall = newrest;
            newrest = (remainderSmall * 3277) >> 15;
            s[i++] = Digits[(int)(remainderSmall - (newrest * 10))];
            remainderSmall = newrest;
            newrest = (remainderSmall * 3277) >> 15;
            s[i++] = Digits[(int)(remainderSmall - (newrest * 10))];
            remainderSmall = newrest;
            s[i++] = Digits[remainderSmall];
          }
        }
        ReverseChars(s, 0, i);
        if (this.negative) {
          var sb = new System.Text.StringBuilder(i + 1);
          sb.Append('-');
          sb.Append(s, 0, i);
          return sb.ToString();
        }
        return new String(s, 0, i);
      }
      if (radix == 16) {
        // Hex
        var sb = new System.Text.StringBuilder();
        if (this.negative) {
          sb.Append('-');
        }
        var firstBit = true;
        int word = this.words[this.wordCount - 1];
        for (int i = 0; i < 4; ++i) {
          if (!firstBit || (word & 0xf000) != 0) {
            sb.Append(Digits[(word >> 12) & 0x0f]);
            firstBit = false;
          }
          word <<= 4;
        }
        for (int j = this.wordCount - 2; j >= 0; --j) {
          word = this.words[j];
          for (int i = 0; i < 4; ++i) {
            sb.Append(Digits[(word >> 12) & 0x0f]);
            word <<= 4;
          }
        }
        return sb.ToString();
      }
      if (radix == 2) {
        // Binary
        var sb = new System.Text.StringBuilder();
        if (this.negative) {
          sb.Append('-');
        }
        var firstBit = true;
        int word = this.words[this.wordCount - 1];
        for (int i = 0; i < 16; ++i) {
          if (!firstBit || (word & 0x8000) != 0) {
            sb.Append((word & 0x8000) == 0 ? '0' : '1');
            firstBit = false;
          }
          word <<= 1;
        }
        for (int j = this.wordCount - 2; j >= 0; --j) {
          word = this.words[j];
          for (int i = 0; i < 16; ++i) {
            sb.Append((word & 0x8000) == 0 ? '0' : '1');
            word <<= 1;
          }
        }
        return sb.ToString();
      } else {
        // Other radixes
        var tempReg = new short[this.wordCount];
        Array.Copy(this.words, tempReg, tempReg.Length);
        int numWordCount = tempReg.Length;
        while (numWordCount != 0 && tempReg[numWordCount - 1] == 0) {
          --numWordCount;
        }
        var i = 0;
        var s = new char[(numWordCount << 4) + 1];
        while (numWordCount != 0) {
          if (numWordCount == 1 && tempReg[0] > 0 && tempReg[0] <= 0x7fff) {
            int rest = tempReg[0];
            while (rest != 0) {
              int newrest = rest / radix;
              s[i++] = Digits[rest - (newrest * radix)];
              rest = newrest;
            }
            break;
          }
          if (numWordCount == 2 && tempReg[1] > 0 && tempReg[1] <= 0x7fff) {
            int rest = ((int)tempReg[0]) & 0xffff;
            rest |= (((int)tempReg[1]) & 0xffff) << 16;
            while (rest != 0) {
              int newrest = rest / radix;
              s[i++] = Digits[rest - (newrest * radix)];
              rest = newrest;
            }
            break;
          } else {
            int wci = numWordCount;
            short remainderShort = 0;
            int quo, rem;
            // Divide by radix
            while ((wci--) > 0) {
              int currentDividend = unchecked((int)((((int)tempReg[wci]) &
                    0xffff) | ((int)remainderShort << 16)));
              quo = currentDividend / radix;
              tempReg[wci] = unchecked((short)quo);
              rem = currentDividend - (radix * quo);
              remainderShort = unchecked((short)rem);
            }
            int remainderSmall = remainderShort;
            // Recalculate word count
            while (numWordCount != 0 && tempReg[numWordCount - 1] == 0) {
              --numWordCount;
            }
            s[i++] = Digits[remainderSmall];
          }
        }
        ReverseChars(s, 0, i);
        if (this.negative) {
          var sb = new System.Text.StringBuilder(i + 1);
          sb.Append('-');
          sb.Append(s, 0, i);
          return sb.ToString();
        }
        return new String(s, 0, i);
      }
    }

    /// <summary>Converts this object to a text string in base
    /// 10.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      if (this.IsZero) {
        return "0";
      }
      return this.HasSmallValue() ? this.SmallValueToString() :
        this.toRadixString(10);
    }

    /// <summary>Converts a string to an arbitrary-precision
    /// integer.</summary>
    /// <param name='str'>A string containing only digits, except that it
    /// may start with a minus sign.</param>
    /// <returns>A BigInteger object with the same value as given in the
    /// string.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='FormatException'>The parameter <paramref
    /// name='str'/> is in an invalid format.</exception>
    public static BigInteger fromString(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      return fromRadixSubstring(str, 10, 0, str.Length);
    }

    /// <summary>Converts a string to an arbitrary-precision integer. The
    /// string portion can begin with a minus sign ('-') to indicate that
    /// it's negative.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='radix'>A base from 2 to 36. The possible digits start
    /// from 0 to 9, then from A to Z in base 36, and the possible digits
    /// start from 0 to 9, then from A to F in base 16.</param>
    /// <returns>A BigInteger object with the same value as given in the
    /// string.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='radix'/> is less than 2 or greater than 36.</exception>
    /// <exception cref='FormatException'>The string is empty or in an
    /// invalid format.</exception>
    public static BigInteger fromRadixString(string str, int radix) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      return fromRadixSubstring(str, radix, 0, str.Length);
    }

    /// <summary>Converts a portion of a string to an arbitrary-precision
    /// integer. The string portion can begin with a minus sign ('-') to
    /// indicate that it's negative.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>The index of the string that starts the string
    /// portion.</param>
    /// <param name='endIndex'>The index of the string that ends the string
    /// portion. The length will be index + endIndex - 1.</param>
    /// <returns>A BigInteger object with the same value as given in the
    /// string portion.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='index'/> is less than 0, <paramref name='endIndex'/> is less
    /// than 0, or either is greater than the string's length, or <paramref
    /// name='endIndex'/> is less than <paramref name='index'/>
    /// .</exception>
    /// <exception cref='FormatException'>The string portion is empty or in
    /// an invalid format.</exception>
    public static BigInteger fromSubstring(
      string str,
      int index,
      int endIndex) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      return fromRadixSubstring(str, 10, index, endIndex);
    }

    private static readonly int[] valueMaxSafeInts = { 1073741823, 715827881,
      536870911, 429496728, 357913940, 306783377, 268435455, 238609293,
      214748363, 195225785, 178956969, 165191048, 153391688, 143165575,
      134217727, 126322566, 119304646, 113025454, 107374181, 102261125,
      97612892, 93368853, 89478484, 85899344, 82595523, 79536430, 76695843,
      74051159, 71582787, 69273665, 67108863, 65075261, 63161282, 61356674,
      59652322 };

 private static readonly int[] valueCharToDigit = { 36, 36, 36, 36, 36, 36,
      36,
      36,
      36, 36, 36, 36, 36, 36, 36, 36,
      36, 36, 36, 36, 36, 36, 36, 36,
      36, 36, 36, 36, 36, 36, 36, 36,
      36, 36, 36, 36, 36, 36, 36, 36,
      36, 36, 36, 36, 36, 36, 36, 36,
      0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 36, 36, 36, 36, 36, 36,
      36, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
      25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 36, 36, 36, 36,
      36, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
      25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 36, 36, 36, 36 };

    /// <summary>Converts a portion of a string to an arbitrary-precision
    /// integer in a given radix. The string portion can begin with a minus
    /// sign ('-') to indicate that it's negative.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='radix'>A base from 2 to 36. The possible digits start
    /// from 0 to 9, then from A to Z in base 36, and the possible digits
    /// start from 0 to 9, then from A to F in base 16.</param>
    /// <param name='index'>The index of the string that starts the string
    /// portion.</param>
    /// <param name='endIndex'>The index of the string that ends the string
    /// portion. The length will be index + endIndex - 1.</param>
    /// <returns>A BigInteger object with the same value as given in the
    /// string portion.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='index'/> is less than 0, <paramref name='endIndex'/> is less
    /// than 0, or either is greater than the string's length, or <paramref
    /// name='endIndex'/> is less than <paramref name='index'/>
    /// .</exception>
    /// <exception cref='FormatException'>The string portion is empty or in
    /// an invalid format.</exception>
    public static BigInteger fromRadixSubstring(
      string str,
      int radix,
      int index,
      int endIndex) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (radix < 2) {
        throw new ArgumentException("radix (" + radix +
                    ") is less than 2");
      }
      if (radix > 36) {
        throw new ArgumentException("radix (" + radix +
                    ") is more than 36");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + index + ") is less than " +
                    "0");
      }
      if (index > str.Length) {
        throw new ArgumentException("index (" + index + ") is more than " +
                    str.Length);
      }
      if (endIndex < 0) {
        throw new ArgumentException("endIndex (" + endIndex +
                    ") is less than 0");
      }
      if (endIndex > str.Length) {
        throw new ArgumentException("endIndex (" + endIndex +
                    ") is more than " + str.Length);
      }
      if (endIndex < index) {
        throw new ArgumentException("endIndex (" + endIndex +
                    ") is less than " + index);
      }
      if (index == endIndex) {
        throw new FormatException("No digits");
      }
      var negative = false;
      if (str[index] == '-') {
        ++index;
        if (index == endIndex) {
          throw new FormatException("No digits");
        }
        negative = true;
      }
      // Skip leading zeros
      for (; index < endIndex; ++index) {
        char c = str[index];
        if (c != 0x30) {
          break;
        }
      }
      int effectiveLength = endIndex - index;
      if (effectiveLength == 0) {
        return BigInteger.ZERO;
      }
      short[] bigint;
      if (radix == 16) {
        // Special case for hexadecimal radix
        int leftover = effectiveLength & 3;
        int wordCount = effectiveLength >> 2;
        if (leftover != 0) {
          ++wordCount;
        }
        bigint = new short[wordCount + (wordCount & 1)];
        int currentDigit = wordCount - 1;
        // Get most significant digits if effective
        // length is not divisible by 4
        if (leftover != 0) {
          var extraWord = 0;
          for (int i = 0; i < leftover; ++i) {
            extraWord <<= 4;
            char c = str[index + i];
            int digit = (c >= 0x80) ? 36 : valueCharToDigit[(int)c];
            if (digit >= 16) {
              throw new FormatException("Illegal character found");
            }
            extraWord |= digit;
          }
          bigint[currentDigit] = unchecked((short)extraWord);
          --currentDigit;
          index += leftover;
        }
        #if DEBUG
        if ((endIndex - index) % 4 != 0) {
          throw new ArgumentException(
            "doesn't satisfy (endIndex - index) % 4 == 0");
        }
        #endif
        while (index < endIndex) {
          char c = str[index + 3];
          int digit = (c >= 0x80) ? 36 : valueCharToDigit[(int)c];
          if (digit >= 16) {
            throw new FormatException("Illegal character found");
          }
          int word = digit;
          c = str[index + 2];
          digit = (c >= 0x80) ? 36 : valueCharToDigit[(int)c];
          if (digit >= 16) {
            throw new FormatException("Illegal character found");
          }

          word |= digit << 4;
          c = str[index + 1];
          digit = (c >= 0x80) ? 36 : valueCharToDigit[(int)c];
          if (digit >= 16) {
            throw new FormatException("Illegal character found");
          }

          word |= digit << 8;
          c = str[index];
          digit = (c >= 0x80) ? 36 : valueCharToDigit[(int)c];
          if (digit >= 16) {
            throw new FormatException("Illegal character found");
          }
          word |= digit << 12;
          index += 4;
          bigint[currentDigit] = unchecked((short)word);
          --currentDigit;
        }
      } else {
        bigint = new short[4];
        var haveSmallInt = true;
        int maxSafeInt = valueMaxSafeInts[radix - 2];
        int maxShortPlusOneMinusRadix = 65536 - radix;
        var smallInt = 0;
        for (int i = index; i < endIndex; ++i) {
          char c = str[i];
          int digit = (c >= 0x80) ? 36 : valueCharToDigit[(int)c];
          if (digit >= radix) {
            throw new FormatException("Illegal character found");
          }
          if (haveSmallInt && smallInt < maxSafeInt) {
            smallInt *= radix;
            smallInt += digit;
          } else {
            if (haveSmallInt) {
              bigint[0] = unchecked((short)(smallInt & 0xffff));
              bigint[1] = unchecked((short)((smallInt >> 16) & 0xffff));
              haveSmallInt = false;
            }
            // Multiply by the radix
            short carry = 0;
            int n = bigint.Length;
            for (int j = 0; j < n; ++j) {
              int p;
              unchecked {
                p = (((int)bigint[j]) & 0xffff) * radix;
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
              if (d <= maxShortPlusOneMinusRadix) {
                bigint[0] = unchecked((short)(d + digit));
              } else if (Increment(bigint, 0, bigint.Length, (short)digit) !=
                    0) {
                bigint = GrowForCarry(bigint, (short)1);
              }
            }
          }
        }
        if (haveSmallInt) {
          bigint[0] = unchecked((short)(smallInt & 0xffff));
          bigint[1] = unchecked((short)((smallInt >> 16) & 0xffff));
        }
      }
      int count = CountWords(bigint, bigint.Length);
      return (count == 0) ? BigInteger.Zero : new BigInteger(
        count,
        bigint,
        negative);
    }

    /// <summary>See <c>getLowBit()</c></summary>
    /// <returns>See getLowBit().</returns>
    [Obsolete("Renamed to getLowBit.")]
    public int getLowestSetBit() {
      return this.getLowBit();
    }

    /// <summary>Gets the lowest set bit in this number's absolute
    /// value.</summary>
    /// <returns>The lowest bit set in the number, starting at 0. Returns 0
    /// if this value is 0 or odd. (NOTE: In future versions, may return -1
    /// instead if this value is 0.).</returns>
    public int getLowBit() {
      var retSetBit = 0;
      for (var i = 0; i < this.wordCount; ++i) {
        short c = this.words[i];
        if (c == (short)0) {
          retSetBit += 16;
        } else {
          return (((c << 15) & 0xffff) != 0) ? (retSetBit + 0) : ((((c <<
                    14) & 0xffff) != 0) ? (retSetBit + 1) : ((((c <<
                    13) & 0xffff) != 0) ? (retSetBit + 2) : ((((c <<
                    12) & 0xffff) != 0) ? (retSetBit + 3) : ((((c << 11) &
                    0xffff) != 0) ? (retSetBit +
                    4) : ((((c << 10) & 0xffff) != 0) ? (retSetBit +
                    5) : ((((c << 9) & 0xffff) != 0) ? (retSetBit + 6) :
                    ((((c <<
                8) & 0xffff) != 0) ? (retSetBit + 7) : ((((c << 7) & 0xffff) !=
                    0) ? (retSetBit + 8) : ((((c << 6) & 0xffff) !=
                    0) ? (retSetBit + 9) : ((((c <<
                    5) & 0xffff) != 0) ? (retSetBit + 10) : ((((c <<
                    4) & 0xffff) != 0) ? (retSetBit + 11) : ((((c << 3) &
                    0xffff) != 0) ? (retSetBit + 12) : ((((c << 2) & 0xffff) !=
                    0) ? (retSetBit + 13) : ((((c << 1) & 0xffff) !=
                    0) ? (retSetBit + 14) : (retSetBit + 15)))))))))))))));
        }
      }
      return 0;
    }

    /// <summary>Returns the greatest common divisor of two integers. The
    /// greatest common divisor (GCD) is also known as the greatest common
    /// factor (GCF).</summary>
    /// <param name='bigintSecond'>Another BigInteger object.</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintSecond'/> is null.</exception>
    public BigInteger gcd(BigInteger bigintSecond) {
      if (bigintSecond == null) {
        throw new ArgumentNullException("bigintSecond");
      }
      if (this.IsZero) {
        return BigInteger.Abs(bigintSecond);
      }
      if (bigintSecond.IsZero) {
        return BigInteger.Abs(this);
      }
      BigInteger thisValue = this.abs();
      bigintSecond = bigintSecond.abs();
      if (bigintSecond.Equals(BigInteger.One) ||
          thisValue.Equals(bigintSecond)) {
        return bigintSecond;
      }
      if (thisValue.Equals(BigInteger.One)) {
        return thisValue;
      }
      if (thisValue.wordCount <= 10 && bigintSecond.wordCount <= 10) {
        int expOfTwo = Math.Min(
          thisValue.getLowBit(),
          bigintSecond.getLowBit());
        while (true) {
          BigInteger bigintA = (thisValue - (BigInteger)bigintSecond).abs();
          if (bigintA.IsZero) {
            if (expOfTwo != 0) {
              thisValue <<= expOfTwo;
            }
            return thisValue;
          }
          int setbit = bigintA.getLowBit();
          bigintA >>= setbit;
          bigintSecond = (thisValue.CompareTo(bigintSecond) < 0) ? thisValue :
            bigintSecond;
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
          thisValue %= (BigInteger)bigintSecond;
        }
        return bigintSecond;
      }
    }

    /// <summary>Calculates the remainder when a BigInteger raised to a
    /// certain power is divided by another BigInteger.</summary>
    /// <param name='pow'>Another BigInteger object.</param>
    /// <param name='mod'>A BigInteger object. (3).</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='pow'/> or <paramref name='mod'/> is null.</exception>
    public BigInteger ModPow(BigInteger pow, BigInteger mod) {
      if (pow == null) {
        throw new ArgumentNullException("pow");
      }
      if (mod == null) {
        throw new ArgumentNullException("mod");
      }
      if (pow.Sign < 0) {
        throw new ArgumentException("pow (" + pow + ") is less than 0");
      }
      if (mod.Sign <= 0) {
        throw new ArgumentException("mod (" + mod + ") is not greater than 0");
      }
      BigInteger r = BigInteger.One;
      BigInteger v = this;
      while (!pow.IsZero) {
        if (!pow.IsEven) {
          r = (r * (BigInteger)v).mod(mod);
        }
        pow >>= 1;
        if (!pow.IsZero) {
          v = (v * (BigInteger)v).mod(mod);
        }
      }
      return r;
    }

    #region Equals and GetHashCode implementation
    /// <inheritdoc/>
    /// <summary>Determines whether this object and another object are
    /// equal.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>True if this object and another object are equal;
    /// otherwise, false.</returns>
    public override bool Equals(object obj) {
      var other = obj as BigInteger;
      if (other == null) {
        return false;
      }
      if (this.wordCount == other.wordCount) {
        if (this.negative != other.negative) {
          return false;
        }
        for (var i = 0; i < this.wordCount; ++i) {
          if (this.words[i] != other.words[i]) {
            return false;
          }
        }
        return true;
      }
      return false;
    }

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public override int GetHashCode() {
      var hashCodeValue = 0;
      unchecked {
        hashCodeValue += 1000000007 * this.Sign.GetHashCode();
        if (this.words != null) {
          for (var i = 0; i < this.wordCount; ++i) {
            hashCodeValue += 1000000013 * this.words[i];
          }
        }
      }
      return hashCodeValue;
    }
    #endregion

    /// <summary>Adds this object and another object.</summary>
    /// <param name='bigintAugend'>Another BigInteger object.</param>
    /// <returns>The sum of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintAugend'/> is null.</exception>
    public BigInteger add(BigInteger bigintAugend) {
      if (bigintAugend == null) {
        throw new ArgumentNullException("bigintAugend");
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
          int intSum = (((int)this.words[0]) & 0xffff) +
            (((int)bigintAugend.words[0]) & 0xffff);
          sumreg = new short[2];
          sumreg[0] = unchecked((short)intSum);
          sumreg[1] = unchecked((short)(intSum >> 16));
          return new BigInteger(
            ((intSum >> 16) == 0) ? 1 : 2,
            sumreg,
            this.negative);
        } else {
          int a = ((int)this.words[0]) & 0xffff;
          int b = ((int)bigintAugend.words[0]) & 0xffff;
          if (a == b) {
            return BigInteger.Zero;
          }
          if (a > b) {
            a -= b;
            sumreg = new short[2];
            sumreg[0] = unchecked((short)a);
            return new BigInteger(1, sumreg, this.negative);
          }
          b -= a;
          sumreg = new short[2];
          sumreg[0] = unchecked((short)b);
          return new BigInteger(1, sumreg, !this.negative);
        }
      }
      if ((!this.negative) == (!bigintAugend.negative)) {
        sumreg = new short[(
          int)Math.Max(
                    this.words.Length,
                    bigintAugend.words.Length)];
        // both nonnegative or both negative
        int carry;
        int addendCount = this.wordCount;
        int augendCount = bigintAugend.wordCount;
        int desiredLength = Math.Max(addendCount, augendCount);
        if (addendCount == augendCount) {
          carry = AddOneByOne(
            sumreg,
            0,
            this.words,
            0,
            bigintAugend.words,
            0,
            addendCount);
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
          Array.Copy(
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
          Array.Copy(
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
        var needShorten = true;
        if (carry != 0) {
          int nextIndex = desiredLength;
          int len = RoundupSize(nextIndex + 1);
          sumreg = CleanGrow(sumreg, len);
          sumreg[nextIndex] = (short)carry;
          needShorten = false;
        }
        int sumwordCount = CountWords(sumreg, sumreg.Length);
        if (sumwordCount == 0) {
          return BigInteger.Zero;
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
      var diffNeg = false;
      short borrow;
      var diffReg = new short[(
        int)Math.Max(
                    minuend.words.Length,
                    subtrahend.words.Length)];
      if (words1Size == words2Size) {
        if (Compare(minuend.words, 0, subtrahend.words, 0, (int)words1Size) >=
            0) {
          // words1 is at least as high as words2
          Subtract(
            diffReg,
            0,
            minuend.words,
            0,
            subtrahend.words,
            0,
            words1Size);
        } else {
          // words1 is less than words2
          Subtract(
            diffReg,
            0,
            subtrahend.words,
            0,
            minuend.words,
            0,
            words1Size);
          diffNeg = true;  // difference will be negative
        }
      } else if (words1Size > words2Size) {
        // words1 is greater than words2
        borrow = (
          short)Subtract(
          diffReg,
          0,
          minuend.words,
          0,
          subtrahend.words,
          0,
          words2Size);
        Array.Copy(
          minuend.words,
          words2Size,
          diffReg,
          words2Size,
          words1Size - words2Size);
        Decrement(diffReg, words2Size, (int)(words1Size - words2Size), borrow);
      } else {
        // words1 is less than words2
        borrow = (
          short)Subtract(
          diffReg,
          0,
          subtrahend.words,
          0,
          minuend.words,
          0,
          words1Size);
        Array.Copy(
          subtrahend.words,
          words1Size,
          diffReg,
          words1Size,
          words2Size - words1Size);
        Decrement(diffReg, words1Size, (int)(words2Size - words1Size), borrow);
        diffNeg = true;
      }
      int count = CountWords(diffReg, diffReg.Length);
      if (count == 0) {
        return BigInteger.Zero;
      }
      diffReg = ShortenArray(diffReg, count);
      return new BigInteger(count, diffReg, diffNeg);
    }

    /// <summary>Subtracts a BigInteger from this BigInteger.</summary>
    /// <param name='subtrahend'>Another BigInteger object.</param>
    /// <returns>The difference of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='subtrahend'/> is null.</exception>
    public BigInteger subtract(BigInteger subtrahend) {
      if (subtrahend == null) {
        throw new ArgumentNullException("subtrahend");
      }
      return (this.wordCount == 0) ? subtrahend.negate() :
        ((subtrahend.wordCount == 0) ? this : this.add(subtrahend.negate()));
    }

    private static short[] ShortenArray(short[] reg, int wordCount) {
      if (reg.Length > 32) {
        int newLength = RoundupSize(wordCount);
        if (newLength < reg.Length && (reg.Length - newLength) >= 16) {
          // Reallocate the array if the rounded length
          // is much smaller than the current length
          var newreg = new short[newLength];
          Array.Copy(reg, newreg, Math.Min(newLength, reg.Length));
          reg = newreg;
        }
      }
      return reg;
    }

    /// <summary>Multiplies this instance by the value of a BigInteger
    /// object.</summary>
    /// <param name='bigintMult'>Another BigInteger object.</param>
    /// <returns>The product of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintMult'/> is null.</exception>
    public BigInteger multiply(BigInteger bigintMult) {
      if (bigintMult == null) {
        throw new ArgumentNullException("bigintMult");
      }
      if (this.wordCount == 0 || bigintMult.wordCount == 0) {
        return BigInteger.Zero;
      }
      if (this.wordCount == 1 && this.words[0] == 1) {
        return this.negative ? bigintMult.negate() : bigintMult;
      }
      if (bigintMult.wordCount == 1 && bigintMult.words[0] == 1) {
        return bigintMult.negative ? this.negate() : this;
      }
      short[] productreg;
      int productwordCount;
      var needShorten = true;
      if (this.wordCount == 1) {
        int wc = bigintMult.wordCount;
        int regLength = RoundupSize(wc + 1);
        productreg = new short[regLength];
        productreg[wc] = LinearMultiply(
          productreg,
          0,
          bigintMult.words,
          0,
          this.words[0],
          wc);
        productwordCount = productreg.Length;
        needShorten = false;
      } else if (bigintMult.wordCount == 1) {
        int wc = this.wordCount;
        int regLength = RoundupSize(wc + 1);
        productreg = new short[regLength];
        productreg[wc] = LinearMultiply(
          productreg,
          0,
          this.words,
          0,
          bigintMult.words[0],
          wc);
        productwordCount = productreg.Length;
        needShorten = false;
      } else if (this.Equals(bigintMult)) {
        int words1Size = RoundupSize(this.wordCount);
        productreg = new short[words1Size + words1Size];
        productwordCount = productreg.Length;
        var workspace = new short[words1Size + words1Size];
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
        productwordCount = productreg.Length;
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
        var workspace = new short[words1Size + words2Size];
        productwordCount = productreg.Length;
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
      return new BigInteger(
        productwordCount,
        productreg,
        this.negative ^ bigintMult.negative);
    }

    private static int BitsToWords(int bitCount) {
      return (bitCount + 15) >> 4;
    }

    private static short FastRemainder(
      short[] dividendReg,
      int count,
      short divisorSmall) {
      int i = count;
      short remainder = 0;
      while ((i--) > 0) {
        remainder = RemainderUnsigned(
          MakeUint(dividendReg[i], remainder),
          divisorSmall);
      }
      return remainder;
    }

    private static void FastDivide(
      short[] quotientReg,
      short[] dividendReg,
      int count,
      short divisorSmall) {
      int i = count;
      short remainderShort = 0;
      int idivisor = ((int)divisorSmall) & 0xffff;
      int quo, rem;
      while ((i--) > 0) {
        int currentDividend = unchecked((int)((((int)dividendReg[i]) & 0xffff) |
                    ((int)remainderShort << 16)));
        if ((currentDividend >> 31) == 0) {
          quo = currentDividend / idivisor;
          quotientReg[i] = unchecked((short)quo);
          if (i > 0) {
            rem = currentDividend - (idivisor * quo);
            remainderShort = unchecked((short)rem);
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
        int currentDividend =
          unchecked((int)((((int)dividendReg[dividendStart + i]) & 0xffff) |
                    ((int)remainderShort << 16)));
        if ((currentDividend >> 31) == 0) {
          quo = currentDividend / idivisor;
          quotientReg[quotientStart + i] = unchecked((short)quo);
          rem = currentDividend - (idivisor * quo);
          remainderShort = unchecked((short)rem);
        } else {
          quotientReg[quotientStart + i] = DivideUnsigned(
            currentDividend,
            divisorSmall);
          remainderShort = RemainderUnsigned(currentDividend, divisorSmall);
        }
      }
      return remainderShort;
    }

    /// <summary>Divides this instance by the value of a BigInteger object.
    /// The result is rounded down (the fractional part is discarded).
    /// Except if the result is 0, it will be negative if this object is
    /// positive and the other is negative, or vice versa, and will be
    /// positive if both are positive or both are negative.</summary>
    /// <param name='bigintDivisor'>Another BigInteger object.</param>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='DivideByZeroException'>The divisor is
    /// zero.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigintDivisor'/> is null.</exception>
    /// <exception cref='System.DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    public BigInteger divide(BigInteger bigintDivisor) {
      if (bigintDivisor == null) {
        throw new ArgumentNullException("bigintDivisor");
      }
      int words1Size = this.wordCount;
      int words2Size = bigintDivisor.wordCount;
      // ---- Special cases
      if (words2Size == 0) {
        throw new DivideByZeroException();
      }
      if (words1Size < words2Size) {
        // dividend is less than divisor (includes case
        // where dividend is 0)
        return BigInteger.Zero;
      }
      if (words1Size <= 2 && words2Size <= 2 && this.canFitInInt() &&
          bigintDivisor.canFitInInt()) {
        int valueASmall = this.intValueChecked();
        int valueBSmall = bigintDivisor.intValueChecked();
        if (valueASmall != Int32.MinValue || valueBSmall != -1) {
          int result = valueASmall / valueBSmall;
          return BigInteger.valueOf(result);
        }
      }
      short[] quotReg;
      int quotwordCount;
      if (words2Size == 1) {
        // divisor is small, use a fast path
        quotReg = new short[this.words.Length];
        quotwordCount = this.wordCount;
        FastDivide(quotReg, this.words, words1Size, bigintDivisor.words[0]);
        while (quotwordCount != 0 && quotReg[quotwordCount - 1] == 0) {
          --quotwordCount;
        }
        return (
          quotwordCount != 0) ? (
          new BigInteger(
            quotwordCount,
            quotReg,
            this.negative ^ bigintDivisor.negative)) : BigInteger.Zero;
      }
      // ---- General case
      words1Size += words1Size & 1;
      words2Size += words2Size & 1;
      quotReg = new short[RoundupSize((int)(words1Size - words2Size + 2))];
      var tempbuf = new short[words1Size + (3 * (words2Size + 2))];
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
      quotwordCount = CountWords(quotReg, quotReg.Length);
      quotReg = ShortenArray(quotReg, quotwordCount);
      return (
        quotwordCount != 0) ? (
        new BigInteger(
          quotwordCount,
          quotReg,
          this.negative ^ bigintDivisor.negative)) : BigInteger.Zero;
    }

    /// <summary>Divides this object by another big integer and returns the
    /// quotient and remainder.</summary>
    /// <param name='divisor'>A BigInteger object.</param>
    /// <returns>An array with two big integers: the first is the quotient,
    /// and the second is the remainder.</returns>
    /// <exception cref='ArgumentNullException'>The parameter divisor is
    /// null.</exception>
    /// <exception cref='DivideByZeroException'>The parameter divisor is
    /// 0.</exception>
    /// <exception cref='System.DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    public BigInteger[] divideAndRemainder(BigInteger divisor) {
      if (divisor == null) {
        throw new ArgumentNullException("divisor");
      }
      int words1Size = this.wordCount;
      int words2Size = divisor.wordCount;
      if (words2Size == 0) {
        throw new DivideByZeroException();
      }

      if (words1Size < words2Size) {
        // dividend is less than divisor (includes case
        // where dividend is 0)
        return new[] { BigInteger.Zero, this };
      }
      if (words2Size == 1) {
        // divisor is small, use a fast path
        var quotient = new short[this.words.Length];
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
          return new[] { BigInteger.Zero, this };
        }
        quotient = ShortenArray(quotient, count);
        var bigquo = new BigInteger(
          count,
          quotient,
          this.negative ^ divisor.negative);
        if (this.negative) {
          smallRemainder = -smallRemainder;
        }
        return new[] { bigquo, BigInteger.valueOf(smallRemainder) };
      }
      if (this.wordCount == 2 && divisor.wordCount == 2 &&
          (this.words[1] >> 15) != 0 && (divisor.words[1] >> 15) != 0) {
        int a = ((int)this.words[0]) & 0xffff;
        int b = ((int)divisor.words[0]) & 0xffff;
        unchecked {
          a |= (((int)this.words[1]) & 0xffff) << 16;
          b |= (((int)divisor.words[1]) & 0xffff) << 16;
          int quo = a / b;
          if (this.negative) {
            quo = -quo;
          }
          int rem = a - (b * quo);
          var quotAndRem = new BigInteger[2];
          quotAndRem[0] = BigInteger.valueOf(quo);
          quotAndRem[1] = BigInteger.valueOf(rem);
          return quotAndRem;
        }
      }
      words1Size += words1Size & 1;
      words2Size += words2Size & 1;
      var bigRemainderreg = new short[RoundupSize((int)words2Size)];
      var quotientreg = new short[RoundupSize((int)(words1Size - words2Size +
                    2))];
      var tempbuf = new short[words1Size + (3 * (words2Size + 2))];
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
      int remCount = CountWords(bigRemainderreg, bigRemainderreg.Length);
      int quoCount = CountWords(quotientreg, quotientreg.Length);
      bigRemainderreg = ShortenArray(bigRemainderreg, remCount);
      quotientreg = ShortenArray(quotientreg, quoCount);
      BigInteger bigrem = (remCount == 0) ? BigInteger.Zero : new
        BigInteger(remCount, bigRemainderreg, this.negative);
      BigInteger bigquo2 = (quoCount == 0) ? BigInteger.Zero : new
        BigInteger(quoCount, quotientreg, this.negative ^ divisor.negative);
      return new[] { bigquo2, bigrem };
    }

    /// <summary>Finds the modulus remainder that results when this
    /// instance is divided by the value of a BigInteger object. The
    /// modulus remainder is the same as the normal remainder if the normal
    /// remainder is positive, and equals divisor plus normal remainder if
    /// the normal remainder is negative.</summary>
    /// <param name='divisor'>A divisor greater than 0 (the
    /// modulus).</param>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='ArithmeticException'>The parameter <paramref
    /// name='divisor'/> is negative.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    public BigInteger mod(BigInteger divisor) {
      if (divisor == null) {
        throw new ArgumentNullException("divisor");
      }
      if (divisor.Sign < 0) {
        throw new ArithmeticException("Divisor is negative");
      }
      BigInteger rem = this.remainder(divisor);
      if (rem.Sign < 0) {
        rem = divisor.add(rem);
      }
      return rem;
    }

    /// <summary>Finds the remainder that results when this instance is
    /// divided by the value of a BigInteger object. The remainder is the
    /// value that remains when the absolute value of this object is
    /// divided by the absolute value of the other object; the remainder
    /// has the same sign (positive or negative) as this object.</summary>
    /// <param name='divisor'>Another BigInteger object.</param>
    /// <returns>The remainder of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='divisor'/> is null.</exception>
    /// <exception cref='System.DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    public BigInteger remainder(BigInteger divisor) {
      if (divisor == null) {
        throw new ArgumentNullException("divisor");
      }
      int words1Size = this.wordCount;
      int words2Size = divisor.wordCount;
      if (words2Size == 0) {
        throw new DivideByZeroException();
      }
      if (words1Size < words2Size) {
        // dividend is less than divisor
        return this;
      }
      if (words2Size == 1) {
        short shortRemainder = FastRemainder(
          this.words,
          this.wordCount,
          divisor.words[0]);
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
      var remainderReg = new short[RoundupSize((int)words2Size)];
      var tempbuf = new short[words1Size + (3 * (words2Size + 2))];
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
      int count = CountWords(remainderReg, remainderReg.Length);
      if (count == 0) {
        return BigInteger.Zero;
      }
      remainderReg = ShortenArray(remainderReg, count);
      return new BigInteger(count, remainderReg, this.negative);
    }

    private int PositiveCompare(BigInteger t) {
      int size = this.wordCount, tempSize = t.wordCount;
      return (
        size == tempSize) ? Compare(
        this.words,
        0,
        t.words,
        0,
        (int)size) : (size > tempSize ? 1 : -1);
    }

    /// <summary>Compares a BigInteger object with this instance.</summary>
    /// <param name='other'>A BigInteger object.</param>
    /// <returns>Zero if the values are equal; a negative number if this
    /// instance is less, or a positive number if this instance is
    /// greater.</returns>
    public int CompareTo(BigInteger other) {
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
          while (unchecked(size--) != 0) {
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

    /// <summary>Gets the sign of this object's value.</summary>
    /// <value>0 if this value is zero; -1 if this value is negative, or 1
    /// if this value is positive.</value>
    public int Sign {
      get {
        return (this.wordCount == 0) ? 0 : (this.negative ? -1 : 1);
      }
    }

    /// <summary>Gets a value indicating whether this value is 0.</summary>
    /// <value>True if this value is 0; otherwise, false.</value>
    public bool IsZero {
      get {
        return this.wordCount == 0;
      }
    }

    /// <summary>Finds the square root of this instance&#x27;s value,
    /// rounded down.</summary>
    /// <returns>The square root of this object's value. Returns 0 if this
    /// value is 0 or less.</returns>
    public BigInteger sqrt() {
      BigInteger[] srrem = this.sqrtWithRemainder();
      return srrem[0];
    }

    /// <summary>Calculates the square root and the remainder.</summary>
    /// <returns>An array of two big integers: the first integer is the
    /// square root, and the second is the difference between this value
    /// and the square of the first integer. Returns two zeros if this
    /// value is 0 or less, or one and zero if this value equals
    /// 1.</returns>
    public BigInteger[] sqrtWithRemainder() {
      if (this.Sign <= 0) {
        return new[] { BigInteger.Zero, BigInteger.Zero };
      }
      if (this.Equals(BigInteger.One)) {
        return new[] { BigInteger.One, BigInteger.Zero };
      }
      BigInteger bigintX;
      BigInteger bigintY;
      BigInteger thisValue = this;
      int powerBits = (thisValue.getUnsignedBitLength() + 1) / 2;
      if (thisValue.canFitInInt()) {
        int smallValue = thisValue.intValueChecked();
        // No need to check for zero; already done above
        var smallintX = 0;
        int smallintY = 1 << powerBits;
        do {
          smallintX = smallintY;
          smallintY = smallValue / smallintX;
          smallintY += smallintX;
          smallintY >>= 1;
        } while (smallintY < smallintX);
        smallintY = smallintX * smallintX;
        smallintY = smallValue - smallintY;
        return new[] {
          (BigInteger)smallintX, (BigInteger)smallintY
        };
      }
      bigintX = BigInteger.Zero;
      bigintY = BigInteger.One << powerBits;
      do {
        bigintX = bigintY;
        bigintY = thisValue / (BigInteger)bigintX;
        bigintY += bigintX;
        bigintY >>= 1;
      } while (bigintY != null && bigintY.CompareTo(bigintX) < 0);
      bigintY = bigintX * (BigInteger)bigintX;
      bigintY = thisValue - (BigInteger)bigintY;
      return new[] {
        bigintX, bigintY
      };
    }

    /// <summary>Gets a value indicating whether this value is
    /// even.</summary>
    /// <value>True if this value is even; otherwise, false.</value>
    public bool IsEven {
      get {
        return !this.GetUnsignedBit(0);
      }
    }

    /// <summary>BigInteger object for the number zero.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "BigInteger is immutable")]
    #endif
    public static readonly BigInteger ZERO = new BigInteger(
      0, new short[] { 0, 0 }, false);

    /// <summary>BigInteger object for the number one.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "BigInteger is immutable")]
    #endif

    public static readonly BigInteger ONE = new BigInteger(
      1, new short[] { 1, 0 }, false);

    /// <summary>BigInteger object for the number ten.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "BigInteger is immutable")]
    #endif

    public static readonly BigInteger TEN = BigInteger.valueOf(10);
  }
}
