## PeterO.Cbor.CBORNumber

    public sealed class CBORNumber :
        System.IComparable

An instance of a number that CBOR or certain CBOR tags can represent. For this purpose, infinities and not-a-number or NaN values are considered numbers. Currently, this class can store one of the following kinds of numbers: 64-bit signed integers or binary floating-point numbers; or arbitrary-precision integers, decimal numbers, binary numbers, or rational numbers.

### Member Summary
* <code>[Abs()](#Abs)</code> - Returns the absolute value of this CBOR number.
* <code>[Add(PeterO.Cbor.CBORNumber)](#Add_PeterO_Cbor_CBORNumber)</code> - Returns the sum of this number and another number.
* <code>[CanFitInDouble()](#CanFitInDouble)</code> - Returns whether this object's value can be converted to a 64-bit floating point number without its value being rounded to another numerical value.
* <code>[CanFitInInt32()](#CanFitInInt32)</code> - Returns whether this object's numerical value is an integer, is -(2^31) or greater, and is less than 2^31.
* <code>[CanFitInInt64()](#CanFitInInt64)</code> - Returns whether this object's numerical value is an integer, is -(2^63) or greater, and is less than 2^63.
* <code>[CanFitInSingle()](#CanFitInSingle)</code> - Returns whether this object's value can be converted to a 32-bit floating point number without its value being rounded to another numerical value.
* <code>[CanFitInUInt64()](#CanFitInUInt64)</code> - Returns whether this object's numerical value is an integer, is 0 or greater, and is less than 2^64.
* <code>[CanTruncatedIntFitInInt32()](#CanTruncatedIntFitInInt32)</code> - Returns whether this object's value, converted to an integer by discarding its fractional part, would be -(2^31) or greater, and less than 2^31.
* <code>[CanTruncatedIntFitInInt64()](#CanTruncatedIntFitInInt64)</code> - Returns whether this object's value, converted to an integer by discarding its fractional part, would be -(2^63) or greater, and less than 2^63.
* <code>[CanTruncatedIntFitInUInt64()](#CanTruncatedIntFitInUInt64)</code> - Returns whether this object's value, converted to an integer by discarding its fractional part, would be 0 or greater, and less than 2^64.
* <code>[CompareTo(int)](#CompareTo_int)</code> - Compares this CBOR number with a 32-bit signed integer.
* <code>[CompareTo(long)](#CompareTo_long)</code> - Compares this CBOR number with a 64-bit signed integer.
* <code>[CompareTo(PeterO.Cbor.CBORNumber)](#CompareTo_PeterO_Cbor_CBORNumber)</code> - Compares this CBOR number with another.
* <code>[Divide(PeterO.Cbor.CBORNumber)](#Divide_PeterO_Cbor_CBORNumber)</code> - Returns the quotient of this number and another number.
* <code>[FromByte(byte)](#FromByte_byte)</code> - Converts a byte (from 0 to 255) to an arbitrary-precision decimal number.
* <code>[FromCBORObject(PeterO.Cbor.CBORObject)](#FromCBORObject_PeterO_Cbor_CBORObject)</code> - Creates a CBOR number object from a CBOR object representing a number (that is, one for which the IsNumber property in.
* <code>[FromInt16(short)](#FromInt16_short)</code> - Converts a 16-bit signed integer to an arbitrary-precision decimal number.
* <code>[IsFinite()](#IsFinite)</code> - Gets a value indicating whether this CBOR object represents a finite number.
* <code>[IsInfinity()](#IsInfinity)</code> - Gets a value indicating whether this object represents infinity.
* <code>[IsInteger()](#IsInteger)</code> - Gets a value indicating whether this object represents an integer number, that is, a number without a fractional part.
* <code>[IsNaN()](#IsNaN)</code> - Gets a value indicating whether this object represents a not-a-number value.
* <code>[IsNegative()](#IsNegative)</code> - Gets a value indicating whether this object is a negative number.
* <code>[IsNegativeInfinity()](#IsNegativeInfinity)</code> - Gets a value indicating whether this object represents negative infinity.
* <code>[IsPositiveInfinity()](#IsPositiveInfinity)</code> - Gets a value indicating whether this object represents positive infinity.
* <code>[IsZero()](#IsZero)</code> - Gets a value indicating whether this object's value equals 0.
* <code>[Kind](#Kind)</code> - Gets the underlying form of this CBOR number object.
* <code>[Multiply(PeterO.Cbor.CBORNumber)](#Multiply_PeterO_Cbor_CBORNumber)</code> - Returns a CBOR number expressing the product of this number and the given number.
* <code>[Negate()](#Negate)</code> - Returns a CBOR number with the same value as this one but with the sign reversed.
* <code>[bool operator &gt;(PeterO.Cbor.CBORNumber, PeterO.Cbor.CBORNumber)](#op_GreaterThan)</code> - Returns whether one object's value is greater than another's.
* <code>[bool operator &gt;=(PeterO.Cbor.CBORNumber, PeterO.Cbor.CBORNumber)](#op_GreaterThanOrEqual)</code> - Returns whether one object's value is at least another's.
* <code>[bool operator &lt;(PeterO.Cbor.CBORNumber, PeterO.Cbor.CBORNumber)](#op_LessThan)</code> - Returns whether one object's value is less than another's.
* <code>[bool operator &lt;=(PeterO.Cbor.CBORNumber, PeterO.Cbor.CBORNumber)](#op_LessThanOrEqual)</code> - Returns whether one object's value is up to another's.
* <code>[Remainder(PeterO.Cbor.CBORNumber)](#Remainder_PeterO_Cbor_CBORNumber)</code> - Returns the remainder when this number is divided by another number.
* <code>[Sign](#Sign)</code> - Gets this value's sign: -1 if nonzero and negative; 1 if nonzero and positive; 0 if zero.
* <code>[Subtract(PeterO.Cbor.CBORNumber)](#Subtract_PeterO_Cbor_CBORNumber)</code> - Returns a number that expresses this number minus another.
* <code>[ToByteChecked()](#ToByteChecked)</code> - Converts this number's value to a byte (from 0 to 255) if it can fit in a byte (from 0 to 255) after converting it to an integer by discarding its fractional part.
* <code>[ToByteIfExact()](#ToByteIfExact)</code> - Converts this number's value to a byte (from 0 to 255) if it can fit in a byte (from 0 to 255) without rounding to a different numerical value.
* <code>[ToByteUnchecked()](#ToByteUnchecked)</code> - Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as a byte (from 0 to 255).
* <code>[ToCBORObject()](#ToCBORObject)</code> - Converts this object's value to a CBOR object.
* <code>[ToDecimal()](#ToDecimal)</code> - Converts this number's value to a CLR decimal.
* <code>[ToEDecimal()](#ToEDecimal)</code> - Converts this object to a decimal number.
* <code>[ToEFloat()](#ToEFloat)</code> - Converts this object to an arbitrary-precision binary floating point number.
* <code>[ToEInteger()](#ToEInteger)</code> - Converts this object to an arbitrary-precision integer.
* <code>[ToEIntegerIfExact()](#ToEIntegerIfExact)</code> - Converts this object to an arbitrary-precision integer if its value is an integer.
* <code>[ToERational()](#ToERational)</code> - Converts this object to a rational number.
* <code>[ToInt16Checked()](#ToInt16Checked)</code> - Converts this number's value to a 16-bit signed integer if it can fit in a 16-bit signed integer after converting it to an integer by discarding its fractional part.
* <code>[ToInt16IfExact()](#ToInt16IfExact)</code> - Converts this number's value to a 16-bit signed integer if it can fit in a 16-bit signed integer without rounding to a different numerical value.
* <code>[ToInt16Unchecked()](#ToInt16Unchecked)</code> - Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as a 16-bit signed integer.
* <code>[ToInt32Checked()](#ToInt32Checked)</code> - Converts this number's value to a 32-bit signed integer if it can fit in a 32-bit signed integer after converting it to an integer by discarding its fractional part.
* <code>[ToInt32IfExact()](#ToInt32IfExact)</code> - Converts this number's value to a 32-bit signed integer if it can fit in a 32-bit signed integer without rounding to a different numerical value.
* <code>[ToInt32Unchecked()](#ToInt32Unchecked)</code> - Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as a 32-bit signed integer.
* <code>[ToInt64Checked()](#ToInt64Checked)</code> - Converts this number's value to a 64-bit signed integer if it can fit in a 64-bit signed integer after converting it to an integer by discarding its fractional part.
* <code>[ToInt64IfExact()](#ToInt64IfExact)</code> - Converts this number's value to a 64-bit signed integer if it can fit in a 64-bit signed integer without rounding to a different numerical value.
* <code>[ToInt64Unchecked()](#ToInt64Unchecked)</code> - Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as a 64-bit signed integer.
* <code>[ToSByteChecked()](#ToSByteChecked)</code> - Converts this number's value to an 8-bit signed integer if it can fit in an 8-bit signed integer after converting it to an integer by discarding its fractional part.
* <code>[ToSByteIfExact()](#ToSByteIfExact)</code> - Converts this number's value to an 8-bit signed integer if it can fit in an 8-bit signed integer without rounding to a different numerical value.
* <code>[ToSByteUnchecked()](#ToSByteUnchecked)</code> - Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as an 8-bit signed integer.
* <code>[ToString()](#ToString)</code> - Returns the value of this object in text form.
* <code>[ToUInt16Checked()](#ToUInt16Checked)</code> - Converts this number's value to a 16-bit unsigned integer if it can fit in a 16-bit unsigned integer after converting it to an integer by discarding its fractional part.
* <code>[ToUInt16IfExact()](#ToUInt16IfExact)</code> - Converts this number's value to a 16-bit unsigned integer if it can fit in a 16-bit unsigned integer without rounding to a different numerical value.
* <code>[ToUInt16Unchecked()](#ToUInt16Unchecked)</code> - Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as a 16-bit unsigned integer.
* <code>[ToUInt32Checked()](#ToUInt32Checked)</code> - Converts this number's value to a 32-bit signed integer if it can fit in a 32-bit signed integer after converting it to an integer by discarding its fractional part.
* <code>[ToUInt32IfExact()](#ToUInt32IfExact)</code> - Converts this number's value to a 32-bit signed integer if it can fit in a 32-bit signed integer without rounding to a different numerical value.
* <code>[ToUInt32Unchecked()](#ToUInt32Unchecked)</code> - Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as a 32-bit signed integer.
* <code>[ToUInt64Checked()](#ToUInt64Checked)</code> - Converts this number's value to a 64-bit unsigned integer if it can fit in a 64-bit unsigned integer after converting it to an integer by discarding its fractional part.
* <code>[ToUInt64IfExact()](#ToUInt64IfExact)</code> - Converts this number's value to a 64-bit unsigned integer if it can fit in a 64-bit unsigned integer without rounding to a different numerical value.
* <code>[ToUInt64Unchecked()](#ToUInt64Unchecked)</code> - Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as a 64-bit unsigned integer.

<a id="Kind"></a>
### Kind

    public PeterO.Cbor.CBORNumber.NumberKind Kind { get; }

Gets the underlying form of this CBOR number object.

<b>Returns:</b>

The underlying form of this CBOR number object.

<a id="Sign"></a>
### Sign

    public int Sign { get; }

Gets this value's sign: -1 if nonzero and negative; 1 if nonzero and positive; 0 if zero. Not-a-number (NaN) values are positive or negative depending on what sign is stored in their underlying forms.

<b>Returns:</b>

This value's sign.

<a id="Abs"></a>
### Abs

    public PeterO.Cbor.CBORNumber Abs();

Returns the absolute value of this CBOR number.

<b>Return Value:</b>

This object's absolute value without its negative sign.

<a id="Add_PeterO_Cbor_CBORNumber"></a>
### Add

    public PeterO.Cbor.CBORNumber Add(
        PeterO.Cbor.CBORNumber b);

Returns the sum of this number and another number.

<b>Parameters:</b>

 * <i>b</i>: The number to add with this one.

<b>Return Value:</b>

The sum of this number and another number.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>b</i>
 is null.

 * System.OutOfMemoryException:
The exact result of the operation might be too big to fit in memory (or might require more than 2 gigabytes of memory to store).

<a id="CanFitInDouble"></a>
### CanFitInDouble

    public bool CanFitInDouble();

Returns whether this object's value can be converted to a 64-bit floating point number without its value being rounded to another numerical value.

<b>Return Value:</b>

 `true`  if this object's value can be converted to a 64-bit floating point number without its value being rounded to another numerical value, or if this is a not-a-number value, even if the value's diagnostic information can't fit in a 64-bit floating point number; otherwise,  `false` .

<a id="CanFitInInt32"></a>
### CanFitInInt32

    public bool CanFitInInt32();

Returns whether this object's numerical value is an integer, is -(2^31) or greater, and is less than 2^31.

<b>Return Value:</b>

 `true`  if this object's numerical value is an integer, is -(2^31) or greater, and is less than 2^31; otherwise,  `false` .

<a id="CanFitInInt64"></a>
### CanFitInInt64

    public bool CanFitInInt64();

Returns whether this object's numerical value is an integer, is -(2^63) or greater, and is less than 2^63.

<b>Return Value:</b>

 `true`  if this object's numerical value is an integer, is -(2^63) or greater, and is less than 2^63; otherwise,  `false` .

<a id="CanFitInSingle"></a>
### CanFitInSingle

    public bool CanFitInSingle();

Returns whether this object's value can be converted to a 32-bit floating point number without its value being rounded to another numerical value.

<b>Return Value:</b>

 `true`  if this object's value can be converted to a 32-bit floating point number without its value being rounded to another numerical value, or if this is a not-a-number value, even if the value's diagnostic information can' t fit in a 32-bit floating point number; otherwise,  `false` .

<a id="CanFitInUInt64"></a>
### CanFitInUInt64

    public bool CanFitInUInt64();

Returns whether this object's numerical value is an integer, is 0 or greater, and is less than 2^64.

<b>Return Value:</b>

 `true`  if this object's numerical value is an integer, is 0 or greater, and is less than 2^64; otherwise,  `false` .

<a id="CanTruncatedIntFitInInt32"></a>
### CanTruncatedIntFitInInt32

    public bool CanTruncatedIntFitInInt32();

Returns whether this object's value, converted to an integer by discarding its fractional part, would be -(2^31) or greater, and less than 2^31.

<b>Return Value:</b>

 `true`  if this object's value, converted to an integer by discarding its fractional part, would be -(2^31) or greater, and less than 2^31; otherwise,  `false` .

<a id="CanTruncatedIntFitInInt64"></a>
### CanTruncatedIntFitInInt64

    public bool CanTruncatedIntFitInInt64();

Returns whether this object's value, converted to an integer by discarding its fractional part, would be -(2^63) or greater, and less than 2^63.

<b>Return Value:</b>

 `true`  if this object's value, converted to an integer by discarding its fractional part, would be -(2^63) or greater, and less than 2^63; otherwise,  `false` .

<a id="CanTruncatedIntFitInUInt64"></a>
### CanTruncatedIntFitInUInt64

    public bool CanTruncatedIntFitInUInt64();

Returns whether this object's value, converted to an integer by discarding its fractional part, would be 0 or greater, and less than 2^64.

<b>Return Value:</b>

 `true`  if this object's value, converted to an integer by discarding its fractional part, would be 0 or greater, and less than 2^64; otherwise,  `false` .

<a id="CompareTo_int"></a>
### CompareTo

    public int CompareTo(
        int other);

Compares this CBOR number with a 32-bit signed integer. In this implementation, the two numbers' mathematical values are compared. Here, NaN (not-a-number) is considered greater than any number.

<b>Parameters:</b>

 * <i>other</i>: A value to compare with. Can be null.

<b>Return Value:</b>

A negative number, if this value is less than the other object; or 0, if both values are equal; or a positive number, if this value is less than the other object or if the other object is null. This implementation returns a positive number if  <i>other</i>
 is null, to conform to the.NET definition of CompareTo. This is the case even in the Java version of this library, for consistency's sake, even though implementations of  `Comparable.compareTo()`  in Java ought to throw an exception if they receive a null argument rather than treating null as less or greater than any object.

.

<a id="CompareTo_long"></a>
### CompareTo

    public int CompareTo(
        long other);

Compares this CBOR number with a 64-bit signed integer. In this implementation, the two numbers' mathematical values are compared. Here, NaN (not-a-number) is considered greater than any number.

<b>Parameters:</b>

 * <i>other</i>: A value to compare with. Can be null.

<b>Return Value:</b>

A negative number, if this value is less than the other object; or 0, if both values are equal; or a positive number, if this value is less than the other object or if the other object is null. This implementation returns a positive number if  <i>other</i>
 is null, to conform to the.NET definition of CompareTo. This is the case even in the Java version of this library, for consistency's sake, even though implementations of  `Comparable.compareTo()`  in Java ought to throw an exception if they receive a null argument rather than treating null as less or greater than any object.

.

<a id="CompareTo_PeterO_Cbor_CBORNumber"></a>
### CompareTo

    public sealed int CompareTo(
        PeterO.Cbor.CBORNumber other);

Compares this CBOR number with another. In this implementation, the two numbers' mathematical values are compared. Here, NaN (not-a-number) is considered greater than any number.

<b>Parameters:</b>

 * <i>other</i>: A value to compare with. Can be null.

<b>Return Value:</b>

A negative number, if this value is less than the other object; or 0, if both values are equal; or a positive number, if this value is less than the other object or if the other object is null. This implementation returns a positive number if  <i>other</i>
 is null, to conform to the.NET definition of CompareTo. This is the case even in the Java version of this library, for consistency's sake, even though implementations of  `Comparable.compareTo()`  in Java ought to throw an exception if they receive a null argument rather than treating null as less or greater than any object.

.

<a id="Divide_PeterO_Cbor_CBORNumber"></a>
### Divide

    public PeterO.Cbor.CBORNumber Divide(
        PeterO.Cbor.CBORNumber b);

Returns the quotient of this number and another number.

<b>Parameters:</b>

 * <i>b</i>: The right-hand side (divisor) to the division operation.

<b>Return Value:</b>

The quotient of this number and another one.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>b</i>
 is null.

 * System.OutOfMemoryException:
The exact result of the operation might be too big to fit in memory (or might require more than 2 gigabytes of memory to store).

<a id="FromByte_byte"></a>
### FromByte

    public static PeterO.Cbor.CBORNumber FromByte(
        byte inputByte);

Converts a byte (from 0 to 255) to an arbitrary-precision decimal number.

<b>Parameters:</b>

 * <i>inputByte</i>: The number to convert as a byte (from 0 to 255).

<b>Return Value:</b>

This number's value as an arbitrary-precision decimal number.

<a id="FromCBORObject_PeterO_Cbor_CBORObject"></a>
### FromCBORObject

    public static PeterO.Cbor.CBORNumber FromCBORObject(
        PeterO.Cbor.CBORObject o);

Creates a CBOR number object from a CBOR object representing a number (that is, one for which the IsNumber property in.NET or the isNumber() method in Java returns true).

<b>Parameters:</b>

 * <i>o</i>: The parameter is a CBOR object representing a number.

<b>Return Value:</b>

A CBOR number object, or null if the given CBOR object is null or does not represent a number.

<a id="FromInt16_short"></a>
### FromInt16

    public static PeterO.Cbor.CBORNumber FromInt16(
        short inputInt16);

Converts a 16-bit signed integer to an arbitrary-precision decimal number.

<b>Parameters:</b>

 * <i>inputInt16</i>: The number to convert as a 16-bit signed integer.

<b>Return Value:</b>

This number's value as an arbitrary-precision decimal number.

<a id="IsFinite"></a>
### IsFinite

    public bool IsFinite();

Gets a value indicating whether this CBOR object represents a finite number.

<b>Return Value:</b>

 `true`  if this CBOR object represents a finite number; otherwise,  `false` .

<a id="IsInfinity"></a>
### IsInfinity

    public bool IsInfinity();

Gets a value indicating whether this object represents infinity.

<b>Return Value:</b>

 `true`  if this object represents infinity; otherwise,  `false` .

<a id="IsInteger"></a>
### IsInteger

    public bool IsInteger();

Gets a value indicating whether this object represents an integer number, that is, a number without a fractional part. Infinity and not-a-number are not considered integers.

<b>Return Value:</b>

 `true`  if this object represents an integer number, that is, a number without a fractional part; otherwise,  `false` .

<a id="IsNaN"></a>
### IsNaN

    public bool IsNaN();

Gets a value indicating whether this object represents a not-a-number value.

<b>Return Value:</b>

 `true`  if this object represents a not-a-number value; otherwise,  `false` .

<a id="IsNegative"></a>
### IsNegative

    public bool IsNegative();

Gets a value indicating whether this object is a negative number.

<b>Return Value:</b>

 `true`  if this object is a negative number; otherwise,  `false` .

<a id="IsNegativeInfinity"></a>
### IsNegativeInfinity

    public bool IsNegativeInfinity();

Gets a value indicating whether this object represents negative infinity.

<b>Return Value:</b>

 `true`  if this object represents negative infinity; otherwise,  `false` .

<a id="IsPositiveInfinity"></a>
### IsPositiveInfinity

    public bool IsPositiveInfinity();

Gets a value indicating whether this object represents positive infinity.

<b>Return Value:</b>

 `true`  if this object represents positive infinity; otherwise,  `false` .

<a id="IsZero"></a>
### IsZero

    public bool IsZero();

Gets a value indicating whether this object's value equals 0.

<b>Return Value:</b>

 `true`  if this object's value equals 0; otherwise,  `false` .

<a id="Multiply_PeterO_Cbor_CBORNumber"></a>
### Multiply

    public PeterO.Cbor.CBORNumber Multiply(
        PeterO.Cbor.CBORNumber b);

Returns a CBOR number expressing the product of this number and the given number.

<b>Parameters:</b>

 * <i>b</i>: The second operand to the multiplication operation.

<b>Return Value:</b>

A number expressing the product of this number and the given number.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>b</i>
 is null.

 * System.OutOfMemoryException:
The exact result of the operation might be too big to fit in memory (or might require more than 2 gigabytes of memory to store).

<a id="Negate"></a>
### Negate

    public PeterO.Cbor.CBORNumber Negate();

Returns a CBOR number with the same value as this one but with the sign reversed.

<b>Return Value:</b>

A CBOR number with the same value as this one but with the sign reversed.

<a id="op_GreaterThan"></a>
### Operator `>`

    public static bool operator >(
        PeterO.Cbor.CBORNumber a,
        PeterO.Cbor.CBORNumber b);

Returns whether one object's value is greater than another's.

<b>Parameters:</b>

 * <i>a</i>: The left-hand side of the comparison.

 * <i>b</i>: The right-hand side of the comparison.

<b>Return Value:</b>

 `true`  if one object's value is greater than another's; otherwise,  `false` .

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>a</i>
 is null.

<a id="op_GreaterThanOrEqual"></a>
### Operator `>=`

    public static bool operator >=(
        PeterO.Cbor.CBORNumber a,
        PeterO.Cbor.CBORNumber b);

Returns whether one object's value is at least another's.

<b>Parameters:</b>

 * <i>a</i>: The left-hand side of the comparison.

 * <i>b</i>: The right-hand side of the comparison.

<b>Return Value:</b>

 `true`  if one object's value is at least another's; otherwise,  `false` .

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>a</i>
 is null.

<a id="op_LessThan"></a>
### Operator `<`

    public static bool operator <(
        PeterO.Cbor.CBORNumber a,
        PeterO.Cbor.CBORNumber b);

Returns whether one object's value is less than another's.

<b>Parameters:</b>

 * <i>a</i>: The left-hand side of the comparison.

 * <i>b</i>: The right-hand side of the comparison.

<b>Return Value:</b>

 `true`  if the first object's value is less than the other's; otherwise,  `false` .

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>a</i>
 is null.

<a id="op_LessThanOrEqual"></a>
### Operator `<=`

    public static bool operator <=(
        PeterO.Cbor.CBORNumber a,
        PeterO.Cbor.CBORNumber b);

Returns whether one object's value is up to another's.

<b>Parameters:</b>

 * <i>a</i>: The left-hand side of the comparison.

 * <i>b</i>: The right-hand side of the comparison.

<b>Return Value:</b>

 `true`  if one object's value is up to another's; otherwise,  `false` .

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>a</i>
 is null.

<a id="Remainder_PeterO_Cbor_CBORNumber"></a>
### Remainder

    public PeterO.Cbor.CBORNumber Remainder(
        PeterO.Cbor.CBORNumber b);

Returns the remainder when this number is divided by another number.

<b>Parameters:</b>

 * <i>b</i>: The right-hand side (dividend) of the remainder operation.

<b>Return Value:</b>

The remainder when this number is divided by the other number.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>b</i>
 is null.

 * System.OutOfMemoryException:
The exact result of the operation might be too big to fit in memory (or might require more than 2 gigabytes of memory to store).

<a id="Subtract_PeterO_Cbor_CBORNumber"></a>
### Subtract

    public PeterO.Cbor.CBORNumber Subtract(
        PeterO.Cbor.CBORNumber b);

Returns a number that expresses this number minus another.

<b>Parameters:</b>

 * <i>b</i>: The second operand to the subtraction.

<b>Return Value:</b>

A CBOR number that expresses this number minus the given number.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>b</i>
 is null.

 * System.OutOfMemoryException:
The exact result of the operation might be too big to fit in memory (or might require more than 2 gigabytes of memory to store).

<a id="ToByteChecked"></a>
### ToByteChecked

    public byte ToByteChecked();

Converts this number's value to a byte (from 0 to 255) if it can fit in a byte (from 0 to 255) after converting it to an integer by discarding its fractional part.

<b>Return Value:</b>

This number's value, truncated to a byte (from 0 to 255).

<b>Exceptions:</b>

 * System.OverflowException:
This value is infinity or not-a-number, or the number, once converted to an integer by discarding its fractional part, is less than 0 or greater than 255.

<a id="ToByteIfExact"></a>
### ToByteIfExact

    public byte ToByteIfExact();

Converts this number's value to a byte (from 0 to 255) if it can fit in a byte (from 0 to 255) without rounding to a different numerical value.

<b>Return Value:</b>

This number's value as a byte (from 0 to 255).

<b>Exceptions:</b>

 * System.ArithmeticException:
This value is infinity or not-a-number, is not an exact integer, or is less than 0 or greater than 255.

<a id="ToByteUnchecked"></a>
### ToByteUnchecked

    public byte ToByteUnchecked();

Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as a byte (from 0 to 255).

<b>Return Value:</b>

This number, converted to a byte (from 0 to 255). Returns 0 if this value is infinity or not-a-number.

<a id="ToCBORObject"></a>
### ToCBORObject

    public PeterO.Cbor.CBORObject ToCBORObject();

Converts this object's value to a CBOR object.

<b>Return Value:</b>

A CBOR object that stores this object's value.

<a id="ToDecimal"></a>
### ToDecimal

    public decimal ToDecimal();

Converts this number's value to a CLR decimal.

<b>Return Value:</b>

This number's value, converted to a decimal as though by  `(decimal)this.ToEDecimal()` .

<b>Exceptions:</b>

 * System.OverflowException:
This value is infinity or not-a-number.

<a id="ToEDecimal"></a>
### ToEDecimal

    public PeterO.Numbers.EDecimal ToEDecimal();

Converts this object to a decimal number.

<b>Return Value:</b>

A decimal number for this object's value.

<a id="ToEFloat"></a>
### ToEFloat

    public PeterO.Numbers.EFloat ToEFloat();

Converts this object to an arbitrary-precision binary floating point number. See the ToObject overload taking a type for more information.

<b>Return Value:</b>

An arbitrary-precision binary floating-point number for this object's value.

<a id="ToEInteger"></a>
### ToEInteger

    public PeterO.Numbers.EInteger ToEInteger();

Converts this object to an arbitrary-precision integer. See the ToObject overload taking a type for more information.

<b>Return Value:</b>

The closest arbitrary-precision integer to this object.

<b>Exceptions:</b>

 * System.ArithmeticException:
This value is infinity or not-a-number.

<a id="ToEIntegerIfExact"></a>
### ToEIntegerIfExact

    public PeterO.Numbers.EInteger ToEIntegerIfExact();

Converts this object to an arbitrary-precision integer if its value is an integer.

<b>Return Value:</b>

The arbitrary-precision integer given by object.

<b>Exceptions:</b>

 * System.ArithmeticException:
This value is infinity or not-a-number or is not an exact integer.

<a id="ToERational"></a>
### ToERational

    public PeterO.Numbers.ERational ToERational();

Converts this object to a rational number. See the ToObject overload taking a type for more information.

<b>Return Value:</b>

A rational number for this object's value.

<a id="ToInt16Checked"></a>
### ToInt16Checked

    public short ToInt16Checked();

Converts this number's value to a 16-bit signed integer if it can fit in a 16-bit signed integer after converting it to an integer by discarding its fractional part.

<b>Return Value:</b>

This number's value, truncated to a 16-bit signed integer.

<b>Exceptions:</b>

 * System.OverflowException:
This value is infinity or not-a-number, or the number, once converted to an integer by discarding its fractional part, is less than -32768 or greater than 32767.

<a id="ToInt16IfExact"></a>
### ToInt16IfExact

    public short ToInt16IfExact();

Converts this number's value to a 16-bit signed integer if it can fit in a 16-bit signed integer without rounding to a different numerical value.

<b>Return Value:</b>

This number's value as a 16-bit signed integer.

<b>Exceptions:</b>

 * System.ArithmeticException:
This value is infinity or not-a-number, is not an exact integer, or is less than -32768 or greater than 32767.

<a id="ToInt16Unchecked"></a>
### ToInt16Unchecked

    public short ToInt16Unchecked();

Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as a 16-bit signed integer.

<b>Return Value:</b>

This number, converted to a 16-bit signed integer. Returns 0 if this value is infinity or not-a-number.

<a id="ToInt32Checked"></a>
### ToInt32Checked

    public int ToInt32Checked();

Converts this number's value to a 32-bit signed integer if it can fit in a 32-bit signed integer after converting it to an integer by discarding its fractional part.

<b>Return Value:</b>

This number's value, truncated to a 32-bit signed integer.

<b>Exceptions:</b>

 * System.OverflowException:
This value is infinity or not-a-number, or the number, once converted to an integer by discarding its fractional part, is less than -2147483648 or greater than 2147483647.

<a id="ToInt32IfExact"></a>
### ToInt32IfExact

    public int ToInt32IfExact();

Converts this number's value to a 32-bit signed integer if it can fit in a 32-bit signed integer without rounding to a different numerical value.

<b>Return Value:</b>

This number's value as a 32-bit signed integer.

<b>Exceptions:</b>

 * System.ArithmeticException:
This value is infinity or not-a-number, is not an exact integer, or is less than -2147483648 or greater than 2147483647.

<a id="ToInt32Unchecked"></a>
### ToInt32Unchecked

    public int ToInt32Unchecked();

Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as a 32-bit signed integer.

<b>Return Value:</b>

This number, converted to a 32-bit signed integer. Returns 0 if this value is infinity or not-a-number.

<a id="ToInt64Checked"></a>
### ToInt64Checked

    public long ToInt64Checked();

Converts this number's value to a 64-bit signed integer if it can fit in a 64-bit signed integer after converting it to an integer by discarding its fractional part.

<b>Return Value:</b>

This number's value, truncated to a 64-bit signed integer.

<b>Exceptions:</b>

 * System.OverflowException:
This value is infinity or not-a-number, or the number, once converted to an integer by discarding its fractional part, is less than -9223372036854775808 or greater than 9223372036854775807.

<a id="ToInt64IfExact"></a>
### ToInt64IfExact

    public long ToInt64IfExact();

Converts this number's value to a 64-bit signed integer if it can fit in a 64-bit signed integer without rounding to a different numerical value.

<b>Return Value:</b>

This number's value as a 64-bit signed integer.

<b>Exceptions:</b>

 * System.ArithmeticException:
This value is infinity or not-a-number, is not an exact integer, or is less than -9223372036854775808 or greater than 9223372036854775807.

<a id="ToInt64Unchecked"></a>
### ToInt64Unchecked

    public long ToInt64Unchecked();

Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as a 64-bit signed integer.

<b>Return Value:</b>

This number, converted to a 64-bit signed integer. Returns 0 if this value is infinity or not-a-number.

<a id="ToSByteChecked"></a>
### ToSByteChecked

    public sbyte ToSByteChecked();

<b>This API is not CLS-compliant.</b>

Converts this number's value to an 8-bit signed integer if it can fit in an 8-bit signed integer after converting it to an integer by discarding its fractional part.

<b>Return Value:</b>

This number's value, truncated to an 8-bit signed integer.

<b>Exceptions:</b>

 * System.OverflowException:
This value is infinity or not-a-number, or the number, once converted to an integer by discarding its fractional part, is less than -128 or greater than 127.

<a id="ToSByteIfExact"></a>
### ToSByteIfExact

    public sbyte ToSByteIfExact();

<b>This API is not CLS-compliant.</b>

Converts this number's value to an 8-bit signed integer if it can fit in an 8-bit signed integer without rounding to a different numerical value.

<b>Return Value:</b>

This number's value as an 8-bit signed integer.

<b>Exceptions:</b>

 * System.ArithmeticException:
This value is infinity or not-a-number, is not an exact integer, or is less than -128 or greater than 127.

<a id="ToSByteUnchecked"></a>
### ToSByteUnchecked

    public sbyte ToSByteUnchecked();

<b>This API is not CLS-compliant.</b>

Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as an 8-bit signed integer.

<b>Return Value:</b>

This number, converted to an 8-bit signed integer. Returns 0 if this value is infinity or not-a-number.

<a id="ToString"></a>
### ToString

    public override string ToString();

Returns the value of this object in text form.

<b>Return Value:</b>

A text string representing the value of this object.

<a id="ToUInt16Checked"></a>
### ToUInt16Checked

    public ushort ToUInt16Checked();

<b>This API is not CLS-compliant.</b>

Converts this number's value to a 16-bit unsigned integer if it can fit in a 16-bit unsigned integer after converting it to an integer by discarding its fractional part.

<b>Return Value:</b>

This number's value, truncated to a 16-bit unsigned integer.

<b>Exceptions:</b>

 * System.OverflowException:
This value is infinity or not-a-number, or the number, once converted to an integer by discarding its fractional part, is less than 0 or greater than 65535.

<a id="ToUInt16IfExact"></a>
### ToUInt16IfExact

    public ushort ToUInt16IfExact();

<b>This API is not CLS-compliant.</b>

Converts this number's value to a 16-bit unsigned integer if it can fit in a 16-bit unsigned integer without rounding to a different numerical value.

<b>Return Value:</b>

This number's value as a 16-bit unsigned integer.

<b>Exceptions:</b>

 * System.ArithmeticException:
This value is infinity or not-a-number, is not an exact integer, or is less than 0 or greater than 65535.

<a id="ToUInt16Unchecked"></a>
### ToUInt16Unchecked

    public ushort ToUInt16Unchecked();

<b>This API is not CLS-compliant.</b>

Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as a 16-bit unsigned integer.

<b>Return Value:</b>

This number, converted to a 16-bit unsigned integer. Returns 0 if this value is infinity or not-a-number.

<a id="ToUInt32Checked"></a>
### ToUInt32Checked

    public uint ToUInt32Checked();

<b>This API is not CLS-compliant.</b>

Converts this number's value to a 32-bit signed integer if it can fit in a 32-bit signed integer after converting it to an integer by discarding its fractional part.

<b>Return Value:</b>

This number's value, truncated to a 32-bit signed integer.

<b>Exceptions:</b>

 * System.OverflowException:
This value is infinity or not-a-number, or the number, once converted to an integer by discarding its fractional part, is less than 0 or greater than 4294967295.

<a id="ToUInt32IfExact"></a>
### ToUInt32IfExact

    public uint ToUInt32IfExact();

<b>This API is not CLS-compliant.</b>

Converts this number's value to a 32-bit signed integer if it can fit in a 32-bit signed integer without rounding to a different numerical value.

<b>Return Value:</b>

This number's value as a 32-bit signed integer.

<b>Exceptions:</b>

 * System.ArithmeticException:
This value is infinity or not-a-number, is not an exact integer, or is less than 0 or greater than 4294967295.

<a id="ToUInt32Unchecked"></a>
### ToUInt32Unchecked

    public uint ToUInt32Unchecked();

<b>This API is not CLS-compliant.</b>

Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as a 32-bit signed integer.

<b>Return Value:</b>

This number, converted to a 32-bit signed integer. Returns 0 if this value is infinity or not-a-number.

<a id="ToUInt64Checked"></a>
### ToUInt64Checked

    public ulong ToUInt64Checked();

<b>This API is not CLS-compliant.</b>

Converts this number's value to a 64-bit unsigned integer if it can fit in a 64-bit unsigned integer after converting it to an integer by discarding its fractional part.

<b>Return Value:</b>

This number's value, truncated to a 64-bit unsigned integer.

<b>Exceptions:</b>

 * System.OverflowException:
This value is infinity or not-a-number, or the number, once converted to an integer by discarding its fractional part, is less than 0 or greater than 18446744073709551615.

<a id="ToUInt64IfExact"></a>
### ToUInt64IfExact

    public ulong ToUInt64IfExact();

<b>This API is not CLS-compliant.</b>

Converts this number's value to a 64-bit unsigned integer if it can fit in a 64-bit unsigned integer without rounding to a different numerical value.

<b>Return Value:</b>

This number's value as a 64-bit unsigned integer.

<b>Exceptions:</b>

 * System.ArithmeticException:
This value is infinity or not-a-number, is not an exact integer, or is less than 0 or greater than 18446744073709551615.

<a id="ToUInt64Unchecked"></a>
### ToUInt64Unchecked

    public ulong ToUInt64Unchecked();

<b>This API is not CLS-compliant.</b>

Converts this number's value to an integer by discarding its fractional part, and returns the least-significant bits of its two's-complement form as a 64-bit unsigned integer.

<b>Return Value:</b>

This number, converted to a 64-bit unsigned integer. Returns 0 if this value is infinity or not-a-number.
