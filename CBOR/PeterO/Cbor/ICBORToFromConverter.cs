/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */

namespace PeterO.Cbor {
  /// <summary>Classes that implement this interface can support
  /// conversions from CBOR objects to a custom type and back.</summary>
  /// <typeparam name='T'>Type of objects to convert to and from CBOR
  /// objects.</typeparam>
  public interface ICBORToFromConverter<T> : ICBORConverter<T>
  {
    /// <summary>Converts a CBOR object to a custom type.</summary>
    /// <param name='obj'>A CBOR object to convert to the custom
    /// type.</param>
    /// <returns>An object of the custom type after conversion.</returns>
    T FromCBORObject(CBORObject obj);
  }
}
