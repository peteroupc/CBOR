## PeterO.BigInteger


    public sealed class BigInteger :
        System.IComparable,
        System.IEquatable


An arbitrary-precision integer. Instances of this class are immutable, so they are inherently safe for use by multiple threads.


An arbitrary-precision integer.


### fromByteArray

    public static PeterO.BigInteger fromByteArray(
        byte[] bytes,
        bool littleEndian);


Initializes a BigInteger object from an array of bytes.


<b>Parameters:</b>

 * <i>bytes</i>: A byte array.

 * <i>littleEndian</i>: A Boolean object.


<b>Returns:</b>

A BigInteger object.


### testBit

    public bool testBit(
        int index);


Returns whether a bit is set in the two's-complement representation of this object's value.


<b>Parameters:</b>

 * <i>index</i>: Zero based index of the bit to test. 0 means the least significant bit.


<b>Returns:</b>

True if the specified bit is set; otherwise, false.


### toByteArray

    public byte[] toByteArray(
        bool littleEndian);


Returns a byte array of this object's value.


<b>Parameters:</b>

 * <i>littleEndian</i>: A Boolean object.


<b>Returns:</b>

A byte array that represents the value of this object.


### shiftLeft

    public PeterO.BigInteger shiftLeft(
        int numberBits);


Shifts this object's value by a number of bits. A value of 1 doubles this value, a value of 2 multiplies it by 4, a value of 3 by 8, a value of 4 by 16, and so on.


<b>Parameters:</b>

 * <i>numberBits</i>: The number of bits to shift. Can be negative, in which case this is the same as shiftRight with the absolute value of numberBits.


<b>Returns:</b>

A BigInteger object.


### shiftRight

    public PeterO.BigInteger shiftRight(
        int numberBits);


Returns a big integer with the bits shifted to the right.


<b>Parameters:</b>

 * <i>numberBits</i>: Number of bits to shift right.


<b>Returns:</b>

A BigInteger object.


### valueOf

    public static PeterO.BigInteger valueOf(
        long longerValue);


Converts a 64-bit signed integer to a big integer.


<b>Parameters:</b>

 * <i>longerValue</i>: A 64-bit signed integer.


<b>Returns:</b>

A BigInteger object with the same value as the 64-bit number.


### intValue

    public int intValue();


Converts this object's value to a 32-bit signed integer.


<b>Returns:</b>

A 32-bit signed integer.


<b>Exceptions:</b>

 * System.OverflowException: 
This object's value is too big to fit a 32-bit signed integer.


### canFitInInt

    public bool canFitInInt();


Returns whether this object's value can fit in a 32-bit signed integer.


<b>Returns:</b>

True if this object's value is MinValue or greater, and MaxValue or less; otherwise, false.


### longValue

    public long longValue();


Converts this object's value to a 64-bit signed integer.


<b>Returns:</b>

A 64-bit signed integer.


<b>Exceptions:</b>

 * System.OverflowException: 
This object's value is too big to fit a 64-bit signed integer.


### PowBigIntVar

    public PeterO.BigInteger PowBigIntVar(
        PeterO.BigInteger power);


Not documented yet.


<b>Parameters:</b>

 * <i>power</i>: A BigInteger object. (2).


<b>Returns:</b>

A BigInteger object.


### pow

    public PeterO.BigInteger pow(
        int powerSmall);


Not documented yet.


<b>Parameters:</b>

 * <i>powerSmall</i>: A 32-bit signed integer.


<b>Returns:</b>

A BigInteger object.


### negate

    public PeterO.BigInteger negate();


Gets the value of this object with the sign reversed.


<b>Returns:</b>

This object's value with the sign reversed.


### abs

    public PeterO.BigInteger abs();


Returns the absolute value of this object's value.


<b>Returns:</b>

This object's value with the sign removed.


### getUnsignedBitLength

    public int getUnsignedBitLength();


Finds the minimum number of bits needed to represent this object's absolute value.


<b>Returns:</b>

The number of bits in this object's value. Returns 0 if this object's value is 0, and returns 1 if the value is negative 1.


### bitLength

    public int bitLength();


Finds the minimum number of bits needed to represent this object's value, except for its sign. If the value is negative, finds the number of bits in (its absolute value minus 1).


<b>Returns:</b>

The number of bits in this object's value. Returns 0 if this object's value is 0 or negative 1.


### getDigitCount

    public int getDigitCount();


Finds the number of decimal digits this number has.


<b>Returns:</b>

The number of decimal digits. Returns 1 if this object' s value is 0.


### ToString

    public override string ToString();


Converts this object to a text string.


<b>Returns:</b>

A string representation of this object.


### fromString

    public static PeterO.BigInteger fromString(
        string str);


Converts a string to an arbitrary-precision integer.


<b>Parameters:</b>

 * <i>str</i>: A string containing only digits, except that it may start with a minus sign.


<b>Returns:</b>

A BigInteger object with the same value as given in the string.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>str</i>
 is null.


 * System.FormatException: 
The parameter  <i>str</i>
is in an invalid format.


### fromSubstring

    public static PeterO.BigInteger fromSubstring(
        string str,
        int index,
        int endIndex);


Converts a portion of a string to an arbitrary-precision integer.


<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>index</i>: The index of the string that starts the string portion.

 * <i>endIndex</i>: The index of the string that ends the string portion. The length will be index + endIndex - 1.


<b>Returns:</b>

A BigInteger object with the same value as given in the string portion.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>str</i>
 is null.


 * System.FormatException: 
The string portion is in an invalid format.


### getLowestSetBit

    public int getLowestSetBit();


Not documented yet.


<b>Returns:</b>

A 32-bit signed integer.


### gcd

    public PeterO.BigInteger gcd(
        PeterO.BigInteger bigintSecond);


Returns the greatest common divisor of two integers.


The greatest common divisor (GCD) is also known as the greatest common factor (GCF).<b>Parameters:</b>

 * <i>bigintSecond</i>: A BigInteger object. (2).


<b>Returns:</b>

A BigInteger object.


### ModPow

    public PeterO.BigInteger ModPow(
        PeterO.BigInteger pow,
        PeterO.BigInteger mod);


Calculates the remainder when a BigInteger raised to a certain power is divided by another BigInteger.


<b>Parameters:</b>

 * <i>pow</i>: A BigInteger object. (2).

 * <i>mod</i>: A BigInteger object. (3).


<b>Returns:</b>

A BigInteger object.


### Equals

    public override bool Equals(
        object obj);


Determines whether this object and another object are equal.


<b>Parameters:</b>

 * <i>obj</i>: An arbitrary object.


<b>Returns:</b>

True if the objects are equal; otherwise, false.


### GetHashCode

    public override int GetHashCode();


Returns the hash code for this instance.


<b>Returns:</b>

A 32-bit hash code.


### add

    public PeterO.BigInteger add(
        PeterO.BigInteger bigintAugend);


Adds this object and another object.


<b>Parameters:</b>

 * <i>bigintAugend</i>: A BigInteger object.


<b>Returns:</b>

The sum of the two objects.


### subtract

    public PeterO.BigInteger subtract(
        PeterO.BigInteger subtrahend);


Subtracts a BigInteger from this BigInteger.


<b>Parameters:</b>

 * <i>subtrahend</i>: A BigInteger object.


<b>Returns:</b>

The difference of the two objects.


### multiply

    public PeterO.BigInteger multiply(
        PeterO.BigInteger bigintMult);


Multiplies this instance by the value of a BigInteger object.


<b>Parameters:</b>

 * <i>bigintMult</i>: A BigInteger object.


<b>Returns:</b>

The product of the two objects.


### divide

    public PeterO.BigInteger divide(
        PeterO.BigInteger bigintDivisor);


Divides this instance by the value of a BigInteger object. The result is rounded down (the fractional part is discarded). Except if the result is 0, it will be negative if this object is positive and the other is negative, or vice versa, and will be positive if both are positive or both are negative.


<b>Parameters:</b>

 * <i>bigintDivisor</i>: A BigInteger object.


<b>Returns:</b>

The quotient of the two objects.


<b>Exceptions:</b>

 * System.DivideByZeroException: 
The divisor is zero.


### divideAndRemainder

    public PeterO.BigInteger[] divideAndRemainder(
        PeterO.BigInteger divisor);


Divides this object by another big integer and returns the quotient and remainder.


<b>Parameters:</b>

 * <i>divisor</i>: The divisor.


<b>Returns:</b>

An array with two big integers: the first is the quotient, and the second is the remainder.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter divisor is null.


 * System.DivideByZeroException: 
The parameter divisor is 0.


### mod

    public PeterO.BigInteger mod(
        PeterO.BigInteger divisor);


Finds the modulus remainder that results when this instance is divided by the value of a BigInteger object. The modulus remainder is the same as the normal remainder if the normal remainder is positive, and equals divisor plus normal remainder if the normal remainder is negative.


<b>Parameters:</b>

 * <i>divisor</i>: A divisor greater than 0 (the modulus).


<b>Returns:</b>

A BigInteger object.


<b>Exceptions:</b>

 * System.ArithmeticException: 
The parameter  <i>divisor</i>
 is negative.


### remainder

    public PeterO.BigInteger remainder(
        PeterO.BigInteger divisor);


Finds the remainder that results when this instance is divided by the value of a BigInteger object. The remainder is the value that remains when the absolute value of this object is divided by the absolute value of the other object; the remainder has the same sign (positive or negative) as this object.


<b>Parameters:</b>

 * <i>divisor</i>: A BigInteger object.


<b>Returns:</b>

The remainder of the two objects.


### CompareTo

    public sealed int CompareTo(
        PeterO.BigInteger other);


Compares a BigInteger object with this instance.


<b>Parameters:</b>

 * <i>other</i>: A BigInteger object.


<b>Returns:</b>

Zero if the values are equal; a negative number if this instance is less, or a positive number if this instance is greater.


### sqrt

    public PeterO.BigInteger sqrt();


Finds the square root of this instance's value, rounded down.


<b>Returns:</b>

The square root of this object's value. Returns 0 if this value is 0 or less.


### sqrtWithRemainder

    public PeterO.BigInteger[] sqrtWithRemainder();


Calculates the square root and the remainder.


<b>Returns:</b>

An array of two big integers: the first integer is the square root, and the second is the difference between this value and the last square of the first integer, before this value. Returns two zeros if this value is 0 or less, or two ones if this value equals 1.


### ZERO

    public static readonly PeterO.BigInteger ZERO;


BigInteger object for the number zero.


### ONE

    public static readonly PeterO.BigInteger ONE;


BigInteger object for the number one.


### TEN

    public static readonly PeterO.BigInteger TEN;


BigInteger object for the number ten.


### Operator `+`

    public static PeterO.BigInteger operator +(
        PeterO.BigInteger bthis,
        PeterO.BigInteger augend);


Adds a BigInteger object and a BigInteger object.


<b>Parameters:</b>

 * <i>bthis</i>: A BigInteger object.

 * <i>augend</i>: A BigInteger object. (2).


<b>Returns:</b>

The sum of the two objects.


### Operator `-`

    public static PeterO.BigInteger operator -(
        PeterO.BigInteger bthis,
        PeterO.BigInteger subtrahend);


Subtracts two BigInteger values.


<b>Parameters:</b>

 * <i>bthis</i>: A BigInteger value.

 * <i>subtrahend</i>: A BigInteger object.


<b>Returns:</b>

The difference of the two objects.


### Operator `*`

    public static PeterO.BigInteger operator *(
        PeterO.BigInteger operand1,
        PeterO.BigInteger operand2);


Multiplies a BigInteger object by the value of a BigInteger object.


<b>Parameters:</b>

 * <i>operand1</i>: A BigInteger object.

 * <i>operand2</i>: A BigInteger object. (2).


<b>Returns:</b>

The product of the two objects.


### Operator `/`

    public static PeterO.BigInteger operator /(
        PeterO.BigInteger dividend,
        PeterO.BigInteger divisor);


Divides a BigInteger object by the value of a BigInteger object.


<b>Parameters:</b>

 * <i>dividend</i>: A BigInteger object.

 * <i>divisor</i>: A BigInteger object. (2).


<b>Returns:</b>

The quotient of the two objects.


### Operator `%`

    public static PeterO.BigInteger operator %(
        PeterO.BigInteger dividend,
        PeterO.BigInteger divisor);


Finds the remainder that results when a BigInteger object is divided by the value of a BigInteger object.


<b>Parameters:</b>

 * <i>dividend</i>: A BigInteger object.

 * <i>divisor</i>: A BigInteger object. (2).


<b>Returns:</b>

The remainder of the two objects.


### Operator `<<`

    public static PeterO.BigInteger operator <<(
        PeterO.BigInteger bthis,
        int bitCount);


Not documented yet.


<b>Parameters:</b>

 * <i>bthis</i>: A BigInteger object. (2).

 * <i>bitCount</i>: A 32-bit signed integer.


<b>Returns:</b>

A BigInteger object.


### ModPow

    public static PeterO.BigInteger ModPow(
        PeterO.BigInteger bigintValue,
        PeterO.BigInteger pow,
        PeterO.BigInteger mod);


Calculates the remainder when a BigInteger raised to a certain power is divided by another BigInteger.


<b>Parameters:</b>

 * <i>bigintValue</i>: A BigInteger object.

 * <i>pow</i>: A BigInteger object. (2).

 * <i>mod</i>: A BigInteger object. (3).


<b>Returns:</b>

The value ( <i>bigintValue</i>
^ <i>pow</i>
)% <i>mod</i>
.


### Operator `>>`

    public static PeterO.BigInteger operator >>(
        PeterO.BigInteger bthis,
        int bigValue);


Shifts the bits of a BigInteger instance to the right.


For this operation, the BigInteger is treated as a two's complement representation. Thus, for negative values, the BigInteger is sign-extended.<b>Parameters:</b>

 * <i>bthis</i>: A BigInteger object. (2).

 * <i>bigValue</i>: A 32-bit signed integer.


<b>Returns:</b>

A BigInteger object.


### Operator `-`

    public static PeterO.BigInteger operator -(
        PeterO.BigInteger bigValue);


Negates a BigInteger object.


<b>Parameters:</b>

 * <i>bigValue</i>: A BigInteger object. (2).


<b>Returns:</b>

A BigInteger object.


### Operator `<`

    public static bool operator <(
        PeterO.BigInteger thisValue,
        PeterO.BigInteger otherValue);


Determines whether a BigInteger instance is less than another BigInteger instance.


<b>Parameters:</b>

 * <i>thisValue</i>: A BigInteger object.

 * <i>otherValue</i>: A BigInteger object. (2).


<b>Returns:</b>

True if  <i>thisValue</i>
 is less than  <i>otherValue</i>
; otherwise, false.


### Operator `<=`

    public static bool operator <=(
        PeterO.BigInteger thisValue,
        PeterO.BigInteger otherValue);


Determines whether a BigInteger instance is less than or equal to another BigInteger instance.


<b>Parameters:</b>

 * <i>thisValue</i>: A BigInteger object.

 * <i>otherValue</i>: A BigInteger object. (2).


<b>Returns:</b>

True if  <i>thisValue</i>
 is up to  <i>otherValue</i>
; otherwise, false.


### Operator `>`

    public static bool operator >(
        PeterO.BigInteger thisValue,
        PeterO.BigInteger otherValue);


Determines whether a BigInteger instance is greater than another BigInteger instance.


<b>Parameters:</b>

 * <i>thisValue</i>: A BigInteger object.

 * <i>otherValue</i>: A BigInteger object. (2).


<b>Returns:</b>

True if  <i>thisValue</i>
 is greater than <i>otherValue</i>
; otherwise, false.


### Operator `>=`

    public static bool operator >=(
        PeterO.BigInteger thisValue,
        PeterO.BigInteger otherValue);


Determines whether a BigInteger value is greater than another BigInteger value.


<b>Parameters:</b>

 * <i>thisValue</i>: A BigInteger object.

 * <i>otherValue</i>: A BigInteger object. (2).


<b>Returns:</b>

True if  <i>thisValue</i>
 is at least  <i>otherValue</i>
; otherwise, false.


### Abs

    public static PeterO.BigInteger Abs(
        PeterO.BigInteger thisValue);


Not documented yet.


Not documented yet.


<b>Parameters:</b>

 * <i>thisValue</i>: A BigInteger object. (2).


<b>Returns:</b>

A BigInteger object.


### GetBits

    public long GetBits(
        int index,
        int numberBits);


Not documented yet.


<b>Parameters:</b>

 * <i>index</i>: A 32-bit signed integer.

 * <i>numberBits</i>: A 32-bit signed integer. (2).


<b>Returns:</b>

A 64-bit signed integer.


### GreatestCommonDivisor

    public static PeterO.BigInteger GreatestCommonDivisor(
        PeterO.BigInteger bigintFirst,
        PeterO.BigInteger bigintSecond);


Not documented yet.


<b>Parameters:</b>

 * <i>bigintFirst</i>: A BigInteger object. (2).

 * <i>bigintSecond</i>: A BigInteger object. (3).


<b>Returns:</b>

A BigInteger object.


### ToByteArray

    public byte[] ToByteArray();


Not documented yet.


<b>Returns:</b>

A byte array.


### Pow

    public static PeterO.BigInteger Pow(
        PeterO.BigInteger bigValue,
        PeterO.BigInteger power);


Not documented yet.


<b>Parameters:</b>

 * <i>bigValue</i>: A BigInteger object. (2).

 * <i>power</i>: A BigInteger object. (3).


<b>Returns:</b>

A BigInteger object.


### Pow

    public static PeterO.BigInteger Pow(
        PeterO.BigInteger bigValue,
        int power);


Not documented yet.


<b>Parameters:</b>

 * <i>bigValue</i>: A BigInteger object. (2).

 * <i>power</i>: A 32-bit signed integer.


<b>Returns:</b>

A BigInteger object.


### BigInteger Constructor

    public BigInteger(
        byte[] bytes);


Initializes a new instance of the BigInteger class.


<b>Parameters:</b>

 * <i>bytes</i>: A byte array.


### Equals

    public sealed bool Equals(
        PeterO.BigInteger other);


Not documented yet.


<b>Parameters:</b>

 * <i>other</i>: A BigInteger object.


<b>Returns:</b>

A Boolean object.


### Not

    public static PeterO.BigInteger Not(
        PeterO.BigInteger valueA);


Returns a BigInteger with every bit flipped.


<b>Parameters:</b>

 * <i>valueA</i>: A BigInteger object. (2).


<b>Returns:</b>

A BigInteger object.


### And

    public static PeterO.BigInteger And(
        PeterO.BigInteger a,
        PeterO.BigInteger b);


Does an AND operation between two BigInteger values.


Each BigInteger instance is treated as a two's complement representation for the purposes of this operator.<b>Parameters:</b>

 * <i>a</i>: A BigInteger instance.

 * <i>b</i>: Another BigInteger instance.


<b>Returns:</b>

A BigInteger object.


### Or

    public static PeterO.BigInteger Or(
        PeterO.BigInteger first,
        PeterO.BigInteger second);


Does an OR operation between two BigInteger instances.


Each BigInteger instance is treated as a two's complement representation for the purposes of this operator.<b>Parameters:</b>

 * <i>first</i>: A BigInteger object. (2).

 * <i>second</i>: A BigInteger object. (3).


<b>Returns:</b>

A BigInteger object.


### Xor

    public static PeterO.BigInteger Xor(
        PeterO.BigInteger a,
        PeterO.BigInteger b);


Finds the exclusive "or" of two BigInteger objects.


Each BigInteger instance is treated as a two's complement representation for the purposes of this operator.<b>Parameters:</b>

 * <i>a</i>: A BigInteger instance.

 * <i>b</i>: Another BigInteger instance.


<b>Returns:</b>

A BigInteger object.


### Sign

    public int Sign { get; }


Gets the sign of this object's value.


<b>Returns:</b>

0 if this value is zero; -1 if this value is negative, or 1 if this value is positive.


### IsZero

    public bool IsZero { get; }


Gets a value indicating whether this value is 0.


<b>Returns:</b>

True if this value is 0; otherwise, false.


### IsEven

    public bool IsEven { get; }


Gets a value indicating whether this value is even.


<b>Returns:</b>

True if this value is even; otherwise, false.


### IsPowerOfTwo

    public bool IsPowerOfTwo { get; }


Gets a value indicating whether this object's value is a power of two.


<b>Returns:</b>

True if this object's value is a power of two; otherwise, false.


### Zero

    public static PeterO.BigInteger Zero { get; }


Gets the BigInteger object for zero.


<b>Returns:</b>

The BigInteger object for zero.


### One

    public static PeterO.BigInteger One { get; }


Gets the BigInteger object for one.


<b>Returns:</b>

The BigInteger object for one.



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


Parses a number whose format follows the JSON specification (RFC 7159). Roughly speaking, a valid number consists of an optional minus sign, one or more digits (starting with 1 to 9 unless the only digit is 0), an optional decimal point with one or more digits, and an optional letter E or e with one or more digits (the exponent).


<b>Parameters:</b>

 * <i>str</i>: A string to parse.

 * <i>integersOnly</i>: If true, no decimal points or exponents are allowed in the string.

 * <i>positiveOnly</i>: If true, only positive numbers are allowed (the leading minus is disallowed).

 * <i>failOnExponentOverflow</i>: Has no effect.


<b>Returns:</b>

A CBOR object that represents the parsed number.



## PeterO.Cbor.CBORException


    public class CBORException :
        System.Exception,
        System.Runtime.Serialization.ISerializable,
        System.Runtime.InteropServices._Exception


Exception thrown for errors involving CBOR data.


### CBORException Constructor

    public CBORException();


Initializes a new instance of the CBORException class.


### CBORException Constructor

    public CBORException(
        string message);


Initializes a new instance of the CBORException class.


<b>Parameters:</b>

 * <i>message</i>: A string object.


### CBORException Constructor

    public CBORException(
        string message,
        System.Exception innerException);


Initializes a new instance of the CBORException class. Uses the given message and inner exception.


<b>Parameters:</b>

 * <i>message</i>: A string object.

 * <i>innerException</i>: An Exception object.



## PeterO.Cbor.CBORObject


    public sealed class CBORObject :
        System.IComparable,
        System.IEquatable


Represents an object in Concise Binary Object Representation (CBOR) and contains methods for reading and writing CBOR data. CBOR is defined in RFC 7049. There are many ways to get a CBOR object, including from bytes, objects, streams and JSON, as described below.


To and from byte arrays:The CBORObject.DecodeToBytes method converts a byte array in CBOR format to a CBOR object. The EncodeToBytes method converts a CBOR object to its corresponding byte array in CBOR format. 


To and from data streams:The CBORObject.Write methods write many kinds of objects to a data stream, including numbers, CBOR objects, strings, and arrays of numbers and strings. The CBORObject.Read method reads a CBOR object from a data stream. 


To and from other objects:The CBORObject.FromObject methods converts many kinds of objects to a CBOR object, including numbers, strings, and arrays and maps of numbers and strings. Methods like AsDouble, AsByte, and AsString convert a CBOR object to different types of object. 


