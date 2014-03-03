package com.upokecenter.util;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/27/2014
 * Time: 7:43 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

    /**
     * Description of CBORTag5.
     */
  class CBORTag4 implements ICBORTag
  {
    private static CBORTypeFilter valueFilter = new CBORTypeFilter().WithArray(
      2,
      CBORTypeFilter.UnsignedInteger.WithNegativeInteger(),
      CBORTypeFilter.UnsignedInteger.WithNegativeInteger().WithTags(2, 3));

    /**
     * Not documented yet.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter GetTypeFilter() {
      return valueFilter;
    }

    /**
     * Not documented yet.
     * @param obj A CBORObject object. (2).
     * @return A CBORObject object.
     */
    public CBORObject ValidateObject(CBORObject obj) {
      return CBORTag5.ConvertToDecimalFrac(obj, true);
    }
  }
