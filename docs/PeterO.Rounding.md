## PeterO.Rounding

    public sealed struct Rounding :
        System.Enum,
        System.IComparable,
        System.IConvertible,
        System.IFormattable

<b>Deprecated.</b> Use ERounding from PeterO.Numbers/com.upokecenter.numbers.

<b>This class is obsolete. It will be replaced by a new version of this class in a different namespace/package and library, called `PeterO.Numbers.ERounding` in the `PeterO.ERounding` library (in .NET), or `com.upokecenter.numbers.EFloat` in the<a href="https://github.com/peteroupc/numbers-java"> `com.github.peteroupc/numbers` </a>artifact (in Java).</b>

Specifies the mode to use when "shortening" numbers that otherwise can't it a given number of digits, so that the shortened number has about the ame value. This "shortening" is known as rounding.

### Member Summary
* <code>[public static PeterO.Rounding Ceiling = 2;](#Ceiling)</code> - If there is a fractional part, the number is rounded to the highest representable number that's closest to it.
* <code>[public static PeterO.Rounding Down = 1;](#Down)</code> - The fractional part is discarded (the number is truncated).
* <code>[public static PeterO.Rounding Floor = 3;](#Floor)</code> - If there is a fractional part, the number is rounded to the lowest representable number that's closest to it.
* <code>[public static PeterO.Rounding HalfDown = 5;](#HalfDown)</code> - Rounded to the nearest number; if the fractional part is exactly half, it is discarded.
* <code>[public static PeterO.Rounding HalfEven = 6;](#HalfEven)</code> - Rounded to the nearest number; if the fractional part is exactly half, the number is rounded to the closest representable number that is even.
* <code>[public static PeterO.Rounding HalfUp = 4;](#HalfUp)</code> - Rounded to the nearest number; if the fractional part is exactly half, the number is rounded to the closest representable number away from zero.
* <code>[public static PeterO.Rounding Odd = 9;](#Odd)</code> - If there is a fractional part and the whole number part is even, the number is rounded to the closest representable odd number away from zero.
* <code>[public static PeterO.Rounding OddOrZeroFiveUp = 10;](#OddOrZeroFiveUp)</code> - For binary floating point numbers, this is the same as Odd.
* <code>[public static PeterO.Rounding Unnecessary = 7;](#Unnecessary)</code> - Indicates that rounding will not be used.
* <code>[public static PeterO.Rounding Up = 0;](#Up)</code> - If there is a fractional part, the number is rounded to the closest representable number away from zero.
* <code>[public static PeterO.Rounding ZeroFiveUp = 8;](#ZeroFiveUp)</code> - If there is a fractional part and if the last digit before rounding is 0 or half the radix, the number is rounded to the closest representable number away from zero; otherwise the fractional part is discarded.

<a id="Ceiling"></a>
### Ceiling

    public static PeterO.Rounding Ceiling = 2;

If there is a fractional part, the number is rounded to the highest representable number that's closest to it.

<a id="Down"></a>
### Down

    public static PeterO.Rounding Down = 1;

The fractional part is discarded (the number is truncated).

<a id="Floor"></a>
### Floor

    public static PeterO.Rounding Floor = 3;

If there is a fractional part, the number is rounded to the lowest representable number that's closest to it.

<a id="HalfDown"></a>
### HalfDown

    public static PeterO.Rounding HalfDown = 5;

Rounded to the nearest number; if the fractional part is exactly half, it is discarded.

<a id="HalfEven"></a>
### HalfEven

    public static PeterO.Rounding HalfEven = 6;

Rounded to the nearest number; if the fractional part is exactly half, the number is rounded to the closest representable number that is even. This is sometimes also known as "banker's rounding".

<a id="HalfUp"></a>
### HalfUp

    public static PeterO.Rounding HalfUp = 4;

Rounded to the nearest number; if the fractional part is exactly half, the number is rounded to the closest representable number away from zero. This is the most familiar rounding mode for many people.

<a id="Odd"></a>
### Odd

    public static PeterO.Rounding Odd = 9;

If there is a fractional part and the whole number part is even, the number is rounded to the closest representable odd number away from zero.

<a id="OddOrZeroFiveUp"></a>
### OddOrZeroFiveUp

    public static PeterO.Rounding OddOrZeroFiveUp = 10;

For binary floating point numbers, this is the same as Odd. For other bases (including decimal numbers), this is the same as ZeroFiveUp. This rounding mode is useful for rounding intermediate results at a slightly higher precision (at least 2 bits more for binary) than the final precision.

<a id="Unnecessary"></a>
### Unnecessary

    public static PeterO.Rounding Unnecessary = 7;

Indicates that rounding will not be used. If rounding is required, the rounding operation will report an error.

<a id="Up"></a>
### Up

    public static PeterO.Rounding Up = 0;

If there is a fractional part, the number is rounded to the closest representable number away from zero.

<a id="ZeroFiveUp"></a>
### ZeroFiveUp

    public static PeterO.Rounding ZeroFiveUp = 8;

If there is a fractional part and if the last digit before rounding is 0 or half the radix, the number is rounded to the closest representable number away from zero; otherwise the fractional part is discarded. In overflow, the fractional part is always discarded.
