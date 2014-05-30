/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;

namespace PeterO.Cbor {
  internal class CBORTag1 : ICBORTag
  {
    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.UnsignedInteger.WithNegativeInteger().WithFloatingPoint();
    }

    public CBORObject ValidateObject(CBORObject obj) {
      if (!obj.IsFinite) {
        throw new CBORException("Not a valid date");
      }
      return obj;
    }
  }
}