To and from JSON:This class also doubles as a reader and writer of JavaScript Object Notation (JSON). The CBORObject.FromJSONString method converts JSON to a CBOR object, and the ToJSONString method converts a CBOR object to a JSON string. 


 Thread Safety: CBOR objects that are numbers, "simple values", and text strings are immutable (their values can't be changed), so they are inherently safe for use by multiple threads. CBOR objects that are arrays, maps, and byte strings are mutable, but this class doesn't attempt to synchronize reads and writes to those objects by multiple threads, so those objects are not thread safe without such synchronization. 


 One kind of CBOR object is called a map, or a list of key-value pairs. Keys can be any kind of CBOR object, including numbers, strings, arrays, and maps. However, since byte strings, arrays, and maps are mutable, it is not advisable to use these three kinds of object as keys; they are much better used as map values instead, keeping in mind that they are not thread safe without synchronizing reads and writes to them. 





### False

    public static readonly PeterO.Cbor.CBORObject False;


Represents the value false.


### True

    public static readonly PeterO.Cbor.CBORObject True;


Represents the value true.


### Null

    public static readonly PeterO.Cbor.CBORObject Null;


Represents the value null.


### Undefined

    public static readonly PeterO.Cbor.CBORObject Undefined;


Represents the value undefined.


### AddConverter

    public static void AddConverter<T>(
        System.Type type,
        PeterO.Cbor.ICBORConverter<T> converter);


Registers an object that converts objects of a given type to CBOR objects (called a CBOR converter).


<b>Parameters:</b>

 * <i>type</i>: A Type object specifying the type that the converter converts to CBOR objects.

 * <i>converter</i>: An ICBORConverter object.

 * &lt;T&gt;: Must be the same as the "type" parameter.


### AddTagHandler

    public static void AddTagHandler(
        PeterO.BigInteger bigintTag,
        PeterO.Cbor.ICBORTag handler);


Not documented yet.


<b>Parameters:</b>

 * <i>bigintTag</i>: A BigInteger object.

 * <i>handler</i>: An ICBORTag object.


### Negate

    public PeterO.Cbor.CBORObject Negate();


Gets this object's value with the sign reversed.


<b>Returns:</b>

The reversed-sign form of this number.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a number type.


### Abs

    public PeterO.Cbor.CBORObject Abs();


Gets this object's absolute value.


<b>Returns:</b>

This object's absolute without its negative sign.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a number type.


### PositiveInfinity

    public static readonly PeterO.Cbor.CBORObject PositiveInfinity;


The value positive infinity.


### NegativeInfinity

    public static readonly PeterO.Cbor.CBORObject NegativeInfinity;


The value negative infinity.


### NaN

    public static readonly PeterO.Cbor.CBORObject NaN;


A not-a-number value.


### IsPositiveInfinity

    public bool IsPositiveInfinity();


Gets a value indicating whether this CBOR object represents positive infinity.


<b>Returns:</b>

True if this CBOR object represents positive infinity; otherwise, false.


### IsInfinity

    public bool IsInfinity();


Gets a value indicating whether this CBOR object represents infinity.


<b>Returns:</b>

True if this CBOR object represents infinity; otherwise, false.


### IsNegativeInfinity

    public bool IsNegativeInfinity();


Gets a value indicating whether this CBOR object represents negative infinity.


<b>Returns:</b>

True if this CBOR object represents negative infinity; otherwise, false.


### IsNaN

    public bool IsNaN();


Gets a value indicating whether this CBOR object represents a not-a-number value (as opposed to whether this object's type is not a number type).


<b>Returns:</b>

True if this CBOR object represents a not-a-number value (as opposed to whether this object's type is not a number type); otherwise, false.


### CompareTo

    public sealed int CompareTo(
        PeterO.Cbor.CBORObject other);


Compares two CBOR objects. In this implementation:





 * The null pointer (null reference) is considered less than any other object.


 *  If either object is true, false, CBORObject.Null, or the undefined value, it is treated as less than the other value. If both objects have one of these four values, then undefined is less than CBORObject.Null, which is less than false, which is less than true.


 *  If both objects are numbers, their mathematical values are compared. Here, NaN (not-a-number) is considered greater than any number.


 *  If both objects are simple values other than true, false, CBORObject.Null, and the undefined value, the objects are compared according to their ordinal numbers.


 *  If both objects are arrays, each element is compared. If one array is shorter than the other and the other array begins with that array (for the purposes of comparison), the shorter array is considered less than the longer array.


 *  If both objects are strings, compares each string code-point by code-point, as though by the DataUtilities.CodePointCompare method.


 *  If both objects are maps, compares each map as though each were an array with the sorted keys of that map as the array's elements. If both maps have the same keys, their values are compared in the order of the sorted keys.


 *  If each object is a different type, then they are sorted by their type number, in the order given for the CBORType enumeration.


 *  If each object has different tags and both objects are otherwise equal under this method, each element is compared as though each were an array with that object's tags listed in order from outermost to innermost. 


 This method is not consistent with the Equals method.





<b>Parameters:</b>

 * <i>other</i>: A value to compare with.


<b>Returns:</b>

Less than 0, if this value is less than the other object; or 0, if both values are equal; or greater than 0, if this value is less than the other object or if the other object is null.


### Untag

    public PeterO.Cbor.CBORObject Untag();


Gets an object with the same value as this one but without the tags it has, if any. If this object is an array, map, or byte string, the data will not be copied to the returned object, so changes to the returned object will be reflected in this one.


<b>Returns:</b>

A CBORObject object.


### UntagOne

    public PeterO.Cbor.CBORObject UntagOne();


Gets an object with the same value as this one but without this object's outermost tag, if any. If this object is an array, map, or byte string, the data will not be copied to the returned object, so changes to the returned object will be reflected in this one.


<b>Returns:</b>

A CBORObject object.


### Equals

    public override bool Equals(
        object obj);


Determines whether this object and another object are equal.


<b>Parameters:</b>

 * <i>obj</i>: An arbitrary object.


<b>Returns:</b>

True if the objects are equal; otherwise, false.


### Equals

    public sealed bool Equals(
        PeterO.Cbor.CBORObject other);


Compares the equality of two CBOR objects.


<b>Parameters:</b>

 * <i>other</i>: The object to compare.


<b>Returns:</b>

True if the objects are equal; otherwise, false.


### GetHashCode

    public override int GetHashCode();


Calculates the hash code of this object.


<b>Returns:</b>

A 32-bit hash code.


### DecodeFromBytes

    public static PeterO.Cbor.CBORObject DecodeFromBytes(
        byte[] data);


Generates a CBOR object from an array of CBOR-encoded bytes.


<b>Parameters:</b>

 * <i>data</i>: A byte array.


<b>Returns:</b>

A CBOR object corresponding to the data.


<b>Exceptions:</b>

 * System.ArgumentException: 
Data is null or empty.


 * PeterO.Cbor.CBORException: 
There was an error in reading or parsing the data. This includes cases where not all of the byte array represents a CBOR object.


### GetByteString

    public byte[] GetByteString();


Gets the byte array used in this object, if this object is a byte string, without copying the data to a new one. This method's return value can be used to modify the array's contents. Note, though, that the array's length can't be changed.


<b>Returns:</b>

A byte array.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object is not a byte string.


### HasTag

    public bool HasTag(
        int tagValue);


Returns whether this object has a tag of the given number.


<b>Parameters:</b>

 * <i>tagValue</i>: The tag value to search for.


<b>Returns:</b>

True if this object has a tag of the given number; otherwise, false.


<b>Exceptions:</b>

 * System.ArgumentException: 
TagValue is less than 0.


### HasTag

    public bool HasTag(
        PeterO.BigInteger bigTagValue);


Returns whether this object has a tag of the given number.


<b>Parameters:</b>

 * <i>bigTagValue</i>: The tag value to search for.


<b>Returns:</b>

True if this object has a tag of the given number; otherwise, false.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
BigTagValue is null.


 * System.ArgumentException: 
BigTagValue is less than 0.


### GetTags

    public PeterO.BigInteger[] GetTags();


Gets a list of all tags, from outermost to innermost.


<b>Returns:</b>

An array of tags, or the empty string if this object is untagged.


### Add

    public PeterO.Cbor.CBORObject Add(
        PeterO.Cbor.CBORObject key,
        PeterO.Cbor.CBORObject value);


Adds a new object to this map.


<b>Parameters:</b>

 * <i>key</i>: A CBOR object representing the key. Can be null, in which case this value is converted to CBORObject.Null.

 * <i>value</i>: A CBOR object representing the value. Can be null, in which case this value is converted to CBORObject.Null.


<b>Returns:</b>

This object.


<b>Exceptions:</b>

 * System.ArgumentException: 
Key already exists in this map.


 * System.InvalidOperationException: 
This object is not a map.


### Add

    public PeterO.Cbor.CBORObject Add(
        object key,
        object valueOb);


Converts an object to a CBOR object and adds it to this map.


<b>Parameters:</b>

 * <i>key</i>: A string representing the key. Can be null, in which case this value is converted to CBORObject.Null.

 * <i>valueOb</i>: An arbitrary object. Can be null, in which case this value is converted to CBORObject.Null.


<b>Returns:</b>

This object.


<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter  <i>key</i>
 or "value" has an unsupported type.


 * System.ArgumentException: 
The parameter  <i>key</i>
 already exists in this map.


 * System.InvalidOperationException: 
This object is not a map.


### ContainsKey

    public bool ContainsKey(
        PeterO.Cbor.CBORObject key);


Determines whether a value of the given key exists in this object.


<b>Parameters:</b>

 * <i>key</i>: An object that serves as the key.


<b>Returns:</b>

True if the given key is found, or false if the given key is not found or this object is not a map.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
Key is null (as opposed to CBORObject.Null).


### ContainsKey

    public bool ContainsKey(
        string key);


Determines whether a value of the given key exists in this object.


<b>Parameters:</b>

 * <i>key</i>: A string that serves as the key.


<b>Returns:</b>

True if the given key (as a CBOR object) is found, or false if the given key is not found or this object is not a map.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
Key is null (as opposed to CBORObject.Null).


### Add

    public PeterO.Cbor.CBORObject Add(
        PeterO.Cbor.CBORObject obj);


Adds a new object to the end of this array.


<b>Parameters:</b>

 * <i>obj</i>: A CBOR object.


<b>Returns:</b>

This object.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object is not an array.


 * System.ArgumentNullException: 
The parameter <i>obj</i>
 is null (as opposed to CBORObject.Null).


### Add

    public PeterO.Cbor.CBORObject Add(
        object obj);


Converts an object to a CBOR object and adds it to the end of this array.


<b>Parameters:</b>

 * <i>obj</i>: A CBOR object.


<b>Returns:</b>

This object.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object is not an array.


 * System.ArgumentException: 
The object's type is not supported.


### Remove

    public bool Remove(
        PeterO.Cbor.CBORObject obj);


If this object is an array, removes the first instance of the specified item from the array. If this object is a map, removes the item with the given key from the map.


<b>Parameters:</b>

 * <i>obj</i>: The item or key to remove.


<b>Returns:</b>

True if the item was removed; otherwise, false.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>obj</i>
 is null (as opposed to CBORObject.Null).


 * System.InvalidOperationException: 
The object is not an array or map.


### AsDouble

    public double AsDouble();


Converts this object to a 64-bit floating point number.


<b>Returns:</b>

The closest 64-bit floating point number to this object. The return value can be positive infinity or negative infinity if this value exceeds the range of a 64-bit floating point number.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a number type.


### AsExtendedDecimal

    public PeterO.ExtendedDecimal AsExtendedDecimal();


Converts this object to a decimal number.


<b>Returns:</b>

A decimal number for this object's value. If this object is a rational number with a nonterminating decimal expansion, returns a decimal number rounded to 34 digits.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a number type.


### AsExtendedRational

    public PeterO.ExtendedRational AsExtendedRational();


Converts this object to a rational number.


<b>Returns:</b>

A rational number for this object's value.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a number type.


### AsExtendedFloat

    public PeterO.ExtendedFloat AsExtendedFloat();


Converts this object to an arbitrary-precision binary floating point number.


<b>Returns:</b>

An arbitrary-precision binary floating point number for this object's value. Note that if this object is a decimal number with a fractional part, the conversion may lose information depending on the number. If this object is a rational number with a nonterminating binary expansion, returns a decimal number rounded to 113 digits.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a number type.


### AsSingle

    public float AsSingle();


Converts this object to a 32-bit floating point number.


<b>Returns:</b>

The closest 32-bit floating point number to this object. The return value can be positive infinity or negative infinity if this object's value exceeds the range of a 32-bit floating point number.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a number type.


### AsBigInteger

    public PeterO.BigInteger AsBigInteger();


Converts this object to an arbitrary-precision integer. Fractional values are truncated to an integer.


<b>Returns:</b>

The closest big integer to this object.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a number type.


 * System.OverflowException: 
This object's value is infinity or not-a-number (NaN).


### AsBoolean

    public bool AsBoolean();


Returns false if this object is False, Null, or Undefined; otherwise, true.


<b>Returns:</b>

A Boolean object.


### AsInt16

    public short AsInt16();


Converts this object to a 16-bit signed integer. Floating point values are truncated to an integer.


<b>Returns:</b>

The closest 16-bit signed integer to this object.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a number type.


 * System.OverflowException: 
This object's value exceeds the range of a 16-bit signed integer.


### AsByte

    public byte AsByte();


Converts this object to a byte (0 to 255). Floating point values are truncated to an integer.


<b>Returns:</b>

The closest byte-sized integer to this object.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a number type.


 * System.OverflowException: 
This object's value exceeds the range of a byte (would be less than 0 or greater than 255 when truncated to an integer).


### AsInt64

    public long AsInt64();


Converts this object to a 64-bit signed integer. Floating point values are truncated to an integer.


<b>Returns:</b>

The closest 64-bit signed integer to this object.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a number type.


 * System.OverflowException: 
This object's value exceeds the range of a 64-bit signed integer.


### CanFitInSingle

    public bool CanFitInSingle();


Returns whether this object's value can be converted to a 32-bit floating point number without loss of its numerical value.


<b>Returns:</b>

Whether this object's value can be converted to a 32-bit floating point number without loss of its numerical value. Returns true if this is a not-a-number value, even if the value's diagnostic information can't fit in a 32-bit floating point number.


### CanFitInDouble

    public bool CanFitInDouble();


Returns whether this object's value can be converted to a 64-bit floating point number without loss of its numerical value.


<b>Returns:</b>

Whether this object's value can be converted to a 64-bit floating point number without loss of its numerical value. Returns true if this is a not-a-number value, even if the value's diagnostic information can't fit in a 64-bit floating point number.


### CanFitInInt32

    public bool CanFitInInt32();


Returns whether this object's value is an integral value, is -(2^32) or greater, and is less than 2^32.


<b>Returns:</b>

True if this object's value is an integral value, is -(2^32) or greater, and is less than 2^32; otherwise, false.


### CanFitInInt64

    public bool CanFitInInt64();


Returns whether this object's value is an integral value, is -(2^63) or greater, and is less than 2^63.


<b>Returns:</b>

True if this object's value is an integral value, is -(2^63) or greater, and is less than 2^63; otherwise, false.


### CanTruncatedIntFitInInt64

    public bool CanTruncatedIntFitInInt64();


Returns whether this object's value, truncated to an integer, would be -(2^63) or greater, and less than 2^63.


<b>Returns:</b>

True if this object's value, truncated to an integer, would be -(2^63) or greater, and less than 2^63; otherwise, false.


### CanTruncatedIntFitInInt32

    public bool CanTruncatedIntFitInInt32();


Returns whether this object's value, truncated to an integer, would be -(2^31) or greater, and less than 2^31.


<b>Returns:</b>

True if this object's value, truncated to an integer, would be -(2^31) or greater, and less than 2^31; otherwise, false.


### AsInt32

    public int AsInt32();


Converts this object to a 32-bit signed integer. Floating point values are truncated to an integer.


<b>Returns:</b>

The closest big integer to this object.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a number type.


 * System.OverflowException: 
This object's value exceeds the range of a 32-bit signed integer.


### AsString

    public string AsString();


Gets the value of this object as a string object.


<b>Returns:</b>

Gets this object's string.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a string.


### Read

    public static PeterO.Cbor.CBORObject Read(
        System.IO.Stream stream);


Reads an object in CBOR format from a data stream.


<b>Parameters:</b>

 * <i>stream</i>: A readable data stream.


<b>Returns:</b>

A CBOR object that was read.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.


 * PeterO.Cbor.CBORException: 
There was an error in reading or parsing the data.


### Write

    public static void Write(
        string str,
        System.IO.Stream stream);


Writes a string in CBOR format to a data stream.


<b>Parameters:</b>

 * <i>str</i>: The string to write. Can be null.

 * <i>stream</i>: A writable data stream.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.


 * System.IO.IOException: 
An I/O error occurred.


### Write

    public static void Write(
        PeterO.ExtendedFloat bignum,
        System.IO.Stream stream);


Writes a binary floating-point number in CBOR format to a data stream as follows: 


 * If the value is null, writes the byte 0xF6.


 * If the value is negative zero, infinity, or NaN, converts the number to a  `double` and writes that  `double` . If negative zero should not be written this way, use the Plus method to convert the value beforehand.


 * If the value has an exponent of zero, writes the value as an unsigned integer or signed integer if the number can fit either type or as a big integer otherwise.


 * In all other cases, writes the value as a big float.





<b>Parameters:</b>

 * <i>bignum</i>: An ExtendedFloat object.

 * <i>stream</i>: A writable data stream.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.


 * System.IO.IOException: 
An I/O error occurred.


### Write

    public static void Write(
        PeterO.ExtendedRational rational,
        System.IO.Stream stream);


Writes a rational number in CBOR format to a data stream.


<b>Parameters:</b>

 * <i>rational</i>: An ExtendedRational object.

 * <i>stream</i>: A writable data stream.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.


 * System.IO.IOException: 
An I/O error occurred.


### Write

    public static void Write(
        PeterO.ExtendedDecimal bignum,
        System.IO.Stream stream);


Writes a decimal floating-point number in CBOR format to a data stream, as follows: 


 * If the value is null, writes the byte 0xF6.


 * If the value is negative zero, infinity, or NaN, converts the number to a  `double` and writes that  `double` . If negative zero should not be written this way, use the Plus method to convert the value beforehand.


 * If the value has an exponent of zero, writes the value as an unsigned integer or signed integer if the number can fit either type or as a big integer otherwise.


 * In all other cases, writes the value as a decimal number.





<b>Parameters:</b>

 * <i>bignum</i>: Decimal fraction to write. Can be null.

 * <i>stream</i>: Stream to write to.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.


 * System.IO.IOException: 
An I/O error occurred.


### Write

    public static void Write(
        PeterO.BigInteger bigint,
        System.IO.Stream stream);


Writes a big integer in CBOR format to a data stream.


<b>Parameters:</b>

 * <i>bigint</i>: Big integer to write. Can be null.

 * <i>stream</i>: A writable data stream.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.


 * System.IO.IOException: 
An I/O error occurred.


### WriteTo

    public void WriteTo(
        System.IO.Stream stream);


Writes this CBOR object to a data stream.


<b>Parameters:</b>

 * <i>stream</i>: A writable data stream.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.


 * System.IO.IOException: 
An I/O error occurred.


### Write

    public static void Write(
        long value,
        System.IO.Stream stream);


Writes a 64-bit signed integer in CBOR format to a data stream.


<b>Parameters:</b>

 * <i>value</i>: The value to write.

 * <i>stream</i>: A writable data stream.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.


 * System.IO.IOException: 
An I/O error occurred.


### Write

    public static void Write(
        int value,
        System.IO.Stream stream);


Writes a 32-bit signed integer in CBOR format to a data stream.


<b>Parameters:</b>

 * <i>value</i>: The value to write.

 * <i>stream</i>: A writable data stream.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.


 * System.IO.IOException: 
An I/O error occurred.


### Write

    public static void Write(
        short value,
        System.IO.Stream stream);


Writes a 16-bit signed integer in CBOR format to a data stream.


<b>Parameters:</b>

 * <i>value</i>: The value to write.

 * <i>stream</i>: A writable data stream.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.


 * System.IO.IOException: 
An I/O error occurred.


### Write

    public static void Write(
        char value,
        System.IO.Stream stream);


Writes a Unicode character as a string in CBOR format to a data stream.


<b>Parameters:</b>

 * <i>value</i>: The value to write.

 * <i>stream</i>: A writable data stream.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.


 * System.ArgumentException: 
The parameter  <i>value</i>
 is a surrogate code point.


 * System.IO.IOException: 
An I/O error occurred.


### Write

    public static void Write(
        bool value,
        System.IO.Stream stream);


Writes a Boolean value in CBOR format to a data stream.


<b>Parameters:</b>

 * <i>value</i>: The value to write.

 * <i>stream</i>: A writable data stream.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.


 * System.IO.IOException: 
An I/O error occurred.


### Write

    public static void Write(
        byte value,
        System.IO.Stream stream);


Writes a byte (0 to 255) in CBOR format to a data stream. If the value is less than 24, writes that byte. If the value is 25 to 255, writes the byte 24, then this byte's value.


<b>Parameters:</b>

 * <i>value</i>: The value to write.

 * <i>stream</i>: A writable data stream.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.


 * System.IO.IOException: 
An I/O error occurred.


### Write

    public static void Write(
        float value,
        System.IO.Stream s);


Writes a 32-bit floating-point number in CBOR format to a data stream.


<b>Parameters:</b>

 * <i>value</i>: The value to write.

 * <i>s</i>: A writable data stream.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>s</i>
 is null.


 * System.IO.IOException: 
An I/O error occurred.


### Write

    public static void Write(
        double value,
        System.IO.Stream stream);


Writes a 64-bit floating-point number in CBOR format to a data stream.


<b>Parameters:</b>

 * <i>value</i>: The value to write.

 * <i>stream</i>: A writable data stream.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.


 * System.IO.IOException: 
An I/O error occurred.


### EncodeToBytes

    public byte[] EncodeToBytes();


Gets the binary representation of this data item.


<b>Returns:</b>

A byte array in CBOR format.


### Write

    public static void Write(
        PeterO.Cbor.CBORObject value,
        System.IO.Stream stream);


Writes a CBOR object to a CBOR data stream.


<b>Parameters:</b>

 * <i>value</i>: The value to write. Can be null.

 * <i>stream</i>: A writable data stream.


### Write

    public static void Write(
        object objValue,
        System.IO.Stream stream);


Writes an arbitrary object to a CBOR data stream. Currently, the following objects are supported: 


 * Lists of CBORObject.


 * Maps of CBORObject.


 * Null.


 * Any object accepted by the FromObject static methods.





<b>Parameters:</b>

 * <i>objValue</i>: The value to write.

 * <i>stream</i>: A writable data stream.


<b>Exceptions:</b>

 * System.ArgumentException: 
The object's type is not supported.


### FromJSONString

    public static PeterO.Cbor.CBORObject FromJSONString(
        string str);


Generates a CBOR object from a string in JavaScript Object Notation (JSON) format. If a JSON object has the same key, only the last given value will be used for each duplicated key. The JSON string may not begin with a byte order mark (U + FEFF).





<b>Parameters:</b>

 * <i>str</i>: A string in JSON format.


<b>Returns:</b>

A CBORObject object.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>str</i>
 is null.


 * PeterO.Cbor.CBORException: 
The string is not in JSON format.


### ReadJSON

    public static PeterO.Cbor.CBORObject ReadJSON(
        System.IO.Stream stream);


Generates a CBOR object from a data stream in JavaScript Object Notation (JSON) format and UTF-8 encoding. The JSON stream may begin with a byte order mark (U + FEFF); however, this implementation's ToJSONString method will not place this character at the beginning of a JSON text, since doing so is forbidden under RFC 7159. If a JSON object has the same key, only the last given value will be used for each duplicated key.





<b>Parameters:</b>

 * <i>stream</i>: A readable data stream.


<b>Returns:</b>

A CBORObject object.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.


 * System.IO.IOException: 
An I/O error occurred.


 * PeterO.Cbor.CBORException: 
The data stream contains invalid UTF-8 or is not in JSON format.


### ToJSONString

    public string ToJSONString();


Converts this object to a JSON string. This function works not only with arrays and maps, but also integers, strings, byte arrays, and other JSON data types.


<b>Returns:</b>

A string object containing the converted object.


### Addition

    public static PeterO.Cbor.CBORObject Addition(
        PeterO.Cbor.CBORObject first,
        PeterO.Cbor.CBORObject second);


Finds the sum of two CBOR number objects.


<b>Parameters:</b>

 * <i>first</i>: A CBORObject object. (2).

 * <i>second</i>: A CBORObject object. (3).


<b>Returns:</b>

A CBORObject object.


<b>Exceptions:</b>

 * System.ArgumentException: 
Either or both operands are not numbers (as opposed to Not-a-Number, NaN).


### Subtract

    public static PeterO.Cbor.CBORObject Subtract(
        PeterO.Cbor.CBORObject first,
        PeterO.Cbor.CBORObject second);


Finds the difference between two CBOR number objects.


<b>Parameters:</b>

 * <i>first</i>: A CBORObject object.

 * <i>second</i>: A CBORObject object. (2).


<b>Returns:</b>

The difference of the two objects.


<b>Exceptions:</b>

 * System.ArgumentException: 
Either or both operands are not numbers (as opposed to Not-a-Number, NaN).


### Multiply

    public static PeterO.Cbor.CBORObject Multiply(
        PeterO.Cbor.CBORObject first,
        PeterO.Cbor.CBORObject second);


Multiplies two CBOR number objects.


<b>Parameters:</b>

 * <i>first</i>: A CBORObject object.

 * <i>second</i>: A CBORObject object. (2).


<b>Returns:</b>

The product of the two objects.


<b>Exceptions:</b>

 * System.ArgumentException: 
Either or both operands are not numbers (as opposed to Not-a-Number, NaN).


### Divide

    public static PeterO.Cbor.CBORObject Divide(
        PeterO.Cbor.CBORObject first,
        PeterO.Cbor.CBORObject second);


Divides a CBORObject object by the value of a CBORObject object.


<b>Parameters:</b>

 * <i>first</i>: A CBORObject object.

 * <i>second</i>: A CBORObject object. (2).


<b>Returns:</b>

The quotient of the two objects.


### Remainder

    public static PeterO.Cbor.CBORObject Remainder(
        PeterO.Cbor.CBORObject first,
        PeterO.Cbor.CBORObject second);


Finds the remainder that results when a CBORObject object is divided by the value of a CBORObject object.


<b>Parameters:</b>

 * <i>first</i>: A CBORObject object.

 * <i>second</i>: A CBORObject object. (2).


<b>Returns:</b>

The remainder of the two objects.


### NewArray

    public static PeterO.Cbor.CBORObject NewArray();


Creates a new empty CBOR array.


<b>Returns:</b>

A new CBOR array.


### NewMap

    public static PeterO.Cbor.CBORObject NewMap();


Creates a new empty CBOR map.


<b>Returns:</b>

A new CBOR map.


### FromSimpleValue

    public static PeterO.Cbor.CBORObject FromSimpleValue(
        int simpleValue);


Creates a CBOR object from a simple value number.


<b>Parameters:</b>

 * <i>simpleValue</i>: A 32-bit signed integer.


<b>Returns:</b>

A CBORObject object.


<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter  <i>simpleValue</i>
 is less than 0, greater than 255, or from 24 through 31.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        long value);


Generates a CBOR object from a 64-bit signed integer.


<b>Parameters:</b>

 * <i>value</i>: A 64-bit signed integer.


<b>Returns:</b>

A CBORObject object.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        PeterO.Cbor.CBORObject value);


Generates a CBOR object from a CBOR object.


<b>Parameters:</b>

 * <i>value</i>: A CBOR object.


<b>Returns:</b>

Same as.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        PeterO.BigInteger bigintValue);


