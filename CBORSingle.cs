/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/21/2014
 * Time: 9:26 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO
{
  internal class CBORSingle : ICBORNumber
  {
    
    public bool IsPositiveInfinity(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public bool IsInfinity(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public bool IsNegativeInfinity(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public bool IsNaN(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public double AsDouble(object obj)
    {
      return (double)(float)obj;
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
      float fltItem = (float)obj;
      if (Single.IsNaN(fltItem)) {
        throw new OverflowException("This object's value is out of range");
      }
      fltItem = (fltItem < 0) ? (float)Math.Ceiling(fltItem) : (float)Math.Floor(fltItem);
      if (fltItem >= Int64.MinValue && fltItem <= Int64.MaxValue) {
        return (long)fltItem;
      }
      throw new OverflowException("This object's value is out of range");
    }
    
    public bool CanFitInSingle(object obj)
    {
      return true;
    }
    
    public bool CanFitInDouble(object obj)
    {
      return true;
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
    
    public int AsInt32(object obj, int minValue, int maxValue)
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
      throw new NotImplementedException();  // TODO: Implement
    }
    
  public bool CanFitInTypeZeroOrOne(object obj)
  {
    throw new NotImplementedException();
  }
  }
}
