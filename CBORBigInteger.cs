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
  internal class CBORBigInteger : ICBORNumber
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
      throw new NotImplementedException();  // TODO: Implement
    }
    
    private static BigInteger valueLowestMajorType1 = BigInteger.Zero - (BigInteger.One << 64);

    private static BigInteger valueUInt64MaxValue = (BigInteger.One << 64) - BigInteger.One;

    public bool CanFitInTypeZeroOrOne(object obj)
    {
      BigInteger bigint = (BigInteger)obj;
      return bigint.CompareTo(valueLowestMajorType1) >= 0 &&
        bigint.CompareTo(valueUInt64MaxValue) <= 0;
    }
    
    public int AsInt32(object obj, int minValue, int maxValue)
    {
      throw new NotImplementedException();  // TODO: Implement
    }
  }
}
