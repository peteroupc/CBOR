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

A CBOR object that represents the parsed number. Returns null if the parsing fails.

### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str,
        bool integersOnly,
        bool positiveOnly);

Parses a number whose format follows the JSON specification (RFC 7159). Roughly speaking, a valid number consists of an optional minus sign, one or more digits (starting with 1 to 9 unless the only digit is 0), an optional decimal point with one or more digits, and an optional letter E or e with one or more digits (the exponent).

<b>Parameters:</b>

 * <i>str</i>: A string to parse.

 * <i>integersOnly</i>: If true, no decimal points or exponents are allowed in the string.

 * <i>positiveOnly</i>: If true, only positive numbers are allowed (the leading minus is disallowed).

<b>Returns:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails.

### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str,
        bool integersOnly,
        bool positiveOnly,
        bool failOnExponentOverflow);

<b>Deprecated.</b> Use the three-argument version instead; the 'failOnExponentOverflow' parameter now has no effect.

Parses a number whose format follows the JSON specification (RFC 7159). Roughly speaking, a valid number consists of an optional minus sign, one or more digits (starting with 1 to 9 unless the only digit is 0), an optional decimal point with one or more digits, and an optional letter E or e with one or more digits (the exponent).

<b>Parameters:</b>

 * <i>str</i>: A string to parse.

 * <i>integersOnly</i>: If true, no decimal points or exponents are allowed in the string.

 * <i>positiveOnly</i>: If true, only positive numbers are allowed (the leading minus is disallowed).

 * <i>failOnExponentOverflow</i>: Has no effect.

<b>Returns:</b>

A CBOR object that represents the parsed number.


