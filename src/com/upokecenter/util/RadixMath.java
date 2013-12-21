package com.upokecenter.util;


//import java.math.*;


    /**
     * Encapsulates radix-independent arithmetic.
     */
  class RadixMath<T> {

    IRadixMathHelper<T> helper;

    public RadixMath (IRadixMathHelper<T> helper) {
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
                       boolean neg, FastInteger fastint) {
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
      int radix = helper.GetRadix();
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
      int sign = helper.GetSign(val);
      if (negative && sign > 0) {
        BigInteger bigmant = helper.GetMantissa(val);
        bigmant=(bigmant).negate();
        BigInteger e = helper.GetExponent(val);
        return helper.CreateNew(bigmant, e);
      } else if (!negative && sign < 0) {
        return Abs(val);
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
      FastInteger desiredScale = FastInteger.FromBig(
        helper.GetExponent(thisValue)).SubtractBig(
        helper.GetExponent(divisor));
      T ret = DivideInternal(thisValue, divisor,
                             PrecisionContext.ForRounding(Rounding.Down).WithBigPrecision(
                               ctx==null ? BigInteger.ZERO : ctx.getPrecision()),
                             IntegerModeFixedScale, BigInteger.ZERO,
                             null);
      boolean neg = (helper.GetSign(thisValue) < 0) ^ (helper.GetSign(divisor) < 0);
      // Now the exponent's sign can only be 0 or positive
      if (helper.GetSign(ret) == 0) {
        // Value is 0, so just change the exponent
        // to the preferred one
        BigInteger divisorExp=helper.GetExponent(divisor);
        ret = helper.CreateNew(BigInteger.ZERO, helper.GetExponent(thisValue).subtract(divisorExp));
      } else {
        if (desiredScale.signum() < 0) {
          // Desired scale is negative, shift left
          desiredScale.Negate();
          BigInteger bigmantissa = (helper.GetMantissa(ret)).abs();
          bigmantissa = helper.MultiplyByRadixPower(bigmantissa, desiredScale);
          if (helper.GetMantissa(ret).signum() < 0) bigmantissa=bigmantissa.negate();
          ret = helper.CreateNew(bigmantissa, helper.GetExponent(thisValue).subtract(helper.GetExponent(divisor)));
        } else if (desiredScale.signum() > 0) {
          // Desired scale is positive, shift away zeros
          // but not after scale is reached
          BigInteger bigmantissa = (helper.GetMantissa(ret)).abs();
          FastInteger fastexponent = FastInteger.FromBig(helper.GetExponent(ret));
          BigInteger bigradix = BigInteger.valueOf(helper.GetRadix());
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
                             PrecisionContext.ForRounding(Rounding.Down).WithBigPrecision(
                               ctx == null ? BigInteger.ZERO : ctx.getPrecision()),
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
    
    private T Abs(T value) {
      return (helper.GetSign(value)<0) ? Negate(value) : value;
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
        ret = RoundToPrecision(ret, ctx);
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
                             PrecisionContext.ForRounding(Rounding.HalfEven)
                             .WithBigPrecision(ctx == null ? BigInteger.ZERO : ctx.getPrecision()),
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
        ret = RoundToPrecision(ret, ctx);
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
      if((ctx.getPrecision()).signum()<=0)throw new IllegalArgumentException("ctx.getPrecision()"+" not less than "+"0"+" ("+(ctx.getPrecision())+")");
      if(!(ctx.getHasExponentRange()))throw new IllegalArgumentException("doesn't satisfy ctx.getHasExponentRange()");
      BigInteger minusone=(BigInteger.ONE).negate();
      FastInteger minexp=FastInteger.FromBig(ctx.getEMin()).SubtractBig(ctx.getPrecision()).AddInt(1);
      FastInteger bigexp=FastInteger.FromBig(helper.GetExponent(thisValue));
      if(bigexp.compareTo(minexp)<0){
        // Use a smaller exponent if the input exponent is already
        // very small
        minexp=FastInteger.Copy(bigexp).SubtractInt(2);
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
      if((ctx.getPrecision()).signum()<=0)throw new IllegalArgumentException("ctx.getPrecision()"+" not less than "+"0"+" ("+(ctx.getPrecision())+")");
      if(!(ctx.getHasExponentRange()))throw new IllegalArgumentException("doesn't satisfy ctx.getHasExponentRange()");
      PrecisionContext ctx2;
      int cmp=compareTo(thisValue,otherValue);
      if(cmp==0){
        return RoundToPrecision(thisValue,ctx.WithNoFlags());
      } else {
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
        BigInteger bigdir=BigInteger.ONE;
        if(cmp>0){
          bigdir=(bigdir).negate();
        }
        T quantum=helper.CreateNew(bigdir,minexp.AsBigInteger());
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
      if((ctx.getPrecision()).signum()<=0)throw new IllegalArgumentException("ctx.getPrecision()"+" not less than "+"0"+" ("+(ctx.getPrecision())+")");
      if(!(ctx.getHasExponentRange()))throw new IllegalArgumentException("doesn't satisfy ctx.getHasExponentRange()");
      BigInteger minusone=BigInteger.ONE;
      FastInteger minexp=FastInteger.FromBig(ctx.getEMin()).SubtractBig(ctx.getPrecision()).AddInt(1);
      FastInteger bigexp=FastInteger.FromBig(helper.GetExponent(thisValue));
      if(bigexp.compareTo(minexp)<0){
        // Use a smaller exponent if the input exponent is already
        // very small
        minexp=FastInteger.Copy(bigexp).SubtractInt(2);
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
        throw new ArithmeticException("Exponent not within exponent range: "+desiredExponent.toString());
      PrecisionContext ctx2 = (ctx == null) ?
        PrecisionContext.ForRounding(Rounding.HalfDown) :
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
        ctx.setFlags(ctx.getFlags()|flags);
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
      int signA = helper.GetSign(thisValue);
      int signB = helper.GetSign(divisor);
      if (signB == 0) {
        throw new ArithmeticException();
      }
      int radix=helper.GetRadix();
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
        FastInteger expDividend = FastInteger.FromBig(helper.GetExponent(thisValue));
        FastInteger expDivisor = FastInteger.FromBig(helper.GetExponent(divisor));
        FastInteger expdiff = FastInteger.Copy(expDividend).Subtract(expDivisor);
        FastInteger adjust = new FastInteger(0);
        FastInteger result = new FastInteger(0);
        FastInteger naturalExponent = FastInteger.Copy(expdiff);
        boolean negA = (signA < 0);
        boolean negB = (signB < 0);
        if (negA) mantissaDividend=mantissaDividend.negate();
        if (negB) mantissaDivisor=mantissaDivisor.negate();
        FastInteger fastPrecision=(ctx==null) ? new FastInteger(0) :
          FastInteger.FromBig(ctx.getPrecision());
        if(integerMode==IntegerModeFixedScale){
          FastInteger shift;
          BigInteger rem;
          FastInteger fastDesiredExponent = FastInteger.FromBig(desiredExponent);
          if(ctx!=null && ctx.getHasFlags() && fastDesiredExponent.compareTo(naturalExponent)>0){
            // Treat as rounded if the desired exponent is greater
            // than the "ideal" exponent
            ctx.setFlags(ctx.getFlags()|PrecisionContext.FlagRounded);
          }
          if(expdiff.compareTo(fastDesiredExponent)<=0){
            shift=FastInteger.Copy(fastDesiredExponent).Subtract(expdiff);
            BigInteger quo;
BigInteger[] divrem=(mantissaDividend).divideAndRemainder(mantissaDivisor);
quo=divrem[0];
rem=divrem[1];
            quo=RoundToScale(quo,rem,mantissaDivisor,shift,negA^negB,ctx);
            return helper.CreateNew(quo,desiredExponent);
          } else if (ctx != null && (ctx.getPrecision()).signum()!=0 &&
                     FastInteger.Copy(expdiff).SubtractInt(8).compareTo(fastPrecision) > 0
                    ) { // NOTE: 8 guard digits
            // Result would require a too-high precision since
            // exponent difference is much higher
            throw new ArithmeticException("Result can't fit the precision");
          } else {
            shift=FastInteger.Copy(expdiff).Subtract(fastDesiredExponent);
            mantissaDividend=helper.MultiplyByRadixPower(mantissaDividend,shift);
            BigInteger quo;
BigInteger[] divrem=(mantissaDividend).divideAndRemainder(mantissaDivisor);
quo=divrem[0];
rem=divrem[1];
            quo=RoundToScale(quo,rem,mantissaDivisor,new FastInteger(0),negA^negB,ctx);
            return helper.CreateNew(quo,desiredExponent);
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
        if (negA ^ negB) {
          bigResult=bigResult.negate();
        }
        if(ctx!=null && ctx.getHasFlags() && exp.compareTo(naturalExponent)>0){
          // Treat as rounded if the true exponent is greater
          // than the "ideal" exponent
          ctx.setFlags(ctx.getFlags()|PrecisionContext.FlagRounded);
        }
        BigInteger bigexp=exp.AsBigInteger();
        T retval=helper.CreateNew(
          bigResult, bigexp);
        if(atMaxPrecision && !ctx.getHasExponentRange()){
          // At this point, the check for rounding with Rounding.Unnecessary
          // already occurred above
          if(!RoundGivenDigits(lastDiscarded,olderDiscarded,rounding,negA^negB,posBigResult)){
            if(ctx!=null && ctx.getHasFlags() && (lastDiscarded|olderDiscarded)!=0){
              ctx.setFlags(ctx.getFlags()|PrecisionContext.FlagInexact|PrecisionContext.FlagRounded);
            }
            return retval;
          } else if(posBigResult.testBit(0)==false && (helper.GetRadix()&1)==0){
            posBigResult=posBigResult.add(BigInteger.ONE);
            if(negA^negB)posBigResult=posBigResult.negate();
            if(ctx!=null && ctx.getHasFlags() && (lastDiscarded|olderDiscarded)!=0){
              ctx.setFlags(ctx.getFlags()|PrecisionContext.FlagInexact|PrecisionContext.FlagRounded);
            }
            return helper.CreateNew(posBigResult,bigexp);
          }
        }
        if(atMaxPrecision && ctx.getHasExponentRange()){
          BigInteger fastAdjustedExp = FastInteger.Copy(exp)
            .AddBig(ctx.getPrecision()).SubtractInt(1).AsBigInteger();
          if(fastAdjustedExp.compareTo(ctx.getEMin())>=0 && fastAdjustedExp.compareTo(ctx.getEMax())<=0){
            // At this point, the check for rounding with Rounding.Unnecessary
            // already occurred above
            if(!RoundGivenDigits(lastDiscarded,olderDiscarded,rounding,negA^negB,posBigResult)){
              if(ctx!=null && ctx.getHasFlags() && (lastDiscarded|olderDiscarded)!=0){
                ctx.setFlags(ctx.getFlags()|PrecisionContext.FlagInexact|PrecisionContext.FlagRounded);
              }
              return retval;
            } else if(posBigResult.testBit(0)==false && (helper.GetRadix()&1)==0){
              posBigResult=posBigResult.add(BigInteger.ONE);
              if(negA^negB)posBigResult=posBigResult.negate();
              if(ctx!=null && ctx.getHasFlags() && (lastDiscarded|olderDiscarded)!=0){
                ctx.setFlags(ctx.getFlags()|PrecisionContext.FlagInexact|PrecisionContext.FlagRounded);
              }
              return helper.CreateNew(posBigResult,bigexp);
            }
          }
        }
        return RoundToPrecisionWithShift(
          retval,
          ctx,
          lastDiscarded, olderDiscarded, new FastInteger(0));
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
      int cmp = compareTo(Abs(a), Abs(b));
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
      int cmp = compareTo(Abs(a), Abs(b));
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
      return RoundToBinaryPrecisionWithShift(thisValue, context, 0, 0, new FastInteger(0));
    }
    private T RoundToBinaryPrecisionWithShift(
      T thisValue,
      PrecisionContext context,
      int lastDiscarded,
      int olderDiscarded,
      FastInteger shift
     ) {
      if ((context) == null) return thisValue;
      if ((context.getPrecision()).signum()==0 && !context.getHasExponentRange() &&
          (lastDiscarded | olderDiscarded) == 0 && shift.signum()==0)
        return thisValue;
      if((context.getPrecision()).signum()==0 || helper.GetRadix()==2)
        return RoundToPrecisionWithShift(thisValue,context,lastDiscarded,olderDiscarded,shift);
      FastInteger fastEMin = (context.getHasExponentRange()) ? FastInteger.FromBig(context.getEMin()) : null;
      FastInteger fastEMax = (context.getHasExponentRange()) ? FastInteger.FromBig(context.getEMax()) : null;
      FastInteger fastPrecision = FastInteger.FromBig(context.getPrecision());
      int[] signals = new int[1];
      T dfrac = RoundToPrecisionInternal(
        thisValue,fastPrecision,
        context.getRounding(), fastEMin, fastEMax,
        lastDiscarded,
        olderDiscarded,
        shift,true,
        signals);
      // Clamp exponents to eMax + 1 - precision
      // if directed
      if (context.getClampNormalExponents() && dfrac != null) {
        FastInteger digitCount=null;
        if(helper.GetRadix()==2){
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
          BigInteger bigmantissa = helper.GetMantissa(dfrac);
          int sign = bigmantissa.signum();
          if (sign != 0) {
            if (sign < 0) bigmantissa=bigmantissa.negate();
            FastInteger expdiff = FastInteger.Copy(fastExp).Subtract(clamp);
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
      return RoundToPrecisionWithShift(thisValue, context, 0, 0,new FastInteger(0));
    }
    private T RoundToPrecisionWithShift(
      T thisValue,
      PrecisionContext context,
      int lastDiscarded,
      int olderDiscarded,
      FastInteger shift
     ) {
      if ((context) == null) return thisValue;
      if ((context.getPrecision()).signum()==0 && !context.getHasExponentRange() &&
          (lastDiscarded | olderDiscarded) == 0 && shift.signum()==0)
        return thisValue;
      FastInteger fastEMin = (context.getHasExponentRange()) ? FastInteger.FromBig(context.getEMin()) : null;
      FastInteger fastEMax = (context.getHasExponentRange()) ? FastInteger.FromBig(context.getEMax()) : null;
      FastInteger fastPrecision=FastInteger.FromBig(context.getPrecision());
      if (fastPrecision.signum() > 0 && fastPrecision.CompareToInt(34) <= 0 && shift.signum()==0) {
        // Check if rounding is necessary at all
        // for small precisions
        BigInteger mantabs = (helper.GetMantissa(thisValue)).abs();
        BigInteger radixPower=helper.MultiplyByRadixPower(BigInteger.ONE, fastPrecision);
        if (mantabs.compareTo(radixPower) < 0) {
          boolean neg=helper.GetSign(thisValue)<0;
          if(!RoundGivenDigits(lastDiscarded,olderDiscarded,context.getRounding(),
                               neg,mantabs)){
            if(context.getHasFlags() && (lastDiscarded|olderDiscarded)!=0){
              context.setFlags(context.getFlags()|PrecisionContext.FlagInexact|PrecisionContext.FlagRounded);
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
              context.setFlags(context.getFlags()|PrecisionContext.FlagInexact|PrecisionContext.FlagRounded);
            }
            mantabs=mantabs.add(BigInteger.ONE);
            if(mantabs.compareTo(radixPower)<0){
              if(neg)mantabs=mantabs.negate();
              if (!context.getHasExponentRange())
                return helper.CreateNew(mantabs,helper.GetExponent(thisValue));
              FastInteger fastExp = FastInteger.FromBig(helper.GetExponent(thisValue));
              FastInteger fastAdjustedExp = FastInteger.Copy(fastExp)
                .Add(fastPrecision).SubtractInt(1);
              FastInteger fastNormalMin = FastInteger.Copy(fastEMin)
                .Add(fastPrecision).SubtractInt(1);
              if (fastAdjustedExp.compareTo(fastEMax) <= 0 &&
                  fastAdjustedExp.compareTo(fastNormalMin) >= 0) {
                return helper.CreateNew(mantabs,helper.GetExponent(thisValue));
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
        shift,false,
        context.getHasFlags() ? signals : null);
      if (context.getClampNormalExponents() && dfrac != null) {
        // Clamp exponents to eMax + 1 - precision
        // if directed
        FastInteger clamp = FastInteger.Copy(fastEMax).AddInt(1).Subtract(fastPrecision);
        FastInteger fastExp = FastInteger.FromBig(helper.GetExponent(dfrac));
        if (fastExp.compareTo(clamp) > 0) {
          BigInteger bigmantissa = helper.GetMantissa(dfrac);
          int sign = bigmantissa.signum();
          if (sign != 0) {
            if (sign < 0) bigmantissa=bigmantissa.negate();
            FastInteger expdiff = FastInteger.Copy(fastExp).Subtract(clamp);
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
      BigInteger expOther = helper.GetExponent(otherValue);
      if(ctx!=null && !ctx.ExponentWithinRange(expOther))
        throw new ArithmeticException("Exponent not within exponent range: "+expOther.toString());
      PrecisionContext tmpctx = (ctx == null ?
                                 PrecisionContext.ForRounding(Rounding.HalfEven) :
                                 ctx.Copy()).WithBlankFlags();
      BigInteger mantThis = (helper.GetMantissa(thisValue)).abs();
      BigInteger expThis = helper.GetExponent(thisValue);
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
        FastInteger radixPower = FastInteger.FromBig(expThis).SubtractBig(expOther);
        if ((tmpctx.getPrecision()).signum()>0 &&
            radixPower.compareTo(FastInteger.FromBig(tmpctx.getPrecision()).AddInt(10)) > 0) {
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
        FastInteger shift=FastInteger.FromBig(expOther).SubtractBig(expThis);
        if (signThis < 0)
          mantThis=mantThis.negate();
        ret = helper.CreateNew(mantThis, expOther);
        ret = RoundToPrecisionWithShift(ret, tmpctx, 0, 0, shift);
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
    
    /**
     * 
     * @param thisValue A T object.
     * @param expOther A BigInteger object.
     * @param ctx A PrecisionContext object.
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
        T ret = Quantize(thisValue, helper.CreateNew(
          BigInteger.ONE, expOther),
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
     * @param expOther A BigInteger object.
     * @param ctx A PrecisionContext object.
     */
    public T RoundToExponentSimple(
      T thisValue,
      BigInteger expOther,
      PrecisionContext ctx) {
      if (helper.GetExponent(thisValue).compareTo(expOther) >= 0) {
        return RoundToPrecision(thisValue, ctx);
      } else {
        if(ctx!=null && !ctx.ExponentWithinRange(expOther))
          throw new ArithmeticException("Exponent not within exponent range: "+expOther.toString());
        BigInteger bigmantissa=helper.GetMantissa(thisValue);
        boolean neg=bigmantissa.signum()<0;
        if(neg)bigmantissa=bigmantissa.negate();
        FastInteger shift=FastInteger.FromBig(expOther).SubtractBig(helper.GetExponent(thisValue));
        IShiftAccumulator accum=helper.CreateShiftAccumulator(bigmantissa);
        accum.ShiftRight(shift);
        bigmantissa=accum.getShiftedInt();
        if(neg)bigmantissa=bigmantissa.negate();
        return RoundToPrecisionWithShift(
          helper.CreateNew(bigmantissa,expOther),ctx,
          accum.getLastDiscardedDigit(),
          accum.getOlderDiscardedDigits(), new FastInteger(0));
      }
    }

    /**
     * 
     * @param thisValue A T object.
     * @param ctx A PrecisionContext object.
     * @param exponent A BigInteger object.
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
        FastInteger exp=FastInteger.FromBig(helper.GetExponent(ret));
        if(bigmant.signum()==0){
          exp=new FastInteger(0);
        } else {
          int radix=helper.GetRadix();
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
        boolean sign=(helper.GetSign(ret)<0);
        ret=helper.CreateNew(bigmant,exp.AsBigInteger());
        if(ctx!=null && ctx.getClampNormalExponents()){
          PrecisionContext ctxtmp=ctx.WithBlankFlags();
          ret=RoundToPrecision(ret,ctxtmp);
          if(ctx.getHasFlags()){
            ctx.setFlags(ctx.getFlags()|(ctx.getFlags()&~PrecisionContext.FlagClamped));
          }
        }
        ret=EnsureSign(ret,sign);
      }
      return ret;
    }
    
    /*
    public T NaturalLog(T thisValue, PrecisionContext ctx) {
      if((ctx)==null)throw new NullPointerException("ctx");
      if((ctx.getPrecision()).signum()<=0)throw new IllegalArgumentException("ctx.getPrecision()"+" not less than "+"0"+" ("+(ctx.getPrecision())+")");
      throw new UnsupportedOperationException();
    }
     */
    /**
     * 
     * @param thisValue A T object.
     * @param ctx A PrecisionContext object.
     */
    public T SquareRoot(T thisValue, PrecisionContext ctx) {
      if((thisValue)==null)throw new NullPointerException("thisValue");
      if((ctx)==null)throw new NullPointerException("ctx");
      if((ctx.getPrecision()).signum()<=0)throw new IllegalArgumentException("ctx.getPrecision()"+" not less than "+"0"+" ("+(ctx.getPrecision())+")");
      
      int sign=helper.GetSign(thisValue);
      if(sign<0)
        throw new ArithmeticException();

      throw new UnsupportedOperationException();
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
      int[] signals
     ) {
      if (precision.signum() < 0) throw new IllegalArgumentException("precision" + " not greater or equal to " + "0" + " (" + precision + ")");
      if(helper.GetRadix()==2 || precision.signum()==0){
        // "binaryPrec" will have no special effect here
        binaryPrec=false;
      }
      BigInteger bigmantissa = helper.GetMantissa(thisValue);
      boolean neg = bigmantissa.signum() < 0;
      if (neg) bigmantissa=bigmantissa.negate();
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
        exp.Subtract(accum.getDiscardedDigitCount());
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
      if(signals!=null && fastEMin != null && adjExponent.compareTo(fastEMin) < 0){
        earlyRounded=accum.getShiftedInt();
        if(RoundGivenBigInt(accum, rounding, neg, earlyRounded)){
          earlyRounded=earlyRounded.add(BigInteger.ONE);
          if (earlyRounded.testBit(0)==false && (helper.GetRadix()&1)==0) {
            IShiftAccumulator accum2 = helper.CreateShiftAccumulator(earlyRounded);
            accum2.ShiftToDigits(fastPrecision);
            if ((accum2.getDiscardedDigitCount()).signum() != 0) {
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
          return helper.CreateNew(oldmantissa, fastEMax.AsBigInteger());
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
          if (neg) overflowMant=overflowMant.negate();
          if (signals != null) signals[0] = flags;
          clamp = FastInteger.Copy(fastEMax).AddInt(1)
            .Subtract(fastPrecision);
          return helper.CreateNew(overflowMant, clamp.AsBigInteger());
        }
        if (signals != null) signals[0] = flags;
        return null;
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
        if (exp.compareTo(fastETiny) < 0) {
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
            if ((flags & (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact)) == (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact))
              flags |= PrecisionContext.FlagUnderflow | PrecisionContext.FlagRounded;
            signals[0] = flags;
          }
          if (neg) newmantissa.Negate();
          return helper.CreateNew(newmantissa.AsBigInteger(), fastETiny.AsBigInteger());
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
          if (bigmantissa.testBit(0)==false && (helper.GetRadix()&1)==0) {
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
            if (neg) overflowMant=overflowMant.negate();
            if (signals != null) signals[0] = flags;
            clamp = FastInteger.Copy(fastEMax).AddInt(1)
              .Subtract(fastPrecision);
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
        T op1 = thisValue;
        T op2 = decfrac;
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
            BigInteger op1MantAbs=(helper.GetMantissa(op1)).abs();
            BigInteger op2MantAbs=(helper.GetMantissa(op2)).abs();
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
                  FastInteger tmp=FastInteger.Copy(fastOp2Exp).SubtractInt(8)
                    .Subtract(digitLength1)
                    .SubtractBig(ctx.getPrecision());
                  FastInteger newDiff=FastInteger.Copy(tmp).Subtract(fastOp2Exp).Abs();
                  if(newDiff.compareTo(expdiff)<0){
                    // Can be treated as almost zero
                    if(helper.GetSign(thisValue)==helper.GetSign(decfrac)){
                      FastInteger digitLength2=helper.CreateShiftAccumulator(
                        op2MantAbs).GetDigitLength();
                      if(digitLength2.compareTo(fastPrecision)<0){
                        // Second operand's precision too short
                        FastInteger precisionDiff=FastInteger.Copy(fastPrecision).Subtract(digitLength2);
                        op2MantAbs=helper.MultiplyByRadixPower(
                          op2MantAbs,precisionDiff);
                        BigInteger bigintTemp=precisionDiff.AsBigInteger();
                        op2Exponent=op2Exponent.subtract(bigintTemp);
                        if(helper.GetSign(decfrac)<0)op2MantAbs=op2MantAbs.negate();
                        decfrac=helper.CreateNew(op2MantAbs,op2Exponent);
                        return RoundToPrecisionWithShift(decfrac,ctx,0,1, new FastInteger(0));
                      } else {
                        return RoundToPrecisionWithShift(decfrac,ctx,0,1, new FastInteger(0));
                      }
                    } else {
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
                  FastInteger tmp=FastInteger.Copy(fastOp1Exp).SubtractInt(8)
                    .Subtract(digitLength2)
                    .SubtractBig(ctx.getPrecision());
                  FastInteger newDiff=FastInteger.Copy(tmp).Subtract(fastOp1Exp).Abs();
                  if(newDiff.compareTo(expdiff)<0){
                    // Can be treated as almost zero
                    if(helper.GetSign(thisValue)==helper.GetSign(decfrac)){
                      FastInteger digitLength1=helper.CreateShiftAccumulator(op1MantAbs).GetDigitLength();
                      if(digitLength1.compareTo(fastPrecision)<0){
                        // First operand's precision too short
                        FastInteger precisionDiff=FastInteger.Copy(fastPrecision).Subtract(digitLength1);
                        op1MantAbs=helper.MultiplyByRadixPower(
                          op1MantAbs,precisionDiff);
                        BigInteger bigintTemp=precisionDiff.AsBigInteger();
                        op1Exponent=op1Exponent.subtract(bigintTemp);
                        if(helper.GetSign(thisValue)<0)op1MantAbs=op1MantAbs.negate();
                        thisValue=helper.CreateNew(op1MantAbs,op1Exponent);
                        return RoundToPrecisionWithShift(thisValue,ctx,0,1, new FastInteger(0));
                      } else {
                        return RoundToPrecisionWithShift(thisValue,ctx,0,1, new FastInteger(0));
                      }
                    } else {
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
      if (ds == 0) {
        // Special case: Second operand is zero
        return s;
      }
      if (s == 0) {
        // Special case: First operand is zero
        return -ds;
      }
      int expcmp = helper.GetExponent(thisValue).compareTo(helper.GetExponent(decfrac));
      int mantcmp = helper.GetMantissa(thisValue).compareTo(helper.GetMantissa(decfrac));
      if (mantcmp == 0) {
        // Special case: Mantissas are equal
        return s == 0 ? 0 : expcmp * s;
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
        return newmant.compareTo(helper.GetMantissa(decfrac));
      } else {
        BigInteger newmant = helper.RescaleByExponentDiff(
          helper.GetMantissa(decfrac), op1Exponent, op2Exponent);
        return helper.GetMantissa(thisValue).compareTo(newmant);
      }
    }
  }
