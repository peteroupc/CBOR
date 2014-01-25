/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;

namespace PeterO
{
    /// <summary>Description of TrappableRadixMath.</summary>
    /// <typeparam name='T'>Data type for a numeric value in a particular
    /// radix.</typeparam>
  internal class TrappableRadixMath<T> : IRadixMath<T>
  {
    private static PrecisionContext GetTrappableContext(PrecisionContext ctx) {
      if (ctx == null) {
        return null;
      }
      if (ctx.Traps == 0) {
        return ctx;
      }
      return ctx.WithBlankFlags();
    }

    private T TriggerTraps(T result, PrecisionContext src, PrecisionContext dst) {
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
        throw new TrapException(traps & PrecisionContext.FlagSubnormal, dst, result);
      }
      if ((traps & PrecisionContext.FlagInexact) != 0) {
        throw new TrapException(traps & PrecisionContext.FlagInexact, dst, result);
      }
      if ((traps & PrecisionContext.FlagRounded) != 0) {
        throw new TrapException(traps & PrecisionContext.FlagRounded, dst, result);
      }
      if ((traps & PrecisionContext.FlagClamped) != 0) {
        throw new TrapException(traps & PrecisionContext.FlagClamped, dst, result);
      }
      return result;
    }

    private IRadixMath<T> math;

    public TrappableRadixMath(IRadixMath<T> math) {
      this.math = math;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='divisor'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T DivideToIntegerNaturalScale(T thisValue, T divisor, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.DivideToIntegerNaturalScale(thisValue, divisor, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='divisor'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T DivideToIntegerZeroScale(T thisValue, T divisor, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.DivideToIntegerZeroScale(thisValue, divisor, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Abs(T value, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Abs(value, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Negate(T value, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Negate(value, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Finds the remainder that results when dividing two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The remainder of the two objects.</returns>
    public T Remainder(T thisValue, T divisor, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Remainder(thisValue, divisor, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='divisor'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RemainderNear(T thisValue, T divisor, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.RemainderNear(thisValue, divisor, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Pi(PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Pi(tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='pow'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Power(T thisValue, T pow, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Power(thisValue, pow, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Log10(T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Log10(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Ln(T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Ln(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Exp(T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Exp(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T SquareRoot(T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.SquareRoot(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T NextMinus(T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.NextMinus(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='otherValue'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T NextToward(T thisValue, T otherValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.NextToward(thisValue, otherValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T NextPlus(T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.NextPlus(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='divisor'>A T object. (3).</param>
    /// <param name='desiredExponent'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T DivideToExponent(T thisValue, T divisor, BigInteger desiredExponent, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.DivideToExponent(thisValue, divisor, desiredExponent, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Divides two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The quotient of the two objects.</returns>
    public T Divide(T thisValue, T divisor, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Divide(thisValue, divisor, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='a'>A T object. (2).</param>
    /// <param name='b'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T MinMagnitude(T a, T b, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.MinMagnitude(a, b, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='a'>A T object. (2).</param>
    /// <param name='b'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T MaxMagnitude(T a, T b, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.MaxMagnitude(a, b, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='a'>A T object. (2).</param>
    /// <param name='b'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Max(T a, T b, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Max(a, b, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='a'>A T object. (2).</param>
    /// <param name='b'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Min(T a, T b, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Min(a, b, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Multiplies two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='other'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The product of the two objects.</returns>
    public T Multiply(T thisValue, T other, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Multiply(thisValue, other, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='multiplicand'>A T object. (3).</param>
    /// <param name='augend'>A T object. (4).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T MultiplyAndAdd(T thisValue, T multiplicand, T augend, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.MultiplyAndAdd(thisValue, multiplicand, augend, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RoundToBinaryPrecision(T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.RoundToBinaryPrecision(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Plus(T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Plus(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RoundToPrecision(T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.RoundToPrecision(thisValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='otherValue'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Quantize(T thisValue, T otherValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Quantize(thisValue, otherValue, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='expOther'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RoundToExponentExact(T thisValue, BigInteger expOther, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.RoundToExponentExact(thisValue, expOther, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='expOther'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RoundToExponentSimple(T thisValue, BigInteger expOther, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.RoundToExponentSimple(thisValue, expOther, ctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='exponent'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RoundToExponentNoRoundedFlag(T thisValue, BigInteger exponent, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.RoundToExponentNoRoundedFlag(thisValue, exponent, ctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Reduce(T thisValue, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Reduce(thisValue, ctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='other'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Add(T thisValue, T other, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.Add(thisValue, other, tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Compares a T object with this instance.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='numberObject'>A T object. (2).</param>
    /// <param name='treatQuietNansAsSignaling'>A Boolean object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    public T CompareToWithContext(T thisValue, T numberObject, bool treatQuietNansAsSignaling, PrecisionContext ctx) {
      PrecisionContext tctx = GetTrappableContext(ctx);
      T result = this.math.CompareToWithContext(
        thisValue,
        numberObject,
        treatQuietNansAsSignaling,
        tctx);
      return this.TriggerTraps(result, tctx, ctx);
    }

    /// <summary>Compares a T object with this instance.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='numberObject'>A T object. (2).</param>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    public int CompareTo(T thisValue, T numberObject) {
      return this.math.CompareTo(thisValue, numberObject);
    }
  }
}
