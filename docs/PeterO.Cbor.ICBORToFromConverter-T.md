## PeterO.Cbor.ICBORToFromConverter<T>

    public interface ICBORToFromConverter<T> :
        PeterO.Cbor.ICBORConverter<T>

Classes that implement this interface can support conversions from CBOR objects to a custom type and back.

### Member Summary
* <code>[FromCBORObject(PeterO.Cbor.CBORObject)](#FromCBORObject_PeterO_Cbor_CBORObject)</code> - Converts a CBOR object to a custom type.

<b>Parameters:</b>

 * &lt;T&gt;: Type of objects to convert to and from CBOR objects.

<a id="FromCBORObject_PeterO_Cbor_CBORObject"></a>
### FromCBORObject

    T FromCBORObject(
        PeterO.Cbor.CBORObject obj);

Converts a CBOR object to a custom type.

<b>Parameters:</b>

 * <i>obj</i>: A CBOR object to convert to the custom type.

<b>Return Value:</b>

An object of the custom type after conversion.
