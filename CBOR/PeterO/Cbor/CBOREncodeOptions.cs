using System;

namespace PeterO.Cbor {
    /// <summary>Specifies options for encoding and decoding CBOR
    /// objects.</summary>
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
    /// <see cref='PeterO.Cbor.CBOREncodeOptions'/> class.</summary>
    public CBOREncodeOptions() : this(false, false) {
}

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.CBOREncodeOptions'/> class.</summary>
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
    /// <see cref='PeterO.Cbor.CBOREncodeOptions'/> class.</summary>
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

    /// <summary>Gets a value indicating whether to encode strings with an
    /// indefinite-length encoding under certain circumstances.</summary>
    /// <value>A value indicating whether to encode strings with an
    /// indefinite-length encoding under certain circumstances. The default
    /// is false.</value>
    public bool UseIndefLengthStrings { get; private set; }

    /// <summary>Gets a value indicating whether to allow duplicate keys
    /// when reading CBOR objects from a data stream. Used only when
    /// decoding CBOR objects.</summary>
    /// <value>A value indicating whether to allow duplicate keys when
    /// reading CBOR objects from a data stream. The default is
    /// false.</value>
    public bool AllowDuplicateKeys { get; private set; }

    /// <summary>Gets a value indicating whether CBOR objects are written
    /// out using the CTAP2 canonical CBOR encoding form, which is useful
    /// for implementing Web Authentication. In this form, CBOR tags are
    /// not used, map keys are written out in a canonical order, and
    /// non-integer numbers and integers 2^63 or greater are written as
    /// 64-bit binary floating-point numbers.</summary>
    /// <value><c>true</c> if CBOR objects are written out using the CTAP2
    /// canonical CBOR encoding form; otherwise, <c>false</c>.</value>
    public bool Ctap2Canonical { get; private set; }
  }
}
