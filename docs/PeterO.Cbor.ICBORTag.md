## PeterO.Cbor.ICBORTag

    public interface ICBORTag

<b>Deprecated.</b> May be removed in the future without replacement. Not as useful as ICBORConverters and ICBORObjectConverters for FromObject and ToObject.

 Implemented by classes that validate CBOR objects belonging to a specific tag.

### Member Summary
* <code>[GetTypeFilter()](#GetTypeFilter)</code> - Gets a type filter specifying what kinds of CBOR objects are supported by this tag.
* <code>[ValidateObject(PeterO.Cbor.CBORObject)](#ValidateObject_PeterO_Cbor_CBORObject)</code> - Generates a CBOR object based on the data of another object.

<a id="GetTypeFilter"></a>
### GetTypeFilter

    PeterO.Cbor.CBORTypeFilter GetTypeFilter();

 Gets a type filter specifying what kinds of CBOR objects are supported by this tag.

 <b>Return Value:</b>

A CBOR type filter.

<a id="ValidateObject_PeterO_Cbor_CBORObject"></a>
### ValidateObject

    PeterO.Cbor.CBORObject ValidateObject(
        PeterO.Cbor.CBORObject obj);

 Generates a CBOR object based on the data of another object. If the data is not valid, should throw a CBORException.

 <b>Parameters:</b>

 * <i>obj</i>: A CBOR object with the corresponding tag handled by the ICBORTag object.

<b>Return Value:</b>

A CBORObject object. Note that this method may choose to return the same object as the parameter.
