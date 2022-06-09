## PeterO.Cbor.PODOptions

    public class PODOptions

Options for controlling how certain DotNET or Java objects, such as so-called "plain old data" objects (better known as POCOs in DotNET or POJOs in Java), are converted to CBOR objects.

### Member Summary
* <code>[public static readonly PeterO.Cbor.PODOptions Default;](#Default)</code> - The default settings for "plain old data" options.
* <code>[ToString()](#ToString)</code> - Gets the values of this options object's properties in text form.
* <code>[UseCamelCase](#UseCamelCase)</code> - Gets a value indicating whether property, field, and method names are converted to camel case before they are used as keys.

<a id="Void_ctor_Boolean_Boolean"></a>
### PODOptions Constructor

    public PODOptions(
        bool removeIsPrefix,
        bool useCamelCase);

<b>Deprecated.</b> Use the more readable string constructor instead.

Initializes a new instance of the [PeterO.Cbor.PODOptions](PeterO.Cbor.PODOptions.md) class.

<b>Parameters:</b>

 * <i>removeIsPrefix</i>: The parameter is not used.

 * <i>useCamelCase</i>: The value of the "UseCamelCase" property.

<a id="Void_ctor_System_String"></a>
### PODOptions Constructor

    public PODOptions(
        string paramString);

Initializes a new instance of the [PeterO.Cbor.PODOptions](PeterO.Cbor.PODOptions.md) class.

<b>Parameters:</b>

 * <i>paramString</i>: A string setting forth the options to use. This is a semicolon-separated list of options, each of which has a key and a value separated by an equal sign ("="). Whitespace and line separators are not allowed to appear between the semicolons or between the equal signs, nor may the string begin or end with whitespace. The string can be empty, but cannot be null. The following is an example of this parameter:  `usecamelcase=true` . The key can be any one of the following where the letters can be any combination of basic upper-case and/or basic lower-case letters:  `usecamelcase` . Other keys are ignored in this version of the CBOR library. (Keys are compared using a basic case-insensitive comparison, in which two strings are equal if they match after converting the basic upper-case letters A to Z (U+0041 to U+005A) in both strings to basic lower-case letters.) If two or more key/value pairs have equal keys (in a basic case-insensitive comparison), the value given for the last such key is used. The key just given can have a value of  `1` ,  `true` ,  `yes` , or  `on`  (where the letters can be any combination of basic upper-case and/or basic lower-case letters), which means true, and any other value meaning false. For example,  `usecamelcase=Yes`  and  `usecamelcase=1`  both set the  `UseCamelCase`  property to true. In the future, this class may allow other keys to store other kinds of values, not just true or false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>paramString</i>
 is null.

<a id="Void_ctor"></a>
### PODOptions Constructor

    public PODOptions();

Initializes a new instance of the [PeterO.Cbor.PODOptions](PeterO.Cbor.PODOptions.md) class with all the default options.

<a id="Default"></a>
### Default

    public static readonly PeterO.Cbor.PODOptions Default;

The default settings for "plain old data" options.

<a id="UseCamelCase"></a>
### UseCamelCase

    public bool UseCamelCase { get; }

Gets a value indicating whether property, field, and method names are converted to camel case before they are used as keys. This option changes the behavior of key name serialization as follows. If "useCamelCase" is  `false`  :

 * In the .NET version, all key names are capitalized, meaning the first letter in the name is converted to a basic upper-case letter if it's a basic lower-case letter ("a" to "z"). (For example, "Name" and "IsName" both remain unchanged.)

 * In the Java version, all field names are capitalized, and for each eligible method name, the word "get" or "set" is removed from the name if the name starts with that word, then the name is capitalized. (For example, "getName" and "setName" both become "Name", and "isName" becomes "IsName".)

If "useCamelCase" is  `true`  :

 * In the .NET version, for each eligible property or field name, the word "Is" is removed from the name if the name starts with that word, then the name is converted to camel case, meaning the first letter in the name is converted to a basic lower-case letter if it's a basic upper-case letter ("A" to "Z"). (For example, "Name" and "IsName" both become "name", and "IsIsName" becomes "isName".)

 * In the Java version: For each eligible method name, the word "get", "set", or "is" is removed from the name if the name starts with that word, then the name is converted to camel case. (For example, "getName", "setName", and "isName" all become "name".) For each eligible field name, the word "is" is removed from the name if the name starts with that word, then the name is converted to camel case. (For example, "name" and "isName" both become "name".)

In the description above, a name "starts with" a word if that word begins the name and is followed by a character other than a basic digit or basic lower-case letter, that is, other than "a" to "z" or "0" to "9".

<b>Returns:</b>

 `true`  If the names are converted to camel case; otherwise,  `false` . This property is  `true`  by default.

<a id="ToString"></a>
### ToString

    public override string ToString();

Gets the values of this options object's properties in text form.

<b>Return Value:</b>

A text string containing the values of this options object's properties. The format of the string is the same as the one described in the String constructor for this class.
