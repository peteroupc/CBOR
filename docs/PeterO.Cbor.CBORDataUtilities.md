## PeterO.Cbor.CBORDataUtilities

    public static class CBORDataUtilities

Contains methods useful for reading and writing data, with a focus on BOR.

### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str);

Parses a number whose format follows the JSON specification. See ParseJSONNumber(String, integersOnly, parseOnly) for more information.

<b>Parameters:</b>

 * <i>str</i>: A string to parse. The string is not allowed to contain white space haracters, including spaces.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns positive zero if he number is a zero that starts with a minus sign (such as "-0" or -0.0"). Returns null if the parsing fails, including if the string is ull or empty.

### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str,
        bool integersOnly,
        bool positiveOnly);

Parses a number whose format follows the JSON specification (RFC 8259). oughly speaking, a valid number consists of an optional minus sign, one r more basic digits (starting with 1 to 9 unless the only digit is 0), an ptional decimal point (".", full stop) with one or more basic digits, and n optional letter E or e with an optional plus or minus sign and one or ore basic digits (the exponent).

<b>Parameters:</b>

 * <i>str</i>: A string to parse. The string is not allowed to contain white space haracters, including spaces.

 * <i>integersOnly</i>: If true, no decimal points or exponents are allowed in the string.

 * <i>positiveOnly</i>: If true, only positive numbers are allowed (the leading minus is isallowed).

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns positive zero if he number is a zero that starts with a minus sign (such as "-0" or -0.0"). Returns null if the parsing fails, including if the string is ull or empty.

### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str,
        bool integersOnly,
        bool positiveOnly,
        bool preserveNegativeZero);

Parses a number whose format follows the JSON specification (RFC 8259). oughly speaking, a valid number consists of an optional minus sign, one r more basic digits (starting with 1 to 9 unless the only digit is 0), an ptional decimal point (".", full stop) with one or more basic digits, and n optional letter E or e with an optional plus or minus sign and one or ore basic digits (the exponent).

<b>Parameters:</b>

 * <i>str</i>: A string to parse. The string is not allowed to contain white space haracters, including spaces.

 * <i>integersOnly</i>: If true, no decimal points or exponents are allowed in the string.

 * <i>positiveOnly</i>: If true, only positive numbers are allowed (the leading minus is isallowed).

 * <i>preserveNegativeZero</i>: If true, returns positive zero if the number is a zero that starts with a inus sign (such as "-0" or "-0.0"). Otherwise, returns negative zero in his case.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the arsing fails, including if the string is null or empty.
