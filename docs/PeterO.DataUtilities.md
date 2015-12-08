## PeterO.DataUtilities

    public static class DataUtilities

Contains methods useful for reading and writing strings. It is designed to have no dependencies other than the basic runtime class library.

### CodePointAt

    public static int CodePointAt(
        string str,
        int index);

Gets the Unicode code point at the given index of the string.

<b>Parameters:</b>

 * <i>str</i>: A string.

 * <i>index</i>: Index of the current position into the string.

<b>Returns:</b>

The Unicode code point at the given position. Returns -1 if  <i>index</i>
 is less than 0, or is the string's length or greater. Returns the replacement character (U + FFFD) if the current character is an unpaired surrogate code point.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

### CodePointAt

    public static int CodePointAt(
        string str,
        int index,
        int surrogateBehavior);

Gets the Unicode code point at the given index of the string.

<b>Parameters:</b>

 * <i>str</i>: A string.

 * <i>index</i>: Index of the current position into the string.

 * <i>surrogateBehavior</i>: Specifies what kind of value to return if the previous character is an unpaired surrogate code point: if 0, return the replacement character (U + FFFD); if 1, return the value of the surrogate code point; if neither 0 nor 1, return -1.

<b>Returns:</b>

The Unicode code point at the current position. Returns -1 if  <i>index</i>
 is less than 0, or is the string's length or greater. Returns a value as specified under  <i>surrogateBehavior</i>
 if the previous character is an unpaired surrogate code point.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

### CodePointBefore

    public static int CodePointBefore(
        string str,
        int index);

Gets the Unicode code point just before the given index of the string.

<b>Parameters:</b>

 * <i>str</i>: A string.

 * <i>index</i>: Index of the current position into the string.

<b>Returns:</b>

The Unicode code point at the previous position. Returns -1 if  <i>index</i>
 is 0 or less, or is greater than the string's length. Returns the replacement character (U + FFFD) if the previous character is an unpaired surrogate code point.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

### CodePointBefore

    public static int CodePointBefore(
        string str,
        int index,
        int surrogateBehavior);

Gets the Unicode code point just before the given index of the string.

<b>Parameters:</b>

 * <i>str</i>: A string.

 * <i>index</i>: Index of the current position into the string.

 * <i>surrogateBehavior</i>: Specifies what kind of value to return if the previous character is an unpaired surrogate code point: if 0, return the replacement character (U + FFFD); if 1, return the value of the surrogate code point; if neither 0 nor 1, return -1.

<b>Returns:</b>

The Unicode code point at the previous position. Returns -1 if  <i>index</i>
 is 0 or less, or is greater than the string's length. Returns a value as specified under  <i>surrogateBehavior</i>
 if the previous character is an unpaired surrogate code point.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

### CodePointCompare

    public static int CodePointCompare(
        string strA,
        string strB);

Compares two strings in Unicode code point order. Unpaired surrogates are treated as individual code points.

<b>Parameters:</b>

 * <i>strA</i>: The first string. Can be null.

 * <i>strB</i>: The second string. Can be null.

<b>Returns:</b>

A value indicating which string is " less" or " greater" . 0: Both strings are equal or null. Less than 0: a is null and b isn't; or the first code point that's different is less in A than in B; or b starts with a and is longer than a. Greater than 0: b is null and a isn't; or the first code point that's different is greater in A than in B; or a starts with b and is longer than b.

### GetUtf8Bytes

    public static byte[] GetUtf8Bytes(
        string str,
        bool replace);

Encodes a string in UTF-8 as a byte array.

<b>Parameters:</b>

 * <i>str</i>: A text string.

 * <i>replace</i>: If true, replaces unpaired surrogate code points with the replacement character (U + FFFD). If false, stops processing when an unpaired surrogate code point is seen.

<b>Returns:</b>

The string encoded in UTF-8.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

 * System.ArgumentException:
The string contains an unpaired surrogate code point and  <i>replace</i>
 is false, or an internal error occurred.

### GetUtf8Bytes

    public static byte[] GetUtf8Bytes(
        string str,
        bool replace,
        bool lenientLineBreaks);

Encodes a string in UTF-8 as a byte array.

<b>Parameters:</b>

 * <i>str</i>: A text string.

 * <i>replace</i>: If true, replaces unpaired surrogate code points with the replacement character (U + FFFD). If false, stops processing when an unpaired surrogate code point is seen.

 * <i>lenientLineBreaks</i>: If true, replaces carriage return (CR) not followed by line feed (LF) and LF not preceded by CR with CR-LF pairs.

<b>Returns:</b>

The string encoded in UTF-8.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

 * System.ArgumentException:
The string contains an unpaired surrogate code point and  <i>replace</i>
 is false, or an internal error occurred.

