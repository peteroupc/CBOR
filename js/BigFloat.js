
var BigFloat = 

function(mantissa, exponent) {

    this.exponent = exponent;
    this.mantissa = mantissa;
};
(function(constructor,prototype){
    prototype.exponent = null;
    prototype.mantissa = null;
    prototype.getExponent = function() {
        return this.exponent;
    };
    prototype.getMantissa = function() {
        return this.mantissa;
    };
    prototype.EqualsInternal = function(other) {
        var otherValue = ((other.constructor==BigFloat) ? other : null);
        if (otherValue == null) return false;
        return this.exponent.equals(otherValue.exponent) && this.mantissa.equals(otherValue.mantissa);
    };
    prototype.equals = function(obj) {
        return this.EqualsInternal((obj.constructor==BigFloat) ? obj : null);
    };
    prototype.hashCode = function() {
        var hashCode_ = 0;
        {
            hashCode_ += 1000000007 * this.exponent.hashCode();
            hashCode_ += 1000000009 * this.mantissa.hashCode();
        }
        return hashCode_;
    };
    constructor.BigShiftIteration = BigInteger.valueOf(1000000);
    constructor.ShiftIteration = 1000000;
    constructor.ShiftLeft = function(val, bigShift) {
        while (bigShift.compareTo(BigFloat.BigShiftIteration) > 0) {
            val = val.shiftLeft(1000000);
            bigShift = bigShift.subtract(BigFloat.BigShiftIteration);
        }
        var lastshift = bigShift.intValue();
        val = val.shiftLeft(lastshift);
        return val;
    };
    constructor.ShiftLeftInt = function(val, shift) {
        while (shift > BigFloat.ShiftIteration) {
            val = val.shiftLeft(1000000);
            shift -= BigFloat.ShiftIteration;
        }
        var lastshift = (shift|0);
        val = val.shiftLeft(lastshift);
        return val;
    };
    constructor.FromBigInteger = function(bigint) {
        return new BigFloat(bigint, BigInteger.ZERO);
    };
    constructor.FromInt64 = function(numberValue_obj) {
        var numberValue = JSInteropFactory.createLong(numberValue_obj);
        var bigint = BigInteger.valueOf(numberValue);
        return new BigFloat(bigint, BigInteger.ZERO);
    };
    
    constructor.FromDecimalFraction = function(decfrac) {
        if ((decfrac) == null) throw ("decfrac");
        var bigintExp = decfrac.getExponent();
        var bigintMant = decfrac.getMantissa();
        if (bigintMant.signum() == 0) return BigFloat.Zero;
        if (bigintExp.signum() == 0) {
            
            return BigFloat.FromBigInteger(bigintMant);
        } else if (bigintExp.signum() > 0) {
            
            var bigmantissa = bigintMant;
            bigmantissa = bigmantissa.multiply(DecimalFraction.FindPowerOfTenFromBig(bigintExp));
            return BigFloat.FromBigInteger(bigmantissa);
        } else {
            
            var scale = FastInteger.FromBig(bigintExp);
            var bigmantissa = bigintMant;
            var neg = (bigmantissa.signum() < 0);
            var remainder;
            if (neg) bigmantissa = (bigmantissa).negate();
            var negscale = FastInteger.Copy(scale).Negate();
            var divisor = DecimalFraction.FindPowerOfFiveFromBig(negscale.AsBigInteger());
            while (true) {
                var quotient;
                var divrem = (bigmantissa).divideAndRemainder(divisor);
                quotient = divrem[0];
                remainder = divrem[1];
                
                if (remainder.signum() != 0 && quotient.compareTo(BigFloat.OneShift62) < 0) {
                    
                    var bits = FastInteger.GetLastWords(quotient, 2);
                    var shift = 0;
                    if ((bits[0] | bits[1]) != 0) {
                        
                        var bitPrecision = DecimalFraction.BitPrecisionInt(bits[1]);
                        if (bitPrecision != 0) bitPrecision += 32; else bitPrecision = DecimalFraction.BitPrecisionInt(bits[0]);
                        shift = 63 - bitPrecision;
                        scale.SubtractInt(shift);
                    } else {
                        
                        shift = 1;
                        scale.SubtractInt(shift);
                    }
                    
                    bigmantissa = bigmantissa.shiftLeft(shift);
                } else {
                    bigmantissa = quotient;
                    break;
                }
            }
            
            var halfDivisor = divisor;
            halfDivisor = halfDivisor.shiftRight(1);
            var cmp = remainder.compareTo(halfDivisor);
            
            if (cmp > 0) {
                
                bigmantissa = bigmantissa.add(BigInteger.ONE);
            }
            if (neg) bigmantissa = (bigmantissa).negate();
            return new BigFloat(bigmantissa, scale.AsBigInteger());
        }
    };
    
    constructor.FromSingle = function(flt) {
        var value = Float.floatToRawIntBits(flt);
        var fpExponent = (((value >> 23) & 255)|0);
        if (fpExponent == 255) throw ("Value is infinity or NaN");
        var fpMantissa = value & 8388607;
        if (fpExponent == 0) fpExponent++; else fpMantissa |= (1 << 23);
        if (fpMantissa != 0) {
            while ((fpMantissa & 1) == 0) {
                fpExponent++;
                fpMantissa >>= 1;
            }
            if ((value >> 31) != 0) fpMantissa = -fpMantissa;
        }
        return new BigFloat(BigInteger.valueOf(JSInteropFactory.createLong(fpMantissa)), BigInteger.valueOf(fpExponent - 150));
    };
    
    constructor.FromDouble = function(dbl) {
        var value = Extras.DoubleToIntegers(dbl);
        var fpExponent = (((value[1] >> 20) & 2047)|0);
        var neg = (value[1] >> 31) != 0;
        if (fpExponent == 2047) throw ("Value is infinity or NaN");
        value[1] = value[1] & 1048575;
        
        if (fpExponent == 0) fpExponent++; else value[1] = value[1] | 1048576;
        if ((value[1] | value[0]) != 0) {
            fpExponent += DecimalFraction.ShiftAwayTrailingZerosTwoElements(value);
        }
        var ret = new BigFloat(FastInteger.WordsToBigInteger(value), BigInteger.valueOf(fpExponent - 1075));
        if (neg) ret = ret.Negate();
        return ret;
    };
    
    prototype.ToBigInteger = function() {
        var expsign = this.getExponent().signum();
        if (expsign == 0) {
            
            return this.getMantissa();
        } else if (expsign > 0) {
            
            var curexp = this.getExponent();
            var bigmantissa = this.getMantissa();
            if (bigmantissa.signum() == 0) return bigmantissa;
            var neg = (bigmantissa.signum() < 0);
            if (neg) bigmantissa = bigmantissa.negate();
            while (curexp.signum() > 0 && bigmantissa.signum() != 0) {
                var shift = 4096;
                if (curexp.compareTo(BigInteger.valueOf(shift)) < 0) {
                    shift = curexp.intValue();
                }
                bigmantissa = bigmantissa.shiftLeft(shift);
                curexp = curexp.subtract(BigInteger.valueOf(shift));
            }
            if (neg) bigmantissa = bigmantissa.negate();
            return bigmantissa;
        } else {
            
            var curexp = this.getExponent();
            var bigmantissa = this.getMantissa();
            if (bigmantissa.signum() == 0) return bigmantissa;
            var neg = (bigmantissa.signum() < 0);
            if (neg) bigmantissa = bigmantissa.negate();
            while (curexp.signum() < 0 && bigmantissa.signum() != 0) {
                var shift = 4096;
                if (curexp.compareTo(BigInteger.valueOf(-4096)) > 0) {
                    shift = -(curexp.intValue());
                }
                bigmantissa = bigmantissa.shiftRight(shift);
                curexp = curexp.add(BigInteger.valueOf(shift));
            }
            if (neg) bigmantissa = bigmantissa.negate();
            return bigmantissa;
        }
    };
    constructor.OneShift23 = BigInteger.ONE.shiftLeft(23);
    constructor.OneShift52 = BigInteger.ONE.shiftLeft(52);
    constructor.OneShift62 = BigInteger.ONE.shiftLeft(62);
    
    prototype.ToSingle = function() {
        var bigmant = (this.mantissa).abs();
        var bigexponent = FastInteger.FromBig(this.exponent);
        var bitLeftmost = 0;
        var bitsAfterLeftmost = 0;
        if (this.mantissa.signum() == 0) {
            return 0.0;
        }
        var smallmant = 0;
        var fastSmallMant;
        if (bigmant.compareTo(BigFloat.OneShift23) < 0) {
            smallmant = bigmant.intValue();
            var exponentchange = 0;
            while (smallmant < (1 << 23)) {
                smallmant <<= 1;
                exponentchange++;
            }
            bigexponent.SubtractInt(exponentchange);
            fastSmallMant = new FastInteger(smallmant);
        } else {
            var accum = new BitShiftAccumulator(bigmant, 0, 0);
            accum.ShiftToDigitsInt(24);
            bitsAfterLeftmost = accum.getOlderDiscardedDigits();
            bitLeftmost = accum.getLastDiscardedDigit();
            bigexponent.Add(accum.getDiscardedDigitCount());
            fastSmallMant = accum.getShiftedIntFast();
        }
        
        if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 || !fastSmallMant.isEvenNumber())) {
            fastSmallMant.AddInt(1);
            if (fastSmallMant.CompareToInt(1 << 24) == 0) {
                fastSmallMant = new FastInteger(1 << 23);
                bigexponent.AddInt(1);
            }
        }
        var subnormal = false;
        if (bigexponent.CompareToInt(104) > 0) {
            
            return (this.mantissa.signum() < 0) ? Number.NEGATIVE_INFINITY : Number.POSITIVE_INFINITY;
        } else if (bigexponent.CompareToInt(-149) < 0) {
            
            subnormal = true;
            
            var accum = BitShiftAccumulator.FromInt32(fastSmallMant.AsInt32());
            var fi = FastInteger.Copy(bigexponent).SubtractInt(-149).Abs();
            accum.ShiftRight(fi);
            bitsAfterLeftmost = accum.getOlderDiscardedDigits();
            bitLeftmost = accum.getLastDiscardedDigit();
            bigexponent.Add(accum.getDiscardedDigitCount());
            fastSmallMant = accum.getShiftedIntFast();
            
            if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 || !fastSmallMant.isEvenNumber())) {
                fastSmallMant.AddInt(1);
                if (fastSmallMant.CompareToInt(1 << 24) == 0) {
                    fastSmallMant = new FastInteger(1 << 23);
                    bigexponent.AddInt(1);
                }
            }
        }
        if (bigexponent.CompareToInt(-149) < 0) {
            
            return (this.mantissa.signum() < 0) ? Float.intBitsToFloat(1 << 31) : Float.intBitsToFloat(0);
        } else {
            var smallexponent = bigexponent.AsInt32();
            smallexponent = smallexponent + 150;
            var smallmantissa = ((fastSmallMant.AsInt32())|0) & 8388607;
            if (!subnormal) {
                smallmantissa |= (smallexponent << 23);
            }
            if (this.mantissa.signum() < 0) smallmantissa |= (1 << 31);
            return Float.intBitsToFloat(smallmantissa);
        }
    };
    
    prototype.ToDouble = function() {
        var bigmant = (this.mantissa).abs();
        var bigexponent = FastInteger.FromBig(this.exponent);
        var bitLeftmost = 0;
        var bitsAfterLeftmost = 0;
        if (this.mantissa.signum() == 0) {
            return 0.0;
        }
        var mantissaBits;
        if (bigmant.compareTo(BigFloat.OneShift52) < 0) {
            mantissaBits = FastInteger.GetLastWords(bigmant, 2);
            
            while (!DecimalFraction.HasBitSet(mantissaBits, 52)) {
                DecimalFraction.ShiftLeftOne(mantissaBits);
                bigexponent.SubtractInt(1);
            }
        } else {
            var accum = new BitShiftAccumulator(bigmant, 0, 0);
            accum.ShiftToDigitsInt(53);
            bitsAfterLeftmost = accum.getOlderDiscardedDigits();
            bitLeftmost = accum.getLastDiscardedDigit();
            bigexponent.Add(accum.getDiscardedDigitCount());
            mantissaBits = FastInteger.GetLastWords(accum.getShiftedInt(), 2);
        }
        
        if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 || !DecimalFraction.HasBitSet(mantissaBits, 0))) {
            
            mantissaBits[0] = ((mantissaBits[0] + 1)|0);
            if (mantissaBits[0] == 0) mantissaBits[1] = ((mantissaBits[1] + 1)|0);
            if (mantissaBits[0] == 0 && mantissaBits[1] == (1 << 21)) {
                
                mantissaBits[1] = mantissaBits[1] >> 1;
                
                bigexponent.AddInt(1);
            }
        }
        var subnormal = false;
        if (bigexponent.CompareToInt(971) > 0) {
            
            return (this.mantissa.signum() < 0) ? Number.NEGATIVE_INFINITY : Number.POSITIVE_INFINITY;
        } else if (bigexponent.CompareToInt(-1074) < 0) {
            
            subnormal = true;
            
            var accum = new BitShiftAccumulator(FastInteger.WordsToBigInteger(mantissaBits), 0, 0);
            var fi = FastInteger.Copy(bigexponent).SubtractInt(-1074).Abs();
            accum.ShiftRight(fi);
            bitsAfterLeftmost = accum.getOlderDiscardedDigits();
            bitLeftmost = accum.getLastDiscardedDigit();
            bigexponent.Add(accum.getDiscardedDigitCount());
            mantissaBits = FastInteger.GetLastWords(accum.getShiftedInt(), 2);
            
            if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 || !DecimalFraction.HasBitSet(mantissaBits, 0))) {
                
                mantissaBits[0] = ((mantissaBits[0] + 1)|0);
                if (mantissaBits[0] == 0) mantissaBits[1] = ((mantissaBits[1] + 1)|0);
                if (mantissaBits[0] == 0 && mantissaBits[1] == (1 << 21)) {
                    
                    mantissaBits[1] = mantissaBits[1] >> 1;
                    
                    bigexponent.AddInt(1);
                }
            }
        }
        if (bigexponent.CompareToInt(-1074) < 0) {
            
            return (this.mantissa.signum() < 0) ? Extras.IntegersToDouble([0, ((-2147483648)|0)]) : 0.0;
        } else {
            bigexponent.AddInt(1075);
            
            mantissaBits[1] = mantissaBits[1] & 1048575;
            if (!subnormal) {
                var smallexponent = bigexponent.AsInt32() << 20;
                mantissaBits[1] = mantissaBits[1] | smallexponent;
            }
            if (this.mantissa.signum() < 0) {
                mantissaBits[1] = mantissaBits[1] | ((1 << 31)|0);
            }
            return Extras.IntegersToDouble(mantissaBits);
        }
    };
    
    constructor.BinaryMathHelper = function BigFloat$BinaryMathHelper(){};
    (function(constructor,prototype){
        
        prototype.GetRadix = function() {
            return 2;
        };
        
        prototype.GetSign = function(value) {
            return value.signum();
        };
        
        prototype.GetMantissa = function(value) {
            return value.mantissa;
        };
        
        prototype.GetExponent = function(value) {
            return value.exponent;
        };
        
        prototype.RescaleByExponentDiff = function(mantissa, e1, e2) {
            var negative = (mantissa.signum() < 0);
            if (negative) mantissa = mantissa.negate();
            var diff = (e1.subtract(e2)).abs();
            mantissa = BigFloat.ShiftLeft(mantissa, diff);
            if (negative) mantissa = mantissa.negate();
            return mantissa;
        };
        
        prototype.CreateNew = function(mantissa, exponent) {
            return new BigFloat(mantissa, exponent);
        };
        
        prototype.CreateShiftAccumulatorWithDigits = function(bigint, lastDigit, olderDigits) {
            return new BitShiftAccumulator(bigint, lastDigit, olderDigits);
        };
        
        prototype.CreateShiftAccumulator = function(bigint) {
            return new BitShiftAccumulator(bigint, 0, 0);
        };
        
        prototype.HasTerminatingRadixExpansion = function(num, den) {
            var gcd = num.gcd(den);
            if (gcd.signum() == 0) return false;
            den = den.divide(gcd);
            while (den.testBit(0) == false) {
                den = den.shiftRight(1);
            }
            return den.equals(BigInteger.ONE);
        };
        
        prototype.MultiplyByRadixPower = function(bigint, power) {
            if (power.signum() <= 0) return bigint;
            if (power.CanFitInInt32()) {
                return BigFloat.ShiftLeftInt(bigint, power.AsInt32());
            } else {
                return BigFloat.ShiftLeft(bigint, power.AsBigInteger());
            }
        };
    })(BigFloat.BinaryMathHelper,BigFloat.BinaryMathHelper.prototype);

    
    prototype.signum = function() {
        return this.mantissa.signum();
    };
    
    prototype.isZero = function() {
        return this.mantissa.signum() == 0;
    };
    
    prototype.Abs = function() {
        if (this.signum() < 0) {
            return this.Negate();
        } else {
            return this;
        }
    };
    
    prototype.Negate = function() {
        var neg = (this.mantissa).negate();
        return new BigFloat(neg, this.exponent);
    };
    
    prototype.DivideToSameExponent = function(divisor, rounding) {
        return this.DivideToExponent(divisor, this.exponent, PrecisionContext.ForRounding(rounding));
    };
    
    prototype.RemainderNaturalScale = function(divisor, ctx) {
        return this.Subtract(this.DivideToIntegerNaturalScale(divisor, null).Multiply(divisor, null), ctx);
    };
    
    prototype.Subtract = function(decfrac, ctx) {
        if ((decfrac) == null) throw ("decfrac");
        return this.Add(decfrac.Negate(), ctx);
    };
    constructor.math = new RadixMath(new BigFloat.BinaryMathHelper());
    
    
    prototype.DivideToIntegerNaturalScale = function(divisor, ctx) {
        return BigFloat.math.DivideToIntegerNaturalScale(this, divisor, ctx);
    };
    
    prototype.DivideToIntegerZeroScale = function(divisor, ctx) {
        return BigFloat.math.DivideToIntegerZeroScale(this, divisor, ctx);
    };
    
    prototype.Remainder = function(divisor, ctx) {
        return BigFloat.math.Remainder(this, divisor, ctx);
    };
    
    prototype.RemainderNear = function(divisor, ctx) {
        return BigFloat.math.RemainderNear(this, divisor, ctx);
    };
    
    prototype.Divide = function(divisor, ctx) {
        return BigFloat.math.Divide(this, divisor, ctx);
    };
    
    prototype.DivideToExponent = function(divisor, exponent, ctx) {
        return BigFloat.math.DivideToExponent(this, divisor, exponent, ctx);
    };
    
    constructor.Max = function(first, second) {
        return BigFloat.math.Max(first, second);
    };
    
    constructor.Min = function(first, second) {
        return BigFloat.math.Min(first, second);
    };
    
    constructor.MaxMagnitude = function(first, second) {
        return BigFloat.math.MaxMagnitude(first, second);
    };
    
    constructor.MinMagnitude = function(first, second) {
        return BigFloat.math.MinMagnitude(first, second);
    };
    
    prototype.compareTo = function(other) {
        return BigFloat.math.compareTo(this, other);
    };
    
    prototype.Add = function(decfrac, ctx) {
        return BigFloat.math.Add(this, decfrac, ctx);
    };
    
    prototype.Quantize = function(otherValue, ctx) {
        return BigFloat.math.Quantize(this, otherValue, ctx);
    };
    
    prototype.RoundToIntegralExact = function(ctx) {
        return BigFloat.math.RoundToExponentExact(this, BigInteger.ZERO, ctx);
    };
    
    prototype.RoundToIntegralNoRoundedFlag = function(ctx) {
        return BigFloat.math.RoundToExponentNoRoundedFlag(this, BigInteger.ZERO, ctx);
    };
    
    prototype.Reduce = function(ctx) {
        return BigFloat.math.Reduce(this, ctx);
    };
    
    prototype.NextMinus = function(ctx) {
        return BigFloat.math.NextMinus(this, ctx);
    };
    
    prototype.NextPlus = function(ctx) {
        return BigFloat.math.NextPlus(this, ctx);
    };
    
    prototype.NextToward = function(otherValue, ctx) {
        return BigFloat.math.NextToward(this, otherValue, ctx);
    };
    
    prototype.Multiply = function(op, ctx) {
        return BigFloat.math.Multiply(this, op, ctx);
    };
    
    prototype.MultiplyAndAdd = function(multiplicand, augend, ctx) {
        return BigFloat.math.MultiplyAndAdd(this, multiplicand, augend, ctx);
    };
    
    prototype.RoundToPrecision = function(ctx) {
        return BigFloat.math.RoundToPrecision(this, ctx);
    };
    
    prototype.RoundToBinaryPrecision = function(ctx) {
        return BigFloat.math.RoundToBinaryPrecision(this, ctx);
    };
    constructor.One = new BigFloat(BigInteger.ONE, BigInteger.ZERO);
    constructor.Zero = new BigFloat(BigInteger.ZERO, BigInteger.ZERO);
    constructor.Ten = BigFloat.FromInt64(JSInteropFactory.createLong(10));
})(BigFloat,BigFloat.prototype);


if(typeof exports!=="undefined")exports.BigFloat=BigFloat;