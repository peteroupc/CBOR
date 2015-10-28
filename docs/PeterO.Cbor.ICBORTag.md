## PeterO.Cbor.ICBORTag

    public interface ICBORTag

Implemented by classes that validate CBOR objects belonging to a specific tag.

### GetTypeFilter

    PeterO.Cbor.CBORTypeFilter GetTypeFilter();

Gets a type filter specifying what kinds of CBOR objects are supported by this tag.

<b>Returns:</b>

A CBOR type filter.

### ValidateObject

    PeterO.Cbor.CBORObject ValidateObject(
        PeterO.Cbor.CBORObject obj);

Generates a CBOR object based on the data of another object. If the data is not valid, should throw a CBORException.

<b>Parameters:</b>

 * <i>obj</i>: A CBOR object with the corresponding tag handled by the ICBORTag object.

<b>Returns:</b>

A CBORObject object. Note that this method may choose to return the same object as the parameter.
