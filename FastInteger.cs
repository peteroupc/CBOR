/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;
//using System.Numerics;
namespace PeterO {
    /// <summary> A mutable integer class initially backed by a small integer,
    /// that only uses a big integer when arithmetic operations would overflow
    /// the small integer. <para> This class is ideal for cases where operations
    /// should be arbitrary precision, but the need to use a high precision
    /// is rare.</para>
    /// <para> Many methods in this class return a reference to the same object
    /// as used in the call. This allows chaining operations in a single line
    /// of code. For example:</para>
    /// <code> fastInt.Add(5).Multiply(10);</code>
    /// </summary>
  sealed class FastInteger : IComparable<FastInteger> {
    int smallValue; // if integerMode is 0
    MutableNumber mnum; // if integerMode is 1
    BigInteger largeValue; // if integerMode is 2
    int integerMode;

    private static BigInteger Int32MinValue = (BigInteger)Int32.MinValue;
    private static BigInteger Int32MaxValue = (BigInteger)Int32.MaxValue;
    private static BigInteger NegativeInt32MinValue = -(BigInteger)Int32MinValue;

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
      int sign = bigintVal.Sign;
      if (sign == 0 ||
          (sign < 0 && bigintVal.CompareTo(Int32MinValue) >= 0) ||
          (sign > 0 && bigintVal.CompareTo(Int32MaxValue) <= 0)) {
        integerMode=0;
        smallValue = (int)bigintVal;
      } else if(sign>0){
        integerMode=1;
        mnum=new MutableNumber(bigintVal);
      } else {
        integerMode=2;
        largeValue = bigintVal;
      }
    }

    /// <summary> </summary>
    /// <returns></returns>
    public int AsInt32() {
      switch(this.integerMode){
        case 0:
          return smallValue;
          case 1:{
            BigInteger bigint=mnum.ToBigInteger();
            return (int)bigint;
          }
        case 2:
          return (int)largeValue;
        default:
          throw new InvalidOperationException();
      }
    }
    
    /// <summary>Compares a FastInteger object with this instance.</summary>
    /// <param name='val'>A FastInteger object.</param>
    /// <returns>Zero if the values are equal; a negative number is this instance
    /// is less, or a positive number if this instance is greater.</returns>
    public int CompareTo(FastInteger val) {
      switch(this.integerMode){
        case 0:
          if(val.integerMode==0){
            return (val.smallValue == smallValue) ? 0 :
              (smallValue < val.smallValue ? -1 : 1);
          } else {
            return AsBigInteger().CompareTo(val.AsBigInteger());
          }
        case 1:
          return AsBigInteger().CompareTo(val.AsBigInteger());
        case 2:
          return AsBigInteger().CompareTo(val.AsBigInteger());
        default:
          throw new InvalidOperationException();
      }
    }
    /// <summary> </summary>
    /// <returns></returns>
    public FastInteger Abs() {
      return (this.Sign < 0) ? Negate() : this;
    }

