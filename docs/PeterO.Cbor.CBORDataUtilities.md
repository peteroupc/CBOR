## PeterO.Cbor.CBORDataUtilities

    public static class CBORDataUtilities

Contains methods useful for reading and writing data, with a focus on CBOR.

### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str);

Parses a number whose format follows the JSON specification. See #ParseJSONNumber(String, integersOnly, parseOnly) for more information.

<b>Parameters:</b>

 * <i>str</i>: A string to parse.

<b>Returns:</b>

A CBOR object that represents the parsed number. Returns positive zero if the number is a zero that starts with a minus sign (such as "-0" or "-0.0"). Returns null if the parsing fails.

### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str,
        bool integersOnly,
        bool positiveOnly);

Parses a number whose format follows the JSON specification (RFC 7159). Roughly speaking, a valid number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless the only digit is 0), an optional decimal point ("." , full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent).

<b>Parameters:</b>

 * <i>str</i>: A string to parse.

 * <i>integersOnly</i>: If true, no decimal points or exponents are allowed in the string.

 * <i>positiveOnly</i>: If true, only positive numbers are allowed (the leading minus is disallowed).

<b>Returns:</b>

A CBOR object that represents the parsed number. Returns positive zero if the number is a zero that starts with a minus sign (such as "-0" or "-0.0"). Returns null if the parsing fails.
