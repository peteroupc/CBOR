## PeterO.Cbor.CBORTypeFilter

    public sealed class CBORTypeFilter

<b>Deprecated.</b> May be removed without replacement.

 Specifies what kinds of CBOR objects a tag can be. This class is used when a CBOR object is being read from a data stream. This class can't be inherited; this is a change in version 2.0 from previous versions, where the class was inadvertently left inheritable.

### Member Summary
* <code>[public static readonly PeterO.Cbor.CBORTypeFilter Any;](#Any)</code> - A filter that allows any CBOR object.
* <code>[ArrayIndexAllowed(int)](#ArrayIndexAllowed_int)</code> - Determines whether this type filter allows CBOR arrays and the given array index is allowed under this type filter.
* <code>[ArrayLengthMatches(int)](#ArrayLengthMatches_int)</code> - Returns whether an array's length is allowed under this filter.
* <code>[ArrayLengthMatches(long)](#ArrayLengthMatches_long)</code> - Returns whether an array's length is allowed under a filter.
* <code>[ArrayLengthMatches(PeterO.Numbers.EInteger)](#ArrayLengthMatches_PeterO_Numbers_EInteger)</code> - Returns whether an array's length is allowed under a filter.
* <code>[public static readonly PeterO.Cbor.CBORTypeFilter ByteString;](#ByteString)</code> - A filter that allows byte strings.
* <code>[GetSubFilter(int)](#GetSubFilter_int)</code> - Gets the type filter for this array filter by its index.
* <code>[GetSubFilter(long)](#GetSubFilter_long)</code> - Gets the type filter for this array filter by its index.
* <code>[MajorTypeMatches(int)](#MajorTypeMatches_int)</code> - Returns whether the given CBOR major type matches a major type allowed by this filter.
* <code>[public static readonly PeterO.Cbor.CBORTypeFilter NegativeInteger;](#NegativeInteger)</code> - A filter that allows negative integers.
* <code>[public static readonly PeterO.Cbor.CBORTypeFilter None;](#None)</code> - A filter that allows no CBOR types.
* <code>[NonFPSimpleValueAllowed()](#NonFPSimpleValueAllowed)</code> - Returns whether this filter allows simple values that are not floating-point numbers.
* <code>[TagAllowed(int)](#TagAllowed_int)</code> - Gets a value indicating whether CBOR objects can have the given tag number.
* <code>[TagAllowed(long)](#TagAllowed_long)</code> - Gets a value indicating whether CBOR objects can have the given tag number.
* <code>[TagAllowed(PeterO.Numbers.EInteger)](#TagAllowed_PeterO_Numbers_EInteger)</code> - Gets a value indicating whether CBOR objects can have the given tag number.
* <code>[public static readonly PeterO.Cbor.CBORTypeFilter TextString;](#TextString)</code> - A filter that allows text strings.
* <code>[public static readonly PeterO.Cbor.CBORTypeFilter UnsignedInteger;](#UnsignedInteger)</code> - A filter that allows unsigned integers.
* <code>[WithArrayAnyLength()](#WithArrayAnyLength)</code> - Copies this filter and includes arrays of any length in the new filter.
* <code>[WithArrayExactLength(int, params PeterO.Cbor.CBORTypeFilter[])](#WithArrayExactLength_int_params_PeterO_Cbor_CBORTypeFilter)</code> - Copies this filter and includes CBOR arrays with an exact length to the new filter.
* <code>[WithArrayMinLength(int, params PeterO.Cbor.CBORTypeFilter[])](#WithArrayMinLength_int_params_PeterO_Cbor_CBORTypeFilter)</code> - Copies this filter and includes CBOR arrays with at least a given length to the new filter.
* <code>[WithByteString()](#WithByteString)</code> - Copies this filter and includes byte strings in the new filter.
* <code>[WithFloatingPoint()](#WithFloatingPoint)</code> - Copies this filter and includes floating-point numbers in the new filter.
* <code>[WithMap()](#WithMap)</code> - Copies this filter and includes maps in the new filter.
* <code>[WithNegativeInteger()](#WithNegativeInteger)</code> - Copies this filter and includes negative integers in the new filter.
* <code>[WithTags(params int[])](#WithTags_params_int)</code> - Copies this filter and includes a set of valid CBOR tags in the new filter.
* <code>[WithTextString()](#WithTextString)</code> - Copies this filter and includes text strings in the new filter.
* <code>[WithUnsignedInteger()](#WithUnsignedInteger)</code> - Copies this filter and includes unsigned integers in the new filter.

<a id="Any"></a>
### Any

    public static readonly PeterO.Cbor.CBORTypeFilter Any;

 A filter that allows any CBOR object.

  <a id="ByteString"></a>
### ByteString

    public static readonly PeterO.Cbor.CBORTypeFilter ByteString;

 A filter that allows byte strings.

  <a id="NegativeInteger"></a>
### NegativeInteger

    public static readonly PeterO.Cbor.CBORTypeFilter NegativeInteger;

 A filter that allows negative integers.

  <a id="None"></a>
### None

    public static readonly PeterO.Cbor.CBORTypeFilter None;

 A filter that allows no CBOR types.

  <a id="TextString"></a>
### TextString

    public static readonly PeterO.Cbor.CBORTypeFilter TextString;

 A filter that allows text strings.

  <a id="UnsignedInteger"></a>
### UnsignedInteger

    public static readonly PeterO.Cbor.CBORTypeFilter UnsignedInteger;

 A filter that allows unsigned integers.

  <a id="ArrayIndexAllowed_int"></a>
### ArrayIndexAllowed

    public bool ArrayIndexAllowed(
        int index);

 Determines whether this type filter allows CBOR arrays and the given array index is allowed under this type filter.

 <b>Parameters:</b>

 * <i>index</i>: An array index, starting from 0.

<b>Return Value:</b>

 `true`  if this type filter allows CBOR arrays and the given array index is allowed under this type filter; otherwise,  `false`  .

<a id="ArrayLengthMatches_int"></a>
### ArrayLengthMatches

    public bool ArrayLengthMatches(
        int length);

 Returns whether an array's length is allowed under this filter.

 <b>Parameters:</b>

 * <i>length</i>: The length of a CBOR array.

<b>Return Value:</b>

 `true`  if this filter allows CBOR arrays and an array's length is allowed under this filter; otherwise,  `false`  .

<a id="ArrayLengthMatches_long"></a>
### ArrayLengthMatches

    public bool ArrayLengthMatches(
        long length);

 Returns whether an array's length is allowed under a filter.

 <b>Parameters:</b>

 * <i>length</i>: The length of a CBOR array.

<b>Return Value:</b>

 `true`  if this filter allows CBOR arrays and an array's length is allowed under a filter; otherwise,  `false`  .

<a id="ArrayLengthMatches_PeterO_Numbers_EInteger"></a>
### ArrayLengthMatches

    public bool ArrayLengthMatches(
        PeterO.Numbers.EInteger bigLength);

 Returns whether an array's length is allowed under a filter.

 <b>Parameters:</b>

 * <i>bigLength</i>: An arbitrary-precision integer.

<b>Return Value:</b>

 `true`  if this filter allows CBOR arrays and an array's length is allowed under a filter; otherwise,  `false`  .

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bigLength</i>
 is null.

<a id="GetSubFilter_int"></a>
### GetSubFilter

    public PeterO.Cbor.CBORTypeFilter GetSubFilter(
        int index);

 Gets the type filter for this array filter by its index.

 <b>Parameters:</b>

 * <i>index</i>: A zero-based index of the filter to retrieve.

<b>Return Value:</b>

Returns None if the index is out of range, or Any if this filter doesn't filter an array. Returns the appropriate filter for the array index otherwise.

<a id="GetSubFilter_long"></a>
### GetSubFilter

    public PeterO.Cbor.CBORTypeFilter GetSubFilter(
        long index);

 Gets the type filter for this array filter by its index.

 <b>Parameters:</b>

 * <i>index</i>: A zero-based index of the filter to retrieve.

<b>Return Value:</b>

Returns None if the index is out of range, or Any if this filter doesn't filter an array. Returns the appropriate filter for the array index otherwise.

<a id="MajorTypeMatches_int"></a>
### MajorTypeMatches

    public bool MajorTypeMatches(
        int type);

 Returns whether the given CBOR major type matches a major type allowed by this filter.

 <b>Parameters:</b>

 * <i>type</i>: A CBOR major type from 0 to 7.

<b>Return Value:</b>

 `true`  if the given CBOR major type matches a major type allowed by this filter; otherwise,  `false`  .

<a id="NonFPSimpleValueAllowed"></a>
### NonFPSimpleValueAllowed

    public bool NonFPSimpleValueAllowed();

 Returns whether this filter allows simple values that are not floating-point numbers.

 <b>Return Value:</b>

 `true`  if this filter allows simple values that are not floating-point numbers; otherwise,  `false`  .

<a id="TagAllowed_int"></a>
### TagAllowed

    public bool TagAllowed(
        int tag);

 Gets a value indicating whether CBOR objects can have the given tag number.

 <b>Parameters:</b>

 * <i>tag</i>: A tag number. Returns false if this is less than 0.

<b>Return Value:</b>

 `true`  if CBOR objects can have the given tag number; otherwise,  `false`  .

<a id="TagAllowed_long"></a>
### TagAllowed

    public bool TagAllowed(
        long longTag);

 Gets a value indicating whether CBOR objects can have the given tag number.

 <b>Parameters:</b>

 * <i>longTag</i>: A tag number. Returns false if this is less than 0.

<b>Return Value:</b>

 `true`  if CBOR objects can have the given tag number; otherwise,  `false`  .

<a id="TagAllowed_PeterO_Numbers_EInteger"></a>
### TagAllowed

    public bool TagAllowed(
        PeterO.Numbers.EInteger bigTag);

 Gets a value indicating whether CBOR objects can have the given tag number.

 <b>Parameters:</b>

 * <i>bigTag</i>: A tag number. Returns false if this is less than 0.

<b>Return Value:</b>

 `true`  if CBOR objects can have the given tag number; otherwise,  `false`  .

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bigTag</i>
 is null.

<a id="WithArrayAnyLength"></a>
### WithArrayAnyLength

    public PeterO.Cbor.CBORTypeFilter WithArrayAnyLength();

 Copies this filter and includes arrays of any length in the new filter.

 <b>Return Value:</b>

A CBORTypeFilter object.

<a id="WithArrayExactLength_int_params_PeterO_Cbor_CBORTypeFilter"></a>
### WithArrayExactLength

    public PeterO.Cbor.CBORTypeFilter WithArrayExactLength(
        int arrayLength,
        params PeterO.Cbor.CBORTypeFilter[] elements);

 Copies this filter and includes CBOR arrays with an exact length to the new filter.

 <b>Parameters:</b>

 * <i>arrayLength</i>: The desired maximum length of an array.

 * <i>elements</i>: An array containing the allowed types for each element in the array. There must be at least as many elements here as given in the arrayLength parameter.

<b>Return Value:</b>

A CBORTypeFilter object.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter arrayLength is less than 0.

 * System.ArgumentNullException:
The parameter elements is null.

 * System.ArgumentException:
The parameter elements has fewer elements than specified in arrayLength.

<a id="WithArrayMinLength_int_params_PeterO_Cbor_CBORTypeFilter"></a>
### WithArrayMinLength

    public PeterO.Cbor.CBORTypeFilter WithArrayMinLength(
        int arrayLength,
        params PeterO.Cbor.CBORTypeFilter[] elements);

 Copies this filter and includes CBOR arrays with at least a given length to the new filter.

 <b>Parameters:</b>

 * <i>arrayLength</i>: The desired minimum length of an array.

 * <i>elements</i>: An array containing the allowed types for each element in the array. There must be at least as many elements here as given in the arrayLength parameter.

<b>Return Value:</b>

A CBORTypeFilter object.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter arrayLength is less than 0.

 * System.ArgumentNullException:
The parameter elements is null.

 * System.ArgumentException:
The parameter elements has fewer elements than specified in arrayLength.

<a id="WithByteString"></a>
### WithByteString

    public PeterO.Cbor.CBORTypeFilter WithByteString();

 Copies this filter and includes byte strings in the new filter.

 <b>Return Value:</b>

A CBORTypeFilter object.

<a id="WithFloatingPoint"></a>
### WithFloatingPoint

    public PeterO.Cbor.CBORTypeFilter WithFloatingPoint();

 Copies this filter and includes floating-point numbers in the new filter.

 <b>Return Value:</b>

A CBORTypeFilter object.

<a id="WithMap"></a>
### WithMap

    public PeterO.Cbor.CBORTypeFilter WithMap();

 Copies this filter and includes maps in the new filter.

 <b>Return Value:</b>

A CBORTypeFilter object.

<a id="WithNegativeInteger"></a>
### WithNegativeInteger

    public PeterO.Cbor.CBORTypeFilter WithNegativeInteger();

 Copies this filter and includes negative integers in the new filter.

 <b>Return Value:</b>

A CBORTypeFilter object.

<a id="WithTags_params_int"></a>
### WithTags

    public PeterO.Cbor.CBORTypeFilter WithTags(
        params int[] tags);

 Copies this filter and includes a set of valid CBOR tags in the new filter.

 <b>Parameters:</b>

 * <i>tags</i>: An array of the CBOR tags to add to the new filter.

<b>Return Value:</b>

A CBORTypeFilter object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>tags</i>
 or  <i>tags</i>
 is null.

<a id="WithTextString"></a>
### WithTextString

    public PeterO.Cbor.CBORTypeFilter WithTextString();

 Copies this filter and includes text strings in the new filter.

 <b>Return Value:</b>

A CBORTypeFilter object.

<a id="WithUnsignedInteger"></a>
### WithUnsignedInteger

    public PeterO.Cbor.CBORTypeFilter WithUnsignedInteger();

 Copies this filter and includes unsigned integers in the new filter.

 <b>Return Value:</b>

A CBORTypeFilter object.
