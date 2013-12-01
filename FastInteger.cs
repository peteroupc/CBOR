/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;
using System.Numerics;
namespace PeterO {
    /// <summary> A mutable integer class initially backed by a 64-bit integer,
    /// that only uses a big integer when arithmetic operations would overflow
    /// the 64-bit integer. <para> This class is ideal for cases where operations
    /// should be arbitrary precision, but the need to use a precision greater
    /// than 64 bits is very rare.</para>
    /// <para> Many methods in this class return a reference to the same object
    /// as used in the call. This allows chaining operations in a single line
    /// of code. For example:</para>
    /// <code> fastInt.Add(5).Multiply(10);</code>
    /// </summary>
  sealed class FastInteger : IComparable<FastInteger> {
    long smallValue;
    BigInteger largeValue;
    bool usingLarge;

    private static BigInteger Int64MinValue = (BigInteger)Int64.MinValue;
    private static BigInteger Int64MaxValue = (BigInteger)Int64.MaxValue;
    private static BigInteger Int32MinValue = (BigInteger)Int32.MinValue;
    private static BigInteger Int32MaxValue = (BigInteger)Int32.MaxValue;

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
      int sign = bigintVal.Sign;
      if (sign == 0 ||
         (sign < 0 && bigintVal.CompareTo(Int64MinValue) >= 0) ||
         (sign > 0 && bigintVal.CompareTo(Int64MaxValue) <= 0)) {
        smallValue = (long)bigintVal;
        usingLarge = false;
      } else {
        usingLarge = true;
        largeValue = bigintVal;
      }
    }

    /// <summary> </summary>
    /// <returns></returns>
    /// <remarks/>
    public int AsInt32() {
      if (usingLarge) {
        return (int)largeValue;
      } else {
        return unchecked((int)smallValue);
      }
    }
    /// <summary> </summary>
    /// <returns></returns>
    /// <remarks/>
    public long AsInt64() {
      if (usingLarge) {
        return (long)largeValue;
      } else {
        return unchecked((long)smallValue);
      }
    }
    /// <summary> Compares a FastInteger object with this instance.</summary>
    /// <param name='val'> A FastInteger object.</param>
    /// <returns> Zero if the values are equal; a negative number is this instance
    /// is less, or a positive number if this instance is greater.</returns>
    /// <remarks/>
    public int CompareTo(FastInteger val) {
      if (usingLarge || val.usingLarge) {
        BigInteger valValue = val.AsBigInteger();
        return largeValue.CompareTo(valValue);
      } else {
        return (val.smallValue == smallValue) ? 0 :
          (smallValue < val.smallValue ? -1 : 1);
      }
    }
    /// <summary> </summary>
    /// <returns></returns>
    /// <remarks/>
    public FastInteger Abs() {
      return (this.Sign < 0) ? Negate() : this;
    }
    /// <summary> Sets this object's value to the remainder of the current
    /// value divided by the given integer. </summary>
    /// <param name='divisor'> The divisor.</param>
    /// <returns> This object.</returns>
    public FastInteger Mod(int divisor) {
      if (usingLarge) {
        // Mod operator will always result in a
        // number that fits an int for int divisors
        largeValue = largeValue % (BigInteger)divisor;
        smallValue = (int)largeValue;
        usingLarge = false;
      } else {
        smallValue %= divisor;
      }
      return this;
    }

    /// <summary> Compares a Int64 object with this instance.</summary>
    /// <param name='val'> A 64-bit signed integer.</param>
    /// <returns> Zero if the values are equal; a negative number is this instance
    /// is less, or a positive number if this instance is greater.</returns>
    /// <remarks/>
    public int CompareTo(long val) {
      if (usingLarge) {
        return largeValue.CompareTo((BigInteger)val);
      } else {
        return (val == smallValue) ? 0 : (smallValue < val ? -1 : 1);
      }
    }

    /// <summary> Sets this object's value to the current value times another
    /// integer. </summary>
    /// <param name='val'> The integer to multiply by.</param>
    /// <returns> This object.</returns>
    public FastInteger Multiply(int val) {
      if (val == 0) {
        smallValue = 0;
        usingLarge = false;
      } else if (usingLarge) {
        largeValue *= (BigInteger)val;
      } else {
        bool apos = (smallValue > 0L);
        bool bpos = (val > 0L);
        if (
          (apos && ((!bpos && (Int64.MinValue / smallValue) > val) ||
                    (bpos && smallValue > (Int64.MaxValue / val)))) ||
          (!apos && ((!bpos && smallValue != 0L &&
                      (Int64.MaxValue / smallValue) > val) ||
                     (bpos && smallValue < (Int64.MinValue / val))))) {
          // would overflow, convert to large
          largeValue = (BigInteger)smallValue;
          usingLarge = true;
          largeValue *= (BigInteger)val;
        } else {
          smallValue *= val;
        }
      }
      return this;
    }

