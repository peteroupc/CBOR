## PeterO.BigInteger

    public sealed class BigInteger :
        System.IComparable,
        System.IEquatable

<b>Deprecated.</b> Use EInteger from PeterO.Numbers/com.upokecenter.numbers and the output of this class's ToString method.

<b>This class is largely obsolete. It will be replaced by a new version of this class in a different namespace/package and library, called  `PeterO.Numbers.EInteger`  in the<a href="https://www.nuget.org/packages/PeterO.Numbers"> `PeterO.Numbers` </a>library (in .NET), or  `com.upokecenter.numbers.EInteger`  in the<a href="https://github.com/peteroupc/numbers-java"> `com.github.peteroupc/numbers` </a>artifact (in Java). This new class can be used in the `CBORObject.FromObject(object)`  method (by including the new library in your code, among other things).</b>

 An arbitrary-precision integer.<b>Thread safety:</b> Instances of this class are immutable, so they are inherently safe for use by multiple threads. Multiple instances of this object with the same value are interchangeable, but they should be compared using the "Equals" method rather than the "==" operator.

<b>This class is largely obsolete. It will be replaced by a new version of this class in a different namespace/package and library, called  `PeterO.Numbers.EInteger`  in the<a href="https://www.nuget.org/packages/PeterO.Numbers"> `PeterO.Numbers` </a>library (in .NET), or  `com.upokecenter.numbers.EInteger`  in the<a href="https://github.com/peteroupc/numbers-java"> `com.github.peteroupc/numbers` </a>artifact (in Java). This new class can be used in the `CBORObject.FromObject(object)`  method (by including the new library in your code, among other things).</b>

 An arbitrary-precision integer.<b>Thread safety:</b> Instances of this class are immutable, so they are inherently safe for use by multiple threads. Multiple instances of this object with the same value are interchangeable, but they should be compared using the "Equals" method rather than the "==" operator.

### ONE

    public static readonly PeterO.BigInteger ONE;

BigInteger for the number one.

### One

    public static PeterO.BigInteger One { get; }

Gets the arbitrary-precision integer for one.

<b>Returns:</b>

The arbitrary-precision integer for one.

### Zero

    public static PeterO.BigInteger Zero { get; }

Gets the arbitrary-precision integer for zero.

<b>Returns:</b>

The arbitrary-precision integer for zero.

### bitLength

    public int bitLength();

Finds the minimum number of bits needed to represent this object's value, except for its sign. If the value is negative, finds the number of bits in a value equal to this object's absolute value minus 1.

<b>Return Value:</b>

The number of bits in this object's value. Returns 0 if this object's value is 0 or negative 1.

### CompareTo

    public sealed int CompareTo(
        PeterO.BigInteger other);

Not documented yet.

<b>Parameters:</b>

 * <i>other</i>: A BigInteger object.

<b>Return Value:</b>

A 32-bit signed integer.

### Equals

    public override bool Equals(
        object obj);

Determines whether this object and another object are equal.

<b>Parameters:</b>

 * <i>obj</i>: An arbitrary object.

<b>Return Value:</b>

 `true`  if this object and another object are equal; otherwise,  `false` .

### Equals

    public sealed bool Equals(
        PeterO.BigInteger other);

Returns whether this number's value equals another number's value.

<b>Parameters:</b>

 * <i>other</i>: An arbitrary-precision integer.

<b>Return Value:</b>

 `true`  if this number's value equals another number's value; otherwise,  `false` .

### fromBytes

    public static PeterO.BigInteger fromBytes(
        byte[] bytes,
        bool littleEndian);

Initializes an arbitrary-precision integer from an array of bytes.

<b>Parameters:</b>

 * <i>bytes</i>: A byte array consisting of the two's-complement form of the arbitrary-precision integer to create. The last byte contains the lowest 8-bits, the next-to-last contains the next lowest 8 bits, and so on. To encode negative numbers, take the absolute value of the number, subtract by 1, encode the number into bytes, XOR each byte, and if the most-significant bit of the first byte isn't set, add an additional byte at the start with the value 255. For little-endian, the byte order is reversed from the byte order just discussed.

 * <i>littleEndian</i>: If true, the byte order is little-endian, or least-significant-byte first. If false, the byte order is big-endian, or most-significant-byte first.

