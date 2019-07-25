## PeterO.Cbor.JSONOptions

    public sealed class JSONOptions

 Includes options to control how CBOR objects are converted to JSON.

### Member Summary
* <code>[Base64Padding](#Base64Padding)</code> - Gets a value indicating whether the Base64Padding property is true.
* <code>[public static readonly PeterO.Cbor.JSONOptions Default;](#Default)</code> - The default options for converting CBOR objects to JSON.
* <code>[ReplaceSurrogates](#ReplaceSurrogates)</code> - Gets a value indicating whether surrogate code points not part of a surrogate pair (which consists of two consecutive char s forming one Unicode code point) are each replaced with a replacement character (U + FFFD).
* <code>[ToString()](#ToString)</code> - Gets the values of this options object's properties in text form.

<a id="Void_ctor_Boolean"></a>
### JSONOptions Constructor

    public JSONOptions(
        bool base64Padding);

 Initializes a new instance of the [PeterO.Cbor.JSONOptions](PeterO.Cbor.JSONOptions.md) class with the given value for the Base64Padding option.

   <b>Parameters:</b>

 * <i>base64Padding</i>: Whether padding is included when writing data in base64url or traditional base64 format to JSON.

<a id="Void_ctor_Boolean_Boolean"></a>
### JSONOptions Constructor

    public JSONOptions(
        bool base64Padding,
        bool replaceSurrogates);

 Initializes a new instance of the [PeterO.Cbor.JSONOptions](PeterO.Cbor.JSONOptions.md) class with the given values for the options.

    <b>Parameters:</b>

 * <i>base64Padding</i>: Whether padding is included when writing data in base64url or traditional base64 format to JSON.

 * <i>replaceSurrogates</i>: Whether surrogate code points not part of a surrogate pair (which consists of two consecutive  `char`  s forming one Unicode code point) are each replaced with a replacement character (U + FFFD). The default is false; an exception is thrown when such code points are encountered.

<a id="Void_ctor_System_String"></a>
### JSONOptions Constructor

    public JSONOptions(
        string paramString);

 Initializes a new instance of the [PeterO.Cbor.JSONOptions](PeterO.Cbor.JSONOptions.md) class.

    <b>Parameters:</b>

 * <i>paramString</i>: A string setting forth the options to use. This is a semicolon-separated list of options, each of which has a key and a value separated by an equal sign ("="). Whitespace and line separators are not allowed to appear between the semicolons or between the equal signs, nor may the string begin or end with whitespace. The following is an example of this parameter:  `base64padding=false;replacesurrogates=true` . The key can be any one of the following in any combination of case:  `base64padding` ,  `replacesurrogates` . Other keys are ignored. If the same key appears more than once, the value given for the last such key is used. The two keys just given can have a value of  `1` ,  `true` ,  `yes` , or  `on`  (in any combination of case), which means true, or any other value meaning false. For example,  `base64padding=Yes`  and  `base64padding =
            1`  both set the  `Base64Padding`  property to true.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>paramString</i>
 is null.

<a id="Void_ctor"></a>
### JSONOptions Constructor

    public JSONOptions();

 Initializes a new instance of the [PeterO.Cbor.JSONOptions](PeterO.Cbor.JSONOptions.md) class with default options.

  <a id="Default"></a>
### Default

    public static readonly PeterO.Cbor.JSONOptions Default;

 The default options for converting CBOR objects to JSON.

  <a id="Base64Padding"></a>
### Base64Padding

    public bool Base64Padding { get; }

<b>Deprecated.</b> This option now has no effect. This library now includes necessary padding when writing traditional base64 to JSON and includes no padding when writing base64url to JSON, in accordance with the revision of the CBOR specification.

 Gets a value indicating whether the Base64Padding property is true. This property has no effect; in previous versions, this property meant that padding was written out when writing base64url or traditional base64 to JSON.

   <b>Returns:</b>

A value indicating whether the Base64Padding property is true.

<a id="ReplaceSurrogates"></a>
### ReplaceSurrogates

    public bool ReplaceSurrogates { get; }

 Gets a value indicating whether surrogate code points not part of a surrogate pair (which consists of two consecutive  `char`  s forming one Unicode code point) are each replaced with a replacement character (U + FFFD). The default is false; an exception is thrown when such code points are encountered.

   <b>Returns:</b>

True, if surrogate code points not part of a surrogate pair are each replaced with a replacement character, or false if an exception is thrown when such code points are encountered.

<a id="ToString"></a>
### ToString

    public override string ToString();

 Gets the values of this options object's properties in text form.

   <b>Return Value:</b>

A text string containing the values of this options object's properties. The format of the string is the same as the one described in the String constructor for this class.
