## PeterO.Cbor.JSONOptions

    public sealed class JSONOptions

Includes options to control how CBOR objects are converted to and from JavaScript Object Notation (JSON).

### Member Summary
* <code>[AllowDuplicateKeys](#AllowDuplicateKeys)</code> - Gets a value indicating whether to allow duplicate keys when reading JSON.
* <code>[Base64Padding](#Base64Padding)</code> - <b>Deprecated:</b> This property now has no effect. This library now includes necessary padding when writing traditional base64 to JSON and includes no padding when writing base64url to JSON, in accordance with the revision of the CBOR specification.
* <code>[public static readonly PeterO.Cbor.JSONOptions Default;](#Default)</code> - The default options for converting CBOR objects to JSON.
* <code>[KeepKeyOrder](#KeepKeyOrder)</code> - Gets a value indicating whether to preserve the order in which a map's keys appear when decoding JSON, by using maps created as though by CBORObject.
* <code>[NumberConversion](#NumberConversion)</code> - Gets a value indicating how JSON numbers are decoded to CBOR objects.
* <code>[PreserveNegativeZero](#PreserveNegativeZero)</code> - Gets a value indicating whether the JSON decoder should preserve the distinction between positive zero and negative zero when the decoder decodes JSON to a floating-point number format that makes this distinction.
* <code>[ReplaceSurrogates](#ReplaceSurrogates)</code> - Gets a value indicating whether surrogate code points not part of a surrogate pair (which consists of two consecutive char s forming one Unicode code point) are each replaced with a replacement character (U+FFFD).
* <code>[ToString()](#ToString)</code> - Gets the values of this options object's properties in text form.
* <code>[WriteBasic](#WriteBasic)</code> - Gets a value indicating whether JSON is written using only code points from the Basic Latin block (U+0000 to U+007F), also known as ASCII.

<a id="Void_ctor_Boolean"></a>
### JSONOptions Constructor

    public JSONOptions(
        bool base64Padding);

<b>Deprecated.</b> Use the more readable string constructor instead.

Initializes a new instance of the [PeterO.Cbor.JSONOptions](PeterO.Cbor.JSONOptions.md) class with the given value for the Base64Padding option.

<b>Parameters:</b>

 * <i>base64Padding</i>: Whether padding is included when writing data in base64url or traditional base64 format to JSON.

<a id="Void_ctor_Boolean_Boolean"></a>
### JSONOptions Constructor

    public JSONOptions(
        bool base64Padding,
        bool replaceSurrogates);

<b>Deprecated.</b> Use the more readable string constructor instead.

Initializes a new instance of the [PeterO.Cbor.JSONOptions](PeterO.Cbor.JSONOptions.md) class with the given values for the options.

<b>Parameters:</b>

 * <i>base64Padding</i>: Whether padding is included when writing data in base64url or traditional base64 format to JSON.

 * <i>replaceSurrogates</i>: Whether surrogate code points not part of a surrogate pair (which consists of two consecutive  `char`  s forming one Unicode code point) are each replaced with a replacement character (U+FFFD). The default is false; an exception is thrown when such code points are encountered.

<a id="Void_ctor_System_String"></a>
### JSONOptions Constructor

    public JSONOptions(
        string paramString);

Initializes a new instance of the [PeterO.Cbor.JSONOptions](PeterO.Cbor.JSONOptions.md) class.

<b>Parameters:</b>

 * <i>paramString</i>: A string setting forth the options to use. This is a semicolon-separated list of options, each of which has a key and a value separated by an equal sign ("="). Whitespace and line separators are not allowed to appear between the semicolons or between the equal signs, nor may the string begin or end with whitespace. The string can be empty, but cannot be null. The following is an example of this parameter:  `base64padding=false;replacesurrogates=true` . The key can be any one of the following where the letters can be any combination of basic upper-case and/or basic lower-case letters:  `base64padding` ,  `replacesurrogates` ,  `allowduplicatekeys` ,  `preservenegativezero` ,  `numberconversion` ,  `writebasic` ,  `keepkeyorder` . Other keys are ignored in this version of the CBOR library. (Keys are compared using a basic case-insensitive comparison, in which two strings are equal if they match after converting the basic upper-case letters A to Z (U+0041 to U+005A) in both strings to basic lower-case letters.) If two or more key/value pairs have equal keys (in a basic case-insensitive comparison), the value given for the last such key is used. The first four keys just given can have a value of  `1` ,  `true` ,  `yes` , or  `on`  (where the letters can be any combination of basic upper-case and/or basic lower-case letters), which means true, and any other value meaning false. The last key,  `numberconversion` , can have a value of any name given in the  `JSONOptions.ConversionMode`  enumeration (where the letters can be any combination of basic upper-case and/or basic lower-case letters), and any other value is unrecognized. (If the  `numberconversion`  key is not given, its value is treated as  `intorfloat`  (formerly  `full`  in versions earlier than 5.0). If that key is given, but has an unrecognized value, an exception is thrown.) For example,  `base64padding=Yes`  and  `base64padding=1`  both set the  `Base64Padding`  property to true, and  `numberconversion=double`  sets the  `NumberConversion`  property to  `ConversionMode.Double`  .

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>paramString</i>
 is null. In the future, this class may allow other keys to store other kinds of values, not just true or false.

 * System.ArgumentException:
An unrecognized value for  `numberconversion`  was given.

<a id="Void_ctor"></a>
### JSONOptions Constructor

    public JSONOptions();

Initializes a new instance of the [PeterO.Cbor.JSONOptions](PeterO.Cbor.JSONOptions.md) class with default options.

<a id="Default"></a>
### Default

    public static readonly PeterO.Cbor.JSONOptions Default;

The default options for converting CBOR objects to JSON.

<a id="AllowDuplicateKeys"></a>
### AllowDuplicateKeys

    public bool AllowDuplicateKeys { get; }

Gets a value indicating whether to allow duplicate keys when reading JSON. Used only when decoding JSON. If this property is  `true`  and a JSON object has two or more values with the same key, the last value of that key set forth in the JSON object is taken.

<b>Returns:</b>

A value indicating whether to allow duplicate keys when reading JSON. The default is false.

<a id="Base64Padding"></a>
### Base64Padding

    public bool Base64Padding { get; }

<b>Deprecated.</b> This property now has no effect. This library now includes  necessary padding when writing traditional base64 to JSON and includes no padding when writing base64url to JSON, in  accordance with the revision of the CBOR specification.

Gets a value indicating whether the Base64Padding property is true. This property has no effect; in previous versions, this property meant that padding was written out when writing base64url or traditional base64 to JSON.

<b>Returns:</b>

A value indicating whether the Base64Padding property is true.

<a id="KeepKeyOrder"></a>
### KeepKeyOrder

    public bool KeepKeyOrder { get; }

Gets a value indicating whether to preserve the order in which a map's keys appear when decoding JSON, by using maps created as though by CBORObject.NewOrderedMap. If false, key order is not guaranteed to be preserved when decoding JSON.

<b>Returns:</b>

A value indicating whether to preserve the order in which a CBOR map's keys appear when decoding JSON. The default is false.

<a id="NumberConversion"></a>
### NumberConversion

    public PeterO.Cbor.JSONOptions.ConversionMode NumberConversion { get; }

Gets a value indicating how JSON numbers are decoded to CBOR objects. None of the conversion modes affects how CBOR objects are later encoded (such as via  `EncodeToBytes`  ).

<b>Returns:</b>

A value indicating how JSON numbers are decoded to CBOR. The default is  `ConversionMode.Full` .

<a id="PreserveNegativeZero"></a>
### PreserveNegativeZero

    public bool PreserveNegativeZero { get; }

Gets a value indicating whether the JSON decoder should preserve the distinction between positive zero and negative zero when the decoder decodes JSON to a floating-point number format that makes this distinction. For a value of  `false` , if the result of parsing a JSON string would be a floating-point negative zero, that result is a positive zero instead. (Note that this property has no effect for conversion kind  `IntOrFloatFromDouble` , where floating-point zeros are not possible.).

<b>Returns:</b>

A value indicating whether to preserve the distinction between positive zero and negative zero when decoding JSON. The default is true.

<a id="ReplaceSurrogates"></a>
### ReplaceSurrogates

    public bool ReplaceSurrogates { get; }

Gets a value indicating whether surrogate code points not part of a surrogate pair (which consists of two consecutive  `char`  s forming one Unicode code point) are each replaced with a replacement character (U+FFFD). If false, an exception is thrown when such code points are encountered.

<b>Returns:</b>

True, if surrogate code points not part of a surrogate pair are each replaced with a replacement character, or false if an exception is thrown when such code points are encountered. The default is false.

<a id="WriteBasic"></a>
### WriteBasic

    public bool WriteBasic { get; }

Gets a value indicating whether JSON is written using only code points from the Basic Latin block (U+0000 to U+007F), also known as ASCII.

<b>Returns:</b>

A value indicating whether JSON is written using only code points from the Basic Latin block (U+0000 to U+007F), also known as ASCII. Default is false.

<a id="ToString"></a>
### ToString

    public override string ToString();

Gets the values of this options object's properties in text form.

<b>Return Value:</b>

A text string containing the values of this options object's properties. The format of the string is the same as the one described in the String constructor for this class.
