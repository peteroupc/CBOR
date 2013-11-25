package com.upokecenter.util;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 11/23/2013
 * Time: 6:13 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import java.math.*;

    /**
     * Description of FastInteger2.
     */
  class FastInteger2 {
    long smallValue;
    MutableBigInteger largeValue;
    boolean usingLarge;

    public FastInteger2() {
      smallValue = 0;
      usingLarge = false;
    }

    /**
     * 
     */
    public FastInteger2 MultiplyByTen() {
      if (usingLarge) {
        largeValue.Multiply(10);
      } else {
        boolean apos = (smallValue > 0L);
        if (
          (apos && ((smallValue > 922337203685477580L))) ||
          (!apos && ((smallValue < -922337203685477580L)))) {
          // would overflow, convert to large
          largeValue = new MutableBigInteger(smallValue);
          usingLarge = true;
          largeValue.Multiply(10);
        } else {
          smallValue *= 10;
        }
      }
      return this;
    }

    /**
     * 
     * @param val A 32-bit signed integer.
     */
    public FastInteger2 Add(int val) {
      if (val != 0) {
        if (usingLarge) {
          largeValue.Add(val);
        } else if ((smallValue < 0 && (long)val < Long.MIN_VALUE - smallValue) ||
                  (smallValue > 0 && (long)val > Long.MAX_VALUE - smallValue)) {
          // would overflow, convert to large
          largeValue = new MutableBigInteger(smallValue);
          usingLarge = true;
          largeValue.Add(val);
        } else {
          smallValue += val;
        }
      }
      return this;
    }

    /**
     * 
     * @param a A FastInteger object.
     */
    public void AddThisTo(FastInteger a) {
      if (usingLarge) {
        a.Add(new FastInteger(largeValue.ToBigInteger()));
      } else {
        a.Add(smallValue);
      }
    }
    /**
     * 
     * @param a A FastInteger object.
     */
    public void SubtractThisFrom(FastInteger a) {
      if (usingLarge) {
        a.Subtract(new FastInteger(largeValue.ToBigInteger()));
      } else {
        a.Subtract(smallValue);
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
    public BigInteger AsBigInteger() {
      return usingLarge ? largeValue.ToBigInteger() : BigInteger.valueOf(smallValue);
    }
    /**
     * 
     */
    public BigInteger AsNegatedBigInteger() {
      if (usingLarge) {
        BigInteger bigint = largeValue.ToBigInteger();
        bigint=(bigint).negate();
        return bigint;
      } else {
        BigInteger bigint=(BigInteger.valueOf(smallValue)).negate();
        return bigint;
      }
    }
  }
