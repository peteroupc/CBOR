/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
using System.Text;
//using System.Numerics;

namespace PeterO {
    /// <summary> Encapsulates radix-independent arithmetic. </summary>
    /// <typeparam name='T'>Data type for a numeric value in a particular
    /// radix.</typeparam>
  class RadixMath<T> {

    IRadixMathHelper<T> helper;
    int thisRadix;
    int support;

    public RadixMath(IRadixMathHelper<T> helper) {
      this.helper = helper;
      this.support = helper.GetArithmeticSupport();
      this.thisRadix = helper.GetRadix();
    }

    private T ReturnQuietNaN(T thisValue, PrecisionContext ctx) {
      BigInteger mant = BigInteger.Abs(helper.GetMantissa(thisValue));
      bool mantChanged = false;
      if (!(mant.IsZero) && ctx != null && !((ctx.Precision).IsZero)) {
        BigInteger limit = helper.MultiplyByRadixPower(
          BigInteger.One, FastInteger.FromBig(ctx.Precision));
        if (mant.CompareTo(limit) >= 0) {
          mant = mant % (BigInteger)limit;
          mantChanged = true;
        }
      }
      int flags = helper.GetFlags(thisValue);
      if (!mantChanged && (flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return thisValue;
      }
      flags &= BigNumberFlags.FlagNegative;
      flags |= BigNumberFlags.FlagQuietNaN;
      return helper.CreateNewWithFlags(mant, BigInteger.Zero, flags);
    }

    private T SquareRootHandleSpecial(T thisValue, PrecisionContext ctx) {
      int thisFlags = helper.GetFlags(thisValue);
      if (((thisFlags) & BigNumberFlags.FlagSpecial) != 0) {
        if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
          return SignalingNaNInvalid(thisValue, ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
          return ReturnQuietNaN(thisValue, ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          // Square root of infinity
          if ((thisFlags & BigNumberFlags.FlagNegative) != 0) {
            return SignalInvalid(ctx);
          }
          return thisValue;
        }
      }
      int sign=helper.GetSign(thisValue);
      if(sign<0){
        return SignalInvalid(ctx);
      }
      return default(T);
    }

    private T DivisionHandleSpecial(T thisValue, T other, PrecisionContext ctx) {
      int thisFlags = helper.GetFlags(thisValue);
      int otherFlags = helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = HandleNotANumber(thisValue, other, ctx);
        if ((Object)result != (Object)default(T)) return result;
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0 && (otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          // Attempt to divide infinity by infinity
          return SignalInvalid(ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          return EnsureSign(thisValue, ((thisFlags ^ otherFlags) & BigNumberFlags.FlagNegative) != 0);
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          // Divisor is infinity, so result will be epsilon
          if (ctx != null && ctx.HasExponentRange && (ctx.Precision).Sign > 0) {
            if (ctx.HasFlags) {
              ctx.Flags |= PrecisionContext.FlagClamped;
            }
            BigInteger bigexp = ctx.EMin;
            BigInteger bigprec = ctx.Precision;
            bigexp -= (BigInteger)bigprec;
            bigexp += BigInteger.One;
            thisFlags = ((thisFlags ^ otherFlags) & BigNumberFlags.FlagNegative);
            return helper.CreateNewWithFlags(
              BigInteger.Zero, bigexp, thisFlags);
          }
          thisFlags = ((thisFlags ^ otherFlags) & BigNumberFlags.FlagNegative);
          return RoundToPrecision(helper.CreateNewWithFlags(
            BigInteger.Zero, BigInteger.Zero,
            thisFlags), ctx);
        }
      }
      return default(T);
    }

    private T RemainderHandleSpecial(T thisValue, T other, PrecisionContext ctx) {
      int thisFlags = helper.GetFlags(thisValue);
      int otherFlags = helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = HandleNotANumber(thisValue, other, ctx);
        if ((Object)result != (Object)default(T)) return result;
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          return SignalInvalid(ctx);
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          return RoundToPrecision(thisValue, ctx);
        }
      }
      if (helper.GetMantissa(other).IsZero) {
        return SignalInvalid(ctx);
      }
      return default(T);
    }

    private T MinMaxHandleSpecial(T thisValue, T otherValue, PrecisionContext ctx,
                                  bool isMinOp, bool compareAbs) {
      int thisFlags = helper.GetFlags(thisValue);
      int otherFlags = helper.GetFlags(otherValue);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        // Check this value then the other value for signaling NaN
        if ((helper.GetFlags(thisValue) & BigNumberFlags.FlagSignalingNaN) != 0) {
          return SignalingNaNInvalid(thisValue, ctx);
        }
        if ((helper.GetFlags(otherValue) & BigNumberFlags.FlagSignalingNaN) != 0) {
          return SignalingNaNInvalid(otherValue, ctx);
        }
        // Check this value then the other value for quiet NaN
        if ((helper.GetFlags(thisValue) & BigNumberFlags.FlagQuietNaN) != 0) {
          if ((helper.GetFlags(otherValue) & BigNumberFlags.FlagQuietNaN) != 0) {
            // both values are quiet NaN
            return ReturnQuietNaN(thisValue, ctx);
          }
          // return "other" for being numeric
          return RoundToPrecision(otherValue, ctx);
        }
        if ((helper.GetFlags(otherValue) & BigNumberFlags.FlagQuietNaN) != 0) {
          // At this point, "thisValue" can't be NaN,
          // return "thisValue" for being numeric
          return RoundToPrecision(thisValue, ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          if (compareAbs && (otherFlags & BigNumberFlags.FlagInfinity) == 0) {
            // treat this as larger
            return (isMinOp) ? RoundToPrecision(otherValue, ctx) : thisValue;
          }
          // This value is infinity
          if (isMinOp) {
            return ((thisFlags & BigNumberFlags.FlagNegative) != 0) ?
              thisValue : // if negative, will be less than every other number
              RoundToPrecision(otherValue, ctx); // if positive, will be greater
          } else {
            return ((thisFlags & BigNumberFlags.FlagNegative) == 0) ?
              thisValue : // if positive, will be greater than every other number
              RoundToPrecision(otherValue, ctx);
          }
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          if (compareAbs) {
            // treat this as larger (the first value
            // won't be infinity at this point
            return (isMinOp) ? RoundToPrecision(thisValue, ctx) : otherValue;
          }
          if (isMinOp) {
            return ((otherFlags & BigNumberFlags.FlagNegative) == 0) ?
              RoundToPrecision(thisValue, ctx) :
              otherValue;
          } else {
            return ((otherFlags & BigNumberFlags.FlagNegative) != 0) ?
              RoundToPrecision(thisValue, ctx) :
              otherValue;
          }
        }
      }
      return default(T);
    }

    private T HandleNotANumber(T thisValue, T other, PrecisionContext ctx) {
      int thisFlags = helper.GetFlags(thisValue);
      int otherFlags = helper.GetFlags(other);
      // Check this value then the other value for signaling NaN
      if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return SignalingNaNInvalid(thisValue, ctx);
      }
      if ((otherFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return SignalingNaNInvalid(other, ctx);
      }
      // Check this value then the other value for quiet NaN
      if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
        return ReturnQuietNaN(thisValue, ctx);
      }
      if ((otherFlags & BigNumberFlags.FlagQuietNaN) != 0) {
        return ReturnQuietNaN(other, ctx);
      }
      return default(T);
    }