### GetUtf8Length

    public static long GetUtf8Length(
        string str,
        bool replace);

Calculates the number of bytes needed to encode a string in UTF-8.

<b>Parameters:</b>

 * <i>str</i>: A String object.

 * <i>replace</i>: If true, treats unpaired surrogate code points as having 3 UTF-8 bytes (the UTF-8 length of the replacement character U + FFFD).

<b>Returns:</b>

The number of bytes needed to encode the given string in UTF-8, or -1 if the string contains an unpaired surrogate code point and  <i>replace</i>
 is false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

### GetUtf8String

    public static string GetUtf8String(
        byte[] bytes,
        bool replace);

Generates a text string from a UTF-8 byte array.

<b>Parameters:</b>

 * <i>bytes</i>: A byte array containing text encoded in UTF-8.

 * <i>replace</i>: If true, replaces invalid encoding with the replacement character (U + FFFD). If false, stops processing when invalid UTF-8 is seen.

<b>Returns:</b>

A string represented by the UTF-8 byte array.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 is null.

 * System.ArgumentException:
The string is not valid UTF-8 and  <i>replace</i>
 is false.

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

 * <i>bytesCount</i>: Length, in bytes, of the UTF-8 string.

 * <i>replace</i>: If true, replaces invalid encoding with the replacement character (U + FFFD). If false, stops processing when invalid UTF-8 is seen.

<b>Returns:</b>

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

<b>Returns:</b>

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

### ReadUtf8FromBytes

    public static int ReadUtf8FromBytes(
        byte[] data,
        int offset,
        int bytesCount,
        System.Text.StringBuilder builder,
        bool replace);

Reads a string in UTF-8 encoding from a byte array.

<b>Parameters:</b>

 * <i>data</i>: A byte array containing a UTF-8 string.

 * <i>offset</i>: Offset into the byte array to start reading.

 * <i>bytesCount</i>: Length, in bytes, of the UTF-8 string.

 * <i>builder</i>: A string builder object where the resulting string will be stored.

 * <i>replace</i>: If true, replaces invalid encoding with the replacement character (U + FFFD). If false, stops processing when invalid UTF-8 is seen.

<b>Returns:</b>

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

### ReadUtf8ToString

    public static string ReadUtf8ToString(
        System.IO.Stream stream);

Reads a string in UTF-8 encoding from a data stream in full and returns that string. Replaces invalid encoding with the replacement character (U + FFFD).

<b>Parameters:</b>

 * <i>stream</i>: A readable data stream.

<b>Returns:</b>

The string read.

<b>Exceptions:</b>

 * System.IO.IOException:
An I/O error occurred.

 * System.ArgumentNullException:
The parameter  <i>stream</i>
 is null.

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

<b>Returns:</b>

The string read.

<b>Exceptions:</b>

 * System.IO.IOException:
An I/O error occurred; or, the string is not valid UTF-8 and  <i>replace</i>
 is false.

 * System.ArgumentNullException:
The parameter  <i>stream</i>
 is null.

### ToLowerCaseAscii

    public static string ToLowerCaseAscii(
        string str);

Returns a string with the basic upper-case letters A to Z (U + 0041 to U + 005A) converted to lower-case. Other characters remain unchanged.

<b>Parameters:</b>

 * <i>str</i>: A string.

<b>Returns:</b>

The converted string, or null if  <i>str</i>
 is null.

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

 * <i>offset</i>: The zero-based index where the string portion to write begins.

 * <i>length</i>: The length of the string portion to write.

 * <i>stream</i>: A writable data stream.

 * <i>replace</i>: If true, replaces unpaired surrogate code points with the replacement character (U + FFFD). If false, stops processing when an unpaired surrogate code point is seen.

<b>Returns:</b>

0 if the entire string portion was written; or -1 if the string portion contains an unpaired surrogate code point and <i>replace</i>
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

 * <i>offset</i>: The zero-based index where the string portion to write begins.

 * <i>length</i>: The length of the string portion to write.

 * <i>stream</i>: A writable data stream.

 * <i>replace</i>: If true, replaces unpaired surrogate code points with the replacement character (U + FFFD). If false, stops processing when an unpaired surrogate code point is seen.

 * <i>lenientLineBreaks</i>: If true, replaces carriage return (CR) not followed by line feed (LF) and LF not preceded by CR with CR-LF pairs.

<b>Returns:</b>

0 if the entire string portion was written; or -1 if the string portion contains an unpaired surrogate code point and <i>replace</i>
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

<b>Returns:</b>

0 if the entire string was written; or -1 if the string contains an unpaired surrogate code point and  <i>replace</i>
 is false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null or  <i>stream</i>
 is null.

 * System.IO.IOException:
An I/O error occurred.
