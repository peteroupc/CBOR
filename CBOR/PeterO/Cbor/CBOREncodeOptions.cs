using System;

namespace PeterO.Cbor {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Cbor.CBOREncodeOptions"]/*'/>
  public sealed class CBOREncodeOptions {

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBOREncodeOptions.Default"]/*'/>
    public static readonly CBOREncodeOptions Default =
      new CBOREncodeOptions(false, false);

    // TODO: Avoid
    private readonly int value;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBOREncodeOptions.#ctor"]/*'/>
    public CBOREncodeOptions() : this(false, false) {
}

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBOREncodeOptions.#ctor(System.Boolean,System.Boolean)"]/*'/>
    public CBOREncodeOptions(
  bool useIndefLengthStrings,
  bool allowDuplicateKeys) :
        this(useIndefLengthStrings, allowDuplicateKeys, false) {
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBOREncodeOptions.#ctor(System.Boolean,System.Boolean,System.Boolean)"]/*'/>
    public CBOREncodeOptions(
  bool useIndefLengthStrings,
  bool allowDuplicateKeys,
  bool ctap2Canonical) {
      var val = 0;
      if (!useIndefLengthStrings) {
        val |= 1;
      }
      if (!allowDuplicateKeys) {
        val |= 2;
      }
      this.value = val;
      this.Ctap2Canonical = ctap2Canonical;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.UseIndefLengthStrings"]/*'/>
    public bool UseIndefLengthStrings {
      get {
        return (this.value & 1) == 0;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.AllowDuplicateKeys"]/*'/>
    public bool AllowDuplicateKeys {
      get {
        return (this.value & 2) == 0;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.Ctap2Canonical"]/*'/>
    public bool Ctap2Canonical { get; private set; }
  }
}
