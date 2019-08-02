using System;

namespace PeterO.Cbor {
    /// <summary>Interface implemented by classes that convert objects of arbitrary types
    /// to and from CBOR objects.</summary>
    /// <typeparam name='T'>
    /// Type of objects that a class implementing this method can convert to and
    /// from CBOR objects.
    /// </typeparam>
  public interface ICBORToFromConverter<T> : ICBORConverter<T> {
    /// <summary>Converts a CBOR object to an object of a type supported by the
    /// implementing class.</summary>
    /// <param name='cbor'>A CBOR object to convert.
    /// </param>
    /// <returns>The converted object.
    /// </returns>
    /// <exception cref='PeterO.Cbor.CBORException'>An error occurred in the conversion; for example, the conversion doesn't
    /// support the given CBOR object.</exception>
    T FromCBORObject(CBORObject cbor);
  }
}
