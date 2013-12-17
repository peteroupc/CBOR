
var DigitShiftAccumulator = 

function(bigint, lastDiscarded, olderDiscarded) {

    if (bigint.signum() < 0) throw ("bigint is negative");
    this.discardedBitCount = new FastInteger(0);
    if (bigint.compareTo(DigitShiftAccumulator.Int32MaxValue) <= 0) {
        this.shiftedSmall = bigint.intValue();
        this.isSmall = true;
    } else {
        this.shiftedBigInt = bigint;
        this.isSmall = false;
    }
    this.bitsAfterLeftmost = (olderDiscarded != 0) ? 1 : 0;
    this.bitLeftmost = lastDiscarded;
};
(function(constructor,prototype){
    prototype.bitLeftmost = null;
    prototype.getLastDiscardedDigit = function() {
        return this.bitLeftmost;
    };
    prototype.bitsAfterLeftmost = null;
    prototype.getOlderDiscardedDigits = function() {
        return this.bitsAfterLeftmost;
    };
    prototype.shiftedBigInt = null;
    prototype.knownBitLength = null;
    constructor.SmallBitLength = 32;
    prototype.GetDigitLength = function() {
        if (this.knownBitLength == null) {
            this.knownBitLength = this.CalcKnownBitLength();
        }
        return FastInteger.Copy(this.knownBitLength);
    };
    prototype.shiftedSmall = null;
    prototype.isSmall = null;
    prototype.getShiftedInt = function() {
        if (this.isSmall) return BigInteger.valueOf(this.shiftedSmall); else return this.shiftedBigInt;
    };
    prototype.discardedBitCount = null;
    prototype.getDiscardedDigitCount = function() {
        return this.discardedBitCount;
    };
    constructor.Int32MaxValue = BigInteger.valueOf(2147483647);
    constructor.FastParseBigInt = function(str, offset, length) {
        
        var mbi = new FastInteger(0);
        for (var i = 0; i < length; i++) {
            var digit = ((str.charAt(offset + i) - '0')|0);
            mbi.Multiply(10).AddInt(digit);
        }
        return mbi.AsBigInteger();
    };
    constructor.FastParseLong = function(str, offset, length) {
        
        if ((length) > 9) throw ("length" + " not less or equal to " + "9" + " (" + (length) + ")");
        var ret = 0;
        for (var i = 0; i < length; i++) {
            var digit = ((str.charAt(offset + i) - '0')|0);
            ret *= 10;
            ret += digit;
        }
        return ret;
    };
    
    prototype.getShiftedIntFast = function() {
        if (this.isSmall) {
            return new FastInteger(this.shiftedSmall);
        } else {
            return FastInteger.FromBig(this.shiftedBigInt);
        }
    };
    
    prototype.ShiftRight = function(fastint) {
        if ((fastint) == null) throw ("fastint");
        if (fastint.signum() <= 0) return;
        if (fastint.CanFitInInt32()) {
            this.ShiftRightInt(fastint.AsInt32());
        } else {
            var bi = fastint.AsBigInteger();
            while (bi.signum() > 0) {
                var count = 1000000;
                if (bi.compareTo(BigInteger.valueOf(1000000)) < 0) {
                    count = bi.intValue();
                }
                this.ShiftRightInt(count);
                bi = bi.subtract(BigInteger.valueOf(count));
            }
        }
    };
    prototype.ShiftRightBig = function(digits) {
        if (digits <= 0) return;
        if (this.shiftedBigInt.signum() == 0) {
            this.discardedBitCount.AddInt(digits);
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = 0;
            this.knownBitLength = new FastInteger(1);
            return;
        }
        var str = this.shiftedBigInt.toString();
        
        var digitLength = str.length;
        var bitDiff = 0;
        if (digits > digitLength) {
            bitDiff = digits - digitLength;
        }
        this.discardedBitCount.AddInt(digits);
        this.bitsAfterLeftmost |= this.bitLeftmost;
        var digitShift = (digitLength < digits ? digitLength : digits);
        if (digits >= digitLength) {
            this.isSmall = true;
            this.shiftedSmall = 0;
            this.knownBitLength = new FastInteger(1);
        } else {
            var newLength = ((digitLength - digitShift)|0);
            this.knownBitLength = new FastInteger(newLength);
            if (newLength <= 9) {
                
                this.isSmall = true;
                this.shiftedSmall = DigitShiftAccumulator.FastParseLong(str, 0, newLength);
            } else {
                this.shiftedBigInt = DigitShiftAccumulator.FastParseBigInt(str, 0, newLength);
            }
        }
        for (var i = str.length - 1; i >= 0; i--) {
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = ((str.charAt(i) - '0')|0);
            digitShift--;
            if (digitShift <= 0) {
                break;
            }
        }
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        if (bitDiff > 0) {
            
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = 0;
        }
    };
    
    prototype.ShiftToBitsBig = function(digits) {
        var str = this.shiftedBigInt.toString();
        
        var digitLength = str.length;
        this.knownBitLength = new FastInteger(digitLength);
        
        if (digitLength > digits) {
            var digitShift = digitLength - digits;
            var bitShiftCount = digitShift;
            var newLength = ((digitLength - digitShift)|0);
            if (digitShift <= 2147483647) this.discardedBitCount.AddInt(digitShift|0); else this.discardedBitCount.AddBig(BigInteger.valueOf(digitShift));
            for (var i = str.length - 1; i >= 0; i--) {
                this.bitsAfterLeftmost |= this.bitLeftmost;
                this.bitLeftmost = ((str.charAt(i) - '0')|0);
                digitShift--;
                if (digitShift <= 0) {
                    break;
                }
            }
            this.knownBitLength = new FastInteger(digits);
            if (newLength <= 9) {
                this.isSmall = true;
                this.shiftedSmall = DigitShiftAccumulator.FastParseLong(str, 0, newLength);
            } else {
                this.shiftedBigInt = DigitShiftAccumulator.FastParseBigInt(str, 0, newLength);
            }
            this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        }
    };
    
    prototype.ShiftRightInt = function(digits) {
        if (this.isSmall) this.ShiftRightSmall(digits); else this.ShiftRightBig(digits);
    };
    prototype.ShiftRightSmall = function(digits) {
        if (digits <= 0) return;
        if (this.shiftedSmall == 0) {
            this.discardedBitCount.AddInt(digits);
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = 0;
            this.knownBitLength = new FastInteger(1);
            return;
        }
        var kb = 0;
        var tmp = this.shiftedSmall;
        while (tmp > 0) {
            kb++;
            tmp = ((tmp / 10)|0);
        }
        
        if (kb == 0) kb++;
        this.knownBitLength = new FastInteger(kb);
        this.discardedBitCount.AddInt(digits);
        while (digits > 0) {
            if (this.shiftedSmall == 0) {
                this.bitsAfterLeftmost |= this.bitLeftmost;
                this.bitLeftmost = 0;
                this.knownBitLength = new FastInteger(0);
                break;
            } else {
                var digit = ((this.shiftedSmall % 10)|0);
                this.bitsAfterLeftmost |= this.bitLeftmost;
                this.bitLeftmost = digit;
                digits--;
                this.shiftedSmall = ((this.shiftedSmall / 10)|0);
                this.knownBitLength.SubtractInt(1);
            }
        }
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
    };
    
    prototype.ShiftToDigits = function(bits) {
        if (bits.signum() < 0) throw ("bits is negative");
        if (bits.CanFitInInt32()) {
            this.ShiftToDigitsInt(bits.AsInt32());
        } else {
            this.knownBitLength = this.CalcKnownBitLength();
            var bigintDiff = this.knownBitLength.AsBigInteger();
            var bitsBig = bits.AsBigInteger();
            bigintDiff = bigintDiff.subtract(bitsBig);
            if (bigintDiff.signum() > 0) {
                
                this.ShiftRight(FastInteger.FromBig(bigintDiff));
            }
        }
    };
    
    prototype.ShiftToDigitsInt = function(digits) {
        if (this.isSmall) this.ShiftToBitsSmall(digits); else this.ShiftToBitsBig(digits);
    };
    prototype.CalcKnownBitLength = function() {
        if (this.isSmall) {
            var kb = 0;
            var tmp = this.shiftedSmall;
            while (tmp > 0) {
                kb++;
                tmp = ((tmp / 10)|0);
            }
            kb = (kb == 0 ? 1 : kb);
            return new FastInteger(kb);
        } else {
            var str = this.shiftedBigInt.toString();
            return new FastInteger(str.length);
        }
    };
    prototype.ShiftToBitsSmall = function(digits) {
        var kb = 0;
        var tmp = this.shiftedSmall;
        while (tmp > 0) {
            kb++;
            tmp = ((tmp / 10)|0);
        }
        
        if (kb == 0) kb++;
        this.knownBitLength = new FastInteger(kb);
        if (kb > digits) {
            var digitShift = ((kb - digits)|0);
            var newLength = ((kb - digitShift)|0);
            this.knownBitLength = new FastInteger(1 > newLength ? 1 : newLength);
            this.discardedBitCount.AddInt(digitShift);
            for (var i = 0; i < digitShift; i++) {
                var digit = ((this.shiftedSmall % 10)|0);
                this.shiftedSmall = ((this.shiftedSmall / 10)|0);
                this.bitsAfterLeftmost |= this.bitLeftmost;
                this.bitLeftmost = digit;
            }
            this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        }
    };
})(DigitShiftAccumulator,DigitShiftAccumulator.prototype);


if(typeof exports!=="undefined")exports.DigitShiftAccumulator=DigitShiftAccumulator;