## PeterO.Cbor.CBORTypeFilter

    public class CBORTypeFilter

Specifies what kinds of CBOR objects a tag can be. This class is used when a CBOR object is being read from a data stream.

### WithUnsignedInteger

    public PeterO.Cbor.CBORTypeFilter WithUnsignedInteger();

Copies this filter and includes unsigned integers in the new filter.

<b>Returns:</b>

A CBORTypeFilter object.

### WithNegativeInteger

    public PeterO.Cbor.CBORTypeFilter WithNegativeInteger();

Copies this filter and includes negative integers in the new filter.

<b>Returns:</b>

A CBORTypeFilter object.

### WithByteString

    public PeterO.Cbor.CBORTypeFilter WithByteString();

Copies this filter and includes byte strings in the new filter.

<b>Returns:</b>

A CBORTypeFilter object.

### WithMap

    public PeterO.Cbor.CBORTypeFilter WithMap();

Copies this filter and includes maps in the new filter.

<b>Returns:</b>

A CBORTypeFilter object.

### WithTextString

    public PeterO.Cbor.CBORTypeFilter WithTextString();

Copies this filter and includes text strings in the new filter.

<b>Returns:</b>

A CBORTypeFilter object.

### WithTags

    public PeterO.Cbor.CBORTypeFilter WithTags(
        params int[] tags);

Not documented yet.

<b>Parameters:</b>

 * <i>tags</i>: An integer array of tags allowed.

<b>Returns:</b>

A CBORTypeFilter object.

### WithTags

    public PeterO.Cbor.CBORTypeFilter WithTags(
        params PeterO.BigInteger[] tags);

Not documented yet.

<b>Parameters:</b>

 * <i>tags</i>: A BigInteger[] object.

<b>Returns:</b>

A CBORTypeFilter object.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter "tags[i]" is null.

### WithArrayExactLength

    public PeterO.Cbor.CBORTypeFilter WithArrayExactLength(
        int arrayLength,
        params PeterO.Cbor.CBORTypeFilter[] elements);

Copies this filter and includes CBOR arrays with an exact length to the new filter.

<b>Parameters:</b>

 * <i>arrayLength</i>: The desired maximum length of an array.

 * <i>elements</i>: An array containing the allowed types for each element in the array. There must be at least as many elements here as given in the arrayLength parameter.

<b>Returns:</b>

A CBORTypeFilter object.

<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter arrayLength is less than 0.

 * System.ArgumentNullException: 
The parameter elements is null.

 * System.ArgumentException: 
The parameter elements has fewer elements than specified in arrayLength.

### WithArrayMinLength

    public PeterO.Cbor.CBORTypeFilter WithArrayMinLength(
        int arrayLength,
        params PeterO.Cbor.CBORTypeFilter[] elements);

Copies this filter and includes CBOR arrays with at least a given length to the new filter.

<b>Parameters:</b>

 * <i>arrayLength</i>: The desired minimum length of an array.

 * <i>elements</i>: An array containing the allowed types for each element in the array. There must be at least as many elements here as given in the arrayLength parameter.

<b>Returns:</b>

A CBORTypeFilter object.

<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter arrayLength is less than 0.

 * System.ArgumentNullException: 
The parameter elements is null.

 * System.ArgumentException: 
The parameter elements has fewer elements than specified in arrayLength.

### WithArrayAnyLength

    public PeterO.Cbor.CBORTypeFilter WithArrayAnyLength();

Copies this filter and includes arrays of any length in the new filter.

<b>Returns:</b>

A CBORTypeFilter object.

### WithFloatingPoint

    public PeterO.Cbor.CBORTypeFilter WithFloatingPoint();

Copies this filter and includes floating-point numbers in the new filter.

<b>Returns:</b>

A CBORTypeFilter object.

### MajorTypeMatches

    public bool MajorTypeMatches(
        int type);

Not documented yet.

<b>Parameters:</b>

 * <i>type</i>: A 32-bit signed integer.

<b>Returns:</b>

A Boolean object.

