## PeterO.Cbor.CBORDataUtilities

    public static class CBORDataUtilities

Contains methods useful for reading and writing data, with a focus on CBOR.

### Member Summary
* <code>[ParseJSONNumber(string)](#ParseJSONNumber_string)</code> - Parses a number whose format follows the JSON specification.
* <code>[ParseJSONNumber(string, bool, bool)](#ParseJSONNumber_string_bool_bool)</code> - <b>Deprecated:</b> Call the one-argument version of this method instead. If this method call used positiveOnly = true, check that the string does not begin with '-' before calling that version. If this method call used integersOnly = true, check that the string does not contain '.', 'E', or 'e' before calling that version.
* <code>[ParseJSONNumber(string, bool, bool, bool)](#ParseJSONNumber_string_bool_bool_bool)</code> - Parses a number whose format follows the JSON specification (RFC 8259).
* <code>[ParseJSONNumber(string, int, int, PeterO.Cbor.JSONOptions)](#ParseJSONNumber_string_int_int_PeterO_Cbor_JSONOptions)</code> - Parses a number whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.
* <code>[ParseJSONNumber(string, PeterO.Cbor.JSONOptions)](#ParseJSONNumber_string_PeterO_Cbor_JSONOptions)</code> - Parses a number whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.

<a id="ParseJSONNumber_string"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str);

Parses a number whose format follows the JSON specification. The method uses a JSONOptions with all default properties except for a PreserveNegativeZero property of false.

<b>Parameters:</b>

 * <i>str</i>: A text string to parse as a JSON string.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns positive zero if the number is a zero that starts with a minus sign (such as "-0" or "-0.0"). Returns null if the parsing fails, including if the string is null or empty.

<a id="ParseJSONNumber_string_bool_bool"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str,
        bool integersOnly,
        bool positiveOnly);

<b>Deprecated.</b> Call the one-argument version of this method instead. If this method call used positiveOnly = true, check that the string does not begin with '-' before calling that version. If this method call used integersOnly = true, check that the string does not contain '.', 'E', or 'e' before calling that version.

Parses a number whose format follows the JSON specification (RFC 8259). The method uses a JSONOptions with all default properties except for a PreserveNegativeZero property of false.

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless there is only one digit and that digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A string representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>str</i>: A text string to parse as a JSON number.

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

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless there is only one digit and that digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A string representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>str</i>: A text string to parse as a JSON number.

 * <i>integersOnly</i>: If true, no decimal points or exponents are allowed in the string. The default is false.

 * <i>positiveOnly</i>: If true, the leading minus is disallowed in the string. The default is false.

 * <i>preserveNegativeZero</i>: If true, returns positive zero if the number is a zero that starts with a minus sign (such as "-0" or "-0.0"). Otherwise, returns negative zero in this case. The default is false.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the string is null or empty.

<a id="ParseJSONNumber_string_int_int_PeterO_Cbor_JSONOptions"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str,
        int offset,
        int count,
        PeterO.Cbor.JSONOptions options);

Parses a number whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless there is only one digit and that digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A string representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>str</i>: A text string to parse as a JSON number.

 * <i>offset</i>: An index, starting at 0, showing where the desired portion of  <i>str</i>
 begins.

 * <i>count</i>: The length, in code units, of the desired portion of  <i>str</i>
 (but not more than  <i>str</i>
 's length).

 * <i>options</i>: An object containing options to control how JSON numbers are decoded to CBOR objects. Can be null, in which case a JSONOptions object with all default properties is used instead.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the string is null or empty or  <i>count</i>
 is 0 or less.

<b>Exceptions:</b>

 * System.ArgumentException:
Either  <i>offset</i>
 or  <i>count</i>
 is less than 0 or greater than  <i>str</i>
 's length, or  <i>str</i>
 's length minus  <i>offset</i>
 is less than  <i>count</i>
.

<a id="ParseJSONNumber_string_PeterO_Cbor_JSONOptions"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str,
        PeterO.Cbor.JSONOptions options);

Parses a number whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless there is only one digit and that digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A string representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>str</i>: A text string to parse as a JSON number.

 * <i>options</i>: An object containing options to control how JSON numbers are decoded to CBOR objects. Can be null, in which case a JSONOptions object with all default properties is used instead.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the string is null or empty.
