/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using PeterO;

namespace PeterO.Numbers {
  // <summary>Implements arithmetic methods that support
  // traps.</summary>
  // <typeparam name='T'>Data type for a numeric value in a particular
  // radix.</typeparam>
  internal class TrappableRadixMath<T> : IRadixMath<T> {
    private static void ThrowTrapException(
    int flag,
    EContext ctx,
    object result) {
      throw new ETrapException(flag, ctx, result);
    }

    private static EContext GetTrappableContext(EContext ctx) {
      return (ctx == null) ? null : ((ctx.Traps == 0) ? ctx :
      ctx.WithBlankFlags());
    }

    private T TriggerTraps(
T result,
EContext src,
EContext dst) {
      if (src == null || src.Flags == 0) {
        return result;
      }
      if (dst != null && dst.HasFlags) {
        dst.Flags |= src.Flags;
      }
      int traps = (dst != null) ? dst.Traps : 0;
      traps &= src.Flags;
      if (traps == 0) {
        return result;
      }
      int mutexConditions = traps & (~(
        EContext.FlagClamped | EContext.FlagInexact | EContext.FlagRounded | EContext.FlagSubnormal));
      if (mutexConditions != 0) {
        for (var i = 0; i < 32; ++i) {
          int flag = mutexConditions & (i << 1);
          if (flag != 0) {
            ThrowTrapException(flag, dst, result);
          }
        }
      }
      if ((traps & EContext.FlagSubnormal) != 0) {
        ThrowTrapException(
traps & EContext.FlagSubnormal,
dst,
result);
      }
      if ((traps & EContext.FlagInexact) != 0) {
        ThrowTrapException(
traps & EContext.FlagInexact,
dst,
result);
      }
      if ((traps & EContext.FlagRounded) != 0) {
        ThrowTrapException(
traps & EContext.FlagRounded,
dst,
result);
      }
      if ((traps & EContext.FlagClamped) != 0) {
        ThrowTrapException(
traps & EContext.FlagClamped,
dst,
result);
      }
      return result;
    }

    private readonly IRadixMath<T> math;

    public TrappableRadixMath(IRadixMath<T> math) {
#if DEBUG
      if (math == null) {
        throw new ArgumentNullException("math");
      }
#endif
      this.math = math;
    }

    public T DivideToIntegerNaturalScale(
T thisValue,
T divisor,
EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.DivideToIntegerNaturalScale(
thisValue,
divisor,
tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T DivideToIntegerZeroScale(
T thisValue,
T divisor,
EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.DivideToIntegerZeroScale(thisValue, divisor, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Abs(T value, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Abs(value, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Negate(T value, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Negate(value, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    // <summary>Finds the remainder that results when dividing two T
    // objects.</summary>
    // <param name='thisValue'></param>
    // <summary>Finds the remainder that results when dividing two T
    // objects.</summary>
    // <param name='thisValue'></param>
    // <param name='divisor'></param>
    // <param name='ctx'> (3).</param>
    // <returns>The remainder of the two objects.</returns>
    public T Remainder(T thisValue, T divisor, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Remainder(thisValue, divisor, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public IRadixMathHelper<T> GetHelper() {
      return this.math.GetHelper();
    }

    public T RemainderNear(T thisValue, T divisor, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.RemainderNear(thisValue, divisor, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Pi(EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Pi(tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Power(T thisValue, T pow, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Power(thisValue, pow, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Log10(T thisValue, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Log10(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Ln(T thisValue, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Ln(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Exp(T thisValue, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Exp(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T SquareRoot(T thisValue, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.SquareRoot(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T NextMinus(T thisValue, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.NextMinus(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T NextToward(T thisValue, T otherValue, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.NextToward(thisValue, otherValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T NextPlus(T thisValue, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.NextPlus(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T DivideToExponent(
T thisValue,
T divisor,
EInteger desiredExponent,
EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.DivideToExponent(
thisValue,
divisor,
desiredExponent,
tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    // <summary>Divides two T objects.</summary>
    // <param name='thisValue'></param>
    // <summary>Divides two T objects.</summary>
    // <param name='thisValue'></param>
    // <param name='divisor'></param>
    // <param name='ctx'> (3).</param>
    // <returns>The quotient of the two objects.</returns>
    public T Divide(T thisValue, T divisor, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Divide(thisValue, divisor, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T MinMagnitude(T a, T b, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.MinMagnitude(a, b, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T MaxMagnitude(T a, T b, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.MaxMagnitude(a, b, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Max(T a, T b, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Max(a, b, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Min(T a, T b, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Min(a, b, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    // <summary>Multiplies two T objects.</summary>
    // <param name='thisValue'></param>
    // <summary>Multiplies two T objects.</summary>
    // <param name='thisValue'></param>
    // <param name='other'></param>
    // <param name='ctx'> (3).</param>
    // <returns>The product of the two objects.</returns>
    public T Multiply(T thisValue, T other, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Multiply(thisValue, other, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T MultiplyAndAdd(
T thisValue,
T multiplicand,
T augend,
EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.MultiplyAndAdd(
thisValue,
multiplicand,
augend,
tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Plus(T thisValue, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Plus(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T RoundToPrecision(T thisValue, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.RoundToPrecision(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Quantize(T thisValue, T otherValue, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Quantize(thisValue, otherValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T RoundToExponentExact(
T thisValue,
EInteger expOther,
EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.RoundToExponentExact(thisValue, expOther, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T RoundToExponentSimple(
T thisValue,
EInteger expOther,
EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.RoundToExponentSimple(thisValue, expOther, ctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T RoundToExponentNoRoundedFlag(
T thisValue,
EInteger exponent,
EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.RoundToExponentNoRoundedFlag(
thisValue,
exponent,
ctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Reduce(T thisValue, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Reduce(thisValue, ctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T Add(T thisValue, T other, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.Add(thisValue, other, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    // <summary>Compares a T object with this instance.</summary>
    // <param name='thisValue'></param>
    // <param name='otherValue'>A T object.</param>
    // <param name='treatQuietNansAsSignaling'>A Boolean object.</param>
    // <param name='ctx'>A PrecisionContext object.</param>
    // <returns>Zero if the values are equal; a negative number if this
    // instance is less, or a positive number if this instance is
    // greater.</returns>
    public T CompareToWithContext(
T thisValue,
T otherValue,
bool treatQuietNansAsSignaling,
EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.CompareToWithContext(
        thisValue,
        otherValue,
        treatQuietNansAsSignaling,
        tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    // <summary>Compares a T object with this instance.</summary>
    // <param name='thisValue'></param>
    // <returns>Zero if the values are equal; a negative number if this
    // instance is less, or a positive number if this instance is
    // greater.</returns>
    public int CompareTo(T thisValue, T otherValue) {
      return this.math.CompareTo(thisValue, otherValue);
    }

    public T RoundAfterConversion(T thisValue, EContext ctx) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.RoundAfterConversion(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    public T AddEx(
    T thisValue,
    T other,
    EContext ctx,
    bool roundToOperandPrecision) {
      EContext tctx = GetTrappableContext(ctx);
      T result = this.math.AddEx(
thisValue,
other,
ctx,
roundToOperandPrecision);
      return this.TriggerTraps(result, tctx, ctx);
    }
  }
}
