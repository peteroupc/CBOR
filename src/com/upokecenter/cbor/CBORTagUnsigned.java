package com.upokecenter.cbor;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/6/2014
 * Time: 10:05 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

    /**
     * Description of CBORTag25.
     */
  class CBORTagUnsigned implements ICBORTag
  {
    /**
     * Not documented yet.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.UnsignedInteger;
    }

    /**
     * Not documented yet.
     * @param obj A CBORObject object. (2).
     * @return A CBORObject object.
     */
    public CBORObject ValidateObject(CBORObject obj) {

      if (!obj.isIntegral() || !obj.CanFitInInt64() || obj.signum() < 0) {
        throw new CBORException("Not a 64-bit unsigned integer");
      }
      return obj;
    }
  }
