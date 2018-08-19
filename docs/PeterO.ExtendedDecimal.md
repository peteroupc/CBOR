## PeterO.ExtendedDecimal

    public sealed class ExtendedDecimal :
        System.IComparable,
        System.IEquatable

<b>Deprecated.</b> Use EDecimal from PeterO.Numbers/com.upokecenter.numbers and the output of this class's ToString method.

<b>This class is largely obsolete. It will be replaced by a new version of this class in a different namespace/package and library, called `PeterO.Numbers.EDecimal` in the<a href="https://www.nuget.org/packages/PeterO.Numbers"> `PeterO.Numbers` </a>library (in .NET), or `com.upokecenter.numbers.EDecimal` in the<a href="https://github.com/peteroupc/numbers-java"> `com.github.peteroupc/numbers` </a>artifact (in Java). This new class can be used in the `CBORObject.FromObject(object)` method (by including the new library in your code, among other hings).</b>

Represents an arbitrary-precision decimal floating-point number.<b>About decimal arithmetic</b>

Decimal (base-10) arithmetic, such as that provided by this class, is ppropriate for calculations involving such real-world data as prices nd other sums of money, tax rates, and measurements. These calculations ften involve multiplying or dividing one decimal with another decimal, r performing other operations on decimal numbers. Many of these alculations also rely on rounding behavior in which the result after ounding is a decimal number (for example, multiplying a price by a remium rate, then rounding, should result in a decimal amount of oney).

On the other hand, most implementations of `float` and `double` , including in C# and Java, store numbers in a binary (base-2) loating-point format and use binary floating-point arithmetic. Many ecimal numbers can't be represented exactly in binary floating-point ormat (regardless of its length). Applying binary arithmetic to numbers ntended to be decimals can sometimes lead to unintuitive results, as is hown in the description for the FromDouble() method of this class.

<b>About ExtendedDecimal instances</b>

Each instance of this class consists of an integer mantissa and an nteger exponent, both arbitrary-precision. The value of the number quals mantissa * 10^exponent.

The mantissa is the value of the digits that make up a number, ignoring he decimal point and exponent. For example, in the number 2356.78, the antissa is 235678. The exponent is where the "floating" decimal point f the number is located. A positive exponent means "move it to the ight", and a negative exponent means "move it to the left." In the xample 2, 356.78, the exponent is -2, since it has 2 decimal places and he decimal point is "moved to the left by 2." Therefore, in the rbitrary-precision decimal representation, this number would be stored s 235678 * 10^-2.

The mantissa and exponent format preserves trailing zeros in the umber's value. This may give rise to multiple ways to store the same alue. For example, 1.00 and 1 would be stored differently, even though hey have the same value. In the first case, 100 * 10^-2 (100 with ecimal point moved left by 2), and in the second case, 1 * 10^0 (1 with ecimal point moved 0).

This class also supports values for negative zero, not-a-number (NaN) alues, and infinity.<b>Negative zero</b>is generally used when a negative number is rounded to 0; it has the ame mathematical value as positive zero.<b>Infinity</b>is generally used when a non-zero number is divided by zero, or when a ery high number can't be represented in a given exponent range.<b>Not-a-number</b>is generally used to signal errors.

<b>Errors and Exceptions</b>

Passing a signaling NaN to any arithmetic operation shown here will ignal the flag FlagInvalid and return a quiet NaN, even if another perand to that operation is a quiet NaN, unless noted otherwise.

Passing a quiet NaN to any arithmetic operation shown here will return quiet NaN, unless noted otherwise. Invalid operations will also return quiet NaN, as stated in the individual methods.

Unless noted otherwise,passing a null arbitrary-precision decimal rgument to any method here will throw an exception.

When an arithmetic operation signals the flag FlagInvalid, lagOverflow, or FlagDivideByZero, it will not throw an exception too, nless the flag's trap is enabled in the precision context (see Context's Traps property).

