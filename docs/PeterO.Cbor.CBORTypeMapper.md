## PeterO.Cbor.CBORTypeMapper

    public sealed class CBORTypeMapper

Holds converters to customize the serialization and deserialization behavior of  `CBORObject.FromObject`  and `CBORObject#ToObject` , as well as type filters for `ToObject`

### Member Summary
* <code>[AddConverter&lt;T&gt;(System.Type, PeterO.Cbor.ICBORConverter&lt;T&gt;)](#AddConverter_T_System_Type_PeterO_Cbor_ICBORConverter_T)</code> - Not documented yet.
* <code>[AddTypeName(string)](#AddTypeName_string)</code> - Not documented yet.
* <code>[AddTypePrefix(string)](#AddTypePrefix_string)</code> - Not documented yet.
* <code>[FilterTypeName(string)](#FilterTypeName_string)</code> - Not documented yet.

<a id="Void_ctor"></a>
### CBORTypeMapper Constructor

    public CBORTypeMapper();

Initializes a new instance of the CBORTypeMapper class.

<a id="AddConverter_T_System_Type_PeterO_Cbor_ICBORConverter_T"></a>
### AddConverter

    public PeterO.Cbor.CBORTypeMapper AddConverter<T>(
        System.Type type,
        PeterO.Cbor.ICBORConverter<T> converter);

Not documented yet.

<b>Parameters:</b>

 * <i>type</i>: Not documented yet.

 * <i>converter</i>: Not documented yet.

<b>Return Value:</b>

A CBORTypeMapper object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>type</i>
 or  <i>converter</i>
 is null.

 * System.ArgumentException:
Converter doesn't contain a proper ToCBORObject method.

<a id="AddTypeName_string"></a>
### AddTypeName

    public PeterO.Cbor.CBORTypeMapper AddTypeName(
        string name);

Not documented yet.

<b>Parameters:</b>

 * <i>name</i>: Not documented yet.

<b>Return Value:</b>

A CBORTypeMapper object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>name</i>
 is null.

<a id="AddTypePrefix_string"></a>
### AddTypePrefix

    public PeterO.Cbor.CBORTypeMapper AddTypePrefix(
        string prefix);

Not documented yet.

<b>Parameters:</b>

 * <i>prefix</i>: Not documented yet.

<b>Return Value:</b>

A CBORTypeMapper object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>prefix</i>
 is null.

<a id="FilterTypeName_string"></a>
### FilterTypeName

    public bool FilterTypeName(
        string typeName);

Not documented yet.

<b>Parameters:</b>

 * <i>typeName</i>: Not documented yet.

<b>Return Value:</b>

A Boolean object.
