/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/7/2014
 * Time: 10:27 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using PeterO;

namespace PeterO.Cbor
{
  /// <summary>
  /// Description of SharedRefs.
  /// </summary>
  internal class SharedRefs
  {
    private IList<CBORObject> sharedObjects;
    private int refCount;
    
    public SharedRefs()
    {
      sharedObjects=new List<CBORObject>();
    }
    
    public void FillRef(CBORObject obj){
      if(refCount>0){
        sharedObjects.Add(obj);
        refCount-=1;
      }
    }
    
    public void AddRef()
    {
      if(refCount==Int32.MaxValue)
        throw new CBORException("Shared ref nesting too deep");
      refCount+=1;
    }
    
        /// <summary>Not documented yet.</summary>
    /// <param name='smallIndex'>A 64-bit signed integer.</param>
    /// <returns>A string object.</returns>
    public CBORObject GetObject(long smallIndex) {
      if (smallIndex < 0) {
        throw new CBORException("Unexpected index");
      }
      if (smallIndex > Int32.MaxValue) {
        throw new CBORException("Index " + smallIndex + " is bigger than supported");
      }
      int index = (int)smallIndex;
      if (index >= sharedObjects.Count) {
        throw new CBORException("Index " + index + " is not valid");
      }
      return sharedObjects[index];
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A string object.</returns>
    /// <param name='bigIndex'>A BigInteger object.</param>
    public CBORObject GetObject(BigInteger bigIndex) {
      if (bigIndex.Sign < 0) {
        throw new CBORException("Unexpected index");
      }
      if (!bigIndex.canFitInInt()) {
        throw new CBORException("Index " + bigIndex + " is bigger than supported");
      }
      int index = (int)bigIndex;
      IList<CBORObject> sharedObjects = this.stack[this.stack.Count - 1];
      if (index >= sharedObjects.Count) {
        throw new CBORException("Index " + index + " is not valid");
      }
      return sharedObjects[index];
    }
  }
}
