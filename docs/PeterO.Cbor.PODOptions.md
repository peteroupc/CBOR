## PeterO.Cbor.PODOptions

    public class PODOptions

 Options for converting "plain old data" objects to CBOR objects.

### Member Summary
* <code>[public static readonly PeterO.Cbor.PODOptions Default;](#Default)</code> - The default settings for "plain old data" options.
* <code>[RemoveIsPrefix](#RemoveIsPrefix)</code> - Gets a value indicating whether the "Is" prefix in property names is removed before they are used as keys.
* <code>[ToString()](#ToString)</code> - Gets the values of this options object's properties in text form.
* <code>[UseCamelCase](#UseCamelCase)</code> - Gets a value indicating whether property names are converted to camel case before they are used as keys.

<a id="Void_ctor_Boolean_Boolean"></a>
### PODOptions Constructor

    public PODOptions(
        bool removeIsPrefix,
        bool useCamelCase);

 Initializes a new instance of the [PeterO.Cbor.PODOptions](PeterO.Cbor.PODOptions.md) class.

   <b>Parameters:</b>

 * <i>removeIsPrefix</i>: If set to  `true`  remove is prefix. NOTE: May be ignored in future versions of this library.

 * <i>useCamelCase</i>: If set to  `true`  use camel case.

<a id="Void_ctor_System_String"></a>
### PODOptions Constructor

    public PODOptions(
        string paramString);

 Initializes a new instance of the [PeterO.Cbor.PODOptions](PeterO.Cbor.PODOptions.md) class.

   <b>Parameters:</b>

 * <i>paramString</i>: A string setting forth the options to use. This is a semicolon-separated list of options, each of which has a key and a value separated by an equal sign ("="). Whitespace and line separators are not allowed to appear between the semicolons or between the equal signs, nor may the string begin or end with whitespace. The string can be empty, but cannot be null. The following is an example of this parameter:  `usecamelcase=true` . The key can be any one of the following in any combination of case:  `usecamelcase` . Other keys are ignored. If the same key appears more than once, the value given for the last such key is used. The key just given can have a value of  `1` ,  `true` ,  `yes` , or  `on`  (in any combination of case), which means true, and any other value meaning false. For example,  `usecamelcase=Yes`  and  `usecamelcase=1`  both set the  `UseCamelCase`  property to true.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>paramString</i>
 is null.

<a id="Void_ctor"></a>
### PODOptions Constructor

    public PODOptions();

 Initializes a new instance of the [PeterO.Cbor.PODOptions](PeterO.Cbor.PODOptions.md) class.

 <a id="Default"></a>
### Default

    public static readonly PeterO.Cbor.PODOptions Default;

 The default settings for "plain old data" options.

 <a id="RemoveIsPrefix"></a>
### RemoveIsPrefix

    public bool RemoveIsPrefix { get; }

<b>Deprecated.</b> Property name conversion may change, making this property obsolete.

 Gets a value indicating whether the "Is" prefix in property names is removed before they are used as keys.

 <b>Returns:</b>

 `true`  If the prefix is removed; otherwise, .  `false`  .

<a id="UseCamelCase"></a>
### UseCamelCase

    public bool UseCamelCase { get; }

 Gets a value indicating whether property names are converted to camel case before they are used as keys.

 <b>Returns:</b>

 `true`  If the names are converted to camel case; otherwise, .  `false`  .

<a id="ToString"></a>
### ToString

    public override string ToString();

 Gets the values of this options object's properties in text form.

 <b>Return Value:</b>

A text string containing the values of this options object's properties. The format of the string is the same as the one described in the String constructor for this class.
