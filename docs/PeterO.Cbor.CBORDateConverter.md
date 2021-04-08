## PeterO.Cbor.CBORDateConverter

    public sealed class CBORDateConverter :
        PeterO.Cbor.ICBORConverter,
        PeterO.Cbor.ICBORToFromConverter

A class for converting date-time objects to and from tagged CBOR objects.

In this method's documentation, the "number of seconds since the start of 1970" is based on the POSIX definition of "seconds since the Epoch", a definition that does not count leap seconds. This number of seconds assumes the use of a proleptic Gregorian calendar, in which the rules regarding the number of days in each month and which years are leap years are the same for all years as they were in 1970 (including without regard to transitions from other calendars to the Gregorian).

### Member Summary
* <code>[FromCBORObject(PeterO.Cbor.CBORObject)](#FromCBORObject_PeterO_Cbor_CBORObject)</code> - Not documented yet.
* <code>[StringToDateTime(string)](#StringToDateTime_string)</code> - Not documented yet.
* <code>[public static readonly PeterO.Cbor.CBORDateConverter TaggedNumber;](#TaggedNumber)</code> - A converter object where FromCBORObject accepts CBOR objects with tag 0 (date/time strings) and tag 1 (number of seconds since the start of 1970), and ToCBORObject converts date/time objects (DateTime in DotNet, and Date in Java) to CBOR objects of tag 1.
* <code>[public static readonly PeterO.Cbor.CBORDateConverter TaggedString;](#TaggedString)</code> - A converter object where FromCBORObject accepts CBOR objects with tag 0 (date/time strings) and tag 1 (number of seconds since the start of 1970), and ToCBORObject converts date/time objects (DateTime in DotNet, and Date in Java) to CBOR objects of tag 0.
* <code>[ToCBORObject(System.DateTime)](#ToCBORObject_System_DateTime)</code> - Not documented yet.
* <code>[public static readonly PeterO.Cbor.CBORDateConverter UntaggedNumber;](#UntaggedNumber)</code> - A converter object where FromCBORObject accepts untagged CBOR integer or CBOR floating-point objects that give the number of seconds since the start of 1970, and where ToCBORObject converts date/time objects (DateTime in DotNet, and Date in Java) to such untagged CBOR objects.

<a id="Void_ctor_ConversionType"></a>
### CBORDateConverter Constructor

    public CBORDateConverter(
        PeterO.Cbor.CBORDateConverter.ConversionType convType);

Initializes a new instance of the CBORDateConverter class.

Initializes a new instance of the CBORDateConverter class.

<b>Parameters:</b>

 * <i>convType</i>:

<a id="Void_ctor"></a>
### CBORDateConverter Constructor

    public CBORDateConverter();

Initializes a new instance of the CBORDateConverter class.

Initializes a new instance of the CBORDateConverter class.

<a id="TaggedNumber"></a>
### TaggedNumber

    public static readonly PeterO.Cbor.CBORDateConverter TaggedNumber;

A converter object where FromCBORObject accepts CBOR objects with tag 0 (date/time strings) and tag 1 (number of seconds since the start of 1970), and ToCBORObject converts date/time objects (DateTime in DotNet, and Date in Java) to CBOR objects of tag 1. The ToCBORObject conversion is lossless only if the number of seconds since the start of 1970 can be represented exactly as an integer in the interval [-(2^64), 2^64 - 1] or as a 64-bit floating-point number in the IEEE 754r binary64 format; the conversion is lossy otherwise. The ToCBORObject conversion will throw an exception if the conversion to binary64 results in positive infinity, negative infinity, or not-a-number.

<a id="TaggedString"></a>
### TaggedString

    public static readonly PeterO.Cbor.CBORDateConverter TaggedString;

A converter object where FromCBORObject accepts CBOR objects with tag 0 (date/time strings) and tag 1 (number of seconds since the start of 1970), and ToCBORObject converts date/time objects (DateTime in DotNet, and Date in Java) to CBOR objects of tag 0.

<a id="UntaggedNumber"></a>
### UntaggedNumber

    public static readonly PeterO.Cbor.CBORDateConverter UntaggedNumber;

A converter object where FromCBORObject accepts untagged CBOR integer or CBOR floating-point objects that give the number of seconds since the start of 1970, and where ToCBORObject converts date/time objects (DateTime in DotNet, and Date in Java) to such untagged CBOR objects. The ToCBORObject conversion is lossless only if the number of seconds since the start of 1970 can be represented exactly as an integer in the interval [-(2^64), 2^64 - 1] or as a 64-bit floating-point number in the IEEE 754r binary64 format; the conversion is lossy otherwise. The ToCBORObject conversion will throw an exception if the conversion to binary64 results in positive infinity, negative infinity, or not-a-number.

<a id="FromCBORObject_PeterO_Cbor_CBORObject"></a>
### FromCBORObject

    public sealed System.DateTime FromCBORObject(
        PeterO.Cbor.CBORObject obj);

Not documented yet.

Not documented yet.

<b>Parameters:</b>

 * <i>obj</i>: Not documented yet.

<b>Return Value:</b>

The return value is not documented yet.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>obj</i>
 is null.

<a id="StringToDateTime_string"></a>
### StringToDateTime

    public static System.DateTime StringToDateTime(
        string str);

Not documented yet.

Not documented yet.

<b>Parameters:</b>

 * <i>str</i>: Not documented yet.

<b>Return Value:</b>

The return value is not documented yet.

<a id="ToCBORObject_System_DateTime"></a>
### ToCBORObject

    public sealed PeterO.Cbor.CBORObject ToCBORObject(
        System.DateTime obj);

Not documented yet.

Not documented yet.

<b>Parameters:</b>

 * <i>obj</i>: Not documented yet.

<b>Return Value:</b>

The return value is not documented yet.
