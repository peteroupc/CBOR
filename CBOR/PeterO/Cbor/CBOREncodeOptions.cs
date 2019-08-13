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
      this.ResolveReferences = false;
      this.AllowEmpty = false;
      this.UseIndefLengthStrings = useIndefLengthStrings;
      this.AllowDuplicateKeys = allowDuplicateKeys;
      this.Ctap2Canonical = ctap2Canonical;
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.CBOREncodeOptions'/> class.</summary>
    /// <param name='paramString'>A string setting forth the options to
    /// use. This is a semicolon-separated list of options, each of which
    /// has a key and a value separated by an equal sign ("="). Whitespace
    /// and line separators are not allowed to appear between the
    /// semicolons or between the equal signs, nor may the string begin or
    /// end with whitespace. The string can be empty, but cannot be null.
    /// The following is an example of this parameter:
    /// <c>allowduplicatekeys=true;ctap2Canonical=true</c>. The key can be
    /// any one of the following in any combination of case:
    /// <c>allowduplicatekeys</c>, <c>ctap2canonical</c>,
    /// <c>resolvereferences</c>, <c>useindeflengthstrings</c>,
    /// <c>allowempty</c>. Keys other than these are ignored. If the same
    /// key appears more than once, the value given for the last such key
    /// is used. The four keys just given can have a value of <c>1</c>,
    /// <c>true</c>, <c>yes</c>, or <c>on</c> (in any combination of
    /// case), which means true, and any other value meaning false. For
    /// example, <c>allowduplicatekeys=Yes</c> and
    /// <c>allowduplicatekeys=1</c> both set the <c>AllowDuplicateKeys</c>
    /// property to true.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='paramString'/> is null.</exception>
    public CBOREncodeOptions(string paramString) {
      if (paramString == null) {
        throw new ArgumentNullException(nameof(paramString));
      }
      var parser = new OptionsParser(paramString);
      this.ResolveReferences = parser.GetBoolean("resolvereferences", false);
      this.UseIndefLengthStrings = parser.GetBoolean(
        "useindeflengthstrings",
        false);
      this.AllowDuplicateKeys = parser.GetBoolean("allowduplicatekeys", false);
      this.AllowEmpty = parser.GetBoolean("allowempty", false);
      this.Ctap2Canonical = parser.GetBoolean("ctap2canonical", false);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBOREncodeOptions.ToString"]/*'/>
    public override string ToString() {
      return new System.Text.StringBuilder()
           .Append("allowduplicatekeys=")
           .Append(this.AllowDuplicateKeys ? "true" : "false")
           .Append(";useindeflengthstrings=")
           .Append(this.UseIndefLengthStrings ? "true" : "false")
           .Append(";ctap2canonical=")
           .Append(this.Ctap2Canonical ? "true" : "false")
           .Append(";resolvereferences=")
           .Append(this.ResolveReferences ? "true" : "false")
           .Append(";allowempty=").Append(this.AllowEmpty ? "true" : "false")
           .ToString();
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.ResolveReferences"]/*'/>
    public bool ResolveReferences { get; private set; }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.UseIndefLengthStrings"]/*'/>
    public bool UseIndefLengthStrings { get; private set; }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.AllowEmpty"]/*'/>
    public bool AllowEmpty { get; private set; }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.AllowDuplicateKeys"]/*'/>
    public bool AllowDuplicateKeys { get; private set; }

    // TODO: Support rejection of canonical CBOR in decoding
    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.Ctap2Canonical"]/*'/>
    public bool Ctap2Canonical { get; private set; }
  }
}
