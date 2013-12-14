package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */


//import java.math.*;

    /**
     * A mutable integer class initially backed by a small integer, that
     * only uses a big integer when arithmetic operations would overflow
     * the small integer. <p> This class is ideal for cases where operations
     * should be arbitrary precision, but the need to use a high precision
     * is rare.</p> <p> Many methods in this class return a reference to the
     * same object as used in the call. This allows chaining operations in
     * a single line of code. For example:</p> <code> fastInt.Add(5).Multiply(10);</code>
     */
  final class FastInteger implements Comparable<FastInteger> {
    int smallValue; // if integerMode is 0
    MutableNumber mnum; // if integerMode is 1
    BigInteger largeValue; // if integerMode is 2
    int integerMode;

    private static BigInteger Int32MinValue = BigInteger.valueOf(Integer.MIN_VALUE);
    private static BigInteger Int32MaxValue = BigInteger.valueOf(Integer.MAX_VALUE);
    private static BigInteger NegativeInt32MinValue=(Int32MinValue).negate();

    public FastInteger() {
    }

    public FastInteger(int value) {
      smallValue = value;
    }

    public FastInteger(FastInteger value) {
      smallValue = value.smallValue;
      integerMode = value.integerMode;
      largeValue = value.largeValue;
      mnum=(value.mnum==null) ? null : value.mnum.Copy();
    }

    public FastInteger(BigInteger bigintVal) {
      int sign = bigintVal.signum();
      if (sign == 0 ||
          (sign < 0 && bigintVal.compareTo(Int32MinValue) >= 0) ||
          (sign > 0 && bigintVal.compareTo(Int32MaxValue) <= 0)) {
        integerMode=0;
        smallValue = bigintVal.intValue();
      } else if(sign>0){
        integerMode=1;
        mnum=new MutableNumber(bigintVal);
      } else {
        integerMode=2;
        largeValue = bigintVal;
      }
    }

    /**
     * 
     */
    public int AsInt32() {
      switch(this.integerMode){
        case 0:
          return smallValue;
          case 1:{
            BigInteger bigint=mnum.ToBigInteger();
            return bigint.intValue();
          }
        case 2:
          return largeValue.intValue();
        default:
          throw new IllegalStateException();
      }
    }
    
    /**
     * Compares a FastInteger object with this instance.
     * @param val A FastInteger object.
     * @return Zero if the values are equal; a negative number is this instance
     * is less, or a positive number if this instance is greater.
     */
    public int compareTo(FastInteger val) {
      switch(this.integerMode){
        case 0:
          if(val.integerMode==0){
            return (val.smallValue == smallValue) ? 0 :
              (smallValue < val.smallValue ? -1 : 1);
          } else {
            return AsBigInteger().compareTo(val.AsBigInteger());
          }
        case 1:
          return AsBigInteger().compareTo(val.AsBigInteger());
        case 2:
          return AsBigInteger().compareTo(val.AsBigInteger());
        default:
          throw new IllegalStateException();
      }
    }
    /**
     * 
     */
    public FastInteger Abs() {
      return (this.signum() < 0) ? Negate() : this;
    }

    /**
     * Sets this object's value to the current value times another integer.
     * @param val The integer to multiply by.
     * @return This object.
     */
    public FastInteger Multiply(int val) {
      if (val == 0) {
        smallValue = 0;
        integerMode=0;
      } else {
        switch (integerMode) {
          case 0:
            boolean apos = (smallValue > 0L);
            boolean bpos = (val > 0L);
            if (
              (apos && ((!bpos && (Integer.MIN_VALUE / smallValue) > val) ||
                        (bpos && smallValue > (Integer.MAX_VALUE / val)))) ||
              (!apos && ((!bpos && smallValue != 0L &&
                          (Integer.MAX_VALUE / smallValue) > val) ||
                         (bpos && smallValue < (Integer.MIN_VALUE / val))))) {
              // would overflow, convert to large
              if(apos && bpos){
                // if both operands are nonnegative
                // convert to mutable big integer
                integerMode=1;
                mnum=new MutableNumber(smallValue);
                mnum.Multiply(val);
              } else {
                // if either operand is negative
                // convert to big integer
                integerMode=2;
                largeValue = BigInteger.valueOf(smallValue);
                largeValue=largeValue.multiply(BigInteger.valueOf(val));
              }
            } else {
              smallValue *= val;
            }
            break;
          case 1:
            if(val<0){
              integerMode=2;
              largeValue=mnum.ToBigInteger();
              largeValue=largeValue.multiply(BigInteger.valueOf(val));
            } else {
              mnum.Multiply(val);
            }
            break;
          case 2:
            largeValue=largeValue.multiply(BigInteger.valueOf(val));
            break;
          default:
            throw new IllegalStateException();
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
      switch (integerMode) {
        case 0:
          if (smallValue == Integer.MIN_VALUE) {
            // would overflow, convert to large
            integerMode=1;
            mnum = new MutableNumber(NegativeInt32MinValue);
          } else {
            smallValue = -smallValue;
          }
          break;
        case 1:
          integerMode=2;
          largeValue=mnum.ToBigInteger();
          largeValue=(largeValue).negate();
          break;
        case 2:
          largeValue=(largeValue).negate();
          break;
        default:
          throw new IllegalStateException();
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
      BigInteger valValue;
      switch (integerMode) {
        case 0:
          if(val.integerMode==0){
            if (((int)val.smallValue < 0 && Integer.MAX_VALUE + (int)val.smallValue < smallValue) ||
                ((int)val.smallValue > 0 && Integer.MIN_VALUE + (int)val.smallValue > smallValue)) {
              // would overflow, convert to large
              integerMode=2;
              largeValue = BigInteger.valueOf(smallValue);
              largeValue=largeValue.subtract(BigInteger.valueOf(val.smallValue));
            } else {
              smallValue-=val.smallValue;
            }
          } else {
            integerMode=2;
            largeValue=BigInteger.valueOf(smallValue);
            valValue = val.AsBigInteger();
            largeValue=largeValue.subtract(valValue);
          }
          break;
        case 1:
          integerMode=2;
          largeValue=mnum.ToBigInteger();
          valValue = val.AsBigInteger();
          largeValue=largeValue.subtract(valValue);
          break;
        case 2:
          valValue = val.AsBigInteger();
          largeValue=largeValue.subtract(valValue);
          break;
        default:
          throw new IllegalStateException();
      }
      return this;
    }
    /**
     * Sets this object's value to the current value minus the given integer.
     * @param val The subtrahend.
     * @return This object.
     */
    public FastInteger Subtract(int val) {
      if(val==Integer.MIN_VALUE){
        return Add(NegativeInt32MinValue);
      } else {
        return Add(-val);
      }
    }
    
    

    /**
     * Sets this object's value to the current value plus the given integer.
     * @param bigintVal The number to add.
     * @return This object.
     */
    public FastInteger Add(BigInteger bigintVal) {
      switch (integerMode) {
          case 0:{
            int sign = bigintVal.signum();
            if (sign == 0 ||
                (sign < 0 && bigintVal.compareTo(Int32MinValue) >= 0) ||
                (sign > 0 && bigintVal.compareTo(Int32MaxValue) <= 0)) {
              return Add(bigintVal.intValue());
            }
            return Add(new FastInteger(bigintVal));
          }
        case 1:
          integerMode=2;
          largeValue=mnum.ToBigInteger();
          largeValue=largeValue.add(bigintVal);
          break;
        case 2:
          largeValue=largeValue.add(bigintVal);
          break;
        default:
          throw new IllegalStateException();
      }
      return this;
    }
    

    /**
     * Sets this object's value to the current value minus the given integer.
     * @param bigintVal The subtrahend.
     * @return This object.
     */
    public FastInteger Subtract(BigInteger bigintVal) {
      if (integerMode==2) {
        largeValue=largeValue.subtract(bigintVal);
        return this;
      } else {
        int sign = bigintVal.signum();
        if (sign == 0)return this;
        // Check if this value fits an int, except if
        // it's MinValue
        if(sign < 0 && bigintVal.compareTo(Int32MinValue) > 0){
          return Add(-(bigintVal.intValue()));
        }
        if(sign > 0 && bigintVal.compareTo(Int32MaxValue) <= 0){
          return Subtract(bigintVal.intValue());
        }
        bigintVal=bigintVal.negate();
        return Add(bigintVal);
      }
    }
    /**
     * 
     * @param val A FastInteger object.
     */
    public FastInteger Add(FastInteger val) {
      BigInteger valValue;
      switch (integerMode) {
        case 0:
          if(val.integerMode==0){
            if ((smallValue < 0 && (int)val.smallValue < Integer.MIN_VALUE - smallValue) ||
                (smallValue > 0 && (int)val.smallValue > Integer.MAX_VALUE - smallValue)) {
              // would overflow
              if(val.smallValue>=0){
                integerMode=1;
                mnum=new MutableNumber(this.smallValue);
                mnum.Add(val.smallValue);
              } else {
                integerMode=2;
                largeValue = BigInteger.valueOf(smallValue);
                largeValue=largeValue.add(BigInteger.valueOf(val.smallValue));
              }
            } else {
              smallValue+=val.smallValue;
            }
          } else {
            integerMode=2;
            largeValue=BigInteger.valueOf(smallValue);
            valValue = val.AsBigInteger();
            largeValue=largeValue.add(valValue);
          }
          break;
        case 1:
          if(val.integerMode==0 && val.smallValue>=0){
            mnum.Add(val.smallValue);
          } else {
            integerMode=2;
            largeValue=mnum.ToBigInteger();
            valValue = val.AsBigInteger();
            largeValue=largeValue.add(valValue);
          }
          break;
        case 2:
          valValue = val.AsBigInteger();
          largeValue=largeValue.add(valValue);
          break;
        default:
          throw new IllegalStateException();
      }
      return this;
    }
    /**
     * Sets this object's value to the remainder of the current value divided
     * by the given integer.
     * @param divisor The divisor.
     * @return This object.
     */
    public FastInteger Mod(int divisor) {
      // Mod operator will always result in a
      // number that fits an int for int divisors
      if (divisor != 0) {
        switch (integerMode) {
          case 0:
            smallValue %= divisor;
            break;
          case 1:
            largeValue=mnum.ToBigInteger();
            largeValue = largeValue.remainder(BigInteger.valueOf(divisor));
            smallValue = largeValue.intValue();
            integerMode=0;
            break;
          case 2:
            largeValue = largeValue.remainder(BigInteger.valueOf(divisor));
            smallValue = largeValue.intValue();
            integerMode=0;
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
     * Divides this instance by the value of a Int32 object.
     * @param divisor A 32-bit signed integer.
     * @return The quotient of the two objects.
     */
    public FastInteger Divide(int divisor) {
      if (divisor != 0) {
        switch (integerMode) {
          case 0:
            if (divisor == -1 && smallValue == Integer.MIN_VALUE) {
              // would overflow, convert to large
              integerMode=1;
              mnum = new MutableNumber(NegativeInt32MinValue);
            } else {
              smallValue /= divisor;
            }
            break;
          case 1:
            integerMode=2;
            largeValue=mnum.ToBigInteger();
            largeValue=largeValue.divide(BigInteger.valueOf(divisor));
            if(largeValue.signum()==0){
              integerMode=0;
              smallValue=0;
            }
            break;
          case 2:
            largeValue=largeValue.divide(BigInteger.valueOf(divisor));
            if(largeValue.signum()==0){
              integerMode=0;
              smallValue=0;
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
     * 
     */
    public boolean isEvenNumber() {
        switch (integerMode) {
          case 0:
            return (smallValue&1)==0;
          case 1:
            return mnum.isEvenNumber();
          case 2:
            return largeValue.testBit(0)==false;
          default:
            throw new IllegalStateException();
        }
      }
    /**
     * 
     * @param val A 64-bit signed integer.
     */
    public FastInteger Add(int val) {
      BigInteger valValue;
      switch (integerMode) {
        case 0:
          if ((smallValue < 0 && (int)val < Integer.MIN_VALUE - smallValue) ||
              (smallValue > 0 && (int)val > Integer.MAX_VALUE - smallValue)) {
            // would overflow
            if(val>=0){
              integerMode=1;
              mnum=new MutableNumber(this.smallValue);
              mnum.Add(val);
            } else {
              integerMode=2;
              largeValue = BigInteger.valueOf(smallValue);
              largeValue=largeValue.add(BigInteger.valueOf(val));
            }
          } else {
            smallValue+=val;
          }
          break;
        case 1:
          if(val>=0){
            mnum.Add(val);
          } else {
            integerMode=2;
            largeValue=mnum.ToBigInteger();
            valValue = BigInteger.valueOf(val);
            largeValue=largeValue.add(valValue);
          }
          break;
        case 2:
          valValue = BigInteger.valueOf(val);
          largeValue=largeValue.add(valValue);
          break;
        default:
          throw new IllegalStateException();
      }
      return this;
    }

    /**
     * 
     */
    public boolean CanFitInInt32() {
      int sign;
      switch(this.integerMode){
        case 0:
          return true;
        case 1:
          return mnum.CanFitInInt32();
          case 2:{
            sign = largeValue.signum();
            if (sign == 0) return true;
            if (sign < 0) return largeValue.compareTo(Int32MinValue) >= 0;
            return largeValue.compareTo(Int32MaxValue) <= 0;
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
      switch(this.integerMode){
        case 0:
          return Integer.toString((int)smallValue);
        case 1:
          return mnum.ToBigInteger().toString();
        case 2:
          return largeValue.toString();
        default:
          throw new IllegalStateException();
      }
    }
    /**
     * 
     */
    public int signum() {
        switch(this.integerMode){
          case 0:
            if(this.smallValue==0)return 0;
            return (this.smallValue<0) ? -1 : 1;
          case 1:
            return mnum.signum();
          case 2:
            return largeValue.signum();
          default:
            throw new IllegalStateException();
        }
      }

    /**
     * Compares a Int32 object with this instance.
     * @param val A 64-bit signed integer.
     * @return Zero if the values are equal; a negative number is this instance
     * is less, or a positive number if this instance is greater.
     */
    public int compareTo(int val) {
      switch(this.integerMode){
        case 0:
          return (val == smallValue) ? 0 : (smallValue < val ? -1 : 1);
        case 1:
          return mnum.ToBigInteger().compareTo(BigInteger.valueOf(val));
        case 2:
          return largeValue.compareTo(BigInteger.valueOf(val));
        default:
          throw new IllegalStateException();
      }
    }

    /**
     * 
     */
    public BigInteger AsBigInteger() {
      switch(this.integerMode){
        case 0:
          return BigInteger.valueOf(smallValue);
        case 1:
          return mnum.ToBigInteger();
        case 2:
          return largeValue;
        default:
          throw new IllegalStateException();
      }
    }
  }
