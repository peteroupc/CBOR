package com.upokecenter.util;


//import java.math.*;


    /**
     * Encapsulates radix-independent arithmetic.
     */
  class RadixMath<T> {
    
    IRadixMathHelper<T> helper;
    int thisRadix;
    int support;
    
    public RadixMath (IRadixMathHelper<T> helper) {
      this.helper = helper;
      this.support=helper.GetArithmeticSupport();
      this.thisRadix=helper.GetRadix();
    }

    private T ReturnQuietNaNFastIntPrecision(T thisValue, FastInteger precision) {
      BigInteger mant=(helper.GetMantissa(thisValue)).abs();
      boolean mantChanged=false;
      if(!(mant.signum()==0) && precision!=null && precision.signum()>0){
        BigInteger limit=helper.MultiplyByRadixPower(
          BigInteger.ONE,precision);
        if(mant.compareTo(limit)>=0){
          mant=mant.remainder(limit);
          mantChanged=true;
        }
      }
      int flags=helper.GetFlags(thisValue);
      if(!mantChanged && (flags&BigNumberFlags.FlagQuietNaN)!=0){
        return thisValue;
      }
      flags&=BigNumberFlags.FlagNegative;
      flags|=BigNumberFlags.FlagQuietNaN;
      return helper.CreateNewWithFlags(mant,BigInteger.ZERO,flags);
    }

    private T ReturnQuietNaN(T thisValue, PrecisionContext ctx) {
      BigInteger mant=(helper.GetMantissa(thisValue)).abs();
      boolean mantChanged=false;
      if(!(mant.signum()==0) && ctx!=null && !((ctx.getPrecision()).signum()==0)){
        BigInteger limit=helper.MultiplyByRadixPower(
          BigInteger.ONE,FastInteger.FromBig(ctx.getPrecision()));
        if(mant.compareTo(limit)>=0){
          mant=mant.remainder(limit);
          mantChanged=true;
        }
      }
      int flags=helper.GetFlags(thisValue);
      if(!mantChanged && (flags&BigNumberFlags.FlagQuietNaN)!=0){
        return thisValue;
      }
      flags&=BigNumberFlags.FlagNegative;
      flags|=BigNumberFlags.FlagQuietNaN;
      return helper.CreateNewWithFlags(mant,BigInteger.ZERO,flags);
    }

    private T DivisionHandleSpecial(T thisValue, T other, PrecisionContext ctx) {
      int thisFlags=helper.GetFlags(thisValue);
      int otherFlags=helper.GetFlags(other);
      if(((thisFlags|otherFlags)&BigNumberFlags.FlagSpecial)!=0){
        T result=HandleNotANumber(thisValue,other,ctx);
        if((Object)result!=(Object)null)return result;
        if((thisFlags&BigNumberFlags.FlagInfinity)!=0 && (otherFlags&BigNumberFlags.FlagInfinity)!=0){
          // Attempt to divide infinity by infinity
          return SignalInvalid(ctx);
        }
        if((thisFlags&BigNumberFlags.FlagInfinity)!=0){
          return EnsureSign(thisValue,((thisFlags^otherFlags)&BigNumberFlags.FlagNegative)!=0);
        }
        if((otherFlags&BigNumberFlags.FlagInfinity)!=0){
          // Divisor is infinity, so result will be epsilon
          if(ctx!=null && ctx.getHasExponentRange() && (ctx.getPrecision()).signum()>0){
            if(ctx.getHasFlags()){
              ctx.setFlags(ctx.getFlags()|(PrecisionContext.FlagClamped));
            }
            BigInteger bigexp=ctx.getEMin();
            bigexp=bigexp.subtract(BigInteger.valueOf(ctx.getPrecision()));
            bigexp=bigexp.add(BigInteger.ONE);
            thisFlags=((thisFlags^otherFlags)&BigNumberFlags.FlagNegative);
            return helper.CreateNewWithFlags(
              BigInteger.ZERO,bigexp,thisFlags);
          }
          thisFlags=((thisFlags^otherFlags)&BigNumberFlags.FlagNegative);
          return RoundToPrecision(helper.CreateNewWithFlags(
            BigInteger.ZERO,BigInteger.ZERO,
            thisFlags),ctx);
        }
      }
      return null;
    }
    
    private T RemainderHandleSpecial(T thisValue, T other, PrecisionContext ctx) {
      int thisFlags=helper.GetFlags(thisValue);
      int otherFlags=helper.GetFlags(other);
      if(((thisFlags|otherFlags)&BigNumberFlags.FlagSpecial)!=0){
        T result=HandleNotANumber(thisValue,other,ctx);
        if((Object)result!=(Object)null)return result;
        if((thisFlags&BigNumberFlags.FlagInfinity)!=0){
          return SignalInvalid(ctx);
        }
        if((otherFlags&BigNumberFlags.FlagInfinity)!=0){
          return RoundToPrecision(thisValue,ctx);
        }
      }
      if(helper.GetMantissa(other).signum()==0){
        return SignalInvalid(ctx);
      }
      return null;
    }

    private T MinMaxHandleSpecial(T thisValue, T otherValue, PrecisionContext ctx,
                                  boolean isMinOp, boolean compareAbs) {
      int thisFlags=helper.GetFlags(thisValue);
      int otherFlags=helper.GetFlags(otherValue);
      if(((thisFlags|otherFlags)&BigNumberFlags.FlagSpecial)!=0){
        // Check this value then the other value for signaling NaN
        if((helper.GetFlags(thisValue)&BigNumberFlags.FlagSignalingNaN)!=0){
          return SignalingNaNInvalid(thisValue,ctx);
        }
        if((helper.GetFlags(otherValue)&BigNumberFlags.FlagSignalingNaN)!=0){
          return SignalingNaNInvalid(otherValue,ctx);
        }
        // Check this value then the other value for quiet NaN
        if((helper.GetFlags(thisValue)&BigNumberFlags.FlagQuietNaN)!=0){
          if((helper.GetFlags(otherValue)&BigNumberFlags.FlagQuietNaN)!=0){
            // both values are quiet NaN
            return ReturnQuietNaN(thisValue,ctx);
          }
          // return "other" for being numeric
          return RoundToPrecision(otherValue,ctx);
        }
        if((helper.GetFlags(otherValue)&BigNumberFlags.FlagQuietNaN)!=0){
          // At this point, "thisValue" can't be NaN,
          // return "thisValue" for being numeric
          return RoundToPrecision(thisValue,ctx);
        }
        if((thisFlags&BigNumberFlags.FlagInfinity)!=0){
          if(compareAbs && (otherFlags&BigNumberFlags.FlagInfinity)==0){
            // treat this as larger
            return (isMinOp) ? RoundToPrecision(otherValue,ctx) : thisValue;
          }
          // This value is infinity
          if(isMinOp){
            return ((thisFlags&BigNumberFlags.FlagNegative)!=0) ?
              thisValue : // if negative, will be less than every other number
              RoundToPrecision(otherValue,ctx); // if positive, will be greater
          } else {
            return ((thisFlags&BigNumberFlags.FlagNegative)==0) ?
              thisValue : // if positive, will be greater than every other number
              RoundToPrecision(otherValue,ctx);
          }
        }
        if((otherFlags&BigNumberFlags.FlagInfinity)!=0){
          if(compareAbs){
            // treat this as larger (the first value
            // won't be infinity at this point
            return (isMinOp) ? RoundToPrecision(thisValue,ctx) : otherValue;
          }
          if(isMinOp){
            return ((otherFlags&BigNumberFlags.FlagNegative)==0) ?
              RoundToPrecision(thisValue,ctx) :
              otherValue;
          } else {
            return ((otherFlags&BigNumberFlags.FlagNegative)!=0) ?
              RoundToPrecision(thisValue,ctx) :
              otherValue;
          }
        }
      }
      return null;
    }
    
    private T HandleNotANumber(T thisValue, T other, PrecisionContext ctx) {
      int thisFlags=helper.GetFlags(thisValue);
      int otherFlags=helper.GetFlags(other);
      // Check this value then the other value for signaling NaN
      if((helper.GetFlags(thisValue)&BigNumberFlags.FlagSignalingNaN)!=0){
        return SignalingNaNInvalid(thisValue,ctx);
      }
      if((helper.GetFlags(other)&BigNumberFlags.FlagSignalingNaN)!=0){
        return SignalingNaNInvalid(other,ctx);
      }
      // Check this value then the other value for quiet NaN
      if((helper.GetFlags(thisValue)&BigNumberFlags.FlagQuietNaN)!=0){
        return ReturnQuietNaN(thisValue,ctx);
      }
      if((helper.GetFlags(other)&BigNumberFlags.FlagQuietNaN)!=0){
        return ReturnQuietNaN(other,ctx);
      }
      return null;
    }
    
    private T ValueOf(int value, PrecisionContext ctx) {
      if(ctx==null || !ctx.getHasExponentRange() || ctx.ExponentWithinRange(BigInteger.ZERO))
        return helper.ValueOf(value);
      return RoundToPrecision(helper.ValueOf(value),ctx);
    }
    private int CompareToHandleSpecialReturnInt(
      T thisValue, T other) {
      int thisFlags=helper.GetFlags(thisValue);
      int otherFlags=helper.GetFlags(other);
      if(((thisFlags|otherFlags)&BigNumberFlags.FlagSpecial)!=0){
        if(((thisFlags|otherFlags)&BigNumberFlags.FlagNaN)!=0){
          throw new ArithmeticException("Either operand is NaN");
        }
        if((thisFlags&BigNumberFlags.FlagInfinity)!=0){
          // thisValue is infinity
          if((thisFlags&(BigNumberFlags.FlagInfinity|BigNumberFlags.FlagNegative))==
             (otherFlags&(BigNumberFlags.FlagInfinity|BigNumberFlags.FlagNegative)))
            return 0;
          return ((thisFlags&BigNumberFlags.FlagNegative)==0) ? 1 : -1;
        }
        if((otherFlags&BigNumberFlags.FlagInfinity)!=0){
          // the other value is infinity
          if((thisFlags&(BigNumberFlags.FlagInfinity|BigNumberFlags.FlagNegative))==
             (otherFlags&(BigNumberFlags.FlagInfinity|BigNumberFlags.FlagNegative)))
            return 0;
          return ((otherFlags&BigNumberFlags.FlagNegative)==0) ? -1 : 1;
        }
      }
      return 2;
    }
    
    private T CompareToHandleSpecial(T thisValue, T other, boolean treatQuietNansAsSignaling, PrecisionContext ctx) {
      int thisFlags=helper.GetFlags(thisValue);
      int otherFlags=helper.GetFlags(other);
      if(((thisFlags|otherFlags)&BigNumberFlags.FlagSpecial)!=0){
        // Check this value then the other value for signaling NaN
        if((helper.GetFlags(thisValue)&BigNumberFlags.FlagSignalingNaN)!=0){
          return SignalingNaNInvalid(thisValue,ctx);
        }
        if((helper.GetFlags(other)&BigNumberFlags.FlagSignalingNaN)!=0){
          return SignalingNaNInvalid(other,ctx);
        }
        if(treatQuietNansAsSignaling){
          if((helper.GetFlags(thisValue)&BigNumberFlags.FlagQuietNaN)!=0){
            return SignalingNaNInvalid(thisValue,ctx);
          }
          if((helper.GetFlags(other)&BigNumberFlags.FlagQuietNaN)!=0){
            return SignalingNaNInvalid(other,ctx);
          }
        } else {
          // Check this value then the other value for quiet NaN
          if((helper.GetFlags(thisValue)&BigNumberFlags.FlagQuietNaN)!=0){
            return ReturnQuietNaN(thisValue,ctx);
          }
          if((helper.GetFlags(other)&BigNumberFlags.FlagQuietNaN)!=0){
            return ReturnQuietNaN(other,ctx);
          }
        }
        if((thisFlags&BigNumberFlags.FlagInfinity)!=0){
          // thisValue is infinity
          if((thisFlags&(BigNumberFlags.FlagInfinity|BigNumberFlags.FlagNegative))==
             (otherFlags&(BigNumberFlags.FlagInfinity|BigNumberFlags.FlagNegative)))
            return ValueOf(0,null);
          return ((thisFlags&BigNumberFlags.FlagNegative)==0) ?
            ValueOf(1,null) : ValueOf(-1,null);
        }
        if((otherFlags&BigNumberFlags.FlagInfinity)!=0){
          // the other value is infinity
          if((thisFlags&(BigNumberFlags.FlagInfinity|BigNumberFlags.FlagNegative))==
             (otherFlags&(BigNumberFlags.FlagInfinity|BigNumberFlags.FlagNegative)))
            return ValueOf(0,null);
          return ((otherFlags&BigNumberFlags.FlagNegative)==0) ?
            ValueOf(-1,null) : ValueOf(1,null);
        }
      }
      return null;
    }
    private T SignalingNaNInvalid(T value, PrecisionContext ctx) {
      int flags=helper.GetFlags(value);
      if(ctx!=null && ctx.getHasFlags()){
        ctx.setFlags(ctx.getFlags()|(PrecisionContext.FlagInvalid));
      }
      return ReturnQuietNaN(value,ctx);
    }
    private T SignalInvalid(PrecisionContext ctx) {
      if(support==BigNumberFlags.FiniteOnly)
        throw new ArithmeticException("Invalid operation");
      if(ctx!=null && ctx.getHasFlags()){
        ctx.setFlags(ctx.getFlags()|(PrecisionContext.FlagInvalid));
      }
      return helper.CreateNewWithFlags(BigInteger.ZERO,BigInteger.ZERO,BigNumberFlags.FlagQuietNaN);
    }
    private T SignalInvalidWithMessage(PrecisionContext ctx, String str) {
      if(support==BigNumberFlags.FiniteOnly)
        throw new ArithmeticException("Invalid operation");
      if(ctx!=null && ctx.getHasFlags()){
        ctx.setFlags(ctx.getFlags()|(PrecisionContext.FlagInvalid));
      }
      return helper.CreateNewWithFlags(BigInteger.ZERO,BigInteger.ZERO,BigNumberFlags.FlagQuietNaN);
    }
    
    private T SignalOverflow(boolean neg) {
      return support==BigNumberFlags.FiniteOnly ? null :
        helper.CreateNewWithFlags(
          BigInteger.ZERO,BigInteger.ZERO,
          (neg ? BigNumberFlags.FlagNegative : 0)|BigNumberFlags.FlagInfinity);
    }
    
    private T SignalDivideByZero(PrecisionContext ctx, boolean neg) {
      if(support==BigNumberFlags.FiniteOnly)
        throw new ArithmeticException("Division by zero");
      if(ctx!=null && ctx.getHasFlags()){
        ctx.setFlags(ctx.getFlags()|(PrecisionContext.FlagDivideByZero));
      }
      return helper.CreateNewWithFlags(
        BigInteger.ZERO,BigInteger.ZERO,
        BigNumberFlags.FlagInfinity|(neg ? BigNumberFlags.FlagNegative : 0));
    }

    private boolean Round(IShiftAccumulator accum, Rounding rounding,
                       boolean neg, FastInteger fastint) {
      boolean incremented = false;
      int radix = thisRadix;
      if (rounding == Rounding.HalfUp) {
        if (accum.getLastDiscardedDigit() >= (radix / 2)) {
          incremented = true;
        }
      } else if (rounding == Rounding.HalfEven) {
        if (accum.getLastDiscardedDigit() >= (radix / 2)) {
          if ((accum.getLastDiscardedDigit() > (radix / 2) || accum.getOlderDiscardedDigits() != 0)) {
            incremented = true;
          } else if (!fastint.isEvenNumber()) {
            incremented = true;
          }
        }
      } else if (rounding == Rounding.Ceiling) {
        if (!neg && (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.Floor) {
        if (neg && (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.HalfDown) {
        if (accum.getLastDiscardedDigit() > (radix / 2) ||
            (accum.getLastDiscardedDigit() == (radix / 2) && accum.getOlderDiscardedDigits() != 0)) {
          incremented = true;
        }
      } else if (rounding == Rounding.Up) {
        if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.ZeroFiveUp) {
        if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
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

    private boolean RoundGivenDigits(int lastDiscarded, int olderDiscarded, Rounding rounding,
                                  boolean neg, BigInteger bigval) {
      boolean incremented = false;
      int radix = thisRadix;
      if (rounding == Rounding.HalfUp) {
        if (lastDiscarded >= (radix / 2)) {
          incremented = true;
        }
      } else if (rounding == Rounding.HalfEven) {
        if (lastDiscarded >= (radix / 2)) {
          if ((lastDiscarded > (radix / 2) || olderDiscarded != 0)) {
            incremented = true;
          } else if (bigval.testBit(0)) {
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

    private boolean RoundGivenBigInt(IShiftAccumulator accum, Rounding rounding,
                                  boolean neg, BigInteger bigval) {
      return RoundGivenDigits(accum.getLastDiscardedDigit(),accum.getOlderDiscardedDigits(),rounding,
                              neg,bigval);
    }

    private T EnsureSign(T val, boolean negative) {
      if (val == null) return val;
      int flags = helper.GetFlags(val);
      if ((negative && (flags&BigNumberFlags.FlagNegative)==0) ||
          (!negative && (flags&BigNumberFlags.FlagNegative)!=0)) {
        flags&=~BigNumberFlags.FlagNegative;
        flags|=(negative ? BigNumberFlags.FlagNegative : 0);
        return helper.CreateNewWithFlags(
          helper.GetMantissa(val),helper.GetExponent(val),flags);
      }
      return val;
    }

    /**
     * 
     * @param thisValue A T object.
     * @param divisor A T object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T DivideToIntegerNaturalScale(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      FastInteger desiredScale = FastInteger.FromBig(
        helper.GetExponent(thisValue)).SubtractBig(
        helper.GetExponent(divisor));
      PrecisionContext ctx2=PrecisionContext.ForRounding(Rounding.Down).WithBigPrecision(
        ctx == null ? BigInteger.ZERO : ctx.getPrecision()).WithBlankFlags();
      T ret = DivideInternal(thisValue, divisor,
                             ctx2,
                             IntegerModeFixedScale, BigInteger.ZERO,
                             null);
      if((ctx2.getFlags()&(PrecisionContext.FlagInvalid|PrecisionContext.FlagDivideByZero))!=0){
        if(ctx.getHasFlags()){
          ctx.setFlags(ctx.getFlags()|((PrecisionContext.FlagInvalid|PrecisionContext.FlagDivideByZero)));
        }
        return ret;
      }
      boolean neg = (helper.GetSign(thisValue) < 0) ^ (helper.GetSign(divisor) < 0);
      // Now the exponent's sign can only be 0 or positive
      if (helper.GetMantissa(ret).signum()==0) {
        // Value is 0, so just change the exponent
        // to the preferred one
        BigInteger dividendExp=helper.GetExponent(thisValue);
        BigInteger divisorExp=helper.GetExponent(divisor);
        ret = helper.CreateNewWithFlags(BigInteger.ZERO,
                                        (dividendExp.subtract(divisorExp)),helper.GetFlags(ret));
      } else {
        if (desiredScale.signum() < 0) {
          // Desired scale is negative, shift left
          desiredScale.Negate();
          BigInteger bigmantissa = (helper.GetMantissa(ret)).abs();
          bigmantissa = helper.MultiplyByRadixPower(bigmantissa, desiredScale);
          ret = helper.CreateNewWithFlags(
            bigmantissa,
            helper.GetExponent(thisValue).subtract(helper.GetExponent(divisor)),
            helper.GetFlags(ret));
        } else if (desiredScale.signum() > 0) {
          // Desired scale is positive, shift away zeros
          // but not after scale is reached
          BigInteger bigmantissa = (helper.GetMantissa(ret)).abs();
          FastInteger fastexponent = FastInteger.FromBig(helper.GetExponent(ret));
          BigInteger bigradix = BigInteger.valueOf(thisRadix);
          while(true){
            if(desiredScale.compareTo(fastexponent)==0)
              break;
            BigInteger bigrem;
            BigInteger bigquo;
BigInteger[] divrem=(bigmantissa).divideAndRemainder(bigradix);
bigquo=divrem[0];
bigrem=divrem[1];
            if(bigrem.signum()!=0)
              break;
            bigmantissa=bigquo;
            fastexponent.AddInt(1);
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

    /**
     * 
     * @param thisValue A T object.
     * @param divisor A T object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T DivideToIntegerZeroScale(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      PrecisionContext ctx2=PrecisionContext.ForRounding(Rounding.Down).WithBigPrecision(
        ctx == null ? BigInteger.ZERO : ctx.getPrecision()).WithBlankFlags();
      T ret = DivideInternal(thisValue, divisor,
                             ctx2,
                             IntegerModeFixedScale, BigInteger.ZERO,
                             null);
      if((ctx2.getFlags()&(PrecisionContext.FlagInvalid|PrecisionContext.FlagDivideByZero))!=0){
        if(ctx.getHasFlags()){
          ctx.setFlags(ctx.getFlags()|((ctx2.getFlags()&(PrecisionContext.FlagInvalid|PrecisionContext.FlagDivideByZero))));
        }
        return ret;
      }
      if (ctx != null) {
        ctx2 = ctx.WithBlankFlags().WithUnlimitedExponents();
        ret = RoundToPrecision(ret, ctx2);
        if ((ctx2.getFlags() & PrecisionContext.FlagRounded) != 0) {
          return SignalInvalid(ctx);
        }
      }
      return ret;
    }
    
    /**
     * 
     * @param value A T object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Abs(T value, PrecisionContext ctx) {
      int flags=helper.GetFlags(value);
      if((flags&BigNumberFlags.FlagSignalingNaN)!=0){
        return SignalingNaNInvalid(value,ctx);
      }
      if((flags&BigNumberFlags.FlagQuietNaN)!=0){
        return ReturnQuietNaN(value,ctx);
      }
      if((flags&BigNumberFlags.FlagNegative)!=0){
        return RoundToPrecision(
          helper.CreateNewWithFlags(
            helper.GetMantissa(value),helper.GetExponent(value),
            flags&~BigNumberFlags.FlagNegative),
          ctx);
      }
      return RoundToPrecision(value,ctx);
    }

    /**
     * 
     * @param value A T object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Negate(T value, PrecisionContext ctx) {
      int flags=helper.GetFlags(value);
      if((flags&BigNumberFlags.FlagSignalingNaN)!=0){
        return SignalingNaNInvalid(value,ctx);
      }
      if((flags&BigNumberFlags.FlagQuietNaN)!=0){
        return ReturnQuietNaN(value,ctx);
      }
      BigInteger mant=helper.GetMantissa(value);
      if((flags&BigNumberFlags.FlagInfinity)==0 && mant.signum()==0){
        if((flags&BigNumberFlags.FlagNegative)==0){
          // positive 0 minus positive 0 is always positive 0
          return RoundToPrecision(helper.CreateNewWithFlags(
            mant,helper.GetExponent(value),
            flags&~BigNumberFlags.FlagNegative),ctx);
        } else if((ctx!=null && ctx.getRounding()== Rounding.Floor)){
          // positive 0 minus negative 0 is negative 0 only if
          // the rounding is Floor
          return RoundToPrecision(helper.CreateNewWithFlags(
            mant,helper.GetExponent(value),
            flags|BigNumberFlags.FlagNegative),ctx);
        } else {
          return RoundToPrecision(helper.CreateNewWithFlags(
            mant,helper.GetExponent(value),
            flags&~BigNumberFlags.FlagNegative),ctx);
        }
      }
      flags=flags^BigNumberFlags.FlagNegative;
      return RoundToPrecision(
        helper.CreateNewWithFlags(mant,helper.GetExponent(value),
                                  flags),
        ctx);
    }

    private T AbsRaw(T value) {
      return EnsureSign(value,false);
    }

    private T NegateRaw(T val) {
      if (val == null) return val;
      int sign = helper.GetFlags(val)&BigNumberFlags.FlagNegative;
      return helper.CreateNewWithFlags(helper.GetMantissa(val),helper.GetExponent(val),
                                       sign==0 ? BigNumberFlags.FlagNegative : 0);
    }

    private static void TransferFlags(PrecisionContext ctxDst, PrecisionContext ctxSrc) {
      if(ctxDst!=null && ctxDst.getHasFlags()){
        if((ctxSrc.getFlags()&(PrecisionContext.FlagInvalid|PrecisionContext.FlagDivideByZero))!=0){
          ctxDst.setFlags(ctxDst.getFlags()|((ctxSrc.getFlags()&(PrecisionContext.FlagInvalid|PrecisionContext.FlagDivideByZero))));
        } else {
          ctxDst.setFlags(ctxDst.getFlags()|(ctxSrc.getFlags()));
        }
      }
    }
    
    /**
     * Finds the remainder that results when dividing two T objects.
     * @param thisValue A T object.
     * @param divisor A T object.
     * @param ctx A PrecisionContext object.
     * @return The remainder of the two objects.
     */
    public T Remainder(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      PrecisionContext ctx2=ctx==null ? null : ctx.WithBlankFlags();
      T ret=RemainderHandleSpecial(thisValue,divisor,ctx2);
      if((Object)ret!=(Object)null){
        TransferFlags(ctx,ctx2);
        return ret;
      }
      ret=DivideToIntegerZeroScale(thisValue,divisor,ctx2);
      ret=Add(thisValue,NegateRaw(Multiply(ret,divisor,null)),ctx2);
      ret=EnsureSign(ret,(helper.GetFlags(thisValue)&BigNumberFlags.FlagNegative)!=0);
      TransferFlags(ctx,ctx2);
      return ret;
    }

    /**
     * 
     * @param thisValue A T object.
     * @param divisor A T object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T RemainderNear(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      PrecisionContext ctx2=ctx==null ?
        PrecisionContext.ForRounding(Rounding.HalfEven).WithBlankFlags() :
        ctx.WithRounding(Rounding.HalfEven).WithBlankFlags();
      T ret=RemainderHandleSpecial(thisValue,divisor,ctx2);
      if((Object)ret!=(Object)null){
        TransferFlags(ctx,ctx2);
        return ret;
      }
      ret=DivideInternal(thisValue,divisor,ctx2,
                         IntegerModeFixedScale,BigInteger.ZERO,null);
      if((ctx2.getFlags()&(PrecisionContext.FlagInvalid))!=0){
        return SignalInvalid(ctx);
      }
      ctx2=ctx2.WithBlankFlags();
      ret=RoundToPrecision(ret,ctx2);
      if((ctx2.getFlags()&(PrecisionContext.FlagRounded|PrecisionContext.FlagInvalid))!=0){
        return SignalInvalid(ctx);
      }
      ctx2=ctx==null ? null : ctx.WithBlankFlags();
      T ret2=Add(thisValue,NegateRaw(Multiply(ret,divisor,null)),ctx2);
      if((ctx2.getFlags()&(PrecisionContext.FlagInvalid))!=0){
        return SignalInvalid(ctx);
      }
      if(helper.GetFlags(ret2)==0 && helper.GetMantissa(ret2).signum()==0){
        ret2=EnsureSign(ret2,(helper.GetFlags(thisValue)&BigNumberFlags.FlagNegative)!=0);
      }
      TransferFlags(ctx,ctx2);
      return ret2;
    }

    /**
     * 
     * @param thisValue A T object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T NextMinus(
      T thisValue,
      PrecisionContext ctx
     ) {
      if((ctx)==null)throw new NullPointerException("ctx");
      if((ctx.getPrecision()).signum()<=0)throw new IllegalArgumentException("ctx.getPrecision()"+" not less than "+"0"+" ("+(ctx.getPrecision())+")");
      if(!(ctx.getHasExponentRange()))throw new IllegalArgumentException("doesn't satisfy ctx.getHasExponentRange()");
      int flags=helper.GetFlags(thisValue);
      if((flags&BigNumberFlags.FlagSignalingNaN)!=0){
        return SignalingNaNInvalid(thisValue,ctx);
      }
      if((flags&BigNumberFlags.FlagQuietNaN)!=0){
        return ReturnQuietNaN(thisValue,ctx);
      }
      if((flags&BigNumberFlags.FlagInfinity)!=0){
        if((flags&BigNumberFlags.FlagNegative)!=0){
          return thisValue;
        } else {
          BigInteger bigexp2 = ctx.getEMax();
          bigexp2=bigexp2.add(BigInteger.ONE);
          bigexp2=bigexp2.subtract(BigInteger.valueOf(ctx.getPrecision()));
          BigInteger overflowMant=helper.MultiplyByRadixPower(
            BigInteger.ONE,FastInteger.FromBig(ctx.getPrecision()));
          overflowMant=overflowMant.subtract(BigInteger.ONE);
          return helper.CreateNewWithFlags(overflowMant,bigexp2,0);
        }
      }
      FastInteger minexp=FastInteger.FromBig(ctx.getEMin()).SubtractBig(ctx.getPrecision()).AddInt(1);
      FastInteger bigexp=FastInteger.FromBig(helper.GetExponent(thisValue));
      if(bigexp.compareTo(minexp)<=0){
        // Use a smaller exponent if the input exponent is already
        // very small
        minexp=FastInteger.Copy(bigexp).SubtractInt(2);
      }
      T quantum=helper.CreateNewWithFlags(
        BigInteger.ONE,
        minexp.AsBigInteger(),BigNumberFlags.FlagNegative);
      PrecisionContext ctx2;
      ctx2=ctx.WithRounding(Rounding.Floor);
      return Add(thisValue,quantum,ctx2);
    }

    /**
     * 
     * @param thisValue A T object.
     * @param otherValue A T object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T NextToward(
      T thisValue,
      T otherValue,
      PrecisionContext ctx
     ) {
      if((ctx)==null)throw new NullPointerException("ctx");
      if((ctx.getPrecision()).signum()<=0)throw new IllegalArgumentException("ctx.getPrecision()"+" not less than "+"0"+" ("+(ctx.getPrecision())+")");
      if(!(ctx.getHasExponentRange()))throw new IllegalArgumentException("doesn't satisfy ctx.getHasExponentRange()");
      int thisFlags=helper.GetFlags(thisValue);
      int otherFlags=helper.GetFlags(otherValue);
      if(((thisFlags|otherFlags)&BigNumberFlags.FlagSpecial)!=0){
        T result=HandleNotANumber(thisValue,otherValue,ctx);
        if((Object)result!=(Object)null)return result;
      }
      PrecisionContext ctx2;
      int cmp=compareTo(thisValue,otherValue);
      if(cmp==0){
        return RoundToPrecision(
          EnsureSign(thisValue,(otherFlags&BigNumberFlags.FlagNegative)!=0),
          ctx.WithNoFlags());
      } else {
        if((thisFlags&BigNumberFlags.FlagInfinity)!=0){
          if((thisFlags&(BigNumberFlags.FlagInfinity|BigNumberFlags.FlagNegative))==
             (otherFlags&(BigNumberFlags.FlagInfinity|BigNumberFlags.FlagNegative))){
            // both values are the same infinity
            return thisValue;
          } else {
            BigInteger bigexp2 = ctx.getEMax();
            bigexp2=bigexp2.add(BigInteger.ONE);
            bigexp2=bigexp2.subtract(BigInteger.valueOf(ctx.getPrecision()));
            BigInteger overflowMant=helper.MultiplyByRadixPower(
              BigInteger.ONE,FastInteger.FromBig(ctx.getPrecision()));
            overflowMant=overflowMant.subtract(BigInteger.ONE);
            return helper.CreateNewWithFlags(overflowMant,bigexp2,
                                             thisFlags&BigNumberFlags.FlagNegative);
          }
        }
        FastInteger minexp=FastInteger.FromBig(ctx.getEMin()).SubtractBig(ctx.getPrecision()).AddInt(1);
        FastInteger bigexp=FastInteger.FromBig(helper.GetExponent(thisValue));
        if(bigexp.compareTo(minexp)<0){
          // Use a smaller exponent if the input exponent is already
          // very small
          minexp=FastInteger.Copy(bigexp).SubtractInt(2);
        } else {
          // Ensure the exponent is lower than the exponent range
          // (necessary to flag underflow correctly)
          minexp.SubtractInt(2);
        }
        T quantum=helper.CreateNewWithFlags(
          BigInteger.ONE,minexp.AsBigInteger(),
          (cmp>0) ? BigNumberFlags.FlagNegative : 0);
        T val=thisValue;
        ctx2=ctx.WithRounding((cmp>0) ? Rounding.Floor : Rounding.Ceiling).WithBlankFlags();
        val=Add(val,quantum,ctx2);
        if((ctx2.getFlags()&(PrecisionContext.FlagOverflow|PrecisionContext.FlagUnderflow))==0){
          // Don't set flags except on overflow or underflow
          // TODO: Pending clarification from Mike Cowlishaw,
          // author of the Decimal Arithmetic test cases from
          // speleotrove.com
          ctx2.setFlags(0);
        }
        if((ctx2.getFlags()&(PrecisionContext.FlagUnderflow))!=0){
          BigInteger bigmant=(helper.GetMantissa(val)).abs();
          BigInteger maxmant=helper.MultiplyByRadixPower(
            BigInteger.ONE,FastInteger.FromBig(ctx.getPrecision()).SubtractInt(1));
          if(bigmant.compareTo(maxmant)>=0 || (ctx.getPrecision()).compareTo(BigInteger.ONE)==0){
            // don't treat max-precision results as having underflowed
            ctx2.setFlags(0);
          }
        }
        if(ctx.getHasFlags()){
          ctx.setFlags(ctx.getFlags()|(ctx2.getFlags()));
        }
        return val;
      }
    }
    
    /**
     * 
     * @param thisValue A T object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T NextPlus(
      T thisValue,
      PrecisionContext ctx
     ) {
      if((ctx)==null)throw new NullPointerException("ctx");
      if((ctx.getPrecision()).signum()<=0)throw new IllegalArgumentException("ctx.getPrecision()"+" not less than "+"0"+" ("+(ctx.getPrecision())+")");
      if(!(ctx.getHasExponentRange()))throw new IllegalArgumentException("doesn't satisfy ctx.getHasExponentRange()");
      int flags=helper.GetFlags(thisValue);
      if((flags&BigNumberFlags.FlagSignalingNaN)!=0){
        return SignalingNaNInvalid(thisValue,ctx);
      }
      if((flags&BigNumberFlags.FlagQuietNaN)!=0){
        return ReturnQuietNaN(thisValue,ctx);
      }
      if((flags&BigNumberFlags.FlagInfinity)!=0){
        if((flags&BigNumberFlags.FlagNegative)!=0){
          BigInteger bigexp2 = ctx.getEMax();
          bigexp2=bigexp2.add(BigInteger.ONE);
          bigexp2=bigexp2.subtract(BigInteger.valueOf(ctx.getPrecision()));
          BigInteger overflowMant=helper.MultiplyByRadixPower(
            BigInteger.ONE,FastInteger.FromBig(ctx.getPrecision()));
          overflowMant=overflowMant.subtract(BigInteger.ONE);
          return helper.CreateNewWithFlags(overflowMant,bigexp2,BigNumberFlags.FlagNegative);
        } else {
          return thisValue;
        }
      }
      FastInteger minexp=FastInteger.FromBig(ctx.getEMin()).SubtractBig(ctx.getPrecision()).AddInt(1);
      FastInteger bigexp=FastInteger.FromBig(helper.GetExponent(thisValue));
      if(bigexp.compareTo(minexp)<=0){
        // Use a smaller exponent if the input exponent is already
        // very small
        minexp=FastInteger.Copy(bigexp).SubtractInt(2);
      }
      T quantum=helper.CreateNewWithFlags(
        BigInteger.ONE,
        minexp.AsBigInteger(),0);
      PrecisionContext ctx2;
      T val=thisValue;
      ctx2=ctx.WithRounding(Rounding.Ceiling);
      return Add(val,quantum,ctx2);
    }

    /**
     * Divides two T objects.
     * @param thisValue A T object.
     * @param divisor A T object.
     * @param desiredExponent A BigInteger object.
     * @param ctx A PrecisionContext object. Precision is ignored.
     * @return The quotient of the two objects.
     */
    public T DivideToExponent(
      T thisValue,
      T divisor,
      BigInteger desiredExponent,
      PrecisionContext ctx
     ) {
      if(ctx!=null && !ctx.ExponentWithinRange(desiredExponent))
        return SignalInvalidWithMessage(ctx,"Exponent not within exponent range: "+desiredExponent.toString());
      PrecisionContext ctx2 = (ctx == null) ?
        PrecisionContext.ForRounding(Rounding.HalfDown) :
        ctx.WithUnlimitedExponents().WithPrecision(0);
      T ret = DivideInternal(thisValue, divisor,
                             ctx2,
                             IntegerModeFixedScale, desiredExponent,null);
      if (ctx != null && ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags()|(ctx2.getFlags()));
      }
      return ret;
    }

    /**
     * Divides two T objects.
     * @param thisValue A T object.
     * @param divisor A T object.
     * @param ctx A PrecisionContext object.
     * @return The quotient of the two objects.
     */
    public T Divide(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      return DivideInternal(thisValue, divisor,
                            ctx, IntegerModeRegular, BigInteger.ZERO, null);
    }

    private BigInteger RoundToScale(
      BigInteger mantissa, // Assumes mantissa is nonnegative
      BigInteger remainder,// Assumes value is nonnegative
      BigInteger divisor,// Assumes value is nonnegative
      FastInteger shift,// Number of digits to shift right
      boolean neg,// Whether return value should be negated
      PrecisionContext ctx
     ) {
      IShiftAccumulator accum;
      Rounding rounding = (ctx == null) ? Rounding.HalfEven : ctx.getRounding();
      int lastDiscarded = 0;
      int olderDiscarded = 0;
      if (!(remainder.signum()==0)) {
        if(rounding== Rounding.HalfDown ||
           rounding== Rounding.HalfUp ||
           rounding== Rounding.HalfEven){
          BigInteger halfDivisor = (divisor.shiftRight(1));
          int cmpHalf = remainder.compareTo(halfDivisor);
          if ((cmpHalf == 0) && divisor.testBit(0)==false) {
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
      BigInteger newmantissa=mantissa;
      if(shift.signum()==0){
        if ((lastDiscarded|olderDiscarded) != 0) {
          flags |= PrecisionContext.FlagInexact|PrecisionContext.FlagRounded;
          if (rounding == Rounding.Unnecessary)
            throw new ArithmeticException("Rounding was required");
          if (RoundGivenDigits(lastDiscarded,olderDiscarded,
                               rounding, neg, newmantissa)) {
            newmantissa=newmantissa.add(BigInteger.ONE);
          }
        }
      } else {
        accum = helper.CreateShiftAccumulatorWithDigits(
          mantissa, lastDiscarded, olderDiscarded);
        accum.ShiftRight(shift);
        newmantissa = accum.getShiftedInt();
        if ((accum.getDiscardedDigitCount()).signum() != 0 ||
            (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
          if (mantissa.signum()!=0)
            flags |= PrecisionContext.FlagRounded;
          if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
            flags |= PrecisionContext.FlagInexact|PrecisionContext.FlagRounded;
            if (rounding == Rounding.Unnecessary)
              throw new ArithmeticException("Rounding was required");
          }
          if (RoundGivenBigInt(accum, rounding, neg, newmantissa)) {
            newmantissa=newmantissa.add(BigInteger.ONE);
          }
        }
      }
      if (ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags()|(flags));
      }
      if (neg) {
        newmantissa=newmantissa.negate();
      }
      return newmantissa;
    }

    private static final int IntegerModeFixedScale = 1;
    private static final int IntegerModeRegular = 0;

    private static final int NonTerminatingCheckThreshold = 5;
    
    private T DivideInternal(
      T thisValue,
      T divisor,
      PrecisionContext ctx,
      int integerMode,
      BigInteger desiredExponent,
      T[] remainder
     ) {
      T ret=DivisionHandleSpecial(thisValue,divisor,ctx);
      if((Object)ret!=(Object)null)return ret;
      int signA = helper.GetSign(thisValue);
      int signB = helper.GetSign(divisor);
      if(signB==0){
        if(signA==0){
          return SignalInvalid(ctx);
        }
        return SignalDivideByZero(
          ctx,
          ((helper.GetFlags(thisValue)&BigNumberFlags.FlagNegative)!=0)^
          ((helper.GetFlags(divisor)&BigNumberFlags.FlagNegative)!=0));
      }
      int radix=thisRadix;
      if (signA == 0) {
        T retval=null;
        if (integerMode == IntegerModeFixedScale) {
          int newflags=((helper.GetFlags(thisValue)&BigNumberFlags.FlagNegative))^
            ((helper.GetFlags(divisor)&BigNumberFlags.FlagNegative));
          retval=helper.CreateNewWithFlags(BigInteger.ZERO, desiredExponent,newflags);
        } else {
          BigInteger dividendExp=helper.GetExponent(thisValue);
          BigInteger divisorExp=helper.GetExponent(divisor);
          int newflags=((helper.GetFlags(thisValue)&BigNumberFlags.FlagNegative))^
            ((helper.GetFlags(divisor)&BigNumberFlags.FlagNegative));
          retval=RoundToPrecision(helper.CreateNewWithFlags(
            BigInteger.ZERO, (dividendExp.subtract(divisorExp)),
            newflags), ctx);
        }
        if(remainder!=null){
          remainder[0]=retval;
        }
        return retval;
      } else {
        BigInteger mantissaDividend = (helper.GetMantissa(thisValue)).abs();
        BigInteger mantissaDivisor = (helper.GetMantissa(divisor)).abs();
        FastInteger expDividend = FastInteger.FromBig(helper.GetExponent(thisValue));
        FastInteger expDivisor = FastInteger.FromBig(helper.GetExponent(divisor));
        FastInteger expdiff = FastInteger.Copy(expDividend).Subtract(expDivisor);
        FastInteger adjust = new FastInteger(0);
        FastInteger result = new FastInteger(0);
        FastInteger naturalExponent = FastInteger.Copy(expdiff);
        boolean resultNeg = (helper.GetFlags(thisValue)&BigNumberFlags.FlagNegative)!=
          (helper.GetFlags(divisor)&BigNumberFlags.FlagNegative);
        FastInteger fastPrecision=(ctx==null) ? new FastInteger(0) :
          FastInteger.FromBig(ctx.getPrecision());
        if(integerMode==IntegerModeFixedScale){
          FastInteger shift;
          BigInteger rem;
          FastInteger fastDesiredExponent = FastInteger.FromBig(desiredExponent);
          if(ctx!=null && ctx.getHasFlags() && fastDesiredExponent.compareTo(naturalExponent)>0){
            // Treat as rounded if the desired exponent is greater
            // than the "ideal" exponent
            ctx.setFlags(ctx.getFlags()|(PrecisionContext.FlagRounded));
          }
          if(expdiff.compareTo(fastDesiredExponent)<=0){
            shift=FastInteger.Copy(fastDesiredExponent).Subtract(expdiff);
            BigInteger quo;
BigInteger[] divrem=(mantissaDividend).divideAndRemainder(mantissaDivisor);
quo=divrem[0];
rem=divrem[1];
            quo=RoundToScale(quo,rem,mantissaDivisor,shift,resultNeg,ctx);
            return helper.CreateNewWithFlags(quo,desiredExponent,resultNeg ?
                                             BigNumberFlags.FlagNegative : 0);
          } else if (ctx != null && (ctx.getPrecision()).signum()!=0 &&
                     FastInteger.Copy(expdiff).SubtractInt(8).compareTo(fastPrecision) > 0
                    ) { // NOTE: 8 guard digits
            // Result would require a too-high precision since
            // exponent difference is much higher
            return SignalInvalidWithMessage(ctx,"Result can't fit the precision");
          } else {
            shift=FastInteger.Copy(expdiff).Subtract(fastDesiredExponent);
            mantissaDividend=helper.MultiplyByRadixPower(mantissaDividend,shift);
            BigInteger quo;
BigInteger[] divrem=(mantissaDividend).divideAndRemainder(mantissaDivisor);
quo=divrem[0];
rem=divrem[1];
            quo=RoundToScale(quo,rem,mantissaDivisor,new FastInteger(0),resultNeg,ctx);
            return helper.CreateNewWithFlags(quo,desiredExponent,resultNeg ?
                                             BigNumberFlags.FlagNegative : 0);
          }
        }
        FastInteger resultPrecision = new FastInteger(1);
        int mantcmp = mantissaDividend.compareTo(mantissaDivisor);
        if (mantcmp < 0) {
          // dividend mantissa is less than divisor mantissa
          FastInteger dividendPrecision =
            helper.CreateShiftAccumulator(mantissaDividend).GetDigitLength();
          FastInteger divisorPrecision =
            helper.CreateShiftAccumulator(mantissaDivisor).GetDigitLength();
          divisorPrecision.Subtract(dividendPrecision);
          if(divisorPrecision.signum()==0)
            divisorPrecision.AddInt(1);
          // multiply dividend mantissa so precisions are the same
          // (except if they're already the same, in which case multiply
          // by radix)
          mantissaDividend = helper.MultiplyByRadixPower(
            mantissaDividend, divisorPrecision);
          adjust.Add(divisorPrecision);
          if (mantissaDividend.compareTo(mantissaDivisor) < 0) {
            // dividend mantissa is still less, multiply once more
            if(radix==2){
              mantissaDividend=mantissaDividend.shiftLeft(1);
            } else {
              mantissaDividend=mantissaDividend.multiply(BigInteger.valueOf(radix));
            }
            adjust.AddInt(1);
          }
        } else if (mantcmp > 0) {
          // dividend mantissa is greater than divisor mantissa
          FastInteger dividendPrecision =
            helper.CreateShiftAccumulator(mantissaDividend).GetDigitLength();
          FastInteger divisorPrecision =
            helper.CreateShiftAccumulator(mantissaDivisor).GetDigitLength();
          dividendPrecision.Subtract(divisorPrecision);
          BigInteger oldMantissaB = mantissaDivisor;
          mantissaDivisor = helper.MultiplyByRadixPower(
            mantissaDivisor, dividendPrecision);
          adjust.Subtract(dividendPrecision);
          if (mantissaDividend.compareTo(mantissaDivisor) < 0) {
            // dividend mantissa is now less, divide by radix power
            if (dividendPrecision.CompareToInt(1)==0) {
              // no need to divide here, since that would just undo
              // the multiplication
              mantissaDivisor = oldMantissaB;
            } else {
              BigInteger bigpow = BigInteger.valueOf(radix);
              mantissaDivisor=mantissaDivisor.divide(bigpow);
            }
            adjust.AddInt(1);
          }
        }
        boolean atMaxPrecision=false;
        if (mantcmp == 0) {
          result = new FastInteger(1);
          mantissaDividend = BigInteger.ZERO;
        } else {
          int check = 0;
          FastInteger divs=FastInteger.FromBig(mantissaDivisor);
          FastInteger divd=FastInteger.FromBig(mantissaDividend);
          FastInteger divsHalfRadix=null;
          if(radix!=2){
            divsHalfRadix=FastInteger.FromBig(mantissaDivisor).Multiply(radix/2);
          }
          boolean hasPrecision=ctx != null && (ctx.getPrecision()).signum()!=0;
          while (true) {
            boolean remainderZero=false;
            if (check == NonTerminatingCheckThreshold && !hasPrecision &&
                integerMode == IntegerModeRegular) {
              // Check for a non-terminating radix expansion
              // if using unlimited precision and not in integer
              // mode
              if (!helper.HasTerminatingRadixExpansion(
                divd.AsBigInteger(), mantissaDivisor)) {
                throw new ArithmeticException("Result would have a nonterminating expansion");
              }
              check++;
            } else if (check < NonTerminatingCheckThreshold) {
              check++;
            }
            int count=0;
            if(divsHalfRadix!=null && divd.compareTo(divsHalfRadix)>=0){
              divd.Subtract(divsHalfRadix);
              count+=radix/2;
            }
            while(divd.compareTo(divs)>=0){
              divd.Subtract(divs);
              count++;
            }
            result.AddInt(count);
            remainderZero=(divd.signum()==0);
            if (hasPrecision && resultPrecision.compareTo(fastPrecision) == 0) {
              mantissaDividend=divd.AsBigInteger();
              atMaxPrecision=true;
              break;
            }
            if (remainderZero && adjust.signum() >= 0) {
              mantissaDividend=divd.AsBigInteger();
              break;
            }
            adjust.AddInt(1);
            if (result.signum() != 0) {
              resultPrecision.AddInt(1);
            }
            result.Multiply(radix);
            divd.Multiply(radix);
          }
        }
        // mantissaDividend now has the remainder
        FastInteger exp = FastInteger.Copy(expdiff).Subtract(adjust);
        Rounding rounding=(ctx==null) ? Rounding.HalfEven : ctx.getRounding();
        int lastDiscarded = 0;
        int olderDiscarded = 0;
        if (!(mantissaDividend.signum()==0)) {
          if(rounding==Rounding.HalfDown ||
             rounding==Rounding.HalfEven ||
             rounding==Rounding.HalfUp
            ){
            BigInteger halfDivisor = (mantissaDivisor.shiftRight(1));
            int cmpHalf = mantissaDividend.compareTo(halfDivisor);
            if ((cmpHalf == 0) && mantissaDivisor.testBit(0)==false) {
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
            if(rounding==Rounding.Unnecessary)
              throw new ArithmeticException("Rounding was required");
            lastDiscarded=1;
            olderDiscarded=1;
          }
        }
        BigInteger bigResult = result.AsBigInteger();
        BigInteger posBigResult=bigResult;
        if(ctx!=null && ctx.getHasFlags() && exp.compareTo(naturalExponent)>0){
          // Treat as rounded if the true exponent is greater
          // than the "ideal" exponent
          ctx.setFlags(ctx.getFlags()|(PrecisionContext.FlagRounded));
        }
        BigInteger bigexp=exp.AsBigInteger();
        T retval=helper.CreateNewWithFlags(
          bigResult, bigexp, resultNeg ? BigNumberFlags.FlagNegative : 0);
        if(atMaxPrecision && !ctx.getHasExponentRange()){
          // At this point, the check for rounding with Rounding.Unnecessary
          // already occurred above
          if(!RoundGivenDigits(lastDiscarded,olderDiscarded,rounding,resultNeg,posBigResult)){
            if(ctx!=null && ctx.getHasFlags() && (lastDiscarded|olderDiscarded)!=0){
              ctx.setFlags(ctx.getFlags()|(PrecisionContext.FlagInexact|PrecisionContext.FlagRounded));
            }
            return retval;
          } else if(posBigResult.testBit(0)==false && (thisRadix&1)==0){
            posBigResult=posBigResult.add(BigInteger.ONE);
            if(ctx!=null && ctx.getHasFlags() && (lastDiscarded|olderDiscarded)!=0){
              ctx.setFlags(ctx.getFlags()|(PrecisionContext.FlagInexact|PrecisionContext.FlagRounded));
            }
            return helper.CreateNewWithFlags(posBigResult,bigexp,
                                             resultNeg ? BigNumberFlags.FlagNegative : 0);
          }
        }
        if(atMaxPrecision && ctx.getHasExponentRange()){
          BigInteger fastAdjustedExp = FastInteger.Copy(exp)
            .AddBig(ctx.getPrecision()).SubtractInt(1).AsBigInteger();
          if(fastAdjustedExp.compareTo(ctx.getEMin())>=0 && fastAdjustedExp.compareTo(ctx.getEMax())<=0){
            // At this point, the check for rounding with Rounding.Unnecessary
            // already occurred above
            if(!RoundGivenDigits(lastDiscarded,olderDiscarded,rounding,resultNeg,posBigResult)){
              if(ctx!=null && ctx.getHasFlags() && (lastDiscarded|olderDiscarded)!=0){
                ctx.setFlags(ctx.getFlags()|(PrecisionContext.FlagInexact|PrecisionContext.FlagRounded));
              }
              return retval;
            } else if(posBigResult.testBit(0)==false && (thisRadix&1)==0){
              posBigResult=posBigResult.add(BigInteger.ONE);
              if(ctx!=null && ctx.getHasFlags() && (lastDiscarded|olderDiscarded)!=0){
                ctx.setFlags(ctx.getFlags()|(PrecisionContext.FlagInexact|PrecisionContext.FlagRounded));
              }
              return helper.CreateNewWithFlags(posBigResult,bigexp,
                                               resultNeg ? BigNumberFlags.FlagNegative : 0);
            }
          }
        }
        return RoundToPrecisionWithShift(
          retval,
          ctx,
          lastDiscarded, olderDiscarded, new FastInteger(0),false);
      }
    }

    /**
     * Gets the lesser value between two values, ignoring their signs. If
     * the absolute values are equal, has the same effect as Min.
     * @param a A T object.
     * @param b A T object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T MinMagnitude(T a, T b, PrecisionContext ctx) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      // Handle infinity and NaN
      T result=MinMaxHandleSpecial(a,b,ctx,true,true);
      if((Object)result!=(Object)null)return result;
      int cmp = compareTo(AbsRaw(a), AbsRaw(b));
      if (cmp == 0) return Min(a, b, ctx);
      return (cmp < 0) ? RoundToPrecision(a,ctx) :
        RoundToPrecision(b,ctx);
    }
    /**
     * Gets the greater value between two values, ignoring their signs.
     * If the absolute values are equal, has the same effect as Max.
     * @param a A T object.
     * @param b A T object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T MaxMagnitude(T a, T b, PrecisionContext ctx) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      // Handle infinity and NaN
      T result=MinMaxHandleSpecial(a,b,ctx,false,true);
      if((Object)result!=(Object)null)return result;
      int cmp = compareTo(AbsRaw(a), AbsRaw(b));
      if (cmp == 0) return Max(a, b, ctx);
      return (cmp > 0) ? RoundToPrecision(a,ctx) :
        RoundToPrecision(b,ctx);
    }
    /**
     * Gets the greater value between two T values.
     * @param a A T object.
     * @param b A T object.
     * @param ctx A PrecisionContext object.
     * @return The larger value of the two objects.
     */
    public T Max(T a, T b, PrecisionContext ctx) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      // Handle infinity and NaN
      T result=MinMaxHandleSpecial(a,b,ctx,false,false);
      if((Object)result!=(Object)null)return result;
      int cmp = compareTo(a, b);
      if (cmp != 0)
        return cmp < 0 ? RoundToPrecision(b,ctx) :
          RoundToPrecision(a,ctx);
      int flagNegA=(helper.GetFlags(a)&BigNumberFlags.FlagNegative);
      if(flagNegA!=(helper.GetFlags(b)&BigNumberFlags.FlagNegative)){
        return (flagNegA!=0) ? RoundToPrecision(b,ctx) :
          RoundToPrecision(a,ctx);
      }
      if (flagNegA==0) {
        return helper.GetExponent(a).compareTo(helper.GetExponent(b)) > 0 ? RoundToPrecision(a,ctx) :
          RoundToPrecision(b,ctx);
      } else {
        return helper.GetExponent(a).compareTo(helper.GetExponent(b)) > 0 ? RoundToPrecision(b,ctx) :
          RoundToPrecision(a,ctx);
      }
    }

    /**
     * Gets the lesser value between two T values.
     * @param a A T object.
     * @param b A T object.
     * @param ctx A PrecisionContext object.
     * @return The smaller value of the two objects.
     */
    public T Min(T a, T b, PrecisionContext ctx) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      // Handle infinity and NaN
      T result=MinMaxHandleSpecial(a,b,ctx,true,false);
      if((Object)result!=(Object)null)return result;
      int cmp = compareTo(a, b);
      if (cmp != 0)
        return cmp > 0 ? RoundToPrecision(b,ctx) :
          RoundToPrecision(a,ctx);
      int signANeg=helper.GetFlags(a)&BigNumberFlags.FlagNegative;
      if(signANeg!=(helper.GetFlags(b)&BigNumberFlags.FlagNegative)){
        return (signANeg!=0) ? RoundToPrecision(a,ctx) :
          RoundToPrecision(b,ctx);
      }
      if (signANeg==0) {
        return (helper.GetExponent(a)).compareTo(helper.GetExponent(b)) > 0 ? RoundToPrecision(b,ctx) :
          RoundToPrecision(a,ctx);
      } else {
        return (helper.GetExponent(a)).compareTo(helper.GetExponent(b)) > 0 ? RoundToPrecision(a,ctx) :
          RoundToPrecision(b,ctx);
      }
    }

    /**
     * Multiplies two T objects.
     * @param thisValue A T object.
     * @param ctx A PrecisionContext object.
     * @param other A T object.
     * @return The product of the two objects.
     */
    public T Multiply(T thisValue, T other, PrecisionContext ctx) {
      int thisFlags=helper.GetFlags(thisValue);
      int otherFlags=helper.GetFlags(other);
      if(((thisFlags|otherFlags)&BigNumberFlags.FlagSpecial)!=0){
        T result=HandleNotANumber(thisValue,other,ctx);
        if((Object)result!=(Object)null)return result;
        if((thisFlags&BigNumberFlags.FlagInfinity)!=0){
          // Attempt to multiply infinity by 0
          if((otherFlags&BigNumberFlags.FlagSpecial)==0 && helper.GetMantissa(other).signum()==0)
            return SignalInvalid(ctx);
          return EnsureSign(thisValue,((thisFlags&BigNumberFlags.FlagNegative)^(otherFlags&BigNumberFlags.FlagNegative))!=0);
        }
        if((otherFlags&BigNumberFlags.FlagInfinity)!=0){
          // Attempt to multiply infinity by 0
          if((thisFlags&BigNumberFlags.FlagSpecial)==0 && helper.GetMantissa(thisValue).signum()==0)
            return SignalInvalid(ctx);
          return EnsureSign(other,((thisFlags&BigNumberFlags.FlagNegative)^(otherFlags&BigNumberFlags.FlagNegative))!=0);
        }
      }
      BigInteger bigintOp2 = helper.GetExponent(other);
      BigInteger newexp = (helper.GetExponent(thisValue).add(bigintOp2));
      thisFlags=(thisFlags&BigNumberFlags.FlagNegative)^(otherFlags&BigNumberFlags.FlagNegative);
      T ret = helper.CreateNewWithFlags(
        helper.GetMantissa(thisValue).multiply(helper.GetMantissa(other)), newexp,
        thisFlags
       );
      if (ctx != null) {
        ret = RoundToPrecision(ret, ctx);
      }
      return ret;
    }
    /**
     * 
     * @param thisValue A T object.
     * @param multiplicand A T object.
     * @param augend A T object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T MultiplyAndAdd(T thisValue, T multiplicand,
                            T augend,
                            PrecisionContext ctx) {
      PrecisionContext ctx2=PrecisionContext.Unlimited.WithBlankFlags();
      T ret=Add(Multiply(thisValue,multiplicand,ctx2), augend, ctx);
      if(ctx.getHasFlags())ctx.setFlags(ctx.getFlags()|(ctx2.getFlags()));
      return ret;
    }

    /**
     * 
     * @param thisValue A T object.
     * @param context A PrecisionContext object.
     * @return A T object.
     */
    public T RoundToBinaryPrecision(
      T thisValue,
      PrecisionContext context
     ) {
      return RoundToBinaryPrecisionWithShift(thisValue, context, 0, 0, new FastInteger(0),false);
    }
    private T RoundToBinaryPrecisionWithShift(
      T thisValue,
      PrecisionContext context,
      int lastDiscarded,
      int olderDiscarded,
      FastInteger shift,
      boolean adjustNegativeZero
     ) {
      if ((context) == null) return thisValue;
      if ((context.getPrecision()).signum()==0 && !context.getHasExponentRange() &&
          (lastDiscarded | olderDiscarded) == 0 && shift.signum()==0)
        return thisValue;
      if((context.getPrecision()).signum()==0 || thisRadix==2)
        return RoundToPrecisionWithShift(thisValue,context,lastDiscarded,olderDiscarded,shift,false);
      FastInteger fastEMin = (context.getHasExponentRange()) ? FastInteger.FromBig(context.getEMin()) : null;
      FastInteger fastEMax = (context.getHasExponentRange()) ? FastInteger.FromBig(context.getEMax()) : null;
      FastInteger fastPrecision = FastInteger.FromBig(context.getPrecision());
      int[] signals = new int[1];
      T dfrac = RoundToPrecisionInternal(
        thisValue,fastPrecision,
        context.getRounding(), fastEMin, fastEMax,
        lastDiscarded,
        olderDiscarded,
        shift,true,false,
        signals);
      // Clamp exponents to eMax + 1 - precision
      // if directed
      if (context.getClampNormalExponents() && dfrac != null) {
        FastInteger digitCount=null;
        if(thisRadix==2){
          digitCount=FastInteger.Copy(fastPrecision);
        } else {
          // TODO: Use a faster way to get the digit
          // count for radix 10
          BigInteger maxMantissa = BigInteger.ONE;
          FastInteger prec=FastInteger.Copy(fastPrecision);
          while(prec.signum()>0){
            int bitShift=prec.CompareToInt(1000000)>=0 ? 1000000 : prec.AsInt32();
            maxMantissa=maxMantissa.shiftLeft(bitShift);
            prec.SubtractInt(bitShift);
          }
          maxMantissa=maxMantissa.subtract(BigInteger.ONE);
          // Get the digit length of the maximum possible mantissa
          // for the given binary precision
          digitCount=helper.CreateShiftAccumulator(maxMantissa)
            .GetDigitLength();
        }
        FastInteger clamp = FastInteger.Copy(fastEMax).AddInt(1).Subtract(digitCount);
        FastInteger fastExp = FastInteger.FromBig(helper.GetExponent(dfrac));
        if (fastExp.compareTo(clamp) > 0) {
          boolean neg=(helper.GetFlags(dfrac)&BigNumberFlags.FlagNegative)!=0;
          BigInteger bigmantissa = (helper.GetMantissa(dfrac)).abs();
          if (!(bigmantissa.signum()==0)) {
            FastInteger expdiff = FastInteger.Copy(fastExp).Subtract(clamp);
            bigmantissa = helper.MultiplyByRadixPower(bigmantissa, expdiff);
          }
          if (signals != null)
            signals[0] |= PrecisionContext.FlagClamped;
          dfrac = helper.CreateNewWithFlags(bigmantissa, clamp.AsBigInteger(),
                                            neg ? BigNumberFlags.FlagNegative : 0);
        }
      }
      if (context.getHasFlags()) {
        context.setFlags(context.getFlags()|(signals[0]));
      }
      return dfrac;
    }
    
    /**
     * 
     * @param thisValue A T object.
     * @param context A PrecisionContext object.
     * @return A T object.
     */
    public T Plus(
      T thisValue,
      PrecisionContext context
     ) {
      return RoundToPrecisionWithShift(thisValue, context, 0, 0,new FastInteger(0), true);
    }
    /**
     * 
     * @param thisValue A T object.
     * @param context A PrecisionContext object.
     * @return A T object.
     */
    public T RoundToPrecision(
      T thisValue,
      PrecisionContext context
     ) {
      return RoundToPrecisionWithShift(thisValue, context, 0, 0,new FastInteger(0), false);
    }
    private T RoundToPrecisionWithShift(
      T thisValue,
      PrecisionContext context,
      int lastDiscarded,
      int olderDiscarded,
      FastInteger shift,
      boolean adjustNegativeZero
     ) {
      if ((context) == null) return thisValue;
      // If context has unlimited precision and exponent range,
      // and no discarded digits or shifting
      if ((context.getPrecision()).signum()==0 && !context.getHasExponentRange() &&
          (lastDiscarded | olderDiscarded) == 0 && shift.signum()==0)
        return thisValue;
      FastInteger fastEMin = (context.getHasExponentRange()) ? FastInteger.FromBig(context.getEMin()) : null;
      FastInteger fastEMax = (context.getHasExponentRange()) ? FastInteger.FromBig(context.getEMax()) : null;
      FastInteger fastPrecision=FastInteger.FromBig(context.getPrecision());
      int thisFlags=helper.GetFlags(thisValue);
      if (fastPrecision.signum() > 0 && fastPrecision.CompareToInt(34) <= 0 &&
          (shift==null || shift.signum()==0) &&
          (thisFlags&BigNumberFlags.FlagSpecial)==0) {
        // Check if rounding is necessary at all
        // for small precisions
        BigInteger mantabs = (helper.GetMantissa(thisValue)).abs();
        BigInteger radixPower=helper.MultiplyByRadixPower(BigInteger.ONE, fastPrecision);
        if(adjustNegativeZero &&
           (thisFlags&BigNumberFlags.FlagNegative)!=0 && mantabs.signum()==0 &&
           (context.getRounding()!= Rounding.Floor)){
          // Change negative zero to positive zero
          // except if the rounding mode is Floor
          thisValue=EnsureSign(thisValue,false);
          thisFlags=0;
        }
        if (mantabs.compareTo(radixPower) < 0) {
          if(!RoundGivenDigits(lastDiscarded,olderDiscarded,context.getRounding(),
                               (thisFlags&BigNumberFlags.FlagNegative)!=0,mantabs)){
            if(context.getHasFlags() && (lastDiscarded|olderDiscarded)!=0){
              context.setFlags(context.getFlags()|(PrecisionContext.FlagInexact|PrecisionContext.FlagRounded));
            }
            if (!context.getHasExponentRange())
              return thisValue;
            FastInteger fastExp = FastInteger.FromBig(helper.GetExponent(thisValue));
            FastInteger fastAdjustedExp = FastInteger.Copy(fastExp)
              .Add(fastPrecision).SubtractInt(1);
            FastInteger fastNormalMin = FastInteger.Copy(fastEMin)
              .Add(fastPrecision).SubtractInt(1);
            if (fastAdjustedExp.compareTo(fastEMax) <= 0 &&
                fastAdjustedExp.compareTo(fastNormalMin) >= 0) {
              return thisValue;
            }
          } else {
            if(context.getHasFlags() && (lastDiscarded|olderDiscarded)!=0){
              context.setFlags(context.getFlags()|(PrecisionContext.FlagInexact|PrecisionContext.FlagRounded));
            }
            mantabs=mantabs.add(BigInteger.ONE);
            if(mantabs.compareTo(radixPower)<0){
              if (!context.getHasExponentRange())
                return helper.CreateNewWithFlags(mantabs,helper.GetExponent(thisValue),thisFlags);
              FastInteger fastExp = FastInteger.FromBig(helper.GetExponent(thisValue));
              FastInteger fastAdjustedExp = FastInteger.Copy(fastExp)
                .Add(fastPrecision).SubtractInt(1);
              FastInteger fastNormalMin = FastInteger.Copy(fastEMin)
                .Add(fastPrecision).SubtractInt(1);
              if (fastAdjustedExp.compareTo(fastEMax) <= 0 &&
                  fastAdjustedExp.compareTo(fastNormalMin) >= 0) {
                return helper.CreateNewWithFlags(mantabs,helper.GetExponent(thisValue),thisFlags);
              }
            }
          }
        }
      }
      int[] signals = new int[1];
      T dfrac = RoundToPrecisionInternal(
        thisValue,fastPrecision,
        context.getRounding(), fastEMin, fastEMax,
        lastDiscarded,
        olderDiscarded,
        shift,false,adjustNegativeZero,
        context.getHasFlags() ? signals : null);
      if (context.getClampNormalExponents() && dfrac != null) {
        // Clamp exponents to eMax + 1 - precision
        // if directed
        FastInteger clamp = FastInteger.Copy(fastEMax).AddInt(1).Subtract(fastPrecision);
        FastInteger fastExp = FastInteger.FromBig(helper.GetExponent(dfrac));
        if (fastExp.compareTo(clamp) > 0) {
          boolean neg=(helper.GetFlags(dfrac)&BigNumberFlags.FlagNegative)!=0;
          BigInteger bigmantissa = (helper.GetMantissa(dfrac)).abs();
          if (!(bigmantissa.signum()==0)) {
            FastInteger expdiff = FastInteger.Copy(fastExp).Subtract(clamp);
            bigmantissa = helper.MultiplyByRadixPower(bigmantissa, expdiff);
          }
          if (signals != null)
            signals[0] |= PrecisionContext.FlagClamped;
          dfrac = helper.CreateNewWithFlags(bigmantissa, clamp.AsBigInteger(),
                                            neg ? BigNumberFlags.FlagNegative : 0);
        }
      }
      if (context.getHasFlags()) {
        context.setFlags(context.getFlags()|(signals[0]));
      }
      return dfrac;
    }

    /**
     * 
     * @param thisValue A T object.
     * @param otherValue A T object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Quantize(
      T thisValue,
      T otherValue,
      PrecisionContext ctx
     ) {
      int thisFlags=helper.GetFlags(thisValue);
      int otherFlags=helper.GetFlags(otherValue);
      if(((thisFlags|otherFlags)&BigNumberFlags.FlagSpecial)!=0){
        T result=HandleNotANumber(thisValue,otherValue,ctx);
        if((Object)result!=(Object)null)return result;
        if(((thisFlags&otherFlags)&BigNumberFlags.FlagInfinity)!=0){
          return RoundToPrecision(thisValue,ctx);
        }
        if(((thisFlags|otherFlags)&BigNumberFlags.FlagInfinity)!=0){
          return SignalInvalid(ctx);
        }
      }
      BigInteger expOther = helper.GetExponent(otherValue);
      if(ctx!=null && !ctx.ExponentWithinRange(expOther))
        return SignalInvalidWithMessage(ctx,"Exponent not within exponent range: "+expOther.toString());
      PrecisionContext tmpctx = (ctx == null ?
                                 PrecisionContext.ForRounding(Rounding.HalfEven) :
                                 ctx.Copy()).WithBlankFlags();
      BigInteger mantThis = (helper.GetMantissa(thisValue)).abs();
      BigInteger expThis = helper.GetExponent(thisValue);
      int expcmp = expThis.compareTo(expOther);
      int negativeFlag=(helper.GetFlags(thisValue)&BigNumberFlags.FlagNegative);
      T ret = null;
      if (expcmp == 0) {
        ret = RoundToPrecision(thisValue, tmpctx);
      } else if (mantThis.signum()==0) {
        ret = helper.CreateNewWithFlags(BigInteger.ZERO, expOther,negativeFlag);
        ret = RoundToPrecision(ret, tmpctx);
      } else if (expcmp > 0) {
        // Other exponent is less
        FastInteger radixPower = FastInteger.FromBig(expThis).SubtractBig(expOther);
        if ((tmpctx.getPrecision()).signum()>0 &&
            radixPower.compareTo(FastInteger.FromBig(tmpctx.getPrecision()).AddInt(10)) > 0) {
          // Radix power is much too high for the current precision
          return SignalInvalidWithMessage(ctx,"Result too high for current precision");
        }
        mantThis = helper.MultiplyByRadixPower(mantThis, radixPower);
        ret = helper.CreateNewWithFlags(mantThis, expOther, negativeFlag);
        ret = RoundToPrecision(ret, tmpctx);
      } else {
        // Other exponent is greater
        FastInteger shift=FastInteger.FromBig(expOther).SubtractBig(expThis);
        ret = RoundToPrecisionWithShift(thisValue, tmpctx, 0, 0, shift, false);
      }
      if ((tmpctx.getFlags() & PrecisionContext.FlagOverflow) != 0) {
        return SignalInvalid(ctx);
      }
      if (ret == null || !helper.GetExponent(ret).equals(expOther)) {
        return SignalInvalid(ctx);
      }
      ret=EnsureSign(ret,negativeFlag!=0);
      if (ctx != null && ctx.getHasFlags()) {
        int flags = tmpctx.getFlags();
        flags &= ~PrecisionContext.FlagUnderflow;
        ctx.setFlags(ctx.getFlags()|(flags));
      }
      return ret;
    }
    
    
    
    /**
     * 
     * @param thisValue A T object.
     * @param expOther A BigInteger object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T RoundToExponentExact(
      T thisValue,
      BigInteger expOther,
      PrecisionContext ctx) {
      if (helper.GetExponent(thisValue).compareTo(expOther) >= 0) {
        return RoundToPrecision(thisValue, ctx);
      } else {
        PrecisionContext pctx = (ctx == null) ? null :
          ctx.WithPrecision(0).WithBlankFlags();
        T ret = Quantize(thisValue, helper.CreateNewWithFlags(
          BigInteger.ONE, expOther, 0),
                         pctx);
        if (ctx != null && ctx.getHasFlags()) {
          ctx.setFlags(ctx.getFlags()|(pctx.getFlags()));
        }
        return ret;
      }
    }

    /**
     * 
     * @param thisValue A T object.
     * @param expOther A BigInteger object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T RoundToExponentSimple(
      T thisValue,
      BigInteger expOther,
      PrecisionContext ctx) {
      int thisFlags=helper.GetFlags(thisValue);
      if((thisFlags&BigNumberFlags.FlagSpecial)!=0){
        T result=HandleNotANumber(thisValue,thisValue,ctx);
        if((Object)result!=(Object)null)return result;
        if((thisFlags&BigNumberFlags.FlagInfinity)!=0){
          return thisValue;
        }
      }
      if (helper.GetExponent(thisValue).compareTo(expOther) >= 0) {
        return RoundToPrecision(thisValue, ctx);
      } else {
        if(ctx!=null && !ctx.ExponentWithinRange(expOther))
          return SignalInvalidWithMessage(ctx,"Exponent not within exponent range: "+expOther.toString());
        BigInteger bigmantissa=(helper.GetMantissa(thisValue)).abs();
        FastInteger shift=FastInteger.FromBig(expOther).SubtractBig(helper.GetExponent(thisValue));
        IShiftAccumulator accum=helper.CreateShiftAccumulator(bigmantissa);
        accum.ShiftRight(shift);
        bigmantissa=accum.getShiftedInt();
        return RoundToPrecisionWithShift(
          helper.CreateNewWithFlags(bigmantissa,expOther,thisFlags),ctx,
          accum.getLastDiscardedDigit(),
          accum.getOlderDiscardedDigits(), new FastInteger(0), false);
      }
    }

    /**
     * 
     * @param thisValue A T object.
     * @param ctx A PrecisionContext object.
     * @param exponent A BigInteger object.
     * @return A T object.
     */
    public T RoundToExponentNoRoundedFlag(
      T thisValue,
      BigInteger exponent,
      PrecisionContext ctx
     ) {
      PrecisionContext pctx = (ctx == null) ? null :
        ctx.WithBlankFlags();
      T ret = RoundToExponentExact(thisValue, exponent, pctx);
      if (ctx != null && ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags()|((pctx.getFlags()&~(PrecisionContext.FlagInexact|PrecisionContext.FlagRounded))));
      }
      return ret;
    }

    /**
     * 
     * @param thisValue A T object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T Reduce(
      T thisValue,
      PrecisionContext ctx
     ) {
      T ret = RoundToPrecision(thisValue, ctx);
      if(ret!=null && (helper.GetFlags(ret)&BigNumberFlags.FlagSpecial)==0){
        BigInteger bigmant=(helper.GetMantissa(ret)).abs();
        FastInteger exp=FastInteger.FromBig(helper.GetExponent(ret));
        if(bigmant.signum()==0){
          exp=new FastInteger(0);
        } else {
          int radix=thisRadix;
          BigInteger bigradix=BigInteger.valueOf(radix);
          while(!(bigmant.signum()==0)){
            BigInteger bigrem;
            BigInteger bigquo;
BigInteger[] divrem=(bigmant).divideAndRemainder(bigradix);
bigquo=divrem[0];
bigrem=divrem[1];
            if(bigrem.signum()!=0)
              break;
            bigmant=bigquo;
            exp.AddInt(1);
          }
        }
        int flags=helper.GetFlags(thisValue);
        ret=helper.CreateNewWithFlags(bigmant,exp.AsBigInteger(),flags);
        if(ctx!=null && ctx.getClampNormalExponents()){
          PrecisionContext ctxtmp=ctx.WithBlankFlags();
          ret=RoundToPrecision(ret,ctxtmp);
          if(ctx.getHasFlags()){
            ctx.setFlags(ctx.getFlags()|((ctxtmp.getFlags()&~PrecisionContext.FlagClamped)));
          }
        }
        ret=EnsureSign(ret,(flags&BigNumberFlags.FlagNegative)!=0);
      }
      return ret;
    }
    
    private T RoundToPrecisionInternal(
      T thisValue,
      FastInteger precision,
      Rounding rounding,
      FastInteger fastEMin,
      FastInteger fastEMax,
      int lastDiscarded,
      int olderDiscarded,
      FastInteger shift,
      boolean binaryPrec, // whether "precision" is the number of bits, not digits
      boolean adjustNegativeZero,
      int[] signals
     ) {
      if (precision.signum() < 0) throw new IllegalArgumentException("precision" + " not greater or equal to " + "0" + " (" + precision + ")");
      if(thisRadix==2 || precision.signum()==0){
        // "binaryPrec" will have no special effect here
        binaryPrec=false;
      }
      int thisFlags=helper.GetFlags(thisValue);
      if((thisFlags&BigNumberFlags.FlagSpecial)!=0){
        if((thisFlags&BigNumberFlags.FlagSignalingNaN)!=0){
          if(signals!=null){
            signals[0]|=PrecisionContext.FlagInvalid;
          }
          return ReturnQuietNaNFastIntPrecision(thisValue,precision);
        }
        if((thisFlags&BigNumberFlags.FlagQuietNaN)!=0){
          return ReturnQuietNaNFastIntPrecision(thisValue,precision);
        }
        if((thisFlags&BigNumberFlags.FlagInfinity)!=0){
          return thisValue;
        }
      }
      if(adjustNegativeZero &&
         (thisFlags&BigNumberFlags.FlagNegative)!=0 && helper.GetMantissa(thisValue).signum()==0 &&
         (rounding!= Rounding.Floor)){
        // Change negative zero to positive zero
        // except if the rounding mode is Floor
        thisValue=EnsureSign(thisValue,false);
        thisFlags=0;
      }
      boolean neg=(thisFlags&BigNumberFlags.FlagNegative)!=0;
      BigInteger bigmantissa = (helper.GetMantissa(thisValue)).abs();
      // save mantissa in case result is subnormal
      // and must be rounded again
      BigInteger oldmantissa = bigmantissa;
      boolean mantissaWasZero=(oldmantissa.signum()==0 && (lastDiscarded|olderDiscarded)==0);
      BigInteger maxMantissa=BigInteger.ONE;
      FastInteger exp = FastInteger.FromBig(helper.GetExponent(thisValue));
      int flags = 0;
      IShiftAccumulator accum = helper.CreateShiftAccumulatorWithDigits(
        bigmantissa, lastDiscarded, olderDiscarded);
      boolean unlimitedPrec = (precision.signum()==0);
      FastInteger fastPrecision=precision;
      if(binaryPrec){
        FastInteger prec=FastInteger.Copy(precision);
        while(prec.signum()>0){
          int bitShift=(prec.CompareToInt(1000000)>=0) ? 1000000 : prec.AsInt32();
          maxMantissa=maxMantissa.shiftLeft(bitShift);
          prec.SubtractInt(bitShift);
        }
        maxMantissa=maxMantissa.subtract(BigInteger.ONE);
        IShiftAccumulator accumMaxMant = helper.CreateShiftAccumulator(
          maxMantissa);
        // Get the digit length of the maximum possible mantissa
        // for the given binary precision
        fastPrecision=accumMaxMant.GetDigitLength();
      } else {
        fastPrecision=precision;
      }
      
      if(shift!=null){
        accum.ShiftRight(shift);
      }
      if (!unlimitedPrec) {
        accum.ShiftToDigits(fastPrecision);
      } else {
        fastPrecision = accum.GetDigitLength();
      }
      if(binaryPrec){
        while((accum.getShiftedInt()).compareTo(maxMantissa)>0){
          accum.ShiftRightInt(1);
        }
      }
      FastInteger discardedBits = FastInteger.Copy(accum.getDiscardedDigitCount());
      exp.Add(discardedBits);
      FastInteger adjExponent = FastInteger.Copy(exp)
        .Add(accum.GetDigitLength()).SubtractInt(1);
      //System.out.println("{0}->{1} digits={2} exp={3} [curexp={4}] adj={5},max={6}",bigmantissa,accum.getShiftedInt(),
      //              accum.getDiscardedDigitCount(),exp,helper.GetExponent(thisValue),adjExponent,fastEMax);
      FastInteger newAdjExponent = adjExponent;
      FastInteger clamp = null;
      BigInteger earlyRounded=BigInteger.ZERO;
      if(binaryPrec && fastEMax!=null && adjExponent.compareTo(fastEMax)==0){
        // May or may not be an overflow depending on the mantissa
        FastInteger expdiff=FastInteger.Copy(fastPrecision).Subtract(accum.GetDigitLength());
        BigInteger currMantissa=accum.getShiftedInt();
        currMantissa=helper.MultiplyByRadixPower(currMantissa,expdiff);
        if((currMantissa).compareTo(maxMantissa)>0){
          // Mantissa too high, treat as overflow
          adjExponent.AddInt(1);
        }
      }
      //System.out.println("{0} adj={1} emin={2}",thisValue,adjExponent,fastEMin);
      if(signals!=null && fastEMin != null && adjExponent.compareTo(fastEMin) < 0){
        earlyRounded=accum.getShiftedInt();
        if(RoundGivenBigInt(accum, rounding, neg, earlyRounded)){
          earlyRounded=earlyRounded.add(BigInteger.ONE);
          //System.out.println(earlyRounded);
          if (earlyRounded.testBit(0)==false && (thisRadix&1)==0) {
            IShiftAccumulator accum2 = helper.CreateShiftAccumulator(earlyRounded);
            accum2.ShiftToDigits(fastPrecision);
            //System.out.println("{0} {1}",accum2.getShiftedInt(),accum2.getDiscardedDigitCount());
            if ((accum2.getDiscardedDigitCount()).signum() != 0) {
              //overPrecision=true;
              earlyRounded = accum2.getShiftedInt();
            }
            newAdjExponent=FastInteger.Copy(exp)
              .Add(accum2.GetDigitLength())
              .SubtractInt(1);
          }
        }
      }
      if (fastEMax != null && adjExponent.compareTo(fastEMax) > 0) {
        if (mantissaWasZero) {
          if (signals != null) {
            signals[0] = flags | PrecisionContext.FlagClamped;
          }
          return helper.CreateNewWithFlags(oldmantissa, fastEMax.AsBigInteger(),thisFlags);
        }
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
          BigInteger overflowMant = BigInteger.ZERO;
          if(binaryPrec){
            overflowMant=maxMantissa;
          } else {
            overflowMant=helper.MultiplyByRadixPower(BigInteger.ONE, fastPrecision);
            overflowMant=overflowMant.subtract(BigInteger.ONE);
          }
          if (signals != null) signals[0] = flags;
          clamp = FastInteger.Copy(fastEMax).AddInt(1)
            .Subtract(fastPrecision);
          return helper.CreateNewWithFlags(overflowMant,
                                           clamp.AsBigInteger(),
                                           neg ? BigNumberFlags.FlagNegative : 0
                                          );
        }
        if (signals != null) signals[0] = flags;
        return SignalOverflow(neg);
      } else if (fastEMin != null && adjExponent.compareTo(fastEMin) < 0) {
        // Subnormal
        FastInteger fastETiny = FastInteger.Copy(fastEMin)
          .Subtract(fastPrecision)
          .AddInt(1);
        if (signals!=null){
          if(earlyRounded.signum()!=0){
            if(newAdjExponent.compareTo(fastEMin)<0){
              flags |= PrecisionContext.FlagSubnormal;
            }
          }
        }
        //System.out.println("exp={0} eTiny={1}",exp,fastETiny);
        FastInteger subExp=FastInteger.Copy(exp);
        //System.out.println("exp={0} eTiny={1}",subExp,fastETiny);
        if (subExp.compareTo(fastETiny) < 0) {
          //System.out.println("Less than ETiny");
          FastInteger expdiff = FastInteger.Copy(fastETiny).Subtract(exp);
          expdiff.Add(discardedBits);
          accum = helper.CreateShiftAccumulatorWithDigits(
            oldmantissa, lastDiscarded, olderDiscarded);
          accum.ShiftRight(expdiff);
          FastInteger newmantissa = accum.getShiftedIntFast();
          if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
            if (rounding == Rounding.Unnecessary)
              throw new ArithmeticException("Rounding was required");
          }
          if ((accum.getDiscardedDigitCount()).signum() != 0 ||
              (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
            if(signals!=null){
              if (!mantissaWasZero)
                flags |= PrecisionContext.FlagRounded;
              if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                flags |= PrecisionContext.FlagInexact|PrecisionContext.FlagRounded;
              }
            }
            if (Round(accum, rounding, neg, newmantissa)) {
              newmantissa.AddInt(1);
            }
          }
          if (signals != null){
            if (newmantissa.signum()==0)
              flags |= PrecisionContext.FlagClamped;
            if ((flags & (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact)) ==
                (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact))
              flags |= PrecisionContext.FlagUnderflow | PrecisionContext.FlagRounded;
            signals[0] = flags;
          }
          return helper.CreateNewWithFlags(newmantissa.AsBigInteger(), fastETiny.AsBigInteger(),
                                           neg ? BigNumberFlags.FlagNegative : 0);
        }
      }
      boolean recheckOverflow = false;
      if ((accum.getDiscardedDigitCount()).signum() != 0 ||
          (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
        if (bigmantissa.signum()!=0)
          flags |= PrecisionContext.FlagRounded;
        bigmantissa = accum.getShiftedInt();
        if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
          flags |= PrecisionContext.FlagInexact|PrecisionContext.FlagRounded;
          if (rounding == Rounding.Unnecessary)
            throw new ArithmeticException("Rounding was required");
        }
        if (RoundGivenBigInt(accum, rounding, neg, bigmantissa)) {
          bigmantissa=bigmantissa.add(BigInteger.ONE);
          if(binaryPrec)recheckOverflow=true;
          if (bigmantissa.testBit(0)==false && (thisRadix&1)==0) {
            accum = helper.CreateShiftAccumulator(bigmantissa);
            accum.ShiftToDigits(fastPrecision);
            if(binaryPrec){
              while((accum.getShiftedInt()).compareTo(maxMantissa)>0){
                accum.ShiftRightInt(1);
              }
            }
            if ((accum.getDiscardedDigitCount()).signum() != 0) {
              exp.Add(accum.getDiscardedDigitCount());
              discardedBits.Add(accum.getDiscardedDigitCount());
              bigmantissa = accum.getShiftedInt();
              if(!binaryPrec)recheckOverflow = true;
            }
          }
        }
      }
      if (recheckOverflow && fastEMax != null) {
        // Check for overflow again
        adjExponent = FastInteger.Copy(exp);
        adjExponent.Add(accum.GetDigitLength()).SubtractInt(1);
        if(binaryPrec && fastEMax!=null && adjExponent.compareTo(fastEMax)==0){
          // May or may not be an overflow depending on the mantissa
          FastInteger expdiff=FastInteger.Copy(fastPrecision).Subtract(accum.GetDigitLength());
          BigInteger currMantissa=accum.getShiftedInt();
          currMantissa=helper.MultiplyByRadixPower(currMantissa,expdiff);
          if((currMantissa).compareTo(maxMantissa)>0){
            // Mantissa too high, treat as overflow
            adjExponent.AddInt(1);
          }
        }
        if (adjExponent.compareTo(fastEMax) > 0) {
          flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
          if (!unlimitedPrec &&
              (rounding == Rounding.Down ||
               rounding == Rounding.ZeroFiveUp ||
               (rounding == Rounding.Ceiling && neg) ||
               (rounding == Rounding.Floor && !neg))) {
            // Set to the highest possible value for
            // the given precision
            BigInteger overflowMant = BigInteger.ZERO;
            if(binaryPrec){
              overflowMant=maxMantissa;
            } else {
              overflowMant=helper.MultiplyByRadixPower(BigInteger.ONE, fastPrecision);
              overflowMant=overflowMant.subtract(BigInteger.ONE);
            }
            if (signals != null) signals[0] = flags;
            clamp = FastInteger.Copy(fastEMax).AddInt(1)
              .Subtract(fastPrecision);
            return helper.CreateNewWithFlags(overflowMant,
                                             clamp.AsBigInteger(),
                                             neg ? BigNumberFlags.FlagNegative : 0);
          }
          if (signals != null) signals[0] = flags;
          return SignalOverflow(neg);
        }
      }
      if (signals != null) signals[0] = flags;
      return helper.CreateNewWithFlags(bigmantissa, exp.AsBigInteger(),
                                       neg ? BigNumberFlags.FlagNegative : 0);
    }
    
    private T AddCore(BigInteger mant1, // assumes mant1 is nonnegative
                      BigInteger mant2, // assumes mant2 is nonnegative
                      BigInteger exponent, int flags1, int flags2, PrecisionContext ctx) {
      
      boolean neg1=(flags1&BigNumberFlags.FlagNegative)!=0;
      boolean neg2=(flags2&BigNumberFlags.FlagNegative)!=0;
      boolean negResult=false;
      if(neg1!=neg2){
        // Signs are different, treat as a subtraction
        mant1=mant1.subtract(mant2);
        int mant1Sign=mant1.signum();
        negResult=neg1^(mant1Sign==0 ? neg2 : (mant1Sign<0));
      } else {
        // Signs are same, treat as an addition
        mant1=mant1.add(mant2);
        negResult=neg1;
      }
      if(mant1.signum()==0 && negResult){
        // Result is negative zero
        if(!((neg1 && neg2) || ((neg1^neg2) && ctx!=null && ctx.getRounding()== Rounding.Floor))){
          negResult=false;
        }
      }
      return helper.CreateNewWithFlags(mant1,exponent,negResult ? BigNumberFlags.FlagNegative : 0);
    }

    /**
     * 
     * @param thisValue A T object.
     * @param ctx A PrecisionContext object.
     * @param other A T object.
     * @return A T object.
     */
    public T Add(T thisValue, T other, PrecisionContext ctx) {
      int thisFlags=helper.GetFlags(thisValue);
      int otherFlags=helper.GetFlags(other);
      if(((thisFlags|otherFlags)&BigNumberFlags.FlagSpecial)!=0){
        T result=HandleNotANumber(thisValue,other,ctx);
        if((Object)result!=(Object)null)return result;
        if((thisFlags&BigNumberFlags.FlagInfinity)!=0){
          if((otherFlags&BigNumberFlags.FlagInfinity)!=0){
            if((thisFlags&BigNumberFlags.FlagNegative)!=(otherFlags&BigNumberFlags.FlagNegative))
              return SignalInvalid(ctx);
          }
          return thisValue;
        }
        if((otherFlags&BigNumberFlags.FlagInfinity)!=0){
          return other;
        }
      }
      int expcmp = helper.GetExponent(thisValue).compareTo(helper.GetExponent(other));
      T retval = null;
      BigInteger op1MantAbs=(helper.GetMantissa(thisValue)).abs();
      BigInteger op2MantAbs=(helper.GetMantissa(other)).abs();
      if (expcmp == 0) {
        retval = AddCore(op1MantAbs,op2MantAbs,helper.GetExponent(thisValue),thisFlags,otherFlags,ctx);
      } else {
        // choose the minimum exponent
        T op1 = thisValue;
        T op2 = other;
        BigInteger op1Exponent = helper.GetExponent(op1);
        BigInteger op2Exponent = helper.GetExponent(op2);
        BigInteger resultExponent = (expcmp < 0 ? op1Exponent : op2Exponent);
        FastInteger fastOp1Exp=FastInteger.FromBig(op1Exponent);
        FastInteger fastOp2Exp=FastInteger.FromBig(op2Exponent);
        FastInteger expdiff = FastInteger.Copy(fastOp1Exp).Subtract(fastOp2Exp).Abs();
        if (ctx != null && (ctx.getPrecision()).signum() > 0) {
          // Check if exponent difference is too big for
          // radix-power calculation to work quickly
          FastInteger fastPrecision=FastInteger.FromBig(ctx.getPrecision());
          // If exponent difference is greater than the precision
          if (FastInteger.Copy(expdiff).compareTo(fastPrecision) > 0) {
            int expcmp2 = fastOp1Exp.compareTo(fastOp2Exp);
            if (expcmp2 < 0) {
              if(!(op2MantAbs.signum()==0)){
                // first operand's exponent is less
                // and second operand isn't zero
                // second mantissa will be shifted by the exponent
                // difference
                //                    111111111111|
                //        222222222222222|
                FastInteger digitLength1=helper.CreateShiftAccumulator(op1MantAbs)
                  .GetDigitLength();
                if (
                  FastInteger.Copy(fastOp1Exp)
                  .Add(digitLength1)
                  .AddInt(2)
                  .compareTo(fastOp2Exp) < 0) {
                  // first operand's mantissa can't reach the
                  // second operand's mantissa, so the exponent can be
                  // raised without affecting the result
                  FastInteger tmp=FastInteger.Copy(fastOp2Exp).SubtractInt(4)
                    .Subtract(digitLength1)
                    .SubtractBig(ctx.getPrecision());
                  FastInteger newDiff=FastInteger.Copy(tmp).Subtract(fastOp2Exp).Abs();
                  if(newDiff.compareTo(expdiff)<0){
                    // Can be treated as almost zero
                    if(helper.GetSign(thisValue)==helper.GetSign(other)){
                      FastInteger digitLength2=helper.CreateShiftAccumulator(
                        op2MantAbs).GetDigitLength();
                      if(digitLength2.compareTo(fastPrecision)<0){
                        // Second operand's precision too short
                        FastInteger precisionDiff=FastInteger.Copy(fastPrecision).Subtract(digitLength2);
                        op2MantAbs=helper.MultiplyByRadixPower(
                          op2MantAbs,precisionDiff);
                        BigInteger bigintTemp=precisionDiff.AsBigInteger();
                        op2Exponent=op2Exponent.subtract(bigintTemp);
                        other=helper.CreateNewWithFlags(op2MantAbs,op2Exponent,helper.GetFlags(other));
                        return RoundToPrecisionWithShift(other,ctx,0,1,null,false);
                      } else {
                        FastInteger shift=FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                        return RoundToPrecisionWithShift(other,ctx,0,1,shift,false);
                      }
                    } else {
                      if(!(op1MantAbs.signum()==0))
                        op1MantAbs = BigInteger.ONE;
                      op1Exponent = (tmp.AsBigInteger());
                    }
                  }
                }
              }
            } else if (expcmp2 > 0) {
              if(!(op1MantAbs.signum()==0)){
                // first operand's exponent is greater
                // and first operand isn't zero
                // first mantissa will be shifted by the exponent
                // difference
                //       111111111111|
                //                222222222222222|
                FastInteger digitLength2=helper.CreateShiftAccumulator(op2MantAbs).GetDigitLength();
                if (
                  FastInteger.Copy(fastOp2Exp)
                  .Add(digitLength2)
                  .AddInt(2)
                  .compareTo(fastOp1Exp) < 0) {
                  // second operand's mantissa can't reach the
                  // first operand's mantissa, so the exponent can be
                  // raised without affecting the result
                  FastInteger tmp=FastInteger.Copy(fastOp1Exp).SubtractInt(4)
                    .Subtract(digitLength2)
                    .SubtractBig(ctx.getPrecision());
                  FastInteger newDiff=FastInteger.Copy(tmp).Subtract(fastOp1Exp).Abs();
                  if(newDiff.compareTo(expdiff)<0){
                    // Can be treated as almost zero
                    if(helper.GetSign(thisValue)==helper.GetSign(other)){
                      FastInteger digitLength1=helper.CreateShiftAccumulator(op1MantAbs).GetDigitLength();
                      if(digitLength1.compareTo(fastPrecision)<0){
                        // First operand's precision too short
                        FastInteger precisionDiff=FastInteger.Copy(fastPrecision).Subtract(digitLength1);
                        op1MantAbs=helper.MultiplyByRadixPower(
                          op1MantAbs,precisionDiff);
                        BigInteger bigintTemp=precisionDiff.AsBigInteger();
                        op1Exponent=op1Exponent.subtract(bigintTemp);
                        thisValue=helper.CreateNewWithFlags(op1MantAbs,op1Exponent,
                                                            helper.GetFlags(thisValue));
                        return RoundToPrecisionWithShift(thisValue,ctx,0,1, null,false);
                      } else {
                        FastInteger shift=FastInteger.Copy(digitLength1).Subtract(fastPrecision);
                        return RoundToPrecisionWithShift(thisValue,ctx,0,1, shift,false);
                      }
                    } else {
                      if(!(op2MantAbs.signum()==0))
                        op2MantAbs = BigInteger.ONE;
                      op2Exponent = (tmp.AsBigInteger());
                    }
                  }
                }
              }
            }
            expcmp = op1Exponent.compareTo(op2Exponent);
            resultExponent = (expcmp < 0 ? op1Exponent : op2Exponent);
          }
        }
        if (expcmp > 0) {
          op1MantAbs = helper.RescaleByExponentDiff(
            op1MantAbs, op1Exponent, op2Exponent);
          //System.out.println("{0} {1} -> {2}",op1MantAbs,op2MantAbs,op2MantAbs-op1MantAbs);
          retval = AddCore(
            op1MantAbs, op2MantAbs,resultExponent,
            thisFlags,otherFlags,ctx);
        } else {
          op2MantAbs = helper.RescaleByExponentDiff(
            op2MantAbs, op1Exponent, op2Exponent);
          //System.out.println("{0} {1} -> {2}",op1MantAbs,op2MantAbs,op2MantAbs-op1MantAbs);
          retval = AddCore(
            op1MantAbs, op2MantAbs,resultExponent,
            thisFlags,otherFlags,ctx);
        }
      }
      if (ctx != null) {
        retval = RoundToPrecision(retval, ctx);
      }
      return retval;
    }
    
    /**
     * Compares a T object with this instance.
     * @param thisValue A T object.
     * @param decfrac A T object.
     * @param treatQuietNansAsSignaling A Boolean object.
     * @param ctx A PrecisionContext object.
     * @return A T object.
     */
    public T CompareToWithContext(T thisValue, T decfrac, boolean treatQuietNansAsSignaling, PrecisionContext ctx) {
      if(decfrac==null)return SignalInvalid(ctx);
      T result=CompareToHandleSpecial(thisValue,decfrac,treatQuietNansAsSignaling,ctx);
      if((Object)result!=(Object)null)return result;
      return ValueOf(compareTo(thisValue,decfrac),null);
    }
    
    /**
     * Compares a T object with this instance.
     * @param thisValue A T object.
     * @param decfrac A T object.
     * @return Zero if the values are equal; a negative number if this instance
     * is less, or a positive number if this instance is greater.
     */
public int compareTo(T thisValue, T decfrac) {
      if (decfrac == null)return 1;
      int flagsThis=helper.GetFlags(thisValue);
      int flagsOther=helper.GetFlags(decfrac);
      if((flagsThis&BigNumberFlags.FlagNaN)!=0){
        if((flagsOther&BigNumberFlags.FlagNaN)!=0){
          return 0;
        }
        return 1; // Treat NaN as greater
      }
      if((flagsOther&BigNumberFlags.FlagNaN)!=0){
        return -1; // Treat as less than NaN
      }
      int s=CompareToHandleSpecialReturnInt(thisValue,decfrac);
      if(s<=1)return s;
      s = helper.GetSign(thisValue);
      int ds = helper.GetSign(decfrac);
      if (s != ds) return (s < ds) ? -1 : 1;
      if (ds == 0 || s==0) {
        // Special case: Either operand is zero
        return 0;
      }
      int expcmp = helper.GetExponent(thisValue).compareTo(helper.GetExponent(decfrac));
      // At this point, the signs are equal so we can compare
      // their absolute values instead
      int mantcmp = (helper.GetMantissa(thisValue)).abs()
        .compareTo((helper.GetMantissa(decfrac)).abs());
      if(s<0)mantcmp=-mantcmp;
      if (mantcmp == 0) {
        // Special case: Mantissas are equal
        return s<0 ? -expcmp : expcmp;
      }
      if(expcmp==0){
        return mantcmp;
      }
      BigInteger op1Exponent = helper.GetExponent(thisValue);
      BigInteger op2Exponent = helper.GetExponent(decfrac);
      FastInteger fastOp1Exp=FastInteger.FromBig(op1Exponent);
      FastInteger fastOp2Exp=FastInteger.FromBig(op2Exponent);
      FastInteger expdiff = FastInteger.Copy(fastOp1Exp).Subtract(fastOp2Exp).Abs();
      // Check if exponent difference is too big for
      // radix-power calculation to work quickly
      if (expdiff.CompareToInt(100) >= 0) {
        BigInteger op1MantAbs=(helper.GetMantissa(thisValue)).abs();
        BigInteger op2MantAbs=(helper.GetMantissa(decfrac)).abs();
        FastInteger precision1 = helper.CreateShiftAccumulator(
          op1MantAbs).GetDigitLength();
        FastInteger precision2 = helper.CreateShiftAccumulator(
          op2MantAbs).GetDigitLength();
        FastInteger maxPrecision=null;
        if(precision1.compareTo(precision2)>0)
          maxPrecision=precision1;
        else
          maxPrecision=precision2;
        // If exponent difference is greater than the
        // maximum precision of the two operands
        if (FastInteger.Copy(expdiff).compareTo(maxPrecision) > 0) {
          int expcmp2 = fastOp1Exp.compareTo(fastOp2Exp);
          if (expcmp2 < 0) {
            if(!(op2MantAbs.signum()==0)){
              // first operand's exponent is less
              // and second operand isn't zero
              // second mantissa will be shifted by the exponent
              // difference
              //                    111111111111|
              //        222222222222222|
              FastInteger digitLength1=helper.CreateShiftAccumulator(
                op1MantAbs).GetDigitLength();
              if (
                FastInteger.Copy(fastOp1Exp)
                .Add(digitLength1)
                .AddInt(2)
                .compareTo(fastOp2Exp) < 0) {
                // first operand's mantissa can't reach the
                // second operand's mantissa, so the exponent can be
                // raised without affecting the result
                FastInteger tmp=FastInteger.Copy(fastOp2Exp).SubtractInt(8)
                  .Subtract(digitLength1)
                  .Subtract(maxPrecision);
                FastInteger newDiff=FastInteger.Copy(tmp).Subtract(fastOp2Exp).Abs();
                if(newDiff.compareTo(expdiff)<0){
                  if(s==ds){
                    return (s<0) ? 1 : -1;
                  } else {
                    op1Exponent = (tmp.AsBigInteger());
                  }
                }
              }
            }
          } else if (expcmp2 > 0) {
            if(!(op1MantAbs.signum()==0)){
              // first operand's exponent is greater
              // and second operand isn't zero
              // first mantissa will be shifted by the exponent
              // difference
              //       111111111111|
              //                222222222222222|
              FastInteger digitLength2=helper.CreateShiftAccumulator(
                op2MantAbs).GetDigitLength();
              if (
                FastInteger.Copy(fastOp2Exp)
                .Add(digitLength2)
                .AddInt(2)
                .compareTo(fastOp1Exp) < 0) {
                // second operand's mantissa can't reach the
                // first operand's mantissa, so the exponent can be
                // raised without affecting the result
                FastInteger tmp=FastInteger.Copy(fastOp1Exp).SubtractInt(8)
                  .Subtract(digitLength2)
                  .Subtract(maxPrecision);
                FastInteger newDiff=FastInteger.Copy(tmp).Subtract(fastOp1Exp).Abs();
                if(newDiff.compareTo(expdiff)<0){
                  if(s==ds){
                    return (s<0) ? -1 : 1;
                  } else {
                    op2Exponent = (tmp.AsBigInteger());
                  }
                }
              }
            }
          }
          expcmp = op1Exponent.compareTo(op2Exponent);
        }
      }
      if (expcmp > 0) {
        BigInteger newmant = helper.RescaleByExponentDiff(
          helper.GetMantissa(thisValue), op1Exponent, op2Exponent);
        BigInteger othermant=(helper.GetMantissa(decfrac)).abs();
        newmant=(newmant).abs();
        mantcmp=newmant.compareTo(othermant);
        return (s<0) ? -mantcmp : mantcmp;
      } else {
        BigInteger newmant = helper.RescaleByExponentDiff(
          helper.GetMantissa(decfrac), op1Exponent, op2Exponent);
        BigInteger othermant=(helper.GetMantissa(thisValue)).abs();
        newmant=(newmant).abs();
        mantcmp=othermant.compareTo(newmant);
        return (s<0) ? -mantcmp : mantcmp;
      }
    }
  }
