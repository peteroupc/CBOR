## PeterO.Cbor.CBORNumber

    public sealed class CBORNumber :
        System.IComparable

 An instance of a number that CBOR or certain CBOR tags can represent. For this purpose, infinities and not-a-number or NaN values are considered numbers. Currently, this class can store one of the following kinds of numbers: 64-bit signed integers or binary floating-point numbers; or arbitrary-precision integers, decimal numbers, binary numbers, or rational numbers.

### Member Summary
* <code>[Add(PeterO.Cbor.CBORNumber)](#Add_PeterO_Cbor_CBORNumber)</code> - Returns the sum of this number and another number.
* <code>[CompareTo(PeterO.Cbor.CBORNumber)](#CompareTo_PeterO_Cbor_CBORNumber)</code> - Compares two CBOR numbers.
* <code>[Divide(PeterO.Cbor.CBORNumber)](#Divide_PeterO_Cbor_CBORNumber)</code> - Returns the quotient of this number and another number.
* <code>[FromCBORObject(PeterO.Cbor.CBORObject)](#FromCBORObject_PeterO_Cbor_CBORObject)</code> - Creates a CBOR number object from a CBOR object representing a number (that is, one for which the IsNumber property in.
* <code>[Multiply(PeterO.Cbor.CBORNumber)](#Multiply_PeterO_Cbor_CBORNumber)</code> - Returns a CBOR number expressing the product of this number and the given number.
* <code>[Negate()](#Negate)</code> - Returns a CBOR number with the same value as this one but with the sign reversed.
* <code>[bool operator &gt;(PeterO.Cbor.CBORNumber, PeterO.Cbor.CBORNumber)](#op_GreaterThan)</code> - Returns whether one object's value is greater than another's.
* <code>[bool operator &gt;=(PeterO.Cbor.CBORNumber, PeterO.Cbor.CBORNumber)](#op_GreaterThanOrEqual)</code> - Returns whether one object's value is at least another's.
* <code>[bool operator &lt;(PeterO.Cbor.CBORNumber, PeterO.Cbor.CBORNumber)](#op_LessThan)</code> - Returns whether one object's value is less than another's.
* <code>[bool operator &lt;=(PeterO.Cbor.CBORNumber, PeterO.Cbor.CBORNumber)](#op_LessThanOrEqual)</code> - Returns whether one object's value is up to another's.
* <code>[Remainder(PeterO.Cbor.CBORNumber)](#Remainder_PeterO_Cbor_CBORNumber)</code> - Returns the remainder when this number is divided by another number.
* <code>[Subtract(PeterO.Cbor.CBORNumber)](#Subtract_PeterO_Cbor_CBORNumber)</code> - Returns a number that expresses this number minus another.
* <code>[ToCBORObject()](#ToCBORObject)</code> - Converts this object's value to a CBOR object.
* <code>[ToString()](#ToString)</code> - Returns the value of this object in text form.

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

<a id="CompareTo_PeterO_Cbor_CBORNumber"></a>
### CompareTo

    public sealed int CompareTo(
        PeterO.Cbor.CBORNumber other);

 Compares two CBOR numbers. In this implementation, the two numbers' mathematical values are compared. Here, NaN (not-a-number) is considered greater than any number.

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

<a id="FromCBORObject_PeterO_Cbor_CBORObject"></a>
### FromCBORObject

    public static PeterO.Cbor.CBORNumber FromCBORObject(
        PeterO.Cbor.CBORObject o);

 Creates a CBOR number object from a CBOR object representing a number (that is, one for which the IsNumber property in.NET or the isNumber() method in Java returns true).

    <b>Parameters:</b>

 * <i>o</i>: The parameter is a CBOR object representing a number.

<b>Return Value:</b>

A CBOR number object, or null if the given CBOR object is null or does not represent a number.

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

 `true`  if one object's value is less than another's; otherwise,  `false` .

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

<a id="ToCBORObject"></a>
### ToCBORObject

    public PeterO.Cbor.CBORObject ToCBORObject();

 Converts this object's value to a CBOR object.

   <b>Return Value:</b>

A CBOR object that stores this object's value.

<a id="ToString"></a>
### ToString

    public override string ToString();

 Returns the value of this object in text form.

   <b>Return Value:</b>

A text string representing the value of this object.
