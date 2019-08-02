/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
#pragma warning disable 618
namespace PeterO.Cbor {
    /// <summary>Implements CBOR tag 3.</summary>
  internal class CBORTag3 : ICBORTag
  {
    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.ByteString;
    }

    public CBORObject ValidateObject(CBORObject obj) {
      return CBORTag2.ConvertToBigNum(obj, true);
    }
  }
}
