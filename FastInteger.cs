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

namespace PeterO
{
  /// <summary>
  /// A mutable integer class initially backed by a 64-bit integer,
  /// that only uses a big integer when arithmetic operations would
  /// overflow the 64-bit integer.
  /// </summary>
  sealed class FastInteger {
    long smallValue;
    BigInteger largeValue;
    bool usingLarge;
    
    public FastInteger(){
      smallValue=0;
      usingLarge=false;
    }
    
    public FastInteger(long value){
      smallValue=value;
      usingLarge=false;
    }
    
    public FastInteger(FastInteger value){
      smallValue=value.smallValue;
      usingLarge=value.usingLarge;
      largeValue=value.largeValue;
    }
    
    // This constructor converts a big integer to a 64-bit one
    // if it's small enough
    public FastInteger(BigInteger bigintVal){
      if(bigintVal.CompareTo((BigInteger)Int64.MinValue)>=0 &&
         bigintVal.CompareTo((BigInteger)Int64.MaxValue)<=0){
        smallValue=(long)bigintVal;
        usingLarge=false;
      } else {
        smallValue=0;
        usingLarge=true;
        largeValue=bigintVal;
      }
    }
    
    public int AsInt32(){
      if(usingLarge){
        return (int)largeValue;
      } else {
        return unchecked((int)smallValue);
      }
    }
    public long AsInt64(){
      if(usingLarge){
        return (long)largeValue;
      } else {
        return unchecked((long)smallValue);
      }
    }
    public int CompareTo(FastInteger val){
      if(usingLarge || val.usingLarge){
        BigInteger valValue=val.largeValue;
        return largeValue.CompareTo(valValue);
      } else {
        return (val.smallValue==smallValue) ? 0 :
          (smallValue<val.smallValue ? -1 : 1);
      }
    }
    public FastInteger Abs(){
      return (this.Sign<0) ? Negate() : this;
    }
    public FastInteger Mod(int divisor){
      if(usingLarge){
        // Mod operator will always result in a
        // number that fits an int for int divisors
        largeValue=largeValue%(BigInteger)divisor;
        smallValue=(int)largeValue;
        usingLarge=false;
      } else {
        smallValue%=divisor;
      }
      return this;
    }
    
    public int CompareTo(long val){
      if(usingLarge){
        return largeValue.CompareTo((BigInteger)val);
      } else {
        return (val==smallValue) ? 0 : (smallValue<val ? -1 : 1);
      }
    }
    
    public FastInteger Multiply(int val){
      if(val==0){
        smallValue=0;
        usingLarge=false;
      } else if(usingLarge){
        largeValue*=(BigInteger)val;
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
          largeValue=(BigInteger)smallValue;
          usingLarge=true;
          largeValue*=(BigInteger)val;
        } else {
          smallValue*=val;
        }
      }
      return this;
    }
    
    public FastInteger Negate(){
      if(usingLarge){
        largeValue=-(BigInteger)largeValue;
      } else {
        if(smallValue==Int64.MinValue){
          // would overflow, convert to large
          largeValue=(BigInteger)smallValue;
          usingLarge=true;
          largeValue=-(BigInteger)largeValue;
        } else {
          smallValue=-smallValue;
        }
      }
      return this;
    }
    
    public FastInteger Subtract(FastInteger val){
      if(usingLarge || val.usingLarge){
        BigInteger valValue=val.largeValue;
        largeValue-=(BigInteger)valValue;
      } else if(((long)val.smallValue < 0 && Int64.MaxValue + (long)val.smallValue < smallValue) ||
                ((long)val.smallValue > 0 && Int64.MinValue + (long)val.smallValue > smallValue)){
        // would overflow, convert to large
        largeValue=(BigInteger)smallValue;
        usingLarge=true;
        largeValue-=(BigInteger)val.smallValue;
      } else{
        smallValue-=val.smallValue;
      }
      return this;
    }
    
    public FastInteger Subtract(int val){
      if(usingLarge){
        largeValue-=(BigInteger)val;
      } else if(((long)val < 0 && Int64.MaxValue + (long)val < smallValue) ||
                ((long)val > 0 && Int64.MinValue + (long)val > smallValue)){
        // would overflow, convert to large
        largeValue=(BigInteger)smallValue;
        usingLarge=true;
        largeValue-=(BigInteger)val;
      } else{
        smallValue-=val;
      }
      return this;
    }
    
    public FastInteger Add(FastInteger val){
      if(usingLarge || val.usingLarge){
        BigInteger valValue=val.largeValue;
        largeValue+=(BigInteger)valValue;
      } else if((smallValue < 0 && (long)val.smallValue < Int64.MinValue - smallValue) ||
                (smallValue > 0 && (long)val.smallValue > Int64.MaxValue - smallValue)){
        // would overflow, convert to large
        largeValue=(BigInteger)smallValue;
        usingLarge=true;
        largeValue+=(BigInteger)val.smallValue;
      } else{
        smallValue+=val.smallValue;
      }
      return this;
    }
    
    public FastInteger Divide(int divisor){
      if(divisor!=0){
        if(usingLarge){
          largeValue/=(BigInteger)divisor;
        } else if(divisor==-1 && smallValue==Int64.MinValue){
          // would overflow, convert to large
          largeValue=(BigInteger)smallValue;
          usingLarge=true;
          largeValue/=(BigInteger)divisor;
        } else{
          smallValue/=divisor;
        }
      }
      return this;
    }
    
    public FastInteger Add(int val){
      if(val!=0){
        if(usingLarge){
          largeValue+=(BigInteger)val;
        } else if((smallValue < 0 && (long)val < Int64.MinValue - smallValue) ||
                  (smallValue > 0 && (long)val > Int64.MaxValue - smallValue)){
          // would overflow, convert to large
          largeValue=(BigInteger)smallValue;
          usingLarge=true;
          largeValue+=(BigInteger)val;
        } else{
          smallValue+=val;
        }
      }
      return this;
    }
    
    public bool CanFitInInt64() {
      if(usingLarge){
        return (largeValue.CompareTo((BigInteger)Int64.MinValue)>=0 &&
                largeValue.CompareTo((BigInteger)Int64.MaxValue)<=0);
      } else {
        return true;
      }
    }
    
    public int Sign {
      get {
        return usingLarge ? largeValue.Sign : (
          (smallValue==0) ? 0 : (smallValue<0 ? -1 : 1));
      }
    }
    
    public BigInteger AsBigInteger() {
      return usingLarge ? largeValue : (BigInteger)smallValue;
    }
  }
}
