package com.upokecenter.util;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/27/2014
 * Time: 2:06 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

    /**
     * Implements CBOR tag 3.
     */
  class CBORTag3 implements ICBORTag
  {
    /**
     * Not documented yet.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.ByteString;
    }

    /**
     * Not documented yet.
     * @param obj A CBORObject object. (2).
     * @return A CBORObject object.
     */
    public CBORObject ValidateObject(CBORObject obj) {
      return CBORTag2.ConvertToBigNum(obj, true);
    }
  }
