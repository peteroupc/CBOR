/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;

namespace PeterO {
  internal sealed class SimpleRadixMath<T> : IRadixMath<T> {
    private IRadixMath<T> wrapper;

    public SimpleRadixMath(IRadixMath<T> wrapper) {
      this.wrapper = wrapper;
    }

    private T PostProcess(T thisValue, PrecisionContext ctx) {
      int thisFlags = this.GetHelper().GetFlags(thisValue);
      if ((thisFlags & BigNumberFlags.FlagSpecial) != 0) {
        return thisValue;
      }
      BigInteger mant = BigInteger.Abs(this.GetHelper().GetMantissa(thisValue));
      if (mant.IsZero) {
 return this.GetHelper().ValueOf(0);
}
      BigInteger exp = this.GetHelper().GetExponent(thisValue);
      if (exp.Sign > 0) {
        FastInteger fastExp = FastInteger.FromBig(exp);
        if (ctx == null || ctx.Precision.IsZero) {
          mant = this.GetHelper().MultiplyByRadixPower(mant, fastExp);
          return this.GetHelper().CreateNewWithFlags(mant, BigInteger.Zero, thisFlags);
        }
        if (!ctx.ExponentWithinRange(exp)) {
 return thisValue;
}
        FastInteger prec = FastInteger.FromBig(ctx.Precision);
        FastInteger digits = this.GetHelper().CreateShiftAccumulator(mant).GetDigitLength();
        prec.Subtract(digits);
        if (prec.Sign > 0 && prec.CompareTo(fastExp) >= 0) {
          mant = this.GetHelper().MultiplyByRadixPower(mant, fastExp);
          return this.GetHelper().CreateNewWithFlags(mant, BigInteger.Zero, thisFlags);
        }
        return thisValue;
      }
      return thisValue;
    }

    private T ReturnQuietNaN(T thisValue, PrecisionContext ctx) {
      BigInteger mant = BigInteger.Abs(this.GetHelper().GetMantissa(thisValue));
      bool mantChanged = false;
      if (!mant.IsZero && ctx != null && !ctx.Precision.IsZero) {
        BigInteger limit = this.GetHelper().MultiplyByRadixPower(BigInteger.One, FastInteger.FromBig(ctx.Precision));
        if (mant.CompareTo(limit) >= 0) {
          mant %= (BigInteger)limit;
          mantChanged = true;
        }
      }
      int flags = this.GetHelper().GetFlags(thisValue);
      if (!mantChanged && (flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return thisValue;
      }
      flags &= BigNumberFlags.FlagNegative;
      flags |= BigNumberFlags.FlagQuietNaN;
      return this.GetHelper().CreateNewWithFlags(mant, BigInteger.Zero, flags);
    }

    private T HandleNotANumber(T thisValue, T other, PrecisionContext ctx) {
      int thisFlags = this.GetHelper().GetFlags(thisValue);
      int otherFlags = this.GetHelper().GetFlags(other);
      // Check this value then the other value for signaling NaN
      if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(thisValue, ctx);
      }
      if ((otherFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(other, ctx);
      }
      // Check this value then the other value for quiet NaN
      if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.ReturnQuietNaN(thisValue, ctx);
      }
      if ((otherFlags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.ReturnQuietNaN(other, ctx);
      }
      return default(T);
    }

    private T SignalingNaNInvalid(T value, PrecisionContext ctx) {
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= PrecisionContext.FlagInvalid;
      }
      return this.ReturnQuietNaN(value, ctx);
    }

    private T CheckNotANumber1(T val, PrecisionContext ctx) {
      return this.HandleNotANumber(val, val, ctx);
    }

    private T CheckNotANumber2(T val, T val2, PrecisionContext ctx) {
      return this.HandleNotANumber(val, val2, ctx);
    }

    private T CheckNotANumber3(T val, T val2, T val3, PrecisionContext ctx) {
      throw new NotImplementedException();
    }

