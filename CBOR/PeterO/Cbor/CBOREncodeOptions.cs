using System;

namespace PeterO.Cbor {
   /// <include file='../../docs.xml'
  /// path='docs/doc[@name="T:PeterO.Cbor.CBOREncodeOptions"]/*'/>
  public sealed class CBOREncodeOptions {
   ///
   /// <summary>Default options for CBOR objects. Disallow duplicate keys, and always
   /// encode strings using definite-length encoding.
   /// </summary>
   ///
    public static readonly CBOREncodeOptions Default =
      new CBOREncodeOptions(false, false);

   ///
   /// <summary>Default options for CBOR objects serialized using the CTAP2
   /// canonicalization (used in Web Authentication, among other specifications).
   /// Disallow duplicate keys, and always encode strings using definite-length
   /// encoding.
   /// </summary>
   ///
    public static readonly CBOREncodeOptions DefaultCtap2Canonical =
      new CBOREncodeOptions(false, false, true);

   /// <include file='../../docs.xml'
  /// path='docs/doc[@name="M:PeterO.Cbor.CBOREncodeOptions.#ctor"]/*'/>
    public CBOREncodeOptions() : this(false, false) {
}

   /// <include file='../../docs.xml'
  /// path='docs/doc[@name="M:PeterO.Cbor.CBOREncodeOptions.#ctor(System.Boolean,System.Boolean)"]/*'/>
    public CBOREncodeOptions(
  bool useIndefLengthStrings,
  bool allowDuplicateKeys)
        : this(useIndefLengthStrings, allowDuplicateKeys, false) {
    }

   /// <include file='../../docs.xml'
  /// path='docs/doc[@name="M:PeterO.Cbor.CBOREncodeOptions.#ctor(System.Boolean,System.Boolean,System.Boolean)"]/*'/>
    public CBOREncodeOptions(
  bool useIndefLengthStrings,
  bool allowDuplicateKeys,
  bool ctap2Canonical) {
      this.UseIndefLengthStrings = useIndefLengthStrings;
      this.AllowDuplicateKeys = allowDuplicateKeys;
      this.Ctap2Canonical = ctap2Canonical;
    }

   /// <include file='../../docs.xml'
  /// path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.UseIndefLengthStrings"]/*'/>
    public bool UseIndefLengthStrings { get; private set; }

   /// <include file='../../docs.xml'
  /// path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.AllowDuplicateKeys"]/*'/>
    public bool AllowDuplicateKeys { get; private set; }

   /// <include file='../../docs.xml'
  /// path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.Ctap2Canonical"]/*'/>
    public bool Ctap2Canonical { get; private set; }
  }
}
