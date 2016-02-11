## PeterO.ExtendedRational

    public sealed class ExtendedRational :
        System.IComparable,
        System.IEquatable

This class is largely obsolete. It will be replaced by a new version of this class in a different namespace/package and library, called  `PeterO.Numbers.ERational`  in the  `PeterO.Numbers` library (in .NET), or  `com.upokecenter.numbers.ERational`  in the  `com.github.peteroupc/numbers` artifact (in Java). This new class can be used in the  `CBORObject.FromObject(Object)` method (by including the new library in your code, among other things), but this version of the CBOR library doesn't include any methods that explicitly take an  `ERational`  as a parameter or return value.

Arbitrary-precision rational number. This class cannot be inherited; this is a change in version 2.0 from previous versions, where the class was inadvertently left inheritable.Thread safety:Instances of this class are immutable, so they are inherently safe for use by multiple threads. Multiple instances of this object with the same properties are interchangeable, so they should not be compared using the "==" operator (which only checks if each side of the operator is the same instance).

### ExtendedRational Constructor

    public ExtendedRational(
        PeterO.BigInteger numerator,
        PeterO.BigInteger denominator);

Initializes a new instance of the  class.

<b>Parameters:</b>

 * <i>numerator</i>: An arbitrary-precision integer.

 * <i>denominator</i>: Another arbitrary-precision integer.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>numerator</i>
 or  <i>denominator</i>
 is null.

### NaN

    public static readonly PeterO.ExtendedRational NaN;

A not-a-number value.

### NegativeInfinity

    public static readonly PeterO.ExtendedRational NegativeInfinity;

Negative infinity, less than any other number.

### NegativeZero

    public static readonly PeterO.ExtendedRational NegativeZero;

A rational number for negative zero.

### One

    public static readonly PeterO.ExtendedRational One;

The rational number one.

### PositiveInfinity

    public static readonly PeterO.ExtendedRational PositiveInfinity;

Positive infinity, greater than any other number.

### SignalingNaN

    public static readonly PeterO.ExtendedRational SignalingNaN;

A signaling not-a-number value.

### Ten

    public static readonly PeterO.ExtendedRational Ten;

The rational number ten.

### Zero

    public static readonly PeterO.ExtendedRational Zero;

A rational number for zero.

### Denominator

    public PeterO.BigInteger Denominator { get; }

Gets this object's denominator.

<b>Returns:</b>

This object's denominator.

### IsFinite

    public bool IsFinite { get; }

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Gets a value indicating whether this object is finite (not infinity or NaN).

<b>Returns:</b>

 `true`  if this object is finite (not infinity or NaN); otherwise,  `false` .

### IsNegative

    public bool IsNegative { get; }

Gets a value indicating whether this object's value is negative (including negative zero).

<b>Returns:</b>

 `true`  if this object's value is negative; otherwise,  `false` .

### IsZero

    public bool IsZero { get; }

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Gets a value indicating whether this object's value equals 0.

<b>Returns:</b>

 `true`  if this object's value equals 0; otherwise,  `false` .

### Numerator

    public PeterO.BigInteger Numerator { get; }

Gets this object's numerator.

<b>Returns:</b>

This object's numerator. If this object is a not-a-number value, returns the diagnostic information (which will be negative if this object is negative).

### Sign

    public int Sign { get; }

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Gets the sign of this rational number.

<b>Returns:</b>

Zero if this value is zero or negative zero; -1 if this value is less than 0; and 1 if this value is greater than 0.

### UnsignedNumerator

    public PeterO.BigInteger UnsignedNumerator { get; }

Gets this object's numerator with the sign removed.

<b>Returns:</b>

This object's numerator. If this object is a not-a-number value, returns the diagnostic information.

### Abs

    public PeterO.ExtendedRational Abs();

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Finds the absolute value of this rational number.

<b>Returns:</b>

An arbitrary-precision rational number.

### Add

    public PeterO.ExtendedRational Add(
        PeterO.ExtendedRational otherValue);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Adds two rational numbers.

<b>Parameters:</b>

 * <i>otherValue</i>: Another arbitrary-precision rational number.

<b>Returns:</b>

The sum of the two numbers. Returns not-a-number (NaN) if either operand is NaN.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>otherValue</i>
 is null.

### CompareTo

    public sealed int CompareTo(
        PeterO.ExtendedRational other);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Compares an arbitrary-precision rational number with this instance.

