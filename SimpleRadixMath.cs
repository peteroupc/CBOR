/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;

namespace PeterO {
    /// <summary>Implements the simplified arithmetic in Appendix A of
    /// the General Decimal Arithmetic Specification. Unfortunately,
    /// it doesn't pass all the test cases, since some aspects of the spec are
    /// left open. For example: in which cases is the Clamped flag set? The
    /// test cases set the Clamped flag in only a handful of test cases, all
    /// within the <c>exp</c>
    /// operation.</summary>
    /// <typeparam name='T'>Data type for a numeric value in a particular
    /// radix.</typeparam>
  internal sealed class SimpleRadixMath<T> : IRadixMath<T> {
    private IRadixMath<T> wrapper;

    public SimpleRadixMath(IRadixMath<T> wrapper) {
      this.wrapper = wrapper;
    }

    private static PrecisionContext GetContextWithFlags(PrecisionContext ctx) {
      if (ctx == null) {
        return ctx;
      }
      return ctx.WithBlankFlags();
    }

    private T SignalInvalid(PrecisionContext ctx) {
      if (this.GetHelper().GetArithmeticSupport() == BigNumberFlags.FiniteOnly) {
        throw new ArithmeticException("Invalid operation");
      }
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= PrecisionContext.FlagInvalid;
      }
      return this.GetHelper().CreateNewWithFlags(BigInteger.Zero, BigInteger.Zero, BigNumberFlags.FlagQuietNaN);
    }

    private T PostProcess(T thisValue, PrecisionContext ctxDest, PrecisionContext ctxSrc) {
      return this.PostProcessEx(thisValue, ctxDest, ctxSrc, false, false);
    }

    private T PostProcessAfterDivision(T thisValue, PrecisionContext ctxDest, PrecisionContext ctxSrc) {
      return this.PostProcessEx(thisValue, ctxDest, ctxSrc, true, false);
    }

    private T PostProcessAfterQuantize(T thisValue, PrecisionContext ctxDest, PrecisionContext ctxSrc) {
      return this.PostProcessEx(thisValue, ctxDest, ctxSrc, false, true);
    }

    private T PostProcessEx(T thisValue, PrecisionContext ctxDest, PrecisionContext ctxSrc, bool afterDivision, bool afterQuantize) {
      int thisFlags = this.GetHelper().GetFlags(thisValue);
      if (ctxDest != null && ctxSrc != null) {
        if (ctxDest.HasFlags) {
          if (!ctxSrc.ClampNormalExponents) {
            ctxSrc.Flags &= ~PrecisionContext.FlagClamped;
          }
          ctxDest.Flags |= ctxSrc.Flags;
          if ((ctxSrc.Flags & PrecisionContext.FlagSubnormal) != 0) {
            // Treat subnormal numbers as underflows
            int newflags = PrecisionContext.FlagUnderflow | PrecisionContext.FlagInexact |
              PrecisionContext.FlagRounded;
            ctxDest.Flags |= newflags;
          }
        }
      }
      if ((thisFlags & BigNumberFlags.FlagSpecial) != 0) {
        if (ctxDest.Flags == 0) {
          return this.SignalInvalid(ctxDest);
        }
        return thisValue;
      }
      BigInteger mant = BigInteger.Abs(this.GetHelper().GetMantissa(thisValue));
      if (mant.IsZero) {
        if (afterQuantize) {
          return this.GetHelper().CreateNewWithFlags(mant, this.GetHelper().GetExponent(thisValue), 0);
        }
        return this.wrapper.RoundToPrecision(this.GetHelper().ValueOf(0), ctxDest);
      }
      if (afterQuantize) {
        return thisValue;
      }
      BigInteger exp = this.GetHelper().GetExponent(thisValue);
      if (exp.Sign > 0) {
        FastInteger fastExp = FastInteger.FromBig(exp);
        if (ctxDest == null || !ctxDest.HasMaxPrecision) {
          mant = this.GetHelper().MultiplyByRadixPower(mant, fastExp);
          return this.GetHelper().CreateNewWithFlags(mant, BigInteger.Zero, thisFlags);
        }
        if (!ctxDest.ExponentWithinRange(exp)) {
          return thisValue;
        }
        FastInteger prec = FastInteger.FromBig(ctxDest.Precision);
        FastInteger digits = this.GetHelper().CreateShiftAccumulator(mant).GetDigitLength();
        prec.Subtract(digits);
        if (prec.Sign > 0 && prec.CompareTo(fastExp) >= 0) {
          mant = this.GetHelper().MultiplyByRadixPower(mant, fastExp);
          return this.GetHelper().CreateNewWithFlags(mant, BigInteger.Zero, thisFlags);
        }
        if (afterDivision) {
          mant = DecimalUtility.ReduceTrailingZeros(mant, fastExp, this.GetHelper().GetRadix(), null, null, null);
          thisValue = this.GetHelper().CreateNewWithFlags(mant, fastExp.AsBigInteger(), thisFlags);
        }
      } else if (afterDivision && exp.Sign < 0) {
        FastInteger fastExp = FastInteger.FromBig(exp);
        mant = DecimalUtility.ReduceTrailingZeros(mant, fastExp, this.GetHelper().GetRadix(), null, null, new FastInteger(0));
        thisValue = this.GetHelper().CreateNewWithFlags(mant, fastExp.AsBigInteger(), thisFlags);
      }
      return thisValue;
    }