    /// <summary> Sets this object's value to 0 minus its current value (reverses
    /// its sign). </summary>
    /// <returns> This object.</returns>
    public FastInteger Negate() {
      if (usingLarge) {
        largeValue = -(BigInteger)largeValue;
      } else {
        if (smallValue == Int64.MinValue) {
          // would overflow, convert to large
          largeValue = (BigInteger)smallValue;
          usingLarge = true;
          largeValue = -(BigInteger)largeValue;
        } else {
          smallValue = -smallValue;
        }
      }
      return this;
    }

    /// <summary> Sets this object's value to the current value minus the
    /// given FastInteger value. </summary>
    /// <param name='val'> The subtrahend.</param>
    /// <returns> This object.</returns>
    public FastInteger Subtract(FastInteger val) {
      if (usingLarge || val.usingLarge) {
        BigInteger valValue = val.AsBigInteger();
        largeValue -= (BigInteger)valValue;
      } else if (((long)val.smallValue < 0 && Int64.MaxValue + (long)val.smallValue < smallValue) ||
                ((long)val.smallValue > 0 && Int64.MinValue + (long)val.smallValue > smallValue)) {
        // would overflow, convert to large
        largeValue = (BigInteger)smallValue;
        usingLarge = true;
        largeValue -= (BigInteger)val.smallValue;
      } else {
        smallValue -= val.smallValue;
      }
      return this;
    }
    /// <summary> Sets this object's value to the current value minus the
    /// given integer value. </summary>
    /// <param name='val'> The subtrahend.</param>
    /// <returns> This object.</returns>
    public FastInteger Subtract(long val) {
      if (usingLarge) {
        BigInteger valValue = (BigInteger)val;
        largeValue -= (BigInteger)valValue;
      } else if (((long)val < 0 && Int64.MaxValue + (long)val < smallValue) ||
                ((long)val > 0 && Int64.MinValue + (long)val > smallValue)) {
        // would overflow, convert to large
        largeValue = (BigInteger)smallValue;
        usingLarge = true;
        largeValue -= (BigInteger)val;
      } else {
        smallValue -= val;
      }
      return this;
    }

    /// <summary> Sets this object's value to the current value minus the
    /// given integer. </summary>
    /// <param name='val'> The subtrahend.</param>
    /// <returns> This object.</returns>
    public FastInteger Subtract(int val) {
      if (usingLarge) {
        largeValue -= (BigInteger)val;
      } else if (((long)val < 0 && Int64.MaxValue + (long)val < smallValue) ||
                ((long)val > 0 && Int64.MinValue + (long)val > smallValue)) {
        // would overflow, convert to large
        largeValue = (BigInteger)smallValue;
        usingLarge = true;
        largeValue -= (BigInteger)val;
      } else {
        smallValue -= val;
      }
      return this;
    }

    /// <summary> Sets this object's value to the current value minus the
    /// given integer. </summary>
    /// <param name='bigintVal'> The subtrahend.</param>
    /// <returns> This object.</returns>
    public FastInteger Subtract(BigInteger bigintVal) {
      if (usingLarge) {
        largeValue -= (BigInteger)bigintVal;
      } else {
        int sign = bigintVal.Sign;
        if (sign == 0 ||
           (sign < 0 && bigintVal.CompareTo(Int64MinValue) >= 0) ||
           (sign > 0 && bigintVal.CompareTo(Int64MaxValue) <= 0)) {
          long val = (long)bigintVal;
          if ((val < 0 && Int64.MaxValue + val < smallValue) ||
             (val > 0 && Int64.MinValue + val > smallValue)) {
            // would overflow, convert to large
            largeValue = (BigInteger)smallValue;
            usingLarge = true;
            largeValue -= (BigInteger)val;
          } else {
            smallValue -= val;
          }
        } else {
          // convert to large
          largeValue = (BigInteger)smallValue;
          usingLarge = true;
          largeValue -= (BigInteger)bigintVal;
        }
      }
      return this;
    }
    /// <summary> </summary>
    /// <param name='val'> A FastInteger object.</param>
    /// <returns></returns>
    /// <remarks/>
    public FastInteger Add(FastInteger val) {
      if (usingLarge || val.usingLarge) {
        BigInteger valValue = val.AsBigInteger();
        largeValue += (BigInteger)valValue;
      } else if ((smallValue < 0 && (long)val.smallValue < Int64.MinValue - smallValue) ||
                (smallValue > 0 && (long)val.smallValue > Int64.MaxValue - smallValue)) {
        // would overflow, convert to large
        largeValue = (BigInteger)smallValue;
        usingLarge = true;
        largeValue += (BigInteger)val.smallValue;
      } else {
        smallValue += val.smallValue;
      }
      return this;
    }

