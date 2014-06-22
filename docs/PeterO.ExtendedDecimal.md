## PeterO.ExtendedDecimal

    public sealed class ExtendedDecimal :
        System.IComparable,
        System.IEquatable

Represents an arbitrary-precision decimal floating-point number. Consists of an integer mantissa and an integer exponent, both arbitrary-precision. The value of the number equals mantissa * 10^exponent. The mantissa is the value of the digits that make up a number, ignoring the decimal point and exponent. For example, in the number 2356.78, the mantissa is 235678. The exponent is where the "floating" decimal point of the number is located. A positive exponent means "move it to the right", and a negative exponent means "move it to the left." In the example 2,356.78, the exponent is -2, since it has 2 decimal places and the decimal point is "moved to the left by 2." Therefore, in the ExtendedDecimal representation, this number would be stored as 235678 * 10^-2.

The mantissa and exponent format preserves trailing zeros in the number's value. This may give rise to multiple ways to store the same value. For example, 1.00 and 1 would be stored differently, even though they have the same value. In the first case, 100 * 10^-2 (100 with decimal point moved left by 2), and in the second case, 1 * 10^0 (1 with decimal point moved 0).

This class also supports values for negative zero, not-a-number (NaN) values, and infinity. Negative zerois generally used when a negative number is rounded to 0; it has the same mathematical value as positive zero. Infinityis generally used when a non-zero number is divided by zero, or when a very high number can't be represented in a given exponent range. Not-a-numberis generally used to signal errors.

This class implements the General Decimal Arithmetic Specification version 1.70:  `http://speleotrove.com/decimal/decarith.html` 

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

Creates a number with the value exponent*10^mantissa.

<b>Parameters:</b>

 * <i>mantissaSmall</i>: The un-scaled value.

 * <i>exponentSmall</i>: The decimal exponent.

<b>Returns:</b>

An ExtendedDecimal object.

### Create

    public static PeterO.ExtendedDecimal Create(
        PeterO.BigInteger mantissa,
        PeterO.BigInteger exponent);

Creates a number with the value exponent*10^mantissa.

<b>Parameters:</b>

 * <i>mantissa</i>: The un-scaled value.

 * <i>exponent</i>: The decimal exponent.

<b>Returns:</b>

An ExtendedDecimal object.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>mantissa</i>
 or  <i>exponent</i>
 is null.

### CreateNaN

    public static PeterO.ExtendedDecimal CreateNaN(
        PeterO.BigInteger diag);

Creates a not-a-number ExtendedDecimal object.

<b>Parameters:</b>

 * <i>diag</i>: A number to use as diagnostic information associated with this object. If none is needed, should be zero.

<b>Returns:</b>

A quiet not-a-number object.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>diag</i>
 is null or is less than 0.

### CreateNaN

    public static PeterO.ExtendedDecimal CreateNaN(
        PeterO.BigInteger diag,
        bool signaling,
        bool negative,
        PeterO.PrecisionContext ctx);

Creates a not-a-number ExtendedDecimal object.

<b>Parameters:</b>

 * <i>diag</i>: A number to use as diagnostic information associated with this object. If none is needed, should be zero.

 * <i>signaling</i>: Whether the return value will be signaling (true) or quiet (false).

 * <i>negative</i>: Whether the return value is negative.

 * <i>ctx</i>: A PrecisionContext object.

<b>Returns:</b>

An ExtendedDecimal object.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>diag</i>
 is null or is less than 0.

### FromString

    public static PeterO.ExtendedDecimal FromString(
        string str);

Creates a decimal number from a string that represents a number. See  `FromString(String, int, int, PrecisionContext)` for more information.

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

Creates a decimal number from a string that represents a number. See  `FromString(String, int, int, PrecisionContext)` for more information.

<b>Parameters:</b>

 * <i>str</i>: A string that represents a number.

 * <i>ctx</i>: A PrecisionContext object.

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
        int offset,
        int length);

Creates a decimal number from a string that represents a number. See  `FromString(String, int, int, PrecisionContext)` for more information.

<b>Parameters:</b>

 * <i>str</i>: A string that represents a number.

 * <i>offset</i>: A 32-bit signed integer.

 * <i>length</i>: A 32-bit signed integer. (2).

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
        int offset,
        int length,
        PeterO.PrecisionContext ctx);

Creates a decimal number from a string that represents a number. The format of the string generally consists of: 

 * An optional '-' or '+' character (if '-', the value is negative.)

 * One or more digits, with a single optional decimal point after the first digit and before the last digit.

 * Optionally, E+ (positive exponent) or E- (negative exponent) plus one or more digits specifying the exponent.

The string can also be "-INF", "-Infinity", "Infinity", "INF", quiet NaN ("qNaN"/"-qNaN") followed by any number of digits, or signaling NaN ("sNaN"/"-sNaN") followed by any number of digits, all in any combination of upper and lower case.

The format generally follows the definition in java.math.BigDecimal(), except that the digits must be ASCII digits ('0' through '9').

<b>Parameters:</b>

 * <i>str</i>: A String object.

 * <i>offset</i>: A 32-bit signed integer.

 * <i>length</i>: A 32-bit signed integer. (2).

 * <i>ctx</i>: A PrecisionContext object.

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

