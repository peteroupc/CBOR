package com.upokecenter.cbor;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * A generic CBOR tag class for strings.
     */
  class CBORTagGenericString implements ICBORTag {

    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.TextString;
    }

    public CBORObject ValidateObject(CBORObject obj) {
      if (obj.getType() == CBORType.TextString) {
        throw new CBORException("Not a text String");
      }
      return obj;
    }
  }
