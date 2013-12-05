package com.upokecenter.util;


//import java.math.*;


    /**
     * Encapsulates radix-independent arithmetic.
     */
  class RadixMath<T> {

    IRadixMathHelper<T> helper;

    public RadixMath(IRadixMathHelper<T> helper) {
      this.helper = helper;
    }
    /**
     * Gets the lesser value between two T values.
     * @param a A T object.
     * @param b A T object.
     * @return The smaller value of the two objects.
     */
    public T Min(T a, T b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      int cmp = compareTo(a, b);
      if (cmp != 0)
        return cmp > 0 ? b : a;
      // Here the signs of both a and b can only be
      // equal (negative zeros are not supported)
      if (helper.GetSign(a) >= 0) {
        return (helper.GetExponent(a)).compareTo(helper.GetExponent(b)) > 0 ? b : a;
      } else {
        return (helper.GetExponent(a)).compareTo(helper.GetExponent(b)) > 0 ? a : b;
      }
    }


    private boolean Round(IShiftAccumulator accum, Rounding rounding,
                       boolean neg, BigInteger bigval) {
      boolean incremented = false;
      int radix = helper.GetRadix();
      if (rounding == Rounding.HalfUp) {
        if (accum.getLastDiscardedDigit() >= (radix / 2)) {
          incremented = true;
        }
      } else if (rounding == Rounding.HalfEven) {
        if (accum.getLastDiscardedDigit() >= (radix / 2)) {
          if ((accum.getLastDiscardedDigit() > (radix / 2) || accum.getOlderDiscardedDigits() != 0)) {
            incremented = true;
          } else if (bigval.testBit(0)) {
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

    private T EnsureSign(T val, boolean negative) {
      if (val == null) return val;
      int sign = helper.GetSign(val);
      if (negative && sign > 0) {
        BigInteger bigmant = helper.GetMantissa(val);
        bigmant=(bigmant).negate();
        BigInteger e = helper.GetExponent(val);
        return helper.CreateNew(bigmant, e);
      } else if (!negative && sign < 0) {
        return helper.Abs(val);
      }
      return val;
    }

    /**
     * 
     * @param thisValue A T object.
     * @param divisor A T object.
     * @param ctx A PrecisionContext object.
     */
    public T DivideToIntegerNaturalScale(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      T ret = DivideInternal(thisValue, divisor,
                             new PrecisionContext(Rounding.Down),
                             IntegerModeNaturalScale, BigInteger.ZERO,
                             null);
      boolean neg = (helper.GetSign(thisValue) < 0) ^ (helper.GetSign(divisor) < 0);
      if (helper.GetExponent(ret).signum() < 0) {
        BigInteger bigmantissa = (helper.GetMantissa(ret)).abs();
        IShiftAccumulator accum = helper.CreateShiftAccumulator(bigmantissa);
        accum.ShiftRight(new FastInteger(helper.GetExponent(ret)).Negate());
        bigmantissa = accum.getShiftedInt();
        if (helper.GetMantissa(ret).signum() < 0) bigmantissa=bigmantissa.negate();
        ret = helper.CreateNew(bigmantissa, BigInteger.ZERO);
      }
      if (helper.GetSign(ret) == 0) {
        BigInteger divisorExp=helper.GetExponent(divisor);
        ret = helper.CreateNew(BigInteger.ZERO, helper.GetExponent(thisValue).subtract(divisorExp));
      } else {
        FastInteger desiredScale = new FastInteger(helper.GetExponent(thisValue)).Subtract(
          helper.GetExponent(divisor));
        if (desiredScale.signum() < 0) {
          desiredScale.Negate();
          BigInteger bigmantissa = (helper.GetMantissa(ret)).abs();
          bigmantissa = helper.MultiplyByRadixPower(bigmantissa, desiredScale);
          if (helper.GetMantissa(ret).signum() < 0) bigmantissa=bigmantissa.negate();
          ret = helper.CreateNew(bigmantissa, helper.GetExponent(thisValue).subtract(helper.GetExponent(divisor)));
        } else if (desiredScale.signum() > 0) {
          BigInteger bigmantissa = (helper.GetMantissa(ret)).abs();
          FastInteger fastexponent = new FastInteger(helper.GetExponent(ret));
          BigInteger bigradix = BigInteger.valueOf(helper.GetRadix());
          while(true){
            BigInteger bigrem;
            BigInteger bigquo;
BigInteger[] divrem=(bigmantissa).divideAndRemainder(bigradix);
bigquo=divrem[0];
bigrem=divrem[1];
            if(bigrem.signum()!=0)
              break;
            bigmantissa=bigquo;
            fastexponent.Add(1);
          }
          if (helper.GetMantissa(ret).signum() < 0) bigmantissa=bigmantissa.negate();
          ret = helper.CreateNew(bigmantissa, fastexponent.AsBigInteger());
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
     */
    public T DivideToIntegerZeroScale(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      T ret = DivideInternal(thisValue, divisor,
                             new PrecisionContext(Rounding.Down).WithPrecision(ctx == null ? 0 : ctx.getPrecision()),
                             IntegerModeFixedScale, BigInteger.ZERO,
                             null);
      if (ctx != null) {
        PrecisionContext ctx2 = ctx.WithBlankFlags().WithUnlimitedExponents();
        ret = RoundToPrecision(ret, ctx2);
        if ((ctx2.getFlags() & PrecisionContext.FlagRounded) != 0) {
          throw new ArithmeticException("Result would require a higher precision");
        }
      }
      return ret;
    }
    
    private T Negate(T value) {
      BigInteger mant=helper.GetMantissa(value);
      mant=(mant).negate();
      return helper.CreateNew(mant,helper.GetExponent(value));
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
      T ret=DivideToIntegerZeroScale(thisValue,divisor,ctx);
      ret=Add(thisValue,Negate(Multiply(ret,divisor,null)),null);
      if (ctx != null) {
        PrecisionContext ctx2 = ctx.WithBlankFlags();
        ret = RoundToPrecision(ret, ctx2);
      }
      return ret;
    }

    /**
     * 
     * @param thisValue A T object.
     * @param divisor A T object.
     * @param ctx A PrecisionContext object.
     */
    public T RemainderNear(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      T ret = DivideInternal(thisValue, divisor,
                             new PrecisionContext(Rounding.HalfEven)
                             .WithPrecision(ctx == null ? 0 : ctx.getPrecision()),
                             IntegerModeFixedScale, BigInteger.ZERO,
                             null);
      if (ctx != null) {
        PrecisionContext ctx2 = ctx.WithBlankFlags().WithUnlimitedExponents();
        ret = RoundToPrecision(ret, ctx2);
        if ((ctx2.getFlags() & PrecisionContext.FlagRounded) != 0) {
          throw new ArithmeticException("Result would require a higher precision");
        }
      }
      ret=Add(thisValue,Negate(Multiply(ret,divisor,null)),null);
      if (ctx != null) {
        PrecisionContext ctx2 = ctx.WithBlankFlags();
        ret = RoundToPrecision(ret, ctx2);
      }
      return ret;
    }


    /**
     * 
     * @param thisValue A T object.
     * @param ctx A PrecisionContext object.
     */
    public T NextMinus(
      T thisValue,
      PrecisionContext ctx
     ) {
      if((ctx)==null)throw new NullPointerException("ctx");
      if((ctx.getPrecision())<=0)throw new IllegalArgumentException("ctx.getPrecision()"+" not less than "+"0"+" ("+Long.toString((long)(long)(ctx.getPrecision()))+")");
      if(!(ctx.getHasExponentRange()))throw new IllegalArgumentException("doesn't satisfy ctx.getHasExponentRange()");
      BigInteger minusone=(BigInteger.ONE).negate();
      FastInteger minexp=new FastInteger(ctx.getEMin()).Subtract(ctx.getPrecision()).Add(1);
      FastInteger bigexp=new FastInteger(helper.GetExponent(thisValue));
      if(bigexp.compareTo(minexp)<0){
        // Use a smaller exponent if the input exponent is already
        // very small
        minexp=new FastInteger(bigexp).Subtract(2);
      }
      T quantum=helper.CreateNew(minusone,minexp.AsBigInteger());
      PrecisionContext ctx2;
      T val=thisValue;
      ctx2=ctx.WithRounding(Rounding.Floor);
      return Add(val,quantum,ctx2);
    }

    /**
     * 
     * @param thisValue A T object.
     * @param otherValue A T object.
     * @param ctx A PrecisionContext object.
     */
    public T NextToward(
      T thisValue,
      T otherValue,
      PrecisionContext ctx
     ) {
      if((ctx)==null)throw new NullPointerException("ctx");
      if((ctx.getPrecision())<=0)throw new IllegalArgumentException("ctx.getPrecision()"+" not less than "+"0"+" ("+Long.toString((long)(long)(ctx.getPrecision()))+")");
      if(!(ctx.getHasExponentRange()))throw new IllegalArgumentException("doesn't satisfy ctx.getHasExponentRange()");
      PrecisionContext ctx2;
      int cmp=compareTo(thisValue,otherValue);
      if(cmp==0){
        return RoundToPrecision(thisValue,ctx.WithNoFlags());
      } else {
        FastInteger minexp=new FastInteger(ctx.getEMin()).Subtract(ctx.getPrecision()).Add(1);
        FastInteger bigexp=new FastInteger(helper.GetExponent(thisValue));
        if(bigexp.compareTo(minexp)<0){
          // Use a smaller exponent if the input exponent is already
          // very small
          minexp=new FastInteger(bigexp).Subtract(2);
        }
        BigInteger bigdir=BigInteger.ONE;
        if(cmp>0){
          bigdir=(bigdir).negate();
        }
        T quantum=helper.CreateNew(bigdir,minexp.AsBigInteger());
        T val=thisValue;
        ctx2=ctx.WithRounding((cmp>0) ? Rounding.Floor : Rounding.Ceiling).WithBlankFlags();
        val=Add(val,quantum,ctx2);
        if(ctx.getHasFlags()){
          ctx.setFlags(ctx.getFlags()|ctx2.getFlags());
        }
        return val;
      }
    }
    
    /**
     * 
     * @param thisValue A T object.
     * @param ctx A PrecisionContext object.
     */
    public T NextPlus(
      T thisValue,
      PrecisionContext ctx
     ) {
      if((ctx)==null)throw new NullPointerException("ctx");
      if((ctx.getPrecision())<=0)throw new IllegalArgumentException("ctx.getPrecision()"+" not less than "+"0"+" ("+Long.toString((long)(long)(ctx.getPrecision()))+")");
      if(!(ctx.getHasExponentRange()))throw new IllegalArgumentException("doesn't satisfy ctx.getHasExponentRange()");
      BigInteger minusone=BigInteger.ONE;
      FastInteger minexp=new FastInteger(ctx.getEMin()).Subtract(ctx.getPrecision()).Add(1);
      FastInteger bigexp=new FastInteger(helper.GetExponent(thisValue));
      if(bigexp.compareTo(minexp)<0){
        // Use a smaller exponent if the input exponent is already
        // very small
        minexp=new FastInteger(bigexp).Subtract(2);
      }
      T quantum=helper.CreateNew(minusone,minexp.AsBigInteger());
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
     * @param ctx A PrecisionContext object.
     * @return The quotient of the two objects.
     */
    public T Divide(
      T thisValue,
      T divisor,
      BigInteger desiredExponent,
      PrecisionContext ctx
     ) {
      PrecisionContext ctx2 = (ctx == null) ?
        new PrecisionContext(Rounding.HalfDown) :
        ctx.WithUnlimitedExponents().WithPrecision(0);
      T ret = DivideInternal(thisValue, divisor,
                             ctx2,
                             IntegerModeFixedScale, desiredExponent,null);
      if (ctx != null && ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags()|ctx2.getFlags());
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
      FastInteger shift,
      boolean neg,
      PrecisionContext ctx
     ) {
      IShiftAccumulator accum;
      int lastDiscarded = 0;
      int olderDiscarded = 0;
      if (!(remainder.signum()==0)) {
        BigInteger halfDivisor = (divisor.shiftRight(1));
        int cmpHalf = remainder.compareTo(halfDivisor);
        if ((cmpHalf == 0) && divisor.testBit(0)==false) {
          // remainder is exactly half
          lastDiscarded = (helper.GetRadix() / 2);
          olderDiscarded = 0;
        } else if (cmpHalf > 0) {
          // remainder is greater than half
          lastDiscarded = (helper.GetRadix() / 2);
          olderDiscarded = 1;
        } else {
          // remainder is less than half
          lastDiscarded = 0;
          olderDiscarded = 1;
        }
      }
      accum = helper.CreateShiftAccumulator(
        mantissa, lastDiscarded, olderDiscarded);
      accum.ShiftRight(shift);
      int flags = 0;
      Rounding rounding = (ctx == null) ? Rounding.HalfEven : ctx.getRounding();
      BigInteger newmantissa = accum.getShiftedInt();
      if ((accum.getDiscardedDigitCount()).signum() != 0 ||
          (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
        if (mantissa.signum()!=0)
          flags |= PrecisionContext.FlagRounded;
        if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
          flags |= PrecisionContext.FlagInexact;
          if (rounding == Rounding.Unnecessary)
            throw new ArithmeticException("Rounding was required");
        }
        if (Round(accum, rounding, neg, newmantissa)) {
          newmantissa=newmantissa.add(BigInteger.ONE);
        }
      }
      if (ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags()|flags);
      }
      if (neg) {
        newmantissa=newmantissa.negate();
      }
      return newmantissa;
    }

    private static final int IntegerModeFixedScale = 1;
    private static final int IntegerModeNaturalScale = 2;
    private static final int IntegerModeRegular = 0;

    private T DivideInternal(
      T thisValue,
      T divisor,
      PrecisionContext ctx,
      int integerMode,
      BigInteger desiredExponent,
      T[] remainder
     ) {
      int signA = helper.GetSign(thisValue);
      int signB = helper.GetSign(divisor);
      if (signB == 0) {
        throw new ArithmeticException();
      }
      if (signA == 0) {
        T retval=null;
        if (integerMode == IntegerModeFixedScale) {
          retval=helper.CreateNew(BigInteger.ZERO, desiredExponent);
        } else {
          BigInteger divExp=helper.GetExponent(divisor);
          retval=RoundToPrecision(helper.CreateNew(
            BigInteger.ZERO, (helper.GetExponent(thisValue).subtract(divExp))), ctx);
        }
        if(remainder!=null){
          remainder[0]=retval;
        }
        return retval;
      } else {
        BigInteger mantissaDividend = helper.GetMantissa(thisValue);
        BigInteger mantissaDivisor = helper.GetMantissa(divisor);
        FastInteger adjust = new FastInteger();
        FastInteger2 result = new FastInteger2();
        boolean negA = (signA < 0);
        boolean negB = (signB < 0);
        if (negA) mantissaDividend=mantissaDividend.negate();
        if (negB) mantissaDivisor=mantissaDivisor.negate();
        FastInteger resultPrecision = new FastInteger(1);
        int mantcmp = mantissaDividend.compareTo(mantissaDivisor);
        if (mantcmp < 0) {
          // dividend mantissa is less than divisor mantissa
          long dividendPrecision = helper.CreateShiftAccumulator(mantissaDividend).getDigitLength();
          long divisorPrecision = helper.CreateShiftAccumulator(mantissaDivisor).getDigitLength();
          long powerOfRadix = Math.max(1, divisorPrecision - dividendPrecision);
          // multiply dividend mantissa so precisions are the same
          // (except if they're already the same, in which case multiply
          // by radix)
          mantissaDividend = helper.MultiplyByRadixPower(mantissaDividend, powerOfRadix);
          adjust.Add(powerOfRadix);
          if (mantissaDividend.compareTo(mantissaDivisor) < 0) {
            // dividend mantissa is still less, multiply once more
            mantissaDividend = helper.MultiplyByRadixPower(mantissaDividend, 1);
            adjust.Add(1);
          }
        } else if (mantcmp > 0) {
          // dividend mantissa is greater than divisor mantissa
          long dividendPrecision = helper.CreateShiftAccumulator(mantissaDividend).getDigitLength();
          long divisorPrecision = helper.CreateShiftAccumulator(mantissaDivisor).getDigitLength();
          long powerOfRadix = (dividendPrecision - divisorPrecision);
          BigInteger oldMantissaB = mantissaDivisor;
          mantissaDivisor = helper.MultiplyByRadixPower(mantissaDivisor, powerOfRadix);
          adjust.Subtract(powerOfRadix);
          if (mantissaDividend.compareTo(mantissaDivisor) < 0) {
            // dividend mantissa is now less, divide by radix power
            if (powerOfRadix == 1) {
              // no need to divide here, since thisValue would just undo
              // the multiplication
              mantissaDivisor = oldMantissaB;
            } else {
              BigInteger bigpow = BigInteger.valueOf(helper.GetRadix());
              mantissaDivisor=mantissaDivisor.divide(bigpow);
            }
            adjust.Add(1);
          }
        }
        FastInteger expdiff = new FastInteger(helper.GetExponent(thisValue)).Subtract(
          helper.GetExponent(divisor));
        FastInteger fastDesiredExponent = new FastInteger(desiredExponent);
        if (integerMode == IntegerModeFixedScale) {
          if (ctx != null && ctx.getPrecision() != 0 &&
              new FastInteger(expdiff).Subtract(8).compareTo(ctx.getPrecision()) > 0 &&
              desiredExponent.signum()==0) { // NOTE: 8 guard digits
            // Result would require a too-high precision since
            // exponent difference is much higher
            throw new ArithmeticException("Result can't fit the precision");
          }
        }
        BigInteger lastRemainder=BigInteger.ZERO;
        if (mantcmp == 0) {
          result = new FastInteger2().Add(1);
          mantissaDividend = BigInteger.ZERO;
        } else {
          int check = 0;
          while (true) {
            BigInteger currentRemainder;
            BigInteger olddividend = mantissaDividend;
            BigInteger quotient;
BigInteger[] divrem=(mantissaDividend).divideAndRemainder(mantissaDivisor);
quotient=divrem[0];
currentRemainder=divrem[1];
            result.Add(quotient.intValue());
            mantissaDividend = currentRemainder;
            if (ctx != null && ctx.getPrecision() != 0 &&
                resultPrecision.compareTo(ctx.getPrecision()) == 0) {
              break;
            }
            if (currentRemainder.signum()==0 && adjust.signum() >= 0) {
              break;
            }
            // NOTE: 5 is an arbitrary threshold
            if (check == 5 && (ctx == null || ctx.getPrecision() == 0) &&
                integerMode == IntegerModeRegular) {
              // Check for a non-terminating radix expansion
              // if using unlimited precision and not in integer
              // mode
              if (!helper.HasTerminatingRadixExpansion(olddividend, mantissaDivisor)) {
                throw new ArithmeticException("Result would have a nonterminating expansion");
              }
              check++;
            } else if (check < 5) {
              check++;
            }
            if (integerMode == IntegerModeFixedScale ||
                integerMode == IntegerModeNaturalScale) {
              if (new FastInteger(expdiff).Subtract(fastDesiredExponent).compareTo(adjust) <= 0) {
                // Value is a full integer or has a fractional part
                break;
              }
            }
            adjust.Add(1);
            if (result.signum() != 0) {
              resultPrecision.Add(1);
            }
            lastRemainder=mantissaDividend;
            result.Multiply(helper.GetRadix());
            mantissaDividend = helper.MultiplyByRadixPower(mantissaDividend, 1);
          }
        }
        // mantissaDividend now has the remainder
        FastInteger exp = new FastInteger(expdiff).Subtract(adjust);
        if (integerMode == IntegerModeFixedScale) {
          FastInteger expshift = new FastInteger(exp).Subtract(fastDesiredExponent);
          if(remainder!=null){
            remainder[0]=helper.CreateNew(lastRemainder,
                                          fastDesiredExponent.AsBigInteger());
          }
          if (result.signum() != 0) {
            if (expshift.signum() > 0) {
              // Exponent is greater than desired exponent
              if (ctx != null && ctx.getPrecision() != 0 &&
                  expshift.compareTo(new FastInteger(ctx.getPrecision()).Add(8)) > 0) {
                // Result would require a too-high precision since
                // exponent shift is much higher
                throw new ArithmeticException("Result can't fit the precision");
              }
              mantissaDividend = helper.MultiplyByRadixPower(result.AsBigInteger(), expshift);
              if (negA ^ negB) {
                mantissaDividend=mantissaDividend.negate();
              }
              return helper.CreateNew(mantissaDividend, desiredExponent);
            } else if (expshift.signum() < 0) {
              // Exponent is less than desired exponent
              expshift.Negate();
              if (expshift.compareTo(resultPrecision) > 0) {
                // Exponent minus desired exponent
                // is greater than the result's precision,
                // so the result would be reduced to zero
                if (ctx != null && ctx.getRounding() == Rounding.Down && !ctx.getHasFlags()) {
                  return helper.CreateNew(BigInteger.ZERO, desiredExponent);
                }
              }
              if (ctx != null && ctx.getRounding() == Rounding.Down && !ctx.getHasFlags()) {
                IShiftAccumulator accum = helper.CreateShiftAccumulator(result.AsBigInteger());
                accum.ShiftRight(expshift);
                mantissaDividend = accum.getShiftedInt();
                if (negA ^ negB) {
                  mantissaDividend=mantissaDividend.negate();
                }
                return helper.CreateNew(mantissaDividend, desiredExponent);
              } else {
                mantissaDividend = RoundToScale(
                  result.AsBigInteger(),
                  mantissaDividend,
                  mantissaDivisor,
                  expshift,
                  negA ^ negB,
                  ctx);
                return helper.CreateNew(mantissaDividend, desiredExponent);
              }
            } else {
              mantissaDividend = RoundToScale(
                result.AsBigInteger(),
                mantissaDividend,
                mantissaDivisor,
                expshift,
                negA ^ negB,
                ctx);
              return helper.CreateNew(mantissaDividend, desiredExponent);
            }
          } else if (ctx != null && ctx.getRounding() == Rounding.Down && !ctx.getHasFlags()) {
            return helper.CreateNew(BigInteger.ZERO, desiredExponent);
          }
        }
        int lastDiscarded = 0;
        int olderDiscarded = 0;
        if (!(mantissaDividend.signum()==0)) {
          BigInteger halfDivisor = (mantissaDivisor.shiftRight(1));
          int cmpHalf = mantissaDividend.compareTo(halfDivisor);
          if ((cmpHalf == 0) && mantissaDivisor.testBit(0)==false) {
            // remainder is exactly half
            lastDiscarded = (helper.GetRadix() / 2);
            olderDiscarded = 0;
          } else if (cmpHalf > 0) {
            // remainder is greater than half
            lastDiscarded = (helper.GetRadix() / 2);
            olderDiscarded = 1;
          } else {
            // remainder is less than half
            lastDiscarded = 0;
            olderDiscarded = 1;
          }
        }
        BigInteger bigResult = result.AsBigInteger();
        if (negA ^ negB) {
          bigResult=bigResult.negate();
        }
        return RoundToPrecision(
          helper.CreateNew(
            bigResult, exp.AsBigInteger()),
          ctx,
          lastDiscarded, olderDiscarded);
      }
    }

    /**
     * Gets the lesser value between two values, ignoring their signs. If
     * the absolute values are equal, has the same effect as Min.
     * @param a A T object.
     * @param b A T object.
     */
    public T MinMagnitude(T a, T b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      int cmp = compareTo(helper.Abs(a), helper.Abs(b));
      if (cmp == 0) return Min(a, b);
      return (cmp < 0) ? a : b;
    }
    /**
     * Gets the greater value between two values, ignoring their signs.
     * If the absolute values are equal, has the same effect as Max.
     * @param a A T object.
     * @param b A T object.
     */
    public T MaxMagnitude(T a, T b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      int cmp = compareTo(helper.Abs(a), helper.Abs(b));
      if (cmp == 0) return Max(a, b);
      return (cmp > 0) ? a : b;
    }
    /**
     * Gets the greater value between two T values.
     * @param a A T object.
     * @param b A T object.
     * @return The larger value of the two objects.
     */
    public T Max(T a, T b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      int cmp = compareTo(a, b);
      if (cmp != 0)
        return cmp > 0 ? a : b;
      // Here the signs of both a and b can only be
      // equal (negative zeros are not supported)
      if (helper.GetSign(a) >= 0) {
        return helper.GetExponent(a).compareTo(helper.GetExponent(b)) > 0 ? a : b;
      } else {
        return helper.GetExponent(a).compareTo(helper.GetExponent(b)) > 0 ? b : a;
      }
    }

    /**
     * Multiplies two T objects.
     * @param thisValue A T object.
     * @param decfrac A T object.
     * @param ctx A PrecisionContext object.
     * @return The product of the two objects.
     */
    public T Multiply(T thisValue, T decfrac, PrecisionContext ctx) {
      BigInteger bigintOp2 = helper.GetExponent(decfrac);
      BigInteger newexp = (helper.GetExponent(thisValue).add(bigintOp2));
      T ret = helper.CreateNew(helper.GetMantissa(thisValue).multiply(helper.GetMantissa(decfrac)), newexp);
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
     */
    public T MultiplyAndAdd(T thisValue, T multiplicand,
                            T augend,
                            PrecisionContext ctx) {
      BigInteger bigintOp2 = helper.GetExponent(multiplicand);
      BigInteger newexp = (helper.GetExponent(thisValue).add(bigintOp2));
      bigintOp2 = helper.GetMantissa(multiplicand);
      bigintOp2=bigintOp2.multiply(helper.GetMantissa(thisValue));
      T addend = helper.CreateNew(bigintOp2,newexp);
      return Add(addend, augend, ctx);
    }

    /**
     * 
     * @param thisValue A T object.
     * @param context A PrecisionContext object.
     */
    public T RoundToBinaryPrecision(
      T thisValue,
      PrecisionContext context
     ) {
      return RoundToBinaryPrecision(thisValue, context, 0, 0);
    }
    private T RoundToBinaryPrecision(
      T thisValue,
      PrecisionContext context,
      int lastDiscarded,
      int olderDiscarded
     ) {
      if ((context) == null) return thisValue;
      if (context.getPrecision() == 0 && !context.getHasExponentRange() &&
          (lastDiscarded | olderDiscarded) == 0)
        return thisValue;
      if(context.getPrecision()==0 || helper.GetRadix()==2)
        return RoundToPrecision(thisValue,context,lastDiscarded,olderDiscarded);
      FastInteger fastEMin = (context.getHasExponentRange()) ? new FastInteger().Add(context.getEMin()) : null;
      FastInteger fastEMax = (context.getHasExponentRange()) ? new FastInteger().Add(context.getEMax()) : null;
      int[] signals = new int[1];
      T dfrac = RoundToBinaryPrecisionInternal(
        thisValue,
        context.getPrecision(),
        context.getRounding(), fastEMin, fastEMax,
        lastDiscarded,
        olderDiscarded,
        signals);
      // Clamp exponents to eMax + 1 - precision
      // if directed
      if (context.getClampNormalExponents() && dfrac != null) {
        BigInteger maxMantissa = BigInteger.ONE;
        long prec=context.getPrecision();
        while(prec>0){
          int shift=(int)Math.min(1000000,prec);
          maxMantissa=maxMantissa.shiftLeft(shift);
          prec-=shift;
        }
        maxMantissa=maxMantissa.subtract(BigInteger.ONE);
        // Get the digit length of the maximum possible mantissa
        // for the given binary precision
        long digitCount=helper.CreateShiftAccumulator(maxMantissa).getDigitLength();
        FastInteger clamp = new FastInteger(fastEMax).Add(1).Subtract(digitCount);
        FastInteger fastExp = new FastInteger(helper.GetExponent(dfrac));
        if (fastExp.compareTo(clamp) > 0) {
          BigInteger bigmantissa = helper.GetMantissa(dfrac);
          int sign = bigmantissa.signum();
          if (sign != 0) {
            if (sign < 0) bigmantissa=bigmantissa.negate();
            FastInteger expdiff = new FastInteger(fastExp).Subtract(clamp);
            bigmantissa = helper.MultiplyByRadixPower(bigmantissa, expdiff);
            if (sign < 0) bigmantissa=bigmantissa.negate();
          }
          if (signals != null)
            signals[0] |= PrecisionContext.FlagClamped;
          dfrac = helper.CreateNew(bigmantissa, clamp.AsBigInteger());
        }
      }
      if (context.getHasFlags()) {
        context.setFlags(context.getFlags()|signals[0]);
      }
      return dfrac;
    }
    
    /**
     * 
     * @param thisValue A T object.
     * @param context A PrecisionContext object.
     */
    public T RoundToPrecision(
      T thisValue,
      PrecisionContext context
     ) {
      return RoundToPrecision(thisValue, context, 0, 0);
    }
    private T RoundToPrecision(
      T thisValue,
      PrecisionContext context,
      int lastDiscarded,
      int olderDiscarded
     ) {
      if ((context) == null) return thisValue;
      if (context.getPrecision() == 0 && !context.getHasExponentRange() &&
          (lastDiscarded | olderDiscarded) == 0)
        return thisValue;
      FastInteger fastEMin = (context.getHasExponentRange()) ? new FastInteger().Add(context.getEMin()) : null;
      FastInteger fastEMax = (context.getHasExponentRange()) ? new FastInteger().Add(context.getEMax()) : null;
      if (context.getPrecision() > 0 && context.getPrecision() <= 18 &&
          (lastDiscarded | olderDiscarded) == 0) {
        // Check if rounding is necessary at all
        // for small precisions
        BigInteger mantabs = (helper.GetMantissa(thisValue)).abs();
        if (mantabs.compareTo(helper.MultiplyByRadixPower(BigInteger.ONE, context.getPrecision())) < 0) {
          if (!context.getHasExponentRange())
            return thisValue;
          FastInteger fastExp = new FastInteger().Add(helper.GetExponent(thisValue));
          FastInteger fastAdjustedExp = new FastInteger(fastExp)
            .Add(context.getPrecision()).Subtract(1);
          FastInteger fastNormalMin = new FastInteger(fastEMin)
            .Add(context.getPrecision()).Subtract(1);
          if (fastAdjustedExp.compareTo(fastEMax) <= 0 &&
              fastAdjustedExp.compareTo(fastNormalMin) >= 0) {
            return thisValue;
          }
        }
      }
      int[] signals = new int[1];
      T dfrac = RoundToPrecisionInternal(
        thisValue,
        context.getPrecision(),
        context.getRounding(), fastEMin, fastEMax,
        lastDiscarded,
        olderDiscarded,
        signals);
      if (context.getClampNormalExponents() && dfrac != null) {
        // Clamp exponents to eMax + 1 - precision
        // if directed
        FastInteger clamp = new FastInteger(fastEMax).Add(1).Subtract(context.getPrecision());
        FastInteger fastExp = new FastInteger(helper.GetExponent(dfrac));
        if (fastExp.compareTo(clamp) > 0) {
          BigInteger bigmantissa = helper.GetMantissa(dfrac);
          int sign = bigmantissa.signum();
          if (sign != 0) {
            if (sign < 0) bigmantissa=bigmantissa.negate();
            FastInteger expdiff = new FastInteger(fastExp).Subtract(clamp);
            bigmantissa = helper.MultiplyByRadixPower(bigmantissa, expdiff);
            if (sign < 0) bigmantissa=bigmantissa.negate();
          }
          if (signals != null)
            signals[0] |= PrecisionContext.FlagClamped;
          dfrac = helper.CreateNew(bigmantissa, clamp.AsBigInteger());
        }
      }
      if (context.getHasFlags()) {
        context.setFlags(context.getFlags()|signals[0]);
      }
      return dfrac;
    }

    /**
     * 
     * @param thisValue A T object.
     * @param otherValue A T object.
     * @param ctx A PrecisionContext object.
     */
    public T Quantize(
      T thisValue,
      T otherValue,
      PrecisionContext ctx
     ) {
      PrecisionContext tmpctx = (ctx == null ?
                                 new PrecisionContext(Rounding.HalfEven) :
                                 new PrecisionContext(ctx)).WithBlankFlags();
      BigInteger mantThis = (helper.GetMantissa(thisValue)).abs();
      BigInteger expThis = helper.GetExponent(thisValue);
      BigInteger expOther = helper.GetExponent(otherValue);
      int expcmp = expThis.compareTo(expOther);
      int signThis = helper.GetSign(thisValue);
      T ret = null;
      if (expcmp == 0) {
        ret = RoundToPrecision(thisValue, tmpctx);
      } else if (mantThis.signum()==0) {
        ret = helper.CreateNew(BigInteger.ZERO, expOther);
        ret = RoundToPrecision(ret, tmpctx);
      } else if (expcmp > 0) {
        // Other exponent is less
        FastInteger radixPower = new FastInteger(expThis).Subtract(expOther);
        if (tmpctx.getPrecision() > 0 &&
            radixPower.compareTo(new FastInteger(tmpctx.getPrecision()).Add(10)) > 0) {
          // Radix power is much too high for the current precision
          throw new ArithmeticException();
        }
        mantThis = helper.MultiplyByRadixPower(mantThis, radixPower);
        if (signThis < 0)
          mantThis=mantThis.negate();
        ret = helper.CreateNew(mantThis, expOther);
        ret = RoundToPrecision(ret, tmpctx);
      } else {
        // Other exponent is greater
        IShiftAccumulator accum = helper.CreateShiftAccumulator(mantThis);
        accum.ShiftRight(new FastInteger(expOther).Subtract(expThis));
        mantThis = accum.getShiftedInt();
        if (signThis < 0)
          mantThis=mantThis.negate();
        ret = helper.CreateNew(mantThis, expOther);
        ret = RoundToPrecision(ret, tmpctx, accum.getLastDiscardedDigit(),
                               accum.getOlderDiscardedDigits());
      }
      if ((tmpctx.getFlags() & PrecisionContext.FlagOverflow) != 0) {
        throw new ArithmeticException();
      }
      if (ret == null || !helper.GetExponent(ret).equals(expOther)) {
        throw new ArithmeticException();
      }
      if (signThis < 0 && helper.GetSign(ret) > 0) {
        BigInteger mantRet = helper.GetMantissa(ret);
        mantRet=(mantRet).negate();
        ret = helper.CreateNew(mantRet,helper.GetExponent(ret));
      }
      if (ctx != null && ctx.getHasFlags()) {
        int flags = tmpctx.getFlags();
        flags &= ~PrecisionContext.FlagUnderflow;
        ctx.setFlags(ctx.getFlags()|flags);
      }
      return ret;
    }

    private T QuantizeClose(
      T thisValue,
      T otherValue,
      PrecisionContext ctx
     ) {
      BigInteger expThis = helper.GetExponent(thisValue);
      BigInteger expOther = helper.GetExponent(otherValue);
      int expcmp = expThis.compareTo(expOther);
      T ret;
      PrecisionContext ctx2=PrecisionContext.Unlimited.WithBlankFlags();
      if(expcmp<0){
        // Other exponent is greater, so
        // result must be rounded
        int signThis = helper.GetSign(thisValue);
        BigInteger mantThis = (helper.GetMantissa(thisValue)).abs();
        IShiftAccumulator accum = helper.CreateShiftAccumulator(mantThis);
        accum.ShiftRight(new FastInteger(expOther).Subtract(expThis));
        if((accum.getLastDiscardedDigit()|accum.getOlderDiscardedDigits())!=0){
          // would be inexact
          ret=thisValue;
          return RoundToPrecision(ret,ctx);
        }
        mantThis = accum.getShiftedInt();
        if (signThis < 0)
          mantThis=mantThis.negate();
        ret = helper.CreateNew(mantThis, expOther);
        return RoundToPrecision(ret, ctx, accum.getLastDiscardedDigit(),
                                accum.getOlderDiscardedDigits());
      } else {
        // No need to round during Quantize
        ret = Quantize(thisValue,otherValue,ctx2);
        return RoundToPrecision(ret,ctx);
      }
    }
    
    /**
     * 
     * @param thisValue A T object.
     * @param ctx A PrecisionContext object.
     */
    public T RoundToIntegralExact(
      T thisValue,
      PrecisionContext ctx) {
      if (helper.GetExponent(thisValue).signum() >= 0) {
        return RoundToPrecision(thisValue, ctx);
      } else {
        PrecisionContext pctx = (ctx == null) ? null :
          ctx.WithPrecision(0).WithBlankFlags();
        T ret = Quantize(thisValue, helper.CreateNew(
          BigInteger.ONE, BigInteger.ZERO),
                         pctx);
        if (ctx != null && ctx.getHasFlags()) {
          ctx.setFlags(ctx.getFlags()|pctx.getFlags());
        }
        return ret;
      }
    }

    /**
     * 
     * @param thisValue A T object.
     * @param ctx A PrecisionContext object.
     */
    public T RoundToIntegralValue(
      T thisValue,
      PrecisionContext ctx
     ) {
      PrecisionContext pctx = (ctx == null) ? null :
        ctx.WithBlankFlags();
      T ret = RoundToIntegralExact(thisValue, pctx);
      if (ctx != null && ctx.getHasFlags()) {
        ctx.setFlags(ctx.getFlags()|(pctx.getFlags()&~(PrecisionContext.FlagInexact|PrecisionContext.FlagRounded)));
      }
      return ret;
    }

    /**
     * 
     * @param thisValue A T object.
     * @param ctx A PrecisionContext object.
     */
    public T Reduce(
      T thisValue,
      PrecisionContext ctx
     ) {
      T ret = RoundToPrecision(thisValue, ctx);
      if(ret!=null){
        BigInteger bigmant=(helper.GetMantissa(ret)).abs();
        FastInteger exp=new FastInteger(helper.GetExponent(ret));
        if(bigmant.signum()==0){
          exp=new FastInteger(0);
        } else {
          long radix=helper.GetRadix();
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
            exp.Add(1);
          }
        }
        boolean sign=(helper.GetSign(ret)<0);
        ret=helper.CreateNew(bigmant,exp.AsBigInteger());
        if(ctx!=null && ctx.getClampNormalExponents()){
          ret=RoundToPrecision(ret,ctx);
        }
        ret=EnsureSign(ret,sign);
      }
      return ret;
    }
    
    
    /**
     * 
     * @param thisValue A T object.
     * @param ctx A PrecisionContext object.
     */
    public T NaturalLog(T thisValue, PrecisionContext ctx) {
      if((thisValue)==null)throw new NullPointerException("thisValue");
      if((ctx)==null)throw new NullPointerException("ctx");
      if((ctx.getPrecision())<=0)throw new IllegalArgumentException("ctx.getPrecision()"+" not less than "+"0"+" ("+Long.toString((long)(long)(ctx.getPrecision()))+")");
      PrecisionContext ctx2=HigherPrecision(ctx);
      PrecisionContext ctxFinal=(ctx==null ? new PrecisionContext(Rounding.HalfEven) :
                                 ctx.WithNoFlags().WithRounding(Rounding.HalfEven));
      T one=helper.CreateNew(BigInteger.ONE,BigInteger.ZERO);
      T xp1=Add(thisValue,one,null);
      T xm1=Add(thisValue,Negate(one),null);
      T z=Divide(xm1,xp1,ctx2);
      T zsq=Multiply(z,z,null); // z^2
      T zaccum=Multiply(zsq,z,null); // z^3
      T zseries=z;
      T guess=helper.Abs(z);
      T roundedGuess=RoundToPrecision(guess,ctxFinal);
      int correct=0;
      FastInteger count=new FastInteger(3);
      while(true){
        T newguess;
        T element=Divide(zaccum,helper.CreateNew(
          count.AsBigInteger(),BigInteger.ZERO),ctx2);
        zseries=Add(zseries,element,null);
        newguess=Add(zseries,zseries,null);
        newguess=helper.Abs(newguess);
        T newRoundedGuess=RoundToPrecision(newguess,ctxFinal);
        if(compareTo(newRoundedGuess,roundedGuess)==0){
          correct++;
          if(correct>=4){
            break;
          }
        } else {
          correct=0;
        }
        zaccum=Multiply(zaccum,zsq,null);
        count.Add(2);
        guess=newguess;
        roundedGuess=newRoundedGuess;
      }
      return roundedGuess;
    }
    
    private static BigInteger FloorOfHalf(BigInteger bi) {
      if(bi.signum()>=0){
        return (bi.shiftRight(1));
      } else {
        return (bi.subtract(BigInteger.ONE)).divide(BigInteger.valueOf(2));
      }
    }
    
    private static PrecisionContext HigherPrecision(PrecisionContext ctx) {
      FastInteger fi=new FastInteger(ctx.getPrecision()).Add(6);
      long prec=fi.CanFitInInt64() ? fi.AsInt64() : Long.MAX_VALUE;
      prec=Math.max(16,prec);
      return ctx.WithPrecision(prec);
    }
    
    /**
     * 
     * @param thisValue A T object.
     * @param ctx A PrecisionContext object.
     */
    public T SquareRoot(T thisValue, PrecisionContext ctx) {
      if((thisValue)==null)throw new NullPointerException("thisValue");
      if((ctx)==null)throw new NullPointerException("ctx");
      if((ctx.getPrecision())<=0)throw new IllegalArgumentException("ctx.getPrecision()"+" not less than "+"0"+" ("+Long.toString((long)(long)(ctx.getPrecision()))+")");
      
      int sign=helper.GetSign(thisValue);
      if(sign<0)
        throw new ArithmeticException();
      PrecisionContext ctx2=(ctx.WithNoFlags().WithRounding(Rounding.HalfEven));
      PrecisionContext ctx2flags=HigherPrecision(ctx2).WithUnlimitedExponents();
      T idealExp=helper.CreateNew(
        BigInteger.ONE,
        FloorOfHalf(helper.GetExponent(thisValue)));
      if(sign==0){
        return QuantizeClose(thisValue,idealExp,ctx2);
      }
      T one=helper.CreateNew(BigInteger.ONE,BigInteger.ZERO);
      T a=one;
      T b=one;
      int correct=0;
      T result=null;
      T oldresult=null;
      while(true){
        T newB=null;
        T newA=null;
        for(int i=0;i<5;i++){
          newB=Add(a,b,null);
          T tmp=helper.Abs(Multiply(b,thisValue,null));
          newA=Add(a,tmp,null);
          a=newA;
          b=newB;
        }
        result=Divide(a,b,ctx2flags);
        if(!result.equals(null) &&
           compareTo(result,oldresult)==0){
          correct++;
          if(correct>=2){
            if(ctx.getHasFlags()){
              ctx.setFlags(ctx.getFlags()|ctx2flags.getFlags());
            }
            break;
          }
        } else {
          correct=0;
        }
        a=Reduce(newA,null);
        b=Reduce(newB,null);
        oldresult=result;
      }
      ctx2flags=ctx2.WithBlankFlags();
      result=QuantizeClose(result,idealExp,ctx2flags);
      if(ctx.getHasFlags()){
        ctx.setFlags(ctx.getFlags()|ctx2flags.getFlags());
      }
      return result;
    }


    private T RoundToBinaryPrecisionInternal(
      T thisValue,
      long precision,
      Rounding rounding,
      FastInteger fastEMin,
      FastInteger fastEMax,
      int lastDiscarded,
      int olderDiscarded,
      int[] signals
     ) {
      if ((precision) < 0) throw new IllegalArgumentException("precision" + " not greater or equal to " + "0" + " (" + Long.toString((long)(long)(precision)) + ")");
      if(helper.GetRadix()==2 || precision==0){
        return RoundToPrecisionInternal(thisValue,precision,rounding,
                                        fastEMin,fastEMax,lastDiscarded,
                                        olderDiscarded,signals);
      }
      boolean neg = helper.GetMantissa(thisValue).signum() < 0;
      BigInteger bigmantissa = helper.GetMantissa(thisValue);
      if (neg) bigmantissa=bigmantissa.negate();
      // save mantissa in case result is subnormal
      // and must be rounded again
      BigInteger oldmantissa = bigmantissa;
      BigInteger maxMantissa = BigInteger.ONE;
      long prec=precision;
      while(prec>0){
        int shift=(int)Math.min(1000000,prec);
        maxMantissa=maxMantissa.shiftLeft(shift);
        prec-=shift;
      }
      maxMantissa=maxMantissa.subtract(BigInteger.ONE);
      FastInteger exp = new FastInteger(helper.GetExponent(thisValue));
      int flags = 0;
      IShiftAccumulator accumMaxMant = helper.CreateShiftAccumulator(
        maxMantissa);
      // Get the digit length of the maximum possible mantissa
      // for the given binary precision
      long digitCount=accumMaxMant.getDigitLength();
      IShiftAccumulator accum = helper.CreateShiftAccumulator(
        bigmantissa, lastDiscarded, olderDiscarded);
      accum.ShiftToDigits(digitCount);
      while((accum.getShiftedInt()).compareTo(maxMantissa)>0){
        accum.ShiftRight(1);
      }
      FastInteger discardedBits = new FastInteger(accum.getDiscardedDigitCount());
      exp.Add(discardedBits);
      FastInteger adjExponent = new FastInteger(exp)
        .Add(accum.getDigitLength()).Subtract(1);
      FastInteger clamp = null;
      if(fastEMax!=null && adjExponent.compareTo(fastEMax)==0){
        // May or may not be an overflow depending on the mantissa
        FastInteger expdiff=new FastInteger(digitCount).Subtract(accum.getDigitLength());
        BigInteger currMantissa=accum.getShiftedInt();
        currMantissa=helper.MultiplyByRadixPower(currMantissa,expdiff);
        if((currMantissa).compareTo(maxMantissa)>0){
          // Mantissa too high, treat as overflow
          adjExponent.Add(1);
        }
      }
      if (fastEMax != null && adjExponent.compareTo(fastEMax) > 0) {
        if (oldmantissa.signum()==0) {
          flags |= PrecisionContext.FlagClamped;
          if (signals != null) signals[0] = flags;
          return helper.CreateNew(oldmantissa, fastEMax.AsBigInteger());
        }
        // Overflow
        flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
        if (rounding == Rounding.Unnecessary)
          throw new ArithmeticException("Rounding was required");
        if ((rounding == Rounding.Down ||
             rounding == Rounding.ZeroFiveUp ||
             (rounding == Rounding.Ceiling && neg) ||
             (rounding == Rounding.Floor && !neg))) {
          // Set to the highest possible value for
          // the given precision
          BigInteger overflowMant = maxMantissa;
          if (neg) overflowMant=overflowMant.negate();
          if (signals != null) signals[0] = flags;
          clamp = new FastInteger(fastEMax).Add(1).Subtract(digitCount);
          return helper.CreateNew(overflowMant, clamp.AsBigInteger());
        }
        if (signals != null) signals[0] = flags;
        return null;
      } else if (fastEMin != null && adjExponent.compareTo(fastEMin) < 0) {
        // Subnormal
        FastInteger fastETiny = new FastInteger(fastEMin)
          .Subtract(digitCount)
          .Add(1);
        if (oldmantissa.signum()!=0)
          flags |= PrecisionContext.FlagSubnormal;
        if (exp.compareTo(fastETiny) < 0) {
          FastInteger expdiff = new FastInteger(fastETiny).Subtract(exp);
          expdiff.Add(discardedBits);
          accum = helper.CreateShiftAccumulator(oldmantissa, lastDiscarded, olderDiscarded);
          accum.ShiftRight(expdiff);
          BigInteger newmantissa = accum.getShiftedInt();
          if ((accum.getDiscardedDigitCount()).signum() != 0 ||
              (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
            if (oldmantissa.signum()!=0)
              flags |= PrecisionContext.FlagRounded;
            if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
              flags |= PrecisionContext.FlagInexact;
              if (rounding == Rounding.Unnecessary)
                throw new ArithmeticException("Rounding was required");
            }
            if (Round(accum, rounding, neg, newmantissa)) {
              newmantissa=newmantissa.add(BigInteger.ONE);
            }
          }
          if (newmantissa.signum()==0)
            flags |= PrecisionContext.FlagClamped;
          if ((flags & (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact)) == (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact))
            flags |= PrecisionContext.FlagUnderflow | PrecisionContext.FlagRounded;
          if (signals != null) signals[0] = flags;
          if (neg) newmantissa=newmantissa.negate();
          return helper.CreateNew(newmantissa, fastETiny.AsBigInteger());
        }
      }
      boolean mantChanged=false;
      if ((accum.getDiscardedDigitCount()).signum() != 0 ||
          (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
        if (bigmantissa.signum()!=0)
          flags |= PrecisionContext.FlagRounded;
        bigmantissa = accum.getShiftedInt();
        if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
          flags |= PrecisionContext.FlagInexact;
          if (rounding == Rounding.Unnecessary)
            throw new ArithmeticException("Rounding was required");
        }
        if (Round(accum, rounding, neg, bigmantissa)) {
          bigmantissa=bigmantissa.add(BigInteger.ONE);
          mantChanged = true;
          if (bigmantissa.testBit(0)==false) {
            accum = helper.CreateShiftAccumulator(bigmantissa);
            accum.ShiftToDigits(digitCount);
            while((accum.getShiftedInt()).compareTo(maxMantissa)>0){
              accum.ShiftRight(1);
            }
            if ((accum.getDiscardedDigitCount()).signum() != 0) {
              exp.Add(accum.getDiscardedDigitCount());
              discardedBits.Add(accum.getDiscardedDigitCount());
              bigmantissa = accum.getShiftedInt();
            }
          }
        }
      }
      if (mantChanged && fastEMax != null) {
        // If mantissa changed, check for overflow again
        adjExponent = new FastInteger(exp);
        adjExponent.Add(accum.getDigitLength()).Subtract(1);
        if(fastEMax!=null && adjExponent.compareTo(fastEMax)==0 && mantChanged){
          // May or may not be an overflow depending on the mantissa
          FastInteger expdiff=new FastInteger(digitCount).Subtract(accum.getDigitLength());
          BigInteger currMantissa=accum.getShiftedInt();
          currMantissa=helper.MultiplyByRadixPower(currMantissa,expdiff);
          if((currMantissa).compareTo(maxMantissa)>0){
            // Mantissa too high, treat as overflow
            adjExponent.Add(1);
          }
        }
        if (adjExponent.compareTo(fastEMax) > 0) {
          flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
          if ((rounding == Rounding.Down ||
               rounding == Rounding.ZeroFiveUp ||
               (rounding == Rounding.Ceiling && neg) ||
               (rounding == Rounding.Floor && !neg))) {
            // Set to the highest possible value for
            // the given precision
            BigInteger overflowMant = maxMantissa;
            if (neg) overflowMant=overflowMant.negate();
            if (signals != null) signals[0] = flags;
            clamp = new FastInteger(fastEMax).Add(1).Subtract(digitCount);
            return helper.CreateNew(overflowMant, clamp.AsBigInteger());
          }
          if (signals != null) signals[0] = flags;
          return null;
        }
      }
      if (signals != null) signals[0] = flags;
      if (neg) bigmantissa=bigmantissa.negate();
      return helper.CreateNew(bigmantissa, exp.AsBigInteger());
    }

    
    private T RoundToPrecisionInternal(
      T thisValue,
      long precision,
      Rounding rounding,
      FastInteger fastEMin,
      FastInteger fastEMax,
      int lastDiscarded,
      int olderDiscarded,
      int[] signals
     ) {
      if ((precision) < 0) throw new IllegalArgumentException("precision" + " not greater or equal to " + "0" + " (" + Long.toString((long)(long)(precision)) + ")");
      boolean neg = helper.GetMantissa(thisValue).signum() < 0;
      BigInteger bigmantissa = helper.GetMantissa(thisValue);
      if (neg) bigmantissa=bigmantissa.negate();
      // save mantissa in case result is subnormal
      // and must be rounded again
      BigInteger oldmantissa = bigmantissa;
      FastInteger exp = new FastInteger(helper.GetExponent(thisValue));
      int flags = 0;
      IShiftAccumulator accum = helper.CreateShiftAccumulator(
        bigmantissa, lastDiscarded, olderDiscarded);
      boolean unlimitedPrec = (precision == 0);
      if (precision > 0) {
        accum.ShiftToDigits(precision);
      } else {
        precision = accum.getDigitLength();
      }
      FastInteger discardedBits = new FastInteger(accum.getDiscardedDigitCount());
      exp.Add(discardedBits);
      FastInteger adjExponent = new FastInteger(exp)
        .Add(accum.getDigitLength()).Subtract(1);
      FastInteger clamp = null;
      if (fastEMax != null && adjExponent.compareTo(fastEMax) > 0) {
        if (oldmantissa.signum()==0) {
          flags |= PrecisionContext.FlagClamped;
          if (signals != null) signals[0] = flags;
          return helper.CreateNew(oldmantissa, fastEMax.AsBigInteger());
        }
        // Overflow
        flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
        if (rounding == Rounding.Unnecessary)
          throw new ArithmeticException("Rounding was required");
        if (!unlimitedPrec &&
            (rounding == Rounding.Down ||
             rounding == Rounding.ZeroFiveUp ||
             (rounding == Rounding.Ceiling && neg) ||
             (rounding == Rounding.Floor && !neg))) {
          // Set to the highest possible value for
          // the given precision
          BigInteger overflowMant = helper.MultiplyByRadixPower(BigInteger.ONE, precision);
          overflowMant=overflowMant.subtract(BigInteger.ONE);
          if (neg) overflowMant=overflowMant.negate();
          if (signals != null) signals[0] = flags;
          clamp = new FastInteger(fastEMax).Add(1).Subtract(precision);
          return helper.CreateNew(overflowMant, clamp.AsBigInteger());
        }
        if (signals != null) signals[0] = flags;
        return null;
      } else if (fastEMin != null && adjExponent.compareTo(fastEMin) < 0) {
        // Subnormal
        FastInteger fastETiny = new FastInteger(fastEMin)
          .Subtract(precision)
          .Add(1);
        if (oldmantissa.signum()!=0)
          flags |= PrecisionContext.FlagSubnormal;
        if (exp.compareTo(fastETiny) < 0) {
          FastInteger expdiff = new FastInteger(fastETiny).Subtract(exp);
          expdiff.Add(discardedBits);
          accum = helper.CreateShiftAccumulator(oldmantissa, lastDiscarded, olderDiscarded);
          accum.ShiftRight(expdiff);
          BigInteger newmantissa = accum.getShiftedInt();
          if ((accum.getDiscardedDigitCount()).signum() != 0 ||
              (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
            if (oldmantissa.signum()!=0)
              flags |= PrecisionContext.FlagRounded;
            if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
              flags |= PrecisionContext.FlagInexact;
              if (rounding == Rounding.Unnecessary)
                throw new ArithmeticException("Rounding was required");
            }
            if (Round(accum, rounding, neg, newmantissa)) {
              newmantissa=newmantissa.add(BigInteger.ONE);
            }
          }
          if (newmantissa.signum()==0)
            flags |= PrecisionContext.FlagClamped;
          if ((flags & (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact)) == (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact))
            flags |= PrecisionContext.FlagUnderflow | PrecisionContext.FlagRounded;
          if (signals != null) signals[0] = flags;
          if (neg) newmantissa=newmantissa.negate();
          return helper.CreateNew(newmantissa, fastETiny.AsBigInteger());
        }
      }
      boolean expChanged = false;
      if ((accum.getDiscardedDigitCount()).signum() != 0 ||
          (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
        if (bigmantissa.signum()!=0)
          flags |= PrecisionContext.FlagRounded;
        bigmantissa = accum.getShiftedInt();
        if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
          flags |= PrecisionContext.FlagInexact;
          if (rounding == Rounding.Unnecessary)
            throw new ArithmeticException("Rounding was required");
        }
        if (Round(accum, rounding, neg, bigmantissa)) {
          bigmantissa=bigmantissa.add(BigInteger.ONE);
          if (bigmantissa.testBit(0)==false) {
            accum = helper.CreateShiftAccumulator(bigmantissa);
            accum.ShiftToDigits(precision);
            if ((accum.getDiscardedDigitCount()).signum() != 0) {
              exp.Add(accum.getDiscardedDigitCount());
              discardedBits.Add(accum.getDiscardedDigitCount());
              bigmantissa = accum.getShiftedInt();
              expChanged = true;
            }
          }
        }
      }
      if (expChanged && fastEMax != null) {
        // If exponent changed, check for overflow again
        adjExponent = new FastInteger(exp);
        adjExponent.Add(accum.getDigitLength()).Subtract(1);
        if (adjExponent.compareTo(fastEMax) > 0) {
          flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
          if (!unlimitedPrec &&
              (rounding == Rounding.Down ||
               rounding == Rounding.ZeroFiveUp ||
               (rounding == Rounding.Ceiling && neg) ||
               (rounding == Rounding.Floor && !neg))) {
            // Set to the highest possible value for
            // the given precision
            BigInteger overflowMant = helper.MultiplyByRadixPower(BigInteger.ONE, precision);
            overflowMant=overflowMant.subtract(BigInteger.ONE);
            if (neg) overflowMant=overflowMant.negate();
            if (signals != null) signals[0] = flags;
            clamp = new FastInteger(fastEMax).Add(1).Subtract(precision);
            return helper.CreateNew(overflowMant, clamp.AsBigInteger());
          }
          if (signals != null) signals[0] = flags;
          return null;
        }
      }
      if (signals != null) signals[0] = flags;
      if (neg) bigmantissa=bigmantissa.negate();
      return helper.CreateNew(bigmantissa, exp.AsBigInteger());
    }

    /**
     * 
     * @param thisValue A T object.
     * @param decfrac A T object.
     * @param ctx A PrecisionContext object.
     */
    public T Add(T thisValue, T decfrac, PrecisionContext ctx) {
      int expcmp = helper.GetExponent(thisValue).compareTo(helper.GetExponent(decfrac));
      T retval = null;
      if (expcmp == 0) {
        retval = helper.CreateNew(
          helper.GetMantissa(thisValue).add(helper.GetMantissa(decfrac)), helper.GetExponent(thisValue));
      } else {
        // choose the minimum exponent
        BigInteger resultExponent = (expcmp < 0 ? helper.GetExponent(thisValue) : helper.GetExponent(decfrac));
        T op1 = thisValue;
        T op2 = decfrac;
        BigInteger op1Exponent = helper.GetExponent(op1);
        BigInteger op2Exponent = helper.GetExponent(op2);
        BigInteger expdiff = (op1Exponent.subtract(op2Exponent)).abs();
        if (ctx != null && ctx.getPrecision() > 0) {
          // Check if exponent difference is too big for
          // radix-power calculation to work quickly
          if (expdiff.compareTo(BigInteger.valueOf(100)) >= 0) {
            FastInteger fastint = new FastInteger(expdiff);
            // If exponent difference is greater than the precision
            if (fastint.compareTo(ctx.getPrecision()) > 0) {
              int expcmp2 = op1Exponent.compareTo(op2Exponent);
              if (expcmp2 < 0) {
                BigInteger bigMant2=(helper.GetMantissa(op2)).abs();
                if(!(bigMant2.signum()==0)){
                  // first operand's exponent is less
                  // and second operand isn't zero
                  // the 8 digits at the end are guard digits
                  FastInteger tmp=new FastInteger(op2Exponent).Subtract(ctx.getPrecision()).Subtract(8);
                  if((helper.GetMantissa(op1)).abs().compareTo(bigMant2)>0){
                    // first mantissa's absolute value is greater, subtract precision again
                    tmp.Subtract(ctx.getPrecision());
                  }
                  op1Exponent = (tmp.AsBigInteger());
                }
              } else if (expcmp2 > 0) {
                BigInteger bigMant1=(helper.GetMantissa(op1)).abs();
                if(!(bigMant1.signum()==0)){
                  // first operand's exponent is greater
                  // and first operand isn't zero
                  // the 8 digits at the end are guard digits
                  FastInteger tmp=new FastInteger(op1Exponent).Subtract(ctx.getPrecision()).Subtract(8);
                  if((helper.GetMantissa(op2)).abs().compareTo(bigMant1)>0){
                    // second mantissa's absolute value is greater, subtract precision again
                    tmp.Subtract(ctx.getPrecision());
                  }
                  op2Exponent = (tmp.AsBigInteger());
                }
              }
              expcmp = op1Exponent.compareTo(op2Exponent);
              resultExponent = (expcmp < 0 ? op1Exponent : op2Exponent);
            }
          }
        }
        if (expcmp > 0) {
          BigInteger newmant = helper.RescaleByExponentDiff(
            helper.GetMantissa(op1), op1Exponent, op2Exponent);
          retval = helper.CreateNew(
            newmant.add(helper.GetMantissa(op2)), resultExponent);
        } else {
          BigInteger newmant = helper.RescaleByExponentDiff(
            helper.GetMantissa(op2), op1Exponent, op2Exponent);
          retval = helper.CreateNew(
            newmant.add(helper.GetMantissa(op1)), resultExponent);
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
     * @return Zero if the values are equal; a negative number is this instance
     * is less, or a positive number if this instance is greater.
     */
    public int compareTo(T thisValue, T decfrac) {
      if (decfrac == null) return 1;
      int s = helper.GetSign(thisValue);
      int ds = helper.GetSign(decfrac);
      if (s != ds) return (s < ds) ? -1 : 1;
      int expcmp = helper.GetExponent(thisValue).compareTo(helper.GetExponent(decfrac));
      int mantcmp = helper.GetMantissa(thisValue).compareTo(helper.GetMantissa(decfrac));
      if (mantcmp == 0) {
        // Special case: Mantissas are equal
        return s == 0 ? 0 : expcmp * s;
      }
      if (ds == 0) {
        // Special case: Second operand is zero
        return s;
      }
      if (s == 0) {
        // Special case: First operand is zero
        return -ds;
      }
      if (expcmp == 0) {
        return mantcmp;
      } else {
        BigInteger op1Exponent = helper.GetExponent(thisValue);
        BigInteger op2Exponent = helper.GetExponent(decfrac);
        BigInteger expdiff = (op1Exponent.subtract(op2Exponent)).abs();
        // Check if exponent difference is too big for
        // radix-power calculation to work quickly
        if (expdiff.compareTo(BigInteger.valueOf(100)) >= 0) {
          FastInteger fastint = new FastInteger(expdiff);
          BigInteger op1Mantissa = helper.GetMantissa(thisValue);
          BigInteger op2Mantissa = helper.GetMantissa(decfrac);
          long precision1 = helper.CreateShiftAccumulator(
            (op1Mantissa).abs()).getDigitLength();
          long precision2 = helper.CreateShiftAccumulator(
            (op2Mantissa).abs()).getDigitLength();
          long maxPrecision = Math.max(precision1, precision2);
          // If exponent difference is greater than the
          // maximum precision of the two operands
          if (fastint.compareTo(maxPrecision) > 0) {
            int expcmp2 = op1Exponent.compareTo(op2Exponent);
            if (expcmp2 < 0) {
              BigInteger bigMant2=(op2Mantissa).abs();
              if(!(bigMant2.signum()==0)){
                // first operand's exponent is less
                // and second operand isn't zero
                // the 8 digits at the end are guard digits
                FastInteger tmp=new FastInteger(op2Exponent).Subtract(maxPrecision).Subtract(8);
                if((op1Mantissa).abs().compareTo(bigMant2)>0){
                  // first mantissa's absolute value is greater, subtract precision again
                  tmp.Subtract(maxPrecision);
                }
                op1Exponent = (tmp.AsBigInteger());
              }
            } else if (expcmp2 > 0) {
              BigInteger bigMant1=(op1Mantissa).abs();
              if(!(bigMant1.signum()==0)){
                // first operand's exponent is greater
                // and first operand isn't zero
                // the 8 digits at the end are guard digits
                FastInteger tmp=new FastInteger(op1Exponent).Subtract(maxPrecision).Subtract(8);
                if((op2Mantissa).abs().compareTo(bigMant1)>0){
                  // second mantissa's absolute value is greater, subtract precision again
                  tmp.Subtract(maxPrecision);
                }
                op2Exponent = (tmp.AsBigInteger());
              }
            }
            expcmp = op1Exponent.compareTo(op2Exponent);
          }
        }
        if (expcmp > 0) {
          BigInteger newmant = helper.RescaleByExponentDiff(
            helper.GetMantissa(thisValue), op1Exponent, op2Exponent);
          return newmant.compareTo(helper.GetMantissa(decfrac));
        } else {
          BigInteger newmant = helper.RescaleByExponentDiff(
            helper.GetMantissa(decfrac), op1Exponent, op2Exponent);
          return helper.GetMantissa(thisValue).compareTo(newmant);
        }
      }
    }
  }


