/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;

namespace PeterO.Cbor {
    /// <summary>Description of ICBORTag.</summary>
  public interface ICBORTag
  {
    CBORTypeFilter GetTypeFilter();

    // NOTE: Will be passed an object with the corresponding tag
    CBORObject ValidateObject(CBORObject obj);
  }
}
