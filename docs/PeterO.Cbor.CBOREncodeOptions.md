## PeterO.Cbor.CBOREncodeOptions

    public sealed class CBOREncodeOptions

Specifies options for encoding and decoding CBOR objects.

### Member Summary
* <code>[AllowDuplicateKeys](#AllowDuplicateKeys)</code> - Gets a value indicating whether to allow duplicate keys when reading CBOR objects from a data stream.
* <code>[Ctap2Canonical](#Ctap2Canonical)</code> - Gets a value indicating whether CBOR objects are written out using the CTAP2 canonical CBOR encoding form, which is useful for implementing Web Authentication.
* <code>[public static readonly PeterO.Cbor.CBOREncodeOptions Default;](#Default)</code> - Default options for CBOR objects.
* <code>[UseIndefLengthStrings](#UseIndefLengthStrings)</code> - Gets a value indicating whether to encode strings with an indefinite-length encoding under certain circumstances.

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

 * <i>useIndefLengthStrings</i>: A value indicating whether to encode strings with a definite-length encoding in certain cases.

 * <i>allowDuplicateKeys</i>: A value indicating whether to allow duplicate keys when reading CBOR objects from a data stream.

 * <i>ctap2Canonical</i>: A value indicating whether CBOR objects are written out using the CTAP2 canonical CBOR encoding form, which is useful for implementing Web Authentication.

<a id="Default"></a>
### Default

    public static readonly PeterO.Cbor.CBOREncodeOptions Default;

Default options for CBOR objects. Disallow duplicate keys, and always encode strings using definite-length encoding. These are recommended settings for the options that may be adopted by certain CBORObject methods in the next major version.

<a id="AllowDuplicateKeys"></a>
### AllowDuplicateKeys

    public bool AllowDuplicateKeys { get; }

Gets a value indicating whether to allow duplicate keys when reading CBOR objects from a data stream. Used only when decoding CBOR objects.

<b>Returns:</b>

A value indicating whether to allow duplicate keys when reading CBOR objects from a data stream. The default is false.

<a id="Ctap2Canonical"></a>
### Ctap2Canonical

    public bool Ctap2Canonical { get; }

Gets a value indicating whether CBOR objects are written out using the CTAP2 canonical CBOR encoding form, which is useful for implementing Web Authentication. In this form, CBOR tags are not used, map keys are written out in a canonical order, and non-integer numbers and integers 2^63 or greater are written as 64-bit binary floating-point numbers.

<b>Returns:</b>

 `true`  if CBOR objects are written out using the CTAP2 canonical CBOR encoding form; otherwise,  `false` .

<a id="UseIndefLengthStrings"></a>
### UseIndefLengthStrings

    public bool UseIndefLengthStrings { get; }

Gets a value indicating whether to encode strings with an indefinite-length encoding under certain circumstances.

<b>Returns:</b>

A value indicating whether to encode strings with an indefinite-length encoding under certain circumstances. The default is false.
