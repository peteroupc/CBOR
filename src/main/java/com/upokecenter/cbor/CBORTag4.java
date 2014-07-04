package com.upokecenter.cbor;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  class CBORTag4 implements ICBORTag {

    public CBORTag4 () {
 this(false);
    }

    private boolean extended;

    public CBORTag4 (boolean extended) {
      this.extended = extended;
    }

    public CBORTypeFilter GetTypeFilter() {
      return this.extended ? CBORTag5.ExtendedFilter : CBORTag5.Filter;
    }

    public CBORObject ValidateObject(CBORObject obj) {
      return CBORTag5.ConvertToDecimalFrac(obj, true, this.extended);
    }
  }
