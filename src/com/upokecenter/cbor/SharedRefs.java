package com.upokecenter.cbor;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
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
      this.sharedObjects = new ArrayList<CBORObject>();
    }

    /**
     * Not documented yet.
     * @param obj A CBORObject object.
     */
    public void AddObject(CBORObject obj) {
      if (this.refCount > 0) {
        this.sharedObjects.add(obj);
        --this.refCount;
      }
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
      if (index >= this.sharedObjects.size()) {
        throw new CBORException("Index " + index + " is not valid");
      }
      return this.sharedObjects.get(index);
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
      if (index >= this.sharedObjects.size()) {
        throw new CBORException("Index " + index + " is not valid");
      }
      return this.sharedObjects.get(index);
    }
  }
