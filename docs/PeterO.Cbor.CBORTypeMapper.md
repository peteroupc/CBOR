## PeterO.Cbor.CBORTypeMapper

    public sealed class CBORTypeMapper

Holds converters to customize the serialization and deserialization behavior of  `CBORObject.FromObject`  and  `CBORObject#ToObject` , as well as type filters for  `ToObject` .

### Member Summary
* <code>[AddConverter&lt;T&gt;(System.Type, PeterO.Cbor.ICBORConverter&lt;T&gt;)](#AddConverter_T_System_Type_PeterO_Cbor_ICBORConverter_T)</code> - Registers an object that converts objects of a given type to CBOR objects (called a CBOR converter).
* <code>[AddTypeName(string)](#AddTypeName_string)</code> - Adds the fully qualified name of a Java or.
* <code>[AddTypePrefix(string)](#AddTypePrefix_string)</code> - Adds a prefix of a Java or.
* <code>[FilterTypeName(string)](#FilterTypeName_string)</code> - Returns whether the given Java or.

<a id="Void_ctor"></a>
### CBORTypeMapper Constructor

    public CBORTypeMapper();

Initializes a new instance of the [PeterO.Cbor.CBORTypeMapper](PeterO.Cbor.CBORTypeMapper.md) class.

<a id="AddConverter_T_System_Type_PeterO_Cbor_ICBORConverter_T"></a>
### AddConverter

    public PeterO.Cbor.CBORTypeMapper AddConverter<T>(
        System.Type type,
        PeterO.Cbor.ICBORConverter<T> converter);

Registers an object that converts objects of a given type to CBOR objects (called a CBOR converter). If the CBOR converter converts to and from CBOR objects, it should implement the ICBORToFromConverter interface and provide ToCBORObject and FromCBORObject methods. If the CBOR converter only supports converting to (not from) CBOR objects, it should implement the ICBORConverter interface and provide a ToCBORObject method.

<b>Parameters:</b>

 * <i>type</i>: A Type object specifying the type that the converter converts to CBOR objects.

 * <i>converter</i>: The parameter  <i>converter</i>
 is an ICBORConverter object.

 * &lt;T&gt;: Must be the same as the "type" parameter.

<b>Return Value:</b>

This object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>type</i>
 or  <i>converter</i>
 is null.

 * System.ArgumentException:
Converter doesn't contain a proper ToCBORObject method".

<a id="AddTypeName_string"></a>
### AddTypeName

    public PeterO.Cbor.CBORTypeMapper AddTypeName(
        string name);

Adds the fully qualified name of a Java or.NET type for use in type matching.

<b>Parameters:</b>

 * <i>name</i>: The fully qualified name of a Java or.NET class (e.g.,  `java.math.BigInteger`  or  `System.Globalization.CultureInfo`  ).

<b>Return Value:</b>

This object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>name</i>
 is null.

 * System.ArgumentException:
The parameter  <i>name</i>
 is empty.

<a id="AddTypePrefix_string"></a>
### AddTypePrefix

    public PeterO.Cbor.CBORTypeMapper AddTypePrefix(
        string prefix);

Adds a prefix of a Java or.NET type for use in type matching. A type matches a prefix if its fully qualified name is or begins with that prefix, using codepoint-by-codepoint (case-sensitive) matching.

<b>Parameters:</b>

 * <i>prefix</i>: The prefix of a Java or.NET type (e.g., `java.math.` or `System.Globalization`).

<b>Return Value:</b>

This object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>prefix</i>
 is null.

 * System.ArgumentException:
The parameter  <i>prefix</i>
 is empty.

<a id="FilterTypeName_string"></a>
### FilterTypeName

    public bool FilterTypeName(
        string typeName);

Returns whether the given Java or.NET type name fits the filters given in this mapper.

<b>Parameters:</b>

 * <i>typeName</i>: The fully qualified name of a Java or.NET class (e.g.,  `java.math.BigInteger`  or  `System.Globalization.CultureInfo`  ).

<b>Return Value:</b>

Either  `true`  if the given Java or.NET type name fits the filters given in this mapper, or  `false`  otherwise.
