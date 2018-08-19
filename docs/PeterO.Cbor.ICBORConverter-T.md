## PeterO.Cbor.ICBORConverter<T>

    public interface ICBORConverter<T>

Interface implemented by classes that convert objects of arbitrary types to CBOR objects.

<b>Parameters:</b>

 * &lt;T&gt;: Type to convert to a CBOR object.

### ToCBORObject

    PeterO.Cbor.CBORObject ToCBORObject(
        T obj);

Converts an object to a CBOR object.

<b>Parameters:</b>

 * <i>obj</i>: An object to convert to a CBOR object.

<b>Return Value:</b>

A CBOR object.
