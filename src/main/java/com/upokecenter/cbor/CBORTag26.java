package com.upokecenter.cbor;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  class CBORTag26 implements ICBORTag {

    public CBORTypeFilter GetTypeFilter() {
      return new CBORTypeFilter().WithArrayMinLength(1, CBORTypeFilter.Any);
    }

    public CBORObject ValidateObject(CBORObject obj) {
      if (obj.getType() != CBORType.Array || obj.size() < 1) {
        throw new CBORException("Not an array, or is an empty array.");
      }
      return obj;
    }
  }