Generates a CBOR object from an arbitrary-precision integer.


<b>Parameters:</b>

 * <i>bigintValue</i>: An arbitrary-precision value.


<b>Returns:</b>

A CBOR number object.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        PeterO.ExtendedFloat bigValue);


Generates a CBOR object from an arbitrary-precision binary floating-point number.


<b>Parameters:</b>

 * <i>bigValue</i>: An arbitrary-precision binary floating-point number.


<b>Returns:</b>

A CBOR number object.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        PeterO.ExtendedRational bigValue);


Generates a CBOR object from a rational number.


<b>Parameters:</b>

 * <i>bigValue</i>: A rational number.


<b>Returns:</b>

A CBOR number object.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        PeterO.ExtendedDecimal otherValue);


Generates a CBOR object from a decimal number.


<b>Parameters:</b>

 * <i>otherValue</i>: An arbitrary-precision decimal number.


<b>Returns:</b>

A CBOR number object.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        string strValue);


Generates a CBOR object from a string.


<b>Parameters:</b>

 * <i>strValue</i>: A string value. Can be null.


<b>Returns:</b>

A CBOR object representing the string, or CBORObject.Null if stringValue is null.


<b>Exceptions:</b>

 * System.ArgumentException: 
The string contains an unpaired surrogate code point.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        int value);


Generates a CBOR object from a 32-bit signed integer.


<b>Parameters:</b>

 * <i>value</i>: A 32-bit signed integer.


<b>Returns:</b>

A CBORObject object.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        short value);


Generates a CBOR object from a 16-bit signed integer.


<b>Parameters:</b>

 * <i>value</i>: A 16-bit signed integer.


<b>Returns:</b>

A CBORObject object.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        char value);


Generates a CBOR string object from a Unicode character.


<b>Parameters:</b>

 * <i>value</i>: A char object.


<b>Returns:</b>

A CBORObject object.


<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter  <i>value</i>
 is a surrogate code point.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        bool value);


Returns the CBOR true value or false value, depending on "value".


<b>Parameters:</b>

 * <i>value</i>: Either True or False.


<b>Returns:</b>

CBORObject.True if value is true; otherwise CBORObject.False.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        byte value);


Generates a CBOR object from a byte (0 to 255).


<b>Parameters:</b>

 * <i>value</i>: A Byte object.


<b>Returns:</b>

A CBORObject object.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        float value);


Generates a CBOR object from a 32-bit floating-point number.


<b>Parameters:</b>

 * <i>value</i>: A 32-bit floating-point number.


<b>Returns:</b>

A CBORObject object.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        double value);


Generates a CBOR object from a 64-bit floating-point number.


<b>Parameters:</b>

 * <i>value</i>: A 64-bit floating-point number.


<b>Returns:</b>

A CBORObject object.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        byte[] bytes);


Generates a CBOR object from a byte array. The byte array is copied to a new byte array. (This method can't be used to decode CBOR data from a byte array; for that, use the DecodeFromBytes method instead.).


<b>Parameters:</b>

 * <i>bytes</i>: A byte array. Can be null.


<b>Returns:</b>

A CBOR byte string object where each byte of the given byte array is copied to a new array, or CBORObject.Null if the value is null.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        PeterO.Cbor.CBORObject[] array);


Generates a CBOR object from an array of CBOR objects.


<b>Parameters:</b>

 * <i>array</i>: An array of CBOR objects.


<b>Returns:</b>

A CBOR object where each element of the given array is copied to a new array, or CBORObject.Null if the value is null.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        int[] array);


Generates a CBOR object from an array of 32-bit integers.


<b>Parameters:</b>

 * <i>array</i>: An array of 32-bit integers.


<b>Returns:</b>

A CBOR array object where each element of the given array is copied to a new array, or CBORObject.Null if the value is null.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        long[] array);


Generates a CBOR object from an array of 64-bit integers.


<b>Parameters:</b>

 * <i>array</i>: An array of 64-bit integers.


<b>Returns:</b>

A CBOR array object where each element of the given array is copied to a new array, or CBORObject.Null if the value is null.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject<T>(
        System.Collections.Generic.IList<T> value);


Generates a CBOR object from a list of objects.


<b>Parameters:</b>

 * <i>value</i>: An array of CBOR objects. Can be null.

 * &lt;T&gt;: A type convertible to CBORObject.


<b>Returns:</b>

A CBOR object where each element of the given array is converted to a CBOR object and copied to a new array, or CBORObject.Null if the value is null.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject<T>(
        System.Collections.Generic.IEnumerable<T> value);


Generates a CBOR object from an enumerable set of objects.


<b>Parameters:</b>

 * <i>value</i>: An object that implements the IEnumerable interface. In the .NET version, this can be the return value of an iterator or the result of a LINQ query.

 * &lt;T&gt;: A type convertible to CBORObject.


<b>Returns:</b>

A CBOR object where each element of the given enumerable object is converted to a CBOR object and copied to a new array, or CBORObject.Null if the value is null.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject<TKey, TValue>(
        System.Collections.Generic.IDictionary<TKey, TValue> dic);


Generates a CBOR object from a map of objects.


<b>Parameters:</b>

 * <i>dic</i>: A map of CBOR objects.

 * &lt;TKey&gt;: A type convertible to CBORObject; the type of the keys.

 * &lt;TValue&gt;: A type convertible to CBORObject; the type of the values.


<b>Returns:</b>

A CBOR object where each key and value of the given map is converted to a CBOR object and copied to a new map, or CBORObject.Null if  <i>dic</i>
 is null.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        object obj);


Generates a CBORObject from an arbitrary object. The following types are specially handled by this method:  `null` , primitive types, strings,  `CBORObject` ,  `ExtendedDecimal` ,  `ExtendedFloat` , the custom  `BigInteger` , lists, arrays, enumerations ( `Enum` objects), and maps.In the .NET version, if the object is a type not specially handled by this method, returns a CBOR map with the values of each of its read/write properties (or all properties in the case of an anonymous type). Properties are converted to their camel-case names (meaning if a name starts with A to Z, that letter is lower-cased). If the property name begins with the word "Is", that word is deleted from the name. Also, .NET  `Enum` objects will be converted to their integer values, and a multidimensional array is converted to an array of arrays.


In the Java version, if the object is a type not specially handled by this method, this method checks the CBOR object for methods starting with the word "get" or "is" that take no parameters, and returns a CBOR map with one entry for each such method found. For each method found, the starting word "get" or "is" is deleted from its name, and the name is converted to camel case (meaning if a name starts with A to Z, that letter is lower-cased). Also, Java  `Enum` objects will be converted to the result of their  `name` method.





<b>Parameters:</b>

 * <i>obj</i>: An arbitrary object.


<b>Returns:</b>

A CBOR object corresponding to the given object. Returns CBORObject.Null if the object is null.


<b>Exceptions:</b>

 * System.ArgumentException: 
The object's type is not supported.


### FromObjectAndTag

    public static PeterO.Cbor.CBORObject FromObjectAndTag(
        object valueOb,
        PeterO.BigInteger bigintTag);


Generates a CBOR object from an arbitrary object and gives the resulting object a tag.


<b>Parameters:</b>

 * <i>valueOb</i>: An arbitrary object. If the tag number is 2 or 3, this must be a byte string whose bytes represent an integer in little-endian byte order, and the value of the number is 1 minus the integer's value for tag 3. If the tag number is 4 or 5, this must be an array with two elements: the first must be an integer representing the exponent, and the second must be an integer representing a mantissa.

 * <i>bigintTag</i>: Tag number. The tag number 55799 can be used to mark a "self-described CBOR" object.


<b>Returns:</b>

A CBOR object where the object  <i>valueOb</i>
is converted to a CBOR object and given the tag  <i>bigintTag</i>
.


<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter  <i>bigintTag</i>
 is less than 0 or greater than 2^64-1, or  <i>valueOb</i>
's type is unsupported.


### FromObjectAndTag

    public static PeterO.Cbor.CBORObject FromObjectAndTag(
        object valueObValue,
        int smallTag);


Generates a CBOR object from an arbitrary object and gives the resulting object a tag.


<b>Parameters:</b>

 * <i>valueObValue</i>: An arbitrary object. If the tag number is 2 or 3, this must be a byte string whose bytes represent an integer in little-endian byte order, and the value of the number is 1 minus the integer's value for tag 3. If the tag number is 4 or 5, this must be an array with two elements: the first must be an integer representing the exponent, and the second must be an integer representing a mantissa.

 * <i>smallTag</i>: A 32-bit integer that specifies a tag number. The tag number 55799 can be used to mark a "self-described CBOR" object.


<b>Returns:</b>

A CBOR object where the object  <i>valueObValue</i>
is converted to a CBOR object and given the tag  <i>smallTag</i>
.


<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter  <i>smallTag</i>
 is less than 0 or  <i>valueObValue</i>
's type is unsupported.


### ToString

    public override string ToString();


Returns this CBOR object in string form. The format is intended to be human-readable, not machine-readable, and the format may change at any time.


<b>Returns:</b>

A text representation of this object.


### AsUInt16

    public ushort AsUInt16();


Converts this object to a 16-bit unsigned integer. The return value will be truncated as necessary.


<b>Returns:</b>

A 16-bit unsigned integer.


<b>Exceptions:</b>

 * System.OverflowException: 
This object's value is outside the range of a 16-bit unsigned integer.


### AsUInt32

    public uint AsUInt32();


Converts this object to a 32-bit unsigned integer. The return value will be truncated as necessary.


<b>Returns:</b>

A 32-bit unsigned integer.


<b>Exceptions:</b>

 * System.OverflowException: 
This object's value is outside the range of a 32-bit unsigned integer.


### AsSByte

    public sbyte AsSByte();


Converts this object to an 8-bit signed integer.


<b>Returns:</b>

An 8-bit signed integer.


### AsDecimal

    public System.Decimal AsDecimal();


Converts this object to a .NET decimal.


<b>Returns:</b>

The closest big integer to this object.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a number type.


 * System.OverflowException: 
This object's value exceeds the range of a .NET decimal.


### AsUInt64

    public ulong AsUInt64();


Converts this object to a 64-bit unsigned integer. Floating point values are truncated to an integer.


<b>Returns:</b>

The closest big integer to this object.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a number type.


 * System.OverflowException: 
This object's value exceeds the range of a 64-bit unsigned integer.


### Write

    public static void Write(
        sbyte value,
        System.IO.Stream stream);


Writes an 8-bit signed integer in CBOR format to a data stream.


<b>Parameters:</b>

 * <i>value</i>: An 8-bit signed integer.

 * <i>stream</i>: A writable data stream.


### Write

    public static void Write(
        ulong value,
        System.IO.Stream stream);


Writes a 64-bit unsigned integer in CBOR format to a data stream.


<b>Parameters:</b>

 * <i>value</i>: A 64-bit unsigned integer.

 * <i>stream</i>: A writable data stream.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        System.Decimal value);


Converts a .NET decimal to a CBOR object.


<b>Parameters:</b>

 * <i>value</i>: A Decimal object.


<b>Returns:</b>

A CBORObject object with the same value as the .NET decimal.


### Write

    public static void Write(
        uint value,
        System.IO.Stream stream);


Writes a 32-bit unsigned integer in CBOR format to a data stream.


<b>Parameters:</b>

 * <i>value</i>: A 32-bit unsigned integer.

 * <i>stream</i>: A writable data stream.


### Write

    public static void Write(
        ushort value,
        System.IO.Stream stream);


Writes a 16-bit unsigned integer in CBOR format to a data stream.


<b>Parameters:</b>

 * <i>value</i>: A 16-bit unsigned integer.

 * <i>stream</i>: A writable data stream.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        sbyte value);


Converts a signed 8-bit integer to a CBOR object.


<b>Parameters:</b>

 * <i>value</i>: An 8-bit signed integer.


<b>Returns:</b>

A CBORObject object.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        ulong value);


Converts a 64-bit unsigned integer to a CBOR object.


<b>Parameters:</b>

 * <i>value</i>: A 64-bit unsigned integer.


<b>Returns:</b>

A CBORObject object.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        uint value);


Converts a 32-bit unsigned integer to a CBOR object.


<b>Parameters:</b>

 * <i>value</i>: A 32-bit unsigned integer.


<b>Returns:</b>

A CBORObject object.


### FromObject

    public static PeterO.Cbor.CBORObject FromObject(
        ushort value);


Converts a 16-bit unsigned integer to a CBOR object.


<b>Parameters:</b>

 * <i>value</i>: A 16-bit unsigned integer.


<b>Returns:</b>

A CBORObject object.


### FromObjectAndTag

    public static PeterO.Cbor.CBORObject FromObjectAndTag(
        object o,
        ulong tag);


Generates a CBOR object from an arbitrary object and gives the resulting object a tag.


<b>Parameters:</b>

 * <i>o</i>: An arbitrary object.

 * <i>tag</i>: A 64-bit unsigned integer.


<b>Returns:</b>

A CBOR object where the object  <i>o</i>
 is converted to a CBOR object and given the tag  <i>tag</i>
.


### Operator `+`

    public static PeterO.Cbor.CBORObject operator +(
        PeterO.Cbor.CBORObject a,
        PeterO.Cbor.CBORObject b);


Adds a CBORObject object and a CBORObject object.


<b>Parameters:</b>

 * <i>a</i>: A CBORObject object.

 * <i>b</i>: A CBORObject object. (2).


<b>Returns:</b>

The sum of the two objects.


### Operator `-`

    public static PeterO.Cbor.CBORObject operator -(
        PeterO.Cbor.CBORObject a,
        PeterO.Cbor.CBORObject b);


Subtracts a CBORObject object from a CBORObject object.


<b>Parameters:</b>

 * <i>a</i>: A CBORObject object.

 * <i>b</i>: A CBORObject object. (2).


<b>Returns:</b>

The difference of the two objects.


### Operator `*`

    public static PeterO.Cbor.CBORObject operator *(
        PeterO.Cbor.CBORObject a,
        PeterO.Cbor.CBORObject b);


Multiplies a CBORObject object by the value of a CBORObject object.


<b>Parameters:</b>

 * <i>a</i>: A CBORObject object.

 * <i>b</i>: A CBORObject object. (2).


<b>Returns:</b>

The product of the two objects.


### Operator `/`

    public static PeterO.Cbor.CBORObject operator /(
        PeterO.Cbor.CBORObject a,
        PeterO.Cbor.CBORObject b);


Divides a CBORObject object by the value of a CBORObject object.


<b>Parameters:</b>

 * <i>a</i>: A CBORObject object.

 * <i>b</i>: A CBORObject object. (2).


<b>Returns:</b>

The quotient of the two objects.


### Operator `%`

    public static PeterO.Cbor.CBORObject operator %(
        PeterO.Cbor.CBORObject a,
        PeterO.Cbor.CBORObject b);


Finds the remainder that results when a CBORObject object is divided by the value of a CBORObject object.


<b>Parameters:</b>

 * <i>a</i>: A CBORObject object.

 * <i>b</i>: A CBORObject object. (2).


<b>Returns:</b>

The remainder of the two objects.


### Type

    public PeterO.Cbor.CBORType Type { get; }


Gets the general data type of this CBOR object.


<b>Returns:</b>

The general data type of this CBOR object.


### IsTrue

    public bool IsTrue { get; }


Gets a value indicating whether this value is a CBOR true value.


<b>Returns:</b>

True if this value is a CBOR true value; otherwise, false.


### IsFalse

    public bool IsFalse { get; }


Gets a value indicating whether this value is a CBOR false value.


<b>Returns:</b>

True if this value is a CBOR false value; otherwise, false.


### IsNull

    public bool IsNull { get; }


Gets a value indicating whether this value is a CBOR null value.


<b>Returns:</b>

True if this value is a CBOR null value; otherwise, false.


### IsUndefined

    public bool IsUndefined { get; }


Gets a value indicating whether this value is a CBOR undefined value.


<b>Returns:</b>

True if this value is a CBOR undefined value; otherwise, false.


### IsZero

    public bool IsZero { get; }


Gets a value indicating whether this object's value equals 0.


<b>Returns:</b>

True if this object's value equals 0; otherwise, false.


### Sign

    public int Sign { get; }


Gets this value's sign: -1 if negative; 1 if positive; 0 if zero.


<b>Returns:</b>

This value's sign: -1 if negative; 1 if positive; 0 if zero.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object's type is not a number type, including the special not-a-number value (NaN).


### IsFinite

    public bool IsFinite { get; }


Gets a value indicating whether this CBOR object represents a finite number.


<b>Returns:</b>

True if this CBOR object represents a finite number; otherwise, false.


### Count

    public int Count { get; }


Gets the number of keys in this map, or the number of items in this array, or 0 if this item is neither an array nor a map.


<b>Returns:</b>

The number of keys in this map, or the number of items in this array, or 0 if this item is neither an array nor a map.


### IsTagged

    public bool IsTagged { get; }


Gets a value indicating whether this data item has at least one tag.


<b>Returns:</b>

True if this data item has at least one tag; otherwise, false.


### OutermostTag

    public PeterO.BigInteger OutermostTag { get; }


Gets the outermost tag for this CBOR data item, or -1 if the item is untagged.


<b>Returns:</b>

The outermost tag for this CBOR data item, or -1 if the item is untagged.


### InnermostTag

    public PeterO.BigInteger InnermostTag { get; }


Gets the last defined tag for this CBOR data item, or -1 if the item is untagged.


<b>Returns:</b>

The last defined tag for this CBOR data item, or -1 if the item is untagged.


### Keys

    public System.Collections.Generic.ICollection Keys { get; }


Gets a collection of the keys of this CBOR object in an undefined order.


<b>Returns:</b>

A collection of the keys of this CBOR object.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object is not a map.


### Values

    public System.Collections.Generic.ICollection Values { get; }


Gets a collection of the values of this CBOR object. If this object is a map, returns one value for each key in the map in an undefined order. If this is an array, returns all the values of the array in the order they are listed.


<b>Returns:</b>

A collection of the values of this CBOR object.


<b>Exceptions:</b>

 * System.InvalidOperationException: 
This object is not a map or an array.


### SimpleValue

    public int SimpleValue { get; }


Gets the simple value ID of this object, or -1 if this object is not a simple value (including if the value is a floating-point number).


<b>Returns:</b>

The simple value ID of this object, or -1 if this object is not a simple value (including if the value is a floating-point number).


### IsIntegral

    public bool IsIntegral { get; }


Gets a value indicating whether this object represents an integral number, that is, a number without a fractional part. Infinity and not-a-number are not considered integral.


<b>Returns:</b>

True if this object represents an integral number, that is, a number without a fractional part; otherwise, false.



## PeterO.Cbor.CBORType


    public sealed struct CBORType :
        System.Enum,
        System.IComparable,
        System.IFormattable,
        System.IConvertible


Represents a type that a CBOR object can have.


### Number

    public static PeterO.Cbor.CBORType Number = 0;


A number of any kind, including integers, big integers, floating point numbers, and decimal numbers. The floating-point value Not-a-Number is also included in the Number type.


### Boolean

    public static PeterO.Cbor.CBORType Boolean = 1;


The simple values true and false.


### SimpleValue

    public static PeterO.Cbor.CBORType SimpleValue = 2;


A "simple value" other than floating point values, true, and false.


### ByteString

    public static PeterO.Cbor.CBORType ByteString = 3;


An array of bytes.


### TextString

    public static PeterO.Cbor.CBORType TextString = 4;


A text string.


### Array

    public static PeterO.Cbor.CBORType Array = 5;


An array of CBOR objects.


### Map

    public static PeterO.Cbor.CBORType Map = 6;


A map of CBOR objects.



## PeterO.Cbor.CBORTypeFilter


    public class CBORTypeFilter


Specifies what kinds of CBOR objects a tag can be.


### WithUnsignedInteger

    public PeterO.Cbor.CBORTypeFilter WithUnsignedInteger();


Copies this filter and includes unsigned integers in the new filter.


<b>Returns:</b>

A CBORTypeFilter object.


### WithNegativeInteger

    public PeterO.Cbor.CBORTypeFilter WithNegativeInteger();


Copies this filter and includes negative integers in the new filter.


<b>Returns:</b>

A CBORTypeFilter object.


### WithByteString

    public PeterO.Cbor.CBORTypeFilter WithByteString();


Copies this filter and includes byte strings in the new filter.


<b>Returns:</b>

A CBORTypeFilter object.


### WithMap

    public PeterO.Cbor.CBORTypeFilter WithMap();


Copies this filter and includes maps in the new filter.


<b>Returns:</b>

A CBORTypeFilter object.


### WithTextString

    public PeterO.Cbor.CBORTypeFilter WithTextString();


Copies this filter and includes text strings in the new filter.


<b>Returns:</b>

A CBORTypeFilter object.


### WithTags

    public PeterO.Cbor.CBORTypeFilter WithTags(
        params int[] tags);


Not documented yet.


<b>Parameters:</b>

 * <i>tags</i>: An integer array of tags allowed.


<b>Returns:</b>

A CBORTypeFilter object.


### WithTags

    public PeterO.Cbor.CBORTypeFilter WithTags(
        params PeterO.BigInteger[] tags);


Not documented yet.


<b>Parameters:</b>

 * <i>tags</i>: A BigInteger[] object.


<b>Returns:</b>

A CBORTypeFilter object.


### WithArrayExactLength

    public PeterO.Cbor.CBORTypeFilter WithArrayExactLength(
        int arrayLength,
        params PeterO.Cbor.CBORTypeFilter[] elements);


Copies this filter and includes CBOR arrays with an exact length to the new filter.


<b>Parameters:</b>

 * <i>arrayLength</i>: The desired maximum length of an array.

 * <i>elements</i>: An array containing the allowed types for each element in the array. There must be at least as many elements here as given in the arrayLength parameter.


<b>Returns:</b>

A CBORTypeFilter object.


<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter arrayLength is less than 0.


 * System.ArgumentNullException: 
The parameter elements is null.


 * System.ArgumentException: 
The parameter elements has fewer elements than specified in arrayLength.


### WithArrayMinLength

    public PeterO.Cbor.CBORTypeFilter WithArrayMinLength(
        int arrayLength,
        params PeterO.Cbor.CBORTypeFilter[] elements);


Copies this filter and includes CBOR arrays with at least a given length to the new filter.


<b>Parameters:</b>

 * <i>arrayLength</i>: The desired minimum length of an array.

 * <i>elements</i>: An array containing the allowed types for each element in the array. There must be at least as many elements here as given in the arrayLength parameter.


<b>Returns:</b>

A CBORTypeFilter object.


<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter arrayLength is less than 0.


 * System.ArgumentNullException: 
The parameter elements is null.


 * System.ArgumentException: 
The parameter elements has fewer elements than specified in arrayLength.


### WithArrayAnyLength

    public PeterO.Cbor.CBORTypeFilter WithArrayAnyLength();


Copies this filter and includes arrays of any length in the new filter.


<b>Returns:</b>

A CBORTypeFilter object.


### WithFloatingPoint

    public PeterO.Cbor.CBORTypeFilter WithFloatingPoint();


Copies this filter and includes floating-point numbers in the new filter.


<b>Returns:</b>

A CBORTypeFilter object.


### MajorTypeMatches

    public bool MajorTypeMatches(
        int type);


Not documented yet.


<b>Parameters:</b>

 * <i>type</i>: A 32-bit signed integer.


<b>Returns:</b>

A Boolean object.


### ArrayLengthMatches

    public bool ArrayLengthMatches(
        int length);


Returns whether an array's length is allowed under this filter.


<b>Parameters:</b>

 * <i>length</i>: The length of a CBOR array.


<b>Returns:</b>

True if an array's length is allowed under this filter; otherwise, false.


### ArrayLengthMatches

    public bool ArrayLengthMatches(
        long length);


Returns whether an array's length is allowed under a filter.


<b>Parameters:</b>

 * <i>length</i>: The length of a CBOR array.


<b>Returns:</b>

True if an array's length is allowed under a filter; otherwise, false.


### ArrayLengthMatches

    public bool ArrayLengthMatches(
        PeterO.BigInteger bigLength);


Not documented yet.


<b>Parameters:</b>

 * <i>bigLength</i>: A BigInteger object.


<b>Returns:</b>

A Boolean object.


### TagAllowed

    public bool TagAllowed(
        int tag);


Gets a value indicating whether CBOR objects can have the given tag number.


<b>Parameters:</b>

 * <i>tag</i>: A tag number. Returns false if this is less than 0.


<b>Returns:</b>

True if CBOR objects can have the given tag number; otherwise, false.


