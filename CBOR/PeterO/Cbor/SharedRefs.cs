/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal class SharedRefs {
    private readonly IList<CBORObject> sharedObjects;

    public SharedRefs() {
      this.sharedObjects = new List<CBORObject>();
    }

    public void AddObject(CBORObject obj) {
      this.sharedObjects.Add(obj);
    }

    public CBORObject GetObject(long smallIndex) {
      if (smallIndex < 0) {
        throw new CBORException("Unexpected index");
      }
      if (smallIndex > Int32.MaxValue) {
        throw new CBORException("Index " + smallIndex +
          " is bigger than supported ");
      }
      var index = (int)smallIndex;
      return index >= this.sharedObjects.Count ? throw new
CBORException("Index " + index + " is not valid") :
this.sharedObjects[index];
    }

    public CBORObject GetObject(EInteger bigIndex) {
      if (bigIndex.Sign < 0) {
        throw new CBORException("Unexpected index");
      }
      if (!bigIndex.CanFitInInt32()) {
        throw new CBORException("Index " + bigIndex +
          " is bigger than supported ");
      }
      var index = (int)bigIndex;
      return index >= this.sharedObjects.Count ? throw new
CBORException("Index " + index + " is not valid") :
this.sharedObjects[index];
    }
  }
}
