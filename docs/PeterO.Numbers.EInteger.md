## PeterO.Numbers.EInteger

    public sealed class EInteger :
        System.IComparable,
        System.IEquatable

An arbitrary-precision integer.Instances of this class are immutable, so they are inherently safe for use by multiple threads. Multiple instances of this object with the same value are interchangeable, so they should not be compared using the "==" operator (which only checks if each side of the operator is the same instance).

An arbitrary-precision integer.

### ONE

    public static readonly PeterO.Numbers.EInteger ONE;

EInteger object for the number one.

### TEN

    public static readonly PeterO.Numbers.EInteger TEN;

EInteger object for the number ten.

### ZERO

    public static readonly PeterO.Numbers.EInteger ZERO;

EInteger object for the number zero.

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

### IsZero

    public bool IsZero { get; }

Gets a value indicating whether this value is 0.

<b>Returns:</b>

True if this value is 0; otherwise, false.

### One

    public static PeterO.Numbers.EInteger One { get; }

Gets the BigInteger object for one.

<b>Returns:</b>

The BigInteger object for one.

### Sign

    public int Sign { get; }

Gets the sign of this object's value.

<b>Returns:</b>

0 if this value is zero; -1 if this value is negative, or 1 if this value is positive.

### Zero

    public static PeterO.Numbers.EInteger Zero { get; }

Gets the BigInteger object for zero.

<b>Returns:</b>

The BigInteger object for zero.

### abs

    public PeterO.Numbers.EInteger abs();

Returns the absolute value of this object's value.

<b>Returns:</b>

This object's value with the sign removed.

### Abs

    public static PeterO.Numbers.EInteger Abs(
        PeterO.Numbers.EInteger thisValue);

Not documented yet.

<b>Parameters:</b>

 * <i>thisValue</i>: Another BigInteger object.

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>thisValue</i>
 is null.

### add

    public PeterO.Numbers.EInteger add(
        PeterO.Numbers.EInteger bigintAugend);

Adds this object and another object.

<b>Parameters:</b>

 * <i>bigintAugend</i>: Another EInteger object.

<b>Returns:</b>

The sum of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bigintAugend</i>
 is null.

### And

    public static PeterO.Numbers.EInteger And(
        PeterO.Numbers.EInteger a,
        PeterO.Numbers.EInteger b);

Does an AND operation between two BigInteger values.

Each BigInteger instance is treated as a two's complement representation for the purposes of this operator.

<b>Parameters:</b>

 * <i>a</i>: A BigInteger instance.

 * <i>b</i>: Another BigInteger instance.

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>a</i>
 or  <i>b</i>
 is null.

### bitLength

    public int bitLength();

Finds the minimum number of bits needed to represent this object's value, except for its sign. If the value is negative, finds the number of bits in a value equal to this object's absolute value minus 1.

<b>Returns:</b>

The number of bits in this object's value. Returns 0 if this object's value is 0 or negative 1.

### canFitInInt

    public bool canFitInInt();

Returns whether this object's value can fit in a 32-bit signed integer.

<b>Returns:</b>

True if this object's value is MinValue or greater, and MaxValue or less; otherwise, false.

### CompareTo

    public sealed int CompareTo(
        PeterO.Numbers.EInteger other);

Compares a EInteger object with this instance.

<b>Parameters:</b>

 * <i>other</i>: An EInteger object.

<b>Returns:</b>

Zero if the values are equal; a negative number if this instance is less, or a positive number if this instance is greater.

### divide

    public PeterO.Numbers.EInteger divide(
        PeterO.Numbers.EInteger bigintDivisor);

Divides this instance by the value of a EInteger object. The result is rounded down (the fractional part is discarded). Except if the result is 0, it will be negative if this object is positive and the other is negative, or vice versa, and will be positive if both are positive or both are negative.

<b>Parameters:</b>

 * <i>bigintDivisor</i>: Another EInteger object.

<b>Returns:</b>

The quotient of the two objects.

<b>Exceptions:</b>

 * System.DivideByZeroException:
The divisor is zero.

 * System.ArgumentNullException:
The parameter  <i>bigintDivisor</i>
 is null.

 * System.DivideByZeroException:
Attempted to divide by zero.

### divideAndRemainder

    public PeterO.Numbers.EInteger[] divideAndRemainder(
        PeterO.Numbers.EInteger divisor);

