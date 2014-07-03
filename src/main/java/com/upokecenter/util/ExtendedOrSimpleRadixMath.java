package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  class ExtendedOrSimpleRadixMath<T> implements IRadixMath<T> {

    private RadixMath<T> ext;
    private SimpleRadixMath<T> simp;

    public ExtendedOrSimpleRadixMath (final IRadixMathHelper<T> helper) {
      this.ext = new RadixMath<T>(helper);
      this.simp = new SimpleRadixMath<T>(this.ext);
    }

    public IRadixMathHelper<T> GetHelper() {
      // Both RadixMath implementations return the
      // same helper, so use the ext implementation
      return this.ext.GetHelper();
    }

    public T DivideToIntegerNaturalScale(
final T thisValue,
final T divisor,
final PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.DivideToIntegerNaturalScale(thisValue, divisor, ctx) :
        this.simp.DivideToIntegerNaturalScale(thisValue, divisor, ctx);
    }

    public T DivideToIntegerZeroScale(
final T thisValue,
final T divisor,
final PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.DivideToIntegerZeroScale(thisValue, divisor, ctx) :
        this.simp.DivideToIntegerZeroScale(thisValue, divisor, ctx);
    }

    public T Abs(final T value, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ? this.ext.Abs(value, ctx) :
        this.simp.Abs(value, ctx);
    }

    public T Negate(final T value, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ? this.ext.Negate(value, ctx) :
        this.simp.Negate(value, ctx);
    }

    public T Remainder(final T thisValue, T divisor, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.Remainder(thisValue, divisor, ctx) :
        this.simp.Remainder(thisValue, divisor, ctx);
    }

    public T RemainderNear(final T thisValue, T divisor, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.RemainderNear(thisValue, divisor, ctx) :
        this.simp.RemainderNear(thisValue, divisor, ctx);
    }

    public T Pi(final PrecisionContext ctx) {
      return (!ctx.isSimplified()) ? this.ext.Pi(ctx) : this.simp.Pi(ctx);
    }

    public T Power(final T thisValue, T pow, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.Power(
thisValue,
pow,
ctx) :
        this.simp.Power(thisValue, pow, ctx);
    }

    public T Log10(final T thisValue, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.Log10(
thisValue,
ctx) :
        this.simp.Log10(thisValue, ctx);
    }

    public T Ln(final T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ? this.ext.Ln(thisValue, ctx) :
        this.simp.Ln(thisValue, ctx);
    }

    public T Exp(final T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ? this.ext.Exp(thisValue, ctx) :
        this.simp.Exp(thisValue, ctx);
    }

    public T SquareRoot(final T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.SquareRoot(thisValue, ctx) :
        this.simp.SquareRoot(thisValue, ctx);
    }

    public T NextMinus(final T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.NextMinus(thisValue, ctx) :
        this.simp.NextMinus(thisValue, ctx);
    }

    public T NextToward(final T thisValue, T otherValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.NextToward(thisValue, otherValue, ctx) :
        this.simp.NextToward(thisValue, otherValue, ctx);
    }

    public T NextPlus(final T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.NextPlus(thisValue, ctx) :
        this.simp.NextPlus(thisValue, ctx);
    }

    public T DivideToExponent(
final T thisValue,
final T divisor,
final BigInteger desiredExponent,
final PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.DivideToExponent(thisValue, divisor, desiredExponent, ctx) :
        this.simp.DivideToExponent(thisValue, divisor, desiredExponent, ctx);
    }

   public T Divide(final T thisValue, T divisor, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.Divide(
thisValue,
divisor,
ctx) :
        this.simp.Divide(thisValue, divisor, ctx);
    }

    public T MinMagnitude(final T a, T b, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.MinMagnitude(
a,
b,
ctx) : this.simp.MinMagnitude(a, b, ctx);
    }

    public T MaxMagnitude(final T a, T b, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.MaxMagnitude(
a,
b,
ctx) : this.simp.MaxMagnitude(a, b, ctx);
    }

    public T Max(final T a, T b, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ? this.ext.Max(a, b, ctx) :
      this.simp.Max(a, b, ctx);
    }

    public T Min(final T a, T b, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ? this.ext.Min(a, b, ctx) :
      this.simp.Min(a, b, ctx);
    }

    public T Multiply(final T thisValue, T other, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.Multiply(thisValue, other, ctx) :
        this.simp.Multiply(thisValue, other, ctx);
    }

    public T MultiplyAndAdd(
final T thisValue,
final T multiplicand,
final T augend,
final PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.MultiplyAndAdd(thisValue, multiplicand, augend, ctx) :
        this.simp.MultiplyAndAdd(thisValue, multiplicand, augend, ctx);
    }

    public T Plus(final T thisValue, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.Plus(
thisValue,
ctx) :
        this.simp.Plus(thisValue, ctx);
    }

    public T RoundToPrecision(final T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.RoundToPrecision(thisValue, ctx) :
        this.simp.RoundToPrecision(thisValue, ctx);
    }

    public T RoundAfterConversion(final T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.RoundAfterConversion(thisValue, ctx) :
        this.simp.RoundAfterConversion(thisValue, ctx);
    }

    public T Quantize(final T thisValue, T otherValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.Quantize(thisValue, otherValue, ctx) :
        this.simp.Quantize(thisValue, otherValue, ctx);
    }

    public T RoundToExponentExact(
final T thisValue,
final BigInteger expOther,
final PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.RoundToExponentExact(thisValue, expOther, ctx) :
        this.simp.RoundToExponentExact(thisValue, expOther, ctx);
    }

    public T RoundToExponentSimple(
final T thisValue,
final BigInteger expOther,
final PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.RoundToExponentSimple(thisValue, expOther, ctx) :
        this.simp.RoundToExponentSimple(thisValue, expOther, ctx);
    }

    public T RoundToExponentNoRoundedFlag(
final T thisValue,
final BigInteger exponent,
final PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.RoundToExponentNoRoundedFlag(thisValue, exponent, ctx) :
        this.simp.RoundToExponentNoRoundedFlag(thisValue, exponent, ctx);
    }

    public T Reduce(final T thisValue, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.Reduce(
thisValue,
ctx) :
        this.simp.Reduce(thisValue, ctx);
    }

    public T Add(final T thisValue, T other, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.Add(
thisValue,
other,
ctx) :
        this.simp.Add(thisValue, other, ctx);
    }

    public T AddEx(
final T thisValue,
final T other,
final PrecisionContext ctx,
final boolean roundToOperandPrecision) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.AddEx(
thisValue,
other,
ctx,
roundToOperandPrecision) :
        this.simp.AddEx(thisValue, other, ctx, roundToOperandPrecision);
    }

    public T CompareToWithContext(
final T thisValue,
final T otherValue,
final boolean treatQuietNansAsSignaling,
final PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.CompareToWithContext(
thisValue,
otherValue,
treatQuietNansAsSignaling,
ctx) :
        this.simp.CompareToWithContext(
thisValue,
otherValue,
treatQuietNansAsSignaling,
ctx);
    }

    public int compareTo(final T thisValue, T otherValue) {
      return this.ext.compareTo(thisValue, otherValue);
    }
  }
