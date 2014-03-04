package com.upokecenter.cbor;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/27/2014
 * Time: 2:03 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

    /**
     * Description of ICBORTag.
     */
  public interface ICBORTag
  {
    CBORTypeFilter GetTypeFilter();

    // NOTE: Will be passed an object with the corresponding tag
    CBORObject ValidateObject(CBORObject obj);
  }
