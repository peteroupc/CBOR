using System;
using System.Text;
using System.Numerics;

namespace PeterO {
    /// <summary> Encapsulates radix-independent arithmetic. </summary>
    /// <typeparam name='T'>Data type for a numeric value in a particular
    /// radix.</typeparam>
  class RadixMath<T> {

    IRadixMathHelper<T> helper;

    public RadixMath(IRadixMathHelper<T> helper) {
      this.helper = helper;
    }
    /// <summary> Gets the lesser value between two T values.</summary>
    /// <returns> The smaller value of the two objects.</returns>
    /// <param name='a'> A T object.</param>
    /// <param name='b'> A T object.</param>
    public T Min(T a, T b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      int cmp = CompareTo(a, b);
      if (cmp != 0)
        return cmp > 0 ? b : a;
      // Here the signs of both a and b can only be
      // equal (negative zeros are not supported)
      if (helper.GetSign(a) >= 0) {
        return (helper.GetExponent(a)).CompareTo(helper.GetExponent(b)) > 0 ? b : a;
      } else {
        return (helper.GetExponent(a)).CompareTo(helper.GetExponent(b)) > 0 ? a : b;
      }
    }


    private bool Round(IShiftAccumulator accum, Rounding rounding,
                       bool neg, BigInteger bigval) {
      bool incremented = false;
      int radix = helper.GetRadix();
      if (rounding == Rounding.HalfUp) {
        if (accum.LastDiscardedDigit >= (radix / 2)) {
          incremented = true;
        }
      } else if (rounding == Rounding.HalfEven) {
        if (accum.LastDiscardedDigit >= (radix / 2)) {
          if ((accum.LastDiscardedDigit > (radix / 2) || accum.OlderDiscardedDigits != 0)) {
            incremented = true;
          } else if (!bigval.IsEven) {
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

    private T EnsureSign(T val, bool negative) {
      if (val == null) return val;
      int sign = helper.GetSign(val);
      if (negative && sign > 0) {
        BigInteger bigmant = helper.GetMantissa(val);
        bigmant=-(BigInteger)bigmant;
        BigInteger e = helper.GetExponent(val);
        return helper.CreateNew(bigmant, e);
      } else if (!negative && sign < 0) {
        return helper.Abs(val);
      }
      return val;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <remarks/>
    public T DivideToIntegerNaturalScale(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      T ret = DivideInternal(thisValue, divisor,
                             new PrecisionContext(Rounding.Down),
                             IntegerModeNaturalScale, BigInteger.Zero,
                             null);
      bool neg = (helper.GetSign(thisValue) < 0) ^ (helper.GetSign(divisor) < 0);
      if (helper.GetExponent(ret).Sign < 0) {
        BigInteger bigmantissa = BigInteger.Abs(helper.GetMantissa(ret));
        IShiftAccumulator accum = helper.CreateShiftAccumulator(bigmantissa);
        accum.ShiftRight(new FastInteger(helper.GetExponent(ret)).Negate());
        bigmantissa = accum.ShiftedInt;
        if (helper.GetMantissa(ret).Sign < 0) bigmantissa = -bigmantissa;
        ret = helper.CreateNew(bigmantissa, BigInteger.Zero);
      }
      if (helper.GetSign(ret) == 0) {
        BigInteger divisorExp=helper.GetExponent(divisor);
        ret = helper.CreateNew(BigInteger.Zero, helper.GetExponent(thisValue) - (BigInteger)divisorExp);
      } else {
        FastInteger desiredScale = new FastInteger(helper.GetExponent(thisValue)).Subtract(
          helper.GetExponent(divisor));
        if (desiredScale.Sign < 0) {
          desiredScale.Negate();
          BigInteger bigmantissa = BigInteger.Abs(helper.GetMantissa(ret));
          bigmantissa = helper.MultiplyByRadixPower(bigmantissa, desiredScale);
          if (helper.GetMantissa(ret).Sign < 0) bigmantissa = -bigmantissa;
          ret = helper.CreateNew(bigmantissa, helper.GetExponent(thisValue) -
                                 (BigInteger)(helper.GetExponent(divisor)));
        } else if (desiredScale.Sign > 0) {
          BigInteger bigmantissa = BigInteger.Abs(helper.GetMantissa(ret));
          FastInteger fastexponent = new FastInteger(helper.GetExponent(ret));
          BigInteger bigradix = (BigInteger)(helper.GetRadix());
          while(true){
            BigInteger bigrem;
            BigInteger bigquo=BigInteger.DivRem(bigmantissa,bigradix,out bigrem);
            if(!bigrem.IsZero)
              break;
            bigmantissa=bigquo;
            fastexponent.Add(1);
          }
          if (helper.GetMantissa(ret).Sign < 0) bigmantissa = -bigmantissa;
          ret = helper.CreateNew(bigmantissa, fastexponent.AsBigInteger());
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
    /// <returns></returns>
    /// <remarks/>
    public T DivideToIntegerZeroScale(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      T ret = DivideInternal(thisValue, divisor,
                             new PrecisionContext(Rounding.Down).WithPrecision(ctx == null ? 0 : ctx.Precision),
                             IntegerModeFixedScale, BigInteger.Zero,
                             null);
      if (ctx != null) {
        PrecisionContext ctx2 = ctx.WithBlankFlags().WithUnlimitedExponents();
        ret = RoundToPrecision(ret, ctx2);
        if ((ctx2.Flags & PrecisionContext.FlagRounded) != 0) {
          throw new ArithmeticException("Result would require a higher precision");
        }
      }
      return ret;
    }
    
    private T Negate(T value){
      BigInteger mant=helper.GetMantissa(value);
      mant=-(BigInteger)mant;
      return helper.CreateNew(mant,helper.GetExponent(value));
    }

    /// <summary>Finds the remainder that results when dividing two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The remainder of the two objects.</returns>
    /// <remarks/>
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

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <remarks/>
    public T RemainderNear(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      T ret = DivideInternal(thisValue, divisor,
                             new PrecisionContext(Rounding.HalfEven)
                             .WithPrecision(ctx == null ? 0 : ctx.Precision),
                             IntegerModeFixedScale, BigInteger.Zero,
                             null);
      if (ctx != null) {
        PrecisionContext ctx2 = ctx.WithBlankFlags().WithUnlimitedExponents();
        ret = RoundToPrecision(ret, ctx2);
        if ((ctx2.Flags & PrecisionContext.FlagRounded) != 0) {
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


    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <remarks/>
    public T NextMinus(
      T thisValue,
      PrecisionContext ctx
     ){
      if((ctx)==null)throw new ArgumentNullException("ctx");
      if((ctx.Precision)<=0)throw new ArgumentException("ctx.Precision"+" not less than "+"0"+" ("+Convert.ToString((long)(long)(ctx.Precision),System.Globalization.CultureInfo.InvariantCulture)+")");
      if(!(ctx.HasExponentRange))throw new ArgumentException("doesn't satisfy ctx.HasExponentRange");
      BigInteger minusone=-(BigInteger)BigInteger.One;
      FastInteger minexp=new FastInteger(ctx.EMin).Subtract(ctx.Precision).Add(1);
      FastInteger bigexp=new FastInteger(helper.GetExponent(thisValue));
      if(bigexp.CompareTo(minexp)<0){
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

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='otherValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <remarks/>
    public T NextToward(
      T thisValue,
      T otherValue,
      PrecisionContext ctx
     ){
      if((ctx)==null)throw new ArgumentNullException("ctx");
      if((ctx.Precision)<=0)throw new ArgumentException("ctx.Precision"+" not less than "+"0"+" ("+Convert.ToString((long)(long)(ctx.Precision),System.Globalization.CultureInfo.InvariantCulture)+")");
      if(!(ctx.HasExponentRange))throw new ArgumentException("doesn't satisfy ctx.HasExponentRange");
      PrecisionContext ctx2;
      int cmp=CompareTo(thisValue,otherValue);
      if(cmp==0){
        return RoundToPrecision(thisValue,ctx.WithNoFlags());
      } else {
        FastInteger minexp=new FastInteger(ctx.EMin).Subtract(ctx.Precision).Add(1);
        FastInteger bigexp=new FastInteger(helper.GetExponent(thisValue));
        if(bigexp.CompareTo(minexp)<0){
          // Use a smaller exponent if the input exponent is already
          // very small
          minexp=new FastInteger(bigexp).Subtract(2);
        }
        BigInteger bigdir=BigInteger.One;
        if(cmp>0){
          bigdir=-(BigInteger)bigdir;
        }
        T quantum=helper.CreateNew(bigdir,minexp.AsBigInteger());
        T val=thisValue;
        ctx2=ctx.WithRounding((cmp>0) ? Rounding.Floor : Rounding.Ceiling).WithBlankFlags();
        val=Add(val,quantum,ctx2);
        if(ctx.HasFlags){
          ctx.Flags|=ctx2.Flags;
        }
        return val;
      }
    }
    
    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <remarks/>
    public T NextPlus(
      T thisValue,
      PrecisionContext ctx
     ){
      if((ctx)==null)throw new ArgumentNullException("ctx");
      if((ctx.Precision)<=0)throw new ArgumentException("ctx.Precision"+" not less than "+"0"+" ("+Convert.ToString((long)(long)(ctx.Precision),System.Globalization.CultureInfo.InvariantCulture)+")");
      if(!(ctx.HasExponentRange))throw new ArgumentException("doesn't satisfy ctx.HasExponentRange");
      BigInteger minusone=BigInteger.One;
      FastInteger minexp=new FastInteger(ctx.EMin).Subtract(ctx.Precision).Add(1);
      FastInteger bigexp=new FastInteger(helper.GetExponent(thisValue));
      if(bigexp.CompareTo(minexp)<0){
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

    /// <summary>Divides two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object.</param>
    /// <param name='desiredExponent'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The quotient of the two objects.</returns>
    /// <remarks/>
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
    /// <remarks/>
    public T Divide(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      return DivideInternal(thisValue, divisor,
                            ctx, IntegerModeRegular, BigInteger.Zero, null);
    }

    private BigInteger RoundToScale(
      BigInteger mantissa, // Assumes mantissa is nonnegative
      BigInteger remainder,// Assumes value is nonnegative
      BigInteger divisor,// Assumes value is nonnegative
      FastInteger shift,
      bool neg,
      PrecisionContext ctx
     ) {
      IShiftAccumulator accum;
      int lastDiscarded = 0;
      int olderDiscarded = 0;
      if (!(remainder.IsZero)) {
        BigInteger halfDivisor = (divisor >> 1);
        int cmpHalf = remainder.CompareTo(halfDivisor);
        if ((cmpHalf == 0) && divisor.IsEven) {
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
      Rounding rounding = (ctx == null) ? Rounding.HalfEven : ctx.Rounding;
      BigInteger newmantissa = accum.ShiftedInt;
      if ((accum.DiscardedDigitCount).Sign != 0 ||
          (accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
        if (!mantissa.IsZero)
          flags |= PrecisionContext.FlagRounded;
        if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
          flags |= PrecisionContext.FlagInexact;
          if (rounding == Rounding.Unnecessary)
            throw new ArithmeticException("Rounding was required");
        }
        if (Round(accum, rounding, neg, newmantissa)) {
          newmantissa += BigInteger.One;
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
    private const int IntegerModeNaturalScale = 2;
    private const int IntegerModeRegular = 0;

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
        throw new DivideByZeroException();
      }
      if (signA == 0) {
        T retval=default(T);
        if (integerMode == IntegerModeFixedScale) {
          retval=helper.CreateNew(BigInteger.Zero, desiredExponent);
        } else {
          BigInteger divExp=helper.GetExponent(divisor);
          retval=RoundToPrecision(helper.CreateNew(
            BigInteger.Zero, (helper.GetExponent(thisValue) -
                              (BigInteger)divExp)), ctx);
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
        bool negA = (signA < 0);
        bool negB = (signB < 0);
        if (negA) mantissaDividend = -mantissaDividend;
        if (negB) mantissaDivisor = -mantissaDivisor;
        FastInteger resultPrecision = new FastInteger(1);
        int mantcmp = mantissaDividend.CompareTo(mantissaDivisor);
        if (mantcmp < 0) {
          // dividend mantissa is less than divisor mantissa
          long dividendPrecision = helper.CreateShiftAccumulator(mantissaDividend).DigitLength;
          long divisorPrecision = helper.CreateShiftAccumulator(mantissaDivisor).DigitLength;
          long powerOfRadix = Math.Max(1, divisorPrecision - dividendPrecision);
          // multiply dividend mantissa so precisions are the same
          // (except if they're already the same, in which case multiply
          // by radix)
          mantissaDividend = helper.MultiplyByRadixPower(mantissaDividend, powerOfRadix);
          adjust.Add(powerOfRadix);
          if (mantissaDividend.CompareTo(mantissaDivisor) < 0) {
            // dividend mantissa is still less, multiply once more
            mantissaDividend = helper.MultiplyByRadixPower(mantissaDividend, 1);
            adjust.Add(1);
          }
        } else if (mantcmp > 0) {
          // dividend mantissa is greater than divisor mantissa
          long dividendPrecision = helper.CreateShiftAccumulator(mantissaDividend).DigitLength;
          long divisorPrecision = helper.CreateShiftAccumulator(mantissaDivisor).DigitLength;
          long powerOfRadix = (dividendPrecision - divisorPrecision);
          BigInteger oldMantissaB = mantissaDivisor;
          mantissaDivisor = helper.MultiplyByRadixPower(mantissaDivisor, powerOfRadix);
          adjust.Subtract(powerOfRadix);
          if (mantissaDividend.CompareTo(mantissaDivisor) < 0) {
            // dividend mantissa is now less, divide by radix power
            if (powerOfRadix == 1) {
              // no need to divide here, since thisValue would just undo
              // the multiplication
              mantissaDivisor = oldMantissaB;
            } else {
              BigInteger bigpow = (BigInteger)(helper.GetRadix());
              mantissaDivisor /= bigpow;
            }
            adjust.Add(1);
          }
        }
        FastInteger expdiff = new FastInteger(helper.GetExponent(thisValue)).Subtract(
          helper.GetExponent(divisor));
        FastInteger fastDesiredExponent = new FastInteger(desiredExponent);
        if (integerMode == IntegerModeFixedScale) {
          if (ctx != null && ctx.Precision != 0 &&
              new FastInteger(expdiff).Subtract(8).CompareTo(ctx.Precision) > 0 &&
              desiredExponent.IsZero) { // NOTE: 8 guard digits
            // Result would require a too-high precision since
            // exponent difference is much higher
            throw new ArithmeticException("Result can't fit the precision");
          }
        }
        BigInteger lastRemainder=BigInteger.Zero;
        if (mantcmp == 0) {
          result = new FastInteger2().Add(1);
          mantissaDividend = BigInteger.Zero;
        } else {
          int check = 0;
          while (true) {
            BigInteger currentRemainder;
            BigInteger olddividend = mantissaDividend;
            BigInteger quotient = BigInteger.DivRem(mantissaDividend, mantissaDivisor, out currentRemainder);
            result.Add((int)quotient);
            mantissaDividend = currentRemainder;
            if (ctx != null && ctx.Precision != 0 &&
                resultPrecision.CompareTo(ctx.Precision) == 0) {
              break;
            }
            if (currentRemainder.IsZero && adjust.Sign >= 0) {
              break;
            }
            // NOTE: 5 is an arbitrary threshold
            if (check == 5 && (ctx == null || ctx.Precision == 0) &&
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
              if (new FastInteger(expdiff).Subtract(fastDesiredExponent).CompareTo(adjust) <= 0) {
                // Value is a full integer or has a fractional part
                break;
              }
            }
            adjust.Add(1);
            if (result.Sign != 0) {
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
          if (result.Sign != 0) {
            if (expshift.Sign > 0) {
              // Exponent is greater than desired exponent
              if (ctx != null && ctx.Precision != 0 &&
                  expshift.CompareTo(new FastInteger(ctx.Precision).Add(8)) > 0) {
                // Result would require a too-high precision since
                // exponent shift is much higher
                throw new ArithmeticException();
              }
              mantissaDividend = helper.MultiplyByRadixPower(result.AsBigInteger(), expshift);
              if (negA ^ negB) {
                mantissaDividend = -mantissaDividend;
              }
              return helper.CreateNew(mantissaDividend, desiredExponent);
            } else if (expshift.Sign < 0) {
              // Exponent is less than desired exponent
              expshift.Negate();
              if (expshift.CompareTo(resultPrecision) > 0) {
                // Exponent minus desired exponent
                // is greater than the result's precision,
                // so the result would be reduced to zero
                if (ctx != null && ctx.Rounding == Rounding.Down && !ctx.HasFlags) {
                  return helper.CreateNew(BigInteger.Zero, desiredExponent);
                }
              }
              if (ctx != null && ctx.Rounding == Rounding.Down && !ctx.HasFlags) {
                IShiftAccumulator accum = helper.CreateShiftAccumulator(result.AsBigInteger());
                accum.ShiftRight(expshift);
                mantissaDividend = accum.ShiftedInt;
                if (negA ^ negB) {
                  mantissaDividend = -mantissaDividend;
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
          } else if (ctx != null && ctx.Rounding == Rounding.Down && !ctx.HasFlags) {
            return helper.CreateNew(BigInteger.Zero, desiredExponent);
          }
        }
        int lastDiscarded = 0;
        int olderDiscarded = 0;
        if (!(mantissaDividend.IsZero)) {
          BigInteger halfDivisor = (mantissaDivisor >> 1);
          int cmpHalf = mantissaDividend.CompareTo(halfDivisor);
          if ((cmpHalf == 0) && mantissaDivisor.IsEven) {
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
          bigResult = -bigResult;
        }
        return RoundToPrecision(
          helper.CreateNew(
            bigResult, exp.AsBigInteger()),
          ctx,
          lastDiscarded, olderDiscarded);
      }
    }

    /// <summary> Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Min.
    /// </summary>
    /// <returns></returns>
    /// <param name='a'> A T object.</param>
    /// <param name='b'> A T object.</param>
    public T MinMagnitude(T a, T b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      int cmp = CompareTo(helper.Abs(a), helper.Abs(b));
      if (cmp == 0) return Min(a, b);
      return (cmp < 0) ? a : b;
    }
    /// <summary> Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Max.
    /// </summary>
    /// <returns></returns>
    /// <param name='a'> A T object.</param>
    /// <param name='b'> A T object.</param>
    public T MaxMagnitude(T a, T b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      int cmp = CompareTo(helper.Abs(a), helper.Abs(b));
      if (cmp == 0) return Max(a, b);
      return (cmp > 0) ? a : b;
    }
    /// <summary> Gets the greater value between two T values. </summary>
    /// <returns> The larger value of the two objects.</returns>
    /// <param name='a'> A T object.</param>
    /// <param name='b'> A T object.</param>
    public T Max(T a, T b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      int cmp = CompareTo(a, b);
      if (cmp != 0)
        return cmp > 0 ? a : b;
      // Here the signs of both a and b can only be
      // equal (negative zeros are not supported)
      if (helper.GetSign(a) >= 0) {
        return helper.GetExponent(a).CompareTo(helper.GetExponent(b)) > 0 ? a : b;
      } else {
        return helper.GetExponent(a).CompareTo(helper.GetExponent(b)) > 0 ? b : a;
      }
    }

    /// <summary>Multiplies two T objects.</summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='decfrac'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The product of the two objects.</returns>
    /// <remarks/>
    public T Multiply(T thisValue, T decfrac, PrecisionContext ctx) {
      BigInteger bigintOp2 = helper.GetExponent(decfrac);
      BigInteger newexp = (helper.GetExponent(thisValue) + (BigInteger)bigintOp2);
      T ret = helper.CreateNew(helper.GetMantissa(thisValue) * 
                               (BigInteger)(helper.GetMantissa(decfrac)), newexp);
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
    /// <returns></returns>
    /// <remarks/>
    public T MultiplyAndAdd(T thisValue, T multiplicand,
                            T augend,
                            PrecisionContext ctx) {
      BigInteger bigintOp2 = helper.GetExponent(multiplicand);
      BigInteger newexp = (helper.GetExponent(thisValue) +
                           (BigInteger)bigintOp2);
      bigintOp2 = helper.GetMantissa(multiplicand);
      bigintOp2*=(BigInteger)(helper.GetMantissa(thisValue));
      T addend = helper.CreateNew(bigintOp2,newexp);
      return Add(addend, augend, ctx);
    }



    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='context'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <remarks/>
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
      if (context.Precision == 0 && !context.HasExponentRange &&
          (lastDiscarded | olderDiscarded) == 0)
        return thisValue;
      FastInteger fastEMin = (context.HasExponentRange) ? new FastInteger(context.EMin) : null;
      FastInteger fastEMax = (context.HasExponentRange) ? new FastInteger(context.EMax) : null;
      if (context.Precision > 0 && context.Precision <= 18 &&
          (lastDiscarded | olderDiscarded) == 0) {
        // Check if rounding is necessary at all
        // for small precisions
        BigInteger mantabs = BigInteger.Abs(helper.GetMantissa(thisValue));
        if (mantabs.CompareTo(helper.MultiplyByRadixPower(BigInteger.One, context.Precision)) < 0) {
          if (!context.HasExponentRange)
            return thisValue;
          FastInteger fastExp = new FastInteger(helper.GetExponent(thisValue));
          FastInteger fastAdjustedExp = new FastInteger(fastExp)
            .Add(context.Precision).Subtract(1);
          FastInteger fastNormalMin = new FastInteger(fastEMin)
            .Add(context.Precision).Subtract(1);
          if (fastAdjustedExp.CompareTo(fastEMax) <= 0 &&
              fastAdjustedExp.CompareTo(fastNormalMin) >= 0) {
            return thisValue;
          }
        }
      }
      int[] signals = new int[1];
      T dfrac = RoundToPrecisionInternal(
        thisValue,
        context.Precision,
        context.Rounding, fastEMin, fastEMax,
        lastDiscarded,
        olderDiscarded,
        signals);
      if (context.ClampNormalExponents && dfrac != null) {
        // Clamp exponents to eMax + 1 - precision
        // if directed
        FastInteger clamp = new FastInteger(fastEMax).Add(1).Subtract(context.Precision);
        FastInteger fastExp = new FastInteger(helper.GetExponent(dfrac));
        if (fastExp.CompareTo(clamp) > 0) {
          BigInteger bigmantissa = helper.GetMantissa(dfrac);
          int sign = bigmantissa.Sign;
          if (sign != 0) {
            if (sign < 0) bigmantissa = -bigmantissa;
            FastInteger expdiff = new FastInteger(fastExp).Subtract(clamp);
            bigmantissa = helper.MultiplyByRadixPower(bigmantissa, expdiff);
            if (sign < 0) bigmantissa = -bigmantissa;
          }
          if (signals != null)
            signals[0] |= PrecisionContext.FlagClamped;
          dfrac = helper.CreateNew(bigmantissa, clamp.AsBigInteger());
        }
      }
      if (context.HasFlags) {
        context.Flags |= signals[0];
      }
      return dfrac;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='otherValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <remarks/>
    public T Quantize(
      T thisValue,
      T otherValue,
      PrecisionContext ctx
     ) {
      PrecisionContext tmpctx = (ctx == null ?
                                 new PrecisionContext(Rounding.HalfEven) :
                                 new PrecisionContext(ctx)).WithBlankFlags();
      BigInteger mantThis = BigInteger.Abs(helper.GetMantissa(thisValue));
      BigInteger expThis = helper.GetExponent(thisValue);
      BigInteger expOther = helper.GetExponent(otherValue);
      int expcmp = expThis.CompareTo(expOther);
      int signThis = helper.GetSign(thisValue);
      T ret = default(T);
      if (expcmp == 0) {
        ret = RoundToPrecision(thisValue, tmpctx);
      } else if (mantThis.IsZero) {
        ret = helper.CreateNew(BigInteger.Zero, expOther);
        ret = RoundToPrecision(ret, tmpctx);
      } else if (expcmp > 0) {
        // Other exponent is less
        FastInteger radixPower = new FastInteger(expThis).Subtract(expOther);
        if (tmpctx.Precision > 0 &&
            radixPower.CompareTo(new FastInteger(tmpctx.Precision).Add(10)) > 0) {
          // Radix power is much too high for the current precision
          throw new ArithmeticException();
        }
        mantThis = helper.MultiplyByRadixPower(mantThis, radixPower);
        if (signThis < 0)
          mantThis = -mantThis;
        ret = helper.CreateNew(mantThis, expOther);
        ret = RoundToPrecision(ret, tmpctx);
      } else {
        // Other exponent is greater
        IShiftAccumulator accum = helper.CreateShiftAccumulator(mantThis);
        accum.ShiftRight(new FastInteger(expOther).Subtract(expThis));
        mantThis = accum.ShiftedInt;
        if (signThis < 0)
          mantThis = -mantThis;
        ret = helper.CreateNew(mantThis, expOther);
        ret = RoundToPrecision(ret, tmpctx, accum.LastDiscardedDigit,
                               accum.OlderDiscardedDigits);
      }
      if (ret == null || !helper.GetExponent(ret).Equals(expOther)) {
        throw new ArithmeticException();
      }
      if ((tmpctx.Flags & PrecisionContext.FlagOverflow) != 0) {
        throw new ArithmeticException();
      }
      if (signThis < 0 && helper.GetSign(ret) > 0) {
        BigInteger mantRet = helper.GetMantissa(ret);
        mantRet=-(BigInteger)mantRet;
        ret = helper.CreateNew(mantRet,helper.GetExponent(ret));
      }
      if (ctx != null && ctx.HasFlags) {
        int flags = tmpctx.Flags;
        flags &= ~PrecisionContext.FlagUnderflow;
        ctx.Flags |= flags;
      }
      return ret;
    }

    private T QuantizeClose(
      T thisValue,
      T otherValue,
      PrecisionContext ctx
     ){
      BigInteger expThis = helper.GetExponent(thisValue);
      BigInteger expOther = helper.GetExponent(otherValue);
      int expcmp = expThis.CompareTo(expOther);
      T ret;
      PrecisionContext ctx2=PrecisionContext.Unlimited.WithBlankFlags();
      if(expcmp<0){
        // Other exponent is greater, so
        // result must be rounded
        int signThis = helper.GetSign(thisValue);
        BigInteger mantThis = BigInteger.Abs(helper.GetMantissa(thisValue));
        IShiftAccumulator accum = helper.CreateShiftAccumulator(mantThis);
        accum.ShiftRight(new FastInteger(expOther).Subtract(expThis));
        if((accum.LastDiscardedDigit|accum.OlderDiscardedDigits)!=0){
          // would be inexact
          ret=thisValue;
          return RoundToPrecision(ret,ctx);
        }
        mantThis = accum.ShiftedInt;
        if (signThis < 0)
          mantThis = -mantThis;
        ret = helper.CreateNew(mantThis, expOther);
        return RoundToPrecision(ret, ctx, accum.LastDiscardedDigit,
                                accum.OlderDiscardedDigits);
      } else {
        // No need to round during Quantize
        ret = Quantize(thisValue,otherValue,ctx2);
        return RoundToPrecision(ret,ctx);
      }
    }
    
    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <remarks/>
    public T RoundToIntegralExact(
      T thisValue,
      PrecisionContext ctx) {
      if (helper.GetExponent(thisValue).Sign >= 0) {
        return RoundToPrecision(thisValue, ctx);
      } else {
        PrecisionContext pctx = (ctx == null) ? null :
          ctx.WithPrecision(0).WithBlankFlags();
        T ret = Quantize(thisValue, helper.CreateNew(
          BigInteger.One, BigInteger.Zero),
                         pctx);
        if (ctx != null && ctx.HasFlags) {
          ctx.Flags |= pctx.Flags;
        }
        return ret;
      }
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <remarks/>
    public T RoundToIntegralValue(
      T thisValue,
      PrecisionContext ctx
     ){
      PrecisionContext pctx = (ctx == null) ? null :
        ctx.WithBlankFlags();
      T ret = RoundToIntegralExact(thisValue, pctx);
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |=(pctx.Flags&~(PrecisionContext.FlagInexact|PrecisionContext.FlagRounded));
      }
      return ret;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <remarks/>
    public T Reduce(
      T thisValue,
      PrecisionContext ctx
     ){
      T ret = RoundToPrecision(thisValue, ctx);
      if(ret!=null){
        BigInteger bigmant=BigInteger.Abs(helper.GetMantissa(ret));
        FastInteger exp=new FastInteger(helper.GetExponent(ret));
        if(bigmant.IsZero){
          exp=new FastInteger(0);
        } else {
          long radix=helper.GetRadix();
          BigInteger bigradix=(BigInteger)radix;
          while(!(bigmant.IsZero)){
            BigInteger bigrem;
            BigInteger bigquo=BigInteger.DivRem(bigmant,bigradix,out bigrem);
            if(!bigrem.IsZero)
              break;
            bigmant=bigquo;
            exp.Add(1);
          }
        }
        bool sign=(helper.GetSign(ret)<0);
        ret=helper.CreateNew(bigmant,exp.AsBigInteger());
        if(ctx!=null && ctx.ClampNormalExponents){
          ret=RoundToPrecision(ret,ctx);
        }
        ret=EnsureSign(ret,sign);
      }
      return ret;
    }
    
    
    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <remarks/>
    public T NaturalLog(T thisValue, PrecisionContext ctx){
      if((thisValue)==null)throw new ArgumentNullException("thisValue");
      if((ctx)==null)throw new ArgumentNullException("ctx");
      if((ctx.Precision)<=0)throw new ArgumentException("ctx.Precision"+" not less than "+"0"+" ("+Convert.ToString((long)(long)(ctx.Precision),System.Globalization.CultureInfo.InvariantCulture)+")");
      PrecisionContext ctx2=HigherPrecision(ctx);
      PrecisionContext ctxFinal=(ctx==null ? new PrecisionContext(Rounding.HalfEven) :
                                 ctx.WithNoFlags().WithRounding(Rounding.HalfEven));
      T one=helper.CreateNew(BigInteger.One,BigInteger.Zero);
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
          count.AsBigInteger(),BigInteger.Zero),ctx2);
        zseries=Add(zseries,element,null);
        newguess=Add(zseries,zseries,null);
        newguess=helper.Abs(newguess);
        T newRoundedGuess=RoundToPrecision(newguess,ctxFinal);
        if(CompareTo(newRoundedGuess,roundedGuess)==0){
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
    
    private static BigInteger FloorOfHalf(BigInteger bi){
      if(bi.Sign>=0){
        return (bi>>1);
      } else {
        return (bi-BigInteger.One)/(BigInteger)2;
      }
    }
    
    private static PrecisionContext HigherPrecision(PrecisionContext ctx){
      FastInteger fi=new FastInteger(ctx.Precision).Add(6);
      long prec=fi.CanFitInInt64() ? fi.AsInt64() : Int64.MaxValue;
      prec=Math.Max(16,prec);
      return ctx.WithPrecision(prec);
    }
    
    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <remarks/>
    public T SquareRoot(T thisValue, PrecisionContext ctx){
      if((thisValue)==null)throw new ArgumentNullException("thisValue");
      if((ctx)==null)throw new ArgumentNullException("ctx");
      if((ctx.Precision)<=0)throw new ArgumentException("ctx.Precision"+" not less than "+"0"+" ("+Convert.ToString((long)(long)(ctx.Precision),System.Globalization.CultureInfo.InvariantCulture)+")");
      
      int sign=helper.GetSign(thisValue);
      if(sign<0)
        throw new ArithmeticException();
      PrecisionContext ctx2=(ctx.WithNoFlags().WithRounding(Rounding.HalfEven));
      PrecisionContext ctx2flags=HigherPrecision(ctx2).WithUnlimitedExponents();
      T idealExp=helper.CreateNew(
        BigInteger.One,
        FloorOfHalf(helper.GetExponent(thisValue)));
      if(sign==0){
        return QuantizeClose(thisValue,idealExp,ctx2);
      }
      T one=helper.CreateNew(BigInteger.One,BigInteger.Zero);
      T a=one;
      T b=one;
      int correct=0;
      T result=default(T);
      T oldresult=default(T);
      while(true){
        T newB=default(T);
        T newA=default(T);
        for(int i=0;i<5;i++){
          newB=Add(a,b,null);
          T tmp=helper.Abs(Multiply(b,thisValue,null));
          newA=Add(a,tmp,null);
          a=newA;
          b=newB;
        }
        result=Divide(a,b,ctx2flags);
        if(!result.Equals(default(T)) &&
           CompareTo(result,oldresult)==0){
          correct++;
          if(correct>=2){
            if(ctx.HasFlags){
              ctx.Flags|=ctx2flags.Flags;
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
      if(ctx.HasFlags){
        ctx.Flags|=ctx2flags.Flags;
      }
      return result;
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
      if ((precision) < 0) throw new ArgumentException("precision" + " not greater or equal to " + "0" + " (" + Convert.ToString((long)(long)(precision),System.Globalization.CultureInfo.InvariantCulture) + ")");
      bool neg = helper.GetMantissa(thisValue).Sign < 0;
      BigInteger bigmantissa = helper.GetMantissa(thisValue);
      if (neg) bigmantissa = -bigmantissa;
      // save mantissa in case result is subnormal
      // and must be rounded again
      BigInteger oldmantissa = bigmantissa;
      FastInteger exp = new FastInteger(helper.GetExponent(thisValue));
      int flags = 0;
      IShiftAccumulator accum = helper.CreateShiftAccumulator(
        bigmantissa, lastDiscarded, olderDiscarded);
      bool unlimitedPrec = (precision == 0);
      if (precision > 0) {
        accum.ShiftToDigits(precision);
      } else {
        precision = accum.DigitLength;
      }
      FastInteger discardedBits = new FastInteger(accum.DiscardedDigitCount);
      exp.Add(discardedBits);
      FastInteger adjExponent = new FastInteger(exp)
        .Add(accum.DigitLength).Subtract(1);
      FastInteger clamp = null;
      if (fastEMax != null && adjExponent.CompareTo(fastEMax) > 0) {
        if (oldmantissa.IsZero) {
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
          BigInteger overflowMant = helper.MultiplyByRadixPower(BigInteger.One, precision);
          overflowMant -= BigInteger.One;
          if (neg) overflowMant = -overflowMant;
          if (signals != null) signals[0] = flags;
          clamp = new FastInteger(fastEMax).Add(1).Subtract(precision);
          return helper.CreateNew(overflowMant, clamp.AsBigInteger());
        }
        if (signals != null) signals[0] = flags;
        return default(T);
      } else if (fastEMin != null && adjExponent.CompareTo(fastEMin) < 0) {
        // Subnormal
        FastInteger fastETiny = new FastInteger(fastEMin)
          .Subtract(precision)
          .Add(1);
        if (!oldmantissa.IsZero)
          flags |= PrecisionContext.FlagSubnormal;
        if (exp.CompareTo(fastETiny) < 0) {
          FastInteger expdiff = new FastInteger(fastETiny).Subtract(exp);
          expdiff.Add(discardedBits);
          accum = helper.CreateShiftAccumulator(oldmantissa, lastDiscarded, olderDiscarded);
          accum.ShiftRight(expdiff);
          BigInteger newmantissa = accum.ShiftedInt;
          if ((accum.DiscardedDigitCount).Sign != 0 ||
              (accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
            if (!oldmantissa.IsZero)
              flags |= PrecisionContext.FlagRounded;
            if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
              flags |= PrecisionContext.FlagInexact;
              if (rounding == Rounding.Unnecessary)
                throw new ArithmeticException("Rounding was required");
            }
            if (Round(accum, rounding, neg, newmantissa)) {
              newmantissa += BigInteger.One;
            }
          }
          if (newmantissa.IsZero)
            flags |= PrecisionContext.FlagClamped;
          if ((flags & (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact)) == (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact))
            flags |= PrecisionContext.FlagUnderflow | PrecisionContext.FlagRounded;
          if (signals != null) signals[0] = flags;
          if (neg) newmantissa = -newmantissa;
          return helper.CreateNew(newmantissa, fastETiny.AsBigInteger());
        }
      }
      bool expChanged = false;
      if ((accum.DiscardedDigitCount).Sign != 0 ||
          (accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
        if (!bigmantissa.IsZero)
          flags |= PrecisionContext.FlagRounded;
        bigmantissa = accum.ShiftedInt;
        if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
          flags |= PrecisionContext.FlagInexact;
          if (rounding == Rounding.Unnecessary)
            throw new ArithmeticException("Rounding was required");
        }
        if (Round(accum, rounding, neg, bigmantissa)) {
          bigmantissa += BigInteger.One;
          if (bigmantissa.IsEven) {
            accum = helper.CreateShiftAccumulator(bigmantissa);
            accum.ShiftToDigits(precision);
            if ((accum.DiscardedDigitCount).Sign != 0) {
              exp.Add(accum.DiscardedDigitCount);
              discardedBits.Add(accum.DiscardedDigitCount);
              bigmantissa = accum.ShiftedInt;
              expChanged = true;
            }
          }
        }
      }
      if (expChanged && fastEMax != null) {
        // If exponent changed, check for overflow again
        adjExponent = new FastInteger(exp);
        adjExponent.Add(accum.DigitLength).Subtract(1);
        if (adjExponent.CompareTo(fastEMax) > 0) {
          flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
          if (!unlimitedPrec &&
              (rounding == Rounding.Down ||
               rounding == Rounding.ZeroFiveUp ||
               (rounding == Rounding.Ceiling && neg) ||
               (rounding == Rounding.Floor && !neg))) {
            // Set to the highest possible value for
            // the given precision
            BigInteger overflowMant = helper.MultiplyByRadixPower(BigInteger.One, precision);
            overflowMant -= BigInteger.One;
            if (neg) overflowMant = -overflowMant;
            if (signals != null) signals[0] = flags;
            clamp = new FastInteger(fastEMax).Add(1).Subtract(precision);
            return helper.CreateNew(overflowMant, clamp.AsBigInteger());
          }
          if (signals != null) signals[0] = flags;
          return default(T);
        }
      }
      if (signals != null) signals[0] = flags;
      if (neg) bigmantissa = -bigmantissa;
      return helper.CreateNew(bigmantissa, exp.AsBigInteger());
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='decfrac'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <remarks/>
    public T Add(T thisValue, T decfrac, PrecisionContext ctx) {
      int expcmp = helper.GetExponent(thisValue).CompareTo((BigInteger)helper.GetExponent(decfrac));
      T retval = default(T);
      if (expcmp == 0) {
        retval = helper.CreateNew(
          helper.GetMantissa(thisValue) + (BigInteger)(helper.GetMantissa(decfrac)), helper.GetExponent(thisValue));
      } else {
        // choose the minimum exponent
        BigInteger resultExponent = (expcmp < 0 ? helper.GetExponent(thisValue) : helper.GetExponent(decfrac));
        T op1 = thisValue;
        T op2 = decfrac;
        BigInteger op1Exponent = helper.GetExponent(op1);
        BigInteger op2Exponent = helper.GetExponent(op2);
        BigInteger expdiff = BigInteger.Abs(op1Exponent - (BigInteger)op2Exponent);
        if (ctx != null && ctx.Precision > 0) {
          // Check if exponent difference is too big for
          // radix-power calculation to work quickly
          if (expdiff.CompareTo((BigInteger)100) >= 0) {
            FastInteger fastint = new FastInteger(expdiff).Add(3);
            // If exponent difference plus 3 is greater than the precision
            if (fastint.CompareTo(ctx.Precision) > 0) {
              int expcmp2 = op1Exponent.CompareTo(op2Exponent);
              if (expcmp2 < 0 && !(helper.GetMantissa(op2).IsZero)) {
                // first operand's exponent is less
                // and second operand isn't zero
                // the 8 digits at the end are guard digits
                op1Exponent = (new FastInteger(op2Exponent).Subtract(ctx.Precision).Subtract(8)
                               .AsBigInteger());
              } else if (expcmp2 > 0 && !(helper.GetMantissa(op1).IsZero)) {
                // first operand's exponent is greater
                // and first operand isn't zero
                // the 8 digits at the end are guard digits
                op2Exponent = (new FastInteger(op1Exponent).Subtract(ctx.Precision).Subtract(8)
                               .AsBigInteger());
              }
              expcmp = op1Exponent.CompareTo((BigInteger)op2Exponent);
              resultExponent = (expcmp < 0 ? op1Exponent : op2Exponent);
            }
          }
        }
        if (expcmp > 0) {
          BigInteger newmant = helper.RescaleByExponentDiff(
            helper.GetMantissa(op1), op1Exponent, op2Exponent);
          retval = helper.CreateNew(
            newmant + (BigInteger)(helper.GetMantissa(op2)), resultExponent);
        } else {
          BigInteger newmant = helper.RescaleByExponentDiff(
            helper.GetMantissa(op2), op1Exponent, op2Exponent);
          retval = helper.CreateNew(
            newmant + (BigInteger)(helper.GetMantissa(op1)), resultExponent);
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
    /// <returns>Zero if the values are equal; a negative number is this instance
    /// is less, or a positive number if this instance is greater.</returns>
    /// <remarks/>
    public int CompareTo(T thisValue, T decfrac) {
      if (decfrac == null) return 1;
      int s = helper.GetSign(thisValue);
      int ds = helper.GetSign(decfrac);
      if (s != ds) return (s < ds) ? -1 : 1;
      int expcmp = helper.GetExponent(thisValue).CompareTo((BigInteger)helper.GetExponent(decfrac));
      int mantcmp = helper.GetMantissa(thisValue).CompareTo((BigInteger)helper.GetMantissa(decfrac));
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
        BigInteger expdiff = BigInteger.Abs(op1Exponent - (BigInteger)op2Exponent);
        // Check if exponent difference is too big for
        // radix-power calculation to work quickly
        if (expdiff.CompareTo((BigInteger)100) >= 0) {
          FastInteger fastint = new FastInteger(expdiff).Add(3);
          long precision1 = helper.CreateShiftAccumulator(
            BigInteger.Abs(helper.GetMantissa(thisValue))).DigitLength;
          long precision2 = helper.CreateShiftAccumulator(
            BigInteger.Abs(helper.GetMantissa(decfrac))).DigitLength;
          long maxPrecision = Math.Max(precision1, precision2);
          // If exponent difference plus 3 is greater than the
          // maximum precision of the two operands
          if (fastint.CompareTo(maxPrecision) > 0) {
            int expcmp2 = op1Exponent.CompareTo(op2Exponent);
            if (expcmp2 < 0) {
              // first operand's exponent is less
              // (second operand won't be zero at thisValue point)
              // the 8 digits at the end are guard digits
              op1Exponent = (new FastInteger(op2Exponent).Subtract(maxPrecision).Subtract(8)
                             .AsBigInteger());
            } else if (expcmp2 > 0) {
              // first operand's exponent is greater
              // (first operand won't be zero at thisValue point)
              // the 8 digits at the end are guard digits
              op2Exponent = (new FastInteger(op1Exponent).Subtract(maxPrecision).Subtract(8)
                             .AsBigInteger());
            }
            expcmp = op1Exponent.CompareTo((BigInteger)op2Exponent);
          }
        }
        if (expcmp > 0) {
          BigInteger newmant = helper.RescaleByExponentDiff(
            helper.GetMantissa(thisValue), op1Exponent, op2Exponent);
          return newmant.CompareTo((BigInteger)helper.GetMantissa(decfrac));
        } else {
          BigInteger newmant = helper.RescaleByExponentDiff(
            helper.GetMantissa(decfrac), op1Exponent, op2Exponent);
          return helper.GetMantissa(thisValue).CompareTo((BigInteger)newmant);
        }
      }
    }


  }


}