Divides this object by another big integer and returns the quotient and remainder.

<b>Parameters:</b>

 * <i>divisor</i>: An EInteger object.

<b>Returns:</b>

An array with two big integers: the first is the quotient, and the second is the remainder.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter divisor is null.

 * System.DivideByZeroException:
The parameter divisor is 0.

 * System.DivideByZeroException:
Attempted to divide by zero.

### Equals

    public override bool Equals(
        object obj);

Determines whether this object and another object are equal.

<b>Parameters:</b>

 * <i>obj</i>: An arbitrary object.

<b>Returns:</b>

True if this object and another object are equal; otherwise, false.

### Equals

    public sealed bool Equals(
        PeterO.Numbers.EInteger other);

Not documented yet.

<b>Parameters:</b>

 * <i>other</i>: A BigInteger object.

<b>Returns:</b>

A Boolean object.

### fromBytes

    public static PeterO.Numbers.EInteger fromBytes(
        byte[] bytes,
        bool littleEndian);

Initializes a EInteger object from an array of bytes.

<b>Parameters:</b>

 * <i>bytes</i>: A byte array.

 * <i>littleEndian</i>: A Boolean object.

<b>Returns:</b>

An EInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 is null.

### fromRadixString

    public static PeterO.Numbers.EInteger fromRadixString(
        string str,
        int radix);

Converts a string to an arbitrary-precision integer. The string portion can begin with a minus sign ("-" , U+002D) to indicate that it's negative.

The following example (C#) converts a number in the orm of a hex string to a big integer.    public static EInteger HexToEInteger(string hexString) {
      // Parse the hexadecimal string as a big integer.  Will
      // throw a FormatException if the parsing fails
      var EInteger = EInteger.fromRadixString(hexString, 16);
      // Optional: Check if the parsed integer is negative
      if (EInteger.Sign < 0) {
        throw new FormatException("negative hex string");
      }
      return EInteger;
    }

<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>radix</i>: A base from 2 to 36. Depending on the radix, the string can use the basic digits 0 to 9 (U + 0030 to U + 0039) and then the basic letters A to Z (U + 0041 to U + 005A). For example, 0-9 in radix 10, and 0-9, then A-F in radix 16.

<b>Returns:</b>

A EInteger object with the same value as given in the string.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

 * System.ArgumentException:
The parameter  <i>radix</i>
 is less than 2 or greater than 36.

 * System.FormatException:
The string is empty or in an invalid format.

### fromRadixSubstring

    public static PeterO.Numbers.EInteger fromRadixSubstring(
        string str,
        int radix,
        int index,
        int endIndex);

Converts a portion of a string to an arbitrary-precision integer in a given radix. The string portion can begin with a minus sign ("-" , U+002D) to indicate that it's negative.

<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>radix</i>: A base from 2 to 36. Depending on the radix, the string can use the basic digits 0 to 9 (U + 0030 to U + 0039) and then the basic letters A to Z (U + 0041 to U + 005A). For example, 0-9 in radix 10, and 0-9, then A-F in radix 16.

 * <i>index</i>: The index of the string that starts the string portion.

 * <i>endIndex</i>: The index of the string that ends the string portion. The length will be index + endIndex - 1.

<b>Returns:</b>

A EInteger object with the same value as given in the string portion.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

 * System.ArgumentException:
The parameter  <i>index</i>
 is less than 0,  <i>endIndex</i>
 is less than 0, or either is greater than the string's length, or  <i>endIndex</i>
 is less than  <i>index</i>
.

 * System.FormatException:
The string portion is empty or in an invalid format.

### fromString

    public static PeterO.Numbers.EInteger fromString(
        string str);

Converts a string to an arbitrary-precision integer.

<b>Parameters:</b>

 * <i>str</i>: A string containing only basic digits 0 to 9 (U + 0030 to U + 0039), except that it may start with a minus sign ("-", U + 002D).

<b>Returns:</b>

A EInteger object with the same value as given in the string.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

 * System.FormatException:
The parameter  <i>str</i>
 is in an invalid format.

### fromSubstring

    public static PeterO.Numbers.EInteger fromSubstring(
        string str,
        int index,
        int endIndex);

Converts a portion of a string to an arbitrary-precision integer. The string portion can begin with a minus sign ("-", U+002D) to indicate that it's negative.

