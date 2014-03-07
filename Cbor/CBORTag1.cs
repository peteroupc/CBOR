/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/7/2014
 * Time: 9:44 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO.Cbor
{
  /// <summary>
  /// Description of CBORTag1.
  /// </summary>
  public class CBORTag1 : ICBORTag
  {
    
    public CBORTypeFilter GetTypeFilter()
    {
      return CBORTypeFilter.UnsignedInteger.WithNegativeInteger().WithFloatingPoint();
    }
    
    public CBORObject ValidateObject(CBORObject obj)
    {
      if(!obj.IsFinite)
        throw new CBORException("Not a valid date");
      return obj;
    }
  }
}
