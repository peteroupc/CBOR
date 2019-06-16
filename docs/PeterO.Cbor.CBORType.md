## PeterO.Cbor.CBORType

    public sealed struct CBORType :
        System.Enum,
        System.IFormattable,
        System.IComparable,
        System.IConvertible

Represents a type that a CBOR object can have.

### Member Summary
* <code>[Array](#Array)</code> - An array of CBOR objects.
* <code>[Boolean](#Boolean)</code> - The simple values true and false.
* <code>[ByteString](#ByteString)</code> - An array of bytes.
* <code>[Map](#Map)</code> - A map of CBOR objects.
* <code>[Number](#Number)</code> - A number of any kind, including integers, big integers, floating point numbers, and decimal numbers.
* <code>[SimpleValue](#SimpleValue)</code> - A "simple value" other than floating point values, true, and false.
* <code>[TextString](#TextString)</code> - A text string.

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

<a id="Map"></a>
### Map

    public static PeterO.Cbor.CBORType Map = 6;

A map of CBOR objects.

<a id="Number"></a>
### Number

    public static PeterO.Cbor.CBORType Number = 0;

A number of any kind, including integers, big integers, floating point numbers, and decimal numbers. The floating-point value Not-a-Number is also included in the Number type.

<a id="SimpleValue"></a>
### SimpleValue

    public static PeterO.Cbor.CBORType SimpleValue = 2;

A "simple value" other than floating point values, true, and false.

<a id="TextString"></a>
### TextString

    public static PeterO.Cbor.CBORType TextString = 4;

A text string.
