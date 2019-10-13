/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Cbor {
  /// <summary>Classes that implement this interface can support
  /// conversions from CBOR objects to a custom type and back.</summary>
  /// <typeparam name='T'>Type of objects to convert to and from CBOR
  /// objects.</typeparam>
  public interface ICBORToFromConverter<T> : ICBORConverter<T> {
    /// <summary>Converts a CBOR object to a custom type.</summary>
    /// <param name='obj'>A CBOR object to convert to the custom
    /// type.</param>
    /// <returns>An object of the custom type after conversion.</returns>
    T FromCBORObject(CBORObject obj);
  }
}
