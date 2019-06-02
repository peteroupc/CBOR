## PeterO.Cbor.JSONOptions

    public sealed class JSONOptions

Includes options to control how CBOR objects are converted to JSON.

### Member Summary
* <code>[Base64Padding](#Base64Padding)</code> - Gets a value indicating whether padding is written out when writing base64url or traditional base64 to JSON.
* <code>[public static readonly PeterO.Cbor.JSONOptions Default;](#Default)</code> - The default options for converting CBOR objects to JSON.

<a id="Void_ctor_Boolean"></a>
### JSONOptions Constructor

    public JSONOptions(
        bool base64Padding);

Initializes a new instance of the[PeterO.Cbor.JSONOptions](PeterO.Cbor.JSONOptions.md) class with the given values for the options.

NOTE: The base64Padding parameter may have no effect in the future. A future version may, by default, include necessary padding when writing traditional base64 to JSON and include no padding when writing base64url to JSON, in accordance with the revision of the CBOR specification.

<b>Parameters:</b>

 * <i>base64Padding</i>: Whether padding is included when writing data in base64url or traditional base64 format to JSON.

<a id="Default"></a>
### Default

    public static readonly PeterO.Cbor.JSONOptions Default;

The default options for converting CBOR objects to JSON.

<a id="Base64Padding"></a>
### Base64Padding

    public bool Base64Padding { get; }

<b>Deprecated.</b> This option may have no effect in the future.  A future version may, by default, include necessary padding when writing traditional base64 to JSON and include no padding when writing base64url to JSON, in accordance with the revision of the CBOR specification.

Gets a value indicating whether padding is written out when writing base64url or traditional base64 to JSON.

The padding character is '='.

<b>Returns:</b>

The default is false, no padding.
