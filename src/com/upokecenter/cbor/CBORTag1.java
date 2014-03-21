package com.upokecenter.cbor;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/7/2014
 * Time: 9:44 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

    /**
     * Description of CBORTag1.
     */
  public class CBORTag1 implements ICBORTag
  {
    /**
     * Not documented yet.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.UnsignedInteger.WithNegativeInteger().WithFloatingPoint();
    }

    /**
     * Not documented yet.
     * @param obj A CBORObject object. (2).
     * @return A CBORObject object.
     */
    public CBORObject ValidateObject(CBORObject obj) {
      if (!obj.isFinite()) {
        throw new CBORException("Not a valid date");
      }
      return obj;
    }
  }