If an operation requires creating an intermediate value that might be oo big to fit in memory (or might require more than 2 gigabytes of emory to store -- due to the current use of a 32-bit integer internally s a length), the operation may signal an invalid-operation flag and eturn not-a-number (NaN). In certain rare cases, the CompareTo method ay throw OutOfMemoryException (called OutOfMemoryError in Java) in the ame circumstances.

<b>Serialization</b>

An arbitrary-precision decimal value can be serialized (converted to a table format) in one of the following ways:

 * By calling the toString() method, which will always return distinct trings for distinct arbitrary-precision decimal values.

 * By calling the UnsignedMantissa, Exponent, and IsNegative properties, nd calling the IsInfinity, IsQuietNaN, and IsSignalingNaN methods. he return values combined will uniquely identify a particular rbitrary-precision decimal value.

<b>Thread safety</b>

Instances of this class are immutable, so they are inherently safe for se by multiple threads. Multiple instances of this object with the same roperties are interchangeable, so they should not be compared using the ==" operator (which might only check if each side of the operator is he same instance).

<b>Comparison considerations</b>

This class's natural ordering (under the CompareTo method) is not onsistent with the Equals method. This means that two values that ompare as equal under the CompareTo method might not be equal under the quals method. The CompareTo method compares the mathematical values of he two instances passed to it (and considers two different NaN values s equal), while two instances with the same mathematical value, but ifferent exponents, will be considered unequal under the Equals method.

### NaN

    public static readonly PeterO.ExtendedDecimal NaN;

A not-a-number value.

### NegativeInfinity

    public static readonly PeterO.ExtendedDecimal NegativeInfinity;

Negative infinity, less than any other number.

### NegativeZero

    public static readonly PeterO.ExtendedDecimal NegativeZero;

Represents the number negative zero.

### One

    public static readonly PeterO.ExtendedDecimal One;

Represents the number 1.

### PositiveInfinity

    public static readonly PeterO.ExtendedDecimal PositiveInfinity;

Positive infinity, greater than any other number.

### SignalingNaN

    public static readonly PeterO.ExtendedDecimal SignalingNaN;

A not-a-number value that signals an invalid operation flag when it's passed as an argument to any arithmetic operation in arbitrary-precision decimal.

### Ten

    public static readonly PeterO.ExtendedDecimal Ten;

Represents the number 10.

### Zero

    public static readonly PeterO.ExtendedDecimal Zero;

Represents the number 0.

### Exponent

    public PeterO.BigInteger Exponent { get; }

Gets this object's exponent. This object's value will be an integer if the exponent is positive or zero.

<b>Returns:</b>

This object's exponent. This object's value will be an integer if the xponent is positive or zero.

### IsNegative

    public bool IsNegative { get; }

<b>Deprecated.</b> Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.

Gets a value indicating whether this object is negative, including negative zero.

<b>Returns:</b>

 `true` If this object is negative, including negative zero; otherwise,  `false` .

### Mantissa

    public PeterO.BigInteger Mantissa { get; }

Gets this object's un-scaled value.

<b>Returns:</b>

This object's un-scaled value. Will be negative if this object's value is egative (including a negative NaN).

### Sign

    public int Sign { get; }

<b>Deprecated.</b> Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.

Gets this value's sign: -1 if negative; 1 if positive; 0 if zero.

<b>Returns:</b>

This value's sign: -1 if negative; 1 if positive; 0 if zero.

### UnsignedMantissa

    public PeterO.BigInteger UnsignedMantissa { get; }

Gets the absolute value of this object's un-scaled value.

<b>Returns:</b>

The absolute value of this object's un-scaled value.

### CompareTo

    public sealed int CompareTo(
        PeterO.ExtendedDecimal other);

Compares this extended decimal to another.

<b>Parameters:</b>

 * <i>other</i>: The parameter <i>other</i>
is an ExtendedDecimal object.

<b>Return Value:</b>

Less than 0 if this value is less than, 0 if equal to, or greater than 0 if greater than the other extended decimal.

