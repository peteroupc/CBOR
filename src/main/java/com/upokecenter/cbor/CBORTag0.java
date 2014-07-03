package com.upokecenter.cbor;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/28/2014
 * Time: 11:49 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

    /**
     * Description of CBORTag0.
     */
  class CBORTag0 implements ICBORTag {

    static void AddConverter() {
      }
/**
 * Not documented yet.
 *
 * @return A CBORTypeFilter object.
 */
public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.TextString;
    }

/**
 * {@inheritDoc}
 *
 * Not documented yet.
 */
public CBORObject ValidateObject(final CBORObject obj) {
      if (obj.getType() != CBORType.TextString) {
 throw new CBORException("Not a text String");
}
      return obj;
    }
  }
