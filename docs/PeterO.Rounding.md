## PeterO.Rounding

    public sealed struct Rounding :
        System.Enum,
        System.IComparable,
        System.IConvertible,
        System.IFormattable

<b>Deprecated.</b> Use ERounding from PeterO.Numbers/com.upokecenter.numbers.

<b>This class is obsolete. It will be replaced by a new version of this class in a different namespace/package and library, called `PeterO.Numbers.ERounding` in the `PeterO.ERounding` library (in .NET), or `com.upokecenter.numbers.EFloat` in the<a href="https://github.com/peteroupc/numbers-java"> `com.github.peteroupc/numbers` </a>artifact (in Java).</b>

Specifies the mode to use when "shortening" numbers that otherwise can't it a given number of digits, so that the shortened number has about the ame value. This "shortening" is known as rounding.

### Ceiling

    public static PeterO.Rounding Ceiling = 2;

If there is a fractional part, the number is rounded to the highest epresentable number that's closest to it.

### Down

    public static PeterO.Rounding Down = 1;

The fractional part is discarded (the number is truncated).

### Floor

    public static PeterO.Rounding Floor = 3;

If there is a fractional part, the number is rounded to the lowest epresentable number that's closest to it.

### HalfDown

    public static PeterO.Rounding HalfDown = 5;

Rounded to the nearest number; if the fractional part is exactly half, it s discarded.

### HalfEven

    public static PeterO.Rounding HalfEven = 6;

Rounded to the nearest number; if the fractional part is exactly half, he number is rounded to the closest representable number that is even. his is sometimes also known as "banker's rounding".

### HalfUp

    public static PeterO.Rounding HalfUp = 4;

Rounded to the nearest number; if the fractional part is exactly half, he number is rounded to the closest representable number away from zero. his is the most familiar rounding mode for many people.

### Odd

    public static PeterO.Rounding Odd = 9;

If there is a fractional part and the whole number part is even, the umber is rounded to the closest representable odd number away from zero.

### OddOrZeroFiveUp

    public static PeterO.Rounding OddOrZeroFiveUp = 10;

For binary floating point numbers, this is the same as Odd. For other ases (including decimal numbers), this is the same as ZeroFiveUp. This ounding mode is useful for rounding intermediate results at a slightly igher precision (at least 2 bits more for binary) than the final recision.

### Unnecessary

    public static PeterO.Rounding Unnecessary = 7;

Indicates that rounding will not be used. If rounding is required, the ounding operation will report an error.

### Up

    public static PeterO.Rounding Up = 0;

If there is a fractional part, the number is rounded to the closest epresentable number away from zero.

### ZeroFiveUp

    public static PeterO.Rounding ZeroFiveUp = 8;

If there is a fractional part and if the last digit before rounding is 0 r half the radix, the number is rounded to the closest representable umber away from zero; otherwise the fractional part is discarded. In verflow, the fractional part is always discarded.
