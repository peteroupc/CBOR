/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/6/2014
 * Time: 9:54 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using PeterO;
using System.Collections.Generic;

namespace PeterO.Cbor
{
  /// <summary>Description of StringRefs.</summary>
  internal class StringRefs
  {
    private List<IList<CBORObject>> stack;

    public StringRefs() {
      this.stack = new List<IList<CBORObject>>();
      this.stack.Add(new List<CBORObject>());
    }

    /// <summary>Not documented yet.</summary>
    public void Push() {
      this.stack.Add(new List<CBORObject>());
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
      #endif

      bool addStr = false;
      IList<CBORObject> lastList = this.stack[this.stack.Count - 1];
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
      //Console.WriteLine("addStr={0} lengthHint={1} str={2}",addStr,lengthHint,str);
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
      IList<CBORObject> lastList = this.stack[this.stack.Count - 1];
      if (index >= lastList.Count) {
        throw new CBORException("Index " + index + " is not valid");
      }
      return lastList[index];
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
      IList<CBORObject> lastList = this.stack[this.stack.Count - 1];
      if (index >= lastList.Count) {
        throw new CBORException("Index " + index + " is not valid");
      }
      return lastList[index];
    }
  }
}
