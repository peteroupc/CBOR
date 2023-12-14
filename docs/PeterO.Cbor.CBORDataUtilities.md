## PeterO.Cbor.CBORDataUtilities

    public static class CBORDataUtilities

Contains methods useful for reading and writing data, with a focus on CBOR.

### Member Summary
* <code>[ParseJSONNumber(byte[])](#ParseJSONNumber_byte)</code> - Parses a number from a byte sequence whose format follows the JSON specification.
* <code>[ParseJSONNumber(byte[], int, int)](#ParseJSONNumber_byte_int_int)</code> - Parses a number whose format follows the JSON specification (RFC 8259) from a portion of a byte sequence, and converts that number to a CBOR object.
* <code>[ParseJSONNumber(byte[], int, int, PeterO.Cbor.JSONOptions)](#ParseJSONNumber_byte_int_int_PeterO_Cbor_JSONOptions)</code> - Parses a number from a byte sequence whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.
* <code>[ParseJSONNumber(byte[], PeterO.Cbor.JSONOptions)](#ParseJSONNumber_byte_PeterO_Cbor_JSONOptions)</code> - Parses a number from a byte sequence whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.
* <code>[ParseJSONNumber(char[])](#ParseJSONNumber_char)</code> - Parses a number from a sequence of char s whose format follows the JSON specification.
* <code>[ParseJSONNumber(char[], int, int)](#ParseJSONNumber_char_int_int)</code> - Parses a number whose format follows the JSON specification (RFC 8259) from a portion of a sequence of char s, and converts that number to a CBOR object.
* <code>[ParseJSONNumber(char[], int, int, PeterO.Cbor.JSONOptions)](#ParseJSONNumber_char_int_int_PeterO_Cbor_JSONOptions)</code> - Parses a number from a sequence of char s whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.
* <code>[ParseJSONNumber(char[], PeterO.Cbor.JSONOptions)](#ParseJSONNumber_char_PeterO_Cbor_JSONOptions)</code> - Parses a number from a sequence of char s whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.
* <code>[ParseJSONNumber(string)](#ParseJSONNumber_string)</code> - Parses a number whose format follows the JSON specification.
* <code>[ParseJSONNumber(string, int, int)](#ParseJSONNumber_string_int_int)</code> - Parses a number whose format follows the JSON specification (RFC 8259) from a portion of a text string, and converts that number to a CBOR object.
* <code>[ParseJSONNumber(string, int, int, PeterO.Cbor.JSONOptions)](#ParseJSONNumber_string_int_int_PeterO_Cbor_JSONOptions)</code> - Parses a number whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.
* <code>[ParseJSONNumber(string, PeterO.Cbor.JSONOptions)](#ParseJSONNumber_string_PeterO_Cbor_JSONOptions)</code> - Parses a number whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.

<a id="ParseJSONNumber_byte"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        byte[] bytes);

Parses a number from a byte sequence whose format follows the JSON specification. The method uses a JSONOptions with all default properties.

<b>Parameters:</b>

 * <i>bytes</i>: A byte sequence to parse as a JSON number.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the byte sequence is null or empty.

<a id="ParseJSONNumber_byte_int_int"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        byte[] bytes,
        int offset,
        int count);

Parses a number whose format follows the JSON specification (RFC 8259) from a portion of a byte sequence, and converts that number to a CBOR object.

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless there is only one digit and that digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A byte sequence representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>bytes</i>: A sequence of bytes to parse as a JSON number.

 * <i>offset</i>: An index, starting at 0, showing where the desired portion of  <i>bytes</i>
 begins.

 * <i>count</i>: The length, in code units, of the desired portion of  <i>bytes</i>
 (but not more than  <i>bytes</i>
 's length).

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the byte sequence is null or empty.

<b>Exceptions:</b>

 * System.ArgumentException:
Either  <i>offset</i>
 or  <i>count</i>
 is less than 0 or greater than  <i>bytes</i>
 's length, or  <i>bytes</i>
 's length minus  <i>offset</i>
 is less than  <i>count</i>
.

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 is null.

<a id="ParseJSONNumber_byte_int_int_PeterO_Cbor_JSONOptions"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        byte[] bytes,
        int offset,
        int count,
        PeterO.Cbor.JSONOptions options);

Parses a number from a byte sequence whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless there is only one digit and that digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A byte sequence representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>bytes</i>: A sequence of bytes to parse as a JSON number.

 * <i>offset</i>: An index, starting at 0, showing where the desired portion of  <i>bytes</i>
 begins.

 * <i>count</i>: The length, in code units, of the desired portion of  <i>bytes</i>
 (but not more than  <i>bytes</i>
 's length).

 * <i>options</i>: An object containing options to control how JSON numbers are decoded to CBOR objects. Can be null, in which case a JSONOptions object with all default properties is used instead.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the byte sequence is null or empty or  <i>count</i>
 is 0 or less.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 is null.

 * System.ArgumentException:
Unsupported conversion kind.

<a id="ParseJSONNumber_byte_PeterO_Cbor_JSONOptions"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        byte[] bytes,
        PeterO.Cbor.JSONOptions options);

Parses a number from a byte sequence whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless there is only one digit and that digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A byte sequence representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>bytes</i>: A sequence of bytes to parse as a JSON number.

 * <i>options</i>: An object containing options to control how JSON numbers are decoded to CBOR objects. Can be null, in which case a JSONOptions object with all default properties is used instead.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the byte sequence is null or empty.

<a id="ParseJSONNumber_char"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        char[] chars);

Parses a number from a sequence of  `char`  s whose format follows the JSON specification. The method uses a JSONOptions with all default properties.

<b>Parameters:</b>

 * <i>chars</i>: A sequence of  `char`  s to parse as a JSON number.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the sequence of  `char`  s is null or empty.

<a id="ParseJSONNumber_char_int_int"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        char[] chars,
        int offset,
        int count);

Parses a number whose format follows the JSON specification (RFC 8259) from a portion of a sequence of  `char`  s, and converts that number to a CBOR object.

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless there is only one digit and that digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A sequence of  `char`  s representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>chars</i>: A sequence of  `char`  s to parse as a JSON number.

 * <i>offset</i>: An index, starting at 0, showing where the desired portion of  <i>chars</i>
 begins.

 * <i>count</i>: The length, in code units, of the desired portion of  <i>chars</i>
 (but not more than  <i>chars</i>
 's length).

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the sequence of  `char`  s is null or empty.

<b>Exceptions:</b>

 * System.ArgumentException:
Either  <i>offset</i>
 or  <i>count</i>
 is less than 0 or greater than  <i>chars</i>
 's length, or  <i>chars</i>
 's length minus  <i>offset</i>
 is less than  <i>count</i>
.

 * System.ArgumentNullException:
The parameter  <i>chars</i>
 is null.

<a id="ParseJSONNumber_char_int_int_PeterO_Cbor_JSONOptions"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        char[] chars,
        int offset,
        int count,
        PeterO.Cbor.JSONOptions options);

Parses a number from a sequence of  `char`  s whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless there is only one digit and that digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A sequence of  `char`  s representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>chars</i>: A sequence of  `char`  s to parse as a JSON number.

 * <i>offset</i>: An index, starting at 0, showing where the desired portion of  <i>chars</i>
 begins.

 * <i>count</i>: The length, in code units, of the desired portion of  <i>chars</i>
 (but not more than  <i>chars</i>
 's length).

 * <i>options</i>: An object containing options to control how JSON numbers are decoded to CBOR objects. Can be null, in which case a JSONOptions object with all default properties is used instead.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the sequence of  `char`  s is null or empty or  <i>count</i>
 is 0 or less.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>chars</i>
 is null.

 * System.ArgumentException:
Unsupported conversion kind.

<a id="ParseJSONNumber_char_PeterO_Cbor_JSONOptions"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        char[] chars,
        PeterO.Cbor.JSONOptions options);

Parses a number from a sequence of  `char`  s whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless there is only one digit and that digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A sequence of  `char`  s representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>chars</i>: A sequence of  `char`  s to parse as a JSON number.

 * <i>options</i>: An object containing options to control how JSON numbers are decoded to CBOR objects. Can be null, in which case a JSONOptions object with all default properties is used instead.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the sequence of  `char`  s is null or empty.

<a id="ParseJSONNumber_string"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str);

Parses a number whose format follows the JSON specification. The method uses a JSONOptions with all default properties.

<b>Parameters:</b>

 * <i>str</i>: A text string to parse as a JSON number.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the string is null or empty.

<a id="ParseJSONNumber_string_int_int"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str,
        int offset,
        int count);

Parses a number whose format follows the JSON specification (RFC 8259) from a portion of a text string, and converts that number to a CBOR object.

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless there is only one digit and that digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A text string representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>str</i>: A text string containing the portion to parse as a JSON number.

 * <i>offset</i>: An index, starting at 0, showing where the desired portion of  <i>str</i>
 begins.

 * <i>count</i>: The length, in code units, of the desired portion of  <i>str</i>
 (but not more than  <i>str</i>
 's length).

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the string is null or empty.

<b>Exceptions:</b>

 * System.ArgumentException:
Either  <i>offset</i>
 or  <i>count</i>
 is less than 0 or greater than  <i>str</i>
 's length, or  <i>str</i>
 's length minus  <i>offset</i>
 is less than  <i>count</i>
.

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

<a id="ParseJSONNumber_string_int_int_PeterO_Cbor_JSONOptions"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str,
        int offset,
        int count,
        PeterO.Cbor.JSONOptions options);

Parses a number whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless there is only one digit and that digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A text string representing a valid JSON number is not allowed to contain white space characters, including spaces.

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

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

 * System.ArgumentException:
Unsupported conversion kind.

<a id="ParseJSONNumber_string_PeterO_Cbor_JSONOptions"></a>
### ParseJSONNumber

    public static PeterO.Cbor.CBORObject ParseJSONNumber(
        string str,
        PeterO.Cbor.JSONOptions options);

Parses a number whose format follows the JSON specification (RFC 8259) and converts that number to a CBOR object.

Roughly speaking, a valid JSON number consists of an optional minus sign, one or more basic digits (starting with 1 to 9 unless there is only one digit and that digit is 0), an optional decimal point (".", full stop) with one or more basic digits, and an optional letter E or e with an optional plus or minus sign and one or more basic digits (the exponent). A text string representing a valid JSON number is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>str</i>: A text string to parse as a JSON number.

 * <i>options</i>: An object containing options to control how JSON numbers are decoded to CBOR objects. Can be null, in which case a JSONOptions object with all default properties is used instead.

<b>Return Value:</b>

A CBOR object that represents the parsed number. Returns null if the parsing fails, including if the string is null or empty.