<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>index</i>: The index of the string that starts the string portion.

 * <i>endIndex</i>: The index of the string that ends the string portion. The length will be index + endIndex - 1.

<b>Returns:</b>

A EInteger object with the same value as given in the string portion.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

 * System.ArgumentException:
The parameter  <i>index</i>
 is less than 0,  <i>endIndex</i>
 is less than 0, or either is greater than the string's length, or  <i>endIndex</i>
 is less than  <i>index</i>
.

 * System.FormatException:
The string portion is empty or in an invalid format.

### gcd

    public PeterO.Numbers.EInteger gcd(
        PeterO.Numbers.EInteger bigintSecond);

Returns the greatest common divisor of two integers. The greatest common divisor (GCD) is also known as the greatest common factor (GCF).

<b>Parameters:</b>

 * <i>bigintSecond</i>: Another EInteger object.

<b>Returns:</b>

An EInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bigintSecond</i>
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

### getDigitCount

    public int getDigitCount();

Finds the number of decimal digits this number has.

<b>Returns:</b>

The number of decimal digits. Returns 1 if this object' s value is 0.

### GetHashCode

    public override int GetHashCode();

Returns the hash code for this instance.

<b>Returns:</b>

A 32-bit signed integer.

### getLowBit

    public int getLowBit();

Gets the lowest set bit in this number's absolute value.

<b>Returns:</b>

The lowest bit set in the number, starting at 0. Returns 0 if this value is 0 or odd. (NOTE: In future versions, may return -1 instead if this value is 0.).

### getUnsignedBitLength

    public int getUnsignedBitLength();

Finds the minimum number of bits needed to represent this object's absolute value.

<b>Returns:</b>

The number of bits in this object's value. Returns 0 if this object's value is 0, and returns 1 if the value is negative 1.

### GreatestCommonDivisor

    public static PeterO.Numbers.EInteger GreatestCommonDivisor(
        PeterO.Numbers.EInteger bigintFirst,
        PeterO.Numbers.EInteger bigintSecond);

Not documented yet.

<b>Parameters:</b>

 * <i>bigintFirst</i>: Another BigInteger object.

 * <i>bigintSecond</i>: A BigInteger object. (3).

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bigintFirst</i>
 is null.

### intValueChecked

    public int intValueChecked();

Converts this object's value to a 32-bit signed integer, throwing an exception if it can't fit.

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

Converts this object's value to a 64-bit signed integer, throwing an exception if it can't fit.

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

### mod

    public PeterO.Numbers.EInteger mod(
        PeterO.Numbers.EInteger divisor);

Finds the modulus remainder that results when this instance is divided by the value of a EInteger object. The modulus remainder is the same as the normal remainder if the normal remainder is positive, and equals divisor plus normal remainder if the normal remainder is negative.

<b>Parameters:</b>

 * <i>divisor</i>: A divisor greater than 0 (the modulus).

<b>Returns:</b>

An EInteger object.

<b>Exceptions:</b>

 * System.ArithmeticException:
The parameter  <i>divisor</i>
 is negative.

 * System.ArgumentNullException:
The parameter  <i>divisor</i>
 is null.

### ModPow

    public PeterO.Numbers.EInteger ModPow(
        PeterO.Numbers.EInteger pow,
        PeterO.Numbers.EInteger mod);

Calculates the remainder when a EInteger raised to a certain power is divided by another EInteger.

<b>Parameters:</b>

 * <i>pow</i>: Another EInteger object.

 * <i>mod</i>: A EInteger object. (3).

<b>Returns:</b>

An EInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>pow</i>
 or  <i>mod</i>
 is null.

### ModPow

    public static PeterO.Numbers.EInteger ModPow(
        PeterO.Numbers.EInteger bigintValue,
        PeterO.Numbers.EInteger pow,
        PeterO.Numbers.EInteger mod);

Calculates the remainder when a BigInteger raised to a certain power is divided by another BigInteger.

<b>Parameters:</b>

 * <i>bigintValue</i>: A BigInteger object.

 * <i>pow</i>: Another BigInteger object.

 * <i>mod</i>: A BigInteger object. (3).

<b>Returns:</b>

The value (  <i>bigintValue</i>
 ^  <i>pow</i>
 )%  <i>mod</i>
