## PeterO.Cbor.CBORDateConverter.ConversionType

    public sealed struct ConversionType :
        System.Enum,
        System.IComparable,
        System.IConvertible,
        System.IFormattable

Conversion type for date-time conversion.

### Member Summary
* <code>[public static PeterO.Cbor.CBORDateConverter.ConversionType TaggedNumber = 1;](#TaggedNumber)</code> - FromCBORObject accepts objects with tag 0 (date/time strings) and tag 1 (number of seconds since the start of 1970), and ToCBORObject converts date/time objects to CBOR objects of tag 1.
* <code>[public static PeterO.Cbor.CBORDateConverter.ConversionType TaggedString = 0;](#TaggedString)</code> - FromCBORObject accepts CBOR objects with tag 0 (date/time strings) and tag 1 (number of seconds since the start of 1970), and ToCBORObject converts date/time objects to CBOR objects of tag 0.
* <code>[public static PeterO.Cbor.CBORDateConverter.ConversionType UntaggedNumber = 2;](#UntaggedNumber)</code> - FromCBORObject accepts untagged CBOR integer or CBOR floating-point objects that give the number of seconds since the start of 1970, and ToCBORObject converts date/time objects (DateTime in DotNet, and Date in Java) to such untagged CBOR objects.

<a id="TaggedNumber"></a>
### TaggedNumber

    public static PeterO.Cbor.CBORDateConverter.ConversionType TaggedNumber = 1;

FromCBORObject accepts objects with tag 0 (date/time strings) and tag 1 (number of seconds since the start of 1970), and ToCBORObject converts date/time objects to CBOR objects of tag 1. The ToCBORObject conversion is lossless only if the number of seconds since the start of 1970 can be represented exactly as an integer in the interval [-(2^64), 2^64 - 1] or as a 64-bit floating-point number in the IEEE 754r binary64 format; the conversion is lossy otherwise. The ToCBORObject conversion will throw an exception if the conversion to binary64 results in positive infinity, negative infinity, or not-a-number.

<a id="TaggedString"></a>
### TaggedString

    public static PeterO.Cbor.CBORDateConverter.ConversionType TaggedString = 0;

FromCBORObject accepts CBOR objects with tag 0 (date/time strings) and tag 1 (number of seconds since the start of 1970), and ToCBORObject converts date/time objects to CBOR objects of tag 0.

<a id="UntaggedNumber"></a>
### UntaggedNumber

    public static PeterO.Cbor.CBORDateConverter.ConversionType UntaggedNumber = 2;

FromCBORObject accepts untagged CBOR integer or CBOR floating-point objects that give the number of seconds since the start of 1970, and ToCBORObject converts date/time objects (DateTime in DotNet, and Date in Java) to such untagged CBOR objects. The ToCBORObject conversion is lossless only if the number of seconds since the start of 1970 can be represented exactly as an integer in the interval [-(2^64), 2^64 - 1] or as a 64-bit floating-point number in the IEEE 754r binary64 format; the conversion is lossy otherwise. The ToCBORObject conversion will throw an exception if the conversion to binary64 results in positive infinity, negative infinity, or not-a-number.
