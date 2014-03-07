package com.upokecenter.cbor;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/6/2014
 * Time: 9:54 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import java.util.*;

  /**
   * Description of StringRefs.
   */
  class StringRefs
  {
    private ArrayList<List<CBORObject>> stack;

    public StringRefs () {
      this.stack = new ArrayList<List<CBORObject>>();
      this.stack.Add(new ArrayList<CBORObject>());
    }

    /**
     * Not documented yet.
     */
    public void Push() {
      this.stack.Add(new ArrayList<CBORObject>());
    }

    /**
     * Not documented yet.
     */
    public void Pop() {

      this.stack.RemoveAt(this.stack.size() - 1);
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @param lengthHint A 32-bit signed integer.
     */
    public void AddStringIfNeeded(CBORObject str, int lengthHint) {

      boolean addStr = false;
      List<CBORObject> lastList = this.stack.get(this.stack.size() - 1);
      if (lastList.size() < 24) {
        if (lengthHint >= 3) {
          addStr = true;
        }
      } else if (lastList.size() < 256) {
        if (lengthHint >= 4) {
          addStr = true;
        }
      } else if (lastList.size() < 65536) {
        if (lengthHint >= 5) {
          addStr = true;
        }
      } else if ((long)lastList.size() <= 0xFFFFFFFFL) {
        if (lengthHint >= 7) {
          addStr = true;
        }
      } else {
        if (lengthHint >= 11) {
          addStr = true;
        }
      }
      if (addStr) {
        lastList.add(str);
      }
    }

    /**
     * Not documented yet.
     * @param smallIndex A 64-bit signed integer.
     * @return A string object.
     */
    public CBORObject GetString(long smallIndex) {
      throw new UnsupportedOperationException();
    }

    /**
     * Not documented yet.
     * @param smallIndex A BigInteger object.
     * @return A string object.
     */
    public CBORObject GetString(BigInteger smallIndex) {
      throw new UnsupportedOperationException();
    }
  }
