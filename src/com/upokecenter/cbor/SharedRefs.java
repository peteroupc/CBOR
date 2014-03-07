package com.upokecenter.cbor;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/7/2014
 * Time: 10:27 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import java.util.*;
import com.upokecenter.util.*;

    /**
     * Description of SharedRefs.
     */
  class SharedRefs
  {
    private List<CBORObject> sharedObjects;
    private int refCount;

    public SharedRefs () {
      sharedObjects = new ArrayList<CBORObject>();
    }

    /**
     * Not documented yet.
     * @param obj A CBORObject object.
     */
public void FillRef(CBORObject obj) {
      if (refCount>0) {
        sharedObjects.add(obj);
        refCount-=1;
      }
    }

    /**
     * Not documented yet.
     */
public void AddRef() {
      if (refCount == Integer.MAX_VALUE) {
 throw new CBORException("Shared ref nesting too deep");
}
      refCount+=1;
    }

    /**
     * Not documented yet.
     * @param smallIndex A 64-bit signed integer.
     * @return A string object.
     */
    public CBORObject GetObject(long smallIndex) {
      if (smallIndex < 0) {
        throw new CBORException("Unexpected index");
      }
      if (smallIndex > Integer.MAX_VALUE) {
        throw new CBORException("Index " + smallIndex + " is bigger than supported");
      }
      int index = (int)smallIndex;
      if (index >= sharedObjects.size()) {
        throw new CBORException("Index " + index + " is not valid");
      }
      return sharedObjects.get(index);
    }

    /**
     * Not documented yet.
     * @param bigIndex A BigInteger object.
     * @return A string object.
     */
    public CBORObject GetObject(BigInteger bigIndex) {
      if (bigIndex.signum() < 0) {
        throw new CBORException("Unexpected index");
      }
      if (!bigIndex.canFitInInt()) {
        throw new CBORException("Index " + bigIndex + " is bigger than supported");
      }
      int index = bigIndex.intValue();
      List<CBORObject> sharedObjects = this.stack.get(this.stack.size() - 1);
      if (index >= sharedObjects.size()) {
        throw new CBORException("Index " + index + " is not valid");
      }
      return sharedObjects.get(index);
    }
  }
