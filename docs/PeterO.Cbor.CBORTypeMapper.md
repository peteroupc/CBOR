## PeterO.Cbor.CBORTypeMapper

    public sealed class CBORTypeMapper

Not documented yet.

### Member Summary
* <code>[AddConverter&lt;T&gt;(System.Type, PeterO.Cbor.ICBORConverter&lt;T&gt;)](#AddConverter_T_System_Type_PeterO_Cbor_ICBORConverter_T)</code> - Not documented yet.
* <code>[AddTypeName(string)](#AddTypeName_string)</code> - Adds the fully qualified name of a Java or .
* <code>[AddTypePrefix(string)](#AddTypePrefix_string)</code> - Adds a prefix of a Java or .
* <code>[FilterTypeName(string)](#FilterTypeName_string)</code> - Not documented yet.

<a id="Void_ctor"></a>
### CBORTypeMapper Constructor

    public CBORTypeMapper();

Initializes a new instance of the [PeterO.Cbor.CBORTypeMapper](PeterO.Cbor.CBORTypeMapper.md) class.

<a id="AddConverter_T_System_Type_PeterO_Cbor_ICBORConverter_T"></a>
### AddConverter

    public void AddConverter<T>(
        System.Type type,
        PeterO.Cbor.ICBORConverter<T> converter);

Not documented yet.

<b>Parameters:</b>

 * <i>type</i>: The parameter  <i>type</i>
 is not documented yet.

 * <i>converter</i>: The parameter  <i>converter</i>
 is not documented yet.

 * &lt;T&gt;: Type parameter not documented yet.

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

Adds the fully qualified name of a Java or .NET type for use in type matching.

<b>Parameters:</b>

 * <i>name</i>: The parameter  <i>name</i>
 is not documented yet.

<b>Return Value:</b>

A CBORTypeMapper object.

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

Adds a prefix of a Java or .NET type for use in type matching. A type matches a prefix if its fully qualified name is or begins with that prefix, using codepoint-by-codepoint (case-sensitive) matching.

<b>Parameters:</b>

 * <i>prefix</i>: The parameter  <i>prefix</i>
 is not documented yet.

<b>Return Value:</b>

A CBORTypeMapper object.

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

Not documented yet.

<b>Parameters:</b>

 * <i>typeName</i>: The parameter  <i>typeName</i>
 is not documented yet.

<b>Return Value:</b>

Either  `true`  or  `false` .
