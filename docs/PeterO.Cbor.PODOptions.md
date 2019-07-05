## PeterO.Cbor.PODOptions

    public class PODOptions

 Options for converting "plain old data" objects (better known as POCOs in .NET or POJOs in Java) to CBOR objects.

### Member Summary
* <code>[public static readonly PeterO.Cbor.PODOptions Default;](#Default)</code> - The default settings for "plain old data" options.
* <code>[UseCamelCase](#UseCamelCase)</code> - Gets a value indicating whether property names are converted to camel case before they are used as keys.

<a id="Void_ctor_Boolean_Boolean"></a>
### PODOptions Constructor

    public PODOptions(
        bool removeIsPrefix,
        bool useCamelCase);

 Initializes a new instance of the [PeterO.Cbor.PODOptions](PeterO.Cbor.PODOptions.md) class.    <b>Parameters:</b>

 * <i>removeIsPrefix</i>: If set to  `true`  remove is prefix.

 * <i>useCamelCase</i>: If set to  `true`  use camel case.

<a id="Void_ctor"></a>
### PODOptions Constructor

    public PODOptions();

 Initializes a new instance of the [PeterO.Cbor.PODOptions](PeterO.Cbor.PODOptions.md) class.  <a id="Default"></a>
### Default

    public static readonly PeterO.Cbor.PODOptions Default;

 The default settings for "plain old data" options.  <a id="UseCamelCase"></a>
### UseCamelCase

    public bool UseCamelCase { get; }

 Gets a value indicating whether property names are converted to camel case before they are used as keys. This option changes the behavior of key name serialization as follows. If "useCamelCase" is  `false`  :

  * In the .NET version, all key names are capitalized, meaning the first letter in the name is converted to upper case if it's a basic lower-case letter ("a" to "z"). (For example, "Name" and "IsName" both remain unchanged.)

  * In the Java version, for each eligible method name, the word "get" or "set" is removed from the name if the name starts with that word, then the name is capitalized. (For example, "getName" and "setName" both become "Name", and "isName" becomes "IsName".)

  If "useCamelCase" is  `true`  :

  * In the .NET version, for each eligible property name, the word "Is" is removed from the name if the name starts with that word, then the name is converted to camel case, meaning the first letter in the name is converted to lower case if it's a basic upper-case letter ("A" to "Z"). (For example, "Name" and "IsName" both become "name".)

  * In the Java version, for each eligible method name, the word "get", "set", or "is" is removed from the name if the name starts with that word, then the name is converted to camel case. (For example, "getName", "setName", and "isName" all become "name".)

  In the description above, a name "starts with" a word if that word begins the name and is followed by a character other than a basic digit or lower-case letter, that is, other than "a" to "z" or "0" to "9".

  <b>Returns:</b>

 `true`  If the names are converted to camel case; otherwise,  `false`  . This property is  `true`  by default.
