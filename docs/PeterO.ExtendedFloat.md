## PeterO.ExtendedFloat

    public sealed class ExtendedFloat :
        System.IComparable,
        System.IEquatable

Represents an arbitrary-precision binary floating-point number. Consists of an integer mantissa and an integer exponent, both arbitrary-precision. The value of the number equals mantissa * 2^exponent. This class also supports values for negative zero, not-a-number (NaN) values, and infinity. Passing a signaling NaN to any arithmetic operation shown here will signal the flag FlagInvalid and return a quiet NaN, even if another operand to that operation is a quiet NaN, unless noted otherwise.

Passing a quiet NaN to any arithmetic operation shown here will return a quiet NaN, unless noted otherwise.

Unless noted otherwise, passing a null ExtendedFloat argument to any method here will throw an exception.

When an arithmetic operation signals the flag FlagInvalid, FlagOverflow, or FlagDivideByZero, it will not throw an exception too, unless the operation's trap is enabled in the precision context (see PrecisionContext's Traps property).

An ExtendedFloat value can be serialized in one of the following ways:

 * By calling the toString() method. However, not all strings can be converted back to an ExtendedFloat without loss, especially if the string has a fractional part.

 * By calling the UnsignedMantissa, Exponent, and IsNegative properties, and calling the IsInfinity, IsQuietNaN, and IsSignalingNaN methods. The return values combined will uniquely identify a particular ExtendedFloat value.

### NaN

    public static readonly PeterO.ExtendedFloat NaN;

A not-a-number value.

### NegativeInfinity

    public static readonly PeterO.ExtendedFloat NegativeInfinity;

Negative infinity, less than any other number.

### NegativeZero

    public static readonly PeterO.ExtendedFloat NegativeZero;

Represents the number negative zero.

### One

    public static readonly PeterO.ExtendedFloat One;

Represents the number 1.

### PositiveInfinity

    public static readonly PeterO.ExtendedFloat PositiveInfinity;

Positive infinity, greater than any other number.

### SignalingNaN

    public static readonly PeterO.ExtendedFloat SignalingNaN;

A not-a-number value that signals an invalid operation flag when it's passed as an argument to any arithmetic operation in ExtendedFloat.

### Ten

    public static readonly PeterO.ExtendedFloat Ten;

Represents the number 10.

### Zero

    public static readonly PeterO.ExtendedFloat Zero;

Represents the number 0.

### Exponent

    public PeterO.BigInteger Exponent { get; }

Gets this object's exponent. This object's value will be an integer if the exponent is positive or zero.

<b>Returns:</b>

This object's exponent. This object's value will be an integer if the exponent is positive or zero.

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

### IsZero

    public bool IsZero { get; }

Gets a value indicating whether this object's value equals 0.

<b>Returns:</b>

True if this object's value equals 0; otherwise, false.

### Mantissa

    public PeterO.BigInteger Mantissa { get; }

Gets this object's un-scaled value.

<b>Returns:</b>

This object's un-scaled value. Will be negative if this object's value is negative (including a negative NaN).

### Sign

    public int Sign { get; }

Gets this value's sign: -1 if negative; 1 if positive; 0 if zero.

<b>Returns:</b>

This value's sign: -1 if negative; 1 if positive; 0 if zero.

### UnsignedMantissa

    public PeterO.BigInteger UnsignedMantissa { get; }

Gets the absolute value of this object's un-scaled value.

<b>Returns:</b>

The absolute value of this object's un-scaled value.

### Abs

    public PeterO.ExtendedFloat Abs(
        PeterO.PrecisionContext context);

