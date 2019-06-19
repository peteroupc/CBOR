## PeterO.Cbor.JSONOptions

    public sealed class JSONOptions

 Includes options to control how CBOR objects are converted to JSON.  ### Member Summary
* <code>[Base64Padding](#Base64Padding)</code> - Gets a value indicating whether padding is written out when writing base64url or traditional base64 to JSON.
* <code>[public static readonly PeterO.Cbor.JSONOptions Default;](#Default)</code> - The default options for converting CBOR objects to JSON.
* <code>[ReplaceSurrogates](#ReplaceSurrogates)</code> - Gets a value indicating whether surrogate code points not part of a surrogate pair (which consists of two consecutive char s forming one Unicode code point) are each replaced with a replacement character (U+FFFD).

<a id="Default"></a>
### Default

    public static readonly PeterO.Cbor.JSONOptions Default;

 The default options for converting CBOR objects to JSON.  <a id="Base64Padding"></a>
### Base64Padding

    public bool Base64Padding { get; }

<b>Deprecated.</b> This option may have no effect in the future. A future version may, by default, include necessary padding when writing traditional base64 to JSON and include no padding when writing base64url to JSON, in accordance with the revision of the CBOR specification.

 Gets a value indicating whether padding is written out when writing base64url or traditional base64 to JSON.

 The padding character is '='.  <b>Returns:</b>

The default is false, no padding.

<a id="ReplaceSurrogates"></a>
### ReplaceSurrogates

    public bool ReplaceSurrogates { get; }

 Gets a value indicating whether surrogate code points not part of a surrogate pair (which consists of two consecutive  `char` char s forming one Unicode code point) are each replaced with a replacement character (U+FFFD). The default is false; an exception is thrown when such code points are encountered.  <b>Returns:</b>

True, if surrogate code points not part of a surrogate pair are each replaced with a replacement character, or false if an exception is thrown when such code points are encountered.
