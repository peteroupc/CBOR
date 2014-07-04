package com.upokecenter.cbor;
import com.upokecenter.util.*;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the private Domain.
http://creativecommons.org/privatedomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

// import java.math.*;

/**
 * A mutable integer class initially backed by a small integer, that
 * only uses a big integer when arithmetic operations would overflow
 * the small integer.<p> This class is ideal for cases where operations
 * should be arbitrary precision, but the need to use a high precision
 * is rare.</p> <p> Many methods in this class return a reference to the
 * same object as used in the call. This allows chaining operations in
 * a single line of code. For example:</p> <code> fastInt.Add(5).Multiply(10);
 * </code>
 */
final class FastInteger {
  private static final class MutableNumber {
    private int[] data;

    private int wordCount;

    private static MutableNumber FromBigInteger(final BigInteger bigintVal) {
      MutableNumber mnum = new MutableNumber(0);
      if (bigintVal.signum() < 0) {
        throw new IllegalArgumentException("bigintVal's sign (" + bigintVal + ") is not greater or equal to " + "0");
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
          int x = ((int)bytes[i]) & 0xFF;
          if (i + 1 < len) {
            x |= (((int)bytes[i + 1]) & 0xFF) << 8;
          }
          if (i + 2 < len) {
            x |= (((int)bytes[i + 2]) & 0xFF) << 16;
          }
          if (i + 3 < len) {
            x |= (((int)bytes[i + 3]) & 0xFF) << 24;
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

    private MutableNumber (final int val) {
      if (val < 0) {
        throw new IllegalArgumentException("val (" + Long.toString((long)val) + ") is not greater or equal to " + "0");
      }
      this.data = new int[4];
      this.wordCount = (val == 0) ? 0 : 1;
      this.data[0] = ((int)(val & 0xFFFFFFFFL));
    }

    /**
     * Not documented yet.
     * @return A BigInteger object.
     */
    private BigInteger ToBigInteger() {
      if (this.wordCount == 1 && (this.data[0] >> 31) == 0) {
        return BigInteger.valueOf((int)this.data[0]);
      }
      byte[] bytes = new byte[(this.wordCount * 4) + 1];
      for (int i = 0; i < this.wordCount; ++i) {
        bytes[i * 4] = (byte)(this.data[i] & 0xFF);
        bytes[(i * 4) + 1] = (byte)((this.data[i] >> 8) & 0xFF);
        bytes[(i * 4) + 2] = (byte)((this.data[i] >> 16) & 0xFF);
        bytes[(i * 4) + 3] = (byte)((this.data[i] >> 24) & 0xFF);
      }
      bytes[bytes.length - 1] = (byte)0;
      return BigInteger.fromByteArray((byte[])bytes,true);
    }

    /**
     * Not documented yet.
     * @return A Boolean object.
     */
    private boolean CanFitInInt32() {
      return this.wordCount == 0 || (this.wordCount == 1 && (this.data[0] >> 31) == 0);
    }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
    private int ToInt32() {
      return this.wordCount == 0 ? 0 : this.data[0];
    }

    /**
     * Multiplies this instance by the value of a 32-bit signed integer.
     * @param multiplicand A 32-bit signed integer.
     * @return The product of the two objects.
     */
    private MutableNumber Multiply(final int multiplicand) {
      if (multiplicand < 0) {
        throw new IllegalArgumentException("multiplicand (" + Long.toString((long)multiplicand) + ") is not greater or equal to " + "0");
      } else if (multiplicand != 0) {
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
            result1 = (temp >> 16) & 65535; result0 = temp & 65535;
            result2 = 0;
            temp = (x1 * y0);  // b * c
            result2 += (temp >> 16) & 65535; result1 += temp & 65535;
            result2 += (result1 >> 16) & 65535;
            result1 &= 65535;
            result3 = (result2 >> 16) & 65535;
            result2 &= 65535;
            // Add carry
            x0 = ((int)(result0 | (result1 << 16)));
            x1 = ((int)(result2 | (result3 << 16)));
            int x2 = (x0 + carry);
            if (((x2 >> 31) == (x0 >> 31)) ? ((x2 & Integer.MAX_VALUE) < (x0 & Integer.MAX_VALUE)) :
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
            result1 = (temp >> 16) & 65535; result0 = temp & 65535;
            temp = (x0 * y1);  // a * d
            result2 = (temp >> 16) & 65535; result1 += temp & 65535;
            result2 += (result1 >> 16) & 65535;
            result1 &= 65535;
            temp = (x1 * y0);  // b * c
            result2 += (temp >> 16) & 65535; result1 += temp & 65535;
            result2 += (result1 >> 16) & 65535;
            result1 &= 65535;
            result3 = (result2 >> 16) & 65535;
            result2 &= 65535;
            temp = (x1 * y1);  // b * d
            result3 += (temp >> 16) & 65535; result2 += temp & 65535;
            result3 += (result2 >> 16) & 65535;
            result2 &= 65535;
            // Add carry
            x0 = ((int)(result0 | (result1 << 16)));
            x1 = ((int)(result2 | (result3 << 16)));
            int x2 = (x0 + carry);
            if (((x2 >> 31) == (x0 >> 31)) ? ((x2 & Integer.MAX_VALUE) < (x0 & Integer.MAX_VALUE)) :
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

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    private int signum() {
      return this.wordCount == 0 ? 0 : 1;
    }

    /**
     * Subtracts a 32-bit signed integer from this instance.
     * @param other A 32-bit signed integer.
     * @return The difference of the two objects.
     */
    private MutableNumber SubtractInt(
        final int other) {
      if (other < 0) {
        throw new IllegalArgumentException("other (" + Long.toString((long)other) + ") is not greater or equal to " + "0");
      } else if (other != 0) {
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
                  ((this.data[i] & Integer.MAX_VALUE) < (u & Integer.MAX_VALUE)) :
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
    private MutableNumber Subtract(
        final MutableNumber other) {
      {
        {
          // System.out.println("" + this.data.length + " " + (other.data.length));
          int neededSize = (this.wordCount > other.wordCount) ? this.wordCount : other.wordCount;
          if (this.data.length < neededSize) {
            int[] newdata = new int[neededSize + 20];
            System.arraycopy(this.data, 0, newdata, 0, this.data.length);
            this.data = newdata;
          }
          neededSize = (this.wordCount < other.wordCount) ? this.wordCount : other.wordCount;
          int u = 0;
          int borrow = 0;
          for (int i = 0; i < neededSize; ++i) {
            int a = this.data[i];
            u = (a - other.data[i]) - borrow;
            borrow = ((((a >> 31) == (u >> 31)) ? ((a & Integer.MAX_VALUE) < (u & Integer.MAX_VALUE)) :
              ((a >> 31) == 0)) || (a == u && other.data[i] != 0)) ? 1 : 0;
            this.data[i] = (int)u;
          }
          if (borrow != 0) {
            for (int i = neededSize; i < this.wordCount; ++i) {
              int a = this.data[i];
              u = (a - other.data[i]) - borrow;
              borrow = ((((a >> 31) == (u >> 31)) ? ((a & Integer.MAX_VALUE) < (u & Integer.MAX_VALUE)) :
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

    /**
     * Adds a 32-bit signed integer to this instance.
     * @param augend A 32-bit signed integer.
     * @return This instance.
     */
    private MutableNumber Add(final int augend) {
      if (augend < 0) {
        throw new IllegalArgumentException("augend (" + augend + ") is not greater or equal to " + "0");
      } else if (augend != 0) {
        int carry = 0;
        // Ensure a length of at least 1
        if (this.wordCount == 0) {
          if (this.data.length == 0) {
            this.data = new int[4];
          }
          this.data[0] = 0;
          this.wordCount = 1;
        }
        int aug = augend;
        for (int i = 0; i < this.wordCount; ++i) {
          int u;
          int a = this.data[i];
          u = (a + aug) + carry;
          carry = ((((u >> 31) == (a >> 31)) ? ((u & Integer.MAX_VALUE) < (a & Integer.MAX_VALUE)) :
            ((u >> 31) == 0)) || (u == a && aug != 0)) ? 1 : 0;
          this.data[i] = u;
          if (carry == 0) {
            return this;
          }
          aug = 0;
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

  private int smallValue;  // if integerMode is 0
  private MutableNumber mnum;  // if integerMode is 1
  private BigInteger largeValue;  // if integerMode is 2
  private int integerMode = 0;

  private static BigInteger valueInt32MinValue = BigInteger.valueOf(Integer.MIN_VALUE);
  private static BigInteger valueNegativeInt32MinValue = (valueInt32MinValue).negate();

  FastInteger (final int value) {
    this.smallValue = value;
  }

  private static FastInteger FromBig(final BigInteger bigintVal) {
    if (bigintVal.canFitInInt()) {
      return new FastInteger(bigintVal.intValue());
    } else if (bigintVal.signum() > 0) {
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

  /**
   * Not documented yet.
   * @return A 32-bit signed integer.
   */
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
    * Sets this object&apos;s value to the current value times another
    * integer.
    * @param val The integer to multiply by.
    * @return This object.
    */
   FastInteger Multiply(final int val) {
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
    * Sets this object&apos;s value to the current value minus the given
    * FastInteger value.
    * @param val The subtrahend.
    * @return This object.
    */
   FastInteger Subtract(final FastInteger val) {
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
    * Sets this object&apos;s value to the current value minus the given
    * integer.
    * @param val The subtrahend.
    * @return This object.
    */
   FastInteger SubtractInt(final int val) {
     if (val == Integer.MIN_VALUE) {
       return this.AddBig(valueNegativeInt32MinValue);
     } else if (this.integerMode == 0) {
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
     } else {
       return this.AddInt(-val);
     }
   }

   /**
    * Sets this object&apos;s value to the current value plus the given
    * integer.
    * @param bigintVal The number to add.
    * @return This object.
    */
   private FastInteger AddBig(final BigInteger bigintVal) {
     switch (this.integerMode) {
     case 0: {
       if (bigintVal.canFitInInt()) {
         return this.AddInt(bigintVal.intValue());
       }
       return this.Add(FastInteger.FromBig(bigintVal));
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
    * Not documented yet.
    * @param val A FastInteger object. (2).
    * @return A FastInteger object.
    */
   FastInteger Add(final FastInteger val) {
     BigInteger valValue;
     switch (this.integerMode) {
     case 0:
       if (val.integerMode == 0) {
         if ((this.smallValue < 0 && (int)val.smallValue < Integer.MIN_VALUE - this.smallValue) ||
             (this.smallValue > 0 && (int)val.smallValue > Integer.MAX_VALUE - this.smallValue)) {
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
    * Adds a 32-bit signed integer to this instance.
    * @param val A 32-bit signed integer.
    * @return This instance.
    */
   FastInteger AddInt(final int val) {
     BigInteger valValue;
     switch (this.integerMode) {
     case 0:
       if ((this.smallValue < 0 && (int)val < Integer.MIN_VALUE - this.smallValue) ||
           (this.smallValue > 0 && (int)val > Integer.MAX_VALUE - this.smallValue)) {
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

   /**
    * Not documented yet.
    * @return A Boolean object.
    */
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
    * {@inheritDoc}
    *
    * Converts this object to a text string.
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
   int signum() {
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
    * Not documented yet.
    * @return A BigInteger object.
    */
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
