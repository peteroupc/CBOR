## PeterO.DataUtilities

    public static class DataUtilities

 Contains methods useful for reading and writing strings. It is designed to have no dependencies other than the basic runtime class library. Many of these methods work with text encoded in UTF-8, an encoding form of the Unicode Standard which uses one byte to encode the most basic characters and two to four bytes to encode other characters. For example, the  `GetUtf8`  method converts a text string to an array of bytes in UTF-8.

 In C# and Java, text strings are represented as sequences of 16-bit values called  `char`  s. These sequences are well-formed under UTF-16, a 16-bit encoding form of Unicode, except if they contain unpaired surrogate code points. (A surrogate code point is used to encode supplementary characters, those with code points U+10000 or higher, in UTF-16. A surrogate pair is a high surrogate [U+D800 to U+DBFF] followed by a low surrogate [U+DC00 to U+DFFF]. An unpaired surrogate code point is a surrogate not appearing in a surrogate pair.) Many of the methods in this class allow setting the behavior to follow when unpaired surrogate code points are found in text strings, such as throwing an error or treating the unpaired surrogate as a replacement character (U+FFFD).

### Member Summary
* <code>[CodePointAt(string, int)](#CodePointAt_string_int)</code> - Gets the Unicode code point at the given index of the string.
* <code>[CodePointAt(string, int, int)](#CodePointAt_string_int_int)</code> - Gets the Unicode code point at the given index of the string.
* <code>[CodePointBefore(string, int)](#CodePointBefore_string_int)</code> - Gets the Unicode code point just before the given index of the string.
* <code>[CodePointBefore(string, int, int)](#CodePointBefore_string_int_int)</code> - Gets the Unicode code point just before the given index of the string.
* <code>[CodePointCompare(string, string)](#CodePointCompare_string_string)</code> - Compares two strings in Unicode code point order.
* <code>[CodePointLength(string)](#CodePointLength_string)</code> - Finds the number of Unicode code points in the given text string.
* <code>[GetUtf8Bytes(string, bool)](#GetUtf8Bytes_string_bool)</code> - Encodes a string in UTF-8 as a byte array.
* <code>[GetUtf8Bytes(string, bool, bool)](#GetUtf8Bytes_string_bool_bool)</code> - Encodes a string in UTF-8 as a byte array.
* <code>[GetUtf8Length(string, bool)](#GetUtf8Length_string_bool)</code> - Calculates the number of bytes needed to encode a string in UTF-8.
* <code>[GetUtf8String(byte[], bool)](#GetUtf8String_byte_bool)</code> - Generates a text string from a UTF-8 byte array.
* <code>[GetUtf8String(byte[], int, int, bool)](#GetUtf8String_byte_int_int_bool)</code> - Generates a text string from a portion of a UTF-8 byte array.
* <code>[ReadUtf8(System.IO.Stream, int, System.Text.StringBuilder, bool)](#ReadUtf8_System_IO_Stream_int_System_Text_StringBuilder_bool)</code> - Reads a string in UTF-8 encoding from a data stream.
* <code>[ReadUtf8FromBytes(byte[], int, int, System.Text.StringBuilder, bool)](#ReadUtf8FromBytes_byte_int_int_System_Text_StringBuilder_bool)</code> - Reads a string in UTF-8 encoding from a byte array.
* <code>[ReadUtf8ToString(System.IO.Stream)](#ReadUtf8ToString_System_IO_Stream)</code> - Reads a string in UTF-8 encoding from a data stream in full and returns that string.
* <code>[ReadUtf8ToString(System.IO.Stream, int, bool)](#ReadUtf8ToString_System_IO_Stream_int_bool)</code> - Reads a string in UTF-8 encoding from a data stream and returns that string.
* <code>[ToLowerCaseAscii(string)](#ToLowerCaseAscii_string)</code> - Returns a string with the basic upper-case letters A to Z (U+0041 to U+005A) converted to lower-case.
* <code>[ToUpperCaseAscii(string)](#ToUpperCaseAscii_string)</code> - Returns a string with the basic lower-case letters A to Z (U+0061 to U+007A) converted to upper-case.
* <code>[WriteUtf8(string, int, int, System.IO.Stream, bool)](#WriteUtf8_string_int_int_System_IO_Stream_bool)</code> - Writes a portion of a string in UTF-8 encoding to a data stream.
* <code>[WriteUtf8(string, int, int, System.IO.Stream, bool, bool)](#WriteUtf8_string_int_int_System_IO_Stream_bool_bool)</code> - Writes a portion of a string in UTF-8 encoding to a data stream.
* <code>[WriteUtf8(string, System.IO.Stream, bool)](#WriteUtf8_string_System_IO_Stream_bool)</code> - Writes a string in UTF-8 encoding to a data stream.

<a id="CodePointAt_string_int"></a>
### CodePointAt

    public static int CodePointAt(
        string str,
        int index);

 Gets the Unicode code point at the given index of the string.

      <b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

 * <i>index</i>: Index of the current position into the string.

<b>Return Value:</b>

The Unicode code point at the given position. Returns -1 if  <i>index</i>
 is 0 or less, or is greater than the string's length. Returns the replacement character (U + FFFD) if the code point at that position is an unpaired surrogate code point. If the return value is 65536 (0x10000) or greater, the code point takes up two UTF-16 code units.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

<a id="CodePointAt_string_int_int"></a>
### CodePointAt

    public static int CodePointAt(
        string str,
        int index,
        int surrogateBehavior);

 Gets the Unicode code point at the given index of the string.

       The following example shows how to iterate a text string code point by code point, terminating the loop when an unpaired surrogate is found.

    for (var i = 0;i<str.Length; ++i) { int codePoint =
                DataUtilities.CodePointAt(str, i, 2); if (codePoint < 0) { break; /*
                Unpaired surrogate */ } Console.WriteLine("codePoint:"+codePoint); if
                (codePoint >= 0x10000) { i++; /* Supplementary code point */ } }

 .

  <b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

 * <i>index</i>: Index of the current position into the string.

 * <i>surrogateBehavior</i>: Specifies what kind of value to return if the code point at the given index is an unpaired surrogate code point: if 0, return the replacement character (U + FFFD); if 1, return the value of the surrogate code point; if neither 0 nor 1, return -1.

<b>Return Value:</b>

The Unicode code point at the given position. Returns -1 if  <i>index</i>
 is 0 or less, or is greater than the string's length. Returns a value as specified under  <i>surrogateBehavior</i>
 if the code point at that position is an unpaired surrogate code point. If the return value is 65536 (0x10000) or greater, the code point takes up two UTF-16 code units.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

<a id="CodePointBefore_string_int"></a>
### CodePointBefore

    public static int CodePointBefore(
        string str,
        int index);

 Gets the Unicode code point just before the given index of the string.

      <b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

 * <i>index</i>: Index of the current position into the string.

<b>Return Value:</b>

The Unicode code point at the previous position. Returns -1 if  <i>index</i>
 is 0 or less, or is greater than the string's length. Returns the replacement character (U + FFFD) if the code point at the previous position is an unpaired surrogate code point. If the return value is 65536 (0x10000) or greater, the code point takes up two UTF-16 code units.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

<a id="CodePointBefore_string_int_int"></a>
### CodePointBefore

    public static int CodePointBefore(
        string str,
        int index,
        int surrogateBehavior);

 Gets the Unicode code point just before the given index of the string.

       <b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

 * <i>index</i>: Index of the current position into the string.

 * <i>surrogateBehavior</i>: Specifies what kind of value to return if the previous code point is an unpaired surrogate code point: if 0, return the replacement character (U + FFFD); if 1, return the value of the surrogate code point; if neither 0 nor 1, return -1.

<b>Return Value:</b>

The Unicode code point at the previous position. Returns -1 if  <i>index</i>
 is 0 or less, or is greater than the string's length. Returns a value as specified under  <i>surrogateBehavior</i>
 if the code point at the previous position is an unpaired surrogate code point. If the return value is 65536 (0x10000) or greater, the code point takes up two UTF-16 code units.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

<a id="CodePointCompare_string_string"></a>
### CodePointCompare

    public static int CodePointCompare(
        string strA,
        string strB);

 Compares two strings in Unicode code point order. Unpaired surrogate code points are treated as individual code points.

     <b>Parameters:</b>

 * <i>strA</i>: The first string. Can be null.

 * <i>strB</i>: The second string. Can be null.

<b>Return Value:</b>

A value indicating which string is " less" or " greater" . 0: Both strings are equal or null. Less than 0: a is null and b isn't; or the first code point that's different is less in A than in B; or b starts with a and is longer than a. Greater than 0: b is null and a isn't; or the first code point that's different is greater in A than in B; or a starts with b and is longer than b.

<a id="CodePointLength_string"></a>
### CodePointLength

    public static int CodePointLength(
        string str);

 Finds the number of Unicode code points in the given text string. Unpaired surrogate code points increase this number by 1. This is not necessarily the length of the string in "char" s.

     <b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

<b>Return Value:</b>

The number of Unicode code points in the given string.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

<a id="GetUtf8Bytes_string_bool"></a>
### GetUtf8Bytes

    public static byte[] GetUtf8Bytes(
        string str,
        bool replace);

 Encodes a string in UTF-8 as a byte array. This method does not insert a byte-order mark (U+FEFF) at the beginning of the encoded byte array.

 REMARK: It is not recommended to use  `Encoding.UTF8.GetBytes`  in.NET, or the  `getBytes()`  method in Java to do this. For instance,  `getBytes()`  encodes text strings in a default (so not fixed) character encoding, which can be undesirable.

       <b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

 * <i>replace</i>: If true, replaces unpaired surrogate code points with the replacement character (U + FFFD). If false, stops processing when an unpaired surrogate code point is seen.

<b>Return Value:</b>

The string encoded in UTF-8.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

 * System.ArgumentException:
The string contains an unpaired surrogate code point and  <i>replace</i>
 is false, or an internal error occurred.

<a id="GetUtf8Bytes_string_bool_bool"></a>
### GetUtf8Bytes

    public static byte[] GetUtf8Bytes(
        string str,
        bool replace,
        bool lenientLineBreaks);

 Encodes a string in UTF-8 as a byte array. This method does not insert a byte-order mark (U+FEFF) at the beginning of the encoded byte array.

 REMARK: It is not recommended to use  `Encoding.UTF8.GetBytes`  in.NET, or the  `getBytes()`  method in Java to do this. For instance,  `getBytes()`  encodes text strings in a default (so not fixed) character encoding, which can be undesirable.

        <b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

 * <i>replace</i>: If true, replaces unpaired surrogate code points with the replacement character (U + FFFD). If false, stops processing when an unpaired surrogate code point is seen.

 * <i>lenientLineBreaks</i>: If true, replaces carriage return (CR) not followed by line feed (LF) and LF not preceded by CR with CR-LF pairs.

<b>Return Value:</b>

The string encoded in UTF-8.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

 * System.ArgumentException:
The string contains an unpaired surrogate code point and  <i>replace</i>
 is false, or an internal error occurred.

<a id="GetUtf8Length_string_bool"></a>
### GetUtf8Length

    public static long GetUtf8Length(
        string str,
        bool replace);

 Calculates the number of bytes needed to encode a string in UTF-8.

      <b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

 * <i>replace</i>: If true, treats unpaired surrogate code points as having 3 UTF-8 bytes (the UTF-8 length of the replacement character U + FFFD).

<b>Return Value:</b>

The number of bytes needed to encode the given string in UTF-8, or -1 if the string contains an unpaired surrogate code point and  <i>replace</i>
 is false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

<a id="GetUtf8String_byte_bool"></a>
### GetUtf8String

    public static string GetUtf8String(
        byte[] bytes,
        bool replace);

 Generates a text string from a UTF-8 byte array.

       <b>Parameters:</b>

 * <i>bytes</i>: A byte array containing text encoded in UTF-8.

 * <i>replace</i>: If true, replaces invalid encoding with the replacement character (U + FFFD). If false, stops processing when invalid UTF-8 is seen.

<b>Return Value:</b>

A string represented by the UTF-8 byte array.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 is null.

 * System.ArgumentException:
The string is not valid UTF-8 and  <i>replace</i>
 is false.

<a id="GetUtf8String_byte_int_int_bool"></a>
### GetUtf8String

    public static string GetUtf8String(
        byte[] bytes,
        int offset,
        int bytesCount,
        bool replace);

 Generates a text string from a portion of a UTF-8 byte array.

          <b>Parameters:</b>

 * <i>bytes</i>: A byte array containing text encoded in UTF-8.

 * <i>offset</i>: Offset into the byte array to start reading.

 * <i>bytesCount</i>: Length, in bytes, of the UTF-8 text string.

 * <i>replace</i>: If true, replaces invalid encoding with the replacement character (U + FFFD). If false, stops processing when invalid UTF-8 is seen.

<b>Return Value:</b>

A string represented by the UTF-8 byte array.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 is null.

 * System.ArgumentException:
The portion of the byte array is not valid UTF-8 and  <i>replace</i>
 is false.

 * System.ArgumentException:
The parameter  <i>offset</i>
 is less than 0,  <i>bytesCount</i>
 is less than 0, or offset plus bytesCount is greater than the length of "data" .

<a id="ReadUtf8_System_IO_Stream_int_System_Text_StringBuilder_bool"></a>
### ReadUtf8

    public static int ReadUtf8(
        System.IO.Stream stream,
        int bytesCount,
        System.Text.StringBuilder builder,
        bool replace);

 Reads a string in UTF-8 encoding from a data stream.

         <b>Parameters:</b>

 * <i>stream</i>: A readable data stream.

 * <i>bytesCount</i>: The length, in bytes, of the string. If this is less than 0, this function will read until the end of the stream.

 * <i>builder</i>: A string builder object where the resulting string will be stored.

 * <i>replace</i>: If true, replaces invalid encoding with the replacement character (U + FFFD). If false, stops processing when an unpaired surrogate code point is seen.

<b>Return Value:</b>

0 if the entire string was read without errors, -1 if the string is not valid UTF-8 and  <i>replace</i>
 is false, or -2 if the end of the stream was reached before the last character was read completely (which is only the case if  <i>bytesCount</i>
 is 0 or greater).

<b>Exceptions:</b>

 * System.IO.IOException:
An I/O error occurred.

 * System.ArgumentNullException:
The parameter  <i>stream</i>
 is null or  <i>builder</i>
 is null.

<a id="ReadUtf8FromBytes_byte_int_int_System_Text_StringBuilder_bool"></a>
### ReadUtf8FromBytes

    public static int ReadUtf8FromBytes(
        byte[] data,
        int offset,
        int bytesCount,
        System.Text.StringBuilder builder,
        bool replace);

 Reads a string in UTF-8 encoding from a byte array.

          <b>Parameters:</b>

 * <i>data</i>: A byte array containing a UTF-8 text string.

 * <i>offset</i>: Offset into the byte array to start reading.

 * <i>bytesCount</i>: Length, in bytes, of the UTF-8 text string.

 * <i>builder</i>: A string builder object where the resulting string will be stored.

 * <i>replace</i>: If true, replaces invalid encoding with the replacement character (U + FFFD). If false, stops processing when invalid UTF-8 is seen.

<b>Return Value:</b>

0 if the entire string was read without errors, or -1 if the string is not valid UTF-8 and  <i>replace</i>
 is false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>data</i>
 is null or  <i>builder</i>
 is null.

 * System.ArgumentException:
The parameter  <i>offset</i>
 is less than 0,  <i>bytesCount</i>
 is less than 0, or offset plus bytesCount is greater than the length of  <i>data</i>
.

<a id="ReadUtf8ToString_System_IO_Stream"></a>
### ReadUtf8ToString

    public static string ReadUtf8ToString(
        System.IO.Stream stream);

 Reads a string in UTF-8 encoding from a data stream in full and returns that string. Replaces invalid encoding with the replacement character (U+FFFD).

      <b>Parameters:</b>

 * <i>stream</i>: A readable data stream.

<b>Return Value:</b>

The string read.

<b>Exceptions:</b>

 * System.IO.IOException:
An I/O error occurred.

 * System.ArgumentNullException:
The parameter  <i>stream</i>
 is null.

<a id="ReadUtf8ToString_System_IO_Stream_int_bool"></a>
### ReadUtf8ToString

    public static string ReadUtf8ToString(
        System.IO.Stream stream,
        int bytesCount,
        bool replace);

 Reads a string in UTF-8 encoding from a data stream and returns that string.

        <b>Parameters:</b>

 * <i>stream</i>: A readable data stream.

 * <i>bytesCount</i>: The length, in bytes, of the string. If this is less than 0, this function will read until the end of the stream.

 * <i>replace</i>: If true, replaces invalid encoding with the replacement character (U + FFFD). If false, throws an error if an unpaired surrogate code point is seen.

<b>Return Value:</b>

The string read.

<b>Exceptions:</b>

 * System.IO.IOException:
An I/O error occurred; or, the string is not valid UTF-8 and  <i>replace</i>
 is false.

 * System.ArgumentNullException:
The parameter  <i>stream</i>
 is null.

<a id="ToLowerCaseAscii_string"></a>
### ToLowerCaseAscii

    public static string ToLowerCaseAscii(
        string str);

 Returns a string with the basic upper-case letters A to Z (U+0041 to U+005A) converted to lower-case. Other characters remain unchanged.

    <b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

<b>Return Value:</b>

The converted string, or null if  <i>str</i>
 is null.

<a id="ToUpperCaseAscii_string"></a>
### ToUpperCaseAscii

    public static string ToUpperCaseAscii(
        string str);

 Returns a string with the basic lower-case letters A to Z (U+0061 to U+007A) converted to upper-case. Other characters remain unchanged.

    <b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

<b>Return Value:</b>

The converted string, or null if  <i>str</i>
 is null.

<a id="WriteUtf8_string_int_int_System_IO_Stream_bool"></a>
### WriteUtf8

    public static int WriteUtf8(
        string str,
        int offset,
        int length,
        System.IO.Stream stream,
        bool replace);

 Writes a portion of a string in UTF-8 encoding to a data stream.

           <b>Parameters:</b>

 * <i>str</i>: A string to write.

 * <i>offset</i>: The Index starting at 0 where the string portion to write begins.

 * <i>length</i>: The length of the string portion to write.

 * <i>stream</i>: A writable data stream.

 * <i>replace</i>: If true, replaces unpaired surrogate code points with the replacement character (U + FFFD). If false, stops processing when an unpaired surrogate code point is seen.

<b>Return Value:</b>

0 if the entire string portion was written; or -1 if the string portion contains an unpaired surrogate code point and  <i>replace</i>
 is false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null or  <i>stream</i>
 is null.

 * System.IO.IOException:
An I/O error occurred.

 * System.ArgumentException:
Either  <i>offset</i>
 or  <i>length</i>
 is less than 0 or greater than  <i>str</i>
 's length, or  <i>str</i>
 's length minus  <i>offset</i>
 is less than  <i>length</i>
.

<a id="WriteUtf8_string_int_int_System_IO_Stream_bool_bool"></a>
### WriteUtf8

    public static int WriteUtf8(
        string str,
        int offset,
        int length,
        System.IO.Stream stream,
        bool replace,
        bool lenientLineBreaks);

 Writes a portion of a string in UTF-8 encoding to a data stream.

            <b>Parameters:</b>

 * <i>str</i>: A string to write.

 * <i>offset</i>: The Index starting at 0 where the string portion to write begins.

 * <i>length</i>: The length of the string portion to write.

 * <i>stream</i>: A writable data stream.

 * <i>replace</i>: If true, replaces unpaired surrogate code points with the replacement character (U + FFFD). If false, stops processing when an unpaired surrogate code point is seen.

 * <i>lenientLineBreaks</i>: If true, replaces carriage return (CR) not followed by line feed (LF) and LF not preceded by CR with CR-LF pairs.

<b>Return Value:</b>

0 if the entire string portion was written; or -1 if the string portion contains an unpaired surrogate code point and  <i>replace</i>
 is false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null or  <i>stream</i>
 is null.

 * System.ArgumentException:
The parameter  <i>offset</i>
 is less than 0,  <i>length</i>
 is less than 0, or  <i>offset</i>
 plus  <i>length</i>
 is greater than the string's length.

 * System.IO.IOException:
An I/O error occurred.

<a id="WriteUtf8_string_System_IO_Stream_bool"></a>
### WriteUtf8

    public static int WriteUtf8(
        string str,
        System.IO.Stream stream,
        bool replace);

 Writes a string in UTF-8 encoding to a data stream.

        <b>Parameters:</b>

 * <i>str</i>: A string to write.

 * <i>stream</i>: A writable data stream.

 * <i>replace</i>: If true, replaces unpaired surrogate code points with the replacement character (U + FFFD). If false, stops processing when an unpaired surrogate code point is seen.

<b>Return Value:</b>

0 if the entire string was written; or -1 if the string contains an unpaired surrogate code point and  <i>replace</i>
 is false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null or  <i>stream</i>
 is null.

 * System.IO.IOException:
An I/O error occurred.
