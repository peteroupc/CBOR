## PeterO.Cbor.CBORDataUtilities

    public static class CBORDataUtilities

Contains methods useful for reading and writing data, with a focus on CBOR.

### Member Summary
* <code>[ParseJSONDouble(string)](#ParseJSONDouble_string)</code> - Parses a number whose format follows the JSON specification (RFC 8259), in the form of a 64-bit binary floating-point number.
* <code>[ParseJSONDouble(string, bool)](#ParseJSONDouble_string_bool)</code> - Parses a number whose format follows the JSON specification (RFC 8259), in the form of a 64-bit binary floating-point number.
* <code>[ParseJSONNumber(string)](#ParseJSONNumber_string)</code> - Parses a number whose format follows the JSON specification.
* <code>[ParseJSONNumber(string, bool, bool)](#ParseJSONNumber_string_bool_bool)</code> - <b>Deprecated:</b> Call the one-argument version of this method instead. If this method call used positiveOnly = true, check that the string does not begin with '-' before calling that version. If this method call used integersOnly = true, check that the string does not contain '.', 'E', or 'e' before calling that version.
* <code>[ParseJSONNumber(string, bool, bool, bool)](#ParseJSONNumber_string_bool_bool_bool)</code> - Parses a number whose format follows the JSON specification (RFC 8259).
* <code>[ParseJSONNumber(string, int, int, bool, bool)](#ParseJSONNumber_string_int_int_bool_bool)</code> - Parses a number whose format follows the JSON specification (RFC 8259).
* <code>[ParseJSONNumberAsIntegerOrFloatingPoint(string, bool, bool, bool)](#ParseJSONNumberAsIntegerOrFloatingPoint_string_bool_bool_bool)</code> - Parses a number whose format follows the JSON specification (RFC 8259), in the form of a CBOR integer if the number represents an integer at least -(2^53) and less than or equal to 2^53, or in the form of a CBOR (64-bit) floating-point number otherwise.

<a id="ParseJSONDouble_string"></a>
### ParseJSONDouble

    public static double ParseJSONDouble(
        string str);

Parses a number whose format follows the JSON specification (RFC 8259), in the form of a 64-bit binary floating-point number. See #ParseJSONDouble(String, preserveNegativeZero) for more information.

<b>Parameters:</b>

 * <i>str</i>: A string to parse as a JSON number.

<b>Return Value:</b>

A 64-bit binary floating-point number parsed from the given string. Returns NaN if the parsing fails, including if the string is null or empty. (To check for NaN, use  `Double.IsNaN()`  in.NET or  `Double.isNaN()`  in Java.)

<a id="ParseJSONDouble_string_bool"></a>
### ParseJSONDouble

    public static double ParseJSONDouble(
        string str,
        bool preserveNegativeZero);

Parses a number whose format follows the JSON specification (RFC 8259), in the form of a 64-bit binary floating-point number.

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless the only digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A string representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>str</i>: A string to parse as a JSON number.

 * <i>preserveNegativeZero</i>: If true, returns positive zero if the number is a zero that starts with a minus sign (such as "-0" or "-0.0"). Otherwise, returns negative zero in this case. The default is false.

<b>Return Value:</b>

A 64-bit binary floating-point number parsed from the given string. Returns NaN if the parsing fails, including if the string is null or empty. (To check for NaN, use  `Double.IsNaN()`  in.NET or  `Double.isNaN()`  in Java.)

<a id="ParseJSONNumber_string"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str);

Parses a number whose format follows the JSON specification. See #ParseJSONNumber(String, integersOnly, parseOnly) for more information.

<b>Parameters:</b>

 * <i>str</i>: A string to parse as a JSON string.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns positive zero if the number is a zero that starts with a minus sign (such as "-0" or "-0.0"). Returns null if the parsing fails, including if the string is null or empty.

<a id="ParseJSONNumber_string_bool_bool"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str,
        bool integersOnly,
        bool positiveOnly);

<b>Deprecated.</b> Call the one-argument version of this method instead. If this method call used positiveOnly = true, check that the string does not begin with '-' before calling that version. If this method call used integersOnly = true, check that the string does not contain '.', 'E', or 'e' before calling that version.

Parses a number whose format follows the JSON specification (RFC 8259).

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless the only digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A string representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>str</i>: A string to parse as a JSON number.

 * <i>integersOnly</i>: If true, no decimal points or exponents are allowed in the string. The default is false.

 * <i>positiveOnly</i>: If true, only positive numbers are allowed (the leading minus is disallowed). The default is false.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns positive zero if the number is a zero that starts with a minus sign (such as "-0" or "-0.0"). Returns null if the parsing fails, including if the string is null or empty.

<a id="ParseJSONNumber_string_bool_bool_bool"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str,
        bool integersOnly,
        bool positiveOnly,
        bool preserveNegativeZero);

Parses a number whose format follows the JSON specification (RFC 8259).

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless the only digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A string representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>str</i>: A string to parse as a JSON number.

 * <i>integersOnly</i>: If true, no decimal points or exponents are allowed in the string. The default is false.

 * <i>positiveOnly</i>: If true, the leading minus is disallowed in the string. The default is false.

 * <i>preserveNegativeZero</i>: If true, returns positive zero if the number is a zero that starts with a minus sign (such as "-0" or "-0.0"). Otherwise, returns negative zero in this case. The default is false.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the string is null or empty.

<a id="ParseJSONNumber_string_int_int_bool_bool"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str,
        int offset,
        int count,
        bool integersOnly,
        bool preserveNegativeZero);

Parses a number whose format follows the JSON specification (RFC 8259).

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless the only digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A string representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>str</i>: Not documented yet.

 * <i>offset</i>: Not documented yet.

 * <i>count</i>: Not documented yet.

 * <i>integersOnly</i>: Not documented yet.

 * <i>preserveNegativeZero</i>: Not documented yet.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the string is null or empty or "count" is 0 or less.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

<a id="ParseJSONNumberAsIntegerOrFloatingPoint_string_bool_bool_bool"></a>
### ParseJSONNumberAsIntegerOrFloatingPoint

    public static PeterO.Cbor.CBORObject ParseJSONNumberAsIntegerOrFloatingPoint(
        string str,
        bool integersOnly,
        bool positiveOnly,
        bool doubleApprox);

Parses a number whose format follows the JSON specification (RFC 8259), in the form of a CBOR integer if the number represents an integer at least -(2^53) and less than or equal to 2^53, or in the form of a CBOR (64-bit) floating-point number otherwise.

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless the only digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A string representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>str</i>: A string to parse as a JSON number.

 * <i>integersOnly</i>: If true, no decimal points or exponents are allowed in the string. The default is false.

 * <i>positiveOnly</i>: If true, the leading minus is disallowed in the string. The default is false.

 * <i>doubleApprox</i>: If true, treats a JSON number as an integer or noninteger based on its closest approximation as a CBOR (64-bit) floating-point number. If false, this treatment is based on the full precision of the given JSON number string. For example, given the string "0.99999999999999999999999999999999999", the nearest representable CBOR floating-point number is 1.0, and if this parameter is  `true` , this string is treated as 1.0, an integer, so that the result is the CBOR integer 1, and if this parameter is  `false` , this string is not treated as an integer so that the result is the closest CBOR floating-point approximation, 1.0.

<b>Return Value:</b>

A CBOR object that represents the parsed number or its closest approximation to it. Returns null if the parsing fails, including if the string is null or empty.
