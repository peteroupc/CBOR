
var BitShiftAccumulator = 

function(bigint, lastDiscarded, olderDiscarded) {

    if (bigint.signum() < 0) throw ("bigint is negative");
    this.shiftedBigInt = bigint;
    this.discardedBitCount = new FastInteger(0);
    this.bitsAfterLeftmost = (olderDiscarded != 0) ? 1 : 0;
    this.bitLeftmost = (lastDiscarded != 0) ? 1 : 0;
};
(function(constructor,prototype){
    prototype.bitLeftmost = null;
    prototype.getLastDiscardedDigit = function() {
        return this.bitLeftmost;
    };
    prototype.bitsAfterLeftmost = null;
    constructor.SmallBitLength = 32;
    prototype.getOlderDiscardedDigits = function() {
        return this.bitsAfterLeftmost;
    };
    prototype.shiftedBigInt = null;
    prototype.knownBitLength = null;
    prototype.GetDigitLength = function() {
        if (this.knownBitLength == null) {
            this.knownBitLength = this.CalcKnownBitLength();
        }
        return FastInteger.Copy(this.knownBitLength);
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
    prototype.shiftedSmall = null;
    prototype.isSmall = null;
    prototype.getShiftedInt = function() {
        if (this.isSmall) return BigInteger.valueOf(this.shiftedSmall); else return this.shiftedBigInt;
    };
    prototype.getShiftedIntFast = function() {
        if (this.isSmall) {
            return new FastInteger(this.shiftedSmall);
        } else {
            return FastInteger.FromBig(this.shiftedBigInt);
        }
    };
    prototype.discardedBitCount = null;
    prototype.getDiscardedDigitCount = function() {
        return this.discardedBitCount;
    };
    constructor.FromInt32 = function(smallNumber) {
        if (smallNumber < 0) throw ("longInt is negative");
        var bsa = new BitShiftAccumulator(BigInteger.ZERO, 0, 0);
        bsa.shiftedSmall = smallNumber;
        bsa.discardedBitCount = new FastInteger(0);
        bsa.isSmall = true;
        return bsa;
    };
    
    prototype.ShiftRight = function(fastint) {
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
    prototype.ShiftRightBig = function(bits) {
        if (bits <= 0) return;
        if (this.shiftedBigInt.signum() == 0) {
            this.discardedBitCount.AddInt(bits);
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = 0;
            this.knownBitLength = new FastInteger(1);
            return;
        }
        var bytes = this.shiftedBigInt.toByteArray(true);
        this.knownBitLength = this.ByteArrayBitLength(bytes);
        var bitDiff = new FastInteger(0);
        var bitShift = null;
        if (this.knownBitLength.CompareToInt(bits) < 0) {
            bitDiff = new FastInteger(bits).Subtract(this.knownBitLength);
            bitShift = FastInteger.Copy(this.knownBitLength);
        } else {
            bitShift = new FastInteger(bits);
        }
        if (this.knownBitLength.CompareToInt(bits) <= 0) {
            this.isSmall = true;
            this.shiftedSmall = 0;
            this.knownBitLength.Multiply(0).AddInt(1);
        } else {
            var tmpBitShift = FastInteger.Copy(bitShift);
            while (tmpBitShift.signum() > 0 && this.shiftedBigInt.signum() != 0) {
                var bs = tmpBitShift.MinInt32(1000000);
                this.shiftedBigInt = this.shiftedBigInt.shiftRight(bs);
                tmpBitShift.SubtractInt(bs);
            }
            this.knownBitLength.Subtract(bitShift);
        }
        this.discardedBitCount.AddInt(bits);
        this.bitsAfterLeftmost |= this.bitLeftmost;
        for (var i = 0; i < bytes.length; i++) {
            if (bitShift.CompareToInt(8) > 0) {
                
                this.bitsAfterLeftmost |= bytes[i];
                bitShift.SubtractInt(8);
            } else {
                
                this.bitsAfterLeftmost |= ((bytes[i] << (9 - bitShift.AsInt32())) & 255);
                
                this.bitLeftmost = (bytes[i] >> ((bitShift.AsInt32()) - 1)) & 1;
                break;
            }
        }
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        if (bitDiff.signum() > 0) {
            
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = 0;
        }
    };
    prototype.ByteArrayBitLength = function(bytes) {
        var fastKB = new FastInteger(bytes.length).Multiply(8);
        for (var i = bytes.length - 1; i >= 0; i--) {
            var b = ((bytes[i])|0);
            if (b != 0) {
                if ((b & 128) != 0) {
                    break;
                }
                if ((b & 64) != 0) {
                    fastKB.SubtractInt(1);
                    break;
                }
                if ((b & 32) != 0) {
                    fastKB.SubtractInt(2);
                    break;
                }
                if ((b & 16) != 0) {
                    fastKB.SubtractInt(3);
                    break;
                }
                if ((b & 8) != 0) {
                    fastKB.SubtractInt(4);
                    break;
                }
                if ((b & 4) != 0) {
                    fastKB.SubtractInt(5);
                    break;
                }
                if ((b & 2) != 0) {
                    fastKB.SubtractInt(6);
                    break;
                }
                if ((b & 1) != 0) {
                    fastKB.SubtractInt(7);
                    break;
                }
            }
            fastKB.SubtractInt(8);
        }
        
        if (fastKB.signum() == 0) fastKB.AddInt(1);
        return fastKB;
    };
    prototype.CalcKnownBitLength = function() {
        if (this.isSmall) {
            var kb = BitShiftAccumulator.SmallBitLength;
            for (var i = BitShiftAccumulator.SmallBitLength - 1; i >= 0; i++) {
                if ((this.shiftedSmall & (1 << i)) != 0) {
                    break;
                } else {
                    kb--;
                }
            }
            
            if (kb == 0) kb++;
            return new FastInteger(kb);
        } else {
            var bytes = this.shiftedBigInt.toByteArray(true);
            
            return this.ByteArrayBitLength(bytes);
        }
    };
    
    prototype.ShiftBigToBits = function(bits) {
        var bytes = this.shiftedBigInt.toByteArray(true);
        this.knownBitLength = this.ByteArrayBitLength(bytes);
        
        if (this.knownBitLength.CompareToInt(bits) > 0) {
            var bitShift = FastInteger.Copy(this.knownBitLength).SubtractInt(bits);
            var tmpBitShift = FastInteger.Copy(bitShift);
            while (tmpBitShift.signum() > 0 && this.shiftedBigInt.signum() != 0) {
                var bs = tmpBitShift.MinInt32(1000000);
                this.shiftedBigInt = this.shiftedBigInt.shiftRight(bs);
                tmpBitShift.SubtractInt(bs);
            }
            this.knownBitLength.Multiply(0).AddInt(bits);
            if (bits < BitShiftAccumulator.SmallBitLength) {
                
                this.isSmall = true;
                this.shiftedSmall = this.shiftedBigInt.intValue();
            }
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.discardedBitCount.Add(bitShift);
            for (var i = 0; i < bytes.length; i++) {
                if (bitShift.CompareToInt(8) > 0) {
                    
                    this.bitsAfterLeftmost |= bytes[i];
                    bitShift.SubtractInt(8);
                } else {
                    
                    this.bitsAfterLeftmost |= ((bytes[i] << (9 - bitShift.AsInt32())) & 255);
                    
                    this.bitLeftmost = (bytes[i] >> ((bitShift.AsInt32()) - 1)) & 1;
                    break;
                }
            }
            this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        }
    };
    
    prototype.ShiftRightInt = function(bits) {
        if (this.isSmall) this.ShiftRightSmall(bits); else this.ShiftRightBig(bits);
    };
    prototype.ShiftRightSmall = function(bits) {
        if (bits <= 0) return;
        if (this.shiftedSmall == 0) {
            this.discardedBitCount.AddInt(bits);
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = 0;
            this.knownBitLength = new FastInteger(1);
            return;
        }
        var kb = BitShiftAccumulator.SmallBitLength;
        for (var i = BitShiftAccumulator.SmallBitLength - 1; i >= 0; i++) {
            if ((this.shiftedSmall & (1 << i)) != 0) {
                break;
            } else {
                kb--;
            }
        }
        var shift = ((kb < bits ? kb : bits)|0);
        var shiftingMoreBits = (bits > kb);
        kb = kb - shift;
        this.knownBitLength = new FastInteger(kb);
        this.discardedBitCount.AddInt(bits);
        this.bitsAfterLeftmost |= this.bitLeftmost;
        
        this.bitsAfterLeftmost |= (((this.shiftedSmall << (BitShiftAccumulator.SmallBitLength + 1 - shift)) != 0) ? 1 : 0);
        
        this.bitLeftmost = ((((this.shiftedSmall >> ((shift) - 1)) & 1))|0);
        this.shiftedSmall >>= shift;
        if (shiftingMoreBits) {
            
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = 0;
        }
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
    };
    
    prototype.ShiftToDigitsInt = function(bits) {
        if (bits < 0) throw ("bits is negative");
        if (this.isSmall) this.ShiftSmallToBits(bits); else this.ShiftBigToBits(bits);
    };
    prototype.ShiftSmallToBits = function(bits) {
        var kbl = 64;
        for (var i = 63; i >= 0; i++) {
            if ((this.shiftedSmall & (1 << i)) != 0) {
                break;
            } else {
                kbl--;
            }
        }
        if (kbl == 0) kbl++;
        
        if (kbl > bits) {
            var bitShift = kbl - (bits|0);
            var shift = (bitShift|0);
            this.knownBitLength = new FastInteger(bits);
            this.discardedBitCount.AddInt(bitShift);
            this.bitsAfterLeftmost |= this.bitLeftmost;
            
            this.bitsAfterLeftmost |= (((this.shiftedSmall << (65 - shift)) != 0) ? 1 : 0);
            
            this.bitLeftmost = ((((this.shiftedSmall >> ((shift|0) - 1)) & 1))|0);
            this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
            this.shiftedSmall >>= shift;
        } else {
            this.knownBitLength = new FastInteger(kbl);
        }
    };
})(BitShiftAccumulator,BitShiftAccumulator.prototype);


if(typeof exports!=="undefined")exports.BitShiftAccumulator=BitShiftAccumulator;