package com.upokecenter.cbor;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Implements CBOR tag 3.
     */
  class CBORTag3 implements ICBORTag {

    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.ByteString;
    }

    public CBORObject ValidateObject(CBORObject obj) {
      return CBORTag2.ConvertToBigNum(obj, true);
    }
  }
