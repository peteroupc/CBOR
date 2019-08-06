## PeterO.Cbor.CBOREncodeOptions

    public sealed class CBOREncodeOptions

 Specifies options for encoding and decoding CBOR objects.

### Member Summary
* <code>[AllowDuplicateKeys](#AllowDuplicateKeys)</code> - Gets a value indicating whether to disallow duplicate keys when reading CBOR objects from a data stream.
* <code>[AllowEmpty](#AllowEmpty)</code> - Gets a value indicating whether decoding a CBOR object will return null instead of a CBOR object if the stream has no content or the end of the stream is reached before decoding begins.
* <code>[And(PeterO.Cbor.CBOREncodeOptions)](#And_PeterO_Cbor_CBOREncodeOptions)</code> - Returns an options object containing the flags shared by this and another options object.
* <code>[Ctap2Canonical](#Ctap2Canonical)</code> - Gets a value indicating whether CBOR objects are written out using the CTAP2 canonical CBOR encoding form.
* <code>[public static readonly PeterO.Cbor.CBOREncodeOptions Default;](#Default)</code> - Default options for CBOR objects.
* <code>[public static readonly PeterO.Cbor.CBOREncodeOptions NoDuplicateKeys;](#NoDuplicateKeys)</code> - Disallow duplicate keys when reading CBOR objects from a data stream.
* <code>[public static readonly PeterO.Cbor.CBOREncodeOptions NoIndefLengthStrings;](#NoIndefLengthStrings)</code> - Always encode strings with a definite-length encoding.
* <code>[public static readonly PeterO.Cbor.CBOREncodeOptions None;](#None)</code> - No special options for encoding/decoding.
* <code>[Or(PeterO.Cbor.CBOREncodeOptions)](#Or_PeterO_Cbor_CBOREncodeOptions)</code> - Returns an options object containing the combined flags of this and another options object.
* <code>[ResolveReferences](#ResolveReferences)</code> - Gets a value indicating whether to resolve references to sharable objects and sharable strings in the process of decoding a CBOR object.
* <code>[ToString()](#ToString)</code> - Gets the values of this options object's properties in text form.
* <code>[UseIndefLengthStrings](#UseIndefLengthStrings)</code> - Gets a value indicating whether to always encode strings with a definite-length encoding.
* <code>[Value](#Value)</code> - Gets this options object's value.

<a id="Void_ctor_Boolean_Boolean"></a>
### CBOREncodeOptions Constructor

    public CBOREncodeOptions(
        bool useIndefLengthStrings,
        bool allowDuplicateKeys);

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

 Initializes a new instance of the [PeterO.Cbor.CBOREncodeOptions](PeterO.Cbor.CBOREncodeOptions.md) class.

     <b>Parameters:</b>

 * <i>useIndefLengthStrings</i>: A value indicating whether to always encode strings with a definite-length encoding.

 * <i>allowDuplicateKeys</i>: A value indicating whether to disallow duplicate keys when reading CBOR objects from a data stream.

 * <i>ctap2Canonical</i>: A value indicating whether to encode CBOR objects in the CTAP2 canonical encoding form.

<a id="Void_ctor_System_String"></a>
### CBOREncodeOptions Constructor

    public CBOREncodeOptions(
        string paramString);

 Initializes a new instance of the [PeterO.Cbor.CBOREncodeOptions](PeterO.Cbor.CBOREncodeOptions.md) class.

    <b>Parameters:</b>

 * <i>paramString</i>: A string setting forth the options to use. This is a semicolon-separated list of options, each of which has a key and a value separated by an equal sign ("="). Whitespace and line separators are not allowed to appear between the semicolons or between the equal signs, nor may the string begin or end with whitespace. The string can be empty, but cannot be null. The following is an example of this parameter:  `allowduplicatekeys=true;ctap2Canonical=true` . The key can be any one of the following in any combination of case:  `allowduplicatekeys` ,  `ctap2canonical` ,  `resolvereferences` ,  `useindeflengthstrings` ,  `allowempty` . Keys other than these are ignored. If the same key appears more than once, the value given for the last such key is used. The four keys just given can have a value of  `1` ,  `true` ,  `yes` , or  `on`  (in any combination of case), which means true, and any other value meaning false. For example,  `allowduplicatekeys=Yes`  and  `allowduplicatekeys=1`  both set the  `AllowDuplicateKeys`  property to true.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>paramString</i>
 is null.

<a id="Void_ctor"></a>
### CBOREncodeOptions Constructor

    public CBOREncodeOptions();

 Initializes a new instance of the [PeterO.Cbor.CBOREncodeOptions](PeterO.Cbor.CBOREncodeOptions.md) class.

  <a id="Default"></a>
### Default

    public static readonly PeterO.Cbor.CBOREncodeOptions Default;

 Default options for CBOR objects. Disallow duplicate keys, and always encode strings using definite-length encoding. These are recommended settings for the options that may be adopted by certain CBORObject methods in the next major version.

  <a id="NoDuplicateKeys"></a>
### NoDuplicateKeys

    public static readonly PeterO.Cbor.CBOREncodeOptions NoDuplicateKeys;

 Disallow duplicate keys when reading CBOR objects from a data stream. Used only when decoding CBOR objects. Value: 2.

  <a id="NoIndefLengthStrings"></a>
### NoIndefLengthStrings

    public static readonly PeterO.Cbor.CBOREncodeOptions NoIndefLengthStrings;

 Always encode strings with a definite-length encoding. Used only when encoding CBOR objects. Value: 1.

  <a id="None"></a>
### None

    public static readonly PeterO.Cbor.CBOREncodeOptions None;

 No special options for encoding/decoding. Value: 0.

  <a id="AllowDuplicateKeys"></a>
### AllowDuplicateKeys

    public bool AllowDuplicateKeys { get; }

 Gets a value indicating whether to disallow duplicate keys when reading CBOR objects from a data stream. Used only when decoding CBOR objects.

 <b>Returns:</b>

A value indicating whether to disallow duplicate keys when reading CBOR objects from a data stream.

<a id="AllowEmpty"></a>
### AllowEmpty

    public bool AllowEmpty { get; }

 Gets a value indicating whether decoding a CBOR object will return  `null`  instead of a CBOR object if the stream has no content or the end of the stream is reached before decoding begins. Used only when decoding CBOR objects.

 <b>Returns:</b>

A value indicating whether decoding a CBOR object will return  `null`  instead of a CBOR object if the stream has no content or the end of the stream is reached before decoding begins. The default is false.

<a id="Ctap2Canonical"></a>
### Ctap2Canonical

    public bool Ctap2Canonical { get; }

 Gets a value indicating whether CBOR objects are written out using the CTAP2 canonical CBOR encoding form. In this form, CBOR tags are not used, map keys are written out in a canonical order, and non-integer numbers and integers 2^63 or greater are written as 64-bit binary floating-point numbers.

 <b>Returns:</b>

 `true`  if CBOR objects are written out using the CTAP2 canonical CBOR encoding form; otherwise,  `false`  .. In this form, CBOR tags are not used, map keys are written out in a canonical order, and non-integer numbers and integers 2^63 or greater are written as 64-bit binary floating-point numbers.

<a id="ResolveReferences"></a>
### ResolveReferences

    public bool ResolveReferences { get; }

 Gets a value indicating whether to resolve references to sharable objects and sharable strings in the process of decoding a CBOR object.

 Sharable objects are marked with tag 28, and references to those objects are marked with tag 29 (where a reference of 0 means the first sharable object in the CBOR stream, a reference of 1 means the second, and so on). Sharable strings (byte strings and text strings) appear within an enclosing object marked with tag 256, and references to them are marked with tag 25; in general, a string is sharable only if storing its reference rather than the string would save space.

 Note that unlike most other tags, these tags generally care about the relative order in which objects appear in a CBOR stream; thus they are not interoperable with CBOR implementations that follow the generic CBOR data model (since they may list map keys in an unspecified order). Interoperability problems with these tags can be reduced by not using them to mark keys or values of a map or to mark objects within those keys or values.

 <b>Returns:</b>

A value indicating whether to resolve references to sharable objects and sharable strings. The default is false.

<a id="UseIndefLengthStrings"></a>
### UseIndefLengthStrings

    public bool UseIndefLengthStrings { get; }

 Gets a value indicating whether to always encode strings with a definite-length encoding.

 <b>Returns:</b>

A value indicating whether to always encode strings with a definite-length encoding.

<a id="Value"></a>
### Value

    public int Value { get; }

<b>Deprecated.</b> Use the ToString() method to get the values of all this object's properties.

 Gets this options object's value.

 The return value is 1 if UseIndefLengthStrings is false or 0 if true, plus 2 if AllowDuplicateKeys is false or 0 if true.

 <b>Returns:</b>

This options object's value.

<a id="And_PeterO_Cbor_CBOREncodeOptions"></a>
### And

    public PeterO.Cbor.CBOREncodeOptions And(
        PeterO.Cbor.CBOREncodeOptions o);

<b>Deprecated.</b> May be removed in a later version. Option classes in this library will follow a different form in a later version -- the approach used in this class is too complicated.

 Returns an options object containing the flags shared by this and another options object.

 For compatibility reasons, for the object returned by this method, the UseIndefLengthStrings property will be set to true if that property is true in <i> either or both </i> objects, or false otherwise; the AllowDuplicateKeys property will be set to true if that property is true in <i> either or both </i> objects, or false otherwise; and other properties will have their default values. (This class formerly used the inverse of both properties and And works off of that.)

 <b>Parameters:</b>

 * <i>o</i>: The parameter  <i>o</i>
 is a CBOREncodeOptions object.

<b>Return Value:</b>

A CBOREncodeOptions object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>o</i>
 is null.

<a id="Or_PeterO_Cbor_CBOREncodeOptions"></a>
### Or

    public PeterO.Cbor.CBOREncodeOptions Or(
        PeterO.Cbor.CBOREncodeOptions o);

<b>Deprecated.</b> May be removed in a later version. Option classes in this library will follow a different form in a later version -- the approach used in this class is too complicated.

 Returns an options object containing the combined flags of this and another options object.

 For compatibility reasons, for the object returned by this method, the UseIndefLengthStrings property will be set to true if that property is true in <i> both </i> objects, or false otherwise; the AllowDuplicateKeys property will be set to true if that property is true in <i> both </i> objects, or false otherwise; and other properties will have their default values. (This class formerly used the inverse of both properties and Or works off of that.)

 <b>Parameters:</b>

 * <i>o</i>: The parameter  <i>o</i>
 is a CBOREncodeOptions object.

<b>Return Value:</b>

A new CBOREncodeOptions object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>o</i>
 is null.

<a id="ToString"></a>
### ToString

    public override string ToString();

 Gets the values of this options object's properties in text form.

 <b>Return Value:</b>

A text string containing the values of this options object's properties. The format of the string is the same as the one described in the String constructor for this class.
