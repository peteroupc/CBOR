package com.upokecenter.cbor;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  class CBORTag28 implements ICBORTag {

    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.Any;
    }

    public CBORObject ValidateObject(CBORObject obj) {
      // Return this object without tag 28

      return obj.UntagOne();
    }
  }
