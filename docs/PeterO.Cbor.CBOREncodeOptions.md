## PeterO.Cbor.CBOREncodeOptions

    public sealed class CBOREncodeOptions

Specifies options for encoding and decoding CBOR objects.

### CBOREncodeOptions Constructor

    public CBOREncodeOptions(
        bool useIndefLengthStrings,
        bool useDuplicateKeys);

Initializes a new instance of the CBOREncodeOptions class.

<b>Parameters:</b>

 * <i>useIndefLengthStrings</i>: A Boolean object.

 * <i>useDuplicateKeys</i>: Another Boolean object.

### NoDuplicateKeys

    public static readonly PeterO.Cbor.CBOREncodeOptions NoDuplicateKeys;

Disallow duplicate keys when reading CBOR objects from a data stream. Used only when decoding CBOR objects. Value: 2.

### NoIndefLengthStrings

    public static readonly PeterO.Cbor.CBOREncodeOptions NoIndefLengthStrings;

Always encode strings with a definite-length encoding. Used only when encoding CBOR objects. Value: 1.

### None

    public static readonly PeterO.Cbor.CBOREncodeOptions None;

No special options for encoding/decoding. Value: 0.

### UseDuplicateKeys

    public bool UseDuplicateKeys { get; }

Gets a value not documented yet.

<b>Returns:</b>

A value not documented yet.

### UseIndefLengthStrings

    public bool UseIndefLengthStrings { get; }

Gets a value not documented yet.

<b>Returns:</b>

A value not documented yet.

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

 * <i>o</i>: The parameter  <i>o</i>
 is a CBOREncodeOptions object.

<b>Return Value:</b>

A CBOREncodeOptions object.

### Or

    public PeterO.Cbor.CBOREncodeOptions Or(
        PeterO.Cbor.CBOREncodeOptions o);

<b>Deprecated.</b> May be removed in a later version. Option classes in this library will follow the form seen in JSONOptions in a later version; the approach used in this class is too complicated.

Returns an options object containing the combined flags of this and another options object.

<b>Parameters:</b>

 * <i>o</i>: The parameter  <i>o</i>
 is a CBOREncodeOptions object.

<b>Return Value:</b>

A new CBOREncodeOptions object.
