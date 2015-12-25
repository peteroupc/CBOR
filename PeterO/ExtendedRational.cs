/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using PeterO.Numbers;
namespace PeterO {
    /// <summary>Arbitrary-precision rational number. This class cannot be
    /// inherited; this is a change in version 2.0 from previous versions,
    /// where the class was inadvertently left inheritable.</summary>
  public sealed class ExtendedRational : IComparable<ExtendedRational>,
    IEquatable<ExtendedRational> {
    /// <summary>Gets this object's numerator.</summary>
    /// <value>This object&apos;s numerator. If this object is a
    /// not-a-number value, returns the diagnostic information (which will
    /// be negative if this object is negative).</value>
    public  BigInteger Numerator {
get {
return new BigInteger(this.er.Numerator);
} }

    /// <summary>Gets this object's numerator with the sign
    /// removed.</summary>
    /// <value>This object&apos;s numerator. If this object is a
    /// not-a-number value, returns the diagnostic information.</value>
    public  BigInteger UnsignedNumerator {
get {
return new BigInteger(this.er.UnsignedNumerator);
} }

    /// <summary>Gets this object's denominator.</summary>
    /// <value>This object&apos;s denominator.</value>
    public  BigInteger Denominator {
get {
return new BigInteger(this.er.Denominator);
} }

    #region Equals and GetHashCode implementation
    /// <summary>Determines whether this object and another object are
    /// equal.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object obj) {
      var bi = obj as ExtendedRational;
      return (bi == null) ? (false) : (this.er.Equals(bi.er));
    }

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit hash code.</returns>
    public override int GetHashCode() {
return this.er.GetHashCode();
}
    #endregion

    /// <summary>Creates a number with the given numerator and
    /// denominator.</summary>
    /// <param name='numeratorSmall'>A 32-bit signed integer.</param>
    /// <param name='denominatorSmall'>A 32-bit signed integer.
    /// (2).</param>
    /// <returns>An ExtendedRational object.</returns>
    public static ExtendedRational Create(int numeratorSmall,
int denominatorSmall) {
return new ExtendedRational(ERational.Create(numeratorSmall, denominatorSmall));
}

    /// <summary>Creates a number with the given numerator and
    /// denominator.</summary>
    /// <param name='numerator'>A BigInteger object.</param>
    /// <param name='denominator'>Another BigInteger object.</param>
    /// <returns>An ExtendedRational object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='numerator'/> or <paramref name='denominator'/> is
    /// null.</exception>
    public static ExtendedRational Create(BigInteger numerator,
BigInteger denominator) {
  if ((numerator) == null) {
  throw new ArgumentNullException("numerator");
}
  if ((denominator) == null) {
  throw new ArgumentNullException("denominator");
}
return new ExtendedRational(ERational.Create(numerator.ei, denominator.ei));
}

    /// <summary>Initializes a new instance of the ExtendedRational
    /// class.</summary>
    /// <param name='numerator'>A BigInteger object.</param>
    /// <param name='denominator'>Another BigInteger object.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='numerator'/> or <paramref name='denominator'/> is
    /// null.</exception>
    public ExtendedRational(BigInteger numerator, BigInteger denominator) {
      this.er = new ERational(numerator.ei, denominator.ei);
 }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object. The result can be
    /// Infinity, NaN, or sNaN (with a minus sign before it for negative
    /// values), or a number of the following form:
    /// [-]numerator/denominator.</returns>
    public override string ToString() {
return this.er.ToString();
}

    internal ERational er;
    internal ExtendedRational(ERational er) {
      if ((er) == null) {
  throw new ArgumentNullException("er");
}
      this.er = er;
    }

    /// <summary>Converts a big integer to a rational number.</summary>
    /// <param name='bigint'>A BigInteger object.</param>
    /// <returns>The exact value of the integer as a rational
    /// number.</returns>
    public static ExtendedRational FromBigInteger(BigInteger bigint) {
      return new ExtendedRational(ERational.FromBigInteger(bigint.ei));
 }

    /// <summary>Converts this rational number to a decimal
    /// number.</summary>
    /// <returns>The exact value of the rational number, or not-a-number
    /// (NaN) if the result can't be exact because it has a nonterminating
    /// decimal expansion.</returns>
    public ExtendedDecimal ToExtendedDecimal() {
return new ExtendedDecimal(this.er.ToExtendedDecimal());
}

    /// <summary>Converts a 32-bit floating-point number to a rational
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting
    /// the number to a string.</summary>
    /// <param name='flt'>A 32-bit floating-point number.</param>
    /// <returns>A rational number with the same value as <paramref
    /// name='flt'/>.</returns>
    public static ExtendedRational FromSingle(float flt) {
return new ExtendedRational(ERational.FromSingle(flt));
}