    /// <summary> Sets this object's value to the current value times another
    /// integer. </summary>
    /// <param name='val'> The integer to multiply by.</param>
    /// <returns> This object.</returns>
    public FastInteger Multiply(int val) {
      if (val == 0) {
        smallValue = 0;
        integerMode=0;
      } else {
        switch (integerMode) {
          case 0:
            bool apos = (smallValue > 0L);
            bool bpos = (val > 0L);
            if (
              (apos && ((!bpos && (Int32.MinValue / smallValue) > val) ||
                        (bpos && smallValue > (Int32.MaxValue / val)))) ||
              (!apos && ((!bpos && smallValue != 0L &&
                          (Int32.MaxValue / smallValue) > val) ||
                         (bpos && smallValue < (Int32.MinValue / val))))) {
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
                largeValue = (BigInteger)smallValue;
                largeValue *= (BigInteger)val;
              }
            } else {
              smallValue *= val;
            }
            break;
          case 1:
            if(val<0){
              integerMode=2;
              largeValue=mnum.ToBigInteger();
              largeValue*=(BigInteger)val;
            } else {
              mnum.Multiply(val);
            }
            break;
          case 2:
            largeValue*=(BigInteger)val;
            break;
          default:
            throw new InvalidOperationException();
        }
      }
      return this;
    }

    /// <summary> Sets this object's value to 0 minus its current value (reverses
    /// its sign). </summary>
    /// <returns> This object.</returns>
    public FastInteger Negate() {
      switch (integerMode) {
        case 0:
          if (smallValue == Int32.MinValue) {
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
          largeValue=-(BigInteger)largeValue;
          break;
        case 2:
          largeValue=-(BigInteger)largeValue;
          break;
        default:
          throw new InvalidOperationException();
      }
      return this;
    }

    /// <summary> Sets this object's value to the current value minus the
    /// given FastInteger value. </summary>
    /// <param name='val'> The subtrahend.</param>
    /// <returns> This object.</returns>
    public FastInteger Subtract(FastInteger val) {
      BigInteger valValue;
      switch (integerMode) {
        case 0:
          if(val.integerMode==0){
            if (((int)val.smallValue < 0 && Int32.MaxValue + (int)val.smallValue < smallValue) ||
                ((int)val.smallValue > 0 && Int32.MinValue + (int)val.smallValue > smallValue)) {
              // would overflow, convert to large
              integerMode=2;
              largeValue = (BigInteger)smallValue;
              largeValue -= (BigInteger)val.smallValue;
            } else {
              smallValue-=val.smallValue;
            }
          } else {
            integerMode=2;
            largeValue=(BigInteger)smallValue;
            valValue = val.AsBigInteger();
            largeValue -= (BigInteger)valValue;
          }
          break;
        case 1:
          integerMode=2;
          largeValue=mnum.ToBigInteger();
          valValue = val.AsBigInteger();
          largeValue -= (BigInteger)valValue;
          break;
        case 2:
          valValue = val.AsBigInteger();
          largeValue -= (BigInteger)valValue;
          break;
        default:
          throw new InvalidOperationException();
      }
      return this;
    }
    /// <summary> Sets this object's value to the current value minus the
    /// given integer. </summary>
    /// <param name='val'> The subtrahend.</param>
    /// <returns> This object.</returns>
    public FastInteger Subtract(int val) {
      if(val==Int32.MinValue){
        return Add(NegativeInt32MinValue);
      } else {
        return Add(-val);
      }
    }
    
    

    /// <summary> Sets this object's value to the current value plus the given
    /// integer. </summary>
    /// <param name='bigintVal'> The number to add.</param>
    /// <returns> This object.</returns>
    public FastInteger Add(BigInteger bigintVal) {
      switch (integerMode) {
          case 0:{
            int sign = bigintVal.Sign;
            if (sign == 0 ||
                (sign < 0 && bigintVal.CompareTo(Int32MinValue) >= 0) ||
                (sign > 0 && bigintVal.CompareTo(Int32MaxValue) <= 0)) {
              return Add((int)bigintVal);
            }
            return Add(new FastInteger(bigintVal));
          }
        case 1:
          integerMode=2;
          largeValue=mnum.ToBigInteger();
          largeValue += bigintVal;
          break;
        case 2:
          largeValue += bigintVal;
          break;
        default:
          throw new InvalidOperationException();
      }
      return this;
    }
    

    /// <summary> Sets this object's value to the current value minus the
    /// given integer. </summary>
    /// <param name='bigintVal'> The subtrahend.</param>
    /// <returns> This object.</returns>
    public FastInteger Subtract(BigInteger bigintVal) {
      if (integerMode==2) {
        largeValue -= (BigInteger)bigintVal;
        return this;
      } else {
        int sign = bigintVal.Sign;
        if (sign == 0)return this;
        // Check if this value fits an int, except if
        // it's MinValue
        if(sign < 0 && bigintVal.CompareTo(Int32MinValue) > 0){
          return Add(-((int)bigintVal));
        }
        if(sign > 0 && bigintVal.CompareTo(Int32MaxValue) <= 0){
          return Subtract((int)bigintVal);
        }
        bigintVal=-bigintVal;
        return Add(bigintVal);
      }
    }
    /// <summary> </summary>
    /// <param name='val'> A FastInteger object.</param>
    /// <returns></returns>
    public FastInteger Add(FastInteger val) {
      BigInteger valValue;
      switch (integerMode) {
        case 0:
          if(val.integerMode==0){
            if ((smallValue < 0 && (int)val.smallValue < Int32.MinValue - smallValue) ||
                (smallValue > 0 && (int)val.smallValue > Int32.MaxValue - smallValue)) {
              // would overflow
              if(val.smallValue>=0){
                integerMode=1;
                mnum=new MutableNumber(this.smallValue);
                mnum.Add(val.smallValue);
              } else {
                integerMode=2;
                largeValue = (BigInteger)smallValue;
                largeValue += (BigInteger)val.smallValue;
              }
            } else {
              smallValue+=val.smallValue;
            }
          } else {
            integerMode=2;
            largeValue=(BigInteger)smallValue;
            valValue = val.AsBigInteger();
            largeValue += (BigInteger)valValue;
          }
          break;
        case 1:
          if(val.integerMode==0 && val.smallValue>=0){
            mnum.Add(val.smallValue);
          } else {
            integerMode=2;
            largeValue=mnum.ToBigInteger();
            valValue = val.AsBigInteger();
            largeValue += (BigInteger)valValue;
          }
          break;
        case 2:
          valValue = val.AsBigInteger();
          largeValue += (BigInteger)valValue;
          break;
        default:
          throw new InvalidOperationException();
      }
      return this;
    }
    /// <summary> Sets this object's value to the remainder of the current
    /// value divided by the given integer. </summary>
    /// <param name='divisor'> The divisor.</param>
    /// <returns> This object.</returns>
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
            largeValue = largeValue % (BigInteger)divisor;
            smallValue = (int)largeValue;
            integerMode=0;
            break;
          case 2:
            largeValue = largeValue % (BigInteger)divisor;
            smallValue = (int)largeValue;
            integerMode=0;
            break;
          default:
            throw new InvalidOperationException();
        }
      } else {
        throw new DivideByZeroException();
      }
      return this;
    }


    /// <summary> Divides this instance by the value of a Int32 object.</summary>
    /// <param name='divisor'> A 32-bit signed integer.</param>
    /// <returns> The quotient of the two objects.</returns>
    public FastInteger Divide(int divisor) {
      if (divisor != 0) {
        switch (integerMode) {
          case 0:
            if (divisor == -1 && smallValue == Int32.MinValue) {
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
            largeValue/=(BigInteger)divisor;
            if(largeValue.IsZero){
              integerMode=0;
              smallValue=0;
            }
            break;
          case 2:
            largeValue/=(BigInteger)divisor;
            if(largeValue.IsZero){
              integerMode=0;
              smallValue=0;
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
    
    /// <summary> </summary>
    public bool IsEvenNumber{
      get {
        switch (integerMode) {
          case 0:
            return (smallValue&1)==0;
          case 1:
            return mnum.IsEvenNumber;
          case 2:
            return largeValue.IsEven;
          default:
            throw new InvalidOperationException();
        }
      }
    }
    /// <summary> </summary>
    /// <param name='val'> A 64-bit signed integer.</param>
    /// <returns></returns>
    public FastInteger Add(int val) {
      BigInteger valValue;
      switch (integerMode) {
        case 0:
          if ((smallValue < 0 && (int)val < Int32.MinValue - smallValue) ||
              (smallValue > 0 && (int)val > Int32.MaxValue - smallValue)) {
            // would overflow
            if(val>=0){
              integerMode=1;
              mnum=new MutableNumber(this.smallValue);
              mnum.Add(val);
            } else {
              integerMode=2;
              largeValue = (BigInteger)smallValue;
              largeValue += (BigInteger)val;
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
            valValue = (BigInteger)val;
            largeValue += (BigInteger)valValue;
          }
          break;
        case 2:
          valValue = (BigInteger)val;
          largeValue += (BigInteger)valValue;
          break;
        default:
          throw new InvalidOperationException();
      }
      return this;
    }

    /// <summary> </summary>
    /// <returns></returns>
    public bool CanFitInInt32() {
      int sign;
      switch(this.integerMode){
        case 0:
          return true;
        case 1:
          return mnum.CanFitInInt32();
          case 2:{
            sign = largeValue.Sign;
            if (sign == 0) return true;
            if (sign < 0) return largeValue.CompareTo(Int32MinValue) >= 0;
            return largeValue.CompareTo(Int32MaxValue) <= 0;
          }
        default:
          throw new InvalidOperationException();
      }
    }

    /// <summary> Converts this object to a text string.</summary>
    /// <returns> A string representation of this object.</returns>
    public override string ToString() {
      switch(this.integerMode){
        case 0:
          return Convert.ToString((int)smallValue, System.Globalization.CultureInfo.InvariantCulture);
        case 1:
          return mnum.ToBigInteger().ToString();
        case 2:
          return largeValue.ToString();
        default:
          throw new InvalidOperationException();
      }
    }
    /// <summary> </summary>
    public int Sign {
      get {
        switch(this.integerMode){
          case 0:
            if(this.smallValue==0)return 0;
            return (this.smallValue<0) ? -1 : 1;
          case 1:
            return mnum.Sign;
          case 2:
            return largeValue.Sign;
          default:
            throw new InvalidOperationException();
        }
      }
    }

    /// <summary> Compares a Int32 object with this instance.</summary>
    /// <param name='val'> A 64-bit signed integer.</param>
    /// <returns> Zero if the values are equal; a negative number is this instance
    /// is less, or a positive number if this instance is greater.</returns>
    public int CompareTo(int val) {
      switch(this.integerMode){
        case 0:
          return (val == smallValue) ? 0 : (smallValue < val ? -1 : 1);
        case 1:
          return mnum.ToBigInteger().CompareTo((BigInteger)val);
        case 2:
          return largeValue.CompareTo((BigInteger)val);
        default:
          throw new InvalidOperationException();
      }
    }

    /// <summary> </summary>
    /// <returns></returns>
    public BigInteger AsBigInteger() {
      switch(this.integerMode){
        case 0:
          return (BigInteger)smallValue;
        case 1:
          return mnum.ToBigInteger();
        case 2:
          return largeValue;
        default:
          throw new InvalidOperationException();
      }
    }
  }
}