<b>Return Value:</b>

An arbitrary-precision integer. Returns 0 if the byte array's length is 0.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>bytes</i>
 is null.

### fromRadixString

    public static PeterO.BigInteger fromRadixString(
        string str,
        int radix);

Converts a string to an arbitrary-precision integer.

The following example (C#) converts a number in the orm of a hex string to a big integer.    public static arbitrary-precision integer HexToBigInteger(string
    hexString) {  // Parse the hexadecimal string as a big integer. Will  //
    throw a FormatException if the parsing fails var bigInteger =
    arbitrary-precision integer.fromRadixString(hexString, 16);  // Optional:
    Check if the parsed integer is negative if (bigInteger.Sign < 0) {
    throw new FormatException("negative hex string"); } return bigInteger; }

<b>Parameters:</b>

 * <i>str</i>: A text string. The string must contain only characters allowed by the given radix, except that it may start with a minus sign ("-", U+002D) to indicate a negative number. The string is not allowed to contain white space characters, including spaces.

 * <i>radix</i>: A base from 2 to 36. Depending on the radix, the string can use the basic digits 0 to 9 (U+0030 to U+0039) and then the basic letters A to Z (U+0041 to U+005A). For example, 0-9 in radix 10, and 0-9, then A-F in radix 16.

<b>Return Value:</b>

An arbitrary-precision integer with the same value as given in the string.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
 is null.

 * System.ArgumentException:
The parameter <i>radix</i>
 is less than 2 or greater than 36.

 * System.FormatException:
The string is empty or in an invalid format.

### fromString

    public static PeterO.BigInteger fromString(
        string str);

Converts a string to an arbitrary-precision integer.

<b>Parameters:</b>

 * <i>str</i>: A text string. The string must contain only basic digits 0 to 9 (U+0030 to U+0039), except that it may start with a minus sign ("-", U+002D) to indicate a negative number. The string is not allowed to contain white space characters, including spaces.

<b>Return Value:</b>

An arbitrary-precision integer with the same value as given in the string.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
 is null.

 * System.FormatException:
The parameter  <i>str</i>
 is in an invalid format.

### GetHashCode

    public override int GetHashCode();

Returns the hash code for this instance.

<b>Return Value:</b>

A 32-bit signed integer.

### toBytes

    public byte[] toBytes(
        bool littleEndian);

Returns a byte array of this object's value. The byte array will take the form of the number's two' s-complement representation, using the fewest bytes necessary to represent its value unambiguously. If this value is negative, the bits that appear "before" the most significant bit of the number will be all ones.

<b>Parameters:</b>

 * <i>littleEndian</i>: If true, the least significant bits will appear first.

<b>Return Value:</b>

A byte array. If this value is 0, returns a byte array with the single element 0.

### toRadixString

    public string toRadixString(
        int radix);

Generates a string representing the value of this object, in the given radix.

<b>Parameters:</b>

 * <i>radix</i>: A radix from 2 through 36. For example, to generate a hexadecimal (base-16) string, specify 16. To generate a decimal (base-10) string, specify 10.

<b>Return Value:</b>

A string representing the value of this object. If this value is 0, returns "0". If negative, the string will begin with a hyphen/minus ("-"). Depending on the radix, the string will use the basic digits 0 to 9 (U+0030 to U+0039) and then the basic letters A to Z (U+0041 to U+005A). For example, 0-9 in radix 10, and 0-9, then A-F in radix 16.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter "index" is less than 0, "endIndex" is less than 0, or either is greater than the string's length, or "endIndex" is less than "index" ; or radix is less than 2 or greater than 36.

### ToString

    public override string ToString();

Converts this object to a text string in base 10.

<b>Return Value:</b>

A string representation of this object. If negative, the string will begin with a minus sign ("-", U+002D). The string will use the basic digits 0 to 9 (U+0030 to U+0039).

### valueOf

    public static PeterO.BigInteger valueOf(
        long longerValue);

Converts a 64-bit signed integer to a big integer.

<b>Parameters:</b>

 * <i>longerValue</i>: A 64-bit signed integer.

<b>Return Value:</b>

An arbitrary-precision integer with the same value as the 64-bit number.
