using System;

namespace PeterO.Cbor {
    /// <summary>Not documented yet.</summary>
    /// <typeparam name='T'>Type parameter not documented yet.</typeparam>
  public interface ICBORToFromConverter<T> : ICBORConverter<T> {
    /// <summary>Not documented yet.</summary>
    /// <param name='cbor'>Not documented yet.</param>
    /// <returns>Not documented yet.</returns>
    T FromCBORObject(CBORObject cbor);
  }
}
