## PeterO.Cbor.CBOREncodeOptions

    public sealed class CBOREncodeOptions

Specifies options for encoding and decoding CBOR objects.

### CBOREncodeOptions Constructor

    public CBOREncodeOptions(
        bool useIndefLengthStrings,
        bool allowDuplicateKeys);

Initializes a new instance of the [PeterO.Cbor.CBOREncodeOptions](PeterO.Cbor.CBOREncodeOptions.md) class.

<b>Parameters:</b>

 * <i>useIndefLengthStrings</i>: A value indicating whether to always encode strings with a efinite-length encoding.

 * <i>allowDuplicateKeys</i>: A value indicating whether to disallow duplicate keys when reading CBOR bjects from a data stream.

### Default

    public static readonly PeterO.Cbor.CBOREncodeOptions Default;

Default options for CBOR objects. Disallow duplicate keys, and always ncode strings using definite-length encoding. These are recommended ettings for the options that may be adopted by certain CBORObject methods n the next major version.

### NoDuplicateKeys

    public static readonly PeterO.Cbor.CBOREncodeOptions NoDuplicateKeys;

Disallow duplicate keys when reading CBOR objects from a data stream. sed only when decoding CBOR objects. Value: 2.

### NoIndefLengthStrings

    public static readonly PeterO.Cbor.CBOREncodeOptions NoIndefLengthStrings;

Always encode strings with a definite-length encoding. Used only when ncoding CBOR objects. Value: 1.

### None

    public static readonly PeterO.Cbor.CBOREncodeOptions None;

No special options for encoding/decoding. Value: 0.

### AllowDuplicateKeys

    public bool AllowDuplicateKeys { get; }

Gets a value indicating whether to disallow duplicate keys when reading BOR objects from a data stream. Used only when decoding CBOR objects.

<b>Returns:</b>

A value indicating whether to disallow duplicate keys when reading CBOR bjects from a data stream.

### UseIndefLengthStrings

    public bool UseIndefLengthStrings { get; }

Gets a value indicating whether to always encode strings with a efinite-length encoding.

<b>Returns:</b>

A value indicating whether to always encode strings with a efinite-length encoding.

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

Returns an options object containing the flags shared by this and another ptions object.

<b>Parameters:</b>

 * <i>o</i>: The parameter <i>o</i>
is a CBOREncodeOptions object.

<b>Return Value:</b>

A CBOREncodeOptions object.

### Or

    public PeterO.Cbor.CBOREncodeOptions Or(
        PeterO.Cbor.CBOREncodeOptions o);

<b>Deprecated.</b> May be removed in a later version. Option classes in this library will follow the form seen in JSONOptions in a later version; the approach used in this class is too complicated.

Returns an options object containing the combined flags of this and nother options object.

<b>Parameters:</b>

 * <i>o</i>: The parameter <i>o</i>
is a CBOREncodeOptions object.

<b>Return Value:</b>

A new CBOREncodeOptions object.
