/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using PeterO;

namespace PeterO.Cbor {
    /// <summary>Implements CBOR string references, described at <code>http://cbor.schmorp.de/stringref</code>
    /// </summary>
  internal class StringRefs
  {
    private List<List<CBORObject>> stack;

    public StringRefs() {
      this.stack = new List<List<CBORObject>>();
      var firstItem = new List<CBORObject>();
      this.stack.Add(firstItem);
    }

    /// <summary>Not documented yet.</summary>
    public void Push() {
      var firstItem = new List<CBORObject>();
      this.stack.Add(firstItem);
    }

    /// <summary>Not documented yet.</summary>
    public void Pop() {
      #if DEBUG
      if (this.stack.Count <= 0) {
        throw new ArgumentException("this.stack.Count (" + Convert.ToString((long)this.stack.Count, System.Globalization.CultureInfo.InvariantCulture) + ") is not greater than " + "0");
      }
      #endif

      this.stack.RemoveAt(this.stack.Count - 1);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='lengthHint'>A 32-bit signed integer.</param>
    public void AddStringIfNeeded(CBORObject str, int lengthHint) {
      #if DEBUG
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (!(str.Type == CBORType.ByteString || str.Type == CBORType.TextString)) {
        throw new ArgumentException("doesn't satisfy str.Type== CBORType.ByteString || str.Type== CBORType.TextString");
      }
      if (lengthHint < 0) {
        throw new ArgumentException("lengthHint (" + Convert.ToString((long)lengthHint, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      #endif

      bool addStr = false;
      List<CBORObject> lastList = this.stack[this.stack.Count - 1];
      if (lastList.Count < 24) {
        if (lengthHint >= 3) {
          addStr = true;
        }
      } else if (lastList.Count < 256) {
        if (lengthHint >= 4) {
          addStr = true;
        }
      } else if (lastList.Count < 65536) {
        if (lengthHint >= 5) {
          addStr = true;
        }
      } else if ((long)lastList.Count <= 0xFFFFFFFFL) {
        if (lengthHint >= 7) {
          addStr = true;
        }
      } else {
        if (lengthHint >= 11) {
          addStr = true;
        }
      }
      // Console.WriteLine("addStr=" + addStr + " lengthHint=" + lengthHint + " str=" + (str));
      if (addStr) {
        lastList.Add(str);
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='smallIndex'>A 64-bit signed integer.</param>
    /// <returns>A string object.</returns>
    public CBORObject GetString(long smallIndex) {
      if (smallIndex < 0) {
        throw new CBORException("Unexpected index");
      }
      if (smallIndex > Int32.MaxValue) {
        throw new CBORException("Index " + smallIndex + " is bigger than supported");
      }
      int index = (int)smallIndex;
      List<CBORObject> lastList = this.stack[this.stack.Count - 1];
      if (index >= lastList.Count) {
        throw new CBORException("Index " + index + " is not valid");
      }
      CBORObject ret = lastList[index];
      if (ret.Type == CBORType.ByteString) {
        // Byte strings are mutable, so make a copy
        return CBORObject.FromObject(ret.GetByteString());
      } else {
        return ret;
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A string object.</returns>
    /// <param name='bigIndex'>A BigInteger object.</param>
    public CBORObject GetString(BigInteger bigIndex) {
      if (bigIndex.Sign < 0) {
        throw new CBORException("Unexpected index");
      }
      if (!bigIndex.canFitInInt()) {
        throw new CBORException("Index " + bigIndex + " is bigger than supported");
      }
      int index = (int)bigIndex;
      List<CBORObject> lastList = this.stack[this.stack.Count - 1];
      if (index >= lastList.Count) {
        throw new CBORException("Index " + index + " is not valid");
      }
      CBORObject ret = lastList[index];
      if (ret.Type == CBORType.ByteString) {
        // Byte strings are mutable, so make a copy
        return CBORObject.FromObject(ret.GetByteString());
      } else {
        return ret;
      }
    }
  }
}