    private T ReturnQuietNaN(T thisValue, PrecisionContext ctx) {
      BigInteger mant = BigInteger.Abs(this.GetHelper().GetMantissa(thisValue));
      bool mantChanged = false;
      if (!mant.IsZero && ctx != null && ctx.HasMaxPrecision) {
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

    private T CheckNotANumber3(T thisValue, T other, T other2, PrecisionContext ctx) {
      int thisFlags = this.GetHelper().GetFlags(thisValue);
      int otherFlags = this.GetHelper().GetFlags(other);
      int other2Flags = this.GetHelper().GetFlags(other2);
      // Check this value then the other value for signaling NaN
      if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(thisValue, ctx);
      }
      if ((otherFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(other, ctx);
      }
      if ((other2Flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(other, ctx);
      }
      // Check this value then the other value for quiet NaN
      if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.ReturnQuietNaN(thisValue, ctx);
      }
      if ((otherFlags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.ReturnQuietNaN(other, ctx);
      }
      if ((other2Flags & BigNumberFlags.FlagQuietNaN) != 0) {
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

    private T RoundBeforeOp(T val, PrecisionContext ctx) {
      if (ctx == null || !ctx.HasMaxPrecision) {
        return val;
      }
      int thisFlags = this.GetHelper().GetFlags(val);
      if ((thisFlags & BigNumberFlags.FlagSpecial) != 0) {
        return val;
      }
      FastInteger fastPrecision = FastInteger.FromBig(ctx.Precision);
      BigInteger mant = BigInteger.Abs(this.GetHelper().GetMantissa(val));
      FastInteger digits = this.GetHelper().CreateShiftAccumulator(mant).GetDigitLength();
      PrecisionContext ctx2 = ctx.WithBlankFlags().WithTraps(0);
      if (digits.CompareTo(fastPrecision) <= 0) {
        // Rounding is only to be done if the digit count is
        // too big (distinguishing this case is material
        // if the value also has an exponent that's out of range)
        return val;
      }
      val = this.wrapper.RoundToPrecision(val, ctx2);
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
          int newflags = PrecisionContext.FlagLostDigits | PrecisionContext.FlagInexact |
            PrecisionContext.FlagRounded;
          ctx.Flags |= newflags;
        }
      }
      if ((ctx2.Flags & PrecisionContext.FlagRounded) != 0) {
        if (ctx.HasFlags) {
          ctx.Flags |= PrecisionContext.FlagRounded;
        }
      }
      if ((ctx2.Flags & PrecisionContext.FlagSubnormal) != 0) {
        // Console.WriteLine("Subnormal input: " + val);
      }
      if ((ctx2.Flags & PrecisionContext.FlagUnderflow) != 0) {
        // Console.WriteLine("Underflow");
      }
      if ((ctx2.Flags & PrecisionContext.FlagOverflow) != 0) {
        bool neg = (thisFlags & BigNumberFlags.FlagNegative) != 0;
        ctx.Flags |= PrecisionContext.FlagLostDigits;
        return this.SignalOverflow2(ctx, neg);
      }
      return val;
    }

    public T DivideToIntegerNaturalScale(T thisValue, T divisor, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      divisor = this.RoundBeforeOp(divisor, ctx2);
      thisValue = this.wrapper.DivideToIntegerNaturalScale(thisValue, divisor, ctx2);
      return this.PostProcessAfterDivision(thisValue, ctx, ctx2);
    }

    public T DivideToIntegerZeroScale(T thisValue, T divisor, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      divisor = this.RoundBeforeOp(divisor, ctx2);
      thisValue = this.wrapper.DivideToIntegerZeroScale(thisValue, divisor, ctx2);
      return this.PostProcessAfterDivision(thisValue, ctx, ctx2);
    }

    public T Abs(T value, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(value, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      value = this.RoundBeforeOp(value, ctx2);
      value = this.wrapper.Abs(value, ctx2);
      return this.PostProcess(value, ctx, ctx2);
    }

    public T Negate(T value, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(value, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      value = this.RoundBeforeOp(value, ctx2);
      value = this.wrapper.Negate(value, ctx2);
      return this.PostProcess(value, ctx, ctx2);
    }

    /// <summary>Finds the remainder that results when dividing two T objects.</summary>
    /// <returns>The remainder of the two objects.</returns>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public T Remainder(T thisValue, T divisor, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      divisor = this.RoundBeforeOp(divisor, ctx2);
      thisValue = this.wrapper.Remainder(thisValue, divisor, ctx2);
      return this.PostProcess(thisValue, ctx, ctx2);
    }

    public T RemainderNear(T thisValue, T divisor, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      divisor = this.RoundBeforeOp(divisor, ctx2);
      thisValue = this.wrapper.RemainderNear(thisValue, divisor, ctx2);
      return this.PostProcess(thisValue, ctx, ctx2);
    }

    public T Pi(PrecisionContext ctx) {
      return this.wrapper.Pi(ctx);
    }

    private T SignalOverflow2(PrecisionContext pc, bool neg) {
      if (pc != null) {
        Rounding roundingOnOverflow = pc.Rounding;
        if (pc.HasFlags) {
          pc.Flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
        }
        if (pc.HasMaxPrecision && pc.HasExponentRange &&
            (roundingOnOverflow == Rounding.Down || roundingOnOverflow == Rounding.ZeroFiveUp ||
             (roundingOnOverflow == Rounding.Ceiling && neg) || (roundingOnOverflow == Rounding.Floor && !neg))) {
          // Set to the highest possible value for
          // the given precision
          BigInteger overflowMant = BigInteger.Zero;
          FastInteger fastPrecision = FastInteger.FromBig(pc.Precision);
          overflowMant = this.GetHelper().MultiplyByRadixPower(BigInteger.One, fastPrecision);
          overflowMant -= BigInteger.One;
          FastInteger clamp = FastInteger.FromBig(pc.EMax).Increment().Subtract(fastPrecision);
          return this.GetHelper().CreateNewWithFlags(overflowMant, clamp.AsBigInteger(), neg ? BigNumberFlags.FlagNegative : 0);
        }
      }
      return this.GetHelper().GetArithmeticSupport() == BigNumberFlags.FiniteOnly ?
        default(T) : this.GetHelper().CreateNewWithFlags(BigInteger.Zero, BigInteger.Zero, (neg ? BigNumberFlags.FlagNegative : 0) | BigNumberFlags.FlagInfinity);
    }

    public T Power(T thisValue, T pow, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, pow, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      // Console.WriteLine("op was " + thisValue + ", "+pow);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      pow = this.RoundBeforeOp(pow, ctx2);
      // Console.WriteLine("op now " + thisValue + ", "+pow);
      int powSign = this.GetHelper().GetSign(pow);
      if (powSign == 0 && this.GetHelper().GetSign(thisValue) == 0) {
        thisValue = this.wrapper.RoundToPrecision(this.GetHelper().ValueOf(1), ctx2);
      } else {
        // Console.WriteLine("was " + thisValue);
        // BigInteger powExponent = this.GetHelper().GetExponent(pow);
        // BigInteger powInteger = BigInteger.Abs(this.GetHelper().GetMantissa(pow));
        {
          thisValue = this.wrapper.Power(thisValue, pow, ctx2);
        }
      }
      // Console.WriteLine("was " + thisValue);
      thisValue = this.PostProcessAfterDivision(thisValue, ctx, ctx2);
      // Console.WriteLine("result was " + thisValue);
      // Console.WriteLine("now " + thisValue);
      return thisValue;
    }

    public T Log10(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      thisValue = this.wrapper.Log10(thisValue, ctx2);
      return this.PostProcess(thisValue, ctx, ctx2);
    }

    public T Ln(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      // Console.WriteLine("was: " + thisValue);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      // Console.WriteLine("now: " + thisValue);
      thisValue = this.wrapper.Ln(thisValue, ctx2);
      // Console.WriteLine("result: " + thisValue);
      return this.PostProcess(thisValue, ctx, ctx2);
    }

    public IRadixMathHelper<T> GetHelper() {
      return this.wrapper.GetHelper();
    }

    public T Exp(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      thisValue = this.wrapper.Exp(thisValue, ctx2);
      return this.PostProcess(thisValue, ctx, ctx2);
    }

    public T SquareRoot(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      // Console.WriteLine("op was " + thisValue);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      // Console.WriteLine("op now " + thisValue);
      thisValue = this.wrapper.SquareRoot(thisValue, ctx2);
      // Console.WriteLine("result was " + thisValue);
      return this.PostProcess(thisValue, ctx, ctx2);
    }

    public T NextMinus(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      thisValue = this.wrapper.NextMinus(thisValue, ctx2);
      return this.PostProcess(thisValue, ctx, ctx2);
    }

    public T NextToward(T thisValue, T otherValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, otherValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      otherValue = this.RoundBeforeOp(otherValue, ctx2);
      thisValue = this.wrapper.NextToward(thisValue, otherValue, ctx2);
      return this.PostProcess(thisValue, ctx, ctx2);
    }

    public T NextPlus(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      thisValue = this.wrapper.NextPlus(thisValue, ctx2);
      return this.PostProcess(thisValue, ctx, ctx2);
    }

    public T DivideToExponent(T thisValue, T divisor, BigInteger desiredExponent, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      divisor = this.RoundBeforeOp(divisor, ctx2);
      thisValue = this.wrapper.DivideToExponent(thisValue, divisor, desiredExponent, ctx2);
      return this.PostProcessAfterDivision(thisValue, ctx, ctx2);
    }

    /// <summary>Divides two T objects.</summary>
    /// <returns>The quotient of the two objects.</returns>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public T Divide(T thisValue, T divisor, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, divisor, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      divisor = this.RoundBeforeOp(divisor, ctx2);
      thisValue = this.wrapper.Divide(thisValue, divisor, ctx2);
      return this.PostProcessAfterDivision(thisValue, ctx, ctx2);
    }

    public T MinMagnitude(T a, T b, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(a, b, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      a = this.RoundBeforeOp(a, ctx2);
      b = this.RoundBeforeOp(b, ctx2);
      a = this.wrapper.MinMagnitude(a, b, ctx2);
      return this.PostProcess(a, ctx, ctx2);
    }

    public T MaxMagnitude(T a, T b, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(a, b, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      a = this.RoundBeforeOp(a, ctx2);
      b = this.RoundBeforeOp(b, ctx2);
      a = this.wrapper.MaxMagnitude(a, b, ctx2);
      return this.PostProcess(a, ctx, ctx2);
    }

    public T Max(T a, T b, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(a, b, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      a = this.RoundBeforeOp(a, ctx2);
      b = this.RoundBeforeOp(b, ctx2);
      // choose the left operand if both are equal
      a = (this.CompareTo(a, b) >= 0) ? a : b;
      return this.PostProcess(a, ctx, ctx2);
    }

    public T Min(T a, T b, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(a, b, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      a = this.RoundBeforeOp(a, ctx2);
      b = this.RoundBeforeOp(b, ctx2);
      // choose the left operand if both are equal
      a = (this.CompareTo(a, b) <= 0) ? a : b;
      return this.PostProcess(a, ctx, ctx2);
    }

    /// <summary>Multiplies two T objects.</summary>
    /// <returns>The product of the two objects.</returns>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='other'>A T object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public T Multiply(T thisValue, T other, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, other, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      other = this.RoundBeforeOp(other, ctx2);
      thisValue = this.wrapper.Multiply(thisValue, other, ctx2);
      return this.PostProcess(thisValue, ctx, ctx2);
    }

    public T MultiplyAndAdd(T thisValue, T multiplicand, T augend, PrecisionContext ctx) {
      T ret = this.CheckNotANumber3(thisValue, multiplicand, augend, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      multiplicand = this.RoundBeforeOp(multiplicand, ctx2);
      augend = this.RoundBeforeOp(augend, ctx2);
      // the only time the first operand to the addition can be
      // 0 is if either thisValue rounded or multiplicand
      // rounded is 0
      bool zeroA = this.GetHelper().GetSign(thisValue) == 0 ||
        this.GetHelper().GetSign(multiplicand) == 0;
      bool zeroB = this.GetHelper().GetSign(augend) == 0;
      if (zeroA) {
        thisValue = zeroB ? this.wrapper.RoundToPrecision(this.GetHelper().ValueOf(0), ctx2) : augend;
        thisValue = this.RoundToPrecision(thisValue, ctx2);
      } else if (!zeroB) {
        thisValue = this.wrapper.MultiplyAndAdd(thisValue, multiplicand, augend, ctx2);
      } else {
        // Augend is 0, so it's the same as calling Multiply
        thisValue = this.wrapper.Multiply(thisValue, multiplicand, ctx2);
      }
      return this.PostProcess(thisValue, ctx, ctx2);
    }

    public T RoundToBinaryPrecision(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      thisValue = this.wrapper.RoundToBinaryPrecision(thisValue, ctx2);
      return this.PostProcess(thisValue, ctx, ctx2);
    }

    public T Plus(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      thisValue = this.wrapper.Plus(thisValue, ctx2);
      return this.PostProcess(thisValue, ctx, ctx2);
    }

    public T RoundToPrecision(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      thisValue = this.wrapper.RoundToPrecision(thisValue, ctx2);
      return this.PostProcess(thisValue, ctx, ctx2);
    }

    public T Quantize(T thisValue, T otherValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      // Console.WriteLine("was: "+thisValue+", "+otherValue);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      // Console.WriteLine("now: "+thisValue+", "+otherValue);
      otherValue = this.RoundBeforeOp(otherValue, ctx2);
      // Apparently, subnormal values of "otherValue" raise
      // an invalid operation flag, according to the test cases
      PrecisionContext ctx3 = ctx2 == null ? null : ctx2.WithBlankFlags();
      this.wrapper.RoundToPrecision(otherValue, ctx3);
      if (ctx3 != null && (ctx3.Flags & PrecisionContext.FlagSubnormal) != 0) {
        return this.SignalInvalid(ctx);
      }
      thisValue = this.wrapper.Quantize(thisValue, otherValue, ctx2);
      // Console.WriteLine("result: "+thisValue);
      return this.PostProcessAfterQuantize(thisValue, ctx, ctx2);
    }

    public T RoundToExponentExact(T thisValue, BigInteger expOther, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      thisValue = this.wrapper.RoundToExponentExact(thisValue, expOther, ctx);
      return this.PostProcessAfterQuantize(thisValue, ctx, ctx2);
    }

    public T RoundToExponentSimple(T thisValue, BigInteger expOther, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      thisValue = this.wrapper.RoundToExponentSimple(thisValue, expOther, ctx2);
      return this.PostProcessAfterQuantize(thisValue, ctx, ctx2);
    }

    public T RoundToExponentNoRoundedFlag(T thisValue, BigInteger exponent, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      thisValue = this.wrapper.RoundToExponentNoRoundedFlag(thisValue, exponent, ctx);
      return this.PostProcessAfterQuantize(thisValue, ctx, ctx2);
    }

    public T Reduce(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      thisValue = this.wrapper.Reduce(thisValue, ctx);
      return this.PostProcessAfterQuantize(thisValue, ctx, ctx2);
    }

    public T Add(T thisValue, T other, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, other, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.RoundBeforeOp(thisValue, ctx2);
      other = this.RoundBeforeOp(other, ctx2);
      bool zeroA = this.GetHelper().GetSign(thisValue) == 0;
      bool zeroB = this.GetHelper().GetSign(other) == 0;
      if (zeroA) {
        thisValue = zeroB ? this.wrapper.RoundToPrecision(this.GetHelper().ValueOf(0), ctx2) : other;
        thisValue = this.RoundToPrecision(thisValue, ctx2);
      } else if (!zeroB) {
        thisValue = this.wrapper.AddEx(thisValue, other, ctx2, true);
      } else {
        thisValue = this.RoundToPrecision(thisValue, ctx2);
      }
      return this.PostProcess(thisValue, ctx, ctx2);
    }

    public T AddEx(T thisValue, T other, PrecisionContext ctx, bool roundToOperandPrecision) {
      // NOTE: Ignores roundToOperandPrecision
      return this.Add(thisValue, other, ctx);
    }

    /// <summary>Compares a T object with this instance.</summary>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='otherValue'>A T object. (2).</param>
    /// <param name='treatQuietNansAsSignaling'>A Boolean object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public T CompareToWithContext(T thisValue, T otherValue, bool treatQuietNansAsSignaling, PrecisionContext ctx) {
      T ret = this.CheckNotANumber2(thisValue, otherValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      thisValue = this.RoundBeforeOp(thisValue, ctx);
      otherValue = this.RoundBeforeOp(otherValue, ctx);
      return this.wrapper.CompareToWithContext(
        thisValue,
        otherValue,
        treatQuietNansAsSignaling,
        ctx);
    }

    /// <summary>Compares a T object with this instance.</summary>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='otherValue'>A T object. (2).</param>
    public int CompareTo(T thisValue, T otherValue) {
      return this.wrapper.CompareTo(thisValue, otherValue);
    }

    public T RoundAfterConversion(T thisValue, PrecisionContext ctx) {
      T ret = this.CheckNotANumber1(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      if (this.GetHelper().GetSign(thisValue) == 0) {
        return this.wrapper.RoundToPrecision(this.GetHelper().ValueOf(0), ctx);
      }
      PrecisionContext ctx2 = GetContextWithFlags(ctx);
      thisValue = this.wrapper.RoundToPrecision(thisValue, ctx2);
      return this.PostProcessAfterQuantize(thisValue, ctx, ctx2);
    }
  }
}
