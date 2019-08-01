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
      this.ResolveReferences = false;
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
    /// end with whitespace. The following is an example of this parameter:
    /// <c>allowduplicatekeys=true;ctap2Canonical=true</c>. The key can be
    /// any one of the following in any combination of case:
    /// <c>allowduplicatekeys</c>, <c>ctap2canonical</c>,
    /// <c>resolvereferences</c>, <c>useindeflengthstrings</c>. Keys
    /// other than these are ignored. If the same key appears more than
    /// once, the value given for the last such key is used. The four keys
    /// just given can have a value of <c>1</c>, <c>true</c>, <c>yes</c>
    /// , or <c>on</c> (in any combination of case), which means true, and
    /// any other value meaning false. For example,
    /// <c>allowduplicatekeys=Yes</c> and <c>allowduplicatekeys=1</c> both
    /// set the <c>AllowDuplicateKeys</c> property to true.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='paramString'/> is null.</exception>
    public CBOREncodeOptions(string paramString) {
      if (paramString == null) {
        throw new ArgumentNullException(nameof(paramString));
      }
      var parser = new OptionsParser(paramString);
      this.ResolveReferences = parser.GetBoolean("resolvereferences", false);
      this.UseIndefLengthStrings =
parser.GetBoolean("useindeflengthstrings", false);
      this.AllowDuplicateKeys = parser.GetBoolean("allowduplicatekeys", false);
      this.Ctap2Canonical = parser.GetBoolean("ctap2canonical", false);
    }

    /// <summary>Gets the values of this options object's properties in
    /// text form.</summary>
    /// <returns>A text string containing the values of this options
    /// object's properties. The format of the string is the same as the
    /// one described in the String constructor for this class.</returns>
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
           .ToString();
    }

    /// <summary>Gets a value indicating whether to resolve references to
    /// sharable objects and sharable strings in the process of decoding a
    /// CBOR object.</summary>
    /// <value>A value indicating whether to resolve references to sharable
    /// objects and sharable strings. The default is false.</value>
    /// <remarks>
    /// <para>Sharable objects are marked with tag 28, and references to
    /// those objects are marked with tag 29 (where a reference of 0 means
    /// the first sharable object in the CBOR stream, a reference of 1
    /// means the second, and so on). Sharable strings (byte strings and
    /// text strings) appear within an enclosing object marked with tag
    /// 256, and references to them are marked with tag 25; in general, a
    /// string is sharable only if storing its reference rather than the
    /// string would save space.</para>
    /// <para>Note that unlike most other tags, these tags generally care
    /// about the relative order in which objects appear in a CBOR stream;
    /// thus they are not interoperable with CBOR implementations that
    /// follow the generic CBOR data model (since they may list map keys in
    /// an unspecified order). Interoperability problems with these tags
    /// can be reduced by not using them to mark keys or values of a map or
    /// to mark objects within those keys or values.</para></remarks>
    public bool ResolveReferences { get; private set; }

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
