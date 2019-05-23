## PeterO.Cbor.JSONOptions

    public sealed class JSONOptions

Includes options to control how CBOR objects are converted to JSON.

### Member Summary
* <code>[Base64Padding](#Base64Padding)</code> - Gets a value indicating whether padding is written out when writing base64url or traditional base64 to JSON.
* <code>[public static readonly PeterO.Cbor.JSONOptions Default;](#Default)</code> - The default options for converting CBOR objects to JSON.
* <code>[ReplaceSurrogates](#ReplaceSurrogates)</code> - Gets a value not documented yet.

<a id="Void_ctor_Boolean"></a>
### JSONOptions Constructor

    public JSONOptions(
        bool base64Padding);

Initializes a new instance of the [PeterO.Cbor.JSONOptions](PeterO.Cbor.JSONOptions.md) class with the given values for the options.

<b>Parameters:</b>

 * <i>base64Padding</i>: Whether padding is included when writing data in base64url or traditional base64 format to JSON.

<a id="Void_ctor_Boolean_Boolean"></a>
### JSONOptions Constructor

    public JSONOptions(
        bool base64Padding,
        bool replaceSurrogates);

Initializes a new instance of the JSONOptions class.

<b>Parameters:</b>

 * <i>base64Padding</i>: A Boolean object.

 * <i>replaceSurrogates</i>: Another Boolean object.

<a id="Default"></a>
### Default

    public static readonly PeterO.Cbor.JSONOptions Default;

The default options for converting CBOR objects to JSON.

<a id="Base64Padding"></a>
### Base64Padding

    public bool Base64Padding { get; }

Gets a value indicating whether padding is written out when writing base64url or traditional base64 to JSON.

The padding character is '='.

<b>Returns:</b>

The default is false, no padding.

<a id="ReplaceSurrogates"></a>
### ReplaceSurrogates

    public bool ReplaceSurrogates { get; }

Gets a value not documented yet.

<b>Returns:</b>

A value not documented yet.