    private T MultiplyAddHandleSpecial(T op1, T op2, T op3, PrecisionContext ctx) {
      int op1Flags = helper.GetFlags(op1);
      // Check operands in order for signaling NaN
      if ((op1Flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return SignalingNaNInvalid(op1, ctx);
      }
      int op2Flags = helper.GetFlags(op2);
      if ((op2Flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return SignalingNaNInvalid(op2, ctx);
      }
      int op3Flags = helper.GetFlags(op3);
      if ((op3Flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return SignalingNaNInvalid(op3, ctx);
      }
      // Check operands in order for quiet NaN
      if ((op1Flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return ReturnQuietNaN(op1, ctx);
      }
      if ((op2Flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return ReturnQuietNaN(op2, ctx);
      }
      // Check multiplying infinity by 0 (important to check
      // now before checking third operand for quiet NaN because
      // this signals invalid operation and the operation starts
      // with multiplying only the first two operands)
      if ((op1Flags & BigNumberFlags.FlagInfinity) != 0) {
        // Attempt to multiply infinity by 0
        if ((op2Flags & BigNumberFlags.FlagSpecial) == 0 && helper.GetMantissa(op2).IsZero)
          return SignalInvalid(ctx);
      }
      if ((op2Flags & BigNumberFlags.FlagInfinity) != 0) {
        // Attempt to multiply infinity by 0
        if ((op1Flags & BigNumberFlags.FlagSpecial) == 0 && helper.GetMantissa(op1).IsZero)
          return SignalInvalid(ctx);
      }
      // Now check third operand for quiet NaN
      if ((op3Flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return ReturnQuietNaN(op3, ctx);
      }
      return default(T);
    }

    private T ValueOf(int value, PrecisionContext ctx) {
      if (ctx == null || !ctx.HasExponentRange || ctx.ExponentWithinRange(BigInteger.Zero))
        return helper.ValueOf(value);
      return RoundToPrecision(helper.ValueOf(value), ctx);
    }
    private int CompareToHandleSpecialReturnInt(
      T thisValue, T other) {
      int thisFlags = helper.GetFlags(thisValue);
      int otherFlags = helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        if (((thisFlags | otherFlags) & BigNumberFlags.FlagNaN) != 0) {
          throw new ArithmeticException("Either operand is NaN");
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          // thisValue is infinity
          if ((thisFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) ==
              (otherFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)))
            return 0;
          return ((thisFlags & BigNumberFlags.FlagNegative) == 0) ? 1 : -1;
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          // the other value is infinity
          if ((thisFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) ==
              (otherFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)))
            return 0;
          return ((otherFlags & BigNumberFlags.FlagNegative) == 0) ? -1 : 1;
        }
      }
      return 2;
    }

    private T CompareToHandleSpecial(T thisValue, T other, bool treatQuietNansAsSignaling, PrecisionContext ctx) {
      int thisFlags = helper.GetFlags(thisValue);
      int otherFlags = helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        // Check this value then the other value for signaling NaN
        if ((helper.GetFlags(thisValue) & BigNumberFlags.FlagSignalingNaN) != 0) {
          return SignalingNaNInvalid(thisValue, ctx);
        }
        if ((helper.GetFlags(other) & BigNumberFlags.FlagSignalingNaN) != 0) {
          return SignalingNaNInvalid(other, ctx);
        }
        if (treatQuietNansAsSignaling) {
          if ((helper.GetFlags(thisValue) & BigNumberFlags.FlagQuietNaN) != 0) {
            return SignalingNaNInvalid(thisValue, ctx);
          }
          if ((helper.GetFlags(other) & BigNumberFlags.FlagQuietNaN) != 0) {
            return SignalingNaNInvalid(other, ctx);
          }
        } else {
          // Check this value then the other value for quiet NaN
          if ((helper.GetFlags(thisValue) & BigNumberFlags.FlagQuietNaN) != 0) {
            return ReturnQuietNaN(thisValue, ctx);
          }
          if ((helper.GetFlags(other) & BigNumberFlags.FlagQuietNaN) != 0) {
            return ReturnQuietNaN(other, ctx);
          }
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          // thisValue is infinity
          if ((thisFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) ==
              (otherFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)))
            return ValueOf(0, null);
          return ((thisFlags & BigNumberFlags.FlagNegative) == 0) ?
            ValueOf(1, null) : ValueOf(-1, null);
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          // the other value is infinity
          if ((thisFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) ==
              (otherFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)))
            return ValueOf(0, null);
          return ((otherFlags & BigNumberFlags.FlagNegative) == 0) ?
            ValueOf(-1, null) : ValueOf(1, null);
        }
      }
      return default(T);
    }
    private T SignalingNaNInvalid(T value, PrecisionContext ctx) {
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= PrecisionContext.FlagInvalid;
      }
      return ReturnQuietNaN(value, ctx);
    }
    private T SignalInvalid(PrecisionContext ctx) {
      if (support == BigNumberFlags.FiniteOnly)
        throw new ArithmeticException("Invalid operation");
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= PrecisionContext.FlagInvalid;
      }
      return helper.CreateNewWithFlags(BigInteger.Zero, BigInteger.Zero, BigNumberFlags.FlagQuietNaN);
    }
    private T SignalInvalidWithMessage(PrecisionContext ctx, String str) {
      if (support == BigNumberFlags.FiniteOnly)
        throw new ArithmeticException("Invalid operation");
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= PrecisionContext.FlagInvalid;
      }
      return helper.CreateNewWithFlags(BigInteger.Zero, BigInteger.Zero, BigNumberFlags.FlagQuietNaN);
    }

    private T SignalOverflow(bool neg){
      return support == BigNumberFlags.FiniteOnly ? default(T) :
        helper.CreateNewWithFlags(
          BigInteger.Zero, BigInteger.Zero,
          (neg ? BigNumberFlags.FlagNegative : 0) | BigNumberFlags.FlagInfinity);
    }

    private T SignalOverflow2(PrecisionContext pc, bool neg){
      if(pc!=null && pc.HasFlags){
        pc.Flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
      }
      if (pc!=null && !((pc.Precision).IsZero) &&
          pc.HasExponentRange &&
          (pc.Rounding == Rounding.Down ||
           pc.Rounding == Rounding.ZeroFiveUp ||
           (pc.Rounding == Rounding.Ceiling && neg) ||
           (pc.Rounding == Rounding.Floor && !neg))) {
        // Set to the highest possible value for
        // the given precision
        BigInteger overflowMant = BigInteger.Zero;
        FastInteger fastPrecision=FastInteger.FromBig(pc.Precision);
        overflowMant = helper.MultiplyByRadixPower(BigInteger.One, fastPrecision);
        overflowMant -= BigInteger.One;
        FastInteger clamp = FastInteger.FromBig(pc.EMax).Increment()
          .Subtract(fastPrecision);
        return helper.CreateNewWithFlags(overflowMant,
                                         clamp.AsBigInteger(),
                                         neg ? BigNumberFlags.FlagNegative : 0);
      }
      return SignalOverflow(neg);
    }

    private T SignalDivideByZero(PrecisionContext ctx, bool neg) {
      if (support == BigNumberFlags.FiniteOnly)
        throw new DivideByZeroException("Division by zero");
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= PrecisionContext.FlagDivideByZero;
      }
      return helper.CreateNewWithFlags(
        BigInteger.Zero, BigInteger.Zero,
        BigNumberFlags.FlagInfinity | (neg ? BigNumberFlags.FlagNegative : 0));
    }

    private bool Round(IShiftAccumulator accum, Rounding rounding,
                       bool neg, FastInteger fastint) {
      bool incremented = false;
      int radix = thisRadix;
      if (rounding == Rounding.HalfUp) {
        if (accum.LastDiscardedDigit >= (radix / 2)) {
          incremented = true;
        }
      } else if (rounding == Rounding.HalfEven) {
        if (accum.LastDiscardedDigit >= (radix / 2)) {
          if ((accum.LastDiscardedDigit > (radix / 2) || accum.OlderDiscardedDigits != 0)) {
            incremented = true;
          } else if (!fastint.IsEvenNumber) {
            incremented = true;
          }
        }
      } else if (rounding == Rounding.Ceiling) {
        if (!neg && (accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.Floor) {
        if (neg && (accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.HalfDown) {
        if (accum.LastDiscardedDigit > (radix / 2) ||
            (accum.LastDiscardedDigit == (radix / 2) && accum.OlderDiscardedDigits != 0)) {
          incremented = true;
        }
      } else if (rounding == Rounding.Up) {
        if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.ZeroFiveUp) {
        if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
          if (radix == 2) {
            incremented = true;
          } else {
            int lastDigit = FastInteger.Copy(fastint).Mod(radix).AsInt32();
            if (lastDigit == 0 || lastDigit == (radix / 2)) {
              incremented = true;
            }
          }
        }
      }
      return incremented;
    }

    private bool RoundGivenDigits(int lastDiscarded, int olderDiscarded, Rounding rounding,
                                  bool neg, BigInteger bigval) {
      bool incremented = false;
      int radix = thisRadix;
      if (rounding == Rounding.HalfUp) {
        if (lastDiscarded >= (radix / 2)) {
          incremented = true;
        }
      } else if (rounding == Rounding.HalfEven) {
        if (lastDiscarded >= (radix / 2)) {
          if ((lastDiscarded > (radix / 2) || olderDiscarded != 0)) {
            incremented = true;
          } else if (!bigval.IsEven) {
            incremented = true;
          }
        }
      } else if (rounding == Rounding.Ceiling) {
        if (!neg && (lastDiscarded | olderDiscarded) != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.Floor) {
        if (neg && (lastDiscarded | olderDiscarded) != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.HalfDown) {
        if (lastDiscarded > (radix / 2) ||
            (lastDiscarded == (radix / 2) && olderDiscarded != 0)) {
          incremented = true;
        }
      } else if (rounding == Rounding.Up) {
        if ((lastDiscarded | olderDiscarded) != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.ZeroFiveUp) {
        if ((lastDiscarded | olderDiscarded) != 0) {
          if (radix == 2) {
            incremented = true;
          } else {
            BigInteger bigdigit = bigval % (BigInteger)radix;
            int lastDigit = (int)bigdigit;
            if (lastDigit == 0 || lastDigit == (radix / 2)) {
              incremented = true;
            }
          }
        }
      }
      return incremented;
    }

    private bool RoundGivenBigInt(IShiftAccumulator accum, Rounding rounding,
                                  bool neg, BigInteger bigval) {
      return RoundGivenDigits(accum.LastDiscardedDigit, accum.OlderDiscardedDigits, rounding,
                              neg, bigval);
    }

    private T EnsureSign(T val, bool negative) {
      if (val == null) return val;
      int flags = helper.GetFlags(val);
      if ((negative && (flags & BigNumberFlags.FlagNegative) == 0) ||
          (!negative && (flags & BigNumberFlags.FlagNegative) != 0)) {
        flags &= ~BigNumberFlags.FlagNegative;
        flags |= (negative ? BigNumberFlags.FlagNegative : 0);
        return helper.CreateNewWithFlags(
          helper.GetMantissa(val), helper.GetExponent(val), flags);
      }
      return val;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T DivideToIntegerNaturalScale(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      FastInteger desiredScale = FastInteger.FromBig(
        helper.GetExponent(thisValue)).SubtractBig(
        helper.GetExponent(divisor));
      PrecisionContext ctx2 = PrecisionContext.ForRounding(Rounding.Down).WithBigPrecision(
        ctx == null ? BigInteger.Zero : ctx.Precision).WithBlankFlags();
      T ret = DivideInternal(thisValue, divisor,
                             ctx2,
                             IntegerModeFixedScale, BigInteger.Zero);
      if ((ctx2.Flags & (PrecisionContext.FlagInvalid | PrecisionContext.FlagDivideByZero)) != 0) {
        if (ctx.HasFlags) {
          ctx.Flags |= (PrecisionContext.FlagInvalid | PrecisionContext.FlagDivideByZero);
        }
        return ret;
      }
      bool neg = (helper.GetSign(thisValue) < 0) ^ (helper.GetSign(divisor) < 0);
      // Now the exponent's sign can only be 0 or positive
      if (helper.GetMantissa(ret).IsZero) {
        // Value is 0, so just change the exponent
        // to the preferred one
        BigInteger dividendExp = helper.GetExponent(thisValue);
        BigInteger divisorExp = helper.GetExponent(divisor);
        ret = helper.CreateNewWithFlags(BigInteger.Zero,
                                        (dividendExp - (BigInteger)divisorExp), helper.GetFlags(ret));
      } else {
        if (desiredScale.Sign < 0) {
          // Desired scale is negative, shift left
          desiredScale.Negate();
          BigInteger bigmantissa = BigInteger.Abs(helper.GetMantissa(ret));
          bigmantissa = helper.MultiplyByRadixPower(bigmantissa, desiredScale);
          ret = helper.CreateNewWithFlags(
            bigmantissa,
            helper.GetExponent(thisValue) - (BigInteger)(helper.GetExponent(divisor)),
            helper.GetFlags(ret));
        } else if (desiredScale.Sign > 0) {
          // Desired scale is positive, shift away zeros
          // but not after scale is reached
          BigInteger bigmantissa = BigInteger.Abs(helper.GetMantissa(ret));
          FastInteger fastexponent = FastInteger.FromBig(helper.GetExponent(ret));
          BigInteger bigradix = (BigInteger)(thisRadix);
          while (true) {
            if (desiredScale.CompareTo(fastexponent) == 0)
              break;
            BigInteger bigrem;
            BigInteger bigquo = BigInteger.DivRem(bigmantissa, bigradix, out bigrem);
            if (!bigrem.IsZero)
              break;
            bigmantissa = bigquo;
            fastexponent.Increment();
          }
          ret = helper.CreateNewWithFlags(bigmantissa, fastexponent.AsBigInteger(),
                                          helper.GetFlags(ret));
        }
      }
      if (ctx != null) {
        ret = RoundToPrecision(ret, ctx);
      }
      ret = EnsureSign(ret, neg);
      return ret;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T DivideToIntegerZeroScale(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      PrecisionContext ctx2 = PrecisionContext.ForRounding(Rounding.Down).WithBigPrecision(
        ctx == null ? BigInteger.Zero : ctx.Precision).WithBlankFlags();
      T ret = DivideInternal(thisValue, divisor,
                             ctx2,
                             IntegerModeFixedScale, BigInteger.Zero);
      if ((ctx2.Flags & (PrecisionContext.FlagInvalid | PrecisionContext.FlagDivideByZero)) != 0) {
        if (ctx.HasFlags) {
          ctx.Flags |= (ctx2.Flags & (PrecisionContext.FlagInvalid | PrecisionContext.FlagDivideByZero));
        }
        return ret;
      }
      if (ctx != null) {
        ctx2 = ctx.WithBlankFlags().WithUnlimitedExponents();
        ret = RoundToPrecision(ret, ctx2);
        if ((ctx2.Flags & PrecisionContext.FlagRounded) != 0) {
          return SignalInvalid(ctx);
        }
      }
      return ret;
    }

    /// <summary> </summary>
    /// <param name='value'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Abs(T value, PrecisionContext ctx) {
      int flags = helper.GetFlags(value);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return SignalingNaNInvalid(value, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return ReturnQuietNaN(value, ctx);
      }
      if ((flags & BigNumberFlags.FlagNegative) != 0) {
        return RoundToPrecision(
          helper.CreateNewWithFlags(
            helper.GetMantissa(value), helper.GetExponent(value),
            flags & ~BigNumberFlags.FlagNegative),
          ctx);
      }
      return RoundToPrecision(value, ctx);
    }

    /// <summary> </summary>
    /// <param name='value'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Negate(T value, PrecisionContext ctx) {
      int flags = helper.GetFlags(value);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return SignalingNaNInvalid(value, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return ReturnQuietNaN(value, ctx);
      }
      BigInteger mant = helper.GetMantissa(value);
      if ((flags & BigNumberFlags.FlagInfinity) == 0 && mant.IsZero) {
        if ((flags & BigNumberFlags.FlagNegative) == 0) {
          // positive 0 minus positive 0 is always positive 0
          return RoundToPrecision(helper.CreateNewWithFlags(
            mant, helper.GetExponent(value),
            flags & ~BigNumberFlags.FlagNegative), ctx);
        } else if ((ctx != null && ctx.Rounding == Rounding.Floor)) {
          // positive 0 minus negative 0 is negative 0 only if
          // the rounding is Floor
          return RoundToPrecision(helper.CreateNewWithFlags(
            mant, helper.GetExponent(value),
            flags | BigNumberFlags.FlagNegative), ctx);
        } else {
          return RoundToPrecision(helper.CreateNewWithFlags(
            mant, helper.GetExponent(value),
            flags & ~BigNumberFlags.FlagNegative), ctx);
        }
      }
      flags = flags ^ BigNumberFlags.FlagNegative;
      return RoundToPrecision(
        helper.CreateNewWithFlags(mant, helper.GetExponent(value),
                                  flags),
        ctx);
    }

    private T AbsRaw(T value) {
      return EnsureSign(value, false);
    }

    private bool IsFinite(T val) {
      return (helper.GetFlags(val) & BigNumberFlags.FlagSpecial)==0;
    }

    private bool IsNegative(T val) {
      return (helper.GetFlags(val) & BigNumberFlags.FlagNegative)!=0;
    }
    private T NegateRaw(T val) {
      if (val == null) return val;
      int sign = helper.GetFlags(val) & BigNumberFlags.FlagNegative;
      return helper.CreateNewWithFlags(helper.GetMantissa(val), helper.GetExponent(val),
                                       sign == 0 ? BigNumberFlags.FlagNegative : 0);
    }

    private static void TransferFlags(PrecisionContext ctxDst, PrecisionContext ctxSrc) {
      if (ctxDst != null && ctxDst.HasFlags) {
        if ((ctxSrc.Flags & (PrecisionContext.FlagInvalid | PrecisionContext.FlagDivideByZero)) != 0) {
          ctxDst.Flags |= (ctxSrc.Flags & (PrecisionContext.FlagInvalid | PrecisionContext.FlagDivideByZero));
        } else {
          ctxDst.Flags |= ctxSrc.Flags;
        }
      }
    }

    /// <summary>Finds the remainder that results when dividing two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The remainder of the two objects.</returns>
    public T Remainder(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      PrecisionContext ctx2 = ctx == null ? null : ctx.WithBlankFlags();
      T ret = RemainderHandleSpecial(thisValue, divisor, ctx2);
      if ((Object)ret != (Object)default(T)) {
        TransferFlags(ctx, ctx2);
        return ret;
      }
      ret = DivideToIntegerZeroScale(thisValue, divisor, ctx2);
      if((ctx2.Flags&PrecisionContext.FlagInvalid)!=0){
        return SignalInvalid(ctx);
      }
      ret = Add(thisValue, NegateRaw(Multiply(ret, divisor, null)), ctx2);
      ret = EnsureSign(ret, (helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative) != 0);
      TransferFlags(ctx, ctx2);
      return ret;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RemainderNear(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      PrecisionContext ctx2 = ctx == null ?
        PrecisionContext.ForRounding(Rounding.HalfEven).WithBlankFlags() :
        ctx.WithRounding(Rounding.HalfEven).WithBlankFlags();
      T ret = RemainderHandleSpecial(thisValue, divisor, ctx2);
      if ((Object)ret != (Object)default(T)) {
        TransferFlags(ctx, ctx2);
        return ret;
      }
      ret = DivideInternal(thisValue, divisor, ctx2,
                           IntegerModeFixedScale, BigInteger.Zero);
      if ((ctx2.Flags & (PrecisionContext.FlagInvalid)) != 0) {
        return SignalInvalid(ctx);
      }
      ctx2 = ctx2.WithBlankFlags();
      ret = RoundToPrecision(ret, ctx2);
      if ((ctx2.Flags & (PrecisionContext.FlagRounded | PrecisionContext.FlagInvalid)) != 0) {
        return SignalInvalid(ctx);
      }
      ctx2 = ctx == null ? PrecisionContext.Unlimited.WithBlankFlags() :
        ctx.WithBlankFlags();
      T ret2 = Add(thisValue, NegateRaw(Multiply(ret, divisor, null)), ctx2);
      if ((ctx2.Flags & (PrecisionContext.FlagInvalid)) != 0) {
        return SignalInvalid(ctx);
      }
      if (helper.GetFlags(ret2) == 0 && helper.GetMantissa(ret2).IsZero) {
        ret2 = EnsureSign(ret2, (helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative) != 0);
      }
      TransferFlags(ctx, ctx2);
      return ret2;
    }

    /// <summary> </summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Pi(PrecisionContext ctx){
      if((ctx==null || (ctx.Precision).IsZero))
        throw new ArgumentException("ctx is null or has unlimited precision");
      // Gauss-Legendre algorithm
      T a=helper.ValueOf(1);
      PrecisionContext ctxdiv=ctx.WithBigPrecision((ctx.Precision)+(BigInteger)10)
        .WithRounding(Rounding.ZeroFiveUp);
      T two=helper.ValueOf(2);
      T b=Divide(a,SquareRoot(two,ctxdiv),ctxdiv);
      T four=helper.ValueOf(4);
      T half=((thisRadix&1)==0) ?
        helper.CreateNewWithFlags((BigInteger)(thisRadix/2),
                                  BigInteger.Zero-BigInteger.One,0) : default(T);
      T t=Divide(a,four,ctxdiv);
      bool more = true;
      int lastCompare=0;
      int vacillations=0;
      T lastGuess=default(T);
      T guess=default(T);
      BigInteger powerTwo=BigInteger.One;
      while (more) {
        lastGuess = guess;
        T aplusB=Add(a,b,null);
        T newA=(half==null) ? Divide(aplusB,two,ctxdiv) : Multiply(aplusB,half,null);
        T aMinusNewA=Add(a,NegateRaw(newA),null);
        if(!a.Equals(b)){
          T atimesB=Multiply(a,b,ctxdiv);
          b=SquareRoot(atimesB,ctxdiv);
        }
        a=newA;
        guess=Multiply(aplusB,aplusB,null);
        guess=Divide(guess,Multiply(t,four,null),ctxdiv);
        T newGuess = guess;
        if((Object)lastGuess!=(Object)default(T)){
          int guessCmp=CompareTo(lastGuess,newGuess);
          if (guessCmp==0) {
            more = false;
          } else if((guessCmp>0 && lastCompare<0) || (lastCompare>0 && guessCmp<0)){
            // Guesses are vacillating
            vacillations++;
            if(vacillations>3 && guessCmp>0){
              // When guesses are vacillating, choose the lower guess
              // to reduce rounding errors
              more=false;
            }
          }
          lastCompare=guessCmp;
        }
        if(more){
          T tmpT=Multiply(aMinusNewA,aMinusNewA,null);
          tmpT=Multiply(tmpT,helper.CreateNewWithFlags(powerTwo,BigInteger.Zero,0),null);
          t=Add(t,NegateRaw(tmpT),ctxdiv);
          powerTwo<<=1;
        }
        guess=newGuess;
      }
      return RoundToPrecision(guess,ctx);
    }

    private T LnSeries1(T thisValue, PrecisionContext ctx){
      bool more = true;
      int lastCompare=0;
      int vacillations=0;
      PrecisionContext ctxdiv=ctx.WithBigPrecision((ctx.Precision)+(BigInteger)6)
        .WithRounding(Rounding.ZeroFiveUp);
      T z=Add(NegateRaw(thisValue),helper.ValueOf(1),null);
      T zpow=Multiply(z,z,ctxdiv);
      T guess=NegateRaw(z);
      T lastGuess=default(T);
      BigInteger denom=(BigInteger)2;
      while (more) {
        lastGuess = guess;
        T tmp=Divide(zpow,helper.CreateNewWithFlags(denom,BigInteger.Zero,0),ctxdiv);
        T newGuess=Add(guess,NegateRaw(tmp),ctxdiv);
        {
          int guessCmp=CompareTo(lastGuess,newGuess);
          if (guessCmp==0) {
            more = false;
          } else if((guessCmp>0 && lastCompare<0) || (lastCompare>0 && guessCmp<0)){
            // Guesses are vacillating
            vacillations++;
            if(vacillations>3 && guessCmp>0){
              // When guesses are vacillating, choose the lower guess
              // to reduce rounding errors
              more=false;
            }
          }
          lastCompare=guessCmp;
        }
        guess=newGuess;
        if(more){
          zpow=Multiply(zpow,z,ctxdiv);
          denom+=BigInteger.One;
        }
      }
      return RoundToPrecision(guess,ctx);
    }

    /*
    private T LnSeries2(T thisValue, PrecisionContext ctx){
      bool more = true;
      int lastCompare=0;
      int vacillations=0;
      PrecisionContext ctxdiv=ctx.WithBigPrecision((ctx.Precision)+(BigInteger)10)
        .WithRounding(Rounding.ZeroFiveUp);
      T z=Add(thisValue,helper.ValueOf(-1),null);
      T zpow=Multiply(z,z,ctxdiv);
      T guess=NegateRaw(z);
      T lastGuess=default(T);
      bool negative=false;
      BigInteger denom=(BigInteger)2;
      while (more) {
        lastGuess = guess;
        T tmp=Divide(zpow,helper.CreateNewWithFlags(denom,BigInteger.Zero,0),ctxdiv);
        if(negative)tmp=NegateRaw(tmp);
        T newGuess=Add(guess,NegateRaw(tmp),ctxdiv);
        {
          int guessCmp=CompareTo(lastGuess,newGuess);
          if (guessCmp==0) {
            more = false;
          } else if((guessCmp>0 && lastCompare<0) || (lastCompare>0 && guessCmp<0)){
            // Guesses are vacillating
            vacillations++;
            if(vacillations>3 && guessCmp>0){
              // When guesses are vacillating, choose the lower guess
              // to reduce rounding errors
              more=false;
            }
          }
          lastCompare=guessCmp;
        }
        guess=newGuess;
        if(more){
          zpow=Multiply(zpow,z,ctxdiv);
          negative=!negative;
          denom+=BigInteger.One;
        }
      }
      return RoundToPrecision(guess,ctx);
    }*/

    private T ExpInternal(T thisValue, PrecisionContext ctx){
      T one=helper.ValueOf(1);
      PrecisionContext ctxdiv=ctx.WithBigPrecision((ctx.Precision)+(BigInteger)6)
        .WithRounding(Rounding.ZeroFiveUp);
      BigInteger bigintN=(BigInteger)2;
      BigInteger facto=BigInteger.One;
      // Guess starts with 1 + thisValue
      T guess=Add(one,thisValue,null);
      T lastGuess=guess;
      T pow=thisValue;
      bool more = true;
      int lastCompare=0;
      int vacillations=0;
      while (more) {
        lastGuess = guess;
        // Iterate by:
        // newGuess = guess + (thisValue^n/factorial(n))
        // (n starts at 2 and increases by 1 after
        // each iteration)
        pow=Multiply(pow,thisValue,ctxdiv);
        facto*=(BigInteger)bigintN;
        T tmp=Divide(pow,helper.CreateNewWithFlags(facto,BigInteger.Zero,0),ctxdiv);
        T newGuess = Add(guess,tmp,ctxdiv);
        {
          int guessCmp=CompareTo(lastGuess,newGuess);
          if (guessCmp==0) {
            more = false;
          } else if((guessCmp>0 && lastCompare<0) || (lastCompare>0 && guessCmp<0)){
            // Guesses are vacillating
            vacillations++;
            if(vacillations>3 && guessCmp>0){
              // When guesses are vacillating, choose the lower guess
              // to reduce rounding errors
              more=false;
            }
          }
          lastCompare=guessCmp;
        }
        guess=newGuess;
        if(more){
          bigintN+=BigInteger.One;
        }
      }
      return RoundToPrecision(guess,ctx);
    }
    private T PowerIntegral(T thisValue, BigInteger powIntBig, PrecisionContext ctx){
      int sign=powIntBig.Sign;
      T one=helper.ValueOf(1);
      if (sign==0)
        return RoundToPrecision(one,ctx); // however 0 to the power of 0 is undefined
      else if (powIntBig.Equals(BigInteger.One))
        return RoundToPrecision(thisValue,ctx);
      else if (powIntBig.Equals((BigInteger)2))
        return Multiply(thisValue,thisValue,ctx);
      else if (powIntBig.Equals((BigInteger)3))
        return Multiply(thisValue,Multiply(thisValue,thisValue,null),ctx);
      bool retvalNeg=IsNegative(thisValue) && !(powIntBig.IsEven);
      FastInteger error=helper.CreateShiftAccumulator(
        BigInteger.Abs(powIntBig)).GetDigitLength();
      error.AddInt(6);
      BigInteger bigError=error.AsBigInteger();
      PrecisionContext ctxdiv=ctx.WithBigPrecision(
        (ctx.Precision)+bigError)
        .WithRounding(Rounding.ZeroFiveUp).WithBlankFlags();
      if (sign < 0){
        // Use the reciprocal for negative powers
        thisValue=Divide(one,thisValue,ctxdiv);
        powIntBig=-powIntBig;
      }
      T r=one;
      while (!powIntBig.IsZero) {
        if (!powIntBig.IsEven) {
          r=Multiply(r,thisValue,ctxdiv);
          //  Console.WriteLine(r);
          if((ctxdiv.Flags&PrecisionContext.FlagOverflow)!=0){
            return SignalOverflow2(ctx,retvalNeg);
          }
        }
        powIntBig >>= 1;
        if (!powIntBig.IsZero) {
          ctxdiv.Flags=0;
          T tmp=Multiply(thisValue,thisValue,ctxdiv);
          if((ctxdiv.Flags&PrecisionContext.FlagOverflow)!=0){
            // Avoid multiplying too huge numbers with
            // limited exponent range
            return SignalOverflow2(ctx,retvalNeg);
          }
          thisValue=tmp;
        }
      }
      return RoundToPrecision(r,ctx);
    }

    private T ExtendPrecision(T thisValue, PrecisionContext ctx){
      if(ctx==null || (ctx.Precision).IsZero){
        return RoundToPrecision(thisValue,ctx);
      }
      BigInteger mant=BigInteger.Abs(helper.GetMantissa(thisValue));
      FastInteger digits=helper.CreateShiftAccumulator(mant).GetDigitLength();
      FastInteger fastPrecision=FastInteger.FromBig(ctx.Precision);
      BigInteger exponent=helper.GetExponent(thisValue);
      if(digits.CompareTo(fastPrecision)<0){
        fastPrecision.Subtract(digits);
        mant=helper.MultiplyByRadixPower(mant,fastPrecision);
        BigInteger bigPrec=fastPrecision.AsBigInteger();
        exponent-=(BigInteger)bigPrec;
      }
      if(ctx!=null && ctx.HasFlags){
        ctx.Flags|=PrecisionContext.FlagRounded;
        ctx.Flags|=PrecisionContext.FlagInexact;
      }
      return RoundToPrecision(helper.CreateNewWithFlags(
        mant,exponent,0),ctx);
    }

    private bool IsWithinExponentRangeForPow(T thisValue, PrecisionContext ctx){
      if(ctx==null || !ctx.HasExponentRange)
        return true;
      FastInteger digits=helper.CreateShiftAccumulator(BigInteger.Abs(
        helper.GetMantissa(thisValue))).GetDigitLength();
      BigInteger exp=helper.GetExponent(thisValue);
      FastInteger fi=FastInteger.FromBig(exp);
      fi.Add(digits);
      fi.Decrement();
      //Console.WriteLine("{0} -> {1}",exp,fi);
      if(fi.Sign<0){
        fi.Negate().Divide(2).Negate();
        //   Console.WriteLine("{0} II -> {1}",exp,fi);
      }
      exp=fi.AsBigInteger();
      if(exp.CompareTo(ctx.EMin)<0 || exp.CompareTo(ctx.EMax)>0){
        return false;
      }
      return true;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='pow'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Power(T thisValue, T pow, PrecisionContext ctx){
      T ret = HandleNotANumber(thisValue, pow, ctx);
      if ((Object)ret != (Object)default(T)) {
        return ret;
      }
      int thisSign=helper.GetSign(thisValue);
      int powSign=helper.GetSign(pow);
      int thisFlags=helper.GetFlags(thisValue);
      int powFlags=helper.GetFlags(pow);
      if(thisSign==0 && powSign==0){
        // Both operands are zero: invalid
        return SignalInvalid(ctx);
      }
      if(thisSign<0 && (powFlags&BigNumberFlags.FlagInfinity)!=0){
        // This value is negative and power is infinity: invalid
        return SignalInvalid(ctx);
      }
      if(thisSign>0 &&
         (thisFlags&BigNumberFlags.FlagInfinity)==0 &&
         (powFlags&BigNumberFlags.FlagInfinity)!=0){
        // Power is infinity and this value is greater than
        // zero and not infinity
        int cmp=CompareTo(thisValue,helper.ValueOf(1));
        if(cmp<0){
          // Value is less than 1
          if(powSign<0){
            // Power is negative infinity, return positive infinity
            return helper.CreateNewWithFlags(
              BigInteger.Zero,
              BigInteger.Zero,BigNumberFlags.FlagInfinity);
          } else {
            // Power is positive infinity, return 0
            return RoundToPrecision(helper.CreateNewWithFlags(BigInteger.Zero,
                                                              BigInteger.Zero,0),ctx);
          }
        } else if(cmp==0){
          // Extend the precision of the mantissa as much as possible,
          // in the special case that this value is 1
          return ExtendPrecision(helper.ValueOf(1),ctx);
        } else {
          // Value is greater than 1
          if(powSign>0){
            // Power is positive infinity, return positive infinity
            return pow;
          } else {
            // Power is negative infinity, return 0
            return RoundToPrecision(helper.CreateNewWithFlags(BigInteger.Zero,
                                                              BigInteger.Zero,0),ctx);
          }
        }
      }
      BigInteger powExponent=helper.GetExponent(pow);
      bool isPowIntegral=powExponent.Sign>0;
      bool isPowOdd=false;
      T powInt=default(T);
      if(!isPowIntegral){
        powInt=Quantize(pow,helper.CreateNewWithFlags(BigInteger.Zero,
                                                      BigInteger.Zero,0),
                        PrecisionContext.ForRounding(Rounding.Down));
        isPowIntegral=CompareTo(powInt,pow)==0;
        isPowOdd=!(helper.GetMantissa(powInt).IsEven);
      } else {
        if(powExponent.Equals(BigInteger.Zero)){
          isPowOdd=!(helper.GetMantissa(powInt).IsEven);
        } else if(thisRadix%2==0){
          // Never odd for even radixes
          isPowOdd=false;
        } else {
          powInt=Quantize(pow,helper.CreateNewWithFlags(BigInteger.Zero,
                                                        BigInteger.Zero,0),
                          PrecisionContext.ForRounding(Rounding.Down));
          isPowOdd=!(helper.GetMantissa(powInt).IsEven);
        }
      }
      //  Console.WriteLine("pow={0} powint={1}",pow,powInt);
      bool isResultNegative=false;
      if((thisFlags&BigNumberFlags.FlagNegative)!=0 &&
         (powFlags&BigNumberFlags.FlagInfinity)==0 && isPowIntegral && isPowOdd){
        isResultNegative=true;
      }
      if(thisSign==0 && powSign!=0){
        int infinityFlags=(powSign<0) ? BigNumberFlags.FlagInfinity : 0;
        if(isResultNegative){
          infinityFlags|=BigNumberFlags.FlagNegative;
        }
        thisValue=helper.CreateNewWithFlags(
          BigInteger.Zero,
          BigInteger.Zero,infinityFlags);
        if((infinityFlags&BigNumberFlags.FlagInfinity)==0){
          thisValue=RoundToPrecision(thisValue,ctx);
        }
        return thisValue;
      }
      if((!isPowIntegral || powSign<0)&& (ctx==null || (ctx.Precision).IsZero))
        throw new ArgumentException("ctx is null or has unlimited precision, and pow's exponent is not an integer or is negative");
      if(thisSign<0 && !isPowIntegral){
        return SignalInvalid(ctx);
      }
      if((thisFlags&BigNumberFlags.FlagInfinity)!=0){
        // This value is infinity
        if(powSign>0){
          return RoundToPrecision(helper.CreateNewWithFlags(
            BigInteger.Zero,
            BigInteger.Zero,
            (isResultNegative ?
             BigNumberFlags.FlagNegative : 0)|BigNumberFlags.FlagInfinity),ctx);
        } else if(powSign<0){
          return RoundToPrecision(helper.CreateNewWithFlags(
            BigInteger.Zero,
            BigInteger.Zero,
            (isResultNegative ?
             BigNumberFlags.FlagNegative : 0)),ctx);
        } else {
          return RoundToPrecision(helper.CreateNewWithFlags(BigInteger.One,
                                                            BigInteger.Zero,0),ctx);
        }
      }
      if(powSign==0){
        return RoundToPrecision(helper.CreateNewWithFlags(BigInteger.One,
                                                          BigInteger.Zero,0),ctx);
      }
      if(isPowIntegral){
        // Special case for 1
        if(CompareTo(thisValue,helper.ValueOf(1))==0){
          if(!IsWithinExponentRangeForPow(pow,ctx)){
            return SignalInvalid(ctx);
          }
          return helper.ValueOf(1);
        }
        if((Object)powInt==(Object)default(T)){
          powInt=Quantize(pow,helper.CreateNewWithFlags(BigInteger.Zero,
                                                        BigInteger.Zero,0),
                          PrecisionContext.ForRounding(Rounding.Down));
        }
        BigInteger signedMant=BigInteger.Abs(helper.GetMantissa(powInt));
        if(powSign<0)signedMant=-signedMant;
        //        Console.WriteLine("tv={0} mant={1}",thisValue,signedMant);
        return PowerIntegral(thisValue,signedMant,ctx);
      }
      // Special case for 1
      if(CompareTo(thisValue,helper.ValueOf(1))==0 && powSign>0){
        if(!IsWithinExponentRangeForPow(pow,ctx)){
          return SignalInvalid(ctx);
        }
        return ExtendPrecision(helper.ValueOf(1),ctx);
      }
      #if DEBUG
if((ctx)==null)throw new ArgumentNullException("ctx");
#endif

      PrecisionContext ctxdiv=ctx.WithBigPrecision((ctx.Precision)+(BigInteger)10).WithRounding(Rounding.ZeroFiveUp).WithBlankFlags();
      T lnresult=Ln(thisValue,ctxdiv);
      lnresult=Multiply(lnresult,pow,null);
      ctxdiv=ctx.WithBlankFlags();
      lnresult=Exp(lnresult,ctxdiv);
      if((ctxdiv.Flags&(PrecisionContext.FlagClamped|PrecisionContext.FlagOverflow))!=0){
        if(!IsWithinExponentRangeForPow(thisValue,ctx)){
          return SignalInvalid(ctx);
        }
        if(!IsWithinExponentRangeForPow(pow,ctx)){
          return SignalInvalid(ctx);
        }
      }
      if(ctx.HasFlags){
        ctx.Flags|=ctxdiv.Flags;
      }
      return lnresult;
    }
    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Log10(T thisValue, PrecisionContext ctx){
      if(ctx==null || (ctx.Precision).IsZero)
        throw new ArgumentException("ctx is null or has unlimited precision");
      int flags=helper.GetFlags(thisValue);
      if((flags&BigNumberFlags.FlagSignalingNaN)!=0){
        // NOTE: Returning a signaling NaN is independent of
        // rounding mode
        return SignalingNaNInvalid(thisValue,ctx);
      }
      if((flags&BigNumberFlags.FlagQuietNaN)!=0){
        // NOTE: Returning a quiet NaN is independent of
        // rounding mode
        return ReturnQuietNaN(thisValue,ctx);
      }
      int sign=helper.GetSign(thisValue);
      if(sign<0)
        return SignalInvalid(ctx);
      if((flags&BigNumberFlags.FlagInfinity)!=0){
        return thisValue;
      }
      PrecisionContext ctxCopy=ctx.WithBlankFlags();
      T one=helper.ValueOf(1);
      if(sign==0){
        // Result is negative infinity if input is 0
        thisValue=RoundToPrecision(helper.CreateNewWithFlags(
          BigInteger.Zero,BigInteger.Zero,
          BigNumberFlags.FlagNegative|BigNumberFlags.FlagInfinity),ctxCopy);
      } else if(CompareTo(thisValue,one)==0){
        // Result is 0 if input is 1
        thisValue=RoundToPrecision(helper.CreateNewWithFlags(BigInteger.Zero,
                                                             BigInteger.Zero,0),ctxCopy);
      } else {
        BigInteger exp=helper.GetExponent(thisValue);
        BigInteger mant=BigInteger.Abs(helper.GetMantissa(thisValue));
        if(mant.Equals(BigInteger.One) && thisRadix==10){
          // Value is 1 and radix is 10, so the result is the exponent
          thisValue=RoundToPrecision(helper.CreateNewWithFlags(
            exp,BigInteger.Zero,
            exp.Sign<0 ? BigNumberFlags.FlagNegative : 0),ctxCopy);
        } else {
          BigInteger mantissa=helper.GetMantissa(thisValue);
          FastInteger expTmp=FastInteger.FromBig(exp);
          BigInteger tenBig=(BigInteger)10;
          while (true) {
            BigInteger bigrem;
            BigInteger bigquo = BigInteger.DivRem(mantissa, tenBig, out bigrem);
            if (!bigrem.IsZero)
              break;
            mantissa = bigquo;
            expTmp.Increment();
          }
          if(mantissa.CompareTo(BigInteger.One)==0){
            // Value is an integer power of 10
            thisValue=RoundToPrecision(helper.CreateNewWithFlags(
              expTmp.AsBigInteger(),BigInteger.Zero,
              expTmp.Sign<0 ? BigNumberFlags.FlagNegative : 0),ctxCopy);
          } else {
            PrecisionContext ctxdiv=ctx.WithBigPrecision((ctx.Precision)+(BigInteger)10)
              .WithRounding(Rounding.ZeroFiveUp).WithBlankFlags();
            T ten=helper.CreateNewWithFlags(
              (BigInteger)10,BigInteger.Zero,0);
            T lnNatural=Ln(thisValue,ctxdiv);
            T lnTen=Ln(ten,ctxdiv);
            thisValue=Divide(lnNatural,lnTen,ctx);
          }
        }
      }
      if(ctx.HasFlags){
        ctx.Flags|=ctxCopy.Flags;
      }
      return thisValue;
    }

    private static BigInteger PowerOfTwo(FastInteger fi){
      if(fi.Sign<=0)return BigInteger.One;
      if(fi.CanFitInInt32()){
        int val=fi.AsInt32();
        if(val<=30){
          val=1<<val;
          return (BigInteger)val;
        }
        return BigInteger.One<<val;
      } else {
        BigInteger bi=BigInteger.One;
        FastInteger fi2=FastInteger.Copy(fi);
        while (fi2.Sign > 0) {
          int count = 1000000;
          if (fi2.CompareToInt(1000000) < 0) {
            count = (int)bi;
          }
          bi<<=count;
          fi2.SubtractInt(count);
        }
        return bi;
      }
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Ln(T thisValue, PrecisionContext ctx){
      if(ctx==null || (ctx.Precision).IsZero)
        throw new ArgumentException("ctx is null or has unlimited precision");
      int flags=helper.GetFlags(thisValue);
      if((flags&BigNumberFlags.FlagSignalingNaN)!=0){
        // NOTE: Returning a signaling NaN is independent of
        // rounding mode
        return SignalingNaNInvalid(thisValue,ctx);
      }
      if((flags&BigNumberFlags.FlagQuietNaN)!=0){
        // NOTE: Returning a quiet NaN is independent of
        // rounding mode
        return ReturnQuietNaN(thisValue,ctx);
      }
      int sign=helper.GetSign(thisValue);
      if(sign<0)
        return SignalInvalid(ctx);
      if((flags&BigNumberFlags.FlagInfinity)!=0){
        return thisValue;
      }
      PrecisionContext ctxCopy=ctx.WithBlankFlags();
      T one=helper.ValueOf(1);
      if(sign==0){
        return helper.CreateNewWithFlags(BigInteger.Zero,BigInteger.Zero,
                                         BigNumberFlags.FlagNegative|BigNumberFlags.FlagInfinity);
      } else {
        int cmpOne=CompareTo(thisValue,one);
        PrecisionContext ctxdiv=null;
        if(cmpOne==0){
          // Equal to 1
          thisValue=RoundToPrecision(helper.CreateNewWithFlags(BigInteger.Zero,
                                                               BigInteger.Zero,0),ctxCopy);
        } else if(cmpOne<0){
          // Less than 1
          FastInteger error=helper.CreateShiftAccumulator(
            BigInteger.Abs(helper.GetMantissa(thisValue))).GetDigitLength();
          error.AddInt(6);
          BigInteger bigError=error.AsBigInteger();
          ctxdiv=ctx.WithBigPrecision(
            (ctx.Precision)+bigError)
            .WithRounding(Rounding.ZeroFiveUp).WithBlankFlags();
          T quarter=Divide(one,helper.ValueOf(4),ctxCopy);
          if(CompareTo(thisValue,quarter)<=0){
            // One quarter or less
            T half=Multiply(quarter,helper.ValueOf(2),null);
            FastInteger roots=new FastInteger(0);
            // Take square root until this value
            // is one half or more
            while(CompareTo(thisValue,half)<0){
              thisValue=SquareRoot(thisValue,ctxdiv.WithUnlimitedExponents());
              roots.Increment();
            }
            thisValue=LnSeries1(thisValue,ctxdiv);
            BigInteger biRoots=PowerOfTwo(roots);
            // Multiply back 2^X, where X is the number
            // of square root calls
            thisValue=Multiply(
              thisValue,helper.CreateNewWithFlags(
                biRoots,BigInteger.Zero,0),ctxCopy);
          } else {
            thisValue=LnSeries1(thisValue,ctxCopy);
          }
          if(ctx.HasFlags){
            ctxCopy.Flags|=PrecisionContext.FlagInexact;
            ctxCopy.Flags|=PrecisionContext.FlagRounded;
          }
        } else {
          // Greater than 1
          FastInteger error=helper.CreateShiftAccumulator(
            BigInteger.Abs(helper.GetMantissa(thisValue))).GetDigitLength();
          error.AddInt(6);
          BigInteger bigError=error.AsBigInteger();
          ctxdiv=ctx.WithBigPrecision(
            (ctx.Precision)+bigError)
            .WithRounding(Rounding.ZeroFiveUp).WithBlankFlags();
          T two=helper.ValueOf(2);
          if(CompareTo(thisValue,two)>=0){
            FastInteger roots=new FastInteger(0);
            // Take square root until this value
            // is less than 2
            while(CompareTo(thisValue,two)>=0){
              thisValue=SquareRoot(thisValue,ctxdiv.WithUnlimitedExponents());
              roots.Increment();
            }
            // Find -Ln(1/thisValue)
            thisValue=Divide(one,thisValue,ctxdiv);
            thisValue=LnSeries1(thisValue,ctxdiv);
            thisValue=NegateRaw(thisValue);
            BigInteger biRoots=PowerOfTwo(roots);
            // Multiply back 2^X, where X is the number
            // of square root calls
            thisValue=Multiply(thisValue,
                               helper.CreateNewWithFlags(biRoots,
                                                         BigInteger.Zero,0),ctxCopy);
          } else {
            // Find -Ln(1/thisValue)
            thisValue=Divide(one,thisValue,ctxdiv);
            thisValue=LnSeries1(thisValue,ctxdiv);
            thisValue=NegateRaw(thisValue);
            thisValue=RoundToPrecision(thisValue,ctxCopy);
          }
          if(ctx.HasFlags){
            ctxCopy.Flags|=PrecisionContext.FlagInexact;
            ctxCopy.Flags|=PrecisionContext.FlagRounded;
          }
        }
      }
      if(ctx.HasFlags){
        ctx.Flags|=ctxCopy.Flags;
      }
      return thisValue;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Exp(T thisValue, PrecisionContext ctx){
      if(ctx==null || (ctx.Precision).IsZero)
        throw new ArgumentException("ctx is null or has unlimited precision");
      int flags=helper.GetFlags(thisValue);
      if((flags&BigNumberFlags.FlagSignalingNaN)!=0){
        // NOTE: Returning a signaling NaN is independent of
        // rounding mode
        return SignalingNaNInvalid(thisValue,ctx);
      }
      if((flags&BigNumberFlags.FlagQuietNaN)!=0){
        // NOTE: Returning a quiet NaN is independent of
        // rounding mode
        return ReturnQuietNaN(thisValue,ctx);
      }
      PrecisionContext ctxCopy=ctx.WithBlankFlags();
      if((flags&BigNumberFlags.FlagInfinity)!=0){
        if((flags&BigNumberFlags.FlagNegative)!=0){
          T retval=RoundToPrecision(helper.CreateNewWithFlags(
            BigInteger.Zero,BigInteger.Zero,0),ctxCopy);
          if(ctx.HasFlags){
            ctx.Flags|=ctxCopy.Flags;
          }
          return retval;
        }
        return thisValue;
      }
      int sign=helper.GetSign(thisValue);
      T one=helper.ValueOf(1);
      PrecisionContext ctxdiv=ctx.WithBigPrecision((ctx.Precision)+(BigInteger)10)
        .WithRounding(Rounding.ZeroFiveUp)
        .WithBlankFlags();
      if(sign==0){
        thisValue=RoundToPrecision(one,ctxCopy);
      } else if(sign<0){
        T val=Exp(NegateRaw(thisValue),ctxdiv);
        if((ctxdiv.Flags&PrecisionContext.FlagOverflow)!=0 || !IsFinite(val)){
          // Overflow, try again with unlimited exponents
          ctxdiv.Flags=0;
          ctxdiv=ctxdiv.WithUnlimitedExponents();
          thisValue=Exp(NegateRaw(thisValue),ctxdiv);
        } else {
          thisValue=val;
        }
        //     Console.WriteLine("exp interim {0}",thisValue);
        thisValue=Divide(one,thisValue,ctxCopy);
        //   Console.WriteLine("exp final {0}",thisValue);
        if(ctx.HasFlags){
          ctx.Flags|=PrecisionContext.FlagInexact|PrecisionContext.FlagRounded;
        }
      } else if(CompareTo(thisValue,one)<0){
        thisValue=ExpInternal(thisValue,ctxCopy);
        if(ctx.HasFlags){
          ctx.Flags|=PrecisionContext.FlagInexact|PrecisionContext.FlagRounded;
        }
      } else {
        T intpart=Quantize(thisValue,one,PrecisionContext.ForRounding(Rounding.Down));
        T fracpart=Add(thisValue,NegateRaw(intpart),null);
        fracpart=Add(one,Divide(fracpart,intpart,ctxdiv),null);
        ctxdiv.Flags=0;
        thisValue=ExpInternal(fracpart,ctxdiv);
        if((ctxdiv.Flags&PrecisionContext.FlagUnderflow)!=0){
          if(ctx.HasFlags){
            ctx.Flags|=ctxdiv.Flags;
          }
        }
        if(ctx.HasFlags){
          ctx.Flags|=PrecisionContext.FlagInexact|PrecisionContext.FlagRounded;
        }
        thisValue=PowerIntegral(thisValue,helper.GetMantissa(intpart),ctxdiv);
        if((ctxdiv.Flags&PrecisionContext.FlagOverflow)!=0){
          if(ctx.HasFlags){
            ctx.Flags|=ctxdiv.Flags;
          }
        }
        thisValue=RoundToPrecision(thisValue,ctxCopy);
      }
      if(ctx.HasFlags){
        ctx.Flags|=ctxCopy.Flags;
      }
      return thisValue;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T SquareRoot(T thisValue, PrecisionContext ctx) {
      if(ctx==null || (ctx.Precision).IsZero)
        throw new ArgumentException("ctx is null or has unlimited precision");
      T ret = SquareRootHandleSpecial(thisValue, ctx);
      if ((Object)ret != (Object)default(T)) {
        return ret;
      }
      PrecisionContext ctxtmp=ctx.WithBlankFlags();
      BigInteger currentExp=helper.GetExponent(thisValue);
      BigInteger origExp=currentExp;
      BigInteger idealExp;
      idealExp=currentExp;
      idealExp/=(BigInteger)2;
      if(currentExp.Sign<0 && !currentExp.IsEven){
        // Round towards negative infinity; BigInteger's
        // division operation rounds towards zero
        idealExp-=BigInteger.One;
      }
      //Console.WriteLine("curr={0} ideal={1}",currentExp,idealExp);
      if(helper.GetSign(thisValue)==0){
        ret=RoundToPrecision(helper.CreateNewWithFlags(
          BigInteger.Zero,idealExp,helper.GetFlags(thisValue)),ctxtmp);
        if(ctx.HasFlags){
          ctx.Flags|=ctxtmp.Flags;
        }
        return ret;
      }
      BigInteger mantissa=BigInteger.Abs(helper.GetMantissa(thisValue));
      IShiftAccumulator accum=helper.CreateShiftAccumulator(mantissa);
      FastInteger digitCount=accum.GetDigitLength();
      FastInteger targetPrecision=FastInteger.FromBig(ctx.Precision);
      FastInteger precision=FastInteger.Copy(targetPrecision).Multiply(2).AddInt(2);
      bool rounded=false;
      bool inexact=false;
      if(digitCount.CompareTo(precision)<0){
        FastInteger diff=FastInteger.Copy(precision).Subtract(digitCount);
        //Console.WriteLine(diff);
        if((!diff.IsEvenNumber)^(!(origExp.IsEven))){
          diff.Increment();
        }
        BigInteger bigdiff=diff.AsBigInteger();
        currentExp-=(BigInteger)bigdiff;
        mantissa=helper.MultiplyByRadixPower(mantissa,diff);
      } else if(digitCount.CompareTo(precision)<0){
        FastInteger diff=FastInteger.Copy(digitCount).Subtract(precision);
        accum.ShiftRight(diff);
        BigInteger bigdiff=diff.AsBigInteger();
        currentExp+=(BigInteger)bigdiff;
        mantissa=accum.ShiftedInt;
        rounded=true;
        inexact=(accum.LastDiscardedDigit|accum.OlderDiscardedDigits)!=0;
      }
      BigInteger[] sr=mantissa.sqrtWithRemainder();
      digitCount=helper.CreateShiftAccumulator(sr[0]).GetDigitLength();
      //Console.WriteLine("I {0} -> {1} [target={2}], (zero={3})",
      //                mantissa,sr[0],targetPrecision,
      //                sr[1].IsZero);
      mantissa=sr[0];
      if(!(sr[1].IsZero)){
        rounded=true;
        inexact=true;
      }
      BigInteger oldexp=currentExp;
      currentExp/=(BigInteger)2;
      if(oldexp.Sign<0 && !oldexp.IsEven){
        // Round towards negative infinity; BigInteger's
        // division operation rounds towards zero
        currentExp-=BigInteger.One;
      }
      T retval=helper.CreateNewWithFlags(mantissa,currentExp,0);
      //Console.WriteLine("idealExp={0}, curr {1} guess={2}",idealExp,currentExp,mantissa);
      retval=RoundToPrecisionInternal(
        retval,0,inexact ? 1 : 0,null,false,false,ctxtmp);
      currentExp=helper.GetExponent(retval);
      //Console.WriteLine("guess I {0} idealExp={1}, curr {2} clamped={3}",guess,
      //                idealExp,currentExp,(ctxtmp.Flags&PrecisionContext.FlagClamped));
      if((ctxtmp.Flags&PrecisionContext.FlagUnderflow)==0){
        int expcmp=currentExp.CompareTo(idealExp);
        if(expcmp<=0 || !IsFinite(retval)){
          retval=ReduceToPrecisionAndIdealExponent(
            retval,ctx.HasExponentRange ? ctxtmp : null,inexact ? targetPrecision : null,
            FastInteger.FromBig(idealExp));
        }
      }
      if(ctx.HasFlags && ctx.ClampNormalExponents &&
         !helper.GetExponent(retval).Equals(idealExp) &&
         (ctxtmp.Flags&PrecisionContext.FlagInexact)==0){
        ctx.Flags|=PrecisionContext.FlagClamped;
      }
      if((ctxtmp.Flags&PrecisionContext.FlagOverflow)!=0){
        rounded=true;
      }
      //Console.WriteLine("guess II {0}",guess);
      currentExp=helper.GetExponent(retval);
      if(rounded){
        ctxtmp.Flags|=PrecisionContext.FlagRounded;
      } else {
        if(currentExp.CompareTo(idealExp)>0){
          // Greater than the ideal, treat as rounded anyway
          ctxtmp.Flags|=PrecisionContext.FlagRounded;
        } else {
          //          Console.WriteLine("idealExp={0}, curr {1} (II)",idealExp,currentExp);
          ctxtmp.Flags&=~PrecisionContext.FlagRounded;
        }
      }
      if(inexact){
        ctxtmp.Flags|=PrecisionContext.FlagRounded;
        ctxtmp.Flags|=PrecisionContext.FlagInexact;
      }
      if(ctx.HasFlags){
        ctx.Flags|=ctxtmp.Flags;
      }
      return retval;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T NextMinus(
      T thisValue,
      PrecisionContext ctx
     ) {
      if ((ctx) == null) throw new ArgumentNullException("ctx");
      if ((ctx.Precision).Sign <= 0) throw new ArgumentException("ctx.Precision" + " not less than " + "0" + " (" + Convert.ToString((ctx.Precision), System.Globalization.CultureInfo.InvariantCulture) + ")");
      if (!(ctx.HasExponentRange)) throw new ArgumentException("doesn't satisfy ctx.HasExponentRange");
      int flags = helper.GetFlags(thisValue);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return SignalingNaNInvalid(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return ReturnQuietNaN(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagInfinity) != 0) {
        if ((flags & BigNumberFlags.FlagNegative) != 0) {
          return thisValue;
        } else {
          BigInteger bigexp2 = ctx.EMax;
          BigInteger bigprec = ctx.Precision;
          bigexp2 += BigInteger.One;
          bigexp2 -= (BigInteger)bigprec;
          BigInteger overflowMant = helper.MultiplyByRadixPower(
            BigInteger.One, FastInteger.FromBig(ctx.Precision));
          overflowMant -= BigInteger.One;
          return helper.CreateNewWithFlags(overflowMant, bigexp2, 0);
        }
      }
      FastInteger minexp = FastInteger.FromBig(ctx.EMin).SubtractBig(ctx.Precision).Increment();
      FastInteger bigexp = FastInteger.FromBig(helper.GetExponent(thisValue));
      if (bigexp.CompareTo(minexp) <= 0) {
        // Use a smaller exponent if the input exponent is already
        // very small
        minexp = FastInteger.Copy(bigexp).SubtractInt(2);
      }
      T quantum = helper.CreateNewWithFlags(
        BigInteger.One,
        minexp.AsBigInteger(), BigNumberFlags.FlagNegative);
      PrecisionContext ctx2;
      ctx2 = ctx.WithRounding(Rounding.Floor);
      return Add(thisValue, quantum, ctx2);
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='otherValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T NextToward(
      T thisValue,
      T otherValue,
      PrecisionContext ctx
     ) {
      if ((ctx) == null) throw new ArgumentNullException("ctx");
      if ((ctx.Precision).Sign <= 0) throw new ArgumentException("ctx.Precision" + " not less than " + "0" + " (" + Convert.ToString((ctx.Precision), System.Globalization.CultureInfo.InvariantCulture) + ")");
      if (!(ctx.HasExponentRange)) throw new ArgumentException("doesn't satisfy ctx.HasExponentRange");
      int thisFlags = helper.GetFlags(thisValue);
      int otherFlags = helper.GetFlags(otherValue);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = HandleNotANumber(thisValue, otherValue, ctx);
        if ((Object)result != (Object)default(T)) return result;
      }
      PrecisionContext ctx2;
      int cmp = CompareTo(thisValue, otherValue);
      if (cmp == 0) {
        return RoundToPrecision(
          EnsureSign(thisValue, (otherFlags & BigNumberFlags.FlagNegative) != 0),
          ctx.WithNoFlags());
      } else {
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          if ((thisFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) ==
              (otherFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative))) {
            // both values are the same infinity
            return thisValue;
          } else {
            BigInteger bigexp2 = ctx.EMax;
            BigInteger bigprec = ctx.Precision;
            bigexp2 += BigInteger.One;
            bigexp2 -= (BigInteger)bigprec;
            BigInteger overflowMant = helper.MultiplyByRadixPower(
              BigInteger.One, FastInteger.FromBig(ctx.Precision));
            overflowMant -= BigInteger.One;
            return helper.CreateNewWithFlags(overflowMant, bigexp2,
                                             thisFlags & BigNumberFlags.FlagNegative);
          }
        }
        FastInteger minexp = FastInteger.FromBig(ctx.EMin).SubtractBig(ctx.Precision).Increment();
        FastInteger bigexp = FastInteger.FromBig(helper.GetExponent(thisValue));
        if (bigexp.CompareTo(minexp) < 0) {
          // Use a smaller exponent if the input exponent is already
          // very small
          minexp = FastInteger.Copy(bigexp).SubtractInt(2);
        } else {
          // Ensure the exponent is lower than the exponent range
          // (necessary to flag underflow correctly)
          minexp.SubtractInt(2);
        }
        T quantum = helper.CreateNewWithFlags(
          BigInteger.One, minexp.AsBigInteger(),
          (cmp > 0) ? BigNumberFlags.FlagNegative : 0);
        T val = thisValue;
        ctx2 = ctx.WithRounding((cmp > 0) ? Rounding.Floor : Rounding.Ceiling).WithBlankFlags();
        val = Add(val, quantum, ctx2);
        if ((ctx2.Flags & (PrecisionContext.FlagOverflow | PrecisionContext.FlagUnderflow)) == 0) {
          // Don't set flags except on overflow or underflow
          // TODO: Pending clarification from Mike Cowlishaw,
          // author of the Decimal Arithmetic test cases from
          // speleotrove.com
          ctx2.Flags = 0;
        }
        if ((ctx2.Flags & (PrecisionContext.FlagUnderflow)) != 0) {
          BigInteger bigmant = BigInteger.Abs(helper.GetMantissa(val));
          BigInteger maxmant = helper.MultiplyByRadixPower(
            BigInteger.One, FastInteger.FromBig(ctx.Precision).Decrement());
          if (bigmant.CompareTo(maxmant) >= 0 || (ctx.Precision).CompareTo(BigInteger.One) == 0) {
            // don't treat max-precision results as having underflowed
            ctx2.Flags = 0;
          }
        }
        if (ctx.HasFlags) {
          ctx.Flags |= ctx2.Flags;
        }
        return val;
      }
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T NextPlus(
      T thisValue,
      PrecisionContext ctx
     ) {
      if ((ctx) == null) throw new ArgumentNullException("ctx");
      if ((ctx.Precision).Sign <= 0) throw new ArgumentException("ctx.Precision" + " not less than " + "0" + " (" + Convert.ToString((ctx.Precision), System.Globalization.CultureInfo.InvariantCulture) + ")");
      if (!(ctx.HasExponentRange)) throw new ArgumentException("doesn't satisfy ctx.HasExponentRange");
      int flags = helper.GetFlags(thisValue);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return SignalingNaNInvalid(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return ReturnQuietNaN(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagInfinity) != 0) {
        if ((flags & BigNumberFlags.FlagNegative) != 0) {
          BigInteger bigexp2 = ctx.EMax;
          BigInteger bigprec = ctx.Precision;
          bigexp2 += BigInteger.One;
          bigexp2 -= (BigInteger)bigprec;
          BigInteger overflowMant = helper.MultiplyByRadixPower(
            BigInteger.One, FastInteger.FromBig(ctx.Precision));
          overflowMant -= BigInteger.One;
          return helper.CreateNewWithFlags(overflowMant, bigexp2, BigNumberFlags.FlagNegative);
        } else {
          return thisValue;
        }
      }
      FastInteger minexp = FastInteger.FromBig(ctx.EMin).SubtractBig(ctx.Precision).Increment();
      FastInteger bigexp = FastInteger.FromBig(helper.GetExponent(thisValue));
      if (bigexp.CompareTo(minexp) <= 0) {
        // Use a smaller exponent if the input exponent is already
        // very small
        minexp = FastInteger.Copy(bigexp).SubtractInt(2);
      }
      T quantum = helper.CreateNewWithFlags(
        BigInteger.One,
        minexp.AsBigInteger(), 0);
      PrecisionContext ctx2;
      T val = thisValue;
      ctx2 = ctx.WithRounding(Rounding.Ceiling);
      return Add(val, quantum, ctx2);
    }

    /// <summary>Divides two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object.</param>
    /// <param name='desiredExponent'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object. Precision is ignored.</param>
    /// <returns>The quotient of the two objects.</returns>
    public T DivideToExponent(
      T thisValue,
      T divisor,
      BigInteger desiredExponent,
      PrecisionContext ctx
     ) {
      if (ctx != null && !ctx.ExponentWithinRange(desiredExponent))
        return SignalInvalidWithMessage(ctx, "Exponent not within exponent range: " + desiredExponent.ToString());
      PrecisionContext ctx2 = (ctx == null) ?
        PrecisionContext.ForRounding(Rounding.HalfDown) :
        ctx.WithUnlimitedExponents().WithPrecision(0);
      T ret = DivideInternal(thisValue, divisor,
                             ctx2,
                             IntegerModeFixedScale, desiredExponent);
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= ctx2.Flags;
      }
      return ret;
    }

    /// <summary>Divides two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The quotient of the two objects.</returns>
    public T Divide(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      return DivideInternal(thisValue, divisor,
                            ctx, IntegerModeRegular, BigInteger.Zero);
    }

    private int[] RoundToScaleStatus(
      BigInteger remainder,// Assumes value is nonnegative
      BigInteger divisor,// Assumes value is nonnegative
      bool neg,// Whether return value should be negated
      PrecisionContext ctx
     ) {
      Rounding rounding = (ctx == null) ? Rounding.HalfEven : ctx.Rounding;
      int lastDiscarded = 0;
      int olderDiscarded = 0;
      if (!(remainder.IsZero)) {
        if (rounding == Rounding.HalfDown ||
            rounding == Rounding.HalfUp ||
            rounding == Rounding.HalfEven) {
          BigInteger halfDivisor = (divisor >> 1);
          int cmpHalf = remainder.CompareTo(halfDivisor);
          if ((cmpHalf == 0) && divisor.IsEven) {
            // remainder is exactly half
            lastDiscarded = (thisRadix / 2);
            olderDiscarded = 0;
          } else if (cmpHalf > 0) {
            // remainder is greater than half
            lastDiscarded = (thisRadix / 2);
            olderDiscarded = 1;
          } else {
            // remainder is less than half
            lastDiscarded = 0;
            olderDiscarded = 1;
          }
        } else {
          // Rounding mode doesn't care about
          // whether remainder is exactly half
          if (rounding == Rounding.Unnecessary)
            throw new ArithmeticException("Rounding was required");
          lastDiscarded = 1;
          olderDiscarded = 1;
        }
      }
      return new int[]{lastDiscarded,olderDiscarded};
    }

    private BigInteger RoundToScale(
      BigInteger mantissa, // Assumes mantissa is nonnegative
      BigInteger remainder,// Assumes value is nonnegative
      BigInteger divisor,// Assumes value is nonnegative
      FastInteger shift,// Number of digits to shift right
      bool neg,// Whether return value should be negated
      PrecisionContext ctx
     ) {
      IShiftAccumulator accum;
      Rounding rounding = (ctx == null) ? Rounding.HalfEven : ctx.Rounding;
      int lastDiscarded = 0;
      int olderDiscarded = 0;
      if (!(remainder.IsZero)) {
        if (rounding == Rounding.HalfDown ||
            rounding == Rounding.HalfUp ||
            rounding == Rounding.HalfEven) {
          BigInteger halfDivisor = (divisor >> 1);
          int cmpHalf = remainder.CompareTo(halfDivisor);
          if ((cmpHalf == 0) && divisor.IsEven) {
            // remainder is exactly half
            lastDiscarded = (thisRadix / 2);
            olderDiscarded = 0;
          } else if (cmpHalf > 0) {
            // remainder is greater than half
            lastDiscarded = (thisRadix / 2);
            olderDiscarded = 1;
          } else {
            // remainder is less than half
            lastDiscarded = 0;
            olderDiscarded = 1;
          }
        } else {
          // Rounding mode doesn't care about
          // whether remainder is exactly half
          if (rounding == Rounding.Unnecessary)
            throw new ArithmeticException("Rounding was required");
          lastDiscarded = 1;
          olderDiscarded = 1;
        }
      }
      int flags = 0;
      BigInteger newmantissa = mantissa;
      if (shift.IsValueZero) {
        if ((lastDiscarded | olderDiscarded) != 0) {
          flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
          if (rounding == Rounding.Unnecessary)
            throw new ArithmeticException("Rounding was required");
          if (RoundGivenDigits(lastDiscarded, olderDiscarded,
                               rounding, neg, newmantissa)) {
            newmantissa += BigInteger.One;
          }
        }
      } else {
        accum = helper.CreateShiftAccumulatorWithDigits(
          mantissa, lastDiscarded, olderDiscarded);
        accum.ShiftRight(shift);
        newmantissa = accum.ShiftedInt;
        if ((accum.DiscardedDigitCount).Sign != 0 ||
            (accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
          if (!mantissa.IsZero)
            flags |= PrecisionContext.FlagRounded;
          if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
            flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
            if (rounding == Rounding.Unnecessary)
              throw new ArithmeticException("Rounding was required");
          }
          if (RoundGivenBigInt(accum, rounding, neg, newmantissa)) {
            newmantissa += BigInteger.One;
          }
        }
      }
      if (ctx.HasFlags) {
        ctx.Flags |= flags;
      }
      if (neg) {
        newmantissa = -newmantissa;
      }
      return newmantissa;
    }

    private const int IntegerModeFixedScale = 1;
    private const int IntegerModeRegular = 0;

    private T DivideInternal(
      T thisValue,
      T divisor,
      PrecisionContext ctx,
      int integerMode,
      BigInteger desiredExponent
     ) {
      T ret = DivisionHandleSpecial(thisValue, divisor, ctx);
      if ((Object)ret != (Object)default(T)) return ret;
      int signA = helper.GetSign(thisValue);
      int signB = helper.GetSign(divisor);
      if (signB == 0) {
        if (signA == 0) {
          return SignalInvalid(ctx);
        }
        return SignalDivideByZero(
          ctx,
          ((helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative) != 0) ^
          ((helper.GetFlags(divisor) & BigNumberFlags.FlagNegative) != 0));
      }
      int radix = thisRadix;
      if (signA == 0) {
        T retval = default(T);
        if (integerMode == IntegerModeFixedScale) {
          int newflags = ((helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative)) ^
            ((helper.GetFlags(divisor) & BigNumberFlags.FlagNegative));
          retval = helper.CreateNewWithFlags(BigInteger.Zero, desiredExponent, newflags);
        } else {
          BigInteger dividendExp = helper.GetExponent(thisValue);
          BigInteger divisorExp = helper.GetExponent(divisor);
          int newflags = ((helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative)) ^
            ((helper.GetFlags(divisor) & BigNumberFlags.FlagNegative));
          retval = RoundToPrecision(helper.CreateNewWithFlags(
            BigInteger.Zero, (dividendExp - (BigInteger)divisorExp),
            newflags), ctx);
        }
        return retval;
      } else {
        BigInteger mantissaDividend = BigInteger.Abs(helper.GetMantissa(thisValue));
        BigInteger mantissaDivisor = BigInteger.Abs(helper.GetMantissa(divisor));
        FastInteger expDividend = FastInteger.FromBig(helper.GetExponent(thisValue));
        FastInteger expDivisor = FastInteger.FromBig(helper.GetExponent(divisor));
        FastInteger expdiff = FastInteger.Copy(expDividend).Subtract(expDivisor);
        FastInteger adjust = new FastInteger(0);
        FastInteger result = new FastInteger(0);
        FastInteger naturalExponent = FastInteger.Copy(expdiff);
        bool hasPrecision = ctx != null && (ctx.Precision).Sign != 0;
        bool resultNeg = (helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative) !=
          (helper.GetFlags(divisor) & BigNumberFlags.FlagNegative);
        FastInteger fastPrecision = (!hasPrecision) ? new FastInteger(0) :
          FastInteger.FromBig(ctx.Precision);
        FastInteger dividendPrecision=null;
        FastInteger divisorPrecision=null;
        if (integerMode == IntegerModeFixedScale) {
          FastInteger shift;
          BigInteger rem;
          FastInteger fastDesiredExponent = FastInteger.FromBig(desiredExponent);
          if (ctx != null && ctx.HasFlags && fastDesiredExponent.CompareTo(naturalExponent) > 0) {
            // Treat as rounded if the desired exponent is greater
            // than the "ideal" exponent
            ctx.Flags |= PrecisionContext.FlagRounded;
          }
          if (expdiff.CompareTo(fastDesiredExponent) <= 0) {
            shift = FastInteger.Copy(fastDesiredExponent).Subtract(expdiff);
            BigInteger quo = BigInteger.DivRem(mantissaDividend, mantissaDivisor, out rem);
            quo = RoundToScale(quo, rem, mantissaDivisor, shift, resultNeg, ctx);
            return helper.CreateNewWithFlags(quo, desiredExponent, resultNeg ?
                                             BigNumberFlags.FlagNegative : 0);
          } else if (ctx != null && (ctx.Precision).Sign != 0 &&
                     FastInteger.Copy(expdiff).SubtractInt(8).CompareTo(fastPrecision) > 0
                    ) { // NOTE: 8 guard digits
            // Result would require a too-high precision since
            // exponent difference is much higher
            return SignalInvalidWithMessage(ctx, "Result can't fit the precision");
          } else {
            shift = FastInteger.Copy(expdiff).Subtract(fastDesiredExponent);
            mantissaDividend = helper.MultiplyByRadixPower(mantissaDividend, shift);
            BigInteger quo = BigInteger.DivRem(mantissaDividend, mantissaDivisor, out rem);
            quo = RoundToScale(quo, rem, mantissaDivisor, new FastInteger(0), resultNeg, ctx);
            return helper.CreateNewWithFlags(quo, desiredExponent, resultNeg ?
                                             BigNumberFlags.FlagNegative : 0);
          }
        }
        if (integerMode == IntegerModeRegular) {
          BigInteger rem=null;
          BigInteger quo=null;
          //Console.WriteLine("div={0} divs={1}",mantissaDividend.getUnsignedBitLength(),
          //                mantissaDivisor.getUnsignedBitLength());
          if((mantissaDividend%mantissaDivisor).IsZero){
            quo = mantissaDividend/mantissaDivisor;
            quo = RoundToScale(quo, BigInteger.Zero, mantissaDivisor, new FastInteger(0), resultNeg, ctx);
            return RoundToPrecision(helper.CreateNewWithFlags(quo, naturalExponent.AsBigInteger(), resultNeg ?
                                                              BigNumberFlags.FlagNegative : 0),ctx);
          }
          if(hasPrecision){
            #if DEBUG
if((ctx)==null)throw new ArgumentNullException("ctx");
#endif

            BigInteger divid=mantissaDividend;
            FastInteger shift=FastInteger.FromBig(ctx.Precision);
            dividendPrecision =
              helper.CreateShiftAccumulator(mantissaDividend).GetDigitLength();
            divisorPrecision =
              helper.CreateShiftAccumulator(mantissaDivisor).GetDigitLength();
            if(dividendPrecision.CompareTo(divisorPrecision)<=0){
              divisorPrecision.Subtract(dividendPrecision);
              divisorPrecision.Increment();
              shift.Add(divisorPrecision);
              divid=helper.MultiplyByRadixPower(
                divid, shift);
            } else {
              // Already greater than divisor precision
              dividendPrecision.Subtract(divisorPrecision);
              if(dividendPrecision.CompareTo(shift)<=0){
                shift.Subtract(dividendPrecision);
                shift.Increment();
                divid=helper.MultiplyByRadixPower(
                  divid, shift);
              } else {
                // no need to shift
                shift.SetInt(0);
              }
            }
            dividendPrecision =
              helper.CreateShiftAccumulator(divid).GetDigitLength();
            divisorPrecision =
              helper.CreateShiftAccumulator(mantissaDivisor).GetDigitLength();
            if(shift.Sign!=0 || quo==null){
              // if shift isn't zero, recalculate the quotient
              // and remainder
              quo = BigInteger.DivRem(divid, mantissaDivisor, out rem);
            }
            int[] digitStatus=RoundToScaleStatus(rem,mantissaDivisor,resultNeg,ctx);
            FastInteger natexp=FastInteger.Copy(naturalExponent).Subtract(shift);
            PrecisionContext ctxcopy=ctx.WithBlankFlags();
            T retval2=RoundToPrecisionWithShift(
              helper.CreateNewWithFlags(
                quo, natexp.AsBigInteger(),
                (resultNeg ? BigNumberFlags.FlagNegative : 0)),ctxcopy,
              digitStatus[0],digitStatus[1],new FastInteger(0),false);
            if((ctxcopy.Flags&PrecisionContext.FlagInexact)!=0){
              if(ctx!=null && ctx.HasFlags)
                ctx.Flags|=ctxcopy.Flags;
              return retval2;
            } else {
              if(ctx!=null && ctx.HasFlags){
                ctx.Flags|=ctxcopy.Flags;
                ctx.Flags&=~PrecisionContext.FlagRounded;
              }
              return ReduceToPrecisionAndIdealExponent(
                retval2,
                ctx,
                rem.IsZero ? null : fastPrecision,
                expdiff);
            }
          }
        }
        //
        // Rest of method assumes unlimited precision
        // and IntegerModeRegular
        //
        int mantcmp = mantissaDividend.CompareTo(mantissaDivisor);
        if (mantcmp < 0) {
          // dividend mantissa is less than divisor mantissa
          dividendPrecision =
            helper.CreateShiftAccumulator(mantissaDividend).GetDigitLength();
          divisorPrecision =
            helper.CreateShiftAccumulator(mantissaDivisor).GetDigitLength();
          divisorPrecision.Subtract(dividendPrecision);
          if (divisorPrecision.IsValueZero)
            divisorPrecision.Increment();
          // multiply dividend mantissa so precisions are the same
          // (except if they're already the same, in which case multiply
          // by radix)
          mantissaDividend = helper.MultiplyByRadixPower(
            mantissaDividend, divisorPrecision);
          adjust.Add(divisorPrecision);
          if (mantissaDividend.CompareTo(mantissaDivisor) < 0) {
            // dividend mantissa is still less, multiply once more
            if (radix == 2) {
              mantissaDividend <<= 1;
            } else {
              mantissaDividend *= (BigInteger)radix;
            }
            adjust.Increment();
          }
        } else if (mantcmp > 0) {
          // dividend mantissa is greater than divisor mantissa
          dividendPrecision =
            helper.CreateShiftAccumulator(mantissaDividend).GetDigitLength();
          divisorPrecision =
            helper.CreateShiftAccumulator(mantissaDivisor).GetDigitLength();
          dividendPrecision.Subtract(divisorPrecision);
          BigInteger oldMantissaB = mantissaDivisor;
          mantissaDivisor = helper.MultiplyByRadixPower(
            mantissaDivisor, dividendPrecision);
          adjust.Subtract(dividendPrecision);
          if (mantissaDividend.CompareTo(mantissaDivisor) < 0) {
            // dividend mantissa is now less, divide by radix power
            if (dividendPrecision.CompareToInt(1) == 0) {
              // no need to divide here, since that would just undo
              // the multiplication
              mantissaDivisor = oldMantissaB;
            } else {
              BigInteger bigpow = (BigInteger)(radix);
              mantissaDivisor /= bigpow;
            }
            adjust.Increment();
          }
        }
        if (mantcmp == 0) {
          result = new FastInteger(1);
          mantissaDividend = BigInteger.Zero;
        } else {
          {
            if (!helper.HasTerminatingRadixExpansion(
              mantissaDividend, mantissaDivisor)) {
              throw new ArithmeticException("Result would have a nonterminating expansion");
            }
            FastInteger divs = FastInteger.FromBig(mantissaDivisor);
            FastInteger divd = FastInteger.FromBig(mantissaDividend);
            bool divisorFits=divs.CanFitInInt32();
            int smallDivisor=(divisorFits ? divs.AsInt32() : 0);
            int halfRadix=radix/2;
            FastInteger divsHalfRadix = null;
            if (radix != 2) {
              divsHalfRadix = FastInteger.FromBig(mantissaDivisor).Multiply(halfRadix);
            }
            while (true) {
              bool remainderZero = false;
              int count = 0;
              if(divd.CanFitInInt32()){
                if(divisorFits){
                  int smallDividend=divd.AsInt32();
                  count=smallDividend/smallDivisor;
                  divd.SetInt(smallDividend%smallDivisor);
                } else {
                  count=0;
                }
              } else {
                if(divsHalfRadix!=null){
                  count+=halfRadix*divd.RepeatedSubtract(divsHalfRadix);
                }
                count+=divd.RepeatedSubtract(divs);
              }
              result.AddInt(count);
              remainderZero = (divd.IsValueZero);
              if (remainderZero && adjust.Sign >= 0) {
                mantissaDividend = divd.AsBigInteger();
                break;
              }
              adjust.Increment();
              result.Multiply(radix);
              divd.Multiply(radix);
            }
          }
        }
        // mantissaDividend now has the remainder
        FastInteger exp = FastInteger.Copy(expdiff).Subtract(adjust);
        Rounding rounding = (ctx == null) ? Rounding.HalfEven : ctx.Rounding;
        int lastDiscarded = 0;
        int olderDiscarded = 0;
        if (!(mantissaDividend.IsZero)) {
          if (rounding == Rounding.HalfDown ||
              rounding == Rounding.HalfEven ||
              rounding == Rounding.HalfUp
             ) {
            BigInteger halfDivisor = (mantissaDivisor >> 1);
            int cmpHalf = mantissaDividend.CompareTo(halfDivisor);
            if ((cmpHalf == 0) && mantissaDivisor.IsEven) {
              // remainder is exactly half
              lastDiscarded = (radix / 2);
              olderDiscarded = 0;
            } else if (cmpHalf > 0) {
              // remainder is greater than half
              lastDiscarded = (radix / 2);
              olderDiscarded = 1;
            } else {
              // remainder is less than half
              lastDiscarded = 0;
              olderDiscarded = 1;
            }
          } else {
            if (rounding == Rounding.Unnecessary)
              throw new ArithmeticException("Rounding was required");
            lastDiscarded = 1;
            olderDiscarded = 1;
          }
        }
        BigInteger bigResult = result.AsBigInteger();
        if (ctx != null && ctx.HasFlags && exp.CompareTo(expdiff) > 0) {
          // Treat as rounded if the true exponent is greater
          // than the "ideal" exponent
          ctx.Flags |= PrecisionContext.FlagRounded;
        }
        BigInteger bigexp = exp.AsBigInteger();
        T retval = helper.CreateNewWithFlags(
          bigResult, bigexp, resultNeg ? BigNumberFlags.FlagNegative : 0);
        return RoundToPrecisionWithShift(
          retval,
          ctx,
          lastDiscarded, olderDiscarded, new FastInteger(0), false);
      }
    }

    /// <summary> Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Min.
    /// </summary>
    /// <returns>A T object.</returns>
    /// <param name='a'>A T object.</param>
    /// <param name='b'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public T MinMagnitude(T a, T b, PrecisionContext ctx) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      // Handle infinity and NaN
      T result = MinMaxHandleSpecial(a, b, ctx, true, true);
      if ((Object)result != (Object)default(T)) return result;
      int cmp = CompareTo(AbsRaw(a), AbsRaw(b));
      if (cmp == 0) return Min(a, b, ctx);
      return (cmp < 0) ? RoundToPrecision(a, ctx) :
        RoundToPrecision(b, ctx);
    }
    /// <summary> Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Max.
    /// </summary>
    /// <returns>A T object.</returns>
    /// <param name='a'>A T object.</param>
    /// <param name='b'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public T MaxMagnitude(T a, T b, PrecisionContext ctx) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      // Handle infinity and NaN
      T result = MinMaxHandleSpecial(a, b, ctx, false, true);
      if ((Object)result != (Object)default(T)) return result;
      int cmp = CompareTo(AbsRaw(a), AbsRaw(b));
      if (cmp == 0) return Max(a, b, ctx);
      return (cmp > 0) ? RoundToPrecision(a, ctx) :
        RoundToPrecision(b, ctx);
    }
    /// <summary> Gets the greater value between two T values. </summary>
    /// <returns>The larger value of the two objects.</returns>
    /// <param name='a'>A T object.</param>
    /// <param name='b'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public T Max(T a, T b, PrecisionContext ctx) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      // Handle infinity and NaN
      T result = MinMaxHandleSpecial(a, b, ctx, false, false);
      if ((Object)result != (Object)default(T)) return result;
      int cmp = CompareTo(a, b);
      if (cmp != 0)
        return cmp < 0 ? RoundToPrecision(b, ctx) :
          RoundToPrecision(a, ctx);
      int flagNegA = (helper.GetFlags(a) & BigNumberFlags.FlagNegative);
      if (flagNegA != (helper.GetFlags(b) & BigNumberFlags.FlagNegative)) {
        return (flagNegA != 0) ? RoundToPrecision(b, ctx) :
          RoundToPrecision(a, ctx);
      }
      if (flagNegA == 0) {
        return helper.GetExponent(a).CompareTo(helper.GetExponent(b)) > 0 ? RoundToPrecision(a, ctx) :
          RoundToPrecision(b, ctx);
      } else {
        return helper.GetExponent(a).CompareTo(helper.GetExponent(b)) > 0 ? RoundToPrecision(b, ctx) :
          RoundToPrecision(a, ctx);
      }
    }

    /// <summary> Gets the lesser value between two T values.</summary>
    /// <returns>The smaller value of the two objects.</returns>
    /// <param name='a'>A T object.</param>
    /// <param name='b'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public T Min(T a, T b, PrecisionContext ctx) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      // Handle infinity and NaN
      T result = MinMaxHandleSpecial(a, b, ctx, true, false);
      if ((Object)result != (Object)default(T)) return result;
      int cmp = CompareTo(a, b);
      if (cmp != 0)
        return cmp > 0 ? RoundToPrecision(b, ctx) :
          RoundToPrecision(a, ctx);
      int signANeg = helper.GetFlags(a) & BigNumberFlags.FlagNegative;
      if (signANeg != (helper.GetFlags(b) & BigNumberFlags.FlagNegative)) {
        return (signANeg != 0) ? RoundToPrecision(a, ctx) :
          RoundToPrecision(b, ctx);
      }
      if (signANeg == 0) {
        return (helper.GetExponent(a)).CompareTo(helper.GetExponent(b)) > 0 ? RoundToPrecision(b, ctx) :
          RoundToPrecision(a, ctx);
      } else {
        return (helper.GetExponent(a)).CompareTo(helper.GetExponent(b)) > 0 ? RoundToPrecision(a, ctx) :
          RoundToPrecision(b, ctx);
      }
    }

    /// <summary>Multiplies two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The product of the two objects.</returns>
    /// <param name='other'>A T object.</param>
    public T Multiply(T thisValue, T other, PrecisionContext ctx) {
      int thisFlags = helper.GetFlags(thisValue);
      int otherFlags = helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = HandleNotANumber(thisValue, other, ctx);
        if ((Object)result != (Object)default(T)) return result;
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          // Attempt to multiply infinity by 0
          if ((otherFlags & BigNumberFlags.FlagSpecial) == 0 && helper.GetMantissa(other).IsZero)
            return SignalInvalid(ctx);
          return EnsureSign(thisValue, ((thisFlags & BigNumberFlags.FlagNegative) ^ (otherFlags & BigNumberFlags.FlagNegative)) != 0);
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          // Attempt to multiply infinity by 0
          if ((thisFlags & BigNumberFlags.FlagSpecial) == 0 && helper.GetMantissa(thisValue).IsZero)
            return SignalInvalid(ctx);
          return EnsureSign(other, ((thisFlags & BigNumberFlags.FlagNegative) ^ (otherFlags & BigNumberFlags.FlagNegative)) != 0);
        }
      }
      BigInteger bigintOp2 = helper.GetExponent(other);
      BigInteger newexp = (helper.GetExponent(thisValue) + (BigInteger)bigintOp2);
      thisFlags = (thisFlags & BigNumberFlags.FlagNegative) ^ (otherFlags & BigNumberFlags.FlagNegative);
      T ret = helper.CreateNewWithFlags(
        helper.GetMantissa(thisValue) *
        (BigInteger)(helper.GetMantissa(other)), newexp,
        thisFlags
       );
      if (ctx != null) {
        ret = RoundToPrecision(ret, ctx);
      }
      return ret;
    }
    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='multiplicand'>A T object.</param>
    /// <param name='augend'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T MultiplyAndAdd(T thisValue, T multiplicand,
                            T augend,
                            PrecisionContext ctx) {
      PrecisionContext ctx2 = PrecisionContext.Unlimited.WithBlankFlags();
      T ret=MultiplyAddHandleSpecial(thisValue,multiplicand,augend,ctx);
      if((Object)ret!=(Object)default(T))return ret;
      ret = Add(Multiply(thisValue, multiplicand, ctx2), augend, ctx);
      if (ctx != null && ctx.HasFlags) ctx.Flags |= ctx2.Flags;
      return ret;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='context'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RoundToBinaryPrecision(
      T thisValue,
      PrecisionContext context
     ) {
      return RoundToBinaryPrecisionWithShift(thisValue, context, 0, 0, null, false);
    }
    private T RoundToBinaryPrecisionWithShift(
      T thisValue,
      PrecisionContext context,
      int lastDiscarded,
      int olderDiscarded,
      FastInteger shift,
      bool adjustNegativeZero
     ) {
      return RoundToPrecisionInternal(
        thisValue,
        lastDiscarded,
        olderDiscarded,
        shift, true, adjustNegativeZero,
        context);
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='context'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Plus(
      T thisValue,
      PrecisionContext context
     ) {
      return RoundToPrecisionInternal(thisValue, 0,0,null, false,true,context);
    }
    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='context'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RoundToPrecision(
      T thisValue,
      PrecisionContext context
     ) {
      return RoundToPrecisionInternal(thisValue, 0,0,null, false,false,context);
    }
    private T RoundToPrecisionWithShift(
      T thisValue,
      PrecisionContext context,
      int lastDiscarded,
      int olderDiscarded,
      FastInteger shift,
      bool adjustNegativeZero
     ) {
      return RoundToPrecisionInternal(
        thisValue,
        lastDiscarded,
        olderDiscarded,
        shift, false, adjustNegativeZero,
        context);
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='otherValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Quantize(
      T thisValue,
      T otherValue,
      PrecisionContext ctx
     ) {
      int thisFlags = helper.GetFlags(thisValue);
      int otherFlags = helper.GetFlags(otherValue);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = HandleNotANumber(thisValue, otherValue, ctx);
        if ((Object)result != (Object)default(T)) return result;
        if (((thisFlags & otherFlags) & BigNumberFlags.FlagInfinity) != 0) {
          return RoundToPrecision(thisValue, ctx);
        }
        if (((thisFlags | otherFlags) & BigNumberFlags.FlagInfinity) != 0) {
          return SignalInvalid(ctx);
        }
      }
      BigInteger expOther = helper.GetExponent(otherValue);
      if (ctx != null && !ctx.ExponentWithinRange(expOther))
        return SignalInvalidWithMessage(ctx, "Exponent not within exponent range: " + expOther.ToString());
      PrecisionContext tmpctx = (ctx == null ?
                                 PrecisionContext.ForRounding(Rounding.HalfEven) :
                                 ctx.Copy()).WithBlankFlags();
      BigInteger mantThis = BigInteger.Abs(helper.GetMantissa(thisValue));
      BigInteger expThis = helper.GetExponent(thisValue);
      int expcmp = expThis.CompareTo(expOther);
      int negativeFlag = (helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative);
      T ret = default(T);
      if (expcmp == 0) {
        ret = RoundToPrecision(thisValue, tmpctx);
      } else if (mantThis.IsZero) {
        ret = helper.CreateNewWithFlags(BigInteger.Zero, expOther, negativeFlag);
        ret = RoundToPrecision(ret, tmpctx);
      } else if (expcmp > 0) {
        // Other exponent is less
        FastInteger radixPower = FastInteger.FromBig(expThis).SubtractBig(expOther);
        if ((tmpctx.Precision).Sign > 0 &&
            radixPower.CompareTo(FastInteger.FromBig(tmpctx.Precision).AddInt(10)) > 0) {
          // Radix power is much too high for the current precision
          return SignalInvalidWithMessage(ctx, "Result too high for current precision");
        }
        mantThis = helper.MultiplyByRadixPower(mantThis, radixPower);
        ret = helper.CreateNewWithFlags(mantThis, expOther, negativeFlag);
        ret = RoundToPrecision(ret, tmpctx);
      } else {
        // Other exponent is greater
        FastInteger shift = FastInteger.FromBig(expOther).SubtractBig(expThis);
        ret = RoundToPrecisionWithShift(thisValue, tmpctx, 0, 0, shift, false);
      }
      if ((tmpctx.Flags & PrecisionContext.FlagOverflow) != 0) {
        return SignalInvalid(ctx);
      }
      if (ret == null || !helper.GetExponent(ret).Equals(expOther)) {
        return SignalInvalid(ctx);
      }
      ret = EnsureSign(ret, negativeFlag != 0);
      if (ctx != null && ctx.HasFlags) {
        int flags = tmpctx.Flags;
        flags &= ~PrecisionContext.FlagUnderflow;
        ctx.Flags |= flags;
      }
      return ret;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='expOther'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RoundToExponentExact(
      T thisValue,
      BigInteger expOther,
      PrecisionContext ctx) {
      if (helper.GetExponent(thisValue).CompareTo(expOther) >= 0) {
        return RoundToPrecision(thisValue, ctx);
      } else {
        PrecisionContext pctx = (ctx == null) ? null :
          ctx.WithPrecision(0).WithBlankFlags();
        T ret = Quantize(thisValue, helper.CreateNewWithFlags(
          BigInteger.One, expOther, 0),
                         pctx);
        if (ctx != null && ctx.HasFlags) {
          ctx.Flags |= pctx.Flags;
        }
        return ret;
      }
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='expOther'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T RoundToExponentSimple(
      T thisValue,
      BigInteger expOther,
      PrecisionContext ctx) {
      int thisFlags = helper.GetFlags(thisValue);
      if ((thisFlags & BigNumberFlags.FlagSpecial) != 0) {
        T result = HandleNotANumber(thisValue, thisValue, ctx);
        if ((Object)result != (Object)default(T)) return result;
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          return thisValue;
        }
      }
      if (helper.GetExponent(thisValue).CompareTo(expOther) >= 0) {
        return RoundToPrecision(thisValue, ctx);
      } else {
        if (ctx != null && !ctx.ExponentWithinRange(expOther))
          return SignalInvalidWithMessage(ctx, "Exponent not within exponent range: " + expOther.ToString());
        BigInteger bigmantissa = BigInteger.Abs(helper.GetMantissa(thisValue));
        FastInteger shift = FastInteger.FromBig(expOther).SubtractBig(helper.GetExponent(thisValue));
        IShiftAccumulator accum = helper.CreateShiftAccumulator(bigmantissa);
        accum.ShiftRight(shift);
        bigmantissa = accum.ShiftedInt;
        return RoundToPrecisionWithShift(
          helper.CreateNewWithFlags(bigmantissa, expOther, thisFlags), ctx,
          accum.LastDiscardedDigit,
          accum.OlderDiscardedDigits, new FastInteger(0), false);
      }
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    /// <param name='exponent'>A BigInteger object.</param>
    public T RoundToExponentNoRoundedFlag(
      T thisValue,
      BigInteger exponent,
      PrecisionContext ctx
     ) {
      PrecisionContext pctx = (ctx == null) ? null :
        ctx.WithBlankFlags();
      T ret = RoundToExponentExact(thisValue, exponent, pctx);
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= (pctx.Flags & ~(PrecisionContext.FlagInexact | PrecisionContext.FlagRounded));
      }
      return ret;
    }
    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <param name='precision'>A FastInteger object.</param>
    /// <param name='idealExp'>A FastInteger object.</param>
    /// <returns>A T object.</returns>
    public T ReduceToPrecisionAndIdealExponent(
      T thisValue,
      PrecisionContext ctx,
      FastInteger precision,
      FastInteger idealExp
     ) {
      T ret = RoundToPrecision(thisValue, ctx);
      if (ret != null && (helper.GetFlags(ret) & BigNumberFlags.FlagSpecial) == 0) {
        BigInteger bigmant = BigInteger.Abs(helper.GetMantissa(ret));
        FastInteger exp = FastInteger.FromBig(helper.GetExponent(ret));
        if (bigmant.IsZero) {
          exp = new FastInteger(0);
        } else {
          int radix = thisRadix;
          FastInteger digits=(precision==null) ? null :
            helper.CreateShiftAccumulator(bigmant).GetDigitLength();
          BigInteger bigradix = (BigInteger)radix;
          while (!(bigmant.IsZero)) {
            if(precision!=null && digits.CompareTo(precision)==0){
              break;
            }
            if(idealExp!=null && exp.CompareTo(idealExp)==0){
              break;
            }
            BigInteger bigrem;
            BigInteger bigquo = BigInteger.DivRem(bigmant, bigradix, out bigrem);
            if (!bigrem.IsZero)
              break;
            bigmant = bigquo;
            exp.Increment();
            if(digits!=null)digits.Decrement();
          }
        }
        int flags = helper.GetFlags(thisValue);
        ret = helper.CreateNewWithFlags(bigmant, exp.AsBigInteger(), flags);
        if (ctx != null && ctx.ClampNormalExponents) {
          PrecisionContext ctxtmp = ctx.WithBlankFlags();
          ret = RoundToPrecision(ret, ctxtmp);
          if (ctx.HasFlags) {
            ctx.Flags |= (ctxtmp.Flags & ~PrecisionContext.FlagClamped);
          }
        }
        ret = EnsureSign(ret, (flags & BigNumberFlags.FlagNegative) != 0);
      }
      return ret;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T Reduce(
      T thisValue,
      PrecisionContext ctx
     ) {
      return ReduceToPrecisionAndIdealExponent(thisValue,ctx,null,null);
    }

    private T RoundToPrecisionInternal(
      T thisValue,
      int lastDiscarded,
      int olderDiscarded,
      FastInteger shift,
      bool binaryPrec, // whether "precision" is the number of bits, not digits
      bool adjustNegativeZero,
      PrecisionContext ctx
     ) {
      if ((ctx) == null)
        ctx=PrecisionContext.Unlimited.WithRounding(Rounding.HalfEven);
      // If context has unlimited precision and exponent range,
      // and no discarded digits or shifting
      if ((ctx.Precision).IsZero && !ctx.HasExponentRange &&
          (lastDiscarded | olderDiscarded) == 0 && (shift==null || shift.IsValueZero))
        return thisValue;
      int thisFlags = helper.GetFlags(thisValue);
      if ((thisFlags & BigNumberFlags.FlagSpecial) != 0) {
        if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
          if (ctx.HasFlags) {
            ctx.Flags|=PrecisionContext.FlagInvalid;
          }
          return ReturnQuietNaN(thisValue,ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
          return ReturnQuietNaN(thisValue,ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          return thisValue;
        }
      }
      // get the precision
      FastInteger fastPrecision=((ctx.Precision).canFitInInt()) ?
        new FastInteger((ctx.Precision).intValue()) :
        FastInteger.FromBig(ctx.Precision);
      if (fastPrecision.Sign < 0) throw new ArgumentException("precision" + " not greater or equal to " + "0" + " (" + fastPrecision + ")");
      if (thisRadix == 2 || fastPrecision.IsValueZero) {
        // "binaryPrec" will have no special effect here
        binaryPrec = false;
      }
      IShiftAccumulator accum=null;
      FastInteger fastEMin=null;
      FastInteger fastEMax=null;
      // get the exponent range
      if(ctx!=null && ctx.HasExponentRange){
        fastEMax=((ctx.EMax).canFitInInt()) ? new FastInteger((ctx.EMax).intValue()) :
          FastInteger.FromBig(ctx.EMax);
        fastEMin=((ctx.EMin).canFitInInt()) ? new FastInteger((ctx.EMin).intValue()) :
          FastInteger.FromBig(ctx.EMin);
      }
      Rounding rounding=(ctx==null) ? Rounding.HalfEven : ctx.Rounding;
      bool unlimitedPrec = (fastPrecision.IsValueZero);
      if(!binaryPrec){
        // Fast path to check if rounding is necessary at all
        if (fastPrecision.Sign > 0 &&
            (shift == null || shift.IsValueZero) &&
            (thisFlags & BigNumberFlags.FlagSpecial) == 0) {
          BigInteger mantabs = BigInteger.Abs(helper.GetMantissa(thisValue));
          if (adjustNegativeZero &&
              (thisFlags & BigNumberFlags.FlagNegative) != 0 && mantabs.IsZero &&
              (ctx.Rounding != Rounding.Floor)) {
            // Change negative zero to positive zero
            // except if the rounding mode is Floor
            thisValue = EnsureSign(thisValue, false);
            thisFlags = 0;
          }
          accum = helper.CreateShiftAccumulatorWithDigits(
            mantabs, lastDiscarded, olderDiscarded);
          FastInteger digitCount=accum.GetDigitLength();
          if (digitCount.CompareTo(fastPrecision) <= 0) {
            if (!RoundGivenDigits(lastDiscarded, olderDiscarded, ctx.Rounding,
                                  (thisFlags & BigNumberFlags.FlagNegative) != 0, mantabs)) {
              if (ctx.HasFlags && (lastDiscarded | olderDiscarded) != 0) {
                ctx.Flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
              }
              if (!ctx.HasExponentRange)
                return thisValue;
              BigInteger bigexp=helper.GetExponent(thisValue);
              FastInteger fastExp=(bigexp.canFitInInt()) ?
                new FastInteger(bigexp.intValue()) :
                FastInteger.FromBig(bigexp);
              FastInteger fastAdjustedExp = FastInteger.Copy(fastExp)
                .Add(fastPrecision).Decrement();
              FastInteger fastNormalMin = FastInteger.Copy(fastEMin)
                .Add(fastPrecision).Decrement();
              if (fastAdjustedExp.CompareTo(fastEMax) <= 0 &&
                  fastAdjustedExp.CompareTo(fastNormalMin) >= 0) {
                return thisValue;
              }
            } else {
              if (ctx.HasFlags && (lastDiscarded | olderDiscarded) != 0) {
                ctx.Flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
              }
              bool stillWithinPrecision=false;
              mantabs += BigInteger.One;
              if(digitCount.CompareTo(fastPrecision)<0){
                stillWithinPrecision=true;
              } else {
                BigInteger radixPower = helper.MultiplyByRadixPower(BigInteger.One, fastPrecision);
                stillWithinPrecision=(mantabs.CompareTo(radixPower) < 0);
              }
              if (stillWithinPrecision) {
                if (!ctx.HasExponentRange)
                  return helper.CreateNewWithFlags(mantabs, helper.GetExponent(thisValue), thisFlags);
                BigInteger bigexp=helper.GetExponent(thisValue);
                FastInteger fastExp=(bigexp.canFitInInt()) ?
                  new FastInteger(bigexp.intValue()) :
                  FastInteger.FromBig(bigexp);
                FastInteger fastAdjustedExp = FastInteger.Copy(fastExp)
                  .Add(fastPrecision).Decrement();
                FastInteger fastNormalMin = FastInteger.Copy(fastEMin)
                  .Add(fastPrecision).Decrement();
                if (fastAdjustedExp.CompareTo(fastEMax) <= 0 &&
                    fastAdjustedExp.CompareTo(fastNormalMin) >= 0) {
                  return helper.CreateNewWithFlags(mantabs, bigexp, thisFlags);
                }
              }
            }
          }
        }
      }
      if (adjustNegativeZero &&
          (thisFlags & BigNumberFlags.FlagNegative) != 0 && helper.GetMantissa(thisValue).IsZero &&
          (rounding != Rounding.Floor)) {
        // Change negative zero to positive zero
        // except if the rounding mode is Floor
        thisValue = EnsureSign(thisValue, false);
        thisFlags = 0;
      }
      bool neg = (thisFlags & BigNumberFlags.FlagNegative) != 0;
      BigInteger bigmantissa = BigInteger.Abs(helper.GetMantissa(thisValue));
      // save mantissa in case result is subnormal
      // and must be rounded again
      BigInteger oldmantissa = bigmantissa;
      bool mantissaWasZero = (oldmantissa.IsZero && (lastDiscarded | olderDiscarded) == 0);
      BigInteger maxMantissa = BigInteger.One;
      FastInteger exp = FastInteger.FromBig(helper.GetExponent(thisValue));
      int flags = 0;
      if(accum==null)
        accum = helper.CreateShiftAccumulatorWithDigits(
          bigmantissa, lastDiscarded, olderDiscarded);
      if (binaryPrec) {
        FastInteger prec = FastInteger.Copy(fastPrecision);
        while (prec.Sign > 0) {
          int bitShift = (prec.CompareToInt(1000000) >= 0) ? 1000000 : prec.AsInt32();
          maxMantissa <<= bitShift;
          prec.SubtractInt(bitShift);
        }
        maxMantissa -= BigInteger.One;
        IShiftAccumulator accumMaxMant = helper.CreateShiftAccumulator(
          maxMantissa);
        // Get the digit length of the maximum possible mantissa
        // for the given binary precision, and use that for
        // fastPrecision
        fastPrecision = accumMaxMant.GetDigitLength();
      }
      if (shift != null && shift.Sign!=0) {
        accum.ShiftRight(shift);
      }
      if (!unlimitedPrec) {
        accum.ShiftToDigits(fastPrecision);
      } else {
        fastPrecision = accum.GetDigitLength();
      }
      if (binaryPrec) {
        while ((accum.ShiftedInt).CompareTo(maxMantissa) > 0) {
          accum.ShiftRightInt(1);
        }
      }
      FastInteger discardedBits = FastInteger.Copy(accum.DiscardedDigitCount);
      exp.Add(discardedBits);
      FastInteger adjExponent = FastInteger.Copy(exp)
        .Add(accum.GetDigitLength()).Decrement();
      //Console.WriteLine("{0}->{1} digits={2} exp={3} [curexp={4}] adj={5},max={6}",bigmantissa,accum.ShiftedInt,
      //              accum.DiscardedDigitCount,exp,helper.GetExponent(thisValue),adjExponent,fastEMax);
      FastInteger newAdjExponent = adjExponent;
      FastInteger clamp = null;
      BigInteger earlyRounded = BigInteger.Zero;
      if (binaryPrec && fastEMax != null && adjExponent.CompareTo(fastEMax) == 0) {
        // May or may not be an overflow depending on the mantissa
        FastInteger expdiff = FastInteger.Copy(fastPrecision).Subtract(accum.GetDigitLength());
        BigInteger currMantissa = accum.ShiftedInt;
        currMantissa = helper.MultiplyByRadixPower(currMantissa, expdiff);
        if ((currMantissa).CompareTo(maxMantissa) > 0) {
          // Mantissa too high, treat as overflow
          adjExponent.Increment();
        }
      }
      //Console.WriteLine("{0} adj={1} emin={2}",thisValue,adjExponent,fastEMin);
      if (ctx.HasFlags && fastEMin != null && !unlimitedPrec &&
          adjExponent.CompareTo(fastEMin) < 0) {
        earlyRounded = accum.ShiftedInt;
        if (RoundGivenBigInt(accum, rounding, neg, earlyRounded)) {
          earlyRounded += BigInteger.One;
          if ((earlyRounded.IsEven || (thisRadix & 1) != 0)) {
            IShiftAccumulator accum2 = helper.CreateShiftAccumulator(earlyRounded);
            FastInteger newDigitLength=accum2.GetDigitLength();
            // Ensure newDigitLength doesn't exceed precision
            if(binaryPrec || newDigitLength.CompareTo(fastPrecision)>0){
              newDigitLength=FastInteger.Copy(fastPrecision);
            }
            newAdjExponent = FastInteger.Copy(exp)
              .Add(newDigitLength)
              .Decrement();
          }
        }
      }
      if (fastEMax != null && adjExponent.CompareTo(fastEMax) > 0) {
        if (mantissaWasZero) {
          if (ctx.HasFlags) {
            ctx.Flags |= flags | PrecisionContext.FlagClamped;
          }
          if (ctx.ClampNormalExponents) {
            // Clamp exponents to eMax + 1 - precision
            // if directed
            if (binaryPrec && thisRadix != 2) {
              fastPrecision = helper.CreateShiftAccumulator(maxMantissa)
                .GetDigitLength();
            }
            FastInteger clampExp = FastInteger.Copy(fastEMax).Increment().Subtract(fastPrecision);
            if (fastEMax.CompareTo(clampExp) > 0) {
              if (ctx.HasFlags)
                ctx.Flags |= PrecisionContext.FlagClamped;
              fastEMax=clampExp;
            }
          }
          return helper.CreateNewWithFlags(oldmantissa, fastEMax.AsBigInteger(), thisFlags);
        } else {
          // Overflow
          flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact |
            PrecisionContext.FlagRounded;
          if (rounding == Rounding.Unnecessary)
            throw new ArithmeticException("Rounding was required");
          if (!unlimitedPrec &&
              (rounding == Rounding.Down ||
               rounding == Rounding.ZeroFiveUp ||
               (rounding == Rounding.Ceiling && neg) ||
               (rounding == Rounding.Floor && !neg))) {
            // Set to the highest possible value for
            // the given precision
            BigInteger overflowMant = BigInteger.Zero;
            if (binaryPrec) {
              overflowMant = maxMantissa;
            } else {
              overflowMant = helper.MultiplyByRadixPower(BigInteger.One, fastPrecision);
              overflowMant -= BigInteger.One;
            }
            if (ctx.HasFlags) ctx.Flags |= flags;
            clamp = FastInteger.Copy(fastEMax).Increment()
              .Subtract(fastPrecision);
            return helper.CreateNewWithFlags(overflowMant,
                                             clamp.AsBigInteger(),
                                             neg ? BigNumberFlags.FlagNegative : 0
                                            );
          }
          if (ctx.HasFlags) ctx.Flags |= flags;
          return SignalOverflow(neg);
        }
      } else if (fastEMin != null && adjExponent.CompareTo(fastEMin) < 0) {
        // Subnormal
        FastInteger fastETiny = FastInteger.Copy(fastEMin)
          .Subtract(fastPrecision)
          .Increment();
        if (ctx.HasFlags) {
          if (!earlyRounded.IsZero) {
            if (newAdjExponent.CompareTo(fastEMin) < 0) {
              flags |= PrecisionContext.FlagSubnormal;
            }
          }
        }
        //Console.WriteLine("exp={0} eTiny={1}",exp,fastETiny);
        FastInteger subExp = FastInteger.Copy(exp);
        //Console.WriteLine("exp={0} eTiny={1}",subExp,fastETiny);
        if (subExp.CompareTo(fastETiny) < 0) {
          //Console.WriteLine("Less than ETiny");
          FastInteger expdiff = FastInteger.Copy(fastETiny).Subtract(exp);
          expdiff.Add(discardedBits);
          accum = helper.CreateShiftAccumulatorWithDigits(
            oldmantissa, lastDiscarded, olderDiscarded);
          accum.ShiftRight(expdiff);
          FastInteger newmantissa = accum.ShiftedIntFast;
          if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
            if (rounding == Rounding.Unnecessary)
              throw new ArithmeticException("Rounding was required");
          }
          if ((accum.DiscardedDigitCount).Sign != 0 ||
              (accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
            if (ctx.HasFlags) {
              if (!mantissaWasZero)
                flags |= PrecisionContext.FlagRounded;
              if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
                flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
              }
            }
            if (Round(accum, rounding, neg, newmantissa)) {
              newmantissa.Increment();
            }
          }
          if (ctx.HasFlags) {
            if (newmantissa.IsValueZero)
              flags |= PrecisionContext.FlagClamped;
            if ((flags & (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact)) ==
                (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact))
              flags |= PrecisionContext.FlagUnderflow | PrecisionContext.FlagRounded;
            ctx.Flags |= flags;
          }
          bigmantissa=newmantissa.AsBigInteger();
          if (ctx.ClampNormalExponents) {
            // Clamp exponents to eMax + 1 - precision
            // if directed
            if (binaryPrec && thisRadix != 2) {
              fastPrecision = helper.CreateShiftAccumulator(maxMantissa)
                .GetDigitLength();
            }
            FastInteger clampExp = FastInteger.Copy(fastEMax).Increment().Subtract(fastPrecision);
            if (fastETiny.CompareTo(clampExp) > 0) {
              if (!(bigmantissa.IsZero)) {
                expdiff = FastInteger.Copy(fastETiny).Subtract(clampExp);
                bigmantissa = helper.MultiplyByRadixPower(bigmantissa, expdiff);
              }
              if (ctx.HasFlags)
                ctx.Flags |= PrecisionContext.FlagClamped;
              fastETiny=clampExp;
            }
          }
          return helper.CreateNewWithFlags(newmantissa.AsBigInteger(), fastETiny.AsBigInteger(),
                                           neg ? BigNumberFlags.FlagNegative : 0);
        }
      }
      bool recheckOverflow = false;
      if ((accum.DiscardedDigitCount).Sign != 0 ||
          (accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
        if (!bigmantissa.IsZero)
          flags |= PrecisionContext.FlagRounded;
        bigmantissa = accum.ShiftedInt;
        if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
          flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
          if (rounding == Rounding.Unnecessary)
            throw new ArithmeticException("Rounding was required");
        }
        if (RoundGivenBigInt(accum, rounding, neg, bigmantissa)) {
          FastInteger oldDigitLength=accum.GetDigitLength();
          bigmantissa += BigInteger.One;
          if (binaryPrec) recheckOverflow = true;
          // Check if mantissa's precision is now greater
          // than the one set by the context
          if (!unlimitedPrec &&
              (bigmantissa.IsEven || (thisRadix & 1) != 0) &&
              (binaryPrec || oldDigitLength.CompareTo(fastPrecision)>=0)
             ) {
            accum = helper.CreateShiftAccumulator(bigmantissa);
            FastInteger newDigitLength=accum.GetDigitLength();
            if(binaryPrec || newDigitLength.CompareTo(fastPrecision)>0){
              FastInteger neededShift=FastInteger.Copy(newDigitLength).Subtract(fastPrecision);
              accum.ShiftRight(neededShift);
              if (binaryPrec) {
                while ((accum.ShiftedInt).CompareTo(maxMantissa) > 0) {
                  accum.ShiftRightInt(1);
                }
              }
              if ((accum.DiscardedDigitCount).Sign != 0) {
                exp.Add(accum.DiscardedDigitCount);
                discardedBits.Add(accum.DiscardedDigitCount);
                bigmantissa = accum.ShiftedInt;
                if (!binaryPrec) recheckOverflow = true;
              }
            }
          }
        }
      }
      if (recheckOverflow && fastEMax != null) {
        // Check for overflow again
        adjExponent = FastInteger.Copy(exp);
        adjExponent.Add(accum.GetDigitLength()).Decrement();
        if (binaryPrec && fastEMax != null && adjExponent.CompareTo(fastEMax) == 0) {
          // May or may not be an overflow depending on the mantissa
          // (uses accumulator from previous steps, including the check
          // if the mantissa now exceeded the precision)
          FastInteger expdiff = FastInteger.Copy(fastPrecision).Subtract(accum.GetDigitLength());
          BigInteger currMantissa = accum.ShiftedInt;
          currMantissa = helper.MultiplyByRadixPower(currMantissa, expdiff);
          if ((currMantissa).CompareTo(maxMantissa) > 0) {
            // Mantissa too high, treat as overflow
            adjExponent.Increment();
          }
        }
        if (adjExponent.CompareTo(fastEMax) > 0) {
          flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
          if (!unlimitedPrec &&
              (rounding == Rounding.Down ||
               rounding == Rounding.ZeroFiveUp ||
               (rounding == Rounding.Ceiling && neg) ||
               (rounding == Rounding.Floor && !neg))) {
            // Set to the highest possible value for
            // the given precision
            BigInteger overflowMant = BigInteger.Zero;
            if (binaryPrec) {
              overflowMant = maxMantissa;
            } else {
              overflowMant = helper.MultiplyByRadixPower(BigInteger.One, fastPrecision);
              overflowMant -= BigInteger.One;
            }
            if (ctx.HasFlags) ctx.Flags |= flags;
            clamp = FastInteger.Copy(fastEMax).Increment()
              .Subtract(fastPrecision);
            return helper.CreateNewWithFlags(overflowMant,
                                             clamp.AsBigInteger(),
                                             neg ? BigNumberFlags.FlagNegative : 0);
          }
          if (ctx.HasFlags) ctx.Flags |= flags;
          return SignalOverflow(neg);
        }
      }
      if (ctx.HasFlags) ctx.Flags |= flags;
      if (ctx.ClampNormalExponents) {
        // Clamp exponents to eMax + 1 - precision
        // if directed
        if (binaryPrec && thisRadix != 2) {
          fastPrecision = helper.CreateShiftAccumulator(maxMantissa)
            .GetDigitLength();
        }
        FastInteger clampExp = FastInteger.Copy(fastEMax).Increment().Subtract(fastPrecision);
        if (exp.CompareTo(clampExp) > 0) {
          if (!(bigmantissa.IsZero)) {
            FastInteger expdiff = FastInteger.Copy(exp).Subtract(clampExp);
            bigmantissa = helper.MultiplyByRadixPower(bigmantissa, expdiff);
          }
          if (ctx.HasFlags)
            ctx.Flags |= PrecisionContext.FlagClamped;
          exp=clampExp;
        }
      }
      return helper.CreateNewWithFlags(bigmantissa, exp.AsBigInteger(),
                                       neg ? BigNumberFlags.FlagNegative : 0);
    }

    private T AddCore(BigInteger mant1, // assumes mant1 is nonnegative
                      BigInteger mant2, // assumes mant2 is nonnegative
                      BigInteger exponent, int flags1, int flags2, PrecisionContext ctx) {
      #if DEBUG
      if (mant1.Sign < 0) throw new InvalidOperationException();
      if (mant2.Sign < 0) throw new InvalidOperationException();
      #endif
      bool neg1 = (flags1 & BigNumberFlags.FlagNegative) != 0;
      bool neg2 = (flags2 & BigNumberFlags.FlagNegative) != 0;
      bool negResult = false;
      if (neg1 != neg2) {
        // Signs are different, treat as a subtraction
        mant1 -= (BigInteger)mant2;
        int mant1Sign = mant1.Sign;
        negResult = neg1 ^ (mant1Sign == 0 ? neg2 : (mant1Sign < 0));
      } else {
        // Signs are same, treat as an addition
        mant1 += (BigInteger)mant2;
        negResult = neg1;
      }
      if (mant1.IsZero && negResult) {
        // Result is negative zero
        if (!((neg1 && neg2) || ((neg1 ^ neg2) && ctx != null && ctx.Rounding == Rounding.Floor))) {
          negResult = false;
        }
      }
      return helper.CreateNewWithFlags(mant1, exponent, negResult ? BigNumberFlags.FlagNegative : 0);
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    /// <param name='other'>A T object.</param>
    public T Add(T thisValue, T other, PrecisionContext ctx) {
      int thisFlags = helper.GetFlags(thisValue);
      int otherFlags = helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = HandleNotANumber(thisValue, other, ctx);
        if ((Object)result != (Object)default(T)) return result;
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
            if ((thisFlags & BigNumberFlags.FlagNegative) != (otherFlags & BigNumberFlags.FlagNegative))
              return SignalInvalid(ctx);
          }
          return thisValue;
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          return other;
        }
      }
      int expcmp = helper.GetExponent(thisValue).CompareTo((BigInteger)helper.GetExponent(other));
      T retval = default(T);
      BigInteger op1MantAbs = BigInteger.Abs(helper.GetMantissa(thisValue));
      BigInteger op2MantAbs = BigInteger.Abs(helper.GetMantissa(other));
      if (expcmp == 0) {
        retval = AddCore(op1MantAbs, op2MantAbs, helper.GetExponent(thisValue), thisFlags, otherFlags, ctx);
      } else {
        // choose the minimum exponent
        T op1 = thisValue;
        T op2 = other;
        BigInteger op1Exponent = helper.GetExponent(op1);
        BigInteger op2Exponent = helper.GetExponent(op2);
        BigInteger resultExponent = (expcmp < 0 ? op1Exponent : op2Exponent);
        FastInteger fastOp1Exp = FastInteger.FromBig(op1Exponent);
        FastInteger fastOp2Exp = FastInteger.FromBig(op2Exponent);
        FastInteger expdiff = FastInteger.Copy(fastOp1Exp).Subtract(fastOp2Exp).Abs();
        if (ctx != null && (ctx.Precision).Sign > 0) {
          // Check if exponent difference is too big for
          // radix-power calculation to work quickly
          FastInteger fastPrecision = FastInteger.FromBig(ctx.Precision);
          // If exponent difference is greater than the precision
          if (FastInteger.Copy(expdiff).CompareTo(fastPrecision) > 0) {
            int expcmp2 = fastOp1Exp.CompareTo(fastOp2Exp);
            if (expcmp2 < 0) {
              if (!(op2MantAbs.IsZero)) {
                // first operand's exponent is less
                // and second operand isn't zero
                // second mantissa will be shifted by the exponent
                // difference
                //                    111111111111|
                //        222222222222222|
                FastInteger digitLength1 = helper.CreateShiftAccumulator(op1MantAbs)
                  .GetDigitLength();
                if (
                  FastInteger.Copy(fastOp1Exp)
                  .Add(digitLength1)
                  .AddInt(2)
                  .CompareTo(fastOp2Exp) < 0) {
                  // first operand's mantissa can't reach the
                  // second operand's mantissa, so the exponent can be
                  // raised without affecting the result
                  FastInteger tmp = FastInteger.Copy(fastOp2Exp).SubtractInt(4)
                    .Subtract(digitLength1)
                    .SubtractBig(ctx.Precision);
                  FastInteger newDiff = FastInteger.Copy(tmp).Subtract(fastOp2Exp).Abs();
                  if (newDiff.CompareTo(expdiff) < 0) {
                    // Can be treated as almost zero
                    bool sameSign=(helper.GetSign(thisValue) == helper.GetSign(other));
                    bool oneOpIsZero=(op1MantAbs.IsZero);
                    FastInteger digitLength2 = helper.CreateShiftAccumulator(
                      op2MantAbs).GetDigitLength();
                    if (digitLength2.CompareTo(fastPrecision) < 0) {
                      // Second operand's precision too short
                      FastInteger precisionDiff = FastInteger.Copy(fastPrecision).Subtract(digitLength2);
                      if(!oneOpIsZero && !sameSign){
                        precisionDiff.AddInt(2);
                      }
                      op2MantAbs = helper.MultiplyByRadixPower(
                        op2MantAbs, precisionDiff);
                      BigInteger bigintTemp = precisionDiff.AsBigInteger();
                      op2Exponent -= (BigInteger)bigintTemp;
                      if(!oneOpIsZero && !sameSign){
                        op2MantAbs-=BigInteger.One;
                      }
                      other = helper.CreateNewWithFlags(op2MantAbs, op2Exponent, helper.GetFlags(other));
                      FastInteger shift = FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                      if(oneOpIsZero && ctx!=null && ctx.HasFlags){
                        ctx.Flags|=PrecisionContext.FlagRounded;
                      }
                      return RoundToPrecisionWithShift(
                        other, ctx,
                        (oneOpIsZero || sameSign) ? 0 : 1,
                        (oneOpIsZero && !sameSign) ? 0 : 1,
                        shift, false);
                    } else {
                      if(!oneOpIsZero && !sameSign){
                        op2MantAbs=helper.MultiplyByRadixPower(op2MantAbs,new FastInteger(2));
                        op2Exponent -= (BigInteger)2;
                        op2MantAbs-=BigInteger.One;
                        other = helper.CreateNewWithFlags(op2MantAbs, op2Exponent, helper.GetFlags(other));
                        FastInteger shift = FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                        return RoundToPrecisionWithShift(other, ctx, 0, 0, shift, false);
                      } else {
                        FastInteger shift2 = FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                        if(!sameSign && ctx!=null && ctx.HasFlags){
                          ctx.Flags|=PrecisionContext.FlagRounded;
                        }
                        return RoundToPrecisionWithShift(
                          other, ctx,
                          0,
                          sameSign ? 1 : 0, shift2, false);
                      }
                    }
                  }
                }
              }
            } else if (expcmp2 > 0) {
              if (!(op1MantAbs.IsZero)) {
                // first operand's exponent is greater
                // and first operand isn't zero
                // first mantissa will be shifted by the exponent
                // difference
                //       111111111111|
                //                222222222222222|
                FastInteger digitLength2 = helper.CreateShiftAccumulator(op2MantAbs).GetDigitLength();
                if (
                  FastInteger.Copy(fastOp2Exp)
                  .Add(digitLength2)
                  .AddInt(2)
                  .CompareTo(fastOp1Exp) < 0) {
                  // second operand's mantissa can't reach the
                  // first operand's mantissa, so the exponent can be
                  // raised without affecting the result
                  FastInteger tmp = FastInteger.Copy(fastOp1Exp).SubtractInt(4)
                    .Subtract(digitLength2)
                    .SubtractBig(ctx.Precision);
                  FastInteger newDiff = FastInteger.Copy(tmp).Subtract(fastOp1Exp).Abs();
                  if (newDiff.CompareTo(expdiff) < 0) {
                    // Can be treated as almost zero
                    bool sameSign=(helper.GetSign(thisValue) == helper.GetSign(other));
                    bool oneOpIsZero=(op2MantAbs.IsZero);
                    digitLength2 = helper.CreateShiftAccumulator(
                      op1MantAbs).GetDigitLength();
                    if (digitLength2.CompareTo(fastPrecision) < 0) {
                      // Second operand's precision too short
                      FastInteger precisionDiff = FastInteger.Copy(fastPrecision).Subtract(digitLength2);
                      if(!oneOpIsZero && !sameSign){
                        precisionDiff.AddInt(2);
                      }
                      op1MantAbs = helper.MultiplyByRadixPower(
                        op1MantAbs, precisionDiff);
                      BigInteger bigintTemp = precisionDiff.AsBigInteger();
                      op1Exponent -= (BigInteger)bigintTemp;
                      if(!oneOpIsZero && !sameSign){
                        op1MantAbs-=BigInteger.One;
                      }
                      thisValue = helper.CreateNewWithFlags(op1MantAbs, op1Exponent, helper.GetFlags(thisValue));
                      FastInteger shift = FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                      if(oneOpIsZero && ctx!=null && ctx.HasFlags){
                        ctx.Flags|=PrecisionContext.FlagRounded;
                      }
                      return RoundToPrecisionWithShift(
                        thisValue, ctx,
                        (oneOpIsZero || sameSign) ? 0 : 1,
                        (oneOpIsZero && !sameSign) ? 0 : 1,
                        shift, false);
                    } else {
                      if(!oneOpIsZero && !sameSign){
                        op1MantAbs=helper.MultiplyByRadixPower(op1MantAbs,new FastInteger(2));
                        op1Exponent -= (BigInteger)2;
                        op1MantAbs-=BigInteger.One;
                        thisValue = helper.CreateNewWithFlags(op1MantAbs, op1Exponent, helper.GetFlags(thisValue));
                        FastInteger shift = FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                        return RoundToPrecisionWithShift(thisValue, ctx, 0, 0, shift, false);
                      } else {
                        FastInteger shift2 = FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                        if(!sameSign && ctx!=null && ctx.HasFlags){
                          ctx.Flags|=PrecisionContext.FlagRounded;
                        }
                        return RoundToPrecisionWithShift(
                          thisValue, ctx,
                          0,
                          sameSign ? 1 : 0, shift2, false);
                      }
                    }

                  }
                }
              }
            }
            expcmp = op1Exponent.CompareTo((BigInteger)op2Exponent);
            resultExponent = (expcmp < 0 ? op1Exponent : op2Exponent);
          }
        }
        if (expcmp > 0) {
          op1MantAbs = helper.RescaleByExponentDiff(
            op1MantAbs, op1Exponent, op2Exponent);
          //Console.WriteLine("{0} {1} -> {2}",op1MantAbs,op2MantAbs,op2MantAbs-op1MantAbs);
          retval = AddCore(
            op1MantAbs, op2MantAbs, resultExponent,
            thisFlags, otherFlags, ctx);
        } else {
          op2MantAbs = helper.RescaleByExponentDiff(
            op2MantAbs, op1Exponent, op2Exponent);
          //Console.WriteLine("{0} {1} -> {2}",op1MantAbs,op2MantAbs,op2MantAbs-op1MantAbs);
          retval = AddCore(
            op1MantAbs, op2MantAbs, resultExponent,
            thisFlags, otherFlags, ctx);
        }
      }
      if (ctx != null) {
        retval = RoundToPrecision(retval, ctx);
      }
      return retval;
    }

    /// <summary>Compares a T object with this instance.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='decfrac'>A T object.</param>
    /// <param name='treatQuietNansAsSignaling'>A Boolean object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A T object.</returns>
    public T CompareToWithContext(T thisValue, T decfrac, bool treatQuietNansAsSignaling, PrecisionContext ctx) {
      if (decfrac == null) return SignalInvalid(ctx);
      T result = CompareToHandleSpecial(thisValue, decfrac, treatQuietNansAsSignaling, ctx);
      if ((Object)result != (Object)default(T)) return result;
      return ValueOf(CompareTo(thisValue, decfrac), null);
    }

    /// <summary>Compares a T object with this instance.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='decfrac'>A T object.</param>
    /// <returns>Zero if the values are equal; a negative number if this instance
    /// is less, or a positive number if this instance is greater.</returns>
    public int CompareTo(T thisValue, T decfrac) {
      if (decfrac == null) return 1;
      int flagsThis = helper.GetFlags(thisValue);
      int flagsOther = helper.GetFlags(decfrac);
      if ((flagsThis & BigNumberFlags.FlagNaN) != 0) {
        if ((flagsOther & BigNumberFlags.FlagNaN) != 0) {
          return 0;
        }
        return 1; // Treat NaN as greater
      }
      if ((flagsOther & BigNumberFlags.FlagNaN) != 0) {
        return -1; // Treat as less than NaN
      }
      int s = CompareToHandleSpecialReturnInt(thisValue, decfrac);
      if (s <= 1) return s;
      s = helper.GetSign(thisValue);
      int ds = helper.GetSign(decfrac);
      if (s != ds) return (s < ds) ? -1 : 1;
      if (ds == 0 || s == 0) {
        // Special case: Either operand is zero
        return 0;
      }
      int expcmp = helper.GetExponent(thisValue).CompareTo((BigInteger)helper.GetExponent(decfrac));
      // At this point, the signs are equal so we can compare
      // their absolute values instead
      int mantcmp = BigInteger.Abs(helper.GetMantissa(thisValue))
        .CompareTo(BigInteger.Abs(helper.GetMantissa(decfrac)));
      if (s < 0) mantcmp = -mantcmp;
      if (mantcmp == 0) {
        // Special case: Mantissas are equal
        return s < 0 ? -expcmp : expcmp;
      }
      if (expcmp == 0) {
        return mantcmp;
      }
      BigInteger op1Exponent = helper.GetExponent(thisValue);
      BigInteger op2Exponent = helper.GetExponent(decfrac);
      FastInteger fastOp1Exp = FastInteger.FromBig(op1Exponent);
      FastInteger fastOp2Exp = FastInteger.FromBig(op2Exponent);
      FastInteger expdiff = FastInteger.Copy(fastOp1Exp).Subtract(fastOp2Exp).Abs();
      // Check if exponent difference is too big for
      // radix-power calculation to work quickly
      if (expdiff.CompareToInt(100) >= 0) {
        BigInteger op1MantAbs = BigInteger.Abs(helper.GetMantissa(thisValue));
        BigInteger op2MantAbs = BigInteger.Abs(helper.GetMantissa(decfrac));
        FastInteger precision1 = helper.CreateShiftAccumulator(
          op1MantAbs).GetDigitLength();
        FastInteger precision2 = helper.CreateShiftAccumulator(
          op2MantAbs).GetDigitLength();
        FastInteger maxPrecision = null;
        if (precision1.CompareTo(precision2) > 0)
          maxPrecision = precision1;
        else
          maxPrecision = precision2;
        // If exponent difference is greater than the
        // maximum precision of the two operands
        if (FastInteger.Copy(expdiff).CompareTo(maxPrecision) > 0) {
          int expcmp2 = fastOp1Exp.CompareTo(fastOp2Exp);
          if (expcmp2 < 0) {
            if (!(op2MantAbs.IsZero)) {
              // first operand's exponent is less
              // and second operand isn't zero
              // second mantissa will be shifted by the exponent
              // difference
              //                    111111111111|
              //        222222222222222|
              FastInteger digitLength1 = helper.CreateShiftAccumulator(
                op1MantAbs).GetDigitLength();
              if (
                FastInteger.Copy(fastOp1Exp)
                .Add(digitLength1)
                .AddInt(2)
                .CompareTo(fastOp2Exp) < 0) {
                // first operand's mantissa can't reach the
                // second operand's mantissa, so the exponent can be
                // raised without affecting the result
                FastInteger tmp = FastInteger.Copy(fastOp2Exp).SubtractInt(8)
                  .Subtract(digitLength1)
                  .Subtract(maxPrecision);
                FastInteger newDiff = FastInteger.Copy(tmp).Subtract(fastOp2Exp).Abs();
                if (newDiff.CompareTo(expdiff) < 0) {
                  if (s == ds) {
                    return (s < 0) ? 1 : -1;
                  } else {
                    op1Exponent = (tmp.AsBigInteger());
                  }
                }
              }
            }
          } else if (expcmp2 > 0) {
            if (!(op1MantAbs.IsZero)) {
              // first operand's exponent is greater
              // and second operand isn't zero
              // first mantissa will be shifted by the exponent
              // difference
              //       111111111111|
              //                222222222222222|
              FastInteger digitLength2 = helper.CreateShiftAccumulator(
                op2MantAbs).GetDigitLength();
              if (
                FastInteger.Copy(fastOp2Exp)
                .Add(digitLength2)
                .AddInt(2)
                .CompareTo(fastOp1Exp) < 0) {
                // second operand's mantissa can't reach the
                // first operand's mantissa, so the exponent can be
                // raised without affecting the result
                FastInteger tmp = FastInteger.Copy(fastOp1Exp).SubtractInt(8)
                  .Subtract(digitLength2)
                  .Subtract(maxPrecision);
                FastInteger newDiff = FastInteger.Copy(tmp).Subtract(fastOp1Exp).Abs();
                if (newDiff.CompareTo(expdiff) < 0) {
                  if (s == ds) {
                    return (s < 0) ? -1 : 1;
                  } else {
                    op2Exponent = (tmp.AsBigInteger());
                  }
                }
              }
            }
          }
          expcmp = op1Exponent.CompareTo((BigInteger)op2Exponent);
        }
      }
      if (expcmp > 0) {
        BigInteger newmant = helper.RescaleByExponentDiff(
          helper.GetMantissa(thisValue), op1Exponent, op2Exponent);
        BigInteger othermant = BigInteger.Abs(helper.GetMantissa(decfrac));
        newmant = BigInteger.Abs(newmant);
        mantcmp = newmant.CompareTo(othermant);
        return (s < 0) ? -mantcmp : mantcmp;
      } else {
        BigInteger newmant = helper.RescaleByExponentDiff(
          helper.GetMantissa(decfrac), op1Exponent, op2Exponent);
        BigInteger othermant = BigInteger.Abs(helper.GetMantissa(thisValue));
        newmant = BigInteger.Abs(newmant);
        mantcmp = othermant.CompareTo(newmant);
        return (s < 0) ? -mantcmp : mantcmp;
      }
    }
  }
}
