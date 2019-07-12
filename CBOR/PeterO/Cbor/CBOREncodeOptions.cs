using System;

namespace PeterO.Cbor {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Cbor.CBOREncodeOptions"]/*'/>
  public sealed class CBOREncodeOptions {
    /// <summary>Default options for CBOR objects. Disallow duplicate keys,
    /// and always encode strings using definite-length encoding.</summary>
    public static readonly CBOREncodeOptions Default =
      new CBOREncodeOptions(false, false);

    /// <summary>Default options for CBOR objects serialized using the
    /// CTAP2 canonicalization (used in Web Authentication, among other
    /// specifications). Disallow duplicate keys, and always encode strings
    /// using definite-length encoding.</summary>
    public static readonly CBOREncodeOptions DefaultCtap2Canonical =
      new CBOREncodeOptions(false, false, true);

    /// <summary>Initializes a new instance of the
    /// <see cref='CBOREncodeOptions'/> class.</summary>
    public CBOREncodeOptions() : this(false, false) {
}

    /// <summary>Initializes a new instance of the
    /// <see cref='CBOREncodeOptions'/> class.</summary>
    /// <param name='useIndefLengthStrings'>A value indicating whether to
    /// always encode strings with a definite-length encoding.</param>
    /// <param name='allowDuplicateKeys'>A value indicating whether to
    /// disallow duplicate keys when reading CBOR objects from a data
    /// stream.</param>
    public CBOREncodeOptions(
      bool useIndefLengthStrings,
      bool allowDuplicateKeys)
        : this(useIndefLengthStrings, allowDuplicateKeys, false) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='CBOREncodeOptions'/> class.</summary>
    /// <param name='useIndefLengthStrings'>A value indicating whether to
    /// encode strings with a definite-length encoding in certain
    /// cases.</param>
    /// <param name='allowDuplicateKeys'>A value indicating whether to
    /// allow duplicate keys when reading CBOR objects from a data
    /// stream.</param>
    /// <param name='ctap2Canonical'>A value indicating whether CBOR
    /// objects are written out using the CTAP2 canonical CBOR encoding
    /// form, which is useful for implementing Web Authentication.</param>
    public CBOREncodeOptions(
      bool useIndefLengthStrings,
      bool allowDuplicateKeys,
      bool ctap2Canonical) {
      this.UseIndefLengthStrings = useIndefLengthStrings;
      this.AllowDuplicateKeys = allowDuplicateKeys;
      this.Ctap2Canonical = ctap2Canonical;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.UseIndefLengthStrings"]/*'/>
    public bool UseIndefLengthStrings { get; private set; }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.AllowDuplicateKeys"]/*'/>
    public bool AllowDuplicateKeys { get; private set; }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.Ctap2Canonical"]/*'/>
    public bool Ctap2Canonical { get; private set; }
  }
}
