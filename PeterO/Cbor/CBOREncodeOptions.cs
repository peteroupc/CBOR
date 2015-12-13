using System;

namespace PeterO.Cbor {
    /// <summary>Specifies options for encoding CBOR objects to
    /// bytes.</summary>
  public sealed class CBOREncodeOptions {
    /// <summary>No special options for encoding. Value: 0.</summary>
    public static readonly CBOREncodeOptions None =
      new CBOREncodeOptions(0);

    /// <summary>Always encode strings with a definite-length encoding.
    /// Value: 1.</summary>
    public static readonly CBOREncodeOptions NoIndefLengthStrings =
      new CBOREncodeOptions(1);

    private readonly int value;

    /// <summary>Gets this options object's value.</summary>
    /// <value>This options object&apos;s value.</value>
    public int Value {
      get {
 return this.value;
}
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='other'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public override bool Equals(object other) {
      var enc = other as CBOREncodeOptions;
      return (enc == null) ? (false) : (enc.Value == this.Value);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public override int GetHashCode() {
      return this.Value;
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