### ArrayLengthMatches

    public bool ArrayLengthMatches(
        int length);

Returns whether an array's length is allowed under this filter.

<b>Parameters:</b>

 * <i>length</i>: The length of a CBOR array.

<b>Returns:</b>

True if this filter allows CBOR arrays and an array's length is allowed under this filter; otherwise, false.

### ArrayLengthMatches

    public bool ArrayLengthMatches(
        long length);

Returns whether an array's length is allowed under a filter.

<b>Parameters:</b>

 * <i>length</i>: The length of a CBOR array.

<b>Returns:</b>

True if this filter allows CBOR arrays and an array's length is allowed under a filter; otherwise, false.

### ArrayLengthMatches

    public bool ArrayLengthMatches(
        PeterO.BigInteger bigLength);

Returns whether an array's length is allowed under a filter.

<b>Parameters:</b>

 * <i>bigLength</i>: A BigInteger object.

<b>Returns:</b>

True if this filter allows CBOR arrays and an array's length is allowed under a filter; otherwise, false.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bigLength</i>
 is null.

### TagAllowed

    public bool TagAllowed(
        int tag);

Gets a value indicating whether CBOR objects can have the given tag number.

<b>Parameters:</b>

 * <i>tag</i>: A tag number. Returns false if this is less than 0.

<b>Returns:</b>

True if CBOR objects can have the given tag number; otherwise, false.

### TagAllowed

    public bool TagAllowed(
        long tag);

Gets a value indicating whether CBOR objects can have the given tag number.

<b>Parameters:</b>

 * <i>tag</i>: A tag number. Returns false if this is less than 0.

<b>Returns:</b>

True if CBOR objects can have the given tag number; otherwise, false.

### TagAllowed

    public bool TagAllowed(
        PeterO.BigInteger bigTag);

Gets a value indicating whether CBOR objects can have the given tag number.

<b>Parameters:</b>

 * <i>bigTag</i>: A tag number. Returns false if this is less than 0.

<b>Returns:</b>

True if CBOR objects can have the given tag number; otherwise, false.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bigTag</i>
 is null.

### ArrayIndexAllowed

    public bool ArrayIndexAllowed(
        int index);

Determines whether this type filter allows CBOR arrays and the given array index is allowed under this type filter.

<b>Parameters:</b>

 * <i>index</i>: An array index, starting from 0.

<b>Returns:</b>

True if this type filter allows CBOR arrays and the given array index is allowed under this type filter; otherwise, false.

### GetSubFilter

    public PeterO.Cbor.CBORTypeFilter GetSubFilter(
        int index);

Not documented yet.

<b>Parameters:</b>

 * <i>index</i>: A 32-bit signed integer.

<b>Returns:</b>

A CBORTypeFilter object.

### GetSubFilter

    public PeterO.Cbor.CBORTypeFilter GetSubFilter(
        long index);

Not documented yet.

<b>Parameters:</b>

 * <i>index</i>: A 64-bit signed integer.

<b>Returns:</b>

A CBORTypeFilter object.

### NonFPSimpleValueAllowed

    public bool NonFPSimpleValueAllowed();

Returns whether this filter allows simple values that are not floating-point numbers.

<b>Returns:</b>

True if this filter allows simple values that are not floating-point numbers; otherwise, false.

### None

    public static readonly PeterO.Cbor.CBORTypeFilter None;

A filter that allows no CBOR types.

### UnsignedInteger

    public static readonly PeterO.Cbor.CBORTypeFilter UnsignedInteger;

A filter that allows unsigned integers.

### NegativeInteger

    public static readonly PeterO.Cbor.CBORTypeFilter NegativeInteger;

A filter that allows negative integers.

### Any

    public static readonly PeterO.Cbor.CBORTypeFilter Any;

A filter that allows any CBOR object.

### ByteString

    public static readonly PeterO.Cbor.CBORTypeFilter ByteString;

A filter that allows byte strings.

### TextString

    public static readonly PeterO.Cbor.CBORTypeFilter TextString;

A filter that allows text strings.


