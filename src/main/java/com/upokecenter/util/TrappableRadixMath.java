package com.upokecenter.util;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Implements arithmetic methods that support traps.
     * @param <T> Data type for a numeric value in a particular radix.
     */
  class TrappableRadixMath<T> implements IRadixMath<T> {

    private static PrecisionContext GetTrappableContext(final PrecisionContext ctx) {
      return (ctx == null) ? null : ((ctx.getTraps() == 0) ? ctx :
      ctx.WithBlankFlags());
    }

    private T TriggerTraps(
final T result,
final PrecisionContext src,
final PrecisionContext dst) {
      if (src == null || src.getFlags() == 0) {
        return result;
      }
      if (dst != null && dst.getHasFlags()) {
        dst.setFlags(dst.getFlags() | (src.getFlags()));
      }
      int traps = (dst != null) ? dst.getTraps() : 0;
      traps &= src.getFlags();
      if (traps == 0) {
        return result;
      }
      int mutexConditions = traps & (~(
        PrecisionContext.FlagClamped | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded | PrecisionContext.FlagSubnormal));
      if (mutexConditions != 0) {
        for (int i = 0; i < 32; ++i) {
          int flag = mutexConditions & (i << 1);
          if (flag != 0) {
            throw new TrapException(flag, dst, result);
          }
        }
      }
      if ((traps & PrecisionContext.FlagSubnormal) != 0) {
        throw new TrapException(
traps & PrecisionContext.FlagSubnormal,
dst,
result);
      }
      if ((traps & PrecisionContext.FlagInexact) != 0) {
        throw new TrapException(
traps & PrecisionContext.FlagInexact,
dst,
result);
      }
      if ((traps & PrecisionContext.FlagRounded) != 0) {
        throw new TrapException(
traps & PrecisionContext.FlagRounded,
dst,
result);
      }
      if ((traps & PrecisionContext.FlagClamped) != 0) {
        throw new TrapException(
traps & PrecisionContext.FlagClamped,
dst,
result);
      }
      return result;
    }

    private IRadixMath<T> math;

    public TrappableRadixMath (final IRadixMath<T> math) {
      this.math = math;
    }

    public T DivideToIntegerNaturalScale(
final T thisValue,
final T divisor,
final PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.DivideToIntegerNaturalScale(
thisValue,
divisor,
tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T DivideToIntegerZeroScale(
final T thisValue,
final T divisor,
final PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.DivideToIntegerZeroScale(thisValue, divisor, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Abs(final T value, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Abs(value, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Negate(final T value, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Negate(value, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /**
     * Finds the remainder that results when dividing two T objects.
     * @param thisValue A T object.
     * @param divisor A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return The remainder of the two objects.
     */
    public T Remainder(final T thisValue, T divisor, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Remainder(thisValue, divisor, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public IRadixMathHelper<T> GetHelper() {
      return this.math.GetHelper();
    }

    public T RemainderNear(final T thisValue, T divisor, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.RemainderNear(thisValue, divisor, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Pi(final PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Pi(tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Power(final T thisValue, T pow, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Power(thisValue, pow, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Log10(final T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Log10(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Ln(final T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Ln(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Exp(final T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Exp(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T SquareRoot(final T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.SquareRoot(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T NextMinus(final T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.NextMinus(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T NextToward(final T thisValue, T otherValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.NextToward(thisValue, otherValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T NextPlus(final T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.NextPlus(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T DivideToExponent(
final T thisValue,
final T divisor,
final BigInteger desiredExponent,
final PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.DivideToExponent(
thisValue,
divisor,
desiredExponent,
tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /**
     * Divides two T objects.
     * @param thisValue A T object.
     * @param divisor A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return The quotient of the two objects.
     */
    public T Divide(final T thisValue, T divisor, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Divide(thisValue, divisor, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T MinMagnitude(final T a, T b, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.MinMagnitude(a, b, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T MaxMagnitude(final T a, T b, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.MaxMagnitude(a, b, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Max(final T a, T b, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Max(a, b, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Min(final T a, T b, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Min(a, b, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /**
     * Multiplies two T objects.
     * @param thisValue A T object.
     * @param other A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return The product of the two objects.
     */
    public T Multiply(final T thisValue, T other, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Multiply(thisValue, other, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T MultiplyAndAdd(
final T thisValue,
final T multiplicand,
final T augend,
final PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.MultiplyAndAdd(
thisValue,
multiplicand,
augend,
tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Plus(final T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Plus(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T RoundToPrecision(final T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.RoundToPrecision(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Quantize(final T thisValue, T otherValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Quantize(thisValue, otherValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T RoundToExponentExact(
final T thisValue,
final BigInteger expOther,
final PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.RoundToExponentExact(thisValue, expOther, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T RoundToExponentSimple(
final T thisValue,
final BigInteger expOther,
final PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.RoundToExponentSimple(thisValue, expOther, ctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T RoundToExponentNoRoundedFlag(
final T thisValue,
final BigInteger exponent,
final PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.RoundToExponentNoRoundedFlag(
thisValue,
exponent,
ctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Reduce(final T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Reduce(thisValue, ctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Add(final T thisValue, T other, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Add(thisValue, other, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /**
     * Compares a T object with this instance.
     * @param thisValue A T object.
     * @param otherValue A T object. (2).
     * @param treatQuietNansAsSignaling A Boolean object.
     * @param ctx A PrecisionContext object.
     * @return Zero if the values are equal; a negative number if this instance is
     * less, or a positive number if this instance is greater.
     */
    public T CompareToWithContext(
final T thisValue,
final T otherValue,
final boolean treatQuietNansAsSignaling,
final PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.CompareToWithContext(
        thisValue,
        otherValue,
        treatQuietNansAsSignaling,
        tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /**
     * Compares a T object with this instance.
     * @param thisValue A T object.
     * @param otherValue A T object. (2).
     * @return Zero if the values are equal; a negative number if this instance is
     * less, or a positive number if this instance is greater.
     */
    public int compareTo(final T thisValue, T otherValue) {
      return this.math.compareTo(thisValue, otherValue);
    }

    public T RoundAfterConversion(final T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.RoundAfterConversion(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

public T AddEx(
final T thisValue,
final T other,
final PrecisionContext ctx,
final boolean roundToOperandPrecision) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.AddEx(
thisValue,
other,
ctx,
roundToOperandPrecision);
      return this.TriggerTraps(result, tctx, ctx);
    }
  }