Finds the absolute value of this object (if it's negative, it becomes positive).

<b>Parameters:</b>

 * <i>context</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

The absolute value of this object.

### Abs

    public PeterO.ExtendedFloat Abs();

Gets the absolute value of this object.

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

### CompareTo

    public sealed int CompareTo(
        PeterO.ExtendedFloat other);

Compares the mathematical values of this object and another object, accepting NaN values. This method is not consistent with the Equals method because two different numbers with the same mathematical value, but different exponents, will compare as equal.

In this method, negative zero and positive zero are considered equal.

If this object or the other object is a quiet NaN or signaling NaN, this method will not trigger an error. Instead, NaN will compare greater than any other number, including infinity. Two different NaN values will be considered equal.

<b>Parameters:</b>

 * <i>other</i>: An ExtendedFloat object.

<b>Returns:</b>

Less than 0 if this object's value is less than the other value, or greater than 0 if this object's value is greater than the other value or if <i>other</i>
 is null, or 0 if both values are equal.

### CompareToSignal

    public PeterO.ExtendedFloat CompareToSignal(
        PeterO.ExtendedFloat other,
        PeterO.PrecisionContext ctx);

Compares the mathematical values of this object and another object, treating quiet NaN as signaling. In this method, negative zero and positive zero are considered equal.

If this object or the other object is a quiet NaN or signaling NaN, this method will return a quiet NaN and will signal a FlagInvalid flag.

<b>Parameters:</b>

 * <i>other</i>: An ExtendedFloat object.

 * <i>ctx</i>: A precision context. The precision, rounding, and exponent range are ignored. If HasFlags of the context is true, will store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

Quiet NaN if this object or the other object is NaN, or 0 if both objects have the same value, or -1 if this object is less than the other value, or 1 if this object is greater.

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

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>mantissa</i>
 or  <i>exponent</i>
 is null.

### CreateNaN

    public static PeterO.ExtendedFloat CreateNaN(
        PeterO.BigInteger diag);

Creates a not-a-number ExtendedFloat object.

<b>Parameters:</b>

 * <i>diag</i>: A number to use as diagnostic information associated with this object. If none is needed, should be zero.

<b>Returns:</b>

A quiet not-a-number object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>diag</i>
 is null.

 * System.ArgumentException:
The parameter  <i>diag</i>
 is less than 0.

### CreateNaN

    public static PeterO.ExtendedFloat CreateNaN(
        PeterO.BigInteger diag,
        bool signaling,
        bool negative,
        PeterO.PrecisionContext ctx);

Creates a not-a-number ExtendedFloat object.

<b>Parameters:</b>

 * <i>diag</i>: A number to use as diagnostic information associated with this object. If none is needed, should be zero.

 * <i>signaling</i>: Whether the return value will be signaling (true) or quiet (false).

 * <i>negative</i>: Whether the return value is negative.

 * <i>ctx</i>: A PrecisionContext object.

<b>Returns:</b>

An ExtendedFloat object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>diag</i>
 is null.

 * System.ArgumentException:
The parameter  <i>diag</i>
 is less than 0.

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

### DivideAndRemainderNaturalScale

    public PeterO.ExtendedFloat[] DivideAndRemainderNaturalScale(
        PeterO.ExtendedFloat divisor);

Calculates the quotient and remainder using the DivideToIntegerNaturalScale and the formula in RemainderNaturalScale. This is meant to be similar to the divideAndRemainder method in Java's BigDecimal.

<b>Parameters:</b>

 * <i>divisor</i>: The number to divide by.

<b>Returns:</b>

A 2 element array consisting of the quotient and remainder in that order.

### DivideAndRemainderNaturalScale

    public PeterO.ExtendedFloat[] DivideAndRemainderNaturalScale(
        PeterO.ExtendedFloat divisor,
        PeterO.PrecisionContext ctx);

Calculates the quotient and remainder using the DivideToIntegerNaturalScale and the formula in RemainderNaturalScale. This is meant to be similar to the divideAndRemainder method in Java's BigDecimal.

<b>Parameters:</b>

 * <i>divisor</i>: The number to divide by.

 * <i>ctx</i>: A precision context object to control the precision, rounding, and exponent range of the result. This context will be used only in the division portion of the remainder calculation; as a result, it's possible for the remainder to have a higher precision than given in this context. Flags will be set on the given context only if the context's HasFlags is true and the integer part of the division result doesn't fit the precision and exponent range without rounding.

<b>Returns:</b>

A 2 element array consisting of the quotient and remainder in that order.

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

### DivideToIntegerNaturalScale

    public PeterO.ExtendedFloat DivideToIntegerNaturalScale(
        PeterO.ExtendedFloat divisor);

Divides two ExtendedFloat objects, and returns the integer part of the result, rounded down, with the preferred exponent set to this value's exponent minus the divisor's exponent.

<b>Parameters:</b>

 * <i>divisor</i>: The divisor.

<b>Returns:</b>

The integer part of the quotient of the two objects. Signals FlagDivideByZero and returns infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor and the dividend are 0.

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

### Equals

    public override bool Equals(
        object obj);

Determines whether this object's mantissa and exponent are equal to those of another object and that other object is a decimal fraction.

<b>Parameters:</b>

 * <i>obj</i>: An arbitrary object.

<b>Returns:</b>

True if the objects are equal; otherwise, false.

### Equals

    public sealed bool Equals(
        PeterO.ExtendedFloat other);

Determines whether this object's mantissa and exponent are equal to those of another object.

<b>Parameters:</b>

 * <i>other</i>: An ExtendedFloat object.

<b>Returns:</b>

True if this object's mantissa and exponent are equal to those of another object; otherwise, false.

### EqualsInternal

    public bool EqualsInternal(
        PeterO.ExtendedFloat otherValue);

Determines whether this object's mantissa and exponent are equal to those of another object.

<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedFloat object.

<b>Returns:</b>

True if this object's mantissa and exponent are equal to those of another object; otherwise, false.

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

### FromBigInteger

    public static PeterO.ExtendedFloat FromBigInteger(
        PeterO.BigInteger bigint);

Not documented yet.

<b>Parameters:</b>

 * <i>bigint</i>: A BigInteger object.

<b>Returns:</b>

An ExtendedFloat object.

### FromDouble

    public static PeterO.ExtendedFloat FromDouble(
        double dbl);

Creates a binary float from a 64-bit floating-point number. This method computes the exact value of the floating point number, not an approximation, as is often the case by converting the floating point number to a string first.

<b>Parameters:</b>

 * <i>dbl</i>: A 64-bit floating-point number.

<b>Returns:</b>

A binary float with the same value as  <i>dbl</i>
.

### FromInt32

    public static PeterO.ExtendedFloat FromInt32(
        int valueSmaller);

Creates a binary float from a 32-bit signed integer.

<b>Parameters:</b>

 * <i>valueSmaller</i>: A 32-bit signed integer.

<b>Returns:</b>

An ExtendedDecimal object.

### FromInt64

    public static PeterO.ExtendedFloat FromInt64(
        long valueSmall);

Not documented yet.

<b>Parameters:</b>

 * <i>valueSmall</i>: A 64-bit signed integer.

<b>Returns:</b>

An ExtendedFloat object.

### FromSingle

    public static PeterO.ExtendedFloat FromSingle(
        float flt);

Creates a binary float from a 32-bit floating-point number. This method computes the exact value of the floating point number, not an approximation, as is often the case by converting the floating point number to a string first.

<b>Parameters:</b>

 * <i>flt</i>: A 32-bit floating-point number.

<b>Returns:</b>

A binary float with the same value as  <i>flt</i>
.

### FromString

    public static PeterO.ExtendedFloat FromString(
        string str);

Not documented yet.

<b>Parameters:</b>

 * <i>str</i>: A String object.

<b>Returns:</b>

An ExtendedFloat object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
is null.

### FromString

    public static PeterO.ExtendedFloat FromString(
        string str,
        int offset,
        int length);

Not documented yet.

<b>Parameters:</b>

 * <i>str</i>: A String object.

 * <i>offset</i>: A 32-bit signed integer.

 * <i>length</i>: A 32-bit signed integer. (2).

<b>Returns:</b>

An ExtendedFloat object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
is null.

### FromString

    public static PeterO.ExtendedFloat FromString(
        string str,
        int offset,
        int length,
        PeterO.PrecisionContext ctx);

Creates a binary float from a string that represents a number. Note that if the string contains a negative exponent, the resulting value might not be exact. However, the resulting binary float will contain enough precision to accurately convert it to a 32-bit or 64-bit floating point number (float or double). The format of the string generally consists of:

 * An optional '-' or '+' character (if '-' , the value is negative.)

 * One or more digits, with a single optional decimal point after the first digit and before the last digit.

 * Optionally, E+ (positive exponent) or E- (negative exponent) plus one or more digits specifying the exponent.

The string can also be "-INF" , "-Infinity" , "Infinity" , "INF", quiet NaN ("qNaN") followed by any number of digits, or signaling NaN ("sNaN") followed by any number of digits, all in any combination of upper and lower case.

The format generally follows the definition in java.math.BigDecimal(), except that the digits must be ASCII digits ('0' through '9').

<b>Parameters:</b>

 * <i>str</i>: A String object.

 * <i>offset</i>: A 32-bit signed integer.

 * <i>length</i>: A 32-bit signed integer. (2).

 * <i>ctx</i>: A PrecisionContext object.

<b>Returns:</b>

An ExtendedFloat object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
is null.

### FromString

    public static PeterO.ExtendedFloat FromString(
        string str,
        PeterO.PrecisionContext ctx);

Not documented yet.

<b>Parameters:</b>

 * <i>str</i>: A String object.

 * <i>ctx</i>: A PrecisionContext object.

<b>Returns:</b>

An ExtendedFloat object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
is null.

### GetHashCode

    public override int GetHashCode();

Calculates this object's hash code.

<b>Returns:</b>

This object's hash code.

### IsInfinity

    public bool IsInfinity();

Gets a value indicating whether this object is positive or negative infinity.

<b>Returns:</b>

True if this object is positive or negative infinity; otherwise, false.

### IsNaN

    public bool IsNaN();

Returns whether this object is a not-a-number value.

<b>Returns:</b>

True if this object is a not-a-number value; otherwise, false.

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

### Max

    public static PeterO.ExtendedFloat Max(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second);

Gets the greater value between two binary floats.

<b>Parameters:</b>

 * <i>first</i>: An ExtendedFloat object.

 * <i>second</i>: Another ExtendedFloat object.

<b>Returns:</b>

The larger value of the two objects.

### Max

    public static PeterO.ExtendedFloat Max(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second,
        PeterO.PrecisionContext ctx);

Gets the greater value between two binary floats.

<b>Parameters:</b>

 * <i>first</i>: An ExtendedFloat object.

 * <i>second</i>: Another ExtendedFloat object.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

The larger value of the two objects.

### MaxMagnitude

    public static PeterO.ExtendedFloat MaxMagnitude(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second);

Gets the greater value between two values, ignoring their signs. If the absolute values are equal, has the same effect as Max.

<b>Parameters:</b>

 * <i>first</i>: Another ExtendedFloat object.

 * <i>second</i>: An ExtendedFloat object. (3).

<b>Returns:</b>

An ExtendedFloat object.

### MaxMagnitude

    public static PeterO.ExtendedFloat MaxMagnitude(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second,
        PeterO.PrecisionContext ctx);

Gets the greater value between two values, ignoring their signs. If the absolute values are equal, has the same effect as Max.

<b>Parameters:</b>

 * <i>first</i>: Another ExtendedFloat object.

 * <i>second</i>: An ExtendedFloat object. (3).

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

An ExtendedFloat object.

### Min

    public static PeterO.ExtendedFloat Min(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second);

Gets the lesser value between two binary floats.

<b>Parameters:</b>

 * <i>first</i>: An ExtendedFloat object.

 * <i>second</i>: Another ExtendedFloat object.

<b>Returns:</b>

The smaller value of the two objects.

### Min

    public static PeterO.ExtendedFloat Min(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second,
        PeterO.PrecisionContext ctx);

Gets the lesser value between two binary floats.

<b>Parameters:</b>

 * <i>first</i>: An ExtendedFloat object.

 * <i>second</i>: Another ExtendedFloat object.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

The smaller value of the two objects.

### MinMagnitude

    public static PeterO.ExtendedFloat MinMagnitude(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second);

Gets the lesser value between two values, ignoring their signs. If the absolute values are equal, has the same effect as Min.

<b>Parameters:</b>

 * <i>first</i>: Another ExtendedFloat object.

 * <i>second</i>: An ExtendedFloat object. (3).

<b>Returns:</b>

An ExtendedFloat object.

### MinMagnitude

    public static PeterO.ExtendedFloat MinMagnitude(
        PeterO.ExtendedFloat first,
        PeterO.ExtendedFloat second,
        PeterO.PrecisionContext ctx);

Gets the lesser value between two values, ignoring their signs. If the absolute values are equal, has the same effect as Min.

<b>Parameters:</b>

 * <i>first</i>: Another ExtendedFloat object.

 * <i>second</i>: An ExtendedFloat object. (3).

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

An ExtendedFloat object.

### MovePointLeft

    public PeterO.ExtendedFloat MovePointLeft(
        int places);

Returns a number similar to this number but with the radix point moved to the left.

<b>Parameters:</b>

 * <i>places</i>: A 32-bit signed integer.

<b>Returns:</b>

An ExtendedFloat object.

### MovePointLeft

    public PeterO.ExtendedFloat MovePointLeft(
        int places,
        PeterO.PrecisionContext ctx);

Returns a number similar to this number but with the radix point moved to the left.

<b>Parameters:</b>

 * <i>places</i>: A 32-bit signed integer.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

An ExtendedFloat object.

### MovePointLeft

    public PeterO.ExtendedFloat MovePointLeft(
        PeterO.BigInteger bigPlaces);

Returns a number similar to this number but with the radix point moved to the left.

<b>Parameters:</b>

 * <i>bigPlaces</i>: A BigInteger object.

<b>Returns:</b>

An ExtendedFloat object.

### MovePointLeft

    public PeterO.ExtendedFloat MovePointLeft(
        PeterO.BigInteger bigPlaces,
        PeterO.PrecisionContext ctx);

Returns a number similar to this number but with the radix point moved to the left.

<b>Parameters:</b>

 * <i>bigPlaces</i>: A BigInteger object.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

An ExtendedFloat object.

### MovePointRight

    public PeterO.ExtendedFloat MovePointRight(
        int places);

Returns a number similar to this number but with the radix point moved to the right.

<b>Parameters:</b>

 * <i>places</i>: A 32-bit signed integer.

<b>Returns:</b>

An ExtendedFloat object.

### MovePointRight

    public PeterO.ExtendedFloat MovePointRight(
        int places,
        PeterO.PrecisionContext ctx);

Returns a number similar to this number but with the radix point moved to the right.

<b>Parameters:</b>

 * <i>places</i>: A 32-bit signed integer.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

An ExtendedFloat object.

### MovePointRight

    public PeterO.ExtendedFloat MovePointRight(
        PeterO.BigInteger bigPlaces);

Returns a number similar to this number but with the radix point moved to the right.

<b>Parameters:</b>

 * <i>bigPlaces</i>: A BigInteger object.

<b>Returns:</b>

An ExtendedFloat object.

### MovePointRight

    public PeterO.ExtendedFloat MovePointRight(
        PeterO.BigInteger bigPlaces,
        PeterO.PrecisionContext ctx);

Returns a number similar to this number but with the radix point moved to the right.

<b>Parameters:</b>

 * <i>bigPlaces</i>: A BigInteger object.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

A number whose scale is increased by  <i>bigPlaces</i>
 , but not to more than 0.

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

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter "otherValue" is null.

### Negate

    public PeterO.ExtendedFloat Negate(
        PeterO.PrecisionContext context);

Returns a binary float with the same value as this object but with the sign reversed.

<b>Parameters:</b>

 * <i>context</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

An ExtendedFloat object.

### Negate

    public PeterO.ExtendedFloat Negate();

Gets an object with the same value as this one, but with the sign reversed.

<b>Returns:</b>

An ExtendedFloat object.

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

### Plus

    public PeterO.ExtendedFloat Plus(
        PeterO.PrecisionContext ctx);

Rounds this object's value to a given precision, using the given rounding mode and range of exponent, and also converts negative zero to positive zero.

<b>Parameters:</b>

 * <i>ctx</i>: A context for controlling the precision, rounding mode, and exponent range. Can be null.

<b>Returns:</b>

The closest value to this object's value, rounded to the specified precision. Returns the same value as this object if  <i>ctx</i>
is null or the precision and exponent range are unlimited.

### Pow

    public PeterO.ExtendedFloat Pow(
        int exponentSmall);

Raises this object's value to the given exponent.

<b>Parameters:</b>

 * <i>exponentSmall</i>: A 32-bit signed integer.

<b>Returns:</b>

This^exponent. Returns NaN if this object and exponent are both 0.

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

### Precision

    public PeterO.BigInteger Precision();

Finds the number of digits in this number's mantissa. Returns 1 if this value is 0, and 0 if this value is infinity or NaN.

<b>Returns:</b>

A BigInteger object.

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
        PeterO.ExtendedFloat otherValue,
        PeterO.PrecisionContext ctx);

