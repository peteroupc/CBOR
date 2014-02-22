/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

using System;

namespace PeterO
{
  public class CBORInteger : ICBORNumber
  {
    
    
    public bool IsPositiveInfinity(object obj)
    {
      return false;
    }
    
    public bool IsInfinity(object obj)
    {
      return false;
    }
    
    public bool IsNegativeInfinity(object obj)
    {
      return false;
    }
    
    public bool IsNaN(object obj)
    {
      return false;
    }
    
    public double AsDouble(object obj)
    {
      return (double)(long)obj;
    }
    
    public ExtendedDecimal AsExtendedDecimal(object obj)
    {
      return ExtendedDecimal.FromInt64((long)obj);
    }
    
    public ExtendedFloat AsExtendedFloat(object obj)
    {
      return ExtendedFloat.FromInt64((long)obj);
    }
    
    public float AsSingle(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public BigInteger AsBigInteger(object obj)
    {
      return (BigInteger)(long)obj;
    }
    
    public long AsInt64(object obj)
    {
      return (long)obj;
    }
    
    public bool CanFitInSingle(object obj)
    {
      long intItem = (long)obj;
      if (intItem == Int64.MinValue) {
        return true;
      }
      intItem = Math.Abs(intItem);
      while (intItem >= (1L << 24) && (intItem & 1) == 0) {
        intItem >>= 1;
      }
      return intItem < (1L << 24);
    }
    
    public bool CanFitInDouble(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public bool CanFitInInt32(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public bool CanFitInInt64(object obj)
    {
      return true;
    }
    
    public bool CanTruncatedIntFitInInt64(object obj)
    {
      return true;
    }
    
    public bool CanTruncatedIntFitInInt32(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public int AsInt32(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public bool IsZero(object obj)
    {
      return ((long)obj)==0;
    }
    
    public int Sign(object obj)
    {
      long val=(long)obj;
      return (val==0) ? 0 : ((val<0) ? -1 : 1);
    }
    
    public bool IsIntegral(object obj)
    {
      return true;
    }
    
    public bool CanFitInTypeZeroOrOne(object obj)
    {
      return true;
    }
    
    public int AsInt32(object obj, int minValue, int maxValue)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
  }
}
