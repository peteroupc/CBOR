/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

 */
using System;
using System.Collections.Generic;
using PeterO;
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
      if (index >= this.sharedObjects.Count) {
        throw new CBORException("Index " + index + " is not valid");
      }
      return this.sharedObjects[index];
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
      if (index >= this.sharedObjects.Count) {
        throw new CBORException("Index " + index + " is not valid");
      }
      return this.sharedObjects[index];
    }
  }
}
