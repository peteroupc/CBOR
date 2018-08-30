## PeterO.Cbor.JSONOptions

    public sealed class JSONOptions

Includes options to control how CBOR objects are converted to JSON.

### JSONOptions Constructor

    public JSONOptions(
        bool base64Padding);

Initializes a new instance of the [PeterO.Cbor.JSONOptions](PeterO.Cbor.JSONOptions.md) class with the given values for the options.

<b>Parameters:</b>

 * <i>base64Padding</i>: Whether padding is included when writing data in base64url or traditional base64 format to JSON.

### Default

    public static readonly PeterO.Cbor.JSONOptions Default;

The default options for converting CBOR objects to JSON.

### Base64Padding

    public bool Base64Padding { get; }

Gets a value indicating whether padding is written out when writing base64url or traditional base64 to JSON.

The padding character is '='.

<b>Returns:</b>

The default is false, no padding.