    /// <summary> Divides this instance by the value of a Int32 object.</summary>
    /// <param name='divisor'> A 32-bit signed integer.</param>
    /// <returns> The quotient of the two objects.</returns>
    /// <remarks/>
    public FastInteger Divide(int divisor) {
      if (divisor != 0) {
        if (usingLarge) {
          largeValue /= (BigInteger)divisor;
        } else if (divisor == -1 && smallValue == Int64.MinValue) {
          // would overflow, convert to large
          largeValue = (BigInteger)smallValue;
          usingLarge = true;
          largeValue /= (BigInteger)divisor;
        } else {
          smallValue /= divisor;
        }
      }
      return this;
    }

    /// <summary> </summary>
    /// <param name='val'> A 32-bit signed integer.</param>
    /// <returns></returns>
    /// <remarks/>
    public FastInteger Add(int val) {
      if (val != 0) {
        if (usingLarge) {
          largeValue += (BigInteger)val;
        } else if ((smallValue < 0 && (long)val < Int64.MinValue - smallValue) ||
                  (smallValue > 0 && (long)val > Int64.MaxValue - smallValue)) {
          // would overflow, convert to large
          largeValue = (BigInteger)smallValue;
          usingLarge = true;
          largeValue += (BigInteger)val;
        } else {
          smallValue += val;
        }
      }
      return this;
    }
    /// <summary> </summary>
    /// <param name='val'> A 64-bit signed integer.</param>
    /// <returns></returns>
    /// <remarks/>
    public FastInteger Add(long val) {
      if (val != 0) {
        if (usingLarge) {
          largeValue += (BigInteger)val;
        } else if ((smallValue < 0 && val < Int64.MinValue - smallValue) ||
                  (smallValue > 0 && val > Int64.MaxValue - smallValue)) {
          // would overflow, convert to large
          largeValue = (BigInteger)smallValue;
          usingLarge = true;
          largeValue += (BigInteger)val;
        } else {
          smallValue += val;
        }
      }
      return this;
    }

    /// <summary> Gets whether this object's value is within the range of
    /// a 64-bit signed integer. </summary>
    /// <returns></returns>
    public bool CanFitInInt64() {
      if (!usingLarge) {
        return true;
      } else {
        int sign = largeValue.Sign;
        if (sign == 0) return true;
        if (sign < 0) return largeValue.CompareTo(Int64MinValue) >= 0;
        return largeValue.CompareTo(Int64MaxValue) <= 0;
      }
    }

    /// <summary> Gets whether this object's value is within the range of
    /// a 32-bit signed integer. </summary>
    /// <returns></returns>
    public bool CanFitInInt32() {
      if (usingLarge) {
        int sign = largeValue.Sign;
        if (sign == 0) return true;
        if (sign < 0) return largeValue.CompareTo(Int32MinValue) >= 0;
        return largeValue.CompareTo(Int32MaxValue) <= 0;
      } else {
        return (smallValue >= Int32.MinValue &&
                smallValue <= Int32.MaxValue);
      }
    }

    /// <summary> Converts this object to a text string.</summary>
    /// <returns> A string representation of this object.</returns>
    /// <remarks/>
    public override string ToString() {
      return usingLarge ? largeValue.ToString() : Convert.ToString((long)smallValue, System.Globalization.CultureInfo.InvariantCulture);
    }
    /// <summary> </summary>
    /// <remarks/>
    public int Sign {
      get {
        return usingLarge ? largeValue.Sign : (
          (smallValue == 0) ? 0 : (smallValue < 0 ? -1 : 1));
      }
    }

    /// <summary> </summary>
    /// <returns></returns>
    /// <remarks/>
    public BigInteger AsBigInteger() {
      return usingLarge ? largeValue : (BigInteger)smallValue;
    }
  }
}