using System;
using System.Text;
//using System.Numerics;

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
                       bool neg, FastInteger fastint) {
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

    private bool RoundGivenBigInt(IShiftAccumulator accum, Rounding rounding,
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
        return Abs(val);
      }
      return val;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
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
                               ctx==null ? BigInteger.Zero : ctx.Precision),
                             IntegerModeFixedScale, BigInteger.Zero,
                             null);
      bool neg = (helper.GetSign(thisValue) < 0) ^ (helper.GetSign(divisor) < 0);
      // Now the exponent's sign can only be 0 or positive
      if (helper.GetSign(ret) == 0) {
        // Value is 0, so just change the exponent
        // to the preferred one
        BigInteger divisorExp=helper.GetExponent(divisor);
        ret = helper.CreateNew(BigInteger.Zero, helper.GetExponent(thisValue) -
                               (BigInteger)divisorExp);
      } else {
        if (desiredScale.Sign < 0) {
          // Desired scale is negative, shift left
          desiredScale.Negate();
          BigInteger bigmantissa = BigInteger.Abs(helper.GetMantissa(ret));
          bigmantissa = helper.MultiplyByRadixPower(bigmantissa, desiredScale);
          if (helper.GetMantissa(ret).Sign < 0) bigmantissa = -bigmantissa;
          ret = helper.CreateNew(bigmantissa, helper.GetExponent(thisValue) -
                                 (BigInteger)(helper.GetExponent(divisor)));
        } else if (desiredScale.Sign > 0) {
          // Desired scale is positive, shift away zeros
          // but not after scale is reached
          BigInteger bigmantissa = BigInteger.Abs(helper.GetMantissa(ret));
          FastInteger fastexponent = FastInteger.FromBig(helper.GetExponent(ret));
          BigInteger bigradix = (BigInteger)(helper.GetRadix());
          while(true){
            if(desiredScale.CompareTo(fastexponent)==0)
              break;
            BigInteger bigrem;
            BigInteger bigquo=BigInteger.DivRem(bigmantissa,bigradix,out bigrem);
            if(!bigrem.IsZero)
              break;
            bigmantissa=bigquo;
            fastexponent.AddInt(1);
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
    public T DivideToIntegerZeroScale(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      T ret = DivideInternal(thisValue, divisor,
                             PrecisionContext.ForRounding(Rounding.Down).WithBigPrecision(
                               ctx == null ? BigInteger.Zero : ctx.Precision),
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
    
    private T Abs(T value){
      return (helper.GetSign(value)<0) ? Negate(value) : value;
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

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='divisor'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    public T RemainderNear(
      T thisValue,
      T divisor,
      PrecisionContext ctx
     ) {
      T ret = DivideInternal(thisValue, divisor,
                             PrecisionContext.ForRounding(Rounding.HalfEven)
                             .WithBigPrecision(ctx == null ? BigInteger.Zero : ctx.Precision),
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
        ret = RoundToPrecision(ret, ctx);
      }
      return ret;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    public T NextMinus(
      T thisValue,
      PrecisionContext ctx
     ){
      if((ctx)==null)throw new ArgumentNullException("ctx");
      if((ctx.Precision).Sign<=0)throw new ArgumentException("ctx.Precision"+" not less than "+"0"+" ("+Convert.ToString((ctx.Precision),System.Globalization.CultureInfo.InvariantCulture)+")");
      if(!(ctx.HasExponentRange))throw new ArgumentException("doesn't satisfy ctx.HasExponentRange");
      BigInteger minusone=-(BigInteger)BigInteger.One;
      FastInteger minexp=FastInteger.FromBig(ctx.EMin).SubtractBig(ctx.Precision).AddInt(1);
      FastInteger bigexp=FastInteger.FromBig(helper.GetExponent(thisValue));
      if(bigexp.CompareTo(minexp)<0){
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

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='otherValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    public T NextToward(
      T thisValue,
      T otherValue,
      PrecisionContext ctx
     ){
      if((ctx)==null)throw new ArgumentNullException("ctx");
      if((ctx.Precision).Sign<=0)throw new ArgumentException("ctx.Precision"+" not less than "+"0"+" ("+Convert.ToString((ctx.Precision),System.Globalization.CultureInfo.InvariantCulture)+")");
      if(!(ctx.HasExponentRange))throw new ArgumentException("doesn't satisfy ctx.HasExponentRange");
      PrecisionContext ctx2;
      int cmp=CompareTo(thisValue,otherValue);
      if(cmp==0){
        return RoundToPrecision(thisValue,ctx.WithNoFlags());
      } else {
        FastInteger minexp=FastInteger.FromBig(ctx.EMin).SubtractBig(ctx.Precision).AddInt(1);
        FastInteger bigexp=FastInteger.FromBig(helper.GetExponent(thisValue));
        if(bigexp.CompareTo(minexp)<0){
          // Use a smaller exponent if the input exponent is already
          // very small
          minexp=FastInteger.Copy(bigexp).SubtractInt(2);
        } else {
          // Ensure the exponent is lower than the exponent range
          // (necessary to flag underflow correctly)
          minexp.SubtractInt(2);
        }
        BigInteger bigdir=BigInteger.One;
        if(cmp>0){
          bigdir=-(BigInteger)bigdir;
        }
        T quantum=helper.CreateNew(bigdir,minexp.AsBigInteger());
        T val=thisValue;
        ctx2=ctx.WithRounding((cmp>0) ? Rounding.Floor : Rounding.Ceiling).WithBlankFlags();
        val=Add(val,quantum,ctx2);
        if((ctx2.Flags&(PrecisionContext.FlagOverflow|PrecisionContext.FlagUnderflow))==0){
          // Don't set flags except on overflow or underflow
          // TODO: Pending clarification from Mike Cowlishaw,
          // author of the Decimal Arithmetic test cases from
          // speleotrove.com
          ctx2.Flags=0;
        }
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
    public T NextPlus(
      T thisValue,
      PrecisionContext ctx
     ){
      if((ctx)==null)throw new ArgumentNullException("ctx");
      if((ctx.Precision).Sign<=0)throw new ArgumentException("ctx.Precision"+" not less than "+"0"+" ("+Convert.ToString((ctx.Precision),System.Globalization.CultureInfo.InvariantCulture)+")");
      if(!(ctx.HasExponentRange))throw new ArgumentException("doesn't satisfy ctx.HasExponentRange");
      BigInteger minusone=BigInteger.One;
      FastInteger minexp=FastInteger.FromBig(ctx.EMin).SubtractBig(ctx.Precision).AddInt(1);
      FastInteger bigexp=FastInteger.FromBig(helper.GetExponent(thisValue));
      if(bigexp.CompareTo(minexp)<0){
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
      if(ctx!=null && !ctx.ExponentWithinRange(desiredExponent))
        throw new ArithmeticException("Exponent not within exponent range: "+desiredExponent.ToString());
      PrecisionContext ctx2 = (ctx == null) ?
        PrecisionContext.ForRounding(Rounding.HalfDown) :
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
      FastInteger shift,// Number of digits to shift right
      bool neg,// Whether return value should be negated
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
      accum = helper.CreateShiftAccumulatorWithDigits(
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
          flags |= PrecisionContext.FlagInexact|PrecisionContext.FlagRounded;
          if (rounding == Rounding.Unnecessary)
            throw new ArithmeticException("Rounding was required");
        }
        if (RoundGivenBigInt(accum, rounding, neg, newmantissa)) {
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
    private const int IntegerModeRegular = 0;

    private const int NonTerminatingCheckThreshold = 5;
    
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
      int radix=helper.GetRadix();
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
        FastInteger expDividend = FastInteger.FromBig(helper.GetExponent(thisValue));
        FastInteger expDivisor = FastInteger.FromBig(helper.GetExponent(divisor));
        FastInteger expdiff = FastInteger.Copy(expDividend).Subtract(expDivisor);
        FastInteger adjust = new FastInteger(0);
        FastInteger result = new FastInteger(0);
        FastInteger naturalExponent = FastInteger.Copy(expdiff);
        FastInteger fastDesiredExponent = FastInteger.FromBig(desiredExponent);
        bool negA = (signA < 0);
        bool negB = (signB < 0);
        if (negA) mantissaDividend = -mantissaDividend;
        if (negB) mantissaDivisor = -mantissaDivisor;
        FastInteger fastPrecision=(ctx==null) ? new FastInteger(0) :
          FastInteger.FromBig(ctx.Precision);
        if(integerMode==IntegerModeFixedScale){
          FastInteger shift;
          BigInteger rem;
          if(ctx!=null && ctx.HasFlags && FastInteger.FromBig(desiredExponent)
             .CompareTo(naturalExponent)>0){
            // Treat as rounded if the desired exponent is greater
            // than the "ideal" exponent
            ctx.Flags|=PrecisionContext.FlagRounded;
          }
          if(expdiff.CompareTo(fastDesiredExponent)<=0){
            shift=FastInteger.Copy(fastDesiredExponent).Subtract(expdiff);
            BigInteger quo=BigInteger.DivRem(mantissaDividend,mantissaDivisor,out rem);
            quo=RoundToScale(quo,rem,mantissaDivisor,shift,negA^negB,ctx);
            return helper.CreateNew(quo,desiredExponent);
          } else if (ctx != null && (ctx.Precision).Sign!=0 &&
                     FastInteger.Copy(expdiff).SubtractInt(8).CompareTo(fastPrecision) > 0
                    ) { // NOTE: 8 guard digits
            // Result would require a too-high precision since
            // exponent difference is much higher
            throw new ArithmeticException("Result can't fit the precision");
          } else {
            shift=FastInteger.Copy(expdiff).Subtract(fastDesiredExponent);
            mantissaDividend=helper.MultiplyByRadixPower(mantissaDividend,shift);
            BigInteger quo=BigInteger.DivRem(mantissaDividend,mantissaDivisor,out rem);
            quo=RoundToScale(quo,rem,mantissaDivisor,new FastInteger(0),negA^negB,ctx);
            return helper.CreateNew(quo,desiredExponent);
          }
        }
        FastInteger resultPrecision = new FastInteger(1);
        int mantcmp = mantissaDividend.CompareTo(mantissaDivisor);
        if (mantcmp < 0) {
          // dividend mantissa is less than divisor mantissa
          FastInteger dividendPrecision =
            helper.CreateShiftAccumulator(mantissaDividend).GetDigitLength();
          FastInteger divisorPrecision =
            helper.CreateShiftAccumulator(mantissaDivisor).GetDigitLength();
          divisorPrecision.Subtract(dividendPrecision);
          if(divisorPrecision.Sign==0)
            divisorPrecision.AddInt(1);
          // multiply dividend mantissa so precisions are the same
          // (except if they're already the same, in which case multiply
          // by radix)
          mantissaDividend = helper.MultiplyByRadixPower(
            mantissaDividend, divisorPrecision);
          adjust.Add(divisorPrecision);
          if (mantissaDividend.CompareTo(mantissaDivisor) < 0) {
            // dividend mantissa is still less, multiply once more
            if(radix==2){
              mantissaDividend<<=1;
            } else {
              mantissaDividend*=(BigInteger)radix;
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
          if (mantissaDividend.CompareTo(mantissaDivisor) < 0) {
            // dividend mantissa is now less, divide by radix power
            if (dividendPrecision.CompareToInt(1)==0) {
              // no need to divide here, since that would just undo
              // the multiplication
              mantissaDivisor = oldMantissaB;
            } else {
              BigInteger bigpow = (BigInteger)(radix);
              mantissaDivisor /= bigpow;
            }
            adjust.AddInt(1);
          }
        }
        if (mantcmp == 0) {
          result = new FastInteger(1);
          mantissaDividend = BigInteger.Zero;
        } else {
          int check = 0;
          FastInteger divs=FastInteger.FromBig(mantissaDivisor);
          FastInteger divd=FastInteger.FromBig(mantissaDividend);
          FastInteger divsHalfRadix=null;
          if(radix!=2){
            divsHalfRadix=FastInteger.FromBig(mantissaDivisor).Multiply(radix/2);
          }
          bool hasPrecision=ctx != null && (ctx.Precision).Sign!=0;
          while (true) {
            bool remainderZero=false;
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
            if(divsHalfRadix!=null && divd.CompareTo(divsHalfRadix)>=0){
              divd.Subtract(divsHalfRadix);
              count+=radix/2;
            }
            while(divd.CompareTo(divs)>=0){
              divd.Subtract(divs);
              count++;
            }
            result.AddInt(count);
            remainderZero=(divd.Sign==0);
            if (hasPrecision && resultPrecision.CompareTo(fastPrecision) == 0) {
              mantissaDividend=divd.AsBigInteger();
              break;
            }
            if (remainderZero && adjust.Sign >= 0) {
              mantissaDividend=divd.AsBigInteger();
              break;
            }
            adjust.AddInt(1);
            if (result.Sign != 0) {
              resultPrecision.AddInt(1);
            }
            result.Multiply(radix);
            divd.Multiply(radix);
          }
        }
        // mantissaDividend now has the remainder
        FastInteger exp = FastInteger.Copy(expdiff).Subtract(adjust);
        int lastDiscarded = 0;
        int olderDiscarded = 0;
        if (!(mantissaDividend.IsZero)) {
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
        }
        BigInteger bigResult = result.AsBigInteger();
        if (negA ^ negB) {
          bigResult = -bigResult;
        }
        if(ctx!=null && ctx.HasFlags && exp.CompareTo(naturalExponent)>0){
          // Treat as rounded if the true exponent is greater
          // than the "ideal" exponent
          ctx.Flags|=PrecisionContext.FlagRounded;
        }
        return RoundToPrecisionWithDigits(
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
      int cmp = CompareTo(Abs(a), Abs(b));
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
      int cmp = CompareTo(Abs(a), Abs(b));
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
    public T RoundToBinaryPrecision(
      T thisValue,
      PrecisionContext context
     ) {
      return RoundToBinaryPrecisionWithDigits(thisValue, context, 0, 0);
    }
    private T RoundToBinaryPrecisionWithDigits(
      T thisValue,
      PrecisionContext context,
      int lastDiscarded,
      int olderDiscarded
     ) {
      if ((context) == null) return thisValue;
      if ((context.Precision).IsZero && !context.HasExponentRange &&
          (lastDiscarded | olderDiscarded) == 0)
        return thisValue;
      if((context.Precision).IsZero || helper.GetRadix()==2)
        return RoundToPrecisionWithDigits(thisValue,context,lastDiscarded,olderDiscarded);
      FastInteger fastEMin = (context.HasExponentRange) ? FastInteger.FromBig(context.EMin) : null;
      FastInteger fastEMax = (context.HasExponentRange) ? FastInteger.FromBig(context.EMax) : null;
      FastInteger fastPrecision = FastInteger.FromBig(context.Precision);
      int[] signals = new int[1];
      T dfrac = RoundToBinaryPrecisionInternal(
        thisValue,fastPrecision,
        context.Rounding, fastEMin, fastEMax,
        lastDiscarded,
        olderDiscarded,
        signals);
      // Clamp exponents to eMax + 1 - precision
      // if directed
      if (context.ClampNormalExponents && dfrac != null) {
        FastInteger digitCount=null;
        if(helper.GetRadix()==2){
          digitCount=FastInteger.Copy(fastPrecision);
        } else {
          // TODO: Use a faster way to get the digit
          // count for radix 10
          BigInteger maxMantissa = BigInteger.One;
          FastInteger prec=FastInteger.Copy(fastPrecision);
          while(prec.Sign>0){
            int shift=prec.CompareToInt(1000000)>=0 ? 1000000 : prec.AsInt32();
            maxMantissa<<=shift;
            prec.SubtractInt(shift);
          }
          maxMantissa-=BigInteger.One;
          // Get the digit length of the maximum possible mantissa
          // for the given binary precision
          digitCount=helper.CreateShiftAccumulator(maxMantissa)
            .GetDigitLength();
        }
        FastInteger clamp = FastInteger.Copy(fastEMax).AddInt(1).Subtract(digitCount);
        FastInteger fastExp = FastInteger.FromBig(helper.GetExponent(dfrac));
        if (fastExp.CompareTo(clamp) > 0) {
          BigInteger bigmantissa = helper.GetMantissa(dfrac);
          int sign = bigmantissa.Sign;
          if (sign != 0) {
            if (sign < 0) bigmantissa = -bigmantissa;
            FastInteger expdiff = FastInteger.Copy(fastExp).Subtract(clamp);
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
    /// <param name='context'>A PrecisionContext object.</param>
    /// <returns></returns>
    public T RoundToPrecision(
      T thisValue,
      PrecisionContext context
     ) {
      return RoundToPrecisionWithDigits(thisValue, context, 0, 0);
    }
    private T RoundToPrecisionWithDigits(
      T thisValue,
      PrecisionContext context,
      int lastDiscarded,
      int olderDiscarded
     ) {
      return RoundToPrecisionWithShift(thisValue,context,
                                        lastDiscarded,olderDiscarded,new FastInteger(0));
    }
    private T RoundToPrecisionWithShift(
      T thisValue,
      PrecisionContext context,
      int lastDiscarded,
      int olderDiscarded,
      FastInteger shift
     ) {
      if ((context) == null) return thisValue;
      if ((context.Precision).IsZero && !context.HasExponentRange &&
          (lastDiscarded | olderDiscarded) == 0 && shift.Sign==0)
        return thisValue;
      FastInteger fastEMin = (context.HasExponentRange) ? FastInteger.FromBig(context.EMin) : null;
      FastInteger fastEMax = (context.HasExponentRange) ? FastInteger.FromBig(context.EMax) : null;
      FastInteger fastPrecision=FastInteger.FromBig(context.Precision);
      if (fastPrecision.Sign > 0 && fastPrecision.CompareToInt(18) <= 0 &&
          (lastDiscarded | olderDiscarded) == 0 && shift.Sign==0) {
        // Check if rounding is necessary at all
        // for small precisions
        BigInteger mantabs = BigInteger.Abs(helper.GetMantissa(thisValue));
        if (mantabs.CompareTo(helper.MultiplyByRadixPower(BigInteger.One, fastPrecision)) < 0) {
          if (!context.HasExponentRange)
            return thisValue;
          FastInteger fastExp = FastInteger.FromBig(helper.GetExponent(thisValue));
          FastInteger fastAdjustedExp = FastInteger.Copy(fastExp)
            .Add(fastPrecision).SubtractInt(1);
          FastInteger fastNormalMin = FastInteger.Copy(fastEMin)
            .Add(fastPrecision).SubtractInt(1);
          if (fastAdjustedExp.CompareTo(fastEMax) <= 0 &&
              fastAdjustedExp.CompareTo(fastNormalMin) >= 0) {
            return thisValue;
          }
        }
      }
      int[] signals = new int[1];
      T dfrac = RoundToPrecisionInternal(
        thisValue,fastPrecision,
        context.Rounding, fastEMin, fastEMax,
        lastDiscarded,
        olderDiscarded,
        shift,
        context.HasFlags ? signals : null);
      if (context.ClampNormalExponents && dfrac != null) {
        // Clamp exponents to eMax + 1 - precision
        // if directed
        FastInteger clamp = FastInteger.Copy(fastEMax).AddInt(1).Subtract(fastPrecision);
        FastInteger fastExp = FastInteger.FromBig(helper.GetExponent(dfrac));
        if (fastExp.CompareTo(clamp) > 0) {
          BigInteger bigmantissa = helper.GetMantissa(dfrac);
          int sign = bigmantissa.Sign;
          if (sign != 0) {
            if (sign < 0) bigmantissa = -bigmantissa;
            FastInteger expdiff = FastInteger.Copy(fastExp).Subtract(clamp);
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
    public T Quantize(
      T thisValue,
      T otherValue,
      PrecisionContext ctx
     ) {
      BigInteger expOther = helper.GetExponent(otherValue);
      if(ctx!=null && !ctx.ExponentWithinRange(expOther))
        throw new ArithmeticException("Exponent not within exponent range: "+expOther.ToString());
      PrecisionContext tmpctx = (ctx == null ?
                                 PrecisionContext.ForRounding(Rounding.HalfEven) :
                                 ctx.Copy()).WithBlankFlags();
      BigInteger mantThis = BigInteger.Abs(helper.GetMantissa(thisValue));
      BigInteger expThis = helper.GetExponent(thisValue);
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
        FastInteger radixPower = FastInteger.FromBig(expThis).SubtractBig(expOther);
        if ((tmpctx.Precision).Sign>0 &&
            radixPower.CompareTo(FastInteger.FromBig(tmpctx.Precision).AddInt(10)) > 0) {
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
        FastInteger shift=FastInteger.FromBig(expOther).SubtractBig(expThis);
        if (signThis < 0)
          mantThis = -mantThis;
        ret = helper.CreateNew(mantThis, expOther);
        ret = RoundToPrecisionWithShift(ret, tmpctx, 0, 0, shift);
      }
      if ((tmpctx.Flags & PrecisionContext.FlagOverflow) != 0) {
        throw new OverflowException();
      }
      if (ret == null || !helper.GetExponent(ret).Equals(expOther)) {
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
    
    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='expOther'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    public T RoundToExponentExact(
      T thisValue,
      BigInteger expOther,
      PrecisionContext ctx) {
      if (helper.GetExponent(thisValue).CompareTo(expOther) >= 0) {
        return RoundToPrecision(thisValue, ctx);
      } else {
        PrecisionContext pctx = (ctx == null) ? null :
          ctx.WithPrecision(0).WithBlankFlags();
        T ret = Quantize(thisValue, helper.CreateNew(
          BigInteger.One, expOther),
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
    /// <returns></returns>
    public T RoundToExponentSimple(
      T thisValue,
      BigInteger expOther,
      PrecisionContext ctx) {
      if (helper.GetExponent(thisValue).CompareTo(expOther) >= 0) {
        return RoundToPrecision(thisValue, ctx);
      } else {
        if(ctx!=null && !ctx.ExponentWithinRange(expOther))
          throw new ArithmeticException("Exponent not within exponent range: "+expOther.ToString());
        BigInteger bigmantissa=helper.GetMantissa(thisValue);
        bool neg=bigmantissa.Sign<0;
        if(neg)bigmantissa=-bigmantissa;
        FastInteger shift=FastInteger.FromBig(expOther).SubtractBig(helper.GetExponent(thisValue));
        IShiftAccumulator accum=helper.CreateShiftAccumulator(bigmantissa);
        accum.ShiftRight(shift);
        bigmantissa=accum.ShiftedInt;
        if(neg)bigmantissa=-bigmantissa;
        return RoundToPrecisionWithDigits(
          helper.CreateNew(bigmantissa,expOther),ctx,
          accum.LastDiscardedDigit,
          accum.OlderDiscardedDigits);
      }
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <param name='exponent'>A BigInteger object.</param>
    public T RoundToExponentNoRoundedFlag(
      T thisValue,
      BigInteger exponent,
      PrecisionContext ctx
     ){
      PrecisionContext pctx = (ctx == null) ? null :
        ctx.WithBlankFlags();
      T ret = RoundToExponentExact(thisValue, exponent, pctx);
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |=(pctx.Flags&~(PrecisionContext.FlagInexact|PrecisionContext.FlagRounded));
      }
      return ret;
    }

    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    public T Reduce(
      T thisValue,
      PrecisionContext ctx
     ){
      T ret = RoundToPrecision(thisValue, ctx);
      if(ret!=null){
        BigInteger bigmant=BigInteger.Abs(helper.GetMantissa(ret));
        FastInteger exp=FastInteger.FromBig(helper.GetExponent(ret));
        if(bigmant.IsZero){
          exp=new FastInteger(0);
        } else {
          int radix=helper.GetRadix();
          BigInteger bigradix=(BigInteger)radix;
          while(!(bigmant.IsZero)){
            BigInteger bigrem;
            BigInteger bigquo=BigInteger.DivRem(bigmant,bigradix,out bigrem);
            if(!bigrem.IsZero)
              break;
            bigmant=bigquo;
            exp.AddInt(1);
          }
        }
        bool sign=(helper.GetSign(ret)<0);
        ret=helper.CreateNew(bigmant,exp.AsBigInteger());
        if(ctx!=null && ctx.ClampNormalExponents){
          PrecisionContext ctxtmp=ctx.WithBlankFlags();
          ret=RoundToPrecision(ret,ctxtmp);
          if(ctx.HasFlags){
            ctx.Flags|=(ctx.Flags&~PrecisionContext.FlagClamped);
          }
        }
        ret=EnsureSign(ret,sign);
      }
      return ret;
    }
    
    /*
    public  T NaturalLog(T thisValue, PrecisionContext ctx){
      if((ctx)==null)throw new ArgumentNullException("ctx");
      if((ctx.Precision).Sign<=0)throw new ArgumentException("ctx.Precision"+" not less than "+"0"+" ("+Convert.ToString((ctx.Precision),System.Globalization.CultureInfo.InvariantCulture)+")");
      throw new NotImplementedException();
    }
     */
    /// <summary> </summary>
    /// <param name='thisValue'>A T object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    public T SquareRoot(T thisValue, PrecisionContext ctx){
      if((thisValue)==null)throw new ArgumentNullException("thisValue");
      if((ctx)==null)throw new ArgumentNullException("ctx");
      if((ctx.Precision).Sign<=0)throw new ArgumentException("ctx.Precision"+" not less than "+"0"+" ("+Convert.ToString((ctx.Precision),System.Globalization.CultureInfo.InvariantCulture)+")");
      
      int sign=helper.GetSign(thisValue);
      if(sign<0)
        throw new ArithmeticException();

      throw new NotImplementedException();
    }

    private T RoundToBinaryPrecisionInternal(
      T thisValue,
      FastInteger precision,
      Rounding rounding,
      FastInteger fastEMin,
      FastInteger fastEMax,
      int lastDiscarded,
      int olderDiscarded,
      int[] signals
     ) {
      // TODO: Update for changes in RoundToPrecisionInternal
      if (precision.Sign < 0) throw new ArgumentException("precision" + " not greater or equal to " + "0" + " (" + Convert.ToString((precision),System.Globalization.CultureInfo.InvariantCulture) + ")");
      if(helper.GetRadix()==2 || precision.Sign==0){
        return RoundToPrecisionInternal(thisValue,precision,rounding,
                                        fastEMin,fastEMax,lastDiscarded,
                                        olderDiscarded,null,signals);
      }
      bool neg = helper.GetMantissa(thisValue).Sign < 0;
      BigInteger bigmantissa = helper.GetMantissa(thisValue);
      if (neg) bigmantissa = -bigmantissa;
      // save mantissa in case result is subnormal
      // and must be rounded again
      BigInteger oldmantissa = bigmantissa;
      bool mantissaWasZero=(oldmantissa.IsZero && (lastDiscarded|olderDiscarded)==0);
      BigInteger maxMantissa = BigInteger.One;
      FastInteger prec=FastInteger.Copy(precision);
      while(prec.Sign>0){
        int shift=(prec.CompareToInt(1000000)>=0) ? 1000000 : prec.AsInt32();
        maxMantissa<<=shift;
        prec.SubtractInt(shift);
      }
      maxMantissa-=BigInteger.One;
      FastInteger exp = FastInteger.FromBig(helper.GetExponent(thisValue));
      int flags = 0;
      IShiftAccumulator accumMaxMant = helper.CreateShiftAccumulator(
        maxMantissa);
      // Get the digit length of the maximum possible mantissa
      // for the given binary precision
      FastInteger digitCount=accumMaxMant.GetDigitLength();
      IShiftAccumulator accum = helper.CreateShiftAccumulatorWithDigits(
        bigmantissa, lastDiscarded, olderDiscarded);
      accum.ShiftToDigits(digitCount);
      while((accum.ShiftedInt).CompareTo(maxMantissa)>0){
        accum.ShiftRightInt(1);
      }
      FastInteger discardedBits = FastInteger.Copy(accum.DiscardedDigitCount);
      exp.Add(discardedBits);
      FastInteger adjExponent = FastInteger.Copy(exp)
        .Add(accum.GetDigitLength()).SubtractInt(1);
      FastInteger clamp = null;
      if(fastEMax!=null && adjExponent.CompareTo(fastEMax)==0){
        // May or may not be an overflow depending on the mantissa
        FastInteger expdiff=FastInteger.Copy(digitCount).Subtract(accum.GetDigitLength());
        BigInteger currMantissa=accum.ShiftedInt;
        currMantissa=helper.MultiplyByRadixPower(currMantissa,expdiff);
        if((currMantissa).CompareTo(maxMantissa)>0){
          // Mantissa too high, treat as overflow
          adjExponent.AddInt(1);
        }
      }
      if (fastEMax != null && adjExponent.CompareTo(fastEMax) > 0) {
        if (mantissaWasZero) {
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
          if (neg) overflowMant = -overflowMant;
          if (signals != null) signals[0] = flags;
          clamp = FastInteger.Copy(fastEMax).AddInt(1).Subtract(digitCount);
          return helper.CreateNew(overflowMant, clamp.AsBigInteger());
        }
        if (signals != null) signals[0] = flags;
        return default(T);
      } else if (fastEMin != null && adjExponent.CompareTo(fastEMin) < 0) {
        // Subnormal
        FastInteger fastETiny = FastInteger.Copy(fastEMin)
          .Subtract(digitCount)
          .AddInt(1);
        if (!mantissaWasZero)
          flags |= PrecisionContext.FlagSubnormal;
        if (exp.CompareTo(fastETiny) < 0) {
          FastInteger expdiff = FastInteger.Copy(fastETiny).Subtract(exp);
          expdiff.Add(discardedBits);
          accum = helper.CreateShiftAccumulatorWithDigits(oldmantissa, lastDiscarded, olderDiscarded);
          accum.ShiftRight(expdiff);
          BigInteger newmantissa = accum.ShiftedInt;
          if ((accum.DiscardedDigitCount).Sign != 0 ||
              (accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
            if (!mantissaWasZero)
              flags |= PrecisionContext.FlagRounded;
            if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
              flags |= PrecisionContext.FlagInexact|PrecisionContext.FlagRounded;
              if (rounding == Rounding.Unnecessary)
                throw new ArithmeticException("Rounding was required");
            }
            if (RoundGivenBigInt(accum, rounding, neg, newmantissa)) {
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
      bool mantChanged=false;
      if ((accum.DiscardedDigitCount).Sign != 0 ||
          (accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
        if (!bigmantissa.IsZero)
          flags |= PrecisionContext.FlagRounded;
        bigmantissa = accum.ShiftedInt;
        if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
          flags |= PrecisionContext.FlagInexact|PrecisionContext.FlagRounded;
          if (rounding == Rounding.Unnecessary)
            throw new ArithmeticException("Rounding was required");
        }
        if (RoundGivenBigInt(accum, rounding, neg, bigmantissa)) {
          bigmantissa += BigInteger.One;
          mantChanged = true;
          if (bigmantissa.IsEven) {
            accum = helper.CreateShiftAccumulator(bigmantissa);
            accum.ShiftToDigits(digitCount);
            while((accum.ShiftedInt).CompareTo(maxMantissa)>0){
              accum.ShiftRightInt(1);
            }
            if ((accum.DiscardedDigitCount).Sign != 0) {
              exp.Add(accum.DiscardedDigitCount);
              discardedBits.Add(accum.DiscardedDigitCount);
              bigmantissa = accum.ShiftedInt;
            }
          }
        }
      }
      if (mantChanged && fastEMax != null) {
        // If mantissa changed, check for overflow again
        adjExponent = FastInteger.Copy(exp);
        adjExponent.Add(accum.GetDigitLength()).SubtractInt(1);
        if(fastEMax!=null && adjExponent.CompareTo(fastEMax)==0 && mantChanged){
          // May or may not be an overflow depending on the mantissa
          FastInteger expdiff=FastInteger.Copy(digitCount).Subtract(accum.GetDigitLength());
          BigInteger currMantissa=accum.ShiftedInt;
          currMantissa=helper.MultiplyByRadixPower(currMantissa,expdiff);
          if((currMantissa).CompareTo(maxMantissa)>0){
            // Mantissa too high, treat as overflow
            adjExponent.AddInt(1);
          }
        }
        if (adjExponent.CompareTo(fastEMax) > 0) {
          flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
          if ((rounding == Rounding.Down ||
               rounding == Rounding.ZeroFiveUp ||
               (rounding == Rounding.Ceiling && neg) ||
               (rounding == Rounding.Floor && !neg))) {
            // Set to the highest possible value for
            // the given precision
            BigInteger overflowMant = maxMantissa;
            if (neg) overflowMant = -overflowMant;
            if (signals != null) signals[0] = flags;
            clamp = FastInteger.Copy(fastEMax).AddInt(1).Subtract(digitCount);
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
    
    private T RoundToPrecisionInternal(
      T thisValue,
      FastInteger precision,
      Rounding rounding,
      FastInteger fastEMin,
      FastInteger fastEMax,
      int lastDiscarded,
      int olderDiscarded,
      FastInteger shift,
      int[] signals
     ) {
      if (precision.Sign < 0) throw new ArgumentException("precision" + " not greater or equal to " + "0" + " (" + precision + ")");
      BigInteger bigmantissa = helper.GetMantissa(thisValue);
      bool neg = bigmantissa.Sign < 0;
      if (neg) bigmantissa = -bigmantissa;
      // save mantissa in case result is subnormal
      // and must be rounded again
      BigInteger oldmantissa = bigmantissa;
      bool mantissaWasZero=(oldmantissa.IsZero && (lastDiscarded|olderDiscarded)==0);
      FastInteger exp = FastInteger.FromBig(helper.GetExponent(thisValue));
      int flags = 0;
      IShiftAccumulator accum = helper.CreateShiftAccumulatorWithDigits(
        bigmantissa, lastDiscarded, olderDiscarded);
      bool unlimitedPrec = (precision.Sign==0);
      if(shift!=null){
        accum.ShiftRight(shift);
        exp.Subtract(accum.DiscardedDigitCount);
      }
      if (precision.Sign > 0) {
        accum.ShiftToDigits(precision);
      } else {
        precision = accum.GetDigitLength();
      }
      FastInteger discardedBits = FastInteger.Copy(accum.DiscardedDigitCount);
      FastInteger fastPrecision=precision;
      exp.Add(discardedBits);
      FastInteger adjExponent = FastInteger.Copy(exp)
        .Add(accum.GetDigitLength())
        .SubtractInt(1);
      FastInteger newAdjExponent = adjExponent;
      FastInteger clamp = null;
      /*
      Console.WriteLine("exp={0} {1} adjexp={2} was={3},{4},{5} now={6} digits={7} digitlen={8},{9}",
                        fastEMin,fastEMax,adjExponent,oldmantissa,
                        lastDiscarded,olderDiscarded,accum.ShiftedInt,
                        accum.DiscardedDigitCount,accum.GetDigitLength(),exp);
       */
      BigInteger earlyRounded=null;
      if(signals!=null && fastEMin != null && adjExponent.CompareTo(fastEMin) < 0){
        earlyRounded=accum.ShiftedInt;
        if(RoundGivenBigInt(accum, rounding, neg, earlyRounded)){
          earlyRounded += BigInteger.One;
          if (earlyRounded.IsEven) {
            IShiftAccumulator accum2 = helper.CreateShiftAccumulator(earlyRounded);
            accum2.ShiftToDigits(fastPrecision);
            if ((accum2.DiscardedDigitCount).Sign != 0) {
              earlyRounded = accum2.ShiftedInt;
            }
            newAdjExponent=FastInteger.Copy(exp)
              .Add(accum2.GetDigitLength())
              .SubtractInt(1);
          }
        }
      }
      if (fastEMax != null && adjExponent.CompareTo(fastEMax) > 0) {
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
          BigInteger overflowMant = helper.MultiplyByRadixPower(BigInteger.One, fastPrecision);
          overflowMant -= BigInteger.One;
          if (neg) overflowMant = -overflowMant;
          if (signals != null) signals[0] = flags;
          clamp = FastInteger.Copy(fastEMax).AddInt(1).Subtract(fastPrecision);
          return helper.CreateNew(overflowMant, clamp.AsBigInteger());
        }
        if (signals != null) signals[0] = flags;
        return default(T);
      } else if (fastEMin != null && adjExponent.CompareTo(fastEMin) < 0) {
        // Subnormal
        FastInteger fastETiny = FastInteger.Copy(fastEMin)
          .Subtract(fastPrecision)
          .AddInt(1);
        if (signals!=null){
          if(!earlyRounded.IsZero){
            if(newAdjExponent.CompareTo(fastEMin)<0){
              flags |= PrecisionContext.FlagSubnormal;
            }
          }
        }
        if (exp.CompareTo(fastETiny) < 0) {
          FastInteger expdiff = FastInteger.Copy(fastETiny).Subtract(exp);
          expdiff.Add(discardedBits);
          accum = helper.CreateShiftAccumulatorWithDigits(oldmantissa, lastDiscarded, olderDiscarded);
          accum.ShiftRight(expdiff);
          FastInteger newmantissa = accum.ShiftedIntFast;
          if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
            if (rounding == Rounding.Unnecessary)
              throw new ArithmeticException("Rounding was required");
          }
          if ((accum.DiscardedDigitCount).Sign != 0 ||
              (accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
            if(signals!=null){
              if (!mantissaWasZero)
                flags |= PrecisionContext.FlagRounded;
              if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
                flags |= PrecisionContext.FlagInexact|PrecisionContext.FlagRounded;
              }
            }
            if (Round(accum, rounding, neg, newmantissa)) {
              newmantissa.AddInt(1);
            }
          }
          if (signals != null){
            if (newmantissa.Sign==0)
              flags |= PrecisionContext.FlagClamped;
            if ((flags & (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact)) == (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact))
              flags |= PrecisionContext.FlagUnderflow | PrecisionContext.FlagRounded;
            signals[0] = flags;
          }
          if (neg) newmantissa.Negate();
          return helper.CreateNew(newmantissa.AsBigInteger(), fastETiny.AsBigInteger());
        }
      }
      bool expChanged = false;
      if ((accum.DiscardedDigitCount).Sign != 0 ||
          (accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
        if (!bigmantissa.IsZero)
          flags |= PrecisionContext.FlagRounded;
        bigmantissa = accum.ShiftedInt;
        if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
          flags |= PrecisionContext.FlagInexact|PrecisionContext.FlagRounded;
          if (rounding == Rounding.Unnecessary)
            throw new ArithmeticException("Rounding was required");
        }
        if (RoundGivenBigInt(accum, rounding, neg, bigmantissa)) {
          bigmantissa += BigInteger.One;
          if (bigmantissa.IsEven) {
            accum = helper.CreateShiftAccumulator(bigmantissa);
            accum.ShiftToDigits(fastPrecision);
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
        adjExponent = FastInteger.Copy(exp);
        adjExponent.Add(accum.GetDigitLength()).SubtractInt(1);
        if (adjExponent.CompareTo(fastEMax) > 0) {
          flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
          if (!unlimitedPrec &&
              (rounding == Rounding.Down ||
               rounding == Rounding.ZeroFiveUp ||
               (rounding == Rounding.Ceiling && neg) ||
               (rounding == Rounding.Floor && !neg))) {
            // Set to the highest possible value for
            // the given precision
            BigInteger overflowMant = helper.MultiplyByRadixPower(BigInteger.One, fastPrecision);
            overflowMant -= BigInteger.One;
            if (neg) overflowMant = -overflowMant;
            if (signals != null) signals[0] = flags;
            clamp = FastInteger.Copy(fastEMax).AddInt(1).Subtract(fastPrecision);
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
    public T Add(T thisValue, T decfrac, PrecisionContext ctx) {
      int expcmp = helper.GetExponent(thisValue).CompareTo((BigInteger)helper.GetExponent(decfrac));
      T retval = default(T);
      if (expcmp == 0) {
        retval = helper.CreateNew(
          helper.GetMantissa(thisValue) + (BigInteger)(helper.GetMantissa(decfrac)), helper.GetExponent(thisValue));
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
        if (ctx != null && (ctx.Precision).Sign > 0) {
          // Check if exponent difference is too big for
          // radix-power calculation to work quickly
          FastInteger fastPrecision=FastInteger.FromBig(ctx.Precision);
          // If exponent difference is greater than the precision
          if (FastInteger.Copy(expdiff).CompareTo(fastPrecision) > 0) {
            BigInteger op1MantAbs=BigInteger.Abs(helper.GetMantissa(op1));
            BigInteger op2MantAbs=BigInteger.Abs(helper.GetMantissa(op2));
            int expcmp2 = fastOp1Exp.CompareTo(fastOp2Exp);
            if (expcmp2 < 0) {
              if(!(op2MantAbs.IsZero)){
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
                  .CompareTo(fastOp2Exp) < 0) {
                  // first operand's mantissa can't reach the
                  // second operand's mantissa, so the exponent can be
                  // raised without affecting the result
                  FastInteger tmp=FastInteger.Copy(fastOp2Exp).SubtractInt(8)
                    .Subtract(digitLength1)
                    .SubtractBig(ctx.Precision);
                  FastInteger newDiff=FastInteger.Copy(tmp).Subtract(fastOp2Exp).Abs();
                  if(newDiff.CompareTo(expdiff)<0){
                    // Can be treated as almost zero
                    if(helper.GetSign(thisValue)==helper.GetSign(decfrac)){
                      FastInteger digitLength2=helper.CreateShiftAccumulator(
                        op2MantAbs).GetDigitLength();
                      if(digitLength2.CompareTo(fastPrecision)<0){
                        // Second operand's precision too short
                        FastInteger precisionDiff=FastInteger.Copy(fastPrecision).Subtract(digitLength2);
                        op2MantAbs=helper.MultiplyByRadixPower(
                          op2MantAbs,precisionDiff);
                        BigInteger bigintTemp=precisionDiff.AsBigInteger();
                        op2Exponent-=(BigInteger)bigintTemp;
                        if(helper.GetSign(decfrac)<0)op2MantAbs=-op2MantAbs;
                        decfrac=helper.CreateNew(op2MantAbs,op2Exponent);
                        return RoundToPrecisionWithDigits(decfrac,ctx,0,1);
                      } else {
                        return RoundToPrecisionWithDigits(decfrac,ctx,0,1);
                      }
                    } else {
                      op1Exponent = (tmp.AsBigInteger());
                    }
                  }
                }
              }
            } else if (expcmp2 > 0) {
              if(!(op1MantAbs.IsZero)){
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
                  .CompareTo(fastOp1Exp) < 0) {
                  // second operand's mantissa can't reach the
                  // first operand's mantissa, so the exponent can be
                  // raised without affecting the result
                  FastInteger tmp=FastInteger.Copy(fastOp1Exp).SubtractInt(8)
                    .Subtract(digitLength2)
                    .SubtractBig(ctx.Precision);
                  FastInteger newDiff=FastInteger.Copy(tmp).Subtract(fastOp1Exp).Abs();
                  if(newDiff.CompareTo(expdiff)<0){
                    // Can be treated as almost zero
                    if(helper.GetSign(thisValue)==helper.GetSign(decfrac)){
                      FastInteger digitLength1=helper.CreateShiftAccumulator(op1MantAbs).GetDigitLength();
                      if(digitLength1.CompareTo(fastPrecision)<0){
                        // First operand's precision too short
                        FastInteger precisionDiff=FastInteger.Copy(fastPrecision).Subtract(digitLength1);
                        op1MantAbs=helper.MultiplyByRadixPower(
                          op1MantAbs,precisionDiff);
                        BigInteger bigintTemp=precisionDiff.AsBigInteger();
                        op1Exponent-=(BigInteger)bigintTemp;
                        if(helper.GetSign(thisValue)<0)op1MantAbs=-op1MantAbs;
                        thisValue=helper.CreateNew(op1MantAbs,op1Exponent);
                        return RoundToPrecisionWithDigits(thisValue,ctx,0,1);
                      } else {
                        return RoundToPrecisionWithDigits(thisValue,ctx,0,1);
                      }
                    } else {
                      op2Exponent = (tmp.AsBigInteger());
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
        BigInteger op1MantAbs=BigInteger.Abs(helper.GetMantissa(thisValue));
        BigInteger op2MantAbs=BigInteger.Abs(helper.GetMantissa(decfrac));
        FastInteger precision1 = helper.CreateShiftAccumulator(
          op1MantAbs).GetDigitLength();
        FastInteger precision2 = helper.CreateShiftAccumulator(
          op2MantAbs).GetDigitLength();
        FastInteger maxPrecision=null;
        if(precision1.CompareTo(precision2)>0)
          maxPrecision=precision1;
        else
          maxPrecision=precision2;
        // If exponent difference is greater than the
        // maximum precision of the two operands
        if (FastInteger.Copy(expdiff).CompareTo(maxPrecision) > 0) {
          int expcmp2 = fastOp1Exp.CompareTo(fastOp2Exp);
          if (expcmp2 < 0) {
            if(!(op2MantAbs.IsZero)){
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
                .CompareTo(fastOp2Exp) < 0) {
                // first operand's mantissa can't reach the
                // second operand's mantissa, so the exponent can be
                // raised without affecting the result
                FastInteger tmp=FastInteger.Copy(fastOp2Exp).SubtractInt(8)
                  .Subtract(digitLength1)
                  .Subtract(maxPrecision);
                FastInteger newDiff=FastInteger.Copy(tmp).Subtract(fastOp2Exp).Abs();
                if(newDiff.CompareTo(expdiff)<0){
                  if(s==ds){
                    return (s<0) ? 1 : -1;
                  } else {
                    op1Exponent = (tmp.AsBigInteger());
                  }
                }
              }
            }
          } else if (expcmp2 > 0) {
            if(!(op1MantAbs.IsZero)){
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
                .CompareTo(fastOp1Exp) < 0) {
                // second operand's mantissa can't reach the
                // first operand's mantissa, so the exponent can be
                // raised without affecting the result
                FastInteger tmp=FastInteger.Copy(fastOp1Exp).SubtractInt(8)
                  .Subtract(digitLength2)
                  .Subtract(maxPrecision);
                FastInteger newDiff=FastInteger.Copy(tmp).Subtract(fastOp1Exp).Abs();
                if(newDiff.CompareTo(expdiff)<0){
                  if(s==ds){
                    return (s<0) ? -1 : 1;
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
        return newmant.CompareTo((BigInteger)helper.GetMantissa(decfrac));
      } else {
        BigInteger newmant = helper.RescaleByExponentDiff(
          helper.GetMantissa(decfrac), op1Exponent, op2Exponent);
        return helper.GetMantissa(thisValue).CompareTo((BigInteger)newmant);
      }
    }
  }

}