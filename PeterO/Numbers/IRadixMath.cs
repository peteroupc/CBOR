/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Numbers {
  internal interface IRadixMath<T> {
    IRadixMathHelper<T> GetHelper();

    T DivideToIntegerNaturalScale(T thisValue, T divisor, EContext ctx);

    T DivideToIntegerZeroScale(T thisValue, T divisor, EContext ctx);

    T Abs(T value, EContext ctx);

    T Negate(T value, EContext ctx);

    T Remainder(T thisValue, T divisor, EContext ctx);

    T RemainderNear(T thisValue, T divisor, EContext ctx);

    T Pi(EContext ctx);

    T Power(T thisValue, T pow, EContext ctx);

    T Log10(T thisValue, EContext ctx);

    T Ln(T thisValue, EContext ctx);

    T Exp(T thisValue, EContext ctx);

    T SquareRoot(T thisValue, EContext ctx);

    T NextMinus(T thisValue, EContext ctx);

    T NextToward(T thisValue, T otherValue, EContext ctx);

    T NextPlus(T thisValue, EContext ctx);

    T DivideToExponent(
T thisValue,
T divisor,
EInteger desiredExponent,
EContext ctx);

    T Divide(T thisValue, T divisor, EContext ctx);

    T MinMagnitude(T a, T b, EContext ctx);

    T MaxMagnitude(T a, T b, EContext ctx);

    T Max(T a, T b, EContext ctx);

    T Min(T a, T b, EContext ctx);

    T Multiply(T thisValue, T other, EContext ctx);

    T MultiplyAndAdd(
T thisValue,
T multiplicand,
T augend,
EContext ctx);

    T Plus(T thisValue, EContext ctx);

    T RoundToPrecision(T thisValue, EContext ctx);

    T RoundAfterConversion(T thisValue, EContext ctx);

    T Quantize(T thisValue, T otherValue, EContext ctx);

    T RoundToExponentExact(
T thisValue,
EInteger expOther,
EContext ctx);

    T RoundToExponentSimple(
T thisValue,
EInteger expOther,
EContext ctx);

    T RoundToExponentNoRoundedFlag(
T thisValue,
EInteger exponent,
EContext ctx);

    T Reduce(T thisValue, EContext ctx);

    T Add(T thisValue, T other, EContext ctx);

    T AddEx(
T thisValue,
T other,
EContext ctx,
bool roundToOperandPrecision);

    T CompareToWithContext(
T thisValue,
T otherValue,
bool treatQuietNansAsSignaling,
EContext ctx);

    int CompareTo(T thisValue, T otherValue);
  }
}
