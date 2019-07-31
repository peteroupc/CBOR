## PeterO.Cbor.CBORNumber

    public sealed class CBORNumber :
        System.IComparable

 Not documented yet.

### Member Summary
* <code>[Add(PeterO.Cbor.CBORNumber)](#Add_PeterO_Cbor_CBORNumber)</code> - Not documented yet.
* <code>[CompareTo(PeterO.Cbor.CBORNumber)](#CompareTo_PeterO_Cbor_CBORNumber)</code> - Compares two CBOR numbers.
* <code>[Divide(PeterO.Cbor.CBORNumber)](#Divide_PeterO_Cbor_CBORNumber)</code> - Not documented yet.
* <code>[FromCBORObject(PeterO.Cbor.CBORObject)](#FromCBORObject_PeterO_Cbor_CBORObject)</code> - Not documented yet.
* <code>[Multiply(PeterO.Cbor.CBORNumber)](#Multiply_PeterO_Cbor_CBORNumber)</code> - Not documented yet.
* <code>[Negate()](#Negate)</code> - Not documented yet.
* <code>[Remainder(PeterO.Cbor.CBORNumber)](#Remainder_PeterO_Cbor_CBORNumber)</code> - Not documented yet.
* <code>[Subtract(PeterO.Cbor.CBORNumber)](#Subtract_PeterO_Cbor_CBORNumber)</code> - Not documented yet.
* <code>[ToCBORObject()](#ToCBORObject)</code> - Not documented yet.
* <code>[ToString()](#ToString)</code> - Not documented yet.

<a id="Add_PeterO_Cbor_CBORNumber"></a>
### Add

    public PeterO.Cbor.CBORNumber Add(
        PeterO.Cbor.CBORNumber b);

 Not documented yet.

     <b>Parameters:</b>

 * <i>b</i>: Not documented yet.

<b>Return Value:</b>

A CBORNumber object.

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

Less than 0, if this value is less than the other object; or 0, if both values are equal; or greater than 0, if this value is less than the other object or if the other object is null.

<b>Exceptions:</b>

 * System.ArgumentException:
An internal error occurred.

<a id="Divide_PeterO_Cbor_CBORNumber"></a>
### Divide

    public PeterO.Cbor.CBORNumber Divide(
        PeterO.Cbor.CBORNumber b);

 Not documented yet.

     <b>Parameters:</b>

 * <i>b</i>: Not documented yet.

<b>Return Value:</b>

A CBORNumber object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>b</i>
 is null.

<a id="FromCBORObject_PeterO_Cbor_CBORObject"></a>
### FromCBORObject

    public static PeterO.Cbor.CBORNumber FromCBORObject(
        PeterO.Cbor.CBORObject o);

 Not documented yet.

     <b>Parameters:</b>

 * <i>o</i>: Not documented yet.

<b>Return Value:</b>

A CBORNumber object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>o</i>
 is null.

<a id="Multiply_PeterO_Cbor_CBORNumber"></a>
### Multiply

    public PeterO.Cbor.CBORNumber Multiply(
        PeterO.Cbor.CBORNumber b);

 Not documented yet.

     <b>Parameters:</b>

 * <i>b</i>: Not documented yet.

<b>Return Value:</b>

A CBORNumber object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>b</i>
 is null.

<a id="Negate"></a>
### Negate

    public PeterO.Cbor.CBORNumber Negate();

 Not documented yet.

   <b>Return Value:</b>

A CBORNumber object.

<a id="Remainder_PeterO_Cbor_CBORNumber"></a>
### Remainder

    public PeterO.Cbor.CBORNumber Remainder(
        PeterO.Cbor.CBORNumber b);

 Not documented yet.

     <b>Parameters:</b>

 * <i>b</i>: Not documented yet.

<b>Return Value:</b>

A CBORNumber object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>b</i>
 is null.

<a id="Subtract_PeterO_Cbor_CBORNumber"></a>
### Subtract

    public PeterO.Cbor.CBORNumber Subtract(
        PeterO.Cbor.CBORNumber b);

 Not documented yet.

     <b>Parameters:</b>

 * <i>b</i>: Not documented yet.

<b>Return Value:</b>

A CBORNumber object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>b</i>
 is null.

<a id="ToCBORObject"></a>
### ToCBORObject

    public PeterO.Cbor.CBORObject ToCBORObject();

 Not documented yet.

   <b>Return Value:</b>

A CBORObject object.

<a id="ToString"></a>
### ToString

    public override string ToString();

 Not documented yet.

   <b>Return Value:</b>

A text string.
