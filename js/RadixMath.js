
var RadixMath = function(helper) {

    this.helper = helper;
};
(function(constructor,prototype){
    prototype.helper = null;
    
    prototype.Min = function(a, b) {
        if (a == null) throw ("a");
        if (b == null) throw ("b");
        var cmp = this.compareTo(a, b);
        if (cmp != 0) return cmp > 0 ? b : a;
        
        if (this.helper.GetSign(a) >= 0) {
            return (this.helper.GetExponent(a)).compareTo(this.helper.GetExponent(b)) > 0 ? b : a;
        } else {
            return (this.helper.GetExponent(a)).compareTo(this.helper.GetExponent(b)) > 0 ? a : b;
        }
    };
    prototype.Round = function(accum, rounding, neg, fastint) {
        var incremented = false;
        var radix = this.helper.GetRadix();
        if (rounding == Rounding.HalfUp) {
            if (accum.getLastDiscardedDigit() >= ((radix / 2)|0)) {
                incremented = true;
            }
        } else if (rounding == Rounding.HalfEven) {
            if (accum.getLastDiscardedDigit() >= ((radix / 2)|0)) {
                if (accum.getLastDiscardedDigit() > ((radix / 2)|0) || accum.getOlderDiscardedDigits() != 0) {
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
            if (accum.getLastDiscardedDigit() > ((radix / 2)|0) || (accum.getLastDiscardedDigit() == ((radix / 2)|0) && accum.getOlderDiscardedDigits() != 0)) {
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
                    var lastDigit = FastInteger.Copy(fastint).Mod(radix).AsInt32();
                    if (lastDigit == 0 || lastDigit == ((radix / 2)|0)) {
                        incremented = true;
                    }
                }
            }
        }
        return incremented;
    };
    prototype.RoundGivenBigInt = function(accum, rounding, neg, bigval) {
        var incremented = false;
        var radix = this.helper.GetRadix();
        if (rounding == Rounding.HalfUp) {
            if (accum.getLastDiscardedDigit() >= ((radix / 2)|0)) {
                incremented = true;
            }
        } else if (rounding == Rounding.HalfEven) {
            if (accum.getLastDiscardedDigit() >= ((radix / 2)|0)) {
                if (accum.getLastDiscardedDigit() > ((radix / 2)|0) || accum.getOlderDiscardedDigits() != 0) {
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
            if (accum.getLastDiscardedDigit() > ((radix / 2)|0) || (accum.getLastDiscardedDigit() == ((radix / 2)|0) && accum.getOlderDiscardedDigits() != 0)) {
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
                    var bigdigit = bigval.remainder(BigInteger.valueOf(radix));
                    var lastDigit = bigdigit.intValue();
                    if (lastDigit == 0 || lastDigit == ((radix / 2)|0)) {
                        incremented = true;
                    }
                }
            }
        }
        return incremented;
    };
    prototype.EnsureSign = function(val, negative) {
        if (val == null) return val;
        var sign = this.helper.GetSign(val);
        if (negative && sign > 0) {
            var bigmant = this.helper.GetMantissa(val);
            bigmant = (bigmant).negate();
            var e = this.helper.GetExponent(val);
            return this.helper.CreateNew(bigmant, e);
        } else if (!negative && sign < 0) {
            return this.Abs(val);
        }
        return val;
    };
    
    prototype.DivideToIntegerNaturalScale = function(thisValue, divisor, ctx) {
        var desiredScale = FastInteger.FromBig(this.helper.GetExponent(thisValue)).SubtractBig(this.helper.GetExponent(divisor));
        var ret = this.DivideInternal(thisValue, divisor, PrecisionContext.ForRounding(Rounding.Down).WithBigPrecision(ctx == null ? BigInteger.ZERO : ctx.getPrecision()), RadixMath.IntegerModeFixedScale, BigInteger.ZERO, null);
        var neg = (this.helper.GetSign(thisValue) < 0) ^ (this.helper.GetSign(divisor) < 0);
        
        if (this.helper.GetSign(ret) == 0) {
            
            var divisorExp = this.helper.GetExponent(divisor);
            ret = this.helper.CreateNew(BigInteger.ZERO, this.helper.GetExponent(thisValue).subtract(divisorExp));
        } else {
            if (desiredScale.signum() < 0) {
                
                desiredScale.Negate();
                var bigmantissa = (this.helper.GetMantissa(ret)).abs();
                bigmantissa = this.helper.MultiplyByRadixPower(bigmantissa, desiredScale);
                if (this.helper.GetMantissa(ret).signum() < 0) bigmantissa = bigmantissa.negate();
                ret = this.helper.CreateNew(bigmantissa, this.helper.GetExponent(thisValue).subtract(this.helper.GetExponent(divisor)));
            } else if (desiredScale.signum() > 0) {
                
                var bigmantissa = (this.helper.GetMantissa(ret)).abs();
                var fastexponent = FastInteger.FromBig(this.helper.GetExponent(ret));
                var bigradix = BigInteger.valueOf(this.helper.GetRadix());
                while (true) {
                    if (desiredScale.compareTo(fastexponent) == 0) break;
                    var bigrem;
                    var bigquo;
                    var divrem = (bigmantissa).divideAndRemainder(bigradix);
                    bigquo = divrem[0];
                    bigrem = divrem[1];
                    if (bigrem.signum() != 0) break;
                    bigmantissa = bigquo;
                    fastexponent.AddInt(1);
                }
                if (this.helper.GetMantissa(ret).signum() < 0) bigmantissa = bigmantissa.negate();
                ret = this.helper.CreateNew(bigmantissa, fastexponent.AsBigInteger());
            }
        }
        if (ctx != null) {
            ret = this.RoundToPrecision(ret, ctx);
        }
        ret = this.EnsureSign(ret, neg);
        return ret;
    };
    
    prototype.DivideToIntegerZeroScale = function(thisValue, divisor, ctx) {
        var ret = this.DivideInternal(thisValue, divisor, PrecisionContext.ForRounding(Rounding.Down).WithBigPrecision(ctx == null ? BigInteger.ZERO : ctx.getPrecision()), RadixMath.IntegerModeFixedScale, BigInteger.ZERO, null);
        if (ctx != null) {
            var ctx2 = ctx.WithBlankFlags().WithUnlimitedExponents();
            ret = this.RoundToPrecision(ret, ctx2);
            if ((ctx2.getFlags() & PrecisionContext.FlagRounded) != 0) {
                throw ("Result would require a higher precision");
            }
        }
        return ret;
    };
    prototype.Abs = function(value) {
        return (this.helper.GetSign(value) < 0) ? this.Negate(value) : value;
    };
    prototype.Negate = function(value) {
        var mant = this.helper.GetMantissa(value);
        mant = (mant).negate();
        return this.helper.CreateNew(mant, this.helper.GetExponent(value));
    };
    
    prototype.Remainder = function(thisValue, divisor, ctx) {
        var ret = this.DivideToIntegerZeroScale(thisValue, divisor, ctx);
        ret = this.Add(thisValue, this.Negate(this.Multiply(ret, divisor, null)), null);
        if (ctx != null) {
            var ctx2 = ctx.WithBlankFlags();
            ret = this.RoundToPrecision(ret, ctx2);
        }
        return ret;
    };
    
    prototype.RemainderNear = function(thisValue, divisor, ctx) {
        var ret = this.DivideInternal(thisValue, divisor, PrecisionContext.ForRounding(Rounding.HalfEven).WithBigPrecision(ctx == null ? BigInteger.ZERO : ctx.getPrecision()), RadixMath.IntegerModeFixedScale, BigInteger.ZERO, null);
        if (ctx != null) {
            var ctx2 = ctx.WithBlankFlags().WithUnlimitedExponents();
            ret = this.RoundToPrecision(ret, ctx2);
            if ((ctx2.getFlags() & PrecisionContext.FlagRounded) != 0) {
                throw ("Result would require a higher precision");
            }
        }
        ret = this.Add(thisValue, this.Negate(this.Multiply(ret, divisor, null)), null);
        if (ctx != null) {
            var ctx2 = ctx.WithBlankFlags();
            ret = this.RoundToPrecision(ret, ctx2);
        }
        return ret;
    };
    
    prototype.NextMinus = function(thisValue, ctx) {
        if ((ctx) == null) throw ("ctx");
        if ((ctx.getPrecision()).signum() <= 0) throw ("ctx.getPrecision()" + " not less than " + "0" + " (" + (ctx.getPrecision()) + ")");
        if (!(ctx.getHasExponentRange())) throw ("doesn't satisfy ctx.getHasExponentRange()");
        var minusone = (BigInteger.ONE).negate();
        var minexp = FastInteger.FromBig(ctx.getEMin()).SubtractBig(ctx.getPrecision()).AddInt(1);
        var bigexp = FastInteger.FromBig(this.helper.GetExponent(thisValue));
        if (bigexp.compareTo(minexp) < 0) {
            
            minexp = FastInteger.Copy(bigexp).SubtractInt(2);
        }
        var quantum = this.helper.CreateNew(minusone, minexp.AsBigInteger());
        var ctx2;
        var val = thisValue;
        ctx2 = ctx.WithRounding(Rounding.Floor);
        return this.Add(val, quantum, ctx2);
    };
    
    prototype.NextToward = function(thisValue, otherValue, ctx) {
        if ((ctx) == null) throw ("ctx");
        if ((ctx.getPrecision()).signum() <= 0) throw ("ctx.getPrecision()" + " not less than " + "0" + " (" + (ctx.getPrecision()) + ")");
        if (!(ctx.getHasExponentRange())) throw ("doesn't satisfy ctx.getHasExponentRange()");
        var ctx2;
        var cmp = this.compareTo(thisValue, otherValue);
        if (cmp == 0) {
            return this.RoundToPrecision(thisValue, ctx.WithNoFlags());
        } else {
            var minexp = FastInteger.FromBig(ctx.getEMin()).SubtractBig(ctx.getPrecision()).AddInt(1);
            var bigexp = FastInteger.FromBig(this.helper.GetExponent(thisValue));
            if (bigexp.compareTo(minexp) < 0) {
                
                minexp = FastInteger.Copy(bigexp).SubtractInt(2);
            }
            var bigdir = BigInteger.ONE;
            if (cmp > 0) {
                bigdir = (bigdir).negate();
            }
            var quantum = this.helper.CreateNew(bigdir, minexp.AsBigInteger());
            var val = thisValue;
            ctx2 = ctx.WithRounding((cmp > 0) ? Rounding.Floor : Rounding.Ceiling).WithBlankFlags();
            val = this.Add(val, quantum, ctx2);
            if (ctx.getHasFlags()) {
                ctx.setFlags(ctx.getFlags() | ctx2.getFlags());
            }
            return val;
        }
    };
    
    prototype.NextPlus = function(thisValue, ctx) {
        if ((ctx) == null) throw ("ctx");
        if ((ctx.getPrecision()).signum() <= 0) throw ("ctx.getPrecision()" + " not less than " + "0" + " (" + (ctx.getPrecision()) + ")");
        if (!(ctx.getHasExponentRange())) throw ("doesn't satisfy ctx.getHasExponentRange()");
        var minusone = BigInteger.ONE;
        var minexp = FastInteger.FromBig(ctx.getEMin()).SubtractBig(ctx.getPrecision()).AddInt(1);
        var bigexp = FastInteger.FromBig(this.helper.GetExponent(thisValue));
        if (bigexp.compareTo(minexp) < 0) {
            
            minexp = FastInteger.Copy(bigexp).SubtractInt(2);
        }
        var quantum = this.helper.CreateNew(minusone, minexp.AsBigInteger());
        var ctx2;
        var val = thisValue;
        ctx2 = ctx.WithRounding(Rounding.Ceiling);
        return this.Add(val, quantum, ctx2);
    };
    
    prototype.DivideToExponent = function(thisValue, divisor, desiredExponent, ctx) {
        if (ctx != null && !ctx.ExponentWithinRange(desiredExponent)) throw ("Exponent not within exponent range: " + desiredExponent.toString());
        var ctx2 = (ctx == null) ? PrecisionContext.ForRounding(Rounding.HalfDown) : ctx.WithUnlimitedExponents().WithPrecision(0);
        var ret = this.DivideInternal(thisValue, divisor, ctx2, RadixMath.IntegerModeFixedScale, desiredExponent, null);
        if (ctx != null && ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | ctx2.getFlags());
        }
        return ret;
    };
    
    prototype.Divide = function(thisValue, divisor, ctx) {
        return this.DivideInternal(thisValue, divisor, ctx, RadixMath.IntegerModeRegular, BigInteger.ZERO, null);
    };
    prototype.RoundToScale = function(mantissa, remainder, divisor, shift, neg, ctx) {
        
        
        
        
        
        var accum;
        var lastDiscarded = 0;
        var olderDiscarded = 0;
        if (!(remainder.signum() == 0)) {
            var halfDivisor = (divisor.shiftRight(1));
            var cmpHalf = remainder.compareTo(halfDivisor);
            if ((cmpHalf == 0) && divisor.testBit(0) == false) {
                
                lastDiscarded = ((this.helper.GetRadix() / 2)|0);
                olderDiscarded = 0;
            } else if (cmpHalf > 0) {
                
                lastDiscarded = ((this.helper.GetRadix() / 2)|0);
                olderDiscarded = 1;
            } else {
                
                lastDiscarded = 0;
                olderDiscarded = 1;
            }
        }
        accum = this.helper.CreateShiftAccumulatorWithDigits(mantissa, lastDiscarded, olderDiscarded);
        accum.ShiftRight(shift);
        var flags = 0;
        var rounding = (ctx == null) ? Rounding.HalfEven : ctx.getRounding();
        var newmantissa = accum.getShiftedInt();
        if ((accum.getDiscardedDigitCount()).signum() != 0 || (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
            if (mantissa.signum() != 0) flags |= PrecisionContext.FlagRounded;
            if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                flags |= PrecisionContext.FlagInexact;
                if (rounding == Rounding.Unnecessary) throw ("Rounding was required");
            }
            if (this.RoundGivenBigInt(accum, rounding, neg, newmantissa)) {
                newmantissa = newmantissa.add(BigInteger.ONE);
            }
        }
        if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | flags);
        }
        if (neg) {
            newmantissa = newmantissa.negate();
        }
        return newmantissa;
    };
    constructor.IntegerModeFixedScale = 1;
    constructor.IntegerModeRegular = 0;
    constructor.NonTerminatingCheckThreshold = 5;
    prototype.DivideInternal = function(thisValue, divisor, ctx, integerMode, desiredExponent, remainder) {
        var signA = this.helper.GetSign(thisValue);
        var signB = this.helper.GetSign(divisor);
        if (signB == 0) {
            throw "exception";
        }
        var radix = this.helper.GetRadix();
        if (signA == 0) {
            var retval = null;
            if (integerMode == RadixMath.IntegerModeFixedScale) {
                retval = this.helper.CreateNew(BigInteger.ZERO, desiredExponent);
            } else {
                var divExp = this.helper.GetExponent(divisor);
                retval = this.RoundToPrecision(this.helper.CreateNew(BigInteger.ZERO, (this.helper.GetExponent(thisValue).subtract(divExp))), ctx);
            }
            if (remainder != null) {
                remainder[0] = retval;
            }
            return retval;
        } else {
            var mantissaDividend = this.helper.GetMantissa(thisValue);
            var mantissaDivisor = this.helper.GetMantissa(divisor);
            var expDividend = FastInteger.FromBig(this.helper.GetExponent(thisValue));
            var expDivisor = FastInteger.FromBig(this.helper.GetExponent(divisor));
            var expdiff = FastInteger.Copy(expDividend).Subtract(expDivisor);
            var adjust = new FastInteger(0);
            var result = new FastInteger(0);
            var fastDesiredExponent = FastInteger.FromBig(desiredExponent);
            var negA = (signA < 0);
            var negB = (signB < 0);
            if (negA) mantissaDividend = mantissaDividend.negate();
            if (negB) mantissaDivisor = mantissaDivisor.negate();
            var fastPrecision = (ctx == null) ? new FastInteger(0) : FastInteger.FromBig(ctx.getPrecision());
            if (integerMode == RadixMath.IntegerModeFixedScale) {
                var shift;
                var rem;
                if (expdiff.compareTo(fastDesiredExponent) <= 0) {
                    shift = FastInteger.Copy(fastDesiredExponent).Subtract(expdiff);
                    var quo;
                    var divrem = (mantissaDividend).divideAndRemainder(mantissaDivisor);
                    quo = divrem[0];
                    rem = divrem[1];
                    quo = this.RoundToScale(quo, rem, mantissaDivisor, shift, negA ^ negB, ctx);
                    return this.helper.CreateNew(quo, desiredExponent);
                } else if (ctx != null && (ctx.getPrecision()).signum() != 0 && FastInteger.Copy(expdiff).SubtractInt(8).compareTo(fastPrecision) > 0) {
                    
                    throw ("Result can't fit the precision");
                } else {
                    shift = FastInteger.Copy(expdiff).Subtract(fastDesiredExponent);
                    mantissaDividend = this.helper.MultiplyByRadixPower(mantissaDividend, shift);
                    var quo;
                    var divrem = (mantissaDividend).divideAndRemainder(mantissaDivisor);
                    quo = divrem[0];
                    rem = divrem[1];
                    quo = this.RoundToScale(quo, rem, mantissaDivisor, new FastInteger(0), negA ^ negB, ctx);
                    return this.helper.CreateNew(quo, desiredExponent);
                }
            }
            var resultPrecision = new FastInteger(1);
            var mantcmp = mantissaDividend.compareTo(mantissaDivisor);
            if (mantcmp < 0) {
                
                var dividendPrecision = this.helper.CreateShiftAccumulator(mantissaDividend).GetDigitLength();
                var divisorPrecision = this.helper.CreateShiftAccumulator(mantissaDivisor).GetDigitLength();
                divisorPrecision.Subtract(dividendPrecision);
                if (divisorPrecision.signum() == 0) divisorPrecision.AddInt(1);
                
                mantissaDividend = this.helper.MultiplyByRadixPower(mantissaDividend, divisorPrecision);
                adjust.Add(divisorPrecision);
                if (mantissaDividend.compareTo(mantissaDivisor) < 0) {
                    
                    if (radix == 2) {
                        mantissaDividend = mantissaDividend.shiftLeft(1);
                    } else {
                        mantissaDividend = mantissaDividend.multiply(BigInteger.valueOf(radix));
                    }
                    adjust.AddInt(1);
                }
            } else if (mantcmp > 0) {
                
                var dividendPrecision = this.helper.CreateShiftAccumulator(mantissaDividend).GetDigitLength();
                var divisorPrecision = this.helper.CreateShiftAccumulator(mantissaDivisor).GetDigitLength();
                dividendPrecision.Subtract(divisorPrecision);
                var oldMantissaB = mantissaDivisor;
                mantissaDivisor = this.helper.MultiplyByRadixPower(mantissaDivisor, dividendPrecision);
                adjust.Subtract(dividendPrecision);
                if (mantissaDividend.compareTo(mantissaDivisor) < 0) {
                    
                    if (dividendPrecision.CompareToInt(1) == 0) {
                        
                        mantissaDivisor = oldMantissaB;
                    } else {
                        var bigpow = BigInteger.valueOf(radix);
                        mantissaDivisor = mantissaDivisor.divide(bigpow);
                    }
                    adjust.AddInt(1);
                }
            }
            if (mantcmp == 0) {
                result = new FastInteger(1);
                mantissaDividend = BigInteger.ZERO;
            } else {
                var check = 0;
                var divs = FastInteger.FromBig(mantissaDivisor);
                var divd = FastInteger.FromBig(mantissaDividend);
                var divsHalfRadix = null;
                if (radix != 2) {
                    divsHalfRadix = FastInteger.FromBig(mantissaDivisor).Multiply((radix / 2)|0);
                }
                var hasPrecision = ctx != null && (ctx.getPrecision()).signum() != 0;
                while (true) {
                    var remainderZero = false;
                    if (check == RadixMath.NonTerminatingCheckThreshold && !hasPrecision && integerMode == RadixMath.IntegerModeRegular) {
                        
                        if (!this.helper.HasTerminatingRadixExpansion(divd.AsBigInteger(), mantissaDivisor)) {
                            throw ("Result would have a nonterminating expansion");
                        }
                        check++;
                    } else if (check < RadixMath.NonTerminatingCheckThreshold) {
                        check++;
                    }
                    var count = 0;
                    if (divsHalfRadix != null && divd.compareTo(divsHalfRadix) >= 0) {
                        divd.Subtract(divsHalfRadix);
                        count += ((radix / 2)|0);
                    }
                    while (divd.compareTo(divs) >= 0) {
                        divd.Subtract(divs);
                        count++;
                    }
                    result.AddInt(count);
                    remainderZero = (divd.signum() == 0);
                    if (hasPrecision && resultPrecision.compareTo(fastPrecision) == 0) {
                        mantissaDividend = divd.AsBigInteger();
                        break;
                    }
                    if (remainderZero && adjust.signum() >= 0) {
                        mantissaDividend = divd.AsBigInteger();
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
            
            var exp = FastInteger.Copy(expdiff).Subtract(adjust);
            var lastDiscarded = 0;
            var olderDiscarded = 0;
            if (!(mantissaDividend.signum() == 0)) {
                var halfDivisor = (mantissaDivisor.shiftRight(1));
                var cmpHalf = mantissaDividend.compareTo(halfDivisor);
                if ((cmpHalf == 0) && mantissaDivisor.testBit(0) == false) {
                    
                    lastDiscarded = ((radix / 2)|0);
                    olderDiscarded = 0;
                } else if (cmpHalf > 0) {
                    
                    lastDiscarded = ((radix / 2)|0);
                    olderDiscarded = 1;
                } else {
                    
                    lastDiscarded = 0;
                    olderDiscarded = 1;
                }
            }
            var bigResult = result.AsBigInteger();
            if (negA ^ negB) {
                bigResult = bigResult.negate();
            }
            return this.RoundToPrecisionWithDigits(this.helper.CreateNew(bigResult, exp.AsBigInteger()), ctx, lastDiscarded, olderDiscarded);
        }
    };
    
    prototype.MinMagnitude = function(a, b) {
        if (a == null) throw ("a");
        if (b == null) throw ("b");
        var cmp = this.compareTo(this.Abs(a), this.Abs(b));
        if (cmp == 0) return this.Min(a, b);
        return (cmp < 0) ? a : b;
    };
    
    prototype.MaxMagnitude = function(a, b) {
        if (a == null) throw ("a");
        if (b == null) throw ("b");
        var cmp = this.compareTo(this.Abs(a), this.Abs(b));
        if (cmp == 0) return this.Max(a, b);
        return (cmp > 0) ? a : b;
    };
    
    prototype.Max = function(a, b) {
        if (a == null) throw ("a");
        if (b == null) throw ("b");
        var cmp = this.compareTo(a, b);
        if (cmp != 0) return cmp > 0 ? a : b;
        
        if (this.helper.GetSign(a) >= 0) {
            return this.helper.GetExponent(a).compareTo(this.helper.GetExponent(b)) > 0 ? a : b;
        } else {
            return this.helper.GetExponent(a).compareTo(this.helper.GetExponent(b)) > 0 ? b : a;
        }
    };
    
    prototype.Multiply = function(thisValue, decfrac, ctx) {
        var bigintOp2 = this.helper.GetExponent(decfrac);
        var newexp = (this.helper.GetExponent(thisValue).add(bigintOp2));
        var ret = this.helper.CreateNew(this.helper.GetMantissa(thisValue).multiply(this.helper.GetMantissa(decfrac)), newexp);
        if (ctx != null) {
            ret = this.RoundToPrecision(ret, ctx);
        }
        return ret;
    };
    
    prototype.MultiplyAndAdd = function(thisValue, multiplicand, augend, ctx) {
        var bigintOp2 = this.helper.GetExponent(multiplicand);
        var newexp = (this.helper.GetExponent(thisValue).add(bigintOp2));
        bigintOp2 = this.helper.GetMantissa(multiplicand);
        bigintOp2 = bigintOp2.multiply(this.helper.GetMantissa(thisValue));
        var addend = this.helper.CreateNew(bigintOp2, newexp);
        return this.Add(addend, augend, ctx);
    };
    
    prototype.RoundToBinaryPrecision = function(thisValue, context) {
        return this.RoundToBinaryPrecisionWithDigits(thisValue, context, 0, 0);
    };
    prototype.RoundToBinaryPrecisionWithDigits = function(thisValue, context, lastDiscarded, olderDiscarded) {
        if ((context) == null) return thisValue;
        if ((context.getPrecision()).signum() == 0 && !context.getHasExponentRange() && (lastDiscarded | olderDiscarded) == 0) return thisValue;
        if ((context.getPrecision()).signum() == 0 || this.helper.GetRadix() == 2) return this.RoundToPrecisionWithDigits(thisValue, context, lastDiscarded, olderDiscarded);
        var fastEMin = (context.getHasExponentRange()) ? FastInteger.FromBig(context.getEMin()) : null;
        var fastEMax = (context.getHasExponentRange()) ? FastInteger.FromBig(context.getEMax()) : null;
        var fastPrecision = FastInteger.FromBig(context.getPrecision());
        var signals = [0];
        var dfrac = this.RoundToBinaryPrecisionInternal(thisValue, fastPrecision, context.getRounding(), fastEMin, fastEMax, lastDiscarded, olderDiscarded, signals);
        
        if (context.getClampNormalExponents() && dfrac != null) {
            var digitCount = null;
            if (this.helper.GetRadix() == 2) {
                digitCount = FastInteger.Copy(fastPrecision);
            } else {
                
                var maxMantissa = BigInteger.ONE;
                var prec = FastInteger.Copy(fastPrecision);
                while (prec.signum() > 0) {
                    var shift = prec.CompareToInt(1000000) >= 0 ? 1000000 : prec.AsInt32();
                    maxMantissa = maxMantissa.shiftLeft(shift);
                    prec.SubtractInt(shift);
                }
                maxMantissa = maxMantissa.subtract(BigInteger.ONE);
                
                digitCount = this.helper.CreateShiftAccumulator(maxMantissa).GetDigitLength();
            }
            var clamp = FastInteger.Copy(fastEMax).AddInt(1).Subtract(digitCount);
            var fastExp = FastInteger.FromBig(this.helper.GetExponent(dfrac));
            if (fastExp.compareTo(clamp) > 0) {
                var bigmantissa = this.helper.GetMantissa(dfrac);
                var sign = bigmantissa.signum();
                if (sign != 0) {
                    if (sign < 0) bigmantissa = bigmantissa.negate();
                    var expdiff = FastInteger.Copy(fastExp).Subtract(clamp);
                    bigmantissa = this.helper.MultiplyByRadixPower(bigmantissa, expdiff);
                    if (sign < 0) bigmantissa = bigmantissa.negate();
                }
                if (signals != null) signals[0] = signals[0] | PrecisionContext.FlagClamped;
                dfrac = this.helper.CreateNew(bigmantissa, clamp.AsBigInteger());
            }
        }
        if (context.getHasFlags()) {
            context.setFlags(context.getFlags() | signals[0]);
        }
        return dfrac;
    };
    
    prototype.RoundToPrecision = function(thisValue, context) {
        return this.RoundToPrecisionWithDigits(thisValue, context, 0, 0);
    };
    prototype.RoundToPrecisionWithDigits = function(thisValue, context, lastDiscarded, olderDiscarded) {
        if ((context) == null) return thisValue;
        if ((context.getPrecision()).signum() == 0 && !context.getHasExponentRange() && (lastDiscarded | olderDiscarded) == 0) return thisValue;
        var fastEMin = (context.getHasExponentRange()) ? FastInteger.FromBig(context.getEMin()) : null;
        var fastEMax = (context.getHasExponentRange()) ? FastInteger.FromBig(context.getEMax()) : null;
        var fastPrecision = FastInteger.FromBig(context.getPrecision());
        if (fastPrecision.signum() > 0 && fastPrecision.CompareToInt(18) <= 0 && (lastDiscarded | olderDiscarded) == 0) {
            
            var mantabs = (this.helper.GetMantissa(thisValue)).abs();
            if (mantabs.compareTo(this.helper.MultiplyByRadixPower(BigInteger.ONE, fastPrecision)) < 0) {
                if (!context.getHasExponentRange()) return thisValue;
                var fastExp = FastInteger.FromBig(this.helper.GetExponent(thisValue));
                var fastAdjustedExp = FastInteger.Copy(fastExp).Add(fastPrecision).SubtractInt(1);
                var fastNormalMin = FastInteger.Copy(fastEMin).Add(fastPrecision).SubtractInt(1);
                if (fastAdjustedExp.compareTo(fastEMax) <= 0 && fastAdjustedExp.compareTo(fastNormalMin) >= 0) {
                    return thisValue;
                }
            }
        }
        var signals = [0];
        var dfrac = this.RoundToPrecisionInternal(thisValue, fastPrecision, context.getRounding(), fastEMin, fastEMax, lastDiscarded, olderDiscarded, signals);
        if (context.getClampNormalExponents() && dfrac != null) {
            
            var clamp = FastInteger.Copy(fastEMax).AddInt(1).Subtract(fastPrecision);
            var fastExp = FastInteger.FromBig(this.helper.GetExponent(dfrac));
            if (fastExp.compareTo(clamp) > 0) {
                var bigmantissa = this.helper.GetMantissa(dfrac);
                var sign = bigmantissa.signum();
                if (sign != 0) {
                    if (sign < 0) bigmantissa = bigmantissa.negate();
                    var expdiff = FastInteger.Copy(fastExp).Subtract(clamp);
                    bigmantissa = this.helper.MultiplyByRadixPower(bigmantissa, expdiff);
                    if (sign < 0) bigmantissa = bigmantissa.negate();
                }
                if (signals != null) signals[0] = signals[0] | PrecisionContext.FlagClamped;
                dfrac = this.helper.CreateNew(bigmantissa, clamp.AsBigInteger());
            }
        }
        if (context.getHasFlags()) {
            context.setFlags(context.getFlags() | signals[0]);
        }
        return dfrac;
    };
    
    prototype.Quantize = function(thisValue, otherValue, ctx) {
        var expOther = this.helper.GetExponent(otherValue);
        if (ctx != null && !ctx.ExponentWithinRange(expOther)) throw ("Exponent not within exponent range: " + expOther.toString());
        var tmpctx = (ctx == null ? PrecisionContext.ForRounding(Rounding.HalfEven) : ctx.Copy()).WithBlankFlags();
        var mantThis = (this.helper.GetMantissa(thisValue)).abs();
        var expThis = this.helper.GetExponent(thisValue);
        var expcmp = expThis.compareTo(expOther);
        var signThis = this.helper.GetSign(thisValue);
        var ret = null;
        if (expcmp == 0) {
            ret = this.RoundToPrecision(thisValue, tmpctx);
        } else if (mantThis.signum() == 0) {
            ret = this.helper.CreateNew(BigInteger.ZERO, expOther);
            ret = this.RoundToPrecision(ret, tmpctx);
        } else if (expcmp > 0) {
            
            var radixPower = FastInteger.FromBig(expThis).SubtractBig(expOther);
            if ((tmpctx.getPrecision()).signum() > 0 && radixPower.compareTo(FastInteger.FromBig(tmpctx.getPrecision()).AddInt(10)) > 0) {
                
                throw "exception";
            }
            mantThis = this.helper.MultiplyByRadixPower(mantThis, radixPower);
            if (signThis < 0) mantThis = mantThis.negate();
            ret = this.helper.CreateNew(mantThis, expOther);
            ret = this.RoundToPrecision(ret, tmpctx);
        } else {
            
            var accum = this.helper.CreateShiftAccumulator(mantThis);
            accum.ShiftRight(FastInteger.FromBig(expOther).SubtractBig(expThis));
            mantThis = accum.getShiftedInt();
            if (signThis < 0) mantThis = mantThis.negate();
            ret = this.helper.CreateNew(mantThis, expOther);
            ret = this.RoundToPrecisionWithDigits(ret, tmpctx, accum.getLastDiscardedDigit(), accum.getOlderDiscardedDigits());
        }
        if ((tmpctx.getFlags() & PrecisionContext.FlagOverflow) != 0) {
            throw "exception";
        }
        if (ret == null || !this.helper.GetExponent(ret).equals(expOther)) {
            throw "exception";
        }
        if (signThis < 0 && this.helper.GetSign(ret) > 0) {
            var mantRet = this.helper.GetMantissa(ret);
            mantRet = (mantRet).negate();
            ret = this.helper.CreateNew(mantRet, this.helper.GetExponent(ret));
        }
        if (ctx != null && ctx.getHasFlags()) {
            var flags = tmpctx.getFlags();
            flags &= ~PrecisionContext.FlagUnderflow;
            ctx.setFlags(ctx.getFlags() | flags);
        }
        return ret;
    };
    
    prototype.RoundToExponentExact = function(thisValue, expOther, ctx) {
        if (this.helper.GetExponent(thisValue).compareTo(expOther) >= 0) {
            return this.RoundToPrecision(thisValue, ctx);
        } else {
            var pctx = (ctx == null) ? null : ctx.WithPrecision(0).WithBlankFlags();
            var ret = this.Quantize(thisValue, this.helper.CreateNew(BigInteger.ONE, expOther), pctx);
            if (ctx != null && ctx.getHasFlags()) {
                ctx.setFlags(ctx.getFlags() | pctx.getFlags());
            }
            return ret;
        }
    };
    
    prototype.RoundToExponentSimple = function(thisValue, expOther, ctx) {
        if (this.helper.GetExponent(thisValue).compareTo(expOther) >= 0) {
            return this.RoundToPrecision(thisValue, ctx);
        } else {
            if (ctx != null && !ctx.ExponentWithinRange(expOther)) throw ("Exponent not within exponent range: " + expOther.toString());
            var bigmantissa = this.helper.GetMantissa(thisValue);
            var neg = bigmantissa.signum() < 0;
            if (neg) bigmantissa = bigmantissa.negate();
            var shift = FastInteger.FromBig(expOther).SubtractBig(this.helper.GetExponent(thisValue));
            var accum = this.helper.CreateShiftAccumulator(bigmantissa);
            accum.ShiftRight(shift);
            bigmantissa = accum.getShiftedInt();
            if (neg) bigmantissa = bigmantissa.negate();
            return this.RoundToPrecisionWithDigits(this.helper.CreateNew(bigmantissa, expOther), ctx, accum.getLastDiscardedDigit(), accum.getOlderDiscardedDigits());
        }
    };
    
    prototype.RoundToExponentNoRoundedFlag = function(thisValue, exponent, ctx) {
        var pctx = (ctx == null) ? null : ctx.WithBlankFlags();
        var ret = this.RoundToExponentExact(thisValue, exponent, pctx);
        if (ctx != null && ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (pctx.getFlags() & ~(PrecisionContext.FlagInexact | PrecisionContext.FlagRounded)));
        }
        return ret;
    };
    
    prototype.Reduce = function(thisValue, ctx) {
        var ret = this.RoundToPrecision(thisValue, ctx);
        if (ret != null) {
            var bigmant = (this.helper.GetMantissa(ret)).abs();
            var exp = FastInteger.FromBig(this.helper.GetExponent(ret));
            if (bigmant.signum() == 0) {
                exp = new FastInteger(0);
            } else {
                var radix = this.helper.GetRadix();
                var bigradix = BigInteger.valueOf(radix);
                while (!(bigmant.signum() == 0)) {
                    var bigrem;
                    var bigquo;
                    var divrem = (bigmant).divideAndRemainder(bigradix);
                    bigquo = divrem[0];
                    bigrem = divrem[1];
                    if (bigrem.signum() != 0) break;
                    bigmant = bigquo;
                    exp.AddInt(1);
                }
            }
            var sign = (this.helper.GetSign(ret) < 0);
            ret = this.helper.CreateNew(bigmant, exp.AsBigInteger());
            if (ctx != null && ctx.getClampNormalExponents()) {
                ret = this.RoundToPrecision(ret, ctx);
            }
            ret = this.EnsureSign(ret, sign);
        }
        return ret;
    };
    
    prototype.SquareRoot = function(thisValue, ctx) {
        if ((thisValue) == null) throw ("thisValue");
        if ((ctx) == null) throw ("ctx");
        if ((ctx.getPrecision()).signum() <= 0) throw ("ctx.getPrecision()" + " not less than " + "0" + " (" + (ctx.getPrecision()) + ")");
        var sign = this.helper.GetSign(thisValue);
        if (sign < 0) throw "exception";
        throw "exception";
    };
    prototype.RoundToBinaryPrecisionInternal = function(thisValue, precision, rounding, fastEMin, fastEMax, lastDiscarded, olderDiscarded, signals) {
        if (precision.signum() < 0) throw ("precision" + " not greater or equal to " + "0" + " (" + (precision) + ")");
        if (this.helper.GetRadix() == 2 || precision.signum() == 0) {
            return this.RoundToPrecisionInternal(thisValue, precision, rounding, fastEMin, fastEMax, lastDiscarded, olderDiscarded, signals);
        }
        var neg = this.helper.GetMantissa(thisValue).signum() < 0;
        var bigmantissa = this.helper.GetMantissa(thisValue);
        if (neg) bigmantissa = bigmantissa.negate();
        
        var oldmantissa = bigmantissa;
        var maxMantissa = BigInteger.ONE;
        var prec = FastInteger.Copy(precision);
        while (prec.signum() > 0) {
            var shift = (prec.CompareToInt(1000000) >= 0) ? 1000000 : prec.AsInt32();
            maxMantissa = maxMantissa.shiftLeft(shift);
            prec.SubtractInt(shift);
        }
        maxMantissa = maxMantissa.subtract(BigInteger.ONE);
        var exp = FastInteger.FromBig(this.helper.GetExponent(thisValue));
        var flags = 0;
        var accumMaxMant = this.helper.CreateShiftAccumulator(maxMantissa);
        
        var digitCount = accumMaxMant.GetDigitLength();
        var accum = this.helper.CreateShiftAccumulatorWithDigits(bigmantissa, lastDiscarded, olderDiscarded);
        accum.ShiftToDigits(digitCount);
        while ((accum.getShiftedInt()).compareTo(maxMantissa) > 0) {
            accum.ShiftRightInt(1);
        }
        var discardedBits = FastInteger.Copy(accum.getDiscardedDigitCount());
        exp.Add(discardedBits);
        var adjExponent = FastInteger.Copy(exp).Add(accum.GetDigitLength()).SubtractInt(1);
        var clamp = null;
        if (fastEMax != null && adjExponent.compareTo(fastEMax) == 0) {
            
            var expdiff = FastInteger.Copy(digitCount).Subtract(accum.GetDigitLength());
            var currMantissa = accum.getShiftedInt();
            currMantissa = this.helper.MultiplyByRadixPower(currMantissa, expdiff);
            if ((currMantissa).compareTo(maxMantissa) > 0) {
                
                adjExponent.AddInt(1);
            }
        }
        if (fastEMax != null && adjExponent.compareTo(fastEMax) > 0) {
            if (oldmantissa.signum() == 0) {
                flags |= PrecisionContext.FlagClamped;
                if (signals != null) signals[0] = flags;
                return this.helper.CreateNew(oldmantissa, fastEMax.AsBigInteger());
            }
            
            flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
            if (rounding == Rounding.Unnecessary) throw ("Rounding was required");
            if (rounding == Rounding.Down || rounding == Rounding.ZeroFiveUp || (rounding == Rounding.Ceiling && neg) || (rounding == Rounding.Floor && !neg)) {
                
                var overflowMant = maxMantissa;
                if (neg) overflowMant = overflowMant.negate();
                if (signals != null) signals[0] = flags;
                clamp = FastInteger.Copy(fastEMax).AddInt(1).Subtract(digitCount);
                return this.helper.CreateNew(overflowMant, clamp.AsBigInteger());
            }
            if (signals != null) signals[0] = flags;
            return null;
        } else if (fastEMin != null && adjExponent.compareTo(fastEMin) < 0) {
            
            var fastETiny = FastInteger.Copy(fastEMin).Subtract(digitCount).AddInt(1);
            if (oldmantissa.signum() != 0) flags |= PrecisionContext.FlagSubnormal;
            if (exp.compareTo(fastETiny) < 0) {
                var expdiff = FastInteger.Copy(fastETiny).Subtract(exp);
                expdiff.Add(discardedBits);
                accum = this.helper.CreateShiftAccumulatorWithDigits(oldmantissa, lastDiscarded, olderDiscarded);
                accum.ShiftRight(expdiff);
                var newmantissa = accum.getShiftedInt();
                if ((accum.getDiscardedDigitCount()).signum() != 0 || (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                    if (oldmantissa.signum() != 0) flags |= PrecisionContext.FlagRounded;
                    if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                        flags |= PrecisionContext.FlagInexact;
                        if (rounding == Rounding.Unnecessary) throw ("Rounding was required");
                    }
                    if (this.RoundGivenBigInt(accum, rounding, neg, newmantissa)) {
                        newmantissa = newmantissa.add(BigInteger.ONE);
                    }
                }
                if (newmantissa.signum() == 0) flags |= PrecisionContext.FlagClamped;
                if ((flags & (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact)) == (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact)) flags |= PrecisionContext.FlagUnderflow | PrecisionContext.FlagRounded;
                if (signals != null) signals[0] = flags;
                if (neg) newmantissa = newmantissa.negate();
                return this.helper.CreateNew(newmantissa, fastETiny.AsBigInteger());
            }
        }
        var mantChanged = false;
        if ((accum.getDiscardedDigitCount()).signum() != 0 || (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
            if (bigmantissa.signum() != 0) flags |= PrecisionContext.FlagRounded;
            bigmantissa = accum.getShiftedInt();
            if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                flags |= PrecisionContext.FlagInexact;
                if (rounding == Rounding.Unnecessary) throw ("Rounding was required");
            }
            if (this.RoundGivenBigInt(accum, rounding, neg, bigmantissa)) {
                bigmantissa = bigmantissa.add(BigInteger.ONE);
                mantChanged = true;
                if (bigmantissa.testBit(0) == false) {
                    accum = this.helper.CreateShiftAccumulator(bigmantissa);
                    accum.ShiftToDigits(digitCount);
                    while ((accum.getShiftedInt()).compareTo(maxMantissa) > 0) {
                        accum.ShiftRightInt(1);
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
            
            adjExponent = FastInteger.Copy(exp);
            adjExponent.Add(accum.GetDigitLength()).SubtractInt(1);
            if (fastEMax != null && adjExponent.compareTo(fastEMax) == 0 && mantChanged) {
                
                var expdiff = FastInteger.Copy(digitCount).Subtract(accum.GetDigitLength());
                var currMantissa = accum.getShiftedInt();
                currMantissa = this.helper.MultiplyByRadixPower(currMantissa, expdiff);
                if ((currMantissa).compareTo(maxMantissa) > 0) {
                    
                    adjExponent.AddInt(1);
                }
            }
            if (adjExponent.compareTo(fastEMax) > 0) {
                flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
                if (rounding == Rounding.Down || rounding == Rounding.ZeroFiveUp || (rounding == Rounding.Ceiling && neg) || (rounding == Rounding.Floor && !neg)) {
                    
                    var overflowMant = maxMantissa;
                    if (neg) overflowMant = overflowMant.negate();
                    if (signals != null) signals[0] = flags;
                    clamp = FastInteger.Copy(fastEMax).AddInt(1).Subtract(digitCount);
                    return this.helper.CreateNew(overflowMant, clamp.AsBigInteger());
                }
                if (signals != null) signals[0] = flags;
                return null;
            }
        }
        if (signals != null) signals[0] = flags;
        if (neg) bigmantissa = bigmantissa.negate();
        return this.helper.CreateNew(bigmantissa, exp.AsBigInteger());
    };
    prototype.RoundToPrecisionInternal = function(thisValue, precision, rounding, fastEMin, fastEMax, lastDiscarded, olderDiscarded, signals) {
        if (precision.signum() < 0) throw ("precision" + " not greater or equal to " + "0" + " (" + precision + ")");
        var bigmantissa = this.helper.GetMantissa(thisValue);
        var neg = bigmantissa.signum() < 0;
        if (neg) bigmantissa = bigmantissa.negate();
        
        var oldmantissa = bigmantissa;
        var exp = FastInteger.FromBig(this.helper.GetExponent(thisValue));
        var flags = 0;
        var accum = this.helper.CreateShiftAccumulatorWithDigits(bigmantissa, lastDiscarded, olderDiscarded);
        var unlimitedPrec = (precision.signum() == 0);
        if (precision.signum() > 0) {
            accum.ShiftToDigits(precision);
        } else {
            precision = accum.GetDigitLength();
        }
        var discardedBits = FastInteger.Copy(accum.getDiscardedDigitCount());
        var fastPrecision = precision;
        exp.Add(discardedBits);
        var adjExponent = FastInteger.Copy(exp).Add(accum.GetDigitLength()).SubtractInt(1);
        var clamp = null;
        if (fastEMax != null && adjExponent.compareTo(fastEMax) > 0) {
            if (oldmantissa.signum() == 0) {
                flags |= PrecisionContext.FlagClamped;
                if (signals != null) signals[0] = flags;
                return this.helper.CreateNew(oldmantissa, fastEMax.AsBigInteger());
            }
            
            flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
            if (rounding == Rounding.Unnecessary) throw ("Rounding was required");
            if (!unlimitedPrec && (rounding == Rounding.Down || rounding == Rounding.ZeroFiveUp || (rounding == Rounding.Ceiling && neg) || (rounding == Rounding.Floor && !neg))) {
                
                var overflowMant = this.helper.MultiplyByRadixPower(BigInteger.ONE, fastPrecision);
                overflowMant = overflowMant.subtract(BigInteger.ONE);
                if (neg) overflowMant = overflowMant.negate();
                if (signals != null) signals[0] = flags;
                clamp = FastInteger.Copy(fastEMax).AddInt(1).Subtract(fastPrecision);
                return this.helper.CreateNew(overflowMant, clamp.AsBigInteger());
            }
            if (signals != null) signals[0] = flags;
            return null;
        } else if (fastEMin != null && adjExponent.compareTo(fastEMin) < 0) {
            
            var fastETiny = FastInteger.Copy(fastEMin).Subtract(fastPrecision).AddInt(1);
            if (oldmantissa.signum() != 0) flags |= PrecisionContext.FlagSubnormal;
            if (exp.compareTo(fastETiny) < 0) {
                var expdiff = FastInteger.Copy(fastETiny).Subtract(exp);
                expdiff.Add(discardedBits);
                accum = this.helper.CreateShiftAccumulatorWithDigits(oldmantissa, lastDiscarded, olderDiscarded);
                accum.ShiftRight(expdiff);
                var newmantissa = accum.getShiftedIntFast();
                if ((accum.getDiscardedDigitCount()).signum() != 0 || (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                    if (oldmantissa.signum() != 0) flags |= PrecisionContext.FlagRounded;
                    if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                        flags |= PrecisionContext.FlagInexact;
                        if (rounding == Rounding.Unnecessary) throw ("Rounding was required");
                    }
                    if (this.Round(accum, rounding, neg, newmantissa)) {
                        newmantissa.AddInt(1);
                    }
                }
                if (newmantissa.signum() == 0) flags |= PrecisionContext.FlagClamped;
                if ((flags & (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact)) == (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact)) flags |= PrecisionContext.FlagUnderflow | PrecisionContext.FlagRounded;
                if (signals != null) signals[0] = flags;
                if (neg) newmantissa.Negate();
                return this.helper.CreateNew(newmantissa.AsBigInteger(), fastETiny.AsBigInteger());
            }
        }
        var expChanged = false;
        if ((accum.getDiscardedDigitCount()).signum() != 0 || (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
            if (bigmantissa.signum() != 0) flags |= PrecisionContext.FlagRounded;
            bigmantissa = accum.getShiftedInt();
            if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                flags |= PrecisionContext.FlagInexact;
                if (rounding == Rounding.Unnecessary) throw ("Rounding was required");
            }
            if (this.RoundGivenBigInt(accum, rounding, neg, bigmantissa)) {
                bigmantissa = bigmantissa.add(BigInteger.ONE);
                if (bigmantissa.testBit(0) == false) {
                    accum = this.helper.CreateShiftAccumulator(bigmantissa);
                    accum.ShiftToDigits(fastPrecision);
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
            
            adjExponent = FastInteger.Copy(exp);
            adjExponent.Add(accum.GetDigitLength()).SubtractInt(1);
            if (adjExponent.compareTo(fastEMax) > 0) {
                flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
                if (!unlimitedPrec && (rounding == Rounding.Down || rounding == Rounding.ZeroFiveUp || (rounding == Rounding.Ceiling && neg) || (rounding == Rounding.Floor && !neg))) {
                    
                    var overflowMant = this.helper.MultiplyByRadixPower(BigInteger.ONE, fastPrecision);
                    overflowMant = overflowMant.subtract(BigInteger.ONE);
                    if (neg) overflowMant = overflowMant.negate();
                    if (signals != null) signals[0] = flags;
                    clamp = FastInteger.Copy(fastEMax).AddInt(1).Subtract(fastPrecision);
                    return this.helper.CreateNew(overflowMant, clamp.AsBigInteger());
                }
                if (signals != null) signals[0] = flags;
                return null;
            }
        }
        if (signals != null) signals[0] = flags;
        if (neg) bigmantissa = bigmantissa.negate();
        return this.helper.CreateNew(bigmantissa, exp.AsBigInteger());
    };
    
    prototype.Add = function(thisValue, decfrac, ctx) {
        var expcmp = this.helper.GetExponent(thisValue).compareTo(this.helper.GetExponent(decfrac));
        var retval = null;
        if (expcmp == 0) {
            retval = this.helper.CreateNew(this.helper.GetMantissa(thisValue).add(this.helper.GetMantissa(decfrac)), this.helper.GetExponent(thisValue));
        } else {
            
            var op1 = thisValue;
            var op2 = decfrac;
            var op1Exponent = this.helper.GetExponent(op1);
            var op2Exponent = this.helper.GetExponent(op2);
            var resultExponent = (expcmp < 0 ? op1Exponent : op2Exponent);
            var fastOp1Exp = FastInteger.FromBig(op1Exponent);
            var fastOp2Exp = FastInteger.FromBig(op2Exponent);
            var expdiff = FastInteger.Copy(fastOp1Exp).Subtract(fastOp2Exp).Abs();
            if (ctx != null && (ctx.getPrecision()).signum() > 0) {
                
                var fastPrecision = FastInteger.FromBig(ctx.getPrecision());
                
                if (FastInteger.Copy(expdiff).compareTo(fastPrecision) > 0) {
                    var op1MantAbs = (this.helper.GetMantissa(op1)).abs();
                    var op2MantAbs = (this.helper.GetMantissa(op2)).abs();
                    var expcmp2 = fastOp1Exp.compareTo(fastOp2Exp);
                    if (expcmp2 < 0) {
                        if (!(op2MantAbs.signum() == 0)) {
                            
                            var digitLength1 = this.helper.CreateShiftAccumulator(op1MantAbs).GetDigitLength();
                            if (FastInteger.Copy(fastOp1Exp).Add(digitLength1).AddInt(2).compareTo(fastOp2Exp) < 0) {
                                
                                var tmp = FastInteger.Copy(fastOp2Exp).SubtractInt(8).Subtract(digitLength1).SubtractBig(ctx.getPrecision());
                                var newDiff = FastInteger.Copy(tmp).Subtract(fastOp2Exp).Abs();
                                if (newDiff.compareTo(expdiff) < 0) {
                                    
                                    if (this.helper.GetSign(thisValue) == this.helper.GetSign(decfrac)) {
                                        var digitLength2 = this.helper.CreateShiftAccumulator(op2MantAbs).GetDigitLength();
                                        if (digitLength2.compareTo(fastPrecision) < 0) {
                                            
                                            var precisionDiff = FastInteger.Copy(fastPrecision).Subtract(digitLength2);
                                            op2MantAbs = this.helper.MultiplyByRadixPower(op2MantAbs, precisionDiff);
                                            var bigintTemp = precisionDiff.AsBigInteger();
                                            op2Exponent = op2Exponent.subtract(bigintTemp);
                                            if (this.helper.GetSign(decfrac) < 0) op2MantAbs = op2MantAbs.negate();
                                            decfrac = this.helper.CreateNew(op2MantAbs, op2Exponent);
                                            return this.RoundToPrecisionWithDigits(decfrac, ctx, 0, 1);
                                        } else {
                                            return this.RoundToPrecisionWithDigits(decfrac, ctx, 0, 1);
                                        }
                                    } else {
                                        op1Exponent = (tmp.AsBigInteger());
                                    }
                                }
                            }
                        }
                    } else if (expcmp2 > 0) {
                        if (!(op1MantAbs.signum() == 0)) {
                            
                            var digitLength2 = this.helper.CreateShiftAccumulator(op2MantAbs).GetDigitLength();
                            if (FastInteger.Copy(fastOp2Exp).Add(digitLength2).AddInt(2).compareTo(fastOp1Exp) < 0) {
                                
                                var tmp = FastInteger.Copy(fastOp1Exp).SubtractInt(8).Subtract(digitLength2).SubtractBig(ctx.getPrecision());
                                var newDiff = FastInteger.Copy(tmp).Subtract(fastOp1Exp).Abs();
                                if (newDiff.compareTo(expdiff) < 0) {
                                    
                                    if (this.helper.GetSign(thisValue) == this.helper.GetSign(decfrac)) {
                                        var digitLength1 = this.helper.CreateShiftAccumulator(op1MantAbs).GetDigitLength();
                                        if (digitLength1.compareTo(fastPrecision) < 0) {
                                            
                                            var precisionDiff = FastInteger.Copy(fastPrecision).Subtract(digitLength1);
                                            op1MantAbs = this.helper.MultiplyByRadixPower(op1MantAbs, precisionDiff);
                                            var bigintTemp = precisionDiff.AsBigInteger();
                                            op1Exponent = op1Exponent.subtract(bigintTemp);
                                            if (this.helper.GetSign(thisValue) < 0) op1MantAbs = op1MantAbs.negate();
                                            thisValue = this.helper.CreateNew(op1MantAbs, op1Exponent);
                                            return this.RoundToPrecisionWithDigits(thisValue, ctx, 0, 1);
                                        } else {
                                            return this.RoundToPrecisionWithDigits(thisValue, ctx, 0, 1);
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
                var newmant = this.helper.RescaleByExponentDiff(this.helper.GetMantissa(op1), op1Exponent, op2Exponent);
                retval = this.helper.CreateNew(newmant.add(this.helper.GetMantissa(op2)), resultExponent);
            } else {
                var newmant = this.helper.RescaleByExponentDiff(this.helper.GetMantissa(op2), op1Exponent, op2Exponent);
                retval = this.helper.CreateNew(newmant.add(this.helper.GetMantissa(op1)), resultExponent);
            }
        }
        if (ctx != null) {
            retval = this.RoundToPrecision(retval, ctx);
        }
        return retval;
    };
    
    prototype.compareTo = function(thisValue, decfrac) {
        if (decfrac == null) return 1;
        var s = this.helper.GetSign(thisValue);
        var ds = this.helper.GetSign(decfrac);
        if (s != ds) return (s < ds) ? -1 : 1;
        var expcmp = this.helper.GetExponent(thisValue).compareTo(this.helper.GetExponent(decfrac));
        var mantcmp = this.helper.GetMantissa(thisValue).compareTo(this.helper.GetMantissa(decfrac));
        if (mantcmp == 0) {
            
            return s == 0 ? 0 : expcmp * s;
        }
        if (ds == 0) {
            
            return s;
        }
        if (s == 0) {
            
            return -ds;
        }
        if (expcmp == 0) {
            return mantcmp;
        }
        var op1Exponent = this.helper.GetExponent(thisValue);
        var op2Exponent = this.helper.GetExponent(decfrac);
        var fastOp1Exp = FastInteger.FromBig(op1Exponent);
        var fastOp2Exp = FastInteger.FromBig(op2Exponent);
        var expdiff = FastInteger.Copy(fastOp1Exp).Subtract(fastOp2Exp).Abs();
        
        if (expdiff.CompareToInt(100) >= 0) {
            var op1MantAbs = (this.helper.GetMantissa(thisValue)).abs();
            var op2MantAbs = (this.helper.GetMantissa(decfrac)).abs();
            var precision1 = this.helper.CreateShiftAccumulator(op1MantAbs).GetDigitLength();
            var precision2 = this.helper.CreateShiftAccumulator(op2MantAbs).GetDigitLength();
            var maxPrecision = null;
            if (precision1.compareTo(precision2) > 0) maxPrecision = precision1; else maxPrecision = precision2;
            
            if (FastInteger.Copy(expdiff).compareTo(maxPrecision) > 0) {
                var expcmp2 = fastOp1Exp.compareTo(fastOp2Exp);
                if (expcmp2 < 0) {
                    if (!(op2MantAbs.signum() == 0)) {
                        
                        var digitLength1 = this.helper.CreateShiftAccumulator(op1MantAbs).GetDigitLength();
                        if (FastInteger.Copy(fastOp1Exp).Add(digitLength1).AddInt(2).compareTo(fastOp2Exp) < 0) {
                            
                            var tmp = FastInteger.Copy(fastOp2Exp).SubtractInt(8).Subtract(digitLength1).Subtract(maxPrecision);
                            var newDiff = FastInteger.Copy(tmp).Subtract(fastOp2Exp).Abs();
                            if (newDiff.compareTo(expdiff) < 0) {
                                if (s == ds) {
                                    return (s < 0) ? 1 : -1;
                                } else {
                                    op1Exponent = (tmp.AsBigInteger());
                                }
                            }
                        }
                    }
                } else if (expcmp2 > 0) {
                    if (!(op1MantAbs.signum() == 0)) {
                        
                        var digitLength2 = this.helper.CreateShiftAccumulator(op2MantAbs).GetDigitLength();
                        if (FastInteger.Copy(fastOp2Exp).Add(digitLength2).AddInt(2).compareTo(fastOp1Exp) < 0) {
                            
                            var tmp = FastInteger.Copy(fastOp1Exp).SubtractInt(8).Subtract(digitLength2).Subtract(maxPrecision);
                            var newDiff = FastInteger.Copy(tmp).Subtract(fastOp1Exp).Abs();
                            if (newDiff.compareTo(expdiff) < 0) {
                                if (s == ds) {
                                    return (s < 0) ? -1 : 1;
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
            var newmant = this.helper.RescaleByExponentDiff(this.helper.GetMantissa(thisValue), op1Exponent, op2Exponent);
            return newmant.compareTo(this.helper.GetMantissa(decfrac));
        } else {
            var newmant = this.helper.RescaleByExponentDiff(this.helper.GetMantissa(decfrac), op1Exponent, op2Exponent);
            return this.helper.GetMantissa(thisValue).compareTo(newmant);
        }
    };
})(RadixMath,RadixMath.prototype);


if(typeof exports!=="undefined")exports.RadixMath=RadixMath;