    /// <summary>Converts a 64-bit floating-point number to a rational
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting
    /// the number to a string.</summary>
    /// <param name='flt'>A 64-bit floating-point number.</param>
    /// <returns>A rational number with the same value as <paramref
    /// name='flt'/>.</returns>
    public static ExtendedRational FromDouble(double flt) {
return new ExtendedRational(ERational.FromDouble(flt));
}

    /// <summary>Creates a not-a-number ExtendedRational object.</summary>
    /// <param name='diag'>A number to use as diagnostic information
    /// associated with this object. If none is needed, should be
    /// zero.</param>
    /// <returns>An ExtendedRational object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='diag'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='diag'/> is less than 0.</exception>
    public static ExtendedRational CreateNaN(BigInteger diag) {
  if ((diag) == null) {
  throw new ArgumentNullException("diag");
}
return new ExtendedRational(ERational.CreateNaN(diag.ei));
}

    /// <summary>Creates a not-a-number ExtendedRational object.</summary>
    /// <param name='diag'>A number to use as diagnostic information
    /// associated with this object. If none is needed, should be
    /// zero.</param>
    /// <param name='signaling'>Whether the return value will be signaling
    /// (true) or quiet (false).</param>
    /// <param name='negative'>Whether the return value is
    /// negative.</param>
    /// <returns>An ExtendedRational object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='diag'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='diag'/> is less than 0.</exception>
    public static ExtendedRational CreateNaN(BigInteger diag,
bool signaling,
bool negative) {
  if ((diag) == null) {
  throw new ArgumentNullException("diag");
}
return new ExtendedRational(ERational.CreateNaN(diag.ei, signaling, negative));
}

    /// <summary>Not documented yet.</summary>
    /// <param name='ef'>An ExtendedFloat object.</param>
    /// <returns>An ExtendedRational object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ef'/> is null.</exception>
    public static ExtendedRational FromExtendedFloat(ExtendedFloat ef) {
  if ((ef) == null) {
  throw new ArgumentNullException("ef");
}
return new ExtendedRational(ERational.FromExtendedFloat(ef.ef));
}

    /// <summary>Not documented yet.</summary>
    /// <param name='ef'>An ExtendedDecimal object.</param>
    /// <returns>An ExtendedRational object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ef'/> is null.</exception>
    public static ExtendedRational FromExtendedDecimal(ExtendedDecimal ef) {
  if ((ef) == null) {
  throw new ArgumentNullException("ef");
}
return new ExtendedRational(ERational.FromExtendedDecimal(ef.ed));
}

