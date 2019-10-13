/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Cbor {
  /// <summary>Interface implemented by classes that convert objects of
  /// arbitrary types to CBOR objects.</summary>
  /// <typeparam name='T'>Type to convert to a CBOR object.</typeparam>
  public interface ICBORConverter<T> {
    /// <summary>Converts an object to a CBOR object.</summary>
    /// <param name='obj'>An object to convert to a CBOR object.</param>
    /// <returns>A CBOR object.</returns>
    CBORObject ToCBORObject(T obj);
  }
}
