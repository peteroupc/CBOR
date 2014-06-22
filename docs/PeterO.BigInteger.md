## PeterO.BigInteger

    public sealed class BigInteger :
        System.IComparable,
        System.IEquatable

An arbitrary-precision integer. Instances of this class are immutable, so they are inherently safe for use by multiple threads. Multiple instances of this object with the same value are interchangeable, so they should not be compared using the "==" operator (which only checks if each side of the operator is the same instance).

An arbitrary-precision integer.

### fromByteArray

    public static PeterO.BigInteger fromByteArray(
        byte[] bytes,
        bool littleEndian);

Initializes a BigInteger object from an array of bytes.

<b>Parameters:</b>

 * <i>bytes</i>: A byte array. Can be empty, in which case the return value is 0.

 * <i>littleEndian</i>: A Boolean object.

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bytes</i>
 is null.

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

### intValueChecked

    public int intValueChecked();

Converts this object's value to a 32-bit signed integer.

<b>Returns:</b>

A 32-bit signed integer.

<b>Exceptions:</b>

 * System.OverflowException: 
This object's value is too big to fit a 32-bit signed integer.

### intValueUnchecked

    public int intValueUnchecked();

Converts this object's value to a 32-bit signed integer. If the value can't fit in a 32-bit integer, returns the lower 32 bits of this object's two's complement representation (in which case the return value might have a different sign than this object's value).

<b>Returns:</b>

A 32-bit signed integer.

### longValueChecked

    public long longValueChecked();

Converts this object's value to a 64-bit signed integer.

<b>Returns:</b>

A 64-bit signed integer.

<b>Exceptions:</b>

 * System.OverflowException: 
This object's value is too big to fit a 64-bit signed integer.

### longValueUnchecked

    public long longValueUnchecked();

Converts this object's value to a 64-bit signed integer. If the value can't fit in a 64-bit integer, returns the lower 64 bits of this object's two's complement representation (in which case the return value might have a different sign than this object's value).

<b>Returns:</b>

A 64-bit signed integer.

### intValue

    public int intValue();

Converts this object's value to a 32-bit signed integer. This method is obsoleted by the  `intValueChecked` and  `intValueUnchecked` methods, which should be used instead.

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

Converts this object's value to a 64-bit signed integer. This method is obsoleted by the longValueChecked and longValueUnchecked methods, which should be used instead.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>power</i>
 is null.

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

Finds the minimum number of bits needed to represent this object's value, except for its sign. If the value is negative, finds the number of bits in a value equal to this object's absolute value minus 1.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bigintSecond</i>
 is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>pow</i>
 is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bigintAugend</i>
 is null.

### subtract

    public PeterO.BigInteger subtract(
        PeterO.BigInteger subtrahend);

Subtracts a BigInteger from this BigInteger.

<b>Parameters:</b>

 * <i>subtrahend</i>: A BigInteger object.

<b>Returns:</b>

The difference of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>subtrahend</i>
 is null.

### multiply

    public PeterO.BigInteger multiply(
        PeterO.BigInteger bigintMult);

Multiplies this instance by the value of a BigInteger object.

<b>Parameters:</b>

 * <i>bigintMult</i>: A BigInteger object.

<b>Returns:</b>

The product of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bigintMult</i>
 is null.

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

 * System.ArgumentNullException: 
The parameter <i>bigintDivisor</i>
 is null.

 * System.DivideByZeroException: 
Attempted to divide by zero.

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

 * System.DivideByZeroException: 
Attempted to divide by zero.

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

 * System.ArgumentNullException: 
The parameter <i>divisor</i>
 is null.

### remainder

    public PeterO.BigInteger remainder(
        PeterO.BigInteger divisor);

Finds the remainder that results when this instance is divided by the value of a BigInteger object. The remainder is the value that remains when the absolute value of this object is divided by the absolute value of the other object; the remainder has the same sign (positive or negative) as this object.

<b>Parameters:</b>

 * <i>divisor</i>: A BigInteger object.

<b>Returns:</b>

The remainder of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>divisor</i>
 is null.

 * System.DivideByZeroException: 
Attempted to divide by zero.

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

An array of two big integers: the first integer is the square root, and the second is the difference between this value and the square of the first integer. Returns two zeros if this value is 0 or less, or one and zero if this value equals 1.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bthis</i>
 is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bthis</i>
 is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>operand1</i>
 is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>dividend</i>
 is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>dividend</i>
 is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bthis</i>
 is null.

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

The value (  <i>bigintValue</i>
 ^  <i>pow</i>
 )%  <i>mod</i>
 .

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bigintValue</i>
 is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bthis</i>
 is null.

### Operator `-`

    public static PeterO.BigInteger operator -(
        PeterO.BigInteger bigValue);

Negates a BigInteger object.

<b>Parameters:</b>

 * <i>bigValue</i>: A BigInteger object. (2).

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bigValue</i>
 is null.

### Operator `<`

    public static bool operator <(
        PeterO.BigInteger thisValue,
        PeterO.BigInteger otherValue);

Determines whether a BigInteger instance is less than another BigInteger instance.

<b>Parameters:</b>

 * <i>thisValue</i>: The first BigInteger object.

 * <i>otherValue</i>: The second BigInteger object.

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

 * <i>thisValue</i>: The first BigInteger object.

 * <i>otherValue</i>: The second BigInteger object.

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

 * <i>thisValue</i>: The first BigInteger object.

 * <i>otherValue</i>: The second BigInteger object.

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

 * <i>thisValue</i>: The first BigInteger object.

 * <i>otherValue</i>: The second BigInteger object.

<b>Returns:</b>

True if  <i>thisValue</i>
 is at least  <i>otherValue</i>
 ; otherwise, false.

### Abs

    public static PeterO.BigInteger Abs(
        PeterO.BigInteger thisValue);

Not documented yet.

<b>Parameters:</b>

 * <i>thisValue</i>: A BigInteger object. (2).

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>thisValue</i>
 is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bigintFirst</i>
 is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bigValue</i>
 or  <i>power</i>
 is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bigValue</i>
 is null.

### BigInteger Constructor

    public BigInteger(
        byte[] bytes);

<b>Deprecated.</b> Use BigInteger.fromByteArray(bytes,true) instead. This method may be removed in version 2.0.

Initializes a new instance of the BigInteger class.

<b>Parameters:</b>

 * <i>bytes</i>: A byte array.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bytes</i>
 is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>valueA</i>
 is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>a</i>
 or  <i>b</i>
 is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>first</i>
 or  <i>second</i>
 is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>a</i>
 or  <i>b</i>
 is null.

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


