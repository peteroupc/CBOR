## PeterO.Cbor.CBORType

    public sealed struct CBORType :
        System.Enum,
        System.IComparable,
        System.IConvertible,
        System.IFormattable

Represents a type that a CBOR object can have.

### Member Summary
* <code>[public static PeterO.Cbor.CBORType Array = 5;](#Array)</code> - An array of CBOR objects.
* <code>[public static PeterO.Cbor.CBORType Boolean = 1;](#Boolean)</code> - The simple values true and false.
* <code>[public static PeterO.Cbor.CBORType ByteString = 3;](#ByteString)</code> - An array of bytes.
* <code>[public static PeterO.Cbor.CBORType FloatingPoint = 8;](#FloatingPoint)</code> - A 16-, 32-, or 64-bit binary floating-point number.
* <code>[public static PeterO.Cbor.CBORType Integer = 7;](#Integer)</code> - An integer in the interval [-(2^64), 2^64 - 1], or an integer of major type 0 and 1.
* <code>[public static PeterO.Cbor.CBORType Map = 6;](#Map)</code> - A map of CBOR objects.
* <code>[public static PeterO.Cbor.CBORType Number = 0;](#Number)</code> - <b>Deprecated:</b> Since version 4.0, CBORObject.Type no longer returns this value for any CBOR object - this is a breaking change from earlier versions. Instead, use the IsNumber property of CBORObject to determine whether a CBOR object represents a number, or use the two new CBORType values instead. CBORType.Integer covers CBOR objects representing integers of major type 0 and 1. CBORType.FloatingPoint covers CBOR objects representing 16-, 32-, and 64-bit floating-point numbers. CBORType.Number may be removed in version 5.0 or later.
* <code>[public static PeterO.Cbor.CBORType SimpleValue = 2;](#SimpleValue)</code> - A "simple value" other than floating point values, true, and false.
* <code>[public static PeterO.Cbor.CBORType TextString = 4;](#TextString)</code> - A text string.

<a id="Array"></a>
### Array

    public static PeterO.Cbor.CBORType Array = 5;

An array of CBOR objects.

<a id="Boolean"></a>
### Boolean

    public static PeterO.Cbor.CBORType Boolean = 1;

The simple values true and false.

<a id="ByteString"></a>
### ByteString

    public static PeterO.Cbor.CBORType ByteString = 3;

An array of bytes.

<a id="FloatingPoint"></a>
### FloatingPoint

    public static PeterO.Cbor.CBORType FloatingPoint = 8;

A 16-, 32-, or 64-bit binary floating-point number.

<a id="Integer"></a>
### Integer

    public static PeterO.Cbor.CBORType Integer = 7;

An integer in the interval [-(2^64), 2^64 - 1], or an integer of major type 0 and 1.

<a id="Map"></a>
### Map

    public static PeterO.Cbor.CBORType Map = 6;

A map of CBOR objects.

<a id="Number"></a>
### Number

    public static PeterO.Cbor.CBORType Number = 0;

<b>Deprecated.</b> Since version 4.0, CBORObject.Type no longer returns this value for any CBOR object - this is a breaking change from earlier versions. Instead, use the IsNumber property of CBORObject to determine whether a CBOR object represents a number, or use the two new CBORType values instead. CBORType.Integer covers CBOR objects representing integers of major type 0 and 1. CBORType.FloatingPoint covers CBOR objects representing 16-, 32-, and 64-bit floating-point numbers. CBORType.Number may be removed in version 5.0 or later.

This property is no longer used.

<a id="SimpleValue"></a>
### SimpleValue

    public static PeterO.Cbor.CBORType SimpleValue = 2;

A "simple value" other than floating point values, true, and false.

<a id="TextString"></a>
### TextString

    public static PeterO.Cbor.CBORType TextString = 4;

A text string.
