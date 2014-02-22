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
  internal class CBORExtendedRational : ICBORNumber
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
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public ExtendedDecimal AsExtendedDecimal(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public ExtendedFloat AsExtendedFloat(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public float AsSingle(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public BigInteger AsBigInteger(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public long AsInt64(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public bool CanFitInSingle(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
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
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public bool CanTruncatedIntFitInInt64(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
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
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public int Sign(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public bool IsIntegral(object obj)
    {
      ExtendedRational ef = (ExtendedRational)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.Denominator.Equals(BigInteger.One)) {
        return true;
      }
      // A rational number is integral if the remainder
      // of the numerator divided by the denominator is 0
      BigInteger denom = ef.Denominator;
      BigInteger rem = ef.Numerator % (BigInteger)denom;
      return rem.IsZero;
    }
    
  public bool CanFitInTypeZeroOrOne(object obj)
  {
    throw new NotImplementedException();
  }
    
  public int AsInt32(object obj, int minValue, int maxValue)
  {
    throw new NotImplementedException();
  }
  }
}