### TagAllowed

    public bool TagAllowed(
        long tag);


Gets a value indicating whether CBOR objects can have the given tag number.


<b>Parameters:</b>

 * <i>tag</i>: A tag number. Returns false if this is less than 0.


<b>Returns:</b>

True if CBOR objects can have the given tag number; otherwise, false.


### TagAllowed

    public bool TagAllowed(
        PeterO.BigInteger bigTag);


Gets a value indicating whether CBOR objects can have the given tag number.


<b>Parameters:</b>

 * <i>bigTag</i>: A tag number. Returns false if this is less than 0.


<b>Returns:</b>

True if CBOR objects can have the given tag number; otherwise, false.


### ArrayIndexAllowed

    public bool ArrayIndexAllowed(
        int index);


Not documented yet.


<b>Parameters:</b>

 * <i>index</i>: A 32-bit signed integer.


<b>Returns:</b>

A Boolean object.


### GetSubFilter

    public PeterO.Cbor.CBORTypeFilter GetSubFilter(
        int index);


Not documented yet.


<b>Parameters:</b>

 * <i>index</i>: A 32-bit signed integer.


<b>Returns:</b>

A CBORTypeFilter object.


### GetSubFilter

    public PeterO.Cbor.CBORTypeFilter GetSubFilter(
        long index);


Not documented yet.


<b>Parameters:</b>

 * <i>index</i>: A 64-bit signed integer.


<b>Returns:</b>

A CBORTypeFilter object.


### NonFPSimpleValueAllowed

    public bool NonFPSimpleValueAllowed();


Returns whether this filter allows simple values that are not floating-point numbers.


<b>Returns:</b>

True if this filter allows simple values that are not floating-point numbers; otherwise, false.


### None

    public static readonly PeterO.Cbor.CBORTypeFilter None;


A filter that allows no CBOR types.


### UnsignedInteger

    public static readonly PeterO.Cbor.CBORTypeFilter UnsignedInteger;


A filter that allows unsigned integers.


### NegativeInteger

    public static readonly PeterO.Cbor.CBORTypeFilter NegativeInteger;


A filter that allows negative integers.


### Any

    public static readonly PeterO.Cbor.CBORTypeFilter Any;


A filter that allows any CBOR object.


### ByteString

    public static readonly PeterO.Cbor.CBORTypeFilter ByteString;


A filter that allows byte strings.


### TextString

    public static readonly PeterO.Cbor.CBORTypeFilter TextString;


A filter that allows text strings.



