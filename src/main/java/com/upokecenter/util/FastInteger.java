package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  final class FastInteger implements Comparable<FastInteger> {
    private static final class MutableNumber {
      private int[] data;
      private int wordCount;

      static MutableNumber FromBigInteger(BigInteger bigintVal) {
        MutableNumber mnum = new MutableNumber(0);
        if (bigintVal.signum() < 0) {
          throw new IllegalArgumentException("bigintVal's sign (" + bigintVal.signum() +
            ") is less than " + "0 ");
        }
        byte[] bytes = bigintVal.toByteArray(true);
        int len = bytes.length;
        int newWordCount = Math.max(4, (len / 4) + 1);
        if (newWordCount > mnum.data.length) {
          mnum.data = new int[newWordCount];
        }
        mnum.wordCount = newWordCount;
        {
          for (int i = 0; i < len; i += 4) {
            int x = ((int)bytes[i]) & 0xff;
            if (i + 1 < len) {
              x |= (((int)bytes[i + 1]) & 0xff) << 8;
            }
            if (i + 2 < len) {
              x |= (((int)bytes[i + 2]) & 0xff) << 16;
            }
            if (i + 3 < len) {
              x |= (((int)bytes[i + 3]) & 0xff) << 24;
            }
            mnum.data[i >> 2] = x;
          }
        }
        // Calculate the correct data length
        while (mnum.wordCount != 0 && mnum.data[mnum.wordCount - 1] == 0) {
          --mnum.wordCount;
        }
        return mnum;
      }

      MutableNumber(int val) {
        if (val < 0) {
          throw new IllegalArgumentException("val (" + val + ") is less than " + "0 ");
        }
        this.data = new int[4];
        this.wordCount = (val == 0) ? 0 : 1;
        this.data[0] = ((int)(val & 0xFFFFFFFFL));
      }

      MutableNumber SetInt(int val) {
        if (val < 0) {
          throw new IllegalArgumentException("val (" + val + ") is less than " + "0 ");
        }
        this.wordCount = (val == 0) ? 0 : 1;
        this.data[0] = ((int)(val & 0xFFFFFFFFL));
        return this;
      }

      BigInteger ToBigInteger() {
        if (this.wordCount == 1 && (this.data[0] >> 31) == 0) {
          return BigInteger.valueOf((int)this.data[0]);
        }
        byte[] bytes = new byte[(this.wordCount * 4) + 1];
        for (int i = 0; i < this.wordCount; ++i) {
          bytes[i * 4] = (byte)(this.data[i] & 0xff);
          bytes[(i * 4) + 1] = (byte)((this.data[i] >> 8) & 0xff);
          bytes[(i * 4) + 2] = (byte)((this.data[i] >> 16) & 0xff);
          bytes[(i * 4) + 3] = (byte)((this.data[i] >> 24) & 0xff);
        }
        bytes[bytes.length - 1] = (byte)0;
        return BigInteger.fromByteArray(bytes, true);
      }

      int[] GetLastWordsInternal(int numWords32Bit) {
        int[] ret = new int[numWords32Bit];
        System.arraycopy(this.data, 0, ret, 0, Math.min(numWords32Bit, this.wordCount));
        return ret;
      }

      boolean CanFitInInt32() {
        return this.wordCount == 0 || (this.wordCount == 1 && (this.data[0] >>
        31) == 0);
      }

      int ToInt32() {
        return this.wordCount == 0 ? 0 : this.data[0];
      }

      MutableNumber Copy() {
        MutableNumber mbi = new MutableNumber(0);
        if (this.wordCount > mbi.data.length) {
          mbi.data = new int[this.wordCount];
        }
        System.arraycopy(this.data, 0, mbi.data, 0, this.wordCount);
        mbi.wordCount = this.wordCount;
        return mbi;
      }

    /**
     * Multiplies this instance by the value of a 32-bit signed integer.
     * @param multiplicand A 32-bit signed integer.
     * @return The product of the two objects.
     */
      MutableNumber Multiply(int multiplicand) {
        if (multiplicand < 0) {
          throw new IllegalArgumentException("multiplicand (" + multiplicand +
            ") is less than " + "0 ");
        }
        if (multiplicand != 0) {
          int carry = 0;
          if (this.wordCount == 0) {
            if (this.data.length == 0) {
              this.data = new int[4];
            }
            this.data[0] = 0;
            this.wordCount = 1;
          }
          int result0, result1, result2, result3;
          if (multiplicand < 65536) {
            for (int i = 0; i < this.wordCount; ++i) {
              int x0 = this.data[i];
              int x1 = x0;
              int y0 = multiplicand;
              x0 &= 65535;
              x1 = (x1 >> 16) & 65535;
              int temp = (x0 * y0);  // a * c
              result1 = (temp >> 16) & 65535;
              result0 = temp & 65535;
              result2 = 0;
              temp = (x1 * y0);  // b * c
              result2 += (temp >> 16) & 65535;
              result1 += temp & 65535;
              result2 += (result1 >> 16) & 65535;
              result1 &= 65535;
              result3 = (result2 >> 16) & 65535;
              result2 &= 65535;
              // Add carry
              x0 = ((int)(result0 | (result1 << 16)));
              x1 = ((int)(result2 | (result3 << 16)));
              int x2 = (x0 + carry);
              if (((x2 >> 31) == (x0 >> 31)) ? ((x2 & Integer.MAX_VALUE) < (x0 &
              Integer.MAX_VALUE)) :
                  ((x2 >> 31) == 0)) {
                // Carry in addition
                x1 = (x1 + 1);
              }
              this.data[i] = x2;
              carry = x1;
            }
          } else {
            for (int i = 0; i < this.wordCount; ++i) {
              int x0 = this.data[i];
              int x1 = x0;
              int y0 = multiplicand;
              int y1 = y0;
              x0 &= 65535;
              y0 &= 65535;
              x1 = (x1 >> 16) & 65535;
              y1 = (y1 >> 16) & 65535;
              int temp = (x0 * y0);  // a * c
              result1 = (temp >> 16) & 65535;
              result0 = temp & 65535;
              temp = (x0 * y1);  // a * d
              result2 = (temp >> 16) & 65535;
              result1 += temp & 65535;
              result2 += (result1 >> 16) & 65535;
              result1 &= 65535;
              temp = (x1 * y0);  // b * c
              result2 += (temp >> 16) & 65535;
              result1 += temp & 65535;
              result2 += (result1 >> 16) & 65535;
              result1 &= 65535;
              result3 = (result2 >> 16) & 65535;
              result2 &= 65535;
              temp = (x1 * y1);  // b * d
              result3 += (temp >> 16) & 65535;
              result2 += temp & 65535;
              result3 += (result2 >> 16) & 65535;
              result2 &= 65535;
              // Add carry
              x0 = ((int)(result0 | (result1 << 16)));
              x1 = ((int)(result2 | (result3 << 16)));
              int x2 = (x0 + carry);
              if (((x2 >> 31) == (x0 >> 31)) ? ((x2 & Integer.MAX_VALUE) < (x0 &
              Integer.MAX_VALUE)) :
                  ((x2 >> 31) == 0)) {
                // Carry in addition
                x1 = (x1 + 1);
              }
              this.data[i] = x2;
              carry = x1;
            }
          }
          if (carry != 0) {
            if (this.wordCount >= this.data.length) {
              int[] newdata = new int[this.wordCount + 20];
              System.arraycopy(this.data, 0, newdata, 0, this.data.length);
              this.data = newdata;
            }
            this.data[this.wordCount] = carry;
            ++this.wordCount;
          }
          // Calculate the correct data length
          while (this.wordCount != 0 && this.data[this.wordCount - 1] == 0) {
            --this.wordCount;
          }
        } else {
          if (this.data.length > 0) {
            this.data[0] = 0;
          }
          this.wordCount = 0;
        }
        return this;
      }

      final int signum() {
          return this.wordCount == 0 ? 0 : 1;
        }

     final boolean isEvenNumber() {
          return this.wordCount == 0 || (this.data[0] & 1) == 0;
        }

      int CompareToInt(int val) {
        if (val < 0 || this.wordCount > 1) {
          return 1;
        }
        if (this.wordCount == 0) {
          // this value is 0
          return (val == 0) ? 0 : -1;
        }
        if (this.data[0] == val) {
          return 0;
        }
        return (((this.data[0] >> 31) == (val >> 31)) ? ((this.data[0] &
        Integer.MAX_VALUE) < (val & Integer.MAX_VALUE)) :
                  ((this.data[0] >> 31) == 0)) ? -1 : 1;
      }

      MutableNumber SubtractInt(
        int other) {
        if (other < 0) {
     throw new IllegalArgumentException("other (" + other + ") is less than " +
            "0 ");
        }
      if (other != 0) {
          {
            // Ensure a length of at least 1
            if (this.wordCount == 0) {
              if (this.data.length == 0) {
                this.data = new int[4];
              }
              this.data[0] = 0;
              this.wordCount = 1;
            }
            int borrow;
            int u;
            int a = this.data[0];
            u = a - other;
            borrow = ((((a >> 31) == (u >> 31)) ?
                       ((a & Integer.MAX_VALUE) < (u & Integer.MAX_VALUE)) :
                       ((a >> 31) == 0)) || (a == u && other != 0)) ? 1 : 0;
            this.data[0] = (int)u;
            if (borrow != 0) {
              for (int i = 1; i < this.wordCount; ++i) {
                u = this.data[i] - borrow;
                borrow = (((this.data[i] >> 31) == (u >> 31)) ?
                          ((this.data[i] & Integer.MAX_VALUE) < (u &
                          Integer.MAX_VALUE)) :
                          ((this.data[i] >> 31) == 0)) ? 1 : 0;
                this.data[i] = (int)u;
              }
            }
            // Calculate the correct data length
            while (this.wordCount != 0 && this.data[this.wordCount - 1] == 0) {
              --this.wordCount;
            }
          }
        }
        return this;
      }

    /**
     * Subtracts a MutableNumber object from this instance.
     * @param other A MutableNumber object.
     * @return The difference of the two objects.
     */
      MutableNumber Subtract(
        MutableNumber other) {
        {
          {
       // System.out.println("" + this.data.length + " " +
             // (other.data.length));
            int neededSize = (this.wordCount > other.wordCount) ?
            this.wordCount : other.wordCount;
            if (this.data.length < neededSize) {
              int[] newdata = new int[neededSize + 20];
              System.arraycopy(this.data, 0, newdata, 0, this.data.length);
              this.data = newdata;
            }
            neededSize = (this.wordCount < other.wordCount) ? this.wordCount :
            other.wordCount;
            int u = 0;
            int borrow = 0;
            for (int i = 0; i < neededSize; ++i) {
              int a = this.data[i];
              u = (a - other.data[i]) - borrow;
              borrow = ((((a >> 31) == (u >> 31)) ? ((a & Integer.MAX_VALUE) <
              (u & Integer.MAX_VALUE)) :
                         ((a >> 31) == 0)) || (a == u && other.data[i] !=
                         0)) ? 1 : 0;
              this.data[i] = (int)u;
            }
            if (borrow != 0) {
              for (int i = neededSize; i < this.wordCount; ++i) {
                int a = this.data[i];
                u = (a - other.data[i]) - borrow;
                borrow = ((((a >> 31) == (u >> 31)) ? ((a & Integer.MAX_VALUE) <
                (u & Integer.MAX_VALUE)) :
                           ((a >> 31) == 0)) || (a == u && other.data[i] !=
                           0)) ? 1 : 0;
                this.data[i] = (int)u;
              }
            }
            // Calculate the correct data length
            while (this.wordCount != 0 && this.data[this.wordCount - 1] == 0) {
              --this.wordCount;
            }
            return this;
          }
        }
      }

      public int compareTo(MutableNumber other) {
        if (this.wordCount != other.wordCount) {
          return (this.wordCount < other.wordCount) ? -1 : 1;
        }
        int valueN = this.wordCount;
        while ((valueN--) != 0) {
          int an = this.data[valueN];
          int bn = other.data[valueN];
          // Unsigned less-than check
          if (((an >> 31) == (bn >> 31)) ?
              ((an & Integer.MAX_VALUE) < (bn & Integer.MAX_VALUE)) :
              ((an >> 31) == 0)) {
            return -1;
          }
          if (an != bn) {
            return 1;
          }
        }
        return 0;
      }

    /**
     * Adds a 32-bit signed integer to this instance.
     * @param augend A 32-bit signed integer.
     * @return This instance.
     */
      MutableNumber Add(int augend) {
        if (augend < 0) {
   throw new IllegalArgumentException("augend (" + augend + ") is less than " +
            "0 ");
        }
        {
        if (augend != 0) {
          int carry = 0;
          // Ensure a length of at least 1
          if (this.wordCount == 0) {
            if (this.data.length == 0) {
              this.data = new int[4];
            }
            this.data[0] = 0;
            this.wordCount = 1;
          }
          for (int i = 0; i < this.wordCount; ++i) {
            int u;
            int a = this.data[i];
            u = (a + augend) + carry;
            carry = ((((u >> 31) == (a >> 31)) ? ((u & Integer.MAX_VALUE) < (a &
            Integer.MAX_VALUE)) :
                      ((u >> 31) == 0)) || (u == a && augend != 0)) ? 1 : 0;
            this.data[i] = u;
            if (carry == 0) {
              return this;
            }
            augend = 0;
          }
          if (carry != 0) {
            if (this.wordCount >= this.data.length) {
              int[] newdata = new int[this.wordCount + 20];
              System.arraycopy(this.data, 0, newdata, 0, this.data.length);
              this.data = newdata;
            }
            this.data[this.wordCount] = carry;
            ++this.wordCount;
          }
        }
        // Calculate the correct data length
        while (this.wordCount != 0 && this.data[this.wordCount - 1] == 0) {
          --this.wordCount;
        }
        return this;
      }
    }
    }

    private int smallValue;  // if integerMode is 0
    private MutableNumber mnum;  // if integerMode is 1
    private BigInteger largeValue;  // if integerMode is 2
    private int integerMode;
    private static BigInteger valueInt32MinValue = BigInteger.valueOf(Integer.MIN_VALUE);
    private static BigInteger valueInt32MaxValue = BigInteger.valueOf(Integer.MAX_VALUE);
    private static BigInteger valueNegativeInt32MinValue = (valueInt32MinValue).negate();

    FastInteger(int value) {
      this.smallValue = value;
    }

    static FastInteger Copy(FastInteger value) {
      FastInteger fi = new FastInteger(value.smallValue);
      fi.integerMode = value.integerMode;
      fi.largeValue = value.largeValue;
      fi.mnum = (value.mnum == null || value.integerMode != 1) ? null :
      value.mnum.Copy();
      return fi;
    }

    static FastInteger FromBig(BigInteger bigintVal) {
      if (bigintVal.canFitInInt()) {
        return new FastInteger(bigintVal.intValue());
      }
      if (bigintVal.signum() > 0) {
        FastInteger fi = new FastInteger(0);
        fi.integerMode = 1;
        fi.mnum = MutableNumber.FromBigInteger(bigintVal);
        return fi;
      } else {
        FastInteger fi = new FastInteger(0);
        fi.integerMode = 2;
        fi.largeValue = bigintVal;
        return fi;
      }
    }

    int AsInt32() {
      switch (this.integerMode) {
        case 0:
          return this.smallValue;
        case 1:
          return this.mnum.ToInt32();
        case 2:
          return this.largeValue.intValue();
        default:
          throw new IllegalStateException();
      }
    }

    /**
     * Compares a FastInteger object with this instance.
     * @param val A FastInteger object.
     * @return Zero if the values are equal; a negative number if this instance is
     * less, or a positive number if this instance is greater.
     */
    public int compareTo(FastInteger val) {
      switch ((this.integerMode << 2) | val.integerMode) {
          case (0 << 2) | 0: {
            int vsv = val.smallValue;
            return (this.smallValue == vsv) ? 0 :
              (this.smallValue < vsv ? -1 : 1);
          }
        case (0 << 2) | 1:
          return -val.mnum.CompareToInt(this.smallValue);
        case (0 << 2) | 2:
          return this.AsBigInteger().compareTo(val.largeValue);
        case (1 << 2) | 0:
          return this.mnum.CompareToInt(val.smallValue);
        case (1 << 2) | 1:
          return this.mnum.compareTo(val.mnum);
        case (1 << 2) | 2:
          return this.AsBigInteger().compareTo(val.largeValue);
        case (2 << 2) | 0:
        case (2 << 2) | 1:
        case (2 << 2) | 2:
          return this.largeValue.compareTo(val.AsBigInteger());
        default:
          throw new IllegalStateException();
      }
    }

    FastInteger Abs() {
      return (this.signum() < 0) ? this.Negate() : this;
    }

    static BigInteger WordsToBigInteger(int[] words) {
      int wordCount = words.length;
      if (wordCount == 1 && (words[0] >> 31) == 0) {
        return BigInteger.valueOf((int)words[0]);
      }
      byte[] bytes = new byte[(wordCount * 4) + 1];
      for (int i = 0; i < wordCount; ++i) {
        bytes[(i * 4) + 0] = (byte)(words[i] & 0xff);
        bytes[(i * 4) + 1] = (byte)((words[i] >> 8) & 0xff);
        bytes[(i * 4) + 2] = (byte)((words[i] >> 16) & 0xff);
        bytes[(i * 4) + 3] = (byte)((words[i] >> 24) & 0xff);
      }
      bytes[bytes.length - 1] = (byte)0;
      return BigInteger.fromByteArray(bytes, true);
    }

    static int[] GetLastWords(BigInteger bigint, int numWords32Bit) {
      return
      MutableNumber.FromBigInteger(bigint).GetLastWordsInternal(numWords32Bit);
    }

    FastInteger SetInt(int val) {
      this.smallValue = val;
      this.integerMode = 0;
      return this;
    }

    int RepeatedSubtract(FastInteger divisor) {
      if (this.integerMode == 1) {
        int count = 0;
        if (divisor.integerMode == 1) {
          while (this.mnum.compareTo(divisor.mnum) >= 0) {
            this.mnum.Subtract(divisor.mnum);
            ++count;
          }
          return count;
        }
        if (divisor.integerMode == 0 && divisor.smallValue >= 0) {
          if (this.mnum.CanFitInInt32()) {
            int small = this.mnum.ToInt32();
            count = small / divisor.smallValue;
            this.mnum.SetInt(small % divisor.smallValue);
          } else {
            MutableNumber dmnum = new MutableNumber(divisor.smallValue);
            while (this.mnum.compareTo(dmnum) >= 0) {
              this.mnum.Subtract(dmnum);
              ++count;
            }
          }
          return count;
        } else {
          BigInteger bigrem;
          BigInteger bigquo;
{
BigInteger[] divrem = (this.AsBigInteger()).divideAndRemainder(divisor.AsBigInteger());
bigquo = divrem[0];
bigrem = divrem[1]; }
          int smallquo = bigquo.intValue();
          this.integerMode = 2;
          this.largeValue = bigrem;
          return smallquo;
        }
      } else {
        BigInteger bigrem;
        BigInteger bigquo;
{
BigInteger[] divrem = (this.AsBigInteger()).divideAndRemainder(divisor.AsBigInteger());
bigquo = divrem[0];
bigrem = divrem[1]; }
        int smallquo = bigquo.intValue();
        this.integerMode = 2;
        this.largeValue = bigrem;
        return smallquo;
      }
    }

    /**
     * Sets this object&apos;s value to the current value times another integer.
     * @param val The integer to multiply by.
     * @return This object.
     */
    FastInteger Multiply(int val) {
      if (val == 0) {
        this.smallValue = 0;
        this.integerMode = 0;
      } else {
        switch (this.integerMode) {
          case 0:
            boolean apos = this.smallValue > 0L;
            boolean bpos = val > 0L;
            if (
              (apos && ((!bpos && (Integer.MIN_VALUE / this.smallValue) > val) ||
                        (bpos && this.smallValue > (Integer.MAX_VALUE / val)))) ||
              (!apos && ((!bpos && this.smallValue != 0L &&
                          (Integer.MAX_VALUE / this.smallValue) > val) ||
                         (bpos && this.smallValue < (Integer.MIN_VALUE / val))))) {
              // would overflow, convert to large
              if (apos && bpos) {
                // if both operands are nonnegative
                // convert to mutable big integer
                this.integerMode = 1;
                this.mnum = new MutableNumber(this.smallValue);
                this.mnum.Multiply(val);
              } else {
                // if either operand is negative
                // convert to big integer
                this.integerMode = 2;
                this.largeValue = BigInteger.valueOf(this.smallValue);
                this.largeValue = this.largeValue.multiply(BigInteger.valueOf(val));
              }
            } else {
              smallValue *= val;
            }
            break;
          case 1:
            if (val < 0) {
              this.integerMode = 2;
              this.largeValue = this.mnum.ToBigInteger();
              this.largeValue = this.largeValue.multiply(BigInteger.valueOf(val));
            } else {
              mnum.Multiply(val);
            }
            break;
          case 2:
            this.largeValue = this.largeValue.multiply(BigInteger.valueOf(val));
            break;
          default:
            throw new IllegalStateException();
        }
      }
      return this;
    }

    /**
     * Sets this object&apos;s value to 0 minus its current value (reverses its
     * sign).
     * @return This object.
     */
    FastInteger Negate() {
      switch (this.integerMode) {
        case 0:
          if (this.smallValue == Integer.MIN_VALUE) {
            // would overflow, convert to large
            this.integerMode = 1;
            this.mnum =
            MutableNumber.FromBigInteger(valueNegativeInt32MinValue);
          } else {
            smallValue = -smallValue;
          }
          break;
        case 1:
          this.integerMode = 2;
          this.largeValue = this.mnum.ToBigInteger();
          this.largeValue = (this.largeValue).negate();
          break;
        case 2:
          this.largeValue = (this.largeValue).negate();
          break;
        default:
          throw new IllegalStateException();
      }
      return this;
    }

    /**
     * Sets this object&apos;s value to the current value minus the given
     * FastInteger value.
     * @param val The subtrahend.
     * @return This object.
     */
    FastInteger Subtract(FastInteger val) {
      BigInteger valValue;
      switch (this.integerMode) {
        case 0:
          if (val.integerMode == 0) {
            int vsv = val.smallValue;
            if ((vsv < 0 && Integer.MAX_VALUE + vsv < this.smallValue) ||
                (vsv > 0 && Integer.MIN_VALUE + vsv > this.smallValue)) {
              // would overflow, convert to large
              this.integerMode = 2;
              this.largeValue = BigInteger.valueOf(this.smallValue);
              this.largeValue = this.largeValue.subtract(BigInteger.valueOf(vsv));
            } else {
              this.smallValue -= vsv;
            }
          } else {
            integerMode = 2;
            largeValue = BigInteger.valueOf(smallValue);
            valValue = val.AsBigInteger();
            largeValue = largeValue.subtract(valValue);
          }
          break;
        case 1:
          if (val.integerMode == 1) {
            // NOTE: Mutable numbers are
            // currently always zero or positive
            this.mnum.Subtract(val.mnum);
          } else if (val.integerMode == 0 && val.smallValue >= 0) {
            mnum.SubtractInt(val.smallValue);
          } else {
            integerMode = 2;
            largeValue = mnum.ToBigInteger();
            valValue = val.AsBigInteger();
            largeValue = largeValue.subtract(valValue);
          }
          break;
        case 2:
          valValue = val.AsBigInteger();
          this.largeValue = this.largeValue.subtract(valValue);
          break;
        default:
          throw new IllegalStateException();
      }
      return this;
    }

    /**
     * Sets this object&apos;s value to the current value minus the given integer.
     * @param val The subtrahend.
     * @return This object.
     */
    FastInteger SubtractInt(int val) {
      if (val == Integer.MIN_VALUE) {
        return this.AddBig(valueNegativeInt32MinValue);
      }
      if (this.integerMode == 0) {
        if ((val < 0 && Integer.MAX_VALUE + val < this.smallValue) ||
                (val > 0 && Integer.MIN_VALUE + val > this.smallValue)) {
          // would overflow, convert to large
          this.integerMode = 2;
          this.largeValue = BigInteger.valueOf(this.smallValue);
          this.largeValue = this.largeValue.subtract(BigInteger.valueOf(val));
        } else {
          this.smallValue -= val;
        }
        return this;
      }
      return this.AddInt(-val);
    }

    /**
     * Sets this object&apos;s value to the current value plus the given integer.
     * @param bigintVal The number to add.
     * @return This object.
     */
    FastInteger AddBig(BigInteger bigintVal) {
      switch (this.integerMode) {
          case 0: {
            return bigintVal.canFitInInt() ? this.AddInt(bigintVal.intValue()) :
            this.Add(FastInteger.FromBig(bigintVal));
          }
        case 1:
          this.integerMode = 2;
          this.largeValue = this.mnum.ToBigInteger();
          this.largeValue = largeValue.add(bigintVal);
          break;
        case 2:
          this.largeValue = largeValue.add(bigintVal);
          break;
        default:
          throw new IllegalStateException();
      }
      return this;
    }

    /**
     * Sets this object&apos;s value to the current value minus the given integer.
     * @param bigintVal The subtrahend.
     * @return This object.
     */
    FastInteger SubtractBig(BigInteger bigintVal) {
      if (this.integerMode == 2) {
        this.largeValue = this.largeValue.subtract(bigintVal);
        return this;
      } else {
        int sign = bigintVal.signum();
        if (sign == 0) {
          return this;
        }
        // Check if this value fits an int, except if
        // it's MinValue
        if (sign < 0 && bigintVal.compareTo(valueInt32MinValue) > 0) {
          return this.AddInt(-(bigintVal.intValue()));
        }
        if (sign > 0 && bigintVal.compareTo(valueInt32MaxValue) <= 0) {
          return this.SubtractInt(bigintVal.intValue());
        }
        bigintVal = bigintVal.negate();
        return this.AddBig(bigintVal);
      }
    }

    FastInteger Add(FastInteger val) {
      BigInteger valValue;
      switch (this.integerMode) {
        case 0:
          if (val.integerMode == 0) {
            if ((this.smallValue < 0 && (int)val.smallValue < Integer.MIN_VALUE -
            this.smallValue) ||
                (this.smallValue > 0 && (int)val.smallValue > Integer.MAX_VALUE -
                this.smallValue)) {
              // would overflow
              if (val.smallValue >= 0) {
                this.integerMode = 1;
                this.mnum = new MutableNumber(this.smallValue);
                this.mnum.Add(val.smallValue);
              } else {
                this.integerMode = 2;
                this.largeValue = BigInteger.valueOf(this.smallValue);
                this.largeValue = this.largeValue.add(BigInteger.valueOf(val.smallValue));
              }
            } else {
              this.smallValue += val.smallValue;
            }
          } else {
            integerMode = 2;
            largeValue = BigInteger.valueOf(smallValue);
            valValue = val.AsBigInteger();
            largeValue = largeValue.add(valValue);
          }
          break;
        case 1:
          if (val.integerMode == 0 && val.smallValue >= 0) {
            this.mnum.Add(val.smallValue);
          } else {
            integerMode = 2;
            largeValue = mnum.ToBigInteger();
            valValue = val.AsBigInteger();
            largeValue = largeValue.add(valValue);
          }
          break;
        case 2:
          valValue = val.AsBigInteger();
          this.largeValue = this.largeValue.add(valValue);
          break;
        default:
          throw new IllegalStateException();
      }
      return this;
    }

    /**
     * Sets this object&apos;s value to the remainder of the current value divided
     * by the given integer.
     * @param divisor The divisor.
     * @return This object.
     * @throws ArithmeticException Attempted to divide by zero.
     */
    FastInteger Remainder(int divisor) {
      // Mod operator will always result in a
      // number that fits an int for int divisors
      if (divisor != 0) {
        switch (this.integerMode) {
          case 0:
            this.smallValue %= divisor;
            break;
          case 1:
            this.largeValue = this.mnum.ToBigInteger();
            this.largeValue = this.largeValue.remainder(BigInteger.valueOf(divisor));
            this.smallValue = this.largeValue.intValue();
            this.integerMode = 0;
            break;
          case 2:
            this.largeValue = this.largeValue.remainder(BigInteger.valueOf(divisor));
            this.smallValue = this.largeValue.intValue();
            this.integerMode = 0;
            break;
          default:
            throw new IllegalStateException();
        }
      } else {
        throw new ArithmeticException();
      }
      return this;
    }

    FastInteger Increment() {
      if (this.integerMode == 0) {
        if (this.smallValue != Integer.MAX_VALUE) {
          ++this.smallValue;
        } else {
          this.integerMode = 1;
          this.mnum = MutableNumber.FromBigInteger(valueNegativeInt32MinValue);
        }
        return this;
      }
      return this.AddInt(1);
    }

    FastInteger Decrement() {
      if (this.integerMode == 0) {
        if (this.smallValue != Integer.MIN_VALUE) {
          --this.smallValue;
        } else {
          this.integerMode = 1;
          this.mnum = MutableNumber.FromBigInteger(valueInt32MinValue);
          this.mnum.SubtractInt(1);
        }
        return this;
      }
      return this.SubtractInt(1);
    }

    /**
     * Divides this instance by the value of a 32-bit signed integer.
     * @param divisor A 32-bit signed integer.
     * @return The quotient of the two objects.
     * @throws ArithmeticException Attempted to divide by zero.
     */
    FastInteger Divide(int divisor) {
      if (divisor != 0) {
        switch (this.integerMode) {
          case 0:
            if (divisor == -1 && this.smallValue == Integer.MIN_VALUE) {
              // would overflow, convert to large
              this.integerMode = 1;
              this.mnum =
              MutableNumber.FromBigInteger(valueNegativeInt32MinValue);
            } else {
              smallValue /= divisor;
            }
            break;
          case 1:
            this.integerMode = 2;
            this.largeValue = this.mnum.ToBigInteger();
            this.largeValue = this.largeValue.divide(BigInteger.valueOf(divisor));
            if (this.largeValue.signum() == 0) {
              this.integerMode = 0;
              this.smallValue = 0;
            }
            break;
          case 2:
            this.largeValue = this.largeValue.divide(BigInteger.valueOf(divisor));
            if (this.largeValue.signum() == 0) {
              this.integerMode = 0;
              this.smallValue = 0;
            }
            break;
          default:
            throw new IllegalStateException();
        }
      } else {
        throw new ArithmeticException();
      }
      return this;
    }

    /**
     * Gets a value indicating whether this object&apos;s value is even.
     * @return True if this object's value is even; otherwise, false.
     */
    final boolean isEvenNumber() {
        switch (this.integerMode) {
          case 0:
            return (this.smallValue & 1) == 0;
          case 1:
            return this.mnum.isEvenNumber();
          case 2:
            return this.largeValue.testBit(0) == false;
          default:
            throw new IllegalStateException();
        }
      }

    /**
     * Adds a 32-bit signed integer to this instance.
     * @param val A 32-bit signed integer.
     * @return This instance.
     */
    FastInteger AddInt(int val) {
      BigInteger valValue;
      switch (this.integerMode) {
        case 0:
          if ((this.smallValue < 0 && (int)val < Integer.MIN_VALUE -
          this.smallValue) ||
              (this.smallValue > 0 && (int)val > Integer.MAX_VALUE -
              this.smallValue)) {
            // would overflow
            if (val >= 0) {
              this.integerMode = 1;
              this.mnum = new MutableNumber(this.smallValue);
              this.mnum.Add(val);
            } else {
              this.integerMode = 2;
              this.largeValue = BigInteger.valueOf(this.smallValue);
              this.largeValue = this.largeValue.add(BigInteger.valueOf(val));
            }
          } else {
            smallValue += val;
          }
          break;
        case 1:
          if (val >= 0) {
            this.mnum.Add(val);
          } else {
            integerMode = 2;
            largeValue = mnum.ToBigInteger();
            valValue = BigInteger.valueOf(val);
            largeValue = largeValue.add(valValue);
          }
          break;
        case 2:
          valValue = BigInteger.valueOf(val);
          this.largeValue = this.largeValue.add(valValue);
          break;
        default:
          throw new IllegalStateException();
      }
      return this;
    }

    boolean CanFitInInt32() {
      switch (this.integerMode) {
        case 0:
          return true;
        case 1:
          return this.mnum.CanFitInInt32();
          case 2: {
            return this.largeValue.canFitInInt();
          }
        default:
          throw new IllegalStateException();
      }
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      switch (this.integerMode) {
        case 0:
          return Integer.toString((int)this.smallValue);
        case 1:
          return this.mnum.ToBigInteger().toString();
        case 2:
          return this.largeValue.toString();
        default:
          return "";
      }
    }

    /**
     * Gets the sign of this object&apos;s value.
     * @return 1 if positive, -1 if negative, 0 if zero.
     */
    final int signum() {
        switch (this.integerMode) {
          case 0:
            return ((this.smallValue == 0) ? 0 : ((this.smallValue < 0) ? -1 : 1));
          case 1:
            return this.mnum.signum();
          case 2:
            return this.largeValue.signum();
          default:
            return 0;
        }
      }

    /**
     * Gets a value indicating whether this value is zero.
     * @return True if this value is zero; otherwise, false.
     */
    final boolean isValueZero() {
        switch (this.integerMode) {
          case 0:
            return this.smallValue == 0;
          case 1:
            return this.mnum.signum() == 0;
          case 2:
            return this.largeValue.signum() == 0;
          default:
            return false;
        }
      }

    /**
     * Compares a 32-bit signed integer with this instance.
     * @param val A 32-bit signed integer.
     * @return Zero if the values are equal; a negative number if this instance is
     * less, or a positive number if this instance is greater.
     */
    int CompareToInt(int val) {
      switch (this.integerMode) {
        case 0:
          return (val == this.smallValue) ? 0 : (this.smallValue < val ? -1 :
          1);
        case 1:
          return this.mnum.ToBigInteger().compareTo(BigInteger.valueOf(val));
        case 2:
          return this.largeValue.compareTo(BigInteger.valueOf(val));
        default:
          return 0;
      }
    }

    BigInteger AsBigInteger() {
      switch (this.integerMode) {
        case 0:
          return BigInteger.valueOf(this.smallValue);
        case 1:
          return this.mnum.ToBigInteger();
        case 2:
          return this.largeValue;
        default:
          throw new IllegalStateException();
      }
    }
  }
