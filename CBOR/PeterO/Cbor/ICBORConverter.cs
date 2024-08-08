/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */

namespace PeterO.Cbor {
  /// <summary>Interface implemented by classes that convert objects of
  /// arbitrary types to CBOR objects.</summary>
  /// <typeparam name='T'>Type to convert to a CBOR object.</typeparam>
  public interface ICBORConverter<T>
  {
    /// <summary>Converts an object to a CBOR object.</summary>
    /// <param name='obj'>An object to convert to a CBOR object.</param>
    /// <returns>A CBOR object.</returns>
    CBORObject ToCBORObject(T obj);
  }
}