    private T RoundBeforeOp(T val, PrecisionContext ctx) {
      if (ctx == null || ctx.Precision.IsZero) {
        return val;
      }
      PrecisionContext ctx2 = ctx.WithUnlimitedExponents().WithBlankFlags().WithTraps(0);
      val = this.wrapper.RoundToPrecision(val, ctx);
      // the only time rounding can signal an invalid
      // operation is if an operand is signaling NaN, but
      // this was already checked beforehand
      #if DEBUG
      if (!((ctx2.Flags & PrecisionContext.FlagInvalid) == 0)) {
        throw new ArgumentException("doesn't satisfy (ctx2.Flags&PrecisionContext.FlagInvalid)==0");
      }
      #endif

      if ((ctx2.Flags & PrecisionContext.FlagInexact) != 0) {
        if (ctx.HasFlags) {
          ctx.Flags |= PrecisionContext.FlagLostDigits;
        }
      }
      return val;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='divisor'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T DivideToIntegerNaturalScale(T thisValue, T divisor, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      divisor = this.RoundBeforeOp(divisor, ctx);
      throw new NotImplementedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='divisor'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T DivideToIntegerZeroScale(T thisValue, T divisor, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      divisor = this.RoundBeforeOp(divisor, ctx);
      thisValue = this.wrapper.DivideToIntegerZeroScale(thisValue, divisor, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Abs(T value, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(value, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      value = this.RoundBeforeOp(value, ctx);
      value = this.wrapper.Abs(value, ctx);
      return this.PostProcess(value, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Negate(T value, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(value, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      value = this.Negate(value, ctx);
      value = this.wrapper.Negate(value, ctx);
      return this.PostProcess(value, ctx);
    }

    /// <summary>Finds the remainder that results when dividing two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The remainder of the two objects.</returns>
    public T Remainder(T thisValue, T divisor, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      divisor = this.RoundBeforeOp(divisor, ctx);
      thisValue = this.wrapper.Remainder(thisValue, divisor, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='divisor'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RemainderNear(T thisValue, T divisor, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      divisor = this.RoundBeforeOp(divisor, ctx);
      thisValue = this.wrapper.RemainderNear(thisValue, divisor, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Pi(PrecisionContext ctx) {
      return this.wrapper.Pi(ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='pow'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Power(T thisValue, T pow, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, pow, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      pow = this.RoundBeforeOp(pow, ctx);
      thisValue = this.wrapper.Power(thisValue, pow, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Log10(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.Log10(thisValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Ln(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.Ln(thisValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An IRadixMathHelper(T) object.</returns>
    public IRadixMathHelper<T> GetHelper() {
      return this.wrapper.GetHelper();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Exp(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.Exp(thisValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T SquareRoot(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.SquareRoot(thisValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T NextMinus(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.NextMinus(thisValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='otherValue'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T NextToward(T thisValue, T otherValue, PrecisionContext ctx) {
      throw new NotImplementedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T NextPlus(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.NextPlus(thisValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='divisor'>A T object. (3).</param>
    /// <param name='desiredExponent'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T DivideToExponent(T thisValue, T divisor, BigInteger desiredExponent, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      divisor = this.RoundBeforeOp(divisor, ctx);
      thisValue = this.DivideToExponent(thisValue, divisor, desiredExponent, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Divides two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The quotient of the two objects.</returns>
    public T Divide(T thisValue, T divisor, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      divisor = this.RoundBeforeOp(divisor, ctx);
      thisValue = this.Divide(thisValue, divisor, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='a'>A T object. (2).</param>
    /// <param name='b'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T MinMagnitude(T a, T b, PrecisionContext ctx) {
      throw new NotImplementedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='a'>A T object. (2).</param>
    /// <param name='b'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T MaxMagnitude(T a, T b, PrecisionContext ctx) {
      throw new NotImplementedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='a'>A T object. (2).</param>
    /// <param name='b'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Max(T a, T b, PrecisionContext ctx) {
      throw new NotImplementedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='a'>A T object. (2).</param>
    /// <param name='b'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Min(T a, T b, PrecisionContext ctx) {
      throw new NotImplementedException();
    }

    /// <summary>Multiplies two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='other'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The product of the two objects.</returns>
    public T Multiply(T thisValue, T other, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, other, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      throw new NotImplementedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='multiplicand'>A T object. (3).</param>
    /// <param name='augend'>A T object. (4).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T MultiplyAndAdd(T thisValue, T multiplicand, T augend, PrecisionContext ctx) {
      throw new NotImplementedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RoundToBinaryPrecision(T thisValue, PrecisionContext ctx) {
      throw new NotImplementedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Plus(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.Plus(thisValue, ctx);
      throw new NotImplementedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RoundToPrecision(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.RoundToPrecision(thisValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='otherValue'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Quantize(T thisValue, T otherValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.Quantize(thisValue, otherValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='expOther'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RoundToExponentExact(T thisValue, BigInteger expOther, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.RoundToExponentExact(thisValue, expOther, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='expOther'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RoundToExponentSimple(T thisValue, BigInteger expOther, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.RoundToExponentSimple(thisValue, expOther, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='exponent'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RoundToExponentNoRoundedFlag(T thisValue, BigInteger exponent, PrecisionContext ctx) {
      throw new NotImplementedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Reduce(T thisValue, PrecisionContext ctx) {
      throw new NotImplementedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='thisValue'>A T object. (2).</param>
    /// <param name='other'>A T object. (3).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Add(T thisValue, T other, PrecisionContext ctx) {
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      other = this.RoundBeforeOp(other, ctx);
      throw new NotImplementedException();
    }

    /// <summary>Compares a T object with this instance.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='otherValue'>A T object. (2).</param>
    /// <param name='treatQuietNansAsSignaling'>A Boolean object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    public T CompareToWithContext(T thisValue, T otherValue, bool treatQuietNansAsSignaling, PrecisionContext ctx) {
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      otherValue = this.RoundBeforeOp(otherValue, ctx);
      return this.CompareToWithContext(thisValue, otherValue, treatQuietNansAsSignaling, ctx);
    }

    /// <summary>Compares a T object with this instance.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='otherValue'>A T object. (2).</param>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    public int CompareTo(T thisValue, T otherValue) {
      return this.wrapper.CompareTo(thisValue, otherValue);
    }
  }
}
