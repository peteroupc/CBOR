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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>numerator</i>
 or  <i>denominator</i>
is null.

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

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>diag</i>
 is null.

### FromExtendedFloat

    public static PeterO.ExtendedRational FromExtendedFloat(
        PeterO.ExtendedFloat ef);

Not documented yet.

<b>Parameters:</b>

 * <i>ef</i>: An ExtendedFloat object.

<b>Returns:</b>

An ExtendedRational object.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>ef</i>
 is null.

### FromExtendedDecimal

    public static PeterO.ExtendedRational FromExtendedDecimal(
        PeterO.ExtendedDecimal ef);

Not documented yet.

<b>Parameters:</b>

 * <i>ef</i>: An ExtendedDecimal object.

<b>Returns:</b>

An ExtendedRational object.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>ef</i>
 is null.

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

Returns whether thi object is a not-a-number value.

<b>Returns:</b>

True if thi object is a not-a-number value; otherwise, false.

### IsInfinity

    public bool IsInfinity();

Gets a value indicating whether this object's value is infinity.

<b>Returns:</b>

True if this object's value is infinity; otherwise, false.

### IsQuietNaN

    public bool IsQuietNaN();

Returns whether this object is a quiet not-a-number value.

<b>Returns:</b>

True if this object is a quiet not-a-number value; otherwise, false.

### IsSignalingNaN

    public bool IsSignalingNaN();

Returns whether this object is a signaling not-a-number value (which causes an error if the value is passed to any arithmetic operation in this class).

<b>Returns:</b>

True if this object is a signaling not-a-number value (which causes an error if the value is passed to any arithmetic operation in this class); otherwise, false.

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

Adds two rational numbers.

<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedRational object. (2).

<b>Returns:</b>

The sum of the two numbers. Returns NaN if either operand is NaN.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>otherValue</i>
 is null.

### Subtract

    public PeterO.ExtendedRational Subtract(
        PeterO.ExtendedRational otherValue);

Subtracts an ExtendedRational object from this instance.

<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedRational object.

<b>Returns:</b>

The difference of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>otherValue</i>
 is null.

### Multiply

    public PeterO.ExtendedRational Multiply(
        PeterO.ExtendedRational otherValue);

Multiplies this instance by the value of an ExtendedRational object.

<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedRational object.

<b>Returns:</b>

The product of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>otherValue</i>
 is null.

### Divide

    public PeterO.ExtendedRational Divide(
        PeterO.ExtendedRational otherValue);

Divides this instance by the value of an ExtendedRational object.

<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedRational object.

<b>Returns:</b>

The quotient of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>otherValue</i>
 is null.

### Remainder

    public PeterO.ExtendedRational Remainder(
        PeterO.ExtendedRational otherValue);

Finds the remainder that results when this instance is divided by the value of a ExtendedRational object.

<b>Parameters:</b>

 * <i>otherValue</i>: An ExtendedRational object.

<b>Returns:</b>

The remainder of the two objects.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>otherValue</i>
 is null.

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

Gets the sign of this rational number.

<b>Returns:</b>

Zero if this value is zero or negative zero; -1 if this value is less than 0; and 1 if this value is greater than 0.

### IsNegative

    public bool IsNegative { get; }

Gets a value indicating whether this object's value is negative (including negative zero).

<b>Returns:</b>

True if this object's value is negative; otherwise, false.