<b>Parameters:</b>

 * <i>other</i>: An arbitrary-precision rational number.

<b>Returns:</b>

Zero if the values are equal; a negative number if this instance is less, or a positive number if this instance is greater.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>other</i>
 is null.

### CompareToBinary

    public int CompareToBinary(
        PeterO.ExtendedFloat other);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Compares an arbitrary-precision binary float with this instance.

<b>Parameters:</b>

 * <i>other</i>: An arbitrary-precision binary float.

<b>Returns:</b>

Zero if the values are equal; a negative number if this instance is less, or a positive number if this instance is greater.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>other</i>
 is null.

### CompareToDecimal

    public int CompareToDecimal(
        PeterO.ExtendedDecimal other);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Compares an arbitrary-precision decimal number with this instance.

<b>Parameters:</b>

 * <i>other</i>: An arbitrary-precision decimal number.

<b>Returns:</b>

Zero if the values are equal; a negative number if this instance is less, or a positive number if this instance is greater.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>other</i>
 is null.

### Create

    public static PeterO.ExtendedRational Create(
        int numeratorSmall,
        int denominatorSmall);

Creates a rational number with the given numerator and denominator.

<b>Parameters:</b>

 * <i>numeratorSmall</i>: A 32-bit signed integer.

 * <i>denominatorSmall</i>: A 32-bit signed integer. (2).

<b>Returns:</b>

An arbitrary-precision rational number.

### Create

    public static PeterO.ExtendedRational Create(
        PeterO.BigInteger numerator,
        PeterO.BigInteger denominator);

Creates a rational number with the given numerator and denominator.

<b>Parameters:</b>

 * <i>numerator</i>: An arbitrary-precision integer.

 * <i>denominator</i>: Another arbitrary-precision integer.

<b>Returns:</b>

An arbitrary-precision rational number.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>numerator</i>
 or  <i>denominator</i>
 is null.

### CreateNaN

    public static PeterO.ExtendedRational CreateNaN(
        PeterO.BigInteger diag);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Creates a not-a-number arbitrary-precision rational number.

<b>Parameters:</b>

 * <i>diag</i>: A number to use as diagnostic information associated with this object. If none is needed, should be zero.

<b>Returns:</b>

An arbitrary-precision rational number.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>diag</i>
 is null.

 * System.ArgumentException:
The parameter <i>diag</i>
 is less than 0.

### CreateNaN

    public static PeterO.ExtendedRational CreateNaN(
        PeterO.BigInteger diag,
        bool signaling,
        bool negative);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Creates a not-a-number arbitrary-precision rational number.

<b>Parameters:</b>

 * <i>diag</i>: A number to use as diagnostic information associated with this object. If none is needed, should be zero.

 * <i>signaling</i>: Whether the return value will be signaling (true) or quiet (false).

 * <i>negative</i>: Whether the return value is negative.

<b>Returns:</b>

An arbitrary-precision rational number.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>diag</i>
 is null.

 * System.ArgumentException:
The parameter <i>diag</i>
 is less than 0.

### Divide

    public PeterO.ExtendedRational Divide(
        PeterO.ExtendedRational otherValue);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Divides this instance by the value of an arbitrary-precision rational number object.

<b>Parameters:</b>

 * <i>otherValue</i>: An arbitrary-precision rational number.

<b>Returns:</b>

The quotient of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>otherValue</i>
 is null.

### Equals

    public override bool Equals(
        object obj);

Determines whether this object and another object are equal.

<b>Parameters:</b>

 * <i>obj</i>: An arbitrary object.

<b>Returns:</b>

 `true`  if the objects are equal; otherwise,  `false` .

### Equals

    public sealed bool Equals(
        PeterO.ExtendedRational other);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Returns whether this object's properties are equal to the properties of another rational number object.

<b>Parameters:</b>

 * <i>other</i>: Another arbitrary-precision rational number.

<b>Returns:</b>

 `true`  if this object's properties are equal to the properties of another rational number object; otherwise,  `false` .

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>other</i>
 is null.

### FromBigInteger

    public static PeterO.ExtendedRational FromBigInteger(
        PeterO.BigInteger bigint);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts a big integer to a rational number.

<b>Parameters:</b>

 * <i>bigint</i>: An arbitrary-precision integer.

<b>Returns:</b>

The exact value of the integer as a rational number.

### FromDouble

    public static PeterO.ExtendedRational FromDouble(
        double flt);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts a 64-bit floating-point number to a rational number. This method computes the exact value of the floating point number, not an approximation, as is often the case by converting the number to a string.