### ToBigIntegerExact

    public PeterO.BigInteger ToBigIntegerExact();

Converts this value to an arbitrary-precision integer, checking whether the fractional part of the integer would be lost.

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.OverflowException: 
This object's value is infinity or NaN.

 * System.ArithmeticException: 
This object's value is not an exact integer.

### ToExtendedFloat

    public PeterO.ExtendedFloat ToExtendedFloat();

Creates a binary floating-point number from this object's value. Note that if the binary floating-point number contains a negative exponent, the resulting value might not be exact. However, the resulting binary float will contain enough precision to accurately convert it to a 32-bit or 64-bit floating point number (float or double).

<b>Returns:</b>

An ExtendedFloat object.

### ToSingle

    public float ToSingle();

Converts this value to a 32-bit floating-point number. The half-even rounding mode is used. If this value is a NaN, sets the high bit of the 32-bit floating point number's mantissa for a quiet NaN, and clears it for a signaling NaN. Then the next highest bit of the mantissa is cleared for a quiet NaN, and set for a signaling NaN. Then the other bits of the mantissa are set to the lowest bits of this object's unsigned mantissa.

<b>Returns:</b>

The closest 32-bit floating-point number to this value. The return value can be positive infinity or negative infinity if this value exceeds the range of a 32-bit floating point number.

### ToDouble

    public double ToDouble();

Converts this value to a 64-bit floating-point number. The half-even rounding mode is used. If this value is a NaN, sets the high bit of the 64-bit floating point number's mantissa for a quiet NaN, and clears it for a signaling NaN. Then the next highest bit of the mantissa is cleared for a quiet NaN, and set for a signaling NaN. Then the other bits of the mantissa are set to the lowest bits of this object's unsigned mantissa.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>bigfloat</i>
 is null.

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

Removes trailing zeros from this object's mantissa. For example, 1.000 becomes 1. If this object's value is 0, changes the exponent to 0. (This is unlike the behavior in Java's BigDecimal method "stripTrailingZeros" in Java 7 and earlier.)

<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

This value with trailing zeros removed. Note that if the result has a very high exponent and the context says to clamp high exponents, there may still be some trailing zeros in the mantissa.

### RemainderNaturalScale

    public PeterO.ExtendedDecimal RemainderNaturalScale(
        PeterO.ExtendedDecimal divisor);

Calculates the remainder of a number by the formula "this" - (("this" / "divisor") * "divisor"). This is meant to be similar to the remainder operation in Java's BigDecimal.

<b>Parameters:</b>

 * <i>divisor</i>: The number to divide by.

<b>Returns:</b>

An ExtendedDecimal object.

### RemainderNaturalScale

    public PeterO.ExtendedDecimal RemainderNaturalScale(
        PeterO.ExtendedDecimal divisor,
        PeterO.PrecisionContext ctx);

Calculates the remainder of a number by the formula "this" - (("this" / "divisor") * "divisor"). This is meant to be similar to the remainder operation in Java's BigDecimal.

<b>Parameters:</b>

 * <i>divisor</i>: The number to divide by.

 * <i>ctx</i>: A precision context object to control the precision, rounding, and exponent range of the result. This context will be used only in the division portion of the remainder calculation; as a result, it's possible for the return value to have a higher precision than given in this context. Flags will be set on the given context only if the context's HasFlags is true and the integer part of the division result doesn't fit the precision and exponent range without rounding.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>otherValue</i>
 is null.

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

 * If this and the other object divide evenly, the result is 0.

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

 * <i>first</i>: The first value to compare.

 * <i>second</i>: The second value to compare.

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

Compares the mathematical values of this object and another object. In this method, negative zero and positive zero are considered equal.

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

Compares the mathematical values of this object and another object, treating quiet NaN as signaling. In this method, negative zero and positive zero are considered equal.

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

Returns a decimal number with the same value as this object but with the same exponent as another decimal number. Note that this is not always the same as rounding to a given number of decimal places, since it can fail if the difference between this value's exponent and the desired exponent is too big, depending on the maximum precision. If rounding to a number of decimal places is desired, it's better to use the RoundToExponent and RoundToIntegral methods instead.

<b>Parameters:</b>

 * <i>otherValue</i>: A decimal number containing the desired exponent of the result. The mantissa is ignored. The exponent is the number of fractional digits in the result, expressed as a negative number. Can also be positive, which eliminates lower-order places from the number. For example, -3 means round to the thousandth (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A value of 0 rounds the number to an integer.

 * <i>ctx</i>: A precision context to control precision and rounding of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.

<b>Returns:</b>

A decimal number with the same value as this object but with the exponent changed. Signals FlagInvalid and returns NaN if an overflow error occurred, or the result can't fit the given precision without rounding. Signals FlagInvalid and returns NaN if the new exponent is outside of the valid range of the precision context, if it defines an exponent range.

### RoundToIntegralExact

    public PeterO.ExtendedDecimal RoundToIntegralExact(
        PeterO.PrecisionContext ctx);

Returns a decimal number with the same value as this object but rounded to an integer, and signals an invalid operation if the result would be inexact.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter "otherValue" is null.

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

<b>Deprecated.</b> Instead of this method, use RoundToPrecision and pass a precision context with the IsPrecisionInBits property set.

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


