## PeterO.Cbor.CBORDateConverter

    public sealed class CBORDateConverter :
        PeterO.Cbor.ICBORConverter,
        PeterO.Cbor.ICBORToFromConverter

A class for converting date-time objects to and from tagged CBOR objects.

In this method's documentation, the "number of seconds since the start of 1970" is based on the POSIX definition of "seconds since the Epoch", a definition that does not count leap seconds. This number of seconds assumes the use of a proleptic Gregorian calendar, in which the rules regarding the number of days in each month and which years are leap years are the same for all years as they were in 1970 (including without regard to time zone differences or transitions from other calendars to the Gregorian).

### Member Summary
* <code>[DateFieldsToCBORObject(int, int, int)](#DateFieldsToCBORObject_int_int_int)</code> - Not documented yet.
* <code>[DateFieldsToCBORObject(int, int, int, int, int, int)](#DateFieldsToCBORObject_int_int_int_int_int_int)</code> - Not documented yet.
* <code>[DateFieldsToCBORObject(PeterO.Numbers.EInteger, int[])](#DateFieldsToCBORObject_PeterO_Numbers_EInteger_int)</code> - Not documented yet.
* <code>[FromCBORObject(PeterO.Cbor.CBORObject)](#FromCBORObject_PeterO_Cbor_CBORObject)</code> - Not documented yet.
* <code>[public static readonly PeterO.Cbor.CBORDateConverter TaggedNumber;](#TaggedNumber)</code> - A converter object where FromCBORObject accepts CBOR objects with tag 0 (date/time strings) and tag 1 (number of seconds since the start of 1970), and ToCBORObject converts date/time objects (DateTime in DotNet, and Date in Java) to CBOR objects of tag 1.
* <code>[public static readonly PeterO.Cbor.CBORDateConverter TaggedString;](#TaggedString)</code> - A converter object where FromCBORObject accepts CBOR objects with tag 0 (date/time strings) and tag 1 (number of seconds since the start of 1970), and ToCBORObject converts date/time objects (DateTime in DotNet, and Date in Java) to CBOR objects of tag 0.
* <code>[ToCBORObject(System.DateTime)](#ToCBORObject_System_DateTime)</code> - Not documented yet.
* <code>[TryGetDateTimeFields(PeterO.Cbor.CBORObject, PeterO.Numbers.EInteger[], int[])](#TryGetDateTimeFields_PeterO_Cbor_CBORObject_PeterO_Numbers_EInteger_int)</code> - Tries to extract the fields of a date and time in the form of a CBOR object.
* <code>[public static readonly PeterO.Cbor.CBORDateConverter UntaggedNumber;](#UntaggedNumber)</code> - A converter object where FromCBORObject accepts untagged CBOR integer or CBOR floating-point objects that give the number of seconds since the start of 1970, and where ToCBORObject converts date/time objects (DateTime in DotNet, and Date in Java) to such untagged CBOR objects.

<a id="Void_ctor_ConversionType"></a>
### CBORDateConverter Constructor

    public CBORDateConverter(
        PeterO.Cbor.CBORDateConverter.ConversionType convType);

Initializes a new instance of the [PeterO.Cbor.CBORDateConverter](PeterO.Cbor.CBORDateConverter.md) class.

<b>Parameters:</b>

 * <i>convType</i>: The parameter  <i>convType</i>
 is a Cbor.CBORDateConverter.ConversionType object.

<a id="Void_ctor"></a>
### CBORDateConverter Constructor

    public CBORDateConverter();

Initializes a new instance of the [PeterO.Cbor.CBORDateConverter](PeterO.Cbor.CBORDateConverter.md) class.

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

<a id="DateFieldsToCBORObject_int_int_int"></a>
### DateFieldsToCBORObject

    public PeterO.Cbor.CBORObject DateFieldsToCBORObject(
        int smallYear,
        int month,
        int day);

Not documented yet.

Not documented yet.

<b>Parameters:</b>

 * <i>smallYear</i>: Not documented yet.

 * <i>month</i>: Not documented yet.

 * <i>day</i>: Not documented yet.

<b>Return Value:</b>

<a id="DateFieldsToCBORObject_int_int_int_int_int_int"></a>
### DateFieldsToCBORObject

    public PeterO.Cbor.CBORObject DateFieldsToCBORObject(
        int smallYear,
        int month,
        int day,
        int hour,
        int minute,
        int second);

Not documented yet.

Not documented yet.

<b>Parameters:</b>

 * <i>smallYear</i>: Not documented yet.

 * <i>month</i>: Not documented yet.

 * <i>day</i>: Not documented yet.

 * <i>hour</i>: Not documented yet.

 * <i>minute</i>: Not documented yet.

 * <i>second</i>: Not documented yet.

<b>Return Value:</b>

<a id="DateFieldsToCBORObject_PeterO_Numbers_EInteger_int"></a>
### DateFieldsToCBORObject

    public PeterO.Cbor.CBORObject DateFieldsToCBORObject(
        PeterO.Numbers.EInteger bigYear,
        int[] lesserFields);

Not documented yet.

Not documented yet.

<b>Parameters:</b>

 * <i>bigYear</i>: Not documented yet.

 * <i>lesserFields</i>: Not documented yet.

<b>Return Value:</b>

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bigYear</i>
 or  <i>lesserFields</i>
 is null.

<a id="FromCBORObject_PeterO_Cbor_CBORObject"></a>
### FromCBORObject

    public sealed System.DateTime FromCBORObject(
        PeterO.Cbor.CBORObject obj);

Not documented yet.

<b>Parameters:</b>

 * <i>obj</i>: The parameter  <i>obj</i>
 is a Cbor.CBORObject object.

<b>Return Value:</b>

The return value is not documented yet.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>obj</i>
 is null.

<a id="ToCBORObject_System_DateTime"></a>
### ToCBORObject

    public sealed PeterO.Cbor.CBORObject ToCBORObject(
        System.DateTime obj);

Not documented yet.

<b>Parameters:</b>

 * <i>obj</i>: The parameter  <i>obj</i>
 is a DateTime object.

<b>Return Value:</b>

The return value is not documented yet.

<a id="TryGetDateTimeFields_PeterO_Cbor_CBORObject_PeterO_Numbers_EInteger_int"></a>
### TryGetDateTimeFields

    public bool TryGetDateTimeFields(
        PeterO.Cbor.CBORObject obj,
        PeterO.Numbers.EInteger[] year,
        int[] lesserFields);

Tries to extract the fields of a date and time in the form of a CBOR object.

<b>Parameters:</b>

 * <i>obj</i>: Not documented yet.

 * <i>year</i>: An array whose first element will store the year. The array's length must be 1 or greater. If this function fails, the first element is set to null.

 * <i>lesserFields</i>: An array that will store the fields (other than the year) of the date and time. The array's length must be 7 or greater. If this function fails, the first seven elements are set to 0. If this method is successful, the first seven elements of the array (starting at 0) will be as follows:

 * 0 - Month of the year, from 1 (January) through 12 (December).

 * 1 - Day of the month, from 1 through 31.

 * 2 - Hour of the day, from 0 through 23.

 * 3 - Minute of the hour, from 0 through 59.

 * 4 - Second of the minute, from 0 through 59.

 * 5 - Fractional seconds, expressed in nanoseconds. This value cannot be less than 0.

 * 6 - Number of minutes to subtract from this date and time to get global time. This number can be positive or negative, but cannot be less than -1439 or greater than 1439. For tags 0 and 1, this value is always 0.

<b>Return Value:</b>

Either  `true`  if the method is successful, or  `false`  otherwise.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>year</i>
 or  <i>lesserFields</i>
 is null, or contains fewer elements than required.
