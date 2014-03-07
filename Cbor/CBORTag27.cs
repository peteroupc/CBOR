/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/7/2014
 * Time: 10:01 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO.Cbor
{
  /// <summary>
  /// Description of CBORTag26.
  /// </summary>
  public class CBORTag27 : ICBORTag
  {    
    public CBORTypeFilter GetTypeFilter()
    {
      return new CBORTypeFilter().WithArrayMinLength(1,CBORTypeFilter.Any);
    }
    
    public CBORObject ValidateObject(CBORObject obj)
    {
      if(obj.Type!= CBORType.Array || obj.Count<1)
        throw new CBORException("Not an array, or is an empty array.");
      return obj;
    }
  }
}
