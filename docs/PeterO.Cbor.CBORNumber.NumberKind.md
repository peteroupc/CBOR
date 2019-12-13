## PeterO.Cbor.CBORNumber.NumberKind

    public sealed struct NumberKind :
        System.Enum,
        System.IComparable,
        System.IConvertible,
        System.IFormattable

Specifies the underlying form of this CBOR number object.

### Member Summary
* <code>[public static PeterO.Cbor.CBORNumber.NumberKind Double = 1;](#Double)</code> - A 64-bit binary floating-point number.
* <code>[public static PeterO.Cbor.CBORNumber.NumberKind EDecimal = 3;](#EDecimal)</code> - An arbitrary-precision decimal number.
* <code>[public static PeterO.Cbor.CBORNumber.NumberKind EFloat = 4;](#EFloat)</code> - An arbitrary-precision binary number.
* <code>[public static PeterO.Cbor.CBORNumber.NumberKind EInteger = 2;](#EInteger)</code> - An arbitrary-precision integer.
* <code>[public static PeterO.Cbor.CBORNumber.NumberKind ERational = 5;](#ERational)</code> - An arbitrary-precision rational number.
* <code>[public static PeterO.Cbor.CBORNumber.NumberKind Integer = 0;](#Integer)</code> - A 64-bit signed integer.

<a id="Double"></a>
### Double

    public static PeterO.Cbor.CBORNumber.NumberKind Double = 1;

A 64-bit binary floating-point number.

<a id="EDecimal"></a>
### EDecimal

    public static PeterO.Cbor.CBORNumber.NumberKind EDecimal = 3;

An arbitrary-precision decimal number.

<a id="EFloat"></a>
### EFloat

    public static PeterO.Cbor.CBORNumber.NumberKind EFloat = 4;

An arbitrary-precision binary number.

<a id="EInteger"></a>
### EInteger

    public static PeterO.Cbor.CBORNumber.NumberKind EInteger = 2;

An arbitrary-precision integer.

<a id="ERational"></a>
### ERational

    public static PeterO.Cbor.CBORNumber.NumberKind ERational = 5;

An arbitrary-precision rational number.

<a id="Integer"></a>
### Integer

    public static PeterO.Cbor.CBORNumber.NumberKind Integer = 0;

A 64-bit signed integer.
