package com.upokecenter.cbor;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/27/2014
 * Time: 2:08 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import com.upokecenter.util.*;

    /**
     * Description of CBORTag30.
     */
  class CBORTag30 implements ICBORTag
  {
    /**
     * Not documented yet.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter GetTypeFilter() {
      return new CBORTypeFilter().WithArray(
        2,
        CBORTypeFilter.UnsignedInteger.WithNegativeInteger().WithTags(2, 3),
        CBORTypeFilter.UnsignedInteger.WithTags(2));
    }

    /**
     * Not documented yet.
     * @param obj A CBORObject object. (2).
     * @return A CBORObject object.
     */
    public CBORObject ValidateObject(CBORObject obj) {
      if (obj.getType() != CBORType.Array) {
        throw new CBORException("Rational number must be an array");
      }
      if (obj.size() != 2) {
        throw new CBORException("Rational number requires exactly 2 items");
      }
      CBORObject first = obj.get(0);
      CBORObject second = obj.get(1);
      if (!first.isIntegral()) {
        throw new CBORException("Rational number requires integer numerator");
      }
      if (!second.isIntegral()) {
        throw new CBORException("Rational number requires integer denominator");
      }
      if (second.signum() <= 0) {
        throw new CBORException("Rational number requires denominator greater than 0");
      }
      BigInteger denom = second.AsBigInteger();
      // NOTE: Discards tags.  See comment in CBORTag2.
      if (denom.equals(BigInteger.ONE)) {
        return CBORObject.FromObject(first.AsBigInteger());
      }
      return CBORObject.FromObject(new ExtendedRational(first.AsBigInteger(), denom));
    }
  }