<b>Parameters:</b>

 * <i>flt</i>: A 64-bit floating-point number.

<b>Returns:</b>

A rational number with the same value as  <i>flt</i>
.

### FromExtendedDecimal

    public static PeterO.ExtendedRational FromExtendedDecimal(
        PeterO.ExtendedDecimal ef);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts an arbitrary-precision decimal number to a rational number.

<b>Parameters:</b>

 * <i>ef</i>: An arbitrary-precision decimal number.

<b>Returns:</b>

An arbitrary-precision rational number.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>ef</i>
 is null.

### FromExtendedFloat

    public static PeterO.ExtendedRational FromExtendedFloat(
        PeterO.ExtendedFloat ef);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts an arbitrary-precision binary float to a rational number.

<b>Parameters:</b>

 * <i>ef</i>: An arbitrary-precision binary float.

<b>Returns:</b>

An arbitrary-precision rational number.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>ef</i>
 is null.

### FromInt32

    public static PeterO.ExtendedRational FromInt32(
        int smallint);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts a 32-bit signed integer to a rational number.

<b>Parameters:</b>

 * <i>smallint</i>: A 32-bit signed integer.

<b>Returns:</b>

An arbitrary-precision rational number.

### FromInt64

    public static PeterO.ExtendedRational FromInt64(
        long longInt);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts a 64-bit signed integer to a rational number.

<b>Parameters:</b>

 * <i>longInt</i>: A 64-bit signed integer.

<b>Returns:</b>

An arbitrary-precision rational number.

### FromSingle

    public static PeterO.ExtendedRational FromSingle(
        float flt);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts a 32-bit floating-point number to a rational number. This method computes the exact value of the floating point number, not an approximation, as is often the case by converting the number to a string.

<b>Parameters:</b>

 * <i>flt</i>: A 32-bit floating-point number.

<b>Returns:</b>

A rational number with the same value as  <i>flt</i>
.

### GetHashCode

    public override int GetHashCode();

Returns the hash code for this instance.

<b>Returns:</b>

A 32-bit hash code.

### IsInfinity

    public bool IsInfinity();

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Gets a value indicating whether this object's value is infinity.

<b>Returns:</b>

 `true`  if this object's value is infinity; otherwise,  `false` .

### IsNaN

    public bool IsNaN();

Returns whether this object is a not-a-number value.

<b>Returns:</b>

 `true`  if this object is a not-a-number value; otherwise,  `false` .

### IsNegativeInfinity

    public bool IsNegativeInfinity();

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Returns whether this object is negative infinity.

<b>Returns:</b>

 `true`  if this object is negative infinity; otherwise,  `false` .

### IsPositiveInfinity

    public bool IsPositiveInfinity();

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Returns whether this object is positive infinity.

<b>Returns:</b>

 `true`  if this object is positive infinity; otherwise,  `false` .

### IsQuietNaN

    public bool IsQuietNaN();

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Returns whether this object is a quiet not-a-number value.

<b>Returns:</b>

 `true`  if this object is a quiet not-a-number value; otherwise,  `false` .

### IsSignalingNaN

    public bool IsSignalingNaN();

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Returns whether this object is a signaling not-a-number value (which causes an error if the value is passed to any arithmetic operation in this class).

<b>Returns:</b>

 `true`  if this object is a signaling not-a-number value (which causes an error if the value is passed to any arithmetic operation in this class); otherwise,  `false` .

### Multiply

    public PeterO.ExtendedRational Multiply(
        PeterO.ExtendedRational otherValue);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Multiplies this instance by the value of an arbitrary-precision rational number.

<b>Parameters:</b>

 * <i>otherValue</i>: An arbitrary-precision rational number.

<b>Returns:</b>

The product of the two numbers.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>otherValue</i>
 is null.

### Negate

    public PeterO.ExtendedRational Negate();

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Finds a rational number with the same value as this object but with the sign reversed.

<b>Returns:</b>

The negated form of this rational number.

### Remainder

    public PeterO.ExtendedRational Remainder(
        PeterO.ExtendedRational otherValue);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Finds the remainder that results when this instance is divided by the value of an arbitrary-precision rational number.

<b>Parameters:</b>

 * <i>otherValue</i>: An arbitrary-precision rational number.

<b>Returns:</b>

The remainder of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>otherValue</i>
 is null.

