/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 11/23/2013
 * Time: 6:13 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
//using System.Numerics;
namespace PeterO {
    /// <summary> Description of FastInteger2. </summary>
  class FastInteger2 {
    long smallValue;
    MutableBigInteger largeValue;
    bool usingLarge;

    public FastInteger2() {
    }

    /// <summary> </summary>
    /// <returns></returns>
    /// <remarks/>
    public FastInteger2 MultiplyByTen() {
      if (usingLarge) {
        largeValue.Multiply(10);
      } else {
        bool apos = (smallValue > 0L);
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

    /// <summary>Multiplies this instance by the value of a Int32 object.</summary>
    /// <returns>The product of the two objects.</returns>
    /// <remarks/><param name='val'>A 32-bit signed integer.</param>
    public FastInteger2 Multiply(int val) {
      if (usingLarge) {
        largeValue.Multiply(val);
      } else {
        bool apos = (smallValue > 0L);
        if (
          (apos && ((smallValue > Int64.MaxValue/val))) ||
          (!apos && ((smallValue < Int64.MinValue/val)))) {
          // would overflow, convert to large
          largeValue = new MutableBigInteger(smallValue);
          usingLarge = true;
          largeValue.Multiply(val);
        } else {
          smallValue *= val;
        }
      }
      return this;
    }

    /// <summary> </summary>
    /// <param name='val'> A 32-bit signed integer.</param>
    /// <returns></returns>
    /// <remarks/>
    public FastInteger2 Add(int val) {
      if (val != 0) {
        if (usingLarge) {
          largeValue.Add(val);
        } else if ((smallValue < 0 && (long)val < Int64.MinValue - smallValue) ||
                  (smallValue > 0 && (long)val > Int64.MaxValue - smallValue)) {
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

    /// <summary> </summary>
    /// <param name='a'> A FastInteger object.</param>
    /// <returns></returns>
    /// <remarks/>
    public void AddThisTo(FastInteger a) {
      if (usingLarge) {
        a.Add(new FastInteger(largeValue.ToBigInteger()));
      } else {
        a.Add(smallValue);
      }
    }
    /// <summary> </summary>
    /// <param name='a'> A FastInteger object.</param>
    /// <returns></returns>
    /// <remarks/>
    public void SubtractThisFrom(FastInteger a) {
      if (usingLarge) {
        a.Subtract(new FastInteger(largeValue.ToBigInteger()));
      } else {
        a.Subtract(smallValue);
      }
    }
    
    /// <summary> </summary>
    /// <remarks/>
public int Sign{
      get {
        if(usingLarge){
          return largeValue.Sign;
        } else {
          return (smallValue==0) ? 0 : (smallValue<0 ? -1 : 1);
        }
      }
    }

    /// <summary> Converts this object to a text string.</summary>
    /// <returns> A string representation of this object.</returns>
    /// <remarks/>
    public override string ToString() {
      return usingLarge ? largeValue.ToString() : Convert.ToString((long)smallValue, System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary> </summary>
    /// <returns></returns>
    /// <remarks/>
    public BigInteger AsBigInteger() {
      return usingLarge ? largeValue.ToBigInteger() : (BigInteger)smallValue;
    }
    /// <summary> </summary>
    /// <returns></returns>
    /// <remarks/>
    public BigInteger AsNegatedBigInteger() {
      if (usingLarge) {
        BigInteger bigint = largeValue.ToBigInteger();
        bigint = -(BigInteger)bigint;
        return bigint;
      } else {
        BigInteger bigint = -(BigInteger)smallValue;
        return bigint;
      }
    }
  }
}