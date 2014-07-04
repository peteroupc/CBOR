package com.upokecenter.cbor;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  class CBORTagUnsigned implements ICBORTag {

    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.UnsignedInteger;
    }

    public CBORObject ValidateObject(CBORObject obj) {
      if (!obj.isIntegral() || !obj.CanFitInInt64() || obj.signum() < 0) {
        throw new CBORException("Not a 64-bit unsigned integer");
      }
      return obj;
    }
  }
