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

Determines whether a number can have the given Exponent property under this precision context.

<b>Parameters:</b>

 * <i>exponent</i>: A BigInteger object indicating the desired exponent.

<b>Returns:</b>

True if a number can have the given Exponent property under this precision context; otherwise, false.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>exponent</i>
 is null.

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

 * <i>exponentMinSmall</i>: Desired minimum exponent (EMin).

 * <i>exponentMaxSmall</i>: Desired maximum exponent (EMax).

<b>Returns:</b>

A PrecisionContext object.

### WithBigExponentRange

    public PeterO.PrecisionContext WithBigExponentRange(
        PeterO.BigInteger exponentMin,
        PeterO.BigInteger exponentMax);

Copies this precision context and sets the copy's exponent range.

<b>Parameters:</b>

 * <i>exponentMin</i>: Desired minimum exponent (EMin).

 * <i>exponentMax</i>: Desired maximum exponent (EMax).

<b>Returns:</b>

A PrecisionContext object.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>exponentMin</i>
 is null.

### WithNoFlags

    public PeterO.PrecisionContext WithNoFlags();

Copies this PrecisionContext with HasFlags set to false and a Flags value of 0.

<b>Returns:</b>

A PrecisionContext object.

### WithPrecisionInBits

    public PeterO.PrecisionContext WithPrecisionInBits(
        bool isPrecisionBits);

Copies this PrecisionContext and sets the copy's "IsPrecisionInBits" property to the given value.

<b>Parameters:</b>

 * <i>isPrecisionBits</i>: A Boolean object.

<b>Returns:</b>

A PrecisionContext object.

### WithSimplified

    public PeterO.PrecisionContext WithSimplified(
        bool simplified);

Copies this PrecisionContext and sets the copy's "IsSimplified" property to the given value.

<b>Parameters:</b>

 * <i>simplified</i>: A Boolean object.

<b>Returns:</b>

A PrecisionContext object.

### WithAdjustExponent

    public PeterO.PrecisionContext WithAdjustExponent(
        bool adjustExponent);

Copies this PrecisionContext and sets the copy's "AdjustExponent" property to the given value.

<b>Parameters:</b>

 * <i>adjustExponent</i>: A Boolean object.

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

Creates a new PrecisionContext object initialized with unlimited precision and exponent range, and the given rounding mode.

<b>Parameters:</b>

 * <i>rounding</i>: The rounding mode for the new precision context.

<b>Returns:</b>

A PrecisionContext object.

### ForPrecisionAndRounding

    public static PeterO.PrecisionContext ForPrecisionAndRounding(
        int precision,
        PeterO.Rounding rounding);

Creates a new PrecisionContext object initialized with unlimited and exponent range, and the given rounding mode and maximum precision.

<b>Parameters:</b>

 * <i>precision</i>: Maximum number of digits (precision).

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

Precision context for the IEEE-754-2008 binary16 format, 11 bits precision. The default rounding mode is HalfEven.

### Binary32

    public static readonly PeterO.PrecisionContext Binary32;

Precision context for the IEEE-754-2008 binary32 format, 24 bits precision. The default rounding mode is HalfEven.

### Binary64

    public static readonly PeterO.PrecisionContext Binary64;

Precision context for the IEEE-754-2008 binary64 format, 53 bits precision. The default rounding mode is HalfEven.

### Binary128

    public static readonly PeterO.PrecisionContext Binary128;

Precision context for the IEEE-754-2008 binary128 format, 113 bits precision. The default rounding mode is HalfEven.

### Decimal32

    public static readonly PeterO.PrecisionContext Decimal32;

Precision context for the IEEE-754-2008 decimal32 format. The default rounding mode is HalfEven.

### Decimal64

    public static readonly PeterO.PrecisionContext Decimal64;

Precision context for the IEEE-754-2008 decimal64 format. The default rounding mode is HalfEven.

### Decimal128

    public static readonly PeterO.PrecisionContext Decimal128;

Precision context for the IEEE-754-2008 decimal128 format. The default rounding mode is HalfEven.

### Basic

    public static readonly PeterO.PrecisionContext Basic;

Basic precision context, 9 digits precision, rounding mode half-up, unlimited exponent range. The default rounding mode is HalfUp.

### CliDecimal

    public static readonly PeterO.PrecisionContext CliDecimal;

Precision context for the Common Language Infrastructure (.NET Framework) decimal format, 96 bits precision, and a valid exponent range of -28 to 0. The default rounding mode is HalfEven.

### BigDecimalJava

    public static readonly PeterO.PrecisionContext BigDecimalJava;

Precision context for Java's BigDecimal format. The default rounding mode is HalfUp.

### JavaBigDecimal

    public static readonly PeterO.PrecisionContext JavaBigDecimal;

Precision context for Java's BigDecimal format. The default rounding mode is HalfUp.

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

Gets the lowest exponent possible when a converted number is expressed in scientific notation with one digit before the decimal point. For example, with a precision of 3 and an EMin of -100, the next value that comes after 0 is 0.001E-100. (If AdjustExponent is false, this property specifies the lowest possible Exponent property instead.) If HasExponentRange is false, this value will be 0.

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

### IsPrecisionInBits

    public bool IsPrecisionInBits { get; }

Gets a value indicating whether this context's Precision property is in bits, rather than digits. The default is false.

<b>Returns:</b>

True if this context's Precision property is in bits, rather than digits; otherwise, false.. The default is false.

### IsSimplified

    public bool IsSimplified { get; }

Gets a value indicating whether to use a "simplified" arithmetic. In the simplified arithmetic, infinity, not-a-number, and subnormal numbers are not allowed, and negative zero is treated the same as positive zero. For further details, see  `http://speleotrove.com/decimal/dax3274.html` 

<b>Returns:</b>

True if a "simplified" arithmetic will be used; otherwise, false.

### AdjustExponent

    public bool AdjustExponent { get; }

Gets a value indicating whether the EMax and EMin properties refer to the number's Exponent property adjusted to the number's precision, or just the number's Exponent property. The default value is true, meaning that EMax and EMin refer to the adjusted exponent. Setting this value to false (using WithAdjustExponent) is useful for modeling floating point representations with an integer mantissa and an integer exponent, such as Java's BigDecimal.

<b>Returns:</b>

True if the EMax and EMin properties refer to the number's Exponent property adjusted to the number's precision, or false if they refer to just the number's Exponent property.