    /// <summary>Converts this rational number to a decimal number and
    /// rounds the result to the given precision.</summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedDecimal ToExtendedDecimal(PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedDecimal(this.er.ToExtendedDecimal(ctx == null ? null :
  ctx.ec));
}

    /// <summary>Converts this rational number to a decimal number, but if
    /// the result would have a nonterminating decimal expansion, rounds
    /// that result to the given precision.</summary>
    /// <param name='ctx'>A precision context object to control the
    /// precision. The rounding and exponent range settings of this context
    /// are ignored. This context will be used only if the exact result
    /// would have a nonterminating decimal expansion. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null, in which case this method is the same as
    /// ToExtendedDecimal.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedDecimal ToExtendedDecimalExactIfPossible(PrecisionContext
      ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new
  ExtendedDecimal(this.er.ToExtendedDecimalExactIfPossible(ctx == null ? null:
  ctx.ec));
}

    /// <summary>Converts this rational number to a binary
    /// number.</summary>
    /// <returns>The exact value of the rational number, or not-a-number
    /// (NaN) if the result can't be exact because it has a nonterminating
    /// binary expansion.</returns>
    public ExtendedFloat ToExtendedFloat() {
return new ExtendedFloat(this.er.ToExtendedFloat());
}

    /// <summary>Converts this rational number to a binary number and
    /// rounds the result to the given precision.</summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat ToExtendedFloat(PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedFloat(this.er.ToExtendedFloat(ctx == null ? null : ctx.ec));
}

    /// <summary>Converts this rational number to a binary number, but if
    /// the result would have a nonterminating binary expansion, rounds
    /// that result to the given precision.</summary>
    /// <param name='ctx'>A precision context object to control the
    /// precision. The rounding and exponent range settings of this context
    /// are ignored. This context will be used only if the exact result
    /// would have a nonterminating binary expansion. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null, in which case this method is the same as
    /// ToExtendedFloat.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='ctx'/> is null.</exception>
    public ExtendedFloat ToExtendedFloatExactIfPossible(PrecisionContext ctx) {
  if ((ctx) == null) {
  throw new ArgumentNullException("ctx");
}
return new ExtendedFloat(this.er.ToExtendedFloatExactIfPossible(ctx == null ?
  null : ctx.ec));
}

    /// <summary>Gets a value indicating whether this object is finite (not
    /// infinity or NaN).</summary>
    /// <value>True if this object is finite (not infinity or NaN);
    /// otherwise, false.</value>
    public  bool IsFinite {
get {
return this.er.IsFinite;
} }

    /// <summary>Converts this value to an arbitrary-precision integer. Any
    /// fractional part in this value will be discarded when converting to
    /// a big integer.</summary>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='OverflowException'>This object's value is infinity
    /// or NaN.</exception>
    public BigInteger ToBigInteger() {
return new BigInteger(this.er.ToBigInteger());
}

    /// <summary>Converts this value to an arbitrary-precision integer,
    /// checking whether the value is an exact integer.</summary>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='OverflowException'>This object's value is infinity
    /// or NaN.</exception>
    /// <exception cref='ArithmeticException'>This object's value is not an
    /// exact integer.</exception>
    public BigInteger ToBigIntegerExact() {
return new BigInteger(this.er.ToBigIntegerExact());
}

    /// <summary>Not documented yet.</summary>
    /// <param name='smallint'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedRational object.</returns>
    public static ExtendedRational FromInt32(int smallint) {
return new ExtendedRational(ERational.FromInt32(smallint));
}

    /// <summary>Not documented yet.</summary>
    /// <param name='longInt'>A 64-bit signed integer.</param>
    /// <returns>An ExtendedRational object.</returns>
    public static ExtendedRational FromInt64(long longInt) {
return new ExtendedRational(ERational.FromInt64(longInt));
}

    /// <summary>Converts this value to a 64-bit floating-point number. The
    /// half-even rounding mode is used.</summary>
    /// <returns>The closest 64-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 64-bit floating point
    /// number.</returns>
    public double ToDouble() {
return this.er.ToDouble();
}

    /// <summary>Converts this value to a 32-bit floating-point number. The
    /// half-even rounding mode is used.</summary>
    /// <returns>The closest 32-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 32-bit floating point
    /// number.</returns>
    public float ToSingle() {
return this.er.ToSingle();
}

    /// <summary>Not documented yet.</summary>
    /// <returns>An ExtendedRational object.</returns>
    public ExtendedRational Abs() {
return new ExtendedRational(this.er.Abs());
}

    /// <summary>Not documented yet.</summary>
    /// <returns>An ExtendedRational object.</returns>
    public ExtendedRational Negate() {
return new ExtendedRational(this.er.Negate());
}

    /// <summary>Gets a value indicating whether this object's value equals
    /// 0.</summary>
    /// <value>True if this object&apos;s value equals 0; otherwise,
    /// false.</value>
    public  bool IsZero {
get {
return this.er.IsZero;
} }

    /// <summary>Gets the sign of this rational number.</summary>
    /// <value>Zero if this value is zero or negative zero; -1 if this
    /// value is less than 0; and 1 if this value is greater than
    /// 0.</value>
    public  int Sign {
get {
return this.er.Sign;
} }

    /// <summary>Compares an ExtendedRational object with this
    /// instance.</summary>
    /// <param name='other'>An ExtendedRational object.</param>
    /// <returns>Zero if the values are equal; a negative number if this
    /// instance is less, or a positive number if this instance is
    /// greater.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='other'/> is null.</exception>
    public int CompareTo(ExtendedRational other) {
  if ((other) == null) {
  throw new ArgumentNullException("other");
}
return this.er.CompareTo(other.er);
}

    /// <summary>Compares an ExtendedFloat object with this
    /// instance.</summary>
    /// <param name='other'>An ExtendedFloat object.</param>
    /// <returns>Zero if the values are equal; a negative number if this
    /// instance is less, or a positive number if this instance is
    /// greater.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='other'/> is null.</exception>
    public int CompareToBinary(ExtendedFloat other) {
  if ((other) == null) {
  throw new ArgumentNullException("other");
}
return this.er.CompareToBinary(other.ef);
}

    /// <summary>Compares an ExtendedDecimal object with this
    /// instance.</summary>
    /// <param name='other'>An ExtendedDecimal object.</param>
    /// <returns>Zero if the values are equal; a negative number if this
    /// instance is less, or a positive number if this instance is
    /// greater.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='other'/> is null.</exception>
    public int CompareToDecimal(ExtendedDecimal other) {
  if ((other) == null) {
  throw new ArgumentNullException("other");
}
return this.er.CompareToDecimal(other.ed);
}

    /// <summary>Not documented yet.</summary>
    /// <param name='other'>An ExtendedRational object.</param>
    /// <returns>A Boolean object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='other'/> is null.</exception>
    public bool Equals(ExtendedRational other) {
  if ((other) == null) {
  throw new ArgumentNullException("other");
}
return this.er.Equals(other.er);
}

    /// <summary>Returns whether this object is negative
    /// infinity.</summary>
    /// <returns>True if this object is negative infinity; otherwise,
    /// false.</returns>
    public bool IsNegativeInfinity() {
return this.er.IsNegativeInfinity();
}

    /// <summary>Returns whether this object is positive
    /// infinity.</summary>
    /// <returns>True if this object is positive infinity; otherwise,
    /// false.</returns>
    public bool IsPositiveInfinity() {
return this.er.IsPositiveInfinity();
}

    /// <summary>Returns whether this object is a not-a-number
    /// value.</summary>
    /// <returns>True if this object is a not-a-number value; otherwise,
    /// false.</returns>
    public bool IsNaN() {
return this.er.IsNaN();
}

    /// <summary>Gets a value indicating whether this object's value is
    /// negative (including negative zero).</summary>
    /// <value>True if this object&apos;s value is negative; otherwise,
    /// false.</value>
    public  bool IsNegative {
get {
return this.er.IsNegative;
} }

    /// <summary>Gets a value indicating whether this object's value is
    /// infinity.</summary>
    /// <returns>True if this object's value is infinity; otherwise,
    /// false.</returns>
    public bool IsInfinity() {
return this.er.IsInfinity();
}

    /// <summary>Returns whether this object is a quiet not-a-number
    /// value.</summary>
    /// <returns>True if this object is a quiet not-a-number value;
    /// otherwise, false.</returns>
    public bool IsQuietNaN() {
return this.er.IsQuietNaN();
}

    /// <summary>Returns whether this object is a signaling not-a-number
    /// value (which causes an error if the value is passed to any
    /// arithmetic operation in this class).</summary>
    /// <returns>True if this object is a signaling not-a-number value
    /// (which causes an error if the value is passed to any arithmetic
    /// operation in this class); otherwise, false.</returns>
    public bool IsSignalingNaN() {
return this.er.IsSignalingNaN();
}

    /// <summary>A not-a-number value.</summary>
    public static readonly ExtendedRational NaN =
      new ExtendedRational(ERational.NaN);

    /// <summary>A signaling not-a-number value.</summary>
    public static readonly ExtendedRational SignalingNaN = new
      ExtendedRational(ERational.SignalingNaN);

    /// <summary>Positive infinity, greater than any other
    /// number.</summary>
    public static readonly ExtendedRational PositiveInfinity = new
      ExtendedRational(ERational.PositiveInfinity);

    /// <summary>Negative infinity, less than any other number.</summary>
    public static readonly ExtendedRational NegativeInfinity = new
      ExtendedRational(ERational.NegativeInfinity);

    /// <summary>Adds two rational numbers.</summary>
    /// <param name='otherValue'>Another ExtendedRational object.</param>
    /// <returns>The sum of the two numbers. Returns NaN if either operand
    /// is NaN.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ExtendedRational Add(ExtendedRational otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedRational(this.er.Add(otherValue.er));
}

    /// <summary>Subtracts an ExtendedRational object from this
    /// instance.</summary>
    /// <param name='otherValue'>An ExtendedRational object.</param>
    /// <returns>The difference of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ExtendedRational Subtract(ExtendedRational otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedRational(this.er.Subtract(otherValue.er));
}

    /// <summary>Multiplies this instance by the value of an
    /// ExtendedRational object.</summary>
    /// <param name='otherValue'>An ExtendedRational object.</param>
    /// <returns>The product of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ExtendedRational Multiply(ExtendedRational otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedRational(this.er.Multiply(otherValue.er));
}

    /// <summary>Divides this instance by the value of an ExtendedRational
    /// object.</summary>
    /// <param name='otherValue'>An ExtendedRational object.</param>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ExtendedRational Divide(ExtendedRational otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedRational(this.er.Divide(otherValue.er));
}

    /// <summary>Finds the remainder that results when this instance is
    /// divided by the value of a ExtendedRational object.</summary>
    /// <param name='otherValue'>An ExtendedRational object.</param>
    /// <returns>The remainder of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ExtendedRational Remainder(ExtendedRational otherValue) {
  if ((otherValue) == null) {
  throw new ArgumentNullException("otherValue");
}
return new ExtendedRational(this.er.Remainder(otherValue.er));
}

    /// <summary>A rational number for zero.</summary>
public static readonly ExtendedRational Zero =
      FromBigInteger(BigInteger.Zero);

    /// <summary>A rational number for negative zero.</summary>
    public static readonly ExtendedRational NegativeZero =
      new ExtendedRational(ERational.NegativeZero);

    /// <summary>The rational number one.</summary>
  public static readonly ExtendedRational One =
      FromBigInteger(BigInteger.One);

    /// <summary>The rational number ten.</summary>
  public static readonly ExtendedRational Ten =
      FromBigInteger((BigInteger)10);
  }
}
