package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */


import java.math.*;

    /**
     * A mutable integer class initially backed by a 64-bit integer, that
     * only uses a big integer when arithmetic operations would overflow
     * the 64-bit integer. <p> This class is ideal for cases where operations
     * should be arbitrary precision, but the need to use a precision greater
     * than 64 bits is very rare.</p> <p> Many methods in this class return
     * a reference to the same object as used in the call. This allows chaining
     * operations in a single line of code. For example:</p> <code> fastInt.Add(5).Multiply(10);</code>
     */
  final class FastInteger implements Comparable<FastInteger> {
    long smallValue;
    BigInteger largeValue;
    boolean usingLarge;

    private static BigInteger Int64MinValue = BigInteger.valueOf(Long.MIN_VALUE);
    private static BigInteger Int64MaxValue = BigInteger.valueOf(Long.MAX_VALUE);
    private static BigInteger Int32MinValue = BigInteger.valueOf(Integer.MIN_VALUE);
    private static BigInteger Int32MaxValue = BigInteger.valueOf(Integer.MAX_VALUE);

    public FastInteger() {
    }

    public FastInteger(long value) {
      smallValue = value;
    }

    public FastInteger(FastInteger value) {
      smallValue = value.smallValue;
      usingLarge = value.usingLarge;
      largeValue = value.largeValue;
    }

    // This constructor converts a big integer to a 64-bit one
    // if it's small enough
    public FastInteger(BigInteger bigintVal) {
      int sign = bigintVal.signum();
      if (sign == 0 ||
         (sign < 0 && bigintVal.compareTo(Int64MinValue) >= 0) ||
         (sign > 0 && bigintVal.compareTo(Int64MaxValue) <= 0)) {
        smallValue = bigintVal.longValue();
        usingLarge = false;
      } else {
        usingLarge = true;
        largeValue = bigintVal;
      }
    }

    /**
     * 
     */
    public int AsInt32() {
      if (usingLarge) {
        return largeValue.intValue();
      } else {
        return ((int)smallValue);
      }
    }
    /**
     * 
     */
    public long AsInt64() {
      if (usingLarge) {
        return largeValue.longValue();
      } else {
        return ((long)smallValue);
      }
    }
    /**
     * Compares a FastInteger object with this instance.
     * @param val A FastInteger object.
     * @return Zero if the values are equal; a negative number is this instance
     * is less, or a positive number if this instance is greater.
     */
    public int compareTo(FastInteger val) {
      if (usingLarge || val.usingLarge) {
        BigInteger valValue = val.AsBigInteger();
        return largeValue.compareTo(valValue);
      } else {
        return (val.smallValue == smallValue) ? 0 :
          (smallValue < val.smallValue ? -1 : 1);
      }
    }
    /**
     * 
     */
    public FastInteger Abs() {
      return (this.signum() < 0) ? Negate() : this;
    }
    /**
     * Sets this object's value to the remainder of the current value divided
     * by the given integer.
     * @param divisor The divisor.
     * @return This object.
     */
    public FastInteger Mod(int divisor) {
      if (usingLarge) {
        // Mod operator will always result in a
        // number that fits an int for int divisors
        largeValue = largeValue.remainder(BigInteger.valueOf(divisor));
        smallValue = largeValue.intValue();
        usingLarge = false;
      } else {
        smallValue %= divisor;
      }
      return this;
    }

    /**
     * Compares a Int64 object with this instance.
     * @param val A 64-bit signed integer.
     * @return Zero if the values are equal; a negative number is this instance
     * is less, or a positive number if this instance is greater.
     */
    public int compareTo(long val) {
      if (usingLarge) {
        return largeValue.compareTo(BigInteger.valueOf(val));
      } else {
        return (val == smallValue) ? 0 : (smallValue < val ? -1 : 1);
      }
    }

    /**
     * Sets this object's value to the current value times another integer.
     * @param val The integer to multiply by.
     * @return This object.
     */
    public FastInteger Multiply(int val) {
      if (val == 0) {
        smallValue = 0;
        usingLarge = false;
      } else if (usingLarge) {
        largeValue=largeValue.multiply(BigInteger.valueOf(val));
      } else {
        boolean apos = (smallValue > 0L);
        boolean bpos = (val > 0L);
        if (
          (apos && ((!bpos && (Long.MIN_VALUE / smallValue) > val) ||
                    (bpos && smallValue > (Long.MAX_VALUE / val)))) ||
          (!apos && ((!bpos && smallValue != 0L &&
                      (Long.MAX_VALUE / smallValue) > val) ||
                     (bpos && smallValue < (Long.MIN_VALUE / val))))) {
          // would overflow, convert to large
          largeValue = BigInteger.valueOf(smallValue);
          usingLarge = true;
          largeValue=largeValue.multiply(BigInteger.valueOf(val));
        } else {
          smallValue *= val;
        }
      }
      return this;
    }

    /**
     * Sets this object's value to 0 minus its current value (reverses its
     * sign).
     * @return This object.
     */
    public FastInteger Negate() {
      if (usingLarge) {
        largeValue=(largeValue).negate();
      } else {
        if (smallValue == Long.MIN_VALUE) {
          // would overflow, convert to large
          largeValue = BigInteger.valueOf(smallValue);
          usingLarge = true;
          largeValue=(largeValue).negate();
        } else {
          smallValue = -smallValue;
        }
      }
      return this;
    }

    /**
     * Sets this object's value to the current value minus the given FastInteger
     * value.
     * @param val The subtrahend.
     * @return This object.
     */
    public FastInteger Subtract(FastInteger val) {
      if (usingLarge || val.usingLarge) {
        BigInteger valValue = val.AsBigInteger();
        largeValue=largeValue.subtract(valValue);
      } else if (((long)val.smallValue < 0 && Long.MAX_VALUE + (long)val.smallValue < smallValue) ||
                ((long)val.smallValue > 0 && Long.MIN_VALUE + (long)val.smallValue > smallValue)) {
        // would overflow, convert to large
        largeValue = BigInteger.valueOf(smallValue);
        usingLarge = true;
        largeValue=largeValue.subtract(BigInteger.valueOf(val.smallValue));
      } else {
        smallValue -= val.smallValue;
      }
      return this;
    }
    /**
     * Sets this object's value to the current value minus the given integer
     * value.
     * @param val The subtrahend.
     * @return This object.
     */
    public FastInteger Subtract(long val) {
      if (usingLarge) {
        BigInteger valValue = BigInteger.valueOf(val);
        largeValue=largeValue.subtract(valValue);
      } else if (((long)val < 0 && Long.MAX_VALUE + (long)val < smallValue) ||
                ((long)val > 0 && Long.MIN_VALUE + (long)val > smallValue)) {
        // would overflow, convert to large
        largeValue = BigInteger.valueOf(smallValue);
        usingLarge = true;
        largeValue=largeValue.subtract(BigInteger.valueOf(val));
      } else {
        smallValue -= val;
      }
      return this;
    }

    /**
     * Sets this object's value to the current value minus the given integer.
     * @param val The subtrahend.
     * @return This object.
     */
    public FastInteger Subtract(int val) {
      if (usingLarge) {
        largeValue=largeValue.subtract(BigInteger.valueOf(val));
      } else if (((long)val < 0 && Long.MAX_VALUE + (long)val < smallValue) ||
                ((long)val > 0 && Long.MIN_VALUE + (long)val > smallValue)) {
        // would overflow, convert to large
        largeValue = BigInteger.valueOf(smallValue);
        usingLarge = true;
        largeValue=largeValue.subtract(BigInteger.valueOf(val));
      } else {
        smallValue -= val;
      }
      return this;
    }

    /**
     * Sets this object's value to the current value minus the given integer.
     * @param bigintVal The subtrahend.
     * @return This object.
     */
    public FastInteger Subtract(BigInteger bigintVal) {
      if (usingLarge) {
        largeValue=largeValue.subtract(bigintVal);
      } else {
        int sign = bigintVal.signum();
        if (sign == 0 ||
           (sign < 0 && bigintVal.compareTo(Int64MinValue) >= 0) ||
           (sign > 0 && bigintVal.compareTo(Int64MaxValue) <= 0)) {
          long val = bigintVal.longValue();
          if ((val < 0 && Long.MAX_VALUE + val < smallValue) ||
             (val > 0 && Long.MIN_VALUE + val > smallValue)) {
            // would overflow, convert to large
            largeValue = BigInteger.valueOf(smallValue);
            usingLarge = true;
            largeValue=largeValue.subtract(BigInteger.valueOf(val));
          } else {
            smallValue -= val;
          }
        } else {
          // convert to large
          largeValue = BigInteger.valueOf(smallValue);
          usingLarge = true;
          largeValue=largeValue.subtract(bigintVal);
        }
      }
      return this;
    }
    /**
     * 
     * @param val A FastInteger object.
     */
    public FastInteger Add(FastInteger val) {
      if (usingLarge || val.usingLarge) {
        BigInteger valValue = val.AsBigInteger();
        largeValue=largeValue.add(valValue);
      } else if ((smallValue < 0 && (long)val.smallValue < Long.MIN_VALUE - smallValue) ||
                (smallValue > 0 && (long)val.smallValue > Long.MAX_VALUE - smallValue)) {
        // would overflow, convert to large
        largeValue = BigInteger.valueOf(smallValue);
        usingLarge = true;
        largeValue=largeValue.add(BigInteger.valueOf(val.smallValue));
      } else {
        smallValue += val.smallValue;
      }
      return this;
    }

    /**
     * Divides this instance by the value of a Int32 object.
     * @param divisor A 32-bit signed integer.
     * @return The quotient of the two objects.
     */
    public FastInteger Divide(int divisor) {
      if (divisor != 0) {
        if (usingLarge) {
          largeValue=largeValue.divide(BigInteger.valueOf(divisor));
        } else if (divisor == -1 && smallValue == Long.MIN_VALUE) {
          // would overflow, convert to large
          largeValue = BigInteger.valueOf(smallValue);
          usingLarge = true;
          largeValue=largeValue.divide(BigInteger.valueOf(divisor));
        } else {
          smallValue /= divisor;
        }
      }
      return this;
    }

    /**
     * 
     * @param val A 32-bit signed integer.
     */
    public FastInteger Add(int val) {
      if (val != 0) {
        if (usingLarge) {
          largeValue=largeValue.add(BigInteger.valueOf(val));
        } else if ((smallValue < 0 && (long)val < Long.MIN_VALUE - smallValue) ||
                  (smallValue > 0 && (long)val > Long.MAX_VALUE - smallValue)) {
          // would overflow, convert to large
          largeValue = BigInteger.valueOf(smallValue);
          usingLarge = true;
          largeValue=largeValue.add(BigInteger.valueOf(val));
        } else {
          smallValue += val;
        }
      }
      return this;
    }
    /**
     * 
     * @param val A 64-bit signed integer.
     */
    public FastInteger Add(long val) {
      if (val != 0) {
        if (usingLarge) {
          largeValue=largeValue.add(BigInteger.valueOf(val));
        } else if ((smallValue < 0 && val < Long.MIN_VALUE - smallValue) ||
                  (smallValue > 0 && val > Long.MAX_VALUE - smallValue)) {
          // would overflow, convert to large
          largeValue = BigInteger.valueOf(smallValue);
          usingLarge = true;
          largeValue=largeValue.add(BigInteger.valueOf(val));
        } else {
          smallValue += val;
        }
      }
      return this;
    }

    /**
     * Gets whether this object's value is within the range of a 64-bit signed
     * integer.
     */
    public boolean CanFitInInt64() {
      if (!usingLarge) {
        return true;
      } else {
        int sign = largeValue.signum();
        if (sign == 0) return true;
        if (sign < 0) return largeValue.compareTo(Int64MinValue) >= 0;
        return largeValue.compareTo(Int64MaxValue) <= 0;
      }
    }

    /**
     * Gets whether this object's value is within the range of a 32-bit signed
     * integer.
     */
    public boolean CanFitInInt32() {
      if (usingLarge) {
        int sign = largeValue.signum();
        if (sign == 0) return true;
        if (sign < 0) return largeValue.compareTo(Int32MinValue) >= 0;
        return largeValue.compareTo(Int32MaxValue) <= 0;
      } else {
        return (smallValue >= Integer.MIN_VALUE &&
                smallValue <= Integer.MAX_VALUE);
      }
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return usingLarge ? largeValue.toString() : Long.toString((long)smallValue);
    }
    /**
     * 
     */
    public int signum() {
        return usingLarge ? largeValue.signum() : (
          (smallValue == 0) ? 0 : (smallValue < 0 ? -1 : 1));
      }

    /**
     * 
     */
    public BigInteger AsBigInteger() {
      return usingLarge ? largeValue : BigInteger.valueOf(smallValue);
    }
  }
