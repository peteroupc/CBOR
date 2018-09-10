## PeterO.Cbor.CBOREncodeOptions

    public sealed class CBOREncodeOptions

Specifies options for encoding and decoding CBOR objects.

### CBOREncodeOptions Constructor

    public CBOREncodeOptions(
        bool useIndefLengthStrings,
        bool allowDuplicateKeys);

Initializes a new instance of the [PeterO.Cbor.CBOREncodeOptions](PeterO.Cbor.CBOREncodeOptions.md) class.

<b>Parameters:</b>

 * <i>useIndefLengthStrings</i>: A value indicating whether to always encode strings with a definite-length encoding.

 * <i>allowDuplicateKeys</i>: A value indicating whether to disallow duplicate keys when reading CBOR objects from a data stream.

### CBOREncodeOptions Constructor

    public CBOREncodeOptions(
        bool useIndefLengthStrings,
        bool allowDuplicateKeys,
        bool ctap2Canonical);

Initializes a new instance of the [PeterO.Cbor.CBOREncodeOptions](PeterO.Cbor.CBOREncodeOptions.md) class.

<b>Parameters:</b>

 * <i>useIndefLengthStrings</i>: A value indicating whether to always encode strings with a definite-length encoding.

 * <i>allowDuplicateKeys</i>: A value indicating whether to disallow duplicate keys when reading CBOR objects from a data stream.

 * <i>ctap2Canonical</i>: Either  `true`  or  `false` .

### Default

    public static readonly PeterO.Cbor.CBOREncodeOptions Default;

Default options for CBOR objects. Disallow duplicate keys, and always encode strings using definite-length encoding. These are recommended settings for the options that may be adopted by certain CBORObject methods in the next major version.

### NoDuplicateKeys

    public static readonly PeterO.Cbor.CBOREncodeOptions NoDuplicateKeys;

Disallow duplicate keys when reading CBOR objects from a data stream. Used only when decoding CBOR objects. Value: 2.

### NoIndefLengthStrings

    public static readonly PeterO.Cbor.CBOREncodeOptions NoIndefLengthStrings;

Always encode strings with a definite-length encoding. Used only when encoding CBOR objects. Value: 1.

### None

    public static readonly PeterO.Cbor.CBOREncodeOptions None;

No special options for encoding/decoding. Value: 0.

### AllowDuplicateKeys

    public bool AllowDuplicateKeys { get; }

Gets a value indicating whether to disallow duplicate keys when reading CBOR objects from a data stream. Used only when decoding CBOR objects.

<b>Returns:</b>

A value indicating whether to disallow duplicate keys when reading CBOR objects from a data stream.

### Ctap2Canonical

    public bool Ctap2Canonical { get; }

Gets a value indicating whether CBOR objects are written out using the CTAP2 canonical CBOR encoding form. In this form, CBOR tags are not used, map keys are written out in a canonical order, and non-integer numbers and integers 2^63 or greater are written as 64-bit binary floating-point numbers.

<b>Returns:</b>

 `true`  if CBOR objects are written out using the CTAP2 canonical CBOR encoding form; otherwise,  `false` .. In this form, CBOR tags are not used, map keys are written out in a canonical order, and non-integer numbers and integers 2^63 or greater are written as 64-bit binary floating-point numbers.

### UseIndefLengthStrings

    public bool UseIndefLengthStrings { get; }

Gets a value indicating whether to always encode strings with a definite-length encoding.

<b>Returns:</b>

A value indicating whether to always encode strings with a definite-length encoding.

### Value

    public int Value { get; }

<b>Deprecated.</b> Option classes in this library will follow the form seen in JSONOptions in a later version; the approach used in this class is too complicated.

Gets this options object's value.

<b>Returns:</b>

This options object's value.

### And

    public PeterO.Cbor.CBOREncodeOptions And(
        PeterO.Cbor.CBOREncodeOptions o);

<b>Deprecated.</b> May be removed in a later version. Option classes in this library will follow the form seen in JSONOptions in a later version; the approach used in this class is too complicated.

Returns an options object containing the flags shared by this and another options object.

<b>Parameters:</b>

 * <i>o</i>: The parameter <i>o</i>
is a CBOREncodeOptions object.

<b>Return Value:</b>

A CBOREncodeOptions object.

### Or

    public PeterO.Cbor.CBOREncodeOptions Or(
        PeterO.Cbor.CBOREncodeOptions o);

<b>Deprecated.</b> May be removed in a later version. Option classes in this library will follow the form seen in JSONOptions in a later version; the approach used in this class is too complicated.

Returns an options object containing the combined flags of this and another options object.

<b>Parameters:</b>

 * <i>o</i>: The parameter <i>o</i>
is a CBOREncodeOptions object.

<b>Return Value:</b>

A new CBOREncodeOptions object.
