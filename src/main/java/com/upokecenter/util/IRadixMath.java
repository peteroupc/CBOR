package com.upokecenter.util;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  interface IRadixMath<T> {
    IRadixMathHelper<T> GetHelper();

    T DivideToIntegerNaturalScale(T thisValue, T divisor, PrecisionContext ctx);

    T DivideToIntegerZeroScale(T thisValue, T divisor, PrecisionContext ctx);

    T Abs(T value, PrecisionContext ctx);

    T Negate(T value, PrecisionContext ctx);

    T Remainder(T thisValue, T divisor, PrecisionContext ctx);

    T RemainderNear(T thisValue, T divisor, PrecisionContext ctx);

    T Pi(PrecisionContext ctx);

    T Power(T thisValue, T pow, PrecisionContext ctx);

    T Log10(T thisValue, PrecisionContext ctx);

    T Ln(T thisValue, PrecisionContext ctx);

    T Exp(T thisValue, PrecisionContext ctx);

    T SquareRoot(T thisValue, PrecisionContext ctx);

    T NextMinus(T thisValue, PrecisionContext ctx);

    T NextToward(T thisValue, T otherValue, PrecisionContext ctx);

    T NextPlus(T thisValue, PrecisionContext ctx);

    T DivideToExponent(
T thisValue,
T divisor,
BigInteger desiredExponent,
PrecisionContext ctx);

    T Divide(T thisValue, T divisor, PrecisionContext ctx);

    T MinMagnitude(T a, T b, PrecisionContext ctx);

    T MaxMagnitude(T a, T b, PrecisionContext ctx);

    T Max(T a, T b, PrecisionContext ctx);

    T Min(T a, T b, PrecisionContext ctx);

    T Multiply(T thisValue, T other, PrecisionContext ctx);

    T MultiplyAndAdd(
T thisValue,
T multiplicand,
T augend,
PrecisionContext ctx);

    T Plus(T thisValue, PrecisionContext ctx);

    T RoundToPrecision(T thisValue, PrecisionContext ctx);

    T RoundAfterConversion(T thisValue, PrecisionContext ctx);

    T Quantize(T thisValue, T otherValue, PrecisionContext ctx);

    T RoundToExponentExact(
T thisValue,
BigInteger expOther,
PrecisionContext ctx);

    T RoundToExponentSimple(
T thisValue,
BigInteger expOther,
PrecisionContext ctx);

    T RoundToExponentNoRoundedFlag(
T thisValue,
BigInteger exponent,
PrecisionContext ctx);

    T Reduce(T thisValue, PrecisionContext ctx);

    T Add(T thisValue, T other, PrecisionContext ctx);

    T AddEx(
T thisValue,
T other,
PrecisionContext ctx,
boolean roundToOperandPrecision);

    T CompareToWithContext(
T thisValue,
T otherValue,
boolean treatQuietNansAsSignaling,
PrecisionContext ctx);

    int compareTo(T thisValue, T otherValue);
  }