.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bigintValue</i>
 is null.

### multiply

    public PeterO.Numbers.EInteger multiply(
        PeterO.Numbers.EInteger bigintMult);

Multiplies this instance by the value of a EInteger object.

<b>Parameters:</b>

 * <i>bigintMult</i>: Another EInteger object.

<b>Returns:</b>

The product of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bigintMult</i>
 is null.

### negate

    public PeterO.Numbers.EInteger negate();

Gets the value of this object with the sign reversed.

<b>Returns:</b>

This object's value with the sign reversed.

### Not

    public static PeterO.Numbers.EInteger Not(
        PeterO.Numbers.EInteger valueA);

Returns a BigInteger with every bit flipped.

<b>Parameters:</b>

 * <i>valueA</i>: Another BigInteger object.

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>valueA</i>
 is null.

### Operator `+`

    public static PeterO.Numbers.EInteger operator +(
        PeterO.Numbers.EInteger bthis,
        PeterO.Numbers.EInteger augend);

Adds a BigInteger object and a BigInteger object.

<b>Parameters:</b>

 * <i>bthis</i>: A BigInteger object.

 * <i>augend</i>: Another BigInteger object.

<b>Returns:</b>

The sum of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bthis</i>
 is null.

### Operator `/`

    public static PeterO.Numbers.EInteger operator /(
        PeterO.Numbers.EInteger dividend,
        PeterO.Numbers.EInteger divisor);

Divides a BigInteger object by the value of a BigInteger object.

<b>Parameters:</b>

 * <i>dividend</i>: A BigInteger object.

 * <i>divisor</i>: Another BigInteger object.

<b>Returns:</b>

The quotient of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>dividend</i>
 is null.

### Operator `>`

    public static bool operator >(
        PeterO.Numbers.EInteger thisValue,
        PeterO.Numbers.EInteger otherValue);

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
        PeterO.Numbers.EInteger thisValue,
        PeterO.Numbers.EInteger otherValue);

Determines whether a BigInteger value is greater than another BigInteger value.

<b>Parameters:</b>

 * <i>thisValue</i>: The first BigInteger object.

 * <i>otherValue</i>: The second BigInteger object.

<b>Returns:</b>

True if  <i>thisValue</i>
 is at least  <i>otherValue</i>
 ; otherwise, false.

### Operator `<<`

    public static PeterO.Numbers.EInteger operator <<(
        PeterO.Numbers.EInteger bthis,
        int bitCount);

Not documented yet.

<b>Parameters:</b>

 * <i>bthis</i>: Another BigInteger object.

 * <i>bitCount</i>: A 32-bit signed integer.

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bthis</i>
 is null.

### Operator `<`

    public static bool operator <(
        PeterO.Numbers.EInteger thisValue,
        PeterO.Numbers.EInteger otherValue);

Determines whether a BigInteger instance is less than another BigInteger instance.

<b>Parameters:</b>

 * <i>thisValue</i>: The first BigInteger object.

 * <i>otherValue</i>: The second BigInteger object.

<b>Returns:</b>

True if  <i>thisValue</i>
 is less than <i>otherValue</i>
 ; otherwise, false.

### Operator `<=`

    public static bool operator <=(
        PeterO.Numbers.EInteger thisValue,
        PeterO.Numbers.EInteger otherValue);

Determines whether a BigInteger instance is less than or equal to another BigInteger instance.

<b>Parameters:</b>

 * <i>thisValue</i>: The first BigInteger object.

 * <i>otherValue</i>: The second BigInteger object.

<b>Returns:</b>

True if  <i>thisValue</i>
 is up to  <i>otherValue</i>
 ; otherwise, false.

### Operator `%`

    public static PeterO.Numbers.EInteger operator %(
        PeterO.Numbers.EInteger dividend,
        PeterO.Numbers.EInteger divisor);

Finds the remainder that results when a BigInteger object is divided by the value of a BigInteger object.

<b>Parameters:</b>

 * <i>dividend</i>: A BigInteger object.

 * <i>divisor</i>: Another BigInteger object.

<b>Returns:</b>

The remainder of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>dividend</i>
 is null.

### Operator `*`

    public static PeterO.Numbers.EInteger operator *(
        PeterO.Numbers.EInteger operand1,
        PeterO.Numbers.EInteger operand2);

