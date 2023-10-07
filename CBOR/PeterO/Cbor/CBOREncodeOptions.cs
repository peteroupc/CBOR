using System;

namespace PeterO.Cbor {
  /// <summary>Specifies options for encoding and decoding CBOR
  /// objects.</summary>
  public sealed class CBOREncodeOptions {
    /// <summary>Default options for CBOR objects. Disallow duplicate keys,
    /// and always encode strings using definite-length encoding.</summary>
    public static readonly CBOREncodeOptions Default =
      new CBOREncodeOptions();

    /// <summary>Default options for CBOR objects serialized using the
    /// CTAP2 canonicalization (used in Web Authentication, among other
    /// specifications). Disallow duplicate keys, and always encode strings
    /// using definite-length encoding.</summary>
    public static readonly CBOREncodeOptions DefaultCtap2Canonical =
      new CBOREncodeOptions("ctap2canonical=true");

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.CBOREncodeOptions'/> class with all the
    /// default options.</summary>
    public CBOREncodeOptions() : this(String.Empty) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.CBOREncodeOptions'/> class.</summary>
    /// <param name='useIndefLengthStrings'>A value indicating whether to
    /// always encode strings with a definite-length encoding.</param>
    /// <param name='allowDuplicateKeys'>A value indicating whether to
    /// disallow duplicate keys when reading CBOR objects from a data
    /// stream.</param>
    [Obsolete("Use the more readable string constructor instead.")]
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
    [Obsolete("Use the more readable string constructor instead.")]
    public CBOREncodeOptions(
      bool useIndefLengthStrings,
      bool allowDuplicateKeys,
      bool ctap2Canonical) {
      this.ResolveReferences = false;
      this.AllowEmpty = false;
      this.Float64 = false;
      this.KeepKeyOrder = false;
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
    /// any one of the following where the letters can be any combination
    /// of basic upper-case and/or basic lower-case letters:
    /// <c>allowduplicatekeys</c>, <c>ctap2canonical</c>,
    /// <c>resolvereferences</c>, <c>useindeflengthstrings</c>,
    /// <c>allowempty</c>, <c>float64</c>, <c>keepkeyorder</c>. Keys
    /// other than these are ignored in this version of the CBOR library.
    /// The key <c>float64</c> was introduced in version 4.4 of this
    /// library. The key <c>keepkeyorder</c> was introduced in version 4.5
    /// of this library.(Keys are compared using a basic case-insensitive
    /// comparison, in which two strings are equal if they match after
    /// converting the basic upper-case letters A to Z (U+0041 to U+005A)
    /// in both strings to basic lower-case letters.) If two or more
    /// key/value pairs have equal keys (in a basic case-insensitive
    /// comparison), the value given for the last such key is used. The
    /// four keys just given can have a value of <c>1</c>, <c>true</c>,
    /// <c>yes</c>, or <c>on</c> (where the letters can be any combination
    /// of basic upper-case and/or basic lower-case letters), which means
    /// true, and any other value meaning false. For example,
    /// <c>allowduplicatekeys=Yes</c> and <c>allowduplicatekeys=1</c> both
    /// set the <c>AllowDuplicateKeys</c> property to true. In the future,
    /// this class may allow other keys to store other kinds of values, not
    /// just true or false.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='paramString'/> is null.</exception>
    public CBOREncodeOptions(string paramString) {
      if (paramString == null) {
        throw new ArgumentNullException(nameof(paramString));
      }
      var parser = new OptionsParser(paramString);
      this.ResolveReferences = parser.GetBoolean("resolvereferences",
          false);
      this.UseIndefLengthStrings = parser.GetBoolean(
        "useindeflengthstrings",
        false);
      this.Float64 = parser.GetBoolean(
        "float64",
        false);
      this.AllowDuplicateKeys = parser.GetBoolean("allowduplicatekeys",
          false);
      this.KeepKeyOrder = parser.GetBoolean("keepkeyorder",
          false);
      this.AllowEmpty = parser.GetBoolean("allowempty", false);
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
        .Append(";float64=").Append(this.Float64 ? "true" : "false")
        .Append(";ctap2canonical=")
        .Append(this.Ctap2Canonical ? "true" : "false")
        .Append(";keepkeyorder=").Append(this.KeepKeyOrder ? "true" : "false")
        .Append(";resolvereferences=")
        .Append(this.ResolveReferences ? "true" : "false")
        .Append(";allowempty=").Append(this.AllowEmpty ? "true" : "false")
        .ToString();
    }

    /// <summary>Gets a value indicating whether to resolve references to
    /// sharable objects and sharable strings in the process of decoding a
    /// CBOR object. Enabling this property, however, can cause a security
    /// risk if a decoded CBOR object is then re-encoded.</summary>
    /// <value>A value indicating whether to resolve references to sharable
    /// objects and sharable strings. The default is false.</value>
    /// <remarks>
    /// <para><b>About sharable objects and references</b></para>
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
    /// to mark objects within those keys or values.</para>
    /// <para><b>Security Note</b></para>
    /// <para>When this property is enabled and a decoded CBOR object
    /// contains references to sharable CBOR objects within it, those
    /// references will be replaced with the sharable objects they refer to
    /// (but without making a copy of those objects). However, if shared
    /// references are deeply nested and used multiple times, these
    /// references can result in a CBOR object that is orders of magnitude
    /// bigger than if shared references weren't resolved, and this can
    /// cause a denial of service when the decoded CBOR object is then
    /// serialized (e.g., with <c>EncodeToBytes()</c>, <c>ToString()</c>,
    /// <c>ToJSONString()</c>, or <c>WriteTo</c> ), because object
    /// references are expanded in the process.</para>
    /// <para>For example, the following object in CBOR diagnostic
    /// notation, <c>[28(["xxx", "yyy"]), 28([29(0), 29(0), 29(0)]),
    /// 28([29(1), 29(1)]), 28([29(2), 29(2)]), 28([29(3), 29(3)]),
    /// 28([29(4), 29(4)]), 28([29(5), 29(5)])]</c>, expands to a CBOR
    /// object with a serialized size of about 1831 bytes when this
    /// property is enabled, as opposed to about 69 bytes when this
    /// property is disabled.</para>
    /// <para>One way to mitigate security issues with this property is to
    /// limit the maximum supported size a CBORObject can have once
    /// serialized to CBOR or JSON. This can be done by passing a so-called
    /// "limited memory stream" to the <c>WriteTo</c> or <c>WriteJSONTo</c>
    /// methods when serializing the object to JSON or CBOR. A "limited
    /// memory stream" is a <c>Stream</c> (or <c>OutputStream</c> in Java)
    /// that throws an exception if it would write more bytes than a given
    /// maximum size or would seek past that size. (See the documentation
    /// for <c>CBORObject.WriteTo</c> or <c>CBORObject.WriteJSONTo</c> for
    /// example code.) Another mitigation is to check the CBOR object's
    /// type before serializing it, since only arrays and maps can have the
    /// security problem described here, or to check the maximum nesting
    /// depth of a CBOR array or map before serializing
    /// it.</para></remarks>
    public bool ResolveReferences
    {
      get;
      private set;
    }

    /// <summary>Gets a value indicating whether to encode strings with an
    /// indefinite-length encoding under certain circumstances.</summary>
    /// <value>A value indicating whether to encode strings with an
    /// indefinite-length encoding under certain circumstances. The default
    /// is false.</value>
    public bool UseIndefLengthStrings
    {
      get;
      private set;
    }

    /// <summary>Gets a value indicating whether to preserve the order in
    /// which a CBOR map's keys appear when decoding a CBOR object, by
    /// using maps created as though by CBORObject.NewOrderedMap. If false,
    /// key order is not guaranteed to be preserved when decoding
    /// CBOR.</summary>
    /// <value>A value indicating whether to preserve the order in which a
    /// CBOR map's keys appear when decoding a CBOR object. The default is
    /// false.</value>
    public bool KeepKeyOrder
    {
      get;
      private set;
    }

    /// <summary>Gets a value indicating whether decoding a CBOR object
    /// will return <c>null</c> instead of a CBOR object if the stream has
    /// no content or the end of the stream is reached before decoding
    /// begins. Used only when decoding CBOR objects.</summary>
    /// <value>A value indicating whether decoding a CBOR object will
    /// return <c>null</c> instead of a CBOR object if the stream has no
    /// content or the end of the stream is reached before decoding begins.
    /// The default is false.</value>
    public bool AllowEmpty
    {
      get;
      private set;
    }

    /// <summary>Gets a value indicating whether to allow duplicate keys
    /// when reading CBOR objects from a data stream. Used only when
    /// decoding CBOR objects. If this property is <c>true</c> and a CBOR
    /// map has two or more values with the same key, the last value of
    /// that key set forth in the CBOR map is taken.</summary>
    /// <value>A value indicating whether to allow duplicate keys when
    /// reading CBOR objects from a data stream. The default is
    /// false.</value>
    public bool AllowDuplicateKeys
    {
      get;
      private set;
    }

    /// <summary>Gets a value indicating whether to encode floating-point
    /// numbers in a CBOR object in their 64-bit encoding form regardless
    /// of whether their value can be encoded without loss in a smaller
    /// form. Used only when encoding CBOR objects.</summary>
    /// <value>Gets a value indicating whether to encode floating-point
    /// numbers in a CBOR object in their 64-bit encoding form regardless
    /// of whether their value can be encoded without loss in a smaller
    /// form. Used only when encoding CBOR objects. The default is
    /// false.</value>
    public bool Float64
    {
      get;
      private set;
    }

    /// <summary>Gets a value indicating whether CBOR objects:
    /// <list>
    /// <item>When encoding, are written out using the CTAP2 canonical CBOR
    /// encoding form, which is useful for implementing Web Authentication
    /// (WebAuthn).</item>
    /// <item>When decoding, are checked for compliance with the CTAP2
    /// canonical encoding form.</item></list> In this form, CBOR tags are
    /// not used, map keys are written out in a canonical order, a maximum
    /// depth of four levels of arrays and/or maps is allowed, duplicate
    /// map keys are not allowed when decoding, and floating-point numbers
    /// are written out in their 64-bit encoding form regardless of whether
    /// their value can be encoded without loss in a smaller form. This
    /// implementation allows CBOR objects whose canonical form exceeds
    /// 1024 bytes, the default maximum size for CBOR objects in that form
    /// according to the FIDO Client-to-Authenticator Protocol 2
    /// specification.</summary>
    /// <value><c>true</c> if CBOR objects are written out using the CTAP2
    /// canonical CBOR encoding form; otherwise, <c>false</c>. The default
    /// is <c>false</c>.</value>
    public bool Ctap2Canonical
    {
      get;
      private set;
    }
  }
}
