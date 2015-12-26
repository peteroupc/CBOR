/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Cbor {
    /// <include file='docs.xml' 
    /// path='docs/doc[@name="T:PeterO.Cbor.ICBORTag"]'/>
  public interface ICBORTag
  {
    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Cbor.ICBORTag.GetTypeFilter"]'/>
    CBORTypeFilter GetTypeFilter();

    /// <include file='docs.xml' 
    /// path='docs/doc[@name="M:PeterO.Cbor.ICBORTag.ValidateObject(PeterO.Cbor.CBORObject)"]'/>
    CBORObject ValidateObject(CBORObject obj);
  }
}
