## PeterO.ExtendedFloat

    public sealed class ExtendedFloat :
        System.IEquatable,
        System.IComparable

<b>Deprecated.</b> Use EFloat from PeterO.Numbers/com.upokecenter.numbers and the output of this class's ToString method.

<b>This class is largely obsolete. It will be replaced by a new version of this class in a different namespace/package and library, called `PeterO.Numbers.EFloat
          ` in the<a href="https://www.nuget.org/packages/PeterO.Numbers"> `PeterO.Numbers
            ` </a>library (in .NET), or `com.upokecenter.numbers.EFloat
          ` in the<a href="https://github.com/peteroupc/numbers-java"> `com.github.peteroupc/numbers
            ` </a>artifact (in Java). This new class can be used in the `CBORObject.FromObject(object)
          ` method (by including the new library in your code, among other hings).</b>

Represents an arbitrary-precision binary floating-point number. Consists of an integer mantissa and an integer exponent, both arbitrary-precision. The value of the number equals mantissa * 2^exponent. This class also supports values for negative zero, not-a-number (NaN) values, and infinity.

Passing a signaling NaN to any arithmetic operation shown here will signal the flag FlagInvalid and return a quiet NaN, even if another operand to that operation is a quiet NaN, unless noted otherwise.

Passing a quiet NaN to any arithmetic operation shown here will return a quiet NaN, unless noted otherwise.

Unless noted otherwise,passing a null arbitrary-precision binary float argument to any method here will throw an exception.

When an arithmetic operation signals the flag FlagInvalid, FlagOverflow, or FlagDivideByZero, it will not throw an exception too, unless the operation's trap is enabled in the precision context (see PrecisionContext's Traps property).

An arbitrary-precision binary float value can be serialized in one of the following ways:

 * By calling the toString() method. However, not all strings can be converted back to an arbitrary-precision binary float without loss, especially if the string has a fractional part.

 * By calling the UnsignedMantissa, Exponent, and IsNegative properties, and calling the IsInfinity, IsQuietNaN, and IsSignalingNaN methods. The return values combined will uniquely identify a particular arbitrary-precision binary float value.

If an operation requires creating an intermediate value that might be too big to fit in memory (or might require more than 2 gigabytes of memory to store -- due to the current use of a 32-bit integer internally as a length), the operation may signal an invalid-operation flag and return not-a-number (NaN). In certain rare cases, the CompareTo method may throw OutOfMemoryException (called OutOfMemoryError in Java) in the same circumstances.

<b>Thread safety:</b>Instances of this class are immutable, so they are inherently safe for se by multiple threads. Multiple instances of this object with the same roperties are interchangeable, so they should not be compared using the ==" operator (which might only check if each side of the operator is he same instance).

