
var DecimalFraction = 

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
        var otherValue = ((other.constructor==DecimalFraction) ? other : null);
        if (otherValue == null) return false;
        return this.exponent.equals(otherValue.exponent) && this.mantissa.equals(otherValue.mantissa);
    };
    prototype.equals = function(obj) {
        return this.EqualsInternal((obj.constructor==DecimalFraction) ? obj : null);
    };
    prototype.hashCode = function() {
        var hashCode_ = 0;
        {
            hashCode_ += 1000000007 * this.exponent.hashCode();
            hashCode_ += 1000000009 * this.mantissa.hashCode();
        }
        return hashCode_;
    };
    
    constructor.FromString = function(str) {
        if (str == null) throw ("str");
        if (str.length == 0) throw "exception";
        var offset = 0;
        var negative = false;
        if (str.charAt(0) == '+' || str.charAt(0) == '-') {
            negative = (str.charAt(0) == '-');
            offset++;
        }
        var mant = new FastInteger(0);
        var haveDecimalPoint = false;
        var haveDigits = false;
        var haveExponent = false;
        var newScale = new FastInteger(0);
        var i = offset;
        for (; i < str.length; i++) {
            if (str.charAt(i) >= '0' && str.charAt(i) <= '9') {
                var thisdigit = ((str.charAt(i) - '0')|0);
                mant.Multiply(10);
                mant.AddInt(thisdigit);
                haveDigits = true;
                if (haveDecimalPoint) {
                    newScale.AddInt(-1);
                }
            } else if (str.charAt(i) == '.') {
                if (haveDecimalPoint) throw "exception";
                haveDecimalPoint = true;
            } else if (str.charAt(i) == 'E' || str.charAt(i) == 'e') {
                haveExponent = true;
                i++;
                break;
            } else {
                throw "exception";
            }
        }
        if (!haveDigits) throw "exception";
        if (haveExponent) {
            var exp = new FastInteger(0);
            offset = 1;
            haveDigits = false;
            if (i == str.length) throw "exception";
            if (str.charAt(i) == '+' || str.charAt(i) == '-') {
                if (str.charAt(i) == '-') offset = -1;
                i++;
            }
            for (; i < str.length; i++) {
                if (str.charAt(i) >= '0' && str.charAt(i) <= '9') {
                    haveDigits = true;
                    var thisdigit = ((str.charAt(i) - '0')|0);
                    exp.Multiply(10);
                    exp.AddInt(thisdigit);
                } else {
                    throw "exception";
                }
            }
            if (!haveDigits) throw "exception";
            if (offset < 0) newScale.Subtract(exp); else newScale.Add(exp);
        } else if (i != str.length) {
            throw "exception";
        }
        return new DecimalFraction((negative) ? mant.Negate().AsBigInteger() : mant.AsBigInteger(), newScale.AsBigInteger());
    };
    constructor.ShiftLeftOne = function(arr) {
        {
            var carry = 0;
            for (var i = 0; i < arr.length; i++) {
                var item = arr[i];
                arr[i] = ((arr[i] << 1)|0) | (carry|0);
                carry = ((item >> 31) != 0) ? 1 : 0;
            }
            return carry;
        }
    };
    constructor.CountTrailingZeros = function(numberValue) {
        if (numberValue == 0) return 32;
        var i = 0;
        {
            if ((numberValue << 16) == 0) {
                numberValue >>= 16;
                i += 16;
            }
            if ((numberValue << 24) == 0) {
                numberValue >>= 8;
                i += 8;
            }
            if ((numberValue << 28) == 0) {
                numberValue >>= 4;
                i += 4;
            }
            if ((numberValue << 30) == 0) {
                numberValue >>= 2;
                i += 2;
            }
            if ((numberValue << 31) == 0) ++i;
        }
        return i;
    };
    constructor.BitPrecisionInt = function(numberValue) {
        if (numberValue == 0) return 0;
        var i = 32;
        {
            if ((numberValue >> 16) == 0) {
                numberValue <<= 16;
                i -= 8;
            }
            if ((numberValue >> 24) == 0) {
                numberValue <<= 8;
                i -= 8;
            }
            if ((numberValue >> 28) == 0) {
                numberValue <<= 4;
                i -= 4;
            }
            if ((numberValue >> 30) == 0) {
                numberValue <<= 2;
                i -= 2;
            }
            if ((numberValue >> 31) == 0) --i;
        }
        return i;
    };
    constructor.ShiftAwayTrailingZerosTwoElements = function(arr) {
        var a0 = arr[0];
        var a1 = arr[1];
        var tz = DecimalFraction.CountTrailingZeros(a0);
        if (tz == 0) return 0;
        {
            if (tz < 32) {
                var carry = a1 << (32 - tz);
                arr[0] = (((a0 >> tz) & (2147483647 >> (tz - 1)))|0) | (carry|0);
                arr[1] = ((a1 >> tz) & (2147483647 >> (tz - 1)));
                return tz;
            } else {
                tz = DecimalFraction.CountTrailingZeros(a1);
                if (tz == 32) {
                    arr[0] = 0;
                } else if (tz > 0) {
                    arr[0] = ((a1 >> tz) & (2147483647 >> (tz - 1)));
                } else {
                    arr[0] = a1;
                }
                arr[1] = 0;
                return 32 + tz;
            }
        }
    };
    constructor.HasBitSet = function(arr, bit) {
        return ((bit >> 5) < arr.length && (arr[bit >> 5] & (1 << (bit & 31))) != 0);
    };
    constructor.FindPowerOfFiveFromBig = function(diff) {
        if (diff.signum() <= 0) return BigInteger.ONE;
        var bigpow = BigInteger.ZERO;
        var intcurexp = FastInteger.FromBig(diff);
        if (intcurexp.CompareToInt(54) <= 0) {
            return DecimalFraction.FindPowerOfFive(intcurexp.AsInt32());
        }
        var mantissa = BigInteger.ONE;
        while (intcurexp.signum() > 0) {
            if (intcurexp.CompareToInt(27) <= 0) {
                bigpow = DecimalFraction.FindPowerOfFive(intcurexp.AsInt32());
                mantissa = mantissa.multiply(bigpow);
                break;
            } else if (intcurexp.CompareToInt(9999999) <= 0) {
                bigpow = (DecimalFraction.FindPowerOfFive(1)).pow(intcurexp.AsInt32());
                mantissa = mantissa.multiply(bigpow);
                break;
            } else {
                if (bigpow.signum() == 0) bigpow = (DecimalFraction.FindPowerOfFive(1)).pow(9999999);
                mantissa = mantissa.multiply(bigpow);
                intcurexp.AddInt(-9999999);
            }
        }
        return mantissa;
    };
    constructor.BigInt36 = BigInteger.valueOf(36);
    constructor.FindPowerOfTenFromBig = function(bigintExponent) {
        if (bigintExponent.signum() <= 0) return BigInteger.ONE;
        if (bigintExponent.compareTo(DecimalFraction.BigInt36) <= 0) {
            return DecimalFraction.FindPowerOfTen(bigintExponent.intValue());
        }
        var intcurexp = FastInteger.FromBig(bigintExponent);
        var mantissa = BigInteger.ONE;
        var bigpow = BigInteger.ZERO;
        while (intcurexp.signum() > 0) {
            if (intcurexp.CompareToInt(18) <= 0) {
                bigpow = DecimalFraction.FindPowerOfTen(intcurexp.AsInt32());
                mantissa = mantissa.multiply(bigpow);
                break;
            } else if (intcurexp.CompareToInt(9999999) <= 0) {
                var val = intcurexp.AsInt32();
                bigpow = DecimalFraction.FindPowerOfFive(val);
                bigpow = bigpow.shiftLeft(val);
                mantissa = mantissa.multiply(bigpow);
                break;
            } else {
                if (bigpow.signum() == 0) {
                    bigpow = DecimalFraction.FindPowerOfFive(9999999);
                    bigpow = bigpow.shiftLeft(9999999);
                }
                mantissa = mantissa.multiply(bigpow);
                intcurexp.AddInt(-9999999);
            }
        }
        return mantissa;
    };
    constructor.FivePower40 = (BigInteger.valueOf(JSInteropFactory.createLongFromInts(1977800241, 22204))).multiply(BigInteger.valueOf(JSInteropFactory.createLongFromInts(1977800241, 22204)));
    constructor.FindPowerOfFive = function(precision) {
        if (precision <= 0) return BigInteger.ONE;
        var bigpow;
        var ret;
        if (precision <= 27) return DecimalFraction.BigIntPowersOfFive[(precision|0)];
        if (precision == 40) return DecimalFraction.FivePower40;
        if (precision <= 54) {
            if ((precision & 1) == 0) {
                ret = DecimalFraction.BigIntPowersOfFive[((precision >> 1)|0)];
                ret = ret.multiply(ret);
                return ret;
            } else {
                ret = DecimalFraction.BigIntPowersOfFive[27];
                bigpow = DecimalFraction.BigIntPowersOfFive[(precision|0) - 27];
                ret = ret.multiply(bigpow);
                return ret;
            }
        }
        if (precision > 40 && precision <= 94) {
            ret = DecimalFraction.FivePower40;
            bigpow = DecimalFraction.FindPowerOfFive(precision - 40);
            ret = ret.multiply(bigpow);
            return ret;
        }
        ret = BigInteger.ONE;
        var first = true;
        bigpow = BigInteger.ZERO;
        while (precision > 0) {
            if (precision <= 27) {
                bigpow = DecimalFraction.BigIntPowersOfFive[(precision|0)];
                if (first) ret = bigpow; else ret = ret.multiply(bigpow);
                first = false;
                break;
            } else if (precision <= 9999999) {
                bigpow = (DecimalFraction.BigIntPowersOfFive[1]).pow(precision|0);
                if (first) ret = bigpow; else ret = ret.multiply(bigpow);
                first = false;
                break;
            } else {
                if (bigpow.signum() == 0) bigpow = (DecimalFraction.BigIntPowersOfFive[1]).pow(9999999);
                if (first) ret = bigpow; else ret = ret.multiply(bigpow);
                first = false;
                precision -= 9999999;
            }
        }
        return ret;
    };
    constructor.FindPowerOfTen = function(precision) {
        if (precision <= 0) return BigInteger.ONE;
        var ret;
        var bigpow;
        if (precision <= 18) return DecimalFraction.BigIntPowersOfTen[(precision|0)];
        if (precision <= 27) {
            var prec = (precision|0);
            ret = DecimalFraction.BigIntPowersOfFive[prec];
            ret = ret.shiftLeft(prec);
            return ret;
        }
        if (precision <= 36) {
            if ((precision & 1) == 0) {
                ret = DecimalFraction.BigIntPowersOfTen[((precision >> 1)|0)];
                ret = ret.multiply(ret);
                return ret;
            } else {
                ret = DecimalFraction.BigIntPowersOfTen[18];
                bigpow = DecimalFraction.BigIntPowersOfTen[(precision|0) - 18];
                ret = ret.multiply(bigpow);
                return ret;
            }
        }
        ret = BigInteger.ONE;
        var first = true;
        bigpow = BigInteger.ZERO;
        while (precision > 0) {
            if (precision <= 18) {
                bigpow = DecimalFraction.BigIntPowersOfTen[(precision|0)];
                if (first) ret = bigpow; else ret = ret.multiply(bigpow);
                first = false;
                break;
            } else if (precision <= 9999999) {
                var prec = (precision|0);
                bigpow = DecimalFraction.FindPowerOfFive(prec);
                bigpow = bigpow.shiftLeft(prec);
                if (first) ret = bigpow; else ret = ret.multiply(bigpow);
                first = false;
                break;
            } else {
                if (bigpow.signum() == 0) bigpow = (DecimalFraction.BigIntPowersOfTen[1]).pow(9999999);
                if (first) ret = bigpow; else ret = ret.multiply(bigpow);
                first = false;
                precision -= 9999999;
            }
        }
        return ret;
    };
    constructor.DecimalMathHelper = function DecimalFraction$DecimalMathHelper(){};
    (function(constructor,prototype){
        
        prototype.GetRadix = function() {
            return 10;
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
            if (mantissa.signum() == 0) return BigInteger.ZERO;
            if (negative) mantissa = mantissa.negate();
            var diff = FastInteger.FromBig(e1).SubtractBig(e2).Abs();
            if (diff.CanFitInInt32()) {
                mantissa = mantissa.multiply(DecimalFraction.FindPowerOfTen(diff.AsInt32()));
            } else {
                mantissa = mantissa.multiply(DecimalFraction.FindPowerOfTenFromBig(diff.AsBigInteger()));
            }
            if (negative) mantissa = mantissa.negate();
            return mantissa;
        };
        
        prototype.CreateNew = function(mantissa, exponent) {
            return new DecimalFraction(mantissa, exponent);
        };
        
        prototype.CreateShiftAccumulatorWithDigits = function(bigint, lastDigit, olderDigits) {
            return new DigitShiftAccumulator(bigint, lastDigit, olderDigits);
        };
        
        prototype.CreateShiftAccumulator = function(bigint) {
            return new DigitShiftAccumulator(bigint, 0, 0);
        };
        
        prototype.HasTerminatingRadixExpansion = function(numerator, denominator) {
            
            var gcd = numerator.gcd(denominator);
            denominator = denominator.divide(gcd);
            if (denominator.signum() == 0) return false;
            
            while (denominator.testBit(0) == false) {
                denominator = denominator.shiftRight(1);
            }
            
            while (true) {
                var bigrem;
                var bigquo;
                var divrem = (denominator).divideAndRemainder(BigInteger.valueOf(5));
                bigquo = divrem[0];
                bigrem = divrem[1];
                if (bigrem.signum() != 0) break;
                denominator = bigquo;
            }
            return denominator.compareTo(BigInteger.ONE) == 0;
        };
        
        prototype.MultiplyByRadixPower = function(bigint, power) {
            if (power.signum() <= 0) return bigint;
            if (bigint.signum() == 0) return bigint;
            if (bigint.compareTo(BigInteger.ONE) != 0) {
                if (power.CanFitInInt32()) {
                    bigint = bigint.multiply(DecimalFraction.FindPowerOfTen(power.AsInt32()));
                } else {
                    bigint = bigint.multiply(DecimalFraction.FindPowerOfTenFromBig(power.AsBigInteger()));
                }
            } else {
                if (power.CanFitInInt32()) {
                    bigint = DecimalFraction.FindPowerOfTen(power.AsInt32());
                } else {
                    bigint = DecimalFraction.FindPowerOfTenFromBig(power.AsBigInteger());
                }
            }
            return bigint;
        };
    })(DecimalFraction.DecimalMathHelper,DecimalFraction.DecimalMathHelper.prototype);

    constructor.AppendString = function(builder, c, count) {
        if (count.CompareToInt(2147483647) > 0 || count.signum() < 0) {
            throw "exception";
        }
        var icount = count.AsInt32();
        for (var i = icount - 1; i >= 0; i--) {
            builder.append(c);
        }
        return true;
    };
    prototype.ToStringInternal = function(mode) {
        
        var mantissaString = this.mantissa.toString();
        var scaleSign = -this.exponent.signum();
        if (scaleSign == 0) return mantissaString;
        var iszero = (this.mantissa.signum() == 0);
        if (mode == 2 && iszero && scaleSign < 0) {
            
            return mantissaString;
        }
        var sbLength = new FastInteger(mantissaString.length);
        var negaPos = 0;
        if (mantissaString.charAt(0) == '-') {
            sbLength.AddInt(-1);
            negaPos = 1;
        }
        var adjustedExponent = FastInteger.FromBig(this.exponent);
        var thisExponent = FastInteger.Copy(adjustedExponent);
        adjustedExponent.Add(sbLength).AddInt(-1);
        var decimalPointAdjust = new FastInteger(1);
        var threshold = new FastInteger(-6);
        if (mode == 1) {
            
            var newExponent = FastInteger.Copy(adjustedExponent);
            var adjExponentNegative = (adjustedExponent.signum() < 0);
            var intphase = FastInteger.Copy(adjustedExponent).Abs().Mod(3).AsInt32();
            if (iszero && (adjustedExponent.compareTo(threshold) < 0 || scaleSign < 0)) {
                if (intphase == 1) {
                    if (adjExponentNegative) {
                        decimalPointAdjust.AddInt(1);
                        newExponent.AddInt(1);
                    } else {
                        decimalPointAdjust.AddInt(2);
                        newExponent.AddInt(2);
                    }
                } else if (intphase == 2) {
                    if (!adjExponentNegative) {
                        decimalPointAdjust.AddInt(1);
                        newExponent.AddInt(1);
                    } else {
                        decimalPointAdjust.AddInt(2);
                        newExponent.AddInt(2);
                    }
                }
                threshold.AddInt(1);
            } else {
                if (intphase == 1) {
                    if (!adjExponentNegative) {
                        decimalPointAdjust.AddInt(1);
                        newExponent.AddInt(-1);
                    } else {
                        decimalPointAdjust.AddInt(2);
                        newExponent.AddInt(-2);
                    }
                } else if (intphase == 2) {
                    if (adjExponentNegative) {
                        decimalPointAdjust.AddInt(1);
                        newExponent.AddInt(-1);
                    } else {
                        decimalPointAdjust.AddInt(2);
                        newExponent.AddInt(-2);
                    }
                }
            }
            adjustedExponent = newExponent;
        }
        if (mode == 2 || (adjustedExponent.compareTo(threshold) >= 0 && scaleSign >= 0)) {
            if (scaleSign > 0) {
                var decimalPoint = FastInteger.Copy(thisExponent).AddInt(negaPos).Add(sbLength);
                var cmp = decimalPoint.CompareToInt(negaPos);
                var builder = null;
                if (cmp < 0) {
                    var tmpFast = new FastInteger(mantissaString.length).AddInt(6);
                    builder = JSInteropFactory.createStringBuilder(tmpFast.CompareToInt(2147483647) > 0 ? 2147483647 : tmpFast.AsInt32());
                    for (var arrfillI = 0; arrfillI < (0) + (negaPos); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                    builder.append("0.");
                    DecimalFraction.AppendString(builder, '0', new FastInteger(negaPos).Subtract(decimalPoint));
                    for (var arrfillI = negaPos; arrfillI < (negaPos) + (mantissaString.length - negaPos); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                } else if (cmp == 0) {
                    if (!decimalPoint.CanFitInInt32()) throw "exception";
                    var tmpInt = decimalPoint.AsInt32();
                    if (tmpInt < 0) tmpInt = 0;
                    var tmpFast = new FastInteger(mantissaString.length).AddInt(6);
                    builder = JSInteropFactory.createStringBuilder(tmpFast.CompareToInt(2147483647) > 0 ? 2147483647 : tmpFast.AsInt32());
                    for (var arrfillI = 0; arrfillI < (0) + (tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                    builder.append("0.");
                    for (var arrfillI = tmpInt; arrfillI < (tmpInt) + (mantissaString.length - tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                } else if (decimalPoint.compareTo(new FastInteger(negaPos).AddInt(mantissaString.length)) > 0) {
                    var insertionPoint = new FastInteger(negaPos).Add(sbLength);
                    if (!insertionPoint.CanFitInInt32()) throw "exception";
                    var tmpInt = insertionPoint.AsInt32();
                    if (tmpInt < 0) tmpInt = 0;
                    var tmpFast = new FastInteger(mantissaString.length).AddInt(6);
                    builder = JSInteropFactory.createStringBuilder(tmpFast.CompareToInt(2147483647) > 0 ? 2147483647 : tmpFast.AsInt32());
                    for (var arrfillI = 0; arrfillI < (0) + (tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                    DecimalFraction.AppendString(builder, '0', FastInteger.Copy(decimalPoint).SubtractInt(builder.length));
                    builder.append('.');
                    for (var arrfillI = tmpInt; arrfillI < (tmpInt) + (mantissaString.length - tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                } else {
                    if (!decimalPoint.CanFitInInt32()) throw "exception";
                    var tmpInt = decimalPoint.AsInt32();
                    if (tmpInt < 0) tmpInt = 0;
                    var tmpFast = new FastInteger(mantissaString.length).AddInt(6);
                    builder = JSInteropFactory.createStringBuilder(tmpFast.CompareToInt(2147483647) > 0 ? 2147483647 : tmpFast.AsInt32());
                    for (var arrfillI = 0; arrfillI < (0) + (tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                    builder.append('.');
                    for (var arrfillI = tmpInt; arrfillI < (tmpInt) + (mantissaString.length - tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                }
                return builder.toString();
            } else if (mode == 2 && scaleSign < 0) {
                var negscale = FastInteger.Copy(thisExponent);
                var builder = JSInteropFactory.createStringBuilder(16);
                builder.append(mantissaString);
                DecimalFraction.AppendString(builder, '0', negscale);
                return builder.toString();
            } else {
                return mantissaString;
            }
        } else {
            var builder = null;
            if (mode == 1 && iszero && decimalPointAdjust.CompareToInt(1) > 0) {
                builder = JSInteropFactory.createStringBuilder(16);
                builder.append(mantissaString);
                builder.append('.');
                DecimalFraction.AppendString(builder, '0', FastInteger.Copy(decimalPointAdjust).AddInt(-1));
            } else {
                var tmp = new FastInteger(negaPos).Add(decimalPointAdjust);
                var cmp = tmp.CompareToInt(mantissaString.length);
                if (cmp > 0) {
                    tmp.SubtractInt(mantissaString.length);
                    builder = JSInteropFactory.createStringBuilder(16);
                    builder.append(mantissaString);
                    DecimalFraction.AppendString(builder, '0', tmp);
                } else if (cmp < 0) {
                    
                    if (!tmp.CanFitInInt32()) throw "exception";
                    var tmpInt = tmp.AsInt32();
                    if (tmp.signum() < 0) tmpInt = 0;
                    var tmpFast = new FastInteger(mantissaString.length).AddInt(6);
                    builder = JSInteropFactory.createStringBuilder(tmpFast.CompareToInt(2147483647) > 0 ? 2147483647 : tmpFast.AsInt32());
                    for (var arrfillI = 0; arrfillI < (0) + (tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                    builder.append('.');
                    for (var arrfillI = tmpInt; arrfillI < (tmpInt) + (mantissaString.length - tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                } else if (adjustedExponent.signum() == 0) {
                    return mantissaString;
                } else {
                    builder = JSInteropFactory.createStringBuilder(16);
                    builder.append(mantissaString);
                }
            }
            if (adjustedExponent.signum() != 0) {
                builder.append(adjustedExponent.signum() < 0 ? "E-" : "E+");
                adjustedExponent.Abs();
                var builderReversed = JSInteropFactory.createStringBuilder(16);
                while (adjustedExponent.signum() != 0) {
                    var digit = FastInteger.Copy(adjustedExponent).Mod(10).AsInt32();
                    
                    builderReversed.append('0' + digit);
                    adjustedExponent.Divide(10);
                }
                var count = builderReversed.length;
                for (var i = 0; i < count; i++) {
                    builder.append(builderReversed.charAt(count - 1 - i));
                }
            }
            return builder.toString();
        }
    };
    constructor.BigIntPowersOfTen = [BigInteger.ONE, BigInteger.TEN, BigInteger.valueOf(100), BigInteger.valueOf(1000), BigInteger.valueOf(10000), BigInteger.valueOf(100000), BigInteger.valueOf(1000000), BigInteger.valueOf(10000000), BigInteger.valueOf(100000000), BigInteger.valueOf(1000000000), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1410065408, 2)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1215752192, 23)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-727379968, 232)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1316134912, 2328)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(276447232, 23283)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-1530494976, 232830)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1874919424, 2328306)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1569325056, 23283064)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-1486618624, 232830643))];
    constructor.BigIntPowersOfFive = [BigInteger.ONE, BigInteger.valueOf(5), BigInteger.valueOf(25), BigInteger.valueOf(125), BigInteger.valueOf(625), BigInteger.valueOf(3125), BigInteger.valueOf(15625), BigInteger.valueOf(78125), BigInteger.valueOf(390625), BigInteger.valueOf(1953125), BigInteger.valueOf(9765625), BigInteger.valueOf(48828125), BigInteger.valueOf(244140625), BigInteger.valueOf(1220703125), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1808548329, 1)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(452807053, 7)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-2030932031, 35)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-1564725563, 177)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(766306777, 888)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-463433411, 4440)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1977800241, 22204)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1299066613, 111022)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-2094601527, 555111)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-1883073043, 2775557)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-825430623, 13877787)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(167814181, 69388939)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(839070905, 346944695)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-99612771, 1734723475))];
    
    prototype.ToBigInteger = function() {
        var sign = this.getExponent().signum();
        if (sign == 0) {
            return this.getMantissa();
        } else if (sign > 0) {
            var bigmantissa = this.getMantissa();
            bigmantissa = bigmantissa.multiply(DecimalFraction.FindPowerOfTenFromBig(this.getExponent()));
            return bigmantissa;
        } else {
            var bigmantissa = this.getMantissa();
            var bigexponent = this.getExponent();
            bigexponent = bigexponent.negate();
            bigmantissa = bigmantissa.divide(DecimalFraction.FindPowerOfTenFromBig(bigexponent));
            return bigmantissa;
        }
    };
    
    constructor.FromSingle = function(flt) {
        var value = Float.floatToRawIntBits(flt);
        var fpExponent = (((value >> 23) & 255)|0);
        if (fpExponent == 255) throw ("Value is infinity or NaN");
        var fpMantissa = value & 8388607;
        if (fpExponent == 0) fpExponent++; else fpMantissa |= (1 << 23);
        if (fpMantissa == 0) return DecimalFraction.Zero;
        fpExponent -= 150;
        while ((fpMantissa & 1) == 0) {
            fpExponent++;
            fpMantissa >>= 1;
        }
        var neg = ((value >> 31) != 0);
        if (fpExponent == 0) {
            if (neg) fpMantissa = -fpMantissa;
            return DecimalFraction.FromInt64(fpMantissa);
        } else if (fpExponent > 0) {
            
            var bigmantissa = BigInteger.valueOf(fpMantissa);
            bigmantissa = bigmantissa.shiftLeft(fpExponent);
            if (neg) bigmantissa = (bigmantissa).negate();
            return DecimalFraction.FromBigInteger(bigmantissa);
        } else {
            
            var bigmantissa = BigInteger.valueOf(fpMantissa);
            bigmantissa = bigmantissa.multiply(DecimalFraction.FindPowerOfFive(-fpExponent));
            if (neg) bigmantissa = (bigmantissa).negate();
            return new DecimalFraction(bigmantissa, BigInteger.valueOf(fpExponent));
        }
    };
    constructor.FromBigInteger = function(bigint) {
        return new DecimalFraction(bigint, BigInteger.ZERO);
    };
    constructor.FromInt64 = function(valueSmall_obj) {
        var valueSmall = JSInteropFactory.createLong(valueSmall_obj);
        var bigint = BigInteger.valueOf(valueSmall);
        return new DecimalFraction(bigint, BigInteger.ZERO);
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
        fpExponent -= 1075;
        var fpMantissaBig = FastInteger.WordsToBigInteger(value);
        if (fpExponent == 0) {
            if (neg) fpMantissaBig = fpMantissaBig.negate();
            return DecimalFraction.FromBigInteger(fpMantissaBig);
        } else if (fpExponent > 0) {
            
            var bigmantissa = fpMantissaBig;
            bigmantissa = bigmantissa.shiftLeft(fpExponent);
            if (neg) bigmantissa = (bigmantissa).negate();
            return DecimalFraction.FromBigInteger(bigmantissa);
        } else {
            
            var bigmantissa = fpMantissaBig;
            bigmantissa = bigmantissa.multiply(DecimalFraction.FindPowerOfFive(-fpExponent));
            if (neg) bigmantissa = (bigmantissa).negate();
            return new DecimalFraction(bigmantissa, BigInteger.valueOf(fpExponent));
        }
    };
    
    constructor.FromBigFloat = function(bigfloat) {
        if ((bigfloat) == null) throw ("bigfloat");
        var bigintExp = bigfloat.getExponent();
        var bigintMant = bigfloat.getMantissa();
        if (bigintExp.signum() == 0) {
            
            return DecimalFraction.FromBigInteger(bigintMant);
        } else if (bigintExp.signum() > 0) {
            
            var intcurexp = FastInteger.FromBig(bigintExp);
            var bigmantissa = bigintMant;
            var neg = (bigmantissa.signum() < 0);
            if (neg) bigmantissa = (bigmantissa).negate();
            while (intcurexp.signum() > 0) {
                var shift = 512;
                if (intcurexp.CompareToInt(512) < 0) {
                    shift = intcurexp.AsInt32();
                }
                bigmantissa = bigmantissa.shiftLeft(shift);
                intcurexp.AddInt(-shift);
            }
            if (neg) bigmantissa = (bigmantissa).negate();
            return DecimalFraction.FromBigInteger(bigmantissa);
        } else {
            
            var bigmantissa = bigintMant;
            var negbigintExp = (bigintExp).negate();
            bigmantissa = bigmantissa.multiply(DecimalFraction.FindPowerOfFiveFromBig(negbigintExp));
            return new DecimalFraction(bigmantissa, bigintExp);
        }
    };
    
    prototype.toString = function() {
        return this.ToStringInternal(0);
    };
    
    prototype.ToEngineeringString = function() {
        return this.ToStringInternal(1);
    };
    
    prototype.ToPlainString = function() {
        return this.ToStringInternal(2);
    };
    constructor.One = new DecimalFraction(BigInteger.ONE, BigInteger.ZERO);
    constructor.Zero = new DecimalFraction(BigInteger.ZERO, BigInteger.ZERO);
    constructor.Ten = new DecimalFraction(BigInteger.TEN, BigInteger.ZERO);
    
    
    
    
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
        return new DecimalFraction(neg, this.exponent);
    };
    
    prototype.DivideToSameExponent = function(divisor, rounding) {
        return this.DivideToExponent(divisor, this.exponent, PrecisionContext.ForRounding(rounding));
    };
    
    prototype.Reduce = function(ctx) {
        return DecimalFraction.math.Reduce(this, ctx);
    };
    
    prototype.RemainderNaturalScale = function(divisor, ctx) {
        return this.Subtract(this.DivideToIntegerNaturalScale(divisor, null).Multiply(divisor, null), ctx);
    };
    
    prototype.DivideToExponent = function(divisor, exponent, ctx) {
        return DecimalFraction.math.DivideToExponent(this, divisor, exponent, ctx);
    };
    
    prototype.Subtract = function(decfrac, ctx) {
        if ((decfrac) == null) throw ("decfrac");
        return this.Add(decfrac.Negate(), ctx);
    };
    constructor.math = new RadixMath(new DecimalFraction.DecimalMathHelper());
    
    
    prototype.DivideToIntegerNaturalScale = function(divisor, ctx) {
        return DecimalFraction.math.DivideToIntegerNaturalScale(this, divisor, ctx);
    };
    
    prototype.DivideToIntegerZeroScale = function(divisor, ctx) {
        return DecimalFraction.math.DivideToIntegerZeroScale(this, divisor, ctx);
    };
    
    prototype.Remainder = function(divisor, ctx) {
        return DecimalFraction.math.Remainder(this, divisor, ctx);
    };
    
    prototype.RemainderNear = function(divisor, ctx) {
        return DecimalFraction.math.RemainderNear(this, divisor, ctx);
    };
    
    prototype.NextMinus = function(ctx) {
        return DecimalFraction.math.NextMinus(this, ctx);
    };
    
    prototype.NextPlus = function(ctx) {
        return DecimalFraction.math.NextPlus(this, ctx);
    };
    
    prototype.NextToward = function(otherValue, ctx) {
        return DecimalFraction.math.NextToward(this, otherValue, ctx);
    };
    
    prototype.Divide = function(divisor, ctx) {
        return DecimalFraction.math.Divide(this, divisor, ctx);
    };
    
    constructor.Max = function(first, second) {
        return DecimalFraction.math.Max(first, second);
    };
    
    constructor.Min = function(first, second) {
        return DecimalFraction.math.Min(first, second);
    };
    
    constructor.MaxMagnitude = function(first, second) {
        return DecimalFraction.math.MaxMagnitude(first, second);
    };
    
    constructor.MinMagnitude = function(first, second) {
        return DecimalFraction.math.MinMagnitude(first, second);
    };
    
    prototype.compareTo = function(other) {
        return DecimalFraction.math.compareTo(this, other);
    };
    
    prototype.Add = function(decfrac, ctx) {
        return DecimalFraction.math.Add(this, decfrac, ctx);
    };
    
    prototype.Quantize = function(otherValue, ctx) {
        return DecimalFraction.math.Quantize(this, otherValue, ctx);
    };
    
    prototype.RoundToIntegralExact = function(ctx) {
        return DecimalFraction.math.RoundToExponentExact(this, BigInteger.ZERO, ctx);
    };
    
    prototype.RoundToIntegralNoRoundedFlag = function(ctx) {
        return DecimalFraction.math.RoundToExponentNoRoundedFlag(this, BigInteger.ZERO, ctx);
    };
    
    prototype.RoundToExponentExact = function(exponent, ctx) {
        return DecimalFraction.math.RoundToExponentExact(this, exponent, ctx);
    };
    
    prototype.RoundToExponent = function(exponent, ctx) {
        return DecimalFraction.math.RoundToExponentSimple(this, exponent, ctx);
    };
    
    prototype.Multiply = function(op, ctx) {
        return DecimalFraction.math.Multiply(this, op, ctx);
    };
    
    prototype.MultiplyAndAdd = function(op, augend, ctx) {
        return DecimalFraction.math.MultiplyAndAdd(this, op, augend, ctx);
    };
    
    prototype.RoundToPrecision = function(ctx) {
        return DecimalFraction.math.RoundToPrecision(this, ctx);
    };
    
    prototype.RoundToBinaryPrecision = function(ctx) {
        return DecimalFraction.math.RoundToBinaryPrecision(this, ctx);
    };
})(DecimalFraction,DecimalFraction.prototype);


if(typeof exports!=="undefined")exports.DecimalFraction=DecimalFraction;