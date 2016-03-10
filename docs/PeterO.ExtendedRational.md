## PeterO.ExtendedRational

    public sealed class ExtendedRational :
        System.IComparable,
        System.IEquatable

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers and the output of this class's ToString method.

<b>This class is largely obsolete. It will be replaced by a new version of this class in a different namespace/package and library, called  `PeterO.Numbers.ERational`  in the<a href="https://www.nuget.org/packages/PeterO.Numbers"> `PeterO.Numbers` </a>library (in .NET), or  `com.upokecenter.numbers.ERational`  in the<a href="https://github.com/peteroupc/numbers-java"> `com.github.peteroupc/numbers` </a>artifact (in Java). This new class can be used in the `CBORObject.FromObject(object)`  method (by including the new library in your code, among other things).</b>

Arbitrary-precision rational number. This class can't be inherited; this is a change in version 2.0 from previous versions, where the class was inadvertently left inheritable.<b>Thread safety:</b> Instances of this class are immutable, so they are inherently safe for use by multiple threads. Multiple instances of this object with the same properties are interchangeable, so they should not be compared using the "==" operator (which might only check if each side of the operator is the same instance).

### ExtendedRational Constructor

    public ExtendedRational(
        PeterO.BigInteger numerator,
        PeterO.BigInteger denominator);

Initializes a new instance of the [PeterO.ExtendedRational](PeterO.ExtendedRational.md) class.

<b>Parameters:</b>

 * <i>numerator</i>: An arbitrary-precision integer.

 * <i>denominator</i>: Another arbitrary-precision integer.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>numerator</i>
 or  <i>denominator</i>
 is null.

### NaN

    public static readonly PeterO.ExtendedRational NaN;

A not-a-number value.

### NegativeInfinity

    public static readonly PeterO.ExtendedRational NegativeInfinity;

Negative infinity, less than any other number.

### NegativeZero

    public static readonly PeterO.ExtendedRational NegativeZero;

A rational number for negative zero.

### One

    public static readonly PeterO.ExtendedRational One;

The rational number one.

### PositiveInfinity

    public static readonly PeterO.ExtendedRational PositiveInfinity;

Positive infinity, greater than any other number.

### SignalingNaN

    public static readonly PeterO.ExtendedRational SignalingNaN;

A signaling not-a-number value.

### Ten

    public static readonly PeterO.ExtendedRational Ten;

The rational number ten.

### Zero

    public static readonly PeterO.ExtendedRational Zero;

A rational number for zero.

### Denominator

    public PeterO.BigInteger Denominator { get; }

Gets this object's denominator.

<b>Returns:</b>

This object's denominator.

### IsFinite

    public bool IsFinite { get; }

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Gets a value indicating whether this object is finite (not infinity or NaN).

<b>Returns:</b>

 `true`  If this object is finite (not infinity or NaN); otherwise,  `false` .

### IsNegative

    public bool IsNegative { get; }

Gets a value indicating whether this object's value is negative (including negative zero).

<b>Returns:</b>

 `true`  If this object's value is negative; otherwise,  `false` .

### IsZero

    public bool IsZero { get; }

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Gets a value indicating whether this object's value equals 0.

<b>Returns:</b>

 `true`  If this object's value equals 0; otherwise, .  `false` .

### Numerator

    public PeterO.BigInteger Numerator { get; }

Gets this object's numerator.

<b>Returns:</b>

This object's numerator. If this object is a not-a-number value, returns the diagnostic information (which will be negative if this object is negative).

### Sign

    public int Sign { get; }

<b>Deprecated.</b> Use ERational from PeterO.Numbers/com.upokecenter.numbers.

Gets the sign of this rational number.

<b>Returns:</b>

Zero if this value is zero or negative zero; -1 if this value is less than 0; and 1 if this value is greater than 0.

### UnsignedNumerator

    public PeterO.BigInteger UnsignedNumerator { get; }

Gets this object's numerator with the sign removed.

<b>Returns:</b>

This object's numerator. If this object is a not-a-number value, returns the diagnostic information.

### CompareTo

    public sealed int CompareTo(
        PeterO.ExtendedRational other);

Not documented yet.

<b>Parameters:</b>

 * <i>other</i>: An ExtendedRational object.

<b>Return Value:</b>

A 32-bit signed integer.

### Create

    public static PeterO.ExtendedRational Create(
        int numeratorSmall,
        int denominatorSmall);

Creates a rational number with the given numerator and denominator.

<b>Parameters:</b>

 * <i>numeratorSmall</i>: A 32-bit signed integer.

 * <i>denominatorSmall</i>: A 32-bit signed integer. (2).

<b>Return Value:</b>

An arbitrary-precision rational number.

### Create

    public static PeterO.ExtendedRational Create(
        PeterO.BigInteger numerator,
        PeterO.BigInteger denominator);

Creates a rational number with the given numerator and denominator.

<b>Parameters:</b>

 * <i>numerator</i>: An arbitrary-precision integer.

 * <i>denominator</i>: Another arbitrary-precision integer.

<b>Return Value:</b>

An arbitrary-precision rational number.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>numerator</i>
 or  <i>denominator</i>
 is null.

### Equals

    public override bool Equals(
        object obj);

Not documented yet.

<b>Parameters:</b>

 * <i>obj</i>: An arbitrary object.

<b>Return Value:</b>

A Boolean object.

### Equals

    public sealed bool Equals(
        PeterO.ExtendedRational other);

Not documented yet.

<b>Parameters:</b>

 * <i>other</i>: An ExtendedRational object.

<b>Return Value:</b>

A Boolean object.

### GetHashCode

    public override int GetHashCode();

Calculates the hash code for this object. No application or process IDs are used in the hash code calculation.

<b>Return Value:</b>

A 32-bit signed integer.

### ToString

    public override string ToString();

Converts this object to a text string.

<b>Return Value:</b>

A string representation of this object. The result can be Infinity, NaN, or sNaN (with a minus sign before it for negative values), or a number of the following form: [-]numerator/denominator.