### Subtract

    public PeterO.ExtendedRational Subtract(
        PeterO.ExtendedRational otherValue);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Subtracts an arbitrary-precision rational number from this instance.

<b>Parameters:</b>

 * <i>otherValue</i>: An arbitrary-precision rational number.

<b>Returns:</b>

The difference of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>otherValue</i>
 is null.

### ToBigInteger

    public PeterO.BigInteger ToBigInteger();

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts this value to an arbitrary-precision integer. Any fractional part in this value will be discarded when converting to a big integer.

<b>Returns:</b>

An arbitrary-precision integer.

<b>Exceptions:</b>

 * System.OverflowException:
This object's value is infinity or NaN.

### ToBigIntegerExact

    public PeterO.BigInteger ToBigIntegerExact();

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts this value to an arbitrary-precision integer, checking whether the value is an exact integer.

<b>Returns:</b>

An arbitrary-precision integer.

<b>Exceptions:</b>

 * System.OverflowException:
This object's value is infinity or NaN.

 * System.ArithmeticException:
This object's value is not an exact integer.

### ToDouble

    public double ToDouble();

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts this value to a 64-bit floating-point number. The half-even rounding mode is used.

<b>Returns:</b>

The closest 64-bit floating-point number to this value. The return value can be positive infinity or negative infinity if this value exceeds the range of a 64-bit floating point number.

### ToExtendedDecimal

    public PeterO.ExtendedDecimal ToExtendedDecimal(
        PeterO.PrecisionContext ctx);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts this rational number to a decimal number and rounds the result to the given precision.

<b>Parameters:</b>

 * <i>ctx</i>: A PrecisionContext object.

<b>Returns:</b>

An arbitrary-precision decimal.

### ToExtendedDecimal

    public PeterO.ExtendedDecimal ToExtendedDecimal();

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts this rational number to a decimal number.

<b>Returns:</b>

The exact value of the rational number, or not-a-number (NaN) if the result can't be exact because it has a nonterminating decimal expansion.

### ToExtendedDecimalExactIfPossible

    public PeterO.ExtendedDecimal ToExtendedDecimalExactIfPossible(
        PeterO.PrecisionContext ctx);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts this rational number to a decimal number, but if the result would have a nonterminating decimal expansion, rounds that result to the given precision.

<b>Parameters:</b>

 * <i>ctx</i>: A precision context object to control the precision. The rounding and exponent range settings of this context are ignored. This context will be used only if the exact result would have a nonterminating decimal expansion. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case this method is the same as ToExtendedDecimal.

<b>Returns:</b>

An arbitrary-precision decimal.

### ToExtendedFloat

    public PeterO.ExtendedFloat ToExtendedFloat(
        PeterO.PrecisionContext ctx);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts this rational number to a binary number and rounds the result to the given precision.

<b>Parameters:</b>

 * <i>ctx</i>: A PrecisionContext object.

<b>Returns:</b>

An arbitrary-precision binary float.

### ToExtendedFloat

    public PeterO.ExtendedFloat ToExtendedFloat();

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts this rational number to a binary number.

<b>Returns:</b>

The exact value of the rational number, or not-a-number (NaN) if the result can't be exact because it has a nonterminating binary expansion.

### ToExtendedFloatExactIfPossible

    public PeterO.ExtendedFloat ToExtendedFloatExactIfPossible(
        PeterO.PrecisionContext ctx);

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts this rational number to a binary number, but if the result would have a nonterminating binary expansion, rounds that result to the given precision.

<b>Parameters:</b>

 * <i>ctx</i>: A precision context object to control the precision. The rounding and exponent range settings of this context are ignored. This context will be used only if the exact result would have a nonterminating binary expansion. If HasFlags of the context is true, will also store the flags resulting from the operation (the flags are in addition to the pre-existing flags). Can be null, in which case this method is the same as ToExtendedFloat.

<b>Returns:</b>

An arbitrary-precision binary float.

### ToSingle

    public float ToSingle();

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Converts this value to a 32-bit floating-point number. The half-even rounding mode is used.

<b>Returns:</b>

The closest 32-bit floating-point number to this value. The return value can be positive infinity or negative infinity if this value exceeds the range of a 32-bit floating point number.

### ToString

    public override string ToString();

Converts this object to a text string.

<b>Returns:</b>

A string representation of this object. The result can be Infinity, NaN, or sNaN (with a minus sign before it for negative values), or a number of the following form: [-]numerator/denominator.