Multiplies a BigInteger object by the value of a BigInteger object.

<b>Parameters:</b>

 * <i>operand1</i>: A BigInteger object.

 * <i>operand2</i>: Another BigInteger object.

<b>Returns:</b>

The product of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>operand1</i>
 is null.

### Operator `>>`

    public static PeterO.Numbers.EInteger operator >>(
        PeterO.Numbers.EInteger bthis,
        int bigValue);

Shifts the bits of a BigInteger instance to the right.

For this operation, the BigInteger is treated as a two's complement representation. Thus, for negative values, the BigInteger is sign-extended.

<b>Parameters:</b>

 * <i>bthis</i>: Another BigInteger object.

 * <i>bigValue</i>: A 32-bit signed integer.

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bthis</i>
 is null.

### Operator `-`

    public static PeterO.Numbers.EInteger operator -(
        PeterO.Numbers.EInteger bthis,
        PeterO.Numbers.EInteger subtrahend);

Subtracts two BigInteger values.

<b>Parameters:</b>

 * <i>bthis</i>: A BigInteger value.

 * <i>subtrahend</i>: A BigInteger object.

<b>Returns:</b>

The difference of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bthis</i>
 is null.

### Operator `-`

    public static PeterO.Numbers.EInteger operator -(
        PeterO.Numbers.EInteger bigValue);

Negates a BigInteger object.

<b>Parameters:</b>

 * <i>bigValue</i>: Another BigInteger object.

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bigValue</i>
 is null.

### Or

    public static PeterO.Numbers.EInteger Or(
        PeterO.Numbers.EInteger first,
        PeterO.Numbers.EInteger second);

Does an OR operation between two BigInteger instances.

Each BigInteger instance is treated as a two's complement representation for the purposes of this operator.

<b>Parameters:</b>

 * <i>first</i>: Another BigInteger object.

 * <i>second</i>: A BigInteger object. (3).

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>first</i>
 or  <i>second</i>
 is null.

### pow

    public PeterO.Numbers.EInteger pow(
        int powerSmall);

Raises a big integer to a power.

<b>Parameters:</b>

 * <i>powerSmall</i>: The exponent to raise to.

<b>Returns:</b>

The result. Returns 1 if  <i>powerSmall</i>
 is 0.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter  <i>powerSmall</i>
 is less than 0.

### Pow

    public static PeterO.Numbers.EInteger Pow(
        PeterO.Numbers.EInteger bigValue,
        int power);

Not documented yet.

<b>Parameters:</b>

 * <i>bigValue</i>: Another BigInteger object.

 * <i>power</i>: A 32-bit signed integer.

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bigValue</i>
 is null.

### Pow

    public static PeterO.Numbers.EInteger Pow(
        PeterO.Numbers.EInteger bigValue,
        PeterO.Numbers.EInteger power);

Not documented yet.

<b>Parameters:</b>

 * <i>bigValue</i>: Another BigInteger object.

 * <i>power</i>: A BigInteger object. (3).

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bigValue</i>
 or  <i>power</i>
 is null.

### PowBigIntVar

    public PeterO.Numbers.EInteger PowBigIntVar(
        PeterO.Numbers.EInteger power);

Raises a big integer to a power, which is given as another big integer.

<b>Parameters:</b>

 * <i>power</i>: The exponent to raise to.

<b>Returns:</b>

The result. Returns 1 if  <i>power</i>
 is 0.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>power</i>
 is null.

 * System.ArgumentException:
The parameter  <i>power</i>
 is less than 0.

### remainder

    public PeterO.Numbers.EInteger remainder(
        PeterO.Numbers.EInteger divisor);

Finds the remainder that results when this instance is divided by the value of a EInteger object. The remainder is the value that remains when the absolute value of this object is divided by the absolute value of the other object; the remainder has the same sign (positive or negative) as this object.

<b>Parameters:</b>

 * <i>divisor</i>: Another EInteger object.

<b>Returns:</b>

The remainder of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>divisor</i>
 is null.

 * System.DivideByZeroException:
Attempted to divide by zero.

### shiftLeft

    public PeterO.Numbers.EInteger shiftLeft(
        int numberBits);

Returns a big integer with the bits shifted to the left by a number of bits. A value of 1 doubles this value, a value of 2 multiplies it by 4, a value of 3 by 8, a value of 4 by 16, and so on.

