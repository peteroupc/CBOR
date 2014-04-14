package com.upokecenter.cbor;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/7/2014
 * Time: 10:01 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

    /**
     * Description of CBORTag26.
     */
  class CBORTag26 implements ICBORTag
  {
    /**
     * Not documented yet.
     * @return A CBORTypeFilter object.
     */
public CBORTypeFilter GetTypeFilter() {
      return new CBORTypeFilter().WithArrayMinLength(1, CBORTypeFilter.Any);
    }

    /**
     * Not documented yet.
     * @param obj A CBORObject object. (2).
     * @return A CBORObject object.
     */
public CBORObject ValidateObject(CBORObject obj) {
      if (obj.getType() != CBORType.Array || obj.size() < 1) {
 throw new CBORException("Not an array, or is an empty array.");
}
      return obj;
    }
  }
