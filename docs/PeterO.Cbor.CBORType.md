## PeterO.Cbor.CBORType

    public sealed struct CBORType :
        System.Enum,
        System.IComparable,
        System.IFormattable,
        System.IConvertible

Represents a type that a CBOR object can have.

### Number

    public static PeterO.Cbor.CBORType Number = 0;

A number of any kind, including integers, big integers, floating point numbers, and decimal numbers. The floating-point value Not-a-Number is also included in the Number type.

### Boolean

    public static PeterO.Cbor.CBORType Boolean = 1;

The simple values true and false.

### SimpleValue

    public static PeterO.Cbor.CBORType SimpleValue = 2;

A "simple value" other than floating point values, true, and false.

### ByteString

    public static PeterO.Cbor.CBORType ByteString = 3;

An array of bytes.

### TextString

    public static PeterO.Cbor.CBORType TextString = 4;

A text string.

### Array

    public static PeterO.Cbor.CBORType Array = 5;

An array of CBOR objects.

### Map

    public static PeterO.Cbor.CBORType Map = 6;

A map of CBOR objects.