### Create

    public static PeterO.ExtendedDecimal Create(
        PeterO.BigInteger mantissa,
        PeterO.BigInteger exponent);

Creates a number with the value exponent*10^mantissa.

<b>Parameters:</b>

 * <i>mantissa</i>: The un-scaled value.

 * <i>exponent</i>: The decimal exponent.

<b>Return Value:</b>

An arbitrary-precision decimal number.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>mantissa</i>
or <i>exponent</i>
is null.

### Equals

    public override bool Equals(
        object obj);

Determines whether this object's mantissa and exponent are equal to those of another object and that other object is an arbitrary-precision decimal number.

<b>Parameters:</b>

 * <i>obj</i>: The parameter <i>obj</i>
is an arbitrary object.

<b>Return Value:</b>

 `true` if the objects are equal; otherwise,  `false` .

### Equals

    public sealed bool Equals(
        PeterO.ExtendedDecimal other);

<b>Deprecated.</b> Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.

Determines whether this object's mantissa and exponent are equal to those of another object.

<b>Parameters:</b>

 * <i>other</i>: An arbitrary-precision decimal number.

<b>Return Value:</b>

 `true` if this object's mantissa and exponent are equal to those of another bject; otherwise,  `false` .

### FromString

    public static PeterO.ExtendedDecimal FromString(
        string str);

Creates a decimal number from a text string that represents a number. See `FromString(String, int, int, EContext)` for more information.

<b>Parameters:</b>

 * <i>str</i>: A string that represents a number.

<b>Return Value:</b>

An arbitrary-precision decimal number with the same value as the given string.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
is null.

 * System.FormatException:
The parameter <i>str</i>
is not a correctly formatted number string.

### GetHashCode

    public override int GetHashCode();

Calculates this object's hash code. No application or process IDs are used in the hash code calculation.

<b>Return Value:</b>

This object's hash code.

### IsInfinity

    public bool IsInfinity();

Gets a value indicating whether this object is positive or negative infinity.

<b>Return Value:</b>

 `true` if this object is positive or negative infinity; otherwise,  `false` .

### IsNaN

    public bool IsNaN();

Gets a value indicating whether this object is not a number (NaN).

<b>Return Value:</b>

 `true` if this object is not a number (NaN); otherwise,  `false` .

### IsQuietNaN

    public bool IsQuietNaN();

<b>Deprecated.</b> Use EDecimal from PeterO.Numbers/com.upokecenter.numbers.

Gets a value indicating whether this object is a quiet not-a-number value.

<b>Return Value:</b>

 `true` if this object is a quiet not-a-number value; otherwise,  `false` .

### ToDouble

    public double ToDouble();

Converts this value to a 64-bit floating-point number. The half-even rounding mode is used.If this value is a NaN, sets the high bit of the 64-bit floating point umber's mantissa for a quiet NaN, and clears it for a signaling NaN. hen the next highest bit of the mantissa is cleared for a quiet NaN, nd set for a signaling NaN. Then the other bits of the mantissa are set o the lowest bits of this object's unsigned mantissa.

<b>Return Value:</b>

The closest 64-bit floating-point number to this value. The return value can be positive infinity or negative infinity if this value exceeds the range of a 64-bit floating point number.

### ToSingle

    public float ToSingle();

Converts this value to a 32-bit floating-point number. The half-even rounding mode is used.If this value is a NaN, sets the high bit of the 32-bit floating point umber's mantissa for a quiet NaN, and clears it for a signaling NaN. hen the next highest bit of the mantissa is cleared for a quiet NaN, nd set for a signaling NaN. Then the other bits of the mantissa are set o the lowest bits of this object's unsigned mantissa.

<b>Return Value:</b>

The closest 32-bit floating-point number to this value. The return value can be positive infinity or negative infinity if this value exceeds the range of a 32-bit floating point number.

### ToString

    public override string ToString();

Converts this value to a string. Returns a value compatible with this class's FromString method.

<b>Return Value:</b>

A string representation of this object.
