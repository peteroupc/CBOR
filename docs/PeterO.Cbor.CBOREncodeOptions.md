## PeterO.Cbor.CBOREncodeOptions

    public sealed class CBOREncodeOptions

Specifies options for encoding and decoding CBOR objects.

### Member Summary
* <code>[AllowDuplicateKeys](#AllowDuplicateKeys)</code> - Gets a value indicating whether to allow duplicate keys when reading CBOR objects from a data stream.
* <code>[AllowEmpty](#AllowEmpty)</code> - Gets a value indicating whether decoding a CBOR object will return null instead of a CBOR object if the stream has no content or the end of the stream is reached before decoding begins.
* <code>[Ctap2Canonical](#Ctap2Canonical)</code> - Gets a value indicating whether CBOR objects: When encoding, are written out using the CTAP2 canonical CBOR encoding form, which is useful for implementing Web Authentication (WebAuthn).
* <code>[public static readonly PeterO.Cbor.CBOREncodeOptions Default;](#Default)</code> - Default options for CBOR objects.
* <code>[public static readonly PeterO.Cbor.CBOREncodeOptions DefaultCtap2Canonical;](#DefaultCtap2Canonical)</code> - Default options for CBOR objects serialized using the CTAP2 canonicalization (used in Web Authentication, among other specifications).
* <code>[Float64](#Float64)</code> - Gets a value indicating whether to encode floating-point numbers in a CBOR object in their 64-bit encoding form regardless of whether their value can be encoded without loss in a smaller form.
* <code>[KeepKeyOrder](#KeepKeyOrder)</code> - Gets a value indicating whether to preserve the order in which a CBOR map's keys appear when decoding a CBOR object, by using maps created as though by CBORObject.
* <code>[ResolveReferences](#ResolveReferences)</code> - Gets a value indicating whether to resolve references to sharable objects and sharable strings in the process of decoding a CBOR object.
* <code>[ToString()](#ToString)</code> - Gets the values of this options object's properties in text form.
* <code>[UseIndefLengthStrings](#UseIndefLengthStrings)</code> - Gets a value indicating whether to encode strings with an indefinite-length encoding under certain circumstances.

<a id="Void_ctor_Boolean_Boolean"></a>
### CBOREncodeOptions Constructor

    public CBOREncodeOptions(
        bool useIndefLengthStrings,
        bool allowDuplicateKeys);

<b>Deprecated.</b> Use the more readable string constructor instead.

Initializes a new instance of the [PeterO.Cbor.CBOREncodeOptions](PeterO.Cbor.CBOREncodeOptions.md) class.

<b>Parameters:</b>

 * <i>useIndefLengthStrings</i>: A value indicating whether to always encode strings with a definite-length encoding.

 * <i>allowDuplicateKeys</i>: A value indicating whether to disallow duplicate keys when reading CBOR objects from a data stream.

<a id="Void_ctor_Boolean_Boolean_Boolean"></a>
### CBOREncodeOptions Constructor

    public CBOREncodeOptions(
        bool useIndefLengthStrings,
        bool allowDuplicateKeys,
        bool ctap2Canonical);

<b>Deprecated.</b> Use the more readable string constructor instead.

Initializes a new instance of the [PeterO.Cbor.CBOREncodeOptions](PeterO.Cbor.CBOREncodeOptions.md) class.

<b>Parameters:</b>

 * <i>useIndefLengthStrings</i>: A value indicating whether to encode strings with a definite-length encoding in certain cases.

 * <i>allowDuplicateKeys</i>: A value indicating whether to allow duplicate keys when reading CBOR objects from a data stream.

 * <i>ctap2Canonical</i>: A value indicating whether CBOR objects are written out using the CTAP2 canonical CBOR encoding form, which is useful for implementing Web Authentication.

<a id="Void_ctor_System_String"></a>
### CBOREncodeOptions Constructor

    public CBOREncodeOptions(
        string paramString);

Initializes a new instance of the [PeterO.Cbor.CBOREncodeOptions](PeterO.Cbor.CBOREncodeOptions.md) class.

<b>Parameters:</b>

 * <i>paramString</i>: A string setting forth the options to use. This is a semicolon-separated list of options, each of which has a key and a value separated by an equal sign ("="). Whitespace and line separators are not allowed to appear between the semicolons or between the equal signs, nor may the string begin or end with whitespace. The string can be empty, but cannot be null. The following is an example of this parameter:  `allowduplicatekeys=true;ctap2Canonical=true` . The key can be any one of the following where the letters can be any combination of basic upper-case and/or basic lower-case letters:  `allowduplicatekeys` ,  `ctap2canonical` ,  `resolvereferences` ,  `useindeflengthstrings` ,  `allowempty` ,  `float64` ,  `keepkeyorder` . Keys other than these are ignored in this version of the CBOR library. The key  `float64`  was introduced in version 4.4 of this library. The key  `keepkeyorder`  was introduced in version 4.5 of this library.(Keys are compared using a basic case-insensitive comparison, in which two strings are equal if they match after converting the basic upper-case letters A to Z (U+0041 to U+005A) in both strings to basic lower-case letters.) If two or more key/value pairs have equal keys (in a basic case-insensitive comparison), the value given for the last such key is used. The four keys just given can have a value of  `1` ,  `true` ,  `yes` , or  `on`  (where the letters can be any combination of basic upper-case and/or basic lower-case letters), which means true, and any other value meaning false. For example,  `allowduplicatekeys=Yes`  and  `allowduplicatekeys=1`  both set the  `AllowDuplicateKeys`  property to true. In the future, this class may allow other keys to store other kinds of values, not just true or false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>paramString</i>
 is null.

<a id="Void_ctor"></a>
### CBOREncodeOptions Constructor

    public CBOREncodeOptions();

Initializes a new instance of the [PeterO.Cbor.CBOREncodeOptions](PeterO.Cbor.CBOREncodeOptions.md) class with all the default options.

<a id="Default"></a>
### Default

    public static readonly PeterO.Cbor.CBOREncodeOptions Default;

Default options for CBOR objects. Disallow duplicate keys, and always encode strings using definite-length encoding.

<a id="DefaultCtap2Canonical"></a>
### DefaultCtap2Canonical

    public static readonly PeterO.Cbor.CBOREncodeOptions DefaultCtap2Canonical;

Default options for CBOR objects serialized using the CTAP2 canonicalization (used in Web Authentication, among other specifications). Disallow duplicate keys, and always encode strings using definite-length encoding.

<a id="AllowDuplicateKeys"></a>
### AllowDuplicateKeys

    public bool AllowDuplicateKeys { get; }

Gets a value indicating whether to allow duplicate keys when reading CBOR objects from a data stream. Used only when decoding CBOR objects. If this property is  `true`  and a CBOR map has two or more values with the same key, the last value of that key set forth in the CBOR map is taken.

<b>Returns:</b>

A value indicating whether to allow duplicate keys when reading CBOR objects from a data stream. The default is false.

<a id="AllowEmpty"></a>
### AllowEmpty

    public bool AllowEmpty { get; }

Gets a value indicating whether decoding a CBOR object will return  `null`  instead of a CBOR object if the stream has no content or the end of the stream is reached before decoding begins. Used only when decoding CBOR objects.

<b>Returns:</b>

A value indicating whether decoding a CBOR object will return  `null`  instead of a CBOR object if the stream has no content or the end of the stream is reached before decoding begins. The default is false.

<a id="Ctap2Canonical"></a>
### Ctap2Canonical

    public bool Ctap2Canonical { get; }

Gets a value indicating whether CBOR objects:

 * When encoding, are written out using the CTAP2 canonical CBOR encoding form, which is useful for implementing Web Authentication (WebAuthn).

 * When decoding, are checked for compliance with the CTAP2 canonical encoding form.

 In this form, CBOR tags are not used, map keys are written out in a canonical order, a maximum depth of four levels of arrays and/or maps is allowed, duplicate map keys are not allowed when decoding, and floating-point numbers are written out in their 64-bit encoding form regardless of whether their value can be encoded without loss in a smaller form. This implementation allows CBOR objects whose canonical form exceeds 1024 bytes, the default maximum size for CBOR objects in that form according to the FIDO Client-to-Authenticator Protocol 2 specification.

<b>Returns:</b>

 `true`  if CBOR objects are written out using the CTAP2 canonical CBOR encoding form; otherwise,  `false` . The default is  `false` .

<a id="Float64"></a>
### Float64

    public bool Float64 { get; }

Gets a value indicating whether to encode floating-point numbers in a CBOR object in their 64-bit encoding form regardless of whether their value can be encoded without loss in a smaller form. Used only when encoding CBOR objects.

<b>Returns:</b>

Gets a value indicating whether to encode floating-point numbers in a CBOR object in their 64-bit encoding form regardless of whether their value can be encoded without loss in a smaller form. Used only when encoding CBOR objects. The default is false.

<a id="KeepKeyOrder"></a>
### KeepKeyOrder

    public bool KeepKeyOrder { get; }

Gets a value indicating whether to preserve the order in which a CBOR map's keys appear when decoding a CBOR object, by using maps created as though by CBORObject.NewOrderedMap. If false, key order is not guaranteed to be preserved when decoding CBOR.

<b>Returns:</b>

A value indicating whether to preserve the order in which a CBOR map's keys appear when decoding a CBOR object. The default is false.

<a id="ResolveReferences"></a>
### ResolveReferences

    public bool ResolveReferences { get; }

Gets a value indicating whether to resolve references to sharable objects and sharable strings in the process of decoding a CBOR object. Enabling this property, however, can cause a security risk if a decoded CBOR object is then re-encoded.

<b>About sharable objects and references</b>

Sharable objects are marked with tag 28, and references to those objects are marked with tag 29 (where a reference of 0 means the first sharable object in the CBOR stream, a reference of 1 means the second, and so on). Sharable strings (byte strings and text strings) appear within an enclosing object marked with tag 256, and references to them are marked with tag 25; in general, a string is sharable only if storing its reference rather than the string would save space.

Note that unlike most other tags, these tags generally care about the relative order in which objects appear in a CBOR stream; thus they are not interoperable with CBOR implementations that follow the generic CBOR data model (since they may list map keys in an unspecified order). Interoperability problems with these tags can be reduced by not using them to mark keys or values of a map or to mark objects within those keys or values.

<b>Security Note</b>

When this property is enabled and a decoded CBOR object contains references to sharable CBOR objects within it, those references will be replaced with the sharable objects they refer to (but without making a copy of those objects). However, if shared references are deeply nested and used multiple times, these references can result in a CBOR object that is orders of magnitude bigger than if shared references weren't resolved, and this can cause a denial of service when the decoded CBOR object is then serialized (e.g., with  `EncodeToBytes()` ,  `ToString()` ,  `ToJSONString()` , or  `WriteTo`  ), because object references are expanded in the process.

For example, the following object in CBOR diagnostic notation,  `[28(["xxx", "yyy"]), 28([29(0), 29(0), 29(0)]),
            28([29(1), 29(1)]), 28([29(2), 29(2)]), 28([29(3), 29(3)]),
            28([29(4), 29(4)]), 28([29(5), 29(5)])]` , expands to a CBOR object with a serialized size of about 1831 bytes when this property is enabled, as opposed to about 69 bytes when this property is disabled.

One way to mitigate security issues with this property is to limit the maximum supported size a CBORObject can have once serialized to CBOR or JSON. This can be done by passing a so-called "limited memory stream" to the  `WriteTo`  or  `WriteJSONTo`  methods when serializing the object to JSON or CBOR. A "limited memory stream" is a  `Stream`  (or  `OutputStream`  in Java) that throws an exception if it would write more bytes than a given maximum size or would seek past that size. (See the documentation for  `CBORObject.WriteTo`  or  `CBORObject.WriteJSONTo`  for example code.) Another mitigation is to check the CBOR object's type before serializing it, since only arrays and maps can have the security problem described here, or to check the maximum nesting depth of a CBOR array or map before serializing it.

<b>Returns:</b>

A value indicating whether to resolve references to sharable objects and sharable strings. The default is false.

<a id="UseIndefLengthStrings"></a>
### UseIndefLengthStrings

    public bool UseIndefLengthStrings { get; }

Gets a value indicating whether to encode strings with an indefinite-length encoding under certain circumstances.

<b>Returns:</b>

A value indicating whether to encode strings with an indefinite-length encoding under certain circumstances. The default is false.

<a id="ToString"></a>
### ToString

    public override string ToString();

Gets the values of this options object's properties in text form.

<b>Return Value:</b>

A text string containing the values of this options object's properties. The format of the string is the same as the one described in the String constructor for this class.