Returns a binary float with the same value as this object but with the same exponent as another binary float.

<b>Parameters:</b>

 * <i>otherValue</i>: A binary float containing the desired exponent of the result. The mantissa is ignored. The exponent is the number of fractional digits in the result, expressed as a negative number. Can also be positive, which eliminates lower-order places from the number. For example, -3 means round to the thousandth (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A value of 0 rounds the number to an integer.

 * <i>ctx</i>: A precision context to control precision and rounding of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.

<b>Returns:</b>

A binary float with the same value as this object but with the exponent changed. Signals FlagInvalid and returns NaN if an overflow error occurred, or the result can't fit the given precision without rounding. Signals FlagInvalid and returns NaN if the new exponent is outside of the valid range of the precision context, if it defines an exponent range.

### Reduce

    public PeterO.ExtendedFloat Reduce(
        PeterO.PrecisionContext ctx);

Removes trailing zeros from this object's mantissa. For example, 1.000 becomes 1. If this object's value is 0, changes the exponent to 0. (This is unlike the behavior in Java's BigDecimal method "stripTrailingZeros" in Java 7 and earlier.)

<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

This value with trailing zeros removed. Note that if the result has a very high exponent and the context says to clamp high exponents, there may still be some trailing zeros in the mantissa.

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

### RemainderNaturalScale

    public PeterO.ExtendedFloat RemainderNaturalScale(
        PeterO.ExtendedFloat divisor);

Not documented yet.

<b>Parameters:</b>

 * <i>divisor</i>: Another ExtendedFloat object.

<b>Returns:</b>

An ExtendedFloat object.

### RemainderNaturalScale

    public PeterO.ExtendedFloat RemainderNaturalScale(
        PeterO.ExtendedFloat divisor,
        PeterO.PrecisionContext ctx);

Calculates the remainder of a number by the formula this - ((this / divisor) * divisor). This is meant to be similar to the remainder operation in Java's BigDecimal.

<b>Parameters:</b>

 * <i>divisor</i>: Another ExtendedFloat object.

 * <i>ctx</i>: A precision context object to control the precision, rounding, and exponent range of the integer part of the result. This context will be used only in the division portion of the remainder calculation. Flags will be set on the given context only if the context's HasFlags is true and the integer part of the division result doesn't fit the precision and exponent range without rounding.

<b>Returns:</b>

An ExtendedFloat object.

### RemainderNear

    public PeterO.ExtendedFloat RemainderNear(
        PeterO.ExtendedFloat divisor,
        PeterO.PrecisionContext ctx);

Finds the distance to the closest multiple of the given divisor, based on the result of dividing this object's value by another object's value.

 * If this and the other object divide evenly, the result is 0.

 * If the remainder's absolute value is less than half of the divisor's absolute value, the result has the same sign as this object and will be the distance to the closest multiple.

 * If the remainder's absolute value is more than half of the divisor' s absolute value, the result has the opposite sign of this object and will be the distance to the closest multiple.

 * If the remainder's absolute value is exactly half of the divisor's absolute value, the result has the opposite sign of this object if the quotient, rounded down, is odd, and has the same sign as this object if the quotient, rounded down, is even, and the result's absolute value is half of the divisor's absolute value.

This function is also known as the "IEEE Remainder" function.

<b>Parameters:</b>

 * <i>divisor</i>: The divisor.

 * <i>ctx</i>: A precision context object to control the precision. The rounding and exponent range settings of this context are ignored (the rounding mode is always treated as HalfEven). If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

The distance of the closest multiple. Signals FlagInvalid and returns NaN if the divisor is 0, or either the result of integer division (the quotient) or the remainder wouldn't fit the given precision.

### RoundToBinaryPrecision

    public PeterO.ExtendedFloat RoundToBinaryPrecision(
        PeterO.PrecisionContext ctx);

<b>Deprecated.</b> Instead of this method use RoundToPrecision and pass a precision context with the IsPrecisionInBits property set.

Rounds this object's value to a given maximum bit length, using the given rounding mode and range of exponent.

<b>Parameters:</b>

 * <i>ctx</i>: A context for controlling the precision, rounding mode, and exponent range. The precision is interpreted as the maximum bit length of the mantissa. Can be null.

<b>Returns:</b>

The closest value to this object's value, rounded to the specified precision. Returns the same value as this object if  <i>ctx</i>
is null or the precision and exponent range are unlimited.

### RoundToExponent

    public PeterO.ExtendedFloat RoundToExponent(
        int exponentSmall,
        PeterO.PrecisionContext ctx);

Returns a binary number with the same value as this object, and rounds it to a new exponent if necessary.

<b>Parameters:</b>

 * <i>exponentSmall</i>: The minimum exponent the result can have. This is the maximum number of fractional digits in the result, expressed as a negative number. Can also be positive, which eliminates lower-order places number. For example, -3 means round to the sixteenth (10b^-3, 0.0001b), and 3 means round to the sixteen-place (10b^3, 1000b). A value of 0 rounds the number to an integer.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.

<b>Returns:</b>

A binary number rounded to the closest value representable in the given precision, meaning if the result can't fit the precision, additional digits are discarded to make it fit. Signals FlagInvalid and returns NaN if the precision context defines an exponent range, the new exponent must be changed to the given exponent when rounding, and the given exponent is outside of the valid range of the precision context.

### RoundToExponent

    public PeterO.ExtendedFloat RoundToExponent(
        PeterO.BigInteger exponent,
        PeterO.PrecisionContext ctx);

Returns a binary number with the same value as this object, and rounds it to a new exponent if necessary.

<b>Parameters:</b>

 * <i>exponent</i>: The minimum exponent the result can have. This is the maximum number of fractional digits in the result, expressed as a negative number. Can also be positive, which eliminates lower-order places from the number. For example, -3 means round to the sixteenth (10b^-3, 0.0001b), and 3 means round to the sixteen-place (10b^3, 1000b). A value of 0 rounds the number to an integer.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.

<b>Returns:</b>

A binary number rounded to the closest value representable in the given precision, meaning if the result can't fit the precision, additional digits are discarded to make it fit. Signals FlagInvalid and returns NaN if the precision context defines an exponent range, the new exponent must be changed to the given exponent when rounding, and the given exponent is outside of the valid range of the precision context.

### RoundToExponentExact

    public PeterO.ExtendedFloat RoundToExponentExact(
        int exponentSmall,
        PeterO.PrecisionContext ctx);

Returns a binary number with the same value as this object but rounded to an integer, and signals an invalid operation if the result would be inexact.

<b>Parameters:</b>

 * <i>exponentSmall</i>: The minimum exponent the result can have. This is the maximum number of fractional digits in the result, expressed as a negative number. Can also be positive, which eliminates lower-order places number. For example, -3 means round to the sixteenth (10b^-3, 0.0001b), and 3 means round to the sixteen-place (10b^3, 1000b). A value of 0 rounds the number to an integer.

 * <i>ctx</i>: A PrecisionContext object.

<b>Returns:</b>

A binary number rounded to the closest value representable in the given precision. Signals FlagInvalid and returns NaN if the result can't fit the given precision without rounding. Signals FlagInvalid and returns NaN if the precision context defines an exponent range, the new exponent must be changed to the given exponent when rounding, and the given exponent is outside of the valid range of the precision context.

### RoundToExponentExact

    public PeterO.ExtendedFloat RoundToExponentExact(
        PeterO.BigInteger exponent,
        PeterO.PrecisionContext ctx);

Returns a binary number with the same value as this object but rounded to an integer, and signals an invalid operation if the result would be inexact.

<b>Parameters:</b>

 * <i>exponent</i>: The minimum exponent the result can have. This is the maximum number of fractional digits in the result, expressed as a negative number. Can also be positive, which eliminates lower-order places from the number. For example, -3 means round to the sixteenth (10b^-3, 0.0001b), and 3 means round to the sixteen-place (10b^3, 1000b). A value of 0 rounds the number to an integer.

 * <i>ctx</i>: A PrecisionContext object.

<b>Returns:</b>

A binary number rounded to the closest value representable in the given precision. Signals FlagInvalid and returns NaN if the result can't fit the given precision without rounding. Signals FlagInvalid and returns NaN if the precision context defines an exponent range, the new exponent must be changed to the given exponent when rounding, and the given exponent is outside of the valid range of the precision context.

### RoundToIntegralExact

    public PeterO.ExtendedFloat RoundToIntegralExact(
        PeterO.PrecisionContext ctx);

Returns a binary number with the same value as this object but rounded to an integer, and signals an invalid operation if the result would be inexact.

<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision and rounding of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case the default rounding mode is HalfEven.

<b>Returns:</b>

A binary number rounded to the closest integer representable in the given precision. Signals FlagInvalid and returns NaN if the result can't fit the given precision without rounding. Signals FlagInvalid and returns NaN if the precision context defines an exponent range, the new exponent must be changed to 0 when rounding, and 0 is outside of the valid range of the precision context.

### RoundToIntegralNoRoundedFlag

    public PeterO.ExtendedFloat RoundToIntegralNoRoundedFlag(
        PeterO.PrecisionContext ctx);

Returns a binary number with the same value as this object but rounded to an integer, without adding the FlagInexact or FlagRounded flags.

<b>Parameters:</b>

 * <i>ctx</i>: A precision context to control precision and rounding of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags), except that this function will never add the FlagRounded and FlagInexact flags (the only difference between this and RoundToExponentExact). Can be null, in which case the default rounding mode is HalfEven.

<b>Returns:</b>

A binary number rounded to the closest integer representable in the given precision, meaning if the result can't fit the precision, additional digits are discarded to make it fit. Signals FlagInvalid and returns NaN if the precision context defines an exponent range, the new exponent must be changed to 0 when rounding, and 0 is outside of the valid range of the precision context.

### RoundToPrecision

    public PeterO.ExtendedFloat RoundToPrecision(
        PeterO.PrecisionContext ctx);

Rounds this object's value to a given precision, using the given rounding mode and range of exponent.

<b>Parameters:</b>

 * <i>ctx</i>: A context for controlling the precision, rounding mode, and exponent range. Can be null.

<b>Returns:</b>

The closest value to this object's value, rounded to the specified precision. Returns the same value as this object if  <i>ctx</i>
is null or the precision and exponent range are unlimited.

### ScaleByPowerOfTwo

    public PeterO.ExtendedFloat ScaleByPowerOfTwo(
        int places);

Returns a number similar to this number but with the scale adjusted.

<b>Parameters:</b>

 * <i>places</i>: A 32-bit signed integer.

<b>Returns:</b>

An ExtendedDecimal object.

### ScaleByPowerOfTwo

    public PeterO.ExtendedFloat ScaleByPowerOfTwo(
        int places,
        PeterO.PrecisionContext ctx);

Returns a number similar to this number but with the scale adjusted.

<b>Parameters:</b>

 * <i>places</i>: A 32-bit signed integer.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

An ExtendedDecimal object.

### ScaleByPowerOfTwo

    public PeterO.ExtendedFloat ScaleByPowerOfTwo(
        PeterO.BigInteger bigPlaces);

Returns a number similar to this number but with the scale adjusted.

<b>Parameters:</b>

 * <i>bigPlaces</i>: A BigInteger object.

<b>Returns:</b>

An ExtendedDecimal object.

### ScaleByPowerOfTwo

    public PeterO.ExtendedFloat ScaleByPowerOfTwo(
        PeterO.BigInteger bigPlaces,
        PeterO.PrecisionContext ctx);

Returns a number similar to this number but with its scale adjusted.

<b>Parameters:</b>

 * <i>bigPlaces</i>: A BigInteger object.

 * <i>ctx</i>: A precision context to control precision, rounding, and exponent range of the result. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null.

<b>Returns:</b>

A number whose scale is increased by  <i>bigPlaces</i>
.

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

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>otherValue</i>
 is null.

### ToBigInteger

    public PeterO.BigInteger ToBigInteger();

Converts this value to an arbitrary-precision integer. Any fractional part of this value will be discarded when converting to a big integer.

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.OverflowException:
This object's value is infinity or NaN.

### ToBigIntegerExact

    public PeterO.BigInteger ToBigIntegerExact();

Converts this value to an arbitrary-precision integer, checking whether the value contains a fractional part.

<b>Returns:</b>

A BigInteger object.

<b>Exceptions:</b>

 * System.OverflowException:
This object's value is infinity or NaN.

 * System.ArithmeticException:
This object's value is not an exact integer.

### ToDouble

    public double ToDouble();

Converts this value to a 64-bit floating-point number. The half-even rounding mode is used. If this value is a NaN, sets the high bit of the 64-bit floating point number's mantissa for a quiet NaN, and clears it for a signaling NaN. Then the next highest bit of the mantissa is cleared for a quiet NaN, and set for a signaling NaN. Then the other bits of the mantissa are set to the lowest bits of this object's unsigned mantissa.

<b>Returns:</b>

The closest 64-bit floating-point number to this value. The return value can be positive infinity or negative infinity if this value exceeds the range of a 64-bit floating point number.

### ToEngineeringString

    public string ToEngineeringString();

Same as toString(), except that when an exponent is used it will be a multiple of 3. The format of the return value follows the format of the java.math.BigDecimal.toEngineeringString() method.

<b>Returns:</b>

A string object.

### ToExtendedDecimal

    public PeterO.ExtendedDecimal ToExtendedDecimal();

Not documented yet.

<b>Returns:</b>

An ExtendedDecimal object.

### ToPlainString

    public string ToPlainString();

Converts this value to a string, but without an exponent part. The format of the return value follows the format of the java.math.BigDecimal.toPlainString() method.

<b>Returns:</b>

A string object.

### ToSingle

    public float ToSingle();

Converts this value to a 32-bit floating-point number. The half-even rounding mode is used. If this value is a NaN, sets the high bit of the 32-bit floating point number's mantissa for a quiet NaN, and clears it for a signaling NaN. Then the next highest bit of the mantissa is cleared for a quiet NaN, and set for a signaling NaN. Then the other bits of the mantissa are set to the lowest bits of this object's unsigned mantissa.

<b>Returns:</b>

The closest 32-bit floating-point number to this value. The return value can be positive infinity or negative infinity if this value exceeds the range of a 32-bit floating point number.

### ToString

    public override string ToString();

Converts this value to a string.

<b>Returns:</b>

A string representation of this object.

### Ulp

    public PeterO.ExtendedFloat Ulp();

Returns the unit in the last place. The mantissa will be 1 and the exponent will be this number's exponent. Returns 1 with an exponent of 0 if this number is infinity or NaN.

<b>Returns:</b>

An ExtendedFloat object.