## PeterO.Cbor.ICBORConverter<T>


    public abstract interface ICBORConverter`1<T>


Interface implemented by classes that convert objects of arbitrary types to CBOR objects.


### ToCBORObject

    public abstract virtual PeterO.Cbor.CBORObject ToCBORObject(
        T obj);


Converts an object to a CBOR object.


<b>Parameters:</b>

 * <i>obj</i>: An object to convert to a CBOR object.


<b>Returns:</b>

A CBOR object.



## PeterO.Cbor.ICBORTag


    public abstract interface ICBORTag


Implemented by classes that validate CBOR objects belonging to a specific tag.


### GetTypeFilter

    public abstract virtual PeterO.Cbor.CBORTypeFilter GetTypeFilter();


Gets a type filter specifying what kinds of CBOR objects are supported by this tag.


<b>Returns:</b>

A CBOR type filter.


### ValidateObject

    public abstract virtual PeterO.Cbor.CBORObject ValidateObject(
        PeterO.Cbor.CBORObject obj);


Generates a CBOR object based on the data of another object. If the data is not valid, should throw a CBORException.


<b>Parameters:</b>

 * <i>obj</i>: A CBOR object with the corresponding tag handled by the ICBORTag object.


<b>Returns:</b>

A CBORObject object. Note that this method may choose to return the same object as the parameter.



## PeterO.DataUtilities


    public static class DataUtilities


Contains methods useful for reading and writing strings. It is designed to have no dependencies other than the basic runtime class library.


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
The parameter <i>bytes</i>
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
The parameter <i>bytes</i>
 is null.


 * System.ArgumentException: 
The portion of the byte array is not valid UTF-8 and  <i>replace</i>
 is false.


 * System.ArgumentException: 
The parameter  <i>offset</i>
 is less than 0,  <i>bytesCount</i>
is less than 0, or offset plus bytesCount is greater than the length of "data" .


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
The parameter <i>str</i>
 is null.


 * System.ArgumentException: 
The string contains an unpaired surrogate code point and  <i>replace</i>
is false, or an internal error occurred.


 * System.ArgumentException: 
The parameter "offset" is less than 0, "bytesCount" is less than 0, or offset plus bytesCount is greater than the length of "data" .


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
The parameter <i>str</i>
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
The parameter <i>str</i>
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
The parameter <i>str</i>
 is null.


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
The parameter <i>str</i>
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
The parameter <i>str</i>
 is null.


### ToLowerCaseAscii

    public static string ToLowerCaseAscii(
        string str);


Returns a string with upper-case ASCII letters (A to Z) converted to lower-case. Other characters remain unchanged.


<b>Parameters:</b>

 * <i>str</i>: A string.


<b>Returns:</b>

The converted string, or null if  <i>str</i>
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

0 if the entire string portion was written; or -1 if the string portion contains an unpaired surrogate code point and  <i>replace</i>
 is false.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>str</i>
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

0 if the entire string portion was written; or -1 if the string portion contains an unpaired surrogate code point and  <i>replace</i>
 is false.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>str</i>
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
The parameter <i>str</i>
 is null or  <i>stream</i>
 is null.


 * System.IO.IOException: 
An I/O error occurred.


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
The parameter <i>data</i>
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
The parameter <i>stream</i>
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
The parameter <i>stream</i>
 is null.


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
The parameter <i>stream</i>
 is null or  <i>builder</i>
is null.



## PeterO.ExtendedDecimal


    public sealed class ExtendedDecimal :
        System.IComparable,
        System.IEquatable


Represents an arbitrary-precision decimal floating-point number. Consists of an integer mantissa and an integer exponent, both arbitrary-precision. The value of the number equals mantissa * 10^exponent. The mantissa is the value of the digits that make up a number, ignoring the decimal point and exponent. For example, in the number 2356.78, the mantissa is 235678. The exponent is where the "floating" decimal point of the number is located. A positive exponent means "move it to the right", and a negative exponent means "move it to the left." In the example 2,356.78, the exponent is -2, since it has 2 decimal places and the decimal point is "moved to the left by 2." Therefore, in the ExtendedDecimal representation, this number would be stored as 235678 * 10^-2.


The mantissa and exponent format preserves trailing zeros in the number's value. This may give rise to multiple ways to store the same value. For example, 1.00 and 1 would be stored differently, even though they have the same value. In the first case, 100 * 10^-2 (100 with decimal point moved left by 2), and in the second case, 1 * 10^0 (1 with decimal point moved 0).


This class also supports values for negative zero, not-a-number (NaN) values, and infinity. Negative zerois generally used when a negative number is rounded to 0; it has the same mathematical value as positive zero. Infinityis generally used when a non-zero number is divided by zero, or when a very high number can't be represented in a given exponent range.Not-a-numberis generally used to signal errors. 


This class implements the General Decimal Arithmetic Specificationversion 1.70.


Passing a signaling NaN to any arithmetic operation shown here will signal the flag FlagInvalid and return a quiet NaN, even if another operand to that operation is a quiet NaN, unless noted otherwise.


Passing a quiet NaN to any arithmetic operation shown here will return a quiet NaN, unless noted otherwise. Invalid operations will also return a quiet NaN, as stated in the individual methods.


Unless noted otherwise, passing a null ExtendedDecimal argument to any method here will throw an exception.


When an arithmetic operation signals the flag FlagInvalid, FlagOverflow, or FlagDivideByZero, it will not throw an exception too, unless the flag's trap is enabled in the precision context (see PrecisionContext's Traps property).


An ExtendedDecimal value can be serialized in one of the following ways:





 * By calling the toString() method, which will always return distinct strings for distinct ExtendedDecimal values.


 * By calling the UnsignedMantissa, Exponent, and IsNegative properties, and calling the IsInfinity, IsQuietNaN, and IsSignalingNaN methods. The return values combined will uniquely identify a particular ExtendedDecimal value.





### Equals

    public sealed bool Equals(
        PeterO.ExtendedDecimal other);


Determines whether this object's mantissa and exponent are equal to those of another object.


<b>Parameters:</b>

 * <i>other</i>: An ExtendedDecimal object.


<b>Returns:</b>

True if this object's mantissa and exponent are equal to those of another object; otherwise, false.


### Equals

    public override bool Equals(
        object obj);


Determines whether this object's mantissa and exponent are equal to those of another object and that other object is a decimal fraction.


<b>Parameters:</b>

 * <i>obj</i>: An arbitrary object.


<b>Returns:</b>

True if the objects are equal; otherwise, false.


### GetHashCode

    public override int GetHashCode();


Calculates this object's hash code.


<b>Returns:</b>

This object's hash code.


### Create

    public static PeterO.ExtendedDecimal Create(
        int mantissaSmall,
        int exponentSmall);


Creates a decimal number with the value exponent*10^mantissa.


<b>Parameters:</b>

 * <i>mantissaSmall</i>: The un-scaled value.

 * <i>exponentSmall</i>: The decimal exponent.


<b>Returns:</b>

An ExtendedDecimal object.


### CreateNaN

    public static PeterO.ExtendedDecimal CreateNaN(
        PeterO.BigInteger diag);


Not documented yet.


<b>Parameters:</b>

 * <i>diag</i>: A BigInteger object.


<b>Returns:</b>

An ExtendedDecimal object.


### CreateNaN

    public static PeterO.ExtendedDecimal CreateNaN(
        PeterO.BigInteger diag,
        bool signaling,
        bool negative,
        PeterO.PrecisionContext ctx);


Not documented yet.


<b>Parameters:</b>

 * <i>diag</i>: A BigInteger object.

 * <i>signaling</i>: A Boolean object.

 * <i>negative</i>: A Boolean object. (2).

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

An ExtendedDecimal object.


### Create

    public static PeterO.ExtendedDecimal Create(
        PeterO.BigInteger mantissa,
        PeterO.BigInteger exponent);


Creates a decimal number with the value exponent*10^mantissa.


<b>Parameters:</b>

 * <i>mantissa</i>: The un-scaled value.

 * <i>exponent</i>: The decimal exponent.


<b>Returns:</b>

An ExtendedDecimal object.


### FromString

    public static PeterO.ExtendedDecimal FromString(
        string str);


Creates a decimal number from a string that represents a number. See FromString(String, PrecisionContext) for more information.


<b>Parameters:</b>

 * <i>str</i>: A string that represents a number.


<b>Returns:</b>

An arbitrary-precision decimal number with the same value as the given string.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>str</i>
 is null.


 * System.FormatException: 
The parameter  <i>str</i>
is not a correctly formatted number string.


### FromString

    public static PeterO.ExtendedDecimal FromString(
        string str,
        PeterO.PrecisionContext ctx);


Creates a decimal number from a string that represents a number. The format of the string generally consists of:


 *  An optional '-' or '+' character (if '-', the value is negative.)


 *  One or more digits, with a single optional decimal point after the first digit and before the last digit.


 *  Optionally, E+ (positive exponent) or E- (negative exponent) plus one or more digits specifying the exponent.





The string can also be "-INF", "-Infinity", "Infinity", "INF", quiet NaN ("qNaN"/"-qNaN") followed by any number of digits, or signaling NaN ("sNaN"/"-sNaN") followed by any number of digits, all in any combination of upper and lower case.


 The format generally follows the definition in java.math.BigDecimal(), except that the digits must be ASCII digits ('0' through '9').





<b>Parameters:</b>

 * <i>str</i>: A string that represents a number.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

An arbitrary-precision decimal number with the same value as the given string.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>str</i>
 is null.


 * System.FormatException: 
The parameter  <i>str</i>
is not a correctly formatted number string.


### CompareToBinary

    public int CompareToBinary(
        PeterO.ExtendedFloat other);


Compares a ExtendedFloat object with this instance.


<b>Parameters:</b>

 * <i>other</i>: An ExtendedFloat object.


<b>Returns:</b>

Zero if the values are equal; a negative number if this instance is less, or a positive number if this instance is greater.


### ToBigInteger

    public PeterO.BigInteger ToBigInteger();


Converts this value to an arbitrary-precision integer. Any fractional part in this value will be discarded when converting to a big integer.


<b>Returns:</b>

A BigInteger object.


<b>Exceptions:</b>

 * System.OverflowException: 
This object's value is infinity or NaN.


### ToExtendedFloat

    public PeterO.ExtendedFloat ToExtendedFloat();


Creates a binary floating-point number from this object's value. Note that if the binary floating-point number contains a negative exponent, the resulting value might not be exact. However, the resulting binary float will contain enough precision to accurately convert it to a 32-bit or 64-bit floating point number (float or double).


<b>Returns:</b>

An ExtendedFloat object.


### ToSingle

    public float ToSingle();


Converts this value to a 32-bit floating-point number. The half-even rounding mode is used.If this value is a NaN, sets the high bit of the 32-bit floating point number's mantissa for a quiet NaN, and clears it for a signaling NaN. Then the next highest bit of the mantissa is cleared for a quiet NaN, and set for a signaling NaN. Then the other bits of the mantissa are set to the lowest bits of this object's unsigned mantissa. 





<b>Returns:</b>

The closest 32-bit floating-point number to this value. The return value can be positive infinity or negative infinity if this value exceeds the range of a 32-bit floating point number.


### ToDouble

    public double ToDouble();


Converts this value to a 64-bit floating-point number. The half-even rounding mode is used.If this value is a NaN, sets the high bit of the 64-bit floating point number's mantissa for a quiet NaN, and clears it for a signaling NaN. Then the next highest bit of the mantissa is cleared for a quiet NaN, and set for a signaling NaN. Then the other bits of the mantissa are set to the lowest bits of this object's unsigned mantissa. 





<b>Returns:</b>

The closest 64-bit floating-point number to this value. The return value can be positive infinity or negative infinity if this value exceeds the range of a 64-bit floating point number.


### FromSingle

    public static PeterO.ExtendedDecimal FromSingle(
        float flt);


Creates a decimal number from a 32-bit floating-point number. This method computes the exact value of the floating point number, not an approximation, as is often the case by converting the number to a string.


<b>Parameters:</b>

 * <i>flt</i>: A 32-bit floating-point number.


<b>Returns:</b>

A decimal number with the same value as  <i>flt</i>
.


### FromBigInteger

    public static PeterO.ExtendedDecimal FromBigInteger(
        PeterO.BigInteger bigint);


Converts a big integer to an arbitrary precision decimal.


<b>Parameters:</b>

 * <i>bigint</i>: A BigInteger object.


<b>Returns:</b>

An ExtendedDecimal object with the exponent set to 0.


### FromInt64

    public static PeterO.ExtendedDecimal FromInt64(
        long valueSmall);


Creates a decimal number from a 64-bit signed integer.


<b>Parameters:</b>

 * <i>valueSmall</i>: A 64-bit signed integer.


<b>Returns:</b>

An ExtendedDecimal object with the exponent set to 0.


### FromInt32

    public static PeterO.ExtendedDecimal FromInt32(
        int valueSmaller);


Creates a decimal number from a 32-bit signed integer.


<b>Parameters:</b>

 * <i>valueSmaller</i>: A 32-bit signed integer.


<b>Returns:</b>

An ExtendedDecimal object.


### FromDouble

    public static PeterO.ExtendedDecimal FromDouble(
        double dbl);


Creates a decimal number from a 64-bit floating-point number. This method computes the exact value of the floating point number, not an approximation, as is often the case by converting the number to a string.


<b>Parameters:</b>

 * <i>dbl</i>: A 64-bit floating-point number.


<b>Returns:</b>

A decimal number with the same value as  <i>dbl</i>
.


### FromExtendedFloat

    public static PeterO.ExtendedDecimal FromExtendedFloat(
        PeterO.ExtendedFloat bigfloat);


Creates a decimal number from an arbitrary-precision binary floating-point number.


<b>Parameters:</b>

 * <i>bigfloat</i>: A big floating-point number.


<b>Returns:</b>

An ExtendedDecimal object.


### ToString

    public override string ToString();


Converts this value to a string. Returns a value compatible with this class's FromString method.


<b>Returns:</b>

A string representation of this object.


### ToEngineeringString

    public string ToEngineeringString();


Same as toString(), except that when an exponent is used it will be a multiple of 3. The format of the return value follows the format of the java.math.BigDecimal.toEngineeringString() method.


<b>Returns:</b>

A string object.


### ToPlainString

    public string ToPlainString();


Converts this value to a string, but without an exponent part. The format of the return value follows the format of the java.math.BigDecimal.toPlainString() method.


<b>Returns:</b>

A string object.


### One

    public static readonly PeterO.ExtendedDecimal One;


Represents the number 1.


### Zero

    public static readonly PeterO.ExtendedDecimal Zero;


Represents the number 0.


### NegativeZero

    public static readonly PeterO.ExtendedDecimal NegativeZero;


Represents the number negative zero.


### Ten

    public static readonly PeterO.ExtendedDecimal Ten;


Represents the number 10.


### NaN

    public static readonly PeterO.ExtendedDecimal NaN;


A not-a-number value.


### SignalingNaN

    public static readonly PeterO.ExtendedDecimal SignalingNaN;


A not-a-number value that signals an invalid operation flag when it's passed as an argument to any arithmetic operation in ExtendedDecimal.


### PositiveInfinity

    public static readonly PeterO.ExtendedDecimal PositiveInfinity;


Positive infinity, greater than any other number.


### NegativeInfinity

    public static readonly PeterO.ExtendedDecimal NegativeInfinity;


Negative infinity, less than any other number.


### IsNegativeInfinity

    public bool IsNegativeInfinity();


Returns whether this object is negative infinity.


<b>Returns:</b>

True if this object is negative infinity; otherwise, false.


### IsPositiveInfinity

    public bool IsPositiveInfinity();


Returns whether this object is positive infinity.


<b>Returns:</b>

True if this object is positive infinity; otherwise, false.


### IsNaN

    public bool IsNaN();


Gets a value indicating whether this object is not a number (NaN).


<b>Returns:</b>

True if this object is not a number (NaN); otherwise, false.


### IsInfinity

    public bool IsInfinity();


Gets a value indicating whether this object is positive or negative infinity.


<b>Returns:</b>

True if this object is positive or negative infinity; otherwise, false.


### IsQuietNaN

    public bool IsQuietNaN();


Gets a value indicating whether this object is a quiet not-a-number value.


<b>Returns:</b>

True if this object is a quiet not-a-number value; otherwise, false.


### IsSignalingNaN

    public bool IsSignalingNaN();


Gets a value indicating whether this object is a signaling not-a-number value.


<b>Returns:</b>

True if this object is a signaling not-a-number value; otherwise, false.


### Abs

    public PeterO.ExtendedDecimal Abs();


Gets the absolute value of this object.


<b>Returns:</b>

An ExtendedDecimal object.


### Negate

    public PeterO.ExtendedDecimal Negate();


Gets an object with the same value as this one, but with the sign reversed.


<b>Returns:</b>

An ExtendedDecimal object.


### Divide

    public PeterO.ExtendedDecimal Divide(
        PeterO.ExtendedDecimal divisor);


Divides this object by another decimal number and returns the result. When possible, the result will be exact.


<b>Parameters:</b>

 * <i>divisor</i>: The divisor.


<b>Returns:</b>

The quotient of the two numbers. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Returns NaN if the divisor and the dividend are 0. Returns NaN if the result can't be exact because it would have a nonterminating decimal expansion.


### DivideToSameExponent

    public PeterO.ExtendedDecimal DivideToSameExponent(
        PeterO.ExtendedDecimal divisor,
        PeterO.Rounding rounding);


Divides this object by another decimal number and returns a result with the same exponent as this object (the dividend).


<b>Parameters:</b>

 * <i>divisor</i>: The divisor.

 * <i>rounding</i>: The rounding mode to use if the result must be scaled down to have the same exponent as this value.


<b>Returns:</b>

The quotient of the two numbers. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0. Signals FlagInvalid and returns NaN if the rounding mode is Rounding.Unnecessary and the result is not exact.


### DivideToIntegerNaturalScale

    public PeterO.ExtendedDecimal DivideToIntegerNaturalScale(
        PeterO.ExtendedDecimal divisor);


Divides two ExtendedDecimal objects, and returns the integer part of the result, rounded down, with the preferred exponent set to this value's exponent minus the divisor's exponent.


<b>Parameters:</b>

 * <i>divisor</i>: The divisor.


<b>Returns:</b>

The integer part of the quotient of the two objects. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0.


### Reduce

    public PeterO.ExtendedDecimal Reduce(
        PeterO.PrecisionContext ctx);


Removes trailing zeros from this object's mantissa. For example, 1.000 becomes 1.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

This value with trailing zeros removed. Note that if the result has a very high exponent and the context says to clamp high exponents, there may still be some trailing zeros in the mantissa.


### RemainderNaturalScale

    public PeterO.ExtendedDecimal RemainderNaturalScale(
        PeterO.ExtendedDecimal divisor);


Not documented yet.


<b>Parameters:</b>

 * <i>divisor</i>: An ExtendedDecimal object. (2).


<b>Returns:</b>

An ExtendedDecimal object.


### RemainderNaturalScale

    public PeterO.ExtendedDecimal RemainderNaturalScale(
        PeterO.ExtendedDecimal divisor,
        PeterO.PrecisionContext ctx);


Not documented yet.


<b>Parameters:</b>

 * <i>divisor</i>: An ExtendedDecimal object. (2).

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

An ExtendedDecimal object.


### DivideToExponent

    public PeterO.ExtendedDecimal DivideToExponent(
        PeterO.ExtendedDecimal divisor,
        long desiredExponentSmall,
        PeterO.PrecisionContext ctx);


Divides two ExtendedDecimal objects, and gives a particular exponent to the result.


<b>Parameters:</b>

 * <i>divisor</i>: An ExtendedDecimal object.

 * <i>desiredExponentSmall</i>: The desired exponent. A negative number places the cutoff point to the right of the usual decimal point. A positive number places the cutoff point to the left of the usual decimal point.

 * <i>ctx</i>: A precision context object to control the rounding mode to use if the result must be scaled down to have the same exponent as this value. If the precision given in the context is other than 0, calls the Quantize method with both arguments equal to the result of the operation (and can signal FlagInvalid and return NaN if the result doesn't fit the given precision). If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.


<b>Returns:</b>

The quotient of the two objects. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0. Signals FlagInvalid and returns NaN if the context defines an exponent range and the desired exponent is outside that range. Signals FlagInvalid and returns NaN if the rounding mode is Rounding.Unnecessary and the result is not exact.


### Divide

    public PeterO.ExtendedDecimal Divide(
        PeterO.ExtendedDecimal divisor,
        PeterO.PrecisionContext ctx);


Divides this ExtendedDecimal object by another ExtendedDecimal object. The preferred exponent for the result is this object's exponent minus the divisor's exponent.


<b>Parameters:</b>

 * <i>divisor</i>: The divisor.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The quotient of the two objects. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0; or, either  <i>ctx</i>
 is null or  <i>ctx</i>
's precision is 0, and the result would have a nonterminating decimal expansion; or, the rounding mode is Rounding.Unnecessary and the result is not exact.


### DivideToExponent

    public PeterO.ExtendedDecimal DivideToExponent(
        PeterO.ExtendedDecimal divisor,
        long desiredExponentSmall,
        PeterO.Rounding rounding);


Divides two ExtendedDecimal objects, and gives a particular exponent to the result.


<b>Parameters:</b>

 * <i>divisor</i>: An ExtendedDecimal object.

 * <i>desiredExponentSmall</i>: The desired exponent. A negative number places the cutoff point to the right of the usual decimal point. A positive number places the cutoff point to the left of the usual decimal point.

 * <i>rounding</i>: The rounding mode to use if the result must be scaled down to have the same exponent as this value.


<b>Returns:</b>

The quotient of the two objects. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0. Signals FlagInvalid and returns NaN if the rounding mode is Rounding.Unnecessary and the result is not exact.


### DivideToExponent

    public PeterO.ExtendedDecimal DivideToExponent(
        PeterO.ExtendedDecimal divisor,
        PeterO.BigInteger exponent,
        PeterO.PrecisionContext ctx);


Divides two ExtendedDecimal objects, and gives a particular exponent to the result.


<b>Parameters:</b>

 * <i>divisor</i>: An ExtendedDecimal object.

 * <i>exponent</i>: The desired exponent. A negative number places the cutoff point to the right of the usual decimal point. A positive number places the cutoff point to the left of the usual decimal point.

 * <i>ctx</i>: A precision context object to control the rounding mode to use if the result must be scaled down to have the same exponent as this value. If the precision given in the context is other than 0, calls the Quantize method with both arguments equal to the result of the operation (and can signal FlagInvalid and return NaN if the result doesn't fit the given precision). If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.


<b>Returns:</b>

The quotient of the two objects. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0. Signals FlagInvalid and returns NaN if the context defines an exponent range and the desired exponent is outside that range. Signals FlagInvalid and returns NaN if the rounding mode is Rounding.Unnecessary and the result is not exact.


### DivideToExponent

    public PeterO.ExtendedDecimal DivideToExponent(
        PeterO.ExtendedDecimal divisor,
        PeterO.BigInteger desiredExponent,
        PeterO.Rounding rounding);


Divides two ExtendedDecimal objects, and gives a particular exponent to the result.


<b>Parameters:</b>

 * <i>divisor</i>: An ExtendedDecimal object.

 * <i>desiredExponent</i>: The desired exponent. A negative number places the cutoff point to the right of the usual decimal point. A positive number places the cutoff point to the left of the usual decimal point.

 * <i>rounding</i>: The rounding mode to use if the result must be scaled down to have the same exponent as this value.


<b>Returns:</b>

The quotient of the two objects. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Returns NaN if the divisor and the dividend are 0. Returns NaN if the rounding mode is Rounding.Unnecessary and the result is not exact.


### Abs

    public PeterO.ExtendedDecimal Abs(
        PeterO.PrecisionContext context);


Finds the absolute value of this object (if it's negative, it becomes positive).


<b>Parameters:</b>

 * <i>context</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The absolute value of this object.


### Negate

    public PeterO.ExtendedDecimal Negate(
        PeterO.PrecisionContext context);


Returns a decimal number with the same value as this object but with the sign reversed.


<b>Parameters:</b>

 * <i>context</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

An ExtendedDecimal object.


### Add

    public PeterO.ExtendedDecimal Add(
        PeterO.ExtendedDecimal otherValue);


Adds this object and another decimal number and returns the result.


<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedDecimal object.


<b>Returns:</b>

The sum of the two objects.


### Subtract

    public PeterO.ExtendedDecimal Subtract(
        PeterO.ExtendedDecimal otherValue);


Subtracts an ExtendedDecimal object from this instance and returns the result.


<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedDecimal object.


<b>Returns:</b>

The difference of the two objects.


### Subtract

    public PeterO.ExtendedDecimal Subtract(
        PeterO.ExtendedDecimal otherValue,
        PeterO.PrecisionContext ctx);


Subtracts an ExtendedDecimal object from this instance.


<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedDecimal object.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The difference of the two objects.


### Multiply

    public PeterO.ExtendedDecimal Multiply(
        PeterO.ExtendedDecimal otherValue);


Multiplies two decimal numbers. The resulting exponent will be the sum of the exponents of the two decimal numbers.


<b>Parameters:</b>

 * <i>otherValue</i>: Another decimal number.


<b>Returns:</b>

The product of the two decimal numbers.


### MultiplyAndAdd

    public PeterO.ExtendedDecimal MultiplyAndAdd(
        PeterO.ExtendedDecimal multiplicand,
        PeterO.ExtendedDecimal augend);


Multiplies by one decimal number, and then adds another decimal number.


<b>Parameters:</b>

 * <i>multiplicand</i>: The value to multiply.

 * <i>augend</i>: The value to add.


<b>Returns:</b>

The result this *  <i>multiplicand</i>
 +  <i>augend</i>
.


### DivideToIntegerNaturalScale

    public PeterO.ExtendedDecimal DivideToIntegerNaturalScale(
        PeterO.ExtendedDecimal divisor,
        PeterO.PrecisionContext ctx);


Divides this object by another object, and returns the integer part of the result, with the preferred exponent set to this value's exponent minus the divisor's exponent.


<b>Parameters:</b>

 * <i>divisor</i>: The divisor.

 * <i>ctx</i>: A precision context object to control the precision, rounding, and exponent range of the integer part of the result. Flags will be set on the given context only if the context's HasFlags is true and the integer part of the result doesn't fit the precision and exponent range without rounding.


<b>Returns:</b>

The integer part of the quotient of the two objects. Signals FlagInvalid and returns NaN if the return value would overflow the exponent range. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0. Signals FlagInvalid and returns NaN if the rounding mode is Rounding.Unnecessary and the result is not exact.


### DivideToIntegerZeroScale

    public PeterO.ExtendedDecimal DivideToIntegerZeroScale(
        PeterO.ExtendedDecimal divisor,
        PeterO.PrecisionContext ctx);


Divides this object by another object, and returns the integer part of the result, with the exponent set to 0.


<b>Parameters:</b>

 * <i>divisor</i>: The divisor.

 * <i>ctx</i>: A precision context object to control the precision. The rounding and exponent range settings of this context are ignored. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The integer part of the quotient of the two objects. The exponent will be set to 0. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0, or if the result doesn't fit the given precision.


### Remainder

    public PeterO.ExtendedDecimal Remainder(
        PeterO.ExtendedDecimal divisor,
        PeterO.PrecisionContext ctx);


Finds the remainder that results when dividing two ExtendedDecimal objects.


<b>Parameters:</b>

 * <i>divisor</i>: An ExtendedDecimal object.

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

The remainder of the two objects.


### RemainderNear

    public PeterO.ExtendedDecimal RemainderNear(
        PeterO.ExtendedDecimal divisor,
        PeterO.PrecisionContext ctx);


Finds the distance to the closest multiple of the given divisor, based on the result of dividing this object's value by another object's value.


 *  If this and the other object divide evenly, the result is 0.


 * If the remainder's absolute value is less than half of the divisor's absolute value, the result has the same sign as this object and will be the distance to the closest multiple.


 * If the remainder's absolute value is more than half of the divisor's absolute value, the result has the opposite sign of this object and will be the distance to the closest multiple.


 * If the remainder's absolute value is exactly half of the divisor's absolute value, the result has the opposite sign of this object if the quotient, rounded down, is odd, and has the same sign as this object if the quotient, rounded down, is even, and the result's absolute value is half of the divisor's absolute value.


This function is also known as the "IEEE Remainder" function.


<b>Parameters:</b>

 * <i>divisor</i>: The divisor.

 * <i>ctx</i>: A precision context object to control the precision. The rounding and exponent range settings of this context are ignored (the rounding mode is always treated as HalfEven). If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The distance of the closest multiple. Signals FlagInvalid and returns NaN if the divisor is 0, or either the result of integer division (the quotient) or the remainder wouldn't fit the given precision.


### NextMinus

    public PeterO.ExtendedDecimal NextMinus(
        PeterO.PrecisionContext ctx);


Finds the largest value that's smaller than the given value.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context object to control the precision and exponent range of the result. The rounding mode from this context is ignored. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags).


<b>Returns:</b>

Returns the largest value that's less than the given value. Returns negative infinity if the result is negative infinity. Signals FlagInvalid and returns NaN if the parameter  <i>ctx</i>
is null, the precision is 0, or  <i>ctx</i>
 has an unlimited exponent range.


### NextPlus

    public PeterO.ExtendedDecimal NextPlus(
        PeterO.PrecisionContext ctx);


Finds the smallest value that's greater than the given value.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context object to control the precision and exponent range of the result. The rounding mode from this context is ignored. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags).


<b>Returns:</b>

Returns the smallest value that's greater than the given value.Signals FlagInvalid and returns NaN if the parameter  <i>ctx</i>
 is null, the precision is 0, or  <i>ctx</i>
has an unlimited exponent range.


### NextToward

    public PeterO.ExtendedDecimal NextToward(
        PeterO.ExtendedDecimal otherValue,
        PeterO.PrecisionContext ctx);


Finds the next value that is closer to the other object's value than this object's value.


<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedDecimal object.

 * <i>ctx</i>: A precision context object to control the precision and exponent range of the result. The rounding mode from this context is ignored. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags).


<b>Returns:</b>

Returns the next value that is closer to the other object' s value than this object's value. Signals FlagInvalid and returns NaN if the parameter  <i>ctx</i>
 is null, the precision is 0, or  <i>ctx</i>
 has an unlimited exponent range.


### Max

    public static PeterO.ExtendedDecimal Max(
        PeterO.ExtendedDecimal first,
        PeterO.ExtendedDecimal second,
        PeterO.PrecisionContext ctx);


Gets the greater value between two decimal numbers.


<b>Parameters:</b>

 * <i>first</i>: The first value to compare.

 * <i>second</i>: The second value to compare.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The larger value of the two objects.


### Min

    public static PeterO.ExtendedDecimal Min(
        PeterO.ExtendedDecimal first,
        PeterO.ExtendedDecimal second,
        PeterO.PrecisionContext ctx);


Gets the lesser value between two decimal numbers.


<b>Parameters:</b>

 * <i>first</i>: The first value to compare.

 * <i>second</i>: The second value to compare.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The smaller value of the two objects.


### MaxMagnitude

    public static PeterO.ExtendedDecimal MaxMagnitude(
        PeterO.ExtendedDecimal first,
        PeterO.ExtendedDecimal second,
        PeterO.PrecisionContext ctx);


Gets the greater value between two values, ignoring their signs. If the absolute values are equal, has the same effect as Max.


<b>Parameters:</b>

 * <i>first</i>: The first value to compare.

 * <i>second</i>: The second value to compare.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

An ExtendedDecimal object.


### MinMagnitude

    public static PeterO.ExtendedDecimal MinMagnitude(
        PeterO.ExtendedDecimal first,
        PeterO.ExtendedDecimal second,
        PeterO.PrecisionContext ctx);


Gets the lesser value between two values, ignoring their signs. If the absolute values are equal, has the same effect as Min.


<b>Parameters:</b>

 * <i>first</i>: An ExtendedDecimal object. (2).

 * <i>second</i>: An ExtendedDecimal object. (3).

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

An ExtendedDecimal object.


### Max

    public static PeterO.ExtendedDecimal Max(
        PeterO.ExtendedDecimal first,
        PeterO.ExtendedDecimal second);


Gets the greater value between two decimal numbers.


<b>Parameters:</b>

 * <i>first</i>: An ExtendedDecimal object.

 * <i>second</i>: An ExtendedDecimal object. (2).


<b>Returns:</b>

The larger value of the two objects.


### Min

    public static PeterO.ExtendedDecimal Min(
        PeterO.ExtendedDecimal first,
        PeterO.ExtendedDecimal second);


Gets the lesser value between two decimal numbers.


<b>Parameters:</b>

 * <i>first</i>: The first value to compare.

 * <i>second</i>: The second value to compare.


<b>Returns:</b>

The smaller value of the two objects.


### MaxMagnitude

    public static PeterO.ExtendedDecimal MaxMagnitude(
        PeterO.ExtendedDecimal first,
        PeterO.ExtendedDecimal second);


Gets the greater value between two values, ignoring their signs. If the absolute values are equal, has the same effect as Max.


<b>Parameters:</b>

 * <i>first</i>: The first value to compare.

 * <i>second</i>: The second value to compare.


<b>Returns:</b>

An ExtendedDecimal object.


### MinMagnitude

    public static PeterO.ExtendedDecimal MinMagnitude(
        PeterO.ExtendedDecimal first,
        PeterO.ExtendedDecimal second);


Gets the lesser value between two values, ignoring their signs. If the absolute values are equal, has the same effect as Min.


<b>Parameters:</b>

 * <i>first</i>: The first value to compare.

 * <i>second</i>: The second value to compare.


<b>Returns:</b>

An ExtendedDecimal object.


### CompareTo

    public sealed int CompareTo(
        PeterO.ExtendedDecimal other);


Compares the mathematical values of this object and another object, accepting NaN values. This method is not consistent with the Equals method because two different numbers with the same mathematical value, but different exponents, will compare as equal.


In this method, negative zero and positive zero are considered equal.


If this object or the other object is a quiet NaN or signaling NaN, this method will not trigger an error. Instead, NaN will compare greater than any other number, including infinity. Two different NaN values will be considered equal.





<b>Parameters:</b>

 * <i>other</i>: An ExtendedDecimal object.


<b>Returns:</b>

Less than 0 if this object's value is less than the other value, or greater than 0 if this object's value is greater than the other value or if  <i>other</i>
 is null, or 0 if both values are equal.


### CompareToWithContext

    public PeterO.ExtendedDecimal CompareToWithContext(
        PeterO.ExtendedDecimal other,
        PeterO.PrecisionContext ctx);


Compares the mathematical values of this object and another object.In this method, negative zero and positive zero are considered equal.


If this object or the other object is a quiet NaN or signaling NaN, this method returns a quiet NaN, and will signal a FlagInvalid flag if either is a signaling NaN.





<b>Parameters:</b>

 * <i>other</i>: An ExtendedDecimal object.

 * <i>ctx</i>: A precision context. The precision, rounding, and exponent range are ignored. If HasFlags of the context is true, will store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

Quiet NaN if this object or the other object is NaN, or 0 if both objects have the same value, or -1 if this object is less than the other value, or 1 if this object is greater.


### CompareToSignal

    public PeterO.ExtendedDecimal CompareToSignal(
        PeterO.ExtendedDecimal other,
        PeterO.PrecisionContext ctx);


Compares the mathematical values of this object and another object, treating quiet NaN as signaling.In this method, negative zero and positive zero are considered equal.


If this object or the other object is a quiet NaN or signaling NaN, this method will return a quiet NaN and will signal a FlagInvalid flag.





<b>Parameters:</b>

 * <i>other</i>: An ExtendedDecimal object.

 * <i>ctx</i>: A precision context. The precision, rounding, and exponent range are ignored. If HasFlags of the context is true, will store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

Quiet NaN if this object or the other object is NaN, or 0 if both objects have the same value, or -1 if this object is less than the other value, or 1 if this object is greater.


### Add

    public PeterO.ExtendedDecimal Add(
        PeterO.ExtendedDecimal otherValue,
        PeterO.PrecisionContext ctx);


Finds the sum of this object and another object. The result's exponent is set to the lower of the exponents of the two operands.


<b>Parameters:</b>

 * <i>otherValue</i>: The number to add to.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The sum of thisValue and the other object.


### Quantize

    public PeterO.ExtendedDecimal Quantize(
        PeterO.BigInteger desiredExponent,
        PeterO.PrecisionContext ctx);


Returns a decimal number with the same value but a new exponent.Note that this is not always the same as rounding to a given number of decimal places, since it can fail if the difference between this value's exponent and the desired exponent is too big, depending on the maximum precision. If rounding to a number of decimal places is desired, it's better to use the RoundToExponent and RoundToIntegral methods instead.





<b>Parameters:</b>

 * <i>desiredExponent</i>: A BigInteger object.

 * <i>ctx</i>: A precision context to control precision and rounding of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.


<b>Returns:</b>

A decimal number with the same value as this object but with the exponent changed. Signals FlagInvalid and returns NaN if an overflow error occurred, or the rounded result can't fit the given precision, or if the context defines an exponent range and the given exponent is outside that range.


### Quantize

    public PeterO.ExtendedDecimal Quantize(
        int desiredExponentSmall,
        PeterO.Rounding rounding);


Returns a decimal number with the same value as this one but a new exponent.


<b>Parameters:</b>

 * <i>desiredExponentSmall</i>: A 32-bit signed integer.

 * <i>rounding</i>: A Rounding object.


<b>Returns:</b>

A decimal number with the same value as this object but with the exponent changed. Returns NaN if the rounding mode is Rounding.Unnecessary and the result is not exact.


### Quantize

    public PeterO.ExtendedDecimal Quantize(
        int desiredExponentSmall,
        PeterO.PrecisionContext ctx);


Returns a decimal number with the same value but a new exponent.Note that this is not always the same as rounding to a given number of decimal places, since it can fail if the difference between this value's exponent and the desired exponent is too big, depending on the maximum precision. If rounding to a number of decimal places is desired, it's better to use the RoundToExponent and RoundToIntegral methods instead.





<b>Parameters:</b>

 * <i>desiredExponentSmall</i>: The desired exponent for the result. The exponent is the number of fractional digits in the result, expressed as a negative number. Can also be positive, which eliminates lower-order places from the number. For example, -3 means round to the thousandth (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A value of 0 rounds the number to an integer.

 * <i>ctx</i>: A precision context to control precision and rounding of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.


<b>Returns:</b>

A decimal number with the same value as this object but with the exponent changed. Signals FlagInvalid and returns NaN if an overflow error occurred, or the rounded result can't fit the given precision, or if the context defines an exponent range and the given exponent is outside that range.


### Quantize

    public PeterO.ExtendedDecimal Quantize(
        PeterO.ExtendedDecimal otherValue,
        PeterO.PrecisionContext ctx);


Returns a decimal number with the same value as this object but with the same exponent as another decimal number.


<b>Parameters:</b>

 * <i>otherValue</i>: A decimal number containing the desired exponent of the result. The mantissa is ignored. The exponent is the number of fractional digits in the result, expressed as a negative number. Can also be positive, which eliminates lower-order places from the number. For example, -3 means round to the thousandth (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A value of 0 rounds the number to an integer.

 * <i>ctx</i>: A precision context to control precision and rounding of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.


<b>Returns:</b>

A decimal number with the same value as this object but with the exponent changed. Signals FlagInvalid and returns NaN if an overflow error occurred, or the result can't fit the given precision without rounding. Signals FlagInvalid and returns NaN if the new exponent is outside of the valid range of the precision context, if it defines an exponent range.


### RoundToIntegralExact

    public PeterO.ExtendedDecimal RoundToIntegralExact(
        PeterO.PrecisionContext ctx);


Returns a decimal number with the same value as this object but rounded to an integer.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision and rounding of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.


<b>Returns:</b>

A decimal number with the same value as this object but rounded to an integer. Signals FlagInvalid and returns NaN if an overflow error occurred, or the result can't fit the given precision without rounding. Signals FlagInvalid and returns NaN if the new exponent must be changed to 0 when rounding and 0 is outside of the valid range of the precision context, if it defines an exponent range.


### RoundToIntegralNoRoundedFlag

    public PeterO.ExtendedDecimal RoundToIntegralNoRoundedFlag(
        PeterO.PrecisionContext ctx);


Returns a decimal number with the same value as this object but rounded to an integer, without adding the FlagInexact or FlagRounded flags.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision and rounding of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags), except that this function will never add the FlagRounded and FlagInexact flags (the only difference between this and RoundToExponentExact). Can be null, in which case the default rounding mode is HalfEven.


<b>Returns:</b>

A decimal number with the same value as this object but rounded to an integer. Signals FlagInvalid and returns NaN if an overflow error occurred, or the result can't fit the given precision without rounding. Signals FlagInvalid and returns NaN if the new exponent must be changed to 0 when rounding and 0 is outside of the valid range of the precision context, if it defines an exponent range.


### RoundToExponentExact

    public PeterO.ExtendedDecimal RoundToExponentExact(
        PeterO.BigInteger exponent,
        PeterO.PrecisionContext ctx);


Returns a decimal number with the same value as this object but rounded to an integer, and signals an invalid operation if the result would be inexact.


<b>Parameters:</b>

 * <i>exponent</i>: The minimum exponent the result can have. This is the maximum number of fractional digits in the result, expressed as a negative number. Can also be positive, which eliminates lower-order places from the number. For example, -3 means round to the thousandth (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A value of 0 rounds the number to an integer.

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

A decimal number with the same value as this object but rounded to an integer. Signals FlagInvalid and returns NaN if an overflow error occurred, or the result can't fit the given precision without rounding. Signals FlagInvalid and returns NaN if the new exponent is outside of the valid range of the precision context, if it defines an exponent range.


### RoundToExponent

    public PeterO.ExtendedDecimal RoundToExponent(
        PeterO.BigInteger exponent,
        PeterO.PrecisionContext ctx);


Returns a decimal number with the same value as this object, and rounds it to a new exponent if necessary.


<b>Parameters:</b>

 * <i>exponent</i>: The minimum exponent the result can have. This is the maximum number of fractional digits in the result, expressed as a negative number. Can also be positive, which eliminates lower-order places from the number. For example, -3 means round to the thousandth (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A value of 0 rounds the number to an integer.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.


<b>Returns:</b>

A decimal number rounded to the closest value representable in the given precision, meaning if the result can't fit the precision, additional digits are discarded to make it fit. Signals FlagInvalid and returns NaN if the new exponent must be changed when rounding and the new exponent is outside of the valid range of the precision context, if it defines an exponent range.


### RoundToExponentExact

    public PeterO.ExtendedDecimal RoundToExponentExact(
        int exponentSmall,
        PeterO.PrecisionContext ctx);


Returns a decimal number with the same value as this object but rounded to an integer, and signals an invalid operation if the result would be inexact.


<b>Parameters:</b>

 * <i>exponentSmall</i>: The minimum exponent the result can have. This is the maximum number of fractional digits in the result, expressed as a negative number. Can also be positive, which eliminates lower-order places from the number. For example, -3 means round to the thousandth (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A value of 0 rounds the number to an integer.

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

A decimal number with the same value as this object but rounded to an integer. Signals FlagInvalid and returns NaN if an overflow error occurred, or the result can't fit the given precision without rounding. Signals FlagInvalid and returns NaN if the new exponent is outside of the valid range of the precision context, if it defines an exponent range.


### RoundToExponent

    public PeterO.ExtendedDecimal RoundToExponent(
        int exponentSmall,
        PeterO.PrecisionContext ctx);


Returns a decimal number with the same value as this object, and rounds it to a new exponent if necessary.


<b>Parameters:</b>

 * <i>exponentSmall</i>: The minimum exponent the result can have. This is the maximum number of fractional digits in the result, expressed as a negative number. Can also be positive, which eliminates lower-order places from the number. For example, -3 means round to the thousandth (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A value of 0 rounds the number to an integer.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.


<b>Returns:</b>

A decimal number rounded to the closest value representable in the given precision, meaning if the result can't fit the precision, additional digits are discarded to make it fit. Signals FlagInvalid and returns NaN if the new exponent must be changed when rounding and the new exponent is outside of the valid range of the precision context, if it defines an exponent range.


### Multiply

    public PeterO.ExtendedDecimal Multiply(
        PeterO.ExtendedDecimal op,
        PeterO.PrecisionContext ctx);


Multiplies two decimal numbers. The resulting scale will be the sum of the scales of the two decimal numbers. The result's sign is positive if both operands have the same sign, and negative if they have different signs.


<b>Parameters:</b>

 * <i>op</i>: Another decimal number.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The product of the two decimal numbers.


### MultiplyAndAdd

    public PeterO.ExtendedDecimal MultiplyAndAdd(
        PeterO.ExtendedDecimal op,
        PeterO.ExtendedDecimal augend,
        PeterO.PrecisionContext ctx);


Multiplies by one value, and then adds another value.


<b>Parameters:</b>

 * <i>op</i>: The value to multiply.

 * <i>augend</i>: The value to add.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The result thisValue * multiplicand + augend.


### MultiplyAndSubtract

    public PeterO.ExtendedDecimal MultiplyAndSubtract(
        PeterO.ExtendedDecimal op,
        PeterO.ExtendedDecimal subtrahend,
        PeterO.PrecisionContext ctx);


Multiplies by one value, and then subtracts another value.


<b>Parameters:</b>

 * <i>op</i>: The value to multiply.

 * <i>subtrahend</i>: The value to subtract.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The result thisValue * multiplicand - subtrahend.


### RoundToPrecision

    public PeterO.ExtendedDecimal RoundToPrecision(
        PeterO.PrecisionContext ctx);


Rounds this object's value to a given precision, using the given rounding mode and range of exponent.


<b>Parameters:</b>

 * <i>ctx</i>: A context for controlling the precision, rounding mode, and exponent range. Can be null.


<b>Returns:</b>

The closest value to this object's value, rounded to the specified precision. Returns the same value as this object if  <i>ctx</i>
 is null or the precision and exponent range are unlimited.


### Plus

    public PeterO.ExtendedDecimal Plus(
        PeterO.PrecisionContext ctx);


Rounds this object's value to a given precision, using the given rounding mode and range of exponent, and also converts negative zero to positive zero.


<b>Parameters:</b>

 * <i>ctx</i>: A context for controlling the precision, rounding mode, and exponent range. Can be null.


<b>Returns:</b>

The closest value to this object's value, rounded to the specified precision. Returns the same value as this object if  <i>ctx</i>
 is null or the precision and exponent range are unlimited.


### RoundToBinaryPrecision

    public PeterO.ExtendedDecimal RoundToBinaryPrecision(
        PeterO.PrecisionContext ctx);


Rounds this object's value to a given maximum bit length, using the given rounding mode and range of exponent.


<b>Parameters:</b>

 * <i>ctx</i>: A context for controlling the precision, rounding mode, and exponent range. The precision is interpreted as the maximum bit length of the mantissa. Can be null.


<b>Returns:</b>

The closest value to this object's value, rounded to the specified precision. Returns the same value as this object if  <i>ctx</i>
 is null or the precision and exponent range are unlimited.


### SquareRoot

    public PeterO.ExtendedDecimal SquareRoot(
        PeterO.PrecisionContext ctx);


Finds the square root of this object's value.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). --This parameter cannot be null, as the square root function's results are generally not exact for many inputs.--.


<b>Returns:</b>

The square root. Signals the flag FlagInvalid and returns NaN if this object is less than 0 (the square root would be a complex number, but the return value is still NaN). Signals FlagInvalid and returns NaN if the parameter  <i>ctx</i>
 is null or the precision is unlimited (the context's Precision property is 0).


### Exp

    public PeterO.ExtendedDecimal Exp(
        PeterO.PrecisionContext ctx);


Finds e (the base of natural logarithms) raised to the power of this object's value.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). --This parameter cannot be null, as the exponential function's results are generally not exact.--.


<b>Returns:</b>

Exponential of this object. If this object's value is 1, returns an approximation to " e" within the given precision. Signals FlagInvalid and returns NaN if the parameter  <i>ctx</i>
is null or the precision is unlimited (the context's Precision property is 0).


### Log

    public PeterO.ExtendedDecimal Log(
        PeterO.PrecisionContext ctx);


Finds the natural logarithm of this object, that is, the power (exponent) that e (the base of natural logarithms) must be raised to in order to equal this object's value.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). --This parameter cannot be null, as the ln function's results are generally not exact.--.


<b>Returns:</b>

Ln(this object). Signals the flag FlagInvalid and returns NaN if this object is less than 0 (the result would be a complex number with a real part equal to Ln of this object's absolute value and an imaginary part equal to pi, but the return value is still NaN.). Signals FlagInvalid and returns NaN if the parameter  <i>ctx</i>
 is null or the precision is unlimited (the context's Precision property is 0). Signals no flags and returns negative infinity if this object's value is 0.


### Log10

    public PeterO.ExtendedDecimal Log10(
        PeterO.PrecisionContext ctx);


Finds the base-10 logarithm of this object, that is, the power (exponent) that the number 10 must be raised to in order to equal this object's value.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). --This parameter cannot be null, as the ln function's results are generally not exact.--.


<b>Returns:</b>

Ln(this object)/Ln(10). Signals the flag FlagInvalid and returns NaN if this object is less than 0. Signals FlagInvalid and returns NaN if the parameter  <i>ctx</i>
 is null or the precision is unlimited (the context's Precision property is 0).


### Pow

    public PeterO.ExtendedDecimal Pow(
        PeterO.ExtendedDecimal exponent,
        PeterO.PrecisionContext ctx);


Raises this object's value to the given exponent.


<b>Parameters:</b>

 * <i>exponent</i>: An ExtendedDecimal object.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags).


<b>Returns:</b>

This^exponent. Signals the flag FlagInvalid and returns NaN if this object and exponent are both 0; or if this value is less than 0 and the exponent either has a fractional part or is infinity. Signals FlagInvalid and returns NaN if the parameter  <i>ctx</i>
is null or the precision is unlimited (the context's Precision property is 0), and the exponent has a fractional part.


### Pow

    public PeterO.ExtendedDecimal Pow(
        int exponentSmall,
        PeterO.PrecisionContext ctx);


Raises this object's value to the given exponent.


<b>Parameters:</b>

 * <i>exponentSmall</i>: A 32-bit signed integer.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags).


<b>Returns:</b>

This^exponent. Signals the flag FlagInvalid and returns NaN if this object and exponent are both 0.


### Pow

    public PeterO.ExtendedDecimal Pow(
        int exponentSmall);


Raises this object's value to the given exponent.


<b>Parameters:</b>

 * <i>exponentSmall</i>: A 32-bit signed integer.


<b>Returns:</b>

This^exponent. Returns NaN if this object and exponent are both 0.


### PI

    public static PeterO.ExtendedDecimal PI(
        PeterO.PrecisionContext ctx);


Finds the constant pi.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). --This parameter cannot be null, as pi can never be represented exactly.--.


<b>Returns:</b>

Pi rounded to the given precision. Signals FlagInvalid and returns NaN if the parameter  <i>ctx</i>
 is null or the precision is unlimited (the context's Precision property is 0).


### Exponent

    public PeterO.BigInteger Exponent { get; }


Gets this object's exponent. This object's value will be an integer if the exponent is positive or zero.


<b>Returns:</b>

This object's exponent. This object's value will be an integer if the exponent is positive or zero.


### UnsignedMantissa

    public PeterO.BigInteger UnsignedMantissa { get; }


Gets the absolute value of this object's un-scaled value.


<b>Returns:</b>

The absolute value of this object's un-scaled value.


### Mantissa

    public PeterO.BigInteger Mantissa { get; }


Gets this object's un-scaled value.


<b>Returns:</b>

This object's un-scaled value. Will be negative if this object's value is negative (including a negative NaN).


### IsFinite

    public bool IsFinite { get; }


Gets a value indicating whether this object is finite (not infinity or NaN).


<b>Returns:</b>

True if this object is finite (not infinity or NaN); otherwise, false.


### IsNegative

    public bool IsNegative { get; }


Gets a value indicating whether this object is negative, including negative zero.


<b>Returns:</b>

True if this object is negative, including negative zero; otherwise, false.


### Sign

    public int Sign { get; }


Gets this value's sign: -1 if negative; 1 if positive; 0 if zero.


<b>Returns:</b>

This value's sign: -1 if negative; 1 if positive; 0 if zero.


### IsZero

    public bool IsZero { get; }


Gets a value indicating whether this object's value equals 0.


<b>Returns:</b>

True if this object's value equals 0; otherwise, false.



## PeterO.ExtendedFloat


    public sealed class ExtendedFloat :
        System.IComparable,
        System.IEquatable


Represents an arbitrary-precision binary floating-point number. Consists of an integer mantissa and an integer exponent, both arbitrary-precision. The value of the number equals mantissa * 2^exponent. This class also supports values for negative zero, not-a-number (NaN) values, and infinity.Passing a signaling NaN to any arithmetic operation shown here will signal the flag FlagInvalid and return a quiet NaN, even if another operand to that operation is a quiet NaN, unless noted otherwise.


Passing a quiet NaN to any arithmetic operation shown here will return a quiet NaN, unless noted otherwise.


Unless noted otherwise, passing a null ExtendedFloat argument to any method here will throw an exception.


When an arithmetic operation signals the flag FlagInvalid, FlagOverflow, or FlagDivideByZero, it will not throw an exception too, unless the operation's trap is enabled in the precision context (see PrecisionContext's Traps property).


An ExtendedFloat value can be serialized in one of the following ways:





 * By calling the toString() method. However, not all strings can be converted back to an ExtendedFloat without loss, especially if the string has a fractional part.


 * By calling the UnsignedMantissa, Exponent, and IsNegative properties, and calling the IsInfinity, IsQuietNaN, and IsSignalingNaN methods. The return values combined will uniquely identify a particular ExtendedFloat value.





### EqualsInternal

    public bool EqualsInternal(
        PeterO.ExtendedFloat otherValue);


Determines whether this object's mantissa and exponent are equal to those of another object.


<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedFloat object.


<b>Returns:</b>

True if this object's mantissa and exponent are equal to those of another object; otherwise, false.


### Equals

    public sealed bool Equals(
        PeterO.ExtendedFloat other);


Not documented yet.


<b>Parameters:</b>

 * <i>other</i>: An ExtendedFloat object.


<b>Returns:</b>

A Boolean object.


### Equals

    public override bool Equals(
        object obj);


Determines whether this object's mantissa and exponent are equal to those of another object and that other object is a decimal fraction.


<b>Parameters:</b>

 * <i>obj</i>: An arbitrary object.


<b>Returns:</b>

True if the objects are equal; otherwise, false.


### GetHashCode

    public override int GetHashCode();


Calculates this object's hash code.


<b>Returns:</b>

This object's hash code.


### CreateNaN

    public static PeterO.ExtendedFloat CreateNaN(
        PeterO.BigInteger diag);


Not documented yet.


<b>Parameters:</b>

 * <i>diag</i>: A BigInteger object.


<b>Returns:</b>

An ExtendedFloat object.


### CreateNaN

    public static PeterO.ExtendedFloat CreateNaN(
        PeterO.BigInteger diag,
        bool signaling,
        bool negative,
        PeterO.PrecisionContext ctx);


Not documented yet.


<b>Parameters:</b>

 * <i>diag</i>: A BigInteger object.

 * <i>signaling</i>: A Boolean object.

 * <i>negative</i>: A Boolean object. (2).

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

An ExtendedFloat object.


### Create

    public static PeterO.ExtendedFloat Create(
        int mantissaSmall,
        int exponentSmall);


Creates a number with the value exponent*2^mantissa.


<b>Parameters:</b>

 * <i>mantissaSmall</i>: The un-scaled value.

 * <i>exponentSmall</i>: The binary exponent.


<b>Returns:</b>

An ExtendedFloat object.


### Create

    public static PeterO.ExtendedFloat Create(
        PeterO.BigInteger mantissa,
        PeterO.BigInteger exponent);


Creates a number with the value exponent*2^mantissa.


<b>Parameters:</b>

 * <i>mantissa</i>: The un-scaled value.

 * <i>exponent</i>: The binary exponent.


<b>Returns:</b>

An ExtendedFloat object.


### FromString

    public static PeterO.ExtendedFloat FromString(
        string str,
        PeterO.PrecisionContext ctx);


Creates a binary float from a string that represents a number. Note that if the string contains a negative exponent, the resulting value might not be exact. However, the resulting binary float will contain enough precision to accurately convert it to a 32-bit or 64-bit floating point number (float or double). The format of the string generally consists of:


 *  An optional '-' or '+' character (if '-', the value is negative.)


 *  One or more digits, with a single optional decimal point after the first digit and before the last digit.


 *  Optionally, E+ (positive exponent) or E- (negative exponent) plus one or more digits specifying the exponent.





The string can also be "-INF", "-Infinity", "Infinity", "INF", quiet NaN ("qNaN") followed by any number of digits, or signaling NaN ("sNaN") followed by any number of digits, all in any combination of upper and lower case.


 The format generally follows the definition in java.math.BigDecimal(), except that the digits must be ASCII digits ('0' through '9').





<b>Parameters:</b>

 * <i>str</i>: A string that represents a number.

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

An ExtendedFloat object.


### FromString

    public static PeterO.ExtendedFloat FromString(
        string str);


Not documented yet.


<b>Parameters:</b>

 * <i>str</i>: A String object.


<b>Returns:</b>

An ExtendedFloat object.


### ToBigInteger

    public PeterO.BigInteger ToBigInteger();


Converts this value to an arbitrary-precision integer. Any fractional part in this value will be discarded when converting to a big integer.


<b>Returns:</b>

A BigInteger object.


<b>Exceptions:</b>

 * System.OverflowException: 
This object's value is infinity or NaN.


### ToSingle

    public float ToSingle();


Converts this value to a 32-bit floating-point number. The half-even rounding mode is used.If this value is a NaN, sets the high bit of the 32-bit floating point number's mantissa for a quiet NaN, and clears it for a signaling NaN. Then the next highest bit of the mantissa is cleared for a quiet NaN, and set for a signaling NaN. Then the other bits of the mantissa are set to the lowest bits of this object's unsigned mantissa. 





<b>Returns:</b>

The closest 32-bit floating-point number to this value. The return value can be positive infinity or negative infinity if this value exceeds the range of a 32-bit floating point number.


### ToDouble

    public double ToDouble();


Converts this value to a 64-bit floating-point number. The half-even rounding mode is used.If this value is a NaN, sets the high bit of the 64-bit floating point number's mantissa for a quiet NaN, and clears it for a signaling NaN. Then the next highest bit of the mantissa is cleared for a quiet NaN, and set for a signaling NaN. Then the other bits of the mantissa are set to the lowest bits of this object's unsigned mantissa. 





<b>Returns:</b>

The closest 64-bit floating-point number to this value. The return value can be positive infinity or negative infinity if this value exceeds the range of a 64-bit floating point number.


### FromSingle

    public static PeterO.ExtendedFloat FromSingle(
        float flt);


Creates a binary float from a 32-bit floating-point number. This method computes the exact value of the floating point number, not an approximation, as is often the case by converting the number to a string.


<b>Parameters:</b>

 * <i>flt</i>: A 32-bit floating-point number.


<b>Returns:</b>

A binary float with the same value as  <i>flt</i>
.


### FromBigInteger

    public static PeterO.ExtendedFloat FromBigInteger(
        PeterO.BigInteger bigint);


Not documented yet.


<b>Parameters:</b>

 * <i>bigint</i>: A BigInteger object.


<b>Returns:</b>

An ExtendedFloat object.


### FromInt64

    public static PeterO.ExtendedFloat FromInt64(
        long valueSmall);


Not documented yet.


<b>Parameters:</b>

 * <i>valueSmall</i>: A 64-bit signed integer.


<b>Returns:</b>

An ExtendedFloat object.


### FromInt32

    public static PeterO.ExtendedFloat FromInt32(
        int valueSmaller);


Creates a binary float from a 32-bit signed integer.


<b>Parameters:</b>

 * <i>valueSmaller</i>: A 32-bit signed integer.


<b>Returns:</b>

An ExtendedDecimal object.


### FromDouble

    public static PeterO.ExtendedFloat FromDouble(
        double dbl);


Creates a binary float from a 64-bit floating-point number. This method computes the exact value of the floating point number, not an approximation, as is often the case by converting the number to a string.


<b>Parameters:</b>

 * <i>dbl</i>: A 64-bit floating-point number.


<b>Returns:</b>

A binary float with the same value as  <i>dbl</i>
.


### ToExtendedDecimal

    public PeterO.ExtendedDecimal ToExtendedDecimal();


Not documented yet.


<b>Returns:</b>

An ExtendedDecimal object.


### ToString

    public override string ToString();


Converts this value to a string.


<b>Returns:</b>

A string representation of this object.


### ToEngineeringString

    public string ToEngineeringString();


Same as toString(), except that when an exponent is used it will be a multiple of 3. The format of the return value follows the format of the java.math.BigDecimal.toEngineeringString() method.


<b>Returns:</b>

A string object.


### ToPlainString

    public string ToPlainString();


Converts this value to a string, but without an exponent part. The format of the return value follows the format of the java.math.BigDecimal.toPlainString() method.


<b>Returns:</b>

A string object.


### One

    public static readonly PeterO.ExtendedFloat One;


Represents the number 1.


### Zero

    public static readonly PeterO.ExtendedFloat Zero;


Represents the number 0.


### NegativeZero

    public static readonly PeterO.ExtendedFloat NegativeZero;


Represents the number negative zero.


### Ten

    public static readonly PeterO.ExtendedFloat Ten;


Represents the number 10.


### NaN

    public static readonly PeterO.ExtendedFloat NaN;


A not-a-number value.


### SignalingNaN

    public static readonly PeterO.ExtendedFloat SignalingNaN;


A not-a-number value that signals an invalid operation flag when it's passed as an argument to any arithmetic operation in ExtendedFloat.


### PositiveInfinity

    public static readonly PeterO.ExtendedFloat PositiveInfinity;


Positive infinity, greater than any other number.


### NegativeInfinity

    public static readonly PeterO.ExtendedFloat NegativeInfinity;


Negative infinity, less than any other number.


### IsNegativeInfinity

    public bool IsNegativeInfinity();


Returns whether this object is negative infinity.


<b>Returns:</b>

True if this object is negative infinity; otherwise, false.


### IsPositiveInfinity

    public bool IsPositiveInfinity();


Returns whether this object is positive infinity.


<b>Returns:</b>

True if this object is positive infinity; otherwise, false.


### IsNaN

    public bool IsNaN();


Returns whether this object is a not-a-number value.


<b>Returns:</b>

True if this object is a not-a-number value; otherwise, false.


### IsInfinity

    public bool IsInfinity();


Gets a value indicating whether this object is positive or negative infinity.


<b>Returns:</b>

True if this object is positive or negative infinity; otherwise, false.


### IsQuietNaN

    public bool IsQuietNaN();


Gets a value indicating whether this object is a quiet not-a-number value.


<b>Returns:</b>

True if this object is a quiet not-a-number value; otherwise, false.


### IsSignalingNaN

    public bool IsSignalingNaN();


Gets a value indicating whether this object is a signaling not-a-number value.


<b>Returns:</b>

True if this object is a signaling not-a-number value; otherwise, false.


### Abs

    public PeterO.ExtendedFloat Abs();


Gets the absolute value of this object.


<b>Returns:</b>

An ExtendedFloat object.


### Negate

    public PeterO.ExtendedFloat Negate();


Gets an object with the same value as this one, but with the sign reversed.


<b>Returns:</b>

An ExtendedFloat object.


### Divide

    public PeterO.ExtendedFloat Divide(
        PeterO.ExtendedFloat divisor);


Divides this object by another binary float and returns the result. When possible, the result will be exact.


<b>Parameters:</b>

 * <i>divisor</i>: The divisor.


<b>Returns:</b>

The quotient of the two numbers. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0.


<b>Exceptions:</b>

 * System.ArithmeticException: 
The result can't be exact because it would have a nonterminating binary expansion.


### DivideToSameExponent

    public PeterO.ExtendedFloat DivideToSameExponent(
        PeterO.ExtendedFloat divisor,
        PeterO.Rounding rounding);


Divides this object by another binary float and returns a result with the same exponent as this object (the dividend).


<b>Parameters:</b>

 * <i>divisor</i>: The divisor.

 * <i>rounding</i>: The rounding mode to use if the result must be scaled down to have the same exponent as this value.


<b>Returns:</b>

The quotient of the two numbers. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0.


<b>Exceptions:</b>

 * System.ArithmeticException: 
The rounding mode is Rounding.Unnecessary and the result is not exact.


### DivideToIntegerNaturalScale

    public PeterO.ExtendedFloat DivideToIntegerNaturalScale(
        PeterO.ExtendedFloat divisor);


Divides two ExtendedFloat objects, and returns the integer part of the result, rounded down, with the preferred exponent set to this value's exponent minus the divisor's exponent.


<b>Parameters:</b>

 * <i>divisor</i>: The divisor.


<b>Returns:</b>

The integer part of the quotient of the two objects. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0.


### Reduce

    public PeterO.ExtendedFloat Reduce(
        PeterO.PrecisionContext ctx);


Removes trailing zeros from this object's mantissa. For example, 1.000 becomes 1.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

This value with trailing zeros removed. Note that if the result has a very high exponent and the context says to clamp high exponents, there may still be some trailing zeros in the mantissa.


### RemainderNaturalScale

    public PeterO.ExtendedFloat RemainderNaturalScale(
        PeterO.ExtendedFloat divisor);


Not documented yet.


<b>Parameters:</b>

 * <i>divisor</i>: An ExtendedFloat object. (2).


<b>Returns:</b>

An ExtendedFloat object.


### RemainderNaturalScale

    public PeterO.ExtendedFloat RemainderNaturalScale(
        PeterO.ExtendedFloat divisor,
        PeterO.PrecisionContext ctx);


Not documented yet.


<b>Parameters:</b>

 * <i>divisor</i>: An ExtendedFloat object. (2).

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

An ExtendedFloat object.


### DivideToExponent

    public PeterO.ExtendedFloat DivideToExponent(
        PeterO.ExtendedFloat divisor,
        long desiredExponentSmall,
        PeterO.PrecisionContext ctx);


Divides two ExtendedFloat objects, and gives a particular exponent to the result.


<b>Parameters:</b>

 * <i>divisor</i>: An ExtendedFloat object.

 * <i>desiredExponentSmall</i>: The desired exponent. A negative number places the cutoff point to the right of the usual decimal point. A positive number places the cutoff point to the left of the usual decimal point.

 * <i>ctx</i>: A precision context object to control the rounding mode to use if the result must be scaled down to have the same exponent as this value. If the precision given in the context is other than 0, calls the Quantize method with both arguments equal to the result of the operation (and can signal FlagInvalid and return NaN if the result doesn't fit the given precision). If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.


<b>Returns:</b>

The quotient of the two objects. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0. Signals FlagInvalid and returns NaN if the context defines an exponent range and the desired exponent is outside that range.


<b>Exceptions:</b>

 * System.ArithmeticException: 
The rounding mode is Rounding.Unnecessary and the result is not exact.


### Divide

    public PeterO.ExtendedFloat Divide(
        PeterO.ExtendedFloat divisor,
        PeterO.PrecisionContext ctx);


Divides this ExtendedFloat object by another ExtendedFloat object. The preferred exponent for the result is this object's exponent minus the divisor's exponent.


<b>Parameters:</b>

 * <i>divisor</i>: The divisor.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The quotient of the two objects. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0.


<b>Exceptions:</b>

 * System.ArithmeticException: 
Either  <i>ctx</i>
is null or  <i>ctx</i>
's precision is 0, and the result would have a nonterminating binary expansion; or, the rounding mode is Rounding.Unnecessary and the result is not exact.


### DivideToExponent

    public PeterO.ExtendedFloat DivideToExponent(
        PeterO.ExtendedFloat divisor,
        long desiredExponentSmall,
        PeterO.Rounding rounding);


Divides two ExtendedFloat objects, and gives a particular exponent to the result.


<b>Parameters:</b>

 * <i>divisor</i>: An ExtendedFloat object.

 * <i>desiredExponentSmall</i>: The desired exponent. A negative number places the cutoff point to the right of the usual decimal point. A positive number places the cutoff point to the left of the usual decimal point.

 * <i>rounding</i>: The rounding mode to use if the result must be scaled down to have the same exponent as this value.


<b>Returns:</b>

The quotient of the two objects. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0.


<b>Exceptions:</b>

 * System.ArithmeticException: 
The rounding mode is Rounding.Unnecessary and the result is not exact.


### DivideToExponent

    public PeterO.ExtendedFloat DivideToExponent(
        PeterO.ExtendedFloat divisor,
        PeterO.BigInteger exponent,
        PeterO.PrecisionContext ctx);


Divides two ExtendedFloat objects, and gives a particular exponent to the result.


<b>Parameters:</b>

 * <i>divisor</i>: An ExtendedFloat object.

 * <i>exponent</i>: The desired exponent. A negative number places the cutoff point to the right of the usual decimal point. A positive number places the cutoff point to the left of the usual decimal point.

 * <i>ctx</i>: A precision context object to control the rounding mode to use if the result must be scaled down to have the same exponent as this value. If the precision given in the context is other than 0, calls the Quantize method with both arguments equal to the result of the operation (and can signal FlagInvalid and return NaN if the result doesn't fit the given precision). If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.


<b>Returns:</b>

The quotient of the two objects. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0. Signals FlagInvalid and returns NaN if the context defines an exponent range and the desired exponent is outside that range.


<b>Exceptions:</b>

 * System.ArithmeticException: 
The rounding mode is Rounding.Unnecessary and the result is not exact.


### DivideToExponent

    public PeterO.ExtendedFloat DivideToExponent(
        PeterO.ExtendedFloat divisor,
        PeterO.BigInteger desiredExponent,
        PeterO.Rounding rounding);


Divides two ExtendedFloat objects, and gives a particular exponent to the result.


<b>Parameters:</b>

 * <i>divisor</i>: An ExtendedFloat object.

 * <i>desiredExponent</i>: The desired exponent. A negative number places the cutoff point to the right of the usual decimal point. A positive number places the cutoff point to the left of the usual decimal point.

 * <i>rounding</i>: The rounding mode to use if the result must be scaled down to have the same exponent as this value.


<b>Returns:</b>

The quotient of the two objects. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0.


<b>Exceptions:</b>

 * System.ArithmeticException: 
The rounding mode is Rounding.Unnecessary and the result is not exact.


### Abs

    public PeterO.ExtendedFloat Abs(
        PeterO.PrecisionContext context);


Finds the absolute value of this object (if it's negative, it becomes positive).


<b>Parameters:</b>

 * <i>context</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The absolute value of this object.


### Negate

    public PeterO.ExtendedFloat Negate(
        PeterO.PrecisionContext context);


Returns a binary float with the same value as this object but with the sign reversed.


<b>Parameters:</b>

 * <i>context</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

An ExtendedFloat object.


### Add

    public PeterO.ExtendedFloat Add(
        PeterO.ExtendedFloat otherValue);


Adds this object and another binary float and returns the result.


<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedFloat object.


<b>Returns:</b>

The sum of the two objects.


### Subtract

    public PeterO.ExtendedFloat Subtract(
        PeterO.ExtendedFloat otherValue);


Subtracts an ExtendedFloat object from this instance and returns the result..


<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedFloat object.


<b>Returns:</b>

The difference of the two objects.


### Subtract

    public PeterO.ExtendedFloat Subtract(
        PeterO.ExtendedFloat otherValue,
        PeterO.PrecisionContext ctx);


Subtracts an ExtendedFloat object from this instance.


<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedFloat object.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The difference of the two objects.


### Multiply

    public PeterO.ExtendedFloat Multiply(
        PeterO.ExtendedFloat otherValue);


Multiplies two binary floats. The resulting exponent will be the sum of the exponents of the two binary floats.


<b>Parameters:</b>

 * <i>otherValue</i>: Another binary float.


<b>Returns:</b>

The product of the two binary floats.


### MultiplyAndAdd

    public PeterO.ExtendedFloat MultiplyAndAdd(
        PeterO.ExtendedFloat multiplicand,
        PeterO.ExtendedFloat augend);


Multiplies by one binary float, and then adds another binary float.


<b>Parameters:</b>

 * <i>multiplicand</i>: The value to multiply.

 * <i>augend</i>: The value to add.


<b>Returns:</b>

The result this * multiplicand + augend.


### DivideToIntegerNaturalScale

    public PeterO.ExtendedFloat DivideToIntegerNaturalScale(
        PeterO.ExtendedFloat divisor,
        PeterO.PrecisionContext ctx);


Divides this object by another object, and returns the integer part of the result, with the preferred exponent set to this value's exponent minus the divisor's exponent.


<b>Parameters:</b>

 * <i>divisor</i>: The divisor.

 * <i>ctx</i>: A precision context object to control the precision, rounding, and exponent range of the integer part of the result. Flags will be set on the given context only if the context's HasFlags is true and the integer part of the result doesn't fit the precision and exponent range without rounding.


<b>Returns:</b>

The integer part of the quotient of the two objects. Returns null if the return value would overflow the exponent range. A caller can handle a null return value by treating it as positive infinity if both operands have the same sign or as negative infinity if both operands have different signs. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0.


<b>Exceptions:</b>

 * System.ArithmeticException: 
The rounding mode is Rounding.Unnecessary and the integer part of the result is not exact.


### DivideToIntegerZeroScale

    public PeterO.ExtendedFloat DivideToIntegerZeroScale(
        PeterO.ExtendedFloat divisor,
        PeterO.PrecisionContext ctx);


Divides this object by another object, and returns the integer part of the result, with the exponent set to 0.


<b>Parameters:</b>

 * <i>divisor</i>: The divisor.

 * <i>ctx</i>: A precision context object to control the precision. The rounding and exponent range settings of this context are ignored. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The integer part of the quotient of the two objects. The exponent will be set to 0. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0, or if the result doesn't fit the given precision.


### Remainder

    public PeterO.ExtendedFloat Remainder(
        PeterO.ExtendedFloat divisor,
        PeterO.PrecisionContext ctx);


Finds the remainder that results when dividing two ExtendedFloat objects.


<b>Parameters:</b>

 * <i>divisor</i>: An ExtendedFloat object.

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

The remainder of the two objects.


### RemainderNear

    public PeterO.ExtendedFloat RemainderNear(
        PeterO.ExtendedFloat divisor,
        PeterO.PrecisionContext ctx);


Finds the distance to the closest multiple of the given divisor, based on the result of dividing this object's value by another object's value.


 *  If this and the other object divide evenly, the result is 0.


 * If the remainder's absolute value is less than half of the divisor's absolute value, the result has the same sign as this object and will be the distance to the closest multiple.


 * If the remainder's absolute value is more than half of the divisor's absolute value, the result has the opposite sign of this object and will be the distance to the closest multiple.


 * If the remainder's absolute value is exactly half of the divisor's absolute value, the result has the opposite sign of this object if the quotient, rounded down, is odd, and has the same sign as this object if the quotient, rounded down, is even, and the result's absolute value is half of the divisor's absolute value.


This function is also known as the "IEEE Remainder" function.


<b>Parameters:</b>

 * <i>divisor</i>: The divisor.

 * <i>ctx</i>: A precision context object to control the precision. The rounding and exponent range settings of this context are ignored (the rounding mode is always treated as HalfEven). If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The distance of the closest multiple. Signals FlagInvalid and returns NaN if the divisor is 0, or either the result of integer division (the quotient) or the remainder wouldn't fit the given precision.


### NextMinus

    public PeterO.ExtendedFloat NextMinus(
        PeterO.PrecisionContext ctx);


Finds the largest value that's smaller than the given value.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context object to control the precision and exponent range of the result. The rounding mode from this context is ignored. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags).


<b>Returns:</b>

Returns the largest value that's less than the given value. Returns negative infinity if the result is negative infinity.


<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter  <i>ctx</i>
 is null, the precision is 0, or  <i>ctx</i>
has an unlimited exponent range.


### NextPlus

    public PeterO.ExtendedFloat NextPlus(
        PeterO.PrecisionContext ctx);


Finds the smallest value that's greater than the given value.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context object to control the precision and exponent range of the result. The rounding mode from this context is ignored. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags).


<b>Returns:</b>

Returns the smallest value that's greater than the given value.


<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter  <i>ctx</i>
 is null, the precision is 0, or  <i>ctx</i>
has an unlimited exponent range.


### NextToward

    public PeterO.ExtendedFloat NextToward(
        PeterO.ExtendedFloat otherValue,
        PeterO.PrecisionContext ctx);


Finds the next value that is closer to the other object's value than this object's value.


<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedFloat object.

 * <i>ctx</i>: A precision context object to control the precision and exponent range of the result. The rounding mode from this context is ignored. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags).


<b>Returns:</b>

Returns the next value that is closer to the other object' s value than this object's value.


<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter  <i>ctx</i>
 is null, the precision is 0, or  <i>ctx</i>
has an unlimited exponent range.


### Max

    public static PeterO.ExtendedFloat Max(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second,
        PeterO.PrecisionContext ctx);


Gets the greater value between two binary floats.


<b>Parameters:</b>

 * <i>first</i>: An ExtendedFloat object.

 * <i>second</i>: An ExtendedFloat object. (2).

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The larger value of the two objects.


### Min

    public static PeterO.ExtendedFloat Min(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second,
        PeterO.PrecisionContext ctx);


Gets the lesser value between two binary floats.


<b>Parameters:</b>

 * <i>first</i>: An ExtendedFloat object.

 * <i>second</i>: An ExtendedFloat object. (2).

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The smaller value of the two objects.


### MaxMagnitude

    public static PeterO.ExtendedFloat MaxMagnitude(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second,
        PeterO.PrecisionContext ctx);


Gets the greater value between two values, ignoring their signs. If the absolute values are equal, has the same effect as Max.


<b>Parameters:</b>

 * <i>first</i>: An ExtendedFloat object. (2).

 * <i>second</i>: An ExtendedFloat object. (3).

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

An ExtendedFloat object.


### MinMagnitude

    public static PeterO.ExtendedFloat MinMagnitude(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second,
        PeterO.PrecisionContext ctx);


Gets the lesser value between two values, ignoring their signs. If the absolute values are equal, has the same effect as Min.


<b>Parameters:</b>

 * <i>first</i>: An ExtendedFloat object. (2).

 * <i>second</i>: An ExtendedFloat object. (3).

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

An ExtendedFloat object.


### Max

    public static PeterO.ExtendedFloat Max(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second);


Gets the greater value between two binary floats.


<b>Parameters:</b>

 * <i>first</i>: An ExtendedFloat object.

 * <i>second</i>: An ExtendedFloat object. (2).


<b>Returns:</b>

The larger value of the two objects.


### Min

    public static PeterO.ExtendedFloat Min(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second);


Gets the lesser value between two binary floats.


<b>Parameters:</b>

 * <i>first</i>: An ExtendedFloat object.

 * <i>second</i>: An ExtendedFloat object. (2).


<b>Returns:</b>

The smaller value of the two objects.


### MaxMagnitude

    public static PeterO.ExtendedFloat MaxMagnitude(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second);


Gets the greater value between two values, ignoring their signs. If the absolute values are equal, has the same effect as Max.


<b>Parameters:</b>

 * <i>first</i>: An ExtendedFloat object. (2).

 * <i>second</i>: An ExtendedFloat object. (3).


<b>Returns:</b>

An ExtendedFloat object.


### MinMagnitude

    public static PeterO.ExtendedFloat MinMagnitude(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second);


Gets the lesser value between two values, ignoring their signs. If the absolute values are equal, has the same effect as Min.


<b>Parameters:</b>

 * <i>first</i>: An ExtendedFloat object. (2).

 * <i>second</i>: An ExtendedFloat object. (3).


<b>Returns:</b>

An ExtendedFloat object.


### CompareTo

    public sealed int CompareTo(
        PeterO.ExtendedFloat other);


Compares the mathematical values of this object and another object, accepting NaN values. This method is not consistent with the Equals method because two different numbers with the same mathematical value, but different exponents, will compare as equal.


In this method, negative zero and positive zero are considered equal.


If this object or the other object is a quiet NaN or signaling NaN, this method will not trigger an error. Instead, NaN will compare greater than any other number, including infinity. Two different NaN values will be considered equal.





<b>Parameters:</b>

 * <i>other</i>: An ExtendedFloat object.


<b>Returns:</b>

Less than 0 if this object's value is less than the other value, or greater than 0 if this object's value is greater than the other value or if  <i>other</i>
 is null, or 0 if both values are equal.


### CompareToWithContext

    public PeterO.ExtendedFloat CompareToWithContext(
        PeterO.ExtendedFloat other,
        PeterO.PrecisionContext ctx);


Compares the mathematical values of this object and another object.In this method, negative zero and positive zero are considered equal.


If this object or the other object is a quiet NaN or signaling NaN, this method returns a quiet NaN, and will signal a FlagInvalid flag if either is a signaling NaN.





<b>Parameters:</b>

 * <i>other</i>: An ExtendedFloat object.

 * <i>ctx</i>: A precision context. The precision, rounding, and exponent range are ignored. If HasFlags of the context is true, will store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

Quiet NaN if this object or the other object is NaN, or 0 if both objects have the same value, or -1 if this object is less than the other value, or 1 if this object is greater.


### CompareToSignal

    public PeterO.ExtendedFloat CompareToSignal(
        PeterO.ExtendedFloat other,
        PeterO.PrecisionContext ctx);


Compares the mathematical values of this object and another object, treating quiet NaN as signaling.In this method, negative zero and positive zero are considered equal.


If this object or the other object is a quiet NaN or signaling NaN, this method will return a quiet NaN and will signal a FlagInvalid flag.





<b>Parameters:</b>

 * <i>other</i>: An ExtendedFloat object.

 * <i>ctx</i>: A precision context. The precision, rounding, and exponent range are ignored. If HasFlags of the context is true, will store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

Quiet NaN if this object or the other object is NaN, or 0 if both objects have the same value, or -1 if this object is less than the other value, or 1 if this object is greater.


### Add

    public PeterO.ExtendedFloat Add(
        PeterO.ExtendedFloat otherValue,
        PeterO.PrecisionContext ctx);


Finds the sum of this object and another object. The result's exponent is set to the lower of the exponents of the two operands.


<b>Parameters:</b>

 * <i>otherValue</i>: The number to add to.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The sum of thisValue and the other object.


### Quantize

    public PeterO.ExtendedFloat Quantize(
        PeterO.BigInteger desiredExponent,
        PeterO.PrecisionContext ctx);


Returns a binary float with the same value but a new exponent.


<b>Parameters:</b>

 * <i>desiredExponent</i>: A BigInteger object.

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

A binary float with the same value as this object but with the exponent changed. Signals FlagInvalid and returns NaN if an overflow error occurred, or the rounded result can't fit the given precision, or if the context defines an exponent range and the given exponent is outside that range.


### Quantize

    public PeterO.ExtendedFloat Quantize(
        int desiredExponentSmall,
        PeterO.PrecisionContext ctx);


Returns a binary float with the same value but a new exponent.


<b>Parameters:</b>

 * <i>desiredExponentSmall</i>: A 32-bit signed integer.

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

A binary float with the same value as this object but with the exponent changed. Signals FlagInvalid and returns NaN if an overflow error occurred, or the rounded result can't fit the given precision, or if the context defines an exponent range and the given exponent is outside that range.


### Quantize

    public PeterO.ExtendedFloat Quantize(
        PeterO.ExtendedFloat otherValue,
        PeterO.PrecisionContext ctx);


Returns a binary float with the same value as this object but with the same exponent as another binary float.


<b>Parameters:</b>

 * <i>otherValue</i>: A binary float containing the desired exponent of the result. The mantissa is ignored. The exponent is the number of fractional digits in the result, expressed as a negative number. Can also be positive, which eliminates lower-order places from the number. For example, -3 means round to the thousandth (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A value of 0 rounds the number to an integer.

 * <i>ctx</i>: A precision context to control precision and rounding of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.


<b>Returns:</b>

A binary float with the same value as this object but with the exponent changed. Signals FlagInvalid and returns NaN if an overflow error occurred, or the result can't fit the given precision without rounding. Signals FlagInvalid and returns NaN if the new exponent is outside of the valid range of the precision context, if it defines an exponent range.


### RoundToIntegralExact

    public PeterO.ExtendedFloat RoundToIntegralExact(
        PeterO.PrecisionContext ctx);


Returns a binary float with the same value as this object but rounded to an integer.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision and rounding of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.


<b>Returns:</b>

A binary float with the same value as this object but rounded to an integer. Signals FlagInvalid and returns NaN if an overflow error occurred, or the result can't fit the given precision without rounding. Signals FlagInvalid and returns NaN if the new exponent must be changed to 0 when rounding and 0 is outside of the valid range of the precision context, if it defines an exponent range.


### RoundToIntegralNoRoundedFlag

    public PeterO.ExtendedFloat RoundToIntegralNoRoundedFlag(
        PeterO.PrecisionContext ctx);


Returns a binary float with the same value as this object but rounded to an integer, without adding the FlagInexact or FlagRounded flags.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision and rounding of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags), except that this function will never add the FlagRounded and FlagInexact flags (the only difference between this and RoundToExponentExact). Can be null, in which case the default rounding mode is HalfEven.


<b>Returns:</b>

A binary float with the same value as this object but rounded to an integer. Signals FlagInvalid and returns NaN if an overflow error occurred, or the result can't fit the given precision without rounding. Signals FlagInvalid and returns NaN if the new exponent must be changed to 0 when rounding and 0 is outside of the valid range of the precision context, if it defines an exponent range.


### RoundToExponentExact

    public PeterO.ExtendedFloat RoundToExponentExact(
        PeterO.BigInteger exponent,
        PeterO.PrecisionContext ctx);


Returns a binary float with the same value as this object but rounded to an integer.


<b>Parameters:</b>

 * <i>exponent</i>: A BigInteger object.

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

A binary float with the same value as this object but rounded to an integer. Signals FlagInvalid and returns NaN if an overflow error occurred, or the result can't fit the given precision without rounding. Signals FlagInvalid and returns NaN if the new exponent is outside of the valid range of the precision context, if it defines an exponent range.


### RoundToExponent

    public PeterO.ExtendedFloat RoundToExponent(
        PeterO.BigInteger exponent,
        PeterO.PrecisionContext ctx);


Returns a binary float with the same value as this object, and rounds it to a new exponent if necessary.


<b>Parameters:</b>

 * <i>exponent</i>: The minimum exponent the result can have. This is the maximum number of fractional digits in the result, expressed as a negative number. Can also be positive, which eliminates lower-order places from the number. For example, -3 means round to the thousandth (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A value of 0 rounds the number to an integer.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.


<b>Returns:</b>

A binary float rounded to the closest value representable in the given precision, meaning if the result can't fit the precision, additional digits are discarded to make it fit. Signals FlagInvalid and returns NaN if the new exponent must be changed when rounding and the new exponent is outside of the valid range of the precision context, if it defines an exponent range.


### Multiply

    public PeterO.ExtendedFloat Multiply(
        PeterO.ExtendedFloat op,
        PeterO.PrecisionContext ctx);


Multiplies two binary floats. The resulting scale will be the sum of the scales of the two binary floats. The result's sign is positive if both operands have the same sign, and negative if they have different signs.


<b>Parameters:</b>

 * <i>op</i>: Another binary float.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The product of the two binary floats.


### MultiplyAndAdd

    public PeterO.ExtendedFloat MultiplyAndAdd(
        PeterO.ExtendedFloat op,
        PeterO.ExtendedFloat augend,
        PeterO.PrecisionContext ctx);


Multiplies by one value, and then adds another value.


<b>Parameters:</b>

 * <i>op</i>: The value to multiply.

 * <i>augend</i>: The value to add.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The result thisValue * multiplicand + augend.


### MultiplyAndSubtract

    public PeterO.ExtendedFloat MultiplyAndSubtract(
        PeterO.ExtendedFloat op,
        PeterO.ExtendedFloat subtrahend,
        PeterO.PrecisionContext ctx);


Multiplies by one value, and then subtracts another value.


<b>Parameters:</b>

 * <i>op</i>: The value to multiply.

 * <i>subtrahend</i>: The value to subtract.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.


<b>Returns:</b>

The result thisValue * multiplicand - subtrahend.


### RoundToPrecision

    public PeterO.ExtendedFloat RoundToPrecision(
        PeterO.PrecisionContext ctx);


Rounds this object's value to a given precision, using the given rounding mode and range of exponent.


<b>Parameters:</b>

 * <i>ctx</i>: A context for controlling the precision, rounding mode, and exponent range. Can be null.


<b>Returns:</b>

The closest value to this object's value, rounded to the specified precision. Returns the same value as this object if  <i>ctx</i>
 is null or the precision and exponent range are unlimited.


### Plus

    public PeterO.ExtendedFloat Plus(
        PeterO.PrecisionContext ctx);


Rounds this object's value to a given precision, using the given rounding mode and range of exponent, and also converts negative zero to positive zero.


<b>Parameters:</b>

 * <i>ctx</i>: A context for controlling the precision, rounding mode, and exponent range. Can be null.


<b>Returns:</b>

The closest value to this object's value, rounded to the specified precision. Returns the same value as this object if  <i>ctx</i>
 is null or the precision and exponent range are unlimited.


### RoundToBinaryPrecision

    public PeterO.ExtendedFloat RoundToBinaryPrecision(
        PeterO.PrecisionContext ctx);


Rounds this object's value to a given maximum bit length, using the given rounding mode and range of exponent.


<b>Parameters:</b>

 * <i>ctx</i>: A context for controlling the precision, rounding mode, and exponent range. The precision is interpreted as the maximum bit length of the mantissa. Can be null.


<b>Returns:</b>

The closest value to this object's value, rounded to the specified precision. Returns the same value as this object if  <i>ctx</i>
 is null or the precision and exponent range are unlimited.


### SquareRoot

    public PeterO.ExtendedFloat SquareRoot(
        PeterO.PrecisionContext ctx);


Finds the square root of this object's value.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). --This parameter cannot be null, as the square root function's results are generally not exact for many inputs.--.


<b>Returns:</b>

The square root. Signals the flag FlagInvalid and returns NaN if this object is less than 0 (the square root would be a complex number, but the return value is still NaN).


<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter  <i>ctx</i>
 is null or the precision is unlimited (the context's Precision property is 0).


### Exp

    public PeterO.ExtendedFloat Exp(
        PeterO.PrecisionContext ctx);


Finds e (the base of natural logarithms) raised to the power of this object's value.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). --This parameter cannot be null, as the exponential function's results are generally not exact.--.


<b>Returns:</b>

Exponential of this object. If this object's value is 1, returns an approximation to " e" within the given precision.


<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter  <i>ctx</i>
 is null or the precision is unlimited (the context's Precision property is 0).


### Log

    public PeterO.ExtendedFloat Log(
        PeterO.PrecisionContext ctx);


Finds the natural logarithm of this object, that is, the power (exponent) that e (the base of natural logarithms) must be raised to in order to equal this object's value.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). --This parameter cannot be null, as the ln function's results are generally not exact.--.


<b>Returns:</b>

Ln(this object). Signals the flag FlagInvalid and returns NaN if this object is less than 0 (the result would be a complex number with a real part equal to Ln of this object's absolute value and an imaginary part equal to pi, but the return value is still NaN.).


<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter  <i>ctx</i>
 is null or the precision is unlimited (the context's Precision property is 0).


### Log10

    public PeterO.ExtendedFloat Log10(
        PeterO.PrecisionContext ctx);


Finds the base-10 logarithm of this object, that is, the power (exponent) that the number 10 must be raised to in order to equal this object's value.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). --This parameter cannot be null, as the ln function's results are generally not exact.--.


<b>Returns:</b>

Ln(this object)/Ln(10). Signals the flag FlagInvalid and returns NaN if this object is less than 0. Signals FlagInvalid and returns NaN if the parameter  <i>ctx</i>
 is null or the precision is unlimited (the context's Precision property is 0).


### Pow

    public PeterO.ExtendedFloat Pow(
        PeterO.ExtendedFloat exponent,
        PeterO.PrecisionContext ctx);


Raises this object's value to the given exponent.


<b>Parameters:</b>

 * <i>exponent</i>: An ExtendedFloat object.

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

This^exponent. Signals the flag FlagInvalid and returns NaN if this object and exponent are both 0; or if this value is less than 0 and the exponent either has a fractional part or is infinity.


<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter  <i>ctx</i>
 is null or the precision is unlimited (the context's Precision property is 0), and the exponent has a fractional part.


### Pow

    public PeterO.ExtendedFloat Pow(
        int exponentSmall,
        PeterO.PrecisionContext ctx);


Raises this object's value to the given exponent.


<b>Parameters:</b>

 * <i>exponentSmall</i>: A 32-bit signed integer.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags).


<b>Returns:</b>

This^exponent. Signals the flag FlagInvalid and returns NaN if this object and exponent are both 0.


### Pow

    public PeterO.ExtendedFloat Pow(
        int exponentSmall);


Raises this object's value to the given exponent.


<b>Parameters:</b>

 * <i>exponentSmall</i>: A 32-bit signed integer.


<b>Returns:</b>

This^exponent. Returns NaN if this object and exponent are both 0.


### PI

    public static PeterO.ExtendedFloat PI(
        PeterO.PrecisionContext ctx);


Finds the constant pi.


<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). --This parameter cannot be null, as pi can never be represented exactly.--.


<b>Returns:</b>

Pi rounded to the given precision.


<b>Exceptions:</b>

 * System.ArgumentException: 
The parameter  <i>ctx</i>
 is null or the precision is unlimited (the context's Precision property is 0).


### Exponent

    public PeterO.BigInteger Exponent { get; }


Gets this object's exponent. This object's value will be an integer if the exponent is positive or zero.


<b>Returns:</b>

This object's exponent. This object's value will be an integer if the exponent is positive or zero.


### UnsignedMantissa

    public PeterO.BigInteger UnsignedMantissa { get; }


Gets the absolute value of this object's un-scaled value.


<b>Returns:</b>

The absolute value of this object's un-scaled value.


### Mantissa

    public PeterO.BigInteger Mantissa { get; }


Gets this object's un-scaled value.


<b>Returns:</b>

This object's un-scaled value. Will be negative if this object's value is negative (including a negative NaN).


### IsFinite

    public bool IsFinite { get; }


Gets a value indicating whether this object is finite (not infinity or NaN).


<b>Returns:</b>

True if this object is finite (not infinity or NaN); otherwise, false.


### IsNegative

    public bool IsNegative { get; }


Gets a value indicating whether this object is negative, including negative zero.


<b>Returns:</b>

True if this object is negative, including negative zero; otherwise, false.


### Sign

    public int Sign { get; }


Gets this value's sign: -1 if negative; 1 if positive; 0 if zero.


<b>Returns:</b>

This value's sign: -1 if negative; 1 if positive; 0 if zero.


### IsZero

    public bool IsZero { get; }


Gets a value indicating whether this object's value equals 0.


<b>Returns:</b>

True if this object's value equals 0; otherwise, false.



## PeterO.ExtendedRational


    public class ExtendedRational :
        System.IComparable,
        System.IEquatable


Arbitrary-precision rational number.


### Equals

    public override bool Equals(
        object obj);


Determines whether this object and another object are equal.


<b>Parameters:</b>

 * <i>obj</i>: An arbitrary object.


<b>Returns:</b>

True if the objects are equal; otherwise, false.


### GetHashCode

    public override int GetHashCode();


Returns the hash code for this instance.


<b>Returns:</b>

A 32-bit hash code.


### ExtendedRational Constructor

    public ExtendedRational(
        PeterO.BigInteger numerator,
        PeterO.BigInteger denominator);


Initializes a new instance of the ExtendedRational class.


<b>Parameters:</b>

 * <i>numerator</i>: A BigInteger object.

 * <i>denominator</i>: A BigInteger object. (2).


### ToString

    public override string ToString();


Converts this object to a text string.


<b>Returns:</b>

A string representation of this object. The result can be Infinity, NaN, or sNaN (with a minus sign before it for negative values), or a number of the following form: [-]numerator/denominator.


### FromBigInteger

    public static PeterO.ExtendedRational FromBigInteger(
        PeterO.BigInteger bigint);


Not documented yet.


<b>Parameters:</b>

 * <i>bigint</i>: A BigInteger object.


<b>Returns:</b>

An ExtendedRational object.


### ToExtendedDecimal

    public PeterO.ExtendedDecimal ToExtendedDecimal();


Converts this rational number to a decimal number.


<b>Returns:</b>

The exact value of the rational number, or not-a-number (NaN) if the result can't be exact because it has a nonterminating decimal expansion.


### FromSingle

    public static PeterO.ExtendedRational FromSingle(
        float flt);


Converts a 32-bit floating-point number to a rational number. This method computes the exact value of the floating point number, not an approximation, as is often the case by converting the number to a string.


<b>Parameters:</b>

 * <i>flt</i>: A 32-bit floating-point number.


<b>Returns:</b>

A rational number with the same value as  <i>flt</i>
.


### FromDouble

    public static PeterO.ExtendedRational FromDouble(
        double flt);


Converts a 64-bit floating-point number to a rational number. This method computes the exact value of the floating point number, not an approximation, as is often the case by converting the number to a string.


<b>Parameters:</b>

 * <i>flt</i>: A 64-bit floating-point number.


<b>Returns:</b>

A rational number with the same value as  <i>flt</i>
.


### CreateNaN

    public static PeterO.ExtendedRational CreateNaN(
        PeterO.BigInteger diag);


Not documented yet.


<b>Parameters:</b>

 * <i>diag</i>: A BigInteger object.


<b>Returns:</b>

An ExtendedRational object.


### CreateNaN

    public static PeterO.ExtendedRational CreateNaN(
        PeterO.BigInteger diag,
        bool signaling,
        bool negative);


Not documented yet.


<b>Parameters:</b>

 * <i>diag</i>: A BigInteger object.

 * <i>signaling</i>: A Boolean object.

 * <i>negative</i>: A Boolean object. (2).


<b>Returns:</b>

An ExtendedRational object.


### FromExtendedFloat

    public static PeterO.ExtendedRational FromExtendedFloat(
        PeterO.ExtendedFloat ef);


Not documented yet.


<b>Parameters:</b>

 * <i>ef</i>: An ExtendedFloat object.


<b>Returns:</b>

An ExtendedRational object.


### FromExtendedDecimal

    public static PeterO.ExtendedRational FromExtendedDecimal(
        PeterO.ExtendedDecimal ef);


Not documented yet.


<b>Parameters:</b>

 * <i>ef</i>: An ExtendedDecimal object.


<b>Returns:</b>

An ExtendedRational object.


### ToExtendedDecimal

    public PeterO.ExtendedDecimal ToExtendedDecimal(
        PeterO.PrecisionContext ctx);


Converts this rational number to a decimal number and rounds the result to the given precision.


<b>Parameters:</b>

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

An ExtendedDecimal object.


### ToExtendedDecimalExactIfPossible

    public PeterO.ExtendedDecimal ToExtendedDecimalExactIfPossible(
        PeterO.PrecisionContext ctx);


Converts this rational number to a decimal number, but if the result would have a nonterminating decimal expansion, rounds that result to the given precision.


<b>Parameters:</b>

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

An ExtendedDecimal object.


### ToExtendedFloat

    public PeterO.ExtendedFloat ToExtendedFloat();


Converts this rational number to a binary number.


<b>Returns:</b>

The exact value of the rational number, or not-a-number (NaN) if the result can't be exact because it has a nonterminating binary expansion.


### ToExtendedFloat

    public PeterO.ExtendedFloat ToExtendedFloat(
        PeterO.PrecisionContext ctx);


Converts this rational number to a binary number and rounds the result to the given precision.


<b>Parameters:</b>

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

An ExtendedFloat object.


### ToExtendedFloatExactIfPossible

    public PeterO.ExtendedFloat ToExtendedFloatExactIfPossible(
        PeterO.PrecisionContext ctx);


Converts this rational number to a binary number, but if the result would have a nonterminating binary expansion, rounds that result to the given precision.


<b>Parameters:</b>

 * <i>ctx</i>: A PrecisionContext object.


<b>Returns:</b>

An ExtendedFloat object.


### ToBigInteger

    public PeterO.BigInteger ToBigInteger();


Converts this value to an arbitrary-precision integer. Any fractional part in this value will be discarded when converting to a big integer.


<b>Returns:</b>

A BigInteger object.


### FromInt32

    public static PeterO.ExtendedRational FromInt32(
        int smallint);


Not documented yet.


<b>Parameters:</b>

 * <i>smallint</i>: A 32-bit signed integer.


<b>Returns:</b>

An ExtendedRational object.


### FromInt64

    public static PeterO.ExtendedRational FromInt64(
        long longInt);


Not documented yet.


<b>Parameters:</b>

 * <i>longInt</i>: A 64-bit signed integer.


<b>Returns:</b>

An ExtendedRational object.


### ToDouble

    public double ToDouble();


Converts this value to a 64-bit floating-point number. The half-even rounding mode is used.


<b>Returns:</b>

The closest 64-bit floating-point number to this value. The return value can be positive infinity or negative infinity if this value exceeds the range of a 64-bit floating point number.


### ToSingle

    public float ToSingle();


Converts this value to a 32-bit floating-point number. The half-even rounding mode is used.


<b>Returns:</b>

The closest 32-bit floating-point number to this value. The return value can be positive infinity or negative infinity if this value exceeds the range of a 32-bit floating point number.


### Abs

    public PeterO.ExtendedRational Abs();


Not documented yet.


<b>Returns:</b>

An ExtendedRational object.


### Negate

    public PeterO.ExtendedRational Negate();


Not documented yet.


<b>Returns:</b>

An ExtendedRational object.


### CompareTo

    public sealed int CompareTo(
        PeterO.ExtendedRational other);


Compares an ExtendedRational object with this instance.


<b>Parameters:</b>

 * <i>other</i>: An ExtendedRational object.


<b>Returns:</b>

Zero if the values are equal; a negative number if this instance is less, or a positive number if this instance is greater.


### CompareToBinary

    public int CompareToBinary(
        PeterO.ExtendedFloat other);


Compares an ExtendedFloat object with this instance.


<b>Parameters:</b>

 * <i>other</i>: An ExtendedFloat object.


<b>Returns:</b>

Zero if the values are equal; a negative number if this instance is less, or a positive number if this instance is greater.


### CompareToDecimal

    public int CompareToDecimal(
        PeterO.ExtendedDecimal other);


Compares an ExtendedDecimal object with this instance.


<b>Parameters:</b>

 * <i>other</i>: An ExtendedDecimal object.


<b>Returns:</b>

Zero if the values are equal; a negative number if this instance is less, or a positive number if this instance is greater.


### Equals

    public sealed bool Equals(
        PeterO.ExtendedRational other);


Not documented yet.


<b>Parameters:</b>

 * <i>other</i>: An ExtendedRational object.


<b>Returns:</b>

A Boolean object.


### IsNegativeInfinity

    public bool IsNegativeInfinity();


Returns whether this object is negative infinity.


<b>Returns:</b>

True if this object is negative infinity; otherwise, false.


### IsPositiveInfinity

    public bool IsPositiveInfinity();


Returns whether this object is positive infinity.


<b>Returns:</b>

True if this object is positive infinity; otherwise, false.


### IsNaN

    public bool IsNaN();


Not documented yet.


<b>Returns:</b>

A Boolean object.


### IsInfinity

    public bool IsInfinity();


Gets a value indicating whether this object's value is infinity.


<b>Returns:</b>

True if this object's value is infinity; otherwise, false.


### IsQuietNaN

    public bool IsQuietNaN();


Not documented yet.


<b>Returns:</b>

A Boolean object.


### IsSignalingNaN

    public bool IsSignalingNaN();


Not documented yet.


<b>Returns:</b>

A Boolean object.


### NaN

    public static readonly PeterO.ExtendedRational NaN;


A not-a-number value.


### SignalingNaN

    public static readonly PeterO.ExtendedRational SignalingNaN;


A signaling not-a-number value.


### PositiveInfinity

    public static readonly PeterO.ExtendedRational PositiveInfinity;


Positive infinity, greater than any other number.


### NegativeInfinity

    public static readonly PeterO.ExtendedRational NegativeInfinity;


Negative infinity, less than any other number.


### Add

    public PeterO.ExtendedRational Add(
        PeterO.ExtendedRational otherValue);


Not documented yet.


<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedRational object. (2).


<b>Returns:</b>

An ExtendedRational object.


### Subtract

    public PeterO.ExtendedRational Subtract(
        PeterO.ExtendedRational otherValue);


Subtracts an ExtendedRational object from this instance.


<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedRational object.


<b>Returns:</b>

The difference of the two objects.


### Multiply

    public PeterO.ExtendedRational Multiply(
        PeterO.ExtendedRational otherValue);


Multiplies this instance by the value of an ExtendedRational object.


<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedRational object.


<b>Returns:</b>

The product of the two objects.


### Divide

    public PeterO.ExtendedRational Divide(
        PeterO.ExtendedRational otherValue);


Divides this instance by the value of an ExtendedRational object.


<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedRational object.


<b>Returns:</b>

The quotient of the two objects.


### Remainder

    public PeterO.ExtendedRational Remainder(
        PeterO.ExtendedRational otherValue);


Finds the remainder that results when this instance is divided by the value of a ExtendedRational object.


<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedRational object.


<b>Returns:</b>

The remainder of the two objects.


### Zero

    public static readonly PeterO.ExtendedRational Zero;


A rational number for zero.


### NegativeZero

    public static readonly PeterO.ExtendedRational NegativeZero;


A rational number for negative zero.


### One

    public static readonly PeterO.ExtendedRational One;


The rational number one.


### Ten

    public static readonly PeterO.ExtendedRational Ten;


The rational number ten.


### Numerator

    public PeterO.BigInteger Numerator { get; }


Gets this object's numerator.


<b>Returns:</b>

This object's numerator. If this object is a not-a-number value, returns the diagnostic information (which will be negative if this object is negative).


### UnsignedNumerator

    public PeterO.BigInteger UnsignedNumerator { get; }


Gets this object's numerator with the sign removed.


<b>Returns:</b>

This object's numerator. If this object is a not-a-number value, returns the diagnostic information.


### Denominator

    public PeterO.BigInteger Denominator { get; }


Gets this object's denominator.


<b>Returns:</b>

This object's denominator.


### IsFinite

    public bool IsFinite { get; }


Gets a value indicating whether this object is finite (not infinity or NaN).


<b>Returns:</b>

True if this object is finite (not infinity or NaN); otherwise, false.


### IsZero

    public bool IsZero { get; }


Gets a value indicating whether this object's value equals 0.


<b>Returns:</b>

True if this object's value equals 0; otherwise, false.


### Sign

    public int Sign { get; }


Gets a value not documented yet.


<b>Returns:</b>

A value not documented yet.


### IsNegative

    public bool IsNegative { get; }


Gets a value indicating whether this object's value is negative (including negative zero).


<b>Returns:</b>

True if this object's value is negative; otherwise, false.



## PeterO.PrecisionContext


    public class PrecisionContext


Contains parameters for controlling the precision, rounding, and exponent range of arbitrary-precision numbers.


### FlagInexact

    public static int FlagInexact = 1;


Signals that the result was rounded to a different mathematical value, but as close as possible to the original.


### FlagRounded

    public static int FlagRounded = 2;


Signals that the result was rounded to fit the precision; either the value or the exponent may have changed from the original.


### FlagSubnormal

    public static int FlagSubnormal = 4;


Signals that the result's exponent, before rounding, is lower than the lowest exponent allowed.


### FlagUnderflow

    public static int FlagUnderflow = 8;


Signals that the result's exponent, before rounding, is lower than the lowest exponent allowed, and the result was rounded to a different mathematical value, but as close as possible to the original.


### FlagOverflow

    public static int FlagOverflow = 16;


Signals that the result is non-zero and the exponent is higher than the highest exponent allowed.


### FlagClamped

    public static int FlagClamped = 32;


Signals that the exponent was adjusted to fit the exponent range.


### FlagInvalid

    public static int FlagInvalid = 64;


Signals an invalid operation.


### FlagDivideByZero

    public static int FlagDivideByZero = 128;


Signals a division of a nonzero number by zero.


### FlagLostDigits

    public static int FlagLostDigits = 256;


Signals that an operand was rounded to a different mathematical value before an operation.


### ExponentWithinRange

    public bool ExponentWithinRange(
        PeterO.BigInteger exponent);


Not documented yet.


<b>Parameters:</b>

 * <i>exponent</i>: A BigInteger object.


<b>Returns:</b>

A Boolean object.


### ToString

    public override string ToString();


Gets a string representation of this object. Note that the format is not intended to be parsed and may change at any time.


<b>Returns:</b>

A string representation of this object.


### WithRounding

    public PeterO.PrecisionContext WithRounding(
        PeterO.Rounding rounding);


Copies this PrecisionContext with the specified rounding mode.


<b>Parameters:</b>

 * <i>rounding</i>: A Rounding object.


<b>Returns:</b>

A PrecisionContext object.


### WithBlankFlags

    public PeterO.PrecisionContext WithBlankFlags();


Copies this PrecisionContext with HasFlags set to true and a Flags value of 0.


<b>Returns:</b>

A PrecisionContext object.


### WithTraps

    public PeterO.PrecisionContext WithTraps(
        int traps);


Copies this PrecisionContext with Traps set to the given value.


<b>Parameters:</b>

 * <i>traps</i>: Flags representing the traps to enable. See the property "Traps".


<b>Returns:</b>

A PrecisionContext object.


### WithExponentClamp

    public PeterO.PrecisionContext WithExponentClamp(
        bool clamp);


Copies this precision context and sets the copy's "ClampNormalExponents" flag to the given value.


<b>Parameters:</b>

 * <i>clamp</i>: A Boolean object.


<b>Returns:</b>

A PrecisionContext object.


### WithExponentRange

    public PeterO.PrecisionContext WithExponentRange(
        int exponentMinSmall,
        int exponentMaxSmall);


Copies this precision context and sets the copy's exponent range.


<b>Parameters:</b>

 * <i>exponentMinSmall</i>: A 32-bit signed integer.

 * <i>exponentMaxSmall</i>: A 32-bit signed integer. (2).


<b>Returns:</b>

A PrecisionContext object.


### WithBigExponentRange

    public PeterO.PrecisionContext WithBigExponentRange(
        PeterO.BigInteger exponentMin,
        PeterO.BigInteger exponentMax);


Copies this precision context and sets the copy's exponent range.


<b>Parameters:</b>

 * <i>exponentMin</i>: Desired minimum exponent (EMin).

 * <i>exponentMax</i>: Desired maximum exponent.


<b>Returns:</b>

A PrecisionContext object.


### WithNoFlags

    public PeterO.PrecisionContext WithNoFlags();


Copies this PrecisionContext with HasFlags set to false and a Flags value of 0.


<b>Returns:</b>

A PrecisionContext object.


### WithSimplified

    public PeterO.PrecisionContext WithSimplified(
        bool simplified);


Not documented yet.


<b>Parameters:</b>

 * <i>simplified</i>: A Boolean object.


<b>Returns:</b>

A PrecisionContext object.


### WithUnlimitedExponents

    public PeterO.PrecisionContext WithUnlimitedExponents();


Copies this PrecisionContext with an unlimited exponent range.


<b>Returns:</b>

A PrecisionContext object.


### WithPrecision

    public PeterO.PrecisionContext WithPrecision(
        int precision);


Copies this PrecisionContext and gives it a particular precision value.


<b>Parameters:</b>

 * <i>precision</i>: Desired precision. 0 means unlimited precision.


<b>Returns:</b>

A PrecisionContext object.


### WithBigPrecision

    public PeterO.PrecisionContext WithBigPrecision(
        PeterO.BigInteger bigintPrecision);


Copies this PrecisionContext and gives it a particular precision value.


<b>Parameters:</b>

 * <i>bigintPrecision</i>: A BigInteger object.


<b>Returns:</b>

A PrecisionContext object.


<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bigintPrecision</i>
 is null.


### Copy

    public PeterO.PrecisionContext Copy();


Initializes a new PrecisionContext that is a copy of another PrecisionContext.


<b>Returns:</b>

A PrecisionContext object.


### ForPrecision

    public static PeterO.PrecisionContext ForPrecision(
        int precision);


Creates a new precision context using the given maximum number of digits and an unlimited exponent range.


<b>Parameters:</b>

 * <i>precision</i>: Maximum number of digits (precision).


<b>Returns:</b>

A PrecisionContext object.


### ForRounding

    public static PeterO.PrecisionContext ForRounding(
        PeterO.Rounding rounding);


Not documented yet.


<b>Parameters:</b>

 * <i>rounding</i>: A Rounding object.


<b>Returns:</b>

A PrecisionContext object.


### ForPrecisionAndRounding

    public static PeterO.PrecisionContext ForPrecisionAndRounding(
        int precision,
        PeterO.Rounding rounding);


Not documented yet.


<b>Parameters:</b>

 * <i>precision</i>: A 32-bit signed integer.

 * <i>rounding</i>: A Rounding object.


<b>Returns:</b>

A PrecisionContext object.


### PrecisionContext Constructor

    public PrecisionContext(
        int precision,
        PeterO.Rounding rounding,
        int exponentMinSmall,
        int exponentMaxSmall,
        bool clampNormalExponents);


Initializes a new instance of the PrecisionContext class. HasFlags will be set to false.


<b>Parameters:</b>

 * <i>precision</i>: A 32-bit signed integer.

 * <i>rounding</i>: A Rounding object.

 * <i>exponentMinSmall</i>: A 32-bit signed integer. (2).

 * <i>exponentMaxSmall</i>: A 32-bit signed integer. (3).

 * <i>clampNormalExponents</i>: A Boolean object.


### Unlimited

    public static readonly PeterO.PrecisionContext Unlimited;


No specific limit on precision. Rounding mode HalfUp.


### Binary16

    public static readonly PeterO.PrecisionContext Binary16;


Precision context for the IEEE-754-2008 binary16 format, 11 bits precision.


### Binary32

    public static readonly PeterO.PrecisionContext Binary32;


Precision context for the IEEE-754-2008 binary32 format, 24 bits precision.


### Binary64

    public static readonly PeterO.PrecisionContext Binary64;


Precision context for the IEEE-754-2008 binary64 format, 53 bits precision.


### Binary128

    public static readonly PeterO.PrecisionContext Binary128;


Precision context for the IEEE-754-2008 binary128 format, 113 bits precision.


### Decimal32

    public static readonly PeterO.PrecisionContext Decimal32;


Precision context for the IEEE-754-2008 decimal32 format.


### Decimal64

    public static readonly PeterO.PrecisionContext Decimal64;


Precision context for the IEEE-754-2008 decimal64 format.


### Decimal128

    public static readonly PeterO.PrecisionContext Decimal128;


Precision context for the IEEE-754-2008 decimal128 format.


### Basic

    public static readonly PeterO.PrecisionContext Basic;


Basic precision context, 9 digits precision, rounding mode half-up, unlimited exponent range.


### CliDecimal

    public static readonly PeterO.PrecisionContext CliDecimal;


Precision context for the Common Language Infrastructure (.NET Framework) decimal format, 96 bits precision. Use RoundToBinaryPrecision to round a decimal number to this format.


### EMax

    public PeterO.BigInteger EMax { get; }


Gets the highest exponent possible when a converted number is expressed in scientific notation with one digit before the decimal point. For example, with a precision of 3 and an EMax of 100, the maximum value possible is 9.99E + 100. (This is not the same as the highest possible Exponent property.) If HasExponentRange is false, this value will be 0.


<b>Returns:</b>

The highest exponent possible when a converted number is expressed in scientific notation with one digit before the decimal point. For example, with a precision of 3 and an EMax of 100, the maximum value possible is 9.99E + 100. (This is not the same as the highest possible Exponent property.) If HasExponentRange is false, this value will be 0.


### Traps

    public int Traps { get; }


Gets the traps that are set for each flag in the context. Whenever a flag is signaled, even if HasFlags is false, and the flag's trap is enabled, the operation will throw a TrapException. For example, if Traps equals FlagInexact and FlagSubnormal, a TrapException will be thrown if an operation's return value is not the same as the exact result (FlagInexact) or if the return value's exponent is lower than the lowest allowed (FlagSubnormal).





<b>Returns:</b>

The traps that are set for each flag in the context.


### HasExponentRange

    public bool HasExponentRange { get; }


Gets a value indicating whether this context defines a minimum and maximum exponent. If false, converted exponents can have any exponent and operations can't cause overflow or underflow.


<b>Returns:</b>

True if this context defines a minimum and maximum exponent; otherwise, false.


### HasMaxPrecision

    public bool HasMaxPrecision { get; }


Gets a value indicating whether this context defines a maximum precision.


<b>Returns:</b>

True if this context defines a maximum precision; otherwise, false.


### EMin

    public PeterO.BigInteger EMin { get; }


Gets the lowest exponent possible when a converted number is expressed in scientific notation with one digit before the decimal point. For example, with a precision of 3 and an EMin of -100, the next value that comes after 0 is 0.001E-100. (This is not the same as the lowest possible Exponent property.) If HasExponentRange is false, this value will be 0.


<b>Returns:</b>

The lowest exponent possible when a converted number is expressed in scientific notation with one digit before the decimal point.


### Precision

    public PeterO.BigInteger Precision { get; }


Gets the maximum length of a converted number in digits, ignoring the decimal point and exponent. For example, if precision is 3, a converted number's mantissa can range from 0 to 999 (up to three digits long). If 0, converted numbers can have any precision.


<b>Returns:</b>

The maximum length of a converted number in digits, ignoring the decimal point and exponent.


### ClampNormalExponents

    public bool ClampNormalExponents { get; }


Gets a value indicating whether a converted number's Exponent property will not be higher than EMax + 1 - Precision. If a number's exponent is higher than that value, but not high enough to cause overflow, the exponent is clamped to that value and enough zeros are added to the number's mantissa to account for the adjustment. If HasExponentRange is false, this value is always false.


<b>Returns:</b>

If true, a converted number's Exponent property will not be higher than EMax + 1 - Precision.


### Rounding

    public PeterO.Rounding Rounding { get; }


Gets the desired rounding mode when converting numbers that can't be represented in the given precision and exponent range.


<b>Returns:</b>

The desired rounding mode when converting numbers that can't be represented in the given precision and exponent range.


### HasFlags

    public bool HasFlags { get; }


Gets a value indicating whether this context has a mutable Flags field.


<b>Returns:</b>

True if this context has a mutable Flags field; otherwise, false.


### Flags

    public int Flags { get; set;}


Gets or sets the flags that are set from converting numbers according to this precision context. If HasFlags is false, this value will be 0. This value is a combination of bit fields. To retrieve a particular flag, use the AND operation on the return value of this method. For example:  `(this.Flags & PrecisionContext.FlagInexact)
            != 0` returns TRUE if the Inexact flag is set.


<b>Returns:</b>

The flags that are set from converting numbers according to this precision context. If HasFlags is false, this value will be 0.


### IsSimplified

    public bool IsSimplified { get; }


Gets a value indicating whether to use a "simplified" arithmetic.


<b>Returns:</b>

True if to use a "simplified" arithmetic; otherwise, false.



## PeterO.Rounding


    public sealed struct Rounding :
        System.Enum,
        System.IComparable,
        System.IFormattable,
        System.IConvertible


Specifies the mode to use when "shortening" numbers that otherwise can't fit a given number of digits, so that the shortened number has about the same value. This "shortening" is known as rounding.


### Up

    public static PeterO.Rounding Up = 0;


If there is a fractional part, the number is rounded to the closest representable number away from zero.


### Down

    public static PeterO.Rounding Down = 1;


The fractional part is discarded (the number is truncated).


### Ceiling

    public static PeterO.Rounding Ceiling = 2;


If there is a fractional part, the number is rounded to the highest representable number that's closest to it.


### Floor

    public static PeterO.Rounding Floor = 3;


If there is a fractional part, the number is rounded to the lowest representable number that's closest to it.


### HalfUp

    public static PeterO.Rounding HalfUp = 4;


Rounded to the nearest number; if the fractional part is exactly half, the number is rounded to the closest representable number away from zero. This is the most familiar rounding mode for many people.


### HalfDown

    public static PeterO.Rounding HalfDown = 5;


Rounded to the nearest number; if the fractional part is exactly half, it is discarded.


### HalfEven

    public static PeterO.Rounding HalfEven = 6;


Rounded to the nearest number; if the fractional part is exactly half, the number is rounded to the closest representable number that is even. This is sometimes also known as "banker's rounding".


### Unnecessary

    public static PeterO.Rounding Unnecessary = 7;


Indicates that rounding will not be used. If rounding is required, the rounding operation will report an error.


### ZeroFiveUp

    public static PeterO.Rounding ZeroFiveUp = 8;


If there is a fractional part and if the last digit before rounding is 0 or half the radix, the number is rounded to the closest representable number away from zero; otherwise the fractional part is discarded. In overflow, the fractional part is always discarded. This rounding mode is useful for rounding intermediate results at a slightly higher precision than the final precision.



## PeterO.TrapException


    public class TrapException :
        System.ArithmeticException,
        System.Runtime.Serialization.ISerializable,
        System.Runtime.InteropServices._Exception


Exception thrown for arithmetic trap errors.


### TrapException Constructor

    public TrapException(
        int flag,
        PeterO.PrecisionContext ctx,
        object result);


Initializes a new instance of the TrapException class.


<b>Parameters:</b>

 * <i>flag</i>: A 32-bit signed integer.

 * <i>ctx</i>: A PrecisionContext object.

 * <i>result</i>: An arbitrary object.


### Context

    public PeterO.PrecisionContext Context { get; }


Gets the precision context used during the operation that triggered the trap. May be null.


<b>Returns:</b>

The precision context used during the operation that triggered the trap. May be null.


### Result

    public object Result { get; }


Gets the defined result of the operation that caused the trap.


<b>Returns:</b>

The defined result of the operation that caused the trap.


### Error

    public int Error { get; }


Gets the flag that specifies the kind of error (PrecisionContext.FlagXXX). This will only be one flag, such as FlagInexact or FlagSubnormal.


<b>Returns:</b>

The flag that specifies the kind of error (PrecisionContext.FlagXXX). This will only be one flag, such as FlagInexact or FlagSubnormal.



