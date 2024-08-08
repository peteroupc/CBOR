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
  /// <summary>Implements CBOR string references, described at
  /// <c>http://cbor.schmorp.de/stringref</c>.</summary>
  internal class StringRefs {
    private readonly List<List<CBORObject>> stack;

    public StringRefs() {
      this.stack = new List<List<CBORObject>>();
      var firstItem = new List<CBORObject>();
      this.stack.Add(firstItem);
    }

    public void Push() {
      var firstItem = new List<CBORObject>();
      this.stack.Add(firstItem);
    }

    public void Pop() {
#if DEBUG
      if (this.stack.Count <= 0) {
        throw new ArgumentException("this.stack.Count(" + this.stack.Count +
          ") is not greater than " + "0 ");
      }
#endif
      this.stack.RemoveAt(this.stack.Count - 1);
    }

    public void AddStringIfNeeded(CBORObject str, int lengthHint) {
#if DEBUG
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (!(str.Type == CBORType.ByteString || str.Type ==
          CBORType.TextString)) {
        throw new ArgumentException(
          "doesn't satisfy str.Type== ByteString or TextString");
      }
      if (lengthHint < 0) {
        throw new ArgumentException("lengthHint(" + lengthHint +
          ") is less than " + "0 ");
      }
#endif
      var addStr = false;
      List<CBORObject> lastList = this.stack[this.stack.Count - 1];
      if (lastList.Count < 24) {
        addStr |= lengthHint >= 3;
      } else if (lastList.Count < 256) {
        addStr |= lengthHint >= 4;
      } else if (lastList.Count < 65536) {
        addStr |= lengthHint >= 5;
      } else {
        // NOTE: lastList's size can't be higher than (2^64)-1
        addStr |= lengthHint >= 7;
      }
      // NOTE: An additional branch, with lengthHint >= 11, would
      // be needed if the size could be higher than (2^64)-1
      if (addStr) {
        lastList.Add(str);
      }
    }

    public CBORObject GetString(long smallIndex) {
      if (smallIndex < 0) {
        throw new CBORException("Unexpected index");
      }
      if (smallIndex > Int32.MaxValue) {
        throw new CBORException("Index " + smallIndex +
          " is bigger than supported ");
      }
      var index = (int)smallIndex;
      List<CBORObject> lastList = this.stack[this.stack.Count - 1];
      if (index >= lastList.Count) {
        throw new CBORException("Index " + index + " is not valid");
      }
      CBORObject ret = lastList[index];
      // Byte strings are mutable, so make a copy
      return (ret.Type == CBORType.ByteString) ?
        CBORObject.FromByteArray(ret.GetByteString()) : ret;
    }

    public CBORObject GetString(EInteger bigIndex) {
      if (bigIndex.Sign < 0) {
        throw new CBORException("Unexpected index");
      }
      if (!bigIndex.CanFitInInt32()) {
        throw new CBORException("Index " + bigIndex +
          " is bigger than supported ");
      }
      var index = (int)bigIndex;
      List<CBORObject> lastList = this.stack[this.stack.Count - 1];
      if (index >= lastList.Count) {
        throw new CBORException("Index " + index + " is not valid");
      }
      CBORObject ret = lastList[index];
      // Byte strings are mutable, so make a copy
      return (ret.Type == CBORType.ByteString) ?
        CBORObject.FromByteArray(ret.GetByteString()) : ret;
    }
  }
}
