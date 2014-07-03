package com.upokecenter.cbor;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Implemented by classes that validate CBOR objects belonging to a specific
     * tag.
     */
  public interface ICBORTag {

    /**
     * Gets a type filter specifying what kinds of CBOR objects are supported by
     * this tag.
     * @return A CBOR type filter.
     */
    CBORTypeFilter GetTypeFilter();

    /**
     * Generates a CBOR object based on the data of another object. If the data is
     * not valid, should throw a CBORException.
     * @param obj A CBOR object with the corresponding tag handled by the ICBORTag
     * object.
     * @return A CBORObject object. Note that this method may choose to return the
     * same object as the parameter.
     */
    CBORObject ValidateObject(CBORObject obj);
  }
