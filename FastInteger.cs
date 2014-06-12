/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;

namespace PeterO {
  internal sealed class FastInteger : IComparable<FastInteger> {
    private sealed class MutableNumber {
      private int[] data;

      private int wordCount;

      public static MutableNumber FromBigInteger(BigInteger bigintVal) {
        MutableNumber mnum = new MutableNumber(0);
        if (bigintVal.Sign < 0) {
          throw new ArgumentException("bigintVal's sign (" + Convert.ToString((int)bigintVal.Sign, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
        }
        byte[] bytes = bigintVal.ToByteArray();
        int len = bytes.Length;
        int newWordCount = Math.Max(4, (len / 4) + 1);
        if (newWordCount > mnum.data.Length) {
          mnum.data = new int[newWordCount];
        }
        mnum.wordCount = newWordCount;
        unchecked {
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

      public MutableNumber(int val) {
        if (val < 0) {
          throw new ArgumentException("val (" + Convert.ToString((int)val, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
        }
        this.data = new int[4];
        this.wordCount = (val == 0) ? 0 : 1;
        this.data[0] = unchecked((int)(val & 0xFFFFFFFFL));
      }

      public MutableNumber SetInt(int val) {
        if (val < 0) {
          throw new ArgumentException("val (" + Convert.ToString((int)val, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
        }
        this.wordCount = (val == 0) ? 0 : 1;
        this.data[0] = unchecked((int)(val & 0xFFFFFFFFL));
        return this;
      }

      public BigInteger ToBigInteger() {
        if (this.wordCount == 1 && (this.data[0] >> 31) == 0) {
          return (BigInteger)((int)this.data[0]);
        }
        byte[] bytes = new byte[(this.wordCount * 4) + 1];
        for (int i = 0; i < this.wordCount; ++i) {
          bytes[i * 4] = (byte)(this.data[i] & 0xff);
          bytes[(i * 4) + 1] = (byte)((this.data[i] >> 8) & 0xff);
          bytes[(i * 4) + 2] = (byte)((this.data[i] >> 16) & 0xff);
          bytes[(i * 4) + 3] = (byte)((this.data[i] >> 24) & 0xff);
        }
        bytes[bytes.Length - 1] = (byte)0;
        return new BigInteger((byte[])bytes);
      }

      internal int[] GetLastWordsInternal(int numWords32Bit) {
        int[] ret = new int[numWords32Bit];
        Array.Copy(this.data, ret, Math.Min(numWords32Bit, this.wordCount));
        return ret;
      }

      public bool CanFitInInt32() {
        return this.wordCount == 0 || (this.wordCount == 1 && (this.data[0] >> 31) == 0);
      }

      public int ToInt32() {
        return this.wordCount == 0 ? 0 : this.data[0];
      }

      public MutableNumber Copy() {
        MutableNumber mbi = new MutableNumber(0);
        if (this.wordCount > mbi.data.Length) {
          mbi.data = new int[this.wordCount];
        }
        Array.Copy(this.data, mbi.data, this.wordCount);
        mbi.wordCount = this.wordCount;
        return mbi;
      }

    /// <summary>Multiplies this instance by the value of a 32-bit signed
    /// integer.</summary>
    /// <returns>The product of the two objects.</returns>
    /// <param name='multiplicand'>A 32-bit signed integer.</param>
      public MutableNumber Multiply(int multiplicand) {
        if (multiplicand < 0) {
          throw new ArgumentException("multiplicand (" + Convert.ToString((int)multiplicand, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
        } else if (multiplicand != 0) {
          int carry = 0;
          if (this.wordCount == 0) {
            if (this.data.Length == 0) {
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
              int temp = unchecked(x0 * y0);  // a * c
              result1 = (temp >> 16) & 65535; result0 = temp & 65535;
              result2 = 0;
              temp = unchecked(x1 * y0);  // b * c
              result2 += (temp >> 16) & 65535; result1 += temp & 65535;
              result2 += (result1 >> 16) & 65535;
              result1 &= 65535;
              result3 = (result2 >> 16) & 65535;
              result2 &= 65535;
              // Add carry
              x0 = unchecked((int)(result0 | (result1 << 16)));
              x1 = unchecked((int)(result2 | (result3 << 16)));
              int x2 = unchecked(x0 + carry);
              if (((x2 >> 31) == (x0 >> 31)) ? ((x2 & Int32.MaxValue) < (x0 & Int32.MaxValue)) :
                  ((x2 >> 31) == 0)) {
                // Carry in addition
                x1 = unchecked(x1 + 1);
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
              int temp = unchecked(x0 * y0);  // a * c
              result1 = (temp >> 16) & 65535; result0 = temp & 65535;
              temp = unchecked(x0 * y1);  // a * d
              result2 = (temp >> 16) & 65535; result1 += temp & 65535;
              result2 += (result1 >> 16) & 65535;
              result1 &= 65535;
              temp = unchecked(x1 * y0);  // b * c
              result2 += (temp >> 16) & 65535; result1 += temp & 65535;
              result2 += (result1 >> 16) & 65535;
              result1 &= 65535;
              result3 = (result2 >> 16) & 65535;
              result2 &= 65535;
              temp = unchecked(x1 * y1);  // b * d
              result3 += (temp >> 16) & 65535; result2 += temp & 65535;
              result3 += (result2 >> 16) & 65535;
              result2 &= 65535;
              // Add carry
              x0 = unchecked((int)(result0 | (result1 << 16)));
              x1 = unchecked((int)(result2 | (result3 << 16)));
              int x2 = unchecked(x0 + carry);
              if (((x2 >> 31) == (x0 >> 31)) ? ((x2 & Int32.MaxValue) < (x0 & Int32.MaxValue)) :
                  ((x2 >> 31) == 0)) {
                // Carry in addition
                x1 = unchecked(x1 + 1);
              }
              this.data[i] = x2;
              carry = x1;
            }
          }
          if (carry != 0) {
            if (this.wordCount >= this.data.Length) {
              int[] newdata = new int[this.wordCount + 20];
              Array.Copy(this.data, 0, newdata, 0, this.data.Length);
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
          if (this.data.Length > 0) {
            this.data[0] = 0;
          }
          this.wordCount = 0;
        }
        return this;
      }

      public int Sign {
        get {
          return this.wordCount == 0 ? 0 : 1;
        }
      }

    /// <summary>Gets a value indicating whether this value is even.</summary>
    /// <value>True if this value is even; otherwise, false.</value>
      public bool IsEvenNumber {
        get {
          return this.wordCount == 0 || (this.data[0] & 1) == 0;
        }
      }

    /// <summary>Compares a 32-bit signed integer with this instance.</summary>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    /// <param name='val'>A 32-bit signed integer.</param>
      public int CompareToInt(int val) {
        if (val < 0 || this.wordCount > 1) {
          return 1;
        }
        if (this.wordCount == 0) {
          // this value is 0
          return (val == 0) ? 0 : -1;
        } else if (this.data[0] == val) {
          return 0;
        } else {
          return (((this.data[0] >> 31) == (val >> 31)) ? ((this.data[0] & Int32.MaxValue) < (val & Int32.MaxValue))
                  : ((this.data[0] >> 31) == 0)) ? -1 : 1;
        }
      }

    /// <summary>Subtracts a 32-bit signed integer from this instance.</summary>
    /// <returns>The difference of the two objects.</returns>
    /// <param name='other'>A 32-bit signed integer.</param>
      public MutableNumber SubtractInt(
        int other) {
        if (other < 0) {
          throw new ArgumentException("other (" + Convert.ToString((int)other, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
        } else if (other != 0) {
          unchecked {
            // Ensure a length of at least 1
            if (this.wordCount == 0) {
              if (this.data.Length == 0) {
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
                       ((a & Int32.MaxValue) < (u & Int32.MaxValue)) :
                       ((a >> 31) == 0)) || (a == u && other != 0)) ? 1 : 0;
            this.data[0] = (int)u;
            if (borrow != 0) {
              for (int i = 1; i < this.wordCount; ++i) {
                u = this.data[i] - borrow;
                borrow = (((this.data[i] >> 31) == (u >> 31)) ?
                          ((this.data[i] & Int32.MaxValue) < (u & Int32.MaxValue)) :
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

    /// <summary>Subtracts a MutableNumber object from this instance.</summary>
    /// <returns>The difference of the two objects.</returns>
    /// <param name='other'>A MutableNumber object.</param>
      public MutableNumber Subtract(
        MutableNumber other) {
        unchecked {
          {
            // Console.WriteLine("" + this.data.Length + " " + (other.data.Length));
            int neededSize = (this.wordCount > other.wordCount) ? this.wordCount : other.wordCount;
            if (this.data.Length < neededSize) {
              int[] newdata = new int[neededSize + 20];
              Array.Copy(this.data, 0, newdata, 0, this.data.Length);
              this.data = newdata;
            }
            neededSize = (this.wordCount < other.wordCount) ? this.wordCount : other.wordCount;
            int u = 0;
            int borrow = 0;
            for (int i = 0; i < neededSize; ++i) {
              int a = this.data[i];
              u = (a - other.data[i]) - borrow;
              borrow = ((((a >> 31) == (u >> 31)) ? ((a & Int32.MaxValue) < (u & Int32.MaxValue)) :
                         ((a >> 31) == 0)) || (a == u && other.data[i] != 0)) ? 1 : 0;
              this.data[i] = (int)u;
            }
            if (borrow != 0) {
              for (int i = neededSize; i < this.wordCount; ++i) {
                int a = this.data[i];
                u = (a - other.data[i]) - borrow;
                borrow = ((((a >> 31) == (u >> 31)) ? ((a & Int32.MaxValue) < (u & Int32.MaxValue)) :
                           ((a >> 31) == 0)) || (a == u && other.data[i] != 0)) ? 1 : 0;
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

    /// <summary>Compares a MutableNumber object with this instance.</summary>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    /// <param name='other'>A MutableNumber object.</param>
      public int CompareTo(MutableNumber other) {
        if (this.wordCount != other.wordCount) {
          return (this.wordCount < other.wordCount) ? -1 : 1;
        }
        int valueN = this.wordCount;
        while (unchecked(valueN--) != 0) {
          int an = this.data[valueN];
          int bn = other.data[valueN];
          // Unsigned less-than check
          if (((an >> 31) == (bn >> 31)) ?
              ((an & Int32.MaxValue) < (bn & Int32.MaxValue)) :
              ((an >> 31) == 0)) {
            return -1;
          } else if (an != bn) {
            return 1;
          }
        }
        return 0;
      }

    /// <summary>Adds a 32-bit signed integer to this instance.</summary>
    /// <returns>This instance.</returns>
    /// <param name='augend'>A 32-bit signed integer.</param>
      public MutableNumber Add(int augend) {
        if (augend < 0) {
          throw new ArgumentException("augend (" + Convert.ToString((int)augend, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
        } else if (augend != 0) {
          int carry = 0;
          // Ensure a length of at least 1
          if (this.wordCount == 0) {
            if (this.data.Length == 0) {
              this.data = new int[4];
            }
            this.data[0] = 0;
            this.wordCount = 1;
          }
          for (int i = 0; i < this.wordCount; ++i) {
            int u;
            int a = this.data[i];
            u = (a + augend) + carry;
            carry = ((((u >> 31) == (a >> 31)) ? ((u & Int32.MaxValue) < (a & Int32.MaxValue)) :
                      ((u >> 31) == 0)) || (u == a && augend != 0)) ? 1 : 0;
            this.data[i] = u;
            if (carry == 0) {
              return this;
            }
            augend = 0;
          }
          if (carry != 0) {
            if (this.wordCount >= this.data.Length) {
              int[] newdata = new int[this.wordCount + 20];
              Array.Copy(this.data, 0, newdata, 0, this.data.Length);
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

    private int smallValue;  // if integerMode is 0
    private MutableNumber mnum;  // if integerMode is 1
    private BigInteger largeValue;  // if integerMode is 2
    private int integerMode = 0;

    private static BigInteger valueInt32MinValue = (BigInteger)Int32.MinValue;
    private static BigInteger valueInt32MaxValue = (BigInteger)Int32.MaxValue;
    private static BigInteger valueNegativeInt32MinValue = -(BigInteger)valueInt32MinValue;

    public FastInteger(int value) {
      this.smallValue = value;
    }

    public static FastInteger Copy(FastInteger value) {
      FastInteger fi = new FastInteger(value.smallValue);
      fi.integerMode = value.integerMode;
      fi.largeValue = value.largeValue;
      fi.mnum = (value.mnum == null || value.integerMode != 1) ? null : value.mnum.Copy();
      return fi;
    }

    public static FastInteger FromBig(BigInteger bigintVal) {
      if (bigintVal.canFitInInt()) {
        return new FastInteger(bigintVal.intValue());
      } else if (bigintVal.Sign > 0) {
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

    public int AsInt32() {
      switch (this.integerMode) {
        case 0:
          return this.smallValue;
        case 1:
          return this.mnum.ToInt32();
        case 2:
          return (int)this.largeValue;
        default:
          throw new InvalidOperationException();
      }
    }

    /// <summary>Compares a FastInteger object with this instance.</summary>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    /// <param name='val'>A FastInteger object.</param>
    public int CompareTo(FastInteger val) {
      switch ((this.integerMode << 2) | val.integerMode) {
          case (0 << 2) | 0: {
            int vsv = val.smallValue;
            return (this.smallValue == vsv) ? 0 :
              (this.smallValue < vsv ? -1 : 1);
          }
        case (0 << 2) | 1:
          return -val.mnum.CompareToInt(this.smallValue);
        case (0 << 2) | 2:
          return this.AsBigInteger().CompareTo(val.largeValue);
        case (1 << 2) | 0:
          return this.mnum.CompareToInt(val.smallValue);
        case (1 << 2) | 1:
          return this.mnum.CompareTo(val.mnum);
        case (1 << 2) | 2:
          return this.AsBigInteger().CompareTo(val.largeValue);
        case (2 << 2) | 0:
        case (2 << 2) | 1:
        case (2 << 2) | 2:
          return this.largeValue.CompareTo(val.AsBigInteger());
        default:
          throw new InvalidOperationException();
      }
    }

    public FastInteger Abs() {
      return (this.Sign < 0) ? this.Negate() : this;
    }

    public static BigInteger WordsToBigInteger(int[] words) {
      int wordCount = words.Length;
      if (wordCount == 1 && (words[0] >> 31) == 0) {
        return (BigInteger)((int)words[0]);
      }
      byte[] bytes = new byte[(wordCount * 4) + 1];
      for (int i = 0; i < wordCount; ++i) {
        bytes[(i * 4) + 0] = (byte)(words[i] & 0xff);
        bytes[(i * 4) + 1] = (byte)((words[i] >> 8) & 0xff);
        bytes[(i * 4) + 2] = (byte)((words[i] >> 16) & 0xff);
        bytes[(i * 4) + 3] = (byte)((words[i] >> 24) & 0xff);
      }
      bytes[bytes.Length - 1] = (byte)0;
      return new BigInteger((byte[])bytes);
    }

    public static int[] GetLastWords(BigInteger bigint, int numWords32Bit) {
      return MutableNumber.FromBigInteger(bigint).GetLastWordsInternal(numWords32Bit);
    }

    public FastInteger SetInt(int val) {
      this.smallValue = val;
      this.integerMode = 0;
      return this;
    }

    public int RepeatedSubtract(FastInteger divisor) {
      if (this.integerMode == 1) {
        int count = 0;
        if (divisor.integerMode == 1) {
          while (this.mnum.CompareTo(divisor.mnum) >= 0) {
            this.mnum.Subtract(divisor.mnum);
            ++count;
          }
          return count;
        } else if (divisor.integerMode == 0 && divisor.smallValue >= 0) {
          if (this.mnum.CanFitInInt32()) {
            int small = this.mnum.ToInt32();
            count = small / divisor.smallValue;
            this.mnum.SetInt(small % divisor.smallValue);
          } else {
            MutableNumber dmnum = new MutableNumber(divisor.smallValue);
            while (this.mnum.CompareTo(dmnum) >= 0) {
              this.mnum.Subtract(dmnum);
              ++count;
            }
          }
          return count;
        } else {
          BigInteger bigrem;
          BigInteger bigquo = BigInteger.DivRem(this.AsBigInteger(), divisor.AsBigInteger(), out bigrem);
          int smallquo = (int)bigquo;
          this.integerMode = 2;
          this.largeValue = bigrem;
          return smallquo;
        }
      } else {
        BigInteger bigrem;
        BigInteger bigquo = BigInteger.DivRem(this.AsBigInteger(), divisor.AsBigInteger(), out bigrem);
        int smallquo = (int)bigquo;
        this.integerMode = 2;
        this.largeValue = bigrem;
        return smallquo;
      }
    }

    /// <summary>Sets this object&apos;s value to the current value times
    /// another integer.</summary>
    /// <param name='val'>The integer to multiply by.</param>
    /// <returns>This object.</returns>
    public FastInteger Multiply(int val) {
      if (val == 0) {
        this.smallValue = 0;
        this.integerMode = 0;
      } else {
        switch (this.integerMode) {
          case 0:
            bool apos = this.smallValue > 0L;
            bool bpos = val > 0L;
            if (
              (apos && ((!bpos && (Int32.MinValue / this.smallValue) > val) ||
                        (bpos && this.smallValue > (Int32.MaxValue / val)))) ||
              (!apos && ((!bpos && this.smallValue != 0L &&
                          (Int32.MaxValue / this.smallValue) > val) ||
                         (bpos && this.smallValue < (Int32.MinValue / val))))) {
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
                this.largeValue = (BigInteger)this.smallValue;
                this.largeValue *= (BigInteger)val;
              }
            } else {
              smallValue *= val;
            }
            break;
          case 1:
            if (val < 0) {
              this.integerMode = 2;
              this.largeValue = this.mnum.ToBigInteger();
              this.largeValue *= (BigInteger)val;
            } else {
              mnum.Multiply(val);
            }
            break;
          case 2:
            this.largeValue *= (BigInteger)val;
            break;
          default:
            throw new InvalidOperationException();
        }
      }
      return this;
    }

    /// <summary>Sets this object&apos;s value to 0 minus its current value
    /// (reverses its sign).</summary>
    /// <returns>This object.</returns>
    public FastInteger Negate() {
      switch (this.integerMode) {
        case 0:
          if (this.smallValue == Int32.MinValue) {
            // would overflow, convert to large
            this.integerMode = 1;
            this.mnum = MutableNumber.FromBigInteger(valueNegativeInt32MinValue);
          } else {
            smallValue = -smallValue;
          }
          break;
        case 1:
          this.integerMode = 2;
          this.largeValue = this.mnum.ToBigInteger();
          this.largeValue = -(BigInteger)this.largeValue;
          break;
        case 2:
          this.largeValue = -(BigInteger)this.largeValue;
          break;
        default:
          throw new InvalidOperationException();
      }
      return this;
    }

    /// <summary>Sets this object&apos;s value to the current value minus
    /// the given FastInteger value.</summary>
    /// <param name='val'>The subtrahend.</param>
    /// <returns>This object.</returns>
    public FastInteger Subtract(FastInteger val) {
      BigInteger valValue;
      switch (this.integerMode) {
        case 0:
          if (val.integerMode == 0) {
            int vsv = val.smallValue;
            if ((vsv < 0 && Int32.MaxValue + vsv < this.smallValue) ||
                (vsv > 0 && Int32.MinValue + vsv > this.smallValue)) {
              // would overflow, convert to large
              this.integerMode = 2;
              this.largeValue = (BigInteger)this.smallValue;
              this.largeValue -= (BigInteger)vsv;
            } else {
              this.smallValue -= vsv;
            }
          } else {
            integerMode = 2;
            largeValue = (BigInteger)smallValue;
            valValue = val.AsBigInteger();
            largeValue -= (BigInteger)valValue;
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
            largeValue -= (BigInteger)valValue;
          }
          break;
        case 2:
          valValue = val.AsBigInteger();
          this.largeValue -= (BigInteger)valValue;
          break;
        default:
          throw new InvalidOperationException();
      }
      return this;
    }

    /// <summary>Sets this object&apos;s value to the current value minus
    /// the given integer.</summary>
    /// <param name='val'>The subtrahend.</param>
    /// <returns>This object.</returns>
    public FastInteger SubtractInt(int val) {
      if (val == Int32.MinValue) {
        return this.AddBig(valueNegativeInt32MinValue);
      } else if (this.integerMode == 0) {
        if ((val < 0 && Int32.MaxValue + val < this.smallValue) ||
            (val > 0 && Int32.MinValue + val > this.smallValue)) {
          // would overflow, convert to large
          this.integerMode = 2;
          this.largeValue = (BigInteger)this.smallValue;
          this.largeValue -= (BigInteger)val;
        } else {
          this.smallValue -= val;
        }
        return this;
      } else {
        return this.AddInt(-val);
      }
    }

    /// <summary>Sets this object&apos;s value to the current value plus
    /// the given integer.</summary>
    /// <param name='bigintVal'>The number to add.</param>
    /// <returns>This object.</returns>
    public FastInteger AddBig(BigInteger bigintVal) {
      switch (this.integerMode) {
          case 0: {
            if (bigintVal.canFitInInt()) {
              return this.AddInt((int)bigintVal);
            }
            return this.Add(FastInteger.FromBig(bigintVal));
          }
        case 1:
          this.integerMode = 2;
          this.largeValue = this.mnum.ToBigInteger();
          this.largeValue += bigintVal;
          break;
        case 2:
          this.largeValue += bigintVal;
          break;
        default:
          throw new InvalidOperationException();
      }
      return this;
    }

    /// <summary>Sets this object&apos;s value to the current value minus
    /// the given integer.</summary>
    /// <param name='bigintVal'>The subtrahend.</param>
    /// <returns>This object.</returns>
    public FastInteger SubtractBig(BigInteger bigintVal) {
      if (this.integerMode == 2) {
        this.largeValue -= (BigInteger)bigintVal;
        return this;
      } else {
        int sign = bigintVal.Sign;
        if (sign == 0) {
          return this;
        }
        // Check if this value fits an int, except if
        // it's MinValue
        if (sign < 0 && bigintVal.CompareTo(valueInt32MinValue) > 0) {
          return this.AddInt(-((int)bigintVal));
        }
        if (sign > 0 && bigintVal.CompareTo(valueInt32MaxValue) <= 0) {
          return this.SubtractInt((int)bigintVal);
        }
        bigintVal = -bigintVal;
        return this.AddBig(bigintVal);
      }
    }

    public FastInteger Add(FastInteger val) {
      BigInteger valValue;
      switch (this.integerMode) {
        case 0:
          if (val.integerMode == 0) {
            if ((this.smallValue < 0 && (int)val.smallValue < Int32.MinValue - this.smallValue) ||
                (this.smallValue > 0 && (int)val.smallValue > Int32.MaxValue - this.smallValue)) {
              // would overflow
              if (val.smallValue >= 0) {
                this.integerMode = 1;
                this.mnum = new MutableNumber(this.smallValue);
                this.mnum.Add(val.smallValue);
              } else {
                this.integerMode = 2;
                this.largeValue = (BigInteger)this.smallValue;
                this.largeValue += (BigInteger)val.smallValue;
              }
            } else {
              this.smallValue += val.smallValue;
            }
          } else {
            integerMode = 2;
            largeValue = (BigInteger)smallValue;
            valValue = val.AsBigInteger();
            largeValue += (BigInteger)valValue;
          }
          break;
        case 1:
          if (val.integerMode == 0 && val.smallValue >= 0) {
            this.mnum.Add(val.smallValue);
          } else {
            integerMode = 2;
            largeValue = mnum.ToBigInteger();
            valValue = val.AsBigInteger();
            largeValue += (BigInteger)valValue;
          }
          break;
        case 2:
          valValue = val.AsBigInteger();
          this.largeValue += (BigInteger)valValue;
          break;
        default:
          throw new InvalidOperationException();
      }
      return this;
    }

    /// <summary>Sets this object&apos;s value to the remainder of the current
    /// value divided by the given integer.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>This object.</returns>
    /// <exception cref='System.DivideByZeroException'>Attempted
    /// to divide by zero.</exception>
    public FastInteger Remainder(int divisor) {
      // Mod operator will always result in a
      // number that fits an int for int divisors
      if (divisor != 0) {
        switch (this.integerMode) {
          case 0:
            this.smallValue %= divisor;
            break;
          case 1:
            this.largeValue = this.mnum.ToBigInteger();
            this.largeValue %= (BigInteger)divisor;
            this.smallValue = (int)this.largeValue;
            this.integerMode = 0;
            break;
          case 2:
            this.largeValue %= (BigInteger)divisor;
            this.smallValue = (int)this.largeValue;
            this.integerMode = 0;
            break;
          default:
            throw new InvalidOperationException();
        }
      } else {
        throw new DivideByZeroException();
      }
      return this;
    }

    public FastInteger Increment() {
      if (this.integerMode == 0) {
        if (this.smallValue != Int32.MaxValue) {
          ++this.smallValue;
        } else {
          this.integerMode = 1;
          this.mnum = MutableNumber.FromBigInteger(valueNegativeInt32MinValue);
        }
        return this;
      } else {
        return this.AddInt(1);
      }
    }

    public FastInteger Decrement() {
      if (this.integerMode == 0) {
        if (this.smallValue != Int32.MinValue) {
          --this.smallValue;
        } else {
          this.integerMode = 1;
          this.mnum = MutableNumber.FromBigInteger(valueInt32MinValue);
          this.mnum.SubtractInt(1);
        }
        return this;
      } else {
        return this.SubtractInt(1);
      }
    }

    /// <summary>Divides this instance by the value of a 32-bit signed integer.</summary>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='System.DivideByZeroException'>Attempted
    /// to divide by zero.</exception>
    /// <param name='divisor'>A 32-bit signed integer.</param>
    public FastInteger Divide(int divisor) {
      if (divisor != 0) {
        switch (this.integerMode) {
          case 0:
            if (divisor == -1 && this.smallValue == Int32.MinValue) {
              // would overflow, convert to large
              this.integerMode = 1;
              this.mnum = MutableNumber.FromBigInteger(valueNegativeInt32MinValue);
            } else {
              smallValue /= divisor;
            }
            break;
          case 1:
            this.integerMode = 2;
            this.largeValue = this.mnum.ToBigInteger();
            this.largeValue /= (BigInteger)divisor;
            if (this.largeValue.IsZero) {
              this.integerMode = 0;
              this.smallValue = 0;
            }
            break;
          case 2:
            this.largeValue /= (BigInteger)divisor;
            if (this.largeValue.IsZero) {
              this.integerMode = 0;
              this.smallValue = 0;
            }
            break;
          default:
            throw new InvalidOperationException();
        }
      } else {
        throw new DivideByZeroException();
      }
      return this;
    }

    /// <summary>Gets a value indicating whether this object&apos;s value
    /// is even.</summary>
    /// <value>True if this object&apos;s value is even; otherwise, false.</value>
    public bool IsEvenNumber {
      get {
        switch (this.integerMode) {
          case 0:
            return (this.smallValue & 1) == 0;
          case 1:
            return this.mnum.IsEvenNumber;
          case 2:
            return this.largeValue.IsEven;
          default:
            throw new InvalidOperationException();
        }
      }
    }

    /// <summary>Adds a 32-bit signed integer to this instance.</summary>
    /// <returns>This instance.</returns>
    /// <param name='val'>A 32-bit signed integer.</param>
    public FastInteger AddInt(int val) {
      BigInteger valValue;
      switch (this.integerMode) {
        case 0:
          if ((this.smallValue < 0 && (int)val < Int32.MinValue - this.smallValue) ||
              (this.smallValue > 0 && (int)val > Int32.MaxValue - this.smallValue)) {
            // would overflow
            if (val >= 0) {
              this.integerMode = 1;
              this.mnum = new MutableNumber(this.smallValue);
              this.mnum.Add(val);
            } else {
              this.integerMode = 2;
              this.largeValue = (BigInteger)this.smallValue;
              this.largeValue += (BigInteger)val;
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
            valValue = (BigInteger)val;
            largeValue += (BigInteger)valValue;
          }
          break;
        case 2:
          valValue = (BigInteger)val;
          this.largeValue += (BigInteger)valValue;
          break;
        default:
          throw new InvalidOperationException();
      }
      return this;
    }

    public bool CanFitInInt32() {
      switch (this.integerMode) {
        case 0:
          return true;
        case 1:
          return this.mnum.CanFitInInt32();
          case 2: {
            return this.largeValue.canFitInInt();
          }
        default:
          throw new InvalidOperationException();
      }
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      switch (this.integerMode) {
        case 0:
          return Convert.ToString((int)this.smallValue, System.Globalization.CultureInfo.InvariantCulture);
        case 1:
          return this.mnum.ToBigInteger().ToString();
        case 2:
          return this.largeValue.ToString();
        default:
          return String.Empty;
      }
    }

    /// <summary>Gets the sign of this object&apos;s value.</summary>
    /// <value>1 if positive, -1 if negative, 0 if zero.</value>
    public int Sign {
      get {
        switch (this.integerMode) {
          case 0:
            return Math.Sign(this.smallValue);
          case 1:
            return this.mnum.Sign;
          case 2:
            return this.largeValue.Sign;
          default:
            return 0;
        }
      }
    }

    /// <summary>Gets a value indicating whether this value is zero.</summary>
    /// <value>True if this value is zero; otherwise, false.</value>
    public bool IsValueZero {
      get {
        switch (this.integerMode) {
          case 0:
            return this.smallValue == 0;
          case 1:
            return this.mnum.Sign == 0;
          case 2:
            return this.largeValue.IsZero;
          default:
            return false;
        }
      }
    }

    /// <summary>Compares a 32-bit signed integer with this instance.</summary>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    /// <param name='val'>A 32-bit signed integer.</param>
    public int CompareToInt(int val) {
      switch (this.integerMode) {
        case 0:
          return (val == this.smallValue) ? 0 : (this.smallValue < val ? -1 : 1);
        case 1:
          return this.mnum.ToBigInteger().CompareTo((BigInteger)val);
        case 2:
          return this.largeValue.CompareTo((BigInteger)val);
        default:
          return 0;
      }
    }

    public BigInteger AsBigInteger() {
      switch (this.integerMode) {
        case 0:
          return BigInteger.valueOf(this.smallValue);
        case 1:
          return this.mnum.ToBigInteger();
        case 2:
          return this.largeValue;
        default:
          throw new InvalidOperationException();
      }
    }
  }
}
