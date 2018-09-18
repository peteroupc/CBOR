## PeterO.Cbor.ICBORObjectConverter<T>

    public interface ICBORObjectConverter<T> :
        PeterO.Cbor.ICBORConverter<T>

Interface implemented by classes that convert objects of arbitrary types to and from CBOR objects.

### Member Summary
* <code>[FromCBORObject(PeterO.Cbor.CBORObject)](#FromCBORObject_PeterO_Cbor_CBORObject)</code> - Converts a CBOR object to an object of a type supported by the implementing class.

<b>Parameters:</b>

 * &lt;T&gt;: Type of objects that a class implementing this method can convert to and from CBOR objects.

<a id="FromCBORObject_PeterO_Cbor_CBORObject"></a>
### FromCBORObject

    T FromCBORObject(
        PeterO.Cbor.CBORObject cbor);

Converts a CBOR object to an object of a type supported by the implementing class.

<b>Parameters:</b>

 * <i>cbor</i>: A CBOR object to convert.

<b>Return Value:</b>

The converted object.

<b>Exceptions:</b>

 * PeterO.Cbor.CBORException:
An error occurred in the conversion; for example, the conversion doesn't support the given CBOR object.
