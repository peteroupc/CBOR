## PeterO.Cbor.JSONOptions.ConversionMode

    public sealed struct ConversionMode :
        System.Enum,
        System.IComparable,
        System.IConvertible,
        System.IFormattable

Specifies how JSON numbers are converted to CBOR when decoding JSON.

### Member Summary
* <code>[public static PeterO.Cbor.JSONOptions.ConversionMode Decimal128 = 4;](#Decimal128)</code> - JSON numbers are decoded to CBOR as their closest-rounded approximation to an IEEE 854 decimal128 value, using the rules for the EDecimal form of that approximation as given in the CBORObject.
* <code>[public static PeterO.Cbor.JSONOptions.ConversionMode Double = 1;](#Double)</code> - JSON numbers are decoded to CBOR as their closest-rounded approximation as 64-bit binary floating-point numbers.
* <code>[public static PeterO.Cbor.JSONOptions.ConversionMode Full = 0;](#Full)</code> - JSON numbers are decoded to CBOR using the full precision given in the JSON text.
* <code>[public static PeterO.Cbor.JSONOptions.ConversionMode IntOrFloat = 2;](#IntOrFloat)</code> - A JSON number is decoded to CBOR either as a CBOR integer (major type 0 or 1) if the JSON number represents an integer at least -(2^53)+1 and less than 2^53, or as their closest-rounded approximation as 64-bit binary floating-point numbers otherwise.
* <code>[public static PeterO.Cbor.JSONOptions.ConversionMode IntOrFloatFromDouble = 3;](#IntOrFloatFromDouble)</code> - A JSON number is decoded to CBOR either as a CBOR integer (major type 0 or 1) if the number's closest-rounded approximation as a 64-bit binary floating-point number represents an integer at least -(2^53)+1 and less than 2^53, or as that approximation otherwise.

<a id="Decimal128"></a>
### Decimal128

    public static PeterO.Cbor.JSONOptions.ConversionMode Decimal128 = 4;

JSON numbers are decoded to CBOR as their closest-rounded approximation to an IEEE 854 decimal128 value, using the rules for the EDecimal form of that approximation as given in the  `CBORObject.FromObject(EDecimal)`  method.

<a id="Double"></a>
### Double

    public static PeterO.Cbor.JSONOptions.ConversionMode Double = 1;

JSON numbers are decoded to CBOR as their closest-rounded approximation as 64-bit binary floating-point numbers. (In some cases, numbers extremely close to zero may underflow to positive or negative zero, and numbers of extremely large magnitude may overflow to infinity.).

<a id="Full"></a>
### Full

    public static PeterO.Cbor.JSONOptions.ConversionMode Full = 0;

JSON numbers are decoded to CBOR using the full precision given in the JSON text. The number will be converted to a CBOR object as follows: If the number's exponent is 0 (after shifting the decimal point to the end of the number without changing its value), using the rules given in the  `CBORObject.FromObject(EInteger)`  method; otherwise, using the rules given in the  `CBORObject.FromObject(EDecimal)`  method. An exception in version 4.x involves negative zeros; if the negative zero's exponent is 0, it's written as a CBOR floating-point number; otherwise the negative zero is written as an EDecimal.

<a id="IntOrFloat"></a>
### IntOrFloat

    public static PeterO.Cbor.JSONOptions.ConversionMode IntOrFloat = 2;

A JSON number is decoded to CBOR either as a CBOR integer (major type 0 or 1) if the JSON number represents an integer at least -(2^53)+1 and less than 2^53, or as their closest-rounded approximation as 64-bit binary floating-point numbers otherwise. For example, the JSON number 0.99999999999999999999999999999999999 is not an integer, so it's converted to its closest floating-point approximation, namely 1.0. (In some cases, numbers extremely close to zero may underflow to positive or negative zero, and numbers of extremely large magnitude may overflow to infinity.).

<a id="IntOrFloatFromDouble"></a>
### IntOrFloatFromDouble

    public static PeterO.Cbor.JSONOptions.ConversionMode IntOrFloatFromDouble = 3;

A JSON number is decoded to CBOR either as a CBOR integer (major type 0 or 1) if the number's closest-rounded approximation as a 64-bit binary floating-point number represents an integer at least -(2^53)+1 and less than 2^53, or as that approximation otherwise. For example, the JSON number 0.99999999999999999999999999999999999 is the integer 1 when rounded to its closest floating-point approximation (1.0), so it's converted to the CBOR integer 1 (major type 0). (In some cases, numbers extremely close to zero may underflow to zero, and numbers of extremely large magnitude may overflow to infinity.).
