/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using PeterO;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal class CBORTag2 : ICBORTag
  {
    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.ByteString;
    }

    public CBORObject ValidateObject(CBORObject obj) {
      return ConvertToBigNum(obj, false);
    }
  }
}
