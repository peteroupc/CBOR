## PeterO.Cbor.PODOptions

    public class PODOptions

Options for converting "plain old data" objects to CBOR objects.

### PODOptions Constructor

    public PODOptions(
        bool removeIsPrefix,
        bool useCamelCase);

Initializes a new instance of the [PeterO.Cbor.PODOptions](PeterO.Cbor.PODOptions.md) class.

<b>Parameters:</b>

 * <i>removeIsPrefix</i>: If set to `true` remove is prefix.

 * <i>useCamelCase</i>: If set to `true` use camel case.

### Default

    public static readonly PeterO.Cbor.PODOptions Default;

The default settings for "plain old data" options.

### RemoveIsPrefix

    public bool RemoveIsPrefix { get; }

Gets a value indicating whether the "Is" prefix in property names is emoved before they are used as keys.

<b>Returns:</b>

 `true` if the prefix is removed; otherwise,  `false` .

### UseCamelCase

    public bool UseCamelCase { get; }

Gets a value indicating whether property names are converted to camel ase before they are used as keys.

<b>Returns:</b>

 `true` if the names are converted to camel case; otherwise,  `false` .
