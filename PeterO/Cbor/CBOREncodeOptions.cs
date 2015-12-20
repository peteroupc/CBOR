using System;

namespace PeterO.Cbor {
    /// <summary>Specifies options for encoding and decoding CBOR
    /// objects.</summary>
  public sealed class CBOREncodeOptions {
    /// <summary>No special options for encoding/decoding. Value:
    /// 0.</summary>
    public static readonly CBOREncodeOptions None =
      new CBOREncodeOptions(0);

    /// <summary>Always encode strings with a definite-length encoding.
    /// Used only when encoding CBOR objects. Value: 1.</summary>
    public static readonly CBOREncodeOptions NoIndefLengthStrings =
      new CBOREncodeOptions(1);

    /// <summary>Disallow duplicate keys when reading CBOR objects from a
    /// data stream. Used only when decoding CBOR objects. Value:
    /// 2.</summary>
    public static readonly CBOREncodeOptions NoDuplicateKeys =
      new CBOREncodeOptions(2);

    private readonly int value;

    /// <summary>Gets this options object's value.</summary>
    /// <value>This options object&apos;s value.</value>
    public int Value {
      get {
 return this.value;
}
    }

    private CBOREncodeOptions(int value) {
      this.value = value;
    }

    /// <summary>Combines the flags of this options object with another
    /// options object.</summary>
    /// <param name='o'>Another CBOREncodeOptions object.</param>
    /// <returns>A CBOREncodeOptions object.</returns>
    public CBOREncodeOptions Or(CBOREncodeOptions o) {
      return new CBOREncodeOptions(this.value | o.value);
    }

    /// <summary>Returns an options object whose flags are shared by this
    /// and another options object.</summary>
    /// <param name='o'>Another CBOREncodeOptions object.</param>
    /// <returns>A CBOREncodeOptions object.</returns>
    public CBOREncodeOptions And(CBOREncodeOptions o) {
      return new CBOREncodeOptions(this.value & o.value);
    }
  }
}
