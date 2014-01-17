/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
using System.Text;
// using System.Numerics;
namespace PeterO {
    /// <summary>Represents an arbitrary-precision binary floating-point
    /// number. Consists of an integer mantissa and an integer exponent,
    /// both arbitrary-precision. The value of the number is equal to mantissa
    /// * 2^exponent.</summary>
  [Obsolete("Use ExtendedFloat instead, which supports more kinds of values than BigFloat.")]
  public sealed class BigFloat : IComparable<BigFloat>, IEquatable<BigFloat> {
    private BigInteger exponent;
    private BigInteger mantissa;

    /// <summary>Gets this object&apos;s exponent. This object&apos;s
    /// value will be an integer if the exponent is positive or zero.</summary>
    /// <value>This object&apos;s exponent. This object&apos;s value
    /// will be an integer if the exponent is positive or zero.</value>
    public BigInteger Exponent {
      get {
        return this.exponent;
      }
    }

    /// <summary>Gets this object&apos;s un-scaled value.</summary>
    /// <value>This object&apos;s un-scaled value.</value>
    public BigInteger Mantissa {
      get {
        return this.mantissa;
      }
    }
    #region Equals and GetHashCode implementation

    private bool EqualsInternal(BigFloat other) {
      BigFloat otherValue = other as BigFloat;
      if (otherValue == null) {
        return false;
      }
      return this.exponent.Equals(otherValue.exponent) &&
        this.mantissa.Equals(otherValue.mantissa);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='other'>A BigFloat object.</param>
    /// <returns>A Boolean object.</returns>
    public bool Equals(BigFloat other) {
      return this.EqualsInternal(other);
    }

    /// <summary>Determines whether this object&apos;s mantissa and exponent
    /// are equal to those of another object and that object has the same type
    /// as this one.</summary>
    /// <returns>True if the objects are equal; false otherwise.</returns>
    /// <param name='obj'>An arbitrary object.</param>
    public override bool Equals(object obj) {
      return this.EqualsInternal(obj as BigFloat);
    }

    /// <summary>Calculates this object&apos;s hash code.</summary>
    /// <returns>This object's hash code.</returns>
    public override int GetHashCode() {
      int hashCode = 0;
      unchecked {
        hashCode += 1000000007 * this.exponent.GetHashCode();
        hashCode += 1000000009 * this.mantissa.GetHashCode();
      }
      return hashCode;
    }
    #endregion
    /// <summary>Initializes a new instance of the BigFloat class. Creates
    /// a binary floating-point number with the value exponent*2^mantissa.</summary>
    /// <param name='mantissa'>The un-scaled value.</param>
    /// <param name='exponent'>The binary exponent.</param>
    public BigFloat(BigInteger mantissa, BigInteger exponent) {
      this.exponent = exponent;
      this.mantissa = mantissa;
    }

    private static BigInteger valueBigShiftIteration = (BigInteger)1000000;
    private static int valueShiftIteration = 1000000;

    private static BigInteger ShiftLeft(BigInteger val, BigInteger bigShift) {
      while (bigShift.CompareTo(valueBigShiftIteration) > 0) {
        val <<= 1000000;
        bigShift -= (BigInteger)valueBigShiftIteration;
      }
      int lastshift = (int)bigShift;
      val <<= lastshift;
      return val;
    }

    private static BigInteger ShiftLeftInt(BigInteger val, int shift) {
      while (shift > valueShiftIteration) {
        val <<= 1000000;
        shift -= valueShiftIteration;
      }
      int lastshift = (int)shift;
      val <<= lastshift;
      return val;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bigint'>A BigInteger object.</param>
    /// <returns>A BigFloat object.</returns>
    public static BigFloat FromBigInteger(BigInteger bigint) {
      return new BigFloat(bigint, BigInteger.Zero);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='numberValue'>A 64-bit signed integer.</param>
    /// <returns>A BigFloat object.</returns>
    public static BigFloat FromInt64(long numberValue) {
      BigInteger bigint = (BigInteger)numberValue;
      return new BigFloat(bigint, BigInteger.Zero);
    }

    public static BigFloat FromExtendedFloat(ExtendedFloat ef) {
      if (ef.IsNaN() || ef.IsInfinity()) {
 throw new OverflowException("Is NaN or infinity");
}
      return new BigFloat(ef.Mantissa, ef.Exponent);
    }

    /// <summary>Creates a binary floating-point number from its string
    /// representation. Note that if the binary floating-point number contains
    /// a negative exponent, the resulting value might not be exact. However,
    /// it will contain enough precision to accurately convert it to a 32-bit
    /// or 64-bit floating point number (float or double).</summary>
    /// <returns>A BigFloat object.</returns>
    /// <param name='str'>A String object.</param>
    public static BigFloat FromString(String str) {
      return BigFloat.FromExtendedFloat(ExtendedDecimal.FromString(str).ToExtendedFloat());
    }

    /// <summary>Creates a binary floating-point number from a 32-bit floating-point
    /// number.</summary>
    /// <param name='flt'>A 32-bit floating-point number.</param>
    /// <returns>A big floating-point number with the same value as <paramref
    /// name='flt'/>.</returns>
    /// <exception cref='OverflowException'>The parameter <paramref
    /// name='flt'/> is infinity or not-a-number.</exception>
    public static BigFloat FromSingle(float flt) {
      return BigFloat.FromExtendedFloat(ExtendedFloat.FromSingle(flt));
    }

    /// <summary>Creates a binary floating-point number from a 64-bit floating-point
    /// number.</summary>
    /// <param name='dbl'>A 64-bit floating-point number.</param>
    /// <returns>A big floating-point number with the same value as <paramref
    /// name='dbl'/>.</returns>
    /// <exception cref='OverflowException'>The parameter <paramref
    /// name='dbl'/> is infinity or not-a-number.</exception>
    public static BigFloat FromDouble(double dbl) {
      return BigFloat.FromExtendedFloat(ExtendedFloat.FromDouble(dbl));
    }

    /// <summary>Converts this value to an arbitrary-precision integer.
    /// Any fractional part in this value will be discarded when converting
    /// to a big integer.</summary>
    /// <returns>A BigInteger object.</returns>
    public BigInteger ToBigInteger() {
      return ExtendedFloat.Create(this.Mantissa, this.Exponent).ToBigInteger();
    }

    /// <summary>Converts this value to a 32-bit floating-point number.
    /// The half-even rounding mode is used.</summary>
    /// <returns>The closest 32-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 32-bit floating point number.</returns>
    public float ToSingle() {
      return ExtendedFloat.Create(this.Mantissa, this.Exponent).ToSingle();
    }

    /// <summary>Converts this value to a 64-bit floating-point number.
    /// The half-even rounding mode is used.</summary>
    /// <returns>The closest 64-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 64-bit floating point number.</returns>
    public double ToDouble() {
      return ExtendedFloat.Create(this.Mantissa, this.Exponent).ToDouble();
    }

    /// <summary>Converts this value to a string.The format of the return
    /// value is exactly the same as that of the java.math.BigDecimal.toString()
    /// method.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      return ExtendedFloat.Create(this.Mantissa, this.Exponent).ToString();
    }

    /// <summary>Same as toString(), except that when an exponent is used
    /// it will be a multiple of 3. The format of the return value follows the
    /// format of the java.math.BigDecimal.toEngineeringString() method.</summary>
    /// <returns>A string object.</returns>
    public string ToEngineeringString() {
      return ExtendedFloat.Create(this.Mantissa, this.Exponent).ToEngineeringString();
    }

    /// <summary>Converts this value to a string, but without an exponent
    /// part. The format of the return value follows the format of the java.math.BigDecimal.toPlainString()
    /// method.</summary>
    /// <returns>A string object.</returns>
    public string ToPlainString() {
      return ExtendedFloat.Create(this.Mantissa, this.Exponent).ToPlainString();
    }

    /// <summary>Represents the number 1.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="BigFloat is immutable")]
    #endif
    public static readonly BigFloat One = new BigFloat(BigInteger.One, BigInteger.Zero);

    /// <summary>Represents the number 0.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="BigFloat is immutable")]
    #endif
    public static readonly BigFloat Zero = new BigFloat(BigInteger.Zero, BigInteger.Zero);

    /// <summary>Represents the number 10.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="BigFloat is immutable")]
    #endif

    public static readonly BigFloat Ten = FromInt64((long)10);

    private sealed class BinaryMathHelper : IRadixMathHelper<BigFloat> {
    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetRadix() {
        return 2;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A BigFloat object.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetSign(BigFloat value) {
        return value.Sign;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A BigFloat object.</param>
    /// <returns>A BigInteger object.</returns>
      public BigInteger GetMantissa(BigFloat value) {
        return value.mantissa;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A BigFloat object.</param>
    /// <returns>A BigInteger object.</returns>
      public BigInteger GetExponent(BigFloat value) {
        return value.exponent;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>An IShiftAccumulator object.</returns>
    /// <param name='bigint'>A BigInteger object.</param>
    /// <param name='lastDigit'>A 32-bit signed integer.</param>
    /// <param name='olderDigits'>A 32-bit signed integer. (2).</param>
      public IShiftAccumulator CreateShiftAccumulatorWithDigits(BigInteger bigint, int lastDigit, int olderDigits) {
        return new BitShiftAccumulator(bigint, lastDigit, olderDigits);
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>An IShiftAccumulator object.</returns>
    /// <param name='bigint'>A BigInteger object.</param>
      public IShiftAccumulator CreateShiftAccumulator(BigInteger bigint) {
        return new BitShiftAccumulator(bigint, 0, 0);
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='num'>A BigInteger object.</param>
    /// <param name='den'>A BigInteger object. (2).</param>
    /// <returns>A Boolean object.</returns>
      public bool HasTerminatingRadixExpansion(BigInteger num, BigInteger den) {
        BigInteger gcd = BigInteger.GreatestCommonDivisor(num, den);
        if (gcd.IsZero) {
          return false;
        }
        den /= gcd;
        while (den.IsEven) {
          den >>= 1;
        }
        return den.Equals(BigInteger.One);
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='bigint'>A BigInteger object. (2).</param>
    /// <param name='power'>A FastInteger object.</param>
    /// <returns>A BigInteger object.</returns>
      public BigInteger MultiplyByRadixPower(BigInteger bigint, FastInteger power) {
        if (power.Sign <= 0) {
          return bigint;
        }
         if (bigint.Sign < 0) {
          bigint = -bigint;
          if (power.CanFitInInt32()) {
            bigint = ShiftLeftInt(bigint, power.AsInt32());
            bigint = -bigint;
          } else {
            bigint = ShiftLeft(bigint, power.AsBigInteger());
            bigint = -bigint;
          }
          return bigint;
        } else {
          if (power.CanFitInInt32()) {
            return ShiftLeftInt(bigint, power.AsInt32());
          } else {
            return ShiftLeft(bigint, power.AsBigInteger());
          }
        }
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A BigFloat object.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetFlags(BigFloat value)
      {
        return value.mantissa.Sign < 0 ? BigNumberFlags.FlagNegative : 0;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='mantissa'>A BigInteger object.</param>
    /// <param name='exponent'>A BigInteger object. (2).</param>
    /// <param name='flags'>A 32-bit signed integer.</param>
    /// <returns>A BigFloat object.</returns>
      public BigFloat CreateNewWithFlags(BigInteger mantissa, BigInteger exponent, int flags)
      {
        bool neg = (flags & BigNumberFlags.FlagNegative) != 0;
        if ((neg && mantissa.Sign > 0) || (!neg && mantissa.Sign < 0)) {
 mantissa = -mantissa;
}
        return new BigFloat(mantissa, exponent);
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetArithmeticSupport()
      {
        return BigNumberFlags.FiniteOnly;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='val'>A 32-bit signed integer.</param>
    /// <returns>A BigFloat object.</returns>
      public BigFloat ValueOf(int val) {
        return FromInt64(val);
      }
    }

    /// <summary>Gets this value&apos;s sign: -1 if negative; 1 if positive;
    /// 0 if zero.</summary>
    /// <value>This value&apos;s sign: -1 if negative; 1 if positive; 0 if
    /// zero.</value>
    public int Sign {
      get {
        return this.mantissa.Sign;
      }
    }

    /// <summary>Gets a value indicating whether this object&apos;s value
    /// equals 0.</summary>
    /// <value>Whether this object&apos;s value equals 0.</value>
    public bool IsZero {
      get {
        return this.mantissa.IsZero;
      }
    }

    /// <summary>Gets the absolute value of this object.</summary>
    /// <returns>A BigFloat object.</returns>
    public BigFloat Abs() {
      return this.Abs(null);
    }

    /// <summary>Gets an object with the same value as this one, but with the
    /// sign reversed.</summary>
    /// <returns>A BigFloat object.</returns>
    public BigFloat Negate() {
      return this.Negate(null);
    }

    /// <summary>Divides this object by another binary floating-point
    /// number and returns the result. When possible, the result will be exact.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The quotient of the two numbers.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The result would have
    /// a nonterminating decimal expansion.</exception>
    public BigFloat Divide(BigFloat divisor) {
      return this.Divide(divisor, PrecisionContext.ForRounding(Rounding.Unnecessary));
    }

    /// <summary>Divides this object by another binary floating-point
    /// number and returns a result with the same exponent as this object (the
    /// dividend).</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two numbers.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public BigFloat DivideToSameExponent(BigFloat divisor, Rounding rounding) {
      return this.DivideToExponent(divisor, this.exponent, PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Divides two BigFloat objects, and returns the integer
    /// part of the result, rounded down, with the preferred exponent set
    /// to this value&apos;s exponent minus the divisor&apos;s exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The integer part of the quotient of the two objects.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    public BigFloat DivideToIntegerNaturalScale(
      BigFloat divisor)
    {
      return this.DivideToIntegerNaturalScale(divisor, PrecisionContext.ForRounding(Rounding.Down));
    }

    /// <summary>Removes trailing zeros from this object&apos;s mantissa.
    /// For example, 1.000 becomes 1.</summary>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>This value with trailing zeros removed. Note that if the
    /// result has a very high exponent and the context says to clamp high exponents,
    /// there may still be some trailing zeros in the mantissa. If a precision
    /// context is given, returns null if the result of the rounding overflowed
    /// the exponent range.</returns>
    public BigFloat Reduce(
      PrecisionContext ctx) {
      return math.Reduce(this, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='divisor'>A BigFloat object. (2).</param>
    /// <returns>A BigFloat object.</returns>
    public BigFloat RemainderNaturalScale(
      BigFloat divisor)
    {
      return this.RemainderNaturalScale(divisor, null);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='divisor'>A BigFloat object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A BigFloat object.</returns>
    public BigFloat RemainderNaturalScale(
      BigFloat divisor,
      PrecisionContext ctx)
    {
      return this.Subtract(
        this.DivideToIntegerNaturalScale(divisor, null).Multiply(divisor, null),
        ctx);
    }

    /// <summary>Divides two BigFloat objects, and gives a particular exponent
    /// to the result.</summary>
    /// <param name='divisor'>A BigFloat object.</param>
    /// <param name='desiredExponentSmall'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal point.
    /// A positive number places the cutoff point to the left of the usual decimal
    /// point.</param>
    /// <param name='ctx'>A precision context object to control the rounding
    /// mode to use if the result must be scaled down to have the same exponent
    /// as this value. The precision setting of this context is ignored. If
    /// HasFlags of the context is true, will also store the flags resulting
    /// from the operation (the flags are in addition to the pre-existing
    /// flags). Can be null, in which case the default rounding mode is HalfEven.</param>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The desired exponent
    /// is outside of the valid range of the precision context, if it defines
    /// an exponent range.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public BigFloat DivideToExponent(
      BigFloat divisor,
      long desiredExponentSmall,
      PrecisionContext ctx)
    {
      return this.DivideToExponent(divisor, (BigInteger)desiredExponentSmall, ctx);
    }

    /// <summary>Divides two BigFloat objects, and gives a particular exponent
    /// to the result.</summary>
    /// <param name='divisor'>A BigFloat object.</param>
    /// <param name='desiredExponentSmall'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal point.
    /// A positive number places the cutoff point to the left of the usual decimal
    /// point.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    /// <returns>The quotient of the two objects.</returns>
    public BigFloat DivideToExponent(
      BigFloat divisor,
      long desiredExponentSmall,
      Rounding rounding)
    {
      return this.DivideToExponent(divisor, (BigInteger)desiredExponentSmall, PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Divides two BigFloat objects, and gives a particular exponent
    /// to the result.</summary>
    /// <param name='divisor'>A BigFloat object.</param>
    /// <param name='exponent'>The desired exponent. A negative number
    /// places the cutoff point to the right of the usual decimal point. A positive
    /// number places the cutoff point to the left of the usual decimal point.</param>
    /// <param name='ctx'>A precision context object to control the rounding
    /// mode to use if the result must be scaled down to have the same exponent
    /// as this value. The precision setting of this context is ignored. If
    /// HasFlags of the context is true, will also store the flags resulting
    /// from the operation (the flags are in addition to the pre-existing
    /// flags). Can be null, in which case the default rounding mode is HalfEven.</param>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The desired exponent
    /// is outside of the valid range of the precision context, if it defines
    /// an exponent range.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public BigFloat DivideToExponent(
      BigFloat divisor,
      BigInteger exponent,
      PrecisionContext ctx) {
      return math.DivideToExponent(this, divisor, exponent, ctx);
    }

    /// <summary>Divides two BigFloat objects, and gives a particular exponent
    /// to the result.</summary>
    /// <param name='divisor'>A BigFloat object.</param>
    /// <param name='desiredExponent'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal point.
    /// A positive number places the cutoff point to the left of the usual decimal
    /// point.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public BigFloat DivideToExponent(
      BigFloat divisor,
      BigInteger desiredExponent,
      Rounding rounding)
    {
      return this.DivideToExponent(divisor, desiredExponent, PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Finds the absolute value of this object (if it&apos;s negative,
    /// it becomes positive).</summary>
    /// <param name='context'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the context
    /// is true, will also store the flags resulting from the operation (the
    /// flags are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The absolute value of this object.</returns>
    public BigFloat Abs(PrecisionContext context) {
      if (this.Sign < 0) {
        return this.Negate(context);
      } else {
        return this.RoundToPrecision(context);
      }
    }

    /// <summary>Returns a binary floating-point number with the same value
    /// as this object but with the sign reversed.</summary>
    /// <param name='context'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the context
    /// is true, will also store the flags resulting from the operation (the
    /// flags are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>A BigFloat object.</returns>
    public BigFloat Negate(PrecisionContext context) {
      BigInteger neg = -(BigInteger)this.mantissa;
      return new BigFloat(neg, this.exponent).RoundToPrecision(context);
    }

    /// <summary>Adds this object and another binary floating-point number
    /// and returns the result.</summary>
    /// <param name='decfrac'>A BigFloat object.</param>
    /// <returns>The sum of the two objects.</returns>
    public BigFloat Add(BigFloat decfrac) {
      if (decfrac == null) {
        throw new ArgumentNullException("decfrac");
      }
      return this.Add(decfrac, PrecisionContext.Unlimited);
    }

    /// <summary>Subtracts a BigFloat object from this instance and returns
    /// the result.</summary>
    /// <param name='decfrac'>A BigFloat object.</param>
    /// <returns>The difference of the two objects.</returns>
    public BigFloat Subtract(BigFloat decfrac) {
      return this.Subtract(decfrac, null);
    }

    /// <summary>Subtracts a BigFloat object from this instance.</summary>
    /// <param name='decfrac'>A BigFloat object.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The difference of the two objects. If a precision context
    /// is given, returns null if the result of the rounding overflowed the
    /// exponent range.</returns>
    public BigFloat Subtract(BigFloat decfrac, PrecisionContext ctx) {
      if (decfrac == null) {
        throw new ArgumentNullException("decfrac");
      }
      return this.Add(decfrac.Negate(null), ctx);
    }

    /// <summary>Multiplies two binary floating-point numbers. The resulting
    /// scale will be the sum of the scales of the two binary floating-point
    /// numbers.</summary>
    /// <param name='decfrac'>Another big floating-point number.</param>
    /// <returns>The product of the two big floating-point numbers. If a
    /// precision context is given, returns null if the result of the rounding
    /// overflowed the exponent range.</returns>
    public BigFloat Multiply(BigFloat decfrac) {
      if (decfrac == null) {
        throw new ArgumentNullException("decfrac");
      }
      return this.Multiply(decfrac, PrecisionContext.Unlimited);
    }

    /// <summary>Multiplies by one binary floating-point number, and then
    /// adds another binary floating-point number.</summary>
    /// <param name='multiplicand'>The value to multiply.</param>
    /// <param name='augend'>The value to add.</param>
    /// <returns>The result this * multiplicand + augend.</returns>
    public BigFloat MultiplyAndAdd(
      BigFloat multiplicand,
      BigFloat augend) {
      return this.MultiplyAndAdd(multiplicand, augend, null);
    }
    //----------------------------------------------------------------
    private static RadixMath<BigFloat> math = new RadixMath<BigFloat>(
      new BinaryMathHelper());

    /// <summary>Divides this object by another object, and returns the
    /// integer part of the result, with the preferred exponent set to this
    /// value&apos;s exponent minus the divisor&apos;s exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the precision,
    /// rounding, and exponent range of the integer part of the result. Flags
    /// will be set on the given context only if the context&apos;s HasFlags
    /// is true and the integer part of the result doesn&apos;t fit the precision
    /// and exponent range without rounding.</param>
    /// <returns>The integer part of the quotient of the two objects. If a
    /// precision context is given, returns null if the result of the rounding
    /// overflowed the exponent range.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the integer part of the result is not exact.</exception>
    public BigFloat DivideToIntegerNaturalScale(
      BigFloat divisor,
      PrecisionContext ctx) {
      return math.DivideToIntegerNaturalScale(this, divisor, ctx);
    }

    /// <summary>Divides this object by another object, and returns the
    /// integer part of the result, with the exponent set to 0.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the precision.
    /// The rounding and exponent range settings of this context are ignored.
    /// No flags will be set from this operation even if HasFlags of the context
    /// is true. Can be null.</param>
    /// <returns>The integer part of the quotient of the two objects. The
    /// exponent will be set to 0.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The result doesn't fit
    /// the given precision.</exception>
    public BigFloat DivideToIntegerZeroScale(
      BigFloat divisor,
      PrecisionContext ctx) {
      return math.DivideToIntegerZeroScale(this, divisor, ctx);
    }

    /// <summary>Finds the remainder that results when dividing two BigFloat
    /// objects. The remainder is the value that remains when the absolute
    /// value of this object is divided by the absolute value of the other object;
    /// the remainder has the same sign (positive or negative) as this object.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the precision.
    /// The rounding and exponent range settings of this context are ignored.
    /// No flags will be set from this operation even if HasFlags of the context
    /// is true. Can be null.</param>
    /// <returns>The remainder of the two objects.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The result of integer
    /// division (the quotient, not the remainder) wouldn't fit the given
    /// precision.</exception>
    public BigFloat Remainder(
      BigFloat divisor,
      PrecisionContext ctx) {
      return math.Remainder(this, divisor, ctx);
    }

    /// <summary>Finds the distance to the closest multiple of the given
    /// divisor, based on the result of dividing this object&apos;s value
    /// by another object&apos;s value.<list type=''> <item> If this and
    /// the other object divide evenly, the result is 0.</item>
    /// <item>If the remainder's absolute value is less than half of the divisor's
    /// absolute value, the result has the same sign as this object and will
    /// be the distance to the closest multiple.</item>
    /// <item>If the remainder's absolute value is more than half of the divisor's
    /// absolute value, the result has the opposite sign of this object and
    /// will be the distance to the closest multiple.</item>
    /// <item>If the remainder's absolute value is exactly half of the divisor's
    /// absolute value, the result has the opposite sign of this object if
    /// the quotient, rounded down, is odd, and has the same sign as this object
    /// if the quotient, rounded down, is even, and the result's absolute
    /// value is half of the divisor's absolute value.</item>
    /// </list>
    /// This function is also known as the "IEEE Remainder" function.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the precision.
    /// The rounding and exponent range settings of this context are ignored
    /// (the rounding mode is always treated as HalfEven). No flags will be
    /// set from this operation even if HasFlags of the context is true. Can
    /// be null.</param>
    /// <returns>The distance of the closest multiple.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>Either the result of integer
    /// division (the quotient) or the remainder wouldn't fit the given precision.</exception>
    public BigFloat RemainderNear(
      BigFloat divisor,
      PrecisionContext ctx) {
      return math.RemainderNear(this, divisor, ctx);
    }

    /// <summary>Finds the largest value that&apos;s smaller than the given
    /// value.</summary>
    /// <param name='ctx'>A precision context object to control the precision
    /// and exponent range of the result. The rounding mode from this context
    /// is ignored. No flags will be set from this operation even if HasFlags
    /// of the context is true.</param>
    /// <returns>Returns the largest value that's less than the given value.
    /// Returns null if the result is negative infinity.</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null, the precision is 0, or <paramref name='ctx'/>
    /// has an unlimited exponent range.</exception>
    public BigFloat NextMinus(
      PrecisionContext ctx)
    {
      return math.NextMinus(this, ctx);
    }

    /// <summary>Finds the smallest value that&apos;s greater than the
    /// given value.</summary>
    /// <param name='ctx'>A precision context object to control the precision
    /// and exponent range of the result. The rounding mode from this context
    /// is ignored. No flags will be set from this operation even if HasFlags
    /// of the context is true.</param>
    /// <returns>Returns the smallest value that's greater than the given
    /// value. Returns null if the result is positive infinity.</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null, the precision is 0, or <paramref name='ctx'/>
    /// has an unlimited exponent range.</exception>
    public BigFloat NextPlus(
      PrecisionContext ctx)
    {
      return math.NextPlus(this, ctx);
    }

    /// <summary>Finds the next value that is closer to the other object&apos;s
    /// value than this object&apos;s value.</summary>
    /// <param name='otherValue'>A BigFloat object.</param>
    /// <param name='ctx'>A precision context object to control the precision
    /// and exponent range of the result. The rounding mode from this context
    /// is ignored. No flags will be set from this operation even if HasFlags
    /// of the context is true.</param>
    /// <returns>Returns the next value that is closer to the other object'
    /// s value than this object's value. Returns null if the result is infinity.</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null, the precision is 0, or <paramref name='ctx'/>
    /// has an unlimited exponent range.</exception>
    public BigFloat NextToward(
      BigFloat otherValue,
      PrecisionContext ctx)
    {
      return math.NextToward(this, otherValue, ctx);
    }

    /// <summary>Divides this BigFloat object by another BigFloat object.
    /// The preferred exponent for the result is this object&apos;s exponent
    /// minus the divisor&apos;s exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The quotient of the two objects. If a precision context
    /// is given, returns null if the result of the rounding overflowed the
    /// exponent range.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>Either ctx is null or ctx's
    /// precision is 0, and the result would have a nonterminating decimal
    /// expansion; or, the rounding mode is Rounding.Unnecessary and the
    /// result is not exact.</exception>
    public BigFloat Divide(
      BigFloat divisor,
      PrecisionContext ctx)
    {
      return math.Divide(this, divisor, ctx);
    }

    /// <summary>Gets the greater value between two binary floating-point
    /// numbers.</summary>
    /// <returns>The larger value of the two objects.</returns>
    /// <param name='first'>A BigFloat object.</param>
    /// <param name='second'>A BigFloat object. (2).</param>
    public static BigFloat Max(
      BigFloat first,
      BigFloat second) {
      return math.Max(first, second, null);
    }

    /// <summary>Gets the lesser value between two binary floating-point
    /// numbers.</summary>
    /// <returns>The smaller value of the two objects.</returns>
    /// <param name='first'>A BigFloat object.</param>
    /// <param name='second'>A BigFloat object. (2).</param>
    public static BigFloat Min(
      BigFloat first,
      BigFloat second) {
      return math.Min(first, second, null);
    }

    /// <summary>Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Max.</summary>
    /// <returns>A BigFloat object.</returns>
    /// <param name='first'>A BigFloat object. (2).</param>
    /// <param name='second'>A BigFloat object. (3).</param>
    public static BigFloat MaxMagnitude(
      BigFloat first,
      BigFloat second) {
      return math.MaxMagnitude(first, second, null);
    }

    /// <summary>Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Min.</summary>
    /// <returns>A BigFloat object.</returns>
    /// <param name='first'>A BigFloat object. (2).</param>
    /// <param name='second'>A BigFloat object. (3).</param>
    public static BigFloat MinMagnitude(
      BigFloat first,
      BigFloat second) {
      return math.MinMagnitude(first, second, null);
    }

    /// <summary>Compares the mathematical values of this object and another
    /// object.<para> This method is not consistent with the Equals method
    /// because two different big floating-point numbers with the same mathematical
    /// value, but different exponents, will compare as equal.</para>
    /// </summary>
    /// <returns>Less than 0 if this object's value is less than the other
    /// value, or greater than 0 if this object's value is greater than the
    /// other value or if <paramref name='other'/> is null, or 0 if both values
    /// are equal.</returns>
    /// <param name='other'>A BigFloat object.</param>
    public int CompareTo(
      BigFloat other) {
      return math.CompareTo(this, other);
    }

    /// <summary>Compares a BigFloat object with this instance.</summary>
    /// <param name='other'>A BigFloat object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A BigFloat object.</returns>
    public BigFloat CompareToWithContext(
      BigFloat other,
      PrecisionContext ctx) {
      return math.CompareToWithContext(this, other, false, ctx);
    }

    /// <summary>Compares a BigFloat object with this instance.</summary>
    /// <param name='other'>A BigFloat object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A BigFloat object.</returns>
    public BigFloat CompareToSignal(
      BigFloat other,
      PrecisionContext ctx) {
      return math.CompareToWithContext(this, other, true, ctx);
    }

    /// <summary>Finds the sum of this object and another object. The result&apos;s
    /// exponent is set to the lower of the exponents of the two operands.</summary>
    /// <param name='decfrac'>The number to add to.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The sum of thisValue and the other object. If a precision
    /// context is given, returns null if the result of the rounding overflowed
    /// the exponent range.</returns>
    public BigFloat Add(
      BigFloat decfrac,
      PrecisionContext ctx) {
      return math.Add(this, decfrac, ctx);
    }

    /// <summary>Returns a binary floating-point number with the same value
    /// but a new exponent.</summary>
    /// <param name='desiredExponent'>The desired exponent of the result.</param>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will also
    /// store the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>A big floating-point number with the same value as this
    /// object but with the exponent changed.</returns>
    /// <exception cref='ArithmeticException'>An overflow error occurred,
    /// or the result can't fit the given precision without rounding.</exception>
    /// <exception cref='System.ArgumentException'>The exponent is
    /// outside of the valid range of the precision context, if it defines
    /// an exponent range.</exception>
    public BigFloat Quantize(
      BigInteger desiredExponent,
      PrecisionContext ctx) {
      return this.Quantize(new BigFloat(BigInteger.One, desiredExponent), ctx);
    }

    /// <summary>Returns a binary floating-point number with the same value
    /// but a new exponent.</summary>
    /// <returns>A big floating-point number with the same value as this
    /// object but with the exponent changed.</returns>
    /// <exception cref='ArithmeticException'>An overflow error occurred,
    /// or the result can't fit the given precision without rounding.</exception>
    /// <exception cref='System.ArgumentException'>The exponent is
    /// outside of the valid range of the precision context, if it defines
    /// an exponent range.</exception>
    /// <param name='desiredExponentSmall'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public BigFloat Quantize(
      int desiredExponentSmall,
      PrecisionContext ctx) {
      return this.Quantize(new BigFloat(BigInteger.One, (BigInteger)desiredExponentSmall), ctx);
    }

    /// <summary>Returns a binary floating-point number with the same value
    /// as this object but with the same exponent as another binary floating-point
    /// number.</summary>
    /// <param name='otherValue'>A big floating-point number containing
    /// the desired exponent of the result. The mantissa is ignored.</param>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will also
    /// store the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>A big floating-point number with the same value as this
    /// object but with the exponent changed.</returns>
    /// <exception cref='ArithmeticException'>An overflow error occurred,
    /// or the result can't fit the given precision without rounding.</exception>
    /// <exception cref='System.ArgumentException'>The new exponent
    /// is outside of the valid range of the precision context, if it defines
    /// an exponent range.</exception>
    public BigFloat Quantize(
      BigFloat otherValue,
      PrecisionContext ctx) {
      return math.Quantize(this, otherValue, ctx);
    }

    /// <summary>Returns a binary floating-point number with the same value
    /// as this object but rounded to an integer.</summary>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will also
    /// store the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>A big floating-point number with the same value as this
    /// object but rounded to an integer.</returns>
    /// <exception cref='ArithmeticException'>An overflow error occurred,
    /// or the result can't fit the given precision without rounding.</exception>
    /// <exception cref='System.ArgumentException'>The new exponent
    /// must be changed to 0 when rounding and 0 is outside of the valid range
    /// of the precision context, if it defines an exponent range.</exception>
    public BigFloat RoundToIntegralExact(
      PrecisionContext ctx) {
      return math.RoundToExponentExact(this, BigInteger.Zero, ctx);
    }

    /// <summary>Returns a binary floating-point number with the same value
    /// as this object but rounded to an integer, without adding the FlagInexact
    /// or FlagRounded flags.</summary>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will also
    /// store the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags), except that this function will never
    /// add the FlagRounded and FlagInexact flags (the only difference between
    /// this and RoundToExponentExact). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>A big floating-point number with the same value as this
    /// object but rounded to an integer.</returns>
    /// <exception cref='ArithmeticException'>An overflow error occurred,
    /// or the result can't fit the given precision without rounding.</exception>
    /// <exception cref='System.ArgumentException'>The new exponent
    /// must be changed to 0 when rounding and 0 is outside of the valid range
    /// of the precision context, if it defines an exponent range.</exception>
    public BigFloat RoundToIntegralNoRoundedFlag(
      PrecisionContext ctx) {
      return math.RoundToExponentNoRoundedFlag(this, BigInteger.Zero, ctx);
    }

    /// <summary>Returns a binary floating-point number with the same value
    /// as this object but rounded to a given exponent.</summary>
    /// <param name='exponent'>The minimum exponent the result can have.
    /// This is the maximum number of fractional digits in the result, expressed
    /// as a negative number. Can also be positive, which eliminates lower-order
    /// places from the number. For example, -3 means round to the thousandth
    /// (10^-3), and 3 means round to the thousand (10^3). A value of 0 rounds
    /// the number to an integer.</param>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will also
    /// store the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>A big floating-point number rounded to the given exponent.</returns>
    /// <exception cref='ArithmeticException'>An overflow error occurred,
    /// or the result can't fit the given precision without rounding.</exception>
    /// <exception cref='System.ArgumentException'>The new exponent
    /// must be changed when rounding and the new exponent is outside of the
    /// valid range of the precision context, if it defines an exponent range.</exception>
    public BigFloat RoundToExponentExact(
      BigInteger exponent,
      PrecisionContext ctx) {
      return math.RoundToExponentExact(this, exponent, ctx);
    }

    /// <summary>Returns a binary floating-point number with the same value
    /// as this object but rounded to a given exponent, without throwing an
    /// exception if the result overflows or doesn&apos;t fit the precision
    /// range.</summary>
    /// <param name='exponent'>The minimum exponent the result can have.
    /// This is the maximum number of fractional digits in the result, expressed
    /// as a negative number. Can also be positive, which eliminates lower-order
    /// places from the number. For example, -3 means round to the thousandth
    /// (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A
    /// value of 0 rounds the number to an integer.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null, in which case
    /// the default rounding mode is HalfEven.</param>
    /// <returns>A big floating-point number rounded to the closest value
    /// representable in the given precision, meaning if the result can't
    /// fit the precision, additional digits are discarded to make it fit.
    /// If a precision context is given, returns null if the result of the rounding
    /// overflowed the exponent range.</returns>
    /// <exception cref='System.ArgumentException'>The new exponent
    /// must be changed when rounding and the new exponent is outside of the
    /// valid range of the precision context, if it defines an exponent range.</exception>
    public BigFloat RoundToExponent(
      BigInteger exponent,
      PrecisionContext ctx) {
      return math.RoundToExponentSimple(this, exponent, ctx);
    }

    /// <summary>Multiplies two binary floating-point numbers. The resulting
    /// scale will be the sum of the scales of the two binary floating-point
    /// numbers.</summary>
    /// <param name='op'>Another big floating-point number.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The product of the two big floating-point numbers. If a
    /// precision context is given, returns null if the result of the rounding
    /// overflowed the exponent range.</returns>
    public BigFloat Multiply(
      BigFloat op,
      PrecisionContext ctx) {
      return math.Multiply(this, op, ctx);
    }

    /// <summary>Multiplies by one value, and then adds another value.</summary>
    /// <param name='op'>The value to multiply.</param>
    /// <param name='augend'>The value to add.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The result thisValue * multiplicand + augend. If a precision
    /// context is given, returns null if the result of the rounding overflowed
    /// the exponent range.</returns>
    public BigFloat MultiplyAndAdd(
      BigFloat op,
      BigFloat augend,
      PrecisionContext ctx) {
      return math.MultiplyAndAdd(this, op, augend, ctx);
    }

    /// <summary>Rounds this object&apos;s value to a given precision,
    /// using the given rounding mode and range of exponent.</summary>
    /// <param name='ctx'>A context for controlling the precision, rounding
    /// mode, and exponent range. Can be null.</param>
    /// <returns>The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if " context"
    /// is null or the precision and exponent range are unlimited. If a precision
    /// context is given, returns null if the result of the rounding overflowed
    /// the exponent range.</returns>
    public BigFloat RoundToPrecision(
      PrecisionContext ctx) {
      return math.RoundToPrecision(this, ctx);
    }

    /// <summary>Rounds this object&apos;s value to a given maximum bit
    /// length, using the given rounding mode and range of exponent.</summary>
    /// <param name='ctx'>A context for controlling the precision, rounding
    /// mode, and exponent range. The precision is interpreted as the maximum
    /// bit length of the mantissa. Can be null.</param>
    /// <returns>The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if " context"
    /// is null or the precision and exponent range are unlimited. Returns
    /// null if the result of the rounding overflowed the exponent range.</returns>
    public BigFloat RoundToBinaryPrecision(
      PrecisionContext ctx) {
      return math.RoundToBinaryPrecision(this, ctx);
    }
  }
}
