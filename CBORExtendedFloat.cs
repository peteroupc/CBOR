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
  internal class CBORExtendedFloat : ICBORNumber
  {
    
    public bool IsPositiveInfinity(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj;
      return ef.IsPositiveInfinity();
    }
    
    public bool IsInfinity(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj;
      return ef.IsInfinity();
    }
    
    public bool IsNegativeInfinity(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj;
      return ef.IsNegativeInfinity();
    }
    
    public bool IsNaN(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj;
      return ef.IsNaN();
    }
    
    public double AsDouble(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj;
      return ef.ToDouble();
    }
    
    public ExtendedDecimal AsExtendedDecimal(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj;
      return ef.ToExtendedDecimal();
    }
    
    public ExtendedFloat AsExtendedFloat(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj;
      return ef;
    }
    
    public float AsSingle(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj;
      return ef.ToSingle();
    }
    
    public BigInteger AsBigInteger(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj;
      return ef.ToBigInteger();
    }
    
    public long AsInt64(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj;
      if (ef.IsFinite) {
        BigInteger bi = ef.ToBigInteger();
        if (bi.CompareTo(CBORObject.Int64MaxValue) <= 0 &&
            bi.CompareTo(CBORObject.Int64MinValue) >= 0) {
          return (long)bi;
        }
      }
      throw new OverflowException("This object's value is out of range");
    }
    
    public bool CanFitInSingle(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj; throw new NotImplementedException();
    }
    
    public bool CanFitInDouble(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj; throw new NotImplementedException();
    }
    
    public bool CanFitInInt32(object obj)
    {
      return IsIntegral(obj) && CanTruncatedIntFitInInt32(obj);
    }
    
    public bool CanFitInInt64(object obj)
    {
      return IsIntegral(obj) && CanTruncatedIntFitInInt64(obj);
    }
    
    public bool CanTruncatedIntFitInInt64(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj; throw new NotImplementedException();
    }
    
    public bool CanTruncatedIntFitInInt32(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj; throw new NotImplementedException();
    }
    
    public int AsInt32(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj; throw new NotImplementedException();
    }
    
    public bool IsZero(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj; throw new NotImplementedException();
    }
    
    public int Sign(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj; throw new NotImplementedException();
    }
    
    public bool IsIntegral(object obj)
    {
      ExtendedFloat ef=(ExtendedFloat)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.Exponent.Sign >= 0) {
        return true;
      }
      ExtendedFloat ef2 = ExtendedFloat.FromBigInteger(ef.ToBigInteger());
      return ef2.CompareTo(ef) == 0;
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
