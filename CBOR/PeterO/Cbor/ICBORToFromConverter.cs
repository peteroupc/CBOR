/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Cbor {
    /// <summary>Not documented yet.</summary>
    /// <typeparam name='T'>Type parameter not documented yet.</typeparam>
  public interface ICBORToFromConverter<T> : ICBORConverter<T>
  {
    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>Not documented yet.</param>
    /// <returns>Not documented yet.</returns>
    T FromCBORObject(CBORObject obj);
  }
}