### Member Summary
* <code>[CompareTo(PeterO.ExtendedFloat)](#CompareTo_PeterO_ExtendedFloat)</code> - Compares this extended float to another.
* <code>[Create(PeterO.BigInteger, PeterO.BigInteger)](#Create_PeterO_BigInteger_PeterO_BigInteger)</code> - Creates a number with the value exponent*2^mantissa.
* <code>[Create(int, int)](#Create_int_int)</code> - Creates a number with the value exponent*2^mantissa.
* <code>[EqualsInternal(PeterO.ExtendedFloat)](#EqualsInternal_PeterO_ExtendedFloat)</code> - Determines whether this object's mantissa and exponent are equal to those of another object.
* <code>[Equals(PeterO.ExtendedFloat)](#Equals_PeterO_ExtendedFloat)</code> - Determines whether this object's mantissa and exponent are equal to those of another object.
* <code>[Equals(object)](#Equals_object)</code> - Determines whether this object's mantissa and exponent are equal to those of another object and that other object is an arbitrary-precision decimal number.
* <code>[Exponent](#Exponent)</code> - Gets this object's exponent.
* <code>[FromString(string)](#FromString_string)</code> - Creates a binary float from a text string that represents a number.
* <code>[FromString(string, int, int, PeterO.PrecisionContext)](#FromString_string_int_int_PeterO_PrecisionContext)</code> - Creates a binary float from a text string that represents a number.
* <code>[GetHashCode()](#GetHashCode)</code> - Calculates this object's hash code.
* <code>[IsInfinity()](#IsInfinity)</code> - Gets a value indicating whether this object is positive or negative infinity.
* <code>[IsNaN()](#IsNaN)</code> - Returns whether this object is a not-a-number value.
* <code>[IsNegative](#IsNegative)</code> - Gets a value indicating whether this object is negative, including negative zero.
* <code>[IsNegativeInfinity()](#IsNegativeInfinity)</code> - Returns whether this object is negative infinity.
* <code>[IsPositiveInfinity()](#IsPositiveInfinity)</code> - Returns whether this object is positive infinity.
* <code>[IsQuietNaN()](#IsQuietNaN)</code> - Gets a value indicating whether this object is a quiet not-a-number value.
* <code>[IsSignalingNaN()](#IsSignalingNaN)</code> - Gets a value indicating whether this object is a signaling not-a-number value.
* <code>[Mantissa](#Mantissa)</code> - Gets this object's un-scaled value.
* <code>[public static readonly PeterO.ExtendedFloat NaN;](#NaN)</code> - A not-a-number value.
* <code>[public static readonly PeterO.ExtendedFloat NegativeInfinity;](#NegativeInfinity)</code> - Negative infinity, less than any other number.
* <code>[public static readonly PeterO.ExtendedFloat NegativeZero;](#NegativeZero)</code> - Represents the number negative zero.
* <code>[public static readonly PeterO.ExtendedFloat One;](#One)</code> - Represents the number 1.
* <code>[public static readonly PeterO.ExtendedFloat PositiveInfinity;](#PositiveInfinity)</code> - Positive infinity, greater than any other number.
* <code>[Sign](#Sign)</code> - Gets this value's sign: -1 if negative; 1 if positive; 0 if zero.
* <code>[public static readonly PeterO.ExtendedFloat SignalingNaN;](#SignalingNaN)</code> - A not-a-number value that signals an invalid operation flag when it's passed as an argument to any arithmetic operation in arbitrary-precision binary float.
* <code>[public static readonly PeterO.ExtendedFloat Ten;](#Ten)</code> - Represents the number 10.
* <code>[ToString()](#ToString)</code> - Converts this value to a string.
* <code>[UnsignedMantissa](#UnsignedMantissa)</code> - Gets the absolute value of this object's un-scaled value.
* <code>[public static readonly PeterO.ExtendedFloat Zero;](#Zero)</code> - Represents the number 0.

<a id="NaN"></a>
### NaN

    public static readonly PeterO.ExtendedFloat NaN;

A not-a-number value.

<a id="NegativeInfinity"></a>
### NegativeInfinity

    public static readonly PeterO.ExtendedFloat NegativeInfinity;

Negative infinity, less than any other number.

<a id="NegativeZero"></a>
### NegativeZero

    public static readonly PeterO.ExtendedFloat NegativeZero;

Represents the number negative zero.

<a id="One"></a>
### One

    public static readonly PeterO.ExtendedFloat One;

Represents the number 1.

<a id="PositiveInfinity"></a>
### PositiveInfinity

    public static readonly PeterO.ExtendedFloat PositiveInfinity;

Positive infinity, greater than any other number.

<a id="SignalingNaN"></a>
### SignalingNaN

    public static readonly PeterO.ExtendedFloat SignalingNaN;

A not-a-number value that signals an invalid operation flag when it's passed as an argument to any arithmetic operation in arbitrary-precision binary float.

<a id="Ten"></a>
### Ten

    public static readonly PeterO.ExtendedFloat Ten;

Represents the number 10.

<a id="Zero"></a>
### Zero

    public static readonly PeterO.ExtendedFloat Zero;

Represents the number 0.

<a id="Exponent"></a>
### Exponent

    public PeterO.BigInteger Exponent { get; }

Gets this object's exponent. This object's value will be an integer if the exponent is positive or zero.

<b>Returns:</b>

This object's exponent. This object's value will be an integer if the exponent is positive or zero.

<a id="IsNegative"></a>
### IsNegative

    public bool IsNegative { get; }

<b>Deprecated.</b> Use EFloat from PeterO.Numbers/com.upokecenter.numbers.

Gets a value indicating whether this object is negative, including negative zero.

<b>Returns:</b>

 `true
      ` If this object is negative, including negative zero; otherwise, . `false
      ` .

<a id="Mantissa"></a>
### Mantissa

    public PeterO.BigInteger Mantissa { get; }

Gets this object's un-scaled value.

<b>Returns:</b>

This object's un-scaled value. Will be negative if this object's value is negative (including a negative NaN).

<a id="Sign"></a>
### Sign

    public int Sign { get; }

<b>Deprecated.</b> Use EFloat from PeterO.Numbers/com.upokecenter.numbers.

Gets this value's sign: -1 if negative; 1 if positive; 0 if zero.

<b>Returns:</b>

This value's sign: -1 if negative; 1 if positive; 0 if zero.

<a id="UnsignedMantissa"></a>
### UnsignedMantissa

    public PeterO.BigInteger UnsignedMantissa { get; }

Gets the absolute value of this object's un-scaled value.

<b>Returns:</b>

The absolute value of this object's un-scaled value.

<a id="CompareTo_PeterO_ExtendedFloat"></a>
### CompareTo

    public sealed int CompareTo(
        PeterO.ExtendedFloat other);

Compares this extended float to another.

<b>Parameters:</b>

 * <i>other</i>: An extended float to compare this one with.

<b>Return Value:</b>

Less than 0 if this value is less than, 0 if equal to, or greater than 0 if greater than the other value.

<a id="Create_int_int"></a>
### Create

    public static PeterO.ExtendedFloat Create(
        int mantissaSmall,
        int exponentSmall);

Creates a number with the value exponent*2^mantissa.

<b>Parameters:</b>

 * <i>mantissaSmall</i>: The un-scaled value.

 * <i>exponentSmall</i>: The binary exponent.

<b>Return Value:</b>

An arbitrary-precision binary float.

<a id="Create_PeterO_BigInteger_PeterO_BigInteger"></a>
### Create

    public static PeterO.ExtendedFloat Create(
        PeterO.BigInteger mantissa,
        PeterO.BigInteger exponent);

Creates a number with the value exponent*2^mantissa.

<b>Parameters:</b>

 * <i>mantissa</i>: The un-scaled value.

 * <i>exponent</i>: The binary exponent.

<b>Return Value:</b>

An arbitrary-precision binary float.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>mantissa</i>
or <i>exponent</i>
is null.

<a id="Equals_object"></a>
### Equals

    public override bool Equals(
        object obj);

Determines whether this object's mantissa and exponent are equal to those of another object and that other object is an arbitrary-precision decimal number.

<b>Parameters:</b>

 * <i>obj</i>: The parameter <i>obj</i>
is an arbitrary object.

<b>Return Value:</b>

 `true
      ` if the objects are equal; otherwise, `false
      ` .

<a id="Equals_PeterO_ExtendedFloat"></a>
### Equals

    public sealed bool Equals(
        PeterO.ExtendedFloat other);

<b>Deprecated.</b> Use EFloat from PeterO.Numbers/com.upokecenter.numbers.

Determines whether this object's mantissa and exponent are equal to those of another object.

<b>Parameters:</b>

 * <i>other</i>: An arbitrary-precision binary float.

<b>Return Value:</b>

 `true
      ` if this object's mantissa and exponent are equal to those of another bject; otherwise, `false
      ` .

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>other</i>
is null.

<a id="EqualsInternal_PeterO_ExtendedFloat"></a>
### EqualsInternal

    public bool EqualsInternal(
        PeterO.ExtendedFloat otherValue);

<b>Deprecated.</b> Use EFloat from PeterO.Numbers/com.upokecenter.numbers.

Determines whether this object's mantissa and exponent are equal to those of another object.

<b>Parameters:</b>

 * <i>otherValue</i>: An arbitrary-precision binary float.

<b>Return Value:</b>

 `true
      ` if this object's mantissa and exponent are equal to those of another bject; otherwise, `false
      ` .

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>otherValue</i>
is null.

<a id="FromString_string"></a>
### FromString

    public static PeterO.ExtendedFloat FromString(
        string str);

Creates a binary float from a text string that represents a number.

<b>Parameters:</b>

 * <i>str</i>: A text string containing the number to convert.

<b>Return Value:</b>

The parsed number, converted to arbitrary-precision binary float.

<a id="FromString_string_int_int_PeterO_PrecisionContext"></a>
### FromString

    public static PeterO.ExtendedFloat FromString(
        string str,
        int offset,
        int length,
        PeterO.PrecisionContext ctx);

<b>Deprecated.</b> Use EFloat from PeterO.Numbers/com.upokecenter.numbers.

Creates a binary float from a text string that represents a number. Note that if the string contains a negative exponent, the resulting value might not be exact, in which case the resulting binary float will be an approximation of this decimal number's value. (NOTE: This documentation previously said the binary float will contain enough precision to accurately convert it to a 32-bit or 64-bit floating point number. Due to double rounding, this will generally not be the case for certain numbers converted from decimal to ExtendedFloat via this method and in turn converted to `double
      ` or `float
      ` .)The format of the string generally consists of:

 * An optional plus sign ("+" , U+002B) or minus sign ("-", U+002D) (if '-' , the value is negative.)

 * One or more digits, with a single optional decimal point after the first digit and before the last digit.

 * Optionally, "E+"/"e+" (positive exponent) or "E-"/"e-" (negative exponent) plus one or more digits specifying the exponent.

The string can also be "-INF", "-Infinity", "Infinity", "INF", quiet NaN ("NaN") followed by any number of digits, or signaling NaN ("sNaN") followed by any number of digits, all in any combination of upper and lower case.

All characters mentioned above are the corresponding characters in the Basic Latin range. In particular, the digits must be the basic digits 0 to 9 (U+0030 to U+0039). The string is not allowed to contain white space characters, including spaces.

<b>Parameters:</b>

 * <i>str</i>: The parameter <i>str</i>
is a text string.

 * <i>offset</i>: A zero-based index showing where the desired portion of <i>str</i>
begins.

 * <i>length</i>: The length, in code units, of the desired portion of <i>str</i>
(but not more than <i>str</i>
's length).

 * <i>ctx</i>: A PrecisionContext object specifying the precision, rounding, and exponent range to apply to the parsed number. Can be null.

<b>Return Value:</b>

The parsed number, converted to arbitrary-precision binary float.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
is null.

 * System.ArgumentException:
Either <i>offset</i>
or <i>length</i>
is less than 0 or greater than <i>str</i>
's length, or <i>str</i>
' s length minus <i>offset</i>
is less than <i>length</i>
.

<a id="GetHashCode"></a>
### GetHashCode

    public override int GetHashCode();

Calculates this object's hash code. No application or process IDs are used in the hash code calculation.

<b>Return Value:</b>

This object's hash code.

<a id="IsInfinity"></a>
### IsInfinity

    public bool IsInfinity();

Gets a value indicating whether this object is positive or negative infinity.

<b>Return Value:</b>

 `true
      ` if this object is positive or negative infinity; otherwise, `false
      ` .

<a id="IsNaN"></a>
### IsNaN

    public bool IsNaN();

Returns whether this object is a not-a-number value.

<b>Return Value:</b>

 `true
      ` if this object is a not-a-number value; otherwise, `false
      ` .

<a id="IsNegativeInfinity"></a>
### IsNegativeInfinity

    public bool IsNegativeInfinity();

<b>Deprecated.</b> Use EFloat from PeterO.Numbers/com.upokecenter.numbers.

Returns whether this object is negative infinity.

<b>Return Value:</b>

 `true
      ` if this object is negative infinity; otherwise, `false
      ` .

<a id="IsPositiveInfinity"></a>
### IsPositiveInfinity

    public bool IsPositiveInfinity();

<b>Deprecated.</b> Use EFloat from PeterO.Numbers/com.upokecenter.numbers.

Returns whether this object is positive infinity.

<b>Return Value:</b>

 `true
      ` if this object is positive infinity; otherwise, `false
      ` .

<a id="IsQuietNaN"></a>
### IsQuietNaN

    public bool IsQuietNaN();

<b>Deprecated.</b> Use EFloat from PeterO.Numbers/com.upokecenter.numbers.

Gets a value indicating whether this object is a quiet not-a-number value.

<b>Return Value:</b>

 `true
      ` if this object is a quiet not-a-number value; otherwise, `false
      ` .

<a id="IsSignalingNaN"></a>
### IsSignalingNaN

    public bool IsSignalingNaN();

<b>Deprecated.</b> Use EFloat from PeterO.Numbers/com.upokecenter.numbers.

Gets a value indicating whether this object is a signaling not-a-number value.

<b>Return Value:</b>

 `true
      ` if this object is a signaling not-a-number value; otherwise, `false
      ` .

<a id="ToString"></a>
### ToString

    public override string ToString();

Converts this value to a string.

<b>Return Value:</b>

A string representation of this object. The value is converted to decimal and the decimal form of this number's value is returned.
