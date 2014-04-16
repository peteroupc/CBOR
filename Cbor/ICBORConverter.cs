/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;

namespace PeterO.Cbor
{
    /// <summary>Description of ICBORConverter.</summary>
    /// <typeparam name='T'>Type to convert to a CBOR object.</typeparam>
  public interface ICBORConverter<T>
  {
    CBORObject ToCBORObject(T obj);
  }
}
