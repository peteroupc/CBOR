## PeterO.Cbor.CBOREncodeOptions

    public sealed class CBOREncodeOptions

Specifies options for encoding CBOR objects to bytes.

### NoIndefLengthStrings

    public static readonly PeterO.Cbor.CBOREncodeOptions NoIndefLengthStrings;

Always encode strings with a definite-length encoding. Value: 1.

### None

    public static readonly PeterO.Cbor.CBOREncodeOptions None;

No special options for encoding. Value: 0.

### Value

    public int Value { get; }

Gets this options object's value.

<b>Returns:</b>

This options object's value.

### And

    public PeterO.Cbor.CBOREncodeOptions And(
        PeterO.Cbor.CBOREncodeOptions o);

Returns an options object whose flags are shared by this and another options object.

<b>Parameters:</b>

 * <i>o</i>: A CBOREncodeOptions object. (2).

<b>Returns:</b>

A CBOREncodeOptions object.

### Or

    public PeterO.Cbor.CBOREncodeOptions Or(
        PeterO.Cbor.CBOREncodeOptions o);

Combines the flags of this options object with another options object.

<b>Parameters:</b>

 * <i>o</i>: A CBOREncodeOptions object. (2).

<b>Returns:</b>

A CBOREncodeOptions object.


