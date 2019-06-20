## PeterO.Cbor.CBORTypeMapper

    public sealed class CBORTypeMapper

 Holds converters to customize the serialization and deserialization behavior of  `CBORObject.FromObject` CBORObject.FromObject and  `CBORObject#ToObject` CBORObject#ToObject , as well as type filters for  `ToObject` ToObject  ### Member Summary
* <code>[AddConverter&lt;T&gt;(System.Type, PeterO.Cbor.ICBORConverter&lt;T&gt;)](#AddConverter_T_System_Type_PeterO_Cbor_ICBORConverter_T)</code> -
* <code>[AddTypeName(string)](#AddTypeName_string)</code> - Adds the fully qualified name of a Java or .
* <code>[AddTypePrefix(string)](#AddTypePrefix_string)</code> - Adds a prefix of a Java or .
* <code>[FilterTypeName(string)](#FilterTypeName_string)</code> - Returns whether the given Java or .

<a id="Void_ctor"></a>
### CBORTypeMapper Constructor

    public CBORTypeMapper();

 Initializes a new instance of the [PeterO.Cbor.CBORTypeMapper](PeterO.Cbor.CBORTypeMapper.md) class.  <a id="AddTypeName_string"></a>
### AddTypeName

    public PeterO.Cbor.CBORTypeMapper AddTypeName(
        string name);

 Adds the fully qualified name of a Java or .NET type for use in type matching.  <b>Parameters:</b>

 * <i>name</i>: The fully qualified name of a Java or .NET class (e.g.,  `java.math.BigInteger` java.math.BigInteger or  `System.Globalization.CultureInfo` System.Globalization.CultureInfo ).

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

 Adds a prefix of a Java or .NET type for use in type matching. A type matches a prefix if its fully qualified name is or begins with that prefix, using codepoint-by-codepoint (case-sensitive) matching.  <b>Parameters:</b>

 * <i>prefix</i>: The prefix of a Java or .NET type (e.g., `java.math.` or `System.Globalization`).

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

 Returns whether the given Java or .NET type name fits the filters given in this mapper.  <b>Parameters:</b>

 * <i>typeName</i>: The fully qualified name of a Java or .NET class (e.g.,  `java.math.BigInteger` java.math.BigInteger or  `System.Globalization.CultureInfo` System.Globalization.CultureInfo ).

<b>Return Value:</b>

Either  `true` true if the given Java or .NET type name fits the filters given in this mapper, or  `false` false otherwise.
