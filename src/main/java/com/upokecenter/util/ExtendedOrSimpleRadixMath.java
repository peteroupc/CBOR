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

    public ExtendedOrSimpleRadixMath (IRadixMathHelper<T> helper) {
      this.ext = new RadixMath<T>(helper);
      this.simp = new SimpleRadixMath<T>(this.ext);
    }

    public IRadixMathHelper<T> GetHelper() {
      // Both RadixMath implementations return the
      // same helper, so use the ext implementation
      return this.ext.GetHelper();
    }

    public T DivideToIntegerNaturalScale(
T thisValue,
T divisor,
PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.DivideToIntegerNaturalScale(thisValue, divisor, ctx) :
        this.simp.DivideToIntegerNaturalScale(thisValue, divisor, ctx);
    }

    public T DivideToIntegerZeroScale(
T thisValue,
T divisor,
PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.DivideToIntegerZeroScale(thisValue, divisor, ctx) :
        this.simp.DivideToIntegerZeroScale(thisValue, divisor, ctx);
    }

    public T Abs(T value, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ? this.ext.Abs(value, ctx) :
        this.simp.Abs(value, ctx);
    }

    public T Negate(T value, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ? this.ext.Negate(value, ctx) :
        this.simp.Negate(value, ctx);
    }

    public T Remainder(T thisValue, T divisor, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.Remainder(thisValue, divisor, ctx) :
        this.simp.Remainder(thisValue, divisor, ctx);
    }

    public T RemainderNear(T thisValue, T divisor, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.RemainderNear(thisValue, divisor, ctx) :
        this.simp.RemainderNear(thisValue, divisor, ctx);
    }

    public T Pi(PrecisionContext ctx) {
      return (!ctx.isSimplified()) ? this.ext.Pi(ctx) : this.simp.Pi(ctx);
    }

    public T Power(T thisValue, T pow, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.Power(
thisValue,
pow,
ctx) :
        this.simp.Power(thisValue, pow, ctx);
    }

    public T Log10(T thisValue, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.Log10(
thisValue,
ctx) :
        this.simp.Log10(thisValue, ctx);
    }

    public T Ln(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ? this.ext.Ln(thisValue, ctx) :
        this.simp.Ln(thisValue, ctx);
    }

    public T Exp(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ? this.ext.Exp(thisValue, ctx) :
        this.simp.Exp(thisValue, ctx);
    }

    public T SquareRoot(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.SquareRoot(thisValue, ctx) :
        this.simp.SquareRoot(thisValue, ctx);
    }

    public T NextMinus(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.NextMinus(thisValue, ctx) :
        this.simp.NextMinus(thisValue, ctx);
    }

    public T NextToward(T thisValue, T otherValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.NextToward(thisValue, otherValue, ctx) :
        this.simp.NextToward(thisValue, otherValue, ctx);
    }

    public T NextPlus(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.NextPlus(thisValue, ctx) :
        this.simp.NextPlus(thisValue, ctx);
    }

    public T DivideToExponent(
T thisValue,
T divisor,
BigInteger desiredExponent,
PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.DivideToExponent(thisValue, divisor, desiredExponent, ctx) :
        this.simp.DivideToExponent(thisValue, divisor, desiredExponent, ctx);
    }

   public T Divide(T thisValue, T divisor, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.Divide(
thisValue,
divisor,
ctx) :
        this.simp.Divide(thisValue, divisor, ctx);
    }

    public T MinMagnitude(T a, T b, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.MinMagnitude(
a,
b,
ctx) : this.simp.MinMagnitude(a, b, ctx);
    }

    public T MaxMagnitude(T a, T b, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.MaxMagnitude(
a,
b,
ctx) : this.simp.MaxMagnitude(a, b, ctx);
    }

    public T Max(T a, T b, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ? this.ext.Max(a, b, ctx) :
      this.simp.Max(a, b, ctx);
    }

    public T Min(T a, T b, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ? this.ext.Min(a, b, ctx) :
      this.simp.Min(a, b, ctx);
    }

    public T Multiply(T thisValue, T other, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.Multiply(thisValue, other, ctx) :
        this.simp.Multiply(thisValue, other, ctx);
    }

    public T MultiplyAndAdd(
T thisValue,
T multiplicand,
T augend,
PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.MultiplyAndAdd(thisValue, multiplicand, augend, ctx) :
        this.simp.MultiplyAndAdd(thisValue, multiplicand, augend, ctx);
    }

    public T Plus(T thisValue, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.Plus(
thisValue,
ctx) :
        this.simp.Plus(thisValue, ctx);
    }

    public T RoundToPrecision(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.RoundToPrecision(thisValue, ctx) :
        this.simp.RoundToPrecision(thisValue, ctx);
    }

    public T RoundAfterConversion(T thisValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.RoundAfterConversion(thisValue, ctx) :
        this.simp.RoundAfterConversion(thisValue, ctx);
    }

    public T Quantize(T thisValue, T otherValue, PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.Quantize(thisValue, otherValue, ctx) :
        this.simp.Quantize(thisValue, otherValue, ctx);
    }

    public T RoundToExponentExact(
T thisValue,
BigInteger expOther,
PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.RoundToExponentExact(thisValue, expOther, ctx) :
        this.simp.RoundToExponentExact(thisValue, expOther, ctx);
    }

    public T RoundToExponentSimple(
T thisValue,
BigInteger expOther,
PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.RoundToExponentSimple(thisValue, expOther, ctx) :
        this.simp.RoundToExponentSimple(thisValue, expOther, ctx);
    }

    public T RoundToExponentNoRoundedFlag(
T thisValue,
BigInteger exponent,
PrecisionContext ctx) {
      return (ctx == null || !ctx.isSimplified()) ?
      this.ext.RoundToExponentNoRoundedFlag(thisValue, exponent, ctx) :
        this.simp.RoundToExponentNoRoundedFlag(thisValue, exponent, ctx);
    }

    public T Reduce(T thisValue, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.Reduce(
thisValue,
ctx) :
        this.simp.Reduce(thisValue, ctx);
    }

    public T Add(T thisValue, T other, PrecisionContext ctx) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.Add(
thisValue,
other,
ctx) :
        this.simp.Add(thisValue, other, ctx);
    }

    public T AddEx(
T thisValue,
T other,
PrecisionContext ctx,
boolean roundToOperandPrecision) {
      return (
ctx == null || !ctx.isSimplified()) ? this.ext.AddEx(
thisValue,
other,
ctx,
roundToOperandPrecision) :
        this.simp.AddEx(thisValue, other, ctx, roundToOperandPrecision);
    }

    public T CompareToWithContext(
T thisValue,
T otherValue,
boolean treatQuietNansAsSignaling,
PrecisionContext ctx) {
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

    public int compareTo(T thisValue, T otherValue) {
      return this.ext.compareTo(thisValue, otherValue);
    }
  }