<b>Parameters:</b>

 * <i>numberBits</i>: The number of bits to shift. Can be negative, in which case this is the same as shiftRight with the absolute value of numberBits.

<b>Returns:</b>

An EInteger object.

### shiftRight

    public PeterO.Numbers.EInteger shiftRight(
        int numberBits);

Returns a big integer with the bits shifted to the right. For this operation, the big integer is treated as a two's complement representation. Thus, for negative values, the EInteger is sign-extended.

<b>Parameters:</b>

 * <i>numberBits</i>: Number of bits to shift right.

<b>Returns:</b>

An EInteger object.

### sqrt

    public PeterO.Numbers.EInteger sqrt();

Finds the square root of this instance's value, rounded down.

<b>Returns:</b>

The square root of this object's value. Returns 0 if this value is 0 or less.

### sqrtWithRemainder

    public PeterO.Numbers.EInteger[] sqrtWithRemainder();

Calculates the square root and the remainder.

<b>Returns:</b>

An array of two big integers: the first integer is the square root, and the second is the difference between this value and the square of the first integer. Returns two zeros if this value is 0 or less, or one and zero if this value equals 1.

### subtract

    public PeterO.Numbers.EInteger subtract(
        PeterO.Numbers.EInteger subtrahend);

Subtracts a EInteger from this EInteger.

<b>Parameters:</b>

 * <i>subtrahend</i>: Another EInteger object.

<b>Returns:</b>

The difference of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>subtrahend</i>
 is null.

### testBit

    public bool testBit(
        int index);

Returns whether a bit is set in the two's-complement representation of this object's value.

<b>Parameters:</b>

 * <i>index</i>: Zero based index of the bit to test. 0 means the least significant bit.

<b>Returns:</b>

True if a bit is set in the two's-complement representation of this object's value; otherwise, false.

### toBytes

    public byte[] toBytes(
        bool littleEndian);

Returns a byte array of this object's value. The byte array will take the form of the number's two' s-complement representation, using the fewest bytes necessary to represent its value unambiguously. If this value is negative, the bits that appear "before" the most significant bit of the number will be all ones.

<b>Parameters:</b>

 * <i>littleEndian</i>: If true, the least significant bits will appear first.

<b>Returns:</b>

A byte array. If this value is 0, returns a byte array with the single element 0.

### toRadixString

    public string toRadixString(
        int radix);

Generates a string representing the value of this object, in the given radix.

<b>Parameters:</b>

 * <i>radix</i>: A radix from 2 through 36. For example, to generate a hexadecimal (base-16) string, specify 16. To generate a decimal (base-10) string, specify 10.

<b>Returns:</b>

A string representing the value of this object. If this value is 0, returns "0". If negative, the string will begin with a hyphen/minus ("-"). Depending on the radix, the string will use the basic digits 0 to 9 (U + 0030 to U + 0039) and then the basic letters A to Z (U + 0041 to U + 005A). For example, 0-9 in radix 10, and 0-9, then A-F in radix 16.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter "index" is less than 0, "endIndex" is less than 0, or either is greater than the string's length, or "endIndex" is less than "index" ; or radix is less than 2 or greater than 36.

### ToString

    public override string ToString();

Converts this object to a text string in base 10.

<b>Returns:</b>

A string representation of this object. If negative, the string will begin with a minus sign ("-", U+002D). The string will use the basic digits 0 to 9 (U + 0030 to U + 0039).

### valueOf

    public static PeterO.Numbers.EInteger valueOf(
        long longerValue);

Converts a 64-bit signed integer to a big integer.

<b>Parameters:</b>

 * <i>longerValue</i>: A 64-bit signed integer.

<b>Returns:</b>

A EInteger object with the same value as the 64-bit number.

### Xor

    public static PeterO.Numbers.EInteger Xor(
        PeterO.Numbers.EInteger a,
        PeterO.Numbers.EInteger b);

Finds the exclusive "or" of two BigInteger objects.

Each BigInteger instance is treated as a two's complement representation for the purposes of this operator.

<b>Parameters:</b>

 * <i>a</i>: A BigInteger instance.

 * <i>b</i>: Another BigInteger instance.

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>a</i>
 or  <i>b</i>
 is null.
