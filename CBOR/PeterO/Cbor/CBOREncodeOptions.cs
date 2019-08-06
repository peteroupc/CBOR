using System;

namespace PeterO.Cbor {
  /// <include file='../../docs.xml'
  /// path='docs/doc[@name="T:PeterO.Cbor.CBOREncodeOptions"]/*'/>
  public sealed class CBOREncodeOptions {
    /// <summary>No special options for encoding/decoding. Value:
    /// 0.</summary>
    [Obsolete("Use 'new CBOREncodeOptions(true,true)' instead. Option classes" +
" in this library will follow a different form in a later" +
" version -- the approach used in this class is too complicated. " +
"'CBOREncodeOptions.Default' contains recommended default options that" +
" may be adopted by certain CBORObject methods in the next major " +
"version.")]
    public static readonly CBOREncodeOptions None =
 new CBOREncodeOptions("useindeflengthstrings=1;allowduplicatekeys=1");

    /// <summary>Default options for CBOR objects. Disallow duplicate keys,
    /// and always encode strings using definite-length encoding. These are
    /// recommended settings for the options that may be adopted by certain
    /// CBORObject methods in the next major version.</summary>
    public static readonly CBOREncodeOptions Default =
      new CBOREncodeOptions(false, false);

    /// <summary>Always encode strings with a definite-length encoding.
    /// Used only when encoding CBOR objects. Value: 1.</summary>
    [Obsolete("Use 'new CBOREncodeOptions(false,false)' instead. Option" +
" classes" +
"\u0020in this library will follow a different form in a later" +
"\u0020version -- the approach used in this class is too complicated.")]
    public static readonly CBOREncodeOptions NoIndefLengthStrings =
new CBOREncodeOptions("useindeflengthstrings=0;allowduplicatekeys=1");

    /// <summary>Disallow duplicate keys when reading CBOR objects from a
    /// data stream. Used only when decoding CBOR objects. Value:
    /// 2.</summary>
    [Obsolete("Use 'new CBOREncodeOptions(true,false)' instead. Option" +
" classes" +
"\u0020in this library will follow a different form in a later" +
"\u0020version -- the approach used in this class is too complicated.")]
    public static readonly CBOREncodeOptions NoDuplicateKeys =
 new CBOREncodeOptions("useindeflengthstrings=1;allowduplicatekeys=0");

    private readonly int value;

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
    /// always encode strings with a definite-length encoding.</param>
    /// <param name='allowDuplicateKeys'>A value indicating whether to
    /// disallow duplicate keys when reading CBOR objects from a data
    /// stream.</param>
    /// <param name='ctap2Canonical'>A value indicating whether to encode CBOR objects in the CTAP2 canonical encoding form.</param>
    public CBOREncodeOptions(
      bool useIndefLengthStrings,
      bool allowDuplicateKeys,
      bool ctap2Canonical) : this(BuildString(useIndefLengthStrings, allowDuplicateKeys, ctap2Canonical)){
    }

    private static string BuildString(bool useIndefLengthStrings,
      bool allowDuplicateKeys,
      bool ctap2Canonical){
      return "useindeflengthstrings="+(useIndefLengthStrings ? "1" : "0")+
         ";allowduplicatekeys="+(useIndefLengthStrings ? "1" : "0")+
         ";ctap2canonical="+(useIndefLengthStrings ? "1" : "0");
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
      this.UseIndefLengthStrings =
parser.GetBoolean("useindeflengthstrings", true);
      this.AllowDuplicateKeys = parser.GetBoolean("allowduplicatekeys", true);
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
           .Append(";ctap2canonical=")
           .Append(this.Ctap2Canonical ? "true" : "false")
           .Append(";resolvereferences=")
           .Append(this.ResolveReferences ? "true" : "false")
           .Append(";allowempty=").Append(this.AllowEmpty ? "true" : "false")
           .ToString();
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.Value"]/*'/>
    /// <remarks>The return value is 1 if UseIndefLengthStrings is false or 0 if true,
    /// plus 2 if AllowDuplicateKeys is false or 0 if true.</remarks>
    [Obsolete("Use the ToString() method to get the values of " +
     "all this object's properties.")]
    public int Value {
      get {
        return (this.UseIndefLengthStrings ? 0 : 1)+(this.AllowDuplicateKeys ? 0 : 2);
      }
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

    /// <summary>Gets a value indicating whether decoding a CBOR object
    /// will return <c>null</c> instead of a CBOR object if the stream has
    /// no content or the end of the stream is reached before decoding
    /// begins. Used only when decoding CBOR objects.</summary>
    /// <value>A value indicating whether decoding a CBOR object will
    /// return <c>null</c> instead of a CBOR object if the stream has no
    /// content or the end of the stream is reached before decoding begins.
    /// The default is false.</value>
    public bool AllowEmpty { get; private set; }


    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBOREncodeOptions.Or(PeterO.Cbor.CBOREncodeOptions)"]/*'/>
     /// <remarks>For compatibility reasons, for the object returned by this method, the UseIndefLengthStrings property will be set to true
 /// if that property is true in <i>both</i> objects, the AllowDuplicateKeys property will
 /// be set to true if that property is true in <i>both</i> objects, and other properties
 /// will have their default values. (This class formerly used the inverse of both properties and
 /// Or works off of that.)</remarks>
    [Obsolete("May be removed in a later version. Option classes" +
"\u0020in this library will follow a different form in a later" +
"\u0020version -- the approach used in this class is too complicated.")]
    public CBOREncodeOptions Or(CBOREncodeOptions o) {
      if (o == null) {
        throw new ArgumentNullException(nameof(o));
      }
      string str = new System.Text.StringBuilder()
           .Append("allowduplicatekeys=")
           .Append((this.AllowDuplicateKeys && o.AllowDuplicateKeys) ? "true" : "false")
           .Append(";useindeflengthstrings=")
           .Append((this.UseIndefLengthStrings && o.UseIndefLengthStrings) ? "true" : "false")
           .ToString();
      return new CBOREncodeOptions(str);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBOREncodeOptions.And(PeterO.Cbor.CBOREncodeOptions)"]/*'/>
     /// <remarks>For compatibility reasons, for the object returned by this method, the UseIndefLengthStrings property will be set to true
 /// if that property is true in <i>either or both</i> objects, the AllowDuplicateKeys property will
 /// be set to true if that property is true in <i>either or both</i> objects, and other properties
 /// will have their default values. (This class formerly used the inverse of both properties and
 /// And works off of that.)</remarks>
    [Obsolete("May be removed in a later version. Option classes" +
"\u0020in this library will follow a different form in a later" +
"\u0020version -- the approach used in this class is too complicated.")]
    public CBOREncodeOptions And(CBOREncodeOptions o) {
      if (o == null) {
        throw new ArgumentNullException(nameof(o));
      }
      string str = new System.Text.StringBuilder()
           .Append("allowduplicatekeys=")
           .Append((this.AllowDuplicateKeys || o.AllowDuplicateKeys) ? "true" : "false")
           .Append(";useindeflengthstrings=")
           .Append((this.UseIndefLengthStrings || o.UseIndefLengthStrings) ? "true" : "false")
           .ToString();
      return new CBOREncodeOptions(str);
    }
  }
}
