package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Encapsulates radix-independent arithmetic.
     * @param <T> Data type for a numeric value in a particular radix.
     */
  class RadixMath<T> implements IRadixMath<T> {
    private static final int IntegerModeFixedScale = 1;
    private static final int IntegerModeRegular = 0;
    private IRadixMathHelper<T> helper;
    private int thisRadix;
    private int support;

    public RadixMath (IRadixMathHelper<T> helper) {
      this.helper = helper;
      this.support = helper.GetArithmeticSupport();
      this.thisRadix = helper.GetRadix();
    }

    private T ReturnQuietNaN(T thisValue, PrecisionContext ctx) {
      BigInteger mant = (this.helper.GetMantissa(thisValue)).abs();
      boolean mantChanged = false;
      if (mant.signum() != 0 && ctx != null && ctx.getHasMaxPrecision()) {
        FastInteger compPrecision = FastInteger.FromBig(ctx.getPrecision());
        if (this.helper.CreateShiftAccumulator(mant).GetDigitLength()
            .compareTo(compPrecision) >= 0) {
          // Mant's precision is higher than the maximum precision
          BigInteger limit = this.TryMultiplyByRadixPower(
            BigInteger.ONE,
            compPrecision);
          if (limit == null) {
            // Limit can't be allocated
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          if (mant.compareTo(limit) >= 0) {
            mant = mant.remainder(limit);
            mantChanged = true;
          }
        }
      }
      int flags = this.helper.GetFlags(thisValue);
      if (!mantChanged && (flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return thisValue;
      }
      flags &= BigNumberFlags.FlagNegative;
      flags |= BigNumberFlags.FlagQuietNaN;
      return this.helper.CreateNewWithFlags(mant, BigInteger.ZERO, flags);
    }

    private T SquareRootHandleSpecial(T thisValue, PrecisionContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      if ((thisFlags & BigNumberFlags.FlagSpecial) != 0) {
        if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
          return this.SignalingNaNInvalid(thisValue, ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
          return this.ReturnQuietNaN(thisValue, ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          // Square root of infinity
          return ((thisFlags & BigNumberFlags.FlagNegative) != 0) ?
            this.SignalInvalid(ctx) : thisValue;
        }
      }
      int sign = this.helper.GetSign(thisValue);
      return (sign < 0) ? this.SignalInvalid(ctx) : null;
    }

    private T DivisionHandleSpecial(
      T thisValue,
      T other,
      PrecisionContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = this.HandleNotANumber(thisValue, other, ctx);
        if ((Object)result != (Object)null) {
          return result;
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0 && (otherFlags &
  BigNumberFlags.FlagInfinity) !=

            0) {
          // Attempt to divide infinity by infinity
          return this.SignalInvalid(ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          return this.EnsureSign(
            thisValue,
            ((thisFlags ^ otherFlags) & BigNumberFlags.FlagNegative) != 0);
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          // Divisor is infinity, so result will be epsilon
          if (ctx != null && ctx.getHasExponentRange() && ctx.getPrecision().signum() > 0) {
            if (ctx.getHasFlags()) {
              ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagClamped));
            }
            BigInteger bigexp = ctx.getEMin();
            BigInteger bigprec = ctx.getPrecision();
            if (ctx.getAdjustExponent()) {
              bigexp = bigexp.subtract(bigprec);
              bigexp = bigexp.add(BigInteger.ONE);
            }
            thisFlags = (thisFlags ^ otherFlags) & BigNumberFlags.FlagNegative;
            return this.helper.CreateNewWithFlags(
              BigInteger.ZERO,
              bigexp,
              thisFlags);
          }
          thisFlags = (thisFlags ^ otherFlags) & BigNumberFlags.FlagNegative;
          return
            this.RoundToPrecision(
   this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, thisFlags),
   ctx);
        }
      }
      return null;
    }

    private T RemainderHandleSpecial(
      T thisValue,
      T other,
      PrecisionContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = this.HandleNotANumber(thisValue, other, ctx);
        if ((Object)result != (Object)null) {
          return result;
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          return this.SignalInvalid(ctx);
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          return this.RoundToPrecision(thisValue, ctx);
        }
      }
      return this.helper.GetMantissa(other).signum() == 0 ? this.SignalInvalid(ctx) :
        null;
    }

    private T MinMaxHandleSpecial(
      T thisValue,
      T otherValue,
      PrecisionContext ctx,
      boolean isMinOp,
      boolean compareAbs) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(otherValue);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        // Check this value then the other value for signaling NaN
        if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
          return this.SignalingNaNInvalid(thisValue, ctx);
        }
        if ((otherFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
          return this.SignalingNaNInvalid(otherValue, ctx);
        }
        // Check this value then the other value for quiet NaN
        if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
          if ((otherFlags & BigNumberFlags.FlagQuietNaN) != 0) {
            // both values are quiet NaN
            return this.ReturnQuietNaN(thisValue, ctx);
          }
          // return "other" for being numeric
          return this.RoundToPrecision(otherValue, ctx);
        }
        if ((otherFlags & BigNumberFlags.FlagQuietNaN) != 0) {
          // At this point, "thisValue" can't be NaN,
          // return "thisValue" for being numeric
          return this.RoundToPrecision(thisValue, ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          if (compareAbs && (otherFlags & BigNumberFlags.FlagInfinity) == 0) {
            // treat this as larger
            return isMinOp ? this.RoundToPrecision(otherValue, ctx) : thisValue;
          }
          // This value is infinity
          if (isMinOp) {
            // if negative, will be less than every other number
            return ((thisFlags & BigNumberFlags.FlagNegative) != 0) ?
              thisValue : this.RoundToPrecision(otherValue, ctx);
            // if positive, will be greater
          }
          // if positive, will be greater than every other number
          return ((thisFlags & BigNumberFlags.FlagNegative) == 0) ?
            thisValue : this.RoundToPrecision(otherValue, ctx);
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          if (compareAbs) {
            // treat this as larger (the first value
            // won't be infinity at this point
            return isMinOp ? this.RoundToPrecision(thisValue, ctx) : otherValue;
          }
          return isMinOp ? (((otherFlags & BigNumberFlags.FlagNegative) ==
                             0) ? this.RoundToPrecision(thisValue, ctx) :
                            otherValue) :
            (((otherFlags & BigNumberFlags.FlagNegative) != 0) ?
             this.RoundToPrecision(thisValue, ctx) : otherValue);
        }
      }
      return null;
    }

    private T HandleNotANumber(T thisValue, T other, PrecisionContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(other);
      // Check this value then the other value for signaling NaN
      if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(thisValue, ctx);
      }
      if ((otherFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(other, ctx);
      }
      // Check this value then the other value for quiet NaN
      return ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) ?
        this.ReturnQuietNaN(thisValue, ctx) : (((otherFlags &
                                           BigNumberFlags.FlagQuietNaN) !=
                                                   0) ?
                                               this.ReturnQuietNaN(
                                                 other,
                                                 ctx) : null);
    }

    private T MultiplyAddHandleSpecial(
      T op1,
      T op2,
      T op3,
      PrecisionContext ctx) {
      int op1Flags = this.helper.GetFlags(op1);
      // Check operands in order for signaling NaN
      if ((op1Flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(op1, ctx);
      }
      int op2Flags = this.helper.GetFlags(op2);
      if ((op2Flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(op2, ctx);
      }
      int op3Flags = this.helper.GetFlags(op3);
      if ((op3Flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(op3, ctx);
      }
      // Check operands in order for quiet NaN
      if ((op1Flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.ReturnQuietNaN(op1, ctx);
      }
      if ((op2Flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.ReturnQuietNaN(op2, ctx);
      }
      // Check multiplying infinity by 0 (important to check
      // now before checking third operand for quiet NaN because
      // this signals invalid operation and the operation starts
      // with multiplying only the first two operands)
      if ((op1Flags & BigNumberFlags.FlagInfinity) != 0) {
        // Attempt to multiply infinity by 0
        if ((op2Flags & BigNumberFlags.FlagSpecial) == 0 &&
            this.helper.GetMantissa(op2).signum() == 0) {
          return this.SignalInvalid(ctx);
        }
      }
      if ((op2Flags & BigNumberFlags.FlagInfinity) != 0) {
        // Attempt to multiply infinity by 0
        if ((op1Flags & BigNumberFlags.FlagSpecial) == 0 &&
            this.helper.GetMantissa(op1).signum() == 0) {
          return this.SignalInvalid(ctx);
        }
      }
      // Now check third operand for quiet NaN
      return ((op3Flags & BigNumberFlags.FlagQuietNaN) != 0) ?
        this.ReturnQuietNaN(op3, ctx) : null;
    }

    private T ValueOf(int value, PrecisionContext ctx) {
      return (ctx == null || !ctx.getHasExponentRange() ||
              ctx.ExponentWithinRange(BigInteger.ZERO)) ?
        this.helper.ValueOf(value) :
        this.RoundToPrecision(this.helper.ValueOf(value), ctx);
    }

    private int CompareToHandleSpecialReturnInt(T thisValue, T other) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        // Assumes that neither operand is NaN

        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          // thisValue is infinity
          return ((thisFlags & (BigNumberFlags.FlagInfinity |
                                BigNumberFlags.FlagNegative)) == (otherFlags &
  (BigNumberFlags.FlagInfinity |
  BigNumberFlags.FlagNegative))) ?
            0 :
            (((thisFlags & BigNumberFlags.FlagNegative) == 0) ? 1 : -1);
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          // the other value is infinity
          return ((thisFlags & (BigNumberFlags.FlagInfinity |
                                BigNumberFlags.FlagNegative)) == (otherFlags &
  (BigNumberFlags.FlagInfinity |
  BigNumberFlags.FlagNegative))) ?
            0 :
            (((otherFlags & BigNumberFlags.FlagNegative) == 0) ? -1 : 1);
        }
      }
      return 2;
    }

    private T CompareToHandleSpecial(
      T thisValue,
      T other,
      boolean treatQuietNansAsSignaling,
      PrecisionContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        // Check this value then the other value for signaling NaN
        if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
          return this.SignalingNaNInvalid(thisValue, ctx);
        }
        if ((otherFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
          return this.SignalingNaNInvalid(other, ctx);
        }
        if (treatQuietNansAsSignaling) {
          if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.SignalingNaNInvalid(thisValue, ctx);
          }
          if ((otherFlags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.SignalingNaNInvalid(other, ctx);
          }
        } else {
          // Check this value then the other value for quiet NaN
          if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.ReturnQuietNaN(thisValue, ctx);
          }
          if ((otherFlags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.ReturnQuietNaN(other, ctx);
          }
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          // thisValue is infinity
          return ((thisFlags & (BigNumberFlags.FlagInfinity |
                                BigNumberFlags.FlagNegative)) == (otherFlags &
  (BigNumberFlags.FlagInfinity |
  BigNumberFlags.FlagNegative))) ?

            this.ValueOf(0, null) : (((thisFlags &
                             BigNumberFlags.FlagNegative) == 0) ?
                                         this.ValueOf(
                                       1,
                                       null) :
                                     this.ValueOf(-1, null));
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          // the other value is infinity
          return ((thisFlags & (BigNumberFlags.FlagInfinity |
                                BigNumberFlags.FlagNegative)) == (otherFlags &
  (BigNumberFlags.FlagInfinity |
  BigNumberFlags.FlagNegative))) ?

            this.ValueOf(0, null) : (((otherFlags &
                                       BigNumberFlags.FlagNegative) == 0) ?
                                     this.ValueOf(-1, null) :
                                     this.ValueOf(1, null));
        }
      }
      return null;
    }

    private T SignalingNaNInvalid(T value, PrecisionContext ctx) {
      if (ctx != null && ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInvalid));
      }
      return this.ReturnQuietNaN(value, ctx);
    }

    private T SignalInvalid(PrecisionContext ctx) {
      if (ctx != null && ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInvalid));
      }
      if (this.support == BigNumberFlags.FiniteOnly) {
        throw new ArithmeticException("Invalid operation");
      }
      return this.helper.CreateNewWithFlags(
        BigInteger.ZERO,
        BigInteger.ZERO,
        BigNumberFlags.FlagQuietNaN);
    }

    private T SignalInvalidWithMessage(PrecisionContext ctx, String str) {
      if (ctx != null && ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInvalid));
      }
      if (this.support == BigNumberFlags.FiniteOnly) {
        throw new ArithmeticException(str);
      }
      return this.helper.CreateNewWithFlags(
        BigInteger.ZERO,
        BigInteger.ZERO,
        BigNumberFlags.FlagQuietNaN);
    }

    private T SignalOverflow(boolean neg) {
      return this.support == BigNumberFlags.FiniteOnly ? null :
        this.helper.CreateNewWithFlags(
          BigInteger.ZERO,
          BigInteger.ZERO,
          (neg ? BigNumberFlags.FlagNegative : 0) | BigNumberFlags.FlagInfinity);
    }

    private T SignalOverflow2(PrecisionContext ctx, boolean neg) {
      if (ctx != null) {
        Rounding roundingOnOverflow = ctx.getRounding();
        if (ctx.getHasFlags()) {
          ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagOverflow |
            PrecisionContext.FlagInexact | PrecisionContext.FlagRounded));
        }
        if (ctx.getHasMaxPrecision() && ctx.getHasExponentRange() &&
            (roundingOnOverflow == Rounding.Down || roundingOnOverflow ==
             Rounding.ZeroFiveUp ||
             (roundingOnOverflow == Rounding.Ceiling && neg) ||
             (roundingOnOverflow == Rounding.Floor && !neg))) {
          // Set to the highest possible value for
          // the given precision
          BigInteger overflowMant = BigInteger.ZERO;
          FastInteger fastPrecision = FastInteger.FromBig(ctx.getPrecision());
          overflowMant = this.TryMultiplyByRadixPower(
            BigInteger.ONE,
            fastPrecision);
          if (overflowMant == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          overflowMant = overflowMant.subtract(BigInteger.ONE);
          FastInteger clamp = FastInteger.FromBig(ctx.getEMax());
          if (ctx.getAdjustExponent()) {
            clamp.Increment().Subtract(fastPrecision);
          }
          return this.helper.CreateNewWithFlags(
            overflowMant,
            clamp.AsBigInteger(),
            neg ? BigNumberFlags.FlagNegative : 0);
        }
      }
      return this.SignalOverflow(neg);
    }

    private T SignalDivideByZero(PrecisionContext ctx, boolean neg) {
      if (ctx != null && ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagDivideByZero));
      }
      if (this.support == BigNumberFlags.FiniteOnly) {
        throw new ArithmeticException("Division by zero");
      }
      return this.helper.CreateNewWithFlags(
        BigInteger.ZERO,
        BigInteger.ZERO,
        BigNumberFlags.FlagInfinity | (neg ? BigNumberFlags.FlagNegative : 0));
    }

    private boolean Round(
      IShiftAccumulator accum,
      Rounding rounding,
      boolean neg,
      FastInteger fastint) {
      boolean incremented = false;
      if (rounding == Rounding.HalfEven) {
        int radix = this.thisRadix;
        if (accum.getLastDiscardedDigit() >= (radix / 2)) {
          if (accum.getLastDiscardedDigit() > (radix / 2) ||
              accum.getOlderDiscardedDigits() != 0) {
            incremented = true;
          } else {
            incremented |= !fastint.isEvenNumber();
          }
        }
      } else if (rounding == Rounding.ZeroFiveUp) {
        int radix = this.thisRadix;
        if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
          if (radix == 2) {
            incremented = true;
          } else {
            int lastDigit =
              FastInteger.Copy(fastint).Remainder(radix).AsInt32();
            if (lastDigit == 0 || lastDigit == (radix / 2)) {
              incremented = true;
            }
          }
        }
      } else if (rounding != Rounding.Down) {
        incremented = this.RoundGivenDigits(
          accum.getLastDiscardedDigit(),
          accum.getOlderDiscardedDigits(),
          rounding,
          neg,
          BigInteger.ZERO);
      }
      return incremented;
    }

    private boolean RoundGivenDigits(
      int lastDiscarded,
      int olderDiscarded,
      Rounding rounding,
      boolean neg,
      BigInteger bigval) {
      boolean incremented = false;
      int radix = this.thisRadix;
      if (rounding == Rounding.HalfUp) {
        incremented |= lastDiscarded >= (radix / 2);
      } else if (rounding == Rounding.HalfEven) {
        // System.out.println("rgd last= " + lastDiscarded + " older=" +
        // olderDiscarded + " even= " + (bigval.testBit(0)==false));
        // System.out.println("--- --- " +
        // (BitMantissa(helper.CreateNewWithFlags(bigval, BigInteger.ZERO,
        // 0))));
        if (lastDiscarded >= (radix / 2)) {
          if (lastDiscarded > (radix / 2) || olderDiscarded != 0) {
            incremented = true;
          } else {
            incremented |= bigval.testBit(0);
          }
        }
      } else if (rounding == Rounding.Ceiling) {
        incremented |= !neg && (lastDiscarded | olderDiscarded) != 0;
      } else if (rounding == Rounding.Floor) {
        incremented |= neg && (lastDiscarded | olderDiscarded) != 0;
      } else if (rounding == Rounding.HalfDown) {
        incremented |= lastDiscarded > (radix / 2) || (lastDiscarded ==
                                               (radix / 2) && olderDiscarded !=

                                                       0);
      } else if (rounding == Rounding.Up) {
        incremented |= (lastDiscarded | olderDiscarded) != 0;
      } else if (rounding == Rounding.ZeroFiveUp) {
        if ((lastDiscarded | olderDiscarded) != 0) {
          if (radix == 2) {
            incremented = true;
          } else {
            BigInteger bigdigit = bigval.remainder(BigInteger.valueOf(radix));
            int lastDigit = bigdigit.intValue();
            if (lastDigit == 0 || lastDigit == (radix / 2)) {
              incremented = true;
            }
          }
        }
      }
      return incremented;
    }

    private boolean RoundGivenBigInt(
      IShiftAccumulator accum,
      Rounding rounding,
      boolean neg,
      BigInteger bigval) {
      return this.RoundGivenDigits(
        accum.getLastDiscardedDigit(),
        accum.getOlderDiscardedDigits(),
        rounding,
        neg,
        bigval);
    }

    private BigInteger RescaleByExponentDiff(
      BigInteger mantissa,
      BigInteger e1,
      BigInteger e2) {
      if (mantissa.signum() == 0) {
        return BigInteger.ZERO;
      }
      FastInteger diff = FastInteger.FromBig(e1).SubtractBig(e2).Abs();
      return this.TryMultiplyByRadixPower(mantissa, diff);
    }

    private T EnsureSign(T val, boolean negative) {
      if (val == null) {
        return val;
      }
      int flags = this.helper.GetFlags(val);
      if ((negative && (flags & BigNumberFlags.FlagNegative) == 0) ||
          (!negative && (flags & BigNumberFlags.FlagNegative) != 0)) {
        flags &= ~BigNumberFlags.FlagNegative;
        flags |= negative ? BigNumberFlags.FlagNegative : 0;
        return this.helper.CreateNewWithFlags(
          this.helper.GetMantissa(val),
          this.helper.GetExponent(val),
          flags);
      }
      return val;
    }

    public T DivideToIntegerNaturalScale(
      T thisValue,
      T divisor,
      PrecisionContext ctx) {
      FastInteger desiredScale =
        FastInteger.FromBig(this.helper.GetExponent(thisValue))
        .SubtractBig(this.helper.GetExponent(divisor));
      PrecisionContext ctx2 =
        PrecisionContext.ForRounding(Rounding.Down).WithBigPrecision(ctx ==
                                                       null ?
  BigInteger.ZERO :
ctx.getPrecision()).WithBlankFlags();
      T ret = this.DivideInternal(
        thisValue,
        divisor,
        ctx2,
        IntegerModeFixedScale,
        BigInteger.ZERO);
      if ((ctx2.getFlags() & (PrecisionContext.FlagInvalid |
                         PrecisionContext.FlagDivideByZero)) != 0) {
        if (ctx != null && ctx.getHasFlags()) {
          ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInvalid |
            PrecisionContext.FlagDivideByZero));
        }
        return ret;
      }
      boolean neg = (this.helper.GetSign(thisValue) < 0) ^
        (this.helper.GetSign(divisor) < 0);
      // Now the exponent's sign can only be 0 or positive
      if (this.helper.GetMantissa(ret).signum() == 0) {
        // Value is 0, so just change the exponent
        // to the preferred one
        BigInteger dividendExp = this.helper.GetExponent(thisValue);
        BigInteger divisorExp = this.helper.GetExponent(divisor);
        ret = this.helper.CreateNewWithFlags(
          BigInteger.ZERO,
          dividendExp.subtract(divisorExp),
          this.helper.GetFlags(ret));
      } else {
        if (desiredScale.signum() < 0) {
          // Desired scale is negative, shift left
          desiredScale.Negate();
          BigInteger bigmantissa = (this.helper.GetMantissa(ret)).abs();
          bigmantissa = this.TryMultiplyByRadixPower(bigmantissa, desiredScale);
          if (bigmantissa == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          BigInteger exponentDivisor = this.helper.GetExponent(divisor);
          ret = this.helper.CreateNewWithFlags(
            bigmantissa,
            this.helper.GetExponent(thisValue).subtract(exponentDivisor),
            this.helper.GetFlags(ret));
        } else if (desiredScale.signum() > 0) {
          // Desired scale is positive, shift away zeros
          // but not after scale is reached
          BigInteger bigmantissa = (this.helper.GetMantissa(ret)).abs();
          FastInteger fastexponent =
            FastInteger.FromBig(this.helper.GetExponent(ret));
          BigInteger bigradix = BigInteger.valueOf(this.thisRadix);
          while (true) {
            if (desiredScale.compareTo(fastexponent) == 0) {
              break;
            }
            BigInteger bigrem;
            BigInteger bigquo;
{
BigInteger[] divrem = (bigmantissa).divideAndRemainder(bigradix);
bigquo = divrem[0];
bigrem = divrem[1]; }
            if (bigrem.signum() != 0) {
              break;
            }
            bigmantissa = bigquo;
            fastexponent.Increment();
          }
          ret = this.helper.CreateNewWithFlags(
            bigmantissa,
            fastexponent.AsBigInteger(),
            this.helper.GetFlags(ret));
        }
      }
      if (ctx != null) {
        ret = this.RoundToPrecision(ret, ctx);
      }
      ret = this.EnsureSign(ret, neg);
      return ret;
    }

    public T DivideToIntegerZeroScale(
      T thisValue,
      T divisor,
      PrecisionContext ctx) {
      PrecisionContext ctx2 =
        PrecisionContext.ForRounding(Rounding.Down)
        .WithBigPrecision(ctx ==
                          null ?
                          BigInteger.ZERO :

                          ctx.getPrecision()).WithBlankFlags();
      T ret = this.DivideInternal(
        thisValue,
        divisor,
        ctx2,
        IntegerModeFixedScale,
        BigInteger.ZERO);
      if ((ctx2.getFlags() & (PrecisionContext.FlagInvalid |
                         PrecisionContext.FlagDivideByZero)) != 0) {
        if (ctx.getHasFlags()) {
          ctx.setFlags(ctx.getFlags() | (ctx2.getFlags() & (PrecisionContext.FlagInvalid |
                                     PrecisionContext.FlagDivideByZero)));
        }
        return ret;
      }
      if (ctx != null) {
        ctx2 = ctx.WithBlankFlags().WithUnlimitedExponents();
        ret = this.RoundToPrecision(ret, ctx2);
        if ((ctx2.getFlags() & PrecisionContext.FlagRounded) != 0) {
          return this.SignalInvalid(ctx);
        }
      }
      return ret;
    }

    public T Abs(T value, PrecisionContext ctx) {
      int flags = this.helper.GetFlags(value);
      return ((flags & BigNumberFlags.FlagSignalingNaN) != 0) ?
        this.SignalingNaNInvalid(value, ctx) : (
          ((flags & BigNumberFlags.FlagQuietNaN) != 0) ?
          this.ReturnQuietNaN(
            value,
            ctx) :
          (((flags &
             BigNumberFlags.FlagNegative) !=
            0) ?
           this.RoundToPrecision(
             this.helper.CreateNewWithFlags(this.helper.GetMantissa(value), this.helper.GetExponent(value), flags & ~BigNumberFlags.FlagNegative),
             ctx) :
           this.RoundToPrecision(
             value,
             ctx)));
    }

    public T Negate(T value, PrecisionContext ctx) {
      int flags = this.helper.GetFlags(value);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(value, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.ReturnQuietNaN(value, ctx);
      }
      BigInteger mant = this.helper.GetMantissa(value);
      if ((flags & BigNumberFlags.FlagInfinity) == 0 && mant.signum() == 0) {
        if ((flags & BigNumberFlags.FlagNegative) == 0) {
          // positive 0 minus positive 0 is always positive 0
          return this.RoundToPrecision(
            this.helper.CreateNewWithFlags(mant, this.helper.GetExponent(value), flags & ~BigNumberFlags.FlagNegative),
            ctx);
        }
        if (ctx != null && ctx.getRounding() == Rounding.Floor) {
          // positive 0 minus negative 0 is negative 0 only if
          // the rounding is Floor
          return this.RoundToPrecision(
            this.helper.CreateNewWithFlags(mant, this.helper.GetExponent(value), flags | BigNumberFlags.FlagNegative),
            ctx);
        }
        return this.RoundToPrecision(
          this.helper.CreateNewWithFlags(mant, this.helper.GetExponent(value), flags & ~BigNumberFlags.FlagNegative),
          ctx);
      }
      flags ^= BigNumberFlags.FlagNegative;
      return this.RoundToPrecision(
   this.helper.CreateNewWithFlags(mant, this.helper.GetExponent(value), flags),
   ctx);
    }

    private T AbsRaw(T value) {
      return this.EnsureSign(value, false);
    }

    private boolean IsFinite(T val) {
      return (this.helper.GetFlags(val) & BigNumberFlags.FlagSpecial) == 0;
    }

    private boolean IsNegative(T val) {
      return (this.helper.GetFlags(val) & BigNumberFlags.FlagNegative) != 0;
    }

    private T NegateRaw(T val) {
      if (val == null) {
        return val;
      }
      int sign = this.helper.GetFlags(val) & BigNumberFlags.FlagNegative;
      return this.helper.CreateNewWithFlags(
        this.helper.GetMantissa(val),
        this.helper.GetExponent(val),
        sign == 0 ? BigNumberFlags.FlagNegative : 0);
    }

    private static void TransferFlags(
      PrecisionContext ctxDst,
      PrecisionContext ctxSrc) {
      if (ctxDst != null && ctxDst.getHasFlags()) {
        if ((ctxSrc.getFlags() & (PrecisionContext.FlagInvalid |
                             PrecisionContext.FlagDivideByZero)) != 0) {
          ctxDst.setFlags(ctxDst.getFlags() | (ctxSrc.getFlags() & (PrecisionContext.FlagInvalid |
                                          PrecisionContext.FlagDivideByZero)));
        } else {
          ctxDst.setFlags(ctxDst.getFlags() | (ctxSrc.getFlags()));
        }
      }
    }

    public T Remainder(T thisValue, T divisor, PrecisionContext ctx) {
      PrecisionContext ctx2 = ctx == null ? null : ctx.WithBlankFlags();
      T ret = this.RemainderHandleSpecial(thisValue, divisor, ctx2);
      if ((Object)ret != (Object)null) {
        TransferFlags(ctx, ctx2);
        return ret;
      }
      ret = this.DivideToIntegerZeroScale(thisValue, divisor, ctx2);
      if ((ctx2.getFlags() & PrecisionContext.FlagInvalid) != 0) {
        return this.SignalInvalid(ctx);
      }
      ret = this.Add(
        thisValue,
        this.NegateRaw(this.Multiply(ret, divisor, null)),
        ctx2);
      ret = this.EnsureSign(
        ret,
        (this.helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative) != 0);
      TransferFlags(ctx, ctx2);
      return ret;
    }

    public T RemainderNear(T thisValue, T divisor, PrecisionContext ctx) {
      PrecisionContext ctx2 = ctx == null ?
        PrecisionContext.ForRounding(Rounding.HalfEven).WithBlankFlags() :
        ctx.WithRounding(Rounding.HalfEven).WithBlankFlags();
      T ret = this.RemainderHandleSpecial(thisValue, divisor, ctx2);
      if ((Object)ret != (Object)null) {
        TransferFlags(ctx, ctx2);
        return ret;
      }
      ret = this.DivideInternal(
        thisValue,
        divisor,
        ctx2,
        IntegerModeFixedScale,
        BigInteger.ZERO);
      if ((ctx2.getFlags() & PrecisionContext.FlagInvalid) != 0) {
        return this.SignalInvalid(ctx);
      }
      ctx2 = ctx2.WithBlankFlags();
      ret = this.RoundToPrecision(ret, ctx2);
      if ((ctx2.getFlags() & (PrecisionContext.FlagRounded |
                         PrecisionContext.FlagInvalid)) != 0) {
        return this.SignalInvalid(ctx);
      }
      ctx2 = ctx == null ? PrecisionContext.Unlimited.WithBlankFlags() :
        ctx.WithBlankFlags();
      T ret2 = this.Add(
        thisValue,
        this.NegateRaw(this.Multiply(ret, divisor, null)),
        ctx2);
      if ((ctx2.getFlags() & PrecisionContext.FlagInvalid) != 0) {
        return this.SignalInvalid(ctx);
      }
      if (this.helper.GetFlags(ret2) == 0 &&
          this.helper.GetMantissa(ret2).signum() == 0) {
        ret2 = this.EnsureSign(
          ret2,
          (this.helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative) != 0);
      }
      TransferFlags(ctx, ctx2);
      return ret2;
    }

    public T Pi(PrecisionContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.getHasMaxPrecision()) {
        return this.SignalInvalidWithMessage(
          ctx,
          "ctx has unlimited precision");
      }
      // Gauss-Legendre algorithm
      T a = this.helper.ValueOf(1);
      PrecisionContext ctxdiv = SetPrecisionIfLimited(
        ctx,
        ctx.getPrecision().add(BigInteger.TEN)).WithRounding(this.thisRadix == 2 ?
                                                   Rounding.HalfEven :
                                                   Rounding.ZeroFiveUp);
      T two = this.helper.ValueOf(2);
      T b = this.Divide(a, this.SquareRoot(two, ctxdiv), ctxdiv);
      T four = this.helper.ValueOf(4);
      T half = ((this.thisRadix & 1) == 0) ?
        this.helper.CreateNewWithFlags(
          BigInteger.valueOf(this.thisRadix / 2),
          BigInteger.ZERO.subtract(BigInteger.ONE),
          0) : null;
      T t = this.Divide(a, four, ctxdiv);
      boolean more = true;
      int lastCompare = 0;
      int vacillations = 0;
      T lastGuess = null;
      T guess = null;
      BigInteger powerTwo = BigInteger.ONE;
      while (more) {
        lastGuess = guess;
        T aplusB = this.Add(a, b, null);
        T newA = (half == null) ? this.Divide(aplusB, two, ctxdiv) :
          this.Multiply(aplusB, half, null);
        T valueAMinusNewA = this.Add(a, this.NegateRaw(newA), null);
        if (!a.equals(b)) {
          T atimesB = this.Multiply(a, b, ctxdiv);
          b = this.SquareRoot(atimesB, ctxdiv);
        }
        a = newA;
        guess = this.Multiply(aplusB, aplusB, null);
        guess = this.Divide(guess, this.Multiply(t, four, null), ctxdiv);
        T newGuess = guess;
        if ((Object)lastGuess != (Object)null) {
          int guessCmp = this.compareTo(lastGuess, newGuess);
          if (guessCmp == 0) {
            more = false;
          } else if ((guessCmp > 0 && lastCompare < 0) || (lastCompare > 0 &&
                                                           guessCmp < 0)) {
            // Guesses are vacillating
            ++vacillations;
            more &= vacillations <= 3 || guessCmp <= 0;
          }
          lastCompare = guessCmp;
        }
        if (more) {
          T tmpT = this.Multiply(valueAMinusNewA, valueAMinusNewA, null);
          tmpT = this.Multiply(
            tmpT,
            this.helper.CreateNewWithFlags(powerTwo, BigInteger.ZERO, 0),
            null);
          t = this.Add(t, this.NegateRaw(tmpT), ctxdiv);
          powerTwo = powerTwo.shiftLeft(1);
        }
        guess = newGuess;
      }
      return this.RoundToPrecision(guess, ctx);
    }

    private T LnInternal(
      T thisValue,
      BigInteger workingPrecision,
      PrecisionContext ctx) {
      boolean more = true;
      int lastCompare = 0;
      int vacillations = 0;
      PrecisionContext ctxdiv = SetPrecisionIfLimited(
        ctx,
        workingPrecision.add(BigInteger.valueOf(6)))
        .WithRounding(this.thisRadix == 2 ? Rounding.HalfEven :
                      Rounding.ZeroFiveUp);
      T z = this.Add(this.NegateRaw(thisValue), this.helper.ValueOf(1), null);
      T zpow = this.Multiply(z, z, ctxdiv);
      T guess = this.NegateRaw(z);
      T lastGuess = null;
      BigInteger denom = BigInteger.valueOf(2);
      while (more) {
        lastGuess = guess;
        T tmp = this.Divide(
          zpow,
          this.helper.CreateNewWithFlags(denom, BigInteger.ZERO, 0),
          ctxdiv);
        T newGuess = this.Add(guess, this.NegateRaw(tmp), ctxdiv);
        {
          int guessCmp = this.compareTo(lastGuess, newGuess);
          if (guessCmp == 0) {
            more = false;
          } else if ((guessCmp > 0 && lastCompare < 0) || (lastCompare > 0 &&
                                                           guessCmp < 0)) {
            // Guesses are vacillating
            ++vacillations;
            more &= vacillations <= 3 || guessCmp <= 0;
          }
          lastCompare = guessCmp;
        }
        guess = newGuess;
        if (more) {
          zpow = this.Multiply(zpow, z, ctxdiv);
          denom = denom.add(BigInteger.ONE);
        }
      }
      return this.RoundToPrecision(guess, ctx);
    }

    private T ExpInternal(
      T thisValue,
      BigInteger workingPrecision,
      PrecisionContext ctx) {
      T one = this.helper.ValueOf(1);
      PrecisionContext ctxdiv = SetPrecisionIfLimited(
        ctx,
        workingPrecision.add(BigInteger.valueOf(6)))
        .WithRounding(this.thisRadix == 2 ? Rounding.Down :
                      Rounding.ZeroFiveUp);
      BigInteger bigintN = BigInteger.valueOf(2);
      BigInteger facto = BigInteger.ONE;
      // Guess starts with 1 + thisValue
      T guess = this.Add(one, thisValue, null);
      T lastGuess = guess;
      T pow = thisValue;
      boolean more = true;
      int lastCompare = 0;
      int vacillations = 0;
      while (more) {
        lastGuess = guess;
        // Iterate by:
        // newGuess = guess + (thisValue^n/factorial(n))
        // (n starts at 2 and increases by 1 after
        // each iteration)
        pow = this.Multiply(pow, thisValue, ctxdiv);
        facto = facto.multiply(bigintN);
        T tmp = this.Divide(
          pow,
          this.helper.CreateNewWithFlags(facto, BigInteger.ZERO, 0),
          ctxdiv);
        T newGuess = this.Add(guess, tmp, ctxdiv);
        // System.out.println("newguess" +
        // this.helper.GetMantissa(newGuess)+" ctxdiv " +
        // ctxdiv.getPrecision());
        // System.out.println("newguess " + newGuess);
        // System.out.println("newguessN " + NextPlus(newGuess,ctxdiv));
        {
          int guessCmp = this.compareTo(lastGuess, newGuess);
          if (guessCmp == 0) {
            more = false;
          } else if ((guessCmp > 0 && lastCompare < 0) || (lastCompare > 0 &&
                                                           guessCmp < 0)) {
            // Guesses are vacillating
            ++vacillations;
            more &= vacillations <= 3 || guessCmp <= 0;
          }
          lastCompare = guessCmp;
        }
        guess = newGuess;
        if (more) {
          bigintN = bigintN.add(BigInteger.ONE);
        }
      }
      return this.RoundToPrecision(guess, ctx);
    }

    private static PrecisionContext SetPrecisionIfLimited(
      PrecisionContext ctx,
      BigInteger bigPrecision) {
      return (ctx == null || !ctx.getHasMaxPrecision()) ? ctx :
        ctx.WithBigPrecision(bigPrecision);
    }

    private T PowerIntegral(
      T thisValue,
      BigInteger powIntBig,
      PrecisionContext ctx) {
      int sign = powIntBig.signum();
      T one = this.helper.ValueOf(1);
      if (sign == 0) {
        // however 0 to the power of 0 is undefined
        return this.RoundToPrecision(one, ctx);
      }
      if (powIntBig.equals(BigInteger.ONE)) {
        return this.RoundToPrecision(thisValue, ctx);
      }
      if (powIntBig.equals(BigInteger.valueOf(2))) {
        return this.Multiply(thisValue, thisValue, ctx);
      }
      if (powIntBig.equals(BigInteger.valueOf(3))) {
        return this.Multiply(
          thisValue,
          this.Multiply(thisValue, thisValue, null),
          ctx);
      }
      boolean retvalNeg = this.IsNegative(thisValue) && powIntBig.testBit(0);
      FastInteger error = this.helper.CreateShiftAccumulator(
        (powIntBig).abs()).GetDigitLength();
      error.AddInt(6);
      BigInteger bigError = error.AsBigInteger();
      PrecisionContext ctxdiv = SetPrecisionIfLimited(
        ctx,
        ctx.getPrecision().add(bigError))
        .WithRounding(this.thisRadix == 2 ? Rounding.HalfEven :
                      Rounding.ZeroFiveUp).WithBlankFlags();
      if (sign < 0) {
        // Use the reciprocal for negative powers
        thisValue = this.Divide(one, thisValue, ctxdiv);
        if ((ctxdiv.getFlags() & PrecisionContext.FlagOverflow) != 0) {
          return this.SignalOverflow2(ctx, retvalNeg);
        }
        powIntBig = powIntBig.negate();
      }
      T r = one;
      // System.out.println("starting pow prec="+ctxdiv.getPrecision());
      while (powIntBig.signum() != 0) {
        // System.out.println("powIntBig "+powIntBig.bitLength());
        if (powIntBig.testBit(0)) {
          r = this.Multiply(r, thisValue, ctxdiv);
          // System.out.println("mult mant="
          // +helper.GetMantissa(r).bitLength()+ ", e"
          // +helper.GetExponent(r));
          if ((ctxdiv.getFlags() & PrecisionContext.FlagOverflow) != 0) {
            return this.SignalOverflow2(ctx, retvalNeg);
          }
        }
        powIntBig = powIntBig.shiftRight(1);
        if (powIntBig.signum() != 0) {
          ctxdiv.setFlags(0);
          T tmp = this.Multiply(thisValue, thisValue, ctxdiv);
          // System.out.println("sqr e"+helper.GetExponent(tmp));
          if ((ctxdiv.getFlags() & PrecisionContext.FlagOverflow) != 0) {
            // Avoid multiplying too huge numbers with
            // limited exponent range
            return this.SignalOverflow2(ctx, retvalNeg);
          }
          thisValue = tmp;
        }
      }
      return this.RoundToPrecision(r, ctx);
    }

    private T ExtendPrecision(T thisValue, PrecisionContext ctx) {
      if (ctx == null || !ctx.getHasMaxPrecision()) {
        return this.RoundToPrecision(thisValue, ctx);
      }
      BigInteger mant = (this.helper.GetMantissa(thisValue)).abs();
      FastInteger digits =
        this.helper.CreateShiftAccumulator(mant).GetDigitLength();
      FastInteger fastPrecision = FastInteger.FromBig(ctx.getPrecision());
      BigInteger exponent = this.helper.GetExponent(thisValue);
      if (digits.compareTo(fastPrecision) < 0) {
        fastPrecision.Subtract(digits);
        mant = this.TryMultiplyByRadixPower(mant, fastPrecision);
        if (mant == null) {
          return this.SignalInvalidWithMessage(
            ctx,
            "Result requires too much memory");
        }
        BigInteger bigPrec = fastPrecision.AsBigInteger();
        exponent = exponent.subtract(bigPrec);
      }
      if (ctx != null && ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
        ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact));
      }
      return this.RoundToPrecision(
        this.helper.CreateNewWithFlags(mant, exponent, 0),
        ctx);
    }

    private boolean IsWithinExponentRangeForPow(
      T thisValue,
      PrecisionContext ctx) {
      if (ctx == null || !ctx.getHasExponentRange()) {
        return true;
      }
      FastInteger digits =

  this.helper.CreateShiftAccumulator((this.helper.GetMantissa(thisValue)).abs())
        .GetDigitLength();
      BigInteger exp = this.helper.GetExponent(thisValue);
      FastInteger fi = FastInteger.FromBig(exp);
      if (ctx.getAdjustExponent()) {
        fi.Add(digits);
        fi.Decrement();
      }
      // System.out.println("" + exp + " -> " + (fi));
      if (fi.signum() < 0) {
        fi.Negate().Divide(2).Negate();
        // System.out.println("" + exp + " II -> " + (fi));
      }
      exp = fi.AsBigInteger();
      return exp.compareTo(ctx.getEMin()) >= 0 && exp.compareTo(ctx.getEMax()) <= 0;
    }

    public T Power(T thisValue, T pow, PrecisionContext ctx) {
      T ret = this.HandleNotANumber(thisValue, pow, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      int thisSign = this.helper.GetSign(thisValue);
      int powSign = this.helper.GetSign(pow);
      int thisFlags = this.helper.GetFlags(thisValue);
      int powFlags = this.helper.GetFlags(pow);
      if (thisSign == 0 && powSign == 0) {
        // Both operands are zero: invalid
        return this.SignalInvalid(ctx);
      }
      if (thisSign < 0 && (powFlags & BigNumberFlags.FlagInfinity) != 0) {
        // This value is negative and power is infinity: invalid
        return this.SignalInvalid(ctx);
      }
      if (thisSign > 0 && (thisFlags & BigNumberFlags.FlagInfinity) == 0 &&
          (powFlags & BigNumberFlags.FlagInfinity) != 0) {
        // Power is infinity and this value is greater than
        // zero and not infinity
        int cmp = this.compareTo(thisValue, this.helper.ValueOf(1));
        if (cmp < 0) {
          // Value is less than 1
          if (powSign < 0) {
            // Power is negative infinity, return positive infinity
            return this.helper.CreateNewWithFlags(
              BigInteger.ZERO,
              BigInteger.ZERO,
              BigNumberFlags.FlagInfinity);
          }
          // Power is positive infinity, return 0
          return
            this.RoundToPrecision(
           this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, 0),
           ctx);
        }
        if (cmp == 0) {
          // Extend the precision of the mantissa as much as possible,
          // in the special case that this value is 1
          return this.ExtendPrecision(this.helper.ValueOf(1), ctx);
        }
        // Value is greater than 1
        if (powSign > 0) {
          // Power is positive infinity, return positive infinity
          return pow;
        }
        // Power is negative infinity, return 0
        return
          this.RoundToPrecision(
            this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, 0),
            ctx);
      }
      BigInteger powExponent = this.helper.GetExponent(pow);
      boolean isPowIntegral = powExponent.signum() > 0;
      boolean isPowOdd = false;
      T powInt = null;
      if (!isPowIntegral) {
        powInt = this.Quantize(
          pow,
          this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, 0),
          PrecisionContext.ForRounding(Rounding.Down));
        isPowIntegral = this.compareTo(powInt, pow) == 0;
        isPowOdd = !this.helper.GetMantissa(powInt).testBit(0) == false;
      } else {
        if (powExponent.equals(BigInteger.ZERO)) {
          isPowOdd = !this.helper.GetMantissa(powInt).testBit(0) == false;
        } else if (this.thisRadix % 2 == 0) {
          // Never odd for even radixes
          isPowOdd = false;
        } else {
          powInt = this.Quantize(
            pow,
            this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, 0),
            PrecisionContext.ForRounding(Rounding.Down));
          isPowOdd = !this.helper.GetMantissa(powInt).testBit(0) == false;
        }
      }
      // System.out.println("pow=" + pow + " powint=" + (powInt));
      boolean isResultNegative = (thisFlags & BigNumberFlags.FlagNegative) != 0 &&
        (powFlags & BigNumberFlags.FlagInfinity) == 0 && isPowIntegral &&
        isPowOdd;
      if (thisSign == 0 && powSign != 0) {
        int infinityFlags = (powSign < 0) ? BigNumberFlags.FlagInfinity : 0;
        if (isResultNegative) {
          infinityFlags |= BigNumberFlags.FlagNegative;
        }
        thisValue = this.helper.CreateNewWithFlags(
          BigInteger.ZERO,
          BigInteger.ZERO,
          infinityFlags);
        if ((infinityFlags & BigNumberFlags.FlagInfinity) == 0) {
          thisValue = this.RoundToPrecision(thisValue, ctx);
        }
        return thisValue;
      }
      if ((!isPowIntegral || powSign < 0) && (ctx == null ||
                                              !ctx.getHasMaxPrecision())) {
        String outputMessage = "ctx is null or has unlimited precision, " +
          "and pow's exponent is not an integer or is negative";
        return this.SignalInvalidWithMessage(
          ctx,
          outputMessage);
      }
      if (thisSign < 0 && !isPowIntegral) {
        return this.SignalInvalid(ctx);
      }
      if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
        // This value is infinity
        int negflag = isResultNegative ?
                       BigNumberFlags.FlagNegative : 0;
        return (powSign > 0) ?
          this.RoundToPrecision(
            this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, negflag | BigNumberFlags.FlagInfinity),
            ctx) : ((powSign < 0) ?
                this.RoundToPrecision(
     this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, negflag),
     ctx) :
                this.RoundToPrecision(
            this.helper.CreateNewWithFlags(BigInteger.ONE, BigInteger.ZERO, 0),
            ctx));
      }
      if (powSign == 0) {
        return
          this.RoundToPrecision(
            this.helper.CreateNewWithFlags(BigInteger.ONE, BigInteger.ZERO, 0),
            ctx);
      }
      if (isPowIntegral) {
        // Special case for 1
        if (this.compareTo(thisValue, this.helper.ValueOf(1)) == 0) {
          return (!this.IsWithinExponentRangeForPow(pow, ctx)) ?
            this.SignalInvalid(ctx) : this.helper.ValueOf(1);
        }
        if ((Object)powInt == (Object)null) {
          powInt = this.Quantize(
            pow,
            this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, 0),
            PrecisionContext.ForRounding(Rounding.Down));
        }
        BigInteger signedMant = (this.helper.GetMantissa(powInt)).abs();
        if (powSign < 0) {
          signedMant = signedMant.negate();
        }
        // System.out.println("tv=" + thisValue + " mant=" + (signedMant));
        return this.PowerIntegral(thisValue, signedMant, ctx);
      }
      // Special case for 1
      if (this.compareTo(thisValue, this.helper.ValueOf(1)) == 0 && powSign >
          0) {
        return (!this.IsWithinExponentRangeForPow(pow, ctx)) ?
          this.SignalInvalid(ctx) :
          this.ExtendPrecision(this.helper.ValueOf(1), ctx);
      }

      // Special case for 0.5
      if (this.thisRadix == 10 || this.thisRadix == 2) {
        T half = (this.thisRadix == 10) ?
          this.helper.CreateNewWithFlags(
            BigInteger.valueOf(5),
            BigInteger.ZERO.subtract(BigInteger.ONE),
            0) :
          this.helper.CreateNewWithFlags(
            BigInteger.ONE,
            BigInteger.ZERO.subtract(BigInteger.ONE),
            0);
        if (this.compareTo(pow, half) == 0 &&
            this.IsWithinExponentRangeForPow(pow, ctx) &&
            this.IsWithinExponentRangeForPow(thisValue, ctx)) {
          PrecisionContext ctxCopy = ctx.WithBlankFlags();
          thisValue = this.SquareRoot(thisValue, ctxCopy);
          ctxCopy.setFlags(ctxCopy.getFlags() | (PrecisionContext.FlagInexact));
          ctxCopy.setFlags(ctxCopy.getFlags() | (PrecisionContext.FlagRounded));
          if ((ctxCopy.getFlags() & PrecisionContext.FlagSubnormal) != 0) {
            ctxCopy.setFlags(ctxCopy.getFlags() | (PrecisionContext.FlagUnderflow));
          }
          thisValue = this.ExtendPrecision(thisValue, ctxCopy);
          if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (ctxCopy.getFlags()));
          }
          return thisValue;
        }
      }
      int guardDigitCount = this.thisRadix == 2 ? 32 : 10;
      BigInteger guardDigits = BigInteger.valueOf(guardDigitCount);
      PrecisionContext ctxdiv = SetPrecisionIfLimited(
        ctx,
        ctx.getPrecision().add(guardDigits));
      ctxdiv = ctxdiv.WithRounding(this.thisRadix == 2 ? Rounding.HalfEven :
                                   Rounding.ZeroFiveUp).WithBlankFlags();
      T lnresult = this.Ln(thisValue, ctxdiv);
      /*
      System.out.println("guard= " + guardDigits + " prec=" + ctx.getPrecision()+
        " newprec= " + ctxdiv.getPrecision());
      System.out.println("pwrIn " + pow);
      System.out.println("lnIn " + thisValue);
      System.out.println("lnOut " + lnresult);
      System.out.println("lnOut.get(n) "+this.NextPlus(lnresult,ctxdiv));*/
      lnresult = this.Multiply(lnresult, pow, ctxdiv);
      // System.out.println("expIn " + lnresult);
      // Now use original precision and rounding mode
      ctxdiv = ctx.WithBlankFlags();
      lnresult = this.Exp(lnresult, ctxdiv);
      if ((ctxdiv.getFlags() & (PrecisionContext.FlagClamped |
                           PrecisionContext.FlagOverflow)) != 0) {
        if (!this.IsWithinExponentRangeForPow(thisValue, ctx)) {
          return this.SignalInvalid(ctx);
        }
        if (!this.IsWithinExponentRangeForPow(pow, ctx)) {
          return this.SignalInvalid(ctx);
        }
      }
      if (ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (ctxdiv.getFlags()));
      }
      return lnresult;
    }

    public T Log10(T thisValue, PrecisionContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.getHasMaxPrecision()) {
        return this.SignalInvalidWithMessage(
          ctx,
          "ctx has unlimited precision");
      }
      int flags = this.helper.GetFlags(thisValue);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        // NOTE: Returning a signaling NaN is independent of
        // rounding mode
        return this.SignalingNaNInvalid(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        // NOTE: Returning a quiet NaN is independent of
        // rounding mode
        return this.ReturnQuietNaN(thisValue, ctx);
      }
      int sign = this.helper.GetSign(thisValue);
      if (sign < 0) {
        return this.SignalInvalid(ctx);
      }
      if ((flags & BigNumberFlags.FlagInfinity) != 0) {
        return thisValue;
      }
      PrecisionContext ctxCopy = ctx.WithBlankFlags();
      T one = this.helper.ValueOf(1);
      // System.out.println("input " + (thisValue));
      if (sign == 0) {
        // Result is negative infinity if input is 0
        thisValue =
          this.RoundToPrecision(
            this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagNegative | BigNumberFlags.FlagInfinity),
            ctxCopy);
      } else if (this.compareTo(thisValue, one) == 0) {
        // Result is 0 if input is 1
        thisValue =
          this.RoundToPrecision(
            this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, 0),
            ctxCopy);
      } else {
        BigInteger exp = this.helper.GetExponent(thisValue);
        BigInteger mant = (this.helper.GetMantissa(thisValue)).abs();
        if (mant.equals(BigInteger.ONE) && this.thisRadix == 10) {
          // Value is 1 and radix is 10, so the result is the exponent
          thisValue = this.helper.CreateNewWithFlags(
            exp,
            BigInteger.ZERO,
            exp.signum() < 0 ? BigNumberFlags.FlagNegative : 0);
          thisValue =
            this.RoundToPrecision(
              thisValue,
              ctxCopy);
        } else {
          BigInteger mantissa = this.helper.GetMantissa(thisValue);
          FastInteger expTmp = FastInteger.FromBig(exp);
          BigInteger tenBig = BigInteger.TEN;
          while (true) {
            BigInteger bigrem;
            BigInteger bigquo;
{
BigInteger[] divrem = (mantissa).divideAndRemainder(tenBig);
bigquo = divrem[0];
bigrem = divrem[1]; }
            if (bigrem.signum() != 0) {
              break;
            }
            mantissa = bigquo;
            expTmp.Increment();
          }
          if (mantissa.compareTo(BigInteger.ONE) == 0 &&
              (this.thisRadix == 10 || expTmp.signum() == 0 || exp.signum() == 0)) {
            // Value is an integer power of 10
            thisValue = this.helper.CreateNewWithFlags(
              expTmp.AsBigInteger(),
              BigInteger.ZERO,
              expTmp.signum() < 0 ? BigNumberFlags.FlagNegative : 0);
            thisValue =
              thisValue = this.RoundToPrecision(
                thisValue,
                ctxCopy);
          } else {
            PrecisionContext ctxdiv = SetPrecisionIfLimited(
              ctx,
              ctx.getPrecision().add(BigInteger.TEN))
              .WithRounding(this.thisRadix == 2 ? Rounding.HalfEven :
                            Rounding.ZeroFiveUp).WithBlankFlags();
            T logNatural = this.Ln(thisValue, ctxdiv);
            T logTen = this.LnTenConstant(ctxdiv);
            // T logTen = this.Ln(this.helper.ValueOf(10), ctxdiv);
            thisValue = this.Divide(logNatural, logTen, ctx);
            // Treat result as inexact
            if (ctx.getHasFlags()) {
              ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact |
                PrecisionContext.FlagRounded));
            }
          }
        }
      }
      if (ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (ctxCopy.getFlags()));
      }
      return thisValue;
    }

    private static BigInteger PowerOfTwo(FastInteger fi) {
      if (fi.signum() <= 0) {
        return BigInteger.ONE;
      }
      if (fi.CanFitInInt32()) {
        int val = fi.AsInt32();
        if (val <= 30) {
          val = 1 << val;
          return BigInteger.valueOf(val);
        }
        return BigInteger.ONE.shiftLeft(val);
      } else {
        BigInteger bi = BigInteger.ONE;
        FastInteger fi2 = FastInteger.Copy(fi);
        while (fi2.signum() > 0) {
          int count = 1000000;
          if (fi2.CompareToInt(1000000) < 0) {
            count = bi.intValue();
          }
          bi = bi.shiftLeft(count);
          fi2.SubtractInt(count);
        }
        return bi;
      }
    }

    private T LnTenConstant(PrecisionContext ctx) {
      T thisValue = this.helper.ValueOf(10);
      FastInteger error;
      BigInteger bigError;
      error = new FastInteger(10);
      bigError = error.AsBigInteger();
      PrecisionContext ctxdiv = SetPrecisionIfLimited(
        ctx,
        ctx.getPrecision().add(bigError))
        .WithRounding(this.thisRadix == 2 ? Rounding.HalfEven :
                      Rounding.ZeroFiveUp).WithBlankFlags();
      for (int i = 0; i < 9; ++i) {
        thisValue = this.SquareRoot(thisValue, ctxdiv.WithUnlimitedExponents());
      }
      // Find -Ln(1/thisValue)
      thisValue = this.Divide(this.helper.ValueOf(1), thisValue, ctxdiv);
      thisValue = this.LnInternal(thisValue, ctxdiv.getPrecision(), ctxdiv);
      thisValue = this.NegateRaw(thisValue);
      thisValue = this.Multiply(thisValue, this.helper.ValueOf(1 << 9), ctx);
      if (ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact));
        ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
      }
      return thisValue;
    }

    public T Ln(T thisValue, PrecisionContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.getHasMaxPrecision()) {
        return this.SignalInvalidWithMessage(
          ctx,
          "ctx has unlimited precision");
      }
      int flags = this.helper.GetFlags(thisValue);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        // NOTE: Returning a signaling NaN is independent of
        // rounding mode
        return this.SignalingNaNInvalid(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        // NOTE: Returning a quiet NaN is independent of
        // rounding mode
        return this.ReturnQuietNaN(thisValue, ctx);
      }
      int sign = this.helper.GetSign(thisValue);
      if (sign < 0) {
        return this.SignalInvalid(ctx);
      }
      if ((flags & BigNumberFlags.FlagInfinity) != 0) {
        return thisValue;
      }
      PrecisionContext ctxCopy = ctx.WithBlankFlags();
      T one = this.helper.ValueOf(1);
      if (sign == 0) {
        return this.helper.CreateNewWithFlags(
          BigInteger.ZERO,
          BigInteger.ZERO,
          BigNumberFlags.FlagNegative | BigNumberFlags.FlagInfinity);
      } else {
        int cmpOne = this.compareTo(thisValue, one);
        PrecisionContext ctxdiv = null;
        if (cmpOne == 0) {
          // Equal to 1
          thisValue =
            this.RoundToPrecision(
           this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, 0),
           ctxCopy);
        } else if (cmpOne < 0) {
          // Less than 1
          FastInteger error = new FastInteger(10);
          BigInteger bigError = error.AsBigInteger();
          ctxdiv = SetPrecisionIfLimited(ctx, ctx.getPrecision().add(bigError))
            .WithRounding(this.thisRadix == 2 ? Rounding.HalfEven :
                          Rounding.ZeroFiveUp).WithBlankFlags();
          T quarter = this.Divide(one, this.helper.ValueOf(4), ctxCopy);
          if (this.compareTo(thisValue, quarter) <= 0) {
            // One quarter or less
            T half = this.Multiply(quarter, this.helper.ValueOf(2), null);
            FastInteger roots = new FastInteger(0);
            // Take square root until this value
            // is one half or more
            while (this.compareTo(thisValue, half) < 0) {
              thisValue = this.SquareRoot(
                thisValue,
                ctxdiv.WithUnlimitedExponents());
              roots.Increment();
            }
            thisValue = this.LnInternal(thisValue, ctxdiv.getPrecision(), ctxdiv);
            BigInteger bigintRoots = PowerOfTwo(roots);
            // Multiply back 2^X, where X is the number
            // of square root calls
            thisValue = this.Multiply(
              thisValue,
              this.helper.CreateNewWithFlags(bigintRoots, BigInteger.ZERO, 0),
              ctxCopy);
          } else {
            T smallfrac = this.Divide(one, this.helper.ValueOf(16), ctxdiv);
            T closeToOne = this.Add(one, this.NegateRaw(smallfrac), null);
            if (this.compareTo(thisValue, closeToOne) >= 0) {
              // This value is close to 1, so use a higher working precision
              error =

  this.helper.CreateShiftAccumulator((this.helper.GetMantissa(thisValue)).abs())
                .GetDigitLength();
              error.AddInt(6);
              error.AddBig(ctx.getPrecision());
              bigError = error.AsBigInteger();
              thisValue = this.LnInternal(
                thisValue,
                error.AsBigInteger(),
                ctxCopy);
            } else {
              thisValue = this.LnInternal(thisValue, ctxdiv.getPrecision(), ctxCopy);
            }
          }
          if (ctx.getHasFlags()) {
            ctxCopy.setFlags(ctxCopy.getFlags() | (PrecisionContext.FlagInexact));
            ctxCopy.setFlags(ctxCopy.getFlags() | (PrecisionContext.FlagRounded));
          }
        } else {
          // Greater than 1
          T two = this.helper.ValueOf(2);
          if (this.compareTo(thisValue, two) >= 0) {
            FastInteger roots = new FastInteger(0);
            FastInteger error;
            BigInteger bigError;
            error = new FastInteger(10);
            bigError = error.AsBigInteger();
            ctxdiv = SetPrecisionIfLimited(ctx, ctx.getPrecision().add(bigError))
              .WithRounding(this.thisRadix == 2 ? Rounding.HalfEven :
                            Rounding.ZeroFiveUp).WithBlankFlags();
            T smallfrac = this.Divide(one, this.helper.ValueOf(10), ctxdiv);
            T closeToOne = this.Add(one, smallfrac, null);
            // Take square root until this value
            // is close to 1
            while (this.compareTo(thisValue, closeToOne) >= 0) {
              thisValue = this.SquareRoot(
                thisValue,
                ctxdiv.WithUnlimitedExponents());
              roots.Increment();
            }
            // Find -Ln(1/thisValue)
            thisValue = this.Divide(one, thisValue, ctxdiv);
            thisValue = this.LnInternal(thisValue, ctxdiv.getPrecision(), ctxdiv);
            thisValue = this.NegateRaw(thisValue);
            BigInteger bigintRoots = PowerOfTwo(roots);
            // Multiply back 2^X, where X is the number
            // of square root calls
            thisValue = this.Multiply(
              thisValue,
              this.helper.CreateNewWithFlags(bigintRoots, BigInteger.ZERO, 0),
              ctxCopy);
          } else {
            FastInteger error;
            BigInteger bigError;
            error = new FastInteger(10);
            bigError = error.AsBigInteger();
            ctxdiv = SetPrecisionIfLimited(ctx, ctx.getPrecision().add(bigError))
              .WithRounding(this.thisRadix == 2 ? Rounding.HalfEven :
                            Rounding.ZeroFiveUp).WithBlankFlags();
            T smallfrac = this.Divide(one, this.helper.ValueOf(16), ctxdiv);
            T closeToOne = this.Add(one, smallfrac, null);
            if (this.compareTo(thisValue, closeToOne) >= 0) {
              // Find -Ln(1/thisValue)
              thisValue = this.Divide(one, thisValue, ctxdiv);
              thisValue = this.LnInternal(thisValue, ctxdiv.getPrecision(), ctxCopy);
              thisValue = this.NegateRaw(thisValue);
            } else {
              error =

  this.helper.CreateShiftAccumulator((this.helper.GetMantissa(thisValue)).abs())
                .GetDigitLength();
              error.AddInt(6);
              error.AddBig(ctx.getPrecision());
              bigError = error.AsBigInteger();
              // Greater than 1 and close to 1, will require a higher working
              // precision
              thisValue = this.LnInternal(
                thisValue,
                error.AsBigInteger(),
                ctxCopy);
            }
          }
          if (ctx.getHasFlags()) {
            ctxCopy.setFlags(ctxCopy.getFlags() | (PrecisionContext.FlagInexact));
            ctxCopy.setFlags(ctxCopy.getFlags() | (PrecisionContext.FlagRounded));
          }
        }
      }
      if (ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (ctxCopy.getFlags()));
      }
      return thisValue;
    }

    public T Exp(T thisValue, PrecisionContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.getHasMaxPrecision()) {
        return this.SignalInvalidWithMessage(
          ctx,
          "ctx has unlimited precision");
      }
      int flags = this.helper.GetFlags(thisValue);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        // NOTE: Returning a signaling NaN is independent of
        // rounding mode
        return this.SignalingNaNInvalid(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        // NOTE: Returning a quiet NaN is independent of
        // rounding mode
        return this.ReturnQuietNaN(thisValue, ctx);
      }
      PrecisionContext ctxCopy = ctx.WithBlankFlags();
      if ((flags & BigNumberFlags.FlagInfinity) != 0) {
        if ((flags & BigNumberFlags.FlagNegative) != 0) {
          T retval = this.helper.CreateNewWithFlags(
            BigInteger.ZERO,
            BigInteger.ZERO,
            0);
          retval = this.RoundToPrecision(
            retval,
            ctxCopy);
          if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (ctxCopy.getFlags()));
          }
          return retval;
        }
        return thisValue;
      }
      int sign = this.helper.GetSign(thisValue);
      T one = this.helper.ValueOf(1);
      BigInteger guardDigits = this.thisRadix == 2 ? ctx.getPrecision().add(BigInteger.TEN) :
        BigInteger.TEN;
      PrecisionContext ctxdiv = SetPrecisionIfLimited(
        ctx,
        ctx.getPrecision().add(guardDigits))
        .WithRounding(this.thisRadix == 2 ? Rounding.HalfEven :
                      Rounding.ZeroFiveUp).WithBlankFlags();
      if (sign == 0) {
        thisValue = this.RoundToPrecision(one, ctxCopy);
      } else if (sign > 0 && this.compareTo(thisValue, one) < 0) {
        thisValue = this.ExpInternal(thisValue, ctxdiv.getPrecision(), ctxCopy);
        if (ctx.getHasFlags()) {
          ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact |
            PrecisionContext.FlagRounded));
        }
      } else if (sign < 0) {
        T val = this.Exp(this.NegateRaw(thisValue), ctxdiv);
        if ((ctxdiv.getFlags() & PrecisionContext.FlagOverflow) != 0 ||
            !this.IsFinite(val)) {
          // Overflow, try again with expanded exponent range
          BigInteger newMax;
          ctxdiv.setFlags(0);
          newMax = ctx.getEMax();
          BigInteger expdiff = ctx.getEMin();
          expdiff = newMax.subtract(expdiff);
          newMax = newMax.add(expdiff);
          ctxdiv = ctxdiv.WithBigExponentRange(ctxdiv.getEMin(), newMax);
          thisValue = this.Exp(this.NegateRaw(thisValue), ctxdiv);
          if ((ctxdiv.getFlags() & PrecisionContext.FlagOverflow) != 0) {
            // Still overflowed
            if (ctx.getHasFlags()) {
              ctx.setFlags(ctx.getFlags() | (BigNumberFlags.UnderflowFlags));
            }
            // Return a "subnormal" zero, with fake extra digits to stimulate
            // rounding
            BigInteger ctxdivPrec = ctxdiv.getPrecision();
            newMax = ctx.getEMin();
            if (ctx.getAdjustExponent()) {
              newMax = newMax.subtract(ctxdivPrec);
              newMax = newMax.add(BigInteger.ONE);
            }
            thisValue = this.helper.CreateNewWithFlags(
              BigInteger.ZERO,
              newMax,
              0);
            return this.RoundToPrecisionInternal(
              thisValue,
              0,
              1,
              null,
              false,
              ctx);
          }
        } else {
          thisValue = val;
        }
        thisValue = this.Divide(one, thisValue, ctxCopy);
        // System.out.println("end= " + (thisValue));
        // System.out.println("endbit "+this.BitMantissa(thisValue));
        if (ctx.getHasFlags()) {
          ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact |
            PrecisionContext.FlagRounded));
        }
      } else {
        T intpart = this.Quantize(
          thisValue,
          one,
          PrecisionContext.ForRounding(Rounding.Down));
        if (this.compareTo(thisValue, this.helper.ValueOf(50000)) > 0 &&
            ctx.getHasExponentRange()) {
          // Try to check for overflow quickly
          // Do a trial powering using a lower number than e,
          // and a power of 50000
          this.PowerIntegral(
            this.helper.ValueOf(2),
            BigInteger.valueOf(50000),
            ctxCopy);
          if ((ctxCopy.getFlags() & PrecisionContext.FlagOverflow) != 0) {
            // The trial powering caused overflow, so exp will
            // cause overflow as well
            return this.SignalOverflow2(ctx, false);
          }
          ctxCopy.setFlags(0);
          // Now do the same using the integer part of the operand
          // as the power
          this.PowerIntegral(
            this.helper.ValueOf(2),
            this.helper.GetMantissa(intpart),
            ctxCopy);
          if ((ctxCopy.getFlags() & PrecisionContext.FlagOverflow) != 0) {
            // The trial powering caused overflow, so exp will
            // cause overflow as well
            return this.SignalOverflow2(ctx, false);
          }
          ctxCopy.setFlags(0);
        }
        T fracpart = this.Add(thisValue, this.NegateRaw(intpart), null);
        fracpart = this.Add(one, this.Divide(fracpart, intpart, ctxdiv), null);
        ctxdiv.setFlags(0);
        // System.out.println(fracpart);
        thisValue = this.ExpInternal(fracpart, ctxdiv.getPrecision(), ctxdiv);
        // System.out.println(thisValue);
        if ((ctxdiv.getFlags() & PrecisionContext.FlagUnderflow) != 0) {
          if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (ctxdiv.getFlags()));
          }
        }
        if (ctx.getHasFlags()) {
          ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact |
            PrecisionContext.FlagRounded));
        }
        thisValue = this.PowerIntegral(
          thisValue,
          this.helper.GetMantissa(intpart),
          ctxCopy);
      }
      if (ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (ctxCopy.getFlags()));
      }
      return thisValue;
    }

    public T SquareRoot(T thisValue, PrecisionContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.getHasMaxPrecision()) {
        return this.SignalInvalidWithMessage(
          ctx,
          "ctx has unlimited precision");
      }
      T ret = this.SquareRootHandleSpecial(thisValue, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      PrecisionContext ctxtmp = ctx.WithBlankFlags();
      BigInteger currentExp = this.helper.GetExponent(thisValue);
      BigInteger origExp = currentExp;
      BigInteger idealExp;
      idealExp = currentExp;
      idealExp = idealExp.divide(BigInteger.valueOf(2));
      if (currentExp.signum() < 0 && currentExp.testBit(0)) {
        // Round towards negative infinity; BigInteger's
        // division operation rounds towards zero
        idealExp = idealExp.subtract(BigInteger.ONE);
      }
      // System.out.println("curr=" + currentExp + " ideal=" + (idealExp));
      if (this.helper.GetSign(thisValue) == 0) {
        ret =
          this.RoundToPrecision(
            this.helper.CreateNewWithFlags(BigInteger.ZERO, idealExp, this.helper.GetFlags(thisValue)),
            ctxtmp);
        if (ctx.getHasFlags()) {
          ctx.setFlags(ctx.getFlags() | (ctxtmp.getFlags()));
        }
        return ret;
      }
      BigInteger mantissa = (this.helper.GetMantissa(thisValue)).abs();
      IShiftAccumulator accum = this.helper.CreateShiftAccumulator(mantissa);
      FastInteger digitCount = accum.GetDigitLength();
      FastInteger targetPrecision = FastInteger.FromBig(ctx.getPrecision());
      FastInteger precision =
        FastInteger.Copy(targetPrecision).Multiply(2).AddInt(2);
      boolean rounded = false;
      boolean inexact = false;
      if (digitCount.compareTo(precision) < 0) {
        FastInteger diff = FastInteger.Copy(precision).Subtract(digitCount);
        // System.out.println(diff);
        if ((!diff.isEvenNumber()) ^ (origExp.testBit(0))) {
          diff.Increment();
        }
        BigInteger bigdiff = diff.AsBigInteger();
        currentExp = currentExp.subtract(bigdiff);
        mantissa = this.TryMultiplyByRadixPower(mantissa, diff);
        if (mantissa == null) {
          return this.SignalInvalidWithMessage(
            ctx,
            "Result requires too much memory");
        }
      }
      BigInteger[] sr = mantissa.sqrtWithRemainder();
      digitCount = this.helper.CreateShiftAccumulator(sr[0]).GetDigitLength();
      BigInteger squareRootRemainder = sr[1];
      // System.out.println("I " + mantissa + " -> " + sr[0] + " [target="+
      // targetPrecision + "], (zero= " + squareRootRemainder.signum()==0 +") "
      mantissa = sr[0];
      if (squareRootRemainder.signum() != 0) {
        rounded = true;
        inexact = true;
      }
      BigInteger oldexp = currentExp;
      currentExp = currentExp.divide(BigInteger.valueOf(2));
      if (oldexp.signum() < 0 && oldexp.testBit(0)) {
        // Round towards negative infinity; BigInteger's
        // division operation rounds towards zero
        currentExp = currentExp.subtract(BigInteger.ONE);
      }
      T retval = this.helper.CreateNewWithFlags(mantissa, currentExp, 0);
      // System.out.println("idealExp= " + idealExp + ", curr" + currentExp
      // +" guess= " + (mantissa));
      retval = this.RoundToPrecisionInternal(
        retval,
        0,
        inexact ? 1 : 0,
        null,
        false,
        ctxtmp);
      currentExp = this.helper.GetExponent(retval);
      // System.out.println("guess I " + guess + " idealExp=" + idealExp
      // +", curr " + currentExp + " clamped= " +
      // (ctxtmp.getFlags()&PrecisionContext.FlagClamped));
      if ((ctxtmp.getFlags() & PrecisionContext.FlagUnderflow) == 0) {
        int expcmp = currentExp.compareTo(idealExp);
        if (expcmp <= 0 || !this.IsFinite(retval)) {
          retval = this.ReduceToPrecisionAndIdealExponent(
            retval,
            ctx.getHasExponentRange() ? ctxtmp : null,
            inexact ? targetPrecision : null,
            FastInteger.FromBig(idealExp));
        }
      }
      if (ctx.getHasFlags() && ctx.getClampNormalExponents() &&
          !this.helper.GetExponent(retval).equals(idealExp) && (ctxtmp.getFlags() &
  PrecisionContext.FlagInexact) ==
          0) {
        ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagClamped));
      }
      rounded |= (ctxtmp.getFlags() & PrecisionContext.FlagOverflow) != 0;
      // System.out.println("guess II " + (guess));
      currentExp = this.helper.GetExponent(retval);
      if (rounded) {
        ctxtmp.setFlags(ctxtmp.getFlags() | (PrecisionContext.FlagRounded));
      } else {
        if (currentExp.compareTo(idealExp) > 0) {
          // Greater than the ideal, treat as rounded anyway
          ctxtmp.setFlags(ctxtmp.getFlags() | (PrecisionContext.FlagRounded));
        } else {
          // System.out.println("idealExp= " + idealExp + ", curr" +
          // currentExp + " (II)");
          ctxtmp.setFlags(ctxtmp.getFlags() & ~(PrecisionContext.FlagRounded));
        }
      }
      if (inexact) {
        ctxtmp.setFlags(ctxtmp.getFlags() | (PrecisionContext.FlagRounded));
        ctxtmp.setFlags(ctxtmp.getFlags() | (PrecisionContext.FlagInexact));
      }
      if (ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (ctxtmp.getFlags()));
      }
      return retval;
    }

    public T NextMinus(T thisValue, PrecisionContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.getHasMaxPrecision()) {
        return this.SignalInvalidWithMessage(
          ctx,
          "ctx has unlimited precision");
      }
      if (!ctx.getHasExponentRange()) {
        return this.SignalInvalidWithMessage(
          ctx,
          "doesn't satisfy ctx.getHasExponentRange()");
      }
      int flags = this.helper.GetFlags(thisValue);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.ReturnQuietNaN(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagInfinity) != 0) {
        if ((flags & BigNumberFlags.FlagNegative) != 0) {
          return thisValue;
        } else {
          BigInteger bigexp2 = ctx.getEMax();
          BigInteger bigprec = ctx.getPrecision();
          if (ctx.getAdjustExponent()) {
            bigexp2 = bigexp2.add(BigInteger.ONE);
            bigexp2 = bigexp2.subtract(bigprec);
          }
          BigInteger overflowMant =
            this.TryMultiplyByRadixPower(
              BigInteger.ONE,
              FastInteger.FromBig(ctx.getPrecision()));
          if (overflowMant == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          overflowMant = overflowMant.subtract(BigInteger.ONE);
          return this.helper.CreateNewWithFlags(overflowMant, bigexp2, 0);
        }
      }
      FastInteger minexp = FastInteger.FromBig(ctx.getEMin());
      if (ctx.getAdjustExponent()) {
        minexp.SubtractBig(ctx.getPrecision()).Increment();
      }
      FastInteger bigexp =
        FastInteger.FromBig(this.helper.GetExponent(thisValue));
      if (bigexp.compareTo(minexp) <= 0) {
        // Use a smaller exponent if the input exponent is already
        // very small
        minexp = FastInteger.Copy(bigexp).SubtractInt(2);
      }
      T quantum = this.helper.CreateNewWithFlags(
        BigInteger.ONE,
        minexp.AsBigInteger(),
        BigNumberFlags.FlagNegative);
      PrecisionContext ctx2;
      ctx2 = ctx.WithRounding(Rounding.Floor);
      return this.Add(thisValue, quantum, ctx2);
    }

    public T NextToward(T thisValue, T otherValue, PrecisionContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.getHasMaxPrecision()) {
        return this.SignalInvalidWithMessage(
          ctx,
          "ctx has unlimited precision");
      }
      if (!ctx.getHasExponentRange()) {
        return this.SignalInvalidWithMessage(
          ctx,
          "doesn't satisfy ctx.getHasExponentRange()");
      }
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(otherValue);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = this.HandleNotANumber(thisValue, otherValue, ctx);
        if ((Object)result != (Object)null) {
          return result;
        }
      }
      PrecisionContext ctx2;
      int cmp = this.compareTo(thisValue, otherValue);
      if (cmp == 0) {
        return this.RoundToPrecision(
   this.EnsureSign(thisValue, (otherFlags & BigNumberFlags.FlagNegative) != 0),
   ctx.WithNoFlags());
      } else {
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          if ((thisFlags & (BigNumberFlags.FlagInfinity |
                            BigNumberFlags.FlagNegative)) == (otherFlags &
                                                (BigNumberFlags.FlagInfinity |
  BigNumberFlags.FlagNegative))) {
            // both values are the same infinity
            return thisValue;
          } else {
            BigInteger bigexp2 = ctx.getEMax();
            BigInteger bigprec = ctx.getPrecision();
            if (ctx.getAdjustExponent()) {
              bigexp2 = bigexp2.add(BigInteger.ONE);
              bigexp2 = bigexp2.subtract(bigprec);
            }
            BigInteger overflowMant =
              this.TryMultiplyByRadixPower(
                BigInteger.ONE,
                FastInteger.FromBig(ctx.getPrecision()));
            if (overflowMant == null) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Result requires too much memory");
            }
            overflowMant = overflowMant.subtract(BigInteger.ONE);
            return this.helper.CreateNewWithFlags(
              overflowMant,
              bigexp2,
              thisFlags & BigNumberFlags.FlagNegative);
          }
        }
        FastInteger minexp = FastInteger.FromBig(ctx.getEMin());
        if (ctx.getAdjustExponent()) {
          minexp.SubtractBig(ctx.getPrecision()).Increment();
        }
        FastInteger bigexp =
          FastInteger.FromBig(this.helper.GetExponent(thisValue));
        if (bigexp.compareTo(minexp) < 0) {
          // Use a smaller exponent if the input exponent is already
          // very small
          minexp = FastInteger.Copy(bigexp).SubtractInt(2);
        } else {
          // Ensure the exponent is lower than the exponent range
          // (necessary to flag underflow correctly)
          minexp.SubtractInt(2);
        }
        T quantum = this.helper.CreateNewWithFlags(
          BigInteger.ONE,
          minexp.AsBigInteger(),
          (cmp > 0) ? BigNumberFlags.FlagNegative : 0);
        T val = thisValue;
        ctx2 = ctx.WithRounding((cmp > 0) ? Rounding.Floor :
                                Rounding.Ceiling).WithBlankFlags();
        val = this.Add(val, quantum, ctx2);
        if ((ctx2.getFlags() & (PrecisionContext.FlagOverflow |
                           PrecisionContext.FlagUnderflow)) == 0) {
          // Don't set flags except on overflow or underflow,
          // in accordance with the DecTest test cases
          ctx2.setFlags(0);
        }
        if ((ctx2.getFlags() & PrecisionContext.FlagUnderflow) != 0) {
          BigInteger bigmant = (this.helper.GetMantissa(val)).abs();
          BigInteger maxmant = this.TryMultiplyByRadixPower(
            BigInteger.ONE,
            FastInteger.FromBig(ctx.getPrecision()).Decrement());
          if (maxmant == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          if (bigmant.compareTo(maxmant) >= 0 ||
              ctx.getPrecision().compareTo(BigInteger.ONE) == 0) {
            // don't treat max-precision results as having underflowed
            ctx2.setFlags(0);
          }
        }
        if (ctx.getHasFlags()) {
          ctx.setFlags(ctx.getFlags() | (ctx2.getFlags()));
        }
        return val;
      }
    }

    public T NextPlus(T thisValue, PrecisionContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.getHasMaxPrecision()) {
        return this.SignalInvalidWithMessage(
          ctx,
          "ctx has unlimited precision");
      }
      if (!ctx.getHasExponentRange()) {
        return this.SignalInvalidWithMessage(
          ctx,
          "doesn't satisfy ctx.getHasExponentRange()");
      }
      int flags = this.helper.GetFlags(thisValue);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.ReturnQuietNaN(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagInfinity) != 0) {
        if ((flags & BigNumberFlags.FlagNegative) != 0) {
          BigInteger bigexp2 = ctx.getEMax();
          BigInteger bigprec = ctx.getPrecision();
          if (ctx.getAdjustExponent()) {
            bigexp2 = bigexp2.add(BigInteger.ONE);
            bigexp2 = bigexp2.subtract(bigprec);
          }
          BigInteger overflowMant =
            this.TryMultiplyByRadixPower(
              BigInteger.ONE,
              FastInteger.FromBig(ctx.getPrecision()));
          if (overflowMant == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          overflowMant = overflowMant.subtract(BigInteger.ONE);
          return this.helper.CreateNewWithFlags(
            overflowMant,
            bigexp2,
            BigNumberFlags.FlagNegative);
        }
        return thisValue;
      }
      FastInteger minexp = FastInteger.FromBig(ctx.getEMin());
      if (ctx.getAdjustExponent()) {
        minexp.SubtractBig(ctx.getPrecision()).Increment();
      }
      FastInteger bigexp =
        FastInteger.FromBig(this.helper.GetExponent(thisValue));
      if (bigexp.compareTo(minexp) <= 0) {
        // Use a smaller exponent if the input exponent is already
        // very small
        minexp = FastInteger.Copy(bigexp).SubtractInt(2);
      }
      T quantum = this.helper.CreateNewWithFlags(
        BigInteger.ONE,
        minexp.AsBigInteger(),
        0);
      PrecisionContext ctx2;
      T val = thisValue;
      ctx2 = ctx.WithRounding(Rounding.Ceiling);
      return this.Add(val, quantum, ctx2);
    }

    public T DivideToExponent(
      T thisValue,
      T divisor,
      BigInteger desiredExponent,
      PrecisionContext ctx) {
      if (ctx != null && !ctx.ExponentWithinRange(desiredExponent)) {
        return this.SignalInvalidWithMessage(
          ctx,
          "Exponent not within exponent range: " + desiredExponent);
      }
      PrecisionContext ctx2 = (ctx == null) ?
        PrecisionContext.ForRounding(Rounding.HalfDown) :
        ctx.WithUnlimitedExponents().WithPrecision(0);
      T ret = this.DivideInternal(
        thisValue,
        divisor,
        ctx2,
        IntegerModeFixedScale,
        desiredExponent);
      if (!ctx2.getHasMaxPrecision() && this.IsFinite(ret)) {
        // If a precision is given, call Quantize to ensure
        // that the value fits the precision
        ret = this.Quantize(ret, ret, ctx2);
        if ((ctx2.getFlags() & PrecisionContext.FlagInvalid) != 0) {
          ctx2.setFlags(PrecisionContext.FlagInvalid);
        }
      }
      if (ctx != null && ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (ctx2.getFlags()));
      }
      return ret;
    }

    public T Divide(T thisValue, T divisor, PrecisionContext ctx) {
      return this.DivideInternal(
        thisValue,
        divisor,
        ctx,
        IntegerModeRegular,
        BigInteger.ZERO);
    }

    private int[] RoundToScaleStatus(
      BigInteger remainder,
      BigInteger divisor,
      PrecisionContext ctx) {
      Rounding rounding = (ctx == null) ? Rounding.HalfEven : ctx.getRounding();
      int lastDiscarded = 0;
      int olderDiscarded = 0;
      if (remainder.signum() != 0) {
        if (rounding == Rounding.HalfDown || rounding == Rounding.HalfUp ||
            rounding == Rounding.HalfEven) {
          BigInteger halfDivisor = divisor.shiftRight(1);
          int cmpHalf = remainder.compareTo(halfDivisor);
          if ((cmpHalf == 0) && divisor.testBit(0) == false) {
            // remainder is exactly half
            lastDiscarded = this.thisRadix / 2;
            olderDiscarded = 0;
          } else if (cmpHalf > 0) {
            // remainder is greater than half
            lastDiscarded = this.thisRadix / 2;
            olderDiscarded = 1;
          } else {
            // remainder is less than half
            lastDiscarded = 0;
            olderDiscarded = 1;
          }
        } else {
          // Rounding mode doesn't care about
          // whether remainder is exactly half
          if (rounding == Rounding.Unnecessary) {
            // Rounding was required
            return null;
          }
          lastDiscarded = 1;
          olderDiscarded = 1;
        }
      }
      return new int[] { lastDiscarded,
        olderDiscarded };
    }

    private BigInteger TryMultiplyByRadixPower(
      BigInteger bi,
      FastInteger radixPower) {
      if (bi.signum() == 0) {
        return bi;
      }
      if (!radixPower.CanFitInInt32()) {
        return null;
      }
      FastInteger tmp = FastInteger.Copy(radixPower);
      if (this.thisRadix == 10) {
        // NOTE: 3 is a conservative number less than ln(10)/ln(2).
        // 8 is the number of bits in a byte. Thus, we check if the
        // radix power's length in bytes will meet or exceed the range of a
        // 32-bit signed integer.
        if (tmp.Multiply(3).Divide(8).CompareToInt(Integer.MAX_VALUE) > 0) {
          return null;
        }
      }
      return this.helper.MultiplyByRadixPower(bi, radixPower);
    }

    private T RoundToScale(
      BigInteger mantissa,
      BigInteger remainder,
      BigInteger divisor,
      BigInteger desiredExponent,
      FastInteger shift,
      boolean neg,
      PrecisionContext ctx) {
      IShiftAccumulator accum;
      Rounding rounding = (ctx == null) ? Rounding.HalfEven : ctx.getRounding();
      int lastDiscarded = 0;
      int olderDiscarded = 0;
      if (remainder.signum() != 0) {
        if (rounding == Rounding.HalfDown || rounding == Rounding.HalfUp ||
            rounding == Rounding.HalfEven) {
          BigInteger halfDivisor = divisor.shiftRight(1);
          int cmpHalf = remainder.compareTo(halfDivisor);
          if ((cmpHalf == 0) && divisor.testBit(0) == false) {
            // remainder is exactly half
            lastDiscarded = this.thisRadix / 2;
            olderDiscarded = 0;
          } else if (cmpHalf > 0) {
            // remainder is greater than half
            lastDiscarded = this.thisRadix / 2;
            olderDiscarded = 1;
          } else {
            // remainder is less than half
            lastDiscarded = 0;
            olderDiscarded = 1;
          }
        } else {
          // Rounding mode doesn't care about
          // whether remainder is exactly half
          if (rounding == Rounding.Unnecessary) {
            return this.SignalInvalidWithMessage(ctx, "Rounding was required");
          }
          lastDiscarded = 1;
          olderDiscarded = 1;
        }
      }
      int flags = 0;
      BigInteger newmantissa = mantissa;
      if (shift.isValueZero()) {
        if ((lastDiscarded | olderDiscarded) != 0) {
          flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
          if (rounding == Rounding.Unnecessary) {
            return this.SignalInvalidWithMessage(ctx, "Rounding was required");
          }
          if (
            this.RoundGivenDigits(
              lastDiscarded,
              olderDiscarded,
              rounding,
              neg,
              newmantissa)) {
            newmantissa = newmantissa.add(BigInteger.ONE);
          }
        }
      } else {
        accum = this.helper.CreateShiftAccumulatorWithDigits(
          mantissa,
          lastDiscarded,
          olderDiscarded);
        accum.ShiftRight(shift);
        newmantissa = accum.getShiftedInt();
        if (accum.getDiscardedDigitCount().signum() != 0 ||
            (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) !=
            0) {
          if (mantissa.signum() != 0) {
            flags |= PrecisionContext.FlagRounded;
          }
          if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
            flags |= PrecisionContext.FlagInexact |
              PrecisionContext.FlagRounded;
            if (rounding == Rounding.Unnecessary) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Rounding was required");
            }
          }
          if (this.RoundGivenBigInt(accum, rounding, neg, newmantissa)) {
            newmantissa = newmantissa.add(BigInteger.ONE);
          }
        }
      }
      if (ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (flags));
      }
      return this.helper.CreateNewWithFlags(
        newmantissa,
        desiredExponent,
        neg ? BigNumberFlags.FlagNegative : 0);
    }

    private T DivideInternal(
      T thisValue,
      T divisor,
      PrecisionContext ctx,
      int integerMode,
      BigInteger desiredExponent) {
      T ret = this.DivisionHandleSpecial(thisValue, divisor, ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      int signA = this.helper.GetSign(thisValue);
      int signB = this.helper.GetSign(divisor);
      if (signB == 0) {
        if (signA == 0) {
          return this.SignalInvalid(ctx);
        }
        boolean flagsNeg = ((this.helper.GetFlags(thisValue) &
                          BigNumberFlags.FlagNegative) != 0) ^
          ((this.helper.GetFlags(divisor) &

            BigNumberFlags.FlagNegative) !=
           0);
        return this.SignalDivideByZero(ctx, flagsNeg);
      }
      int radix = this.thisRadix;
      if (signA == 0) {
        T retval = null;
        if (integerMode == IntegerModeFixedScale) {
          int newflags = (this.helper.GetFlags(thisValue) &
                          BigNumberFlags.FlagNegative) ^
            (this.helper.GetFlags(divisor) &
             BigNumberFlags.FlagNegative);
          retval = this.helper.CreateNewWithFlags(
            BigInteger.ZERO,
            desiredExponent,
            newflags);
        } else {
          BigInteger dividendExp = this.helper.GetExponent(thisValue);
          BigInteger divisorExp = this.helper.GetExponent(divisor);
          int newflags = (this.helper.GetFlags(thisValue) &
                          BigNumberFlags.FlagNegative) ^
            (this.helper.GetFlags(divisor) &
             BigNumberFlags.FlagNegative);
          retval =
            this.RoundToPrecision(
              this.helper.CreateNewWithFlags(BigInteger.ZERO, dividendExp.subtract(divisorExp), newflags),
              ctx);
        }
        return retval;
      } else {
        BigInteger mantissaDividend =
          (this.helper.GetMantissa(thisValue)).abs();
        BigInteger mantissaDivisor =
          (this.helper.GetMantissa(divisor)).abs();
        FastInteger expDividend =
          FastInteger.FromBig(this.helper.GetExponent(thisValue));
        FastInteger expDivisor =
          FastInteger.FromBig(this.helper.GetExponent(divisor));
        FastInteger expdiff =
          FastInteger.Copy(expDividend).Subtract(expDivisor);
        FastInteger adjust = new FastInteger(0);
        FastInteger result = new FastInteger(0);
        FastInteger naturalExponent = FastInteger.Copy(expdiff);
        boolean hasPrecision = ctx != null && ctx.getPrecision().signum() != 0;
        boolean resultNeg = (this.helper.GetFlags(thisValue) &
                          BigNumberFlags.FlagNegative) !=
          (this.helper.GetFlags(divisor) &
           BigNumberFlags.FlagNegative);
        FastInteger fastPrecision = (!hasPrecision) ? new FastInteger(0) :
          FastInteger.FromBig(ctx.getPrecision());
        FastInteger dividendPrecision = null;
        FastInteger divisorPrecision = null;
        if (integerMode == IntegerModeFixedScale) {
          FastInteger shift;
          BigInteger rem;
          FastInteger fastDesiredExponent =
            FastInteger.FromBig(desiredExponent);
          if (ctx != null && ctx.getHasFlags() &&
              fastDesiredExponent.compareTo(naturalExponent) > 0) {
            // Treat as rounded if the desired exponent is greater
            // than the "ideal" exponent
            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
          }
          if (expdiff.compareTo(fastDesiredExponent) <= 0) {
            shift = FastInteger.Copy(fastDesiredExponent).Subtract(expdiff);
            BigInteger quo;
{
BigInteger[] divrem = (mantissaDividend).divideAndRemainder(mantissaDivisor);
quo = divrem[0];
rem = divrem[1]; }
            return this.RoundToScale(
              quo,
              rem,
              mantissaDivisor,
              desiredExponent,
              shift,
              resultNeg,
              ctx);
          }
          if (ctx != null && ctx.getPrecision().signum() != 0 &&
              FastInteger.Copy(expdiff).SubtractInt(8)
              .compareTo(fastPrecision) >
              0) {
            // NOTE: 8 guard digits
            // Result would require a too-high precision since
            // exponent difference is much higher
            return this.SignalInvalidWithMessage(
              ctx,
              "Result can't fit the precision");
          } else {
            shift = FastInteger.Copy(expdiff).Subtract(fastDesiredExponent);
            mantissaDividend =
              this.TryMultiplyByRadixPower(mantissaDividend, shift);
            if (mantissaDividend == null) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Result requires too much memory");
            }
            BigInteger quo;
{
BigInteger[] divrem = (mantissaDividend).divideAndRemainder(mantissaDivisor);
quo = divrem[0];
rem = divrem[1]; }
            return this.RoundToScale(
              quo,
              rem,
              mantissaDivisor,
              desiredExponent,
              new FastInteger(0),
              resultNeg,
              ctx);
          }
        }
        if (integerMode == IntegerModeRegular) {
          BigInteger rem = null;
          BigInteger quo = null;
          // System.out.println("div=" +
          // (mantissaDividend.getUnsignedBitLength()) + " divs= " +
          // (mantissaDivisor.getUnsignedBitLength()));
          {
BigInteger[] divrem = (mantissaDividend).divideAndRemainder(mantissaDivisor);
quo = divrem[0];
rem = divrem[1]; }
          if (rem.signum() == 0) {
            // Dividend is divisible by divisor
            if (resultNeg) {
              quo = quo.negate();
            }
            return this.RoundToPrecision(
              this.helper.CreateNewWithFlags(quo, naturalExponent.AsBigInteger(), resultNeg ? BigNumberFlags.FlagNegative : 0),
              ctx);
          }
          rem = null;
          quo = null;
          if (hasPrecision) {
            BigInteger divid = mantissaDividend;
            FastInteger shift = FastInteger.FromBig(ctx.getPrecision());
            dividendPrecision =
              this.helper.CreateShiftAccumulator(mantissaDividend)
              .GetDigitLength();
            divisorPrecision =
              this.helper.CreateShiftAccumulator(mantissaDivisor)
              .GetDigitLength();
            if (dividendPrecision.compareTo(divisorPrecision) <= 0) {
              divisorPrecision.Subtract(dividendPrecision);
              divisorPrecision.Increment();
              shift.Add(divisorPrecision);
              divid = this.TryMultiplyByRadixPower(divid, shift);
              if (divid == null) {
                return this.SignalInvalidWithMessage(
                  ctx,
                  "Result requires too much memory");
              }
            } else {
              // Already greater than divisor precision
              dividendPrecision.Subtract(divisorPrecision);
              if (dividendPrecision.compareTo(shift) <= 0) {
                shift.Subtract(dividendPrecision);
                shift.Increment();
                divid = this.TryMultiplyByRadixPower(divid, shift);
                if (divid == null) {
                  return this.SignalInvalidWithMessage(
                    ctx,
                    "Result requires too much memory");
                }
              } else {
                // no need to shift
                shift.SetInt(0);
              }
            }
            dividendPrecision =
              this.helper.CreateShiftAccumulator(divid).GetDigitLength();
            divisorPrecision =
              this.helper.CreateShiftAccumulator(mantissaDivisor)
              .GetDigitLength();
            if (shift.signum() != 0 || quo == null) {
              // if shift isn't zero, recalculate the quotient
              // and remainder
              {
BigInteger[] divrem = (divid).divideAndRemainder(mantissaDivisor);
quo = divrem[0];
rem = divrem[1]; }
            }
            // System.out.println(String.Format("" + divid + "" +
            // mantissaDivisor + " -> quo= " + quo + " rem= " +
            // (rem)));
            int[] digitStatus = this.RoundToScaleStatus(
              rem,
              mantissaDivisor,
              ctx);
            if (digitStatus == null) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Rounding was required");
            }
            FastInteger natexp =
              FastInteger.Copy(naturalExponent).Subtract(shift);
            PrecisionContext ctxcopy = ctx.WithBlankFlags();
            T retval2 = this.helper.CreateNewWithFlags(
              quo,
              natexp.AsBigInteger(),
              resultNeg ? BigNumberFlags.FlagNegative : 0);
            retval2 = this.RoundToPrecisionWithShift(
              retval2,
              ctxcopy,
              digitStatus[0],
              digitStatus[1],
              null,
              false);
            if ((ctxcopy.getFlags() & PrecisionContext.FlagInexact) != 0) {
              if (ctx.getHasFlags()) {
                ctx.setFlags(ctx.getFlags() | (ctxcopy.getFlags()));
              }
              return retval2;
            }
            if (ctx.getHasFlags()) {
              ctx.setFlags(ctx.getFlags() | (ctxcopy.getFlags()));
              ctx.setFlags(ctx.getFlags() & ~(PrecisionContext.FlagRounded));
            }
            return this.ReduceToPrecisionAndIdealExponent(
              retval2,
              ctx,
              rem.signum() == 0 ? null : fastPrecision,
              expdiff);
          }
        }
        // Rest of method assumes unlimited precision
        // and IntegerModeRegular
        int mantcmp = mantissaDividend.compareTo(mantissaDivisor);
        if (mantcmp < 0) {
          // dividend mantissa is less than divisor mantissa
          dividendPrecision =
            this.helper.CreateShiftAccumulator(mantissaDividend)
            .GetDigitLength();
          divisorPrecision =
            this.helper.CreateShiftAccumulator(mantissaDivisor)
            .GetDigitLength();
          divisorPrecision.Subtract(dividendPrecision);
          if (divisorPrecision.isValueZero()) {
            divisorPrecision.Increment();
          }
          // multiply dividend mantissa so precisions are the same
          // (except if they're already the same, in which case multiply
          // by radix)
          mantissaDividend = this.TryMultiplyByRadixPower(
            mantissaDividend,
            divisorPrecision);
          if (mantissaDividend == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          adjust.Add(divisorPrecision);
          if (mantissaDividend.compareTo(mantissaDivisor) < 0) {
            // dividend mantissa is still less, multiply once more
            if (radix == 2) {
              mantissaDividend = mantissaDividend.shiftLeft(1);
            } else {
              mantissaDividend = mantissaDividend.multiply(BigInteger.valueOf(radix));
            }
            adjust.Increment();
          }
        } else if (mantcmp > 0) {
          // dividend mantissa is greater than divisor mantissa
          dividendPrecision =
            this.helper.CreateShiftAccumulator(mantissaDividend)
            .GetDigitLength();
          divisorPrecision =
            this.helper.CreateShiftAccumulator(mantissaDivisor)
            .GetDigitLength();
          dividendPrecision.Subtract(divisorPrecision);
          BigInteger oldMantissaB = mantissaDivisor;
          mantissaDivisor = this.TryMultiplyByRadixPower(
            mantissaDivisor,
            dividendPrecision);
          if (mantissaDivisor == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          adjust.Subtract(dividendPrecision);
          if (mantissaDividend.compareTo(mantissaDivisor) < 0) {
            // dividend mantissa is now less, divide by radix power
            if (dividendPrecision.CompareToInt(1) == 0) {
              // no need to divide here, since that would just undo
              // the multiplication
              mantissaDivisor = oldMantissaB;
            } else {
              BigInteger bigpow = BigInteger.valueOf(radix);
              mantissaDivisor = mantissaDivisor.divide(bigpow);
            }
            adjust.Increment();
          }
        }
        if (mantcmp == 0) {
          result = new FastInteger(1);
          mantissaDividend = BigInteger.ZERO;
        } else {
          {
            if (!this.helper.HasTerminatingRadixExpansion(
              mantissaDividend,
              mantissaDivisor)) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Result would have a nonterminating expansion");
            }
            FastInteger divs = FastInteger.FromBig(mantissaDivisor);
            FastInteger divd = FastInteger.FromBig(mantissaDividend);
            boolean divisorFits = divs.CanFitInInt32();
            int smallDivisor = divisorFits ? divs.AsInt32() : 0;
            int halfRadix = radix / 2;
            FastInteger divsHalfRadix = null;
            if (radix != 2) {
              divsHalfRadix =
                FastInteger.FromBig(mantissaDivisor).Multiply(halfRadix);
            }
            while (true) {
              boolean remainderZero = false;
              int count = 0;
              if (divd.CanFitInInt32()) {
                if (divisorFits) {
                  int smallDividend = divd.AsInt32();
                  count = smallDividend / smallDivisor;
                  divd.SetInt(smallDividend % smallDivisor);
                } else {
                  count = 0;
                }
              } else {
                if (divsHalfRadix != null) {
                  count += halfRadix * divd.RepeatedSubtract(divsHalfRadix);
                }
                count += divd.RepeatedSubtract(divs);
              }
              result.AddInt(count);
              remainderZero = divd.isValueZero();
              if (remainderZero && adjust.signum() >= 0) {
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
        Rounding rounding = (ctx == null) ? Rounding.HalfEven : ctx.getRounding();
        int lastDiscarded = 0;
        int olderDiscarded = 0;
        if (mantissaDividend.signum() != 0) {
          if (rounding == Rounding.HalfDown || rounding == Rounding.HalfEven ||
              rounding == Rounding.HalfUp) {
            BigInteger halfDivisor = mantissaDivisor.shiftRight(1);
            int cmpHalf = mantissaDividend.compareTo(halfDivisor);
            if ((cmpHalf == 0) && mantissaDivisor.testBit(0) == false) {
              // remainder is exactly half
              lastDiscarded = radix / 2;
              olderDiscarded = 0;
            } else if (cmpHalf > 0) {
              // remainder is greater than half
              lastDiscarded = radix / 2;
              olderDiscarded = 1;
            } else {
              // remainder is less than half
              lastDiscarded = 0;
              olderDiscarded = 1;
            }
          } else {
            if (rounding == Rounding.Unnecessary) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Rounding was required");
            }
            lastDiscarded = 1;
            olderDiscarded = 1;
          }
        }
        BigInteger bigResult = result.AsBigInteger();
        if (ctx != null && ctx.getHasFlags() && exp.compareTo(expdiff) > 0) {
          // Treat as rounded if the true exponent is greater
          // than the "ideal" exponent
          ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
        }
        BigInteger bigexp = exp.AsBigInteger();
        T retval = this.helper.CreateNewWithFlags(
          bigResult,
          bigexp,
          resultNeg ? BigNumberFlags.FlagNegative : 0);
        return this.RoundToPrecisionWithShift(
          retval,
          ctx,
          lastDiscarded,
          olderDiscarded,
          null,
          false);
      }
    }

    /**
     * Gets the lesser value between two values, ignoring their signs. If the
     * absolute values are equal, has the same effect as Min.
     * @param a A T object. (2).
     * @param b A T object. (3).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     * @throws NullPointerException The parameter {@code a} or {@code b} is null.
     */
    public T MinMagnitude(T a, T b, PrecisionContext ctx) {
      if (a == null) {
        throw new NullPointerException("a");
      }
      if (b == null) {
        throw new NullPointerException("b");
      }
      // Handle infinity and NaN
      T result = this.MinMaxHandleSpecial(a, b, ctx, true, true);
      if ((Object)result != (Object)null) {
        return result;
      }
      int cmp = this.compareTo(this.AbsRaw(a), this.AbsRaw(b));
      return (cmp == 0) ? this.Min(a, b, ctx) : ((cmp < 0) ?
                                                 this.RoundToPrecision(
                                                   a,
                                                   ctx) : this.RoundToPrecision(
                                                   b,
                                                   ctx));
    }

    /**
     * Gets the greater value between two values, ignoring their signs. If the
     * absolute values are equal, has the same effect as Max.
     * @param a A T object. (2).
     * @param b A T object. (3).
     * @param ctx A PrecisionContext object.
     * @return A T object.
     * @throws NullPointerException The parameter {@code a} or {@code b} is null.
     */
    public T MaxMagnitude(T a, T b, PrecisionContext ctx) {
      if (a == null) {
        throw new NullPointerException("a");
      }
      if (b == null) {
        throw new NullPointerException("b");
      }
      // Handle infinity and NaN
      T result = this.MinMaxHandleSpecial(a, b, ctx, false, true);
      if ((Object)result != (Object)null) {
        return result;
      }
      int cmp = this.compareTo(this.AbsRaw(a), this.AbsRaw(b));
      return (cmp == 0) ? this.Max(a, b, ctx) : ((cmp > 0) ?
                                                 this.RoundToPrecision(
                                                   a,
                                                   ctx) : this.RoundToPrecision(
                                                   b,
                                                   ctx));
    }

    /**
     * Gets the greater value between two T values.
     * @param a A T object.
     * @param b A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return The larger value of the two objects.
     * @throws NullPointerException The parameter {@code a} or {@code b} is null.
     */
    public T Max(T a, T b, PrecisionContext ctx) {
      if (a == null) {
        throw new NullPointerException("a");
      }
      if (b == null) {
        throw new NullPointerException("b");
      }
      // Handle infinity and NaN
      T result = this.MinMaxHandleSpecial(a, b, ctx, false, false);
      if ((Object)result != (Object)null) {
        return result;
      }
      int cmp = this.compareTo(a, b);
      if (cmp != 0) {
        return cmp < 0 ? this.RoundToPrecision(b, ctx) :
          this.RoundToPrecision(a, ctx);
      }
      int flagNegA = this.helper.GetFlags(a) & BigNumberFlags.FlagNegative;
      return (flagNegA != (this.helper.GetFlags(b) &
                           BigNumberFlags.FlagNegative)) ? ((flagNegA != 0) ?
                                       this.RoundToPrecision(b, ctx) :
                                       this.RoundToPrecision(a, ctx)) :
        ((flagNegA == 0) ?
         (this.helper.GetExponent(a).compareTo(this.helper.GetExponent(b)) > 0 ?
          this.RoundToPrecision(a, ctx) : this.RoundToPrecision(b, ctx)) :
         (this.helper.GetExponent(a).compareTo(this.helper.GetExponent(b)) > 0 ?
          this.RoundToPrecision(b, ctx) : this.RoundToPrecision(a, ctx)));
    }

    /**
     * Gets the lesser value between two T values.
     * @param a A T object.
     * @param b A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return The smaller value of the two objects.
     * @throws NullPointerException The parameter {@code a} or {@code b} is null.
     */
    public T Min(T a, T b, PrecisionContext ctx) {
      if (a == null) {
        throw new NullPointerException("a");
      }
      if (b == null) {
        throw new NullPointerException("b");
      }
      // Handle infinity and NaN
      T result = this.MinMaxHandleSpecial(a, b, ctx, true, false);
      if ((Object)result != (Object)null) {
        return result;
      }
      int cmp = this.compareTo(a, b);
      if (cmp != 0) {
        return cmp > 0 ? this.RoundToPrecision(b, ctx) :
          this.RoundToPrecision(a, ctx);
      }
      int signANeg = this.helper.GetFlags(a) & BigNumberFlags.FlagNegative;
      return (signANeg != (this.helper.GetFlags(b) &
                           BigNumberFlags.FlagNegative)) ? ((signANeg != 0) ?
                                       this.RoundToPrecision(a, ctx) :
                                       this.RoundToPrecision(b, ctx)) :
        ((signANeg == 0) ?
         (this.helper.GetExponent(a).compareTo(this.helper.GetExponent(b)) > 0 ?
          this.RoundToPrecision(b, ctx) : this.RoundToPrecision(a, ctx)) :
         (this.helper.GetExponent(a).compareTo(this.helper.GetExponent(b)) > 0 ?
          this.RoundToPrecision(a, ctx) : this.RoundToPrecision(b, ctx)));
    }

    /**
     * Multiplies two T objects.
     * @param thisValue A T object.
     * @param other A T object. (2).
     * @param ctx A PrecisionContext object.
     * @return The product of the two objects.
     */
    public T Multiply(T thisValue, T other, PrecisionContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = this.HandleNotANumber(thisValue, other, ctx);
        if ((Object)result != (Object)null) {
          return result;
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          // Attempt to multiply infinity by 0
          boolean negflag = ((thisFlags & BigNumberFlags.FlagNegative) != 0) ^
            ((otherFlags & BigNumberFlags.FlagNegative) != 0);
          return ((otherFlags & BigNumberFlags.FlagSpecial) == 0 &&
                  this.helper.GetMantissa(other).signum() == 0) ?
            this.SignalInvalid(ctx) :
            this.EnsureSign(
              thisValue,
              negflag);
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          // Attempt to multiply infinity by 0
          boolean negflag = ((thisFlags & BigNumberFlags.FlagNegative) != 0) ^
            ((otherFlags & BigNumberFlags.FlagNegative) != 0);
          return ((thisFlags & BigNumberFlags.FlagSpecial) == 0 &&
                  this.helper.GetMantissa(thisValue).signum() == 0) ?
            this.SignalInvalid(
              ctx) : this.EnsureSign(other, negflag);
        }
      }
      BigInteger bigintOp2 = this.helper.GetExponent(other);
      BigInteger newexp = this.helper.GetExponent(thisValue).add(bigintOp2);
      BigInteger mantissaOp2 = this.helper.GetMantissa(other);
      // System.out.println("" + (this.helper.GetMantissa(thisValue)) + "," +
      // (this.helper.GetExponent(thisValue)) + " -> " + mantissaOp2 +", " +
      // (bigintOp2));
      thisFlags = (thisFlags & BigNumberFlags.FlagNegative) ^ (otherFlags &
  BigNumberFlags.FlagNegative);
      T ret =
        this.helper.CreateNewWithFlags(
          this.helper.GetMantissa(thisValue).multiply(mantissaOp2),
          newexp,
          thisFlags);
      if (ctx != null) {
        ret = this.RoundToPrecision(ret, ctx);
      }
      return ret;
    }

    public T MultiplyAndAdd(
      T thisValue,
      T multiplicand,
      T augend,
      PrecisionContext ctx) {
      PrecisionContext ctx2 = PrecisionContext.Unlimited.WithBlankFlags();
      T ret = this.MultiplyAddHandleSpecial(
        thisValue,
        multiplicand,
        augend,
        ctx);
      if ((Object)ret != (Object)null) {
        return ret;
      }
      ret = this.Add(this.Multiply(thisValue, multiplicand, ctx2), augend, ctx);
      if (ctx != null && ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (ctx2.getFlags()));
      }
      return ret;
    }

    public T Plus(T thisValue, PrecisionContext context) {
      return this.RoundToPrecisionInternal(
        thisValue,
        0,
        0,
        null,
        true,
        context);
    }

    public T RoundToPrecision(T thisValue, PrecisionContext context) {
      return this.RoundToPrecisionInternal(
        thisValue,
        0,
        0,
        null,
        false,
        context);
    }

    private T RoundToPrecisionWithShift(
      T thisValue,
      PrecisionContext context,
      int lastDiscarded,
      int olderDiscarded,
      FastInteger shift,
      boolean adjustNegativeZero) {
      return this.RoundToPrecisionInternal(
        thisValue,
        lastDiscarded,
        olderDiscarded,
        shift,
        adjustNegativeZero,
        context);
    }

    public T Quantize(T thisValue, T otherValue, PrecisionContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(otherValue);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = this.HandleNotANumber(thisValue, otherValue, ctx);
        if ((Object)result != (Object)null) {
          return result;
        }
        if (((thisFlags & otherFlags) & BigNumberFlags.FlagInfinity) != 0) {
          return this.RoundToPrecision(thisValue, ctx);
        }
        // At this point, it's only the case that either value
        // is infinity
        return this.SignalInvalid(ctx);
      }
      BigInteger expOther = this.helper.GetExponent(otherValue);
      if (ctx != null && !ctx.ExponentWithinRange(expOther)) {
        // System.out.println("exp not within range");
        return this.SignalInvalidWithMessage(
          ctx,
          "Exponent not within exponent range: " + expOther);
      }
      PrecisionContext tmpctx = (ctx == null ?
  PrecisionContext.ForRounding(Rounding.HalfEven) :
                                 ctx.Copy()).WithBlankFlags();
      BigInteger mantThis = (this.helper.GetMantissa(thisValue)).abs();
      BigInteger expThis = this.helper.GetExponent(thisValue);
      int expcmp = expThis.compareTo(expOther);
      int negativeFlag = this.helper.GetFlags(thisValue) &
        BigNumberFlags.FlagNegative;
      T ret = null;
      if (expcmp == 0) {
        // System.out.println("exp same");
        ret = this.RoundToPrecision(thisValue, tmpctx);
      } else if (mantThis.signum() == 0) {
        // System.out.println("mant is 0");
        ret = this.helper.CreateNewWithFlags(
          BigInteger.ZERO,
          expOther,
          negativeFlag);
        ret = this.RoundToPrecision(ret, tmpctx);
      } else if (expcmp > 0) {
        // Other exponent is less
        // System.out.println("other exp less");
        FastInteger radixPower =
          FastInteger.FromBig(expThis).SubtractBig(expOther);
        if (tmpctx.getPrecision().signum() > 0 &&
            radixPower.compareTo(FastInteger.FromBig(tmpctx.getPrecision())
                                 .AddInt(10)) >
            0) {
          // Radix power is much too high for the current precision
          // System.out.println("result too high for prec:" +
          // tmpctx.getPrecision() + " radixPower= " + radixPower);
          return this.SignalInvalidWithMessage(
            ctx,
            "Result too high for current precision");
        }
        mantThis = this.TryMultiplyByRadixPower(mantThis, radixPower);
        if (mantThis == null) {
          return this.SignalInvalidWithMessage(
            ctx,
            "Result requires too much memory");
        }
        ret = this.helper.CreateNewWithFlags(mantThis, expOther, negativeFlag);
        ret = this.RoundToPrecision(ret, tmpctx);
      } else {
        // Other exponent is greater
        // System.out.println("other exp greater");
        FastInteger shift = FastInteger.FromBig(expOther).SubtractBig(expThis);
        ret = this.RoundToPrecisionWithShift(
          thisValue,
          tmpctx,
          0,
          0,
          shift,
          false);
      }
      if ((tmpctx.getFlags() & PrecisionContext.FlagOverflow) != 0) {
        // System.out.println("overflow occurred");
        return this.SignalInvalid(ctx);
      }
      if (ret == null || !this.helper.GetExponent(ret).equals(expOther)) {
        // System.out.println("exp not same "+ret);
        return this.SignalInvalid(ctx);
      }
      ret = this.EnsureSign(ret, negativeFlag != 0);
      if (ctx != null && ctx.getHasFlags()) {
        int flags = tmpctx.getFlags();
        flags &= ~PrecisionContext.FlagUnderflow;
        ctx.setFlags(ctx.getFlags() | (flags));
      }
      return ret;
    }

    public T RoundToExponentExact(
      T thisValue,
      BigInteger expOther,
      PrecisionContext ctx) {
      if (this.helper.GetExponent(thisValue).compareTo(expOther) >= 0) {
        return this.RoundToPrecision(thisValue, ctx);
      } else {
        PrecisionContext pctx = (ctx == null) ? null :
          ctx.WithPrecision(0).WithBlankFlags();
        T ret = this.Quantize(
          thisValue,
          this.helper.CreateNewWithFlags(BigInteger.ONE, expOther, 0),
          pctx);
        if (ctx != null && ctx.getHasFlags()) {
          ctx.setFlags(ctx.getFlags() | (pctx.getFlags()));
        }
        return ret;
      }
    }

    public T RoundToExponentSimple(
      T thisValue,
      BigInteger expOther,
      PrecisionContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      if ((thisFlags & BigNumberFlags.FlagSpecial) != 0) {
        T result = this.HandleNotANumber(thisValue, thisValue, ctx);
        if ((Object)result != (Object)null) {
          return result;
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          return thisValue;
        }
      }
      if (this.helper.GetExponent(thisValue).compareTo(expOther) >= 0) {
        return this.RoundToPrecision(thisValue, ctx);
      } else {
        if (ctx != null && !ctx.ExponentWithinRange(expOther)) {
          return this.SignalInvalidWithMessage(
            ctx,
            "Exponent not within exponent range: " + expOther);
        }
        BigInteger bigmantissa =
          (this.helper.GetMantissa(thisValue)).abs();
        FastInteger shift =
          FastInteger.FromBig(expOther)
          .SubtractBig(this.helper.GetExponent(thisValue));
        IShiftAccumulator accum =
          this.helper.CreateShiftAccumulator(bigmantissa);
        accum.ShiftRight(shift);
        bigmantissa = accum.getShiftedInt();
        thisValue = this.helper.CreateNewWithFlags(
          bigmantissa,
          expOther,
          thisFlags);
        return this.RoundToPrecisionWithShift(
          thisValue,
          ctx,
          accum.getLastDiscardedDigit(),
          accum.getOlderDiscardedDigits(),
          null,
          false);
      }
    }

    public T RoundToExponentNoRoundedFlag(
      T thisValue,
      BigInteger exponent,
      PrecisionContext ctx) {
      PrecisionContext pctx = (ctx == null) ? null : ctx.WithBlankFlags();
      T ret = this.RoundToExponentExact(thisValue, exponent, pctx);
      if (ctx != null && ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (pctx.getFlags() & ~(PrecisionContext.FlagInexact |
                                    PrecisionContext.FlagRounded)));
      }
      return ret;
    }

    private T ReduceToPrecisionAndIdealExponent(
      T thisValue,
      PrecisionContext ctx,
      FastInteger precision,
      FastInteger idealExp) {
      T ret = this.RoundToPrecision(thisValue, ctx);
      if (ret != null && (this.helper.GetFlags(ret) &
                          BigNumberFlags.FlagSpecial) == 0) {
        BigInteger bigmant = (this.helper.GetMantissa(ret)).abs();
        FastInteger exp = FastInteger.FromBig(this.helper.GetExponent(ret));
        int radix = this.thisRadix;
        if (bigmant.signum() == 0) {
          exp = new FastInteger(0);
        } else {
          FastInteger digits = (precision == null) ? null :
            this.helper.CreateShiftAccumulator(bigmant).GetDigitLength();
          bigmant = DecimalUtility.ReduceTrailingZeros(
            bigmant,
            exp,
            radix,
            digits,
            precision,
            idealExp);
        }
        int flags = this.helper.GetFlags(thisValue);
        ret = this.helper.CreateNewWithFlags(
          bigmant,
          exp.AsBigInteger(),
          flags);
        if (ctx != null && ctx.getClampNormalExponents()) {
          PrecisionContext ctxtmp = ctx.WithBlankFlags();
          ret = this.RoundToPrecision(ret, ctxtmp);
          if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (ctxtmp.getFlags() & ~PrecisionContext.FlagClamped));
          }
        }
        ret = this.EnsureSign(ret, (flags & BigNumberFlags.FlagNegative) != 0);
      }
      return ret;
    }

    public T Reduce(T thisValue, PrecisionContext ctx) {
      return this.ReduceToPrecisionAndIdealExponent(thisValue, ctx, null, null);
    }
    // binaryPrec means whether precision is the number of bits and not
    // digits
    private T RoundToPrecisionInternal(
      T thisValue,
      int lastDiscarded,
      int olderDiscarded,
      FastInteger shift,
      boolean adjustNegativeZero,
      PrecisionContext ctx) {
      ctx = (ctx == null) ? (PrecisionContext.Unlimited.WithRounding(Rounding.HalfEven)) : ctx;
      // If context has unlimited precision and exponent range,
      // and no discarded digits or shifting
      if (!ctx.getHasMaxPrecision() && !ctx.getHasExponentRange() && (lastDiscarded |
                                                            olderDiscarded) ==
          0 && (shift ==
                   null ||
                   shift.isValueZero())) {
        return thisValue;
      }
      boolean binaryPrec = ctx.isPrecisionInBits();
      int thisFlags = this.helper.GetFlags(thisValue);
      if ((thisFlags & BigNumberFlags.FlagSpecial) != 0) {
        if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
          if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInvalid));
          }
          return this.ReturnQuietNaN(thisValue, ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
          return this.ReturnQuietNaN(thisValue, ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          return thisValue;
        }
      }
      // get the precision
      FastInteger fastPrecision = ctx.getPrecision().canFitInInt() ? new
        FastInteger(ctx.getPrecision().intValue()) :
        FastInteger.FromBig(ctx.getPrecision());
      // No need to check if precision is less than 0, since the
      // PrecisionContext Object should already ensure this

      binaryPrec &= this.thisRadix != 2 && !fastPrecision.isValueZero();
      IShiftAccumulator accum = null;
      FastInteger fastEMin = null;
      FastInteger fastEMax = null;
      // get the exponent range
      if (ctx != null && ctx.getHasExponentRange()) {
        fastEMax = ctx.getEMax().canFitInInt() ? new
          FastInteger(ctx.getEMax().intValue()) : FastInteger.FromBig(ctx.getEMax());
        fastEMin = ctx.getEMin().canFitInInt() ? new
          FastInteger(ctx.getEMin().intValue()) : FastInteger.FromBig(ctx.getEMin());
      }
      Rounding rounding = (ctx == null) ? Rounding.HalfEven : ctx.getRounding();
      boolean unlimitedPrec = fastPrecision.isValueZero();
      if (!binaryPrec) {
        // Fast path to check if rounding is necessary at all
        if (fastPrecision.signum() > 0 &&
            (shift == null || shift.isValueZero()) &&
            (thisFlags & BigNumberFlags.FlagSpecial) == 0) {
          BigInteger mantabs =
            (this.helper.GetMantissa(thisValue)).abs();
          if (adjustNegativeZero && (thisFlags & BigNumberFlags.FlagNegative) !=
              0 &&

              mantabs.signum() == 0 &&
              (ctx.getRounding() != Rounding.Floor)) {
            // Change negative zero to positive zero
            // except if the rounding mode is Floor
            thisValue = this.EnsureSign(thisValue, false);
            thisFlags = 0;
          }
          accum = this.helper.CreateShiftAccumulatorWithDigits(
            mantabs,
            lastDiscarded,
            olderDiscarded);
          FastInteger digitCount = accum.GetDigitLength();
          if (digitCount.compareTo(fastPrecision) <= 0) {
            if (!this.RoundGivenDigits(
              lastDiscarded,
              olderDiscarded,
              ctx.getRounding(),
              (thisFlags & BigNumberFlags.FlagNegative) != 0,
              mantabs)) {
              if (ctx.getHasFlags() && (lastDiscarded | olderDiscarded) != 0) {
                ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact |
                  PrecisionContext.FlagRounded));
              }
              if (!ctx.getHasExponentRange()) {
                return thisValue;
              }
              BigInteger bigexp = this.helper.GetExponent(thisValue);
              FastInteger fastExp = bigexp.canFitInInt() ? new
                FastInteger(bigexp.intValue()) : FastInteger.FromBig(bigexp);
              FastInteger fastAdjustedExp = FastInteger.Copy(fastExp);
              FastInteger fastNormalMin = FastInteger.Copy(fastEMin);
              if (ctx == null || ctx.getAdjustExponent()) {
                fastAdjustedExp.Add(fastPrecision).Decrement();
                fastNormalMin.Add(fastPrecision).Decrement();
              }
              if (fastAdjustedExp.compareTo(fastEMax) <= 0 &&
                  fastAdjustedExp.compareTo(fastNormalMin) >= 0) {
                return thisValue;
              }
            } else {
              if (ctx.getHasFlags() && (lastDiscarded | olderDiscarded) != 0) {
                ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact |
                  PrecisionContext.FlagRounded));
              }
              boolean stillWithinPrecision = false;
              mantabs = mantabs.add(BigInteger.ONE);
              if (digitCount.compareTo(fastPrecision) < 0) {
                stillWithinPrecision = true;
              } else {
                BigInteger radixPower =
                  this.TryMultiplyByRadixPower(BigInteger.ONE, fastPrecision);
                if (radixPower == null) {
                  return this.SignalInvalidWithMessage(
                    ctx,
                    "Result requires too much memory");
                }
                stillWithinPrecision = mantabs.compareTo(radixPower) < 0;
              }
              if (stillWithinPrecision) {
                BigInteger bigexp = this.helper.GetExponent(thisValue);
                if (!ctx.getHasExponentRange()) {
                  return this.helper.CreateNewWithFlags(
                    mantabs,
                    bigexp,
                    thisFlags);
                }
                FastInteger fastExp = bigexp.canFitInInt() ? new
                  FastInteger(bigexp.intValue()) :
                  FastInteger.FromBig(bigexp);
                FastInteger fastAdjustedExp = FastInteger.Copy(fastExp);
                FastInteger fastNormalMin = FastInteger.Copy(fastEMin);
                if (ctx == null || ctx.getAdjustExponent()) {
                  fastAdjustedExp.Add(fastPrecision).Decrement();
                  fastNormalMin.Add(fastPrecision).Decrement();
                }
                if (fastAdjustedExp.compareTo(fastEMax) <= 0 &&
                    fastAdjustedExp.compareTo(fastNormalMin) >= 0) {
                  return this.helper.CreateNewWithFlags(
                    mantabs,
                    bigexp,
                    thisFlags);
                }
              }
            }
          }
        }
      }
      if (adjustNegativeZero && (thisFlags & BigNumberFlags.FlagNegative) !=
          0 && this.helper.GetMantissa(thisValue).signum() == 0 && (rounding !=
                                                             Rounding.Floor)) {
        // Change negative zero to positive zero
        // except if the rounding mode is Floor
        thisValue = this.EnsureSign(thisValue, false);
        thisFlags = 0;
      }
      boolean neg = (thisFlags & BigNumberFlags.FlagNegative) != 0;
      BigInteger bigmantissa =
        (this.helper.GetMantissa(thisValue)).abs();
      // save mantissa in case result is subnormal
      // and must be rounded again
      BigInteger oldmantissa = bigmantissa;
      boolean mantissaWasZero = oldmantissa.signum() == 0 && (lastDiscarded |
                                                    olderDiscarded) == 0;
      BigInteger maxMantissa = BigInteger.ONE;
      FastInteger exp = FastInteger.FromBig(this.helper.GetExponent(thisValue));
      int flags = 0;
      accum = (accum == null) ? (this.helper.CreateShiftAccumulatorWithDigits(
          bigmantissa,
          lastDiscarded,
          olderDiscarded)) : accum;
      if (binaryPrec) {
        FastInteger prec = FastInteger.Copy(fastPrecision);
        while (prec.signum() > 0) {
          int bitShift = (prec.CompareToInt(1000000) >= 0) ? 1000000 :
            prec.AsInt32();
          maxMantissa = maxMantissa.shiftLeft(bitShift);
          prec.SubtractInt(bitShift);
        }
        maxMantissa = maxMantissa.subtract(BigInteger.ONE);
        IShiftAccumulator accumMaxMant =
          this.helper.CreateShiftAccumulator(maxMantissa);
        // Get the digit length of the maximum possible mantissa
        // for the given binary precision, and use that for
        // fastPrecision
        fastPrecision = accumMaxMant.GetDigitLength();
      }
      if (shift != null && shift.signum() != 0) {
        accum.ShiftRight(shift);
      }
      if (!unlimitedPrec) {
        accum.ShiftToDigits(fastPrecision);
      } else {
        fastPrecision = accum.GetDigitLength();
      }
      if (binaryPrec) {
        while (accum.getShiftedInt().compareTo(maxMantissa) > 0) {
          accum.ShiftRightInt(1);
        }
      }
      FastInteger discardedBits = FastInteger.Copy(accum.getDiscardedDigitCount());
      exp.Add(discardedBits);
      FastInteger adjExponent = FastInteger.Copy(exp);
      if (ctx.getAdjustExponent()) {
        adjExponent = adjExponent.Add(accum.GetDigitLength()).Decrement();
      }
      // System.out.println("" + bigmantissa + "-> " + accum.getShiftedInt()
      // +" digits= " + accum.getDiscardedDigitCount() + " exp= " + exp
      // +" [curexp= " + (helper.GetExponent(thisValue)) + "] adj=" +
      // adjExponent + ",max= " + (fastEMax));
      FastInteger newAdjExponent = adjExponent;
      FastInteger clamp = null;
      BigInteger earlyRounded = BigInteger.ZERO;
      if (binaryPrec && fastEMax != null && adjExponent.compareTo(fastEMax) ==
          0) {
        // May or may not be an overflow depending on the mantissa
        FastInteger expdiff =
          FastInteger.Copy(fastPrecision).Subtract(accum.GetDigitLength());
        BigInteger currMantissa = accum.getShiftedInt();
        currMantissa = this.TryMultiplyByRadixPower(currMantissa, expdiff);
        if (currMantissa == null) {
          return this.SignalInvalidWithMessage(
            ctx,
            "Result requires too much memory");
        }
        if (currMantissa.compareTo(maxMantissa) > 0) {
          // Mantissa too high, treat as overflow
          adjExponent.Increment();
        }
      }
      if (ctx.getHasFlags() && fastEMin != null &&
          adjExponent.compareTo(fastEMin) < 0) {
        earlyRounded = accum.getShiftedInt();
        if (this.RoundGivenBigInt(accum, rounding, neg, earlyRounded)) {
          earlyRounded = earlyRounded.add(BigInteger.ONE);
          if (!unlimitedPrec && (earlyRounded.testBit(0) == false || (this.thisRadix & 1) !=
                                 0)) {
            IShiftAccumulator accum2 =
              this.helper.CreateShiftAccumulator(earlyRounded);
            FastInteger newDigitLength = accum2.GetDigitLength();
            // Ensure newDigitLength doesn't exceed precision
            if (binaryPrec || newDigitLength.compareTo(fastPrecision) > 0) {
              newDigitLength = FastInteger.Copy(fastPrecision);
            }
            newAdjExponent = FastInteger.Copy(exp);
            if (ctx.getAdjustExponent()) {
              newAdjExponent = newAdjExponent.Add(newDigitLength).Decrement();
            }
          }
        }
      }
      if (fastEMax != null && adjExponent.compareTo(fastEMax) > 0) {
        if (mantissaWasZero) {
          if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (flags | PrecisionContext.FlagClamped));
          }
          if (ctx.getClampNormalExponents()) {
            // Clamp exponents to eMax + 1 - precision
            // if directed
            if (binaryPrec && this.thisRadix != 2) {
              fastPrecision =
                this.helper.CreateShiftAccumulator(maxMantissa)
                .GetDigitLength();
            }
            FastInteger clampExp = FastInteger.Copy(fastEMax);
            if (ctx.getAdjustExponent()) {
              clampExp.Increment().Subtract(fastPrecision);
            }
            if (fastEMax.compareTo(clampExp) > 0) {
              if (ctx.getHasFlags()) {
                ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagClamped));
              }
              fastEMax = clampExp;
            }
          }
          return this.helper.CreateNewWithFlags(
            oldmantissa,
            fastEMax.AsBigInteger(),
            thisFlags);
        }
        // Overflow
        flags |= PrecisionContext.FlagOverflow |
          PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
        if (rounding == Rounding.Unnecessary) {
          return this.SignalInvalidWithMessage(ctx, "Rounding was required");
        }
        if (!unlimitedPrec && (rounding == Rounding.Down || rounding ==
                           Rounding.ZeroFiveUp || (rounding ==
                                 Rounding.Ceiling &&

                                                       neg) ||

                               (rounding == Rounding.Floor && !neg))) {
          // Set to the highest possible value for
          // the given precision
          BigInteger overflowMant = BigInteger.ZERO;
          if (binaryPrec) {
            overflowMant = maxMantissa;
          } else {
            overflowMant = this.TryMultiplyByRadixPower(
              BigInteger.ONE,
              fastPrecision);
            if (overflowMant == null) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Result requires too much memory");
            }
            overflowMant = overflowMant.subtract(BigInteger.ONE);
          }
          if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (flags));
          }
          clamp = FastInteger.Copy(fastEMax);
          if (ctx.getAdjustExponent()) {
            clamp.Increment().Subtract(fastPrecision);
          }
          return this.helper.CreateNewWithFlags(
            overflowMant,
            clamp.AsBigInteger(),
            neg ? BigNumberFlags.FlagNegative : 0);
        }
        if (ctx.getHasFlags()) {
          ctx.setFlags(ctx.getFlags() | (flags));
        }
        return this.SignalOverflow(neg);
      }
      if (fastEMin != null && adjExponent.compareTo(fastEMin) < 0) {
        // Subnormal
        FastInteger fastETiny = FastInteger.Copy(fastEMin);
        if (ctx.getAdjustExponent()) {
          fastETiny = fastETiny.Subtract(fastPrecision).Increment();
        }
        if (ctx.getHasFlags()) {
          if (earlyRounded.signum() != 0) {
            if (newAdjExponent.compareTo(fastEMin) < 0) {
              flags |= PrecisionContext.FlagSubnormal;
            }
          }
        }
        // System.out.println("exp=" + exp + " eTiny=" + (fastETiny));
        FastInteger subExp = FastInteger.Copy(exp);
        // System.out.println("exp=" + subExp + " eTiny=" + (fastETiny));
        if (subExp.compareTo(fastETiny) < 0) {
          // System.out.println("Less than ETiny");
          FastInteger expdiff = FastInteger.Copy(fastETiny).Subtract(exp);
          expdiff.Add(discardedBits);
          accum = this.helper.CreateShiftAccumulatorWithDigits(
            oldmantissa,
            lastDiscarded,
            olderDiscarded);
          accum.ShiftRight(expdiff);
          FastInteger newmantissa = accum.getShiftedIntFast();
          if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
            if (rounding == Rounding.Unnecessary) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Rounding was required");
            }
          }
          if (accum.getDiscardedDigitCount().signum() != 0 ||
              (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
            if (ctx.getHasFlags()) {
              if (!mantissaWasZero) {
                flags |= PrecisionContext.FlagRounded;
              }
              if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) !=
                  0) {
                flags |= PrecisionContext.FlagInexact |
                  PrecisionContext.FlagRounded;
              }
            }
            if (this.Round(accum, rounding, neg, newmantissa)) {
              newmantissa.Increment();
            }
          }
          if (ctx.getHasFlags()) {
            if (newmantissa.isValueZero()) {
              flags |= PrecisionContext.FlagClamped;
            }
            if ((flags & (PrecisionContext.FlagSubnormal |
                          PrecisionContext.FlagInexact)) ==
                (PrecisionContext.FlagSubnormal |
                 PrecisionContext.FlagInexact)) {
              flags |= PrecisionContext.FlagUnderflow |
                PrecisionContext.FlagRounded;
            }
            ctx.setFlags(ctx.getFlags() | (flags));
          }
          bigmantissa = newmantissa.AsBigInteger();
          if (ctx.getClampNormalExponents()) {
            // Clamp exponents to eMax + 1 - precision
            // if directed
            if (binaryPrec && this.thisRadix != 2) {
              fastPrecision =
                this.helper.CreateShiftAccumulator(maxMantissa)
                .GetDigitLength();
            }
            FastInteger clampExp = FastInteger.Copy(fastEMax);
            if (ctx.getAdjustExponent()) {
              clampExp.Increment().Subtract(fastPrecision);
            }
            if (fastETiny.compareTo(clampExp) > 0) {
              if (bigmantissa.signum() != 0) {
                expdiff = FastInteger.Copy(fastETiny).Subtract(clampExp);
                // Change bigmantissa for use
                // in the return value
                bigmantissa = this.TryMultiplyByRadixPower(
                  bigmantissa,
                  expdiff);
                if (bigmantissa == null) {
                  return this.SignalInvalidWithMessage(
                    ctx,
                    "Result requires too much memory");
                }
              }
              if (ctx.getHasFlags()) {
                ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagClamped));
              }
              fastETiny = clampExp;
            }
          }
          return this.helper.CreateNewWithFlags(
            bigmantissa,
            fastETiny.AsBigInteger(),
            neg ? BigNumberFlags.FlagNegative : 0);
        }
      }
      boolean recheckOverflow = false;
      if (accum.getDiscardedDigitCount().signum() != 0 || (accum.getLastDiscardedDigit() |
                                                  accum.getOlderDiscardedDigits()) !=
          0) {
        if (bigmantissa.signum() != 0) {
          flags |= PrecisionContext.FlagRounded;
        }
        bigmantissa = accum.getShiftedInt();
        if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
          flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
          if (rounding == Rounding.Unnecessary) {
            return this.SignalInvalidWithMessage(ctx, "Rounding was required");
          }
        }
        if (this.RoundGivenBigInt(accum, rounding, neg, bigmantissa)) {
          FastInteger oldDigitLength = accum.GetDigitLength();
          bigmantissa = bigmantissa.add(BigInteger.ONE);
          recheckOverflow |= binaryPrec;
          // Check if mantissa's precision is now greater
          // than the one set by the context
          if (!unlimitedPrec && (bigmantissa.testBit(0) == false || (this.thisRadix &
                                                        1) != 0) && (binaryPrec ||

                           oldDigitLength.compareTo(fastPrecision) >=

                           0)) {
            accum = this.helper.CreateShiftAccumulator(bigmantissa);
            FastInteger newDigitLength = accum.GetDigitLength();
            if (binaryPrec || newDigitLength.compareTo(fastPrecision) > 0) {
              FastInteger neededShift =
                FastInteger.Copy(newDigitLength).Subtract(fastPrecision);
              accum.ShiftRight(neededShift);
              if (binaryPrec) {
                while (accum.getShiftedInt().compareTo(maxMantissa) > 0) {
                  accum.ShiftRightInt(1);
                }
              }
              if (accum.getDiscardedDigitCount().signum() != 0) {
                exp.Add(accum.getDiscardedDigitCount());
                discardedBits.Add(accum.getDiscardedDigitCount());
                bigmantissa = accum.getShiftedInt();
                recheckOverflow |= !binaryPrec;
              }
            }
          }
        }
      }
      if (recheckOverflow && fastEMax != null) {
        // Check for overflow again
        adjExponent = FastInteger.Copy(exp);
        if (ctx.getAdjustExponent()) {
          adjExponent.Add(accum.GetDigitLength()).Decrement();
        }
        if (binaryPrec && fastEMax != null &&
            adjExponent.compareTo(fastEMax) == 0) {
          // May or may not be an overflow depending on the mantissa
          // (uses accumulator from previous steps, including the check
          // if the mantissa now exceeded the precision)
          FastInteger expdiff =
            FastInteger.Copy(fastPrecision).Subtract(accum.GetDigitLength());
          BigInteger currMantissa = accum.getShiftedInt();
          currMantissa = this.TryMultiplyByRadixPower(currMantissa, expdiff);
          if (currMantissa == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          if (currMantissa.compareTo(maxMantissa) > 0) {
            // Mantissa too high, treat as overflow
            adjExponent.Increment();
          }
        }
        if (adjExponent.compareTo(fastEMax) > 0) {
          flags |= PrecisionContext.FlagOverflow |
            PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
          if (!unlimitedPrec && (rounding == Rounding.Down || rounding ==
                   Rounding.ZeroFiveUp || (rounding == Rounding.Ceiling &&
                                   neg) ||

                                 (rounding == Rounding.Floor && !neg))) {
            // Set to the highest possible value for
            // the given precision
            BigInteger overflowMant = BigInteger.ZERO;
            if (binaryPrec) {
              overflowMant = maxMantissa;
            } else {
              overflowMant = this.TryMultiplyByRadixPower(
                BigInteger.ONE,
                fastPrecision);
              if (overflowMant == null) {
                return this.SignalInvalidWithMessage(
                  ctx,
                  "Result requires too much memory");
              }
              overflowMant = overflowMant.subtract(BigInteger.ONE);
            }
            if (ctx.getHasFlags()) {
              ctx.setFlags(ctx.getFlags() | (flags));
            }
            clamp = FastInteger.Copy(fastEMax);
            if (ctx.getAdjustExponent()) {
              clamp.Increment().Subtract(fastPrecision);
            }
            return this.helper.CreateNewWithFlags(
              overflowMant,
              clamp.AsBigInteger(),
              neg ? BigNumberFlags.FlagNegative : 0);
          }
          if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (flags));
          }
          return this.SignalOverflow(neg);
        }
      }
      if (ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags() | (flags));
      }
      if (ctx.getClampNormalExponents()) {
        // Clamp exponents to eMax + 1 - precision
        // if directed
        if (binaryPrec && this.thisRadix != 2) {
          fastPrecision =
            this.helper.CreateShiftAccumulator(maxMantissa).GetDigitLength();
        }
        FastInteger clampExp = FastInteger.Copy(fastEMax);
        if (ctx.getAdjustExponent()) {
          clampExp.Increment().Subtract(fastPrecision);
        }
        if (exp.compareTo(clampExp) > 0) {
          if (bigmantissa.signum() != 0) {
            FastInteger expdiff = FastInteger.Copy(exp).Subtract(clampExp);
            bigmantissa = this.TryMultiplyByRadixPower(bigmantissa, expdiff);
            if (bigmantissa == null) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Result requires too much memory");
            }
          }
          if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagClamped));
          }
          exp = clampExp;
        }
      }
      return this.helper.CreateNewWithFlags(
        bigmantissa,
        exp.AsBigInteger(),
        neg ? BigNumberFlags.FlagNegative : 0);
    }
    // mant1 and mant2 are assumed to be nonnegative
    private T AddCore(
      BigInteger mant1,
      BigInteger mant2,
      BigInteger exponent,
      int flags1,
      int flags2,
      PrecisionContext ctx) {
      boolean neg1 = (flags1 & BigNumberFlags.FlagNegative) != 0;
      boolean neg2 = (flags2 & BigNumberFlags.FlagNegative) != 0;
      boolean negResult = false;
      // System.out.println("neg1=" + neg1 + " neg2=" + (neg2));
      if (neg1 != neg2) {
        // Signs are different, treat as a subtraction
        mant1 = mant1.subtract(mant2);
        int mant1Sign = mant1.signum();
        negResult = neg1 ^ (mant1Sign == 0 ? neg2 : (mant1Sign < 0));
      } else {
        // Signs are same, treat as an addition
        mant1 = mant1.add(mant2);
        negResult = neg1;
      }
      if (mant1.signum() == 0 && negResult) {
        // Result is negative zero
        negResult &= (neg1 && neg2) || ((neg1 ^ neg2) && ctx != null &&
                                        ctx.getRounding() == Rounding.Floor);
      }
      // System.out.println("mant1= " + mant1 + " exp= " + exponent +" neg= "+
      // (negResult));
      return this.helper.CreateNewWithFlags(
        mant1,
        exponent,
        negResult ? BigNumberFlags.FlagNegative : 0);
    }

    public T Add(T thisValue, T other, PrecisionContext ctx) {
      if ((Object)thisValue == null) {
        throw new NullPointerException("thisValue");
      }
      if ((Object)other == null) {
        throw new NullPointerException("other");
      }
      return this.AddEx(thisValue, other, ctx, false);
    }

    public T AddEx(
      T thisValue,
      T other,
      PrecisionContext ctx,
      boolean roundToOperandPrecision) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = this.HandleNotANumber(thisValue, other, ctx);
        if ((Object)result != (Object)null) {
          return result;
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
            if ((thisFlags & BigNumberFlags.FlagNegative) != (otherFlags &
  BigNumberFlags.FlagNegative)) {
              return this.SignalInvalid(ctx);
            }
          }
          return thisValue;
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          return other;
        }
      }
      int expcmp =
        this.helper.GetExponent(thisValue)
        .compareTo((BigInteger)this.helper.GetExponent(other));
      T retval = null;
      BigInteger op1MantAbs =
        (this.helper.GetMantissa(thisValue)).abs();
      BigInteger op2MantAbs = (this.helper.GetMantissa(other)).abs();
      if (expcmp == 0) {
        retval = this.AddCore(
          op1MantAbs,
          op2MantAbs,
          this.helper.GetExponent(thisValue),
          thisFlags,
          otherFlags,
          ctx);
        if (ctx != null) {
          retval = this.RoundToPrecision(retval, ctx);
        }
      } else {
        // choose the minimum exponent
        T op1 = thisValue;
        T op2 = other;
        BigInteger op1Exponent = this.helper.GetExponent(op1);
        BigInteger op2Exponent = this.helper.GetExponent(op2);
        BigInteger resultExponent = expcmp < 0 ? op1Exponent : op2Exponent;
        FastInteger fastOp1Exp = FastInteger.FromBig(op1Exponent);
        FastInteger fastOp2Exp = FastInteger.FromBig(op2Exponent);
        FastInteger expdiff =
          FastInteger.Copy(fastOp1Exp).Subtract(fastOp2Exp).Abs();
        if (ctx != null && ctx.getPrecision().signum() > 0) {
          // Check if exponent difference is too big for
          // radix-power calculation to work quickly
          FastInteger fastPrecision = FastInteger.FromBig(ctx.getPrecision());
          // If exponent difference is greater than the precision
          if (FastInteger.Copy(expdiff).compareTo(fastPrecision) > 0) {
            int expcmp2 = fastOp1Exp.compareTo(fastOp2Exp);
            if (expcmp2 < 0) {
              if (op2MantAbs.signum() != 0) {
                // first operand's exponent is less
                // and second operand isn't zero
                // second mantissa will be shifted by the exponent
                // difference
                // _________________________111111111111|_
                // ___222222222222222|____________________
                FastInteger digitLength1 =
                  this.helper.CreateShiftAccumulator(op1MantAbs)
                  .GetDigitLength();
                if
                  (FastInteger.Copy(fastOp1Exp).Add(digitLength1).AddInt(2)
                   .compareTo(fastOp2Exp) <

                   0) {
                  // first operand's mantissa can't reach the
                  // second operand's mantissa, so the exponent can be
                  // raised without affecting the result
                  FastInteger tmp =
                    FastInteger.Copy(fastOp2Exp).SubtractInt(4)
                    .Subtract(digitLength1).SubtractBig(ctx.getPrecision());
                  FastInteger newDiff =
                    FastInteger.Copy(tmp).Subtract(fastOp2Exp).Abs();
                  if (newDiff.compareTo(expdiff) < 0) {
                    // Can be treated as almost zero
                    boolean sameSign = this.helper.GetSign(thisValue) ==
                      this.helper.GetSign(other);
                    boolean oneOpIsZero = op1MantAbs.signum() == 0;
                    FastInteger digitLength2 =
                      this.helper.CreateShiftAccumulator(op2MantAbs)
                      .GetDigitLength();
                    if (digitLength2.compareTo(fastPrecision) < 0) {
                      // Second operand's precision too short
                      FastInteger precisionDiff =
                        FastInteger.Copy(fastPrecision).Subtract(digitLength2);
                      if (!oneOpIsZero && !sameSign) {
                        precisionDiff.AddInt(2);
                      }
                      op2MantAbs = this.TryMultiplyByRadixPower(
                        op2MantAbs,
                        precisionDiff);
                      if (op2MantAbs == null) {
                        return this.SignalInvalidWithMessage(
                          ctx,
                          "Result requires too much memory");
                      }
                      BigInteger bigintTemp = precisionDiff.AsBigInteger();
                      op2Exponent = op2Exponent.subtract(bigintTemp);
                      if (!oneOpIsZero && !sameSign) {
                        op2MantAbs = op2MantAbs.subtract(BigInteger.ONE);
                      }
                      other = this.helper.CreateNewWithFlags(
                        op2MantAbs,
                        op2Exponent,
                        this.helper.GetFlags(other));
                      FastInteger shift =
                        FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                      if (oneOpIsZero && ctx != null && ctx.getHasFlags()) {
                        ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
                      }
                      // System.out.println("Second op's prec too short:
                      // op2MantAbs=" + op2MantAbs + " precdiff= " +
                      // (precisionDiff));
                      return this.RoundToPrecisionWithShift(
                        other,
                        ctx,
                        (oneOpIsZero || sameSign) ? 0 : 1,
                        (oneOpIsZero && !sameSign) ? 0 : 1,
                        shift,
                        false);
                    }
                    if (!oneOpIsZero && !sameSign) {
                      op2MantAbs = this.TryMultiplyByRadixPower(
                        op2MantAbs,
                        new FastInteger(2));
                      if (op2MantAbs == null) {
                        return this.SignalInvalidWithMessage(
                          ctx,
                          "Result requires too much memory");
                      }
                      op2Exponent = op2Exponent.subtract(BigInteger.valueOf(2));
                      op2MantAbs = op2MantAbs.subtract(BigInteger.ONE);
                      other = this.helper.CreateNewWithFlags(
                        op2MantAbs,
                        op2Exponent,
                        this.helper.GetFlags(other));
                      FastInteger shift =
                        FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                      return this.RoundToPrecisionWithShift(
                        other,
                        ctx,
                        0,
                        0,
                        shift,
                        false);
                    } else {
                      FastInteger shift2 =
                        FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                      if (!sameSign && ctx != null && ctx.getHasFlags()) {
                        ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
                      }
                      return this.RoundToPrecisionWithShift(
                        other,
                        ctx,
                        0,
                        sameSign ? 1 : 0,
                        shift2,
                        false);
                    }
                  }
                }
              }
            } else if (expcmp2 > 0) {
              if (op1MantAbs.signum() != 0) {
                // first operand's exponent is greater
                // and first operand isn't zero
                // first mantissa will be shifted by the exponent
                // difference
                // __111111111111|
                // ____________________222222222222222|
                FastInteger digitLength2 =
                  this.helper.CreateShiftAccumulator(op2MantAbs)
                  .GetDigitLength();
                if
                  (FastInteger.Copy(fastOp2Exp).Add(digitLength2).AddInt(2)
                   .compareTo(fastOp1Exp) <

                   0) {
                  // second operand's mantissa can't reach the
                  // first operand's mantissa, so the exponent can be
                  // raised without affecting the result
                  FastInteger tmp =
                    FastInteger.Copy(fastOp1Exp).SubtractInt(4)
                    .Subtract(digitLength2)
                    .SubtractBig(ctx.getPrecision());
                  FastInteger newDiff =
                    FastInteger.Copy(tmp).Subtract(fastOp1Exp).Abs();
                  if (newDiff.compareTo(expdiff) < 0) {
                    // Can be treated as almost zero
                    boolean sameSign = this.helper.GetSign(thisValue) ==
                      this.helper.GetSign(other);
                    boolean oneOpIsZero = op2MantAbs.signum() == 0;
                    digitLength2 =
                      this.helper.CreateShiftAccumulator(op1MantAbs)
                      .GetDigitLength();
                    if (digitLength2.compareTo(fastPrecision) < 0) {
                      // Second operand's precision too short
                      FastInteger precisionDiff =
                        FastInteger.Copy(fastPrecision).Subtract(digitLength2);
                      if (!oneOpIsZero && !sameSign) {
                        precisionDiff.AddInt(2);
                      }
                      op1MantAbs = this.TryMultiplyByRadixPower(
                        op1MantAbs,
                        precisionDiff);
                      if (op1MantAbs == null) {
                        return this.SignalInvalidWithMessage(
                          ctx,
                          "Result requires too much memory");
                      }
                      BigInteger bigintTemp = precisionDiff.AsBigInteger();
                      op1Exponent = op1Exponent.subtract(bigintTemp);
                      if (!oneOpIsZero && !sameSign) {
                        op1MantAbs = op1MantAbs.subtract(BigInteger.ONE);
                      }
                      thisValue = this.helper.CreateNewWithFlags(
                        op1MantAbs,
                        op1Exponent,
                        this.helper.GetFlags(thisValue));
                      FastInteger shift =
                        FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                      if (oneOpIsZero && ctx != null && ctx.getHasFlags()) {
                        ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
                      }
                      return this.RoundToPrecisionWithShift(
                        thisValue,
                        ctx,
                        (oneOpIsZero || sameSign) ? 0 : 1,
                        (oneOpIsZero && !sameSign) ? 0 : 1,
                        shift,
                        false);
                    }
                    if (!oneOpIsZero && !sameSign) {
                      op1MantAbs = this.TryMultiplyByRadixPower(
                        op1MantAbs,
                        new FastInteger(2));
                      if (op1MantAbs == null) {
                        return this.SignalInvalidWithMessage(
                          ctx,
                          "Result requires too much memory");
                      }
                      op1Exponent = op1Exponent.subtract(BigInteger.valueOf(2));
                      op1MantAbs = op1MantAbs.subtract(BigInteger.ONE);
                      thisValue = this.helper.CreateNewWithFlags(
                        op1MantAbs,
                        op1Exponent,
                        this.helper.GetFlags(thisValue));
                      FastInteger shift =
                        FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                      return this.RoundToPrecisionWithShift(
                        thisValue,
                        ctx,
                        0,
                        0,
                        shift,
                        false);
                    } else {
                      FastInteger shift2 =
                        FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                      if (!sameSign && ctx != null && ctx.getHasFlags()) {
                        ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
                      }
                      return this.RoundToPrecisionWithShift(
                        thisValue,
                        ctx,
                        0,
                        sameSign ? 1 : 0,
                        shift2,
                        false);
                    }
                  }
                }
              }
            }
            expcmp = op1Exponent.compareTo(op2Exponent);
            resultExponent = expcmp < 0 ? op1Exponent : op2Exponent;
          }
        }
        if (expcmp > 0) {
          // System.out.println("" + op1MantAbs + " " + (op2MantAbs));
          op1MantAbs = this.RescaleByExponentDiff(
            op1MantAbs,
            op1Exponent,
            op2Exponent);
          if (op1MantAbs == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          retval = this.AddCore(
            op1MantAbs,
            op2MantAbs,
            resultExponent,
            thisFlags,
            otherFlags,
            ctx);
          // System.out.println("" + op1MantAbs + " " + op2MantAbs + " ->" +
          // (op2Exponent-op1Exponent) + " [op1 exp greater]");
        } else {
          // System.out.println("" + op1MantAbs + " " + (op2MantAbs));
          op2MantAbs = this.RescaleByExponentDiff(
            op2MantAbs,
            op1Exponent,
            op2Exponent);
          if (op2MantAbs == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          // System.out.println("" + op1MantAbs + " " + op1Exponent + "" +
          // op2MantAbs + "e " + op2Exponent + " ->" +
          // (op2Exponent-op1Exponent) + " [op2 exp greater]");
          retval = this.AddCore(
            op1MantAbs,
            op2MantAbs,
            resultExponent,
            thisFlags,
            otherFlags,
            ctx);
          // System.out.println("" + op1MantAbs + " " + op2MantAbs + " ->" +
          // (op2Exponent-op1Exponent) + " [op2 exp greater]");
        }
        if (roundToOperandPrecision && ctx != null && ctx.getHasMaxPrecision()) {
          FastInteger digitLength1 =
            this.helper.CreateShiftAccumulator((op1MantAbs).abs())
            .GetDigitLength();
          FastInteger digitLength2 =
            this.helper.CreateShiftAccumulator((op2MantAbs).abs())
            .GetDigitLength();
          FastInteger maxDigitLength =
            (digitLength1.compareTo(digitLength2) > 0) ? digitLength1 :
            digitLength2;
          maxDigitLength.SubtractBig(ctx.getPrecision());
          // System.out.println("retval= " + retval + " maxdl=" +
          // maxDigitLength + " prec= " + (ctx.getPrecision()));
          retval = (maxDigitLength.signum() > 0) ?
            this.RoundToPrecisionWithShift(
              retval,
              ctx,
              0,
              0,
              maxDigitLength,
              false) : this.RoundToPrecision(retval, ctx);
          // System.out.println("retval now " + (retval));
        } else {
          retval = this.RoundToPrecision(retval, ctx);
        }
      }
      return retval;
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
      T thisValue,
      T otherValue,
      boolean treatQuietNansAsSignaling,
      PrecisionContext ctx) {
      if (otherValue == null) {
        return this.SignalInvalid(ctx);
      }
      T result = this.CompareToHandleSpecial(
        thisValue,
        otherValue,
        treatQuietNansAsSignaling,
        ctx);
      if ((Object)result != (Object)null) {
        return result;
      }
      int cmp = this.CompareToInternal(thisValue, otherValue, false);
      return (
        cmp == -2) ? this.SignalInvalidWithMessage(
        ctx,
        "Out of memory ") :
        this.ValueOf(this.compareTo(thisValue, otherValue), null);
    }

    public int compareTo(T thisValue, T otherValue) {
      return this.CompareToInternal(thisValue, otherValue, true);
    }

    /**
     * Compares a T object with this instance.
     * @param thisValue A T object.
     * @param otherValue A T object. (2).
     * @param reportOOM A Boolean object.
     * @return Zero if the values are equal; a negative number if this instance is
     * less, or a positive number if this instance is greater.
     */
    private int CompareToInternal(T thisValue, T otherValue, boolean reportOOM) {
      if (otherValue == null) {
        return 1;
      }
      int flagsThis = this.helper.GetFlags(thisValue);
      int flagsOther = this.helper.GetFlags(otherValue);
      if ((flagsThis & BigNumberFlags.FlagNaN) != 0) {
        if ((flagsOther & BigNumberFlags.FlagNaN) != 0) {
          return 0;
        }
        // Consider NaN to be greater
        return 1;
      }
      if ((flagsOther & BigNumberFlags.FlagNaN) != 0) {
        // Consider this to be less than NaN
        return -1;
      }
      int signA = this.CompareToHandleSpecialReturnInt(thisValue, otherValue);
      if (signA <= 1) {
        return signA;
      }
      signA = this.helper.GetSign(thisValue);
      int signB = this.helper.GetSign(otherValue);
      if (signA != signB) {
        return (signA < signB) ? -1 : 1;
      }
      if (signB == 0 || signA == 0) {
        // Special case: Either operand is zero
        return 0;
      }
      int expcmp =
        this.helper.GetExponent(thisValue)
        .compareTo((BigInteger)this.helper.GetExponent(otherValue));
      // At this point, the signs are equal so we can compare
      // their absolute values instead
      int mantcmp =
        (this.helper.GetMantissa(thisValue)).abs()
        .compareTo((this.helper.GetMantissa(otherValue)).abs());
      if (signA < 0) {
        mantcmp = -mantcmp;
      }
      if (mantcmp == 0) {
        // Special case: Mantissas are equal
        return signA < 0 ? -expcmp : expcmp;
      }
      if (expcmp == 0) {
        return mantcmp;
      }
      BigInteger op1Exponent = this.helper.GetExponent(thisValue);
      BigInteger op2Exponent = this.helper.GetExponent(otherValue);
      FastInteger fastOp1Exp = FastInteger.FromBig(op1Exponent);
      FastInteger fastOp2Exp = FastInteger.FromBig(op2Exponent);
      FastInteger expdiff =
        FastInteger.Copy(fastOp1Exp).Subtract(fastOp2Exp).Abs();
      // Check if exponent difference is too big for
      // radix-power calculation to work quickly
      if (expdiff.CompareToInt(100) >= 0) {
        BigInteger op1MantAbs =
          (this.helper.GetMantissa(thisValue)).abs();
        BigInteger op2MantAbs =
          (this.helper.GetMantissa(otherValue)).abs();
        FastInteger precision1 =
          this.helper.CreateShiftAccumulator(op1MantAbs).GetDigitLength();
        FastInteger precision2 =
          this.helper.CreateShiftAccumulator(op2MantAbs).GetDigitLength();
        FastInteger maxPrecision = null;
        maxPrecision = (precision1.compareTo(precision2) > 0) ? precision1 :
          precision2;
        // If exponent difference is greater than the
        // maximum precision of the two operands
        if (FastInteger.Copy(expdiff).compareTo(maxPrecision) > 0) {
          int expcmp2 = fastOp1Exp.compareTo(fastOp2Exp);
          if (expcmp2 < 0) {
            if (op2MantAbs.signum() != 0) {
              // first operand's exponent is less
              // and second operand isn't zero
              // second mantissa will be shifted by the exponent
              // difference
              FastInteger digitLength1 =
                this.helper.CreateShiftAccumulator(op1MantAbs).GetDigitLength();
              if (FastInteger.Copy(fastOp1Exp).Add(digitLength1).AddInt(2)
                  .compareTo(fastOp2Exp) <

                  0) {
                // first operand's mantissa can't reach the
                // second operand's mantissa, so the exponent can be
                // raised without affecting the result
                FastInteger tmp = FastInteger.Copy(fastOp2Exp)
                  .SubtractInt(8).Subtract(digitLength1)
                  .Subtract(maxPrecision);
                FastInteger newDiff =
                  FastInteger.Copy(tmp).Subtract(fastOp2Exp).Abs();
                if (newDiff.compareTo(expdiff) < 0) {
                  // At this point, both operands have the same sign
                  return (signA < 0) ? 1 : -1;
                }
              }
            }
          } else if (expcmp2 > 0) {
            if (op1MantAbs.signum() != 0) {
              // first operand's exponent is greater
              // and second operand isn't zero
              // first mantissa will be shifted by the exponent
              // difference
              FastInteger digitLength2 =
                this.helper.CreateShiftAccumulator(op2MantAbs).GetDigitLength();
              if (FastInteger.Copy(fastOp2Exp)
                  .Add(digitLength2).AddInt(2).compareTo(fastOp1Exp) <

                  0) {
                // second operand's mantissa can't reach the
                // first operand's mantissa, so the exponent can be
                // raised without affecting the result
                FastInteger tmp = FastInteger.Copy(fastOp1Exp)
                  .SubtractInt(8)
                  .Subtract(digitLength2)
                  .Subtract(maxPrecision);
                FastInteger newDiff =
                  FastInteger.Copy(tmp).Subtract(fastOp1Exp).Abs();
                if (newDiff.compareTo(expdiff) < 0) {
                  // At this point, both operands have the same sign
                  return (signA < 0) ? -1 : 1;
                }
              }
            }
          }
          expcmp = op1Exponent.compareTo(op2Exponent);
        }
      }
      if (expcmp > 0) {
        BigInteger newmant =
          this.RescaleByExponentDiff(
            this.helper.GetMantissa(thisValue),
            op1Exponent,
            op2Exponent);
        if (newmant == null) {
          if (reportOOM) {
            throw new OutOfMemoryError("Result requires too much memory");
          }
          return -2;
        }
        BigInteger othermant =
          (this.helper.GetMantissa(otherValue)).abs();
        newmant = (newmant).abs();
        mantcmp = newmant.compareTo(othermant);
        return (signA < 0) ? -mantcmp : mantcmp;
      } else {
        BigInteger newmant =
          this.RescaleByExponentDiff(
            this.helper.GetMantissa(otherValue),
            op1Exponent,
            op2Exponent);
        if (newmant == null) {
          if (reportOOM) {
            throw new OutOfMemoryError("Result requires too much memory");
          }
          return -2;
        }
        BigInteger othermant =
          (this.helper.GetMantissa(thisValue)).abs();
        newmant = (newmant).abs();
        mantcmp = othermant.compareTo(newmant);
        return (signA < 0) ? -mantcmp : mantcmp;
      }
    }

    public IRadixMathHelper<T> GetHelper() {
      return this.helper;
    }

    public T RoundAfterConversion(T thisValue, PrecisionContext ctx) {
      // System.out.println("RM RoundAfterConversion");
      return this.RoundToPrecision(thisValue, ctx);
    }
  }
