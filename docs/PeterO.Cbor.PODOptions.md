## PeterO.Cbor.PODOptions

    public class PODOptions

Options for converting "plain old data" objects to CBOR objects.

### Member Summary
* <code>[public static readonly PeterO.Cbor.PODOptions Default;](#Default)</code> - The default settings for "plain old data" options.
* <code>[RemoveIsPrefix](#RemoveIsPrefix)</code> - Gets a value indicating whether the "Is" prefix in property names is removed before they are used as keys.
* <code>[UseCamelCase](#UseCamelCase)</code> - Gets a value indicating whether property names are converted to camel case before they are used as keys.

<a id="Void_ctor_Boolean_Boolean"></a>
### PODOptions Constructor

    public PODOptions(
        bool removeIsPrefix,
        bool useCamelCase);

Initializes a new instance of the [PeterO.Cbor.PODOptions](PeterO.Cbor.PODOptions.md) class.

<b>Parameters:</b>

 * <i>removeIsPrefix</i>: If set to `true
      ` remove is prefix. NOTE: May be ignored in future versions of this ibrary.

 * <i>useCamelCase</i>: If set to `true
      ` use camel case.

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

 `true
      ` If the prefix is removed; otherwise, . `false
      ` .

<a id="UseCamelCase"></a>
### UseCamelCase

    public bool UseCamelCase { get; }

Gets a value indicating whether property names are converted to camel case before they are used as keys.

<b>Returns:</b>

 `true
      ` If the names are converted to camel case; otherwise, . `false
      ` .
