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
  internal class CBORDouble : ICBORNumber
  {
    
    public bool IsPositiveInfinity(object obj)
    {
      return Double.IsPositiveInfinity((double)obj);
    }
    
    public bool IsInfinity(object obj)
    {
      return Double.IsInfinity((double)obj);
    }
    
    public bool IsNegativeInfinity(object obj)
    {
      return Double.IsNegativeInfinity((double)obj);
    }
    
    public bool IsNaN(object obj)
    {
      return Double.IsNaN((double)obj);
    }
    
    public double AsDouble(object obj)
    {
      return (double)obj;
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
      return (float)(double)obj;
    }
    
    public BigInteger AsBigInteger(object obj)
    {
      return CBORUtilities.BigIntegerFromDouble((double)obj);
    }
    
    public long AsInt64(object obj)
    {
      double fltItem = (double)obj;
      if (Double.IsNaN(fltItem)) {
        throw new OverflowException("This object's value is out of range");
      }
      fltItem = (fltItem < 0) ? Math.Ceiling(fltItem) : Math.Floor(fltItem);
      if (fltItem >= Int64.MinValue && fltItem <= Int64.MaxValue) {
        return (long)fltItem;
      }
      throw new OverflowException("This object's value is out of range");
    }
    
    public bool CanFitInSingle(object obj)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
    
    public bool CanFitInDouble(object obj)
    {
      return true;
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
      return ((double)obj)==0.0;
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
    throw new NotImplementedException();  // TODO: Implement
  }
  }
}
