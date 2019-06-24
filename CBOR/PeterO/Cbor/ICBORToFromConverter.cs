/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Cbor {
   /// <include file='../../docs.xml'
   /// path='docs/doc[@name="T:PeterO.Cbor.ICBORToFromConverter`1"]/*'/>
  public interface ICBORToFromConverter<T> : ICBORConverter<T>
  {
   /// <include file='../../docs.xml'
   /// path='docs/doc[@name="M:PeterO.Cbor.ICBORToFromConverter`1.FromCBORObject(PeterO.Cbor.CBORObject)"]/*'/>
    T FromCBORObject(CBORObject obj);
  }
}
