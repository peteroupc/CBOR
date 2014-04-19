/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;

namespace PeterO {
    /// <summary>Description of ExtendedOrSimpleRadixMath.</summary>
    /// <typeparam name='T'>Number data type.</typeparam>
  internal class ExtendedOrSimpleRadixMath<T> : IRadixMath<T>
  {
    private RadixMath<T> ext;
    private SimpleRadixMath<T> simp;

    public ExtendedOrSimpleRadixMath(IRadixMathHelper<T> helper) {
      this.ext = new RadixMath<T>(helper);
      this.simp = new SimpleRadixMath<T>(this.ext);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An IRadixMathHelper(T) object.</returns>
public IRadixMathHelper<T> GetHelper() {
      // Both RadixMath implementations return the
      // same helper, so use the ext implementation
      return this.ext.GetHelper();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='divisor'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T DivideToIntegerNaturalScale(T thisValue, T divisor, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.DivideToIntegerNaturalScale(thisValue, divisor, ctx) :
        this.simp.DivideToIntegerNaturalScale(thisValue, divisor, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='divisor'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T DivideToIntegerZeroScale(T thisValue, T divisor, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.DivideToIntegerZeroScale(thisValue, divisor, ctx) :
        this.simp.DivideToIntegerZeroScale(thisValue, divisor, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T Abs(T value, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.Abs(value, ctx) :
        this.simp.Abs(value, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T Negate(T value, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.Negate(value, ctx) :
        this.simp.Negate(value, ctx);
    }

    /// <summary>Finds the remainder that results when dividing two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The remainder of the two objects.</returns>
public T Remainder(T thisValue, T divisor, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.Remainder(thisValue, divisor, ctx) :
        this.simp.Remainder(thisValue, divisor, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='divisor'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T RemainderNear(T thisValue, T divisor, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.RemainderNear(thisValue, divisor, ctx) :
        this.simp.RemainderNear(thisValue, divisor, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T Pi(PrecisionContext ctx) {
      return (!ctx.IsSimplified) ? this.ext.Pi(ctx) : this.simp.Pi(ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='pow'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T Power(T thisValue, T pow, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.Power(thisValue, pow, ctx) :
        this.simp.Power(thisValue, pow, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T Log10(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.Log10(thisValue, ctx) :
        this.simp.Log10(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T Ln(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.Ln(thisValue, ctx) :
        this.simp.Ln(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T Exp(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.Exp(thisValue, ctx) :
        this.simp.Exp(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T SquareRoot(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.SquareRoot(thisValue, ctx) :
        this.simp.SquareRoot(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T NextMinus(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.NextMinus(thisValue, ctx) :
        this.simp.NextMinus(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='otherValue'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T NextToward(T thisValue, T otherValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.NextToward(thisValue, otherValue, ctx) :
        this.simp.NextToward(thisValue, otherValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T NextPlus(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.NextPlus(thisValue, ctx) :
        this.simp.NextPlus(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='divisor'>A T object. (3).</param>
    /// <param name='desiredExponent'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T DivideToExponent(T thisValue, T divisor, BigInteger desiredExponent, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.DivideToExponent(thisValue, divisor, desiredExponent, ctx) :
        this.simp.DivideToExponent(thisValue, divisor, desiredExponent, ctx);
    }

    /// <summary>Divides two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The quotient of the two objects.</returns>
public T Divide(T thisValue, T divisor, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.Divide(thisValue, divisor, ctx) :
        this.simp.Divide(thisValue, divisor, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='a'>A T object. (2).</param>
    /// <param name='b'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T MinMagnitude(T a, T b, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.MinMagnitude(a, b, ctx) : this.simp.MinMagnitude(a, b, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='a'>A T object. (2).</param>
    /// <param name='b'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T MaxMagnitude(T a, T b, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.MaxMagnitude(a, b, ctx) : this.simp.MaxMagnitude(a, b, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='a'>A T object. (2).</param>
    /// <param name='b'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T Max(T a, T b, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.Max(a, b, ctx) : this.simp.Max(a, b, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='a'>A T object. (2).</param>
    /// <param name='b'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T Min(T a, T b, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.Min(a, b, ctx) : this.simp.Min(a, b, ctx);
    }

    /// <summary>Multiplies two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='other'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The product of the two objects.</returns>
public T Multiply(T thisValue, T other, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.Multiply(thisValue, other, ctx) :
        this.simp.Multiply(thisValue, other, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='multiplicand'>A T object. (3).</param>
    /// <param name='augend'>A T object. (4).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T MultiplyAndAdd(T thisValue, T multiplicand, T augend, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.MultiplyAndAdd(thisValue, multiplicand, augend, ctx) :
        this.simp.MultiplyAndAdd(thisValue, multiplicand, augend, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T RoundToBinaryPrecision(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.RoundToBinaryPrecision(thisValue, ctx) :
        this.simp.RoundToBinaryPrecision(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T Plus(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.Plus(thisValue, ctx) :
        this.simp.Plus(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T RoundToPrecision(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.RoundToPrecision(thisValue, ctx) :
        this.simp.RoundToPrecision(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T RoundAfterConversion(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.RoundAfterConversion(thisValue, ctx) :
        this.simp.RoundAfterConversion(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='otherValue'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T Quantize(T thisValue, T otherValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.Quantize(thisValue, otherValue, ctx) :
        this.simp.Quantize(thisValue, otherValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='expOther'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T RoundToExponentExact(T thisValue, BigInteger expOther, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.RoundToExponentExact(thisValue, expOther, ctx) :
        this.simp.RoundToExponentExact(thisValue, expOther, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='expOther'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T RoundToExponentSimple(T thisValue, BigInteger expOther, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.RoundToExponentSimple(thisValue, expOther, ctx) :
        this.simp.RoundToExponentSimple(thisValue, expOther, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='exponent'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T RoundToExponentNoRoundedFlag(T thisValue, BigInteger exponent, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.RoundToExponentNoRoundedFlag(thisValue, exponent, ctx) :
        this.simp.RoundToExponentNoRoundedFlag(thisValue, exponent, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T Reduce(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.Reduce(thisValue, ctx) :
        this.simp.Reduce(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='other'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
public T Add(T thisValue, T other, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.Add(thisValue, other, ctx) :
        this.simp.Add(thisValue, other, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='other'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <param name='roundToOperandPrecision'>A Boolean object.</param>
    /// <returns>A T object.</returns>
public T AddEx(T thisValue, T other, PrecisionContext ctx, bool roundToOperandPrecision) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.AddEx(thisValue, other, ctx, roundToOperandPrecision) :
        this.simp.AddEx(thisValue, other, ctx, roundToOperandPrecision);
    }

    /// <summary>Compares a T object with this instance.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='otherValue'>A T object. (2).</param>
    /// <param name='treatQuietNansAsSignaling'>A Boolean object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
public T CompareToWithContext(T thisValue, T otherValue, bool treatQuietNansAsSignaling, PrecisionContext ctx) {
      return (ctx == null || !ctx.IsSimplified) ? this.ext.CompareToWithContext(thisValue, otherValue, treatQuietNansAsSignaling, ctx) :
        this.simp.CompareToWithContext(thisValue, otherValue, treatQuietNansAsSignaling, ctx);
    }

    /// <summary>Compares a T object with this instance.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='otherValue'>A T object. (2).</param>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
public int CompareTo(T thisValue, T otherValue) {
      return this.ext.CompareTo(thisValue, otherValue);
    }
  }
}
