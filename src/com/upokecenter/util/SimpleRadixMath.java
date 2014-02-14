package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

  final class SimpleRadixMath<T> implements IRadixMath<T> {
    private IRadixMath<T> wrapper;

    public SimpleRadixMath (IRadixMath<T> wrapper) {
      this.wrapper = wrapper;
    }

    private T PostProcess(T thisValue, PrecisionContext ctx) {
      int thisFlags = this.GetHelper().GetFlags(thisValue);
      if ((thisFlags & BigNumberFlags.FlagSpecial) != 0) {
        return thisValue;
      }
      BigInteger mant = (this.GetHelper().GetMantissa(thisValue)).abs();
      if (mant.signum()==0) {
 return this.GetHelper().ValueOf(0);
}
      BigInteger exp = this.GetHelper().GetExponent(thisValue);
      if (exp.signum() > 0) {
        FastInteger fastExp = FastInteger.FromBig(exp);
        if (ctx == null || ctx.getPrecision().signum()==0) {
          mant = this.GetHelper().MultiplyByRadixPower(mant, fastExp);
          return this.GetHelper().CreateNewWithFlags(mant, BigInteger.ZERO, thisFlags);
        }
        if (!ctx.ExponentWithinRange(exp)) {
 return thisValue;
}
        FastInteger prec = FastInteger.FromBig(ctx.getPrecision());
        FastInteger digits = this.GetHelper().CreateShiftAccumulator(mant).GetDigitLength();
        prec.Subtract(digits);
        if (prec.signum() > 0 && prec.compareTo(fastExp) >= 0) {
          mant = this.GetHelper().MultiplyByRadixPower(mant, fastExp);
          return this.GetHelper().CreateNewWithFlags(mant, BigInteger.ZERO, thisFlags);
        }
        return thisValue;
      }
      return thisValue;
    }

    private T ReturnQuietNaN(T thisValue, PrecisionContext ctx) {
      BigInteger mant = (this.GetHelper().GetMantissa(thisValue)).abs();
      boolean mantChanged = false;
      if (mant.signum()!=0 && ctx != null && ctx.getPrecision().signum()!=0) {
        BigInteger limit = this.GetHelper().MultiplyByRadixPower(BigInteger.ONE, FastInteger.FromBig(ctx.getPrecision()));
        if (mant.compareTo(limit) >= 0) {
          mant=mant.remainder(limit);
          mantChanged = true;
        }
      }
      int flags = this.GetHelper().GetFlags(thisValue);
      if (!mantChanged && (flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return thisValue;
      }
      flags &= BigNumberFlags.FlagNegative;
      flags |= BigNumberFlags.FlagQuietNaN;
      return this.GetHelper().CreateNewWithFlags(mant, BigInteger.ZERO, flags);
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
      return null;
    }

    private T SignalingNaNInvalid(T value, PrecisionContext ctx) {
      if (ctx != null && ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags()|(PrecisionContext.FlagInvalid));
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
      throw new UnsupportedOperationException();
    }

    private T RoundBeforeOp(T val, PrecisionContext ctx) {
      if (ctx == null || ctx.getPrecision().signum()==0) {
        return val;
      }
      PrecisionContext ctx2 = ctx.WithUnlimitedExponents().WithBlankFlags().WithTraps(0);
      val = this.wrapper.RoundToPrecision(val, ctx);
      // the only time rounding can signal an invalid
      // operation is if an operand is signaling NaN, but
      // this was already checked beforehand

      if ((ctx2.getFlags() & PrecisionContext.FlagInexact) != 0) {
        if (ctx.getHasFlags()) {
          ctx.setFlags(ctx.getFlags()|(PrecisionContext.FlagLostDigits));
        }
      }
      return val;
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param divisor A T object. (3).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T DivideToIntegerNaturalScale(T thisValue, T divisor, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      divisor = this.RoundBeforeOp(divisor, ctx);
      throw new UnsupportedOperationException();
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param divisor A T object. (3).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T DivideToIntegerZeroScale(T thisValue, T divisor, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      divisor = this.RoundBeforeOp(divisor, ctx);
      thisValue = this.wrapper.DivideToIntegerZeroScale(thisValue, divisor, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Not documented yet.
     * @param value A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Abs(T value, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(value, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      value = this.RoundBeforeOp(value, ctx);
      value = this.wrapper.Abs(value, ctx);
      return this.PostProcess(value, ctx);
    }

    /**
     * Not documented yet.
     * @param value A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Negate(T value, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(value, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      value = this.Negate(value, ctx);
      value = this.wrapper.Negate(value, ctx);
      return this.PostProcess(value, ctx);
    }

    /**
     * Finds the remainder that results when dividing two T objects.
     * @param thisValue A T object.
     * @param divisor A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return The remainder of the two objects.
     */
    public T Remainder(T thisValue, T divisor, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      divisor = this.RoundBeforeOp(divisor, ctx);
      thisValue = this.wrapper.Remainder(thisValue, divisor, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param divisor A T object. (3).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T RemainderNear(T thisValue, T divisor, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      divisor = this.RoundBeforeOp(divisor, ctx);
      thisValue = this.wrapper.RemainderNear(thisValue, divisor, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Not documented yet.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Pi(PrecisionContext ctx) {
      return this.wrapper.Pi(ctx);
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param pow A T object. (3).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Power(T thisValue, T pow, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, pow, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      pow = this.RoundBeforeOp(pow, ctx);
      thisValue = this.wrapper.Power(thisValue, pow, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Log10(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.Log10(thisValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Ln(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.Ln(thisValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Not documented yet.
     * @return An IRadixMathHelper(T) object.
     */
    public IRadixMathHelper<T> GetHelper() {
      return this.wrapper.GetHelper();
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Exp(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.Exp(thisValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T SquareRoot(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.SquareRoot(thisValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T NextMinus(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.NextMinus(thisValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param otherValue A T object. (3).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T NextToward(T thisValue, T otherValue, PrecisionContext ctx) {
      throw new UnsupportedOperationException();
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T NextPlus(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.NextPlus(thisValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param divisor A T object. (3).
     * @param desiredExponent A BigInteger object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T DivideToExponent(T thisValue, T divisor, BigInteger desiredExponent, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      divisor = this.RoundBeforeOp(divisor, ctx);
      thisValue = this.DivideToExponent(thisValue, divisor, desiredExponent, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Divides two T objects.
     * @param thisValue A T object.
     * @param divisor A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return The quotient of the two objects.
     */
    public T Divide(T thisValue, T divisor, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      divisor = this.RoundBeforeOp(divisor, ctx);
      thisValue = this.Divide(thisValue, divisor, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Not documented yet.
     * @param a A T object. (2).
     * @param b A T object. (3).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T MinMagnitude(T a, T b, PrecisionContext ctx) {
      throw new UnsupportedOperationException();
    }

    /**
     * Not documented yet.
     * @param a A T object. (2).
     * @param b A T object. (3).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T MaxMagnitude(T a, T b, PrecisionContext ctx) {
      throw new UnsupportedOperationException();
    }

    /**
     * Not documented yet.
     * @param a A T object. (2).
     * @param b A T object. (3).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Max(T a, T b, PrecisionContext ctx) {
      throw new UnsupportedOperationException();
    }

    /**
     * Not documented yet.
     * @param a A T object. (2).
     * @param b A T object. (3).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Min(T a, T b, PrecisionContext ctx) {
      throw new UnsupportedOperationException();
    }

    /**
     * Multiplies two T objects.
     * @param thisValue A T object.
     * @param other A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return The product of the two objects.
     */
    public T Multiply(T thisValue, T other, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, other, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      throw new UnsupportedOperationException();
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param multiplicand A T object. (3).
     * @param augend A T object. (4).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T MultiplyAndAdd(T thisValue, T multiplicand, T augend, PrecisionContext ctx) {
      throw new UnsupportedOperationException();
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T RoundToBinaryPrecision(T thisValue, PrecisionContext ctx) {
      throw new UnsupportedOperationException();
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Plus(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.Plus(thisValue, ctx);
      throw new UnsupportedOperationException();
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T RoundToPrecision(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.RoundToPrecision(thisValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param otherValue A T object. (3).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Quantize(T thisValue, T otherValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.Quantize(thisValue, otherValue, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param expOther A BigInteger object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T RoundToExponentExact(T thisValue, BigInteger expOther, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.RoundToExponentExact(thisValue, expOther, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param expOther A BigInteger object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T RoundToExponentSimple(T thisValue, BigInteger expOther, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      thisValue = this.wrapper.RoundToExponentSimple(thisValue, expOther, ctx);
      return this.PostProcess(thisValue, ctx);
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param exponent A BigInteger object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T RoundToExponentNoRoundedFlag(T thisValue, BigInteger exponent, PrecisionContext ctx) {
      throw new UnsupportedOperationException();
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Reduce(T thisValue, PrecisionContext ctx) {
      throw new UnsupportedOperationException();
    }

    /**
     * Not documented yet.
     * @param thisValue A T object. (2).
     * @param other A T object. (3).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Add(T thisValue, T other, PrecisionContext ctx) {
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      other = this.RoundBeforeOp(other, ctx);
      throw new UnsupportedOperationException();
    }

    /**
     * Compares a T object with this instance.
     * @param thisValue A T object.
     * @param otherValue A T object. (2).
     * @param treatQuietNansAsSignaling A Boolean object.
     * @param ctx A PrecisionContext object.
     * @return Zero if the values are equal; a negative number if this instance
     * is less, or a positive number if this instance is greater.
     */
    public T CompareToWithContext(T thisValue, T otherValue, boolean treatQuietNansAsSignaling, PrecisionContext ctx) {
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      otherValue = this.RoundBeforeOp(otherValue, ctx);
      return this.CompareToWithContext(thisValue, otherValue, treatQuietNansAsSignaling, ctx);
    }

    /**
     * Compares a T object with this instance.
     * @param thisValue A T object.
     * @param otherValue A T object. (2).
     * @return Zero if the values are equal; a negative number if this instance
     * is less, or a positive number if this instance is greater.
     */
    public int compareTo(T thisValue, T otherValue) {
      return this.wrapper.compareTo(thisValue, otherValue);
    }
  }
